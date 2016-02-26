Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 
Namespace javax.swing




	''' <summary>
	''' A PaintManager implementation that uses a BufferStrategy for
	''' rendering.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class BufferStrategyPaintManager
		Inherits RepaintManager.PaintManager

		'
		' All drawing is done to a BufferStrategy.  At the end of painting
		' (endPaint) the region that was painted is flushed to the screen
		' (using BufferStrategy.show).
		'
		' PaintManager.show is overriden to show directly from the
		' BufferStrategy (when using blit), if successful true is
		' returned and a paint event will not be generated.  To avoid
		' showing from the buffer while painting a locking scheme is
		' implemented.  When beginPaint is invoked the field painting is
		' set to true.  If painting is true and show is invoked we
		' immediately return false.  This is done to avoid blocking the
		' toolkit thread while painting happens.  In a similar way when
		' show is invoked the field showing is set to true, beginPaint
		' will then block until showing is true.  This scheme ensures we
		' only ever have one thread using the BufferStrategy and it also
		' ensures the toolkit thread remains as responsive as possible.
		'
		' If we're using a flip strategy the contents of the backbuffer may
		' have changed and so show only attempts to show from the backbuffer
		' if we get a blit strategy.
		'

		'
		' Methods used to create BufferStrategy for Applets.
		'
		Private Shared COMPONENT_CREATE_BUFFER_STRATEGY_METHOD As Method
		Private Shared COMPONENT_GET_BUFFER_STRATEGY_METHOD As Method

		Private Shared ReadOnly LOGGER As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("javax.swing.BufferStrategyPaintManager")

		''' <summary>
		''' List of BufferInfos.  We don't use a Map primarily because
		''' there are typically only a handful of top level components making
		''' a Map overkill.
		''' </summary>
		Private bufferInfos As List(Of BufferInfo)

		''' <summary>
		''' Indicates <code>beginPaint</code> has been invoked.  This is
		''' set to true for the life of beginPaint/endPaint pair.
		''' </summary>
		Private painting As Boolean
		''' <summary>
		''' Indicates we're in the process of showing.  All painting, on the EDT,
		''' is blocked while this is true.
		''' </summary>
		Private showing As Boolean

		'
		' Region that we need to flush.  When beginPaint is called these are
		' reset and any subsequent calls to paint/copyArea then update these
		' fields accordingly.  When endPaint is called we then try and show
		' the accumulated region.
		' These fields are in the coordinate system of the root.
		'
		Private accumulatedX As Integer
		Private accumulatedY As Integer
		Private accumulatedMaxX As Integer
		Private accumulatedMaxY As Integer

		'
		' The following fields are set by prepare
		'

		''' <summary>
		''' Farthest JComponent ancestor for the current paint/copyArea.
		''' </summary>
		Private rootJ As JComponent
		''' <summary>
		''' Location of component being painted relative to root.
		''' </summary>
		Private xOffset As Integer
		''' <summary>
		''' Location of component being painted relative to root.
		''' </summary>
		Private yOffset As Integer
		''' <summary>
		''' Graphics from the BufferStrategy.
		''' </summary>
		Private bsg As Graphics
		''' <summary>
		''' BufferStrategy currently being used.
		''' </summary>
		Private bufferStrategy As BufferStrategy
		''' <summary>
		''' BufferInfo corresponding to root.
		''' </summary>
		Private bufferInfo As BufferInfo

		''' <summary>
		''' Set to true if the bufferInfo needs to be disposed when current
		''' paint loop is done.
		''' </summary>
		Private disposeBufferOnEnd As Boolean

		Private Property Shared getBufferStrategyMethod As Method
			Get
				If COMPONENT_GET_BUFFER_STRATEGY_METHOD Is Nothing Then methods
				Return COMPONENT_GET_BUFFER_STRATEGY_METHOD
			End Get
		End Property

		Private Property Shared createBufferStrategyMethod As Method
			Get
				If COMPONENT_CREATE_BUFFER_STRATEGY_METHOD Is Nothing Then methods
				Return COMPONENT_CREATE_BUFFER_STRATEGY_METHOD
			End Get
		End Property

		Private Shared Sub getMethods()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'		{
	'			public Object run()
	'			{
	'				try
	'				{
	'					COMPONENT_CREATE_BUFFER_STRATEGY_METHOD = Component.class.getDeclaredMethod("createBufferStrategy", New Class[] { int.class, BufferCapabilities.class });
	'					COMPONENT_CREATE_BUFFER_STRATEGY_METHOD.setAccessible(True);
	'					COMPONENT_GET_BUFFER_STRATEGY_METHOD = Component.class.getDeclaredMethod("getBufferStrategy");
	'					COMPONENT_GET_BUFFER_STRATEGY_METHOD.setAccessible(True);
	'				}
	'				catch (SecurityException e)
	'				{
	'					assert False;
	'				}
	'				catch (NoSuchMethodException nsme)
	'				{
	'					assert False;
	'				}
	'				Return Nothing;
	'			}
	'		});
		End Sub

		Friend Sub New()
			bufferInfos = New List(Of BufferInfo)(1)
		End Sub

		'
		' PaintManager methods
		'

		''' <summary>
		''' Cleans up any created BufferStrategies.
		''' </summary>
		Protected Friend Overridable Sub dispose()
			' dipose can be invoked at any random time. To avoid
			' threading dependancies we do the actual diposing via an
			' invokeLater.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			SwingUtilities.invokeLater(New Runnable()
	'		{
	'			public void run()
	'			{
	'				java.util.List<BufferInfo> bufferInfos;
	'				synchronized(BufferStrategyPaintManager.this)
	'				{
	'					while (showing)
	'					{
	'						try
	'						{
	'							outerInstance.wait();
	'						}
	'						catch (InterruptedException ie)
	'						{
	'						}
	'					}
	'					bufferInfos = outerInstance.bufferInfos;
	'					outerInstance.bufferInfos = Nothing;
	'				}
	'				dispose(bufferInfos);
	'			}
	'		});
		End Sub

		Private Sub dispose(ByVal bufferInfos As IList(Of BufferInfo))
			If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("BufferStrategyPaintManager disposed", New Exception)
			If bufferInfos IsNot Nothing Then
				For Each ___bufferInfo As BufferInfo In bufferInfos
					___bufferInfo.Dispose()
				Next ___bufferInfo
			End If
		End Sub

		''' <summary>
		''' Shows the specified region of the back buffer.  This will return
		''' true if successful, false otherwise.  This is invoked on the
		''' toolkit thread in response to an expose event.
		''' </summary>
		Public Overridable Function show(ByVal c As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			SyncLock Me
				If painting Then Return False
				showing = True
			End SyncLock
			Try
				Dim info As BufferInfo = getBufferInfo(c)
				Dim bufferStrategy As BufferStrategy
				bufferStrategy = info.getBufferStrategy(False)
				If info IsNot Nothing AndAlso info.inSync AndAlso bufferStrategy IsNot Nothing Then
					Dim bsSubRegion As sun.awt.SubRegionShowable = CType(bufferStrategy, sun.awt.SubRegionShowable)
					Dim paintAllOnExpose As Boolean = info.paintAllOnExpose
					info.paintAllOnExpose = False
					If bsSubRegion.showIfNotLost(x, y, (x + w), (y + h)) Then Return Not paintAllOnExpose
					' Mark the buffer as needing to be repainted.  We don't
					' immediately do a repaint as this method will return false
					' indicating a PaintEvent should be generated which will
					' trigger a complete repaint.
					bufferInfo.contentsLostDuringExpose = True
				End If
			Finally
				SyncLock Me
					showing = False
					notifyAll()
				End SyncLock
			End Try
			Return False
		End Function

		Public Overridable Function paint(ByVal paintingComponent As JComponent, ByVal bufferComponent As JComponent, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			Dim root As Container = fetchRoot(paintingComponent)

			If prepare(paintingComponent, root, True, x, y, w, h) Then
				If (TypeOf g Is sun.java2d.SunGraphics2D) AndAlso CType(g, sun.java2d.SunGraphics2D).destination Is root Then
					' BufferStrategy may have already constrained the Graphics. To
					' account for that we revert the constrain, then apply a
					' constrain for Swing on top of that.
					Dim cx As Integer = CType(bsg, sun.java2d.SunGraphics2D).constrainX
					Dim cy As Integer = CType(bsg, sun.java2d.SunGraphics2D).constrainY
					If cx <> 0 OrElse cy <> 0 Then bsg.translate(-cx, -cy)
					CType(bsg, sun.java2d.SunGraphics2D).constrain(xOffset + cx, yOffset + cy, x + w, y + h)
					bsg.cliplip(x, y, w, h)
					paintingComponent.paintToOffscreen(bsg, x, y, w, h, x + w, y + h)
					accumulate(xOffset + x, yOffset + y, w, h)
					Return True
				Else
					' Assume they are going to eventually render to the screen.
					' This disables showing from backbuffer until a complete
					' repaint occurs.
					bufferInfo.inSync = False
					' Fall through to old rendering.
				End If
			End If
			' Invalid root, do what Swing has always done.
			If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("prepare failed")
			Return MyBase.paint(paintingComponent, bufferComponent, g, x, y, w, h)
		End Function

		Public Overridable Sub copyArea(ByVal c As JComponent, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal deltaX As Integer, ByVal deltaY As Integer, ByVal clip As Boolean)
			' Note: this method is only called internally and we know that
			' g is from a heavyweight Component, so no check is necessary as
			' it is in paint() above.
			'
			' If the buffer isn't in sync there is no point in doing a copyArea,
			' it has garbage.
			Dim root As Container = fetchRoot(c)

			If prepare(c, root, False, 0, 0, 0, 0) AndAlso bufferInfo.inSync Then
				If clip Then
					Dim cBounds As Rectangle = c.visibleRect
					Dim relX As Integer = xOffset + x
					Dim relY As Integer = yOffset + y
					bsg.clipRect(xOffset + cBounds.x, yOffset + cBounds.y, cBounds.width, cBounds.height)
					bsg.copyArea(relX, relY, w, h, deltaX, deltaY)
				Else
					bsg.copyArea(xOffset + x, yOffset + y, w, h, deltaX, deltaY)
				End If
				accumulate(x + xOffset + deltaX, y + yOffset + deltaY, w, h)
			Else
				If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("copyArea: prepare failed or not in sync")
				' Prepare failed, or not in sync. By calling super.copyArea
				' we'll copy on screen. We need to flush any pending paint to
				' the screen otherwise we'll do a copyArea on the wrong thing.
				If Not flushAccumulatedRegion() Then
					' Flush failed, copyArea will be copying garbage,
					' force repaint of all.
					rootJ.repaint()
				Else
					MyBase.copyArea(c, g, x, y, w, h, deltaX, deltaY, clip)
				End If
			End If
		End Sub

		Public Overridable Sub beginPaint()
			SyncLock Me
				painting = True
				' Make sure another thread isn't attempting to show from
				' the back buffer.
				Do While showing
					Try
						wait()
					Catch ie As InterruptedException
					End Try
				Loop
			End SyncLock
			If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then LOGGER.finest("beginPaint")
			' Reset the area that needs to be painted.
			resetAccumulated()
		End Sub

		Public Overridable Sub endPaint()
			If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then LOGGER.finest("endPaint: region " & accumulatedX & " " & accumulatedY & " " & accumulatedMaxX & " " & accumulatedMaxY)
			If painting Then
				If Not flushAccumulatedRegion() Then
					If Not repaintingRoot Then
						repaintRoot(rootJ)
					Else
						' Contents lost twice in a row, punt.
						resetDoubleBufferPerWindow()
						' In case we've left junk on the screen, force a repaint.
						rootJ.repaint()
					End If
				End If
			End If

			Dim toDispose As BufferInfo = Nothing
			SyncLock Me
				painting = False
				If disposeBufferOnEnd Then
					disposeBufferOnEnd = False
					toDispose = bufferInfo
					bufferInfos.Remove(toDispose)
				End If
			End SyncLock
			If toDispose IsNot Nothing Then toDispose.Dispose()
		End Sub

		''' <summary>
		''' Renders the BufferStrategy to the screen.
		''' </summary>
		''' <returns> true if successful, false otherwise. </returns>
		Private Function flushAccumulatedRegion() As Boolean
			Dim success As Boolean = True
			If accumulatedX <> Integer.MaxValue Then
				Dim bsSubRegion As sun.awt.SubRegionShowable = CType(bufferStrategy, sun.awt.SubRegionShowable)
				Dim contentsLost As Boolean = bufferStrategy.contentsLost()
				If Not contentsLost Then
					bsSubRegion.show(accumulatedX, accumulatedY, accumulatedMaxX, accumulatedMaxY)
					contentsLost = bufferStrategy.contentsLost()
				End If
				If contentsLost Then
					If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("endPaint: contents lost")
					' Shown region was bogus, mark buffer as out of sync.
					bufferInfo.inSync = False
					success = False
				End If
			End If
			resetAccumulated()
			Return success
		End Function

		Private Sub resetAccumulated()
			accumulatedX = Integer.MaxValue
			accumulatedY = Integer.MaxValue
			accumulatedMaxX = 0
			accumulatedMaxY = 0
		End Sub

		''' <summary>
		''' Invoked when the double buffering or useTrueDoubleBuffering
		''' changes for a JRootPane.  If the rootpane is not double
		''' buffered, or true double buffering changes we throw out any
		''' cache we may have.
		''' </summary>
		Public Overridable Sub doubleBufferingChanged(ByVal rootPane As JRootPane)
			If ((Not rootPane.doubleBuffered) OrElse (Not rootPane.useTrueDoubleBuffering)) AndAlso rootPane.parent IsNot Nothing Then
				If Not SwingUtilities.eventDispatchThread Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Runnable updater = New Runnable()
	'				{
	'					public void run()
	'					{
	'						doubleBufferingChanged0(rootPane);
	'					}
	'				};
					SwingUtilities.invokeLater(updater)
				Else
					doubleBufferingChanged0(rootPane)
				End If
			End If
		End Sub

		''' <summary>
		''' Does the work for doubleBufferingChanged.
		''' </summary>
		Private Sub doubleBufferingChanged0(ByVal rootPane As JRootPane)
			' This will only happen on the EDT.
			Dim info As BufferInfo
			SyncLock Me
				' Make sure another thread isn't attempting to show from
				' the back buffer.
				Do While showing
					Try
						wait()
					Catch ie As InterruptedException
					End Try
				Loop
				info = getBufferInfo(rootPane.parent)
				If painting AndAlso bufferInfo Is info Then
					' We're in the process of painting and the user grabbed
					' the Graphics. If we dispose now, endPaint will attempt
					' to show a bogus BufferStrategy. Set a flag so that
					' endPaint knows it needs to dispose this buffer.
					disposeBufferOnEnd = True
					info = Nothing
				ElseIf info IsNot Nothing Then
					bufferInfos.Remove(info)
				End If
			End SyncLock
			If info IsNot Nothing Then info.Dispose()
		End Sub

		''' <summary>
		''' Calculates information common to paint/copyArea.
		''' </summary>
		''' <returns> true if should use buffering per window in painting. </returns>
		Private Function prepare(ByVal c As JComponent, ByVal root As Container, ByVal isPaint As Boolean, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			If bsg IsNot Nothing Then
				bsg.Dispose()
				bsg = Nothing
			End If
			bufferStrategy = Nothing
			If root IsNot Nothing Then
				Dim contentsLost As Boolean = False
				Dim ___bufferInfo As BufferInfo = getBufferInfo(root)
				If ___bufferInfo Is Nothing Then
					contentsLost = True
					___bufferInfo = New BufferInfo(Me, root)
					bufferInfos.Add(___bufferInfo)
					If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("prepare: new BufferInfo: " & root)
				End If
				Me.bufferInfo = ___bufferInfo
				If Not ___bufferInfo.hasBufferStrategyChanged() Then
					bufferStrategy = ___bufferInfo.getBufferStrategy(True)
					If bufferStrategy IsNot Nothing Then
						bsg = bufferStrategy.drawGraphics
						If bufferStrategy.contentsRestored() Then
							contentsLost = True
							If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("prepare: contents restored in prepare")
						End If
					Else
						' Couldn't create BufferStrategy, fallback to normal
						' painting.
						Return False
					End If
					If ___bufferInfo.contentsLostDuringExpose Then
						contentsLost = True
						___bufferInfo.contentsLostDuringExpose = False
						If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("prepare: contents lost on expose")
					End If
					If isPaint AndAlso c Is rootJ AndAlso x = 0 AndAlso y = 0 AndAlso c.width = w AndAlso c.height = h Then
						___bufferInfo.inSync = True
					ElseIf contentsLost Then
						' We either recreated the BufferStrategy, or the contents
						' of the buffer strategy were restored.  We need to
						' repaint the root pane so that the back buffer is in sync
						' again.
						___bufferInfo.inSync = False
						If Not repaintingRoot Then
							repaintRoot(rootJ)
						Else
							' Contents lost twice in a row, punt
							resetDoubleBufferPerWindow()
						End If
					End If
					Return (bufferInfos IsNot Nothing)
				End If
			End If
			Return False
		End Function

		Private Function fetchRoot(ByVal c As JComponent) As Container
			Dim encounteredHW As Boolean = False
			rootJ = c
			Dim root As Container = c
				yOffset = 0
				xOffset = yOffset
			Do While root IsNot Nothing AndAlso (Not(TypeOf root Is Window) AndAlso (Not sun.awt.SunToolkit.isInstanceOf(root, "java.applet.Applet")))
				xOffset += root.x
				yOffset += root.y
				root = root.parent
				If root IsNot Nothing Then
					If TypeOf root Is JComponent Then
						rootJ = CType(root, JComponent)
					ElseIf Not root.lightweight Then
						If Not encounteredHW Then
							encounteredHW = True
						Else
							' We've encountered two hws now and may have
							' a containment hierarchy with lightweights containing
							' heavyweights containing other lightweights.
							' Heavyweights poke holes in lightweight
							' rendering so that if we call show on the BS
							' (which is associated with the Window) you will
							' not see the contents over any child
							' heavyweights.  If we didn't do this when we
							' went to show the descendants of the nested hw
							' you would see nothing, so, we bail out here.
							Return Nothing
						End If
					End If
				End If
			Loop
			If (TypeOf root Is RootPaneContainer) AndAlso (TypeOf rootJ Is JRootPane) Then
				' We're in a Swing heavyeight (JFrame/JWindow...), use double
				' buffering if double buffering enabled on the JRootPane and
				' the JRootPane wants true double buffering.
				If rootJ.doubleBuffered AndAlso CType(rootJ, JRootPane).useTrueDoubleBuffering Then Return root
			End If
			' Don't do true double buffering.
			Return Nothing
		End Function

		''' <summary>
		''' Turns off double buffering per window.
		''' </summary>
		Private Sub resetDoubleBufferPerWindow()
			If bufferInfos IsNot Nothing Then
				Dispose(bufferInfos)
				bufferInfos = Nothing
				___repaintManager.paintManager = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns the BufferInfo for the specified root or null if one
		''' hasn't been created yet.
		''' </summary>
		Private Function getBufferInfo(ByVal root As Container) As BufferInfo
			For counter As Integer = bufferInfos.Count - 1 To 0 Step -1
				Dim ___bufferInfo As BufferInfo = bufferInfos(counter)
				Dim biRoot As Container = ___bufferInfo.root
				If biRoot Is Nothing Then
					' Window gc'ed
					bufferInfos.RemoveAt(counter)
					If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("BufferInfo pruned, root null")
				ElseIf biRoot Is root Then
					Return ___bufferInfo
				End If
			Next counter
			Return Nothing
		End Function

		Private Sub accumulate(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			accumulatedX = Math.Min(x, accumulatedX)
			accumulatedY = Math.Min(y, accumulatedY)
			accumulatedMaxX = Math.Max(accumulatedMaxX, x + w)
			accumulatedMaxY = Math.Max(accumulatedMaxY, y + h)
		End Sub



		''' <summary>
		''' BufferInfo is used to track the BufferStrategy being used for
		''' a particular Component.  In addition to tracking the BufferStrategy
		''' it will install a WindowListener and ComponentListener.  When the
		''' component is hidden/iconified the buffer is marked as needing to be
		''' completely repainted.
		''' </summary>
		Private Class BufferInfo
			Inherits ComponentAdapter
			Implements WindowListener

			Private ReadOnly outerInstance As BufferStrategyPaintManager

			' NOTE: This class does NOT hold a direct reference to the root, if it
			' did there would be a cycle between the BufferPerWindowPaintManager
			' and the Window so that it could never be GC'ed
			'
			' Reference to BufferStrategy is referenced via WeakReference for
			' same reason.
			Private weakBS As WeakReference(Of BufferStrategy)
			Private root As WeakReference(Of Container)
			' Indicates whether or not the backbuffer and display are in sync.
			' This is set to true when a full repaint on the rootpane is done.
			Private inSync As Boolean
			' Indicates the contents were lost during and expose event.
			Private contentsLostDuringExpose As Boolean
			' Indicates we need to generate a paint event on expose.
			Private paintAllOnExpose As Boolean


			Public Sub New(ByVal outerInstance As BufferStrategyPaintManager, ByVal root As Container)
					Me.outerInstance = outerInstance
				Me.root = New WeakReference(Of Container)(root)
				root.addComponentListener(Me)
				If TypeOf root Is Window Then CType(root, Window).addWindowListener(Me)
			End Sub

			Public Overridable Property paintAllOnExpose As Boolean
				Set(ByVal paintAllOnExpose As Boolean)
					Me.paintAllOnExpose = paintAllOnExpose
				End Set
				Get
					Return paintAllOnExpose
				End Get
			End Property


			Public Overridable Property contentsLostDuringExpose As Boolean
				Set(ByVal value As Boolean)
					contentsLostDuringExpose = value
				End Set
				Get
					Return contentsLostDuringExpose
				End Get
			End Property


			Public Overridable Property inSync As Boolean
				Set(ByVal inSync As Boolean)
					Me.inSync = inSync
				End Set
				Get
					Return inSync
				End Get
			End Property


			''' <summary>
			''' Returns the Root (Window or Applet) that this BufferInfo references.
			''' </summary>
			Public Overridable Property root As Container
				Get
					Return If(root Is Nothing, Nothing, root.get())
				End Get
			End Property

			''' <summary>
			''' Returns the BufferStartegy.  This will return null if
			''' the BufferStartegy hasn't been created and <code>create</code> is
			''' false, or if there is a problem in creating the
			''' <code>BufferStartegy</code>.
			''' </summary>
			''' <param name="create"> If true, and the BufferStartegy is currently null,
			'''               one will be created. </param>
			Public Overridable Function getBufferStrategy(ByVal create As Boolean) As BufferStrategy
				Dim bs As BufferStrategy = If(weakBS Is Nothing, Nothing, weakBS.get())
				If bs Is Nothing AndAlso create Then
					bs = createBufferStrategy()
					If bs IsNot Nothing Then weakBS = New WeakReference(Of BufferStrategy)(bs)
					If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("getBufferStrategy: created bs: " & bs)
				End If
				Return bs
			End Function

			''' <summary>
			''' Returns true if the buffer strategy of the component differs
			''' from current buffer strategy.
			''' </summary>
			Public Overridable Function hasBufferStrategyChanged() As Boolean
				Dim ___root As Container = root
				If ___root IsNot Nothing Then
					Dim ourBS As BufferStrategy = Nothing
					Dim componentBS As BufferStrategy = Nothing

					ourBS = getBufferStrategy(False)
					If TypeOf ___root Is Window Then
						componentBS = CType(___root, Window).bufferStrategy
					Else
						Try
							componentBS = CType(getBufferStrategyMethod.invoke(___root), BufferStrategy)
						Catch ite As InvocationTargetException
							Debug.Assert(False)
						Catch iae As System.ArgumentException
							Debug.Assert(False)
						Catch iae2 As IllegalAccessException
							Debug.Assert(False)
						End Try
					End If
					If componentBS IsNot ourBS Then
						' Component has a different BS, dispose ours.
						If ourBS IsNot Nothing Then ourBS.Dispose()
						weakBS = Nothing
						Return True
					End If
				End If
				Return False
			End Function

			''' <summary>
			''' Creates the BufferStrategy.  If the appropriate system property
			''' has been set we'll try for flip first and then we'll try for
			''' blit.
			''' </summary>
			Private Function createBufferStrategy() As BufferStrategy
				Dim ___root As Container = root
				If ___root Is Nothing Then Return Nothing
				Dim bs As BufferStrategy = Nothing
				If com.sun.java.swing.SwingUtilities3.isVsyncRequested(___root) Then
					bs = createBufferStrategy(___root, True)
					If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("createBufferStrategy: using vsynced strategy")
				End If
				If bs Is Nothing Then bs = createBufferStrategy(___root, False)
				If Not(TypeOf bs Is sun.awt.SubRegionShowable) Then bs = Nothing
				Return bs
			End Function

			' Creates and returns a buffer strategy.  If
			' there is a problem creating the buffer strategy this will
			' eat the exception and return null.
			Private Function createBufferStrategy(ByVal root As Container, ByVal isVsynced As Boolean) As BufferStrategy
				Dim caps As BufferCapabilities
				If isVsynced Then
					caps = New sun.java2d.pipe.hw.ExtendedBufferCapabilities(New ImageCapabilities(True), New ImageCapabilities(True), BufferCapabilities.FlipContents.COPIED, sun.java2d.pipe.hw.ExtendedBufferCapabilities.VSyncType.VSYNC_ON)
				Else
					caps = New BufferCapabilities(New ImageCapabilities(True), New ImageCapabilities(True), Nothing)
				End If
				Dim bs As BufferStrategy = Nothing
				If sun.awt.SunToolkit.isInstanceOf(root, "java.applet.Applet") Then
					Try
						createBufferStrategyMethod.invoke(root, 2, caps)
						bs = CType(getBufferStrategyMethod.invoke(root), BufferStrategy)
					Catch ite As InvocationTargetException
						' Type is not supported
						If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("createBufferStratety failed", ite)
					Catch iae As System.ArgumentException
						Debug.Assert(False)
					Catch iae2 As IllegalAccessException
						Debug.Assert(False)
					End Try
				Else
					Try
						CType(root, Window).createBufferStrategy(2, caps)
						bs = CType(root, Window).bufferStrategy
					Catch e As AWTException
						' Type not supported
						If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("createBufferStratety failed", e)
					End Try
				End If
				Return bs
			End Function

			''' <summary>
			''' Cleans up and removes any references.
			''' </summary>
			Public Overridable Sub dispose()
				Dim ___root As Container = root
				If LOGGER.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then LOGGER.finer("disposed BufferInfo for: " & ___root)
				If ___root IsNot Nothing Then
					___root.removeComponentListener(Me)
					If TypeOf ___root Is Window Then CType(___root, Window).removeWindowListener(Me)
					Dim bs As BufferStrategy = getBufferStrategy(False)
					If bs IsNot Nothing Then bs.Dispose()
				End If
				Me.root = Nothing
				weakBS = Nothing
			End Sub

			' We mark the buffer as needing to be painted on a hide/iconify
			' because the developer may have conditionalized painting based on
			' visibility.
			' Ideally we would also move to having the BufferStrategy being
			' a SoftReference in Component here, but that requires changes to
			' Component and the like.
			Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
				Dim ___root As Container = root
				If ___root IsNot Nothing AndAlso ___root.visible Then
					' This case will only happen if a developer calls
					' hide immediately followed by show.  In this case
					' the event is delivered after show and the window
					' will still be visible.  If a developer altered the
					' contents of the window between the hide/show
					' invocations we won't recognize we need to paint and
					' the contents would be bogus.  Calling repaint here
					' fixs everything up.
					___root.repaint()
				Else
					paintAllOnExpose = True
				End If
			End Sub

			Public Overridable Sub windowIconified(ByVal e As WindowEvent)
				paintAllOnExpose = True
			End Sub

			' On a dispose we chuck everything.
			Public Overridable Sub windowClosed(ByVal e As WindowEvent)
				' Make sure we're not showing.
				SyncLock BufferStrategyPaintManager.this
					Do While outerInstance.showing
						Try
							outerInstance.wait()
						Catch ie As InterruptedException
						End Try
					Loop
					outerInstance.bufferInfos.Remove(Me)
				End SyncLock
				Dispose()
			End Sub

			Public Overridable Sub windowOpened(ByVal e As WindowEvent)
			End Sub

			Public Overridable Sub windowClosing(ByVal e As WindowEvent)
			End Sub

			Public Overridable Sub windowDeiconified(ByVal e As WindowEvent)
			End Sub

			Public Overridable Sub windowActivated(ByVal e As WindowEvent)
			End Sub

			Public Overridable Sub windowDeactivated(ByVal e As WindowEvent)
			End Sub
		End Class
	End Class

End Namespace
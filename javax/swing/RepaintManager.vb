Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' This class manages repaint requests, allowing the number
	''' of repaints to be minimized, for example by collapsing multiple
	''' requests into a single repaint for members of a component tree.
	''' <p>
	''' As of 1.6 <code>RepaintManager</code> handles repaint requests
	''' for Swing's top level components (<code>JApplet</code>,
	''' <code>JWindow</code>, <code>JFrame</code> and <code>JDialog</code>).
	''' Any calls to <code>repaint</code> on one of these will call into the
	''' appropriate <code>addDirtyRegion</code> method.
	''' 
	''' @author Arnaud Weber
	''' </summary>
	Public Class RepaintManager
		''' <summary>
		''' Whether or not the RepaintManager should handle paint requests
		''' for top levels.
		''' </summary>
		Friend Shared ReadOnly HANDLE_TOP_LEVEL_PAINT As Boolean

		Private Const BUFFER_STRATEGY_NOT_SPECIFIED As Short = 0
		Private Const BUFFER_STRATEGY_SPECIFIED_ON As Short = 1
		Private Const BUFFER_STRATEGY_SPECIFIED_OFF As Short = 2

		Private Shared ReadOnly BUFFER_STRATEGY_TYPE As Short

		''' <summary>
		''' Maps from GraphicsConfiguration to VolatileImage.
		''' </summary>
		Private volatileMap As IDictionary(Of GraphicsConfiguration, java.awt.image.VolatileImage) = New Dictionary(Of GraphicsConfiguration, java.awt.image.VolatileImage)(1)

		'
		' As of 1.6 Swing handles scheduling of paint events from native code.
		' That is, SwingPaintEventDispatcher is invoked on the toolkit thread,
		' which in turn invokes nativeAddDirtyRegion.  Because this is invoked
		' from the native thread we can not invoke any public methods and so
		' we introduce these added maps.  So, any time nativeAddDirtyRegion is
		' invoked the region is added to hwDirtyComponents and a work request
		' is scheduled.  When the work request is processed all entries in
		' this map are pushed to the real map (dirtyComponents) and then
		' painted with the rest of the components.
		'
		Private hwDirtyComponents As IDictionary(Of Container, Rectangle)

		Private dirtyComponents As IDictionary(Of Component, Rectangle)
		Private tmpDirtyComponents As IDictionary(Of Component, Rectangle)
		Private invalidComponents As IList(Of Component)

		' List of Runnables that need to be processed before painting from AWT.
		Private runnableList As IList(Of Runnable)

		Friend doubleBufferingEnabled As Boolean = True

		Private doubleBufferMaxSize As Dimension

		' Support for both the standard and volatile offscreen buffers exists to
		' provide backwards compatibility for the [rare] programs which may be
		' calling getOffScreenBuffer() and not expecting to get a VolatileImage.
		' Swing internally is migrating to use *only* the volatile image buffer.

		' Support for standard offscreen buffer
		'
		Friend standardDoubleBuffer As DoubleBufferInfo

		''' <summary>
		''' Object responsible for hanlding core paint functionality.
		''' </summary>
		Private paintManager As PaintManager

		Private Shared ReadOnly repaintManagerKey As Object = GetType(RepaintManager)

		' Whether or not a VolatileImage should be used for double-buffered painting
		Friend Shared volatileImageBufferEnabled As Boolean = True
		''' <summary>
		''' Type of VolatileImage which should be used for double-buffered
		''' painting.
		''' </summary>
		Private Shared ReadOnly volatileBufferType As Integer
		''' <summary>
		''' Value of the system property awt.nativeDoubleBuffering.
		''' </summary>
		Private Shared nativeDoubleBuffering As Boolean

		' The maximum number of times Swing will attempt to use the VolatileImage
		' buffer during a paint operation.
		Private Const VOLATILE_LOOP_MAX As Integer = 2

		''' <summary>
		''' Number of <code>beginPaint</code> that have been invoked.
		''' </summary>
		Private paintDepth As Integer = 0

		''' <summary>
		''' Type of buffer strategy to use.  Will be one of the BUFFER_STRATEGY_
		''' constants.
		''' </summary>
		Private bufferStrategyType As Short

		'
		' BufferStrategyPaintManager has the unique characteristic that it
		' must deal with the buffer being lost while painting to it.  For
		' example, if we paint a component and show it and the buffer has
		' become lost we must repaint the whole window.  To deal with that
		' the PaintManager calls into repaintRoot, and if we're still in
		' the process of painting the repaintRoot field is set to the JRootPane
		' and after the current JComponent.paintImmediately call finishes
		' paintImmediately will be invoked on the repaintRoot.  In this
		' way we don't try to show garbage to the screen.
		'
		''' <summary>
		''' True if we're in the process of painting the dirty regions.  This is
		''' set to true in <code>paintDirtyRegions</code>.
		''' </summary>
		Private painting As Boolean
		''' <summary>
		''' If the PaintManager calls into repaintRoot during painting this field
		''' will be set to the root.
		''' </summary>
		Private repaintRoot As JComponent

		''' <summary>
		''' The Thread that has initiated painting.  If null it
		''' indicates painting is not currently in progress.
		''' </summary>
		Private paintThread As Thread

		''' <summary>
		''' Runnable used to process all repaint/revalidate requests.
		''' </summary>
		Private ReadOnly processingRunnable As ProcessingRunnable

		Private Shared ReadOnly javaSecurityAccess As sun.misc.JavaSecurityAccess = sun.misc.SharedSecrets.javaSecurityAccess

		''' <summary>
		''' Listener installed to detect display changes. When display changes,
		''' schedules a callback to notify all RepaintManagers of the display
		''' changes.
		''' </summary>
		Private Shared ReadOnly displayChangedHandler As sun.awt.DisplayChangedListener = New DisplayChangedHandler

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.swing.SwingAccessor.setRepaintManagerAccessor(New sun.swing.SwingAccessor.RepaintManagerAccessor()
	'		{
	'			@Override public void addRepaintListener(RepaintManager rm, RepaintListener l)
	'			{
	'				rm.addRepaintListener(l);
	'			}
	'			@Override public void removeRepaintListener(RepaintManager rm, RepaintListener l)
	'			{
	'				rm.removeRepaintListener(l);
	'			}
	'		});

			volatileImageBufferEnabled = "true".Equals(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.volatileImageBufferEnabled", "true")))
			Dim headless As Boolean = GraphicsEnvironment.headless
			If volatileImageBufferEnabled AndAlso headless Then volatileImageBufferEnabled = False
			nativeDoubleBuffering = "true".Equals(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("awt.nativeDoubleBuffering")))
			Dim bs As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.bufferPerWindow"))
			If headless Then
				BUFFER_STRATEGY_TYPE = BUFFER_STRATEGY_SPECIFIED_OFF
			ElseIf bs Is Nothing Then
				BUFFER_STRATEGY_TYPE = BUFFER_STRATEGY_NOT_SPECIFIED
			ElseIf "true".Equals(bs) Then
				BUFFER_STRATEGY_TYPE = BUFFER_STRATEGY_SPECIFIED_ON
			Else
				BUFFER_STRATEGY_TYPE = BUFFER_STRATEGY_SPECIFIED_OFF
			End If
			HANDLE_TOP_LEVEL_PAINT = "true".Equals(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.handleTopLevelPaint", "true")))
			Dim ge As GraphicsEnvironment = GraphicsEnvironment.localGraphicsEnvironment
			If TypeOf ge Is sun.java2d.SunGraphicsEnvironment Then CType(ge, sun.java2d.SunGraphicsEnvironment).addDisplayChangedListener(displayChangedHandler)
			Dim tk As Toolkit = Toolkit.defaultToolkit
			If (TypeOf tk Is sun.awt.SunToolkit) AndAlso CType(tk, sun.awt.SunToolkit).swingBackbufferTranslucencySupported Then
				volatileBufferType = Transparency.TRANSLUCENT
			Else
				volatileBufferType = Transparency.OPAQUE
			End If
		End Sub

		''' <summary>
		''' Return the RepaintManager for the calling thread given a Component.
		''' </summary>
		''' <param name="c"> a Component -- unused in the default implementation, but could
		'''          be used by an overridden version to return a different RepaintManager
		'''          depending on the Component </param>
		''' <returns> the RepaintManager object </returns>
		Public Shared Function currentManager(ByVal c As Component) As RepaintManager
			' Note: DisplayChangedRunnable passes in null as the component, so if
			' component is ever used to determine the current
			' RepaintManager, DisplayChangedRunnable will need to be modified
			' accordingly.
			Return currentManager(sun.awt.AppContext.appContext)
		End Function

		''' <summary>
		''' Returns the RepaintManager for the specified AppContext.  If
		''' a RepaintManager has not been created for the specified
		''' AppContext this will return null.
		''' </summary>
		Shared Function currentManager(ByVal appContext As sun.awt.AppContext) As RepaintManager
			Dim rm As RepaintManager = CType(appContext.get(repaintManagerKey), RepaintManager)
			If rm Is Nothing Then
				rm = New RepaintManager(BUFFER_STRATEGY_TYPE)
				appContext.put(repaintManagerKey, rm)
			End If
			Return rm
		End Function

		''' <summary>
		''' Return the RepaintManager for the calling thread given a JComponent.
		''' <p>
		''' Note: This method exists for backward binary compatibility with earlier
		''' versions of the Swing library. It simply returns the result returned by
		''' <seealso cref="#currentManager(Component)"/>.
		''' </summary>
		''' <param name="c"> a JComponent -- unused </param>
		''' <returns> the RepaintManager object </returns>
		Public Shared Function currentManager(ByVal c As JComponent) As RepaintManager
			Return currentManager(CType(c, Component))
		End Function


		''' <summary>
		''' Set the RepaintManager that should be used for the calling
		''' thread. <b>aRepaintManager</b> will become the current RepaintManager
		''' for the calling thread's thread group. </summary>
		''' <param name="aRepaintManager">  the RepaintManager object to use </param>
		Public Shared Property currentManager As RepaintManager
			Set(ByVal aRepaintManager As RepaintManager)
				If aRepaintManager IsNot Nothing Then
					SwingUtilities.appContextPut(repaintManagerKey, aRepaintManager)
				Else
					SwingUtilities.appContextRemove(repaintManagerKey)
				End If
			End Set
		End Property

		''' <summary>
		''' Create a new RepaintManager instance. You rarely call this constructor.
		''' directly. To get the default RepaintManager, use
		''' RepaintManager.currentManager(JComponent) (normally "this").
		''' </summary>
		Public Sub New()
			' Because we can't know what a subclass is doing with the
			' volatile image we immediately punt in subclasses.  If this
			' poses a problem we'll need a more sophisticated detection algorithm,
			' or API.
			Me.New(BUFFER_STRATEGY_SPECIFIED_OFF)
		End Sub

		Private Sub New(ByVal bufferStrategyType As Short)
			' If native doublebuffering is being used, do NOT use
			' Swing doublebuffering.
			doubleBufferingEnabled = Not nativeDoubleBuffering
			SyncLock Me
				dirtyComponents = New IdentityHashMap(Of Component, Rectangle)
				tmpDirtyComponents = New IdentityHashMap(Of Component, Rectangle)
				Me.bufferStrategyType = bufferStrategyType
				hwDirtyComponents = New IdentityHashMap(Of Container, Rectangle)
			End SyncLock
			processingRunnable = New ProcessingRunnable(Me)
		End Sub

		Private Sub displayChanged()
			clearImages()
		End Sub

		''' <summary>
		''' Mark the component as in need of layout and queue a runnable
		''' for the event dispatching thread that will validate the components
		''' first isValidateRoot() ancestor.
		''' </summary>
		''' <seealso cref= JComponent#isValidateRoot </seealso>
		''' <seealso cref= #removeInvalidComponent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addInvalidComponent(ByVal invalidComponent As JComponent)
			Dim ___delegate As RepaintManager = getDelegate(invalidComponent)
			If ___delegate IsNot Nothing Then
				___delegate.addInvalidComponent(invalidComponent)
				Return
			End If
			Dim validateRoot As Component = SwingUtilities.getValidateRoot(invalidComponent, True)

			If validateRoot Is Nothing Then Return

	'         Lazily create the invalidateComponents vector and add the
	'         * validateRoot if it's not there already.  If this validateRoot
	'         * is already in the vector, we're done.
	'         
			If invalidComponents Is Nothing Then
				invalidComponents = New List(Of Component)
			Else
				Dim n As Integer = invalidComponents.Count
				For i As Integer = 0 To n - 1
					If validateRoot Is invalidComponents(i) Then Return
				Next i
			End If
			invalidComponents.Add(validateRoot)

			' Queue a Runnable to invoke paintDirtyRegions and
			' validateInvalidComponents.
			scheduleProcessingRunnable(sun.awt.SunToolkit.targetToAppContext(invalidComponent))
		End Sub


		''' <summary>
		''' Remove a component from the list of invalid components.
		''' </summary>
		''' <seealso cref= #addInvalidComponent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeInvalidComponent(ByVal component As JComponent)
			Dim ___delegate As RepaintManager = getDelegate(component)
			If ___delegate IsNot Nothing Then
				___delegate.removeInvalidComponent(component)
				Return
			End If
			If invalidComponents IsNot Nothing Then
				Dim index As Integer = invalidComponents.IndexOf(component)
				If index <> -1 Then invalidComponents.RemoveAt(index)
			End If
		End Sub


		''' <summary>
		''' Add a component in the list of components that should be refreshed.
		''' If <i>c</i> already has a dirty region, the rectangle <i>(x,y,w,h)</i>
		''' will be unioned with the region that should be redrawn.
		''' </summary>
		''' <seealso cref= JComponent#repaint </seealso>
		Private Sub addDirtyRegion0(ByVal c As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
	'         Special cases we don't have to bother with.
	'         
			If (w <= 0) OrElse (h <= 0) OrElse (c Is Nothing) Then Return

			If (c.width <= 0) OrElse (c.height <= 0) Then Return

			If extendDirtyRegion(c, x, y, w, h) Then Return

	'         Make sure that c and all it ancestors (up to an Applet or
	'         * Window) are visible.  This loop has the same effect as
	'         * checking c.isShowing() (and note that it's still possible
	'         * that c is completely obscured by an opaque ancestor in
	'         * the specified rectangle).
	'         
			Dim root As Component = Nothing

			' Note: We can't synchronize around this, Frame.getExtendedState
			' is synchronized so that if we were to synchronize around this
			' it could lead to the possibility of getting locks out
			' of order and deadlocking.
			Dim p As Container = c
			Do While p IsNot Nothing
				If (Not p.visible) OrElse (p.peer Is Nothing) Then Return
				If (TypeOf p Is Window) OrElse (TypeOf p Is Applet) Then
					' Iconified frames are still visible!
					If TypeOf p Is Frame AndAlso (CType(p, Frame).extendedState And Frame.ICONIFIED) = Frame.ICONIFIED Then Return
					root = p
					Exit Do
				End If
				p = p.parent
			Loop

			If root Is Nothing Then Return

			SyncLock Me
				If extendDirtyRegion(c, x, y, w, h) Then Return
				dirtyComponents(c) = New Rectangle(x, y, w, h)
			End SyncLock

			' Queue a Runnable to invoke paintDirtyRegions and
			' validateInvalidComponents.
			scheduleProcessingRunnable(sun.awt.SunToolkit.targetToAppContext(c))
		End Sub

		''' <summary>
		''' Add a component in the list of components that should be refreshed.
		''' If <i>c</i> already has a dirty region, the rectangle <i>(x,y,w,h)</i>
		''' will be unioned with the region that should be redrawn.
		''' </summary>
		''' <param name="c"> Component to repaint, null results in nothing happening. </param>
		''' <param name="x"> X coordinate of the region to repaint </param>
		''' <param name="y"> Y coordinate of the region to repaint </param>
		''' <param name="w"> Width of the region to repaint </param>
		''' <param name="h"> Height of the region to repaint </param>
		''' <seealso cref= JComponent#repaint </seealso>
		Public Overridable Sub addDirtyRegion(ByVal c As JComponent, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim ___delegate As RepaintManager = getDelegate(c)
			If ___delegate IsNot Nothing Then
				___delegate.addDirtyRegion(c, x, y, w, h)
				Return
			End If
			addDirtyRegion0(c, x, y, w, h)
		End Sub

		''' <summary>
		''' Adds <code>window</code> to the list of <code>Component</code>s that
		''' need to be repainted.
		''' </summary>
		''' <param name="window"> Window to repaint, null results in nothing happening. </param>
		''' <param name="x"> X coordinate of the region to repaint </param>
		''' <param name="y"> Y coordinate of the region to repaint </param>
		''' <param name="w"> Width of the region to repaint </param>
		''' <param name="h"> Height of the region to repaint </param>
		''' <seealso cref= JFrame#repaint </seealso>
		''' <seealso cref= JWindow#repaint </seealso>
		''' <seealso cref= JDialog#repaint
		''' @since 1.6 </seealso>
		Public Overridable Sub addDirtyRegion(ByVal window As Window, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			addDirtyRegion0(window, x, y, w, h)
		End Sub

		''' <summary>
		''' Adds <code>applet</code> to the list of <code>Component</code>s that
		''' need to be repainted.
		''' </summary>
		''' <param name="applet"> Applet to repaint, null results in nothing happening. </param>
		''' <param name="x"> X coordinate of the region to repaint </param>
		''' <param name="y"> Y coordinate of the region to repaint </param>
		''' <param name="w"> Width of the region to repaint </param>
		''' <param name="h"> Height of the region to repaint </param>
		''' <seealso cref= JApplet#repaint
		''' @since 1.6 </seealso>
		Public Overridable Sub addDirtyRegion(ByVal applet As Applet, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			addDirtyRegion0(applet, x, y, w, h)
		End Sub

		Friend Overridable Sub scheduleHeavyWeightPaints()
			Dim hws As IDictionary(Of Container, Rectangle)

			SyncLock Me
				If hwDirtyComponents.Count = 0 Then Return
				hws = hwDirtyComponents
				hwDirtyComponents = New IdentityHashMap(Of Container, Rectangle)
			End SyncLock
			For Each hw As Container In hws.Keys
				Dim dirty As Rectangle = hws(hw)
				If TypeOf hw Is Window Then
					addDirtyRegion(CType(hw, Window), dirty.x, dirty.y, dirty.width, dirty.height)
				ElseIf TypeOf hw Is Applet Then
					addDirtyRegion(CType(hw, Applet), dirty.x, dirty.y, dirty.width, dirty.height)
				Else ' SwingHeavyWeight
					addDirtyRegion0(hw, dirty.x, dirty.y, dirty.width, dirty.height)
				End If
			Next hw
		End Sub

		'
		' This is called from the toolkit thread when a native expose is
		' received.
		'
		Friend Overridable Sub nativeAddDirtyRegion(ByVal appContext As sun.awt.AppContext, ByVal c As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If w > 0 AndAlso h > 0 Then
				SyncLock Me
					Dim dirty As Rectangle = hwDirtyComponents(c)
					If dirty Is Nothing Then
						hwDirtyComponents(c) = New Rectangle(x, y, w, h)
					Else
						hwDirtyComponents(c) = SwingUtilities.computeUnion(x, y, w, h, dirty)
					End If
				End SyncLock
				scheduleProcessingRunnable(appContext)
			End If
		End Sub

		'
		' This is called from the toolkit thread when awt needs to run a
		' Runnable before we paint.
		'
		Friend Overridable Sub nativeQueueSurfaceDataRunnable(ByVal appContext As sun.awt.AppContext, ByVal c As Component, ByVal r As Runnable)
			SyncLock Me
				If runnableList Is Nothing Then runnableList = New LinkedList(Of Runnable)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				runnableList.add(New Runnable()
	'			{
	'				public void run()
	'				{
	'					AccessControlContext stack = AccessController.getContext();
	'					AccessControlContext acc = AWTAccessor.getComponentAccessor().getAccessControlContext(c);
	'					javaSecurityAccess.doIntersectionPrivilege(New PrivilegedAction<Void>()
	'					{
	'						public Void run()
	'						{
	'							r.run();
	'							Return Nothing;
	'						}
	'					}, stack, acc);
	'				}
	'			});
			End SyncLock
			scheduleProcessingRunnable(appContext)
		End Sub

		''' <summary>
		''' Extends the dirty region for the specified component to include
		''' the new region.
		''' </summary>
		''' <returns> false if <code>c</code> is not yet marked dirty. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function extendDirtyRegion(ByVal c As Component, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			Dim r As Rectangle = dirtyComponents(c)
			If r IsNot Nothing Then
				' A non-null r implies c is already marked as dirty,
				' and that the parent is valid. Therefore we can
				' just union the rect and bail.
				SwingUtilities.computeUnion(x, y, w, h, r)
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Return the current dirty region for a component.
		'''  Return an empty rectangle if the component is not
		'''  dirty.
		''' </summary>
		Public Overridable Function getDirtyRegion(ByVal aComponent As JComponent) As Rectangle
			Dim ___delegate As RepaintManager = getDelegate(aComponent)
			If ___delegate IsNot Nothing Then Return ___delegate.getDirtyRegion(aComponent)
			Dim r As Rectangle
			SyncLock Me
				r = dirtyComponents(aComponent)
			End SyncLock
			If r Is Nothing Then
				Return New Rectangle(0,0,0,0)
			Else
				Return New Rectangle(r)
			End If
		End Function

		''' <summary>
		''' Mark a component completely dirty. <b>aComponent</b> will be
		''' completely painted during the next paintDirtyRegions() call.
		''' </summary>
		Public Overridable Sub markCompletelyDirty(ByVal aComponent As JComponent)
			Dim ___delegate As RepaintManager = getDelegate(aComponent)
			If ___delegate IsNot Nothing Then
				___delegate.markCompletelyDirty(aComponent)
				Return
			End If
			addDirtyRegion(aComponent,0,0,Integer.MaxValue,Integer.MaxValue)
		End Sub

		''' <summary>
		''' Mark a component completely clean. <b>aComponent</b> will not
		''' get painted during the next paintDirtyRegions() call.
		''' </summary>
		Public Overridable Sub markCompletelyClean(ByVal aComponent As JComponent)
			Dim ___delegate As RepaintManager = getDelegate(aComponent)
			If ___delegate IsNot Nothing Then
				___delegate.markCompletelyClean(aComponent)
				Return
			End If
			SyncLock Me
					dirtyComponents.Remove(aComponent)
			End SyncLock
		End Sub

		''' <summary>
		''' Convenience method that returns true if <b>aComponent</b> will be completely
		''' painted during the next paintDirtyRegions(). If computing dirty regions is
		''' expensive for your component, use this method and avoid computing dirty region
		''' if it return true.
		''' </summary>
		Public Overridable Function isCompletelyDirty(ByVal aComponent As JComponent) As Boolean
			Dim ___delegate As RepaintManager = getDelegate(aComponent)
			If ___delegate IsNot Nothing Then Return ___delegate.isCompletelyDirty(aComponent)
			Dim r As Rectangle

			r = getDirtyRegion(aComponent)
			If r.width = Integer.MaxValue AndAlso r.height = Integer.MaxValue Then
				Return True
			Else
				Return False
			End If
		End Function


		''' <summary>
		''' Validate all of the components that have been marked invalid. </summary>
		''' <seealso cref= #addInvalidComponent </seealso>
		Public Overridable Sub validateInvalidComponents()
			Dim ic As IList(Of Component)
			SyncLock Me
				If invalidComponents Is Nothing Then Return
				ic = invalidComponents
				invalidComponents = Nothing
			End SyncLock
			Dim n As Integer = ic.Count
			For i As Integer = 0 To n - 1
				Dim c As Component = ic(i)
				Dim stack As java.security.AccessControlContext = java.security.AccessController.context
				Dim acc As java.security.AccessControlContext = sun.awt.AWTAccessor.componentAccessor.getAccessControlContext(c)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Void>()
	'			{
	'					public Void run()
	'					{
	'						c.validate();
	'						Return Nothing;
	'					}
	'				}, stack, acc);
			Next i
		End Sub


		''' <summary>
		''' This is invoked to process paint requests.  It's needed
		''' for backward compatibility in so far as RepaintManager would previously
		''' not see paint requests for top levels, so, we have to make sure
		''' a subclass correctly paints any dirty top levels.
		''' </summary>
		Private Sub prePaintDirtyRegions()
			Dim dirtyComponents As IDictionary(Of Component, Rectangle)
			Dim runnableList As IList(Of Runnable)
			SyncLock Me
				dirtyComponents = Me.dirtyComponents
				runnableList = Me.runnableList
				Me.runnableList = Nothing
			End SyncLock
			If runnableList IsNot Nothing Then
				For Each runnable As Runnable In runnableList
					runnable.run()
				Next runnable
			End If
			paintDirtyRegions()
			If dirtyComponents.Count > 0 Then paintDirtyRegions(dirtyComponents)
		End Sub

		Private Sub updateWindows(ByVal dirtyComponents As IDictionary(Of Component, Rectangle))
			Dim toolkit As Toolkit = Toolkit.defaultToolkit
			If Not(TypeOf toolkit Is sun.awt.SunToolkit AndAlso CType(toolkit, sun.awt.SunToolkit).needUpdateWindow()) Then Return

			Dim windows As [Set](Of Window) = New HashSet(Of Window)
			Dim dirtyComps As IDictionary(Of Component, Rectangle).KeyCollection = dirtyComponents.Keys
			Dim it As IEnumerator(Of Component) = dirtyComps.GetEnumerator()
			Do While it.MoveNext()
				Dim dirty As Component = it.Current
				Dim window As Window = If(TypeOf dirty Is Window, CType(dirty, Window), SwingUtilities.getWindowAncestor(dirty))
				If window IsNot Nothing AndAlso (Not window.opaque) Then windows.add(window)
			Loop

			For Each window As Window In windows
				sun.awt.AWTAccessor.windowAccessor.updateWindow(window)
			Next window
		End Sub

		Friend Overridable Property painting As Boolean
			Get
				Return painting
			End Get
		End Property

		''' <summary>
		''' Paint all of the components that have been marked dirty.
		''' </summary>
		''' <seealso cref= #addDirtyRegion </seealso>
		Public Overridable Sub paintDirtyRegions()
			SyncLock Me ' swap for thread safety
				Dim tmp As IDictionary(Of Component, Rectangle) = tmpDirtyComponents
				tmpDirtyComponents = dirtyComponents
				dirtyComponents = tmp
				dirtyComponents.Clear()
			End SyncLock
			paintDirtyRegions(tmpDirtyComponents)
		End Sub

		Private Sub paintDirtyRegions(ByVal tmpDirtyComponents As IDictionary(Of Component, Rectangle))
			If tmpDirtyComponents.Count = 0 Then Return

			Dim roots As IList(Of Component) = New List(Of Component)(tmpDirtyComponents.Count)
			For Each dirty As Component In tmpDirtyComponents.Keys
				collectDirtyComponents(tmpDirtyComponents, dirty, roots)
			Next dirty

			Dim count As New java.util.concurrent.atomic.AtomicInteger(roots.Count)
			painting = True
			Try
				For j As Integer = 0 To count.get() - 1
					Dim i As Integer = j
					Dim dirtyComponent As Component = roots(j)
					Dim stack As java.security.AccessControlContext = java.security.AccessController.context
					Dim acc As java.security.AccessControlContext = sun.awt.AWTAccessor.componentAccessor.getAccessControlContext(dirtyComponent)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Void>()
	'				{
	'					public Void run()
	'					{
	'						Rectangle rect = tmpDirtyComponents.get(dirtyComponent);
	'						' Sometimes when RepaintManager is changed during the painting
	'						' we may get null here, see #6995769 for details
	'						if (rect == Nothing)
	'						{
	'							Return Nothing;
	'						}
	'
	'						int localBoundsH = dirtyComponent.getHeight();
	'						int localBoundsW = dirtyComponent.getWidth();
	'						SwingUtilities.computeIntersection(0, 0, localBoundsW, localBoundsH, rect);
	'						if (dirtyComponent instanceof JComponent)
	'						{
	'							((JComponent)dirtyComponent).paintImmediately(rect.x,rect.y,rect.width, rect.height);
	'						}
	'						else if (dirtyComponent.isShowing())
	'						{
	'							Graphics g = JComponent.safelyGetGraphics(dirtyComponent, dirtyComponent);
	'							' If the Graphics goes away, it means someone disposed of
	'							' the window, don't do anything.
	'							if (g != Nothing)
	'							{
	'								g.setClip(rect.x, rect.y, rect.width, rect.height);
	'								try
	'								{
	'									dirtyComponent.paint(g);
	'								}
	'								finally
	'								{
	'									g.dispose();
	'								}
	'							}
	'						}
	'						' If the repaintRoot has been set, service it now and
	'						' remove any components that are children of repaintRoot.
	'						if (repaintRoot != Nothing)
	'						{
	'							adjustRoots(repaintRoot, roots, i + 1);
	'							count.set(roots.size());
	'							paintManager.isRepaintingRoot = True;
	'							repaintRoot.paintImmediately(0, 0, repaintRoot.getWidth(), repaintRoot.getHeight());
	'							paintManager.isRepaintingRoot = False;
	'							' Only service repaintRoot once.
	'							repaintRoot = Nothing;
	'						}
	'
	'						Return Nothing;
	'					}
	'				}, stack, acc);
				Next j
			Finally
				painting = False
			End Try

			updateWindows(tmpDirtyComponents)

			tmpDirtyComponents.Clear()
		End Sub


		''' <summary>
		''' Removes any components from roots that are children of
		''' root.
		''' </summary>
		Private Sub adjustRoots(ByVal root As JComponent, ByVal roots As IList(Of Component), ByVal index As Integer)
			For i As Integer = roots.Count - 1 To index Step -1
				Dim c As Component = roots(i)
				Do
					If c Is root OrElse c Is Nothing OrElse Not(TypeOf c Is JComponent) Then Exit Do
					c = c.parent
				Loop
				If c Is root Then roots.RemoveAt(i)
			Next i
		End Sub

		Friend tmp As New Rectangle

		Friend Overridable Sub collectDirtyComponents(ByVal dirtyComponents As IDictionary(Of Component, Rectangle), ByVal dirtyComponent As Component, ByVal roots As IList(Of Component))
			Dim dx, dy, rootDx, rootDy As Integer
			Dim component, rootDirtyComponent, parent As Component
			Dim cBounds As Rectangle

			' Find the highest parent which is dirty.  When we get out of this
			' rootDx and rootDy will contain the translation from the
			' rootDirtyComponent's coordinate system to the coordinates of the
			' original dirty component.  The tmp Rect is also used to compute the
			' visible portion of the dirtyRect.

				rootDirtyComponent = dirtyComponent
				component = rootDirtyComponent

			Dim x As Integer = dirtyComponent.x
			Dim y As Integer = dirtyComponent.y
			Dim w As Integer = dirtyComponent.width
			Dim h As Integer = dirtyComponent.height

				rootDx = 0
				dx = rootDx
				rootDy = 0
				dy = rootDy
			tmp.bounds = dirtyComponents(dirtyComponent)

			' System.out.println("Collect dirty component for bound " + tmp +
			'                                   "component bounds is " + cBounds);
			SwingUtilities.computeIntersection(0,0,w,h,tmp)

			If tmp.empty Then Return

			Do
				If Not(TypeOf component Is JComponent) Then Exit Do

				parent = component.parent
				If parent Is Nothing Then Exit Do

				component = parent

				dx += x
				dy += y
				tmp.locationion(tmp.x + x, tmp.y + y)

				x = component.x
				y = component.y
				w = component.width
				h = component.height
				tmp = SwingUtilities.computeIntersection(0,0,w,h,tmp)

				If tmp.empty Then Return

				If dirtyComponents(component) IsNot Nothing Then
					rootDirtyComponent = component
					rootDx = dx
					rootDy = dy
				End If
			Loop

			If dirtyComponent IsNot rootDirtyComponent Then
				Dim r As Rectangle
				tmp.locationion(tmp.x + rootDx - dx, tmp.y + rootDy - dy)
				r = dirtyComponents(rootDirtyComponent)
				SwingUtilities.computeUnion(tmp.x,tmp.y,tmp.width,tmp.height,r)
			End If

			' If we haven't seen this root before, then we need to add it to the
			' list of root dirty Views.

			If Not roots.Contains(rootDirtyComponent) Then roots.Add(rootDirtyComponent)
		End Sub


		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			If dirtyComponents IsNot Nothing Then sb.Append("" & dirtyComponents)
			Return sb.ToString()
		End Function


	   ''' <summary>
	   ''' Return the offscreen buffer that should be used as a double buffer with
	   ''' the component <code>c</code>.
	   ''' By default there is a double buffer per RepaintManager.
	   ''' The buffer might be smaller than <code>(proposedWidth,proposedHeight)</code>
	   ''' This happens when the maximum double buffer size as been set for the receiving
	   ''' repaint manager.
	   ''' </summary>
		Public Overridable Function getOffscreenBuffer(ByVal c As Component, ByVal proposedWidth As Integer, ByVal proposedHeight As Integer) As Image
			Dim ___delegate As RepaintManager = getDelegate(c)
			If ___delegate IsNot Nothing Then Return ___delegate.getOffscreenBuffer(c, proposedWidth, proposedHeight)
			Return _getOffscreenBuffer(c, proposedWidth, proposedHeight)
		End Function

	  ''' <summary>
	  ''' Return a volatile offscreen buffer that should be used as a
	  ''' double buffer with the specified component <code>c</code>.
	  ''' The image returned will be an instance of VolatileImage, or null
	  ''' if a VolatileImage object could not be instantiated.
	  ''' This buffer might be smaller than <code>(proposedWidth,proposedHeight)</code>.
	  ''' This happens when the maximum double buffer size has been set for this
	  ''' repaint manager.
	  ''' </summary>
	  ''' <seealso cref= java.awt.image.VolatileImage
	  ''' @since 1.4 </seealso>
		Public Overridable Function getVolatileOffscreenBuffer(ByVal c As Component, ByVal proposedWidth As Integer, ByVal proposedHeight As Integer) As Image
			Dim ___delegate As RepaintManager = getDelegate(c)
			If ___delegate IsNot Nothing Then Return ___delegate.getVolatileOffscreenBuffer(c, proposedWidth, proposedHeight)

			' If the window is non-opaque, it's double-buffered at peer's level
			Dim w As Window = If(TypeOf c Is Window, CType(c, Window), SwingUtilities.getWindowAncestor(c))
			If Not w.opaque Then
				Dim tk As Toolkit = Toolkit.defaultToolkit
				If (TypeOf tk Is sun.awt.SunToolkit) AndAlso (CType(tk, sun.awt.SunToolkit).needUpdateWindow()) Then Return Nothing
			End If

			Dim config As GraphicsConfiguration = c.graphicsConfiguration
			If config Is Nothing Then config = GraphicsEnvironment.localGraphicsEnvironment.defaultScreenDevice.defaultConfiguration
			Dim maxSize As Dimension = doubleBufferMaximumSize
			Dim width As Integer = If(proposedWidth < 1, 1, (If(proposedWidth > maxSize.width, maxSize.width, proposedWidth)))
			Dim height As Integer = If(proposedHeight < 1, 1, (If(proposedHeight > maxSize.height, maxSize.height, proposedHeight)))
			Dim image As java.awt.image.VolatileImage = volatileMap(config)
			If image Is Nothing OrElse image.width < width OrElse image.height < height Then
				If image IsNot Nothing Then image.flush()
				image = config.createCompatibleVolatileImage(width, height, volatileBufferType)
				volatileMap(config) = image
			End If
			Return image
		End Function

		Private Function _getOffscreenBuffer(ByVal c As Component, ByVal proposedWidth As Integer, ByVal proposedHeight As Integer) As Image
			Dim maxSize As Dimension = doubleBufferMaximumSize
			Dim doubleBuffer As DoubleBufferInfo
			Dim width, height As Integer

			' If the window is non-opaque, it's double-buffered at peer's level
			Dim w As Window = If(TypeOf c Is Window, CType(c, Window), SwingUtilities.getWindowAncestor(c))
			If Not w.opaque Then
				Dim tk As Toolkit = Toolkit.defaultToolkit
				If (TypeOf tk Is sun.awt.SunToolkit) AndAlso (CType(tk, sun.awt.SunToolkit).needUpdateWindow()) Then Return Nothing
			End If

			If standardDoubleBuffer Is Nothing Then standardDoubleBuffer = New DoubleBufferInfo(Me)
			doubleBuffer = standardDoubleBuffer

			width = If(proposedWidth < 1, 1, (If(proposedWidth > maxSize.width, maxSize.width, proposedWidth)))
			height = If(proposedHeight < 1, 1, (If(proposedHeight > maxSize.height, maxSize.height, proposedHeight)))

			If doubleBuffer.needsReset OrElse (doubleBuffer.image IsNot Nothing AndAlso (doubleBuffer.size.width < width OrElse doubleBuffer.size.height < height)) Then
				doubleBuffer.needsReset = False
				If doubleBuffer.image IsNot Nothing Then
					doubleBuffer.image.flush()
					doubleBuffer.image = Nothing
				End If
				width = Math.Max(doubleBuffer.size.width, width)
				height = Math.Max(doubleBuffer.size.height, height)
			End If

			Dim result As Image = doubleBuffer.image

			If doubleBuffer.image Is Nothing Then
				result = c.createImage(width, height)
				doubleBuffer.size = New Dimension(width, height)
				If TypeOf c Is JComponent Then
					CType(c, JComponent).createdDoubleBuffer = True
					doubleBuffer.image = result
				End If
				' JComponent will inform us when it is no longer valid
				' (via removeNotify) we have no such hook to other components,
				' therefore we don't keep a ref to the Component
				' (indirectly through the Image) by stashing the image.
			End If
			Return result
		End Function


		''' <summary>
		''' Set the maximum double buffer size. * </summary>
		Public Overridable Property doubleBufferMaximumSize As Dimension
			Set(ByVal d As Dimension)
				doubleBufferMaxSize = d
				If doubleBufferMaxSize Is Nothing Then
					clearImages()
				Else
					clearImages(d.width, d.height)
				End If
			End Set
			Get
				If doubleBufferMaxSize Is Nothing Then
					Try
						Dim virtualBounds As New Rectangle
						Dim ge As GraphicsEnvironment = GraphicsEnvironment.localGraphicsEnvironment
						For Each gd As GraphicsDevice In ge.screenDevices
							Dim gc As GraphicsConfiguration = gd.defaultConfiguration
							virtualBounds = virtualBounds.union(gc.bounds)
						Next gd
						doubleBufferMaxSize = New Dimension(virtualBounds.width, virtualBounds.height)
					Catch e As HeadlessException
						doubleBufferMaxSize = New Dimension(Integer.MaxValue, Integer.MaxValue)
					End Try
				End If
				Return doubleBufferMaxSize
			End Get
		End Property

		Private Sub clearImages()
			clearImages(0, 0)
		End Sub

		Private Sub clearImages(ByVal width As Integer, ByVal height As Integer)
			If standardDoubleBuffer IsNot Nothing AndAlso standardDoubleBuffer.image IsNot Nothing Then
				If standardDoubleBuffer.image.getWidth(Nothing) > width OrElse standardDoubleBuffer.image.getHeight(Nothing) > height Then
					standardDoubleBuffer.image.flush()
					standardDoubleBuffer.image = Nothing
				End If
			End If
			' Clear out the VolatileImages
			Dim gcs As IEnumerator(Of GraphicsConfiguration) = volatileMap.Keys.GetEnumerator()
			Do While gcs.MoveNext()
				Dim gc As GraphicsConfiguration = gcs.Current
				Dim image As java.awt.image.VolatileImage = volatileMap(gc)
				If image.width > width OrElse image.height > height Then
					image.flush()
					gcs.remove()
				End If
			Loop
		End Sub


		''' <summary>
		''' Enables or disables double buffering in this RepaintManager.
		''' CAUTION: The default value for this property is set for optimal
		''' paint performance on the given platform and it is not recommended
		''' that programs modify this property directly.
		''' </summary>
		''' <param name="aFlag">  true to activate double buffering </param>
		''' <seealso cref= #isDoubleBufferingEnabled </seealso>
		Public Overridable Property doubleBufferingEnabled As Boolean
			Set(ByVal aFlag As Boolean)
				doubleBufferingEnabled = aFlag
				Dim ___paintManager As PaintManager = paintManager
				If (Not aFlag) AndAlso ___paintManager.GetType() IsNot GetType(PaintManager) Then paintManager = New PaintManager
			End Set
			Get
				Return doubleBufferingEnabled
			End Get
		End Property


		''' <summary>
		''' This resets the double buffer. Actually, it marks the double buffer
		''' as invalid, the double buffer will then be recreated on the next
		''' invocation of getOffscreenBuffer.
		''' </summary>
		Friend Overridable Sub resetDoubleBuffer()
			If standardDoubleBuffer IsNot Nothing Then standardDoubleBuffer.needsReset = True
		End Sub

		''' <summary>
		''' This resets the volatile double buffer.
		''' </summary>
		Friend Overridable Sub resetVolatileDoubleBuffer(ByVal gc As GraphicsConfiguration)
			Dim image As Image = volatileMap.Remove(gc)
			If image IsNot Nothing Then image.flush()
		End Sub

		''' <summary>
		''' Returns true if we should use the <code>Image</code> returned
		''' from <code>getVolatileOffscreenBuffer</code> to do double buffering.
		''' </summary>
		Friend Overridable Function useVolatileDoubleBuffer() As Boolean
			Return volatileImageBufferEnabled
		End Function

		''' <summary>
		''' Returns true if the current thread is the thread painting.  This
		''' will return false if no threads are painting.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property paintingThread As Boolean
			Get
				Return (Thread.CurrentThread Is paintThread)
			End Get
		End Property
		'
		' Paint methods.  You very, VERY rarely need to invoke these.
		' They are invoked directly from JComponent's painting code and
		' when painting happens outside the normal flow: DefaultDesktopManager
		' and JViewport.  If you end up needing these methods in other places be
		' careful that you don't get stuck in a paint loop.
		'

		''' <summary>
		''' Paints a region of a component
		''' </summary>
		''' <param name="paintingComponent"> Component to paint </param>
		''' <param name="bufferComponent"> Component to obtain buffer for </param>
		''' <param name="g"> Graphics to paint to </param>
		''' <param name="x"> X-coordinate </param>
		''' <param name="y"> Y-coordinate </param>
		''' <param name="w"> Width </param>
		''' <param name="h"> Height </param>
		Friend Overridable Sub paint(ByVal paintingComponent As JComponent, ByVal bufferComponent As JComponent, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim ___paintManager As PaintManager = paintManager
			If Not paintingThread Then
				' We're painting to two threads at once.  PaintManager deals
				' with this a bit better than BufferStrategyPaintManager, use
				' it to avoid possible exceptions/corruption.
				If ___paintManager.GetType() IsNot GetType(PaintManager) Then
					___paintManager = New PaintManager
					___paintManager.___repaintManager = Me
				End If
			End If
			If Not ___paintManager.paint(paintingComponent, bufferComponent, g, x, y, w, h) Then
				g.cliplip(x, y, w, h)
				paintingComponent.paintToOffscreen(g, x, y, w, h, x + w, y + h)
			End If
		End Sub

		''' <summary>
		''' Does a copy area on the specified region.
		''' </summary>
		''' <param name="clip"> Whether or not the copyArea needs to be clipped to the
		'''             Component's bounds. </param>
		Friend Overridable Sub copyArea(ByVal c As JComponent, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal deltaX As Integer, ByVal deltaY As Integer, ByVal clip As Boolean)
			paintManager.copyArea(c, g, x, y, w, h, deltaX, deltaY, clip)
		End Sub

		Private repaintListeners As IList(Of sun.swing.SwingUtilities2.RepaintListener) = New List(Of sun.swing.SwingUtilities2.RepaintListener)(1)

		Private Sub addRepaintListener(ByVal l As sun.swing.SwingUtilities2.RepaintListener)
			repaintListeners.Add(l)
		End Sub

		Private Sub removeRepaintListener(ByVal l As sun.swing.SwingUtilities2.RepaintListener)
			repaintListeners.Remove(l)
		End Sub

		''' <summary>
		''' Notify the attached repaint listeners that an area of the {@code c} component
		''' has been immediately repainted, that is without scheduling a repaint runnable,
		''' due to performing a "blit" (via calling the {@code copyArea} method).
		''' </summary>
		''' <param name="c"> the component </param>
		''' <param name="x"> the x coordinate of the area </param>
		''' <param name="y"> the y coordinate of the area </param>
		''' <param name="w"> the width of the area </param>
		''' <param name="h"> the height of the area </param>
		Friend Overridable Sub notifyRepaintPerformed(ByVal c As JComponent, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			For Each l As sun.swing.SwingUtilities2.RepaintListener In repaintListeners
				l.repaintPerformed(c, x, y, w, h)
			Next l
		End Sub

		''' <summary>
		''' Invoked prior to any paint/copyArea method calls.  This will
		''' be followed by an invocation of <code>endPaint</code>.
		''' <b>WARNING</b>: Callers of this method need to wrap the call
		''' in a <code>try/finally</code>, otherwise if an exception is thrown
		''' during the course of painting the RepaintManager may
		''' be left in a state in which the screen is not updated, eg:
		''' <pre>
		''' repaintManager.beginPaint();
		''' try {
		'''   repaintManager.paint(...);
		''' } finally {
		'''   repaintManager.endPaint();
		''' }
		''' </pre>
		''' </summary>
		Friend Overridable Sub beginPaint()
			Dim multiThreadedPaint As Boolean = False
			Dim paintDepth As Integer
			Dim currentThread As Thread = Thread.CurrentThread
			SyncLock Me
				paintDepth = Me.paintDepth
				If paintThread Is Nothing OrElse currentThread Is paintThread Then
					paintThread = currentThread
					Me.paintDepth += 1
				Else
					multiThreadedPaint = True
				End If
			End SyncLock
			If (Not multiThreadedPaint) AndAlso paintDepth = 0 Then paintManager.beginPaint()
		End Sub

		''' <summary>
		''' Invoked after <code>beginPaint</code> has been invoked.
		''' </summary>
		Friend Overridable Sub endPaint()
			If paintingThread Then
				Dim ___paintManager As PaintManager = Nothing
				SyncLock Me
					paintDepth -= 1
					If paintDepth = 0 Then ___paintManager = paintManager
				End SyncLock
				If ___paintManager IsNot Nothing Then
					___paintManager.endPaint()
					SyncLock Me
						paintThread = Nothing
					End SyncLock
				End If
			End If
		End Sub

		''' <summary>
		''' If possible this will show a previously rendered portion of
		''' a Component.  If successful, this will return true, otherwise false.
		''' <p>
		''' WARNING: This method is invoked from the native toolkit thread, be
		''' very careful as to what methods this invokes!
		''' </summary>
		Friend Overridable Function show(ByVal c As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			Return paintManager.show(c, x, y, w, h)
		End Function

		''' <summary>
		''' Invoked when the doubleBuffered or useTrueDoubleBuffering
		''' properties of a JRootPane change.  This may come in on any thread.
		''' </summary>
		Friend Overridable Sub doubleBufferingChanged(ByVal rootPane As JRootPane)
			paintManager.doubleBufferingChanged(rootPane)
		End Sub

		''' <summary>
		''' Sets the <code>PaintManager</code> that is used to handle all
		''' double buffered painting.
		''' </summary>
		''' <param name="paintManager"> The PaintManager to use.  Passing in null indicates
		'''        the fallback PaintManager should be used. </param>
		Friend Overridable Property paintManager As PaintManager
			Set(ByVal paintManager As PaintManager)
				If paintManager Is Nothing Then paintManager = New PaintManager
				Dim oldPaintManager As PaintManager
				SyncLock Me
					oldPaintManager = Me.paintManager
					Me.paintManager = paintManager
					paintManager.___repaintManager = Me
				End SyncLock
				If oldPaintManager IsNot Nothing Then oldPaintManager.Dispose()
			End Set
			Get
				If paintManager Is Nothing Then
					Dim ___paintManager As PaintManager = Nothing
					If doubleBufferingEnabled AndAlso (Not nativeDoubleBuffering) Then
						Select Case bufferStrategyType
						Case BUFFER_STRATEGY_NOT_SPECIFIED
							Dim tk As Toolkit = Toolkit.defaultToolkit
							If TypeOf tk Is sun.awt.SunToolkit Then
								Dim stk As sun.awt.SunToolkit = CType(tk, sun.awt.SunToolkit)
								If stk.useBufferPerWindow() Then ___paintManager = New BufferStrategyPaintManager
							End If
						Case BUFFER_STRATEGY_SPECIFIED_ON
							___paintManager = New BufferStrategyPaintManager
						Case Else
						End Select
					End If
					' null case handled in setPaintManager
					paintManager = ___paintManager
				End If
				Return paintManager
			End Get
		End Property


		Private Sub scheduleProcessingRunnable(ByVal context As sun.awt.AppContext)
			If processingRunnable.markPending() Then
				Dim tk As Toolkit = Toolkit.defaultToolkit
				If TypeOf tk Is sun.awt.SunToolkit Then
					sun.awt.SunToolkit.getSystemEventQueueImplPP(context).postEvent(New InvocationEvent(Toolkit.defaultToolkit, processingRunnable))
				Else
					Toolkit.defaultToolkit.systemEventQueue.postEvent(New InvocationEvent(Toolkit.defaultToolkit, processingRunnable))
				End If
			End If
		End Sub


		''' <summary>
		''' PaintManager is used to handle all double buffered painting for
		''' Swing.  Subclasses should call back into the JComponent method
		''' <code>paintToOffscreen</code> to handle the actual painting.
		''' </summary>
		Friend Class PaintManager
			''' <summary>
			''' RepaintManager the PaintManager has been installed on.
			''' </summary>
			Protected Friend ___repaintManager As RepaintManager
			Friend ___isRepaintingRoot As Boolean

			''' <summary>
			''' Paints a region of a component
			''' </summary>
			''' <param name="paintingComponent"> Component to paint </param>
			''' <param name="bufferComponent"> Component to obtain buffer for </param>
			''' <param name="g"> Graphics to paint to </param>
			''' <param name="x"> X-coordinate </param>
			''' <param name="y"> Y-coordinate </param>
			''' <param name="w"> Width </param>
			''' <param name="h"> Height </param>
			''' <returns> true if painting was successful. </returns>
			Public Overridable Function paint(ByVal paintingComponent As JComponent, ByVal bufferComponent As JComponent, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
				' First attempt to use VolatileImage buffer for performance.
				' If this fails (which should rarely occur), fallback to a
				' standard Image buffer.
				Dim paintCompleted As Boolean = False
				Dim offscreen As Image
				offscreen = getValidImage(___repaintManager.getVolatileOffscreenBuffer(bufferComponent, w, h))
				If ___repaintManager.useVolatileDoubleBuffer() AndAlso offscreen IsNot Nothing Then
					Dim vImage As java.awt.image.VolatileImage = CType(offscreen, java.awt.image.VolatileImage)
					Dim gc As GraphicsConfiguration = bufferComponent.graphicsConfiguration
					Dim i As Integer = 0
					Do While (Not paintCompleted) AndAlso i < RepaintManager.VOLATILE_LOOP_MAX
						If vImage.validate(gc) = java.awt.image.VolatileImage.IMAGE_INCOMPATIBLE Then
							___repaintManager.resetVolatileDoubleBuffer(gc)
							offscreen = ___repaintManager.getVolatileOffscreenBuffer(bufferComponent,w, h)
							vImage = CType(offscreen, java.awt.image.VolatileImage)
						End If
						paintDoubleBuffered(paintingComponent, vImage, g, x, y, w, h)
						paintCompleted = Not vImage.contentsLost()
						i += 1
					Loop
				End If
				' VolatileImage painting loop failed, fallback to regular
				' offscreen buffer
				offscreen = getValidImage(___repaintManager.getOffscreenBuffer(bufferComponent, w, h))
				If (Not paintCompleted) AndAlso offscreen IsNot Nothing Then
					paintDoubleBuffered(paintingComponent, offscreen, g, x, y, w, h)
					paintCompleted = True
				End If
				Return paintCompleted
			End Function

			''' <summary>
			''' Does a copy area on the specified region.
			''' </summary>
			Public Overridable Sub copyArea(ByVal c As JComponent, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal deltaX As Integer, ByVal deltaY As Integer, ByVal clip As Boolean)
				g.copyArea(x, y, w, h, deltaX, deltaY)
			End Sub

			''' <summary>
			''' Invoked prior to any calls to paint or copyArea.
			''' </summary>
			Public Overridable Sub beginPaint()
			End Sub

			''' <summary>
			''' Invoked to indicate painting has been completed.
			''' </summary>
			Public Overridable Sub endPaint()
			End Sub

			''' <summary>
			''' Shows a region of a previously rendered component.  This
			''' will return true if successful, false otherwise.  The default
			''' implementation returns false.
			''' </summary>
			Public Overridable Function show(ByVal c As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
				Return False
			End Function

			''' <summary>
			''' Invoked when the doubleBuffered or useTrueDoubleBuffering
			''' properties of a JRootPane change.  This may come in on any thread.
			''' </summary>
			Public Overridable Sub doubleBufferingChanged(ByVal rootPane As JRootPane)
			End Sub

			''' <summary>
			''' Paints a portion of a component to an offscreen buffer.
			''' </summary>
			Protected Friend Overridable Sub paintDoubleBuffered(ByVal c As JComponent, ByVal image As Image, ByVal g As Graphics, ByVal clipX As Integer, ByVal clipY As Integer, ByVal clipW As Integer, ByVal clipH As Integer)
				Dim osg As Graphics = image.graphics
				Dim bw As Integer = Math.Min(clipW, image.getWidth(Nothing))
				Dim bh As Integer = Math.Min(clipH, image.getHeight(Nothing))
				Dim x, y, maxx, maxy As Integer

				Try
					x = clipX
					maxx = clipX+clipW
					Do While x < maxx
						y=clipY
						maxy = clipY + clipH
						Do While y < maxy
							osg.translate(-x, -y)
							osg.cliplip(x,y,bw,bh)
							If volatileBufferType <> Transparency.OPAQUE AndAlso TypeOf osg Is Graphics2D Then
								Dim g2d As Graphics2D = CType(osg, Graphics2D)
								Dim oldBg As Color = g2d.background
								g2d.background = c.background
								g2d.clearRect(x, y, bw, bh)
								g2d.background = oldBg
							End If
							c.paintToOffscreen(osg, x, y, bw, bh, maxx, maxy)
							g.cliplip(x, y, bw, bh)
							If volatileBufferType <> Transparency.OPAQUE AndAlso TypeOf g Is Graphics2D Then
								Dim g2d As Graphics2D = CType(g, Graphics2D)
								Dim oldComposite As Composite = g2d.composite
								g2d.composite = AlphaComposite.Src
								g2d.drawImage(image, x, y, c)
								g2d.composite = oldComposite
							Else
								g.drawImage(image, x, y, c)
							End If
							osg.translate(x, y)
							y += bh
						Loop
						x += bw
					Loop
				Finally
					osg.Dispose()
				End Try
			End Sub

			''' <summary>
			''' If <code>image</code> is non-null with a positive size it
			''' is returned, otherwise null is returned.
			''' </summary>
			Private Function getValidImage(ByVal image As Image) As Image
				If image IsNot Nothing AndAlso image.getWidth(Nothing) > 0 AndAlso image.getHeight(Nothing) > 0 Then Return image
				Return Nothing
			End Function

			''' <summary>
			''' Schedules a repaint for the specified component.  This differs
			''' from <code>root.repaint</code> in that if the RepaintManager is
			''' currently processing paint requests it'll process this request
			''' with the current set of requests.
			''' </summary>
			Protected Friend Overridable Sub repaintRoot(ByVal root As JComponent)
				assert(___repaintManager.repaintRoot Is Nothing)
				If ___repaintManager.painting Then
					___repaintManager.repaintRoot = root
				Else
					root.repaint()
				End If
			End Sub

			''' <summary>
			''' Returns true if the component being painted is the root component
			''' that was previously passed to <code>repaintRoot</code>.
			''' </summary>
			Protected Friend Overridable Property repaintingRoot As Boolean
				Get
					Return ___isRepaintingRoot
				End Get
			End Property

			''' <summary>
			''' Cleans up any state.  After invoked the PaintManager will no
			''' longer be used anymore.
			''' </summary>
			Protected Friend Overridable Sub dispose()
			End Sub
		End Class


		Private Class DoubleBufferInfo
			Private ReadOnly outerInstance As RepaintManager

			Public Sub New(ByVal outerInstance As RepaintManager)
				Me.outerInstance = outerInstance
			End Sub

			Public image As Image
			Public size As Dimension
			Public needsReset As Boolean = False
		End Class


		''' <summary>
		''' Listener installed to detect display changes. When display changes,
		''' schedules a callback to notify all RepaintManagers of the display
		''' changes. Only one DisplayChangedHandler is ever installed. The
		''' singleton instance will schedule notification for all AppContexts.
		''' </summary>
		Private NotInheritable Class DisplayChangedHandler
			Implements sun.awt.DisplayChangedListener

			' Empty non private constructor was added because access to this
			' class shouldn't be generated by the compiler using synthetic
			' accessor method
			Friend Sub New()
			End Sub

			Public Sub displayChanged()
				scheduleDisplayChanges()
			End Sub

			Public Sub paletteChanged()
			End Sub

			Private Shared Sub scheduleDisplayChanges()
				' To avoid threading problems, we notify each RepaintManager
				' on the thread it was created on.
				For Each context As sun.awt.AppContext In sun.awt.AppContext.appContexts
					SyncLock context
						If Not context.disposed Then
							Dim eventQueue As EventQueue = CType(context.get(sun.awt.AppContext.EVENT_QUEUE_KEY), EventQueue)
							If eventQueue IsNot Nothing Then eventQueue.postEvent(New InvocationEvent(Toolkit.defaultToolkit, New DisplayChangedRunnable))
						End If
					End SyncLock
				Next context
			End Sub
		End Class


		Private NotInheritable Class DisplayChangedRunnable
			Implements Runnable

			Public Sub run()
				RepaintManager.currentManager(CType(Nothing, JComponent)).displayChanged()
			End Sub
		End Class


		''' <summary>
		''' Runnable used to process all repaint/revalidate requests.
		''' </summary>
		Private NotInheritable Class ProcessingRunnable
			Implements Runnable

			Private ReadOnly outerInstance As RepaintManager

			Public Sub New(ByVal outerInstance As RepaintManager)
				Me.outerInstance = outerInstance
			End Sub

			' If true, we're wainting on the EventQueue.
			Private pending As Boolean

			''' <summary>
			''' Marks this processing runnable as pending. If this was not
			''' already marked as pending, true is returned.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Function markPending() As Boolean
				If Not pending Then
					pending = True
					Return True
				End If
				Return False
			End Function

			Public Sub run()
				SyncLock Me
					pending = False
				End SyncLock
				' First pass, flush any heavy paint events into real paint
				' events.  If there are pending heavy weight requests this will
				' result in q'ing this request up one more time.  As
				' long as no other requests come in between now and the time
				' the second one is processed nothing will happen.  This is not
				' ideal, but the logic needed to suppress the second request is
				' more headache than it's worth.
				outerInstance.scheduleHeavyWeightPaints()
				' Do the actual validation and painting.
				outerInstance.validateInvalidComponents()
				outerInstance.prePaintDirtyRegions()
			End Sub
		End Class
		Private Function getDelegate(ByVal c As Component) As RepaintManager
			Dim ___delegate As RepaintManager = com.sun.java.swing.SwingUtilities3.getDelegateRepaintManager(c)
			If Me Is ___delegate Then ___delegate = Nothing
			Return ___delegate
		End Function
	End Class

End Namespace
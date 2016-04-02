Imports System
Imports System.Runtime.InteropServices
Imports javax.accessibility

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt



	''' <summary>
	''' A container class which implements automatic horizontal and/or
	''' vertical scrolling for a single child component.  The display
	''' policy for the scrollbars can be set to:
	''' <OL>
	''' <LI>as needed: scrollbars created and shown only when needed by scrollpane
	''' <LI>always: scrollbars created and always shown by the scrollpane
	''' <LI>never: scrollbars never created or shown by the scrollpane
	''' </OL>
	''' <P>
	''' The state of the horizontal and vertical scrollbars is represented
	''' by two <code>ScrollPaneAdjustable</code> objects (one for each
	''' dimension) which implement the <code>Adjustable</code> interface.
	''' The API provides methods to access those objects such that the
	''' attributes on the Adjustable object (such as unitIncrement, value,
	''' etc.) can be manipulated.
	''' <P>
	''' Certain adjustable properties (minimum, maximum, blockIncrement,
	''' and visibleAmount) are set internally by the scrollpane in accordance
	''' with the geometry of the scrollpane and its child and these should
	''' not be set by programs using the scrollpane.
	''' <P>
	''' If the scrollbar display policy is defined as "never", then the
	''' scrollpane can still be programmatically scrolled using the
	''' setScrollPosition() method and the scrollpane will move and clip
	''' the child's contents appropriately.  This policy is useful if the
	''' program needs to create and manage its own adjustable controls.
	''' <P>
	''' The placement of the scrollbars is controlled by platform-specific
	''' properties set by the user outside of the program.
	''' <P>
	''' The initial size of this container is set to 100x100, but can
	''' be reset using setSize().
	''' <P>
	''' Scrolling with the wheel on a wheel-equipped mouse is enabled by default.
	''' This can be disabled using <code>setWheelScrollingEnabled</code>.
	''' Wheel scrolling can be customized by setting the block and
	''' unit increment of the horizontal and vertical Adjustables.
	''' For information on how mouse wheel events are dispatched, see
	''' the class description for <seealso cref="MouseWheelEvent"/>.
	''' <P>
	''' Insets are used to define any space used by scrollbars and any
	''' borders created by the scroll pane. getInsets() can be used
	''' to get the current value for the insets.  If the value of
	''' scrollbarsAlwaysVisible is false, then the value of the insets
	''' will change dynamically depending on whether the scrollbars are
	''' currently visible or not.
	''' 
	''' @author      Tom Ball
	''' @author      Amy Fowler
	''' @author      Tim Prinzing
	''' </summary>
	Public Class ScrollPane
		Inherits Container
		Implements Accessible


		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Specifies that horizontal/vertical scrollbar should be shown
		''' only when the size of the child exceeds the size of the scrollpane
		''' in the horizontal/vertical dimension.
		''' </summary>
		Public Const SCROLLBARS_AS_NEEDED As Integer = 0

		''' <summary>
		''' Specifies that horizontal/vertical scrollbars should always be
		''' shown regardless of the respective sizes of the scrollpane and child.
		''' </summary>
		Public Const SCROLLBARS_ALWAYS As Integer = 1

		''' <summary>
		''' Specifies that horizontal/vertical scrollbars should never be shown
		''' regardless of the respective sizes of the scrollpane and child.
		''' </summary>
		Public Const SCROLLBARS_NEVER As Integer = 2

		''' <summary>
		''' There are 3 ways in which a scroll bar can be displayed.
		''' This integer will represent one of these 3 displays -
		''' (SCROLLBARS_ALWAYS, SCROLLBARS_AS_NEEDED, SCROLLBARS_NEVER)
		''' 
		''' @serial </summary>
		''' <seealso cref= #getScrollbarDisplayPolicy </seealso>
		Private scrollbarDisplayPolicy As Integer

		''' <summary>
		''' An adjustable vertical scrollbar.
		''' It is important to note that you must <em>NOT</em> call 3
		''' <code>Adjustable</code> methods, namely:
		''' <code>setMinimum()</code>, <code>setMaximum()</code>,
		''' <code>setVisibleAmount()</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getVAdjustable </seealso>
		Private vAdjustable As ScrollPaneAdjustable

		''' <summary>
		''' An adjustable horizontal scrollbar.
		''' It is important to note that you must <em>NOT</em> call 3
		''' <code>Adjustable</code> methods, namely:
		''' <code>setMinimum()</code>, <code>setMaximum()</code>,
		''' <code>setVisibleAmount()</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getHAdjustable </seealso>
		Private hAdjustable As ScrollPaneAdjustable

		Private Const base As String = "scrollpane"
		Private Shared nameCounter As Integer = 0

		Private Const defaultWheelScroll As Boolean = True

		''' <summary>
		''' Indicates whether or not scrolling should take place when a
		''' MouseWheelEvent is received.
		''' 
		''' @serial
		''' @since 1.4
		''' </summary>
		Private wheelScrollingEnabled As Boolean = defaultWheelScroll

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 7956609840827222915L

		''' <summary>
		''' Create a new scrollpane container with a scrollbar display
		''' policy of "as needed". </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''     returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New(SCROLLBARS_AS_NEEDED)
		End Sub

		''' <summary>
		''' Create a new scrollpane container. </summary>
		''' <param name="scrollbarDisplayPolicy"> policy for when scrollbars should be shown </param>
		''' <exception cref="IllegalArgumentException"> if the specified scrollbar
		'''     display policy is invalid </exception>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''     returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal scrollbarDisplayPolicy As Integer)
			GraphicsEnvironment.checkHeadless()
			Me.layoutMgr = Nothing
			Me.width = 100
			Me.height = 100
			Select Case scrollbarDisplayPolicy
				Case SCROLLBARS_NEVER, SCROLLBARS_AS_NEEDED, SCROLLBARS_ALWAYS
					Me.scrollbarDisplayPolicy = scrollbarDisplayPolicy
				Case Else
					Throw New IllegalArgumentException("illegal scrollbar display policy")
			End Select

			vAdjustable = New ScrollPaneAdjustable(Me, New PeerFixer(Me, Me), Adjustable.VERTICAL)
			hAdjustable = New ScrollPaneAdjustable(Me, New PeerFixer(Me, Me), Adjustable.HORIZONTAL)
			wheelScrollingEnabled = defaultWheelScroll
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(ScrollPane)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		' The scrollpane won't work with a windowless child... it assumes
		' it is moving a child window around so the windowless child is
		' wrapped with a window.
		Private Sub addToPanel(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			Dim child As New Panel
			child.layout = New BorderLayout
			child.add(comp)
			MyBase.addImpl(child, constraints, index)
			validate()
		End Sub

		''' <summary>
		''' Adds the specified component to this scroll pane container.
		''' If the scroll pane has an existing child component, that
		''' component is removed and the new one is added. </summary>
		''' <param name="comp"> the component to be added </param>
		''' <param name="constraints">  not applicable </param>
		''' <param name="index"> position of child component (must be &lt;= 0) </param>
		Protected Friend NotOverridable Overrides Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			SyncLock treeLock
				If componentCount > 0 Then remove(0)
				If index > 0 Then Throw New IllegalArgumentException("position greater than 0")

				If Not sun.awt.SunToolkit.isLightweightOrUnknown(comp) Then
					MyBase.addImpl(comp, constraints, index)
				Else
					addToPanel(comp, constraints, index)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the display policy for the scrollbars. </summary>
		''' <returns> the display policy for the scrollbars </returns>
		Public Overridable Property scrollbarDisplayPolicy As Integer
			Get
				Return scrollbarDisplayPolicy
			End Get
		End Property

		''' <summary>
		''' Returns the current size of the scroll pane's view port. </summary>
		''' <returns> the size of the view port in pixels </returns>
		Public Overridable Property viewportSize As Dimension
			Get
				Dim i As Insets = insets
				Return New Dimension(width - i.right - i.left, height - i.top - i.bottom)
			End Get
		End Property

		''' <summary>
		''' Returns the height that would be occupied by a horizontal
		''' scrollbar, which is independent of whether it is currently
		''' displayed by the scroll pane or not. </summary>
		''' <returns> the height of a horizontal scrollbar in pixels </returns>
		Public Overridable Property hScrollbarHeight As Integer
			Get
				Dim h As Integer = 0
				If scrollbarDisplayPolicy <> SCROLLBARS_NEVER Then
					Dim peer_Renamed As java.awt.peer.ScrollPanePeer = CType(Me.peer, java.awt.peer.ScrollPanePeer)
					If peer_Renamed IsNot Nothing Then h = peer_Renamed.hScrollbarHeight
				End If
				Return h
			End Get
		End Property

		''' <summary>
		''' Returns the width that would be occupied by a vertical
		''' scrollbar, which is independent of whether it is currently
		''' displayed by the scroll pane or not. </summary>
		''' <returns> the width of a vertical scrollbar in pixels </returns>
		Public Overridable Property vScrollbarWidth As Integer
			Get
				Dim w As Integer = 0
				If scrollbarDisplayPolicy <> SCROLLBARS_NEVER Then
					Dim peer_Renamed As java.awt.peer.ScrollPanePeer = CType(Me.peer, java.awt.peer.ScrollPanePeer)
					If peer_Renamed IsNot Nothing Then w = peer_Renamed.vScrollbarWidth
				End If
				Return w
			End Get
		End Property

		''' <summary>
		''' Returns the <code>ScrollPaneAdjustable</code> object which
		''' represents the state of the vertical scrollbar.
		''' The declared return type of this method is
		''' <code>Adjustable</code> to maintain backward compatibility. </summary>
		''' <seealso cref= java.awt.ScrollPaneAdjustable </seealso>
		Public Overridable Property vAdjustable As Adjustable
			Get
				Return vAdjustable
			End Get
		End Property

		''' <summary>
		''' Returns the <code>ScrollPaneAdjustable</code> object which
		''' represents the state of the horizontal scrollbar.
		''' The declared return type of this method is
		''' <code>Adjustable</code> to maintain backward compatibility. </summary>
		''' <seealso cref= java.awt.ScrollPaneAdjustable </seealso>
		Public Overridable Property hAdjustable As Adjustable
			Get
				Return hAdjustable
			End Get
		End Property

		''' <summary>
		''' Scrolls to the specified position within the child component.
		''' A call to this method is only valid if the scroll pane contains
		''' a child.  Specifying a position outside of the legal scrolling bounds
		''' of the child will scroll to the closest legal position.
		''' Legal bounds are defined to be the rectangle:
		''' x = 0, y = 0, width = (child width - view port width),
		''' height = (child height - view port height).
		''' This is a convenience method which interfaces with the Adjustable
		''' objects which represent the state of the scrollbars. </summary>
		''' <param name="x"> the x position to scroll to </param>
		''' <param name="y"> the y position to scroll to </param>
		''' <exception cref="NullPointerException"> if the scrollpane does not contain
		'''     a child </exception>
		Public Overridable Sub setScrollPosition(ByVal x As Integer, ByVal y As Integer)
			SyncLock treeLock
				If componentCount=0 Then Throw New NullPointerException("child is null")
				hAdjustable.value = x
				vAdjustable.value = y
			End SyncLock
		End Sub

		''' <summary>
		''' Scrolls to the specified position within the child component.
		''' A call to this method is only valid if the scroll pane contains
		''' a child and the specified position is within legal scrolling bounds
		''' of the child.  Specifying a position outside of the legal scrolling
		''' bounds of the child will scroll to the closest legal position.
		''' Legal bounds are defined to be the rectangle:
		''' x = 0, y = 0, width = (child width - view port width),
		''' height = (child height - view port height).
		''' This is a convenience method which interfaces with the Adjustable
		''' objects which represent the state of the scrollbars. </summary>
		''' <param name="p"> the Point representing the position to scroll to </param>
		''' <exception cref="NullPointerException"> if {@code p} is {@code null} </exception>
		Public Overridable Property scrollPosition As Point
			Set(ByVal p As Point)
				scrollPositionion(p.x, p.y)
			End Set
			Get
				SyncLock treeLock
					If componentCount=0 Then Throw New NullPointerException("child is null")
					Return New Point(hAdjustable.value, vAdjustable.value)
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the current x,y position within the child which is displayed
		''' at the 0,0 location of the scrolled panel's view port.
		''' This is a convenience method which interfaces with the adjustable
		''' objects which represent the state of the scrollbars. </summary>
		''' <returns> the coordinate position for the current scroll position </returns>
		''' <exception cref="NullPointerException"> if the scrollpane does not contain
		'''     a child </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Sets the layout manager for this container.  This method is
		''' overridden to prevent the layout mgr from being set. </summary>
		''' <param name="mgr"> the specified layout manager </param>
		Public NotOverridable Overrides Property layout As LayoutManager
			Set(ByVal mgr As LayoutManager)
				Throw New AWTError("ScrollPane controls layout")
			End Set
		End Property

		''' <summary>
		''' Lays out this container by resizing its child to its preferred size.
		''' If the new preferred size of the child causes the current scroll
		''' position to be invalid, the scroll position is set to the closest
		''' valid position.
		''' </summary>
		''' <seealso cref= Component#validate </seealso>
		Public Overrides Sub doLayout()
			layout()
		End Sub

		''' <summary>
		''' Determine the size to allocate the child component.
		''' If the viewport area is bigger than the preferred size
		''' of the child then the child is allocated enough
		''' to fill the viewport, otherwise the child is given
		''' it's preferred size.
		''' </summary>
		Friend Overridable Function calculateChildSize() As Dimension
			'
			' calculate the view size, accounting for border but not scrollbars
			' - don't use right/bottom insets since they vary depending
			'   on whether or not scrollbars were displayed on last resize
			'
			Dim size_Renamed As Dimension = size
			Dim insets_Renamed As Insets = insets
			Dim viewWidth As Integer = size_Renamed.width - insets_Renamed.left*2
			Dim viewHeight As Integer = size_Renamed.height - insets_Renamed.top*2

			'
			' determine whether or not horz or vert scrollbars will be displayed
			'
			Dim vbarOn As Boolean
			Dim hbarOn As Boolean
			Dim child As Component = getComponent(0)
			Dim childSize As New Dimension(child.preferredSize)

			If scrollbarDisplayPolicy = SCROLLBARS_AS_NEEDED Then
				vbarOn = childSize.height > viewHeight
				hbarOn = childSize.width > viewWidth
			ElseIf scrollbarDisplayPolicy = SCROLLBARS_ALWAYS Then
					hbarOn = True
					vbarOn = hbarOn ' SCROLLBARS_NEVER
			Else
					hbarOn = False
					vbarOn = hbarOn
			End If

			'
			' adjust predicted view size to account for scrollbars
			'
			Dim vbarWidth As Integer = vScrollbarWidth
			Dim hbarHeight As Integer = hScrollbarHeight
			If vbarOn Then viewWidth -= vbarWidth
			If hbarOn Then viewHeight -= hbarHeight

			'
			' if child is smaller than view, size it up
			'
			If childSize.width < viewWidth Then childSize.width = viewWidth
			If childSize.height < viewHeight Then childSize.height = viewHeight

			Return childSize
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>doLayout()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Sub layout()
			If componentCount=0 Then Return
			Dim c As Component = getComponent(0)
			Dim p As Point = scrollPosition
			Dim cs As Dimension = calculateChildSize()
			Dim vs As Dimension = viewportSize

			c.reshape(- p.x, - p.y, cs.width, cs.height)
			Dim peer_Renamed As java.awt.peer.ScrollPanePeer = CType(Me.peer, java.awt.peer.ScrollPanePeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.childResized(cs.width, cs.height)

			' update adjustables... the viewport size may have changed
			' with the scrollbars coming or going so the viewport size
			' is updated before the adjustables.
			vs = viewportSize
			hAdjustable.spanpan(0, cs.width, vs.width)
			vAdjustable.spanpan(0, cs.height, vs.height)
		End Sub

		''' <summary>
		''' Prints the component in this scroll pane. </summary>
		''' <param name="g"> the specified Graphics window </param>
		''' <seealso cref= Component#print </seealso>
		''' <seealso cref= Component#printAll </seealso>
		Public Overrides Sub printComponents(ByVal g As Graphics)
			If componentCount=0 Then Return
			Dim c As Component = getComponent(0)
			Dim p As Point = c.location
			Dim vs As Dimension = viewportSize
			Dim i As Insets = insets

			Dim cg As Graphics = g.create()
			Try
				cg.clipRect(i.left, i.top, vs.width, vs.height)
				cg.translate(p.x, p.y)
				c.printAll(cg)
			Finally
				cg.Dispose()
			End Try
		End Sub

		''' <summary>
		''' Creates the scroll pane's peer.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock

				Dim vAdjustableValue As Integer = 0
				Dim hAdjustableValue As Integer = 0

				' Bug 4124460. Save the current adjustable values,
				' so they can be restored after addnotify. Set the
				' adjustables to 0, to prevent crashes for possible
				' negative values.
				If componentCount > 0 Then
					vAdjustableValue = vAdjustable.value
					hAdjustableValue = hAdjustable.value
					vAdjustable.value = 0
					hAdjustable.value = 0
				End If

				If peer Is Nothing Then peer = toolkit.createScrollPane(Me)
				MyBase.addNotify()

				' Bug 4124460. Restore the adjustable values.
				If componentCount > 0 Then
					vAdjustable.value = vAdjustableValue
					hAdjustable.value = hAdjustableValue
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Returns a string representing the state of this
		''' <code>ScrollPane</code>. This
		''' method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns> the parameter string of this scroll pane </returns>
		Public Overrides Function paramString() As String
			Dim sdpStr As String
			Select Case scrollbarDisplayPolicy
				Case SCROLLBARS_AS_NEEDED
					sdpStr = "as-needed"
				Case SCROLLBARS_ALWAYS
					sdpStr = "always"
				Case SCROLLBARS_NEVER
					sdpStr = "never"
				Case Else
					sdpStr = "invalid display policy"
			End Select
			Dim p As Point = If(componentCount>0, scrollPosition, New Point(0,0))
			Dim i As Insets = insets
			Return MyBase.paramString() & ",ScrollPosition=(" & p.x & "," & p.y & ")" & ",Insets=(" & i.top & "," & i.left & "," & i.bottom & "," & i.right & ")" & ",ScrollbarDisplayPolicy=" & sdpStr & ",wheelScrollingEnabled=" & wheelScrollingEnabled
		End Function

		Friend Overrides Sub autoProcessMouseWheel(ByVal e As MouseWheelEvent)
			processMouseWheelEvent(e)
		End Sub

		''' <summary>
		''' Process mouse wheel events that are delivered to this
		''' <code>ScrollPane</code> by scrolling an appropriate amount.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e">  the mouse wheel event
		''' @since 1.4 </param>
		Protected Friend Overrides Sub processMouseWheelEvent(ByVal e As MouseWheelEvent)
			If wheelScrollingEnabled Then
				sun.awt.ScrollPaneWheelScroller.handleWheelScrolling(Me, e)
				e.consume()
			End If
			MyBase.processMouseWheelEvent(e)
		End Sub

		''' <summary>
		''' If wheel scrolling is enabled, we return true for MouseWheelEvents
		''' @since 1.4
		''' </summary>
		Protected Friend Overrides Function eventTypeEnabled(ByVal type As Integer) As Boolean
			If type = MouseEvent.MOUSE_WHEEL AndAlso wheelScrollingEnabled Then
				Return True
			Else
				Return MyBase.eventTypeEnabled(type)
			End If
		End Function

		''' <summary>
		''' Enables/disables scrolling in response to movement of the mouse wheel.
		''' Wheel scrolling is enabled by default.
		''' </summary>
		''' <param name="handleWheel">   <code>true</code> if scrolling should be done
		'''                      automatically for a MouseWheelEvent,
		'''                      <code>false</code> otherwise. </param>
		''' <seealso cref= #isWheelScrollingEnabled </seealso>
		''' <seealso cref= java.awt.event.MouseWheelEvent </seealso>
		''' <seealso cref= java.awt.event.MouseWheelListener
		''' @since 1.4 </seealso>
		Public Overridable Property wheelScrollingEnabled As Boolean
			Set(ByVal handleWheel As Boolean)
				wheelScrollingEnabled = handleWheel
			End Set
			Get
				Return wheelScrollingEnabled
			End Get
		End Property



		''' <summary>
		''' Writes default serializable fields to stream.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' 4352819: We only need this degenerate writeObject to make
			' it safe for future versions of this class to write optional
			' data to the stream.
			s.defaultWriteObject()
		End Sub

		''' <summary>
		''' Reads default serializable fields to stream. </summary>
		''' <exception cref="HeadlessException"> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns
		''' <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			GraphicsEnvironment.checkHeadless()
			' 4352819: Gotcha!  Cannot use s.defaultReadObject here and
			' then continue with reading optional data.  Use GetField instead.
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			' Old fields
			scrollbarDisplayPolicy = f.get("scrollbarDisplayPolicy", SCROLLBARS_AS_NEEDED)
			hAdjustable = CType(f.get("hAdjustable", Nothing), ScrollPaneAdjustable)
			vAdjustable = CType(f.get("vAdjustable", Nothing), ScrollPaneAdjustable)

			' Since 1.4
			wheelScrollingEnabled = f.get("wheelScrollingEnabled", defaultWheelScroll)

	'      // Note to future maintainers
	'      if (f.defaulted("wheelScrollingEnabled")) {
	'          // We are reading pre-1.4 stream that doesn't have
	'          // optional data, not even the TC_ENDBLOCKDATA marker.
	'          // Reading anything after this point is unsafe as we will
	'          // read unrelated objects further down the stream (4352819).
	'      }
	'      else {
	'          // Reading data from 1.4 or later, it's ok to try to read
	'          // optional data as OptionalDataException with eof == true
	'          // will be correctly reported
	'      }
		End Sub

		<Serializable> _
		Friend Class PeerFixer
			Implements AdjustmentListener

			Private ReadOnly outerInstance As ScrollPane

			Private Const serialVersionUID As Long = 1043664721353696630L

			Friend Sub New(ByVal outerInstance As ScrollPane, ByVal scroller As ScrollPane)
					Me.outerInstance = outerInstance
				Me.scroller = scroller
			End Sub

			''' <summary>
			''' Invoked when the value of the adjustable has changed.
			''' </summary>
			Public Overridable Sub adjustmentValueChanged(ByVal e As AdjustmentEvent) Implements AdjustmentListener.adjustmentValueChanged
				Dim adj As Adjustable = e.adjustable
				Dim value As Integer = e.value
				Dim peer As java.awt.peer.ScrollPanePeer = CType(scroller.peer, java.awt.peer.ScrollPanePeer)
				If peer IsNot Nothing Then peer.valuelue(adj, value)

				Dim c As Component = scroller.getComponent(0)
				Select Case adj.orientation
				Case Adjustable.VERTICAL
					c.move(c.location.x, -(value))
				Case Adjustable.HORIZONTAL
					c.move(-(value), c.location.y)
				Case Else
					Throw New IllegalArgumentException("Illegal adjustable orientation")
				End Select
			End Sub

			Private scroller As ScrollPane
		End Class


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this ScrollPane.
		''' For scroll panes, the AccessibleContext takes the form of an
		''' AccessibleAWTScrollPane.
		''' A new AccessibleAWTScrollPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTScrollPane that serves as the
		'''         AccessibleContext of this ScrollPane
		''' @since 1.3 </returns>
		Public  Overrides ReadOnly Property  accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTScrollPane(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>ScrollPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to scroll pane user-interface
		''' elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTScrollPane
			Inherits AccessibleAWTContainer

			Private ReadOnly outerInstance As ScrollPane

			Public Sub New(ByVal outerInstance As ScrollPane)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 6100703663886637L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SCROLL_PANE
				End Get
			End Property

		End Class ' class AccessibleAWTScrollPane

	End Class

	'
	' * In JDK 1.1.1, the pkg private class java.awt.PeerFixer was moved to
	' * become an inner class of ScrollPane, which broke serialization
	' * for ScrollPane objects using JDK 1.1.
	' * Instead of moving it back out here, which would break all JDK 1.1.x
	' * releases, we keep PeerFixer in both places. Because of the scoping rules,
	' * the PeerFixer that is used in ScrollPane will be the one that is the
	' * inner class. This pkg private PeerFixer class below will only be used
	' * if the Java 2 platform is used to deserialize ScrollPane objects that were serialized
	' * using JDK1.1
	' 
	<Serializable> _
	Friend Class PeerFixer
		Implements AdjustmentListener

	'    
	'     * serialVersionUID
	'     
		Private Const serialVersionUID As Long = 7051237413532574756L

		Friend Sub New(ByVal scroller As ScrollPane)
			Me.scroller = scroller
		End Sub

		''' <summary>
		''' Invoked when the value of the adjustable has changed.
		''' </summary>
		Public Overridable Sub adjustmentValueChanged(ByVal e As AdjustmentEvent) Implements AdjustmentListener.adjustmentValueChanged
			Dim adj As Adjustable = e.adjustable
			Dim value As Integer = e.value
			Dim peer As java.awt.peer.ScrollPanePeer = CType(scroller.peer, java.awt.peer.ScrollPanePeer)
			If peer IsNot Nothing Then peer.valuelue(adj, value)

			Dim c As Component = scroller.getComponent(0)
			Select Case adj.orientation
			Case Adjustable.VERTICAL
				c.move(c.location.x, -(value))
			Case Adjustable.HORIZONTAL
				c.move(-(value), c.location.y)
			Case Else
				Throw New IllegalArgumentException("Illegal adjustable orientation")
			End Select
		End Sub

		Private scroller As ScrollPane
	End Class

End Namespace
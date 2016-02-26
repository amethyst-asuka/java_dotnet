Imports System
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports javax.accessibility

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Scrollbar</code> class embodies a scroll bar, a
	''' familiar user-interface object. A scroll bar provides a
	''' convenient means for allowing a user to select from a
	''' range of values. The following three vertical
	''' scroll bars could be used as slider controls to pick
	''' the red, green, and blue components of a color:
	''' <p>
	''' <img src="doc-files/Scrollbar-1.gif" alt="Image shows 3 vertical sliders, side-by-side."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' Each scroll bar in this example could be created with
	''' code similar to the following:
	''' 
	''' <hr><blockquote><pre>
	''' redSlider=new Scrollbar(Scrollbar.VERTICAL, 0, 1, 0, 255);
	''' add(redSlider);
	''' </pre></blockquote><hr>
	''' <p>
	''' Alternatively, a scroll bar can represent a range of values. For
	''' example, if a scroll bar is used for scrolling through text, the
	''' width of the "bubble" (also called the "thumb" or "scroll box")
	''' can be used to represent the amount of text that is visible.
	''' Here is an example of a scroll bar that represents a range:
	''' <p>
	''' <img src="doc-files/Scrollbar-2.gif"
	''' alt="Image shows horizontal slider with starting range of 0 and ending range of 300. The slider thumb is labeled 60."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' The value range represented by the bubble in this example
	''' is the <em>visible amount</em>. The horizontal scroll bar
	''' in this example could be created with code like the following:
	''' 
	''' <hr><blockquote><pre>
	''' ranger = new Scrollbar(Scrollbar.HORIZONTAL, 0, 60, 0, 300);
	''' add(ranger);
	''' </pre></blockquote><hr>
	''' <p>
	''' Note that the actual maximum value of the scroll bar is the
	''' <code>maximum</code> minus the <code>visible amount</code>.
	''' In the previous example, because the <code>maximum</code> is
	''' 300 and the <code>visible amount</code> is 60, the actual maximum
	''' value is 240.  The range of the scrollbar track is 0 - 300.
	''' The left side of the bubble indicates the value of the
	''' scroll bar.
	''' <p>
	''' Normally, the user changes the value of the scroll bar by
	''' making a gesture with the mouse. For example, the user can
	''' drag the scroll bar's bubble up and down, or click in the
	''' scroll bar's unit increment or block increment areas. Keyboard
	''' gestures can also be mapped to the scroll bar. By convention,
	''' the <b>Page&nbsp;Up</b> and <b>Page&nbsp;Down</b>
	''' keys are equivalent to clicking in the scroll bar's block
	''' increment and block decrement areas.
	''' <p>
	''' When the user changes the value of the scroll bar, the scroll bar
	''' receives an instance of <code>AdjustmentEvent</code>.
	''' The scroll bar processes this event, passing it along to
	''' any registered listeners.
	''' <p>
	''' Any object that wishes to be notified of changes to the
	''' scroll bar's value should implement
	''' <code>AdjustmentListener</code>, an interface defined in
	''' the package <code>java.awt.event</code>.
	''' Listeners can be added and removed dynamically by calling
	''' the methods <code>addAdjustmentListener</code> and
	''' <code>removeAdjustmentListener</code>.
	''' <p>
	''' The <code>AdjustmentEvent</code> class defines five types
	''' of adjustment event, listed here:
	''' 
	''' <ul>
	''' <li><code>AdjustmentEvent.TRACK</code> is sent out when the
	''' user drags the scroll bar's bubble.
	''' <li><code>AdjustmentEvent.UNIT_INCREMENT</code> is sent out
	''' when the user clicks in the left arrow of a horizontal scroll
	''' bar, or the top arrow of a vertical scroll bar, or makes the
	''' equivalent gesture from the keyboard.
	''' <li><code>AdjustmentEvent.UNIT_DECREMENT</code> is sent out
	''' when the user clicks in the right arrow of a horizontal scroll
	''' bar, or the bottom arrow of a vertical scroll bar, or makes the
	''' equivalent gesture from the keyboard.
	''' <li><code>AdjustmentEvent.BLOCK_INCREMENT</code> is sent out
	''' when the user clicks in the track, to the left of the bubble
	''' on a horizontal scroll bar, or above the bubble on a vertical
	''' scroll bar. By convention, the <b>Page&nbsp;Up</b>
	''' key is equivalent, if the user is using a keyboard that
	''' defines a <b>Page&nbsp;Up</b> key.
	''' <li><code>AdjustmentEvent.BLOCK_DECREMENT</code> is sent out
	''' when the user clicks in the track, to the right of the bubble
	''' on a horizontal scroll bar, or below the bubble on a vertical
	''' scroll bar. By convention, the <b>Page&nbsp;Down</b>
	''' key is equivalent, if the user is using a keyboard that
	''' defines a <b>Page&nbsp;Down</b> key.
	''' </ul>
	''' <p>
	''' The JDK&nbsp;1.0 event system is supported for backwards
	''' compatibility, but its use with newer versions of the platform is
	''' discouraged. The five types of adjustment events introduced
	''' with JDK&nbsp;1.1 correspond to the five event types
	''' that are associated with scroll bars in previous platform versions.
	''' The following list gives the adjustment event type,
	''' and the corresponding JDK&nbsp;1.0 event type it replaces.
	''' 
	''' <ul>
	''' <li><code>AdjustmentEvent.TRACK</code> replaces
	''' <code>Event.SCROLL_ABSOLUTE</code>
	''' <li><code>AdjustmentEvent.UNIT_INCREMENT</code> replaces
	''' <code>Event.SCROLL_LINE_UP</code>
	''' <li><code>AdjustmentEvent.UNIT_DECREMENT</code> replaces
	''' <code>Event.SCROLL_LINE_DOWN</code>
	''' <li><code>AdjustmentEvent.BLOCK_INCREMENT</code> replaces
	''' <code>Event.SCROLL_PAGE_UP</code>
	''' <li><code>AdjustmentEvent.BLOCK_DECREMENT</code> replaces
	''' <code>Event.SCROLL_PAGE_DOWN</code>
	''' </ul>
	''' <p>
	''' <b>Note</b>: We recommend using a <code>Scrollbar</code>
	''' for value selection only.  If you want to implement
	''' a scrollable component inside a container, we recommend you use
	''' a <seealso cref="ScrollPane ScrollPane"/>. If you use a
	''' <code>Scrollbar</code> for this purpose, you are likely to
	''' encounter issues with painting, key handling, sizing and
	''' positioning.
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.event.AdjustmentEvent </seealso>
	''' <seealso cref=         java.awt.event.AdjustmentListener
	''' @since       JDK1.0 </seealso>
	Public Class Scrollbar
		Inherits Component
		Implements Adjustable, Accessible

		''' <summary>
		''' A constant that indicates a horizontal scroll bar.
		''' </summary>
		Public Const HORIZONTAL As Integer = 0

		''' <summary>
		''' A constant that indicates a vertical scroll bar.
		''' </summary>
		Public Const VERTICAL As Integer = 1

		''' <summary>
		''' The value of the <code>Scrollbar</code>.
		''' This property must be greater than or equal to <code>minimum</code>
		''' and less than or equal to
		''' <code>maximum - visibleAmount</code>
		''' 
		''' @serial </summary>
		''' <seealso cref= #getValue </seealso>
		''' <seealso cref= #setValue </seealso>
		Friend value As Integer

		''' <summary>
		''' The maximum value of the <code>Scrollbar</code>.
		''' This value must be greater than the <code>minimum</code>
		''' value.<br>
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMaximum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		Friend maximum As Integer

		''' <summary>
		''' The minimum value of the <code>Scrollbar</code>.
		''' This value must be less than the <code>maximum</code>
		''' value.<br>
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMinimum </seealso>
		''' <seealso cref= #setMinimum </seealso>
		Friend minimum As Integer

		''' <summary>
		''' The size of the <code>Scrollbar</code>'s bubble.
		''' When a scroll bar is used to select a range of values,
		''' the visibleAmount represents the size of this range.
		''' Depending on platform, this may be visually indicated
		''' by the size of the bubble.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getVisibleAmount </seealso>
		''' <seealso cref= #setVisibleAmount </seealso>
		Friend visibleAmount As Integer

		''' <summary>
		''' The <code>Scrollbar</code>'s orientation--being either horizontal
		''' or vertical.
		''' This value should be specified when the scrollbar is created.<BR>
		''' orientation can be either : <code>VERTICAL</code> or
		''' <code>HORIZONTAL</code> only.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getOrientation </seealso>
		''' <seealso cref= #setOrientation </seealso>
		Friend orientation As Integer

		''' <summary>
		''' The amount by which the scrollbar value will change when going
		''' up or down by a line.
		''' This value must be greater than zero.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLineIncrement </seealso>
		''' <seealso cref= #setLineIncrement </seealso>
		Friend lineIncrement As Integer = 1

		''' <summary>
		''' The amount by which the scrollbar value will change when going
		''' up or down by a page.
		''' This value must be greater than zero.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getPageIncrement </seealso>
		''' <seealso cref= #setPageIncrement </seealso>
		Friend pageIncrement As Integer = 10

		''' <summary>
		''' The adjusting status of the <code>Scrollbar</code>.
		''' True if the value is in the process of changing as a result of
		''' actions being taken by the user.
		''' </summary>
		''' <seealso cref= #getValueIsAdjusting </seealso>
		''' <seealso cref= #setValueIsAdjusting
		''' @since 1.4 </seealso>
		<NonSerialized> _
		Friend isAdjusting As Boolean

		<NonSerialized> _
		Friend adjustmentListener As AdjustmentListener

		Private Const base As String = "scrollbar"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 8451667562882310543L

		''' <summary>
		''' Initialize JNI field and method IDs.
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
		''' Constructs a new vertical scroll bar.
		''' The default properties of the scroll bar are listed in
		''' the following table:
		''' 
		''' <table border=1 summary="Scrollbar default properties">
		''' <tr>
		'''   <th>Property</th>
		'''   <th>Description</th>
		'''   <th>Default Value</th>
		''' </tr>
		''' <tr>
		'''   <td>orientation</td>
		'''   <td>indicates whether the scroll bar is vertical
		'''   <br>or horizontal</td>
		'''   <td><code>Scrollbar.VERTICAL</code></td>
		''' </tr>
		''' <tr>
		'''   <td>value</td>
		'''   <td>value which controls the location
		'''   <br>of the scroll bar's bubble</td>
		'''   <td>0</td>
		''' </tr>
		''' <tr>
		'''   <td>visible amount</td>
		'''   <td>visible amount of the scroll bar's range,
		'''   <br>typically represented by the size of the
		'''   <br>scroll bar's bubble</td>
		'''   <td>10</td>
		''' </tr>
		''' <tr>
		'''   <td>minimum</td>
		'''   <td>minimum value of the scroll bar</td>
		'''   <td>0</td>
		''' </tr>
		''' <tr>
		'''   <td>maximum</td>
		'''   <td>maximum value of the scroll bar</td>
		'''   <td>100</td>
		''' </tr>
		''' <tr>
		'''   <td>unit increment</td>
		'''   <td>amount the value changes when the
		'''   <br>Line Up or Line Down key is pressed,
		'''   <br>or when the end arrows of the scrollbar
		'''   <br>are clicked </td>
		'''   <td>1</td>
		''' </tr>
		''' <tr>
		'''   <td>block increment</td>
		'''   <td>amount the value changes when the
		'''   <br>Page Up or Page Down key is pressed,
		'''   <br>or when the scrollbar track is clicked
		'''   <br>on either side of the bubble </td>
		'''   <td>10</td>
		''' </tr>
		''' </table>
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New(VERTICAL, 0, 10, 0, 100)
		End Sub

		''' <summary>
		''' Constructs a new scroll bar with the specified orientation.
		''' <p>
		''' The <code>orientation</code> argument must take one of the two
		''' values <code>Scrollbar.HORIZONTAL</code>,
		''' or <code>Scrollbar.VERTICAL</code>,
		''' indicating a horizontal or vertical scroll bar, respectively.
		''' </summary>
		''' <param name="orientation">   indicates the orientation of the scroll bar </param>
		''' <exception cref="IllegalArgumentException">    when an illegal value for
		'''                    the <code>orientation</code> argument is supplied </exception>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal orientation As Integer)
			Me.New(orientation, 0, 10, 0, 100)
		End Sub

		''' <summary>
		''' Constructs a new scroll bar with the specified orientation,
		''' initial value, visible amount, and minimum and maximum values.
		''' <p>
		''' The <code>orientation</code> argument must take one of the two
		''' values <code>Scrollbar.HORIZONTAL</code>,
		''' or <code>Scrollbar.VERTICAL</code>,
		''' indicating a horizontal or vertical scroll bar, respectively.
		''' <p>
		''' The parameters supplied to this constructor are subject to the
		''' constraints described in <seealso cref="#setValues(int, int, int, int)"/>.
		''' </summary>
		''' <param name="orientation">   indicates the orientation of the scroll bar. </param>
		''' <param name="value">     the initial value of the scroll bar </param>
		''' <param name="visible">   the visible amount of the scroll bar, typically
		'''                      represented by the size of the bubble </param>
		''' <param name="minimum">   the minimum value of the scroll bar </param>
		''' <param name="maximum">   the maximum value of the scroll bar </param>
		''' <exception cref="IllegalArgumentException">    when an illegal value for
		'''                    the <code>orientation</code> argument is supplied </exception>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= #setValues </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal orientation As Integer, ByVal value As Integer, ByVal visible As Integer, ByVal minimum As Integer, ByVal maximum As Integer)
			GraphicsEnvironment.checkHeadless()
			Select Case orientation
			  Case HORIZONTAL, VERTICAL
				Me.orientation = orientation
			  Case Else
				Throw New IllegalArgumentException("illegal scrollbar orientation")
			End Select
			valuesues(value, visible, minimum, maximum)
		End Sub

		''' <summary>
		''' Constructs a name for this component.  Called by <code>getName</code>
		''' when the name is <code>null</code>.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Scrollbar)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the <code>Scrollbar</code>'s peer.  The peer allows you to modify
		''' the appearance of the <code>Scrollbar</code> without changing any of its
		''' functionality.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createScrollbar(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the orientation of this scroll bar.
		''' </summary>
		''' <returns>    the orientation of this scroll bar, either
		'''               <code>Scrollbar.HORIZONTAL</code> or
		'''               <code>Scrollbar.VERTICAL</code> </returns>
		''' <seealso cref=       java.awt.Scrollbar#setOrientation </seealso>
		Public Overridable Property orientation As Integer Implements Adjustable.getOrientation
			Get
				Return orientation
			End Get
			Set(ByVal orientation As Integer)
				SyncLock treeLock
					If orientation = Me.orientation Then Return
					Select Case orientation
						Case HORIZONTAL, VERTICAL
							Me.orientation = orientation
						Case Else
							Throw New IllegalArgumentException("illegal scrollbar orientation")
					End Select
					' Create a new peer with the specified orientation. 
					If peer IsNot Nothing Then
						removeNotify()
						addNotify()
						invalidate()
					End If
				End SyncLock
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, (If(orientation = VERTICAL, AccessibleState.HORIZONTAL, AccessibleState.VERTICAL)), (If(orientation = VERTICAL, AccessibleState.VERTICAL, AccessibleState.HORIZONTAL)))
			End Set
		End Property


		''' <summary>
		''' Gets the current value of this scroll bar.
		''' </summary>
		''' <returns>      the current value of this scroll bar </returns>
		''' <seealso cref=         java.awt.Scrollbar#getMinimum </seealso>
		''' <seealso cref=         java.awt.Scrollbar#getMaximum </seealso>
		Public Overridable Property value As Integer Implements Adjustable.getValue
			Get
				Return value
			End Get
			Set(ByVal newValue As Integer)
				' Use setValues so that a consistent policy relating
				' minimum, maximum, visible amount, and value is enforced.
				valuesues(newValue, visibleAmount, minimum, maximum)
			End Set
		End Property


		''' <summary>
		''' Gets the minimum value of this scroll bar.
		''' </summary>
		''' <returns>      the minimum value of this scroll bar </returns>
		''' <seealso cref=         java.awt.Scrollbar#getValue </seealso>
		''' <seealso cref=         java.awt.Scrollbar#getMaximum </seealso>
		Public Overridable Property minimum As Integer Implements Adjustable.getMinimum
			Get
				Return minimum
			End Get
			Set(ByVal newMinimum As Integer)
				' No checks are necessary in this method since minimum is
				' the first variable checked in the setValues function.
    
				' Use setValues so that a consistent policy relating
				' minimum, maximum, visible amount, and value is enforced.
				valuesues(value, visibleAmount, newMinimum, maximum)
			End Set
		End Property


		''' <summary>
		''' Gets the maximum value of this scroll bar.
		''' </summary>
		''' <returns>      the maximum value of this scroll bar </returns>
		''' <seealso cref=         java.awt.Scrollbar#getValue </seealso>
		''' <seealso cref=         java.awt.Scrollbar#getMinimum </seealso>
		Public Overridable Property maximum As Integer Implements Adjustable.getMaximum
			Get
				Return maximum
			End Get
			Set(ByVal newMaximum As Integer)
				' minimum is checked first in setValues, so we need to
				' enforce minimum and maximum checks here.
				If newMaximum = Integer.MinValue Then newMaximum = Integer.MinValue + 1
    
				If minimum >= newMaximum Then minimum = newMaximum - 1
    
				' Use setValues so that a consistent policy relating
				' minimum, maximum, visible amount, and value is enforced.
				valuesues(value, visibleAmount, minimum, newMaximum)
			End Set
		End Property


		''' <summary>
		''' Gets the visible amount of this scroll bar.
		''' <p>
		''' When a scroll bar is used to select a range of values,
		''' the visible amount is used to represent the range of values
		''' that are currently visible.  The size of the scroll bar's
		''' bubble (also called a thumb or scroll box), usually gives a
		''' visual representation of the relationship of the visible
		''' amount to the range of the scroll bar.
		''' Note that depending on platform, the value of the visible amount property
		''' may not be visually indicated by the size of the bubble.
		''' <p>
		''' The scroll bar's bubble may not be displayed when it is not
		''' moveable (e.g. when it takes up the entire length of the
		''' scroll bar's track, or when the scroll bar is disabled).
		''' Whether the bubble is displayed or not will not affect
		''' the value returned by <code>getVisibleAmount</code>.
		''' </summary>
		''' <returns>      the visible amount of this scroll bar </returns>
		''' <seealso cref=         java.awt.Scrollbar#setVisibleAmount
		''' @since       JDK1.1 </seealso>
		Public Overridable Property visibleAmount As Integer Implements Adjustable.getVisibleAmount
			Get
				Return visible
			End Get
			Set(ByVal newAmount As Integer)
				' Use setValues so that a consistent policy relating
				' minimum, maximum, visible amount, and value is enforced.
				valuesues(value, newAmount, minimum, maximum)
			End Set
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getVisibleAmount()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property visible As Integer
			Get
				Return visibleAmount
			End Get
		End Property


		''' <summary>
		''' Sets the unit increment for this scroll bar.
		''' <p>
		''' The unit increment is the value that is added or subtracted
		''' when the user activates the unit increment area of the
		''' scroll bar, generally through a mouse or keyboard gesture
		''' that the scroll bar receives as an adjustment event.
		''' The unit increment must be greater than zero.
		''' Attepts to set the unit increment to a value lower than 1
		''' will result in a value of 1 being set.
		''' <p>
		''' In some operating systems, this property
		''' can be ignored by the underlying controls.
		''' </summary>
		''' <param name="v">  the amount by which to increment or decrement
		'''                         the scroll bar's value </param>
		''' <seealso cref=          java.awt.Scrollbar#getUnitIncrement
		''' @since        JDK1.1 </seealso>
		Public Overridable Property unitIncrement Implements Adjustable.setUnitIncrement As Integer
			Set(ByVal v As Integer)
				lineIncrement = v
			End Set
			Get
				Return lineIncrement
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setUnitIncrement(int)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property lineIncrement As Integer
			Set(ByVal v As Integer)
				Dim tmp As Integer = If(v < 1, 1, v)
    
				If lineIncrement = tmp Then Return
				lineIncrement = tmp
    
				Dim peer_Renamed As java.awt.peer.ScrollbarPeer = CType(Me.peer, java.awt.peer.ScrollbarPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.lineIncrement = lineIncrement
			End Set
			Get
				Return lineIncrement
			End Get
		End Property



		''' <summary>
		''' Sets the block increment for this scroll bar.
		''' <p>
		''' The block increment is the value that is added or subtracted
		''' when the user activates the block increment area of the
		''' scroll bar, generally through a mouse or keyboard gesture
		''' that the scroll bar receives as an adjustment event.
		''' The block increment must be greater than zero.
		''' Attepts to set the block increment to a value lower than 1
		''' will result in a value of 1 being set.
		''' </summary>
		''' <param name="v">  the amount by which to increment or decrement
		'''                         the scroll bar's value </param>
		''' <seealso cref=          java.awt.Scrollbar#getBlockIncrement
		''' @since        JDK1.1 </seealso>
		Public Overridable Property blockIncrement Implements Adjustable.setBlockIncrement As Integer
			Set(ByVal v As Integer)
				pageIncrement = v
			End Set
			Get
				Return pageIncrement
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setBlockIncrement()</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property pageIncrement As Integer
			Set(ByVal v As Integer)
				Dim tmp As Integer = If(v < 1, 1, v)
    
				If pageIncrement = tmp Then Return
				pageIncrement = tmp
    
				Dim peer_Renamed As java.awt.peer.ScrollbarPeer = CType(Me.peer, java.awt.peer.ScrollbarPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.pageIncrement = pageIncrement
			End Set
			Get
				Return pageIncrement
			End Get
		End Property



		''' <summary>
		''' Sets the values of four properties for this scroll bar:
		''' <code>value</code>, <code>visibleAmount</code>,
		''' <code>minimum</code>, and <code>maximum</code>.
		''' If the values supplied for these properties are inconsistent
		''' or incorrect, they will be changed to ensure consistency.
		''' <p>
		''' This method simultaneously and synchronously sets the values
		''' of four scroll bar properties, assuring that the values of
		''' these properties are mutually consistent. It enforces the
		''' following constraints:
		''' <code>maximum</code> must be greater than <code>minimum</code>,
		''' <code>maximum - minimum</code> must not be greater
		'''     than <code> [Integer].MAX_VALUE</code>,
		''' <code>visibleAmount</code> must be greater than zero.
		''' <code>visibleAmount</code> must not be greater than
		'''     <code>maximum - minimum</code>,
		''' <code>value</code> must not be less than <code>minimum</code>,
		''' and <code>value</code> must not be greater than
		'''     <code>maximum - visibleAmount</code>
		''' <p>
		''' Calling this method does not fire an
		''' <code>AdjustmentEvent</code>.
		''' </summary>
		''' <param name="value"> is the position in the current window </param>
		''' <param name="visible"> is the visible amount of the scroll bar </param>
		''' <param name="minimum"> is the minimum value of the scroll bar </param>
		''' <param name="maximum"> is the maximum value of the scroll bar </param>
		''' <seealso cref=        #setMinimum </seealso>
		''' <seealso cref=        #setMaximum </seealso>
		''' <seealso cref=        #setVisibleAmount </seealso>
		''' <seealso cref=        #setValue </seealso>
		Public Overridable Sub setValues(ByVal value As Integer, ByVal visible As Integer, ByVal minimum As Integer, ByVal maximum As Integer)
			Dim oldValue As Integer
			SyncLock Me
				If minimum = Integer.MaxValue Then minimum = Integer.MaxValue - 1
				If maximum <= minimum Then maximum = minimum + 1

				Dim maxMinusMin As Long = CLng(maximum) - CLng(minimum)
				If maxMinusMin > Integer.MaxValue Then
					maxMinusMin = Integer.MaxValue
					maximum = minimum + CInt(maxMinusMin)
				End If
				If visible > (Integer) maxMinusMin Then visible = CInt(maxMinusMin)
				If visible < 1 Then visible = 1

				If value < minimum Then value = minimum
				If value > maximum - visible Then value = maximum - visible

				oldValue = Me.value
				Me.value = value
				Me.visibleAmount = visible
				Me.minimum = minimum
				Me.maximum = maximum
				Dim peer_Renamed As java.awt.peer.ScrollbarPeer = CType(Me.peer, java.awt.peer.ScrollbarPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.valuesues(value, visibleAmount, minimum, maximum)
			End SyncLock

			If (oldValue <> value) AndAlso (accessibleContext IsNot Nothing) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(oldValue), Convert.ToInt32(value))
		End Sub

		''' <summary>
		''' Returns true if the value is in the process of changing as a
		''' result of actions being taken by the user.
		''' </summary>
		''' <returns> the value of the <code>valueIsAdjusting</code> property </returns>
		''' <seealso cref= #setValueIsAdjusting
		''' @since 1.4 </seealso>
		Public Overridable Property valueIsAdjusting As Boolean
			Get
				Return isAdjusting
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean
    
				SyncLock Me
					oldValue = isAdjusting
					isAdjusting = b
				End SyncLock
    
				If (oldValue <> b) AndAlso (accessibleContext IsNot Nothing) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, (If(oldValue, AccessibleState.BUSY, Nothing)), (If(b, AccessibleState.BUSY, Nothing)))
			End Set
		End Property




		''' <summary>
		''' Adds the specified adjustment listener to receive instances of
		''' <code>AdjustmentEvent</code> from this scroll bar.
		''' If l is <code>null</code>, no exception is thrown and no
		''' action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the adjustment listener </param>
		''' <seealso cref=          #removeAdjustmentListener </seealso>
		''' <seealso cref=          #getAdjustmentListeners </seealso>
		''' <seealso cref=          java.awt.event.AdjustmentEvent </seealso>
		''' <seealso cref=          java.awt.event.AdjustmentListener
		''' @since        JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addAdjustmentListener(ByVal l As AdjustmentListener) Implements Adjustable.addAdjustmentListener
			If l Is Nothing Then Return
			adjustmentListener = AWTEventMulticaster.add(adjustmentListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified adjustment listener so that it no longer
		''' receives instances of <code>AdjustmentEvent</code> from this scroll bar.
		''' If l is <code>null</code>, no exception is thrown and no action
		''' is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">    the adjustment listener </param>
		''' <seealso cref=             #addAdjustmentListener </seealso>
		''' <seealso cref=             #getAdjustmentListeners </seealso>
		''' <seealso cref=             java.awt.event.AdjustmentEvent </seealso>
		''' <seealso cref=             java.awt.event.AdjustmentListener
		''' @since           JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeAdjustmentListener(ByVal l As AdjustmentListener) Implements Adjustable.removeAdjustmentListener
			If l Is Nothing Then Return
			adjustmentListener = AWTEventMulticaster.remove(adjustmentListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the adjustment listeners
		''' registered on this scrollbar.
		''' </summary>
		''' <returns> all of this scrollbar's <code>AdjustmentListener</code>s
		'''         or an empty array if no adjustment
		'''         listeners are currently registered </returns>
		''' <seealso cref=             #addAdjustmentListener </seealso>
		''' <seealso cref=             #removeAdjustmentListener </seealso>
		''' <seealso cref=             java.awt.event.AdjustmentEvent </seealso>
		''' <seealso cref=             java.awt.event.AdjustmentListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property adjustmentListeners As AdjustmentListener()
			Get
				Return getListeners(GetType(AdjustmentListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>Scrollbar</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal,  such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>Scrollbar</code> <code>c</code>
		''' for its mouse listeners with the following code:
		''' 
		''' <pre>MouseListener[] mls = (MouseListener[])(c.getListeners(MouseListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this component,
		'''          or an empty array if no such listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' 
		''' @since 1.3 </exception>
		Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As [Class]) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(AdjustmentListener) Then
				l = adjustmentListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			If e.id = AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED Then
				If (eventMask And AWTEvent.ADJUSTMENT_EVENT_MASK) <> 0 OrElse adjustmentListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this scroll bar. If the event is an
		''' instance of <code>AdjustmentEvent</code>, it invokes the
		''' <code>processAdjustmentEvent</code> method.
		''' Otherwise, it invokes its superclass's
		''' <code>processEvent</code> method.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=          java.awt.event.AdjustmentEvent </seealso>
		''' <seealso cref=          java.awt.Scrollbar#processAdjustmentEvent
		''' @since        JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is AdjustmentEvent Then
				processAdjustmentEvent(CType(e, AdjustmentEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes adjustment events occurring on this
		''' scrollbar by dispatching them to any registered
		''' <code>AdjustmentListener</code> objects.
		''' <p>
		''' This method is not called unless adjustment events are
		''' enabled for this component. Adjustment events are enabled
		''' when one of the following occurs:
		''' <ul>
		''' <li>An <code>AdjustmentListener</code> object is registered
		''' via <code>addAdjustmentListener</code>.
		''' <li>Adjustment events are enabled via <code>enableEvents</code>.
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the adjustment event </param>
		''' <seealso cref=         java.awt.event.AdjustmentEvent </seealso>
		''' <seealso cref=         java.awt.event.AdjustmentListener </seealso>
		''' <seealso cref=         java.awt.Scrollbar#addAdjustmentListener </seealso>
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processAdjustmentEvent(ByVal e As AdjustmentEvent)
			Dim listener As AdjustmentListener = adjustmentListener
			If listener IsNot Nothing Then listener.adjustmentValueChanged(e)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>Scrollbar</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>      the parameter string of this scroll bar </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString() & ",val=" & value & ",vis=" & visibleAmount & ",min=" & minimum & ",max=" & maximum + (If(orientation = VERTICAL, ",vert", ",horz")) & ",isAdjusting=" & isAdjusting
		End Function


	'     Serialization support.
	'     

		''' <summary>
		''' The scroll bar's serialized Data Version.
		''' 
		''' @serial
		''' </summary>
		Private scrollbarSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>AdjustmentListeners</code>
		''' as optional data. The non-serializable listeners are
		''' detected and no attempt is made to serialize them.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData <code>null</code> terminated sequence of 0
		'''   or more pairs; the pair consists of a <code>String</code>
		'''   and an <code>Object</code>; the <code>String</code> indicates
		'''   the type of object and is one of the following:
		'''   <code>adjustmentListenerK</code> indicating an
		'''     <code>AdjustmentListener</code> object
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#adjustmentListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, adjustmentListenerK, adjustmentListener)
		  s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code> and if
		''' it isn't <code>null</code> adds a listener to
		''' receive adjustment events fired by the
		''' <code>Scrollbar</code>.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  GraphicsEnvironment.checkHeadless()
		  s.defaultReadObject()

		  Dim keyOrNull As Object
		  keyOrNull = s.readObject()
		  Do While Nothing IsNot keyOrNull
			Dim key As String = CStr(keyOrNull).intern()

			If adjustmentListenerK = key Then
			  addAdjustmentListener(CType(s.readObject(), AdjustmentListener))

			Else ' skip value for unrecognized key
			  s.readObject()
			End If
			  keyOrNull = s.readObject()
		  Loop
		End Sub


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>Scrollbar</code>. For scrollbars, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleAWTScrollBar</code>. A new
		''' <code>AccessibleAWTScrollBar</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleAWTScrollBar</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>ScrollBar</code>
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTScrollBar(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Scrollbar</code> class.  It provides an implementation of
		''' the Java Accessibility API appropriate to scrollbar
		''' user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTScrollBar
			Inherits AccessibleAWTComponent
			Implements AccessibleValue

			Private ReadOnly outerInstance As Scrollbar

			Public Sub New(ByVal outerInstance As Scrollbar)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -344337268523697807L

			''' <summary>
			''' Get the state set of this object.
			''' </summary>
			''' <returns> an instance of <code>AccessibleState</code>
			'''     containing the current state of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.valueIsAdjusting Then states.add(AccessibleState.BUSY)
					If outerInstance.orientation = VERTICAL Then
						states.add(AccessibleState.VERTICAL)
					Else
						states.add(AccessibleState.HORIZONTAL)
					End If
					Return states
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of <code>AccessibleRole</code>
			'''     describing the role of the object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SCROLL_BAR
				End Get
			End Property

			''' <summary>
			''' Get the <code>AccessibleValue</code> associated with this
			''' object.  In the implementation of the Java Accessibility
			''' API for this [Class], return this object, which is
			''' responsible for implementing the
			''' <code>AccessibleValue</code> interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleValue As AccessibleValue
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Get the accessible value of this object.
			''' </summary>
			''' <returns> The current value of this object. </returns>
			Public Overridable Property currentAccessibleValue As Number
				Get
					Return Convert.ToInt32(outerInstance.value)
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <returns> True if the value was set. </returns>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean
				If TypeOf n Is Integer? Then
					outerInstance.value = n
					Return True
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Get the minimum accessible value of this object.
			''' </summary>
			''' <returns> The minimum value of this object. </returns>
			Public Overridable Property minimumAccessibleValue As Number
				Get
					Return Convert.ToInt32(outerInstance.minimum)
				End Get
			End Property

			''' <summary>
			''' Get the maximum accessible value of this object.
			''' </summary>
			''' <returns> The maximum value of this object. </returns>
			Public Overridable Property maximumAccessibleValue As Number
				Get
					Return Convert.ToInt32(outerInstance.maximum)
				End Get
			End Property

		End Class ' AccessibleAWTScrollBar

	End Class

End Namespace
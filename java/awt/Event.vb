Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices
Imports [event]

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
    ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
    ''' available only for backwards compatibility.  It has been replaced
    ''' by the <code>AWTEvent</code> class and its subclasses.
    ''' <p>
    ''' <code>Event</code> is a platform-independent class that
    ''' encapsulates events from the platform's Graphical User
    ''' Interface in the Java&nbsp;1.0 event model. In Java&nbsp;1.1
    ''' and later versions, the <code>Event</code> class is maintained
    ''' only for backwards compatibility. The information in this
    ''' class description is provided to assist programmers in
    ''' converting Java&nbsp;1.0 programs to the new event model.
    ''' <p>
    ''' In the Java&nbsp;1.0 event model, an event contains an
    ''' <seealso cref="Event#id"/> field
    ''' that indicates what type of event it is and which other
    ''' <code>Event</code> variables are relevant for the event.
    ''' <p>
    ''' For keyboard events, <seealso cref="Event#key"/>
    ''' contains a value indicating which key was activated, and
    ''' <seealso cref="Event#modifiers"/> contains the
    ''' modifiers for that event.  For the KEY_PRESS and KEY_RELEASE
    ''' event ids, the value of <code>key</code> is the unicode
    ''' character code for the key. For KEY_ACTION and
    ''' KEY_ACTION_RELEASE, the value of <code>key</code> is
    ''' one of the defined action-key identifiers in the
    ''' <code>Event</code> class (<code>PGUP</code>,
    ''' <code>PGDN</code>, <code>F1</code>, <code>F2</code>, etc).
    '''
    ''' @author     Sami Shaio
    ''' @since      JDK1.0
    ''' </summary>
    <Serializable>
    Public Class [Event]
        <NonSerialized>
        Private data As Long

        ' Modifier constants

        ''' <summary>
        ''' This flag indicates that the Shift key was down when the event
        ''' occurred.
        ''' </summary>
        Public Shared ReadOnly SHIFT_MASK As Integer = 1 << 0

        ''' <summary>
        ''' This flag indicates that the Control key was down when the event
        ''' occurred.
        ''' </summary>
        Public Shared ReadOnly CTRL_MASK As Integer = 1 << 1

        ''' <summary>
        ''' This flag indicates that the Meta key was down when the event
        ''' occurred. For mouse events, this flag indicates that the right
        ''' button was pressed or released.
        ''' </summary>
        Public Shared ReadOnly META_MASK As Integer = 1 << 2

        ''' <summary>
        ''' This flag indicates that the Alt key was down when
        ''' the event occurred. For mouse events, this flag indicates that the
        ''' middle mouse button was pressed or released.
        ''' </summary>
        Public Shared ReadOnly ALT_MASK As Integer = 1 << 3

        ' Action keys

        ''' <summary>
        ''' The Home key, a non-ASCII action key.
        ''' </summary>
        Public Const HOME As Integer = 1000

        ''' <summary>
        ''' The End key, a non-ASCII action key.
        ''' </summary>
        Public Const [END] As Integer = 1001

        ''' <summary>
        ''' The Page Up key, a non-ASCII action key.
        ''' </summary>
        Public Const PGUP As Integer = 1002

        ''' <summary>
        ''' The Page Down key, a non-ASCII action key.
        ''' </summary>
        Public Const PGDN As Integer = 1003

        ''' <summary>
        ''' The Up Arrow key, a non-ASCII action key.
        ''' </summary>
        Public Const UP As Integer = 1004

        ''' <summary>
        ''' The Down Arrow key, a non-ASCII action key.
        ''' </summary>
        Public Const DOWN As Integer = 1005

        ''' <summary>
        ''' The Left Arrow key, a non-ASCII action key.
        ''' </summary>
        Public Const LEFT As Integer = 1006

        ''' <summary>
        ''' The Right Arrow key, a non-ASCII action key.
        ''' </summary>
        Public Const RIGHT As Integer = 1007

        ''' <summary>
        ''' The F1 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F1 As Integer = 1008

        ''' <summary>
        ''' The F2 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F2 As Integer = 1009

        ''' <summary>
        ''' The F3 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F3 As Integer = 1010

        ''' <summary>
        ''' The F4 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F4 As Integer = 1011

        ''' <summary>
        ''' The F5 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F5 As Integer = 1012

        ''' <summary>
        ''' The F6 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F6 As Integer = 1013

        ''' <summary>
        ''' The F7 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F7 As Integer = 1014

        ''' <summary>
        ''' The F8 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F8 As Integer = 1015

        ''' <summary>
        ''' The F9 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F9 As Integer = 1016

        ''' <summary>
        ''' The F10 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F10 As Integer = 1017

        ''' <summary>
        ''' The F11 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F11 As Integer = 1018

        ''' <summary>
        ''' The F12 function key, a non-ASCII action key.
        ''' </summary>
        Public Const F12 As Integer = 1019

        ''' <summary>
        ''' The Print Screen key, a non-ASCII action key.
        ''' </summary>
        Public Const PRINT_SCREEN As Integer = 1020

        ''' <summary>
        ''' The Scroll Lock key, a non-ASCII action key.
        ''' </summary>
        Public Const SCROLL_LOCK As Integer = 1021

        ''' <summary>
        ''' The Caps Lock key, a non-ASCII action key.
        ''' </summary>
        Public Const CAPS_LOCK As Integer = 1022

        ''' <summary>
        ''' The Num Lock key, a non-ASCII action key.
        ''' </summary>
        Public Const NUM_LOCK As Integer = 1023

        ''' <summary>
        ''' The Pause key, a non-ASCII action key.
        ''' </summary>
        Public Const PAUSE As Integer = 1024

        ''' <summary>
        ''' The Insert key, a non-ASCII action key.
        ''' </summary>
        Public Const INSERT As Integer = 1025

        ' Non-action keys

        ''' <summary>
        ''' The Enter key.
        ''' </summary>
        Public Shared ReadOnly ENTER As Integer = ControlChars.Lf

        ''' <summary>
        ''' The BackSpace key.
        ''' </summary>
        Public Shared ReadOnly BACK_SPACE As Integer = ControlChars.Back

        ''' <summary>
        ''' The Tab key.
        ''' </summary>
        Public Shared ReadOnly TAB As Integer = ControlChars.Tab

        ''' <summary>
        ''' The Escape key.
        ''' </summary>
        Public Const ESCAPE As Integer = 27

        ''' <summary>
        ''' The Delete key.
        ''' </summary>
        Public Const DELETE As Integer = 127


        ' Base for all window events.
        Private Const WINDOW_EVENT As Integer = 200

        ''' <summary>
        ''' The user has asked the window manager to kill the window.
        ''' </summary>
        Public Shared ReadOnly WINDOW_DESTROY As Integer = 1 + WINDOW_EVENT

        ''' <summary>
        ''' The user has asked the window manager to expose the window.
        ''' </summary>
        Public Shared ReadOnly WINDOW_EXPOSE As Integer = 2 + WINDOW_EVENT

        ''' <summary>
        ''' The user has asked the window manager to iconify the window.
        ''' </summary>
        Public Shared ReadOnly WINDOW_ICONIFY As Integer = 3 + WINDOW_EVENT

        ''' <summary>
        ''' The user has asked the window manager to de-iconify the window.
        ''' </summary>
        Public Shared ReadOnly WINDOW_DEICONIFY As Integer = 4 + WINDOW_EVENT

        ''' <summary>
        ''' The user has asked the window manager to move the window.
        ''' </summary>
        Public Shared ReadOnly WINDOW_MOVED As Integer = 5 + WINDOW_EVENT

        ' Base for all keyboard events.
        Private Const KEY_EVENT As Integer = 400

        ''' <summary>
        ''' The user has pressed a normal key.
        ''' </summary>
        Public Shared ReadOnly KEY_PRESS As Integer = 1 + KEY_EVENT

        ''' <summary>
        ''' The user has released a normal key.
        ''' </summary>
        Public Shared ReadOnly KEY_RELEASE As Integer = 2 + KEY_EVENT

        ''' <summary>
        ''' The user has pressed a non-ASCII <em>action</em> key.
        ''' The <code>key</code> field contains a value that indicates
        ''' that the event occurred on one of the action keys, which
        ''' comprise the 12 function keys, the arrow (cursor) keys,
        ''' Page Up, Page Down, Home, End, Print Screen, Scroll Lock,
        ''' Caps Lock, Num Lock, Pause, and Insert.
        ''' </summary>
        Public Shared ReadOnly KEY_ACTION As Integer = 3 + KEY_EVENT

        ''' <summary>
        ''' The user has released a non-ASCII <em>action</em> key.
        ''' The <code>key</code> field contains a value that indicates
        ''' that the event occurred on one of the action keys, which
        ''' comprise the 12 function keys, the arrow (cursor) keys,
        ''' Page Up, Page Down, Home, End, Print Screen, Scroll Lock,
        ''' Caps Lock, Num Lock, Pause, and Insert.
        ''' </summary>
        Public Shared ReadOnly KEY_ACTION_RELEASE As Integer = 4 + KEY_EVENT

        ' Base for all mouse events.
        Private Const MOUSE_EVENT As Integer = 500

        ''' <summary>
        ''' The user has pressed the mouse button. The <code>ALT_MASK</code>
        ''' flag indicates that the middle button has been pressed.
        ''' The <code>META_MASK</code>flag indicates that the
        ''' right button has been pressed. </summary>
        ''' <seealso cref=     java.awt.Event#ALT_MASK </seealso>
        ''' <seealso cref=     java.awt.Event#META_MASK </seealso>
        Public Shared ReadOnly MOUSE_DOWN As Integer = 1 + MOUSE_EVENT

        ''' <summary>
        ''' The user has released the mouse button. The <code>ALT_MASK</code>
        ''' flag indicates that the middle button has been released.
        ''' The <code>META_MASK</code>flag indicates that the
        ''' right button has been released. </summary>
        ''' <seealso cref=     java.awt.Event#ALT_MASK </seealso>
        ''' <seealso cref=     java.awt.Event#META_MASK </seealso>
        Public Shared ReadOnly MOUSE_UP As Integer = 2 + MOUSE_EVENT

        ''' <summary>
        ''' The mouse has moved with no button pressed.
        ''' </summary>
        Public Shared ReadOnly MOUSE_MOVE As Integer = 3 + MOUSE_EVENT

        ''' <summary>
        ''' The mouse has entered a component.
        ''' </summary>
        Public Shared ReadOnly MOUSE_ENTER As Integer = 4 + MOUSE_EVENT

        ''' <summary>
        ''' The mouse has exited a component.
        ''' </summary>
        Public Shared ReadOnly MOUSE_EXIT As Integer = 5 + MOUSE_EVENT

        ''' <summary>
        ''' The user has moved the mouse with a button pressed. The
        ''' <code>ALT_MASK</code> flag indicates that the middle
        ''' button is being pressed. The <code>META_MASK</code> flag indicates
        ''' that the right button is being pressed. </summary>
        ''' <seealso cref=     java.awt.Event#ALT_MASK </seealso>
        ''' <seealso cref=     java.awt.Event#META_MASK </seealso>
        Public Shared ReadOnly MOUSE_DRAG As Integer = 6 + MOUSE_EVENT


        ' Scrolling events
        Private Const SCROLL_EVENT As Integer = 600

        ''' <summary>
        ''' The user has activated the <em>line up</em>
        ''' area of a scroll bar.
        ''' </summary>
        Public Shared ReadOnly SCROLL_LINE_UP As Integer = 1 + SCROLL_EVENT

        ''' <summary>
        ''' The user has activated the <em>line down</em>
        ''' area of a scroll bar.
        ''' </summary>
        Public Shared ReadOnly SCROLL_LINE_DOWN As Integer = 2 + SCROLL_EVENT

        ''' <summary>
        ''' The user has activated the <em>page up</em>
        ''' area of a scroll bar.
        ''' </summary>
        Public Shared ReadOnly SCROLL_PAGE_UP As Integer = 3 + SCROLL_EVENT

        ''' <summary>
        ''' The user has activated the <em>page down</em>
        ''' area of a scroll bar.
        ''' </summary>
        Public Shared ReadOnly SCROLL_PAGE_DOWN As Integer = 4 + SCROLL_EVENT

        ''' <summary>
        ''' The user has moved the bubble (thumb) in a scroll bar,
        ''' moving to an "absolute" position, rather than to
        ''' an offset from the last position.
        ''' </summary>
        Public Shared ReadOnly SCROLL_ABSOLUTE As Integer = 5 + SCROLL_EVENT

        ''' <summary>
        ''' The scroll begin event.
        ''' </summary>
        Public Shared ReadOnly SCROLL_BEGIN As Integer = 6 + SCROLL_EVENT

        ''' <summary>
        ''' The scroll end event.
        ''' </summary>
        Public Shared ReadOnly SCROLL_END As Integer = 7 + SCROLL_EVENT

        ' List Events
        Private Const LIST_EVENT As Integer = 700

        ''' <summary>
        ''' An item in a list has been selected.
        ''' </summary>
        Public Shared ReadOnly LIST_SELECT As Integer = 1 + LIST_EVENT

        ''' <summary>
        ''' An item in a list has been deselected.
        ''' </summary>
        Public Shared ReadOnly LIST_DESELECT As Integer = 2 + LIST_EVENT

        ' Misc Event
        Private Const MISC_EVENT As Integer = 1000

        ''' <summary>
        ''' This event indicates that the user wants some action to occur.
        ''' </summary>
        Public Shared ReadOnly ACTION_EVENT As Integer = 1 + MISC_EVENT

        ''' <summary>
        ''' A file loading event.
        ''' </summary>
        Public Shared ReadOnly LOAD_FILE As Integer = 2 + MISC_EVENT

        ''' <summary>
        ''' A file saving event.
        ''' </summary>
        Public Shared ReadOnly SAVE_FILE As Integer = 3 + MISC_EVENT

        ''' <summary>
        ''' A component gained the focus.
        ''' </summary>
        Public Shared ReadOnly GOT_FOCUS As Integer = 4 + MISC_EVENT

        ''' <summary>
        ''' A component lost the focus.
        ''' </summary>
        Public Shared ReadOnly LOST_FOCUS As Integer = 5 + MISC_EVENT

        ''' <summary>
        ''' The target component. This indicates the component over which the
        ''' event occurred or with which the event is associated.
        ''' This object has been replaced by AWTEvent.getSource()
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.AWTEvent#getSource() </seealso>
        Public target As Object

        ''' <summary>
        ''' The time stamp.
        ''' Replaced by InputEvent.getWhen().
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.event.InputEvent#getWhen() </seealso>
        Public [when] As Long

        ''' <summary>
        ''' Indicates which type of event the event is, and which
        ''' other <code>Event</code> variables are relevant for the event.
        ''' This has been replaced by AWTEvent.getID()
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.AWTEvent#getID() </seealso>
        Public id As Integer

        ''' <summary>
        ''' The <i>x</i> coordinate of the event.
        ''' Replaced by MouseEvent.getX()
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.event.MouseEvent#getX() </seealso>
        Public x As Integer

        ''' <summary>
        ''' The <i>y</i> coordinate of the event.
        ''' Replaced by MouseEvent.getY()
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.event.MouseEvent#getY() </seealso>
        Public y As Integer

        ''' <summary>
        ''' The key code of the key that was pressed in a keyboard event.
        ''' This has been replaced by KeyEvent.getKeyCode()
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.event.KeyEvent#getKeyCode() </seealso>
        Public key As Integer

        ''' <summary>
        ''' The key character that was pressed in a keyboard event.
        ''' </summary>
        '    public char keyChar;

        ''' <summary>
        ''' The state of the modifier keys.
        ''' This is replaced with InputEvent.getModifiers()
        ''' In java 1.1 MouseEvent and KeyEvent are subclasses
        ''' of InputEvent.
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.event.InputEvent#getModifiers() </seealso>
        Public modifiers As Integer

        ''' <summary>
        ''' For <code>MOUSE_DOWN</code> events, this field indicates the
        ''' number of consecutive clicks. For other events, its value is
        ''' <code>0</code>.
        ''' This field has been replaced by MouseEvent.getClickCount().
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.event.MouseEvent#getClickCount() </seealso>
        Public clickCount As Integer

        ''' <summary>
        ''' An arbitrary argument of the event. The value of this field
        ''' depends on the type of event.
        ''' <code>arg</code> has been replaced by event specific property.
        '''
        ''' @serial
        ''' </summary>
        Public arg As Object

        ''' <summary>
        ''' The next event. This field is set when putting events into a
        ''' linked list.
        ''' This has been replaced by EventQueue.
        '''
        ''' @serial </summary>
        ''' <seealso cref= java.awt.EventQueue </seealso>
        Public evt As [event]

        ' table for mapping old Event action keys to KeyEvent virtual keys.
        Private Shared ReadOnly actionKeyCodes As Integer()() '= {New Integer() {KeyEvent.VK_HOME, [event].HOME}, New Integer() {KeyEvent.VK_END, Event.END }, New Integer() {KeyEvent.VK_PAGE_UP, Event.PGUP }, New Integer() {KeyEvent.VK_PAGE_DOWN, Event.PGDN }, New Integer() {KeyEvent.VK_UP, Event.UP }, New Integer() {KeyEvent.VK_DOWN, Event.DOWN }, New Integer() {KeyEvent.VK_LEFT, Event.LEFT }, New Integer() {KeyEvent.VK_RIGHT, Event.RIGHT }, New Integer() {KeyEvent.VK_F1, Event.F1 }, New Integer() {KeyEvent.VK_F2, Event.F2 }, New Integer() {KeyEvent.VK_F3, Event.F3 }, New Integer() {KeyEvent.VK_F4, Event.F4 }, New Integer() {KeyEvent.VK_F5, Event.F5 }, New Integer() {KeyEvent.VK_F6, Event.F6 }, New Integer() {KeyEvent.VK_F7, Event.F7 }, New Integer() {KeyEvent.VK_F8, Event.F8 }, New Integer() {KeyEvent.VK_F9, Event.F9 }, New Integer() {KeyEvent.VK_F10, Event.F10 }, New Integer() {KeyEvent.VK_F11, Event.F11 }, New Integer() {KeyEvent.VK_F12, Event.F12 }, New Integer() {KeyEvent.VK_PRINTSCREEN, Event.PRINT_SCREEN }, New Integer() {KeyEvent.VK_SCROLL_LOCK, Event.SCROLL_LOCK }, New Integer() {KeyEvent.VK_CAPS_LOCK, Event.CAPS_LOCK }, New Integer() {KeyEvent.VK_NUM_LOCK, Event.NUM_LOCK }, New Integer() {KeyEvent.VK_PAUSE, Event.PAUSE }, New Integer() {KeyEvent.VK_INSERT, Event.INSERT }}

        ''' <summary>
        ''' This field controls whether or not the event is sent back
        ''' down to the peer once the target has processed it -
        ''' false means it's sent to the peer, true means it's not.
        '''
        ''' @serial </summary>
        ''' <seealso cref= #isConsumed() </seealso>
        Private consumed As Boolean = False

        '
        '     * JDK 1.1 serialVersionUID
        '
        Private Const serialVersionUID As Long = 5488922509400504703L

        Shared Sub New()
            ' ensure that the necessary native libraries are loaded
            Toolkit.loadLibraries()
            If Not GraphicsEnvironment.headless Then initIDs()
        End Sub

        ''' <summary>
        ''' Initialize JNI field and method IDs for fields that may be
        '''   accessed from C.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub initIDs()
        End Sub

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Creates an instance of <code>Event</code> with the specified target
        ''' component, time stamp, event type, <i>x</i> and <i>y</i>
        ''' coordinates, keyboard key, state of the modifier keys, and
        ''' argument. </summary>
        ''' <param name="target">     the target component. </param>
        ''' <param name="when">       the time stamp. </param>
        ''' <param name="id">         the event type. </param>
        ''' <param name="x">          the <i>x</i> coordinate. </param>
        ''' <param name="y">          the <i>y</i> coordinate. </param>
        ''' <param name="key">        the key pressed in a keyboard event. </param>
        ''' <param name="modifiers">  the state of the modifier keys. </param>
        ''' <param name="arg">        the specified argument. </param>
        Public Sub New(ByVal target As Object, ByVal [when] As Long, ByVal id As Integer, ByVal x As Integer, ByVal y As Integer, ByVal key As Integer, ByVal modifiers As Integer, ByVal arg As Object)
            Me.target = target
            Me.when = [when]
            Me.id = id
            Me.x = x
            Me.y = y
            Me.key = key
            Me.modifiers = modifiers
            Me.arg = arg
            Me.data = 0
            Me.clickCount = 0
            Select Case id
                Case ACTION_EVENT, WINDOW_DESTROY, WINDOW_ICONIFY, WINDOW_DEICONIFY, WINDOW_MOVED, SCROLL_LINE_UP, SCROLL_LINE_DOWN, SCROLL_PAGE_UP, SCROLL_PAGE_DOWN, SCROLL_ABSOLUTE, SCROLL_BEGIN, SCROLL_END, LIST_SELECT, LIST_DESELECT
                    consumed = True ' these types are not passed back to peer
                Case Else
            End Select
        End Sub

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Creates an instance of <code>Event</code>, with the specified target
        ''' component, time stamp, event type, <i>x</i> and <i>y</i>
        ''' coordinates, keyboard key, state of the modifier keys, and an
        ''' argument set to <code>null</code>. </summary>
        ''' <param name="target">     the target component. </param>
        ''' <param name="when">       the time stamp. </param>
        ''' <param name="id">         the event type. </param>
        ''' <param name="x">          the <i>x</i> coordinate. </param>
        ''' <param name="y">          the <i>y</i> coordinate. </param>
        ''' <param name="key">        the key pressed in a keyboard event. </param>
        ''' <param name="modifiers">  the state of the modifier keys. </param>
        Public Sub New(ByVal target As Object, ByVal [when] As Long, ByVal id As Integer, ByVal x As Integer, ByVal y As Integer, ByVal key As Integer, ByVal modifiers As Integer)
            Me.New(target, [when], id, x, y, key, modifiers, Nothing)
        End Sub

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Creates an instance of <code>Event</code> with the specified
        ''' target component, event type, and argument. </summary>
        ''' <param name="target">     the target component. </param>
        ''' <param name="id">         the event type. </param>
        ''' <param name="arg">        the specified argument. </param>
        Public Sub New(ByVal target As Object, ByVal id As Integer, ByVal arg As Object)
            Me.New(target, 0, id, 0, 0, 0, 0, arg)
        End Sub

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Translates this event so that its <i>x</i> and <i>y</i>
        ''' coordinates are increased by <i>dx</i> and <i>dy</i>,
        ''' respectively.
        ''' <p>
        ''' This method translates an event relative to the given component.
        ''' This involves, at a minimum, translating the coordinates into the
        ''' local coordinate system of the given component. It may also involve
        ''' translating a region in the case of an expose event. </summary>
        ''' <param name="dx">     the distance to translate the <i>x</i> coordinate. </param>
        ''' <param name="dy">     the distance to translate the <i>y</i> coordinate. </param>
        Public Overridable Sub translate(ByVal dx As Integer, ByVal dy As Integer)
            Me.x += dx
            Me.y += dy
        End Sub

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Checks if the Shift key is down. </summary>
        ''' <returns>    <code>true</code> if the key is down;
        '''            <code>false</code> otherwise. </returns>
        ''' <seealso cref=       java.awt.Event#modifiers </seealso>
        ''' <seealso cref=       java.awt.Event#controlDown </seealso>
        ''' <seealso cref=       java.awt.Event#metaDown </seealso>
        Public Overridable Function shiftDown() As Boolean
            Return (modifiers And SHIFT_MASK) <> 0
        End Function

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Checks if the Control key is down. </summary>
        ''' <returns>    <code>true</code> if the key is down;
        '''            <code>false</code> otherwise. </returns>
        ''' <seealso cref=       java.awt.Event#modifiers </seealso>
        ''' <seealso cref=       java.awt.Event#shiftDown </seealso>
        ''' <seealso cref=       java.awt.Event#metaDown </seealso>
        Public Overridable Function controlDown() As Boolean
            Return (modifiers And CTRL_MASK) <> 0
        End Function

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Checks if the Meta key is down.
        ''' </summary>
        ''' <returns>    <code>true</code> if the key is down;
        '''            <code>false</code> otherwise. </returns>
        ''' <seealso cref=       java.awt.Event#modifiers </seealso>
        ''' <seealso cref=       java.awt.Event#shiftDown </seealso>
        ''' <seealso cref=       java.awt.Event#controlDown </seealso>
        Public Overridable Function metaDown() As Boolean
            Return (modifiers And META_MASK) <> 0
        End Function

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' </summary>
        Friend Overridable Sub consume()
            Select Case id
                Case KEY_PRESS, KEY_RELEASE, KEY_ACTION, KEY_ACTION_RELEASE
                    consumed = True
                Case Else
                    ' event type cannot be consumed
            End Select
        End Sub

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' </summary>
        Friend Overridable ReadOnly Property consumed As Boolean
            Get
                Return consumed
            End Get
        End Property

        '
        '     * <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        '     * available only for backwards compatibility.  It has been replaced
        '     * by the <code>AWTEvent</code> class and its subclasses.
        '     * <p>
        '     * Returns the integer key-code associated with the key in this event,
        '     * as described in java.awt.Event.
        '
        Friend Shared Function getOldEventKey(ByVal e As KeyEvent) As Integer
            Dim keyCode As Integer = e.keyCode
            For i As Integer = 0 To actionKeyCodes.Length - 1
                If actionKeyCodes(i)(0) = keyCode Then Return actionKeyCodes(i)(1)
            Next i
            Return AscW(e.keyChar)
        End Function

        '
        '     * <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        '     * available only for backwards compatibility.  It has been replaced
        '     * by the <code>AWTEvent</code> class and its subclasses.
        '     * <p>
        '     * Returns a new KeyEvent char which corresponds to the int key
        '     * of this old event.
        '
        Friend Overridable ReadOnly Property keyEventChar As Char
            Get
                For i As Integer = 0 To actionKeyCodes.Length - 1
                    If actionKeyCodes(i)(1) = key Then Return KeyEvent.CHAR_UNDEFINED
                Next i
                Return ChrW(key)
            End Get
        End Property

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Returns a string representing the state of this <code>Event</code>.
        ''' This method is intended to be used only for debugging purposes, and the
        ''' content and format of the returned string may vary between
        ''' implementations. The returned string may be empty but may not be
        ''' <code>null</code>.
        ''' </summary>
        ''' <returns>    the parameter string of this event </returns>
        Protected Friend Overridable Function paramString() As String
            Dim str As String = "id=" & id & ",x=" & x & ",y=" & y
            If key <> 0 Then str &= ",key=" & key
            If shiftDown() Then str &= ",shift"
            If controlDown() Then str &= ",control"
            If metaDown() Then str &= ",meta"
            If target IsNot Nothing Then str &= ",target=" & target
            If arg IsNot Nothing Then str &= ",arg=" & arg
            Return str
        End Function

        ''' <summary>
        ''' <b>NOTE:</b> The <code>Event</code> class is obsolete and is
        ''' available only for backwards compatibility.  It has been replaced
        ''' by the <code>AWTEvent</code> class and its subclasses.
        ''' <p>
        ''' Returns a representation of this event's values as a string. </summary>
        ''' <returns>    a string that represents the event and the values
        '''                 of its member fields. </returns>
        ''' <seealso cref=       java.awt.Event#paramString
        ''' @since     JDK1.1 </seealso>
        Public Overrides Function ToString() As String
            Return Me.GetType().Name & "[" & paramString() & "]"
        End Function
    End Class

End Namespace
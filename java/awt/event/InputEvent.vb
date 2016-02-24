Imports System
Imports System.Runtime.InteropServices
Imports java.lang

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

Namespace java.awt.event



    ''' <summary>
    ''' The root event class for all component-level input events.
    ''' 
    ''' Input events are delivered to listeners before they are
    ''' processed normally by the source where they originated.
    ''' This allows listeners and component subclasses to "consume"
    ''' the event so that the source will not process them in their
    ''' default manner.  For example, consuming mousePressed events
    ''' on a Button component will prevent the Button from being
    ''' activated.
    ''' 
    ''' @author Carl Quinn
    ''' </summary>
    ''' <seealso cref= KeyEvent </seealso>
    ''' <seealso cref= KeyAdapter </seealso>
    ''' <seealso cref= MouseEvent </seealso>
    ''' <seealso cref= MouseAdapter </seealso>
    ''' <seealso cref= MouseMotionAdapter
    ''' 
    ''' @since 1.1 </seealso>
    Public MustInherit Class InputEvent
        Inherits ComponentEvent

        Private Shared ReadOnly logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.event.InputEvent")

        ''' <summary>
        ''' The Shift key modifier constant.
        ''' It is recommended that SHIFT_DOWN_MASK be used instead.
        ''' </summary>
        Public Shared ReadOnly SHIFT_MASK As Integer = java.awt.event.SHIFT_MASK

        ''' <summary>
        ''' The Control key modifier constant.
        ''' It is recommended that CTRL_DOWN_MASK be used instead.
        ''' </summary>
        Public Shared ReadOnly CTRL_MASK As Integer = java.awt.event.CTRL_MASK

        ''' <summary>
        ''' The Meta key modifier constant.
        ''' It is recommended that META_DOWN_MASK be used instead.
        ''' </summary>
        Public Shared ReadOnly META_MASK As Integer = java.awt.event.META_MASK

        ''' <summary>
        ''' The Alt key modifier constant.
        ''' It is recommended that ALT_DOWN_MASK be used instead.
        ''' </summary>
        Public Shared ReadOnly ALT_MASK As Integer = java.awt.event.ALT_MASK

        ''' <summary>
        ''' The AltGraph key modifier constant.
        ''' </summary>
        Public Shared ReadOnly ALT_GRAPH_MASK As Integer = 1 << 5

        ''' <summary>
        ''' The Mouse Button1 modifier constant.
        ''' It is recommended that BUTTON1_DOWN_MASK be used instead.
        ''' </summary>
        Public Shared ReadOnly BUTTON1_MASK As Integer = 1 << 4

        ''' <summary>
        ''' The Mouse Button2 modifier constant.
        ''' It is recommended that BUTTON2_DOWN_MASK be used instead.
        ''' Note that BUTTON2_MASK has the same value as ALT_MASK.
        ''' </summary>
        Public Shared ReadOnly BUTTON2_MASK As Integer = java.awt.event.ALT_MASK

        ''' <summary>
        ''' The Mouse Button3 modifier constant.
        ''' It is recommended that BUTTON3_DOWN_MASK be used instead.
        ''' Note that BUTTON3_MASK has the same value as META_MASK.
        ''' </summary>
        Public Shared ReadOnly BUTTON3_MASK As Integer = java.awt.event.META_MASK

        ''' <summary>
        ''' The Shift key extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly SHIFT_DOWN_MASK As Integer = 1 << 6

        ''' <summary>
        ''' The Control key extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly CTRL_DOWN_MASK As Integer = 1 << 7

        ''' <summary>
        ''' The Meta key extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly META_DOWN_MASK As Integer = 1 << 8

        ''' <summary>
        ''' The Alt key extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly ALT_DOWN_MASK As Integer = 1 << 9

        ''' <summary>
        ''' The Mouse Button1 extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly BUTTON1_DOWN_MASK As Integer = 1 << 10

        ''' <summary>
        ''' The Mouse Button2 extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly BUTTON2_DOWN_MASK As Integer = 1 << 11

        ''' <summary>
        ''' The Mouse Button3 extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly BUTTON3_DOWN_MASK As Integer = 1 << 12

        ''' <summary>
        ''' The AltGraph key extended modifier constant.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly ALT_GRAPH_DOWN_MASK As Integer = 1 << 13

        ''' <summary>
        ''' An array of extended modifiers for additional buttons. </summary>
        ''' <seealso cref= getButtonDownMasks
        ''' There are twenty buttons fit into 4byte space.
        ''' one more bit is reserved for FIRST_HIGH_BIT.
        ''' @since 7.0 </seealso>
        Private Shared ReadOnly BUTTON_DOWN_MASK As Integer() = {BUTTON1_DOWN_MASK, BUTTON2_DOWN_MASK, BUTTON3_DOWN_MASK, 1 << 14, 1 << 15, 1 << 16, 1 << 17, 1 << 18, 1 << 19, 1 << 20, 1 << 21, 1 << 22, 1 << 23, 1 << 24, 1 << 25, 1 << 26, 1 << 27, 1 << 28, 1 << 29, 1 << 30}

        ''' <summary>
        ''' A method to access an array of extended modifiers for additional buttons.
        ''' @since 7.0
        ''' </summary>
        Private Shared ReadOnly Property buttonDownMasks As Integer()
            Get
                Return java.util.Arrays.copyOf(BUTTON_DOWN_MASK, BUTTON_DOWN_MASK.Length)
            End Get
        End Property


        ''' <summary>
        ''' A method to obtain a mask for any existing mouse button.
        ''' The returned mask may be used for different purposes. Following are some of them:
        ''' <ul>
        ''' <li> <seealso cref="java.awt.Robot#mousePress(int) mousePress(buttons)"/> and
        '''      <seealso cref="java.awt.Robot#mouseRelease(int) mouseRelease(buttons)"/>
        ''' <li> as a {@code modifiers} parameter when creating a new <seealso cref="MouseEvent"/> instance
        ''' <li> to check <seealso cref="MouseEvent#getModifiersEx() modifiersEx"/> of existing {@code MouseEvent}
        ''' </ul> </summary>
        ''' <param name="button"> is a number to represent a button starting from 1.
        ''' For example,
        ''' <pre>
        ''' int button = InputEvent.getMaskForButton(1);
        ''' </pre>
        ''' will have the same meaning as
        ''' <pre>
        ''' int button = InputEvent.getMaskForButton(MouseEvent.BUTTON1);
        ''' </pre>
        ''' because <seealso cref="MouseEvent#BUTTON1 MouseEvent.BUTTON1"/> equals to 1.
        ''' If a mouse has three enabled buttons(see <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>)
        ''' then the values from the left column passed into the method will return
        ''' corresponding values from the right column:
        ''' <PRE>
        '''    <b>button </b>   <b>returned mask</b>
        '''    <seealso cref="MouseEvent#BUTTON1 BUTTON1"/>  <seealso cref="MouseEvent#BUTTON1_DOWN_MASK BUTTON1_DOWN_MASK"/>
        '''    <seealso cref="MouseEvent#BUTTON2 BUTTON2"/>  <seealso cref="MouseEvent#BUTTON2_DOWN_MASK BUTTON2_DOWN_MASK"/>
        '''    <seealso cref="MouseEvent#BUTTON3 BUTTON3"/>  <seealso cref="MouseEvent#BUTTON3_DOWN_MASK BUTTON3_DOWN_MASK"/>
        ''' </PRE>
        ''' If a mouse has more than three enabled buttons then more values
        ''' are admissible (4, 5, etc.). There is no assigned constants for these extended buttons.
        ''' The button masks for the extra buttons returned by this method have no assigned names like the
        ''' first three button masks.
        ''' <p>
        ''' This method has the following implementation restriction.
        ''' It returns masks for a limited number of buttons only. The maximum number is
        ''' implementation dependent and may vary.
        ''' This limit is defined by the relevant number
        ''' of buttons that may hypothetically exist on the mouse but it is greater than the
        ''' <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>.
        ''' <p> </param>
        ''' <exception cref="IllegalArgumentException"> if {@code button} is less than zero or greater than the number
        '''         of button masks reserved for buttons
        ''' @since 7.0 </exception>
        ''' <seealso cref= java.awt.MouseInfo#getNumberOfButtons() </seealso>
        ''' <seealso cref= Toolkit#areExtraMouseButtonsEnabled() </seealso>
        ''' <seealso cref= MouseEvent#getModifiers() </seealso>
        ''' <seealso cref= MouseEvent#getModifiersEx() </seealso>
        Public Shared Function getMaskForButton(ByVal button_Renamed As Integer) As Integer
            If button_Renamed <= 0 OrElse button_Renamed > BUTTON_DOWN_MASK.Length Then Throw New IllegalArgumentException("button doesn't exist " & button_Renamed)
            Return BUTTON_DOWN_MASK(button_Renamed - 1)
        End Function

        ' the constant below MUST be updated if any extra modifier
        ' bits are to be added!
        ' in fact, it is undesirable to add modifier bits
        ' to the same field as this may break applications
        ' see bug# 5066958
        Friend Shared ReadOnly FIRST_HIGH_BIT As Integer = 1 << 31

        Friend Shared ReadOnly JDK_1_3_MODIFIERS As Integer = SHIFT_DOWN_MASK - 1
        Friend Shared ReadOnly HIGH_MODIFIERS As Integer = Not (FIRST_HIGH_BIT - 1)

        ''' <summary>
        ''' The input event's Time stamp in UTC format.  The time stamp
        ''' indicates when the input event was created.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getWhen() </seealso>
        Friend [when] As Long

        ''' <summary>
        ''' The state of the modifier mask at the time the input
        ''' event was fired.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getModifiers() </seealso>
        ''' <seealso cref= #getModifiersEx() </seealso>
        ''' <seealso cref= java.awt.event.KeyEvent </seealso>
        ''' <seealso cref= java.awt.event.MouseEvent </seealso>
        Friend modifiers As Integer

        '    
        '     * A flag that indicates that this instance can be used to access
        '     * the system clipboard.
        '     
        <NonSerialized>
        Private canAccessSystemClipboard_Renamed As Boolean

        Shared Sub New()
            ' ensure that the necessary native libraries are loaded 
            NativeLibLoader.loadLibraries()
            If Not java.awt.GraphicsEnvironment.headless Then initIDs()
            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			sun.awt.AWTAccessor.setInputEventAccessor(New sun.awt.AWTAccessor.InputEventAccessor()
            '		{
            '				public int[] getButtonDownMasks()
            '				{
            '					Return InputEvent.getButtonDownMasks();
            '				}
            '			});
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
        ''' Constructs an InputEvent object with the specified source component,
        ''' modifiers, and type.
        ''' <p> This method throws an
        ''' <code>IllegalArgumentException</code> if <code>source</code>
        ''' is <code>null</code>.
        ''' </summary>
        ''' <param name="source"> the object where the event originated </param>
        ''' <param name="id">           the integer that identifies the event type.
        '''                     It is allowed to pass as parameter any value that
        '''                     allowed for some subclass of {@code InputEvent} class.
        '''                     Passing in the value different from those values result
        '''                     in unspecified behavior </param>
        ''' <param name="when">         a long int that gives the time the event occurred.
        '''                     Passing negative or zero value
        '''                     is not recommended </param>
        ''' <param name="modifiers">    a modifier mask describing the modifier keys and mouse
        '''                     buttons (for example, shift, ctrl, alt, and meta) that
        '''                     are down during the event.
        '''                     Only extended modifiers are allowed to be used as a
        '''                     value for this parameter (see the <seealso cref="InputEvent#getModifiersEx"/>
        '''                     class for the description of extended modifiers).
        '''                     Passing negative parameter
        '''                     is not recommended.
        '''                     Zero value means that no modifiers were passed </param>
        ''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
        ''' <seealso cref= #getSource() </seealso>
        ''' <seealso cref= #getID() </seealso>
        ''' <seealso cref= #getWhen() </seealso>
        ''' <seealso cref= #getModifiers() </seealso>
        Friend Sub New(ByVal source As java.awt.Component, ByVal id As Integer, ByVal [when] As Long, ByVal modifiers As Integer)
            MyBase.New(source, id)
            Me.when = [when]
            Me.modifiers = modifiers
            canAccessSystemClipboard_Renamed = canAccessSystemClipboard()
        End Sub

        Private Function canAccessSystemClipboard() As Boolean
            Dim b As Boolean = False

            If Not java.awt.GraphicsEnvironment.headless Then
                Dim sm As SecurityManager = System.securityManager
                If sm IsNot Nothing Then
                    Try
                        sm.checkPermission(sun.security.util.SecurityConstants.AWT.ACCESS_CLIPBOARD_PERMISSION)
                        b = True
                    Catch se As SecurityException
                        If logger.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then logger.fine("InputEvent.canAccessSystemClipboard() got SecurityException ", se)
                    End Try
                Else
                    b = True
                End If
            End If

            Return b
        End Function

        ''' <summary>
        ''' Returns whether or not the Shift modifier is down on this event.
        ''' </summary>
        Public Overridable ReadOnly Property shiftDown As Boolean
            Get
                Return (modifiers And SHIFT_MASK) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns whether or not the Control modifier is down on this event.
        ''' </summary>
        Public Overridable ReadOnly Property controlDown As Boolean
            Get
                Return (modifiers And CTRL_MASK) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns whether or not the Meta modifier is down on this event.
        ''' </summary>
        Public Overridable ReadOnly Property metaDown As Boolean
            Get
                Return (modifiers And META_MASK) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns whether or not the Alt modifier is down on this event.
        ''' </summary>
        Public Overridable ReadOnly Property altDown As Boolean
            Get
                Return (modifiers And ALT_MASK) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns whether or not the AltGraph modifier is down on this event.
        ''' </summary>
        Public Overridable ReadOnly Property altGraphDown As Boolean
            Get
                Return (modifiers And ALT_GRAPH_MASK) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns the difference in milliseconds between the timestamp of when this event occurred and
        ''' midnight, January 1, 1970 UTC.
        ''' </summary>
        Public Overridable Property [when] As Long
            Get
                Return [when]
            End Get
        End Property

        ''' <summary>
        ''' Returns the modifier mask for this event.
        ''' </summary>
        Public Overridable ReadOnly Property modifiers As Integer
            Get
                Return modifiers And (JDK_1_3_MODIFIERS Or HIGH_MODIFIERS)
            End Get
        End Property

        ''' <summary>
        ''' Returns the extended modifier mask for this event.
        ''' <P>
        ''' Extended modifiers are the modifiers that ends with the _DOWN_MASK suffix,
        ''' such as ALT_DOWN_MASK, BUTTON1_DOWN_MASK, and others.
        ''' <P>
        ''' Extended modifiers represent the state of all modal keys,
        ''' such as ALT, CTRL, META, and the mouse buttons just after
        ''' the event occurred.
        ''' <P>
        ''' For example, if the user presses <b>button 1</b> followed by
        ''' <b>button 2</b>, and then releases them in the same order,
        ''' the following sequence of events is generated:
        ''' <PRE>
        '''    <code>MOUSE_PRESSED</code>:  <code>BUTTON1_DOWN_MASK</code>
        '''    <code>MOUSE_PRESSED</code>:  <code>BUTTON1_DOWN_MASK | BUTTON2_DOWN_MASK</code>
        '''    <code>MOUSE_RELEASED</code>: <code>BUTTON2_DOWN_MASK</code>
        '''    <code>MOUSE_CLICKED</code>:  <code>BUTTON2_DOWN_MASK</code>
        '''    <code>MOUSE_RELEASED</code>:
        '''    <code>MOUSE_CLICKED</code>:
        ''' </PRE>
        ''' <P>
        ''' It is not recommended to compare the return value of this method
        ''' using <code>==</code> because new modifiers can be added in the future.
        ''' For example, the appropriate way to check that SHIFT and BUTTON1 are
        ''' down, but CTRL is up is demonstrated by the following code:
        ''' <PRE>
        '''    int onmask = SHIFT_DOWN_MASK | BUTTON1_DOWN_MASK;
        '''    int offmask = CTRL_DOWN_MASK;
        '''    if ((event.getModifiersEx() &amp; (onmask | offmask)) == onmask) {
        '''        ...
        '''    }
        ''' </PRE>
        ''' The above code will work even if new modifiers are added.
        ''' 
        ''' @since 1.4
        ''' </summary>
        Public Overridable ReadOnly Property modifiersEx As Integer
            Get
                Return modifiers And Not JDK_1_3_MODIFIERS
            End Get
        End Property

        ''' <summary>
        ''' Consumes this event so that it will not be processed
        ''' in the default manner by the source which originated it.
        ''' </summary>
        Public Overrides Sub consume()
            _consumed = True
        End Sub

        ''' <summary>
        ''' Returns whether or not this event has been consumed. </summary>
        ''' <seealso cref= #consume </seealso>
        Public Overrides ReadOnly Property consumed As Boolean

        ' state serialization compatibility with JDK 1.1
        Friend Const serialVersionUID As Long = -2482525981698309786L

        ''' <summary>
        ''' Returns a String describing the extended modifier keys and
        ''' mouse buttons, such as "Shift", "Button1", or "Ctrl+Shift".
        ''' These strings can be localized by changing the
        ''' <code>awt.properties</code> file.
        ''' <p>
        ''' Note that passing negative parameter is incorrect,
        ''' and will cause the returning an unspecified string.
        ''' Zero parameter means that no modifiers were passed and will
        ''' cause the returning an empty string.
        ''' </summary>
        ''' <param name="modifiers"> a modifier mask describing the extended
        '''                modifier keys and mouse buttons for the event </param>
        ''' <returns> a text description of the combination of extended
        '''         modifier keys and mouse buttons that were held down
        '''         during the event.
        ''' @since 1.4 </returns>
        Public Shared Function getModifiersExText(ByVal modifiers As Integer) As String
            Dim buf As New StringBuilder
            If (modifiers And InputEvent.META_DOWN_MASK) <> 0 Then
                buf.append(java.awt.Toolkit.getProperty("AWT.meta", "Meta"))
                buf.append("+")
            End If
            If (modifiers And InputEvent.CTRL_DOWN_MASK) <> 0 Then
                buf.append(java.awt.Toolkit.getProperty("AWT.control", "Ctrl"))
                buf.append("+")
            End If
            If (modifiers And InputEvent.ALT_DOWN_MASK) <> 0 Then
                buf.append(java.awt.Toolkit.getProperty("AWT.alt", "Alt"))
                buf.append("+")
            End If
            If (modifiers And InputEvent.SHIFT_DOWN_MASK) <> 0 Then
                buf.append(java.awt.Toolkit.getProperty("AWT.shift", "Shift"))
                buf.append("+")
            End If
            If (modifiers And InputEvent.ALT_GRAPH_DOWN_MASK) <> 0 Then
                buf.append(java.awt.Toolkit.getProperty("AWT.altGraph", "Alt Graph"))
                buf.append("+")
            End If

            Dim buttonNumber As Integer = 1
            For Each mask As Integer In InputEvent.BUTTON_DOWN_MASK
                If (modifiers And mask) <> 0 Then
                    buf.append(java.awt.Toolkit.getProperty("AWT.button" & buttonNumber, "Button" & buttonNumber))
                    buf.append("+")
                End If
                buttonNumber += 1
            Next mask
            If buf.length() > 0 Then buf.length = buf.length() - 1 ' remove trailing '+'
            Return buf.ToString()
        End Function
    End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

'
' * Copyright (c) 1999, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is used to generate native system input events
	''' for the purposes of test automation, self-running demos, and
	''' other applications where control of the mouse and keyboard
	''' is needed. The primary purpose of Robot is to facilitate
	''' automated testing of Java platform implementations.
	''' <p>
	''' Using the class to generate input events differs from posting
	''' events to the AWT event queue or AWT components in that the
	''' events are generated in the platform's native input
	''' queue. For example, <code>Robot.mouseMove</code> will actually move
	''' the mouse cursor instead of just generating mouse move events.
	''' <p>
	''' Note that some platforms require special privileges or extensions
	''' to access low-level input control. If the current platform configuration
	''' does not allow input control, an <code>AWTException</code> will be thrown
	''' when trying to construct Robot objects. For example, X-Window systems
	''' will throw the exception if the XTEST 2.2 standard extension is not supported
	''' (or not enabled) by the X server.
	''' <p>
	''' Applications that use Robot for purposes other than self-testing should
	''' handle these error conditions gracefully.
	''' 
	''' @author      Robi Khan
	''' @since       1.3
	''' </summary>
	Public Class Robot
		Private Const MAX_DELAY As Integer = 60000
		Private peer As java.awt.peer.RobotPeer
		Private isAutoWaitForIdle_Renamed As Boolean = False
		Private autoDelay_Renamed As Integer = 0
		Private Shared LEGAL_BUTTON_MASK As Integer = 0

		Private screenCapCM As java.awt.image.DirectColorModel = Nothing

		''' <summary>
		''' Constructs a Robot object in the coordinate system of the primary screen.
		''' <p>
		''' </summary>
		''' <exception cref="AWTException"> if the platform configuration does not allow
		''' low-level input control.  This exception is always thrown when
		''' GraphicsEnvironment.isHeadless() returns true </exception>
		''' <exception cref="SecurityException"> if <code>createRobot</code> permission is not granted </exception>
		''' <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref=     SecurityManager#checkPermission </seealso>
		''' <seealso cref=     AWTPermission </seealso>
		Public Sub New()
			If GraphicsEnvironment.headless Then Throw New AWTException("headless environment")
			init(GraphicsEnvironment.localGraphicsEnvironment.defaultScreenDevice)
		End Sub

		''' <summary>
		''' Creates a Robot for the given screen device. Coordinates passed
		''' to Robot method calls like mouseMove and createScreenCapture will
		''' be interpreted as being in the same coordinate system as the
		''' specified screen. Note that depending on the platform configuration,
		''' multiple screens may either:
		''' <ul>
		''' <li>share the same coordinate system to form a combined virtual screen</li>
		''' <li>use different coordinate systems to act as independent screens</li>
		''' </ul>
		''' This constructor is meant for the latter case.
		''' <p>
		''' If screen devices are reconfigured such that the coordinate system is
		''' affected, the behavior of existing Robot objects is undefined.
		''' </summary>
		''' <param name="screen">    A screen GraphicsDevice indicating the coordinate
		'''                  system the Robot will operate in. </param>
		''' <exception cref="AWTException"> if the platform configuration does not allow
		''' low-level input control.  This exception is always thrown when
		''' GraphicsEnvironment.isHeadless() returns true. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>screen</code> is not a screen
		'''          GraphicsDevice. </exception>
		''' <exception cref="SecurityException"> if <code>createRobot</code> permission is not granted </exception>
		''' <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref=     GraphicsDevice </seealso>
		''' <seealso cref=     SecurityManager#checkPermission </seealso>
		''' <seealso cref=     AWTPermission </seealso>
		Public Sub New(ByVal screen As GraphicsDevice)
			checkIsScreenDevice(screen)
			init(screen)
		End Sub

		Private Sub init(ByVal screen As GraphicsDevice)
			checkRobotAllowed()
			Dim toolkit_Renamed As Toolkit = Toolkit.defaultToolkit
			If TypeOf toolkit_Renamed Is sun.awt.ComponentFactory Then
				peer = CType(toolkit_Renamed, sun.awt.ComponentFactory).createRobot(Me, screen)
				disposer = New RobotDisposer(peer)
				sun.java2d.Disposer.addRecord(anchor, disposer)
			End If
			initLegalButtonMask()
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub initLegalButtonMask()
			If LEGAL_BUTTON_MASK <> 0 Then Return

			Dim tmpMask As Integer = 0
			If Toolkit.defaultToolkit.areExtraMouseButtonsEnabled() Then
				If TypeOf Toolkit.defaultToolkit Is sun.awt.SunToolkit Then
					Dim buttonsNumber As Integer = CType(Toolkit.defaultToolkit, sun.awt.SunToolkit).numberOfButtons
					For i As Integer = 0 To buttonsNumber - 1
						tmpMask = tmpMask Or java.awt.event.InputEvent.getMaskForButton(i+1)
					Next i
				End If
			End If
			tmpMask = tmpMask Or java.awt.event.InputEvent.BUTTON1_MASK Or java.awt.event.InputEvent.BUTTON2_MASK Or java.awt.event.InputEvent.BUTTON3_MASK Or java.awt.event.InputEvent.BUTTON1_DOWN_MASK Or java.awt.event.InputEvent.BUTTON2_DOWN_MASK Or java.awt.event.InputEvent.BUTTON3_DOWN_MASK
			LEGAL_BUTTON_MASK = tmpMask
		End Sub

		' determine if the security policy allows Robot's to be created 
		Private Sub checkRobotAllowed()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.CREATE_ROBOT_PERMISSION)
		End Sub

		' check if the given device is a screen device 
		Private Sub checkIsScreenDevice(ByVal device As GraphicsDevice)
			If device Is Nothing OrElse device.type <> GraphicsDevice.TYPE_RASTER_SCREEN Then Throw New IllegalArgumentException("not a valid screen device")
		End Sub

		<NonSerialized> _
		Private anchor As New Object

		Friend Class RobotDisposer
			Implements sun.java2d.DisposerRecord

			Private ReadOnly peer As java.awt.peer.RobotPeer
			Public Sub New(ByVal peer As java.awt.peer.RobotPeer)
				Me.peer = peer
			End Sub
			Public Overridable Sub dispose()
				If peer IsNot Nothing Then peer.Dispose()
			End Sub
		End Class

		<NonSerialized> _
		Private disposer As RobotDisposer

		''' <summary>
		''' Moves mouse pointer to given screen coordinates. </summary>
		''' <param name="x">         X position </param>
		''' <param name="y">         Y position </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub mouseMove(ByVal x As Integer, ByVal y As Integer)
			peer.mouseMove(x, y)
			afterEvent()
		End Sub

		''' <summary>
		''' Presses one or more mouse buttons.  The mouse buttons should
		''' be released using the <seealso cref="#mouseRelease(int)"/> method.
		''' </summary>
		''' <param name="buttons"> the Button mask; a combination of one or more
		''' mouse button masks.
		''' <p>
		''' It is allowed to use only a combination of valid values as a {@code buttons} parameter.
		''' A valid combination consists of {@code InputEvent.BUTTON1_DOWN_MASK},
		''' {@code InputEvent.BUTTON2_DOWN_MASK}, {@code InputEvent.BUTTON3_DOWN_MASK}
		''' and values returned by the
		''' <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/> method.
		''' 
		''' The valid combination also depends on a
		''' <seealso cref="Toolkit#areExtraMouseButtonsEnabled() Toolkit.areExtraMouseButtonsEnabled()"/> value as follows:
		''' <ul>
		''' <li> If support for extended mouse buttons is
		''' <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java
		''' then it is allowed to use only the following standard button masks:
		''' {@code InputEvent.BUTTON1_DOWN_MASK}, {@code InputEvent.BUTTON2_DOWN_MASK},
		''' {@code InputEvent.BUTTON3_DOWN_MASK}.
		''' <li> If support for extended mouse buttons is
		''' <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java
		''' then it is allowed to use the standard button masks
		''' and masks for existing extended mouse buttons, if the mouse has more then three buttons.
		''' In that way, it is allowed to use the button masks corresponding to the buttons
		''' in the range from 1 to <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>.
		''' <br>
		''' It is recommended to use the <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/>
		''' method to obtain the mask for any mouse button by its number.
		''' </ul>
		''' <p>
		''' The following standard button masks are also accepted:
		''' <ul>
		''' <li>{@code InputEvent.BUTTON1_MASK}
		''' <li>{@code InputEvent.BUTTON2_MASK}
		''' <li>{@code InputEvent.BUTTON3_MASK}
		''' </ul>
		''' However, it is recommended to use {@code InputEvent.BUTTON1_DOWN_MASK},
		''' {@code InputEvent.BUTTON2_DOWN_MASK},  {@code InputEvent.BUTTON3_DOWN_MASK} instead.
		''' Either extended {@code _DOWN_MASK} or old {@code _MASK} values
		''' should be used, but both those models should not be mixed. </param>
		''' <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		'''         and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		'''         that does not exist on the mouse and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java </exception>
		''' <seealso cref= #mouseRelease(int) </seealso>
		''' <seealso cref= InputEvent#getMaskForButton(int) </seealso>
		''' <seealso cref= Toolkit#areExtraMouseButtonsEnabled() </seealso>
		''' <seealso cref= java.awt.MouseInfo#getNumberOfButtons() </seealso>
		''' <seealso cref= java.awt.event.MouseEvent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub mousePress(ByVal buttons As Integer)
			checkButtonsArgument(buttons)
			peer.mousePress(buttons)
			afterEvent()
		End Sub

		''' <summary>
		''' Releases one or more mouse buttons.
		''' </summary>
		''' <param name="buttons"> the Button mask; a combination of one or more
		''' mouse button masks.
		''' <p>
		''' It is allowed to use only a combination of valid values as a {@code buttons} parameter.
		''' A valid combination consists of {@code InputEvent.BUTTON1_DOWN_MASK},
		''' {@code InputEvent.BUTTON2_DOWN_MASK}, {@code InputEvent.BUTTON3_DOWN_MASK}
		''' and values returned by the
		''' <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/> method.
		''' 
		''' The valid combination also depends on a
		''' <seealso cref="Toolkit#areExtraMouseButtonsEnabled() Toolkit.areExtraMouseButtonsEnabled()"/> value as follows:
		''' <ul>
		''' <li> If the support for extended mouse buttons is
		''' <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java
		''' then it is allowed to use only the following standard button masks:
		''' {@code InputEvent.BUTTON1_DOWN_MASK}, {@code InputEvent.BUTTON2_DOWN_MASK},
		''' {@code InputEvent.BUTTON3_DOWN_MASK}.
		''' <li> If the support for extended mouse buttons is
		''' <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java
		''' then it is allowed to use the standard button masks
		''' and masks for existing extended mouse buttons, if the mouse has more then three buttons.
		''' In that way, it is allowed to use the button masks corresponding to the buttons
		''' in the range from 1 to <seealso cref="java.awt.MouseInfo#getNumberOfButtons() MouseInfo.getNumberOfButtons()"/>.
		''' <br>
		''' It is recommended to use the <seealso cref="InputEvent#getMaskForButton(int) InputEvent.getMaskForButton(button)"/>
		''' method to obtain the mask for any mouse button by its number.
		''' </ul>
		''' <p>
		''' The following standard button masks are also accepted:
		''' <ul>
		''' <li>{@code InputEvent.BUTTON1_MASK}
		''' <li>{@code InputEvent.BUTTON2_MASK}
		''' <li>{@code InputEvent.BUTTON3_MASK}
		''' </ul>
		''' However, it is recommended to use {@code InputEvent.BUTTON1_DOWN_MASK},
		''' {@code InputEvent.BUTTON2_DOWN_MASK},  {@code InputEvent.BUTTON3_DOWN_MASK} instead.
		''' Either extended {@code _DOWN_MASK} or old {@code _MASK} values
		''' should be used, but both those models should not be mixed. </param>
		''' <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		'''         and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() disabled"/> by Java </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code buttons} mask contains the mask for extra mouse button
		'''         that does not exist on the mouse and support for extended mouse buttons is <seealso cref="Toolkit#areExtraMouseButtonsEnabled() enabled"/> by Java </exception>
		''' <seealso cref= #mousePress(int) </seealso>
		''' <seealso cref= InputEvent#getMaskForButton(int) </seealso>
		''' <seealso cref= Toolkit#areExtraMouseButtonsEnabled() </seealso>
		''' <seealso cref= java.awt.MouseInfo#getNumberOfButtons() </seealso>
		''' <seealso cref= java.awt.event.MouseEvent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub mouseRelease(ByVal buttons As Integer)
			checkButtonsArgument(buttons)
			peer.mouseRelease(buttons)
			afterEvent()
		End Sub

		Private Sub checkButtonsArgument(ByVal buttons As Integer)
			If (buttons Or LEGAL_BUTTON_MASK) <> LEGAL_BUTTON_MASK Then Throw New IllegalArgumentException("Invalid combination of button flags")
		End Sub

		''' <summary>
		''' Rotates the scroll wheel on wheel-equipped mice.
		''' </summary>
		''' <param name="wheelAmt">  number of "notches" to move the mouse wheel
		'''                  Negative values indicate movement up/away from the user,
		'''                  positive values indicate movement down/towards the user.
		''' 
		''' @since 1.4 </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub mouseWheel(ByVal wheelAmt As Integer)
			peer.mouseWheel(wheelAmt)
			afterEvent()
		End Sub

		''' <summary>
		''' Presses a given key.  The key should be released using the
		''' <code>keyRelease</code> method.
		''' <p>
		''' Key codes that have more than one physical key associated with them
		''' (e.g. <code>KeyEvent.VK_SHIFT</code> could mean either the
		''' left or right shift key) will map to the left key.
		''' </summary>
		''' <param name="keycode"> Key to press (e.g. <code>KeyEvent.VK_A</code>) </param>
		''' <exception cref="IllegalArgumentException"> if <code>keycode</code> is not
		'''          a valid key </exception>
		''' <seealso cref=     #keyRelease(int) </seealso>
		''' <seealso cref=     java.awt.event.KeyEvent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub keyPress(ByVal keycode As Integer)
			checkKeycodeArgument(keycode)
			peer.keyPress(keycode)
			afterEvent()
		End Sub

		''' <summary>
		''' Releases a given key.
		''' <p>
		''' Key codes that have more than one physical key associated with them
		''' (e.g. <code>KeyEvent.VK_SHIFT</code> could mean either the
		''' left or right shift key) will map to the left key.
		''' </summary>
		''' <param name="keycode"> Key to release (e.g. <code>KeyEvent.VK_A</code>) </param>
		''' <exception cref="IllegalArgumentException"> if <code>keycode</code> is not a
		'''          valid key </exception>
		''' <seealso cref=  #keyPress(int) </seealso>
		''' <seealso cref=     java.awt.event.KeyEvent </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub keyRelease(ByVal keycode As Integer)
			checkKeycodeArgument(keycode)
			peer.keyRelease(keycode)
			afterEvent()
		End Sub

		Private Sub checkKeycodeArgument(ByVal keycode As Integer)
			' rather than build a big table or switch statement here, we'll
			' just check that the key isn't VK_UNDEFINED and assume that the
			' peer implementations will throw an exception for other bogus
			' values e.g. -1, 999999
			If keycode = java.awt.event.KeyEvent.VK_UNDEFINED Then Throw New IllegalArgumentException("Invalid key code")
		End Sub

		''' <summary>
		''' Returns the color of a pixel at the given screen coordinates. </summary>
		''' <param name="x">       X position of pixel </param>
		''' <param name="y">       Y position of pixel </param>
		''' <returns>  Color of the pixel </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getPixelColor(ByVal x As Integer, ByVal y As Integer) As Color
			Dim color_Renamed As New Color(peer.getRGBPixel(x, y))
			Return color_Renamed
		End Function

		''' <summary>
		''' Creates an image containing pixels read from the screen.  This image does
		''' not include the mouse cursor. </summary>
		''' <param name="screenRect">      Rect to capture in screen coordinates </param>
		''' <returns>  The captured image </returns>
		''' <exception cref="IllegalArgumentException"> if <code>screenRect</code> width and height are not greater than zero </exception>
		''' <exception cref="SecurityException"> if <code>readDisplayPixels</code> permission is not granted </exception>
		''' <seealso cref=     SecurityManager#checkPermission </seealso>
		''' <seealso cref=     AWTPermission </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function createScreenCapture(ByVal screenRect As Rectangle) As java.awt.image.BufferedImage
			checkScreenCaptureAllowed()

			checkValidRect(screenRect)

			Dim image_Renamed As java.awt.image.BufferedImage
			Dim buffer As java.awt.image.DataBufferInt
			Dim raster As java.awt.image.WritableRaster

			If screenCapCM Is Nothing Then screenCapCM = New java.awt.image.DirectColorModel(24, &HFF0000, &HFF00, &HFF)

			' need to sync the toolkit prior to grabbing the pixels since in some
			' cases rendering to the screen may be delayed
			Toolkit.defaultToolkit.sync()

			Dim pixels As Integer()
			Dim bandmasks As Integer() = New Integer(2){}

			pixels = peer.getRGBPixels(screenRect)
			buffer = New java.awt.image.DataBufferInt(pixels, pixels.Length)

			bandmasks(0) = screenCapCM.redMask
			bandmasks(1) = screenCapCM.greenMask
			bandmasks(2) = screenCapCM.blueMask

			raster = java.awt.image.Raster.createPackedRaster(buffer, screenRect.width, screenRect.height, screenRect.width, bandmasks, Nothing)
			sun.awt.image.SunWritableRaster.makeTrackable(buffer)

			image_Renamed = New java.awt.image.BufferedImage(screenCapCM, raster, False, Nothing)

			Return image_Renamed
		End Function

		Private Shared Sub checkValidRect(ByVal rect As Rectangle)
			If rect.width <= 0 OrElse rect.height <= 0 Then Throw New IllegalArgumentException("Rectangle width and height must be > 0")
		End Sub

		Private Shared Sub checkScreenCaptureAllowed()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.READ_DISPLAY_PIXELS_PERMISSION)
		End Sub

	'    
	'     * Called after an event is generated
	'     
		Private Sub afterEvent()
			autoWaitForIdle()
			autoDelay()
		End Sub

		''' <summary>
		''' Returns whether this Robot automatically invokes <code>waitForIdle</code>
		''' after generating an event. </summary>
		''' <returns> Whether <code>waitForIdle</code> is automatically called </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property autoWaitForIdle As Boolean
			Get
				Return isAutoWaitForIdle_Renamed
			End Get
			Set(ByVal isOn As Boolean)
				isAutoWaitForIdle_Renamed = isOn
			End Set
		End Property


	'    
	'     * Calls waitForIdle after every event if so desired.
	'     
		Private Sub autoWaitForIdle()
			If isAutoWaitForIdle_Renamed Then waitForIdle()
		End Sub

		''' <summary>
		''' Returns the number of milliseconds this Robot sleeps after generating an event.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property autoDelay As Integer
			Get
				Return autoDelay_Renamed
			End Get
			Set(ByVal ms As Integer)
				checkDelayArgument(ms)
				autoDelay_Renamed = ms
			End Set
		End Property


	'    
	'     * Automatically sleeps for the specified interval after event generated.
	'     
		Private Sub autoDelay()
			delay(autoDelay_Renamed)
		End Sub

		''' <summary>
		''' Sleeps for the specified time.
		''' To catch any <code>InterruptedException</code>s that occur,
		''' <code>Thread.sleep()</code> may be used instead. </summary>
		''' <param name="ms">      time to sleep in milliseconds </param>
		''' <exception cref="IllegalArgumentException"> if <code>ms</code> is not between 0 and 60,000 milliseconds inclusive </exception>
		''' <seealso cref=     java.lang.Thread#sleep </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub delay(ByVal ms As Integer)
			checkDelayArgument(ms)
			Try
				Thread.Sleep(ms)
			Catch ite As InterruptedException
				Console.WriteLine(ite.ToString())
				Console.Write(ite.StackTrace)
			End Try
		End Sub

		Private Sub checkDelayArgument(ByVal ms As Integer)
			If ms < 0 OrElse ms > MAX_DELAY Then Throw New IllegalArgumentException("Delay must be to 0 to 60,000ms")
		End Sub

		''' <summary>
		''' Waits until all events currently on the event queue have been processed. </summary>
		''' <exception cref="IllegalThreadStateException"> if called on the AWT event dispatching thread </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub waitForIdle()
			checkNotDispatchThread()
			' post a dummy event to the queue so we know when
			' all the events before it have been processed
			Try
				sun.awt.SunToolkit.flushPendingEvents()
				EventQueue.invokeAndWait(New RunnableAnonymousInnerClassHelper
			Catch ite As InterruptedException
				Console.Error.WriteLine("Robot.waitForIdle, non-fatal exception caught:")
				Console.WriteLine(ite.ToString())
				Console.Write(ite.StackTrace)
			Catch ine As InvocationTargetException
				Console.Error.WriteLine("Robot.waitForIdle, non-fatal exception caught:")
				Console.WriteLine(ine.ToString())
				Console.Write(ine.StackTrace)
			End Try
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				' dummy implementation
			End Sub
		End Class

		Private Sub checkNotDispatchThread()
			If EventQueue.dispatchThread Then Throw New IllegalThreadStateException("Cannot call method from the event dispatcher thread")
		End Sub

		''' <summary>
		''' Returns a string representation of this Robot.
		''' </summary>
		''' <returns>  the string representation. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			Dim params As String = "autoDelay = " & autoDelay & ", " & "autoWaitForIdle = " & autoWaitForIdle
			Return Me.GetType().name & "[ " & params & " ]"
		End Function
	End Class

End Namespace
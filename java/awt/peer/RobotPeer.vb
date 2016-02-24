'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.peer


	''' <summary>
	''' RobotPeer defines an interface whereby toolkits support automated testing
	''' by allowing native input events to be generated from Java code.
	''' 
	''' This interface should not be directly imported by code outside the
	''' java.awt.* hierarchy; it is not to be considered public and is subject
	''' to change.
	''' 
	''' @author      Robi Khan
	''' </summary>
	Public Interface RobotPeer
		''' <summary>
		''' Moves the mouse pointer to the specified screen location.
		''' </summary>
		''' <param name="x"> the X location on screen </param>
		''' <param name="y"> the Y location on screen
		''' </param>
		''' <seealso cref= Robot#mouseMove(int, int) </seealso>
		Sub mouseMove(ByVal x As Integer, ByVal y As Integer)

		''' <summary>
		''' Simulates a mouse press with the specified button(s).
		''' </summary>
		''' <param name="buttons"> the button mask
		''' </param>
		''' <seealso cref= Robot#mousePress(int) </seealso>
		Sub mousePress(ByVal buttons As Integer)

		''' <summary>
		''' Simulates a mouse release with the specified button(s).
		''' </summary>
		''' <param name="buttons"> the button mask
		''' </param>
		''' <seealso cref= Robot#mouseRelease(int) </seealso>
		Sub mouseRelease(ByVal buttons As Integer)

		''' <summary>
		''' Simulates mouse wheel action.
		''' </summary>
		''' <param name="wheelAmt"> number of notches to move the mouse wheel
		''' </param>
		''' <seealso cref= Robot#mouseWheel(int) </seealso>
		Sub mouseWheel(ByVal wheelAmt As Integer)

		''' <summary>
		''' Simulates a key press of the specified key.
		''' </summary>
		''' <param name="keycode"> the key code to press
		''' </param>
		''' <seealso cref= Robot#keyPress(int) </seealso>
		Sub keyPress(ByVal keycode As Integer)

		''' <summary>
		''' Simulates a key release of the specified key.
		''' </summary>
		''' <param name="keycode"> the key code to release
		''' </param>
		''' <seealso cref= Robot#keyRelease(int) </seealso>
		Sub keyRelease(ByVal keycode As Integer)

		''' <summary>
		''' Gets the RGB value of the specified pixel on screen.
		''' </summary>
		''' <param name="x"> the X screen coordinate </param>
		''' <param name="y"> the Y screen coordinate
		''' </param>
		''' <returns> the RGB value of the specified pixel on screen
		''' </returns>
		''' <seealso cref= Robot#getPixelColor(int, int) </seealso>
		Function getRGBPixel(ByVal x As Integer, ByVal y As Integer) As Integer

		''' <summary>
		''' Gets the RGB values of the specified screen area as an array.
		''' </summary>
		''' <param name="bounds"> the screen area to capture the RGB values from
		''' </param>
		''' <returns> the RGB values of the specified screen area
		''' </returns>
		''' <seealso cref= Robot#createScreenCapture(Rectangle) </seealso>
		Function getRGBPixels(ByVal bounds As Rectangle) As Integer()

		''' <summary>
		''' Disposes the robot peer when it is not needed anymore.
		''' </summary>
		Sub dispose()
	End Interface

End Namespace
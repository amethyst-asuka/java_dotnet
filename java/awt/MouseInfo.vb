Imports Microsoft.VisualBasic
Imports System.Diagnostics

'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>MouseInfo</code>  provides methods for getting information about the mouse,
	''' such as mouse pointer location and the number of mouse buttons.
	''' 
	''' @author     Roman Poborchiy
	''' @since 1.5
	''' </summary>

	Public Class MouseInfo

		''' <summary>
		''' Private constructor to prevent instantiation.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Returns a <code>PointerInfo</code> instance that represents the current
		''' location of the mouse pointer.
		''' The <code>GraphicsDevice</code> stored in this <code>PointerInfo</code>
		''' contains the mouse pointer. The coordinate system used for the mouse position
		''' depends on whether or not the <code>GraphicsDevice</code> is part of a virtual
		''' screen device.
		''' For virtual screen devices, the coordinates are given in the virtual
		''' coordinate system, otherwise they are returned in the coordinate system
		''' of the <code>GraphicsDevice</code>. See <seealso cref="GraphicsConfiguration"/>
		''' for more information about the virtual screen devices.
		''' On systems without a mouse, returns <code>null</code>.
		''' <p>
		''' If there is a security manager, its <code>checkPermission</code> method
		''' is called with an <code>AWTPermission("watchMousePointer")</code>
		''' permission before creating and returning a <code>PointerInfo</code>
		''' object. This may result in a <code>SecurityException</code>.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''            <code>checkPermission</code> method doesn't allow the operation </exception>
		''' <seealso cref=       GraphicsConfiguration </seealso>
		''' <seealso cref=       SecurityManager#checkPermission </seealso>
		''' <seealso cref=       java.awt.AWTPermission </seealso>
		''' <returns>    location of the mouse pointer
		''' @since     1.5 </returns>
		PublicShared ReadOnly PropertypointerInfo As PointerInfo
			Get
				If GraphicsEnvironment.headless Then Throw New HeadlessException
    
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.WATCH_MOUSE_PERMISSION)
    
				Dim point As New Point(0, 0)
				Dim deviceNum As Integer = Toolkit.defaultToolkit.mouseInfoPeer.fillPointWithCoords(point)
				Dim gds As GraphicsDevice() = GraphicsEnvironment.localGraphicsEnvironment.screenDevices
				Dim retval As PointerInfo = Nothing
				If areScreenDevicesIndependent(gds) Then
					retval = New PointerInfo(gds(deviceNum), point)
				Else
					For i As Integer = 0 To gds.Length - 1
						Dim gc As GraphicsConfiguration = gds(i).defaultConfiguration
						Dim bounds As Rectangle = gc.bounds
						If bounds.contains(point) Then retval = New PointerInfo(gds(i), point)
					Next i
				End If
    
				Return retval
			End Get
		End Property

		Private Shared Function areScreenDevicesIndependent(ByVal gds As GraphicsDevice()) As Boolean
			For i As Integer = 0 To gds.Length - 1
				Dim bounds As Rectangle = gds(i).defaultConfiguration.bounds
				If bounds.x <> 0 OrElse bounds.y <> 0 Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Returns the number of buttons on the mouse.
		''' On systems without a mouse, returns <code>-1</code>.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
		''' <returns> number of buttons on the mouse
		''' @since 1.5 </returns>
		PublicShared ReadOnly PropertynumberOfButtons As Integer
			Get
				If GraphicsEnvironment.headless Then Throw New HeadlessException
				Dim prop As Object = Toolkit.defaultToolkit.getDesktopProperty("awt.mouse.numButtons")
				If TypeOf prop Is Integer? Then Return CInt(Fix(prop))
    
				' This should never happen.
				Debug.Assert(False, "awt.mouse.numButtons is not an integer property")
				Return 0
			End Get
		End Property

	End Class

End Namespace
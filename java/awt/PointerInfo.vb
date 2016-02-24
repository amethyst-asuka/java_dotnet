'
' * Copyright (c) 2003, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A class that describes the pointer position.
	''' It provides the {@code GraphicsDevice} where the pointer is and
	''' the {@code Point} that represents the coordinates of the pointer.
	''' <p>
	''' Instances of this class should be obtained via
	''' <seealso cref="MouseInfo#getPointerInfo"/>.
	''' The {@code PointerInfo} instance is not updated dynamically as the mouse
	''' moves. To get the updated location, you must call
	''' <seealso cref="MouseInfo#getPointerInfo"/> again.
	''' </summary>
	''' <seealso cref= MouseInfo#getPointerInfo
	''' @author Roman Poborchiy
	''' @since 1.5 </seealso>
	Public Class PointerInfo

		Private ReadOnly device As GraphicsDevice
		Private ReadOnly location As Point

		''' <summary>
		''' Package-private constructor to prevent instantiation.
		''' </summary>
		Friend Sub New(ByVal device As GraphicsDevice, ByVal location As Point)
			Me.device = device
			Me.location = location
		End Sub

		''' <summary>
		''' Returns the {@code GraphicsDevice} where the mouse pointer was at the
		''' moment this {@code PointerInfo} was created.
		''' </summary>
		''' <returns> {@code GraphicsDevice} corresponding to the pointer
		''' @since 1.5 </returns>
		Public Overridable Property device As GraphicsDevice
			Get
				Return device
			End Get
		End Property

		''' <summary>
		''' Returns the {@code Point} that represents the coordinates of the pointer
		''' on the screen. See <seealso cref="MouseInfo#getPointerInfo"/> for more information
		''' about coordinate calculation for multiscreen systems.
		''' </summary>
		''' <returns> coordinates of mouse pointer </returns>
		''' <seealso cref= MouseInfo </seealso>
		''' <seealso cref= MouseInfo#getPointerInfo
		''' @since 1.5 </seealso>
		Public Overridable Property location As Point
			Get
				Return location
			End Get
		End Property
	End Class

End Namespace
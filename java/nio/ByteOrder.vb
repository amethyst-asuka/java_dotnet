'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio


	''' <summary>
	''' A typesafe enumeration for byte orders.
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public NotInheritable Class ByteOrder

		Private name As String

		Private Sub New(  name As String)
			Me.name = name
		End Sub

		''' <summary>
		''' Constant denoting big-endian byte order.  In this order, the bytes of a
		''' multibyte value are ordered from most significant to least significant.
		''' </summary>
		Public Shared ReadOnly BIG_ENDIAN As New ByteOrder("BIG_ENDIAN")

		''' <summary>
		''' Constant denoting little-endian byte order.  In this order, the bytes of
		''' a multibyte value are ordered from least significant to most
		''' significant.
		''' </summary>
		Public Shared ReadOnly LITTLE_ENDIAN As New ByteOrder("LITTLE_ENDIAN")

		''' <summary>
		''' Retrieves the native byte order of the underlying platform.
		''' 
		''' <p> This method is defined so that performance-sensitive Java code can
		''' allocate direct buffers with the same byte order as the hardware.
		''' Native code libraries are often more efficient when such buffers are
		''' used.  </p>
		''' </summary>
		''' <returns>  The native byte order of the hardware upon which this Java
		'''          virtual machine is running </returns>
		Public Shared Function nativeOrder() As ByteOrder
			Return Bits.byteOrder()
		End Function

		''' <summary>
		''' Constructs a string describing this object.
		''' 
		''' <p> This method returns the string <tt>"BIG_ENDIAN"</tt> for {@link
		''' #BIG_ENDIAN} and <tt>"LITTLE_ENDIAN"</tt> for <seealso cref="#LITTLE_ENDIAN"/>.
		''' </p>
		''' </summary>
		''' <returns>  The specified string </returns>
		Public Overrides Function ToString() As String
			Return name
		End Function

	End Class

End Namespace
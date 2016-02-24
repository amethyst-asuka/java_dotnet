'
' * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip

	''' <summary>
	''' An interface representing a data checksum.
	''' 
	''' @author      David Connelly
	''' </summary>
	Public Interface Checksum
		''' <summary>
		''' Updates the current checksum with the specified byte.
		''' </summary>
		''' <param name="b"> the byte to update the checksum with </param>
		Sub update(ByVal b As Integer)

		''' <summary>
		''' Updates the current checksum with the specified array of bytes. </summary>
		''' <param name="b"> the byte array to update the checksum with </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the number of bytes to use for the update </param>
		Sub update(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Returns the current checksum value. </summary>
		''' <returns> the current checksum value </returns>
		ReadOnly Property value As Long

		''' <summary>
		''' Resets the checksum to its initial value.
		''' </summary>
		Sub reset()
	End Interface

End Namespace
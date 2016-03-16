Imports Microsoft.VisualBasic

'
' * Copyright (c) 2001, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Utility methods for packing/unpacking primitive values in/out of byte arrays
	''' using big-endian byte ordering.
	''' </summary>
	Friend Class Bits

	'    
	'     * Methods for unpacking primitive values from byte arrays starting at
	'     * given offsets.
	'     

		Friend Shared Function getBoolean(ByVal b As SByte(), ByVal [off] As Integer) As Boolean
			Return b([off]) <> 0
		End Function

		Friend Shared Function getChar(ByVal b As SByte(), ByVal [off] As Integer) As Char
            Return Chr((b([off] + 1) And &HFF) + (b([off]) << 8))
        End Function

		Friend Shared Function getShort(ByVal b As SByte(), ByVal [off] As Integer) As Short
			Return CShort(Fix((b([off] + 1) And &HFF) + (b([off]) << 8)))
		End Function

		Friend Shared Function getInt(ByVal b As SByte(), ByVal [off] As Integer) As Integer
			Return ((b([off] + 3) And &HFF)) + ((b([off] + 2) And &HFF) << 8) + ((b([off] + 1) And &HFF) << 16) + ((b([off])) << 24)
		End Function

		Friend Shared Function getFloat(ByVal b As SByte(), ByVal [off] As Integer) As Single
			Return Float.intBitsToFloat(getInt(b, [off]))
		End Function

		Friend Shared Function getLong(ByVal b As SByte(), ByVal [off] As Integer) As Long
			Return ((b([off] + 7) And &HFFL)) + ((b([off] + 6) And &HFFL) << 8) + ((b([off] + 5) And &HFFL) << 16) + ((b([off] + 4) And &HFFL) << 24) + ((b([off] + 3) And &HFFL) << 32) + ((b([off] + 2) And &HFFL) << 40) + ((b([off] + 1) And &HFFL) << 48) + ((CLng(b([off]))) << 56)
		End Function

		Friend Shared Function getDouble(ByVal b As SByte(), ByVal [off] As Integer) As Double
			Return java.lang.[Double].longBitsToDouble(getLong(b, [off]))
		End Function

	'    
	'     * Methods for packing primitive values into byte arrays starting at given
	'     * offsets.
	'     

		Friend Shared Sub putBoolean(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Boolean)
			b([off]) = CByte(If(val, 1, 0))
		End Sub

        Friend Shared Sub putChar(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Char)
            Dim ascii As Integer = AscW(val)

            b([off] + 1) = ascii
            b([off]) = CByte(CInt(CUInt(ascii) >> 8))
        End Sub

        Friend Shared Sub putShort(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Short)
			b([off] + 1) = CByte(val)
			b([off]) = CByte(CShort(CUShort(val) >> 8))
		End Sub

		Friend Shared Sub putInt(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Integer)
			b([off] + 3) = CByte(val)
			b([off] + 2) = CByte(CInt(CUInt(val) >> 8))
			b([off] + 1) = CByte(CInt(CUInt(val) >> 16))
			b([off]) = CByte(CInt(CUInt(val) >> 24))
		End Sub

		Friend Shared Sub putFloat(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Single)
			putInt(b, [off], Float.floatToIntBits(val))
		End Sub

		Friend Shared Sub putLong(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Long)
			b([off] + 7) = CByte(val)
			b([off] + 6) = CByte(CLng(CULng(val) >> 8))
			b([off] + 5) = CByte(CLng(CULng(val) >> 16))
			b([off] + 4) = CByte(CLng(CULng(val) >> 24))
			b([off] + 3) = CByte(CLng(CULng(val) >> 32))
			b([off] + 2) = CByte(CLng(CULng(val) >> 40))
			b([off] + 1) = CByte(CLng(CULng(val) >> 48))
			b([off]) = CByte(CLng(CULng(val) >> 56))
		End Sub

		Friend Shared Sub putDouble(ByVal b As SByte(), ByVal [off] As Integer, ByVal val As Double)
			putLong(b, [off], java.lang.[Double].doubleToLongBits(val))
		End Sub
	End Class

End Namespace
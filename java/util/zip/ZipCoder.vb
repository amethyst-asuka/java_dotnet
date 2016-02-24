Imports Microsoft.VisualBasic

'
' * Copyright (c) 2009, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Utility class for zipfile name and comment decoding and encoding
	''' </summary>

	Friend NotInheritable Class ZipCoder

		Friend Overrides Function ToString(ByVal ba As SByte(), ByVal length As Integer) As String
			Dim cd As java.nio.charset.CharsetDecoder = decoder().reset()
			Dim len As Integer = CInt(Fix(length * cd.maxCharsPerByte()))
			Dim ca As Char() = New Char(len - 1){}
			If len = 0 Then Return New String(ca)
			' UTF-8 only for now. Other ArrayDeocder only handles
			' CodingErrorAction.REPLACE mode. ZipCoder uses
			' REPORT mode.
			If isUTF8_Renamed AndAlso TypeOf cd Is sun.nio.cs.ArrayDecoder Then
				Dim clen As Integer = CType(cd, sun.nio.cs.ArrayDecoder).decode(ba, 0, length, ca)
				If clen = -1 Then ' malformed Throw New IllegalArgumentException("MALFORMED")
				Return New String(ca, 0, clen)
			End If
			Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(ba, 0, length)
			Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.wrap(ca)
			Dim cr As java.nio.charset.CoderResult = cd.decode(bb, cb, True)
			If Not cr.underflow Then Throw New IllegalArgumentException(cr.ToString())
			cr = cd.flush(cb)
			If Not cr.underflow Then Throw New IllegalArgumentException(cr.ToString())
			Return New String(ca, 0, cb.position())
		End Function

		Friend Overrides Function ToString(ByVal ba As SByte()) As String
			Return ToString(ba, ba.Length)
		End Function

		Friend Function getBytes(ByVal s As String) As SByte()
			Dim ce As java.nio.charset.CharsetEncoder = encoder().reset()
			Dim ca As Char() = s.ToCharArray()
			Dim len As Integer = CInt(Fix(ca.Length * ce.maxBytesPerChar()))
			Dim ba As SByte() = New SByte(len - 1){}
			If len = 0 Then Return ba
			' UTF-8 only for now. Other ArrayDeocder only handles
			' CodingErrorAction.REPLACE mode.
			If isUTF8_Renamed AndAlso TypeOf ce Is sun.nio.cs.ArrayEncoder Then
				Dim blen As Integer = CType(ce, sun.nio.cs.ArrayEncoder).encode(ca, 0, ca.Length, ba)
				If blen = -1 Then ' malformed Throw New IllegalArgumentException("MALFORMED")
				Return java.util.Arrays.copyOf(ba, blen)
			End If
			Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(ba)
			Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.wrap(ca)
			Dim cr As java.nio.charset.CoderResult = ce.encode(cb, bb, True)
			If Not cr.underflow Then Throw New IllegalArgumentException(cr.ToString())
			cr = ce.flush(bb)
			If Not cr.underflow Then Throw New IllegalArgumentException(cr.ToString())
			If bb.position() = ba.Length Then ' defensive copy?
				Return ba
			Else
				Return java.util.Arrays.copyOf(ba, bb.position())
			End If
		End Function

		' assume invoked only if "this" is not utf8
		Friend Function getBytesUTF8(ByVal s As String) As SByte()
			If isUTF8_Renamed Then Return getBytes(s)
			If utf8 Is Nothing Then utf8 = New ZipCoder(java.nio.charset.StandardCharsets.UTF_8)
			Return utf8.getBytes(s)
		End Function


		Friend Function toStringUTF8(ByVal ba As SByte(), ByVal len As Integer) As String
			If isUTF8_Renamed Then Return ToString(ba, len)
			If utf8 Is Nothing Then utf8 = New ZipCoder(java.nio.charset.StandardCharsets.UTF_8)
			Return utf8.ToString(ba, len)
		End Function

		Friend Property uTF8 As Boolean
			Get
				Return isUTF8_Renamed
			End Get
		End Property

		Private cs As java.nio.charset.Charset
		Private dec As java.nio.charset.CharsetDecoder
		Private enc As java.nio.charset.CharsetEncoder
		Private isUTF8_Renamed As Boolean
		Private utf8 As ZipCoder

		Private Sub New(ByVal cs As java.nio.charset.Charset)
			Me.cs = cs
			Me.isUTF8_Renamed = cs.name().Equals(java.nio.charset.StandardCharsets.UTF_8.name())
		End Sub

		Shared Function [get](ByVal charset As java.nio.charset.Charset) As ZipCoder
			Return New ZipCoder(charset)
		End Function

		Private Function decoder() As java.nio.charset.CharsetDecoder
			If dec Is Nothing Then dec = cs.newDecoder().onMalformedInput(java.nio.charset.CodingErrorAction.REPORT).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPORT)
			Return dec
		End Function

		Private Function encoder() As java.nio.charset.CharsetEncoder
			If enc Is Nothing Then enc = cs.newEncoder().onMalformedInput(java.nio.charset.CodingErrorAction.REPORT).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPORT)
			Return enc
		End Function
	End Class

End Namespace
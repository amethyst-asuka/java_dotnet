Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang


	''' <summary>
	''' Utility class for string encoding and decoding.
	''' </summary>

	Friend Class StringCoding

		Private Sub New()
		End Sub

		''' <summary>
		''' The cached coders for each thread </summary>
		Private Shared ReadOnly decoder As New ThreadLocal(Of SoftReference(Of StringDecoder))
		Private Shared ReadOnly encoder As New ThreadLocal(Of SoftReference(Of StringEncoder))

		Private Shared warnUnsupportedCharset_Renamed As Boolean = True

		Private Shared Function deref(Of T)(  tl As ThreadLocal(Of SoftReference(Of T))) As T
			Dim sr As SoftReference(Of T) = tl.get()
			If sr Is Nothing Then Return Nothing
			Return sr.get()
		End Function

		Private Shared Sub [set](Of T)(  tl As ThreadLocal(Of SoftReference(Of T)),   ob As T)
			tl.set(New SoftReference(Of T)(ob))
		End Sub

		' Trim the given byte array to the given length
		'
		Private Shared Function safeTrim(  ba As SByte(),   len As Integer,   cs As java.nio.charset.Charset,   isTrusted As Boolean) As SByte()
			If len = ba.Length AndAlso (isTrusted OrElse System.securityManager Is Nothing) Then
				Return ba
			Else
				Return java.util.Arrays.copyOf(ba, len)
			End If
		End Function

		' Trim the given char array to the given length
		'
		Private Shared Function safeTrim(  ca As Char(),   len As Integer,   cs As java.nio.charset.Charset,   isTrusted As Boolean) As Char()
			If len = ca.Length AndAlso (isTrusted OrElse System.securityManager Is Nothing) Then
				Return ca
			Else
				Return java.util.Arrays.copyOf(ca, len)
			End If
		End Function

		Private Shared Function scale(  len As Integer,   expansionFactor As Single) As Integer
			' We need to perform double, not float, arithmetic; otherwise
			' we lose low order bits when len is larger than 2**24.
			Return CInt(Fix(len * CDbl(expansionFactor)))
		End Function

		Private Shared Function lookupCharset(  csn As String) As java.nio.charset.Charset
			If java.nio.charset.Charset.isSupported(csn) Then
				Try
					Return java.nio.charset.Charset.forName(csn)
				Catch x As java.nio.charset.UnsupportedCharsetException
					Throw New [Error](x)
				End Try
			End If
			Return Nothing
		End Function

		Private Shared Sub warnUnsupportedCharset(  csn As String)
			If warnUnsupportedCharset_Renamed Then
				' Use sun.misc.MessageUtils rather than the Logging API or
				' System.err since this method may be called during VM
				' initialization before either is available.
				sun.misc.MessageUtils.err("WARNING: Default charset " & csn & " not supported, using ISO-8859-1 instead")
				warnUnsupportedCharset_Renamed = False
			End If
		End Sub


		' -- Decoding --
		Private Class StringDecoder
			Private ReadOnly requestedCharsetName_Renamed As String
			Private ReadOnly cs As java.nio.charset.Charset
			Private ReadOnly cd As java.nio.charset.CharsetDecoder
			Private ReadOnly isTrusted As Boolean

			Private Sub New(  cs As java.nio.charset.Charset,   rcn As String)
				Me.requestedCharsetName_Renamed = rcn
				Me.cs = cs
				Me.cd = cs.newDecoder().onMalformedInput(java.nio.charset.CodingErrorAction.REPLACE).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPLACE)
				Me.isTrusted = (cs.GetType().classLoader0 Is Nothing)
			End Sub

			Friend Overridable Function charsetName() As String
				If TypeOf cs Is sun.nio.cs.HistoricallyNamedCharset Then Return CType(cs, sun.nio.cs.HistoricallyNamedCharset).historicalName()
				Return cs.name()
			End Function

			Friend Function requestedCharsetName() As String
				Return requestedCharsetName_Renamed
			End Function

			Friend Overridable Function decode(  ba As SByte(),   [off] As Integer,   len As Integer) As Char()
				Dim en As Integer = scale(len, cd.maxCharsPerByte())
				Dim ca As Char() = New Char(en - 1){}
				If len = 0 Then Return ca
				If TypeOf cd Is sun.nio.cs.ArrayDecoder Then
					Dim clen As Integer = CType(cd, sun.nio.cs.ArrayDecoder).decode(ba, [off], len, ca)
					Return safeTrim(ca, clen, cs, isTrusted)
				Else
					cd.reset()
					Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(ba, [off], len)
					Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.wrap(ca)
					Try
						Dim cr As java.nio.charset.CoderResult = cd.decode(bb, cb, True)
						If Not cr.underflow Then cr.throwException()
						cr = cd.flush(cb)
						If Not cr.underflow Then cr.throwException()
					Catch x As java.nio.charset.CharacterCodingException
						' Substitution is always enabled,
						' so this shouldn't happen
						Throw New [Error](x)
					End Try
					Return safeTrim(ca, cb.position(), cs, isTrusted)
				End If
			End Function
		End Class

		Friend Shared Function decode(  charsetName As String,   ba As SByte(),   [off] As Integer,   len As Integer) As Char()
			Dim sd As StringDecoder = deref(decoder)
			Dim csn As String = If(charsetName Is Nothing, "ISO-8859-1", charsetName)
			If (sd Is Nothing) OrElse Not(csn.Equals(sd.requestedCharsetName()) OrElse csn.Equals(sd.charsetName())) Then
				sd = Nothing
				Try
					Dim cs As java.nio.charset.Charset = lookupCharset(csn)
					If cs IsNot Nothing Then sd = New StringDecoder(cs, csn)
				Catch x As java.nio.charset.IllegalCharsetNameException
				End Try
				If sd Is Nothing Then Throw New java.io.UnsupportedEncodingException(csn)
				[set](decoder, sd)
			End If
			Return sd.decode(ba, [off], len)
		End Function

		Friend Shared Function decode(  cs As java.nio.charset.Charset,   ba As SByte(),   [off] As Integer,   len As Integer) As Char()
			' (1)We never cache the "external" cs, the only benefit of creating
			' an additional StringDe/Encoder object to wrap it is to share the
			' de/encode() method. These SD/E objects are short-lifed, the young-gen
			' gc should be able to take care of them well. But the best approash
			' is still not to generate them if not really necessary.
			' (2)The defensive copy of the input byte/char[] has a big performance
			' impact, as well as the outgoing result byte/char[]. Need to do the
			' optimization check of (sm==null && classLoader0==null) for both.
			' (3)getClass().getClassLoader0() is expensive
			' (4)There might be a timing gap in isTrusted setting. getClassLoader0()
			' is only chcked (and then isTrusted gets set) when (SM==null). It is
			' possible that the SM==null for now but then SM is NOT null later
			' when safeTrim() is invoked...the "safe" way to do is to redundant
			' check (... && (isTrusted || SM == null || getClassLoader0())) in trim
			' but it then can be argued that the SM is null when the opertaion
			' is started...
			Dim cd As java.nio.charset.CharsetDecoder = cs.newDecoder()
			Dim en As Integer = scale(len, cd.maxCharsPerByte())
			Dim ca As Char() = New Char(en - 1){}
			If len = 0 Then Return ca
			Dim isTrusted As Boolean = False
			If System.securityManager IsNot Nothing Then
				isTrusted = (cs.GetType().classLoader0 Is Nothing)
				If Not isTrusted Then
					ba = java.util.Arrays.copyOfRange(ba, [off], [off] + len)
					[off] = 0
				End If
			End If
			cd.onMalformedInput(java.nio.charset.CodingErrorAction.REPLACE).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPLACE).reset()
			If TypeOf cd Is sun.nio.cs.ArrayDecoder Then
				Dim clen As Integer = CType(cd, sun.nio.cs.ArrayDecoder).decode(ba, [off], len, ca)
				Return safeTrim(ca, clen, cs, isTrusted)
			Else
				Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(ba, [off], len)
				Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.wrap(ca)
				Try
					Dim cr As java.nio.charset.CoderResult = cd.decode(bb, cb, True)
					If Not cr.underflow Then cr.throwException()
					cr = cd.flush(cb)
					If Not cr.underflow Then cr.throwException()
				Catch x As java.nio.charset.CharacterCodingException
					' Substitution is always enabled,
					' so this shouldn't happen
					Throw New [Error](x)
				End Try
				Return safeTrim(ca, cb.position(), cs, isTrusted)
			End If
		End Function

		Friend Shared Function decode(  ba As SByte(),   [off] As Integer,   len As Integer) As Char()
			Dim csn As String = java.nio.charset.Charset.defaultCharset().name()
			Try
				' use charset name decode() variant which provides caching.
				Return decode(csn, ba, [off], len)
			Catch x As java.io.UnsupportedEncodingException
				warnUnsupportedCharset(csn)
			End Try
			Try
				Return decode("ISO-8859-1", ba, [off], len)
			Catch x As java.io.UnsupportedEncodingException
				' If this code is hit during VM initialization, MessageUtils is
				' the only way we will be able to get any kind of error message.
				sun.misc.MessageUtils.err("ISO-8859-1 charset not available: " & x.ToString())
				' If we can not find ISO-8859-1 (a required encoding) then things
				' are seriously wrong with the installation.
				Environment.Exit(1)
				Return Nothing
			End Try
		End Function

		' -- Encoding --
		Private Class StringEncoder
			Private cs As java.nio.charset.Charset
			Private ce As java.nio.charset.CharsetEncoder
			Private ReadOnly requestedCharsetName_Renamed As String
			Private ReadOnly isTrusted As Boolean

			Private Sub New(  cs As java.nio.charset.Charset,   rcn As String)
				Me.requestedCharsetName_Renamed = rcn
				Me.cs = cs
				Me.ce = cs.newEncoder().onMalformedInput(java.nio.charset.CodingErrorAction.REPLACE).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPLACE)
				Me.isTrusted = (cs.GetType().classLoader0 Is Nothing)
			End Sub

			Friend Overridable Function charsetName() As String
				If TypeOf cs Is sun.nio.cs.HistoricallyNamedCharset Then Return CType(cs, sun.nio.cs.HistoricallyNamedCharset).historicalName()
				Return cs.name()
			End Function

			Friend Function requestedCharsetName() As String
				Return requestedCharsetName_Renamed
			End Function

			Friend Overridable Function encode(  ca As Char(),   [off] As Integer,   len As Integer) As SByte()
				Dim en As Integer = scale(len, ce.maxBytesPerChar())
				Dim ba As SByte() = New SByte(en - 1){}
				If len = 0 Then Return ba
				If TypeOf ce Is sun.nio.cs.ArrayEncoder Then
					Dim blen As Integer = CType(ce, sun.nio.cs.ArrayEncoder).encode(ca, [off], len, ba)
					Return safeTrim(ba, blen, cs, isTrusted)
				Else
					ce.reset()
					Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(ba)
					Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.wrap(ca, [off], len)
					Try
						Dim cr As java.nio.charset.CoderResult = ce.encode(cb, bb, True)
						If Not cr.underflow Then cr.throwException()
						cr = ce.flush(bb)
						If Not cr.underflow Then cr.throwException()
					Catch x As java.nio.charset.CharacterCodingException
						' Substitution is always enabled,
						' so this shouldn't happen
						Throw New [Error](x)
					End Try
					Return safeTrim(ba, bb.position(), cs, isTrusted)
				End If
			End Function
		End Class

		Friend Shared Function encode(  charsetName As String,   ca As Char(),   [off] As Integer,   len As Integer) As SByte()
			Dim se As StringEncoder = deref(encoder)
			Dim csn As String = If(charsetName Is Nothing, "ISO-8859-1", charsetName)
			If (se Is Nothing) OrElse Not(csn.Equals(se.requestedCharsetName()) OrElse csn.Equals(se.charsetName())) Then
				se = Nothing
				Try
					Dim cs As java.nio.charset.Charset = lookupCharset(csn)
					If cs IsNot Nothing Then se = New StringEncoder(cs, csn)
				Catch x As java.nio.charset.IllegalCharsetNameException
				End Try
				If se Is Nothing Then Throw New java.io.UnsupportedEncodingException(csn)
				[set](encoder, se)
			End If
			Return se.encode(ca, [off], len)
		End Function

		Friend Shared Function encode(  cs As java.nio.charset.Charset,   ca As Char(),   [off] As Integer,   len As Integer) As SByte()
			Dim ce As java.nio.charset.CharsetEncoder = cs.newEncoder()
			Dim en As Integer = scale(len, ce.maxBytesPerChar())
			Dim ba As SByte() = New SByte(en - 1){}
			If len = 0 Then Return ba
			Dim isTrusted As Boolean = False
			If System.securityManager IsNot Nothing Then
				isTrusted = (cs.GetType().classLoader0 Is Nothing)
				If Not isTrusted Then
					ca = java.util.Arrays.copyOfRange(ca, [off], [off] + len)
					[off] = 0
				End If
			End If
			ce.onMalformedInput(java.nio.charset.CodingErrorAction.REPLACE).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPLACE).reset()
			If TypeOf ce Is sun.nio.cs.ArrayEncoder Then
				Dim blen As Integer = CType(ce, sun.nio.cs.ArrayEncoder).encode(ca, [off], len, ba)
				Return safeTrim(ba, blen, cs, isTrusted)
			Else
				Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(ba)
				Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.wrap(ca, [off], len)
				Try
					Dim cr As java.nio.charset.CoderResult = ce.encode(cb, bb, True)
					If Not cr.underflow Then cr.throwException()
					cr = ce.flush(bb)
					If Not cr.underflow Then cr.throwException()
				Catch x As java.nio.charset.CharacterCodingException
					Throw New [Error](x)
				End Try
				Return safeTrim(ba, bb.position(), cs, isTrusted)
			End If
		End Function

		Friend Shared Function encode(  ca As Char(),   [off] As Integer,   len As Integer) As SByte()
			Dim csn As String = java.nio.charset.Charset.defaultCharset().name()
			Try
				' use charset name encode() variant which provides caching.
				Return encode(csn, ca, [off], len)
			Catch x As java.io.UnsupportedEncodingException
				warnUnsupportedCharset(csn)
			End Try
			Try
				Return encode("ISO-8859-1", ca, [off], len)
			Catch x As java.io.UnsupportedEncodingException
				' If this code is hit during VM initialization, MessageUtils is
				' the only way we will be able to get any kind of error message.
				sun.misc.MessageUtils.err("ISO-8859-1 charset not available: " & x.ToString())
				' If we can not find ISO-8859-1 (a required encoding) then things
				' are seriously wrong with the installation.
				Environment.Exit(1)
				Return Nothing
			End Try
		End Function
	End Class

End Namespace
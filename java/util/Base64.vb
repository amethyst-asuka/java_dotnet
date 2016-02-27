Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util


	''' <summary>
	''' This class consists exclusively of static methods for obtaining
	''' encoders and decoders for the Base64 encoding scheme. The
	''' implementation of this class supports the following types of Base64
	''' as specified in
	''' <a href="http://www.ietf.org/rfc/rfc4648.txt">RFC 4648</a> and
	''' <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>.
	''' 
	''' <ul>
	''' <li><a name="basic"><b>Basic</b></a>
	''' <p> Uses "The Base64 Alphabet" as specified in Table 1 of
	'''     RFC 4648 and RFC 2045 for encoding and decoding operation.
	'''     The encoder does not add any line feed (line separator)
	'''     character. The decoder rejects data that contains characters
	'''     outside the base64 alphabet.</p></li>
	''' 
	''' <li><a name="url"><b>URL and Filename safe</b></a>
	''' <p> Uses the "URL and Filename safe Base64 Alphabet" as specified
	'''     in Table 2 of RFC 4648 for encoding and decoding. The
	'''     encoder does not add any line feed (line separator) character.
	'''     The decoder rejects data that contains characters outside the
	'''     base64 alphabet.</p></li>
	''' 
	''' <li><a name="mime"><b>MIME</b></a>
	''' <p> Uses the "The Base64 Alphabet" as specified in Table 1 of
	'''     RFC 2045 for encoding and decoding operation. The encoded output
	'''     must be represented in lines of no more than 76 characters each
	'''     and uses a carriage return {@code '\r'} followed immediately by
	'''     a linefeed {@code '\n'} as the line separator. No line separator
	'''     is added to the end of the encoded output. All line separators
	'''     or other characters not found in the base64 alphabet table are
	'''     ignored in decoding operation.</p></li>
	''' </ul>
	''' 
	''' <p> Unless otherwise noted, passing a {@code null} argument to a
	''' method of this class will cause a {@link java.lang.NullPointerException
	''' NullPointerException} to be thrown.
	''' 
	''' @author  Xueming Shen
	''' @since   1.8
	''' </summary>

	Public Class Base64

		Private Sub New()
		End Sub

		''' <summary>
		''' Returns a <seealso cref="Encoder"/> that encodes using the
		''' <a href="#basic">Basic</a> type base64 encoding scheme.
		''' </summary>
		''' <returns>  A Base64 encoder. </returns>
		Public Property Shared encoder As Encoder
			Get
				 Return Encoder.RFC4648
			End Get
		End Property

		''' <summary>
		''' Returns a <seealso cref="Encoder"/> that encodes using the
		''' <a href="#url">URL and Filename safe</a> type base64
		''' encoding scheme.
		''' </summary>
		''' <returns>  A Base64 encoder. </returns>
		Public Property Shared urlEncoder As Encoder
			Get
				 Return Encoder.RFC4648_URLSAFE
			End Get
		End Property

		''' <summary>
		''' Returns a <seealso cref="Encoder"/> that encodes using the
		''' <a href="#mime">MIME</a> type base64 encoding scheme.
		''' </summary>
		''' <returns>  A Base64 encoder. </returns>
		Public Property Shared mimeEncoder As Encoder
			Get
				Return Encoder.RFC2045
			End Get
		End Property

		''' <summary>
		''' Returns a <seealso cref="Encoder"/> that encodes using the
		''' <a href="#mime">MIME</a> type base64 encoding scheme
		''' with specified line length and line separators.
		''' </summary>
		''' <param name="lineLength">
		'''          the length of each output line (rounded down to nearest multiple
		'''          of 4). If {@code lineLength <= 0} the output will not be separated
		'''          in lines </param>
		''' <param name="lineSeparator">
		'''          the line separator for each output line
		''' </param>
		''' <returns>  A Base64 encoder.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if {@code lineSeparator} includes any
		'''          character of "The Base64 Alphabet" as specified in Table 1 of
		'''          RFC 2045. </exception>
		Public Shared Function getMimeEncoder(ByVal lineLength As Integer, ByVal lineSeparator As SByte()) As Encoder
			 Objects.requireNonNull(lineSeparator)
			 Dim base64_Renamed As Integer() = Decoder.fromBase64
			 For Each b As SByte In lineSeparator
				 If base64_Renamed(b And &Hff) <> -1 Then Throw New IllegalArgumentException("Illegal base64 line separator character 0x" & Convert.ToString(b, 16))
			 Next b
			 If lineLength <= 0 Then Return Encoder.RFC4648
			 Return New Encoder(False, lineSeparator, lineLength >> 2 << 2, True)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Decoder"/> that decodes using the
		''' <a href="#basic">Basic</a> type base64 encoding scheme.
		''' </summary>
		''' <returns>  A Base64 decoder. </returns>
		Public Property Shared decoder As Decoder
			Get
				 Return Decoder.RFC4648
			End Get
		End Property

		''' <summary>
		''' Returns a <seealso cref="Decoder"/> that decodes using the
		''' <a href="#url">URL and Filename safe</a> type base64
		''' encoding scheme.
		''' </summary>
		''' <returns>  A Base64 decoder. </returns>
		Public Property Shared urlDecoder As Decoder
			Get
				 Return Decoder.RFC4648_URLSAFE
			End Get
		End Property

		''' <summary>
		''' Returns a <seealso cref="Decoder"/> that decodes using the
		''' <a href="#mime">MIME</a> type base64 decoding scheme.
		''' </summary>
		''' <returns>  A Base64 decoder. </returns>
		Public Property Shared mimeDecoder As Decoder
			Get
				 Return Decoder.RFC2045
			End Get
		End Property

		''' <summary>
		''' This class implements an encoder for encoding byte data using
		''' the Base64 encoding scheme as specified in RFC 4648 and RFC 2045.
		''' 
		''' <p> Instances of <seealso cref="Encoder"/> class are safe for use by
		''' multiple concurrent threads.
		''' 
		''' <p> Unless otherwise noted, passing a {@code null} argument to
		''' a method of this class will cause a
		''' <seealso cref="java.lang.NullPointerException NullPointerException"/> to
		''' be thrown.
		''' </summary>
		''' <seealso cref=     Decoder
		''' @since   1.8 </seealso>
		Public Class Encoder

			Private ReadOnly newline As SByte()
			Private ReadOnly linemax As Integer
			Private ReadOnly isURL As Boolean
			Private ReadOnly doPadding As Boolean

			Private Sub New(ByVal isURL As Boolean, ByVal newline As SByte(), ByVal linemax As Integer, ByVal doPadding As Boolean)
				Me.isURL = isURL
				Me.newline = newline
				Me.linemax = linemax
				Me.doPadding = doPadding
			End Sub

			''' <summary>
			''' This array is a lookup table that translates 6-bit positive integer
			''' index values into their "Base64 Alphabet" equivalents as specified
			''' in "Table 1: The Base64 Alphabet" of RFC 2045 (and RFC 4648).
			''' </summary>
			Private Shared ReadOnly toBase64 As Char() = { "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "+"c, "/"c }

			''' <summary>
			''' It's the lookup table for "URL and Filename safe Base64" as specified
			''' in Table 2 of the RFC 4648, with the '+' and '/' changed to '-' and
			''' '_'. This table is used when BASE64_URL is specified.
			''' </summary>
			Private Shared ReadOnly toBase64URL As Char() = { "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "-"c, "_"c }

			Private Const MIMELINEMAX As Integer = 76
			Private Shared ReadOnly CRLF As SByte() = {ControlChars.Cr, ControlChars.Lf}

			Friend Shared ReadOnly RFC4648 As New Encoder(False, Nothing, -1, True)
			Friend Shared ReadOnly RFC4648_URLSAFE As New Encoder(True, Nothing, -1, True)
			Friend Shared ReadOnly RFC2045 As New Encoder(False, CRLF, MIMELINEMAX, True)

			Private Function outLength(ByVal srclen As Integer) As Integer
				Dim len As Integer = 0
				If doPadding Then
					len = 4 * ((srclen + 2) \ 3)
				Else
					Dim n As Integer = srclen Mod 3
					len = 4 * (srclen \ 3) + (If(n = 0, 0, n + 1))
				End If
				If linemax > 0 Then ' line separators len += (len - 1) \ linemax * newline.Length
				Return len
			End Function

			''' <summary>
			''' Encodes all bytes from the specified byte array into a newly-allocated
			''' byte array using the <seealso cref="Base64"/> encoding scheme. The returned byte
			''' array is of the length of the resulting bytes.
			''' </summary>
			''' <param name="src">
			'''          the byte array to encode </param>
			''' <returns>  A newly-allocated byte array containing the resulting
			'''          encoded bytes. </returns>
			Public Overridable Function encode(ByVal src As SByte()) As SByte()
				Dim len As Integer = outLength(src.Length) ' dst array size
				Dim dst As SByte() = New SByte(len - 1){}
				Dim ret As Integer = encode0(src, 0, src.Length, dst)
				If ret <> dst.Length Then Return Arrays.copyOf(dst, ret)
				Return dst
			End Function

			''' <summary>
			''' Encodes all bytes from the specified byte array using the
			''' <seealso cref="Base64"/> encoding scheme, writing the resulting bytes to the
			''' given output byte array, starting at offset 0.
			''' 
			''' <p> It is the responsibility of the invoker of this method to make
			''' sure the output byte array {@code dst} has enough space for encoding
			''' all bytes from the input byte array. No bytes will be written to the
			''' output byte array if the output byte array is not big enough.
			''' </summary>
			''' <param name="src">
			'''          the byte array to encode </param>
			''' <param name="dst">
			'''          the output byte array </param>
			''' <returns>  The number of bytes written to the output byte array
			''' </returns>
			''' <exception cref="IllegalArgumentException"> if {@code dst} does not have enough
			'''          space for encoding all input bytes. </exception>
			Public Overridable Function encode(ByVal src As SByte(), ByVal dst As SByte()) As Integer
				Dim len As Integer = outLength(src.Length) ' dst array size
				If dst.Length < len Then Throw New IllegalArgumentException("Output byte array is too small for encoding all input bytes")
				Return encode0(src, 0, src.Length, dst)
			End Function

			''' <summary>
			''' Encodes the specified byte array into a String using the <seealso cref="Base64"/>
			''' encoding scheme.
			''' 
			''' <p> This method first encodes all input bytes into a base64 encoded
			''' byte array and then constructs a new String by using the encoded byte
			''' array and the {@link java.nio.charset.StandardCharsets#ISO_8859_1
			''' ISO-8859-1} charset.
			''' 
			''' <p> In other words, an invocation of this method has exactly the same
			''' effect as invoking
			''' {@code new String(encode(src), StandardCharsets.ISO_8859_1)}.
			''' </summary>
			''' <param name="src">
			'''          the byte array to encode </param>
			''' <returns>  A String containing the resulting Base64 encoded characters </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function encodeToString(ByVal src As SByte()) As String
				Dim encoded As SByte() = encode(src)
				Return New String(encoded, 0, 0, encoded.Length)
			End Function

			''' <summary>
			''' Encodes all remaining bytes from the specified byte buffer into
			''' a newly-allocated ByteBuffer using the <seealso cref="Base64"/> encoding
			''' scheme.
			''' 
			''' Upon return, the source buffer's position will be updated to
			''' its limit; its limit will not have been changed. The returned
			''' output buffer's position will be zero and its limit will be the
			''' number of resulting encoded bytes.
			''' </summary>
			''' <param name="buffer">
			'''          the source ByteBuffer to encode </param>
			''' <returns>  A newly-allocated byte buffer containing the encoded bytes. </returns>
			Public Overridable Function encode(ByVal buffer As java.nio.ByteBuffer) As java.nio.ByteBuffer
				Dim len As Integer = outLength(buffer.remaining())
				Dim dst As SByte() = New SByte(len - 1){}
				Dim ret As Integer = 0
				If buffer.hasArray() Then
					ret = encode0(buffer.array(), buffer.arrayOffset() + buffer.position(), buffer.arrayOffset() + buffer.limit(), dst)
					buffer.position(buffer.limit())
				Else
					Dim src As SByte() = New SByte(buffer.remaining() - 1){}
					buffer.get(src)
					ret = encode0(src, 0, src.Length, dst)
				End If
				If ret <> dst.Length Then
					 dst = New SByte(ret - 1){}
					 Array.Copy(dst, dst, ret)
				End If
				Return java.nio.ByteBuffer.wrap(dst)
			End Function

			''' <summary>
			''' Wraps an output stream for encoding byte data using the <seealso cref="Base64"/>
			''' encoding scheme.
			''' 
			''' <p> It is recommended to promptly close the returned output stream after
			''' use, during which it will flush all possible leftover bytes to the underlying
			''' output stream. Closing the returned output stream will close the underlying
			''' output stream.
			''' </summary>
			''' <param name="os">
			'''          the output stream. </param>
			''' <returns>  the output stream for encoding the byte data into the
			'''          specified Base64 encoded format </returns>
			Public Overridable Function wrap(ByVal os As java.io.OutputStream) As java.io.OutputStream
				Objects.requireNonNull(os)
				Return New EncOutputStream(os,If(isURL, toBase64URL, toBase64), newline, linemax, doPadding)
			End Function

			''' <summary>
			''' Returns an encoder instance that encodes equivalently to this one,
			''' but without adding any padding character at the end of the encoded
			''' byte data.
			''' 
			''' <p> The encoding scheme of this encoder instance is unaffected by
			''' this invocation. The returned encoder instance should be used for
			''' non-padding encoding operation.
			''' </summary>
			''' <returns> an equivalent encoder that encodes without adding any
			'''         padding character at the end </returns>
			Public Overridable Function withoutPadding() As Encoder
				If Not doPadding Then Return Me
				Return New Encoder(isURL, newline, linemax, False)
			End Function

			Private Function encode0(ByVal src As SByte(), ByVal [off] As Integer, ByVal [end] As Integer, ByVal dst As SByte()) As Integer
				Dim base64_Renamed As Char() = If(isURL, toBase64URL, toBase64)
				Dim sp As Integer = [off]
				Dim slen As Integer = ([end] - [off]) \ 3 * 3
				Dim sl As Integer = [off] + slen
				If linemax > 0 AndAlso slen > linemax \ 4 * 3 Then slen = linemax \ 4 * 3
				Dim dp As Integer = 0
				Do While sp < sl
					Dim sl0 As Integer = System.Math.Min(sp + slen, sl)
					Dim sp0 As Integer = sp
					Dim dp0 As Integer = dp
					Do While sp0 < sl0
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Dim bits As Integer = (src(sp0++) And &Hff) << 16 Or (src(sp0++) And &Hff) << 8 Or (src(sp0++) And &Hff)
						dst(dp0) = AscW(base64_Renamed((CInt(CUInt(bits) >> 18)) And &H3f))
						dp0 += 1
						dst(dp0) = AscW(base64_Renamed((CInt(CUInt(bits) >> 12)) And &H3f))
						dp0 += 1
						dst(dp0) = AscW(base64_Renamed((CInt(CUInt(bits) >> 6)) And &H3f))
						dp0 += 1
						dst(dp0) = AscW(base64_Renamed(bits And &H3f))
						dp0 += 1
					Loop
					Dim dlen As Integer = (sl0 - sp) \ 3 * 4
					dp += dlen
					sp = sl0
					If dlen = linemax AndAlso sp < [end] Then
						For Each b As SByte In newline
							dst(dp) = b
							dp += 1
						Next b
					End If
				Loop
				If sp < [end] Then ' 1 or 2 leftover bytes
					Dim b0 As Integer = src(sp) And &Hff
					sp += 1
					dst(dp) = AscW(base64_Renamed(b0 >> 2))
					dp += 1
					If sp = [end] Then
						dst(dp) = AscW(base64_Renamed((b0 << 4) And &H3f))
						dp += 1
						If doPadding Then
							dst(dp) = AscW("="c)
							dp += 1
							dst(dp) = AscW("="c)
							dp += 1
						End If
					Else
						Dim b1 As Integer = src(sp) And &Hff
						sp += 1
						dst(dp) = AscW(base64_Renamed((b0 << 4) And &H3f Or (b1 >> 4)))
						dp += 1
						dst(dp) = AscW(base64_Renamed((b1 << 2) And &H3f))
						dp += 1
						If doPadding Then
							dst(dp) = AscW("="c)
							dp += 1
						End If
					End If
				End If
				Return dp
			End Function
		End Class

		''' <summary>
		''' This class implements a decoder for decoding byte data using the
		''' Base64 encoding scheme as specified in RFC 4648 and RFC 2045.
		''' 
		''' <p> The Base64 padding character {@code '='} is accepted and
		''' interpreted as the end of the encoded byte data, but is not
		''' required. So if the final unit of the encoded byte data only has
		''' two or three Base64 characters (without the corresponding padding
		''' character(s) padded), they are decoded as if followed by padding
		''' character(s). If there is a padding character present in the
		''' final unit, the correct number of padding character(s) must be
		''' present, otherwise {@code IllegalArgumentException} (
		''' {@code IOException} when reading from a Base64 stream) is thrown
		''' during decoding.
		''' 
		''' <p> Instances of <seealso cref="Decoder"/> class are safe for use by
		''' multiple concurrent threads.
		''' 
		''' <p> Unless otherwise noted, passing a {@code null} argument to
		''' a method of this class will cause a
		''' <seealso cref="java.lang.NullPointerException NullPointerException"/> to
		''' be thrown.
		''' </summary>
		''' <seealso cref=     Encoder
		''' @since   1.8 </seealso>
		Public Class Decoder

			Private ReadOnly isURL As Boolean
			Private ReadOnly isMIME As Boolean

			Private Sub New(ByVal isURL As Boolean, ByVal isMIME As Boolean)
				Me.isURL = isURL
				Me.isMIME = isMIME
			End Sub

			''' <summary>
			''' Lookup table for decoding unicode characters drawn from the
			''' "Base64 Alphabet" (as specified in Table 1 of RFC 2045) into
			''' their 6-bit positive integer equivalents.  Characters that
			''' are not in the Base64 alphabet but fall within the bounds of
			''' the array are encoded to -1.
			''' 
			''' </summary>
			Private Shared ReadOnly fromBase64 As Integer() = New Integer(255){}
			Shared Sub New()
				Arrays.fill(fromBase64, -1)
				For i As Integer = 0 To Encoder.toBase64.length - 1
					fromBase64(Encoder.toBase64(i)) = i
				Next i
				fromBase64(AscW("="c)) = -2
				Arrays.fill(fromBase64URL, -1)
				For i As Integer = 0 To Encoder.toBase64URL.length - 1
					fromBase64URL(Encoder.toBase64URL(i)) = i
				Next i
				fromBase64URL(AscW("="c)) = -2
			End Sub

			''' <summary>
			''' Lookup table for decoding "URL and Filename safe Base64 Alphabet"
			''' as specified in Table2 of the RFC 4648.
			''' </summary>
			Private Shared ReadOnly fromBase64URL As Integer() = New Integer(255){}


			Friend Shared ReadOnly RFC4648 As New Decoder(False, False)
			Friend Shared ReadOnly RFC4648_URLSAFE As New Decoder(True, False)
			Friend Shared ReadOnly RFC2045 As New Decoder(False, True)

			''' <summary>
			''' Decodes all bytes from the input byte array using the <seealso cref="Base64"/>
			''' encoding scheme, writing the results into a newly-allocated output
			''' byte array. The returned byte array is of the length of the resulting
			''' bytes.
			''' </summary>
			''' <param name="src">
			'''          the byte array to decode
			''' </param>
			''' <returns>  A newly-allocated byte array containing the decoded bytes.
			''' </returns>
			''' <exception cref="IllegalArgumentException">
			'''          if {@code src} is not in valid Base64 scheme </exception>
			Public Overridable Function decode(ByVal src As SByte()) As SByte()
				Dim dst As SByte() = New SByte(outLength(src, 0, src.Length) - 1){}
				Dim ret As Integer = decode0(src, 0, src.Length, dst)
				If ret <> dst.Length Then
					dst = New SByte(ret - 1){}
					Array.Copy(dst, dst, ret)
				End If
				Return dst
			End Function

			''' <summary>
			''' Decodes a Base64 encoded String into a newly-allocated byte array
			''' using the <seealso cref="Base64"/> encoding scheme.
			''' 
			''' <p> An invocation of this method has exactly the same effect as invoking
			''' {@code decode(src.getBytes(StandardCharsets.ISO_8859_1))}
			''' </summary>
			''' <param name="src">
			'''          the string to decode
			''' </param>
			''' <returns>  A newly-allocated byte array containing the decoded bytes.
			''' </returns>
			''' <exception cref="IllegalArgumentException">
			'''          if {@code src} is not in valid Base64 scheme </exception>
			Public Overridable Function decode(ByVal src As String) As SByte()
				Return decode(src.getBytes(java.nio.charset.StandardCharsets.ISO_8859_1))
			End Function

			''' <summary>
			''' Decodes all bytes from the input byte array using the <seealso cref="Base64"/>
			''' encoding scheme, writing the results into the given output byte array,
			''' starting at offset 0.
			''' 
			''' <p> It is the responsibility of the invoker of this method to make
			''' sure the output byte array {@code dst} has enough space for decoding
			''' all bytes from the input byte array. No bytes will be be written to
			''' the output byte array if the output byte array is not big enough.
			''' 
			''' <p> If the input byte array is not in valid Base64 encoding scheme
			''' then some bytes may have been written to the output byte array before
			''' IllegalargumentException is thrown.
			''' </summary>
			''' <param name="src">
			'''          the byte array to decode </param>
			''' <param name="dst">
			'''          the output byte array
			''' </param>
			''' <returns>  The number of bytes written to the output byte array
			''' </returns>
			''' <exception cref="IllegalArgumentException">
			'''          if {@code src} is not in valid Base64 scheme, or {@code dst}
			'''          does not have enough space for decoding all input bytes. </exception>
			Public Overridable Function decode(ByVal src As SByte(), ByVal dst As SByte()) As Integer
				Dim len As Integer = outLength(src, 0, src.Length)
				If dst.Length < len Then Throw New IllegalArgumentException("Output byte array is too small for decoding all input bytes")
				Return decode0(src, 0, src.Length, dst)
			End Function

			''' <summary>
			''' Decodes all bytes from the input byte buffer using the <seealso cref="Base64"/>
			''' encoding scheme, writing the results into a newly-allocated ByteBuffer.
			''' 
			''' <p> Upon return, the source buffer's position will be updated to
			''' its limit; its limit will not have been changed. The returned
			''' output buffer's position will be zero and its limit will be the
			''' number of resulting decoded bytes
			''' 
			''' <p> {@code IllegalArgumentException} is thrown if the input buffer
			''' is not in valid Base64 encoding scheme. The position of the input
			''' buffer will not be advanced in this case.
			''' </summary>
			''' <param name="buffer">
			'''          the ByteBuffer to decode
			''' </param>
			''' <returns>  A newly-allocated byte buffer containing the decoded bytes
			''' </returns>
			''' <exception cref="IllegalArgumentException">
			'''          if {@code src} is not in valid Base64 scheme. </exception>
			Public Overridable Function decode(ByVal buffer As java.nio.ByteBuffer) As java.nio.ByteBuffer
				Dim pos0 As Integer = buffer.position()
				Try
					Dim src As SByte()
					Dim sp, sl As Integer
					If buffer.hasArray() Then
						src = buffer.array()
						sp = buffer.arrayOffset() + buffer.position()
						sl = buffer.arrayOffset() + buffer.limit()
						buffer.position(buffer.limit())
					Else
						src = New SByte(buffer.remaining() - 1){}
						buffer.get(src)
						sp = 0
						sl = src.Length
					End If
					Dim dst As SByte() = New SByte(outLength(src, sp, sl) - 1){}
					Return java.nio.ByteBuffer.wrap(dst, 0, decode0(src, sp, sl, dst))
				Catch iae As IllegalArgumentException
					buffer.position(pos0)
					Throw iae
				End Try
			End Function

			''' <summary>
			''' Returns an input stream for decoding <seealso cref="Base64"/> encoded byte stream.
			''' 
			''' <p> The {@code read}  methods of the returned {@code InputStream} will
			''' throw {@code IOException} when reading bytes that cannot be decoded.
			''' 
			''' <p> Closing the returned input stream will close the underlying
			''' input stream.
			''' </summary>
			''' <param name="is">
			'''          the input stream
			''' </param>
			''' <returns>  the input stream for decoding the specified Base64 encoded
			'''          byte stream </returns>
			Public Overridable Function wrap(ByVal [is] As java.io.InputStream) As java.io.InputStream
				Objects.requireNonNull([is])
				Return New DecInputStream([is],If(isURL, fromBase64URL, fromBase64), isMIME)
			End Function

			Private Function outLength(ByVal src As SByte(), ByVal sp As Integer, ByVal sl As Integer) As Integer
				Dim base64_Renamed As Integer() = If(isURL, fromBase64URL, fromBase64)
				Dim paddings As Integer = 0
				Dim len As Integer = sl - sp
				If len = 0 Then Return 0
				If len < 2 Then
					If isMIME AndAlso base64_Renamed(0) = -1 Then Return 0
					Throw New IllegalArgumentException("Input byte[] should at least have 2 bytes for base64 bytes")
				End If
				If isMIME Then
					' scan all bytes to fill out all non-alphabet. a performance
					' trade-off of pre-scan or Arrays.copyOf
					Dim n As Integer = 0
					Do While sp < sl
						Dim b As Integer = src(sp) And &Hff
						sp += 1
						If b = AscW("="c) Then
							len -= (sl - sp + 1)
							Exit Do
						End If
						b = base64_Renamed(b)
						If b = -1 Then n += 1
					Loop
					len -= n
				Else
					If src(sl - 1) = AscW("="c) Then
						paddings += 1
						If src(sl - 2) = AscW("="c) Then paddings += 1
					End If
				End If
				If paddings = 0 AndAlso (len And &H3) <> 0 Then paddings = 4 - (len And &H3)
				Return 3 * ((len + 3) \ 4) - paddings
			End Function

			Private Function decode0(ByVal src As SByte(), ByVal sp As Integer, ByVal sl As Integer, ByVal dst As SByte()) As Integer
				Dim base64_Renamed As Integer() = If(isURL, fromBase64URL, fromBase64)
				Dim dp As Integer = 0
				Dim bits As Integer = 0
				Dim shiftto As Integer = 18 ' pos of first byte of 4-byte atom
				Do While sp < sl
					Dim b As Integer = src(sp) And &Hff
					sp += 1
					b = base64_Renamed(b)
					If b < 0 Then
						If b = -2 Then ' padding byte '='
							' =     shiftto==18 unnecessary padding
							' x=    shiftto==12 a dangling single x
							' x     to be handled together with non-padding case
							' xx=   shiftto==6&&sp==sl missing last =
							' xx=y  shiftto==6 last is not =
							Dim tempVar As Boolean = shiftto = 6 AndAlso (sp = sl OrElse src(sp) <> AscW("="c)) OrElse shiftto = 18
							sp += 1
							If tempVar Then Throw New IllegalArgumentException("Input byte array has wrong 4-byte ending unit")
							Exit Do
						End If
						If isMIME Then ' skip if for rfc2045
							Continue Do
						Else
							Throw New IllegalArgumentException("Illegal base64 character " & Convert.ToString(src(sp - 1), 16))
						End If
					End If
					bits = bits Or (b << shiftto)
					shiftto -= 6
					If shiftto < 0 Then
						dst(dp) = CByte(bits >> 16)
						dp += 1
						dst(dp) = CByte(bits >> 8)
						dp += 1
						dst(dp) = CByte(bits)
						dp += 1
						shiftto = 18
						bits = 0
					End If
				Loop
				' reached end of byte array or hit padding '=' characters.
				If shiftto = 6 Then
					dst(dp) = CByte(bits >> 16)
					dp += 1
				ElseIf shiftto = 0 Then
					dst(dp) = CByte(bits >> 16)
					dp += 1
					dst(dp) = CByte(bits >> 8)
					dp += 1
				ElseIf shiftto = 12 Then
					' dangling single "x", incorrectly encoded.
					Throw New IllegalArgumentException("Last unit does not have enough valid bits")
				End If
				' anything left is invalid, if is not MIME.
				' if MIME, ignore all non-base64 character
				Do While sp < sl
					Dim tempVar2 As Boolean = isMIME AndAlso base64_Renamed(src(sp)) < 0
					sp += 1
					If tempVar2 Then Continue Do
					Throw New IllegalArgumentException("Input byte array has incorrect ending byte at " & sp)
				Loop
				Return dp
			End Function
		End Class

	'    
	'     * An output stream for encoding bytes into the Base64.
	'     
		Private Class EncOutputStream
			Inherits java.io.FilterOutputStream

			Private leftover As Integer = 0
			Private b0, b1, b2 As Integer
			Private closed As Boolean = False

			Private ReadOnly base64_Renamed As Char() ' byte->base64 mapping
			Private ReadOnly newline As SByte() ' line separator, if needed
			Private ReadOnly linemax As Integer
			Private ReadOnly doPadding As Boolean ' whether or not to pad
			Private linepos As Integer = 0

			Friend Sub New(ByVal os As java.io.OutputStream, ByVal base64_Renamed As Char(), ByVal newline As SByte(), ByVal linemax As Integer, ByVal doPadding As Boolean)
				MyBase.New(os)
				Me.base64_Renamed = base64_Renamed
				Me.newline = newline
				Me.linemax = linemax
				Me.doPadding = doPadding
			End Sub

			Public Overrides Sub write(ByVal b As Integer)
				Dim buf As SByte() = New SByte(0){}
				buf(0) = CByte(b And &Hff)
				write(buf, 0, 1)
			End Sub

			Private Sub checkNewline()
				If linepos = linemax Then
					out.write(newline)
					linepos = 0
				End If
			End Sub

			Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
				If closed Then Throw New java.io.IOException("Stream is closed")
				If [off] < 0 OrElse len < 0 OrElse [off] + len > b.Length Then Throw New ArrayIndexOutOfBoundsException
				If len = 0 Then Return
				If leftover <> 0 Then
					If leftover = 1 Then
						b1 = b([off]) And &Hff
						[off] += 1
						len -= 1
						If len = 0 Then
							leftover += 1
							Return
						End If
					End If
					b2 = b([off]) And &Hff
					[off] += 1
					len -= 1
					checkNewline()
					out.write(base64_Renamed(b0 >> 2))
					out.write(base64_Renamed((b0 << 4) And &H3f Or (b1 >> 4)))
					out.write(base64_Renamed((b1 << 2) And &H3f Or (b2 >> 6)))
					out.write(base64_Renamed(b2 And &H3f))
					linepos += 4
				End If
				Dim nBits24 As Integer = len \ 3
				leftover = len - (nBits24 * 3)
				Dim tempVar As Boolean = nBits24 > 0
				nBits24 -= 1
				Do While tempVar
					checkNewline()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim bits As Integer = (b([off]++) And &Hff) << 16 Or (b([off]++) And &Hff) << 8 Or (b([off]++) And &Hff)
					out.write(base64_Renamed((CInt(CUInt(bits) >> 18)) And &H3f))
					out.write(base64_Renamed((CInt(CUInt(bits) >> 12)) And &H3f))
					out.write(base64_Renamed((CInt(CUInt(bits) >> 6)) And &H3f))
					out.write(base64_Renamed(bits And &H3f))
					linepos += 4
					tempVar = nBits24 > 0
					nBits24 -= 1
				Loop
				If leftover = 1 Then
					b0 = b([off]) And &Hff
					[off] += 1
				ElseIf leftover = 2 Then
					b0 = b([off]) And &Hff
					[off] += 1
					b1 = b([off]) And &Hff
					[off] += 1
				End If
			End Sub

			Public Overrides Sub close()
				If Not closed Then
					closed = True
					If leftover = 1 Then
						checkNewline()
						out.write(base64_Renamed(b0 >> 2))
						out.write(base64_Renamed((b0 << 4) And &H3f))
						If doPadding Then
							out.write("="c)
							out.write("="c)
						End If
					ElseIf leftover = 2 Then
						checkNewline()
						out.write(base64_Renamed(b0 >> 2))
						out.write(base64_Renamed((b0 << 4) And &H3f Or (b1 >> 4)))
						out.write(base64_Renamed((b1 << 2) And &H3f))
						If doPadding Then out.write("="c)
					End If
					leftover = 0
					out.close()
				End If
			End Sub
		End Class

	'    
	'     * An input stream for decoding Base64 bytes
	'     
		Private Class DecInputStream
			Inherits java.io.InputStream

			Private ReadOnly [is] As java.io.InputStream
			Private ReadOnly isMIME As Boolean
			Private ReadOnly base64_Renamed As Integer() ' base64 -> byte mapping
			Private bits As Integer = 0 ' 24-bit buffer for decoding
			Private nextin As Integer = 18 ' next available "off" in "bits" for input;
											 ' -> 18, 12, 6, 0
			Private nextout As Integer = -8 ' next available "off" in "bits" for output;
											 ' -> 8, 0, -8 (no byte for output)
			Private eof As Boolean = False
			Private closed As Boolean = False

			Friend Sub New(ByVal [is] As java.io.InputStream, ByVal base64_Renamed As Integer(), ByVal isMIME As Boolean)
				Me.is = [is]
				Me.base64_Renamed = base64_Renamed
				Me.isMIME = isMIME
			End Sub

			Private sbBuf As SByte() = New SByte(0){}

			Public Overrides Function read() As Integer
				Return If(read(sbBuf, 0, 1) = -1, -1, sbBuf(0) And &Hff)
			End Function

			Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
				If closed Then Throw New java.io.IOException("Stream is closed")
				If eof AndAlso nextout < 0 Then ' eof and no leftover Return -1
				If [off] < 0 OrElse len < 0 OrElse len > b.Length - [off] Then Throw New IndexOutOfBoundsException
				Dim oldOff As Integer = [off]
				If nextout >= 0 Then ' leftover output byte(s) in bits buf
					Do
						If len = 0 Then Return [off] - oldOff
						b([off]) = CByte(bits >> nextout)
						[off] += 1
						len -= 1
						nextout -= 8
					Loop While nextout >= 0
					bits = 0
				End If
				Do While len > 0
					Dim v As Integer = [is].read()
					If v = -1 Then
						eof = True
						If nextin <> 18 Then
							If nextin = 12 Then Throw New java.io.IOException("Base64 stream has one un-decoded dangling java.lang.[Byte].")
							' treat ending xx/xxx without padding character legal.
							' same logic as v == '=' below
							b([off]) = CByte(bits >> (16))
							[off] += 1
							len -= 1
							If nextin = 0 Then ' only one padding byte
								If len = 0 Then ' no enough output space
									bits >>= 8 ' shift to lowest byte
									nextout = 0
								Else
									b([off]) = CByte(bits >> 8)
									[off] += 1
								End If
							End If
						End If
						If [off] = oldOff Then
							Return -1
						Else
							Return [off] - oldOff
						End If
					End If
					If v = AscW("="c) Then ' padding byte(s)
						' =     shiftto==18 unnecessary padding
						' x=    shiftto==12 dangling x, invalid unit
						' xx=   shiftto==6 && missing last '='
						' xx=y  or last is not '='
						If nextin = 18 OrElse nextin = 12 OrElse nextin = 6 AndAlso [is].read() <> AscW("="c) Then Throw New java.io.IOException("Illegal base64 ending sequence:" & nextin)
						b([off]) = CByte(bits >> (16))
						[off] += 1
						len -= 1
						If nextin = 0 Then ' only one padding byte
							If len = 0 Then ' no enough output space
								bits >>= 8 ' shift to lowest byte
								nextout = 0
							Else
								b([off]) = CByte(bits >> 8)
								[off] += 1
							End If
						End If
						eof = True
						Exit Do
					End If
					v = base64_Renamed(v)
					If v = -1 Then
						If isMIME Then ' skip if for rfc2045
							Continue Do
						Else
							Throw New java.io.IOException("Illegal base64 character " & Convert.ToString(v, 16))
						End If
					End If
					bits = bits Or (v << nextin)
					If nextin = 0 Then
						nextin = 18 ' clear for next
						nextout = 16
						Do While nextout >= 0
							b([off]) = CByte(bits >> nextout)
							[off] += 1
							len -= 1
							nextout -= 8
							If len = 0 AndAlso nextout >= 0 Then ' don't clean "bits" Return [off] - oldOff
						Loop
						bits = 0
					Else
						nextin -= 6
					End If
				Loop
				Return [off] - oldOff
			End Function

			Public Overrides Function available() As Integer
				If closed Then Throw New java.io.IOException("Stream is closed")
				Return [is].available() ' TBD:
			End Function

			Public Overrides Sub close()
				If Not closed Then
					closed = True
					[is].close()
				End If
			End Sub
		End Class
	End Class

End Namespace
Imports System
Imports System.Diagnostics
Imports System.Text

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind



	''' <summary>
	''' This class is the JAXB RI's default implementation of the
	''' <seealso cref="DatatypeConverterInterface"/>.
	''' 
	''' <p>
	''' When client applications specify the use of the static print/parse
	''' methods in <seealso cref="DatatypeConverter"/>, it will delegate
	''' to this class.
	''' 
	''' <p>
	''' This class is responsible for whitespace normalization.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li></ul>
	''' @since JAXB2.1
	''' </summary>
	Friend NotInheritable Class DatatypeConverterImpl
		Implements DatatypeConverterInterface

		''' <summary>
		''' To avoid re-creating instances, we cache one instance.
		''' </summary>
		Public Shared ReadOnly theInstance As DatatypeConverterInterface = New DatatypeConverterImpl

		Protected Friend Sub New()
		End Sub

		Public Function parseString(ByVal lexicalXSDString As String) As String Implements DatatypeConverterInterface.parseString
			Return lexicalXSDString
		End Function

		Public Function parseInteger(ByVal lexicalXSDInteger As String) As System.Numerics.BigInteger Implements DatatypeConverterInterface.parseInteger
			Return _parseInteger(lexicalXSDInteger)
		End Function

		Public Shared Function _parseInteger(ByVal s As CharSequence) As System.Numerics.BigInteger
			Return New System.Numerics.BigInteger(removeOptionalPlus(WhiteSpaceProcessor.Trim(s)).ToString())
		End Function

		Public Function printInteger(ByVal val As System.Numerics.BigInteger) As String Implements DatatypeConverterInterface.printInteger
			Return _printInteger(val)
		End Function

		Public Shared Function _printInteger(ByVal val As System.Numerics.BigInteger) As String
			Return val.ToString()
		End Function

		Public Function parseInt(ByVal s As String) As Integer Implements DatatypeConverterInterface.parseInt
			Return _parseInt(s)
		End Function

		''' <summary>
		''' Faster but less robust String->int conversion.
		''' 
		''' Note that:
		''' <ol>
		'''  <li>XML Schema allows '+', but <seealso cref="Integer#valueOf(String)"/> is not.
		'''  <li>XML Schema allows leading and trailing (but not in-between) whitespaces.
		'''      <seealso cref="Integer#valueOf(String)"/> doesn't allow any.
		''' </ol>
		''' </summary>
		Public Shared Function _parseInt(ByVal s As CharSequence) As Integer
			Dim len As Integer = s.length()
			Dim sign As Integer = 1

			Dim r As Integer = 0

			For i As Integer = 0 To len - 1
				Dim ch As Char = s.Chars(i)
				If WhiteSpaceProcessor.isWhiteSpace(ch) Then
					' skip whitespace
				ElseIf "0"c <= ch AndAlso ch <= "9"c Then
					r = r * 10 + (AscW(ch) - AscW("0"c))
				ElseIf ch = "-"c Then
					sign = -1
				ElseIf ch = "+"c Then
					' noop
				Else
					Throw New NumberFormatException("Not a number: " & s)
				End If
			Next i

			Return r * sign
		End Function

		Public Function parseLong(ByVal lexicalXSLong As String) As Long Implements DatatypeConverterInterface.parseLong
			Return _parseLong(lexicalXSLong)
		End Function

		Public Shared Function _parseLong(ByVal s As CharSequence) As Long
			Return Convert.ToInt64(removeOptionalPlus(WhiteSpaceProcessor.Trim(s)).ToString())
		End Function

		Public Function parseShort(ByVal lexicalXSDShort As String) As Short Implements DatatypeConverterInterface.parseShort
			Return _parseShort(lexicalXSDShort)
		End Function

		Public Shared Function _parseShort(ByVal s As CharSequence) As Short
			Return CShort(_parseInt(s))
		End Function

		Public Function printShort(ByVal val As Short) As String Implements DatatypeConverterInterface.printShort
			Return _printShort(val)
		End Function

		Public Shared Function _printShort(ByVal val As Short) As String
			Return Convert.ToString(val)
		End Function

		Public Function parseDecimal(ByVal content As String) As Decimal Implements DatatypeConverterInterface.parseDecimal
			Return _parseDecimal(content)
		End Function

		Public Shared Function _parseDecimal(ByVal content As CharSequence) As Decimal
			content = WhiteSpaceProcessor.Trim(content)

			If content.length() <= 0 Then Return Nothing

			Return New Decimal(content.ToString())

			' from purely XML Schema perspective,
			' this implementation has a problem, since
			' in xs:decimal "1.0" and "1" is equal whereas the above
			' code will return different values for those two forms.
			'
			' the code was originally using com.sun.msv.datatype.xsd.NumberType.load,
			' but a profiling showed that the process of normalizing "1.0" into "1"
			' could take non-trivial time.
			'
			' also, from the user's point of view, one might be surprised if
			' 1 (not 1.0) is returned from "1.000"
		End Function

		Public Function parseFloat(ByVal lexicalXSDFloat As String) As Single Implements DatatypeConverterInterface.parseFloat
			Return _parseFloat(lexicalXSDFloat)
		End Function

		Public Shared Function _parseFloat(ByVal _val As CharSequence) As Single
			Dim s As String = WhiteSpaceProcessor.Trim(_val).ToString()
	'         Incompatibilities of XML Schema's float "xfloat" and Java's float "jfloat"
	'
	'         * jfloat.valueOf ignores leading and trailing whitespaces,
	'        whereas this is not allowed in xfloat.
	'         * jfloat.valueOf allows "float type suffix" (f, F) to be
	'        appended after float literal (e.g., 1.52e-2f), whereare
	'        this is not the case of xfloat.
	'
	'        gray zone
	'        ---------
	'         * jfloat allows ".523". And there is no clear statement that mentions
	'        this case in xfloat. Although probably this is allowed.
	'         *
	'         

			If s.Equals("NaN") Then Return Single.NaN
			If s.Equals("INF") Then Return Single.PositiveInfinity
			If s.Equals("-INF") Then Return Single.NegativeInfinity

			If s.Length = 0 OrElse (Not isDigitOrPeriodOrSign(s.Chars(0))) OrElse (Not isDigitOrPeriodOrSign(s.Chars(s.Length - 1))) Then Throw New NumberFormatException

			' these screening process is necessary due to the wobble of Float.valueOf method
			Return Convert.ToSingle(s)
		End Function

		Public Function printFloat(ByVal v As Single) As String Implements DatatypeConverterInterface.printFloat
			Return _printFloat(v)
		End Function

		Public Shared Function _printFloat(ByVal v As Single) As String
			If Single.IsNaN(v) Then Return "NaN"
			If v = Single.PositiveInfinity Then Return "INF"
			If v = Single.NegativeInfinity Then Return "-INF"
			Return Convert.ToString(v)
		End Function

		Public Function parseDouble(ByVal lexicalXSDDouble As String) As Double Implements DatatypeConverterInterface.parseDouble
			Return _parseDouble(lexicalXSDDouble)
		End Function

		Public Shared Function _parseDouble(ByVal _val As CharSequence) As Double
			Dim val As String = WhiteSpaceProcessor.Trim(_val).ToString()

			If val.Equals("NaN") Then Return Double.NaN
			If val.Equals("INF") Then Return Double.PositiveInfinity
			If val.Equals("-INF") Then Return Double.NegativeInfinity

			If val.Length = 0 OrElse (Not isDigitOrPeriodOrSign(val.Chars(0))) OrElse (Not isDigitOrPeriodOrSign(val.Chars(val.Length - 1))) Then Throw New NumberFormatException(val)


			' these screening process is necessary due to the wobble of Float.valueOf method
			Return Convert.ToDouble(val)
		End Function

		Public Function parseBoolean(ByVal lexicalXSDBoolean As String) As Boolean Implements DatatypeConverterInterface.parseBoolean
			Dim b As Boolean? = _parseBoolean(lexicalXSDBoolean)
			Return If(b Is Nothing, False, b)
		End Function

		Public Shared Function _parseBoolean(ByVal literal As CharSequence) As Boolean?
			If literal Is Nothing Then Return Nothing

			Dim i As Integer = 0
			Dim len As Integer = literal.length()
			Dim ch As Char
			Dim value As Boolean = False

			If literal.length() <= 0 Then Return Nothing

			Do
				ch = literal.Chars(i)
				i += 1
			Loop While WhiteSpaceProcessor.isWhiteSpace(ch) AndAlso i < len

			Dim strIndex As Integer = 0

			Select Case ch
				Case "1"c
					value = True
				Case "0"c
					value = False
				Case "t"c
					Dim strTrue As String = "rue"
					Dim tempVar As Boolean
					Do
						ch = literal.Chars(i)
						i += 1
						tempVar = (strTrue.Chars(strIndex) = ch) AndAlso i < len AndAlso strIndex < 3
						strIndex += 1
					Loop While tempVar

					If strIndex = 3 Then
						value = True
					Else
						Return False
					End If
	'                    throw new IllegalArgumentException("String \"" + literal + "\" is not valid boolean value.");

				Case "f"c
					Dim strFalse As String = "alse"
					Dim tempVar2 As Boolean
					Do
						ch = literal.Chars(i)
						i += 1
						tempVar2 = (strFalse.Chars(strIndex) = ch) AndAlso i < len AndAlso strIndex < 4
						strIndex += 1
					Loop While tempVar2


					If strIndex = 4 Then
						value = False
					Else
						Return False
					End If
	'                    throw new IllegalArgumentException("String \"" + literal + "\" is not valid boolean value.");

			End Select

			If i < len Then
				Do
					ch = literal.Chars(i)
					i += 1
				Loop While WhiteSpaceProcessor.isWhiteSpace(ch) AndAlso i < len
			End If

			If i = len Then
				Return value
			Else
				Return Nothing
			End If
	'            throw new IllegalArgumentException("String \"" + literal + "\" is not valid boolean value.");
		End Function

		Public Function printBoolean(ByVal val As Boolean) As String Implements DatatypeConverterInterface.printBoolean
			Return If(val, "true", "false")
		End Function

		Public Shared Function _printBoolean(ByVal val As Boolean) As String
			Return If(val, "true", "false")
		End Function

		Public Function parseByte(ByVal lexicalXSDByte As String) As SByte Implements DatatypeConverterInterface.parseByte
			Return _parseByte(lexicalXSDByte)
		End Function

		Public Shared Function _parseByte(ByVal literal As CharSequence) As SByte
			Return CByte(_parseInt(literal))
		End Function

		Public Function printByte(ByVal val As SByte) As String Implements DatatypeConverterInterface.printByte
			Return _printByte(val)
		End Function

		Public Shared Function _printByte(ByVal val As SByte) As String
			Return Convert.ToString(val)
		End Function

		Public Function parseQName(ByVal lexicalXSDQName As String, ByVal nsc As javax.xml.namespace.NamespaceContext) As javax.xml.namespace.QName Implements DatatypeConverterInterface.parseQName
			Return _parseQName(lexicalXSDQName, nsc)
		End Function

		''' <returns> null if fails to convert. </returns>
		Public Shared Function _parseQName(ByVal text As CharSequence, ByVal nsc As javax.xml.namespace.NamespaceContext) As javax.xml.namespace.QName
			Dim length As Integer = text.length()

			' trim whitespace
			Dim start As Integer = 0
			Do While start < length AndAlso WhiteSpaceProcessor.isWhiteSpace(text.Chars(start))
				start += 1
			Loop

			Dim [end] As Integer = length
			Do While [end] > start AndAlso WhiteSpaceProcessor.isWhiteSpace(text.Chars([end] - 1))
				[end] -= 1
			Loop

			If [end] = start Then Throw New System.ArgumentException("input is empty")


			Dim uri As String
			Dim localPart As String
			Dim prefix As String

			' search ':'
			Dim idx As Integer = start + 1 ' no point in searching the first char. that's not valid.
			Do While idx < [end] AndAlso text.Chars(idx) <> ":"c
				idx += 1
			Loop

			If idx = [end] Then
				uri = nsc.getNamespaceURI("")
				localPart = text.subSequence(start, [end]).ToString()
				prefix = ""
			Else
				' Prefix exists, check everything
				prefix = text.subSequence(start, idx).ToString()
				localPart = text.subSequence(idx + 1, [end]).ToString()
				uri = nsc.getNamespaceURI(prefix)
				' uri can never be null according to javadoc,
				' but some users reported that there are implementations that return null.
				If uri Is Nothing OrElse uri.Length = 0 Then ' crap. the NamespaceContext interface is broken. Throw New System.ArgumentException("prefix " & prefix & " is not bound to a namespace")
			End If

			Return New javax.xml.namespace.QName(uri, localPart, prefix)
		End Function

		Public Function parseDateTime(ByVal lexicalXSDDateTime As String) As DateTime Implements DatatypeConverterInterface.parseDateTime
			Return _parseDateTime(lexicalXSDDateTime)
		End Function

		Public Shared Function _parseDateTime(ByVal s As CharSequence) As java.util.GregorianCalendar
			Dim val As String = WhiteSpaceProcessor.Trim(s).ToString()
			Return datatypeFactory.newXMLGregorianCalendar(val).toGregorianCalendar()
		End Function

		Public Function printDateTime(ByVal val As DateTime) As String Implements DatatypeConverterInterface.printDateTime
			Return _printDateTime(val)
		End Function

		Public Shared Function _printDateTime(ByVal val As DateTime) As String
			Return CalendarFormatter.doFormat("%Y-%M-%DT%h:%m:%s%z", val)
		End Function

		Public Function parseBase64Binary(ByVal lexicalXSDBase64Binary As String) As SByte() Implements DatatypeConverterInterface.parseBase64Binary
			Return _parseBase64Binary(lexicalXSDBase64Binary)
		End Function

		Public Function parseHexBinary(ByVal s As String) As SByte() Implements DatatypeConverterInterface.parseHexBinary
			Dim len As Integer = s.Length

			' "111" is not a valid hex encoding.
			If len Mod 2 <> 0 Then Throw New System.ArgumentException("hexBinary needs to be even-length: " & s)

			Dim out As SByte() = New SByte(len \ 2 - 1){}

			For i As Integer = 0 To len - 1 Step 2
				Dim h As Integer = hexToBin(s.Chars(i))
				Dim l As Integer = hexToBin(s.Chars(i + 1))
				If h = -1 OrElse l = -1 Then Throw New System.ArgumentException("contains illegal character for hexBinary: " & s)

				out(i \ 2) = CByte(h * 16 + l)
			Next i

			Return out
		End Function

		Private Shared Function hexToBin(ByVal ch As Char) As Integer
			If "0"c <= ch AndAlso ch <= "9"c Then Return AscW(ch) - AscW("0"c)
			If "A"c <= ch AndAlso ch <= "F"c Then Return AscW(ch) - AscW("A"c) + 10
			If "a"c <= ch AndAlso ch <= "f"c Then Return AscW(ch) - AscW("a"c) + 10
			Return -1
		End Function
		Private Shared ReadOnly hexCode As Char() = "0123456789ABCDEF".toCharArray()

		Public Function printHexBinary(ByVal data As SByte()) As String Implements DatatypeConverterInterface.printHexBinary
			Dim r As New StringBuilder(data.Length * 2)
			For Each b As SByte In data
				r.Append(hexCode((b >> 4) And &HF))
				r.Append(hexCode((b And &HF)))
			Next b
			Return r.ToString()
		End Function

		Public Function parseUnsignedInt(ByVal lexicalXSDUnsignedInt As String) As Long Implements DatatypeConverterInterface.parseUnsignedInt
			Return _parseLong(lexicalXSDUnsignedInt)
		End Function

		Public Function printUnsignedInt(ByVal val As Long) As String Implements DatatypeConverterInterface.printUnsignedInt
			Return _printLong(val)
		End Function

		Public Function parseUnsignedShort(ByVal lexicalXSDUnsignedShort As String) As Integer Implements DatatypeConverterInterface.parseUnsignedShort
			Return _parseInt(lexicalXSDUnsignedShort)
		End Function

		Public Function parseTime(ByVal lexicalXSDTime As String) As DateTime Implements DatatypeConverterInterface.parseTime
			Return datatypeFactory.newXMLGregorianCalendar(lexicalXSDTime).toGregorianCalendar()
		End Function

		Public Function printTime(ByVal val As DateTime) As String Implements DatatypeConverterInterface.printTime
			Return CalendarFormatter.doFormat("%h:%m:%s%z", val)
		End Function

		Public Function parseDate(ByVal lexicalXSDDate As String) As DateTime Implements DatatypeConverterInterface.parseDate
			Return datatypeFactory.newXMLGregorianCalendar(lexicalXSDDate).toGregorianCalendar()
		End Function

		Public Function printDate(ByVal val As DateTime) As String Implements DatatypeConverterInterface.printDate
			Return _printDate(val)
		End Function

		Public Shared Function _printDate(ByVal val As DateTime) As String
			Return CalendarFormatter.doFormat(((New StringBuilder("%Y-%M-%D")).Append("%z")).ToString(),val)
		End Function

		Public Function parseAnySimpleType(ByVal lexicalXSDAnySimpleType As String) As String Implements DatatypeConverterInterface.parseAnySimpleType
			Return lexicalXSDAnySimpleType
	'        return (String)SimpleURType.theInstance._createValue( lexicalXSDAnySimpleType, null );
		End Function

		Public Function printString(ByVal val As String) As String Implements DatatypeConverterInterface.printString
	'        return StringType.theInstance.convertToLexicalValue( val, null );
			Return val
		End Function

		Public Function printInt(ByVal val As Integer) As String Implements DatatypeConverterInterface.printInt
			Return _printInt(val)
		End Function

		Public Shared Function _printInt(ByVal val As Integer) As String
			Return Convert.ToString(val)
		End Function

		Public Function printLong(ByVal val As Long) As String Implements DatatypeConverterInterface.printLong
			Return _printLong(val)
		End Function

		Public Shared Function _printLong(ByVal val As Long) As String
			Return Convert.ToString(val)
		End Function

		Public Function printDecimal(ByVal val As Decimal) As String Implements DatatypeConverterInterface.printDecimal
			Return _printDecimal(val)
		End Function

		Public Shared Function _printDecimal(ByVal val As Decimal) As String
			Return val.toPlainString()
		End Function

		Public Function printDouble(ByVal v As Double) As String Implements DatatypeConverterInterface.printDouble
			Return _printDouble(v)
		End Function

		Public Shared Function _printDouble(ByVal v As Double) As String
			If Double.IsNaN(v) Then Return "NaN"
			If v = Double.PositiveInfinity Then Return "INF"
			If v = Double.NegativeInfinity Then Return "-INF"
			Return Convert.ToString(v)
		End Function

		Public Function printQName(ByVal val As javax.xml.namespace.QName, ByVal nsc As javax.xml.namespace.NamespaceContext) As String Implements DatatypeConverterInterface.printQName
			Return _printQName(val, nsc)
		End Function

		Public Shared Function _printQName(ByVal val As javax.xml.namespace.QName, ByVal nsc As javax.xml.namespace.NamespaceContext) As String
			' Double-check
			Dim qname As String
			Dim prefix As String = nsc.getPrefix(val.namespaceURI)
			Dim localPart As String = val.localPart

			If prefix Is Nothing OrElse prefix.Length = 0 Then ' be defensive
				qname = localPart
			Else
				qname = prefix + AscW(":"c) + localPart
			End If

			Return qname
		End Function

		Public Function printBase64Binary(ByVal val As SByte()) As String Implements DatatypeConverterInterface.printBase64Binary
			Return _printBase64Binary(val)
		End Function

		Public Function printUnsignedShort(ByVal val As Integer) As String Implements DatatypeConverterInterface.printUnsignedShort
			Return Convert.ToString(val)
		End Function

		Public Function printAnySimpleType(ByVal val As String) As String Implements DatatypeConverterInterface.printAnySimpleType
			Return val
		End Function

		''' <summary>
		''' Just return the string passed as a parameter but
		''' installs an instance of this class as the DatatypeConverter
		''' implementation. Used from static fixed value initializers.
		''' </summary>
		Public Shared Function installHook(ByVal s As String) As String
			DatatypeConverter.datatypeConverter = theInstance
			Return s
		End Function
	' base64 decoder
		Private Shared ReadOnly decodeMap As SByte() = initDecodeMap()
		Private Const PADDING As SByte = 127

		Private Shared Function initDecodeMap() As SByte()
			Dim map As SByte() = New SByte(127){}
			Dim i As Integer
			For i = 0 To 127
				map(i) = -1
			Next i

			For i = AscW("A"c) To AscW("Z"c)
				map(i) = CByte(i - AscW("A"c))
			Next i
			For i = AscW("a"c) To AscW("z"c)
				map(i) = CByte(i - AscW("a"c) + 26)
			Next i
			For i = AscW("0"c) To AscW("9"c)
				map(i) = CByte(i - AscW("0"c) + 52)
			Next i
			map(AscW("+"c)) = 62
			map(AscW("/"c)) = 63
			map(AscW("="c)) = PADDING

			Return map
		End Function

		''' <summary>
		''' computes the length of binary data speculatively.
		''' 
		''' <p>
		''' Our requirement is to create byte[] of the exact length to store the binary data.
		''' If we do this in a straight-forward way, it takes two passes over the data.
		''' Experiments show that this is a non-trivial overhead (35% or so is spent on
		''' the first pass in calculating the length.)
		''' 
		''' <p>
		''' So the approach here is that we compute the length speculatively, without looking
		''' at the whole contents. The obtained speculative value is never less than the
		''' actual length of the binary data, but it may be bigger. So if the speculation
		''' goes wrong, we'll pay the cost of reallocation and buffer copying.
		''' 
		''' <p>
		''' If the base64 text is tightly packed with no indentation nor illegal char
		''' (like what most web services produce), then the speculation of this method
		''' will be correct, so we get the performance benefit.
		''' </summary>
		Private Shared Function guessLength(ByVal text As String) As Integer
			Dim len As Integer = text.Length

			' compute the tail '=' chars
			Dim j As Integer = len - 1
			Do While j >= 0
				Dim code As SByte = decodeMap(AscW(text.Chars(j)))
				If code = PADDING Then
					j -= 1
					Continue Do
				End If
				If code = -1 Then ' most likely this base64 text is indented. go with the upper bound Return text.Length \ 4 * 3
				Exit Do
				j -= 1
			Loop

			j += 1 ' text.charAt(j) is now at some base64 char, so +1 to make it the size
			Dim padSize As Integer = len - j
			If padSize > 2 Then ' something is wrong with base64. be safe and go with the upper bound Return text.Length \ 4 * 3

			' so far this base64 looks like it's unindented tightly packed base64.
			' take a chance and create an array with the expected size
			Return text.Length \ 4 * 3 - padSize
		End Function

		''' <param name="text">
		'''      base64Binary data is likely to be long, and decoding requires
		'''      each character to be accessed twice (once for counting length, another
		'''      for decoding.)
		''' 
		'''      A benchmark showed that taking <seealso cref="String"/> is faster, presumably
		'''      because JIT can inline a lot of string access (with data of 1K chars, it was twice as fast) </param>
		Public Shared Function _parseBase64Binary(ByVal text As String) As SByte()
			Dim buflen As Integer = guessLength(text)
			Dim out As SByte() = New SByte(buflen - 1){}
			Dim o As Integer = 0

			Dim len As Integer = text.Length
			Dim i As Integer

			Dim quadruplet As SByte() = New SByte(3){}
			Dim q As Integer = 0

			' convert each quadruplet to three bytes.
			For i = 0 To len - 1
				Dim ch As Char = text.Chars(i)
				Dim v As SByte = decodeMap(AscW(ch))

				If v <> -1 Then
					quadruplet(q) = v
					q += 1
				End If

				If q = 4 Then
					' quadruplet is now filled.
					out(o) = CByte((quadruplet(0) << 2) Or (quadruplet(1) >> 4))
					o += 1
					If quadruplet(2) <> PADDING Then
						out(o) = CByte((quadruplet(1) << 4) Or (quadruplet(2) >> 2))
						o += 1
					End If
					If quadruplet(3) <> PADDING Then
						out(o) = CByte((quadruplet(2) << 6) Or (quadruplet(3)))
						o += 1
					End If
					q = 0
				End If
			Next i

			If buflen = o Then ' speculation worked out to be OK Return out

			' we overestimated, so need to create a new buffer
			Dim nb As SByte() = New SByte(o - 1){}
			Array.Copy(out, 0, nb, 0, o)
			Return nb
		End Function
		Private Shared ReadOnly encodeMap As Char() = initEncodeMap()

		Private Shared Function initEncodeMap() As Char()
			Dim map As Char() = New Char(63){}
			Dim i As Integer
			For i = 0 To 25
				map(i) = ChrW(AscW("A"c) + i)
			Next i
			For i = 26 To 51
				map(i) = ChrW(AscW("a"c) + (i - 26))
			Next i
			For i = 52 To 61
				map(i) = ChrW(AscW("0"c) + (i - 52))
			Next i
			map(62) = "+"c
			map(63) = "/"c

			Return map
		End Function

		Public Shared Function encode(ByVal i As Integer) As Char
			Return encodeMap(i And &H3F)
		End Function

		Public Shared Function encodeByte(ByVal i As Integer) As SByte
			Return AscW(encodeMap(i And &H3F))
		End Function

		Public Shared Function _printBase64Binary(ByVal input As SByte()) As String
			Return _printBase64Binary(input, 0, input.Length)
		End Function

		Public Shared Function _printBase64Binary(ByVal input As SByte(), ByVal offset As Integer, ByVal len As Integer) As String
			Dim buf As Char() = New Char(((len + 2) \ 3) * 4 - 1){}
			Dim ptr As Integer = _printBase64Binary(input, offset, len, buf, 0)
			Debug.Assert(ptr = buf.Length)
			Return New String(buf)
		End Function

		''' <summary>
		''' Encodes a byte array into a char array by doing base64 encoding.
		''' 
		''' The caller must supply a big enough buffer.
		''' 
		''' @return
		'''      the value of {@code ptr+((len+2)/3)*4}, which is the new offset
		'''      in the output buffer where the further bytes should be placed.
		''' </summary>
		Public Shared Function _printBase64Binary(ByVal input As SByte(), ByVal offset As Integer, ByVal len As Integer, ByVal buf As Char(), ByVal ptr As Integer) As Integer
			' encode elements until only 1 or 2 elements are left to encode
			Dim remaining As Integer = len
			Dim i As Integer
			i = offset
			Do While remaining >= 3
				buf(ptr) = encode(input(i) >> 2)
				ptr += 1
				buf(ptr) = encode(((input(i) And &H3) << 4) Or ((input(i + 1) >> 4) And &HF))
				ptr += 1
				buf(ptr) = encode(((input(i + 1) And &HF) << 2) Or ((input(i + 2) >> 6) And &H3))
				ptr += 1
				buf(ptr) = encode(input(i + 2) And &H3F)
				ptr += 1
				remaining -= 3
				i += 3
			Loop
			' encode when exactly 1 element (left) to encode
			If remaining = 1 Then
				buf(ptr) = encode(input(i) >> 2)
				ptr += 1
				buf(ptr) = encode(((input(i)) And &H3) << 4)
				ptr += 1
				buf(ptr) = "="c
				ptr += 1
				buf(ptr) = "="c
				ptr += 1
			End If
			' encode when exactly 2 elements (left) to encode
			If remaining = 2 Then
				buf(ptr) = encode(input(i) >> 2)
				ptr += 1
				buf(ptr) = encode(((input(i) And &H3) << 4) Or ((input(i + 1) >> 4) And &HF))
				ptr += 1
				buf(ptr) = encode((input(i + 1) And &HF) << 2)
				ptr += 1
				buf(ptr) = "="c
				ptr += 1
			End If
			Return ptr
		End Function

		''' <summary>
		''' Encodes a byte array into another byte array by first doing base64 encoding
		''' then encoding the result in ASCII.
		''' 
		''' The caller must supply a big enough buffer.
		''' 
		''' @return
		'''      the value of {@code ptr+((len+2)/3)*4}, which is the new offset
		'''      in the output buffer where the further bytes should be placed.
		''' </summary>
		Public Shared Function _printBase64Binary(ByVal input As SByte(), ByVal offset As Integer, ByVal len As Integer, ByVal out As SByte(), ByVal ptr As Integer) As Integer
			Dim buf As SByte() = out
			Dim remaining As Integer = len
			Dim i As Integer
			i=offset
			Do While remaining >= 3
				buf(ptr) = encodeByte(input(i)>>2)
				ptr += 1
				buf(ptr) = encodeByte(((input(i) And &H3)<<4) Or ((input(i+1)>>4) And &HF))
				ptr += 1
				buf(ptr) = encodeByte(((input(i+1) And &HF)<<2) Or ((input(i+2)>>6) And &H3))
				ptr += 1
				buf(ptr) = encodeByte(input(i+2) And &H3F)
				ptr += 1
				remaining -= 3
				i += 3
			Loop
			' encode when exactly 1 element (left) to encode
			If remaining = 1 Then
				buf(ptr) = encodeByte(input(i)>>2)
				ptr += 1
				buf(ptr) = encodeByte(((input(i)) And &H3)<<4)
				ptr += 1
				buf(ptr) = AscW("="c)
				ptr += 1
				buf(ptr) = AscW("="c)
				ptr += 1
			End If
			' encode when exactly 2 elements (left) to encode
			If remaining = 2 Then
				buf(ptr) = encodeByte(input(i)>>2)
				ptr += 1
				buf(ptr) = encodeByte(((input(i) And &H3)<<4) Or ((input(i+1)>>4) And &HF))
				ptr += 1
				buf(ptr) = encodeByte((input(i+1) And &HF)<<2)
				ptr += 1
				buf(ptr) = AscW("="c)
				ptr += 1
			End If

			Return ptr
		End Function

		Private Shared Function removeOptionalPlus(ByVal s As CharSequence) As CharSequence
			Dim len As Integer = s.length()

			If len <= 1 OrElse s.Chars(0) <> "+"c Then Return s

			s = s.subSequence(1, len)
			Dim ch As Char = s.Chars(0)
			If "0"c <= ch AndAlso ch <= "9"c Then Return s
			If "."c = ch Then Return s

			Throw New NumberFormatException
		End Function

		Private Shared Function isDigitOrPeriodOrSign(ByVal ch As Char) As Boolean
			If "0"c <= ch AndAlso ch <= "9"c Then Return True
			If ch = "+"c OrElse ch = "-"c OrElse ch = "."c Then Return True
			Return False
		End Function
		Private Shared ReadOnly datatypeFactory As javax.xml.datatype.DatatypeFactory

		Shared Sub New()
			Try
				datatypeFactory = javax.xml.datatype.DatatypeFactory.newInstance()
			Catch e As javax.xml.datatype.DatatypeConfigurationException
				Throw New Exception(e)
			End Try
		End Sub

		Private NotInheritable Class CalendarFormatter

			Public Shared Function doFormat(ByVal format As String, ByVal cal As DateTime) As String
				Dim fidx As Integer = 0
				Dim flen As Integer = format.Length
				Dim buf As New StringBuilder

				Do While fidx < flen
					Dim fch As Char = format.Chars(fidx)
					fidx += 1

					If fch <> "%"c Then ' not a meta character
						buf.Append(fch)
						Continue Do
					End If

					' seen meta character. we don't do error check against the format
					Dim tempVar As Char = format.Chars(fidx)
					fidx += 1
					Select Case tempVar
						Case "Y"c ' year
							formatYear(cal, buf)

						Case "M"c ' month
							formatMonth(cal, buf)

						Case "D"c ' days
							formatDays(cal, buf)

						Case "h"c ' hours
							formatHours(cal, buf)

						Case "m"c ' minutes
							formatMinutes(cal, buf)

						Case "s"c ' parse seconds.
							formatSeconds(cal, buf)

						Case "z"c ' time zone
							formatTimeZone(cal, buf)

						Case Else
							' illegal meta character. impossible.
							Throw New InternalError
					End Select
				Loop

				Return buf.ToString()
			End Function

			Private Shared Sub formatYear(ByVal cal As DateTime, ByVal buf As StringBuilder)
				Dim year As Integer = cal.Year

				Dim s As String
				If year <= 0 Then ' negative value
					s = Convert.ToString(1 - year) ' positive value
				Else
					s = Convert.ToString(year)
				End If

				Do While s.Length < 4
					s = AscW("0"c) + s
				Loop
				If year <= 0 Then s = AscW("-"c) + s

				buf.Append(s)
			End Sub

			Private Shared Sub formatMonth(ByVal cal As DateTime, ByVal buf As StringBuilder)
				formatTwoDigits(cal.Month + 1, buf)
			End Sub

			Private Shared Sub formatDays(ByVal cal As DateTime, ByVal buf As StringBuilder)
				formatTwoDigits(cal.Day, buf)
			End Sub

			Private Shared Sub formatHours(ByVal cal As DateTime, ByVal buf As StringBuilder)
				formatTwoDigits(cal.get(DateTime.HOUR_OF_DAY), buf)
			End Sub

			Private Shared Sub formatMinutes(ByVal cal As DateTime, ByVal buf As StringBuilder)
				formatTwoDigits(cal.Minute, buf)
			End Sub

			Private Shared Sub formatSeconds(ByVal cal As DateTime, ByVal buf As StringBuilder)
				formatTwoDigits(cal.Second, buf)
				If cal.isSet(DateTime.MILLISECOND) Then ' milliseconds
					Dim n As Integer = cal.Millisecond
					If n <> 0 Then
						Dim ms As String = Convert.ToString(n)
						Do While ms.Length < 3
							ms = AscW("0"c) + ms ' left 0 paddings.
						Loop
						buf.Append("."c)
						buf.Append(ms)
					End If
				End If
			End Sub

			''' <summary>
			''' formats time zone specifier. </summary>
			Private Shared Sub formatTimeZone(ByVal cal As DateTime, ByVal buf As StringBuilder)
				Dim tz As java.util.TimeZone = cal.timeZone

				If tz Is Nothing Then Return

				' otherwise print out normally.
				Dim offset As Integer = tz.getOffset(cal.time)

				If offset = 0 Then
					buf.Append("Z"c)
					Return
				End If

				If offset >= 0 Then
					buf.Append("+"c)
				Else
					buf.Append("-"c)
					offset *= -1
				End If

				offset \= 60 * 1000 ' offset is in milli-seconds

				formatTwoDigits(offset \ 60, buf)
				buf.Append(":"c)
				formatTwoDigits(offset Mod 60, buf)
			End Sub

			''' <summary>
			''' formats Integer into two-character-wide string. </summary>
			Private Shared Sub formatTwoDigits(ByVal n As Integer, ByVal buf As StringBuilder)
				' n is always non-negative.
				If n < 10 Then buf.Append("0"c)
				buf.Append(n)
			End Sub
		End Class
	End Class

End Namespace
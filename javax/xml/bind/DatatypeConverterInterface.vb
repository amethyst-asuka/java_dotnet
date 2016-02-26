Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>
	''' The DatatypeConverterInterface is for JAXB provider use only. A
	''' JAXB provider must supply a class that implements this interface.
	''' JAXB Providers are required to call the
	''' {@link DatatypeConverter#setDatatypeConverter(DatatypeConverterInterface)
	''' DatatypeConverter.setDatatypeConverter} api at
	''' some point before the first marshal or unmarshal operation (perhaps during
	''' the call to JAXBContext.newInstance).  This step is necessary to configure
	''' the converter that should be used to perform the print and parse
	''' functionality.  Calling this api repeatedly will have no effect - the
	''' DatatypeConverter instance passed into the first invocation is the one that
	''' will be used from then on.
	''' </p>
	''' 
	''' <p>
	''' This interface defines the parse and print methods. There is one
	''' parse and print method for each XML schema datatype specified in the
	''' the default binding Table 5-1 in the JAXB specification.
	''' </p>
	''' 
	''' <p>
	''' The parse and print methods defined here are invoked by the static parse
	''' and print methods defined in the <seealso cref="DatatypeConverter DatatypeConverter"/>
	''' class.
	''' </p>
	''' 
	''' <p>
	''' A parse method for a XML schema datatype must be capable of converting any
	''' lexical representation of the XML schema datatype ( specified by the
	''' <a href="http://www.w3.org/TR/xmlschema-2/"> XML Schema Part2: Datatypes
	''' specification</a> into a value in the value space of the XML schema datatype.
	''' If an error is encountered during conversion, then an IllegalArgumentException
	''' or a subclass of IllegalArgumentException must be thrown by the method.
	''' 
	''' </p>
	''' 
	''' <p>
	''' A print method for a XML schema datatype can output any lexical
	''' representation that is valid with respect to the XML schema datatype.
	''' If an error is encountered during conversion, then an IllegalArgumentException,
	''' or a subclass of IllegalArgumentException must be thrown by the method.
	''' </p>
	''' 
	''' The prefix xsd: is used to refer to XML schema datatypes
	''' <a href="http://www.w3.org/TR/xmlschema-2/"> XML Schema Part2: Datatypes
	''' specification.</a>
	''' 
	''' <p>
	''' @author <ul><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Ryan Shoemaker,Sun Microsystems Inc.</li></ul> </summary>
	''' <seealso cref= DatatypeConverter </seealso>
	''' <seealso cref= ParseConversionEvent </seealso>
	''' <seealso cref= PrintConversionEvent
	''' @since JAXB1.0 </seealso>

	Public Interface DatatypeConverterInterface
		''' <summary>
		''' <p>
		''' Convert the string argument into a string. </summary>
		''' <param name="lexicalXSDString">
		'''     A lexical representation of the XML Schema datatype xsd:string
		''' @return
		'''     A string that is the same as the input string. </param>
		Function parseString(ByVal lexicalXSDString As String) As String

		''' <summary>
		''' <p>
		''' Convert the string argument into a BigInteger value. </summary>
		''' <param name="lexicalXSDInteger">
		'''     A string containing a lexical representation of
		'''     xsd:integer.
		''' @return
		'''     A BigInteger value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDInteger</code> is not a valid string representation of a <seealso cref="java.math.BigInteger"/> value. </exception>
		Function parseInteger(ByVal lexicalXSDInteger As String) As System.Numerics.BigInteger

		''' <summary>
		''' <p>
		''' Convert the string argument into an int value. </summary>
		''' <param name="lexicalXSDInt">
		'''     A string containing a lexical representation of
		'''     xsd:int.
		''' @return
		'''     An int value represented byte the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDInt</code> is not a valid string representation of an <code>int</code> value. </exception>
		Function parseInt(ByVal lexicalXSDInt As String) As Integer

		''' <summary>
		''' <p>
		''' Converts the string argument into a long value. </summary>
		''' <param name="lexicalXSDLong">
		'''     A string containing lexical representation of
		'''     xsd:long.
		''' @return
		'''     A long value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDLong</code> is not a valid string representation of a <code>long</code> value. </exception>
		Function parseLong(ByVal lexicalXSDLong As String) As Long

		''' <summary>
		''' <p>
		''' Converts the string argument into a short value. </summary>
		''' <param name="lexicalXSDShort">
		'''     A string containing lexical representation of
		'''     xsd:short.
		''' @return
		'''     A short value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDShort</code> is not a valid string representation of a <code>short</code> value. </exception>
		Function parseShort(ByVal lexicalXSDShort As String) As Short

		''' <summary>
		''' <p>
		''' Converts the string argument into a BigDecimal value. </summary>
		''' <param name="lexicalXSDDecimal">
		'''     A string containing lexical representation of
		'''     xsd:decimal.
		''' @return
		'''     A BigDecimal value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDDecimal</code> is not a valid string representation of <seealso cref="java.math.BigDecimal"/>. </exception>
		Function parseDecimal(ByVal lexicalXSDDecimal As String) As Decimal

		''' <summary>
		''' <p>
		''' Converts the string argument into a float value. </summary>
		''' <param name="lexicalXSDFloat">
		'''     A string containing lexical representation of
		'''     xsd:float.
		''' @return
		'''     A float value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDFloat</code> is not a valid string representation of a <code>float</code> value. </exception>
		Function parseFloat(ByVal lexicalXSDFloat As String) As Single

		''' <summary>
		''' <p>
		''' Converts the string argument into a double value. </summary>
		''' <param name="lexicalXSDDouble">
		'''     A string containing lexical representation of
		'''     xsd:double.
		''' @return
		'''     A double value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDDouble</code> is not a valid string representation of a <code>double</code> value. </exception>
		Function parseDouble(ByVal lexicalXSDDouble As String) As Double

		''' <summary>
		''' <p>
		''' Converts the string argument into a boolean value. </summary>
		''' <param name="lexicalXSDBoolean">
		'''     A string containing lexical representation of
		'''     xsd:boolean.
		''' @return
		'''     A boolean value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:boolean. </exception>
		Function parseBoolean(ByVal lexicalXSDBoolean As String) As Boolean

		''' <summary>
		''' <p>
		''' Converts the string argument into a byte value. </summary>
		''' <param name="lexicalXSDByte">
		'''     A string containing lexical representation of
		'''     xsd:byte.
		''' @return
		'''     A byte value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDByte</code> does not contain a parseable byte. </exception>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:byte. </exception>
		Function parseByte(ByVal lexicalXSDByte As String) As SByte

		''' <summary>
		''' <p>
		''' Converts the string argument into a QName value.
		''' 
		''' <p>
		''' String parameter <tt>lexicalXSDQname</tt> must conform to lexical value space specifed at
		''' <a href="http://www.w3.org/TR/xmlschema-2/#QName">XML Schema Part 2:Datatypes specification:QNames</a>
		''' </summary>
		''' <param name="lexicalXSDQName">
		'''     A string containing lexical representation of xsd:QName. </param>
		''' <param name="nsc">
		'''     A namespace context for interpreting a prefix within a QName.
		''' @return
		'''     A QName value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException">  if string parameter does not conform to XML Schema Part 2 specification or
		'''      if namespace prefix of <tt>lexicalXSDQname</tt> is not bound to a URI in NamespaceContext <tt>nsc</tt>. </exception>
		Function parseQName(ByVal lexicalXSDQName As String, ByVal nsc As javax.xml.namespace.NamespaceContext) As javax.xml.namespace.QName

		''' <summary>
		''' <p>
		''' Converts the string argument into a Calendar value. </summary>
		''' <param name="lexicalXSDDateTime">
		'''     A string containing lexical representation of
		'''     xsd:datetime.
		''' @return
		'''     A Calendar object represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:dateTime. </exception>
		Function parseDateTime(ByVal lexicalXSDDateTime As String) As DateTime

		''' <summary>
		''' <p>
		''' Converts the string argument into an array of bytes. </summary>
		''' <param name="lexicalXSDBase64Binary">
		'''     A string containing lexical representation
		'''     of xsd:base64Binary.
		''' @return
		'''     An array of bytes represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:base64Binary </exception>
		Function parseBase64Binary(ByVal lexicalXSDBase64Binary As String) As SByte()

		''' <summary>
		''' <p>
		''' Converts the string argument into an array of bytes. </summary>
		''' <param name="lexicalXSDHexBinary">
		'''     A string containing lexical representation of
		'''     xsd:hexBinary.
		''' @return
		'''     An array of bytes represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:hexBinary. </exception>
		Function parseHexBinary(ByVal lexicalXSDHexBinary As String) As SByte()

		''' <summary>
		''' <p>
		''' Converts the string argument into a long value. </summary>
		''' <param name="lexicalXSDUnsignedInt">
		'''     A string containing lexical representation
		'''     of xsd:unsignedInt.
		''' @return
		'''     A long value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> if string parameter can not be parsed into a <tt>long</tt> value. </exception>
		Function parseUnsignedInt(ByVal lexicalXSDUnsignedInt As String) As Long

		''' <summary>
		''' <p>
		''' Converts the string argument into an int value. </summary>
		''' <param name="lexicalXSDUnsignedShort">
		'''     A string containing lexical
		'''     representation of xsd:unsignedShort.
		''' @return
		'''     An int value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> if string parameter can not be parsed into an <tt>int</tt> value. </exception>
		Function parseUnsignedShort(ByVal lexicalXSDUnsignedShort As String) As Integer

		''' <summary>
		''' <p>
		''' Converts the string argument into a Calendar value. </summary>
		''' <param name="lexicalXSDTime">
		'''     A string containing lexical representation of
		'''     xsd:Time.
		''' @return
		'''     A Calendar value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:Time. </exception>
		Function parseTime(ByVal lexicalXSDTime As String) As DateTime

		''' <summary>
		''' <p>
		''' Converts the string argument into a Calendar value. </summary>
		''' <param name="lexicalXSDDate">
		'''     A string containing lexical representation of
		'''     xsd:Date.
		''' @return
		'''     A Calendar value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:Date. </exception>
		Function parseDate(ByVal lexicalXSDDate As String) As DateTime

		''' <summary>
		''' <p>
		''' Return a string containing the lexical representation of the
		''' simple type. </summary>
		''' <param name="lexicalXSDAnySimpleType">
		'''     A string containing lexical
		'''     representation of the simple type.
		''' @return
		'''     A string containing the lexical representation of the
		'''     simple type. </param>
		Function parseAnySimpleType(ByVal lexicalXSDAnySimpleType As String) As String

		''' <summary>
		''' <p>
		''' Converts the string argument into a string. </summary>
		''' <param name="val">
		'''     A string value.
		''' @return
		'''     A string containing a lexical representation of xsd:string </param>
		Function printString(ByVal val As String) As String

		''' <summary>
		''' <p>
		''' Converts a BigInteger value into a string. </summary>
		''' <param name="val">
		'''     A BigInteger value
		''' @return
		'''     A string containing a lexical representation of xsd:integer </param>
		''' <exception cref="IllegalArgumentException"> <tt>val</tt> is null. </exception>
		Function printInteger(ByVal val As System.Numerics.BigInteger) As String

		''' <summary>
		''' <p>
		''' Converts an int value into a string. </summary>
		''' <param name="val">
		'''     An int value
		''' @return
		'''     A string containing a lexical representation of xsd:int </param>
		Function printInt(ByVal val As Integer) As String


		''' <summary>
		''' <p>
		''' Converts a long value into a string. </summary>
		''' <param name="val">
		'''     A long value
		''' @return
		'''     A string containing a lexical representation of xsd:long </param>
		Function printLong(ByVal val As Long) As String

		''' <summary>
		''' <p>
		''' Converts a short value into a string. </summary>
		''' <param name="val">
		'''     A short value
		''' @return
		'''     A string containing a lexical representation of xsd:short </param>
		Function printShort(ByVal val As Short) As String

		''' <summary>
		''' <p>
		''' Converts a BigDecimal value into a string. </summary>
		''' <param name="val">
		'''     A BigDecimal value
		''' @return
		'''     A string containing a lexical representation of xsd:decimal </param>
		''' <exception cref="IllegalArgumentException"> <tt>val</tt> is null. </exception>
		Function printDecimal(ByVal val As Decimal) As String

		''' <summary>
		''' <p>
		''' Converts a float value into a string. </summary>
		''' <param name="val">
		'''     A float value
		''' @return
		'''     A string containing a lexical representation of xsd:float </param>
		Function printFloat(ByVal val As Single) As String

		''' <summary>
		''' <p>
		''' Converts a double value into a string. </summary>
		''' <param name="val">
		'''     A double value
		''' @return
		'''     A string containing a lexical representation of xsd:double </param>
		Function printDouble(ByVal val As Double) As String

		''' <summary>
		''' <p>
		''' Converts a boolean value into a string. </summary>
		''' <param name="val">
		'''     A boolean value
		''' @return
		'''     A string containing a lexical representation of xsd:boolean </param>
		Function printBoolean(ByVal val As Boolean) As String

		''' <summary>
		''' <p>
		''' Converts a byte value into a string. </summary>
		''' <param name="val">
		'''     A byte value
		''' @return
		'''     A string containing a lexical representation of xsd:byte </param>
		Function printByte(ByVal val As SByte) As String

		''' <summary>
		''' <p>
		''' Converts a QName instance into a string. </summary>
		''' <param name="val">
		'''     A QName value </param>
		''' <param name="nsc">
		'''     A namespace context for interpreting a prefix within a QName.
		''' @return
		'''     A string containing a lexical representation of QName </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null or
		''' if <tt>nsc</tt> is non-null or <tt>nsc.getPrefix(nsprefixFromVal)</tt> is null. </exception>
		Function printQName(ByVal val As javax.xml.namespace.QName, ByVal nsc As javax.xml.namespace.NamespaceContext) As String

		''' <summary>
		''' <p>
		''' Converts a Calendar value into a string. </summary>
		''' <param name="val">
		'''     A Calendar value
		''' @return
		'''     A string containing a lexical representation of xsd:dateTime </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Function printDateTime(ByVal val As DateTime) As String

		''' <summary>
		''' <p>
		''' Converts an array of bytes into a string. </summary>
		''' <param name="val">
		'''     an array of bytes
		''' @return
		'''     A string containing a lexical representation of xsd:base64Binary </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Function printBase64Binary(ByVal val As SByte()) As String

		''' <summary>
		''' <p>
		''' Converts an array of bytes into a string. </summary>
		''' <param name="val">
		'''     an array of bytes
		''' @return
		'''     A string containing a lexical representation of xsd:hexBinary </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Function printHexBinary(ByVal val As SByte()) As String

		''' <summary>
		''' <p>
		''' Converts a long value into a string. </summary>
		''' <param name="val">
		'''     A long value
		''' @return
		'''     A string containing a lexical representation of xsd:unsignedInt </param>
		Function printUnsignedInt(ByVal val As Long) As String

		''' <summary>
		''' <p>
		''' Converts an int value into a string. </summary>
		''' <param name="val">
		'''     An int value
		''' @return
		'''     A string containing a lexical representation of xsd:unsignedShort </param>
		Function printUnsignedShort(ByVal val As Integer) As String

		''' <summary>
		''' <p>
		''' Converts a Calendar value into a string. </summary>
		''' <param name="val">
		'''     A Calendar value
		''' @return
		'''     A string containing a lexical representation of xsd:time </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Function printTime(ByVal val As DateTime) As String

		''' <summary>
		''' <p>
		''' Converts a Calendar value into a string. </summary>
		''' <param name="val">
		'''     A Calendar value
		''' @return
		'''     A string containing a lexical representation of xsd:date </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Function printDate(ByVal val As DateTime) As String

		''' <summary>
		''' <p>
		''' Converts a string value into a string. </summary>
		''' <param name="val">
		'''     A string value
		''' @return
		'''     A string containing a lexical representation of xsd:AnySimpleType </param>
		Function printAnySimpleType(ByVal val As String) As String
	End Interface

End Namespace
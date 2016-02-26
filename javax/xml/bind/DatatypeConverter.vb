Imports System
Imports System.Runtime.CompilerServices

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
	''' The javaType binding declaration can be used to customize the binding of
	''' an XML schema datatype to a Java datatype. Customizations can involve
	''' writing a parse and print method for parsing and printing lexical
	''' representations of a XML schema datatype respectively. However, writing
	''' parse and print methods requires knowledge of the lexical representations (
	''' <a href="http://www.w3.org/TR/xmlschema-2/"> XML Schema Part2: Datatypes
	''' specification </a>) and hence may be difficult to write.
	''' </p>
	''' <p>
	''' This class makes it easier to write parse and print methods. It defines
	''' static parse and print methods that provide access to a JAXB provider's
	''' implementation of parse and print methods. These methods are invoked by
	''' custom parse and print methods. For example, the binding of xsd:dateTime
	''' to a long can be customized using parse and print methods as follows:
	''' <blockquote>
	'''    <pre>
	'''    // Customized parse method
	'''    public long myParseCal( String dateTimeString ) {
	'''        java.util.Calendar cal = DatatypeConverter.parseDateTime(dateTimeString);
	'''        long longval = convert_calendar_to_long(cal); //application specific
	'''        return longval;
	'''    }
	''' 
	'''    // Customized print method
	'''    public String myPrintCal( Long longval ) {
	'''        java.util.Calendar cal = convert_long_to_calendar(longval) ; //application specific
	'''        String dateTimeString = DatatypeConverter.printDateTime(cal);
	'''        return dateTimeString;
	'''    }
	'''    </pre>
	''' </blockquote>
	''' <p>
	''' There is a static parse and print method corresponding to each parse and
	''' print method respectively in the {@link DatatypeConverterInterface
	''' DatatypeConverterInterface}.
	''' <p>
	''' The static methods defined in the class can also be used to specify
	''' a parse or a print method in a javaType binding declaration.
	''' </p>
	''' <p>
	''' JAXB Providers are required to call the
	''' {@link #setDatatypeConverter(DatatypeConverterInterface)
	''' setDatatypeConverter} api at some point before the first marshal or unmarshal
	''' operation (perhaps during the call to JAXBContext.newInstance).  This step is
	''' necessary to configure the converter that should be used to perform the
	''' print and parse functionality.
	''' </p>
	''' 
	''' <p>
	''' A print method for a XML schema datatype can output any lexical
	''' representation that is valid with respect to the XML schema datatype.
	''' If an error is encountered during conversion, then an IllegalArgumentException,
	''' or a subclass of IllegalArgumentException must be thrown by the method.
	''' </p>
	''' 
	''' @author <ul><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Ryan Shoemaker,Sun Microsystems Inc.</li></ul> </summary>
	''' <seealso cref= DatatypeConverterInterface </seealso>
	''' <seealso cref= ParseConversionEvent </seealso>
	''' <seealso cref= PrintConversionEvent
	''' @since JAXB1.0 </seealso>

	Public NotInheritable Class DatatypeConverter

		' delegate to this instance of DatatypeConverter
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared theConverter As DatatypeConverterInterface = Nothing

		Private Shared ReadOnly SET_DATATYPE_CONVERTER_PERMISSION As New JAXBPermission("setDatatypeConverter")

		Private Sub New()
			' private constructor
		End Sub

		''' <summary>
		''' This method is for JAXB provider use only.
		''' <p>
		''' JAXB Providers are required to call this method at some point before
		''' allowing any of the JAXB client marshal or unmarshal operations to
		''' occur.  This is necessary to configure the datatype converter that
		''' should be used to perform the print and parse conversions.
		''' 
		''' <p>
		''' Calling this api repeatedly will have no effect - the
		''' DatatypeConverterInterface instance passed into the first invocation is
		''' the one that will be used from then on.
		''' </summary>
		''' <param name="converter"> an instance of a class that implements the
		''' DatatypeConverterInterface class - this parameter must not be null. </param>
		''' <exception cref="IllegalArgumentException"> if the parameter is null </exception>
		''' <exception cref="SecurityException">
		'''      If the <seealso cref="SecurityManager"/> in charge denies the access to
		'''      set the datatype converter. </exception>
		''' <seealso cref= JAXBPermission </seealso>
		Public Shared Property datatypeConverter As DatatypeConverterInterface
			Set(ByVal converter As DatatypeConverterInterface)
				If converter Is Nothing Then
					Throw New System.ArgumentException(Messages.format(Messages.CONVERTER_MUST_NOT_BE_NULL))
				ElseIf theConverter Is Nothing Then
					Dim sm As SecurityManager = System.securityManager
					If sm IsNot Nothing Then sm.checkPermission(SET_DATATYPE_CONVERTER_PERMISSION)
					theConverter = converter
				End If
			End Set
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub initConverter()
			theConverter = New DatatypeConverterImpl
		End Sub

		''' <summary>
		''' <p>
		''' Convert the lexical XSD string argument into a String value. </summary>
		''' <param name="lexicalXSDString">
		'''     A string containing a lexical representation of
		'''     xsd:string.
		''' @return
		'''     A String value represented by the string argument. </param>
		Public Shared Function parseString(ByVal lexicalXSDString As String) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseString(lexicalXSDString)
		End Function

		''' <summary>
		''' <p>
		''' Convert the string argument into a BigInteger value. </summary>
		''' <param name="lexicalXSDInteger">
		'''     A string containing a lexical representation of
		'''     xsd:integer.
		''' @return
		'''     A BigInteger value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDInteger</code> is not a valid string representation of a <seealso cref="java.math.BigInteger"/> value. </exception>
		Public Shared Function parseInteger(ByVal lexicalXSDInteger As String) As System.Numerics.BigInteger
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseInteger(lexicalXSDInteger)
		End Function

		''' <summary>
		''' <p>
		''' Convert the string argument into an int value. </summary>
		''' <param name="lexicalXSDInt">
		'''     A string containing a lexical representation of
		'''     xsd:int.
		''' @return
		'''     A int value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDInt</code> is not a valid string representation of an <code>int</code> value. </exception>
		Public Shared Function parseInt(ByVal lexicalXSDInt As String) As Integer
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseInt(lexicalXSDInt)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a long value. </summary>
		''' <param name="lexicalXSDLong">
		'''     A string containing lexical representation of
		'''     xsd:long.
		''' @return
		'''     A long value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDLong</code> is not a valid string representation of a <code>long</code> value. </exception>
		Public Shared Function parseLong(ByVal lexicalXSDLong As String) As Long
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseLong(lexicalXSDLong)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a short value. </summary>
		''' <param name="lexicalXSDShort">
		'''     A string containing lexical representation of
		'''     xsd:short.
		''' @return
		'''     A short value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDShort</code> is not a valid string representation of a <code>short</code> value. </exception>
		Public Shared Function parseShort(ByVal lexicalXSDShort As String) As Short
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseShort(lexicalXSDShort)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a BigDecimal value. </summary>
		''' <param name="lexicalXSDDecimal">
		'''     A string containing lexical representation of
		'''     xsd:decimal.
		''' @return
		'''     A BigDecimal value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDDecimal</code> is not a valid string representation of <seealso cref="java.math.BigDecimal"/>. </exception>
		Public Shared Function parseDecimal(ByVal lexicalXSDDecimal As String) As Decimal
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseDecimal(lexicalXSDDecimal)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a float value. </summary>
		''' <param name="lexicalXSDFloat">
		'''     A string containing lexical representation of
		'''     xsd:float.
		''' @return
		'''     A float value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDFloat</code> is not a valid string representation of a <code>float</code> value. </exception>
		Public Shared Function parseFloat(ByVal lexicalXSDFloat As String) As Single
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseFloat(lexicalXSDFloat)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a double value. </summary>
		''' <param name="lexicalXSDDouble">
		'''     A string containing lexical representation of
		'''     xsd:double.
		''' @return
		'''     A double value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> <code>lexicalXSDDouble</code> is not a valid string representation of a <code>double</code> value. </exception>
		Public Shared Function parseDouble(ByVal lexicalXSDDouble As String) As Double
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseDouble(lexicalXSDDouble)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a boolean value. </summary>
		''' <param name="lexicalXSDBoolean">
		'''     A string containing lexical representation of
		'''     xsd:boolean.
		''' @return
		'''     A boolean value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:boolean. </exception>
		Public Shared Function parseBoolean(ByVal lexicalXSDBoolean As String) As Boolean
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseBoolean(lexicalXSDBoolean)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a byte value. </summary>
		''' <param name="lexicalXSDByte">
		'''     A string containing lexical representation of
		'''     xsd:byte.
		''' @return
		'''     A byte value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:byte. </exception>
		Public Shared Function parseByte(ByVal lexicalXSDByte As String) As SByte
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseByte(lexicalXSDByte)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a byte value.
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
		Public Shared Function parseQName(ByVal lexicalXSDQName As String, ByVal nsc As javax.xml.namespace.NamespaceContext) As javax.xml.namespace.QName
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseQName(lexicalXSDQName, nsc)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a Calendar value. </summary>
		''' <param name="lexicalXSDDateTime">
		'''     A string containing lexical representation of
		'''     xsd:datetime.
		''' @return
		'''     A Calendar object represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:dateTime. </exception>
		Public Shared Function parseDateTime(ByVal lexicalXSDDateTime As String) As DateTime
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseDateTime(lexicalXSDDateTime)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into an array of bytes. </summary>
		''' <param name="lexicalXSDBase64Binary">
		'''     A string containing lexical representation
		'''     of xsd:base64Binary.
		''' @return
		'''     An array of bytes represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:base64Binary </exception>
		Public Shared Function parseBase64Binary(ByVal lexicalXSDBase64Binary As String) As SByte()
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseBase64Binary(lexicalXSDBase64Binary)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into an array of bytes. </summary>
		''' <param name="lexicalXSDHexBinary">
		'''     A string containing lexical representation of
		'''     xsd:hexBinary.
		''' @return
		'''     An array of bytes represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:hexBinary. </exception>
	   Public Shared Function parseHexBinary(ByVal lexicalXSDHexBinary As String) As SByte()
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseHexBinary(lexicalXSDHexBinary)
	   End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a long value. </summary>
		''' <param name="lexicalXSDUnsignedInt">
		'''     A string containing lexical representation
		'''     of xsd:unsignedInt.
		''' @return
		'''     A long value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> if string parameter can not be parsed into a <tt>long</tt> value. </exception>
		Public Shared Function parseUnsignedInt(ByVal lexicalXSDUnsignedInt As String) As Long
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseUnsignedInt(lexicalXSDUnsignedInt)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into an int value. </summary>
		''' <param name="lexicalXSDUnsignedShort">
		'''     A string containing lexical
		'''     representation of xsd:unsignedShort.
		''' @return
		'''     An int value represented by the string argument. </param>
		''' <exception cref="NumberFormatException"> if string parameter can not be parsed into an <tt>int</tt> value. </exception>
		Public Shared Function parseUnsignedShort(ByVal lexicalXSDUnsignedShort As String) As Integer
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseUnsignedShort(lexicalXSDUnsignedShort)
		End Function

		''' <summary>
		''' <p>
		''' Converts the string argument into a Calendar value. </summary>
		''' <param name="lexicalXSDTime">
		'''     A string containing lexical representation of
		'''     xsd:time.
		''' @return
		'''     A Calendar value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:Time. </exception>
		Public Shared Function parseTime(ByVal lexicalXSDTime As String) As DateTime
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseTime(lexicalXSDTime)
		End Function
		''' <summary>
		''' <p>
		''' Converts the string argument into a Calendar value. </summary>
		''' <param name="lexicalXSDDate">
		'''      A string containing lexical representation of
		'''     xsd:Date.
		''' @return
		'''     A Calendar value represented by the string argument. </param>
		''' <exception cref="IllegalArgumentException"> if string parameter does not conform to lexical value space defined in XML Schema Part 2: Datatypes for xsd:Date. </exception>
		Public Shared Function parseDate(ByVal lexicalXSDDate As String) As DateTime
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseDate(lexicalXSDDate)
		End Function

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
		Public Shared Function parseAnySimpleType(ByVal lexicalXSDAnySimpleType As String) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.parseAnySimpleType(lexicalXSDAnySimpleType)
		End Function
		''' <summary>
		''' <p>
		''' Converts the string argument into a string. </summary>
		''' <param name="val">
		'''     A string value.
		''' @return
		'''     A string containing a lexical representation of xsd:string. </param>
		 ' also indicate the print methods produce a lexical
		 ' representation for given Java datatypes.

		Public Shared Function printString(ByVal val As String) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printString(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a BigInteger value into a string. </summary>
		''' <param name="val">
		'''     A BigInteger value
		''' @return
		'''     A string containing a lexical representation of xsd:integer </param>
		''' <exception cref="IllegalArgumentException"> <tt>val</tt> is null. </exception>
		Public Shared Function printInteger(ByVal val As System.Numerics.BigInteger) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printInteger(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts an int value into a string. </summary>
		''' <param name="val">
		'''     An int value
		''' @return
		'''     A string containing a lexical representation of xsd:int </param>
		Public Shared Function printInt(ByVal val As Integer) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printInt(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts A long value into a string. </summary>
		''' <param name="val">
		'''     A long value
		''' @return
		'''     A string containing a lexical representation of xsd:long </param>
		Public Shared Function printLong(ByVal val As Long) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printLong(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a short value into a string. </summary>
		''' <param name="val">
		'''     A short value
		''' @return
		'''     A string containing a lexical representation of xsd:short </param>
		Public Shared Function printShort(ByVal val As Short) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printShort(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a BigDecimal value into a string. </summary>
		''' <param name="val">
		'''     A BigDecimal value
		''' @return
		'''     A string containing a lexical representation of xsd:decimal </param>
		''' <exception cref="IllegalArgumentException"> <tt>val</tt> is null. </exception>
		Public Shared Function printDecimal(ByVal val As Decimal) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printDecimal(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a float value into a string. </summary>
		''' <param name="val">
		'''     A float value
		''' @return
		'''     A string containing a lexical representation of xsd:float </param>
		Public Shared Function printFloat(ByVal val As Single) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printFloat(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a double value into a string. </summary>
		''' <param name="val">
		'''     A double value
		''' @return
		'''     A string containing a lexical representation of xsd:double </param>
		Public Shared Function printDouble(ByVal val As Double) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printDouble(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a boolean value into a string. </summary>
		''' <param name="val">
		'''     A boolean value
		''' @return
		'''     A string containing a lexical representation of xsd:boolean </param>
		Public Shared Function printBoolean(ByVal val As Boolean) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printBoolean(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a byte value into a string. </summary>
		''' <param name="val">
		'''     A byte value
		''' @return
		'''     A string containing a lexical representation of xsd:byte </param>
		Public Shared Function printByte(ByVal val As SByte) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printByte(val)
		End Function

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
		Public Shared Function printQName(ByVal val As javax.xml.namespace.QName, ByVal nsc As javax.xml.namespace.NamespaceContext) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printQName(val, nsc)
		End Function

		''' <summary>
		''' <p>
		''' Converts a Calendar value into a string. </summary>
		''' <param name="val">
		'''     A Calendar value
		''' @return
		'''     A string containing a lexical representation of xsd:dateTime </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Public Shared Function printDateTime(ByVal val As DateTime) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printDateTime(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts an array of bytes into a string. </summary>
		''' <param name="val">
		'''     An array of bytes
		''' @return
		'''     A string containing a lexical representation of xsd:base64Binary </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Public Shared Function printBase64Binary(ByVal val As SByte()) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printBase64Binary(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts an array of bytes into a string. </summary>
		''' <param name="val">
		'''     An array of bytes
		''' @return
		'''     A string containing a lexical representation of xsd:hexBinary </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Public Shared Function printHexBinary(ByVal val As SByte()) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printHexBinary(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a long value into a string. </summary>
		''' <param name="val">
		'''     A long value
		''' @return
		'''     A string containing a lexical representation of xsd:unsignedInt </param>
		Public Shared Function printUnsignedInt(ByVal val As Long) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printUnsignedInt(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts an int value into a string. </summary>
		''' <param name="val">
		'''     An int value
		''' @return
		'''     A string containing a lexical representation of xsd:unsignedShort </param>
		Public Shared Function printUnsignedShort(ByVal val As Integer) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printUnsignedShort(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a Calendar value into a string. </summary>
		''' <param name="val">
		'''     A Calendar value
		''' @return
		'''     A string containing a lexical representation of xsd:time </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Public Shared Function printTime(ByVal val As DateTime) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printTime(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a Calendar value into a string. </summary>
		''' <param name="val">
		'''     A Calendar value
		''' @return
		'''     A string containing a lexical representation of xsd:date </param>
		''' <exception cref="IllegalArgumentException"> if <tt>val</tt> is null. </exception>
		Public Shared Function printDate(ByVal val As DateTime) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printDate(val)
		End Function

		''' <summary>
		''' <p>
		''' Converts a string value into a string. </summary>
		''' <param name="val">
		'''     A string value
		''' @return
		'''     A string containing a lexical representation of xsd:AnySimpleType </param>
		Public Shared Function printAnySimpleType(ByVal val As String) As String
			If theConverter Is Nothing Then initConverter()
			Return theConverter.printAnySimpleType(val)
		End Function
	End Class

End Namespace
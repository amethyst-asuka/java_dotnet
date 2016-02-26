Imports System

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.datatype


	''' <summary>
	''' <p>Factory that creates new <code>javax.xml.datatype</code> <code>Object</code>s that map XML to/from Java <code>Object</code>s.</p>
	''' 
	''' <p>A new instance of the <code>DatatypeFactory</code> is created through the <seealso cref="#newInstance()"/> method
	''' that uses the following implementation resolution mechanisms to determine an implementation:</p>
	''' <ol>
	'''    <li>
	'''      If the system property specified by <seealso cref="#DATATYPEFACTORY_PROPERTY"/>, "<code>javax.xml.datatype.DatatypeFactory</code>",
	'''      exists, a class with the name of the property value is instantiated.
	'''      Any Exception thrown during the instantiation process is wrapped as a <seealso cref="DatatypeConfigurationException"/>.
	'''    </li>
	'''    <li>
	'''      If the file ${JAVA_HOME}/lib/jaxp.properties exists, it is loaded in a <seealso cref="java.util.Properties"/> <code>Object</code>.
	'''      The <code>Properties</code> <code>Object </code> is then queried for the property as documented in the prior step
	'''      and processed as documented in the prior step.
	'''    </li>
	'''    <li>
	'''     Uses the service-provider loading facilities, defined by the <seealso cref="java.util.ServiceLoader"/> class, to attempt
	'''     to locate and load an implementation of the service using the {@linkplain
	'''     java.util.ServiceLoader#load(java.lang.Class) default loading mechanism}:
	'''     the service-provider loading facility will use the {@linkplain
	'''     java.lang.Thread#getContextClassLoader() current thread's context class loader}
	'''     to attempt to load the service. If the context class
	'''     loader is null, the {@linkplain
	'''     ClassLoader#getSystemClassLoader() system class loader} will be used.
	'''     <br>
	'''     In case of {@link java.util.ServiceConfigurationError service
	'''     configuration error} a <seealso cref="javax.xml.datatype.DatatypeConfigurationException"/>
	'''     will be thrown.
	'''    </li>
	'''    <li>
	'''      The final mechanism is to attempt to instantiate the <code>Class</code> specified by
	'''      <seealso cref="#DATATYPEFACTORY_IMPLEMENTATION_CLASS"/>.
	'''      Any Exception thrown during the instantiation process is wrapped as a <seealso cref="DatatypeConfigurationException"/>.
	'''    </li>
	''' </ol>
	''' 
	''' @author <a href="mailto:Joseph.Fialli@Sun.COM">Joseph Fialli</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @author <a href="mailto:Neeraj.Bajaj@sun.com">Neeraj Bajaj</a>
	''' 
	''' @version $Revision: 1.13 $, $Date: 2010/03/11 23:10:53 $
	''' @since 1.5
	''' </summary>
	Public MustInherit Class DatatypeFactory

		''' <summary>
		''' <p>Default property name as defined in JSR 206: Java(TM) API for XML Processing (JAXP) 1.3.</p>
		''' 
		''' <p>Default value is <code>javax.xml.datatype.DatatypeFactory</code>.</p>
		''' </summary>
		Public Const DATATYPEFACTORY_PROPERTY As String = "javax.xml.datatype.DatatypeFactory"
				' We use a String constant here, rather than calling
				' DatatypeFactory.class.getName() - in order to make javadoc
				' generate a See Also: Constant Field Value link.

		''' <summary>
		''' <p>Default implementation class name as defined in
		''' <em>JSR 206: Java(TM) API for XML Processing (JAXP) 1.3</em>.</p>
		''' 
		''' <p>Implementers should specify the name of an appropriate class
		''' to be instantiated if no other implementation resolution mechanism
		''' succeeds.</p>
		''' 
		''' <p>Users should not refer to this field; it is intended only to
		''' document a factory implementation detail.
		''' </p>
		''' </summary>
		Public Shared ReadOnly DATATYPEFACTORY_IMPLEMENTATION_CLASS As New String("com.sun.org.apache.xerces.internal.jaxp.datatype.DatatypeFactoryImpl")
			' We use new String() here to prevent javadoc from generating
			' a See Also: Constant Field Value link.

		''' <summary>
		''' http://www.w3.org/TR/xpath-datamodel/#xdtschema defines two regexps
		''' to constrain the value space of dayTimeDuration ([^YM]*[DT].*)
		''' and yearMonthDuration ([^DT]*). Note that these expressions rely on
		''' the fact that the value must be an xs:Duration, they simply exclude
		''' some Durations.
		''' </summary>
		Private Shared ReadOnly XDTSCHEMA_YMD As java.util.regex.Pattern = java.util.regex.Pattern.compile("[^DT]*")

		Private Shared ReadOnly XDTSCHEMA_DTD As java.util.regex.Pattern = java.util.regex.Pattern.compile("[^YM]*[DT].*")

		''' <summary>
		''' <p>Protected constructor to prevent instaniation outside of package.</p>
		''' 
		''' <p>Use <seealso cref="#newInstance()"/> to create a <code>DatatypeFactory</code>.</p>
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' <p>Obtain a new instance of a <code>DatatypeFactory</code>.</p>
		''' 
		''' <p>The implementation resolution mechanisms are <a href="#DatatypeFactory.newInstance">defined</a> in this
		''' <code>Class</code>'s documentation.</p>
		''' </summary>
		''' <returns> New instance of a <code>DatatypeFactory</code>
		''' </returns>
		''' <exception cref="DatatypeConfigurationException"> If the implementation is not
		'''   available or cannot be instantiated.
		''' </exception>
		''' <seealso cref= #newInstance(String factoryClassName, ClassLoader classLoader) </seealso>
		Public Shared Function newInstance() As DatatypeFactory

				Return FactoryFinder.find(GetType(DatatypeFactory), DATATYPEFACTORY_IMPLEMENTATION_CLASS)
						' The default property name according to the JAXP spec 
						' The fallback implementation class name 
		End Function

		''' <summary>
		''' <p>Obtain a new instance of a <code>DatatypeFactory</code> from class name.
		''' This function is useful when there are multiple providers in the classpath.
		''' It gives more control to the application as it can specify which provider
		''' should be loaded.</p>
		''' 
		''' <p>Once an application has obtained a reference to a <code>DatatypeFactory</code>
		''' it can use the factory to configure and obtain datatype instances.</P>
		''' 
		''' 
		''' <h2>Tip for Trouble-shooting</h2>
		''' <p>Setting the <code>jaxp.debug</code> system property will cause
		''' this method to print a lot of debug messages
		''' to <code>System.err</code> about what it is doing and where it is looking at.</p>
		''' 
		''' <p> If you have problems try:</p>
		''' <pre>
		''' java -Djaxp.debug=1 YourProgram ....
		''' </pre>
		''' </summary>
		''' <param name="factoryClassName"> fully qualified factory class name that provides implementation of <code>javax.xml.datatype.DatatypeFactory</code>.
		''' </param>
		''' <param name="classLoader"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		'''                     current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' </param>
		''' <returns> New instance of a <code>DatatypeFactory</code>
		''' </returns>
		''' <exception cref="DatatypeConfigurationException"> if <code>factoryClassName</code> is <code>null</code>, or
		'''                                   the factory class cannot be loaded, instantiated.
		''' </exception>
		''' <seealso cref= #newInstance()
		''' 
		''' @since 1.6 </seealso>
		Public Shared Function newInstance(ByVal factoryClassName As String, ByVal classLoader As ClassLoader) As DatatypeFactory
			Return FactoryFinder.newInstance(GetType(DatatypeFactory), factoryClassName, classLoader, False)
		End Function

		''' <summary>
		''' <p>Obtain a new instance of a <code>Duration</code>
		''' specifying the <code>Duration</code> as its string representation, "PnYnMnDTnHnMnS",
		''' as defined in XML Schema 1.0 section 3.2.6.1.</p>
		''' 
		''' <p>XML Schema Part 2: Datatypes, 3.2.6 duration, defines <code>duration</code> as:</p>
		''' <blockquote>
		''' duration represents a duration of time.
		''' The value space of duration is a six-dimensional space where the coordinates designate the
		''' Gregorian year, month, day, hour, minute, and second components defined in Section 5.5.3.2 of [ISO 8601], respectively.
		''' These components are ordered in their significance by their order of appearance i.e. as
		''' year, month, day, hour, minute, and second.
		''' </blockquote>
		''' <p>All six values are set and available from the created <seealso cref="Duration"/></p>
		''' 
		''' <p>The XML Schema specification states that values can be of an arbitrary size.
		''' Implementations may chose not to or be incapable of supporting arbitrarily large and/or small values.
		''' An <seealso cref="UnsupportedOperationException"/> will be thrown with a message indicating implementation limits
		''' if implementation capacities are exceeded.</p>
		''' </summary>
		''' <param name="lexicalRepresentation"> <code>String</code> representation of a <code>Duration</code>.
		''' </param>
		''' <returns> New <code>Duration</code> created from parsing the <code>lexicalRepresentation</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If <code>lexicalRepresentation</code> is not a valid representation of a <code>Duration</code>. </exception>
		''' <exception cref="UnsupportedOperationException"> If implementation cannot support requested values. </exception>
		''' <exception cref="NullPointerException"> if <code>lexicalRepresentation</code> is <code>null</code>. </exception>
		Public MustOverride Function newDuration(ByVal lexicalRepresentation As String) As Duration

		''' <summary>
		''' <p>Obtain a new instance of a <code>Duration</code>
		''' specifying the <code>Duration</code> as milliseconds.</p>
		''' 
		''' <p>XML Schema Part 2: Datatypes, 3.2.6 duration, defines <code>duration</code> as:</p>
		''' <blockquote>
		''' duration represents a duration of time.
		''' The value space of duration is a six-dimensional space where the coordinates designate the
		''' Gregorian year, month, day, hour, minute, and second components defined in Section 5.5.3.2 of [ISO 8601], respectively.
		''' These components are ordered in their significance by their order of appearance i.e. as
		''' year, month, day, hour, minute, and second.
		''' </blockquote>
		''' <p>All six values are set by computing their values from the specified milliseconds
		''' and are available using the <code>get</code> methods of  the created <seealso cref="Duration"/>.
		''' The values conform to and are defined by:</p>
		''' <ul>
		'''   <li>ISO 8601:2000(E) Section 5.5.3.2 Alternative format</li>
		'''   <li><a href="http://www.w3.org/TR/xmlschema-2/#isoformats">
		'''     W3C XML Schema 1.0 Part 2, Appendix D, ISO 8601 Date and Time Formats</a>
		'''   </li>
		'''   <li><seealso cref="XMLGregorianCalendar"/>  Date/Time Datatype Field Mapping Between XML Schema 1.0 and Java Representation</li>
		''' </ul>
		''' 
		''' <p>The default start instance is defined by <seealso cref="GregorianCalendar"/>'s use of the start of the epoch: i.e.,
		''' <seealso cref="java.util.Calendar#YEAR"/> = 1970,
		''' <seealso cref="java.util.Calendar#MONTH"/> = <seealso cref="java.util.Calendar#JANUARY"/>,
		''' <seealso cref="java.util.Calendar#DATE"/> = 1, etc.
		''' This is important as there are variations in the Gregorian Calendar,
		''' e.g. leap years have different days in the month = <seealso cref="java.util.Calendar#FEBRUARY"/>
		''' so the result of <seealso cref="Duration#getMonths()"/> and <seealso cref="Duration#getDays()"/> can be influenced.</p>
		''' </summary>
		''' <param name="durationInMilliSeconds"> Duration in milliseconds to create.
		''' </param>
		''' <returns> New <code>Duration</code> representing <code>durationInMilliSeconds</code>. </returns>
		Public MustOverride Function newDuration(ByVal durationInMilliSeconds As Long) As Duration

		''' <summary>
		''' <p>Obtain a new instance of a <code>Duration</code>
		''' specifying the <code>Duration</code> as isPositive, years, months, days, hours, minutes, seconds.</p>
		''' 
		''' <p>The XML Schema specification states that values can be of an arbitrary size.
		''' Implementations may chose not to or be incapable of supporting arbitrarily large and/or small values.
		''' An <seealso cref="UnsupportedOperationException"/> will be thrown with a message indicating implementation limits
		''' if implementation capacities are exceeded.</p>
		''' 
		''' <p>A <code>null</code> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="isPositive"> Set to <code>false</code> to create a negative duration. When the length
		'''   of the duration is zero, this parameter will be ignored. </param>
		''' <param name="years"> of this <code>Duration</code> </param>
		''' <param name="months"> of this <code>Duration</code> </param>
		''' <param name="days"> of this <code>Duration</code> </param>
		''' <param name="hours"> of this <code>Duration</code> </param>
		''' <param name="minutes"> of this <code>Duration</code> </param>
		''' <param name="seconds"> of this <code>Duration</code>
		''' </param>
		''' <returns> New <code>Duration</code> created from the specified values.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the values are not a valid representation of a
		''' <code>Duration</code>: if all the fields (years, months, ...) are null or
		''' if any of the fields is negative. </exception>
		''' <exception cref="UnsupportedOperationException"> If implementation cannot support requested values. </exception>
		Public MustOverride Function newDuration(ByVal isPositive As Boolean, ByVal years As System.Numerics.BigInteger, ByVal months As System.Numerics.BigInteger, ByVal days As System.Numerics.BigInteger, ByVal hours As System.Numerics.BigInteger, ByVal minutes As System.Numerics.BigInteger, ByVal seconds As Decimal) As Duration

		''' <summary>
		''' <p>Obtain a new instance of a <code>Duration</code>
		''' specifying the <code>Duration</code> as isPositive, years, months, days, hours, minutes, seconds.</p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="isPositive"> Set to <code>false</code> to create a negative duration. When the length
		'''   of the duration is zero, this parameter will be ignored. </param>
		''' <param name="years"> of this <code>Duration</code> </param>
		''' <param name="months"> of this <code>Duration</code> </param>
		''' <param name="days"> of this <code>Duration</code> </param>
		''' <param name="hours"> of this <code>Duration</code> </param>
		''' <param name="minutes"> of this <code>Duration</code> </param>
		''' <param name="seconds"> of this <code>Duration</code>
		''' </param>
		''' <returns> New <code>Duration</code> created from the specified values.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the values are not a valid representation of a
		''' <code>Duration</code>: if any of the fields is negative.
		''' </exception>
		''' <seealso cref= #newDuration(
		'''   boolean isPositive,
		'''   BigInteger years,
		'''   BigInteger months,
		'''   BigInteger days,
		'''   BigInteger hours,
		'''   BigInteger minutes,
		'''   BigDecimal seconds) </seealso>
		Public Overridable Function newDuration(ByVal isPositive As Boolean, ByVal years As Integer, ByVal months As Integer, ByVal days As Integer, ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer) As Duration

				' years may not be set
				Dim realYears As System.Numerics.BigInteger = If(years <> DatatypeConstants.FIELD_UNDEFINED, System.Numerics.BigInteger.valueOf(CLng(years)), Nothing)

				' months may not be set
				Dim realMonths As System.Numerics.BigInteger = If(months <> DatatypeConstants.FIELD_UNDEFINED, System.Numerics.BigInteger.valueOf(CLng(months)), Nothing)

				' days may not be set
				Dim realDays As System.Numerics.BigInteger = If(days <> DatatypeConstants.FIELD_UNDEFINED, System.Numerics.BigInteger.valueOf(CLng(days)), Nothing)

				' hours may not be set
				Dim realHours As System.Numerics.BigInteger = If(hours <> DatatypeConstants.FIELD_UNDEFINED, System.Numerics.BigInteger.valueOf(CLng(hours)), Nothing)

				' minutes may not be set
				Dim realMinutes As System.Numerics.BigInteger = If(minutes <> DatatypeConstants.FIELD_UNDEFINED, System.Numerics.BigInteger.valueOf(CLng(minutes)), Nothing)

				' seconds may not be set
				Dim realSeconds As Decimal = If(seconds <> DatatypeConstants.FIELD_UNDEFINED, Decimal.valueOf(CLng(seconds)), Nothing)

						Return newDuration(isPositive, realYears, realMonths, realDays, realHours, realMinutes, realSeconds)
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:dayTimeDuration</code> by parsing its <code>String</code> representation,
		''' "<em>PnDTnHnMnS</em>", <a href="http://www.w3.org/TR/xpath-datamodel#dayTimeDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:dayTimeDuration</a>.</p>
		''' 
		''' <p>The datatype <code>xdt:dayTimeDuration</code> is a subtype of <code>xs:duration</code>
		''' whose lexical representation contains only day, hour, minute, and second components.
		''' This datatype resides in the namespace <code>http://www.w3.org/2003/11/xpath-datatypes</code>.</p>
		''' 
		''' <p>All four values are set and available from the created <seealso cref="Duration"/></p>
		''' 
		''' <p>The XML Schema specification states that values can be of an arbitrary size.
		''' Implementations may chose not to or be incapable of supporting arbitrarily large and/or small values.
		''' An <seealso cref="UnsupportedOperationException"/> will be thrown with a message indicating implementation limits
		''' if implementation capacities are exceeded.</p>
		''' </summary>
		''' <param name="lexicalRepresentation"> Lexical representation of a duration.
		''' </param>
		''' <returns> New <code>Duration</code> created using the specified <code>lexicalRepresentation</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If <code>lexicalRepresentation</code> is not a valid representation of a <code>Duration</code> expressed only in terms of days and time. </exception>
		''' <exception cref="UnsupportedOperationException"> If implementation cannot support requested values. </exception>
		''' <exception cref="NullPointerException"> If <code>lexicalRepresentation</code> is <code>null</code>. </exception>
		Public Overridable Function newDurationDayTime(ByVal lexicalRepresentation As String) As Duration
			' lexicalRepresentation must be non-null
			If lexicalRepresentation Is Nothing Then Throw New NullPointerException("Trying to create an xdt:dayTimeDuration with an invalid" & " lexical representation of ""null""")

			' test lexicalRepresentation against spec regex
			Dim matcher As java.util.regex.Matcher = XDTSCHEMA_DTD.matcher(lexicalRepresentation)
			If Not matcher.matches() Then Throw New System.ArgumentException("Trying to create an xdt:dayTimeDuration with an invalid" & " lexical representation of """ & lexicalRepresentation & """, data model requires years and months only.")

			Return newDuration(lexicalRepresentation)
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:dayTimeDuration</code> using the specified milliseconds as defined in
		''' <a href="http://www.w3.org/TR/xpath-datamodel#dayTimeDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:dayTimeDuration</a>.</p>
		''' 
		''' <p>The datatype <code>xdt:dayTimeDuration</code> is a subtype of <code>xs:duration</code>
		''' whose lexical representation contains only day, hour, minute, and second components.
		''' This datatype resides in the namespace <code>http://www.w3.org/2003/11/xpath-datatypes</code>.</p>
		''' 
		''' <p>All four values are set by computing their values from the specified milliseconds
		''' and are available using the <code>get</code> methods of  the created <seealso cref="Duration"/>.
		''' The values conform to and are defined by:</p>
		''' <ul>
		'''   <li>ISO 8601:2000(E) Section 5.5.3.2 Alternative format</li>
		'''   <li><a href="http://www.w3.org/TR/xmlschema-2/#isoformats">
		'''     W3C XML Schema 1.0 Part 2, Appendix D, ISO 8601 Date and Time Formats</a>
		'''   </li>
		'''   <li><seealso cref="XMLGregorianCalendar"/>  Date/Time Datatype Field Mapping Between XML Schema 1.0 and Java Representation</li>
		''' </ul>
		''' 
		''' <p>The default start instance is defined by <seealso cref="GregorianCalendar"/>'s use of the start of the epoch: i.e.,
		''' <seealso cref="java.util.Calendar#YEAR"/> = 1970,
		''' <seealso cref="java.util.Calendar#MONTH"/> = <seealso cref="java.util.Calendar#JANUARY"/>,
		''' <seealso cref="java.util.Calendar#DATE"/> = 1, etc.
		''' This is important as there are variations in the Gregorian Calendar,
		''' e.g. leap years have different days in the month = <seealso cref="java.util.Calendar#FEBRUARY"/>
		''' so the result of <seealso cref="Duration#getDays()"/> can be influenced.</p>
		''' 
		''' <p>Any remaining milliseconds after determining the day, hour, minute and second are discarded.</p>
		''' </summary>
		''' <param name="durationInMilliseconds"> Milliseconds of <code>Duration</code> to create.
		''' </param>
		''' <returns> New <code>Duration</code> created with the specified <code>durationInMilliseconds</code>.
		''' </returns>
		''' <seealso cref= <a href="http://www.w3.org/TR/xpath-datamodel#dayTimeDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:dayTimeDuration</a> </seealso>
		Public Overridable Function newDurationDayTime(ByVal durationInMilliseconds As Long) As Duration

				Return newDuration(durationInMilliseconds)
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:dayTimeDuration</code> using the specified
		''' <code>day</code>, <code>hour</code>, <code>minute</code> and <code>second</code> as defined in
		''' <a href="http://www.w3.org/TR/xpath-datamodel#dayTimeDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:dayTimeDuration</a>.</p>
		''' 
		''' <p>The datatype <code>xdt:dayTimeDuration</code> is a subtype of <code>xs:duration</code>
		''' whose lexical representation contains only day, hour, minute, and second components.
		''' This datatype resides in the namespace <code>http://www.w3.org/2003/11/xpath-datatypes</code>.</p>
		''' 
		''' <p>The XML Schema specification states that values can be of an arbitrary size.
		''' Implementations may chose not to or be incapable of supporting arbitrarily large and/or small values.
		''' An <seealso cref="UnsupportedOperationException"/> will be thrown with a message indicating implementation limits
		''' if implementation capacities are exceeded.</p>
		''' 
		''' <p>A <code>null</code> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="isPositive"> Set to <code>false</code> to create a negative duration. When the length
		'''   of the duration is zero, this parameter will be ignored. </param>
		''' <param name="day"> Day of <code>Duration</code>. </param>
		''' <param name="hour"> Hour of <code>Duration</code>. </param>
		''' <param name="minute"> Minute of <code>Duration</code>. </param>
		''' <param name="second"> Second of <code>Duration</code>.
		''' </param>
		''' <returns> New <code>Duration</code> created with the specified <code>day</code>, <code>hour</code>, <code>minute</code>
		''' and <code>second</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the values are not a valid representation of a
		''' <code>Duration</code>: if all the fields (day, hour, ...) are null or
		''' if any of the fields is negative. </exception>
		''' <exception cref="UnsupportedOperationException"> If implementation cannot support requested values. </exception>
		Public Overridable Function newDurationDayTime(ByVal isPositive As Boolean, ByVal day As System.Numerics.BigInteger, ByVal hour As System.Numerics.BigInteger, ByVal minute As System.Numerics.BigInteger, ByVal second As System.Numerics.BigInteger) As Duration

				Return newDuration(isPositive, Nothing, Nothing, day, hour, minute,If(second IsNot Nothing, New Decimal(second), Nothing)) ' months -  years
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:dayTimeDuration</code> using the specified
		''' <code>day</code>, <code>hour</code>, <code>minute</code> and <code>second</code> as defined in
		''' <a href="http://www.w3.org/TR/xpath-datamodel#dayTimeDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:dayTimeDuration</a>.</p>
		''' 
		''' <p>The datatype <code>xdt:dayTimeDuration</code> is a subtype of <code>xs:duration</code>
		''' whose lexical representation contains only day, hour, minute, and second components.
		''' This datatype resides in the namespace <code>http://www.w3.org/2003/11/xpath-datatypes</code>.</p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="isPositive"> Set to <code>false</code> to create a negative duration. When the length
		'''   of the duration is zero, this parameter will be ignored. </param>
		''' <param name="day"> Day of <code>Duration</code>. </param>
		''' <param name="hour"> Hour of <code>Duration</code>. </param>
		''' <param name="minute"> Minute of <code>Duration</code>. </param>
		''' <param name="second"> Second of <code>Duration</code>.
		''' </param>
		''' <returns> New <code>Duration</code> created with the specified <code>day</code>, <code>hour</code>, <code>minute</code>
		''' and <code>second</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the values are not a valid representation of a
		''' <code>Duration</code>: if any of the fields (day, hour, ...) is negative. </exception>
		Public Overridable Function newDurationDayTime(ByVal isPositive As Boolean, ByVal day As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer) As Duration

						Return newDurationDayTime(isPositive, System.Numerics.BigInteger.valueOf(CLng(day)), System.Numerics.BigInteger.valueOf(CLng(hour)), System.Numerics.BigInteger.valueOf(CLng(minute)), System.Numerics.BigInteger.valueOf(CLng(second)))
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:yearMonthDuration</code> by parsing its <code>String</code> representation,
		''' "<em>PnYnM</em>", <a href="http://www.w3.org/TR/xpath-datamodel#yearMonthDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:yearMonthDuration</a>.</p>
		''' 
		''' <p>The datatype <code>xdt:yearMonthDuration</code> is a subtype of <code>xs:duration</code>
		''' whose lexical representation contains only year and month components.
		''' This datatype resides in the namespace <seealso cref="javax.xml.XMLConstants#W3C_XPATH_DATATYPE_NS_URI"/>.</p>
		''' 
		''' <p>Both values are set and available from the created <seealso cref="Duration"/></p>
		''' 
		''' <p>The XML Schema specification states that values can be of an arbitrary size.
		''' Implementations may chose not to or be incapable of supporting arbitrarily large and/or small values.
		''' An <seealso cref="UnsupportedOperationException"/> will be thrown with a message indicating implementation limits
		''' if implementation capacities are exceeded.</p>
		''' </summary>
		''' <param name="lexicalRepresentation"> Lexical representation of a duration.
		''' </param>
		''' <returns> New <code>Duration</code> created using the specified <code>lexicalRepresentation</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If <code>lexicalRepresentation</code> is not a valid representation of a <code>Duration</code> expressed only in terms of years and months. </exception>
		''' <exception cref="UnsupportedOperationException"> If implementation cannot support requested values. </exception>
		''' <exception cref="NullPointerException"> If <code>lexicalRepresentation</code> is <code>null</code>. </exception>
		Public Overridable Function newDurationYearMonth(ByVal lexicalRepresentation As String) As Duration

			' lexicalRepresentation must be non-null
			If lexicalRepresentation Is Nothing Then Throw New NullPointerException("Trying to create an xdt:yearMonthDuration with an invalid" & " lexical representation of ""null""")

			' test lexicalRepresentation against spec regex
			Dim matcher As java.util.regex.Matcher = XDTSCHEMA_YMD.matcher(lexicalRepresentation)
			If Not matcher.matches() Then Throw New System.ArgumentException("Trying to create an xdt:yearMonthDuration with an invalid" & " lexical representation of """ & lexicalRepresentation & """, data model requires days and times only.")

			Return newDuration(lexicalRepresentation)
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:yearMonthDuration</code> using the specified milliseconds as defined in
		''' <a href="http://www.w3.org/TR/xpath-datamodel#yearMonthDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:yearMonthDuration</a>.</p>
		''' 
		''' <p>The datatype <code>xdt:yearMonthDuration</code> is a subtype of <code>xs:duration</code>
		''' whose lexical representation contains only year and month components.
		''' This datatype resides in the namespace <seealso cref="javax.xml.XMLConstants#W3C_XPATH_DATATYPE_NS_URI"/>.</p>
		''' 
		''' <p>Both values are set by computing their values from the specified milliseconds
		''' and are available using the <code>get</code> methods of  the created <seealso cref="Duration"/>.
		''' The values conform to and are defined by:</p>
		''' <ul>
		'''   <li>ISO 8601:2000(E) Section 5.5.3.2 Alternative format</li>
		'''   <li><a href="http://www.w3.org/TR/xmlschema-2/#isoformats">
		'''     W3C XML Schema 1.0 Part 2, Appendix D, ISO 8601 Date and Time Formats</a>
		'''   </li>
		'''   <li><seealso cref="XMLGregorianCalendar"/>  Date/Time Datatype Field Mapping Between XML Schema 1.0 and Java Representation</li>
		''' </ul>
		''' 
		''' <p>The default start instance is defined by <seealso cref="GregorianCalendar"/>'s use of the start of the epoch: i.e.,
		''' <seealso cref="java.util.Calendar#YEAR"/> = 1970,
		''' <seealso cref="java.util.Calendar#MONTH"/> = <seealso cref="java.util.Calendar#JANUARY"/>,
		''' <seealso cref="java.util.Calendar#DATE"/> = 1, etc.
		''' This is important as there are variations in the Gregorian Calendar,
		''' e.g. leap years have different days in the month = <seealso cref="java.util.Calendar#FEBRUARY"/>
		''' so the result of <seealso cref="Duration#getMonths()"/> can be influenced.</p>
		''' 
		''' <p>Any remaining milliseconds after determining the year and month are discarded.</p>
		''' </summary>
		''' <param name="durationInMilliseconds"> Milliseconds of <code>Duration</code> to create.
		''' </param>
		''' <returns> New <code>Duration</code> created using the specified <code>durationInMilliseconds</code>. </returns>
		Public Overridable Function newDurationYearMonth(ByVal durationInMilliseconds As Long) As Duration

			' create a Duration that only has sign, year & month
			' Duration is immutable, so need to create a new Duration
			' implementations may override this method in a more efficient way
			Dim fullDuration As Duration = newDuration(durationInMilliseconds)
			Dim isPositive As Boolean = If(fullDuration.sign = -1, False, True)
			Dim years As System.Numerics.BigInteger = CType(fullDuration.getField(DatatypeConstants.YEARS), System.Numerics.BigInteger)
			If years Is Nothing Then years = System.Numerics.BigInteger.ZERO
			Dim months As System.Numerics.BigInteger = CType(fullDuration.getField(DatatypeConstants.MONTHS), System.Numerics.BigInteger)
			If months Is Nothing Then months = System.Numerics.BigInteger.ZERO

			Return newDurationYearMonth(isPositive, years, months)
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:yearMonthDuration</code> using the specified
		''' <code>year</code> and <code>month</code> as defined in
		''' <a href="http://www.w3.org/TR/xpath-datamodel#yearMonthDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:yearMonthDuration</a>.</p>
		''' 
		''' <p>The XML Schema specification states that values can be of an arbitrary size.
		''' Implementations may chose not to or be incapable of supporting arbitrarily large and/or small values.
		''' An <seealso cref="UnsupportedOperationException"/> will be thrown with a message indicating implementation limits
		''' if implementation capacities are exceeded.</p>
		''' 
		''' <p>A <code>null</code> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="isPositive"> Set to <code>false</code> to create a negative duration. When the length
		'''   of the duration is zero, this parameter will be ignored. </param>
		''' <param name="year"> Year of <code>Duration</code>. </param>
		''' <param name="month"> Month of <code>Duration</code>.
		''' </param>
		''' <returns> New <code>Duration</code> created using the specified <code>year</code> and <code>month</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the values are not a valid representation of a
		''' <code>Duration</code>: if all of the fields (year, month) are null or
		''' if any of the fields is negative. </exception>
		''' <exception cref="UnsupportedOperationException"> If implementation cannot support requested values. </exception>
		Public Overridable Function newDurationYearMonth(ByVal isPositive As Boolean, ByVal year As System.Numerics.BigInteger, ByVal month As System.Numerics.BigInteger) As Duration

				Return newDuration(isPositive, year, month, Nothing, Nothing, Nothing, Nothing) ' seconds -  minutes -  hours -  days
		End Function

		''' <summary>
		''' <p>Create a <code>Duration</code> of type <code>xdt:yearMonthDuration</code> using the specified
		''' <code>year</code> and <code>month</code> as defined in
		''' <a href="http://www.w3.org/TR/xpath-datamodel#yearMonthDuration">
		'''   XQuery 1.0 and XPath 2.0 Data Model, xdt:yearMonthDuration</a>.</p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="isPositive"> Set to <code>false</code> to create a negative duration. When the length
		'''   of the duration is zero, this parameter will be ignored. </param>
		''' <param name="year"> Year of <code>Duration</code>. </param>
		''' <param name="month"> Month of <code>Duration</code>.
		''' </param>
		''' <returns> New <code>Duration</code> created using the specified <code>year</code> and <code>month</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the values are not a valid representation of a
		''' <code>Duration</code>: if any of the fields (year, month) is negative. </exception>
		Public Overridable Function newDurationYearMonth(ByVal isPositive As Boolean, ByVal year As Integer, ByVal month As Integer) As Duration

				Return newDurationYearMonth(isPositive, System.Numerics.BigInteger.valueOf(CLng(year)), System.Numerics.BigInteger.valueOf(CLng(month)))
		End Function

		''' <summary>
		''' <p>Create a new instance of an <code>XMLGregorianCalendar</code>.</p>
		''' 
		''' <p>All date/time datatype fields set to <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> or null.</p>
		''' </summary>
		''' <returns> New <code>XMLGregorianCalendar</code> with all date/time datatype fields set to
		'''   <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> or null. </returns>
		Public MustOverride Function newXMLGregorianCalendar() As XMLGregorianCalendar

		''' <summary>
		''' <p>Create a new XMLGregorianCalendar by parsing the String as a lexical representation.</p>
		''' 
		''' <p>Parsing the lexical string representation is defined in
		''' <a href="http://www.w3.org/TR/xmlschema-2/#dateTime-order">XML Schema 1.0 Part 2, Section 3.2.[7-14].1,
		''' <em>Lexical Representation</em>.</a></p>
		''' 
		''' <p>The string representation may not have any leading and trailing whitespaces.</p>
		''' 
		''' <p>The parsing is done field by field so that
		''' the following holds for any lexically correct String x:</p>
		''' <pre>
		''' newXMLGregorianCalendar(x).toXMLFormat().equals(x)
		''' </pre>
		''' <p>Except for the noted lexical/canonical representation mismatches
		''' listed in <a href="http://www.w3.org/2001/05/xmlschema-errata#e2-45">
		''' XML Schema 1.0 errata, Section 3.2.7.2</a>.</p>
		''' </summary>
		''' <param name="lexicalRepresentation"> Lexical representation of one the eight XML Schema date/time datatypes.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from the <code>lexicalRepresentation</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the <code>lexicalRepresentation</code> is not a valid <code>XMLGregorianCalendar</code>. </exception>
		''' <exception cref="NullPointerException"> If <code>lexicalRepresentation</code> is <code>null</code>. </exception>
		Public MustOverride Function newXMLGregorianCalendar(ByVal lexicalRepresentation As String) As XMLGregorianCalendar

		''' <summary>
		''' <p>Create an <code>XMLGregorianCalendar</code> from a <seealso cref="GregorianCalendar"/>.</p>
		''' 
		''' <table border="2" rules="all" cellpadding="2">
		'''   <thead>
		'''     <tr>
		'''       <th align="center" colspan="2">
		'''          Field by Field Conversion from
		'''          <seealso cref="GregorianCalendar"/> to an <seealso cref="XMLGregorianCalendar"/>
		'''       </th>
		'''     </tr>
		'''     <tr>
		'''        <th><code>java.util.GregorianCalendar</code> field</th>
		'''        <th><code>javax.xml.datatype.XMLGregorianCalendar</code> field</th>
		'''     </tr>
		'''   </thead>
		'''   <tbody>
		'''     <tr>
		'''       <td><code>ERA == GregorianCalendar.BC ? -YEAR : YEAR</code></td>
		'''       <td><seealso cref="XMLGregorianCalendar#setYear(int year)"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>MONTH + 1</code></td>
		'''       <td><seealso cref="XMLGregorianCalendar#setMonth(int month)"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>DAY_OF_MONTH</code></td>
		'''       <td><seealso cref="XMLGregorianCalendar#setDay(int day)"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>HOUR_OF_DAY, MINUTE, SECOND, MILLISECOND</code></td>
		'''       <td><seealso cref="XMLGregorianCalendar#setTime(int hour, int minute, int second, BigDecimal fractional)"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td>
		'''         <code>(ZONE_OFFSET + DST_OFFSET) / (60*1000)</code><br/>
		'''         <em>(in minutes)</em>
		'''       </td>
		'''       <td><seealso cref="XMLGregorianCalendar#setTimezone(int offset)"/><sup><em>*</em></sup>
		'''       </td>
		'''     </tr>
		'''   </tbody>
		''' </table>
		''' <p><em>*</em>conversion loss of information. It is not possible to represent
		''' a <code>java.util.GregorianCalendar</code> daylight savings timezone id in the
		''' XML Schema 1.0 date/time datatype representation.</p>
		''' 
		''' <p>To compute the return value's <code>TimeZone</code> field,
		''' <ul>
		''' <li>when <code>this.getTimezone() != FIELD_UNDEFINED</code>,
		''' create a <code>java.util.TimeZone</code> with a custom timezone id
		''' using the <code>this.getTimezone()</code>.</li>
		''' <li>else use the <code>GregorianCalendar</code> default timezone value
		''' for the host is defined as specified by
		''' <code>java.util.TimeZone.getDefault()</code>.</li></p>
		''' </summary>
		''' <param name="cal"> <code>java.util.GregorianCalendar</code> used to create <code>XMLGregorianCalendar</code>
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from <code>java.util.GregorianCalendar</code>
		''' </returns>
		''' <exception cref="NullPointerException"> If <code>cal</code> is <code>null</code>. </exception>
		Public MustOverride Function newXMLGregorianCalendar(ByVal cal As java.util.GregorianCalendar) As XMLGregorianCalendar

		''' <summary>
		''' <p>Constructor allowing for complete value spaces allowed by
		''' W3C XML Schema 1.0 recommendation for xsd:dateTime and related
		''' builtin datatypes. Note that <code>year</code> parameter supports
		''' arbitrarily large numbers and fractionalSecond has infinite
		''' precision.</p>
		''' 
		''' <p>A <code>null</code> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="year"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="month"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="day"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="hour"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="minute"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="second"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="fractionalSecond"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="timezone"> of <code>XMLGregorianCalendar</code> to be created.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from specified values.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If any individual parameter's value is outside the maximum value constraint for the field
		'''   as determined by the Date/Time Data Mapping table in <seealso cref="XMLGregorianCalendar"/>
		'''   or if the composite values constitute an invalid <code>XMLGregorianCalendar</code> instance
		'''   as determined by <seealso cref="XMLGregorianCalendar#isValid()"/>. </exception>
		Public MustOverride Function newXMLGregorianCalendar(ByVal year As System.Numerics.BigInteger, ByVal month As Integer, ByVal day As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal fractionalSecond As Decimal, ByVal timezone As Integer) As XMLGregorianCalendar

		''' <summary>
		''' <p>Constructor of value spaces that a
		''' <code>java.util.GregorianCalendar</code> instance would need to convert to an
		''' <code>XMLGregorianCalendar</code> instance.</p>
		''' 
		''' <p><code>XMLGregorianCalendar eon</code> and
		''' <code>fractionalSecond</code> are set to <code>null</code></p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="year"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="month"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="day"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="hour"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="minute"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="second"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="millisecond"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="timezone"> of <code>XMLGregorianCalendar</code> to be created.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from specified values.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If any individual parameter's value is outside the maximum value constraint for the field
		'''   as determined by the Date/Time Data Mapping table in <seealso cref="XMLGregorianCalendar"/>
		'''   or if the composite values constitute an invalid <code>XMLGregorianCalendar</code> instance
		'''   as determined by <seealso cref="XMLGregorianCalendar#isValid()"/>. </exception>
		Public Overridable Function newXMLGregorianCalendar(ByVal year As Integer, ByVal month As Integer, ByVal day As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal millisecond As Integer, ByVal timezone As Integer) As XMLGregorianCalendar

				' year may be undefined
				Dim realYear As System.Numerics.BigInteger = If(year <> DatatypeConstants.FIELD_UNDEFINED, System.Numerics.BigInteger.valueOf(CLng(year)), Nothing)

				' millisecond may be undefined
				' millisecond must be >= 0 millisecond <= 1000
				Dim realMillisecond As Decimal = Nothing ' undefined value
				If millisecond <> DatatypeConstants.FIELD_UNDEFINED Then
						If millisecond < 0 OrElse millisecond > 1000 Then Throw New System.ArgumentException("javax.xml.datatype.DatatypeFactory#newXMLGregorianCalendar(" & "int year, int month, int day, int hour, int minute, int second, int millisecond, int timezone)" & "with invalid millisecond: " & millisecond)

						realMillisecond = Decimal.valueOf(CLng(millisecond)).movePointLeft(3)
				End If

				Return newXMLGregorianCalendar(realYear, month, day, hour, minute, second, realMillisecond, timezone)
		End Function

		''' <summary>
		''' <p>Create a Java representation of XML Schema builtin datatype <code>date</code> or <code>g*</code>.</p>
		''' 
		''' <p>For example, an instance of <code>gYear</code> can be created invoking this factory
		''' with <code>month</code> and <code>day</code> parameters set to
		''' <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="year"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="month"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="day"> of <code>XMLGregorianCalendar</code> to be created. </param>
		''' <param name="timezone"> offset in minutes. <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> indicates optional field is not set.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from parameter values.
		''' </returns>
		''' <seealso cref= DatatypeConstants#FIELD_UNDEFINED
		''' </seealso>
		''' <exception cref="IllegalArgumentException"> If any individual parameter's value is outside the maximum value constraint for the field
		'''   as determined by the Date/Time Data Mapping table in <seealso cref="XMLGregorianCalendar"/>
		'''   or if the composite values constitute an invalid <code>XMLGregorianCalendar</code> instance
		'''   as determined by <seealso cref="XMLGregorianCalendar#isValid()"/>. </exception>
		Public Overridable Function newXMLGregorianCalendarDate(ByVal year As Integer, ByVal month As Integer, ByVal day As Integer, ByVal timezone As Integer) As XMLGregorianCalendar

				Return newXMLGregorianCalendar(year, month, day, DatatypeConstants.FIELD_UNDEFINED, DatatypeConstants.FIELD_UNDEFINED, DatatypeConstants.FIELD_UNDEFINED, DatatypeConstants.FIELD_UNDEFINED, timezone) ' millisecond -  second -  minute -  hour
		End Function

		''' <summary>
		''' <p>Create a Java instance of XML Schema builtin datatype <code>time</code>.</p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="hours"> number of hours </param>
		''' <param name="minutes"> number of minutes </param>
		''' <param name="seconds"> number of seconds </param>
		''' <param name="timezone"> offset in minutes. <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> indicates optional field is not set.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from parameter values.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If any individual parameter's value is outside the maximum value constraint for the field
		'''   as determined by the Date/Time Data Mapping table in <seealso cref="XMLGregorianCalendar"/>
		'''   or if the composite values constitute an invalid <code>XMLGregorianCalendar</code> instance
		'''   as determined by <seealso cref="XMLGregorianCalendar#isValid()"/>.
		''' </exception>
		''' <seealso cref= DatatypeConstants#FIELD_UNDEFINED </seealso>
		Public Overridable Function newXMLGregorianCalendarTime(ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer, ByVal timezone As Integer) As XMLGregorianCalendar

				Return newXMLGregorianCalendar(DatatypeConstants.FIELD_UNDEFINED, DatatypeConstants.FIELD_UNDEFINED, DatatypeConstants.FIELD_UNDEFINED, hours, minutes, seconds, DatatypeConstants.FIELD_UNDEFINED, timezone) 'Millisecond -  Day -  Month -  Year
		End Function

		''' <summary>
		''' <p>Create a Java instance of XML Schema builtin datatype time.</p>
		''' 
		''' <p>A <code>null</code> value indicates that field is not set.</p>
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="hours"> number of hours </param>
		''' <param name="minutes"> number of minutes </param>
		''' <param name="seconds"> number of seconds </param>
		''' <param name="fractionalSecond"> value of <code>null</code> indicates that this optional field is not set. </param>
		''' <param name="timezone"> offset in minutes. <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> indicates optional field is not set.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from parameter values.
		''' </returns>
		''' <seealso cref= DatatypeConstants#FIELD_UNDEFINED
		''' </seealso>
		''' <exception cref="IllegalArgumentException"> If any individual parameter's value is outside the maximum value constraint for the field
		'''   as determined by the Date/Time Data Mapping table in <seealso cref="XMLGregorianCalendar"/>
		'''   or if the composite values constitute an invalid <code>XMLGregorianCalendar</code> instance
		'''   as determined by <seealso cref="XMLGregorianCalendar#isValid()"/>. </exception>
		Public Overridable Function newXMLGregorianCalendarTime(ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer, ByVal fractionalSecond As Decimal, ByVal timezone As Integer) As XMLGregorianCalendar

				Return newXMLGregorianCalendar(Nothing, DatatypeConstants.FIELD_UNDEFINED, DatatypeConstants.FIELD_UNDEFINED, hours, minutes, seconds, fractionalSecond, timezone) ' day -  month -  year
		End Function

		''' <summary>
		''' <p>Create a Java instance of XML Schema builtin datatype time.</p>
		''' 
		''' <p>A <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> value indicates that field is not set.</p>
		''' </summary>
		''' <param name="hours"> number of hours </param>
		''' <param name="minutes"> number of minutes </param>
		''' <param name="seconds"> number of seconds </param>
		''' <param name="milliseconds"> number of milliseconds </param>
		''' <param name="timezone"> offset in minutes. <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> indicates optional field is not set.
		''' </param>
		''' <returns> <code>XMLGregorianCalendar</code> created from parameter values.
		''' </returns>
		''' <seealso cref= DatatypeConstants#FIELD_UNDEFINED
		''' </seealso>
		''' <exception cref="IllegalArgumentException"> If any individual parameter's value is outside the maximum value constraint for the field
		'''   as determined by the Date/Time Data Mapping table in <seealso cref="XMLGregorianCalendar"/>
		'''   or if the composite values constitute an invalid <code>XMLGregorianCalendar</code> instance
		'''   as determined by <seealso cref="XMLGregorianCalendar#isValid()"/>. </exception>
		Public Overridable Function newXMLGregorianCalendarTime(ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer, ByVal milliseconds As Integer, ByVal timezone As Integer) As XMLGregorianCalendar

				' millisecond may be undefined
				' millisecond must be >= 0 millisecond <= 1000
				Dim realMilliseconds As Decimal = Nothing ' undefined value
				If milliseconds <> DatatypeConstants.FIELD_UNDEFINED Then
						If milliseconds < 0 OrElse milliseconds > 1000 Then Throw New System.ArgumentException("javax.xml.datatype.DatatypeFactory#newXMLGregorianCalendarTime(" & "int hours, int minutes, int seconds, int milliseconds, int timezone)" & "with invalid milliseconds: " & milliseconds)

						realMilliseconds = Decimal.valueOf(CLng(milliseconds)).movePointLeft(3)
				End If

				Return newXMLGregorianCalendarTime(hours, minutes, seconds, realMilliseconds, timezone)
		End Function
	End Class

End Namespace
Imports System

'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Representation for W3C XML Schema 1.0 date/time datatypes.
	''' Specifically, these date/time datatypes are
	''' <seealso cref="DatatypeConstants#DATETIME"/>,
	''' <seealso cref="DatatypeConstants#TIME"/>,
	''' <seealso cref="DatatypeConstants#DATE"/>,
	''' <seealso cref="DatatypeConstants#GYEARMONTH"/>,
	''' <seealso cref="DatatypeConstants#GMONTHDAY"/>,
	''' <seealso cref="DatatypeConstants#GYEAR"/>,
	''' <seealso cref="DatatypeConstants#GMONTH"/>, and
	''' <seealso cref="DatatypeConstants#GDAY"/>
	''' defined in the XML Namespace
	''' <code>"http://www.w3.org/2001/XMLSchema"</code>.
	''' These datatypes are normatively defined in
	''' <a href="http://www.w3.org/TR/xmlschema-2/#dateTime">W3C XML Schema 1.0 Part 2, Section 3.2.7-14</a>.</p>
	''' 
	''' <p>The table below defines the mapping between XML Schema 1.0
	''' date/time datatype fields and this class' fields. It also summarizes
	''' the value constraints for the date and time fields defined in
	''' <a href="http://www.w3.org/TR/xmlschema-2/#isoformats">W3C XML Schema 1.0 Part 2, Appendix D,
	''' <i>ISO 8601 Date and Time Formats</i></a>.</p>
	''' 
	''' <a name="datetimefieldmapping"/>
	''' <table border="2" rules="all" cellpadding="2">
	'''   <thead>
	'''     <tr>
	'''       <th align="center" colspan="3">
	'''         Date/Time Datatype Field Mapping Between XML Schema 1.0 and Java Representation
	'''       </th>
	'''     </tr>
	'''   </thead>
	'''   <tbody>
	'''     <tr>
	'''       <th>XML Schema 1.0<br/>
	'''           datatype<br/>
	'''            field</th>
	'''       <th>Related<br/>XMLGregorianCalendar<br/>Accessor(s)</th>
	'''       <th>Value Range</th>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-year"/>year</td>
	'''       <td> <seealso cref="#getYear()"/> + <seealso cref="#getEon()"/> or<br/>
	'''            <seealso cref="#getEonAndYear"/>
	'''       </td>
	'''       <td> <code>getYear()</code> is a value between -(10^9-1) to (10^9)-1
	'''            or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.<br/>
	'''            <seealso cref="#getEon()"/> is high order year value in billion of years.<br/>
	'''            <code>getEon()</code> has values greater than or equal to (10^9) or less than or equal to -(10^9).
	'''            A value of null indicates field is undefined.</br>
	'''            Given that <a href="http://www.w3.org/2001/05/xmlschema-errata#e2-63">XML Schema 1.0 errata</a> states that the year zero
	'''            will be a valid lexical value in a future version of XML Schema,
	'''            this class allows the year field to be set to zero. Otherwise,
	'''            the year field value is handled exactly as described
	'''            in the errata and [ISO-8601-1988]. Note that W3C XML Schema 1.0
	'''            validation does not allow for the year field to have a value of zero.
	'''            </td>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-month"/>month</td>
	'''       <td> <seealso cref="#getMonth()"/> </td>
	'''       <td> 1 to 12 or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> </td>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-day"/>day</td>
	'''       <td> <seealso cref="#getDay()"/> </td>
	'''       <td> Independent of month, max range is 1 to 31 or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.<br/>
	'''            The normative value constraint stated relative to month
	'''            field's value is in <a href="http://www.w3.org/TR/xmlschema-2/#isoformats">W3C XML Schema 1.0 Part 2, Appendix D</a>.
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-hour"/>hour</td>
	'''       <td><seealso cref="#getHour()"/></td>
	'''       <td>
	'''         0 to 23 or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.
	'''         An hour value of 24 is allowed to be set in the lexical space provided the minute and second
	'''         field values are zero. However, an hour value of 24 is not allowed in value space and will be
	'''         transformed to represent the value of the first instance of the following day as per
	'''         <a href="http://www.w3.org/TR/xmlschema-2/#built-in-primitive-datatypes">
	'''         XML Schema Part 2: Datatypes Second Edition, 3.2 Primitive datatypes</a>.
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-minute"/>minute</td>
	'''       <td> <seealso cref="#getMinute()"/> </td>
	'''       <td> 0 to 59 or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> </td>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-second"/>second</td>
	'''       <td>
	'''         <seealso cref="#getSecond()"/> + <seealso cref="#getMillisecond()"/>/1000 or<br/>
	'''         <seealso cref="#getSecond()"/> + <seealso cref="#getFractionalSecond()"/>
	'''       </td>
	'''       <td>
	'''         <seealso cref="#getSecond()"/> from 0 to 60 or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.<br/>
	'''         <i>(Note: 60 only allowable for leap second.)</i><br/>
	'''         <seealso cref="#getFractionalSecond()"/> allows for infinite precision over the range from 0.0 to 1.0 when
	'''         the <seealso cref="#getSecond()"/> is defined.<br/>
	'''         <code>FractionalSecond</code> is optional and has a value of <code>null</code> when it is undefined.<br />
	'''            <seealso cref="#getMillisecond()"/> is the convenience
	'''            millisecond precision of value of <seealso cref="#getFractionalSecond()"/>.
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td><a name="datetimefield-timezone"/>timezone</td>
	'''       <td> <seealso cref="#getTimezone()"/> </td>
	'''       <td> Number of minutes or <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.
	'''         Value range from -14 hours (-14 * 60 minutes) to 14 hours (14 * 60 minutes).
	'''       </td>
	'''     </tr>
	'''   </tbody>
	'''  </table>
	''' 
	''' <p>All maximum value space constraints listed for the fields in the table
	''' above are checked by factory methods, @{link DatatypeFactory},
	''' setter methods and parse methods of
	''' this class. <code>IllegalArgumentException</code> is thrown when a
	''' parameter's value is outside the value constraint for the field or
	''' if the composite
	''' values constitute an invalid XMLGregorianCalendar instance (for example, if
	''' the 31st of June is specified).
	''' </p>
	''' 
	''' <p>The following operations are defined for this class:
	''' <ul>
	'''   <li>accessors/mutators for independent date/time fields</li>
	'''   <li>conversion between this class and W3C XML Schema 1.0 lexical representation,
	'''     <seealso cref="#toString()"/>, <seealso cref="DatatypeFactory#newXMLGregorianCalendar(String lexicalRepresentation)"/></li>
	'''   <li>conversion between this class and <seealso cref="GregorianCalendar"/>,
	'''     <seealso cref="#toGregorianCalendar(java.util.TimeZone timezone, java.util.Locale aLocale, XMLGregorianCalendar defaults)"/>,
	'''     <seealso cref="DatatypeFactory"/></li>
	'''   <li>partial order relation comparator method, <seealso cref="#compare(XMLGregorianCalendar xmlGregorianCalendar)"/></li>
	'''   <li><seealso cref="#equals(Object)"/> defined relative to <seealso cref="#compare(XMLGregorianCalendar xmlGregorianCalendar)"/>.</li>
	'''   <li>addition operation with <seealso cref="Duration"/>
	'''      instance as defined in <a href="http://www.w3.org/TR/xmlschema-2/#adding-durations-to-dateTimes">
	'''      W3C XML Schema 1.0 Part 2, Appendix E, <i>Adding durations to dateTimes</i></a>.
	'''   </li>
	''' </ul>
	''' </p>
	''' 
	''' @author <a href="mailto:Joseph.Fialli@Sun.com">Joseph Fialli</a>
	''' @author <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @author <a href="mailto:Sunitha.Reddy@Sun.com">Sunitha Reddy</a> </summary>
	''' <seealso cref= Duration </seealso>
	''' <seealso cref= DatatypeFactory
	''' @since 1.5 </seealso>

	Public MustInherit Class XMLGregorianCalendar
		Implements ICloneable

			''' <summary>
			''' Default no-arg constructor.
			''' 
			''' <p>Note: Always use the <seealso cref="DatatypeFactory"/> to
			''' construct an instance of <code>XMLGregorianCalendar</code>.
			''' The constructor on this class cannot be guaranteed to
			''' produce an object with a consistent state and may be
			''' removed in the future.</p>
			''' </summary>
			 Public Sub New()
			 End Sub

			''' <summary>
			''' <p>Unset all fields to undefined.</p>
			''' 
			''' <p>Set all int fields to <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> and reference fields
			''' to null.</p>
			''' </summary>
			Public MustOverride Sub clear()

			''' <summary>
			''' <p>Reset this <code>XMLGregorianCalendar</code> to its original values.</p>
			''' 
			''' <p><code>XMLGregorianCalendar</code> is reset to the same values as when it was created with
			''' <seealso cref="DatatypeFactory#newXMLGregorianCalendar()"/>,
			''' <seealso cref="DatatypeFactory#newXMLGregorianCalendar(String lexicalRepresentation)"/>,
			''' {@link DatatypeFactory#newXMLGregorianCalendar(
			'''   BigInteger year,
			'''   int month,
			'''   int day,
			'''   int hour,
			'''   int minute,
			'''   int second,
			'''   BigDecimal fractionalSecond,
			'''   int timezone)},
			''' {@link DatatypeFactory#newXMLGregorianCalendar(
			'''   int year,
			'''   int month,
			'''   int day,
			'''   int hour,
			'''   int minute,
			'''   int second,
			'''   int millisecond,
			'''   int timezone)},
			''' <seealso cref="DatatypeFactory#newXMLGregorianCalendar(GregorianCalendar cal)"/>,
			''' {@link DatatypeFactory#newXMLGregorianCalendarDate(
			'''   int year,
			'''   int month,
			'''   int day,
			'''   int timezone)},
			''' {@link DatatypeFactory#newXMLGregorianCalendarTime(
			'''   int hours,
			'''   int minutes,
			'''   int seconds,
			'''   int timezone)},
			''' {@link DatatypeFactory#newXMLGregorianCalendarTime(
			'''   int hours,
			'''   int minutes,
			'''   int seconds,
			'''   BigDecimal fractionalSecond,
			'''   int timezone)} or
			''' {@link DatatypeFactory#newXMLGregorianCalendarTime(
			'''   int hours,
			'''   int minutes,
			'''   int seconds,
			'''   int milliseconds,
			'''   int timezone)}.
			''' </p>
			''' 
			''' <p><code>reset()</code> is designed to allow the reuse of existing <code>XMLGregorianCalendar</code>s
			''' thus saving resources associated with the creation of new <code>XMLGregorianCalendar</code>s.</p>
			''' </summary>
			Public MustOverride Sub reset()

		''' <summary>
		''' <p>Set low and high order component of XSD <code>dateTime</code> year field.</p>
		''' 
		''' <p>Unset this field by invoking the setter with a parameter value of <code>null</code>.</p>
		''' </summary>
		''' <param name="year"> value constraints summarized in <a href="#datetimefield-year">year field of date/time field mapping table</a>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>year</code> parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public MustOverride Property year As System.Numerics.BigInteger

		''' <summary>
		''' <p>Set year of XSD <code>dateTime</code> year field.</p>
		''' 
		''' <p>Unset this field by invoking the setter with a parameter value of
		''' <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
		''' 
		''' <p>Note: if the absolute value of the <code>year</code> parameter
		''' is less than 10^9, the eon component of the XSD year field is set to
		''' <code>null</code> by this method.</p>
		''' </summary>
		''' <param name="year"> value constraints are summarized in <a href="#datetimefield-year">year field of date/time field mapping table</a>.
		'''   If year is <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>, then eon is set to <code>null</code>. </param>
		Public MustOverride WriteOnly Property year As Integer

		''' <summary>
		''' <p>Set month.</p>
		''' 
		''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
		''' </summary>
		''' <param name="month"> value constraints summarized in <a href="#datetimefield-month">month field of date/time field mapping table</a>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>month</code> parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public MustOverride Property month As Integer

		''' <summary>
		''' <p>Set days in month.</p>
		''' 
		''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
		''' </summary>
		''' <param name="day"> value constraints summarized in <a href="#datetimefield-day">day field of date/time field mapping table</a>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>day</code> parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public MustOverride Property day As Integer

		''' <summary>
		''' <p>Set the number of minutes in the timezone offset.</p>
		''' 
		''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
		''' </summary>
		''' <param name="offset"> value constraints summarized in <a href="#datetimefield-timezone">
		'''   timezone field of date/time field mapping table</a>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>offset</code> parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public MustOverride Property timezone As Integer

		''' <summary>
		''' <p>Set time as one unit.</p>
		''' </summary>
		''' <param name="hour"> value constraints are summarized in
		''' <a href="#datetimefield-hour">hour field of date/time field mapping table</a>. </param>
		''' <param name="minute"> value constraints are summarized in
		''' <a href="#datetimefield-minute">minute field of date/time field mapping table</a>. </param>
		''' <param name="second"> value constraints are summarized in
		''' <a href="#datetimefield-second">second field of date/time field mapping table</a>.
		''' </param>
		''' <seealso cref= #setTime(int, int, int, BigDecimal)
		''' </seealso>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public Overridable Sub setTime(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer)

					timeime(hour, minute, second, Nothing) ' fractional
		End Sub

			''' <summary>
			''' <p>Set hours.</p>
			''' 
			''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
			''' </summary>
			''' <param name="hour"> value constraints summarized in <a href="#datetimefield-hour">hour field of date/time field mapping table</a>.
			''' </param>
			''' <exception cref="IllegalArgumentException"> if <code>hour</code> parameter is outside value constraints for the field as specified in
			'''   <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
			Public MustOverride Property hour As Integer

			''' <summary>
			''' <p>Set minutes.</p>
			''' 
			''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
			''' </summary>
			''' <param name="minute"> value constraints summarized in <a href="#datetimefield-minute">minute field of date/time field mapping table</a>.
			''' </param>
			''' <exception cref="IllegalArgumentException"> if <code>minute</code> parameter is outside value constraints for the field as specified in
			'''   <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
			Public MustOverride Property minute As Integer

			''' <summary>
			''' <p>Set seconds.</p>
			''' 
			''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
			''' </summary>
			''' <param name="second"> value constraints summarized in <a href="#datetimefield-second">second field of date/time field mapping table</a>.
			''' </param>
			''' <exception cref="IllegalArgumentException"> if <code>second</code> parameter is outside value constraints for the field as specified in
			'''   <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
			Public MustOverride Property second As Integer

			''' <summary>
			''' <p>Set milliseconds.</p>
			''' 
			''' <p>Unset this field by invoking the setter with a parameter value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
			''' </summary>
			''' <param name="millisecond"> value constraints summarized in
			'''   <a href="#datetimefield-second">second field of date/time field mapping table</a>.
			''' </param>
			''' <exception cref="IllegalArgumentException"> if <code>millisecond</code> parameter is outside value constraints for the field as specified
			'''   in <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
			Public MustOverride WriteOnly Property millisecond As Integer

			''' <summary>
			''' <p>Set fractional seconds.</p>
			''' 
			''' <p>Unset this field by invoking the setter with a parameter value of <code>null</code>.</p>
			''' </summary>
			''' <param name="fractional"> value constraints summarized in
			'''   <a href="#datetimefield-second">second field of date/time field mapping table</a>.
			''' </param>
			''' <exception cref="IllegalArgumentException"> if <code>fractional</code> parameter is outside value constraints for the field as specified
			'''   in <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
			Public MustOverride Property fractionalSecond As Decimal


		''' <summary>
		''' <p>Set time as one unit, including the optional infinite precision
		''' fractional seconds.</p>
		''' </summary>
		''' <param name="hour"> value constraints are summarized in
		''' <a href="#datetimefield-hour">hour field of date/time field mapping table</a>. </param>
		''' <param name="minute"> value constraints are summarized in
		''' <a href="#datetimefield-minute">minute field of date/time field mapping table</a>. </param>
		''' <param name="second"> value constraints are summarized in
		''' <a href="#datetimefield-second">second field of date/time field mapping table</a>. </param>
		''' <param name="fractional"> value of <code>null</code> indicates this optional
		'''   field is not set.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public Overridable Sub setTime(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal fractional As Decimal)

					hour = hour
			minute = minute
			second = second
			fractionalSecond = fractional
		End Sub


		''' <summary>
		''' <p>Set time as one unit, including optional milliseconds.</p>
		''' </summary>
		''' <param name="hour"> value constraints are summarized in
		''' <a href="#datetimefield-hour">hour field of date/time field mapping table</a>. </param>
		''' <param name="minute"> value constraints are summarized in
		''' <a href="#datetimefield-minute">minute field of date/time field mapping table</a>. </param>
		''' <param name="second"> value constraints are summarized in
		''' <a href="#datetimefield-second">second field of date/time field mapping table</a>. </param>
		''' <param name="millisecond"> value of <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/> indicates this
		'''                    optional field is not set.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' outside value constraints for the field as specified in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>. </exception>
		Public Overridable Sub setTime(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal millisecond As Integer)

			hour = hour
			minute = minute
			second = second
			millisecond = millisecond
		End Sub

			''' <summary>
			''' <p>Return high order component for XML Schema 1.0 dateTime datatype field for
			''' <code>year</code>.
			''' <code>null</code> if this optional part of the year field is not defined.</p>
			''' 
			''' <p>Value constraints for this value are summarized in
			''' <a href="#datetimefield-year">year field of date/time field mapping table</a>.</p> </summary>
			''' <returns> eon of this <code>XMLGregorianCalendar</code>. The value
			''' returned is an integer multiple of 10^9.
			''' </returns>
			''' <seealso cref= #getYear() </seealso>
			''' <seealso cref= #getEonAndYear() </seealso>
			Public MustOverride ReadOnly Property eon As System.Numerics.BigInteger


			''' <summary>
			''' <p>Return XML Schema 1.0 dateTime datatype field for
			''' <code>year</code>.</p>
			''' 
			''' <p>Value constraints for this value are summarized in
			''' <a href="#datetimefield-year">year field of date/time field mapping table</a>.</p>
			''' </summary>
			''' <returns> sum of <code>eon</code> and <code>BigInteger.valueOf(year)</code>
			''' when both fields are defined. When only <code>year</code> is defined,
			''' return it. When both <code>eon</code> and <code>year</code> are not
			''' defined, return <code>null</code>.
			''' </returns>
			''' <seealso cref= #getEon() </seealso>
			''' <seealso cref= #getYear() </seealso>
			Public MustOverride ReadOnly Property eonAndYear As System.Numerics.BigInteger







			''' <summary>
			''' <p>Return millisecond precision of <seealso cref="#getFractionalSecond()"/>.</p>
			''' 
			''' <p>This method represents a convenience accessor to infinite
			''' precision fractional second value returned by
			''' <seealso cref="#getFractionalSecond()"/>. The returned value is the rounded
			''' down to milliseconds value of
			''' <seealso cref="#getFractionalSecond()"/>. When <seealso cref="#getFractionalSecond()"/>
			''' returns <code>null</code>, this method must return
			''' <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.</p>
			''' 
			''' <p>Value constraints for this value are summarized in
			''' <a href="#datetimefield-second">second field of date/time field mapping table</a>.</p>
			''' </summary>
			''' <returns> Millisecond  of this <code>XMLGregorianCalendar</code>.
			''' </returns>
			''' <seealso cref= #getFractionalSecond() </seealso>
			''' <seealso cref= #setTime(int, int, int) </seealso>
			Public Overridable Property millisecond As Integer
				Get
    
						Dim fractionalSeconds As Decimal = fractionalSecond
    
						' is field undefined?
						If fractionalSeconds Is Nothing Then Return DatatypeConstants.FIELD_UNDEFINED
    
						Return fractionalSecond.movePointRight(3)
				End Get
			End Property


		' comparisons
		''' <summary>
		''' <p>Compare two instances of W3C XML Schema 1.0 date/time datatypes
		''' according to partial order relation defined in
		''' <a href="http://www.w3.org/TR/xmlschema-2/#dateTime-order">W3C XML Schema 1.0 Part 2, Section 3.2.7.3,
		''' <i>Order relation on dateTime</i></a>.</p>
		''' 
		''' <p><code>xsd:dateTime</code> datatype field mapping to accessors of
		''' this class are defined in
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>.</p>
		''' </summary>
		''' <param name="xmlGregorianCalendar"> Instance of <code>XMLGregorianCalendar</code> to compare
		''' </param>
		''' <returns> The relationship between <code>this</code> <code>XMLGregorianCalendar</code> and
		'''   the specified <code>xmlGregorianCalendar</code> as
		'''   <seealso cref="DatatypeConstants#LESSER"/>,
		'''   <seealso cref="DatatypeConstants#EQUAL"/>,
		'''   <seealso cref="DatatypeConstants#GREATER"/> or
		'''   <seealso cref="DatatypeConstants#INDETERMINATE"/>.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>xmlGregorianCalendar</code> is null. </exception>
		Public MustOverride Function compare(ByVal xmlGregorianCalendar As XMLGregorianCalendar) As Integer

		''' <summary>
		''' <p>Normalize this instance to UTC.</p>
		''' 
		''' <p>2000-03-04T23:00:00+03:00 normalizes to 2000-03-04T20:00:00Z</p>
		''' <p>Implements W3C XML Schema Part 2, Section 3.2.7.3 (A).</p>
		''' </summary>
		''' <returns> <code>this</code> <code>XMLGregorianCalendar</code> normalized to UTC. </returns>
		Public MustOverride Function normalize() As XMLGregorianCalendar

		''' <summary>
		''' <p>Compares this calendar to the specified object. The result is
		''' <code>true</code> if and only if the argument is not null and is an
		''' <code>XMLGregorianCalendar</code> object that represents the same
		''' instant in time as this object.</p>
		''' </summary>
		''' <param name="obj"> to compare.
		''' </param>
		''' <returns> <code>true</code> when <code>obj</code> is an instance of
		''' <code>XMLGregorianCalendar</code> and
		''' <seealso cref="#compare(XMLGregorianCalendar obj)"/>
		''' returns <seealso cref="DatatypeConstants#EQUAL"/>,
		''' otherwise <code>false</code>. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			If obj Is Nothing OrElse Not(TypeOf obj Is XMLGregorianCalendar) Then Return False
			Return compare(CType(obj, XMLGregorianCalendar)) = DatatypeConstants.EQUAL
		End Function

		''' <summary>
		''' <p>Returns a hash code consistent with the definition of the equals method.</p>
		''' </summary>
		''' <returns> hash code of this object. </returns>
		Public Overrides Function GetHashCode() As Integer

			' Following two dates compare to EQUALS since in different timezones.
			' 2000-01-15T12:00:00-05:00 == 2000-01-15T13:00:00-04:00
			'
			' Must ensure both instances generate same hashcode by normalizing
			' this to UTC timezone.
			Dim ___timezone As Integer = timezone
			If ___timezone = DatatypeConstants.FIELD_UNDEFINED Then ___timezone = 0
			Dim gc As XMLGregorianCalendar = Me
			If ___timezone <> 0 Then gc = Me.normalize()
			Return gc.year + gc.month + gc.day + gc.hour + gc.minute + gc.second
		End Function

		''' <summary>
		''' <p>Return the lexical representation of <code>this</code> instance.
		''' The format is specified in
		''' <a href="http://www.w3.org/TR/xmlschema-2/#dateTime-order">XML Schema 1.0 Part 2, Section 3.2.[7-14].1,
		''' <i>Lexical Representation</i>".</a></p>
		''' 
		''' <p>Specific target lexical representation format is determined by
		''' <seealso cref="#getXMLSchemaType()"/>.</p>
		''' </summary>
		''' <returns> XML, as <code>String</code>, representation of this <code>XMLGregorianCalendar</code>
		''' </returns>
		''' <exception cref="IllegalStateException"> if the combination of set fields
		'''    does not match one of the eight defined XML Schema builtin date/time datatypes. </exception>
		Public MustOverride Function toXMLFormat() As String

		''' <summary>
		''' <p>Return the name of the XML Schema date/time type that this instance
		''' maps to. Type is computed based on fields that are set.</p>
		''' 
		''' <table border="2" rules="all" cellpadding="2">
		'''   <thead>
		'''     <tr>
		'''       <th align="center" colspan="7">
		'''         Required fields for XML Schema 1.0 Date/Time Datatypes.<br/>
		'''         <i>(timezone is optional for all date/time datatypes)</i>
		'''       </th>
		'''     </tr>
		'''   </thead>
		'''   <tbody>
		'''     <tr>
		'''       <td>Datatype</td>
		'''       <td>year</td>
		'''       <td>month</td>
		'''       <td>day</td>
		'''       <td>hour</td>
		'''       <td>minute</td>
		'''       <td>second</td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#DATETIME"/></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#DATE"/></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#TIME"/></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#GYEARMONTH"/></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#GMONTHDAY"/></td>
		'''       <td></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#GYEAR"/></td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#GMONTH"/></td>
		'''       <td></td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#GDAY"/></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''   </tbody>
		''' </table>
		''' </summary>
		''' <exception cref="java.lang.IllegalStateException"> if the combination of set fields
		'''    does not match one of the eight defined XML Schema builtin
		'''    date/time datatypes. </exception>
		''' <returns> One of the following class constants:
		'''   <seealso cref="DatatypeConstants#DATETIME"/>,
		'''   <seealso cref="DatatypeConstants#TIME"/>,
		'''   <seealso cref="DatatypeConstants#DATE"/>,
		'''   <seealso cref="DatatypeConstants#GYEARMONTH"/>,
		'''   <seealso cref="DatatypeConstants#GMONTHDAY"/>,
		'''   <seealso cref="DatatypeConstants#GYEAR"/>,
		'''   <seealso cref="DatatypeConstants#GMONTH"/> or
		'''   <seealso cref="DatatypeConstants#GDAY"/>. </returns>
		Public MustOverride ReadOnly Property xMLSchemaType As javax.xml.namespace.QName

			''' <summary>
			''' <p>Returns a <code>String</code> representation of this <code>XMLGregorianCalendar</code> <code>Object</code>.</p>
			''' 
			''' <p>The result is a lexical representation generated by <seealso cref="#toXMLFormat()"/>.</p>
			''' </summary>
			''' <returns> A non-<code>null</code> valid <code>String</code> representation of this <code>XMLGregorianCalendar</code>.
			''' </returns>
			''' <exception cref="IllegalStateException"> if the combination of set fields
			'''    does not match one of the eight defined XML Schema builtin date/time datatypes.
			''' </exception>
			''' <seealso cref= #toXMLFormat() </seealso>
		Public Overrides Function ToString() As String

			Return toXMLFormat()
		End Function

		''' <summary>
		''' Validate instance by <code>getXMLSchemaType()</code> constraints. </summary>
		''' <returns> true if data values are valid. </returns>
		Public MustOverride ReadOnly Property valid As Boolean

		''' <summary>
		''' <p>Add <code>duration</code> to this instance.</p>
		''' 
		''' <p>The computation is specified in
		''' <a href="http://www.w3.org/TR/xmlschema-2/#adding-durations-to-dateTimes">XML Schema 1.0 Part 2, Appendix E,
		''' <i>Adding durations to dateTimes</i>></a>.
		''' <a href="#datetimefieldmapping">date/time field mapping table</a>
		''' defines the mapping from XML Schema 1.0 <code>dateTime</code> fields
		''' to this class' representation of those fields.</p>
		''' </summary>
		''' <param name="duration"> Duration to add to this <code>XMLGregorianCalendar</code>.
		''' </param>
		''' <exception cref="NullPointerException">  when <code>duration</code> parameter is <code>null</code>. </exception>
		Public MustOverride Sub add(ByVal duration As Duration)

		''' <summary>
		''' <p>Convert this <code>XMLGregorianCalendar</code> to a <seealso cref="GregorianCalendar"/>.</p>
		''' 
		''' <p>When <code>this</code> instance has an undefined field, this
		''' conversion relies on the <code>java.util.GregorianCalendar</code> default
		''' for its corresponding field. A notable difference between
		''' XML Schema 1.0 date/time datatypes and <code>java.util.GregorianCalendar</code>
		''' is that Timezone value is optional for date/time datatypes and it is
		''' a required field for <code>java.util.GregorianCalendar</code>. See javadoc
		''' for <code>java.util.TimeZone.getDefault()</code> on how the default
		''' is determined. To explicitly specify the <code>TimeZone</code>
		''' instance, see
		''' <seealso cref="#toGregorianCalendar(TimeZone, Locale, XMLGregorianCalendar)"/>.</p>
		''' 
		''' <table border="2" rules="all" cellpadding="2">
		'''   <thead>
		'''     <tr>
		'''       <th align="center" colspan="2">
		'''          Field by Field Conversion from this class to
		'''          <code>java.util.GregorianCalendar</code>
		'''       </th>
		'''     </tr>
		'''   </thead>
		'''   <tbody>
		'''     <tr>
		'''        <td><code>java.util.GregorianCalendar</code> field</td>
		'''        <td><code>javax.xml.datatype.XMLGregorianCalendar</code> field</td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>ERA</code></td>
		'''       <td><seealso cref="#getEonAndYear()"/><code>.signum() < 0 ? GregorianCalendar.BC : GregorianCalendar.AD</code></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>YEAR</code></td>
		'''       <td><seealso cref="#getEonAndYear()"/><code>.abs().intValue()</code><i>*</i></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>MONTH</code></td>
		'''       <td><seealso cref="#getMonth()"/> - <seealso cref="DatatypeConstants#JANUARY"/> + <seealso cref="GregorianCalendar#JANUARY"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>DAY_OF_MONTH</code></td>
		'''       <td><seealso cref="#getDay()"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>HOUR_OF_DAY</code></td>
		'''       <td><seealso cref="#getHour()"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>MINUTE</code></td>
		'''       <td><seealso cref="#getMinute()"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>SECOND</code></td>
		'''       <td><seealso cref="#getSecond()"/></td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>MILLISECOND</code></td>
		'''       <td>get millisecond order from <seealso cref="#getFractionalSecond()"/><i>*</i> </td>
		'''     </tr>
		'''     <tr>
		'''       <td><code>GregorianCalendar.setTimeZone(TimeZone)</code></td>
		'''       <td><seealso cref="#getTimezone()"/> formatted into Custom timezone id</td>
		'''     </tr>
		'''   </tbody>
		''' </table>
		''' <i>*</i> designates possible loss of precision during the conversion due
		''' to source datatype having higher precision than target datatype.
		''' 
		''' <p>To ensure consistency in conversion implementations, the new
		''' <code>GregorianCalendar</code> should be instantiated in following
		''' manner.
		''' <ul>
		'''   <li>Using <code>timeZone</code> value as defined above, create a new
		''' <code>java.util.GregorianCalendar(timeZone,Locale.getDefault())</code>.
		'''   </li>
		'''   <li>Initialize all GregorianCalendar fields by calling <seealso cref="java.util.GregorianCalendar#clear()"/>.</li>
		'''   <li>Obtain a pure Gregorian Calendar by invoking
		'''   <code>GregorianCalendar.setGregorianChange(
		'''   new Date(Long.MIN_VALUE))</code>.</li>
		'''   <li>Its fields ERA, YEAR, MONTH, DAY_OF_MONTH, HOUR_OF_DAY,
		'''       MINUTE, SECOND and MILLISECOND are set using the method
		'''       <code>Calendar.set(int,int)</code></li>
		''' </ul>
		''' </p>
		''' </summary>
		''' <seealso cref= #toGregorianCalendar(java.util.TimeZone, java.util.Locale, XMLGregorianCalendar) </seealso>
		Public MustOverride Function toGregorianCalendar() As java.util.GregorianCalendar

		''' <summary>
		''' <p>Convert this <code>XMLGregorianCalendar</code> along with provided parameters
		''' to a <seealso cref="GregorianCalendar"/> instance.</p>
		''' 
		''' <p> Since XML Schema 1.0 date/time datetypes has no concept of
		''' timezone ids or daylight savings timezone ids, this conversion operation
		''' allows the user to explicitly specify one with
		''' <code>timezone</code> parameter.</p>
		''' 
		''' <p>To compute the return value's <code>TimeZone</code> field,
		''' <ul>
		''' <li>when parameter <code>timeZone</code> is non-null,
		''' it is the timezone field.</li>
		''' <li>else when <code>this.getTimezone() != FIELD_UNDEFINED</code>,
		''' create a <code>java.util.TimeZone</code> with a custom timezone id
		''' using the <code>this.getTimezone()</code>.</li>
		''' <li>else when <code>defaults.getTimezone() != FIELD_UNDEFINED</code>,
		''' create a <code>java.util.TimeZone</code> with a custom timezone id
		''' using <code>defaults.getTimezone()</code>.</li>
		''' <li>else use the <code>GregorianCalendar</code> default timezone value
		''' for the host is defined as specified by
		''' <code>java.util.TimeZone.getDefault()</code>.</li></p>
		''' 
		''' <p>To ensure consistency in conversion implementations, the new
		''' <code>GregorianCalendar</code> should be instantiated in following
		''' manner.
		''' <ul>
		'''   <li>Create a new <code>java.util.GregorianCalendar(TimeZone,
		'''       Locale)</code> with TimeZone set as specified above and the
		'''       <code>Locale</code> parameter.
		'''   </li>
		'''   <li>Initialize all GregorianCalendar fields by calling <seealso cref="GregorianCalendar#clear()"/></li>
		'''   <li>Obtain a pure Gregorian Calendar by invoking
		'''   <code>GregorianCalendar.setGregorianChange(
		'''   new Date(Long.MIN_VALUE))</code>.</li>
		'''   <li>Its fields ERA, YEAR, MONTH, DAY_OF_MONTH, HOUR_OF_DAY,
		'''       MINUTE, SECOND and MILLISECOND are set using the method
		'''       <code>Calendar.set(int,int)</code></li>
		''' </ul>
		''' </summary>
		''' <param name="timezone"> provide Timezone. <code>null</code> is a legal value. </param>
		''' <param name="aLocale">  provide explicit Locale. Use default GregorianCalendar locale if
		'''                 value is <code>null</code>. </param>
		''' <param name="defaults"> provide default field values to use when corresponding
		'''                 field for this instance is FIELD_UNDEFINED or null.
		'''                 If <code>defaults</code>is <code>null</code> or a field
		'''                 within the specified <code>defaults</code> is undefined,
		'''                 just use <code>java.util.GregorianCalendar</code> defaults. </param>
		''' <returns> a java.util.GregorianCalendar conversion of this instance. </returns>
		Public MustOverride Function toGregorianCalendar(ByVal timezone As java.util.TimeZone, ByVal aLocale As java.util.Locale, ByVal defaults As XMLGregorianCalendar) As java.util.GregorianCalendar

		''' <summary>
		''' <p>Returns a <code>java.util.TimeZone</code> for this class.</p>
		''' 
		''' <p>If timezone field is defined for this instance,
		''' returns TimeZone initialized with custom timezone id
		''' of zoneoffset. If timezone field is undefined,
		''' try the defaultZoneoffset that was passed in.
		''' If defaultZoneoffset is FIELD_UNDEFINED, return
		''' default timezone for this host.
		''' (Same default as java.util.GregorianCalendar).</p>
		''' </summary>
		''' <param name="defaultZoneoffset"> default zoneoffset if this zoneoffset is
		''' <seealso cref="DatatypeConstants#FIELD_UNDEFINED"/>.
		''' </param>
		''' <returns> TimeZone for this. </returns>
		Public MustOverride Function getTimeZone(ByVal defaultZoneoffset As Integer) As java.util.TimeZone



		''' <summary>
		''' <p>Creates and returns a copy of this object.</p>
		''' </summary>
		''' <returns> copy of this <code>Object</code> </returns>
	   Public MustOverride Function clone() As Object
	End Class

End Namespace
'
' * Copyright (c) 2004, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Utility class to contain basic Datatype values as constants.</p>
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @since 1.5
	''' </summary>

	Public NotInheritable Class DatatypeConstants

		''' <summary>
		''' <p>Private constructor to prevent instantiation.</p>
		''' </summary>
			Private Sub New()
			End Sub

			''' <summary>
			''' Value for first month of year.
			''' </summary>
			Public Const JANUARY As Integer = 1

			''' <summary>
			''' Value for second month of year.
			''' </summary>
			Public Const FEBRUARY As Integer = 2

			''' <summary>
			''' Value for third month of year.
			''' </summary>
			Public Const MARCH As Integer = 3

			''' <summary>
			''' Value for fourth month of year.
			''' </summary>
			Public Const APRIL As Integer = 4

			''' <summary>
			''' Value for fifth month of year.
			''' </summary>
			Public Const MAY As Integer = 5

			''' <summary>
			''' Value for sixth month of year.
			''' </summary>
			Public Const JUNE As Integer = 6

			''' <summary>
			''' Value for seventh month of year.
			''' </summary>
			Public Const JULY As Integer = 7

			''' <summary>
			''' Value for eighth month of year.
			''' </summary>
			Public Const AUGUST As Integer = 8

			''' <summary>
			''' Value for ninth month of year.
			''' </summary>
			Public Const SEPTEMBER As Integer = 9

			''' <summary>
			''' Value for tenth month of year.
			''' </summary>
			Public Const OCTOBER As Integer = 10

			''' <summary>
			''' Value for eleven month of year.
			''' </summary>
			Public Const NOVEMBER As Integer = 11

			''' <summary>
			''' Value for twelve month of year.
			''' </summary>
			Public Const DECEMBER As Integer = 12

			''' <summary>
			''' <p>Comparison result.</p>
			''' </summary>
			Public Const LESSER As Integer = -1

			''' <summary>
			''' <p>Comparison result.</p>
			''' </summary>
			Public Const EQUAL As Integer = 0

			''' <summary>
			''' <p>Comparison result.</p>
			''' </summary>
			Public Const GREATER As Integer = 1

			''' <summary>
			''' <p>Comparison result.</p>
			''' </summary>
			Public Const INDETERMINATE As Integer = 2

			''' <summary>
			''' Designation that an "int" field is not set.
			''' </summary>
			Public Shared ReadOnly FIELD_UNDEFINED As Integer = Integer.MIN_VALUE

			''' <summary>
			''' <p>A constant that represents the years field.</p>
			''' </summary>
			Public Shared ReadOnly YEARS As New Field("YEARS", 0)

			''' <summary>
			''' <p>A constant that represents the months field.</p>
			''' </summary>
			Public Shared ReadOnly MONTHS As New Field("MONTHS", 1)

			''' <summary>
			''' <p>A constant that represents the days field.</p>
			''' </summary>
			Public Shared ReadOnly DAYS As New Field("DAYS", 2)

			''' <summary>
			''' <p>A constant that represents the hours field.</p>
			''' </summary>
			Public Shared ReadOnly HOURS As New Field("HOURS", 3)

			''' <summary>
			''' <p>A constant that represents the minutes field.</p>
			''' </summary>
			Public Shared ReadOnly MINUTES As New Field("MINUTES", 4)

			''' <summary>
			''' <p>A constant that represents the seconds field.</p>
			''' </summary>
			Public Shared ReadOnly SECONDS As New Field("SECONDS", 5)

			''' <summary>
			''' Type-safe enum class that represents six fields
			''' of the <seealso cref="Duration"/> class.
			''' @since 1.5
			''' </summary>
			Public NotInheritable Class Field

					''' <summary>
					''' <p><code>String</code> representation of <code>Field</code>.</p>
					''' </summary>
					Private ReadOnly str As String
					''' <summary>
					''' <p>Unique id of the field.</p>
					''' 
					''' <p>This value allows the <seealso cref="Duration"/> class to use switch
					''' statements to process fields.</p>
					''' </summary>
					Private ReadOnly id As Integer

					''' <summary>
					''' <p>Construct a <code>Field</code> with specified values.</p> </summary>
					''' <param name="str"> <code>String</code> representation of <code>Field</code> </param>
					''' <param name="id">  <code>int</code> representation of <code>Field</code> </param>
					Private Sub New(ByVal str As String, ByVal id As Integer)
							Me.str = str
							Me.id = id
					End Sub
					''' <summary>
					''' Returns a field name in English. This method
					''' is intended to be used for debugging/diagnosis
					''' and not for display to end-users.
					''' 
					''' @return
					'''      a non-null valid String constant.
					''' </summary>
					Public Overrides Function ToString() As String
						Return str
					End Function

					''' <summary>
					''' <p>Get id of this Field.</p>
					''' </summary>
					''' <returns> Id of field. </returns>
					Public Property id As Integer
						Get
								Return id
						End Get
					End Property
			End Class

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>dateTime</code>.</p>
			''' </summary>
			Public Shared ReadOnly DATETIME As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "dateTime")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>time</code>.</p>
			''' </summary>
			Public Shared ReadOnly TIME As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "time")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>date</code>.</p>
			''' </summary>
			Public Shared ReadOnly [DATE] As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "date")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>gYearMonth</code>.</p>
			''' </summary>
			Public Shared ReadOnly GYEARMONTH As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "gYearMonth")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>gMonthDay</code>.</p>
			''' </summary>
			Public Shared ReadOnly GMONTHDAY As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "gMonthDay")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>gYear</code>.</p>
			''' </summary>
			Public Shared ReadOnly GYEAR As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "gYear")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>gMonth</code>.</p>
			''' </summary>
			Public Shared ReadOnly GMONTH As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "gMonth")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema 1.0 datatype <code>gDay</code>.</p>
			''' </summary>
			Public Shared ReadOnly GDAY As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "gDay")

			''' <summary>
			''' <p>Fully qualified name for W3C XML Schema datatype <code>duration</code>.</p>
			''' </summary>
			Public Shared ReadOnly DURATION As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XML_SCHEMA_NS_URI, "duration")

			''' <summary>
			''' <p>Fully qualified name for XQuery 1.0 and XPath 2.0 datatype <code>dayTimeDuration</code>.</p>
			''' </summary>
			Public Shared ReadOnly DURATION_DAYTIME As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XPATH_DATATYPE_NS_URI, "dayTimeDuration")

			''' <summary>
			''' <p>Fully qualified name for XQuery 1.0 and XPath 2.0 datatype <code>yearMonthDuration</code>.</p>
			''' </summary>
			Public Shared ReadOnly DURATION_YEARMONTH As New javax.xml.namespace.QName(javax.xml.XMLConstants.W3C_XPATH_DATATYPE_NS_URI, "yearMonthDuration")

			''' <summary>
			''' W3C XML Schema max timezone offset is -14:00. Zone offset is in minutes.
			''' </summary>
			Public Const MAX_TIMEZONE_OFFSET As Integer = -14 * 60

			''' <summary>
			''' W3C XML Schema min timezone offset is +14:00. Zone offset is in minutes.
			''' </summary>
			Public Const MIN_TIMEZONE_OFFSET As Integer = 14 * 60

	End Class

End Namespace
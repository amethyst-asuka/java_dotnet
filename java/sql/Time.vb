Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.sql


	''' <summary>
	''' <P>A thin wrapper around the <code>java.util.Date</code> class that allows the JDBC
	''' API to identify this as an SQL <code>TIME</code> value. The <code>Time</code>
	''' class adds formatting and
	''' parsing operations to support the JDBC escape syntax for time
	''' values.
	''' <p>The date components should be set to the "zero epoch"
	''' value of January 1, 1970 and should not be accessed.
	''' </summary>
	Public Class Time
		Inherits DateTime?

		''' <summary>
		''' Constructs a <code>Time</code> object initialized with the
		''' given values for the hour, minute, and second.
		''' The driver sets the date components to January 1, 1970.
		''' Any method that attempts to access the date components of a
		''' <code>Time</code> object will throw a
		''' <code>java.lang.IllegalArgumentException</code>.
		''' <P>
		''' The result is undefined if a given argument is out of bounds.
		''' </summary>
		''' <param name="hour"> 0 to 23 </param>
		''' <param name="minute"> 0 to 59 </param>
		''' <param name="second"> 0 to 59
		''' </param>
		''' @deprecated Use the constructor that takes a milliseconds value
		'''             in place of this constructor 
		<Obsolete("Use the constructor that takes a milliseconds value")> _
		Public Sub New(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer)
			MyBase.New(70, 0, 1, hour, minute, second)
		End Sub

		''' <summary>
		''' Constructs a <code>Time</code> object using a milliseconds time value.
		''' </summary>
		''' <param name="time"> milliseconds since January 1, 1970, 00:00:00 GMT;
		'''             a negative number is milliseconds before
		'''               January 1, 1970, 00:00:00 GMT </param>
		Public Sub New(ByVal time_Renamed As Long)
			MyBase.New(time_Renamed)
		End Sub

		''' <summary>
		''' Sets a <code>Time</code> object using a milliseconds time value.
		''' </summary>
		''' <param name="time"> milliseconds since January 1, 1970, 00:00:00 GMT;
		'''             a negative number is milliseconds before
		'''               January 1, 1970, 00:00:00 GMT </param>
		Public Overrides Property time As Long
			Set(ByVal time_Renamed As Long)
				MyBase.time = time_Renamed
			End Set
		End Property

		''' <summary>
		''' Converts a string in JDBC time escape format to a <code>Time</code> value.
		''' </summary>
		''' <param name="s"> time in format "hh:mm:ss" </param>
		''' <returns> a corresponding <code>Time</code> object </returns>
		Public Shared Function valueOf(ByVal s As String) As Time
			Dim hour As Integer
			Dim minute As Integer
			Dim second As Integer
			Dim firstColon As Integer
			Dim secondColon As Integer

			If s Is Nothing Then Throw New System.ArgumentException

			firstColon = s.IndexOf(":"c)
			secondColon = s.IndexOf(":"c, firstColon+1)
			If (firstColon > 0) And (secondColon > 0) And (secondColon < s.length()-1) Then
				hour = Convert.ToInt32(s.Substring(0, firstColon))
				minute = Convert.ToInt32(s.Substring(firstColon+1, secondColon - (firstColon+1)))
				second = Convert.ToInt32(s.Substring(secondColon+1))
			Else
				Throw New System.ArgumentException
			End If

			Return New Time(hour, minute, second)
		End Function

		''' <summary>
		''' Formats a time in JDBC time escape format.
		''' </summary>
		''' <returns> a <code>String</code> in hh:mm:ss format </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function ToString() As String
			Dim hour As Integer = MyBase.hours
			Dim minute As Integer = MyBase.minutes
			Dim second As Integer = MyBase.seconds
			Dim hourString As String
			Dim minuteString As String
			Dim secondString As String

			If hour < 10 Then
				hourString = "0" & hour
			Else
				hourString = Convert.ToString(hour)
			End If
			If minute < 10 Then
				minuteString = "0" & minute
			Else
				minuteString = Convert.ToString(minute)
			End If
			If second < 10 Then
				secondString = "0" & second
			Else
				secondString = Convert.ToString(second)
			End If
			Return (hourString & ":" & minuteString & ":" & secondString)
		End Function

		' Override all the date operations inherited from java.util.Date;

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL <code>TIME</code>
	   ''' values do not have a year component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this
	   '''           method is invoked </exception>
	   ''' <seealso cref= #setYear </seealso>
		<Obsolete> _
		Public Property Overrides year As Integer
			Get
				Throw New System.ArgumentException
			End Get
			Set(ByVal i As Integer)
				Throw New System.ArgumentException
			End Set
		End Property

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL <code>TIME</code>
	   ''' values do not have a month component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this
	   '''           method is invoked </exception>
	   ''' <seealso cref= #setMonth </seealso>
		<Obsolete> _
		Public Property Overrides month As Integer
			Get
				Throw New System.ArgumentException
			End Get
			Set(ByVal i As Integer)
				Throw New System.ArgumentException
			End Set
		End Property

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL <code>TIME</code>
	   ''' values do not have a day component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this
	   '''           method is invoked </exception>
		<Obsolete> _
		Public Property Overrides day As Integer
			Get
				Throw New System.ArgumentException
			End Get
		End Property

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL <code>TIME</code>
	   ''' values do not have a date component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this
	   '''           method is invoked </exception>
	   ''' <seealso cref= #setDate </seealso>
		<Obsolete> _
		Public Property Overrides [date] As Integer
			Get
				Throw New System.ArgumentException
			End Get
			Set(ByVal i As Integer)
				Throw New System.ArgumentException
			End Set
		End Property




	   ''' <summary>
	   ''' Private serial version unique ID to ensure serialization
	   ''' compatibility.
	   ''' </summary>
		Friend Const serialVersionUID As Long = 8397324403548013681L

		''' <summary>
		''' Obtains an instance of {@code Time} from a <seealso cref="LocalTime"/> object
		''' with the same hour, minute and second time value as the given
		''' {@code LocalTime}.
		''' </summary>
		''' <param name="time"> a {@code LocalTime} to convert </param>
		''' <returns> a {@code Time} object </returns>
		''' <exception cref="NullPointerException"> if {@code time} is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function valueOf(ByVal time_Renamed As java.time.LocalTime) As Time
			Return New Time(time_Renamed.hour, time_Renamed.minute, time_Renamed.second)
		End Function

		''' <summary>
		''' Converts this {@code Time} object to a {@code LocalTime}.
		''' <p>
		''' The conversion creates a {@code LocalTime} that represents the same
		''' hour, minute, and second time value as this {@code Time}.
		''' </summary>
		''' <returns> a {@code LocalTime} object representing the same time value
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toLocalTime() As java.time.LocalTime
			Return java.time.LocalTime.of(hours, minutes, seconds)
		End Function

	   ''' <summary>
	   ''' This method always throws an UnsupportedOperationException and should
	   ''' not be used because SQL {@code Time} values do not have a date
	   ''' component.
	   ''' </summary>
	   ''' <exception cref="java.lang.UnsupportedOperationException"> if this method is invoked </exception>
		Public Overrides Function toInstant() As java.time.Instant
			Throw New System.NotSupportedException
		End Function
	End Class

End Namespace
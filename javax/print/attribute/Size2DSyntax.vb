Imports Microsoft.VisualBasic
Imports System
Imports System.Text

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.print.attribute


	''' <summary>
	''' Class Size2DSyntax is an abstract base class providing the common
	''' implementation of all attributes denoting a size in two dimensions.
	''' <P>
	''' A two-dimensional size attribute's value consists of two items, the X
	''' dimension and the Y dimension. A two-dimensional size attribute may be
	''' constructed by supplying the two values and indicating the units in which the
	''' values are measured. Methods are provided to return a two-dimensional size
	''' attribute's values, indicating the units in which the values are to be
	''' returned. The two most common size units are inches (in) and millimeters
	''' (mm), and exported constants <seealso cref="#INCH INCH"/> and {@link #MM
	''' MM} are provided for indicating those units.
	''' <P>
	''' Once constructed, a two-dimensional size attribute's value is immutable.
	''' <P>
	''' <B>Design</B>
	''' <P>
	''' A two-dimensional size attribute's X and Y dimension values are stored
	''' internally as integers in units of micrometers (&#181;m), where 1 micrometer
	''' = 10<SUP>-6</SUP> meter = 1/1000 millimeter = 1/25400 inch. This permits
	''' dimensions to be represented exactly to a precision of 1/1000 mm (= 1
	''' &#181;m) or 1/100 inch (= 254 &#181;m). If fractional inches are expressed in
	''' negative powers of two, this permits dimensions to be represented exactly to
	''' a precision of 1/8 inch (= 3175 &#181;m) but not 1/16 inch (because 1/16 inch
	''' does not equal an integral number of &#181;m).
	''' <P>
	''' Storing the dimensions internally in common units of &#181;m lets two size
	''' attributes be compared without regard to the units in which they were
	''' created; for example, 8.5 in will compare equal to 215.9 mm, as they both are
	''' stored as 215900 &#181;m. For example, a lookup service can
	''' match resolution attributes based on equality of their serialized
	''' representations regardless of the units in which they were created. Using
	''' integers for internal storage allows precise equality comparisons to be done,
	''' which would not be guaranteed if an internal floating point representation
	''' were used. Note that if you're looking for U.S. letter sized media in metric
	''' units, you have to search for a media size of 215.9 x 279.4 mm; rounding off
	''' to an integral 216 x 279 mm will not match.
	''' <P>
	''' The exported constant <seealso cref="#INCH INCH"/> is actually the
	''' conversion factor by which to multiply a value in inches to get the value in
	''' &#181;m. Likewise, the exported constant <seealso cref="#MM MM"/> is the
	''' conversion factor by which to multiply a value in mm to get the value in
	''' &#181;m. A client can specify a resolution value in units other than inches
	''' or mm by supplying its own conversion factor. However, since the internal
	''' units of &#181;m was chosen with supporting only the external units of inch
	''' and mm in mind, there is no guarantee that the conversion factor for the
	''' client's units will be an exact integer. If the conversion factor isn't an
	''' exact integer, resolution values in the client's units won't be stored
	''' precisely.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class Size2DSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = 5584439964938660530L

		''' <summary>
		''' X dimension in units of micrometers (&#181;m).
		''' @serial
		''' </summary>
		Private x As Integer

		''' <summary>
		''' Y dimension in units of micrometers (&#181;m).
		''' @serial
		''' </summary>
		Private y As Integer

		''' <summary>
		''' Value to indicate units of inches (in). It is actually the conversion
		''' factor by which to multiply inches to yield &#181;m (25400).
		''' </summary>
		Public Const INCH As Integer = 25400

		''' <summary>
		''' Value to indicate units of millimeters (mm). It is actually the
		''' conversion factor by which to multiply mm to yield &#181;m (1000).
		''' </summary>
		Public Const MM As Integer = 1000


		''' <summary>
		''' Construct a new two-dimensional size attribute from the given
		''' floating-point values.
		''' </summary>
		''' <param name="x">  X dimension. </param>
		''' <param name="y">  Y dimension. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if {@code x < 0} or {@code y < 0} or
		'''     {@code units < 1}. </exception>
		Protected Friend Sub New(ByVal x As Single, ByVal y As Single, ByVal units As Integer)
			If x < 0.0f Then Throw New System.ArgumentException("x < 0")
			If y < 0.0f Then Throw New System.ArgumentException("y < 0")
			If units < 1 Then Throw New System.ArgumentException("units < 1")
			Me.x = CInt(Fix(x * units + 0.5f))
			Me.y = CInt(Fix(y * units + 0.5f))
		End Sub

		''' <summary>
		''' Construct a new two-dimensional size attribute from the given integer
		''' values.
		''' </summary>
		''' <param name="x">  X dimension. </param>
		''' <param name="y">  Y dimension. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if {@code x < 0} or {@code y < 0}
		'''    or {@code units < 1}. </exception>
		Protected Friend Sub New(ByVal x As Integer, ByVal y As Integer, ByVal units As Integer)
			If x < 0 Then Throw New System.ArgumentException("x < 0")
			If y < 0 Then Throw New System.ArgumentException("y < 0")
			If units < 1 Then Throw New System.ArgumentException("units < 1")
			Me.x = x * units
			Me.y = y * units
		End Sub

		''' <summary>
		''' Convert a value from micrometers to some other units. The result is
		''' returned as a floating-point number.
		''' </summary>
		''' <param name="x">
		'''     Value (micrometers) to convert. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH <CODE>INCH</CODE>"/> or
		'''     <seealso cref="#MM <CODE>MM</CODE>"/>.
		''' </param>
		''' <returns>  The value of <CODE>x</CODE> converted to the desired units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>units</CODE> < 1. </exception>
		Private Shared Function convertFromMicrometers(ByVal x As Integer, ByVal units As Integer) As Single
			If units < 1 Then Throw New System.ArgumentException("units is < 1")
			Return (CSng(x)) / (CSng(units))
		End Function

		''' <summary>
		''' Get this two-dimensional size attribute's dimensions in the given units
		''' as floating-point values.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  A two-element array with the X dimension at index 0 and the Y
		'''          dimension at index 1.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overridable Function getSize(ByVal units As Integer) As Single()
			Return New Single() {getX(units), getY(units)}
		End Function

		''' <summary>
		''' Returns this two-dimensional size attribute's X dimension in the given
		''' units as a floating-point value.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  X dimension.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overridable Function getX(ByVal units As Integer) As Single
			Return convertFromMicrometers(x, units)
		End Function

		''' <summary>
		''' Returns this two-dimensional size attribute's Y dimension in the given
		''' units as a floating-point value.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  Y dimension.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overridable Function getY(ByVal units As Integer) As Single
			Return convertFromMicrometers(y, units)
		End Function

		''' <summary>
		''' Returns a string version of this two-dimensional size attribute in the
		''' given units. The string takes the form <CODE>"<I>X</I>x<I>Y</I>
		''' <I>U</I>"</CODE>, where <I>X</I> is the X dimension, <I>Y</I> is the Y
		''' dimension, and <I>U</I> is the units name. The values are displayed in
		''' floating point.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or <seealso cref="#MM MM"/>.
		''' </param>
		''' <param name="unitsName">
		'''     Units name string, e.g. {@code in} or {@code mm}. If
		'''     null, no units name is appended to the result.
		''' </param>
		''' <returns>  String version of this two-dimensional size attribute.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overrides Function ToString(ByVal units As Integer, ByVal unitsName As String) As String
			Dim result As New StringBuilder
			result.Append(getX(units))
			result.Append("x"c)
			result.Append(getY(units))
			If unitsName IsNot Nothing Then
				result.Append(" "c)
				result.Append(unitsName)
			End If
			Return result.ToString()
		End Function

		''' <summary>
		''' Returns whether this two-dimensional size attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions must
		''' be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class Size2DSyntax.
		''' <LI>
		''' This attribute's X dimension is equal to <CODE>object</CODE>'s X
		''' dimension.
		''' <LI>
		''' This attribute's Y dimension is equal to <CODE>object</CODE>'s Y
		''' dimension.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this
		'''          two-dimensional size attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is Size2DSyntax AndAlso Me.x = CType([object], Size2DSyntax).x AndAlso Me.y = CType([object], Size2DSyntax).y)
		End Function

		''' <summary>
		''' Returns a hash code value for this two-dimensional size attribute.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return (((x And &HFFFF)) Or ((y And &HFFFF) << 16))
		End Function

		''' <summary>
		''' Returns a string version of this two-dimensional size attribute. The
		''' string takes the form <CODE>"<I>X</I>x<I>Y</I> um"</CODE>, where
		''' <I>X</I> is the X dimension and <I>Y</I> is the Y dimension.
		''' The values are reported in the internal units of micrometers.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim result As New StringBuilder
			result.Append(x)
			result.Append("x"c)
			result.Append(y)
			result.Append(" um")
			Return result.ToString()
		End Function

		''' <summary>
		''' Returns this two-dimensional size attribute's X dimension in units of
		''' micrometers (&#181;m). (For use in a subclass.)
		''' </summary>
		''' <returns>  X dimension (&#181;m). </returns>
		Protected Friend Overridable Property xMicrometers As Integer
			Get
				Return x
			End Get
		End Property

		''' <summary>
		''' Returns this two-dimensional size attribute's Y dimension in units of
		''' micrometers (&#181;m). (For use in a subclass.)
		''' </summary>
		''' <returns>  Y dimension (&#181;m). </returns>
		Protected Friend Overridable Property yMicrometers As Integer
			Get
				Return y
			End Get
		End Property

	End Class

End Namespace
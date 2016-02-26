Imports Microsoft.VisualBasic
Imports System

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
Namespace javax.print.attribute.standard


	''' <summary>
	''' Class MediaPrintableArea is a printing attribute used to distinguish
	''' the printable and non-printable areas of media.
	''' <p>
	''' The printable area is specified to be a rectangle, within the overall
	''' dimensions of a media.
	''' <p>
	''' Most printers cannot print on the entire surface of the media, due
	''' to printer hardware limitations. This class can be used to query
	''' the acceptable values for a supposed print job, and to request an area
	''' within the constraints of the printable area to be used in a print job.
	''' <p>
	''' To query for the printable area, a client must supply a suitable context.
	''' Without specifying at the very least the size of the media being used
	''' no meaningful value for printable area can be obtained.
	''' <p>
	''' The attribute is not described in terms of the distance from the edge
	''' of the paper, in part to emphasise that this attribute is not independent
	''' of a particular media, but must be described within the context of a
	''' choice of other attributes. Additionally it is usually more convenient
	''' for a client to use the printable area.
	''' <p>
	''' The hardware's minimum margins is not just a property of the printer,
	''' but may be a function of the media size, orientation, media type, and
	''' any specified finishings.
	''' <code>PrintService</code> provides the method to query the supported
	''' values of an attribute in a suitable context :
	''' See  <seealso cref="javax.print.PrintService#getSupportedAttributeValues(Class,DocFlavor, AttributeSet) PrintService.getSupportedAttributeValues()"/>
	''' <p>
	''' The rectangular printable area is defined thus:
	''' The (x,y) origin is positioned at the top-left of the paper in portrait
	''' mode regardless of the orientation specified in the requesting context.
	''' For example a printable area for A4 paper in portrait or landscape
	''' orientation will have height {@literal >} width.
	''' <p>
	''' A printable area attribute's values are stored
	''' internally as integers in units of micrometers (&#181;m), where 1 micrometer
	''' = 10<SUP>-6</SUP> meter = 1/1000 millimeter = 1/25400 inch. This permits
	''' dimensions to be represented exactly to a precision of 1/1000 mm (= 1
	''' &#181;m) or 1/100 inch (= 254 &#181;m). If fractional inches are expressed in
	''' 
	''' negative powers of two, this permits dimensions to be represented exactly to
	''' a precision of 1/8 inch (= 3175 &#181;m) but not 1/16 inch (because 1/16 inch
	''' 
	''' does not equal an integral number of &#181;m).
	''' <p>
	''' <B>IPP Compatibility:</B> MediaPrintableArea is not an IPP attribute.
	''' </summary>

	Public NotInheritable Class MediaPrintableArea
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private x, y, w, h As Integer
		Private units As Integer

		Private Const serialVersionUID As Long = -1597171464050795793L

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
		''' Constructs a MediaPrintableArea object from floating point values. </summary>
		''' <param name="x">      printable x </param>
		''' <param name="y">      printable y </param>
		''' <param name="w">      printable width </param>
		''' <param name="h">      printable height </param>
		''' <param name="units">  in which the values are expressed.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     Thrown if {@code x < 0} or {@code y < 0}
		'''     or {@code w <= 0} or {@code h <= 0} or
		'''     {@code units < 1}. </exception>
		Public Sub New(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal units As Integer)
			If (x < 0.0) OrElse (y < 0.0) OrElse (w <= 0.0) OrElse (h <= 0.0) OrElse (units < 1) Then Throw New System.ArgumentException("0 or negative value argument")

			Me.x = CInt(Fix(x * units + 0.5f))
			Me.y = CInt(Fix(y * units + 0.5f))
			Me.w = CInt(Fix(w * units + 0.5f))
			Me.h = CInt(Fix(h * units + 0.5f))

		End Sub

		''' <summary>
		''' Constructs a MediaPrintableArea object from integer values. </summary>
		''' <param name="x">      printable x </param>
		''' <param name="y">      printable y </param>
		''' <param name="w">      printable width </param>
		''' <param name="h">      printable height </param>
		''' <param name="units">  in which the values are expressed.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     Thrown if {@code x < 0} or {@code y < 0}
		'''     or {@code w <= 0} or {@code h <= 0} or
		'''     {@code units < 1}. </exception>
		Public Sub New(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal units As Integer)
			If (x < 0) OrElse (y < 0) OrElse (w <= 0) OrElse (h <= 0) OrElse (units < 1) Then Throw New System.ArgumentException("0 or negative value argument")
			Me.x = x * units
			Me.y = y * units
			Me.w = w * units
			Me.h = h * units

		End Sub

		''' <summary>
		''' Get the printable area as an array of 4 values in the order
		''' x, y, w, h. The values returned are in the given units. </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns> printable area as array of x, y, w, h in the specified units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Function getPrintableArea(ByVal units As Integer) As Single()
			Return New Single() { getX(units), getY(units), getWidth(units), getHeight(units) }
		End Function

		''' <summary>
		''' Get the x location of the origin of the printable area in the
		''' specified units. </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  x location of the origin of the printable area in the
		''' specified units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		 Public Function getX(ByVal units As Integer) As Single
			Return convertFromMicrometers(x, units)
		 End Function

		''' <summary>
		''' Get the y location of the origin of the printable area in the
		''' specified units. </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  y location of the origin of the printable area in the
		''' specified units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		 Public Function getY(ByVal units As Integer) As Single
			Return convertFromMicrometers(y, units)
		 End Function

		''' <summary>
		''' Get the width of the printable area in the specified units. </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  width of the printable area in the specified units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		 Public Function getWidth(ByVal units As Integer) As Single
			Return convertFromMicrometers(w, units)
		 End Function

		''' <summary>
		''' Get the height of the printable area in the specified units. </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>.
		''' </param>
		''' <returns>  height of the printable area in the specified units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		 Public Function getHeight(ByVal units As Integer) As Single
			Return convertFromMicrometers(h, units)
		 End Function

		''' <summary>
		''' Returns whether this media margins attribute is equivalent to the passed
		''' in object.
		''' To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class MediaPrintableArea.
		''' <LI>
		''' The origin and dimensions are the same.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this media margins
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Dim ret As Boolean = False
			If TypeOf [object] Is MediaPrintableArea Then
			   Dim ___mm As MediaPrintableArea = CType([object], MediaPrintableArea)
			   If x = ___mm.x AndAlso y = ___mm.y AndAlso w = ___mm.w AndAlso h = ___mm.h Then ret = True
			End If
			Return ret
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class MediaPrintableArea, the category is
		''' class MediaPrintableArea itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(MediaPrintableArea)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class MediaPrintableArea,
		''' the category name is <CODE>"media-printable-area"</CODE>.
		''' <p>This is not an IPP V1.1 attribute.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "media-printable-area"
			End Get
		End Property

		''' <summary>
		''' Returns a string version of this rectangular size attribute in the
		''' given units.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#INCH INCH"/> or
		'''     <seealso cref="#MM MM"/>. </param>
		''' <param name="unitsName">
		'''     Units name string, e.g. <CODE>"in"</CODE> or <CODE>"mm"</CODE>. If
		'''     null, no units name is appended to the result.
		''' </param>
		''' <returns>  String version of this two-dimensional size attribute.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overrides Function ToString(ByVal units As Integer, ByVal unitsName As String) As String
			If unitsName Is Nothing Then unitsName = ""
			Dim vals As Single() = getPrintableArea(units)
			Dim str As String = "(" & vals(0) & "," & vals(1) & ")->(" & vals(2) & "," & vals(3) & ")"
			Return str + unitsName
		End Function

		''' <summary>
		''' Returns a string version of this rectangular size attribute in mm.
		''' </summary>
		Public Overrides Function ToString() As String
			Return (ToString(MM, "mm"))
		End Function

		''' <summary>
		''' Returns a hash code value for this attribute.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return x + 37*y + 43*w + 47*h
		End Function

		Private Shared Function convertFromMicrometers(ByVal x As Integer, ByVal units As Integer) As Single
			If units < 1 Then Throw New System.ArgumentException("units is < 1")
			Return (CSng(x)) / (CSng(units))
		End Function
	End Class

End Namespace
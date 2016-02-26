Imports System
Imports System.Collections

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
	''' Class MediaSize is a two-dimensional size valued printing attribute class
	''' that indicates the dimensions of the medium in a portrait orientation, with
	''' the X dimension running along the bottom edge and the Y dimension running
	''' along the left edge. Thus, the Y dimension must be greater than or equal to
	''' the X dimension. Class MediaSize declares many standard media size
	''' values, organized into nested classes for ISO, JIS, North American,
	''' engineering, and other media.
	''' <P>
	''' MediaSize is not yet used to specify media. Its current role is
	''' as a mapping for named media (see <seealso cref="MediaSizeName MediaSizeName"/>).
	''' Clients can use the mapping method
	''' <code>MediaSize.getMediaSizeForName(MediaSizeName)</code>
	''' to find the physical dimensions of the MediaSizeName instances
	''' enumerated in this API. This is useful for clients which need this
	''' information to format {@literal &} paginate printing.
	''' <P>
	''' 
	''' @author  Phil Race, Alan Kaminsky
	''' </summary>
	Public Class MediaSize
		Inherits javax.print.attribute.Size2DSyntax
		Implements javax.print.attribute.Attribute

		Private Const serialVersionUID As Long = -1967958664615414771L

		Private ___mediaName As MediaSizeName

		Private Shared mediaMap As New Hashtable(100, 10)

		Private Shared sizeVector As New ArrayList(100, 10)

		''' <summary>
		''' Construct a new media size attribute from the given floating-point
		''' values.
		''' </summary>
		''' <param name="x">  X dimension. </param>
		''' <param name="y">  Y dimension. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <CODE>Size2DSyntax.INCH</CODE> or
		'''     <CODE>Size2DSyntax.MM</CODE>.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if {@code x < 0} or {@code y < 0} or
		'''   {@code units < 1} or {@code x > y}. </exception>
		Public Sub New(ByVal x As Single, ByVal y As Single, ByVal units As Integer)
			MyBase.New(x, y, units)
			If x > y Then Throw New System.ArgumentException("X dimension > Y dimension")
			sizeVector.Add(Me)
		End Sub

		''' <summary>
		''' Construct a new media size attribute from the given integer values.
		''' </summary>
		''' <param name="x">  X dimension. </param>
		''' <param name="y">  Y dimension. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <CODE>Size2DSyntax.INCH</CODE> or
		'''     <CODE>Size2DSyntax.MM</CODE>.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if {@code x < 0} or {@code y < 0} or
		'''   {@code units < 1} or {@code x > y}. </exception>
		Public Sub New(ByVal x As Integer, ByVal y As Integer, ByVal units As Integer)
			MyBase.New(x, y, units)
			If x > y Then Throw New System.ArgumentException("X dimension > Y dimension")
			sizeVector.Add(Me)
		End Sub

	   ''' <summary>
	   ''' Construct a new media size attribute from the given floating-point
	   ''' values.
	   ''' </summary>
	   ''' <param name="x">  X dimension. </param>
	   ''' <param name="y">  Y dimension. </param>
	   ''' <param name="units">
	   '''     Unit conversion factor, e.g. <CODE>Size2DSyntax.INCH</CODE> or
	   '''     <CODE>Size2DSyntax.MM</CODE>. </param>
	   ''' <param name="media"> a media name to associate with this MediaSize
	   ''' </param>
	   ''' <exception cref="IllegalArgumentException">
	   '''   (Unchecked exception) Thrown if {@code x < 0} or {@code y < 0} or
	   '''   {@code units < 1} or {@code x > y}. </exception>
		Public Sub New(ByVal x As Single, ByVal y As Single, ByVal units As Integer, ByVal media As MediaSizeName)
			MyBase.New(x, y, units)
			If x > y Then Throw New System.ArgumentException("X dimension > Y dimension")
			If media IsNot Nothing AndAlso mediaMap(media) Is Nothing Then
				___mediaName = media
				mediaMap(___mediaName) = Me
			End If
			sizeVector.Add(Me)
		End Sub

		''' <summary>
		''' Construct a new media size attribute from the given integer values.
		''' </summary>
		''' <param name="x">  X dimension. </param>
		''' <param name="y">  Y dimension. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <CODE>Size2DSyntax.INCH</CODE> or
		'''     <CODE>Size2DSyntax.MM</CODE>. </param>
		''' <param name="media"> a media name to associate with this MediaSize
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if {@code x < 0} or {@code y < 0} or
		'''   {@code units < 1} or {@code x > y}. </exception>
		Public Sub New(ByVal x As Integer, ByVal y As Integer, ByVal units As Integer, ByVal media As MediaSizeName)
			MyBase.New(x, y, units)
			If x > y Then Throw New System.ArgumentException("X dimension > Y dimension")
			If media IsNot Nothing AndAlso mediaMap(media) Is Nothing Then
				___mediaName = media
				mediaMap(___mediaName) = Me
			End If
			sizeVector.Add(Me)
		End Sub

		''' <summary>
		''' Get the media name, if any, for this size.
		''' </summary>
		''' <returns> the name for this media size, or null if no name was
		''' associated with this size (an anonymous size). </returns>
		Public Overridable Property mediaSizeName As MediaSizeName
			Get
				Return ___mediaName
			End Get
		End Property

		''' <summary>
		''' Get the MediaSize for the specified named media.
		''' </summary>
		''' <param name="media"> - the name of the media for which the size is sought </param>
		''' <returns> size of the media, or null if this media is not associated
		''' with any size. </returns>
		Public Shared Function getMediaSizeForName(ByVal media As MediaSizeName) As MediaSize
			Return CType(mediaMap(media), MediaSize)
		End Function

		''' <summary>
		''' The specified dimensions are used to locate a matching MediaSize
		''' instance from amongst all the standard MediaSize instances.
		''' If there is no exact match, the closest match is used.
		''' <p>
		''' The MediaSize is in turn used to locate the MediaSizeName object.
		''' This method may return null if the closest matching MediaSize
		''' has no corresponding Media instance.
		''' <p>
		''' This method is useful for clients which have only dimensions and
		''' want to find a Media which corresponds to the dimensions. </summary>
		''' <param name="x"> - X dimension </param>
		''' <param name="y"> - Y dimension. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <CODE>Size2DSyntax.INCH</CODE> or
		'''     <CODE>Size2DSyntax.MM</CODE> </param>
		''' <returns> MediaSizeName matching these dimensions, or null. </returns>
		''' <exception cref="IllegalArgumentException"> if {@code x <= 0},
		'''      {@code y <= 0}, or {@code units < 1}.
		'''  </exception>
		Public Shared Function findMedia(ByVal x As Single, ByVal y As Single, ByVal units As Integer) As MediaSizeName

			Dim match As MediaSize = MediaSize.ISO.A4

			If x <= 0.0f OrElse y <= 0.0f OrElse units < 1 Then Throw New System.ArgumentException("args must be +ve values")

			Dim ls As Double = x * x + y * y
			Dim tmp_ls As Double
			Dim [dim] As Single()
			Dim diffx As Single = x
			Dim diffy As Single = y

			For i As Integer = 0 To sizeVector.Count - 1
				Dim ___mediaSize As MediaSize = CType(sizeVector(i), MediaSize)
				[dim] = ___mediaSize.getSize(units)
				If x = [dim](0) AndAlso y = [dim](1) Then
					match = ___mediaSize
					Exit For
				Else
					diffx = x - [dim](0)
					diffy = y - [dim](1)
					tmp_ls = diffx * diffx + diffy * diffy
					If tmp_ls < ls Then
						ls = tmp_ls
						match = ___mediaSize
					End If
				End If
			Next i

			Return match.mediaSizeName
		End Function

		''' <summary>
		''' Returns whether this media size attribute is equivalent to the passed
		''' in object.
		''' To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class MediaSize.
		''' <LI>
		''' This media size attribute's X dimension is equal to
		''' <CODE>object</CODE>'s X dimension.
		''' <LI>
		''' This media size attribute's Y dimension is equal to
		''' <CODE>object</CODE>'s Y dimension.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this media size
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is MediaSize)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class MediaSize and any vendor-defined subclasses, the category is
		''' class MediaSize itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(MediaSize)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class MediaSize and any vendor-defined subclasses, the category
		''' name is <CODE>"media-size"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "media-size"
			End Get
		End Property

		''' <summary>
		''' Class MediaSize.ISO includes <seealso cref="MediaSize MediaSize"/> values for ISO
		''' media.
		''' <P>
		''' </summary>
		Public NotInheritable Class ISO
			''' <summary>
			''' Specifies the ISO A0 size, 841 mm by 1189 mm.
			''' </summary>
			Public Shared ReadOnly A0 As New MediaSize(841, 1189, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A0)
			''' <summary>
			''' Specifies the ISO A1 size, 594 mm by 841 mm.
			''' </summary>
			Public Shared ReadOnly A1 As New MediaSize(594, 841, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A1)
			''' <summary>
			''' Specifies the ISO A2 size, 420 mm by 594 mm.
			''' </summary>
			Public Shared ReadOnly A2 As New MediaSize(420, 594, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A2)
			''' <summary>
			''' Specifies the ISO A3 size, 297 mm by 420 mm.
			''' </summary>
			Public Shared ReadOnly A3 As New MediaSize(297, 420, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A3)
			''' <summary>
			''' Specifies the ISO A4 size, 210 mm by 297 mm.
			''' </summary>
			Public Shared ReadOnly A4 As New MediaSize(210, 297, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A4)
			''' <summary>
			''' Specifies the ISO A5 size, 148 mm by 210 mm.
			''' </summary>
			Public Shared ReadOnly A5 As New MediaSize(148, 210, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A5)
			''' <summary>
			''' Specifies the ISO A6 size, 105 mm by 148 mm.
			''' </summary>
			Public Shared ReadOnly A6 As New MediaSize(105, 148, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A6)
			''' <summary>
			''' Specifies the ISO A7 size, 74 mm by 105 mm.
			''' </summary>
			Public Shared ReadOnly A7 As New MediaSize(74, 105, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A7)
			''' <summary>
			''' Specifies the ISO A8 size, 52 mm by 74 mm.
			''' </summary>
			Public Shared ReadOnly A8 As New MediaSize(52, 74, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A8)
			''' <summary>
			''' Specifies the ISO A9 size, 37 mm by 52 mm.
			''' </summary>
			Public Shared ReadOnly A9 As New MediaSize(37, 52, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A9)
			''' <summary>
			''' Specifies the ISO A10 size, 26 mm by 37 mm.
			''' </summary>
			Public Shared ReadOnly A10 As New MediaSize(26, 37, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_A10)
			''' <summary>
			''' Specifies the ISO B0 size, 1000 mm by 1414 mm.
			''' </summary>
			Public Shared ReadOnly B0 As New MediaSize(1000, 1414, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B0)
			''' <summary>
			''' Specifies the ISO B1 size, 707 mm by 1000 mm.
			''' </summary>
			Public Shared ReadOnly B1 As New MediaSize(707, 1000, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B1)
			''' <summary>
			''' Specifies the ISO B2 size, 500 mm by 707 mm.
			''' </summary>
			Public Shared ReadOnly B2 As New MediaSize(500, 707, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B2)
			''' <summary>
			''' Specifies the ISO B3 size, 353 mm by 500 mm.
			''' </summary>
			Public Shared ReadOnly B3 As New MediaSize(353, 500, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B3)
			''' <summary>
			''' Specifies the ISO B4 size, 250 mm by 353 mm.
			''' </summary>
			Public Shared ReadOnly B4 As New MediaSize(250, 353, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B4)
			''' <summary>
			''' Specifies the ISO B5 size, 176 mm by 250 mm.
			''' </summary>
			Public Shared ReadOnly B5 As New MediaSize(176, 250, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B5)
			''' <summary>
			''' Specifies the ISO B6 size, 125 mm by 176 mm.
			''' </summary>
			Public Shared ReadOnly B6 As New MediaSize(125, 176, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B6)
			''' <summary>
			''' Specifies the ISO B7 size, 88 mm by 125 mm.
			''' </summary>
			Public Shared ReadOnly B7 As New MediaSize(88, 125, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B7)
			''' <summary>
			''' Specifies the ISO B8 size, 62 mm by 88 mm.
			''' </summary>
			Public Shared ReadOnly B8 As New MediaSize(62, 88, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B8)
			''' <summary>
			''' Specifies the ISO B9 size, 44 mm by 62 mm.
			''' </summary>
			Public Shared ReadOnly B9 As New MediaSize(44, 62, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B9)
			''' <summary>
			''' Specifies the ISO B10 size, 31 mm by 44 mm.
			''' </summary>
			Public Shared ReadOnly B10 As New MediaSize(31, 44, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_B10)
			''' <summary>
			''' Specifies the ISO C3 size, 324 mm by 458 mm.
			''' </summary>
			Public Shared ReadOnly C3 As New MediaSize(324, 458, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_C3)
			''' <summary>
			''' Specifies the ISO C4 size, 229 mm by 324 mm.
			''' </summary>
			Public Shared ReadOnly C4 As New MediaSize(229, 324, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_C4)
			''' <summary>
			''' Specifies the ISO C5 size, 162 mm by 229 mm.
			''' </summary>
			Public Shared ReadOnly C5 As New MediaSize(162, 229, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_C5)
			''' <summary>
			''' Specifies the ISO C6 size, 114 mm by 162 mm.
			''' </summary>
			Public Shared ReadOnly C6 As New MediaSize(114, 162, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_C6)
			''' <summary>
			''' Specifies the ISO Designated Long size, 110 mm by 220 mm.
			''' </summary>
			Public Shared ReadOnly DESIGNATED_LONG As New MediaSize(110, 220, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ISO_DESIGNATED_LONG)

			''' <summary>
			''' Hide all constructors.
			''' </summary>
			Private Sub New()
			End Sub
		End Class

		''' <summary>
		''' Class MediaSize.JIS includes <seealso cref="MediaSize MediaSize"/> values for JIS
		''' (Japanese) media.      *
		''' </summary>
		Public NotInheritable Class JIS

			''' <summary>
			''' Specifies the JIS B0 size, 1030 mm by 1456 mm.
			''' </summary>
			Public Shared ReadOnly B0 As New MediaSize(1030, 1456, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B0)
			''' <summary>
			''' Specifies the JIS B1 size, 728 mm by 1030 mm.
			''' </summary>
			Public Shared ReadOnly B1 As New MediaSize(728, 1030, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B1)
			''' <summary>
			''' Specifies the JIS B2 size, 515 mm by 728 mm.
			''' </summary>
			Public Shared ReadOnly B2 As New MediaSize(515, 728, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B2)
			''' <summary>
			''' Specifies the JIS B3 size, 364 mm by 515 mm.
			''' </summary>
			Public Shared ReadOnly B3 As New MediaSize(364, 515, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B3)
			''' <summary>
			''' Specifies the JIS B4 size, 257 mm by 364 mm.
			''' </summary>
			Public Shared ReadOnly B4 As New MediaSize(257, 364, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B4)
			''' <summary>
			''' Specifies the JIS B5 size, 182 mm by 257 mm.
			''' </summary>
			Public Shared ReadOnly B5 As New MediaSize(182, 257, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B5)
			''' <summary>
			''' Specifies the JIS B6 size, 128 mm by 182 mm.
			''' </summary>
			Public Shared ReadOnly B6 As New MediaSize(128, 182, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B6)
			''' <summary>
			''' Specifies the JIS B7 size, 91 mm by 128 mm.
			''' </summary>
			Public Shared ReadOnly B7 As New MediaSize(91, 128, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B7)
			''' <summary>
			''' Specifies the JIS B8 size, 64 mm by 91 mm.
			''' </summary>
			Public Shared ReadOnly B8 As New MediaSize(64, 91, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B8)
			''' <summary>
			''' Specifies the JIS B9 size, 45 mm by 64 mm.
			''' </summary>
			Public Shared ReadOnly B9 As New MediaSize(45, 64, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B9)
			''' <summary>
			''' Specifies the JIS B10 size, 32 mm by 45 mm.
			''' </summary>
			Public Shared ReadOnly B10 As New MediaSize(32, 45, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JIS_B10)
			''' <summary>
			''' Specifies the JIS Chou ("long") #1 envelope size, 142 mm by 332 mm.
			''' </summary>
			Public Shared ReadOnly CHOU_1 As New MediaSize(142, 332, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Chou ("long") #2 envelope size, 119 mm by 277 mm.
			''' </summary>
			Public Shared ReadOnly CHOU_2 As New MediaSize(119, 277, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Chou ("long") #3 envelope size, 120 mm by 235 mm.
			''' </summary>
			Public Shared ReadOnly CHOU_3 As New MediaSize(120, 235, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Chou ("long") #4 envelope size, 90 mm by 205 mm.
			''' </summary>
			Public Shared ReadOnly CHOU_4 As New MediaSize(90, 205, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Chou ("long") #30 envelope size, 92 mm by 235 mm.
			''' </summary>
			Public Shared ReadOnly CHOU_30 As New MediaSize(92, 235, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Chou ("long") #40 envelope size, 90 mm by 225 mm.
			''' </summary>
			Public Shared ReadOnly CHOU_40 As New MediaSize(90, 225, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #0 envelope size, 287 mm by 382 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_0 As New MediaSize(287, 382, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #1 envelope size, 270 mm by 382 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_1 As New MediaSize(270, 382, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #2 envelope size, 240 mm by 332 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_2 As New MediaSize(240, 332, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #3 envelope size, 216 mm by 277 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_3 As New MediaSize(216, 277, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #4 envelope size, 197 mm by 267 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_4 As New MediaSize(197, 267, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #5 envelope size, 190 mm by 240 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_5 As New MediaSize(190, 240, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #6 envelope size, 162 mm by 229 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_6 As New MediaSize(162, 229, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #7 envelope size, 142 mm by 205 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_7 As New MediaSize(142, 205, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #8 envelope size, 119 mm by 197 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_8 As New MediaSize(119, 197, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") #20 envelope size, 229 mm by 324 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_20 As New MediaSize(229, 324, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS Kaku ("square") A4 envelope size, 228 mm by 312 mm.
			''' </summary>
			Public Shared ReadOnly KAKU_A4 As New MediaSize(228, 312, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #1 envelope size, 120 mm by 176 mm.
			''' </summary>
			Public Shared ReadOnly YOU_1 As New MediaSize(120, 176, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #2 envelope size, 114 mm by 162 mm.
			''' </summary>
			Public Shared ReadOnly YOU_2 As New MediaSize(114, 162, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #3 envelope size, 98 mm by 148 mm.
			''' </summary>
			Public Shared ReadOnly YOU_3 As New MediaSize(98, 148, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #4 envelope size, 105 mm by 235 mm.
			''' </summary>
			Public Shared ReadOnly YOU_4 As New MediaSize(105, 235, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #5 envelope size, 95 mm by 217 mm.
			''' </summary>
			Public Shared ReadOnly YOU_5 As New MediaSize(95, 217, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #6 envelope size, 98 mm by 190 mm.
			''' </summary>
			Public Shared ReadOnly YOU_6 As New MediaSize(98, 190, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Specifies the JIS You ("Western") #7 envelope size, 92 mm by 165 mm.
			''' </summary>
			Public Shared ReadOnly YOU_7 As New MediaSize(92, 165, javax.print.attribute.Size2DSyntax.MM)
			''' <summary>
			''' Hide all constructors.
			''' </summary>
			Private Sub New()
			End Sub
		End Class

		''' <summary>
		''' Class MediaSize.NA includes <seealso cref="MediaSize MediaSize"/> values for North
		''' American media.
		''' </summary>
		Public NotInheritable Class NA

			''' <summary>
			''' Specifies the North American letter size, 8.5 inches by 11 inches.
			''' </summary>
			Public Shared ReadOnly LETTER As New MediaSize(8.5f, 11.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_LETTER)
			''' <summary>
			''' Specifies the North American legal size, 8.5 inches by 14 inches.
			''' </summary>
			Public Shared ReadOnly LEGAL As New MediaSize(8.5f, 14.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_LEGAL)
			''' <summary>
			''' Specifies the North American 5 inch by 7 inch paper.
			''' </summary>
			Public Shared ReadOnly NA_5X7 As New MediaSize(5, 7, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_5X7)
			''' <summary>
			''' Specifies the North American 8 inch by 10 inch paper.
			''' </summary>
			Public Shared ReadOnly NA_8X10 As New MediaSize(8, 10, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_8X10)
			''' <summary>
			''' Specifies the North American Number 9 business envelope size,
			''' 3.875 inches by 8.875 inches.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_9_ENVELOPE As New MediaSize(3.875f, 8.875f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_NUMBER_9_ENVELOPE)
			''' <summary>
			''' Specifies the North American Number 10 business envelope size,
			''' 4.125 inches by 9.5 inches.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_10_ENVELOPE As New MediaSize(4.125f, 9.5f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_NUMBER_10_ENVELOPE)
			''' <summary>
			''' Specifies the North American Number 11 business envelope size,
			''' 4.5 inches by 10.375 inches.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_11_ENVELOPE As New MediaSize(4.5f, 10.375f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_NUMBER_11_ENVELOPE)
			''' <summary>
			''' Specifies the North American Number 12 business envelope size,
			''' 4.75 inches by 11 inches.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_12_ENVELOPE As New MediaSize(4.75f, 11.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_NUMBER_12_ENVELOPE)
			''' <summary>
			''' Specifies the North American Number 14 business envelope size,
			''' 5 inches by 11.5 inches.
			''' </summary>
			Public Shared ReadOnly NA_NUMBER_14_ENVELOPE As New MediaSize(5.0f, 11.5f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_NUMBER_14_ENVELOPE)

			''' <summary>
			''' Specifies the North American 6 inch by 9 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_6X9_ENVELOPE As New MediaSize(6.0f, 9.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_6X9_ENVELOPE)
			''' <summary>
			''' Specifies the North American 7 inch by 9 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_7X9_ENVELOPE As New MediaSize(7.0f, 9.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_7X9_ENVELOPE)
			''' <summary>
			''' Specifies the North American 9 inch by 11 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_9x11_ENVELOPE As New MediaSize(9.0f, 11.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_9X11_ENVELOPE)
			''' <summary>
			''' Specifies the North American 9 inch by 12 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_9x12_ENVELOPE As New MediaSize(9.0f, 12.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_9X12_ENVELOPE)
			''' <summary>
			''' Specifies the North American 10 inch by 13 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_10x13_ENVELOPE As New MediaSize(10.0f, 13.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_10X13_ENVELOPE)
			''' <summary>
			''' Specifies the North American 10 inch by 14 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_10x14_ENVELOPE As New MediaSize(10.0f, 14.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_10X14_ENVELOPE)
			''' <summary>
			''' Specifies the North American 10 inch by 15 inch envelope size.
			''' </summary>
			Public Shared ReadOnly NA_10X15_ENVELOPE As New MediaSize(10.0f, 15.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.NA_10X15_ENVELOPE)
			''' <summary>
			''' Hide all constructors.
			''' </summary>
			Private Sub New()
			End Sub
		End Class

		''' <summary>
		''' Class MediaSize.Engineering includes <seealso cref="MediaSize MediaSize"/> values
		''' for engineering media.
		''' </summary>
		Public NotInheritable Class Engineering

			''' <summary>
			''' Specifies the engineering A size, 8.5 inch by 11 inch.
			''' </summary>
			Public Shared ReadOnly A As New MediaSize(8.5f, 11.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.A)
			''' <summary>
			''' Specifies the engineering B size, 11 inch by 17 inch.
			''' </summary>
			Public Shared ReadOnly B As New MediaSize(11.0f, 17.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.B)
			''' <summary>
			''' Specifies the engineering C size, 17 inch by 22 inch.
			''' </summary>
			Public Shared ReadOnly C As New MediaSize(17.0f, 22.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.C)
			''' <summary>
			''' Specifies the engineering D size, 22 inch by 34 inch.
			''' </summary>
			Public Shared ReadOnly D As New MediaSize(22.0f, 34.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.D)
			''' <summary>
			''' Specifies the engineering E size, 34 inch by 44 inch.
			''' </summary>
			Public Shared ReadOnly E As New MediaSize(34.0f, 44.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.E)
			''' <summary>
			''' Hide all constructors.
			''' </summary>
			Private Sub New()
			End Sub
		End Class

		''' <summary>
		''' Class MediaSize.Other includes <seealso cref="MediaSize MediaSize"/> values for
		''' miscellaneous media.
		''' </summary>
		Public NotInheritable Class Other
			''' <summary>
			''' Specifies the executive size, 7.25 inches by 10.5 inches.
			''' </summary>
			Public Shared ReadOnly EXECUTIVE As New MediaSize(7.25f, 10.5f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.EXECUTIVE)
			''' <summary>
			''' Specifies the ledger size, 11 inches by 17 inches.
			''' </summary>
			Public Shared ReadOnly LEDGER As New MediaSize(11.0f, 17.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.LEDGER)

			''' <summary>
			''' Specifies the tabloid size, 11 inches by 17 inches.
			''' @since 1.5
			''' </summary>
			Public Shared ReadOnly TABLOID As New MediaSize(11.0f, 17.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.TABLOID)

			''' <summary>
			''' Specifies the invoice size, 5.5 inches by 8.5 inches.
			''' </summary>
			Public Shared ReadOnly INVOICE As New MediaSize(5.5f, 8.5f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.INVOICE)
			''' <summary>
			''' Specifies the folio size, 8.5 inches by 13 inches.
			''' </summary>
			Public Shared ReadOnly FOLIO As New MediaSize(8.5f, 13.0f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.FOLIO)
			''' <summary>
			''' Specifies the quarto size, 8.5 inches by 10.83 inches.
			''' </summary>
			Public Shared ReadOnly QUARTO As New MediaSize(8.5f, 10.83f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.QUARTO)
			''' <summary>
			''' Specifies the Italy envelope size, 110 mm by 230 mm.
			''' </summary>
			Public Shared ReadOnly ITALY_ENVELOPE As New MediaSize(110, 230, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.ITALY_ENVELOPE)
			''' <summary>
			''' Specifies the Monarch envelope size, 3.87 inch by 7.5 inch.
			''' </summary>
			Public Shared ReadOnly MONARCH_ENVELOPE As New MediaSize(3.87f, 7.5f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.MONARCH_ENVELOPE)
			''' <summary>
			''' Specifies the Personal envelope size, 3.625 inch by 6.5 inch.
			''' </summary>
			Public Shared ReadOnly PERSONAL_ENVELOPE As New MediaSize(3.625f, 6.5f, javax.print.attribute.Size2DSyntax.INCH, MediaSizeName.PERSONAL_ENVELOPE)
			''' <summary>
			''' Specifies the Japanese postcard size, 100 mm by 148 mm.
			''' </summary>
			Public Shared ReadOnly JAPANESE_POSTCARD As New MediaSize(100, 148, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JAPANESE_POSTCARD)
			''' <summary>
			''' Specifies the Japanese Double postcard size, 148 mm by 200 mm.
			''' </summary>
			Public Shared ReadOnly JAPANESE_DOUBLE_POSTCARD As New MediaSize(148, 200, javax.print.attribute.Size2DSyntax.MM, MediaSizeName.JAPANESE_DOUBLE_POSTCARD)
			''' <summary>
			''' Hide all constructors.
			''' </summary>
			Private Sub New()
			End Sub
		End Class

	'     force loading of all the subclasses so that the instances
	'     * are created and inserted into the hashmap.
	'     
		Shared Sub New()
			Dim ISOA4 As MediaSize = ISO.A4
			Dim JISB5 As MediaSize = JIS.B5
			Dim NALETTER As MediaSize = NA.LETTER
			Dim EngineeringC As MediaSize = Engineering.C
			Dim OtherEXECUTIVE As MediaSize = Other.EXECUTIVE
		End Sub
	End Class

End Namespace
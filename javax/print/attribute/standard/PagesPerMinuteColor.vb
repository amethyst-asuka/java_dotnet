Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Class PagesPerMinuteColor is an integer valued printing attribute that
	''' indicates the nominal number of pages per minute to the nearest whole number
	''' which may be generated by this printer when printing color (e.g., simplex,
	''' color). For purposes of this attribute, "color" means the same as for the
	''' <seealso cref="ColorSupported ColorSupported"/> attribute, namely, the device is
	''' capable of any type of color printing at all, including highlight color as
	''' well as full process color. This attribute is informative, not a service
	''' guarantee. Generally, it is the value used in the marketing literature to
	''' describe the color capabilities of this device. A value of 0 indicates a
	''' device that takes more than two minutes to process a page. If a color device
	''' has several color modes, it may use the pages-per- minute value for this
	''' attribute that corresponds to the mode that produces the highest number.
	''' <P>
	''' A black and white only printer must not include the PagesPerMinuteColor
	''' attribute in its attribute set or service registration. If this attribute is
	''' present, then the <seealso cref="ColorSupported ColorSupported"/> printer description
	''' attribute must also be present and have a value of SUPPORTED.
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PagesPerMinuteColor
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Friend Const serialVersionUID As Long = 1684993151687470944L

		''' <summary>
		''' Construct a new pages per minute color attribute with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''    (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this pages per minute color attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PagesPerMinuteColor.
		''' <LI>
		''' This pages per minute attribute's value and <CODE>object</CODE>'s
		''' value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this pages per
		'''          minute color attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PagesPerMinuteColor)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PagesPerMinuteColor, the
		''' category is class PagesPerMinuteColor itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PagesPerMinuteColor)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PagesPerMinuteColor, the
		''' category name is <CODE>"pages-per-minute-color"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "pages-per-minute-color"
			End Get
		End Property

	End Class

End Namespace
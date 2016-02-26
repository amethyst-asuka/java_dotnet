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
	''' Class ColorSupported is a printing attribute class, an enumeration, that
	''' identifies whether the device is capable of any type of color printing at
	''' all, including highlight color as well as full process color. All document
	''' instructions having to do with color are embedded within the print data (none
	''' are attributes attached to the job outside the print data).
	''' <P>
	''' Note: End users are able to determine the nature and details of the color
	''' support by querying the {@link PrinterMoreInfoManufacturer
	''' PrinterMoreInfoManufacturer} attribute.
	''' <P>
	''' Don't confuse the ColorSupported attribute with the {@link Chromaticity
	''' Chromaticity} attribute. <seealso cref="Chromaticity Chromaticity"/> is an attribute
	''' the client can specify for a job to tell the printer whether to print a
	''' document in monochrome or color, possibly causing the printer to print a
	''' color document in monochrome. ColorSupported is a printer description
	''' attribute that tells whether the printer can print in color regardless of how
	''' the client specifies to print any particular document.
	''' <P>
	''' <B>IPP Compatibility:</B> The IPP boolean value is "true" for SUPPORTED and
	''' "false" for NOT_SUPPORTED. The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class ColorSupported
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = -2700555589688535545L

		''' <summary>
		''' The printer is not capable of any type of color printing.
		''' </summary>
		Public Shared ReadOnly NOT_SUPPORTED As New ColorSupported(0)

		''' <summary>
		''' The printer is capable of some type of color printing, such as
		''' highlight color or full process color.
		''' </summary>
		Public Shared ReadOnly SUPPORTED As New ColorSupported(1)

		''' <summary>
		''' Construct a new color supported enumeration value with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = {"not-supported", "supported"}

		Private Shared ReadOnly myEnumValueTable As ColorSupported() = {NOT_SUPPORTED, SUPPORTED}

		''' <summary>
		''' Returns the string table for class ColorSupported.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class ColorSupported.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return myEnumValueTable
			End Get
		End Property

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class ColorSupported, the category is class ColorSupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(ColorSupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class ColorSupported, the category name is <CODE>"color-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "color-supported"
			End Get
		End Property

	End Class

End Namespace
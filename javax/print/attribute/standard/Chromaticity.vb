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
	''' Class Chromaticity is a printing attribute class, an enumeration, that
	''' specifies monochrome or color printing. This is used by a print client
	''' to specify how the print data should be generated or processed. It is not
	''' descriptive of the color capabilities of the device. Query the service's
	''' <seealso cref="ColorSupported ColorSupported"/> attribute to determine if the device
	''' can be verified to support color printing.
	''' <P>
	''' The table below shows the effects of specifying a Chromaticity attribute of
	''' <seealso cref="#MONOCHROME MONOCHROME"/> or <seealso cref="#COLOR COLOR"/>
	''' for a monochrome or color document.
	''' <P>
	''' <TABLE BORDER=1 CELLPADDING=2 CELLSPACING=1 SUMMARY="Shows effects of specifying MONOCHROME or COLOR Chromaticity attributes">
	''' <TR>
	''' <TH>
	''' Chromaticity<BR>Attribute
	''' </TH>
	''' <TH>
	''' Effect on<BR>Monochrome Document
	''' </TH>
	''' <TH>
	''' Effect on<BR>Color Document
	''' </TH>
	''' </TR>
	''' <TR>
	''' <TD>
	''' <seealso cref="#MONOCHROME MONOCHROME"/>
	''' </TD>
	''' <TD>
	''' Printed as is, in monochrome
	''' </TD>
	''' <TD>
	''' Printed in monochrome, with colors converted to shades of gray
	''' </TD>
	''' </TR>
	''' <TR>
	''' <TD>
	''' <seealso cref="#COLOR COLOR"/>
	''' </TD>
	''' <TD>
	''' Printed as is, in monochrome
	''' </TD>
	''' <TD>
	''' Printed as is, in color
	''' </TD>
	''' </TR>
	''' </TABLE>
	''' <P>
	''' <P>
	''' <B>IPP Compatibility:</B> Chromaticity is not an IPP attribute at present.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class Chromaticity
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 4660543931355214012L

		''' <summary>
		''' Monochrome printing.
		''' </summary>
		Public Shared ReadOnly MONOCHROME As New Chromaticity(0)

		''' <summary>
		''' Color printing.
		''' </summary>
		Public Shared ReadOnly COLOR As New Chromaticity(1)


		''' <summary>
		''' Construct a new chromaticity enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = {"monochrome", "color"}

		Private Shared ReadOnly myEnumValueTable As Chromaticity() = {MONOCHROME, COLOR}

		''' <summary>
		''' Returns the string table for class Chromaticity.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class Chromaticity.
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
		''' For class Chromaticity, the category is the class Chromaticity itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Chromaticity)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Chromaticity, the category name is <CODE>"chromaticity"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
			Public Property name As String
				Get
					Return "chromaticity"
				End Get
			End Property

	End Class

End Namespace
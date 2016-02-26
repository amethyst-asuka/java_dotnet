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
	''' Class PrintQuality is a printing attribute class, an enumeration,
	''' that specifies the print quality that the printer uses for the job.
	''' <P>
	''' <B>IPP Compatibility:</B> The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class PrintQuality
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -3072341285225858365L
		''' <summary>
		''' Lowest quality available on the printer.
		''' </summary>
		Public Shared ReadOnly DRAFT As New PrintQuality(3)

		''' <summary>
		''' Normal or intermediate quality on the printer.
		''' </summary>
		Public Shared ReadOnly NORMAL As New PrintQuality(4)

		''' <summary>
		''' Highest quality available on the printer.
		''' </summary>
		Public Shared ReadOnly HIGH As New PrintQuality(5)

		''' <summary>
		''' Construct a new print quality enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "draft", "normal", "high" }

		Private Shared ReadOnly myEnumValueTable As PrintQuality() = { DRAFT, NORMAL, HIGH }

		''' <summary>
		''' Returns the string table for class PrintQuality.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class PrintQuality.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property

		''' <summary>
		''' Returns the lowest integer value used by class PrintQuality.
		''' </summary>
		Protected Friend Property Overrides offset As Integer
			Get
				Return 3
			End Get
		End Property

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrintQuality and any vendor-defined subclasses, the category is
		''' class PrintQuality itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrintQuality)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrintQuality and any vendor-defined subclasses, the category
		''' name is <CODE>"print-quality"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "print-quality"
			End Get
		End Property

	End Class

End Namespace
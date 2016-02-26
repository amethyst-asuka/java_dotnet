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
	''' Class JobSheets is a printing attribute class, an enumeration, that
	''' determines which job start and end sheets, if any, must be printed with a
	''' job. Class JobSheets declares keywords for standard job sheets values.
	''' Implementation- or site-defined names for a job sheets attribute may also be
	''' created by defining a subclass of class JobSheets.
	''' <P>
	''' The effect of a JobSheets attribute on multidoc print jobs (jobs with
	''' multiple documents) may be affected by the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} job attribute, depending on the meaning of the
	''' particular JobSheets value.
	''' <P>
	''' <B>IPP Compatibility:</B>  The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The
	''' enumeration's integer value is the IPP enum value.  The
	''' <code>toString()</code> method returns the IPP string representation of
	''' the attribute value. For a subclass, the attribute value must be
	''' localized to give the IPP name and natural language values.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class JobSheets
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -4735258056132519759L

		''' <summary>
		''' No job sheets are printed.
		''' </summary>
		Public Shared ReadOnly NONE As New JobSheets(0)

		''' <summary>
		''' One or more site specific standard job sheets are printed. e.g. a
		''' single start sheet is printed, or both start and end sheets are
		''' printed.
		''' </summary>
		Public Shared ReadOnly STANDARD As New JobSheets(1)

		''' <summary>
		''' Construct a new job sheets enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "none", "standard" }

		Private Shared ReadOnly myEnumValueTable As JobSheets() = { NONE, STANDARD }

		''' <summary>
		''' Returns the string table for class JobSheets.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class JobSheets.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobSheets and any vendor-defined subclasses, the category is
		''' class JobSheets itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobSheets)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobSheets and any vendor-defined subclasses, the category
		''' name is <CODE>"job-sheets"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-sheets"
			End Get
		End Property

	End Class

End Namespace
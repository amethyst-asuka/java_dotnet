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
	''' Class Severity is a printing attribute class, an enumeration, that denotes
	''' the severity of a <seealso cref="PrinterStateReason PrinterStateReason"/> attribute.
	''' <P>
	''' Instances of Severity do not appear in a Print Service's attribute set
	''' directly. Rather, a <seealso cref="PrinterStateReasons PrinterStateReasons"/>
	''' attribute appears in the Print Service's attribute set.
	'''  The {@link PrinterStateReasons
	''' PrinterStateReasons} attribute contains zero, one, or more than one {@link
	''' PrinterStateReason PrinterStateReason} objects which pertain to the Print
	''' Service's status, and each <seealso cref="PrinterStateReason PrinterStateReason"/>
	''' object is associated with a Severity level of REPORT (least severe),
	''' WARNING, or ERROR (most severe).
	''' The printer adds a {@link PrinterStateReason
	''' PrinterStateReason} object to the Print Service's
	''' <seealso cref="PrinterStateReasons PrinterStateReasons"/> attribute when the
	''' corresponding condition becomes true
	''' of the printer, and the printer removes the {@link PrinterStateReason
	''' PrinterStateReason} object again when the corresponding condition becomes
	''' false, regardless of whether the Print Service's overall
	''' <seealso cref="PrinterState PrinterState"/> also changed.
	''' <P>
	''' <B>IPP Compatibility:</B>
	''' <code>Severity.toString()</code> returns either "error", "warning", or
	''' "report".  The string values returned by
	''' each individual <seealso cref="PrinterStateReason"/> and
	''' associated <seealso cref="Severity"/> object's <CODE>toString()</CODE>
	''' methods, concatenated together with a hyphen (<CODE>"-"</CODE>) in
	''' between, gives the IPP keyword value for a <seealso cref="PrinterStateReasons"/>.
	''' The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class Severity
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.Attribute

		Private Const serialVersionUID As Long = 8781881462717925380L

		''' <summary>
		''' Indicates that the <seealso cref="PrinterStateReason PrinterStateReason"/> is a
		''' "report" (least severe). An implementation may choose to omit some or
		''' all reports.
		''' Some reports specify finer granularity about the printer state;
		''' others serve as a precursor to a warning. A report must contain nothing
		''' that could affect the printed output.
		''' </summary>
		Public Shared ReadOnly REPORT As New Severity(0)

		''' <summary>
		''' Indicates that the <seealso cref="PrinterStateReason PrinterStateReason"/> is a
		''' "warning." An implementation may choose to omit some or all warnings.
		''' Warnings serve as a precursor to an error. A warning must contain
		''' nothing  that prevents a job from completing, though in some cases the
		''' output may be of lower quality.
		''' </summary>
		Public Shared ReadOnly WARNING As New Severity(1)

		''' <summary>
		''' Indicates that the <seealso cref="PrinterStateReason PrinterStateReason"/> is an
		''' "error" (most severe). An implementation must include all errors.
		''' If this attribute contains one or more errors, the printer's
		''' <seealso cref="PrinterState PrinterState"/> must be STOPPED.
		''' </summary>
		Public Shared ReadOnly [ERROR] As New Severity(2)

		''' <summary>
		''' Construct a new severity enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "report", "warning", "error" }

		Private Shared ReadOnly myEnumValueTable As Severity() = { REPORT, WARNING, [ERROR] }

		''' <summary>
		''' Returns the string table for class Severity.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class Severity.
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
		''' For class Severity, the category is class Severity itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Severity)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Severit, the category name is <CODE>"severity"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "severity"
			End Get
		End Property

	End Class

End Namespace
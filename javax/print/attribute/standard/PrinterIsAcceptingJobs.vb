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
	''' Class PrinterIsAcceptingJobs is a printing attribute class, an enumeration,
	''' that indicates whether the printer is currently able to accept jobs. This
	''' value is independent of the <seealso cref="PrinterState PrinterState"/> and {@link
	''' PrinterStateReasons PrinterStateReasons} attributes because its value does
	''' not affect the current job; rather it affects future jobs. If the value is
	''' NOT_ACCEPTING_JOBS, the printer will reject jobs even when the {@link
	''' PrinterState PrinterState} is IDLE. If value is ACCEPTING_JOBS, the Printer
	''' will accept jobs even when the <seealso cref="PrinterState PrinterState"/> is STOPPED.
	''' <P>
	''' <B>IPP Compatibility:</B> The IPP boolean value is "true" for ACCEPTING_JOBS
	''' and "false" for NOT_ACCEPTING_JOBS. The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterIsAcceptingJobs
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = -5052010680537678061L

		''' <summary>
		''' The printer is currently rejecting any jobs sent to it.
		''' </summary>
		Public Shared ReadOnly NOT_ACCEPTING_JOBS As New PrinterIsAcceptingJobs(0)

		''' <summary>
		''' The printer is currently accepting jobs.
		''' </summary>
		Public Shared ReadOnly ACCEPTING_JOBS As New PrinterIsAcceptingJobs(1)

		''' <summary>
		''' Construct a new printer is accepting jobs enumeration value with the
		''' given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "not-accepting-jobs", "accepting-jobs" }

		Private Shared ReadOnly myEnumValueTable As PrinterIsAcceptingJobs() = { NOT_ACCEPTING_JOBS, ACCEPTING_JOBS }

		''' <summary>
		''' Returns the string table for class PrinterIsAcceptingJobs.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class PrinterIsAcceptingJobs.
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
		''' For class PrinterIsAcceptingJobs, the
		''' category is class PrinterIsAcceptingJobs itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterIsAcceptingJobs)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterIsAcceptingJobs, the
		''' category name is <CODE>"printer-is-accepting-jobs"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-is-accepting-jobs"
			End Get
		End Property

	End Class

End Namespace
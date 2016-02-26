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
	''' Class PrinterState is a printing attribute class, an enumeration, that
	''' identifies the current state of a printer. Class PrinterState defines
	''' standard printer state values. A Print Service implementation only needs
	''' to report those printer states which are appropriate for the particular
	''' implementation; it does not have to report every defined printer state. The
	''' <seealso cref="PrinterStateReasons PrinterStateReasons"/> attribute augments the
	''' PrinterState attribute to give more detailed information about the printer
	''' in  given printer state.
	''' <P>
	''' <B>IPP Compatibility:</B> The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterState
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = -649578618346507718L

		''' <summary>
		''' The printer state is unknown.
		''' </summary>
		Public Shared ReadOnly UNKNOWN As New PrinterState(0)

		''' <summary>
		''' Indicates that new jobs can start processing without waiting.
		''' </summary>
		Public Shared ReadOnly IDLE As New PrinterState(3)

		''' <summary>
		''' Indicates that jobs are processing;
		''' new jobs will wait before processing.
		''' </summary>
		Public Shared ReadOnly PROCESSING As New PrinterState(4)

		''' <summary>
		''' Indicates that no jobs can be processed and intervention is required.
		''' </summary>
		Public Shared ReadOnly STOPPED As New PrinterState(5)

		''' <summary>
		''' Construct a new printer state enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "unknown", Nothing, Nothing, "idle", "processing", "stopped" }

		Private Shared ReadOnly myEnumValueTable As PrinterState() = { UNKNOWN, Nothing, Nothing, IDLE, PROCESSING, STOPPED }

		''' <summary>
		''' Returns the string table for class PrinterState.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class PrinterState.
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
		''' For class PrinterState, the category is class PrinterState itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterState)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterState, the category name is <CODE>"printer-state"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-state"
			End Get
		End Property

	End Class

End Namespace
Imports System

'
' * Copyright (c) 2003, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' Class DialogTypeSelection is a printing attribute class, an enumeration,
	''' that indicates the user dialog type to be used for specifying
	''' printing options.
	''' If {@code NATIVE} is specified, then where available, a native
	''' platform dialog is displayed.
	''' If {@code COMMON} is specified, a cross-platform print dialog is displayed.
	''' 
	''' This option to specify a native dialog for use with an IPP attribute
	''' set provides a standard way to reflect back of the setting and option
	''' changes made by a user to the calling application, and integrates
	''' the native dialog into the Java printing APIs.
	''' But note that some options and settings in a native dialog may not
	''' necessarily map to IPP attributes as they may be non-standard platform,
	''' or even printer specific options.
	''' <P>
	''' <B>IPP Compatibility:</B> This is not an IPP attribute.
	''' <P>
	''' @since 1.7
	''' 
	''' </summary>
	Public NotInheritable Class DialogTypeSelection
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintRequestAttribute

		Private Const serialVersionUID As Long = 7518682952133256029L

		''' 
		Public Shared ReadOnly NATIVE As New DialogTypeSelection(0)

		''' 
		Public Shared ReadOnly COMMON As New DialogTypeSelection(1)

		''' <summary>
		''' Construct a new dialog type selection enumeration value with the
		''' given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
					MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "native", "common"}


		Private Shared ReadOnly myEnumValueTable As DialogTypeSelection() = { NATIVE, COMMON }

		''' <summary>
		''' Returns the string table for class DialogTypeSelection.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class DialogTypeSelection.
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
	   ''' For class DialogTypeSelection the category is class
	   ''' DialogTypeSelection itself.
	   ''' </summary>
	   ''' <returns>  Printing attribute class (category), an instance of class
	   '''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(DialogTypeSelection)
			End Get
		End Property


		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class DialogTypeSelection the category name is
		''' <CODE>"dialog-type-selection"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "dialog-type-selection"
			End Get
		End Property

	End Class

End Namespace
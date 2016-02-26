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
	''' Class PDLOverrideSupported is a printing attribute class, an enumeration,
	''' that expresses the printer's ability to attempt to override processing
	''' instructions embedded in documents' print data with processing instructions
	''' specified as attributes outside the print data.
	''' <P>
	''' <B>IPP Compatibility:</B> The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class PDLOverrideSupported
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = -4393264467928463934L

		''' <summary>
		''' The printer makes no attempt to make the external job attribute values
		''' take precedence over embedded instructions in the documents' print
		''' data.
		''' </summary>
		Public Shared ReadOnly NOT_ATTEMPTED As New PDLOverrideSupported(0)

		''' <summary>
		''' The printer attempts to make the external job attribute values take
		''' precedence over embedded instructions in the documents' print data,
		''' however there is no guarantee.
		''' </summary>
		Public Shared ReadOnly ATTEMPTED As New PDLOverrideSupported(1)


		''' <summary>
		''' Construct a new PDL override supported enumeration value with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "not-attempted", "attempted" }

		Private Shared ReadOnly myEnumValueTable As PDLOverrideSupported() = { NOT_ATTEMPTED, ATTEMPTED }

		''' <summary>
		''' Returns the string table for class PDLOverrideSupported.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class PDLOverrideSupported.
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
		''' For class PDLOverrideSupported and any vendor-defined subclasses, the
		''' category is class PDLOverrideSupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PDLOverrideSupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PDLOverrideSupported and any vendor-defined subclasses, the
		''' category name is <CODE>"pdl-override-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "pdl-override-supported"
			End Get
		End Property

	End Class

End Namespace
Imports System

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Class Fidelity is a printing attribute class, an enumeration,
	''' that indicates whether total fidelity to client supplied print request
	''' attributes is required.
	''' If FIDELITY_TRUE is specified and a service cannot print the job exactly
	''' as specified it must reject the job.
	''' If FIDELITY_FALSE is specified a reasonable attempt to print the job is
	''' acceptable. If not supplied the default is FIDELITY_FALSE.
	''' 
	''' <P>
	''' <B>IPP Compatibility:</B> The IPP boolean value is "true" for FIDELITY_TRUE
	''' and "false" for FIDELITY_FALSE. The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' See <a href="http://www.ietf.org/rfc/rfc2911.txt">RFC 2911</a> Section 15.1 for
	''' a fuller description of the IPP fidelity attribute.
	''' <P>
	''' 
	''' </summary>
	Public NotInheritable Class Fidelity
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintJobAttribute, javax.print.attribute.PrintRequestAttribute

		Private Const serialVersionUID As Long = 6320827847329172308L

		''' <summary>
		''' The job must be printed exactly as specified. or else rejected.
		''' </summary>
		Public Shared ReadOnly FIDELITY_TRUE As New Fidelity(0)

		''' <summary>
		''' The printer should make reasonable attempts to print the job,
		''' even if it cannot print it exactly as specified.
		''' </summary>
		Public Shared ReadOnly FIDELITY_FALSE As New Fidelity(1)

		''' <summary>
		''' Construct a new fidelity enumeration value with the
		''' given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "true", "false" }


		Private Shared ReadOnly myEnumValueTable As Fidelity() = { FIDELITY_TRUE, FIDELITY_FALSE }

		''' <summary>
		''' Returns the string table for class Fidelity.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class Fidelity.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return myEnumValueTable
			End Get
		End Property
	'     * Get the printing attribute class which is to be used as the "category"
	'     * for this printing attribute value.
	'     * <P>
	'     * For class Fidelity the category is class Fidelity itself.
	'     *
	'     * @return  Printing attribute class (category), an instance of class
	'     *          {@link java.lang.Class java.lang.Class}.
	'     
		Public Property category As Type
			Get
				Return GetType(Fidelity)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Fidelity the category name is
		''' <CODE>"ipp-attribute-fidelity"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "ipp-attribute-fidelity"
			End Get
		End Property

	End Class

End Namespace
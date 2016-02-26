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
	''' Class Copies is an integer valued printing attribute class that specifies the
	''' number of copies to be printed.
	''' <P>
	''' On many devices the supported number of collated copies will be limited by
	''' the number of physical output bins on the device, and may be different from
	''' the number of uncollated copies which can be supported.
	''' <P>
	''' The effect of a Copies attribute with a value of <I>n</I> on a multidoc print
	''' job (a job with multiple documents) depends on the (perhaps defaulted) value
	''' of the <seealso cref="MultipleDocumentHandling MultipleDocumentHandling"/> attribute:
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- The result will be <I>n</I> copies of a single output
	''' document comprising all the input docs.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- The result will be <I>n</I> copies of a single
	''' output document comprising all the input docs, and the first impression of
	''' each input doc will always start on a new media sheet.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- The result will be <I>n</I> copies of
	''' the first input document, followed by <I>n</I> copies of the second input
	''' document, . . . followed by <I>n</I> copies of the last input document.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- The result will be the first input
	''' document, the second input document, . . . the last input document, the group
	''' of documents being repeated <I>n</I> times.
	''' </UL>
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kamihensky
	''' </summary>
	Public NotInheritable Class Copies
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -6426631521680023833L

		''' <summary>
		''' Construct a new copies attribute with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''  (Unchecked exception) Thrown if <CODE>value</CODE> is less than 1. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 1, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this copies attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class Copies.
		''' <LI>
		''' This copies attribute's value and <CODE>object</CODE>'s value are
		''' equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this copies
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return MyBase.Equals([object]) AndAlso TypeOf [object] Is Copies
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class Copies, the category is class Copies itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Copies)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Copies, the category name is <CODE>"copies"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "copies"
			End Get
		End Property

	End Class

End Namespace
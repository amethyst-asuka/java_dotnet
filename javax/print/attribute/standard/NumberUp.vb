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
	''' Class NumberUp is an integer valued printing attribute class that specifies
	''' the number of print-stream pages to impose upon a single side of an
	''' instance of a selected medium. That is, if the NumberUp value is <I>n,</I>
	''' the printer must place <I>n</I> print-stream pages on a single side of
	''' an instance of the
	''' selected medium. To accomplish this, the printer may add some sort of
	''' translation, scaling, or rotation. This attribute primarily controls the
	''' translation, scaling and rotation of print-stream pages.
	''' <P>
	''' The effect of a NumberUp attribute on a multidoc print job (a job with
	''' multiple documents) depends on whether all the docs have the same number up
	''' values specified or whether different docs have different number up values
	''' specified, and on the (perhaps defaulted) value of the {@link
	''' MultipleDocumentHandling MultipleDocumentHandling} attribute.
	''' <UL>
	''' <LI>
	''' If all the docs have the same number up value <I>n</I> specified, then any
	''' value of <seealso cref="MultipleDocumentHandling MultipleDocumentHandling"/> makes
	''' sense, and the printer's processing depends on the {@link
	''' MultipleDocumentHandling MultipleDocumentHandling} value:
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- All the input docs will be combined together into one
	''' output document. Each media impression will consist of <I>n</I>m
	'''  print-stream pages from the output document.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- All the input docs will be combined together
	''' into one output document. Each media impression will consist of <I>n</I>
	''' print-stream pages from the output document. However, the first impression of
	''' each input doc will always start on a new media sheet; this means the last
	''' impression of an input doc may have fewer than <I>n</I> print-stream pages
	'''  on it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- The input docs will remain separate.
	''' Each media impression will consist of <I>n</I> print-stream pages from the
	''' input doc. Since the input docs are separate, the first impression of each
	''' input doc will always start on a new media sheet; this means the last
	''' impression of an input doc may have fewer than <I>n</I> print-stream pages on
	''' it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- The input docs will remain separate.
	''' Each media impression will consist of <I>n</I> print-stream pages from the
	''' input doc. Since the input docs are separate, the first impression of each
	''' input doc will always start on a new media sheet; this means the last
	''' impression of an input doc may have fewer than <I>n</I> print-stream pages
	''' on it.
	''' </UL>
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- All the input docs will be combined together into one
	''' output document. Each media impression will consist of <I>n<SUB>i</SUB></I>
	''' print-stream pages from the output document, where <I>i</I> is the number of
	''' the input doc corresponding to that point in the output document. When the
	''' next input doc has a different number up value from the previous input doc,
	''' the first print-stream page of the next input doc goes at the start of the
	''' next media impression, possibly leaving fewer than the full number of
	''' print-stream pages on the previous media impression.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- All the input docs will be combined together
	''' into one output document. Each media impression will consist of <I>n</I>
	''' print-stream pages from the output document. However, the first impression of
	''' each input doc will always start on a new media sheet; this means the last
	''' impression of an input doc may have fewer than <I>n</I> print-stream pages
	''' on it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- The input docs will remain separate.
	''' For input doc <I>i,</I> each media impression will consist of
	''' <I>n<SUB>i</SUB></I> print-stream pages from the input doc. Since the input
	''' docs are separate, the first impression of each input doc will always start
	''' on a new media sheet; this means the last impression of an input doc may have
	''' fewer than <I>n<SUB>i</SUB></I> print-stream pages on it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- The input docs will remain separate.
	''' For input doc <I>i,</I> each media impression will consist of
	''' <I>n<SUB>i</SUB></I> print-stream pages from the input doc. Since the input
	''' docs are separate, the first impression of each input doc will always start
	''' on a new media sheet; this means the last impression of an input doc may
	''' have fewer than <I>n<SUB>i</SUB></I> print-stream pages on it.
	''' </UL>
	''' </UL>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value.
	''' The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class NumberUp
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -3040436486786527811L


		''' <summary>
		''' Construct a new number up attribute with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if <CODE>value</CODE> is less than 1. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 1, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this number up attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class NumberUp.
		''' <LI>
		''' This number up attribute's value and <CODE>object</CODE>'s value are
		''' equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this number up
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is NumberUp)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class NumberUp, the category is class NumberUp itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(NumberUp)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class NumberUp, the category name is <CODE>"number-up"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "number-up"
			End Get
		End Property

	End Class

End Namespace
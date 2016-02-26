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
	''' Class SheetCollate is a printing attribute class, an enumeration, that
	''' specifies whether or not the media sheets of each copy of each printed
	''' document in a job are to be in sequence, when multiple copies of the document
	''' are specified by the <seealso cref="Copies Copies"/> attribute. When SheetCollate is
	''' COLLATED, each copy of each document is printed with the print-stream sheets
	''' in sequence. When SheetCollate is UNCOLLATED, each print-stream sheet is
	''' printed a number of times equal to the value of the <seealso cref="Copies Copies"/>
	''' attribute in succession. For example, suppose a document produces two media
	''' sheets as output, <seealso cref="Copies Copies"/> is 6, and SheetCollate is UNCOLLATED;
	''' in this case six copies of the first media sheet are printed followed by
	''' six copies of the second media sheet.
	''' <P>
	''' Whether the effect of sheet collation is achieved by placing copies of a
	''' document in multiple output bins or in the same output bin with
	''' implementation defined document separation is implementation dependent.
	''' Also whether it is achieved by making multiple passes over the job or by
	''' using an output sorter is implementation dependent.
	''' <P>
	''' If a printer does not support the SheetCollate attribute (meaning the client
	''' cannot specify any particular sheet collation), the printer must behave as
	''' though SheetCollate were always set to COLLATED.
	''' <P>
	''' The SheetCollate attribute interacts with the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} attribute. The {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} attribute describes the collation of entire
	''' documents, and the SheetCollate attribute describes the semantics of
	''' collating individual pages within a document.
	''' <P>
	''' The effect of a SheetCollate attribute on a multidoc print job (a job with
	''' multiple documents) depends on whether all the docs have the same sheet
	''' collation specified or whether different docs have different sheet
	''' collations specified, and on the (perhaps defaulted) value of the {@link
	''' MultipleDocumentHandling MultipleDocumentHandling} attribute.
	''' <UL>
	''' <LI>
	''' If all the docs have the same sheet collation specified, then the following
	''' combinations of SheetCollate and {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} are permitted, and the printer reports an error
	''' when the job is submitted if any other combination is specified:
	''' <UL>
	''' <LI>
	''' SheetCollate = COLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SINGLE_DOCUMENT -- All the input docs will be
	''' combined into one output document. Multiple copies of the output document
	''' will be produced with pages in collated order, i.e. pages 1, 2, 3, . . .,
	''' 1, 2, 3, . . .
	''' <P>
	''' <LI>
	''' SheetCollate = COLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SINGLE_DOCUMENT_NEW_SHEET -- All the input docs
	''' will be combined into one output document, and the first impression of each
	''' input doc will always start on a new media sheet. Multiple copies of the
	''' output document will be produced with pages in collated order, i.e. pages
	''' 1, 2, 3, . . ., 1, 2, 3, . . .
	''' <P>
	''' <LI>
	''' SheetCollate = COLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- Each
	''' input doc will remain a separate output document. Multiple copies of each
	''' output document (call them A, B, . . .) will be produced with each document's
	''' pages in collated order, but the documents themselves in uncollated order,
	''' i.e. pages A1, A2, A3, . . ., A1, A2, A3, . . ., B1, B2, B3, . . ., B1, B2,
	''' B3, . . .
	''' <P>
	''' <LI>
	''' SheetCollate = COLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SEPARATE_DOCUMENTS_COLLATED_COPIES -- Each input
	''' doc will remain a separate output document. Multiple copies of each output
	''' document (call them A, B, . . .) will be produced with each document's pages
	''' in collated order, with the documents themselves also in collated order, i.e.
	''' pages A1, A2, A3, . . ., B1, B2, B3, . . ., A1, A2, A3, . . ., B1, B2, B3,
	''' . . .
	''' <P>
	''' <LI>
	''' SheetCollate = UNCOLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SINGLE_DOCUMENT -- All the input docs will be
	''' combined into one output document. Multiple copies of the output document
	''' will be produced with pages in uncollated order, i.e. pages 1, 1, . . .,
	''' 2, 2, . . ., 3, 3, . . .
	''' <P>
	''' <LI>
	''' SheetCollate = UNCOLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SINGLE_DOCUMENT_NEW_SHEET -- All the input docs
	''' will be combined into one output document, and the first impression of each
	''' input doc will always start on a new media sheet. Multiple copies of the
	''' output document will be produced with pages in uncollated order, i.e. pages
	''' 1, 1, . . ., 2, 2, . . ., 3, 3, . . .
	''' <P>
	''' <LI>
	''' SheetCollate = UNCOLLATED, {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} = SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- Each
	''' input doc will remain a separate output document. Multiple copies of each
	''' output document (call them A, B, . . .) will be produced with each document's
	''' pages in uncollated order, with the documents themselves also in uncollated
	''' order, i.e. pages A1, A1, . . ., A2, A2, . . ., A3, A3, . . ., B1, B1, . . .,
	''' B2, B2, . . ., B3, B3, . . .
	''' </UL>
	''' <P>
	''' <LI>
	''' If different docs have different sheet collations specified, then only one
	''' value of <seealso cref="MultipleDocumentHandling MultipleDocumentHandling"/> is
	''' permitted, and the printer reports an error when the job is submitted if any
	''' other value is specified:
	''' <UL>
	''' <LI>
	''' <seealso cref="MultipleDocumentHandling MultipleDocumentHandling"/> =
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- Each input doc will remain a separate
	''' output document. Multiple copies of each output document (call them A, B,
	''' . . .) will be produced with each document's pages in collated or uncollated
	''' order as the corresponding input doc's SheetCollate attribute specifies, and
	''' with the documents themselves in uncollated order. If document A had
	''' SheetCollate = UNCOLLATED and document B had SheetCollate = COLLATED, the
	''' following pages would be produced: A1, A1, . . ., A2, A2, . . ., A3, A3,
	''' . . ., B1, B2, B3, . . ., B1, B2, B3, . . .
	''' </UL>
	''' </UL>
	''' <P>
	''' <B>IPP Compatibility:</B> SheetCollate is not an IPP attribute at present.
	''' <P>
	''' </summary>
	''' <seealso cref=  MultipleDocumentHandling
	''' 
	''' @author  Alan Kaminsky </seealso>
	Public NotInheritable Class SheetCollate
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 7080587914259873003L

		''' <summary>
		''' Sheets within a document appear in uncollated order when multiple
		''' copies are printed.
		''' </summary>
		Public Shared ReadOnly UNCOLLATED As New SheetCollate(0)

		''' <summary>
		''' Sheets within a document appear in collated order when multiple copies
		''' are printed.
		''' </summary>
		Public Shared ReadOnly COLLATED As New SheetCollate(1)

		''' <summary>
		''' Construct a new sheet collate enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "uncollated", "collated" }

		Private Shared ReadOnly myEnumValueTable As SheetCollate() = { UNCOLLATED, COLLATED }

		''' <summary>
		''' Returns the string table for class SheetCollate.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class SheetCollate.
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
		''' For class SheetCollate, the category is class SheetCollate itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(SheetCollate)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class SheetCollate, the category name is <CODE>"sheet-collate"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "sheet-collate"
			End Get
		End Property

	End Class

End Namespace
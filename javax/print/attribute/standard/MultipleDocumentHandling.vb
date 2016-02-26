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
	''' Class MultipleDocumentHandling is a printing attribute class, an enumeration,
	''' that controls finishing operations and the placement of one or more
	''' print-stream pages into impressions and onto media sheets. When the value of
	''' the <seealso cref="Copies Copies"/> attribute exceeds 1, MultipleDocumentHandling also
	''' controls the order in which the copies that result from processing the
	''' documents are produced. This attribute is relevant only for a multidoc print
	''' job consisting of two or more individual docs.
	''' <P>
	''' Briefly, MultipleDocumentHandling determines the relationship between the
	''' multiple input (electronic) documents fed into a multidoc print job and the
	''' output (physical) document or documents produced by the multidoc print job.
	''' There are two possibilities:
	''' <UL>
	''' <LI>
	''' The multiple input documents are combined into a single output document.
	''' Finishing operations (<seealso cref="Finishings Finishings"/>),
	''' are performed on this single output
	''' document. The <seealso cref="Copies Copies"/> attribute tells how many copies of this
	''' single output document to produce. The MultipleDocumentHandling values
	''' SINGLE_DOCUMENT and SINGLE_DOCUMENT_NEW_SHEET specify two variations of
	''' this  possibility.
	''' <P>
	''' <LI>
	''' The multiple input documents remain separate output documents. Finishing
	''' operations (<seealso cref="Finishings Finishings"/>),
	''' are performed on each output document
	''' separately. The <seealso cref="Copies Copies"/> attribute tells how many copies of each
	''' separate output document to produce. The MultipleDocumentHandling values
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES and SEPARATE_DOCUMENTS_COLLATED_COPIES
	''' specify two variations of this possibility.
	''' </UL>
	''' <P>
	''' In the detailed explanations below, if "<CODE>a</CODE>" represents an
	''' instance of document data, then the result of processing the data in
	''' document "<CODE>a</CODE>" is a sequence of media sheets represented by
	''' "<CODE>a(*)</CODE>".
	''' <P>
	''' The standard MultipleDocumentHandling values are:
	''' <UL>
	''' <LI>
	''' <A NAME="sdfi">{@link #SINGLE_DOCUMENT
	''' <B>SINGLE_DOCUMENT</B>}</A>. If a print job has multiple
	''' documents -- say, the document data is called <CODE>a</CODE> and
	''' <CODE>b</CODE> -- then the result of processing all the document data
	''' (<CODE>a</CODE> and then <CODE>b</CODE>) must be treated as a single sequence
	''' of media sheets for finishing operations; that is, finishing would be
	''' performed on the concatenation of the sequences <CODE>a(*),b(*)</CODE>. The
	''' printer must not force the data in each document instance to be formatted
	''' onto a new print-stream page, nor to start a new impression on a new media
	''' sheet. If more than one copy is made, the ordering of the sets of media
	''' sheets resulting from processing the document data must be
	''' <CODE>a(*),b(*),a(*),b(*),...</CODE>, and the printer object must force
	''' each copy (<CODE>a(*),b(*)</CODE>) to start on a new media sheet.
	''' <P>
	''' <LI>
	''' <A NAME="sducfi">{@link #SEPARATE_DOCUMENTS_UNCOLLATED_COPIES
	''' <B>SEPARATE_DOCUMENTS_UNCOLLATED_COPIES</B>}</A>. If a print job
	''' has multiple documents -- say, the document data is called <CODE>a</CODE> and
	''' <CODE>b</CODE> -- then the result of processing the data in each document
	''' instance must be treated as a single sequence of media sheets for finishing
	''' operations; that is, the sets <CODE>a(*)</CODE> and <CODE>b(*)</CODE> would
	''' each be finished separately. The printer must force each copy of the result
	''' of processing the data in a single document to start on a new media sheet.
	''' If more than one copy is made, the ordering of the sets of media sheets
	''' resulting from processing the document data must be
	''' <CODE>a(*),a(*),...,b(*),b(*)...</CODE>.
	''' <P>
	''' <LI>
	''' <A NAME="sdccfi">{@link #SEPARATE_DOCUMENTS_COLLATED_COPIES
	''' <B>SEPARATE_DOCUMENTS_COLLATED_COPIES</B>}</A>. If a print job
	''' has multiple documents -- say, the document data is called <CODE>a</CODE> and
	''' <CODE>b</CODE> -- then the result of processing the data in each document
	''' instance must be treated as a single sequence of media sheets for finishing
	''' operations; that is, the sets <CODE>a(*)</CODE> and <CODE>b(*)</CODE> would
	''' each be finished separately. The printer must force each copy of the result
	''' of processing the data in a single document to start on a new media sheet.
	''' If more than one copy is made, the ordering of the sets of media sheets
	''' resulting from processing the document data must be
	''' <CODE>a(*),b(*),a(*),b(*),...</CODE>.
	''' <P>
	''' <LI>
	''' <A NAME="sdnsfi">{@link #SINGLE_DOCUMENT_NEW_SHEET
	''' <B>SINGLE_DOCUMENT_NEW_SHEET</B>}</A>. Same as SINGLE_DOCUMENT,
	''' except that the printer must ensure that the first impression of each
	''' document instance in the job is placed on a new media sheet. This value
	''' allows multiple documents to be stapled together with a single staple where
	''' each document starts on a new sheet.
	''' </UL>
	''' <P>
	''' SINGLE_DOCUMENT is the same as SEPARATE_DOCUMENTS_COLLATED_COPIES with
	''' respect to ordering of print-stream pages, but not media sheet generation,
	''' since SINGLE_DOCUMENT will put the first page of the next document on the
	''' back side of a sheet if an odd number of pages have been produced so far
	''' for the job, while SEPARATE_DOCUMENTS_COLLATED_COPIES always forces the
	''' next document or document copy on to a new sheet.
	''' <P>
	''' In addition, if a <seealso cref="Finishings Finishings"/> attribute of
	''' <seealso cref="Finishings#STAPLE STAPLE"/> is specified, then:
	''' <UL>
	''' <LI>
	''' With SINGLE_DOCUMENT, documents <CODE>a</CODE> and <CODE>b</CODE> are
	''' stapled together as a single document with no regard to new sheets.
	''' <P>
	''' <LI>
	''' With SINGLE_DOCUMENT_NEW_SHEET, documents <CODE>a</CODE> and <CODE>b</CODE>
	''' are stapled together as a single document, but document <CODE>b</CODE>
	''' starts on a new sheet.
	''' <P>
	''' <LI>
	''' With SEPARATE_DOCUMENTS_UNCOLLATED_COPIES and
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES, documents <CODE>a</CODE> and
	''' <CODE>b</CODE> are stapled separately.
	''' </UL>
	''' <P>
	''' <I>Note:</I> None of these values provide means to produce uncollated
	''' sheets within a document, i.e., where multiple copies of sheet <I>n</I>
	''' are produced before sheet <I>n</I>+1 of the same document.
	''' To specify that, see the <seealso cref="SheetCollate SheetCollate"/> attribute.
	''' <P>
	''' <B>IPP Compatibility:</B> The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' </summary>
	''' <seealso cref=  Copies </seealso>
	''' <seealso cref=  Finishings </seealso>
	''' <seealso cref=  NumberUp </seealso>
	''' <seealso cref=  PageRanges </seealso>
	''' <seealso cref=  SheetCollate </seealso>
	''' <seealso cref=  Sides
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky </seealso>
	Public Class MultipleDocumentHandling
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 8098326460746413466L


		''' <summary>
		''' Single document -- see above for <A HREF="#sdfi">further
		''' information</A>.
		''' </summary>
		Public Shared ReadOnly SINGLE_DOCUMENT As New MultipleDocumentHandling(0)

		''' <summary>
		''' Separate documents uncollated copies -- see above for
		''' <A HREF="#sducfi">further information</A>.
		''' </summary>
		Public Shared ReadOnly SEPARATE_DOCUMENTS_UNCOLLATED_COPIES As New MultipleDocumentHandling(1)

		''' <summary>
		''' Separate documents collated copies -- see above for
		''' <A HREF="#sdccfi">further information</A>.
		''' </summary>
		Public Shared ReadOnly SEPARATE_DOCUMENTS_COLLATED_COPIES As New MultipleDocumentHandling(2)

		''' <summary>
		''' Single document new sheet -- see above for
		''' <A HREF="#sdnsfi">further information</A>.
		''' </summary>
		Public Shared ReadOnly SINGLE_DOCUMENT_NEW_SHEET As New MultipleDocumentHandling(3)


		''' <summary>
		''' Construct a new multiple document handling enumeration value with the
		''' given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "single-document", "separate-documents-uncollated-copies", "separate-documents-collated-copies", "single-document-new-sheet" }

		Private Shared ReadOnly myEnumValueTable As MultipleDocumentHandling() = { SINGLE_DOCUMENT, SEPARATE_DOCUMENTS_UNCOLLATED_COPIES, SEPARATE_DOCUMENTS_COLLATED_COPIES, SINGLE_DOCUMENT_NEW_SHEET }

		''' <summary>
		''' Returns the string table for class MultipleDocumentHandling.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class MultipleDocumentHandling.
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
		''' For class MultipleDocumentHandling and any vendor-defined subclasses,
		''' the category is class MultipleDocumentHandling itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(MultipleDocumentHandling)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class MultipleDocumentHandling and any vendor-defined subclasses,
		''' the category name is <CODE>"multiple-document-handling"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "multiple-document-handling"
			End Get
		End Property

	End Class

End Namespace
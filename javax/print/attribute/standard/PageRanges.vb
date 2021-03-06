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
	''' Class PageRanges is a printing attribute class, a set of integers, that
	''' identifies the range(s) of print-stream pages that the Printer object uses
	''' for each copy of each document which are to be printed. Nothing is printed
	''' for any pages identified that do not exist in the document(s). The attribute
	''' is associated with <I>print-stream</I> pages, not application-numbered pages
	''' (for example, the page numbers found in the headers and or footers for
	''' certain word processing applications).
	''' <P>
	''' In most cases, the exact pages to be printed will be generated by a device
	''' driver and this attribute would not be required. However, when printing an
	''' archived document which has already been formatted, the end user may elect to
	''' print just a subset of the pages contained in the document. In this case, if
	''' a page range of <CODE>"<I>n</I>-<I>m</I>"</CODE> is specified, the first page
	''' to be printed will be page <I>n.</I> All subsequent pages of the document
	''' will be printed through and including page <I>m.</I>
	''' <P>
	''' If a PageRanges attribute is not specified for a print job, all pages of
	''' the document will be printed. In other words, the default value for the
	''' PageRanges attribute is always <CODE>{{1, Integer.MAX_VALUE}}</CODE>.
	''' <P>
	''' The effect of a PageRanges attribute on a multidoc print job (a job with
	''' multiple documents) depends on whether all the docs have the same page ranges
	''' specified or whether different docs have different page ranges specified, and
	''' on the (perhaps defaulted) value of the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} attribute.
	''' <UL>
	''' <LI>
	''' If all the docs have the same page ranges specified, then any value of {@link
	''' MultipleDocumentHandling MultipleDocumentHandling} makes sense, and the
	''' printer's processing depends on the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} value:
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- All the input docs will be combined together into one
	''' output document. The specified page ranges of that output document will be
	''' printed.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- All the input docs will be combined together
	''' into one output document, and the first impression of each input doc will
	''' always start on a new media sheet. The specified page ranges of that output
	''' document will be printed.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- For each separate input doc, the
	''' specified page ranges will be printed.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- For each separate input doc, the
	''' specified page ranges will be printed.
	''' </UL>
	''' <UL>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- For each separate input doc, its own
	''' specified page ranges will be printed..
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- For each separate input doc, its own
	''' specified page ranges will be printed..
	''' </UL>
	''' </UL>
	''' <P>
	''' <B>IPP Compatibility:</B> The PageRanges attribute's canonical array form
	''' gives the lower and upper bound for each range of pages to be included in
	''' and IPP "page-ranges" attribute. See class {@link
	''' javax.print.attribute.SetOfIntegerSyntax SetOfIntegerSyntax} for an
	''' explanation of canonical array form. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PageRanges
		Inherits javax.print.attribute.SetOfIntegerSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 8639895197656148392L


		''' <summary>
		''' Construct a new page ranges attribute with the given members. The
		''' members are specified in "array form;" see class {@link
		''' javax.print.attribute.SetOfIntegerSyntax SetOfIntegerSyntax} for an
		''' explanation of array form.
		''' </summary>
		''' <param name="members">  Set members in array form.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>members</CODE> is null or
		'''     any element of <CODE>members</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if any element of
		'''   <CODE>members</CODE> is not a length-one or length-two array. Also
		'''     thrown if <CODE>members</CODE> is a zero-length array or if any
		'''     member of the set is less than 1. </exception>
		Public Sub New(ByVal members As Integer()())
			MyBase.New(members)
			If members Is Nothing Then Throw New NullPointerException("members is null")
			myPageRanges()
		End Sub
		''' <summary>
		''' Construct a new  page ranges attribute with the given members in
		''' string form.
		''' See class {@link javax.print.attribute.SetOfIntegerSyntax
		''' SetOfIntegerSyntax}
		''' for explanation of the syntax.
		''' </summary>
		''' <param name="members">  Set members in string form.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>members</CODE> is null or
		'''     any element of <CODE>members</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>members</CODE> does not
		'''    obey  the proper syntax.  Also
		'''     thrown if the constructed set-of-integer is a
		'''     zero-length array or if any
		'''     member of the set is less than 1. </exception>
		Public Sub New(ByVal members As String)
			MyBase.New(members)
			If members Is Nothing Then Throw New NullPointerException("members is null")
			myPageRanges()
		End Sub

		Private Sub myPageRanges()
			Dim myMembers As Integer()() = members
			Dim n As Integer = myMembers.Length
			If n = 0 Then Throw New System.ArgumentException("members is zero-length")
			Dim i As Integer
			For i = 0 To n - 1
			  If myMembers(i)(0) < 1 Then Throw New System.ArgumentException("Page value < 1 specified")
			Next i
		End Sub

		''' <summary>
		''' Construct a new page ranges attribute containing a single integer. That
		''' is, only the one page is to be printed.
		''' </summary>
		''' <param name="member">  Set member.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>member</CODE> is less than
		'''     1. </exception>
		Public Sub New(ByVal member As Integer)
			MyBase.New(member)
			If member < 1 Then Throw New System.ArgumentException("Page value < 1 specified")
		End Sub

		''' <summary>
		''' Construct a new page ranges attribute containing a single range of
		''' integers. That is, only those pages in the one range are to be printed.
		''' </summary>
		''' <param name="lowerBound">  Lower bound of the range. </param>
		''' <param name="upperBound">  Upper bound of the range.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if a null range is specified or if a
		'''     non-null range is specified with <CODE>lowerBound</CODE> less than
		'''     1. </exception>
		Public Sub New(ByVal lowerBound As Integer, ByVal upperBound As Integer)
			MyBase.New(lowerBound, upperBound)
			If lowerBound > upperBound Then
				Throw New System.ArgumentException("Null range specified")
			ElseIf lowerBound < 1 Then
				Throw New System.ArgumentException("Page value < 1 specified")
			End If
		End Sub

		''' <summary>
		''' Returns whether this page ranges attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PageRanges.
		''' <LI>
		''' This page ranges attribute's members and <CODE>object</CODE>'s members
		''' are the same.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this page ranges
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PageRanges)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PageRanges, the category is class PageRanges itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PageRanges)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PageRanges, the category name is <CODE>"page-ranges"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "page-ranges"
			End Get
		End Property

	End Class

End Namespace
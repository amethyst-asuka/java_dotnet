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
	''' Class Sides is a printing attribute class, an enumeration, that specifies
	''' how print-stream pages are to be imposed upon the sides of an instance of a
	''' selected medium, i.e., an impression.
	''' <P>
	''' The effect of a Sides attribute on a multidoc print job (a job with multiple
	''' documents) depends on whether all the docs have the same sides values
	''' specified or whether different docs have different sides values specified,
	''' and on the (perhaps defaulted) value of the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} attribute.
	''' <UL>
	''' <LI>
	''' If all the docs have the same sides value <I>n</I> specified, then any value
	''' of <seealso cref="MultipleDocumentHandling MultipleDocumentHandling"/> makes sense,
	''' and the printer's processing depends on the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} value:
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- All the input docs will be combined together into one
	''' output document. Each media sheet will consist of <I>n</I> impressions from
	''' the output document.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- All the input docs will be combined together
	''' into one output document. Each media sheet will consist of <I>n</I>
	''' impressions from the output document. However, the first impression of each
	''' input doc will always start on a new media sheet; this means the last media
	''' sheet of an input doc may have only one impression on it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- The input docs will remain separate.
	''' Each media sheet will consist of <I>n</I> impressions from the input doc.
	''' Since the input docs are separate, the first impression of each input doc
	''' will always start on a new media sheet; this means the last media sheet of
	''' an input doc may have only one impression on it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- The input docs will remain separate.
	''' Each media sheet will consist of <I>n</I> impressions from the input doc.
	''' Since the input docs are separate, the first impression of each input doc
	''' will always start on a new media sheet; this means the last media sheet of
	''' an input doc may have only one impression on it.
	''' </UL>
	''' <P>
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- All the input docs will be combined together into one
	''' output document. Each media sheet will consist of <I>n<SUB>i</SUB></I>
	''' impressions from the output document, where <I>i</I> is the number of the
	''' input doc corresponding to that point in the output document. When the next
	''' input doc has a different sides value from the previous input doc, the first
	''' print-stream page of the next input doc goes at the start of the next media
	''' sheet, possibly leaving only one impression on the previous media sheet.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- All the input docs will be combined together
	''' into one output document. Each media sheet will consist of <I>n</I>
	''' impressions from the output document. However, the first impression of each
	''' input doc will always start on a new media sheet; this means the last
	''' impression of an input doc may have only one impression on it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- The input docs will remain separate.
	''' For input doc <I>i,</I> each media sheet will consist of <I>n<SUB>i</SUB></I>
	''' impressions from the input doc. Since the input docs are separate, the first
	''' impression of each input doc will always start on a new media sheet; this
	''' means the last media sheet of an input doc may have only one impression on
	''' it.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- The input docs will remain separate.
	''' For input doc <I>i,</I> each media sheet will consist of <I>n<SUB>i</SUB></I>
	''' impressions from the input doc. Since the input docs are separate, the first
	''' impression of each input doc will always start on a new media sheet; this
	''' means the last media sheet of an input doc may have only one impression on
	''' it.
	''' </UL>
	''' </UL>
	''' <P>
	''' <B>IPP Compatibility:</B> The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>

	Public NotInheritable Class Sides
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -6890309414893262822L

		''' <summary>
		''' Imposes each consecutive print-stream page upon the same side of
		''' consecutive media sheets.
		''' </summary>
		Public Shared ReadOnly ONE_SIDED As New Sides(0)

		''' <summary>
		''' Imposes each consecutive pair of print-stream pages upon front and back
		''' sides of consecutive media sheets, such that the orientation of each
		''' pair of print-stream pages on the medium would be correct for the
		''' reader as if for binding on the long edge. This imposition is also
		''' known as "duplex" (see <seealso cref="#DUPLEX DUPLEX"/>).
		''' </summary>
		Public Shared ReadOnly TWO_SIDED_LONG_EDGE As New Sides(1)

		''' <summary>
		''' Imposes each consecutive pair of print-stream pages upon front and back
		''' sides of consecutive media sheets, such that the orientation of each
		''' pair of print-stream pages on the medium would be correct for the
		''' reader as if for binding on the short edge. This imposition is also
		''' known as "tumble" (see <seealso cref="#TUMBLE TUMBLE"/>).
		''' </summary>
		Public Shared ReadOnly TWO_SIDED_SHORT_EDGE As New Sides(2)

		''' <summary>
		''' An alias for "two sided long edge" (see {@link #TWO_SIDED_LONG_EDGE
		''' TWO_SIDED_LONG_EDGE}).
		''' </summary>
		Public Shared ReadOnly DUPLEX As Sides = TWO_SIDED_LONG_EDGE

		''' <summary>
		''' An alias for "two sided short edge" (see {@link #TWO_SIDED_SHORT_EDGE
		''' TWO_SIDED_SHORT_EDGE}).
		''' </summary>
		Public Shared ReadOnly TUMBLE As Sides = TWO_SIDED_SHORT_EDGE

		''' <summary>
		''' Construct a new sides enumeration value with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "one-sided", "two-sided-long-edge", "two-sided-short-edge" }

		Private Shared ReadOnly myEnumValueTable As Sides() = { ONE_SIDED, TWO_SIDED_LONG_EDGE, TWO_SIDED_SHORT_EDGE }

		''' <summary>
		''' Returns the string table for class Sides.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class Sides.
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
		''' For class Sides, the category is class Sides itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Sides)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Sides, the category name is <CODE>"sides"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "sides"
			End Get
		End Property

	End Class

End Namespace
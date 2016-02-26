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
	''' Class Finishings is a printing attribute class, an enumeration, that
	''' identifies whether the printer applies a finishing operation of some kind
	''' of binding to each copy of each printed document in the job. For multidoc
	''' print jobs (jobs with multiple documents), the
	''' {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} attribute determines what constitutes a "copy"
	''' for purposes of finishing.
	''' <P>
	''' Standard Finishings values are:
	''' <TABLE BORDER=0 CELLPADDING=0 CELLSPACING=0 WIDTH=100% SUMMARY="layout">
	''' <TR>
	''' <TD STYLE="WIDTH:10%">
	''' &nbsp;
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#NONE NONE"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE STAPLE"/>
	''' </TD>
	''' <TD STYLE="WIDTH:36%">
	''' <seealso cref="#EDGE_STITCH EDGE_STITCH"/>
	''' </TD>
	''' </TR>
	''' <TR>
	''' <TD>
	''' &nbsp;
	''' </TD>
	''' <TD>
	''' <seealso cref="#BIND BIND"/>
	''' </TD>
	''' <TD>
	''' <seealso cref="#SADDLE_STITCH SADDLE_STITCH"/>
	''' </TD>
	''' <TD>
	''' <seealso cref="#COVER COVER"/>
	''' </TD>
	''' <TD>
	''' &nbsp;
	''' </TD>
	''' </TR>
	''' </TABLE>
	''' <P>
	''' The following Finishings values are more specific; they indicate a
	''' corner or an edge as if the document were a portrait document:
	''' <TABLE BORDER=0 CELLPADDING=0 CELLSPACING=0 WIDTH=100% SUMMARY="layout">
	''' <TR>
	''' <TD STYLE="WIDTH:10%">
	''' &nbsp;
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_TOP_LEFT STAPLE_TOP_LEFT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#EDGE_STITCH_LEFT EDGE_STITCH_LEFT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_DUAL_LEFT STAPLE_DUAL_LEFT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:9%">
	''' &nbsp;
	''' </TD>
	''' </TR>
	''' <TR>
	''' <TD STYLE="WIDTH:10%">
	''' &nbsp;
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_BOTTOM_LEFT STAPLE_BOTTOM_LEFT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#EDGE_STITCH_TOP EDGE_STITCH_TOP"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_DUAL_TOP STAPLE_DUAL_TOP"/>
	''' </TD>
	''' <TD STYLE="WIDTH:9%">
	''' &nbsp;
	''' </TD>
	''' </TR>
	''' <TR>
	''' <TD STYLE="WIDTH:10%">
	''' &nbsp;
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_TOP_RIGHT STAPLE_TOP_RIGHT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#EDGE_STITCH_RIGHT EDGE_STITCH_RIGHT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_DUAL_RIGHT STAPLE_DUAL_RIGHT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:9%">
	''' &nbsp;
	''' </TD>
	''' </TR>
	''' <TR>
	''' <TD STYLE="WIDTH:10%">
	''' &nbsp;
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_BOTTOM_RIGHT STAPLE_BOTTOM_RIGHT"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#EDGE_STITCH_BOTTOM EDGE_STITCH_BOTTOM"/>
	''' </TD>
	''' <TD STYLE="WIDTH:27%">
	''' <seealso cref="#STAPLE_DUAL_BOTTOM STAPLE_DUAL_BOTTOM"/>
	''' </TD>
	''' <TD STYLE="WIDTH:9%">
	''' &nbsp;
	''' </TD>
	''' </TR>
	''' </TABLE>
	''' <P>
	''' The STAPLE_<I>XXX</I> values are specified with respect to the
	''' document as if the document were a portrait document. If the document is
	''' actually a landscape or a reverse-landscape document, the client supplies the
	''' appropriate transformed value. For example, to position a staple in the upper
	''' left hand corner of a landscape document when held for reading, the client
	''' supplies the STAPLE_BOTTOM_LEFT value (since landscape is
	''' defined as a +90 degree rotation from portrait, i.e., anti-clockwise). On the
	''' other hand, to position a staple in the upper left hand corner of a
	''' reverse-landscape document when held for reading, the client supplies the
	''' STAPLE_TOP_RIGHT value (since reverse-landscape is defined as a
	''' -90 degree rotation from portrait, i.e., clockwise).
	''' <P>
	''' The angle (vertical, horizontal, angled) of each staple with respect to the
	''' document depends on the implementation which may in turn depend on the value
	''' of the attribute.
	''' <P>
	''' The effect of a Finishings attribute on a multidoc print job (a job
	''' with multiple documents) depends on whether all the docs have the same
	''' binding specified or whether different docs have different bindings
	''' specified, and on the (perhaps defaulted) value of the {@link
	''' MultipleDocumentHandling MultipleDocumentHandling} attribute.
	''' <UL>
	''' <LI>
	''' If all the docs have the same binding specified, then any value of {@link
	''' MultipleDocumentHandling MultipleDocumentHandling} makes sense, and the
	''' printer's processing depends on the {@link MultipleDocumentHandling
	''' MultipleDocumentHandling} value:
	''' <UL>
	''' <LI>
	''' SINGLE_DOCUMENT -- All the input docs will be bound together as one output
	''' document with the specified binding.
	''' <P>
	''' <LI>
	''' SINGLE_DOCUMENT_NEW_SHEET -- All the input docs will be bound together as one
	''' output document with the specified binding, and the first impression of each
	''' input doc will always start on a new media sheet.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- Each input doc will be bound
	''' separately with the specified binding.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- Each input doc will be bound separately
	''' with the specified binding.
	''' </UL>
	''' <P>
	''' <LI>
	''' If different docs have different bindings specified, then only two values of
	''' <seealso cref="MultipleDocumentHandling MultipleDocumentHandling"/> make sense, and the
	''' printer reports an error when the job is submitted if any other value is
	''' specified:
	''' <UL>
	''' <LI>
	''' SEPARATE_DOCUMENTS_UNCOLLATED_COPIES -- Each input doc will be bound
	''' separately with its own specified binding.
	''' <P>
	''' <LI>
	''' SEPARATE_DOCUMENTS_COLLATED_COPIES -- Each input doc will be bound separately
	''' with its own specified binding.
	''' </UL>
	''' </UL>
	''' <P>
	''' <B>IPP Compatibility:</B> Class Finishings encapsulates some of the
	''' IPP enum values that can be included in an IPP "finishings" attribute, which
	''' is a set of enums. The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' In IPP Finishings is a multi-value attribute, this API currently allows
	''' only one binding to be specified.
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class Finishings
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -627840419548391754L

		''' <summary>
		''' Perform no binding.
		''' </summary>
		Public Shared ReadOnly NONE As New Finishings(3)

		''' <summary>
		''' Bind the document(s) with one or more staples. The exact number and
		''' placement of the staples is site-defined.
		''' </summary>
		Public Shared ReadOnly STAPLE As New Finishings(4)

		''' <summary>
		''' This value is specified when it is desired to select a non-printed (or
		''' pre-printed) cover for the document. This does not supplant the
		''' specification of a printed cover (on cover stock medium) by the
		''' document  itself.
		''' </summary>
		Public Shared ReadOnly COVER As New Finishings(6)

		''' <summary>
		''' This value indicates that a binding is to be applied to the document;
		''' the type and placement of the binding is site-defined.
		''' </summary>
		Public Shared ReadOnly BIND As New Finishings(7)

		''' <summary>
		''' Bind the document(s) with one or more staples (wire stitches) along the
		''' middle fold. The exact number and placement of the staples and the
		''' middle fold is implementation- and/or site-defined.
		''' </summary>
		Public Shared ReadOnly SADDLE_STITCH As New Finishings(8)

		''' <summary>
		''' Bind the document(s) with one or more staples (wire stitches) along one
		''' edge. The exact number and placement of the staples is implementation-
		''' and/or site- defined.
		''' </summary>
		Public Shared ReadOnly EDGE_STITCH As New Finishings(9)

		''' <summary>
		''' Bind the document(s) with one or more staples in the top left corner.
		''' </summary>
		Public Shared ReadOnly STAPLE_TOP_LEFT As New Finishings(20)

		''' <summary>
		''' Bind the document(s) with one or more staples in the bottom left
		''' corner.
		''' </summary>
		Public Shared ReadOnly STAPLE_BOTTOM_LEFT As New Finishings(21)

		''' <summary>
		''' Bind the document(s) with one or more staples in the top right corner.
		''' </summary>
		Public Shared ReadOnly STAPLE_TOP_RIGHT As New Finishings(22)

		''' <summary>
		''' Bind the document(s) with one or more staples in the bottom right
		''' corner.
		''' </summary>
		Public Shared ReadOnly STAPLE_BOTTOM_RIGHT As New Finishings(23)

		''' <summary>
		''' Bind the document(s) with one or more staples (wire stitches) along the
		''' left edge. The exact number and placement of the staples is
		''' implementation- and/or site-defined.
		''' </summary>
		Public Shared ReadOnly EDGE_STITCH_LEFT As New Finishings(24)

		''' <summary>
		''' Bind the document(s) with one or more staples (wire stitches) along the
		''' top edge. The exact number and placement of the staples is
		''' implementation- and/or site-defined.
		''' </summary>
		Public Shared ReadOnly EDGE_STITCH_TOP As New Finishings(25)

		''' <summary>
		''' Bind the document(s) with one or more staples (wire stitches) along the
		''' right edge. The exact number and placement of the staples is
		''' implementation- and/or site-defined.
		''' </summary>
		Public Shared ReadOnly EDGE_STITCH_RIGHT As New Finishings(26)

		''' <summary>
		''' Bind the document(s) with one or more staples (wire stitches) along the
		''' bottom edge. The exact number and placement of the staples is
		''' implementation- and/or site-defined.
		''' </summary>
		Public Shared ReadOnly EDGE_STITCH_BOTTOM As New Finishings(27)

		''' <summary>
		''' Bind the document(s) with two staples (wire stitches) along the left
		''' edge assuming a portrait document (see above).
		''' </summary>
		Public Shared ReadOnly STAPLE_DUAL_LEFT As New Finishings(28)

		''' <summary>
		''' Bind the document(s) with two staples (wire stitches) along the top
		''' edge assuming a portrait document (see above).
		''' </summary>
		Public Shared ReadOnly STAPLE_DUAL_TOP As New Finishings(29)

		''' <summary>
		''' Bind the document(s) with two staples (wire stitches) along the right
		''' edge assuming a portrait document (see above).
		''' </summary>
		Public Shared ReadOnly STAPLE_DUAL_RIGHT As New Finishings(30)

		''' <summary>
		''' Bind the document(s) with two staples (wire stitches) along the bottom
		''' edge assuming a portrait document (see above).
		''' </summary>
		Public Shared ReadOnly STAPLE_DUAL_BOTTOM As New Finishings(31)

		''' <summary>
		''' Construct a new finishings binding enumeration value with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = {"none", "staple", Nothing, "cover", "bind", "saddle-stitch", "edge-stitch", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "staple-top-left", "staple-bottom-left", "staple-top-right", "staple-bottom-right", "edge-stitch-left", "edge-stitch-top", "edge-stitch-right", "edge-stitch-bottom", "staple-dual-left", "staple-dual-top", "staple-dual-right", "staple-dual-bottom" }

		Private Shared ReadOnly myEnumValueTable As Finishings() = {NONE, STAPLE, Nothing, COVER, BIND, SADDLE_STITCH, EDGE_STITCH, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, STAPLE_TOP_LEFT, STAPLE_BOTTOM_LEFT, STAPLE_TOP_RIGHT, STAPLE_BOTTOM_RIGHT, EDGE_STITCH_LEFT, EDGE_STITCH_TOP, EDGE_STITCH_RIGHT, EDGE_STITCH_BOTTOM, STAPLE_DUAL_LEFT, STAPLE_DUAL_TOP, STAPLE_DUAL_RIGHT, STAPLE_DUAL_BOTTOM }

		''' <summary>
		''' Returns the string table for class Finishings.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class Finishings.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property

		''' <summary>
		''' Returns the lowest integer value used by class Finishings.
		''' </summary>
		Protected Friend Property Overrides offset As Integer
			Get
				Return 3
			End Get
		End Property

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class Finishings and any vendor-defined subclasses, the
		''' category is class Finishings itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Finishings)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Finishings and any vendor-defined subclasses, the
		''' category name is <CODE>"finishings"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "finishings"
			End Get
		End Property

	End Class

End Namespace
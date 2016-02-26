Imports System

'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Class PresentationDirection is a printing attribute class, an enumeration,
	''' that is used in conjunction with the <seealso cref=" NumberUp NumberUp"/> attribute to
	''' indicate the layout of multiple print-stream pages to impose upon a
	''' single side of an instance of a selected medium.
	''' This is useful to mirror the text layout conventions of different scripts.
	''' For example, English is "toright-tobottom", Hebrew is "toleft-tobottom"
	'''  and Japanese is usually "tobottom-toleft".
	''' <P>
	''' <B>IPP Compatibility:</B>  This attribute is not an IPP 1.1
	''' attribute; it is an attribute in the Production Printing Extension
	''' (<a href="ftp://ftp.pwg.org/pub/pwg/standards/pwg5100.3.pdf">PDF</a>)
	''' of IPP 1.1.  The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Phil Race.
	''' </summary>
	Public NotInheritable Class PresentationDirection
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.PrintJobAttribute, javax.print.attribute.PrintRequestAttribute

		Private Const serialVersionUID As Long = 8294728067230931780L

		''' <summary>
		''' Pages are laid out in columns starting at the top left,
		''' proceeding towards the bottom {@literal &} right.
		''' </summary>
		Public Shared ReadOnly TOBOTTOM_TORIGHT As New PresentationDirection(0)

		''' <summary>
		''' Pages are laid out in columns starting at the top right,
		''' proceeding towards the bottom {@literal &} left.
		''' </summary>
		Public Shared ReadOnly TOBOTTOM_TOLEFT As New PresentationDirection(1)

		''' <summary>
		''' Pages are laid out in columns starting at the bottom left,
		''' proceeding towards the top {@literal &} right.
		''' </summary>
		Public Shared ReadOnly TOTOP_TORIGHT As New PresentationDirection(2)

		''' <summary>
		''' Pages are laid out in columns starting at the bottom right,
		''' proceeding towards the top {@literal &} left.
		''' </summary>
		Public Shared ReadOnly TOTOP_TOLEFT As New PresentationDirection(3)

		''' <summary>
		''' Pages are laid out in rows starting at the top left,
		''' proceeding towards the right {@literal &} bottom.
		''' </summary>
		Public Shared ReadOnly TORIGHT_TOBOTTOM As New PresentationDirection(4)

		''' <summary>
		''' Pages are laid out in rows starting at the bottom left,
		''' proceeding towards the right {@literal &} top.
		''' </summary>
		Public Shared ReadOnly TORIGHT_TOTOP As New PresentationDirection(5)

		''' <summary>
		''' Pages are laid out in rows starting at the top right,
		''' proceeding towards the left {@literal &} bottom.
		''' </summary>
		Public Shared ReadOnly TOLEFT_TOBOTTOM As New PresentationDirection(6)

		''' <summary>
		''' Pages are laid out in rows starting at the bottom right,
		''' proceeding towards the left {@literal &} top.
		''' </summary>
		Public Shared ReadOnly TOLEFT_TOTOP As New PresentationDirection(7)

		''' <summary>
		''' Construct a new presentation direction enumeration value with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Private Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "tobottom-toright", "tobottom-toleft", "totop-toright", "totop-toleft", "toright-tobottom", "toright-totop", "toleft-tobottom", "toleft-totop" }

		Private Shared ReadOnly myEnumValueTable As PresentationDirection() = { TOBOTTOM_TORIGHT, TOBOTTOM_TOLEFT, TOTOP_TORIGHT, TOTOP_TOLEFT, TORIGHT_TOBOTTOM, TORIGHT_TOTOP, TOLEFT_TOBOTTOM, TOLEFT_TOTOP }

		''' <summary>
		''' Returns the string table for class PresentationDirection.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class PresentationDirection.
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
		''' For class PresentationDirection
		''' the category is class PresentationDirection itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PresentationDirection)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PresentationDirection
		''' the category name is <CODE>"presentation-direction"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "presentation-direction"
			End Get
		End Property

	End Class

End Namespace
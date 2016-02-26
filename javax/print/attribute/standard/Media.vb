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
	''' Class Media is a printing attribute class that specifies the
	''' medium on which to print.
	''' <p>
	''' Media may be specified in different ways.
	''' <ul>
	''' <li> it may be specified by paper source - eg paper tray
	''' <li> it may be specified by a standard size - eg "A4"
	''' <li> it may be specified by a name - eg "letterhead"
	''' </ul>
	''' Each of these corresponds to the IPP "media" attribute.
	''' The current API does not support describing media by characteristics
	''' (eg colour, opacity).
	''' This may be supported in a later revision of the specification.
	''' <p>
	''' A Media object is constructed with a value which represents
	''' one of the ways in which the Media attribute can be specified.
	''' <p>
	''' <B>IPP Compatibility:</B>  The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author Phil Race
	''' </summary>
	Public MustInherit Class Media
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute, javax.print.attribute.PrintRequestAttribute, javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = -2823970704630722439L

		''' <summary>
		''' Constructs a new media attribute specified by name.
		''' </summary>
		''' <param name="value">         a value </param>
		Protected Friend Sub New(ByVal value As Integer)
			   MyBase.New(value)
		End Sub

		''' <summary>
		''' Returns whether this media attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is of the same subclass of Media as this object.
		''' <LI>
		''' The values are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this media
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is Media AndAlso [object].GetType() Is Me.GetType() AndAlso CType([object], Media).value = Me.value)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class Media and any vendor-defined subclasses, the category is
		''' class Media itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Media)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Media and any vendor-defined subclasses, the category name is
		''' <CODE>"media"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "media"
			End Get
		End Property

	End Class

End Namespace
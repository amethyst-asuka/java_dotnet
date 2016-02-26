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
	''' Class DocumentName is a printing attribute class, a text attribute, that
	''' specifies the name of a document. DocumentName is an attribute of the print
	''' data (the doc), not of the Print Job. A document's name is an arbitrary
	''' string defined by the client.
	''' However if a JobName is not specified, the DocumentName should be used
	''' instead, which implies that supporting specification of DocumentName
	''' requires reporting of JobName and vice versa.
	''' See <seealso cref="JobName JobName"/> for more information.
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class DocumentName
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.DocAttribute

		Private Const serialVersionUID As Long = 7883105848533280430L

		''' <summary>
		''' Constructs a new document name attribute with the given document name
		''' and locale.
		''' </summary>
		''' <param name="documentName">  Document name. </param>
		''' <param name="locale">        Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''   (unchecked exception) Thrown if <CODE>documentName</CODE> is null. </exception>
		Public Sub New(ByVal documentName As String, ByVal locale As java.util.Locale)
			MyBase.New(documentName, locale)
		End Sub

		''' <summary>
		''' Returns whether this document name attribute is equivalent to the
		''' passed in object.
		''' To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class DocumentName.
		''' <LI>
		''' This document name attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This document name attribute's locale and <CODE>object</CODE>'s locale
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this document
		'''          name attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is DocumentName)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class DocumentName, the category is class DocumentName itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(DocumentName)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class DocumentName, the category name is <CODE>"document-name"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "document-name"
			End Get
		End Property

	End Class

End Namespace
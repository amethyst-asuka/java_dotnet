Imports System

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Class ReferenceUriSchemesSupported is a printing attribute class
	''' an enumeration, that indicates a "URI scheme," such as "http:" or "ftp:",
	''' that a printer can use to retrieve print data stored at a URI location.
	''' If a printer supports doc flavors with a print data representation class of
	''' <CODE>"java.net.URL"</CODE>, the printer uses instances of class
	''' ReferenceUriSchemesSupported to advertise the URI schemes it can accept.
	''' The acceptable URI schemes are included as service attributes in the
	''' lookup service; this lets clients search the
	''' for printers that can get print data using a certain URI scheme. The
	''' acceptable URI schemes can also be queried using the capability methods in
	''' interface <code>PrintService</code>. However,
	''' ReferenceUriSchemesSupported attributes are used solely for determining
	''' acceptable URI schemes, they are never included in a doc's,
	''' print request's, print job's, or print service's attribute set.
	''' <P>
	''' The Internet Assigned Numbers Authority maintains the
	''' <A HREF="http://www.iana.org/assignments/uri-schemes.html">official
	''' list of URI schemes</A>.
	''' <p>
	''' Class ReferenceUriSchemesSupported defines enumeration values for widely
	''' used URI schemes. A printer that supports additional URI schemes
	''' can define them in a subclass of class ReferenceUriSchemesSupported.
	''' <P>
	''' <B>IPP Compatibility:</B>  The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class ReferenceUriSchemesSupported
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.Attribute

		Private Const serialVersionUID As Long = -8989076942813442805L

		''' <summary>
		''' File Transfer Protocol (FTP).
		''' </summary>
		Public Shared ReadOnly FTP As New ReferenceUriSchemesSupported(0)

		''' <summary>
		''' HyperText Transfer Protocol (HTTP).
		''' </summary>
		Public Shared ReadOnly HTTP As New ReferenceUriSchemesSupported(1)

		''' <summary>
		''' Secure HyperText Transfer Protocol (HTTPS).
		''' </summary>
		Public Shared ReadOnly HTTPS As New ReferenceUriSchemesSupported(2)

		''' <summary>
		''' Gopher Protocol.
		''' </summary>
		Public Shared ReadOnly GOPHER As New ReferenceUriSchemesSupported(3)

		''' <summary>
		''' USENET news.
		''' </summary>
		Public Shared ReadOnly NEWS As New ReferenceUriSchemesSupported(4)

		''' <summary>
		''' USENET news using Network News Transfer Protocol (NNTP).
		''' </summary>
		Public Shared ReadOnly NNTP As New ReferenceUriSchemesSupported(5)

		''' <summary>
		''' Wide Area Information Server (WAIS) protocol.
		''' </summary>
		Public Shared ReadOnly WAIS As New ReferenceUriSchemesSupported(6)

		''' <summary>
		''' Host-specific file names.
		''' </summary>
		Public Shared ReadOnly FILE As New ReferenceUriSchemesSupported(7)

		''' <summary>
		''' Construct a new reference URI scheme enumeration value with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "ftp", "http", "https", "gopher", "news", "nntp", "wais", "file" }

		Private Shared ReadOnly myEnumValueTable As ReferenceUriSchemesSupported() = { FTP, HTTP, HTTPS, GOPHER, NEWS, NNTP, WAIS, FILE }

		''' <summary>
		''' Returns the string table for class ReferenceUriSchemesSupported.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return myStringTable.clone()
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class
		''' ReferenceUriSchemesSupported.
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
		''' For class ReferenceUriSchemesSupported and any vendor-defined
		''' subclasses, the category is class ReferenceUriSchemesSupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(ReferenceUriSchemesSupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class ReferenceUriSchemesSupported and any vendor-defined
		''' subclasses, the category name is
		''' <CODE>"reference-uri-schemes-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "reference-uri-schemes-supported"
			End Get
		End Property

	End Class

End Namespace
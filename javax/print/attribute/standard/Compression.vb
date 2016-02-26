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
	''' Class Compression is a printing attribute class, an enumeration, that
	''' specifies how print data is compressed. Compression is an attribute of the
	''' print data (the doc), not of the Print Job. If a Compression attribute is not
	''' specified for a doc, the printer assumes the doc's print data is uncompressed
	''' (i.e., the default Compression value is always {@link #NONE
	''' NONE}).
	''' <P>
	''' <B>IPP Compatibility:</B> The category name returned by
	''' <CODE>getName()</CODE> is the IPP attribute name.  The enumeration's
	''' integer value is the IPP enum value.  The <code>toString()</code> method
	''' returns the IPP string representation of the attribute value.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class Compression
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.DocAttribute

		Private Const serialVersionUID As Long = -5716748913324997674L

		''' <summary>
		''' No compression is used.
		''' </summary>
		Public Shared ReadOnly NONE As New Compression(0)

		''' <summary>
		''' ZIP public domain inflate/deflate compression technology.
		''' </summary>
		Public Shared ReadOnly DEFLATE As New Compression(1)

		''' <summary>
		''' GNU zip compression technology described in
		''' <A HREF="http://www.ietf.org/rfc/rfc1952.txt">RFC 1952</A>.
		''' </summary>
		Public Shared ReadOnly GZIP As New Compression(2)

		''' <summary>
		''' UNIX compression technology.
		''' </summary>
		Public Shared ReadOnly COMPRESS As New Compression(3)

		''' <summary>
		''' Construct a new compression enumeration value with the given integer
		''' value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub


		Private Shared ReadOnly myStringTable As String() = {"none", "deflate", "gzip", "compress"}

		Private Shared ReadOnly myEnumValueTable As Compression() = {NONE, DEFLATE, GZIP, COMPRESS}

		''' <summary>
		''' Returns the string table for class Compression.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class Compression.
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
		''' For class Compression and any vendor-defined subclasses, the category is
		''' class Compression itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Compression)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Compression and any vendor-defined subclasses, the category
		''' name is <CODE>"compression"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "compression"
			End Get
		End Property

	End Class

End Namespace
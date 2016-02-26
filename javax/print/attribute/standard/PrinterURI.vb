Imports System

'
' * Copyright (c) 2001, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Class PrinterURI is a printing attribute class, a URI, that specifies the
	''' globally unique name of a printer.  If it has such a name, an administrator
	''' determines a printer's URI and sets this attribute to that name.
	''' <P>
	''' <B>IPP Compatibility:</B>  This implements the
	''' IPP printer-uri attribute. The string form returned by
	''' <CODE>toString()</CODE>  gives the IPP printer-uri value.
	''' The category name returned by <CODE>getName()</CODE>
	''' gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Robert Herriot
	''' </summary>

	Public NotInheritable Class PrinterURI
		Inherits javax.print.attribute.URISyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = 7923912792485606497L

		''' <summary>
		''' Constructs a new PrinterURI attribute with the specified URI.
		''' </summary>
		''' <param name="uri">  URI of the printer
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>uri</CODE> is null. </exception>
		Public Sub New(ByVal uri As java.net.URI)
			MyBase.New(uri)
		End Sub

		''' <summary>
		''' Returns whether this printer name attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PrinterURI.
		''' <LI>
		''' This PrinterURI attribute's underlying URI and
		''' <CODE>object</CODE>'s underlying URI are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this PrinterURI
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PrinterURI)
		End Function

	   ''' <summary>
	   ''' Get the printing attribute class which is to be used as the "category"
	   ''' for this printing attribute value.
	   ''' <P>
	   ''' For class PrinterURI and any vendor-defined subclasses, the category is
	   ''' class PrinterURI itself.
	   ''' </summary>
	   ''' <returns>  Printing attribute class (category), an instance of class
	   '''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterURI)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterURI and any vendor-defined subclasses, the category
		''' name is <CODE>"printer-uri"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-uri"
			End Get
		End Property

	End Class

End Namespace
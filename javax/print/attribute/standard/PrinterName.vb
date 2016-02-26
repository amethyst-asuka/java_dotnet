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
	''' Class PrinterName is a printing attribute class, a text attribute, that
	''' specifies the name of a printer. It is a name that is more end-user friendly
	''' than a URI. An administrator determines a printer's name and sets this
	''' attribute to that name. This name may be the last part of the printer's URI
	''' or it may be unrelated. In non-US-English locales, a name may contain
	''' characters that are not allowed in a URI.
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterName
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = 299740639137803127L

		''' <summary>
		''' Constructs a new printer name attribute with the given name and locale.
		''' </summary>
		''' <param name="printerName">  Printer name. </param>
		''' <param name="locale">       Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>printerName</CODE> is null. </exception>
		Public Sub New(ByVal printerName As String, ByVal locale As java.util.Locale)
			MyBase.New(printerName, locale)
		End Sub

		''' <summary>
		''' Returns whether this printer name attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PrinterName.
		''' <LI>
		''' This printer name attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This printer name attribute's locale and <CODE>object</CODE>'s locale
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this printer
		'''          name attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PrinterName)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrinterName, the category is
		''' class PrinterName itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterName)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterName, the category
		''' name is <CODE>"printer-name"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-name"
			End Get
		End Property

	End Class

End Namespace
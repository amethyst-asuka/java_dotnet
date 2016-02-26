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
	''' Class PrinterInfo is a printing attribute class, a text attribute, that
	''' provides descriptive information about a printer. This could include things
	''' like: <CODE>"This printer can be used for printing color transparencies for
	''' HR presentations"</CODE>, or <CODE>"Out of courtesy for others, please
	''' print only small (1-5 page) jobs at this printer"</CODE>, or even \
	''' <CODE>"This printer is going away on July 1, 1997, please find a new
	''' printer"</CODE>.
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterInfo
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = 7765280618777599727L

		''' <summary>
		''' Constructs a new printer info attribute with the given information
		''' string and locale.
		''' </summary>
		''' <param name="info">    Printer information string. </param>
		''' <param name="locale">  Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>info</CODE> is null. </exception>
		Public Sub New(ByVal info As String, ByVal locale As java.util.Locale)
			MyBase.New(info, locale)
		End Sub

		''' <summary>
		''' Returns whether this printer info attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PrinterInfo.
		''' <LI>
		''' This printer info attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This printer info attribute's locale and <CODE>object</CODE>'s
		''' locale are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this printer
		'''          info attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PrinterInfo)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrinterInfo, the category is class PrinterInfo itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterInfo)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterInfo, the category name is <CODE>"printer-info"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-info"
			End Get
		End Property

	End Class

End Namespace
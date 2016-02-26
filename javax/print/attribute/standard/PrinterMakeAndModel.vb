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
	''' Class PrinterMakeAndModel is a printing attribute class, a text attribute,
	''' that the make and model of the printer.
	''' <P>
	''' <B>IPP Compatibility:</B> The string value gives the IPP name value. The
	''' locale gives the IPP natural language. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterMakeAndModel
		Inherits javax.print.attribute.TextSyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = 4580461489499351411L

		''' <summary>
		''' Constructs a new printer make and model attribute with the given make
		''' and model string and locale.
		''' </summary>
		''' <param name="makeAndModel">  Printer make and model string. </param>
		''' <param name="locale">        Natural language of the text string. null
		''' is interpreted to mean the default locale as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''    (unchecked exception) Thrown if <CODE>makeAndModel</CODE> is null. </exception>
		Public Sub New(ByVal makeAndModel As String, ByVal locale As java.util.Locale)
			MyBase.New(makeAndModel, locale)
		End Sub

		''' <summary>
		''' Returns whether this printer make and model attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PrinterMakeAndModel.
		''' <LI>
		''' This printer make and model attribute's underlying string and
		''' <CODE>object</CODE>'s underlying string are equal.
		''' <LI>
		''' This printer make and model attribute's locale and
		''' <CODE>object</CODE>'s locale are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this printer
		'''          make and model attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PrinterMakeAndModel)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrinterMakeAndModel, the
		''' category is class PrinterMakeAndModel itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterMakeAndModel)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterMakeAndModel, the
		''' category name is <CODE>"printer-make-and-model"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-make-and-model"
			End Get
		End Property

	End Class

End Namespace
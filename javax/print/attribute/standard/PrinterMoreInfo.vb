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
	''' Class PrinterMoreInfo is a printing attribute class, a URI, that is used to
	''' obtain more information about this specific printer. For example, this
	''' could be an HTTP type URI referencing an HTML page accessible to a web
	''' browser. The information obtained from this URI is intended for end user
	''' consumption. Features outside the scope of the Print Service API can be
	''' accessed from this URI.
	''' The information is intended to be specific to this printer instance and
	''' site specific services (e.g. job pricing, services offered, end user
	''' assistance).
	''' <P>
	''' In contrast, the {@link PrinterMoreInfoManufacturer
	''' PrinterMoreInfoManufacturer} attribute is used to find out more information
	''' about this general kind of printer rather than this specific printer.
	''' <P>
	''' <B>IPP Compatibility:</B> The string form returned by
	''' <CODE>toString()</CODE>  gives the IPP uri value.
	''' The category name returned by <CODE>getName()</CODE>
	''' gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class PrinterMoreInfo
		Inherits javax.print.attribute.URISyntax
		Implements javax.print.attribute.PrintServiceAttribute

		Private Const serialVersionUID As Long = 4555850007675338574L

		''' <summary>
		''' Constructs a new printer more info attribute with the specified URI.
		''' </summary>
		''' <param name="uri">  URI.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>uri</CODE> is null. </exception>
		Public Sub New(ByVal uri As java.net.URI)
			MyBase.New(uri)
		End Sub

		''' <summary>
		''' Returns whether this printer more info attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class PrinterMoreInfo.
		''' <LI>
		''' This printer more info attribute's URI and <CODE>object</CODE>'s URI
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this printer
		'''          more info attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is PrinterMoreInfo)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrinterMoreInfo, the category is class PrinterMoreInfo itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterMoreInfo)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterMoreInfo, the
		''' category name is <CODE>"printer-more-info"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-more-info"
			End Get
		End Property

	End Class

End Namespace
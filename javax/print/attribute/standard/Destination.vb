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
	''' Class Destination is a printing attribute class, a URI, that is used to
	''' indicate an alternate destination for the spooled printer formatted
	''' data. Many PrintServices will not support the notion of a destination
	''' other than the printer device, and so will not support this attribute.
	''' <p>
	''' A common use for this attribute will be applications which want
	''' to redirect output to a local disk file : eg."file:out.prn".
	''' Note that proper construction of "file:" scheme URI instances should
	''' be performed using the <code>toURI()</code> method of class
	''' <seealso cref="java.io.File File"/>.
	''' See the documentation on that class for more information.
	''' <p>
	''' If a destination URI is specified in a PrintRequest and it is not
	''' accessible for output by the PrintService, a PrintException will be thrown.
	''' The PrintException may implement URIException to provide a more specific
	''' cause.
	''' <P>
	''' <B>IPP Compatibility:</B> Destination is not an IPP attribute.
	''' <P>
	''' 
	''' @author  Phil Race.
	''' </summary>
	Public NotInheritable Class Destination
		Inherits javax.print.attribute.URISyntax
		Implements javax.print.attribute.PrintJobAttribute, javax.print.attribute.PrintRequestAttribute

		Private Const serialVersionUID As Long = 6776739171700415321L

		''' <summary>
		''' Constructs a new destination attribute with the specified URI.
		''' </summary>
		''' <param name="uri">  URI.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>uri</CODE> is null. </exception>
		Public Sub New(ByVal uri As java.net.URI)
			MyBase.New(uri)
		End Sub

		''' <summary>
		''' Returns whether this destination attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class Destination.
		''' <LI>
		''' This destination attribute's URI and <CODE>object</CODE>'s URI
		''' are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this destination
		'''         attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is Destination)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class Destination, the category is class Destination itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(Destination)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class Destination, the category name is <CODE>"spool-data-destination"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "spool-data-destination"
			End Get
		End Property

	End Class

End Namespace
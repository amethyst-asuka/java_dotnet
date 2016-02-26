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

Namespace javax.print.attribute


	''' <summary>
	''' Interface Attribute is the base interface implemented by any and every
	''' printing attribute class to indicate that the class represents a
	''' printing attribute. All printing attributes are serializable.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	Public Interface Attribute
		Inherits java.io.Serializable

	  ''' <summary>
	  ''' Get the printing attribute class which is to be used as the "category"
	  ''' for this printing attribute value when it is added to an attribute set.
	  ''' </summary>
	  ''' <returns>  Printing attribute class (category), an instance of class
	  '''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
	  ReadOnly Property category As Type

	  ''' <summary>
	  ''' Get the name of the category of which this attribute value is an
	  ''' instance.
	  ''' <P>
	  ''' <I>Note:</I> This method is intended to provide a default, nonlocalized
	  ''' string for the attribute's category. If two attribute objects return the
	  ''' same category from the <CODE>getCategory()</CODE> method, they should
	  ''' return the same name from the <CODE>getName()</CODE> method.
	  ''' </summary>
	  ''' <returns>  Attribute category name. </returns>
	  ReadOnly Property name As String

	End Interface

End Namespace
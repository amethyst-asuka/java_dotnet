Imports System

'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Class HashPrintJobAttributeSet provides an attribute set
	''' which inherits its implementation from class {@link HashAttributeSet
	''' HashAttributeSet} and enforces the semantic restrictions of interface
	''' <seealso cref="PrintJobAttributeSet PrintJobAttributeSet"/>.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public Class HashPrintJobAttributeSet
		Inherits HashAttributeSet
		Implements PrintJobAttributeSet

		Private Const serialVersionUID As Long = -4204473656070350348L

		''' <summary>
		''' Construct a new, empty hash print job attribute set.
		''' </summary>
		Public Sub New()
			MyBase.New(GetType(PrintJobAttribute))
		End Sub

		''' <summary>
		''' Construct a new hash print job attribute set,
		''' initially populated with the given value.
		''' </summary>
		''' <param name="attribute">  Attribute value to add to the set.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is null. </exception>
		Public Sub New(ByVal attribute As PrintJobAttribute)
			MyBase.New(attribute, GetType(PrintJobAttribute))
		End Sub

		''' <summary>
		''' Construct a new hash print job attribute set,
		''' initially populated with the values from the given array.
		''' The new attribute set is populated
		''' by adding the elements of <CODE>attributes</CODE> array to the set in
		''' sequence, starting at index 0. Thus, later array elements may replace
		''' earlier array elements if the array contains duplicate attribute
		''' values or attribute categories.
		''' </summary>
		''' <param name="attributes"> Array of attribute values to add to the set.
		'''                    If null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="NullPointerException"> (unchecked exception)
		''' Thrown if any element of <CODE>attributes</CODE>  is null. </exception>
		Public Sub New(ByVal attributes As PrintJobAttribute())
			MyBase.New(attributes, GetType(PrintJobAttribute))
		End Sub

		''' <summary>
		''' Construct a new attribute set, initially populated with the
		''' values from the  given set where the members of the attribute set
		''' are restricted to the <code>PrintJobAttribute</code> interface.
		''' </summary>
		''' <param name="attributes"> set of attribute values to initialise the set. If
		'''                    null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is not an instance of
		''' <CODE>PrintJobAttribute</CODE>. </exception>
		Public Sub New(ByVal attributes As PrintJobAttributeSet)
			MyBase.New(attributes, GetType(PrintJobAttribute))
		End Sub
	End Class

End Namespace
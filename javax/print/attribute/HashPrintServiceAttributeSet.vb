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
	''' Class HashPrintServiceAttributeSet provides an attribute set
	''' which inherits its implementation from class {@link HashAttributeSet
	''' HashAttributeSet} and enforces the semantic restrictions of interface
	''' <seealso cref="PrintServiceAttributeSet PrintServiceAttributeSet"/>.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public Class HashPrintServiceAttributeSet
		Inherits HashAttributeSet
		Implements PrintServiceAttributeSet

		Private Const serialVersionUID As Long = 6642904616179203070L

		''' <summary>
		''' Construct a new, empty hash print service attribute set.
		''' </summary>
		Public Sub New()
			MyBase.New(GetType(PrintServiceAttribute))
		End Sub


		''' <summary>
		''' Construct a new hash print service attribute set,
		'''  initially populated with the given value.
		''' </summary>
		''' <param name="attribute">  Attribute value to add to the set.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is null. </exception>
		Public Sub New(ByVal attribute As PrintServiceAttribute)
			MyBase.New(attribute, GetType(PrintServiceAttribute))
		End Sub

		''' <summary>
		''' Construct a new print service attribute set, initially populated with
		''' the values from the given array. The new attribute set is populated
		''' by adding the elements of <CODE>attributes</CODE> array to the set in
		''' sequence, starting at index 0. Thus, later array elements may replace
		''' earlier array elements if the array contains duplicate attribute
		''' values or attribute categories.
		''' </summary>
		''' <param name="attributes">  Array of attribute values to add to the set.
		'''                    If null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception)
		'''      Thrown if any element of <CODE>attributes</CODE> is null. </exception>
		Public Sub New(ByVal attributes As PrintServiceAttribute())
			MyBase.New(attributes, GetType(PrintServiceAttribute))
		End Sub


		''' <summary>
		''' Construct a new attribute set, initially populated with the
		''' values from the  given set where the members of the attribute set
		''' are restricted to the <code>PrintServiceAttribute</code> interface.
		''' </summary>
		''' <param name="attributes"> set of attribute values to initialise the set. If
		'''                    null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is not an instance of
		''' <CODE>PrintServiceAttribute</CODE>. </exception>
		Public Sub New(ByVal attributes As PrintServiceAttributeSet)
			MyBase.New(attributes, GetType(PrintServiceAttribute))
		End Sub
	End Class

End Namespace
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
	''' Class HashPrintRequestAttributeSet inherits its implementation from
	''' class <seealso cref="HashAttributeSet HashAttributeSet"/> and enforces the
	''' semantic restrictions of interface
	''' <seealso cref="PrintRequestAttributeSet PrintRequestAttributeSet"/>.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public Class HashPrintRequestAttributeSet
		Inherits HashAttributeSet
		Implements PrintRequestAttributeSet

		Private Const serialVersionUID As Long = 2364756266107751933L

		''' <summary>
		''' Construct a new, empty print request attribute set.
		''' </summary>
		Public Sub New()
			MyBase.New(GetType(PrintRequestAttribute))
		End Sub

		''' <summary>
		''' Construct a new print request attribute set,
		''' initially populated with the given value.
		''' </summary>
		''' <param name="attribute">  Attribute value to add to the set.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is null. </exception>
		Public Sub New(ByVal attribute As PrintRequestAttribute)
			MyBase.New(attribute, GetType(PrintRequestAttribute))
		End Sub

		''' <summary>
		''' Construct a new print request attribute set, initially populated with
		''' the values from the given array. The new attribute set is populated
		''' by adding the elements of <CODE>attributes</CODE> array to the set in
		''' sequence, starting at index 0. Thus, later array elements may replace
		''' earlier array elements if the array contains duplicate attribute
		''' values or attribute categories.
		''' </summary>
		''' <param name="attributes">  Array of attribute values to add to the set.
		'''                     If null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception)
		''' Thrown if any element of <CODE>attributes</CODE> is null. </exception>
		Public Sub New(ByVal attributes As PrintRequestAttribute())
			MyBase.New(attributes, GetType(PrintRequestAttribute))
		End Sub


		''' <summary>
		''' Construct a new attribute set, initially populated with the
		''' values from the  given set where the members of the attribute set
		''' are restricted to the <code>(PrintRequestAttributeSe</code> interface.
		''' </summary>
		''' <param name="attributes"> set of attribute values to initialise the set. If
		'''                    null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is not an instance of
		''' <CODE>(PrintRequestAttributeSe</CODE>. </exception>
		Public Sub New(ByVal attributes As PrintRequestAttributeSet)
			MyBase.New(attributes, GetType(PrintRequestAttribute))
		End Sub


	End Class

End Namespace
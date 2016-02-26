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
	''' Class HashDocAttributeSet provides an attribute set which
	''' inherits its implementation from class {@link HashAttributeSet
	''' HashAttributeSet} and enforces the semantic restrictions of interface {@link
	''' DocAttributeSet DocAttributeSet}.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public Class HashDocAttributeSet
		Inherits HashAttributeSet
		Implements DocAttributeSet

		Private Const serialVersionUID As Long = -1128534486061432528L

		''' <summary>
		''' Construct a new, empty hash doc attribute set.
		''' </summary>
		Public Sub New()
			MyBase.New(GetType(DocAttribute))
		End Sub

		''' <summary>
		''' Construct a new hash doc attribute set,
		''' initially populated with the given value.
		''' </summary>
		''' <param name="attribute">  Attribute value to add to the set.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>attribute</CODE> is null. </exception>
		Public Sub New(ByVal attribute As DocAttribute)
			MyBase.New(attribute, GetType(DocAttribute))
		End Sub

		''' <summary>
		''' Construct a new hash doc attribute set,
		''' initially populated with the values from the given array.
		''' The new attribute set is populated by
		''' adding the elements of <CODE>attributes</CODE> array to the set in
		''' sequence, starting at index 0. Thus, later array elements may replace
		''' earlier array elements if the array contains duplicate attribute
		''' values or attribute categories.
		''' </summary>
		''' <param name="attributes">  Array of attribute values to add to the set.
		'''                     If null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="NullPointerException">
		'''  (unchecked exception)
		''' Thrown if any element of <CODE>attributes</CODE> is null. </exception>
		Public Sub New(ByVal attributes As DocAttribute())
			MyBase.New(attributes, GetType(DocAttribute))
		End Sub

		''' <summary>
		''' Construct a new attribute set, initially populated with the
		''' values from the  given set where the members of the attribute set
		''' are restricted to the <code>DocAttribute</code> interface.
		''' </summary>
		''' <param name="attributes"> set of attribute values to initialise the set. If
		'''                    null, an empty attribute set is constructed.
		''' </param>
		''' <exception cref="ClassCastException">
		'''     (unchecked exception) Thrown if any element of
		''' <CODE>attributes</CODE> is not an instance of
		''' <CODE>DocAttribute</CODE>. </exception>
		Public Sub New(ByVal attributes As DocAttributeSet)
			MyBase.New(attributes, GetType(DocAttribute))
		End Sub

	End Class

End Namespace
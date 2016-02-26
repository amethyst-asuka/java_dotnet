Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' A generic interface for a mutable collection of unique attributes.
	''' 
	''' Implementations will probably want to provide a constructor of the
	''' form:<tt>
	''' public XXXAttributeSet(ConstAttributeSet source);</tt>
	''' 
	''' </summary>
	Public Interface MutableAttributeSet
		Inherits AttributeSet

		''' <summary>
		''' Creates a new attribute set similar to this one except that it contains
		''' an attribute with the given name and value.  The object must be
		''' immutable, or not mutated by any client.
		''' </summary>
		''' <param name="name"> the name </param>
		''' <param name="value"> the value </param>
		Sub addAttribute(ByVal name As Object, ByVal value As Object)

		''' <summary>
		''' Creates a new attribute set similar to this one except that it contains
		''' the given attributes and values.
		''' </summary>
		''' <param name="attributes"> the set of attributes </param>
		Sub addAttributes(ByVal attributes As AttributeSet)

		''' <summary>
		''' Removes an attribute with the given <code>name</code>.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		Sub removeAttribute(ByVal name As Object)

		''' <summary>
		''' Removes an attribute set with the given <code>names</code>.
		''' </summary>
		''' <param name="names"> the set of names </param>
		Sub removeAttributes(Of T1)(ByVal names As System.Collections.IEnumerator(Of T1))

		''' <summary>
		''' Removes a set of attributes with the given <code>name</code>.
		''' </summary>
		''' <param name="attributes"> the set of attributes </param>
		Sub removeAttributes(ByVal attributes As AttributeSet)

		''' <summary>
		''' Sets the resolving parent.  This is the set
		''' of attributes to resolve through if an attribute
		''' isn't defined locally.
		''' </summary>
		''' <param name="parent"> the parent </param>
		WriteOnly Property resolveParent As AttributeSet

	End Interface

End Namespace
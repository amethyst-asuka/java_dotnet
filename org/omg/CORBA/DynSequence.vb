Imports System

'
' * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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


Namespace org.omg.CORBA

	''' <summary>
	''' The representation of a <code>DynAny</code> object that is associated
	''' with an IDL sequence. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynSequence.html">DynSequence</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynSequence.html">DynSequence</a> instead")> _
	Public Interface DynSequence
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' Returns the length of the sequence represented by this
		''' <code>DynFixed</code> object.
		''' </summary>
		''' <returns> the length of the sequence </returns>
		Function length() As Integer

		''' <summary>
		''' Sets the length of the sequence represented by this
		''' <code>DynFixed</code> object to the given argument.
		''' </summary>
		''' <param name="arg"> the length of the sequence </param>
		Sub length(ByVal arg As Integer)

		''' <summary>
		''' Returns the value of every element in this sequence.
		''' </summary>
		''' <returns> an array of <code>Any</code> objects containing the values in
		'''         the sequence </returns>
		''' <seealso cref= #set_elements </seealso>
		Function get_elements() As org.omg.CORBA.Any()

		''' <summary>
		''' Sets the values of all elements in this sequence with the given
		''' array.
		''' </summary>
		''' <param name="value"> the array of <code>Any</code> objects to be set </param>
		''' <exception cref="InvalidSeq"> if the array of values is bad </exception>
		''' <seealso cref= #get_elements </seealso>
		Sub set_elements(ByVal value As org.omg.CORBA.Any())
	End Interface

End Namespace
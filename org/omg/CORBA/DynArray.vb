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
	''' Represents a <tt>DynAny</tt> object associated
	'''  with an array. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynArray.html">DynArray</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynArray.html">DynArray</a> instead")> _
	Public Interface DynArray
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' Returns the value of all the elements of this array.
		''' </summary>
		''' <returns> the array of <code>Any</code> objects that is the value
		'''         for this <code>DynArray</code> object </returns>
		''' <seealso cref= #set_elements </seealso>
		Function get_elements() As org.omg.CORBA.Any()

		''' <summary>
		''' Sets the value of this
		''' <code>DynArray</code> object to the given array.
		''' </summary>
		''' <param name="value"> the array of <code>Any</code> objects </param>
		''' <exception cref="InvalidSeq"> if the sequence is bad </exception>
		''' <seealso cref= #get_elements </seealso>
		Sub set_elements(ByVal value As org.omg.CORBA.Any())
	End Interface

End Namespace
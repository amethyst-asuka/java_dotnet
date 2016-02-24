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
	'''  with an IDL value type. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynValue.html">DynValue</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynValue.html">DynValue</a> instead")> _
	Public Interface DynValue
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' Returns the name of the current member while traversing a
		''' <code>DynAny</code> object that represents a Value object.
		''' </summary>
		''' <returns> the name of the current member </returns>
		Function current_member_name() As String

		''' <summary>
		''' Returns the <code>TCKind</code> object that describes the current member.
		''' </summary>
		''' <returns> the <code>TCKind</code> object corresponding to the current
		''' member </returns>
		Function current_member_kind() As TCKind

		''' <summary>
		''' Returns an array containing all the members of the value object
		''' stored in this <code>DynValue</code>.
		''' </summary>
		''' <returns> an array of name-value pairs. </returns>
		''' <seealso cref= #set_members </seealso>
		Function get_members() As org.omg.CORBA.NameValuePair()

		''' <summary>
		''' Sets the members of the value object this <code>DynValue</code>
		''' object represents to the given array of <code>NameValuePair</code>
		''' objects.
		''' </summary>
		''' <param name="value"> the array of name-value pairs to be set </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidSeq">
		'''         if an inconsistent value is part of the given array </exception>
		''' <seealso cref= #get_members </seealso>
		Sub set_members(ByVal value As NameValuePair())
	End Interface

End Namespace
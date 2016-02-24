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
	'''  with an IDL struct. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynStruct.html">DynStruct</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynStruct.html">DynStruct</a> instead")> _
	Public Interface DynStruct
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' During a traversal, returns the name of the current member.
		''' </summary>
		''' <returns> the string name of the current member </returns>
		Function current_member_name() As String

		''' <summary>
		''' Returns the <code>TCKind</code> object that describes the kind of
		''' the current member.
		''' </summary>
		''' <returns> the <code>TCKind</code> object that describes the current member </returns>
		Function current_member_kind() As org.omg.CORBA.TCKind

		''' <summary>
		''' Returns an array containing all the members of the stored struct.
		''' </summary>
		''' <returns> the array of name-value pairs </returns>
		''' <seealso cref= #set_members </seealso>
		Function get_members() As org.omg.CORBA.NameValuePair()

		''' <summary>
		''' Set the members of the struct.
		''' </summary>
		''' <param name="value"> the array of name-value pairs. </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidSeq"> if the given argument
		'''         is invalid </exception>
		''' <seealso cref= #get_members </seealso>
		Sub set_members(ByVal value As org.omg.CORBA.NameValuePair())
	End Interface

End Namespace
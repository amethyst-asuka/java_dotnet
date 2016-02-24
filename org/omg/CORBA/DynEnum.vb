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
	''' Represents a <tt>DynAny</tt> object  associated
	'''  with an IDL enum. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynEnum.html">DynEnum</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynEnum.html">DynEnum</a> instead")> _
	Public Interface DynEnum
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' Return the value of the IDL enum stored in this
		''' <code>DynEnum</code> as a string.
		''' </summary>
		''' <returns> the stringified value. </returns>
		Function value_as_string() As String

		''' <summary>
		''' Set a particular enum in this <code>DynEnum</code>.
		''' </summary>
		''' <param name="arg"> the string corresponding to the value. </param>
		Sub value_as_string(ByVal arg As String)

		''' <summary>
		''' Return the value of the IDL enum as a Java int.
		''' </summary>
		''' <returns> the integer value. </returns>
		Function value_as_ulong() As Integer

		''' <summary>
		''' Set the value of the IDL enum.
		''' </summary>
		''' <param name="arg"> the int value of the enum. </param>
		Sub value_as_ulong(ByVal arg As Integer)
	End Interface

End Namespace
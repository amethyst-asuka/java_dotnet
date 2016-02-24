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
	''' The <code>DynUnion</code> interface represents a <code>DynAny</code> object
	''' that is associated with an IDL union.
	''' Union values can be traversed using the operations defined in <code>DynAny</code>.
	''' The first component in the union corresponds to the discriminator;
	''' the second corresponds to the actual value of the union.
	''' Calling the method <code>next()</code> twice allows you to access both components. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynUnion.html">DynUnion</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynUnion.html">DynUnion</a> instead")> _
	Public Interface DynUnion
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' Determines whether the discriminator associated with this union has been assigned
		''' a valid default value. </summary>
		''' <returns> <code>true</code> if the discriminator has a default value;
		''' <code>false</code> otherwise </returns>
		Function set_as_default() As Boolean

		''' <summary>
		''' Determines whether the discriminator associated with this union gets assigned
		''' a valid default value. </summary>
		''' <param name="arg"> <code>true</code> if the discriminator gets assigned a default value </param>
		Sub set_as_default(ByVal arg As Boolean)

		''' <summary>
		''' Returns a DynAny object reference that must be narrowed to the type
		''' of the discriminator in order to insert/get the discriminator value. </summary>
		''' <returns> a <code>DynAny</code> object reference representing the discriminator value </returns>
		Function discriminator() As org.omg.CORBA.DynAny

		''' <summary>
		''' Returns the TCKind object associated with the discriminator of this union. </summary>
		''' <returns> the <code>TCKind</code> object associated with the discriminator of this union </returns>
		Function discriminator_kind() As org.omg.CORBA.TCKind

		''' <summary>
		''' Returns a DynAny object reference that is used in order to insert/get
		''' a member of this union. </summary>
		''' <returns> the <code>DynAny</code> object representing a member of this union </returns>
		Function member() As org.omg.CORBA.DynAny

		''' <summary>
		''' Allows for the inspection of the name of this union member
		''' without checking the value of the discriminator. </summary>
		''' <returns> the name of this union member </returns>
		Function member_name() As String

		''' <summary>
		''' Allows for the assignment of the name of this union member. </summary>
		''' <param name="arg"> the new name of this union member </param>
		Sub member_name(ByVal arg As String)

		''' <summary>
		''' Returns the TCKind associated with the member of this union. </summary>
		''' <returns> the <code>TCKind</code> object associated with the member of this union </returns>
		Function member_kind() As org.omg.CORBA.TCKind
	End Interface

End Namespace
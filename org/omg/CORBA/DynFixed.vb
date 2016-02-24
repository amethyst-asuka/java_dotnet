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
	'''  Represents a <code>DynAny</code> object that is associated
	'''  with an IDL fixed type. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynFixed.html">DynFixed</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynFixed.html">DynFixed</a> instead")> _
	Public Interface DynFixed
		Inherits org.omg.CORBA.Object, org.omg.CORBA.DynAny

		''' <summary>
		''' Returns the value of the fixed type represented in this
		''' <code>DynFixed</code> object.
		''' </summary>
		''' <returns> the value as a byte array </returns>
		''' <seealso cref= #set_value </seealso>
		Function get_value() As SByte()

		''' <summary>
		''' Sets the given fixed type instance as the value for this
		''' <code>DynFixed</code> object.
		''' </summary>
		''' <param name="val"> the value of the fixed type as a byte array </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue"> if the given
		'''         argument is bad </exception>
		''' <seealso cref= #get_value </seealso>
		Sub set_value(ByVal val As SByte())
	End Interface

End Namespace
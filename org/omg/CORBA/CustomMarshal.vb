'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' An abstract value type that is meant to
	''' be used by the ORB, not the user. Semantically it is treated
	''' as a custom value type's implicit base class, although the custom
	''' valuetype does not actually inherit it in IDL. The implementer
	''' of a custom value type shall provide an implementation of the
	''' <tt>CustomMarshal</tt> operations. The manner in which this is done is
	''' specified in the IDL to Java langauge mapping. Each custom
	''' marshaled value type shall have its own implementation. </summary>
	''' <seealso cref= DataInputStream </seealso>
	Public Interface CustomMarshal
		''' <summary>
		''' Marshal method has to be implemented by the Customized Marshal class.
		''' This is the method invoked for Marshalling.
		''' </summary>
		''' <param name="os"> a DataOutputStream </param>
		Sub marshal(ByVal os As org.omg.CORBA.DataOutputStream)
		''' <summary>
		''' Unmarshal method has to be implemented by the Customized Marshal class.
		''' This is the method invoked for Unmarshalling.
		''' </summary>
		''' <param name="is"> a DataInputStream </param>
		Sub unmarshal(ByVal [is] As org.omg.CORBA.DataInputStream)
	End Interface

End Namespace
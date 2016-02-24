'
' * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace org.omg.CORBA_2_3

	''' <summary>
	''' A class extending <code>org.omg.CORBA.ORB</code> to make the ORB
	''' portable under the OMG CORBA version 2.3 specification.
	''' </summary>
	Public MustInherit Class ORB
		Inherits org.omg.CORBA.ORB

	''' 
		Public Overridable Function register_value_factory(ByVal id As String, ByVal factory As org.omg.CORBA.portable.ValueFactory) As org.omg.CORBA.portable.ValueFactory
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


	''' 
		Public Overridable Sub unregister_value_factory(ByVal id As String)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub


	''' 
		Public Overridable Function lookup_value_factory(ByVal id As String) As org.omg.CORBA.portable.ValueFactory
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


	''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA_2_3</code> package
	'''      comments for unimplemented features</a> </seealso>
		' always return a ValueDef or throw BAD_PARAM if
		 ' <em>repid</em> does not represent a valuetype
		 Public Overridable Function get_value_def(ByVal repid As String) As org.omg.CORBA.Object
		   Throw New org.omg.CORBA.NO_IMPLEMENT
		 End Function


	''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA_2_3</code> package
	'''      comments for unimplemented features</a> </seealso>
		 Public Overridable Sub set_delegate(ByVal wrapper As Object)
		   Throw New org.omg.CORBA.NO_IMPLEMENT
		 End Sub


	End Class

End Namespace
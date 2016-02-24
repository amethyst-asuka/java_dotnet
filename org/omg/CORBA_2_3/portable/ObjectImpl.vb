'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA_2_3.portable


	''' <summary>
	''' ObjectImpl class is the base class for all stubs.  It provides the
	''' basic delegation mechanism.  It extends org.omg.CORBA.portable.ObjectImpl
	''' and provides new methods defined by CORBA 2.3.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.portable.ObjectImpl
	''' @author  OMG
	''' @since   JDK1.2 </seealso>


	Public MustInherit Class ObjectImpl
		Inherits org.omg.CORBA.portable.ObjectImpl

		''' <summary>
		''' Returns the codebase for this object reference. </summary>
		''' <returns> the codebase as a space delimited list of url strings or
		''' null if none. </returns>
		Public Overridable Function _get_codebase() As String
			Dim [delegate] As org.omg.CORBA.portable.Delegate = _get_delegate()
			If TypeOf [delegate] Is org.omg.CORBA_2_3.portable.Delegate Then Return CType([delegate], org.omg.CORBA_2_3.portable.Delegate).get_codebase(Me)
			Return Nothing
		End Function
	End Class

End Namespace
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
	''' Delegate class provides the ORB vendor specific implementation
	''' of CORBA object.  It extends org.omg.CORBA.portable.Delegate and
	''' provides new methods that were defined by CORBA 2.3.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.portable.Delegate
	''' @author  OMG
	''' @since   JDK1.2 </seealso>

	Public MustInherit Class [Delegate]
		Inherits org.omg.CORBA.portable.Delegate

		''' <summary>
		''' Returns the codebase for object reference provided. </summary>
		''' <param name="self"> the object reference whose codebase needs to be returned. </param>
		''' <returns> the codebase as a space delimited list of url strings or
		''' null if none. </returns>
		Public Overridable Function get_codebase(ByVal self As org.omg.CORBA.Object) As String
			Return Nothing
		End Function
	End Class

End Namespace
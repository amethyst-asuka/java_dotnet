Imports System

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' A class that contains information about the identity of
	''' the client, for access control
	''' and other purposes. It contains a single attribute, the name of the
	''' <code>Principal</code>, encoded as a sequence of bytes.
	''' <P> </summary>
	''' @deprecated Deprecated by CORBA 2.2. 
	<Obsolete("Deprecated by CORBA 2.2.")> _
	Public Class Principal
		''' <summary>
		''' Sets the name of this <code>Principal</code> object to the given value. </summary>
		''' <param name="value"> the value to be set in the <code>Principal</code> </param>
		''' @deprecated Deprecated by CORBA 2.2. 
		<Obsolete("Deprecated by CORBA 2.2.")> _
		Public Overridable Sub name(ByVal value As SByte())
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Gets the name of this <code>Principal</code> object. </summary>
		''' <returns> the name of this <code>Principal</code> object </returns>
		''' @deprecated Deprecated by CORBA 2.2. 
		<Obsolete("Deprecated by CORBA 2.2.")> _
		Public Overridable Function name() As SByte()
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	End Class

End Namespace
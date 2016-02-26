Imports System

'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.rmi.CORBA

	''' <summary>
	''' Defines methods which allow serialization of Java objects
	''' to and from GIOP streams.
	''' 
	''' </summary>
	Public Interface ValueHandler

		''' <summary>
		''' Writes a value to the stream using Java semantics. </summary>
		''' <param name="out"> the stream to write the value to. </param>
		''' <param name="value"> the value to be written to the stream.
		'''  </param>
		Sub writeValue(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal value As java.io.Serializable)

		''' <summary>
		''' Reads a value from the stream using Java semantics. </summary>
		''' <param name="in"> the stream to read the value from. </param>
		''' <param name="offset"> the current position in the input stream. </param>
		''' <param name="clz"> the type of the value to be read in. </param>
		''' <param name="repositoryID"> the RepositoryId of the value to be read in. </param>
		''' <param name="sender"> the sending context runtime codebase. </param>
		''' <returns> the value read from the stream.
		'''  </returns>
		Function readValue(ByVal [in] As org.omg.CORBA.portable.InputStream, ByVal offset As Integer, ByVal clz As Type, ByVal repositoryID As String, ByVal sender As org.omg.SendingContext.RunTime) As java.io.Serializable

		''' <summary>
		''' Returns the CORBA RepositoryId for the given Java class. </summary>
		''' <param name="clz"> a Java class. </param>
		''' <returns> the CORBA RepositoryId for the class.
		'''  </returns>
		Function getRMIRepositoryID(ByVal clz As Type) As String

		''' <summary>
		''' Indicates whether the given class performs custom or
		''' default marshaling. </summary>
		''' <param name="clz"> the class to test for custom marshaling. </param>
		''' <returns> <code>true</code> if the class performs custom marshaling, <code>false</code>
		''' if it does not.
		'''  </returns>
		Function isCustomMarshaled(ByVal clz As Type) As Boolean

		''' <summary>
		''' Returns the CodeBase for this ValueHandler.  This is used by
		''' the ORB runtime.  The server sends the service context containing
		''' the IOR for this CodeBase on the first GIOP reply.  The client
		''' does the same on the first GIOP request. </summary>
		''' <returns> the SendingContext.CodeBase of this ValueHandler.
		'''  </returns>
		ReadOnly Property runTimeCodeBase As org.omg.SendingContext.RunTime

		''' <summary>
		''' If the value contains a <code>writeReplace</code> method then the result
		''' is returned.  Otherwise, the value itself is returned. </summary>
		''' <param name="value"> the value to be marshaled. </param>
		''' <returns> the true value to marshal on the wire.
		'''  </returns>
		Function writeReplace(ByVal value As java.io.Serializable) As java.io.Serializable

	End Interface

End Namespace
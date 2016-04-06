'
' * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util


	''' <summary>
	''' Thrown to indicate that an operation could not complete because
	''' the input did not conform to the appropriate XML document type
	''' for a collection of properties, as per the <seealso cref="Properties"/>
	''' specification.<p>
	''' 
	''' Note, that although InvalidPropertiesFormatException inherits Serializable
	''' interface from Exception, it is not intended to be Serializable. Appropriate
	''' serialization methods are implemented to throw NotSerializableException.
	''' </summary>
	''' <seealso cref=     Properties
	''' @since   1.5
	''' @serial exclude </seealso>

	Public Class InvalidPropertiesFormatException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = 7763056076009360219L

		''' <summary>
		''' Constructs an InvalidPropertiesFormatException with the specified
		''' cause.
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="Throwable#getCause()"/> method). </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(If(cause Is Nothing, Nothing, cause.ToString()))
			Me.initCause(cause)
		End Sub

	   ''' <summary>
	   ''' Constructs an InvalidPropertiesFormatException with the specified
	   ''' detail message.
	   ''' </summary>
	   ''' <param name="message">   the detail message. The detail message is saved for
	   '''          later retrieval by the <seealso cref="Throwable#getMessage()"/> method. </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Throws NotSerializableException, since InvalidPropertiesFormatException
		''' objects are not intended to be serializable.
		''' </summary>
		Private Sub writeObject(  out As java.io.ObjectOutputStream)
			Throw New java.io.NotSerializableException("Not serializable.")
		End Sub

		''' <summary>
		''' Throws NotSerializableException, since InvalidPropertiesFormatException
		''' objects are not intended to be serializable.
		''' </summary>
		Private Sub readObject(  [in] As java.io.ObjectInputStream)
			Throw New java.io.NotSerializableException("Not serializable.")
		End Sub

	End Class

End Namespace
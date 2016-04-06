'
' * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Thrown to indicate that the Java Virtual Machine is broken or has
	''' run out of resources necessary for it to continue operating.
	''' 
	''' 
	''' @author  Frank Yellin
	''' @since   JDK1.0
	''' </summary>
	Public MustInherit Class VirtualMachineError
		Inherits [Error]

		Private Shadows Const serialVersionUID As Long = 4161983926571568670L

		''' <summary>
		''' Constructs a <code>VirtualMachineError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>VirtualMachineError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="message">   the detail message. </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a {@code VirtualMachineError} with the specified
		''' detail message and cause.  <p>Note that the detail message
		''' associated with {@code cause} is <i>not</i> automatically
		''' incorporated in this error's detail message.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''         by the <seealso cref="#getMessage()"/> method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.8 </param>
		Public Sub New(  message As String,   cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs an a {@code VirtualMachineError} with the specified
		''' cause and a detail message of {@code (cause==null ? null :
		''' cause.toString())} (which typically contains the class and
		''' detail message of {@code cause}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method).  (A {@code null} value is
		'''         permitted, and indicates that the cause is nonexistent or
		'''         unknown.)
		''' @since  1.8 </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
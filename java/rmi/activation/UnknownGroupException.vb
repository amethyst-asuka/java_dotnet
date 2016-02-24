'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.activation

	''' <summary>
	''' An <code>UnknownGroupException</code> is thrown by methods of classes and
	''' interfaces in the <code>java.rmi.activation</code> package when the
	''' <code>ActivationGroupID</code> parameter to the method is determined to be
	''' invalid, i.e., not known by the <code>ActivationSystem</code>.  An
	''' <code>UnknownGroupException</code> is also thrown if the
	''' <code>ActivationGroupID</code> in an <code>ActivationDesc</code> refers to
	''' a group that is not registered with the <code>ActivationSystem</code>
	''' 
	''' @author  Ann Wollrath
	''' @since   1.2 </summary>
	''' <seealso cref=     java.rmi.activation.Activatable </seealso>
	''' <seealso cref=     java.rmi.activation.ActivationGroup </seealso>
	''' <seealso cref=     java.rmi.activation.ActivationGroupID </seealso>
	''' <seealso cref=     java.rmi.activation.ActivationMonitor </seealso>
	''' <seealso cref=     java.rmi.activation.ActivationSystem </seealso>
	Public Class UnknownGroupException
		Inherits ActivationException

		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Shadows Const serialVersionUID As Long = 7056094974750002460L

		''' <summary>
		''' Constructs an <code>UnknownGroupException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since 1.2 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
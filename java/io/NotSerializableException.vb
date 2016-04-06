'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Thrown when an instance is required to have a Serializable interface.
	''' The serialization runtime or the class of the instance can throw
	''' this exception. The argument should be the name of the class.
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public Class NotSerializableException
		Inherits ObjectStreamException

		Private Shadows Const serialVersionUID As Long = 2906642554793891381L

		''' <summary>
		''' Constructs a NotSerializableException object with message string.
		''' </summary>
		''' <param name="classname"> Class of the instance being serialized/deserialized. </param>
		Public Sub New(  classname As String)
			MyBase.New(classname)
		End Sub

		''' <summary>
		'''  Constructs a NotSerializableException object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub
	End Class

End Namespace
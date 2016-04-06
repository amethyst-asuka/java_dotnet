Imports System.Threading

'
' * Copyright (c) 2006, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Context during upcalls from object stream to class-defined
	''' readObject/writeObject methods.
	''' Holds object currently being deserialized and descriptor for current class.
	''' 
	''' This context keeps track of the thread it was constructed on, and allows
	''' only a single call of defaultReadObject, readFields, defaultWriteObject
	''' or writeFields which must be invoked on the same thread before the class's
	''' readObject/writeObject method has returned.
	''' If not set to the current thread, the getObj method throws NotActiveException.
	''' </summary>
	Friend NotInheritable Class SerialCallbackContext
		Private ReadOnly obj As Object
		Private ReadOnly desc As ObjectStreamClass
		''' <summary>
		''' Thread this context is in use by.
		''' As this only works in one thread, we do not need to worry about thread-safety.
		''' </summary>
		Private thread As Thread

		Public Sub New(  obj As Object,   desc As ObjectStreamClass)
			Me.obj = obj
			Me.desc = desc
			Me.thread = Thread.CurrentThread
		End Sub

		Public Property obj As Object
			Get
				checkAndSetUsed()
				Return obj
			End Get
		End Property

		Public Property desc As ObjectStreamClass
			Get
				Return desc
			End Get
		End Property

		Public Sub check()
			If thread IsNot Nothing AndAlso thread IsNot Thread.CurrentThread Then Throw New NotActiveException("expected thread: " & thread & ", but got: " & Thread.CurrentThread)
		End Sub

		Private Sub checkAndSetUsed()
			If thread IsNot Thread.CurrentThread Then Throw New NotActiveException("not in readObject invocation or fields already read")
			thread = Nothing
		End Sub

		Public Sub setUsed()
			thread = Nothing
		End Sub
	End Class

End Namespace
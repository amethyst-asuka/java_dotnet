'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an application tries to access a type using a string
	''' representing the type's name, but no definition for the type with
	''' the specified name can be found.   This exception differs from
	''' <seealso cref="ClassNotFoundException"/> in that <tt>ClassNotFoundException</tt> is a
	''' checked exception, whereas this exception is unchecked.
	''' 
	''' <p>Note that this exception may be used when undefined type variables
	''' are accessed as well as when types (e.g., classes, interfaces or
	''' annotation types) are loaded.
	''' In particular, this exception can be thrown by the {@linkplain
	''' java.lang.reflect.AnnotatedElement API used to read annotations
	''' reflectively}.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     java.lang.reflect.AnnotatedElement
	''' @since 1.5 </seealso>
	Public Class TypeNotPresentException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -5101214195716534496L

		Private typeName_Renamed As String

		''' <summary>
		''' Constructs a <tt>TypeNotPresentException</tt> for the named type
		''' with the specified cause.
		''' </summary>
		''' <param name="typeName"> the fully qualified name of the unavailable type </param>
		''' <param name="cause"> the exception that was thrown when the system attempted to
		'''    load the named type, or <tt>null</tt> if unavailable or inapplicable </param>
		Public Sub New(ByVal typeName As String, ByVal cause As Throwable)
			MyBase.New("Type " & typeName & " not present", cause)
			Me.typeName_Renamed = typeName
		End Sub

		''' <summary>
		''' Returns the fully qualified name of the unavailable type.
		''' </summary>
		''' <returns> the fully qualified name of the unavailable type </returns>
		Public Overridable Function typeName() As String
			Return typeName_Renamed
		End Function
	End Class

End Namespace
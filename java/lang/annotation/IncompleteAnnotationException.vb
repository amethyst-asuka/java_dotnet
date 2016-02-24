'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.annotation

	''' <summary>
	''' Thrown to indicate that a program has attempted to access an element of
	''' an annotation type that was added to the annotation type definition after
	''' the annotation was compiled (or serialized).  This exception will not be
	''' thrown if the new element has a default value.
	''' This exception can be thrown by the {@linkplain
	''' java.lang.reflect.AnnotatedElement API used to read annotations
	''' reflectively}.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     java.lang.reflect.AnnotatedElement
	''' @since 1.5 </seealso>
	Public Class IncompleteAnnotationException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 8445097402741811912L

		Private annotationType_Renamed As Class
		Private elementName_Renamed As String

		''' <summary>
		''' Constructs an IncompleteAnnotationException to indicate that
		''' the named element was missing from the specified annotation type.
		''' </summary>
		''' <param name="annotationType"> the Class object for the annotation type </param>
		''' <param name="elementName"> the name of the missing element </param>
		''' <exception cref="NullPointerException"> if either parameter is {@code null} </exception>
		Public Sub New(ByVal annotationType As Class, ByVal elementName As String)
			MyBase.New(annotationType.name & " missing element " & elementName.ToString())

			Me.annotationType_Renamed = annotationType
			Me.elementName_Renamed = elementName
		End Sub

		''' <summary>
		''' Returns the Class object for the annotation type with the
		''' missing element.
		''' </summary>
		''' <returns> the Class object for the annotation type with the
		'''     missing element </returns>
		Public Overridable Function annotationType() As Class
			Return annotationType_Renamed
		End Function

		''' <summary>
		''' Returns the name of the missing element.
		''' </summary>
		''' <returns> the name of the missing element </returns>
		Public Overridable Function elementName() As String
			Return elementName_Renamed
		End Function
	End Class

End Namespace
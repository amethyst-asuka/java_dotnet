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

Namespace java.lang.annotation

	''' <summary>
	''' Thrown to indicate that a program has attempted to access an element of
	''' an annotation whose type has changed after the annotation was compiled
	''' (or serialized).
	''' This exception can be thrown by the {@linkplain
	''' java.lang.reflect.AnnotatedElement API used to read annotations
	''' reflectively}.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     java.lang.reflect.AnnotatedElement
	''' @since 1.5 </seealso>
	Public Class AnnotationTypeMismatchException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = 8125925355765570191L

		''' <summary>
		''' The <tt>Method</tt> object for the annotation element.
		''' </summary>
		Private ReadOnly element_Renamed As Method

		''' <summary>
		''' The (erroneous) type of data found in the annotation.  This string
		''' may, but is not required to, contain the value as well.  The exact
		''' format of the string is unspecified.
		''' </summary>
		Private ReadOnly foundType_Renamed As String

		''' <summary>
		''' Constructs an AnnotationTypeMismatchException for the specified
		''' annotation type element and found data type.
		''' </summary>
		''' <param name="element"> the <tt>Method</tt> object for the annotation element </param>
		''' <param name="foundType"> the (erroneous) type of data found in the annotation.
		'''        This string may, but is not required to, contain the value
		'''        as well.  The exact format of the string is unspecified. </param>
		Public Sub New(  element As Method,   foundType As String)
			MyBase.New("Incorrectly typed data found for annotation element " & element & " (Found data of type " & foundType & ")")
			Me.element_Renamed = element
			Me.foundType_Renamed = foundType
		End Sub

		''' <summary>
		''' Returns the <tt>Method</tt> object for the incorrectly typed element.
		''' </summary>
		''' <returns> the <tt>Method</tt> object for the incorrectly typed element </returns>
		Public Overridable Function element() As Method
			Return Me.element_Renamed
		End Function

		''' <summary>
		''' Returns the type of data found in the incorrectly typed element.
		''' The returned string may, but is not required to, contain the value
		''' as well.  The exact format of the string is unspecified.
		''' </summary>
		''' <returns> the type of data found in the incorrectly typed element </returns>
		Public Overridable Function foundType() As String
			Return Me.foundType_Renamed
		End Function
	End Class

End Namespace
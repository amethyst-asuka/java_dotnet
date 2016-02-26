'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model.element

	''' <summary>
	''' Represents a value of an annotation type element.
	''' A value is of one of the following types:
	''' <ul><li> a wrapper class (such as <seealso cref="Integer"/>) for a primitive type
	'''     <li> {@code String}
	'''     <li> {@code TypeMirror}
	'''     <li> {@code VariableElement} (representing an enum constant)
	'''     <li> {@code AnnotationMirror}
	'''     <li> {@code List<? extends AnnotationValue>}
	'''              (representing the elements, in declared order, if the value is an array)
	''' </ul>
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Interface AnnotationValue

		''' <summary>
		''' Returns the value.
		''' </summary>
		''' <returns> the value </returns>
		ReadOnly Property value As Object

		''' <summary>
		''' Returns a string representation of this value.
		''' This is returned in a form suitable for representing this value
		''' in the source code of an annotation.
		''' </summary>
		''' <returns> a string representation of this value </returns>
		Function ToString() As String

        ''' <summary>
        ''' Applies a visitor to this value.
        ''' </summary>
        ''' @param <R> the return type of the visitor's methods </param>
        ''' @param <P> the type of the additional parameter to the visitor's methods </param>
        ''' <param name="v">   the visitor operating on this value </param>
        ''' <param name="p">   additional parameter to the visitor </param>
        ''' <returns> a visitor-specified result </returns>
        Function accept(Of R, P)(ByVal v As AnnotationValueVisitor(Of R, P), ByVal params As P) As R
    End Interface

End Namespace
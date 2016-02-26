Imports javax.annotation
Imports javax.lang.model.element

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation.processing

	''' <summary>
	''' A {@code Messager} provides the way for an annotation processor to
	''' report error messages, warnings, and other notices.  Elements,
	''' annotations, and annotation values can be passed to provide a
	''' location hint for the message.  However, such location hints may be
	''' unavailable or only approximate.
	''' 
	''' <p>Printing a message with an {@linkplain
	''' javax.tools.Diagnostic.Kind#ERROR error kind} will {@linkplain
	''' RoundEnvironment#errorRaised raise an error}.
	''' 
	''' <p>Note that the messages &quot;printed&quot; by methods in this
	''' interface may or may not appear as textual output to a location
	''' like <seealso cref="System#out"/> or <seealso cref="System#err"/>.  Implementations may
	''' choose to present this information in a different fashion, such as
	''' messages in a window.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= ProcessingEnvironment#getLocale
	''' @since 1.6 </seealso>
	Public Interface Messager
		''' <summary>
		''' Prints a message of the specified kind.
		''' </summary>
		''' <param name="kind"> the kind of message </param>
		''' <param name="msg">  the message, or an empty string if none </param>
		Sub printMessage(ByVal kind As javax.tools.Diagnostic.Kind, ByVal msg As CharSequence)

		''' <summary>
		''' Prints a message of the specified kind at the location of the
		''' element.
		''' </summary>
		''' <param name="kind"> the kind of message </param>
		''' <param name="msg">  the message, or an empty string if none </param>
		''' <param name="e">    the element to use as a position hint </param>
		Sub printMessage(ByVal kind As javax.tools.Diagnostic.Kind, ByVal msg As CharSequence, ByVal e As Element)

		''' <summary>
		''' Prints a message of the specified kind at the location of the
		''' annotation mirror of the annotated element.
		''' </summary>
		''' <param name="kind"> the kind of message </param>
		''' <param name="msg">  the message, or an empty string if none </param>
		''' <param name="e">    the annotated element </param>
		''' <param name="a">    the annotation to use as a position hint </param>
		Sub printMessage(ByVal kind As javax.tools.Diagnostic.Kind, ByVal msg As CharSequence, ByVal e As Element, ByVal a As AnnotationMirror)

		''' <summary>
		''' Prints a message of the specified kind at the location of the
		''' annotation value inside the annotation mirror of the annotated
		''' element.
		''' </summary>
		''' <param name="kind"> the kind of message </param>
		''' <param name="msg">  the message, or an empty string if none </param>
		''' <param name="e">    the annotated element </param>
		''' <param name="a">    the annotation containing the annotation value </param>
		''' <param name="v">    the annotation value to use as a position hint </param>
		Sub printMessage(ByVal kind As javax.tools.Diagnostic.Kind, ByVal msg As CharSequence, ByVal e As Element, ByVal a As AnnotationMirror, ByVal v As AnnotationValue)
	End Interface

End Namespace
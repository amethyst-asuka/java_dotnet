Imports System

'
' * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model

	''' <summary>
	''' Superclass of exceptions which indicate that an unknown kind of
	''' entity was encountered.  This situation can occur if the language
	''' evolves and new kinds of constructs are introduced.  Subclasses of
	''' this exception may be thrown by visitors to indicate that the
	''' visitor was created for a prior version of the language.
	''' 
	''' <p>A common superclass for those exceptions allows a single catch
	''' block to have code handling them uniformly.
	''' 
	''' @author Joseph D. Darcy </summary>
	''' <seealso cref= javax.lang.model.element.UnknownElementException </seealso>
	''' <seealso cref= javax.lang.model.element.UnknownAnnotationValueException </seealso>
	''' <seealso cref= javax.lang.model.type.UnknownTypeException
	''' @since 1.7 </seealso>
	Public Class UnknownEntityException
		Inherits Exception

		Private Const serialVersionUID As Long = 269L

		''' <summary>
		''' Creates a new {@code UnknownEntityException} with the specified
		''' detail message.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Protected Friend Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace
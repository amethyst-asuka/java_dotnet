Imports System

'
' * Copyright (c) 2005, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' Indicates that an unknown kind of annotation value was encountered.
	''' This can occur if the language evolves and new kinds of annotation
	''' values can be stored in an annotation.  May be thrown by an
	''' <seealso cref="AnnotationValueVisitor annotation value visitor"/> to
	''' indicate that the visitor was created for a prior version of the
	''' language.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= AnnotationValueVisitor#visitUnknown
	''' @since 1.6 </seealso>
	Public Class UnknownAnnotationValueException
		Inherits javax.lang.model.UnknownEntityException

		Private Const serialVersionUID As Long = 269L

		<NonSerialized> _
		Private av As AnnotationValue
		<NonSerialized> _
		Private parameter As Object

		''' <summary>
		''' Creates a new {@code UnknownAnnotationValueException}.  The
		''' {@code p} parameter may be used to pass in an additional
		''' argument with information about the context in which the
		''' unknown annotation value was encountered; for example, the
		''' visit methods of <seealso cref="AnnotationValueVisitor"/> may pass in
		''' their additional parameter.
		''' </summary>
		''' <param name="av"> the unknown annotation value, may be {@code null} </param>
		''' <param name="p"> an additional parameter, may be {@code null} </param>
		Public Sub New(ByVal av As AnnotationValue, ByVal p As Object)
			MyBase.New("Unknown annotation value: " & av)
			Me.av = av
			Me.parameter = p
		End Sub

		''' <summary>
		''' Returns the unknown annotation value.
		''' The value may be unavailable if this exception has been
		''' serialized and then read back in.
		''' </summary>
		''' <returns> the unknown element, or {@code null} if unavailable </returns>
		Public Overridable Property unknownAnnotationValue As AnnotationValue
			Get
				Return av
			End Get
		End Property

		''' <summary>
		''' Returns the additional argument.
		''' </summary>
		''' <returns> the additional argument </returns>
		Public Overridable Property argument As Object
			Get
				Return parameter
			End Get
		End Property
	End Class

End Namespace
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

Namespace javax.lang.model.type


	''' <summary>
	''' Indicates that an unknown kind of type was encountered.  This can
	''' occur if the language evolves and new kinds of types are added to
	''' the {@code TypeMirror} hierarchy.  May be thrown by a {@linkplain
	''' TypeVisitor type visitor} to indicate that the visitor was created
	''' for a prior version of the language.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= TypeVisitor#visitUnknown
	''' @since 1.6 </seealso>
	Public Class UnknownTypeException
		Inherits javax.lang.model.UnknownEntityException

		Private Const serialVersionUID As Long = 269L

		<NonSerialized> _
		Private type As TypeMirror
		<NonSerialized> _
		Private parameter As Object

		''' <summary>
		''' Creates a new {@code UnknownTypeException}.The {@code p}
		''' parameter may be used to pass in an additional argument with
		''' information about the context in which the unknown type was
		''' encountered; for example, the visit methods of {@link
		''' TypeVisitor} may pass in their additional parameter.
		''' </summary>
		''' <param name="t"> the unknown type, may be {@code null} </param>
		''' <param name="p"> an additional parameter, may be {@code null} </param>
		Public Sub New(ByVal t As TypeMirror, ByVal p As Object)
			MyBase.New("Unknown type: " & t)
			type = t
			Me.parameter = p
		End Sub

		''' <summary>
		''' Returns the unknown type.
		''' The value may be unavailable if this exception has been
		''' serialized and then read back in.
		''' </summary>
		''' <returns> the unknown type, or {@code null} if unavailable </returns>
		 ReadOnly Public Overridable Property unknownType As TypeMirror
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Returns the additional argument.
		''' </summary>
		''' <returns> the additional argument </returns>
	 ReadOnly 	Public Overridable Property argument As Object
			Get
				Return parameter
			End Get
		End Property
	End Class

End Namespace
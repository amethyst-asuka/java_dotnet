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
	''' Indicates that an unknown kind of element was encountered.  This
	''' can occur if the language evolves and new kinds of elements are
	''' added to the {@code Element} hierarchy.  May be thrown by an
	''' <seealso cref="ElementVisitor element visitor"/> to indicate that the
	''' visitor was created for a prior version of the language.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= ElementVisitor#visitUnknown
	''' @since 1.6 </seealso>
	Public Class UnknownElementException
		Inherits javax.lang.model.UnknownEntityException

		Private Const serialVersionUID As Long = 269L

		<NonSerialized> _
		Private element As Element
		<NonSerialized> _
		Private parameter As Object

		''' <summary>
		''' Creates a new {@code UnknownElementException}.  The {@code p}
		''' parameter may be used to pass in an additional argument with
		''' information about the context in which the unknown element was
		''' encountered; for example, the visit methods of {@link
		''' ElementVisitor} may pass in their additional parameter.
		''' </summary>
		''' <param name="e"> the unknown element, may be {@code null} </param>
		''' <param name="p"> an additional parameter, may be {@code null} </param>
		Public Sub New(ByVal e As Element, ByVal p As Object)
			MyBase.New("Unknown element: " & e)
			element = e
			Me.parameter = p
		End Sub

        ''' <summary>
        ''' Returns the unknown element.
        ''' The value may be unavailable if this exception has been
        ''' serialized and then read back in.
        ''' </summary>
        ''' <returns> the unknown element, or {@code null} if unavailable </returns>
        Public Overridable ReadOnly Property unknownElement As Element
            Get
                Return element
            End Get
        End Property

        ''' <summary>
        ''' Returns the additional argument.
        ''' </summary>
        ''' <returns> the additional argument </returns>
        Public Overridable ReadOnly Property argument As Object
            Get
                Return parameter
            End Get
        End Property
    End Class

End Namespace
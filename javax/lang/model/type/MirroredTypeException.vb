Imports System

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

Namespace javax.lang.model.type



	''' <summary>
	''' Thrown when an application attempts to access the <seealso cref="Class"/> object
	''' corresponding to a <seealso cref="TypeMirror"/>.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= MirroredTypesException </seealso>
	''' <seealso cref= Element#getAnnotation(Class)
	''' @since 1.6 </seealso>
	Public Class MirroredTypeException
		Inherits MirroredTypesException

		Private Const serialVersionUID As Long = 269

		<NonSerialized> _
		Private type As TypeMirror ' cannot be serialized

		''' <summary>
		''' Constructs a new MirroredTypeException for the specified type.
		''' </summary>
		''' <param name="type">  the type being accessed </param>
		Public Sub New(ByVal type As TypeMirror)
			MyBase.New("Attempt to access Class object for TypeMirror " & type.ToString(), type)
			Me.type = type
		End Sub

        ''' <summary>
        ''' Returns the type mirror corresponding to the type being accessed.
        ''' The type mirror may be unavailable if this exception has been
        ''' serialized and then read back in.
        ''' </summary>
        ''' <returns> the type mirror, or {@code null} if unavailable </returns>
        Public Overridable ReadOnly Property typeMirror As TypeMirror
            Get
                Return type
            End Get
        End Property

        ''' <summary>
        ''' Explicitly set all transient fields.
        ''' </summary>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			type = Nothing
			types = Nothing
		End Sub
	End Class

End Namespace
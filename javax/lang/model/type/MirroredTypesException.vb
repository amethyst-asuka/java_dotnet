Imports System
Imports System.Collections.Generic

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
	''' Thrown when an application attempts to access a sequence of {@link
	''' Class} objects each corresponding to a <seealso cref="TypeMirror"/>.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= MirroredTypeException </seealso>
	''' <seealso cref= Element#getAnnotation(Class)
	''' @since 1.6 </seealso>
	Public Class MirroredTypesException
		Inherits Exception

		Private Const serialVersionUID As Long = 269

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		<NonSerialized> _
		Friend types As IList(Of ? As TypeMirror) ' cannot be serialized

	'    
	'     * Trusted constructor to be called by MirroredTypeException.
	'     
		Friend Sub New(ByVal message As String, ByVal type As TypeMirror)
			MyBase.New(message)
			Dim tmp As IList(Of TypeMirror) = (New List(Of TypeMirror))
			tmp.Add(type)
			types = java.util.Collections.unmodifiableList(tmp)
		End Sub

		''' <summary>
		''' Constructs a new MirroredTypesException for the specified types.
		''' </summary>
		''' <param name="types">  the types being accessed </param>
		Public Sub New(Of T1 As TypeMirror)(ByVal types As IList(Of T1))
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			MyBase.New("Attempt to access Class objects for TypeMirrors " & (types = New List(Of TypeMirror)(types)).ToString()) ' defensive copy
			Me.types = java.util.Collections.unmodifiableList(types)
		End Sub

		''' <summary>
		''' Returns the type mirrors corresponding to the types being accessed.
		''' The type mirrors may be unavailable if this exception has been
		''' serialized and then read back in.
		''' </summary>
		''' <returns> the type mirrors in construction order, or {@code null} if unavailable </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property typeMirrors As IList(Of ? As TypeMirror)
			Get
				Return types
			End Get
		End Property

		''' <summary>
		''' Explicitly set all transient fields.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			types = Nothing
		End Sub
	End Class

End Namespace
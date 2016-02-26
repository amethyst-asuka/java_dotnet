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

Namespace javax.tools


	''' <summary>
	''' Forwards calls to a given file object.  Subclasses of this class
	''' might override some of these methods and might also provide
	''' additional fields and methods.
	''' </summary>
	''' @param <F> the kind of file object forwarded to by this object
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public Class ForwardingJavaFileObject(Of F As JavaFileObject)
		Inherits ForwardingFileObject(Of F)
		Implements JavaFileObject

		''' <summary>
		''' Creates a new instance of ForwardingJavaFileObject. </summary>
		''' <param name="fileObject"> delegate to this file object </param>
		Protected Friend Sub New(ByVal fileObject As F)
			MyBase.New(fileObject)
		End Sub

		Public Overridable Property kind As Kind Implements JavaFileObject.getKind
			Get
				Return fileObject.kind
			End Get
		End Property

		Public Overridable Function isNameCompatible(ByVal simpleName As String, ByVal kind As Kind) As Boolean Implements JavaFileObject.isNameCompatible
			Return fileObject.isNameCompatible(simpleName, kind)
		End Function

		Public Overridable Property nestingKind As javax.lang.model.element.NestingKind Implements JavaFileObject.getNestingKind
			Get
				Return fileObject.nestingKind
			End Get
		End Property

		Public Overridable Property accessLevel As javax.lang.model.element.Modifier Implements JavaFileObject.getAccessLevel
			Get
				Return fileObject.accessLevel
			End Get
		End Property

	End Class

End Namespace
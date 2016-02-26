Imports System
Imports System.Collections.Generic
Imports javax.naming

'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.spi

	''' <summary>
	''' This class is for dealing with federations/continuations.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Friend Class ContinuationContext
		Implements Context, Resolver

		Protected Friend cpe As CannotProceedException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend env As Dictionary(Of ?, ?)
		Protected Friend contCtx As Context = Nothing

		Protected Friend Sub New(Of T1)(ByVal cpe As CannotProceedException, ByVal env As Dictionary(Of T1))
			Me.cpe = cpe
			Me.env = env
		End Sub

	ReadOnly	Protected Friend Overridable Property targetContext As Context
			Get
				If contCtx Is Nothing Then
					If cpe.resolvedObj Is Nothing Then Throw CType(cpe.fillInStackTrace(), NamingException)
    
					contCtx = NamingManager.getContext(cpe.resolvedObj, cpe.altName, cpe.altNameCtx, env)
					If contCtx Is Nothing Then Throw CType(cpe.fillInStackTrace(), NamingException)
				End If
				Return contCtx
			End Get
		End Property

		Public Overridable Function lookup(ByVal name As Name) As Object Implements Context.lookup
			Dim ctx As Context = targetContext
			Return ctx.lookup(name)
		End Function

		Public Overridable Function lookup(ByVal name As String) As Object Implements Context.lookup
			Dim ctx As Context = targetContext
			Return ctx.lookup(name)
		End Function

		Public Overridable Sub bind(ByVal name As Name, ByVal newObj As Object) Implements Context.bind
			Dim ctx As Context = targetContext
			ctx.bind(name, newObj)
		End Sub

		Public Overridable Sub bind(ByVal name As String, ByVal newObj As Object) Implements Context.bind
			Dim ctx As Context = targetContext
			ctx.bind(name, newObj)
		End Sub

		Public Overridable Sub rebind(ByVal name As Name, ByVal newObj As Object) Implements Context.rebind
			Dim ctx As Context = targetContext
			ctx.rebind(name, newObj)
		End Sub
		Public Overridable Sub rebind(ByVal name As String, ByVal newObj As Object) Implements Context.rebind
			Dim ctx As Context = targetContext
			ctx.rebind(name, newObj)
		End Sub

		Public Overridable Sub unbind(ByVal name As Name) Implements Context.unbind
			Dim ctx As Context = targetContext
			ctx.unbind(name)
		End Sub
		Public Overridable Sub unbind(ByVal name As String) Implements Context.unbind
			Dim ctx As Context = targetContext
			ctx.unbind(name)
		End Sub

		Public Overridable Sub rename(ByVal name As Name, ByVal newName As Name) Implements Context.rename
			Dim ctx As Context = targetContext
			ctx.rename(name, newName)
		End Sub
		Public Overridable Sub rename(ByVal name As String, ByVal newName As String) Implements Context.rename
			Dim ctx As Context = targetContext
			ctx.rename(name, newName)
		End Sub

		Public Overridable Function list(ByVal name As Name) As NamingEnumeration(Of NameClassPair) Implements Context.list
			Dim ctx As Context = targetContext
			Return ctx.list(name)
		End Function
		Public Overridable Function list(ByVal name As String) As NamingEnumeration(Of NameClassPair) Implements Context.list
			Dim ctx As Context = targetContext
			Return ctx.list(name)
		End Function


		Public Overridable Function listBindings(ByVal name As Name) As NamingEnumeration(Of Binding) Implements Context.listBindings
			Dim ctx As Context = targetContext
			Return ctx.listBindings(name)
		End Function

		Public Overridable Function listBindings(ByVal name As String) As NamingEnumeration(Of Binding) Implements Context.listBindings
			Dim ctx As Context = targetContext
			Return ctx.listBindings(name)
		End Function

		Public Overridable Sub destroySubcontext(ByVal name As Name) Implements Context.destroySubcontext
			Dim ctx As Context = targetContext
			ctx.destroySubcontext(name)
		End Sub
		Public Overridable Sub destroySubcontext(ByVal name As String) Implements Context.destroySubcontext
			Dim ctx As Context = targetContext
			ctx.destroySubcontext(name)
		End Sub

		Public Overridable Function createSubcontext(ByVal name As Name) As Context Implements Context.createSubcontext
			Dim ctx As Context = targetContext
			Return ctx.createSubcontext(name)
		End Function
		Public Overridable Function createSubcontext(ByVal name As String) As Context Implements Context.createSubcontext
			Dim ctx As Context = targetContext
			Return ctx.createSubcontext(name)
		End Function

		Public Overridable Function lookupLink(ByVal name As Name) As Object Implements Context.lookupLink
			Dim ctx As Context = targetContext
			Return ctx.lookupLink(name)
		End Function
		Public Overridable Function lookupLink(ByVal name As String) As Object Implements Context.lookupLink
			Dim ctx As Context = targetContext
			Return ctx.lookupLink(name)
		End Function

		Public Overridable Function getNameParser(ByVal name As Name) As NameParser Implements Context.getNameParser
			Dim ctx As Context = targetContext
			Return ctx.getNameParser(name)
		End Function

		Public Overridable Function getNameParser(ByVal name As String) As NameParser Implements Context.getNameParser
			Dim ctx As Context = targetContext
			Return ctx.getNameParser(name)
		End Function

		Public Overridable Function composeName(ByVal name As Name, ByVal prefix As Name) As Name Implements Context.composeName
			Dim ctx As Context = targetContext
			Return ctx.composeName(name, prefix)
		End Function

		Public Overridable Function composeName(ByVal name As String, ByVal prefix As String) As String Implements Context.composeName
			Dim ctx As Context = targetContext
			Return ctx.composeName(name, prefix)
		End Function

		Public Overridable Function addToEnvironment(ByVal propName As String, ByVal value As Object) As Object Implements Context.addToEnvironment
			Dim ctx As Context = targetContext
			Return ctx.addToEnvironment(propName, value)
		End Function

		Public Overridable Function removeFromEnvironment(ByVal propName As String) As Object Implements Context.removeFromEnvironment
			Dim ctx As Context = targetContext
			Return ctx.removeFromEnvironment(propName)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property environment As Dictionary(Of ?, ?) Implements Context.getEnvironment
			Get
				Dim ctx As Context = targetContext
				Return ctx.environment
			End Get
		End Property

		Public Overridable Property nameInNamespace As String Implements Context.getNameInNamespace
			Get
				Dim ctx As Context = targetContext
				Return ctx.nameInNamespace
			End Get
		End Property

		Public Overridable Function resolveToClass(ByVal name As Name, ByVal contextType As Type) As ResolveResult
			If cpe.resolvedObj Is Nothing Then Throw CType(cpe.fillInStackTrace(), NamingException)

			Dim res As Resolver = NamingManager.getResolver(cpe.resolvedObj, cpe.altName, cpe.altNameCtx, env)
			If res Is Nothing Then Throw CType(cpe.fillInStackTrace(), NamingException)
			Return res.resolveToClass(name, contextType)
		End Function

		Public Overridable Function resolveToClass(ByVal name As String, ByVal contextType As Type) As ResolveResult Implements Resolver.resolveToClass
			If cpe.resolvedObj Is Nothing Then Throw CType(cpe.fillInStackTrace(), NamingException)

			Dim res As Resolver = NamingManager.getResolver(cpe.resolvedObj, cpe.altName, cpe.altNameCtx, env)
			If res Is Nothing Then Throw CType(cpe.fillInStackTrace(), NamingException)
			Return res.resolveToClass(name, contextType)
		End Function

		Public Overridable Sub close() Implements Context.close
			cpe = Nothing
			env = Nothing
			If contCtx IsNot Nothing Then
				contCtx.close()
				contCtx = Nothing
			End If
		End Sub
	End Class

End Namespace
Imports System.Collections.Generic

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
	''' This class is the continuation context for invoking DirContext methods.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Friend Class ContinuationDirContext
		Inherits ContinuationContext
		Implements javax.naming.directory.DirContext

		Friend Sub New(Of T1)(ByVal cpe As javax.naming.CannotProceedException, ByVal env As Dictionary(Of T1))
			MyBase.New(cpe, env)
		End Sub

		Protected Friend Overridable Function getTargetContext(ByVal name As javax.naming.Name) As DirContextNamePair

			If cpe.resolvedObj Is Nothing Then Throw CType(cpe.fillInStackTrace(), javax.naming.NamingException)

			Dim ctx As javax.naming.Context = NamingManager.getContext(cpe.resolvedObj, cpe.altName, cpe.altNameCtx, env)
			If ctx Is Nothing Then Throw CType(cpe.fillInStackTrace(), javax.naming.NamingException)

			If TypeOf ctx Is javax.naming.directory.DirContext Then Return New DirContextNamePair(CType(ctx, javax.naming.directory.DirContext), name)

			If TypeOf ctx Is Resolver Then
				Dim res As Resolver = CType(ctx, Resolver)
				Dim rr As ResolveResult = res.resolveToClass(name, GetType(javax.naming.directory.DirContext))

				' Reached a DirContext; return result.
				Dim dctx As javax.naming.directory.DirContext = CType(rr.resolvedObj, javax.naming.directory.DirContext)
				Return (New DirContextNamePair(dctx, rr.remainingName))
			End If

			' Resolve all the way using lookup().  This may allow the operation
			' to succeed if it doesn't require the penultimate context.
			Dim ultimate As Object = ctx.lookup(name)
			If TypeOf ultimate Is javax.naming.directory.DirContext Then Return (New DirContextNamePair(CType(ultimate, javax.naming.directory.DirContext), New javax.naming.CompositeName))

			Throw CType(cpe.fillInStackTrace(), javax.naming.NamingException)
		End Function

		Protected Friend Overridable Function getTargetContext(ByVal name As String) As DirContextStringPair

			If cpe.resolvedObj Is Nothing Then Throw CType(cpe.fillInStackTrace(), javax.naming.NamingException)

			Dim ctx As javax.naming.Context = NamingManager.getContext(cpe.resolvedObj, cpe.altName, cpe.altNameCtx, env)

			If TypeOf ctx Is javax.naming.directory.DirContext Then Return New DirContextStringPair(CType(ctx, javax.naming.directory.DirContext), name)

			If TypeOf ctx Is Resolver Then
				Dim res As Resolver = CType(ctx, Resolver)
				Dim rr As ResolveResult = res.resolveToClass(name, GetType(javax.naming.directory.DirContext))

				' Reached a DirContext; return result.
				Dim dctx As javax.naming.directory.DirContext = CType(rr.resolvedObj, javax.naming.directory.DirContext)
				Dim tmp As javax.naming.Name = rr.remainingName
				Dim remains As String = If(tmp IsNot Nothing, tmp.ToString(), "")
				Return (New DirContextStringPair(dctx, remains))
			End If

			' Resolve all the way using lookup().  This may allow the operation
			' to succeed if it doesn't require the penultimate context.
			Dim ultimate As Object = ctx.lookup(name)
			If TypeOf ultimate Is javax.naming.directory.DirContext Then Return (New DirContextStringPair(CType(ultimate, javax.naming.directory.DirContext), ""))

			Throw CType(cpe.fillInStackTrace(), javax.naming.NamingException)
		End Function

		Public Overridable Function getAttributes(ByVal name As String) As javax.naming.directory.Attributes
			Dim res As DirContextStringPair = getTargetContext(name)
			Return res.dirContext.getAttributes(res.string)
		End Function

		Public Overridable Function getAttributes(ByVal name As String, ByVal attrIds As String()) As javax.naming.directory.Attributes
				Dim res As DirContextStringPair = getTargetContext(name)
				Return res.dirContext.getAttributes(res.string, attrIds)
		End Function

		Public Overridable Function getAttributes(ByVal name As javax.naming.Name) As javax.naming.directory.Attributes
			Dim res As DirContextNamePair = getTargetContext(name)
			Return res.dirContext.getAttributes(res.name)
		End Function

		Public Overridable Function getAttributes(ByVal name As javax.naming.Name, ByVal attrIds As String()) As javax.naming.directory.Attributes
				Dim res As DirContextNamePair = getTargetContext(name)
				Return res.dirContext.getAttributes(res.name, attrIds)
		End Function

		Public Overridable Sub modifyAttributes(ByVal name As javax.naming.Name, ByVal mod_op As Integer, ByVal attrs As javax.naming.directory.Attributes)
				Dim res As DirContextNamePair = getTargetContext(name)
				res.dirContext.modifyAttributes(res.name, mod_op, attrs)
		End Sub
		Public Overridable Sub modifyAttributes(ByVal name As String, ByVal mod_op As Integer, ByVal attrs As javax.naming.directory.Attributes)
				Dim res As DirContextStringPair = getTargetContext(name)
				res.dirContext.modifyAttributes(res.string, mod_op, attrs)
		End Sub

		Public Overridable Sub modifyAttributes(ByVal name As javax.naming.Name, ByVal mods As javax.naming.directory.ModificationItem())
				Dim res As DirContextNamePair = getTargetContext(name)
				res.dirContext.modifyAttributes(res.name, mods)
		End Sub
		Public Overridable Sub modifyAttributes(ByVal name As String, ByVal mods As javax.naming.directory.ModificationItem())
				Dim res As DirContextStringPair = getTargetContext(name)
				res.dirContext.modifyAttributes(res.string, mods)
		End Sub

		Public Overridable Sub bind(ByVal name As javax.naming.Name, ByVal obj As Object, ByVal attrs As javax.naming.directory.Attributes)
				Dim res As DirContextNamePair = getTargetContext(name)
				res.dirContext.bind(res.name, obj, attrs)
		End Sub
		Public Overridable Sub bind(ByVal name As String, ByVal obj As Object, ByVal attrs As javax.naming.directory.Attributes)
				Dim res As DirContextStringPair = getTargetContext(name)
				res.dirContext.bind(res.string, obj, attrs)
		End Sub

		Public Overridable Sub rebind(ByVal name As javax.naming.Name, ByVal obj As Object, ByVal attrs As javax.naming.directory.Attributes)
				Dim res As DirContextNamePair = getTargetContext(name)
				res.dirContext.rebind(res.name, obj, attrs)
		End Sub
		Public Overridable Sub rebind(ByVal name As String, ByVal obj As Object, ByVal attrs As javax.naming.directory.Attributes)
				Dim res As DirContextStringPair = getTargetContext(name)
				res.dirContext.rebind(res.string, obj, attrs)
		End Sub

		Public Overridable Function createSubcontext(ByVal name As javax.naming.Name, ByVal attrs As javax.naming.directory.Attributes) As javax.naming.directory.DirContext
				Dim res As DirContextNamePair = getTargetContext(name)
				Return res.dirContext.createSubcontext(res.name, attrs)
		End Function

		Public Overridable Function createSubcontext(ByVal name As String, ByVal attrs As javax.naming.directory.Attributes) As javax.naming.directory.DirContext
				Dim res As DirContextStringPair = getTargetContext(name)
				Return res.dirContext.createSubcontext(res.string, attrs)
		End Function

		Public Overridable Function search(ByVal name As javax.naming.Name, ByVal matchingAttributes As javax.naming.directory.Attributes, ByVal attributesToReturn As String()) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextNamePair = getTargetContext(name)
				Return res.dirContext.search(res.name, matchingAttributes, attributesToReturn)
		End Function

		Public Overridable Function search(ByVal name As String, ByVal matchingAttributes As javax.naming.directory.Attributes, ByVal attributesToReturn As String()) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextStringPair = getTargetContext(name)
				Return res.dirContext.search(res.string, matchingAttributes, attributesToReturn)
		End Function

		Public Overridable Function search(ByVal name As javax.naming.Name, ByVal matchingAttributes As javax.naming.directory.Attributes) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextNamePair = getTargetContext(name)
				Return res.dirContext.search(res.name, matchingAttributes)
		End Function
		Public Overridable Function search(ByVal name As String, ByVal matchingAttributes As javax.naming.directory.Attributes) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextStringPair = getTargetContext(name)
				Return res.dirContext.search(res.string, matchingAttributes)
		End Function

		Public Overridable Function search(ByVal name As javax.naming.Name, ByVal filter As String, ByVal cons As javax.naming.directory.SearchControls) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextNamePair = getTargetContext(name)
				Return res.dirContext.search(res.name, filter, cons)
		End Function

		Public Overridable Function search(ByVal name As String, ByVal filter As String, ByVal cons As javax.naming.directory.SearchControls) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextStringPair = getTargetContext(name)
				Return res.dirContext.search(res.string, filter, cons)
		End Function

		Public Overridable Function search(ByVal name As javax.naming.Name, ByVal filterExpr As String, ByVal args As Object(), ByVal cons As javax.naming.directory.SearchControls) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextNamePair = getTargetContext(name)
				Return res.dirContext.search(res.name, filterExpr, args, cons)
		End Function

		Public Overridable Function search(ByVal name As String, ByVal filterExpr As String, ByVal args As Object(), ByVal cons As javax.naming.directory.SearchControls) As javax.naming.NamingEnumeration(Of javax.naming.directory.SearchResult)
				Dim res As DirContextStringPair = getTargetContext(name)
				Return res.dirContext.search(res.string, filterExpr, args, cons)
		End Function

		Public Overridable Function getSchema(ByVal name As String) As javax.naming.directory.DirContext
			Dim res As DirContextStringPair = getTargetContext(name)
			Return res.dirContext.getSchema(res.string)
		End Function

		Public Overridable Function getSchema(ByVal name As javax.naming.Name) As javax.naming.directory.DirContext
			Dim res As DirContextNamePair = getTargetContext(name)
			Return res.dirContext.getSchema(res.name)
		End Function

		Public Overridable Function getSchemaClassDefinition(ByVal name As String) As javax.naming.directory.DirContext
			Dim res As DirContextStringPair = getTargetContext(name)
			Return res.dirContext.getSchemaClassDefinition(res.string)
		End Function

		Public Overridable Function getSchemaClassDefinition(ByVal name As javax.naming.Name) As javax.naming.directory.DirContext
			Dim res As DirContextNamePair = getTargetContext(name)
			Return res.dirContext.getSchemaClassDefinition(res.name)
		End Function
	End Class

	Friend Class DirContextNamePair
			Friend ctx As javax.naming.directory.DirContext
			Friend name As javax.naming.Name

			Friend Sub New(ByVal ctx As javax.naming.directory.DirContext, ByVal name As javax.naming.Name)
				Me.ctx = ctx
				Me.name = name
			End Sub

			Friend Overridable Property dirContext As javax.naming.directory.DirContext
				Get
					Return ctx
				End Get
			End Property

			Friend Overridable Property name As javax.naming.Name
				Get
					Return name
				End Get
			End Property
	End Class

	Friend Class DirContextStringPair
			Friend ctx As javax.naming.directory.DirContext
			Friend str As String

			Friend Sub New(ByVal ctx As javax.naming.directory.DirContext, ByVal str As String)
				Me.ctx = ctx
				Me.str = str
			End Sub

			Friend Overridable Property dirContext As javax.naming.directory.DirContext
				Get
					Return ctx
				End Get
			End Property

			Friend Overridable Property [string] As String
				Get
					Return str
				End Get
			End Property
	End Class

End Namespace
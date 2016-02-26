Imports System.Collections.Generic
Imports javax.naming

'
' * Copyright (c) 1999, 2009, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.naming.directory

	''' <summary>
	''' This class is the starting context for performing
	''' directory operations. The documentation in the class description
	''' of InitialContext (including those for synchronization) apply here.
	''' 
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= javax.naming.InitialContext
	''' @since 1.3 </seealso>

	Public Class InitialDirContext
		Inherits InitialContext
		Implements DirContext

		''' <summary>
		''' Constructs an initial DirContext with the option of not
		''' initializing it.  This may be used by a constructor in
		''' a subclass when the value of the environment parameter
		''' is not yet known at the time the <tt>InitialDirContext</tt>
		''' constructor is called.  The subclass's constructor will
		''' call this constructor, compute the value of the environment,
		''' and then call <tt>init()</tt> before returning.
		''' </summary>
		''' <param name="lazy">
		'''          true means do not initialize the initial DirContext; false
		'''          is equivalent to calling <tt>new InitialDirContext()</tt> </param>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= InitialContext#init(Hashtable)
		''' @since 1.3 </seealso>
		Protected Friend Sub New(ByVal lazy As Boolean)
			MyBase.New(lazy)
		End Sub

		''' <summary>
		''' Constructs an initial DirContext.
		''' No environment properties are supplied.
		''' Equivalent to <tt>new InitialDirContext(null)</tt>.
		''' </summary>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= #InitialDirContext(Hashtable) </seealso>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an initial DirContext using the supplied environment.
		''' Environment properties are discussed in the
		''' <tt>javax.naming.InitialContext</tt> class description.
		''' 
		''' <p> This constructor will not modify <tt>environment</tt>
		''' or save a reference to it, but may save a clone.
		''' Caller should not modify mutable keys and values in
		''' <tt>environment</tt> after it has been passed to the constructor.
		''' </summary>
		''' <param name="environment">
		'''          environment used to create the initial DirContext.
		'''          Null indicates an empty environment.
		''' </param>
		''' <exception cref="NamingException"> if a naming exception is encountered </exception>
		Public Sub New(Of T1)(ByVal environment As Dictionary(Of T1))
			MyBase.New(environment)
		End Sub

		Private Function getURLOrDefaultInitDirCtx(ByVal name As String) As DirContext
			Dim answer As Context = getURLOrDefaultInitCtx(name)
			If Not(TypeOf answer Is DirContext) Then
				If answer Is Nothing Then
					Throw New NoInitialContextException
				Else
					Throw New NotContextException("Not an instance of DirContext")
				End If
			End If
			Return CType(answer, DirContext)
		End Function

		Private Function getURLOrDefaultInitDirCtx(ByVal name As Name) As DirContext
			Dim answer As Context = getURLOrDefaultInitCtx(name)
			If Not(TypeOf answer Is DirContext) Then
				If answer Is Nothing Then
					Throw New NoInitialContextException
				Else
					Throw New NotContextException("Not an instance of DirContext")
				End If
			End If
			Return CType(answer, DirContext)
		End Function

	' DirContext methods
	' Most Javadoc is deferred to the DirContext interface.

		Public Overridable Function getAttributes(ByVal name As String) As Attributes Implements DirContext.getAttributes
			Return getAttributes(name, Nothing)
		End Function

		Public Overridable Function getAttributes(ByVal name As String, ByVal attrIds As String()) As Attributes Implements DirContext.getAttributes
			Return getURLOrDefaultInitDirCtx(name).getAttributes(name, attrIds)
		End Function

		Public Overridable Function getAttributes(ByVal name As Name) As Attributes Implements DirContext.getAttributes
			Return getAttributes(name, Nothing)
		End Function

		Public Overridable Function getAttributes(ByVal name As Name, ByVal attrIds As String()) As Attributes Implements DirContext.getAttributes
			Return getURLOrDefaultInitDirCtx(name).getAttributes(name, attrIds)
		End Function

		Public Overridable Sub modifyAttributes(ByVal name As String, ByVal mod_op As Integer, ByVal attrs As Attributes) Implements DirContext.modifyAttributes
			getURLOrDefaultInitDirCtx(name).modifyAttributes(name, mod_op, attrs)
		End Sub

		Public Overridable Sub modifyAttributes(ByVal name As Name, ByVal mod_op As Integer, ByVal attrs As Attributes) Implements DirContext.modifyAttributes
			getURLOrDefaultInitDirCtx(name).modifyAttributes(name, mod_op, attrs)
		End Sub

		Public Overridable Sub modifyAttributes(ByVal name As String, ByVal mods As ModificationItem()) Implements DirContext.modifyAttributes
			getURLOrDefaultInitDirCtx(name).modifyAttributes(name, mods)
		End Sub

		Public Overridable Sub modifyAttributes(ByVal name As Name, ByVal mods As ModificationItem()) Implements DirContext.modifyAttributes
			getURLOrDefaultInitDirCtx(name).modifyAttributes(name, mods)
		End Sub

		Public Overridable Sub bind(ByVal name As String, ByVal obj As Object, ByVal attrs As Attributes) Implements DirContext.bind
			getURLOrDefaultInitDirCtx(name).bind(name, obj, attrs)
		End Sub

		Public Overridable Sub bind(ByVal name As Name, ByVal obj As Object, ByVal attrs As Attributes) Implements DirContext.bind
			getURLOrDefaultInitDirCtx(name).bind(name, obj, attrs)
		End Sub

		Public Overridable Sub rebind(ByVal name As String, ByVal obj As Object, ByVal attrs As Attributes) Implements DirContext.rebind
			getURLOrDefaultInitDirCtx(name).rebind(name, obj, attrs)
		End Sub

		Public Overridable Sub rebind(ByVal name As Name, ByVal obj As Object, ByVal attrs As Attributes) Implements DirContext.rebind
			getURLOrDefaultInitDirCtx(name).rebind(name, obj, attrs)
		End Sub

		Public Overridable Function createSubcontext(ByVal name As String, ByVal attrs As Attributes) As DirContext Implements DirContext.createSubcontext
			Return getURLOrDefaultInitDirCtx(name).createSubcontext(name, attrs)
		End Function

		Public Overridable Function createSubcontext(ByVal name As Name, ByVal attrs As Attributes) As DirContext Implements DirContext.createSubcontext
			Return getURLOrDefaultInitDirCtx(name).createSubcontext(name, attrs)
		End Function

		Public Overridable Function getSchema(ByVal name As String) As DirContext Implements DirContext.getSchema
			Return getURLOrDefaultInitDirCtx(name).getSchema(name)
		End Function

		Public Overridable Function getSchema(ByVal name As Name) As DirContext Implements DirContext.getSchema
			Return getURLOrDefaultInitDirCtx(name).getSchema(name)
		End Function

		Public Overridable Function getSchemaClassDefinition(ByVal name As String) As DirContext Implements DirContext.getSchemaClassDefinition
			Return getURLOrDefaultInitDirCtx(name).getSchemaClassDefinition(name)
		End Function

		Public Overridable Function getSchemaClassDefinition(ByVal name As Name) As DirContext Implements DirContext.getSchemaClassDefinition
			Return getURLOrDefaultInitDirCtx(name).getSchemaClassDefinition(name)
		End Function

	' -------------------- search operations

		Public Overridable Function search(ByVal name As String, ByVal matchingAttributes As Attributes) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, matchingAttributes)
		End Function

		Public Overridable Function search(ByVal name As Name, ByVal matchingAttributes As Attributes) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, matchingAttributes)
		End Function

		Public Overridable Function search(ByVal name As String, ByVal matchingAttributes As Attributes, ByVal attributesToReturn As String()) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, matchingAttributes, attributesToReturn)
		End Function

		Public Overridable Function search(ByVal name As Name, ByVal matchingAttributes As Attributes, ByVal attributesToReturn As String()) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, matchingAttributes, attributesToReturn)
		End Function

		Public Overridable Function search(ByVal name As String, ByVal filter As String, ByVal cons As SearchControls) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, filter, cons)
		End Function

		Public Overridable Function search(ByVal name As Name, ByVal filter As String, ByVal cons As SearchControls) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, filter, cons)
		End Function

		Public Overridable Function search(ByVal name As String, ByVal filterExpr As String, ByVal filterArgs As Object(), ByVal cons As SearchControls) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, filterExpr, filterArgs, cons)
		End Function

		Public Overridable Function search(ByVal name As Name, ByVal filterExpr As String, ByVal filterArgs As Object(), ByVal cons As SearchControls) As NamingEnumeration(Of SearchResult) Implements DirContext.search
			Return getURLOrDefaultInitDirCtx(name).search(name, filterExpr, filterArgs, cons)
		End Function
	End Class

End Namespace
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.script


	''' <summary>
	''' Simple implementation of ScriptContext.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public Class SimpleScriptContext
		Implements ScriptContext

		''' <summary>
		''' This is the writer to be used to output from scripts.
		''' By default, a <code>PrintWriter</code> based on <code>System.out</code>
		''' is used. Accessor methods getWriter, setWriter are used to manage
		''' this field. </summary>
		''' <seealso cref= java.lang.System#out </seealso>
		''' <seealso cref= java.io.PrintWriter </seealso>
		Protected Friend writer As Writer

		''' <summary>
		''' This is the writer to be used to output errors from scripts.
		''' By default, a <code>PrintWriter</code> based on <code>System.err</code> is
		''' used. Accessor methods getErrorWriter, setErrorWriter are used to manage
		''' this field. </summary>
		''' <seealso cref= java.lang.System#err </seealso>
		''' <seealso cref= java.io.PrintWriter </seealso>
		Protected Friend errorWriter As Writer

		''' <summary>
		''' This is the reader to be used for input from scripts.
		''' By default, a <code>InputStreamReader</code> based on <code>System.in</code>
		''' is used and default charset is used by this reader. Accessor methods
		''' getReader, setReader are used to manage this field. </summary>
		''' <seealso cref= java.lang.System#in </seealso>
		''' <seealso cref= java.io.InputStreamReader </seealso>
		Protected Friend reader As Reader


		''' <summary>
		''' This is the engine scope bindings.
		''' By default, a <code>SimpleBindings</code> is used. Accessor
		''' methods setBindings, getBindings are used to manage this field. </summary>
		''' <seealso cref= SimpleBindings </seealso>
		Protected Friend engineScope As Bindings

		''' <summary>
		''' This is the global scope bindings.
		''' By default, a null value (which means no global scope) is used. Accessor
		''' methods setBindings, getBindings are used to manage this field.
		''' </summary>
		Protected Friend globalScope As Bindings

		''' <summary>
		''' Create a {@code SimpleScriptContext}.
		''' </summary>
		Public Sub New()
			engineScope = New SimpleBindings
			globalScope = Nothing
			reader = New InputStreamReader(System.in)
			writer = New PrintWriter(System.out, True)
			errorWriter = New PrintWriter(System.err, True)
		End Sub

		''' <summary>
		''' Sets a <code>Bindings</code> of attributes for the given scope.  If the value
		''' of scope is <code>ENGINE_SCOPE</code> the given <code>Bindings</code> replaces the
		''' <code>engineScope</code> field.  If the value
		''' of scope is <code>GLOBAL_SCOPE</code> the given <code>Bindings</code> replaces the
		''' <code>globalScope</code> field.
		''' </summary>
		''' <param name="bindings"> The <code>Bindings</code> of attributes to set. </param>
		''' <param name="scope"> The value of the scope in which the attributes are set.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the value of scope is <code>ENGINE_SCOPE</code> and
		''' the specified <code>Bindings</code> is null. </exception>
		Public Overridable Sub setBindings(ByVal bindings As Bindings, ByVal scope As Integer) Implements ScriptContext.setBindings

			Select Case scope

				Case ENGINE_SCOPE
					If bindings Is Nothing Then Throw New NullPointerException("Engine scope cannot be null.")
					engineScope = bindings
				Case GLOBAL_SCOPE
					globalScope = bindings
				Case Else
					Throw New System.ArgumentException("Invalid scope value.")
			End Select
		End Sub


		''' <summary>
		''' Retrieves the value of the attribute with the given name in
		''' the scope occurring earliest in the search order.  The order
		''' is determined by the numeric value of the scope parameter (lowest
		''' scope values first.)
		''' </summary>
		''' <param name="name"> The name of the the attribute to retrieve. </param>
		''' <returns> The value of the attribute in the lowest scope for
		''' which an attribute with the given name is defined.  Returns
		''' null if no attribute with the name exists in any scope. </returns>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the name is empty. </exception>
		Public Overridable Function getAttribute(ByVal name As String) As Object Implements ScriptContext.getAttribute
			checkName(name)
			If engineScope.containsKey(name) Then
				Return getAttribute(name, ENGINE_SCOPE)
			ElseIf globalScope IsNot Nothing AndAlso globalScope.containsKey(name) Then
				Return getAttribute(name, GLOBAL_SCOPE)
			End If

			Return Nothing
		End Function

		''' <summary>
		''' Gets the value of an attribute in a given scope.
		''' </summary>
		''' <param name="name"> The name of the attribute to retrieve. </param>
		''' <param name="scope"> The scope in which to retrieve the attribute. </param>
		''' <returns> The value of the attribute. Returns <code>null</code> is the name
		''' does not exist in the given scope.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the name is empty or if the value of scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		Public Overridable Function getAttribute(ByVal name As String, ByVal scope As Integer) As Object Implements ScriptContext.getAttribute
			checkName(name)
			Select Case scope

				Case ENGINE_SCOPE
					Return engineScope.get(name)

				Case GLOBAL_SCOPE
					If globalScope IsNot Nothing Then Return globalScope.get(name)
					Return Nothing

				Case Else
					Throw New System.ArgumentException("Illegal scope value.")
			End Select
		End Function

		''' <summary>
		''' Remove an attribute in a given scope.
		''' </summary>
		''' <param name="name"> The name of the attribute to remove </param>
		''' <param name="scope"> The scope in which to remove the attribute
		''' </param>
		''' <returns> The removed value. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the name is empty or if the scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		Public Overridable Function removeAttribute(ByVal name As String, ByVal scope As Integer) As Object Implements ScriptContext.removeAttribute
			checkName(name)
			Select Case scope

				Case ENGINE_SCOPE
					If getBindings(ENGINE_SCOPE) IsNot Nothing Then Return getBindings(ENGINE_SCOPE).remove(name)
					Return Nothing

				Case GLOBAL_SCOPE
					If getBindings(GLOBAL_SCOPE) IsNot Nothing Then Return getBindings(GLOBAL_SCOPE).remove(name)
					Return Nothing

				Case Else
					Throw New System.ArgumentException("Illegal scope value.")
			End Select
		End Function

		''' <summary>
		''' Sets the value of an attribute in a given scope.
		''' </summary>
		''' <param name="name"> The name of the attribute to set </param>
		''' <param name="value"> The value of the attribute </param>
		''' <param name="scope"> The scope in which to set the attribute
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''         if the name is empty or if the scope is invalid. </exception>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		Public Overridable Sub setAttribute(ByVal name As String, ByVal value As Object, ByVal scope As Integer) Implements ScriptContext.setAttribute
			checkName(name)
			Select Case scope

				Case ENGINE_SCOPE
					engineScope.put(name, value)
					Return

				Case GLOBAL_SCOPE
					If globalScope IsNot Nothing Then globalScope.put(name, value)
					Return

				Case Else
					Throw New System.ArgumentException("Illegal scope value.")
			End Select
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getWriter() As Writer 'JavaToDotNetTempPropertyGetwriter
		Public Overridable Property writer As Writer
			Get
				Return writer
			End Get
			Set(ByVal writer As Writer)
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property reader As Reader
			Get
				Return reader
			End Get
			Set(ByVal reader As Reader)
				Me.reader = reader
			End Set
		End Property


			Me.writer = writer
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property errorWriter As Writer
			Get
				Return errorWriter
			End Get
			Set(ByVal writer As Writer)
				Me.errorWriter = writer
			End Set
		End Property


		''' <summary>
		''' Get the lowest scope in which an attribute is defined. </summary>
		''' <param name="name"> Name of the attribute
		''' . </param>
		''' <returns> The lowest scope.  Returns -1 if no attribute with the given
		''' name is defined in any scope. </returns>
		''' <exception cref="NullPointerException"> if name is null. </exception>
		''' <exception cref="IllegalArgumentException"> if name is empty. </exception>
		Public Overridable Function getAttributesScope(ByVal name As String) As Integer Implements ScriptContext.getAttributesScope
			checkName(name)
			If engineScope.containsKey(name) Then
				Return ENGINE_SCOPE
			ElseIf globalScope IsNot Nothing AndAlso globalScope.containsKey(name) Then
				Return GLOBAL_SCOPE
			Else
				Return -1
			End If
		End Function

		''' <summary>
		''' Returns the value of the <code>engineScope</code> field if specified scope is
		''' <code>ENGINE_SCOPE</code>.  Returns the value of the <code>globalScope</code> field if the specified scope is
		''' <code>GLOBAL_SCOPE</code>.
		''' </summary>
		''' <param name="scope"> The specified scope </param>
		''' <returns> The value of either the  <code>engineScope</code> or <code>globalScope</code> field. </returns>
		''' <exception cref="IllegalArgumentException"> if the value of scope is invalid. </exception>
		Public Overridable Function getBindings(ByVal scope As Integer) As Bindings Implements ScriptContext.getBindings
			If scope = ENGINE_SCOPE Then
				Return engineScope
			ElseIf scope = GLOBAL_SCOPE Then
				Return globalScope
			Else
				Throw New System.ArgumentException("Illegal scope value.")
			End If
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property scopes As IList(Of Integer?)
			Get
				Return scopes
			End Get
		End Property

		Private Sub checkName(ByVal name As String)
			Objects.requireNonNull(name)
			If name.Length = 0 Then Throw New System.ArgumentException("name cannot be empty")
		End Sub

		Private Shared scopes As IList(Of Integer?)
		Shared Sub New()
			scopes = New List(Of Integer?)(2)
			scopes.Add(ENGINE_SCOPE)
			scopes.Add(GLOBAL_SCOPE)
			scopes = Collections.unmodifiableList(scopes)
		End Sub
	End Class

End Namespace
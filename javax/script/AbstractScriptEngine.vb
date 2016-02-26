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
	''' Provides a standard implementation for several of the variants of the <code>eval</code>
	''' method.
	''' <br><br>
	''' <code><b>eval(Reader)</b></code><p><code><b>eval(String)</b></code><p>
	''' <code><b>eval(String, Bindings)</b></code><p><code><b>eval(Reader, Bindings)</b></code>
	''' <br><br> are implemented using the abstract methods
	''' <br><br>
	''' <code><b>eval(Reader,ScriptContext)</b></code> or
	''' <code><b>eval(String, ScriptContext)</b></code>
	''' <br><br>
	''' with a <code>SimpleScriptContext</code>.
	''' <br><br>
	''' A <code>SimpleScriptContext</code> is used as the default <code>ScriptContext</code>
	''' of the <code>AbstractScriptEngine</code>..
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public MustInherit Class AbstractScriptEngine
		Implements ScriptEngine

			Public MustOverride ReadOnly Property factory As ScriptEngineFactory Implements ScriptEngine.getFactory
			Public MustOverride WriteOnly Property context Implements ScriptEngine.setContext As ScriptContext
			Public MustOverride Function createBindings() As Bindings Implements ScriptEngine.createBindings
			Public MustOverride Function eval(ByVal reader As java.io.Reader, ByVal n As Bindings) As Object Implements ScriptEngine.eval
			Public MustOverride Function eval(ByVal script As String, ByVal n As Bindings) As Object Implements ScriptEngine.eval
			Public MustOverride Function eval(ByVal reader As java.io.Reader, ByVal context As ScriptContext) As Object Implements ScriptEngine.eval
			Public MustOverride Function eval(ByVal script As String, ByVal context As ScriptContext) As Object Implements ScriptEngine.eval

		''' <summary>
		''' The default <code>ScriptContext</code> of this <code>AbstractScriptEngine</code>.
		''' </summary>

		Protected Friend context As ScriptContext

		''' <summary>
		''' Creates a new instance of AbstractScriptEngine using a <code>SimpleScriptContext</code>
		''' as its default <code>ScriptContext</code>.
		''' </summary>
		Public Sub New()

			context = New SimpleScriptContext

		End Sub

		''' <summary>
		''' Creates a new instance using the specified <code>Bindings</code> as the
		''' <code>ENGINE_SCOPE</code> <code>Bindings</code> in the protected <code>context</code> field.
		''' </summary>
		''' <param name="n"> The specified <code>Bindings</code>. </param>
		''' <exception cref="NullPointerException"> if n is null. </exception>
		Public Sub New(ByVal n As Bindings)

			Me.New()
			If n Is Nothing Then Throw New NullPointerException("n is null")
			context.bindingsngs(n, ScriptContext.ENGINE_SCOPE)
		End Sub

		''' <summary>
		''' Sets the value of the protected <code>context</code> field to the specified
		''' <code>ScriptContext</code>.
		''' </summary>
		''' <param name="ctxt"> The specified <code>ScriptContext</code>. </param>
		''' <exception cref="NullPointerException"> if ctxt is null. </exception>
		Public Overridable Property context Implements ScriptEngine.setContext As ScriptContext
			Set(ByVal ctxt As ScriptContext)
				If ctxt Is Nothing Then Throw New NullPointerException("null context")
				context = ctxt
			End Set
			Get
				Return context
			End Get
		End Property


		''' <summary>
		''' Returns the <code>Bindings</code> with the specified scope value in
		''' the protected <code>context</code> field.
		''' </summary>
		''' <param name="scope"> The specified scope
		''' </param>
		''' <returns> The corresponding <code>Bindings</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if the value of scope is
		''' invalid for the type the protected <code>context</code> field. </exception>
		Public Overridable Function getBindings(ByVal scope As Integer) As Bindings Implements ScriptEngine.getBindings

			If scope = ScriptContext.GLOBAL_SCOPE Then
				Return context.getBindings(ScriptContext.GLOBAL_SCOPE)
			ElseIf scope = ScriptContext.ENGINE_SCOPE Then
				Return context.getBindings(ScriptContext.ENGINE_SCOPE)
			Else
				Throw New System.ArgumentException("Invalid scope value.")
			End If
		End Function

		''' <summary>
		''' Sets the <code>Bindings</code> with the corresponding scope value in the
		''' <code>context</code> field.
		''' </summary>
		''' <param name="bindings"> The specified <code>Bindings</code>. </param>
		''' <param name="scope"> The specified scope.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the value of scope is
		''' invalid for the type the <code>context</code> field. </exception>
		''' <exception cref="NullPointerException"> if the bindings is null and the scope is
		''' <code>ScriptContext.ENGINE_SCOPE</code> </exception>
		Public Overridable Sub setBindings(ByVal bindings As Bindings, ByVal scope As Integer) Implements ScriptEngine.setBindings

			If scope = ScriptContext.GLOBAL_SCOPE Then
				context.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
			ElseIf scope = ScriptContext.ENGINE_SCOPE Then
				context.bindingsngs(bindings, ScriptContext.ENGINE_SCOPE)
			Else
				Throw New System.ArgumentException("Invalid scope value.")
			End If
		End Sub

		''' <summary>
		''' Sets the specified value with the specified key in the <code>ENGINE_SCOPE</code>
		''' <code>Bindings</code> of the protected <code>context</code> field.
		''' </summary>
		''' <param name="key"> The specified key. </param>
		''' <param name="value"> The specified value.
		''' </param>
		''' <exception cref="NullPointerException"> if key is null. </exception>
		''' <exception cref="IllegalArgumentException"> if key is empty. </exception>
		Public Overridable Sub put(ByVal key As String, ByVal value As Object) Implements ScriptEngine.put

			Dim nn As Bindings = getBindings(ScriptContext.ENGINE_SCOPE)
			If nn IsNot Nothing Then nn.put(key, value)

		End Sub

		''' <summary>
		''' Gets the value for the specified key in the <code>ENGINE_SCOPE</code> of the
		''' protected <code>context</code> field.
		''' </summary>
		''' <returns> The value for the specified key.
		''' </returns>
		''' <exception cref="NullPointerException"> if key is null. </exception>
		''' <exception cref="IllegalArgumentException"> if key is empty. </exception>
		Public Overridable Function [get](ByVal key As String) As Object Implements ScriptEngine.get

			Dim nn As Bindings = getBindings(ScriptContext.ENGINE_SCOPE)
			If nn IsNot Nothing Then Return nn.get(key)

			Return Nothing
		End Function


		''' <summary>
		''' <code>eval(Reader, Bindings)</code> calls the abstract
		''' <code>eval(Reader, ScriptContext)</code> method, passing it a <code>ScriptContext</code>
		''' whose Reader, Writers and Bindings for scopes other that <code>ENGINE_SCOPE</code>
		''' are identical to those members of the protected <code>context</code> field.  The specified
		''' <code>Bindings</code> is used instead of the <code>ENGINE_SCOPE</code>
		''' 
		''' <code>Bindings</code> of the <code>context</code> field.
		''' </summary>
		''' <param name="reader"> A <code>Reader</code> containing the source of the script. </param>
		''' <param name="bindings"> A <code>Bindings</code> to use for the <code>ENGINE_SCOPE</code>
		''' while the script executes.
		''' </param>
		''' <returns> The return value from <code>eval(Reader, ScriptContext)</code> </returns>
		''' <exception cref="ScriptException"> if an error occurs in script. </exception>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Function eval(ByVal reader As java.io.Reader, ByVal bindings As Bindings) As Object Implements ScriptEngine.eval

			Dim ctxt As ScriptContext = getScriptContext(bindings)

			Return eval(reader, ctxt)
		End Function


		''' <summary>
		''' Same as <code>eval(Reader, Bindings)</code> except that the abstract
		''' <code>eval(String, ScriptContext)</code> is used.
		''' </summary>
		''' <param name="script"> A <code>String</code> containing the source of the script.
		''' </param>
		''' <param name="bindings"> A <code>Bindings</code> to use as the <code>ENGINE_SCOPE</code>
		''' while the script executes.
		''' </param>
		''' <returns> The return value from <code>eval(String, ScriptContext)</code> </returns>
		''' <exception cref="ScriptException"> if an error occurs in script. </exception>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Function eval(ByVal script As String, ByVal bindings As Bindings) As Object Implements ScriptEngine.eval

			Dim ctxt As ScriptContext = getScriptContext(bindings)

			Return eval(script, ctxt)
		End Function

		''' <summary>
		''' <code>eval(Reader)</code> calls the abstract
		''' <code>eval(Reader, ScriptContext)</code> passing the value of the <code>context</code>
		''' field.
		''' </summary>
		''' <param name="reader"> A <code>Reader</code> containing the source of the script. </param>
		''' <returns> The return value from <code>eval(Reader, ScriptContext)</code> </returns>
		''' <exception cref="ScriptException"> if an error occurs in script. </exception>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Function eval(ByVal reader As java.io.Reader) As Object Implements ScriptEngine.eval


			Return eval(reader, context)
		End Function

		''' <summary>
		''' Same as <code>eval(Reader)</code> except that the abstract
		''' <code>eval(String, ScriptContext)</code> is used.
		''' </summary>
		''' <param name="script"> A <code>String</code> containing the source of the script. </param>
		''' <returns> The return value from <code>eval(String, ScriptContext)</code> </returns>
		''' <exception cref="ScriptException"> if an error occurs in script. </exception>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Function eval(ByVal script As String) As Object Implements ScriptEngine.eval


			Return eval(script, context)
		End Function

		''' <summary>
		''' Returns a <code>SimpleScriptContext</code>.  The <code>SimpleScriptContext</code>:
		''' <br><br>
		''' <ul>
		''' <li>Uses the specified <code>Bindings</code> for its <code>ENGINE_SCOPE</code>
		''' </li>
		''' <li>Uses the <code>Bindings</code> returned by the abstract <code>getGlobalScope</code>
		''' method as its <code>GLOBAL_SCOPE</code>
		''' </li>
		''' <li>Uses the Reader and Writer in the default <code>ScriptContext</code> of this
		''' <code>ScriptEngine</code>
		''' </li>
		''' </ul>
		''' <br><br>
		''' A <code>SimpleScriptContext</code> returned by this method is used to implement eval methods
		''' using the abstract <code>eval(Reader,Bindings)</code> and <code>eval(String,Bindings)</code>
		''' versions.
		''' </summary>
		''' <param name="nn"> Bindings to use for the <code>ENGINE_SCOPE</code> </param>
		''' <returns> The <code>SimpleScriptContext</code> </returns>
		Protected Friend Overridable Function getScriptContext(ByVal nn As Bindings) As ScriptContext

			Dim ctxt As New SimpleScriptContext
			Dim gs As Bindings = getBindings(ScriptContext.GLOBAL_SCOPE)

			If gs IsNot Nothing Then ctxt.bindingsngs(gs, ScriptContext.GLOBAL_SCOPE)

			If nn IsNot Nothing Then
				ctxt.bindingsngs(nn, ScriptContext.ENGINE_SCOPE)
			Else
				Throw New NullPointerException("Engine scope Bindings may not be null.")
			End If

			ctxt.reader = context.reader
			ctxt.writer = context.writer
			ctxt.errorWriter = context.errorWriter

			Return ctxt

		End Function
	End Class

End Namespace
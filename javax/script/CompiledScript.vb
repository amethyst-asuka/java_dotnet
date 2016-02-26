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
	''' Extended by classes that store results of compilations.  State
	''' might be stored in the form of Java classes, Java class files or scripting
	''' language opcodes.  The script may be executed repeatedly
	''' without reparsing.
	''' <br><br>
	''' Each <code>CompiledScript</code> is associated with a <code>ScriptEngine</code> -- A call to an  <code>eval</code>
	''' method of the <code>CompiledScript</code> causes the execution of the script by the
	''' <code>ScriptEngine</code>.  Changes in the state of the <code>ScriptEngine</code> caused by execution
	''' of the <code>CompiledScript</code>  may visible during subsequent executions of scripts by the engine.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public MustInherit Class CompiledScript

		''' <summary>
		''' Executes the program stored in this <code>CompiledScript</code> object.
		''' </summary>
		''' <param name="context"> A <code>ScriptContext</code> that is used in the same way as
		''' the <code>ScriptContext</code> passed to the <code>eval</code> methods of
		''' <code>ScriptEngine</code>.
		''' </param>
		''' <returns> The value returned by the script execution, if any.  Should return <code>null</code>
		''' if no value is returned by the script execution.
		''' </returns>
		''' <exception cref="ScriptException"> if an error occurs. </exception>
		''' <exception cref="NullPointerException"> if context is null. </exception>

		Public MustOverride Function eval(ByVal context As ScriptContext) As Object

		''' <summary>
		''' Executes the program stored in the <code>CompiledScript</code> object using
		''' the supplied <code>Bindings</code> of attributes as the <code>ENGINE_SCOPE</code> of the
		''' associated <code>ScriptEngine</code> during script execution.  If bindings is null,
		''' then the effect of calling this method is same as that of eval(getEngine().getContext()).
		''' <p>.
		''' The <code>GLOBAL_SCOPE</code> <code>Bindings</code>, <code>Reader</code> and <code>Writer</code>
		''' associated with the default <code>ScriptContext</code> of the associated <code>ScriptEngine</code> are used.
		''' </summary>
		''' <param name="bindings"> The bindings of attributes used for the <code>ENGINE_SCOPE</code>.
		''' </param>
		''' <returns> The return value from the script execution
		''' </returns>
		''' <exception cref="ScriptException"> if an error occurs. </exception>
		Public Overridable Function eval(ByVal bindings As Bindings) As Object

			Dim ctxt As ScriptContext = engine.context

			If bindings IsNot Nothing Then
				Dim tempctxt As New SimpleScriptContext
				tempctxt.bindingsngs(bindings, ScriptContext.ENGINE_SCOPE)
				tempctxt.bindingsngs(ctxt.getBindings(ScriptContext.GLOBAL_SCOPE), ScriptContext.GLOBAL_SCOPE)
				tempctxt.writer = ctxt.writer
				tempctxt.reader = ctxt.reader
				tempctxt.errorWriter = ctxt.errorWriter
				ctxt = tempctxt
			End If

			Return eval(ctxt)
		End Function


		''' <summary>
		''' Executes the program stored in the <code>CompiledScript</code> object.  The
		''' default <code>ScriptContext</code> of the associated <code>ScriptEngine</code> is used.
		''' The effect of calling this method is same as that of eval(getEngine().getContext()).
		''' </summary>
		''' <returns> The return value from the script execution
		''' </returns>
		''' <exception cref="ScriptException"> if an error occurs. </exception>
		Public Overridable Function eval() As Object
			Return eval(engine.context)
		End Function

		''' <summary>
		''' Returns the <code>ScriptEngine</code> whose <code>compile</code> method created this <code>CompiledScript</code>.
		''' The <code>CompiledScript</code> will execute in this engine.
		''' </summary>
		''' <returns> The <code>ScriptEngine</code> that created this <code>CompiledScript</code> </returns>
		Public MustOverride ReadOnly Property engine As ScriptEngine

	End Class

End Namespace
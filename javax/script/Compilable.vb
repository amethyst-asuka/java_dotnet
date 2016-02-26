'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' The optional interface implemented by ScriptEngines whose methods compile scripts
	''' to a form that can be executed repeatedly without recompilation.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public Interface Compilable
		''' <summary>
		''' Compiles the script (source represented as a <code>String</code>) for
		''' later execution.
		''' </summary>
		''' <param name="script"> The source of the script, represented as a <code>String</code>.
		''' </param>
		''' <returns> An instance of a subclass of <code>CompiledScript</code> to be executed later using one
		''' of the <code>eval</code> methods of <code>CompiledScript</code>.
		''' </returns>
		''' <exception cref="ScriptException"> if compilation fails. </exception>
		''' <exception cref="NullPointerException"> if the argument is null.
		'''  </exception>

		Function compile(ByVal script As String) As CompiledScript

		''' <summary>
		''' Compiles the script (source read from <code>Reader</code>) for
		''' later execution.  Functionality is identical to
		''' <code>compile(String)</code> other than the way in which the source is
		''' passed.
		''' </summary>
		''' <param name="script"> The reader from which the script source is obtained.
		''' </param>
		''' <returns> An instance of a subclass of <code>CompiledScript</code> to be executed
		''' later using one of its <code>eval</code> methods of <code>CompiledScript</code>.
		''' </returns>
		''' <exception cref="ScriptException"> if compilation fails. </exception>
		''' <exception cref="NullPointerException"> if argument is null. </exception>
		Function compile(ByVal script As java.io.Reader) As CompiledScript
	End Interface

End Namespace
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
	''' Common interface for tools that can be invoked from a program.
	''' A tool is traditionally a command line program such as a compiler.
	''' The set of tools available with a platform is defined by the
	''' vendor.
	''' 
	''' <p>Tools can be located using {@link
	''' java.util.ServiceLoader#load(Class)}.
	''' 
	''' @author Neal M Gafter
	''' @author Peter von der Ah&eacute;
	''' @author Jonathan Gibbons
	''' @since 1.6
	''' </summary>
	Public Interface Tool

		''' <summary>
		''' Run the tool with the given I/O channels and arguments. By
		''' convention a tool returns 0 for success and nonzero for errors.
		''' Any diagnostics generated will be written to either {@code out}
		''' or {@code err} in some unspecified format.
		''' </summary>
		''' <param name="in"> "standard" input; use System.in if null </param>
		''' <param name="out"> "standard" output; use System.out if null </param>
		''' <param name="err"> "standard" error; use System.err if null </param>
		''' <param name="arguments"> arguments to pass to the tool </param>
		''' <returns> 0 for success; nonzero otherwise </returns>
		''' <exception cref="NullPointerException"> if the array of arguments contains
		''' any {@code null} elements. </exception>
		Function run(ByVal [in] As java.io.InputStream, ByVal out As java.io.OutputStream, ByVal err As java.io.OutputStream, ParamArray ByVal arguments As String()) As Integer

		''' <summary>
		''' Gets the source versions of the Java&trade; programming language
		''' supported by this tool. </summary>
		''' <returns> a set of supported source versions </returns>
		ReadOnly Property sourceVersions As java.util.Set(Of javax.lang.model.SourceVersion)

	End Interface

End Namespace
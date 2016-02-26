Imports System

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
Namespace javax.tools


	''' <summary>
	''' Interface to invoke Java&trade; programming language documentation tools from
	''' programs.
	''' </summary>
	Public Interface DocumentationTool
		Inherits Tool, OptionChecker

		''' <summary>
		''' Creates a future for a documentation task with the given
		''' components and arguments.  The task might not have
		''' completed as described in the DocumentationTask interface.
		''' 
		''' <p>If a file manager is provided, it must be able to handle all
		''' locations defined in <seealso cref="DocumentationTool.Location"/>,
		''' as well as
		''' <seealso cref="StandardLocation#SOURCE_PATH"/>,
		''' <seealso cref="StandardLocation#CLASS_PATH"/>, and
		''' <seealso cref="StandardLocation#PLATFORM_CLASS_PATH"/>.
		''' </summary>
		''' <param name="out"> a Writer for additional output from the tool;
		''' use {@code System.err} if {@code null}
		''' </param>
		''' <param name="fileManager"> a file manager; if {@code null} use the
		''' tool's standard filemanager
		''' </param>
		''' <param name="diagnosticListener"> a diagnostic listener; if {@code null}
		''' use the tool's default method for reporting diagnostics
		''' </param>
		''' <param name="docletClass"> a class providing the necessary methods required
		''' of a doclet
		''' </param>
		''' <param name="options"> documentation tool options and doclet options,
		''' {@code null} means no options
		''' </param>
		''' <param name="compilationUnits"> the compilation units to compile, {@code
		''' null} means no compilation units
		''' </param>
		''' <returns> an object representing the compilation
		''' </returns>
		''' <exception cref="RuntimeException"> if an unrecoverable error
		''' occurred in a user supplied component.  The
		''' <seealso cref="Throwable#getCause() cause"/> will be the error in
		''' user code.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if any of the given
		''' compilation units are of other kind than
		''' <seealso cref="JavaFileObject.Kind#SOURCE source"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Function getTask(Of T1, T2 As JavaFileObject)(ByVal out As java.io.Writer, ByVal fileManager As JavaFileManager, ByVal diagnosticListener As DiagnosticListener(Of T1), ByVal docletClass As Type, ByVal options As IEnumerable(Of String), ByVal compilationUnits As IEnumerable(Of T2)) As DocumentationTask

		''' <summary>
		''' Gets a new instance of the standard file manager implementation
		''' for this tool.  The file manager will use the given diagnostic
		''' listener for producing any non-fatal diagnostics.  Fatal errors
		''' will be signaled with the appropriate exceptions.
		''' 
		''' <p>The standard file manager will be automatically reopened if
		''' it is accessed after calls to {@code flush} or {@code close}.
		''' The standard file manager must be usable with other tools.
		''' </summary>
		''' <param name="diagnosticListener"> a diagnostic listener for non-fatal
		''' diagnostics; if {@code null} use the compiler's default method
		''' for reporting diagnostics
		''' </param>
		''' <param name="locale"> the locale to apply when formatting diagnostics;
		''' {@code null} means the <seealso cref="Locale#getDefault() default locale"/>.
		''' </param>
		''' <param name="charset"> the character set used for decoding bytes; if
		''' {@code null} use the platform default
		''' </param>
		''' <returns> the standard file manager </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Function getStandardFileManager(Of T1)(ByVal diagnosticListener As DiagnosticListener(Of T1), ByVal locale As java.util.Locale, ByVal charset As java.nio.charset.Charset) As StandardJavaFileManager

		''' <summary>
		''' Interface representing a future for a documentation task.  The
		''' task has not yet started.  To start the task, call
		''' the <seealso cref="#call call"/> method.
		''' 
		''' <p>Before calling the call method, additional aspects of the
		''' task can be configured, for example, by calling the
		''' <seealso cref="#setLocale setLocale"/> method.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		interface DocumentationTask extends java.util.concurrent.Callable(Of java.lang.Boolean)
	'	{
	'		''' <summary>
	'		''' Set the locale to be applied when formatting diagnostics and
	'		''' other localized data.
	'		''' </summary>
	'		''' <param name="locale"> the locale to apply; {@code null} means apply no
	'		''' locale </param>
	'		''' <exception cref="IllegalStateException"> if the task has started </exception>
	'		void setLocale(Locale locale);
	'
	'		''' <summary>
	'		''' Performs this documentation task.  The task may only
	'		''' be performed once.  Subsequent calls to this method throw
	'		''' IllegalStateException.
	'		''' </summary>
	'		''' <returns> true if and only all the files were processed without errors;
	'		''' false otherwise
	'		''' </returns>
	'		''' <exception cref="RuntimeException"> if an unrecoverable error occurred
	'		''' in a user-supplied component.  The
	'		''' <seealso cref="Throwable#getCause() cause"/> will be the error
	'		''' in user code.
	'		''' </exception>
	'		''' <exception cref="IllegalStateException"> if called more than once </exception>
	'		java.lang.Boolean call();
	'	}

		''' <summary>
		''' Locations specific to <seealso cref="DocumentationTool"/>.
		''' </summary>
		''' <seealso cref= StandardLocation </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		enum Location implements JavaFileManager.Location
	'	{
	'		''' <summary>
	'		''' Location of new documentation files.
	'		''' </summary>
	'		DOCUMENTATION_OUTPUT,
	'
	'		''' <summary>
	'		''' Location to search for doclets.
	'		''' </summary>
	'		DOCLET_PATH,
	'
	'		''' <summary>
	'		''' Location to search for taglets.
	'		''' </summary>
	'		TAGLET_PATH;
	'
	'		public String getName() { Return name();
	'	}

			ReadOnly Property outputLocation As Boolean
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				switch (Me)
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					case DOCUMENTATION_OUTPUT:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'						Return True;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					default:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'						Return False;
	End Interface

	}

End Namespace
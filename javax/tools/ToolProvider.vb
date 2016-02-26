Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading

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
	''' Provides methods for locating tool providers, for example,
	''' providers of compilers.  This class complements the
	''' functionality of <seealso cref="java.util.ServiceLoader"/>.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Class ToolProvider

		Private Const propertyName As String = "sun.tools.ToolProvider"
		Private Const loggerName As String = "javax.tools"

	'    
	'     * Define the system property "sun.tools.ToolProvider" to enable
	'     * debugging:
	'     *
	'     *     java ... -Dsun.tools.ToolProvider ...
	'     
		Friend Shared Function trace(Of T)(ByVal level As java.util.logging.Level, ByVal reason As Object) As T
			' NOTE: do not make this method private as it affects stack traces
			Try
				If System.getProperty(propertyName) IsNot Nothing Then
					Dim st As StackTraceElement() = Thread.CurrentThread.stackTrace
					Dim method As String = "???"
					Dim cls As String = GetType(ToolProvider).name
					If st.Length > 2 Then
						Dim frame As StackTraceElement = st(2)
						method = String.format(CType(Nothing, java.util.Locale), "%s(%s:%s)", frame.methodName, frame.fileName, frame.lineNumber)
						cls = frame.className
					End If
					Dim logger As java.util.logging.Logger = java.util.logging.Logger.getLogger(loggerName)
					If TypeOf reason Is Exception Then
						logger.logp(level, cls, method, reason.GetType().name, CType(reason, Exception))
					Else
						logger.logp(level, cls, method, Convert.ToString(reason))
					End If
				End If
			Catch ex As SecurityException
				System.err.format(CType(Nothing, java.util.Locale), "%s: %s; %s%n", GetType(ToolProvider).name, reason, ex.localizedMessage)
			End Try
			Return Nothing
		End Function

		Private Const defaultJavaCompilerName As String = "com.sun.tools.javac.api.JavacTool"

		''' <summary>
		''' Gets the Java&trade; programming language compiler provided
		''' with this platform. </summary>
		''' <returns> the compiler provided with this platform or
		''' {@code null} if no compiler is provided </returns>
		Public Property Shared systemJavaCompiler As JavaCompiler
			Get
				Return instance().getSystemTool(GetType(JavaCompiler), defaultJavaCompilerName)
			End Get
		End Property

		Private Const defaultDocumentationToolName As String = "com.sun.tools.javadoc.api.JavadocTool"

		''' <summary>
		''' Gets the Java&trade; programming language documentation tool provided
		''' with this platform. </summary>
		''' <returns> the documentation tool provided with this platform or
		''' {@code null} if no documentation tool is provided </returns>
		Public Property Shared systemDocumentationTool As DocumentationTool
			Get
				Return instance().getSystemTool(GetType(DocumentationTool), defaultDocumentationToolName)
			End Get
		End Property

		''' <summary>
		''' Returns the class loader for tools provided with this platform.
		''' This does not include user-installed tools.  Use the
		''' <seealso cref="java.util.ServiceLoader service provider mechanism"/>
		''' for locating user installed tools.
		''' </summary>
		''' <returns> the class loader for tools provided with this platform
		''' or {@code null} if no tools are provided </returns>
		Public Property Shared systemToolClassLoader As ClassLoader
			Get
				Try
					Dim c As Type = instance().getSystemToolClass(GetType(JavaCompiler), defaultJavaCompilerName)
					Return c.classLoader
				Catch e As Exception
					Return trace(WARNING, e)
				End Try
			End Get
		End Property


		Private Shared ___instance As ToolProvider

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function instance() As ToolProvider
			If ___instance Is Nothing Then ___instance = New ToolProvider
			Return ___instance
		End Function

		' Cache for tool classes.
		' Use weak references to avoid keeping classes around unnecessarily
		Private toolClasses As IDictionary(Of String, Reference(Of Type)) = New Dictionary(Of String, Reference(Of Type))

		' Cache for tool classloader.
		' Use a weak reference to avoid keeping it around unnecessarily
		Private refToolClassLoader As Reference(Of ClassLoader) = Nothing


		Private Sub New()
		End Sub

		Private Function getSystemTool(Of T)(ByVal clazz As Type, ByVal name As String) As T
			Dim c As Type = getSystemToolClass(clazz, name)
			Try
				Return c.asSubclass(clazz).newInstance()
			Catch e As Exception
				trace(WARNING, e)
				Return Nothing
			End Try
		End Function

		Private Function getSystemToolClass(Of T)(ByVal clazz As Type, ByVal name As String) As Type
			Dim refClass As Reference(Of Type) = toolClasses(name)
			Dim c As Type = (If(refClass Is Nothing, Nothing, refClass.get()))
			If c Is Nothing Then
				Try
					c = findSystemToolClass(name)
				Catch e As Exception
					Return trace(WARNING, e)
				End Try
				toolClasses(name) = New WeakReference(Of Type)(c)
			End If
			Return c.asSubclass(clazz)
		End Function

		Private Shared ReadOnly defaultToolsLocation As String() = { "lib", "tools.jar" }

		Private Function findSystemToolClass(ByVal toolClassName As String) As Type
			' try loading class directly, in case tool is on the bootclasspath
			Try
				Return Type.GetType(toolClassName, False, Nothing)
			Catch e As ClassNotFoundException
				trace(FINE, e)

				' if tool not on bootclasspath, look in default tools location (tools.jar)
				Dim cl As ClassLoader = (If(refToolClassLoader Is Nothing, Nothing, refToolClassLoader.get()))
				If cl Is Nothing Then
					Dim file As New File(System.getProperty("java.home"))
					If file.name.ToUpper() = "jre".ToUpper() Then file = file.parentFile
					For Each name As String In defaultToolsLocation
						file = New File(file, name)
					Next name

					' if tools not found, no point in trying a URLClassLoader
					' so rethrow the original exception.
					If Not file.exists() Then Throw e

					Dim urls As java.net.URL() = { file.toURI().toURL() }
					trace(FINE, urls(0).ToString())

					cl = java.net.URLClassLoader.newInstance(urls)
					refToolClassLoader = New WeakReference(Of ClassLoader)(cl)
				End If

				Return Type.GetType(toolClassName, False, cl)
			End Try
		End Function
	End Class

End Namespace
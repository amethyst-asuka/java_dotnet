Imports System
Imports System.Collections.Generic
Imports System.Threading

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
	''' The <code>ScriptEngineManager</code> implements a discovery and instantiation
	''' mechanism for <code>ScriptEngine</code> classes and also maintains a
	''' collection of key/value pairs storing state shared by all engines created
	''' by the Manager. This class uses the <a href="../../../technotes/guides/jar/jar.html#Service%20Provider">service provider</a> mechanism to enumerate all the
	''' implementations of <code>ScriptEngineFactory</code>. <br><br>
	''' The <code>ScriptEngineManager</code> provides a method to return a list of all these factories
	''' as well as utility methods which look up factories on the basis of language name, file extension
	''' and mime type.
	''' <p>
	''' The <code>Bindings</code> of key/value pairs, referred to as the "Global Scope"  maintained
	''' by the manager is available to all instances of <code>ScriptEngine</code> created
	''' by the <code>ScriptEngineManager</code>.  The values in the <code>Bindings</code> are
	''' generally exposed in all scripts.
	''' 
	''' @author Mike Grogan
	''' @author A. Sundararajan
	''' @since 1.6
	''' </summary>
	Public Class ScriptEngineManager
		Private Const DEBUG As Boolean = False
		''' <summary>
		''' The effect of calling this constructor is the same as calling
		''' <code>ScriptEngineManager(Thread.currentThread().getContextClassLoader())</code>.
		''' </summary>
		''' <seealso cref= java.lang.Thread#getContextClassLoader </seealso>
		Public Sub New()
			Dim ctxtLoader As ClassLoader = Thread.CurrentThread.contextClassLoader
			init(ctxtLoader)
		End Sub

		''' <summary>
		''' This constructor loads the implementations of
		''' <code>ScriptEngineFactory</code> visible to the given
		''' <code>ClassLoader</code> using the <a href="../../../technotes/guides/jar/jar.html#Service%20Provider">service provider</a> mechanism.<br><br>
		''' If loader is <code>null</code>, the script engine factories that are
		''' bundled with the platform and that are in the usual extension
		''' directories (installed extensions) are loaded. <br><br>
		''' </summary>
		''' <param name="loader"> ClassLoader used to discover script engine factories. </param>
		Public Sub New(ByVal loader As ClassLoader)
			init(loader)
		End Sub

		Private Sub init(ByVal loader As ClassLoader)
			globalScope = New SimpleBindings
			engineSpis = New HashSet(Of ScriptEngineFactory)
			nameAssociations = New Dictionary(Of String, ScriptEngineFactory)
			extensionAssociations = New Dictionary(Of String, ScriptEngineFactory)
			mimeTypeAssociations = New Dictionary(Of String, ScriptEngineFactory)
			initEngines(loader)
		End Sub

		Private Function getServiceLoader(ByVal loader As ClassLoader) As java.util.ServiceLoader(Of ScriptEngineFactory)
			If loader IsNot Nothing Then
				Return java.util.ServiceLoader.load(GetType(ScriptEngineFactory), loader)
			Else
				Return java.util.ServiceLoader.loadInstalled(GetType(ScriptEngineFactory))
			End If
		End Function

		Private Sub initEngines(ByVal loader As ClassLoader)
			Dim itr As IEnumerator(Of ScriptEngineFactory) = Nothing
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.util.ServiceLoader<ScriptEngineFactory> sl = AccessController.doPrivileged(New PrivilegedAction<java.util.ServiceLoader<ScriptEngineFactory>>()
	'			{
	'					@Override public ServiceLoader<ScriptEngineFactory> run()
	'					{
	'						Return getServiceLoader(loader);
	'					}
	'				});

				itr = sl.GetEnumerator()
			Catch err As java.util.ServiceConfigurationError
				Console.Error.WriteLine("Can't find ScriptEngineFactory providers: " & err.message)
				If DEBUG Then err.printStackTrace()
				' do not throw any exception here. user may want to
				' manage his/her own factories using this manager
				' by explicit registratation (by registerXXX) methods.
				Return
			End Try

			Try
				Do While itr.MoveNext()
					Try
						Dim fact As ScriptEngineFactory = itr.Current
						engineSpis.Add(fact)
					Catch err As java.util.ServiceConfigurationError
						Console.Error.WriteLine("ScriptEngineManager providers.next(): " & err.message)
						If DEBUG Then err.printStackTrace()
						' one factory failed, but check other factories...
						Continue Do
					End Try
				Loop
			Catch err As java.util.ServiceConfigurationError
				Console.Error.WriteLine("ScriptEngineManager providers.hasNext(): " & err.message)
				If DEBUG Then err.printStackTrace()
				' do not throw any exception here. user may want to
				' manage his/her own factories using this manager
				' by explicit registratation (by registerXXX) methods.
				Return
			End Try
		End Sub

		''' <summary>
		''' <code>setBindings</code> stores the specified <code>Bindings</code>
		''' in the <code>globalScope</code> field. ScriptEngineManager sets this
		''' <code>Bindings</code> as global bindings for <code>ScriptEngine</code>
		''' objects created by it.
		''' </summary>
		''' <param name="bindings"> The specified <code>Bindings</code> </param>
		''' <exception cref="IllegalArgumentException"> if bindings is null. </exception>
		Public Overridable Property bindings As Bindings
			Set(ByVal bindings As Bindings)
				If bindings Is Nothing Then Throw New System.ArgumentException("Global scope cannot be null.")
    
				globalScope = bindings
			End Set
			Get
				Return globalScope
			End Get
		End Property


		''' <summary>
		''' Sets the specified key/value pair in the Global Scope. </summary>
		''' <param name="key"> Key to set </param>
		''' <param name="value"> Value to set. </param>
		''' <exception cref="NullPointerException"> if key is null. </exception>
		''' <exception cref="IllegalArgumentException"> if key is empty string. </exception>
		Public Overridable Sub put(ByVal key As String, ByVal value As Object)
			globalScope.put(key, value)
		End Sub

		''' <summary>
		''' Gets the value for the specified key in the Global Scope </summary>
		''' <param name="key"> The key whose value is to be returned. </param>
		''' <returns> The value for the specified key. </returns>
		Public Overridable Function [get](ByVal key As String) As Object
			Return globalScope.get(key)
		End Function

		''' <summary>
		''' Looks up and creates a <code>ScriptEngine</code> for a given  name.
		''' The algorithm first searches for a <code>ScriptEngineFactory</code> that has been
		''' registered as a handler for the specified name using the <code>registerEngineName</code>
		''' method.
		''' <br><br> If one is not found, it searches the set of <code>ScriptEngineFactory</code> instances
		''' stored by the constructor for one with the specified name.  If a <code>ScriptEngineFactory</code>
		''' is found by either method, it is used to create instance of <code>ScriptEngine</code>. </summary>
		''' <param name="shortName"> The short name of the <code>ScriptEngine</code> implementation.
		''' returned by the <code>getNames</code> method of its <code>ScriptEngineFactory</code>. </param>
		''' <returns> A <code>ScriptEngine</code> created by the factory located in the search.  Returns null
		''' if no such factory was found.  The <code>ScriptEngineManager</code> sets its own <code>globalScope</code>
		''' <code>Bindings</code> as the <code>GLOBAL_SCOPE</code> <code>Bindings</code> of the newly
		''' created <code>ScriptEngine</code>. </returns>
		''' <exception cref="NullPointerException"> if shortName is null. </exception>
		Public Overridable Function getEngineByName(ByVal shortName As String) As ScriptEngine
			If shortName Is Nothing Then Throw New NullPointerException
			'look for registered name first
			Dim obj As Object
			obj = nameAssociations(shortName)
			If Nothing IsNot obj Then
				Dim spi As ScriptEngineFactory = CType(obj, ScriptEngineFactory)
				Try
					Dim engine As ScriptEngine = spi.scriptEngine
					engine.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
					Return engine
				Catch exp As Exception
					If DEBUG Then
						Console.WriteLine(exp.ToString())
						Console.Write(exp.StackTrace)
					End If
				End Try
			End If

			For Each spi As ScriptEngineFactory In engineSpis
				Dim names As IList(Of String) = Nothing
				Try
					names = spi.names
				Catch exp As Exception
					If DEBUG Then
						Console.WriteLine(exp.ToString())
						Console.Write(exp.StackTrace)
					End If
				End Try

				If names IsNot Nothing Then
					For Each name As String In names
						If shortName.Equals(name) Then
							Try
								Dim engine As ScriptEngine = spi.scriptEngine
								engine.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
								Return engine
							Catch exp As Exception
								If DEBUG Then
									Console.WriteLine(exp.ToString())
									Console.Write(exp.StackTrace)
								End If
							End Try
						End If
					Next name
				End If
			Next spi

			Return Nothing
		End Function

		''' <summary>
		''' Look up and create a <code>ScriptEngine</code> for a given extension.  The algorithm
		''' used by <code>getEngineByName</code> is used except that the search starts
		''' by looking for a <code>ScriptEngineFactory</code> registered to handle the
		''' given extension using <code>registerEngineExtension</code>. </summary>
		''' <param name="extension"> The given extension </param>
		''' <returns> The engine to handle scripts with this extension.  Returns <code>null</code>
		''' if not found. </returns>
		''' <exception cref="NullPointerException"> if extension is null. </exception>
		Public Overridable Function getEngineByExtension(ByVal extension As String) As ScriptEngine
			If extension Is Nothing Then Throw New NullPointerException
			'look for registered extension first
			Dim obj As Object
			obj = extensionAssociations(extension)
			If Nothing IsNot obj Then
				Dim spi As ScriptEngineFactory = CType(obj, ScriptEngineFactory)
				Try
					Dim engine As ScriptEngine = spi.scriptEngine
					engine.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
					Return engine
				Catch exp As Exception
					If DEBUG Then
						Console.WriteLine(exp.ToString())
						Console.Write(exp.StackTrace)
					End If
				End Try
			End If

			For Each spi As ScriptEngineFactory In engineSpis
				Dim exts As IList(Of String) = Nothing
				Try
					exts = spi.extensions
				Catch exp As Exception
					If DEBUG Then
						Console.WriteLine(exp.ToString())
						Console.Write(exp.StackTrace)
					End If
				End Try
				If exts Is Nothing Then Continue For
				For Each ext As String In exts
					If extension.Equals(ext) Then
						Try
							Dim engine As ScriptEngine = spi.scriptEngine
							engine.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
							Return engine
						Catch exp As Exception
							If DEBUG Then
								Console.WriteLine(exp.ToString())
								Console.Write(exp.StackTrace)
							End If
						End Try
					End If
				Next ext
			Next spi
			Return Nothing
		End Function

		''' <summary>
		''' Look up and create a <code>ScriptEngine</code> for a given mime type.  The algorithm
		''' used by <code>getEngineByName</code> is used except that the search starts
		''' by looking for a <code>ScriptEngineFactory</code> registered to handle the
		''' given mime type using <code>registerEngineMimeType</code>. </summary>
		''' <param name="mimeType"> The given mime type </param>
		''' <returns> The engine to handle scripts with this mime type.  Returns <code>null</code>
		''' if not found. </returns>
		''' <exception cref="NullPointerException"> if mimeType is null. </exception>
		Public Overridable Function getEngineByMimeType(ByVal mimeType As String) As ScriptEngine
			If mimeType Is Nothing Then Throw New NullPointerException
			'look for registered types first
			Dim obj As Object
			obj = mimeTypeAssociations(mimeType)
			If Nothing IsNot obj Then
				Dim spi As ScriptEngineFactory = CType(obj, ScriptEngineFactory)
				Try
					Dim engine As ScriptEngine = spi.scriptEngine
					engine.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
					Return engine
				Catch exp As Exception
					If DEBUG Then
						Console.WriteLine(exp.ToString())
						Console.Write(exp.StackTrace)
					End If
				End Try
			End If

			For Each spi As ScriptEngineFactory In engineSpis
				Dim types As IList(Of String) = Nothing
				Try
					types = spi.mimeTypes
				Catch exp As Exception
					If DEBUG Then
						Console.WriteLine(exp.ToString())
						Console.Write(exp.StackTrace)
					End If
				End Try
				If types Is Nothing Then Continue For
				For Each type As String In types
					If mimeType.Equals(type) Then
						Try
							Dim engine As ScriptEngine = spi.scriptEngine
							engine.bindingsngs(bindings, ScriptContext.GLOBAL_SCOPE)
							Return engine
						Catch exp As Exception
							If DEBUG Then
								Console.WriteLine(exp.ToString())
								Console.Write(exp.StackTrace)
							End If
						End Try
					End If
				Next type
			Next spi
			Return Nothing
		End Function

		''' <summary>
		''' Returns a list whose elements are instances of all the <code>ScriptEngineFactory</code> classes
		''' found by the discovery mechanism. </summary>
		''' <returns> List of all discovered <code>ScriptEngineFactory</code>s. </returns>
		Public Overridable Property engineFactories As IList(Of ScriptEngineFactory)
			Get
				Dim res As IList(Of ScriptEngineFactory) = New List(Of ScriptEngineFactory)(engineSpis.Count)
				For Each spi As ScriptEngineFactory In engineSpis
					res.Add(spi)
				Next spi
				Return Collections.unmodifiableList(res)
			End Get
		End Property

		''' <summary>
		''' Registers a <code>ScriptEngineFactory</code> to handle a language
		''' name.  Overrides any such association found using the Discovery mechanism. </summary>
		''' <param name="name"> The name to be associated with the <code>ScriptEngineFactory</code>. </param>
		''' <param name="factory"> The class to associate with the given name. </param>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Sub registerEngineName(ByVal name As String, ByVal factory As ScriptEngineFactory)
			If name Is Nothing OrElse factory Is Nothing Then Throw New NullPointerException
			nameAssociations(name) = factory
		End Sub

		''' <summary>
		''' Registers a <code>ScriptEngineFactory</code> to handle a mime type.
		''' Overrides any such association found using the Discovery mechanism.
		''' </summary>
		''' <param name="type"> The mime type  to be associated with the
		''' <code>ScriptEngineFactory</code>.
		''' </param>
		''' <param name="factory"> The class to associate with the given mime type. </param>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Sub registerEngineMimeType(ByVal type As String, ByVal factory As ScriptEngineFactory)
			If type Is Nothing OrElse factory Is Nothing Then Throw New NullPointerException
			mimeTypeAssociations(type) = factory
		End Sub

		''' <summary>
		''' Registers a <code>ScriptEngineFactory</code> to handle an extension.
		''' Overrides any such association found using the Discovery mechanism.
		''' </summary>
		''' <param name="extension"> The extension type  to be associated with the
		''' <code>ScriptEngineFactory</code>. </param>
		''' <param name="factory"> The class to associate with the given extension. </param>
		''' <exception cref="NullPointerException"> if any of the parameters is null. </exception>
		Public Overridable Sub registerEngineExtension(ByVal extension As String, ByVal factory As ScriptEngineFactory)
			If extension Is Nothing OrElse factory Is Nothing Then Throw New NullPointerException
			extensionAssociations(extension) = factory
		End Sub

		''' <summary>
		''' Set of script engine factories discovered. </summary>
		Private engineSpis As HashSet(Of ScriptEngineFactory)

		''' <summary>
		''' Map of engine name to script engine factory. </summary>
		Private nameAssociations As Dictionary(Of String, ScriptEngineFactory)

		''' <summary>
		''' Map of script file extension to script engine factory. </summary>
		Private extensionAssociations As Dictionary(Of String, ScriptEngineFactory)

		''' <summary>
		''' Map of script script MIME type to script engine factory. </summary>
		Private mimeTypeAssociations As Dictionary(Of String, ScriptEngineFactory)

		''' <summary>
		''' Global bindings associated with script engines created by this manager. </summary>
		Private globalScope As Bindings
	End Class

End Namespace
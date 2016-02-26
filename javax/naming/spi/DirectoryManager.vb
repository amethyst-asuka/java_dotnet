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
	''' This class contains methods for supporting <tt>DirContext</tt>
	''' implementations.
	''' <p>
	''' This class is an extension of <tt>NamingManager</tt>.  It contains methods
	''' for use by service providers for accessing object factories and
	''' state factories, and for getting continuation contexts for
	''' supporting federation.
	''' <p>
	''' <tt>DirectoryManager</tt> is safe for concurrent access by multiple threads.
	''' <p>
	''' Except as otherwise noted,
	''' a <tt>Name</tt>, <tt>Attributes</tt>, or environment parameter
	''' passed to any method is owned by the caller.
	''' The implementation will not modify the object or keep a reference
	''' to it, although it may keep a reference to a clone or copy.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= DirObjectFactory </seealso>
	''' <seealso cref= DirStateFactory
	''' @since 1.3 </seealso>

	Public Class DirectoryManager
		Inherits NamingManager

	'    
	'     * Disallow anyone from creating one of these.
	'     
		Friend Sub New()
		End Sub

		''' <summary>
		''' Creates a context in which to continue a <tt>DirContext</tt> operation.
		''' Operates just like <tt>NamingManager.getContinuationContext()</tt>,
		''' only the continuation context returned is a <tt>DirContext</tt>.
		''' </summary>
		''' <param name="cpe">
		'''         The non-null exception that triggered this continuation. </param>
		''' <returns> A non-null <tt>DirContext</tt> object for continuing the operation. </returns>
		''' <exception cref="NamingException"> If a naming exception occurred.
		''' </exception>
		''' <seealso cref= NamingManager#getContinuationContext(CannotProceedException) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getContinuationDirContext(ByVal cpe As javax.naming.CannotProceedException) As javax.naming.directory.DirContext

			Dim env As Dictionary(Of Object, Object) = CType(cpe.environment, Dictionary(Of Object, Object))
			If env Is Nothing Then
				env = New Dictionary(Of )(7)
			Else
				' Make a (shallow) copy of the environment.
				env = CType(env.clone(), Dictionary(Of Object, Object))
			End If
			env(DirectoryManager.CPE) = cpe

			Return (New ContinuationDirContext(cpe, env))
		End Function

		''' <summary>
		''' Creates an instance of an object for the specified object,
		''' attributes, and environment.
		''' <p>
		''' This method is the same as <tt>NamingManager.getObjectInstance</tt>
		''' except for the following differences:
		''' <ul>
		''' <li>
		''' It accepts an <tt>Attributes</tt> parameter that contains attributes
		''' associated with the object. The <tt>DirObjectFactory</tt> might use these
		''' attributes to save having to look them up from the directory.
		''' <li>
		''' The object factories tried must implement either
		''' <tt>ObjectFactory</tt> or <tt>DirObjectFactory</tt>.
		''' If it implements <tt>DirObjectFactory</tt>,
		''' <tt>DirObjectFactory.getObjectInstance()</tt> is used, otherwise,
		''' <tt>ObjectFactory.getObjectInstance()</tt> is used.
		''' </ul>
		''' Service providers that implement the <tt>DirContext</tt> interface
		''' should use this method, not <tt>NamingManager.getObjectInstance()</tt>.
		''' <p>
		''' </summary>
		''' <param name="refInfo"> The possibly null object for which to create an object. </param>
		''' <param name="name"> The name of this object relative to <code>nameCtx</code>.
		'''         Specifying a name is optional; if it is
		'''         omitted, <code>name</code> should be null. </param>
		''' <param name="nameCtx"> The context relative to which the <code>name</code>
		'''         parameter is specified.  If null, <code>name</code> is
		'''         relative to the default initial context. </param>
		''' <param name="environment"> The possibly null environment to
		'''         be used in the creation of the object factory and the object. </param>
		''' <param name="attrs"> The possibly null attributes associated with refInfo.
		'''         This might not be the complete set of attributes for refInfo;
		'''         you might be able to read more attributes from the directory. </param>
		''' <returns> An object created using <code>refInfo</code> and <tt>attrs</tt>; or
		'''         <code>refInfo</code> if an object cannot be created by
		'''         a factory. </returns>
		''' <exception cref="NamingException"> If a naming exception was encountered
		'''         while attempting to get a URL context, or if one of the
		'''         factories accessed throws a NamingException. </exception>
		''' <exception cref="Exception"> If one of the factories accessed throws an
		'''         exception, or if an error was encountered while loading
		'''         and instantiating the factory and object classes.
		'''         A factory should only throw an exception if it does not want
		'''         other factories to be used in an attempt to create an object.
		'''         See <tt>DirObjectFactory.getObjectInstance()</tt>. </exception>
		''' <seealso cref= NamingManager#getURLContext </seealso>
		''' <seealso cref= DirObjectFactory </seealso>
		''' <seealso cref= DirObjectFactory#getObjectInstance
		''' @since 1.3 </seealso>
		Public Shared Function getObjectInstance(Of T1)(ByVal refInfo As Object, ByVal name As javax.naming.Name, ByVal nameCtx As javax.naming.Context, ByVal environment As Dictionary(Of T1), ByVal attrs As javax.naming.directory.Attributes) As Object

				Dim factory As ObjectFactory

				Dim builder As ObjectFactoryBuilder = objectFactoryBuilder
				If builder IsNot Nothing Then
					' builder must return non-null factory
					factory = builder.createObjectFactory(refInfo, environment)
					If TypeOf factory Is DirObjectFactory Then
						Return CType(factory, DirObjectFactory).getObjectInstance(refInfo, name, nameCtx, environment, attrs)
					Else
						Return factory.getObjectInstance(refInfo, name, nameCtx, environment)
					End If
				End If

				' use reference if possible
				Dim ref As javax.naming.Reference = Nothing
				If TypeOf refInfo Is javax.naming.Reference Then
					ref = CType(refInfo, javax.naming.Reference)
				ElseIf TypeOf refInfo Is javax.naming.Referenceable Then
					ref = CType(refInfo, javax.naming.Referenceable).reference
				End If

				Dim answer As Object

				If ref IsNot Nothing Then
					Dim f As String = ref.factoryClassName
					If f IsNot Nothing Then
						' if reference identifies a factory, use exclusively

						factory = getObjectFactoryFromReference(ref, f)
						If TypeOf factory Is DirObjectFactory Then
							Return CType(factory, DirObjectFactory).getObjectInstance(ref, name, nameCtx, environment, attrs)
						ElseIf factory IsNot Nothing Then
							Return factory.getObjectInstance(ref, name, nameCtx, environment)
						End If
						' No factory found, so return original refInfo.
						' Will reach this point if factory class is not in
						' class path and reference does not contain a URL for it
						Return refInfo

					Else
						' if reference has no factory, check for addresses
						' containing URLs
						' ignore name & attrs params; not used in URL factory

						answer = processURLAddrs(ref, name, nameCtx, environment)
						If answer IsNot Nothing Then Return answer
					End If
				End If

				' try using any specified factories
				answer = createObjectFromFactories(refInfo, name, nameCtx, environment, attrs)
				Return If(answer IsNot Nothing, answer, refInfo)
		End Function

		Private Shared Function createObjectFromFactories(Of T1)(ByVal obj As Object, ByVal name As javax.naming.Name, ByVal nameCtx As javax.naming.Context, ByVal environment As Dictionary(Of T1), ByVal attrs As javax.naming.directory.Attributes) As Object

			Dim factories As com.sun.naming.internal.FactoryEnumeration = com.sun.naming.internal.ResourceManager.getFactories(javax.naming.Context.OBJECT_FACTORIES, environment, nameCtx)

			If factories Is Nothing Then Return Nothing

			Dim factory As ObjectFactory
			Dim answer As Object = Nothing
			' Try each factory until one succeeds
			Do While answer Is Nothing AndAlso factories.hasMore()
				factory = CType(factories.next(), ObjectFactory)
				If TypeOf factory Is DirObjectFactory Then
					answer = CType(factory, DirObjectFactory).getObjectInstance(obj, name, nameCtx, environment, attrs)
				Else
					answer = factory.getObjectInstance(obj, name, nameCtx, environment)
				End If
			Loop
			Return answer
		End Function

		''' <summary>
		''' Retrieves the state of an object for binding when given the original
		''' object and its attributes.
		''' <p>
		''' This method is like <tt>NamingManager.getStateToBind</tt> except
		''' for the following differences:
		''' <ul>
		''' <li>It accepts an <tt>Attributes</tt> parameter containing attributes
		'''    that were passed to the <tt>DirContext.bind()</tt> method.
		''' <li>It returns a non-null <tt>DirStateFactory.Result</tt> instance
		'''    containing the object to be bound, and the attributes to
		'''    accompany the binding. Either the object or the attributes may be null.
		''' <li>
		''' The state factories tried must each implement either
		''' <tt>StateFactory</tt> or <tt>DirStateFactory</tt>.
		''' If it implements <tt>DirStateFactory</tt>, then
		''' <tt>DirStateFactory.getStateToBind()</tt> is called; otherwise,
		''' <tt>StateFactory.getStateToBind()</tt> is called.
		''' </ul>
		'''  
		''' Service providers that implement the <tt>DirContext</tt> interface
		''' should use this method, not <tt>NamingManager.getStateToBind()</tt>.
		''' <p>
		''' See NamingManager.getStateToBind() for a description of how
		''' the list of state factories to be tried is determined.
		''' <p>
		''' The object returned by this method is owned by the caller.
		''' The implementation will not subsequently modify it.
		''' It will contain either a new <tt>Attributes</tt> object that is
		''' likewise owned by the caller, or a reference to the original
		''' <tt>attrs</tt> parameter.
		''' </summary>
		''' <param name="obj"> The non-null object for which to get state to bind. </param>
		''' <param name="name"> The name of this object relative to <code>nameCtx</code>,
		'''         or null if no name is specified. </param>
		''' <param name="nameCtx"> The context relative to which the <code>name</code>
		'''         parameter is specified, or null if <code>name</code> is
		'''         relative to the default initial context. </param>
		''' <param name="environment"> The possibly null environment to
		'''         be used in the creation of the state factory and
		'''         the object's state. </param>
		''' <param name="attrs"> The possibly null Attributes that is to be bound with the
		'''         object. </param>
		''' <returns> A non-null DirStateFactory.Result containing
		'''  the object and attributes to be bound.
		'''  If no state factory returns a non-null answer, the result will contain
		'''  the object (<tt>obj</tt>) itself with the original attributes. </returns>
		''' <exception cref="NamingException"> If a naming exception was encountered
		'''         while using the factories.
		'''         A factory should only throw an exception if it does not want
		'''         other factories to be used in an attempt to create an object.
		'''         See <tt>DirStateFactory.getStateToBind()</tt>. </exception>
		''' <seealso cref= DirStateFactory </seealso>
		''' <seealso cref= DirStateFactory#getStateToBind </seealso>
		''' <seealso cref= NamingManager#getStateToBind
		''' @since 1.3 </seealso>
		Public Shared Function getStateToBind(Of T1)(ByVal obj As Object, ByVal name As javax.naming.Name, ByVal nameCtx As javax.naming.Context, ByVal environment As Dictionary(Of T1), ByVal attrs As javax.naming.directory.Attributes) As DirStateFactory.Result

			' Get list of state factories
			Dim factories As com.sun.naming.internal.FactoryEnumeration = com.sun.naming.internal.ResourceManager.getFactories(javax.naming.Context.STATE_FACTORIES, environment, nameCtx)

			If factories Is Nothing Then Return New DirStateFactory.Result(obj, attrs)

			' Try each factory until one succeeds
			Dim factory As StateFactory
			Dim objanswer As Object
			Dim answer As DirStateFactory.Result = Nothing
			Do While answer Is Nothing AndAlso factories.hasMore()
				factory = CType(factories.next(), StateFactory)
				If TypeOf factory Is DirStateFactory Then
					answer = CType(factory, DirStateFactory).getStateToBind(obj, name, nameCtx, environment, attrs)
				Else
					objanswer = factory.getStateToBind(obj, name, nameCtx, environment)
					If objanswer IsNot Nothing Then answer = New DirStateFactory.Result(objanswer, attrs)
				End If
			Loop

			Return If(answer IsNot Nothing, answer, New DirStateFactory.Result(obj, attrs)) ' nothing new
		End Function
	End Class

End Namespace
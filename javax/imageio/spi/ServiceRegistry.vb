Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.spi


	''' <summary>
	''' A registry for service provider instances.
	''' 
	''' <p> A <i>service</i> is a well-known set of interfaces and (usually
	''' abstract) classes.  A <i>service provider</i> is a specific
	''' implementation of a service.  The classes in a provider typically
	''' implement the interface or subclass the class defined by the
	''' service itself.
	''' 
	''' <p> Service providers are stored in one or more <i>categories</i>,
	''' each of which is defined by a class of interface (described by a
	''' <code>Class</code> object) that all of its members must implement.
	''' The set of categories may be changed dynamically.
	''' 
	''' <p> Only a single instance of a given leaf class (that is, the
	''' actual class returned by <code>getClass()</code>, as opposed to any
	''' inherited classes or interfaces) may be registered.  That is,
	''' suppose that the
	''' <code>com.mycompany.mypkg.GreenServiceProvider</code> class
	''' implements the <code>com.mycompany.mypkg.MyService</code>
	''' interface.  If a <code>GreenServiceProvider</code> instance is
	''' registered, it will be stored in the category defined by the
	''' <code>MyService</code> class.  If a new instance of
	''' <code>GreenServiceProvider</code> is registered, it will replace
	''' the previous instance.  In practice, service provider objects are
	''' usually singletons so this behavior is appropriate.
	''' 
	''' <p> To declare a service provider, a <code>services</code>
	''' subdirectory is placed within the <code>META-INF</code> directory
	''' that is present in every JAR file.  This directory contains a file
	''' for each service provider interface that has one or more
	''' implementation classes present in the JAR file.  For example, if
	''' the JAR file contained a class named
	''' <code>com.mycompany.mypkg.MyServiceImpl</code> which implements the
	''' <code>javax.someapi.SomeService</code> interface, the JAR file
	''' would contain a file named: <pre>
	''' META-INF/services/javax.someapi.SomeService </pre>
	''' 
	''' containing the line:
	''' 
	''' <pre>
	''' com.mycompany.mypkg.MyService
	''' </pre>
	''' 
	''' <p> The service provider classes should be to be lightweight and
	''' quick to load.  Implementations of these interfaces should avoid
	''' complex dependencies on other classes and on native code. The usual
	''' pattern for more complex services is to register a lightweight
	''' proxy for the heavyweight service.
	''' 
	''' <p> An application may customize the contents of a registry as it
	''' sees fit, so long as it has the appropriate runtime permission.
	''' 
	''' <p> For more details on declaring service providers, and the JAR
	''' format in general, see the <a
	''' href="../../../../technotes/guides/jar/jar.html">
	''' JAR File Specification</a>.
	''' </summary>
	''' <seealso cref= RegisterableService
	'''  </seealso>
	Public Class ServiceRegistry

		' Class -> Registry
		Private categoryMap As IDictionary = New Hashtable

		''' <summary>
		''' Constructs a <code>ServiceRegistry</code> instance with a
		''' set of categories taken from the <code>categories</code>
		''' argument.
		''' </summary>
		''' <param name="categories"> an <code>Iterator</code> containing
		''' <code>Class</code> objects to be used to define categories.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>categories</code> is <code>null</code>. </exception>
		Public Sub New(ByVal categories As IEnumerator(Of Type))
			If categories Is Nothing Then Throw New System.ArgumentException("categories == null!")
			Do While categories.MoveNext()
				Dim category As Type = CType(categories.Current, [Class])
				Dim reg As New SubRegistry(Me, category)
				categoryMap(category) = reg
			Loop
		End Sub

		' The following two methods expose functionality from
		' sun.misc.Service.  If that class is made public, they may be
		' removed.
		'
		' The sun.misc.ServiceConfigurationError class may also be
		' exposed, in which case the references to 'an
		' <code>Error</code>' below should be changed to 'a
		' <code>ServiceConfigurationError</code>'.

		''' <summary>
		''' Searches for implementations of a particular service class
		''' using the given class loader.
		''' 
		''' <p> This method transforms the name of the given service class
		''' into a provider-configuration filename as described in the
		''' class comment and then uses the <code>getResources</code>
		''' method of the given class loader to find all available files
		''' with that name.  These files are then read and parsed to
		''' produce a list of provider-class names.  The iterator that is
		''' returned uses the given class loader to look up and then
		''' instantiate each element of the list.
		''' 
		''' <p> Because it is possible for extensions to be installed into
		''' a running Java virtual machine, this method may return
		''' different results each time it is invoked.
		''' </summary>
		''' <param name="providerClass"> a <code>Class</code>object indicating the
		''' class or interface of the service providers being detected.
		''' </param>
		''' <param name="loader"> the class loader to be used to load
		''' provider-configuration files and instantiate provider classes,
		''' or <code>null</code> if the system class loader (or, failing that
		''' the bootstrap class loader) is to be used.
		''' </param>
		''' @param <T> the type of the providerClass.
		''' </param>
		''' <returns> An <code>Iterator</code> that yields provider objects
		''' for the given service, in some arbitrary order.  The iterator
		''' will throw an <code>Error</code> if a provider-configuration
		''' file violates the specified format or if a provider class
		''' cannot be found and instantiated.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>providerClass</code> is <code>null</code>. </exception>
		Public Shared Function lookupProviders(Of T)(ByVal providerClass As Type, ByVal loader As ClassLoader) As IEnumerator(Of T)
			If providerClass Is Nothing Then Throw New System.ArgumentException("providerClass == null!")
			Return java.util.ServiceLoader.load(providerClass, loader).GetEnumerator()
		End Function

		''' <summary>
		''' Locates and incrementally instantiates the available providers
		''' of a given service using the context class loader.  This
		''' convenience method is equivalent to:
		''' 
		''' <pre>
		'''   ClassLoader cl = Thread.currentThread().getContextClassLoader();
		'''   return Service.providers(service, cl);
		''' </pre>
		''' </summary>
		''' <param name="providerClass"> a <code>Class</code>object indicating the
		''' class or interface of the service providers being detected.
		''' </param>
		''' @param <T> the type of the providerClass.
		''' </param>
		''' <returns> An <code>Iterator</code> that yields provider objects
		''' for the given service, in some arbitrary order.  The iterator
		''' will throw an <code>Error</code> if a provider-configuration
		''' file violates the specified format or if a provider class
		''' cannot be found and instantiated.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>providerClass</code> is <code>null</code>. </exception>
		Public Shared Function lookupProviders(Of T)(ByVal providerClass As Type) As IEnumerator(Of T)
			If providerClass Is Nothing Then Throw New System.ArgumentException("providerClass == null!")
			Return java.util.ServiceLoader.load(providerClass).GetEnumerator()
		End Function

		''' <summary>
		''' Returns an <code>Iterator</code> of <code>Class</code> objects
		''' indicating the current set of categories.  The iterator will be
		''' empty if no categories exist.
		''' </summary>
		''' <returns> an <code>Iterator</code> containing
		''' <code>Class</code>objects. </returns>
		Public Overridable Property categories As IEnumerator(Of Type)
			Get
				Dim keySet As IDictionary.KeyCollection = categoryMap.Keys
				Return keySet.GetEnumerator()
			End Get
		End Property

		''' <summary>
		''' Returns an Iterator containing the subregistries to which the
		''' provider belongs.
		''' </summary>
		Private Function getSubRegistries(ByVal provider As Object) As IEnumerator
			Dim l As IList = New ArrayList
			Dim iter As IEnumerator = categoryMap.Keys.GetEnumerator()
			Do While iter.hasNext()
				Dim c As Type = CType(iter.next(), [Class])
				If c.IsAssignableFrom(provider.GetType()) Then l.Add(CType(categoryMap(c), SubRegistry))
			Loop
			Return l.GetEnumerator()
		End Function

		''' <summary>
		''' Adds a service provider object to the registry.  The provider
		''' is associated with the given category.
		''' 
		''' <p> If <code>provider</code> implements the
		''' <code>RegisterableService</code> interface, its
		''' <code>onRegistration</code> method will be called.  Its
		''' <code>onDeregistration</code> method will be called each time
		''' it is deregistered from a category, for example if a
		''' category is removed or the registry is garbage collected.
		''' </summary>
		''' <param name="provider"> the service provide object to be registered. </param>
		''' <param name="category"> the category under which to register the
		''' provider. </param>
		''' @param <T> the type of the provider.
		''' </param>
		''' <returns> true if no provider of the same class was previously
		''' registered in the same category category.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>provider</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		''' <exception cref="ClassCastException"> if provider does not implement
		''' the <code>Class</code> defined by <code>category</code>. </exception>
		Public Overridable Function registerServiceProvider(Of T)(ByVal provider As T, ByVal category As Type) As Boolean
			If provider Is Nothing Then Throw New System.ArgumentException("provider == null!")
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			If Not category.IsAssignableFrom(provider.GetType()) Then Throw New ClassCastException

			Return reg.registerServiceProvider(provider)
		End Function

		''' <summary>
		''' Adds a service provider object to the registry.  The provider
		''' is associated within each category present in the registry
		''' whose <code>Class</code> it implements.
		''' 
		''' <p> If <code>provider</code> implements the
		''' <code>RegisterableService</code> interface, its
		''' <code>onRegistration</code> method will be called once for each
		''' category it is registered under.  Its
		''' <code>onDeregistration</code> method will be called each time
		''' it is deregistered from a category or when the registry is
		''' finalized.
		''' </summary>
		''' <param name="provider"> the service provider object to be registered.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>provider</code> is <code>null</code>. </exception>
		Public Overridable Sub registerServiceProvider(ByVal provider As Object)
			If provider Is Nothing Then Throw New System.ArgumentException("provider == null!")
			Dim regs As IEnumerator = getSubRegistries(provider)
			Do While regs.hasNext()
				Dim reg As SubRegistry = CType(regs.next(), SubRegistry)
				reg.registerServiceProvider(provider)
			Loop
		End Sub

		''' <summary>
		''' Adds a set of service provider objects, taken from an
		''' <code>Iterator</code> to the registry.  Each provider is
		''' associated within each category present in the registry whose
		''' <code>Class</code> it implements.
		''' 
		''' <p> For each entry of <code>providers</code> that implements
		''' the <code>RegisterableService</code> interface, its
		''' <code>onRegistration</code> method will be called once for each
		''' category it is registered under.  Its
		''' <code>onDeregistration</code> method will be called each time
		''' it is deregistered from a category or when the registry is
		''' finalized.
		''' </summary>
		''' <param name="providers"> an Iterator containing service provider
		''' objects to be registered.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>providers</code>
		''' is <code>null</code> or contains a <code>null</code> entry. </exception>
		Public Overridable Sub registerServiceProviders(Of T1)(ByVal providers As IEnumerator(Of T1))
			If providers Is Nothing Then Throw New System.ArgumentException("provider == null!")
			Do While providers.MoveNext()
				registerServiceProvider(providers.Current)
			Loop
		End Sub

		''' <summary>
		''' Removes a service provider object from the given category.  If
		''' the provider was not previously registered, nothing happens and
		''' <code>false</code> is returned.  Otherwise, <code>true</code>
		''' is returned.  If an object of the same class as
		''' <code>provider</code> but not equal (using <code>==</code>) to
		''' <code>provider</code> is registered, it will not be
		''' deregistered.
		''' 
		''' <p> If <code>provider</code> implements the
		''' <code>RegisterableService</code> interface, its
		''' <code>onDeregistration</code> method will be called.
		''' </summary>
		''' <param name="provider"> the service provider object to be deregistered. </param>
		''' <param name="category"> the category from which to deregister the
		''' provider. </param>
		''' @param <T> the type of the provider.
		''' </param>
		''' <returns> <code>true</code> if the provider was previously
		''' registered in the same category category,
		''' <code>false</code> otherwise.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>provider</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		''' <exception cref="ClassCastException"> if provider does not implement
		''' the class defined by <code>category</code>. </exception>
		Public Overridable Function deregisterServiceProvider(Of T)(ByVal provider As T, ByVal category As Type) As Boolean
			If provider Is Nothing Then Throw New System.ArgumentException("provider == null!")
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			If Not category.IsAssignableFrom(provider.GetType()) Then Throw New ClassCastException
			Return reg.deregisterServiceProvider(provider)
		End Function

		''' <summary>
		''' Removes a service provider object from all categories that
		''' contain it.
		''' </summary>
		''' <param name="provider"> the service provider object to be deregistered.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>provider</code> is
		''' <code>null</code>. </exception>
		Public Overridable Sub deregisterServiceProvider(ByVal provider As Object)
			If provider Is Nothing Then Throw New System.ArgumentException("provider == null!")
			Dim regs As IEnumerator = getSubRegistries(provider)
			Do While regs.hasNext()
				Dim reg As SubRegistry = CType(regs.next(), SubRegistry)
				reg.deregisterServiceProvider(provider)
			Loop
		End Sub

		''' <summary>
		''' Returns <code>true</code> if <code>provider</code> is currently
		''' registered.
		''' </summary>
		''' <param name="provider"> the service provider object to be queried.
		''' </param>
		''' <returns> <code>true</code> if the given provider has been
		''' registered.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>provider</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function contains(ByVal provider As Object) As Boolean
			If provider Is Nothing Then Throw New System.ArgumentException("provider == null!")
			Dim regs As IEnumerator = getSubRegistries(provider)
			Do While regs.hasNext()
				Dim reg As SubRegistry = CType(regs.next(), SubRegistry)
				If reg.contains(provider) Then Return True
			Loop

			Return False
		End Function

		''' <summary>
		''' Returns an <code>Iterator</code> containing all registered
		''' service providers in the given category.  If
		''' <code>useOrdering</code> is <code>false</code>, the iterator
		''' will return all of the server provider objects in an arbitrary
		''' order.  Otherwise, the ordering will respect any pairwise
		''' orderings that have been set.  If the graph of pairwise
		''' orderings contains cycles, any providers that belong to a cycle
		''' will not be returned.
		''' </summary>
		''' <param name="category"> the category to be retrieved from. </param>
		''' <param name="useOrdering"> <code>true</code> if pairwise orderings
		''' should be taken account in ordering the returned objects. </param>
		''' @param <T> the type of the category.
		''' </param>
		''' <returns> an <code>Iterator</code> containing service provider
		''' objects from the given category, possibly in order.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		Public Overridable Function getServiceProviders(Of T)(ByVal category As Type, ByVal useOrdering As Boolean) As IEnumerator(Of T)
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			Return reg.getServiceProviders(useOrdering)
		End Function

		''' <summary>
		''' A simple filter interface used by
		''' <code>ServiceRegistry.getServiceProviders</code> to select
		''' providers matching an arbitrary criterion.  Classes that
		''' implement this interface should be defined in order to make use
		''' of the <code>getServiceProviders</code> method of
		''' <code>ServiceRegistry</code> that takes a <code>Filter</code>.
		''' </summary>
		''' <seealso cref= ServiceRegistry#getServiceProviders(Class, ServiceRegistry.Filter, boolean) </seealso>
		Public Interface Filter

			''' <summary>
			''' Returns <code>true</code> if the given
			''' <code>provider</code> object matches the criterion defined
			''' by this <code>Filter</code>.
			''' </summary>
			''' <param name="provider"> a service provider <code>Object</code>.
			''' </param>
			''' <returns> true if the provider matches the criterion. </returns>
			Function filter(ByVal provider As Object) As Boolean
		End Interface

		''' <summary>
		''' Returns an <code>Iterator</code> containing service provider
		''' objects within a given category that satisfy a criterion
		''' imposed by the supplied <code>ServiceRegistry.Filter</code>
		''' object's <code>filter</code> method.
		''' 
		''' <p> The <code>useOrdering</code> argument controls the
		''' ordering of the results using the same rules as
		''' <code>getServiceProviders(Class, boolean)</code>.
		''' </summary>
		''' <param name="category"> the category to be retrieved from. </param>
		''' <param name="filter"> an instance of <code>ServiceRegistry.Filter</code>
		''' whose <code>filter</code> method will be invoked. </param>
		''' <param name="useOrdering"> <code>true</code> if pairwise orderings
		''' should be taken account in ordering the returned objects. </param>
		''' @param <T> the type of the category.
		''' </param>
		''' <returns> an <code>Iterator</code> containing service provider
		''' objects from the given category, possibly in order.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		Public Overridable Function getServiceProviders(Of T)(ByVal category As Type, ByVal filter As Filter, ByVal useOrdering As Boolean) As IEnumerator(Of T)
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			Dim iter As IEnumerator = getServiceProviders(category, useOrdering)
			Return New FilterIterator(iter, filter)
		End Function

		''' <summary>
		''' Returns the currently registered service provider object that
		''' is of the given class type.  At most one object of a given
		''' class is allowed to be registered at any given time.  If no
		''' registered object has the desired class type, <code>null</code>
		''' is returned.
		''' </summary>
		''' <param name="providerClass"> the <code>Class</code> of the desired
		''' service provider object. </param>
		''' @param <T> the type of the provider.
		''' </param>
		''' <returns> a currently registered service provider object with the
		''' desired <code>Class</code>type, or <code>null</code> is none is
		''' present.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>providerClass</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function getServiceProviderByClass(Of T)(ByVal providerClass As Type) As T
			If providerClass Is Nothing Then Throw New System.ArgumentException("providerClass == null!")
			Dim iter As IEnumerator = categoryMap.Keys.GetEnumerator()
			Do While iter.hasNext()
				Dim c As Type = CType(iter.next(), [Class])
				If c.IsAssignableFrom(providerClass) Then
					Dim reg As SubRegistry = CType(categoryMap(c), SubRegistry)
					Dim provider As T = reg.getServiceProviderByClass(providerClass)
					If provider IsNot Nothing Then Return provider
				End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Sets a pairwise ordering between two service provider objects
		''' within a given category.  If one or both objects are not
		''' currently registered within the given category, or if the
		''' desired ordering is already set, nothing happens and
		''' <code>false</code> is returned.  If the providers previously
		''' were ordered in the reverse direction, that ordering is
		''' removed.
		''' 
		''' <p> The ordering will be used by the
		''' <code>getServiceProviders</code> methods when their
		''' <code>useOrdering</code> argument is <code>true</code>.
		''' </summary>
		''' <param name="category"> a <code>Class</code> object indicating the
		''' category under which the preference is to be established. </param>
		''' <param name="firstProvider"> the preferred provider. </param>
		''' <param name="secondProvider"> the provider to which
		''' <code>firstProvider</code> is preferred. </param>
		''' @param <T> the type of the category.
		''' </param>
		''' <returns> <code>true</code> if a previously unset ordering
		''' was established.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if either provider is
		''' <code>null</code> or they are the same object. </exception>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		Public Overridable Function setOrdering(Of T)(ByVal category As Type, ByVal firstProvider As T, ByVal secondProvider As T) As Boolean
			If firstProvider Is Nothing OrElse secondProvider Is Nothing Then Throw New System.ArgumentException("provider is null!")
			If firstProvider Is secondProvider Then Throw New System.ArgumentException("providers are the same!")
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			If reg.contains(firstProvider) AndAlso reg.contains(secondProvider) Then Return reg.orderinging(firstProvider, secondProvider)
			Return False
		End Function

		''' <summary>
		''' Sets a pairwise ordering between two service provider objects
		''' within a given category.  If one or both objects are not
		''' currently registered within the given category, or if no
		''' ordering is currently set between them, nothing happens
		''' and <code>false</code> is returned.
		''' 
		''' <p> The ordering will be used by the
		''' <code>getServiceProviders</code> methods when their
		''' <code>useOrdering</code> argument is <code>true</code>.
		''' </summary>
		''' <param name="category"> a <code>Class</code> object indicating the
		''' category under which the preference is to be disestablished. </param>
		''' <param name="firstProvider"> the formerly preferred provider. </param>
		''' <param name="secondProvider"> the provider to which
		''' <code>firstProvider</code> was formerly preferred. </param>
		''' @param <T> the type of the category.
		''' </param>
		''' <returns> <code>true</code> if a previously set ordering was
		''' disestablished.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if either provider is
		''' <code>null</code> or they are the same object. </exception>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		Public Overridable Function unsetOrdering(Of T)(ByVal category As Type, ByVal firstProvider As T, ByVal secondProvider As T) As Boolean
			If firstProvider Is Nothing OrElse secondProvider Is Nothing Then Throw New System.ArgumentException("provider is null!")
			If firstProvider Is secondProvider Then Throw New System.ArgumentException("providers are the same!")
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			If reg.contains(firstProvider) AndAlso reg.contains(secondProvider) Then Return reg.unsetOrdering(firstProvider, secondProvider)
			Return False
		End Function

		''' <summary>
		''' Deregisters all service provider object currently registered
		''' under the given category.
		''' </summary>
		''' <param name="category"> the category to be emptied.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there is no category
		''' corresponding to <code>category</code>. </exception>
		Public Overridable Sub deregisterAll(ByVal category As Type)
			Dim reg As SubRegistry = CType(categoryMap(category), SubRegistry)
			If reg Is Nothing Then Throw New System.ArgumentException("category unknown!")
			reg.clear()
		End Sub

		''' <summary>
		''' Deregisters all currently registered service providers from all
		''' categories.
		''' </summary>
		Public Overridable Sub deregisterAll()
			Dim iter As IEnumerator = categoryMap.Values.GetEnumerator()
			Do While iter.hasNext()
				Dim reg As SubRegistry = CType(iter.next(), SubRegistry)
				reg.clear()
			Loop
		End Sub

		''' <summary>
		''' Finalizes this object prior to garbage collection.  The
		''' <code>deregisterAll</code> method is called to deregister all
		''' currently registered service providers.  This method should not
		''' be called from application code.
		''' </summary>
		''' <exception cref="Throwable"> if an error occurs during superclass
		''' finalization. </exception>
		Protected Overrides Sub Finalize()
			deregisterAll()
			MyBase.Finalize()
		End Sub
	End Class


	''' <summary>
	''' A portion of a registry dealing with a single superclass or
	''' interface.
	''' </summary>
	Friend Class SubRegistry

		Friend registry As ServiceRegistry

		Friend category As Type

		' Provider Objects organized by partial oridering
		Friend poset As New PartiallyOrderedSet

		' Class -> Provider Object of that class
		Friend map As IDictionary(Of Type, Object) = New Hashtable

		Public Sub New(ByVal registry As ServiceRegistry, ByVal category As Type)
			Me.registry = registry
			Me.category = category
		End Sub

		Public Overridable Function registerServiceProvider(ByVal provider As Object) As Boolean
			Dim oprovider As Object = map(provider.GetType())
			Dim present As Boolean = oprovider IsNot Nothing

			If present Then deregisterServiceProvider(oprovider)
			map(provider.GetType()) = provider
			poset.add(provider)
			If TypeOf provider Is RegisterableService Then
				Dim rs As RegisterableService = CType(provider, RegisterableService)
				rs.onRegistration(registry, category)
			End If

			Return Not present
		End Function

		''' <summary>
		''' If the provider was not previously registered, do nothing.
		''' </summary>
		''' <returns> true if the provider was previously registered. </returns>
		Public Overridable Function deregisterServiceProvider(ByVal provider As Object) As Boolean
			Dim oprovider As Object = map(provider.GetType())

			If provider Is oprovider Then
				map.Remove(provider.GetType())
				poset.remove(provider)
				If TypeOf provider Is RegisterableService Then
					Dim rs As RegisterableService = CType(provider, RegisterableService)
					rs.onDeregistration(registry, category)
				End If

				Return True
			End If
			Return False
		End Function

		Public Overridable Function contains(ByVal provider As Object) As Boolean
			Dim oprovider As Object = map(provider.GetType())
			Return oprovider Is provider
		End Function

		Public Overridable Function setOrdering(ByVal firstProvider As Object, ByVal secondProvider As Object) As Boolean
			Return poset.setOrdering(firstProvider, secondProvider)
		End Function

		Public Overridable Function unsetOrdering(ByVal firstProvider As Object, ByVal secondProvider As Object) As Boolean
			Return poset.unsetOrdering(firstProvider, secondProvider)
		End Function

		Public Overridable Function getServiceProviders(ByVal useOrdering As Boolean) As IEnumerator
			If useOrdering Then
				Return poset.GetEnumerator()
			Else
				Return map.Values.GetEnumerator()
			End If
		End Function

		Public Overridable Function getServiceProviderByClass(Of T)(ByVal providerClass As Type) As T
			Return CType(map(providerClass), T)
		End Function

		Public Overridable Sub clear()
			Dim iter As IEnumerator = map.Values.GetEnumerator()
			Do While iter.hasNext()
				Dim provider As Object = iter.next()
				iter.remove()

				If TypeOf provider Is RegisterableService Then
					Dim rs As RegisterableService = CType(provider, RegisterableService)
					rs.onDeregistration(registry, category)
				End If
			Loop
			poset.clear()
		End Sub

		Protected Overrides Sub Finalize()
			clear()
		End Sub
	End Class


	''' <summary>
	''' A class for wrapping <code>Iterators</code> with a filter function.
	''' This provides an iterator for a subset without duplication.
	''' </summary>
	Friend Class FilterIterator(Of T)
		Implements IEnumerator(Of T)

		Private iter As IEnumerator(Of T)
		Private filter As ServiceRegistry.Filter

		Private ___next As T = Nothing

		Public Sub New(ByVal iter As IEnumerator(Of T), ByVal filter As ServiceRegistry.Filter)
			Me.iter = iter
			Me.filter = filter
			advance()
		End Sub

		Private Sub advance()
			Do While iter.MoveNext()
				Dim elt As T = iter.Current
				If filter.filter(elt) Then
					___next = elt
					Return
				End If
			Loop

			___next = Nothing
		End Sub

		Public Overridable Function hasNext() As Boolean
			Return ___next IsNot Nothing
		End Function

		Public Overridable Function [next]() As T
			If ___next Is Nothing Then Throw New java.util.NoSuchElementException
			Dim o As T = ___next
			advance()
			Return o
		End Function

		Public Overridable Sub remove()
			Throw New System.NotSupportedException
		End Sub
	End Class

End Namespace
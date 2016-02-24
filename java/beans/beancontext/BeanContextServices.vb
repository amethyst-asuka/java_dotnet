Imports System.Collections

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans.beancontext







	''' <summary>
	''' <p>
	''' The BeanContextServices interface provides a mechanism for a BeanContext
	''' to expose generic "services" to the BeanContextChild objects within.
	''' </p>
	''' </summary>
	Public Interface BeanContextServices
		Inherits java.beans.beancontext.BeanContext, java.beans.beancontext.BeanContextServicesListener

		''' <summary>
		''' Adds a service to this BeanContext.
		''' <code>BeanContextServiceProvider</code>s call this method
		''' to register a particular service with this context.
		''' If the service has not previously been added, the
		''' <code>BeanContextServices</code> associates
		''' the service with the <code>BeanContextServiceProvider</code> and
		''' fires a <code>BeanContextServiceAvailableEvent</code> to all
		''' currently registered <code>BeanContextServicesListeners</code>.
		''' The method then returns <code>true</code>, indicating that
		''' the addition of the service was successful.
		''' If the given service has already been added, this method
		''' simply returns <code>false</code>. </summary>
		''' <param name="serviceClass">     the service to add </param>
		''' <param name="serviceProvider">  the <code>BeanContextServiceProvider</code>
		''' associated with the service </param>
		''' <returns> true if the service was successful added, false otherwise </returns>
		Function addService(ByVal serviceClass As Class, ByVal serviceProvider As java.beans.beancontext.BeanContextServiceProvider) As Boolean

		''' <summary>
		''' BeanContextServiceProviders wishing to remove
		''' a currently registered service from this context
		''' may do so via invocation of this method. Upon revocation of
		''' the service, the <code>BeanContextServices</code> fires a
		''' <code>BeanContextServiceRevokedEvent</code> to its
		''' list of currently registered
		''' <code>BeanContextServiceRevokedListeners</code> and
		''' <code>BeanContextServicesListeners</code>. </summary>
		''' <param name="serviceClass"> the service to revoke from this BeanContextServices </param>
		''' <param name="serviceProvider"> the BeanContextServiceProvider associated with
		''' this particular service that is being revoked </param>
		''' <param name="revokeCurrentServicesNow"> a value of <code>true</code>
		''' indicates an exceptional circumstance where the
		''' <code>BeanContextServiceProvider</code> or
		''' <code>BeanContextServices</code> wishes to immediately
		''' terminate service to all currently outstanding references
		''' to the specified service. </param>
		Sub revokeService(ByVal serviceClass As Class, ByVal serviceProvider As java.beans.beancontext.BeanContextServiceProvider, ByVal revokeCurrentServicesNow As Boolean)

		''' <summary>
		''' Reports whether or not a given service is
		''' currently available from this context. </summary>
		''' <param name="serviceClass"> the service in question </param>
		''' <returns> true if the service is available </returns>
		Function hasService(ByVal serviceClass As Class) As Boolean

		''' <summary>
		''' A <code>BeanContextChild</code>, or any arbitrary object
		''' associated with a <code>BeanContextChild</code>, may obtain
		''' a reference to a currently registered service from its
		''' nesting <code>BeanContextServices</code>
		''' via invocation of this method. When invoked, this method
		''' gets the service by calling the getService() method on the
		''' underlying <code>BeanContextServiceProvider</code>. </summary>
		''' <param name="child"> the <code>BeanContextChild</code>
		''' associated with this request </param>
		''' <param name="requestor"> the object requesting the service </param>
		''' <param name="serviceClass"> class of the requested service </param>
		''' <param name="serviceSelector"> the service dependent parameter </param>
		''' <param name="bcsrl"> the
		''' <code>BeanContextServiceRevokedListener</code> to notify
		''' if the service should later become revoked </param>
		''' <exception cref="TooManyListenersException"> if there are too many listeners </exception>
		''' <returns> a reference to this context's named
		''' Service as requested or <code>null</code> </returns>
		Function getService(ByVal child As BeanContextChild, ByVal requestor As Object, ByVal serviceClass As Class, ByVal serviceSelector As Object, ByVal bcsrl As BeanContextServiceRevokedListener) As Object

		''' <summary>
		''' Releases a <code>BeanContextChild</code>'s
		''' (or any arbitrary object associated with a BeanContextChild)
		''' reference to the specified service by calling releaseService()
		''' on the underlying <code>BeanContextServiceProvider</code>. </summary>
		''' <param name="child"> the <code>BeanContextChild</code> </param>
		''' <param name="requestor"> the requestor </param>
		''' <param name="service"> the service </param>
		Sub releaseService(ByVal child As BeanContextChild, ByVal requestor As Object, ByVal service As Object)

		''' <summary>
		''' Gets the currently available services for this context. </summary>
		''' <returns> an <code>Iterator</code> consisting of the
		''' currently available services </returns>
		ReadOnly Property currentServiceClasses As IEnumerator

		''' <summary>
		''' Gets the list of service dependent service parameters
		''' (Service Selectors) for the specified service, by
		''' calling getCurrentServiceSelectors() on the
		''' underlying BeanContextServiceProvider. </summary>
		''' <param name="serviceClass"> the specified service </param>
		''' <returns> the currently available service selectors
		''' for the named serviceClass </returns>
		Function getCurrentServiceSelectors(ByVal serviceClass As Class) As IEnumerator

		''' <summary>
		''' Adds a <code>BeanContextServicesListener</code> to this BeanContext </summary>
		''' <param name="bcsl"> the <code>BeanContextServicesListener</code> to add </param>
		Sub addBeanContextServicesListener(ByVal bcsl As java.beans.beancontext.BeanContextServicesListener)

		''' <summary>
		''' Removes a <code>BeanContextServicesListener</code>
		''' from this <code>BeanContext</code> </summary>
		''' <param name="bcsl"> the <code>BeanContextServicesListener</code>
		''' to remove from this context </param>
		Sub removeBeanContextServicesListener(ByVal bcsl As java.beans.beancontext.BeanContextServicesListener)
	End Interface

End Namespace
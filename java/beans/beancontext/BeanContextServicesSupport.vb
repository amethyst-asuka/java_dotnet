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
	''' This helper class provides a utility implementation of the
	''' java.beans.beancontext.BeanContextServices interface.
	''' </p>
	''' <p>
	''' Since this class directly implements the BeanContextServices interface,
	''' the class can, and is intended to be used either by subclassing this
	''' implementation, or via delegation of an instance of this class
	''' from another through the BeanContextProxy interface.
	''' </p>
	''' 
	''' @author Laurence P. G. Cable
	''' @since 1.2
	''' </summary>

	Public Class BeanContextServicesSupport
		Inherits BeanContextSupport
		Implements BeanContextServices

		Private Shadows Const serialVersionUID As Long = -8494482757288719206L

		''' <summary>
		''' <p>
		''' Construct a BeanContextServicesSupport instance
		''' </p>
		''' </summary>
		''' <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>
		''' <param name="lcle">      The current Locale for this BeanContext. </param>
		''' <param name="dTime">     The initial state, true if in design mode, false if runtime. </param>
		''' <param name="visible">   The initial visibility.
		'''  </param>

		Public Sub New(ByVal peer As BeanContextServices, ByVal lcle As java.util.Locale, ByVal dTime As Boolean, ByVal visible As Boolean)
			MyBase.New(peer, lcle, dTime, visible)
		End Sub

		''' <summary>
		''' Create an instance using the specified Locale and design mode.
		''' </summary>
		''' <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>
		''' <param name="lcle">      The current Locale for this BeanContext. </param>
		''' <param name="dtime">     The initial state, true if in design mode, false if runtime. </param>

		Public Sub New(ByVal peer As BeanContextServices, ByVal lcle As java.util.Locale, ByVal dtime As Boolean)
			Me.New(peer, lcle, dtime, True)
		End Sub

		''' <summary>
		''' Create an instance using the specified locale
		''' </summary>
		''' <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>
		''' <param name="lcle">      The current Locale for this BeanContext. </param>

		Public Sub New(ByVal peer As BeanContextServices, ByVal lcle As java.util.Locale)
			Me.New(peer, lcle, False, True)
		End Sub

		''' <summary>
		''' Create an instance with a peer
		''' </summary>
		''' <param name="peer">      The peer BeanContext we are supplying an implementation for, if null the this object is its own peer </param>

		Public Sub New(ByVal peer As BeanContextServices)
			Me.New(peer, Nothing, False, True)
		End Sub

		''' <summary>
		''' Create an instance that is not a delegate of another object
		''' </summary>

		Public Sub New()
			Me.New(Nothing, Nothing, False, True)
		End Sub

		''' <summary>
		''' called by BeanContextSupport superclass during construction and
		''' deserialization to initialize subclass transient state.
		''' 
		''' subclasses may envelope this method, but should not override it or
		''' call it directly.
		''' </summary>

		Public Overrides Sub initialize()
			MyBase.initialize()

			services = New Hashtable(serializable + 1)
			bcsListeners = New ArrayList(1)
		End Sub

		''' <summary>
		''' Gets the <tt>BeanContextServices</tt> associated with this
		''' <tt>BeanContextServicesSupport</tt>.
		''' </summary>
		''' <returns> the instance of <tt>BeanContext</tt>
		''' this object is providing the implementation for. </returns>
		Public Overridable Property beanContextServicesPeer As BeanContextServices
			Get
				Return CType(beanContextChildPeer, BeanContextServices)
			End Get
		End Property

		''' <summary>
		'''********************************************************************* </summary>

	'    
	'     * protected nested class containing per child information, an instance
	'     * of which is associated with each child in the "children" hashtable.
	'     * subclasses can extend this class to include their own per-child state.
	'     *
	'     * Note that this 'value' is serialized with the corresponding child 'key'
	'     * when the BeanContextSupport is serialized.
	'     

		Protected Friend Class BCSSChild
			Inherits BeanContextSupport.BCSChild

			Private ReadOnly outerInstance As BeanContextServicesSupport


			Private Const serialVersionUID As Long = -3263851306889194873L

	'        
	'         * private nested class to map serviceClass to Provider and requestors
	'         * listeners.
	'         

			Friend Class BCSSCServiceClassRef
				Private ReadOnly outerInstance As BeanContextServicesSupport.BCSSChild


				' create an instance of a service ref

				Friend Sub New(ByVal outerInstance As BeanContextServicesSupport.BCSSChild, ByVal sc As Class, ByVal bcsp As BeanContextServiceProvider, ByVal delegated As Boolean)
						Me.outerInstance = outerInstance
					MyBase.New()

					serviceClass = sc

					If delegated Then
						delegateProvider = bcsp
					Else
						serviceProvider = bcsp
					End If
				End Sub

				' add a requestor and assoc listener

				Friend Overridable Sub addRequestor(ByVal requestor As Object, ByVal bcsrl As BeanContextServiceRevokedListener)
					Dim cbcsrl As BeanContextServiceRevokedListener = CType(requestors(requestor), BeanContextServiceRevokedListener)

					If cbcsrl IsNot Nothing AndAlso (Not cbcsrl.Equals(bcsrl)) Then Throw New java.util.TooManyListenersException

					requestors(requestor) = bcsrl
				End Sub

				' remove a requestor

				Friend Overridable Sub removeRequestor(ByVal requestor As Object)
					requestors.Remove(requestor)
				End Sub

				' check a requestors listener

				Friend Overridable Sub verifyRequestor(ByVal requestor As Object, ByVal bcsrl As BeanContextServiceRevokedListener)
					Dim cbcsrl As BeanContextServiceRevokedListener = CType(requestors(requestor), BeanContextServiceRevokedListener)

					If cbcsrl IsNot Nothing AndAlso (Not cbcsrl.Equals(bcsrl)) Then Throw New java.util.TooManyListenersException
				End Sub

				Friend Overridable Sub verifyAndMaybeSetProvider(ByVal bcsp As BeanContextServiceProvider, ByVal isDelegated As Boolean)
					Dim current As BeanContextServiceProvider

					If isDelegated Then ' the provider is delegated
						current = delegateProvider

						If current Is Nothing OrElse bcsp Is Nothing Then
							delegateProvider = bcsp
							Return
						End If ' the provider is registered with this BCS
					Else
						current = serviceProvider

						If current Is Nothing OrElse bcsp Is Nothing Then
							serviceProvider = bcsp
							Return
						End If
					End If

					If Not current.Equals(bcsp) Then Throw New UnsupportedOperationException("existing service reference obtained from different BeanContextServiceProvider not supported")

				End Sub

				Friend Overridable Function cloneOfEntries() As IEnumerator
					Return CType(requestors.clone(), Hashtable).GetEnumerator()
				End Function

				Friend Overridable Function entries() As IEnumerator
					Return requestors.GetEnumerator()
				End Function

				Friend Overridable Property empty As Boolean
					Get
						Return requestors.Count = 0
					End Get
				End Property

				Friend Overridable Property serviceClass As Class
					Get
						Return serviceClass
					End Get
				End Property

				Friend Overridable Property serviceProvider As BeanContextServiceProvider
					Get
						Return serviceProvider
					End Get
				End Property

				Friend Overridable Property delegateProvider As BeanContextServiceProvider
					Get
						Return delegateProvider
					End Get
				End Property

				Friend Overridable Property delegated As Boolean
					Get
						Return delegateProvider IsNot Nothing
					End Get
				End Property

				Friend Overridable Sub addRef(ByVal delegated As Boolean)
					If delegated Then
						delegateRefs += 1
					Else
						serviceRefs += 1
					End If
				End Sub


				Friend Overridable Sub releaseRef(ByVal delegated As Boolean)
					If delegated Then
						delegateRefs -= 1
						If delegateRefs = 0 Then delegateProvider = Nothing
					Else
						serviceRefs -= 1
						If serviceRefs <= 0 Then serviceProvider = Nothing
					End If
				End Sub

				Friend Overridable Property refs As Integer
					Get
						Return serviceRefs + delegateRefs
					End Get
				End Property

				Friend Overridable Property delegateRefs As Integer
					Get
						Return delegateRefs
					End Get
				End Property

				Friend Overridable Property serviceRefs As Integer
					Get
						Return serviceRefs
					End Get
				End Property

	'            
	'             * fields
	'             

				Friend serviceClass As Class

				Friend serviceProvider As BeanContextServiceProvider
				Friend serviceRefs As Integer

				Friend delegateProvider As BeanContextServiceProvider ' proxy
				Friend delegateRefs As Integer

				Friend requestors As New Hashtable(1)
			End Class

	'        
	'         * per service reference info ...
	'         

			Friend Class BCSSCServiceRef
				Private ReadOnly outerInstance As BeanContextServicesSupport.BCSSChild

				Friend Sub New(ByVal outerInstance As BeanContextServicesSupport.BCSSChild, ByVal scref As BCSSCServiceClassRef, ByVal isDelegated As Boolean)
						Me.outerInstance = outerInstance
					serviceClassRef = scref
					delegated = isDelegated
				End Sub

				Friend Overridable Sub addRef()
					refCnt += 1
				End Sub
				Friend Overridable Function release() As Integer
						refCnt -= 1
						Return refCnt
				End Function

				Friend Overridable Property serviceClassRef As BCSSCServiceClassRef
					Get
						Return serviceClassRef
					End Get
				End Property

				Friend Overridable Property delegated As Boolean
					Get
						Return delegated
					End Get
				End Property

	'            
	'             * fields
	'             

				Friend serviceClassRef As BCSSCServiceClassRef
				Friend refCnt As Integer = 1
				Friend delegated As Boolean = False
			End Class

			Friend Sub New(ByVal outerInstance As BeanContextServicesSupport, ByVal bcc As Object, ByVal peer As Object)
					Me.outerInstance = outerInstance
				MyBase.New(bcc, peer)
			End Sub

			' note usage of service per requestor, per service

			SyncLock void usingService Object requestor, Object service, Class serviceClass, BeanContextServiceProvider bcsp, Boolean isDelegated, BeanContextServiceRevokedListener bcsrl
				throws java.util.TooManyListenersException, UnsupportedOperationException

				' first, process mapping from serviceClass to requestor(s)

				Dim serviceClassRef As BCSSCServiceClassRef = Nothing

				If serviceClasses Is Nothing Then
					serviceClasses = New Hashtable(1)
				Else
					serviceClassRef = CType(serviceClasses(serviceClass), BCSSCServiceClassRef)
				End If

				If serviceClassRef Is Nothing Then ' new service being used ...
					serviceClassRef = New BCSSCServiceClassRef(Me, serviceClass, bcsp, isDelegated)
					serviceClasses(serviceClass) = serviceClassRef
 ' existing service ...
				Else
					serviceClassRef.verifyAndMaybeSetProvider(bcsp, isDelegated) ' throws
					serviceClassRef.verifyRequestor(requestor, bcsrl) ' throws
				End If

				serviceClassRef.addRequestor(requestor, bcsrl)
				serviceClassRef.addRef(isDelegated)

				' now handle mapping from requestor to service(s)

				Dim serviceRef As BCSSCServiceRef = Nothing
				Dim services As IDictionary = Nothing

				If serviceRequestors Is Nothing Then
					serviceRequestors = New Hashtable(1)
				Else
					services = CType(serviceRequestors(requestor), IDictionary)
				End If

				If services Is Nothing Then
					services = New Hashtable(1)

					serviceRequestors(requestor) = services
				Else
					serviceRef = CType(services(service), BCSSCServiceRef)
				End If

				If serviceRef Is Nothing Then
					serviceRef = New BCSSCServiceRef(Me, serviceClassRef, isDelegated)

					services(service) = serviceRef
				Else
					serviceRef.addRef()
				End If
			End SyncLock

			' release a service reference

			SyncLock void releaseService Object requestor, Object service
				If serviceRequestors Is Nothing Then Return

				Dim services As IDictionary = CType(serviceRequestors(requestor), IDictionary)

				If services Is Nothing Then ' oops its not there anymore! Return

				Dim serviceRef As BCSSCServiceRef = CType(services(service), BCSSCServiceRef)

				If serviceRef Is Nothing Then ' oops its not there anymore! Return

				Dim serviceClassRef As BCSSCServiceClassRef = serviceRef.serviceClassRef
				Dim isDelegated As Boolean = serviceRef.delegated
				Dim bcsp As BeanContextServiceProvider = If(isDelegated, serviceClassRef.delegateProvider, serviceClassRef.serviceProvider)

				bcsp.releaseService(outerInstance.beanContextServicesPeer, requestor, service)

				serviceClassRef.releaseRef(isDelegated)
				serviceClassRef.removeRequestor(requestor)

				If serviceRef.release() = 0 Then

					services.Remove(service)

					If services.Count = 0 Then
						serviceRequestors.Remove(requestor)
						serviceClassRef.removeRequestor(requestor)
					End If

					If serviceRequestors.Count = 0 Then serviceRequestors = Nothing

					If serviceClassRef.empty Then serviceClasses.Remove(serviceClassRef.serviceClass)

					If serviceClasses.Count = 0 Then serviceClasses = Nothing
				End If
			End SyncLock

			' revoke a service

			SyncLock void revokeService Class serviceClass, Boolean isDelegated, Boolean revokeNow
				If serviceClasses Is Nothing Then Return

				Dim serviceClassRef As BCSSCServiceClassRef = CType(serviceClasses(serviceClass), BCSSCServiceClassRef)

				If serviceClassRef Is Nothing Then Return

				Dim i As IEnumerator = serviceClassRef.cloneOfEntries()

				Dim bcsre As New BeanContextServiceRevokedEvent(outerInstance.beanContextServicesPeer, serviceClass, revokeNow)
				Dim noMoreRefs As Boolean = False

				Do While i.hasNext() AndAlso serviceRequestors IsNot Nothing
					Dim entry As DictionaryEntry = CType(i.next(), DictionaryEntry)
					Dim listener As BeanContextServiceRevokedListener = CType(entry.Value, BeanContextServiceRevokedListener)

					If revokeNow Then
						Dim requestor As Object = entry.Key
						Dim services As IDictionary = CType(serviceRequestors(requestor), IDictionary)

						If services IsNot Nothing Then
							Dim i1 As IEnumerator = services.GetEnumerator()

							Do While i1.hasNext()
								Dim tmp As DictionaryEntry = CType(i1.next(), DictionaryEntry)

								Dim serviceRef As BCSSCServiceRef = CType(tmp.Value, BCSSCServiceRef)
								If serviceRef.serviceClassRef.Equals(serviceClassRef) AndAlso isDelegated = serviceRef.delegated Then i1.remove()
							Loop

'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							If noMoreRefs = services.Count = 0 Then serviceRequestors.Remove(requestor)
						End If

						If noMoreRefs Then serviceClassRef.removeRequestor(requestor)
					End If

					listener.serviceRevoked(bcsre)
				Loop

				If revokeNow AndAlso serviceClasses IsNot Nothing Then
					If serviceClassRef.empty Then serviceClasses.Remove(serviceClass)

					If serviceClasses.Count = 0 Then serviceClasses = Nothing
				End If

				If serviceRequestors IsNot Nothing AndAlso serviceRequestors.Count = 0 Then serviceRequestors = Nothing
			End SyncLock

			' release all references for this child since it has been unnested.

			void cleanupReferences()

				If serviceRequestors Is Nothing Then Return

				Dim requestors As IEnumerator = serviceRequestors.GetEnumerator()

				Do While requestors.hasNext()
					Dim tmp As DictionaryEntry = CType(requestors.next(), DictionaryEntry)
					Dim requestor As Object = tmp.Key
					Dim services As IEnumerator = CType(tmp.Value, IDictionary).GetEnumerator()

					requestors.remove()

					Do While services.hasNext()
						Dim entry As DictionaryEntry = CType(services.next(), DictionaryEntry)
						Dim service As Object = entry.Key
						Dim sref As BCSSCServiceRef = CType(entry.Value, BCSSCServiceRef)

						Dim scref As BCSSCServiceClassRef = sref.serviceClassRef

						Dim bcsp As BeanContextServiceProvider = If(sref.delegated, scref.delegateProvider, scref.serviceProvider)

						scref.removeRequestor(requestor)
						services.remove()

						Do While sref.release() >= 0
							bcsp.releaseService(outerInstance.beanContextServicesPeer, requestor, service)
						Loop
					Loop
				Loop

				serviceRequestors = Nothing
				serviceClasses = Nothing

			void revokeAllDelegatedServicesNow()
				If serviceClasses Is Nothing Then Return

				Dim serviceClassRefs As IEnumerator = (New HashSet(serviceClasses.Values)).GetEnumerator()

				Do While serviceClassRefs.hasNext()
					Dim serviceClassRef As BCSSCServiceClassRef = CType(serviceClassRefs.next(), BCSSCServiceClassRef)

					If Not serviceClassRef.delegated Then Continue Do

					Dim i As IEnumerator = serviceClassRef.cloneOfEntries()
					Dim bcsre As New BeanContextServiceRevokedEvent(outerInstance.beanContextServicesPeer, serviceClassRef.serviceClass, True)
					Dim noMoreRefs As Boolean = False

					Do While i.hasNext()
						Dim entry As DictionaryEntry = CType(i.next(), DictionaryEntry)
						Dim listener As BeanContextServiceRevokedListener = CType(entry.Value, BeanContextServiceRevokedListener)

						Dim requestor As Object = entry.Key
						Dim services As IDictionary = CType(serviceRequestors(requestor), IDictionary)

						If services IsNot Nothing Then
							Dim i1 As IEnumerator = services.GetEnumerator()

							Do While i1.hasNext()
								Dim tmp As DictionaryEntry = CType(i1.next(), DictionaryEntry)

								Dim serviceRef As BCSSCServiceRef = CType(tmp.Value, BCSSCServiceRef)
								If serviceRef.serviceClassRef.Equals(serviceClassRef) AndAlso serviceRef.delegated Then i1.remove()
							Loop

'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							If noMoreRefs = services.Count = 0 Then serviceRequestors.Remove(requestor)
						End If

						If noMoreRefs Then serviceClassRef.removeRequestor(requestor)

						listener.serviceRevoked(bcsre)

						If serviceClassRef.empty Then serviceClasses.Remove(serviceClassRef.serviceClass)
					Loop
				Loop

				If serviceClasses.Count = 0 Then serviceClasses = Nothing

				If serviceRequestors IsNot Nothing AndAlso serviceRequestors.Count = 0 Then serviceRequestors = Nothing

	'        
	'         * fields
	'         

			private transient Hashtable serviceClasses
			private transient Hashtable serviceRequestors
		End Class

		''' <summary>
		''' <p>
		''' Subclasses can override this method to insert their own subclass
		''' of Child without having to override add() or the other Collection
		''' methods that add children to the set.
		''' </p>
		''' </summary>
		''' <param name="targetChild"> the child to create the Child on behalf of </param>
		''' <param name="peer">        the peer if the targetChild and peer are related by BeanContextProxy </param>

		protected BCSChild createBCSChild(Object targetChild, Object peer)
			Return New BCSSChild(Me, targetChild, peer)

		''' <summary>
		'''********************************************************************* </summary>

			''' <summary>
			''' subclasses may subclass this nested class to add behaviors for
			''' each BeanContextServicesProvider.
			''' </summary>

			protected static class BCSSServiceProvider implements java.io.Serializable
				private static final Long serialVersionUID = 861278251667444782L

				BCSSServiceProvider(Class sc, BeanContextServiceProvider bcsp)
					MyBase()

					serviceProvider = bcsp

				''' <summary>
				''' Returns the service provider. </summary>
				''' <returns> the service provider </returns>
				protected BeanContextServiceProvider serviceProvider
					Return serviceProvider

				''' <summary>
				''' The service provider.
				''' </summary>

				protected BeanContextServiceProvider serviceProvider

			''' <summary>
			''' subclasses can override this method to create new subclasses of
			''' BCSSServiceProvider without having to override addService() in
			''' order to instantiate. </summary>
			''' <param name="sc"> the class </param>
			''' <param name="bcsp"> the service provider </param>
			''' <returns> a service provider without overriding addService() </returns>

			protected BCSSServiceProvider createBCSSServiceProvider(Class sc, BeanContextServiceProvider bcsp)
				Return New BCSSServiceProvider(sc, bcsp)

		''' <summary>
		'''********************************************************************* </summary>

		''' <summary>
		''' add a BeanContextServicesListener
		''' </summary>
		''' <exception cref="NullPointerException"> if the argument is null </exception>

		public void addBeanContextServicesListener(BeanContextServicesListener bcsl)
			If bcsl Is Nothing Then Throw New NullPointerException("bcsl")

			SyncLock bcsListeners
				If bcsListeners.Contains(bcsl) Then
					Return
				Else
					bcsListeners.Add(bcsl)
				End If
			End SyncLock

		''' <summary>
		''' remove a BeanContextServicesListener
		''' </summary>

		public void removeBeanContextServicesListener(BeanContextServicesListener bcsl)
			If bcsl Is Nothing Then Throw New NullPointerException("bcsl")

			SyncLock bcsListeners
				If Not bcsListeners.Contains(bcsl) Then
					Return
				Else
					bcsListeners.Remove(bcsl)
				End If
			End SyncLock

		''' <summary>
		''' add a service </summary>
		''' <param name="serviceClass"> the service class </param>
		''' <param name="bcsp"> the service provider </param>

		public Boolean addService(Class serviceClass, BeanContextServiceProvider bcsp)
			Return addService(serviceClass, bcsp, True)

		''' <summary>
		''' add a service </summary>
		''' <param name="serviceClass"> the service class </param>
		''' <param name="bcsp"> the service provider </param>
		''' <param name="fireEvent"> whether or not an event should be fired </param>
		''' <returns> true if the service was successfully added </returns>

		protected Boolean addService(Class serviceClass, BeanContextServiceProvider bcsp, Boolean fireEvent)

			If serviceClass Is Nothing Then Throw New NullPointerException("serviceClass")
			If bcsp Is Nothing Then Throw New NullPointerException("bcsp")

			SyncLock BeanContext.globalHierarchyLock
				If services.ContainsKey(serviceClass) Then
					Return False
				Else
					services(serviceClass) = createBCSSServiceProvider(serviceClass, bcsp)

					If TypeOf bcsp Is java.io.Serializable Then serializable += 1

					If Not fireEvent Then Return True


					Dim bcssae As New BeanContextServiceAvailableEvent(beanContextServicesPeer, serviceClass)

					fireServiceAdded(bcssae)

					SyncLock children
						Dim i As IEnumerator = children.Keys.GetEnumerator()

						Do While i.hasNext()
							Dim c As Object = i.next()

							If TypeOf c Is BeanContextServices Then CType(c, BeanContextServicesListener).serviceAvailable(bcssae)
						Loop
					End SyncLock

					Return True
				End If
			End SyncLock

		''' <summary>
		''' remove a service </summary>
		''' <param name="serviceClass"> the service class </param>
		''' <param name="bcsp"> the service provider </param>
		''' <param name="revokeCurrentServicesNow"> whether or not to revoke the service </param>

		public void revokeService(Class serviceClass, BeanContextServiceProvider bcsp, Boolean revokeCurrentServicesNow)

			If serviceClass Is Nothing Then Throw New NullPointerException("serviceClass")
			If bcsp Is Nothing Then Throw New NullPointerException("bcsp")

			SyncLock BeanContext.globalHierarchyLock
				If Not services.ContainsKey(serviceClass) Then Return

				Dim bcsssp As BCSSServiceProvider = CType(services(serviceClass), BCSSServiceProvider)

				If Not bcsssp.serviceProvider.Equals(bcsp) Then Throw New IllegalArgumentException("service provider mismatch")

				services.Remove(serviceClass)

				If TypeOf bcsp Is java.io.Serializable Then serializable -= 1

				Dim i As IEnumerator = bcsChildren() ' get the BCSChild values.

				Do While i.hasNext()
					CType(i.next(), BCSSChild).revokeService(serviceClass, False, revokeCurrentServicesNow)
				Loop

				fireServiceRevoked(serviceClass, revokeCurrentServicesNow)
			End SyncLock

		''' <summary>
		''' has a service, which may be delegated
		''' </summary>

		public synchronized Boolean hasService(Class serviceClass)
			If serviceClass Is Nothing Then Throw New NullPointerException("serviceClass")

			SyncLock BeanContext.globalHierarchyLock
				If services.ContainsKey(serviceClass) Then Return True

				Dim bcs As BeanContextServices = Nothing

				Try
					bcs = CType(beanContext, BeanContextServices)
				Catch cce As ClassCastException
					Return False
				End Try

				Return If(bcs Is Nothing, False, bcs.hasService(serviceClass))
			End SyncLock

		''' <summary>
		'''********************************************************************* </summary>

	'    
	'     * a nested subclass used to represent a proxy for serviceClasses delegated
	'     * to an enclosing BeanContext.
	'     

		protected class BCSSProxyServiceProvider implements BeanContextServiceProvider, BeanContextServiceRevokedListener

			BCSSProxyServiceProvider(BeanContextServices bcs)
				MyBase()

				nestingCtxt = bcs

			public Object getService(BeanContextServices bcs, Object requestor, Class serviceClass, Object serviceSelector)
				Dim service_Renamed As Object = Nothing

				Try
					service_Renamed = nestingCtxt.getService(bcs, requestor, serviceClass, serviceSelector, Me)
				Catch tmle As java.util.TooManyListenersException
					Return Nothing
				End Try

				Return service_Renamed

			public void releaseService(BeanContextServices bcs, Object requestor, Object service)
				nestingCtxt.releaseService(bcs, requestor, service)

			public IEnumerator getCurrentServiceSelectors(BeanContextServices bcs, Class serviceClass)
				Return nestingCtxt.getCurrentServiceSelectors(serviceClass)

			public void serviceRevoked(BeanContextServiceRevokedEvent bcsre)
				Dim i As IEnumerator = bcsChildren() ' get the BCSChild values.

				Do While i.hasNext()
					CType(i.next(), BCSSChild).revokeService(bcsre.serviceClass, True, bcsre.currentServiceInvalidNow)
				Loop

	'        
	'         * fields
	'         

			private BeanContextServices nestingCtxt

		''' <summary>
		'''********************************************************************* </summary>

		''' <summary>
		''' obtain a service which may be delegated
		''' </summary>

		 public Object getService(BeanContextChild child, Object requestor, Class serviceClass, Object serviceSelector, BeanContextServiceRevokedListener bcsrl) throws java.util.TooManyListenersException
			If child Is Nothing Then Throw New NullPointerException("child")
			If serviceClass Is Nothing Then Throw New NullPointerException("serviceClass")
			If requestor Is Nothing Then Throw New NullPointerException("requestor")
			If bcsrl Is Nothing Then Throw New NullPointerException("bcsrl")

			Dim service_Renamed As Object = Nothing
			Dim bcsc As BCSSChild
			Dim bcssp As BeanContextServices = beanContextServicesPeer

			SyncLock BeanContext.globalHierarchyLock
				SyncLock children
					bcsc = CType(children(child), BCSSChild)
				End SyncLock

				If bcsc Is Nothing Then ' not a child ... Throw New IllegalArgumentException("not a child of this context")

				Dim bcsssp As BCSSServiceProvider = CType(services(serviceClass), BCSSServiceProvider)

				If bcsssp IsNot Nothing Then
					Dim bcsp As BeanContextServiceProvider = bcsssp.serviceProvider
					service_Renamed = bcsp.getService(bcssp, requestor, serviceClass, serviceSelector)
					If service_Renamed IsNot Nothing Then ' do bookkeeping ...
						Try
							bcsc.usingService(requestor, service_Renamed, serviceClass, bcsp, False, bcsrl)
						Catch tmle As java.util.TooManyListenersException
							bcsp.releaseService(bcssp, requestor, service_Renamed)
							Throw tmle
						Catch uope As UnsupportedOperationException
							bcsp.releaseService(bcssp, requestor, service_Renamed)
							Throw uope ' unchecked rt exception
						End Try

						Return service_Renamed
					End If
				End If


				If proxy IsNot Nothing Then

					' try to delegate ...

					service_Renamed = proxy.getService(bcssp, requestor, serviceClass, serviceSelector)

					If service_Renamed IsNot Nothing Then ' do bookkeeping ...
						Try
							bcsc.usingService(requestor, service_Renamed, serviceClass, proxy, True, bcsrl)
						Catch tmle As java.util.TooManyListenersException
							proxy.releaseService(bcssp, requestor, service_Renamed)
							Throw tmle
						Catch uope As UnsupportedOperationException
							proxy.releaseService(bcssp, requestor, service_Renamed)
							Throw uope ' unchecked rt exception
						End Try

						Return service_Renamed
					End If
				End If
			End SyncLock

			Return Nothing

		''' <summary>
		''' release a service
		''' </summary>

		public void releaseService(BeanContextChild child, Object requestor, Object service)
			If child Is Nothing Then Throw New NullPointerException("child")
			If requestor Is Nothing Then Throw New NullPointerException("requestor")
			If service Is Nothing Then Throw New NullPointerException("service")

			Dim bcsc As BCSSChild

			SyncLock BeanContext.globalHierarchyLock
					SyncLock children
						bcsc = CType(children(child), BCSSChild)
					End SyncLock

					If bcsc IsNot Nothing Then
						bcsc.releaseService(requestor, service)
					Else
					   Throw New IllegalArgumentException("child actual is not a child of this BeanContext")
					End If
			End SyncLock

		''' <returns> an iterator for all the currently registered service classes. </returns>

		public IEnumerator currentServiceClasses
			Return New BCSIterator(services.Keys.GetEnumerator())

		''' <returns> an iterator for all the currently available service selectors
		''' (if any) available for the specified service. </returns>

		public IEnumerator getCurrentServiceSelectors(Class serviceClass)

			Dim bcsssp As BCSSServiceProvider = CType(services(serviceClass), BCSSServiceProvider)

			Return If(bcsssp IsNot Nothing, New BCSIterator(bcsssp.serviceProvider.getCurrentServiceSelectors(beanContextServicesPeer, serviceClass)), Nothing)

		''' <summary>
		''' BeanContextServicesListener callback, propagates event to all
		''' currently registered listeners and BeanContextServices children,
		''' if this BeanContextService does not already implement this service
		''' itself.
		''' 
		''' subclasses may override or envelope this method to implement their
		''' own propagation semantics.
		''' </summary>

		 public void serviceAvailable(BeanContextServiceAvailableEvent bcssae)
			SyncLock BeanContext.globalHierarchyLock
				If services.ContainsKey(bcssae.serviceClass) Then Return

				fireServiceAdded(bcssae)

				Dim i As IEnumerator

				SyncLock children
					i = children.Keys.GetEnumerator()
				End SyncLock

				Do While i.hasNext()
					Dim c As Object = i.next()

					If TypeOf c Is BeanContextServices Then CType(c, BeanContextServicesListener).serviceAvailable(bcssae)
				Loop
			End SyncLock

		''' <summary>
		''' BeanContextServicesListener callback, propagates event to all
		''' currently registered listeners and BeanContextServices children,
		''' if this BeanContextService does not already implement this service
		''' itself.
		''' 
		''' subclasses may override or envelope this method to implement their
		''' own propagation semantics.
		''' </summary>

		public void serviceRevoked(BeanContextServiceRevokedEvent bcssre)
			SyncLock BeanContext.globalHierarchyLock
				If services.ContainsKey(bcssre.serviceClass) Then Return

				fireServiceRevoked(bcssre)

				Dim i As IEnumerator

				SyncLock children
					i = children.Keys.GetEnumerator()
				End SyncLock

				Do While i.hasNext()
					Dim c As Object = i.next()

					If TypeOf c Is BeanContextServices Then CType(c, BeanContextServicesListener).serviceRevoked(bcssre)
				Loop
			End SyncLock

		''' <summary>
		''' Gets the <tt>BeanContextServicesListener</tt> (if any) of the specified
		''' child.
		''' </summary>
		''' <param name="child"> the specified child </param>
		''' <returns> the BeanContextServicesListener (if any) of the specified child </returns>
		protected static final BeanContextServicesListener getChildBeanContextServicesListener(Object child)
			Try
				Return CType(child, BeanContextServicesListener)
			Catch cce As ClassCastException
				Return Nothing
			End Try

		''' <summary>
		''' called from superclass child removal operations after a child
		''' has been successfully removed. called with child synchronized.
		''' 
		''' This subclass uses this hook to immediately revoke any services
		''' being used by this child if it is a BeanContextChild.
		''' 
		''' subclasses may envelope this method in order to implement their
		''' own child removal side-effects.
		''' </summary>

		protected void childJustRemovedHook(Object child, BCSChild bcsc)
			Dim bcssc As BCSSChild = CType(bcsc, BCSSChild)

			bcssc.cleanupReferences()

		''' <summary>
		''' called from setBeanContext to notify a BeanContextChild
		''' to release resources obtained from the nesting BeanContext.
		''' 
		''' This method revokes any services obtained from its parent.
		''' 
		''' subclasses may envelope this method to implement their own semantics.
		''' </summary>

		protected synchronized void releaseBeanContextResources()
			Dim bcssc As Object()

			MyBase.releaseBeanContextResources()

			SyncLock children
				If children.Count = 0 Then Return

				bcssc = children.Values.ToArray()
			End SyncLock


			For i As Integer = 0 To bcssc.Length - 1
				CType(bcssc(i), BCSSChild).revokeAllDelegatedServicesNow()
			Next i

			proxy = Nothing

		''' <summary>
		''' called from setBeanContext to notify a BeanContextChild
		''' to allocate resources obtained from the nesting BeanContext.
		''' 
		''' subclasses may envelope this method to implement their own semantics.
		''' </summary>

		protected synchronized void initializeBeanContextResources()
			MyBase.initializeBeanContextResources()

			Dim nbc As BeanContext = beanContext

			If nbc Is Nothing Then Return

			Try
				Dim bcs As BeanContextServices = CType(nbc, BeanContextServices)

				proxy = New BCSSProxyServiceProvider(Me, bcs)
			Catch cce As ClassCastException
				' do nothing ...
			End Try

		''' <summary>
		''' Fires a <tt>BeanContextServiceEvent</tt> notifying of a new service. </summary>
		''' <param name="serviceClass"> the service class </param>
		protected final void fireServiceAdded(Class serviceClass)
			Dim bcssae As New BeanContextServiceAvailableEvent(beanContextServicesPeer, serviceClass)

			fireServiceAdded(bcssae)

		''' <summary>
		''' Fires a <tt>BeanContextServiceAvailableEvent</tt> indicating that a new
		''' service has become available.
		''' </summary>
		''' <param name="bcssae"> the <tt>BeanContextServiceAvailableEvent</tt> </param>
		protected final void fireServiceAdded(BeanContextServiceAvailableEvent bcssae)
			Dim copy As Object()

			SyncLock bcsListeners
				copy = bcsListeners.ToArray()
			End SyncLock

			For i As Integer = 0 To copy.Length - 1
				CType(copy(i), BeanContextServicesListener).serviceAvailable(bcssae)
			Next i

		''' <summary>
		''' Fires a <tt>BeanContextServiceEvent</tt> notifying of a service being revoked.
		''' </summary>
		''' <param name="bcsre"> the <tt>BeanContextServiceRevokedEvent</tt> </param>
		protected final void fireServiceRevoked(BeanContextServiceRevokedEvent bcsre)
			Dim copy As Object()

			SyncLock bcsListeners
				copy = bcsListeners.ToArray()
			End SyncLock

			For i As Integer = 0 To copy.Length - 1
				CType(copy(i), BeanContextServiceRevokedListener).serviceRevoked(bcsre)
			Next i

		''' <summary>
		''' Fires a <tt>BeanContextServiceRevokedEvent</tt>
		''' indicating that a particular service is
		''' no longer available. </summary>
		''' <param name="serviceClass"> the service class </param>
		''' <param name="revokeNow"> whether or not the event should be revoked now </param>
		protected final void fireServiceRevoked(Class serviceClass, Boolean revokeNow)
			Dim copy As Object()
			Dim bcsre As New BeanContextServiceRevokedEvent(beanContextServicesPeer, serviceClass, revokeNow)

			SyncLock bcsListeners
				copy = bcsListeners.ToArray()
			End SyncLock

			For i As Integer = 0 To copy.Length - 1
				CType(copy(i), BeanContextServicesListener).serviceRevoked(bcsre)
			Next i

		''' <summary>
		''' called from BeanContextSupport writeObject before it serializes the
		''' children ...
		''' 
		''' This class will serialize any Serializable BeanContextServiceProviders
		''' herein.
		''' 
		''' subclasses may envelope this method to insert their own serialization
		''' processing that has to occur prior to serialization of the children
		''' </summary>

		protected synchronized void bcsPreSerializationHook(java.io.ObjectOutputStream oos) throws java.io.IOException

			oos.writeInt(serializable)

			If serializable <= 0 Then Return

			Dim count As Integer = 0

			Dim i As IEnumerator = services.GetEnumerator()

			Do While i.hasNext() AndAlso count < serializable
				Dim entry As DictionaryEntry = CType(i.next(), DictionaryEntry)
				Dim bcsp As BCSSServiceProvider = Nothing

				 Try
					bcsp = CType(entry.Value, BCSSServiceProvider)
				 Catch cce As ClassCastException
					Continue Do
				 End Try

				 If TypeOf bcsp.serviceProvider Is java.io.Serializable Then
					oos.writeObject(entry.Key)
					oos.writeObject(bcsp)
					count += 1
				 End If
			Loop

			If count <> serializable Then Throw New java.io.IOException("wrote different number of service providers than expected")

		''' <summary>
		''' called from BeanContextSupport readObject before it deserializes the
		''' children ...
		''' 
		''' This class will deserialize any Serializable BeanContextServiceProviders
		''' serialized earlier thus making them available to the children when they
		''' deserialized.
		''' 
		''' subclasses may envelope this method to insert their own serialization
		''' processing that has to occur prior to serialization of the children
		''' </summary>

		protected synchronized void bcsPreDeserializationHook(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException

			serializable = ois.readInt()

			Dim count As Integer = serializable

			Do While count > 0
				services(ois.readObject()) = ois.readObject()
				count -= 1
			Loop

		''' <summary>
		''' serialize the instance
		''' </summary>

		private synchronized void writeObject(java.io.ObjectOutputStream oos) throws java.io.IOException
			oos.defaultWriteObject()

			serialize(oos, CType(bcsListeners, ICollection))

		''' <summary>
		''' deserialize the instance
		''' </summary>

		private synchronized void readObject(java.io.ObjectInputStream ois) throws java.io.IOException, ClassNotFoundException

			ois.defaultReadObject()

			deserialize(ois, CType(bcsListeners, ICollection))


	'    
	'     * fields
	'     

		''' <summary>
		''' all accesses to the <code> protected transient HashMap services </code>
		''' field should be synchronized on that object
		''' </summary>
		protected transient Hashtable services

		''' <summary>
		''' The number of instances of a serializable <tt>BeanContextServceProvider</tt>.
		''' </summary>
		protected transient Integer serializable = 0


		''' <summary>
		''' Delegate for the <tt>BeanContextServiceProvider</tt>.
		''' </summary>
		protected transient BCSSProxyServiceProvider proxy


		''' <summary>
		''' List of <tt>BeanContextServicesListener</tt> objects.
		''' </summary>
		protected transient ArrayList bcsListeners
	End Class

End Namespace
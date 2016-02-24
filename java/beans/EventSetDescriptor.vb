'
' * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	''' <summary>
	''' An EventSetDescriptor describes a group of events that a given Java
	''' bean fires.
	''' <P>
	''' The given group of events are all delivered as method calls on a single
	''' event listener interface, and an event listener object can be registered
	''' via a call on a registration method supplied by the event source.
	''' </summary>
	Public Class EventSetDescriptor
		Inherits FeatureDescriptor

		Private listenerMethodDescriptors As MethodDescriptor()
		Private addMethodDescriptor As MethodDescriptor
		Private removeMethodDescriptor As MethodDescriptor
		Private getMethodDescriptor As MethodDescriptor

		Private listenerMethodsRef As Reference(Of Method())
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private listenerTypeRef As Reference(Of ? As Class)

		Private unicast As Boolean
		Private inDefaultEventSet As Boolean = True

		''' <summary>
		''' Creates an <TT>EventSetDescriptor</TT> assuming that you are
		''' following the most simple standard design pattern where a named
		''' event &quot;fred&quot; is (1) delivered as a call on the single method of
		''' interface FredListener, (2) has a single argument of type FredEvent,
		''' and (3) where the FredListener may be registered with a call on an
		''' addFredListener method of the source component and removed with a
		''' call on a removeFredListener method.
		''' </summary>
		''' <param name="sourceClass">  The class firing the event. </param>
		''' <param name="eventSetName">  The programmatic name of the event.  E.g. &quot;fred&quot;.
		'''          Note that this should normally start with a lower-case character. </param>
		''' <param name="listenerType">  The target interface that events
		'''          will get delivered to. </param>
		''' <param name="listenerMethodName">  The method that will get called when the event gets
		'''          delivered to its target listener interface. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(ByVal sourceClass As Class, ByVal eventSetName As String, ByVal listenerType As Class, ByVal listenerMethodName As String)
			Me.New(sourceClass, eventSetName, listenerType, New String() { listenerMethodName }, Introspector.ADD_PREFIX + getListenerClassName(listenerType), Introspector.REMOVE_PREFIX + getListenerClassName(listenerType), Introspector.GET_PREFIX + getListenerClassName(listenerType) & "s")

			Dim eventName As String = NameGenerator.capitalize(eventSetName) & "Event"
			Dim listenerMethods_Renamed As Method() = listenerMethods
			If listenerMethods_Renamed.Length > 0 Then
				Dim args As Class() = getParameterTypes(class0, listenerMethods_Renamed(0))
				' Check for EventSet compliance. Special case for vetoableChange. See 4529996
				If (Not "vetoableChange".Equals(eventSetName)) AndAlso (Not args(0).name.EndsWith(eventName)) Then Throw New IntrospectionException("Method """ & listenerMethodName & """ should have argument """ & eventName & """")
			End If
		End Sub

		Private Shared Function getListenerClassName(ByVal cls As Class) As String
			Dim className As String = cls.name
			Return className.Substring(className.LastIndexOf("."c) + 1)
		End Function

		''' <summary>
		''' Creates an <TT>EventSetDescriptor</TT> from scratch using
		''' string names.
		''' </summary>
		''' <param name="sourceClass">  The class firing the event. </param>
		''' <param name="eventSetName"> The programmatic name of the event set.
		'''          Note that this should normally start with a lower-case character. </param>
		''' <param name="listenerType">  The Class of the target interface that events
		'''          will get delivered to. </param>
		''' <param name="listenerMethodNames"> The names of the methods that will get called
		'''          when the event gets delivered to its target listener interface. </param>
		''' <param name="addListenerMethodName">  The name of the method on the event source
		'''          that can be used to register an event listener object. </param>
		''' <param name="removeListenerMethodName">  The name of the method on the event source
		'''          that can be used to de-register an event listener object. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String listenerMethodNames() , String addListenerMethodName, String removeListenerMethodName) throws IntrospectionException
			Me(sourceClass, eventSetName, listenerType, listenerMethodNames, addListenerMethodName, removeListenerMethodName, Nothing)

		''' <summary>
		''' This constructor creates an EventSetDescriptor from scratch using
		''' string names.
		''' </summary>
		''' <param name="sourceClass">  The class firing the event. </param>
		''' <param name="eventSetName"> The programmatic name of the event set.
		'''          Note that this should normally start with a lower-case character. </param>
		''' <param name="listenerType">  The Class of the target interface that events
		'''          will get delivered to. </param>
		''' <param name="listenerMethodNames"> The names of the methods that will get called
		'''          when the event gets delivered to its target listener interface. </param>
		''' <param name="addListenerMethodName">  The name of the method on the event source
		'''          that can be used to register an event listener object. </param>
		''' <param name="removeListenerMethodName">  The name of the method on the event source
		'''          that can be used to de-register an event listener object. </param>
		''' <param name="getListenerMethodName"> The method on the event source that
		'''          can be used to access the array of event listener objects. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection.
		''' @since 1.4 </exception>
		public EventSetDescriptor(Class sourceClass, String eventSetName, Class listenerType, String listenerMethodNames() , String addListenerMethodName, String removeListenerMethodName, String getListenerMethodName) throws IntrospectionException
			If sourceClass Is Nothing OrElse eventSetName Is Nothing OrElse listenerType Is Nothing Then Throw New NullPointerException
			name = eventSetName
			class0 = sourceClass
			listenerType = listenerType

			Dim listenerMethods_Renamed As Method() = New Method(listenerMethodNames.length - 1){}
			For i As Integer = 0 To listenerMethodNames.length - 1
				' Check for null names
				If listenerMethodNames(i) Is Nothing Then Throw New NullPointerException
				listenerMethods_Renamed(i) = getMethod(listenerType, listenerMethodNames(i), 1)
			Next i
			listenerMethods = listenerMethods_Renamed

			addListenerMethod = getMethod(sourceClass, addListenerMethodName, 1)
			removeListenerMethod = getMethod(sourceClass, removeListenerMethodName, 1)

			' Be more forgiving of not finding the getListener method.
			Dim method_Renamed As Method = Introspector.findMethod(sourceClass, getListenerMethodName, 0)
			If method_Renamed IsNot Nothing Then getListenerMethod = method_Renamed

		private static Method getMethod(Class cls, String name, Integer args) throws IntrospectionException
			If name Is Nothing Then Return Nothing
			Dim method_Renamed As Method = Introspector.findMethod(cls, name, args)
			If (method_Renamed Is Nothing) OrElse Modifier.isStatic(method_Renamed.modifiers) Then Throw New IntrospectionException("Method not found: " & name & " on class " & cls.name)
			Return method_Renamed

		''' <summary>
		''' Creates an <TT>EventSetDescriptor</TT> from scratch using
		''' <TT>java.lang.reflect.Method</TT> and <TT>java.lang.Class</TT> objects.
		''' </summary>
		''' <param name="eventSetName"> The programmatic name of the event set. </param>
		''' <param name="listenerType"> The Class for the listener interface. </param>
		''' <param name="listenerMethods">  An array of Method objects describing each
		'''          of the event handling methods in the target listener. </param>
		''' <param name="addListenerMethod">  The method on the event source
		'''          that can be used to register an event listener object. </param>
		''' <param name="removeListenerMethod">  The method on the event source
		'''          that can be used to de-register an event listener object. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		public EventSetDescriptor(String eventSetName, Class listenerType, Method listenerMethods() , Method addListenerMethod, Method removeListenerMethod) throws IntrospectionException
			Me(eventSetName, listenerType, listenerMethods, addListenerMethod, removeListenerMethod, Nothing)

		''' <summary>
		''' This constructor creates an EventSetDescriptor from scratch using
		''' java.lang.reflect.Method and java.lang.Class objects.
		''' </summary>
		''' <param name="eventSetName"> The programmatic name of the event set. </param>
		''' <param name="listenerType"> The Class for the listener interface. </param>
		''' <param name="listenerMethods">  An array of Method objects describing each
		'''          of the event handling methods in the target listener. </param>
		''' <param name="addListenerMethod">  The method on the event source
		'''          that can be used to register an event listener object. </param>
		''' <param name="removeListenerMethod">  The method on the event source
		'''          that can be used to de-register an event listener object. </param>
		''' <param name="getListenerMethod"> The method on the event source
		'''          that can be used to access the array of event listener objects. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection.
		''' @since 1.4 </exception>
		public EventSetDescriptor(String eventSetName, Class listenerType, Method listenerMethods() , Method addListenerMethod, Method removeListenerMethod, Method getListenerMethod) throws IntrospectionException
			name = eventSetName
			listenerMethods = listenerMethods
			addListenerMethod = addListenerMethod
			removeListenerMethod = removeListenerMethod
			getListenerMethod = getListenerMethod
			listenerType = listenerType

		''' <summary>
		''' Creates an <TT>EventSetDescriptor</TT> from scratch using
		''' <TT>java.lang.reflect.MethodDescriptor</TT> and <TT>java.lang.Class</TT>
		'''  objects.
		''' </summary>
		''' <param name="eventSetName"> The programmatic name of the event set. </param>
		''' <param name="listenerType"> The Class for the listener interface. </param>
		''' <param name="listenerMethodDescriptors">  An array of MethodDescriptor objects
		'''           describing each of the event handling methods in the
		'''           target listener. </param>
		''' <param name="addListenerMethod">  The method on the event source
		'''          that can be used to register an event listener object. </param>
		''' <param name="removeListenerMethod">  The method on the event source
		'''          that can be used to de-register an event listener object. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		public EventSetDescriptor(String eventSetName, Class listenerType, MethodDescriptor listenerMethodDescriptors() , Method addListenerMethod, Method removeListenerMethod) throws IntrospectionException
			name = eventSetName
			Me.listenerMethodDescriptors = If(listenerMethodDescriptors IsNot Nothing, listenerMethodDescriptors.clone(), Nothing)
			addListenerMethod = addListenerMethod
			removeListenerMethod = removeListenerMethod
			listenerType = listenerType

		''' <summary>
		''' Gets the <TT>Class</TT> object for the target interface.
		''' </summary>
		''' <returns> The Class object for the target interface that will
		''' get invoked when the event is fired. </returns>
		public Class listenerType
			Return If(Me.listenerTypeRef IsNot Nothing, Me.listenerTypeRef.get(), Nothing)

		private void listenerTypeype(Class cls)
			Me.listenerTypeRef = getWeakReference(cls)

		''' <summary>
		''' Gets the methods of the target listener interface.
		''' </summary>
		''' <returns> An array of <TT>Method</TT> objects for the target methods
		''' within the target listener interface that will get called when
		''' events are fired. </returns>
		public synchronized Method() listenerMethods
			Dim methods As Method() = listenerMethods0
			If methods Is Nothing Then
				If listenerMethodDescriptors IsNot Nothing Then
					methods = New Method(listenerMethodDescriptors.Length - 1){}
					For i As Integer = 0 To methods.Length - 1
						methods(i) = listenerMethodDescriptors(i).method
					Next i
				End If
				listenerMethods = methods
			End If
			Return methods

		private void listenerMethodsods(Method() methods)
			If methods Is Nothing Then Return
			If listenerMethodDescriptors Is Nothing Then
				listenerMethodDescriptors = New MethodDescriptor(methods.length - 1){}
				For i As Integer = 0 To methods.length - 1
					listenerMethodDescriptors(i) = New MethodDescriptor(methods(i))
				Next i
			End If
			Me.listenerMethodsRef = getSoftReference(methods)

		private Method() listenerMethods0
			Return If(Me.listenerMethodsRef IsNot Nothing, Me.listenerMethodsRef.get(), Nothing)

		''' <summary>
		''' Gets the <code>MethodDescriptor</code>s of the target listener interface.
		''' </summary>
		''' <returns> An array of <code>MethodDescriptor</code> objects for the target methods
		''' within the target listener interface that will get called when
		''' events are fired. </returns>
		public synchronized MethodDescriptor() listenerMethodDescriptors
			Return If(Me.listenerMethodDescriptors IsNot Nothing, Me.listenerMethodDescriptors.clone(), Nothing)

		''' <summary>
		''' Gets the method used to add event listeners.
		''' </summary>
		''' <returns> The method used to register a listener at the event source. </returns>
		public synchronized Method addListenerMethod
			Return getMethod(Me.addMethodDescriptor)

		private synchronized void addListenerMethodhod(Method method)
			If method Is Nothing Then Return
			If class0 Is Nothing Then class0 = method.declaringClass
			addMethodDescriptor = New MethodDescriptor(method)
			transient = method.getAnnotation(GetType(Transient))

		''' <summary>
		''' Gets the method used to remove event listeners.
		''' </summary>
		''' <returns> The method used to remove a listener at the event source. </returns>
		public synchronized Method removeListenerMethod
			Return getMethod(Me.removeMethodDescriptor)

		private synchronized void removeListenerMethodhod(Method method)
			If method Is Nothing Then Return
			If class0 Is Nothing Then class0 = method.declaringClass
			removeMethodDescriptor = New MethodDescriptor(method)
			transient = method.getAnnotation(GetType(Transient))

		''' <summary>
		''' Gets the method used to access the registered event listeners.
		''' </summary>
		''' <returns> The method used to access the array of listeners at the event
		'''         source or null if it doesn't exist.
		''' @since 1.4 </returns>
		public synchronized Method getListenerMethod
			Return getMethod(Me.getMethodDescriptor)

		private synchronized void getListenerMethodhod(Method method)
			If method Is Nothing Then Return
			If class0 Is Nothing Then class0 = method.declaringClass
			getMethodDescriptor = New MethodDescriptor(method)
			transient = method.getAnnotation(GetType(Transient))

		''' <summary>
		''' Mark an event set as unicast (or not).
		''' </summary>
		''' <param name="unicast">  True if the event set is unicast. </param>
		public void unicastast(Boolean unicast)
			Me.unicast = unicast

		''' <summary>
		''' Normally event sources are multicast.  However there are some
		''' exceptions that are strictly unicast.
		''' </summary>
		''' <returns>  <TT>true</TT> if the event set is unicast.
		'''          Defaults to <TT>false</TT>. </returns>
		public Boolean unicast
			Return unicast

		''' <summary>
		''' Marks an event set as being in the &quot;default&quot; set (or not).
		''' By default this is <TT>true</TT>.
		''' </summary>
		''' <param name="inDefaultEventSet"> <code>true</code> if the event set is in
		'''                          the &quot;default&quot; set,
		'''                          <code>false</code> if not </param>
		public void inDefaultEventSetSet(Boolean inDefaultEventSet)
			Me.inDefaultEventSet = inDefaultEventSet

		''' <summary>
		''' Reports if an event set is in the &quot;default&quot; set.
		''' </summary>
		''' <returns>  <TT>true</TT> if the event set is in
		'''          the &quot;default&quot; set.  Defaults to <TT>true</TT>. </returns>
		public Boolean inDefaultEventSet
			Return inDefaultEventSet

	'    
	'     * Package-private constructor
	'     * Merge two event set descriptors.  Where they conflict, give the
	'     * second argument (y) priority over the first argument (x).
	'     *
	'     * @param x  The first (lower priority) EventSetDescriptor
	'     * @param y  The second (higher priority) EventSetDescriptor
	'     
		EventSetDescriptor(EventSetDescriptor x, EventSetDescriptor y)
			MyBase(x,y)
			listenerMethodDescriptors = x.listenerMethodDescriptors
			If y.listenerMethodDescriptors IsNot Nothing Then listenerMethodDescriptors = y.listenerMethodDescriptors

			listenerTypeRef = x.listenerTypeRef
			If y.listenerTypeRef IsNot Nothing Then listenerTypeRef = y.listenerTypeRef

			addMethodDescriptor = x.addMethodDescriptor
			If y.addMethodDescriptor IsNot Nothing Then addMethodDescriptor = y.addMethodDescriptor

			removeMethodDescriptor = x.removeMethodDescriptor
			If y.removeMethodDescriptor IsNot Nothing Then removeMethodDescriptor = y.removeMethodDescriptor

			getMethodDescriptor = x.getMethodDescriptor
			If y.getMethodDescriptor IsNot Nothing Then getMethodDescriptor = y.getMethodDescriptor

			unicast = y.unicast
			If (Not x.inDefaultEventSet) OrElse (Not y.inDefaultEventSet) Then inDefaultEventSet = False

	'    
	'     * Package-private dup constructor
	'     * This must isolate the new object from any changes to the old object.
	'     
		EventSetDescriptor(EventSetDescriptor old)
			MyBase(old)
			If old.listenerMethodDescriptors IsNot Nothing Then
				Dim len As Integer = old.listenerMethodDescriptors.length
				listenerMethodDescriptors = New MethodDescriptor(len - 1){}
				For i As Integer = 0 To len - 1
					listenerMethodDescriptors(i) = New MethodDescriptor(old.listenerMethodDescriptors(i))
				Next i
			End If
			listenerTypeRef = old.listenerTypeRef

			addMethodDescriptor = old.addMethodDescriptor
			removeMethodDescriptor = old.removeMethodDescriptor
			getMethodDescriptor = old.getMethodDescriptor

			unicast = old.unicast
			inDefaultEventSet = old.inDefaultEventSet

		void appendTo(StringBuilder sb)
			appendTo(sb, "unicast", Me.unicast)
			appendTo(sb, "inDefaultEventSet", Me.inDefaultEventSet)
			appendTo(sb, "listenerType", Me.listenerTypeRef)
			appendTo(sb, "getListenerMethod", getMethod(Me.getMethodDescriptor))
			appendTo(sb, "addListenerMethod", getMethod(Me.addMethodDescriptor))
			appendTo(sb, "removeListenerMethod", getMethod(Me.removeMethodDescriptor))

		private static Method getMethod(MethodDescriptor descriptor)
			Return If(descriptor IsNot Nothing, descriptor.method, Nothing)
	End Class

End Namespace
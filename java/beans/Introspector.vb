Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' The Introspector class provides a standard way for tools to learn about
	''' the properties, events, and methods supported by a target Java Bean.
	''' <p>
	''' For each of those three kinds of information, the Introspector will
	''' separately analyze the bean's class and superclasses looking for
	''' either explicit or implicit information and use that information to
	''' build a BeanInfo object that comprehensively describes the target bean.
	''' <p>
	''' For each class "Foo", explicit information may be available if there exists
	''' a corresponding "FooBeanInfo" class that provides a non-null value when
	''' queried for the information.   We first look for the BeanInfo class by
	''' taking the full package-qualified name of the target bean class and
	''' appending "BeanInfo" to form a new class name.  If this fails, then
	''' we take the final classname component of this name, and look for that
	''' class in each of the packages specified in the BeanInfo package search
	''' path.
	''' <p>
	''' Thus for a class such as "sun.xyz.OurButton" we would first look for a
	''' BeanInfo class called "sun.xyz.OurButtonBeanInfo" and if that failed we'd
	''' look in each package in the BeanInfo search path for an OurButtonBeanInfo
	''' class.  With the default search path, this would mean looking for
	''' "sun.beans.infos.OurButtonBeanInfo".
	''' <p>
	''' If a class provides explicit BeanInfo about itself then we add that to
	''' the BeanInfo information we obtained from analyzing any derived classes,
	''' but we regard the explicit information as being definitive for the current
	''' class and its base classes, and do not proceed any further up the superclass
	''' chain.
	''' <p>
	''' If we don't find explicit BeanInfo on a [Class], we use low-level
	''' reflection to study the methods of the class and apply standard design
	''' patterns to identify property accessors, event sources, or public
	''' methods.  We then proceed to analyze the class's superclass and add
	''' in the information from it (and possibly on up the superclass chain).
	''' <p>
	''' For more information about introspection and design patterns, please
	''' consult the
	'''  <a href="http://www.oracle.com/technetwork/java/javase/documentation/spec-136004.html">JavaBeans&trade; specification</a>.
	''' </summary>

	Public Class Introspector

		' Flags that can be used to control getBeanInfo:
		''' <summary>
		''' Flag to indicate to use of all beaninfo.
		''' </summary>
		Public Const USE_ALL_BEANINFO As Integer = 1
		''' <summary>
		''' Flag to indicate to ignore immediate beaninfo.
		''' </summary>
		Public Const IGNORE_IMMEDIATE_BEANINFO As Integer = 2
		''' <summary>
		''' Flag to indicate to ignore all beaninfo.
		''' </summary>
		Public Const IGNORE_ALL_BEANINFO As Integer = 3

		' Static Caches to speed up introspection.
		Private Shared ReadOnly declaredMethodCache As New com.sun.beans.WeakCache(Of [Class], Method())

		Private beanClass As  [Class]
		Private explicitBeanInfo As BeanInfo
		Private superBeanInfo As BeanInfo
		Private additionalBeanInfo As BeanInfo()

		Private propertyChangeSource As Boolean = False
		Private Shared eventListenerType As  [Class] = GetType(java.util.EventListener)

		' These should be removed.
		Private defaultEventName As String
		Private defaultPropertyName As String
		Private defaultEventIndex As Integer = -1
		Private defaultPropertyIndex As Integer = -1

		' Methods maps from Method names to MethodDescriptors
		Private methods As IDictionary(Of String, MethodDescriptor)

		' properties maps from String names to PropertyDescriptors
		Private properties As IDictionary(Of String, PropertyDescriptor)

		' events maps from String names to EventSetDescriptors
		Private events As IDictionary(Of String, EventSetDescriptor)

		Private Shared ReadOnly EMPTY_EVENTSETDESCRIPTORS As EventSetDescriptor() = New EventSetDescriptor(){}

		Friend Const ADD_PREFIX As String = "add"
		Friend Const REMOVE_PREFIX As String = "remove"
		Friend Const GET_PREFIX As String = "get"
		Friend Const SET_PREFIX As String = "set"
		Friend Const IS_PREFIX As String = "is"

		'======================================================================
		'                          Public methods
		'======================================================================

		''' <summary>
		''' Introspect on a Java Bean and learn about all its properties, exposed
		''' methods, and events.
		''' <p>
		''' If the BeanInfo class for a Java Bean has been previously Introspected
		''' then the BeanInfo class is retrieved from the BeanInfo cache.
		''' </summary>
		''' <param name="beanClass">  The bean class to be analyzed. </param>
		''' <returns>  A BeanInfo object describing the target bean. </returns>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		''' <seealso cref= #flushCaches </seealso>
		''' <seealso cref= #flushFromCaches </seealso>
		Public Shared Function getBeanInfo(ByVal beanClass As [Class]) As BeanInfo
			If Not sun.reflect.misc.ReflectUtil.isPackageAccessible(beanClass) Then Return (New Introspector(beanClass, Nothing, USE_ALL_BEANINFO)).beanInfo
			Dim context As ThreadGroupContext = ThreadGroupContext.context
			Dim beanInfo_Renamed As BeanInfo
			SyncLock declaredMethodCache
				beanInfo_Renamed = context.getBeanInfo(beanClass)
			End SyncLock
			If beanInfo_Renamed Is Nothing Then
				beanInfo_Renamed = (New Introspector(beanClass, Nothing, USE_ALL_BEANINFO)).beanInfo
				SyncLock declaredMethodCache
					context.putBeanInfo(beanClass, beanInfo_Renamed)
				End SyncLock
			End If
			Return beanInfo_Renamed
		End Function

		''' <summary>
		''' Introspect on a Java bean and learn about all its properties, exposed
		''' methods, and events, subject to some control flags.
		''' <p>
		''' If the BeanInfo class for a Java Bean has been previously Introspected
		''' based on the same arguments then the BeanInfo class is retrieved
		''' from the BeanInfo cache.
		''' </summary>
		''' <param name="beanClass">  The bean class to be analyzed. </param>
		''' <param name="flags">  Flags to control the introspection.
		'''     If flags == USE_ALL_BEANINFO then we use all of the BeanInfo
		'''          classes we can discover.
		'''     If flags == IGNORE_IMMEDIATE_BEANINFO then we ignore any
		'''           BeanInfo associated with the specified beanClass.
		'''     If flags == IGNORE_ALL_BEANINFO then we ignore all BeanInfo
		'''           associated with the specified beanClass or any of its
		'''           parent classes. </param>
		''' <returns>  A BeanInfo object describing the target bean. </returns>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Shared Function getBeanInfo(ByVal beanClass As [Class], ByVal flags As Integer) As BeanInfo
			Return getBeanInfo(beanClass, Nothing, flags)
		End Function

		''' <summary>
		''' Introspect on a Java bean and learn all about its properties, exposed
		''' methods, below a given "stop" point.
		''' <p>
		''' If the BeanInfo class for a Java Bean has been previously Introspected
		''' based on the same arguments, then the BeanInfo class is retrieved
		''' from the BeanInfo cache. </summary>
		''' <returns> the BeanInfo for the bean </returns>
		''' <param name="beanClass"> The bean class to be analyzed. </param>
		''' <param name="stopClass"> The baseclass at which to stop the analysis.  Any
		'''    methods/properties/events in the stopClass or in its baseclasses
		'''    will be ignored in the analysis. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Shared Function getBeanInfo(ByVal beanClass As [Class], ByVal stopClass As [Class]) As BeanInfo
			Return getBeanInfo(beanClass, stopClass, USE_ALL_BEANINFO)
		End Function

		''' <summary>
		''' Introspect on a Java Bean and learn about all its properties,
		''' exposed methods and events, below a given {@code stopClass} point
		''' subject to some control {@code flags}.
		''' <dl>
		'''  <dt>USE_ALL_BEANINFO</dt>
		'''  <dd>Any BeanInfo that can be discovered will be used.</dd>
		'''  <dt>IGNORE_IMMEDIATE_BEANINFO</dt>
		'''  <dd>Any BeanInfo associated with the specified {@code beanClass} will be ignored.</dd>
		'''  <dt>IGNORE_ALL_BEANINFO</dt>
		'''  <dd>Any BeanInfo associated with the specified {@code beanClass}
		'''      or any of its parent classes will be ignored.</dd>
		''' </dl>
		''' Any methods/properties/events in the {@code stopClass}
		''' or in its parent classes will be ignored in the analysis.
		''' <p>
		''' If the BeanInfo class for a Java Bean has been
		''' previously introspected based on the same arguments then
		''' the BeanInfo class is retrieved from the BeanInfo cache.
		''' </summary>
		''' <param name="beanClass">  the bean class to be analyzed </param>
		''' <param name="stopClass">  the parent class at which to stop the analysis </param>
		''' <param name="flags">      flags to control the introspection </param>
		''' <returns> a BeanInfo object describing the target bean </returns>
		''' <exception cref="IntrospectionException"> if an exception occurs during introspection
		''' 
		''' @since 1.7 </exception>
		Public Shared Function getBeanInfo(ByVal beanClass As [Class], ByVal stopClass As [Class], ByVal flags As Integer) As BeanInfo
			Dim bi As BeanInfo
			If stopClass Is Nothing AndAlso flags = USE_ALL_BEANINFO Then
				' Same parameters to take advantage of caching.
				bi = getBeanInfo(beanClass)
			Else
				bi = (New Introspector(beanClass, stopClass, flags)).beanInfo
			End If
			Return bi

			' Old behaviour: Make an independent copy of the BeanInfo.
			'return new GenericBeanInfo(bi);
		End Function


		''' <summary>
		''' Utility method to take a string and convert it to normal Java variable
		''' name capitalization.  This normally means converting the first
		''' character from upper case to lower case, but in the (unusual) special
		''' case when there is more than one character and both the first and
		''' second characters are upper case, we leave it alone.
		''' <p>
		''' Thus "FooBah" becomes "fooBah" and "X" becomes "x", but "URL" stays
		''' as "URL".
		''' </summary>
		''' <param name="name"> The string to be decapitalized. </param>
		''' <returns>  The decapitalized version of the string. </returns>
		Public Shared Function decapitalize(ByVal name As String) As String
			If name Is Nothing OrElse name.length() = 0 Then Return name
			If name.length() > 1 AndAlso Char.IsUpper(name.Chars(1)) AndAlso Char.IsUpper(name.Chars(0)) Then Return name
			Dim chars As Char() = name.ToCharArray()
			chars(0) = Char.ToLower(chars(0))
			Return New String(chars)
		End Function

		''' <summary>
		''' Gets the list of package names that will be used for
		'''          finding BeanInfo classes.
		''' </summary>
		''' <returns>  The array of package names that will be searched in
		'''          order to find BeanInfo classes. The default value
		'''          for this array is implementation-dependent; e.g.
		'''          Sun implementation initially sets to {"sun.beans.infos"}. </returns>

		Public Property Shared beanInfoSearchPath As String()
			Get
				Return ThreadGroupContext.context.beanInfoFinder.packages
			End Get
			Set(ByVal path As String())
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPropertiesAccess()
				ThreadGroupContext.context.beanInfoFinder.packages = path
			End Set
		End Property




		''' <summary>
		''' Flush all of the Introspector's internal caches.  This method is
		''' not normally required.  It is normally only needed by advanced
		''' tools that update existing "Class" objects in-place and need
		''' to make the Introspector re-analyze existing Class objects.
		''' </summary>

		Public Shared Sub flushCaches()
			SyncLock declaredMethodCache
				ThreadGroupContext.context.clearBeanInfoCache()
				declaredMethodCache.clear()
			End SyncLock
		End Sub

		''' <summary>
		''' Flush the Introspector's internal cached information for a given class.
		''' This method is not normally required.  It is normally only needed
		''' by advanced tools that update existing "Class" objects in-place
		''' and need to make the Introspector re-analyze an existing Class object.
		''' 
		''' Note that only the direct state associated with the target Class
		''' object is flushed.  We do not flush state for other Class objects
		''' with the same name, nor do we flush state for any related Class
		''' objects (such as subclasses), even though their state may include
		''' information indirectly obtained from the target Class object.
		''' </summary>
		''' <param name="clz">  Class object to be flushed. </param>
		''' <exception cref="NullPointerException"> If the Class object is null. </exception>
		Public Shared Sub flushFromCaches(ByVal clz As [Class])
			If clz Is Nothing Then Throw New NullPointerException
			SyncLock declaredMethodCache
				ThreadGroupContext.context.removeBeanInfo(clz)
				declaredMethodCache.put(clz, Nothing)
			End SyncLock
		End Sub

		'======================================================================
		'                  Private implementation methods
		'======================================================================

		Private Sub New(ByVal beanClass As [Class], ByVal stopClass As [Class], ByVal flags As Integer)
			Me.beanClass = beanClass

			' Check stopClass is a superClass of startClass.
			If stopClass IsNot Nothing Then
				Dim isSuper As Boolean = False
				Dim c As  [Class] = beanClass.BaseType
				Do While c IsNot Nothing
					If c Is stopClass Then isSuper = True
					c = c.BaseType
				Loop
				If Not isSuper Then Throw New IntrospectionException(stopClass.name & " not superclass of " & beanClass.name)
			End If

			If flags = USE_ALL_BEANINFO Then explicitBeanInfo = findExplicitBeanInfo(beanClass)

			Dim superClass As  [Class] = beanClass.BaseType
			If superClass IsNot stopClass Then
				Dim newFlags As Integer = flags
				If newFlags = IGNORE_IMMEDIATE_BEANINFO Then newFlags = USE_ALL_BEANINFO
				superBeanInfo = getBeanInfo(superClass, stopClass, newFlags)
			End If
			If explicitBeanInfo IsNot Nothing Then additionalBeanInfo = explicitBeanInfo.additionalBeanInfo
			If additionalBeanInfo Is Nothing Then additionalBeanInfo = New BeanInfo(){}
		End Sub

		''' <summary>
		''' Constructs a GenericBeanInfo class from the state of the Introspector
		''' </summary>
		Private Property beanInfo As BeanInfo
			Get
    
				' the evaluation order here is import, as we evaluate the
				' event sets and locate PropertyChangeListeners before we
				' look for properties.
				Dim bd As BeanDescriptor = targetBeanDescriptor
				Dim mds As MethodDescriptor() = targetMethodInfo
				Dim esds As EventSetDescriptor() = targetEventInfo
				Dim pds As PropertyDescriptor() = targetPropertyInfo
    
				Dim defaultEvent As Integer = targetDefaultEventIndex
				Dim defaultProperty As Integer = targetDefaultPropertyIndex
    
				Return New GenericBeanInfo(bd, esds, defaultEvent, pds, defaultProperty, mds, explicitBeanInfo)
    
			End Get
		End Property

		''' <summary>
		''' Looks for an explicit BeanInfo class that corresponds to the Class.
		''' First it looks in the existing package that the Class is defined in,
		''' then it checks to see if the class is its own BeanInfo. Finally,
		''' the BeanInfo search path is prepended to the class and searched.
		''' </summary>
		''' <param name="beanClass">  the class type of the bean </param>
		''' <returns> Instance of an explicit BeanInfo class or null if one isn't found. </returns>
		Private Shared Function findExplicitBeanInfo(ByVal beanClass As [Class]) As BeanInfo
			Return ThreadGroupContext.context.beanInfoFinder.find(beanClass)
		End Function

		''' <returns> An array of PropertyDescriptors describing the editable
		''' properties supported by the target bean. </returns>

		Private Property targetPropertyInfo As PropertyDescriptor()
			Get
    
				' Check if the bean has its own BeanInfo that will provide
				' explicit information.
				Dim explicitProperties As PropertyDescriptor() = Nothing
				If explicitBeanInfo IsNot Nothing Then explicitProperties = getPropertyDescriptors(Me.explicitBeanInfo)
    
				If explicitProperties Is Nothing AndAlso superBeanInfo IsNot Nothing Then addPropertyDescriptors(getPropertyDescriptors(Me.superBeanInfo))
    
				For i As Integer = 0 To additionalBeanInfo.Length - 1
					addPropertyDescriptors(additionalBeanInfo(i).propertyDescriptors)
				Next i
    
				If explicitProperties IsNot Nothing Then
					' Add the explicit BeanInfo data to our results.
					addPropertyDescriptors(explicitProperties)
    
				Else
    
					' Apply some reflection to the current class.
    
					' First get an array of all the public methods at this level
					Dim methodList As Method() = getPublicDeclaredMethods(beanClass)
    
					' Now analyze each method.
					For i As Integer = 0 To methodList.Length - 1
						Dim method As Method = methodList(i)
						If method Is Nothing Then Continue For
						' skip static methods.
						Dim mods As Integer = method.modifiers
						If Modifier.isStatic(mods) Then Continue For
						Dim name As String = method.name
						Dim argTypes As  [Class]() = method.parameterTypes
						Dim resultType As  [Class] = method.returnType
						Dim argCount As Integer = argTypes.Length
						Dim pd As PropertyDescriptor = Nothing
    
						If name.length() <= 3 AndAlso (Not name.StartsWith(IS_PREFIX)) Then Continue For
    
						Try
    
							If argCount = 0 Then
								If name.StartsWith(GET_PREFIX) Then
									' Simple getter
									pd = New PropertyDescriptor(Me.beanClass, name.Substring(3), method, Nothing)
								ElseIf resultType Is GetType(Boolean) AndAlso name.StartsWith(IS_PREFIX) Then
									' Boolean getter
									pd = New PropertyDescriptor(Me.beanClass, name.Substring(2), method, Nothing)
								End If
							ElseIf argCount = 1 Then
								If GetType(Integer).Equals(argTypes(0)) AndAlso name.StartsWith(GET_PREFIX) Then
									pd = New IndexedPropertyDescriptor(Me.beanClass, name.Substring(3), Nothing, Nothing, method, Nothing)
								ElseIf GetType(void).Equals(resultType) AndAlso name.StartsWith(SET_PREFIX) Then
									' Simple setter
									pd = New PropertyDescriptor(Me.beanClass, name.Substring(3), Nothing, method)
									If throwsException(method, GetType(PropertyVetoException)) Then pd.constrained = True
								End If
							ElseIf argCount = 2 Then
									If GetType(void).Equals(resultType) AndAlso GetType(Integer).Equals(argTypes(0)) AndAlso name.StartsWith(SET_PREFIX) Then
									pd = New IndexedPropertyDescriptor(Me.beanClass, name.Substring(3), Nothing, Nothing, Nothing, method)
									If throwsException(method, GetType(PropertyVetoException)) Then pd.constrained = True
									End If
							End If
						Catch ex As IntrospectionException
							' This happens if a PropertyDescriptor or IndexedPropertyDescriptor
							' constructor fins that the method violates details of the deisgn
							' pattern, e.g. by having an empty name, or a getter returning
							' void , or whatever.
							pd = Nothing
						End Try
    
						If pd IsNot Nothing Then
							' If this class or one of its base classes is a PropertyChange
							' source, then we assume that any properties we discover are "bound".
							If propertyChangeSource Then pd.bound = True
							addPropertyDescriptor(pd)
						End If
					Next i
				End If
				processPropertyDescriptors()
    
				' Allocate and populate the result array.
				Dim result As PropertyDescriptor() = properties.Values.ToArray(New PropertyDescriptor(properties.Count - 1){})
    
				' Set the default index.
				If defaultPropertyName IsNot Nothing Then
					For i As Integer = 0 To result.Length - 1
						If defaultPropertyName.Equals(result(i).name) Then defaultPropertyIndex = i
					Next i
				End If
    
				Return result
			End Get
		End Property

		Private pdStore As New Dictionary(Of String, IList(Of PropertyDescriptor))

		''' <summary>
		''' Adds the property descriptor to the list store.
		''' </summary>
		Private Sub addPropertyDescriptor(ByVal pd As PropertyDescriptor)
			Dim propName As String = pd.name
			Dim list As IList(Of PropertyDescriptor) = pdStore(propName)
			If list Is Nothing Then
				list = New List(Of )
				pdStore(propName) = list
			End If
			If Me.beanClass IsNot pd.class0 Then
				' replace existing property descriptor
				' only if we have types to resolve
				' in the context of this.beanClass
				Dim read As Method = pd.readMethod
				Dim write As Method = pd.writeMethod
				Dim cls As Boolean = True
				If read IsNot Nothing Then cls = cls AndAlso TypeOf read.genericReturnType Is Class
				If write IsNot Nothing Then cls = cls AndAlso TypeOf write.genericParameterTypes(0) Is Class
				If TypeOf pd Is IndexedPropertyDescriptor Then
					Dim ipd As IndexedPropertyDescriptor = CType(pd, IndexedPropertyDescriptor)
					Dim readI As Method = ipd.indexedReadMethod
					Dim writeI As Method = ipd.indexedWriteMethod
					If readI IsNot Nothing Then cls = cls AndAlso TypeOf readI.genericReturnType Is Class
					If writeI IsNot Nothing Then cls = cls AndAlso TypeOf writeI.genericParameterTypes(1) Is Class
					If Not cls Then
						pd = New IndexedPropertyDescriptor(ipd)
						pd.updateGenericsFor(Me.beanClass)
					End If
				ElseIf Not cls Then
					pd = New PropertyDescriptor(pd)
					pd.updateGenericsFor(Me.beanClass)
				End If
			End If
			list.Add(pd)
		End Sub

		Private Sub addPropertyDescriptors(ByVal descriptors As PropertyDescriptor())
			If descriptors IsNot Nothing Then
				For Each descriptor As PropertyDescriptor In descriptors
					addPropertyDescriptor(descriptor)
				Next descriptor
			End If
		End Sub

		Private Function getPropertyDescriptors(ByVal info As BeanInfo) As PropertyDescriptor()
			Dim descriptors As PropertyDescriptor() = info.propertyDescriptors
			Dim index As Integer = info.defaultPropertyIndex
			If (0 <= index) AndAlso (index < descriptors.Length) Then Me.defaultPropertyName = descriptors(index).name
			Return descriptors
		End Function

		''' <summary>
		''' Populates the property descriptor table by merging the
		''' lists of Property descriptors.
		''' </summary>
		Private Sub processPropertyDescriptors()
			If properties Is Nothing Then properties = New SortedDictionary(Of )

			Dim list As IList(Of PropertyDescriptor)

			Dim pd, gpd, spd As PropertyDescriptor
			Dim ipd, igpd, ispd As IndexedPropertyDescriptor

			Dim it As IEnumerator(Of IList(Of PropertyDescriptor)) = pdStore.Values.GetEnumerator()
			Do While it.MoveNext()
				pd = Nothing
				gpd = Nothing
				spd = Nothing
				ipd = Nothing
				igpd = Nothing
				ispd = Nothing

				list = it.Current

				' First pass. Find the latest getter method. Merge properties
				' of previous getter methods.
				For i As Integer = 0 To list.Count - 1
					pd = list(i)
					If TypeOf pd Is IndexedPropertyDescriptor Then
						ipd = CType(pd, IndexedPropertyDescriptor)
						If ipd.indexedReadMethod IsNot Nothing Then
							If igpd IsNot Nothing Then
								igpd = New IndexedPropertyDescriptor(igpd, ipd)
							Else
								igpd = ipd
							End If
						End If
					Else
						If pd.readMethod IsNot Nothing Then
							Dim pdName As String = pd.readMethod.name
							If gpd IsNot Nothing Then
								' Don't replace the existing read
								' method if it starts with "is"
								Dim gpdName As String = gpd.readMethod.name
								If gpdName.Equals(pdName) OrElse (Not gpdName.StartsWith(IS_PREFIX)) Then gpd = New PropertyDescriptor(gpd, pd)
							Else
								gpd = pd
							End If
						End If
					End If
				Next i

				' Second pass. Find the latest setter method which
				' has the same type as the getter method.
				For i As Integer = 0 To list.Count - 1
					pd = list(i)
					If TypeOf pd Is IndexedPropertyDescriptor Then
						ipd = CType(pd, IndexedPropertyDescriptor)
						If ipd.indexedWriteMethod IsNot Nothing Then
							If igpd IsNot Nothing Then
								If isAssignable(igpd.indexedPropertyType, ipd.indexedPropertyType) Then
									If ispd IsNot Nothing Then
										ispd = New IndexedPropertyDescriptor(ispd, ipd)
									Else
										ispd = ipd
									End If
								End If
							Else
								If ispd IsNot Nothing Then
									ispd = New IndexedPropertyDescriptor(ispd, ipd)
								Else
									ispd = ipd
								End If
							End If
						End If
					Else
						If pd.writeMethod IsNot Nothing Then
							If gpd IsNot Nothing Then
								If isAssignable(gpd.propertyType, pd.propertyType) Then
									If spd IsNot Nothing Then
										spd = New PropertyDescriptor(spd, pd)
									Else
										spd = pd
									End If
								End If
							Else
								If spd IsNot Nothing Then
									spd = New PropertyDescriptor(spd, pd)
								Else
									spd = pd
								End If
							End If
						End If
					End If
				Next i

				' At this stage we should have either PDs or IPDs for the
				' representative getters and setters. The order at which the
				' property descriptors are determined represent the
				' precedence of the property ordering.
				pd = Nothing
				ipd = Nothing

				If igpd IsNot Nothing AndAlso ispd IsNot Nothing Then
					' Complete indexed properties set
					' Merge any classic property descriptors
					If (gpd Is spd) OrElse (gpd Is Nothing) Then
						pd = spd
					ElseIf spd Is Nothing Then
						pd = gpd
					ElseIf TypeOf spd Is IndexedPropertyDescriptor Then
						pd = mergePropertyWithIndexedProperty(gpd, CType(spd, IndexedPropertyDescriptor))
					ElseIf TypeOf gpd Is IndexedPropertyDescriptor Then
						pd = mergePropertyWithIndexedProperty(spd, CType(gpd, IndexedPropertyDescriptor))
					Else
						pd = mergePropertyDescriptor(gpd, spd)
					End If
					If igpd Is ispd Then
						ipd = igpd
					Else
						ipd = mergePropertyDescriptor(igpd, ispd)
					End If
					If pd Is Nothing Then
						pd = ipd
					Else
						Dim propType As  [Class] = pd.propertyType
						Dim ipropType As  [Class] = ipd.indexedPropertyType
						If propType.array AndAlso propType.componentType Is ipropType Then
							pd = If(ipd.class0.IsSubclassOf(pd.class0), New IndexedPropertyDescriptor(pd, ipd), New IndexedPropertyDescriptor(ipd, pd))
						ElseIf ipd.class0.IsSubclassOf(pd.class0) Then
							pd = If(ipd.class0.IsSubclassOf(pd.class0), New PropertyDescriptor(pd, ipd), New PropertyDescriptor(ipd, pd))
						Else
							pd = ipd
						End If
					End If
				ElseIf gpd IsNot Nothing AndAlso spd IsNot Nothing Then
					If igpd IsNot Nothing Then gpd = mergePropertyWithIndexedProperty(gpd, igpd)
					If ispd IsNot Nothing Then spd = mergePropertyWithIndexedProperty(spd, ispd)
					' Complete simple properties set
					If gpd Is spd Then
						pd = gpd
					ElseIf TypeOf spd Is IndexedPropertyDescriptor Then
						pd = mergePropertyWithIndexedProperty(gpd, CType(spd, IndexedPropertyDescriptor))
					ElseIf TypeOf gpd Is IndexedPropertyDescriptor Then
						pd = mergePropertyWithIndexedProperty(spd, CType(gpd, IndexedPropertyDescriptor))
					Else
						pd = mergePropertyDescriptor(gpd, spd)
					End If
				ElseIf ispd IsNot Nothing Then
					' indexed setter
					pd = ispd
					' Merge any classic property descriptors
					If spd IsNot Nothing Then pd = mergePropertyDescriptor(ispd, spd)
					If gpd IsNot Nothing Then pd = mergePropertyDescriptor(ispd, gpd)
				ElseIf igpd IsNot Nothing Then
					' indexed getter
					pd = igpd
					' Merge any classic property descriptors
					If gpd IsNot Nothing Then pd = mergePropertyDescriptor(igpd, gpd)
					If spd IsNot Nothing Then pd = mergePropertyDescriptor(igpd, spd)
				ElseIf spd IsNot Nothing Then
					' simple setter
					pd = spd
				ElseIf gpd IsNot Nothing Then
					' simple getter
					pd = gpd
				End If

				' Very special case to ensure that an IndexedPropertyDescriptor
				' doesn't contain less information than the enclosed
				' PropertyDescriptor. If it does, then recreate as a
				' PropertyDescriptor. See 4168833
				If TypeOf pd Is IndexedPropertyDescriptor Then
					ipd = CType(pd, IndexedPropertyDescriptor)
					If ipd.indexedReadMethod Is Nothing AndAlso ipd.indexedWriteMethod Is Nothing Then pd = New PropertyDescriptor(ipd)
				End If

				' Find the first property descriptor
				' which does not have getter and setter methods.
				' See regression bug 4984912.
				If (pd Is Nothing) AndAlso (list.Count > 0) Then pd = list(0)

				If pd IsNot Nothing Then properties(pd.name) = pd
			Loop
		End Sub

		Private Shared Function isAssignable(ByVal current As [Class], ByVal candidate As [Class]) As Boolean
			Return If((current Is Nothing) OrElse (candidate Is Nothing), current Is candidate, candidate.IsSubclassOf(current))
		End Function

		Private Function mergePropertyWithIndexedProperty(ByVal pd As PropertyDescriptor, ByVal ipd As IndexedPropertyDescriptor) As PropertyDescriptor
			Dim type As  [Class] = pd.propertyType
			If type.array AndAlso (type.componentType Is ipd.indexedPropertyType) Then Return If(ipd.class0.IsSubclassOf(pd.class0), New IndexedPropertyDescriptor(pd, ipd), New IndexedPropertyDescriptor(ipd, pd))
			Return pd
		End Function

		''' <summary>
		''' Adds the property descriptor to the indexedproperty descriptor only if the
		''' types are the same.
		''' 
		''' The most specific property descriptor will take precedence.
		''' </summary>
		Private Function mergePropertyDescriptor(ByVal ipd As IndexedPropertyDescriptor, ByVal pd As PropertyDescriptor) As PropertyDescriptor
			Dim result As PropertyDescriptor = Nothing

			Dim propType As  [Class] = pd.propertyType
			Dim ipropType As  [Class] = ipd.indexedPropertyType

			If propType.array AndAlso propType.componentType Is ipropType Then
				If ipd.class0.IsSubclassOf(pd.class0) Then
					result = New IndexedPropertyDescriptor(pd, ipd)
				Else
					result = New IndexedPropertyDescriptor(ipd, pd)
				End If
			ElseIf (ipd.readMethod Is Nothing) AndAlso (ipd.writeMethod Is Nothing) Then
				If ipd.class0.IsSubclassOf(pd.class0) Then
					result = New PropertyDescriptor(pd, ipd)
				Else
					result = New PropertyDescriptor(ipd, pd)
				End If
			Else
				' Cannot merge the pd because of type mismatch
				' Return the most specific pd
				If ipd.class0.IsSubclassOf(pd.class0) Then
					result = ipd
				Else
					result = pd
					' Try to add methods which may have been lost in the type change
					' See 4168833
					Dim write As Method = result.writeMethod
					Dim read As Method = result.readMethod

					If read Is Nothing AndAlso write IsNot Nothing Then
						read = findMethod(result.class0, GET_PREFIX + NameGenerator.capitalize(result.name), 0)
						If read IsNot Nothing Then
							Try
								result.readMethod = read
							Catch ex As IntrospectionException
								' no consequences for failure.
							End Try
						End If
					End If
					If write Is Nothing AndAlso read IsNot Nothing Then
						write = findMethod(result.class0, SET_PREFIX + NameGenerator.capitalize(result.name), 1, New [Class]() { FeatureDescriptor.getReturnType(result.class0, read) })
						If write IsNot Nothing Then
							Try
								result.writeMethod = write
							Catch ex As IntrospectionException
								' no consequences for failure.
							End Try
						End If
					End If
				End If
			End If
			Return result
		End Function

		' Handle regular pd merge
		Private Function mergePropertyDescriptor(ByVal pd1 As PropertyDescriptor, ByVal pd2 As PropertyDescriptor) As PropertyDescriptor
			If pd2.class0.IsSubclassOf(pd1.class0) Then
				Return New PropertyDescriptor(pd1, pd2)
			Else
				Return New PropertyDescriptor(pd2, pd1)
			End If
		End Function

		' Handle regular ipd merge
		Private Function mergePropertyDescriptor(ByVal ipd1 As IndexedPropertyDescriptor, ByVal ipd2 As IndexedPropertyDescriptor) As IndexedPropertyDescriptor
			If ipd2.class0.IsSubclassOf(ipd1.class0) Then
				Return New IndexedPropertyDescriptor(ipd1, ipd2)
			Else
				Return New IndexedPropertyDescriptor(ipd2, ipd1)
			End If
		End Function

		''' <returns> An array of EventSetDescriptors describing the kinds of
		''' events fired by the target bean. </returns>
		Private Property targetEventInfo As EventSetDescriptor()
			Get
				If events Is Nothing Then events = New Dictionary(Of )
    
				' Check if the bean has its own BeanInfo that will provide
				' explicit information.
				Dim explicitEvents As EventSetDescriptor() = Nothing
				If explicitBeanInfo IsNot Nothing Then
					explicitEvents = explicitBeanInfo.eventSetDescriptors
					Dim ix As Integer = explicitBeanInfo.defaultEventIndex
					If ix >= 0 AndAlso ix < explicitEvents.Length Then defaultEventName = explicitEvents(ix).name
				End If
    
				If explicitEvents Is Nothing AndAlso superBeanInfo IsNot Nothing Then
					' We have no explicit BeanInfo events.  Check with our parent.
					Dim supers As EventSetDescriptor() = superBeanInfo.eventSetDescriptors
					For i As Integer = 0 To supers.Length - 1
						addEvent(supers(i))
					Next i
					Dim ix As Integer = superBeanInfo.defaultEventIndex
					If ix >= 0 AndAlso ix < supers.Length Then defaultEventName = supers(ix).name
				End If
    
				For i As Integer = 0 To additionalBeanInfo.Length - 1
					Dim additional As EventSetDescriptor() = additionalBeanInfo(i).eventSetDescriptors
					If additional IsNot Nothing Then
						For j As Integer = 0 To additional.Length - 1
							addEvent(additional(j))
						Next j
					End If
				Next i
    
				If explicitEvents IsNot Nothing Then
					' Add the explicit explicitBeanInfo data to our results.
					For i As Integer = 0 To explicitEvents.Length - 1
						addEvent(explicitEvents(i))
					Next i
    
				Else
    
					' Apply some reflection to the current class.
    
					' Get an array of all the public beans methods at this level
					Dim methodList As Method() = getPublicDeclaredMethods(beanClass)
    
					' Find all suitable "add", "remove" and "get" Listener methods
					' The name of the listener type is the key for these hashtables
					' i.e, ActionListener
					Dim adds As IDictionary(Of String, Method) = Nothing
					Dim removes As IDictionary(Of String, Method) = Nothing
					Dim gets As IDictionary(Of String, Method) = Nothing
    
					For i As Integer = 0 To methodList.Length - 1
						Dim method As Method = methodList(i)
						If method Is Nothing Then Continue For
						' skip static methods.
						Dim mods As Integer = method.modifiers
						If Modifier.isStatic(mods) Then Continue For
						Dim name As String = method.name
						' Optimization avoid getParameterTypes
						If (Not name.StartsWith(ADD_PREFIX)) AndAlso (Not name.StartsWith(REMOVE_PREFIX)) AndAlso (Not name.StartsWith(GET_PREFIX)) Then Continue For
    
						If name.StartsWith(ADD_PREFIX) Then
							Dim returnType As  [Class] = method.returnType
							If returnType Is GetType(void) Then
								Dim parameterTypes As Type() = method.genericParameterTypes
								If parameterTypes.Length = 1 Then
									Dim type As  [Class] = com.sun.beans.TypeResolver.erase(com.sun.beans.TypeResolver.resolveInClass(beanClass, parameterTypes(0)))
									If Introspector.isSubclass(type, eventListenerType) Then
										Dim listenerName As String = name.Substring(3)
										If listenerName.length() > 0 AndAlso type.name.EndsWith(listenerName) Then
											If adds Is Nothing Then adds = New Dictionary(Of )
											adds(listenerName) = method
										End If
									End If
								End If
							End If
						ElseIf name.StartsWith(REMOVE_PREFIX) Then
							Dim returnType As  [Class] = method.returnType
							If returnType Is GetType(void) Then
								Dim parameterTypes As Type() = method.genericParameterTypes
								If parameterTypes.Length = 1 Then
									Dim type As  [Class] = com.sun.beans.TypeResolver.erase(com.sun.beans.TypeResolver.resolveInClass(beanClass, parameterTypes(0)))
									If Introspector.isSubclass(type, eventListenerType) Then
										Dim listenerName As String = name.Substring(6)
										If listenerName.length() > 0 AndAlso type.name.EndsWith(listenerName) Then
											If removes Is Nothing Then removes = New Dictionary(Of )
											removes(listenerName) = method
										End If
									End If
								End If
							End If
						ElseIf name.StartsWith(GET_PREFIX) Then
							Dim parameterTypes As  [Class]() = method.parameterTypes
							If parameterTypes.Length = 0 Then
								Dim returnType As  [Class] = FeatureDescriptor.getReturnType(beanClass, method)
								If returnType.array Then
									Dim type As  [Class] = returnType.componentType
									If Introspector.isSubclass(type, eventListenerType) Then
										Dim listenerName As String = name.Substring(3, name.length() - 1 - 3)
										If listenerName.length() > 0 AndAlso type.name.EndsWith(listenerName) Then
											If gets Is Nothing Then gets = New Dictionary(Of )
											gets(listenerName) = method
										End If
									End If
								End If
							End If
						End If
					Next i
    
					If adds IsNot Nothing AndAlso removes IsNot Nothing Then
						' Now look for matching addFooListener+removeFooListener pairs.
						' Bonus if there is a matching getFooListeners method as well.
						Dim keys As IEnumerator(Of String) = adds.Keys.GetEnumerator()
						Do While keys.MoveNext()
							Dim listenerName As String = keys.Current
							' Skip any "add" which doesn't have a matching "remove" or
							' a listener name that doesn't end with Listener
							If removes(listenerName) Is Nothing OrElse (Not listenerName.EndsWith("Listener")) Then Continue Do
							Dim eventName As String = decapitalize(listenerName.Substring(0, listenerName.length()-8))
							Dim addMethod As Method = adds(listenerName)
							Dim removeMethod As Method = removes(listenerName)
							Dim getMethod As Method = Nothing
							If gets IsNot Nothing Then getMethod = gets(listenerName)
							Dim argType As  [Class] = FeatureDescriptor.getParameterTypes(beanClass, addMethod)(0)
    
							' generate a list of Method objects for each of the target methods:
							Dim allMethods As Method() = getPublicDeclaredMethods(argType)
							Dim validMethods As IList(Of Method) = New List(Of Method)(allMethods.Length)
							For i As Integer = 0 To allMethods.Length - 1
								If allMethods(i) Is Nothing Then Continue For
    
								If isEventHandler(allMethods(i)) Then validMethods.Add(allMethods(i))
							Next i
							Dim methods As Method() = validMethods.ToArray()
    
							Dim esd As New EventSetDescriptor(eventName, argType, methods, addMethod, removeMethod, getMethod)
    
							' If the adder method throws the TooManyListenersException then it
							' is a Unicast event source.
							If throwsException(addMethod, GetType(java.util.TooManyListenersException)) Then esd.unicast = True
							addEvent(esd)
						Loop
					End If ' if (adds != null ...
				End If
				Dim result As EventSetDescriptor()
				If events.Count = 0 Then
					result = EMPTY_EVENTSETDESCRIPTORS
				Else
					' Allocate and populate the result array.
					result = New EventSetDescriptor(events.Count - 1){}
					result = events.Values.ToArray(result)
    
					' Set the default index.
					If defaultEventName IsNot Nothing Then
						For i As Integer = 0 To result.Length - 1
							If defaultEventName.Equals(result(i).name) Then defaultEventIndex = i
						Next i
					End If
				End If
				Return result
			End Get
		End Property

		Private Sub addEvent(ByVal esd As EventSetDescriptor)
			Dim key As String = esd.name
			If esd.name.Equals("propertyChange") Then propertyChangeSource = True
			Dim old As EventSetDescriptor = events(key)
			If old Is Nothing Then
				events(key) = esd
				Return
			End If
			Dim composite As New EventSetDescriptor(old, esd)
			events(key) = composite
		End Sub

		''' <returns> An array of MethodDescriptors describing the private
		''' methods supported by the target bean. </returns>
		Private Property targetMethodInfo As MethodDescriptor()
			Get
				If methods Is Nothing Then methods = New Dictionary(Of )(100)
    
				' Check if the bean has its own BeanInfo that will provide
				' explicit information.
				Dim explicitMethods As MethodDescriptor() = Nothing
				If explicitBeanInfo IsNot Nothing Then explicitMethods = explicitBeanInfo.methodDescriptors
    
				If explicitMethods Is Nothing AndAlso superBeanInfo IsNot Nothing Then
					' We have no explicit BeanInfo methods.  Check with our parent.
					Dim supers As MethodDescriptor() = superBeanInfo.methodDescriptors
					For i As Integer = 0 To supers.Length - 1
						addMethod(supers(i))
					Next i
				End If
    
				For i As Integer = 0 To additionalBeanInfo.Length - 1
					Dim additional As MethodDescriptor() = additionalBeanInfo(i).methodDescriptors
					If additional IsNot Nothing Then
						For j As Integer = 0 To additional.Length - 1
							addMethod(additional(j))
						Next j
					End If
				Next i
    
				If explicitMethods IsNot Nothing Then
					' Add the explicit explicitBeanInfo data to our results.
					For i As Integer = 0 To explicitMethods.Length - 1
						addMethod(explicitMethods(i))
					Next i
    
				Else
    
					' Apply some reflection to the current class.
    
					' First get an array of all the beans methods at this level
					Dim methodList As Method() = getPublicDeclaredMethods(beanClass)
    
					' Now analyze each method.
					For i As Integer = 0 To methodList.Length - 1
						Dim method As Method = methodList(i)
						If method Is Nothing Then Continue For
						Dim md As New MethodDescriptor(method)
						addMethod(md)
					Next i
				End If
    
				' Allocate and populate the result array.
				Dim result As MethodDescriptor() = New MethodDescriptor(methods.Count - 1){}
				result = methods.Values.ToArray(result)
    
				Return result
			End Get
		End Property

		Private Sub addMethod(ByVal md As MethodDescriptor)
			' We have to be careful here to distinguish method by both name
			' and argument lists.
			' This method gets called a *lot, so we try to be efficient.
			Dim name As String = md.name

			Dim old As MethodDescriptor = methods(name)
			If old Is Nothing Then
				' This is the common case.
				methods(name) = md
				Return
			End If

			' We have a collision on method names.  This is rare.

			' Check if old and md have the same type.
			Dim p1 As String() = md.paramNames
			Dim p2 As String() = old.paramNames

			Dim match As Boolean = False
			If p1.Length = p2.Length Then
				match = True
				For i As Integer = 0 To p1.Length - 1
					If p1(i) <> p2(i) Then
						match = False
						Exit For
					End If
				Next i
			End If
			If match Then
				Dim composite As New MethodDescriptor(old, md)
				methods(name) = composite
				Return
			End If

			' We have a collision on method names with different type signatures.
			' This is very rare.

			Dim longKey As String = makeQualifiedMethodName(name, p1)
			old = methods(longKey)
			If old Is Nothing Then
				methods(longKey) = md
				Return
			End If
			Dim composite As New MethodDescriptor(old, md)
			methods(longKey) = composite
		End Sub

		''' <summary>
		''' Creates a key for a method in a method cache.
		''' </summary>
		Private Shared Function makeQualifiedMethodName(ByVal name As String, ByVal params As String()) As String
			Dim sb As New StringBuffer(name)
			sb.append("="c)
			For i As Integer = 0 To params.Length - 1
				sb.append(":"c)
				sb.append(params(i))
			Next i
			Return sb.ToString()
		End Function

		Private Property targetDefaultEventIndex As Integer
			Get
				Return defaultEventIndex
			End Get
		End Property

		Private Property targetDefaultPropertyIndex As Integer
			Get
				Return defaultPropertyIndex
			End Get
		End Property

		Private Property targetBeanDescriptor As BeanDescriptor
			Get
				' Use explicit info, if available,
				If explicitBeanInfo IsNot Nothing Then
					Dim bd As BeanDescriptor = explicitBeanInfo.beanDescriptor
					If bd IsNot Nothing Then Return (bd)
				End If
				' OK, fabricate a default BeanDescriptor.
				Return New BeanDescriptor(Me.beanClass, findCustomizerClass(Me.beanClass))
			End Get
		End Property

		Private Shared Function findCustomizerClass(ByVal type As [Class]) As  [Class]
			Dim name As String = type.name & "Customizer"
			Try
				type = com.sun.beans.finder.ClassFinder.findClass(name, type.classLoader)
				' Each customizer should inherit java.awt.Component and implement java.beans.Customizer
				' according to the section 9.3 of JavaBeans&trade; specification
				If type.IsSubclassOf(GetType(java.awt.Component)) AndAlso type.IsSubclassOf(GetType(Customizer)) Then Return type
			Catch exception_Renamed As Exception
				' ignore any exceptions
			End Try
			Return Nothing
		End Function

		Private Function isEventHandler(ByVal m As Method) As Boolean
			' We assume that a method is an event handler if it has a single
			' argument, whose type inherit from java.util.Event.
			Dim argTypes As Type() = m.genericParameterTypes
			If argTypes.Length <> 1 Then Return False
			Return isSubclass(com.sun.beans.TypeResolver.erase(com.sun.beans.TypeResolver.resolveInClass(beanClass, argTypes(0))), GetType(java.util.EventObject))
		End Function

	'    
	'     * Internal method to return *public* methods within a class.
	'     
		Private Shared Function getPublicDeclaredMethods(ByVal clz As [Class]) As Method()
			' Looking up Class.getDeclaredMethods is relatively expensive,
			' so we cache the results.
			If Not sun.reflect.misc.ReflectUtil.isPackageAccessible(clz) Then Return New Method(){}
			SyncLock declaredMethodCache
				Dim result As Method() = declaredMethodCache.get(clz)
				If result Is Nothing Then
					result = clz.methods
					For i As Integer = 0 To result.Length - 1
						Dim method As Method = result(i)
						If Not method.declaringClass.Equals(clz) Then
							result(i) = Nothing ' ignore methods declared elsewhere
						Else
							Try
								method = com.sun.beans.finder.MethodFinder.findAccessibleMethod(method)
								Dim type As  [Class] = method.declaringClass
								result(i) = If(type.Equals(clz) OrElse type.interface, method, Nothing) ' ignore methods from superclasses
							Catch exception_Renamed As NoSuchMethodException
								' commented out because of 6976577
								' result[i] = null; // ignore inaccessible methods
							End Try
						End If
					Next i
					declaredMethodCache.put(clz, result)
				End If
				Return result
			End SyncLock
		End Function

		'======================================================================
		' Package private support methods.
		'======================================================================

		''' <summary>
		''' Internal support for finding a target methodName with a given
		''' parameter list on a given class.
		''' </summary>
		Private Shared Function internalFindMethod(ByVal start As [Class], ByVal methodName As String, ByVal argCount As Integer, ByVal args As  [Class]()) As Method
			' For overriden methods we need to find the most derived version.
			' So we start with the given class and walk up the superclass chain.

			Dim method As Method = Nothing

			Dim cl As  [Class] = start
			Do While cl IsNot Nothing
				Dim methods As Method() = getPublicDeclaredMethods(cl)
				For i As Integer = 0 To methods.Length - 1
					method = methods(i)
					If method Is Nothing Then Continue For

					' make sure method signature matches.
					If method.name.Equals(methodName) Then
						Dim params As Type() = method.genericParameterTypes
						If params.Length = argCount Then
							If args IsNot Nothing Then
								Dim different As Boolean = False
								If argCount > 0 Then
									For j As Integer = 0 To argCount - 1
										If com.sun.beans.TypeResolver.erase(com.sun.beans.TypeResolver.resolveInClass(start, params(j))) IsNot args(j) Then
											different = True
											Continue For
										End If
									Next j
									If different Then Continue For
								End If
							End If
							Return method
						End If
					End If
				Next i
				cl = cl.BaseType
			Loop
			method = Nothing

			' Now check any inherited interfaces.  This is necessary both when
			' the argument class is itself an interface, and when the argument
			' class is an abstract class.
			Dim ifcs As  [Class]() = start.interfaces
			For i As Integer = 0 To ifcs.Length - 1
				' Note: The original implementation had both methods calling
				' the 3 arg method. This is preserved but perhaps it should
				' pass the args array instead of null.
				method = internalFindMethod(ifcs(i), methodName, argCount, Nothing)
				If method IsNot Nothing Then Exit For
			Next i
			Return method
		End Function

		''' <summary>
		''' Find a target methodName on a given class.
		''' </summary>
		Friend Shared Function findMethod(ByVal cls As [Class], ByVal methodName As String, ByVal argCount As Integer) As Method
			Return findMethod(cls, methodName, argCount, Nothing)
		End Function

		''' <summary>
		''' Find a target methodName with specific parameter list on a given class.
		''' <p>
		''' Used in the contructors of the EventSetDescriptor,
		''' PropertyDescriptor and the IndexedPropertyDescriptor.
		''' <p> </summary>
		''' <param name="cls"> The Class object on which to retrieve the method. </param>
		''' <param name="methodName"> Name of the method. </param>
		''' <param name="argCount"> Number of arguments for the desired method. </param>
		''' <param name="args"> Array of argument types for the method. </param>
		''' <returns> the method or null if not found </returns>
		Friend Shared Function findMethod(ByVal cls As [Class], ByVal methodName As String, ByVal argCount As Integer, ByVal args As  [Class]()) As Method
			If methodName Is Nothing Then Return Nothing
			Return internalFindMethod(cls, methodName, argCount, args)
		End Function

		''' <summary>
		''' Return true if class a is either equivalent to class b, or
		''' if class a is a subclass of class b, i.e. if a either "extends"
		''' or "implements" b.
		''' Note tht either or both "Class" objects may represent interfaces.
		''' </summary>
		Friend Shared Function isSubclass(ByVal a As [Class], ByVal b As [Class]) As Boolean
			' We rely on the fact that for any given java class or
			' primtitive type there is a unqiue Class object, so
			' we can use object equivalence in the comparisons.
			If a Is b Then Return True
			If a Is Nothing OrElse b Is Nothing Then Return False
			Dim x As  [Class] = a
			Do While x IsNot Nothing
				If x Is b Then Return True
				If b.interface Then
					Dim interfaces As  [Class]() = x.interfaces
					For i As Integer = 0 To interfaces.Length - 1
						If isSubclass(interfaces(i), b) Then Return True
					Next i
				End If
				x = x.BaseType
			Loop
			Return False
		End Function

		''' <summary>
		''' Return true iff the given method throws the given exception.
		''' </summary>
		Private Function throwsException(ByVal method As Method, ByVal exception_Renamed As [Class]) As Boolean
			Dim exs As  [Class]() = method.exceptionTypes
			For i As Integer = 0 To exs.Length - 1
				If exs(i) Is exception_Renamed Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Try to create an instance of a named class.
		''' First try the classloader of "sibling", then try the system
		''' classloader then the class loader of the current Thread.
		''' </summary>
		Friend Shared Function instantiate(ByVal sibling As [Class], ByVal className As String) As Object
			' First check with sibling's classloader (if any).
			Dim cl As  [Class]Loader = sibling.classLoader
			Dim cls As  [Class] = com.sun.beans.finder.ClassFinder.findClass(className, cl)
			Return cls.newInstance()
		End Function

	End Class ' end class Introspector

	'===========================================================================

	''' <summary>
	''' Package private implementation support class for Introspector's
	''' internal use.
	''' <p>
	''' Mostly this is used as a placeholder for the descriptors.
	''' </summary>

	Friend Class GenericBeanInfo
		Inherits SimpleBeanInfo

		Private beanDescriptor As BeanDescriptor
		Private events As EventSetDescriptor()
		Private defaultEvent As Integer
		Private properties As PropertyDescriptor()
		Private defaultProperty As Integer
		Private methods As MethodDescriptor()
		Private targetBeanInfoRef As Reference(Of BeanInfo)

		Public Sub New(ByVal beanDescriptor As BeanDescriptor, ByVal events As EventSetDescriptor(), ByVal defaultEvent As Integer, ByVal properties As PropertyDescriptor(), ByVal defaultProperty As Integer, ByVal methods As MethodDescriptor(), ByVal targetBeanInfo As BeanInfo)
			Me.beanDescriptor = beanDescriptor
			Me.events = events
			Me.defaultEvent = defaultEvent
			Me.properties = properties
			Me.defaultProperty = defaultProperty
			Me.methods = methods
			Me.targetBeanInfoRef = If(targetBeanInfo IsNot Nothing, New SoftReference(Of )(targetBeanInfo), Nothing)
		End Sub

		''' <summary>
		''' Package-private dup constructor
		''' This must isolate the new object from any changes to the old object.
		''' </summary>
		Friend Sub New(ByVal old As GenericBeanInfo)

			beanDescriptor = New BeanDescriptor(old.beanDescriptor)
			If old.events IsNot Nothing Then
				Dim len As Integer = old.events.Length
				events = New EventSetDescriptor(len - 1){}
				For i As Integer = 0 To len - 1
					events(i) = New EventSetDescriptor(old.events(i))
				Next i
			End If
			defaultEvent = old.defaultEvent
			If old.properties IsNot Nothing Then
				Dim len As Integer = old.properties.Length
				properties = New PropertyDescriptor(len - 1){}
				For i As Integer = 0 To len - 1
					Dim oldp As PropertyDescriptor = old.properties(i)
					If TypeOf oldp Is IndexedPropertyDescriptor Then
						properties(i) = New IndexedPropertyDescriptor(CType(oldp, IndexedPropertyDescriptor))
					Else
						properties(i) = New PropertyDescriptor(oldp)
					End If
				Next i
			End If
			defaultProperty = old.defaultProperty
			If old.methods IsNot Nothing Then
				Dim len As Integer = old.methods.Length
				methods = New MethodDescriptor(len - 1){}
				For i As Integer = 0 To len - 1
					methods(i) = New MethodDescriptor(old.methods(i))
				Next i
			End If
			Me.targetBeanInfoRef = old.targetBeanInfoRef
		End Sub

		Public Property Overrides propertyDescriptors As PropertyDescriptor()
			Get
				Return properties
			End Get
		End Property

		Public Property Overrides defaultPropertyIndex As Integer
			Get
				Return defaultProperty
			End Get
		End Property

		Public Property Overrides eventSetDescriptors As EventSetDescriptor()
			Get
				Return events
			End Get
		End Property

		Public Property Overrides defaultEventIndex As Integer
			Get
				Return defaultEvent
			End Get
		End Property

		Public Property Overrides methodDescriptors As MethodDescriptor()
			Get
				Return methods
			End Get
		End Property

		Public Property Overrides beanDescriptor As BeanDescriptor
			Get
				Return beanDescriptor
			End Get
		End Property

		Public Overrides Function getIcon(ByVal iconKind As Integer) As java.awt.Image
			Dim targetBeanInfo_Renamed As BeanInfo = targetBeanInfo
			If targetBeanInfo_Renamed IsNot Nothing Then Return targetBeanInfo_Renamed.getIcon(iconKind)
			Return MyBase.getIcon(iconKind)
		End Function

		Private Property targetBeanInfo As BeanInfo
			Get
				If Me.targetBeanInfoRef Is Nothing Then Return Nothing
				Dim targetBeanInfo_Renamed As BeanInfo = Me.targetBeanInfoRef.get()
				If targetBeanInfo_Renamed Is Nothing Then
					targetBeanInfo_Renamed = ThreadGroupContext.context.beanInfoFinder.find(Me.beanDescriptor.beanClass)
					If targetBeanInfo_Renamed IsNot Nothing Then Me.targetBeanInfoRef = New SoftReference(Of )(targetBeanInfo_Renamed)
				End If
				Return targetBeanInfo_Renamed
			End Get
		End Property
	End Class

End Namespace
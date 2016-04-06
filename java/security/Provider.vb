Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports java.util

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

Namespace java.security


    ''' <summary>
    ''' This class represents a "provider" for the
    ''' Java Security API, where a provider implements some or all parts of
    ''' Java Security. Services that a provider may implement include:
    ''' 
    ''' <ul>
    ''' 
    ''' <li>Algorithms (such as DSA, RSA, MD5 or SHA-1).
    ''' 
    ''' <li>Key generation, conversion, and management facilities (such as for
    ''' algorithm-specific keys).
    ''' 
    ''' </ul>
    ''' 
    ''' <p>Each provider has a name and a version number, and is configured
    ''' in each runtime it is installed in.
    ''' 
    ''' <p>See <a href =
    ''' "../../../technotes/guides/security/crypto/CryptoSpec.html#Provider">The Provider Class</a>
    ''' in the "Java Cryptography Architecture API Specification &amp; Reference"
    ''' for information about how a particular type of provider, the
    ''' cryptographic service provider, works and is installed. However,
    ''' please note that a provider can be used to implement any security
    ''' service in Java that uses a pluggable architecture with a choice
    ''' of implementations that fit underneath.
    ''' 
    ''' <p>Some provider implementations may encounter unrecoverable internal
    ''' errors during their operation, for example a failure to communicate with a
    ''' security token. A <seealso cref="ProviderException"/> should be used to indicate
    ''' such errors.
    ''' 
    ''' <p>The service type {@code Provider} is reserved for use by the
    ''' security framework. Services of this type cannot be added, removed,
    ''' or modified by applications.
    ''' The following attributes are automatically placed in each Provider object:
    ''' <table cellspacing=4>
    ''' <caption><b>Attributes Automatically Placed in a Provider Object</b></caption>
    ''' <tr><th>Name</th><th>Value</th>
    ''' <tr><td>{@code Provider.id name}</td>
    '''    <td>{@code String.valueOf(provider.getName())}</td>
    ''' <tr><td>{@code Provider.id version}</td>
    '''     <td>{@code String.valueOf(provider.getVersion())}</td>
    ''' <tr><td>{@code Provider.id info}</td>
    '''       <td>{@code String.valueOf(provider.getInfo())}</td>
    ''' <tr><td>{@code Provider.id className}</td>
    '''     <td>{@code provider.getClass().getName()}</td>
    ''' </table>
    ''' 
    ''' @author Benjamin Renaud
    ''' @author Andreas Sterbenz
    ''' </summary>
    Public MustInherit Class Provider
        Inherits Properties

        ' Declare serialVersionUID to be compatible with JDK1.1
        Friend Const serialVersionUID As Long = -4298000515446427739L

        Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("provider", "Provider")

        ''' <summary>
        ''' The provider name.
        ''' 
        ''' @serial
        ''' </summary>
        Private name As String

        ''' <summary>
        ''' A description of the provider and its services.
        ''' 
        ''' @serial
        ''' </summary>
        Private info As String

        ''' <summary>
        ''' The provider version number.
        ''' 
        ''' @serial
        ''' </summary>
        Private version As Double


        <NonSerialized>
        Private entrySet_Renamed As [Set](Of KeyValuePair(Of Object, Object)) = Nothing
        <NonSerialized>
        Private entrySetCallCount As Integer = 0

        <NonSerialized>
        Private initialized As Boolean

        ''' <summary>
        ''' Constructs a provider with the specified name, version number,
        ''' and information.
        ''' </summary>
        ''' <param name="name"> the provider name.
        ''' </param>
        ''' <param name="version"> the provider version number.
        ''' </param>
        ''' <param name="info"> a description of the provider and its services. </param>
        Protected Friend Sub New(  name As String,   version As Double,   info As String)
            Me.name = name
            Me.version = version
            Me.info = info
            putId()
            initialized = True
        End Sub

        ''' <summary>
        ''' Returns the name of this provider.
        ''' </summary>
        ''' <returns> the name of this provider. </returns>
        Public Overridable Property name As String
            Get
                Return name
            End Get
        End Property

        ''' <summary>
        ''' Returns the version number for this provider.
        ''' </summary>
        ''' <returns> the version number for this provider. </returns>
        Public Overridable Property version As Double
            Get
                Return version
            End Get
        End Property

        ''' <summary>
        ''' Returns a human-readable description of the provider and its
        ''' services.  This may return an HTML page, with relevant links.
        ''' </summary>
        ''' <returns> a description of the provider and its services. </returns>
        Public Overridable Property info As String
            Get
                Return info
            End Get
        End Property

        ''' <summary>
        ''' Returns a string with the name and the version number
        ''' of this provider.
        ''' </summary>
        ''' <returns> the string with the name and the version number
        ''' for this provider. </returns>
        Public Overrides Function ToString() As String
            Return name & " version " & version
        End Function

        '    
        '     * override the following methods to ensure that provider
        '     * information can only be changed if the caller has the appropriate
        '     * permissions.
        '     

        ''' <summary>
        ''' Clears this provider so that it no longer contains the properties
        ''' used to look up facilities implemented by the provider.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "clearProviderProperties."+name}
        ''' (where {@code name} is the provider name) to see if it's ok to clear
        ''' this provider.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to clear this provider
        ''' 
        ''' @since 1.2 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub clear()
            check("clearProviderProperties." & name)
            If debug IsNot Nothing Then debug.println("Remove " & name & " provider properties")
            implClear()
        End Sub

        ''' <summary>
        ''' Reads a property list (key and element pairs) from the input stream.
        ''' </summary>
        ''' <param name="inStream">   the input stream. </param>
        ''' <exception cref="IOException">  if an error occurred when reading from the
        '''               input stream. </exception>
        ''' <seealso cref= java.util.Properties#load </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub load(  inStream As InputStream)
            check("putProviderProperty." & name)
            If debug IsNot Nothing Then debug.println("Load " & name & " provider properties")
            Dim tempProperties As New Properties
            tempProperties.load(inStream)
            implPutAll(tempProperties)
        End Sub

        ''' <summary>
        ''' Copies all of the mappings from the specified Map to this provider.
        ''' These mappings will replace any properties that this provider had
        ''' for any of the keys currently in the specified Map.
        ''' 
        ''' @since 1.2
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub putAll(Of T1)(  t As Map(Of T1))
            check("putProviderProperty." & name)
            If debug IsNot Nothing Then debug.println("Put all " & name & " provider properties")
            implPutAll(t)
        End Sub

        ''' <summary>
        ''' Returns an unmodifiable Set view of the property entries contained
        ''' in this Provider.
        ''' </summary>
        ''' <seealso cref=   java.util.Map.Entry
        ''' @since 1.2 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function entrySet() As [Set](Of KeyValuePair(Of Object, Object))
            checkInitialized()
            If entrySet_Renamed Is Nothing Then
                Dim tempVar As Boolean = entrySetCallCount = 0
                entrySetCallCount += 1
                If tempVar Then ' Initial call
                    entrySet_Renamed = Collections.unmodifiableMap(Me).entrySet()
                Else
                    Return MyBase.entrySet() ' Recursive call
                End If
            End If

            ' This exception will be thrown if the implementation of
            ' Collections.unmodifiableMap.entrySet() is changed such that it
            ' no longer calls entrySet() on the backing Map.  (Provider's
            ' entrySet implementation depends on this "implementation detail",
            ' which is unlikely to change.
            If entrySetCallCount <> 2 Then Throw New RuntimeException("Internal error.")

            Return entrySet_Renamed
        End Function

        ''' <summary>
        ''' Returns an unmodifiable Set view of the property keys contained in
        ''' this provider.
        ''' 
        ''' @since 1.2
        ''' </summary>
        Public Overrides Function keySet() As [Set](Of Object)
            checkInitialized()
            Return Collections.unmodifiableSet(MyBase.keys)
        End Function

        ''' <summary>
        ''' Returns an unmodifiable Collection view of the property values
        ''' contained in this provider.
        ''' 
        ''' @since 1.2
        ''' </summary>
        Public Overrides Function values() As Collection(Of Object)
            checkInitialized()
            Return Collections.unmodifiableCollection(MyBase.values())
        End Function

        ''' <summary>
        ''' Sets the {@code key} property to have the specified
        ''' {@code value}.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "putProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to set this
        ''' provider's property values.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values.
        ''' 
        ''' @since 1.2 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function put(  key As Object,   value As Object) As Object
            check("putProviderProperty." & name)
            If debug IsNot Nothing Then debug.println("Set " & name & " provider property [" & key & "/" & value & "]")
            Return implPut(key, value)
        End Function

        ''' <summary>
        ''' If the specified key is not already associated with a value (or is mapped
        ''' to {@code null}) associates it with the given value and returns
        ''' {@code null}, else returns the current value.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "putProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to set this
        ''' provider's property values.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values.
        ''' 
        ''' @since 1.8 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function putIfAbsent(  key As Object,   value As Object) As Object
            check("putProviderProperty." & name)
            If debug IsNot Nothing Then debug.println("Set " & name & " provider property [" & key & "/" & value & "]")
            Return implPutIfAbsent(key, value)
        End Function

        ''' <summary>
        ''' Removes the {@code key} property (and its corresponding
        ''' {@code value}).
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "removeProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to remove this
        ''' provider's properties.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to remove this provider's properties.
        ''' 
        ''' @since 1.2 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function remove(  key As Object) As Object
            check("removeProviderProperty." & name)
            If debug IsNot Nothing Then debug.println("Remove " & name & " provider property " & key)
            Return implRemove(key)
        End Function

        ''' <summary>
        ''' Removes the entry for the specified key only if it is currently
        ''' mapped to the specified value.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "removeProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to remove this
        ''' provider's properties.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to remove this provider's properties.
        ''' 
        ''' @since 1.8 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function remove(  key As Object,   value As Object) As Boolean
            check("removeProviderProperty." & name)
            If debug IsNot Nothing Then debug.println("Remove " & name & " provider property " & key)
            Return implRemove(key, value)
        End Function

        ''' <summary>
        ''' Replaces the entry for the specified key only if currently
        ''' mapped to the specified value.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "putProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to set this
        ''' provider's property values.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values.
        ''' 
        ''' @since 1.8 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function replace(  key As Object,   oldValue As Object,   newValue As Object) As Boolean
            check("putProviderProperty." & name)

            If debug IsNot Nothing Then debug.println("Replace " & name & " provider property " & key)
            Return implReplace(key, oldValue, newValue)
        End Function

        ''' <summary>
        ''' Replaces the entry for the specified key only if it is
        ''' currently mapped to some value.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "putProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to set this
        ''' provider's property values.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values.
        ''' 
        ''' @since 1.8 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function replace(  key As Object,   value As Object) As Object
            check("putProviderProperty." & name)

            If debug IsNot Nothing Then debug.println("Replace " & name & " provider property " & key)
            Return implReplace(key, value)
        End Function

        ''' <summary>
        ''' Replaces each entry's value with the result of invoking the given
        ''' function on that entry, in the order entries are returned by an entry
        ''' set iterator, until all entries have been processed or the function
        ''' throws an exception.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the string {@code "putProviderProperty."+name},
        ''' where {@code name} is the provider name, to see if it's ok to set this
        ''' provider's property values.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values.
        ''' 
        ''' @since 1.8 </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub replaceAll(Of T1 As Object)(  [function] As java.util.function.BiFunction(Of T1))
            check("putProviderProperty." & name)

            If debug IsNot Nothing Then debug.println("ReplaceAll " & name & " provider property ")
            implReplaceAll([function])
        End Sub

        ''' <summary>
        ''' Attempts to compute a mapping for the specified key and its
        ''' current mapped value (or {@code null} if there is no current
        ''' mapping).
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the strings {@code "putProviderProperty."+name}
        ''' and {@code "removeProviderProperty."+name}, where {@code name} is the
        ''' provider name, to see if it's ok to set this provider's property values
        ''' and remove this provider's properties.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values or remove properties.
        ''' 
        ''' @since 1.8 </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function compute(Of T1 As Object)(  key As Object,   remappingFunction As java.util.function.BiFunction(Of T1)) As Object
            check("putProviderProperty." & name)
            check("removeProviderProperty" & name)

            If debug IsNot Nothing Then debug.println("Compute " & name & " provider property " & key)
            Return implCompute(key, remappingFunction)
        End Function

        ''' <summary>
        ''' If the specified key is not already associated with a value (or
        ''' is mapped to {@code null}), attempts to compute its value using
        ''' the given mapping function and enters it into this map unless
        ''' {@code null}.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the strings {@code "putProviderProperty."+name}
        ''' and {@code "removeProviderProperty."+name}, where {@code name} is the
        ''' provider name, to see if it's ok to set this provider's property values
        ''' and remove this provider's properties.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values and remove properties.
        ''' 
        ''' @since 1.8 </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function computeIfAbsent(Of T1 As Object)(  key As Object,   mappingFunction As java.util.function.Function(Of T1)) As Object
            check("putProviderProperty." & name)
            check("removeProviderProperty" & name)

            If debug IsNot Nothing Then debug.println("ComputeIfAbsent " & name & " provider property " & key)
            Return implComputeIfAbsent(key, mappingFunction)
        End Function

        ''' <summary>
        ''' If the value for the specified key is present and non-null, attempts to
        ''' compute a new mapping given the key and its current mapped value.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the strings {@code "putProviderProperty."+name}
        ''' and {@code "removeProviderProperty."+name}, where {@code name} is the
        ''' provider name, to see if it's ok to set this provider's property values
        ''' and remove this provider's properties.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values or remove properties.
        ''' 
        ''' @since 1.8 </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function computeIfPresent(Of T1 As Object)(  key As Object,   remappingFunction As java.util.function.BiFunction(Of T1)) As Object
            check("putProviderProperty." & name)
            check("removeProviderProperty" & name)

            If debug IsNot Nothing Then debug.println("ComputeIfPresent " & name & " provider property " & key)
            Return implComputeIfPresent(key, remappingFunction)
        End Function

        ''' <summary>
        ''' If the specified key is not already associated with a value or is
        ''' associated with null, associates it with the given value. Otherwise,
        ''' replaces the value with the results of the given remapping function,
        ''' or removes if the result is null. This method may be of use when
        ''' combining multiple mapped values for a key.
        ''' 
        ''' <p>If a security manager is enabled, its {@code checkSecurityAccess}
        ''' method is called with the strings {@code "putProviderProperty."+name}
        ''' and {@code "removeProviderProperty."+name}, where {@code name} is the
        ''' provider name, to see if it's ok to set this provider's property values
        ''' and remove this provider's properties.
        ''' </summary>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method
        '''          denies access to set property values or remove properties.
        ''' 
        ''' @since 1.8 </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function merge(Of T1 As Object)(  key As Object,   value As Object,   remappingFunction As java.util.function.BiFunction(Of T1)) As Object
            check("putProviderProperty." & name)
            check("removeProviderProperty" & name)

            If debug IsNot Nothing Then debug.println("Merge " & name & " provider property " & key)
            Return implMerge(key, value, remappingFunction)
        End Function

        ' let javadoc show doc from superclass
        Public Overrides Function [get](  key As Object) As Object
            checkInitialized()
            Return MyBase.get(key)
        End Function
        ''' <summary>
        ''' @since 1.8
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Function getOrDefault(  key As Object,   defaultValue As Object) As Object
            checkInitialized()
            Return MyBase.getOrDefault(key, defaultValue)
        End Function

        ''' <summary>
        ''' @since 1.8
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub forEach(Of T1)(  action As java.util.function.BiConsumer(Of T1))
            checkInitialized()
            MyBase.forEach(action)
        End Sub

        ' let javadoc show doc from superclass
        Public Overrides Function keys() As Enumeration(Of Object)
            checkInitialized()
            Return MyBase.keys()
        End Function

        ' let javadoc show doc from superclass
        Public Overrides Function elements() As Enumeration(Of Object)
            checkInitialized()
            Return MyBase.elements()
        End Function

        ' let javadoc show doc from superclass
        Public Overrides Function getProperty(  key As String) As String
            checkInitialized()
            Return MyBase.getProperty(key)
        End Function

        Private Sub checkInitialized()
            If Not initialized Then Throw New IllegalStateException
        End Sub

        Private Sub check(  directive As String)
            checkInitialized()
            Dim security_Renamed As SecurityManager = System.securityManager
            If security_Renamed IsNot Nothing Then security_Renamed.checkSecurityAccess(directive)
        End Sub

        ' legacy properties changed since last call to any services method?
        <NonSerialized>
        Private legacyChanged As Boolean
        ' serviceMap changed since last call to getServices()
        <NonSerialized>
        Private servicesChanged As Boolean

        ' Map<String,String>
        <NonSerialized>
        Private legacyStrings As Map(Of String, String)

        ' Map<ServiceKey,Service>
        ' used for services added via putService(), initialized on demand
        <NonSerialized>
        Private serviceMap As Map(Of ServiceKey, Service)

        ' Map<ServiceKey,Service>
        ' used for services added via legacy methods, init on demand
        <NonSerialized>
        Private legacyMap As Map(Of ServiceKey, Service)

        ' Set<Service>
        ' Unmodifiable set of all services. Initialized on demand.
        <NonSerialized>
        Private serviceSet As [Set](Of Service)

        ' register the id attributes for this provider
        ' this is to ensure that equals() and hashCode() do not incorrectly
        ' report to different provider objects as the same
        Private Sub putId()
            ' note: name and info may be null
            MyBase.put("Provider.id name", Convert.ToString(name))
            MyBase.put("Provider.id version", Convert.ToString(version))
            MyBase.put("Provider.id info", Convert.ToString(info))
            MyBase.put("Provider.id className", Me.GetType().name)
        End Sub

        Private Sub readObject(  [in] As ObjectInputStream)
            Dim copy As Map(Of Object, Object) = New HashMap(Of Object, Object)
            For Each entry As KeyValuePair(Of Object, Object) In MyBase.entrySet()
                copy.put(entry.Key, entry.Value)
            Next entry
            defaults = Nothing
            [in].defaultReadObject()
            implClear()
            initialized = True
            putAll(copy)
        End Sub

        Private Function checkLegacy(  key As Object) As Boolean
            Dim keyString As String = CStr(key)
            If keyString.StartsWith("Provider.") Then Return False

            legacyChanged = True
            If legacyStrings Is Nothing Then legacyStrings = New LinkedHashMap(Of String, String)
            Return True
        End Function

        ''' <summary>
        ''' Copies all of the mappings from the specified Map to this provider.
        ''' Internal method to be called AFTER the security check has been
        ''' performed.
        ''' </summary>
        Private Sub implPutAll(Of T1)(  t As Map(Of T1))
            'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
            For Each e As KeyValuePair(Of ?, ?) In t.entrySet()
                implPut(e.Key, e.Value)
            Next e
        End Sub

        Private Function implRemove(  key As Object) As Object
            If TypeOf key Is String Then
                If Not checkLegacy(key) Then Return Nothing
                legacyStrings.remove(CStr(key))
            End If
            Return MyBase.remove(key)
        End Function

        Private Function implRemove(  key As Object,   value As Object) As Boolean
            If TypeOf key Is String AndAlso TypeOf value Is String Then
                If Not checkLegacy(key) Then Return False
                legacyStrings.remove(CStr(key), value)
            End If
            Return MyBase.remove(key, value)
        End Function

        Private Function implReplace(  key As Object,   oldValue As Object,   newValue As Object) As Boolean
            If (TypeOf key Is String) AndAlso (TypeOf oldValue Is String) AndAlso (TypeOf newValue Is String) Then
                If Not checkLegacy(key) Then Return False
                legacyStrings.replace(CStr(key), CStr(oldValue), CStr(newValue))
            End If
            Return MyBase.replace(key, oldValue, newValue)
        End Function

        Private Function implReplace(  key As Object,   value As Object) As Object
            If (TypeOf key Is String) AndAlso (TypeOf value Is String) Then
                If Not checkLegacy(key) Then Return Nothing
                legacyStrings.replace(CStr(key), CStr(value))
            End If
            Return MyBase.replace(key, value)
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Sub implReplaceAll(Of T1 As Object)(  [function] As java.util.function.BiFunction(Of T1))
            legacyChanged = True
            If legacyStrings Is Nothing Then
                legacyStrings = New LinkedHashMap(Of String, String)
            Else
                'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                legacyStrings.replaceAll(CType([function], java.util.function.BiFunction(Of ?, ?, ? As String)))
            End If
            MyBase.replaceAll([function])
        End Sub


        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Function implMerge(Of T1 As Object)(  key As Object,   value As Object,   remappingFunction As java.util.function.BiFunction(Of T1)) As Object
            If (TypeOf key Is String) AndAlso (TypeOf value Is String) Then
                If Not checkLegacy(key) Then Return Nothing
                'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                legacyStrings.merge(CStr(key), CStr(value), CType(remappingFunction, java.util.function.BiFunction(Of ?, ?, ? As String)))
            End If
            Return MyBase.merge(key, value, remappingFunction)
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Function implCompute(Of T1 As Object)(  key As Object,   remappingFunction As java.util.function.BiFunction(Of T1)) As Object
            If TypeOf key Is String Then
                If Not checkLegacy(key) Then Return Nothing
                'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                legacyStrings.computeIfAbsent(CStr(key), CType(remappingFunction, java.util.function.Function(Of ?, ? As String)))
            End If
            Return MyBase.compute(key, remappingFunction)
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Function implComputeIfAbsent(Of T1 As Object)(  key As Object,   mappingFunction As java.util.function.Function(Of T1)) As Object
            If TypeOf key Is String Then
                If Not checkLegacy(key) Then Return Nothing
                'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                legacyStrings.computeIfAbsent(CStr(key), CType(mappingFunction, java.util.function.Function(Of ?, ? As String)))
            End If
            Return MyBase.computeIfAbsent(key, mappingFunction)
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Function implComputeIfPresent(Of T1 As Object)(  key As Object,   remappingFunction As java.util.function.BiFunction(Of T1)) As Object
            If TypeOf key Is String Then
                If Not checkLegacy(key) Then Return Nothing
                'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                legacyStrings.computeIfPresent(CStr(key), CType(remappingFunction, java.util.function.BiFunction(Of ?, ?, ? As String)))
            End If
            Return MyBase.computeIfPresent(key, remappingFunction)
        End Function

        Private Function implPut(  key As Object,   value As Object) As Object
            If (TypeOf key Is String) AndAlso (TypeOf value Is String) Then
                If Not checkLegacy(key) Then Return Nothing
                legacyStrings.put(CStr(key), CStr(value))
            End If
            Return MyBase.put(key, value)
        End Function

        Private Function implPutIfAbsent(  key As Object,   value As Object) As Object
            If (TypeOf key Is String) AndAlso (TypeOf value Is String) Then
                If Not checkLegacy(key) Then Return Nothing
                legacyStrings.putIfAbsent(CStr(key), CStr(value))
            End If
            Return MyBase.putIfAbsent(key, value)
        End Function

        Private Sub implClear()
            If legacyStrings IsNot Nothing Then legacyStrings.clear()
            If legacyMap IsNot Nothing Then legacyMap.clear()
            If serviceMap IsNot Nothing Then serviceMap.clear()
            legacyChanged = False
            servicesChanged = False
            serviceSet = Nothing
            MyBase.clear()
            putId()
        End Sub

        ' used as key in the serviceMap and legacyMap HashMaps
        Private Class ServiceKey
            Private ReadOnly type As String
            Private ReadOnly algorithm As String
            Private ReadOnly originalAlgorithm As String
            Private Sub New(  type As String,   algorithm As String,   intern As Boolean)
                Me.type = type
                Me.originalAlgorithm = algorithm
                algorithm = algorithm.ToUpper(ENGLISH)
                Me.algorithm = If(intern, algorithm.Intern(), algorithm)
            End Sub
            Public Overrides Function GetHashCode() As Integer
                Return type.GetHashCode() + algorithm.GetHashCode()
            End Function
            Public Overrides Function Equals(  obj As Object) As Boolean
                If Me Is obj Then Return True
                If TypeOf obj Is ServiceKey = False Then Return False
                Dim other As ServiceKey = CType(obj, ServiceKey)
                Return Me.type.Equals(other.type) AndAlso Me.algorithm.Equals(other.algorithm)
            End Function
            Friend Overridable Function matches(  type As String,   algorithm As String) As Boolean
                Return (Me.type = type) AndAlso (Me.originalAlgorithm = algorithm)
            End Function
        End Class

        ''' <summary>
        ''' Ensure all the legacy String properties are fully parsed into
        ''' service objects.
        ''' </summary>
        Private Sub ensureLegacyParsed()
            If (legacyChanged = False) OrElse (legacyStrings Is Nothing) Then Return
            serviceSet = Nothing
            If legacyMap Is Nothing Then
                legacyMap = New LinkedHashMap(Of ServiceKey, Service)
            Else
                legacyMap.clear()
            End If
            For Each entry As KeyValuePair(Of String, String) In legacyStrings.entrySet()
                parseLegacyPut(entry.Key, entry.Value)
            Next entry
            removeInvalidServices(legacyMap)
            legacyChanged = False
        End Sub

        ''' <summary>
        ''' Remove all invalid services from the Map. Invalid services can only
        ''' occur if the legacy properties are inconsistent or incomplete.
        ''' </summary>
        Private Sub removeInvalidServices(  map As Map(Of ServiceKey, Service))
            Dim t As [Iterator](Of KeyValuePair(Of ServiceKey, Service)) = map.entrySet().GetEnumerator()
            Do While t.MoveNext()
                Dim s As Service = t.Current.value
                If s.valid = False Then t.remove()
            Loop
        End Sub

        Private Function getTypeAndAlgorithm(  key As String) As String()
            Dim i As Integer = key.IndexOf(".")
            If i < 1 Then
                If debug IsNot Nothing Then debug.println("Ignoring invalid entry in provider " & name & ":" & key)
                Return Nothing
            End If
            Dim type As String = key.Substring(0, i)
            Dim alg As String = key.Substring(i + 1)
            Return New String() {type, alg}
        End Function

        Private Const ALIAS_PREFIX As String = "Alg.Alias."
        Private Const ALIAS_PREFIX_LOWER As String = "alg.alias."
        Private Shared ReadOnly ALIAS_LENGTH As Integer = ALIAS_PREFIX.Length()

        Private Sub parseLegacyPut(  name As String,   value As String)
            If name.ToLower(ENGLISH).StartsWith(ALIAS_PREFIX_LOWER) Then
                ' e.g. put("Alg.Alias.MessageDigest.SHA", "SHA-1");
                ' aliasKey ~ MessageDigest.SHA
                Dim stdAlg As String = value
                Dim aliasKey As String = name.Substring(ALIAS_LENGTH)
                Dim typeAndAlg As String() = getTypeAndAlgorithm(aliasKey)
                If typeAndAlg Is Nothing Then Return
                Dim type As String = getEngineName(typeAndAlg(0))
                Dim aliasAlg As String = typeAndAlg(1).Intern()
                Dim key As New ServiceKey(type, stdAlg, True)
                Dim s As Service = legacyMap.get(key)
                If s Is Nothing Then
                    s = New Service(Me)
                    s.type = type
                    s.algorithm = stdAlg
                    legacyMap.put(key, s)
                End If
                legacyMap.put(New ServiceKey(type, aliasAlg, True), s)
                s.addAlias(aliasAlg)
            Else
                Dim typeAndAlg As String() = getTypeAndAlgorithm(name)
                If typeAndAlg Is Nothing Then Return
                Dim i As Integer = typeAndAlg(1).IndexOf(" "c)
                If i = -1 Then
                    ' e.g. put("MessageDigest.SHA-1", "sun.security.provider.SHA");
                    Dim type As String = getEngineName(typeAndAlg(0))
                    Dim stdAlg As String = typeAndAlg(1).Intern()
                    Dim className As String = value
                    Dim key As New ServiceKey(type, stdAlg, True)
                    Dim s As Service = legacyMap.get(key)
                    If s Is Nothing Then
                        s = New Service(Me)
                        s.type = type
                        s.algorithm = stdAlg
                        legacyMap.put(key, s)
                    End If
                    s.className = className ' attribute
                Else
                    ' e.g. put("MessageDigest.SHA-1 ImplementedIn", "Software");
                    Dim attributeValue As String = value
                    Dim type As String = getEngineName(typeAndAlg(0))
                    Dim attributeString As String = typeAndAlg(1)
                    Dim stdAlg As String = attributeString.Substring(0, i).Intern()
                    Dim attributeName As String = attributeString.Substring(i + 1)
                    ' kill additional spaces
                    Do While attributeName.StartsWith(" ")
                        attributeName = attributeName.Substring(1)
                    Loop
                    attributeName = attributeName.Intern()
                    Dim key As New ServiceKey(type, stdAlg, True)
                    Dim s As Service = legacyMap.get(key)
                    If s Is Nothing Then
                        s = New Service(Me)
                        s.type = type
                        s.algorithm = stdAlg
                        legacyMap.put(key, s)
                    End If
                    s.addAttribute(attributeName, attributeValue)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Get the service describing this Provider's implementation of the
        ''' specified type of this algorithm or alias. If no such
        ''' implementation exists, this method returns null. If there are two
        ''' matching services, one added to this provider using
        ''' <seealso cref="#putService putService()"/> and one added via <seealso cref="#put put()"/>,
        ''' the service added via <seealso cref="#putService putService()"/> is returned.
        ''' </summary>
        ''' <param name="type"> the type of <seealso cref="Service service"/> requested
        ''' (for example, {@code MessageDigest}) </param>
        ''' <param name="algorithm"> the case insensitive algorithm name (or alternate
        ''' alias) of the service requested (for example, {@code SHA-1})
        ''' </param>
        ''' <returns> the service describing this Provider's matching service
        ''' or null if no such service exists
        ''' </returns>
        ''' <exception cref="NullPointerException"> if type or algorithm is null
        ''' 
        ''' @since 1.5 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Function getService(  type As String,   algorithm As String) As Service
            checkInitialized()
            ' avoid allocating a new key object if possible
            Dim key As ServiceKey = previousKey
            If key.matches(type, algorithm) = False Then
                key = New ServiceKey(type, algorithm, False)
                previousKey = key
            End If
            If serviceMap IsNot Nothing Then
                Dim service_Renamed As Service = serviceMap.get(key)
                If service_Renamed IsNot Nothing Then Return service_Renamed
            End If
            ensureLegacyParsed()
            Return If(legacyMap IsNot Nothing, legacyMap.get(key), Nothing)
        End Function

        ' ServiceKey from previous getService() call
        ' by re-using it if possible we avoid allocating a new object
        ' and the toUpperCase() call.
        ' re-use will occur e.g. as the framework traverses the provider
        ' list and queries each provider with the same values until it finds
        ' a matching service
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        Private Shared previousKey As New ServiceKey("", "", False)

        ''' <summary>
        ''' Get an unmodifiable Set of all services supported by
        ''' this Provider.
        ''' </summary>
        ''' <returns> an unmodifiable Set of all services supported by
        ''' this Provider
        ''' 
        ''' @since 1.5 </returns>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property services As [Set](Of Service)
            Get
                checkInitialized()
                If legacyChanged OrElse servicesChanged Then serviceSet = Nothing
                If serviceSet Is Nothing Then
                    ensureLegacyParsed()
                    Dim [set] As [Set](Of Service) = New LinkedHashSet(Of Service)
                    If serviceMap IsNot Nothing Then [set].addAll(serviceMap.values())
                    If legacyMap IsNot Nothing Then [set].addAll(legacyMap.values())
                    serviceSet = Collections.unmodifiableSet([set])
                    servicesChanged = False
                End If
                Return serviceSet
            End Get
        End Property

        ''' <summary>
        ''' Add a service. If a service of the same type with the same algorithm
        ''' name exists and it was added using <seealso cref="#putService putService()"/>,
        ''' it is replaced by the new service.
        ''' This method also places information about this service
        ''' in the provider's Hashtable values in the format described in the
        ''' <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
        ''' Java Cryptography Architecture API Specification &amp; Reference </a>.
        ''' 
        ''' <p>Also, if there is a security manager, its
        ''' {@code checkSecurityAccess} method is called with the string
        ''' {@code "putProviderProperty."+name}, where {@code name} is
        ''' the provider name, to see if it's ok to set this provider's property
        ''' values. If the default implementation of {@code checkSecurityAccess}
        ''' is used (that is, that method is not overriden), then this results in
        ''' a call to the security manager's {@code checkPermission} method with
        ''' a {@code SecurityPermission("putProviderProperty."+name)}
        ''' permission.
        ''' </summary>
        ''' <param name="s"> the Service to add
        ''' </param>
        ''' <exception cref="SecurityException">
        '''      if a security manager exists and its {@link
        '''      java.lang.SecurityManager#checkSecurityAccess} method denies
        '''      access to set property values. </exception>
        ''' <exception cref="NullPointerException"> if s is null
        ''' 
        ''' @since 1.5 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Protected Friend Overridable Sub putService(  s As Service)
            check("putProviderProperty." & name)
            If debug IsNot Nothing Then debug.println(name & ".putService(): " & s)
            If s Is Nothing Then Throw New NullPointerException
            If s.provider IsNot Me Then Throw New IllegalArgumentException("service.getProvider() must match this Provider object")
            If serviceMap Is Nothing Then serviceMap = New LinkedHashMap(Of ServiceKey, Service)
            servicesChanged = True
            Dim type As String = s.type
            Dim algorithm As String = s.algorithm
            Dim key As New ServiceKey(type, algorithm, True)
            ' remove existing service
            implRemoveService(serviceMap.get(key))
            serviceMap.put(key, s)
            For Each [alias] As String In s.aliases
                serviceMap.put(New ServiceKey(type, [alias], True), s)
            Next [alias]
            putPropertyStrings(s)
        End Sub

        ''' <summary>
        ''' Put the string properties for this Service in this Provider's
        ''' Hashtable.
        ''' </summary>
        Private Sub putPropertyStrings(  s As Service)
            Dim type As String = s.type
            Dim algorithm As String = s.algorithm
            ' use super() to avoid permission check and other processing
            MyBase.put(type & "." & algorithm, s.className)
            For Each [alias] As String In s.aliases
                MyBase.put(ALIAS_PREFIX + type & "." & [alias], algorithm)
            Next [alias]
            For Each entry As KeyValuePair(Of UString, String) In s.attributes.entrySet()
                Dim key As String = type & "." & algorithm & " " & entry.Key
                MyBase.put(key, entry.Value)
            Next entry
        End Sub

        ''' <summary>
        ''' Remove the string properties for this Service from this Provider's
        ''' Hashtable.
        ''' </summary>
        Private Sub removePropertyStrings(  s As Service)
            Dim type As String = s.type
            Dim algorithm As String = s.algorithm
            ' use super() to avoid permission check and other processing
            MyBase.remove(type & "." & algorithm)
            For Each [alias] As String In s.aliases
                MyBase.remove(ALIAS_PREFIX + type & "." & [alias])
            Next [alias]
            For Each entry As KeyValuePair(Of UString, String) In s.attributes.entrySet()
                Dim key As String = type & "." & algorithm & " " & entry.Key
                MyBase.remove(key)
            Next entry
        End Sub

        ''' <summary>
        ''' Remove a service previously added using
        ''' <seealso cref="#putService putService()"/>. The specified service is removed from
        ''' this provider. It will no longer be returned by
        ''' <seealso cref="#getService getService()"/> and its information will be removed
        ''' from this provider's Hashtable.
        ''' 
        ''' <p>Also, if there is a security manager, its
        ''' {@code checkSecurityAccess} method is called with the string
        ''' {@code "removeProviderProperty."+name}, where {@code name} is
        ''' the provider name, to see if it's ok to remove this provider's
        ''' properties. If the default implementation of
        ''' {@code checkSecurityAccess} is used (that is, that method is not
        ''' overriden), then this results in a call to the security manager's
        ''' {@code checkPermission} method with a
        ''' {@code SecurityPermission("removeProviderProperty."+name)}
        ''' permission.
        ''' </summary>
        ''' <param name="s"> the Service to be removed
        ''' </param>
        ''' <exception cref="SecurityException">
        '''          if a security manager exists and its {@link
        '''          java.lang.SecurityManager#checkSecurityAccess} method denies
        '''          access to remove this provider's properties. </exception>
        ''' <exception cref="NullPointerException"> if s is null
        ''' 
        ''' @since 1.5 </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Protected Friend Overridable Sub removeService(  s As Service)
            check("removeProviderProperty." & name)
            If debug IsNot Nothing Then debug.println(name & ".removeService(): " & s)
            If s Is Nothing Then Throw New NullPointerException
            implRemoveService(s)
        End Sub

        Private Sub implRemoveService(  s As Service)
            If (s Is Nothing) OrElse (serviceMap Is Nothing) Then Return
            Dim type As String = s.type
            Dim algorithm As String = s.algorithm
            Dim key As New ServiceKey(type, algorithm, False)
            Dim oldService As Service = serviceMap.get(key)
            If s IsNot oldService Then Return
            servicesChanged = True
            serviceMap.remove(key)
            For Each [alias] As String In s.aliases
                serviceMap.remove(New ServiceKey(type, [alias], False))
            Next [alias]
            removePropertyStrings(s)
        End Sub

        ' Wrapped String that behaves in a case insensitive way for equals/hashCode
        Private Class UString
            Friend ReadOnly string_Renamed As String
            Friend ReadOnly lowerString As String

            Friend Sub New(  s As String)
                Me.string_Renamed = s
                Me.lowerString = s.ToLower(ENGLISH)
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return lowerString.GetHashCode()
            End Function

            Public Overrides Function Equals(  obj As Object) As Boolean
                If Me Is obj Then Return True
                If TypeOf obj Is UString = False Then Return False
                Dim other As UString = CType(obj, UString)
                Return lowerString.Equals(other.lowerString)
            End Function

            Public Overrides Function ToString() As String
                Return string_Renamed
            End Function
        End Class

        ' describe relevant properties of a type of engine
        Private Class EngineDescription
            Friend ReadOnly name As String
            Friend ReadOnly supportsParameter As Boolean
            Friend ReadOnly constructorParameterClassName As String
            Private _constructorParameterClass As [Class]

            Friend Sub New(  name As String,   sp As Boolean,   paramName As String)
                Me.name = name
                Me.supportsParameter = sp
                Me.constructorParameterClassName = paramName
            End Sub
            Friend Overridable ReadOnly Property constructorParameterClass As [Class]
                Get
                    Dim clazz As [Class] = _constructorParameterClass
                    If clazz Is Nothing Then
                        clazz = Type.GetType(constructorParameterClassName)
                        constructorParameterClass = clazz
                    End If
                    Return clazz
                End Get
            End Property
        End Class

        ' built in knowledge of the engine types shipped as part of the JDK
        Private Shared ReadOnly knownEngines As Map(Of String, EngineDescription)

        Private Shared Sub addEngine(  name As String,   sp As Boolean,   paramName As String)
            Dim ed As New EngineDescription(name, sp, paramName)
            ' also index by canonical name to avoid toLowerCase() for some lookups
            knownEngines.put(name.ToLower(ENGLISH), ed)
            knownEngines.put(name, ed)
        End Sub

        Shared Sub New()
            knownEngines = New HashMap(Of String, EngineDescription)
            ' JCA
            addEngine("AlgorithmParameterGenerator", False, Nothing)
            addEngine("AlgorithmParameters", False, Nothing)
            addEngine("KeyFactory", False, Nothing)
            addEngine("KeyPairGenerator", False, Nothing)
            addEngine("KeyStore", False, Nothing)
            addEngine("MessageDigest", False, Nothing)
            addEngine("SecureRandom", False, Nothing)
            addEngine("Signature", True, Nothing)
            addEngine("CertificateFactory", False, Nothing)
            addEngine("CertPathBuilder", False, Nothing)
            addEngine("CertPathValidator", False, Nothing)
            addEngine("CertStore", False, "java.security.cert.CertStoreParameters")
            ' JCE
            addEngine("Cipher", True, Nothing)
            addEngine("ExemptionMechanism", False, Nothing)
            addEngine("Mac", True, Nothing)
            addEngine("KeyAgreement", True, Nothing)
            addEngine("KeyGenerator", False, Nothing)
            addEngine("SecretKeyFactory", False, Nothing)
            ' JSSE
            addEngine("KeyManagerFactory", False, Nothing)
            addEngine("SSLContext", False, Nothing)
            addEngine("TrustManagerFactory", False, Nothing)
            ' JGSS
            addEngine("GssApiMechanism", False, Nothing)
            ' SASL
            addEngine("SaslClientFactory", False, Nothing)
            addEngine("SaslServerFactory", False, Nothing)
            ' POLICY
            addEngine("Policy", False, "java.security.Policy$Parameters")
            ' CONFIGURATION
            addEngine("Configuration", False, "javax.security.auth.login.Configuration$Parameters")
            ' XML DSig
            addEngine("XMLSignatureFactory", False, Nothing)
            addEngine("KeyInfoFactory", False, Nothing)
            addEngine("TransformService", False, Nothing)
            ' Smart Card I/O
            addEngine("TerminalFactory", False, "java.lang.Object")
        End Sub

        ' get the "standard" (mixed-case) engine name for arbitary case engine name
        ' if there is no known engine by that name, return s
        Private Shared Function getEngineName(  s As String) As String
            ' try original case first, usually correct
            Dim e As EngineDescription = knownEngines.get(s)
            If e Is Nothing Then e = knownEngines.get(s.ToLower(ENGLISH))
            Return If(e Is Nothing, s, e.name)
        End Function

        ''' <summary>
        ''' The description of a security service. It encapsulates the properties
        ''' of a service and contains a factory method to obtain new implementation
        ''' instances of this service.
        ''' 
        ''' <p>Each service has a provider that offers the service, a type,
        ''' an algorithm name, and the name of the class that implements the
        ''' service. Optionally, it also includes a list of alternate algorithm
        ''' names for this service (aliases) and attributes, which are a map of
        ''' (name, value) String pairs.
        ''' 
        ''' <p>This class defines the methods {@link #supportsParameter
        ''' supportsParameter()} and <seealso cref="#newInstance newInstance()"/>
        ''' which are used by the Java security framework when it searches for
        ''' suitable services and instantiates them. The valid arguments to those
        ''' methods depend on the type of service. For the service types defined
        ''' within Java SE, see the
        ''' <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
        ''' Java Cryptography Architecture API Specification &amp; Reference </a>
        ''' for the valid values.
        ''' Note that components outside of Java SE can define additional types of
        ''' services and their behavior.
        ''' 
        ''' <p>Instances of this class are immutable.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Class Service

            Private type, algorithm, className As String
            Private ReadOnly provider_Renamed As Provider
            Private aliases As List(Of String)
            Private attributes As Map(Of UString, String)

            ' Reference to the cached implementation Class object
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Private classRef As Reference(Of [Class])

            ' flag indicating whether this service has its attributes for
            ' supportedKeyFormats or supportedKeyClasses set
            ' if null, the values have not been initialized
            ' if TRUE, at least one of supportedFormats/Classes is non null
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Private hasKeyAttributes_Renamed As Boolean?

            ' supported encoding formats
            Private supportedFormats As String()

            ' names of the supported key (super) classes
            Private supportedClasses As [Class]()

            ' whether this service has been registered with the Provider
            Private registered As Boolean

            Private Shared ReadOnly CLASS0 As [Class]() = New [Class]() {}

            ' this constructor and these methods are used for parsing
            ' the legacy string properties.

            Private Sub New(  provider_Renamed As Provider)
                Me.provider_Renamed = provider_Renamed
                aliases = Collections.emptyList(Of String)()
                attributes = Collections.emptyMap(Of UString, String)()
            End Sub

            Private Property valid As Boolean
                Get
                    Return (type IsNot Nothing) AndAlso (algorithm IsNot Nothing) AndAlso (className IsNot Nothing)
                End Get
            End Property

            Private Sub addAlias(  [alias] As String)
                If aliases.empty Then aliases = New List(Of String)(2)
                aliases.add([alias])
            End Sub

            Friend Overridable Sub addAttribute(  type As String,   value As String)
                If attributes.empty Then attributes = New HashMap(Of UString, String)(8)
                attributes.put(New UString(type), value)
            End Sub

            ''' <summary>
            ''' Construct a new service.
            ''' </summary>
            ''' <param name="provider"> the provider that offers this service </param>
            ''' <param name="type"> the type of this service </param>
            ''' <param name="algorithm"> the algorithm name </param>
            ''' <param name="className"> the name of the class implementing this service </param>
            ''' <param name="aliases"> List of aliases or null if algorithm has no aliases </param>
            ''' <param name="attributes"> Map of attributes or null if this implementation
            '''                   has no attributes
            ''' </param>
            ''' <exception cref="NullPointerException"> if provider, type, algorithm, or
            ''' className is null </exception>
            Public Sub New(  provider_Renamed As Provider,   type As String,   algorithm As String,   className As String,   aliases As List(Of String),   attributes As Map(Of String, String))
                If (provider_Renamed Is Nothing) OrElse (type Is Nothing) OrElse (algorithm Is Nothing) OrElse (className Is Nothing) Then Throw New NullPointerException
                Me.provider_Renamed = provider_Renamed
                Me.type = getEngineName(type)
                Me.algorithm = algorithm
                Me.className = className
                If aliases Is Nothing Then
                    Me.aliases = Collections.emptyList(Of String)()
                Else
                    Me.aliases = New List(Of String)(aliases)
                End If
                If attributes Is Nothing Then
                    Me.attributes = Collections.emptyMap(Of UString, String)()
                Else
                    Me.attributes = New HashMap(Of UString, String)
                    For Each entry As KeyValuePair(Of String, String) In attributes.entrySet()
                        Me.attributes.put(New UString(entry.Key), entry.Value)
                    Next entry
                End If
            End Sub

            ''' <summary>
            ''' Get the type of this service. For example, {@code MessageDigest}.
            ''' </summary>
            ''' <returns> the type of this service </returns>
            Public Property type As String
                Get
                    Return type
                End Get
            End Property

            ''' <summary>
            ''' Return the name of the algorithm of this service. For example,
            ''' {@code SHA-1}.
            ''' </summary>
            ''' <returns> the algorithm of this service </returns>
            Public Property algorithm As String
                Get
                    Return algorithm
                End Get
            End Property

            ''' <summary>
            ''' Return the Provider of this service.
            ''' </summary>
            ''' <returns> the Provider of this service </returns>
            Public Property provider As Provider
                Get
                    Return provider_Renamed
                End Get
            End Property

            ''' <summary>
            ''' Return the name of the class implementing this service.
            ''' </summary>
            ''' <returns> the name of the class implementing this service </returns>
            Public Property className As String
                Get
                    Return className
                End Get
            End Property

            ' internal only
            Private Property aliases As List(Of String)
                Get
                    Return aliases
                End Get
            End Property

            ''' <summary>
            ''' Return the value of the specified attribute or null if this
            ''' attribute is not set for this Service.
            ''' </summary>
            ''' <param name="name"> the name of the requested attribute
            ''' </param>
            ''' <returns> the value of the specified attribute or null if the
            '''         attribute is not present
            ''' </returns>
            ''' <exception cref="NullPointerException"> if name is null </exception>
            Public Function getAttribute(  name As String) As String
                If name Is Nothing Then Throw New NullPointerException
                Return attributes.get(New UString(name))
            End Function

            ''' <summary>
            ''' Return a new instance of the implementation described by this
            ''' service. The security provider framework uses this method to
            ''' construct implementations. Applications will typically not need
            ''' to call it.
            ''' 
            ''' <p>The default implementation uses reflection to invoke the
            ''' standard constructor for this type of service.
            ''' Security providers can override this method to implement
            ''' instantiation in a different way.
            ''' For details and the values of constructorParameter that are
            ''' valid for the various types of services see the
            ''' <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
            ''' Java Cryptography Architecture API Specification &amp;
            ''' Reference</a>.
            ''' </summary>
            ''' <param name="constructorParameter"> the value to pass to the constructor,
            ''' or null if this type of service does not use a constructorParameter.
            ''' </param>
            ''' <returns> a new implementation of this service
            ''' </returns>
            ''' <exception cref="InvalidParameterException"> if the value of
            ''' constructorParameter is invalid for this type of service. </exception>
            ''' <exception cref="NoSuchAlgorithmException"> if instantiation failed for
            ''' any other reason. </exception>
            Public Overridable Function newInstance(  constructorParameter As Object) As Object
                If registered = False Then
                    If provider_Renamed.getService(type, algorithm) IsNot Me Then Throw New NoSuchAlgorithmException("Service not registered with Provider " & provider_Renamed.name & ": " & Me)
                    registered = True
                End If
                Try
                    Dim cap As EngineDescription = knownEngines.get(type)
                    If cap Is Nothing Then Return newInstanceGeneric(constructorParameter)
                    If cap.constructorParameterClassName Is Nothing Then
                        If constructorParameter IsNot Nothing Then Throw New InvalidParameterException("constructorParameter not used with " & type & " engines")
                        Dim clazz As [Class] = implClass
                        Dim empty As [Class]() = {}
                        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                        Dim con As Constructor(Of ?) = clazz.getConstructor(empty)
                        Return con.newInstance()
                    Else
                        Dim paramClass As [Class] = cap.constructorParameterClass
                        If constructorParameter IsNot Nothing Then
                            Dim argClass As [Class] = constructorParameter.GetType()
                            If argClass.IsSubclassOf(paramClass) = False Then Throw New InvalidParameterException("constructorParameter must be instanceof " & cap.constructorParameterClassName.Replace("$"c, "."c) & " for engine type " & type)
                        End If
                        Dim clazz As [Class] = implClass
                        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                        Dim cons As Constructor(Of ?) = clazz.getConstructor(paramClass)
                        Return cons.newInstance(constructorParameter)
                    End If
                Catch e As NoSuchAlgorithmException
                    Throw e
                Catch e As InvocationTargetException
                    Throw New NoSuchAlgorithmException("Error constructing implementation (algorithm: " & algorithm & ", provider: " & provider_Renamed.name & ", class: " & className & ")", e.InnerException)
                Catch e As Exception
                    Throw New NoSuchAlgorithmException("Error constructing implementation (algorithm: " & algorithm & ", provider: " & provider_Renamed.name & ", class: " & className & ")", e)
                End Try
            End Function

            ' return the implementation Class object for this service
            Private Property implClass As [Class]
                Get
                    Try
                        Dim ref As Reference(Of [Class]) = classRef
                        Dim clazz As [Class] = If(ref Is Nothing, Nothing, ref.get())
                        If clazz Is Nothing Then
                            Dim cl As ClassLoader = provider_Renamed.GetType().classLoader
                            If cl Is Nothing Then
                                clazz = type.GetType(className)
                            Else
                                clazz = cl.loadClass(className)
                            End If
                            If Not Modifier.isPublic(clazz.modifiers) Then Throw New NoSuchAlgorithmException("class configured for " & type & " (provider: " & provider_Renamed.name & ") is not public.")
                            classRef = New WeakReference(Of [Class])(clazz)
                        End If
                        Return clazz
                    Catch e As ClassNotFoundException
                        Throw New NoSuchAlgorithmException("class configured for " & type & " (provider: " & provider_Renamed.name & ") cannot be found.", e)
                    End Try
                End Get
            End Property

            ''' <summary>
            ''' Generic code path for unknown engine types. Call the
            ''' no-args constructor if constructorParameter is null, otherwise
            ''' use the first matching constructor.
            ''' </summary>
            Private Function newInstanceGeneric(  constructorParameter As Object) As Object
                Dim clazz As [Class] = implClass
                If constructorParameter Is Nothing Then
                    ' create instance with public no-arg constructor if it exists
                    Try
                        Dim empty As [Class]() = {}
                        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                        Dim con As Constructor(Of ?) = clazz.getConstructor(empty)
                        Return con.newInstance()
                    Catch e As NoSuchMethodException
                        Throw New NoSuchAlgorithmException("No public no-arg " & "constructor found in class " & className)
                    End Try
                End If
                Dim argClass As [Class] = constructorParameter.GetType()
                Dim cons As Constructor() = clazz.constructors
                ' find first public constructor that can take the
                ' argument as parameter
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                For Each con As Constructor(Of ?) In cons
                    Dim paramTypes As [Class]() = con.parameterTypes
                    If paramTypes.Length <> 1 Then Continue For
                    If argClass.IsSubclassOf(paramTypes(0)) = False Then Continue For
                    Return con.newInstance(constructorParameter)
                Next con
                Throw New NoSuchAlgorithmException("No public constructor matching " & argClass.name & " found in class " & className)
            End Function

            ''' <summary>
            ''' Test whether this Service can use the specified parameter.
            ''' Returns false if this service cannot use the parameter. Returns
            ''' true if this service can use the parameter, if a fast test is
            ''' infeasible, or if the status is unknown.
            ''' 
            ''' <p>The security provider framework uses this method with
            ''' some types of services to quickly exclude non-matching
            ''' implementations for consideration.
            ''' Applications will typically not need to call it.
            ''' 
            ''' <p>For details and the values of parameter that are valid for the
            ''' various types of services see the top of this class and the
            ''' <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">
            ''' Java Cryptography Architecture API Specification &amp;
            ''' Reference</a>.
            ''' Security providers can override it to implement their own test.
            ''' </summary>
            ''' <param name="parameter"> the parameter to test
            ''' </param>
            ''' <returns> false if this this service cannot use the specified
            ''' parameter; true if it can possibly use the parameter
            ''' </returns>
            ''' <exception cref="InvalidParameterException"> if the value of parameter is
            ''' invalid for this type of service or if this method cannot be
            ''' used with this type of service </exception>
            Public Overridable Function supportsParameter(  parameter As Object) As Boolean
                Dim cap As EngineDescription = knownEngines.get(type)
                If cap Is Nothing Then Return True
                If cap.supportsParameter = False Then Throw New InvalidParameterException("supportsParameter() not " & "used with " & type & " engines")
                ' allow null for keys without attributes for compatibility
                If (parameter IsNot Nothing) AndAlso (TypeOf parameter Is Key = False) Then Throw New InvalidParameterException("Parameter must be instanceof Key for engine " & type)
                If hasKeyAttributes() = False Then Return True
                If parameter Is Nothing Then Return False
                Dim key As Key = CType(parameter, Key)
                If supportsKeyFormat(key) Then Return True
                If supportsKeyClass(key) Then Return True
                Return False
            End Function

            ''' <summary>
            ''' Return whether this service has its Supported* properties for
            ''' keys defined. Parses the attributes if not yet initialized.
            ''' </summary>
            Private Function hasKeyAttributes() As Boolean
                Dim b As Boolean? = hasKeyAttributes_Renamed
                If b Is Nothing Then
                    SyncLock Me
                        Dim s As String
                        s = getAttribute("SupportedKeyFormats")
                        If s IsNot Nothing Then supportedFormats = s.Split("\|")
                        s = getAttribute("SupportedKeyClasses")
                        If s IsNot Nothing Then
                            Dim classNames As String() = s.Split("\|")
                            Dim classList As List(Of [Class]) = New List(Of [Class])(classNames.Length)
                            For Each className_Renamed As String In classNames
                                Dim clazz As [Class] = getKeyClass(className_Renamed)
                                If clazz IsNot Nothing Then classList.add(clazz)
                            Next className_Renamed
                            supportedClasses = classList.ToArray(CLASS0)
                        End If
                        Dim bool As Boolean = (supportedFormats IsNot Nothing) OrElse (supportedClasses IsNot Nothing)
                        b = Convert.ToBoolean(bool)
                        hasKeyAttributes_Renamed = b
                    End SyncLock
                End If
                Return b
            End Function

            ' get the key class object of the specified name
            Private Function getKeyClass(  name As String) As [Class]
                Try
                    Return type.GetType(name)
                Catch e As ClassNotFoundException
                    ' ignore
                End Try
                Try
                    Dim cl As ClassLoader = provider_Renamed.GetType().classLoader
                    If cl IsNot Nothing Then Return cl.loadClass(name)
                Catch e As ClassNotFoundException
                    ' ignore
                End Try
                Return Nothing
            End Function

            Private Function supportsKeyFormat(  key As Key) As Boolean
                If supportedFormats Is Nothing Then Return False
                Dim format As String = key.format
                If format Is Nothing Then Return False
                For Each supportedFormat As String In supportedFormats
                    If supportedFormat.Equals(format) Then Return True
                Next supportedFormat
                Return False
            End Function

            Private Function supportsKeyClass(  key As Key) As Boolean
                If supportedClasses Is Nothing Then Return False
                Dim keyClass_Renamed As [Class] = key.GetType()
                For Each clazz As [Class] In supportedClasses
                    If keyClass_Renamed.IsSubclassOf(clazz) Then Return True
                Next clazz
                Return False
            End Function

            ''' <summary>
            ''' Return a String representation of this service.
            ''' </summary>
            ''' <returns> a String representation of this service. </returns>
            Public Overrides Function ToString() As String
                Dim aString As String = If(aliases.empty, "", vbCrLf & "  aliases: " & aliases.ToString())
                Dim attrs As String = If(attributes.empty, "", vbCrLf & "  attributes: " & attributes.ToString())
                Return provider_Renamed.name & ": " & type & "." & algorithm & " -> " & className + aString + attrs & vbCrLf
            End Function

        End Class

    End Class

End Namespace
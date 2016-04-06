Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2009-2012, Stephen Colebourne & Michael Nascimento Santos
' *
' * All rights reserved.
' *
' * Redistribution and use in source and binary forms, with or without
' * modification, are permitted provided that the following conditions are met:
' *
' *  * Redistributions of source code must retain the above copyright notice,
' *    this list of conditions and the following disclaimer.
' *
' *  * Redistributions in binary form must reproduce the above copyright notice,
' *    this list of conditions and the following disclaimer in the documentation
' *    and/or other materials provided with the distribution.
' *
' *  * Neither the name of JSR-310 nor the names of its contributors
' *    may be used to endorse or promote products derived from this software
' *    without specific prior written permission.
' *
' * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
' * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
' * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
' * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
' * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
' * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
' * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
' * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
' * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
' * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
' * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
' 
Namespace java.time.zone


	''' <summary>
	''' Provider of time-zone rules to the system.
	''' <p>
	''' This class manages the configuration of time-zone rules.
	''' The static methods provide the public API that can be used to manage the providers.
	''' The abstract methods provide the SPI that allows rules to be provided.
	''' <p>
	''' ZoneRulesProvider may be installed in an instance of the Java Platform as
	''' extension classes, that is, jar files placed into any of the usual extension
	''' directories. Installed providers are loaded using the service-provider loading
	''' facility defined by the <seealso cref="ServiceLoader"/> class. A ZoneRulesProvider
	''' identifies itself with a provider configuration file named
	''' {@code java.time.zone.ZoneRulesProvider} in the resource directory
	''' {@code META-INF/services}. The file should contain a line that specifies the
	''' fully qualified concrete zonerules-provider class name.
	''' Providers may also be made available by adding them to the class path or by
	''' registering themselves via <seealso cref="#registerProvider"/> method.
	''' <p>
	''' The Java virtual machine has a default provider that provides zone rules
	''' for the time-zones defined by IANA Time Zone Database (TZDB). If the system
	''' property {@code java.time.zone.DefaultZoneRulesProvider} is defined then
	''' it is taken to be the fully-qualified name of a concrete ZoneRulesProvider
	''' class to be loaded as the default provider, using the system class loader.
	''' If this system property is not defined, a system-default provider will be
	''' loaded to serve as the default provider.
	''' <p>
	''' Rules are looked up primarily by zone ID, as used by <seealso cref="ZoneId"/>.
	''' Only zone region IDs may be used, zone offset IDs are not used here.
	''' <p>
	''' Time-zone rules are political, thus the data can change at any time.
	''' Each provider will provide the latest rules for each zone ID, but they
	''' may also provide the history of how the rules changed.
	''' 
	''' @implSpec
	''' This interface is a service provider that can be called by multiple threads.
	''' Implementations must be immutable and thread-safe.
	''' <p>
	''' Providers must ensure that once a rule has been seen by the application, the
	''' rule must continue to be available.
	''' <p>
	'''  Providers are encouraged to implement a meaningful {@code toString} method.
	''' <p>
	''' Many systems would like to update time-zone rules dynamically without stopping the JVM.
	''' When examined in detail, this is a complex problem.
	''' Providers may choose to handle dynamic updates, however the default provider does not.
	''' 
	''' @since 1.8
	''' </summary>
	Public MustInherit Class ZoneRulesProvider

		''' <summary>
		''' The set of loaded providers.
		''' </summary>
		Private Shared ReadOnly PROVIDERS As New java.util.concurrent.CopyOnWriteArrayList(Of ZoneRulesProvider)
		''' <summary>
		''' The lookup from zone ID to provider.
		''' </summary>
		Private Shared ReadOnly ZONES As java.util.concurrent.ConcurrentMap(Of String, ZoneRulesProvider) = New ConcurrentDictionary(Of String, ZoneRulesProvider)(512, 0.75f, 2)

		Shared Sub New()
			' if the property java.time.zone.DefaultZoneRulesProvider is
			' set then its value is the class name of the default provider
			Dim loaded As IList(Of ZoneRulesProvider) = New List(Of ZoneRulesProvider)
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			Dim sl As java.util.ServiceLoader(Of ZoneRulesProvider) = java.util.ServiceLoader.load(GetType(ZoneRulesProvider), ClassLoader.systemClassLoader)
			Dim it As IEnumerator(Of ZoneRulesProvider) = sl.GetEnumerator()
			Do While it.MoveNext()
				Dim provider_Renamed As ZoneRulesProvider
				Try
					provider_Renamed = it.Current
				Catch ex As java.util.ServiceConfigurationError
					If TypeOf ex.cause Is SecurityException Then Continue Do ' ignore the security exception, try the next provider
					Throw ex
				End Try
				Dim found As Boolean = False
				For Each p As ZoneRulesProvider In loaded
					If p.GetType() Is provider_Renamed.GetType() Then found = True
				Next p
				If Not found Then
					registerProvider0(provider_Renamed)
					loaded.Add(provider_Renamed)
				End If
			Loop
			' CopyOnWriteList could be slow if lots of providers and each added individually
			PROVIDERS.addAll(loaded)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				Dim prop As String = System.getProperty("java.time.zone.DefaultZoneRulesProvider")
				If prop IsNot Nothing Then
					Try
						Dim c As  [Class] = Type.GetType(prop, True, ClassLoader.systemClassLoader)
						Dim provider As ZoneRulesProvider = GetType(ZoneRulesProvider).cast(c.newInstance())
						registerProvider(provider)
						loaded.add(provider)
					Catch x As Exception
						Throw New [Error](x)
					End Try
				Else
					registerProvider(New TzdbZoneRulesProvider)
				End If
				Return Nothing
			End Function
		End Class

		'-------------------------------------------------------------------------
		''' <summary>
		''' Gets the set of available zone IDs.
		''' <p>
		''' These IDs are the string form of a <seealso cref="ZoneId"/>.
		''' </summary>
		''' <returns> a modifiable copy of the set of zone IDs, not null </returns>
		PublicShared ReadOnly PropertyavailableZoneIds As java.util.Set(Of String)
			Get
				Return New HashSet(Of )(ZONES.Keys)
			End Get
		End Property

		''' <summary>
		''' Gets the rules for the zone ID.
		''' <p>
		''' This returns the latest available rules for the zone ID.
		''' <p>
		''' This method relies on time-zone data provider files that are configured.
		''' These are loaded using a {@code ServiceLoader}.
		''' <p>
		''' The caching flag is designed to allow provider implementations to
		''' prevent the rules being cached in {@code ZoneId}.
		''' Under normal circumstances, the caching of zone rules is highly desirable
		''' as it will provide greater performance. However, there is a use case where
		''' the caching would not be desirable, see <seealso cref="#provideRules"/>.
		''' </summary>
		''' <param name="zoneId"> the zone ID as defined by {@code ZoneId}, not null </param>
		''' <param name="forCaching"> whether the rules are being queried for caching,
		''' true if the returned rules will be cached by {@code ZoneId},
		''' false if they will be returned to the user without being cached in {@code ZoneId} </param>
		''' <returns> the rules, null if {@code forCaching} is true and this
		''' is a dynamic provider that wants to prevent caching in {@code ZoneId},
		''' otherwise not null </returns>
		''' <exception cref="ZoneRulesException"> if rules cannot be obtained for the zone ID </exception>
		Public Shared Function getRules(  zoneId_Renamed As String,   forCaching As Boolean) As ZoneRules
			java.util.Objects.requireNonNull(zoneId_Renamed, "zoneId")
			Return getProvider(zoneId_Renamed).provideRules(zoneId_Renamed, forCaching)
		End Function

		''' <summary>
		''' Gets the history of rules for the zone ID.
		''' <p>
		''' Time-zones are defined by governments and change frequently.
		''' This method allows applications to find the history of changes to the
		''' rules for a single zone ID. The map is keyed by a string, which is the
		''' version string associated with the rules.
		''' <p>
		''' The exact meaning and format of the version is provider specific.
		''' The version must follow lexicographical order, thus the returned map will
		''' be order from the oldest known rules to the newest available rules.
		''' The default 'TZDB' group uses version numbering consisting of the year
		''' followed by a letter, such as '2009e' or '2012f'.
		''' <p>
		''' Implementations must provide a result for each valid zone ID, however
		''' they do not have to provide a history of rules.
		''' Thus the map will always contain one element, and will only contain more
		''' than one element if historical rule information is available.
		''' </summary>
		''' <param name="zoneId">  the zone ID as defined by {@code ZoneId}, not null </param>
		''' <returns> a modifiable copy of the history of the rules for the ID, sorted
		'''  from oldest to newest, not null </returns>
		''' <exception cref="ZoneRulesException"> if history cannot be obtained for the zone ID </exception>
		Public Shared Function getVersions(  zoneId_Renamed As String) As java.util.NavigableMap(Of String, ZoneRules)
			java.util.Objects.requireNonNull(zoneId_Renamed, "zoneId")
			Return getProvider(zoneId_Renamed).provideVersions(zoneId_Renamed)
		End Function

		''' <summary>
		''' Gets the provider for the zone ID.
		''' </summary>
		''' <param name="zoneId">  the zone ID as defined by {@code ZoneId}, not null </param>
		''' <returns> the provider, not null </returns>
		''' <exception cref="ZoneRulesException"> if the zone ID is unknown </exception>
		Private Shared Function getProvider(  zoneId_Renamed As String) As ZoneRulesProvider
			Dim provider_Renamed As ZoneRulesProvider = ZONES.get(zoneId_Renamed)
			If provider_Renamed Is Nothing Then
				If ZONES.empty Then Throw New ZoneRulesException("No time-zone data files registered")
				Throw New ZoneRulesException("Unknown time-zone ID: " & zoneId_Renamed)
			End If
			Return provider_Renamed
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Registers a zone rules provider.
		''' <p>
		''' This adds a new provider to those currently available.
		''' A provider supplies rules for one or more zone IDs.
		''' A provider cannot be registered if it supplies a zone ID that has already been
		''' registered. See the notes on time-zone IDs in <seealso cref="ZoneId"/>, especially
		''' the section on using the concept of a "group" to make IDs unique.
		''' <p>
		''' To ensure the integrity of time-zones already created, there is no way
		''' to deregister providers.
		''' </summary>
		''' <param name="provider">  the provider to register, not null </param>
		''' <exception cref="ZoneRulesException"> if a zone ID is already registered </exception>
		Public Shared Sub registerProvider(  provider As ZoneRulesProvider)
			java.util.Objects.requireNonNull(provider, "provider")
			registerProvider0(provider)
			PROVIDERS.add(provider)
		End Sub

		''' <summary>
		''' Registers the provider.
		''' </summary>
		''' <param name="provider">  the provider to register, not null </param>
		''' <exception cref="ZoneRulesException"> if unable to complete the registration </exception>
		Private Shared Sub registerProvider0(  provider As ZoneRulesProvider)
			For Each zoneId_Renamed As String In provider.provideZoneIds()
				java.util.Objects.requireNonNull(zoneId_Renamed, "zoneId")
				Dim old As ZoneRulesProvider = ZONES.putIfAbsent(zoneId_Renamed, provider)
				If old IsNot Nothing Then Throw New ZoneRulesException("Unable to register zone as one already registered with that ID: " & zoneId_Renamed & ", currently loading from provider: " & provider)
			Next zoneId_Renamed
		End Sub

		''' <summary>
		''' Refreshes the rules from the underlying data provider.
		''' <p>
		''' This method allows an application to request that the providers check
		''' for any updates to the provided rules.
		''' After calling this method, the offset stored in any <seealso cref="ZonedDateTime"/>
		''' may be invalid for the zone ID.
		''' <p>
		''' Dynamic update of rules is a complex problem and most applications
		''' should not use this method or dynamic rules.
		''' To achieve dynamic rules, a provider implementation will have to be written
		''' as per the specification of this class.
		''' In addition, instances of {@code ZoneRules} must not be cached in the
		''' application as they will become stale. However, the boolean flag on
		''' <seealso cref="#provideRules(String, boolean)"/> allows provider implementations
		''' to control the caching of {@code ZoneId}, potentially ensuring that
		''' all objects in the system see the new rules.
		''' Note that there is likely to be a cost in performance of a dynamic rules
		''' provider. Note also that no dynamic rules provider is in this specification.
		''' </summary>
		''' <returns> true if the rules were updated </returns>
		''' <exception cref="ZoneRulesException"> if an error occurs during the refresh </exception>
		Public Shared Function refresh() As Boolean
			Dim changed As Boolean = False
			For Each provider_Renamed As ZoneRulesProvider In PROVIDERS
				changed = changed Or provider_Renamed.provideRefresh()
			Next provider_Renamed
			Return changed
		End Function

		''' <summary>
		''' Constructor.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' SPI method to get the available zone IDs.
		''' <p>
		''' This obtains the IDs that this {@code ZoneRulesProvider} provides.
		''' A provider should provide data for at least one zone ID.
		''' <p>
		''' The returned zone IDs remain available and valid for the lifetime of the application.
		''' A dynamic provider may increase the set of IDs as more data becomes available.
		''' </summary>
		''' <returns> the set of zone IDs being provided, not null </returns>
		''' <exception cref="ZoneRulesException"> if a problem occurs while providing the IDs </exception>
		Protected Friend MustOverride Function provideZoneIds() As java.util.Set(Of String)

		''' <summary>
		''' SPI method to get the rules for the zone ID.
		''' <p>
		''' This loads the rules for the specified zone ID.
		''' The provider implementation must validate that the zone ID is valid and
		''' available, throwing a {@code ZoneRulesException} if it is not.
		''' The result of the method in the valid case depends on the caching flag.
		''' <p>
		''' If the provider implementation is not dynamic, then the result of the
		''' method must be the non-null set of rules selected by the ID.
		''' <p>
		''' If the provider implementation is dynamic, then the flag gives the option
		''' of preventing the returned rules from being cached in <seealso cref="ZoneId"/>.
		''' When the flag is true, the provider is permitted to return null, where
		''' null will prevent the rules from being cached in {@code ZoneId}.
		''' When the flag is false, the provider must return non-null rules.
		''' </summary>
		''' <param name="zoneId"> the zone ID as defined by {@code ZoneId}, not null </param>
		''' <param name="forCaching"> whether the rules are being queried for caching,
		''' true if the returned rules will be cached by {@code ZoneId},
		''' false if they will be returned to the user without being cached in {@code ZoneId} </param>
		''' <returns> the rules, null if {@code forCaching} is true and this
		''' is a dynamic provider that wants to prevent caching in {@code ZoneId},
		''' otherwise not null </returns>
		''' <exception cref="ZoneRulesException"> if rules cannot be obtained for the zone ID </exception>
		Protected Friend MustOverride Function provideRules(  zoneId As String,   forCaching As Boolean) As ZoneRules

		''' <summary>
		''' SPI method to get the history of rules for the zone ID.
		''' <p>
		''' This returns a map of historical rules keyed by a version string.
		''' The exact meaning and format of the version is provider specific.
		''' The version must follow lexicographical order, thus the returned map will
		''' be order from the oldest known rules to the newest available rules.
		''' The default 'TZDB' group uses version numbering consisting of the year
		''' followed by a letter, such as '2009e' or '2012f'.
		''' <p>
		''' Implementations must provide a result for each valid zone ID, however
		''' they do not have to provide a history of rules.
		''' Thus the map will contain at least one element, and will only contain
		''' more than one element if historical rule information is available.
		''' <p>
		''' The returned versions remain available and valid for the lifetime of the application.
		''' A dynamic provider may increase the set of versions as more data becomes available.
		''' </summary>
		''' <param name="zoneId">  the zone ID as defined by {@code ZoneId}, not null </param>
		''' <returns> a modifiable copy of the history of the rules for the ID, sorted
		'''  from oldest to newest, not null </returns>
		''' <exception cref="ZoneRulesException"> if history cannot be obtained for the zone ID </exception>
		Protected Friend MustOverride Function provideVersions(  zoneId As String) As java.util.NavigableMap(Of String, ZoneRules)

		''' <summary>
		''' SPI method to refresh the rules from the underlying data provider.
		''' <p>
		''' This method provides the opportunity for a provider to dynamically
		''' recheck the underlying data provider to find the latest rules.
		''' This could be used to load new rules without stopping the JVM.
		''' Dynamic behavior is entirely optional and most providers do not support it.
		''' <p>
		''' This implementation returns false.
		''' </summary>
		''' <returns> true if the rules were updated </returns>
		''' <exception cref="ZoneRulesException"> if an error occurs during the refresh </exception>
		Protected Friend Overridable Function provideRefresh() As Boolean
			Return False
		End Function

	End Class

End Namespace
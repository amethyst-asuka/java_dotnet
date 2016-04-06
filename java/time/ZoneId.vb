Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
' * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time


	''' <summary>
	''' A time-zone ID, such as {@code Europe/Paris}.
	''' <p>
	''' A {@code ZoneId} is used to identify the rules used to convert between
	''' an <seealso cref="Instant"/> and a <seealso cref="LocalDateTime"/>.
	''' There are two distinct types of ID:
	''' <ul>
	''' <li>Fixed offsets - a fully resolved offset from UTC/Greenwich, that uses
	'''  the same offset for all local date-times
	''' <li>Geographical regions - an area where a specific set of rules for finding
	'''  the offset from UTC/Greenwich apply
	''' </ul>
	''' Most fixed offsets are represented by <seealso cref="ZoneOffset"/>.
	''' Calling <seealso cref="#normalized()"/> on any {@code ZoneId} will ensure that a
	''' fixed offset ID will be represented as a {@code ZoneOffset}.
	''' <p>
	''' The actual rules, describing when and how the offset changes, are defined by <seealso cref="ZoneRules"/>.
	''' This class is simply an ID used to obtain the underlying rules.
	''' This approach is taken because rules are defined by governments and change
	''' frequently, whereas the ID is stable.
	''' <p>
	''' The distinction has other effects. Serializing the {@code ZoneId} will only send
	''' the ID, whereas serializing the rules sends the entire data set.
	''' Similarly, a comparison of two IDs only examines the ID, whereas
	''' a comparison of two rules examines the entire data set.
	''' 
	''' <h3>Time-zone IDs</h3>
	''' The ID is unique within the system.
	''' There are three types of ID.
	''' <p>
	''' The simplest type of ID is that from {@code ZoneOffset}.
	''' This consists of 'Z' and IDs starting with '+' or '-'.
	''' <p>
	''' The next type of ID are offset-style IDs with some form of prefix,
	''' such as 'GMT+2' or 'UTC+01:00'.
	''' The recognised prefixes are 'UTC', 'GMT' and 'UT'.
	''' The offset is the suffix and will be normalized during creation.
	''' These IDs can be normalized to a {@code ZoneOffset} using {@code normalized()}.
	''' <p>
	''' The third type of ID are region-based IDs. A region-based ID must be of
	''' two or more characters, and not start with 'UTC', 'GMT', 'UT' '+' or '-'.
	''' Region-based IDs are defined by configuration, see <seealso cref="ZoneRulesProvider"/>.
	''' The configuration focuses on providing the lookup from the ID to the
	''' underlying {@code ZoneRules}.
	''' <p>
	''' Time-zone rules are defined by governments and change frequently.
	''' There are a number of organizations, known here as groups, that monitor
	''' time-zone changes and collate them.
	''' The default group is the IANA Time Zone Database (TZDB).
	''' Other organizations include IATA (the airline industry body) and Microsoft.
	''' <p>
	''' Each group defines its own format for the region ID it provides.
	''' The TZDB group defines IDs such as 'Europe/London' or 'America/New_York'.
	''' TZDB IDs take precedence over other groups.
	''' <p>
	''' It is strongly recommended that the group name is included in all IDs supplied by
	''' groups other than TZDB to avoid conflicts. For example, IATA airline time-zone
	''' region IDs are typically the same as the three letter airport code.
	''' However, the airport of Utrecht has the code 'UTC', which is obviously a conflict.
	''' The recommended format for region IDs from groups other than TZDB is 'group~region'.
	''' Thus if IATA data were defined, Utrecht airport would be 'IATA~UTC'.
	''' 
	''' <h3>Serialization</h3>
	''' This class can be serialized and stores the string zone ID in the external form.
	''' The {@code ZoneOffset} subclass uses a dedicated format that only stores the
	''' offset from UTC/Greenwich.
	''' <p>
	''' A {@code ZoneId} can be deserialized in a Java Runtime where the ID is unknown.
	''' For example, if a server-side Java Runtime has been updated with a new zone ID, but
	''' the client-side Java Runtime has not been updated. In this case, the {@code ZoneId}
	''' object will exist, and can be queried using {@code getId}, {@code equals},
	''' {@code hashCode}, {@code toString}, {@code getDisplayName} and {@code normalized}.
	''' However, any call to {@code getRules} will fail with {@code ZoneRulesException}.
	''' This approach is designed to allow a <seealso cref="ZonedDateTime"/> to be loaded and
	''' queried, but not modified, on a Java Runtime with incomplete time-zone information.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code ZoneId} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This abstract class has two implementations, both of which are immutable and thread-safe.
	''' One implementation models region-based IDs, the other is {@code ZoneOffset} modelling
	''' offset-based IDs. This difference is visible in serialization.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public MustInherit Class ZoneId

		''' <summary>
		''' A map of zone overrides to enable the short time-zone names to be used.
		''' <p>
		''' Use of short zone IDs has been deprecated in {@code java.util.TimeZone}.
		''' This map allows the IDs to continue to be used via the
		''' <seealso cref="#of(String, Map)"/> factory method.
		''' <p>
		''' This map contains a mapping of the IDs that is in line with TZDB 2005r and
		''' later, where 'EST', 'MST' and 'HST' map to IDs which do not include daylight
		''' savings.
		''' <p>
		''' This maps as follows:
		''' <ul>
		''' <li>EST - -05:00</li>
		''' <li>HST - -10:00</li>
		''' <li>MST - -07:00</li>
		''' <li>ACT - Australia/Darwin</li>
		''' <li>AET - Australia/Sydney</li>
		''' <li>AGT - America/Argentina/Buenos_Aires</li>
		''' <li>ART - Africa/Cairo</li>
		''' <li>AST - America/Anchorage</li>
		''' <li>BET - America/Sao_Paulo</li>
		''' <li>BST - Asia/Dhaka</li>
		''' <li>CAT - Africa/Harare</li>
		''' <li>CNT - America/St_Johns</li>
		''' <li>CST - America/Chicago</li>
		''' <li>CTT - Asia/Shanghai</li>
		''' <li>EAT - Africa/Addis_Ababa</li>
		''' <li>ECT - Europe/Paris</li>
		''' <li>IET - America/Indiana/Indianapolis</li>
		''' <li>IST - Asia/Kolkata</li>
		''' <li>JST - Asia/Tokyo</li>
		''' <li>MIT - Pacific/Apia</li>
		''' <li>NET - Asia/Yerevan</li>
		''' <li>NST - Pacific/Auckland</li>
		''' <li>PLT - Asia/Karachi</li>
		''' <li>PNT - America/Phoenix</li>
		''' <li>PRT - America/Puerto_Rico</li>
		''' <li>PST - America/Los_Angeles</li>
		''' <li>SST - Pacific/Guadalcanal</li>
		''' <li>VST - Asia/Ho_Chi_Minh</li>
		''' </ul>
		''' The map is unmodifiable.
		''' </summary>
		Public Shared ReadOnly SHORT_IDS As IDictionary(Of String, String)
		Shared Sub New()
			Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)(64)
			map("ACT") = "Australia/Darwin"
			map("AET") = "Australia/Sydney"
			map("AGT") = "America/Argentina/Buenos_Aires"
			map("ART") = "Africa/Cairo"
			map("AST") = "America/Anchorage"
			map("BET") = "America/Sao_Paulo"
			map("BST") = "Asia/Dhaka"
			map("CAT") = "Africa/Harare"
			map("CNT") = "America/St_Johns"
			map("CST") = "America/Chicago"
			map("CTT") = "Asia/Shanghai"
			map("EAT") = "Africa/Addis_Ababa"
			map("ECT") = "Europe/Paris"
			map("IET") = "America/Indiana/Indianapolis"
			map("IST") = "Asia/Kolkata"
			map("JST") = "Asia/Tokyo"
			map("MIT") = "Pacific/Apia"
			map("NET") = "Asia/Yerevan"
			map("NST") = "Pacific/Auckland"
			map("PLT") = "Asia/Karachi"
			map("PNT") = "America/Phoenix"
			map("PRT") = "America/Puerto_Rico"
			map("PST") = "America/Los_Angeles"
			map("SST") = "Pacific/Guadalcanal"
			map("VST") = "Asia/Ho_Chi_Minh"
			map("EST") = "-05:00"
			map("MST") = "-07:00"
			map("HST") = "-10:00"
			SHORT_IDS = java.util.Collections.unmodifiableMap(map)
		End Sub
		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 8352817235686L

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the system default time-zone.
		''' <p>
		''' This queries <seealso cref="TimeZone#getDefault()"/> to find the default time-zone
		''' and converts it to a {@code ZoneId}. If the system default time-zone is changed,
		''' then the result of this method will also change.
		''' </summary>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if the converted zone ID has an invalid format </exception>
		''' <exception cref="ZoneRulesException"> if the converted zone region ID cannot be found </exception>
		Public Shared Function systemDefault() As ZoneId
			Return java.util.TimeZone.default.toZoneId()
		End Function

		''' <summary>
		''' Gets the set of available zone IDs.
		''' <p>
		''' This set includes the string form of all available region-based IDs.
		''' Offset-based zone IDs are not included in the returned set.
		''' The ID can be passed to <seealso cref="#of(String)"/> to create a {@code ZoneId}.
		''' <p>
		''' The set of zone IDs can increase over time, although in a typical application
		''' the set of IDs is fixed. Each call to this method is thread-safe.
		''' </summary>
		''' <returns> a modifiable copy of the set of zone IDs, not null </returns>
		PublicShared ReadOnly PropertyavailableZoneIds As java.util.Set(Of String)
			Get
				Return java.time.zone.ZoneRulesProvider.availableZoneIds
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZoneId} using its ID using a map
		''' of aliases to supplement the standard zone IDs.
		''' <p>
		''' Many users of time-zones use short abbreviations, such as PST for
		''' 'Pacific Standard Time' and PDT for 'Pacific Daylight Time'.
		''' These abbreviations are not unique, and so cannot be used as IDs.
		''' This method allows a map of string to time-zone to be setup and reused
		''' within an application.
		''' </summary>
		''' <param name="zoneId">  the time-zone ID, not null </param>
		''' <param name="aliasMap">  a map of alias zone IDs (typically abbreviations) to real zone IDs, not null </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if the zone ID has an invalid format </exception>
		''' <exception cref="ZoneRulesException"> if the zone ID is a region ID that cannot be found </exception>
		Public Shared Function [of](  zoneId_Renamed As String,   aliasMap As IDictionary(Of String, String)) As ZoneId
			java.util.Objects.requireNonNull(zoneId_Renamed, "zoneId")
			java.util.Objects.requireNonNull(aliasMap, "aliasMap")
			Dim id_Renamed As String = aliasMap(zoneId_Renamed)
			id_Renamed = (If(id_Renamed IsNot Nothing, id_Renamed, zoneId_Renamed))
			Return [of](id_Renamed)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZoneId} from an ID ensuring that the
		''' ID is valid and available for use.
		''' <p>
		''' This method parses the ID producing a {@code ZoneId} or {@code ZoneOffset}.
		''' A {@code ZoneOffset} is returned if the ID is 'Z', or starts with '+' or '-'.
		''' The result will always be a valid ID for which <seealso cref="ZoneRules"/> can be obtained.
		''' <p>
		''' Parsing matches the zone ID step by step as follows.
		''' <ul>
		''' <li>If the zone ID equals 'Z', the result is {@code ZoneOffset.UTC}.
		''' <li>If the zone ID consists of a single letter, the zone ID is invalid
		'''  and {@code DateTimeException} is thrown.
		''' <li>If the zone ID starts with '+' or '-', the ID is parsed as a
		'''  {@code ZoneOffset} using <seealso cref="ZoneOffset#of(String)"/>.
		''' <li>If the zone ID equals 'GMT', 'UTC' or 'UT' then the result is a {@code ZoneId}
		'''  with the same ID and rules equivalent to {@code ZoneOffset.UTC}.
		''' <li>If the zone ID starts with 'UTC+', 'UTC-', 'GMT+', 'GMT-', 'UT+' or 'UT-'
		'''  then the ID is a prefixed offset-based ID. The ID is split in two, with
		'''  a two or three letter prefix and a suffix starting with the sign.
		'''  The suffix is parsed as a <seealso cref="ZoneOffset#of(String) ZoneOffset"/>.
		'''  The result will be a {@code ZoneId} with the specified UTC/GMT/UT prefix
		'''  and the normalized offset ID as per <seealso cref="ZoneOffset#getId()"/>.
		'''  The rules of the returned {@code ZoneId} will be equivalent to the
		'''  parsed {@code ZoneOffset}.
		''' <li>All other IDs are parsed as region-based zone IDs. Region IDs must
		'''  match the regular expression <code>[A-Za-z][A-Za-z0-9~/._+-]+</code>
		'''  otherwise a {@code DateTimeException} is thrown. If the zone ID is not
		'''  in the configured set of IDs, {@code ZoneRulesException} is thrown.
		'''  The detailed format of the region ID depends on the group supplying the data.
		'''  The default set of data is supplied by the IANA Time Zone Database (TZDB).
		'''  This has region IDs of the form '{area}/{city}', such as 'Europe/Paris' or 'America/New_York'.
		'''  This is compatible with most IDs from <seealso cref="java.util.TimeZone"/>.
		''' </ul>
		''' </summary>
		''' <param name="zoneId">  the time-zone ID, not null </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if the zone ID has an invalid format </exception>
		''' <exception cref="ZoneRulesException"> if the zone ID is a region ID that cannot be found </exception>
		Public Shared Function [of](  zoneId_Renamed As String) As ZoneId
			Return [of](zoneId_Renamed, True)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZoneId} wrapping an offset.
		''' <p>
		''' If the prefix is "GMT", "UTC", or "UT" a {@code ZoneId}
		''' with the prefix and the non-zero offset is returned.
		''' If the prefix is empty {@code ""} the {@code ZoneOffset} is returned.
		''' </summary>
		''' <param name="prefix">  the time-zone ID, not null </param>
		''' <param name="offset">  the offset, not null </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the prefix is not one of
		'''     "GMT", "UTC", or "UT", or "" </exception>
		Public Shared Function ofOffset(  prefix As String,   offset As ZoneOffset) As ZoneId
			java.util.Objects.requireNonNull(prefix, "prefix")
			java.util.Objects.requireNonNull(offset, "offset")
			If prefix.length() = 0 Then Return offset

			If (Not prefix.Equals("GMT")) AndAlso (Not prefix.Equals("UTC")) AndAlso (Not prefix.Equals("UT")) Then Throw New IllegalArgumentException("prefix should be GMT, UTC or UT, is: " & prefix)

			If offset.totalSeconds <> 0 Then prefix = prefix.concat(offset.id)
			Return New ZoneRegion(prefix, offset.rules)
		End Function

		''' <summary>
		''' Parses the ID, taking a flag to indicate whether {@code ZoneRulesException}
		''' should be thrown or not, used in deserialization.
		''' </summary>
		''' <param name="zoneId">  the time-zone ID, not null </param>
		''' <param name="checkAvailable">  whether to check if the zone ID is available </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if the ID format is invalid </exception>
		''' <exception cref="ZoneRulesException"> if checking availability and the ID cannot be found </exception>
		Shared Function [of](  zoneId_Renamed As String,   checkAvailable As Boolean) As ZoneId
			java.util.Objects.requireNonNull(zoneId_Renamed, "zoneId")
			If zoneId_Renamed.length() <= 1 OrElse zoneId_Renamed.StartsWith("+") OrElse zoneId_Renamed.StartsWith("-") Then
				Return ZoneOffset.of(zoneId_Renamed)
			ElseIf zoneId_Renamed.StartsWith("UTC") OrElse zoneId_Renamed.StartsWith("GMT") Then
				Return ofWithPrefix(zoneId_Renamed, 3, checkAvailable)
			ElseIf zoneId_Renamed.StartsWith("UT") Then
				Return ofWithPrefix(zoneId_Renamed, 2, checkAvailable)
			End If
			Return ZoneRegion.ofId(zoneId_Renamed, checkAvailable)
		End Function

		''' <summary>
		''' Parse once a prefix is established.
		''' </summary>
		''' <param name="zoneId">  the time-zone ID, not null </param>
		''' <param name="prefixLength">  the length of the prefix, 2 or 3 </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if the zone ID has an invalid format </exception>
		Private Shared Function ofWithPrefix(  zoneId_Renamed As String,   prefixLength As Integer,   checkAvailable As Boolean) As ZoneId
			Dim prefix As String = zoneId_Renamed.Substring(0, prefixLength)
			If zoneId_Renamed.length() = prefixLength Then Return ofOffset(prefix, ZoneOffset.UTC)
			If zoneId_Renamed.Chars(prefixLength) <> "+"c AndAlso zoneId_Renamed.Chars(prefixLength) <> "-"c Then Return ZoneRegion.ofId(zoneId_Renamed, checkAvailable) ' drop through to ZoneRulesProvider
			Try
				Dim offset As ZoneOffset = ZoneOffset.of(zoneId_Renamed.Substring(prefixLength))
				If offset Is ZoneOffset.UTC Then Return ofOffset(prefix, offset)
				Return ofOffset(prefix, offset)
			Catch ex As DateTimeException
				Throw New DateTimeException("Invalid ID for offset-based ZoneId: " & zoneId_Renamed, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZoneId} from a temporal object.
		''' <p>
		''' This obtains a zone based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ZoneId}.
		''' <p>
		''' A {@code TemporalAccessor} represents some form of date and time information.
		''' This factory converts the arbitrary temporal object to an instance of {@code ZoneId}.
		''' <p>
		''' The conversion will try to obtain the zone in a way that favours region-based
		''' zones over offset-based zones using <seealso cref="TemporalQueries#zone()"/>.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code ZoneId::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code ZoneId} </exception>
		Public Shared Function [from](  temporal As java.time.temporal.TemporalAccessor) As ZoneId
			Dim obj As ZoneId = temporal.query(java.time.temporal.TemporalQueries.zone())
			If obj Is Nothing Then Throw New DateTimeException("Unable to obtain ZoneId from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name)
			Return obj
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor only accessible within the package.
		''' </summary>
		Friend Sub New()
			If Me.GetType() <> GetType(ZoneOffset) AndAlso Me.GetType() <> GetType(ZoneRegion) Then Throw New AssertionError("Invalid subclass")
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the unique time-zone ID.
		''' <p>
		''' This ID uniquely defines this object.
		''' The format of an offset based ID is defined by <seealso cref="ZoneOffset#getId()"/>.
		''' </summary>
		''' <returns> the time-zone unique ID, not null </returns>
		Public MustOverride ReadOnly Property id As String

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the textual representation of the zone, such as 'British Time' or
		''' '+02:00'.
		''' <p>
		''' This returns the textual name used to identify the time-zone ID,
		''' suitable for presentation to the user.
		''' The parameters control the style of the returned text and the locale.
		''' <p>
		''' If no textual mapping is found then the <seealso cref="#getId() full ID"/> is returned.
		''' </summary>
		''' <param name="style">  the length of the text required, not null </param>
		''' <param name="locale">  the locale to use, not null </param>
		''' <returns> the text value of the zone, not null </returns>
		Public Overridable Function getDisplayName(  style As java.time.format.TextStyle,   locale As java.util.Locale) As String
			Return (New java.time.format.DateTimeFormatterBuilder).appendZoneText(style).toFormatter(locale).format(toTemporal())
		End Function

		''' <summary>
		''' Converts this zone to a {@code TemporalAccessor}.
		''' <p>
		''' A {@code ZoneId} can be fully represented as a {@code TemporalAccessor}.
		''' However, the interface is not implemented by this class as most of the
		''' methods on the interface have no meaning to {@code ZoneId}.
		''' <p>
		''' The returned temporal has no supported fields, with the query method
		''' supporting the return of the zone using <seealso cref="TemporalQueries#zoneId()"/>.
		''' </summary>
		''' <returns> a temporal equivalent to this zone, not null </returns>
		Private Function toTemporal() As java.time.temporal.TemporalAccessor
			Return New TemporalAccessorAnonymousInnerClassHelper
		End Function

		Private Class TemporalAccessorAnonymousInnerClassHelper
			Implements java.time.temporal.TemporalAccessor

			Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean
				Return False
			End Function
			Public Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function query(Of R)(  query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R
				If query_Renamed Is java.time.temporal.TemporalQueries.zoneId() Then Return CType(ZoneId.this, R)
				Return outerInstance.query(query_Renamed)
			End Function
		End Class

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the time-zone rules for this ID allowing calculations to be performed.
		''' <p>
		''' The rules provide the functionality associated with a time-zone,
		''' such as finding the offset for a given instant or local date-time.
		''' <p>
		''' A time-zone can be invalid if it is deserialized in a Java Runtime which
		''' does not have the same rules loaded as the Java Runtime that stored it.
		''' In this case, calling this method will throw a {@code ZoneRulesException}.
		''' <p>
		''' The rules are supplied by <seealso cref="ZoneRulesProvider"/>. An advanced provider may
		''' support dynamic updates to the rules without restarting the Java Runtime.
		''' If so, then the result of this method may change over time.
		''' Each individual call will be still remain thread-safe.
		''' <p>
		''' <seealso cref="ZoneOffset"/> will always return a set of rules where the offset never changes.
		''' </summary>
		''' <returns> the rules, not null </returns>
		''' <exception cref="ZoneRulesException"> if no rules are available for this ID </exception>
		Public MustOverride ReadOnly Property rules As java.time.zone.ZoneRules

		''' <summary>
		''' Normalizes the time-zone ID, returning a {@code ZoneOffset} where possible.
		''' <p>
		''' The returns a normalized {@code ZoneId} that can be used in place of this ID.
		''' The result will have {@code ZoneRules} equivalent to those returned by this object,
		''' however the ID returned by {@code getId()} may be different.
		''' <p>
		''' The normalization checks if the rules of this {@code ZoneId} have a fixed offset.
		''' If they do, then the {@code ZoneOffset} equal to that offset is returned.
		''' Otherwise {@code this} is returned.
		''' </summary>
		''' <returns> the time-zone unique ID, not null </returns>
		Public Overridable Function normalized() As ZoneId
			Try
				Dim rules_Renamed As java.time.zone.ZoneRules = rules
				If rules_Renamed.fixedOffset Then Return rules_Renamed.getOffset(Instant.EPOCH)
			Catch ex As java.time.zone.ZoneRulesException
				' invalid ZoneRegion is not important to this method
			End Try
			Return Me
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this time-zone ID is equal to another time-zone ID.
		''' <p>
		''' The comparison is based on the ID.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other time-zone ID </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ZoneId Then
				Dim other As ZoneId = CType(obj, ZoneId)
				Return id.Equals(other.id)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this time-zone ID.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return id.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		''' <summary>
		''' Outputs this zone as a {@code String}, using the ID.
		''' </summary>
		''' <returns> a string representation of this time-zone ID, not null </returns>
		Public Overrides Function ToString() As String
			Return id
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(7);  // identifies a ZoneId (not ZoneOffset)
		'''  out.writeUTF(getId());
		''' </pre>
		''' <p>
		''' When read back in, the {@code ZoneId} will be created as though using
		''' <seealso cref="#of(String)"/>, but without any exception in the case where the
		''' ID has a valid format, but is not in the known set of region-based IDs.
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		' this is here for serialization Javadoc
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZONE_REGION_TYPE, Me)
		End Function

		Friend MustOverride Sub write(  out As java.io.DataOutput)

	End Class

End Namespace
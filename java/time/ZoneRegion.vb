Imports System

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
	''' A geographical region where the same time-zone rules apply.
	''' <p>
	''' Time-zone information is categorized as a set of rules defining when and
	''' how the offset from UTC/Greenwich changes. These rules are accessed using
	''' identifiers based on geographical regions, such as countries or states.
	''' The most common region classification is the Time Zone Database (TZDB),
	''' which defines regions such as 'Europe/Paris' and 'Asia/Tokyo'.
	''' <p>
	''' The region identifier, modeled by this [Class], is distinct from the
	''' underlying rules, modeled by <seealso cref="ZoneRules"/>.
	''' The rules are defined by governments and change frequently.
	''' By contrast, the region identifier is well-defined and long-lived.
	''' This separation also allows rules to be shared between regions if appropriate.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Friend NotInheritable Class ZoneRegion
		Inherits ZoneId

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 8386373296231747096L
		''' <summary>
		''' The time-zone ID, not null.
		''' </summary>
		Private ReadOnly id As String
		''' <summary>
		''' The time-zone rules, null if zone ID was loaded leniently.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly rules As java.time.zone.ZoneRules

		''' <summary>
		''' Obtains an instance of {@code ZoneId} from an identifier.
		''' </summary>
		''' <param name="zoneId">  the time-zone ID, not null </param>
		''' <param name="checkAvailable">  whether to check if the zone ID is available </param>
		''' <returns> the zone ID, not null </returns>
		''' <exception cref="DateTimeException"> if the ID format is invalid </exception>
		''' <exception cref="ZoneRulesException"> if checking availability and the ID cannot be found </exception>
		Shared Function ofId(ByVal zoneId_Renamed As String, ByVal checkAvailable As Boolean) As ZoneRegion
			java.util.Objects.requireNonNull(zoneId_Renamed, "zoneId")
			checkName(zoneId_Renamed)
			Dim rules_Renamed As java.time.zone.ZoneRules = Nothing
			Try
				' always attempt load for better behavior after deserialization
				rules_Renamed = java.time.zone.ZoneRulesProvider.getRules(zoneId_Renamed, True)
			Catch ex As java.time.zone.ZoneRulesException
				If checkAvailable Then Throw ex
			End Try
			Return New ZoneRegion(zoneId_Renamed, rules_Renamed)
		End Function

		''' <summary>
		''' Checks that the given string is a legal ZondId name.
		''' </summary>
		''' <param name="zoneId">  the time-zone ID, not null </param>
		''' <exception cref="DateTimeException"> if the ID format is invalid </exception>
		Private Shared Sub checkName(ByVal zoneId_Renamed As String)
			Dim n As Integer = zoneId_Renamed.length()
			If n < 2 Then Throw New DateTimeException("Invalid ID for region-based ZoneId, invalid format: " & zoneId_Renamed)
			For i As Integer = 0 To n - 1
				Dim c As Char = zoneId_Renamed.Chars(i)
				If c >= "a"c AndAlso c <= "z"c Then Continue For
				If c >= "A"c AndAlso c <= "Z"c Then Continue For
				If c = "/"c AndAlso i <> 0 Then Continue For
				If c >= "0"c AndAlso c <= "9"c AndAlso i <> 0 Then Continue For
				If c = "~"c AndAlso i <> 0 Then Continue For
				If c = "."c AndAlso i <> 0 Then Continue For
				If c = "_"c AndAlso i <> 0 Then Continue For
				If c = "+"c AndAlso i <> 0 Then Continue For
				If c = "-"c AndAlso i <> 0 Then Continue For
				Throw New DateTimeException("Invalid ID for region-based ZoneId, invalid format: " & zoneId_Renamed)
			Next i
		End Sub

		'-------------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="id">  the time-zone ID, not null </param>
		''' <param name="rules">  the rules, null for lazy lookup </param>
		Friend Sub New(ByVal id As String, ByVal rules As java.time.zone.ZoneRules)
			Me.id = id
			Me.rules = rules
		End Sub

		'-----------------------------------------------------------------------
		Public Property Overrides id As String
			Get
				Return id
			End Get
		End Property

		Public Property Overrides rules As java.time.zone.ZoneRules
			Get
				' additional query for group provider when null allows for possibility
				' that the provider was updated after the ZoneId was created
				Return (If(rules IsNot Nothing, rules, java.time.zone.ZoneRulesProvider.getRules(id, False)))
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(7);  // identifies a ZoneId (not ZoneOffset)
		'''  out.writeUTF(zoneId);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZONE_REGION_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Overrides Sub write(ByVal out As java.io.DataOutput)
			out.writeByte(Ser.ZONE_REGION_TYPE)
			writeExternal(out)
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			out.writeUTF(id)
		End Sub

		Friend Shared Function readExternal(ByVal [in] As java.io.DataInput) As ZoneId
			Dim id_Renamed As String = [in].readUTF()
			Return ZoneId.of(id_Renamed, False)
		End Function

	End Class

End Namespace
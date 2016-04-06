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
	''' Loads time-zone rules for 'TZDB'.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class TzdbZoneRulesProvider
		Inherits ZoneRulesProvider

		''' <summary>
		''' All the regions that are available.
		''' </summary>
		Private regionIds As IList(Of String)
		''' <summary>
		''' Version Id of this tzdb rules
		''' </summary>
		Private versionId As String
		''' <summary>
		''' Region to rules mapping
		''' </summary>
		Private ReadOnly regionToRules As IDictionary(Of String, Object) = New ConcurrentDictionary(Of String, Object)

		''' <summary>
		''' Creates an instance.
		''' Created by the {@code ServiceLoader}.
		''' </summary>
		''' <exception cref="ZoneRulesException"> if unable to load </exception>
		Public Sub New()
			Try
				Dim libDir As String = System.getProperty("java.home") + File.separator & "lib"
				Using dis As New java.io.DataInputStream(New java.io.BufferedInputStream(New java.io.FileInputStream(New File(libDir, "tzdb.dat"))))
					load(dis)
				End Using
			Catch ex As Exception
				Throw New ZoneRulesException("Unable to load TZDB time-zone rules", ex)
			End Try
		End Sub

		Protected Friend Overrides Function provideZoneIds() As java.util.Set(Of String)
			Return New HashSet(Of )(regionIds)
		End Function

		Protected Friend Overrides Function provideRules(  zoneId_Renamed As String,   forCaching As Boolean) As ZoneRules
			' forCaching flag is ignored because this is not a dynamic provider
			Dim obj As Object = regionToRules(zoneId_Renamed)
			If obj Is Nothing Then Throw New ZoneRulesException("Unknown time-zone ID: " & zoneId_Renamed)
			Try
				If TypeOf obj Is SByte() Then
					Dim bytes As SByte() = CType(obj, SByte())
					Dim dis As New java.io.DataInputStream(New java.io.ByteArrayInputStream(bytes))
					obj = Ser.read(dis)
					regionToRules(zoneId_Renamed) = obj
				End If
				Return CType(obj, ZoneRules)
			Catch ex As Exception
				Throw New ZoneRulesException("Invalid binary time-zone data: TZDB:" & zoneId_Renamed & ", version: " & versionId, ex)
			End Try
		End Function

		Protected Friend Overrides Function provideVersions(  zoneId_Renamed As String) As java.util.NavigableMap(Of String, ZoneRules)
			Dim map As New SortedDictionary(Of String, ZoneRules)
			Dim rules_Renamed As ZoneRules = getRules(zoneId_Renamed, False)
			If rules_Renamed IsNot Nothing Then map(versionId) = rules_Renamed
			Return map
		End Function

		''' <summary>
		''' Loads the rules from a DateInputStream, often in a jar file.
		''' </summary>
		''' <param name="dis">  the DateInputStream to load, not null </param>
		''' <exception cref="Exception"> if an error occurs </exception>
		Private Sub load(  dis As java.io.DataInputStream)
			If dis.readByte() <> 1 Then Throw New java.io.StreamCorruptedException("File format not recognised")
			' group
			Dim groupId As String = dis.readUTF()
			If "TZDB".Equals(groupId) = False Then Throw New java.io.StreamCorruptedException("File format not recognised")
			' versions
			Dim versionCount As Integer = dis.readShort()
			For i As Integer = 0 To versionCount - 1
				versionId = dis.readUTF()
			Next i
			' regions
			Dim regionCount As Integer = dis.readShort()
			Dim regionArray As String() = New String(regionCount - 1){}
			For i As Integer = 0 To regionCount - 1
				regionArray(i) = dis.readUTF()
			Next i
			regionIds = java.util.Arrays.asList(regionArray)
			' rules
			Dim ruleCount As Integer = dis.readShort()
			Dim ruleArray As Object() = New Object(ruleCount - 1){}
			For i As Integer = 0 To ruleCount - 1
				Dim bytes As SByte() = New SByte(dis.readShort() - 1){}
				dis.readFully(bytes)
				ruleArray(i) = bytes
			Next i
			' link version-region-rules
			For i As Integer = 0 To versionCount - 1
				Dim versionRegionCount As Integer = dis.readShort()
				regionToRules.Clear()
				For j As Integer = 0 To versionRegionCount - 1
					Dim region As String = regionArray(dis.readShort())
					Dim rule As Object = ruleArray(dis.readShort() And &Hffff)
					regionToRules(region) = rule
				Next j
			Next i
		End Sub

		Public Overrides Function ToString() As String
			Return "TZDB[" & versionId & "]"
		End Function
	End Class

End Namespace
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
	''' The rules defining how the zone offset varies for a single time-zone.
	''' <p>
	''' The rules model all the historic and future transitions for a time-zone.
	''' <seealso cref="ZoneOffsetTransition"/> is used for known transitions, typically historic.
	''' <seealso cref="ZoneOffsetTransitionRule"/> is used for future transitions that are based
	''' on the result of an algorithm.
	''' <p>
	''' The rules are loaded via <seealso cref="ZoneRulesProvider"/> using a <seealso cref="ZoneId"/>.
	''' The same rules may be shared internally between multiple zone IDs.
	''' <p>
	''' Serializing an instance of {@code ZoneRules} will store the entire set of rules.
	''' It does not store the zone ID as it is not part of the state of this object.
	''' <p>
	''' A rule implementation may or may not store full information about historic
	''' and future transitions, and the information stored is only as accurate as
	''' that supplied to the implementation by the rules provider.
	''' Applications should treat the data provided as representing the best information
	''' available to the implementation of this rule.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ZoneRules

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 3044319355680032515L
		''' <summary>
		''' The last year to have its transitions cached.
		''' </summary>
		Private Const LAST_CACHED_YEAR As Integer = 2100

		''' <summary>
		''' The transitions between standard offsets (epoch seconds), sorted.
		''' </summary>
		Private ReadOnly standardTransitions As Long()
		''' <summary>
		''' The standard offsets.
		''' </summary>
		Private ReadOnly standardOffsets As java.time.ZoneOffset()
		''' <summary>
		''' The transitions between instants (epoch seconds), sorted.
		''' </summary>
		Private ReadOnly savingsInstantTransitions As Long()
		''' <summary>
		''' The transitions between local date-times, sorted.
		''' This is a paired array, where the first entry is the start of the transition
		''' and the second entry is the end of the transition.
		''' </summary>
		Private ReadOnly savingsLocalTransitions As java.time.LocalDateTime()
		''' <summary>
		''' The wall offsets.
		''' </summary>
		Private ReadOnly wallOffsets As java.time.ZoneOffset()
		''' <summary>
		''' The last rule.
		''' </summary>
		Private ReadOnly lastRules As ZoneOffsetTransitionRule()
		''' <summary>
		''' The map of recent transitions.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly lastRulesCache As java.util.concurrent.ConcurrentMap(Of Integer?, ZoneOffsetTransition()) = New ConcurrentDictionary(Of Integer?, ZoneOffsetTransition())
		''' <summary>
		''' The zero-length long array.
		''' </summary>
		Private Shared ReadOnly EMPTY_LONG_ARRAY As Long() = New Long(){}
		''' <summary>
		''' The zero-length lastrules array.
		''' </summary>
		Private Shared ReadOnly EMPTY_LASTRULES As ZoneOffsetTransitionRule() = New ZoneOffsetTransitionRule(){}
		''' <summary>
		''' The zero-length ldt array.
		''' </summary>
		Private Shared ReadOnly EMPTY_LDT_ARRAY As java.time.LocalDateTime() = New java.time.LocalDateTime(){}

		''' <summary>
		''' Obtains an instance of a ZoneRules.
		''' </summary>
		''' <param name="baseStandardOffset">  the standard offset to use before legal rules were set, not null </param>
		''' <param name="baseWallOffset">  the wall offset to use before legal rules were set, not null </param>
		''' <param name="standardOffsetTransitionList">  the list of changes to the standard offset, not null </param>
		''' <param name="transitionList">  the list of transitions, not null </param>
		''' <param name="lastRules">  the recurring last rules, size 16 or less, not null </param>
		''' <returns> the zone rules, not null </returns>
		Public Shared Function [of](ByVal baseStandardOffset As java.time.ZoneOffset, ByVal baseWallOffset As java.time.ZoneOffset, ByVal standardOffsetTransitionList As IList(Of ZoneOffsetTransition), ByVal transitionList As IList(Of ZoneOffsetTransition), ByVal lastRules As IList(Of ZoneOffsetTransitionRule)) As ZoneRules
			java.util.Objects.requireNonNull(baseStandardOffset, "baseStandardOffset")
			java.util.Objects.requireNonNull(baseWallOffset, "baseWallOffset")
			java.util.Objects.requireNonNull(standardOffsetTransitionList, "standardOffsetTransitionList")
			java.util.Objects.requireNonNull(transitionList, "transitionList")
			java.util.Objects.requireNonNull(lastRules, "lastRules")
			Return New ZoneRules(baseStandardOffset, baseWallOffset, standardOffsetTransitionList, transitionList, lastRules)
		End Function

		''' <summary>
		''' Obtains an instance of ZoneRules that has fixed zone rules.
		''' </summary>
		''' <param name="offset">  the offset this fixed zone rules is based on, not null </param>
		''' <returns> the zone rules, not null </returns>
		''' <seealso cref= #isFixedOffset() </seealso>
		Public Shared Function [of](ByVal offset As java.time.ZoneOffset) As ZoneRules
			java.util.Objects.requireNonNull(offset, "offset")
			Return New ZoneRules(offset)
		End Function

		''' <summary>
		''' Creates an instance.
		''' </summary>
		''' <param name="baseStandardOffset">  the standard offset to use before legal rules were set, not null </param>
		''' <param name="baseWallOffset">  the wall offset to use before legal rules were set, not null </param>
		''' <param name="standardOffsetTransitionList">  the list of changes to the standard offset, not null </param>
		''' <param name="transitionList">  the list of transitions, not null </param>
		''' <param name="lastRules">  the recurring last rules, size 16 or less, not null </param>
		Friend Sub New(ByVal baseStandardOffset As java.time.ZoneOffset, ByVal baseWallOffset As java.time.ZoneOffset, ByVal standardOffsetTransitionList As IList(Of ZoneOffsetTransition), ByVal transitionList As IList(Of ZoneOffsetTransition), ByVal lastRules As IList(Of ZoneOffsetTransitionRule))
			MyBase.New()

			' convert standard transitions

			Me.standardTransitions = New Long(standardOffsetTransitionList.Count - 1){}

			Me.standardOffsets = New java.time.ZoneOffset(standardOffsetTransitionList.Count){}
			Me.standardOffsets(0) = baseStandardOffset
			For i As Integer = 0 To standardOffsetTransitionList.Count - 1
				Me.standardTransitions(i) = standardOffsetTransitionList(i).toEpochSecond()
				Me.standardOffsets(i + 1) = standardOffsetTransitionList(i).offsetAfter
			Next i

			' convert savings transitions to locals
			Dim localTransitionList As IList(Of java.time.LocalDateTime) = New List(Of java.time.LocalDateTime)
			Dim localTransitionOffsetList As IList(Of java.time.ZoneOffset) = New List(Of java.time.ZoneOffset)
			localTransitionOffsetList.Add(baseWallOffset)
			For Each trans As ZoneOffsetTransition In transitionList
				If trans.gap Then
					localTransitionList.Add(trans.dateTimeBefore)
					localTransitionList.Add(trans.dateTimeAfter)
				Else
					localTransitionList.Add(trans.dateTimeAfter)
					localTransitionList.Add(trans.dateTimeBefore)
				End If
				localTransitionOffsetList.Add(trans.offsetAfter)
			Next trans
			Me.savingsLocalTransitions = localTransitionList.ToArray()
			Me.wallOffsets = localTransitionOffsetList.ToArray()

			' convert savings transitions to instants
			Me.savingsInstantTransitions = New Long(transitionList.Count - 1){}
			For i As Integer = 0 To transitionList.Count - 1
				Me.savingsInstantTransitions(i) = transitionList(i).toEpochSecond()
			Next i

			' last rules
			If lastRules.Count > 16 Then Throw New IllegalArgumentException("Too many transition rules")
			Me.lastRules = lastRules.ToArray()
		End Sub

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="standardTransitions">  the standard transitions, not null </param>
		''' <param name="standardOffsets">  the standard offsets, not null </param>
		''' <param name="savingsInstantTransitions">  the standard transitions, not null </param>
		''' <param name="wallOffsets">  the wall offsets, not null </param>
		''' <param name="lastRules">  the recurring last rules, size 15 or less, not null </param>
		Private Sub New(ByVal standardTransitions As Long(), ByVal standardOffsets As java.time.ZoneOffset(), ByVal savingsInstantTransitions As Long(), ByVal wallOffsets As java.time.ZoneOffset(), ByVal lastRules As ZoneOffsetTransitionRule())
			MyBase.New()

			Me.standardTransitions = standardTransitions
			Me.standardOffsets = standardOffsets
			Me.savingsInstantTransitions = savingsInstantTransitions
			Me.wallOffsets = wallOffsets
			Me.lastRules = lastRules

			If savingsInstantTransitions.Length = 0 Then
				Me.savingsLocalTransitions = EMPTY_LDT_ARRAY
			Else
				' convert savings transitions to locals
				Dim localTransitionList As IList(Of java.time.LocalDateTime) = New List(Of java.time.LocalDateTime)
				For i As Integer = 0 To savingsInstantTransitions.Length - 1
					Dim before As java.time.ZoneOffset = wallOffsets(i)
					Dim after As java.time.ZoneOffset = wallOffsets(i + 1)
					Dim trans As New ZoneOffsetTransition(savingsInstantTransitions(i), before, after)
					If trans.gap Then
						localTransitionList.Add(trans.dateTimeBefore)
						localTransitionList.Add(trans.dateTimeAfter)
					Else
						localTransitionList.Add(trans.dateTimeAfter)
						localTransitionList.Add(trans.dateTimeBefore)
					End If
				Next i
				Me.savingsLocalTransitions = localTransitionList.ToArray()
			End If
		End Sub

		''' <summary>
		''' Creates an instance of ZoneRules that has fixed zone rules.
		''' </summary>
		''' <param name="offset">  the offset this fixed zone rules is based on, not null </param>
		''' <returns> the zone rules, not null </returns>
		''' <seealso cref= #isFixedOffset() </seealso>
		Private Sub New(ByVal offset As java.time.ZoneOffset)
			Me.standardOffsets = New java.time.ZoneOffset(0){}
			Me.standardOffsets(0) = offset
			Me.standardTransitions = EMPTY_LONG_ARRAY
			Me.savingsInstantTransitions = EMPTY_LONG_ARRAY
			Me.savingsLocalTransitions = EMPTY_LDT_ARRAY
			Me.wallOffsets = standardOffsets
			Me.lastRules = EMPTY_LASTRULES
		End Sub

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		''' <summary>
		''' Writes the object using a
		''' <a href="../../../serialized-form.html#java.time.zone.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre style="font-size:1.0em">{@code
		''' 
		'''   out.writeByte(1);  // identifies a ZoneRules
		'''   out.writeInt(standardTransitions.length);
		'''   for (long trans : standardTransitions) {
		'''       Ser.writeEpochSec(trans, out);
		'''   }
		'''   for (ZoneOffset offset : standardOffsets) {
		'''       Ser.writeOffset(offset, out);
		'''   }
		'''   out.writeInt(savingsInstantTransitions.length);
		'''   for (long trans : savingsInstantTransitions) {
		'''       Ser.writeEpochSec(trans, out);
		'''   }
		'''   for (ZoneOffset offset : wallOffsets) {
		'''       Ser.writeOffset(offset, out);
		'''   }
		'''   out.writeByte(lastRules.length);
		'''   for (ZoneOffsetTransitionRule rule : lastRules) {
		'''       rule.writeExternal(out);
		'''   }
		''' }
		''' </pre>
		''' <p>
		''' Epoch second values used for offsets are encoded in a variable
		''' length form to make the common cases put fewer bytes in the stream.
		''' <pre style="font-size:1.0em">{@code
		''' 
		'''  static void writeEpochSec(long epochSec, DataOutput out) throws IOException {
		'''     if (epochSec >= -4575744000L && epochSec < 10413792000L && epochSec % 900 == 0) {  // quarter hours between 1825 and 2300
		'''         int store = (int) ((epochSec + 4575744000L) / 900);
		'''         out.writeByte((store >>> 16) & 255);
		'''         out.writeByte((store >>> 8) & 255);
		'''         out.writeByte(store & 255);
		'''      } else {
		'''          out.writeByte(255);
		'''          out.writeLong(epochSec);
		'''      }
		'''  }
		''' }
		''' </pre>
		''' <p>
		''' ZoneOffset values are encoded in a variable length form so the
		''' common cases put fewer bytes in the stream.
		''' <pre style="font-size:1.0em">{@code
		''' 
		'''  static void writeOffset(ZoneOffset offset, DataOutput out) throws IOException {
		'''     final int offsetSecs = offset.getTotalSeconds();
		'''     int offsetByte = offsetSecs % 900 == 0 ? offsetSecs / 900 : 127;  // compress to -72 to +72
		'''     out.writeByte(offsetByte);
		'''     if (offsetByte == 127) {
		'''         out.writeInt(offsetSecs);
		'''     }
		''' }
		''' }
		''' </pre> </summary>
		''' <returns> the replacing object, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZRULES, Me)
		End Function

		''' <summary>
		''' Writes the state to the stream.
		''' </summary>
		''' <param name="out">  the output stream, not null </param>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			out.writeInt(standardTransitions.Length)
			For Each trans As Long In standardTransitions
				Ser.writeEpochSec(trans, out)
			Next trans
			For Each offset_Renamed As java.time.ZoneOffset In standardOffsets
				Ser.writeOffset(offset_Renamed, out)
			Next offset_Renamed
			out.writeInt(savingsInstantTransitions.Length)
			For Each trans As Long In savingsInstantTransitions
				Ser.writeEpochSec(trans, out)
			Next trans
			For Each offset_Renamed As java.time.ZoneOffset In wallOffsets
				Ser.writeOffset(offset_Renamed, out)
			Next offset_Renamed
			out.writeByte(lastRules.Length)
			For Each rule As ZoneOffsetTransitionRule In lastRules
				rule.writeExternal(out)
			Next rule
		End Sub

		''' <summary>
		''' Reads the state from the stream.
		''' </summary>
		''' <param name="in">  the input stream, not null </param>
		''' <returns> the created object, not null </returns>
		''' <exception cref="IOException"> if an error occurs </exception>
		Shared Function readExternal(ByVal [in] As java.io.DataInput) As ZoneRules
			Dim stdSize As Integer = [in].readInt()
			Dim stdTrans As Long() = If(stdSize = 0, EMPTY_LONG_ARRAY, New Long(stdSize - 1){})
			For i As Integer = 0 To stdSize - 1
				stdTrans(i) = Ser.readEpochSec([in])
			Next i
			Dim stdOffsets As java.time.ZoneOffset() = New java.time.ZoneOffset(stdSize){}
			For i As Integer = 0 To stdOffsets.Length - 1
				stdOffsets(i) = Ser.readOffset([in])
			Next i
			Dim savSize As Integer = [in].readInt()
			Dim savTrans As Long() = If(savSize = 0, EMPTY_LONG_ARRAY, New Long(savSize - 1){})
			For i As Integer = 0 To savSize - 1
				savTrans(i) = Ser.readEpochSec([in])
			Next i
			Dim savOffsets As java.time.ZoneOffset() = New java.time.ZoneOffset(savSize){}
			For i As Integer = 0 To savOffsets.Length - 1
				savOffsets(i) = Ser.readOffset([in])
			Next i
			Dim ruleSize As Integer = [in].readByte()
			Dim rules As ZoneOffsetTransitionRule() = If(ruleSize = 0, EMPTY_LASTRULES, New ZoneOffsetTransitionRule(ruleSize - 1){})
			For i As Integer = 0 To ruleSize - 1
				rules(i) = ZoneOffsetTransitionRule.readExternal([in])
			Next i
			Return New ZoneRules(stdTrans, stdOffsets, savTrans, savOffsets, rules)
		End Function

		''' <summary>
		''' Checks of the zone rules are fixed, such that the offset never varies.
		''' </summary>
		''' <returns> true if the time-zone is fixed and the offset never changes </returns>
		Public Property fixedOffset As Boolean
			Get
				Return savingsInstantTransitions.Length = 0
			End Get
		End Property

		''' <summary>
		''' Gets the offset applicable at the specified instant in these rules.
		''' <p>
		''' The mapping from an instant to an offset is simple, there is only
		''' one valid offset for each instant.
		''' This method returns that offset.
		''' </summary>
		''' <param name="instant">  the instant to find the offset for, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the offset, not null </returns>
		Public Function getOffset(ByVal instant_Renamed As java.time.Instant) As java.time.ZoneOffset
			If savingsInstantTransitions.Length = 0 Then Return standardOffsets(0)
			Dim epochSec As Long = instant_Renamed.epochSecond
			' check if using last rules
			If lastRules.Length > 0 AndAlso epochSec > savingsInstantTransitions(savingsInstantTransitions.Length - 1) Then
				Dim year_Renamed As Integer = findYear(epochSec, wallOffsets(wallOffsets.Length - 1))
				Dim transArray As ZoneOffsetTransition() = findTransitionArray(year_Renamed)
				Dim trans As ZoneOffsetTransition = Nothing
				For i As Integer = 0 To transArray.Length - 1
					trans = transArray(i)
					If epochSec < trans.toEpochSecond() Then Return trans.offsetBefore
				Next i
				Return trans.offsetAfter
			End If

			' using historic rules
			Dim index As Integer = java.util.Arrays.binarySearch(savingsInstantTransitions, epochSec)
			If index < 0 Then index = -index - 2
			Return wallOffsets(index + 1)
		End Function

		''' <summary>
		''' Gets a suitable offset for the specified local date-time in these rules.
		''' <p>
		''' The mapping from a local date-time to an offset is not straightforward.
		''' There are three cases:
		''' <ul>
		''' <li>Normal, with one valid offset. For the vast majority of the year, the normal
		'''  case applies, where there is a single valid offset for the local date-time.</li>
		''' <li>Gap, with zero valid offsets. This is when clocks jump forward typically
		'''  due to the spring daylight savings change from "winter" to "summer".
		'''  In a gap there are local date-time values with no valid offset.</li>
		''' <li>Overlap, with two valid offsets. This is when clocks are set back typically
		'''  due to the autumn daylight savings change from "summer" to "winter".
		'''  In an overlap there are local date-time values with two valid offsets.</li>
		''' </ul>
		''' Thus, for any given local date-time there can be zero, one or two valid offsets.
		''' This method returns the single offset in the Normal case, and in the Gap or Overlap
		''' case it returns the offset before the transition.
		''' <p>
		''' Since, in the case of Gap and Overlap, the offset returned is a "best" value, rather
		''' than the "correct" value, it should be treated with care. Applications that care
		''' about the correct offset should use a combination of this method,
		''' <seealso cref="#getValidOffsets(LocalDateTime)"/> and <seealso cref="#getTransition(LocalDateTime)"/>.
		''' </summary>
		''' <param name="localDateTime">  the local date-time to query, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the best available offset for the local date-time, not null </returns>
		Public Function getOffset(ByVal localDateTime_Renamed As java.time.LocalDateTime) As java.time.ZoneOffset
			Dim info As Object = getOffsetInfo(localDateTime_Renamed)
			If TypeOf info Is ZoneOffsetTransition Then Return CType(info, ZoneOffsetTransition).offsetBefore
			Return CType(info, java.time.ZoneOffset)
		End Function

		''' <summary>
		''' Gets the offset applicable at the specified local date-time in these rules.
		''' <p>
		''' The mapping from a local date-time to an offset is not straightforward.
		''' There are three cases:
		''' <ul>
		''' <li>Normal, with one valid offset. For the vast majority of the year, the normal
		'''  case applies, where there is a single valid offset for the local date-time.</li>
		''' <li>Gap, with zero valid offsets. This is when clocks jump forward typically
		'''  due to the spring daylight savings change from "winter" to "summer".
		'''  In a gap there are local date-time values with no valid offset.</li>
		''' <li>Overlap, with two valid offsets. This is when clocks are set back typically
		'''  due to the autumn daylight savings change from "summer" to "winter".
		'''  In an overlap there are local date-time values with two valid offsets.</li>
		''' </ul>
		''' Thus, for any given local date-time there can be zero, one or two valid offsets.
		''' This method returns that list of valid offsets, which is a list of size 0, 1 or 2.
		''' In the case where there are two offsets, the earlier offset is returned at index 0
		''' and the later offset at index 1.
		''' <p>
		''' There are various ways to handle the conversion from a {@code LocalDateTime}.
		''' One technique, using this method, would be:
		''' <pre>
		'''  List&lt;ZoneOffset&gt; validOffsets = rules.getOffset(localDT);
		'''  if (validOffsets.size() == 1) {
		'''    // Normal case: only one valid offset
		'''    zoneOffset = validOffsets.get(0);
		'''  } else {
		'''    // Gap or Overlap: determine what to do from transition (which will be non-null)
		'''    ZoneOffsetTransition trans = rules.getTransition(localDT);
		'''  }
		''' </pre>
		''' <p>
		''' In theory, it is possible for there to be more than two valid offsets.
		''' This would happen if clocks to be put back more than once in quick succession.
		''' This has never happened in the history of time-zones and thus has no special handling.
		''' However, if it were to happen, then the list would return more than 2 entries.
		''' </summary>
		''' <param name="localDateTime">  the local date-time to query for valid offsets, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the list of valid offsets, may be immutable, not null </returns>
		Public Function getValidOffsets(ByVal localDateTime_Renamed As java.time.LocalDateTime) As IList(Of java.time.ZoneOffset)
			' should probably be optimized
			Dim info As Object = getOffsetInfo(localDateTime_Renamed)
			If TypeOf info Is ZoneOffsetTransition Then Return CType(info, ZoneOffsetTransition).validOffsets
			Return java.util.Collections.singletonList(CType(info, java.time.ZoneOffset))
		End Function

		''' <summary>
		''' Gets the offset transition applicable at the specified local date-time in these rules.
		''' <p>
		''' The mapping from a local date-time to an offset is not straightforward.
		''' There are three cases:
		''' <ul>
		''' <li>Normal, with one valid offset. For the vast majority of the year, the normal
		'''  case applies, where there is a single valid offset for the local date-time.</li>
		''' <li>Gap, with zero valid offsets. This is when clocks jump forward typically
		'''  due to the spring daylight savings change from "winter" to "summer".
		'''  In a gap there are local date-time values with no valid offset.</li>
		''' <li>Overlap, with two valid offsets. This is when clocks are set back typically
		'''  due to the autumn daylight savings change from "summer" to "winter".
		'''  In an overlap there are local date-time values with two valid offsets.</li>
		''' </ul>
		''' A transition is used to model the cases of a Gap or Overlap.
		''' The Normal case will return null.
		''' <p>
		''' There are various ways to handle the conversion from a {@code LocalDateTime}.
		''' One technique, using this method, would be:
		''' <pre>
		'''  ZoneOffsetTransition trans = rules.getTransition(localDT);
		'''  if (trans == null) {
		'''    // Gap or Overlap: determine what to do from transition
		'''  } else {
		'''    // Normal case: only one valid offset
		'''    zoneOffset = rule.getOffset(localDT);
		'''  }
		''' </pre>
		''' </summary>
		''' <param name="localDateTime">  the local date-time to query for offset transition, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the offset transition, null if the local date-time is not in transition </returns>
		Public Function getTransition(ByVal localDateTime_Renamed As java.time.LocalDateTime) As ZoneOffsetTransition
			Dim info As Object = getOffsetInfo(localDateTime_Renamed)
			Return (If(TypeOf info Is ZoneOffsetTransition, CType(info, ZoneOffsetTransition), Nothing))
		End Function

		Private Function getOffsetInfo(ByVal dt As java.time.LocalDateTime) As Object
			If savingsInstantTransitions.Length = 0 Then Return standardOffsets(0)
			' check if using last rules
			If lastRules.Length > 0 AndAlso dt.isAfter(savingsLocalTransitions(savingsLocalTransitions.Length - 1)) Then
				Dim transArray As ZoneOffsetTransition() = findTransitionArray(dt.year)
				Dim info As Object = Nothing
				For Each trans As ZoneOffsetTransition In transArray
					info = findOffsetInfo(dt, trans)
					If TypeOf info Is ZoneOffsetTransition OrElse info.Equals(trans.offsetBefore) Then Return info
				Next trans
				Return info
			End If

			' using historic rules
			Dim index As Integer = java.util.Arrays.binarySearch(savingsLocalTransitions, dt)
			If index = -1 Then Return wallOffsets(0)
			If index < 0 Then
				' switch negative insert position to start of matched range
				index = -index - 2
			ElseIf index < savingsLocalTransitions.Length - 1 AndAlso savingsLocalTransitions(index).Equals(savingsLocalTransitions(index + 1)) Then
				' handle overlap immediately following gap
				index += 1
			End If
			If (index And 1) = 0 Then
				' gap or overlap
				Dim dtBefore As java.time.LocalDateTime = savingsLocalTransitions(index)
				Dim dtAfter As java.time.LocalDateTime = savingsLocalTransitions(index + 1)
				Dim offsetBefore As java.time.ZoneOffset = wallOffsets(index \ 2)
				Dim offsetAfter As java.time.ZoneOffset = wallOffsets(index \ 2 + 1)
				If offsetAfter.totalSeconds > offsetBefore.totalSeconds Then
					' gap
					Return New ZoneOffsetTransition(dtBefore, offsetBefore, offsetAfter)
				Else
					' overlap
					Return New ZoneOffsetTransition(dtAfter, offsetBefore, offsetAfter)
				End If
			Else
				' normal (neither gap or overlap)
				Return wallOffsets(index \ 2 + 1)
			End If
		End Function

		''' <summary>
		''' Finds the offset info for a local date-time and transition.
		''' </summary>
		''' <param name="dt">  the date-time, not null </param>
		''' <param name="trans">  the transition, not null </param>
		''' <returns> the offset info, not null </returns>
		Private Function findOffsetInfo(ByVal dt As java.time.LocalDateTime, ByVal trans As ZoneOffsetTransition) As Object
			Dim localTransition As java.time.LocalDateTime = trans.dateTimeBefore
			If trans.gap Then
				If dt.isBefore(localTransition) Then Return trans.offsetBefore
				If dt.isBefore(trans.dateTimeAfter) Then
					Return trans
				Else
					Return trans.offsetAfter
				End If
			Else
				If dt.isBefore(localTransition) = False Then Return trans.offsetAfter
				If dt.isBefore(trans.dateTimeAfter) Then
					Return trans.offsetBefore
				Else
					Return trans
				End If
			End If
		End Function

		''' <summary>
		''' Finds the appropriate transition array for the given year.
		''' </summary>
		''' <param name="year">  the year, not null </param>
		''' <returns> the transition array, not null </returns>
		Private Function findTransitionArray(ByVal year_Renamed As Integer) As ZoneOffsetTransition()
			Dim yearObj As Integer? = year_Renamed ' should use Year class, but this saves a class load
			Dim transArray As ZoneOffsetTransition() = lastRulesCache.get(yearObj)
			If transArray IsNot Nothing Then Return transArray
			Dim ruleArray As ZoneOffsetTransitionRule() = lastRules
			transArray = New ZoneOffsetTransition(ruleArray.Length - 1){}
			For i As Integer = 0 To ruleArray.Length - 1
				transArray(i) = ruleArray(i).createTransition(year_Renamed)
			Next i
			If year_Renamed < LAST_CACHED_YEAR Then lastRulesCache.putIfAbsent(yearObj, transArray)
			Return transArray
		End Function

		''' <summary>
		''' Gets the standard offset for the specified instant in this zone.
		''' <p>
		''' This provides access to historic information on how the standard offset
		''' has changed over time.
		''' The standard offset is the offset before any daylight saving time is applied.
		''' This is typically the offset applicable during winter.
		''' </summary>
		''' <param name="instant">  the instant to find the offset information for, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the standard offset, not null </returns>
		Public Function getStandardOffset(ByVal instant_Renamed As java.time.Instant) As java.time.ZoneOffset
			If savingsInstantTransitions.Length = 0 Then Return standardOffsets(0)
			Dim epochSec As Long = instant_Renamed.epochSecond
			Dim index As Integer = java.util.Arrays.binarySearch(standardTransitions, epochSec)
			If index < 0 Then index = -index - 2
			Return standardOffsets(index + 1)
		End Function

		''' <summary>
		''' Gets the amount of daylight savings in use for the specified instant in this zone.
		''' <p>
		''' This provides access to historic information on how the amount of daylight
		''' savings has changed over time.
		''' This is the difference between the standard offset and the actual offset.
		''' Typically the amount is zero during winter and one hour during summer.
		''' Time-zones are second-based, so the nanosecond part of the duration will be zero.
		''' <p>
		''' This default implementation calculates the duration from the
		''' <seealso cref="#getOffset(java.time.Instant) actual"/> and
		''' <seealso cref="#getStandardOffset(java.time.Instant) standard"/> offsets.
		''' </summary>
		''' <param name="instant">  the instant to find the daylight savings for, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the difference between the standard and actual offset, not null </returns>
		Public Function getDaylightSavings(ByVal instant_Renamed As java.time.Instant) As java.time.Duration
			If savingsInstantTransitions.Length = 0 Then Return java.time.Duration.ZERO
			Dim standardOffset_Renamed As java.time.ZoneOffset = getStandardOffset(instant_Renamed)
			Dim actualOffset As java.time.ZoneOffset = getOffset(instant_Renamed)
			Return java.time.Duration.ofSeconds(actualOffset.totalSeconds - standardOffset_Renamed.totalSeconds)
		End Function

		''' <summary>
		''' Checks if the specified instant is in daylight savings.
		''' <p>
		''' This checks if the standard offset and the actual offset are the same
		''' for the specified instant.
		''' If they are not, it is assumed that daylight savings is in operation.
		''' <p>
		''' This default implementation compares the <seealso cref="#getOffset(java.time.Instant) actual"/>
		''' and <seealso cref="#getStandardOffset(java.time.Instant) standard"/> offsets.
		''' </summary>
		''' <param name="instant">  the instant to find the offset information for, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the standard offset, not null </returns>
		Public Function isDaylightSavings(ByVal instant_Renamed As java.time.Instant) As Boolean
			Return (getStandardOffset(instant_Renamed).Equals(getOffset(instant_Renamed)) = False)
		End Function

		''' <summary>
		''' Checks if the offset date-time is valid for these rules.
		''' <p>
		''' To be valid, the local date-time must not be in a gap and the offset
		''' must match one of the valid offsets.
		''' <p>
		''' This default implementation checks if <seealso cref="#getValidOffsets(java.time.LocalDateTime)"/>
		''' contains the specified offset.
		''' </summary>
		''' <param name="localDateTime">  the date-time to check, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <param name="offset">  the offset to check, null returns false </param>
		''' <returns> true if the offset date-time is valid for these rules </returns>
		Public Function isValidOffset(ByVal localDateTime_Renamed As java.time.LocalDateTime, ByVal offset As java.time.ZoneOffset) As Boolean
			Return getValidOffsets(localDateTime_Renamed).Contains(offset)
		End Function

		''' <summary>
		''' Gets the next transition after the specified instant.
		''' <p>
		''' This returns details of the next transition after the specified instant.
		''' For example, if the instant represents a point where "Summer" daylight savings time
		''' applies, then the method will return the transition to the next "Winter" time.
		''' </summary>
		''' <param name="instant">  the instant to get the next transition after, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the next transition after the specified instant, null if this is after the last transition </returns>
		Public Function nextTransition(ByVal instant_Renamed As java.time.Instant) As ZoneOffsetTransition
			If savingsInstantTransitions.Length = 0 Then Return Nothing
			Dim epochSec As Long = instant_Renamed.epochSecond
			' check if using last rules
			If epochSec >= savingsInstantTransitions(savingsInstantTransitions.Length - 1) Then
				If lastRules.Length = 0 Then Return Nothing
				' search year the instant is in
				Dim year_Renamed As Integer = findYear(epochSec, wallOffsets(wallOffsets.Length - 1))
				Dim transArray As ZoneOffsetTransition() = findTransitionArray(year_Renamed)
				For Each trans As ZoneOffsetTransition In transArray
					If epochSec < trans.toEpochSecond() Then Return trans
				Next trans
				' use first from following year
				If year_Renamed < java.time.Year.MAX_VALUE Then
					transArray = findTransitionArray(year_Renamed + 1)
					Return transArray(0)
				End If
				Return Nothing
			End If

			' using historic rules
			Dim index As Integer = java.util.Arrays.binarySearch(savingsInstantTransitions, epochSec)
			If index < 0 Then
				index = -index - 1 ' switched value is the next transition
			Else
				index += 1 ' exact match, so need to add one to get the next
			End If
			Return New ZoneOffsetTransition(savingsInstantTransitions(index), wallOffsets(index), wallOffsets(index + 1))
		End Function

		''' <summary>
		''' Gets the previous transition before the specified instant.
		''' <p>
		''' This returns details of the previous transition after the specified instant.
		''' For example, if the instant represents a point where "summer" daylight saving time
		''' applies, then the method will return the transition from the previous "winter" time.
		''' </summary>
		''' <param name="instant">  the instant to get the previous transition after, not null, but null
		'''  may be ignored if the rules have a single offset for all instants </param>
		''' <returns> the previous transition after the specified instant, null if this is before the first transition </returns>
		Public Function previousTransition(ByVal instant_Renamed As java.time.Instant) As ZoneOffsetTransition
			If savingsInstantTransitions.Length = 0 Then Return Nothing
			Dim epochSec As Long = instant_Renamed.epochSecond
			If instant_Renamed.nano > 0 AndAlso epochSec < Long.MaxValue Then epochSec += 1 ' allow rest of method to only use seconds

			' check if using last rules
			Dim lastHistoric As Long = savingsInstantTransitions(savingsInstantTransitions.Length - 1)
			If lastRules.Length > 0 AndAlso epochSec > lastHistoric Then
				' search year the instant is in
				Dim lastHistoricOffset As java.time.ZoneOffset = wallOffsets(wallOffsets.Length - 1)
				Dim year_Renamed As Integer = findYear(epochSec, lastHistoricOffset)
				Dim transArray As ZoneOffsetTransition() = findTransitionArray(year_Renamed)
				For i As Integer = transArray.Length - 1 To 0 Step -1
					If epochSec > transArray(i).toEpochSecond() Then Return transArray(i)
				Next i
				' use last from preceding year
				Dim lastHistoricYear As Integer = findYear(lastHistoric, lastHistoricOffset)
				year_Renamed -= 1
				If year_Renamed > lastHistoricYear Then
					transArray = findTransitionArray(year_Renamed)
					Return transArray(transArray.Length - 1)
				End If
				' drop through
			End If

			' using historic rules
			Dim index As Integer = java.util.Arrays.binarySearch(savingsInstantTransitions, epochSec)
			If index < 0 Then index = -index - 1
			If index <= 0 Then Return Nothing
			Return New ZoneOffsetTransition(savingsInstantTransitions(index - 1), wallOffsets(index - 1), wallOffsets(index))
		End Function

		Private Function findYear(ByVal epochSecond As Long, ByVal offset As java.time.ZoneOffset) As Integer
			' inline for performance
			Dim localSecond As Long = epochSecond + offset.totalSeconds
			Dim localEpochDay As Long = Math.floorDiv(localSecond, 86400)
			Return java.time.LocalDate.ofEpochDay(localEpochDay).year
		End Function

		''' <summary>
		''' Gets the complete list of fully defined transitions.
		''' <p>
		''' The complete set of transitions for this rules instance is defined by this method
		''' and <seealso cref="#getTransitionRules()"/>. This method returns those transitions that have
		''' been fully defined. These are typically historical, but may be in the future.
		''' <p>
		''' The list will be empty for fixed offset rules and for any time-zone where there has
		''' only ever been a single offset. The list will also be empty if the transition rules are unknown.
		''' </summary>
		''' <returns> an immutable list of fully defined transitions, not null </returns>
		Public Property transitions As IList(Of ZoneOffsetTransition)
			Get
				Dim list As IList(Of ZoneOffsetTransition) = New List(Of ZoneOffsetTransition)
				For i As Integer = 0 To savingsInstantTransitions.Length - 1
					list.Add(New ZoneOffsetTransition(savingsInstantTransitions(i), wallOffsets(i), wallOffsets(i + 1)))
				Next i
				Return java.util.Collections.unmodifiableList(list)
			End Get
		End Property

		''' <summary>
		''' Gets the list of transition rules for years beyond those defined in the transition list.
		''' <p>
		''' The complete set of transitions for this rules instance is defined by this method
		''' and <seealso cref="#getTransitions()"/>. This method returns instances of <seealso cref="ZoneOffsetTransitionRule"/>
		''' that define an algorithm for when transitions will occur.
		''' <p>
		''' For any given {@code ZoneRules}, this list contains the transition rules for years
		''' beyond those years that have been fully defined. These rules typically refer to future
		''' daylight saving time rule changes.
		''' <p>
		''' If the zone defines daylight savings into the future, then the list will normally
		''' be of size two and hold information about entering and exiting daylight savings.
		''' If the zone does not have daylight savings, or information about future changes
		''' is uncertain, then the list will be empty.
		''' <p>
		''' The list will be empty for fixed offset rules and for any time-zone where there is no
		''' daylight saving time. The list will also be empty if the transition rules are unknown.
		''' </summary>
		''' <returns> an immutable list of transition rules, not null </returns>
		Public Property transitionRules As IList(Of ZoneOffsetTransitionRule)
			Get
				Return java.util.Collections.unmodifiableList(java.util.Arrays.asList(lastRules))
			End Get
		End Property

		''' <summary>
		''' Checks if this set of rules equals another.
		''' <p>
		''' Two rule sets are equal if they will always result in the same output
		''' for any given input instant or local date-time.
		''' Rules from two different groups may return false even if they are in fact the same.
		''' <p>
		''' This definition should result in implementations comparing their entire state.
		''' </summary>
		''' <param name="otherRules">  the other rules, null returns false </param>
		''' <returns> true if this rules is the same as that specified </returns>
		Public Overrides Function Equals(ByVal otherRules As Object) As Boolean
			If Me Is otherRules Then Return True
			If TypeOf otherRules Is ZoneRules Then
				Dim other As ZoneRules = CType(otherRules, ZoneRules)
				Return java.util.Arrays.Equals(standardTransitions, other.standardTransitions) AndAlso java.util.Arrays.Equals(standardOffsets, other.standardOffsets) AndAlso java.util.Arrays.Equals(savingsInstantTransitions, other.savingsInstantTransitions) AndAlso java.util.Arrays.Equals(wallOffsets, other.wallOffsets) AndAlso java.util.Arrays.Equals(lastRules, other.lastRules)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a suitable hash code given the definition of {@code #equals}.
		''' </summary>
		''' <returns> the hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return java.util.Arrays.hashCode(standardTransitions) Xor java.util.Arrays.hashCode(standardOffsets) Xor java.util.Arrays.hashCode(savingsInstantTransitions) Xor java.util.Arrays.hashCode(wallOffsets) Xor java.util.Arrays.hashCode(lastRules)
		End Function

		''' <summary>
		''' Returns a string describing this object.
		''' </summary>
		''' <returns> a string for debugging, not null </returns>
		Public Overrides Function ToString() As String
			Return "ZoneRules[currentStandardOffset=" & standardOffsets(standardOffsets.Length - 1) & "]"
		End Function

	End Class

End Namespace
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
	''' A transition between two offsets caused by a discontinuity in the local time-line.
	''' <p>
	''' A transition between two offsets is normally the result of a daylight savings cutover.
	''' The discontinuity is normally a gap in spring and an overlap in autumn.
	''' {@code ZoneOffsetTransition} models the transition between the two offsets.
	''' <p>
	''' Gaps occur where there are local date-times that simply do not exist.
	''' An example would be when the offset changes from {@code +03:00} to {@code +04:00}.
	''' This might be described as 'the clocks will move forward one hour tonight at 1am'.
	''' <p>
	''' Overlaps occur where there are local date-times that exist twice.
	''' An example would be when the offset changes from {@code +04:00} to {@code +03:00}.
	''' This might be described as 'the clocks will move back one hour tonight at 2am'.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ZoneOffsetTransition
		Implements Comparable(Of ZoneOffsetTransition)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -6946044323557704546L
		''' <summary>
		''' The local transition date-time at the transition.
		''' </summary>
		Private ReadOnly transition As java.time.LocalDateTime
		''' <summary>
		''' The offset before transition.
		''' </summary>
		Private ReadOnly offsetBefore As java.time.ZoneOffset
		''' <summary>
		''' The offset after transition.
		''' </summary>
		Private ReadOnly offsetAfter As java.time.ZoneOffset

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance defining a transition between two offsets.
		''' <p>
		''' Applications should normally obtain an instance from <seealso cref="ZoneRules"/>.
		''' This factory is only intended for use when creating <seealso cref="ZoneRules"/>.
		''' </summary>
		''' <param name="transition">  the transition date-time at the transition, which never
		'''  actually occurs, expressed local to the before offset, not null </param>
		''' <param name="offsetBefore">  the offset before the transition, not null </param>
		''' <param name="offsetAfter">  the offset at and after the transition, not null </param>
		''' <returns> the transition, not null </returns>
		''' <exception cref="IllegalArgumentException"> if {@code offsetBefore} and {@code offsetAfter}
		'''         are equal, or {@code transition.getNano()} returns non-zero value </exception>
		Public Shared Function [of](ByVal transition As java.time.LocalDateTime, ByVal offsetBefore As java.time.ZoneOffset, ByVal offsetAfter As java.time.ZoneOffset) As ZoneOffsetTransition
			java.util.Objects.requireNonNull(transition, "transition")
			java.util.Objects.requireNonNull(offsetBefore, "offsetBefore")
			java.util.Objects.requireNonNull(offsetAfter, "offsetAfter")
			If offsetBefore.Equals(offsetAfter) Then Throw New IllegalArgumentException("Offsets must not be equal")
			If transition.nano <> 0 Then Throw New IllegalArgumentException("Nano-of-second must be zero")
			Return New ZoneOffsetTransition(transition, offsetBefore, offsetAfter)
		End Function

		''' <summary>
		''' Creates an instance defining a transition between two offsets.
		''' </summary>
		''' <param name="transition">  the transition date-time with the offset before the transition, not null </param>
		''' <param name="offsetBefore">  the offset before the transition, not null </param>
		''' <param name="offsetAfter">  the offset at and after the transition, not null </param>
		Friend Sub New(ByVal transition As java.time.LocalDateTime, ByVal offsetBefore As java.time.ZoneOffset, ByVal offsetAfter As java.time.ZoneOffset)
			Me.transition = transition
			Me.offsetBefore = offsetBefore
			Me.offsetAfter = offsetAfter
		End Sub

		''' <summary>
		''' Creates an instance from epoch-second and offsets.
		''' </summary>
		''' <param name="epochSecond">  the transition epoch-second </param>
		''' <param name="offsetBefore">  the offset before the transition, not null </param>
		''' <param name="offsetAfter">  the offset at and after the transition, not null </param>
		Friend Sub New(ByVal epochSecond As Long, ByVal offsetBefore As java.time.ZoneOffset, ByVal offsetAfter As java.time.ZoneOffset)
			Me.transition = java.time.LocalDateTime.ofEpochSecond(epochSecond, 0, offsetBefore)
			Me.offsetBefore = offsetBefore
			Me.offsetAfter = offsetAfter
		End Sub

		'-----------------------------------------------------------------------
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
		''' Refer to the serialized form of
		''' <a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules.writeReplace</a>
		''' for the encoding of epoch seconds and offsets.
		''' <pre style="font-size:1.0em">{@code
		''' 
		'''   out.writeByte(2);                // identifies a ZoneOffsetTransition
		'''   out.writeEpochSec(toEpochSecond);
		'''   out.writeOffset(offsetBefore);
		'''   out.writeOffset(offsetAfter);
		''' }
		''' </pre> </summary>
		''' <returns> the replacing object, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZOT, Me)
		End Function

		''' <summary>
		''' Writes the state to the stream.
		''' </summary>
		''' <param name="out">  the output stream, not null </param>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			Ser.writeEpochSec(toEpochSecond(), out)
			Ser.writeOffset(offsetBefore, out)
			Ser.writeOffset(offsetAfter, out)
		End Sub

		''' <summary>
		''' Reads the state from the stream.
		''' </summary>
		''' <param name="in">  the input stream, not null </param>
		''' <returns> the created object, not null </returns>
		''' <exception cref="IOException"> if an error occurs </exception>
		Shared Function readExternal(ByVal [in] As java.io.DataInput) As ZoneOffsetTransition
			Dim epochSecond As Long = Ser.readEpochSec([in])
			Dim before As java.time.ZoneOffset = Ser.readOffset([in])
			Dim after As java.time.ZoneOffset = Ser.readOffset([in])
			If before.Equals(after) Then Throw New IllegalArgumentException("Offsets must not be equal")
			Return New ZoneOffsetTransition(epochSecond, before, after)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the transition instant.
		''' <p>
		''' This is the instant of the discontinuity, which is defined as the first
		''' instant that the 'after' offset applies.
		''' <p>
		''' The methods <seealso cref="#getInstant()"/>, <seealso cref="#getDateTimeBefore()"/> and <seealso cref="#getDateTimeAfter()"/>
		''' all represent the same instant.
		''' </summary>
		''' <returns> the transition instant, not null </returns>
		Public Property instant As java.time.Instant
			Get
				Return transition.toInstant(offsetBefore)
			End Get
		End Property

		''' <summary>
		''' Gets the transition instant as an epoch second.
		''' </summary>
		''' <returns> the transition epoch second </returns>
		Public Function toEpochSecond() As Long
			Return transition.toEpochSecond(offsetBefore)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Gets the local transition date-time, as would be expressed with the 'before' offset.
		''' <p>
		''' This is the date-time where the discontinuity begins expressed with the 'before' offset.
		''' At this instant, the 'after' offset is actually used, therefore the combination of this
		''' date-time and the 'before' offset will never occur.
		''' <p>
		''' The combination of the 'before' date-time and offset represents the same instant
		''' as the 'after' date-time and offset.
		''' </summary>
		''' <returns> the transition date-time expressed with the before offset, not null </returns>
		Public Property dateTimeBefore As java.time.LocalDateTime
			Get
				Return transition
			End Get
		End Property

		''' <summary>
		''' Gets the local transition date-time, as would be expressed with the 'after' offset.
		''' <p>
		''' This is the first date-time after the discontinuity, when the new offset applies.
		''' <p>
		''' The combination of the 'before' date-time and offset represents the same instant
		''' as the 'after' date-time and offset.
		''' </summary>
		''' <returns> the transition date-time expressed with the after offset, not null </returns>
		Public Property dateTimeAfter As java.time.LocalDateTime
			Get
				Return transition.plusSeconds(durationSeconds)
			End Get
		End Property

		''' <summary>
		''' Gets the offset before the transition.
		''' <p>
		''' This is the offset in use before the instant of the transition.
		''' </summary>
		''' <returns> the offset before the transition, not null </returns>
		Public Property offsetBefore As java.time.ZoneOffset
			Get
				Return offsetBefore
			End Get
		End Property

		''' <summary>
		''' Gets the offset after the transition.
		''' <p>
		''' This is the offset in use on and after the instant of the transition.
		''' </summary>
		''' <returns> the offset after the transition, not null </returns>
		Public Property offsetAfter As java.time.ZoneOffset
			Get
				Return offsetAfter
			End Get
		End Property

		''' <summary>
		''' Gets the duration of the transition.
		''' <p>
		''' In most cases, the transition duration is one hour, however this is not always the case.
		''' The duration will be positive for a gap and negative for an overlap.
		''' Time-zones are second-based, so the nanosecond part of the duration will be zero.
		''' </summary>
		''' <returns> the duration of the transition, positive for gaps, negative for overlaps </returns>
		Public Property duration As java.time.Duration
			Get
				Return java.time.Duration.ofSeconds(durationSeconds)
			End Get
		End Property

		''' <summary>
		''' Gets the duration of the transition in seconds.
		''' </summary>
		''' <returns> the duration in seconds </returns>
		Private Property durationSeconds As Integer
			Get
				Return offsetAfter.totalSeconds - offsetBefore.totalSeconds
			End Get
		End Property

		''' <summary>
		''' Does this transition represent a gap in the local time-line.
		''' <p>
		''' Gaps occur where there are local date-times that simply do not exist.
		''' An example would be when the offset changes from {@code +01:00} to {@code +02:00}.
		''' This might be described as 'the clocks will move forward one hour tonight at 1am'.
		''' </summary>
		''' <returns> true if this transition is a gap, false if it is an overlap </returns>
		Public Property gap As Boolean
			Get
				Return offsetAfter.totalSeconds > offsetBefore.totalSeconds
			End Get
		End Property

		''' <summary>
		''' Does this transition represent an overlap in the local time-line.
		''' <p>
		''' Overlaps occur where there are local date-times that exist twice.
		''' An example would be when the offset changes from {@code +02:00} to {@code +01:00}.
		''' This might be described as 'the clocks will move back one hour tonight at 2am'.
		''' </summary>
		''' <returns> true if this transition is an overlap, false if it is a gap </returns>
		Public Property overlap As Boolean
			Get
				Return offsetAfter.totalSeconds < offsetBefore.totalSeconds
			End Get
		End Property

		''' <summary>
		''' Checks if the specified offset is valid during this transition.
		''' <p>
		''' This checks to see if the given offset will be valid at some point in the transition.
		''' A gap will always return false.
		''' An overlap will return true if the offset is either the before or after offset.
		''' </summary>
		''' <param name="offset">  the offset to check, null returns false </param>
		''' <returns> true if the offset is valid during the transition </returns>
		Public Function isValidOffset(ByVal offset As java.time.ZoneOffset) As Boolean
			Return If(gap, False, (offsetBefore.Equals(offset) OrElse offsetAfter.Equals(offset)))
		End Function

		''' <summary>
		''' Gets the valid offsets during this transition.
		''' <p>
		''' A gap will return an empty list, while an overlap will return both offsets.
		''' </summary>
		''' <returns> the list of valid offsets </returns>
		Friend Property validOffsets As IList(Of java.time.ZoneOffset)
			Get
				If gap Then Return java.util.Collections.emptyList()
				Return java.util.Arrays.asList(offsetBefore, offsetAfter)
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this transition to another based on the transition instant.
		''' <p>
		''' This compares the instants of each transition.
		''' The offsets are ignored, making this order inconsistent with equals.
		''' </summary>
		''' <param name="transition">  the transition to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(ByVal transition As ZoneOffsetTransition) As Integer Implements Comparable(Of ZoneOffsetTransition).compareTo
			Return Me.instant.CompareTo(transition.instant)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this object equals another.
		''' <p>
		''' The entire state of the object is compared.
		''' </summary>
		''' <param name="other">  the other object to compare to, null returns false </param>
		''' <returns> true if equal </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If other Is Me Then Return True
			If TypeOf other Is ZoneOffsetTransition Then
				Dim d As ZoneOffsetTransition = CType(other, ZoneOffsetTransition)
				Return transition.Equals(d.transition) AndAlso offsetBefore.Equals(d.offsetBefore) AndAlso offsetAfter.Equals(d.offsetAfter)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a suitable hash code.
		''' </summary>
		''' <returns> the hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return transition.GetHashCode() Xor offsetBefore.GetHashCode() Xor  java.lang.[Integer].rotateLeft(offsetAfter.GetHashCode(), 16)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a string describing this object.
		''' </summary>
		''' <returns> a string for debugging, not null </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder
			buf.append("Transition[").append(If(gap, "Gap", "Overlap")).append(" at ").append(transition).append(offsetBefore).append(" to ").append(offsetAfter).append("]"c)
			Return buf.ToString()
		End Function

	End Class

End Namespace
Imports Microsoft.VisualBasic
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
	''' A clock providing access to the current instant, date and time using a time-zone.
	''' <p>
	''' Instances of this class are used to find the current instant, which can be
	''' interpreted using the stored time-zone to find the current date and time.
	''' As such, a clock can be used instead of <seealso cref="System#currentTimeMillis()"/>
	''' and <seealso cref="TimeZone#getDefault()"/>.
	''' <p>
	''' Use of a {@code Clock} is optional. All key date-time classes also have a
	''' {@code now()} factory method that uses the system clock in the default time zone.
	''' The primary purpose of this abstraction is to allow alternate clocks to be
	''' plugged in as and when required. Applications use an object to obtain the
	''' current time rather than a static method. This can simplify testing.
	''' <p>
	''' Best practice for applications is to pass a {@code Clock} into any method
	''' that requires the current instant. A dependency injection framework is one
	''' way to achieve this:
	''' <pre>
	'''  public class MyBean {
	'''    private Clock clock;  // dependency inject
	'''    ...
	'''    public void process(LocalDate eventDate) {
	'''      if (eventDate.isBefore(LocalDate.now(clock)) {
	'''        ...
	'''      }
	'''    }
	'''  }
	''' </pre>
	''' This approach allows an alternate clock, such as <seealso cref="#fixed(Instant, ZoneId) fixed"/>
	''' or <seealso cref="#offset(Clock, Duration) offset"/> to be used during testing.
	''' <p>
	''' The {@code system} factory methods provide clocks based on the best available
	''' system clock This may use <seealso cref="System#currentTimeMillis()"/>, or a higher
	''' resolution clock if one is available.
	''' 
	''' @implSpec
	''' This abstract class must be implemented with care to ensure other classes operate correctly.
	''' All implementations that can be instantiated must be final, immutable and thread-safe.
	''' <p>
	''' The principal methods are defined to allow the throwing of an exception.
	''' In normal use, no exceptions will be thrown, however one possible implementation would be to
	''' obtain the time from a central time server across the network. Obviously, in this case the
	''' lookup could fail, and so the method is permitted to throw an exception.
	''' <p>
	''' The returned instants from {@code Clock} work on a time-scale that ignores leap seconds,
	''' as described in <seealso cref="Instant"/>. If the implementation wraps a source that provides leap
	''' second information, then a mechanism should be used to "smooth" the leap second.
	''' The Java Time-Scale mandates the use of UTC-SLS, however clock implementations may choose
	''' how accurate they are with the time-scale so long as they document how they work.
	''' Implementations are therefore not required to actually perform the UTC-SLS slew or to
	''' otherwise be aware of leap seconds.
	''' <p>
	''' Implementations should implement {@code Serializable} wherever possible and must
	''' document whether or not they do support serialization.
	''' 
	''' @implNote
	''' The clock implementation provided here is based on <seealso cref="System#currentTimeMillis()"/>.
	''' That method provides little to no guarantee about the accuracy of the clock.
	''' Applications requiring a more accurate clock must implement this abstract class
	''' themselves using a different external clock, such as an NTP server.
	''' 
	''' @since 1.8
	''' </summary>
	Public MustInherit Class Clock

		''' <summary>
		''' Obtains a clock that returns the current instant using the best available
		''' system clock, converting to date and time using the UTC time-zone.
		''' <p>
		''' This clock, rather than <seealso cref="#systemDefaultZone()"/>, should be used when
		''' you need the current instant without the date or time.
		''' <p>
		''' This clock is based on the best available system clock.
		''' This may use <seealso cref="System#currentTimeMillis()"/>, or a higher resolution
		''' clock if one is available.
		''' <p>
		''' Conversion from instant to date or time uses the <seealso cref="ZoneOffset#UTC UTC time-zone"/>.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}.
		''' It is equivalent to {@code system(ZoneOffset.UTC)}.
		''' </summary>
		''' <returns> a clock that uses the best available system clock in the UTC zone, not null </returns>
		Public Shared Function systemUTC() As Clock
			Return New SystemClock(ZoneOffset.UTC)
		End Function

		''' <summary>
		''' Obtains a clock that returns the current instant using the best available
		''' system clock, converting to date and time using the default time-zone.
		''' <p>
		''' This clock is based on the best available system clock.
		''' This may use <seealso cref="System#currentTimeMillis()"/>, or a higher resolution
		''' clock if one is available.
		''' <p>
		''' Using this method hard codes a dependency to the default time-zone into your application.
		''' It is recommended to avoid this and use a specific time-zone whenever possible.
		''' The <seealso cref="#systemUTC() UTC clock"/> should be used when you need the current instant
		''' without the date or time.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}.
		''' It is equivalent to {@code system(ZoneId.systemDefault())}.
		''' </summary>
		''' <returns> a clock that uses the best available system clock in the default zone, not null </returns>
		''' <seealso cref= ZoneId#systemDefault() </seealso>
		Public Shared Function systemDefaultZone() As Clock
			Return New SystemClock(ZoneId.systemDefault())
		End Function

		''' <summary>
		''' Obtains a clock that returns the current instant using best available
		''' system clock.
		''' <p>
		''' This clock is based on the best available system clock.
		''' This may use <seealso cref="System#currentTimeMillis()"/>, or a higher resolution
		''' clock if one is available.
		''' <p>
		''' Conversion from instant to date or time uses the specified time-zone.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}.
		''' </summary>
		''' <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		''' <returns> a clock that uses the best available system clock in the specified zone, not null </returns>
		Public Shared Function system(ByVal zone As ZoneId) As Clock
			java.util.Objects.requireNonNull(zone, "zone")
			Return New SystemClock(zone)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Obtains a clock that returns the current instant ticking in whole seconds
		''' using best available system clock.
		''' <p>
		''' This clock will always have the nano-of-second field set to zero.
		''' This ensures that the visible time ticks in whole seconds.
		''' The underlying clock is the best available system clock, equivalent to
		''' using <seealso cref="#system(ZoneId)"/>.
		''' <p>
		''' Implementations may use a caching strategy for performance reasons.
		''' As such, it is possible that the start of the second observed via this
		''' clock will be later than that observed directly via the underlying clock.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}.
		''' It is equivalent to {@code tick(system(zone), Duration.ofSeconds(1))}.
		''' </summary>
		''' <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		''' <returns> a clock that ticks in whole seconds using the specified zone, not null </returns>
		Public Shared Function tickSeconds(ByVal zone As ZoneId) As Clock
			Return New TickClock(system(zone), NANOS_PER_SECOND)
		End Function

		''' <summary>
		''' Obtains a clock that returns the current instant ticking in whole minutes
		''' using best available system clock.
		''' <p>
		''' This clock will always have the nano-of-second and second-of-minute fields set to zero.
		''' This ensures that the visible time ticks in whole minutes.
		''' The underlying clock is the best available system clock, equivalent to
		''' using <seealso cref="#system(ZoneId)"/>.
		''' <p>
		''' Implementations may use a caching strategy for performance reasons.
		''' As such, it is possible that the start of the minute observed via this
		''' clock will be later than that observed directly via the underlying clock.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}.
		''' It is equivalent to {@code tick(system(zone), Duration.ofMinutes(1))}.
		''' </summary>
		''' <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		''' <returns> a clock that ticks in whole minutes using the specified zone, not null </returns>
		Public Shared Function tickMinutes(ByVal zone As ZoneId) As Clock
			Return New TickClock(system(zone), NANOS_PER_MINUTE)
		End Function

		''' <summary>
		''' Obtains a clock that returns instants from the specified clock truncated
		''' to the nearest occurrence of the specified duration.
		''' <p>
		''' This clock will only tick as per the specified duration. Thus, if the duration
		''' is half a second, the clock will return instants truncated to the half second.
		''' <p>
		''' The tick duration must be positive. If it has a part smaller than a whole
		''' millisecond, then the whole duration must divide into one second without
		''' leaving a remainder. All normal tick durations will match these criteria,
		''' including any multiple of hours, minutes, seconds and milliseconds, and
		''' sensible nanosecond durations, such as 20ns, 250,000ns and 500,000ns.
		''' <p>
		''' A duration of zero or one nanosecond would have no truncation effect.
		''' Passing one of these will return the underlying clock.
		''' <p>
		''' Implementations may use a caching strategy for performance reasons.
		''' As such, it is possible that the start of the requested duration observed
		''' via this clock will be later than that observed directly via the underlying clock.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}
		''' providing that the base clock is.
		''' </summary>
		''' <param name="baseClock">  the base clock to base the ticking clock on, not null </param>
		''' <param name="tickDuration">  the duration of each visible tick, not negative, not null </param>
		''' <returns> a clock that ticks in whole units of the duration, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the duration is negative, or has a
		'''  part smaller than a whole millisecond such that the whole duration is not
		'''  divisible into one second </exception>
		''' <exception cref="ArithmeticException"> if the duration is too large to be represented as nanos </exception>
		Public Shared Function tick(ByVal baseClock As Clock, ByVal tickDuration As Duration) As Clock
			java.util.Objects.requireNonNull(baseClock, "baseClock")
			java.util.Objects.requireNonNull(tickDuration, "tickDuration")
			If tickDuration.negative Then Throw New IllegalArgumentException("Tick duration must not be negative")
			Dim tickNanos As Long = tickDuration.toNanos()
			If tickNanos Mod 1000000 = 0 Then
				' ok, no fraction of millisecond
			ElseIf 1000000000 Mod tickNanos = 0 Then
				' ok, divides into one second without remainder
			Else
				Throw New IllegalArgumentException("Invalid tick duration")
			End If
			If tickNanos <= 1 Then Return baseClock
			Return New TickClock(baseClock, tickNanos)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a clock that always returns the same instant.
		''' <p>
		''' This clock simply returns the specified instant.
		''' As such, it is not a clock in the conventional sense.
		''' The main use case for this is in testing, where the fixed clock ensures
		''' tests are not dependent on the current clock.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}.
		''' </summary>
		''' <param name="fixedInstant">  the instant to use as the clock, not null </param>
		''' <param name="zone">  the time-zone to use to convert the instant to date-time, not null </param>
		''' <returns> a clock that always returns the same instant, not null </returns>
		Public Shared Function fixed(ByVal fixedInstant As Instant, ByVal zone As ZoneId) As Clock
			java.util.Objects.requireNonNull(fixedInstant, "fixedInstant")
			java.util.Objects.requireNonNull(zone, "zone")
			Return New FixedClock(fixedInstant, zone)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Obtains a clock that returns instants from the specified clock with the
		''' specified duration added
		''' <p>
		''' This clock wraps another clock, returning instants that are later by the
		''' specified duration. If the duration is negative, the instants will be
		''' earlier than the current date and time.
		''' The main use case for this is to simulate running in the future or in the past.
		''' <p>
		''' A duration of zero would have no offsetting effect.
		''' Passing zero will return the underlying clock.
		''' <p>
		''' The returned implementation is immutable, thread-safe and {@code Serializable}
		''' providing that the base clock is.
		''' </summary>
		''' <param name="baseClock">  the base clock to add the duration to, not null </param>
		''' <param name="offsetDuration">  the duration to add, not null </param>
		''' <returns> a clock based on the base clock with the duration added, not null </returns>
		Public Shared Function offset(ByVal baseClock As Clock, ByVal offsetDuration As Duration) As Clock
			java.util.Objects.requireNonNull(baseClock, "baseClock")
			java.util.Objects.requireNonNull(offsetDuration, "offsetDuration")
			If offsetDuration.Equals(Duration.ZERO) Then Return baseClock
			Return New OffsetClock(baseClock, offsetDuration)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor accessible by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the time-zone being used to create dates and times.
		''' <p>
		''' A clock will typically obtain the current instant and then convert that
		''' to a date or time using a time-zone. This method returns the time-zone used.
		''' </summary>
		''' <returns> the time-zone being used to interpret instants, not null </returns>
		Public MustOverride ReadOnly Property zone As ZoneId

		''' <summary>
		''' Returns a copy of this clock with a different time-zone.
		''' <p>
		''' A clock will typically obtain the current instant and then convert that
		''' to a date or time using a time-zone. This method returns a clock with
		''' similar properties but using a different time-zone.
		''' </summary>
		''' <param name="zone">  the time-zone to change to, not null </param>
		''' <returns> a clock based on this clock with the specified time-zone, not null </returns>
		Public MustOverride Function withZone(ByVal zone As ZoneId) As Clock

		'-------------------------------------------------------------------------
		''' <summary>
		''' Gets the current millisecond instant of the clock.
		''' <p>
		''' This returns the millisecond-based instant, measured from 1970-01-01T00:00Z (UTC).
		''' This is equivalent to the definition of <seealso cref="System#currentTimeMillis()"/>.
		''' <p>
		''' Most applications should avoid this method and use <seealso cref="Instant"/> to represent
		''' an instant on the time-line rather than a raw millisecond value.
		''' This method is provided to allow the use of the clock in high performance use cases
		''' where the creation of an object would be unacceptable.
		''' <p>
		''' The default implementation currently calls <seealso cref="#instant"/>.
		''' </summary>
		''' <returns> the current millisecond instant from this clock, measured from
		'''  the Java epoch of 1970-01-01T00:00Z (UTC), not null </returns>
		''' <exception cref="DateTimeException"> if the instant cannot be obtained, not thrown by most implementations </exception>
		Public Overridable Function millis() As Long
			Return instant().toEpochMilli()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the current instant of the clock.
		''' <p>
		''' This returns an instant representing the current instant as defined by the clock.
		''' </summary>
		''' <returns> the current instant from this clock, not null </returns>
		''' <exception cref="DateTimeException"> if the instant cannot be obtained, not thrown by most implementations </exception>
		Public MustOverride Function instant() As Instant

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this clock is equal to another clock.
		''' <p>
		''' Clocks should override this method to compare equals based on
		''' their state and to meet the contract of <seealso cref="Object#equals"/>.
		''' If not overridden, the behavior is defined by <seealso cref="Object#equals"/>
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other clock </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return MyBase.Equals(obj)
		End Function

		''' <summary>
		''' A hash code for this clock.
		''' <p>
		''' Clocks should override this method based on
		''' their state and to meet the contract of <seealso cref="Object#hashCode"/>.
		''' If not overridden, the behavior is defined by <seealso cref="Object#hashCode"/>
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implementation of a clock that always returns the latest time from
		''' <seealso cref="System#currentTimeMillis()"/>.
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class SystemClock
			Inherits Clock

			Private Const serialVersionUID As Long = 6740630888130243051L
			Private ReadOnly zone As ZoneId

			Friend Sub New(ByVal zone As ZoneId)
				Me.zone = zone
			End Sub
			Public Property Overrides zone As ZoneId
				Get
					Return zone
				End Get
			End Property
			Public Overrides Function withZone(ByVal zone As ZoneId) As Clock
				If zone.Equals(Me.zone) Then ' intentional NPE Return Me
				Return New SystemClock(zone)
			End Function
			Public Overrides Function millis() As Long
				Return System.currentTimeMillis()
			End Function
			Public Overrides Function instant() As Instant
				Return Instant.ofEpochMilli(millis())
			End Function
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is SystemClock Then Return zone.Equals(CType(obj, SystemClock).zone)
				Return False
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return zone.GetHashCode() + 1
			End Function
			Public Overrides Function ToString() As String
				Return "SystemClock[" & zone & "]"
			End Function
		End Class

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implementation of a clock that always returns the same instant.
		''' This is typically used for testing.
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class FixedClock
			Inherits Clock

		   Private Const serialVersionUID As Long = 7430389292664866958L
			Private ReadOnly instant_Renamed As Instant
			Private ReadOnly zone As ZoneId

			Friend Sub New(ByVal fixedInstant As Instant, ByVal zone As ZoneId)
				Me.instant_Renamed = fixedInstant
				Me.zone = zone
			End Sub
			Public Property Overrides zone As ZoneId
				Get
					Return zone
				End Get
			End Property
			Public Overrides Function withZone(ByVal zone As ZoneId) As Clock
				If zone.Equals(Me.zone) Then ' intentional NPE Return Me
				Return New FixedClock(instant_Renamed, zone)
			End Function
			Public Overrides Function millis() As Long
				Return instant_Renamed.toEpochMilli()
			End Function
			Public Overrides Function instant() As Instant
				Return instant_Renamed
			End Function
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is FixedClock Then
					Dim other As FixedClock = CType(obj, FixedClock)
					Return instant_Renamed.Equals(other.instant) AndAlso zone.Equals(other.zone)
				End If
				Return False
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return instant_Renamed.GetHashCode() Xor zone.GetHashCode()
			End Function
			Public Overrides Function ToString() As String
				Return "FixedClock[" & instant_Renamed & "," & zone & "]"
			End Function
		End Class

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implementation of a clock that adds an offset to an underlying clock.
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class OffsetClock
			Inherits Clock

		   Private Const serialVersionUID As Long = 2007484719125426256L
			Private ReadOnly baseClock As Clock
			Private ReadOnly offset As Duration

			Friend Sub New(ByVal baseClock As Clock, ByVal offset As Duration)
				Me.baseClock = baseClock
				Me.offset = offset
			End Sub
			Public Property Overrides zone As ZoneId
				Get
					Return baseClock.zone
				End Get
			End Property
			Public Overrides Function withZone(ByVal zone As ZoneId) As Clock
				If zone.Equals(baseClock.zone) Then ' intentional NPE Return Me
				Return New OffsetClock(baseClock.withZone(zone), offset)
			End Function
			Public Overrides Function millis() As Long
				Return System.Math.addExact(baseClock.millis(), offset.toMillis())
			End Function
			Public Overrides Function instant() As Instant
				Return baseClock.instant().plus(offset)
			End Function
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is OffsetClock Then
					Dim other As OffsetClock = CType(obj, OffsetClock)
					Return baseClock.Equals(other.baseClock) AndAlso offset.Equals(other.offset)
				End If
				Return False
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return baseClock.GetHashCode() Xor offset.GetHashCode()
			End Function
			Public Overrides Function ToString() As String
				Return "OffsetClock[" & baseClock & "," & offset & "]"
			End Function
		End Class

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implementation of a clock that adds an offset to an underlying clock.
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class TickClock
			Inherits Clock

			Private Const serialVersionUID As Long = 6504659149906368850L
			Private ReadOnly baseClock As Clock
			Private ReadOnly tickNanos As Long

			Friend Sub New(ByVal baseClock As Clock, ByVal tickNanos As Long)
				Me.baseClock = baseClock
				Me.tickNanos = tickNanos
			End Sub
			Public Property Overrides zone As ZoneId
				Get
					Return baseClock.zone
				End Get
			End Property
			Public Overrides Function withZone(ByVal zone As ZoneId) As Clock
				If zone.Equals(baseClock.zone) Then ' intentional NPE Return Me
				Return New TickClock(baseClock.withZone(zone), tickNanos)
			End Function
			Public Overrides Function millis() As Long
				Dim millis_Renamed As Long = baseClock.millis()
				Return millis_Renamed - System.Math.floorMod(millis_Renamed, tickNanos \ 1000_000L)
			End Function
			Public Overrides Function instant() As Instant
				If (tickNanos Mod 1000000) = 0 Then
					Dim millis As Long = baseClock.millis()
					Return Instant.ofEpochMilli(millis - System.Math.floorMod(millis, tickNanos \ 1000_000L))
				End If
				Dim instant_Renamed As Instant = baseClock.instant()
				Dim nanos As Long = instant_Renamed.nano
				Dim adjust As Long = System.Math.floorMod(nanos, tickNanos)
				Return instant_Renamed.minusNanos(adjust)
			End Function
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is TickClock Then
					Dim other As TickClock = CType(obj, TickClock)
					Return baseClock.Equals(other.baseClock) AndAlso tickNanos = other.tickNanos
				End If
				Return False
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return baseClock.GetHashCode() Xor (CInt(Fix(tickNanos Xor (CLng(CULng(tickNanos) >> 32)))))
			End Function
			Public Overrides Function ToString() As String
				Return "TickClock[" & baseClock & "," & Duration.ofNanos(tickNanos) & "]"
			End Function
		End Class

	End Class

End Namespace
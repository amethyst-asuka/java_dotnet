Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
' * Copyright (c) 2013, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time.chrono



	''' <summary>
	''' A period expressed in terms of a standard year-month-day calendar system.
	''' <p>
	''' This class is used by applications seeking to handle dates in non-ISO calendar systems.
	''' For example, the Japanese, Minguo, Thai Buddhist and others.
	''' 
	''' @implSpec
	''' This class is immutable nad thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Friend NotInheritable Class ChronoPeriodImpl
		Implements ChronoPeriod

		' this class is only used by JDK chronology implementations and makes assumptions based on that fact

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 57387258289L

		''' <summary>
		''' The set of supported units.
		''' </summary>
		Private Shared ReadOnly SUPPORTED_UNITS As IList(Of java.time.temporal.TemporalUnit) = java.util.Collections.unmodifiableList(java.util.Arrays.asList(Of java.time.temporal.TemporalUnit)(YEARS, MONTHS, DAYS))

		''' <summary>
		''' The chronology.
		''' </summary>
		Private ReadOnly chrono As Chronology
		''' <summary>
		''' The number of years.
		''' </summary>
		Friend ReadOnly years As Integer
		''' <summary>
		''' The number of months.
		''' </summary>
		Friend ReadOnly months As Integer
		''' <summary>
		''' The number of days.
		''' </summary>
		Friend ReadOnly days As Integer

		''' <summary>
		''' Creates an instance.
		''' </summary>
		Friend Sub New(ByVal chrono As Chronology, ByVal years As Integer, ByVal months As Integer, ByVal days As Integer)
			java.util.Objects.requireNonNull(chrono, "chrono")
			Me.chrono = chrono
			Me.years = years
			Me.months = months
			Me.days = days
		End Sub

		'-----------------------------------------------------------------------
		Public Overrides Function [get](ByVal unit As java.time.temporal.TemporalUnit) As Long Implements ChronoPeriod.get
			If unit = java.time.temporal.ChronoUnit.YEARS Then
				Return years
			ElseIf unit = java.time.temporal.ChronoUnit.MONTHS Then
				Return months
			ElseIf unit = java.time.temporal.ChronoUnit.DAYS Then
				Return days
			Else
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
		End Function

		Public Property Overrides units As IList(Of java.time.temporal.TemporalUnit) Implements ChronoPeriod.getUnits
			Get
				Return ChronoPeriodImpl.SUPPORTED_UNITS
			End Get
		End Property

		Public Property Overrides chronology As Chronology Implements ChronoPeriod.getChronology
			Get
				Return chrono
			End Get
		End Property

		'-----------------------------------------------------------------------
		Public Property Overrides zero As Boolean Implements ChronoPeriod.isZero
			Get
				Return years = 0 AndAlso months = 0 AndAlso days = 0
			End Get
		End Property

		Public Property Overrides negative As Boolean Implements ChronoPeriod.isNegative
			Get
				Return years < 0 OrElse months < 0 OrElse days < 0
			End Get
		End Property

		'-----------------------------------------------------------------------
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As ChronoPeriod Implements ChronoPeriod.plus
			Dim amount As ChronoPeriodImpl = validateAmount(amountToAdd)
			Return New ChronoPeriodImpl(chrono, System.Math.addExact(years, amount.years), System.Math.addExact(months, amount.months), System.Math.addExact(days, amount.days))
		End Function

		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As ChronoPeriod Implements ChronoPeriod.minus
			Dim amount As ChronoPeriodImpl = validateAmount(amountToSubtract)
			Return New ChronoPeriodImpl(chrono, System.Math.subtractExact(years, amount.years), System.Math.subtractExact(months, amount.months), System.Math.subtractExact(days, amount.days))
		End Function

		''' <summary>
		''' Obtains an instance of {@code ChronoPeriodImpl} from a temporal amount.
		''' </summary>
		''' <param name="amount">  the temporal amount to convert, not null </param>
		''' <returns> the period, not null </returns>
		Private Function validateAmount(ByVal amount As java.time.temporal.TemporalAmount) As ChronoPeriodImpl
			java.util.Objects.requireNonNull(amount, "amount")
			If TypeOf amount Is ChronoPeriodImpl = False Then Throw New java.time.DateTimeException("Unable to obtain ChronoPeriod from TemporalAmount: " & amount.GetType())
			Dim period_Renamed As ChronoPeriodImpl = CType(amount, ChronoPeriodImpl)
			If chrono.Equals(period_Renamed.chronology) = False Then Throw New ClassCastException("Chronology mismatch, expected: " & chrono.id & ", actual: " & period_Renamed.chronology.id)
			Return period_Renamed
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function multipliedBy(ByVal scalar As Integer) As ChronoPeriod Implements ChronoPeriod.multipliedBy
			If Me.zero OrElse scalar = 1 Then Return Me
			Return New ChronoPeriodImpl(chrono, System.Math.multiplyExact(years, scalar), System.Math.multiplyExact(months, scalar), System.Math.multiplyExact(days, scalar))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function normalized() As ChronoPeriod Implements ChronoPeriod.normalized
			Dim monthRange As Long = monthRange()
			If monthRange > 0 Then
				Dim totalMonths As Long = years * monthRange + months
				Dim splitYears As Long = totalMonths \ monthRange
				Dim splitMonths As Integer = CInt(Fix(totalMonths Mod monthRange)) ' no overflow
				If splitYears = years AndAlso splitMonths = months Then Return Me
				Return New ChronoPeriodImpl(chrono, System.Math.toIntExact(splitYears), splitMonths, days)

			End If
			Return Me
		End Function

		''' <summary>
		''' Calculates the range of months.
		''' </summary>
		''' <returns> the month range, -1 if not fixed range </returns>
		Private Function monthRange() As Long
			Dim startRange As java.time.temporal.ValueRange = chrono.range(MONTH_OF_YEAR)
			If startRange.fixed AndAlso startRange.intValue Then Return startRange.maximum - startRange.minimum + 1
			Return -1
		End Function

		'-------------------------------------------------------------------------
		Public Overrides Function addTo(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal Implements ChronoPeriod.addTo
			validateChrono(temporal)
			If months = 0 Then
				If years <> 0 Then temporal = temporal.plus(years, YEARS)
			Else
				Dim monthRange As Long = monthRange()
				If monthRange > 0 Then
					temporal = temporal.plus(years * monthRange + months, MONTHS)
				Else
					If years <> 0 Then temporal = temporal.plus(years, YEARS)
					temporal = temporal.plus(months, MONTHS)
				End If
			End If
			If days <> 0 Then temporal = temporal.plus(days, DAYS)
			Return temporal
		End Function



		Public Overrides Function subtractFrom(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal Implements ChronoPeriod.subtractFrom
			validateChrono(temporal)
			If months = 0 Then
				If years <> 0 Then temporal = temporal.minus(years, YEARS)
			Else
				Dim monthRange As Long = monthRange()
				If monthRange > 0 Then
					temporal = temporal.minus(years * monthRange + months, MONTHS)
				Else
					If years <> 0 Then temporal = temporal.minus(years, YEARS)
					temporal = temporal.minus(months, MONTHS)
				End If
			End If
			If days <> 0 Then temporal = temporal.minus(days, DAYS)
			Return temporal
		End Function

		''' <summary>
		''' Validates that the temporal has the correct chronology.
		''' </summary>
		Private Sub validateChrono(ByVal temporal As java.time.temporal.TemporalAccessor)
			java.util.Objects.requireNonNull(temporal, "temporal")
			Dim temporalChrono As Chronology = temporal.query(java.time.temporal.TemporalQueries.chronology())
			If temporalChrono IsNot Nothing AndAlso chrono.Equals(temporalChrono) = False Then Throw New java.time.DateTimeException("Chronology mismatch, expected: " & chrono.id & ", actual: " & temporalChrono.id)
		End Sub

		'-----------------------------------------------------------------------
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ChronoPeriodImpl Then
				Dim other As ChronoPeriodImpl = CType(obj, ChronoPeriodImpl)
				Return years = other.years AndAlso months = other.months AndAlso days = other.days AndAlso chrono.Equals(other.chrono)
			End If
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return (years +  java.lang.[Integer].rotateLeft(months, 8) +  java.lang.[Integer].rotateLeft(days, 16)) Xor chrono.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function ToString() As String
			If zero Then
				Return chronology.ToString() & " P0D"
			Else
				Dim buf As New StringBuilder
				buf.append(chronology.ToString()).append(" "c).append("P"c)
				If years <> 0 Then buf.append(years).append("Y"c)
				If months <> 0 Then buf.append(months).append("M"c)
				If days <> 0 Then buf.append(days).append("D"c)
				Return buf.ToString()
			End If
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the Chronology using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' <pre>
		'''  out.writeByte(12);  // identifies this as a ChronoPeriodImpl
		'''  out.writeUTF(getId());  // the chronology
		'''  out.writeInt(years);
		'''  out.writeInt(months);
		'''  out.writeInt(days);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Protected Friend Function writeReplace() As Object
			Return New Ser(Ser.CHRONO_PERIOD_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			out.writeUTF(chrono.id)
			out.writeInt(years)
			out.writeInt(months)
			out.writeInt(days)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As ChronoPeriodImpl
			Dim chrono As Chronology = Chronology.of([in].readUTF())
			Dim years As Integer = [in].readInt()
			Dim months As Integer = [in].readInt()
			Dim days As Integer = [in].readInt()
			Return New ChronoPeriodImpl(chrono, years, months, days)
		End Function

	End Class

End Namespace
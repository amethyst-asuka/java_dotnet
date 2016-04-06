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
' * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
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
	''' The shared serialization delegate for this package.
	''' 
	''' @implNote
	''' This class is mutable and should be created once per serialization.
	''' 
	''' @serial include
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class Ser
		Implements java.io.Externalizable

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -8885321777449118786L

		''' <summary>
		''' Type for ZoneRules. </summary>
		Friend Const ZRULES As SByte = 1
		''' <summary>
		''' Type for ZoneOffsetTransition. </summary>
		Friend Const ZOT As SByte = 2
		''' <summary>
		''' Type for ZoneOffsetTransition. </summary>
		Friend Const ZOTRULE As SByte = 3

		''' <summary>
		''' The type being serialized. </summary>
		Private type As SByte
		''' <summary>
		''' The object being serialized. </summary>
		Private object_Renamed As Object

		''' <summary>
		''' Constructor for deserialization.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates an instance for serialization.
		''' </summary>
		''' <param name="type">  the type </param>
		''' <param name="object">  the object </param>
		Friend Sub New(  type As SByte,   [object] As Object)
			Me.type = type
			Me.object_Renamed = object_Renamed
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implements the {@code Externalizable} interface to write the object.
		''' @serialData
		''' Each serializable class is mapped to a type that is the first byte
		''' in the stream.  Refer to each class {@code writeReplace}
		''' serialized form for the value of the type and sequence of values for the type.
		''' 
		''' <ul>
		''' <li><a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransition">ZoneOffsetTransition.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransitionRule">ZoneOffsetTransitionRule.writeReplace</a>
		''' </ul>
		''' </summary>
		''' <param name="out">  the data stream to write to, not null </param>
		Public Overrides Sub writeExternal(  out As java.io.ObjectOutput)
			writeInternal(type, object_Renamed, out)
		End Sub

		Friend Shared Sub write(  [object] As Object,   out As java.io.DataOutput)
			writeInternal(ZRULES, object_Renamed, out)
		End Sub

		Private Shared Sub writeInternal(  type As SByte,   [object] As Object,   out As java.io.DataOutput)
			out.writeByte(type)
			Select Case type
				Case ZRULES
					CType(object_Renamed, ZoneRules).writeExternal(out)
				Case ZOT
					CType(object_Renamed, ZoneOffsetTransition).writeExternal(out)
				Case ZOTRULE
					CType(object_Renamed, ZoneOffsetTransitionRule).writeExternal(out)
				Case Else
					Throw New java.io.InvalidClassException("Unknown serialized type")
			End Select
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implements the {@code Externalizable} interface to read the object.
		''' @serialData
		''' The streamed type and parameters defined by the type's {@code writeReplace}
		''' method are read and passed to the corresponding static factory for the type
		''' to create a new instance.  That instance is returned as the de-serialized
		''' {@code Ser} object.
		''' 
		''' <ul>
		''' <li><a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules</a>
		''' - {@code ZoneRules.of(standardTransitions, standardOffsets, savingsInstantTransitions, wallOffsets, lastRules);}
		''' <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransition">ZoneOffsetTransition</a>
		''' - {@code ZoneOffsetTransition of(LocalDateTime.ofEpochSecond(epochSecond), offsetBefore, offsetAfter);}
		''' <li><a href="../../../serialized-form.html#java.time.zone.ZoneOffsetTransitionRule">ZoneOffsetTransitionRule</a>
		''' - {@code ZoneOffsetTransitionRule.of(month, dom, dow, time, timeEndOfDay, timeDefinition, standardOffset, offsetBefore, offsetAfter);}
		''' </ul> </summary>
		''' <param name="in">  the data to read, not null </param>
		Public Overrides Sub readExternal(  [in] As java.io.ObjectInput)
			type = [in].readByte()
			object_Renamed = readInternal(type, [in])
		End Sub

		Friend Shared Function read(  [in] As java.io.DataInput) As Object
			Dim type As SByte = [in].readByte()
			Return readInternal(type, [in])
		End Function

		Private Shared Function readInternal(  type As SByte,   [in] As java.io.DataInput) As Object
			Select Case type
				Case ZRULES
					Return ZoneRules.readExternal([in])
				Case ZOT
					Return ZoneOffsetTransition.readExternal([in])
				Case ZOTRULE
					Return ZoneOffsetTransitionRule.readExternal([in])
				Case Else
					Throw New java.io.StreamCorruptedException("Unknown serialized type")
			End Select
		End Function

		''' <summary>
		''' Returns the object that will replace this one.
		''' </summary>
		''' <returns> the read object, should never be null </returns>
		Private Function readResolve() As Object
			 Return object_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the state to the stream.
		''' </summary>
		''' <param name="offset">  the offset, not null </param>
		''' <param name="out">  the output stream, not null </param>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Shared Sub writeOffset(  offset As java.time.ZoneOffset,   out As java.io.DataOutput)
			Dim offsetSecs As Integer = offset.totalSeconds
			Dim offsetByte As Integer = If(offsetSecs Mod 900 = 0, offsetSecs \ 900, 127) ' compress to -72 to +72
			out.writeByte(offsetByte)
			If offsetByte = 127 Then out.writeInt(offsetSecs)
		End Sub

		''' <summary>
		''' Reads the state from the stream.
		''' </summary>
		''' <param name="in">  the input stream, not null </param>
		''' <returns> the created object, not null </returns>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Shared Function readOffset(  [in] As java.io.DataInput) As java.time.ZoneOffset
			Dim offsetByte As Integer = [in].readByte()
			Return (If(offsetByte = 127, java.time.ZoneOffset.ofTotalSeconds([in].readInt()), java.time.ZoneOffset.ofTotalSeconds(offsetByte * 900)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the state to the stream.
		''' </summary>
		''' <param name="epochSec">  the epoch seconds, not null </param>
		''' <param name="out">  the output stream, not null </param>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Shared Sub writeEpochSec(  epochSec As Long,   out As java.io.DataOutput)
			If epochSec >= -4575744000L AndAlso epochSec < 10413792000L AndAlso epochSec Mod 900 = 0 Then ' quarter hours between 1825 and 2300
				Dim store As Integer = CInt((epochSec + 4575744000L) \ 900)
				out.writeByte((CInt(CUInt(store) >> 16)) And 255)
				out.writeByte((CInt(CUInt(store) >> 8)) And 255)
				out.writeByte(store And 255)
			Else
				out.writeByte(255)
				out.writeLong(epochSec)
			End If
		End Sub

		''' <summary>
		''' Reads the state from the stream.
		''' </summary>
		''' <param name="in">  the input stream, not null </param>
		''' <returns> the epoch seconds, not null </returns>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Shared Function readEpochSec(  [in] As java.io.DataInput) As Long
			Dim hiByte As Integer = [in].readByte() And 255
			If hiByte = 255 Then
				Return [in].readLong()
			Else
				Dim midByte As Integer = [in].readByte() And 255
				Dim loByte As Integer = [in].readByte() And 255
				Dim tot As Long = ((hiByte << 16) + (midByte << 8) + loByte)
				Return (tot * 900) - 4575744000L
			End If
		End Function

	End Class

End Namespace
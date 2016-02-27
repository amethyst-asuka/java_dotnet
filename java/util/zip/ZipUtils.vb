Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2013, 2015, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip


	Friend Class ZipUtils

		' used to adjust values between Windows and java epoch
		Private Const WINDOWS_EPOCH_IN_MICROSECONDS As Long = -11644473600000000L

		''' <summary>
		''' Converts Windows time (in microseconds, UTC/GMT) time to FileTime.
		''' </summary>
		Public Shared Function winTimeToFileTime(ByVal wtime As Long) As java.nio.file.attribute.FileTime
			Return java.nio.file.attribute.FileTime.from(wtime \ 10 + WINDOWS_EPOCH_IN_MICROSECONDS, java.util.concurrent.TimeUnit.MICROSECONDS)
		End Function

		''' <summary>
		''' Converts FileTime to Windows time.
		''' </summary>
		Public Shared Function fileTimeToWinTime(ByVal ftime As java.nio.file.attribute.FileTime) As Long
			Return (ftime.to(java.util.concurrent.TimeUnit.MICROSECONDS) - WINDOWS_EPOCH_IN_MICROSECONDS) * 10
		End Function

		''' <summary>
		''' Converts "standard Unix time"(in seconds, UTC/GMT) to FileTime
		''' </summary>
		Public Shared Function unixTimeToFileTime(ByVal utime As Long) As java.nio.file.attribute.FileTime
			Return java.nio.file.attribute.FileTime.from(utime, java.util.concurrent.TimeUnit.SECONDS)
		End Function

		''' <summary>
		''' Converts FileTime to "standard Unix time".
		''' </summary>
		Public Shared Function fileTimeToUnixTime(ByVal ftime As java.nio.file.attribute.FileTime) As Long
			Return ftime.to(java.util.concurrent.TimeUnit.SECONDS)
		End Function

		''' <summary>
		''' Converts DOS time to Java time (number of milliseconds since epoch).
		''' </summary>
		Private Shared Function dosToJavaTime(ByVal dtime As Long) As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim d As DateTime? = New DateTime?(CInt(Fix(((dtime >> 25) And &H7f) + 80)), CInt(Fix(((dtime >> 21) And &Hf) - 1)), CInt(Fix((dtime >> 16) And &H1f)), CInt(Fix((dtime >> 11) And &H1f)), CInt(Fix((dtime >> 5) And &H3f)), CInt(Fix((dtime << 1) And &H3e))) ' Use of date constructor.
			Return d.Value.time
		End Function

		''' <summary>
		''' Converts extended DOS time to Java time, where up to 1999 milliseconds
		''' might be encoded into the upper half of the returned java.lang.[Long].
		''' </summary>
		''' <param name="xdostime"> the extended DOS time value </param>
		''' <returns> milliseconds since epoch </returns>
		Public Shared Function extendedDosToJavaTime(ByVal xdostime As Long) As Long
			Dim time As Long = dosToJavaTime(xdostime)
			Return time + (xdostime >> 32)
		End Function

		''' <summary>
		''' Converts Java time to DOS time.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function javaToDosTime(ByVal time As Long) As Long ' Use of date methods
			Dim d As DateTime? = New DateTime?(time)
			Dim year As Integer = d.Value.Year + 1900
			If year < 1980 Then Return ZipEntry.DOSTIME_BEFORE_1980
			Return (year - 1980) << 25 Or (d.Value.Month + 1) << 21 Or d.Value.date << 16 Or d.Value.Hour << 11 Or d.Value.Minute << 5 Or d.Value.Second >> 1
		End Function

		''' <summary>
		''' Converts Java time to DOS time, encoding any milliseconds lost
		''' in the conversion into the upper half of the returned java.lang.[Long].
		''' </summary>
		''' <param name="time"> milliseconds since epoch </param>
		''' <returns> DOS time with 2s remainder encoded into upper half </returns>
		Public Shared Function javaToExtendedDosTime(ByVal time As Long) As Long
			If time < 0 Then Return ZipEntry.DOSTIME_BEFORE_1980
			Dim dostime As Long = javaToDosTime(time)
			Return If(dostime <> ZipEntry.DOSTIME_BEFORE_1980, dostime + ((time Mod 2000) << 32), ZipEntry.DOSTIME_BEFORE_1980)
		End Function

		''' <summary>
		''' Fetches unsigned 16-bit value from byte array at specified offset.
		''' The bytes are assumed to be in Intel (little-endian) byte order.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public static final int get16(byte b() , int off)
			Return java.lang.[Byte].toUnsignedInt(b(off)) Or (Byte.toUnsignedInt(b(off+1)) << 8)

		''' <summary>
		''' Fetches unsigned 32-bit value from byte array at specified offset.
		''' The bytes are assumed to be in Intel (little-endian) byte order.
		''' </summary>
		public static final Long get32(SByte b() , Integer off)
			Return (get16(b, off) Or (CLng(get16(b, off+2)) << 16)) And &HffffffffL

		''' <summary>
		''' Fetches signed 64-bit value from byte array at specified offset.
		''' The bytes are assumed to be in Intel (little-endian) byte order.
		''' </summary>
		public static final Long get64(SByte b() , Integer off)
			Return get32(b, off) Or (get32(b, off+4) << 32)
	End Class

End Namespace
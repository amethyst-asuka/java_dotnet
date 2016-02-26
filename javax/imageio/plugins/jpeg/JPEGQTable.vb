Imports Microsoft.VisualBasic
Imports System.Text

'
' * Copyright (c) 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.plugins.jpeg


	''' <summary>
	''' A class encapsulating a single JPEG quantization table.
	''' The elements appear in natural order (as opposed to zig-zag order).
	''' Static variables are provided for the "standard" tables taken from
	'''  Annex K of the JPEG specification, as well as the default tables
	''' conventionally used for visually lossless encoding.
	''' <p>
	''' For more information about the operation of the standard JPEG plug-in,
	''' see the <A HREF="../../metadata/doc-files/jpeg_metadata.html">JPEG
	''' metadata format specification and usage notes</A>
	''' </summary>

	Public Class JPEGQTable

		Private Shared ReadOnly k1 As Integer() = { 16, 11, 10, 16, 24, 40, 51, 61, 12, 12, 14, 19, 26, 58, 60, 55, 14, 13, 16, 24, 40, 57, 69, 56, 14, 17, 22, 29, 51, 87, 80, 62, 18, 22, 37, 56, 68, 109, 103, 77, 24, 35, 55, 64, 81, 104, 113, 92, 49, 64, 78, 87, 103, 121, 120, 101, 72, 92, 95, 98, 112, 100, 103, 99 }

		Private Shared ReadOnly k1div2 As Integer() = { 8, 6, 5, 8, 12, 20, 26, 31, 6, 6, 7, 10, 13, 29, 30, 28, 7, 7, 8, 12, 20, 29, 35, 28, 7, 9, 11, 15, 26, 44, 40, 31, 9, 11, 19, 28, 34, 55, 52, 39, 12, 18, 28, 32, 41, 52, 57, 46, 25, 32, 39, 44, 52, 61, 60, 51, 36, 46, 48, 49, 56, 50, 52, 50 }

		Private Shared ReadOnly k2 As Integer() = { 17, 18, 24, 47, 99, 99, 99, 99, 18, 21, 26, 66, 99, 99, 99, 99, 24, 26, 56, 99, 99, 99, 99, 99, 47, 66, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }

		Private Shared ReadOnly k2div2 As Integer() = { 9, 9, 12, 24, 50, 50, 50, 50, 9, 11, 13, 33, 50, 50, 50, 50, 12, 13, 28, 50, 50, 50, 50, 50, 24, 33, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 }

		''' <summary>
		''' The sample luminance quantization table given in the JPEG
		''' specification, table K.1. According to the specification,
		''' these values produce "good" quality output. </summary>
		''' <seealso cref= #K1Div2Luminance </seealso>
		Public Shared ReadOnly K1Luminance As New JPEGQTable(k1, False)

		''' <summary>
		''' The sample luminance quantization table given in the JPEG
		''' specification, table K.1, with all elements divided by 2.
		''' According to the specification, these values produce "very good"
		''' quality output. This is the table usually used for "visually lossless"
		''' encoding, and is the default luminance table used if the default
		''' tables and quality settings are used. </summary>
		''' <seealso cref= #K1Luminance </seealso>
		Public Shared ReadOnly K1Div2Luminance As New JPEGQTable(k1div2, False)

		''' <summary>
		''' The sample chrominance quantization table given in the JPEG
		''' specification, table K.2. According to the specification,
		''' these values produce "good" quality output. </summary>
		''' <seealso cref= #K2Div2Chrominance </seealso>
		Public Shared ReadOnly K2Chrominance As New JPEGQTable(k2, False)

		''' <summary>
		''' The sample chrominance quantization table given in the JPEG
		''' specification, table K.1, with all elements divided by 2.
		''' According to the specification, these values produce "very good"
		''' quality output. This is the table usually used for "visually lossless"
		''' encoding, and is the default chrominance table used if the default
		''' tables and quality settings are used. </summary>
		''' <seealso cref= #K2Chrominance </seealso>
		Public Shared ReadOnly K2Div2Chrominance As New JPEGQTable(k2div2, False)

		Private qTable As Integer()

		Private Sub New(ByVal table As Integer(), ByVal copy As Boolean)
			qTable = If(copy, java.util.Arrays.copyOf(table, table.Length), table)
		End Sub

		''' <summary>
		''' Constructs a quantization table from the argument, which must
		''' contain 64 elements in natural order (not zig-zag order).
		''' A copy is made of the the input array. </summary>
		''' <param name="table"> the quantization table, as an <code>int</code> array. </param>
		''' <exception cref="IllegalArgumentException"> if <code>table</code> is
		''' <code>null</code> or <code>table.length</code> is not equal to 64. </exception>
		Public Sub New(ByVal table As Integer())
			If table Is Nothing Then Throw New System.ArgumentException("table must not be null.")
			If table.Length <> 64 Then Throw New System.ArgumentException("table.length != 64")
			qTable = java.util.Arrays.copyOf(table, table.Length)
		End Sub

		''' <summary>
		''' Returns a copy of the current quantization table as an array
		''' of {@code int}s in natural (not zig-zag) order. </summary>
		''' <returns> A copy of the current quantization table. </returns>
		Public Overridable Property table As Integer()
			Get
				Return java.util.Arrays.copyOf(qTable, qTable.Length)
			End Get
		End Property

		''' <summary>
		''' Returns a new quantization table where the values are multiplied
		''' by <code>scaleFactor</code> and then clamped to the range 1..32767
		''' (or to 1..255 if <code>forceBaseline</code> is true).
		''' <p>
		''' Values of <code>scaleFactor</code> less than 1 tend to improve
		''' the quality level of the table, and values greater than 1.0
		''' degrade the quality level of the table. </summary>
		''' <param name="scaleFactor"> multiplication factor for the table. </param>
		''' <param name="forceBaseline"> if <code>true</code>,
		''' the values will be clamped to the range 1..255 </param>
		''' <returns> a new quantization table that is a linear multiple
		''' of the current table. </returns>
		Public Overridable Function getScaledInstance(ByVal scaleFactor As Single, ByVal forceBaseline As Boolean) As JPEGQTable
			Dim max As Integer = If(forceBaseline, 255, 32767)
			Dim scaledTable As Integer() = New Integer(qTable.Length - 1){}
			For i As Integer = 0 To qTable.Length - 1
				Dim sv As Integer = CInt(Fix((qTable(i) * scaleFactor)+0.5f))
				If sv < 1 Then sv = 1
				If sv > max Then sv = max
				scaledTable(i) = sv
			Next i
			Return New JPEGQTable(scaledTable)
		End Function

		''' <summary>
		''' Returns a {@code String} representing this quantization table. </summary>
		''' <returns> a {@code String} representing this quantization table. </returns>
		Public Overrides Function ToString() As String
			Dim ls As String = System.getProperty("line.separator", vbLf)
			Dim sb As New StringBuilder("JPEGQTable:" & ls)
			For i As Integer = 0 To qTable.Length - 1
				If i Mod 8 = 0 Then sb.Append(ControlChars.Tab)
				sb.Append(qTable(i))
				sb.Append(If((i Mod 8) = 7, ls, " "c))
			Next i
			Return sb.ToString()
		End Function
	End Class

End Namespace
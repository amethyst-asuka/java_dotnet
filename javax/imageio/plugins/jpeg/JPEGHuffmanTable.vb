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
	''' A class encapsulating a single JPEG Huffman table.
	''' Fields are provided for the "standard" tables taken
	''' from Annex K of the JPEG specification.
	''' These are the tables used as defaults.
	''' <p>
	''' For more information about the operation of the standard JPEG plug-in,
	''' see the <A HREF="../../metadata/doc-files/jpeg_metadata.html">JPEG
	''' metadata format specification and usage notes</A>
	''' </summary>

	Public Class JPEGHuffmanTable

	'     The data for the publically defined tables, as specified in ITU T.81
	'     * JPEG specification section K3.3 and used in the IJG library.
	'     
		Private Shared ReadOnly StdDCLuminanceLengths As Short() = { &H0, &H1, &H5, &H1, &H1, &H1, &H1, &H1, &H1, &H0, &H0, &H0, &H0, &H0, &H0, &H0 }

		Private Shared ReadOnly StdDCLuminanceValues As Short() = { &H0, &H1, &H2, &H3, &H4, &H5, &H6, &H7, &H8, &H9, &Ha, &Hb }

		Private Shared ReadOnly StdDCChrominanceLengths As Short() = { &H0, &H3, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H0, &H0, &H0, &H0, &H0 }

		Private Shared ReadOnly StdDCChrominanceValues As Short() = { &H0, &H1, &H2, &H3, &H4, &H5, &H6, &H7, &H8, &H9, &Ha, &Hb }

		Private Shared ReadOnly StdACLuminanceLengths As Short() = { &H0, &H2, &H1, &H3, &H3, &H2, &H4, &H3, &H5, &H5, &H4, &H4, &H0, &H0, &H1, &H7d }

		Private Shared ReadOnly StdACLuminanceValues As Short() = { &H1, &H2, &H3, &H0, &H4, &H11, &H5, &H12, &H21, &H31, &H41, &H6, &H13, &H51, &H61, &H7, &H22, &H71, &H14, &H32, &H81, &H91, &Ha1, &H8, &H23, &H42, &Hb1, &Hc1, &H15, &H52, &Hd1, &Hf0, &H24, &H33, &H62, &H72, &H82, &H9, &Ha, &H16, &H17, &H18, &H19, &H1a, &H25, &H26, &H27, &H28, &H29, &H2a, &H34, &H35, &H36, &H37, &H38, &H39, &H3a, &H43, &H44, &H45, &H46, &H47, &H48, &H49, &H4a, &H53, &H54, &H55, &H56, &H57, &H58, &H59, &H5a, &H63, &H64, &H65, &H66, &H67, &H68, &H69, &H6a, &H73, &H74, &H75, &H76, &H77, &H78, &H79, &H7a, &H83, &H84, &H85, &H86, &H87, &H88, &H89, &H8a, &H92, &H93, &H94, &H95, &H96, &H97, &H98, &H99, &H9a, &Ha2, &Ha3, &Ha4, &Ha5, &Ha6, &Ha7, &Ha8, &Ha9, &Haa, &Hb2, &Hb3, &Hb4, &Hb5, &Hb6, &Hb7, &Hb8, &Hb9, &Hba, &Hc2, &Hc3, &Hc4, &Hc5, &Hc6, &Hc7, &Hc8, &Hc9, &Hca, &Hd2, &Hd3, &Hd4, &Hd5, &Hd6, &Hd7, &Hd8, &Hd9, &Hda, &He1, &He2, &He3, &He4, &He5, &He6, &He7, &He8, &He9, &Hea, &Hf1, &Hf2, &Hf3, &Hf4, &Hf5, &Hf6, &Hf7, &Hf8, &Hf9, &Hfa }

		Private Shared ReadOnly StdACChrominanceLengths As Short() = { &H0, &H2, &H1, &H2, &H4, &H4, &H3, &H4, &H7, &H5, &H4, &H4, &H0, &H1, &H2, &H77 }

		Private Shared ReadOnly StdACChrominanceValues As Short() = { &H0, &H1, &H2, &H3, &H11, &H4, &H5, &H21, &H31, &H6, &H12, &H41, &H51, &H7, &H61, &H71, &H13, &H22, &H32, &H81, &H8, &H14, &H42, &H91, &Ha1, &Hb1, &Hc1, &H9, &H23, &H33, &H52, &Hf0, &H15, &H62, &H72, &Hd1, &Ha, &H16, &H24, &H34, &He1, &H25, &Hf1, &H17, &H18, &H19, &H1a, &H26, &H27, &H28, &H29, &H2a, &H35, &H36, &H37, &H38, &H39, &H3a, &H43, &H44, &H45, &H46, &H47, &H48, &H49, &H4a, &H53, &H54, &H55, &H56, &H57, &H58, &H59, &H5a, &H63, &H64, &H65, &H66, &H67, &H68, &H69, &H6a, &H73, &H74, &H75, &H76, &H77, &H78, &H79, &H7a, &H82, &H83, &H84, &H85, &H86, &H87, &H88, &H89, &H8a, &H92, &H93, &H94, &H95, &H96, &H97, &H98, &H99, &H9a, &Ha2, &Ha3, &Ha4, &Ha5, &Ha6, &Ha7, &Ha8, &Ha9, &Haa, &Hb2, &Hb3, &Hb4, &Hb5, &Hb6, &Hb7, &Hb8, &Hb9, &Hba, &Hc2, &Hc3, &Hc4, &Hc5, &Hc6, &Hc7, &Hc8, &Hc9, &Hca, &Hd2, &Hd3, &Hd4, &Hd5, &Hd6, &Hd7, &Hd8, &Hd9, &Hda, &He2, &He3, &He4, &He5, &He6, &He7, &He8, &He9, &Hea, &Hf2, &Hf3, &Hf4, &Hf5, &Hf6, &Hf7, &Hf8, &Hf9, &Hfa }

		''' <summary>
		''' The standard DC luminance Huffman table.
		''' </summary>
		Public Shared ReadOnly StdDCLuminance As New JPEGHuffmanTable(StdDCLuminanceLengths, StdDCLuminanceValues, False)

		''' <summary>
		''' The standard DC chrominance Huffman table.
		''' </summary>
		Public Shared ReadOnly StdDCChrominance As New JPEGHuffmanTable(StdDCChrominanceLengths, StdDCChrominanceValues, False)

		''' <summary>
		''' The standard AC luminance Huffman table.
		''' </summary>
		Public Shared ReadOnly StdACLuminance As New JPEGHuffmanTable(StdACLuminanceLengths, StdACLuminanceValues, False)

		''' <summary>
		''' The standard AC chrominance Huffman table.
		''' </summary>
		Public Shared ReadOnly StdACChrominance As New JPEGHuffmanTable(StdACChrominanceLengths, StdACChrominanceValues, False)

		Private lengths As Short()
		Private values As Short()

		''' <summary>
		''' Creates a Huffman table and initializes it. The input arrays are copied.
		''' The arrays must describe a possible Huffman table.
		''' For example, 3 codes cannot be expressed with a single bit.
		''' </summary>
		''' <param name="lengths"> an array of {@code short}s where <code>lengths[k]</code>
		''' is equal to the number of values with corresponding codes of
		''' length <code>k + 1</code> bits. </param>
		''' <param name="values"> an array of shorts containing the values in
		''' order of increasing code length. </param>
		''' <exception cref="IllegalArgumentException"> if <code>lengths</code> or
		''' <code>values</code> are null, the length of <code>lengths</code> is
		''' greater than 16, the length of <code>values</code> is greater than 256,
		''' if any value in <code>lengths</code> or <code>values</code> is less
		''' than zero, or if the arrays do not describe a valid Huffman table. </exception>
		Public Sub New(ByVal lengths As Short(), ByVal values As Short())
			If lengths Is Nothing OrElse values Is Nothing OrElse lengths.Length = 0 OrElse values.Length = 0 OrElse lengths.Length > 16 OrElse values.Length > 256 Then Throw New System.ArgumentException("Illegal lengths or values")
			For i As Integer = 0 To lengths.Length - 1
				If lengths(i) < 0 Then Throw New System.ArgumentException("lengths[" & i & "] < 0")
			Next i
			For i As Integer = 0 To values.Length - 1
				If values(i) < 0 Then Throw New System.ArgumentException("values[" & i & "] < 0")
			Next i
			Me.lengths = java.util.Arrays.copyOf(lengths, lengths.Length)
			Me.values = java.util.Arrays.copyOf(values, values.Length)
			validate()
		End Sub

		Private Sub validate()
			Dim sumOfLengths As Integer = 0
			For i As Integer = 0 To lengths.Length - 1
				sumOfLengths += lengths(i)
			Next i
			If sumOfLengths <> values.Length Then Throw New System.ArgumentException("lengths do not correspond " & "to length of value table")
		End Sub

		' Internal version which avoids the overhead of copying and checking 
		Private Sub New(ByVal lengths As Short(), ByVal values As Short(), ByVal copy As Boolean)
			If copy Then
				Me.lengths = java.util.Arrays.copyOf(lengths, lengths.Length)
				Me.values = java.util.Arrays.copyOf(values, values.Length)
			Else
				Me.lengths = lengths
				Me.values = values
			End If
		End Sub

		''' <summary>
		''' Returns an array of <code>short</code>s containing the number of values
		''' for each length in the Huffman table. The returned array is a copy.
		''' </summary>
		''' <returns> a <code>short</code> array where <code>array[k-1]</code>
		''' is equal to the number of values in the table of length <code>k</code>. </returns>
		''' <seealso cref= #getValues </seealso>
		Public Overridable Property lengths As Short()
			Get
				Return java.util.Arrays.copyOf(lengths, lengths.Length)
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>short</code>s containing the values arranged
		''' by increasing length of their corresponding codes.
		''' The interpretation of the array is dependent on the values returned
		''' from <code>getLengths</code>. The returned array is a copy.
		''' </summary>
		''' <returns> a <code>short</code> array of values. </returns>
		''' <seealso cref= #getLengths </seealso>
		Public Overridable Property values As Short()
			Get
				Return java.util.Arrays.copyOf(values, values.Length)
			End Get
		End Property

		''' <summary>
		''' Returns a {@code String} representing this Huffman table. </summary>
		''' <returns> a {@code String} representing this Huffman table. </returns>
		Public Overrides Function ToString() As String
			Dim ls As String = System.getProperty("line.separator", vbLf)
			Dim sb As New StringBuilder("JPEGHuffmanTable")
			sb.Append(ls).append("lengths:")
			For i As Integer = 0 To lengths.Length - 1
				sb.Append(" ").append(lengths(i))
			Next i
			sb.Append(ls).append("values:")
			For i As Integer = 0 To values.Length - 1
				sb.Append(" ").append(values(i))
			Next i
			Return sb.ToString()
		End Function
	End Class

End Namespace
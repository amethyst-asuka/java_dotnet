'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image


	''' <summary>
	''' This class defines a lookup table object.  The output of a
	''' lookup operation using an object of this class is interpreted
	''' as an unsigned byte quantity.  The lookup table contains byte
	''' data arrays for one or more bands (or components) of an image,
	''' and it contains an offset which will be subtracted from the
	''' input values before indexing the arrays.  This allows an array
	''' smaller than the native data size to be provided for a
	''' constrained input.  If there is only one array in the lookup
	''' table, it will be applied to all bands.
	''' </summary>
	''' <seealso cref= ShortLookupTable </seealso>
	''' <seealso cref= LookupOp </seealso>
	Public Class ByteLookupTable
		Inherits LookupTable

		''' <summary>
		''' Constants
		''' </summary>

		Friend data As SByte()()

		''' <summary>
		''' Constructs a ByteLookupTable object from an array of byte
		''' arrays representing a lookup table for each
		''' band.  The offset will be subtracted from input
		''' values before indexing into the arrays.  The number of
		''' bands is the length of the data argument.  The
		''' data array for each band is stored as a reference. </summary>
		''' <param name="offset"> the value subtracted from the input values
		'''        before indexing into the arrays </param>
		''' <param name="data"> an array of byte arrays representing a lookup
		'''        table for each band </param>
		''' <exception cref="IllegalArgumentException"> if <code>offset</code> is
		'''         is less than 0 or if the length of <code>data</code>
		'''         is less than 1 </exception>
		Public Sub New(ByVal offset As Integer, ByVal data As SByte()())
			MyBase.New(offset,data.Length)
			numComponents = data.Length
			numEntries = data(0).Length
			Me.data = New SByte(numComponents - 1)(){}
			' Allocate the array and copy the data reference
			For i As Integer = 0 To numComponents - 1
				Me.data(i) = data(i)
			Next i
		End Sub

		''' <summary>
		''' Constructs a ByteLookupTable object from an array
		''' of bytes representing a lookup table to be applied to all
		''' bands.  The offset will be subtracted from input
		''' values before indexing into the array.
		''' The data array is stored as a reference. </summary>
		''' <param name="offset"> the value subtracted from the input values
		'''        before indexing into the array </param>
		''' <param name="data"> an array of bytes </param>
		''' <exception cref="IllegalArgumentException"> if <code>offset</code> is
		'''         is less than 0 or if the length of <code>data</code>
		'''         is less than 1 </exception>
		Public Sub New(ByVal offset As Integer, ByVal data As SByte())
			MyBase.New(offset,data.Length)
			numComponents = 1
			numEntries = data.Length
			Me.data = New SByte(0)(){}
			Me.data(0) = data
		End Sub

		''' <summary>
		''' Returns the lookup table data by reference.  If this ByteLookupTable
		''' was constructed using a single byte array, the length of the returned
		''' array is one. </summary>
		''' <returns> the data array of this <code>ByteLookupTable</code>. </returns>
		Public Property table As SByte()()
			Get
				Return data
			End Get
		End Property

		''' <summary>
		''' Returns an array of samples of a pixel, translated with the lookup
		''' table. The source and destination array can be the same array.
		''' Array <code>dst</code> is returned.
		''' </summary>
		''' <param name="src"> the source array. </param>
		''' <param name="dst"> the destination array. This array must be at least as
		'''         long as <code>src</code>.  If <code>dst</code> is
		'''         <code>null</code>, a new array will be allocated having the
		'''         same length as <code>src</code>. </param>
		''' <returns> the array <code>dst</code>, an <code>int</code> array of
		'''         samples. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>src</code> is
		'''            longer than <code>dst</code> or if for any element
		'''            <code>i</code> of <code>src</code>,
		'''            <code>src[i]-offset</code> is either less than zero or
		'''            greater than or equal to the length of the lookup table
		'''            for any band. </exception>
		Public Overrides Function lookupPixel(ByVal src As Integer(), ByVal dst As Integer()) As Integer()
			If dst Is Nothing Then dst = New Integer(src.Length - 1){}

			If numComponents = 1 Then
				' Apply one LUT to all bands
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = src(i) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = CInt(data(0)(s))
				Next i
			Else
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = src(i) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = CInt(data(i)(s))
				Next i
			End If
			Return dst
		End Function

		''' <summary>
		''' Returns an array of samples of a pixel, translated with the lookup
		''' table. The source and destination array can be the same array.
		''' Array <code>dst</code> is returned.
		''' </summary>
		''' <param name="src"> the source array. </param>
		''' <param name="dst"> the destination array. This array must be at least as
		'''         long as <code>src</code>.  If <code>dst</code> is
		'''         <code>null</code>, a new array will be allocated having the
		'''         same length as <code>src</code>. </param>
		''' <returns> the array <code>dst</code>, an <code>int</code> array of
		'''         samples. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>src</code> is
		'''            longer than <code>dst</code> or if for any element
		'''            <code>i</code> of <code>src</code>,
		'''            {@code (src[i]&0xff)-offset} is either less than
		'''            zero or greater than or equal to the length of the
		'''            lookup table for any band. </exception>
		Public Overridable Function lookupPixel(ByVal src As SByte(), ByVal dst As SByte()) As SByte()
			If dst Is Nothing Then dst = New SByte(src.Length - 1){}

			If numComponents = 1 Then
				' Apply one LUT to all bands
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = (src(i) And &Hff) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = data(0)(s)
				Next i
			Else
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = (src(i) And &Hff) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = data(i)(s)
				Next i
			End If
			Return dst
		End Function

	End Class

End Namespace
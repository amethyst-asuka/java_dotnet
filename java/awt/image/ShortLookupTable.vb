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
	''' as an unsigned short quantity.  The lookup table contains short
	''' data arrays for one or more bands (or components) of an image,
	''' and it contains an offset which will be subtracted from the
	''' input values before indexing the arrays.  This allows an array
	''' smaller than the native data size to be provided for a
	''' constrained input.  If there is only one array in the lookup
	''' table, it will be applied to all bands.
	''' </summary>
	''' <seealso cref= ByteLookupTable </seealso>
	''' <seealso cref= LookupOp </seealso>
	Public Class ShortLookupTable
		Inherits LookupTable

		''' <summary>
		''' Constants
		''' </summary>

		Friend data As Short()()

		''' <summary>
		''' Constructs a ShortLookupTable object from an array of short
		''' arrays representing a lookup table for each
		''' band.  The offset will be subtracted from the input
		''' values before indexing into the arrays.  The number of
		''' bands is the length of the data argument.  The
		''' data array for each band is stored as a reference. </summary>
		''' <param name="offset"> the value subtracted from the input values
		'''        before indexing into the arrays </param>
		''' <param name="data"> an array of short arrays representing a lookup
		'''        table for each band </param>
		Public Sub New(  offset As Integer,   data As Short()())
			MyBase.New(offset,data.Length)
			numComponents = data.Length
			numEntries = data(0).Length
			Me.data = New Short(numComponents - 1)(){}
			' Allocate the array and copy the data reference
			For i As Integer = 0 To numComponents - 1
				Me.data(i) = data(i)
			Next i
		End Sub

		''' <summary>
		''' Constructs a ShortLookupTable object from an array
		''' of shorts representing a lookup table for each
		''' band.  The offset will be subtracted from the input
		''' values before indexing into the array.  The
		''' data array is stored as a reference. </summary>
		''' <param name="offset"> the value subtracted from the input values
		'''        before indexing into the arrays </param>
		''' <param name="data"> an array of shorts </param>
		Public Sub New(  offset As Integer,   data As Short())
			MyBase.New(offset,data.Length)
			numComponents = 1
			numEntries = data.Length
			Me.data = New Short(0)(){}
			Me.data(0) = data
		End Sub

		''' <summary>
		''' Returns the lookup table data by reference.  If this ShortLookupTable
		''' was constructed using a single short array, the length of the returned
		''' array is one. </summary>
		''' <returns> ShortLookupTable data array. </returns>
		Public Property table As Short()()
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
		'''            {@code (src[i]&0xffff)-offset} is either less than
		'''            zero or greater than or equal to the length of the
		'''            lookup table for any band. </exception>
		Public Overrides Function lookupPixel(  src As Integer(),   dst As Integer()) As Integer()
			If dst Is Nothing Then dst = New Integer(src.Length - 1){}

			If numComponents = 1 Then
				' Apply one LUT to all channels
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = (src(i) And &Hffff) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = CInt(data(0)(s))
				Next i
			Else
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = (src(i) And &Hffff) - offset
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
		'''            {@code (src[i]&0xffff)-offset} is either less than
		'''            zero or greater than or equal to the length of the
		'''            lookup table for any band. </exception>
		Public Overridable Function lookupPixel(  src As Short(),   dst As Short()) As Short()
			If dst Is Nothing Then dst = New Short(src.Length - 1){}

			If numComponents = 1 Then
				' Apply one LUT to all channels
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = (src(i) And &Hffff) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = data(0)(s)
				Next i
			Else
				For i As Integer = 0 To src.Length - 1
					Dim s As Integer = (src(i) And &Hffff) - offset
					If s < 0 Then Throw New ArrayIndexOutOfBoundsException("src[" & i & "]-offset is " & "less than zero")
					dst(i) = data(i)(s)
				Next i
			End If
			Return dst
		End Function

	End Class

End Namespace
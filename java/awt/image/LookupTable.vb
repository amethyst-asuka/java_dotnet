'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' This abstract class defines a lookup table object.  ByteLookupTable
	''' and ShortLookupTable are subclasses, which
	''' contain byte and short data, respectively.  A lookup table
	''' contains data arrays for one or more bands (or components) of an image
	''' (for example, separate arrays for R, G, and B),
	''' and it contains an offset which will be subtracted from the
	''' input values before indexing into the arrays.  This allows an array
	''' smaller than the native data size to be provided for a
	''' constrained input.  If there is only one array in the lookup
	''' table, it will be applied to all bands.  All arrays must be the
	''' same size.
	''' </summary>
	''' <seealso cref= ByteLookupTable </seealso>
	''' <seealso cref= ShortLookupTable </seealso>
	''' <seealso cref= LookupOp </seealso>
	Public MustInherit Class LookupTable
		Inherits Object

		''' <summary>
		''' Constants
		''' </summary>

		Friend numComponents As Integer
		Friend offset As Integer
		Friend numEntries As Integer

		''' <summary>
		''' Constructs a new LookupTable from the number of components and an offset
		''' into the lookup table. </summary>
		''' <param name="offset"> the offset to subtract from input values before indexing
		'''        into the data arrays for this <code>LookupTable</code> </param>
		''' <param name="numComponents"> the number of data arrays in this
		'''        <code>LookupTable</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>offset</code> is less than 0
		'''         or if <code>numComponents</code> is less than 1 </exception>
		Protected Friend Sub New(  offset As Integer,   numComponents As Integer)
			If offset < 0 Then Throw New IllegalArgumentException("Offset must be greater than 0")
			If numComponents < 1 Then Throw New IllegalArgumentException("Number of components must " & " be at least 1")
			Me.numComponents = numComponents
			Me.offset = offset
		End Sub

		''' <summary>
		''' Returns the number of components in the lookup table. </summary>
		''' <returns> the number of components in this <code>LookupTable</code>. </returns>
		Public Overridable Property numComponents As Integer
			Get
				Return numComponents
			End Get
		End Property

		''' <summary>
		''' Returns the offset. </summary>
		''' <returns> the offset of this <code>LookupTable</code>. </returns>
		Public Overridable Property offset As Integer
			Get
				Return offset
			End Get
		End Property

		''' <summary>
		''' Returns an <code>int</code> array of components for
		''' one pixel.  The <code>dest</code> array contains the
		''' result of the lookup and is returned.  If dest is
		''' <code>null</code>, a new array is allocated.  The
		''' source and destination can be equal. </summary>
		''' <param name="src"> the source array of components of one pixel </param>
		''' <param name="dest"> the destination array of components for one pixel,
		'''        translated with this <code>LookupTable</code> </param>
		''' <returns> an <code>int</code> array of components for one
		'''         pixel. </returns>
		Public MustOverride Function lookupPixel(  src As Integer(),   dest As Integer()) As Integer()

	End Class

End Namespace
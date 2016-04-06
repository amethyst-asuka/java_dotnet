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
Namespace java.util.stream

	''' <summary>
	''' Base class for a data structure for gathering elements into a buffer and then
	''' iterating them. Maintains an array of increasingly sized arrays, so there is
	''' no copying cost associated with growing the data structure.
	''' @since 1.8
	''' </summary>
	Friend MustInherit Class AbstractSpinedBuffer
		''' <summary>
		''' Minimum power-of-two for the first chunk.
		''' </summary>
		Public Const MIN_CHUNK_POWER As Integer = 4

		''' <summary>
		''' Minimum size for the first chunk.
		''' </summary>
		Public Shared ReadOnly MIN_CHUNK_SIZE As Integer = 1 << MIN_CHUNK_POWER

		''' <summary>
		''' Max power-of-two for chunks.
		''' </summary>
		Public Const MAX_CHUNK_POWER As Integer = 30

		''' <summary>
		''' Minimum array size for array-of-chunks.
		''' </summary>
		Public Const MIN_SPINE_SIZE As Integer = 8


		''' <summary>
		''' log2 of the size of the first chunk.
		''' </summary>
		Protected Friend ReadOnly initialChunkPower As Integer

		''' <summary>
		''' Index of the *next* element to write; may point into, or just outside of,
		''' the current chunk.
		''' </summary>
		Protected Friend elementIndex As Integer

		''' <summary>
		''' Index of the *current* chunk in the spine array, if the spine array is
		''' non-null.
		''' </summary>
		Protected Friend spineIndex As Integer

		''' <summary>
		''' Count of elements in all prior chunks.
		''' </summary>
		Protected Friend priorElementCount As Long()

		''' <summary>
		''' Construct with an initial capacity of 16.
		''' </summary>
		Protected Friend Sub New()
			Me.initialChunkPower = MIN_CHUNK_POWER
		End Sub

		''' <summary>
		''' Construct with a specified initial capacity.
		''' </summary>
		''' <param name="initialCapacity"> The minimum expected number of elements </param>
		Protected Friend Sub New(  initialCapacity As Integer)
			If initialCapacity < 0 Then Throw New IllegalArgumentException("Illegal Capacity: " & initialCapacity)

			Me.initialChunkPower = System.Math.Max(MIN_CHUNK_POWER,  java.lang.[Integer].SIZE -  java.lang.[Integer].numberOfLeadingZeros(initialCapacity - 1))
		End Sub

		''' <summary>
		''' Is the buffer currently empty?
		''' </summary>
		Public Overridable Property empty As Boolean
			Get
				Return (spineIndex = 0) AndAlso (elementIndex = 0)
			End Get
		End Property

		''' <summary>
		''' How many elements are currently in the buffer?
		''' </summary>
		Public Overridable Function count() As Long
			Return If(spineIndex = 0, elementIndex, priorElementCount(spineIndex) + elementIndex)
		End Function

		''' <summary>
		''' How big should the nth chunk be?
		''' </summary>
		Protected Friend Overridable Function chunkSize(  n As Integer) As Integer
			Dim power As Integer = If(n = 0 OrElse n = 1, initialChunkPower, System.Math.Min(initialChunkPower + n - 1, AbstractSpinedBuffer.MAX_CHUNK_POWER))
			Return 1 << power
		End Function

		''' <summary>
		''' Remove all data from the buffer
		''' </summary>
		Public MustOverride Sub clear()
	End Class

End Namespace
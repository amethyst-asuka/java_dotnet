'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio


	''' 
	''' 
	''' 
	''' <summary>
	''' A read-only HeapShortBuffer.  This class extends the corresponding
	''' read/write [Class], overriding the mutation methods to throw a {@link
	''' ReadOnlyBufferException} and overriding the view-buffer methods to return an
	''' instance of this class rather than of the superclass.
	''' 
	''' </summary>

	Friend Class HeapShortBufferR
		Inherits HeapShortBuffer

		' For speed these fields are actually declared in X-Buffer;
		' these declarations are here as documentation
	'    
	'
	'
	'
	'
	'    

		Friend Sub New(  cap As Integer,   lim As Integer) ' package-private







			MyBase.New(cap, lim)
			Me.isReadOnly = True

		End Sub

		Friend Sub New(  buf As Short(),   [off] As Integer,   len As Integer) ' package-private







			MyBase.New(buf, [off], len)
			Me.isReadOnly = True

		End Sub

		Protected Friend Sub New(  buf As Short(),   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)







			MyBase.New(buf, mark, pos, lim, cap, [off])
			Me.isReadOnly = True

		End Sub

		Public Overrides Function slice() As ShortBuffer
			Return New HeapShortBufferR(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
		End Function

		Public Overrides Function duplicate() As ShortBuffer
			Return New HeapShortBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As ShortBuffer








			Return duplicate()

		End Function




































		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function put(  x As Short) As ShortBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As Short) As ShortBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As Short(),   offset As Integer,   length As Integer) As ShortBuffer








			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As ShortBuffer) As ShortBuffer























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As ShortBuffer







			Throw New ReadOnlyBufferException

		End Function






































































































































































































































































































































































		Public Overrides Function order() As ByteOrder
			Return ByteOrder.nativeOrder()
		End Function



	End Class

End Namespace
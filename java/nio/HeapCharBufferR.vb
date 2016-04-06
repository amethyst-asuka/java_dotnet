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
	''' A read-only HeapCharBuffer.  This class extends the corresponding
	''' read/write [Class], overriding the mutation methods to throw a {@link
	''' ReadOnlyBufferException} and overriding the view-buffer methods to return an
	''' instance of this class rather than of the superclass.
	''' 
	''' </summary>

	Friend Class HeapCharBufferR
		Inherits HeapCharBuffer

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

		Friend Sub New(  buf As Char(),   [off] As Integer,   len As Integer) ' package-private







			MyBase.New(buf, [off], len)
			Me.isReadOnly = True

		End Sub

		Protected Friend Sub New(  buf As Char(),   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)







			MyBase.New(buf, mark, pos, lim, cap, [off])
			Me.isReadOnly = True

		End Sub

		Public Overrides Function slice() As CharBuffer
			Return New HeapCharBufferR(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
		End Function

		Public Overrides Function duplicate() As CharBuffer
			Return New HeapCharBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As CharBuffer








			Return duplicate()

		End Function




































		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function put(  x As Char) As CharBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As Char) As CharBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As Char(),   offset As Integer,   length As Integer) As CharBuffer








			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As CharBuffer) As CharBuffer























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As CharBuffer







			Throw New ReadOnlyBufferException

		End Function








































































































































































































































































































































		Friend Overrides Function ToString(  start As Integer,   [end] As Integer) As String ' package-private
			Try
				Return New String(hb, start + offset, [end] - start)
			Catch x As StringIndexOutOfBoundsException
				Throw New IndexOutOfBoundsException
			End Try
		End Function


		' --- Methods to support CharSequence ---

		Public Overrides Function subSequence(  start As Integer,   [end] As Integer) As CharBuffer
			If (start < 0) OrElse ([end] > length()) OrElse (start > [end]) Then Throw New IndexOutOfBoundsException
			Dim pos As Integer = position()
			Return New HeapCharBufferR(hb, -1, pos + start, pos + [end], capacity(), offset)
		End Function






		Public Overrides Function order() As ByteOrder
			Return ByteOrder.nativeOrder()
		End Function



	End Class

End Namespace
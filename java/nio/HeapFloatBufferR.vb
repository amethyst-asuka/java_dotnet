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
	''' A read-only HeapFloatBuffer.  This class extends the corresponding
	''' read/write [Class], overriding the mutation methods to throw a {@link
	''' ReadOnlyBufferException} and overriding the view-buffer methods to return an
	''' instance of this class rather than of the superclass.
	''' 
	''' </summary>

	Friend Class HeapFloatBufferR
		Inherits HeapFloatBuffer

		' For speed these fields are actually declared in X-Buffer;
		' these declarations are here as documentation
	'    
	'
	'
	'
	'
	'    

		Friend Sub New(ByVal cap As Integer, ByVal lim As Integer) ' package-private







			MyBase.New(cap, lim)
			Me.isReadOnly = True

		End Sub

		Friend Sub New(ByVal buf As Single(), ByVal [off] As Integer, ByVal len As Integer) ' package-private







			MyBase.New(buf, [off], len)
			Me.isReadOnly = True

		End Sub

		Protected Friend Sub New(ByVal buf As Single(), ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer)







			MyBase.New(buf, mark, pos, lim, cap, [off])
			Me.isReadOnly = True

		End Sub

		Public Overrides Function slice() As FloatBuffer
			Return New HeapFloatBufferR(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
		End Function

		Public Overrides Function duplicate() As FloatBuffer
			Return New HeapFloatBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As FloatBuffer








			Return duplicate()

		End Function




































		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function put(ByVal x As Single) As FloatBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Single) As FloatBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal src As Single(), ByVal offset As Integer, ByVal length As Integer) As FloatBuffer








			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal src As FloatBuffer) As FloatBuffer























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As FloatBuffer







			Throw New ReadOnlyBufferException

		End Function






































































































































































































































































































































































		Public Overrides Function order() As ByteOrder
			Return ByteOrder.nativeOrder()
		End Function



	End Class

End Namespace
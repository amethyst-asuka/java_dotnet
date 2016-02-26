Imports System.Collections.Generic

'
' * Copyright (c) 2001, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' SegmentCache caches <code>Segment</code>s to avoid continually creating
	''' and destroying of <code>Segment</code>s. A common use of this class would
	''' be:
	''' <pre>
	'''   Segment segment = segmentCache.getSegment();
	'''   // do something with segment
	'''   ...
	'''   segmentCache.releaseSegment(segment);
	''' </pre>
	''' 
	''' </summary>
	Friend Class SegmentCache
		''' <summary>
		''' A global cache.
		''' </summary>
		Private Shared sharedCache As New SegmentCache

		''' <summary>
		''' A list of the currently unused Segments.
		''' </summary>
		Private segments As IList(Of Segment)


		''' <summary>
		''' Returns the shared SegmentCache.
		''' </summary>
		Public Property Shared sharedInstance As SegmentCache
			Get
				Return sharedCache
			End Get
		End Property

		''' <summary>
		''' A convenience method to get a Segment from the shared
		''' <code>SegmentCache</code>.
		''' </summary>
		Public Property Shared sharedSegment As Segment
			Get
				Return sharedInstance.segment
			End Get
		End Property

		''' <summary>
		''' A convenience method to release a Segment to the shared
		''' <code>SegmentCache</code>.
		''' </summary>
		Public Shared Sub releaseSharedSegment(ByVal segment As Segment)
			sharedInstance.releaseSegment(segment)
		End Sub



		''' <summary>
		''' Creates and returns a SegmentCache.
		''' </summary>
		Public Sub New()
			segments = New List(Of Segment)(11)
		End Sub

		''' <summary>
		''' Returns a <code>Segment</code>. When done, the <code>Segment</code>
		''' should be recycled by invoking <code>releaseSegment</code>.
		''' </summary>
		Public Overridable Property segment As Segment
			Get
				SyncLock Me
					Dim size As Integer = segments.Count
    
					If size > 0 Then Return segments.Remove(size - 1)
				End SyncLock
				Return New CachedSegment
			End Get
		End Property

		''' <summary>
		''' Releases a Segment. You should not use a Segment after you release it,
		''' and you should NEVER release the same Segment more than once, eg:
		''' <pre>
		'''   segmentCache.releaseSegment(segment);
		'''   segmentCache.releaseSegment(segment);
		''' </pre>
		''' Will likely result in very bad things happening!
		''' </summary>
		Public Overridable Sub releaseSegment(ByVal segment As Segment)
			If TypeOf segment Is CachedSegment Then
				SyncLock Me
					segment.array = Nothing
					segment.count = 0
					segments.Add(segment)
				End SyncLock
			End If
		End Sub


		''' <summary>
		''' CachedSegment is used as a tagging interface to determine if
		''' a Segment can successfully be shared.
		''' </summary>
		Private Class CachedSegment
			Inherits Segment

		End Class
	End Class

End Namespace
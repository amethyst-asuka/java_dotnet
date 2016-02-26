'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.event


	''' <summary>
	''' An interface used by <code>ImageWriter</code> implementations to notify
	''' callers of their image writing methods of progress.
	''' </summary>
	''' <seealso cref= javax.imageio.ImageWriter#write
	'''  </seealso>
	Public Interface IIOWriteProgressListener
		Inherits java.util.EventListener

		''' <summary>
		''' Reports that an image write operation is beginning.  All
		''' <code>ImageWriter</code> implementations are required to call
		''' this method exactly once when beginning an image write
		''' operation.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this
		''' method. </param>
		''' <param name="imageIndex"> the index of the image being written within
		''' its containing input file or stream. </param>
		Sub imageStarted(ByVal source As javax.imageio.ImageWriter, ByVal imageIndex As Integer)

		''' <summary>
		''' Reports the approximate degree of completion of the current
		''' <code>write</code> call within the associated
		''' <code>ImageWriter</code>.
		''' 
		''' <p> The degree of completion is expressed as an index
		''' indicating which image is being written, and a percentage
		''' varying from <code>0.0F</code> to <code>100.0F</code>
		''' indicating how much of the current image has been output.  The
		''' percentage should ideally be calculated in terms of the
		''' remaining time to completion, but it is usually more practical
		''' to use a more well-defined metric such as pixels decoded or
		''' portion of input stream consumed.  In any case, a sequence of
		''' calls to this method during a given read operation should
		''' supply a monotonically increasing sequence of percentage
		''' values.  It is not necessary to supply the exact values
		''' <code>0</code> and <code>100</code>, as these may be inferred
		''' by the callee from other methods.
		''' 
		''' <p> Each particular <code>ImageWriter</code> implementation may
		''' call this method at whatever frequency it desires.  A rule of
		''' thumb is to call it around each 5 percent mark.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this method. </param>
		''' <param name="percentageDone"> the approximate percentage of decoding that
		''' has been completed. </param>
		Sub imageProgress(ByVal source As javax.imageio.ImageWriter, ByVal percentageDone As Single)

		''' <summary>
		''' Reports that the image write operation has completed.  All
		''' <code>ImageWriter</code> implementations are required to call
		''' this method exactly once upon completion of each image write
		''' operation.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this method. </param>
		Sub imageComplete(ByVal source As javax.imageio.ImageWriter)

		''' <summary>
		''' Reports that a thumbnail write operation is beginning.  All
		''' <code>ImageWriter</code> implementations are required to call
		''' this method exactly once when beginning a thumbnail write
		''' operation.
		''' </summary>
		''' <param name="source"> the <code>ImageWrite</code> object calling this method. </param>
		''' <param name="imageIndex"> the index of the image being written within its
		''' containing input file or stream. </param>
		''' <param name="thumbnailIndex"> the index of the thumbnail being written. </param>
		Sub thumbnailStarted(ByVal source As javax.imageio.ImageWriter, ByVal imageIndex As Integer, ByVal thumbnailIndex As Integer)

		''' <summary>
		''' Reports the approximate degree of completion of the current
		''' thumbnail write within the associated <code>ImageWriter</code>.
		''' The semantics are identical to those of
		''' <code>imageProgress</code>.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this
		''' method. </param>
		''' <param name="percentageDone"> the approximate percentage of decoding that
		''' has been completed. </param>
		Sub thumbnailProgress(ByVal source As javax.imageio.ImageWriter, ByVal percentageDone As Single)

		''' <summary>
		''' Reports that a thumbnail write operation has completed.  All
		''' <code>ImageWriter</code> implementations are required to call
		''' this method exactly once upon completion of each thumbnail
		''' write operation.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this
		''' method. </param>
		Sub thumbnailComplete(ByVal source As javax.imageio.ImageWriter)

		''' <summary>
		''' Reports that a write has been aborted via the writer's
		''' <code>abort</code> method.  No further notifications will be
		''' given.
		''' </summary>
		''' <param name="source"> the <code>ImageWriter</code> object calling this
		''' method. </param>
		Sub writeAborted(ByVal source As javax.imageio.ImageWriter)
	End Interface

End Namespace
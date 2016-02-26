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

Namespace javax.imageio.event


	''' <summary>
	''' An interface used by <code>ImageReader</code> implementations to
	''' notify callers of their image and thumbnail reading methods of
	''' progress.
	''' 
	''' <p> This interface receives general indications of decoding
	''' progress (via the <code>imageProgress</code> and
	''' <code>thumbnailProgress</code> methods), and events indicating when
	''' an entire image has been updated (via the
	''' <code>imageStarted</code>, <code>imageComplete</code>,
	''' <code>thumbnailStarted</code> and <code>thumbnailComplete</code>
	''' methods).  Applications that wish to be informed of pixel updates
	''' as they happen (for example, during progressive decoding), should
	''' provide an <code>IIOReadUpdateListener</code>.
	''' </summary>
	''' <seealso cref= IIOReadUpdateListener </seealso>
	''' <seealso cref= javax.imageio.ImageReader#addIIOReadProgressListener </seealso>
	''' <seealso cref= javax.imageio.ImageReader#removeIIOReadProgressListener
	'''  </seealso>
	Public Interface IIOReadProgressListener
		Inherits java.util.EventListener

		''' <summary>
		''' Reports that a sequence of read operations is beginning.
		''' <code>ImageReader</code> implementations are required to call
		''' this method exactly once from their
		''' <code>readAll(Iterator)</code> method.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		''' <param name="minIndex"> the index of the first image to be read. </param>
		Sub sequenceStarted(ByVal source As javax.imageio.ImageReader, ByVal minIndex As Integer)

		''' <summary>
		''' Reports that a sequence of read operations has completed.
		''' <code>ImageReader</code> implementations are required to call
		''' this method exactly once from their
		''' <code>readAll(Iterator)</code> method.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		Sub sequenceComplete(ByVal source As javax.imageio.ImageReader)

		''' <summary>
		''' Reports that an image read operation is beginning.  All
		''' <code>ImageReader</code> implementations are required to call
		''' this method exactly once when beginning an image read
		''' operation.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		''' <param name="imageIndex"> the index of the image being read within its
		''' containing input file or stream. </param>
		Sub imageStarted(ByVal source As javax.imageio.ImageReader, ByVal imageIndex As Integer)

		''' <summary>
		''' Reports the approximate degree of completion of the current
		''' <code>read</code> call of the associated
		''' <code>ImageReader</code>.
		''' 
		''' <p> The degree of completion is expressed as a percentage
		''' varying from <code>0.0F</code> to <code>100.0F</code>.  The
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
		''' <p> Each particular <code>ImageReader</code> implementation may
		''' call this method at whatever frequency it desires.  A rule of
		''' thumb is to call it around each 5 percent mark.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		''' <param name="percentageDone"> the approximate percentage of decoding that
		''' has been completed. </param>
		Sub imageProgress(ByVal source As javax.imageio.ImageReader, ByVal percentageDone As Single)

		''' <summary>
		''' Reports that the current image read operation has completed.
		''' All <code>ImageReader</code> implementations are required to
		''' call this method exactly once upon completion of each image
		''' read operation.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this
		''' method. </param>
		Sub imageComplete(ByVal source As javax.imageio.ImageReader)

		''' <summary>
		''' Reports that a thumbnail read operation is beginning.  All
		''' <code>ImageReader</code> implementations are required to call
		''' this method exactly once when beginning a thumbnail read
		''' operation.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		''' <param name="imageIndex"> the index of the image being read within its
		''' containing input file or stream. </param>
		''' <param name="thumbnailIndex"> the index of the thumbnail being read. </param>
		Sub thumbnailStarted(ByVal source As javax.imageio.ImageReader, ByVal imageIndex As Integer, ByVal thumbnailIndex As Integer)

		''' <summary>
		''' Reports the approximate degree of completion of the current
		''' <code>getThumbnail</code> call within the associated
		''' <code>ImageReader</code>.  The semantics are identical to those
		''' of <code>imageProgress</code>.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this method. </param>
		''' <param name="percentageDone"> the approximate percentage of decoding that
		''' has been completed. </param>
		Sub thumbnailProgress(ByVal source As javax.imageio.ImageReader, ByVal percentageDone As Single)

		''' <summary>
		''' Reports that a thumbnail read operation has completed.  All
		''' <code>ImageReader</code> implementations are required to call
		''' this method exactly once upon completion of each thumbnail read
		''' operation.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this
		''' method. </param>
		Sub thumbnailComplete(ByVal source As javax.imageio.ImageReader)

		''' <summary>
		''' Reports that a read has been aborted via the reader's
		''' <code>abort</code> method.  No further notifications will be
		''' given.
		''' </summary>
		''' <param name="source"> the <code>ImageReader</code> object calling this
		''' method. </param>
		Sub readAborted(ByVal source As javax.imageio.ImageReader)
	End Interface

End Namespace
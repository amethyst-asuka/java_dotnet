Imports System.Threading

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

'
' 

Namespace java.nio.channels.spi



	''' <summary>
	''' Base implementation class for interruptible channels.
	''' 
	''' <p> This class encapsulates the low-level machinery required to implement
	''' the asynchronous closing and interruption of channels.  A concrete channel
	''' class must invoke the <seealso cref="#begin begin"/> and <seealso cref="#end end"/> methods
	''' before and after, respectively, invoking an I/O operation that might block
	''' indefinitely.  In order to ensure that the <seealso cref="#end end"/> method is always
	''' invoked, these methods should be used within a
	''' <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block:
	''' 
	''' <blockquote><pre>
	''' boolean completed = false;
	''' try {
	'''     begin();
	'''     completed = ...;    // Perform blocking I/O operation
	'''     return ...;         // Return result
	''' } finally {
	'''     end(completed);
	''' }</pre></blockquote>
	''' 
	''' <p> The <tt>completed</tt> argument to the <seealso cref="#end end"/> method tells
	''' whether or not the I/O operation actually completed, that is, whether it had
	''' any effect that would be visible to the invoker.  In the case of an
	''' operation that reads bytes, for example, this argument should be
	''' <tt>true</tt> if, and only if, some bytes were actually transferred into the
	''' invoker's target buffer.
	''' 
	''' <p> A concrete channel class must also implement the {@link
	''' #implCloseChannel implCloseChannel} method in such a way that if it is
	''' invoked while another thread is blocked in a native I/O operation upon the
	''' channel then that operation will immediately return, either by throwing an
	''' exception or by returning normally.  If a thread is interrupted or the
	''' channel upon which it is blocked is asynchronously closed then the channel's
	''' <seealso cref="#end end"/> method will throw the appropriate exception.
	''' 
	''' <p> This class performs the synchronization required to implement the {@link
	''' java.nio.channels.Channel} specification.  Implementations of the {@link
	''' #implCloseChannel implCloseChannel} method need not synchronize against
	''' other threads that might be attempting to close the channel.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class AbstractInterruptibleChannel
		Implements Channel, InterruptibleChannel

		Private ReadOnly closeLock As New Object
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private open As Boolean = True

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Closes this channel.
		''' 
		''' <p> If the channel has already been closed then this method returns
		''' immediately.  Otherwise it marks the channel as closed and then invokes
		''' the <seealso cref="#implCloseChannel implCloseChannel"/> method in order to
		''' complete the close operation.  </p>
		''' </summary>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public Sub close() Implements Channel.close, InterruptibleChannel.close
			SyncLock closeLock
				If Not open Then Return
				open = False
				implCloseChannel()
			End SyncLock
		End Sub

		''' <summary>
		''' Closes this channel.
		''' 
		''' <p> This method is invoked by the <seealso cref="#close close"/> method in order
		''' to perform the actual work of closing the channel.  This method is only
		''' invoked if the channel has not yet been closed, and it is never invoked
		''' more than once.
		''' 
		''' <p> An implementation of this method must arrange for any other thread
		''' that is blocked in an I/O operation upon this channel to return
		''' immediately, either by throwing an exception or by returning normally.
		''' </p>
		''' </summary>
		''' <exception cref="IOException">
		'''          If an I/O error occurs while closing the channel </exception>
		Protected Friend MustOverride Sub implCloseChannel()

		Public Property open As Boolean Implements Channel.isOpen
			Get
				Return open
			End Get
		End Property


		' -- Interruption machinery --

		Private interruptor As sun.nio.ch.Interruptible
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private interrupted As Thread

		''' <summary>
		''' Marks the beginning of an I/O operation that might block indefinitely.
		''' 
		''' <p> This method should be invoked in tandem with the <seealso cref="#end end"/>
		''' method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block as
		''' shown <a href="#be">above</a>, in order to implement asynchronous
		''' closing and interruption for this channel.  </p>
		''' </summary>
		Protected Friend Sub begin()
			If interruptor Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				interruptor = New sun.nio.ch.Interruptible()
	'			{
	'					public  Sub  interrupt(Thread target)
	'					{
	'						synchronized(closeLock)
	'						{
	'							if (!open)
	'								Return;
	'							open = False;
	'							interrupted = target;
	'							try
	'							{
	'								outerInstance.implCloseChannel();
	'							}
	'							catch (IOException x)
	'							{
	'							}
	'						}
	'					}
	'			};
			End If
			blockedOn(interruptor)
			Dim [me] As Thread = Thread.CurrentThread
			If [me].interrupted Then interruptor.interrupt([me])
		End Sub

		''' <summary>
		''' Marks the end of an I/O operation that might block indefinitely.
		''' 
		''' <p> This method should be invoked in tandem with the {@link #begin
		''' begin} method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block
		''' as shown <a href="#be">above</a>, in order to implement asynchronous
		''' closing and interruption for this channel.  </p>
		''' </summary>
		''' <param name="completed">
		'''         <tt>true</tt> if, and only if, the I/O operation completed
		'''         successfully, that is, had some effect that would be visible to
		'''         the operation's invoker
		''' </param>
		''' <exception cref="AsynchronousCloseException">
		'''          If the channel was asynchronously closed
		''' </exception>
		''' <exception cref="ClosedByInterruptException">
		'''          If the thread blocked in the I/O operation was interrupted </exception>
		Protected Friend Sub [end](  completed As Boolean)
			blockedOn(Nothing)
			Dim interrupted As Thread = Me.interrupted
			If interrupted IsNot Nothing AndAlso interrupted Is Thread.CurrentThread Then
				interrupted = Nothing
				Throw New ClosedByInterruptException
			End If
			If (Not completed) AndAlso (Not open) Then Throw New AsynchronousCloseException
		End Sub


		' -- sun.misc.SharedSecrets --
		Friend Shared Sub blockedOn(  intr As sun.nio.ch.Interruptible) ' package-private
			sun.misc.SharedSecrets.javaLangAccess.blockedOn(Thread.CurrentThread, intr)
		End Sub
	End Class

End Namespace
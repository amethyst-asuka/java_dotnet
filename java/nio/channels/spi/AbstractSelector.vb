Imports System.Collections.Generic
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

Namespace java.nio.channels.spi



	''' <summary>
	''' Base implementation class for selectors.
	''' 
	''' <p> This class encapsulates the low-level machinery required to implement
	''' the interruption of selection operations.  A concrete selector class must
	''' invoke the <seealso cref="#begin begin"/> and <seealso cref="#end end"/> methods before and
	''' after, respectively, invoking an I/O operation that might block
	''' indefinitely.  In order to ensure that the <seealso cref="#end end"/> method is always
	''' invoked, these methods should be used within a
	''' <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block:
	''' 
	''' <blockquote><pre>
	''' try {
	'''     begin();
	'''     // Perform blocking I/O operation here
	'''     ...
	''' } finally {
	'''     end();
	''' }</pre></blockquote>
	''' 
	''' <p> This class also defines methods for maintaining a selector's
	''' cancelled-key set and for removing a key from its channel's key set, and
	''' declares the abstract <seealso cref="#register register"/> method that is invoked by a
	''' selectable channel's <seealso cref="AbstractSelectableChannel#register register"/>
	''' method in order to perform the actual work of registering a channel.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class AbstractSelector
		Inherits java.nio.channels.Selector

		Private selectorOpen As New java.util.concurrent.atomic.AtomicBoolean(True)

		' The provider that created this selector
		Private ReadOnly provider_Renamed As SelectorProvider

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <param name="provider">
		'''         The provider that created this selector </param>
		Protected Friend Sub New(ByVal provider As SelectorProvider)
			Me.provider_Renamed = provider
		End Sub

		Private ReadOnly cancelledKeys_Renamed As java.util.Set(Of java.nio.channels.SelectionKey) = New HashSet(Of java.nio.channels.SelectionKey)

		Friend Overridable Sub cancel(ByVal k As java.nio.channels.SelectionKey) ' package-private
			SyncLock cancelledKeys_Renamed
				cancelledKeys_Renamed.add(k)
			End SyncLock
		End Sub

		''' <summary>
		''' Closes this selector.
		''' 
		''' <p> If the selector has already been closed then this method returns
		''' immediately.  Otherwise it marks the selector as closed and then invokes
		''' the <seealso cref="#implCloseSelector implCloseSelector"/> method in order to
		''' complete the close operation.  </p>
		''' </summary>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public NotOverridable Overrides Sub close()
			Dim open_Renamed As Boolean = selectorOpen.getAndSet(False)
			If Not open_Renamed Then Return
			implCloseSelector()
		End Sub

		''' <summary>
		''' Closes this selector.
		''' 
		''' <p> This method is invoked by the <seealso cref="#close close"/> method in order
		''' to perform the actual work of closing the selector.  This method is only
		''' invoked if the selector has not yet been closed, and it is never invoked
		''' more than once.
		''' 
		''' <p> An implementation of this method must arrange for any other thread
		''' that is blocked in a selection operation upon this selector to return
		''' immediately as if by invoking the {@link
		''' java.nio.channels.Selector#wakeup wakeup} method. </p>
		''' </summary>
		''' <exception cref="IOException">
		'''          If an I/O error occurs while closing the selector </exception>
		Protected Friend MustOverride Sub implCloseSelector()

		Public Property NotOverridable Overrides open As Boolean
			Get
				Return selectorOpen.get()
			End Get
		End Property

		''' <summary>
		''' Returns the provider that created this channel.
		''' </summary>
		''' <returns>  The provider that created this channel </returns>
		Public NotOverridable Overrides Function provider() As SelectorProvider
			Return provider_Renamed
		End Function

		''' <summary>
		''' Retrieves this selector's cancelled-key set.
		''' 
		''' <p> This set should only be used while synchronized upon it.  </p>
		''' </summary>
		''' <returns>  The cancelled-key set </returns>
		Protected Friend Function cancelledKeys() As java.util.Set(Of java.nio.channels.SelectionKey)
			Return cancelledKeys_Renamed
		End Function

		''' <summary>
		''' Registers the given channel with this selector.
		''' 
		''' <p> This method is invoked by a channel's {@link
		''' AbstractSelectableChannel#register register} method in order to perform
		''' the actual work of registering the channel with this selector.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel to be registered
		''' </param>
		''' <param name="ops">
		'''         The initial interest set, which must be valid
		''' </param>
		''' <param name="att">
		'''         The initial attachment for the resulting key
		''' </param>
		''' <returns>  A new key representing the registration of the given channel
		'''          with this selector </returns>
		Protected Friend MustOverride Function register(ByVal ch As AbstractSelectableChannel, ByVal ops As Integer, ByVal att As Object) As java.nio.channels.SelectionKey

		''' <summary>
		''' Removes the given key from its channel's key set.
		''' 
		''' <p> This method must be invoked by the selector for each channel that it
		''' deregisters.  </p>
		''' </summary>
		''' <param name="key">
		'''         The selection key to be removed </param>
		Protected Friend Sub deregister(ByVal key As AbstractSelectionKey)
			CType(key.channel(), AbstractSelectableChannel).removeKey(key)
		End Sub


		' -- Interruption machinery --

		Private interruptor As sun.nio.ch.Interruptible = Nothing

		''' <summary>
		''' Marks the beginning of an I/O operation that might block indefinitely.
		''' 
		''' <p> This method should be invoked in tandem with the <seealso cref="#end end"/>
		''' method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block as
		''' shown <a href="#be">above</a>, in order to implement interruption for
		''' this selector.
		''' 
		''' <p> Invoking this method arranges for the selector's {@link
		''' Selector#wakeup wakeup} method to be invoked if a thread's {@link
		''' Thread#interrupt interrupt} method is invoked while the thread is
		''' blocked in an I/O operation upon the selector.  </p>
		''' </summary>
		Protected Friend Sub begin()
			If interruptor Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				interruptor = New sun.nio.ch.Interruptible()
	'			{
	'					public  Sub  interrupt(Thread ignore)
	'					{
	'						outerInstance.wakeup();
	'					}
	'			};
			End If
			AbstractInterruptibleChannel.blockedOn(interruptor)
			Dim [me] As Thread = Thread.CurrentThread
			If [me].interrupted Then interruptor.interrupt([me])
		End Sub

		''' <summary>
		''' Marks the end of an I/O operation that might block indefinitely.
		''' 
		''' <p> This method should be invoked in tandem with the <seealso cref="#begin begin"/>
		''' method, using a <tt>try</tt>&nbsp;...&nbsp;<tt>finally</tt> block as
		''' shown <a href="#be">above</a>, in order to implement interruption for
		''' this selector.  </p>
		''' </summary>
		Protected Friend Sub [end]()
			AbstractInterruptibleChannel.blockedOn(Nothing)
		End Sub

	End Class

End Namespace
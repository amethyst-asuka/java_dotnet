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

Namespace java.nio.channels



	''' <summary>
	''' A pair of channels that implements a unidirectional pipe.
	''' 
	''' <p> A pipe consists of a pair of channels: A writable {@link
	''' Pipe.SinkChannel sink} channel and a readable <seealso cref="Pipe.SourceChannel source"/>
	''' channel.  Once some bytes are written to the sink channel they can be read
	''' from source channel in exactlyAthe order in which they were written.
	''' 
	''' <p> Whether or not a thread writing bytes to a pipe will block until another
	''' thread reads those bytes, or some previously-written bytes, from the pipe is
	''' system-dependent and therefore unspecified.  Many pipe implementations will
	''' buffer up to a certain number of bytes between the sink and source channels,
	''' but such buffering should not be assumed.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class Pipe

		''' <summary>
		''' A channel representing the readable end of a <seealso cref="Pipe"/>.
		''' 
		''' @since 1.4
		''' </summary>
		Public MustInherit Class SourceChannel
			Inherits AbstractSelectableChannel
			Implements ReadableByteChannel, ScatteringByteChannel

				Public MustOverride Function read(  dsts As java.nio.ByteBuffer()) As Long Implements ScatteringByteChannel.read
				Public MustOverride Function read(  dsts As java.nio.ByteBuffer(),   offset As Integer,   length As Integer) As Long Implements ScatteringByteChannel.read
				Public MustOverride Function read(  dst As java.nio.ByteBuffer) As Integer Implements ReadableByteChannel.read
			''' <summary>
			''' Constructs a new instance of this class.
			''' </summary>
			''' <param name="provider">
			'''         The selector provider </param>
			Protected Friend Sub New(  provider As SelectorProvider)
				MyBase.New(provider)
			End Sub

			''' <summary>
			''' Returns an operation set identifying this channel's supported
			''' operations.
			''' 
			''' <p> Pipe-source channels only support reading, so this method
			''' returns <seealso cref="SelectionKey#OP_READ"/>.  </p>
			''' </summary>
			''' <returns>  The valid-operation set </returns>
			Public NotOverridable Overrides Function validOps() As Integer
				Return SelectionKey.OP_READ
			End Function

		End Class

		''' <summary>
		''' A channel representing the writable end of a <seealso cref="Pipe"/>.
		''' 
		''' @since 1.4
		''' </summary>
		Public MustInherit Class SinkChannel
			Inherits AbstractSelectableChannel
			Implements WritableByteChannel, GatheringByteChannel

				Public MustOverride Function write(  srcs As java.nio.ByteBuffer()) As Long Implements GatheringByteChannel.write
				Public MustOverride Function write(  srcs As java.nio.ByteBuffer(),   offset As Integer,   length As Integer) As Long Implements GatheringByteChannel.write
				Public MustOverride Function write(  src As java.nio.ByteBuffer) As Integer Implements WritableByteChannel.write
			''' <summary>
			''' Initializes a new instance of this class.
			''' </summary>
			''' <param name="provider">
			'''         The selector provider </param>
			Protected Friend Sub New(  provider As SelectorProvider)
				MyBase.New(provider)
			End Sub

			''' <summary>
			''' Returns an operation set identifying this channel's supported
			''' operations.
			''' 
			''' <p> Pipe-sink channels only support writing, so this method returns
			''' <seealso cref="SelectionKey#OP_WRITE"/>.  </p>
			''' </summary>
			''' <returns>  The valid-operation set </returns>
			Public NotOverridable Overrides Function validOps() As Integer
				Return SelectionKey.OP_WRITE
			End Function

		End Class

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns this pipe's source channel.
		''' </summary>
		''' <returns>  This pipe's source channel </returns>
		Public MustOverride Function source() As SourceChannel

		''' <summary>
		''' Returns this pipe's sink channel.
		''' </summary>
		''' <returns>  This pipe's sink channel </returns>
		Public MustOverride Function sink() As SinkChannel

		''' <summary>
		''' Opens a pipe.
		''' 
		''' <p> The new pipe is created by invoking the {@link
		''' java.nio.channels.spi.SelectorProvider#openPipe openPipe} method of the
		''' system-wide default <seealso cref="java.nio.channels.spi.SelectorProvider"/>
		''' object.  </p>
		''' </summary>
		''' <returns>  A new pipe
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public Shared Function open() As Pipe
			Return SelectorProvider.provider().openPipe()
		End Function

	End Class

End Namespace
'
' * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A byte channel that maintains a current <i>position</i> and allows the
	''' position to be changed.
	''' 
	''' <p> A seekable byte channel is connected to an entity, typically a file,
	''' that contains a variable-length sequence of bytes that can be read and
	''' written. The current position can be <seealso cref="#position() <i>queried</i>"/> and
	''' <seealso cref="#position(long) <i>modified</i>"/>. The channel also provides access to
	''' the current <i>size</i> of the entity to which the channel is connected. The
	''' size increases when bytes are written beyond its current size; the size
	''' decreases when it is <seealso cref="#truncate <i>truncated</i>"/>.
	''' 
	''' <p> The <seealso cref="#position(long) position"/> and <seealso cref="#truncate truncate"/> methods
	''' which do not otherwise have a value to return are specified to return the
	''' channel upon which they are invoked. This allows method invocations to be
	''' chained. Implementations of this interface should specialize the return type
	''' so that method invocations on the implementation class can be chained.
	''' 
	''' @since 1.7 </summary>
	''' <seealso cref= java.nio.file.Files#newByteChannel </seealso>

	Public Interface SeekableByteChannel
		Inherits ByteChannel

		''' <summary>
		''' Reads a sequence of bytes from this channel into the given buffer.
		''' 
		''' <p> Bytes are read starting at this channel's current position, and
		''' then the position is updated with the number of bytes actually read.
		''' Otherwise this method behaves exactly as specified in the {@link
		''' ReadableByteChannel} interface.
		''' </summary>
		Overrides Function read(ByVal dst As java.nio.ByteBuffer) As Integer

		''' <summary>
		''' Writes a sequence of bytes to this channel from the given buffer.
		''' 
		''' <p> Bytes are written starting at this channel's current position, unless
		''' the channel is connected to an entity such as a file that is opened with
		''' the <seealso cref="java.nio.file.StandardOpenOption#APPEND APPEND"/> option, in
		''' which case the position is first advanced to the end. The entity to which
		''' the channel is connected is grown, if necessary, to accommodate the
		''' written bytes, and then the position is updated with the number of bytes
		''' actually written. Otherwise this method behaves exactly as specified by
		''' the <seealso cref="WritableByteChannel"/> interface.
		''' </summary>
		Overrides Function write(ByVal src As java.nio.ByteBuffer) As Integer

		''' <summary>
		''' Returns this channel's position.
		''' </summary>
		''' <returns>  This channel's position,
		'''          a non-negative integer counting the number of bytes
		'''          from the beginning of the entity to the current position
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Function position() As Long

		''' <summary>
		''' Sets this channel's position.
		''' 
		''' <p> Setting the position to a value that is greater than the current size
		''' is legal but does not change the size of the entity.  A later attempt to
		''' read bytes at such a position will immediately return an end-of-file
		''' indication.  A later attempt to write bytes at such a position will cause
		''' the entity to grow to accommodate the new bytes; the values of any bytes
		''' between the previous end-of-file and the newly-written bytes are
		''' unspecified.
		''' 
		''' <p> Setting the channel's position is not recommended when connected to
		''' an entity, typically a file, that is opened with the {@link
		''' java.nio.file.StandardOpenOption#APPEND APPEND} option. When opened for
		''' append, the position is first advanced to the end before writing.
		''' </summary>
		''' <param name="newPosition">
		'''         The new position, a non-negative integer counting
		'''         the number of bytes from the beginning of the entity
		''' </param>
		''' <returns>  This channel
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the new position is negative </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Function position(ByVal newPosition As Long) As SeekableByteChannel

		''' <summary>
		''' Returns the current size of entity to which this channel is connected.
		''' </summary>
		''' <returns>  The current size, measured in bytes
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Function size() As Long

		''' <summary>
		''' Truncates the entity, to which this channel is connected, to the given
		''' size.
		''' 
		''' <p> If the given size is less than the current size then the entity is
		''' truncated, discarding any bytes beyond the new end. If the given size is
		''' greater than or equal to the current size then the entity is not modified.
		''' In either case, if the current position is greater than the given size
		''' then it is set to that size.
		''' 
		''' <p> An implementation of this interface may prohibit truncation when
		''' connected to an entity, typically a file, opened with the {@link
		''' java.nio.file.StandardOpenOption#APPEND APPEND} option.
		''' </summary>
		''' <param name="size">
		'''         The new size, a non-negative byte count
		''' </param>
		''' <returns>  This channel
		''' </returns>
		''' <exception cref="NonWritableChannelException">
		'''          If this channel was not opened for writing </exception>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the new size is negative </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Function truncate(ByVal size As Long) As SeekableByteChannel
	End Interface

End Namespace
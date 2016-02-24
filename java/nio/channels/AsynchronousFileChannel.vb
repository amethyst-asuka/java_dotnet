Imports System.Collections.Generic

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An asynchronous channel for reading, writing, and manipulating a file.
	''' 
	''' <p> An asynchronous file channel is created when a file is opened by invoking
	''' one of the <seealso cref="#open open"/> methods defined by this class. The file contains
	''' a variable-length sequence of bytes that can be read and written and whose
	''' current size can be <seealso cref="#size() queried"/>. The size of the file increases
	''' when bytes are written beyond its  current size; the size of the file decreases
	''' when it is <seealso cref="#truncate truncated"/>.
	''' 
	''' <p> An asynchronous file channel does not have a <i>current position</i>
	''' within the file. Instead, the file position is specified to each read and
	''' write method that initiates asynchronous operations. A <seealso cref="CompletionHandler"/>
	''' is specified as a parameter and is invoked to consume the result of the I/O
	''' operation. This class also defines read and write methods that initiate
	''' asynchronous operations, returning a <seealso cref="Future"/> to represent the pending
	''' result of the operation. The {@code Future} may be used to check if the
	''' operation has completed, wait for its completion, and retrieve the result.
	''' 
	''' <p> In addition to read and write operations, this class defines the
	''' following operations: </p>
	''' 
	''' <ul>
	''' 
	'''   <li><p> Updates made to a file may be {@link #force <i>forced
	'''   out</i>} to the underlying storage device, ensuring that data are not
	'''   lost in the event of a system crash.  </p></li>
	''' 
	'''   <li><p> A region of a file may be <seealso cref="#lock <i>locked</i>"/> against
	'''   access by other programs.  </p></li>
	''' 
	''' </ul>
	''' 
	''' <p> An {@code AsynchronousFileChannel} is associated with a thread pool to
	''' which tasks are submitted to handle I/O events and dispatch to completion
	''' handlers that consume the results of I/O operations on the channel. The
	''' completion handler for an I/O operation initiated on a channel is guaranteed
	''' to be invoked by one of the threads in the thread pool (This ensures that the
	''' completion handler is run by a thread with the expected <em>identity</em>).
	''' Where an I/O operation completes immediately, and the initiating thread is
	''' itself a thread in the thread pool, then the completion handler may be invoked
	''' directly by the initiating thread. When an {@code AsynchronousFileChannel} is
	''' created without specifying a thread pool then the channel is associated with
	''' a system-dependent default thread pool that may be shared with other
	''' channels. The default thread pool is configured by the system properties
	''' defined by the <seealso cref="AsynchronousChannelGroup"/> class.
	''' 
	''' <p> Channels of this type are safe for use by multiple concurrent threads. The
	''' <seealso cref="Channel#close close"/> method may be invoked at any time, as specified
	''' by the <seealso cref="Channel"/> interface. This causes all outstanding asynchronous
	''' operations on the channel to complete with the exception {@link
	''' AsynchronousCloseException}. Multiple read and write operations may be
	''' outstanding at the same time. When multiple read and write operations are
	''' outstanding then the ordering of the I/O operations, and the order that the
	''' completion handlers are invoked, is not specified; they are not, in particular,
	''' guaranteed to execute in the order that the operations were initiated. The
	''' <seealso cref="java.nio.ByteBuffer ByteBuffers"/> used when reading or writing are not
	''' safe for use by multiple concurrent I/O operations. Furthermore, after an I/O
	''' operation is initiated then care should be taken to ensure that the buffer is
	''' not accessed until after the operation has completed.
	''' 
	''' <p> As with <seealso cref="FileChannel"/>, the view of a file provided by an instance of
	''' this class is guaranteed to be consistent with other views of the same file
	''' provided by other instances in the same program.  The view provided by an
	''' instance of this class may or may not, however, be consistent with the views
	''' seen by other concurrently-running programs due to caching performed by the
	''' underlying operating system and delays induced by network-filesystem protocols.
	''' This is true regardless of the language in which these other programs are
	''' written, and whether they are running on the same machine or on some other
	''' machine.  The exact nature of any such inconsistencies are system-dependent
	''' and are therefore unspecified.
	''' 
	''' @since 1.7
	''' </summary>

	Public MustInherit Class AsynchronousFileChannel
		Implements AsynchronousChannel

			Public MustOverride ReadOnly Property open As Boolean Implements Channel.isOpen
			Public MustOverride Sub close() Implements AsynchronousChannel.close
		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Opens or creates a file for reading and/or writing, returning an
		''' asynchronous file channel to access the file.
		''' 
		''' <p> The {@code options} parameter determines how the file is opened.
		''' The <seealso cref="StandardOpenOption#READ READ"/> and {@link StandardOpenOption#WRITE
		''' WRITE} options determines if the file should be opened for reading and/or
		''' writing. If neither option is contained in the array then an existing file
		''' is opened for  reading.
		''' 
		''' <p> In addition to {@code READ} and {@code WRITE}, the following options
		''' may be present:
		''' 
		''' <table border=1 cellpadding=5 summary="">
		''' <tr> <th>Option</th> <th>Description</th> </tr>
		''' <tr>
		'''   <td> <seealso cref="StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING"/> </td>
		'''   <td> When opening an existing file, the file is first truncated to a
		'''   size of 0 bytes. This option is ignored when the file is opened only
		'''   for reading.</td>
		''' </tr>
		''' <tr>
		'''   <td> <seealso cref="StandardOpenOption#CREATE_NEW CREATE_NEW"/> </td>
		'''   <td> If this option is present then a new file is created, failing if
		'''   the file already exists. When creating a file the check for the
		'''   existence of the file and the creation of the file if it does not exist
		'''   is atomic with respect to other file system operations. This option is
		'''   ignored when the file is opened only for reading. </td>
		''' </tr>
		''' <tr>
		'''   <td > <seealso cref="StandardOpenOption#CREATE CREATE"/> </td>
		'''   <td> If this option is present then an existing file is opened if it
		'''   exists, otherwise a new file is created. When creating a file the check
		'''   for the existence of the file and the creation of the file if it does
		'''   not exist is atomic with respect to other file system operations. This
		'''   option is ignored if the {@code CREATE_NEW} option is also present or
		'''   the file is opened only for reading. </td>
		''' </tr>
		''' <tr>
		'''   <td > <seealso cref="StandardOpenOption#DELETE_ON_CLOSE DELETE_ON_CLOSE"/> </td>
		'''   <td> When this option is present then the implementation makes a
		'''   <em>best effort</em> attempt to delete the file when closed by the
		'''   the <seealso cref="#close close"/> method. If the {@code close} method is not
		'''   invoked then a <em>best effort</em> attempt is made to delete the file
		'''   when the Java virtual machine terminates. </td>
		''' </tr>
		''' <tr>
		'''   <td><seealso cref="StandardOpenOption#SPARSE SPARSE"/> </td>
		'''   <td> When creating a new file this option is a <em>hint</em> that the
		'''   new file will be sparse. This option is ignored when not creating
		'''   a new file. </td>
		''' </tr>
		''' <tr>
		'''   <td> <seealso cref="StandardOpenOption#SYNC SYNC"/> </td>
		'''   <td> Requires that every update to the file's content or metadata be
		'''   written synchronously to the underlying storage device. (see <a
		'''   href="../file/package-summary.html#integrity"> Synchronized I/O file
		'''   integrity</a>). </td>
		''' </tr>
		''' <tr>
		'''   <td> <seealso cref="StandardOpenOption#DSYNC DSYNC"/> </td>
		'''   <td> Requires that every update to the file's content be written
		'''   synchronously to the underlying storage device. (see <a
		'''   href="../file/package-summary.html#integrity"> Synchronized I/O file
		'''   integrity</a>). </td>
		''' </tr>
		''' </table>
		''' 
		''' <p> An implementation may also support additional options.
		''' 
		''' <p> The {@code executor} parameter is the <seealso cref="ExecutorService"/> to
		''' which tasks are submitted to handle I/O events and dispatch completion
		''' results for operations initiated on resulting channel.
		''' The nature of these tasks is highly implementation specific and so care
		''' should be taken when configuring the {@code Executor}. Minimally it
		''' should support an unbounded work queue and should not run tasks on the
		''' caller thread of the <seealso cref="ExecutorService#execute execute"/> method.
		''' Shutting down the executor service while the channel is open results in
		''' unspecified behavior.
		''' 
		''' <p> The {@code attrs} parameter is an optional array of file {@link
		''' FileAttribute file-attributes} to set atomically when creating the file.
		''' 
		''' <p> The new channel is created by invoking the {@link
		''' FileSystemProvider#newFileChannel newFileChannel} method on the
		''' provider that created the {@code Path}.
		''' </summary>
		''' <param name="file">
		'''          The path of the file to open or create </param>
		''' <param name="options">
		'''          Options specifying how the file is opened </param>
		''' <param name="executor">
		'''          The thread pool or {@code null} to associate the channel with
		'''          the default thread pool </param>
		''' <param name="attrs">
		'''          An optional list of file attributes to set atomically when
		'''          creating the file
		''' </param>
		''' <returns>  A new asynchronous file channel
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the set contains an invalid combination of options </exception>
		''' <exception cref="UnsupportedOperationException">
		'''          If the {@code file} is associated with a provider that does not
		'''          support creating asynchronous file channels, or an unsupported
		'''          open option is specified, or the array contains an attribute that
		'''          cannot be set atomically when creating the file </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is installed and it denies an
		'''          unspecified permission required by the implementation.
		'''          In the case of the default provider, the {@link
		'''          SecurityManager#checkRead(String)} method is invoked to check
		'''          read access if the file is opened for reading. The {@link
		'''          SecurityManager#checkWrite(String)} method is invoked to check
		'''          write access if the file is opened for writing </exception>
		Public Shared Function open(Of T1 As OpenOption, T2)(ByVal file As Path, ByVal options As java.util.Set(Of T1), ByVal executor As java.util.concurrent.ExecutorService, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T2)()) As AsynchronousFileChannel
			Dim provider As FileSystemProvider = file.fileSystem.provider()
			Return provider.newAsynchronousFileChannel(file, options, executor, attrs)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly NO_ATTRIBUTES As java.nio.file.attribute.FileAttribute(Of ?)() = New java.nio.file.attribute.FileAttribute(){} ' generic array construction

		''' <summary>
		''' Opens or creates a file for reading and/or writing, returning an
		''' asynchronous file channel to access the file.
		''' 
		''' <p> An invocation of this method behaves in exactly the same way as the
		''' invocation
		''' <pre>
		'''     ch.{@link #open(Path,Set,ExecutorService,FileAttribute[])
		'''       open}(file, opts, null, new FileAttribute&lt;?&gt;[0]);
		''' </pre>
		''' where {@code opts} is a {@code Set} containing the options specified to
		''' this method.
		''' 
		''' <p> The resulting channel is associated with default thread pool to which
		''' tasks are submitted to handle I/O events and dispatch to completion
		''' handlers that consume the result of asynchronous operations performed on
		''' the resulting channel.
		''' </summary>
		''' <param name="file">
		'''          The path of the file to open or create </param>
		''' <param name="options">
		'''          Options specifying how the file is opened
		''' </param>
		''' <returns>  A new asynchronous file channel
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the set contains an invalid combination of options </exception>
		''' <exception cref="UnsupportedOperationException">
		'''          If the {@code file} is associated with a provider that does not
		'''          support creating file channels, or an unsupported open option is
		'''          specified </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is installed and it denies an
		'''          unspecified permission required by the implementation.
		'''          In the case of the default provider, the {@link
		'''          SecurityManager#checkRead(String)} method is invoked to check
		'''          read access if the file is opened for reading. The {@link
		'''          SecurityManager#checkWrite(String)} method is invoked to check
		'''          write access if the file is opened for writing </exception>
		Public Shared Function open(ByVal file As Path, ParamArray ByVal options As OpenOption()) As AsynchronousFileChannel
			Dim [set] As java.util.Set(Of OpenOption) = New HashSet(Of OpenOption)(options.Length)
			java.util.Collections.addAll([set], options)
			Return open(file, [set], Nothing, NO_ATTRIBUTES)
		End Function

		''' <summary>
		''' Returns the current size of this channel's file.
		''' </summary>
		''' <returns>  The current size of this channel's file, measured in bytes
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Public MustOverride Function size() As Long

		''' <summary>
		''' Truncates this channel's file to the given size.
		''' 
		''' <p> If the given size is less than the file's current size then the file
		''' is truncated, discarding any bytes beyond the new end of the file.  If
		''' the given size is greater than or equal to the file's current size then
		''' the file is not modified. </p>
		''' </summary>
		''' <param name="size">
		'''         The new size, a non-negative byte count
		''' </param>
		''' <returns>  This file channel
		''' </returns>
		''' <exception cref="NonWritableChannelException">
		'''          If this channel was not opened for writing
		''' </exception>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the new size is negative
		''' </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Public MustOverride Function truncate(ByVal size As Long) As AsynchronousFileChannel

		''' <summary>
		''' Forces any updates to this channel's file to be written to the storage
		''' device that contains it.
		''' 
		''' <p> If this channel's file resides on a local storage device then when
		''' this method returns it is guaranteed that all changes made to the file
		''' since this channel was created, or since this method was last invoked,
		''' will have been written to that device.  This is useful for ensuring that
		''' critical information is not lost in the event of a system crash.
		''' 
		''' <p> If the file does not reside on a local device then no such guarantee
		''' is made.
		''' 
		''' <p> The {@code metaData} parameter can be used to limit the number of
		''' I/O operations that this method is required to perform.  Passing
		''' {@code false} for this parameter indicates that only updates to the
		''' file's content need be written to storage; passing {@code true}
		''' indicates that updates to both the file's content and metadata must be
		''' written, which generally requires at least one more I/O operation.
		''' Whether this parameter actually has any effect is dependent upon the
		''' underlying operating system and is therefore unspecified.
		''' 
		''' <p> Invoking this method may cause an I/O operation to occur even if the
		''' channel was only opened for reading.  Some operating systems, for
		''' example, maintain a last-access time as part of a file's metadata, and
		''' this time is updated whenever the file is read.  Whether or not this is
		''' actually done is system-dependent and is therefore unspecified.
		''' 
		''' <p> This method is only guaranteed to force changes that were made to
		''' this channel's file via the methods defined in this class.
		''' </summary>
		''' <param name="metaData">
		'''          If {@code true} then this method is required to force changes
		'''          to both the file's content and metadata to be written to
		'''          storage; otherwise, it need only force content changes to be
		'''          written
		''' </param>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed
		''' </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Public MustOverride Sub force(ByVal metaData As Boolean)

		''' <summary>
		''' Acquires a lock on the given region of this channel's file.
		''' 
		''' <p> This method initiates an operation to acquire a lock on the given
		''' region of this channel's file. The {@code handler} parameter is a
		''' completion handler that is invoked when the lock is acquired (or the
		''' operation fails). The result passed to the completion handler is the
		''' resulting {@code FileLock}.
		''' 
		''' <p> The region specified by the {@code position} and {@code size}
		''' parameters need not be contained within, or even overlap, the actual
		''' underlying file.  Lock regions are fixed in size; if a locked region
		''' initially contains the end of the file and the file grows beyond the
		''' region then the new portion of the file will not be covered by the lock.
		''' If a file is expected to grow in size and a lock on the entire file is
		''' required then a region starting at zero, and no smaller than the
		''' expected maximum size of the file, should be locked.  The two-argument
		''' <seealso cref="#lock(Object,CompletionHandler)"/> method simply locks a region
		''' of size <seealso cref="Long#MAX_VALUE"/>. If a lock that overlaps the requested
		''' region is already held by this Java virtual machine, or this method has
		''' been invoked to lock an overlapping region and that operation has not
		''' completed, then this method throws <seealso cref="OverlappingFileLockException"/>.
		''' 
		''' <p> Some operating systems do not support a mechanism to acquire a file
		''' lock in an asynchronous manner. Consequently an implementation may
		''' acquire the file lock in a background thread or from a task executed by
		''' a thread in the associated thread pool. If there are many lock operations
		''' outstanding then it may consume threads in the Java virtual machine for
		''' indefinite periods.
		''' 
		''' <p> Some operating systems do not support shared locks, in which case a
		''' request for a shared lock is automatically converted into a request for
		''' an exclusive lock.  Whether the newly-acquired lock is shared or
		''' exclusive may be tested by invoking the resulting lock object's {@link
		''' FileLock#isShared() isShared} method.
		''' 
		''' <p> File locks are held on behalf of the entire Java virtual machine.
		''' They are not suitable for controlling access to a file by multiple
		''' threads within the same virtual machine.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="position">
		'''          The position at which the locked region is to start; must be
		'''          non-negative </param>
		''' <param name="size">
		'''          The size of the locked region; must be non-negative, and the sum
		'''          {@code position}&nbsp;+&nbsp;{@code size} must be non-negative </param>
		''' <param name="shared">
		'''          {@code true} to request a shared lock, in which case this
		'''          channel must be open for reading (and possibly writing);
		'''          {@code false} to request an exclusive lock, in which case this
		'''          channel must be open for writing (and possibly reading) </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="OverlappingFileLockException">
		'''          If a lock that overlaps the requested region is already held by
		'''          this Java virtual machine, or there is already a pending attempt
		'''          to lock an overlapping region </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		''' <exception cref="NonReadableChannelException">
		'''          If {@code shared} is true but this channel was not opened for reading </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If {@code shared} is false but this channel was not opened for writing </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub lock(Of A, T1)(ByVal position As Long, ByVal size As Long, ByVal [shared] As Boolean, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <summary>
		''' Acquires an exclusive lock on this channel's file.
		''' 
		''' <p> This method initiates an operation to acquire a lock on the given
		''' region of this channel's file. The {@code handler} parameter is a
		''' completion handler that is invoked when the lock is acquired (or the
		''' operation fails). The result passed to the completion handler is the
		''' resulting {@code FileLock}.
		''' 
		''' <p> An invocation of this method of the form {@code ch.lock(att,handler)}
		''' behaves in exactly the same way as the invocation
		''' <pre>
		'''     ch.<seealso cref="#lock(long,long,boolean,Object,CompletionHandler) lock"/>(0L, Long.MAX_VALUE, false, att, handler)
		''' </pre>
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="OverlappingFileLockException">
		'''          If a lock is already held by this Java virtual machine, or there
		'''          is already a pending attempt to lock a region </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If this channel was not opened for writing </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub lock(Of A, T1)(ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))
			lock(0L, Long.MaxValue, False, attachment, handler)
		End Sub

		''' <summary>
		''' Acquires a lock on the given region of this channel's file.
		''' 
		''' <p> This method initiates an operation to acquire a lock on the given
		''' region of this channel's file.  The method behaves in exactly the same
		''' manner as the <seealso cref="#lock(long, long, boolean, Object, CompletionHandler)"/>
		''' method except that instead of specifying a completion handler, this
		''' method returns a {@code Future} representing the pending result. The
		''' {@code Future}'s <seealso cref="Future#get() get"/> method returns the {@link
		''' FileLock} on successful completion.
		''' </summary>
		''' <param name="position">
		'''          The position at which the locked region is to start; must be
		'''          non-negative </param>
		''' <param name="size">
		'''          The size of the locked region; must be non-negative, and the sum
		'''          {@code position}&nbsp;+&nbsp;{@code size} must be non-negative </param>
		''' <param name="shared">
		'''          {@code true} to request a shared lock, in which case this
		'''          channel must be open for reading (and possibly writing);
		'''          {@code false} to request an exclusive lock, in which case this
		'''          channel must be open for writing (and possibly reading)
		''' </param>
		''' <returns>  a {@code Future} object representing the pending result
		''' </returns>
		''' <exception cref="OverlappingFileLockException">
		'''          If a lock is already held by this Java virtual machine, or there
		'''          is already a pending attempt to lock a region </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		''' <exception cref="NonReadableChannelException">
		'''          If {@code shared} is true but this channel was not opened for reading </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If {@code shared} is false but this channel was not opened for writing </exception>
		Public MustOverride Function lock(ByVal position As Long, ByVal size As Long, ByVal [shared] As Boolean) As java.util.concurrent.Future(Of FileLock)

		''' <summary>
		''' Acquires an exclusive lock on this channel's file.
		''' 
		''' <p> This method initiates an operation to acquire an exclusive lock on this
		''' channel's file. The method returns a {@code Future} representing the
		''' pending result of the operation. The {@code Future}'s {@link Future#get()
		''' get} method returns the <seealso cref="FileLock"/> on successful completion.
		''' 
		''' <p> An invocation of this method behaves in exactly the same way as the
		''' invocation
		''' <pre>
		'''     ch.<seealso cref="#lock(long,long,boolean) lock"/>(0L, Long.MAX_VALUE, false)
		''' </pre>
		''' </summary>
		''' <returns>  a {@code Future} object representing the pending result
		''' </returns>
		''' <exception cref="OverlappingFileLockException">
		'''          If a lock is already held by this Java virtual machine, or there
		'''          is already a pending attempt to lock a region </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If this channel was not opened for writing </exception>
		Public Function lock() As java.util.concurrent.Future(Of FileLock)
			Return lock(0L, Long.MaxValue, False)
		End Function

		''' <summary>
		''' Attempts to acquire a lock on the given region of this channel's file.
		''' 
		''' <p> This method does not block. An invocation always returns immediately,
		''' either having acquired a lock on the requested region or having failed to
		''' do so.  If it fails to acquire a lock because an overlapping lock is held
		''' by another program then it returns {@code null}.  If it fails to acquire
		''' a lock for any other reason then an appropriate exception is thrown.
		''' </summary>
		''' <param name="position">
		'''         The position at which the locked region is to start; must be
		'''         non-negative
		''' </param>
		''' <param name="size">
		'''         The size of the locked region; must be non-negative, and the sum
		'''         {@code position}&nbsp;+&nbsp;{@code size} must be non-negative
		''' </param>
		''' <param name="shared">
		'''         {@code true} to request a shared lock,
		'''         {@code false} to request an exclusive lock
		''' </param>
		''' <returns>  A lock object representing the newly-acquired lock,
		'''          or {@code null} if the lock could not be acquired
		'''          because another program holds an overlapping lock
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="OverlappingFileLockException">
		'''          If a lock that overlaps the requested region is already held by
		'''          this Java virtual machine, or if another thread is already
		'''          blocked in this method and is attempting to lock an overlapping
		'''          region of the same file </exception>
		''' <exception cref="NonReadableChannelException">
		'''          If {@code shared} is true but this channel was not opened for reading </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If {@code shared} is false but this channel was not opened for writing
		''' </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs
		''' </exception>
		''' <seealso cref=     #lock(Object,CompletionHandler) </seealso>
		''' <seealso cref=     #lock(long,long,boolean,Object,CompletionHandler) </seealso>
		''' <seealso cref=     #tryLock() </seealso>
		Public MustOverride Function tryLock(ByVal position As Long, ByVal size As Long, ByVal [shared] As Boolean) As FileLock

		''' <summary>
		''' Attempts to acquire an exclusive lock on this channel's file.
		''' 
		''' <p> An invocation of this method of the form {@code ch.tryLock()}
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     ch.<seealso cref="#tryLock(long,long,boolean) tryLock"/>(0L, Long.MAX_VALUE, false) </pre>
		''' </summary>
		''' <returns>  A lock object representing the newly-acquired lock,
		'''          or {@code null} if the lock could not be acquired
		'''          because another program holds an overlapping lock
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="OverlappingFileLockException">
		'''          If a lock that overlaps the requested region is already held by
		'''          this Java virtual machine, or if another thread is already
		'''          blocked in this method and is attempting to lock an overlapping
		'''          region </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If {@code shared} is false but this channel was not opened for writing
		''' </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs
		''' </exception>
		''' <seealso cref=     #lock(Object,CompletionHandler) </seealso>
		''' <seealso cref=     #lock(long,long,boolean,Object,CompletionHandler) </seealso>
		''' <seealso cref=     #tryLock(long,long,boolean) </seealso>
		Public Function tryLock() As FileLock
			Return tryLock(0L, Long.MaxValue, False)
		End Function

		''' <summary>
		''' Reads a sequence of bytes from this channel into the given buffer,
		''' starting at the given file position.
		''' 
		''' <p> This method initiates the reading of a sequence of bytes from this
		''' channel into the given buffer, starting at the given file position. The
		''' result of the read is the number of bytes read or {@code -1} if the given
		''' position is greater than or equal to the file's size at the time that the
		''' read is attempted.
		''' 
		''' <p> This method works in the same manner as the {@link
		''' AsynchronousByteChannel#read(ByteBuffer,Object,CompletionHandler)}
		''' method, except that bytes are read starting at the given file position.
		''' If the given file position is greater than the file's size at the time
		''' that the read is attempted then no bytes are read.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="dst">
		'''          The buffer into which bytes are to be transferred </param>
		''' <param name="position">
		'''          The file position at which the transfer is to begin;
		'''          must be non-negative </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the position is negative or the buffer is read-only </exception>
		''' <exception cref="NonReadableChannelException">
		'''          If this channel was not opened for reading </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub read(Of A, T1)(ByVal dst As java.nio.ByteBuffer, ByVal position As Long, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <summary>
		''' Reads a sequence of bytes from this channel into the given buffer,
		''' starting at the given file position.
		''' 
		''' <p> This method initiates the reading of a sequence of bytes from this
		''' channel into the given buffer, starting at the given file position. This
		''' method returns a {@code Future} representing the pending result of the
		''' operation. The {@code Future}'s <seealso cref="Future#get() get"/> method returns
		''' the number of bytes read or {@code -1} if the given position is greater
		''' than or equal to the file's size at the time that the read is attempted.
		''' 
		''' <p> This method works in the same manner as the {@link
		''' AsynchronousByteChannel#read(ByteBuffer)} method, except that bytes are
		''' read starting at the given file position. If the given file position is
		''' greater than the file's size at the time that the read is attempted then
		''' no bytes are read.
		''' </summary>
		''' <param name="dst">
		'''          The buffer into which bytes are to be transferred </param>
		''' <param name="position">
		'''          The file position at which the transfer is to begin;
		'''          must be non-negative
		''' </param>
		''' <returns>  A {@code Future} object representing the pending result
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the position is negative or the buffer is read-only </exception>
		''' <exception cref="NonReadableChannelException">
		'''          If this channel was not opened for reading </exception>
		Public MustOverride Function read(ByVal dst As java.nio.ByteBuffer, ByVal position As Long) As java.util.concurrent.Future(Of Integer?)

		''' <summary>
		''' Writes a sequence of bytes to this channel from the given buffer, starting
		''' at the given file position.
		''' 
		''' <p> This method works in the same manner as the {@link
		''' AsynchronousByteChannel#write(ByteBuffer,Object,CompletionHandler)}
		''' method, except that bytes are written starting at the given file position.
		''' If the given position is greater than the file's size, at the time that
		''' the write is attempted, then the file will be grown to accommodate the new
		''' bytes; the values of any bytes between the previous end-of-file and the
		''' newly-written bytes are unspecified.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="src">
		'''          The buffer from which bytes are to be transferred </param>
		''' <param name="position">
		'''          The file position at which the transfer is to begin;
		'''          must be non-negative </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the position is negative </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If this channel was not opened for writing </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub write(Of A, T1)(ByVal src As java.nio.ByteBuffer, ByVal position As Long, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <summary>
		''' Writes a sequence of bytes to this channel from the given buffer, starting
		''' at the given file position.
		''' 
		''' <p> This method initiates the writing of a sequence of bytes to this
		''' channel from the given buffer, starting at the given file position. The
		''' method returns a {@code Future} representing the pending result of the
		''' write operation. The {@code Future}'s <seealso cref="Future#get() get"/> method
		''' returns the number of bytes written.
		''' 
		''' <p> This method works in the same manner as the {@link
		''' AsynchronousByteChannel#write(ByteBuffer)} method, except that bytes are
		''' written starting at the given file position. If the given position is
		''' greater than the file's size, at the time that the write is attempted,
		''' then the file will be grown to accommodate the new bytes; the values of
		''' any bytes between the previous end-of-file and the newly-written bytes
		''' are unspecified.
		''' </summary>
		''' <param name="src">
		'''          The buffer from which bytes are to be transferred </param>
		''' <param name="position">
		'''          The file position at which the transfer is to begin;
		'''          must be non-negative
		''' </param>
		''' <returns>  A {@code Future} object representing the pending result
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the position is negative </exception>
		''' <exception cref="NonWritableChannelException">
		'''          If this channel was not opened for writing </exception>
		Public MustOverride Function write(ByVal src As java.nio.ByteBuffer, ByVal position As Long) As java.util.concurrent.Future(Of Integer?)
	End Class

End Namespace
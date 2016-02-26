'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A token representing a lock on a region of a file.
	''' 
	''' <p> A file-lock object is created each time a lock is acquired on a file via
	''' one of the <seealso cref="FileChannel#lock(long,long,boolean) lock"/> or {@link
	''' FileChannel#tryLock(long,long,boolean) tryLock} methods of the
	''' <seealso cref="FileChannel"/> [Class], or the {@link
	''' AsynchronousFileChannel#lock(long,long,boolean,Object,CompletionHandler) lock}
	''' or <seealso cref="AsynchronousFileChannel#tryLock(long,long,boolean) tryLock"/>
	''' methods of the <seealso cref="AsynchronousFileChannel"/> class.
	''' 
	''' <p> A file-lock object is initially valid.  It remains valid until the lock
	''' is released by invoking the <seealso cref="#release release"/> method, by closing the
	''' channel that was used to acquire it, or by the termination of the Java
	''' virtual machine, whichever comes first.  The validity of a lock may be
	''' tested by invoking its <seealso cref="#isValid isValid"/> method.
	''' 
	''' <p> A file lock is either <i>exclusive</i> or <i>shared</i>.  A shared lock
	''' prevents other concurrently-running programs from acquiring an overlapping
	''' exclusive lock, but does allow them to acquire overlapping shared locks.  An
	''' exclusive lock prevents other programs from acquiring an overlapping lock of
	''' either type.  Once it is released, a lock has no further effect on the locks
	''' that may be acquired by other programs.
	''' 
	''' <p> Whether a lock is exclusive or shared may be determined by invoking its
	''' <seealso cref="#isShared isShared"/> method.  Some platforms do not support shared
	''' locks, in which case a request for a shared lock is automatically converted
	''' into a request for an exclusive lock.
	''' 
	''' <p> The locks held on a particular file by a single Java virtual machine do
	''' not overlap.  The <seealso cref="#overlaps overlaps"/> method may be used to test
	''' whether a candidate lock range overlaps an existing lock.
	''' 
	''' <p> A file-lock object records the file channel upon whose file the lock is
	''' held, the type and validity of the lock, and the position and size of the
	''' locked region.  Only the validity of a lock is subject to change over time;
	''' all other aspects of a lock's state are immutable.
	''' 
	''' <p> File locks are held on behalf of the entire Java virtual machine.
	''' They are not suitable for controlling access to a file by multiple
	''' threads within the same virtual machine.
	''' 
	''' <p> File-lock objects are safe for use by multiple concurrent threads.
	''' 
	''' 
	''' <a name="pdep"></a><h2> Platform dependencies </h2>
	''' 
	''' <p> This file-locking API is intended to map directly to the native locking
	''' facility of the underlying operating system.  Thus the locks held on a file
	''' should be visible to all programs that have access to the file, regardless
	''' of the language in which those programs are written.
	''' 
	''' <p> Whether or not a lock actually prevents another program from accessing
	''' the content of the locked region is system-dependent and therefore
	''' unspecified.  The native file-locking facilities of some systems are merely
	''' <i>advisory</i>, meaning that programs must cooperatively observe a known
	''' locking protocol in order to guarantee data integrity.  On other systems
	''' native file locks are <i>mandatory</i>, meaning that if one program locks a
	''' region of a file then other programs are actually prevented from accessing
	''' that region in a way that would violate the lock.  On yet other systems,
	''' whether native file locks are advisory or mandatory is configurable on a
	''' per-file basis.  To ensure consistent and correct behavior across platforms,
	''' it is strongly recommended that the locks provided by this API be used as if
	''' they were advisory locks.
	''' 
	''' <p> On some systems, acquiring a mandatory lock on a region of a file
	''' prevents that region from being {@link java.nio.channels.FileChannel#map
	''' <i>mapped into memory</i>}, and vice versa.  Programs that combine
	''' locking and mapping should be prepared for this combination to fail.
	''' 
	''' <p> On some systems, closing a channel releases all locks held by the Java
	''' virtual machine on the underlying file regardless of whether the locks were
	''' acquired via that channel or via another channel open on the same file.  It
	''' is strongly recommended that, within a program, a unique channel be used to
	''' acquire all locks on any given file.
	''' 
	''' <p> Some network filesystems permit file locking to be used with
	''' memory-mapped files only when the locked regions are page-aligned and a
	''' whole multiple of the underlying hardware's page size.  Some network
	''' filesystems do not implement file locks on regions that extend past a
	''' certain position, often 2<sup>30</sup> or 2<sup>31</sup>.  In general, great
	''' care should be taken when locking files that reside on network filesystems.
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class FileLock
		Implements AutoCloseable

		Private ReadOnly channel_Renamed As Channel
		Private ReadOnly position_Renamed As Long
		Private ReadOnly size_Renamed As Long
		Private ReadOnly [shared] As Boolean

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <param name="channel">
		'''         The file channel upon whose file this lock is held
		''' </param>
		''' <param name="position">
		'''         The position within the file at which the locked region starts;
		'''         must be non-negative
		''' </param>
		''' <param name="size">
		'''         The size of the locked region; must be non-negative, and the sum
		'''         <tt>position</tt>&nbsp;+&nbsp;<tt>size</tt> must be non-negative
		''' </param>
		''' <param name="shared">
		'''         <tt>true</tt> if this lock is shared,
		'''         <tt>false</tt> if it is exclusive
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''         If the preconditions on the parameters do not hold </exception>
		Protected Friend Sub New(ByVal channel As FileChannel, ByVal position As Long, ByVal size As Long, ByVal [shared] As Boolean)
			If position < 0 Then Throw New IllegalArgumentException("Negative position")
			If size < 0 Then Throw New IllegalArgumentException("Negative size")
			If position + size < 0 Then Throw New IllegalArgumentException("Negative position + size")
			Me.channel_Renamed = channel
			Me.position_Renamed = position
			Me.size_Renamed = size
			Me.shared = [shared]
		End Sub

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <param name="channel">
		'''         The channel upon whose file this lock is held
		''' </param>
		''' <param name="position">
		'''         The position within the file at which the locked region starts;
		'''         must be non-negative
		''' </param>
		''' <param name="size">
		'''         The size of the locked region; must be non-negative, and the sum
		'''         <tt>position</tt>&nbsp;+&nbsp;<tt>size</tt> must be non-negative
		''' </param>
		''' <param name="shared">
		'''         <tt>true</tt> if this lock is shared,
		'''         <tt>false</tt> if it is exclusive
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''         If the preconditions on the parameters do not hold
		''' 
		''' @since 1.7 </exception>
		Protected Friend Sub New(ByVal channel As AsynchronousFileChannel, ByVal position As Long, ByVal size As Long, ByVal [shared] As Boolean)
			If position < 0 Then Throw New IllegalArgumentException("Negative position")
			If size < 0 Then Throw New IllegalArgumentException("Negative size")
			If position + size < 0 Then Throw New IllegalArgumentException("Negative position + size")
			Me.channel_Renamed = channel
			Me.position_Renamed = position
			Me.size_Renamed = size
			Me.shared = [shared]
		End Sub

		''' <summary>
		''' Returns the file channel upon whose file this lock was acquired.
		''' 
		''' <p> This method has been superseded by the <seealso cref="#acquiredBy acquiredBy"/>
		''' method.
		''' </summary>
		''' <returns>  The file channel, or {@code null} if the file lock was not
		'''          acquired by a file channel. </returns>
		Public Function channel() As FileChannel
			Return If(TypeOf channel_Renamed Is FileChannel, CType(channel_Renamed, FileChannel), Nothing)
		End Function

		''' <summary>
		''' Returns the channel upon whose file this lock was acquired.
		''' </summary>
		''' <returns>  The channel upon whose file this lock was acquired.
		''' 
		''' @since 1.7 </returns>
		Public Overridable Function acquiredBy() As Channel
			Return channel_Renamed
		End Function

		''' <summary>
		''' Returns the position within the file of the first byte of the locked
		''' region.
		''' 
		''' <p> A locked region need not be contained within, or even overlap, the
		''' actual underlying file, so the value returned by this method may exceed
		''' the file's current size.  </p>
		''' </summary>
		''' <returns>  The position </returns>
		Public Function position() As Long
			Return position_Renamed
		End Function

		''' <summary>
		''' Returns the size of the locked region in bytes.
		''' 
		''' <p> A locked region need not be contained within, or even overlap, the
		''' actual underlying file, so the value returned by this method may exceed
		''' the file's current size.  </p>
		''' </summary>
		''' <returns>  The size of the locked region </returns>
		Public Function size() As Long
			Return size_Renamed
		End Function

		''' <summary>
		''' Tells whether this lock is shared.
		''' </summary>
		''' <returns> <tt>true</tt> if lock is shared,
		'''         <tt>false</tt> if it is exclusive </returns>
		Public Property [shared] As Boolean
			Get
				Return [shared]
			End Get
		End Property

		''' <summary>
		''' Tells whether or not this lock overlaps the given lock range.
		''' </summary>
		''' <param name="position">
		'''          The starting position of the lock range </param>
		''' <param name="size">
		'''          The size of the lock range
		''' </param>
		''' <returns>  <tt>true</tt> if, and only if, this lock and the given lock
		'''          range overlap by at least one byte </returns>
		Public Function overlaps(ByVal position As Long, ByVal size As Long) As Boolean
			If position + size <= Me.position_Renamed Then Return False ' That is below this
			If Me.position_Renamed + Me.size_Renamed <= position Then Return False ' This is below that
			Return True
		End Function

		''' <summary>
		''' Tells whether or not this lock is valid.
		''' 
		''' <p> A lock object remains valid until it is released or the associated
		''' file channel is closed, whichever comes first.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this lock is valid </returns>
		Public MustOverride ReadOnly Property valid As Boolean

		''' <summary>
		''' Releases this lock.
		''' 
		''' <p> If this lock object is valid then invoking this method releases the
		''' lock and renders the object invalid.  If this lock object is invalid
		''' then invoking this method has no effect.  </p>
		''' </summary>
		''' <exception cref="ClosedChannelException">
		'''          If the channel that was used to acquire this lock
		'''          is no longer open
		''' </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Sub release()

		''' <summary>
		''' This method invokes the <seealso cref="#release"/> method. It was added
		''' to the class so that it could be used in conjunction with the
		''' automatic resource management block construct.
		''' 
		''' @since 1.7
		''' </summary>
		Public Sub close() Implements AutoCloseable.close
			release()
		End Sub

		''' <summary>
		''' Returns a string describing the range, type, and validity of this lock.
		''' </summary>
		''' <returns>  A descriptive string </returns>
		Public NotOverridable Overrides Function ToString() As String
			Return (Me.GetType().name & "[" & position_Renamed & ":" & size_Renamed & " " & (If([shared], "shared", "exclusive")) & " " & (If(valid, "valid", "invalid")) & "]")
		End Function

	End Class

End Namespace
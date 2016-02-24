Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io


	''' <summary>
	''' Instances of the file descriptor class serve as an opaque handle
	''' to the underlying machine-specific structure representing an
	''' open file, an open socket, or another source or sink of bytes.
	''' The main practical use for a file descriptor is to create a
	''' <seealso cref="FileInputStream"/> or <seealso cref="FileOutputStream"/> to contain it.
	''' 
	''' <p>Applications should not create their own file descriptors.
	''' 
	''' @author  Pavani Diwanji
	''' @since   JDK1.0
	''' </summary>
	Public NotInheritable Class FileDescriptor

		Private fd As Integer

		Private handle As Long

		Private parent As Closeable
		Private otherParents As IList(Of Closeable)
		Private closed As Boolean

		''' <summary>
		''' Constructs an (invalid) FileDescriptor
		''' object.
		''' </summary>
		Public Sub New()
			fd = -1
			handle = -1
		End Sub

		Shared Sub New()
			initIDs()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaIOFileDescriptorAccess(New sun.misc.JavaIOFileDescriptorAccess()
	'		{
	'				public void set(FileDescriptor obj, int fd)
	'				{
	'					obj.fd = fd;
	'				}
	'
	'				public int get(FileDescriptor obj)
	'				{
	'					Return obj.fd;
	'				}
	'
	'				public void setHandle(FileDescriptor obj, long handle)
	'				{
	'					obj.handle = handle;
	'				}
	'
	'				public long getHandle(FileDescriptor obj)
	'				{
	'					Return obj.handle;
	'				}
	'			}
		   )
		End Sub

		' Set up JavaIOFileDescriptorAccess in SharedSecrets

		''' <summary>
		''' A handle to the standard input stream. Usually, this file
		''' descriptor is not used directly, but rather via the input stream
		''' known as {@code System.in}.
		''' </summary>
		''' <seealso cref=     java.lang.System#in </seealso>
		Public Shared ReadOnly [in] As FileDescriptor = standardStream(0)

		''' <summary>
		''' A handle to the standard output stream. Usually, this file
		''' descriptor is not used directly, but rather via the output stream
		''' known as {@code System.out}. </summary>
		''' <seealso cref=     java.lang.System#out </seealso>
		Public Shared ReadOnly out As FileDescriptor = standardStream(1)

		''' <summary>
		''' A handle to the standard error stream. Usually, this file
		''' descriptor is not used directly, but rather via the output stream
		''' known as {@code System.err}.
		''' </summary>
		''' <seealso cref=     java.lang.System#err </seealso>
		Public Shared ReadOnly err As FileDescriptor = standardStream(2)

		''' <summary>
		''' Tests if this file descriptor object is valid.
		''' </summary>
		''' <returns>  {@code true} if the file descriptor object represents a
		'''          valid, open file, socket, or other active I/O connection;
		'''          {@code false} otherwise. </returns>
		Public Function valid() As Boolean
			Return ((handle <> -1) OrElse (fd <> -1))
		End Function

		''' <summary>
		''' Force all system buffers to synchronize with the underlying
		''' device.  This method returns after all modified data and
		''' attributes of this FileDescriptor have been written to the
		''' relevant device(s).  In particular, if this FileDescriptor
		''' refers to a physical storage medium, such as a file in a file
		''' system, sync will not return until all in-memory modified copies
		''' of buffers associated with this FileDesecriptor have been
		''' written to the physical medium.
		''' 
		''' sync is meant to be used by code that requires physical
		''' storage (such as a file) to be in a known state  For
		''' example, a class that provided a simple transaction facility
		''' might use sync to ensure that all changes to a file caused
		''' by a given transaction were recorded on a storage medium.
		''' 
		''' sync only affects buffers downstream of this FileDescriptor.  If
		''' any in-memory buffering is being done by the application (for
		''' example, by a BufferedOutputStream object), those buffers must
		''' be flushed into the FileDescriptor (for example, by invoking
		''' OutputStream.flush) before that data will be affected by sync.
		''' </summary>
		''' <exception cref="SyncFailedException">
		'''        Thrown when the buffers cannot be flushed,
		'''        or because the system cannot guarantee that all the
		'''        buffers have been synchronized with physical media.
		''' @since     JDK1.1 </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Sub sync()
		End Sub

		' This routine initializes JNI field offsets for the class 
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function [set](ByVal d As Integer) As Long
		End Function

		Private Shared Function standardStream(ByVal fd As Integer) As FileDescriptor
			Dim desc As New FileDescriptor
			desc.handle = set(fd)
			Return desc
		End Function

	'    
	'     * Package private methods to track referents.
	'     * If multiple streams point to the same FileDescriptor, we cycle
	'     * through the list of all referents and call close()
	'     

		''' <summary>
		''' Attach a Closeable to this FD for tracking.
		''' parent reference is added to otherParents when
		''' needed to make closeAll simpler.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Sub attach(ByVal c As Closeable)
			If parent Is Nothing Then
				' first caller gets to do this
				parent = c
			ElseIf otherParents Is Nothing Then
				otherParents = New List(Of )
				otherParents.Add(parent)
				otherParents.Add(c)
			Else
				otherParents.Add(c)
			End If
		End Sub

		''' <summary>
		''' Cycle through all Closeables sharing this FD and call
		''' close() on each one.
		''' 
		''' The caller closeable gets to call close0().
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Sub closeAll(ByVal releaser As Closeable)
			If Not closed Then
				closed = True
				Dim ioe As IOException = Nothing
				Using c As Closeable = releaser
						Try
						If otherParents IsNot Nothing Then
							For Each referent As Closeable In otherParents
								Try
									referent.close()
								Catch x As IOException
									If ioe Is Nothing Then
										ioe = x
									Else
										ioe.addSuppressed(x)
									End If
								End Try
							Next referent
						End If
					Catch ex As IOException
		'                
		'                 * If releaser close() throws IOException
		'                 * add other exceptions as suppressed.
		'                 
						If ioe IsNot Nothing Then ex.addSuppressed(ioe)
						ioe = ex
					Finally
						If ioe IsNot Nothing Then Throw ioe
					End Try
				End Using
			End If
		End Sub
	End Class

End Namespace
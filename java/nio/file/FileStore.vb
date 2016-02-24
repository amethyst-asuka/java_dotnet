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

Namespace java.nio.file


	''' <summary>
	''' Storage for files. A {@code FileStore} represents a storage pool, device,
	''' partition, volume, concrete file system or other implementation specific means
	''' of file storage. The {@code FileStore} for where a file is stored is obtained
	''' by invoking the <seealso cref="Files#getFileStore getFileStore"/> method, or all file
	''' stores can be enumerated by invoking the {@link FileSystem#getFileStores
	''' getFileStores} method.
	''' 
	''' <p> In addition to the methods defined by this class, a file store may support
	''' one or more <seealso cref="FileStoreAttributeView FileStoreAttributeView"/> classes
	''' that provide a read-only or updatable view of a set of file store attributes.
	''' 
	''' @since 1.7
	''' </summary>

	Public MustInherit Class FileStore

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the name of this file store. The format of the name is highly
		''' implementation specific. It will typically be the name of the storage
		''' pool or volume.
		''' 
		''' <p> The string returned by this method may differ from the string
		''' returned by the <seealso cref="Object#toString() toString"/> method.
		''' </summary>
		''' <returns>  the name of this file store </returns>
		Public MustOverride Function name() As String

		''' <summary>
		''' Returns the <em>type</em> of this file store. The format of the string
		''' returned by this method is highly implementation specific. It may
		''' indicate, for example, the format used or if the file store is local
		''' or remote.
		''' </summary>
		''' <returns>  a string representing the type of this file store </returns>
		Public MustOverride Function type() As String

		''' <summary>
		''' Tells whether this file store is read-only. A file store is read-only if
		''' it does not support write operations or other changes to files. Any
		''' attempt to create a file, open an existing file for writing etc. causes
		''' an {@code IOException} to be thrown.
		''' </summary>
		''' <returns>  {@code true} if, and only if, this file store is read-only </returns>
		Public MustOverride ReadOnly Property [readOnly] As Boolean

		''' <summary>
		''' Returns the size, in bytes, of the file store.
		''' </summary>
		''' <returns>  the size of the file store, in bytes
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Public MustOverride ReadOnly Property totalSpace As Long

		''' <summary>
		''' Returns the number of bytes available to this Java virtual machine on the
		''' file store.
		''' 
		''' <p> The returned number of available bytes is a hint, but not a
		''' guarantee, that it is possible to use most or any of these bytes.  The
		''' number of usable bytes is most likely to be accurate immediately
		''' after the space attributes are obtained. It is likely to be made inaccurate
		''' by any external I/O operations including those made on the system outside
		''' of this Java virtual machine.
		''' </summary>
		''' <returns>  the number of bytes available
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Public MustOverride ReadOnly Property usableSpace As Long

		''' <summary>
		''' Returns the number of unallocated bytes in the file store.
		''' 
		''' <p> The returned number of unallocated bytes is a hint, but not a
		''' guarantee, that it is possible to use most or any of these bytes.  The
		''' number of unallocated bytes is most likely to be accurate immediately
		''' after the space attributes are obtained. It is likely to be
		''' made inaccurate by any external I/O operations including those made on
		''' the system outside of this virtual machine.
		''' </summary>
		''' <returns>  the number of unallocated bytes
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Public MustOverride ReadOnly Property unallocatedSpace As Long

		''' <summary>
		''' Tells whether or not this file store supports the file attributes
		''' identified by the given file attribute view.
		''' 
		''' <p> Invoking this method to test if the file store supports {@link
		''' BasicFileAttributeView} will always return {@code true}. In the case of
		''' the default provider, this method cannot guarantee to give the correct
		''' result when the file store is not a local storage device. The reasons for
		''' this are implementation specific and therefore unspecified.
		''' </summary>
		''' <param name="type">
		'''          the file attribute view type
		''' </param>
		''' <returns>  {@code true} if, and only if, the file attribute view is
		'''          supported </returns>
		Public MustOverride Function supportsFileAttributeView(ByVal type As Class) As Boolean

		''' <summary>
		''' Tells whether or not this file store supports the file attributes
		''' identified by the given file attribute view.
		''' 
		''' <p> Invoking this method to test if the file store supports {@link
		''' BasicFileAttributeView}, identified by the name "{@code basic}" will
		''' always return {@code true}. In the case of the default provider, this
		''' method cannot guarantee to give the correct result when the file store is
		''' not a local storage device. The reasons for this are implementation
		''' specific and therefore unspecified.
		''' </summary>
		''' <param name="name">
		'''          the <seealso cref="FileAttributeView#name name"/> of file attribute view
		''' </param>
		''' <returns>  {@code true} if, and only if, the file attribute view is
		'''          supported </returns>
		Public MustOverride Function supportsFileAttributeView(ByVal name As String) As Boolean

		''' <summary>
		''' Returns a {@code FileStoreAttributeView} of the given type.
		''' 
		''' <p> This method is intended to be used where the file store attribute
		''' view defines type-safe methods to read or update the file store attributes.
		''' The {@code type} parameter is the type of the attribute view required and
		''' the method returns an instance of that type if supported.
		''' </summary>
		''' @param   <V>
		'''          The {@code FileStoreAttributeView} type </param>
		''' <param name="type">
		'''          the {@code Class} object corresponding to the attribute view
		''' </param>
		''' <returns>  a file store attribute view of the specified type or
		'''          {@code null} if the attribute view is not available </returns>
		Public MustOverride Function getFileStoreAttributeView(Of V As FileStoreAttributeView)(ByVal type As Class) As V

		''' <summary>
		''' Reads the value of a file store attribute.
		''' 
		''' <p> The {@code attribute} parameter identifies the attribute to be read
		''' and takes the form:
		''' <blockquote>
		''' <i>view-name</i><b>:</b><i>attribute-name</i>
		''' </blockquote>
		''' where the character {@code ':'} stands for itself.
		''' 
		''' <p> <i>view-name</i> is the <seealso cref="FileStoreAttributeView#name name"/> of
		''' a <seealso cref="FileStore AttributeView"/> that identifies a set of file attributes.
		''' <i>attribute-name</i> is the name of the attribute.
		''' 
		''' <p> <b>Usage Example:</b>
		''' Suppose we want to know if ZFS compression is enabled (assuming the "zfs"
		''' view is supported):
		''' <pre>
		'''    boolean compression = (Boolean)fs.getAttribute("zfs:compression");
		''' </pre>
		''' </summary>
		''' <param name="attribute">
		'''          the attribute to read
		''' </param>
		''' <returns>  the attribute value; {@code null} may be a valid valid for some
		'''          attributes
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''          if the attribute view is not available or it does not support
		'''          reading the attribute </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Public MustOverride Function getAttribute(ByVal attribute As String) As Object
	End Class

End Namespace
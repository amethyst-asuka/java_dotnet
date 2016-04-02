'
' * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file.attribute

	''' <summary>
	''' Defines the permissions for use with the permissions component of an ACL
	''' <seealso cref="AclEntry entry"/>.
	''' 
	''' @since 1.7
	''' </summary>

	Public Enum AclEntryPermission

		''' <summary>
		''' Permission to read the data of the file.
		''' </summary>
		READ_DATA

		''' <summary>
		''' Permission to modify the file's data.
		''' </summary>
		WRITE_DATA

		''' <summary>
		''' Permission to append data to a file.
		''' </summary>
		APPEND_DATA

		''' <summary>
		''' Permission to read the named attributes of a file.
		''' 
		''' <p> <a href="http://www.ietf.org/rfc/rfc3530.txt">RFC&nbsp;3530: Network
		''' File System (NFS) version 4 Protocol</a> defines <em>named attributes</em>
		''' as opaque files associated with a file in the file system.
		''' </summary>
		READ_NAMED_ATTRS

		''' <summary>
		''' Permission to write the named attributes of a file.
		''' 
		''' <p> <a href="http://www.ietf.org/rfc/rfc3530.txt">RFC&nbsp;3530: Network
		''' File System (NFS) version 4 Protocol</a> defines <em>named attributes</em>
		''' as opaque files associated with a file in the file system.
		''' </summary>
		WRITE_NAMED_ATTRS

		''' <summary>
		''' Permission to execute a file.
		''' </summary>
		EXECUTE

		''' <summary>
		''' Permission to delete a file or directory within a directory.
		''' </summary>
		DELETE_CHILD

		''' <summary>
		''' The ability to read (non-acl) file attributes.
		''' </summary>
		READ_ATTRIBUTES

		''' <summary>
		''' The ability to write (non-acl) file attributes.
		''' </summary>
		WRITE_ATTRIBUTES

		''' <summary>
		''' Permission to delete the file.
		''' </summary>
		DELETE

		''' <summary>
		''' Permission to read the ACL attribute.
		''' </summary>
		READ_ACL

		''' <summary>
		''' Permission to write the ACL attribute.
		''' </summary>
		WRITE_ACL

		''' <summary>
		''' Permission to change the owner.
		''' </summary>
		WRITE_OWNER

		''' <summary>
		''' Permission to access file locally at the server with synchronous reads
		''' and writes.
		''' </summary>
		SYNCHRONIZE

		''' <summary>
		''' Permission to list the entries of a directory (equal to <seealso cref="#READ_DATA"/>)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		Public Shared final AclEntryPermission LIST_DIRECTORY = READ_DATA;

		''' <summary>
		''' Permission to add a new file to a directory (equal to <seealso cref="#WRITE_DATA"/>)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		Public Shared final AclEntryPermission ADD_FILE = WRITE_DATA;

		''' <summary>
		''' Permission to create a subdirectory to a directory (equal to <seealso cref="#APPEND_DATA"/>)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		Public Shared final AclEntryPermission ADD_SUBDIRECTORY = APPEND_DATA;
	End Enum

End Namespace
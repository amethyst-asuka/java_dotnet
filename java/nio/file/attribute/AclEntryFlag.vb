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
	''' Defines the flags for used by the flags component of an ACL {@link AclEntry
	''' entry}.
	''' 
	''' <p> In this release, this class does not define flags related to {@link
	''' AclEntryType#AUDIT} and <seealso cref="AclEntryType#ALARM"/> entry types.
	''' 
	''' @since 1.7
	''' </summary>

	Public Enum AclEntryFlag

		''' <summary>
		''' Can be placed on a directory and indicates that the ACL entry should be
		''' added to each new non-directory file created.
		''' </summary>
		FILE_INHERIT

		''' <summary>
		''' Can be placed on a directory and indicates that the ACL entry should be
		''' added to each new directory created.
		''' </summary>
		DIRECTORY_INHERIT

		''' <summary>
		''' Can be placed on a directory to indicate that the ACL entry should not
		''' be placed on the newly created directory which is inheritable by
		''' subdirectories of the created directory.
		''' </summary>
		NO_PROPAGATE_INHERIT

		''' <summary>
		''' Can be placed on a directory but does not apply to the directory,
		''' only to newly created files/directories as specified by the
		''' <seealso cref="#FILE_INHERIT"/> and <seealso cref="#DIRECTORY_INHERIT"/> flags.
		''' </summary>
		INHERIT_ONLY
	End Enum

End Namespace
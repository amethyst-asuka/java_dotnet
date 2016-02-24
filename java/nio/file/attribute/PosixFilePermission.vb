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

Namespace java.nio.file.attribute

	''' <summary>
	''' Defines the bits for use with the {@link PosixFileAttributes#permissions()
	''' permissions} attribute.
	''' 
	''' <p> The <seealso cref="PosixFilePermissions"/> class defines methods for manipulating
	''' set of permissions.
	''' 
	''' @since 1.7
	''' </summary>

	Public Enum PosixFilePermission

		''' <summary>
		''' Read permission, owner.
		''' </summary>
		OWNER_READ

		''' <summary>
		''' Write permission, owner.
		''' </summary>
		OWNER_WRITE

		''' <summary>
		''' Execute/search permission, owner.
		''' </summary>
		OWNER_EXECUTE

		''' <summary>
		''' Read permission, group.
		''' </summary>
		GROUP_READ

		''' <summary>
		''' Write permission, group.
		''' </summary>
		GROUP_WRITE

		''' <summary>
		''' Execute/search permission, group.
		''' </summary>
		GROUP_EXECUTE

		''' <summary>
		''' Read permission, others.
		''' </summary>
		OTHERS_READ

		''' <summary>
		''' Write permission, others.
		''' </summary>
		OTHERS_WRITE

		''' <summary>
		''' Execute/search permission, others.
		''' </summary>
		OTHERS_EXECUTE
	End Enum

End Namespace
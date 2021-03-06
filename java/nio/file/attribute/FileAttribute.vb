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

Namespace java.nio.file.attribute

	''' <summary>
	''' An object that encapsulates the value of a file attribute that can be set
	''' atomically when creating a new file or directory by invoking the {@link
	''' java.nio.file.Files#createFile createFile} or {@link
	''' java.nio.file.Files#createDirectory createDirectory} methods.
	''' </summary>
	''' @param <T> The type of the file attribute value
	''' 
	''' @since 1.7 </param>
	''' <seealso cref= PosixFilePermissions#asFileAttribute </seealso>

	Public Interface FileAttribute(Of T)
		''' <summary>
		''' Returns the attribute name.
		''' </summary>
		''' <returns> The attribute name </returns>
		Function name() As String

		''' <summary>
		''' Returns the attribute value.
		''' </summary>
		''' <returns> The attribute value </returns>
		Function value() As T
	End Interface

End Namespace
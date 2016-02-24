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
	''' File attributes associated with a file in a file system that supports
	''' legacy "DOS" attributes.
	''' 
	''' <p> <b>Usage Example:</b>
	''' <pre>
	'''    Path file = ...
	'''    DosFileAttributes attrs = Files.readAttributes(file, DosFileAttributes.class);
	''' </pre>
	''' 
	''' @since 1.7
	''' </summary>

	Public Interface DosFileAttributes
		Inherits BasicFileAttributes

		''' <summary>
		''' Returns the value of the read-only attribute.
		''' 
		''' <p> This attribute is often used as a simple access control mechanism
		''' to prevent files from being deleted or updated. Whether the file system
		''' or platform does any enforcement to prevent <em>read-only</em> files
		''' from being updated is implementation specific.
		''' </summary>
		''' <returns>  the value of the read-only attribute </returns>
		ReadOnly Property [readOnly] As Boolean

		''' <summary>
		''' Returns the value of the hidden attribute.
		''' 
		''' <p> This attribute is often used to indicate if the file is visible to
		''' users.
		''' </summary>
		''' <returns>  the value of the hidden attribute </returns>
		ReadOnly Property hidden As Boolean

		''' <summary>
		''' Returns the value of the archive attribute.
		''' 
		''' <p> This attribute is typically used by backup programs.
		''' </summary>
		''' <returns>  the value of the archive attribute </returns>
		ReadOnly Property archive As Boolean

		''' <summary>
		''' Returns the value of the system attribute.
		''' 
		''' <p> This attribute is often used to indicate that the file is a component
		''' of the operating system.
		''' </summary>
		''' <returns>  the value of the system attribute </returns>
		ReadOnly Property system As Boolean
	End Interface

End Namespace
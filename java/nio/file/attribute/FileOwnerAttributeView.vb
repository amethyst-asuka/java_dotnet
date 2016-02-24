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
	''' A file attribute view that supports reading or updating the owner of a file.
	''' This file attribute view is intended for file system implementations that
	''' support a file attribute that represents an identity that is the owner of
	''' the file. Often the owner of a file is the identity of the entity that
	''' created the file.
	''' 
	''' <p> The <seealso cref="#getOwner getOwner"/> or <seealso cref="#setOwner setOwner"/> methods may
	''' be used to read or update the owner of the file.
	''' 
	''' <p> The <seealso cref="java.nio.file.Files#getAttribute getAttribute"/> and
	''' <seealso cref="java.nio.file.Files#setAttribute setAttribute"/> methods may also be
	''' used to read or update the owner. In that case, the owner attribute is
	''' identified by the name {@code "owner"}, and the value of the attribute is
	''' a <seealso cref="UserPrincipal"/>.
	''' 
	''' @since 1.7
	''' </summary>

	Public Interface FileOwnerAttributeView
		Inherits FileAttributeView

		''' <summary>
		''' Returns the name of the attribute view. Attribute views of this type
		''' have the name {@code "owner"}.
		''' </summary>
		Overrides Function name() As String

		''' <summary>
		''' Read the file owner.
		''' 
		''' <p> It it implementation specific if the file owner can be a {@link
		''' GroupPrincipal group}.
		''' </summary>
		''' <returns>  the file owner
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          In the case of the default provider, a security manager is
		'''          installed, and it denies {@link
		'''          RuntimePermission}<tt>("accessUserInformation")</tt> or its
		'''          <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
		'''          denies read access to the file. </exception>
		Property owner As UserPrincipal

	End Interface

End Namespace
Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security


	''' <summary>
	''' This class is a simple holder for a key pair (a public key and a
	''' private key). It does not enforce any security, and, when initialized,
	''' should be treated like a PrivateKey.
	''' </summary>
	''' <seealso cref= PublicKey </seealso>
	''' <seealso cref= PrivateKey
	''' 
	''' @author Benjamin Renaud </seealso>

	<Serializable> _
	Public NotInheritable Class KeyPair

		Private Const serialVersionUID As Long = -7565189502268009837L

		Private privateKey As PrivateKey
		Private publicKey As PublicKey

		''' <summary>
		''' Constructs a key pair from the given public key and private key.
		''' 
		''' <p>Note that this constructor only stores references to the public
		''' and private key components in the generated key pair. This is safe,
		''' because {@code Key} objects are immutable.
		''' </summary>
		''' <param name="publicKey"> the public key.
		''' </param>
		''' <param name="privateKey"> the private key. </param>
		Public Sub New(  publicKey As PublicKey,   privateKey As PrivateKey)
			Me.publicKey = publicKey
			Me.privateKey = privateKey
		End Sub

		''' <summary>
		''' Returns a reference to the public key component of this key pair.
		''' </summary>
		''' <returns> a reference to the public key. </returns>
		Public Property [public] As PublicKey
			Get
				Return publicKey
			End Get
		End Property

		 ''' <summary>
		 ''' Returns a reference to the private key component of this key pair.
		 ''' </summary>
		 ''' <returns> a reference to the private key. </returns>
	   Public Property [private] As PrivateKey
		   Get
				Return privateKey
		   End Get
	   End Property
	End Class

End Namespace
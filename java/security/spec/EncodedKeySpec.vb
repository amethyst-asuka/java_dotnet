'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.spec

	''' <summary>
	''' This class represents a public or private key in encoded format.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= X509EncodedKeySpec </seealso>
	''' <seealso cref= PKCS8EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class EncodedKeySpec
		Implements KeySpec

		Private encodedKey As SByte()

		''' <summary>
		''' Creates a new EncodedKeySpec with the given encoded key.
		''' </summary>
		''' <param name="encodedKey"> the encoded key. The contents of the
		''' array are copied to protect against subsequent modification. </param>
		''' <exception cref="NullPointerException"> if {@code encodedKey}
		''' is null. </exception>
		Public Sub New(  encodedKey As SByte())
			Me.encodedKey = encodedKey.clone()
		End Sub

		''' <summary>
		''' Returns the encoded key.
		''' </summary>
		''' <returns> the encoded key. Returns a new array each time
		''' this method is called. </returns>
		Public Overridable Property encoded As SByte()
			Get
				Return Me.encodedKey.clone()
			End Get
		End Property

		''' <summary>
		''' Returns the name of the encoding format associated with this
		''' key specification.
		''' 
		''' <p>If the opaque representation of a key
		''' (see <seealso cref="java.security.Key Key"/>) can be transformed
		''' (see <seealso cref="java.security.KeyFactory KeyFactory"/>)
		''' into this key specification (or a subclass of it),
		''' {@code getFormat} called
		''' on the opaque key returns the same value as the
		''' {@code getFormat} method
		''' of this key specification.
		''' </summary>
		''' <returns> a string representation of the encoding format. </returns>
		Public MustOverride ReadOnly Property format As String
	End Class

End Namespace
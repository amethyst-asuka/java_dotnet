Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	''' for the {@code SecureRandom} class.
	''' All the abstract methods in this class must be implemented by each
	''' service provider who wishes to supply the implementation
	''' of a cryptographically strong pseudo-random number generator.
	''' 
	''' </summary>
	''' <seealso cref= SecureRandom
	''' @since 1.2 </seealso>

	<Serializable> _
	Public MustInherit Class SecureRandomSpi

		Private Const serialVersionUID As Long = -2991854161009191830L

		''' <summary>
		''' Reseeds this random object. The given seed supplements, rather than
		''' replaces, the existing seed. Thus, repeated calls are guaranteed
		''' never to reduce randomness.
		''' </summary>
		''' <param name="seed"> the seed. </param>
		Protected Friend MustOverride Sub engineSetSeed(  seed As SByte())

		''' <summary>
		''' Generates a user-specified number of random bytes.
		''' 
		''' <p> If a call to {@code engineSetSeed} had not occurred previously,
		''' the first call to this method forces this SecureRandom implementation
		''' to seed itself.  This self-seeding will not occur if
		''' {@code engineSetSeed} was previously called.
		''' </summary>
		''' <param name="bytes"> the array to be filled in with random bytes. </param>
		Protected Friend MustOverride Sub engineNextBytes(  bytes As SByte())

		''' <summary>
		''' Returns the given number of seed bytes.  This call may be used to
		''' seed other random number generators.
		''' </summary>
		''' <param name="numBytes"> the number of seed bytes to generate.
		''' </param>
		''' <returns> the seed bytes. </returns>
		 Protected Friend MustOverride Function engineGenerateSeed(  numBytes As Integer) As SByte()
	End Class

End Namespace
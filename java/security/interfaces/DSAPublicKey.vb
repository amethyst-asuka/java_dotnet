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

Namespace java.security.interfaces


	''' <summary>
	''' The interface to a DSA public key. DSA (Digital Signature Algorithm)
	''' is defined in NIST's FIPS-186.
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.Signature </seealso>
	''' <seealso cref= DSAKey </seealso>
	''' <seealso cref= DSAPrivateKey
	''' 
	''' @author Benjamin Renaud </seealso>
	Public Interface DSAPublicKey
		Inherits DSAKey, java.security.PublicKey

		' Declare serialVersionUID to be compatible with JDK1.1

	   ''' <summary>
	   ''' The class fingerprint that is set to indicate
	   ''' serialization compatibility with a previous
	   ''' version of the class.
	   ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = 1234526332779022332L;

		''' <summary>
		''' Returns the value of the public key, {@code y}.
		''' </summary>
		''' <returns> the value of the public key, {@code y}. </returns>
		ReadOnly Property y As System.Numerics.BigInteger
	End Interface

End Namespace
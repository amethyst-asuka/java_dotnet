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

Namespace java.security.interfaces


	''' <summary>
	''' The standard interface to a DSA private key. DSA (Digital Signature
	''' Algorithm) is defined in NIST's FIPS-186.
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.Signature </seealso>
	''' <seealso cref= DSAKey </seealso>
	''' <seealso cref= DSAPublicKey
	''' 
	''' @author Benjamin Renaud </seealso>
	Public Interface DSAPrivateKey
		Inherits DSAKey, java.security.PrivateKey

		' Declare serialVersionUID to be compatible with JDK1.1

	   ''' <summary>
	   ''' The class fingerprint that is set to indicate
	   ''' serialization compatibility with a previous
	   ''' version of the class.
	   ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = 7776497482533790279L;

		''' <summary>
		''' Returns the value of the private key, {@code x}.
		''' </summary>
		''' <returns> the value of the private key, {@code x}. </returns>
		ReadOnly Property x As System.Numerics.BigInteger
	End Interface

End Namespace
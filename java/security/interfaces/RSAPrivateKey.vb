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

Namespace java.security.interfaces


	''' <summary>
	''' The interface to an RSA private key.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= RSAPrivateCrtKey </seealso>

	Public Interface RSAPrivateKey
		Inherits java.security.PrivateKey, RSAKey

		''' <summary>
		''' The type fingerprint that is set to indicate
		''' serialization compatibility with a previous
		''' version of the type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = 5187144804936595022L;

		''' <summary>
		''' Returns the private exponent.
		''' </summary>
		''' <returns> the private exponent </returns>
		ReadOnly Property privateExponent As System.Numerics.BigInteger
	End Interface

End Namespace
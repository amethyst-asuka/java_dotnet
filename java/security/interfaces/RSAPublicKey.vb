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
	''' The interface to an RSA public key.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>

	Public Interface RSAPublicKey
		Inherits java.security.PublicKey, RSAKey

		''' <summary>
		''' The type fingerprint that is set to indicate
		''' serialization compatibility with a previous
		''' version of the type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = -8727434096241101194L;

		''' <summary>
		''' Returns the public exponent.
		''' </summary>
		''' <returns> the public exponent </returns>
		ReadOnly Property publicExponent As System.Numerics.BigInteger
	End Interface

End Namespace
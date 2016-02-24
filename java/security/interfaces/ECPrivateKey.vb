'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' The interface to an elliptic curve (EC) private key.
	''' 
	''' @author Valerie Peng
	''' 
	''' </summary>
	''' <seealso cref= PrivateKey </seealso>
	''' <seealso cref= ECKey
	''' 
	''' @since 1.5 </seealso>
	Public Interface ECPrivateKey
		Inherits java.security.PrivateKey, ECKey

	   ''' <summary>
	   ''' The class fingerprint that is set to indicate
	   ''' serialization compatibility.
	   ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = -7896394956925609184L;

		''' <summary>
		''' Returns the private value S. </summary>
		''' <returns> the private value S. </returns>
		ReadOnly Property s As System.Numerics.BigInteger
	End Interface

End Namespace
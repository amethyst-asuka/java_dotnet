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
	''' The interface to an elliptic curve (EC) public key.
	''' 
	''' @author Valerie Peng
	''' 
	''' </summary>
	''' <seealso cref= PublicKey </seealso>
	''' <seealso cref= ECKey </seealso>
	''' <seealso cref= java.security.spec.ECPoint
	''' 
	''' @since 1.5 </seealso>
	Public Interface ECPublicKey
		Inherits java.security.PublicKey, ECKey

	   ''' <summary>
	   ''' The class fingerprint that is set to indicate
	   ''' serialization compatibility.
	   ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = -3314988629879632826L;

		''' <summary>
		''' Returns the public point W. </summary>
		''' <returns> the public point W. </returns>
		ReadOnly Property w As java.security.spec.ECPoint
	End Interface

End Namespace
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
Namespace java.security.spec


	''' <summary>
	''' This interface represents an elliptic curve (EC) finite field.
	''' All specialized EC fields must implements this interface.
	''' </summary>
	''' <seealso cref= ECFieldFp </seealso>
	''' <seealso cref= ECFieldF2m
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Interface ECField
		''' <summary>
		''' Returns the field size in bits. Note: For prime finite
		''' field ECFieldFp, size of prime p in bits is returned.
		''' For characteristic 2 finite field ECFieldF2m, m is returned. </summary>
		''' <returns> the field size in bits. </returns>
		ReadOnly Property fieldSize As Integer
	End Interface

End Namespace
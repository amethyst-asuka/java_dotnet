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
	''' The interface to an elliptic curve (EC) key.
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface ECKey
		''' <summary>
		''' Returns the domain parameters associated
		''' with this key. The domain parameters are
		''' either explicitly specified or implicitly
		''' created during key generation. </summary>
		''' <returns> the associated domain parameters. </returns>
		ReadOnly Property params As java.security.spec.ECParameterSpec
	End Interface

End Namespace
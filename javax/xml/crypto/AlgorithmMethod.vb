'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
'
' * $Id: AlgorithmMethod.java,v 1.4 2005/05/10 15:47:41 mullan Exp $
' 
Namespace javax.xml.crypto


	''' <summary>
	''' An abstract representation of an algorithm defined in the XML Security
	''' specifications. Subclasses represent specific types of XML security
	''' algorithms, such as a <seealso cref="javax.xml.crypto.dsig.Transform"/>.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Interface AlgorithmMethod

		''' <summary>
		''' Returns the algorithm URI of this <code>AlgorithmMethod</code>.
		''' </summary>
		''' <returns> the algorithm URI of this <code>AlgorithmMethod</code> </returns>
		ReadOnly Property algorithm As String

		''' <summary>
		''' Returns the algorithm parameters of this <code>AlgorithmMethod</code>.
		''' </summary>
		''' <returns> the algorithm parameters of this <code>AlgorithmMethod</code>.
		'''    Returns <code>null</code> if this <code>AlgorithmMethod</code> does
		'''    not require parameters and they are not specified. </returns>
		ReadOnly Property parameterSpec As java.security.spec.AlgorithmParameterSpec
	End Interface

End Namespace
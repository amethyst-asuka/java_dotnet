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
' * $Id: SignatureMethodParameterSpec.java,v 1.3 2005/05/10 16:40:17 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' A specification of algorithm parameters for an XML <seealso cref="SignatureMethod"/>
	''' algorithm. The purpose of this interface is to group (and provide type
	''' safety for) all signature method parameter specifications. All signature
	''' method parameter specifications must implement this interface.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= SignatureMethod </seealso>
	Public Interface SignatureMethodParameterSpec
		Inherits java.security.spec.AlgorithmParameterSpec

	End Interface

End Namespace
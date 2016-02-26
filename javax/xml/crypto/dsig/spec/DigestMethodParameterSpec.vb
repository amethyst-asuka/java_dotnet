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
' * $Id: DigestMethodParameterSpec.java,v 1.3 2005/05/10 16:40:17 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' A specification of algorithm parameters for a <seealso cref="DigestMethod"/>
	''' algorithm. The purpose of this interface is to group (and provide type
	''' safety for) all digest method parameter specifications. All digest method
	''' parameter specifications must implement this interface.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= DigestMethod </seealso>
	Public Interface DigestMethodParameterSpec
		Inherits java.security.spec.AlgorithmParameterSpec

	End Interface

End Namespace
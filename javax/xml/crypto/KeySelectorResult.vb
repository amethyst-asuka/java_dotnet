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
' * $Id: KeySelectorResult.java,v 1.3 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto


	''' <summary>
	''' The result returned by the <seealso cref="KeySelector#select KeySelector.select"/>
	''' method.
	''' <p>
	''' At a minimum, a <code>KeySelectorResult</code> contains the <code>Key</code>
	''' selected by the <code>KeySelector</code>. Implementations of this interface
	''' may add methods to return implementation or algorithm specific information,
	''' such as a chain of certificates or debugging information.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= KeySelector </seealso>
	Public Interface KeySelectorResult

		''' <summary>
		''' Returns the selected key.
		''' </summary>
		''' <returns> the selected key, or <code>null</code> if none can be found </returns>
		ReadOnly Property key As java.security.Key
	End Interface

End Namespace
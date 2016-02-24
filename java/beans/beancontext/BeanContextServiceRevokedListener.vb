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
Namespace java.beans.beancontext



	''' <summary>
	'''  The listener interface for receiving
	''' <code>BeanContextServiceRevokedEvent</code> objects. A class that is
	''' interested in processing a <code>BeanContextServiceRevokedEvent</code>
	''' implements this interface.
	''' </summary>
	Public Interface BeanContextServiceRevokedListener
		Inherits java.util.EventListener

		''' <summary>
		''' The service named has been revoked. getService requests for
		''' this service will no longer be satisfied. </summary>
		''' <param name="bcsre"> the <code>BeanContextServiceRevokedEvent</code> received
		''' by this listener. </param>
		Sub serviceRevoked(ByVal bcsre As java.beans.beancontext.BeanContextServiceRevokedEvent)
	End Interface

End Namespace
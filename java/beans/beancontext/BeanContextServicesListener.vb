'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' The listener interface for receiving
	''' <code>BeanContextServiceAvailableEvent</code> objects.
	''' A class that is interested in processing a
	''' <code>BeanContextServiceAvailableEvent</code> implements this interface.
	''' </summary>
	Public Interface BeanContextServicesListener
		Inherits java.beans.beancontext.BeanContextServiceRevokedListener

		''' <summary>
		''' The service named has been registered. getService requests for
		''' this service may now be made. </summary>
		''' <param name="bcsae"> the <code>BeanContextServiceAvailableEvent</code> </param>
		Sub serviceAvailable(  bcsae As java.beans.beancontext.BeanContextServiceAvailableEvent)
	End Interface

End Namespace
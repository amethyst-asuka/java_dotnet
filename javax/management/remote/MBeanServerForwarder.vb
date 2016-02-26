'
' * Copyright (c) 2003, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.remote


	''' <summary>
	''' <p>An object of this class implements the MBeanServer interface and
	''' wraps another object that also implements that interface.
	''' Typically, an implementation of this interface performs some action
	''' in some or all methods of the <code>MBeanServer</code> interface
	''' before and/or after forwarding the method to the wrapped object.
	''' Examples include security checking and logging.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface MBeanServerForwarder
		Inherits javax.management.MBeanServer

		''' <summary>
		''' Returns the MBeanServer object to which requests will be forwarded.
		''' </summary>
		''' <returns> the MBeanServer object to which requests will be forwarded,
		''' or null if there is none.
		''' </returns>
		''' <seealso cref= #setMBeanServer </seealso>
		Property mBeanServer As javax.management.MBeanServer

	End Interface

End Namespace
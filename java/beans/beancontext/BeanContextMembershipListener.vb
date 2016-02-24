'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>
	''' Compliant BeanContexts fire events on this interface when the state of
	''' the membership of the BeanContext changes.
	''' </p>
	''' 
	''' @author      Laurence P. G. Cable
	''' @since       1.2 </summary>
	''' <seealso cref=         java.beans.beancontext.BeanContext </seealso>

	Public Interface BeanContextMembershipListener
		Inherits java.util.EventListener

		''' <summary>
		''' Called when a child or list of children is added to a
		''' <code>BeanContext</code> that this listener is registered with. </summary>
		''' <param name="bcme"> The <code>BeanContextMembershipEvent</code>
		''' describing the change that occurred. </param>
		Sub childrenAdded(ByVal bcme As java.beans.beancontext.BeanContextMembershipEvent)

		''' <summary>
		''' Called when a child or list of children is removed
		''' from a <code>BeanContext</code> that this listener
		''' is registered with. </summary>
		''' <param name="bcme"> The <code>BeanContextMembershipEvent</code>
		''' describing the change that occurred. </param>
		Sub childrenRemoved(ByVal bcme As java.beans.beancontext.BeanContextMembershipEvent)
	End Interface

End Namespace
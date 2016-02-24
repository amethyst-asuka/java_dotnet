'
' * Copyright (c) 1998, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface is implemented by BeanContexts' that have an AWT Container
	''' associated with them.
	''' </p>
	''' 
	''' @author Laurence P. G. Cable
	''' @since 1.2
	''' </summary>
	''' <seealso cref= java.beans.beancontext.BeanContext </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextSupport </seealso>

	Public Interface BeanContextContainerProxy

		''' <summary>
		''' Gets the <code>java.awt.Container</code> associated
		''' with this <code>BeanContext</code>. </summary>
		''' <returns> the <code>java.awt.Container</code> associated
		''' with this <code>BeanContext</code>. </returns>
		ReadOnly Property container As java.awt.Container
	End Interface

End Namespace
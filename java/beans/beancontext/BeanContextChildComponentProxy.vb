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
	''' This interface is implemented by
	''' <code>BeanContextChildren</code> that have an AWT <code>Component</code>
	''' associated with them.
	''' </p>
	''' 
	''' @author Laurence P. G. Cable
	''' @since 1.2
	''' </summary>
	''' <seealso cref= java.beans.beancontext.BeanContext </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextSupport </seealso>

	Public Interface BeanContextChildComponentProxy

		''' <summary>
		''' Gets the <code>java.awt.Component</code> associated with
		''' this <code>BeanContextChild</code>. </summary>
		''' <returns> the AWT <code>Component</code> associated with
		''' this <code>BeanContextChild</code> </returns>

		ReadOnly Property component As java.awt.Component
	End Interface

End Namespace
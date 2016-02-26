Imports System

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management

	' java import

	''' <summary>
	''' Allows a query to be performed in the context of a specific MBean server.
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public MustInherit Class QueryEval

		' Serial version 
		Private Const serialVersionUID As Long = 2675899265640874796L

		Private Shared server As ThreadLocal(Of MBeanServer) = New InheritableThreadLocal(Of MBeanServer)

		''' <summary>
		''' <p>Sets the MBean server on which the query is to be performed.
		''' The setting is valid for the thread performing the set.
		''' It is copied to any threads created by that thread at the moment
		''' of their creation.</p>
		''' 
		''' <p>For historical reasons, this method is not static, but its
		''' behavior does not depend on the instance on which it is
		''' called.</p>
		''' </summary>
		''' <param name="s"> The MBean server on which the query is to be performed.
		''' </param>
		''' <seealso cref= #getMBeanServer </seealso>
		Public Overridable Property mBeanServer As MBeanServer
			Set(ByVal s As MBeanServer)
				server.set(s)
			End Set
			Get
				Return server.get()
			End Get
		End Property

	End Class

End Namespace
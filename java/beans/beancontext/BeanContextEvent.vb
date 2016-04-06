Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1997, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>BeanContextEvent</code> is the abstract root event class
	''' for all events emitted
	''' from, and pertaining to the semantics of, a <code>BeanContext</code>.
	''' This class introduces a mechanism to allow the propagation of
	''' <code>BeanContextEvent</code> subclasses through a hierarchy of
	''' <code>BeanContext</code>s. The <code>setPropagatedFrom()</code>
	''' and <code>getPropagatedFrom()</code> methods allow a
	''' <code>BeanContext</code> to identify itself as the source
	''' of a propagated event.
	''' </p>
	''' 
	''' @author      Laurence P. G. Cable
	''' @since       1.2 </summary>
	''' <seealso cref=         java.beans.beancontext.BeanContext </seealso>

	Public MustInherit Class BeanContextEvent
		Inherits java.util.EventObject

		Private Const serialVersionUID As Long = 7267998073569045052L

		''' <summary>
		''' Contruct a BeanContextEvent
		''' </summary>
		''' <param name="bc">        The BeanContext source </param>
		Protected Friend Sub New(  bc As java.beans.beancontext.BeanContext)
			MyBase.New(bc)
		End Sub

		''' <summary>
		''' Gets the <code>BeanContext</code> associated with this event. </summary>
		''' <returns> the <code>BeanContext</code> associated with this event. </returns>
		Public Overridable Property beanContext As java.beans.beancontext.BeanContext
			Get
				Return CType(source, java.beans.beancontext.BeanContext)
			End Get
		End Property

		''' <summary>
		''' Sets the <code>BeanContext</code> from which this event was propagated. </summary>
		''' <param name="bc"> the <code>BeanContext</code> from which this event
		''' was propagated </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property propagatedFrom As java.beans.beancontext.BeanContext
			Set(  bc As java.beans.beancontext.BeanContext)
				propagatedFrom = bc
			End Set
			Get
				Return propagatedFrom
			End Get
		End Property


		''' <summary>
		''' Reports whether or not this event is
		''' propagated from some other <code>BeanContext</code>. </summary>
		''' <returns> <code>true</code> if propagated, <code>false</code>
		''' if not </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property propagated As Boolean
			Get
				Return propagatedFrom IsNot Nothing
			End Get
		End Property

	'    
	'     * fields
	'     

		''' <summary>
		''' The <code>BeanContext</code> from which this event was propagated
		''' </summary>
		Protected Friend propagatedFrom As java.beans.beancontext.BeanContext
	End Class

End Namespace
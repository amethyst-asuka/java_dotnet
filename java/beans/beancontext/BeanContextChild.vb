'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' JavaBeans wishing to be nested within, and obtain a reference to their
	''' execution environment, or context, as defined by the BeanContext
	''' sub-interface shall implement this interface.
	''' </p>
	''' <p>
	''' Conformant BeanContexts shall as a side effect of adding a BeanContextChild
	''' object shall pass a reference to itself via the setBeanContext() method of
	''' this interface.
	''' </p>
	''' <p>
	''' Note that a BeanContextChild may refuse a change in state by throwing
	''' PropertyVetoedException in response.
	''' </p>
	''' <p>
	''' In order for persistence mechanisms to function properly on BeanContextChild
	''' instances across a broad variety of scenarios, implementing classes of this
	''' interface are required to define as transient, any or all fields, or
	''' instance variables, that may contain, or represent, references to the
	''' nesting BeanContext instance or other resources obtained
	''' from the BeanContext via any unspecified mechanisms.
	''' </p>
	''' 
	''' @author      Laurence P. G. Cable
	''' @since       1.2
	''' </summary>
	''' <seealso cref= java.beans.beancontext.BeanContext </seealso>
	''' <seealso cref= java.beans.PropertyChangeEvent </seealso>
	''' <seealso cref= java.beans.PropertyChangeListener </seealso>
	''' <seealso cref= java.beans.PropertyVetoException </seealso>
	''' <seealso cref= java.beans.VetoableChangeListener </seealso>

	Public Interface BeanContextChild

		''' <summary>
		''' <p>
		''' Objects that implement this interface,
		''' shall fire a java.beans.PropertyChangeEvent, with parameters:
		''' 
		''' propertyName "beanContext", oldValue (the previous nesting
		''' <code>BeanContext</code> instance, or <code>null</code>),
		''' newValue (the current nesting
		''' <code>BeanContext</code> instance, or <code>null</code>).
		''' <p>
		''' A change in the value of the nesting BeanContext property of this
		''' BeanContextChild may be vetoed by throwing the appropriate exception.
		''' </p> </summary>
		''' <param name="bc"> The <code>BeanContext</code> with which
		''' to associate this <code>BeanContextChild</code>. </param>
		''' <exception cref="PropertyVetoException"> if the
		''' addition of the specified <code>BeanContext</code> is refused. </exception>
		Property beanContext As java.beans.beancontext.BeanContext


		''' <summary>
		''' Adds a <code>PropertyChangeListener</code>
		''' to this <code>BeanContextChild</code>
		''' in order to receive a <code>PropertyChangeEvent</code>
		''' whenever the specified property has changed. </summary>
		''' <param name="name"> the name of the property to listen on </param>
		''' <param name="pcl"> the <code>PropertyChangeListener</code> to add </param>
		Sub addPropertyChangeListener(ByVal name As String, ByVal pcl As java.beans.PropertyChangeListener)

		''' <summary>
		''' Removes a <code>PropertyChangeListener</code> from this
		''' <code>BeanContextChild</code>  so that it no longer
		''' receives <code>PropertyChangeEvents</code> when the
		''' specified property is changed.
		''' </summary>
		''' <param name="name"> the name of the property that was listened on </param>
		''' <param name="pcl"> the <code>PropertyChangeListener</code> to remove </param>
		Sub removePropertyChangeListener(ByVal name As String, ByVal pcl As java.beans.PropertyChangeListener)

		''' <summary>
		''' Adds a <code>VetoableChangeListener</code> to
		''' this <code>BeanContextChild</code>
		''' to receive events whenever the specified property changes. </summary>
		''' <param name="name"> the name of the property to listen on </param>
		''' <param name="vcl"> the <code>VetoableChangeListener</code> to add </param>
		Sub addVetoableChangeListener(ByVal name As String, ByVal vcl As java.beans.VetoableChangeListener)

		''' <summary>
		''' Removes a <code>VetoableChangeListener</code> from this
		''' <code>BeanContextChild</code> so that it no longer receives
		''' events when the specified property changes. </summary>
		''' <param name="name"> the name of the property that was listened on. </param>
		''' <param name="vcl"> the <code>VetoableChangeListener</code> to remove. </param>
		Sub removeVetoableChangeListener(ByVal name As String, ByVal vcl As java.beans.VetoableChangeListener)

	End Interface

End Namespace
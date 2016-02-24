'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans

	''' <summary>
	''' <p>
	''' This interface is intended to be implemented by, or delegated from, instances
	''' of java.beans.beancontext.BeanContext, in order to propagate to its nested hierarchy
	''' of java.beans.beancontext.BeanContextChild instances, the current "designTime" property.
	''' <p>
	''' The JavaBeans&trade; specification defines the notion of design time as is a
	''' mode in which JavaBeans instances should function during their composition
	''' and customization in a interactive design, composition or construction tool,
	''' as opposed to runtime when the JavaBean is part of an applet, application,
	''' or other live Java executable abstraction.
	''' 
	''' @author Laurence P. G. Cable
	''' @since 1.2
	''' </summary>
	''' <seealso cref= java.beans.beancontext.BeanContext </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextChild </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextMembershipListener </seealso>
	''' <seealso cref= java.beans.PropertyChangeEvent </seealso>

	Public Interface DesignMode

		''' <summary>
		''' The standard value of the propertyName as fired from a BeanContext or
		''' other source of PropertyChangeEvents.
		''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static String PROPERTYNAME = "designTime";

		''' <summary>
		''' Sets the "value" of the "designTime" property.
		''' <p>
		''' If the implementing object is an instance of java.beans.beancontext.BeanContext,
		''' or a subinterface thereof, then that BeanContext should fire a
		''' PropertyChangeEvent, to its registered BeanContextMembershipListeners, with
		''' parameters:
		''' <ul>
		'''    <li><code>propertyName</code> - <code>java.beans.DesignMode.PROPERTYNAME</code>
		'''    <li><code>oldValue</code> - previous value of "designTime"
		'''    <li><code>newValue</code> - current value of "designTime"
		''' </ul>
		''' Note it is illegal for a BeanContextChild to invoke this method
		''' associated with a BeanContext that it is nested within.
		''' </summary>
		''' <param name="designTime">  the current "value" of the "designTime" property </param>
		''' <seealso cref= java.beans.beancontext.BeanContext </seealso>
		''' <seealso cref= java.beans.beancontext.BeanContextMembershipListener </seealso>
		''' <seealso cref= java.beans.PropertyChangeEvent </seealso>

		Property designTime As Boolean


	End Interface

End Namespace
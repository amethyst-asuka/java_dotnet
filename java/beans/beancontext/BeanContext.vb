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
	''' The BeanContext acts a logical hierarchical container for JavaBeans.
	''' </p>
	''' 
	''' @author Laurence P. G. Cable
	''' @since 1.2
	''' </summary>
	''' <seealso cref= java.beans.Beans </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextChild </seealso>
	''' <seealso cref= java.beans.beancontext.BeanContextMembershipListener </seealso>
	''' <seealso cref= java.beans.PropertyChangeEvent </seealso>
	''' <seealso cref= java.beans.DesignMode </seealso>
	''' <seealso cref= java.beans.Visibility </seealso>
	''' <seealso cref= java.util.Collection </seealso>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface BeanContext
		Inherits BeanContextChild, ICollection, java.beans.DesignMode, java.beans.Visibility

		''' <summary>
		''' Instantiate the javaBean named as a
		''' child of this <code>BeanContext</code>.
		''' The implementation of the JavaBean is
		''' derived from the value of the beanName parameter,
		''' and is defined by the
		''' <code>java.beans.Beans.instantiate()</code> method.
		''' </summary>
		''' <returns> a javaBean named as a child of this
		''' <code>BeanContext</code> </returns>
		''' <param name="beanName"> The name of the JavaBean to instantiate
		''' as a child of this <code>BeanContext</code> </param>
		''' <exception cref="IOException"> if an IO problem occurs </exception>
		''' <exception cref="ClassNotFoundException"> if the class identified
		''' by the beanName parameter is not found </exception>
		Function instantiateChild(ByVal beanName As String) As Object

		''' <summary>
		''' Analagous to <code>java.lang.ClassLoader.getResourceAsStream()</code>,
		''' this method allows a <code>BeanContext</code> implementation
		''' to interpose behavior between the child <code>Component</code>
		''' and underlying <code>ClassLoader</code>.
		''' </summary>
		''' <param name="name"> the resource name </param>
		''' <param name="bcc"> the specified child </param>
		''' <returns> an <code>InputStream</code> for reading the resource,
		''' or <code>null</code> if the resource could not
		''' be found. </returns>
		''' <exception cref="IllegalArgumentException"> if
		''' the resource is not valid </exception>
		Function getResourceAsStream(ByVal name As String, ByVal bcc As BeanContextChild) As java.io.InputStream

		''' <summary>
		''' Analagous to <code>java.lang.ClassLoader.getResource()</code>, this
		''' method allows a <code>BeanContext</code> implementation to interpose
		''' behavior between the child <code>Component</code>
		''' and underlying <code>ClassLoader</code>.
		''' </summary>
		''' <param name="name"> the resource name </param>
		''' <param name="bcc"> the specified child </param>
		''' <returns> a <code>URL</code> for the named
		''' resource for the specified child </returns>
		''' <exception cref="IllegalArgumentException">
		''' if the resource is not valid </exception>
		Function getResource(ByVal name As String, ByVal bcc As BeanContextChild) As java.net.URL

		 ''' <summary>
		 ''' Adds the specified <code>BeanContextMembershipListener</code>
		 ''' to receive <code>BeanContextMembershipEvents</code> from
		 ''' this <code>BeanContext</code> whenever it adds
		 ''' or removes a child <code>Component</code>(s).
		 ''' </summary>
		 ''' <param name="bcml"> the BeanContextMembershipListener to be added </param>
		Sub addBeanContextMembershipListener(ByVal bcml As BeanContextMembershipListener)

		 ''' <summary>
		 ''' Removes the specified <code>BeanContextMembershipListener</code>
		 ''' so that it no longer receives <code>BeanContextMembershipEvent</code>s
		 ''' when the child <code>Component</code>(s) are added or removed.
		 ''' </summary>
		 ''' <param name="bcml"> the <code>BeanContextMembershipListener</code>
		 ''' to be removed </param>
		Sub removeBeanContextMembershipListener(ByVal bcml As BeanContextMembershipListener)

		''' <summary>
		''' This global lock is used by both <code>BeanContext</code>
		''' and <code>BeanContextServices</code> implementors
		''' to serialize changes in a <code>BeanContext</code>
		''' hierarchy and any service requests etc.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Object globalHierarchyLock = New Object();
	End Interface

End Namespace
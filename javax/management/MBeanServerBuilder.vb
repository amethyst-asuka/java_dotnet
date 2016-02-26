'
' * Copyright (c) 2002, 2007, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' <p>This class represents a builder that creates a default
	''' <seealso cref="javax.management.MBeanServer"/> implementation.
	''' The JMX <seealso cref="javax.management.MBeanServerFactory"/> allows
	''' applications to provide their custom MBeanServer
	''' implementation by providing a subclass of this class.</p>
	''' </summary>
	''' <seealso cref= MBeanServer </seealso>
	''' <seealso cref= MBeanServerFactory
	''' 
	''' @since 1.5 </seealso>
	Public Class MBeanServerBuilder
		''' <summary>
		''' Public default constructor.
		''' 
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' This method creates a new MBeanServerDelegate for a new MBeanServer.
		''' When creating a new MBeanServer the
		''' <seealso cref="javax.management.MBeanServerFactory"/> first calls this method
		''' in order to create a new MBeanServerDelegate.
		''' <br>Then it calls
		''' <code>newMBeanServer(defaultDomain,outer,delegate)</code>
		''' passing the <var>delegate</var> that should be used by the MBeanServer
		''' implementation.
		''' <p>Note that the passed <var>delegate</var> might not be directly the
		''' MBeanServerDelegate that was returned by this method. It could
		''' be, for instance, a new object wrapping the previously
		''' returned object.
		''' </summary>
		''' <returns> A new <seealso cref="javax.management.MBeanServerDelegate"/>.
		'''  </returns>
		Public Overridable Function newMBeanServerDelegate() As MBeanServerDelegate
			Return com.sun.jmx.mbeanserver.JmxMBeanServer.newMBeanServerDelegate()
		End Function

		''' <summary>
		''' This method creates a new MBeanServer implementation object.
		''' When creating a new MBeanServer the
		''' <seealso cref="javax.management.MBeanServerFactory"/> first calls
		''' <code>newMBeanServerDelegate()</code> in order to obtain a new
		''' <seealso cref="javax.management.MBeanServerDelegate"/> for the new
		''' MBeanServer. Then it calls
		''' <code>newMBeanServer(defaultDomain,outer,delegate)</code>
		''' passing the <var>delegate</var> that should be used by the MBeanServer
		''' implementation.
		''' <p>Note that the passed <var>delegate</var> might not be directly the
		''' MBeanServerDelegate that was returned by this implementation. It could
		''' be, for instance, a new object wrapping the previously
		''' returned delegate.
		''' <p>The <var>outer</var> parameter is a pointer to the MBeanServer that
		''' should be passed to the <seealso cref="javax.management.MBeanRegistration"/>
		''' interface when registering MBeans inside the MBeanServer.
		''' If <var>outer</var> is <code>null</code>, then the MBeanServer
		''' implementation must use its own <code>this</code> reference when
		''' invoking the <seealso cref="javax.management.MBeanRegistration"/> interface.
		''' <p>This makes it possible for a MBeanServer implementation to wrap
		''' another MBeanServer implementation, in order to implement, e.g,
		''' security checks, or to prevent access to the actual MBeanServer
		''' implementation by returning a pointer to a wrapping object.
		''' </summary>
		''' <param name="defaultDomain"> Default domain of the new MBeanServer. </param>
		''' <param name="outer"> A pointer to the MBeanServer object that must be
		'''        passed to the MBeans when invoking their
		'''        <seealso cref="javax.management.MBeanRegistration"/> interface. </param>
		''' <param name="delegate"> A pointer to the MBeanServerDelegate associated
		'''        with the new MBeanServer. The new MBeanServer must register
		'''        this MBean in its MBean repository.
		''' </param>
		''' <returns> A new private implementation of an MBeanServer.
		'''  </returns>
		Public Overridable Function newMBeanServer(ByVal defaultDomain As String, ByVal outer As MBeanServer, ByVal [delegate] As MBeanServerDelegate) As MBeanServer
			' By default, MBeanServerInterceptors are disabled.
			' Use com.sun.jmx.mbeanserver.MBeanServerBuilder to obtain
			' MBeanServers on which MBeanServerInterceptors are enabled.
			Return com.sun.jmx.mbeanserver.JmxMBeanServer.newMBeanServer(defaultDomain,outer,[delegate], False)
		End Function
	End Class

End Namespace
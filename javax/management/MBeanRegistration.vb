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


	''' <summary>
	''' <p>Can be implemented by an MBean in order to
	''' carry out operations before and after being registered or unregistered from
	''' the MBean Server.  An MBean can also implement this interface in order
	''' to get a reference to the MBean Server and/or its name within that
	''' MBean Server.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface MBeanRegistration


		''' <summary>
		''' Allows the MBean to perform any operations it needs before
		''' being registered in the MBean Server.  If the name of the MBean
		''' is not specified, the MBean can provide a name for its
		''' registration.  If any exception is raised, the MBean will not be
		''' registered in the MBean Server.
		''' </summary>
		''' <param name="server"> The MBean Server in which the MBean will be registered.
		''' </param>
		''' <param name="name"> The object name of the MBean.  This name is null if
		''' the name parameter to one of the <code>createMBean</code> or
		''' <code>registerMBean</code> methods in the <seealso cref="MBeanServer"/>
		''' interface is null.  In that case, this method must return a
		''' non-null ObjectName for the new MBean.
		''' </param>
		''' <returns> The name under which the MBean is to be registered.
		''' This value must not be null.  If the <code>name</code>
		''' parameter is not null, it will usually but not necessarily be
		''' the returned value.
		''' </returns>
		''' <exception cref="java.lang.Exception"> This exception will be caught by
		''' the MBean Server and re-thrown as an {@link
		''' MBeanRegistrationException}. </exception>
		Function preRegister(ByVal server As MBeanServer, ByVal name As ObjectName) As ObjectName

		''' <summary>
		''' Allows the MBean to perform any operations needed after having been
		''' registered in the MBean server or after the registration has failed.
		''' <p>If the implementation of this method throws a <seealso cref="RuntimeException"/>
		''' or an <seealso cref="Error"/>, the MBean Server will rethrow those inside
		''' a <seealso cref="RuntimeMBeanException"/> or <seealso cref="RuntimeErrorException"/>,
		''' respectively. However, throwing an exception in {@code postRegister}
		''' will not change the state of the MBean:
		''' if the MBean was already registered ({@code registrationDone} is
		''' {@code true}), the MBean will remain registered. </p>
		''' <p>This might be confusing for the code calling {@code createMBean()}
		''' or {@code registerMBean()}, as such code might assume that MBean
		''' registration has failed when such an exception is raised.
		''' Therefore it is recommended that implementations of
		''' {@code postRegister} do not throw Runtime Exceptions or Errors if it
		''' can be avoided.</p> </summary>
		''' <param name="registrationDone"> Indicates whether or not the MBean has
		''' been successfully registered in the MBean server. The value
		''' false means that the registration phase has failed. </param>
		Sub postRegister(ByVal registrationDone As Boolean?)

		''' <summary>
		''' Allows the MBean to perform any operations it needs before
		''' being unregistered by the MBean server.
		''' </summary>
		''' <exception cref="java.lang.Exception"> This exception will be caught by
		''' the MBean server and re-thrown as an {@link
		''' MBeanRegistrationException}. </exception>
		Sub preDeregister()

		''' <summary>
		''' Allows the MBean to perform any operations needed after having been
		''' unregistered in the MBean server.
		''' <p>If the implementation of this method throws a <seealso cref="RuntimeException"/>
		''' or an <seealso cref="Error"/>, the MBean Server will rethrow those inside
		''' a <seealso cref="RuntimeMBeanException"/> or <seealso cref="RuntimeErrorException"/>,
		''' respectively. However, throwing an exception in {@code postDeregister}
		''' will not change the state of the MBean:
		''' the MBean was already successfully deregistered and will remain so. </p>
		''' <p>This might be confusing for the code calling
		''' {@code unregisterMBean()}, as it might assume that MBean deregistration
		''' has failed. Therefore it is recommended that implementations of
		''' {@code postDeregister} do not throw Runtime Exceptions or Errors if it
		''' can be avoided.</p>
		''' </summary>
		Sub postDeregister()

	End Interface

End Namespace
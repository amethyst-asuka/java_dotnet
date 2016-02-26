Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.ldap





	''' <summary>
	''' This abstract class represents a factory for creating LDAPv3 controls.
	''' LDAPv3 controls are defined in
	''' <A HREF="http://www.ietf.org/rfc/rfc2251.txt">RFC 2251</A>.
	''' <p>
	''' When a service provider receives a response control, it uses control
	''' factories to return the specific/appropriate control class implementation.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Vincent Ryan
	''' </summary>
	''' <seealso cref= Control
	''' @since 1.3 </seealso>

	Public MustInherit Class ControlFactory
		''' <summary>
		''' Creates a new instance of a control factory.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Creates a control using this control factory.
		''' <p>
		''' The factory is used by the service provider to return controls
		''' that it reads from the LDAP protocol as specialized control classes.
		''' Without this mechanism, the provider would be returning
		''' controls that only contained data in BER encoded format.
		''' <p>
		''' Typically, <tt>ctl</tt> is a "basic" control containing
		''' BER encoded data. The factory is used to create a specialized
		''' control implementation, usually by decoding the BER encoded data,
		''' that provides methods to access that data in a type-safe and friendly
		''' manner.
		''' <p>
		''' For example, a factory might use the BER encoded data in
		''' basic control and return an instance of a VirtualListReplyControl.
		''' <p>
		''' If this factory cannot create a control using the argument supplied,
		''' it should return null.
		''' A factory should only throw an exception if it is sure that
		''' it is the only intended factory and that no other control factories
		''' should be tried. This might happen, for example, if the BER data
		''' in the control does not match what is expected of a control with
		''' the given OID. Since this method throws <tt>NamingException</tt>,
		''' any other internally generated exception that should be propagated
		''' must be wrapped inside a <tt>NamingException</tt>.
		''' </summary>
		''' <param name="ctl"> A non-null control.
		''' </param>
		''' <returns> A possibly null Control. </returns>
		''' <exception cref="NamingException"> If <tt>ctl</tt> contains invalid data that prevents it
		''' from being used to create a control. A factory should only throw
		''' an exception if it knows how to produce the control (identified by the OID)
		''' but is unable to because of, for example invalid BER data. </exception>
		Public MustOverride Function getControlInstance(ByVal ctl As Control) As Control

		''' <summary>
		''' Creates a control using known control factories.
		''' <p>
		''' The following rule is used to create the control:
		''' <ul>
		''' <li> Use the control factories specified in
		'''    the <tt>LdapContext.CONTROL_FACTORIES</tt> property of the
		'''    environment, and of the provider resource file associated with
		'''    <tt>ctx</tt>, in that order.
		'''    The value of this property is a colon-separated list of factory
		'''    class names that are tried in order, and the first one that succeeds
		'''    in creating the control is the one used.
		'''    If none of the factories can be loaded,
		'''    return <code>ctl</code>.
		'''    If an exception is encountered while creating the control, the
		'''    exception is passed up to the caller.
		''' </ul>
		''' <p>
		''' Note that a control factory
		''' must be public and must have a public constructor that accepts no arguments.
		''' <p> </summary>
		''' <param name="ctl"> The non-null control object containing the OID and BER data. </param>
		''' <param name="ctx"> The possibly null context in which the control is being created.
		''' If null, no such information is available. </param>
		''' <param name="env"> The possibly null environment of the context. This is used
		''' to find the value of the <tt>LdapContext.CONTROL_FACTORIES</tt> property. </param>
		''' <returns> A control object created using <code>ctl</code>; or
		'''         <code>ctl</code> if a control object cannot be created using
		'''         the algorithm described above. </returns>
		''' <exception cref="NamingException"> if a naming exception was encountered
		'''         while attempting to create the control object.
		'''         If one of the factories accessed throws an
		'''         exception, it is propagated up to the caller.
		''' If an error was encountered while loading
		''' and instantiating the factory and object classes, the exception
		''' is wrapped inside a <tt>NamingException</tt> and then rethrown. </exception>
		Public Shared Function getControlInstance(Of T1)(ByVal ctl As Control, ByVal ctx As javax.naming.Context, ByVal env As Dictionary(Of T1)) As Control

			' Get object factories list from environment properties or
			' provider resource file.
			Dim factories As com.sun.naming.internal.FactoryEnumeration = com.sun.naming.internal.ResourceManager.getFactories(LdapContext.CONTROL_FACTORIES, env, ctx)

			If factories Is Nothing Then Return ctl

			' Try each factory until one succeeds
			Dim answer As Control = Nothing
			Dim factory As ControlFactory
			Do While answer Is Nothing AndAlso factories.hasMore()
				factory = CType(factories.next(), ControlFactory)
				answer = factory.getControlInstance(ctl)
			Loop

			Return If(answer IsNot Nothing, answer, ctl)
		End Function
	End Class

End Namespace
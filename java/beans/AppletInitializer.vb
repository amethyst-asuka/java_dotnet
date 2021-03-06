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

Namespace java.beans



	''' <summary>
	''' <p>
	''' This interface is designed to work in collusion with java.beans.Beans.instantiate.
	''' The interface is intended to provide mechanism to allow the proper
	''' initialization of JavaBeans that are also Applets, during their
	''' instantiation by java.beans.Beans.instantiate().
	''' </p>
	''' </summary>
	''' <seealso cref= java.beans.Beans#instantiate
	''' 
	''' @since 1.2
	'''  </seealso>


	Public Interface AppletInitializer

		''' <summary>
		''' <p>
		''' If passed to the appropriate variant of java.beans.Beans.instantiate
		''' this method will be called in order to associate the newly instantiated
		''' Applet (JavaBean) with its AppletContext, AppletStub, and Container.
		''' </p>
		''' <p>
		''' Conformant implementations shall:
		''' <ol>
		''' <li> Associate the newly instantiated Applet with the appropriate
		''' AppletContext.
		''' 
		''' <li> Instantiate an AppletStub() and associate that AppletStub with
		''' the Applet via an invocation of setStub().
		''' 
		''' <li> If BeanContext parameter is null, then it shall associate the
		''' Applet with its appropriate Container by adding that Applet to its
		''' Container via an invocation of add(). If the BeanContext parameter is
		''' non-null, then it is the responsibility of the BeanContext to associate
		''' the Applet with its Container during the subsequent invocation of its
		''' addChildren() method.
		''' </ol>
		''' </summary>
		''' <param name="newAppletBean">  The newly instantiated JavaBean </param>
		''' <param name="bCtxt">          The BeanContext intended for this Applet, or
		'''                       null. </param>

		Sub initialize(  newAppletBean As java.applet.Applet,   bCtxt As java.beans.beancontext.BeanContext)

		''' <summary>
		''' <p>
		''' Activate, and/or mark Applet active. Implementors of this interface
		''' shall mark this Applet as active, and optionally invoke its start()
		''' method.
		''' </p>
		''' </summary>
		''' <param name="newApplet">  The newly instantiated JavaBean </param>

		Sub activate(  newApplet As java.applet.Applet)
	End Interface

End Namespace
'
' * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.spi


	''' <summary>
	''' Invoker hides the detail of calling into application endpoint
	''' implementation. Container hands over an implementation of Invoker
	''' to JAX-WS runtime, and jax-ws runtime calls <seealso cref="#invoke"/>
	''' for a web service invocation. Finally, Invoker does the actual
	''' invocation of web service on endpoint instance.
	''' 
	''' Container also injects the provided <code>WebServiceContext</code> and takes
	''' care of invoking <code>javax.annotation.PostConstruct</code> methods,
	''' if present, on the endpoint implementation.
	''' </summary>
	''' <seealso cref= Provider#createEndpoint(String, Class, Invoker, WebServiceFeature...)
	''' @author Jitendra Kotamraju
	''' @since JAX-WS 2.2 </seealso>

	Public MustInherit Class Invoker

		''' <summary>
		''' JAX-WS runtimes calls this method to ask container to inject
		''' WebServiceContext on the endpoint instance. The
		''' <code>WebServiceContext</code> object uses thread-local information
		''' to return the correct information during the actual endpoint invocation
		''' regardless of how many threads are concurrently being used to serve
		''' requests.
		''' </summary>
		''' <param name="webServiceContext"> a holder for MessageContext </param>
		''' <exception cref="IllegalAccessException"> if the injection done
		'''         by reflection API throws this exception </exception>
		''' <exception cref="IllegalArgumentException"> if the injection done
		'''         by reflection API throws this exception </exception>
		''' <exception cref="InvocationTargetException"> if the injection done
		'''         by reflection API throws this exception </exception>
		Public MustOverride Sub inject(ByVal webServiceContext As javax.xml.ws.WebServiceContext)

		''' <summary>
		''' JAX-WS runtime calls this method to do the actual web service
		''' invocation on endpoint instance. The injected
		''' <code>WebServiceContext.getMessageContext()</code> gives the correct
		''' information for this invocation.
		''' </summary>
		''' <param name="m"> Method to be invoked on the service </param>
		''' <param name="args"> Method arguments </param>
		''' <returns> return value of the method </returns>
		''' <exception cref="IllegalAccessException"> if the invocation done
		'''         by reflection API throws this exception </exception>
		''' <exception cref="IllegalArgumentException"> if the invocation done
		'''         by reflection API throws this exception </exception>
		''' <exception cref="InvocationTargetException"> if the invocation done
		'''         by reflection API throws this exception
		''' </exception>
		''' <seealso cref= Method#invoke </seealso>
		Public MustOverride Function invoke(ByVal m As Method, ParamArray ByVal args As Object()) As Object

	End Class

End Namespace
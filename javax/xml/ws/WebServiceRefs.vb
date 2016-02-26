'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws


	''' <summary>
	''' The <code>WebServiceRefs</code> annotation allows
	''' multiple web service references to be declared at the
	''' class level.
	''' 
	''' <p>
	''' It can be used to inject both service and proxy
	''' instances. These injected references are not thread safe.
	''' If the references are accessed by multiple threads,
	''' usual synchronization techniques can be used to
	''' support multiple threads.
	''' 
	''' <p>
	''' There is no way to associate web service features with
	''' the injected instances. If an instance needs to be
	''' configured with web service features, use @WebServiceRef
	''' to inject the resource along with its features.
	''' 
	''' <p>
	''' <b>Example</b>: The <code>StockQuoteProvider</code>
	''' proxy instance, and the <code>StockQuoteService</code> service
	''' instance are injected using @WebServiceRefs.
	''' 
	''' <pre><code>
	'''    &#64;WebServiceRefs({&#64;WebServiceRef(name="service/stockquoteservice", value=StockQuoteService.class),
	'''                     &#64;WebServiceRef(name="service/stockquoteprovider", type=StockQuoteProvider.class, value=StockQuoteService.class})
	'''    public class MyClient {
	'''        void init() {
	'''            Context ic = new InitialContext();
	'''            StockQuoteService service = (StockQuoteService) ic.lookup("java:comp/env/service/stockquoteservice");
	'''            StockQuoteProvider port = (StockQuoteProvider) ic.lookup("java:comp/env/service/stockquoteprovider");
	'''            ...
	'''       }
	'''       ...
	'''    }
	''' </code></pre>
	''' </summary>
	''' <seealso cref= WebServiceRef
	''' @since 2.0 </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class WebServiceRefs
		Inherits System.Attribute

	   ''' <summary>
	   ''' Array used for multiple web service reference declarations.
	   ''' </summary>
	   WebServiceRef() value()
	End Class

End Namespace
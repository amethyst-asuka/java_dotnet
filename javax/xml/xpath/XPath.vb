'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.xpath


	''' <summary>
	''' <p><code>XPath</code> provides access to the XPath evaluation environment and expressions.</p>
	''' 
	''' <a name="XPath-evaluation"/>
	''' <table border="1" cellpadding="2">
	'''   <thead>
	'''     <tr>
	'''       <th colspan="2">Evaluation of XPath Expressions.</th>
	'''     </tr>
	'''   </thead>
	'''   <tbody>
	'''     <tr>
	'''       <td>context</td>
	'''       <td>
	'''         If a request is made to evaluate the expression in the absence
	''' of a context item, an empty document node will be used for the context.
	''' For the purposes of evaluating XPath expressions, a DocumentFragment
	''' is treated like a Document node.
	'''      </td>
	'''    </tr>
	'''    <tr>
	'''      <td>variables</td>
	'''      <td>
	'''        If the expression contains a variable reference, its value will be found through the <seealso cref="XPathVariableResolver"/>
	'''        set with <seealso cref="#setXPathVariableResolver(XPathVariableResolver resolver)"/>.
	'''        An <seealso cref="XPathExpressionException"/> is raised if the variable resolver is undefined or
	'''        the resolver returns <code>null</code> for the variable.
	'''        The value of a variable must be immutable through the course of any single evaluation.</p>
	'''      </td>
	'''    </tr>
	'''    <tr>
	'''      <td>functions</td>
	'''      <td>
	'''        If the expression contains a function reference, the function will be found through the <seealso cref="XPathFunctionResolver"/>
	'''        set with <seealso cref="#setXPathFunctionResolver(XPathFunctionResolver resolver)"/>.
	'''        An <seealso cref="XPathExpressionException"/> is raised if the function resolver is undefined or
	'''        the function resolver returns <code>null</code> for the function.</p>
	'''      </td>
	'''    </tr>
	'''    <tr>
	'''      <td>QNames</td>
	'''      <td>
	'''        QNames in the expression are resolved against the XPath namespace context
	'''        set with <seealso cref="#setNamespaceContext(NamespaceContext nsContext)"/>.
	'''      </td>
	'''    </tr>
	'''    <tr>
	'''      <td>result</td>
	'''      <td>
	'''        This result of evaluating an expression is converted to an instance of the desired return type.
	'''        Valid return types are defined in <seealso cref="XPathConstants"/>.
	'''        Conversion to the return type follows XPath conversion rules.</p>
	'''      </td>
	'''    </tr>
	''' </table>
	''' 
	''' <p>An XPath object is not thread-safe and not reentrant.
	''' In other words, it is the application's responsibility to make
	''' sure that one <seealso cref="XPath"/> object is not used from
	''' more than one thread at any given time, and while the <code>evaluate</code>
	''' method is invoked, applications may not recursively call
	''' the <code>evaluate</code> method.
	''' <p>
	''' 
	''' @author  <a href="Norman.Walsh@Sun.com">Norman Walsh</a>
	''' @author  <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a> </summary>
	''' <seealso cref= <a href="http://www.w3.org/TR/xpath">XML Path Language (XPath) Version 1.0</a>
	''' @since 1.5 </seealso>
	Public Interface XPath

			''' <summary>
			''' <p>Reset this <code>XPath</code> to its original configuration.</p>
			''' 
			''' <p><code>XPath</code> is reset to the same state as when it was created with
			''' <seealso cref="XPathFactory#newXPath()"/>.
			''' <code>reset()</code> is designed to allow the reuse of existing <code>XPath</code>s
			''' thus saving resources associated with the creation of new <code>XPath</code>s.</p>
			''' 
			''' <p>The reset <code>XPath</code> is not guaranteed to have the same <seealso cref="XPathFunctionResolver"/>, <seealso cref="XPathVariableResolver"/>
			''' or <seealso cref="NamespaceContext"/> <code>Object</code>s, e.g. <seealso cref="Object#equals(Object obj)"/>.
			''' It is guaranteed to have a functionally equal <code>XPathFunctionResolver</code>, <code>XPathVariableResolver</code>
			''' and <code>NamespaceContext</code>.</p>
			''' </summary>
			Sub reset()

		''' <summary>
		''' <p>Establish a variable resolver.</p>
		''' 
		''' <p>A <code>NullPointerException</code> is thrown if <code>resolver</code> is <code>null</code>.</p>
		''' </summary>
		''' <param name="resolver"> Variable resolver.
		''' </param>
		'''  <exception cref="NullPointerException"> If <code>resolver</code> is <code>null</code>. </exception>
		Property xPathVariableResolver As XPathVariableResolver


		''' <summary>
		''' <p>Establish a function resolver.</p>
		'''   
		''' <p>A <code>NullPointerException</code> is thrown if <code>resolver</code> is <code>null</code>.</p>
		''' </summary>
		''' <param name="resolver"> XPath function resolver.
		''' </param>
		''' <exception cref="NullPointerException"> If <code>resolver</code> is <code>null</code>. </exception>
		Property xPathFunctionResolver As XPathFunctionResolver


		''' <summary>
		''' <p>Establish a namespace context.</p>
		'''   
		''' <p>A <code>NullPointerException</code> is thrown if <code>nsContext</code> is <code>null</code>.</p>
		''' </summary>
		''' <param name="nsContext"> Namespace context to use.
		''' </param>
		''' <exception cref="NullPointerException"> If <code>nsContext</code> is <code>null</code>. </exception>
		Property namespaceContext As javax.xml.namespace.NamespaceContext


		''' <summary>
		''' <p>Compile an XPath expression for later evaluation.</p>
		'''   
		''' <p>If <code>expression</code> contains any <seealso cref="XPathFunction"/>s,
		''' they must be available via the <seealso cref="XPathFunctionResolver"/>.
		''' An <seealso cref="XPathExpressionException"/> will be thrown if the
		''' <code>XPathFunction</code>
		''' cannot be resovled with the <code>XPathFunctionResolver</code>.</p>
		'''   
		''' <p>If <code>expression</code> contains any variables, the
		''' <seealso cref="XPathVariableResolver"/> in effect
		''' <strong>at compile time</strong> will be used to resolve them.</p>
		'''   
		''' <p>If <code>expression</code> is <code>null</code>, a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="expression"> The XPath expression.
		''' </param>
		''' <returns> Compiled XPath expression.
		''' </returns>
		''' <exception cref="XPathExpressionException"> If <code>expression</code> cannot be compiled. </exception>
		''' <exception cref="NullPointerException"> If <code>expression</code> is <code>null</code>. </exception>
		Function compile(ByVal expression As String) As XPathExpression

		''' <summary>
		''' <p>Evaluate an <code>XPath</code> expression in the specified context and return the result as the specified type.</p>
		''' 
		''' <p>See <a href="#XPath-evaluation">Evaluation of XPath Expressions</a> for context item evaluation,
		''' variable, function and <code>QName</code> resolution and return type conversion.</p>
		''' 
		''' <p>If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/> (
		''' <seealso cref="XPathConstants#NUMBER NUMBER"/>,
		''' <seealso cref="XPathConstants#STRING STRING"/>,
		''' <seealso cref="XPathConstants#BOOLEAN BOOLEAN"/>,
		''' <seealso cref="XPathConstants#NODE NODE"/> or
		''' <seealso cref="XPathConstants#NODESET NODESET"/>)
		''' then an <code>IllegalArgumentException</code> is thrown.</p>
		''' 
		''' <p>If a <code>null</code> value is provided for
		''' <code>item</code>, an empty document will be used for the
		''' context.
		''' If <code>expression</code> or <code>returnType</code> is <code>null</code>, then a
		''' <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="expression"> The XPath expression. </param>
		''' <param name="item"> The starting context (a node, for example). </param>
		''' <param name="returnType"> The desired return type.
		''' </param>
		''' <returns> Result of evaluating an XPath expression as an <code>Object</code> of <code>returnType</code>.
		''' </returns>
		''' <exception cref="XPathExpressionException"> If <code>expression</code> cannot be evaluated. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/>. </exception>
		''' <exception cref="NullPointerException"> If <code>expression</code> or <code>returnType</code> is <code>null</code>. </exception>
		Function evaluate(ByVal expression As String, ByVal item As Object, ByVal returnType As javax.xml.namespace.QName) As Object

		''' <summary>
		''' <p>Evaluate an XPath expression in the specified context and return the result as a <code>String</code>.</p>
		''' 
		''' <p>This method calls <seealso cref="#evaluate(String expression, Object item, QName returnType)"/> with a <code>returnType</code> of
		''' <seealso cref="XPathConstants#STRING"/>.</p>
		''' 
		''' <p>See <a href="#XPath-evaluation">Evaluation of XPath Expressions</a> for context item evaluation,
		''' variable, function and QName resolution and return type conversion.</p>
		''' 
		''' <p>If a <code>null</code> value is provided for
		''' <code>item</code>, an empty document will be used for the
		''' context.
		''' If <code>expression</code> is <code>null</code>, then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="expression"> The XPath expression. </param>
		''' <param name="item"> The starting context (a node, for example).
		''' </param>
		''' <returns> The <code>String</code> that is the result of evaluating the expression and
		'''   converting the result to a <code>String</code>.
		''' </returns>
		''' <exception cref="XPathExpressionException"> If <code>expression</code> cannot be evaluated. </exception>
		''' <exception cref="NullPointerException"> If <code>expression</code> is <code>null</code>. </exception>
		Function evaluate(ByVal expression As String, ByVal item As Object) As String

		''' <summary>
		''' <p>Evaluate an XPath expression in the context of the specified <code>InputSource</code>
		''' and return the result as the specified type.</p>
		''' 
		''' <p>This method builds a data model for the <seealso cref="InputSource"/> and calls
		''' <seealso cref="#evaluate(String expression, Object item, QName returnType)"/> on the resulting document object.</p>
		''' 
		''' <p>See <a href="#XPath-evaluation">Evaluation of XPath Expressions</a> for context item evaluation,
		''' variable, function and QName resolution and return type conversion.</p>
		''' 
		''' <p>If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/>,
		''' then an <code>IllegalArgumentException</code> is thrown.</p>
		''' 
		''' <p>If <code>expression</code>, <code>source</code> or <code>returnType</code> is <code>null</code>,
		''' then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="expression"> The XPath expression. </param>
		''' <param name="source"> The input source of the document to evaluate over. </param>
		''' <param name="returnType"> The desired return type.
		''' </param>
		''' <returns> The <code>Object</code> that encapsulates the result of evaluating the expression.
		''' </returns>
		''' <exception cref="XPathExpressionException"> If expression cannot be evaluated. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/>. </exception>
		''' <exception cref="NullPointerException"> If <code>expression</code>, <code>source</code> or <code>returnType</code>
		'''   is <code>null</code>. </exception>
		Function evaluate(ByVal expression As String, ByVal source As org.xml.sax.InputSource, ByVal returnType As javax.xml.namespace.QName) As Object

		''' <summary>
		''' <p>Evaluate an XPath expression in the context of the specified <code>InputSource</code>
		''' and return the result as a <code>String</code>.</p>
		''' 
		''' <p>This method calls <seealso cref="#evaluate(String expression, InputSource source, QName returnType)"/> with a
		''' <code>returnType</code> of <seealso cref="XPathConstants#STRING"/>.</p>
		''' 
		''' <p>See <a href="#XPath-evaluation">Evaluation of XPath Expressions</a> for context item evaluation,
		''' variable, function and QName resolution and return type conversion.</p>
		''' 
		''' <p>If <code>expression</code> or <code>source</code> is <code>null</code>,
		''' then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="expression"> The XPath expression. </param>
		''' <param name="source"> The <code>InputSource</code> of the document to evaluate over.
		''' </param>
		''' <returns> The <code>String</code> that is the result of evaluating the expression and
		'''   converting the result to a <code>String</code>.
		''' </returns>
		''' <exception cref="XPathExpressionException"> If expression cannot be evaluated. </exception>
		''' <exception cref="NullPointerException"> If <code>expression</code> or <code>source</code> is <code>null</code>. </exception>
		Function evaluate(ByVal expression As String, ByVal source As org.xml.sax.InputSource) As String
	End Interface

End Namespace
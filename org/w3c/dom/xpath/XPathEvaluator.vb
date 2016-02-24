'
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2002 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.xpath



	''' <summary>
	'''  The evaluation of XPath expressions is provided by
	''' <code>XPathEvaluator</code>. In a DOM implementation which supports the
	''' XPath 3.0 feature, as described above, the <code>XPathEvaluator</code>
	''' interface will be implemented on the same object which implements the
	''' <code>Document</code> interface permitting it to be obtained by the usual
	''' binding-specific method such as casting or by using the DOM Level 3
	''' getInterface method. In this case the implementation obtained from the
	''' Document supports the XPath DOM module and is compatible with the XPath
	''' 1.0 specification.
	''' <p>Evaluation of expressions with specialized extension functions or
	''' variables may not work in all implementations and is, therefore, not
	''' portable. <code>XPathEvaluator</code> implementations may be available
	''' from other sources that could provide specific support for specialized
	''' extension functions or variables as would be defined by other
	''' specifications.
	''' <p>See also the <a href='http://www.w3.org/2002/08/WD-DOM-Level-3-XPath-20020820'>Document Object Model (DOM) Level 3 XPath Specification</a>.
	''' </summary>
	Public Interface XPathEvaluator
		''' <summary>
		''' Creates a parsed XPath expression with resolved namespaces. This is
		''' useful when an expression will be reused in an application since it
		''' makes it possible to compile the expression string into a more
		''' efficient internal form and preresolve all namespace prefixes which
		''' occur within the expression. </summary>
		''' <param name="expression"> The XPath expression string to be parsed. </param>
		''' <param name="resolver"> The <code>resolver</code> permits translation of
		'''   prefixes within the XPath expression into appropriate namespace URIs
		'''   . If this is specified as <code>null</code>, any namespace prefix
		'''   within the expression will result in <code>DOMException</code>
		'''   being thrown with the code <code>NAMESPACE_ERR</code>. </param>
		''' <returns> The compiled form of the XPath expression. </returns>
		''' <exception cref="XPathException">
		'''   INVALID_EXPRESSION_ERR: Raised if the expression is not legal
		'''   according to the rules of the <code>XPathEvaluator</code>i </exception>
		''' <exception cref="DOMException">
		'''   NAMESPACE_ERR: Raised if the expression contains namespace prefixes
		'''   which cannot be resolved by the specified
		'''   <code>XPathNSResolver</code>. </exception>
		Function createExpression(ByVal expression As String, ByVal resolver As XPathNSResolver) As XPathExpression

		''' <summary>
		''' Adapts any DOM node to resolve namespaces so that an XPath expression
		''' can be easily evaluated relative to the context of the node where it
		''' appeared within the document. This adapter works like the DOM Level 3
		''' method <code>lookupNamespaceURI</code> on nodes in resolving the
		''' namespaceURI from a given prefix using the current information
		''' available in the node's hierarchy at the time lookupNamespaceURI is
		''' called. also correctly resolving the implicit xml prefix. </summary>
		''' <param name="nodeResolver"> The node to be used as a context for namespace
		'''   resolution. </param>
		''' <returns> <code>XPathNSResolver</code> which resolves namespaces with
		'''   respect to the definitions in scope for a specified node. </returns>
		Function createNSResolver(ByVal nodeResolver As org.w3c.dom.Node) As XPathNSResolver

		''' <summary>
		''' Evaluates an XPath expression string and returns a result of the
		''' specified type if possible. </summary>
		''' <param name="expression"> The XPath expression string to be parsed and
		'''   evaluated. </param>
		''' <param name="contextNode"> The <code>context</code> is context node for the
		'''   evaluation of this XPath expression. If the XPathEvaluator was
		'''   obtained by casting the <code>Document</code> then this must be
		'''   owned by the same document and must be a <code>Document</code>,
		'''   <code>Element</code>, <code>Attribute</code>, <code>Text</code>,
		'''   <code>CDATASection</code>, <code>Comment</code>,
		'''   <code>ProcessingInstruction</code>, or <code>XPathNamespace</code>
		'''   node. If the context node is a <code>Text</code> or a
		'''   <code>CDATASection</code>, then the context is interpreted as the
		'''   whole logical text node as seen by XPath, unless the node is empty
		'''   in which case it may not serve as the XPath context. </param>
		''' <param name="resolver"> The <code>resolver</code> permits translation of
		'''   prefixes within the XPath expression into appropriate namespace URIs
		'''   . If this is specified as <code>null</code>, any namespace prefix
		'''   within the expression will result in <code>DOMException</code>
		'''   being thrown with the code <code>NAMESPACE_ERR</code>. </param>
		''' <param name="type"> If a specific <code>type</code> is specified, then the
		'''   result will be returned as the corresponding type.For XPath 1.0
		'''   results, this must be one of the codes of the
		'''   <code>XPathResult</code> interface. </param>
		''' <param name="result"> The <code>result</code> specifies a specific result
		'''   object which may be reused and returned by this method. If this is
		'''   specified as <code>null</code>or the implementation does not reuse
		'''   the specified result, a new result object will be constructed and
		'''   returned.For XPath 1.0 results, this object will be of type
		'''   <code>XPathResult</code>. </param>
		''' <returns> The result of the evaluation of the XPath expression.For XPath
		'''   1.0 results, this object will be of type <code>XPathResult</code>. </returns>
		''' <exception cref="XPathException">
		'''   INVALID_EXPRESSION_ERR: Raised if the expression is not legal
		'''   according to the rules of the <code>XPathEvaluator</code>i
		'''   <br>TYPE_ERR: Raised if the result cannot be converted to return the
		'''   specified type. </exception>
		''' <exception cref="DOMException">
		'''   NAMESPACE_ERR: Raised if the expression contains namespace prefixes
		'''   which cannot be resolved by the specified
		'''   <code>XPathNSResolver</code>.
		'''   <br>WRONG_DOCUMENT_ERR: The Node is from a document that is not
		'''   supported by this <code>XPathEvaluator</code>.
		'''   <br>NOT_SUPPORTED_ERR: The Node is not a type permitted as an XPath
		'''   context node or the request type is not permitted by this
		'''   <code>XPathEvaluator</code>. </exception>
		Function evaluate(ByVal expression As String, ByVal contextNode As org.w3c.dom.Node, ByVal resolver As XPathNSResolver, ByVal type As Short, ByVal result As Object) As Object

	End Interface

End Namespace
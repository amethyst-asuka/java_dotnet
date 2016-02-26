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
	''' <p><code>XPathVariableResolver</code> provides access to the set of user defined XPath variables.</p>
	''' 
	''' <p>The <code>XPathVariableResolver</code> and the XPath evaluator must adhere to a contract that
	''' cannot be directly enforced by the API.  Although variables may be mutable,
	''' that is, an application may wish to evaluate the same XPath expression more
	''' than once with different variable values, in the course of evaluating any
	''' single XPath expression, a variable's value <strong><em>must</em></strong>
	''' not change.</p>
	''' 
	''' @author  <a href="mailto:Norman.Walsh@Sun.com">Norman Walsh</a>
	''' @author  <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @since 1.5
	''' </summary>
	Public Interface XPathVariableResolver
	  ''' <summary>
	  ''' <p>Find a variable in the set of available variables.</p>
	  ''' 
	  ''' <p>If <code>variableName</code> is <code>null</code>, then a <code>NullPointerException</code> is thrown.</p>
	  ''' </summary>
	  ''' <param name="variableName"> The <code>QName</code> of the variable name.
	  ''' </param>
	  ''' <returns> The variables value, or <code>null</code> if no variable named <code>variableName</code>
	  '''   exists.  The value returned must be of a type appropriate for the underlying object model.
	  ''' </returns>
	  ''' <exception cref="NullPointerException"> If <code>variableName</code> is <code>null</code>. </exception>
	  Function resolveVariable(ByVal variableName As javax.xml.namespace.QName) As Object
	End Interface

End Namespace
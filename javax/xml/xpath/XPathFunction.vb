Imports System.Collections

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
	''' <p><code>XPathFunction</code> provides access to XPath functions.</p>
	''' 
	''' <p>Functions are identified by QName and arity in XPath.</p>
	''' 
	''' @author  <a href="mailto:Norman.Walsh@Sun.com">Norman Walsh</a>
	''' @author  <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @since 1.5
	''' </summary>
	Public Interface XPathFunction
	  ''' <summary>
	  ''' <p>Evaluate the function with the specified arguments.</p>
	  ''' 
	  ''' <p>To the greatest extent possible, side-effects should be avoided in the
	  ''' definition of extension functions. The implementation evaluating an
	  ''' XPath expression is under no obligation to call extension functions in
	  ''' any particular order or any particular number of times.</p>
	  ''' </summary>
	  ''' <param name="args"> The arguments, <code>null</code> is a valid value.
	  ''' </param>
	  ''' <returns> The result of evaluating the <code>XPath</code> function as an <code>Object</code>.
	  ''' </returns>
	  ''' <exception cref="XPathFunctionException"> If <code>args</code> cannot be evaluated with this <code>XPath</code> function. </exception>
	  Function evaluate(ByVal args As IList) As Object
	End Interface

End Namespace
Imports System

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
	''' <code>XPathFactoryConfigurationException</code> represents a configuration error in a <code>XPathFactory</code> environment.</p>
	''' 
	''' @author  <a href="mailto:Norman.Walsh@Sun.com">Norman Walsh</a>
	''' @author  <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @since 1.5
	''' </summary>
	Public Class XPathFactoryConfigurationException
		Inherits XPathException

		''' <summary>
		''' <p>Stream Unique Identifier.</p>
		''' </summary>
		Private Const serialVersionUID As Long = -1837080260374986980L

		''' <summary>
		''' <p>Constructs a new <code>XPathFactoryConfigurationException</code> with the specified detail <code>message</code>.</p>
		''' 
		''' <p>The <code>cause</code> is not initialized.</p>
		''' 
		''' <p>If <code>message</code> is <code>null</code>,
		''' then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="message"> The detail message.
		''' </param>
		''' <exception cref="NullPointerException"> When <code>message</code> is
		'''   <code>null</code>. </exception>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' <p>Constructs a new <code>XPathFactoryConfigurationException</code>
		''' with the specified <code>cause</code>.</p>
		''' 
		''' <p>If <code>cause</code> is <code>null</code>,
		''' then a <code>NullPointerException</code> is thrown.</p>
		''' </summary>
		''' <param name="cause"> The cause.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>cause</code> is <code>null</code>. </exception>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace
Imports System.Collections

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: NodeSetData.java,v 1.5 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto


	''' <summary>
	''' An abstract representation of a <code>Data</code> type containing a
	''' node-set. The type (class) and ordering of the nodes contained in the set
	''' are not defined by this class; instead that behavior should be
	''' defined by <code>NodeSetData</code> subclasses.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Interface NodeSetData
		Inherits Data

		''' <summary>
		''' Returns a read-only iterator over the nodes contained in this
		''' <code>NodeSetData</code> in
		''' <a href="http://www.w3.org/TR/1999/REC-xpath-19991116#dt-document-order">
		''' document order</a>. Attempts to modify the returned iterator
		''' via the <code>remove</code> method throw
		''' <code>UnsupportedOperationException</code>.
		''' </summary>
		''' <returns> an <code>Iterator</code> over the nodes in this
		'''    <code>NodeSetData</code> in document order </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Function [iterator]() As IEnumerator
	End Interface

End Namespace
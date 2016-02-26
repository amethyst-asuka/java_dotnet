'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform.dom




	''' <summary>
	''' Indicates the position of a node in a source DOM, intended
	''' primarily for error reporting.  To use a DOMLocator, the receiver of an
	''' error must downcast the <seealso cref="javax.xml.transform.SourceLocator"/>
	''' object returned by an exception. A <seealso cref="javax.xml.transform.Transformer"/>
	''' may use this object for purposes other than error reporting, for instance,
	''' to indicate the source node that originated a result node.
	''' </summary>
	Public Interface DOMLocator
		Inherits javax.xml.transform.SourceLocator

		''' <summary>
		''' Return the node where the event occurred.
		''' </summary>
		''' <returns> The node that is the location for the event. </returns>
		ReadOnly Property originatingNode As org.w3c.dom.Node
	End Interface

End Namespace
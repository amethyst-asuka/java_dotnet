'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap

	''' <summary>
	''' A representation of a node (element) in an XML document.
	''' This interface extnends the standard DOM Node interface with methods for
	''' getting and setting the value of a node, for
	''' getting and setting the parent of a node, and for removing a node.
	''' </summary>
	Public Interface Node
		Inherits org.w3c.dom.Node

		''' <summary>
		''' Returns the value of this node if this is a <code>Text</code> node or the
		''' value of the immediate child of this node otherwise.
		''' If there is an immediate child of this <code>Node</code> that it is a
		''' <code>Text</code> node then it's value will be returned. If there is
		''' more than one <code>Text</code> node then the value of the first
		''' <code>Text</code> Node will be returned.
		''' Otherwise <code>null</code> is returned.
		''' </summary>
		''' <returns> a <code>String</code> with the text of this node if this is a
		'''          <code>Text</code> node or the text contained by the first
		'''          immediate child of this <code>Node</code> object that is a
		'''          <code>Text</code> object if such a child exists;
		'''          <code>null</code> otherwise. </returns>
		Property value As String


		''' <summary>
		''' Sets the parent of this <code>Node</code> object to the given
		''' <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="parent"> the <code>SOAPElement</code> object to be set as
		'''       the parent of this <code>Node</code> object
		''' </param>
		''' <exception cref="SOAPException"> if there is a problem in setting the
		'''                          parent to the given element </exception>
		''' <seealso cref= #getParentElement </seealso>
		Property parentElement As SOAPElement


		''' <summary>
		''' Removes this <code>Node</code> object from the tree.
		''' </summary>
		Sub detachNode()

		''' <summary>
		''' Notifies the implementation that this <code>Node</code>
		''' object is no longer being used by the application and that the
		''' implementation is free to reuse this object for nodes that may
		''' be created later.
		''' <P>
		''' Calling the method <code>recycleNode</code> implies that the method
		''' <code>detachNode</code> has been called previously.
		''' </summary>
		Sub recycleNode()

	End Interface

End Namespace
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
	''' A representation of a node whose value is text.  A <code>Text</code> object
	''' may represent text that is content or text that is a comment.
	''' 
	''' </summary>
	Public Interface Text
		Inherits Node, org.w3c.dom.Text

		''' <summary>
		''' Retrieves whether this <code>Text</code> object represents a comment.
		''' </summary>
		''' <returns> <code>true</code> if this <code>Text</code> object is a
		'''         comment; <code>false</code> otherwise </returns>
		ReadOnly Property comment As Boolean
	End Interface

End Namespace
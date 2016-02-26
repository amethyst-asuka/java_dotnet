'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text





	''' <summary>
	''' A collection of attributes to associate with an element in a document.
	''' Since these are typically used to associate character and paragraph
	''' styles with the element, operations for this are provided.  Other
	''' customized attributes that get associated with the element will
	''' effectively be name-value pairs that live in a hierarchy and if a name
	''' (key) is not found locally, the request is forwarded to the parent.
	''' Commonly used attributes are separated out to facilitate alternative
	''' implementations that are more efficient.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface Style
		Inherits MutableAttributeSet

		''' <summary>
		''' Fetches the name of the style.   A style is not required to be named,
		''' so <code>null</code> is returned if there is no name
		''' associated with the style.
		''' </summary>
		''' <returns> the name </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Adds a listener to track whenever an attribute
		''' has been changed.
		''' </summary>
		''' <param name="l"> the change listener </param>
		Sub addChangeListener(ByVal l As javax.swing.event.ChangeListener)

		''' <summary>
		''' Removes a listener that was tracking attribute changes.
		''' </summary>
		''' <param name="l"> the change listener </param>
		Sub removeChangeListener(ByVal l As javax.swing.event.ChangeListener)


	End Interface

End Namespace
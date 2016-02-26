Imports javax.swing.text

'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.accessibility



	''' <summary>
	''' Encapsulation of a link, or set of links (e.g. client side imagemap)
	''' in a Hypertext document
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleText </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText
	''' 
	''' @author      Peter Korn </seealso>
	Public MustInherit Class AccessibleHyperlink
		Implements AccessibleAction

			''' <summary>
			''' Since the document a link is associated with may have
			''' changed, this method returns whether or not this Link is still valid
			''' (with respect to the document it references).
			''' </summary>
			''' <returns> a flag indicating whether this link is still valid with
			'''         respect to the AccessibleHypertext it belongs to </returns>
			Public MustOverride ReadOnly Property valid As Boolean

			''' <summary>
			''' Returns the number of accessible actions available in this Link
			''' If there are more than one, the first one is NOT considered the
			''' "default" action of this LINK object (e.g. in an HTML imagemap).
			''' In general, links will have only one AccessibleAction in them.
			''' </summary>
			''' <returns> the zero-based number of Actions in this object </returns>
			Public MustOverride ReadOnly Property accessibleActionCount As Integer Implements AccessibleAction.getAccessibleActionCount

			''' <summary>
			''' Performs the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the action was performed; otherwise false. </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public MustOverride Function doAccessibleAction(ByVal i As Integer) As Boolean Implements AccessibleAction.doAccessibleAction

			''' <summary>
			''' Returns a String description of this particular
			''' link action.  This should be a text string
			''' associated with anchoring text, this should be the
			''' anchor text.  E.g. from HTML:
			'''   &lt;a HREF="http://www.sun.com/access"&gt;Accessibility&lt;/a&gt;
			''' this method would return "Accessibility".
			''' 
			''' Similarly, from this HTML:
			'''   &lt;a HREF="#top"&gt;&lt;img src="top-hat.gif" alt="top hat"&gt;&lt;/a&gt;
			''' this method would return "top hat"
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			''' <returns> a String description of the action </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public MustOverride Function getAccessibleActionDescription(ByVal i As Integer) As String Implements AccessibleAction.getAccessibleActionDescription

			''' <summary>
			''' Returns an object that represents the link action,
			''' as appropriate for that link.  E.g. from HTML:
			'''   &lt;a HREF="http://www.sun.com/access"&gt;Accessibility&lt;/a&gt;
			''' this method would return a
			''' java.net.URL("http://www.sun.com/access.html");
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			''' <returns> an Object representing the hypertext link itself </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public MustOverride Function getAccessibleActionObject(ByVal i As Integer) As Object

			''' <summary>
			''' Returns an object that represents the link anchor,
			''' as appropriate for that link.  E.g. from HTML:
			'''   &lt;a href="http://www.sun.com/access"&gt;Accessibility&lt;/a&gt;
			''' this method would return a String containing the text:
			''' "Accessibility".
			''' 
			''' Similarly, from this HTML:
			'''   &lt;a HREF="#top"&gt;&lt;img src="top-hat.gif" alt="top hat"&gt;&lt;/a&gt;
			''' this might return the object ImageIcon("top-hat.gif", "top hat");
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			''' <returns> an Object representing the hypertext anchor </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public MustOverride Function getAccessibleActionAnchor(ByVal i As Integer) As Object

			''' <summary>
			''' Gets the index with the hypertext document at which this
			''' link begins
			''' </summary>
			''' <returns> index of start of link </returns>
			Public MustOverride ReadOnly Property startIndex As Integer

			''' <summary>
			''' Gets the index with the hypertext document at which this
			''' link ends
			''' </summary>
			''' <returns> index of end of link </returns>
			Public MustOverride ReadOnly Property endIndex As Integer
	End Class

End Namespace
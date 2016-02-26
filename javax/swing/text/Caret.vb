'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A place within a document view that represents where
	''' things can be inserted into the document model.  A caret
	''' has a position in the document referred to as a dot.
	''' The dot is where the caret is currently located in the
	''' model.  There is
	''' a second position maintained by the caret that represents
	''' the other end of a selection called mark.  If there is
	''' no selection the dot and mark will be equal.  If a selection
	''' exists, the two values will be different.
	''' <p>
	''' The dot can be placed by either calling
	''' <code>setDot</code> or <code>moveDot</code>.  Setting
	''' the dot has the effect of removing any selection that may
	''' have previously existed.  The dot and mark will be equal.
	''' Moving the dot has the effect of creating a selection as
	''' the mark is left at whatever position it previously had.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface Caret

		''' <summary>
		''' Called when the UI is being installed into the
		''' interface of a JTextComponent.  This can be used
		''' to gain access to the model that is being navigated
		''' by the implementation of this interface.
		''' </summary>
		''' <param name="c"> the JTextComponent </param>
		Sub install(ByVal c As JTextComponent)

		''' <summary>
		''' Called when the UI is being removed from the
		''' interface of a JTextComponent.  This is used to
		''' unregister any listeners that were attached.
		''' </summary>
		''' <param name="c"> the JTextComponent </param>
		Sub deinstall(ByVal c As JTextComponent)

		''' <summary>
		''' Renders the caret. This method is called by UI classes.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		Sub paint(ByVal g As java.awt.Graphics)

		''' <summary>
		''' Adds a listener to track whenever the caret position
		''' has been changed.
		''' </summary>
		''' <param name="l"> the change listener </param>
		Sub addChangeListener(ByVal l As javax.swing.event.ChangeListener)

		''' <summary>
		''' Removes a listener that was tracking caret position changes.
		''' </summary>
		''' <param name="l"> the change listener </param>
		Sub removeChangeListener(ByVal l As javax.swing.event.ChangeListener)

		''' <summary>
		''' Determines if the caret is currently visible.
		''' </summary>
		''' <returns> true if the caret is visible else false </returns>
		Property visible As Boolean


		''' <summary>
		''' Determines if the selection is currently visible.
		''' </summary>
		''' <returns> true if the caret is visible else false </returns>
		Property selectionVisible As Boolean


		''' <summary>
		''' Set the current caret visual location.  This can be used when
		''' moving between lines that have uneven end positions (such as
		''' when caret up or down actions occur).  If text flows
		''' left-to-right or right-to-left the x-coordinate will indicate
		''' the desired navigation location for vertical movement.  If
		''' the text flow is top-to-bottom, the y-coordinate will indicate
		''' the desired navigation location for horizontal movement.
		''' </summary>
		''' <param name="p">  the Point to use for the saved position.  This
		'''   can be null to indicate there is no visual location. </param>
		Property magicCaretPosition As java.awt.Point


		''' <summary>
		''' Sets the blink rate of the caret.  This determines if
		''' and how fast the caret blinks, commonly used as one
		''' way to attract attention to the caret.
		''' </summary>
		''' <param name="rate">  the delay in milliseconds &gt;=0.  If this is
		'''  zero the caret will not blink. </param>
		Property blinkRate As Integer


		''' <summary>
		''' Fetches the current position of the caret.
		''' </summary>
		''' <returns> the position &gt;=0 </returns>
		Property dot As Integer

		''' <summary>
		''' Fetches the current position of the mark.  If there
		''' is a selection, the mark will not be the same as
		''' the dot.
		''' </summary>
		''' <returns> the position &gt;=0 </returns>
		ReadOnly Property mark As Integer


		''' <summary>
		''' Moves the caret position (dot) to some other position,
		''' leaving behind the mark.  This is useful for
		''' making selections.
		''' </summary>
		''' <param name="dot">  the new position to move the caret to &gt;=0 </param>
		Sub moveDot(ByVal dot As Integer)

	End Interface

End Namespace
'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.event


	''' <summary>
	''' A semantic event which indicates that an item was selected or deselected.
	''' This high-level event is generated by an ItemSelectable object (such as a
	''' List) when an item is selected or deselected by the user.
	''' The event is passed to every <code>ItemListener</code> object which
	''' registered to receive such events using the component's
	''' <code>addItemListener</code> method.
	''' <P>
	''' The object that implements the <code>ItemListener</code> interface gets
	''' this <code>ItemEvent</code> when the event occurs. The listener is
	''' spared the details of processing individual mouse movements and mouse
	''' clicks, and can instead process a "meaningful" (semantic) event like
	''' "item selected" or "item deselected".
	''' <p>
	''' An unspecified behavior will be caused if the {@code id} parameter
	''' of any particular {@code ItemEvent} instance is not
	''' in the range from {@code ITEM_FIRST} to {@code ITEM_LAST}.
	''' <p>
	''' The {@code stateChange} of any {@code ItemEvent} instance takes one of the following
	''' values:
	'''                     <ul>
	'''                     <li> {@code ItemEvent.SELECTED}
	'''                     <li> {@code ItemEvent.DESELECTED}
	'''                     </ul>
	''' Assigning the value different from listed above will cause an unspecified behavior.
	''' 
	''' @author Carl Quinn
	''' </summary>
	''' <seealso cref= java.awt.ItemSelectable </seealso>
	''' <seealso cref= ItemListener </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/itemlistener.html">Tutorial: Writing an Item Listener</a>
	''' 
	''' @since 1.1 </seealso>
	Public Class ItemEvent
		Inherits java.awt.AWTEvent

		''' <summary>
		''' The first number in the range of ids used for item events.
		''' </summary>
		Public Const ITEM_FIRST As Integer = 701

		''' <summary>
		''' The last number in the range of ids used for item events.
		''' </summary>
		Public Const ITEM_LAST As Integer = 701

		''' <summary>
		''' This event id indicates that an item's state changed.
		''' </summary>
		Public Const ITEM_STATE_CHANGED As Integer = ITEM_FIRST 'Event.LIST_SELECT

		''' <summary>
		''' This state-change value indicates that an item was selected.
		''' </summary>
		Public Const SELECTED As Integer = 1

		''' <summary>
		''' This state-change-value indicates that a selected item was deselected.
		''' </summary>
		Public Const DESELECTED As Integer = 2

		''' <summary>
		''' The item whose selection state has changed.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getItem() </seealso>
		Friend item As Object

		''' <summary>
		''' <code>stateChange</code> indicates whether the <code>item</code>
		''' was selected or deselected.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getStateChange() </seealso>
		Friend stateChange As Integer

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -608708132447206933L

		''' <summary>
		''' Constructs an <code>ItemEvent</code> object.
		''' <p> This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> The <code>ItemSelectable</code> object
		'''               that originated the event </param>
		''' <param name="id">           The integer that identifies the event type.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="ItemEvent"/> </param>
		''' <param name="item">   An object -- the item affected by the event </param>
		''' <param name="stateChange">  An integer that indicates whether the item was
		'''               selected or deselected.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="ItemEvent"/> </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getItemSelectable() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getStateChange() </seealso>
		Public Sub New(  source As java.awt.ItemSelectable,   id As Integer,   item As Object,   stateChange As Integer)
			MyBase.New(source, id)
			Me.item = item
			Me.stateChange = stateChange
		End Sub

		''' <summary>
		''' Returns the originator of the event.
		''' </summary>
		''' <returns> the ItemSelectable object that originated the event. </returns>
		Public Overridable Property itemSelectable As java.awt.ItemSelectable
			Get
				Return CType(source, java.awt.ItemSelectable)
			End Get
		End Property

	   ''' <summary>
	   ''' Returns the item affected by the event.
	   ''' </summary>
	   ''' <returns> the item (object) that was affected by the event </returns>
		Public Overridable Property item As Object
			Get
				Return item
			End Get
		End Property

	   ''' <summary>
	   ''' Returns the type of state change (selected or deselected).
	   ''' </summary>
	   ''' <returns> an integer that indicates whether the item was selected
	   '''         or deselected
	   ''' </returns>
	   ''' <seealso cref= #SELECTED </seealso>
	   ''' <seealso cref= #DESELECTED </seealso>
		Public Overridable Property stateChange As Integer
			Get
				Return stateChange
			End Get
		End Property

		''' <summary>
		''' Returns a parameter string identifying this item event.
		''' This method is useful for event-logging and for debugging.
		''' </summary>
		''' <returns> a string identifying the event and its attributes </returns>
		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case ITEM_STATE_CHANGED
				  typeStr = "ITEM_STATE_CHANGED"
			  Case Else
				  typeStr = "unknown type"
			End Select

			Dim stateStr As String
			Select Case stateChange
			  Case SELECTED
				  stateStr = "SELECTED"
			  Case DESELECTED
				  stateStr = "DESELECTED"
			  Case Else
				  stateStr = "unknown type"
			End Select
			Return typeStr & ",item=" & item & ",stateChange=" & stateStr
		End Function

	End Class

End Namespace
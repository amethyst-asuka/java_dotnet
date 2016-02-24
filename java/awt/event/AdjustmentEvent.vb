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
	''' The adjustment event emitted by Adjustable objects like
	''' <seealso cref="java.awt.Scrollbar"/> and <seealso cref="java.awt.ScrollPane"/>.
	''' When the user changes the value of the scrolling component,
	''' it receives an instance of {@code AdjustmentEvent}.
	''' <p>
	''' An unspecified behavior will be caused if the {@code id} parameter
	''' of any particular {@code AdjustmentEvent} instance is not
	''' in the range from {@code ADJUSTMENT_FIRST} to {@code ADJUSTMENT_LAST}.
	''' <p>
	''' The {@code type} of any {@code AdjustmentEvent} instance takes one of the following
	''' values:
	'''                     <ul>
	'''                     <li> {@code UNIT_INCREMENT}
	'''                     <li> {@code UNIT_DECREMENT}
	'''                     <li> {@code BLOCK_INCREMENT}
	'''                     <li> {@code BLOCK_DECREMENT}
	'''                     <li> {@code TRACK}
	'''                     </ul>
	''' Assigning the value different from listed above will cause an unspecified behavior. </summary>
	''' <seealso cref= java.awt.Adjustable </seealso>
	''' <seealso cref= AdjustmentListener
	''' 
	''' @author Amy Fowler
	''' @since 1.1 </seealso>
	Public Class AdjustmentEvent
		Inherits java.awt.AWTEvent

		''' <summary>
		''' Marks the first integer id for the range of adjustment event ids.
		''' </summary>
		Public Const ADJUSTMENT_FIRST As Integer = 601

		''' <summary>
		''' Marks the last integer id for the range of adjustment event ids.
		''' </summary>
		Public Const ADJUSTMENT_LAST As Integer = 601

		''' <summary>
		''' The adjustment value changed event.
		''' </summary>
		Public Const ADJUSTMENT_VALUE_CHANGED As Integer = ADJUSTMENT_FIRST 'Event.SCROLL_LINE_UP

		''' <summary>
		''' The unit increment adjustment type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const UNIT_INCREMENT As Integer = 1

		''' <summary>
		''' The unit decrement adjustment type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const UNIT_DECREMENT As Integer = 2

		''' <summary>
		''' The block decrement adjustment type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BLOCK_DECREMENT As Integer = 3

		''' <summary>
		''' The block increment adjustment type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BLOCK_INCREMENT As Integer = 4

		''' <summary>
		''' The absolute tracking adjustment type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TRACK As Integer = 5

		''' <summary>
		''' The adjustable object that fired the event.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getAdjustable </seealso>
		Friend adjustable As java.awt.Adjustable

		''' <summary>
		''' <code>value</code> will contain the new value of the
		''' adjustable object.  This value will always be  in a
		''' range associated adjustable object.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getValue </seealso>
		Friend value As Integer

		''' <summary>
		''' The <code>adjustmentType</code> describes how the adjustable
		''' object value has changed.
		''' This value can be increased/decreased by a block or unit amount
		''' where the block is associated with page increments/decrements,
		''' and a unit is associated with line increments/decrements.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getAdjustmentType </seealso>
		Friend adjustmentType As Integer


		''' <summary>
		''' The <code>isAdjusting</code> is true if the event is one
		''' of the series of multiple adjustment events.
		''' 
		''' @since 1.4
		''' @serial </summary>
		''' <seealso cref= #getValueIsAdjusting </seealso>
		Friend isAdjusting As Boolean


	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = 5700290645205279921L


		''' <summary>
		''' Constructs an <code>AdjustmentEvent</code> object with the
		''' specified <code>Adjustable</code> source, event type,
		''' adjustment type, and value.
		''' <p> This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> The <code>Adjustable</code> object where the
		'''               event originated </param>
		''' <param name="id">     An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		''' <param name="type">   An integer indicating the adjustment type.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		''' <param name="value">  The current value of the adjustment </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getAdjustmentType() </seealso>
		''' <seealso cref= #getValue() </seealso>
		Public Sub New(ByVal source As java.awt.Adjustable, ByVal id As Integer, ByVal type As Integer, ByVal value As Integer)
			Me.New(source, id, type, value, False)
		End Sub

		''' <summary>
		''' Constructs an <code>AdjustmentEvent</code> object with the
		''' specified Adjustable source, event type, adjustment type, and value.
		''' <p> This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> The <code>Adjustable</code> object where the
		'''               event originated </param>
		''' <param name="id">     An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		''' <param name="type">   An integer indicating the adjustment type.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="AdjustmentEvent"/> </param>
		''' <param name="value">  The current value of the adjustment </param>
		''' <param name="isAdjusting"> A boolean that equals <code>true</code> if the event is one
		'''               of a series of multiple adjusting events,
		'''               otherwise <code>false</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null
		''' @since 1.4 </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getAdjustmentType() </seealso>
		''' <seealso cref= #getValue() </seealso>
		''' <seealso cref= #getValueIsAdjusting() </seealso>
		Public Sub New(ByVal source As java.awt.Adjustable, ByVal id As Integer, ByVal type As Integer, ByVal value As Integer, ByVal isAdjusting As Boolean)
			MyBase.New(source, id)
			adjustable = source
			Me.adjustmentType = type
			Me.value = value
			Me.isAdjusting = isAdjusting
		End Sub

		''' <summary>
		''' Returns the <code>Adjustable</code> object where this event originated.
		''' </summary>
		''' <returns> the <code>Adjustable</code> object where this event originated </returns>
		Public Overridable Property adjustable As java.awt.Adjustable
			Get
				Return adjustable
			End Get
		End Property

		''' <summary>
		''' Returns the current value in the adjustment event.
		''' </summary>
		''' <returns> the current value in the adjustment event </returns>
		Public Overridable Property value As Integer
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Returns the type of adjustment which caused the value changed
		''' event.  It will have one of the following values:
		''' <ul>
		''' <li><seealso cref="#UNIT_INCREMENT"/>
		''' <li><seealso cref="#UNIT_DECREMENT"/>
		''' <li><seealso cref="#BLOCK_INCREMENT"/>
		''' <li><seealso cref="#BLOCK_DECREMENT"/>
		''' <li><seealso cref="#TRACK"/>
		''' </ul> </summary>
		''' <returns> one of the adjustment values listed above </returns>
		Public Overridable Property adjustmentType As Integer
			Get
				Return adjustmentType
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this is one of multiple
		''' adjustment events.
		''' </summary>
		''' <returns> <code>true</code> if this is one of multiple
		'''         adjustment events, otherwise returns <code>false</code>
		''' @since 1.4 </returns>
		Public Overridable Property valueIsAdjusting As Boolean
			Get
				Return isAdjusting
			End Get
		End Property

		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case ADJUSTMENT_VALUE_CHANGED
				  typeStr = "ADJUSTMENT_VALUE_CHANGED"
			  Case Else
				  typeStr = "unknown type"
			End Select
			Dim adjTypeStr As String
			Select Case adjustmentType
			  Case UNIT_INCREMENT
				  adjTypeStr = "UNIT_INCREMENT"
			  Case UNIT_DECREMENT
				  adjTypeStr = "UNIT_DECREMENT"
			  Case BLOCK_INCREMENT
				  adjTypeStr = "BLOCK_INCREMENT"
			  Case BLOCK_DECREMENT
				  adjTypeStr = "BLOCK_DECREMENT"
			  Case TRACK
				  adjTypeStr = "TRACK"
			  Case Else
				  adjTypeStr = "unknown type"
			End Select
			Return typeStr & ",adjType=" & adjTypeStr & ",value=" & value & ",isAdjusting=" & isAdjusting
		End Function
	End Class

End Namespace
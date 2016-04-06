'
' * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' The component-level paint event.
	''' This event is a special type which is used to ensure that
	''' paint/update method calls are serialized along with the other
	''' events delivered from the event queue.  This event is not
	''' designed to be used with the Event Listener model; programs
	''' should continue to override paint/update methods in order
	''' render themselves properly.
	''' <p>
	''' An unspecified behavior will be caused if the {@code id} parameter
	''' of any particular {@code PaintEvent} instance is not
	''' in the range from {@code PAINT_FIRST} to {@code PAINT_LAST}.
	''' 
	''' @author Amy Fowler
	''' @since 1.1
	''' </summary>
	Public Class PaintEvent
		Inherits ComponentEvent

		''' <summary>
		''' Marks the first integer id for the range of paint event ids.
		''' </summary>
		Public Const PAINT_FIRST As Integer = 800

		''' <summary>
		''' Marks the last integer id for the range of paint event ids.
		''' </summary>
		Public Const PAINT_LAST As Integer = 801

		''' <summary>
		''' The paint event type.
		''' </summary>
		Public Const PAINT As Integer = PAINT_FIRST

		''' <summary>
		''' The update event type.
		''' </summary>
		Public Shared ReadOnly UPDATE As Integer = PAINT_FIRST + 1 '801

		''' <summary>
		''' This is the rectangle that represents the area on the source
		''' component that requires a repaint.
		''' This rectangle should be non null.
		''' 
		''' @serial </summary>
		''' <seealso cref= java.awt.Rectangle </seealso>
		''' <seealso cref= #setUpdateRect(Rectangle) </seealso>
		''' <seealso cref= #getUpdateRect() </seealso>
		Friend updateRect As java.awt.Rectangle

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 1267492026433337593L

		''' <summary>
		''' Constructs a <code>PaintEvent</code> object with the specified
		''' source component and type.
		''' <p> This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source">     The object where the event originated </param>
		''' <param name="id">           The integer that identifies the event type.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="PaintEvent"/> </param>
		''' <param name="updateRect"> The rectangle area which needs to be repainted </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getUpdateRect() </seealso>
		Public Sub New(  source As java.awt.Component,   id As Integer,   updateRect As java.awt.Rectangle)
			MyBase.New(source, id)
			Me.updateRect = updateRect
		End Sub

		''' <summary>
		''' Returns the rectangle representing the area which needs to be
		''' repainted in response to this event.
		''' </summary>
		Public Overridable Property updateRect As java.awt.Rectangle
			Get
				Return updateRect
			End Get
			Set(  updateRect As java.awt.Rectangle)
				Me.updateRect = updateRect
			End Set
		End Property


		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case PAINT
				  typeStr = "PAINT"
			  Case UPDATE
				  typeStr = "UPDATE"
			  Case Else
				  typeStr = "unknown type"
			End Select
			Return typeStr & ",updateRect=" & (If(updateRect IsNot Nothing, updateRect.ToString(), "null"))
		End Function
	End Class

End Namespace
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

Namespace java.awt.dnd

	''' <summary>
	''' The <code>DragSourceDropEvent</code> is delivered
	''' from the <code>DragSourceContextPeer</code>,
	''' via the <code>DragSourceContext</code>, to the <code>dragDropEnd</code>
	''' method of <code>DragSourceListener</code>s registered with that
	''' <code>DragSourceContext</code> and with its associated
	''' <code>DragSource</code>.
	''' It contains sufficient information for the
	''' originator of the operation
	''' to provide appropriate feedback to the end user
	''' when the operation completes.
	''' <P>
	''' @since 1.2
	''' </summary>

	Public Class DragSourceDropEvent
		Inherits DragSourceEvent

		Private Const serialVersionUID As Long = -5571321229470821891L

		''' <summary>
		''' Construct a <code>DragSourceDropEvent</code> for a drop,
		''' given the
		''' <code>DragSourceContext</code>, the drop action,
		''' and a <code>boolean</code> indicating if the drop was successful.
		''' The coordinates for this <code>DragSourceDropEvent</code>
		''' are not specified, so <code>getLocation</code> will return
		''' <code>null</code> for this event.
		''' <p>
		''' The argument <code>action</code> should be one of <code>DnDConstants</code>
		''' that represents a single action.
		''' This constructor does not throw any exception for invalid <code>action</code>.
		''' </summary>
		''' <param name="dsc"> the <code>DragSourceContext</code>
		''' associated with this <code>DragSourceDropEvent</code> </param>
		''' <param name="action"> the drop action </param>
		''' <param name="success"> a boolean indicating if the drop was successful
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= DragSourceEvent#getLocation </seealso>

		Public Sub New(  dsc As DragSourceContext,   action As Integer,   success As Boolean)
			MyBase.New(dsc)

			dropSuccess = success
			dropAction = action
		End Sub

		''' <summary>
		''' Construct a <code>DragSourceDropEvent</code> for a drop, given the
		''' <code>DragSourceContext</code>, the drop action, a <code>boolean</code>
		''' indicating if the drop was successful, and coordinates.
		''' <p>
		''' The argument <code>action</code> should be one of <code>DnDConstants</code>
		''' that represents a single action.
		''' This constructor does not throw any exception for invalid <code>action</code>.
		''' </summary>
		''' <param name="dsc"> the <code>DragSourceContext</code>
		''' associated with this <code>DragSourceDropEvent</code> </param>
		''' <param name="action"> the drop action </param>
		''' <param name="success"> a boolean indicating if the drop was successful </param>
		''' <param name="x">   the horizontal coordinate for the cursor location </param>
		''' <param name="y">   the vertical coordinate for the cursor location
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		''' 
		''' @since 1.4 </exception>
		Public Sub New(  dsc As DragSourceContext,   action As Integer,   success As Boolean,   x As Integer,   y As Integer)
			MyBase.New(dsc, x, y)

			dropSuccess = success
			dropAction = action
		End Sub

		''' <summary>
		''' Construct a <code>DragSourceDropEvent</code>
		''' for a drag that does not result in a drop.
		''' The coordinates for this <code>DragSourceDropEvent</code>
		''' are not specified, so <code>getLocation</code> will return
		''' <code>null</code> for this event.
		''' </summary>
		''' <param name="dsc"> the <code>DragSourceContext</code>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= DragSourceEvent#getLocation </seealso>

		Public Sub New(  dsc As DragSourceContext)
			MyBase.New(dsc)

			dropSuccess = False
		End Sub

		''' <summary>
		''' This method returns a <code>boolean</code> indicating
		''' if the drop was successful.
		''' </summary>
		''' <returns> <code>true</code> if the drop target accepted the drop and
		'''         successfully performed a drop action;
		'''         <code>false</code> if the drop target rejected the drop or
		'''         if the drop target accepted the drop, but failed to perform
		'''         a drop action. </returns>

		Public Overridable Property dropSuccess As Boolean
			Get
				Return dropSuccess
			End Get
		End Property

		''' <summary>
		''' This method returns an <code>int</code> representing
		''' the action performed by the target on the subject of the drop.
		''' </summary>
		''' <returns> the action performed by the target on the subject of the drop
		'''         if the drop target accepted the drop and the target drop action
		'''         is supported by the drag source; otherwise,
		'''         <code>DnDConstants.ACTION_NONE</code>. </returns>

		Public Overridable Property dropAction As Integer
			Get
				Return dropAction
			End Get
		End Property

	'    
	'     * fields
	'     

		''' <summary>
		''' <code>true</code> if the drop was successful.
		''' 
		''' @serial
		''' </summary>
		Private dropSuccess As Boolean

		''' <summary>
		''' The drop action.
		''' 
		''' @serial
		''' </summary>
		Private dropAction As Integer = DnDConstants.ACTION_NONE
	End Class

End Namespace
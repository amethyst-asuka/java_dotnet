'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>DropTargetEvent</code> is the base
	''' class for both the <code>DropTargetDragEvent</code>
	''' and the <code>DropTargetDropEvent</code>.
	''' It encapsulates the current state of the Drag and
	''' Drop operations, in particular the current
	''' <code>DropTargetContext</code>.
	''' 
	''' @since 1.2
	''' 
	''' </summary>

	Public Class DropTargetEvent
		Inherits java.util.EventObject

		Private Const serialVersionUID As Long = 2821229066521922993L

		''' <summary>
		''' Construct a <code>DropTargetEvent</code> object with
		''' the specified <code>DropTargetContext</code>.
		''' <P> </summary>
		''' <param name="dtc"> The <code>DropTargetContext</code> </param>
		''' <exception cref="NullPointerException"> if {@code dtc} equals {@code null}. </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getDropTargetContext() </seealso>

		Public Sub New(  dtc As java.awt.dnd.DropTargetContext)
			MyBase.New(dtc.dropTarget)

			context = dtc
		End Sub

		''' <summary>
		''' This method returns the <code>DropTargetContext</code>
		''' associated with this <code>DropTargetEvent</code>.
		''' <P> </summary>
		''' <returns> the <code>DropTargetContext</code> </returns>

		Public Overridable Property dropTargetContext As java.awt.dnd.DropTargetContext
			Get
				Return context
			End Get
		End Property

		''' <summary>
		''' The <code>DropTargetContext</code> associated with this
		''' <code>DropTargetEvent</code>.
		''' 
		''' @serial
		''' </summary>
		Protected Friend context As java.awt.dnd.DropTargetContext
	End Class

End Namespace
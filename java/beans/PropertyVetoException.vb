Imports System

'
' * Copyright (c) 1996, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	''' <summary>
	''' A PropertyVetoException is thrown when a proposed change to a
	''' property represents an unacceptable value.
	''' </summary>

	Public Class PropertyVetoException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 129596057694162164L

		''' <summary>
		''' Constructs a <code>PropertyVetoException</code> with a
		''' detailed message.
		''' </summary>
		''' <param name="mess"> Descriptive message </param>
		''' <param name="evt"> A PropertyChangeEvent describing the vetoed change. </param>
		Public Sub New(ByVal mess As String, ByVal evt As PropertyChangeEvent)
			MyBase.New(mess)
			Me.evt = evt
		End Sub

		 ''' <summary>
		 ''' Gets the vetoed <code>PropertyChangeEvent</code>.
		 ''' </summary>
		 ''' <returns> A PropertyChangeEvent describing the vetoed change. </returns>
		Public Overridable Property propertyChangeEvent As PropertyChangeEvent
			Get
				Return evt
			End Get
		End Property

		''' <summary>
		''' A PropertyChangeEvent describing the vetoed change.
		''' @serial
		''' </summary>
		Private evt As PropertyChangeEvent
	End Class

End Namespace
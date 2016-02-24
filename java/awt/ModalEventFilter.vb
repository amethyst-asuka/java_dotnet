'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt



	Friend MustInherit Class ModalEventFilter
		Implements EventFilter

		Protected Friend modalDialog As Dialog
		Protected Friend disabled As Boolean

		Protected Friend Sub New(ByVal modalDialog As Dialog)
			Me.modalDialog = modalDialog
			disabled = False
		End Sub

		Friend Overridable Property modalDialog As Dialog
			Get
				Return modalDialog
			End Get
		End Property

		Public Overridable Function acceptEvent(ByVal [event] As AWTEvent) As FilterAction Implements EventFilter.acceptEvent
			If disabled OrElse (Not modalDialog.visible) Then Return FilterAction.ACCEPT
			Dim eventID As Integer = event_Renamed.iD
			If (eventID >= MouseEvent.MOUSE_FIRST AndAlso eventID <= MouseEvent.MOUSE_LAST) OrElse (eventID >= ActionEvent.ACTION_FIRST AndAlso eventID <= ActionEvent.ACTION_LAST) OrElse eventID = WindowEvent.WINDOW_CLOSING Then
				Dim o As Object = event_Renamed.source
				If TypeOf o Is sun.awt.ModalExclude Then
					' Exclude this object from modality and
					' continue to pump it's events.
				ElseIf TypeOf o Is Component Then
					Dim c As Component = CType(o, Component)
					Do While (c IsNot Nothing) AndAlso Not(TypeOf c Is Window)
						c = c.parent_NoClientCode
					Loop
					If c IsNot Nothing Then Return acceptWindow(CType(c, Window))
				End If
			End If
			Return FilterAction.ACCEPT
		End Function

		Protected Friend MustOverride Function acceptWindow(ByVal w As Window) As FilterAction

		' When a modal dialog is hidden its modal filter may not be deleted from
		' EventDispatchThread event filters immediately, so we need to mark the filter
		' as disabled to prevent it from working. Simple checking for visibility of
		' the modalDialog is not enough, as it can be hidden and then shown again
		' with a new event pump and a new filter
		Friend Overridable Sub disable()
			disabled = True
		End Sub

		Friend Overridable Function compareTo(ByVal another As ModalEventFilter) As Integer
			Dim anotherDialog As Dialog = another.modalDialog
			' check if modalDialog is from anotherDialog's hierarchy
			'   or vice versa
			Dim c As Component = modalDialog
			Do While c IsNot Nothing
				If c Is anotherDialog Then Return 1
				c = c.parent_NoClientCode
			Loop
			c = anotherDialog
			Do While c IsNot Nothing
				If c Is modalDialog Then Return -1
				c = c.parent_NoClientCode
			Loop
			' check if one dialog blocks (directly or indirectly) another
			Dim blocker As Dialog = modalDialog.modalBlocker
			Do While blocker IsNot Nothing
				If blocker Is anotherDialog Then Return -1
				blocker = blocker.modalBlocker
			Loop
			blocker = anotherDialog.modalBlocker
			Do While blocker IsNot Nothing
				If blocker Is modalDialog Then Return 1
				blocker = blocker.modalBlocker
			Loop
			' compare modality types
			Return modalDialog.modalityType.CompareTo(anotherDialog.modalityType)
		End Function

		Shared Function createFilterForDialog(ByVal modalDialog As Dialog) As ModalEventFilter
			Select Case modalDialog.modalityType
				Case DOCUMENT_MODAL
					Return New DocumentModalEventFilter(modalDialog)
				Case APPLICATION_MODAL
					Return New ApplicationModalEventFilter(modalDialog)
				Case TOOLKIT_MODAL
					Return New ToolkitModalEventFilter(modalDialog)
			End Select
			Return Nothing
		End Function

		Private Class ToolkitModalEventFilter
			Inherits ModalEventFilter

			Private appContext As sun.awt.AppContext

			Friend Sub New(ByVal modalDialog As Dialog)
				MyBase.New(modalDialog)
				appContext = modalDialog.appContext
			End Sub

			Protected Friend Overrides Function acceptWindow(ByVal w As Window) As FilterAction
				If w.isModalExcluded(Dialog.ModalExclusionType.TOOLKIT_EXCLUDE) Then Return FilterAction.ACCEPT
				If w.appContext IsNot appContext Then Return FilterAction.REJECT
				Do While w IsNot Nothing
					If w Is modalDialog Then Return FilterAction.ACCEPT_IMMEDIATELY
					w = w.owner
				Loop
				Return FilterAction.REJECT
			End Function
		End Class

		Private Class ApplicationModalEventFilter
			Inherits ModalEventFilter

			Private appContext As sun.awt.AppContext

			Friend Sub New(ByVal modalDialog As Dialog)
				MyBase.New(modalDialog)
				appContext = modalDialog.appContext
			End Sub

			Protected Friend Overrides Function acceptWindow(ByVal w As Window) As FilterAction
				If w.isModalExcluded(Dialog.ModalExclusionType.APPLICATION_EXCLUDE) Then Return FilterAction.ACCEPT
				If w.appContext Is appContext Then
					Do While w IsNot Nothing
						If w Is modalDialog Then Return FilterAction.ACCEPT_IMMEDIATELY
						w = w.owner
					Loop
					Return FilterAction.REJECT
				End If
				Return FilterAction.ACCEPT
			End Function
		End Class

		Private Class DocumentModalEventFilter
			Inherits ModalEventFilter

			Private documentRoot As Window

			Friend Sub New(ByVal modalDialog As Dialog)
				MyBase.New(modalDialog)
				documentRoot = modalDialog.documentRoot
			End Sub

			Protected Friend Overrides Function acceptWindow(ByVal w As Window) As FilterAction
				' application- and toolkit-excluded windows are blocked by
				' document-modal dialogs from their child hierarchy
				If w.isModalExcluded(Dialog.ModalExclusionType.APPLICATION_EXCLUDE) Then
					Dim w1 As Window = modalDialog.owner
					Do While w1 IsNot Nothing
						If w1 Is w Then Return FilterAction.REJECT
						w1 = w1.owner
					Loop
					Return FilterAction.ACCEPT
				End If
				Do While w IsNot Nothing
					If w Is modalDialog Then Return FilterAction.ACCEPT_IMMEDIATELY
					If w Is documentRoot Then Return FilterAction.REJECT
					w = w.owner
				Loop
				Return FilterAction.ACCEPT
			End Function
		End Class
	End Class

End Namespace
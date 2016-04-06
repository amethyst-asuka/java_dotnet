'
' * Copyright (c) 2002, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.text

	''' <summary>
	''' DontCareFieldPosition defines no-op FieldDelegate. Its
	''' singleton is used for the format methods that don't take a
	''' FieldPosition.
	''' </summary>
	Friend Class DontCareFieldPosition
		Inherits FieldPosition

		' The singleton of DontCareFieldPosition.
		Friend Shared ReadOnly INSTANCE As FieldPosition = New DontCareFieldPosition

		Private ReadOnly noDelegate As Format.FieldDelegate = New FieldDelegateAnonymousInnerClassHelper

		Private Class FieldDelegateAnonymousInnerClassHelper
			Implements FieldDelegate

			Public Overridable Sub formatted(  attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer)
			End Sub
			Public Overridable Sub formatted(  fieldID As Integer,   attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer)
			End Sub
		End Class

		Private Sub New()
			MyBase.New(0)
		End Sub

		Friend  Overrides ReadOnly Property  fieldDelegate As Format.FieldDelegate
			Get
				Return noDelegate
			End Get
		End Property
	End Class

End Namespace
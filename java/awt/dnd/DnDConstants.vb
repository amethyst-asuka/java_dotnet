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
	''' This class contains constant values representing
	''' the type of action(s) to be performed by a Drag and Drop operation.
	''' @since 1.2
	''' </summary>
	Public NotInheritable Class DnDConstants

		Private Sub New() ' define null private constructor.
		End Sub

		''' <summary>
		''' An <code>int</code> representing no action.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTION_NONE As Integer = &H0

		''' <summary>
		''' An <code>int</code> representing a &quot;copy&quot; action.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTION_COPY As Integer = &H1

		''' <summary>
		''' An <code>int</code> representing a &quot;move&quot; action.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTION_MOVE As Integer = &H2

		''' <summary>
		''' An <code>int</code> representing a &quot;copy&quot; or
		''' &quot;move&quot; action.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly ACTION_COPY_OR_MOVE As Integer = ACTION_COPY Or ACTION_MOVE

		''' <summary>
		''' An <code>int</code> representing a &quot;link&quot; action.
		''' 
		''' The link verb is found in many, if not all native DnD platforms, and the
		''' actual interpretation of LINK semantics is both platform
		''' and application dependent. Broadly speaking, the
		''' semantic is "do not copy, or move the operand, but create a reference
		''' to it". Defining the meaning of "reference" is where ambiguity is
		''' introduced.
		''' 
		''' The verb is provided for completeness, but its use is not recommended
		''' for DnD operations between logically distinct applications where
		''' misinterpretation of the operations semantics could lead to confusing
		''' results for the user.
		''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTION_LINK As Integer = &H40000000

		''' <summary>
		''' An <code>int</code> representing a &quot;reference&quot;
		''' action (synonym for ACTION_LINK).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTION_REFERENCE As Integer = ACTION_LINK

	End Class

End Namespace
Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.callback

	''' <summary>
	''' <p> Underlying security services instantiate and pass a
	''' {@code ChoiceCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to display a list of choices
	''' and to retrieve the selected choice(s).
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class ChoiceCallback
		Implements Callback

		Private Const serialVersionUID As Long = -3975664071579892167L

		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private prompt As String
		''' <summary>
		''' @serial the list of choices
		''' @since 1.4
		''' </summary>
		Private choices As String()
		''' <summary>
		''' @serial the choice to be used as the default choice
		''' @since 1.4
		''' </summary>
		Private defaultChoice As Integer
		''' <summary>
		''' @serial whether multiple selections are allowed from the list of
		''' choices
		''' @since 1.4
		''' </summary>
		Private multipleSelectionsAllowed As Boolean
		''' <summary>
		''' @serial the selected choices, represented as indexes into the
		'''          {@code choices} list.
		''' @since 1.4
		''' </summary>
		Private selections As Integer()

		''' <summary>
		''' Construct a {@code ChoiceCallback} with a prompt,
		''' a list of choices, a default choice, and a boolean specifying
		''' whether or not multiple selections from the list of choices are allowed.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to describe the list of choices. <p>
		''' </param>
		''' <param name="choices"> the list of choices. <p>
		''' </param>
		''' <param name="defaultChoice"> the choice to be used as the default choice
		'''                  when the list of choices are displayed.  This value
		'''                  is represented as an index into the
		'''                  {@code choices} array. <p>
		''' </param>
		''' <param name="multipleSelectionsAllowed"> boolean specifying whether or
		'''                  not multiple selections can be made from the
		'''                  list of choices.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null,
		'''                  if {@code prompt} has a length of 0,
		'''                  if {@code choices} is null,
		'''                  if {@code choices} has a length of 0,
		'''                  if any element from {@code choices} is null,
		'''                  if any element from {@code choices}
		'''                  has a length of 0 or if {@code defaultChoice}
		'''                  does not fall within the array boundaries of
		'''                  {@code choices}. </exception>
		Public Sub New(ByVal prompt As String, ByVal choices As String(), ByVal defaultChoice As Integer, ByVal multipleSelectionsAllowed As Boolean)

			If prompt Is Nothing OrElse prompt.Length = 0 OrElse choices Is Nothing OrElse choices.Length = 0 OrElse defaultChoice < 0 OrElse defaultChoice >= choices.Length Then Throw New System.ArgumentException

			For i As Integer = 0 To choices.Length - 1
				If choices(i) Is Nothing OrElse choices(i).Length = 0 Then Throw New System.ArgumentException
			Next i

			Me.prompt = prompt
			Me.choices = choices
			Me.defaultChoice = defaultChoice
			Me.multipleSelectionsAllowed = multipleSelectionsAllowed
		End Sub

		''' <summary>
		''' Get the prompt.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the prompt. </returns>
		Public Overridable Property prompt As String
			Get
				Return prompt
			End Get
		End Property

		''' <summary>
		''' Get the list of choices.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the list of choices. </returns>
		Public Overridable Property choices As String()
			Get
				Return choices
			End Get
		End Property

		''' <summary>
		''' Get the defaultChoice.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the defaultChoice, represented as an index into
		'''          the {@code choices} list. </returns>
		Public Overridable Property defaultChoice As Integer
			Get
				Return defaultChoice
			End Get
		End Property

		''' <summary>
		''' Get the boolean determining whether multiple selections from
		''' the {@code choices} list are allowed.
		''' 
		''' <p>
		''' </summary>
		''' <returns> whether multiple selections are allowed. </returns>
		Public Overridable Function allowMultipleSelections() As Boolean
			Return multipleSelectionsAllowed
		End Function

		''' <summary>
		''' Set the selected choice.
		''' 
		''' <p>
		''' </summary>
		''' <param name="selection"> the selection represented as an index into the
		'''          {@code choices} list.
		''' </param>
		''' <seealso cref= #getSelectedIndexes </seealso>
		Public Overridable Property selectedIndex As Integer
			Set(ByVal selection As Integer)
				Me.selections = New Integer(0){}
				Me.selections(0) = selection
			End Set
		End Property

		''' <summary>
		''' Set the selected choices.
		''' 
		''' <p>
		''' </summary>
		''' <param name="selections"> the selections represented as indexes into the
		'''          {@code choices} list.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if multiple selections are
		'''          not allowed, as determined by
		'''          {@code allowMultipleSelections}.
		''' </exception>
		''' <seealso cref= #getSelectedIndexes </seealso>
		Public Overridable Property selectedIndexes As Integer()
			Set(ByVal selections As Integer())
				If Not multipleSelectionsAllowed Then Throw New System.NotSupportedException
				Me.selections = selections
			End Set
			Get
				Return selections
			End Get
		End Property

	End Class

End Namespace
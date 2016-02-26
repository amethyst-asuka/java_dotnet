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
	''' {@code ConfirmationCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to ask for YES/NO,
	''' OK/CANCEL, YES/NO/CANCEL or other similar confirmations.
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class ConfirmationCallback
		Implements Callback

		Private Const serialVersionUID As Long = -9095656433782481624L

		''' <summary>
		''' Unspecified option type.
		''' 
		''' <p> The {@code getOptionType} method returns this
		''' value if this {@code ConfirmationCallback} was instantiated
		''' with {@code options} instead of an {@code optionType}.
		''' </summary>
		Public Const UNSPECIFIED_OPTION As Integer = -1

		''' <summary>
		''' YES/NO confirmation option.
		''' 
		''' <p> An underlying security service specifies this as the
		''' {@code optionType} to a {@code ConfirmationCallback}
		''' constructor if it requires a confirmation which can be answered
		''' with either {@code YES} or {@code NO}.
		''' </summary>
		Public Const YES_NO_OPTION As Integer = 0

		''' <summary>
		''' YES/NO/CANCEL confirmation confirmation option.
		''' 
		''' <p> An underlying security service specifies this as the
		''' {@code optionType} to a {@code ConfirmationCallback}
		''' constructor if it requires a confirmation which can be answered
		''' with either {@code YES}, {@code NO} or {@code CANCEL}.
		''' </summary>
		Public Const YES_NO_CANCEL_OPTION As Integer = 1

		''' <summary>
		''' OK/CANCEL confirmation confirmation option.
		''' 
		''' <p> An underlying security service specifies this as the
		''' {@code optionType} to a {@code ConfirmationCallback}
		''' constructor if it requires a confirmation which can be answered
		''' with either {@code OK} or {@code CANCEL}.
		''' </summary>
		Public Const OK_CANCEL_OPTION As Integer = 2

		''' <summary>
		''' YES option.
		''' 
		''' <p> If an {@code optionType} was specified to this
		''' {@code ConfirmationCallback}, this option may be specified as a
		''' {@code defaultOption} or returned as the selected index.
		''' </summary>
		Public Const YES As Integer = 0

		''' <summary>
		''' NO option.
		''' 
		''' <p> If an {@code optionType} was specified to this
		''' {@code ConfirmationCallback}, this option may be specified as a
		''' {@code defaultOption} or returned as the selected index.
		''' </summary>
		Public Const NO As Integer = 1

		''' <summary>
		''' CANCEL option.
		''' 
		''' <p> If an {@code optionType} was specified to this
		''' {@code ConfirmationCallback}, this option may be specified as a
		''' {@code defaultOption} or returned as the selected index.
		''' </summary>
		Public Const CANCEL As Integer = 2

		''' <summary>
		''' OK option.
		''' 
		''' <p> If an {@code optionType} was specified to this
		''' {@code ConfirmationCallback}, this option may be specified as a
		''' {@code defaultOption} or returned as the selected index.
		''' </summary>
		Public Const OK As Integer = 3

		''' <summary>
		''' INFORMATION message type. </summary>
		Public Const INFORMATION As Integer = 0

		''' <summary>
		''' WARNING message type. </summary>
		Public Const WARNING As Integer = 1

		''' <summary>
		''' ERROR message type. </summary>
		Public Const [ERROR] As Integer = 2
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private prompt As String
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private messageType As Integer
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private optionType As Integer = UNSPECIFIED_OPTION
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private defaultOption As Integer
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private options As String()
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private selection As Integer

		''' <summary>
		''' Construct a {@code ConfirmationCallback} with a
		''' message type, an option type and a default option.
		''' 
		''' <p> Underlying security services use this constructor if
		''' they require either a YES/NO, YES/NO/CANCEL or OK/CANCEL
		''' confirmation.
		''' 
		''' <p>
		''' </summary>
		''' <param name="messageType"> the message type ({@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR}). <p>
		''' </param>
		''' <param name="optionType"> the option type ({@code YES_NO_OPTION},
		'''                  {@code YES_NO_CANCEL_OPTION} or
		'''                  {@code OK_CANCEL_OPTION}). <p>
		''' </param>
		''' <param name="defaultOption"> the default option
		'''                  from the provided optionType ({@code YES},
		'''                  {@code NO}, {@code CANCEL} or
		'''                  {@code OK}).
		''' </param>
		''' <exception cref="IllegalArgumentException"> if messageType is not either
		'''                  {@code INFORMATION}, {@code WARNING},
		'''                  or {@code ERROR}, if optionType is not either
		'''                  {@code YES_NO_OPTION},
		'''                  {@code YES_NO_CANCEL_OPTION}, or
		'''                  {@code OK_CANCEL_OPTION},
		'''                  or if {@code defaultOption}
		'''                  does not correspond to one of the options in
		'''                  {@code optionType}. </exception>
		Public Sub New(ByVal messageType As Integer, ByVal optionType As Integer, ByVal defaultOption As Integer)

			If messageType < INFORMATION OrElse messageType > [ERROR] OrElse optionType < YES_NO_OPTION OrElse optionType > OK_CANCEL_OPTION Then Throw New System.ArgumentException

			Select Case optionType
			Case YES_NO_OPTION
				If defaultOption <> YES AndAlso defaultOption <> NO Then Throw New System.ArgumentException
			Case YES_NO_CANCEL_OPTION
				If defaultOption <> YES AndAlso defaultOption <> NO AndAlso defaultOption <> CANCEL Then Throw New System.ArgumentException
			Case OK_CANCEL_OPTION
				If defaultOption <> OK AndAlso defaultOption <> CANCEL Then Throw New System.ArgumentException
			End Select

			Me.messageType = messageType
			Me.optionType = optionType
			Me.defaultOption = defaultOption
		End Sub

		''' <summary>
		''' Construct a {@code ConfirmationCallback} with a
		''' message type, a list of options and a default option.
		''' 
		''' <p> Underlying security services use this constructor if
		''' they require a confirmation different from the available preset
		''' confirmations provided (for example, CONTINUE/ABORT or STOP/GO).
		''' The confirmation options are listed in the {@code options} array,
		''' and are displayed by the {@code CallbackHandler} implementation
		''' in a manner consistent with the way preset options are displayed.
		''' 
		''' <p>
		''' </summary>
		''' <param name="messageType"> the message type ({@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR}). <p>
		''' </param>
		''' <param name="options"> the list of confirmation options. <p>
		''' </param>
		''' <param name="defaultOption"> the default option, represented as an index
		'''                  into the {@code options} array.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if messageType is not either
		'''                  {@code INFORMATION}, {@code WARNING},
		'''                  or {@code ERROR}, if {@code options} is null,
		'''                  if {@code options} has a length of 0,
		'''                  if any element from {@code options} is null,
		'''                  if any element from {@code options}
		'''                  has a length of 0, or if {@code defaultOption}
		'''                  does not lie within the array boundaries of
		'''                  {@code options}. </exception>
		Public Sub New(ByVal messageType As Integer, ByVal options As String(), ByVal defaultOption As Integer)

			If messageType < INFORMATION OrElse messageType > [ERROR] OrElse options Is Nothing OrElse options.Length = 0 OrElse defaultOption < 0 OrElse defaultOption >= options.Length Then Throw New System.ArgumentException

			For i As Integer = 0 To options.Length - 1
				If options(i) Is Nothing OrElse options(i).Length = 0 Then Throw New System.ArgumentException
			Next i

			Me.messageType = messageType
			Me.options = options
			Me.defaultOption = defaultOption
		End Sub

		''' <summary>
		''' Construct a {@code ConfirmationCallback} with a prompt,
		''' message type, an option type and a default option.
		''' 
		''' <p> Underlying security services use this constructor if
		''' they require either a YES/NO, YES/NO/CANCEL or OK/CANCEL
		''' confirmation.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to describe the list of options. <p>
		''' </param>
		''' <param name="messageType"> the message type ({@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR}). <p>
		''' </param>
		''' <param name="optionType"> the option type ({@code YES_NO_OPTION},
		'''                  {@code YES_NO_CANCEL_OPTION} or
		'''                  {@code OK_CANCEL_OPTION}). <p>
		''' </param>
		''' <param name="defaultOption"> the default option
		'''                  from the provided optionType ({@code YES},
		'''                  {@code NO}, {@code CANCEL} or
		'''                  {@code OK}).
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null,
		'''                  if {@code prompt} has a length of 0,
		'''                  if messageType is not either
		'''                  {@code INFORMATION}, {@code WARNING},
		'''                  or {@code ERROR}, if optionType is not either
		'''                  {@code YES_NO_OPTION},
		'''                  {@code YES_NO_CANCEL_OPTION}, or
		'''                  {@code OK_CANCEL_OPTION},
		'''                  or if {@code defaultOption}
		'''                  does not correspond to one of the options in
		'''                  {@code optionType}. </exception>
		Public Sub New(ByVal prompt As String, ByVal messageType As Integer, ByVal optionType As Integer, ByVal defaultOption As Integer)

			If prompt Is Nothing OrElse prompt.Length = 0 OrElse messageType < INFORMATION OrElse messageType > [ERROR] OrElse optionType < YES_NO_OPTION OrElse optionType > OK_CANCEL_OPTION Then Throw New System.ArgumentException

			Select Case optionType
			Case YES_NO_OPTION
				If defaultOption <> YES AndAlso defaultOption <> NO Then Throw New System.ArgumentException
			Case YES_NO_CANCEL_OPTION
				If defaultOption <> YES AndAlso defaultOption <> NO AndAlso defaultOption <> CANCEL Then Throw New System.ArgumentException
			Case OK_CANCEL_OPTION
				If defaultOption <> OK AndAlso defaultOption <> CANCEL Then Throw New System.ArgumentException
			End Select

			Me.prompt = prompt
			Me.messageType = messageType
			Me.optionType = optionType
			Me.defaultOption = defaultOption
		End Sub

		''' <summary>
		''' Construct a {@code ConfirmationCallback} with a prompt,
		''' message type, a list of options and a default option.
		''' 
		''' <p> Underlying security services use this constructor if
		''' they require a confirmation different from the available preset
		''' confirmations provided (for example, CONTINUE/ABORT or STOP/GO).
		''' The confirmation options are listed in the {@code options} array,
		''' and are displayed by the {@code CallbackHandler} implementation
		''' in a manner consistent with the way preset options are displayed.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to describe the list of options. <p>
		''' </param>
		''' <param name="messageType"> the message type ({@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR}). <p>
		''' </param>
		''' <param name="options"> the list of confirmation options. <p>
		''' </param>
		''' <param name="defaultOption"> the default option, represented as an index
		'''                  into the {@code options} array.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null,
		'''                  if {@code prompt} has a length of 0,
		'''                  if messageType is not either
		'''                  {@code INFORMATION}, {@code WARNING},
		'''                  or {@code ERROR}, if {@code options} is null,
		'''                  if {@code options} has a length of 0,
		'''                  if any element from {@code options} is null,
		'''                  if any element from {@code options}
		'''                  has a length of 0, or if {@code defaultOption}
		'''                  does not lie within the array boundaries of
		'''                  {@code options}. </exception>
		Public Sub New(ByVal prompt As String, ByVal messageType As Integer, ByVal options As String(), ByVal defaultOption As Integer)

			If prompt Is Nothing OrElse prompt.Length = 0 OrElse messageType < INFORMATION OrElse messageType > [ERROR] OrElse options Is Nothing OrElse options.Length = 0 OrElse defaultOption < 0 OrElse defaultOption >= options.Length Then Throw New System.ArgumentException

			For i As Integer = 0 To options.Length - 1
				If options(i) Is Nothing OrElse options(i).Length = 0 Then Throw New System.ArgumentException
			Next i

			Me.prompt = prompt
			Me.messageType = messageType
			Me.options = options
			Me.defaultOption = defaultOption
		End Sub

		''' <summary>
		''' Get the prompt.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the prompt, or null if this {@code ConfirmationCallback}
		'''          was instantiated without a {@code prompt}. </returns>
		Public Overridable Property prompt As String
			Get
				Return prompt
			End Get
		End Property

		''' <summary>
		''' Get the message type.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the message type ({@code INFORMATION},
		'''          {@code WARNING} or {@code ERROR}). </returns>
		Public Overridable Property messageType As Integer
			Get
				Return messageType
			End Get
		End Property

		''' <summary>
		''' Get the option type.
		''' 
		''' <p> If this method returns {@code UNSPECIFIED_OPTION}, then this
		''' {@code ConfirmationCallback} was instantiated with
		''' {@code options} instead of an {@code optionType}.
		''' In this case, invoke the {@code getOptions} method
		''' to determine which confirmation options to display.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the option type ({@code YES_NO_OPTION},
		'''          {@code YES_NO_CANCEL_OPTION} or
		'''          {@code OK_CANCEL_OPTION}), or
		'''          {@code UNSPECIFIED_OPTION} if this
		'''          {@code ConfirmationCallback} was instantiated with
		'''          {@code options} instead of an {@code optionType}. </returns>
		Public Overridable Property optionType As Integer
			Get
				Return optionType
			End Get
		End Property

		''' <summary>
		''' Get the confirmation options.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the list of confirmation options, or null if this
		'''          {@code ConfirmationCallback} was instantiated with
		'''          an {@code optionType} instead of {@code options}. </returns>
		Public Overridable Property options As String()
			Get
				Return options
			End Get
		End Property

		''' <summary>
		''' Get the default option.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the default option, represented as
		'''          {@code YES}, {@code NO}, {@code OK} or
		'''          {@code CANCEL} if an {@code optionType}
		'''          was specified to the constructor of this
		'''          {@code ConfirmationCallback}.
		'''          Otherwise, this method returns the default option as
		'''          an index into the
		'''          {@code options} array specified to the constructor
		'''          of this {@code ConfirmationCallback}. </returns>
		Public Overridable Property defaultOption As Integer
			Get
				Return defaultOption
			End Get
		End Property

		''' <summary>
		''' Set the selected confirmation option.
		''' 
		''' <p>
		''' </summary>
		''' <param name="selection"> the selection represented as {@code YES},
		'''          {@code NO}, {@code OK} or {@code CANCEL}
		'''          if an {@code optionType} was specified to the constructor
		'''          of this {@code ConfirmationCallback}.
		'''          Otherwise, the selection represents the index into the
		'''          {@code options} array specified to the constructor
		'''          of this {@code ConfirmationCallback}.
		''' </param>
		''' <seealso cref= #getSelectedIndex </seealso>
		Public Overridable Property selectedIndex As Integer
			Set(ByVal selection As Integer)
				Me.selection = selection
			End Set
			Get
				Return selection
			End Get
		End Property

	End Class

End Namespace
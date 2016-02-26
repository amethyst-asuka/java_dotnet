Imports Microsoft.VisualBasic
Imports System.Diagnostics

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio.charset



	''' <summary>
	''' An engine that can transform a sequence of sixteen-bit Unicode characters into a sequence of
	''' bytes in a specific charset.
	''' 
	''' <a name="steps"></a>
	''' 
	''' <p> The input character sequence is provided in a character buffer or a series
	''' of such buffers.  The output byte sequence is written to a byte buffer
	''' or a series of such buffers.  An encoder should always be used by making
	''' the following sequence of method invocations, hereinafter referred to as an
	''' <i>encoding operation</i>:
	''' 
	''' <ol>
	''' 
	'''   <li><p> Reset the encoder via the <seealso cref="#reset reset"/> method, unless it
	'''   has not been used before; </p></li>
	''' 
	'''   <li><p> Invoke the <seealso cref="#encode encode"/> method zero or more times, as
	'''   long as additional input may be available, passing <tt>false</tt> for the
	'''   <tt>endOfInput</tt> argument and filling the input buffer and flushing the
	'''   output buffer between invocations; </p></li>
	''' 
	'''   <li><p> Invoke the <seealso cref="#encode encode"/> method one final time, passing
	'''   <tt>true</tt> for the <tt>endOfInput</tt> argument; and then </p></li>
	''' 
	'''   <li><p> Invoke the <seealso cref="#flush flush"/> method so that the encoder can
	'''   flush any internal state to the output buffer. </p></li>
	''' 
	''' </ol>
	''' 
	''' Each invocation of the <seealso cref="#encode encode"/> method will encode as many
	''' characters as possible from the input buffer, writing the resulting bytes
	''' to the output buffer.  The <seealso cref="#encode encode"/> method returns when more
	''' input is required, when there is not enough room in the output buffer, or
	''' when an encoding error has occurred.  In each case a <seealso cref="CoderResult"/>
	''' object is returned to describe the reason for termination.  An invoker can
	''' examine this object and fill the input buffer, flush the output buffer, or
	''' attempt to recover from an encoding error, as appropriate, and try again.
	''' 
	''' <a name="ce"></a>
	''' 
	''' <p> There are two general types of encoding errors.  If the input character
	''' sequence is not a legal sixteen-bit Unicode sequence then the input is considered <i>malformed</i>.  If
	''' the input character sequence is legal but cannot be mapped to a valid
	''' byte sequence in the given charset then an <i>unmappable character</i> has been encountered.
	''' 
	''' <a name="cae"></a>
	''' 
	''' <p> How an encoding error is handled depends upon the action requested for
	''' that type of error, which is described by an instance of the {@link
	''' CodingErrorAction} class.  The possible error actions are to {@linkplain
	''' CodingErrorAction#IGNORE ignore} the erroneous input, {@linkplain
	''' CodingErrorAction#REPORT report} the error to the invoker via
	''' the returned <seealso cref="CoderResult"/> object, or {@link CodingErrorAction#REPLACE
	''' replace} the erroneous input with the current value of the
	''' replacement byte array.  The replacement
	''' 
	''' 
	''' is initially set to the encoder's default replacement, which often
	''' (but not always) has the initial value&nbsp;<tt>{</tt>&nbsp;<tt>(byte)'?'</tt>&nbsp;<tt>}</tt>;
	''' 
	''' 
	''' 
	''' 
	''' 
	''' its value may be changed via the {@link #replaceWith(byte[])
	''' replaceWith} method.
	''' 
	''' <p> The default action for malformed-input and unmappable-character errors
	''' is to <seealso cref="CodingErrorAction#REPORT report"/> them.  The
	''' malformed-input error action may be changed via the {@link
	''' #onMalformedInput(CodingErrorAction) onMalformedInput} method; the
	''' unmappable-character action may be changed via the {@link
	''' #onUnmappableCharacter(CodingErrorAction) onUnmappableCharacter} method.
	''' 
	''' <p> This class is designed to handle many of the details of the encoding
	''' process, including the implementation of error actions.  An encoder for a
	''' specific charset, which is a concrete subclass of this [Class], need only
	''' implement the abstract <seealso cref="#encodeLoop encodeLoop"/> method, which
	''' encapsulates the basic encoding loop.  A subclass that maintains internal
	''' state should, additionally, override the <seealso cref="#implFlush implFlush"/> and
	''' <seealso cref="#implReset implReset"/> methods.
	''' 
	''' <p> Instances of this class are not safe for use by multiple concurrent
	''' threads.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>
	''' <seealso cref= ByteBuffer </seealso>
	''' <seealso cref= CharBuffer </seealso>
	''' <seealso cref= Charset </seealso>
	''' <seealso cref= CharsetDecoder </seealso>

	Public MustInherit Class CharsetEncoder

		Private ReadOnly charset_Renamed As Charset
		Private ReadOnly averageBytesPerChar_Renamed As Single
		Private ReadOnly maxBytesPerChar_Renamed As Single

		Private replacement_Renamed As SByte()
		Private malformedInputAction_Renamed As CodingErrorAction = CodingErrorAction.REPORT
		Private unmappableCharacterAction_Renamed As CodingErrorAction = CodingErrorAction.REPORT

		' Internal states
		'
		Private Const ST_RESET As Integer = 0
		Private Const ST_CODING As Integer = 1
		Private Const ST_END As Integer = 2
		Private Const ST_FLUSHED As Integer = 3

		Private state As Integer = ST_RESET

		Private Shared stateNames As String() = { "RESET", "CODING", "CODING_END", "FLUSHED" }


		''' <summary>
		''' Initializes a new encoder.  The new encoder will have the given
		''' bytes-per-char and replacement values.
		''' </summary>
		''' <param name="cs">
		'''         The charset that created this encoder
		''' </param>
		''' <param name="averageBytesPerChar">
		'''         A positive float value indicating the expected number of
		'''         bytes that will be produced for each input character
		''' </param>
		''' <param name="maxBytesPerChar">
		'''         A positive float value indicating the maximum number of
		'''         bytes that will be produced for each input character
		''' </param>
		''' <param name="replacement">
		'''         The initial replacement; must not be <tt>null</tt>, must have
		'''         non-zero length, must not be longer than maxBytesPerChar,
		'''         and must be <seealso cref="#isLegalReplacement legal"/>
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		Protected Friend Sub New(ByVal cs As Charset, ByVal averageBytesPerChar As Single, ByVal maxBytesPerChar As Single, ByVal replacement As SByte())
			Me.charset_Renamed = cs
			If averageBytesPerChar <= 0.0f Then Throw New IllegalArgumentException("Non-positive " & "averageBytesPerChar")
			If maxBytesPerChar <= 0.0f Then Throw New IllegalArgumentException("Non-positive " & "maxBytesPerChar")
			If Not Charset.atBugLevel("1.4") Then
				If averageBytesPerChar > maxBytesPerChar Then Throw New IllegalArgumentException("averageBytesPerChar" & " exceeds " & "maxBytesPerChar")
			End If
			Me.replacement_Renamed = replacement
			Me.averageBytesPerChar_Renamed = averageBytesPerChar
			Me.maxBytesPerChar_Renamed = maxBytesPerChar
			replaceWith(replacement)
		End Sub

		''' <summary>
		''' Initializes a new encoder.  The new encoder will have the given
		''' bytes-per-char values and its replacement will be the
		''' byte array <tt>{</tt>&nbsp;<tt>(byte)'?'</tt>&nbsp;<tt>}</tt>.
		''' </summary>
		''' <param name="cs">
		'''         The charset that created this encoder
		''' </param>
		''' <param name="averageBytesPerChar">
		'''         A positive float value indicating the expected number of
		'''         bytes that will be produced for each input character
		''' </param>
		''' <param name="maxBytesPerChar">
		'''         A positive float value indicating the maximum number of
		'''         bytes that will be produced for each input character
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		Protected Friend Sub New(ByVal cs As Charset, ByVal averageBytesPerChar As Single, ByVal maxBytesPerChar As Single)
			Me.New(cs, averageBytesPerChar, maxBytesPerChar, New SByte() { AscW("?"c) })
		End Sub

		''' <summary>
		''' Returns the charset that created this encoder.
		''' </summary>
		''' <returns>  This encoder's charset </returns>
		Public Function charset() As Charset
			Return charset_Renamed
		End Function

		''' <summary>
		''' Returns this encoder's replacement value.
		''' </summary>
		''' <returns>  This encoder's current replacement,
		'''          which is never <tt>null</tt> and is never empty </returns>
		Public Function replacement() As SByte()




			Return java.util.Arrays.copyOf(replacement_Renamed, replacement_Renamed.Length)

		End Function

		''' <summary>
		''' Changes this encoder's replacement value.
		''' 
		''' <p> This method invokes the <seealso cref="#implReplaceWith implReplaceWith"/>
		''' method, passing the new replacement, after checking that the new
		''' replacement is acceptable.  </p>
		''' </summary>
		''' <param name="newReplacement">  The replacement value
		''' 
		''' 
		''' 
		''' 
		''' 
		''' 
		'''         The new replacement; must not be <tt>null</tt>, must have
		'''         non-zero length, must not be longer than the value returned by
		'''         the <seealso cref="#maxBytesPerChar() maxBytesPerChar"/> method, and
		'''         must be <seealso cref="#isLegalReplacement legal"/>
		''' 
		''' </param>
		''' <returns>  This encoder
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameter do not hold </exception>
		Public Function replaceWith(ByVal newReplacement As SByte()) As CharsetEncoder
			If newReplacement Is Nothing Then Throw New IllegalArgumentException("Null replacement")
			Dim len As Integer = newReplacement.Length
			If len = 0 Then Throw New IllegalArgumentException("Empty replacement")
			If len > maxBytesPerChar_Renamed Then Throw New IllegalArgumentException("Replacement too long")




			If Not isLegalReplacement(newReplacement) Then Throw New IllegalArgumentException("Illegal replacement")
			Me.replacement_Renamed = java.util.Arrays.copyOf(newReplacement, newReplacement.Length)

			implReplaceWith(Me.replacement_Renamed)
			Return Me
		End Function

		''' <summary>
		''' Reports a change to this encoder's replacement value.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by encoders that require notification of changes to
		''' the replacement.  </p>
		''' </summary>
		''' <param name="newReplacement">    The replacement value </param>
		Protected Friend Overridable Sub implReplaceWith(ByVal newReplacement As SByte())
		End Sub



		Private cachedDecoder As WeakReference(Of CharsetDecoder) = Nothing

		''' <summary>
		''' Tells whether or not the given byte array is a legal replacement value
		''' for this encoder.
		''' 
		''' <p> A replacement is legal if, and only if, it is a legal sequence of
		''' bytes in this encoder's charset; that is, it must be possible to decode
		''' the replacement into one or more sixteen-bit Unicode characters.
		''' 
		''' <p> The default implementation of this method is not very efficient; it
		''' should generally be overridden to improve performance.  </p>
		''' </summary>
		''' <param name="repl">  The byte array to be tested
		''' </param>
		''' <returns>  <tt>true</tt> if, and only if, the given byte array
		'''          is a legal replacement value for this encoder </returns>
		Public Overridable Function isLegalReplacement(ByVal repl As SByte()) As Boolean
			Dim wr As WeakReference(Of CharsetDecoder) = cachedDecoder
			Dim dec As CharsetDecoder = Nothing
			dec = wr.get()
			If (wr Is Nothing) OrElse (dec Is Nothing) Then
				dec = charset().newDecoder()
				dec.onMalformedInput(CodingErrorAction.REPORT)
				dec.onUnmappableCharacter(CodingErrorAction.REPORT)
				cachedDecoder = New WeakReference(Of CharsetDecoder)(dec)
			Else
				dec.reset()
			End If
			Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.wrap(repl)
			Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.allocate(CInt(Fix(bb.remaining() * dec.maxCharsPerByte())))
			Dim cr As CoderResult = dec.decode(bb, cb, True)
			Return Not cr.error
		End Function



		''' <summary>
		''' Returns this encoder's current action for malformed-input errors.
		''' </summary>
		''' <returns> The current malformed-input action, which is never <tt>null</tt> </returns>
		Public Overridable Function malformedInputAction() As CodingErrorAction
			Return malformedInputAction_Renamed
		End Function

		''' <summary>
		''' Changes this encoder's action for malformed-input errors.
		''' 
		''' <p> This method invokes the {@link #implOnMalformedInput
		''' implOnMalformedInput} method, passing the new action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action; must not be <tt>null</tt>
		''' </param>
		''' <returns>  This encoder
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''         If the precondition on the parameter does not hold </exception>
		Public Function onMalformedInput(ByVal newAction As CodingErrorAction) As CharsetEncoder
			If newAction Is Nothing Then Throw New IllegalArgumentException("Null action")
			malformedInputAction_Renamed = newAction
			implOnMalformedInput(newAction)
			Return Me
		End Function

		''' <summary>
		''' Reports a change to this encoder's malformed-input action.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by encoders that require notification of changes to
		''' the malformed-input action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action </param>
		Protected Friend Overridable Sub implOnMalformedInput(ByVal newAction As CodingErrorAction)
		End Sub

		''' <summary>
		''' Returns this encoder's current action for unmappable-character errors.
		''' </summary>
		''' <returns> The current unmappable-character action, which is never
		'''         <tt>null</tt> </returns>
		Public Overridable Function unmappableCharacterAction() As CodingErrorAction
			Return unmappableCharacterAction_Renamed
		End Function

		''' <summary>
		''' Changes this encoder's action for unmappable-character errors.
		''' 
		''' <p> This method invokes the {@link #implOnUnmappableCharacter
		''' implOnUnmappableCharacter} method, passing the new action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action; must not be <tt>null</tt>
		''' </param>
		''' <returns>  This encoder
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''         If the precondition on the parameter does not hold </exception>
		Public Function onUnmappableCharacter(ByVal newAction As CodingErrorAction) As CharsetEncoder
			If newAction Is Nothing Then Throw New IllegalArgumentException("Null action")
			unmappableCharacterAction_Renamed = newAction
			implOnUnmappableCharacter(newAction)
			Return Me
		End Function

		''' <summary>
		''' Reports a change to this encoder's unmappable-character action.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by encoders that require notification of changes to
		''' the unmappable-character action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action </param>
		Protected Friend Overridable Sub implOnUnmappableCharacter(ByVal newAction As CodingErrorAction)
		End Sub

		''' <summary>
		''' Returns the average number of bytes that will be produced for each
		''' character of input.  This heuristic value may be used to estimate the size
		''' of the output buffer required for a given input sequence.
		''' </summary>
		''' <returns>  The average number of bytes produced
		'''          per character of input </returns>
		Public Function averageBytesPerChar() As Single
			Return averageBytesPerChar_Renamed
		End Function

		''' <summary>
		''' Returns the maximum number of bytes that will be produced for each
		''' character of input.  This value may be used to compute the worst-case size
		''' of the output buffer required for a given input sequence.
		''' </summary>
		''' <returns>  The maximum number of bytes that will be produced per
		'''          character of input </returns>
		Public Function maxBytesPerChar() As Single
			Return maxBytesPerChar_Renamed
		End Function

		''' <summary>
		''' Encodes as many characters as possible from the given input buffer,
		''' writing the results to the given output buffer.
		''' 
		''' <p> The buffers are read from, and written to, starting at their current
		''' positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> characters
		''' will be read and at most <seealso cref="Buffer#remaining out.remaining()"/>
		''' bytes will be written.  The buffers' positions will be advanced to
		''' reflect the characters read and the bytes written, but their marks and
		''' limits will not be modified.
		''' 
		''' <p> In addition to reading characters from the input buffer and writing
		''' bytes to the output buffer, this method returns a <seealso cref="CoderResult"/>
		''' object to describe its reason for termination:
		''' 
		''' <ul>
		''' 
		'''   <li><p> <seealso cref="CoderResult#UNDERFLOW"/> indicates that as much of the
		'''   input buffer as possible has been encoded.  If there is no further
		'''   input then the invoker can proceed to the next step of the
		'''   <a href="#steps">encoding operation</a>.  Otherwise this method
		'''   should be invoked again with further input.  </p></li>
		''' 
		'''   <li><p> <seealso cref="CoderResult#OVERFLOW"/> indicates that there is
		'''   insufficient space in the output buffer to encode any more characters.
		'''   This method should be invoked again with an output buffer that has
		'''   more <seealso cref="Buffer#remaining remaining"/> bytes. This is
		'''   typically done by draining any encoded bytes from the output
		'''   buffer.  </p></li>
		''' 
		'''   <li><p> A {@link CoderResult#malformedForLength
		'''   malformed-input} result indicates that a malformed-input
		'''   error has been detected.  The malformed characters begin at the input
		'''   buffer's (possibly incremented) position; the number of malformed
		'''   characters may be determined by invoking the result object's {@link
		'''   CoderResult#length() length} method.  This case applies only if the
		'''   <seealso cref="#onMalformedInput malformed action"/> of this encoder
		'''   is <seealso cref="CodingErrorAction#REPORT"/>; otherwise the malformed input
		'''   will be ignored or replaced, as requested.  </p></li>
		''' 
		'''   <li><p> An {@link CoderResult#unmappableForLength
		'''   unmappable-character} result indicates that an
		'''   unmappable-character error has been detected.  The characters that
		'''   encode the unmappable character begin at the input buffer's (possibly
		'''   incremented) position; the number of such characters may be determined
		'''   by invoking the result object's <seealso cref="CoderResult#length() length"/>
		'''   method.  This case applies only if the {@link #onUnmappableCharacter
		'''   unmappable action} of this encoder is {@link
		'''   CodingErrorAction#REPORT}; otherwise the unmappable character will be
		'''   ignored or replaced, as requested.  </p></li>
		''' 
		''' </ul>
		''' 
		''' In any case, if this method is to be reinvoked in the same encoding
		''' operation then care should be taken to preserve any characters remaining
		''' in the input buffer so that they are available to the next invocation.
		''' 
		''' <p> The <tt>endOfInput</tt> parameter advises this method as to whether
		''' the invoker can provide further input beyond that contained in the given
		''' input buffer.  If there is a possibility of providing additional input
		''' then the invoker should pass <tt>false</tt> for this parameter; if there
		''' is no possibility of providing further input then the invoker should
		''' pass <tt>true</tt>.  It is not erroneous, and in fact it is quite
		''' common, to pass <tt>false</tt> in one invocation and later discover that
		''' no further input was actually available.  It is critical, however, that
		''' the final invocation of this method in a sequence of invocations always
		''' pass <tt>true</tt> so that any remaining unencoded input will be treated
		''' as being malformed.
		''' 
		''' <p> This method works by invoking the <seealso cref="#encodeLoop encodeLoop"/>
		''' method, interpreting its results, handling error conditions, and
		''' reinvoking it as necessary.  </p>
		''' 
		''' </summary>
		''' <param name="in">
		'''         The input character buffer
		''' </param>
		''' <param name="out">
		'''         The output byte buffer
		''' </param>
		''' <param name="endOfInput">
		'''         <tt>true</tt> if, and only if, the invoker can provide no
		'''         additional input characters beyond those in the given buffer
		''' </param>
		''' <returns>  A coder-result object describing the reason for termination
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If an encoding operation is already in progress and the previous
		'''          step was an invocation neither of the <seealso cref="#reset reset"/>
		'''          method, nor of this method with a value of <tt>false</tt> for
		'''          the <tt>endOfInput</tt> parameter, nor of this method with a
		'''          value of <tt>true</tt> for the <tt>endOfInput</tt> parameter
		'''          but a return value indicating an incomplete encoding operation
		''' </exception>
		''' <exception cref="CoderMalfunctionError">
		'''          If an invocation of the encodeLoop method threw
		'''          an unexpected exception </exception>
		Public Function encode(ByVal [in] As java.nio.CharBuffer, ByVal out As java.nio.ByteBuffer, ByVal endOfInput As Boolean) As CoderResult
			Dim newState As Integer = If(endOfInput, ST_END, ST_CODING)
			If (state <> ST_RESET) AndAlso (state <> ST_CODING) AndAlso Not(endOfInput AndAlso (state = ST_END)) Then throwIllegalStateException(state, newState)
			state = newState

			Do

				Dim cr As CoderResult
				Try
					cr = encodeLoop([in], out)
				Catch x As java.nio.BufferUnderflowException
					Throw New java.nio.charset.CoderMalfunctionError(x)
				Catch x As java.nio.BufferOverflowException
					Throw New java.nio.charset.CoderMalfunctionError(x)
				End Try

				If cr.overflow Then Return cr

				If cr.underflow Then
					If endOfInput AndAlso [in].hasRemaining() Then
						cr = CoderResult.malformedForLength([in].remaining())
						' Fall through to malformed-input case
					Else
						Return cr
					End If
				End If

				Dim action As CodingErrorAction = Nothing
				If cr.malformed Then
					action = malformedInputAction_Renamed
				ElseIf cr.unmappable Then
					action = unmappableCharacterAction_Renamed
				Else
					Debug.Assert(False, cr.ToString())
				End If

				If action Is CodingErrorAction.REPORT Then Return cr

				If action Is CodingErrorAction.REPLACE Then
					If out.remaining() < replacement_Renamed.Length Then Return CoderResult.OVERFLOW
					out.put(replacement_Renamed)
				End If

				If (action Is CodingErrorAction.IGNORE) OrElse (action Is CodingErrorAction.REPLACE) Then
					' Skip erroneous input either way
					[in].position([in].position() + cr.length())
					Continue Do
				End If

				Debug.Assert(False)
			Loop

		End Function

		''' <summary>
		''' Flushes this encoder.
		''' 
		''' <p> Some encoders maintain internal state and may need to write some
		''' final bytes to the output buffer once the overall input sequence has
		''' been read.
		''' 
		''' <p> Any additional output is written to the output buffer beginning at
		''' its current position.  At most <seealso cref="Buffer#remaining out.remaining()"/>
		''' bytes will be written.  The buffer's position will be advanced
		''' appropriately, but its mark and limit will not be modified.
		''' 
		''' <p> If this method completes successfully then it returns {@link
		''' CoderResult#UNDERFLOW}.  If there is insufficient room in the output
		''' buffer then it returns <seealso cref="CoderResult#OVERFLOW"/>.  If this happens
		''' then this method must be invoked again, with an output buffer that has
		''' more room, in order to complete the current <a href="#steps">encoding
		''' operation</a>.
		''' 
		''' <p> If this encoder has already been flushed then invoking this method
		''' has no effect.
		''' 
		''' <p> This method invokes the <seealso cref="#implFlush implFlush"/> method to
		''' perform the actual flushing operation.  </p>
		''' </summary>
		''' <param name="out">
		'''         The output byte buffer
		''' </param>
		''' <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		'''          <seealso cref="CoderResult#OVERFLOW"/>
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If the previous step of the current encoding operation was an
		'''          invocation neither of the <seealso cref="#flush flush"/> method nor of
		'''          the three-argument {@link
		'''          #encode(CharBuffer,ByteBuffer,boolean) encode} method
		'''          with a value of <tt>true</tt> for the <tt>endOfInput</tt>
		'''          parameter </exception>
		Public Function flush(ByVal out As java.nio.ByteBuffer) As CoderResult
			If state = ST_END Then
				Dim cr As CoderResult = implFlush(out)
				If cr.underflow Then state = ST_FLUSHED
				Return cr
			End If

			If state <> ST_FLUSHED Then throwIllegalStateException(state, ST_FLUSHED)

			Return CoderResult.UNDERFLOW ' Already flushed
		End Function

		''' <summary>
		''' Flushes this encoder.
		''' 
		''' <p> The default implementation of this method does nothing, and always
		''' returns <seealso cref="CoderResult#UNDERFLOW"/>.  This method should be overridden
		''' by encoders that may need to write final bytes to the output buffer
		''' once the entire input sequence has been read. </p>
		''' </summary>
		''' <param name="out">
		'''         The output byte buffer
		''' </param>
		''' <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		'''          <seealso cref="CoderResult#OVERFLOW"/> </returns>
		Protected Friend Overridable Function implFlush(ByVal out As java.nio.ByteBuffer) As CoderResult
			Return CoderResult.UNDERFLOW
		End Function

		''' <summary>
		''' Resets this encoder, clearing any internal state.
		''' 
		''' <p> This method resets charset-independent state and also invokes the
		''' <seealso cref="#implReset() implReset"/> method in order to perform any
		''' charset-specific reset actions.  </p>
		''' </summary>
		''' <returns>  This encoder
		'''  </returns>
		Public Function reset() As CharsetEncoder
			implReset()
			state = ST_RESET
			Return Me
		End Function

		''' <summary>
		''' Resets this encoder, clearing any charset-specific internal state.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by encoders that maintain internal state.  </p>
		''' </summary>
		Protected Friend Overridable Sub implReset()
		End Sub

		''' <summary>
		''' Encodes one or more characters into one or more bytes.
		''' 
		''' <p> This method encapsulates the basic encoding loop, encoding as many
		''' characters as possible until it either runs out of input, runs out of room
		''' in the output buffer, or encounters an encoding error.  This method is
		''' invoked by the <seealso cref="#encode encode"/> method, which handles result
		''' interpretation and error recovery.
		''' 
		''' <p> The buffers are read from, and written to, starting at their current
		''' positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> characters
		''' will be read, and at most <seealso cref="Buffer#remaining out.remaining()"/>
		''' bytes will be written.  The buffers' positions will be advanced to
		''' reflect the characters read and the bytes written, but their marks and
		''' limits will not be modified.
		''' 
		''' <p> This method returns a <seealso cref="CoderResult"/> object to describe its
		''' reason for termination, in the same manner as the <seealso cref="#encode encode"/>
		''' method.  Most implementations of this method will handle encoding errors
		''' by returning an appropriate result object for interpretation by the
		''' <seealso cref="#encode encode"/> method.  An optimized implementation may instead
		''' examine the relevant error action and implement that action itself.
		''' 
		''' <p> An implementation of this method may perform arbitrary lookahead by
		''' returning <seealso cref="CoderResult#UNDERFLOW"/> until it receives sufficient
		''' input.  </p>
		''' </summary>
		''' <param name="in">
		'''         The input character buffer
		''' </param>
		''' <param name="out">
		'''         The output byte buffer
		''' </param>
		''' <returns>  A coder-result object describing the reason for termination </returns>
		Protected Friend MustOverride Function encodeLoop(ByVal [in] As java.nio.CharBuffer, ByVal out As java.nio.ByteBuffer) As CoderResult

		''' <summary>
		''' Convenience method that encodes the remaining content of a single input
		''' character buffer into a newly-allocated byte buffer.
		''' 
		''' <p> This method implements an entire <a href="#steps">encoding
		''' operation</a>; that is, it resets this encoder, then it encodes the
		''' characters in the given character buffer, and finally it flushes this
		''' encoder.  This method should therefore not be invoked if an encoding
		''' operation is already in progress.  </p>
		''' </summary>
		''' <param name="in">
		'''         The input character buffer
		''' </param>
		''' <returns> A newly-allocated byte buffer containing the result of the
		'''         encoding operation.  The buffer's position will be zero and its
		'''         limit will follow the last byte written.
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If an encoding operation is already in progress
		''' </exception>
		''' <exception cref="MalformedInputException">
		'''          If the character sequence starting at the input buffer's current
		'''          position is not a legal sixteen-bit Unicode sequence and the current malformed-input action
		'''          is <seealso cref="CodingErrorAction#REPORT"/>
		''' </exception>
		''' <exception cref="UnmappableCharacterException">
		'''          If the character sequence starting at the input buffer's current
		'''          position cannot be mapped to an equivalent byte sequence and
		'''          the current unmappable-character action is {@link
		'''          CodingErrorAction#REPORT} </exception>
		Public Function encode(ByVal [in] As java.nio.CharBuffer) As java.nio.ByteBuffer
			Dim n As Integer = CInt(Fix([in].remaining() * averageBytesPerChar()))
			Dim out As java.nio.ByteBuffer = java.nio.ByteBuffer.allocate(n)

			If (n = 0) AndAlso ([in].remaining() = 0) Then Return out
			reset()
			Do
				Dim cr As CoderResult = If([in].hasRemaining(), encode([in], out, True), CoderResult.UNDERFLOW)
				If cr.underflow Then cr = flush(out)

				If cr.underflow Then Exit Do
				If cr.overflow Then
					n = 2*n + 1 ' Ensure progress; n might be 0!
					Dim o As java.nio.ByteBuffer = java.nio.ByteBuffer.allocate(n)
					out.flip()
					o.put(out)
					out = o
					Continue Do
				End If
				cr.throwException()
			Loop
			out.flip()
			Return out
		End Function















































































		Private Function canEncode(ByVal cb As java.nio.CharBuffer) As Boolean
			If state = ST_FLUSHED Then
				reset()
			ElseIf state <> ST_RESET Then
				throwIllegalStateException(state, ST_CODING)
			End If
			Dim ma As CodingErrorAction = malformedInputAction()
			Dim ua As CodingErrorAction = unmappableCharacterAction()
			Try
				onMalformedInput(CodingErrorAction.REPORT)
				onUnmappableCharacter(CodingErrorAction.REPORT)
				encode(cb)
			Catch x As CharacterCodingException
				Return False
			Finally
				onMalformedInput(ma)
				onUnmappableCharacter(ua)
				reset()
			End Try
			Return True
		End Function

		''' <summary>
		''' Tells whether or not this encoder can encode the given character.
		''' 
		''' <p> This method returns <tt>false</tt> if the given character is a
		''' surrogate character; such characters can be interpreted only when they
		''' are members of a pair consisting of a high surrogate followed by a low
		''' surrogate.  The {@link #canEncode(java.lang.CharSequence)
		''' canEncode(CharSequence)} method may be used to test whether or not a
		''' character sequence can be encoded.
		''' 
		''' <p> This method may modify this encoder's state; it should therefore not
		''' be invoked if an <a href="#steps">encoding operation</a> is already in
		''' progress.
		''' 
		''' <p> The default implementation of this method is not very efficient; it
		''' should generally be overridden to improve performance.  </p>
		''' </summary>
		''' <param name="c">
		'''          The given character
		''' </param>
		''' <returns>  <tt>true</tt> if, and only if, this encoder can encode
		'''          the given character
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If an encoding operation is already in progress </exception>
		Public Overridable Function canEncode(ByVal c As Char) As Boolean
			Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.allocate(1)
			cb.put(c)
			cb.flip()
			Return canEncode(cb)
		End Function

		''' <summary>
		''' Tells whether or not this encoder can encode the given character
		''' sequence.
		''' 
		''' <p> If this method returns <tt>false</tt> for a particular character
		''' sequence then more information about why the sequence cannot be encoded
		''' may be obtained by performing a full <a href="#steps">encoding
		''' operation</a>.
		''' 
		''' <p> This method may modify this encoder's state; it should therefore not
		''' be invoked if an encoding operation is already in progress.
		''' 
		''' <p> The default implementation of this method is not very efficient; it
		''' should generally be overridden to improve performance.  </p>
		''' </summary>
		''' <param name="cs">
		'''          The given character sequence
		''' </param>
		''' <returns>  <tt>true</tt> if, and only if, this encoder can encode
		'''          the given character without throwing any exceptions and without
		'''          performing any replacements
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If an encoding operation is already in progress </exception>
		Public Overridable Function canEncode(ByVal cs As CharSequence) As Boolean
			Dim cb As java.nio.CharBuffer
			If TypeOf cs Is java.nio.CharBuffer Then
				cb = CType(cs, java.nio.CharBuffer).duplicate()
			Else
				cb = java.nio.CharBuffer.wrap(cs.ToString())
			End If
			Return canEncode(cb)
		End Function




		Private Sub throwIllegalStateException(ByVal [from] As Integer, ByVal [to] As Integer)
			Throw New IllegalStateException("Current state = " & stateNames([from]) & ", new state = " & stateNames([to]))
		End Sub

	End Class

End Namespace
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
	''' An engine that can transform a sequence of bytes in a specific charset into a sequence of
	''' sixteen-bit Unicode characters.
	''' 
	''' <a name="steps"></a>
	''' 
	''' <p> The input byte sequence is provided in a byte buffer or a series
	''' of such buffers.  The output character sequence is written to a character buffer
	''' or a series of such buffers.  A decoder should always be used by making
	''' the following sequence of method invocations, hereinafter referred to as a
	''' <i>decoding operation</i>:
	''' 
	''' <ol>
	''' 
	'''   <li><p> Reset the decoder via the <seealso cref="#reset reset"/> method, unless it
	'''   has not been used before; </p></li>
	''' 
	'''   <li><p> Invoke the <seealso cref="#decode decode"/> method zero or more times, as
	'''   long as additional input may be available, passing <tt>false</tt> for the
	'''   <tt>endOfInput</tt> argument and filling the input buffer and flushing the
	'''   output buffer between invocations; </p></li>
	''' 
	'''   <li><p> Invoke the <seealso cref="#decode decode"/> method one final time, passing
	'''   <tt>true</tt> for the <tt>endOfInput</tt> argument; and then </p></li>
	''' 
	'''   <li><p> Invoke the <seealso cref="#flush flush"/> method so that the decoder can
	'''   flush any internal state to the output buffer. </p></li>
	''' 
	''' </ol>
	''' 
	''' Each invocation of the <seealso cref="#decode decode"/> method will decode as many
	''' bytes as possible from the input buffer, writing the resulting characters
	''' to the output buffer.  The <seealso cref="#decode decode"/> method returns when more
	''' input is required, when there is not enough room in the output buffer, or
	''' when a decoding error has occurred.  In each case a <seealso cref="CoderResult"/>
	''' object is returned to describe the reason for termination.  An invoker can
	''' examine this object and fill the input buffer, flush the output buffer, or
	''' attempt to recover from a decoding error, as appropriate, and try again.
	''' 
	''' <a name="ce"></a>
	''' 
	''' <p> There are two general types of decoding errors.  If the input byte
	''' sequence is not legal for this charset then the input is considered <i>malformed</i>.  If
	''' the input byte sequence is legal but cannot be mapped to a valid
	''' Unicode character then an <i>unmappable character</i> has been encountered.
	''' 
	''' <a name="cae"></a>
	''' 
	''' <p> How a decoding error is handled depends upon the action requested for
	''' that type of error, which is described by an instance of the {@link
	''' CodingErrorAction} class.  The possible error actions are to {@linkplain
	''' CodingErrorAction#IGNORE ignore} the erroneous input, {@linkplain
	''' CodingErrorAction#REPORT report} the error to the invoker via
	''' the returned <seealso cref="CoderResult"/> object, or {@link CodingErrorAction#REPLACE
	''' replace} the erroneous input with the current value of the
	''' replacement string.  The replacement
	''' 
	''' 
	''' 
	''' 
	''' 
	''' 
	''' has the initial value <tt>"&#92;uFFFD"</tt>;
	''' 
	''' 
	''' its value may be changed via the {@link #replaceWith(java.lang.String)
	''' replaceWith} method.
	''' 
	''' <p> The default action for malformed-input and unmappable-character errors
	''' is to <seealso cref="CodingErrorAction#REPORT report"/> them.  The
	''' malformed-input error action may be changed via the {@link
	''' #onMalformedInput(CodingErrorAction) onMalformedInput} method; the
	''' unmappable-character action may be changed via the {@link
	''' #onUnmappableCharacter(CodingErrorAction) onUnmappableCharacter} method.
	''' 
	''' <p> This class is designed to handle many of the details of the decoding
	''' process, including the implementation of error actions.  A decoder for a
	''' specific charset, which is a concrete subclass of this class, need only
	''' implement the abstract <seealso cref="#decodeLoop decodeLoop"/> method, which
	''' encapsulates the basic decoding loop.  A subclass that maintains internal
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
	''' <seealso cref= CharsetEncoder </seealso>

	Public MustInherit Class CharsetDecoder

		Private ReadOnly charset_Renamed As Charset
		Private ReadOnly averageCharsPerByte_Renamed As Single
		Private ReadOnly maxCharsPerByte_Renamed As Single

		Private replacement_Renamed As String
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
		''' Initializes a new decoder.  The new decoder will have the given
		''' chars-per-byte and replacement values.
		''' </summary>
		''' <param name="cs">
		'''         The charset that created this decoder
		''' </param>
		''' <param name="averageCharsPerByte">
		'''         A positive float value indicating the expected number of
		'''         characters that will be produced for each input byte
		''' </param>
		''' <param name="maxCharsPerByte">
		'''         A positive float value indicating the maximum number of
		'''         characters that will be produced for each input byte
		''' </param>
		''' <param name="replacement">
		'''         The initial replacement; must not be <tt>null</tt>, must have
		'''         non-zero length, must not be longer than maxCharsPerByte,
		'''         and must be <seealso cref="#isLegalReplacement legal"/>
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		Private Sub New(ByVal cs As Charset, ByVal averageCharsPerByte As Single, ByVal maxCharsPerByte As Single, ByVal replacement As String)
			Me.charset_Renamed = cs
			If averageCharsPerByte <= 0.0f Then Throw New IllegalArgumentException("Non-positive " & "averageCharsPerByte")
			If maxCharsPerByte <= 0.0f Then Throw New IllegalArgumentException("Non-positive " & "maxCharsPerByte")
			If Not Charset.atBugLevel("1.4") Then
				If averageCharsPerByte > maxCharsPerByte Then Throw New IllegalArgumentException("averageCharsPerByte" & " exceeds " & "maxCharsPerByte")
			End If
			Me.replacement_Renamed = replacement
			Me.averageCharsPerByte_Renamed = averageCharsPerByte
			Me.maxCharsPerByte_Renamed = maxCharsPerByte
			replaceWith(replacement)
		End Sub

		''' <summary>
		''' Initializes a new decoder.  The new decoder will have the given
		''' chars-per-byte values and its replacement will be the
		''' string <tt>"&#92;uFFFD"</tt>.
		''' </summary>
		''' <param name="cs">
		'''         The charset that created this decoder
		''' </param>
		''' <param name="averageCharsPerByte">
		'''         A positive float value indicating the expected number of
		'''         characters that will be produced for each input byte
		''' </param>
		''' <param name="maxCharsPerByte">
		'''         A positive float value indicating the maximum number of
		'''         characters that will be produced for each input byte
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameters do not hold </exception>
		Protected Friend Sub New(ByVal cs As Charset, ByVal averageCharsPerByte As Single, ByVal maxCharsPerByte As Single)
			Me.New(cs, averageCharsPerByte, maxCharsPerByte, ChrW(&HFFFD).ToString())
		End Sub

		''' <summary>
		''' Returns the charset that created this decoder.
		''' </summary>
		''' <returns>  This decoder's charset </returns>
		Public Function charset() As Charset
			Return charset_Renamed
		End Function

		''' <summary>
		''' Returns this decoder's replacement value.
		''' </summary>
		''' <returns>  This decoder's current replacement,
		'''          which is never <tt>null</tt> and is never empty </returns>
		Public Function replacement() As String

			Return replacement_Renamed




		End Function

		''' <summary>
		''' Changes this decoder's replacement value.
		''' 
		''' <p> This method invokes the <seealso cref="#implReplaceWith implReplaceWith"/>
		''' method, passing the new replacement, after checking that the new
		''' replacement is acceptable.  </p>
		''' </summary>
		''' <param name="newReplacement">  The replacement value
		''' 
		''' 
		'''         The new replacement; must not be <tt>null</tt>
		'''         and must have non-zero length
		''' 
		''' 
		''' 
		''' 
		''' 
		''' 
		''' 
		''' </param>
		''' <returns>  This decoder
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameter do not hold </exception>
		Public Function replaceWith(ByVal newReplacement As String) As CharsetDecoder
			If newReplacement Is Nothing Then Throw New IllegalArgumentException("Null replacement")
			Dim len As Integer = newReplacement.length()
			If len = 0 Then Throw New IllegalArgumentException("Empty replacement")
			If len > maxCharsPerByte_Renamed Then Throw New IllegalArgumentException("Replacement too long")

			Me.replacement_Renamed = newReplacement






			implReplaceWith(Me.replacement_Renamed)
			Return Me
		End Function

		''' <summary>
		''' Reports a change to this decoder's replacement value.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by decoders that require notification of changes to
		''' the replacement.  </p>
		''' </summary>
		''' <param name="newReplacement">    The replacement value </param>
		Protected Friend Overridable Sub implReplaceWith(ByVal newReplacement As String)
		End Sub









































		''' <summary>
		''' Returns this decoder's current action for malformed-input errors.
		''' </summary>
		''' <returns> The current malformed-input action, which is never <tt>null</tt> </returns>
		Public Overridable Function malformedInputAction() As CodingErrorAction
			Return malformedInputAction_Renamed
		End Function

		''' <summary>
		''' Changes this decoder's action for malformed-input errors.
		''' 
		''' <p> This method invokes the {@link #implOnMalformedInput
		''' implOnMalformedInput} method, passing the new action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action; must not be <tt>null</tt>
		''' </param>
		''' <returns>  This decoder
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''         If the precondition on the parameter does not hold </exception>
		Public Function onMalformedInput(ByVal newAction As CodingErrorAction) As CharsetDecoder
			If newAction Is Nothing Then Throw New IllegalArgumentException("Null action")
			malformedInputAction_Renamed = newAction
			implOnMalformedInput(newAction)
			Return Me
		End Function

		''' <summary>
		''' Reports a change to this decoder's malformed-input action.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by decoders that require notification of changes to
		''' the malformed-input action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action </param>
		Protected Friend Overridable Sub implOnMalformedInput(ByVal newAction As CodingErrorAction)
		End Sub

		''' <summary>
		''' Returns this decoder's current action for unmappable-character errors.
		''' </summary>
		''' <returns> The current unmappable-character action, which is never
		'''         <tt>null</tt> </returns>
		Public Overridable Function unmappableCharacterAction() As CodingErrorAction
			Return unmappableCharacterAction_Renamed
		End Function

		''' <summary>
		''' Changes this decoder's action for unmappable-character errors.
		''' 
		''' <p> This method invokes the {@link #implOnUnmappableCharacter
		''' implOnUnmappableCharacter} method, passing the new action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action; must not be <tt>null</tt>
		''' </param>
		''' <returns>  This decoder
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''         If the precondition on the parameter does not hold </exception>
		Public Function onUnmappableCharacter(ByVal newAction As CodingErrorAction) As CharsetDecoder
			If newAction Is Nothing Then Throw New IllegalArgumentException("Null action")
			unmappableCharacterAction_Renamed = newAction
			implOnUnmappableCharacter(newAction)
			Return Me
		End Function

		''' <summary>
		''' Reports a change to this decoder's unmappable-character action.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by decoders that require notification of changes to
		''' the unmappable-character action.  </p>
		''' </summary>
		''' <param name="newAction">  The new action </param>
		Protected Friend Overridable Sub implOnUnmappableCharacter(ByVal newAction As CodingErrorAction)
		End Sub

		''' <summary>
		''' Returns the average number of characters that will be produced for each
		''' byte of input.  This heuristic value may be used to estimate the size
		''' of the output buffer required for a given input sequence.
		''' </summary>
		''' <returns>  The average number of characters produced
		'''          per byte of input </returns>
		Public Function averageCharsPerByte() As Single
			Return averageCharsPerByte_Renamed
		End Function

		''' <summary>
		''' Returns the maximum number of characters that will be produced for each
		''' byte of input.  This value may be used to compute the worst-case size
		''' of the output buffer required for a given input sequence.
		''' </summary>
		''' <returns>  The maximum number of characters that will be produced per
		'''          byte of input </returns>
		Public Function maxCharsPerByte() As Single
			Return maxCharsPerByte_Renamed
		End Function

		''' <summary>
		''' Decodes as many bytes as possible from the given input buffer,
		''' writing the results to the given output buffer.
		''' 
		''' <p> The buffers are read from, and written to, starting at their current
		''' positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> bytes
		''' will be read and at most <seealso cref="Buffer#remaining out.remaining()"/>
		''' characters will be written.  The buffers' positions will be advanced to
		''' reflect the bytes read and the characters written, but their marks and
		''' limits will not be modified.
		''' 
		''' <p> In addition to reading bytes from the input buffer and writing
		''' characters to the output buffer, this method returns a <seealso cref="CoderResult"/>
		''' object to describe its reason for termination:
		''' 
		''' <ul>
		''' 
		'''   <li><p> <seealso cref="CoderResult#UNDERFLOW"/> indicates that as much of the
		'''   input buffer as possible has been decoded.  If there is no further
		'''   input then the invoker can proceed to the next step of the
		'''   <a href="#steps">decoding operation</a>.  Otherwise this method
		'''   should be invoked again with further input.  </p></li>
		''' 
		'''   <li><p> <seealso cref="CoderResult#OVERFLOW"/> indicates that there is
		'''   insufficient space in the output buffer to decode any more bytes.
		'''   This method should be invoked again with an output buffer that has
		'''   more <seealso cref="Buffer#remaining remaining"/> characters. This is
		'''   typically done by draining any decoded characters from the output
		'''   buffer.  </p></li>
		''' 
		'''   <li><p> A {@link CoderResult#malformedForLength
		'''   malformed-input} result indicates that a malformed-input
		'''   error has been detected.  The malformed bytes begin at the input
		'''   buffer's (possibly incremented) position; the number of malformed
		'''   bytes may be determined by invoking the result object's {@link
		'''   CoderResult#length() length} method.  This case applies only if the
		'''   <seealso cref="#onMalformedInput malformed action"/> of this decoder
		'''   is <seealso cref="CodingErrorAction#REPORT"/>; otherwise the malformed input
		'''   will be ignored or replaced, as requested.  </p></li>
		''' 
		'''   <li><p> An {@link CoderResult#unmappableForLength
		'''   unmappable-character} result indicates that an
		'''   unmappable-character error has been detected.  The bytes that
		'''   decode the unmappable character begin at the input buffer's (possibly
		'''   incremented) position; the number of such bytes may be determined
		'''   by invoking the result object's <seealso cref="CoderResult#length() length"/>
		'''   method.  This case applies only if the {@link #onUnmappableCharacter
		'''   unmappable action} of this decoder is {@link
		'''   CodingErrorAction#REPORT}; otherwise the unmappable character will be
		'''   ignored or replaced, as requested.  </p></li>
		''' 
		''' </ul>
		''' 
		''' In any case, if this method is to be reinvoked in the same decoding
		''' operation then care should be taken to preserve any bytes remaining
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
		''' pass <tt>true</tt> so that any remaining undecoded input will be treated
		''' as being malformed.
		''' 
		''' <p> This method works by invoking the <seealso cref="#decodeLoop decodeLoop"/>
		''' method, interpreting its results, handling error conditions, and
		''' reinvoking it as necessary.  </p>
		''' 
		''' </summary>
		''' <param name="in">
		'''         The input byte buffer
		''' </param>
		''' <param name="out">
		'''         The output character buffer
		''' </param>
		''' <param name="endOfInput">
		'''         <tt>true</tt> if, and only if, the invoker can provide no
		'''         additional input bytes beyond those in the given buffer
		''' </param>
		''' <returns>  A coder-result object describing the reason for termination
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If a decoding operation is already in progress and the previous
		'''          step was an invocation neither of the <seealso cref="#reset reset"/>
		'''          method, nor of this method with a value of <tt>false</tt> for
		'''          the <tt>endOfInput</tt> parameter, nor of this method with a
		'''          value of <tt>true</tt> for the <tt>endOfInput</tt> parameter
		'''          but a return value indicating an incomplete decoding operation
		''' </exception>
		''' <exception cref="CoderMalfunctionError">
		'''          If an invocation of the decodeLoop method threw
		'''          an unexpected exception </exception>
		Public Function decode(ByVal [in] As java.nio.ByteBuffer, ByVal out As java.nio.CharBuffer, ByVal endOfInput As Boolean) As CoderResult
			Dim newState As Integer = If(endOfInput, ST_END, ST_CODING)
			If (state <> ST_RESET) AndAlso (state <> ST_CODING) AndAlso Not(endOfInput AndAlso (state = ST_END)) Then throwIllegalStateException(state, newState)
			state = newState

			Do

				Dim cr As CoderResult
				Try
					cr = decodeLoop([in], out)
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
					If out.remaining() < replacement_Renamed.length() Then Return CoderResult.OVERFLOW
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
		''' Flushes this decoder.
		''' 
		''' <p> Some decoders maintain internal state and may need to write some
		''' final characters to the output buffer once the overall input sequence has
		''' been read.
		''' 
		''' <p> Any additional output is written to the output buffer beginning at
		''' its current position.  At most <seealso cref="Buffer#remaining out.remaining()"/>
		''' characters will be written.  The buffer's position will be advanced
		''' appropriately, but its mark and limit will not be modified.
		''' 
		''' <p> If this method completes successfully then it returns {@link
		''' CoderResult#UNDERFLOW}.  If there is insufficient room in the output
		''' buffer then it returns <seealso cref="CoderResult#OVERFLOW"/>.  If this happens
		''' then this method must be invoked again, with an output buffer that has
		''' more room, in order to complete the current <a href="#steps">decoding
		''' operation</a>.
		''' 
		''' <p> If this decoder has already been flushed then invoking this method
		''' has no effect.
		''' 
		''' <p> This method invokes the <seealso cref="#implFlush implFlush"/> method to
		''' perform the actual flushing operation.  </p>
		''' </summary>
		''' <param name="out">
		'''         The output character buffer
		''' </param>
		''' <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		'''          <seealso cref="CoderResult#OVERFLOW"/>
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If the previous step of the current decoding operation was an
		'''          invocation neither of the <seealso cref="#flush flush"/> method nor of
		'''          the three-argument {@link
		'''          #decode(ByteBuffer,CharBuffer,boolean) decode} method
		'''          with a value of <tt>true</tt> for the <tt>endOfInput</tt>
		'''          parameter </exception>
		Public Function flush(ByVal out As java.nio.CharBuffer) As CoderResult
			If state = ST_END Then
				Dim cr As CoderResult = implFlush(out)
				If cr.underflow Then state = ST_FLUSHED
				Return cr
			End If

			If state <> ST_FLUSHED Then throwIllegalStateException(state, ST_FLUSHED)

			Return CoderResult.UNDERFLOW ' Already flushed
		End Function

		''' <summary>
		''' Flushes this decoder.
		''' 
		''' <p> The default implementation of this method does nothing, and always
		''' returns <seealso cref="CoderResult#UNDERFLOW"/>.  This method should be overridden
		''' by decoders that may need to write final characters to the output buffer
		''' once the entire input sequence has been read. </p>
		''' </summary>
		''' <param name="out">
		'''         The output character buffer
		''' </param>
		''' <returns>  A coder-result object, either <seealso cref="CoderResult#UNDERFLOW"/> or
		'''          <seealso cref="CoderResult#OVERFLOW"/> </returns>
		Protected Friend Overridable Function implFlush(ByVal out As java.nio.CharBuffer) As CoderResult
			Return CoderResult.UNDERFLOW
		End Function

		''' <summary>
		''' Resets this decoder, clearing any internal state.
		''' 
		''' <p> This method resets charset-independent state and also invokes the
		''' <seealso cref="#implReset() implReset"/> method in order to perform any
		''' charset-specific reset actions.  </p>
		''' </summary>
		''' <returns>  This decoder
		'''  </returns>
		Public Function reset() As CharsetDecoder
			implReset()
			state = ST_RESET
			Return Me
		End Function

		''' <summary>
		''' Resets this decoder, clearing any charset-specific internal state.
		''' 
		''' <p> The default implementation of this method does nothing.  This method
		''' should be overridden by decoders that maintain internal state.  </p>
		''' </summary>
		Protected Friend Overridable Sub implReset()
		End Sub

		''' <summary>
		''' Decodes one or more bytes into one or more characters.
		''' 
		''' <p> This method encapsulates the basic decoding loop, decoding as many
		''' bytes as possible until it either runs out of input, runs out of room
		''' in the output buffer, or encounters a decoding error.  This method is
		''' invoked by the <seealso cref="#decode decode"/> method, which handles result
		''' interpretation and error recovery.
		''' 
		''' <p> The buffers are read from, and written to, starting at their current
		''' positions.  At most <seealso cref="Buffer#remaining in.remaining()"/> bytes
		''' will be read, and at most <seealso cref="Buffer#remaining out.remaining()"/>
		''' characters will be written.  The buffers' positions will be advanced to
		''' reflect the bytes read and the characters written, but their marks and
		''' limits will not be modified.
		''' 
		''' <p> This method returns a <seealso cref="CoderResult"/> object to describe its
		''' reason for termination, in the same manner as the <seealso cref="#decode decode"/>
		''' method.  Most implementations of this method will handle decoding errors
		''' by returning an appropriate result object for interpretation by the
		''' <seealso cref="#decode decode"/> method.  An optimized implementation may instead
		''' examine the relevant error action and implement that action itself.
		''' 
		''' <p> An implementation of this method may perform arbitrary lookahead by
		''' returning <seealso cref="CoderResult#UNDERFLOW"/> until it receives sufficient
		''' input.  </p>
		''' </summary>
		''' <param name="in">
		'''         The input byte buffer
		''' </param>
		''' <param name="out">
		'''         The output character buffer
		''' </param>
		''' <returns>  A coder-result object describing the reason for termination </returns>
		Protected Friend MustOverride Function decodeLoop(ByVal [in] As java.nio.ByteBuffer, ByVal out As java.nio.CharBuffer) As CoderResult

		''' <summary>
		''' Convenience method that decodes the remaining content of a single input
		''' byte buffer into a newly-allocated character buffer.
		''' 
		''' <p> This method implements an entire <a href="#steps">decoding
		''' operation</a>; that is, it resets this decoder, then it decodes the
		''' bytes in the given byte buffer, and finally it flushes this
		''' decoder.  This method should therefore not be invoked if a decoding
		''' operation is already in progress.  </p>
		''' </summary>
		''' <param name="in">
		'''         The input byte buffer
		''' </param>
		''' <returns> A newly-allocated character buffer containing the result of the
		'''         decoding operation.  The buffer's position will be zero and its
		'''         limit will follow the last character written.
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If a decoding operation is already in progress
		''' </exception>
		''' <exception cref="MalformedInputException">
		'''          If the byte sequence starting at the input buffer's current
		'''          position is not legal for this charset and the current malformed-input action
		'''          is <seealso cref="CodingErrorAction#REPORT"/>
		''' </exception>
		''' <exception cref="UnmappableCharacterException">
		'''          If the byte sequence starting at the input buffer's current
		'''          position cannot be mapped to an equivalent character sequence and
		'''          the current unmappable-character action is {@link
		'''          CodingErrorAction#REPORT} </exception>
		Public Function decode(ByVal [in] As java.nio.ByteBuffer) As java.nio.CharBuffer
			Dim n As Integer = CInt(Fix([in].remaining() * averageCharsPerByte()))
			Dim out As java.nio.CharBuffer = java.nio.CharBuffer.allocate(n)

			If (n = 0) AndAlso ([in].remaining() = 0) Then Return out
			reset()
			Do
				Dim cr As CoderResult = If([in].hasRemaining(), decode([in], out, True), CoderResult.UNDERFLOW)
				If cr.underflow Then cr = flush(out)

				If cr.underflow Then Exit Do
				If cr.overflow Then
					n = 2*n + 1 ' Ensure progress; n might be 0!
					Dim o As java.nio.CharBuffer = java.nio.CharBuffer.allocate(n)
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



		''' <summary>
		''' Tells whether or not this decoder implements an auto-detecting charset.
		''' 
		''' <p> The default implementation of this method always returns
		''' <tt>false</tt>; it should be overridden by auto-detecting decoders to
		''' return <tt>true</tt>.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this decoder implements an
		'''          auto-detecting charset </returns>
		Public Overridable Property autoDetecting As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Tells whether or not this decoder has yet detected a
		''' charset&nbsp;&nbsp;<i>(optional operation)</i>.
		''' 
		''' <p> If this decoder implements an auto-detecting charset then at a
		''' single point during a decoding operation this method may start returning
		''' <tt>true</tt> to indicate that a specific charset has been detected in
		''' the input byte sequence.  Once this occurs, the {@link #detectedCharset
		''' detectedCharset} method may be invoked to retrieve the detected charset.
		''' 
		''' <p> That this method returns <tt>false</tt> does not imply that no bytes
		''' have yet been decoded.  Some auto-detecting decoders are capable of
		''' decoding some, or even all, of an input byte sequence without fixing on
		''' a particular charset.
		''' 
		''' <p> The default implementation of this method always throws an {@link
		''' UnsupportedOperationException}; it should be overridden by
		''' auto-detecting decoders to return <tt>true</tt> once the input charset
		''' has been determined.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this decoder has detected a
		'''          specific charset
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''          If this decoder does not implement an auto-detecting charset </exception>
		Public Overridable Property charsetDetected As Boolean
			Get
				Throw New UnsupportedOperationException
			End Get
		End Property

		''' <summary>
		''' Retrieves the charset that was detected by this
		''' decoder&nbsp;&nbsp;<i>(optional operation)</i>.
		''' 
		''' <p> If this decoder implements an auto-detecting charset then this
		''' method returns the actual charset once it has been detected.  After that
		''' point, this method returns the same value for the duration of the
		''' current decoding operation.  If not enough input bytes have yet been
		''' read to determine the actual charset then this method throws an {@link
		''' IllegalStateException}.
		''' 
		''' <p> The default implementation of this method always throws an {@link
		''' UnsupportedOperationException}; it should be overridden by
		''' auto-detecting decoders to return the appropriate value.  </p>
		''' </summary>
		''' <returns>  The charset detected by this auto-detecting decoder,
		'''          or <tt>null</tt> if the charset has not yet been determined
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If insufficient bytes have been read to determine a charset
		''' </exception>
		''' <exception cref="UnsupportedOperationException">
		'''          If this decoder does not implement an auto-detecting charset </exception>
		Public Overridable Function detectedCharset() As Charset
			Throw New UnsupportedOperationException
		End Function
































































































		Private Sub throwIllegalStateException(ByVal [from] As Integer, ByVal [to] As Integer)
			Throw New IllegalStateException("Current state = " & stateNames([from]) & ", new state = " & stateNames([to]))
		End Sub

	End Class

End Namespace
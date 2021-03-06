Imports System

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

Namespace java.awt.event



	''' <summary>
	''' Input method events contain information about text that is being
	''' composed using an input method. Whenever the text changes, the
	''' input method sends an event. If the text component that's currently
	''' using the input method is an active client, the event is dispatched
	''' to that component. Otherwise, it is dispatched to a separate
	''' composition window.
	''' 
	''' <p>
	''' The text included with the input method event consists of two parts:
	''' committed text and composed text. Either part may be empty. The two
	''' parts together replace any uncommitted composed text sent in previous events,
	''' or the currently selected committed text.
	''' Committed text should be integrated into the text component's persistent
	''' data, it will not be sent again. Composed text may be sent repeatedly,
	''' with changes to reflect the user's editing operations. Committed text
	''' always precedes composed text.
	''' 
	''' @author JavaSoft Asia/Pacific
	''' @since 1.2
	''' </summary>
	Public Class InputMethodEvent
		Inherits java.awt.AWTEvent

		''' <summary>
		''' Serial Version ID.
		''' </summary>
		Private Const serialVersionUID As Long = 4727190874778922661L

        ''' <summary>
        ''' Marks the first integer id for the range of input method event ids.
        ''' </summary>
        Public Const INPUT_METHOD_FIRST As Integer = 1100

        ''' <summary>
        ''' The event type indicating changed input method text. This event is
        ''' generated by input methods while processing input.
        ''' </summary>
        Public Const INPUT_METHOD_TEXT_CHANGED As Integer = INPUT_METHOD_FIRST

        ''' <summary>
        ''' The event type indicating a changed insertion point in input method text.
        ''' This event is
        ''' generated by input methods while processing input if only the caret changed.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared ReadOnly CARET_POSITION_CHANGED As Integer = INPUT_METHOD_FIRST + 1

		''' <summary>
		''' Marks the last integer id for the range of input method event ids.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly INPUT_METHOD_LAST As Integer = INPUT_METHOD_FIRST + 1

		''' <summary>
		''' The time stamp that indicates when the event was created.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getWhen
		''' @since 1.4 </seealso>
		Friend [when] As Long

		' Text object
		<NonSerialized> _
		Private text As java.text.AttributedCharacterIterator
		<NonSerialized> _
		Private committedCharacterCount As Integer
		<NonSerialized> _
		Private caret As java.awt.font.TextHitInfo
		<NonSerialized> _
		Private visiblePosition As java.awt.font.TextHitInfo

		''' <summary>
		''' Constructs an <code>InputMethodEvent</code> with the specified
		''' source component, type, time, text, caret, and visiblePosition.
		''' <p>
		''' The offsets of caret and visiblePosition are relative to the current
		''' composed text; that is, the composed text within <code>text</code>
		''' if this is an <code>INPUT_METHOD_TEXT_CHANGED</code> event,
		''' the composed text within the <code>text</code> of the
		''' preceding <code>INPUT_METHOD_TEXT_CHANGED</code> event otherwise.
		''' <p>Note that passing in an invalid <code>id</code> results in
		''' unspecified behavior. This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> the object where the event originated </param>
		''' <param name="id"> the event type </param>
		''' <param name="when"> a long integer that specifies the time the event occurred </param>
		''' <param name="text"> the combined committed and composed text,
		'''      committed text first; must be <code>null</code>
		'''      when the event type is <code>CARET_POSITION_CHANGED</code>;
		'''      may be <code>null</code> for
		'''      <code>INPUT_METHOD_TEXT_CHANGED</code> if there's no
		'''      committed or composed text </param>
		''' <param name="committedCharacterCount"> the number of committed
		'''      characters in the text </param>
		''' <param name="caret"> the caret (a.k.a. insertion point);
		'''      <code>null</code> if there's no caret within current
		'''      composed text </param>
		''' <param name="visiblePosition"> the position that's most important
		'''      to be visible; <code>null</code> if there's no
		'''      recommendation for a visible position within current
		'''      composed text </param>
		''' <exception cref="IllegalArgumentException"> if <code>id</code> is not
		'''      in the range
		'''      <code>INPUT_METHOD_FIRST</code>..<code>INPUT_METHOD_LAST</code>;
		'''      or if id is <code>CARET_POSITION_CHANGED</code> and
		'''      <code>text</code> is not <code>null</code>;
		'''      or if <code>committedCharacterCount</code> is not in the range
		'''      <code>0</code>..<code>(text.getEndIndex() - text.getBeginIndex())</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null
		''' 
		''' @since 1.4 </exception>
		Public Sub New(  source As java.awt.Component,   id As Integer,   [when] As Long,   text As java.text.AttributedCharacterIterator,   committedCharacterCount As Integer,   caret As java.awt.font.TextHitInfo,   visiblePosition As java.awt.font.TextHitInfo)
			MyBase.New(source, id)
			If id < INPUT_METHOD_FIRST OrElse id > INPUT_METHOD_LAST Then Throw New IllegalArgumentException("id outside of valid range")

			If id = CARET_POSITION_CHANGED AndAlso text IsNot Nothing Then Throw New IllegalArgumentException("text must be null for CARET_POSITION_CHANGED")

			Me.when = [when]
			Me.text = text
			Dim textLength As Integer = 0
			If text IsNot Nothing Then textLength = text.endIndex - text.beginIndex

			If committedCharacterCount < 0 OrElse committedCharacterCount > textLength Then Throw New IllegalArgumentException("committedCharacterCount outside of valid range")
			Me.committedCharacterCount = committedCharacterCount

			Me.caret = caret
			Me.visiblePosition = visiblePosition
		End Sub

		''' <summary>
		''' Constructs an <code>InputMethodEvent</code> with the specified
		''' source component, type, text, caret, and visiblePosition.
		''' <p>
		''' The offsets of caret and visiblePosition are relative to the current
		''' composed text; that is, the composed text within <code>text</code>
		''' if this is an <code>INPUT_METHOD_TEXT_CHANGED</code> event,
		''' the composed text within the <code>text</code> of the
		''' preceding <code>INPUT_METHOD_TEXT_CHANGED</code> event otherwise.
		''' The time stamp for this event is initialized by invoking
		''' <seealso cref="java.awt.EventQueue#getMostRecentEventTime()"/>.
		''' <p>Note that passing in an invalid <code>id</code> results in
		''' unspecified behavior. This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> the object where the event originated </param>
		''' <param name="id"> the event type </param>
		''' <param name="text"> the combined committed and composed text,
		'''      committed text first; must be <code>null</code>
		'''      when the event type is <code>CARET_POSITION_CHANGED</code>;
		'''      may be <code>null</code> for
		'''      <code>INPUT_METHOD_TEXT_CHANGED</code> if there's no
		'''      committed or composed text </param>
		''' <param name="committedCharacterCount"> the number of committed
		'''      characters in the text </param>
		''' <param name="caret"> the caret (a.k.a. insertion point);
		'''      <code>null</code> if there's no caret within current
		'''      composed text </param>
		''' <param name="visiblePosition"> the position that's most important
		'''      to be visible; <code>null</code> if there's no
		'''      recommendation for a visible position within current
		'''      composed text </param>
		''' <exception cref="IllegalArgumentException"> if <code>id</code> is not
		'''      in the range
		'''      <code>INPUT_METHOD_FIRST</code>..<code>INPUT_METHOD_LAST</code>;
		'''      or if id is <code>CARET_POSITION_CHANGED</code> and
		'''      <code>text</code> is not <code>null</code>;
		'''      or if <code>committedCharacterCount</code> is not in the range
		'''      <code>0</code>..<code>(text.getEndIndex() - text.getBeginIndex())</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		Public Sub New(  source As java.awt.Component,   id As Integer,   text As java.text.AttributedCharacterIterator,   committedCharacterCount As Integer,   caret As java.awt.font.TextHitInfo,   visiblePosition As java.awt.font.TextHitInfo)
			Me.New(source, id, getMostRecentEventTimeForSource(source), text, committedCharacterCount, caret, visiblePosition)
		End Sub

		''' <summary>
		''' Constructs an <code>InputMethodEvent</code> with the
		''' specified source component, type, caret, and visiblePosition.
		''' The text is set to <code>null</code>,
		''' <code>committedCharacterCount</code> to 0.
		''' <p>
		''' The offsets of <code>caret</code> and <code>visiblePosition</code>
		''' are relative to the current composed text; that is,
		''' the composed text within the <code>text</code> of the
		''' preceding <code>INPUT_METHOD_TEXT_CHANGED</code> event if the
		''' event being constructed as a <code>CARET_POSITION_CHANGED</code> event.
		''' For an <code>INPUT_METHOD_TEXT_CHANGED</code> event without text,
		''' <code>caret</code> and <code>visiblePosition</code> must be
		''' <code>null</code>.
		''' The time stamp for this event is initialized by invoking
		''' <seealso cref="java.awt.EventQueue#getMostRecentEventTime()"/>.
		''' <p>Note that passing in an invalid <code>id</code> results in
		''' unspecified behavior. This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> the object where the event originated </param>
		''' <param name="id"> the event type </param>
		''' <param name="caret"> the caret (a.k.a. insertion point);
		'''      <code>null</code> if there's no caret within current
		'''      composed text </param>
		''' <param name="visiblePosition"> the position that's most important
		'''      to be visible; <code>null</code> if there's no
		'''      recommendation for a visible position within current
		'''      composed text </param>
		''' <exception cref="IllegalArgumentException"> if <code>id</code> is not
		'''      in the range
		'''      <code>INPUT_METHOD_FIRST</code>..<code>INPUT_METHOD_LAST</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		Public Sub New(  source As java.awt.Component,   id As Integer,   caret As java.awt.font.TextHitInfo,   visiblePosition As java.awt.font.TextHitInfo)
			Me.New(source, id, getMostRecentEventTimeForSource(source), Nothing, 0, caret, visiblePosition)
		End Sub

		''' <summary>
		''' Gets the combined committed and composed text.
		''' Characters from index 0 to index <code>getCommittedCharacterCount() - 1</code> are committed
		''' text, the remaining characters are composed text.
		''' </summary>
		''' <returns> the text.
		''' Always null for CARET_POSITION_CHANGED;
		''' may be null for INPUT_METHOD_TEXT_CHANGED if there's no composed or committed text. </returns>
		Public Overridable Property text As java.text.AttributedCharacterIterator
			Get
				Return text
			End Get
		End Property

		''' <summary>
		''' Gets the number of committed characters in the text.
		''' </summary>
		Public Overridable Property committedCharacterCount As Integer
			Get
				Return committedCharacterCount
			End Get
		End Property

		''' <summary>
		''' Gets the caret.
		''' <p>
		''' The offset of the caret is relative to the current
		''' composed text; that is, the composed text within getText()
		''' if this is an <code>INPUT_METHOD_TEXT_CHANGED</code> event,
		''' the composed text within getText() of the
		''' preceding <code>INPUT_METHOD_TEXT_CHANGED</code> event otherwise.
		''' </summary>
		''' <returns> the caret (a.k.a. insertion point).
		''' Null if there's no caret within current composed text. </returns>
		Public Overridable Property caret As java.awt.font.TextHitInfo
			Get
				Return caret
			End Get
		End Property

		''' <summary>
		''' Gets the position that's most important to be visible.
		''' <p>
		''' The offset of the visible position is relative to the current
		''' composed text; that is, the composed text within getText()
		''' if this is an <code>INPUT_METHOD_TEXT_CHANGED</code> event,
		''' the composed text within getText() of the
		''' preceding <code>INPUT_METHOD_TEXT_CHANGED</code> event otherwise.
		''' </summary>
		''' <returns> the position that's most important to be visible.
		''' Null if there's no recommendation for a visible position within current composed text. </returns>
		Public Overridable Property visiblePosition As java.awt.font.TextHitInfo
			Get
				Return visiblePosition
			End Get
		End Property

		''' <summary>
		''' Consumes this event so that it will not be processed
		''' in the default manner by the source which originated it.
		''' </summary>
		Public Overrides Sub consume()
			consumed = True
		End Sub

		''' <summary>
		''' Returns whether or not this event has been consumed. </summary>
		''' <seealso cref= #consume </seealso>
		Public  Overrides ReadOnly Property  consumed As Boolean
			Get
				Return consumed
			End Get
		End Property

		''' <summary>
		''' Returns the time stamp of when this event occurred.
		''' </summary>
		''' <returns> this event's timestamp
		''' @since 1.4 </returns>
		Public Overridable Property [when] As Long
			Get
			  Return [when]
			End Get
		End Property

		''' <summary>
		''' Returns a parameter string identifying this event.
		''' This method is useful for event-logging and for debugging.
		''' It contains the event ID in text form, the characters of the
		''' committed and composed text
		''' separated by "+", the number of committed characters,
		''' the caret, and the visible position.
		''' </summary>
		''' <returns> a string identifying the event and its attributes </returns>
		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case INPUT_METHOD_TEXT_CHANGED
				  typeStr = "INPUT_METHOD_TEXT_CHANGED"
			  Case CARET_POSITION_CHANGED
				  typeStr = "CARET_POSITION_CHANGED"
			  Case Else
				  typeStr = "unknown type"
			End Select

			Dim textString As String
			If text Is Nothing Then
				textString = "no text"
			Else
				Dim textBuffer As New StringBuilder("""")
				Dim committedCharacterCount_Renamed As Integer = Me.committedCharacterCount
				Dim c As Char = text.first()
				Dim tempVar As Boolean = committedCharacterCount_Renamed > 0
				committedCharacterCount_Renamed -= 1
				Do While tempVar
					textBuffer.append(c)
					c = text.next()
					tempVar = committedCharacterCount_Renamed > 0
					committedCharacterCount_Renamed -= 1
				Loop
				textBuffer.append(""" + """)
				Do While c <> java.text.CharacterIterator.DONE
					textBuffer.append(c)
					c = text.next()
				Loop
				textBuffer.append("""")
				textString = textBuffer.ToString()
			End If

			Dim countString As String = committedCharacterCount & " characters committed"

			Dim caretString As String
			If caret Is Nothing Then
				caretString = "no caret"
			Else
				caretString = "caret: " & caret.ToString()
			End If

			Dim visiblePositionString As String
			If visiblePosition Is Nothing Then
				visiblePositionString = "no visible position"
			Else
				visiblePositionString = "visible position: " & visiblePosition.ToString()
			End If

			Return typeStr & ", " & textString & ", " & countString & ", " & caretString & ", " & visiblePositionString
		End Function

		''' <summary>
		''' Initializes the <code>when</code> field if it is not present in the
		''' object input stream. In that case, the field will be initialized by
		''' invoking <seealso cref="java.awt.EventQueue#getMostRecentEventTime()"/>.
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If [when] = 0 Then [when] = java.awt.EventQueue.mostRecentEventTime
		End Sub

		''' <summary>
		''' Get the most recent event time in the {@code EventQueue} which the {@code source}
		''' belongs to.
		''' </summary>
		''' <param name="source"> the source of the event </param>
		''' <exception cref="IllegalArgumentException">  if source is null. </exception>
		''' <returns> most recent event time in the {@code EventQueue} </returns>
		Private Shared Function getMostRecentEventTimeForSource(  source As Object) As Long
			If source Is Nothing Then Throw New IllegalArgumentException("null source")
			Dim appContext As sun.awt.AppContext = sun.awt.SunToolkit.targetToAppContext(source)
			Dim eventQueue_Renamed As java.awt.EventQueue = sun.awt.SunToolkit.getSystemEventQueueImplPP(appContext)
			Return sun.awt.AWTAccessor.eventQueueAccessor.getMostRecentEventTime(eventQueue_Renamed)
		End Function
	End Class

End Namespace
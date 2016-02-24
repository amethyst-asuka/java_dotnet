Imports System
Imports System.Runtime.CompilerServices
Imports javax.accessibility

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' The <code>TextComponent</code> class is the superclass of
	''' any component that allows the editing of some text.
	''' <p>
	''' A text component embodies a string of text.  The
	''' <code>TextComponent</code> class defines a set of methods
	''' that determine whether or not this text is editable. If the
	''' component is editable, it defines another set of methods
	''' that supports a text insertion caret.
	''' <p>
	''' In addition, the class defines methods that are used
	''' to maintain a current <em>selection</em> from the text.
	''' The text selection, a substring of the component's text,
	''' is the target of editing operations. It is also referred
	''' to as the <em>selected text</em>.
	''' 
	''' @author      Sami Shaio
	''' @author      Arthur van Hoff
	''' @since       JDK1.0
	''' </summary>
	Public Class TextComponent
		Inherits Component
		Implements Accessible

		''' <summary>
		''' The value of the text.
		''' A <code>null</code> value is the same as "".
		''' 
		''' @serial </summary>
		''' <seealso cref= #setText(String) </seealso>
		''' <seealso cref= #getText() </seealso>
		Friend text As String

		''' <summary>
		''' A boolean indicating whether or not this
		''' <code>TextComponent</code> is editable.
		''' It will be <code>true</code> if the text component
		''' is editable and <code>false</code> if not.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isEditable() </seealso>
		Friend editable As Boolean = True

		''' <summary>
		''' The selection refers to the selected text, and the
		''' <code>selectionStart</code> is the start position
		''' of the selected text.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getSelectionStart() </seealso>
		''' <seealso cref= #setSelectionStart(int) </seealso>
		Friend selectionStart As Integer

		''' <summary>
		''' The selection refers to the selected text, and the
		''' <code>selectionEnd</code>
		''' is the end position of the selected text.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getSelectionEnd() </seealso>
		''' <seealso cref= #setSelectionEnd(int) </seealso>
		Friend selectionEnd As Integer

		' A flag used to tell whether the background has been set by
		' developer code (as opposed to AWT code).  Used to determine
		' the background color of non-editable TextComponents.
		Friend backgroundSetByClientCode As Boolean = False

		<NonSerialized> _
		Protected Friend textListener As TextListener

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -2214773872412987419L

		''' <summary>
		''' Constructs a new text component initialized with the
		''' specified text. Sets the value of the cursor to
		''' <code>Cursor.TEXT_CURSOR</code>. </summary>
		''' <param name="text">       the text to be displayed; if
		'''             <code>text</code> is <code>null</code>, the empty
		'''             string <code>""</code> will be displayed </param>
		''' <exception cref="HeadlessException"> if
		'''             <code>GraphicsEnvironment.isHeadless</code>
		'''             returns true </exception>
		''' <seealso cref=        java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref=        java.awt.Cursor </seealso>
		Friend Sub New(ByVal text As String)
			GraphicsEnvironment.checkHeadless()
			Me.text = If(text IsNot Nothing, text, "")
			cursor = Cursor.getPredefinedCursor(Cursor.TEXT_CURSOR)
		End Sub

		Private Sub enableInputMethodsIfNecessary()
			If checkForEnableIM Then
				checkForEnableIM = False
				Try
					Dim toolkit_Renamed As Toolkit = Toolkit.defaultToolkit
					Dim shouldEnable As Boolean = False
					If TypeOf toolkit_Renamed Is sun.awt.InputMethodSupport Then shouldEnable = CType(toolkit_Renamed, sun.awt.InputMethodSupport).enableInputMethodsForTextComponent()
					enableInputMethods(shouldEnable)
				Catch e As Exception
					' if something bad happens, just don't enable input methods
				End Try
			End If
		End Sub

		''' <summary>
		''' Enables or disables input method support for this text component. If input
		''' method support is enabled and the text component also processes key events,
		''' incoming events are offered to the current input method and will only be
		''' processed by the component or dispatched to its listeners if the input method
		''' does not consume them. Whether and how input method support for this text
		''' component is enabled or disabled by default is implementation dependent.
		''' </summary>
		''' <param name="enable"> true to enable, false to disable </param>
		''' <seealso cref= #processKeyEvent
		''' @since 1.2 </seealso>
		Public Overrides Sub enableInputMethods(ByVal enable As Boolean)
			checkForEnableIM = False
			MyBase.enableInputMethods(enable)
		End Sub

		Friend Overrides Function areInputMethodsEnabled() As Boolean
			' moved from the constructor above to here and addNotify below,
			' this call will initialize the toolkit if not already initialized.
			If checkForEnableIM Then enableInputMethodsIfNecessary()

			' TextComponent handles key events without touching the eventMask or
			' having a key listener, so just check whether the flag is set
			Return (eventMask And AWTEvent.INPUT_METHODS_ENABLED_MASK) <> 0
		End Function

		Public Property Overrides inputMethodRequests As java.awt.im.InputMethodRequests
			Get
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then
					Return peer_Renamed.inputMethodRequests
				Else
					Return Nothing
				End If
			End Get
		End Property



		''' <summary>
		''' Makes this Component displayable by connecting it to a
		''' native screen resource.
		''' This method is called internally by the toolkit and should
		''' not be called directly by programs. </summary>
		''' <seealso cref=       java.awt.TextComponent#removeNotify </seealso>
		Public Overrides Sub addNotify()
			MyBase.addNotify()
			enableInputMethodsIfNecessary()
		End Sub

		''' <summary>
		''' Removes the <code>TextComponent</code>'s peer.
		''' The peer allows us to modify the appearance of the
		''' <code>TextComponent</code> without changing its
		''' functionality.
		''' </summary>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then
					text = peer_Renamed.text
					selectionStart = peer_Renamed.selectionStart
					selectionEnd = peer_Renamed.selectionEnd
				End If
				MyBase.removeNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the text that is presented by this
		''' text component to be the specified text. </summary>
		''' <param name="t">   the new text;
		'''                  if this parameter is <code>null</code> then
		'''                  the text is set to the empty string "" </param>
		''' <seealso cref=         java.awt.TextComponent#getText </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property text As String
			Set(ByVal t As String)
				Dim skipTextEvent As Boolean = (text Is Nothing OrElse text.empty) AndAlso (t Is Nothing OrElse t.empty)
				text = If(t IsNot Nothing, t, "")
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				' Please note that we do not want to post an event
				' if TextArea.setText() or TextField.setText() replaces an empty text
				' by an empty text, that is, if component's text remains unchanged.
				If peer_Renamed IsNot Nothing AndAlso (Not skipTextEvent) Then peer_Renamed.text = text
			End Set
			Get
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then text = peer_Renamed.text
				Return text
			End Get
		End Property


		''' <summary>
		''' Returns the selected text from the text that is
		''' presented by this text component. </summary>
		''' <returns>      the selected text of this text component </returns>
		''' <seealso cref=         java.awt.TextComponent#select </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedText As String
			Get
				Return text.Substring(selectionStart, selectionEnd - (selectionStart))
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not this text component is editable. </summary>
		''' <returns>     <code>true</code> if this text component is
		'''                  editable; <code>false</code> otherwise. </returns>
		''' <seealso cref=        java.awt.TextComponent#setEditable
		''' @since      JDK1.0 </seealso>
		Public Overridable Property editable As Boolean
			Get
				Return editable
			End Get
			Set(ByVal b As Boolean)
				If editable = b Then Return
    
				editable = b
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.editable = b
			End Set
		End Property


		''' <summary>
		''' Gets the background color of this text component.
		''' 
		''' By default, non-editable text components have a background color
		''' of SystemColor.control.  This default can be overridden by
		''' calling setBackground.
		''' </summary>
		''' <returns> This text component's background color.
		'''         If this text component does not have a background color,
		'''         the background color of its parent is returned. </returns>
		''' <seealso cref= #setBackground(Color)
		''' @since JDK1.0 </seealso>
		Public Property Overrides background As Color
			Get
				If (Not editable) AndAlso (Not backgroundSetByClientCode) Then Return SystemColor.control
    
				Return MyBase.background
			End Get
			Set(ByVal c As Color)
				backgroundSetByClientCode = True
				MyBase.background = c
			End Set
		End Property


		''' <summary>
		''' Gets the start position of the selected text in
		''' this text component. </summary>
		''' <returns>      the start position of the selected text </returns>
		''' <seealso cref=         java.awt.TextComponent#setSelectionStart </seealso>
		''' <seealso cref=         java.awt.TextComponent#getSelectionEnd </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectionStart As Integer
			Get
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then selectionStart = peer_Renamed.selectionStart
				Return selectionStart
			End Get
			Set(ByVal selectionStart As Integer)
		'         Route through select method to enforce consistent policy
		'         * between selectionStart and selectionEnd.
		'         
				[select](selectionStart, selectionEnd)
			End Set
		End Property


		''' <summary>
		''' Gets the end position of the selected text in
		''' this text component. </summary>
		''' <returns>      the end position of the selected text </returns>
		''' <seealso cref=         java.awt.TextComponent#setSelectionEnd </seealso>
		''' <seealso cref=         java.awt.TextComponent#getSelectionStart </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectionEnd As Integer
			Get
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then selectionEnd = peer_Renamed.selectionEnd
				Return selectionEnd
			End Get
			Set(ByVal selectionEnd As Integer)
		'         Route through select method to enforce consistent policy
		'         * between selectionStart and selectionEnd.
		'         
				[select](selectionStart, selectionEnd)
			End Set
		End Property


		''' <summary>
		''' Selects the text between the specified start and end positions.
		''' <p>
		''' This method sets the start and end positions of the
		''' selected text, enforcing the restriction that the start position
		''' must be greater than or equal to zero.  The end position must be
		''' greater than or equal to the start position, and less than or
		''' equal to the length of the text component's text.  The
		''' character positions are indexed starting with zero.
		''' The length of the selection is
		''' <code>endPosition</code> - <code>startPosition</code>, so the
		''' character at <code>endPosition</code> is not selected.
		''' If the start and end positions of the selected text are equal,
		''' all text is deselected.
		''' <p>
		''' If the caller supplies values that are inconsistent or out of
		''' bounds, the method enforces these constraints silently, and
		''' without failure. Specifically, if the start position or end
		''' position is greater than the length of the text, it is reset to
		''' equal the text length. If the start position is less than zero,
		''' it is reset to zero, and if the end position is less than the
		''' start position, it is reset to the start position.
		''' </summary>
		''' <param name="selectionStart"> the zero-based index of the first
		'''                   character (<code>char</code> value) to be selected </param>
		''' <param name="selectionEnd"> the zero-based end position of the
		'''                   text to be selected; the character (<code>char</code> value) at
		'''                   <code>selectionEnd</code> is not selected </param>
		''' <seealso cref=          java.awt.TextComponent#setSelectionStart </seealso>
		''' <seealso cref=          java.awt.TextComponent#setSelectionEnd </seealso>
		''' <seealso cref=          java.awt.TextComponent#selectAll </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub [select](ByVal selectionStart As Integer, ByVal selectionEnd As Integer)
			Dim text_Renamed As String = text
			If selectionStart < 0 Then selectionStart = 0
			If selectionStart > text_Renamed.length() Then selectionStart = text_Renamed.length()
			If selectionEnd > text_Renamed.length() Then selectionEnd = text_Renamed.length()
			If selectionEnd < selectionStart Then selectionEnd = selectionStart

			Me.selectionStart = selectionStart
			Me.selectionEnd = selectionEnd

			Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.select(selectionStart, selectionEnd)
		End Sub

		''' <summary>
		''' Selects all the text in this text component. </summary>
		''' <seealso cref=        java.awt.TextComponent#select </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub selectAll()
			Me.selectionStart = 0
			Me.selectionEnd = text.length()

			Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.select(selectionStart, selectionEnd)
		End Sub

		''' <summary>
		''' Sets the position of the text insertion caret.
		''' The caret position is constrained to be between 0
		''' and the last character of the text, inclusive.
		''' If the passed-in value is greater than this range,
		''' the value is set to the last character (or 0 if
		''' the <code>TextComponent</code> contains no text)
		''' and no error is returned.  If the passed-in value is
		''' less than 0, an <code>IllegalArgumentException</code>
		''' is thrown.
		''' </summary>
		''' <param name="position"> the position of the text insertion caret </param>
		''' <exception cref="IllegalArgumentException"> if <code>position</code>
		'''               is less than zero
		''' @since        JDK1.1 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property caretPosition As Integer
			Set(ByVal position As Integer)
				If position < 0 Then Throw New IllegalArgumentException("position less than zero.")
    
				Dim maxposition As Integer = text.length()
				If position > maxposition Then position = maxposition
    
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				If peer_Renamed IsNot Nothing Then
					peer_Renamed.caretPosition = position
				Else
					[select](position, position)
				End If
			End Set
			Get
				Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
				Dim position As Integer = 0
    
				If peer_Renamed IsNot Nothing Then
					position = peer_Renamed.caretPosition
				Else
					position = selectionStart
				End If
				Dim maxposition As Integer = text.length()
				If position > maxposition Then position = maxposition
				Return position
			End Get
		End Property


		''' <summary>
		''' Adds the specified text event listener to receive text events
		''' from this text component.
		''' If <code>l</code> is <code>null</code>, no exception is
		''' thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the text event listener </param>
		''' <seealso cref=             #removeTextListener </seealso>
		''' <seealso cref=             #getTextListeners </seealso>
		''' <seealso cref=             java.awt.event.TextListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addTextListener(ByVal l As TextListener)
			If l Is Nothing Then Return
			textListener = AWTEventMulticaster.add(textListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified text event listener so that it no longer
		''' receives text events from this text component
		''' If <code>l</code> is <code>null</code>, no exception is
		''' thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">     the text listener </param>
		''' <seealso cref=             #addTextListener </seealso>
		''' <seealso cref=             #getTextListeners </seealso>
		''' <seealso cref=             java.awt.event.TextListener
		''' @since           JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeTextListener(ByVal l As TextListener)
			If l Is Nothing Then Return
			textListener = AWTEventMulticaster.remove(textListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the text listeners
		''' registered on this text component.
		''' </summary>
		''' <returns> all of this text component's <code>TextListener</code>s
		'''         or an empty array if no text
		'''         listeners are currently registered
		''' 
		''' </returns>
		''' <seealso cref= #addTextListener </seealso>
		''' <seealso cref= #removeTextListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property textListeners As TextListener()
			Get
				Return getListeners(GetType(TextListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>TextComponent</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>TextComponent</code> <code>t</code>
		''' for its text listeners with the following code:
		''' 
		''' <pre>TextListener[] tls = (TextListener[])(t.getListeners(TextListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this text component,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getTextListeners
		''' @since 1.3 </seealso>
		Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Class) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(TextListener) Then
				l = textListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			If e.id = TextEvent.TEXT_VALUE_CHANGED Then
				If (eventMask And AWTEvent.TEXT_EVENT_MASK) <> 0 OrElse textListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this text component. If the event is a
		''' <code>TextEvent</code>, it invokes the <code>processTextEvent</code>
		''' method else it invokes its superclass's <code>processEvent</code>.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is TextEvent Then
				processTextEvent(CType(e, TextEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes text events occurring on this text component by
		''' dispatching them to any registered <code>TextListener</code> objects.
		''' <p>
		''' NOTE: This method will not be called unless text events
		''' are enabled for this component. This happens when one of the
		''' following occurs:
		''' <ul>
		''' <li>A <code>TextListener</code> object is registered
		''' via <code>addTextListener</code>
		''' <li>Text events are enabled via <code>enableEvents</code>
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the text event </param>
		''' <seealso cref= Component#enableEvents </seealso>
		Protected Friend Overridable Sub processTextEvent(ByVal e As TextEvent)
			Dim listener As TextListener = textListener
			If listener IsNot Nothing Then
				Dim id As Integer = e.iD
				Select Case id
				Case TextEvent.TEXT_VALUE_CHANGED
					listener.textValueChanged(e)
				End Select
			End If
		End Sub

		''' <summary>
		''' Returns a string representing the state of this
		''' <code>TextComponent</code>. This
		''' method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>      the parameter string of this text component </returns>
		Protected Friend Overrides Function paramString() As String
			Dim str As String = MyBase.paramString() & ",text=" & text
			If editable Then str &= ",editable"
			Return str & ",selection=" & selectionStart & "-" & selectionEnd
		End Function

		''' <summary>
		''' Assigns a valid value to the canAccessClipboard instance variable.
		''' </summary>
		Private Function canAccessClipboard() As Boolean
			Dim sm As SecurityManager = System.securityManager
			If sm Is Nothing Then Return True
			Try
				sm.checkPermission(sun.security.util.SecurityConstants.AWT.ACCESS_CLIPBOARD_PERMISSION)
				Return True
			Catch e As SecurityException
			End Try
			Return False
		End Function

	'    
	'     * Serialization support.
	'     
		''' <summary>
		''' The textComponent SerializedDataVersion.
		''' 
		''' @serial
		''' </summary>
		Private textComponentSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable TextListener(s) as optional data.
		''' The non-serializable TextListener(s) are detected and
		''' no attempt is made to serialize them.
		''' 
		''' @serialData Null terminated sequence of zero or more pairs.
		'''             A pair consists of a String and Object.
		'''             The String indicates the type of object and
		'''             is one of the following :
		'''             textListenerK indicating and TextListener object.
		''' </summary>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#textListenerK </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Serialization support.  Since the value of the fields
			' selectionStart, selectionEnd, and text aren't necessarily
			' up to date, we sync them up with the peer before serializing.
			Dim peer_Renamed As java.awt.peer.TextComponentPeer = CType(Me.peer, java.awt.peer.TextComponentPeer)
			If peer_Renamed IsNot Nothing Then
				text = peer_Renamed.text
				selectionStart = peer_Renamed.selectionStart
				selectionEnd = peer_Renamed.selectionEnd
			End If

			s.defaultWriteObject()

			AWTEventMulticaster.save(s, textListenerK, textListener)
			s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Read the ObjectInputStream, and if it isn't null,
		''' add a listener to receive text events fired by the
		''' TextComponent.  Unrecognized keys or values will be
		''' ignored.
		''' </summary>
		''' <exception cref="HeadlessException"> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns
		''' <code>true</code> </exception>
		''' <seealso cref= #removeTextListener </seealso>
		''' <seealso cref= #addTextListener </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			GraphicsEnvironment.checkHeadless()
			s.defaultReadObject()

			' Make sure the state we just read in for text,
			' selectionStart and selectionEnd has legal values
			Me.text = If(text IsNot Nothing, text, "")
			[select](selectionStart, selectionEnd)

			Dim keyOrNull As Object
			keyOrNull = s.readObject()
			Do While Nothing IsNot keyOrNull
				Dim key As String = CStr(keyOrNull).intern()

				If textListenerK = key Then
					addTextListener(CType(s.readObject(), TextListener))
				Else
					' skip value for unrecognized key
					s.readObject()
				End If
				keyOrNull = s.readObject()
			Loop
			enableInputMethodsIfNecessary()
		End Sub


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this TextComponent.
		''' For text components, the AccessibleContext takes the form of an
		''' AccessibleAWTTextComponent.
		''' A new AccessibleAWTTextComponent instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTTextComponent that serves as the
		'''         AccessibleContext of this TextComponent
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTTextComponent(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>TextComponent</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to text component user-interface
		''' elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTTextComponent
			Inherits AccessibleAWTComponent
			Implements AccessibleText, TextListener

			Private ReadOnly outerInstance As TextComponent

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 3631432373506317811L

			''' <summary>
			''' Constructs an AccessibleAWTTextComponent.  Adds a listener to track
			''' caret change.
			''' </summary>
			Public Sub New(ByVal outerInstance As TextComponent)
					Me.outerInstance = outerInstance
				outerInstance.addTextListener(Me)
			End Sub

			''' <summary>
			''' TextListener notification of a text value change.
			''' </summary>
			Public Overridable Sub textValueChanged(ByVal textEvent_Renamed As TextEvent) Implements TextListener.textValueChanged
				Dim cpos As Integer? = Convert.ToInt32(outerInstance.caretPosition)
				outerInstance.firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, cpos)
			End Sub

			''' <summary>
			''' Gets the state set of the TextComponent.
			''' The AccessibleStateSet of an object is composed of a set of
			''' unique AccessibleStates.  A change in the AccessibleStateSet
			''' of an object will cause a PropertyChangeEvent to be fired
			''' for the AccessibleContext.ACCESSIBLE_STATE_PROPERTY property.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the
			''' current state set of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState </seealso>
			''' <seealso cref= #addPropertyChangeListener </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.editable Then states.add(AccessibleState.EDITABLE)
					Return states
				End Get
			End Property


			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object (AccessibleRole.TEXT) </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TEXT
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleText associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleText interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleText As AccessibleText
				Get
					Return Me
				End Get
			End Property


			' --- interface AccessibleText methods ------------------------

			''' <summary>
			''' Many of these methods are just convenience methods; they
			''' just call the equivalent on the parent
			''' </summary>

			''' <summary>
			''' Given a point in local coordinates, return the zero-based index
			''' of the character under that Point.  If the point is invalid,
			''' this method returns -1.
			''' </summary>
			''' <param name="p"> the Point in local coordinates </param>
			''' <returns> the zero-based index of the character under Point p. </returns>
			Public Overridable Function getIndexAtPoint(ByVal p As Point) As Integer
				Return -1
			End Function

			''' <summary>
			''' Determines the bounding box of the character at the given
			''' index into the string.  The bounds are returned in local
			''' coordinates.  If the index is invalid a null rectangle
			''' is returned.
			''' </summary>
			''' <param name="i"> the index into the String &gt;= 0 </param>
			''' <returns> the screen coordinates of the character's bounding box </returns>
			Public Overridable Function getCharacterBounds(ByVal i As Integer) As Rectangle
				Return Nothing
			End Function

			''' <summary>
			''' Returns the number of characters (valid indicies)
			''' </summary>
			''' <returns> the number of characters &gt;= 0 </returns>
			Public Overridable Property charCount As Integer
				Get
					Return outerInstance.text.length()
				End Get
			End Property

			''' <summary>
			''' Returns the zero-based offset of the caret.
			''' 
			''' Note: The character to the right of the caret will have the
			''' same index value as the offset (the caret is between
			''' two characters).
			''' </summary>
			''' <returns> the zero-based offset of the caret. </returns>
			Public Overridable Property caretPosition As Integer
				Get
					Return outerInstance.caretPosition
				End Get
			End Property

			''' <summary>
			''' Returns the AttributeSet for a given character (at a given index).
			''' </summary>
			''' <param name="i"> the zero-based index into the text </param>
			''' <returns> the AttributeSet of the character </returns>
			Public Overridable Function getCharacterAttribute(ByVal i As Integer) As javax.swing.text.AttributeSet
				Return Nothing ' No attributes in TextComponent
			End Function

			''' <summary>
			''' Returns the start offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' Return 0 if the text is empty, or the caret position
			''' if no selection.
			''' </summary>
			''' <returns> the index into the text of the start of the selection &gt;= 0 </returns>
			Public Overridable Property selectionStart As Integer
				Get
					Return outerInstance.selectionStart
				End Get
			End Property

			''' <summary>
			''' Returns the end offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' Return 0 if the text is empty, or the caret position
			''' if no selection.
			''' </summary>
			''' <returns> the index into the text of the end of the selection &gt;= 0 </returns>
			Public Overridable Property selectionEnd As Integer
				Get
					Return outerInstance.selectionEnd
				End Get
			End Property

			''' <summary>
			''' Returns the portion of the text that is selected.
			''' </summary>
			''' <returns> the text, null if no selection </returns>
			Public Overridable Property selectedText As String
				Get
					Dim selText As String = outerInstance.selectedText
					' Fix for 4256662
					If selText Is Nothing OrElse selText.Equals("") Then Return Nothing
					Return selText
				End Get
			End Property

			''' <summary>
			''' Returns the String at a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			''' or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence,
			'''   null for an invalid index or part </returns>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String
				If index < 0 OrElse index >= outerInstance.text.length() Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					Return outerInstance.text.Substring(index, 1)
				Case AccessibleText.WORD
						Dim s As String = outerInstance.text
						Dim words As java.text.BreakIterator = java.text.BreakIterator.wordInstance
						words.text = s
						Dim [end] As Integer = words.following(index)
						Return s.Substring(words.previous(), [end] - (words.previous()))
				Case AccessibleText.SENTENCE
						Dim s As String = outerInstance.text
						Dim sentence As java.text.BreakIterator = java.text.BreakIterator.sentenceInstance
						sentence.text = s
						Dim [end] As Integer = sentence.following(index)
						Return s.Substring(sentence.previous(), [end] - (sentence.previous()))
				Case Else
					Return Nothing
				End Select
			End Function

			Private Const [NEXT] As Boolean = True
			Private Const PREVIOUS As Boolean = False

			''' <summary>
			''' Needed to unify forward and backward searching.
			''' The method assumes that s is the text assigned to words.
			''' </summary>
			Private Function findWordLimit(ByVal index As Integer, ByVal words As java.text.BreakIterator, ByVal direction As Boolean, ByVal s As String) As Integer
				' Fix for 4256660 and 4256661.
				' Words iterator is different from character and sentence iterators
				' in that end of one word is not necessarily start of another word.
				' Please see java.text.BreakIterator JavaDoc. The code below is
				' based on nextWordStartAfter example from BreakIterator.java.
				Dim last As Integer = If(direction = [NEXT], words.following(index), words.preceding(index))
				Dim current As Integer = If(direction = [NEXT], words.next(), words.previous())
				Do While current <> java.text.BreakIterator.DONE
					For p As Integer = Math.Min(last, current) To Math.Max(last, current) - 1
						If Char.IsLetter(s.Chars(p)) Then Return last
					Next p
					last = current
					current = If(direction = [NEXT], words.next(), words.previous())
				Loop
				Return java.text.BreakIterator.DONE
			End Function

			''' <summary>
			''' Returns the String after a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			''' or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence, null for an invalid
			'''  index or part </returns>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String
				If index < 0 OrElse index >= outerInstance.text.length() Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					If index+1 >= outerInstance.text.length() Then Return Nothing
					Return outerInstance.text.Substring(index+1, index+2 - (index+1))
				Case AccessibleText.WORD
						Dim s As String = outerInstance.text
						Dim words As java.text.BreakIterator = java.text.BreakIterator.wordInstance
						words.text = s
						Dim start As Integer = findWordLimit(index, words, [NEXT], s)
						If start = java.text.BreakIterator.DONE OrElse start >= s.length() Then Return Nothing
						Dim [end] As Integer = words.following(start)
						If [end] = java.text.BreakIterator.DONE OrElse [end] >= s.length() Then Return Nothing
						Return s.Substring(start, [end] - start)
				Case AccessibleText.SENTENCE
						Dim s As String = outerInstance.text
						Dim sentence As java.text.BreakIterator = java.text.BreakIterator.sentenceInstance
						sentence.text = s
						Dim start As Integer = sentence.following(index)
						If start = java.text.BreakIterator.DONE OrElse start >= s.length() Then Return Nothing
						Dim [end] As Integer = sentence.following(start)
						If [end] = java.text.BreakIterator.DONE OrElse [end] >= s.length() Then Return Nothing
						Return s.Substring(start, [end] - start)
				Case Else
					Return Nothing
				End Select
			End Function


			''' <summary>
			''' Returns the String before a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			'''   or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence, null for an invalid index
			'''  or part </returns>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String
				If index < 0 OrElse index >outerInstance.text.length()-1 Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					If index = 0 Then Return Nothing
					Return outerInstance.text.Substring(index-1, index - (index-1))
				Case AccessibleText.WORD
						Dim s As String = outerInstance.text
						Dim words As java.text.BreakIterator = java.text.BreakIterator.wordInstance
						words.text = s
						Dim [end] As Integer = findWordLimit(index, words, PREVIOUS, s)
						If [end] = java.text.BreakIterator.DONE Then Return Nothing
						Dim start As Integer = words.preceding([end])
						If start = java.text.BreakIterator.DONE Then Return Nothing
						Return s.Substring(start, [end] - start)
				Case AccessibleText.SENTENCE
						Dim s As String = outerInstance.text
						Dim sentence As java.text.BreakIterator = java.text.BreakIterator.sentenceInstance
						sentence.text = s
						Dim [end] As Integer = sentence.following(index)
						[end] = sentence.previous()
						Dim start As Integer = sentence.previous()
						If start = java.text.BreakIterator.DONE Then Return Nothing
						Return s.Substring(start, [end] - start)
				Case Else
					Return Nothing
				End Select
			End Function
		End Class ' end of AccessibleAWTTextComponent

		Private checkForEnableIM As Boolean = True
	End Class

End Namespace
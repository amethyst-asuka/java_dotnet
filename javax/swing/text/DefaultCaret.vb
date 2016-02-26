Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Text
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf

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
Namespace javax.swing.text


	''' <summary>
	''' A default implementation of Caret.  The caret is rendered as
	''' a vertical line in the color specified by the CaretColor property
	''' of the associated JTextComponent.  It can blink at the rate specified
	''' by the BlinkRate property.
	''' <p>
	''' This implementation expects two sources of asynchronous notification.
	''' The timer thread fires asynchronously, and causes the caret to simply
	''' repaint the most recent bounding box.  The caret also tracks change
	''' as the document is modified.  Typically this will happen on the
	''' event dispatch thread as a result of some mouse or keyboard event.
	''' The caret behavior on both synchronous and asynchronous documents updates
	''' is controlled by <code>UpdatePolicy</code> property. The repaint of the
	''' new caret location will occur on the event thread in any case, as calls to
	''' <code>modelToView</code> are only safe on the event thread.
	''' <p>
	''' The caret acts as a mouse and focus listener on the text component
	''' it has been installed in, and defines the caret semantics based upon
	''' those events.  The listener methods can be reimplemented to change the
	''' semantics.
	''' By default, the first mouse button will be used to set focus and caret
	''' position.  Dragging the mouse pointer with the first mouse button will
	''' sweep out a selection that is contiguous in the model.  If the associated
	''' text component is editable, the caret will become visible when focus
	''' is gained, and invisible when focus is lost.
	''' <p>
	''' The Highlighter bound to the associated text component is used to
	''' render the selection by default.
	''' Selection appearance can be customized by supplying a
	''' painter to use for the highlights.  By default a painter is used that
	''' will render a solid color as specified in the associated text component
	''' in the <code>SelectionColor</code> property.  This can easily be changed
	''' by reimplementing the
	''' <seealso cref="#getSelectionPainter getSelectionPainter"/>
	''' method.
	''' <p>
	''' A customized caret appearance can be achieved by reimplementing
	''' the paint method.  If the paint method is changed, the damage method
	''' should also be reimplemented to cause a repaint for the area needed
	''' to render the caret.  The caret extends the Rectangle class which
	''' is used to hold the bounding box for where the caret was last rendered.
	''' This enables the caret to repaint in a thread-safe manner when the
	''' caret moves without making a call to modelToView which is unstable
	''' between model updates and view repair (i.e. the order of delivery
	''' to DocumentListeners is not guaranteed).
	''' <p>
	''' The magic caret position is set to null when the caret position changes.
	''' A timer is used to determine the new location (after the caret change).
	''' When the timer fires, if the magic caret position is still null it is
	''' reset to the current caret position. Any actions that change
	''' the caret position and want the magic caret position to remain the
	''' same, must remember the magic caret position, change the cursor, and
	''' then set the magic caret position to its original value. This has the
	''' benefit that only actions that want the magic caret position to persist
	''' (such as open/down) need to know about it.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     Caret </seealso>
	Public Class DefaultCaret
		Inherits Rectangle
		Implements Caret, FocusListener, MouseListener, MouseMotionListener

		''' <summary>
		''' Indicates that the caret position is to be updated only when
		''' document changes are performed on the Event Dispatching Thread. </summary>
		''' <seealso cref= #setUpdatePolicy </seealso>
		''' <seealso cref= #getUpdatePolicy
		''' @since 1.5 </seealso>
		Public Const UPDATE_WHEN_ON_EDT As Integer = 0

		''' <summary>
		''' Indicates that the caret should remain at the same
		''' absolute position in the document regardless of any document
		''' updates, except when the document length becomes less than
		''' the current caret position due to removal. In that case the caret
		''' position is adjusted to the end of the document.
		''' </summary>
		''' <seealso cref= #setUpdatePolicy </seealso>
		''' <seealso cref= #getUpdatePolicy
		''' @since 1.5 </seealso>
		Public Const NEVER_UPDATE As Integer = 1

		''' <summary>
		''' Indicates that the caret position is to be <b>always</b>
		''' updated accordingly to the document changes regardless whether
		''' the document updates are performed on the Event Dispatching Thread
		''' or not.
		''' </summary>
		''' <seealso cref= #setUpdatePolicy </seealso>
		''' <seealso cref= #getUpdatePolicy
		''' @since 1.5 </seealso>
		Public Const ALWAYS_UPDATE As Integer = 2

		''' <summary>
		''' Constructs a default caret.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Sets the caret movement policy on the document updates. Normally
		''' the caret updates its absolute position within the document on
		''' insertions occurred before or at the caret position and
		''' on removals before the caret position. 'Absolute position'
		''' means here the position relative to the start of the document.
		''' For example if
		''' a character is typed within editable text component it is inserted
		''' at the caret position and the caret moves to the next absolute
		''' position within the document due to insertion and if
		''' <code>BACKSPACE</code> is typed then caret decreases its absolute
		''' position due to removal of a character before it. Sometimes
		''' it may be useful to turn off the caret position updates so that
		''' the caret stays at the same absolute position within the
		''' document position regardless of any document updates.
		''' <p>
		''' The following update policies are allowed:
		''' <ul>
		'''   <li><code>NEVER_UPDATE</code>: the caret stays at the same
		'''       absolute position in the document regardless of any document
		'''       updates, except when document length becomes less than
		'''       the current caret position due to removal. In that case caret
		'''       position is adjusted to the end of the document.
		'''       The caret doesn't try to keep itself visible by scrolling
		'''       the associated view when using this policy. </li>
		'''   <li><code>ALWAYS_UPDATE</code>: the caret always tracks document
		'''       changes. For regular changes it increases its position
		'''       if an insertion occurs before or at its current position,
		'''       and decreases position if a removal occurs before
		'''       its current position. For undo/redo updates it is always
		'''       moved to the position where update occurred. The caret
		'''       also tries to keep itself visible by calling
		'''       <code>adjustVisibility</code> method.</li>
		'''   <li><code>UPDATE_WHEN_ON_EDT</code>: acts like <code>ALWAYS_UPDATE</code>
		'''       if the document updates are performed on the Event Dispatching Thread
		'''       and like <code>NEVER_UPDATE</code> if updates are performed on
		'''       other thread. </li>
		''' </ul> <p>
		''' The default property value is <code>UPDATE_WHEN_ON_EDT</code>.
		''' </summary>
		''' <param name="policy"> one of the following values : <code>UPDATE_WHEN_ON_EDT</code>,
		''' <code>NEVER_UPDATE</code>, <code>ALWAYS_UPDATE</code> </param>
		''' <exception cref="IllegalArgumentException"> if invalid value is passed
		''' </exception>
		''' <seealso cref= #getUpdatePolicy </seealso>
		''' <seealso cref= #adjustVisibility </seealso>
		''' <seealso cref= #UPDATE_WHEN_ON_EDT </seealso>
		''' <seealso cref= #NEVER_UPDATE </seealso>
		''' <seealso cref= #ALWAYS_UPDATE
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Property updatePolicy As Integer
			Set(ByVal policy As Integer)
				updatePolicy = policy
			End Set
			Get
				Return updatePolicy
			End Get
		End Property


		''' <summary>
		''' Gets the text editor component that this caret is
		''' is bound to.
		''' </summary>
		''' <returns> the component </returns>
		Protected Friend Property component As JTextComponent
			Get
				Return component
			End Get
		End Property

		''' <summary>
		''' Cause the caret to be painted.  The repaint
		''' area is the bounding box of the caret (i.e.
		''' the caret rectangle or <em>this</em>).
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Sub repaint()
			If component IsNot Nothing Then component.repaint(x, y, width, height)
		End Sub

		''' <summary>
		''' Damages the area surrounding the caret to cause
		''' it to be repainted in a new location.  If paint()
		''' is reimplemented, this method should also be
		''' reimplemented.  This method should update the
		''' caret bounds (x, y, width, and height).
		''' </summary>
		''' <param name="r">  the current location of the caret </param>
		''' <seealso cref= #paint </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub damage(ByVal r As Rectangle)
			If r IsNot Nothing Then
				Dim damageWidth As Integer = getCaretWidth(r.height)
				x = r.x - 4 - (damageWidth >> 1)
				y = r.y
				width = 9 + damageWidth
				height = r.height
				repaint()
			End If
		End Sub

		''' <summary>
		''' Scrolls the associated view (if necessary) to make
		''' the caret visible.  Since how this should be done
		''' is somewhat of a policy, this method can be
		''' reimplemented to change the behavior.  By default
		''' the scrollRectToVisible method is called on the
		''' associated component.
		''' </summary>
		''' <param name="nloc"> the new position to scroll to </param>
		Protected Friend Overridable Sub adjustVisibility(ByVal nloc As Rectangle)
			If component Is Nothing Then Return
			If SwingUtilities.eventDispatchThread Then
					component.scrollRectToVisible(nloc)
			Else
				SwingUtilities.invokeLater(New SafeScroller(Me, nloc))
			End If
		End Sub

		''' <summary>
		''' Gets the painter for the Highlighter.
		''' </summary>
		''' <returns> the painter </returns>
		Protected Friend Overridable Property selectionPainter As Highlighter.HighlightPainter
			Get
				Return DefaultHighlighter.DefaultPainter
			End Get
		End Property

		''' <summary>
		''' Tries to set the position of the caret from
		''' the coordinates of a mouse event, using viewToModel().
		''' </summary>
		''' <param name="e"> the mouse event </param>
		Protected Friend Overridable Sub positionCaret(ByVal e As MouseEvent)
			Dim pt As New Point(e.x, e.y)
			Dim biasRet As Position.Bias() = New Position.Bias(0){}
			Dim pos As Integer = component.uI.viewToModel(component, pt, biasRet)
			If biasRet(0) Is Nothing Then biasRet(0) = Position.Bias.Forward
			If pos >= 0 Then dotDot(pos, biasRet(0))
		End Sub

		''' <summary>
		''' Tries to move the position of the caret from
		''' the coordinates of a mouse event, using viewToModel().
		''' This will cause a selection if the dot and mark
		''' are different.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		Protected Friend Overridable Sub moveCaret(ByVal e As MouseEvent)
			Dim pt As New Point(e.x, e.y)
			Dim biasRet As Position.Bias() = New Position.Bias(0){}
			Dim pos As Integer = component.uI.viewToModel(component, pt, biasRet)
			If biasRet(0) Is Nothing Then biasRet(0) = Position.Bias.Forward
			If pos >= 0 Then moveDot(pos, biasRet(0))
		End Sub

		' --- FocusListener methods --------------------------

		''' <summary>
		''' Called when the component containing the caret gains
		''' focus.  This is implemented to set the caret to visible
		''' if the component is editable.
		''' </summary>
		''' <param name="e"> the focus event </param>
		''' <seealso cref= FocusListener#focusGained </seealso>
		Public Overridable Sub focusGained(ByVal e As FocusEvent)
			If component.enabled Then
				If component.editable Then visible = True
				selectionVisible = True
			End If
		End Sub

		''' <summary>
		''' Called when the component containing the caret loses
		''' focus.  This is implemented to set the caret to visibility
		''' to false.
		''' </summary>
		''' <param name="e"> the focus event </param>
		''' <seealso cref= FocusListener#focusLost </seealso>
		Public Overridable Sub focusLost(ByVal e As FocusEvent)
			visible = False
			selectionVisible = ownsSelection OrElse e.temporary
		End Sub


		''' <summary>
		''' Selects word based on the MouseEvent
		''' </summary>
		Private Sub selectWord(ByVal e As MouseEvent)
			If selectedWordEvent IsNot Nothing AndAlso selectedWordEvent.x = e.x AndAlso selectedWordEvent.y = e.y Then Return
						Dim a As Action = Nothing
						Dim map As ActionMap = component.actionMap
						If map IsNot Nothing Then a = map.get(DefaultEditorKit.selectWordAction)
						If a Is Nothing Then
							If ___selectWord Is Nothing Then ___selectWord = New DefaultEditorKit.SelectWordAction
							a = ___selectWord
						End If
						a.actionPerformed(New java.awt.event.ActionEvent(component, java.awt.event.ActionEvent.ACTION_PERFORMED, Nothing, e.when, e.modifiers))
			selectedWordEvent = e
		End Sub

		' --- MouseListener methods -----------------------------------

		''' <summary>
		''' Called when the mouse is clicked.  If the click was generated
		''' from button1, a double click selects a word,
		''' and a triple click the current line.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseListener#mouseClicked </seealso>
		Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			If component Is Nothing Then Return

			Dim nclicks As Integer = sun.swing.SwingUtilities2.getAdjustedClickCount(component, e)

			If Not e.consumed Then
				If SwingUtilities.isLeftMouseButton(e) Then
					' mouse 1 behavior
					If nclicks = 1 Then
						selectedWordEvent = Nothing
					ElseIf nclicks = 2 AndAlso sun.swing.SwingUtilities2.canEventAccessSystemClipboard(e) Then
						selectWord(e)
						selectedWordEvent = Nothing
					ElseIf nclicks = 3 AndAlso sun.swing.SwingUtilities2.canEventAccessSystemClipboard(e) Then
						Dim a As Action = Nothing
						Dim map As ActionMap = component.actionMap
						If map IsNot Nothing Then a = map.get(DefaultEditorKit.selectLineAction)
						If a Is Nothing Then
							If selectLine Is Nothing Then selectLine = New DefaultEditorKit.SelectLineAction
							a = selectLine
						End If
						a.actionPerformed(New java.awt.event.ActionEvent(component, java.awt.event.ActionEvent.ACTION_PERFORMED, Nothing, e.when, e.modifiers))
					End If
				ElseIf SwingUtilities.isMiddleMouseButton(e) Then
					' mouse 2 behavior
					If nclicks = 1 AndAlso component.editable AndAlso component.enabled AndAlso sun.swing.SwingUtilities2.canEventAccessSystemClipboard(e) Then
						' paste system selection, if it exists
						Dim c As JTextComponent = CType(e.source, JTextComponent)
						If c IsNot Nothing Then
							Try
								Dim tk As Toolkit = c.toolkit
								Dim buffer As Clipboard = tk.systemSelection
								If buffer IsNot Nothing Then
									' platform supports system selections, update it.
									adjustCaret(e)
									Dim th As TransferHandler = c.transferHandler
									If th IsNot Nothing Then
										Dim trans As Transferable = Nothing

										Try
											trans = buffer.getContents(Nothing)
										Catch ise As IllegalStateException
											' clipboard was unavailable
											UIManager.lookAndFeel.provideErrorFeedback(c)
										End Try

										If trans IsNot Nothing Then th.importData(c, trans)
									End If
									adjustFocus(True)
								End If
							Catch he As HeadlessException
								' do nothing... there is no system clipboard
							End Try
						End If
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' If button 1 is pressed, this is implemented to
		''' request focus on the associated text component,
		''' and to set the caret position. If the shift key is held down,
		''' the caret will be moved, potentially resulting in a selection,
		''' otherwise the
		''' caret position will be set to the new location.  If the component
		''' is not enabled, there will be no request for focus.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseListener#mousePressed </seealso>
		Public Overridable Sub mousePressed(ByVal e As MouseEvent)
			Dim nclicks As Integer = sun.swing.SwingUtilities2.getAdjustedClickCount(component, e)

			If SwingUtilities.isLeftMouseButton(e) Then
				If e.consumed Then
					shouldHandleRelease = True
				Else
					shouldHandleRelease = False
					adjustCaretAndFocus(e)
					If nclicks = 2 AndAlso sun.swing.SwingUtilities2.canEventAccessSystemClipboard(e) Then selectWord(e)
				End If
			End If
		End Sub

		Friend Overridable Sub adjustCaretAndFocus(ByVal e As MouseEvent)
			adjustCaret(e)
			adjustFocus(False)
		End Sub

		''' <summary>
		''' Adjusts the caret location based on the MouseEvent.
		''' </summary>
		Private Sub adjustCaret(ByVal e As MouseEvent)
			If (e.modifiers And java.awt.event.ActionEvent.SHIFT_MASK) <> 0 AndAlso dot <> -1 Then
				moveCaret(e)
			ElseIf Not e.popupTrigger Then
				positionCaret(e)
			End If
		End Sub

		''' <summary>
		''' Adjusts the focus, if necessary.
		''' </summary>
		''' <param name="inWindow"> if true indicates requestFocusInWindow should be used </param>
		Private Sub adjustFocus(ByVal inWindow As Boolean)
			If (component IsNot Nothing) AndAlso component.enabled AndAlso component.requestFocusEnabled Then
				If inWindow Then
					component.requestFocusInWindow()
				Else
					component.requestFocus()
				End If
			End If
		End Sub

		''' <summary>
		''' Called when the mouse is released.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseListener#mouseReleased </seealso>
		Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			If (Not e.consumed) AndAlso shouldHandleRelease AndAlso SwingUtilities.isLeftMouseButton(e) Then adjustCaretAndFocus(e)
		End Sub

		''' <summary>
		''' Called when the mouse enters a region.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseListener#mouseEntered </seealso>
		Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
		End Sub

		''' <summary>
		''' Called when the mouse exits a region.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseListener#mouseExited </seealso>
		Public Overridable Sub mouseExited(ByVal e As MouseEvent)
		End Sub

		' --- MouseMotionListener methods -------------------------

		''' <summary>
		''' Moves the caret position
		''' according to the mouse pointer's current
		''' location.  This effectively extends the
		''' selection.  By default, this is only done
		''' for mouse button 1.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseMotionListener#mouseDragged </seealso>
		Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
			If ((Not e.consumed)) AndAlso SwingUtilities.isLeftMouseButton(e) Then moveCaret(e)
		End Sub

		''' <summary>
		''' Called when the mouse is moved.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref= MouseMotionListener#mouseMoved </seealso>
		Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
		End Sub

		' ---- Caret methods ---------------------------------

		''' <summary>
		''' Renders the caret as a vertical line.  If this is reimplemented
		''' the damage method should also be reimplemented as it assumes the
		''' shape of the caret is a vertical line.  Sets the caret color to
		''' the value returned by getCaretColor().
		''' <p>
		''' If there are multiple text directions present in the associated
		''' document, a flag indicating the caret bias will be rendered.
		''' This will occur only if the associated document is a subclass
		''' of AbstractDocument and there are multiple bidi levels present
		''' in the bidi element structure (i.e. the text has multiple
		''' directions associated with it).
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <seealso cref= #damage </seealso>
		Public Overridable Sub paint(ByVal g As Graphics)
			If visible Then
				Try
					Dim mapper As TextUI = component.uI
					Dim r As Rectangle = mapper.modelToView(component, dot, dotBias)

					If (r Is Nothing) OrElse ((r.width = 0) AndAlso (r.height = 0)) Then Return
					If width > 0 AndAlso height > 0 AndAlso (Not Me._contains(r.x, r.y, r.width, r.height)) Then
						' We seem to have gotten out of sync and no longer
						' contain the right location, adjust accordingly.
						Dim clip As Rectangle = g.clipBounds

						If clip IsNot Nothing AndAlso (Not clip.contains(Me)) Then repaint()
						' This will potentially cause a repaint of something
						' we're already repainting, but without changing the
						' semantics of damage we can't really get around this.
						damage(r)
					End If
					g.color = component.caretColor
					Dim paintWidth As Integer = getCaretWidth(r.height)
					r.x -= paintWidth >> 1
					g.fillRect(r.x, r.y, paintWidth, r.height)

					' see if we should paint a flag to indicate the bias
					' of the caret.
					' PENDING(prinz) this should be done through
					' protected methods so that alternative LAF
					' will show bidi information.
					Dim doc As Document = component.document
					If TypeOf doc Is AbstractDocument Then
						Dim bidi As Element = CType(doc, AbstractDocument).bidiRootElement
						If (bidi IsNot Nothing) AndAlso (bidi.elementCount > 1) Then
							' there are multiple directions present.
							flagXPoints(0) = r.x + (If(dotLTR, paintWidth, 0))
							flagYPoints(0) = r.y
							flagXPoints(1) = flagXPoints(0)
							flagYPoints(1) = flagYPoints(0) + 4
							flagXPoints(2) = flagXPoints(0) + (If(dotLTR, 4, -4))
							flagYPoints(2) = flagYPoints(0)
							g.fillPolygon(flagXPoints, flagYPoints, 3)
						End If
					End If
				Catch e As BadLocationException
					' can't render I guess
					'System.err.println("Can't render cursor");
				End Try
			End If
		End Sub

		''' <summary>
		''' Called when the UI is being installed into the
		''' interface of a JTextComponent.  This can be used
		''' to gain access to the model that is being navigated
		''' by the implementation of this interface.  Sets the dot
		''' and mark to 0, and establishes document, property change,
		''' focus, mouse, and mouse motion listeners.
		''' </summary>
		''' <param name="c"> the component </param>
		''' <seealso cref= Caret#install </seealso>
		Public Overridable Sub install(ByVal c As JTextComponent) Implements Caret.install
			component = c
			Dim doc As Document = c.document
				mark = 0
				dot = mark
				markLTR = True
				dotLTR = markLTR
				markBias = Position.Bias.Forward
				dotBias = markBias
			If doc IsNot Nothing Then doc.addDocumentListener(handler)
			c.addPropertyChangeListener(handler)
			c.addFocusListener(Me)
			c.addMouseListener(Me)
			c.addMouseMotionListener(Me)

			' if the component already has focus, it won't
			' be notified.
			If component.hasFocus() Then focusGained(Nothing)

			Dim ratio As Number = CType(c.getClientProperty("caretAspectRatio"), Number)
			If ratio IsNot Nothing Then
				aspectRatio = ratio
			Else
				aspectRatio = -1
			End If

			Dim width As Integer? = CInt(Fix(c.getClientProperty("caretWidth")))
			If width IsNot Nothing Then
				caretWidth = width
			Else
				caretWidth = -1
			End If
		End Sub

		''' <summary>
		''' Called when the UI is being removed from the
		''' interface of a JTextComponent.  This is used to
		''' unregister any listeners that were attached.
		''' </summary>
		''' <param name="c"> the component </param>
		''' <seealso cref= Caret#deinstall </seealso>
		Public Overridable Sub deinstall(ByVal c As JTextComponent) Implements Caret.deinstall
			c.removeMouseListener(Me)
			c.removeMouseMotionListener(Me)
			c.removeFocusListener(Me)
			c.removePropertyChangeListener(handler)
			Dim doc As Document = c.document
			If doc IsNot Nothing Then doc.removeDocumentListener(handler)
			SyncLock Me
				component = Nothing
			End SyncLock
			If flasher IsNot Nothing Then flasher.stop()


		End Sub

		''' <summary>
		''' Adds a listener to track whenever the caret position has
		''' been changed.
		''' </summary>
		''' <param name="l"> the listener </param>
		''' <seealso cref= Caret#addChangeListener </seealso>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener)
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a listener that was tracking caret position changes.
		''' </summary>
		''' <param name="l"> the listener </param>
		''' <seealso cref= Caret#removeChangeListener </seealso>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener)
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the change listeners
		''' registered on this caret.
		''' </summary>
		''' <returns> all of this caret's <code>ChangeListener</code>s
		'''         or an empty
		'''         array if no change listeners are currently registered
		''' </returns>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= #removeChangeListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.  The listener list is processed last to first.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this caret.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' 
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal,
		''' such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>DefaultCaret</code> <code>c</code>
		''' for its change listeners with the following code:
		''' 
		''' <pre>ChangeListener[] cls = (ChangeListener[])(c.getListeners(ChangeListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this component,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getChangeListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		''' <summary>
		''' Changes the selection visibility.
		''' </summary>
		''' <param name="vis"> the new visibility </param>
		Public Overridable Property selectionVisible Implements Caret.setSelectionVisible As Boolean
			Set(ByVal vis As Boolean)
				If vis <> selectionVisible Then
					selectionVisible = vis
					If selectionVisible Then
						' show
						Dim h As Highlighter = component.highlighter
						If (dot <> mark) AndAlso (h IsNot Nothing) AndAlso (selectionTag Is Nothing) Then
							Dim p0 As Integer = Math.Min(dot, mark)
							Dim p1 As Integer = Math.Max(dot, mark)
							Dim p As Highlighter.HighlightPainter = selectionPainter
							Try
								selectionTag = h.addHighlight(p0, p1, p)
							Catch bl As BadLocationException
								selectionTag = Nothing
							End Try
						End If
					Else
						' hide
						If selectionTag IsNot Nothing Then
							Dim h As Highlighter = component.highlighter
							h.removeHighlight(selectionTag)
							selectionTag = Nothing
						End If
					End If
				End If
			End Set
			Get
				Return selectionVisible
			End Get
		End Property


		''' <summary>
		''' Determines if the caret is currently active.
		''' <p>
		''' This method returns whether or not the <code>Caret</code>
		''' is currently in a blinking state. It does not provide
		''' information as to whether it is currently blinked on or off.
		''' To determine if the caret is currently painted use the
		''' <code>isVisible</code> method.
		''' </summary>
		''' <returns> <code>true</code> if active else <code>false</code> </returns>
		''' <seealso cref= #isVisible
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Property active As Boolean
			Get
				Return active
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not the caret is currently visible. As the
		''' caret flashes on and off the return value of this will change
		''' between true, when the caret is painted, and false, when the
		''' caret is not painted. <code>isActive</code> indicates whether
		''' or not the caret is in a blinking state, such that it <b>can</b>
		''' be visible, and <code>isVisible</code> indicates whether or not
		''' the caret <b>is</b> actually visible.
		''' <p>
		''' Subclasses that wish to render a different flashing caret
		''' should override paint and only paint the caret if this method
		''' returns true.
		''' </summary>
		''' <returns> true if visible else false </returns>
		''' <seealso cref= Caret#isVisible </seealso>
		''' <seealso cref= #isActive </seealso>
		Public Overridable Property visible As Boolean Implements Caret.isVisible
			Get
				Return visible
			End Get
			Set(ByVal e As Boolean)
				' focus lost notification can come in later after the
				' caret has been deinstalled, in which case the component
				' will be null.
				active = e
				If component IsNot Nothing Then
					Dim mapper As TextUI = component.uI
					If visible <> e Then
						visible = e
						' repaint the caret
						Try
							Dim loc As Rectangle = mapper.modelToView(component, dot,dotBias)
							damage(loc)
						Catch badloc As BadLocationException
							' hmm... not legally positioned
						End Try
					End If
				End If
				If flasher IsNot Nothing Then
					If visible Then
						flasher.start()
					Else
						flasher.stop()
					End If
				End If
			End Set
		End Property


		''' <summary>
		''' Sets the caret blink rate.
		''' </summary>
		''' <param name="rate"> the rate in milliseconds, 0 to stop blinking </param>
		''' <seealso cref= Caret#setBlinkRate </seealso>
		Public Overridable Property blinkRate Implements Caret.setBlinkRate As Integer
			Set(ByVal rate As Integer)
				If rate <> 0 Then
					If flasher Is Nothing Then flasher = New Timer(rate, handler)
					flasher.delay = rate
				Else
					If flasher IsNot Nothing Then
						flasher.stop()
						flasher.removeActionListener(handler)
						flasher = Nothing
					End If
				End If
			End Set
			Get
				Return If(flasher Is Nothing, 0, flasher.delay)
			End Get
		End Property


		''' <summary>
		''' Fetches the current position of the caret.
		''' </summary>
		''' <returns> the position &gt;= 0 </returns>
		''' <seealso cref= Caret#getDot </seealso>
		Public Overridable Property dot As Integer Implements Caret.getDot
			Get
				Return dot
			End Get
			Set(ByVal dot As Integer)
				dotDot(dot, Position.Bias.Forward)
			End Set
		End Property

		''' <summary>
		''' Fetches the current position of the mark.  If there is a selection,
		''' the dot and mark will not be the same.
		''' </summary>
		''' <returns> the position &gt;= 0 </returns>
		''' <seealso cref= Caret#getMark </seealso>
		Public Overridable Property mark As Integer Implements Caret.getMark
			Get
				Return mark
			End Get
		End Property


		''' <summary>
		''' Moves the caret position to the specified position,
		''' with a forward bias.
		''' </summary>
		''' <param name="dot"> the position &gt;= 0 </param>
		''' <seealso cref= #moveDot(int, javax.swing.text.Position.Bias) </seealso>
		''' <seealso cref= Caret#moveDot </seealso>
		Public Overridable Sub moveDot(ByVal dot As Integer) Implements Caret.moveDot
			moveDot(dot, Position.Bias.Forward)
		End Sub

		' ---- Bidi methods (we could put these in a subclass)

		''' <summary>
		''' Moves the caret position to the specified position, with the
		''' specified bias.
		''' </summary>
		''' <param name="dot"> the position &gt;= 0 </param>
		''' <param name="dotBias"> the bias for this position, not <code>null</code> </param>
		''' <exception cref="IllegalArgumentException"> if the bias is <code>null</code> </exception>
		''' <seealso cref= Caret#moveDot
		''' @since 1.6 </seealso>
		Public Overridable Sub moveDot(ByVal dot As Integer, ByVal dotBias As Position.Bias)
			If dotBias Is Nothing Then Throw New System.ArgumentException("null bias")

			If Not component.enabled Then
				' don't allow selection on disabled components.
				dotDot(dot, dotBias)
				Return
			End If
			If dot <> Me.dot Then
				Dim filter As NavigationFilter = component.navigationFilter

				If filter IsNot Nothing Then
					filter.moveDot(filterBypass, dot, dotBias)
				Else
					handleMoveDot(dot, dotBias)
				End If
			End If
		End Sub

		Friend Overridable Sub handleMoveDot(ByVal dot As Integer, ByVal dotBias As Position.Bias)
			changeCaretPosition(dot, dotBias)

			If selectionVisible Then
				Dim h As Highlighter = component.highlighter
				If h IsNot Nothing Then
					Dim p0 As Integer = Math.Min(dot, mark)
					Dim p1 As Integer = Math.Max(dot, mark)

					' if p0 == p1 then there should be no highlight, remove it if necessary
					If p0 = p1 Then
						If selectionTag IsNot Nothing Then
							h.removeHighlight(selectionTag)
							selectionTag = Nothing
						End If
					' otherwise, change or add the highlight
					Else
						Try
							If selectionTag IsNot Nothing Then
								h.changeHighlight(selectionTag, p0, p1)
							Else
								Dim p As Highlighter.HighlightPainter = selectionPainter
								selectionTag = h.addHighlight(p0, p1, p)
							End If
						Catch e As BadLocationException
							Throw New StateInvariantError("Bad caret position")
						End Try
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Sets the caret position and mark to the specified position, with the
		''' specified bias. This implicitly sets the selection range
		''' to zero.
		''' </summary>
		''' <param name="dot"> the position &gt;= 0 </param>
		''' <param name="dotBias"> the bias for this position, not <code>null</code> </param>
		''' <exception cref="IllegalArgumentException"> if the bias is <code>null</code> </exception>
		''' <seealso cref= Caret#setDot
		''' @since 1.6 </seealso>
		Public Overridable Sub setDot(ByVal dot As Integer, ByVal dotBias As Position.Bias)
			If dotBias Is Nothing Then Throw New System.ArgumentException("null bias")

			Dim filter As NavigationFilter = component.navigationFilter

			If filter IsNot Nothing Then
				filter.dotDot(filterBypass, dot, dotBias)
			Else
				handleSetDot(dot, dotBias)
			End If
		End Sub

		Friend Overridable Sub handleSetDot(ByVal dot As Integer, ByVal dotBias As Position.Bias)
			' move dot, if it changed
			Dim doc As Document = component.document
			If doc IsNot Nothing Then dot = Math.Min(dot, doc.length)
			dot = Math.Max(dot, 0)

			' The position (0,Backward) is out of range so disallow it.
			If dot = 0 Then dotBias = Position.Bias.Forward

			mark = dot
			If Me.dot <> dot OrElse Me.dotBias IsNot dotBias OrElse selectionTag IsNot Nothing OrElse forceCaretPositionChange Then changeCaretPosition(dot, dotBias)
			Me.markBias = Me.dotBias
			Me.markLTR = dotLTR
			Dim h As Highlighter = component.highlighter
			If (h IsNot Nothing) AndAlso (selectionTag IsNot Nothing) Then
				h.removeHighlight(selectionTag)
				selectionTag = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns the bias of the caret position.
		''' </summary>
		''' <returns> the bias of the caret position
		''' @since 1.6 </returns>
		Public Overridable Property dotBias As Position.Bias
			Get
				Return dotBias
			End Get
		End Property

		''' <summary>
		''' Returns the bias of the mark.
		''' </summary>
		''' <returns> the bias of the mark
		''' @since 1.6 </returns>
		Public Overridable Property markBias As Position.Bias
			Get
				Return markBias
			End Get
		End Property

		Friend Overridable Property dotLeftToRight As Boolean
			Get
				Return dotLTR
			End Get
		End Property

		Friend Overridable Property markLeftToRight As Boolean
			Get
				Return markLTR
			End Get
		End Property

		Friend Overridable Function isPositionLTR(ByVal position As Integer, ByVal bias As Position.Bias) As Boolean
			Dim doc As Document = component.document
			position -= 1
			If bias Is Position.Bias.Backward AndAlso position < 0 Then position = 0
			Return AbstractDocument.isLeftToRight(doc, position, position)
		End Function

		Friend Overridable Function guessBiasForOffset(ByVal offset As Integer, ByVal lastBias As Position.Bias, ByVal lastLTR As Boolean) As Position.Bias
			' There is an abiguous case here. That if your model looks like:
			' abAB with the cursor at abB]A (visual representation of
			' 3 forward) deleting could either become abB] or
			' ab[B. I'ld actually prefer abB]. But, if I implement that
			' a delete at abBA] would result in aBA] vs a[BA which I
			' think is totally wrong. To get this right we need to know what
			' was deleted. And we could get this from the bidi structure
			' in the change event. So:
			' PENDING: base this off what was deleted.
			If lastLTR <> isPositionLTR(offset, lastBias) Then
				lastBias = Position.Bias.Backward
			ElseIf lastBias IsNot Position.Bias.Backward AndAlso lastLTR <> isPositionLTR(offset, Position.Bias.Backward) Then
				lastBias = Position.Bias.Backward
			End If
			If lastBias Is Position.Bias.Backward AndAlso offset > 0 Then
				Try
					Dim s As New Segment
					component.document.getText(offset - 1, 1, s)
					If s.count > 0 AndAlso s.array(s.offset) = ControlChars.Lf Then lastBias = Position.Bias.Forward
				Catch ble As BadLocationException
				End Try
			End If
			Return lastBias
		End Function

		' ---- local methods --------------------------------------------

		''' <summary>
		''' Sets the caret position (dot) to a new location.  This
		''' causes the old and new location to be repainted.  It
		''' also makes sure that the caret is within the visible
		''' region of the view, if the view is scrollable.
		''' </summary>
		Friend Overridable Sub changeCaretPosition(ByVal dot As Integer, ByVal dotBias As Position.Bias)
			' repaint the old position and set the new value of
			' the dot.
			repaint()


			' Make sure the caret is visible if this window has the focus.
			If flasher IsNot Nothing AndAlso flasher.running Then
				visible = True
				flasher.restart()
			End If

			' notify listeners at the caret moved
			Me.dot = dot
			Me.dotBias = dotBias
			dotLTR = isPositionLTR(dot, dotBias)
			fireStateChanged()

			updateSystemSelection()

			magicCaretPosition = Nothing

			' We try to repaint the caret later, since things
			' may be unstable at the time this is called
			' (i.e. we don't want to depend upon notification
			' order or the fact that this might happen on
			' an unsafe thread).
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Runnable callRepaintNewCaret = New Runnable()
	'		{
	'			public void run()
	'			{
	'				repaintNewCaret();
	'			}
	'		};
			SwingUtilities.invokeLater(callRepaintNewCaret)
		End Sub

		''' <summary>
		''' Repaints the new caret position, with the
		''' assumption that this is happening on the
		''' event thread so that calling <code>modelToView</code>
		''' is safe.
		''' </summary>
		Friend Overridable Sub repaintNewCaret()
			If component IsNot Nothing Then
				Dim mapper As TextUI = component.uI
				Dim doc As Document = component.document
				If (mapper IsNot Nothing) AndAlso (doc IsNot Nothing) Then
					' determine the new location and scroll if
					' not visible.
					Dim newLoc As Rectangle
					Try
						newLoc = mapper.modelToView(component, Me.dot, Me.dotBias)
					Catch e As BadLocationException
						newLoc = Nothing
					End Try
					If newLoc IsNot Nothing Then
						adjustVisibility(newLoc)
						' If there is no magic caret position, make one
						If magicCaretPosition Is Nothing Then magicCaretPosition = New Point(newLoc.x, newLoc.y)
					End If

					' repaint the new position
					damage(newLoc)
				End If
			End If
		End Sub

		Private Sub updateSystemSelection()
			If Not sun.swing.SwingUtilities2.canCurrentEventAccessSystemClipboard() Then Return
			If Me.dot <> Me.mark AndAlso component IsNot Nothing AndAlso component.hasFocus() Then
				Dim clip As Clipboard = systemSelection
				If clip IsNot Nothing Then
					Dim selectedText As String
					If TypeOf component Is JPasswordField AndAlso component.getClientProperty("JPasswordField.cutCopyAllowed") IsNot Boolean.TRUE Then
						'fix for 4793761
						Dim txt As StringBuilder = Nothing
						Dim echoChar As Char = CType(component, JPasswordField).echoChar
						Dim p0 As Integer = Math.Min(dot, mark)
						Dim p1 As Integer = Math.Max(dot, mark)
						For i As Integer = p0 To p1 - 1
							If txt Is Nothing Then txt = New StringBuilder
							txt.Append(echoChar)
						Next i
						selectedText = If(txt IsNot Nothing, txt.ToString(), Nothing)
					Else
						selectedText = component.selectedText
					End If
					Try
						clip.contentsnts(New StringSelection(selectedText), clipboardOwner)

						ownsSelection = True
					Catch ise As IllegalStateException
						' clipboard was unavailable
						' no need to provide error feedback to user since updating
						' the system selection is not a user invoked action
					End Try
				End If
			End If
		End Sub

		Private Property systemSelection As Clipboard
			Get
				Try
					Return component.toolkit.systemSelection
				Catch he As HeadlessException
					' do nothing... there is no system clipboard
				Catch se As SecurityException
					' do nothing... there is no allowed system clipboard
				End Try
				Return Nothing
			End Get
		End Property

		Private Property clipboardOwner As ClipboardOwner
			Get
				Return handler
			End Get
		End Property

		''' <summary>
		''' This is invoked after the document changes to verify the current
		''' dot/mark is valid. We do this in case the <code>NavigationFilter</code>
		''' changed where to position the dot, that resulted in the current location
		''' being bogus.
		''' </summary>
		Private Sub ensureValidPosition()
			Dim length As Integer = component.document.length
			If dot > length OrElse mark > length Then handleSetDot(length, Position.Bias.Forward)
		End Sub


		''' <summary>
		''' Saves the current caret position.  This is used when
		''' caret up/down actions occur, moving between lines
		''' that have uneven end positions.
		''' </summary>
		''' <param name="p"> the position </param>
		''' <seealso cref= #getMagicCaretPosition </seealso>
		Public Overridable Property magicCaretPosition As Point
			Set(ByVal p As Point)
				magicCaretPosition = p
			End Set
			Get
				Return magicCaretPosition
			End Get
		End Property


		''' <summary>
		''' Compares this object to the specified object.
		''' The superclass behavior of comparing rectangles
		''' is not desired, so this is changed to the Object
		''' behavior.
		''' </summary>
		''' <param name="obj">   the object to compare this font with </param>
		''' <returns>    <code>true</code> if the objects are equal;
		'''            <code>false</code> otherwise </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return (Me Is obj)
		End Function

		Public Overrides Function ToString() As String
			Dim s As String = "Dot=(" & dot & ", " & dotBias & ")"
			s &= " Mark=(" & mark & ", " & markBias & ")"
			Return s
		End Function

		Private Property filterBypass As NavigationFilter.FilterBypass
			Get
				If filterBypass Is Nothing Then filterBypass = New DefaultFilterBypass(Me)
				Return filterBypass
			End Get
		End Property

		' Rectangle.contains returns false if passed a rect with a w or h == 0,
		' this won't (assuming X,Y are contained with this rectangle).
		Private Function _contains(ByVal X As Integer, ByVal Y As Integer, ByVal W As Integer, ByVal H As Integer) As Boolean
			Dim ___w As Integer = Me.width
			Dim ___h As Integer = Me.height
			If (___w Or ___h Or W Or H) < 0 Then Return False
			' Note: if any dimension is zero, tests below must return false...
			Dim ___x As Integer = Me.x
			Dim ___y As Integer = Me.y
			If X < ___x OrElse Y < ___y Then Return False
			If W > 0 Then
				___w += ___x
				W += X
				If W <= X Then
					' X+W overflowed or W was zero, return false if...
					' either original w or W was zero or
					' x+w did not overflow or
					' the overflowed x+w is smaller than the overflowed X+W
					If ___w >= ___x OrElse W > ___w Then Return False
				Else
					' X+W did not overflow and W was not zero, return false if...
					' original w was zero or
					' x+w did not overflow and x+w is smaller than X+W
					If ___w >= ___x AndAlso W > ___w Then Return False
				End If
			ElseIf (___x + ___w) < X Then
				Return False
			End If
			If H > 0 Then
				___h += ___y
				H += Y
				If H <= Y Then
					If ___h >= ___y OrElse H > ___h Then Return False
				Else
					If ___h >= ___y AndAlso H > ___h Then Return False
				End If
			ElseIf (___y + ___h) < Y Then
				Return False
			End If
			Return True
		End Function

		Friend Overridable Function getCaretWidth(ByVal height As Integer) As Integer
			If aspectRatio > -1 Then Return CInt(Fix(aspectRatio * height)) + 1

			If caretWidth > -1 Then
				Return caretWidth
			Else
				Dim [property] As Object = UIManager.get("Caret.width")
				If TypeOf [property] Is Integer? Then
					Return CInt(Fix([property]))
				Else
					Return 1
				End If
			End If
		End Function

		' --- serialization ---------------------------------------------

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			handler = New Handler(Me)
			If Not s.readBoolean() Then
				dotBias = Position.Bias.Forward
			Else
				dotBias = Position.Bias.Backward
			End If
			If Not s.readBoolean() Then
				markBias = Position.Bias.Forward
			Else
				markBias = Position.Bias.Backward
			End If
		End Sub

		Private Sub writeObject(ByVal s As ObjectOutputStream)
			s.defaultWriteObject()
			s.writeBoolean((dotBias Is Position.Bias.Backward))
			s.writeBoolean((markBias Is Position.Bias.Backward))
		End Sub

		' ---- member variables ------------------------------------------

		''' <summary>
		''' The event listener list.
		''' </summary>
		Protected Friend listenerList As New EventListenerList

		''' <summary>
		''' The change event for the model.
		''' Only one ChangeEvent is needed per model instance since the
		''' event's only (read-only) state is the source property.  The source
		''' of events generated here is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		' package-private to avoid inner classes private member
		' access bug
		Friend component As JTextComponent

		Friend updatePolicy As Integer = UPDATE_WHEN_ON_EDT
		Friend visible As Boolean
		Friend active As Boolean
		Friend dot As Integer
		Friend mark As Integer
		Friend selectionTag As Object
		Friend selectionVisible As Boolean
		Friend flasher As Timer
		Friend magicCaretPosition As Point
		<NonSerialized> _
		Friend dotBias As Position.Bias
		<NonSerialized> _
		Friend markBias As Position.Bias
		Friend dotLTR As Boolean
		Friend markLTR As Boolean
		<NonSerialized> _
		Friend handler As New Handler(Me)
		<NonSerialized> _
		Private flagXPoints As Integer() = New Integer(2){}
		<NonSerialized> _
		Private flagYPoints As Integer() = New Integer(2){}
		<NonSerialized> _
		Private filterBypass As NavigationFilter.FilterBypass
		<NonSerialized> _
		Private Shared ___selectWord As Action = Nothing
		<NonSerialized> _
		Private Shared selectLine As Action = Nothing
		''' <summary>
		''' This is used to indicate if the caret currently owns the selection.
		''' This is always false if the system does not support the system
		''' clipboard.
		''' </summary>
		Private ownsSelection As Boolean

		''' <summary>
		''' If this is true, the location of the dot is updated regardless of
		''' the current location. This is set in the DocumentListener
		''' such that even if the model location of dot hasn't changed (perhaps do
		''' to a forward delete) the visual location is updated.
		''' </summary>
		Private forceCaretPositionChange As Boolean

		''' <summary>
		''' Whether or not mouseReleased should adjust the caret and focus.
		''' This flag is set by mousePressed if it wanted to adjust the caret
		''' and focus but couldn't because of a possible DnD operation.
		''' </summary>
		<NonSerialized> _
		Private shouldHandleRelease As Boolean


		''' <summary>
		''' holds last MouseEvent which caused the word selection
		''' </summary>
		<NonSerialized> _
		Private selectedWordEvent As MouseEvent = Nothing

		''' <summary>
		''' The width of the caret in pixels.
		''' </summary>
		Private caretWidth As Integer = -1
		Private aspectRatio As Single = -1

		Friend Class SafeScroller
			Implements Runnable

			Private ReadOnly outerInstance As DefaultCaret


			Friend Sub New(ByVal outerInstance As DefaultCaret, ByVal r As Rectangle)
					Me.outerInstance = outerInstance
				Me.r = r
			End Sub

			Public Overridable Sub run()
				If outerInstance.component IsNot Nothing Then outerInstance.component.scrollRectToVisible(r)
			End Sub

			Friend r As Rectangle
		End Class


		Friend Class Handler
			Implements PropertyChangeListener, DocumentListener, java.awt.event.ActionListener, ClipboardOwner

			Private ReadOnly outerInstance As DefaultCaret

			Public Sub New(ByVal outerInstance As DefaultCaret)
				Me.outerInstance = outerInstance
			End Sub


			' --- ActionListener methods ----------------------------------

			''' <summary>
			''' Invoked when the blink timer fires.  This is called
			''' asynchronously.  The simply changes the visibility
			''' and repaints the rectangle that last bounded the caret.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				If width = 0 OrElse height = 0 Then
					' setVisible(true) will cause a scroll, only do this if the
					' new location is really valid.
					If outerInstance.component IsNot Nothing Then
						Dim mapper As TextUI = outerInstance.component.uI
						Try
							Dim r As Rectangle = mapper.modelToView(outerInstance.component, outerInstance.dot, outerInstance.dotBias)
							If r IsNot Nothing AndAlso r.width <> 0 AndAlso r.height <> 0 Then outerInstance.damage(r)
						Catch ble As BadLocationException
						End Try
					End If
				End If
				outerInstance.visible = Not outerInstance.visible
				outerInstance.repaint()
			End Sub

			' --- DocumentListener methods --------------------------------

			''' <summary>
			''' Updates the dot and mark if they were changed by
			''' the insertion.
			''' </summary>
			''' <param name="e"> the document event </param>
			''' <seealso cref= DocumentListener#insertUpdate </seealso>
			Public Overridable Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
				If outerInstance.updatePolicy = NEVER_UPDATE OrElse (outerInstance.updatePolicy = UPDATE_WHEN_ON_EDT AndAlso (Not SwingUtilities.eventDispatchThread)) Then

					If (e.offset <= outerInstance.dot OrElse e.offset <= outerInstance.mark) AndAlso outerInstance.selectionTag IsNot Nothing Then
						Try
							outerInstance.component.highlighter.changeHighlight(outerInstance.selectionTag, Math.Min(outerInstance.dot, outerInstance.mark), Math.Max(outerInstance.dot, outerInstance.mark))
						Catch e1 As BadLocationException
							Console.WriteLine(e1.ToString())
							Console.Write(e1.StackTrace)
						End Try
					End If
					Return
				End If
				Dim offset As Integer = e.offset
				Dim length As Integer = e.length
				Dim newDot As Integer = outerInstance.dot
				Dim changed As Short = 0

				If TypeOf e Is AbstractDocument.UndoRedoDocumentEvent Then
					outerInstance.dot = offset + length
					Return
				End If
				If newDot >= offset Then
					newDot += length
					changed = changed Or 1
				End If
				Dim newMark As Integer = outerInstance.mark
				If newMark >= offset Then
					newMark += length
					changed = changed Or 2
				End If

				If changed <> 0 Then
					Dim dotBias As Position.Bias = outerInstance.dotBias
					If outerInstance.dot = offset Then
						Dim doc As Document = outerInstance.component.document
						Dim isNewline As Boolean
						Try
							Dim s As New Segment
							doc.getText(newDot - 1, 1, s)
							isNewline = (s.count > 0 AndAlso s.array(s.offset) = ControlChars.Lf)
						Catch ble As BadLocationException
							isNewline = False
						End Try
						If isNewline Then
							dotBias = Position.Bias.Forward
						Else
							dotBias = Position.Bias.Backward
						End If
					End If
					If newMark = newDot Then
						outerInstance.dotDot(newDot, dotBias)
						outerInstance.ensureValidPosition()
					Else
						outerInstance.dotDot(newMark, outerInstance.markBias)
						If outerInstance.dot = newMark Then outerInstance.moveDot(newDot, dotBias)
						outerInstance.ensureValidPosition()
					End If
				End If
			End Sub

			''' <summary>
			''' Updates the dot and mark if they were changed
			''' by the removal.
			''' </summary>
			''' <param name="e"> the document event </param>
			''' <seealso cref= DocumentListener#removeUpdate </seealso>
			Public Overridable Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
				If outerInstance.updatePolicy = NEVER_UPDATE OrElse (outerInstance.updatePolicy = UPDATE_WHEN_ON_EDT AndAlso (Not SwingUtilities.eventDispatchThread)) Then

					Dim length As Integer = outerInstance.component.document.length
					outerInstance.dot = Math.Min(outerInstance.dot, length)
					outerInstance.mark = Math.Min(outerInstance.mark, length)
					If (e.offset < outerInstance.dot OrElse e.offset < outerInstance.mark) AndAlso outerInstance.selectionTag IsNot Nothing Then
						Try
							outerInstance.component.highlighter.changeHighlight(outerInstance.selectionTag, Math.Min(outerInstance.dot, outerInstance.mark), Math.Max(outerInstance.dot, outerInstance.mark))
						Catch e1 As BadLocationException
							Console.WriteLine(e1.ToString())
							Console.Write(e1.StackTrace)
						End Try
					End If
					Return
				End If
				Dim offs0 As Integer = e.offset
				Dim offs1 As Integer = offs0 + e.length
				Dim newDot As Integer = outerInstance.dot
				Dim adjustDotBias As Boolean = False
				Dim newMark As Integer = outerInstance.mark
				Dim adjustMarkBias As Boolean = False

				If TypeOf e Is AbstractDocument.UndoRedoDocumentEvent Then
					outerInstance.dot = offs0
					Return
				End If
				If newDot >= offs1 Then
					newDot -= (offs1 - offs0)
					If newDot = offs1 Then adjustDotBias = True
				ElseIf newDot >= offs0 Then
					newDot = offs0
					adjustDotBias = True
				End If
				If newMark >= offs1 Then
					newMark -= (offs1 - offs0)
					If newMark = offs1 Then adjustMarkBias = True
				ElseIf newMark >= offs0 Then
					newMark = offs0
					adjustMarkBias = True
				End If
				If newMark = newDot Then
					outerInstance.forceCaretPositionChange = True
					Try
						outerInstance.dotDot(newDot, outerInstance.guessBiasForOffset(newDot, outerInstance.dotBias, outerInstance.dotLTR))
					Finally
						outerInstance.forceCaretPositionChange = False
					End Try
					outerInstance.ensureValidPosition()
				Else
					Dim dotBias As Position.Bias = outerInstance.dotBias
					Dim markBias As Position.Bias = outerInstance.markBias
					If adjustDotBias Then dotBias = outerInstance.guessBiasForOffset(newDot, dotBias, outerInstance.dotLTR)
					If adjustMarkBias Then markBias = outerInstance.guessBiasForOffset(outerInstance.mark, markBias, outerInstance.markLTR)
					outerInstance.dotDot(newMark, markBias)
					If outerInstance.dot = newMark Then outerInstance.moveDot(newDot, dotBias)
					outerInstance.ensureValidPosition()
				End If
			End Sub

			''' <summary>
			''' Gives notification that an attribute or set of attributes changed.
			''' </summary>
			''' <param name="e"> the document event </param>
			''' <seealso cref= DocumentListener#changedUpdate </seealso>
			Public Overridable Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
				If outerInstance.updatePolicy = NEVER_UPDATE OrElse (outerInstance.updatePolicy = UPDATE_WHEN_ON_EDT AndAlso (Not SwingUtilities.eventDispatchThread)) Then Return
				If TypeOf e Is AbstractDocument.UndoRedoDocumentEvent Then outerInstance.dot = e.offset + e.length
			End Sub

			' --- PropertyChangeListener methods -----------------------

			''' <summary>
			''' This method gets called when a bound property is changed.
			''' We are looking for document changes on the editor.
			''' </summary>
			Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
				Dim oldValue As Object = evt.oldValue
				Dim newValue As Object = evt.newValue
				If (TypeOf oldValue Is Document) OrElse (TypeOf newValue Is Document) Then
					outerInstance.dot = 0
					If oldValue IsNot Nothing Then CType(oldValue, Document).removeDocumentListener(Me)
					If newValue IsNot Nothing Then CType(newValue, Document).addDocumentListener(Me)
				ElseIf "enabled".Equals(evt.propertyName) Then
					Dim enabled As Boolean? = CBool(evt.newValue)
					If outerInstance.component.focusOwner Then
						If enabled Is Boolean.TRUE Then
							If outerInstance.component.editable Then outerInstance.visible = True
							outerInstance.selectionVisible = True
						Else
							outerInstance.visible = False
							outerInstance.selectionVisible = False
						End If
					End If
				ElseIf "caretWidth".Equals(evt.propertyName) Then
					Dim newWidth As Integer? = CInt(Fix(evt.newValue))
					If newWidth IsNot Nothing Then
						outerInstance.caretWidth = newWidth
					Else
						outerInstance.caretWidth = -1
					End If
					outerInstance.repaint()
				ElseIf "caretAspectRatio".Equals(evt.propertyName) Then
					Dim newRatio As Number = CType(evt.newValue, Number)
					If newRatio IsNot Nothing Then
						outerInstance.aspectRatio = newRatio
					Else
						outerInstance.aspectRatio = -1
					End If
					outerInstance.repaint()
				End If
			End Sub


			'
			' ClipboardOwner
			'
			''' <summary>
			''' Toggles the visibility of the selection when ownership is lost.
			''' </summary>
			Public Overridable Sub lostOwnership(ByVal clipboard As Clipboard, ByVal contents As Transferable)
				If outerInstance.ownsSelection Then
					outerInstance.ownsSelection = False
					If outerInstance.component IsNot Nothing AndAlso (Not outerInstance.component.hasFocus()) Then outerInstance.selectionVisible = False
				End If
			End Sub
		End Class


		Private Class DefaultFilterBypass
			Inherits NavigationFilter.FilterBypass

			Private ReadOnly outerInstance As DefaultCaret

			Public Sub New(ByVal outerInstance As DefaultCaret)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Property caret As Caret
				Get
					Return DefaultCaret.this
				End Get
			End Property

			Public Overridable Sub setDot(ByVal dot As Integer, ByVal bias As Position.Bias)
				outerInstance.handleSetDot(dot, bias)
			End Sub

			Public Overridable Sub moveDot(ByVal dot As Integer, ByVal bias As Position.Bias)
				outerInstance.handleMoveDot(dot, bias)
			End Sub
		End Class
	End Class

End Namespace
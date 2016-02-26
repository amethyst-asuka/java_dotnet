Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Threading
Imports javax.swing.undo
Imports javax.swing.event

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
	''' An implementation of the document interface to serve as a
	''' basis for implementing various kinds of documents.  At this
	''' level there is very little policy, so there is a corresponding
	''' increase in difficulty of use.
	''' <p>
	''' This class implements a locking mechanism for the document.  It
	''' allows multiple readers or one writer, and writers must wait until
	''' all observers of the document have been notified of a previous
	''' change before beginning another mutation to the document.  The
	''' read lock is acquired and released using the <code>render</code>
	''' method.  A write lock is acquired by the methods that mutate the
	''' document, and are held for the duration of the method call.
	''' Notification is done on the thread that produced the mutation,
	''' and the thread has full read access to the document for the
	''' duration of the notification, but other readers are kept out
	''' until the notification has finished.  The notification is a
	''' beans event notification which does not allow any further
	''' mutations until all listeners have been notified.
	''' <p>
	''' Any models subclassed from this class and used in conjunction
	''' with a text component that has a look and feel implementation
	''' that is derived from BasicTextUI may be safely updated
	''' asynchronously, because all access to the View hierarchy
	''' is serialized by BasicTextUI if the document is of type
	''' <code>AbstractDocument</code>.  The locking assumes that an
	''' independent thread will access the View hierarchy only from
	''' the DocumentListener methods, and that there will be only
	''' one event thread active at a time.
	''' <p>
	''' If concurrency support is desired, there are the following
	''' additional implications.  The code path for any DocumentListener
	''' implementation and any UndoListener implementation must be threadsafe,
	''' and not access the component lock if trying to be safe from deadlocks.
	''' The <code>repaint</code> and <code>revalidate</code> methods
	''' on JComponent are safe.
	''' <p>
	''' AbstractDocument models an implied break at the end of the document.
	''' Among other things this allows you to position the caret after the last
	''' character. As a result of this, <code>getLength</code> returns one less
	''' than the length of the Content. If you create your own Content, be
	''' sure and initialize it to have an additional character. Refer to
	''' StringContent and GapContent for examples of this. Another implication
	''' of this is that Elements that model the implied end character will have
	''' an endOffset == (getLength() + 1). For example, in DefaultStyledDocument
	''' <code>getParagraphElement(getLength()).getEndOffset() == getLength() + 1
	''' </code>.
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
	''' @author  Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public MustInherit Class AbstractDocument
		Implements Document

		''' <summary>
		''' Constructs a new <code>AbstractDocument</code>, wrapped around some
		''' specified content storage mechanism.
		''' </summary>
		''' <param name="data"> the content </param>
		Protected Friend Sub New(ByVal data As Content)
			Me.New(data, StyleContext.defaultStyleContext)
		End Sub

		''' <summary>
		''' Constructs a new <code>AbstractDocument</code>, wrapped around some
		''' specified content storage mechanism.
		''' </summary>
		''' <param name="data"> the content </param>
		''' <param name="context"> the attribute context </param>
		Protected Friend Sub New(ByVal data As Content, ByVal context As AttributeContext)
			Me.data = data
			Me.context = context
			bidiRoot = New BidiRootElement(Me)

			If defaultI18NProperty Is Nothing Then
				' determine default setting for i18n support
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				String o = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'			{
	'					public String run()
	'					{
	'						Return System.getProperty(I18NProperty);
	'					}
	'				}
			   )
				If o IsNot Nothing Then
					defaultI18NProperty = Convert.ToBoolean(o)
				Else
					defaultI18NProperty = Boolean.FALSE
				End If
			End If
			putProperty(I18NProperty, defaultI18NProperty)

			'REMIND(bcb) This creates an initial bidi element to account for
			'the \n that exists by default in the content.  Doing it this way
			'seems to expose a little too much knowledge of the content given
			'to us by the sub-class.  Consider having the sub-class' constructor
			'make an initial call to insertUpdate.
			writeLock()
			Try
				Dim p As Element() = New Element(0){}
				p(0) = New BidiElement(Me, bidiRoot, 0, 1, 0)
				bidiRoot.replace(0,0,p)
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Supports managing a set of properties. Callers
		''' can use the <code>documentProperties</code> dictionary
		''' to annotate the document with document-wide properties.
		''' </summary>
		''' <returns> a non-<code>null</code> <code>Dictionary</code> </returns>
		''' <seealso cref= #setDocumentProperties </seealso>
		Public Overridable Property documentProperties As Dictionary(Of Object, Object)
			Get
				If documentProperties Is Nothing Then documentProperties = New Dictionary(Of Object, Object)(2)
				Return documentProperties
			End Get
			Set(ByVal x As Dictionary(Of Object, Object))
				documentProperties = x
			End Set
		End Property


		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireInsertUpdate(ByVal e As DocumentEvent)
			notifyingListeners = True
			Try
				' Guaranteed to return a non-null array
				Dim ___listeners As Object() = listenerList.listenerList
				' Process the listeners last to first, notifying
				' those that are interested in this event
				For i As Integer = ___listeners.Length-2 To 0 Step -2
					If ___listeners(i) Is GetType(DocumentListener) Then CType(___listeners(i+1), DocumentListener).insertUpdate(e)
				Next i
			Finally
				notifyingListeners = False
			End Try
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireChangedUpdate(ByVal e As DocumentEvent)
			notifyingListeners = True
			Try
				' Guaranteed to return a non-null array
				Dim ___listeners As Object() = listenerList.listenerList
				' Process the listeners last to first, notifying
				' those that are interested in this event
				For i As Integer = ___listeners.Length-2 To 0 Step -2
					If ___listeners(i) Is GetType(DocumentListener) Then CType(___listeners(i+1), DocumentListener).changedUpdate(e)
				Next i
			Finally
				notifyingListeners = False
			End Try
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireRemoveUpdate(ByVal e As DocumentEvent)
			notifyingListeners = True
			Try
				' Guaranteed to return a non-null array
				Dim ___listeners As Object() = listenerList.listenerList
				' Process the listeners last to first, notifying
				' those that are interested in this event
				For i As Integer = ___listeners.Length-2 To 0 Step -2
					If ___listeners(i) Is GetType(DocumentListener) Then CType(___listeners(i+1), DocumentListener).removeUpdate(e)
				Next i
			Finally
				notifyingListeners = False
			End Try
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireUndoableEditUpdate(ByVal e As UndoableEditEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(UndoableEditListener) Then CType(___listeners(i+1), UndoableEditListener).undoableEditHappened(e)
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this document.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' document <code>d</code>
		''' for its document listeners with the following code:
		''' 
		''' <pre>DocumentListener[] mls = (DocumentListener[])(d.getListeners(DocumentListener.class));</pre>
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
		''' <seealso cref= #getDocumentListeners </seealso>
		''' <seealso cref= #getUndoableEditListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		''' <summary>
		''' Gets the asynchronous loading priority.  If less than zero,
		''' the document should not be loaded asynchronously.
		''' </summary>
		''' <returns> the asynchronous loading priority, or <code>-1</code>
		'''   if the document should not be loaded asynchronously </returns>
		Public Overridable Property asynchronousLoadPriority As Integer
			Get
				Dim loadPriority As Integer? = CInt(Fix(getProperty(AbstractDocument.AsyncLoadPriority)))
				If loadPriority IsNot Nothing Then Return loadPriority
				Return -1
			End Get
			Set(ByVal p As Integer)
				Dim loadPriority As Integer? = If(p >= 0, Convert.ToInt32(p), Nothing)
				putProperty(AbstractDocument.AsyncLoadPriority, loadPriority)
			End Set
		End Property


		''' <summary>
		''' Sets the <code>DocumentFilter</code>. The <code>DocumentFilter</code>
		''' is passed <code>insert</code> and <code>remove</code> to conditionally
		''' allow inserting/deleting of the text.  A <code>null</code> value
		''' indicates that no filtering will occur.
		''' </summary>
		''' <param name="filter"> the <code>DocumentFilter</code> used to constrain text </param>
		''' <seealso cref= #getDocumentFilter
		''' @since 1.4 </seealso>
		Public Overridable Property documentFilter As DocumentFilter
			Set(ByVal filter As DocumentFilter)
				documentFilter = filter
			End Set
			Get
				Return documentFilter
			End Get
		End Property


		' --- Document methods -----------------------------------------

		''' <summary>
		''' This allows the model to be safely rendered in the presence
		''' of currency, if the model supports being updated asynchronously.
		''' The given runnable will be executed in a way that allows it
		''' to safely read the model with no changes while the runnable
		''' is being executed.  The runnable itself may <em>not</em>
		''' make any mutations.
		''' <p>
		''' This is implemented to acquire a read lock for the duration
		''' of the runnables execution.  There may be multiple runnables
		''' executing at the same time, and all writers will be blocked
		''' while there are active rendering runnables.  If the runnable
		''' throws an exception, its lock will be safely released.
		''' There is no protection against a runnable that never exits,
		''' which will effectively leave the document locked for it's
		''' lifetime.
		''' <p>
		''' If the given runnable attempts to make any mutations in
		''' this implementation, a deadlock will occur.  There is
		''' no tracking of individual rendering threads to enable
		''' detecting this situation, but a subclass could incur
		''' the overhead of tracking them and throwing an error.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="r"> the renderer to execute </param>
		Public Overridable Sub render(ByVal r As Runnable) Implements Document.render
			readLock()
			Try
				r.run()
			Finally
				readUnlock()
			End Try
		End Sub

		''' <summary>
		''' Returns the length of the data.  This is the number of
		''' characters of content that represents the users data.
		''' </summary>
		''' <returns> the length &gt;= 0 </returns>
		''' <seealso cref= Document#getLength </seealso>
		Public Overridable Property length As Integer Implements Document.getLength
			Get
				Return data.length() - 1
			End Get
		End Property

		''' <summary>
		''' Adds a document listener for notification of any changes.
		''' </summary>
		''' <param name="listener"> the <code>DocumentListener</code> to add </param>
		''' <seealso cref= Document#addDocumentListener </seealso>
		Public Overridable Sub addDocumentListener(ByVal listener As DocumentListener) Implements Document.addDocumentListener
			listenerList.add(GetType(DocumentListener), listener)
		End Sub

		''' <summary>
		''' Removes a document listener.
		''' </summary>
		''' <param name="listener"> the <code>DocumentListener</code> to remove </param>
		''' <seealso cref= Document#removeDocumentListener </seealso>
		Public Overridable Sub removeDocumentListener(ByVal listener As DocumentListener) Implements Document.removeDocumentListener
			listenerList.remove(GetType(DocumentListener), listener)
		End Sub

		''' <summary>
		''' Returns an array of all the document listeners
		''' registered on this document.
		''' </summary>
		''' <returns> all of this document's <code>DocumentListener</code>s
		'''         or an empty array if no document listeners are
		'''         currently registered
		''' </returns>
		''' <seealso cref= #addDocumentListener </seealso>
		''' <seealso cref= #removeDocumentListener
		''' @since 1.4 </seealso>
		Public Overridable Property documentListeners As DocumentListener()
			Get
				Return listenerList.getListeners(GetType(DocumentListener))
			End Get
		End Property

		''' <summary>
		''' Adds an undo listener for notification of any changes.
		''' Undo/Redo operations performed on the <code>UndoableEdit</code>
		''' will cause the appropriate DocumentEvent to be fired to keep
		''' the view(s) in sync with the model.
		''' </summary>
		''' <param name="listener"> the <code>UndoableEditListener</code> to add </param>
		''' <seealso cref= Document#addUndoableEditListener </seealso>
		Public Overridable Sub addUndoableEditListener(ByVal listener As UndoableEditListener) Implements Document.addUndoableEditListener
			listenerList.add(GetType(UndoableEditListener), listener)
		End Sub

		''' <summary>
		''' Removes an undo listener.
		''' </summary>
		''' <param name="listener"> the <code>UndoableEditListener</code> to remove </param>
		''' <seealso cref= Document#removeDocumentListener </seealso>
		Public Overridable Sub removeUndoableEditListener(ByVal listener As UndoableEditListener) Implements Document.removeUndoableEditListener
			listenerList.remove(GetType(UndoableEditListener), listener)
		End Sub

		''' <summary>
		''' Returns an array of all the undoable edit listeners
		''' registered on this document.
		''' </summary>
		''' <returns> all of this document's <code>UndoableEditListener</code>s
		'''         or an empty array if no undoable edit listeners are
		'''         currently registered
		''' </returns>
		''' <seealso cref= #addUndoableEditListener </seealso>
		''' <seealso cref= #removeUndoableEditListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property undoableEditListeners As UndoableEditListener()
			Get
				Return listenerList.getListeners(GetType(UndoableEditListener))
			End Get
		End Property

		''' <summary>
		''' A convenience method for looking up a property value. It is
		''' equivalent to:
		''' <pre>
		''' getDocumentProperties().get(key);
		''' </pre>
		''' </summary>
		''' <param name="key"> the non-<code>null</code> property key </param>
		''' <returns> the value of this property or <code>null</code> </returns>
		''' <seealso cref= #getDocumentProperties </seealso>
		Public Function getProperty(ByVal key As Object) As Object Implements Document.getProperty
			Return documentProperties.get(key)
		End Function


		''' <summary>
		''' A convenience method for storing up a property value.  It is
		''' equivalent to:
		''' <pre>
		''' getDocumentProperties().put(key, value);
		''' </pre>
		''' If <code>value</code> is <code>null</code> this method will
		''' remove the property.
		''' </summary>
		''' <param name="key"> the non-<code>null</code> key </param>
		''' <param name="value"> the property value </param>
		''' <seealso cref= #getDocumentProperties </seealso>
		Public Sub putProperty(ByVal key As Object, ByVal value As Object) Implements Document.putProperty
			If value IsNot Nothing Then
				documentProperties.put(key, value)
			Else
				documentProperties.remove(key)
			End If
			If key Is java.awt.font.TextAttribute.RUN_DIRECTION AndAlso Boolean.TRUE.Equals(getProperty(I18NProperty)) Then
				'REMIND - this needs to flip on the i18n property if run dir
				'is rtl and the i18n property is not already on.
				writeLock()
				Try
					Dim e As New DefaultDocumentEvent(Me, 0, length, DocumentEvent.EventType.INSERT)
					updateBidi(e)
				Finally
					writeUnlock()
				End Try
			End If
		End Sub

		''' <summary>
		''' Removes some content from the document.
		''' Removing content causes a write lock to be held while the
		''' actual changes are taking place.  Observers are notified
		''' of the change on the thread that called this method.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offs"> the starting offset &gt;= 0 </param>
		''' <param name="len"> the number of characters to remove &gt;= 0 </param>
		''' <exception cref="BadLocationException">  the given remove position is not a valid
		'''   position within the document </exception>
		''' <seealso cref= Document#remove </seealso>
		Public Overridable Sub remove(ByVal offs As Integer, ByVal len As Integer) Implements Document.remove
			Dim filter As DocumentFilter = documentFilter

			writeLock()
			Try
				If filter IsNot Nothing Then
					filter.remove(filterBypass, offs, len)
				Else
					handleRemove(offs, len)
				End If
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Performs the actual work of the remove. It is assumed the caller
		''' will have obtained a <code>writeLock</code> before invoking this.
		''' </summary>
		Friend Overridable Sub handleRemove(ByVal offs As Integer, ByVal len As Integer)
			If len > 0 Then
				If offs < 0 OrElse (offs + len) > length Then Throw New BadLocationException("Invalid remove", length + 1)
				Dim chng As New DefaultDocumentEvent(Me, offs, len, DocumentEvent.EventType.REMOVE)

				Dim isComposedTextElement As Boolean
				' Check whether the position of interest is the composed text
				isComposedTextElement = Utilities.isComposedTextElement(Me, offs)

				removeUpdate(chng)
				Dim u As UndoableEdit = data.remove(offs, len)
				If u IsNot Nothing Then chng.addEdit(u)
				postRemoveUpdate(chng)
				' Mark the edit as done.
				chng.end()
				fireRemoveUpdate(chng)
				' only fire undo if Content implementation supports it
				' undo for the composed text is not supported for now
				If (u IsNot Nothing) AndAlso (Not isComposedTextElement) Then fireUndoableEditUpdate(New UndoableEditEvent(Me, chng))
			End If
		End Sub

		''' <summary>
		''' Deletes the region of text from <code>offset</code> to
		''' <code>offset + length</code>, and replaces it with <code>text</code>.
		''' It is up to the implementation as to how this is implemented, some
		''' implementations may treat this as two distinct operations: a remove
		''' followed by an insert, others may treat the replace as one atomic
		''' operation.
		''' </summary>
		''' <param name="offset"> index of child element </param>
		''' <param name="length"> length of text to delete, may be 0 indicating don't
		'''               delete anything </param>
		''' <param name="text"> text to insert, <code>null</code> indicates no text to insert </param>
		''' <param name="attrs"> AttributeSet indicating attributes of inserted text,
		'''              <code>null</code>
		'''              is legal, and typically treated as an empty attributeset,
		'''              but exact interpretation is left to the subclass </param>
		''' <exception cref="BadLocationException"> the given position is not a valid
		'''            position within the document
		''' @since 1.4 </exception>
		Public Overridable Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet)
			If length = 0 AndAlso (text Is Nothing OrElse text.Length = 0) Then Return
			Dim filter As DocumentFilter = documentFilter

			writeLock()
			Try
				If filter IsNot Nothing Then
					filter.replace(filterBypass, offset, length, text, attrs)
				Else
					If length > 0 Then remove(offset, length)
					If text IsNot Nothing AndAlso text.Length > 0 Then insertString(offset, text, attrs)
				End If
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Inserts some content into the document.
		''' Inserting content causes a write lock to be held while the
		''' actual changes are taking place, followed by notification
		''' to the observers on the thread that grabbed the write lock.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offs"> the starting offset &gt;= 0 </param>
		''' <param name="str"> the string to insert; does nothing with null/empty strings </param>
		''' <param name="a"> the attributes for the inserted content </param>
		''' <exception cref="BadLocationException">  the given insert position is not a valid
		'''   position within the document </exception>
		''' <seealso cref= Document#insertString </seealso>
		Public Overridable Sub insertString(ByVal offs As Integer, ByVal str As String, ByVal a As AttributeSet) Implements Document.insertString
			If (str Is Nothing) OrElse (str.Length = 0) Then Return
			Dim filter As DocumentFilter = documentFilter

			writeLock()

			Try
				If filter IsNot Nothing Then
					filter.insertString(filterBypass, offs, str, a)
				Else
					handleInsertString(offs, str, a)
				End If
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Performs the actual work of inserting the text; it is assumed the
		''' caller has obtained a write lock before invoking this.
		''' </summary>
		Private Sub handleInsertString(ByVal offs As Integer, ByVal str As String, ByVal a As AttributeSet)
			If (str Is Nothing) OrElse (str.Length = 0) Then Return
			Dim u As UndoableEdit = data.insertString(offs, str)
			Dim e As New DefaultDocumentEvent(Me, offs, str.Length, DocumentEvent.EventType.INSERT)
			If u IsNot Nothing Then e.addEdit(u)

			' see if complex glyph layout support is needed
			If getProperty(I18NProperty).Equals(Boolean.FALSE) Then
				' if a default direction of right-to-left has been specified,
				' we want complex layout even if the text is all left to right.
				Dim d As Object = getProperty(java.awt.font.TextAttribute.RUN_DIRECTION)
				If (d IsNot Nothing) AndAlso (d.Equals(java.awt.font.TextAttribute.RUN_DIRECTION_RTL)) Then
					putProperty(I18NProperty, Boolean.TRUE)
				Else
					Dim chars As Char() = str.ToCharArray()
					If sun.swing.SwingUtilities2.isComplexLayout(chars, 0, chars.Length) Then putProperty(I18NProperty, Boolean.TRUE)
				End If
			End If

			insertUpdate(e, a)
			' Mark the edit as done.
			e.end()
			fireInsertUpdate(e)
			' only fire undo if Content implementation supports it
			' undo for the composed text is not supported for now
			If u IsNot Nothing AndAlso (a Is Nothing OrElse (Not a.isDefined(StyleConstants.ComposedTextAttribute))) Then fireUndoableEditUpdate(New UndoableEditEvent(Me, e))
		End Sub

		''' <summary>
		''' Gets a sequence of text from the document.
		''' </summary>
		''' <param name="offset"> the starting offset &gt;= 0 </param>
		''' <param name="length"> the number of characters to retrieve &gt;= 0 </param>
		''' <returns> the text </returns>
		''' <exception cref="BadLocationException">  the range given includes a position
		'''   that is not a valid position within the document </exception>
		''' <seealso cref= Document#getText </seealso>
		Public Overridable Function getText(ByVal offset As Integer, ByVal length As Integer) As String Implements Document.getText
			If length < 0 Then Throw New BadLocationException("Length must be positive", length)
			Dim str As String = data.getString(offset, length)
			Return str
		End Function

		''' <summary>
		''' Fetches the text contained within the given portion
		''' of the document.
		''' <p>
		''' If the partialReturn property on the txt parameter is false, the
		''' data returned in the Segment will be the entire length requested and
		''' may or may not be a copy depending upon how the data was stored.
		''' If the partialReturn property is true, only the amount of text that
		''' can be returned without creating a copy is returned.  Using partial
		''' returns will give better performance for situations where large
		''' parts of the document are being scanned.  The following is an example
		''' of using the partial return to access the entire document:
		''' 
		''' <pre>
		''' &nbsp; int nleft = doc.getDocumentLength();
		''' &nbsp; Segment text = new Segment();
		''' &nbsp; int offs = 0;
		''' &nbsp; text.setPartialReturn(true);
		''' &nbsp; while (nleft &gt; 0) {
		''' &nbsp;     doc.getText(offs, nleft, text);
		''' &nbsp;     // do something with text
		''' &nbsp;     nleft -= text.count;
		''' &nbsp;     offs += text.count;
		''' &nbsp; }
		''' </pre>
		''' </summary>
		''' <param name="offset"> the starting offset &gt;= 0 </param>
		''' <param name="length"> the number of characters to retrieve &gt;= 0 </param>
		''' <param name="txt"> the Segment object to retrieve the text into </param>
		''' <exception cref="BadLocationException">  the range given includes a position
		'''   that is not a valid position within the document </exception>
		Public Overridable Sub getText(ByVal offset As Integer, ByVal length As Integer, ByVal txt As Segment) Implements Document.getText
			If length < 0 Then Throw New BadLocationException("Length must be positive", length)
			data.getChars(offset, length, txt)
		End Sub

		''' <summary>
		''' Returns a position that will track change as the document
		''' is altered.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offs"> the position in the model &gt;= 0 </param>
		''' <returns> the position </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= Document#createPosition </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function createPosition(ByVal offs As Integer) As Position Implements Document.createPosition
			Return data.createPosition(offs)
		End Function

		''' <summary>
		''' Returns a position that represents the start of the document.  The
		''' position returned can be counted on to track change and stay
		''' located at the beginning of the document.
		''' </summary>
		''' <returns> the position </returns>
		Public Property startPosition As Position Implements Document.getStartPosition
			Get
				Dim p As Position
				Try
					p = createPosition(0)
				Catch bl As BadLocationException
					p = Nothing
				End Try
				Return p
			End Get
		End Property

		''' <summary>
		''' Returns a position that represents the end of the document.  The
		''' position returned can be counted on to track change and stay
		''' located at the end of the document.
		''' </summary>
		''' <returns> the position </returns>
		Public Property endPosition As Position Implements Document.getEndPosition
			Get
				Dim p As Position
				Try
					p = createPosition(data.length())
				Catch bl As BadLocationException
					p = Nothing
				End Try
				Return p
			End Get
		End Property

		''' <summary>
		''' Gets all root elements defined.  Typically, there
		''' will only be one so the default implementation
		''' is to return the default root element.
		''' </summary>
		''' <returns> the root element </returns>
		Public Overridable Property rootElements As Element() Implements Document.getRootElements
			Get
				Dim elems As Element() = New Element(1){}
				elems(0) = defaultRootElement
				elems(1) = bidiRootElement
				Return elems
			End Get
		End Property

		''' <summary>
		''' Returns the root element that views should be based upon
		''' unless some other mechanism for assigning views to element
		''' structures is provided.
		''' </summary>
		''' <returns> the root element </returns>
		''' <seealso cref= Document#getDefaultRootElement </seealso>
		Public MustOverride ReadOnly Property defaultRootElement As Element Implements Document.getDefaultRootElement

		' ---- local methods -----------------------------------------

		''' <summary>
		''' Returns the <code>FilterBypass</code>. This will create one if one
		''' does not yet exist.
		''' </summary>
		Private Property filterBypass As DocumentFilter.FilterBypass
			Get
				If filterBypass Is Nothing Then filterBypass = New DefaultFilterBypass(Me)
				Return filterBypass
			End Get
		End Property

		''' <summary>
		''' Returns the root element of the bidirectional structure for this
		''' document.  Its children represent character runs with a given
		''' Unicode bidi level.
		''' </summary>
		Public Overridable Property bidiRootElement As Element
			Get
				Return bidiRoot
			End Get
		End Property

		''' <summary>
		''' Returns true if the text in the range <code>p0</code> to
		''' <code>p1</code> is left to right.
		''' </summary>
		Friend Shared Function isLeftToRight(ByVal doc As Document, ByVal p0 As Integer, ByVal p1 As Integer) As Boolean
			If Boolean.TRUE.Equals(doc.getProperty(I18NProperty)) Then
				If TypeOf doc Is AbstractDocument Then
					Dim adoc As AbstractDocument = CType(doc, AbstractDocument)
					Dim bidiRoot As Element = adoc.bidiRootElement
					Dim index As Integer = bidiRoot.getElementIndex(p0)
					Dim bidiElem As Element = bidiRoot.getElement(index)
					If bidiElem.endOffset >= p1 Then
						Dim bidiAttrs As AttributeSet = bidiElem.attributes
						Return ((StyleConstants.getBidiLevel(bidiAttrs) Mod 2) = 0)
					End If
				End If
			End If
			Return True
		End Function

		''' <summary>
		''' Get the paragraph element containing the given position.  Sub-classes
		''' must define for themselves what exactly constitutes a paragraph.  They
		''' should keep in mind however that a paragraph should at least be the
		''' unit of text over which to run the Unicode bidirectional algorithm.
		''' </summary>
		''' <param name="pos"> the starting offset &gt;= 0 </param>
		''' <returns> the element  </returns>
		Public MustOverride Function getParagraphElement(ByVal pos As Integer) As Element


		''' <summary>
		''' Fetches the context for managing attributes.  This
		''' method effectively establishes the strategy used
		''' for compressing AttributeSet information.
		''' </summary>
		''' <returns> the context </returns>
		Protected Friend Property attributeContext As AttributeContext
			Get
				Return context
			End Get
		End Property

		''' <summary>
		''' Updates document structure as a result of text insertion.  This
		''' will happen within a write lock.  If a subclass of
		''' this class reimplements this method, it should delegate to the
		''' superclass as well.
		''' </summary>
		''' <param name="chng"> a description of the change </param>
		''' <param name="attr"> the attributes for the change </param>
		Protected Friend Overridable Sub insertUpdate(ByVal chng As DefaultDocumentEvent, ByVal attr As AttributeSet)
			If getProperty(I18NProperty).Equals(Boolean.TRUE) Then updateBidi(chng)

			' Check if a multi byte is encountered in the inserted text.
			If chng.type = DocumentEvent.EventType.INSERT AndAlso chng.length > 0 AndAlso (Not Boolean.TRUE.Equals(getProperty(MultiByteProperty))) Then
				Dim segment As Segment = SegmentCache.sharedSegment
				Try
					getText(chng.offset, chng.length, segment)
					segment.first()
					Do
						If AscW(segment.current()) > 255 Then
							putProperty(MultiByteProperty, Boolean.TRUE)
							Exit Do
						End If
					Loop While segment.next() <> Segment.DONE
				Catch ble As BadLocationException
					' Should never happen
				End Try
				SegmentCache.releaseSharedSegment(segment)
			End If
		End Sub

		''' <summary>
		''' Updates any document structure as a result of text removal.  This
		''' method is called before the text is actually removed from the Content.
		''' This will happen within a write lock. If a subclass
		''' of this class reimplements this method, it should delegate to the
		''' superclass as well.
		''' </summary>
		''' <param name="chng"> a description of the change </param>
		Protected Friend Overridable Sub removeUpdate(ByVal chng As DefaultDocumentEvent)
		End Sub

		''' <summary>
		''' Updates any document structure as a result of text removal.  This
		''' method is called after the text has been removed from the Content.
		''' This will happen within a write lock. If a subclass
		''' of this class reimplements this method, it should delegate to the
		''' superclass as well.
		''' </summary>
		''' <param name="chng"> a description of the change </param>
		Protected Friend Overridable Sub postRemoveUpdate(ByVal chng As DefaultDocumentEvent)
			If getProperty(I18NProperty).Equals(Boolean.TRUE) Then updateBidi(chng)
		End Sub


		''' <summary>
		''' Update the bidi element structure as a result of the given change
		''' to the document.  The given change will be updated to reflect the
		''' changes made to the bidi structure.
		''' 
		''' This method assumes that every offset in the model is contained in
		''' exactly one paragraph.  This method also assumes that it is called
		''' after the change is made to the default element structure.
		''' </summary>
		Friend Overridable Sub updateBidi(ByVal chng As DefaultDocumentEvent)

			' Calculate the range of paragraphs affected by the change.
			Dim firstPStart As Integer
			Dim lastPEnd As Integer
			If chng.type = DocumentEvent.EventType.INSERT OrElse chng.type = DocumentEvent.EventType.CHANGE Then
				Dim chngStart As Integer = chng.offset
				Dim chngEnd As Integer = chngStart + chng.length
				firstPStart = getParagraphElement(chngStart).startOffset
				lastPEnd = getParagraphElement(chngEnd).endOffset
			ElseIf chng.type = DocumentEvent.EventType.REMOVE Then
				Dim paragraph As Element = getParagraphElement(chng.offset)
				firstPStart = paragraph.startOffset
				lastPEnd = paragraph.endOffset
			Else
				Throw New Exception("Internal error: unknown event type.")
			End If
			'System.out.println("updateBidi: firstPStart = " + firstPStart + " lastPEnd = " + lastPEnd );


			' Calculate the bidi levels for the affected range of paragraphs.  The
			' levels array will contain a bidi level for each character in the
			' affected text.
			Dim levels As SByte() = calculateBidiLevels(firstPStart, lastPEnd)


			Dim newElements As New List(Of Element)

			' Calculate the first span of characters in the affected range with
			' the same bidi level.  If this level is the same as the level of the
			' previous bidi element (the existing bidi element containing
			' firstPStart-1), then merge in the previous element.  If not, but
			' the previous element overlaps the affected range, truncate the
			' previous element at firstPStart.
			Dim firstSpanStart As Integer = firstPStart
			Dim removeFromIndex As Integer = 0
			If firstSpanStart > 0 Then
				Dim prevElemIndex As Integer = bidiRoot.getElementIndex(firstPStart-1)
				removeFromIndex = prevElemIndex
				Dim prevElem As Element = bidiRoot.getElement(prevElemIndex)
				Dim prevLevel As Integer=StyleConstants.getBidiLevel(prevElem.attributes)
				'System.out.println("createbidiElements: prevElem= " + prevElem  + " prevLevel= " + prevLevel + "level[0] = " + levels[0]);
				If prevLevel=levels(0) Then
					firstSpanStart = prevElem.startOffset
				ElseIf prevElem.endOffset > firstPStart Then
					newElements.Add(New BidiElement(Me, bidiRoot, prevElem.startOffset, firstPStart, prevLevel))
				Else
					removeFromIndex += 1
				End If
			End If

			Dim firstSpanEnd As Integer = 0
			Do While (firstSpanEnd<levels.Length) AndAlso (levels(firstSpanEnd)=levels(0))
				firstSpanEnd += 1
			Loop


			' Calculate the last span of characters in the affected range with
			' the same bidi level.  If this level is the same as the level of the
			' next bidi element (the existing bidi element containing lastPEnd),
			' then merge in the next element.  If not, but the next element
			' overlaps the affected range, adjust the next element to start at
			' lastPEnd.
			Dim lastSpanEnd As Integer = lastPEnd
			Dim newNextElem As Element = Nothing
			Dim removeToIndex As Integer = bidiRoot.elementCount - 1
			If lastSpanEnd <= length Then
				Dim nextElemIndex As Integer = bidiRoot.getElementIndex(lastPEnd)
				removeToIndex = nextElemIndex
				Dim nextElem As Element = bidiRoot.getElement(nextElemIndex)
				Dim nextLevel As Integer = StyleConstants.getBidiLevel(nextElem.attributes)
				If nextLevel = levels(levels.Length-1) Then
					lastSpanEnd = nextElem.endOffset
				ElseIf nextElem.startOffset < lastPEnd Then
					newNextElem = New BidiElement(Me, bidiRoot, lastPEnd, nextElem.endOffset, nextLevel)
				Else
					removeToIndex -= 1
				End If
			End If

			Dim lastSpanStart As Integer = levels.Length
			Do While (lastSpanStart>firstSpanEnd) AndAlso (levels(lastSpanStart-1)=levels(levels.Length-1))
				lastSpanStart -= 1
			Loop


			' If the first and last spans are contiguous and have the same level,
			' merge them and create a single new element for the entire span.
			' Otherwise, create elements for the first and last spans as well as
			' any spans in between.
			If (firstSpanEnd=lastSpanStart) AndAlso (levels(0)=levels(levels.Length-1)) Then
				newElements.Add(New BidiElement(Me, bidiRoot, firstSpanStart, lastSpanEnd, levels(0)))
			Else
				' Create an element for the first span.
				newElements.Add(New BidiElement(Me, bidiRoot, firstSpanStart, firstSpanEnd+firstPStart, levels(0)))
				' Create elements for the spans in between the first and last
				Dim i As Integer=firstSpanEnd
				Do While i<lastSpanStart
					'System.out.println("executed line 872");
					Dim j As Integer
					j=i
					Do While (j<levels.Length) AndAlso (levels(j) = levels(i))

						j += 1
					Loop
					newElements.Add(New BidiElement(Me, bidiRoot, firstPStart+i, firstPStart+j, CInt(levels(i))))
					i=j
				Loop
				' Create an element for the last span.
				newElements.Add(New BidiElement(Me, bidiRoot, lastSpanStart+firstPStart, lastSpanEnd, levels(levels.Length-1)))
			End If

			If newNextElem IsNot Nothing Then newElements.Add(newNextElem)


			' Calculate the set of existing bidi elements which must be
			' removed.
			Dim removedElemCount As Integer = 0
			If bidiRoot.elementCount > 0 Then removedElemCount = removeToIndex - removeFromIndex + 1
			Dim removedElems As Element() = New Element(removedElemCount - 1){}
			For i As Integer = 0 To removedElemCount - 1
				removedElems(i) = bidiRoot.getElement(removeFromIndex+i)
			Next i

			Dim addedElems As Element() = New Element(newElements.Count - 1){}
			newElements.CopyTo(addedElems)

			' Update the change record.
			Dim ee As New ElementEdit(bidiRoot, removeFromIndex, removedElems, addedElems)
			chng.addEdit(ee)

			' Update the bidi element structure.
			bidiRoot.replace(removeFromIndex, removedElems.Length, addedElems)
		End Sub


		''' <summary>
		''' Calculate the levels array for a range of paragraphs.
		''' </summary>
		Private Function calculateBidiLevels(ByVal firstPStart As Integer, ByVal lastPEnd As Integer) As SByte()

			Dim levels As SByte() = New SByte(lastPEnd - firstPStart - 1){}
			Dim levelsEnd As Integer = 0
			Dim defaultDirection As Boolean? = Nothing
			Dim d As Object = getProperty(java.awt.font.TextAttribute.RUN_DIRECTION)
			If TypeOf d Is Boolean? Then defaultDirection = CBool(d)

			' For each paragraph in the given range of paragraphs, get its
			' levels array and add it to the levels array for the entire span.
			Dim o As Integer=firstPStart
			Do While o<lastPEnd
				Dim p As Element = getParagraphElement(o)
				Dim pStart As Integer = p.startOffset
				Dim pEnd As Integer = p.endOffset

				' default run direction for the paragraph.  This will be
				' null if there is no direction override specified (i.e.
				' the direction will be determined from the content).
				Dim direction As Boolean? = defaultDirection
				d = p.attributes.getAttribute(java.awt.font.TextAttribute.RUN_DIRECTION)
				If TypeOf d Is Boolean? Then direction = CBool(d)

				'System.out.println("updateBidi: paragraph start = " + pStart + " paragraph end = " + pEnd);

				' Create a Bidi over this paragraph then get the level
				' array.
				Dim seg As Segment = SegmentCache.sharedSegment
				Try
					getText(pStart, pEnd-pStart, seg)
				Catch e As BadLocationException
					Throw New Exception("Internal error: " & e.ToString())
				End Try
				' REMIND(bcb) we should really be using a Segment here.
				Dim bidiAnalyzer As java.text.Bidi
				Dim bidiflag As Integer = java.text.Bidi.DIRECTION_DEFAULT_LEFT_TO_RIGHT
				If direction IsNot Nothing Then
					If java.awt.font.TextAttribute.RUN_DIRECTION_LTR.Equals(direction) Then
						bidiflag = java.text.Bidi.DIRECTION_LEFT_TO_RIGHT
					Else
						bidiflag = java.text.Bidi.DIRECTION_RIGHT_TO_LEFT
					End If
				End If
				bidiAnalyzer = New java.text.Bidi(seg.array, seg.offset, Nothing, 0, seg.count, bidiflag)
				sun.font.BidiUtils.getLevels(bidiAnalyzer, levels, levelsEnd)
				levelsEnd += bidiAnalyzer.length

				o = p.endOffset
				SegmentCache.releaseSharedSegment(seg)
			Loop

			' REMIND(bcb) remove this code when debugging is done.
			If levelsEnd <> levels.Length Then Throw New Exception("levelsEnd assertion failed.")

			Return levels
		End Function

		''' <summary>
		''' Gives a diagnostic dump.
		''' </summary>
		''' <param name="out"> the output stream </param>
		Public Overridable Sub dump(ByVal out As PrintStream)
			Dim root As Element = defaultRootElement
			If TypeOf root Is AbstractElement Then CType(root, AbstractElement).dump(out, 0)
			bidiRoot.dump(out,0)
		End Sub

		''' <summary>
		''' Gets the content for the document.
		''' </summary>
		''' <returns> the content </returns>
		Protected Friend Property content As Content
			Get
				Return data
			End Get
		End Property

		''' <summary>
		''' Creates a document leaf element.
		''' Hook through which elements are created to represent the
		''' document structure.  Because this implementation keeps
		''' structure and content separate, elements grow automatically
		''' when content is extended so splits of existing elements
		''' follow.  The document itself gets to decide how to generate
		''' elements to give flexibility in the type of elements used.
		''' </summary>
		''' <param name="parent"> the parent element </param>
		''' <param name="a"> the attributes for the element </param>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		''' <returns> the new element </returns>
		Protected Friend Overridable Function createLeafElement(ByVal parent As Element, ByVal a As AttributeSet, ByVal p0 As Integer, ByVal p1 As Integer) As Element
			Return New LeafElement(Me, parent, a, p0, p1)
		End Function

		''' <summary>
		''' Creates a document branch element, that can contain other elements.
		''' </summary>
		''' <param name="parent"> the parent element </param>
		''' <param name="a"> the attributes </param>
		''' <returns> the element </returns>
		Protected Friend Overridable Function createBranchElement(ByVal parent As Element, ByVal a As AttributeSet) As Element
			Return New BranchElement(Me, parent, a)
		End Function

		' --- Document locking ----------------------------------

		''' <summary>
		''' Fetches the current writing thread if there is one.
		''' This can be used to distinguish whether a method is
		''' being called as part of an existing modification or
		''' if a lock needs to be acquired and a new transaction
		''' started.
		''' </summary>
		''' <returns> the thread actively modifying the document
		'''  or <code>null</code> if there are no modifications in progress </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Property currentWriter As Thread
			Get
				Return currWriter
			End Get
		End Property

		''' <summary>
		''' Acquires a lock to begin mutating the document this lock
		''' protects.  There can be no writing, notification of changes, or
		''' reading going on in order to gain the lock.  Additionally a thread is
		''' allowed to gain more than one <code>writeLock</code>,
		''' as long as it doesn't attempt to gain additional <code>writeLock</code>s
		''' from within document notification.  Attempting to gain a
		''' <code>writeLock</code> from within a DocumentListener notification will
		''' result in an <code>IllegalStateException</code>.  The ability
		''' to obtain more than one <code>writeLock</code> per thread allows
		''' subclasses to gain a writeLock, perform a number of operations, then
		''' release the lock.
		''' <p>
		''' Calls to <code>writeLock</code>
		''' must be balanced with calls to <code>writeUnlock</code>, else the
		''' <code>Document</code> will be left in a locked state so that no
		''' reading or writing can be done.
		''' </summary>
		''' <exception cref="IllegalStateException"> thrown on illegal lock
		'''  attempt.  If the document is implemented properly, this can
		'''  only happen if a document listener attempts to mutate the
		'''  document.  This situation violates the bean event model
		'''  where order of delivery is not guaranteed and all listeners
		'''  should be notified before further mutations are allowed. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Sub writeLock()
			Try
				Do While (numReaders > 0) OrElse (currWriter IsNot Nothing)
					If Thread.CurrentThread Is currWriter Then
						If notifyingListeners Then Throw New IllegalStateException("Attempt to mutate in notification")
						numWriters += 1
						Return
					End If
					wait()
				Loop
				currWriter = Thread.CurrentThread
				numWriters = 1
			Catch e As InterruptedException
				Throw New Exception("Interrupted attempt to acquire write lock")
			End Try
		End Sub

		''' <summary>
		''' Releases a write lock previously obtained via <code>writeLock</code>.
		''' After decrementing the lock count if there are no outstanding locks
		''' this will allow a new writer, or readers.
		''' </summary>
		''' <seealso cref= #writeLock </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Sub writeUnlock()
			numWriters -= 1
			If numWriters <= 0 Then
				numWriters = 0
				currWriter = Nothing
				notifyAll()
			End If
		End Sub

		''' <summary>
		''' Acquires a lock to begin reading some state from the
		''' document.  There can be multiple readers at the same time.
		''' Writing blocks the readers until notification of the change
		''' to the listeners has been completed.  This method should
		''' be used very carefully to avoid unintended compromise
		''' of the document.  It should always be balanced with a
		''' <code>readUnlock</code>.
		''' </summary>
		''' <seealso cref= #readUnlock </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub readLock()
			Try
				Do While currWriter IsNot Nothing
					If currWriter Is Thread.CurrentThread Then Return
					wait()
				Loop
				numReaders += 1
			Catch e As InterruptedException
				Throw New Exception("Interrupted attempt to acquire read lock")
			End Try
		End Sub

		''' <summary>
		''' Does a read unlock.  This signals that one
		''' of the readers is done.  If there are no more readers
		''' then writing can begin again.  This should be balanced
		''' with a readLock, and should occur in a finally statement
		''' so that the balance is guaranteed.  The following is an
		''' example.
		''' <pre><code>
		''' &nbsp;   readLock();
		''' &nbsp;   try {
		''' &nbsp;       // do something
		''' &nbsp;   } finally {
		''' &nbsp;       readUnlock();
		''' &nbsp;   }
		''' </code></pre>
		''' </summary>
		''' <seealso cref= #readLock </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub readUnlock()
			If currWriter Is Thread.CurrentThread Then Return
			If numReaders <= 0 Then Throw New StateInvariantError(BAD_LOCK_STATE)
			numReaders -= 1
			notify()
		End Sub

		' --- serialization ---------------------------------------------

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			listenerList = New EventListenerList

			' Restore bidi structure
			'REMIND(bcb) This creates an initial bidi element to account for
			'the \n that exists by default in the content.
			bidiRoot = New BidiRootElement(Me)
			Try
				writeLock()
				Dim p As Element() = New Element(0){}
				p(0) = New BidiElement(Me, bidiRoot, 0, 1, 0)
				bidiRoot.replace(0,0,p)
			Finally
				writeUnlock()
			End Try
			' At this point bidi root is only partially correct. To fully
			' restore it we need access to getDefaultRootElement. But, this
			' is created by the subclass and at this point will be null. We
			' thus use registerValidation.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			s.registerValidation(New ObjectInputValidation()
	'		{
	'			public void validateObject()
	'			{
	'				try
	'				{
	'					writeLock();
	'					DefaultDocumentEvent e = New DefaultDocumentEvent(0, getLength(), DocumentEvent.EventType.INSERT);
	'					updateBidi(e);
	'				}
	'				finally
	'				{
	'					writeUnlock();
	'				}
	'			}
	'		}, 0);
		End Sub

		' ----- member variables ------------------------------------------

		<NonSerialized> _
		Private numReaders As Integer
		<NonSerialized> _
		Private currWriter As Thread
		''' <summary>
		''' The number of writers, all obtained from <code>currWriter</code>.
		''' </summary>
		<NonSerialized> _
		Private numWriters As Integer
		''' <summary>
		''' True will notifying listeners.
		''' </summary>
		<NonSerialized> _
		Private notifyingListeners As Boolean

		Private Shared defaultI18NProperty As Boolean?

		''' <summary>
		''' Storage for document-wide properties.
		''' </summary>
		Private documentProperties As Dictionary(Of Object, Object) = Nothing

		''' <summary>
		''' The event listener list for the document.
		''' </summary>
		Protected Friend listenerList As New EventListenerList

		''' <summary>
		''' Where the text is actually stored, and a set of marks
		''' that track change as the document is edited are managed.
		''' </summary>
		Private data As Content

		''' <summary>
		''' Factory for the attributes.  This is the strategy for
		''' attribute compression and control of the lifetime of
		''' a set of attributes as a collection.  This may be shared
		''' with other documents.
		''' </summary>
		Private context As AttributeContext

		''' <summary>
		''' The root of the bidirectional structure for this document.  Its children
		''' represent character runs with the same Unicode bidi level.
		''' </summary>
		<NonSerialized> _
		Private bidiRoot As BranchElement

		''' <summary>
		''' Filter for inserting/removing of text.
		''' </summary>
		Private documentFilter As DocumentFilter

		''' <summary>
		''' Used by DocumentFilter to do actual insert/remove.
		''' </summary>
		<NonSerialized> _
		Private filterBypass As DocumentFilter.FilterBypass

		Private Const BAD_LOCK_STATE As String = "document lock failure"

		''' <summary>
		''' Error message to indicate a bad location.
		''' </summary>
		Protected Friend Const BAD_LOCATION As String = "document location failure"

		''' <summary>
		''' Name of elements used to represent paragraphs
		''' </summary>
		Public Const ParagraphElementName As String = "paragraph"

		''' <summary>
		''' Name of elements used to represent content
		''' </summary>
		Public Const ContentElementName As String = "content"

		''' <summary>
		''' Name of elements used to hold sections (lines/paragraphs).
		''' </summary>
		Public Const SectionElementName As String = "section"

		''' <summary>
		''' Name of elements used to hold a unidirectional run
		''' </summary>
		Public Const BidiElementName As String = "bidi level"

		''' <summary>
		''' Name of the attribute used to specify element
		''' names.
		''' </summary>
		Public Const ElementNameAttribute As String = "$ename"

		''' <summary>
		''' Document property that indicates whether internationalization
		''' functions such as text reordering or reshaping should be
		''' performed. This property should not be publicly exposed,
		''' since it is used for implementation convenience only.  As a
		''' side effect, copies of this property may be in its subclasses
		''' that live in different packages (e.g. HTMLDocument as of now),
		''' so those copies should also be taken care of when this property
		''' needs to be modified.
		''' </summary>
		Friend Const I18NProperty As String = "i18n"

		''' <summary>
		''' Document property that indicates if a character has been inserted
		''' into the document that is more than one byte long.  GlyphView uses
		''' this to determine if it should use BreakIterator.
		''' </summary>
		Friend Const MultiByteProperty As Object = "multiByte"

		''' <summary>
		''' Document property that indicates asynchronous loading is
		''' desired, with the thread priority given as the value.
		''' </summary>
		Friend Const AsyncLoadPriority As String = "load priority"

		''' <summary>
		''' Interface to describe a sequence of character content that
		''' can be edited.  Implementations may or may not support a
		''' history mechanism which will be reflected by whether or not
		''' mutations return an UndoableEdit implementation. </summary>
		''' <seealso cref= AbstractDocument </seealso>
		Public Interface Content

			''' <summary>
			''' Creates a position within the content that will
			''' track change as the content is mutated.
			''' </summary>
			''' <param name="offset"> the offset in the content &gt;= 0 </param>
			''' <returns> a Position </returns>
			''' <exception cref="BadLocationException"> for an invalid offset </exception>
			Function createPosition(ByVal offset As Integer) As Position

			''' <summary>
			''' Current length of the sequence of character content.
			''' </summary>
			''' <returns> the length &gt;= 0 </returns>
			Function length() As Integer

			''' <summary>
			''' Inserts a string of characters into the sequence.
			''' </summary>
			''' <param name="where">   offset into the sequence to make the insertion &gt;= 0 </param>
			''' <param name="str">     string to insert </param>
			''' <returns>  if the implementation supports a history mechanism,
			'''    a reference to an <code>Edit</code> implementation will be returned,
			'''    otherwise returns <code>null</code> </returns>
			''' <exception cref="BadLocationException">  thrown if the area covered by
			'''   the arguments is not contained in the character sequence </exception>
			Function insertString(ByVal where As Integer, ByVal str As String) As UndoableEdit

			''' <summary>
			''' Removes some portion of the sequence.
			''' </summary>
			''' <param name="where">   The offset into the sequence to make the
			'''   insertion &gt;= 0. </param>
			''' <param name="nitems">  The number of items in the sequence to remove &gt;= 0. </param>
			''' <returns>  If the implementation supports a history mechanism,
			'''    a reference to an Edit implementation will be returned,
			'''    otherwise null. </returns>
			''' <exception cref="BadLocationException">  Thrown if the area covered by
			'''   the arguments is not contained in the character sequence. </exception>
			Function remove(ByVal where As Integer, ByVal nitems As Integer) As UndoableEdit

			''' <summary>
			''' Fetches a string of characters contained in the sequence.
			''' </summary>
			''' <param name="where">   Offset into the sequence to fetch &gt;= 0. </param>
			''' <param name="len">     number of characters to copy &gt;= 0. </param>
			''' <returns> the string </returns>
			''' <exception cref="BadLocationException">  Thrown if the area covered by
			'''   the arguments is not contained in the character sequence. </exception>
			Function getString(ByVal where As Integer, ByVal len As Integer) As String

			''' <summary>
			''' Gets a sequence of characters and copies them into a Segment.
			''' </summary>
			''' <param name="where"> the starting offset &gt;= 0 </param>
			''' <param name="len"> the number of characters &gt;= 0 </param>
			''' <param name="txt"> the target location to copy into </param>
			''' <exception cref="BadLocationException">  Thrown if the area covered by
			'''   the arguments is not contained in the character sequence. </exception>
			Sub getChars(ByVal where As Integer, ByVal len As Integer, ByVal txt As Segment)
		End Interface

		''' <summary>
		''' An interface that can be used to allow MutableAttributeSet
		''' implementations to use pluggable attribute compression
		''' techniques.  Each mutation of the attribute set can be
		''' used to exchange a previous AttributeSet instance with
		''' another, preserving the possibility of the AttributeSet
		''' remaining immutable.  An implementation is provided by
		''' the StyleContext class.
		''' 
		''' The Element implementations provided by this class use
		''' this interface to provide their MutableAttributeSet
		''' implementations, so that different AttributeSet compression
		''' techniques can be employed.  The method
		''' <code>getAttributeContext</code> should be implemented to
		''' return the object responsible for implementing the desired
		''' compression technique.
		''' </summary>
		''' <seealso cref= StyleContext </seealso>
		Public Interface AttributeContext

			''' <summary>
			''' Adds an attribute to the given set, and returns
			''' the new representative set.
			''' </summary>
			''' <param name="old"> the old attribute set </param>
			''' <param name="name"> the non-null attribute name </param>
			''' <param name="value"> the attribute value </param>
			''' <returns> the updated attribute set </returns>
			''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
			Function addAttribute(ByVal old As AttributeSet, ByVal name As Object, ByVal value As Object) As AttributeSet

			''' <summary>
			''' Adds a set of attributes to the element.
			''' </summary>
			''' <param name="old"> the old attribute set </param>
			''' <param name="attr"> the attributes to add </param>
			''' <returns> the updated attribute set </returns>
			''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
			Function addAttributes(ByVal old As AttributeSet, ByVal attr As AttributeSet) As AttributeSet

			''' <summary>
			''' Removes an attribute from the set.
			''' </summary>
			''' <param name="old"> the old attribute set </param>
			''' <param name="name"> the non-null attribute name </param>
			''' <returns> the updated attribute set </returns>
			''' <seealso cref= MutableAttributeSet#removeAttribute </seealso>
			Function removeAttribute(ByVal old As AttributeSet, ByVal name As Object) As AttributeSet

			''' <summary>
			''' Removes a set of attributes for the element.
			''' </summary>
			''' <param name="old"> the old attribute set </param>
			''' <param name="names"> the attribute names </param>
			''' <returns> the updated attribute set </returns>
			''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
			Function removeAttributes(Of T1)(ByVal old As AttributeSet, ByVal names As System.Collections.IEnumerator(Of T1)) As AttributeSet

			''' <summary>
			''' Removes a set of attributes for the element.
			''' </summary>
			''' <param name="old"> the old attribute set </param>
			''' <param name="attrs"> the attributes </param>
			''' <returns> the updated attribute set </returns>
			''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
			Function removeAttributes(ByVal old As AttributeSet, ByVal attrs As AttributeSet) As AttributeSet

			''' <summary>
			''' Fetches an empty AttributeSet.
			''' </summary>
			''' <returns> the attribute set </returns>
			ReadOnly Property emptySet As AttributeSet

			''' <summary>
			''' Reclaims an attribute set.
			''' This is a way for a MutableAttributeSet to mark that it no
			''' longer need a particular immutable set.  This is only necessary
			''' in 1.1 where there are no weak references.  A 1.1 implementation
			''' would call this in its finalize method.
			''' </summary>
			''' <param name="a"> the attribute set to reclaim </param>
			Sub reclaim(ByVal a As AttributeSet)
		End Interface

		''' <summary>
		''' Implements the abstract part of an element.  By default elements
		''' support attributes by having a field that represents the immutable
		''' part of the current attribute set for the element.  The element itself
		''' implements MutableAttributeSet which can be used to modify the set
		''' by fetching a new immutable set.  The immutable sets are provided
		''' by the AttributeContext associated with the document.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		<Serializable> _
		Public MustInherit Class AbstractElement
			Implements Element, MutableAttributeSet, javax.swing.tree.TreeNode

				Public MustOverride Function getIndex(ByVal node As javax.swing.tree.TreeNode) As Integer
				Public MustOverride Sub removeAttributes(ByVal attributes As AttributeSet) Implements MutableAttributeSet.removeAttributes
				Public MustOverride Sub removeAttributes(Of T1)(ByVal names As System.Collections.IEnumerator(Of T1)) Implements MutableAttributeSet.removeAttributes
				Public MustOverride Sub addAttributes(ByVal attributes As AttributeSet) Implements MutableAttributeSet.addAttributes
				Public MustOverride Sub render(ByVal r As Runnable)
				Public MustOverride ReadOnly Property defaultRootElement As Element
				Public MustOverride ReadOnly Property rootElements As Element()
				Public MustOverride Function createPosition(ByVal offs As Integer) As Position
				Public MustOverride ReadOnly Property endPosition As Position
				Public MustOverride ReadOnly Property startPosition As Position
				Public MustOverride Sub getText(ByVal offset As Integer, ByVal length As Integer, ByVal txt As Segment)
				Public MustOverride Function getText(ByVal offset As Integer, ByVal length As Integer) As String
				Public MustOverride Sub insertString(ByVal offset As Integer, ByVal str As String, ByVal a As AttributeSet)
				Public MustOverride Sub remove(ByVal offs As Integer, ByVal len As Integer)
				Public MustOverride Sub putProperty(ByVal key As Object, ByVal value As Object)
				Public MustOverride Function getProperty(ByVal key As Object) As Object
				Public MustOverride Sub removeUndoableEditListener(ByVal listener As UndoableEditListener)
				Public MustOverride Sub addUndoableEditListener(ByVal listener As UndoableEditListener)
				Public MustOverride Sub removeDocumentListener(ByVal listener As DocumentListener)
				Public MustOverride Sub addDocumentListener(ByVal listener As DocumentListener)
				Public MustOverride ReadOnly Property length As Integer
			Private ReadOnly outerInstance As AbstractDocument


			''' <summary>
			''' Creates a new AbstractElement.
			''' </summary>
			''' <param name="parent"> the parent element </param>
			''' <param name="a"> the attributes for the element
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As AbstractDocument, ByVal parent As Element, ByVal a As AttributeSet)
					Me.outerInstance = outerInstance
				Me.parent = parent
				attributes = outerInstance.attributeContext.emptySet
				If a IsNot Nothing Then addAttributes(a)
			End Sub

			Private Sub indent(ByVal out As PrintWriter, ByVal n As Integer)
				For i As Integer = 0 To n - 1
					out.print("  ")
				Next i
			End Sub

			''' <summary>
			''' Dumps a debugging representation of the element hierarchy.
			''' </summary>
			''' <param name="psOut"> the output stream </param>
			''' <param name="indentAmount"> the indentation level &gt;= 0 </param>
			Public Overridable Sub dump(ByVal psOut As PrintStream, ByVal indentAmount As Integer)
				Dim out As PrintWriter
				Try
					out = New PrintWriter(New OutputStreamWriter(psOut,"JavaEsc"), True)
				Catch e As UnsupportedEncodingException
					out = New PrintWriter(psOut,True)
				End Try
				indent(out, indentAmount)
				If name Is Nothing Then
					out.print("<??")
				Else
					out.print("<" & name)
				End If
				If attributeCount > 0 Then
					out.println("")
					' dump the attributes
					Dim names As System.Collections.IEnumerator = attributes.attributeNames
					Do While names.hasMoreElements()
						Dim ___name As Object = names.nextElement()
						indent(out, indentAmount + 1)
						out.println(___name & "=" & getAttribute(___name))
					Loop
					indent(out, indentAmount)
				End If
				out.println(">")

				If leaf Then
					indent(out, indentAmount+1)
					out.print("[" & startOffset & "," & endOffset & "]")
					Dim c As Content = outerInstance.content
					Try
						Dim contentStr As String = c.getString(startOffset, endOffset - startOffset) '.trim()
						If contentStr.Length > 40 Then contentStr = contentStr.Substring(0, 40) & "..."
						out.println("[" & contentStr & "]")
					Catch e As BadLocationException
					End Try

				Else
					Dim n As Integer = elementCount
					For i As Integer = 0 To n - 1
						Dim e As AbstractElement = CType(getElement(i), AbstractElement)
						e.dump(psOut, indentAmount+1)
					Next i
				End If
			End Sub

			' --- AttributeSet ----------------------------
			' delegated to the immutable field "attributes"

			''' <summary>
			''' Gets the number of attributes that are defined.
			''' </summary>
			''' <returns> the number of attributes &gt;= 0 </returns>
			''' <seealso cref= AttributeSet#getAttributeCount </seealso>
			Public Overridable Property attributeCount As Integer Implements AttributeSet.getAttributeCount
				Get
					Return attributes.attributeCount
				End Get
			End Property

			''' <summary>
			''' Checks whether a given attribute is defined.
			''' </summary>
			''' <param name="attrName"> the non-null attribute name </param>
			''' <returns> true if the attribute is defined </returns>
			''' <seealso cref= AttributeSet#isDefined </seealso>
			Public Overridable Function isDefined(ByVal attrName As Object) As Boolean Implements AttributeSet.isDefined
				Return attributes.isDefined(attrName)
			End Function

			''' <summary>
			''' Checks whether two attribute sets are equal.
			''' </summary>
			''' <param name="attr"> the attribute set to check against </param>
			''' <returns> true if the same </returns>
			''' <seealso cref= AttributeSet#isEqual </seealso>
			Public Overridable Function isEqual(ByVal attr As AttributeSet) As Boolean Implements AttributeSet.isEqual
				Return attributes.isEqual(attr)
			End Function

			''' <summary>
			''' Copies a set of attributes.
			''' </summary>
			''' <returns> the copy </returns>
			''' <seealso cref= AttributeSet#copyAttributes </seealso>
			Public Overridable Function copyAttributes() As AttributeSet Implements AttributeSet.copyAttributes
				Return attributes.copyAttributes()
			End Function

			''' <summary>
			''' Gets the value of an attribute.
			''' </summary>
			''' <param name="attrName"> the non-null attribute name </param>
			''' <returns> the attribute value </returns>
			''' <seealso cref= AttributeSet#getAttribute </seealso>
			Public Overridable Function getAttribute(ByVal attrName As Object) As Object Implements AttributeSet.getAttribute
				Dim value As Object = attributes.getAttribute(attrName)
				If value Is Nothing Then
					' The delegate nor it's resolvers had a match,
					' so we'll try to resolve through the parent
					' element.
					Dim a As AttributeSet = If(parent IsNot Nothing, parent.attributes, Nothing)
					If a IsNot Nothing Then value = a.getAttribute(attrName)
				End If
				Return value
			End Function

			''' <summary>
			''' Gets the names of all attributes.
			''' </summary>
			''' <returns> the attribute names as an enumeration </returns>
			''' <seealso cref= AttributeSet#getAttributeNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Property attributeNames As System.Collections.IEnumerator(Of ?)
				Get
					Return attributes.attributeNames
				End Get
			End Property

			''' <summary>
			''' Checks whether a given attribute name/value is defined.
			''' </summary>
			''' <param name="name"> the non-null attribute name </param>
			''' <param name="value"> the attribute value </param>
			''' <returns> true if the name/value is defined </returns>
			''' <seealso cref= AttributeSet#containsAttribute </seealso>
			Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean Implements AttributeSet.containsAttribute
				Return attributes.containsAttribute(name, value)
			End Function


			''' <summary>
			''' Checks whether the element contains all the attributes.
			''' </summary>
			''' <param name="attrs"> the attributes to check </param>
			''' <returns> true if the element contains all the attributes </returns>
			''' <seealso cref= AttributeSet#containsAttributes </seealso>
			Public Overridable Function containsAttributes(ByVal attrs As AttributeSet) As Boolean Implements AttributeSet.containsAttributes
				Return attributes.containsAttributes(attrs)
			End Function

			''' <summary>
			''' Gets the resolving parent.
			''' If not overridden, the resolving parent defaults to
			''' the parent element.
			''' </summary>
			''' <returns> the attributes from the parent, <code>null</code> if none </returns>
			''' <seealso cref= AttributeSet#getResolveParent </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overridable Function getResolveParent() As AttributeSet Implements AttributeSet.getResolveParent 'JavaToDotNetTempPropertyGetresolveParent
			Public Overridable Property resolveParent As AttributeSet Implements AttributeSet.getResolveParent
				Get
					Dim a As AttributeSet = attributes.resolveParent
					If (a Is Nothing) AndAlso (parent IsNot Nothing) Then a = parent.attributes
					Return a
				End Get
				Set(ByVal parent As AttributeSet)
			End Property

			' --- MutableAttributeSet ----------------------------------
			' should fetch a new immutable record for the field
			' "attributes".

			''' <summary>
			''' Adds an attribute to the element.
			''' </summary>
			''' <param name="name"> the non-null attribute name </param>
			''' <param name="value"> the attribute value </param>
			''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
			Public Overridable Sub addAttribute(ByVal name As Object, ByVal value As Object) Implements MutableAttributeSet.addAttribute
				checkForIllegalCast()
				Dim context As AttributeContext = outerInstance.attributeContext
				attributes = context.addAttribute(attributes, name, value)
			End Sub

			''' <summary>
			''' Adds a set of attributes to the element.
			''' </summary>
			''' <param name="attr"> the attributes to add </param>
			''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
			Public Overridable Sub addAttributes(ByVal attr As AttributeSet) Implements MutableAttributeSet.addAttributes
				checkForIllegalCast()
				Dim context As AttributeContext = outerInstance.attributeContext
				attributes = context.addAttributes(attributes, attr)
			End Sub

			''' <summary>
			''' Removes an attribute from the set.
			''' </summary>
			''' <param name="name"> the non-null attribute name </param>
			''' <seealso cref= MutableAttributeSet#removeAttribute </seealso>
			Public Overridable Sub removeAttribute(ByVal name As Object) Implements MutableAttributeSet.removeAttribute
				checkForIllegalCast()
				Dim context As AttributeContext = outerInstance.attributeContext
				attributes = context.removeAttribute(attributes, name)
			End Sub

			''' <summary>
			''' Removes a set of attributes for the element.
			''' </summary>
			''' <param name="names"> the attribute names </param>
			''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
			Public Overridable Sub removeAttributes(Of T1)(ByVal names As System.Collections.IEnumerator(Of T1))
				checkForIllegalCast()
				Dim context As AttributeContext = outerInstance.attributeContext
				attributes = context.removeAttributes(attributes, names)
			End Sub

			''' <summary>
			''' Removes a set of attributes for the element.
			''' </summary>
			''' <param name="attrs"> the attributes </param>
			''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
			Public Overridable Sub removeAttributes(ByVal attrs As AttributeSet) Implements MutableAttributeSet.removeAttributes
				checkForIllegalCast()
				Dim context As AttributeContext = outerInstance.attributeContext
				If attrs Is Me Then
					attributes = context.emptySet
				Else
					attributes = context.removeAttributes(attributes, attrs)
				End If
			End Sub

				checkForIllegalCast()
				Dim context As AttributeContext = outerInstance.attributeContext
				If parent IsNot Nothing Then
					attributes = context.addAttribute(attributes, StyleConstants.ResolveAttribute, parent)
				Else
					attributes = context.removeAttribute(attributes, StyleConstants.ResolveAttribute)
				End If
			End Sub

			Private Sub checkForIllegalCast()
				Dim t As Thread = outerInstance.currentWriter
				If (t Is Nothing) OrElse (t IsNot Thread.CurrentThread) Then Throw New StateInvariantError("Illegal cast to MutableAttributeSet")
			End Sub

			' --- Element methods -------------------------------------

			''' <summary>
			''' Retrieves the underlying model.
			''' </summary>
			''' <returns> the model </returns>
			Public Overridable Property document As Document Implements Element.getDocument
				Get
					Return AbstractDocument.this
				End Get
			End Property

			''' <summary>
			''' Gets the parent of the element.
			''' </summary>
			''' <returns> the parent </returns>
			Public Overridable Property parentElement As Element Implements Element.getParentElement
				Get
					Return parent
				End Get
			End Property

			''' <summary>
			''' Gets the attributes for the element.
			''' </summary>
			''' <returns> the attribute set </returns>
			Public Overridable Property attributes As AttributeSet Implements Element.getAttributes
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the name of the element.
			''' </summary>
			''' <returns> the name, null if none </returns>
			Public Overridable Property name As String Implements Element.getName
				Get
					If attributes.isDefined(ElementNameAttribute) Then Return CStr(attributes.getAttribute(ElementNameAttribute))
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the starting offset in the model for the element.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			Public MustOverride ReadOnly Property startOffset As Integer Implements Element.getStartOffset

			''' <summary>
			''' Gets the ending offset in the model for the element.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			Public MustOverride ReadOnly Property endOffset As Integer Implements Element.getEndOffset

			''' <summary>
			''' Gets a child element.
			''' </summary>
			''' <param name="index"> the child index, &gt;= 0 &amp;&amp; &lt; getElementCount() </param>
			''' <returns> the child element </returns>
			Public MustOverride Function getElement(ByVal index As Integer) As Element Implements Element.getElement

			''' <summary>
			''' Gets the number of children for the element.
			''' </summary>
			''' <returns> the number of children &gt;= 0 </returns>
			Public MustOverride ReadOnly Property elementCount As Integer Implements Element.getElementCount

			''' <summary>
			''' Gets the child element index closest to the given model offset.
			''' </summary>
			''' <param name="offset"> the offset &gt;= 0 </param>
			''' <returns> the element index &gt;= 0 </returns>
			Public MustOverride Function getElementIndex(ByVal offset As Integer) As Integer Implements Element.getElementIndex

			''' <summary>
			''' Checks whether the element is a leaf.
			''' </summary>
			''' <returns> true if a leaf </returns>
			Public MustOverride ReadOnly Property leaf As Boolean Implements Element.isLeaf

			' --- TreeNode methods -------------------------------------

			''' <summary>
			''' Returns the child <code>TreeNode</code> at index
			''' <code>childIndex</code>.
			''' </summary>
			Public Overridable Function getChildAt(ByVal childIndex As Integer) As javax.swing.tree.TreeNode
				Return CType(getElement(childIndex), javax.swing.tree.TreeNode)
			End Function

			''' <summary>
			''' Returns the number of children <code>TreeNode</code>'s
			''' receiver contains. </summary>
			''' <returns> the number of children <code>TreeNodews</code>'s
			''' receiver contains </returns>
			Public Overridable Property childCount As Integer
				Get
					Return elementCount
				End Get
			End Property

			''' <summary>
			''' Returns the parent <code>TreeNode</code> of the receiver. </summary>
			''' <returns> the parent <code>TreeNode</code> of the receiver </returns>
			Public Overridable Property parent As javax.swing.tree.TreeNode
				Get
					Return CType(parentElement, javax.swing.tree.TreeNode)
				End Get
			End Property

			''' <summary>
			''' Returns the index of <code>node</code> in the receivers children.
			''' If the receiver does not contain <code>node</code>, -1 will be
			''' returned. </summary>
			''' <param name="node"> the location of interest </param>
			''' <returns> the index of <code>node</code> in the receiver's
			''' children, or -1 if absent </returns>
			Public Overridable Function getIndex(ByVal node As javax.swing.tree.TreeNode) As Integer
				For counter As Integer = childCount - 1 To 0 Step -1
					If getChildAt(counter) Is node Then Return counter
				Next counter
				Return -1
			End Function

			''' <summary>
			''' Returns true if the receiver allows children. </summary>
			''' <returns> true if the receiver allows children, otherwise false </returns>
			Public MustOverride ReadOnly Property allowsChildren As Boolean


			''' <summary>
			''' Returns the children of the receiver as an
			''' <code>Enumeration</code>. </summary>
			''' <returns> the children of the receiver as an <code>Enumeration</code> </returns>
			Public MustOverride Function children() As System.Collections.IEnumerator


			' --- serialization ---------------------------------------------

			Private Sub writeObject(ByVal s As ObjectOutputStream)
				s.defaultWriteObject()
				StyleContext.writeAttributeSet(s, attributes)
			End Sub

			Private Sub readObject(ByVal s As ObjectInputStream)
				s.defaultReadObject()
				Dim attr As MutableAttributeSet = New SimpleAttributeSet
				StyleContext.readAttributeSet(s, attr)
				Dim context As AttributeContext = outerInstance.attributeContext
				attributes = context.addAttributes(SimpleAttributeSet.EMPTY, attr)
			End Sub

			' ---- variables -----------------------------------------------------

			Private parent As Element
			<NonSerialized> _
			Private attributes As AttributeSet

		End Class

		''' <summary>
		''' Implements a composite element that contains other elements.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		Public Class BranchElement
			Inherits AbstractElement

			Private ReadOnly outerInstance As AbstractDocument


			''' <summary>
			''' Constructs a composite element that initially contains
			''' no children.
			''' </summary>
			''' <param name="parent">  The parent element </param>
			''' <param name="a"> the attributes for the element
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As AbstractDocument, ByVal parent As Element, ByVal a As AttributeSet)
					Me.outerInstance = outerInstance
				MyBase.New(parent, a)
				___children = New AbstractElement(0){}
				nchildren = 0
				lastIndex = -1
			End Sub

			''' <summary>
			''' Gets the child element that contains
			''' the given model position.
			''' </summary>
			''' <param name="pos"> the position &gt;= 0 </param>
			''' <returns> the element, null if none </returns>
			Public Overridable Function positionToElement(ByVal pos As Integer) As Element
				Dim ___index As Integer = getElementIndex(pos)
				Dim child As Element = ___children(___index)
				Dim p0 As Integer = child.startOffset
				Dim p1 As Integer = child.endOffset
				If (pos >= p0) AndAlso (pos < p1) Then Return child
				Return Nothing
			End Function

			''' <summary>
			''' Replaces content with a new set of elements.
			''' </summary>
			''' <param name="offset"> the starting offset &gt;= 0 </param>
			''' <param name="length"> the length to replace &gt;= 0 </param>
			''' <param name="elems"> the new elements </param>
			Public Overridable Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal elems As Element())
				Dim delta As Integer = elems.Length - length
				Dim src As Integer = offset + length
				Dim nmove As Integer = nchildren - src
				Dim dest As Integer = src + delta
				If (nchildren + delta) >= ___children.Length Then
					' need to grow the array
					Dim newLength As Integer = Math.Max(2*___children.Length, nchildren + delta)
					Dim newChildren As AbstractElement() = New AbstractElement(newLength - 1){}
					Array.Copy(___children, 0, newChildren, 0, offset)
					Array.Copy(elems, 0, newChildren, offset, elems.Length)
					Array.Copy(___children, src, newChildren, dest, nmove)
					___children = newChildren
				Else
					' patch the existing array
					Array.Copy(___children, src, ___children, dest, nmove)
					Array.Copy(elems, 0, ___children, offset, elems.Length)
				End If
				nchildren = nchildren + delta
			End Sub

			''' <summary>
			''' Converts the element to a string.
			''' </summary>
			''' <returns> the string </returns>
			Public Overrides Function ToString() As String
				Return "BranchElement(" & name & ") " & startOffset & "," & endOffset & vbLf
			End Function

			' --- Element methods -----------------------------------

			''' <summary>
			''' Gets the element name.
			''' </summary>
			''' <returns> the element name </returns>
			Public Property Overrides name As String
				Get
					Dim nm As String = MyBase.name
					If nm Is Nothing Then nm = ParagraphElementName
					Return nm
				End Get
			End Property

			''' <summary>
			''' Gets the starting offset in the model for the element.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			Public Property Overrides startOffset As Integer
				Get
					Return ___children(0).startOffset
				End Get
			End Property

			''' <summary>
			''' Gets the ending offset in the model for the element. </summary>
			''' <exception cref="NullPointerException"> if this element has no children
			''' </exception>
			''' <returns> the offset &gt;= 0 </returns>
			Public Property Overrides endOffset As Integer
				Get
					Dim child As Element = If(nchildren > 0, ___children(nchildren - 1), ___children(0))
					Return child.endOffset
				End Get
			End Property

			''' <summary>
			''' Gets a child element.
			''' </summary>
			''' <param name="index"> the child index, &gt;= 0 &amp;&amp; &lt; getElementCount() </param>
			''' <returns> the child element, null if none </returns>
			Public Overrides Function getElement(ByVal index As Integer) As Element
				If index < nchildren Then Return ___children(index)
				Return Nothing
			End Function

			''' <summary>
			''' Gets the number of children for the element.
			''' </summary>
			''' <returns> the number of children &gt;= 0 </returns>
			Public Property Overrides elementCount As Integer
				Get
					Return nchildren
				End Get
			End Property

			''' <summary>
			''' Gets the child element index closest to the given model offset.
			''' </summary>
			''' <param name="offset"> the offset &gt;= 0 </param>
			''' <returns> the element index &gt;= 0 </returns>
			Public Overrides Function getElementIndex(ByVal offset As Integer) As Integer
				Dim ___index As Integer
				Dim lower As Integer = 0
				Dim upper As Integer = nchildren - 1
				Dim mid As Integer = 0
				Dim p0 As Integer = startOffset
				Dim p1 As Integer

				If nchildren = 0 Then Return 0
				If offset >= endOffset Then Return nchildren - 1

				' see if the last index can be used.
				If (lastIndex >= lower) AndAlso (lastIndex <= upper) Then
					Dim lastHit As Element = ___children(lastIndex)
					p0 = lastHit.startOffset
					p1 = lastHit.endOffset
					If (offset >= p0) AndAlso (offset < p1) Then Return lastIndex

					' last index wasn't a hit, but it does give useful info about
					' where a hit (if any) would be.
					If offset < p0 Then
						upper = lastIndex
					Else
						lower = lastIndex
					End If
				End If

				Do While lower <= upper
					mid = lower + ((upper - lower) \ 2)
					Dim elem As Element = ___children(mid)
					p0 = elem.startOffset
					p1 = elem.endOffset
					If (offset >= p0) AndAlso (offset < p1) Then
						' found the location
						___index = mid
						lastIndex = ___index
						Return ___index
					ElseIf offset < p0 Then
						upper = mid - 1
					Else
						lower = mid + 1
					End If
				Loop

				' didn't find it, but we indicate the index of where it would belong
				If offset < p0 Then
					___index = mid
				Else
					___index = mid + 1
				End If
				lastIndex = ___index
				Return ___index
			End Function

			''' <summary>
			''' Checks whether the element is a leaf.
			''' </summary>
			''' <returns> true if a leaf </returns>
			Public Property Overrides leaf As Boolean
				Get
					Return False
				End Get
			End Property


			' ------ TreeNode ----------------------------------------------

			''' <summary>
			''' Returns true if the receiver allows children. </summary>
			''' <returns> true if the receiver allows children, otherwise false </returns>
			Public Property Overrides allowsChildren As Boolean
				Get
					Return True
				End Get
			End Property


			''' <summary>
			''' Returns the children of the receiver as an
			''' <code>Enumeration</code>. </summary>
			''' <returns> the children of the receiver </returns>
			Public Overrides Function children() As System.Collections.IEnumerator
				If nchildren = 0 Then Return Nothing

				Dim tempVector As New List(Of AbstractElement)(nchildren)

				For counter As Integer = 0 To nchildren - 1
					tempVector.Add(___children(counter))
				Next counter
				Return tempVector.elements()
			End Function

			' ------ members ----------------------------------------------

			Private ___children As AbstractElement()
			Private nchildren As Integer
			Private lastIndex As Integer
		End Class

		''' <summary>
		''' Implements an element that directly represents content of
		''' some kind.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref=     Element </seealso>
		Public Class LeafElement
			Inherits AbstractElement

			Private ReadOnly outerInstance As AbstractDocument


			''' <summary>
			''' Constructs an element that represents content within the
			''' document (has no children).
			''' </summary>
			''' <param name="parent">  The parent element </param>
			''' <param name="a">       The element attributes </param>
			''' <param name="offs0">   The start offset &gt;= 0 </param>
			''' <param name="offs1">   The end offset &gt;= offs0
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As AbstractDocument, ByVal parent As Element, ByVal a As AttributeSet, ByVal offs0 As Integer, ByVal offs1 As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(parent, a)
				Try
					p0 = createPosition(offs0)
					p1 = createPosition(offs1)
				Catch e As BadLocationException
					p0 = Nothing
					p1 = Nothing
					Throw New StateInvariantError("Can't create Position references")
				End Try
			End Sub

			''' <summary>
			''' Converts the element to a string.
			''' </summary>
			''' <returns> the string </returns>
			Public Overrides Function ToString() As String
				Return "LeafElement(" & name & ") " & p0 & "," & p1 & vbLf
			End Function

			' --- Element methods ---------------------------------------------

			''' <summary>
			''' Gets the starting offset in the model for the element.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			Public Property Overrides startOffset As Integer
				Get
					Return p0.offset
				End Get
			End Property

			''' <summary>
			''' Gets the ending offset in the model for the element.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			Public Property Overrides endOffset As Integer
				Get
					Return p1.offset
				End Get
			End Property

			''' <summary>
			''' Gets the element name.
			''' </summary>
			''' <returns> the name </returns>
			Public Property Overrides name As String
				Get
					Dim nm As String = MyBase.name
					If nm Is Nothing Then nm = ContentElementName
					Return nm
				End Get
			End Property

			''' <summary>
			''' Gets the child element index closest to the given model offset.
			''' </summary>
			''' <param name="pos"> the offset &gt;= 0 </param>
			''' <returns> the element index &gt;= 0 </returns>
			Public Overrides Function getElementIndex(ByVal pos As Integer) As Integer
				Return -1
			End Function

			''' <summary>
			''' Gets a child element.
			''' </summary>
			''' <param name="index"> the child index, &gt;= 0 &amp;&amp; &lt; getElementCount() </param>
			''' <returns> the child element </returns>
			Public Overrides Function getElement(ByVal index As Integer) As Element
				Return Nothing
			End Function

			''' <summary>
			''' Returns the number of child elements.
			''' </summary>
			''' <returns> the number of children &gt;= 0 </returns>
			Public Property Overrides elementCount As Integer
				Get
					Return 0
				End Get
			End Property

			''' <summary>
			''' Checks whether the element is a leaf.
			''' </summary>
			''' <returns> true if a leaf </returns>
			Public Property Overrides leaf As Boolean
				Get
					Return True
				End Get
			End Property

			' ------ TreeNode ----------------------------------------------

			''' <summary>
			''' Returns true if the receiver allows children. </summary>
			''' <returns> true if the receiver allows children, otherwise false </returns>
			Public Property Overrides allowsChildren As Boolean
				Get
					Return False
				End Get
			End Property


			''' <summary>
			''' Returns the children of the receiver as an
			''' <code>Enumeration</code>. </summary>
			''' <returns> the children of the receiver </returns>
			Public Overrides Function children() As System.Collections.IEnumerator
				Return Nothing
			End Function

			' --- serialization ---------------------------------------------

			Private Sub writeObject(ByVal s As ObjectOutputStream)
				s.defaultWriteObject()
				s.writeInt(p0.offset)
				s.writeInt(p1.offset)
			End Sub

			Private Sub readObject(ByVal s As ObjectInputStream)
				s.defaultReadObject()

				' set the range with positions that track change
				Dim off0 As Integer = s.readInt()
				Dim off1 As Integer = s.readInt()
				Try
					p0 = createPosition(off0)
					p1 = createPosition(off1)
				Catch e As BadLocationException
					p0 = Nothing
					p1 = Nothing
					Throw New IOException("Can't restore Position references")
				End Try
			End Sub

			' ---- members -----------------------------------------------------

			<NonSerialized> _
			Private p0 As Position
			<NonSerialized> _
			Private p1 As Position
		End Class

		''' <summary>
		''' Represents the root element of the bidirectional element structure.
		''' The root element is the only element in the bidi element structure
		''' which contains children.
		''' </summary>
		Friend Class BidiRootElement
			Inherits BranchElement

			Private ReadOnly outerInstance As AbstractDocument


			Friend Sub New(ByVal outerInstance As AbstractDocument)
					Me.outerInstance = outerInstance
				MyBase.New(Nothing, Nothing)
			End Sub

			''' <summary>
			''' Gets the name of the element. </summary>
			''' <returns> the name </returns>
			Public Property Overrides name As String
				Get
					Return "bidi root"
				End Get
			End Property
		End Class

		''' <summary>
		''' Represents an element of the bidirectional element structure.
		''' </summary>
		Friend Class BidiElement
			Inherits LeafElement

			Private ReadOnly outerInstance As AbstractDocument


			''' <summary>
			''' Creates a new BidiElement.
			''' </summary>
			Friend Sub New(ByVal outerInstance As AbstractDocument, ByVal parent As Element, ByVal start As Integer, ByVal [end] As Integer, ByVal level As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(parent, New SimpleAttributeSet, start, [end])
				addAttribute(StyleConstants.BidiLevel, Convert.ToInt32(level))
				'System.out.println("BidiElement: start = " + start
				'                   + " end = " + end + " level = " + level );
			End Sub

			''' <summary>
			''' Gets the name of the element. </summary>
			''' <returns> the name </returns>
			Public Property Overrides name As String
				Get
					Return BidiElementName
				End Get
			End Property

			Friend Overridable Property level As Integer
				Get
					Dim o As Integer? = CInt(Fix(getAttribute(StyleConstants.BidiLevel)))
					If o IsNot Nothing Then Return o
					Return 0 ' Level 0 is base level (non-embedded) left-to-right
				End Get
			End Property

			Friend Overridable Property leftToRight As Boolean
				Get
					Return ((level Mod 2) = 0)
				End Get
			End Property
		End Class

		''' <summary>
		''' Stores document changes as the document is being
		''' modified.  Can subsequently be used for change notification
		''' when done with the document modification transaction.
		''' This is used by the AbstractDocument class and its extensions
		''' for broadcasting change information to the document listeners.
		''' </summary>
		Public Class DefaultDocumentEvent
			Inherits CompoundEdit
			Implements DocumentEvent

			Private ReadOnly outerInstance As AbstractDocument


			''' <summary>
			''' Constructs a change record.
			''' </summary>
			''' <param name="offs"> the offset into the document of the change &gt;= 0 </param>
			''' <param name="len">  the length of the change &gt;= 0 </param>
			''' <param name="type"> the type of event (DocumentEvent.EventType)
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As AbstractDocument, ByVal offs As Integer, ByVal len As Integer, ByVal type As DocumentEvent.EventType)
					Me.outerInstance = outerInstance
				MyBase.New()
				offset = offs
				length = len
				Me.type = type
			End Sub

			''' <summary>
			''' Returns a string description of the change event.
			''' </summary>
			''' <returns> a string </returns>
			Public Overrides Function ToString() As String
				Return edits.ToString()
			End Function

			' --- CompoundEdit methods --------------------------

			''' <summary>
			''' Adds a document edit.  If the number of edits crosses
			''' a threshold, this switches on a hashtable lookup for
			''' ElementChange implementations since access of these
			''' needs to be relatively quick.
			''' </summary>
			''' <param name="anEdit"> a document edit record </param>
			''' <returns> true if the edit was added </returns>
			Public Overrides Function addEdit(ByVal anEdit As UndoableEdit) As Boolean
				' if the number of changes gets too great, start using
				' a hashtable for to locate the change for a given element.
				If (changeLookup Is Nothing) AndAlso (edits.Count > 10) Then
					changeLookup = New Dictionary(Of Element, ElementChange)
					Dim n As Integer = edits.Count
					For i As Integer = 0 To n - 1
						Dim o As Object = edits(i)
						If TypeOf o Is DocumentEvent.ElementChange Then
							Dim ec As DocumentEvent.ElementChange = CType(o, DocumentEvent.ElementChange)
							changeLookup(ec.element) = ec
						End If
					Next i
				End If

				' if we have a hashtable... add the entry if it's
				' an ElementChange.
				If (changeLookup IsNot Nothing) AndAlso (TypeOf anEdit Is DocumentEvent.ElementChange) Then
					Dim ec As DocumentEvent.ElementChange = CType(anEdit, DocumentEvent.ElementChange)
					changeLookup(ec.element) = ec
				End If
				Return MyBase.addEdit(anEdit)
			End Function

			''' <summary>
			''' Redoes a change.
			''' </summary>
			''' <exception cref="CannotRedoException"> if the change cannot be redone </exception>
			Public Overrides Sub redo()
				outerInstance.writeLock()
				Try
					' change the state
					MyBase.redo()
					' fire a DocumentEvent to notify the view(s)
					Dim ev As New UndoRedoDocumentEvent(Me, False)
					If type Is DocumentEvent.EventType.INSERT Then
						outerInstance.fireInsertUpdate(ev)
					ElseIf type Is DocumentEvent.EventType.REMOVE Then
						outerInstance.fireRemoveUpdate(ev)
					Else
						outerInstance.fireChangedUpdate(ev)
					End If
				Finally
					outerInstance.writeUnlock()
				End Try
			End Sub

			''' <summary>
			''' Undoes a change.
			''' </summary>
			''' <exception cref="CannotUndoException"> if the change cannot be undone </exception>
			Public Overrides Sub undo()
				outerInstance.writeLock()
				Try
					' change the state
					MyBase.undo()
					' fire a DocumentEvent to notify the view(s)
					Dim ev As New UndoRedoDocumentEvent(Me, True)
					If type Is DocumentEvent.EventType.REMOVE Then
						outerInstance.fireInsertUpdate(ev)
					ElseIf type Is DocumentEvent.EventType.INSERT Then
						outerInstance.fireRemoveUpdate(ev)
					Else
						outerInstance.fireChangedUpdate(ev)
					End If
				Finally
					outerInstance.writeUnlock()
				End Try
			End Sub

			''' <summary>
			''' DefaultDocument events are significant.  If you wish to aggregate
			''' DefaultDocumentEvents to present them as a single edit to the user
			''' place them into a CompoundEdit.
			''' </summary>
			''' <returns> whether the event is significant for edit undo purposes </returns>
			Public Property Overrides significant As Boolean
				Get
					Return True
				End Get
			End Property


			''' <summary>
			''' Provides a localized, human readable description of this edit
			''' suitable for use in, say, a change log.
			''' </summary>
			''' <returns> the description </returns>
			Public Property Overrides presentationName As String
				Get
					Dim ___type As DocumentEvent.EventType = type
					If ___type Is DocumentEvent.EventType.INSERT Then Return javax.swing.UIManager.getString("AbstractDocument.additionText")
					If ___type Is DocumentEvent.EventType.REMOVE Then Return javax.swing.UIManager.getString("AbstractDocument.deletionText")
					Return javax.swing.UIManager.getString("AbstractDocument.styleChangeText")
				End Get
			End Property

			''' <summary>
			''' Provides a localized, human readable description of the undoable
			''' form of this edit, e.g. for use as an Undo menu item. Typically
			''' derived from getDescription();
			''' </summary>
			''' <returns> the description </returns>
			Public Property Overrides undoPresentationName As String
				Get
					Return javax.swing.UIManager.getString("AbstractDocument.undoText") & " " & presentationName
				End Get
			End Property

			''' <summary>
			''' Provides a localized, human readable description of the redoable
			''' form of this edit, e.g. for use as a Redo menu item. Typically
			''' derived from getPresentationName();
			''' </summary>
			''' <returns> the description </returns>
			Public Property Overrides redoPresentationName As String
				Get
					Return javax.swing.UIManager.getString("AbstractDocument.redoText") & " " & presentationName
				End Get
			End Property

			' --- DocumentEvent methods --------------------------

			''' <summary>
			''' Returns the type of event.
			''' </summary>
			''' <returns> the event type as a DocumentEvent.EventType </returns>
			''' <seealso cref= DocumentEvent#getType </seealso>
			Public Overridable Property type As DocumentEvent.EventType
				Get
					Return type
				End Get
			End Property

			''' <summary>
			''' Returns the offset within the document of the start of the change.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			''' <seealso cref= DocumentEvent#getOffset </seealso>
			Public Overridable Property offset As Integer Implements DocumentEvent.getOffset
				Get
					Return offset
				End Get
			End Property

			''' <summary>
			''' Returns the length of the change.
			''' </summary>
			''' <returns> the length &gt;= 0 </returns>
			''' <seealso cref= DocumentEvent#getLength </seealso>
			Public Overridable Property length As Integer Implements DocumentEvent.getLength
				Get
					Return length
				End Get
			End Property

			''' <summary>
			''' Gets the document that sourced the change event.
			''' </summary>
			''' <returns> the document </returns>
			''' <seealso cref= DocumentEvent#getDocument </seealso>
			Public Overridable Property document As Document Implements DocumentEvent.getDocument
				Get
					Return AbstractDocument.this
				End Get
			End Property

			''' <summary>
			''' Gets the changes for an element.
			''' </summary>
			''' <param name="elem"> the element </param>
			''' <returns> the changes </returns>
			Public Overridable Function getChange(ByVal elem As Element) As DocumentEvent.ElementChange
				If changeLookup IsNot Nothing Then Return changeLookup(elem)
				Dim n As Integer = edits.Count
				For i As Integer = 0 To n - 1
					Dim o As Object = edits(i)
					If TypeOf o Is DocumentEvent.ElementChange Then
						Dim c As DocumentEvent.ElementChange = CType(o, DocumentEvent.ElementChange)
						If elem.Equals(c.element) Then Return c
					End If
				Next i
				Return Nothing
			End Function

			' --- member variables ------------------------------------

			Private offset As Integer
			Private length As Integer
			Private changeLookup As Dictionary(Of Element, ElementChange)
			Private type As DocumentEvent.EventType

		End Class

		''' <summary>
		''' This event used when firing document changes while Undo/Redo
		''' operations. It just wraps DefaultDocumentEvent and delegates
		''' all calls to it except getType() which depends on operation
		''' (Undo or Redo).
		''' </summary>
		Friend Class UndoRedoDocumentEvent
			Implements DocumentEvent

			Private ReadOnly outerInstance As AbstractDocument

			Private src As DefaultDocumentEvent = Nothing
			Private type As EventType = Nothing

			Public Sub New(ByVal outerInstance As AbstractDocument, ByVal src As DefaultDocumentEvent, ByVal isUndo As Boolean)
					Me.outerInstance = outerInstance
				Me.src = src
				If isUndo Then
					If src.type.Equals(EventType.INSERT) Then
						type = EventType.REMOVE
					ElseIf src.type.Equals(EventType.REMOVE) Then
						type = EventType.INSERT
					Else
						type = src.type
					End If
				Else
					type = src.type
				End If
			End Sub

			Public Overridable Property source As DefaultDocumentEvent
				Get
					Return src
				End Get
			End Property

			' DocumentEvent methods delegated to DefaultDocumentEvent source
			' except getType() which depends on operation (Undo or Redo).
			Public Overridable Property offset As Integer Implements DocumentEvent.getOffset
				Get
					Return src.offset
				End Get
			End Property

			Public Overridable Property length As Integer Implements DocumentEvent.getLength
				Get
					Return src.length
				End Get
			End Property

			Public Overridable Property document As Document Implements DocumentEvent.getDocument
				Get
					Return src.document
				End Get
			End Property

			Public Overridable Property type As DocumentEvent.EventType
				Get
					Return type
				End Get
			End Property

			Public Overridable Function getChange(ByVal elem As Element) As DocumentEvent.ElementChange
				Return src.getChange(elem)
			End Function
		End Class

		''' <summary>
		''' An implementation of ElementChange that can be added to the document
		''' event.
		''' </summary>
		Public Class ElementEdit
			Inherits AbstractUndoableEdit
			Implements DocumentEvent.ElementChange

			''' <summary>
			''' Constructs an edit record.  This does not modify the element
			''' so it can safely be used to <em>catch up</em> a view to the
			''' current model state for views that just attached to a model.
			''' </summary>
			''' <param name="e"> the element </param>
			''' <param name="index"> the index into the model &gt;= 0 </param>
			''' <param name="removed"> a set of elements that were removed </param>
			''' <param name="added"> a set of elements that were added </param>
			Public Sub New(ByVal e As Element, ByVal index As Integer, ByVal removed As Element(), ByVal added As Element())
				MyBase.New()
				Me.e = e
				Me.index = index
				Me.removed = removed
				Me.added = added
			End Sub

			''' <summary>
			''' Returns the underlying element.
			''' </summary>
			''' <returns> the element </returns>
			Public Overridable Property element As Element
				Get
					Return e
				End Get
			End Property

			''' <summary>
			''' Returns the index into the list of elements.
			''' </summary>
			''' <returns> the index &gt;= 0 </returns>
			Public Overridable Property index As Integer
				Get
					Return index
				End Get
			End Property

			''' <summary>
			''' Gets a list of children that were removed.
			''' </summary>
			''' <returns> the list </returns>
			Public Overridable Property childrenRemoved As Element()
				Get
					Return removed
				End Get
			End Property

			''' <summary>
			''' Gets a list of children that were added.
			''' </summary>
			''' <returns> the list </returns>
			Public Overridable Property childrenAdded As Element()
				Get
					Return added
				End Get
			End Property

			''' <summary>
			''' Redoes a change.
			''' </summary>
			''' <exception cref="CannotRedoException"> if the change cannot be redone </exception>
			Public Overrides Sub redo()
				MyBase.redo()

				' Since this event will be reused, switch around added/removed.
				Dim tmp As Element() = removed
				removed = added
				added = tmp

				' PENDING(prinz) need MutableElement interface, canRedo() should check
				CType(e, AbstractDocument.BranchElement).replace(index, removed.Length, added)
			End Sub

			''' <summary>
			''' Undoes a change.
			''' </summary>
			''' <exception cref="CannotUndoException"> if the change cannot be undone </exception>
			Public Overrides Sub undo()
				MyBase.undo()
				' PENDING(prinz) need MutableElement interface, canUndo() should check
				CType(e, AbstractDocument.BranchElement).replace(index, added.Length, removed)

				' Since this event will be reused, switch around added/removed.
				Dim tmp As Element() = removed
				removed = added
				added = tmp
			End Sub

			Private e As Element
			Private index As Integer
			Private removed As Element()
			Private added As Element()
		End Class


		Private Class DefaultFilterBypass
			Inherits DocumentFilter.FilterBypass

			Private ReadOnly outerInstance As AbstractDocument

			Public Sub New(ByVal outerInstance As AbstractDocument)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Property document As Document
				Get
					Return AbstractDocument.this
				End Get
			End Property

			Public Overridable Sub remove(ByVal offset As Integer, ByVal length As Integer)
				outerInstance.handleRemove(offset, length)
			End Sub

			Public Overridable Sub insertString(ByVal offset As Integer, ByVal [string] As String, ByVal attr As AttributeSet)
				outerInstance.handleInsertString(offset, [string], attr)
			End Sub

			Public Overridable Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet)
				outerInstance.handleRemove(offset, length)
				outerInstance.handleInsertString(offset, text, attrs)
			End Sub
		End Class
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
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
	''' The <code>Choice</code> class presents a pop-up menu of choices.
	''' The current choice is displayed as the title of the menu.
	''' <p>
	''' The following code example produces a pop-up menu:
	''' 
	''' <hr><blockquote><pre>
	''' Choice ColorChooser = new Choice();
	''' ColorChooser.add("Green");
	''' ColorChooser.add("Red");
	''' ColorChooser.add("Blue");
	''' </pre></blockquote><hr>
	''' <p>
	''' After this choice menu has been added to a panel,
	''' it appears as follows in its normal state:
	''' <p>
	''' <img src="doc-files/Choice-1.gif" alt="The following text describes the graphic"
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' In the picture, <code>"Green"</code> is the current choice.
	''' Pushing the mouse button down on the object causes a menu to
	''' appear with the current choice highlighted.
	''' <p>
	''' Some native platforms do not support arbitrary resizing of
	''' <code>Choice</code> components and the behavior of
	''' <code>setSize()/getSize()</code> is bound by
	''' such limitations.
	''' Native GUI <code>Choice</code> components' size are often bound by such
	''' attributes as font size and length of items contained within
	''' the <code>Choice</code>.
	''' <p>
	''' @author      Sami Shaio
	''' @author      Arthur van Hoff
	''' @since       JDK1.0
	''' </summary>
	Public Class Choice
		Inherits Component
		Implements ItemSelectable, Accessible

		''' <summary>
		''' The items for the <code>Choice</code>.
		''' This can be a <code>null</code> value.
		''' @serial </summary>
		''' <seealso cref= #add(String) </seealso>
		''' <seealso cref= #addItem(String) </seealso>
		''' <seealso cref= #getItem(int) </seealso>
		''' <seealso cref= #getItemCount() </seealso>
		''' <seealso cref= #insert(String, int) </seealso>
		''' <seealso cref= #remove(String) </seealso>
		Friend pItems As Vector(Of String)

		''' <summary>
		''' The index of the current choice for this <code>Choice</code>
		''' or -1 if nothing is selected.
		''' @serial </summary>
		''' <seealso cref= #getSelectedItem() </seealso>
		''' <seealso cref= #select(int) </seealso>
		Friend selectedIndex As Integer = -1

		<NonSerialized> _
		Friend itemListener As ItemListener

		Private Const base As String = "choice"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -4075310674757313071L

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			' initialize JNI field and method ids 
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Creates a new choice menu. The menu initially has no items in it.
		''' <p>
		''' By default, the first item added to the choice menu becomes the
		''' selected item, until a different selection is made by the user
		''' by calling one of the <code>select</code> methods. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref=       #select(int) </seealso>
		''' <seealso cref=       #select(java.lang.String) </seealso>
		Public Sub New()
			GraphicsEnvironment.checkHeadless()
			pItems = New Vector(Of )
		End Sub

		''' <summary>
		''' Constructs a name for this component.  Called by
		''' <code>getName</code> when the name is <code>null</code>.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Choice)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the <code>Choice</code>'s peer.  This peer allows us
		''' to change the look
		''' of the <code>Choice</code> without changing its functionality. </summary>
		''' <seealso cref=     java.awt.Toolkit#createChoice(java.awt.Choice) </seealso>
		''' <seealso cref=     java.awt.Component#getToolkit() </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createChoice(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the number of items in this <code>Choice</code> menu. </summary>
		''' <returns> the number of items in this <code>Choice</code> menu </returns>
		''' <seealso cref=     #getItem
		''' @since   JDK1.1 </seealso>
		Public Overridable Property itemCount As Integer
			Get
				Return countItems()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getItemCount()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function countItems() As Integer
			Return pItems.size()
		End Function

		''' <summary>
		''' Gets the string at the specified index in this
		''' <code>Choice</code> menu. </summary>
		''' <param name="index"> the index at which to begin </param>
		''' <seealso cref=        #getItemCount </seealso>
		Public Overridable Function getItem(ByVal index As Integer) As String
			Return getItemImpl(index)
		End Function

	'    
	'     * This is called by the native code, so client code can't
	'     * be called on the toolkit thread.
	'     
		Friend Function getItemImpl(ByVal index As Integer) As String
			Return pItems.elementAt(index)
		End Function

		''' <summary>
		''' Adds an item to this <code>Choice</code> menu. </summary>
		''' <param name="item">    the item to be added </param>
		''' <exception cref="NullPointerException">   if the item's value is
		'''                  <code>null</code>
		''' @since      JDK1.1 </exception>
		Public Overridable Sub add(ByVal item As String)
			addItem(item)
		End Sub

		''' <summary>
		''' Obsolete as of Java 2 platform v1.1.  Please use the
		''' <code>add</code> method instead.
		''' <p>
		''' Adds an item to this <code>Choice</code> menu. </summary>
		''' <param name="item"> the item to be added </param>
		''' <exception cref="NullPointerException"> if the item's value is equal to
		'''          <code>null</code> </exception>
		Public Overridable Sub addItem(ByVal item As String)
			SyncLock Me
				insertNoInvalidate(item, pItems.size())
			End SyncLock

			' This could change the preferred size of the Component.
			invalidateIfValid()
		End Sub

		''' <summary>
		''' Inserts an item to this <code>Choice</code>,
		''' but does not invalidate the <code>Choice</code>.
		''' Client methods must provide their own synchronization before
		''' invoking this method. </summary>
		''' <param name="item"> the item to be added </param>
		''' <param name="index"> the new item position </param>
		''' <exception cref="NullPointerException"> if the item's value is equal to
		'''          <code>null</code> </exception>
		Private Sub insertNoInvalidate(ByVal item As String, ByVal index As Integer)
			If item Is Nothing Then Throw New NullPointerException("cannot add null item to Choice")
			pItems.insertElementAt(item, index)
			Dim peer_Renamed As java.awt.peer.ChoicePeer = CType(Me.peer, java.awt.peer.ChoicePeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.add(item, index)
			' no selection or selection shifted up
			If selectedIndex < 0 OrElse selectedIndex >= index Then [select](0)
		End Sub


		''' <summary>
		''' Inserts the item into this choice at the specified position.
		''' Existing items at an index greater than or equal to
		''' <code>index</code> are shifted up by one to accommodate
		''' the new item.  If <code>index</code> is greater than or
		''' equal to the number of items in this choice,
		''' <code>item</code> is added to the end of this choice.
		''' <p>
		''' If the item is the first one being added to the choice,
		''' then the item becomes selected.  Otherwise, if the
		''' selected item was one of the items shifted, the first
		''' item in the choice becomes the selected item.  If the
		''' selected item was no among those shifted, it remains
		''' the selected item. </summary>
		''' <param name="item"> the non-<code>null</code> item to be inserted </param>
		''' <param name="index"> the position at which the item should be inserted </param>
		''' <exception cref="IllegalArgumentException"> if index is less than 0 </exception>
		Public Overridable Sub insert(ByVal item As String, ByVal index As Integer)
			SyncLock Me
				If index < 0 Then Throw New IllegalArgumentException("index less than zero.")
				' if the index greater than item count, add item to the end 
				index = Math.Min(index, pItems.size())

				insertNoInvalidate(item, index)
			End SyncLock

			' This could change the preferred size of the Component.
			invalidateIfValid()
		End Sub

		''' <summary>
		''' Removes the first occurrence of <code>item</code>
		''' from the <code>Choice</code> menu.  If the item
		''' being removed is the currently selected item,
		''' then the first item in the choice becomes the
		''' selected item.  Otherwise, the currently selected
		''' item remains selected (and the selected index is
		''' updated accordingly). </summary>
		''' <param name="item">  the item to remove from this <code>Choice</code> menu </param>
		''' <exception cref="IllegalArgumentException">  if the item doesn't
		'''                     exist in the choice menu
		''' @since      JDK1.1 </exception>
		Public Overridable Sub remove(ByVal item As String)
			SyncLock Me
				Dim index As Integer = pItems.IndexOf(item)
				If index < 0 Then
					Throw New IllegalArgumentException("item " & item & " not found in choice")
				Else
					removeNoInvalidate(index)
				End If
			End SyncLock

			' This could change the preferred size of the Component.
			invalidateIfValid()
		End Sub

		''' <summary>
		''' Removes an item from the choice menu
		''' at the specified position.  If the item
		''' being removed is the currently selected item,
		''' then the first item in the choice becomes the
		''' selected item.  Otherwise, the currently selected
		''' item remains selected (and the selected index is
		''' updated accordingly). </summary>
		''' <param name="position"> the position of the item </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified
		'''          position is out of bounds
		''' @since      JDK1.1 </exception>
		Public Overridable Sub remove(ByVal position As Integer)
			SyncLock Me
				removeNoInvalidate(position)
			End SyncLock

			' This could change the preferred size of the Component.
			invalidateIfValid()
		End Sub

		''' <summary>
		''' Removes an item from the <code>Choice</code> at the
		''' specified position, but does not invalidate the <code>Choice</code>.
		''' Client methods must provide their
		''' own synchronization before invoking this method. </summary>
		''' <param name="position">   the position of the item </param>
		Private Sub removeNoInvalidate(ByVal position As Integer)
			pItems.removeElementAt(position)
			Dim peer_Renamed As java.awt.peer.ChoicePeer = CType(Me.peer, java.awt.peer.ChoicePeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.remove(position)
			' Adjust selectedIndex if selected item was removed. 
			If pItems.size() = 0 Then
				selectedIndex = -1
			ElseIf selectedIndex = position Then
				[select](0)
			ElseIf selectedIndex > position Then
				[select](selectedIndex-1)
			End If
		End Sub


		''' <summary>
		''' Removes all items from the choice menu. </summary>
		''' <seealso cref=       #remove
		''' @since     JDK1.1 </seealso>
		Public Overridable Sub removeAll()
			SyncLock Me
				If peer IsNot Nothing Then CType(peer, java.awt.peer.ChoicePeer).removeAll()
				pItems.removeAllElements()
				selectedIndex = -1
			End SyncLock

			' This could change the preferred size of the Component.
			invalidateIfValid()
		End Sub

		''' <summary>
		''' Gets a representation of the current choice as a string. </summary>
		''' <returns>    a string representation of the currently
		'''                     selected item in this choice menu </returns>
		''' <seealso cref=       #getSelectedIndex </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedItem As String
			Get
				Return If(selectedIndex >= 0, getItem(selectedIndex), Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns an array (length 1) containing the currently selected
		''' item.  If this choice has no items, returns <code>null</code>. </summary>
		''' <seealso cref= ItemSelectable </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedObjects As Object() Implements ItemSelectable.getSelectedObjects
			Get
				If selectedIndex >= 0 Then
					Dim items As Object() = New Object(0){}
					items(0) = getItem(selectedIndex)
					Return items
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the index of the currently selected item.
		''' If nothing is selected, returns -1.
		''' </summary>
		''' <returns> the index of the currently selected item, or -1 if nothing
		'''  is currently selected </returns>
		''' <seealso cref= #getSelectedItem </seealso>
		Public Overridable Property selectedIndex As Integer
			Get
				Return selectedIndex
			End Get
		End Property

		''' <summary>
		''' Sets the selected item in this <code>Choice</code> menu to be the
		''' item at the specified position.
		''' 
		''' <p>Note that this method should be primarily used to
		''' initially select an item in this component.
		''' Programmatically calling this method will <i>not</i> trigger
		''' an <code>ItemEvent</code>.  The only way to trigger an
		''' <code>ItemEvent</code> is by user interaction.
		''' </summary>
		''' <param name="pos">      the position of the selected item </param>
		''' <exception cref="IllegalArgumentException"> if the specified
		'''                            position is greater than the
		'''                            number of items or less than zero </exception>
		''' <seealso cref=        #getSelectedItem </seealso>
		''' <seealso cref=        #getSelectedIndex </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub [select](ByVal pos As Integer)
			If (pos >= pItems.size()) OrElse (pos < 0) Then Throw New IllegalArgumentException("illegal Choice item position: " & pos)
			If pItems.size() > 0 Then
				selectedIndex = pos
				Dim peer_Renamed As java.awt.peer.ChoicePeer = CType(Me.peer, java.awt.peer.ChoicePeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.select(pos)
			End If
		End Sub

		''' <summary>
		''' Sets the selected item in this <code>Choice</code> menu
		''' to be the item whose name is equal to the specified string.
		''' If more than one item matches (is equal to) the specified string,
		''' the one with the smallest index is selected.
		''' 
		''' <p>Note that this method should be primarily used to
		''' initially select an item in this component.
		''' Programmatically calling this method will <i>not</i> trigger
		''' an <code>ItemEvent</code>.  The only way to trigger an
		''' <code>ItemEvent</code> is by user interaction.
		''' </summary>
		''' <param name="str">     the specified string </param>
		''' <seealso cref=         #getSelectedItem </seealso>
		''' <seealso cref=         #getSelectedIndex </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub [select](ByVal str As String)
			Dim index As Integer = pItems.IndexOf(str)
			If index >= 0 Then [select](index)
		End Sub

		''' <summary>
		''' Adds the specified item listener to receive item events from
		''' this <code>Choice</code> menu.  Item events are sent in response
		''' to user input, but not in response to calls to <code>select</code>.
		''' If l is <code>null</code>, no exception is thrown and no action
		''' is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model. </summary>
		''' <param name="l">    the item listener </param>
		''' <seealso cref=           #removeItemListener </seealso>
		''' <seealso cref=           #getItemListeners </seealso>
		''' <seealso cref=           #select </seealso>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           java.awt.event.ItemListener
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addItemListener(ByVal l As ItemListener) Implements ItemSelectable.addItemListener
			If l Is Nothing Then Return
			itemListener = AWTEventMulticaster.add(itemListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified item listener so that it no longer receives
		''' item events from this <code>Choice</code> menu.
		''' If l is <code>null</code>, no exception is thrown and no
		''' action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model. </summary>
		''' <param name="l">    the item listener </param>
		''' <seealso cref=           #addItemListener </seealso>
		''' <seealso cref=           #getItemListeners </seealso>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           java.awt.event.ItemListener
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeItemListener(ByVal l As ItemListener) Implements ItemSelectable.removeItemListener
			If l Is Nothing Then Return
			itemListener = AWTEventMulticaster.remove(itemListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the item listeners
		''' registered on this choice.
		''' </summary>
		''' <returns> all of this choice's <code>ItemListener</code>s
		'''         or an empty array if no item
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=           #addItemListener </seealso>
		''' <seealso cref=           #removeItemListener </seealso>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           java.awt.event.ItemListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property itemListeners As ItemListener()
			Get
				Return getListeners(GetType(ItemListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>Choice</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>Choice</code> <code>c</code>
		''' for its item listeners with the following code:
		''' 
		''' <pre>ItemListener[] ils = (ItemListener[])(c.getListeners(ItemListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this choice,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getItemListeners
		''' @since 1.3 </seealso>
		Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Class) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(ItemListener) Then
				l = itemListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			If e.id = ItemEvent.ITEM_STATE_CHANGED Then
				If (eventMask And AWTEvent.ITEM_EVENT_MASK) <> 0 OrElse itemListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this choice. If the event is an
		''' instance of <code>ItemEvent</code>, it invokes the
		''' <code>processItemEvent</code> method. Otherwise, it calls its
		''' superclass's <code>processEvent</code> method.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=        java.awt.event.ItemEvent </seealso>
		''' <seealso cref=        #processItemEvent
		''' @since      JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is ItemEvent Then
				processItemEvent(CType(e, ItemEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes item events occurring on this <code>Choice</code>
		''' menu by dispatching them to any registered
		''' <code>ItemListener</code> objects.
		''' <p>
		''' This method is not called unless item events are
		''' enabled for this component. Item events are enabled
		''' when one of the following occurs:
		''' <ul>
		''' <li>An <code>ItemListener</code> object is registered
		''' via <code>addItemListener</code>.
		''' <li>Item events are enabled via <code>enableEvents</code>.
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the item event </param>
		''' <seealso cref=         java.awt.event.ItemEvent </seealso>
		''' <seealso cref=         java.awt.event.ItemListener </seealso>
		''' <seealso cref=         #addItemListener(ItemListener) </seealso>
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processItemEvent(ByVal e As ItemEvent)
			Dim listener As ItemListener = itemListener
			If listener IsNot Nothing Then listener.itemStateChanged(e)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>Choice</code>
		''' menu. This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>    the parameter string of this <code>Choice</code> menu </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString() & ",current=" & selectedItem
		End Function


	'     Serialization support.
	'     

	'    
	'     * Choice Serial Data Version.
	'     * @serial
	'     
		Private choiceSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>ItemListeners</code>
		''' as optional data. The non-serializable
		''' <code>ItemListeners</code> are detected and
		''' no attempt is made to serialize them.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData <code>null</code> terminated sequence of 0
		'''   or more pairs; the pair consists of a <code>String</code>
		'''   and an <code>Object</code>; the <code>String</code> indicates
		'''   the type of object and is one of the following:
		'''   <code>itemListenerK</code> indicating an
		'''     <code>ItemListener</code> object
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#itemListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, itemListenerK, itemListener)
		  s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code> and if it
		''' isn't <code>null</code> adds a listener to receive
		''' item events fired by the <code>Choice</code> item.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code>
		''' @serial </exception>
		''' <seealso cref= #removeItemListener(ItemListener) </seealso>
		''' <seealso cref= #addItemListener(ItemListener) </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  GraphicsEnvironment.checkHeadless()
		  s.defaultReadObject()

		  Dim keyOrNull As Object
		  keyOrNull = s.readObject()
		  Do While Nothing IsNot keyOrNull
			Dim key As String = CStr(keyOrNull).intern()

			If itemListenerK = key Then
			  addItemListener(CType(s.readObject(), ItemListener))

			Else ' skip value for unrecognized key
			  s.readObject()
			End If
			  keyOrNull = s.readObject()
		  Loop
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>Choice</code>. For <code>Choice</code> components,
		''' the <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleAWTChoice</code>. A new <code>AccessibleAWTChoice</code>
		''' instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleAWTChoice</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>Choice</code>
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTChoice(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Choice</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to choice user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTChoice
			Inherits AccessibleAWTComponent
			Implements AccessibleAction

			Private ReadOnly outerInstance As Choice

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 7175603582428509322L

			Public Sub New(ByVal outerInstance As Choice)
					Me.outerInstance = outerInstance
				MyBase.New()
			End Sub

			''' <summary>
			''' Get the AccessibleAction associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleAction interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			''' <seealso cref= AccessibleAction </seealso>
			Public Overridable Property accessibleAction As AccessibleAction
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.COMBO_BOX
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible actions available in this object
			''' If there are more than one, the first one is considered the "default"
			''' action of the object.
			''' </summary>
			''' <returns> the zero-based number of Actions in this object </returns>
			Public Overridable Property accessibleActionCount As Integer
				Get
					Return 0 '  To be fully implemented in a future release
				End Get
			End Property

			''' <summary>
			''' Returns a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			''' <returns> a String description of the action </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String
				Return Nothing '  To be fully implemented in a future release
			End Function

			''' <summary>
			''' Perform the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the action was performed; otherwise false. </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean
				Return False '  To be fully implemented in a future release
			End Function

		End Class ' inner class AccessibleAWTChoice

	End Class

End Namespace
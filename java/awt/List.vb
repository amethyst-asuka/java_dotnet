Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
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
	''' The <code>List</code> component presents the user with a
	''' scrolling list of text items. The list can be set up so that
	''' the user can choose either one item or multiple items.
	''' <p>
	''' For example, the code&nbsp;.&nbsp;.&nbsp;.
	''' 
	''' <hr><blockquote><pre>
	''' List lst = new List(4, false);
	''' lst.add("Mercury");
	''' lst.add("Venus");
	''' lst.add("Earth");
	''' lst.add("JavaSoft");
	''' lst.add("Mars");
	''' lst.add("Jupiter");
	''' lst.add("Saturn");
	''' lst.add("Uranus");
	''' lst.add("Neptune");
	''' lst.add("Pluto");
	''' cnt.add(lst);
	''' </pre></blockquote><hr>
	''' <p>
	''' where <code>cnt</code> is a container, produces the following
	''' scrolling list:
	''' <p>
	''' <img src="doc-files/List-1.gif"
	''' alt="Shows a list containing: Venus, Earth, JavaSoft, and Mars. Javasoft is selected." style="float:center; margin: 7px 10px;">
	''' <p>
	''' If the List allows multiple selections, then clicking on
	''' an item that is already selected deselects it. In the preceding
	''' example, only one item from the scrolling list can be selected
	''' at a time, since the second argument when creating the new scrolling
	''' list is <code>false</code>. If the List does not allow multiple
	''' selections, selecting an item causes any other selected item
	''' to be deselected.
	''' <p>
	''' Note that the list in the example shown was created with four visible
	''' rows.  Once the list has been created, the number of visible rows
	''' cannot be changed.  A default <code>List</code> is created with
	''' four rows, so that <code>lst = new List()</code> is equivalent to
	''' <code>list = new List(4, false)</code>.
	''' <p>
	''' Beginning with Java&nbsp;1.1, the Abstract Window Toolkit
	''' sends the <code>List</code> object all mouse, keyboard, and focus events
	''' that occur over it. (The old AWT event model is being maintained
	''' only for backwards compatibility, and its use is discouraged.)
	''' <p>
	''' When an item is selected or deselected by the user, AWT sends an instance
	''' of <code>ItemEvent</code> to the list.
	''' When the user double-clicks on an item in a scrolling list,
	''' AWT sends an instance of <code>ActionEvent</code> to the
	''' list following the item event. AWT also generates an action event
	''' when the user presses the return key while an item in the
	''' list is selected.
	''' <p>
	''' If an application wants to perform some action based on an item
	''' in this list being selected or activated by the user, it should implement
	''' <code>ItemListener</code> or <code>ActionListener</code>
	''' as appropriate and register the new listener to receive
	''' events from this list.
	''' <p>
	''' For multiple-selection scrolling lists, it is considered a better
	''' user interface to use an external gesture (such as clicking on a
	''' button) to trigger the action.
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.event.ItemEvent </seealso>
	''' <seealso cref=         java.awt.event.ItemListener </seealso>
	''' <seealso cref=         java.awt.event.ActionEvent </seealso>
	''' <seealso cref=         java.awt.event.ActionListener
	''' @since       JDK1.0 </seealso>
	Public Class List
		Inherits Component
		Implements ItemSelectable, Accessible

		''' <summary>
		''' A vector created to contain items which will become
		''' part of the List Component.
		''' 
		''' @serial </summary>
		''' <seealso cref= #addItem(String) </seealso>
		''' <seealso cref= #getItem(int) </seealso>
		Friend items As New List(Of String)

		''' <summary>
		''' This field will represent the number of visible rows in the
		''' <code>List</code> Component.  It is specified only once, and
		''' that is when the list component is actually
		''' created.  It will never change.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getRows() </seealso>
		Friend rows As Integer = 0

		''' <summary>
		''' <code>multipleMode</code> is a variable that will
		''' be set to <code>true</code> if a list component is to be set to
		''' multiple selection mode, that is where the user can
		''' select more than one item in a list at one time.
		''' <code>multipleMode</code> will be set to false if the
		''' list component is set to single selection, that is where
		''' the user can only select one item on the list at any
		''' one time.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isMultipleMode() </seealso>
		''' <seealso cref= #setMultipleMode(boolean) </seealso>
		Friend multipleMode As Boolean = False

		''' <summary>
		''' <code>selected</code> is an array that will contain
		''' the indices of items that have been selected.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getSelectedIndexes() </seealso>
		''' <seealso cref= #getSelectedIndex() </seealso>
		Friend selected As Integer() = New Integer(){}

		''' <summary>
		''' This variable contains the value that will be used
		''' when trying to make a particular list item visible.
		''' 
		''' @serial </summary>
		''' <seealso cref= #makeVisible(int) </seealso>
		Friend visibleIndex As Integer = -1

		<NonSerialized> _
		Friend actionListener As ActionListener
		<NonSerialized> _
		Friend itemListener As ItemListener

		Private Const base As String = "list"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -3304312411574666869L

		''' <summary>
		''' Creates a new scrolling list.
		''' By default, there are four visible lines and multiple selections are
		''' not allowed.  Note that this is a convenience method for
		''' <code>List(0, false)</code>.  Also note that the number of visible
		''' lines in the list cannot be changed after it has been created. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New(0, False)
		End Sub

		''' <summary>
		''' Creates a new scrolling list initialized with the specified
		''' number of visible lines. By default, multiple selections are
		''' not allowed.  Note that this is a convenience method for
		''' <code>List(rows, false)</code>.  Also note that the number
		''' of visible rows in the list cannot be changed after it has
		''' been created. </summary>
		''' <param name="rows"> the number of items to show. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since       JDK1.1 </seealso>
		Public Sub New(ByVal rows As Integer)
			Me.New(rows, False)
		End Sub

		''' <summary>
		''' The default number of visible rows is 4.  A list with
		''' zero rows is unusable and unsightly.
		''' </summary>
		Friend Const DEFAULT_VISIBLE_ROWS As Integer = 4

		''' <summary>
		''' Creates a new scrolling list initialized to display the specified
		''' number of rows. Note that if zero rows are specified, then
		''' the list will be created with a default of four rows.
		''' Also note that the number of visible rows in the list cannot
		''' be changed after it has been created.
		''' If the value of <code>multipleMode</code> is
		''' <code>true</code>, then the user can select multiple items from
		''' the list. If it is <code>false</code>, only one item at a time
		''' can be selected. </summary>
		''' <param name="rows">   the number of items to show. </param>
		''' <param name="multipleMode">   if <code>true</code>,
		'''                     then multiple selections are allowed;
		'''                     otherwise, only one item can be selected at a time. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal rows As Integer, ByVal multipleMode As Boolean)
			GraphicsEnvironment.checkHeadless()
			Me.rows = If(rows <> 0, rows, DEFAULT_VISIBLE_ROWS)
			Me.multipleMode = multipleMode
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by
		''' <code>getName</code> when the name is <code>null</code>.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(List)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the peer for the list.  The peer allows us to modify the
		''' list's appearance without changing its functionality.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createList(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the peer for this list.  The peer allows us to modify the
		''' list's appearance without changing its functionality.
		''' </summary>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
				If peer_Renamed IsNot Nothing Then selected = peer_Renamed.selectedIndexes
				MyBase.removeNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the number of items in the list. </summary>
		''' <returns>     the number of items in the list </returns>
		''' <seealso cref=        #getItem
		''' @since      JDK1.1 </seealso>
		Public Overridable Property itemCount As Integer
			Get
				Return countItems()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getItemCount()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function countItems() As Integer
			Return items.Count
		End Function

		''' <summary>
		''' Gets the item associated with the specified index. </summary>
		''' <returns>       an item that is associated with
		'''                    the specified index </returns>
		''' <param name="index"> the position of the item </param>
		''' <seealso cref=          #getItemCount </seealso>
		Public Overridable Function getItem(ByVal index As Integer) As String
			Return getItemImpl(index)
		End Function

		' NOTE: This method may be called by privileged threads.
		'       We implement this functionality in a package-private method
		'       to insure that it cannot be overridden by client subclasses.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Friend Function getItemImpl(ByVal index As Integer) As String
			Return items(index)
		End Function

		''' <summary>
		''' Gets the items in the list. </summary>
		''' <returns>       a string array containing items of the list </returns>
		''' <seealso cref=          #select </seealso>
		''' <seealso cref=          #deselect </seealso>
		''' <seealso cref=          #isIndexSelected
		''' @since        JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property items As String()
			Get
				Dim itemCopies As String() = New String(items.Count - 1){}
				items.CopyTo(itemCopies)
				Return itemCopies
			End Get
		End Property

		''' <summary>
		''' Adds the specified item to the end of scrolling list. </summary>
		''' <param name="item"> the item to be added
		''' @since JDK1.1 </param>
		Public Overridable Sub add(ByVal item As String)
			addItem(item)
		End Sub

		''' @deprecated      replaced by <code>add(String)</code>. 
		<Obsolete("     replaced by <code>add(String)</code>.")> _
		Public Overridable Sub addItem(ByVal item As String)
			addItem(item, -1)
		End Sub

		''' <summary>
		''' Adds the specified item to the the scrolling list
		''' at the position indicated by the index.  The index is
		''' zero-based.  If the value of the index is less than zero,
		''' or if the value of the index is greater than or equal to
		''' the number of items in the list, then the item is added
		''' to the end of the list. </summary>
		''' <param name="item">   the item to be added;
		'''              if this parameter is <code>null</code> then the item is
		'''              treated as an empty string, <code>""</code> </param>
		''' <param name="index">  the position at which to add the item
		''' @since       JDK1.1 </param>
		Public Overridable Sub add(ByVal item As String, ByVal index As Integer)
			addItem(item, index)
		End Sub

		''' @deprecated      replaced by <code>add(String, int)</code>. 
		<Obsolete("     replaced by <code>add(String, int)</code>."), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addItem(ByVal item As String, ByVal index As Integer)
			If index < -1 OrElse index >= items.Count Then index = -1

			If item Is Nothing Then item = ""

			If index = -1 Then
				items.Add(item)
			Else
				items.Insert(index, item)
			End If

			Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.add(item, index)
		End Sub

		''' <summary>
		''' Replaces the item at the specified index in the scrolling list
		''' with the new string. </summary>
		''' <param name="newValue">   a new string to replace an existing item </param>
		''' <param name="index">      the position of the item to replace </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''          is out of range </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub replaceItem(ByVal newValue As String, ByVal index As Integer)
			remove(index)
			add(newValue, index)
		End Sub

		''' <summary>
		''' Removes all items from this list. </summary>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= #delItems
		''' @since JDK1.1 </seealso>
		Public Overridable Sub removeAll()
			clear()
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>removeAll()</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub clear()
			Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.removeAll()
			items = New List(Of )
			selected = New Integer(){}
		End Sub

		''' <summary>
		''' Removes the first occurrence of an item from the list.
		''' If the specified item is selected, and is the only selected
		''' item in the list, the list is set to have no selection. </summary>
		''' <param name="item">  the item to remove from the list </param>
		''' <exception cref="IllegalArgumentException">
		'''                     if the item doesn't exist in the list
		''' @since        JDK1.1 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub remove(ByVal item As String)
			Dim index As Integer = items.IndexOf(item)
			If index < 0 Then
				Throw New IllegalArgumentException("item " & item & " not found in list")
			Else
				remove(index)
			End If
		End Sub

		''' <summary>
		''' Removes the item at the specified position
		''' from this scrolling list.
		''' If the item with the specified position is selected, and is the
		''' only selected item in the list, the list is set to have no selection. </summary>
		''' <param name="position">   the index of the item to delete </param>
		''' <seealso cref=        #add(String, int)
		''' @since      JDK1.1 </seealso>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''               if the <code>position</code> is less than 0 or
		'''               greater than <code>getItemCount()-1</code> </exception>
		Public Overridable Sub remove(ByVal position As Integer)
			delItem(position)
		End Sub

		''' @deprecated     replaced by <code>remove(String)</code>
		'''                         and <code>remove(int)</code>. 
		<Obsolete("    replaced by <code>remove(String)</code>")> _
		Public Overridable Sub delItem(ByVal position As Integer)
			delItems(position, position)
		End Sub

		''' <summary>
		''' Gets the index of the selected item on the list,
		''' </summary>
		''' <returns>        the index of the selected item;
		'''                if no item is selected, or if multiple items are
		'''                selected, <code>-1</code> is returned. </returns>
		''' <seealso cref=           #select </seealso>
		''' <seealso cref=           #deselect </seealso>
		''' <seealso cref=           #isIndexSelected </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedIndex As Integer
			Get
				Dim sel As Integer() = selectedIndexes
				Return If(sel.Length = 1, sel(0), -1)
			End Get
		End Property

		''' <summary>
		''' Gets the selected indexes on the list.
		''' </summary>
		''' <returns>        an array of the selected indexes on this scrolling list;
		'''                if no item is selected, a zero-length array is returned. </returns>
		''' <seealso cref=           #select </seealso>
		''' <seealso cref=           #deselect </seealso>
		''' <seealso cref=           #isIndexSelected </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedIndexes As Integer()
			Get
				Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
				If peer_Renamed IsNot Nothing Then selected = peer_Renamed.selectedIndexes
				Return selected.clone()
			End Get
		End Property

		''' <summary>
		''' Gets the selected item on this scrolling list.
		''' </summary>
		''' <returns>        the selected item on the list;
		'''                if no item is selected, or if multiple items are
		'''                selected, <code>null</code> is returned. </returns>
		''' <seealso cref=           #select </seealso>
		''' <seealso cref=           #deselect </seealso>
		''' <seealso cref=           #isIndexSelected </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedItem As String
			Get
				Dim index As Integer = selectedIndex
				Return If(index < 0, Nothing, getItem(index))
			End Get
		End Property

		''' <summary>
		''' Gets the selected items on this scrolling list.
		''' </summary>
		''' <returns>        an array of the selected items on this scrolling list;
		'''                if no item is selected, a zero-length array is returned. </returns>
		''' <seealso cref=           #select </seealso>
		''' <seealso cref=           #deselect </seealso>
		''' <seealso cref=           #isIndexSelected </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedItems As String()
			Get
				Dim sel As Integer() = selectedIndexes
				Dim str As String() = New String(sel.Length - 1){}
				For i As Integer = 0 To sel.Length - 1
					str(i) = getItem(sel(i))
				Next i
				Return str
			End Get
		End Property

		''' <summary>
		''' Gets the selected items on this scrolling list in an array of Objects. </summary>
		''' <returns>        an array of <code>Object</code>s representing the
		'''                selected items on this scrolling list;
		'''                if no item is selected, a zero-length array is returned. </returns>
		''' <seealso cref= #getSelectedItems </seealso>
		''' <seealso cref= ItemSelectable </seealso>
		Public Overridable Property selectedObjects As Object() Implements ItemSelectable.getSelectedObjects
			Get
				Return selectedItems
			End Get
		End Property

		''' <summary>
		''' Selects the item at the specified index in the scrolling list.
		''' <p>
		''' Note that passing out of range parameters is invalid,
		''' and will result in unspecified behavior.
		''' 
		''' <p>Note that this method should be primarily used to
		''' initially select an item in this component.
		''' Programmatically calling this method will <i>not</i> trigger
		''' an <code>ItemEvent</code>.  The only way to trigger an
		''' <code>ItemEvent</code> is by user interaction.
		''' </summary>
		''' <param name="index"> the position of the item to select </param>
		''' <seealso cref=          #getSelectedItem </seealso>
		''' <seealso cref=          #deselect </seealso>
		''' <seealso cref=          #isIndexSelected </seealso>
		Public Overridable Sub [select](ByVal index As Integer)
			' Bug #4059614: select can't be synchronized while calling the peer,
			' because it is called from the Window Thread.  It is sufficient to
			' synchronize the code that manipulates 'selected' except for the
			' case where the peer changes.  To handle this case, we simply
			' repeat the selection process.

			Dim peer_Renamed As java.awt.peer.ListPeer
			Do
				peer_Renamed = CType(Me.peer, java.awt.peer.ListPeer)
				If peer_Renamed IsNot Nothing Then
					peer_Renamed.select(index)
					Return
				End If

				SyncLock Me
					Dim alreadySelected As Boolean = False

					For i As Integer = 0 To selected.Length - 1
						If selected(i) = index Then
							alreadySelected = True
							Exit For
						End If
					Next i

					If Not alreadySelected Then
						If Not multipleMode Then
							selected = New Integer(0){}
							selected(0) = index
						Else
							Dim newsel As Integer() = New Integer(selected.Length){}
							Array.Copy(selected, 0, newsel, 0, selected.Length)
							newsel(selected.Length) = index
							selected = newsel
						End If
					End If
				End SyncLock
			Loop While peer_Renamed IsNot Me.peer
		End Sub

		''' <summary>
		''' Deselects the item at the specified index.
		''' <p>
		''' Note that passing out of range parameters is invalid,
		''' and will result in unspecified behavior.
		''' <p>
		''' If the item at the specified index is not selected,
		''' then the operation is ignored. </summary>
		''' <param name="index"> the position of the item to deselect </param>
		''' <seealso cref=          #select </seealso>
		''' <seealso cref=          #getSelectedItem </seealso>
		''' <seealso cref=          #isIndexSelected </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub deselect(ByVal index As Integer)
			Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
			If peer_Renamed IsNot Nothing Then
				If multipleMode OrElse (selectedIndex Is index) Then peer_Renamed.deselect(index)
			End If

			For i As Integer = 0 To selected.Length - 1
				If selected(i) = index Then
					Dim newsel As Integer() = New Integer(selected.Length - 2){}
					Array.Copy(selected, 0, newsel, 0, i)
					Array.Copy(selected, i+1, newsel, i, selected.Length - (i+1))
					selected = newsel
					Return
				End If
			Next i
		End Sub

		''' <summary>
		''' Determines if the specified item in this scrolling list is
		''' selected. </summary>
		''' <param name="index">   the item to be checked </param>
		''' <returns>     <code>true</code> if the specified item has been
		'''                       selected; <code>false</code> otherwise </returns>
		''' <seealso cref=        #select </seealso>
		''' <seealso cref=        #deselect
		''' @since      JDK1.1 </seealso>
		Public Overridable Function isIndexSelected(ByVal index As Integer) As Boolean
			Return isSelected(index)
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>isIndexSelected(int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function isSelected(ByVal index As Integer) As Boolean
			Dim sel As Integer() = selectedIndexes
			For i As Integer = 0 To sel.Length - 1
				If sel(i) = index Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Gets the number of visible lines in this list.  Note that
		''' once the <code>List</code> has been created, this number
		''' will never change. </summary>
		''' <returns>     the number of visible lines in this scrolling list </returns>
		Public Overridable Property rows As Integer
			Get
				Return rows
			End Get
		End Property

		''' <summary>
		''' Determines whether this list allows multiple selections. </summary>
		''' <returns>     <code>true</code> if this list allows multiple
		'''                 selections; otherwise, <code>false</code> </returns>
		''' <seealso cref=        #setMultipleMode
		''' @since      JDK1.1 </seealso>
		Public Overridable Property multipleMode As Boolean
			Get
				Return allowsMultipleSelections()
			End Get
			Set(ByVal b As Boolean)
				multipleSelections = b
			End Set
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>isMultipleMode()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function allowsMultipleSelections() As Boolean
			Return multipleMode
		End Function


		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setMultipleMode(boolean)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property multipleSelections As Boolean
			Set(ByVal b As Boolean)
				If b <> multipleMode Then
					multipleMode = b
					Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.multipleMode = b
				End If
			End Set
		End Property

		''' <summary>
		''' Gets the index of the item that was last made visible by
		''' the method <code>makeVisible</code>. </summary>
		''' <returns>      the index of the item that was last made visible </returns>
		''' <seealso cref=         #makeVisible </seealso>
		Public Overridable Property visibleIndex As Integer
			Get
				Return visibleIndex
			End Get
		End Property

		''' <summary>
		''' Makes the item at the specified index visible. </summary>
		''' <param name="index">    the position of the item </param>
		''' <seealso cref=         #getVisibleIndex </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub makeVisible(ByVal index As Integer)
			visibleIndex = index
			Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.makeVisible(index)
		End Sub

		''' <summary>
		''' Gets the preferred dimensions for a list with the specified
		''' number of rows. </summary>
		''' <param name="rows">    number of rows in the list </param>
		''' <returns>     the preferred dimensions for displaying this scrolling list
		'''             given that the specified number of rows must be visible </returns>
		''' <seealso cref=        java.awt.Component#getPreferredSize
		''' @since      JDK1.1 </seealso>
		Public Overridable Function getPreferredSize(ByVal rows As Integer) As Dimension
			Return preferredSize(rows)
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getPreferredSize(int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function preferredSize(ByVal rows As Integer) As Dimension
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
				Return If(peer_Renamed IsNot Nothing, peer_Renamed.getPreferredSize(rows), MyBase.preferredSize())
			End SyncLock
		End Function

		''' <summary>
		''' Gets the preferred size of this scrolling list. </summary>
		''' <returns>     the preferred dimensions for displaying this scrolling list </returns>
		''' <seealso cref=        java.awt.Component#getPreferredSize
		''' @since      JDK1.1 </seealso>
		Public Property Overrides preferredSize As Dimension
			Get
				Return preferredSize()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getPreferredSize()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function preferredSize() As Dimension
			SyncLock treeLock
				Return If(rows > 0, preferredSize(rows), MyBase.preferredSize())
			End SyncLock
		End Function

		''' <summary>
		''' Gets the minimum dimensions for a list with the specified
		''' number of rows. </summary>
		''' <param name="rows">    number of rows in the list </param>
		''' <returns>     the minimum dimensions for displaying this scrolling list
		'''             given that the specified number of rows must be visible </returns>
		''' <seealso cref=        java.awt.Component#getMinimumSize
		''' @since      JDK1.1 </seealso>
		Public Overridable Function getMinimumSize(ByVal rows As Integer) As Dimension
			Return minimumSize(rows)
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getMinimumSize(int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function minimumSize(ByVal rows As Integer) As Dimension
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
				Return If(peer_Renamed IsNot Nothing, peer_Renamed.getMinimumSize(rows), MyBase.minimumSize())
			End SyncLock
		End Function

		''' <summary>
		''' Determines the minimum size of this scrolling list. </summary>
		''' <returns>       the minimum dimensions needed
		'''                        to display this scrolling list </returns>
		''' <seealso cref=          java.awt.Component#getMinimumSize()
		''' @since        JDK1.1 </seealso>
		Public Property Overrides minimumSize As Dimension
			Get
				Return minimumSize()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getMinimumSize()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function minimumSize() As Dimension
			SyncLock treeLock
				Return If(rows > 0, minimumSize(rows), MyBase.minimumSize())
			End SyncLock
		End Function

		''' <summary>
		''' Adds the specified item listener to receive item events from
		''' this list.  Item events are sent in response to user input, but not
		''' in response to calls to <code>select</code> or <code>deselect</code>.
		''' If listener <code>l</code> is <code>null</code>,
		''' no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the item listener </param>
		''' <seealso cref=           #removeItemListener </seealso>
		''' <seealso cref=           #getItemListeners </seealso>
		''' <seealso cref=           #select </seealso>
		''' <seealso cref=           #deselect </seealso>
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
		''' Removes the specified item listener so that it no longer
		''' receives item events from this list.
		''' If listener <code>l</code> is <code>null</code>,
		''' no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the item listener </param>
		''' <seealso cref=             #addItemListener </seealso>
		''' <seealso cref=             #getItemListeners </seealso>
		''' <seealso cref=             java.awt.event.ItemEvent </seealso>
		''' <seealso cref=             java.awt.event.ItemListener
		''' @since           JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeItemListener(ByVal l As ItemListener) Implements ItemSelectable.removeItemListener
			If l Is Nothing Then Return
			itemListener = AWTEventMulticaster.remove(itemListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the item listeners
		''' registered on this list.
		''' </summary>
		''' <returns> all of this list's <code>ItemListener</code>s
		'''         or an empty array if no item
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=             #addItemListener </seealso>
		''' <seealso cref=             #removeItemListener </seealso>
		''' <seealso cref=             java.awt.event.ItemEvent </seealso>
		''' <seealso cref=             java.awt.event.ItemListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property itemListeners As ItemListener()
			Get
				Return getListeners(GetType(ItemListener))
			End Get
		End Property

		''' <summary>
		''' Adds the specified action listener to receive action events from
		''' this list. Action events occur when a user double-clicks
		''' on a list item or types Enter when the list has the keyboard
		''' focus.
		''' <p>
		''' If listener <code>l</code> is <code>null</code>,
		''' no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the action listener </param>
		''' <seealso cref=           #removeActionListener </seealso>
		''' <seealso cref=           #getActionListeners </seealso>
		''' <seealso cref=           java.awt.event.ActionEvent </seealso>
		''' <seealso cref=           java.awt.event.ActionListener
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			If l Is Nothing Then Return
			actionListener = AWTEventMulticaster.add(actionListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified action listener so that it no longer
		''' receives action events from this list. Action events
		''' occur when a user double-clicks on a list item.
		''' If listener <code>l</code> is <code>null</code>,
		''' no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">     the action listener </param>
		''' <seealso cref=             #addActionListener </seealso>
		''' <seealso cref=             #getActionListeners </seealso>
		''' <seealso cref=             java.awt.event.ActionEvent </seealso>
		''' <seealso cref=             java.awt.event.ActionListener
		''' @since           JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			If l Is Nothing Then Return
			actionListener = AWTEventMulticaster.remove(actionListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the action listeners
		''' registered on this list.
		''' </summary>
		''' <returns> all of this list's <code>ActionListener</code>s
		'''         or an empty array if no action
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=             #addActionListener </seealso>
		''' <seealso cref=             #removeActionListener </seealso>
		''' <seealso cref=             java.awt.event.ActionEvent </seealso>
		''' <seealso cref=             java.awt.event.ActionListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>List</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>List</code> <code>l</code>
		''' for its item listeners with the following code:
		''' 
		''' <pre>ItemListener[] ils = (ItemListener[])(l.getListeners(ItemListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this list,
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
			If listenerType Is GetType(ActionListener) Then
				l = actionListener
			ElseIf listenerType Is GetType(ItemListener) Then
				l = itemListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			Select Case e.id
			  Case ActionEvent.ACTION_PERFORMED
				If (eventMask And AWTEvent.ACTION_EVENT_MASK) <> 0 OrElse actionListener IsNot Nothing Then Return True
				Return False
			  Case ItemEvent.ITEM_STATE_CHANGED
				If (eventMask And AWTEvent.ITEM_EVENT_MASK) <> 0 OrElse itemListener IsNot Nothing Then Return True
				Return False
			  Case Else
			End Select
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this scrolling list. If an event is
		''' an instance of <code>ItemEvent</code>, it invokes the
		''' <code>processItemEvent</code> method. Else, if the
		''' event is an instance of <code>ActionEvent</code>,
		''' it invokes <code>processActionEvent</code>.
		''' If the event is not an item event or an action event,
		''' it invokes <code>processEvent</code> on the superclass.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=          java.awt.event.ActionEvent </seealso>
		''' <seealso cref=          java.awt.event.ItemEvent </seealso>
		''' <seealso cref=          #processActionEvent </seealso>
		''' <seealso cref=          #processItemEvent
		''' @since        JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is ItemEvent Then
				processItemEvent(CType(e, ItemEvent))
				Return
			ElseIf TypeOf e Is ActionEvent Then
				processActionEvent(CType(e, ActionEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes item events occurring on this list by
		''' dispatching them to any registered
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
		''' <seealso cref=         #addItemListener </seealso>
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processItemEvent(ByVal e As ItemEvent)
			Dim listener As ItemListener = itemListener
			If listener IsNot Nothing Then listener.itemStateChanged(e)
		End Sub

		''' <summary>
		''' Processes action events occurring on this component
		''' by dispatching them to any registered
		''' <code>ActionListener</code> objects.
		''' <p>
		''' This method is not called unless action events are
		''' enabled for this component. Action events are enabled
		''' when one of the following occurs:
		''' <ul>
		''' <li>An <code>ActionListener</code> object is registered
		''' via <code>addActionListener</code>.
		''' <li>Action events are enabled via <code>enableEvents</code>.
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the action event </param>
		''' <seealso cref=         java.awt.event.ActionEvent </seealso>
		''' <seealso cref=         java.awt.event.ActionListener </seealso>
		''' <seealso cref=         #addActionListener </seealso>
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processActionEvent(ByVal e As ActionEvent)
			Dim listener As ActionListener = actionListener
			If listener IsNot Nothing Then listener.actionPerformed(e)
		End Sub

		''' <summary>
		''' Returns the parameter string representing the state of this
		''' scrolling list. This string is useful for debugging. </summary>
		''' <returns>    the parameter string of this scrolling list </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString() & ",selected=" & selectedItem
		End Function

		''' @deprecated As of JDK version 1.1,
		''' Not for public use in the future.
		''' This method is expected to be retained only as a package
		''' private method. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub delItems(ByVal start As Integer, ByVal [end] As Integer)
			For i As Integer = end To start Step -1
				items.RemoveAt(i)
			Next i
			Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.delItems(start, [end])
		End Sub

	'    
	'     * Serialization support.  Since the value of the selected
	'     * field isn't necessarily up to date, we sync it up with the
	'     * peer before serializing.
	'     

		''' <summary>
		''' The <code>List</code> component's
		''' Serialized Data Version.
		''' 
		''' @serial
		''' </summary>
		Private listSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>ItemListeners</code>
		''' and <code>ActionListeners</code> as optional data.
		''' The non-serializable listeners are detected and
		''' no attempt is made to serialize them.
		''' 
		''' @serialData <code>null</code> terminated sequence of 0
		'''  or more pairs; the pair consists of a <code>String</code>
		'''  and an <code>Object</code>; the <code>String</code>
		'''  indicates the type of object and is one of the
		'''  following:
		'''  <code>itemListenerK</code> indicating an
		'''    <code>ItemListener</code> object;
		'''  <code>actionListenerK</code> indicating an
		'''    <code>ActionListener</code> object
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#itemListenerK </seealso>
		''' <seealso cref= java.awt.Component#actionListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  SyncLock Me
			Dim peer_Renamed As java.awt.peer.ListPeer = CType(Me.peer, java.awt.peer.ListPeer)
			If peer_Renamed IsNot Nothing Then selected = peer_Renamed.selectedIndexes
		  End SyncLock
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, itemListenerK, itemListener)
		  AWTEventMulticaster.save(s, actionListenerK, actionListener)
		  s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code> and if it
		''' isn't <code>null</code> adds a listener to receive
		''' both item events and action events (as specified
		''' by the key stored in the stream) fired by the
		''' <code>List</code>.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to write </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
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

			ElseIf actionListenerK = key Then
			  addActionListener(CType(s.readObject(), ActionListener))

			Else ' skip value for unrecognized key
			  s.readObject()
			End If
			  keyOrNull = s.readObject()
		  Loop
		End Sub


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>List</code>. For lists, the <code>AccessibleContext</code>
		''' takes the form of an <code>AccessibleAWTList</code>.
		''' A new <code>AccessibleAWTList</code> instance is created, if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleAWTList</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>List</code>
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTList(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>List</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to list user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTList
			Inherits AccessibleAWTComponent
			Implements AccessibleSelection, ItemListener, ActionListener

			Private ReadOnly outerInstance As List

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 7924617370136012829L

			Public Sub New(ByVal outerInstance As List)
					Me.outerInstance = outerInstance
				MyBase.New()
				outerInstance.addActionListener(Me)
				outerInstance.addItemListener(Me)
			End Sub

			Public Overridable Sub actionPerformed(ByVal [event] As ActionEvent) Implements ActionListener.actionPerformed
			End Sub

			Public Overridable Sub itemStateChanged(ByVal [event] As ItemEvent) Implements ItemListener.itemStateChanged
			End Sub

			''' <summary>
			''' Get the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.multipleMode Then states.add(AccessibleState.MULTISELECTABLE)
					Return states
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
					Return AccessibleRole.LIST
				End Get
			End Property

			''' <summary>
			''' Returns the Accessible child contained at the local coordinate
			''' Point, if one exists.
			''' </summary>
			''' <returns> the Accessible at the specified location, if it exists </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Return Nothing ' fredxFIXME Not implemented yet
			End Function

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement Accessible, than this
			''' method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return outerInstance.itemCount
				End Get
			End Property

			''' <summary>
			''' Return the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				SyncLock List.this
					If i >= outerInstance.itemCount Then
						Return Nothing
					Else
						Return New AccessibleAWTListChild(Me, List.this, i)
					End If
				End SyncLock
			End Function

			''' <summary>
			''' Get the AccessibleSelection associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleSelection interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
					Return Me
				End Get
			End Property

		' AccessibleSelection methods

			''' <summary>
			''' Returns the number of items currently selected.
			''' If no items are selected, the return value will be 0.
			''' </summary>
			''' <returns> the number of items currently selected. </returns>
			 Public Overridable Property accessibleSelectionCount As Integer
				 Get
					 Return outerInstance.selectedIndexes.length
				 End Get
			 End Property

			''' <summary>
			''' Returns an Accessible representing the specified selected item
			''' in the object.  If there isn't a selection, or there are
			''' fewer items selected than the integer passed in, the return
			''' value will be null.
			''' </summary>
			''' <param name="i"> the zero-based index of selected items </param>
			''' <returns> an Accessible containing the selected item </returns>
			 Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible
				 SyncLock List.this
					 Dim len As Integer = accessibleSelectionCount
					 If i < 0 OrElse i >= len Then
						 Return Nothing
					 Else
						 Return getAccessibleChild(outerInstance.selectedIndexes(i))
					 End If
				 End SyncLock
			 End Function

			''' <summary>
			''' Returns true if the current child of this object is selected.
			''' </summary>
			''' <param name="i"> the zero-based index of the child in this Accessible
			''' object. </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean
				Return outerInstance.isIndexSelected(i)
			End Function

			''' <summary>
			''' Adds the specified selected item in the object to the object's
			''' selection.  If the object supports multiple selections,
			''' the specified item is added to any existing selection, otherwise
			''' it replaces any existing selection in the object.  If the
			''' specified item is already selected, this method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			 Public Overridable Sub addAccessibleSelection(ByVal i As Integer)
				 outerInstance.select(i)
			 End Sub

			''' <summary>
			''' Removes the specified selected item in the object from the object's
			''' selection.  If the specified item isn't currently selected, this
			''' method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			 Public Overridable Sub removeAccessibleSelection(ByVal i As Integer)
				 outerInstance.deselect(i)
			 End Sub

			''' <summary>
			''' Clears the selection in the object, so that nothing in the
			''' object is selected.
			''' </summary>
			 Public Overridable Sub clearAccessibleSelection()
				 SyncLock List.this
					 Dim selectedIndexes As Integer() = outerInstance.selectedIndexes
					 If selectedIndexes Is Nothing Then Return
					 For i As Integer = selectedIndexes.Length - 1 To 0 Step -1
						 outerInstance.deselect(selectedIndexes(i))
					 Next i
				 End SyncLock
			 End Sub

			''' <summary>
			''' Causes every selected item in the object to be selected
			''' if the object supports multiple selections.
			''' </summary>
			 Public Overridable Sub selectAllAccessibleSelection()
				 SyncLock List.this
					 For i As Integer = outerInstance.itemCount - 1 To 0 Step -1
						 outerInstance.select(i)
					 Next i
				 End SyncLock
			 End Sub

		   ''' <summary>
		   ''' This class implements accessibility support for
		   ''' List children.  It provides an implementation of the
		   ''' Java Accessibility API appropriate to list children
		   ''' user-interface elements.
		   ''' @since 1.3
		   ''' </summary>
			Protected Friend Class AccessibleAWTListChild
				Inherits AccessibleAWTComponent
				Implements Accessible

				Private ReadOnly outerInstance As List.AccessibleAWTList

	'            
	'             * JDK 1.3 serialVersionUID
	'             
				Private Const serialVersionUID As Long = 4412022926028300317L

			' [[[FIXME]]] need to finish implementing this!!!

				Private parent As List
				Private indexInParent As Integer

				Public Sub New(ByVal outerInstance As List.AccessibleAWTList, ByVal parent As List, ByVal indexInParent As Integer)
						Me.outerInstance = outerInstance
					Me.parent = parent
					Me.accessibleParent = parent
					Me.indexInParent = indexInParent
				End Sub

				'
				' required Accessible methods
				'
			  ''' <summary>
			  ''' Gets the AccessibleContext for this object.  In the
			  ''' implementation of the Java Accessibility API for this class,
			  ''' return this object, which acts as its own AccessibleContext.
			  ''' </summary>
			  ''' <returns> this object </returns>
				Public Overridable Property accessibleContext As AccessibleContext
					Get
						Return Me
					End Get
				End Property

				'
				' required AccessibleContext methods
				'

				''' <summary>
				''' Get the role of this object.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of
				''' the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Overridable Property accessibleRole As AccessibleRole
					Get
						Return AccessibleRole.LIST_ITEM
					End Get
				End Property

				''' <summary>
				''' Get the state set of this object.  The AccessibleStateSet of an
				''' object is composed of a set of unique AccessibleState's.  A
				''' change in the AccessibleStateSet of an object will cause a
				''' PropertyChangeEvent to be fired for the
				''' ACCESSIBLE_STATE_PROPERTY property.
				''' </summary>
				''' <returns> an instance of AccessibleStateSet containing the
				''' current state set of the object </returns>
				''' <seealso cref= AccessibleStateSet </seealso>
				''' <seealso cref= AccessibleState </seealso>
				''' <seealso cref= #addPropertyChangeListener </seealso>
				Public Overridable Property accessibleStateSet As AccessibleStateSet
					Get
						Dim states As AccessibleStateSet = MyBase.accessibleStateSet
						If parent.isIndexSelected(indexInParent) Then states.add(AccessibleState.SELECTED)
						Return states
					End Get
				End Property

				''' <summary>
				''' Gets the locale of the component. If the component does not
				''' have a locale, then the locale of its parent is returned.
				''' </summary>
				''' <returns> This component's locale.  If this component does not have
				''' a locale, the locale of its parent is returned.
				''' </returns>
				''' <exception cref="IllegalComponentStateException">
				''' If the Component does not have its own locale and has not yet
				''' been added to a containment hierarchy such that the locale can
				''' be determined from the containing parent. </exception>
				Public Overridable Property locale As java.util.Locale
					Get
						Return parent.locale
					End Get
				End Property

				''' <summary>
				''' Get the 0-based index of this object in its accessible parent.
				''' </summary>
				''' <returns> the 0-based index of this object in its parent; -1 if
				''' this object does not have an accessible parent.
				''' </returns>
				''' <seealso cref= #getAccessibleParent </seealso>
				''' <seealso cref= #getAccessibleChildrenCount </seealso>
				''' <seealso cref= #getAccessibleChild </seealso>
				Public Overridable Property accessibleIndexInParent As Integer
					Get
						Return indexInParent
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible children of the object.
				''' </summary>
				''' <returns> the number of accessible children of the object. </returns>
				Public Overridable Property accessibleChildrenCount As Integer
					Get
						Return 0 ' list elements can't have children
					End Get
				End Property

				''' <summary>
				''' Return the specified Accessible child of the object.  The
				''' Accessible children of an Accessible object are zero-based,
				''' so the first child of an Accessible child is at index 0, the
				''' second child is at index 1, and so on.
				''' </summary>
				''' <param name="i"> zero-based index of child </param>
				''' <returns> the Accessible child of the object </returns>
				''' <seealso cref= #getAccessibleChildrenCount </seealso>
				Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
					Return Nothing ' list elements can't have children
				End Function


				'
				' AccessibleComponent delegatation to parent List
				'

				''' <summary>
				''' Get the background color of this object.
				''' </summary>
				''' <returns> the background color, if supported, of the object;
				''' otherwise, null </returns>
				''' <seealso cref= #setBackground </seealso>
				Public Overridable Property background As Color
					Get
						Return parent.background
					End Get
					Set(ByVal c As Color)
						parent.background = c
					End Set
				End Property


				''' <summary>
				''' Get the foreground color of this object.
				''' </summary>
				''' <returns> the foreground color, if supported, of the object;
				''' otherwise, null </returns>
				''' <seealso cref= #setForeground </seealso>
				Public Overridable Property foreground As Color
					Get
						Return parent.foreground
					End Get
					Set(ByVal c As Color)
						parent.foreground = c
					End Set
				End Property


				''' <summary>
				''' Get the Cursor of this object.
				''' </summary>
				''' <returns> the Cursor, if supported, of the object; otherwise, null </returns>
				''' <seealso cref= #setCursor </seealso>
				Public Overridable Property cursor As Cursor
					Get
						Return parent.cursor
					End Get
					Set(ByVal cursor_Renamed As Cursor)
						parent.cursor = cursor_Renamed
					End Set
				End Property


				''' <summary>
				''' Get the Font of this object.
				''' </summary>
				''' <returns> the Font,if supported, for the object; otherwise, null </returns>
				''' <seealso cref= #setFont </seealso>
				Public Overridable Property font As Font
					Get
						Return parent.font
					End Get
					Set(ByVal f As Font)
						parent.font = f
					End Set
				End Property


				''' <summary>
				''' Get the FontMetrics of this object.
				''' </summary>
				''' <param name="f"> the Font </param>
				''' <returns> the FontMetrics, if supported, the object; otherwise, null </returns>
				''' <seealso cref= #getFont </seealso>
				Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics
					Return parent.getFontMetrics(f)
				End Function

				''' <summary>
				''' Determine if the object is enabled.  Objects that are enabled
				''' will also have the AccessibleState.ENABLED state set in their
				''' AccessibleStateSet.
				''' </summary>
				''' <returns> true if object is enabled; otherwise, false </returns>
				''' <seealso cref= #setEnabled </seealso>
				''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				''' <seealso cref= AccessibleState#ENABLED </seealso>
				''' <seealso cref= AccessibleStateSet </seealso>
				Public Overridable Property enabled As Boolean
					Get
						Return parent.enabled
					End Get
					Set(ByVal b As Boolean)
						parent.enabled = b
					End Set
				End Property


				''' <summary>
				''' Determine if the object is visible.  Note: this means that the
				''' object intends to be visible; however, it may not be
				''' showing on the screen because one of the objects that this object
				''' is contained by is currently not visible.  To determine if an
				''' object is showing on the screen, use isShowing().
				''' <p>Objects that are visible will also have the
				''' AccessibleState.VISIBLE state set in their AccessibleStateSet.
				''' </summary>
				''' <returns> true if object is visible; otherwise, false </returns>
				''' <seealso cref= #setVisible </seealso>
				''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				''' <seealso cref= AccessibleState#VISIBLE </seealso>
				''' <seealso cref= AccessibleStateSet </seealso>
				Public Overridable Property visible As Boolean
					Get
						' [[[FIXME]]] needs to work like isShowing() below
						Return False
						' return parent.isVisible();
					End Get
					Set(ByVal b As Boolean)
						' [[[FIXME]]] should scroll to item to make it show!
						parent.visible = b
					End Set
				End Property


				''' <summary>
				''' Determine if the object is showing.  This is determined by
				''' checking the visibility of the object and visibility of the
				''' object ancestors.
				''' Note: this will return true even if the object is obscured
				''' by another (for example, it to object is underneath a menu
				''' that was pulled down).
				''' </summary>
				''' <returns> true if object is showing; otherwise, false </returns>
				Public Overridable Property showing As Boolean
					Get
						' [[[FIXME]]] only if it's showing!!!
						Return False
						' return parent.isShowing();
					End Get
				End Property

				''' <summary>
				''' Checks whether the specified point is within this object's
				''' bounds, where the point's x and y coordinates are defined to
				''' be relative to the coordinate system of the object.
				''' </summary>
				''' <param name="p"> the Point relative to the coordinate system of the
				''' object </param>
				''' <returns> true if object contains Point; otherwise false </returns>
				''' <seealso cref= #getBounds </seealso>
				Public Overridable Function contains(ByVal p As Point) As Boolean
					' [[[FIXME]]] - only if p is within the list element!!!
					Return False
					' return parent.contains(p);
				End Function

				''' <summary>
				''' Returns the location of the object on the screen.
				''' </summary>
				''' <returns> location of object on screen; null if this object
				''' is not on the screen </returns>
				''' <seealso cref= #getBounds </seealso>
				''' <seealso cref= #getLocation </seealso>
				Public Overridable Property locationOnScreen As Point
					Get
						' [[[FIXME]]] sigh
						Return Nothing
					End Get
				End Property

				''' <summary>
				''' Gets the location of the object relative to the parent in the
				''' form of a point specifying the object's top-left corner in the
				''' screen's coordinate space.
				''' </summary>
				''' <returns> An instance of Point representing the top-left corner of
				''' the objects's bounds in the coordinate space of the screen; null
				''' if this object or its parent are not on the screen </returns>
				''' <seealso cref= #getBounds </seealso>
				''' <seealso cref= #getLocationOnScreen </seealso>
				Public Overridable Property location As Point
					Get
						' [[[FIXME]]]
						Return Nothing
					End Get
					Set(ByVal p As Point)
						' [[[FIXME]]] maybe - can simply return as no-op
					End Set
				End Property


				''' <summary>
				''' Gets the bounds of this object in the form of a Rectangle object.
				''' The bounds specify this object's width, height, and location
				''' relative to its parent.
				''' </summary>
				''' <returns> A rectangle indicating this component's bounds; null if
				''' this object is not on the screen. </returns>
				''' <seealso cref= #contains </seealso>
				Public Overridable Property bounds As Rectangle
					Get
						' [[[FIXME]]]
						Return Nothing
					End Get
					Set(ByVal r As Rectangle)
						' no-op; not supported
					End Set
				End Property


				''' <summary>
				''' Returns the size of this object in the form of a Dimension
				''' object.  The height field of the Dimension object contains this
				''' objects's height, and the width field of the Dimension object
				''' contains this object's width.
				''' </summary>
				''' <returns> A Dimension object that indicates the size of this
				''' component; null if this object is not on the screen </returns>
				''' <seealso cref= #setSize </seealso>
				Public Overridable Property size As Dimension
					Get
						' [[[FIXME]]]
						Return Nothing
					End Get
					Set(ByVal d As Dimension)
						' not supported; no-op
					End Set
				End Property


				''' <summary>
				''' Returns the <code>Accessible</code> child, if one exists,
				''' contained at the local coordinate <code>Point</code>.
				''' </summary>
				''' <param name="p"> the point relative to the coordinate system of this
				'''     object </param>
				''' <returns> the <code>Accessible</code>, if it exists,
				'''     at the specified location; otherwise <code>null</code> </returns>
				Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
					Return Nothing ' object cannot have children!
				End Function

				''' <summary>
				''' Returns whether this object can accept focus or not.   Objects
				''' that can accept focus will also have the
				''' <code>AccessibleState.FOCUSABLE</code> state set in their
				''' <code>AccessibleStateSet</code>.
				''' </summary>
				''' <returns> true if object can accept focus; otherwise false </returns>
				''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				''' <seealso cref= AccessibleState#FOCUSABLE </seealso>
				''' <seealso cref= AccessibleState#FOCUSED </seealso>
				''' <seealso cref= AccessibleStateSet </seealso>
				Public Overridable Property focusTraversable As Boolean
					Get
						Return False ' list element cannot receive focus!
					End Get
				End Property

				''' <summary>
				''' Requests focus for this object.  If this object cannot accept
				''' focus, nothing will happen.  Otherwise, the object will attempt
				''' to take focus. </summary>
				''' <seealso cref= #isFocusTraversable </seealso>
				Public Overridable Sub requestFocus()
					' nothing to do; a no-op
				End Sub

				''' <summary>
				''' Adds the specified focus listener to receive focus events from
				''' this component.
				''' </summary>
				''' <param name="l"> the focus listener </param>
				''' <seealso cref= #removeFocusListener </seealso>
				Public Overridable Sub addFocusListener(ByVal l As FocusListener)
					' nothing to do; a no-op
				End Sub

				''' <summary>
				''' Removes the specified focus listener so it no longer receives
				''' focus events from this component.
				''' </summary>
				''' <param name="l"> the focus listener </param>
				''' <seealso cref= #addFocusListener </seealso>
				Public Overridable Sub removeFocusListener(ByVal l As FocusListener)
					' nothing to do; a no-op
				End Sub



			End Class ' inner class AccessibleAWTListChild

		End Class ' inner class AccessibleAWTList

	End Class

End Namespace
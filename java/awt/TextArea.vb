Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
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
	''' A <code>TextArea</code> object is a multi-line region
	''' that displays text. It can be set to allow editing or
	''' to be read-only.
	''' <p>
	''' The following image shows the appearance of a text area:
	''' <p>
	''' <img src="doc-files/TextArea-1.gif" alt="A TextArea showing the word 'Hello!'"
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' This text area could be created by the following line of code:
	''' 
	''' <hr><blockquote><pre>
	''' new TextArea("Hello", 5, 40);
	''' </pre></blockquote><hr>
	''' <p>
	''' @author      Sami Shaio
	''' @since       JDK1.0
	''' </summary>
	Public Class TextArea
		Inherits TextComponent

		''' <summary>
		''' The number of rows in the <code>TextArea</code>.
		''' This parameter will determine the text area's height.
		''' Guaranteed to be non-negative.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getRows() </seealso>
		''' <seealso cref= #setRows(int) </seealso>
		Friend rows As Integer

		''' <summary>
		''' The number of columns in the <code>TextArea</code>.
		''' A column is an approximate average character
		''' width that is platform-dependent.
		''' This parameter will determine the text area's width.
		''' Guaranteed to be non-negative.
		''' 
		''' @serial </summary>
		''' <seealso cref=  #setColumns(int) </seealso>
		''' <seealso cref=  #getColumns() </seealso>
		Friend columns As Integer

		Private Const base As String = "text"
		Private Shared nameCounter As Integer = 0

		''' <summary>
		''' Create and display both vertical and horizontal scrollbars.
		''' @since JDK1.1
		''' </summary>
		Public Const SCROLLBARS_BOTH As Integer = 0

		''' <summary>
		''' Create and display vertical scrollbar only.
		''' @since JDK1.1
		''' </summary>
		Public Const SCROLLBARS_VERTICAL_ONLY As Integer = 1

		''' <summary>
		''' Create and display horizontal scrollbar only.
		''' @since JDK1.1
		''' </summary>
		Public Const SCROLLBARS_HORIZONTAL_ONLY As Integer = 2

		''' <summary>
		''' Do not create or display any scrollbars for the text area.
		''' @since JDK1.1
		''' </summary>
		Public Const SCROLLBARS_NONE As Integer = 3

		''' <summary>
		''' Determines which scrollbars are created for the
		''' text area. It can be one of four values :
		''' <code>SCROLLBARS_BOTH</code> = both scrollbars.<BR>
		''' <code>SCROLLBARS_HORIZONTAL_ONLY</code> = Horizontal bar only.<BR>
		''' <code>SCROLLBARS_VERTICAL_ONLY</code> = Vertical bar only.<BR>
		''' <code>SCROLLBARS_NONE</code> = No scrollbars.<BR>
		''' 
		''' @serial </summary>
		''' <seealso cref= #getScrollbarVisibility() </seealso>
		Private scrollbarVisibility As Integer

		''' <summary>
		''' Cache the Sets of forward and backward traversal keys so we need not
		''' look them up each time.
		''' </summary>
		Private Shared forwardTraversalKeys As java.util.Set(Of AWTKeyStroke), backwardTraversalKeys As java.util.Set(Of AWTKeyStroke)

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = 3692302836626095722L

		''' <summary>
		''' Initialize JNI field and method ids
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
			forwardTraversalKeys = KeyboardFocusManager.initFocusTraversalKeysSet("ctrl TAB", New HashSet(Of AWTKeyStroke))
			backwardTraversalKeys = KeyboardFocusManager.initFocusTraversalKeysSet("ctrl shift TAB", New HashSet(Of AWTKeyStroke))
		End Sub

		''' <summary>
		''' Constructs a new text area with the empty string as text.
		''' This text area is created with scrollbar visibility equal to
		''' <seealso cref="#SCROLLBARS_BOTH"/>, so both vertical and horizontal
		''' scrollbars will be visible for this text area. </summary>
		''' <exception cref="HeadlessException"> if
		'''    <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		Public Sub New()
			Me.New("", 0, 0, SCROLLBARS_BOTH)
		End Sub

		''' <summary>
		''' Constructs a new text area with the specified text.
		''' This text area is created with scrollbar visibility equal to
		''' <seealso cref="#SCROLLBARS_BOTH"/>, so both vertical and horizontal
		''' scrollbars will be visible for this text area. </summary>
		''' <param name="text">       the text to be displayed; if
		'''             <code>text</code> is <code>null</code>, the empty
		'''             string <code>""</code> will be displayed </param>
		''' <exception cref="HeadlessException"> if
		'''        <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		Public Sub New(  text As String)
			Me.New(text, 0, 0, SCROLLBARS_BOTH)
		End Sub

		''' <summary>
		''' Constructs a new text area with the specified number of
		''' rows and columns and the empty string as text.
		''' A column is an approximate average character
		''' width that is platform-dependent.  The text area is created with
		''' scrollbar visibility equal to <seealso cref="#SCROLLBARS_BOTH"/>, so both
		''' vertical and horizontal scrollbars will be visible for this
		''' text area. </summary>
		''' <param name="rows"> the number of rows </param>
		''' <param name="columns"> the number of columns </param>
		''' <exception cref="HeadlessException"> if
		'''     <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		Public Sub New(  rows As Integer,   columns As Integer)
			Me.New("", rows, columns, SCROLLBARS_BOTH)
		End Sub

		''' <summary>
		''' Constructs a new text area with the specified text,
		''' and with the specified number of rows and columns.
		''' A column is an approximate average character
		''' width that is platform-dependent.  The text area is created with
		''' scrollbar visibility equal to <seealso cref="#SCROLLBARS_BOTH"/>, so both
		''' vertical and horizontal scrollbars will be visible for this
		''' text area. </summary>
		''' <param name="text">       the text to be displayed; if
		'''             <code>text</code> is <code>null</code>, the empty
		'''             string <code>""</code> will be displayed </param>
		''' <param name="rows">      the number of rows </param>
		''' <param name="columns">   the number of columns </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		Public Sub New(  text As String,   rows As Integer,   columns As Integer)
			Me.New(text, rows, columns, SCROLLBARS_BOTH)
		End Sub

		''' <summary>
		''' Constructs a new text area with the specified text,
		''' and with the rows, columns, and scroll bar visibility
		''' as specified.  All <code>TextArea</code> constructors defer to
		''' this one.
		''' <p>
		''' The <code>TextArea</code> class defines several constants
		''' that can be supplied as values for the
		''' <code>scrollbars</code> argument:
		''' <ul>
		''' <li><code>SCROLLBARS_BOTH</code>,
		''' <li><code>SCROLLBARS_VERTICAL_ONLY</code>,
		''' <li><code>SCROLLBARS_HORIZONTAL_ONLY</code>,
		''' <li><code>SCROLLBARS_NONE</code>.
		''' </ul>
		''' Any other value for the
		''' <code>scrollbars</code> argument is invalid and will result in
		''' this text area being created with scrollbar visibility equal to
		''' the default value of <seealso cref="#SCROLLBARS_BOTH"/>. </summary>
		''' <param name="text">       the text to be displayed; if
		'''             <code>text</code> is <code>null</code>, the empty
		'''             string <code>""</code> will be displayed </param>
		''' <param name="rows">       the number of rows; if
		'''             <code>rows</code> is less than <code>0</code>,
		'''             <code>rows</code> is set to <code>0</code> </param>
		''' <param name="columns">    the number of columns; if
		'''             <code>columns</code> is less than <code>0</code>,
		'''             <code>columns</code> is set to <code>0</code> </param>
		''' <param name="scrollbars">  a constant that determines what
		'''             scrollbars are created to view the text area
		''' @since      JDK1.1 </param>
		''' <exception cref="HeadlessException"> if
		'''    <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		Public Sub New(  text As String,   rows As Integer,   columns As Integer,   scrollbars As Integer)
			MyBase.New(text)

			Me.rows = If(rows >= 0, rows, 0)
			Me.columns = If(columns >= 0, columns, 0)

			If scrollbars >= SCROLLBARS_BOTH AndAlso scrollbars <= SCROLLBARS_NONE Then
				Me.scrollbarVisibility = scrollbars
			Else
				Me.scrollbarVisibility = SCROLLBARS_BOTH
			End If

			focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, forwardTraversalKeys)
			focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, backwardTraversalKeys)
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by <code>getName</code>
		''' when the name is <code>null</code>.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(TextArea)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the <code>TextArea</code>'s peer.  The peer allows us to modify
		''' the appearance of the <code>TextArea</code> without changing any of its
		''' functionality.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createTextArea(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Inserts the specified text at the specified position
		''' in this text area.
		''' <p>Note that passing <code>null</code> or inconsistent
		''' parameters is invalid and will result in unspecified
		''' behavior.
		''' </summary>
		''' <param name="str"> the non-<code>null</code> text to insert </param>
		''' <param name="pos"> the position at which to insert </param>
		''' <seealso cref=        java.awt.TextComponent#setText </seealso>
		''' <seealso cref=        java.awt.TextArea#replaceRange </seealso>
		''' <seealso cref=        java.awt.TextArea#append
		''' @since      JDK1.1 </seealso>
		Public Overridable Sub insert(  str As String,   pos As Integer)
			insertText(str, pos)
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>insert(String, int)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub insertText(  str As String,   pos As Integer)
			Dim peer_Renamed As java.awt.peer.TextAreaPeer = CType(Me.peer, java.awt.peer.TextAreaPeer)
			If peer_Renamed IsNot Nothing Then
				peer_Renamed.insert(str, pos)
			Else
				text = text.Substring(0, pos) + str + text.Substring(pos)
			End If
		End Sub

		''' <summary>
		''' Appends the given text to the text area's current text.
		''' <p>Note that passing <code>null</code> or inconsistent
		''' parameters is invalid and will result in unspecified
		''' behavior.
		''' </summary>
		''' <param name="str"> the non-<code>null</code> text to append </param>
		''' <seealso cref=       java.awt.TextArea#insert
		''' @since     JDK1.1 </seealso>
		Public Overridable Sub append(  str As String)
			appendText(str)
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>append(String)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub appendText(  str As String)
			If peer IsNot Nothing Then
				insertText(str, text.length())
			Else
				text = text + str
			End If
		End Sub

		''' <summary>
		''' Replaces text between the indicated start and end positions
		''' with the specified replacement text.  The text at the end
		''' position will not be replaced.  The text at the start
		''' position will be replaced (unless the start position is the
		''' same as the end position).
		''' The text position is zero-based.  The inserted substring may be
		''' of a different length than the text it replaces.
		''' <p>Note that passing <code>null</code> or inconsistent
		''' parameters is invalid and will result in unspecified
		''' behavior.
		''' </summary>
		''' <param name="str">      the non-<code>null</code> text to use as
		'''                     the replacement </param>
		''' <param name="start">    the start position </param>
		''' <param name="end">      the end position </param>
		''' <seealso cref=       java.awt.TextArea#insert
		''' @since     JDK1.1 </seealso>
		Public Overridable Sub replaceRange(  str As String,   start As Integer,   [end] As Integer)
			replaceText(str, start, [end])
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>replaceRange(String, int, int)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub replaceText(  str As String,   start As Integer,   [end] As Integer)
			Dim peer_Renamed As java.awt.peer.TextAreaPeer = CType(Me.peer, java.awt.peer.TextAreaPeer)
			If peer_Renamed IsNot Nothing Then
				peer_Renamed.replaceRange(str, start, [end])
			Else
				text = text.Substring(0, start) + str + text.Substring([end])
			End If
		End Sub

		''' <summary>
		''' Returns the number of rows in the text area. </summary>
		''' <returns>    the number of rows in the text area </returns>
		''' <seealso cref=       #setRows(int) </seealso>
		''' <seealso cref=       #getColumns()
		''' @since     JDK1 </seealso>
		Public Overridable Property rows As Integer
			Get
				Return rows
			End Get
			Set(  rows As Integer)
				Dim oldVal As Integer = Me.rows
				If rows < 0 Then Throw New IllegalArgumentException("rows less than zero.")
				If rows <> oldVal Then
					Me.rows = rows
					invalidate()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the number of columns in this text area. </summary>
		''' <returns>    the number of columns in the text area </returns>
		''' <seealso cref=       #setColumns(int) </seealso>
		''' <seealso cref=       #getRows() </seealso>
		Public Overridable Property columns As Integer
			Get
				Return columns
			End Get
			Set(  columns As Integer)
				Dim oldVal As Integer = Me.columns
				If columns < 0 Then Throw New IllegalArgumentException("columns less than zero.")
				If columns <> oldVal Then
					Me.columns = columns
					invalidate()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns an enumerated value that indicates which scroll bars
		''' the text area uses.
		''' <p>
		''' The <code>TextArea</code> class defines four integer constants
		''' that are used to specify which scroll bars are available.
		''' <code>TextArea</code> has one constructor that gives the
		''' application discretion over scroll bars.
		''' </summary>
		''' <returns>     an integer that indicates which scroll bars are used </returns>
		''' <seealso cref=        java.awt.TextArea#SCROLLBARS_BOTH </seealso>
		''' <seealso cref=        java.awt.TextArea#SCROLLBARS_VERTICAL_ONLY </seealso>
		''' <seealso cref=        java.awt.TextArea#SCROLLBARS_HORIZONTAL_ONLY </seealso>
		''' <seealso cref=        java.awt.TextArea#SCROLLBARS_NONE </seealso>
		''' <seealso cref=        java.awt.TextArea#TextArea(java.lang.String, int, int, int)
		''' @since      JDK1.1 </seealso>
		Public Overridable Property scrollbarVisibility As Integer
			Get
				Return scrollbarVisibility
			End Get
		End Property


		''' <summary>
		''' Determines the preferred size of a text area with the specified
		''' number of rows and columns. </summary>
		''' <param name="rows">   the number of rows </param>
		''' <param name="columns">   the number of columns </param>
		''' <returns>    the preferred dimensions required to display
		'''                       the text area with the specified
		'''                       number of rows and columns </returns>
		''' <seealso cref=       java.awt.Component#getPreferredSize
		''' @since     JDK1.1 </seealso>
		Public Overridable Function getPreferredSize(  rows As Integer,   columns As Integer) As Dimension
			Return preferredSize(rows, columns)
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getPreferredSize(int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function preferredSize(  rows As Integer,   columns As Integer) As Dimension
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.TextAreaPeer = CType(Me.peer, java.awt.peer.TextAreaPeer)
				Return If(peer_Renamed IsNot Nothing, peer_Renamed.getPreferredSize(rows, columns), MyBase.preferredSize())
			End SyncLock
		End Function

		''' <summary>
		''' Determines the preferred size of this text area. </summary>
		''' <returns>    the preferred dimensions needed for this text area </returns>
		''' <seealso cref=       java.awt.Component#getPreferredSize
		''' @since     JDK1.1 </seealso>
		Public  Overrides ReadOnly Property  preferredSize As Dimension
			Get
				Return preferredSize()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getPreferredSize()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function preferredSize() As Dimension
			SyncLock treeLock
				Return If((rows > 0) AndAlso (columns > 0), preferredSize(rows, columns), MyBase.preferredSize())
			End SyncLock
		End Function

		''' <summary>
		''' Determines the minimum size of a text area with the specified
		''' number of rows and columns. </summary>
		''' <param name="rows">   the number of rows </param>
		''' <param name="columns">   the number of columns </param>
		''' <returns>    the minimum dimensions required to display
		'''                       the text area with the specified
		'''                       number of rows and columns </returns>
		''' <seealso cref=       java.awt.Component#getMinimumSize
		''' @since     JDK1.1 </seealso>
		Public Overridable Function getMinimumSize(  rows As Integer,   columns As Integer) As Dimension
			Return minimumSize(rows, columns)
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getMinimumSize(int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function minimumSize(  rows As Integer,   columns As Integer) As Dimension
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.TextAreaPeer = CType(Me.peer, java.awt.peer.TextAreaPeer)
				Return If(peer_Renamed IsNot Nothing, peer_Renamed.getMinimumSize(rows, columns), MyBase.minimumSize())
			End SyncLock
		End Function

		''' <summary>
		''' Determines the minimum size of this text area. </summary>
		''' <returns>    the preferred dimensions needed for this text area </returns>
		''' <seealso cref=       java.awt.Component#getPreferredSize
		''' @since     JDK1.1 </seealso>
		Public  Overrides ReadOnly Property  minimumSize As Dimension
			Get
				Return minimumSize()
			End Get
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getMinimumSize()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Function minimumSize() As Dimension
			SyncLock treeLock
				Return If((rows > 0) AndAlso (columns > 0), minimumSize(rows, columns), MyBase.minimumSize())
			End SyncLock
		End Function

		''' <summary>
		''' Returns a string representing the state of this <code>TextArea</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>      the parameter string of this text area </returns>
		Protected Friend Overrides Function paramString() As String
			Dim sbVisStr As String
			Select Case scrollbarVisibility
				Case SCROLLBARS_BOTH
					sbVisStr = "both"
				Case SCROLLBARS_VERTICAL_ONLY
					sbVisStr = "vertical-only"
				Case SCROLLBARS_HORIZONTAL_ONLY
					sbVisStr = "horizontal-only"
				Case SCROLLBARS_NONE
					sbVisStr = "none"
				Case Else
					sbVisStr = "invalid display policy"
			End Select

			Return MyBase.paramString() & ",rows=" & rows & ",columns=" & columns & ",scrollbarVisibility=" & sbVisStr
		End Function


	'    
	'     * Serialization support.
	'     
		''' <summary>
		''' The textArea Serialized Data Version.
		''' 
		''' @serial
		''' </summary>
		Private textAreaSerializedDataVersion As Integer = 2

		''' <summary>
		''' Read the ObjectInputStream. </summary>
		''' <exception cref="HeadlessException"> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns
		''' <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			' HeadlessException will be thrown by TextComponent's readObject
			s.defaultReadObject()

			' Make sure the state we just read in for columns, rows,
			' and scrollbarVisibility has legal values
			If columns < 0 Then columns = 0
			If rows < 0 Then rows = 0

			If (scrollbarVisibility < SCROLLBARS_BOTH) OrElse (scrollbarVisibility > SCROLLBARS_NONE) Then Me.scrollbarVisibility = SCROLLBARS_BOTH

			If textAreaSerializedDataVersion < 2 Then
				focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, forwardTraversalKeys)
				focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, backwardTraversalKeys)
			End If
		End Sub


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Returns the <code>AccessibleContext</code> associated with
		''' this <code>TextArea</code>. For text areas, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleAWTTextArea</code>.
		''' A new <code>AccessibleAWTTextArea</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleAWTTextArea</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>TextArea</code>
		''' @since 1.3 </returns>
		Public  Overrides ReadOnly Property  accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTTextArea(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>TextArea</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to text area user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTTextArea
			Inherits AccessibleAWTTextComponent

			Private ReadOnly outerInstance As TextArea

			Public Sub New(  outerInstance As TextArea)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 3472827823632144419L

			''' <summary>
			''' Gets the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet describing the states
			''' of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					states.add(AccessibleState.MULTI_LINE)
					Return states
				End Get
			End Property
		End Class


	End Class

End Namespace
Imports System
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.accessibility

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

Namespace javax.swing.table






	''' <summary>
	''' This is the object which manages the header of the <code>JTable</code>.
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
	''' @author Alan Chung
	''' @author Philip Milne </summary>
	''' <seealso cref= javax.swing.JTable </seealso>
	Public Class JTableHeader
		Inherits JComponent
		Implements TableColumnModelListener, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TableHeaderUI"

	'
	' Instance Variables
	'
		''' <summary>
		''' The table for which this object is the header;
		''' the default is <code>null</code>.
		''' </summary>
		Protected Friend table As JTable

		''' <summary>
		''' The <code>TableColumnModel</code> of the table header.
		''' </summary>
		Protected Friend columnModel As TableColumnModel

		''' <summary>
		''' If true, reordering of columns are allowed by the user;
		''' the default is true.
		''' </summary>
		Protected Friend reorderingAllowed As Boolean

		''' <summary>
		''' If true, resizing of columns are allowed by the user;
		''' the default is true.
		''' </summary>
		Protected Friend resizingAllowed As Boolean

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Real time repaints, in response
		''' to column dragging or resizing, are now unconditional.
		''' </summary>
	'    
	'     * If this flag is true, then the header will repaint the table as
	'     * a column is dragged or resized; the default is true.
	'     
		Protected Friend updateTableInRealTime As Boolean

		''' <summary>
		''' The index of the column being resized. <code>null</code> if not resizing. </summary>
		<NonSerialized> _
		Protected Friend resizingColumn As TableColumn

		''' <summary>
		''' The index of the column being dragged. <code>null</code> if not dragging. </summary>
		<NonSerialized> _
		Protected Friend draggedColumn As TableColumn

		''' <summary>
		''' The distance from its original position the column has been dragged. </summary>
		<NonSerialized> _
		Protected Friend draggedDistance As Integer

		''' <summary>
		'''  The default renderer to be used when a <code>TableColumn</code>
		'''  does not define a <code>headerRenderer</code>.
		''' </summary>
		Private defaultRenderer As TableCellRenderer

	'
	' Constructors
	'

		''' <summary>
		'''  Constructs a <code>JTableHeader</code> with a default
		'''  <code>TableColumnModel</code>.
		''' </summary>
		''' <seealso cref= #createDefaultColumnModel </seealso>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		'''  Constructs a <code>JTableHeader</code> which is initialized with
		'''  <code>cm</code> as the column model.  If <code>cm</code> is
		'''  <code>null</code> this method will initialize the table header
		'''  with a default <code>TableColumnModel</code>.
		''' </summary>
		''' <param name="cm">        the column model for the table </param>
		''' <seealso cref= #createDefaultColumnModel </seealso>
		Public Sub New(ByVal cm As TableColumnModel)
			MyBase.New()

			'setFocusable(false); // for strict win/mac compatibility mode,
								   ' this method should be invoked

			If cm Is Nothing Then cm = createDefaultColumnModel()
			columnModel = cm

			' Initialize local ivars
			initializeLocalVars()

			' Get UI going
			updateUI()
		End Sub

	'
	' Local behavior attributes
	'

		''' <summary>
		'''  Sets the table associated with this header. </summary>
		'''  <param name="table">   the new table
		'''  @beaninfo
		'''   bound: true
		'''   description: The table associated with this header. </param>
		Public Overridable Property table As JTable
			Set(ByVal table As JTable)
				Dim old As JTable = Me.table
				Me.table = table
				firePropertyChange("table", old, table)
			End Set
			Get
				Return table
			End Get
		End Property


		''' <summary>
		'''  Sets whether the user can drag column headers to reorder columns.
		''' </summary>
		''' <param name="reorderingAllowed">       true if the table view should allow
		'''                                  reordering; otherwise false </param>
		''' <seealso cref=     #getReorderingAllowed
		''' @beaninfo
		'''  bound: true
		'''  description: Whether the user can drag column headers to reorder columns. </seealso>
		Public Overridable Property reorderingAllowed As Boolean
			Set(ByVal reorderingAllowed As Boolean)
				Dim old As Boolean = Me.reorderingAllowed
				Me.reorderingAllowed = reorderingAllowed
				firePropertyChange("reorderingAllowed", old, reorderingAllowed)
			End Set
			Get
				Return reorderingAllowed
			End Get
		End Property


		''' <summary>
		'''  Sets whether the user can resize columns by dragging between headers.
		''' </summary>
		''' <param name="resizingAllowed">         true if table view should allow
		'''                                  resizing </param>
		''' <seealso cref=     #getResizingAllowed
		''' @beaninfo
		'''  bound: true
		'''  description: Whether the user can resize columns by dragging between headers. </seealso>
		Public Overridable Property resizingAllowed As Boolean
			Set(ByVal resizingAllowed As Boolean)
				Dim old As Boolean = Me.resizingAllowed
				Me.resizingAllowed = resizingAllowed
				firePropertyChange("resizingAllowed", old, resizingAllowed)
			End Set
			Get
				Return resizingAllowed
			End Get
		End Property


		''' <summary>
		''' Returns the the dragged column, if and only if, a drag is in
		''' process, otherwise returns <code>null</code>.
		''' </summary>
		''' <returns>  the dragged column, if a drag is in
		'''          process, otherwise returns <code>null</code> </returns>
		''' <seealso cref=     #getDraggedDistance </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getDraggedColumn() As TableColumn 'JavaToDotNetTempPropertyGetdraggedColumn
		Public Overridable Property draggedColumn As TableColumn
			Get
				Return draggedColumn
			End Get
			Set(ByVal aColumn As TableColumn)
		End Property

		''' <summary>
		''' Returns the column's horizontal distance from its original
		''' position, if and only if, a drag is in process. Otherwise, the
		''' the return value is meaningless.
		''' </summary>
		''' <returns>  the column's horizontal distance from its original
		'''          position, if a drag is in process, otherwise the return
		'''          value is meaningless </returns>
		''' <seealso cref=     #getDraggedColumn </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getDraggedDistance() As Integer 'JavaToDotNetTempPropertyGetdraggedDistance
		Public Overridable Property draggedDistance As Integer
			Get
				Return draggedDistance
			End Get
			Set(ByVal distance As Integer)
		End Property

		''' <summary>
		''' Returns the resizing column.  If no column is being
		''' resized this method returns <code>null</code>.
		''' </summary>
		''' <returns>  the resizing column, if a resize is in process, otherwise
		'''          returns <code>null</code> </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getResizingColumn() As TableColumn 'JavaToDotNetTempPropertyGetresizingColumn
		Public Overridable Property resizingColumn As TableColumn
			Get
				Return resizingColumn
			End Get
			Set(ByVal aColumn As TableColumn)
		End Property

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Real time repaints, in response to
		''' column dragging or resizing, are now unconditional.
		''' </summary>
	'    
	'     *  Sets whether the body of the table updates in real time when
	'     *  a column is resized or dragged.
	'     *
	'     * @param   flag                    true if tableView should update
	'     *                                  the body of the table in real time
	'     * @see #getUpdateTableInRealTime
	'     
		Public Overridable Property updateTableInRealTime As Boolean
			Set(ByVal flag As Boolean)
				updateTableInRealTime = flag
			End Set
			Get
				Return updateTableInRealTime
			End Get
		End Property

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Real time repaints, in response to
		''' column dragging or resizing, are now unconditional.
		''' </summary>
	'    
	'     * Returns true if the body of the table view updates in real
	'     * time when a column is resized or dragged.  User can set this flag to
	'     * false to speed up the table's response to user resize or drag actions.
	'     * The default is true.
	'     *
	'     * @return  true if the table updates in real time
	'     * @see #setUpdateTableInRealTime
	'     

		''' <summary>
		''' Sets the default renderer to be used when no <code>headerRenderer</code>
		''' is defined by a <code>TableColumn</code>. </summary>
		''' <param name="defaultRenderer">  the default renderer
		''' @since 1.3 </param>
		Public Overridable Property defaultRenderer As TableCellRenderer
			Set(ByVal defaultRenderer As TableCellRenderer)
				Me.defaultRenderer = defaultRenderer
			End Set
			Get
				Return defaultRenderer
			End Get
		End Property

		''' <summary>
		''' Returns the default renderer used when no <code>headerRenderer</code>
		''' is defined by a <code>TableColumn</code>. </summary>
		''' <returns> the default renderer
		''' @since 1.3 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Returns the index of the column that <code>point</code> lies in, or -1 if it
		''' lies out of bounds.
		''' </summary>
		''' <returns>  the index of the column that <code>point</code> lies in, or -1 if it
		'''          lies out of bounds </returns>
		Public Overridable Function columnAtPoint(ByVal point As Point) As Integer
			Dim ___x As Integer = point.x
			If Not componentOrientation.leftToRight Then ___x = widthInRightToLeft - ___x - 1
			Return columnModel.getColumnIndexAtX(___x)
		End Function

		''' <summary>
		''' Returns the rectangle containing the header tile at <code>column</code>.
		''' When the <code>column</code> parameter is out of bounds this method uses the
		''' same conventions as the <code>JTable</code> method <code>getCellRect</code>.
		''' </summary>
		''' <returns>  the rectangle containing the header tile at <code>column</code> </returns>
		''' <seealso cref= JTable#getCellRect </seealso>
		Public Overridable Function getHeaderRect(ByVal column As Integer) As Rectangle
			Dim r As New Rectangle
			Dim cm As TableColumnModel = columnModel

			r.height = height

			If column < 0 Then
				' x = width = 0;
				If Not componentOrientation.leftToRight Then r.x = widthInRightToLeft
			ElseIf column >= cm.columnCount Then
				If componentOrientation.leftToRight Then r.x = width
			Else
				For i As Integer = 0 To column - 1
					r.x += cm.getColumn(i).width
				Next i
				If Not componentOrientation.leftToRight Then r.x = widthInRightToLeft - r.x - cm.getColumn(column).width

				r.width = cm.getColumn(column).width
			End If
			Return r
		End Function


		''' <summary>
		''' Allows the renderer's tips to be used if there is text set. </summary>
		''' <param name="event">  the location of the event identifies the proper
		'''                          renderer and, therefore, the proper tip </param>
		''' <returns> the tool tip for this component </returns>
		Public Overrides Function getToolTipText(ByVal [event] As MouseEvent) As String
			Dim tip As String = Nothing
			Dim p As Point = [event].point
			Dim column As Integer

			' Locate the renderer under the event location
			column = columnAtPoint(p)
			If column <> -1 Then
				Dim aColumn As TableColumn = columnModel.getColumn(column)
				Dim renderer As TableCellRenderer = aColumn.headerRenderer
				If renderer Is Nothing Then renderer = defaultRenderer
				Dim component As Component = renderer.getTableCellRendererComponent(table, aColumn.headerValue, False, False, -1, column)

				' Now have to see if the component is a JComponent before
				' getting the tip
				If TypeOf component Is JComponent Then
					' Convert the event to the renderer's coordinate system
					Dim newEvent As MouseEvent
					Dim cellRect As Rectangle = getHeaderRect(column)

					p.translate(-cellRect.x, -cellRect.y)
					newEvent = New MouseEvent(component, [event].iD, [event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)

					tip = CType(component, JComponent).getToolTipText(newEvent)
				End If
			End If

			' No tip from the renderer get our own tip
			If tip Is Nothing Then tip = toolTipText

			Return tip
		End Function

	'
	' Managing TableHeaderUI
	'

		''' <summary>
		''' Returns the look and feel (L&amp;F) object that renders this component.
		''' </summary>
		''' <returns> the <code>TableHeaderUI</code> object that renders this component </returns>
		Public Overridable Property uI As TableHeaderUI
			Get
				Return CType(ui, TableHeaderUI)
			End Get
			Set(ByVal ui As TableHeaderUI)
				If Me.ui IsNot ui Then
					MyBase.uI = ui
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the look and feel
		''' (L&amp;F) has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), TableHeaderUI)

			Dim renderer As TableCellRenderer = defaultRenderer
			If TypeOf renderer Is Component Then SwingUtilities.updateComponentTreeUI(CType(renderer, Component))
		End Sub


		''' <summary>
		''' Returns the suffix used to construct the name of the look and feel
		''' (L&amp;F) class used to render this component. </summary>
		''' <returns> the string "TableHeaderUI"
		''' </returns>
		''' <returns> "TableHeaderUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


	'
	' Managing models
	'


		''' <summary>
		'''  Sets the column model for this table to <code>newModel</code> and registers
		'''  for listener notifications from the new column model.
		''' </summary>
		''' <param name="columnModel">     the new data source for this table </param>
		''' <exception cref="IllegalArgumentException">
		'''                          if <code>newModel</code> is <code>null</code> </exception>
		''' <seealso cref=     #getColumnModel
		''' @beaninfo
		'''  bound: true
		'''  description: The object governing the way columns appear in the view. </seealso>
		Public Overridable Property columnModel As TableColumnModel
			Set(ByVal columnModel As TableColumnModel)
				If columnModel Is Nothing Then Throw New System.ArgumentException("Cannot set a null ColumnModel")
				Dim old As TableColumnModel = Me.columnModel
				If columnModel IsNot old Then
					If old IsNot Nothing Then old.removeColumnModelListener(Me)
					Me.columnModel = columnModel
					columnModel.addColumnModelListener(Me)
    
					firePropertyChange("columnModel", old, columnModel)
					resizeAndRepaint()
				End If
			End Set
			Get
				Return columnModel
			End Get
		End Property


	'
	' Implementing TableColumnModelListener interface
	'

		''' <summary>
		''' Invoked when a column is added to the table column model.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by <code>JTable</code>.
		''' </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnAdded(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnAdded
			resizeAndRepaint()
		End Sub


		''' <summary>
		''' Invoked when a column is removed from the table column model.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by <code>JTable</code>.
		''' </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnRemoved(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnRemoved
			resizeAndRepaint()
		End Sub


		''' <summary>
		''' Invoked when a column is repositioned.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by <code>JTable</code>.
		''' </summary>
		''' <param name="e"> the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnMoved(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnMoved
			repaint()
		End Sub


		''' <summary>
		''' Invoked when a column is moved due to a margin change.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by <code>JTable</code>.
		''' </summary>
		''' <param name="e"> the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnMarginChanged(ByVal e As ChangeEvent)
			resizeAndRepaint()
		End Sub


		' --Redrawing the header is slow in cell selection mode.
		' --Since header selection is ugly and it is always clear from the
		' --view which columns are selected, don't redraw the header.
		''' <summary>
		''' Invoked when the selection model of the <code>TableColumnModel</code>
		''' is changed.  This method currently has no effect (the header is not
		''' redrawn).
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by <code>JTable</code>.
		''' </summary>
		''' <param name="e"> the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnSelectionChanged(ByVal e As ListSelectionEvent) ' repaint(); }
		End Sub

	'
	'  Package Methods
	'

		''' <summary>
		'''  Returns the default column model object which is
		'''  a <code>DefaultTableColumnModel</code>.  A subclass can override this
		'''  method to return a different column model object
		''' </summary>
		''' <returns> the default column model object </returns>
		Protected Friend Overridable Function createDefaultColumnModel() As TableColumnModel
			Return New DefaultTableColumnModel
		End Function

		''' <summary>
		'''  Returns a default renderer to be used when no header renderer
		'''  is defined by a <code>TableColumn</code>.
		''' </summary>
		'''  <returns> the default table column renderer
		''' @since 1.3 </returns>
		Protected Friend Overridable Function createDefaultRenderer() As TableCellRenderer
			Return New sun.swing.table.DefaultTableCellHeaderRenderer
		End Function


		''' <summary>
		''' Initializes the local variables and properties with default values.
		''' Used by the constructor methods.
		''' </summary>
		Protected Friend Overridable Sub initializeLocalVars()
			opaque = True
			table = Nothing
			reorderingAllowed = True
			resizingAllowed = True
			draggedColumn = Nothing
			draggedDistance = 0
			resizingColumn = Nothing
			updateTableInRealTime = True

			' I'm registered to do tool tips so we can draw tips for the
			' renderers
			Dim ___toolTipManager As ToolTipManager = ToolTipManager.sharedInstance()
			___toolTipManager.registerComponent(Me)
			defaultRenderer = createDefaultRenderer()
		End Sub

		''' <summary>
		''' Sizes the header and marks it as needing display.  Equivalent
		''' to <code>revalidate</code> followed by <code>repaint</code>.
		''' </summary>
		Public Overridable Sub resizeAndRepaint()
			revalidate()
			repaint()
		End Sub

			draggedColumn = aColumn
		End Sub

			draggedDistance = distance
		End Sub

			resizingColumn = aColumn
		End Sub

		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If (ui IsNot Nothing) AndAlso (uIClassID.Equals(uiClassID)) Then ui.installUI(Me)
		End Sub

		Private Property widthInRightToLeft As Integer
			Get
				If (table IsNot Nothing) AndAlso (table.autoResizeMode <> JTable.AUTO_RESIZE_OFF) Then Return table.width
				Return MyBase.width
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this <code>JTableHeader</code>. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' <P>
		''' Overriding <code>paramString</code> to provide information about the
		''' specific new aspects of the JFC components.
		''' </summary>
		''' <returns>  a string representation of this <code>JTableHeader</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim reorderingAllowedString As String = (If(reorderingAllowed, "true", "false"))
			Dim resizingAllowedString As String = (If(resizingAllowed, "true", "false"))
			Dim updateTableInRealTimeString As String = (If(updateTableInRealTime, "true", "false"))

			Return MyBase.paramString() & ",draggedDistance=" & draggedDistance & ",reorderingAllowed=" & reorderingAllowedString & ",resizingAllowed=" & resizingAllowedString & ",updateTableInRealTime=" & updateTableInRealTimeString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JTableHeader.
		''' For JTableHeaders, the AccessibleContext takes the form of an
		''' AccessibleJTableHeader.
		''' A new AccessibleJTableHeader instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJTableHeader that serves as the
		'''         AccessibleContext of this JTableHeader </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJTableHeader(Me)
				Return accessibleContext
			End Get
		End Property

		'
		' *** should also implement AccessibleSelection?
		' *** and what's up with keyboard navigation/manipulation?
		'
		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTableHeader</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to table header user-interface
		''' elements.
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
		Protected Friend Class AccessibleJTableHeader
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JTableHeader

			Public Sub New(ByVal outerInstance As JTableHeader)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PANEL
				End Get
			End Property

			''' <summary>
			''' Returns the Accessible child, if one exists, contained at the local
			''' coordinate Point.
			''' </summary>
			''' <param name="p"> The point defining the top-left corner of the Accessible,
			''' given in the coordinate space of the object's parent. </param>
			''' <returns> the Accessible, if it exists, at the specified location;
			''' else null </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Dim column As Integer

				' Locate the renderer under the Point
				column = outerInstance.columnAtPoint(p)
				If column <> -1 Then
					Dim aColumn As TableColumn = outerInstance.columnModel.getColumn(column)
					Dim renderer As TableCellRenderer = aColumn.headerRenderer
					If renderer Is Nothing Then
						If outerInstance.defaultRenderer IsNot Nothing Then
							renderer = outerInstance.defaultRenderer
						Else
							Return Nothing
						End If
					End If
					Dim component As Component = renderer.getTableCellRendererComponent(outerInstance.table, aColumn.headerValue, False, False, -1, column)

					Return New AccessibleJTableHeaderEntry(Me, column, JTableHeader.this, outerInstance.table)
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement Accessible, than this
			''' method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return outerInstance.columnModel.columnCount
				End Get
			End Property

			''' <summary>
			''' Return the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				If i < 0 OrElse i >= accessibleChildrenCount Then
					Return Nothing
				Else
					Dim aColumn As TableColumn = outerInstance.columnModel.getColumn(i)
					Dim renderer As TableCellRenderer = aColumn.headerRenderer
					If renderer Is Nothing Then
						If outerInstance.defaultRenderer IsNot Nothing Then
							renderer = outerInstance.defaultRenderer
						Else
							Return Nothing
						End If
					End If
					Dim component As Component = renderer.getTableCellRendererComponent(outerInstance.table, aColumn.headerValue, False, False, -1, i)

					Return New AccessibleJTableHeaderEntry(Me, i, JTableHeader.this, outerInstance.table)
				End If
			End Function

		  ''' <summary>
		  ''' This class provides an implementation of the Java Accessibility
		  ''' API appropriate for JTableHeader entries.
		  ''' </summary>
			Protected Friend Class AccessibleJTableHeaderEntry
				Inherits AccessibleContext
				Implements Accessible, AccessibleComponent

				Private ReadOnly outerInstance As JTableHeader.AccessibleJTableHeader


				Private parent As JTableHeader
				Private column As Integer
				Private table As JTable

				''' <summary>
				'''  Constructs an AccessiblJTableHeaaderEntry
				''' @since 1.4
				''' </summary>
				Public Sub New(ByVal outerInstance As JTableHeader.AccessibleJTableHeader, ByVal c As Integer, ByVal p As JTableHeader, ByVal t As JTable)
						Me.outerInstance = outerInstance
					parent = p
					column = c
					table = t
					Me.accessibleParent = parent
				End Sub

				''' <summary>
				''' Get the AccessibleContext associated with this object.
				''' In the implementation of the Java Accessibility API
				''' for this class, returns this object, which serves as
				''' its own AccessibleContext.
				''' </summary>
				''' <returns> this object </returns>
				Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
					Get
						Return Me
					End Get
				End Property

				Private Property currentAccessibleContext As AccessibleContext
					Get
						Dim tcm As TableColumnModel = table.columnModel
						If tcm IsNot Nothing Then
							' Fixes 4772355 - ArrayOutOfBoundsException in
							' JTableHeader
							If column < 0 OrElse column >= tcm.columnCount Then Return Nothing
							Dim aColumn As TableColumn = tcm.getColumn(column)
							Dim renderer As TableCellRenderer = aColumn.headerRenderer
							If renderer Is Nothing Then
								If defaultRenderer IsNot Nothing Then
									renderer = defaultRenderer
								Else
									Return Nothing
								End If
							End If
							Dim c As Component = renderer.getTableCellRendererComponent(outerInstance.table, aColumn.headerValue, False, False, -1, column)
							If TypeOf c Is Accessible Then Return CType(c, Accessible).accessibleContext
						End If
						Return Nothing
					End Get
				End Property

				Private Property currentComponent As Component
					Get
						Dim tcm As TableColumnModel = table.columnModel
						If tcm IsNot Nothing Then
							' Fixes 4772355 - ArrayOutOfBoundsException in
							' JTableHeader
							If column < 0 OrElse column >= tcm.columnCount Then Return Nothing
							Dim aColumn As TableColumn = tcm.getColumn(column)
							Dim renderer As TableCellRenderer = aColumn.headerRenderer
							If renderer Is Nothing Then
								If defaultRenderer IsNot Nothing Then
									renderer = defaultRenderer
								Else
									Return Nothing
								End If
							End If
							Return renderer.getTableCellRendererComponent(outerInstance.table, aColumn.headerValue, False, False, -1, column)
						Else
							Return Nothing
						End If
					End Get
				End Property

			' AccessibleContext methods

				Public Property Overrides accessibleName As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Dim name As String = ac.accessibleName
							If (name IsNot Nothing) AndAlso (name <> "") Then Return name
						End If
						If (accessibleName IsNot Nothing) AndAlso (accessibleName <> "") Then
							Return accessibleName
						Else
							' fall back to the client property
							Dim name As String = CStr(getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
							If name IsNot Nothing Then
								Return name
							Else
								Return table.getColumnName(column)
							End If
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleName = s
						Else
							MyBase.accessibleName = s
						End If
					End Set
				End Property


				'
				' *** should check toolTip text for desc. (needs MouseEvent)
				'
				Public Property Overrides accessibleDescription As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleDescription
						Else
							Return MyBase.accessibleDescription
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleDescription = s
						Else
							MyBase.accessibleDescription = s
						End If
					End Set
				End Property


				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleRole
						Else
							Return AccessibleRole.COLUMN_HEADER
						End If
					End Get
				End Property

				Public Property Overrides accessibleStateSet As AccessibleStateSet
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Dim states As AccessibleStateSet = ac.accessibleStateSet
							If showing Then states.add(AccessibleState.SHOWING)
							Return states
						Else
							Return New AccessibleStateSet ' must be non null?
						End If
					End Get
				End Property

				Public Property Overrides accessibleIndexInParent As Integer
					Get
						Return column
					End Get
				End Property

				Public Property Overrides accessibleChildrenCount As Integer
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleChildrenCount
						Else
							Return 0
						End If
					End Get
				End Property

				Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						Dim ___accessibleChild As Accessible = ac.getAccessibleChild(i)
						ac.accessibleParent = Me
						Return ___accessibleChild
					Else
						Return Nothing
					End If
				End Function

				Public Property Overrides locale As Locale
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.locale
						Else
							Return Nothing
						End If
					End Get
				End Property

				Public Overrides Sub addPropertyChangeListener(ByVal l As java.beans.PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.addPropertyChangeListener(l)
					Else
						MyBase.addPropertyChangeListener(l)
					End If
				End Sub

				Public Overrides Sub removePropertyChangeListener(ByVal l As java.beans.PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.removePropertyChangeListener(l)
					Else
						MyBase.removePropertyChangeListener(l)
					End If
				End Sub

				Public Property Overrides accessibleAction As AccessibleAction
					Get
						Return currentAccessibleContext.accessibleAction
					End Get
				End Property

			   ''' <summary>
			   ''' Get the AccessibleComponent associated with this object.  In the
			   ''' implementation of the Java Accessibility API for this class,
			   ''' return this object, which is responsible for implementing the
			   ''' AccessibleComponent interface on behalf of itself.
			   ''' </summary>
			   ''' <returns> this object </returns>
				Public Property Overrides accessibleComponent As AccessibleComponent
					Get
						Return Me ' to override getBounds()
					End Get
				End Property

				Public Property Overrides accessibleSelection As AccessibleSelection
					Get
						Return currentAccessibleContext.accessibleSelection
					End Get
				End Property

				Public Property Overrides accessibleText As AccessibleText
					Get
						Return currentAccessibleContext.accessibleText
					End Get
				End Property

				Public Property Overrides accessibleValue As AccessibleValue
					Get
						Return currentAccessibleContext.accessibleValue
					End Get
				End Property


			' AccessibleComponent methods

				Public Overridable Property background As Color Implements AccessibleComponent.getBackground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).background
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.background
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).background = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.background = c
						End If
					End Set
				End Property


				Public Overridable Property foreground As Color Implements AccessibleComponent.getForeground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).foreground
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.foreground
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).foreground = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.foreground = c
						End If
					End Set
				End Property


				Public Overridable Property cursor As Cursor Implements AccessibleComponent.getCursor
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).cursor
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.cursor
							Else
								Dim ap As Accessible = accessibleParent
								If TypeOf ap Is AccessibleComponent Then
									Return CType(ap, AccessibleComponent).cursor
								Else
									Return Nothing
								End If
							End If
						End If
					End Get
					Set(ByVal c As Cursor)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).cursor = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.cursor = c
						End If
					End Set
				End Property


				Public Overridable Property font As Font Implements AccessibleComponent.getFont
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).font
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.font
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal f As Font)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).font = f
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.font = f
						End If
					End Set
				End Property


				Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics Implements AccessibleComponent.getFontMetrics
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getFontMetrics(f)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Return c.getFontMetrics(f)
						Else
							Return Nothing
						End If
					End If
				End Function

				Public Overridable Property enabled As Boolean Implements AccessibleComponent.isEnabled
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).enabled
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.enabled
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).enabled = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.enabled = b
						End If
					End Set
				End Property


				Public Overridable Property visible As Boolean Implements AccessibleComponent.isVisible
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).visible
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.visible
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).visible = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.visible = b
						End If
					End Set
				End Property


				Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
					Get
						If visible AndAlso outerInstance.showing Then
							Return True
						Else
							Return False
						End If
					End Get
				End Property

				Public Overridable Function contains(ByVal p As Point) As Boolean Implements AccessibleComponent.contains
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Dim r As Rectangle = CType(ac, AccessibleComponent).bounds
						Return r.contains(p)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Dim r As Rectangle = c.bounds
							Return r.contains(p)
						Else
							Return bounds.contains(p)
						End If
					End If
				End Function

				Public Overridable Property locationOnScreen As Point Implements AccessibleComponent.getLocationOnScreen
					Get
						If parent IsNot Nothing Then
							Dim parentLocation As Point = parent.locationOnScreen
							Dim componentLocation As Point = location
							componentLocation.translate(parentLocation.x, parentLocation.y)
							Return componentLocation
						Else
							Return Nothing
						End If
					End Get
				End Property

				Public Overridable Property location As Point Implements AccessibleComponent.getLocation
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Dim r As Rectangle = CType(ac, AccessibleComponent).bounds
							Return r.location
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Dim r As Rectangle = c.bounds
								Return r.location
							Else
								Return bounds.location
							End If
						End If
					End Get
					Set(ByVal p As Point)
		'                if ((parent != null)  && (parent.contains(p))) {
		'                    ensureIndexIsVisible(indexInParent);
		'                }
					End Set
				End Property


				Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
					Get
						  Dim r As Rectangle = table.getCellRect(-1, column, False)
						  r.y = 0
						  Return r
    
		'                AccessibleContext ac = getCurrentAccessibleContext();
		'                if (ac instanceof AccessibleComponent) {
		'                    return ((AccessibleComponent) ac).getBounds();
		'                } else {
		'                  Component c = getCurrentComponent();
		'                  if (c != null) {
		'                      return c.getBounds();
		'                  } else {
		'                      Rectangle r = table.getCellRect(-1, column, false);
		'                      r.y = 0;
		'                      return r;
		'                  }
		'              }
					End Get
					Set(ByVal r As Rectangle)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).bounds = r
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.bounds = r
						End If
					End Set
				End Property


				Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
					Get
						Return bounds.size
		'                AccessibleContext ac = getCurrentAccessibleContext();
		'                if (ac instanceof AccessibleComponent) {
		'                    Rectangle r = ((AccessibleComponent) ac).getBounds();
		'                    return r.getSize();
		'                } else {
		'                    Component c = getCurrentComponent();
		'                    if (c != null) {
		'                        Rectangle r = c.getBounds();
		'                        return r.getSize();
		'                    } else {
		'                        return getBounds().getSize();
		'                    }
		'                }
					End Get
					Set(ByVal d As Dimension)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).size = d
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.size = d
						End If
					End Set
				End Property


				Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible Implements AccessibleComponent.getAccessibleAt
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getAccessibleAt(p)
					Else
						Return Nothing
					End If
				End Function

				Public Overridable Property focusTraversable As Boolean Implements AccessibleComponent.isFocusTraversable
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).focusTraversable
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.focusTraversable
							Else
								Return False
							End If
						End If
					End Get
				End Property

				Public Overridable Sub requestFocus() Implements AccessibleComponent.requestFocus
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).requestFocus()
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.requestFocus()
					End If
				End Sub

				Public Overridable Sub addFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.addFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).addFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.addFocusListener(l)
					End If
				End Sub

				Public Overridable Sub removeFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.removeFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).removeFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.removeFocusListener(l)
					End If
				End Sub

			End Class ' inner class AccessibleJTableHeaderElement

		End Class ' inner class AccessibleJTableHeader

	End Class ' End of Class JTableHeader

End Namespace
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing
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

Namespace javax.swing.table


	''' <summary>
	''' The standard column-handler for a <code>JTable</code>.
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
	''' <seealso cref= JTable </seealso>
	<Serializable> _
	Public Class DefaultTableColumnModel
		Implements TableColumnModel, java.beans.PropertyChangeListener, ListSelectionListener

	'
	' Instance Variables
	'

		''' <summary>
		''' Array of TableColumn objects in this model </summary>
		Protected Friend tableColumns As List(Of TableColumn)

		''' <summary>
		''' Model for keeping track of column selections </summary>
		Protected Friend selectionModel As ListSelectionModel

		''' <summary>
		''' Width margin between each column </summary>
		Protected Friend columnMargin As Integer

		''' <summary>
		''' List of TableColumnModelListener </summary>
		Protected Friend listenerList As New EventListenerList

		''' <summary>
		''' Change event (only one needed) </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		''' <summary>
		''' Column selection allowed in this column model </summary>
		Protected Friend columnSelectionAllowed As Boolean

		''' <summary>
		''' A local cache of the combined width of all columns </summary>
		Protected Friend totalColumnWidth As Integer

	'
	' Constructors
	'
		''' <summary>
		''' Creates a default table column model.
		''' </summary>
		Public Sub New()
			MyBase.New()

			' Initialize local ivars to default
			tableColumns = New List(Of TableColumn)
			selectionModel = createSelectionModel()
			columnMargin = 1
			invalidateWidthCache()
			columnSelectionAllowed = False
		End Sub

	'
	' Modifying the model
	'

		''' <summary>
		'''  Appends <code>aColumn</code> to the end of the
		'''  <code>tableColumns</code> array.
		'''  This method also posts the <code>columnAdded</code>
		'''  event to its listeners.
		''' </summary>
		''' <param name="aColumn">         the <code>TableColumn</code> to be added </param>
		''' <exception cref="IllegalArgumentException">      if <code>aColumn</code> is
		'''                          <code>null</code> </exception>
		''' <seealso cref=     #removeColumn </seealso>
		Public Overridable Sub addColumn(ByVal aColumn As TableColumn) Implements TableColumnModel.addColumn
			If aColumn Is Nothing Then Throw New System.ArgumentException("Object is null")

			tableColumns.Add(aColumn)
			aColumn.addPropertyChangeListener(Me)
			invalidateWidthCache()

			' Post columnAdded event notification
			fireColumnAdded(New TableColumnModelEvent(Me, 0, columnCount - 1))
		End Sub

		''' <summary>
		'''  Deletes the <code>column</code> from the
		'''  <code>tableColumns</code> array.  This method will do nothing if
		'''  <code>column</code> is not in the table's columns list.
		'''  <code>tile</code> is called
		'''  to resize both the header and table views.
		'''  This method also posts a <code>columnRemoved</code>
		'''  event to its listeners.
		''' </summary>
		''' <param name="column">          the <code>TableColumn</code> to be removed </param>
		''' <seealso cref=     #addColumn </seealso>
		Public Overridable Sub removeColumn(ByVal column As TableColumn) Implements TableColumnModel.removeColumn
			Dim ___columnIndex As Integer = tableColumns.IndexOf(column)

			If ___columnIndex <> -1 Then
				' Adjust for the selection
				If selectionModel IsNot Nothing Then selectionModel.removeIndexInterval(___columnIndex,___columnIndex)

				column.removePropertyChangeListener(Me)
				tableColumns.RemoveAt(___columnIndex)
				invalidateWidthCache()

				' Post columnAdded event notification.  (JTable and JTableHeader
				' listens so they can adjust size and redraw)
				fireColumnRemoved(New TableColumnModelEvent(Me, ___columnIndex, 0))
			End If
		End Sub

		''' <summary>
		''' Moves the column and heading at <code>columnIndex</code> to
		''' <code>newIndex</code>.  The old column at <code>columnIndex</code>
		''' will now be found at <code>newIndex</code>.  The column
		''' that used to be at <code>newIndex</code> is shifted
		''' left or right to make room.  This will not move any columns if
		''' <code>columnIndex</code> equals <code>newIndex</code>.  This method
		''' also posts a <code>columnMoved</code> event to its listeners.
		''' </summary>
		''' <param name="columnIndex">                     the index of column to be moved </param>
		''' <param name="newIndex">                        new index to move the column </param>
		''' <exception cref="IllegalArgumentException">      if <code>column</code> or
		'''                                          <code>newIndex</code>
		'''                                          are not in the valid range </exception>
		Public Overridable Sub moveColumn(ByVal columnIndex As Integer, ByVal newIndex As Integer) Implements TableColumnModel.moveColumn
			If (columnIndex < 0) OrElse (columnIndex >= columnCount) OrElse (newIndex < 0) OrElse (newIndex >= columnCount) Then Throw New System.ArgumentException("moveColumn() - Index out of range")

			Dim aColumn As TableColumn

			' If the column has not yet moved far enough to change positions
			' post the event anyway, the "draggedDistance" property of the
			' tableHeader will say how far the column has been dragged.
			' Here we are really trying to get the best out of an
			' API that could do with some rethinking. We preserve backward
			' compatibility by slightly bending the meaning of these methods.
			If columnIndex = newIndex Then
				fireColumnMoved(New TableColumnModelEvent(Me, columnIndex, newIndex))
				Return
			End If
			aColumn = tableColumns(columnIndex)

			tableColumns.RemoveAt(columnIndex)
			Dim selected As Boolean = selectionModel.isSelectedIndex(columnIndex)
			selectionModel.removeIndexInterval(columnIndex,columnIndex)

			tableColumns.Insert(newIndex, aColumn)
			selectionModel.insertIndexInterval(newIndex, 1, True)
			If selected Then
				selectionModel.addSelectionInterval(newIndex, newIndex)
			Else
				selectionModel.removeSelectionInterval(newIndex, newIndex)
			End If

			fireColumnMoved(New TableColumnModelEvent(Me, columnIndex, newIndex))
		End Sub

		''' <summary>
		''' Sets the column margin to <code>newMargin</code>.  This method
		''' also posts a <code>columnMarginChanged</code> event to its
		''' listeners.
		''' </summary>
		''' <param name="newMargin">               the new margin width, in pixels </param>
		''' <seealso cref=     #getColumnMargin </seealso>
		''' <seealso cref=     #getTotalColumnWidth </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setColumnMargin(ByVal newMargin As Integer) Implements TableColumnModel.setColumnMargin 'JavaToDotNetTempPropertySetcolumnMargin
		Public Overridable Property columnMargin Implements TableColumnModel.setColumnMargin As Integer
			Set(ByVal newMargin As Integer)
				If newMargin <> columnMargin Then
					columnMargin = newMargin
					' Post columnMarginChanged event notification.
					fireColumnMarginChanged()
				End If
			End Set
			Get
		End Property

	'
	' Querying the model
	'

		''' <summary>
		''' Returns the number of columns in the <code>tableColumns</code> array.
		''' </summary>
		''' <returns>  the number of columns in the <code>tableColumns</code> array </returns>
		''' <seealso cref=     #getColumns </seealso>
		Public Overridable Property columnCount As Integer Implements TableColumnModel.getColumnCount
			Get
				Return tableColumns.Count
			End Get
		End Property

		''' <summary>
		''' Returns an <code>Enumeration</code> of all the columns in the model. </summary>
		''' <returns> an <code>Enumeration</code> of the columns in the model </returns>
		Public Overridable Property columns As System.Collections.IEnumerator(Of TableColumn) Implements TableColumnModel.getColumns
			Get
				Return tableColumns.elements()
			End Get
		End Property

		''' <summary>
		''' Returns the index of the first column in the <code>tableColumns</code>
		''' array whose identifier is equal to <code>identifier</code>,
		''' when compared using <code>equals</code>.
		''' </summary>
		''' <param name="identifier">              the identifier object </param>
		''' <returns>          the index of the first column in the
		'''                  <code>tableColumns</code> array whose identifier
		'''                  is equal to <code>identifier</code> </returns>
		''' <exception cref="IllegalArgumentException">  if <code>identifier</code>
		'''                          is <code>null</code>, or if no
		'''                          <code>TableColumn</code> has this
		'''                          <code>identifier</code> </exception>
		''' <seealso cref=             #getColumn </seealso>
		Public Overridable Function getColumnIndex(ByVal identifier As Object) As Integer Implements TableColumnModel.getColumnIndex
			If identifier Is Nothing Then Throw New System.ArgumentException("Identifier is null")

			Dim enumeration As System.Collections.IEnumerator = columns
			Dim aColumn As TableColumn
			Dim index As Integer = 0

			Do While enumeration.hasMoreElements()
				aColumn = CType(enumeration.nextElement(), TableColumn)
				' Compare them this way in case the column's identifier is null.
				If identifier.Equals(aColumn.identifier) Then Return index
				index += 1
			Loop
			Throw New System.ArgumentException("Identifier not found")
		End Function

		''' <summary>
		''' Returns the <code>TableColumn</code> object for the column
		''' at <code>columnIndex</code>.
		''' </summary>
		''' <param name="columnIndex">     the index of the column desired </param>
		''' <returns>  the <code>TableColumn</code> object for the column
		'''                          at <code>columnIndex</code> </returns>
		Public Overridable Function getColumn(ByVal columnIndex As Integer) As TableColumn Implements TableColumnModel.getColumn
			Return tableColumns(columnIndex)
		End Function

			Return columnMargin
		End Function

		''' <summary>
		''' Returns the index of the column that lies at position <code>x</code>,
		''' or -1 if no column covers this point.
		''' 
		''' In keeping with Swing's separable model architecture, a
		''' TableColumnModel does not know how the table columns actually appear on
		''' screen.  The visual presentation of the columns is the responsibility
		''' of the view/controller object using this model (typically JTable).  The
		''' view/controller need not display the columns sequentially from left to
		''' right.  For example, columns could be displayed from right to left to
		''' accommodate a locale preference or some columns might be hidden at the
		''' request of the user.  Because the model does not know how the columns
		''' are laid out on screen, the given <code>xPosition</code> should not be
		''' considered to be a coordinate in 2D graphics space.  Instead, it should
		''' be considered to be a width from the start of the first column in the
		''' model.  If the column index for a given X coordinate in 2D space is
		''' required, <code>JTable.columnAtPoint</code> can be used instead.
		''' </summary>
		''' <param name="x">  the horizontal location of interest </param>
		''' <returns>  the index of the column or -1 if no column is found </returns>
		''' <seealso cref= javax.swing.JTable#columnAtPoint </seealso>
		Public Overridable Function getColumnIndexAtX(ByVal x As Integer) As Integer Implements TableColumnModel.getColumnIndexAtX
			If x < 0 Then Return -1
			Dim cc As Integer = columnCount
			For ___column As Integer = 0 To cc - 1
				x = x - getColumn(___column).width
				If x < 0 Then Return ___column
			Next ___column
			Return -1
		End Function

		''' <summary>
		''' Returns the total combined width of all columns. </summary>
		''' <returns> the <code>totalColumnWidth</code> property </returns>
		Public Overridable Property totalColumnWidth As Integer Implements TableColumnModel.getTotalColumnWidth
			Get
				If totalColumnWidth = -1 Then recalcWidthCache()
				Return totalColumnWidth
			End Get
		End Property

	'
	' Selection model
	'

		''' <summary>
		'''  Sets the selection model for this <code>TableColumnModel</code>
		'''  to <code>newModel</code>
		'''  and registers for listener notifications from the new selection
		'''  model.  If <code>newModel</code> is <code>null</code>,
		'''  an exception is thrown.
		''' </summary>
		''' <param name="newModel">        the new selection model </param>
		''' <exception cref="IllegalArgumentException">      if <code>newModel</code>
		'''                                          is <code>null</code> </exception>
		''' <seealso cref=     #getSelectionModel </seealso>
		Public Overridable Property selectionModel Implements TableColumnModel.setSelectionModel As ListSelectionModel
			Set(ByVal newModel As ListSelectionModel)
				If newModel Is Nothing Then Throw New System.ArgumentException("Cannot set a null SelectionModel")
    
				Dim oldModel As ListSelectionModel = selectionModel
    
				If newModel IsNot oldModel Then
					If oldModel IsNot Nothing Then oldModel.removeListSelectionListener(Me)
    
					selectionModel= newModel
					newModel.addListSelectionListener(Me)
				End If
			End Set
			Get
				Return selectionModel
			End Get
		End Property


		' implements javax.swing.table.TableColumnModel
		''' <summary>
		''' Sets whether column selection is allowed.  The default is false. </summary>
		''' <param name="flag"> true if column selection will be allowed, false otherwise </param>
		Public Overridable Property columnSelectionAllowed Implements TableColumnModel.setColumnSelectionAllowed As Boolean
			Set(ByVal flag As Boolean)
				columnSelectionAllowed = flag
			End Set
			Get
				Return columnSelectionAllowed
			End Get
		End Property

		' implements javax.swing.table.TableColumnModel

		' implements javax.swing.table.TableColumnModel
		''' <summary>
		''' Returns an array of selected columns.  If <code>selectionModel</code>
		''' is <code>null</code>, returns an empty array. </summary>
		''' <returns> an array of selected columns or an empty array if nothing
		'''                  is selected or the <code>selectionModel</code> is
		'''                  <code>null</code> </returns>
		Public Overridable Property selectedColumns As Integer() Implements TableColumnModel.getSelectedColumns
			Get
				If selectionModel IsNot Nothing Then
					Dim iMin As Integer = selectionModel.minSelectionIndex
					Dim iMax As Integer = selectionModel.maxSelectionIndex
    
					If (iMin = -1) OrElse (iMax = -1) Then Return New Integer(){}
    
					Dim rvTmp As Integer() = New Integer(1+ (iMax - iMin) - 1){}
					Dim n As Integer = 0
					For i As Integer = iMin To iMax
						If selectionModel.isSelectedIndex(i) Then
							rvTmp(n) = i
							n += 1
						End If
					Next i
					Dim rv As Integer() = New Integer(n - 1){}
					Array.Copy(rvTmp, 0, rv, 0, n)
					Return rv
				End If
				Return New Integer(){}
			End Get
		End Property

		' implements javax.swing.table.TableColumnModel
		''' <summary>
		''' Returns the number of columns selected. </summary>
		''' <returns> the number of columns selected </returns>
		Public Overridable Property selectedColumnCount As Integer Implements TableColumnModel.getSelectedColumnCount
			Get
				If selectionModel IsNot Nothing Then
					Dim iMin As Integer = selectionModel.minSelectionIndex
					Dim iMax As Integer = selectionModel.maxSelectionIndex
					Dim count As Integer = 0
    
					For i As Integer = iMin To iMax
						If selectionModel.isSelectedIndex(i) Then count += 1
					Next i
					Return count
				End If
				Return 0
			End Get
		End Property

	'
	' Listener Support Methods
	'

		' implements javax.swing.table.TableColumnModel
		''' <summary>
		''' Adds a listener for table column model events. </summary>
		''' <param name="x">  a <code>TableColumnModelListener</code> object </param>
		Public Overridable Sub addColumnModelListener(ByVal x As TableColumnModelListener) Implements TableColumnModel.addColumnModelListener
			listenerList.add(GetType(TableColumnModelListener), x)
		End Sub

		' implements javax.swing.table.TableColumnModel
		''' <summary>
		''' Removes a listener for table column model events. </summary>
		''' <param name="x">  a <code>TableColumnModelListener</code> object </param>
		Public Overridable Sub removeColumnModelListener(ByVal x As TableColumnModelListener) Implements TableColumnModel.removeColumnModelListener
			listenerList.remove(GetType(TableColumnModelListener), x)
		End Sub

		''' <summary>
		''' Returns an array of all the column model listeners
		''' registered on this model.
		''' </summary>
		''' <returns> all of this default table column model's <code>ColumnModelListener</code>s
		'''         or an empty
		'''         array if no column model listeners are currently registered
		''' </returns>
		''' <seealso cref= #addColumnModelListener </seealso>
		''' <seealso cref= #removeColumnModelListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property columnModelListeners As TableColumnModelListener()
			Get
				Return listenerList.getListeners(GetType(TableColumnModelListener))
			End Get
		End Property

	'
	'   Event firing methods
	'

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireColumnAdded(ByVal e As TableColumnModelEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TableColumnModelListener) Then CType(___listeners(i+1), TableColumnModelListener).columnAdded(e)
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireColumnRemoved(ByVal e As TableColumnModelEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TableColumnModelListener) Then CType(___listeners(i+1), TableColumnModelListener).columnRemoved(e)
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <param name="e"> the event received </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireColumnMoved(ByVal e As TableColumnModelEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TableColumnModelListener) Then CType(___listeners(i+1), TableColumnModelListener).columnMoved(e)
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <param name="e"> the event received </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireColumnSelectionChanged(ByVal e As ListSelectionEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TableColumnModelListener) Then CType(___listeners(i+1), TableColumnModelListener).columnSelectionChanged(e)
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireColumnMarginChanged()
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TableColumnModelListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), TableColumnModelListener).columnMarginChanged(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this model.
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
		''' <code>DefaultTableColumnModel</code> <code>m</code>
		''' for its column model listeners with the following code:
		''' 
		''' <pre>ColumnModelListener[] cmls = (ColumnModelListener[])(m.getListeners(ColumnModelListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this model,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getColumnModelListeners
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

	'
	' Implementing the PropertyChangeListener interface
	'

		' PENDING(alan)
		' implements java.beans.PropertyChangeListener
		''' <summary>
		''' Property Change Listener change method.  Used to track changes
		''' to the column width or preferred column width.
		''' </summary>
		''' <param name="evt">  <code>PropertyChangeEvent</code> </param>
		Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			Dim name As String = evt.propertyName

			If name = "width" OrElse name = "preferredWidth" Then
				invalidateWidthCache()
				' This is a misnomer, we're using this method
				' simply to cause a relayout.
				fireColumnMarginChanged()
			End If

		End Sub

	'
	' Implementing ListSelectionListener interface
	'

		' implements javax.swing.event.ListSelectionListener
		''' <summary>
		''' A <code>ListSelectionListener</code> that forwards
		''' <code>ListSelectionEvents</code> when there is a column
		''' selection change.
		''' </summary>
		''' <param name="e">  the change event </param>
		Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
			fireColumnSelectionChanged(e)
		End Sub

	'
	' Protected Methods
	'

		''' <summary>
		''' Creates a new default list selection model.
		''' </summary>
		Protected Friend Overridable Function createSelectionModel() As ListSelectionModel
			Return New DefaultListSelectionModel
		End Function

		''' <summary>
		''' Recalculates the total combined width of all columns.  Updates the
		''' <code>totalColumnWidth</code> property.
		''' </summary>
		Protected Friend Overridable Sub recalcWidthCache()
			Dim enumeration As System.Collections.IEnumerator = columns
			totalColumnWidth = 0
			Do While enumeration.hasMoreElements()
				totalColumnWidth += CType(enumeration.nextElement(), TableColumn).width
			Loop
		End Sub

		Private Sub invalidateWidthCache()
			totalColumnWidth = -1
		End Sub

	End Class ' End of class DefaultTableColumnModel

End Namespace
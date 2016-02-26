Imports System
Imports System.Runtime.CompilerServices
Imports javax.swing
Imports javax.swing.border

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
	'''  A <code>TableColumn</code> represents all the attributes of a column in a
	'''  <code>JTable</code>, such as width, resizability, minimum and maximum width.
	'''  In addition, the <code>TableColumn</code> provides slots for a renderer and
	'''  an editor that can be used to display and edit the values in this column.
	'''  <p>
	'''  It is also possible to specify renderers and editors on a per type basis
	'''  rather than a per column basis - see the
	'''  <code>setDefaultRenderer</code> method in the <code>JTable</code> class.
	'''  This default mechanism is only used when the renderer (or
	'''  editor) in the <code>TableColumn</code> is <code>null</code>.
	''' <p>
	'''  The <code>TableColumn</code> stores the link between the columns in the
	'''  <code>JTable</code> and the columns in the <code>TableModel</code>.
	'''  The <code>modelIndex</code> is the column in the
	'''  <code>TableModel</code>, which will be queried for the data values for the
	'''  cells in this column. As the column moves around in the view this
	'''  <code>modelIndex</code> does not change.
	'''  <p>
	''' <b>Note:</b> Some implementations may assume that all
	'''    <code>TableColumnModel</code>s are unique, therefore we would
	'''    recommend that the same <code>TableColumn</code> instance
	'''    not be added more than once to a <code>TableColumnModel</code>.
	'''    To show <code>TableColumn</code>s with the same column of
	'''    data from the model, create a new instance with the same
	'''    <code>modelIndex</code>.
	'''  <p>
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
	''' <seealso cref= javax.swing.table.TableColumnModel
	''' </seealso>
	''' <seealso cref= javax.swing.table.DefaultTableColumnModel </seealso>
	''' <seealso cref= javax.swing.table.JTableHeader#getDefaultRenderer() </seealso>
	''' <seealso cref= JTable#getDefaultRenderer(Class) </seealso>
	''' <seealso cref= JTable#getDefaultEditor(Class) </seealso>
	''' <seealso cref= JTable#getCellRenderer(int, int) </seealso>
	''' <seealso cref= JTable#getCellEditor(int, int) </seealso>
	<Serializable> _
	Public Class TableColumn
		Inherits Object

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Please use string literals to identify
		''' properties.
		''' </summary>
	'    
	'     * Warning: The value of this constant, "columWidth" is wrong as the
	'     * name of the property is "columnWidth".
	'     
		Public Const COLUMN_WIDTH_PROPERTY As String = "columWidth"

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Please use string literals to identify
		''' properties.
		''' </summary>
		Public Const HEADER_VALUE_PROPERTY As String = "headerValue"

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Please use string literals to identify
		''' properties.
		''' </summary>
		Public Const HEADER_RENDERER_PROPERTY As String = "headerRenderer"

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Please use string literals to identify
		''' properties.
		''' </summary>
		Public Const CELL_RENDERER_PROPERTY As String = "cellRenderer"

	'
	'  Instance Variables
	'

		''' <summary>
		''' The index of the column in the model which is to be displayed by
		''' this <code>TableColumn</code>. As columns are moved around in the
		''' view <code>modelIndex</code> remains constant.
		''' </summary>
		Protected Friend modelIndex As Integer

		''' <summary>
		'''  This object is not used internally by the drawing machinery of
		'''  the <code>JTable</code>; identifiers may be set in the
		'''  <code>TableColumn</code> as as an
		'''  optional way to tag and locate table columns. The table package does
		'''  not modify or invoke any methods in these identifier objects other
		'''  than the <code>equals</code> method which is used in the
		'''  <code>getColumnIndex()</code> method in the
		'''  <code>DefaultTableColumnModel</code>.
		''' </summary>
		Protected Friend identifier As Object

		''' <summary>
		''' The width of the column. </summary>
		Protected Friend width As Integer

		''' <summary>
		''' The minimum width of the column. </summary>
		Protected Friend minWidth As Integer

		''' <summary>
		''' The preferred width of the column. </summary>
		Private preferredWidth As Integer

		''' <summary>
		''' The maximum width of the column. </summary>
		Protected Friend maxWidth As Integer

		''' <summary>
		''' The renderer used to draw the header of the column. </summary>
		Protected Friend headerRenderer As TableCellRenderer

		''' <summary>
		''' The header value of the column. </summary>
		Protected Friend headerValue As Object

		''' <summary>
		''' The renderer used to draw the data cells of the column. </summary>
		Protected Friend cellRenderer As TableCellRenderer

		''' <summary>
		''' The editor used to edit the data cells of the column. </summary>
		Protected Friend cellEditor As TableCellEditor

		''' <summary>
		''' If true, the user is allowed to resize the column; the default is true. </summary>
		Protected Friend isResizable As Boolean

		''' <summary>
		''' This field was not used in previous releases and there are
		''' currently no plans to support it in the future.
		''' </summary>
		''' @deprecated as of Java 2 platform v1.3 
	'    
	'     *  Counter used to disable posting of resizing notifications until the
	'     *  end of the resize.
	'     
		<Obsolete("as of Java 2 platform v1.3"), NonSerialized> _
		Protected Friend resizedPostingDisableCount As Integer

		''' <summary>
		''' If any <code>PropertyChangeListeners</code> have been registered, the
		''' <code>changeSupport</code> field describes them.
		''' </summary>
		Private changeSupport As javax.swing.event.SwingPropertyChangeSupport

	'
	' Constructors
	'

		''' <summary>
		'''  Cover method, using a default model index of 0,
		'''  default width of 75, a <code>null</code> renderer and a
		'''  <code>null</code> editor.
		'''  This method is intended for serialization. </summary>
		'''  <seealso cref= #TableColumn(int, int, TableCellRenderer, TableCellEditor) </seealso>
		Public Sub New()
			Me.New(0)
		End Sub

		''' <summary>
		'''  Cover method, using a default width of 75, a <code>null</code>
		'''  renderer and a <code>null</code> editor. </summary>
		'''  <seealso cref= #TableColumn(int, int, TableCellRenderer, TableCellEditor) </seealso>
		Public Sub New(ByVal modelIndex As Integer)
			Me.New(modelIndex, 75, Nothing, Nothing)
		End Sub

		''' <summary>
		'''  Cover method, using a <code>null</code> renderer and a
		'''  <code>null</code> editor. </summary>
		'''  <seealso cref= #TableColumn(int, int, TableCellRenderer, TableCellEditor) </seealso>
		Public Sub New(ByVal modelIndex As Integer, ByVal width As Integer)
			Me.New(modelIndex, width, Nothing, Nothing)
		End Sub

		''' <summary>
		'''  Creates and initializes an instance of
		'''  <code>TableColumn</code> with the specified model index,
		'''  width, cell renderer, and cell editor;
		'''  all <code>TableColumn</code> constructors delegate to this one.
		'''  The value of <code>width</code> is used
		'''  for both the initial and preferred width;
		'''  if <code>width</code> is negative,
		'''  they're set to 0.
		'''  The minimum width is set to 15 unless the initial width is less,
		'''  in which case the minimum width is set to
		'''  the initial width.
		''' 
		'''  <p>
		'''  When the <code>cellRenderer</code>
		'''  or <code>cellEditor</code> parameter is <code>null</code>,
		'''  a default value provided by the <code>JTable</code>
		'''  <code>getDefaultRenderer</code>
		'''  or <code>getDefaultEditor</code> method, respectively,
		'''  is used to
		'''  provide defaults based on the type of the data in this column.
		'''  This column-centric rendering strategy can be circumvented by overriding
		'''  the <code>getCellRenderer</code> methods in <code>JTable</code>.
		''' </summary>
		''' <param name="modelIndex"> the index of the column
		'''  in the model that supplies the data for this column in the table;
		'''  the model index remains the same
		'''  even when columns are reordered in the view </param>
		''' <param name="width"> this column's preferred width and initial width </param>
		''' <param name="cellRenderer"> the object used to render values in this column </param>
		''' <param name="cellEditor"> the object used to edit values in this column </param>
		''' <seealso cref= #getMinWidth() </seealso>
		''' <seealso cref= JTable#getDefaultRenderer(Class) </seealso>
		''' <seealso cref= JTable#getDefaultEditor(Class) </seealso>
		''' <seealso cref= JTable#getCellRenderer(int, int) </seealso>
		''' <seealso cref= JTable#getCellEditor(int, int) </seealso>
		Public Sub New(ByVal modelIndex As Integer, ByVal width As Integer, ByVal cellRenderer As TableCellRenderer, ByVal cellEditor As TableCellEditor)
			MyBase.New()
			Me.modelIndex = modelIndex
				Me.width = Math.Max(width, 0)
				preferredWidth = Me.width

			Me.cellRenderer = cellRenderer
			Me.cellEditor = cellEditor

			' Set other instance variables to default values.
			minWidth = Math.Min(15, Me.width)
			maxWidth = Integer.MaxValue
			isResizable = True
			resizedPostingDisableCount = 0
			headerValue = Nothing
		End Sub

	'
	' Modifying and Querying attributes
	'

		Private Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If changeSupport IsNot Nothing Then changeSupport.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

		Private Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Integer, ByVal newValue As Integer)
			If oldValue <> newValue Then firePropertyChange(propertyName, Convert.ToInt32(oldValue), Convert.ToInt32(newValue))
		End Sub

		Private Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Boolean, ByVal newValue As Boolean)
			If oldValue <> newValue Then firePropertyChange(propertyName, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue))
		End Sub

		''' <summary>
		''' Sets the model index for this column. The model index is the
		''' index of the column in the model that will be displayed by this
		''' <code>TableColumn</code>. As the <code>TableColumn</code>
		''' is moved around in the view the model index remains constant. </summary>
		''' <param name="modelIndex">  the new modelIndex
		''' @beaninfo
		'''  bound: true
		'''  description: The model index. </param>
		Public Overridable Property modelIndex As Integer
			Set(ByVal modelIndex As Integer)
				Dim old As Integer = Me.modelIndex
				Me.modelIndex = modelIndex
				firePropertyChange("modelIndex", old, modelIndex)
			End Set
			Get
				Return modelIndex
			End Get
		End Property


		''' <summary>
		''' Sets the <code>TableColumn</code>'s identifier to
		''' <code>anIdentifier</code>. <p>
		''' Note: identifiers are not used by the <code>JTable</code>,
		''' they are purely a
		''' convenience for the external tagging and location of columns.
		''' </summary>
		''' <param name="identifier">           an identifier for this column </param>
		''' <seealso cref=        #getIdentifier
		''' @beaninfo
		'''  bound: true
		'''  description: A unique identifier for this column. </seealso>
		Public Overridable Property identifier As Object
			Set(ByVal identifier As Object)
				Dim old As Object = Me.identifier
				Me.identifier = identifier
				firePropertyChange("identifier", old, identifier)
			End Set
			Get
				Return If(identifier IsNot Nothing, identifier, headerValue)
    
			End Get
		End Property



		''' <summary>
		''' Sets the <code>Object</code> whose string representation will be
		''' used as the value for the <code>headerRenderer</code>.  When the
		''' <code>TableColumn</code> is created, the default <code>headerValue</code>
		''' is <code>null</code>. </summary>
		''' <param name="headerValue">  the new headerValue </param>
		''' <seealso cref=       #getHeaderValue
		''' @beaninfo
		'''  bound: true
		'''  description: The text to be used by the header renderer. </seealso>
		Public Overridable Property headerValue As Object
			Set(ByVal headerValue As Object)
				Dim old As Object = Me.headerValue
				Me.headerValue = headerValue
				firePropertyChange("headerValue", old, headerValue)
			End Set
			Get
				Return headerValue
			End Get
		End Property


		'
		' Renderers and Editors
		'

		''' <summary>
		''' Sets the <code>TableCellRenderer</code> used to draw the
		''' <code>TableColumn</code>'s header to <code>headerRenderer</code>.
		''' <p>
		''' It is the header renderers responsibility to render the sorting
		''' indicator.  If you are using sorting and specify a renderer your
		''' renderer must render the sorting indication.
		''' </summary>
		''' <param name="headerRenderer">  the new headerRenderer
		''' </param>
		''' <seealso cref=       #getHeaderRenderer
		''' @beaninfo
		'''  bound: true
		'''  description: The header renderer. </seealso>
		Public Overridable Property headerRenderer As TableCellRenderer
			Set(ByVal headerRenderer As TableCellRenderer)
				Dim old As TableCellRenderer = Me.headerRenderer
				Me.headerRenderer = headerRenderer
				firePropertyChange("headerRenderer", old, headerRenderer)
			End Set
			Get
				Return headerRenderer
			End Get
		End Property


		''' <summary>
		''' Sets the <code>TableCellRenderer</code> used by <code>JTable</code>
		''' to draw individual values for this column.
		''' </summary>
		''' <param name="cellRenderer">  the new cellRenderer </param>
		''' <seealso cref=     #getCellRenderer
		''' @beaninfo
		'''  bound: true
		'''  description: The renderer to use for cell values. </seealso>
		Public Overridable Property cellRenderer As TableCellRenderer
			Set(ByVal cellRenderer As TableCellRenderer)
				Dim old As TableCellRenderer = Me.cellRenderer
				Me.cellRenderer = cellRenderer
				firePropertyChange("cellRenderer", old, cellRenderer)
			End Set
			Get
				Return cellRenderer
			End Get
		End Property


		''' <summary>
		''' Sets the editor to used by when a cell in this column is edited.
		''' </summary>
		''' <param name="cellEditor">  the new cellEditor </param>
		''' <seealso cref=     #getCellEditor
		''' @beaninfo
		'''  bound: true
		'''  description: The editor to use for cell values. </seealso>
		Public Overridable Property cellEditor As TableCellEditor
			Set(ByVal cellEditor As TableCellEditor)
				Dim old As TableCellEditor = Me.cellEditor
				Me.cellEditor = cellEditor
				firePropertyChange("cellEditor", old, cellEditor)
			End Set
			Get
				Return cellEditor
			End Get
		End Property


		''' <summary>
		''' This method should not be used to set the widths of columns in the
		''' <code>JTable</code>, use <code>setPreferredWidth</code> instead.
		''' Like a layout manager in the
		''' AWT, the <code>JTable</code> adjusts a column's width automatically
		''' whenever the
		''' table itself changes size, or a column's preferred width is changed.
		''' Setting widths programmatically therefore has no long term effect.
		''' <p>
		''' This method sets this column's width to <code>width</code>.
		''' If <code>width</code> exceeds the minimum or maximum width,
		''' it is adjusted to the appropriate limiting value. </summary>
		''' <param name="width">  the new width </param>
		''' <seealso cref=     #getWidth </seealso>
		''' <seealso cref=     #setMinWidth </seealso>
		''' <seealso cref=     #setMaxWidth </seealso>
		''' <seealso cref=     #setPreferredWidth </seealso>
		''' <seealso cref=     JTable#doLayout()
		''' @beaninfo
		'''  bound: true
		'''  description: The width of the column. </seealso>
		Public Overridable Property width As Integer
			Set(ByVal width As Integer)
				Dim old As Integer = Me.width
				Me.width = Math.Min(Math.Max(width, minWidth), maxWidth)
				firePropertyChange("width", old, Me.width)
			End Set
			Get
				Return width
			End Get
		End Property


		''' <summary>
		''' Sets this column's preferred width to <code>preferredWidth</code>.
		''' If <code>preferredWidth</code> exceeds the minimum or maximum width,
		''' it is adjusted to the appropriate limiting value.
		''' <p>
		''' For details on how the widths of columns in the <code>JTable</code>
		''' (and <code>JTableHeader</code>) are calculated from the
		''' <code>preferredWidth</code>,
		''' see the <code>doLayout</code> method in <code>JTable</code>.
		''' </summary>
		''' <param name="preferredWidth"> the new preferred width </param>
		''' <seealso cref=     #getPreferredWidth </seealso>
		''' <seealso cref=     JTable#doLayout()
		''' @beaninfo
		'''  bound: true
		'''  description: The preferred width of the column. </seealso>
		Public Overridable Property preferredWidth As Integer
			Set(ByVal preferredWidth As Integer)
				Dim old As Integer = Me.preferredWidth
				Me.preferredWidth = Math.Min(Math.Max(preferredWidth, minWidth), maxWidth)
				firePropertyChange("preferredWidth", old, Me.preferredWidth)
			End Set
			Get
				Return preferredWidth
			End Get
		End Property


		''' <summary>
		''' Sets the <code>TableColumn</code>'s minimum width to
		''' <code>minWidth</code>,
		''' adjusting the new minimum width if necessary to ensure that
		''' 0 &lt;= <code>minWidth</code> &lt;= <code>maxWidth</code>.
		''' For example, if the <code>minWidth</code> argument is negative,
		''' this method sets the <code>minWidth</code> property to 0.
		''' 
		''' <p>
		''' If the value of the
		''' <code>width</code> or <code>preferredWidth</code> property
		''' is less than the new minimum width,
		''' this method sets that property to the new minimum width.
		''' </summary>
		''' <param name="minWidth">  the new minimum width </param>
		''' <seealso cref=     #getMinWidth </seealso>
		''' <seealso cref=     #setPreferredWidth </seealso>
		''' <seealso cref=     #setMaxWidth
		''' @beaninfo
		'''  bound: true
		'''  description: The minimum width of the column. </seealso>
		Public Overridable Property minWidth As Integer
			Set(ByVal minWidth As Integer)
				Dim old As Integer = Me.minWidth
				Me.minWidth = Math.Max(Math.Min(minWidth, maxWidth), 0)
				If width < Me.minWidth Then width = Me.minWidth
				If preferredWidth < Me.minWidth Then preferredWidth = Me.minWidth
				firePropertyChange("minWidth", old, Me.minWidth)
			End Set
			Get
				Return minWidth
			End Get
		End Property


		''' <summary>
		''' Sets the <code>TableColumn</code>'s maximum width to
		''' <code>maxWidth</code> or,
		''' if <code>maxWidth</code> is less than the minimum width,
		''' to the minimum width.
		''' 
		''' <p>
		''' If the value of the
		''' <code>width</code> or <code>preferredWidth</code> property
		''' is more than the new maximum width,
		''' this method sets that property to the new maximum width.
		''' </summary>
		''' <param name="maxWidth">  the new maximum width </param>
		''' <seealso cref=     #getMaxWidth </seealso>
		''' <seealso cref=     #setPreferredWidth </seealso>
		''' <seealso cref=     #setMinWidth
		''' @beaninfo
		'''  bound: true
		'''  description: The maximum width of the column. </seealso>
		Public Overridable Property maxWidth As Integer
			Set(ByVal maxWidth As Integer)
				Dim old As Integer = Me.maxWidth
				Me.maxWidth = Math.Max(minWidth, maxWidth)
				If width > Me.maxWidth Then width = Me.maxWidth
				If preferredWidth > Me.maxWidth Then preferredWidth = Me.maxWidth
				firePropertyChange("maxWidth", old, Me.maxWidth)
			End Set
			Get
				Return maxWidth
			End Get
		End Property


		''' <summary>
		''' Sets whether this column can be resized.
		''' </summary>
		''' <param name="isResizable">  if true, resizing is allowed; otherwise false </param>
		''' <seealso cref=     #getResizable
		''' @beaninfo
		'''  bound: true
		'''  description: Whether or not this column can be resized. </seealso>
		Public Overridable Property resizable As Boolean
			Set(ByVal isResizable As Boolean)
				Dim old As Boolean = Me.isResizable
				Me.isResizable = isResizable
				firePropertyChange("isResizable", old, Me.isResizable)
			End Set
			Get
				Return isResizable
			End Get
		End Property


		''' <summary>
		''' Resizes the <code>TableColumn</code> to fit the width of its header cell.
		''' This method does nothing if the header renderer is <code>null</code>
		''' (the default case). Otherwise, it sets the minimum, maximum and preferred
		''' widths of this column to the widths of the minimum, maximum and preferred
		''' sizes of the Component delivered by the header renderer.
		''' The transient "width" property of this TableColumn is also set to the
		''' preferred width. Note this method is not used internally by the table
		''' package.
		''' </summary>
		''' <seealso cref=     #setPreferredWidth </seealso>
		Public Overridable Sub sizeWidthToFit()
			If headerRenderer Is Nothing Then Return
			Dim c As java.awt.Component = headerRenderer.getTableCellRendererComponent(Nothing, headerValue, False, False, 0, 0)

			minWidth = c.minimumSize.width
			maxWidth = c.maximumSize.width
			preferredWidth = c.preferredSize.width

			width = preferredWidth
		End Sub

		''' <summary>
		''' This field was not used in previous releases and there are
		''' currently no plans to support it in the future.
		''' </summary>
		''' @deprecated as of Java 2 platform v1.3 
		<Obsolete("as of Java 2 platform v1.3")> _
		Public Overridable Sub disableResizedPosting()
			resizedPostingDisableCount += 1
		End Sub

		''' <summary>
		''' This field was not used in previous releases and there are
		''' currently no plans to support it in the future.
		''' </summary>
		''' @deprecated as of Java 2 platform v1.3 
		<Obsolete("as of Java 2 platform v1.3")> _
		Public Overridable Sub enableResizedPosting()
			resizedPostingDisableCount -= 1
		End Sub

	'
	' Property Change Support
	'

		''' <summary>
		''' Adds a <code>PropertyChangeListener</code> to the listener list.
		''' The listener is registered for all properties.
		''' <p>
		''' A <code>PropertyChangeEvent</code> will get fired in response to an
		''' explicit call to <code>setFont</code>, <code>setBackground</code>,
		''' or <code>setForeground</code> on the
		''' current component.  Note that if the current component is
		''' inheriting its foreground, background, or font from its
		''' container, then no event will be fired in response to a
		''' change in the inherited property.
		''' </summary>
		''' <param name="listener">  the listener to be added
		'''  </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			If changeSupport Is Nothing Then changeSupport = New javax.swing.event.SwingPropertyChangeSupport(Me)
			changeSupport.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Removes a <code>PropertyChangeListener</code> from the listener list.
		''' The <code>PropertyChangeListener</code> to be removed was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the listener to be removed
		'''  </param>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			If changeSupport IsNot Nothing Then changeSupport.removePropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>PropertyChangeListener</code>s added
		''' to this TableColumn with addPropertyChangeListener().
		''' </summary>
		''' <returns> all of the <code>PropertyChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property propertyChangeListeners As java.beans.PropertyChangeListener()
			Get
				If changeSupport Is Nothing Then Return New java.beans.PropertyChangeListener(){}
				Return changeSupport.propertyChangeListeners
			End Get
		End Property

	'
	' Protected Methods
	'

		''' <summary>
		''' As of Java 2 platform v1.3, this method is not called by the <code>TableColumn</code>
		''' constructor.  Previously this method was used by the
		''' <code>TableColumn</code> to create a default header renderer.
		''' As of Java 2 platform v1.3, the default header renderer is <code>null</code>.
		''' <code>JTableHeader</code> now provides its own shared default
		''' renderer, just as the <code>JTable</code> does for its cell renderers.
		''' </summary>
		''' <returns> the default header renderer </returns>
		''' <seealso cref= javax.swing.table.JTableHeader#createDefaultRenderer() </seealso>
		Protected Friend Overridable Function createDefaultHeaderRenderer() As TableCellRenderer
			Dim label As DefaultTableCellRenderer = New DefaultTableCellRendererAnonymousInnerClassHelper
			label.horizontalAlignment = JLabel.CENTER
			Return label
		End Function

		Private Class DefaultTableCellRendererAnonymousInnerClassHelper
			Inherits DefaultTableCellRenderer

			Public Overrides Function getTableCellRendererComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As java.awt.Component
				If table IsNot Nothing Then
					Dim header As JTableHeader = table.tableHeader
					If header IsNot Nothing Then
						foreground = header.foreground
						background = header.background
						font = header.font
					End If
				End If

				text = If(value Is Nothing, "", value.ToString())
				border = UIManager.getBorder("TableHeader.cellBorder")
				Return Me
			End Function
		End Class

	End Class ' End of class TableColumn

End Namespace
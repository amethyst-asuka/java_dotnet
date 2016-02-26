Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports javax.swing.event
Imports javax.accessibility
Imports javax.swing.plaf
Imports sun.swing.SwingUtilities2.Section

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

Namespace javax.swing







	''' <summary>
	''' A component that displays a list of objects and allows the user to select
	''' one or more items. A separate model, {@code ListModel}, maintains the
	''' contents of the list.
	''' <p>
	''' It's easy to display an array or Vector of objects, using the {@code JList}
	''' constructor that automatically builds a read-only {@code ListModel} instance
	''' for you:
	''' <pre>
	''' {@code
	''' // Create a JList that displays strings from an array
	''' 
	''' String[] data = {"one", "two", "three", "four"};
	''' JList<String> myList = new JList<String>(data);
	''' 
	''' // Create a JList that displays the superclasses of JList.class, by
	''' // creating it with a Vector populated with this data
	''' 
	''' Vector<Class<?>> superClasses = new Vector<Class<?>>();
	''' Class<JList> rootClass = javax.swing.JList.class;
	''' for(Class<?> cls = rootClass; cls != null; cls = cls.getSuperclass()) {
	'''     superClasses.addElement(cls);
	''' }
	''' JList<Class<?>> myList = new JList<Class<?>>(superClasses);
	''' 
	''' // The automatically created model is stored in JList's "model"
	''' // property, which you can retrieve
	''' 
	''' ListModel<Class<?>> model = myList.getModel();
	''' for(int i = 0; i < model.getSize(); i++) {
	'''     System.out.println(model.getElementAt(i));
	''' }
	''' }
	''' </pre>
	''' <p>
	''' A {@code ListModel} can be supplied directly to a {@code JList} by way of a
	''' constructor or the {@code setModel} method. The contents need not be static -
	''' the number of items, and the values of items can change over time. A correct
	''' {@code ListModel} implementation notifies the set of
	''' {@code javax.swing.event.ListDataListener}s that have been added to it, each
	''' time a change occurs. These changes are characterized by a
	''' {@code javax.swing.event.ListDataEvent}, which identifies the range of list
	''' indices that have been modified, added, or removed. {@code JList}'s
	''' {@code ListUI} is responsible for keeping the visual representation up to
	''' date with changes, by listening to the model.
	''' <p>
	''' Simple, dynamic-content, {@code JList} applications can use the
	''' {@code DefaultListModel} class to maintain list elements. This class
	''' implements the {@code ListModel} interface and also provides a
	''' <code>java.util.Vector</code>-like API. Applications that need a more
	''' custom <code>ListModel</code> implementation may instead wish to subclass
	''' {@code AbstractListModel}, which provides basic support for managing and
	''' notifying listeners. For example, a read-only implementation of
	''' {@code AbstractListModel}:
	''' <pre>
	''' {@code
	''' // This list model has about 2^16 elements.  Enjoy scrolling.
	''' 
	''' ListModel<String> bigData = new AbstractListModel<String>() {
	'''     public int getSize() { return Short.MAX_VALUE; }
	'''     public String getElementAt(int index) { return "Index " + index; }
	''' };
	''' }
	''' </pre>
	''' <p>
	''' The selection state of a {@code JList} is managed by another separate
	''' model, an instance of {@code ListSelectionModel}. {@code JList} is
	''' initialized with a selection model on construction, and also contains
	''' methods to query or set this selection model. Additionally, {@code JList}
	''' provides convenient methods for easily managing the selection. These methods,
	''' such as {@code setSelectedIndex} and {@code getSelectedValue}, are cover
	''' methods that take care of the details of interacting with the selection
	''' model. By default, {@code JList}'s selection model is configured to allow any
	''' combination of items to be selected at a time; selection mode
	''' {@code MULTIPLE_INTERVAL_SELECTION}. The selection mode can be changed
	''' on the selection model directly, or via {@code JList}'s cover method.
	''' Responsibility for updating the selection model in response to user gestures
	''' lies with the list's {@code ListUI}.
	''' <p>
	''' A correct {@code ListSelectionModel} implementation notifies the set of
	''' {@code javax.swing.event.ListSelectionListener}s that have been added to it
	''' each time a change to the selection occurs. These changes are characterized
	''' by a {@code javax.swing.event.ListSelectionEvent}, which identifies the range
	''' of the selection change.
	''' <p>
	''' The preferred way to listen for changes in list selection is to add
	''' {@code ListSelectionListener}s directly to the {@code JList}. {@code JList}
	''' then takes care of listening to the the selection model and notifying your
	''' listeners of change.
	''' <p>
	''' Responsibility for listening to selection changes in order to keep the list's
	''' visual representation up to date lies with the list's {@code ListUI}.
	''' <p>
	''' <a name="renderer"></a>
	''' Painting of cells in a {@code JList} is handled by a delegate called a
	''' cell renderer, installed on the list as the {@code cellRenderer} property.
	''' The renderer provides a {@code java.awt.Component} that is used
	''' like a "rubber stamp" to paint the cells. Each time a cell needs to be
	''' painted, the list's {@code ListUI} asks the cell renderer for the component,
	''' moves it into place, and has it paint the contents of the cell by way of its
	''' {@code paint} method. A default cell renderer, which uses a {@code JLabel}
	''' component to render, is installed by the lists's {@code ListUI}. You can
	''' substitute your own renderer using code like this:
	''' <pre>
	''' {@code
	'''  // Display an icon and a string for each object in the list.
	''' 
	''' class MyCellRenderer extends JLabel implements ListCellRenderer<Object> {
	'''     final static ImageIcon longIcon = new ImageIcon("long.gif");
	'''     final static ImageIcon shortIcon = new ImageIcon("short.gif");
	''' 
	'''     // This is the only method defined by ListCellRenderer.
	'''     // We just reconfigure the JLabel each time we're called.
	''' 
	'''     public Component getListCellRendererComponent(
	'''       JList<?> list,           // the list
	'''       Object value,            // value to display
	'''       int index,               // cell index
	'''       boolean isSelected,      // is the cell selected
	'''       boolean cellHasFocus)    // does the cell have focus
	'''     {
	'''         String s = value.toString();
	'''         setText(s);
	'''         setIcon((s.length() > 10) ? longIcon : shortIcon);
	'''         if (isSelected) {
	'''             setBackground(list.getSelectionBackground());
	'''             setForeground(list.getSelectionForeground());
	'''         } else {
	'''             setBackground(list.getBackground());
	'''             setForeground(list.getForeground());
	'''         }
	'''         setEnabled(list.isEnabled());
	'''         setFont(list.getFont());
	'''         setOpaque(true);
	'''         return this;
	'''     }
	''' }
	''' 
	''' myList.setCellRenderer(new MyCellRenderer());
	''' }
	''' </pre>
	''' <p>
	''' Another job for the cell renderer is in helping to determine sizing
	''' information for the list. By default, the list's {@code ListUI} determines
	''' the size of cells by asking the cell renderer for its preferred
	''' size for each list item. This can be expensive for large lists of items.
	''' To avoid these calculations, you can set a {@code fixedCellWidth} and
	''' {@code fixedCellHeight} on the list, or have these values calculated
	''' automatically based on a single prototype value:
	''' <a name="prototype_example"></a>
	''' <pre>
	''' {@code
	''' JList<String> bigDataList = new JList<String>(bigData);
	''' 
	''' // We don't want the JList implementation to compute the width
	''' // or height of all of the list cells, so we give it a string
	''' // that's as big as we'll need for any cell.  It uses this to
	''' // compute values for the fixedCellWidth and fixedCellHeight
	''' // properties.
	''' 
	''' bigDataList.setPrototypeCellValue("Index 1234567890");
	''' }
	''' </pre>
	''' <p>
	''' {@code JList} doesn't implement scrolling directly. To create a list that
	''' scrolls, make it the viewport view of a {@code JScrollPane}. For example:
	''' <pre>
	''' JScrollPane scrollPane = new JScrollPane(myList);
	''' 
	''' // Or in two steps:
	''' JScrollPane scrollPane = new JScrollPane();
	''' scrollPane.getViewport().setView(myList);
	''' </pre>
	''' <p>
	''' {@code JList} doesn't provide any special handling of double or triple
	''' (or N) mouse clicks, but it's easy to add a {@code MouseListener} if you
	''' wish to take action on these events. Use the {@code locationToIndex}
	''' method to determine what cell was clicked. For example:
	''' <pre>
	''' MouseListener mouseListener = new MouseAdapter() {
	'''     public void mouseClicked(MouseEvent e) {
	'''         if (e.getClickCount() == 2) {
	'''             int index = list.locationToIndex(e.getPoint());
	'''             System.out.println("Double clicked on Item " + index);
	'''          }
	'''     }
	''' };
	''' list.addMouseListener(mouseListener);
	''' </pre>
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' <p>
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/list.html">How to Use Lists</a>
	''' in <a href="https://docs.oracle.com/javase/tutorial/"><em>The Java Tutorial</em></a>
	''' for further documentation.
	''' <p> </summary>
	''' <seealso cref= ListModel </seealso>
	''' <seealso cref= AbstractListModel </seealso>
	''' <seealso cref= DefaultListModel </seealso>
	''' <seealso cref= ListSelectionModel </seealso>
	''' <seealso cref= DefaultListSelectionModel </seealso>
	''' <seealso cref= ListCellRenderer </seealso>
	''' <seealso cref= DefaultListCellRenderer
	''' </seealso>
	''' @param <E> the type of the elements of this list
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A component which allows for the selection of one or more objects from a list.
	''' 
	''' @author Hans Muller </param>
	Public Class JList(Of E)
		Inherits JComponent
		Implements Scrollable, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ListUI"

		''' <summary>
		''' Indicates a vertical layout of cells, in a single column;
		''' the default layout. </summary>
		''' <seealso cref= #setLayoutOrientation
		''' @since 1.4 </seealso>
		Public Const VERTICAL As Integer = 0

		''' <summary>
		''' Indicates a "newspaper style" layout with cells flowing vertically
		''' then horizontally. </summary>
		''' <seealso cref= #setLayoutOrientation
		''' @since 1.4 </seealso>
		Public Const VERTICAL_WRAP As Integer = 1

		''' <summary>
		''' Indicates a "newspaper style" layout with cells flowing horizontally
		''' then vertically. </summary>
		''' <seealso cref= #setLayoutOrientation
		''' @since 1.4 </seealso>
		Public Const HORIZONTAL_WRAP As Integer = 2

		Private fixedCellWidth As Integer = -1
		Private fixedCellHeight As Integer = -1
		Private horizontalScrollIncrement As Integer = -1
		Private prototypeCellValue As E
		Private visibleRowCount As Integer = 8
		Private selectionForeground As Color
		Private selectionBackground As Color
		Private dragEnabled As Boolean

		Private selectionModel As ListSelectionModel
		Private dataModel As ListModel(Of E)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private cellRenderer As ListCellRenderer(Of ?)
		Private selectionListener As ListSelectionListener

		''' <summary>
		''' How to lay out the cells; defaults to <code>VERTICAL</code>.
		''' </summary>
		Private layoutOrientation As Integer

		''' <summary>
		''' The drop mode for this component.
		''' </summary>
		Private dropMode As DropMode = DropMode.USE_SELECTION

		''' <summary>
		''' The drop location.
		''' </summary>
		<NonSerialized> _
		Private dropLocation As DropLocation

		''' <summary>
		''' A subclass of <code>TransferHandler.DropLocation</code> representing
		''' a drop location for a <code>JList</code>.
		''' </summary>
		''' <seealso cref= #getDropLocation
		''' @since 1.6 </seealso>
		Public NotInheritable Class DropLocation
			Inherits TransferHandler.DropLocation

			Private ReadOnly index As Integer
			Private ReadOnly ___isInsert As Boolean

			Private Sub New(ByVal p As Point, ByVal index As Integer, ByVal isInsert As Boolean)
				MyBase.New(p)
				Me.index = index
				Me.___isInsert = isInsert
			End Sub

			''' <summary>
			''' Returns the index where dropped data should be placed in the
			''' list. Interpretation of the value depends on the drop mode set on
			''' the associated component. If the drop mode is either
			''' <code>DropMode.USE_SELECTION</code> or <code>DropMode.ON</code>,
			''' the return value is an index of a row in the list. If the drop mode is
			''' <code>DropMode.INSERT</code>, the return value refers to the index
			''' where the data should be inserted. If the drop mode is
			''' <code>DropMode.ON_OR_INSERT</code>, the value of
			''' <code>isInsert()</code> indicates whether the index is an index
			''' of a row, or an insert index.
			''' <p>
			''' <code>-1</code> indicates that the drop occurred over empty space,
			''' and no index could be calculated.
			''' </summary>
			''' <returns> the drop index </returns>
			Public Property index As Integer
				Get
					Return index
				End Get
			End Property

			''' <summary>
			''' Returns whether or not this location represents an insert
			''' location.
			''' </summary>
			''' <returns> whether or not this is an insert location </returns>
			Public Property insert As Boolean
				Get
					Return ___isInsert
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this drop location.
			''' This method is intended to be used for debugging purposes,
			''' and the content and format of the returned string may vary
			''' between implementations.
			''' </summary>
			''' <returns> a string representation of this drop location </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[dropPoint=" & dropPoint & "," & "index=" & index & "," & "insert=" & ___isInsert & "]"
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code JList} that displays elements from the specified,
		''' {@code non-null}, model. All {@code JList} constructors delegate to
		''' this one.
		''' <p>
		''' This constructor registers the list with the {@code ToolTipManager},
		''' allowing for tooltips to be provided by the cell renderers.
		''' </summary>
		''' <param name="dataModel"> the model for the list </param>
		''' <exception cref="IllegalArgumentException"> if the model is {@code null} </exception>
		Public Sub New(ByVal dataModel As ListModel(Of E))
			If dataModel Is Nothing Then Throw New System.ArgumentException("dataModel must be non null")

			' Register with the ToolTipManager so that tooltips from the
			' renderer show through.
			Dim ___toolTipManager As ToolTipManager = ToolTipManager.sharedInstance()
			___toolTipManager.registerComponent(Me)

			layoutOrientation = VERTICAL

			Me.dataModel = dataModel
			selectionModel = createSelectionModel()
			autoscrolls = True
			opaque = True
			updateUI()
		End Sub


		''' <summary>
		''' Constructs a <code>JList</code> that displays the elements in
		''' the specified array. This constructor creates a read-only model
		''' for the given array, and then delegates to the constructor that
		''' takes a {@code ListModel}.
		''' <p>
		''' Attempts to pass a {@code null} value to this method results in
		''' undefined behavior and, most likely, exceptions. The created model
		''' references the given array directly. Attempts to modify the array
		''' after constructing the list results in undefined behavior.
		''' </summary>
		''' <param name="listData">  the array of Objects to be loaded into the data model,
		'''                   {@code non-null} </param>
		Public Sub New(ByVal listData As E())
			Me.New(New AbstractListModelAnonymousInnerClassHelper(Of E)
		   )
		End Sub

		Private Class AbstractListModelAnonymousInnerClassHelper(Of E)
			Inherits AbstractListModel(Of E)

			Public Overridable Property size As Integer
				Get
					Return listData.length
				End Get
			End Property
			Public Overridable Function getElementAt(ByVal i As Integer) As E
				Return listData(i)
			End Function
		End Class


		''' <summary>
		''' Constructs a <code>JList</code> that displays the elements in
		''' the specified <code>Vector</code>. This constructor creates a read-only
		''' model for the given {@code Vector}, and then delegates to the constructor
		''' that takes a {@code ListModel}.
		''' <p>
		''' Attempts to pass a {@code null} value to this method results in
		''' undefined behavior and, most likely, exceptions. The created model
		''' references the given {@code Vector} directly. Attempts to modify the
		''' {@code Vector} after constructing the list results in undefined behavior.
		''' </summary>
		''' <param name="listData">  the <code>Vector</code> to be loaded into the
		'''                   data model, {@code non-null} </param>
		Public Sub New(Of T1 As E)(ByVal listData As List(Of T1))
			Me.New(New AbstractListModelAnonymousInnerClassHelper2(Of E)
		   )
		End Sub

		Private Class AbstractListModelAnonymousInnerClassHelper2(Of E)
			Inherits AbstractListModel(Of E)

			Public Overridable Property size As Integer
				Get
					Return listData.size()
				End Get
			End Property
			Public Overridable Function getElementAt(ByVal i As Integer) As E
				Return listData.elementAt(i)
			End Function
		End Class


		''' <summary>
		''' Constructs a <code>JList</code> with an empty, read-only, model.
		''' </summary>
		Public Sub New()
			Me.New(New AbstractListModelAnonymousInnerClassHelper3(Of E)
		   )
		End Sub

		Private Class AbstractListModelAnonymousInnerClassHelper3(Of E)
			Inherits AbstractListModel(Of E)

			Public Overridable Property size As Integer
				Get
					Return 0
				End Get
			End Property
			Public Overridable Function getElementAt(ByVal i As Integer) As E
				Throw New System.IndexOutOfRangeException("No Data Model")
			End Function
		End Class


		''' <summary>
		''' Returns the {@code ListUI}, the look and feel object that
		''' renders this component.
		''' </summary>
		''' <returns> the <code>ListUI</code> object that renders this component </returns>
		Public Overridable Property uI As ListUI
			Get
				Return CType(ui, ListUI)
			End Get
			Set(ByVal ui As ListUI)
				MyBase.uI = ui
			End Set
		End Property




		''' <summary>
		''' Resets the {@code ListUI} property by setting it to the value provided
		''' by the current look and feel. If the current cell renderer was installed
		''' by the developer (rather than the look and feel itself), this also causes
		''' the cell renderer and its children to be updated, by calling
		''' {@code SwingUtilities.updateComponentTreeUI} on it.
		''' </summary>
		''' <seealso cref= UIManager#getUI </seealso>
		''' <seealso cref= SwingUtilities#updateComponentTreeUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ListUI)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim renderer As ListCellRenderer(Of ?) = cellRenderer
			If TypeOf renderer Is Component Then SwingUtilities.updateComponentTreeUI(CType(renderer, Component))
		End Sub


		''' <summary>
		''' Returns {@code "ListUI"}, the <code>UIDefaults</code> key used to look
		''' up the name of the {@code javax.swing.plaf.ListUI} class that defines
		''' the look and feel for this component.
		''' </summary>
		''' <returns> the string "ListUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


	'     -----private-----
	'     * This method is called by setPrototypeCellValue and setCellRenderer
	'     * to update the fixedCellWidth and fixedCellHeight properties from the
	'     * current value of prototypeCellValue (if it's non null).
	'     * <p>
	'     * This method sets fixedCellWidth and fixedCellHeight but does <b>not</b>
	'     * generate PropertyChangeEvents for them.
	'     *
	'     * @see #setPrototypeCellValue
	'     * @see #setCellRenderer
	'     
		Private Sub updateFixedCellSize()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cr As ListCellRenderer(Of ?) = cellRenderer
			Dim value As E = prototypeCellValue

			If (cr IsNot Nothing) AndAlso (value IsNot Nothing) Then
				Dim c As Component = cr.getListCellRendererComponent(Me, value, 0, False, False)

	'             The ListUI implementation will add Component c to its private
	'             * CellRendererPane however we can't assume that's already
	'             * been done here.  So we temporarily set the one "inherited"
	'             * property that may affect the renderer components preferred size:
	'             * its font.
	'             
				Dim f As Font = c.font
				c.font = font

				Dim d As Dimension = c.preferredSize
				fixedCellWidth = d.width
				fixedCellHeight = d.height

				c.font = f
			End If
		End Sub


		''' <summary>
		''' Returns the "prototypical" cell value -- a value used to calculate a
		''' fixed width and height for cells. This can be {@code null} if there
		''' is no such value.
		''' </summary>
		''' <returns> the value of the {@code prototypeCellValue} property </returns>
		''' <seealso cref= #setPrototypeCellValue </seealso>
		Public Overridable Property prototypeCellValue As E
			Get
				Return prototypeCellValue
			End Get
			Set(ByVal prototypeCellValue As E)
				Dim oldValue As E = Me.prototypeCellValue
				Me.prototypeCellValue = prototypeCellValue
    
		'         If the prototypeCellValue has changed and is non-null,
		'         * then recompute fixedCellWidth and fixedCellHeight.
		'         
    
				If (prototypeCellValue IsNot Nothing) AndAlso (Not prototypeCellValue.Equals(oldValue)) Then updateFixedCellSize()
    
				firePropertyChange("prototypeCellValue", oldValue, prototypeCellValue)
			End Set
		End Property



		''' <summary>
		''' Returns the value of the {@code fixedCellWidth} property.
		''' </summary>
		''' <returns> the fixed cell width </returns>
		''' <seealso cref= #setFixedCellWidth </seealso>
		Public Overridable Property fixedCellWidth As Integer
			Get
				Return fixedCellWidth
			End Get
			Set(ByVal width As Integer)
				Dim oldValue As Integer = fixedCellWidth
				fixedCellWidth = width
				firePropertyChange("fixedCellWidth", oldValue, fixedCellWidth)
			End Set
		End Property



		''' <summary>
		''' Returns the value of the {@code fixedCellHeight} property.
		''' </summary>
		''' <returns> the fixed cell height </returns>
		''' <seealso cref= #setFixedCellHeight </seealso>
		Public Overridable Property fixedCellHeight As Integer
			Get
				Return fixedCellHeight
			End Get
			Set(ByVal height As Integer)
				Dim oldValue As Integer = fixedCellHeight
				fixedCellHeight = height
				firePropertyChange("fixedCellHeight", oldValue, fixedCellHeight)
			End Set
		End Property



		''' <summary>
		''' Returns the object responsible for painting list items.
		''' </summary>
		''' <returns> the value of the {@code cellRenderer} property </returns>
		''' <seealso cref= #setCellRenderer </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property cellRenderer As ListCellRenderer(Of ?)
			Get
				Return cellRenderer
			End Get
			Set(ByVal cellRenderer As ListCellRenderer(Of T1))
	'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim oldValue As ListCellRenderer(Of ?) = Me.cellRenderer
				Me.cellRenderer = cellRenderer
    
		'         If the cellRenderer has changed and prototypeCellValue
		'         * was set, then recompute fixedCellWidth and fixedCellHeight.
		'         
				If (cellRenderer IsNot Nothing) AndAlso (Not cellRenderer.Equals(oldValue)) Then updateFixedCellSize()
    
				firePropertyChange("cellRenderer", oldValue, cellRenderer)
			End Set
		End Property

		''' <summary>
		''' Sets the delegate that is used to paint each cell in the list.
		''' The job of a cell renderer is discussed in detail in the
		''' <a href="#renderer">class level documentation</a>.
		''' <p>
		''' If the {@code prototypeCellValue} property is {@code non-null},
		''' setting the cell renderer also causes the {@code fixedCellWidth} and
		''' {@code fixedCellHeight} properties to be re-calculated. Only one
		''' <code>PropertyChangeEvent</code> is generated however -
		''' for the <code>cellRenderer</code> property.
		''' <p>
		''' The default value of this property is provided by the {@code ListUI}
		''' delegate, i.e. by the look and feel implementation.
		''' <p>
		''' This is a JavaBeans bound property.
		''' </summary>
		''' <param name="cellRenderer"> the <code>ListCellRenderer</code>
		'''                          that paints list cells </param>
		''' <seealso cref= #getCellRenderer
		''' @beaninfo
		'''       bound: true
		'''   attribute: visualUpdate true
		''' description: The component used to draw the cells. </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:


		''' <summary>
		''' Returns the color used to draw the foreground of selected items.
		''' {@code DefaultListCellRenderer} uses this color to draw the foreground
		''' of items in the selected state, as do the renderers installed by most
		''' {@code ListUI} implementations.
		''' </summary>
		''' <returns> the color to draw the foreground of selected items </returns>
		''' <seealso cref= #setSelectionForeground </seealso>
		''' <seealso cref= DefaultListCellRenderer </seealso>
		Public Overridable Property selectionForeground As Color
			Get
				Return selectionForeground
			End Get
			Set(ByVal selectionForeground As Color)
				Dim oldValue As Color = Me.selectionForeground
				Me.selectionForeground = selectionForeground
				firePropertyChange("selectionForeground", oldValue, selectionForeground)
			End Set
		End Property




		''' <summary>
		''' Returns the color used to draw the background of selected items.
		''' {@code DefaultListCellRenderer} uses this color to draw the background
		''' of items in the selected state, as do the renderers installed by most
		''' {@code ListUI} implementations.
		''' </summary>
		''' <returns> the color to draw the background of selected items </returns>
		''' <seealso cref= #setSelectionBackground </seealso>
		''' <seealso cref= DefaultListCellRenderer </seealso>
		Public Overridable Property selectionBackground As Color
			Get
				Return selectionBackground
			End Get
			Set(ByVal selectionBackground As Color)
				Dim oldValue As Color = Me.selectionBackground
				Me.selectionBackground = selectionBackground
				firePropertyChange("selectionBackground", oldValue, selectionBackground)
			End Set
		End Property




		''' <summary>
		''' Returns the value of the {@code visibleRowCount} property. See the
		''' documentation for <seealso cref="#setVisibleRowCount"/> for details on how to
		''' interpret this value.
		''' </summary>
		''' <returns> the value of the {@code visibleRowCount} property. </returns>
		''' <seealso cref= #setVisibleRowCount </seealso>
		Public Overridable Property visibleRowCount As Integer
			Get
				Return visibleRowCount
			End Get
			Set(ByVal visibleRowCount As Integer)
				Dim oldValue As Integer = Me.visibleRowCount
				Me.visibleRowCount = Math.Max(0, visibleRowCount)
				firePropertyChange("visibleRowCount", oldValue, visibleRowCount)
			End Set
		End Property



		''' <summary>
		''' Returns the layout orientation property for the list: {@code VERTICAL}
		''' if the layout is a single column of cells, {@code VERTICAL_WRAP} if the
		''' layout is "newspaper style" with the content flowing vertically then
		''' horizontally, or {@code HORIZONTAL_WRAP} if the layout is "newspaper
		''' style" with the content flowing horizontally then vertically.
		''' </summary>
		''' <returns> the value of the {@code layoutOrientation} property </returns>
		''' <seealso cref= #setLayoutOrientation
		''' @since 1.4 </seealso>
		Public Overridable Property layoutOrientation As Integer
			Get
				Return layoutOrientation
			End Get
			Set(ByVal layoutOrientation As Integer)
				Dim oldValue As Integer = Me.layoutOrientation
				Select Case layoutOrientation
				Case VERTICAL, VERTICAL_WRAP, HORIZONTAL_WRAP
					Me.layoutOrientation = layoutOrientation
					firePropertyChange("layoutOrientation", oldValue, layoutOrientation)
				Case Else
					Throw New System.ArgumentException("layoutOrientation must be one of: VERTICAL, HORIZONTAL_WRAP or VERTICAL_WRAP")
				End Select
			End Set
		End Property




		''' <summary>
		''' Returns the smallest list index that is currently visible.
		''' In a left-to-right {@code componentOrientation}, the first visible
		''' cell is found closest to the list's upper-left corner. In right-to-left
		''' orientation, it is found closest to the upper-right corner.
		''' If nothing is visible or the list is empty, {@code -1} is returned.
		''' Note that the returned cell may only be partially visible.
		''' </summary>
		''' <returns> the index of the first visible cell </returns>
		''' <seealso cref= #getLastVisibleIndex </seealso>
		''' <seealso cref= JComponent#getVisibleRect </seealso>
		Public Overridable Property firstVisibleIndex As Integer
			Get
				Dim r As Rectangle = visibleRect
				Dim first As Integer
				If Me.componentOrientation.leftToRight Then
					first = locationToIndex(r.location)
				Else
					first = locationToIndex(New Point((r.x + r.width) - 1, r.y))
				End If
				If first <> -1 Then
					Dim ___bounds As Rectangle = getCellBounds(first, first)
					If ___bounds IsNot Nothing Then
						SwingUtilities.computeIntersection(r.x, r.y, r.width, r.height, ___bounds)
						If ___bounds.width = 0 OrElse ___bounds.height = 0 Then first = -1
					End If
				End If
				Return first
			End Get
		End Property


		''' <summary>
		''' Returns the largest list index that is currently visible.
		''' If nothing is visible or the list is empty, {@code -1} is returned.
		''' Note that the returned cell may only be partially visible.
		''' </summary>
		''' <returns> the index of the last visible cell </returns>
		''' <seealso cref= #getFirstVisibleIndex </seealso>
		''' <seealso cref= JComponent#getVisibleRect </seealso>
		Public Overridable Property lastVisibleIndex As Integer
			Get
				Dim leftToRight As Boolean = Me.componentOrientation.leftToRight
				Dim r As Rectangle = visibleRect
				Dim lastPoint As Point
				If leftToRight Then
					lastPoint = New Point((r.x + r.width) - 1, (r.y + r.height) - 1)
				Else
					lastPoint = New Point(r.x, (r.y + r.height) - 1)
				End If
				Dim ___location As Integer = locationToIndex(lastPoint)
    
				If ___location <> -1 Then
					Dim ___bounds As Rectangle = getCellBounds(___location, ___location)
    
					If ___bounds IsNot Nothing Then
						SwingUtilities.computeIntersection(r.x, r.y, r.width, r.height, ___bounds)
						If ___bounds.width = 0 OrElse ___bounds.height = 0 Then
							' Try the top left(LTR) or top right(RTL) corner, and
							' then go across checking each cell for HORIZONTAL_WRAP.
							' Try the lower left corner, and then go across checking
							' each cell for other list layout orientation.
							Dim isHorizontalWrap As Boolean = (layoutOrientation = HORIZONTAL_WRAP)
							Dim visibleLocation As Point = If(isHorizontalWrap, New Point(lastPoint.x, r.y), New Point(r.x, lastPoint.y))
							Dim last As Integer
							Dim visIndex As Integer = -1
							Dim lIndex As Integer = ___location
							___location = -1
    
							Do
								last = visIndex
								visIndex = locationToIndex(visibleLocation)
    
								If visIndex <> -1 Then
									___bounds = getCellBounds(visIndex, visIndex)
									If visIndex <> lIndex AndAlso ___bounds IsNot Nothing AndAlso ___bounds.contains(visibleLocation) Then
										___location = visIndex
										If isHorizontalWrap Then
											visibleLocation.y = ___bounds.y + ___bounds.height
											If visibleLocation.y >= lastPoint.y Then last = visIndex
										Else
											visibleLocation.x = ___bounds.x + ___bounds.width
											If visibleLocation.x >= lastPoint.x Then last = visIndex
										End If
    
									Else
										last = visIndex
									End If
								End If
							Loop While visIndex <> -1 AndAlso last <> visIndex
						End If
					End If
				End If
				Return ___location
			End Get
		End Property


		''' <summary>
		''' Scrolls the list within an enclosing viewport to make the specified
		''' cell completely visible. This calls {@code scrollRectToVisible} with
		''' the bounds of the specified cell. For this method to work, the
		''' {@code JList} must be within a <code>JViewport</code>.
		''' <p>
		''' If the given index is outside the list's range of cells, this method
		''' results in nothing.
		''' </summary>
		''' <param name="index">  the index of the cell to make visible </param>
		''' <seealso cref= JComponent#scrollRectToVisible </seealso>
		''' <seealso cref= #getVisibleRect </seealso>
		Public Overridable Sub ensureIndexIsVisible(ByVal index As Integer)
			Dim ___cellBounds As Rectangle = getCellBounds(index, index)
			If ___cellBounds IsNot Nothing Then scrollRectToVisible(___cellBounds)
		End Sub

		''' <summary>
		''' Turns on or off automatic drag handling. In order to enable automatic
		''' drag handling, this property should be set to {@code true}, and the
		''' list's {@code TransferHandler} needs to be {@code non-null}.
		''' The default value of the {@code dragEnabled} property is {@code false}.
		''' <p>
		''' The job of honoring this property, and recognizing a user drag gesture,
		''' lies with the look and feel implementation, and in particular, the list's
		''' {@code ListUI}. When automatic drag handling is enabled, most look and
		''' feels (including those that subclass {@code BasicLookAndFeel}) begin a
		''' drag and drop operation whenever the user presses the mouse button over
		''' an item and then moves the mouse a few pixels. Setting this property to
		''' {@code true} can therefore have a subtle effect on how selections behave.
		''' <p>
		''' If a look and feel is used that ignores this property, you can still
		''' begin a drag and drop operation by calling {@code exportAsDrag} on the
		''' list's {@code TransferHandler}.
		''' </summary>
		''' <param name="b"> whether or not to enable automatic drag handling </param>
		''' <exception cref="HeadlessException"> if
		'''            <code>b</code> is <code>true</code> and
		'''            <code>GraphicsEnvironment.isHeadless()</code>
		'''            returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #getDragEnabled </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.4
		''' 
		''' @beaninfo
		'''  description: determines whether automatic drag handling is enabled
		'''        bound: false </seealso>
		Public Overridable Property dragEnabled As Boolean
			Set(ByVal b As Boolean)
				If b AndAlso GraphicsEnvironment.headless Then Throw New HeadlessException
				dragEnabled = b
			End Set
			Get
				Return dragEnabled
			End Get
		End Property


		''' <summary>
		''' Sets the drop mode for this component. For backward compatibility,
		''' the default for this property is <code>DropMode.USE_SELECTION</code>.
		''' Usage of one of the other modes is recommended, however, for an
		''' improved user experience. <code>DropMode.ON</code>, for instance,
		''' offers similar behavior of showing items as selected, but does so without
		''' affecting the actual selection in the list.
		''' <p>
		''' <code>JList</code> supports the following drop modes:
		''' <ul>
		'''    <li><code>DropMode.USE_SELECTION</code></li>
		'''    <li><code>DropMode.ON</code></li>
		'''    <li><code>DropMode.INSERT</code></li>
		'''    <li><code>DropMode.ON_OR_INSERT</code></li>
		''' </ul>
		''' The drop mode is only meaningful if this component has a
		''' <code>TransferHandler</code> that accepts drops.
		''' </summary>
		''' <param name="dropMode"> the drop mode to use </param>
		''' <exception cref="IllegalArgumentException"> if the drop mode is unsupported
		'''         or <code>null</code> </exception>
		''' <seealso cref= #getDropMode </seealso>
		''' <seealso cref= #getDropLocation </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.6 </seealso>
		Public Property dropMode As DropMode
			Set(ByVal dropMode As DropMode)
				If dropMode IsNot Nothing Then
					Select Case dropMode
						Case DropMode.USE_SELECTION, ON, INSERT, ON_OR_INSERT
							Me.dropMode = dropMode
							Return
					End Select
				End If
    
				Throw New System.ArgumentException(dropMode & ": Unsupported drop mode for list")
			End Set
			Get
				Return dropMode
			End Get
		End Property


		''' <summary>
		''' Calculates a drop location in this component, representing where a
		''' drop at the given point should insert data.
		''' </summary>
		''' <param name="p"> the point to calculate a drop location for </param>
		''' <returns> the drop location, or <code>null</code> </returns>
		Friend Overrides Function dropLocationForPoint(ByVal p As Point) As DropLocation
			Dim ___location As DropLocation = Nothing
			Dim rect As Rectangle = Nothing

			Dim index As Integer = locationToIndex(p)
			If index <> -1 Then rect = getCellBounds(index, index)

			Select Case dropMode
				Case DropMode.USE_SELECTION, ON
					___location = New DropLocation(p,If(rect IsNot Nothing AndAlso rect.contains(p), index, -1), False)

				Case DropMode.INSERT
					If index = -1 Then
						___location = New DropLocation(p, model.size, True)
						Exit Select
					End If

					If layoutOrientation = HORIZONTAL_WRAP Then
						Dim ltr As Boolean = componentOrientation.leftToRight

						If sun.swing.SwingUtilities2.liesInHorizontal(rect, p, ltr, False) = TRAILING Then
							index += 1
						' special case for below all cells
						ElseIf index = model.size - 1 AndAlso p.y >= rect.y + rect.height Then
							index += 1
						End If
					Else
						If sun.swing.SwingUtilities2.liesInVertical(rect, p, False) = TRAILING Then index += 1
					End If

					___location = New DropLocation(p, index, True)

				Case DropMode.ON_OR_INSERT
					If index = -1 Then
						___location = New DropLocation(p, model.size, True)
						Exit Select
					End If

					Dim between As Boolean = False

					If layoutOrientation = HORIZONTAL_WRAP Then
						Dim ltr As Boolean = componentOrientation.leftToRight

						Dim section As sun.swing.SwingUtilities2.Section = sun.swing.SwingUtilities2.liesInHorizontal(rect, p, ltr, True)
						If section Is TRAILING Then
							index += 1
							between = True
						' special case for below all cells
						ElseIf index = model.size - 1 AndAlso p.y >= rect.y + rect.height Then
							index += 1
							between = True
						ElseIf section Is LEADING Then
							between = True
						End If
					Else
						Dim section As sun.swing.SwingUtilities2.Section = sun.swing.SwingUtilities2.liesInVertical(rect, p, True)
						If section Is LEADING Then
							between = True
						ElseIf section Is TRAILING Then
							index += 1
							between = True
						End If
					End If

					___location = New DropLocation(p, index, between)

				Case Else
					Debug.Assert(False, "Unexpected drop mode")
			End Select

			Return ___location
		End Function

		''' <summary>
		''' Called to set or clear the drop location during a DnD operation.
		''' In some cases, the component may need to use it's internal selection
		''' temporarily to indicate the drop location. To help facilitate this,
		''' this method returns and accepts as a parameter a state object.
		''' This state object can be used to store, and later restore, the selection
		''' state. Whatever this method returns will be passed back to it in
		''' future calls, as the state parameter. If it wants the DnD system to
		''' continue storing the same state, it must pass it back every time.
		''' Here's how this is used:
		''' <p>
		''' Let's say that on the first call to this method the component decides
		''' to save some state (because it is about to use the selection to show
		''' a drop index). It can return a state object to the caller encapsulating
		''' any saved selection state. On a second call, let's say the drop location
		''' is being changed to something else. The component doesn't need to
		''' restore anything yet, so it simply passes back the same state object
		''' to have the DnD system continue storing it. Finally, let's say this
		''' method is messaged with <code>null</code>. This means DnD
		''' is finished with this component for now, meaning it should restore
		''' state. At this point, it can use the state parameter to restore
		''' said state, and of course return <code>null</code> since there's
		''' no longer anything to store.
		''' </summary>
		''' <param name="location"> the drop location (as calculated by
		'''        <code>dropLocationForPoint</code>) or <code>null</code>
		'''        if there's no longer a valid drop location </param>
		''' <param name="state"> the state object saved earlier for this component,
		'''        or <code>null</code> </param>
		''' <param name="forDrop"> whether or not the method is being called because an
		'''        actual drop occurred </param>
		''' <returns> any saved state for this component, or <code>null</code> if none </returns>
		Friend Overrides Function setDropLocation(ByVal location As TransferHandler.DropLocation, ByVal state As Object, ByVal forDrop As Boolean) As Object

			Dim retVal As Object = Nothing
			Dim listLocation As DropLocation = CType(location, DropLocation)

			If dropMode = DropMode.USE_SELECTION Then
				If listLocation Is Nothing Then
					If (Not forDrop) AndAlso state IsNot Nothing Then
						selectedIndices = CType(state, Integer()())(0)

						Dim anchor As Integer = CType(state, Integer()())(1)(0)
						Dim lead As Integer = CType(state, Integer()())(1)(1)

						sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(selectionModel, lead, anchor)
					End If
				Else
					If dropLocation Is Nothing Then
						Dim inds As Integer() = selectedIndices
						retVal = New Integer() {inds, {anchorSelectionIndex, leadSelectionIndex}}
					Else
						retVal = state
					End If

					Dim index As Integer = listLocation.index
					If index = -1 Then
						clearSelection()
						selectionModel.anchorSelectionIndex = -1
						selectionModel.leadSelectionIndex = -1
					Else
						selectionIntervalval(index, index)
					End If
				End If
			End If

			Dim old As DropLocation = dropLocation
			dropLocation = listLocation
			firePropertyChange("dropLocation", old, dropLocation)

			Return retVal
		End Function

		''' <summary>
		''' Returns the location that this component should visually indicate
		''' as the drop location during a DnD operation over the component,
		''' or {@code null} if no location is to currently be shown.
		''' <p>
		''' This method is not meant for querying the drop location
		''' from a {@code TransferHandler}, as the drop location is only
		''' set after the {@code TransferHandler}'s <code>canImport</code>
		''' has returned and has allowed for the location to be shown.
		''' <p>
		''' When this property changes, a property change event with
		''' name "dropLocation" is fired by the component.
		''' <p>
		''' By default, responsibility for listening for changes to this property
		''' and indicating the drop location visually lies with the list's
		''' {@code ListUI}, which may paint it directly and/or install a cell
		''' renderer to do so. Developers wishing to implement custom drop location
		''' painting and/or replace the default cell renderer, may need to honor
		''' this property.
		''' </summary>
		''' <returns> the drop location </returns>
		''' <seealso cref= #setDropMode </seealso>
		''' <seealso cref= TransferHandler#canImport(TransferHandler.TransferSupport)
		''' @since 1.6 </seealso>
		Public Property dropLocation As DropLocation
			Get
				Return dropLocation
			End Get
		End Property

		''' <summary>
		''' Returns the next list element whose {@code toString} value
		''' starts with the given prefix.
		''' </summary>
		''' <param name="prefix"> the string to test for a match </param>
		''' <param name="startIndex"> the index for starting the search </param>
		''' <param name="bias"> the search direction, either
		''' Position.Bias.Forward or Position.Bias.Backward. </param>
		''' <returns> the index of the next list element that
		''' starts with the prefix; otherwise {@code -1} </returns>
		''' <exception cref="IllegalArgumentException"> if prefix is {@code null}
		''' or startIndex is out of bounds
		''' @since 1.4 </exception>
		Public Overridable Function getNextMatch(ByVal prefix As String, ByVal startIndex As Integer, ByVal bias As javax.swing.text.Position.Bias) As Integer
			Dim ___model As ListModel(Of E) = model
			Dim max As Integer = ___model.size
			If prefix Is Nothing Then Throw New System.ArgumentException
			If startIndex < 0 OrElse startIndex >= max Then Throw New System.ArgumentException
			prefix = prefix.ToUpper()

			' start search from the next element after the selected element
			Dim increment As Integer = If(bias Is javax.swing.text.Position.Bias.Forward, 1, -1)
			Dim index As Integer = startIndex
			Do
				Dim element As E = ___model.getElementAt(index)

				If element IsNot Nothing Then
					Dim [string] As String

					If TypeOf element Is String Then
						[string] = CStr(element).ToUpper()
					Else
						[string] = element.ToString()
						If [string] IsNot Nothing Then [string] = [string].ToUpper()
					End If

					If [string] IsNot Nothing AndAlso [string].StartsWith(prefix) Then Return index
				End If
				index = (index + increment + max) Mod max
			Loop While index <> startIndex
			Return -1
		End Function

		''' <summary>
		''' Returns the tooltip text to be used for the given event. This overrides
		''' {@code JComponent}'s {@code getToolTipText} to first check the cell
		''' renderer component for the cell over which the event occurred, returning
		''' its tooltip text, if any. This implementation allows you to specify
		''' tooltip text on the cell level, by using {@code setToolTipText} on your
		''' cell renderer component.
		''' <p>
		''' <strong>Note:</strong> For <code>JList</code> to properly display the
		''' tooltips of its renderers in this manner, <code>JList</code> must be a
		''' registered component with the <code>ToolTipManager</code>. This registration
		''' is done automatically in the constructor. However, if at a later point
		''' <code>JList</code> is unregistered, by way of a call to
		''' {@code setToolTipText(null)}, tips from the renderers will no longer display.
		''' </summary>
		''' <param name="event"> the {@code MouseEvent} to fetch the tooltip text for </param>
		''' <seealso cref= JComponent#setToolTipText </seealso>
		''' <seealso cref= JComponent#getToolTipText </seealso>
		Public Overrides Function getToolTipText(ByVal [event] As MouseEvent) As String
			If [event] IsNot Nothing Then
				Dim p As Point = [event].point
				Dim index As Integer = locationToIndex(p)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim r As ListCellRenderer(Of ?) = cellRenderer
				Dim ___cellBounds As Rectangle

				___cellBounds = getCellBounds(index, index)
				If index <> -1 AndAlso r IsNot Nothing AndAlso ___cellBounds IsNot Nothing AndAlso ___cellBounds.contains(p.x, p.y) Then
					Dim lsm As ListSelectionModel = selectionModel
					Dim rComponent As Component = r.getListCellRendererComponent(Me, model.getElementAt(index), index, lsm.isSelectedIndex(index), (hasFocus() AndAlso (lsm.leadSelectionIndex = index)))

					If TypeOf rComponent Is JComponent Then
						Dim newEvent As MouseEvent

						p.translate(-___cellBounds.x, -___cellBounds.y)
						newEvent = New MouseEvent(rComponent, [event].iD, [event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)

						Dim tip As String = CType(rComponent, JComponent).getToolTipText(newEvent)

						If tip IsNot Nothing Then Return tip
					End If
				End If
			End If
			Return MyBase.toolTipText
		End Function

		''' <summary>
		''' --- ListUI Delegations ---
		''' </summary>


		''' <summary>
		''' Returns the cell index closest to the given location in the list's
		''' coordinate system. To determine if the cell actually contains the
		''' specified location, compare the point against the cell's bounds,
		''' as provided by {@code getCellBounds}. This method returns {@code -1}
		''' if the model is empty
		''' <p>
		''' This is a cover method that delegates to the method of the same name
		''' in the list's {@code ListUI}. It returns {@code -1} if the list has
		''' no {@code ListUI}.
		''' </summary>
		''' <param name="location"> the coordinates of the point </param>
		''' <returns> the cell index closest to the given location, or {@code -1} </returns>
		Public Overridable Function locationToIndex(ByVal location As Point) As Integer
			Dim ___ui As ListUI = uI
			Return If(___ui IsNot Nothing, ___ui.locationToIndex(Me, location), -1)
		End Function


		''' <summary>
		''' Returns the origin of the specified item in the list's coordinate
		''' system. This method returns {@code null} if the index isn't valid.
		''' <p>
		''' This is a cover method that delegates to the method of the same name
		''' in the list's {@code ListUI}. It returns {@code null} if the list has
		''' no {@code ListUI}.
		''' </summary>
		''' <param name="index"> the cell index </param>
		''' <returns> the origin of the cell, or {@code null} </returns>
		Public Overridable Function indexToLocation(ByVal index As Integer) As Point
			Dim ___ui As ListUI = uI
			Return If(___ui IsNot Nothing, ___ui.indexToLocation(Me, index), Nothing)
		End Function


		''' <summary>
		''' Returns the bounding rectangle, in the list's coordinate system,
		''' for the range of cells specified by the two indices.
		''' These indices can be supplied in any order.
		''' <p>
		''' If the smaller index is outside the list's range of cells, this method
		''' returns {@code null}. If the smaller index is valid, but the larger
		''' index is outside the list's range, the bounds of just the first index
		''' is returned. Otherwise, the bounds of the valid range is returned.
		''' <p>
		''' This is a cover method that delegates to the method of the same name
		''' in the list's {@code ListUI}. It returns {@code null} if the list has
		''' no {@code ListUI}.
		''' </summary>
		''' <param name="index0"> the first index in the range </param>
		''' <param name="index1"> the second index in the range </param>
		''' <returns> the bounding rectangle for the range of cells, or {@code null} </returns>
		Public Overridable Function getCellBounds(ByVal index0 As Integer, ByVal index1 As Integer) As Rectangle
			Dim ___ui As ListUI = uI
			Return If(___ui IsNot Nothing, ___ui.getCellBounds(Me, index0, index1), Nothing)
		End Function


		''' <summary>
		''' --- ListModel Support ---
		''' </summary>


		''' <summary>
		''' Returns the data model that holds the list of items displayed
		''' by the <code>JList</code> component.
		''' </summary>
		''' <returns> the <code>ListModel</code> that provides the displayed
		'''                          list of items </returns>
		''' <seealso cref= #setModel </seealso>
		Public Overridable Property model As ListModel(Of E)
			Get
				Return dataModel
			End Get
			Set(ByVal model As ListModel(Of E))
				If model Is Nothing Then Throw New System.ArgumentException("model must be non null")
				Dim oldValue As ListModel(Of E) = dataModel
				dataModel = model
				firePropertyChange("model", oldValue, dataModel)
				clearSelection()
			End Set
		End Property



		''' <summary>
		''' Constructs a read-only <code>ListModel</code> from an array of items,
		''' and calls {@code setModel} with this model.
		''' <p>
		''' Attempts to pass a {@code null} value to this method results in
		''' undefined behavior and, most likely, exceptions. The created model
		''' references the given array directly. Attempts to modify the array
		''' after invoking this method results in undefined behavior.
		''' </summary>
		''' <param name="listData"> an array of {@code E} containing the items to
		'''        display in the list </param>
		''' <seealso cref= #setModel </seealso>
		Public Overridable Property listData As E()
			Set(ByVal listData As E())
				modeldel(New AbstractListModelAnonymousInnerClassHelper4(Of E)
			   )
			End Set
		End Property

		Private Class AbstractListModelAnonymousInnerClassHelper4(Of E)
			Inherits AbstractListModel(Of E)

			Public Overridable Property size As Integer
				Get
					Return listData.length
				End Get
			End Property
			Public Overridable Function getElementAt(ByVal i As Integer) As E
				Return listData(i)
			End Function
		End Class


		''' <summary>
		''' Constructs a read-only <code>ListModel</code> from a <code>Vector</code>
		''' and calls {@code setModel} with this model.
		''' <p>
		''' Attempts to pass a {@code null} value to this method results in
		''' undefined behavior and, most likely, exceptions. The created model
		''' references the given {@code Vector} directly. Attempts to modify the
		''' {@code Vector} after invoking this method results in undefined behavior.
		''' </summary>
		''' <param name="listData"> a <code>Vector</code> containing the items to
		'''                                          display in the list </param>
		''' <seealso cref= #setModel </seealso>
		Public Overridable Property listData(Of T1 As E) As List(Of T1)
			Set(ByVal listData As List(Of T1))
				modeldel(New AbstractListModelAnonymousInnerClassHelper5(Of E)
			   )
			End Set
		End Property

		Private Class AbstractListModelAnonymousInnerClassHelper5(Of E)
			Inherits AbstractListModel(Of E)

			Public Overridable Property size As Integer
				Get
					Return listData.size()
				End Get
			End Property
			Public Overridable Function getElementAt(ByVal i As Integer) As E
				Return listData.elementAt(i)
			End Function
		End Class


		''' <summary>
		''' --- ListSelectionModel delegations and extensions ---
		''' </summary>


		''' <summary>
		''' Returns an instance of {@code DefaultListSelectionModel}; called
		''' during construction to initialize the list's selection model
		''' property.
		''' </summary>
		''' <returns> a {@code DefaultListSelecitonModel}, used to initialize
		'''         the list's selection model property during construction </returns>
		''' <seealso cref= #setSelectionModel </seealso>
		''' <seealso cref= DefaultListSelectionModel </seealso>
		Protected Friend Overridable Function createSelectionModel() As ListSelectionModel
			Return New DefaultListSelectionModel
		End Function


		''' <summary>
		''' Returns the current selection model. The selection model maintains the
		''' selection state of the list. See the class level documentation for more
		''' details.
		''' </summary>
		''' <returns> the <code>ListSelectionModel</code> that maintains the
		'''         list's selections
		''' </returns>
		''' <seealso cref= #setSelectionModel </seealso>
		''' <seealso cref= ListSelectionModel </seealso>
		Public Overridable Property selectionModel As ListSelectionModel
			Get
				Return selectionModel
			End Get
		End Property


		''' <summary>
		''' Notifies {@code ListSelectionListener}s added directly to the list
		''' of selection changes made to the selection model. {@code JList}
		''' listens for changes made to the selection in the selection model,
		''' and forwards notification to listeners added to the list directly,
		''' by calling this method.
		''' <p>
		''' This method constructs a {@code ListSelectionEvent} with this list
		''' as the source, and the specified arguments, and sends it to the
		''' registered {@code ListSelectionListeners}.
		''' </summary>
		''' <param name="firstIndex"> the first index in the range, {@code <= lastIndex} </param>
		''' <param name="lastIndex"> the last index in the range, {@code >= firstIndex} </param>
		''' <param name="isAdjusting"> whether or not this is one in a series of
		'''        multiple events, where changes are still being made
		''' </param>
		''' <seealso cref= #addListSelectionListener </seealso>
		''' <seealso cref= #removeListSelectionListener </seealso>
		''' <seealso cref= javax.swing.event.ListSelectionEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireSelectionValueChanged(ByVal firstIndex As Integer, ByVal lastIndex As Integer, ByVal isAdjusting As Boolean)
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ListSelectionEvent = Nothing

			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ListSelectionListener) Then
					If e Is Nothing Then e = New ListSelectionEvent(Me, firstIndex, lastIndex, isAdjusting)
					CType(___listeners(i+1), ListSelectionListener).valueChanged(e)
				End If
			Next i
		End Sub


	'     A ListSelectionListener that forwards ListSelectionEvents from
	'     * the selectionModel to the JList ListSelectionListeners.  The
	'     * forwarded events only differ from the originals in that their
	'     * source is the JList instead of the selectionModel itself.
	'     
		<Serializable> _
		Private Class ListSelectionHandler
			Implements ListSelectionListener

			Private ReadOnly outerInstance As JList

			Public Sub New(ByVal outerInstance As JList)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				outerInstance.fireSelectionValueChanged(e.firstIndex, e.lastIndex, e.valueIsAdjusting)
			End Sub
		End Class


		''' <summary>
		''' Adds a listener to the list, to be notified each time a change to the
		''' selection occurs; the preferred way of listening for selection state
		''' changes. {@code JList} takes care of listening for selection state
		''' changes in the selection model, and notifies the given listener of
		''' each change. {@code ListSelectionEvent}s sent to the listener have a
		''' {@code source} property set to this list.
		''' </summary>
		''' <param name="listener"> the {@code ListSelectionListener} to add </param>
		''' <seealso cref= #getSelectionModel </seealso>
		''' <seealso cref= #getListSelectionListeners </seealso>
		Public Overridable Sub addListSelectionListener(ByVal listener As ListSelectionListener)
			If selectionListener Is Nothing Then
				selectionListener = New ListSelectionHandler(Me)
				selectionModel.addListSelectionListener(selectionListener)
			End If

			listenerList.add(GetType(ListSelectionListener), listener)
		End Sub


		''' <summary>
		''' Removes a selection listener from the list.
		''' </summary>
		''' <param name="listener"> the {@code ListSelectionListener} to remove </param>
		''' <seealso cref= #addListSelectionListener </seealso>
		''' <seealso cref= #getSelectionModel </seealso>
		Public Overridable Sub removeListSelectionListener(ByVal listener As ListSelectionListener)
			listenerList.remove(GetType(ListSelectionListener), listener)
		End Sub


		''' <summary>
		''' Returns an array of all the {@code ListSelectionListener}s added
		''' to this {@code JList} by way of {@code addListSelectionListener}.
		''' </summary>
		''' <returns> all of the {@code ListSelectionListener}s on this list, or
		'''         an empty array if no listeners have been added </returns>
		''' <seealso cref= #addListSelectionListener
		''' @since 1.4 </seealso>
		Public Overridable Property listSelectionListeners As ListSelectionListener()
			Get
				Return listenerList.getListeners(GetType(ListSelectionListener))
			End Get
		End Property


		''' <summary>
		''' Sets the <code>selectionModel</code> for the list to a
		''' non-<code>null</code> <code>ListSelectionModel</code>
		''' implementation. The selection model handles the task of making single
		''' selections, selections of contiguous ranges, and non-contiguous
		''' selections.
		''' <p>
		''' This is a JavaBeans bound property.
		''' </summary>
		''' <param name="selectionModel">  the <code>ListSelectionModel</code> that
		'''                          implements the selections </param>
		''' <exception cref="IllegalArgumentException">   if <code>selectionModel</code>
		'''                                          is <code>null</code> </exception>
		''' <seealso cref= #getSelectionModel
		''' @beaninfo
		'''       bound: true
		''' description: The selection model, recording which cells are selected. </seealso>
		Public Overridable Property selectionModel As ListSelectionModel
			Set(ByVal selectionModel As ListSelectionModel)
				If selectionModel Is Nothing Then Throw New System.ArgumentException("selectionModel must be non null")
    
		'         Remove the forwarding ListSelectionListener from the old
		'         * selectionModel, and add it to the new one, if necessary.
		'         
				If selectionListener IsNot Nothing Then
					Me.selectionModel.removeListSelectionListener(selectionListener)
					selectionModel.addListSelectionListener(selectionListener)
				End If
    
				Dim oldValue As ListSelectionModel = Me.selectionModel
				Me.selectionModel = selectionModel
				firePropertyChange("selectionModel", oldValue, selectionModel)
			End Set
		End Property


		''' <summary>
		''' Sets the selection mode for the list. This is a cover method that sets
		''' the selection mode directly on the selection model.
		''' <p>
		''' The following list describes the accepted selection modes:
		''' <ul>
		''' <li>{@code ListSelectionModel.SINGLE_SELECTION} -
		'''   Only one list index can be selected at a time. In this mode,
		'''   {@code setSelectionInterval} and {@code addSelectionInterval} are
		'''   equivalent, both replacing the current selection with the index
		'''   represented by the second argument (the "lead").
		''' <li>{@code ListSelectionModel.SINGLE_INTERVAL_SELECTION} -
		'''   Only one contiguous interval can be selected at a time.
		'''   In this mode, {@code addSelectionInterval} behaves like
		'''   {@code setSelectionInterval} (replacing the current selection},
		'''   unless the given interval is immediately adjacent to or overlaps
		'''   the existing selection, and can be used to grow the selection.
		''' <li>{@code ListSelectionModel.MULTIPLE_INTERVAL_SELECTION} -
		'''   In this mode, there's no restriction on what can be selected.
		'''   This mode is the default.
		''' </ul>
		''' </summary>
		''' <param name="selectionMode"> the selection mode </param>
		''' <seealso cref= #getSelectionMode </seealso>
		''' <exception cref="IllegalArgumentException"> if the selection mode isn't
		'''         one of those allowed
		''' @beaninfo
		''' description: The selection mode.
		'''        enum: SINGLE_SELECTION            ListSelectionModel.SINGLE_SELECTION
		'''              SINGLE_INTERVAL_SELECTION   ListSelectionModel.SINGLE_INTERVAL_SELECTION
		'''              MULTIPLE_INTERVAL_SELECTION ListSelectionModel.MULTIPLE_INTERVAL_SELECTION </exception>
		Public Overridable Property selectionMode As Integer
			Set(ByVal selectionMode As Integer)
				selectionModel.selectionMode = selectionMode
			End Set
			Get
				Return selectionModel.selectionMode
			End Get
		End Property



		''' <summary>
		''' Returns the anchor selection index. This is a cover method that
		''' delegates to the method of the same name on the list's selection model.
		''' </summary>
		''' <returns> the anchor selection index </returns>
		''' <seealso cref= ListSelectionModel#getAnchorSelectionIndex </seealso>
		Public Overridable Property anchorSelectionIndex As Integer
			Get
				Return selectionModel.anchorSelectionIndex
			End Get
		End Property


		''' <summary>
		''' Returns the lead selection index. This is a cover method that
		''' delegates to the method of the same name on the list's selection model.
		''' </summary>
		''' <returns> the lead selection index </returns>
		''' <seealso cref= ListSelectionModel#getLeadSelectionIndex
		''' @beaninfo
		''' description: The lead selection index. </seealso>
		Public Overridable Property leadSelectionIndex As Integer
			Get
				Return selectionModel.leadSelectionIndex
			End Get
		End Property


		''' <summary>
		''' Returns the smallest selected cell index, or {@code -1} if the selection
		''' is empty. This is a cover method that delegates to the method of the same
		''' name on the list's selection model.
		''' </summary>
		''' <returns> the smallest selected cell index, or {@code -1} </returns>
		''' <seealso cref= ListSelectionModel#getMinSelectionIndex </seealso>
		Public Overridable Property minSelectionIndex As Integer
			Get
				Return selectionModel.minSelectionIndex
			End Get
		End Property


		''' <summary>
		''' Returns the largest selected cell index, or {@code -1} if the selection
		''' is empty. This is a cover method that delegates to the method of the same
		''' name on the list's selection model.
		''' </summary>
		''' <returns> the largest selected cell index </returns>
		''' <seealso cref= ListSelectionModel#getMaxSelectionIndex </seealso>
		Public Overridable Property maxSelectionIndex As Integer
			Get
				Return selectionModel.maxSelectionIndex
			End Get
		End Property


		''' <summary>
		''' Returns {@code true} if the specified index is selected,
		''' else {@code false}. This is a cover method that delegates to the method
		''' of the same name on the list's selection model.
		''' </summary>
		''' <param name="index"> index to be queried for selection state </param>
		''' <returns> {@code true} if the specified index is selected,
		'''         else {@code false} </returns>
		''' <seealso cref= ListSelectionModel#isSelectedIndex </seealso>
		''' <seealso cref= #setSelectedIndex </seealso>
		Public Overridable Function isSelectedIndex(ByVal index As Integer) As Boolean
			Return selectionModel.isSelectedIndex(index)
		End Function


		''' <summary>
		''' Returns {@code true} if nothing is selected, else {@code false}.
		''' This is a cover method that delegates to the method of the same
		''' name on the list's selection model.
		''' </summary>
		''' <returns> {@code true} if nothing is selected, else {@code false} </returns>
		''' <seealso cref= ListSelectionModel#isSelectionEmpty </seealso>
		''' <seealso cref= #clearSelection </seealso>
		Public Overridable Property selectionEmpty As Boolean
			Get
				Return selectionModel.selectionEmpty
			End Get
		End Property


		''' <summary>
		''' Clears the selection; after calling this method, {@code isSelectionEmpty}
		''' will return {@code true}. This is a cover method that delegates to the
		''' method of the same name on the list's selection model.
		''' </summary>
		''' <seealso cref= ListSelectionModel#clearSelection </seealso>
		''' <seealso cref= #isSelectionEmpty </seealso>
		Public Overridable Sub clearSelection()
			selectionModel.clearSelection()
		End Sub


		''' <summary>
		''' Selects the specified interval. Both {@code anchor} and {@code lead}
		''' indices are included. {@code anchor} doesn't have to be less than or
		''' equal to {@code lead}. This is a cover method that delegates to the
		''' method of the same name on the list's selection model.
		''' <p>
		''' Refer to the documentation of the selection model class being used
		''' for details on how values less than {@code 0} are handled.
		''' </summary>
		''' <param name="anchor"> the first index to select </param>
		''' <param name="lead"> the last index to select </param>
		''' <seealso cref= ListSelectionModel#setSelectionInterval </seealso>
		''' <seealso cref= DefaultListSelectionModel#setSelectionInterval </seealso>
		''' <seealso cref= #createSelectionModel </seealso>
		''' <seealso cref= #addSelectionInterval </seealso>
		''' <seealso cref= #removeSelectionInterval </seealso>
		Public Overridable Sub setSelectionInterval(ByVal anchor As Integer, ByVal lead As Integer)
			selectionModel.selectionIntervalval(anchor, lead)
		End Sub


		''' <summary>
		''' Sets the selection to be the union of the specified interval with current
		''' selection. Both the {@code anchor} and {@code lead} indices are
		''' included. {@code anchor} doesn't have to be less than or
		''' equal to {@code lead}. This is a cover method that delegates to the
		''' method of the same name on the list's selection model.
		''' <p>
		''' Refer to the documentation of the selection model class being used
		''' for details on how values less than {@code 0} are handled.
		''' </summary>
		''' <param name="anchor"> the first index to add to the selection </param>
		''' <param name="lead"> the last index to add to the selection </param>
		''' <seealso cref= ListSelectionModel#addSelectionInterval </seealso>
		''' <seealso cref= DefaultListSelectionModel#addSelectionInterval </seealso>
		''' <seealso cref= #createSelectionModel </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		''' <seealso cref= #removeSelectionInterval </seealso>
		Public Overridable Sub addSelectionInterval(ByVal anchor As Integer, ByVal lead As Integer)
			selectionModel.addSelectionInterval(anchor, lead)
		End Sub


		''' <summary>
		''' Sets the selection to be the set difference of the specified interval
		''' and the current selection. Both the {@code index0} and {@code index1}
		''' indices are removed. {@code index0} doesn't have to be less than or
		''' equal to {@code index1}. This is a cover method that delegates to the
		''' method of the same name on the list's selection model.
		''' <p>
		''' Refer to the documentation of the selection model class being used
		''' for details on how values less than {@code 0} are handled.
		''' </summary>
		''' <param name="index0"> the first index to remove from the selection </param>
		''' <param name="index1"> the last index to remove from the selection </param>
		''' <seealso cref= ListSelectionModel#removeSelectionInterval </seealso>
		''' <seealso cref= DefaultListSelectionModel#removeSelectionInterval </seealso>
		''' <seealso cref= #createSelectionModel </seealso>
		''' <seealso cref= #setSelectionInterval </seealso>
		''' <seealso cref= #addSelectionInterval </seealso>
		Public Overridable Sub removeSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			selectionModel.removeSelectionInterval(index0, index1)
		End Sub


		''' <summary>
		''' Sets the selection model's {@code valueIsAdjusting} property. When
		''' {@code true}, upcoming changes to selection should be considered part
		''' of a single change. This property is used internally and developers
		''' typically need not call this method. For example, when the model is being
		''' updated in response to a user drag, the value of the property is set
		''' to {@code true} when the drag is initiated and set to {@code false}
		''' when the drag is finished. This allows listeners to update only
		''' when a change has been finalized, rather than handling all of the
		''' intermediate values.
		''' <p>
		''' You may want to use this directly if making a series of changes
		''' that should be considered part of a single change.
		''' <p>
		''' This is a cover method that delegates to the method of the same name on
		''' the list's selection model. See the documentation for
		''' <seealso cref="javax.swing.ListSelectionModel#setValueIsAdjusting"/> for
		''' more details.
		''' </summary>
		''' <param name="b"> the new value for the property </param>
		''' <seealso cref= ListSelectionModel#setValueIsAdjusting </seealso>
		''' <seealso cref= javax.swing.event.ListSelectionEvent#getValueIsAdjusting </seealso>
		''' <seealso cref= #getValueIsAdjusting </seealso>
		Public Overridable Property valueIsAdjusting As Boolean
			Set(ByVal b As Boolean)
				selectionModel.valueIsAdjusting = b
			End Set
			Get
				Return selectionModel.valueIsAdjusting
			End Get
		End Property




		''' <summary>
		''' Returns an array of all of the selected indices, in increasing
		''' order.
		''' </summary>
		''' <returns> all of the selected indices, in increasing order,
		'''         or an empty array if nothing is selected </returns>
		''' <seealso cref= #removeSelectionInterval </seealso>
		''' <seealso cref= #addListSelectionListener </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getSelectedIndices() As Integer[] 'JavaToDotNetTempPropertyGetselectedIndices
		Public Overridable Property selectedIndices As Integer()
			Get
				Dim sm As ListSelectionModel = selectionModel
				Dim iMin As Integer = sm.minSelectionIndex
				Dim iMax As Integer = sm.maxSelectionIndex
    
				If (iMin < 0) OrElse (iMax < 0) Then Return New Integer(){}
    
				Dim rvTmp As Integer() = New Integer(1+ (iMax - iMin) - 1){}
				Dim n As Integer = 0
				For i As Integer = iMin To iMax
					If sm.isSelectedIndex(i) Then
						rvTmp(n) = i
						n += 1
					End If
				Next i
				Dim rv As Integer() = New Integer(n - 1){}
				Array.Copy(rvTmp, 0, rv, 0, n)
				Return rv
			End Get
			Set(ByVal indices As Integer())
		End Property


		''' <summary>
		''' Selects a single cell. Does nothing if the given index is greater
		''' than or equal to the model size. This is a convenience method that uses
		''' {@code setSelectionInterval} on the selection model. Refer to the
		''' documentation for the selection model class being used for details on
		''' how values less than {@code 0} are handled.
		''' </summary>
		''' <param name="index"> the index of the cell to select </param>
		''' <seealso cref= ListSelectionModel#setSelectionInterval </seealso>
		''' <seealso cref= #isSelectedIndex </seealso>
		''' <seealso cref= #addListSelectionListener
		''' @beaninfo
		''' description: The index of the selected cell. </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSelectedIndex(ByVal index As Integer) 'JavaToDotNetTempPropertySetselectedIndex
		Public Overridable Property selectedIndex As Integer
			Set(ByVal index As Integer)
				If index >= model.size Then Return
				selectionModel.selectionIntervalval(index, index)
			End Set
			Get
		End Property


			Dim sm As ListSelectionModel = selectionModel
			sm.clearSelection()
			Dim ___size As Integer = model.size
			For Each i As Integer In indices
				If i < ___size Then sm.addSelectionInterval(i, i)
			Next i
		End Sub


		''' <summary>
		''' Returns an array of all the selected values, in increasing order based
		''' on their indices in the list.
		''' </summary>
		''' <returns> the selected values, or an empty array if nothing is selected </returns>
		''' <seealso cref= #isSelectedIndex </seealso>
		''' <seealso cref= #getModel </seealso>
		''' <seealso cref= #addListSelectionListener
		''' </seealso>
		''' @deprecated As of JDK 1.7, replaced by <seealso cref="#getSelectedValuesList()"/> 
		<Obsolete("As of JDK 1.7, replaced by <seealso cref="#getSelectedValuesList()"/>")> _
		Public Overridable Property selectedValues As Object()
			Get
				Dim sm As ListSelectionModel = selectionModel
				Dim dm As ListModel(Of E) = model
    
				Dim iMin As Integer = sm.minSelectionIndex
				Dim iMax As Integer = sm.maxSelectionIndex
    
				If (iMin < 0) OrElse (iMax < 0) Then Return New Object(){}
    
				Dim rvTmp As Object() = New Object(1+ (iMax - iMin) - 1){}
				Dim n As Integer = 0
				For i As Integer = iMin To iMax
					If sm.isSelectedIndex(i) Then
						rvTmp(n) = dm.getElementAt(i)
						n += 1
					End If
				Next i
				Dim rv As Object() = New Object(n - 1){}
				Array.Copy(rvTmp, 0, rv, 0, n)
				Return rv
			End Get
		End Property

		''' <summary>
		''' Returns a list of all the selected items, in increasing order based
		''' on their indices in the list.
		''' </summary>
		''' <returns> the selected items, or an empty list if nothing is selected </returns>
		''' <seealso cref= #isSelectedIndex </seealso>
		''' <seealso cref= #getModel </seealso>
		''' <seealso cref= #addListSelectionListener
		''' 
		''' @since 1.7 </seealso>
		Public Overridable Property selectedValuesList As IList(Of E)
			Get
				Dim sm As ListSelectionModel = selectionModel
				Dim dm As ListModel(Of E) = model
    
				Dim iMin As Integer = sm.minSelectionIndex
				Dim iMax As Integer = sm.maxSelectionIndex
    
				If (iMin < 0) OrElse (iMax < 0) Then Return java.util.Collections.emptyList()
    
				Dim selectedItems As IList(Of E) = New List(Of E)
				For i As Integer = iMin To iMax
					If sm.isSelectedIndex(i) Then selectedItems.Add(dm.getElementAt(i))
				Next i
				Return selectedItems
			End Get
		End Property


			Return minSelectionIndex
		End Function


		''' <summary>
		''' Returns the value for the smallest selected cell index;
		''' <i>the selected value</i> when only a single item is selected in the
		''' list. When multiple items are selected, it is simply the value for the
		''' smallest selected index. Returns {@code null} if there is no selection.
		''' <p>
		''' This is a convenience method that simply returns the model value for
		''' {@code getMinSelectionIndex}.
		''' </summary>
		''' <returns> the first selected value </returns>
		''' <seealso cref= #getMinSelectionIndex </seealso>
		''' <seealso cref= #getModel </seealso>
		''' <seealso cref= #addListSelectionListener </seealso>
		Public Overridable Property selectedValue As E
			Get
				Dim i As Integer = minSelectionIndex
				Return If(i = -1, Nothing, model.getElementAt(i))
			End Get
		End Property


		''' <summary>
		''' Selects the specified object from the list.
		''' </summary>
		''' <param name="anObject">      the object to select </param>
		''' <param name="shouldScroll">  {@code true} if the list should scroll to display
		'''                      the selected object, if one exists; otherwise {@code false} </param>
		Public Overridable Sub setSelectedValue(ByVal anObject As Object, ByVal shouldScroll As Boolean)
			If anObject Is Nothing Then
				selectedIndex = -1
			ElseIf Not anObject.Equals(selectedValue) Then
				Dim i, c As Integer
				Dim dm As ListModel(Of E) = model
				i=0
				c=dm.size
				Do While i<c
					If anObject.Equals(dm.getElementAt(i)) Then
						selectedIndex = i
						If shouldScroll Then ensureIndexIsVisible(i)
						repaint() '* FIX-ME setSelectedIndex does not redraw all the time with the basic l&f*
						Return
					End If
					i += 1
				Loop
				selectedIndex = -1
			End If
			repaint() '* FIX-ME setSelectedIndex does not redraw all the time with the basic l&f*
		End Sub



		''' <summary>
		''' --- The Scrollable Implementation ---
		''' </summary>

		Private Sub checkScrollableParameters(ByVal visibleRect As Rectangle, ByVal orientation As Integer)
			If visibleRect Is Nothing Then Throw New System.ArgumentException("visibleRect must be non-null")
			Select Case orientation
			Case SwingConstants.VERTICAL, SwingConstants.HORIZONTAL
			Case Else
				Throw New System.ArgumentException("orientation must be one of: VERTICAL, HORIZONTAL")
			End Select
		End Sub


		''' <summary>
		''' Computes the size of viewport needed to display {@code visibleRowCount}
		''' rows. The value returned by this method depends on the layout
		''' orientation:
		''' <p>
		''' <b>{@code VERTICAL}:</b>
		''' <br>
		''' This is trivial if both {@code fixedCellWidth} and {@code fixedCellHeight}
		''' have been set (either explicitly or by specifying a prototype cell value).
		''' The width is simply the {@code fixedCellWidth} plus the list's horizontal
		''' insets. The height is the {@code fixedCellHeight} multiplied by the
		''' {@code visibleRowCount}, plus the list's vertical insets.
		''' <p>
		''' If either {@code fixedCellWidth} or {@code fixedCellHeight} haven't been
		''' specified, heuristics are used. If the model is empty, the width is
		''' the {@code fixedCellWidth}, if greater than {@code 0}, or a hard-coded
		''' value of {@code 256}. The height is the {@code fixedCellHeight} multiplied
		''' by {@code visibleRowCount}, if {@code fixedCellHeight} is greater than
		''' {@code 0}, otherwise it is a hard-coded value of {@code 16} multiplied by
		''' {@code visibleRowCount}.
		''' <p>
		''' If the model isn't empty, the width is the preferred size's width,
		''' typically the width of the widest list element. The height is the
		''' {@code fixedCellHeight} multiplied by the {@code visibleRowCount},
		''' plus the list's vertical insets.
		''' <p>
		''' <b>{@code VERTICAL_WRAP} or {@code HORIZONTAL_WRAP}:</b>
		''' <br>
		''' This method simply returns the value from {@code getPreferredSize}.
		''' The list's {@code ListUI} is expected to override {@code getPreferredSize}
		''' to return an appropriate value.
		''' </summary>
		''' <returns> a dimension containing the size of the viewport needed
		'''          to display {@code visibleRowCount} rows </returns>
		''' <seealso cref= #getPreferredScrollableViewportSize </seealso>
		''' <seealso cref= #setPrototypeCellValue </seealso>
		Public Overridable Property preferredScrollableViewportSize As Dimension
			Get
				If layoutOrientation <> VERTICAL Then Return preferredSize
				Dim ___insets As Insets = insets
				Dim dx As Integer = ___insets.left + ___insets.right
				Dim dy As Integer = ___insets.top + ___insets.bottom
    
				Dim ___visibleRowCount As Integer = visibleRowCount
				Dim ___fixedCellWidth As Integer = fixedCellWidth
				Dim ___fixedCellHeight As Integer = fixedCellHeight
    
				If (___fixedCellWidth > 0) AndAlso (___fixedCellHeight > 0) Then
					Dim ___width As Integer = ___fixedCellWidth + dx
					Dim ___height As Integer = (___visibleRowCount * ___fixedCellHeight) + dy
					Return New Dimension(___width, ___height)
				ElseIf model.size > 0 Then
					Dim ___width As Integer = preferredSize.width
					Dim ___height As Integer
					Dim r As Rectangle = getCellBounds(0, 0)
					If r IsNot Nothing Then
						___height = (___visibleRowCount * r.height) + dy
					Else
						' Will only happen if UI null, shouldn't matter what we return
						___height = 1
					End If
					Return New Dimension(___width, ___height)
				Else
					___fixedCellWidth = If(___fixedCellWidth > 0, ___fixedCellWidth, 256)
					___fixedCellHeight = If(___fixedCellHeight > 0, ___fixedCellHeight, 16)
					Return New Dimension(___fixedCellWidth, ___fixedCellHeight * ___visibleRowCount)
				End If
			End Get
		End Property


		''' <summary>
		''' Returns the distance to scroll to expose the next or previous
		''' row (for vertical scrolling) or column (for horizontal scrolling).
		''' <p>
		''' For horizontal scrolling, if the layout orientation is {@code VERTICAL},
		''' then the list's font size is returned (or {@code 1} if the font is
		''' {@code null}).
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> {@code SwingConstants.HORIZONTAL} or
		'''                    {@code SwingConstants.VERTICAL} </param>
		''' <param name="direction"> less or equal to zero to scroll up/back,
		'''                  greater than zero for down/forward </param>
		''' <returns> the "unit" increment for scrolling in the specified direction;
		'''         always positive </returns>
		''' <seealso cref= #getScrollableBlockIncrement </seealso>
		''' <seealso cref= Scrollable#getScrollableUnitIncrement </seealso>
		''' <exception cref="IllegalArgumentException"> if {@code visibleRect} is {@code null}, or
		'''         {@code orientation} isn't one of {@code SwingConstants.VERTICAL} or
		'''         {@code SwingConstants.HORIZONTAL} </exception>
		Public Overridable Function getScrollableUnitIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			checkScrollableParameters(visibleRect, orientation)

			If orientation = SwingConstants.VERTICAL Then
				Dim row As Integer = locationToIndex(visibleRect.location)

				If row = -1 Then
					Return 0
				Else
					' Scroll Down 
					If direction > 0 Then
						Dim r As Rectangle = getCellBounds(row, row)
						Return If(r Is Nothing, 0, r.height - (visibleRect.y - r.y))
					' Scroll Up 
					Else
						Dim r As Rectangle = getCellBounds(row, row)

	'                     The first row is completely visible and it's row 0.
	'                     * We're done.
	'                     
						If (r.y = visibleRect.y) AndAlso (row = 0) Then
							Return 0
	'                     The first row is completely visible, return the
	'                     * height of the previous row or 0 if the first row
	'                     * is the top row of the list.
	'                     
						ElseIf r.y = visibleRect.y Then
							Dim loc As Point = r.location
							loc.y -= 1
							Dim prevIndex As Integer = locationToIndex(loc)
							Dim prevR As Rectangle = getCellBounds(prevIndex, prevIndex)

							If prevR Is Nothing OrElse prevR.y >= r.y Then Return 0
							Return prevR.height
	'                     The first row is partially visible, return the
	'                     * height of hidden part.
	'                     
						Else
							Return visibleRect.y - r.y
						End If
					End If
				End If
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso layoutOrientation <> JList.VERTICAL Then
				Dim leftToRight As Boolean = componentOrientation.leftToRight
				Dim index As Integer
				Dim leadingPoint As Point

				If leftToRight Then
					leadingPoint = visibleRect.location
				Else
					leadingPoint = New Point(visibleRect.x + visibleRect.width -1, visibleRect.y)
				End If
				index = locationToIndex(leadingPoint)

				If index <> -1 Then
					Dim ___cellBounds As Rectangle = getCellBounds(index, index)
					If ___cellBounds IsNot Nothing AndAlso ___cellBounds.contains(leadingPoint) Then
						Dim leadingVisibleEdge As Integer
						Dim leadingCellEdge As Integer

						If leftToRight Then
							leadingVisibleEdge = visibleRect.x
							leadingCellEdge = ___cellBounds.x
						Else
							leadingVisibleEdge = visibleRect.x + visibleRect.width
							leadingCellEdge = ___cellBounds.x + ___cellBounds.width
						End If

						If leadingCellEdge <> leadingVisibleEdge Then
							If direction < 0 Then
								' Show remainder of leading cell
								Return Math.Abs(leadingVisibleEdge - leadingCellEdge)

							ElseIf leftToRight Then
								' Hide rest of leading cell
								Return leadingCellEdge + ___cellBounds.width - leadingVisibleEdge
							Else
								' Hide rest of leading cell
								Return leadingVisibleEdge - ___cellBounds.x
							End If
						End If
						' ASSUME: All cells are the same width
						Return ___cellBounds.width
					End If
				End If
			End If
			Dim f As Font = font
			Return If(f IsNot Nothing, f.size, 1)
		End Function


		''' <summary>
		''' Returns the distance to scroll to expose the next or previous block.
		''' <p>
		''' For vertical scrolling, the following rules are used:
		''' <ul>
		''' <li>if scrolling down, returns the distance to scroll so that the last
		''' visible element becomes the first completely visible element
		''' <li>if scrolling up, returns the distance to scroll so that the first
		''' visible element becomes the last completely visible element
		''' <li>returns {@code visibleRect.height} if the list is empty
		''' </ul>
		''' <p>
		''' For horizontal scrolling, when the layout orientation is either
		''' {@code VERTICAL_WRAP} or {@code HORIZONTAL_WRAP}:
		''' <ul>
		''' <li>if scrolling right, returns the distance to scroll so that the
		''' last visible element becomes
		''' the first completely visible element
		''' <li>if scrolling left, returns the distance to scroll so that the first
		''' visible element becomes the last completely visible element
		''' <li>returns {@code visibleRect.width} if the list is empty
		''' </ul>
		''' <p>
		''' For horizontal scrolling and {@code VERTICAL} orientation,
		''' returns {@code visibleRect.width}.
		''' <p>
		''' Note that the value of {@code visibleRect} must be the equal to
		''' {@code this.getVisibleRect()}.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> {@code SwingConstants.HORIZONTAL} or
		'''                    {@code SwingConstants.VERTICAL} </param>
		''' <param name="direction"> less or equal to zero to scroll up/back,
		'''                  greater than zero for down/forward </param>
		''' <returns> the "block" increment for scrolling in the specified direction;
		'''         always positive </returns>
		''' <seealso cref= #getScrollableUnitIncrement </seealso>
		''' <seealso cref= Scrollable#getScrollableBlockIncrement </seealso>
		''' <exception cref="IllegalArgumentException"> if {@code visibleRect} is {@code null}, or
		'''         {@code orientation} isn't one of {@code SwingConstants.VERTICAL} or
		'''         {@code SwingConstants.HORIZONTAL} </exception>
		Public Overridable Function getScrollableBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			checkScrollableParameters(visibleRect, orientation)
			If orientation = SwingConstants.VERTICAL Then
				Dim inc As Integer = visibleRect.height
				' Scroll Down 
				If direction > 0 Then
					' last cell is the lowest left cell
					Dim last As Integer = locationToIndex(New Point(visibleRect.x, visibleRect.y+visibleRect.height-1))
					If last <> -1 Then
						Dim lastRect As Rectangle = getCellBounds(last,last)
						If lastRect IsNot Nothing Then
							inc = lastRect.y - visibleRect.y
							If (inc = 0) AndAlso (last < model.size-1) Then inc = lastRect.height
						End If
					End If
				' Scroll Up 
				Else
					Dim newFirst As Integer = locationToIndex(New Point(visibleRect.x, visibleRect.y-visibleRect.height))
					Dim first As Integer = firstVisibleIndex
					If newFirst <> -1 Then
						If first = -1 Then first = locationToIndex(visibleRect.location)
						Dim newFirstRect As Rectangle = getCellBounds(newFirst,newFirst)
						Dim firstRect As Rectangle = getCellBounds(first,first)
						If (newFirstRect IsNot Nothing) AndAlso (firstRect IsNot Nothing) Then
							Do While (newFirstRect.y + visibleRect.height < firstRect.y + firstRect.height) AndAlso (newFirstRect.y < firstRect.y)
								newFirst += 1
								newFirstRect = getCellBounds(newFirst,newFirst)
							Loop
							inc = visibleRect.y - newFirstRect.y
							If (inc <= 0) AndAlso (newFirstRect.y > 0) Then
								newFirst -= 1
								newFirstRect = getCellBounds(newFirst,newFirst)
								If newFirstRect IsNot Nothing Then inc = visibleRect.y - newFirstRect.y
							End If
						End If
					End If
				End If
				Return inc
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso layoutOrientation <> JList.VERTICAL Then
				Dim leftToRight As Boolean = componentOrientation.leftToRight
				Dim inc As Integer = visibleRect.width
				' Scroll Right (in ltr mode) or Scroll Left (in rtl mode) 
				If direction > 0 Then
					' position is upper right if ltr, or upper left otherwise
					Dim ___x As Integer = visibleRect.x + (If(leftToRight, (visibleRect.width - 1), 0))
					Dim last As Integer = locationToIndex(New Point(___x, visibleRect.y))

					If last <> -1 Then
						Dim lastRect As Rectangle = getCellBounds(last,last)
						If lastRect IsNot Nothing Then
							If leftToRight Then
								inc = lastRect.x - visibleRect.x
							Else
								inc = visibleRect.x + visibleRect.width - (lastRect.x + lastRect.width)
							End If
							If inc < 0 Then
								inc += lastRect.width
							ElseIf (inc = 0) AndAlso (last < model.size-1) Then
								inc = lastRect.width
							End If
						End If
					End If
				' Scroll Left (in ltr mode) or Scroll Right (in rtl mode) 
				Else
					' position is upper left corner of the visibleRect shifted
					' left by the visibleRect.width if ltr, or upper right shifted
					' right by the visibleRect.width otherwise
					Dim ___x As Integer = visibleRect.x + (If(leftToRight, -visibleRect.width, visibleRect.width - 1 + visibleRect.width))
					Dim first As Integer = locationToIndex(New Point(___x, visibleRect.y))

					If first <> -1 Then
						Dim firstRect As Rectangle = getCellBounds(first,first)
						If firstRect IsNot Nothing Then
							' the right of the first cell
							Dim firstRight As Integer = firstRect.x + firstRect.width

							If leftToRight Then
								If (firstRect.x < visibleRect.x - visibleRect.width) AndAlso (firstRight < visibleRect.x) Then
									inc = visibleRect.x - firstRight
								Else
									inc = visibleRect.x - firstRect.x
								End If
							Else
								Dim visibleRight As Integer = visibleRect.x + visibleRect.width

								If (firstRight > visibleRight + visibleRect.width) AndAlso (firstRect.x > visibleRight) Then
									inc = firstRect.x - visibleRight
								Else
									inc = firstRight - visibleRight
								End If
							End If
						End If
					End If
				End If
				Return inc
			End If
			Return visibleRect.width
		End Function


		''' <summary>
		''' Returns {@code true} if this {@code JList} is displayed in a
		''' {@code JViewport} and the viewport is wider than the list's
		''' preferred width, or if the layout orientation is {@code HORIZONTAL_WRAP}
		''' and {@code visibleRowCount <= 0}; otherwise returns {@code false}.
		''' <p>
		''' If {@code false}, then don't track the viewport's width. This allows
		''' horizontal scrolling if the {@code JViewport} is itself embedded in a
		''' {@code JScrollPane}.
		''' </summary>
		''' <returns> whether or not an enclosing viewport should force the list's
		'''         width to match its own </returns>
		''' <seealso cref= Scrollable#getScrollableTracksViewportWidth </seealso>
		Public Overridable Property scrollableTracksViewportWidth As Boolean Implements Scrollable.getScrollableTracksViewportWidth
			Get
				If layoutOrientation = HORIZONTAL_WRAP AndAlso visibleRowCount <= 0 Then Return True
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then Return parent.width > preferredSize.width
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this {@code JList} is displayed in a
		''' {@code JViewport} and the viewport is taller than the list's
		''' preferred height, or if the layout orientation is {@code VERTICAL_WRAP}
		''' and {@code visibleRowCount <= 0}; otherwise returns {@code false}.
		''' <p>
		''' If {@code false}, then don't track the viewport's height. This allows
		''' vertical scrolling if the {@code JViewport} is itself embedded in a
		''' {@code JScrollPane}.
		''' </summary>
		''' <returns> whether or not an enclosing viewport should force the list's
		'''         height to match its own </returns>
		''' <seealso cref= Scrollable#getScrollableTracksViewportHeight </seealso>
		Public Overridable Property scrollableTracksViewportHeight As Boolean Implements Scrollable.getScrollableTracksViewportHeight
			Get
				If layoutOrientation = VERTICAL_WRAP AndAlso visibleRowCount <= 0 Then Return True
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then Return parent.height > preferredSize.height
				Return False
			End Get
		End Property


	'    
	'     * See {@code readObject} and {@code writeObject} in {@code JComponent}
	'     * for more information about serialization in Swing.
	'     
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub


		''' <summary>
		''' Returns a {@code String} representation of this {@code JList}.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned {@code String} may vary
		''' between implementations. The returned {@code String} may be empty,
		''' but may not be {@code null}.
		''' </summary>
		''' <returns>  a {@code String} representation of this {@code JList}. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim selectionForegroundString As String = (If(selectionForeground IsNot Nothing, selectionForeground.ToString(), ""))
			Dim selectionBackgroundString As String = (If(selectionBackground IsNot Nothing, selectionBackground.ToString(), ""))

			Return MyBase.paramString() & ",fixedCellHeight=" & fixedCellHeight & ",fixedCellWidth=" & fixedCellWidth & ",horizontalScrollIncrement=" & horizontalScrollIncrement & ",selectionBackground=" & selectionBackgroundString & ",selectionForeground=" & selectionForegroundString & ",visibleRowCount=" & visibleRowCount & ",layoutOrientation=" & layoutOrientation
		End Function


		''' <summary>
		''' --- Accessibility Support ---
		''' </summary>

		''' <summary>
		''' Gets the {@code AccessibleContext} associated with this {@code JList}.
		''' For {@code JList}, the {@code AccessibleContext} takes the form of an
		''' {@code AccessibleJList}.
		''' <p>
		''' A new {@code AccessibleJList} instance is created if necessary.
		''' </summary>
		''' <returns> an {@code AccessibleJList} that serves as the
		'''         {@code AccessibleContext} of this {@code JList} </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJList(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' {@code JList} class. It provides an implementation of the
		''' Java Accessibility API appropriate to list user-interface
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
		Protected Friend Class AccessibleJList
			Inherits AccessibleJComponent
			Implements AccessibleSelection, java.beans.PropertyChangeListener, ListSelectionListener, ListDataListener

			Private ReadOnly outerInstance As JList


			Friend leadSelectionIndex As Integer

			Public Sub New(ByVal outerInstance As JList)
					Me.outerInstance = outerInstance
				MyBase.New()
				outerInstance.addPropertyChangeListener(Me)
				outerInstance.selectionModel.addListSelectionListener(Me)
				outerInstance.model.addListDataListener(Me)
				leadSelectionIndex = outerInstance.leadSelectionIndex
			End Sub

			''' <summary>
			''' Property Change Listener change method. Used to track changes
			''' to the DataModel and ListSelectionModel, in order to re-set
			''' listeners to those for reporting changes there via the Accessibility
			''' PropertyChange mechanism.
			''' </summary>
			''' <param name="e"> PropertyChangeEvent </param>
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim name As String = e.propertyName
				Dim oldValue As Object = e.oldValue
				Dim newValue As Object = e.newValue

					' re-set listData listeners
				If name.CompareTo("model") = 0 Then

					If oldValue IsNot Nothing AndAlso TypeOf oldValue Is ListModel Then CType(oldValue, ListModel).removeListDataListener(Me)
					If newValue IsNot Nothing AndAlso TypeOf newValue Is ListModel Then CType(newValue, ListModel).addListDataListener(Me)

					' re-set listSelectionModel listeners
				ElseIf name.CompareTo("selectionModel") = 0 Then

					If oldValue IsNot Nothing AndAlso TypeOf oldValue Is ListSelectionModel Then CType(oldValue, ListSelectionModel).removeListSelectionListener(Me)
					If newValue IsNot Nothing AndAlso TypeOf newValue Is ListSelectionModel Then CType(newValue, ListSelectionModel).addListSelectionListener(Me)

					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
				End If
			End Sub

			''' <summary>
			''' List Selection Listener value change method. Used to fire
			''' the property change
			''' </summary>
			''' <param name="e"> ListSelectionEvent
			'''  </param>
			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				Dim oldLeadSelectionIndex As Integer = leadSelectionIndex
				leadSelectionIndex = outerInstance.leadSelectionIndex
				If oldLeadSelectionIndex <> leadSelectionIndex Then
					Dim oldLS, newLS As Accessible
					oldLS = If(oldLeadSelectionIndex >= 0, getAccessibleChild(oldLeadSelectionIndex), Nothing)
					newLS = If(leadSelectionIndex >= 0, getAccessibleChild(leadSelectionIndex), Nothing)
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY, oldLS, newLS)
				End If

				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))

				' Process the State changes for Multiselectable
				Dim s As AccessibleStateSet = accessibleStateSet
				Dim lsm As ListSelectionModel = outerInstance.selectionModel
				If lsm.selectionMode <> ListSelectionModel.SINGLE_SELECTION Then
					If Not s.contains(AccessibleState.MULTISELECTABLE) Then
						s.add(AccessibleState.MULTISELECTABLE)
						outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.MULTISELECTABLE)
					End If
				Else
					If s.contains(AccessibleState.MULTISELECTABLE) Then
						s.remove(AccessibleState.MULTISELECTABLE)
						outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.MULTISELECTABLE, Nothing)
					End If
				End If
			End Sub

			''' <summary>
			''' List Data Listener interval added method. Used to fire the visible data property change
			''' </summary>
			''' <param name="e"> ListDataEvent
			'''  </param>
			Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			End Sub

			''' <summary>
			''' List Data Listener interval removed method. Used to fire the visible data property change
			''' </summary>
			''' <param name="e"> ListDataEvent
			'''  </param>
			Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			End Sub

			''' <summary>
			''' List Data Listener contents changed method. Used to fire the visible data property change
			''' </summary>
			''' <param name="e"> ListDataEvent
			'''  </param>
			 Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
				 outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			 End Sub

		' AccessibleContext methods

			''' <summary>
			''' Get the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.selectionModel.selectionMode <> ListSelectionModel.SINGLE_SELECTION Then states.add(AccessibleState.MULTISELECTABLE)
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
			''' Returns the <code>Accessible</code> child contained at
			''' the local coordinate <code>Point</code>, if one exists.
			''' Otherwise returns <code>null</code>.
			''' </summary>
			''' <returns> the <code>Accessible</code> at the specified
			'''    location, if it exists </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Dim i As Integer = outerInstance.locationToIndex(p)
				If i >= 0 Then
					Return New AccessibleJListChild(Me, JList.this, i)
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
					Return outerInstance.model.size
				End Get
			End Property

			''' <summary>
			''' Return the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				If i >= outerInstance.model.size Then
					Return Nothing
				Else
					Return New AccessibleJListChild(Me, JList.this, i)
				End If
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
			 Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
				 Get
					 Return outerInstance.selectedIndices.length
				 End Get
			 End Property

			''' <summary>
			''' Returns an Accessible representing the specified selected item
			''' in the object.  If there isn't a selection, or there are
			''' fewer items selected than the integer passed in, the return
			''' value will be <code>null</code>.
			''' </summary>
			''' <param name="i"> the zero-based index of selected items </param>
			''' <returns> an Accessible containing the selected item </returns>
			 Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
				 Dim len As Integer = accessibleSelectionCount
				 If i < 0 OrElse i >= len Then
					 Return Nothing
				 Else
					 Return getAccessibleChild(outerInstance.selectedIndices(i))
				 End If
			 End Function

			''' <summary>
			''' Returns true if the current child of this object is selected.
			''' </summary>
			''' <param name="i"> the zero-based index of the child in this Accessible
			''' object. </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
				Return outerInstance.isSelectedIndex(i)
			End Function

			''' <summary>
			''' Adds the specified selected item in the object to the object's
			''' selection.  If the object supports multiple selections,
			''' the specified item is added to any existing selection, otherwise
			''' it replaces any existing selection in the object.  If the
			''' specified item is already selected, this method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			 Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
				 outerInstance.addSelectionInterval(i, i)
			 End Sub

			''' <summary>
			''' Removes the specified selected item in the object from the object's
			''' selection.  If the specified item isn't currently selected, this
			''' method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			 Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
				 outerInstance.removeSelectionInterval(i, i)
			 End Sub

			''' <summary>
			''' Clears the selection in the object, so that nothing in the
			''' object is selected.
			''' </summary>
			 Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
				 outerInstance.clearSelection()
			 End Sub

			''' <summary>
			''' Causes every selected item in the object to be selected
			''' if the object supports multiple selections.
			''' </summary>
			 Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
				 outerInstance.addSelectionInterval(0, accessibleChildrenCount -1)
			 End Sub

			  ''' <summary>
			  ''' This class implements accessibility support appropriate
			  ''' for list children.
			  ''' </summary>
			Protected Friend Class AccessibleJListChild
				Inherits AccessibleContext
				Implements Accessible, AccessibleComponent

				Private ReadOnly outerInstance As JList.AccessibleJList

				Private parent As JList(Of E) = Nothing
				Private indexInParent As Integer
				Private component As Component = Nothing
				Private ___accessibleContext As AccessibleContext = Nothing
				Private listModel As ListModel(Of E)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Private cellRenderer As ListCellRenderer(Of ?) = Nothing

				Public Sub New(ByVal outerInstance As JList.AccessibleJList, ByVal parent As JList(Of E), ByVal indexInParent As Integer)
						Me.outerInstance = outerInstance
					Me.parent = parent
					Me.accessibleParent = parent
					Me.indexInParent = indexInParent
					If parent IsNot Nothing Then
						listModel = parent.model
						cellRenderer = parent.cellRenderer
					End If
				End Sub

				Private Property currentComponent As Component
					Get
						Return getComponentAtIndex(indexInParent)
					End Get
				End Property

				Private Property currentAccessibleContext As AccessibleContext
					Get
						Dim c As Component = getComponentAtIndex(indexInParent)
						If TypeOf c Is Accessible Then
							Return c.accessibleContext
						Else
							Return Nothing
						End If
					End Get
				End Property

				Private Function getComponentAtIndex(ByVal index As Integer) As Component
					If index < 0 OrElse index >= listModel.size Then Return Nothing
					If (parent IsNot Nothing) AndAlso (listModel IsNot Nothing) AndAlso cellRenderer IsNot Nothing Then
						Dim value As E = listModel.getElementAt(index)
						Dim isSelected As Boolean = parent.isSelectedIndex(index)
						Dim isFocussed As Boolean = parent.focusOwner AndAlso (index = parent.leadSelectionIndex)
						Return cellRenderer.getListCellRendererComponent(parent, value, index, isSelected, isFocussed)
					Else
						Return Nothing
					End If
				End Function


				' Accessible Methods
			   ''' <summary>
			   ''' Get the AccessibleContext for this object. In the
			   ''' implementation of the Java Accessibility API for this class,
			   ''' returns this object, which is its own AccessibleContext.
			   ''' </summary>
			   ''' <returns> this object </returns>
				Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
					Get
						Return Me
					End Get
				End Property


				' AccessibleContext methods

				Public Property Overrides accessibleName As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleName
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then ac.accessibleName = s
					End Set
				End Property


				Public Property Overrides accessibleDescription As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleDescription
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then ac.accessibleDescription = s
					End Set
				End Property


				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleRole
						Else
							Return Nothing
						End If
					End Get
				End Property

				Public Property Overrides accessibleStateSet As AccessibleStateSet
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						Dim s As AccessibleStateSet
						If ac IsNot Nothing Then
							s = ac.accessibleStateSet
						Else
							s = New AccessibleStateSet
						End If
    
						s.add(AccessibleState.SELECTABLE)
						If parent.focusOwner AndAlso (indexInParent = parent.leadSelectionIndex) Then s.add(AccessibleState.ACTIVE)
						If parent.isSelectedIndex(indexInParent) Then s.add(AccessibleState.SELECTED)
						If Me.showing Then
							s.add(AccessibleState.SHOWING)
						ElseIf s.contains(AccessibleState.SHOWING) Then
							s.remove(AccessibleState.SHOWING)
						End If
						If Me.visible Then
							s.add(AccessibleState.VISIBLE)
						ElseIf s.contains(AccessibleState.VISIBLE) Then
							s.remove(AccessibleState.VISIBLE)
						End If
						s.add(AccessibleState.TRANSIENT) ' cell-rendered
						Return s
					End Get
				End Property

				Public Property Overrides accessibleIndexInParent As Integer
					Get
						Return indexInParent
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

				Public Property Overrides locale As java.util.Locale
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
					If ac IsNot Nothing Then ac.addPropertyChangeListener(l)
				End Sub

				Public Overrides Sub removePropertyChangeListener(ByVal l As java.beans.PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then ac.removePropertyChangeListener(l)
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
						Dim fi As Integer = parent.firstVisibleIndex
						Dim li As Integer = parent.lastVisibleIndex
						' The UI incorrectly returns a -1 for the last
						' visible index if the list is smaller than the
						' viewport size.
						If li = -1 Then li = parent.model.size - 1
						Return ((indexInParent >= fi) AndAlso (indexInParent <= li))
					End Get
					Set(ByVal b As Boolean)
					End Set
				End Property


				Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
					Get
						Return (parent.showing AndAlso visible)
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
							Dim listLocation As Point = parent.locationOnScreen
							Dim componentLocation As Point = parent.indexToLocation(indexInParent)
							If componentLocation IsNot Nothing Then
								componentLocation.translate(listLocation.x, listLocation.y)
								Return componentLocation
							Else
								Return Nothing
							End If
						Else
							Return Nothing
						End If
					End Get
				End Property

				Public Overridable Property location As Point Implements AccessibleComponent.getLocation
					Get
						If parent IsNot Nothing Then
							Return parent.indexToLocation(indexInParent)
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal p As Point)
						If (parent IsNot Nothing) AndAlso (parent.contains(p)) Then ensureIndexIsVisible(indexInParent)
					End Set
				End Property


				Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
					Get
						If parent IsNot Nothing Then
							Return parent.getCellBounds(indexInParent,indexInParent)
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal r As Rectangle)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then CType(ac, AccessibleComponent).bounds = r
					End Set
				End Property


				Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
					Get
						Dim cellBounds As Rectangle = Me.bounds
						If cellBounds IsNot Nothing Then
							Return cellBounds.size
						Else
							Return Nothing
						End If
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

				' TIGER - 4733624
				''' <summary>
				''' Returns the icon for the element renderer, as the only item
				''' of an array of <code>AccessibleIcon</code>s or a <code>null</code> array
				''' if the renderer component contains no icons.
				''' </summary>
				''' <returns> an array containing the accessible icon
				'''         or a <code>null</code> array if none
				''' @since 1.3 </returns>
				Public Property Overrides accessibleIcon As AccessibleIcon()
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleIcon
						Else
							Return Nothing
						End If
					End Get
				End Property
			End Class ' inner class AccessibleJListChild
		End Class ' inner class AccessibleJList
	End Class

End Namespace
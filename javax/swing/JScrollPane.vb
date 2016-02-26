Imports System
Imports javax.swing.plaf
Imports javax.swing.border
Imports javax.swing.event
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

Namespace javax.swing




	''' <summary>
	''' Provides a scrollable view of a lightweight component.
	''' A <code>JScrollPane</code> manages a viewport, optional
	''' vertical and horizontal scroll bars, and optional row and
	''' column heading viewports.
	''' You can find task-oriented documentation of <code>JScrollPane</code> in
	'''  <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/scrollpane.html">How to Use Scroll Panes</a>,
	''' a section in <em>The Java Tutorial</em>.  Note that
	''' <code>JScrollPane</code> does not support heavyweight components.
	''' 
	''' <TABLE STYLE="FLOAT:RIGHT" BORDER="0" SUMMARY="layout">
	'''    <TR>
	'''    <TD ALIGN="CENTER">
	'''      <P STYLE="TEXT-ALIGN:CENTER"><IMG SRC="doc-files/JScrollPane-1.gif"
	'''      alt="The following text describes this image."
	'''      WIDTH="256" HEIGHT="248" STYLE="FLOAT:BOTTOM; BORDER:0px">
	'''    </TD>
	'''    </TR>
	''' </TABLE>
	''' The <code>JViewport</code> provides a window,
	''' or &quot;viewport&quot; onto a data
	''' source -- for example, a text file. That data source is the
	''' &quot;scrollable client&quot; (aka data model) displayed by the
	''' <code>JViewport</code> view.
	''' A <code>JScrollPane</code> basically consists of <code>JScrollBar</code>s,
	''' a <code>JViewport</code>, and the wiring between them,
	''' as shown in the diagram at right.
	''' <p>
	''' In addition to the scroll bars and viewport,
	''' a <code>JScrollPane</code> can have a
	''' column header and a row header. Each of these is a
	''' <code>JViewport</code> object that
	''' you specify with <code>setRowHeaderView</code>,
	''' and <code>setColumnHeaderView</code>.
	''' The column header viewport automatically scrolls left and right, tracking
	''' the left-right scrolling of the main viewport.
	''' (It never scrolls vertically, however.)
	''' The row header acts in a similar fashion.
	''' <p>
	''' Where two scroll bars meet, the row header meets the column header,
	''' or a scroll bar meets one of the headers, both components stop short
	''' of the corner, leaving a rectangular space which is, by default, empty.
	''' These spaces can potentially exist in any number of the four corners.
	''' In the previous diagram, the top right space is present and identified
	''' by the label "corner component".
	''' <p>
	''' Any number of these empty spaces can be replaced by using the
	''' <code>setCorner</code> method to add a component to a particular corner.
	''' (Note: The same component cannot be added to multiple corners.)
	''' This is useful if there's
	''' some extra decoration or function you'd like to add to the scroll pane.
	''' The size of each corner component is entirely determined by the size of the
	''' headers and/or scroll bars that surround it.
	''' <p>
	''' A corner component will only be visible if there is an empty space in that
	''' corner for it to exist in. For example, consider a component set into the
	''' top right corner of a scroll pane with a column header. If the scroll pane's
	''' vertical scrollbar is not present, perhaps because the view component hasn't
	''' grown large enough to require it, then the corner component will not be
	''' shown (since there is no empty space in that corner created by the meeting
	''' of the header and vertical scroll bar). Forcing the scroll bar to always be
	''' shown, using
	''' <code>setVerticalScrollBarPolicy(VERTICAL_SCROLLBAR_ALWAYS)</code>,
	''' will ensure that the space for the corner component always exists.
	''' <p>
	''' To add a border around the main viewport,
	''' you can use <code>setViewportBorder</code>.
	''' (Of course, you can also add a border around the whole scroll pane using
	''' <code>setBorder</code>.)
	''' <p>
	''' A common operation to want to do is to set the background color that will
	''' be used if the main viewport view is smaller than the viewport, or is
	''' not opaque. This can be accomplished by setting the background color
	''' of the viewport, via <code>scrollPane.getViewport().setBackground()</code>.
	''' The reason for setting the color of the viewport and not the scrollpane
	''' is that by default <code>JViewport</code> is opaque
	''' which, among other things, means it will completely fill
	''' in its background using its background color.  Therefore when
	''' <code>JScrollPane</code> draws its background the viewport will
	''' usually draw over it.
	''' <p>
	''' By default <code>JScrollPane</code> uses <code>ScrollPaneLayout</code>
	''' to handle the layout of its child Components. <code>ScrollPaneLayout</code>
	''' determines the size to make the viewport view in one of two ways:
	''' <ol>
	'''   <li>If the view implements <code>Scrollable</code>
	'''       a combination of <code>getPreferredScrollableViewportSize</code>,
	'''       <code>getScrollableTracksViewportWidth</code> and
	'''       <code>getScrollableTracksViewportHeight</code>is used, otherwise
	'''   <li><code>getPreferredSize</code> is used.
	''' </ol>
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
	''' </summary>
	''' <seealso cref= JScrollBar </seealso>
	''' <seealso cref= JViewport </seealso>
	''' <seealso cref= ScrollPaneLayout </seealso>
	''' <seealso cref= Scrollable </seealso>
	''' <seealso cref= Component#getPreferredSize </seealso>
	''' <seealso cref= #setViewportView </seealso>
	''' <seealso cref= #setRowHeaderView </seealso>
	''' <seealso cref= #setColumnHeaderView </seealso>
	''' <seealso cref= #setCorner </seealso>
	''' <seealso cref= #setViewportBorder
	''' 
	''' @beaninfo
	'''     attribute: isContainer true
	'''     attribute: containerDelegate getViewport
	'''   description: A specialized container that manages a viewport, optional scrollbars and headers
	''' 
	''' @author Hans Muller </seealso>
	Public Class JScrollPane
		Inherits JComponent
		Implements ScrollPaneConstants, Accessible

		Private viewportBorder As Border

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ScrollPaneUI"

		''' <summary>
		''' The display policy for the vertical scrollbar.
		''' The default is
		''' <code>ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED</code>. </summary>
		''' <seealso cref= #setVerticalScrollBarPolicy </seealso>
		Protected Friend verticalScrollBarPolicy As Integer = VERTICAL_SCROLLBAR_AS_NEEDED


		''' <summary>
		''' The display policy for the horizontal scrollbar.
		''' The default is
		''' <code>ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED</code>. </summary>
		''' <seealso cref= #setHorizontalScrollBarPolicy </seealso>
		Protected Friend horizontalScrollBarPolicy As Integer = HORIZONTAL_SCROLLBAR_AS_NEEDED


		''' <summary>
		''' The scrollpane's viewport child.  Default is an empty
		''' <code>JViewport</code>. </summary>
		''' <seealso cref= #setViewport </seealso>
		Protected Friend viewport As JViewport


		''' <summary>
		''' The scrollpane's vertical scrollbar child.
		''' Default is a <code>JScrollBar</code>. </summary>
		''' <seealso cref= #setVerticalScrollBar </seealso>
		Protected Friend verticalScrollBar As JScrollBar


		''' <summary>
		''' The scrollpane's horizontal scrollbar child.
		''' Default is a <code>JScrollBar</code>. </summary>
		''' <seealso cref= #setHorizontalScrollBar </seealso>
		Protected Friend horizontalScrollBar As JScrollBar


		''' <summary>
		''' The row header child.  Default is <code>null</code>. </summary>
		''' <seealso cref= #setRowHeader </seealso>
		Protected Friend rowHeader As JViewport


		''' <summary>
		''' The column header child.  Default is <code>null</code>. </summary>
		''' <seealso cref= #setColumnHeader </seealso>
		Protected Friend columnHeader As JViewport


		''' <summary>
		''' The component to display in the lower left corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= #setCorner </seealso>
		Protected Friend lowerLeft As java.awt.Component


		''' <summary>
		''' The component to display in the lower right corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= #setCorner </seealso>
		Protected Friend lowerRight As java.awt.Component


		''' <summary>
		''' The component to display in the upper left corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= #setCorner </seealso>
		Protected Friend upperLeft As java.awt.Component


		''' <summary>
		''' The component to display in the upper right corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= #setCorner </seealso>
		Protected Friend upperRight As java.awt.Component

	'    
	'     * State flag for mouse wheel scrolling
	'     
		Private wheelScrollState As Boolean = True

		''' <summary>
		''' Creates a <code>JScrollPane</code> that displays the view
		''' component in a viewport
		''' whose view position can be controlled with a pair of scrollbars.
		''' The scrollbar policies specify when the scrollbars are displayed,
		''' For example, if <code>vsbPolicy</code> is
		''' <code>VERTICAL_SCROLLBAR_AS_NEEDED</code>
		''' then the vertical scrollbar only appears if the view doesn't fit
		''' vertically. The available policy settings are listed at
		''' <seealso cref="#setVerticalScrollBarPolicy"/> and
		''' <seealso cref="#setHorizontalScrollBarPolicy"/>.
		''' </summary>
		''' <seealso cref= #setViewportView
		''' </seealso>
		''' <param name="view"> the component to display in the scrollpanes viewport </param>
		''' <param name="vsbPolicy"> an integer that specifies the vertical
		'''          scrollbar policy </param>
		''' <param name="hsbPolicy"> an integer that specifies the horizontal
		'''          scrollbar policy </param>
		Public Sub New(ByVal view As java.awt.Component, ByVal vsbPolicy As Integer, ByVal hsbPolicy As Integer)
			layout = New ScrollPaneLayout.UIResource
			verticalScrollBarPolicy = vsbPolicy
			horizontalScrollBarPolicy = hsbPolicy
			viewport = createViewport()
			verticalScrollBar = createVerticalScrollBar()
			horizontalScrollBar = createHorizontalScrollBar()
			If view IsNot Nothing Then viewportView = view
			uIPropertyrty("opaque",True)
			updateUI()

			If Not Me.componentOrientation.leftToRight Then viewport.viewPosition = New java.awt.Point(Integer.MaxValue, 0)
		End Sub


		''' <summary>
		''' Creates a <code>JScrollPane</code> that displays the
		''' contents of the specified
		''' component, where both horizontal and vertical scrollbars appear
		''' whenever the component's contents are larger than the view.
		''' </summary>
		''' <seealso cref= #setViewportView </seealso>
		''' <param name="view"> the component to display in the scrollpane's viewport </param>
		Public Sub New(ByVal view As java.awt.Component)
			Me.New(view, VERTICAL_SCROLLBAR_AS_NEEDED, HORIZONTAL_SCROLLBAR_AS_NEEDED)
		End Sub


		''' <summary>
		''' Creates an empty (no viewport view) <code>JScrollPane</code>
		''' with specified
		''' scrollbar policies. The available policy settings are listed at
		''' <seealso cref="#setVerticalScrollBarPolicy"/> and
		''' <seealso cref="#setHorizontalScrollBarPolicy"/>.
		''' </summary>
		''' <seealso cref= #setViewportView
		''' </seealso>
		''' <param name="vsbPolicy"> an integer that specifies the vertical
		'''          scrollbar policy </param>
		''' <param name="hsbPolicy"> an integer that specifies the horizontal
		'''          scrollbar policy </param>
		Public Sub New(ByVal vsbPolicy As Integer, ByVal hsbPolicy As Integer)
			Me.New(Nothing, vsbPolicy, hsbPolicy)
		End Sub


		''' <summary>
		''' Creates an empty (no viewport view) <code>JScrollPane</code>
		''' where both horizontal and vertical scrollbars appear when needed.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, VERTICAL_SCROLLBAR_AS_NEEDED, HORIZONTAL_SCROLLBAR_AS_NEEDED)
		End Sub


		''' <summary>
		''' Returns the look and feel (L&amp;F) object that renders this component.
		''' </summary>
		''' <returns> the <code>ScrollPaneUI</code> object that renders this
		'''                          component </returns>
		''' <seealso cref= #setUI
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''    attribute: visualUpdate true
		'''  description: The UI object that implements the Component's LookAndFeel. </seealso>
		Public Overridable Property uI As ScrollPaneUI
			Get
				Return CType(ui, ScrollPaneUI)
			End Get
			Set(ByVal ui As ScrollPaneUI)
				MyBase.uI = ui
			End Set
		End Property




		''' <summary>
		''' Replaces the current <code>ScrollPaneUI</code> object with a version
		''' from the current default look and feel.
		''' To be called when the default look and feel changes.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		''' <seealso cref= UIManager#getUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ScrollPaneUI)
		End Sub


		''' <summary>
		''' Returns the suffix used to construct the name of the L&amp;F class used to
		''' render this component.
		''' </summary>
		''' <returns> the string "ScrollPaneUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' 
		''' @beaninfo
		'''    hidden: true </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property



		''' <summary>
		''' Sets the layout manager for this <code>JScrollPane</code>.
		''' This method overrides <code>setLayout</code> in
		''' <code>java.awt.Container</code> to ensure that only
		''' <code>LayoutManager</code>s which
		''' are subclasses of <code>ScrollPaneLayout</code> can be used in a
		''' <code>JScrollPane</code>. If <code>layout</code> is non-null, this
		''' will invoke <code>syncWithScrollPane</code> on it.
		''' </summary>
		''' <param name="layout"> the specified layout manager </param>
		''' <exception cref="ClassCastException"> if layout is not a
		'''                  <code>ScrollPaneLayout</code> </exception>
		''' <seealso cref= java.awt.Container#getLayout </seealso>
		''' <seealso cref= java.awt.Container#setLayout
		''' 
		''' @beaninfo
		'''    hidden: true </seealso>
		Public Overridable Property layout As java.awt.LayoutManager
			Set(ByVal layout As java.awt.LayoutManager)
				If TypeOf layout Is ScrollPaneLayout Then
					MyBase.layout = layout
					CType(layout, ScrollPaneLayout).syncWithScrollPane(Me)
				ElseIf layout Is Nothing Then
					MyBase.layout = layout
				Else
					Dim s As String = "layout of JScrollPane must be a ScrollPaneLayout"
					Throw New ClassCastException(s)
				End If
			End Set
		End Property

		''' <summary>
		''' Overridden to return true so that any calls to <code>revalidate</code>
		''' on any descendants of this <code>JScrollPane</code> will cause the
		''' entire tree beginning with this <code>JScrollPane</code> to be
		''' validated.
		''' </summary>
		''' <returns> true </returns>
		''' <seealso cref= java.awt.Container#validate </seealso>
		''' <seealso cref= JComponent#revalidate </seealso>
		''' <seealso cref= JComponent#isValidateRoot </seealso>
		''' <seealso cref= java.awt.Container#isValidateRoot
		''' 
		''' @beaninfo
		'''    hidden: true </seealso>
		Public Property Overrides validateRoot As Boolean
			Get
				Return True
			End Get
		End Property


		''' <summary>
		''' Returns the vertical scroll bar policy value. </summary>
		''' <returns> the <code>verticalScrollBarPolicy</code> property </returns>
		''' <seealso cref= #setVerticalScrollBarPolicy </seealso>
		Public Overridable Property verticalScrollBarPolicy As Integer
			Get
				Return verticalScrollBarPolicy
			End Get
			Set(ByVal policy As Integer)
				Select Case policy
				Case VERTICAL_SCROLLBAR_AS_NEEDED, VERTICAL_SCROLLBAR_NEVER, VERTICAL_SCROLLBAR_ALWAYS
				Case Else
					Throw New System.ArgumentException("invalid verticalScrollBarPolicy")
				End Select
				Dim old As Integer = verticalScrollBarPolicy
				verticalScrollBarPolicy = policy
				firePropertyChange("verticalScrollBarPolicy", old, policy)
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Returns the horizontal scroll bar policy value. </summary>
		''' <returns> the <code>horizontalScrollBarPolicy</code> property </returns>
		''' <seealso cref= #setHorizontalScrollBarPolicy </seealso>
		Public Overridable Property horizontalScrollBarPolicy As Integer
			Get
				Return horizontalScrollBarPolicy
			End Get
			Set(ByVal policy As Integer)
				Select Case policy
				Case HORIZONTAL_SCROLLBAR_AS_NEEDED, HORIZONTAL_SCROLLBAR_NEVER, HORIZONTAL_SCROLLBAR_ALWAYS
				Case Else
					Throw New System.ArgumentException("invalid horizontalScrollBarPolicy")
				End Select
				Dim old As Integer = horizontalScrollBarPolicy
				horizontalScrollBarPolicy = policy
				firePropertyChange("horizontalScrollBarPolicy", old, policy)
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Returns the <code>Border</code> object that surrounds the viewport.
		''' </summary>
		''' <returns> the <code>viewportBorder</code> property </returns>
		''' <seealso cref= #setViewportBorder </seealso>
		Public Overridable Property viewportBorder As Border
			Get
				Return viewportBorder
			End Get
			Set(ByVal viewportBorder As Border)
				Dim oldValue As Border = Me.viewportBorder
				Me.viewportBorder = viewportBorder
				firePropertyChange("viewportBorder", oldValue, viewportBorder)
			End Set
		End Property




		''' <summary>
		''' Returns the bounds of the viewport's border.
		''' </summary>
		''' <returns> a <code>Rectangle</code> object specifying the viewport border </returns>
		Public Overridable Property viewportBorderBounds As java.awt.Rectangle
			Get
				Dim borderR As New java.awt.Rectangle(size)
    
				Dim ___insets As java.awt.Insets = insets
				borderR.x = ___insets.left
				borderR.y = ___insets.top
				borderR.width -= ___insets.left + ___insets.right
				borderR.height -= ___insets.top + ___insets.bottom
    
				Dim leftToRight As Boolean = SwingUtilities.isLeftToRight(Me)
    
		'         If there's a visible column header remove the space it
		'         * needs from the top of borderR.
		'         
    
				Dim colHead As JViewport = columnHeader
				If (colHead IsNot Nothing) AndAlso (colHead.visible) Then
					Dim colHeadHeight As Integer = colHead.height
					borderR.y += colHeadHeight
					borderR.height -= colHeadHeight
				End If
    
		'         If there's a visible row header remove the space it needs
		'         * from the left of borderR.
		'         
    
				Dim rowHead As JViewport = rowHeader
				If (rowHead IsNot Nothing) AndAlso (rowHead.visible) Then
					Dim rowHeadWidth As Integer = rowHead.width
					If leftToRight Then borderR.x += rowHeadWidth
					borderR.width -= rowHeadWidth
				End If
    
		'         If there's a visible vertical scrollbar remove the space it needs
		'         * from the width of borderR.
		'         
				Dim vsb As JScrollBar = verticalScrollBar
				If (vsb IsNot Nothing) AndAlso (vsb.visible) Then
					Dim vsbWidth As Integer = vsb.width
					If Not leftToRight Then borderR.x += vsbWidth
					borderR.width -= vsbWidth
				End If
    
		'         If there's a visible horizontal scrollbar remove the space it needs
		'         * from the height of borderR.
		'         
				Dim hsb As JScrollBar = horizontalScrollBar
				If (hsb IsNot Nothing) AndAlso (hsb.visible) Then borderR.height -= hsb.height
    
				Return borderR
			End Get
		End Property


		''' <summary>
		''' By default <code>JScrollPane</code> creates scrollbars
		''' that are instances
		''' of this class.  <code>Scrollbar</code> overrides the
		''' <code>getUnitIncrement</code> and <code>getBlockIncrement</code>
		''' methods so that, if the viewport's view is a <code>Scrollable</code>,
		''' the view is asked to compute these values. Unless
		''' the unit/block increment have been explicitly set.
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
		''' <seealso cref= Scrollable </seealso>
		''' <seealso cref= JScrollPane#createVerticalScrollBar </seealso>
		''' <seealso cref= JScrollPane#createHorizontalScrollBar </seealso>
		Protected Friend Class ScrollBar
			Inherits JScrollBar
			Implements UIResource

			Private ReadOnly outerInstance As JScrollPane

			''' <summary>
			''' Set to true when the unit increment has been explicitly set.
			''' If this is false the viewport's view is obtained and if it
			''' is an instance of <code>Scrollable</code> the unit increment
			''' from it is used.
			''' </summary>
			Private unitIncrementSet As Boolean
			''' <summary>
			''' Set to true when the block increment has been explicitly set.
			''' If this is false the viewport's view is obtained and if it
			''' is an instance of <code>Scrollable</code> the block increment
			''' from it is used.
			''' </summary>
			Private blockIncrementSet As Boolean

			''' <summary>
			''' Creates a scrollbar with the specified orientation.
			''' The options are:
			''' <ul>
			''' <li><code>ScrollPaneConstants.VERTICAL</code>
			''' <li><code>ScrollPaneConstants.HORIZONTAL</code>
			''' </ul>
			''' </summary>
			''' <param name="orientation">  an integer specifying one of the legal
			'''      orientation values shown above
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As JScrollPane, ByVal orientation As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(orientation)
				Me.putClientProperty("JScrollBar.fastWheelScrolling", Boolean.TRUE)
			End Sub

			''' <summary>
			''' Messages super to set the value, and resets the
			''' <code>unitIncrementSet</code> instance variable to true.
			''' </summary>
			''' <param name="unitIncrement"> the new unit increment value, in pixels </param>
			Public Overrides Property unitIncrement As Integer
				Set(ByVal unitIncrement As Integer)
					unitIncrementSet = True
					Me.putClientProperty("JScrollBar.fastWheelScrolling", Nothing)
					MyBase.unitIncrement = unitIncrement
				End Set
			End Property

			''' <summary>
			''' Computes the unit increment for scrolling if the viewport's
			''' view is a <code>Scrollable</code> object.
			''' Otherwise return <code>super.getUnitIncrement</code>.
			''' </summary>
			''' <param name="direction"> less than zero to scroll up/left,
			'''      greater than zero for down/right </param>
			''' <returns> an integer, in pixels, containing the unit increment </returns>
			''' <seealso cref= Scrollable#getScrollableUnitIncrement </seealso>
			Public Overrides Function getUnitIncrement(ByVal direction As Integer) As Integer
				Dim vp As JViewport = outerInstance.viewport
				If (Not unitIncrementSet) AndAlso (vp IsNot Nothing) AndAlso (TypeOf vp.view Is Scrollable) Then
					Dim view As Scrollable = CType(vp.view, Scrollable)
					Dim vr As java.awt.Rectangle = vp.viewRect
					Return view.getScrollableUnitIncrement(vr, orientation, direction)
				Else
					Return MyBase.getUnitIncrement(direction)
				End If
			End Function

			''' <summary>
			''' Messages super to set the value, and resets the
			''' <code>blockIncrementSet</code> instance variable to true.
			''' </summary>
			''' <param name="blockIncrement"> the new block increment value, in pixels </param>
			Public Overrides Property blockIncrement As Integer
				Set(ByVal blockIncrement As Integer)
					blockIncrementSet = True
					Me.putClientProperty("JScrollBar.fastWheelScrolling", Nothing)
					MyBase.blockIncrement = blockIncrement
				End Set
			End Property

			''' <summary>
			''' Computes the block increment for scrolling if the viewport's
			''' view is a <code>Scrollable</code> object.  Otherwise
			''' the <code>blockIncrement</code> equals the viewport's width
			''' or height.  If there's no viewport return
			''' <code>super.getBlockIncrement</code>.
			''' </summary>
			''' <param name="direction"> less than zero to scroll up/left,
			'''      greater than zero for down/right </param>
			''' <returns> an integer, in pixels, containing the block increment </returns>
			''' <seealso cref= Scrollable#getScrollableBlockIncrement </seealso>
			Public Overrides Function getBlockIncrement(ByVal direction As Integer) As Integer
				Dim vp As JViewport = outerInstance.viewport
				If blockIncrementSet OrElse vp Is Nothing Then
					Return MyBase.getBlockIncrement(direction)
				ElseIf TypeOf vp.view Is Scrollable Then
					Dim view As Scrollable = CType(vp.view, Scrollable)
					Dim vr As java.awt.Rectangle = vp.viewRect
					Return view.getScrollableBlockIncrement(vr, orientation, direction)
				ElseIf orientation = VERTICAL Then
					Return vp.extentSize.height
				Else
					Return vp.extentSize.width
				End If
			End Function

		End Class


		''' <summary>
		''' Returns a <code>JScrollPane.ScrollBar</code> by default.
		''' Subclasses may override this method to force <code>ScrollPaneUI</code>
		''' implementations to use a <code>JScrollBar</code> subclass.
		''' Used by <code>ScrollPaneUI</code> implementations to
		''' create the horizontal scrollbar.
		''' </summary>
		''' <returns> a <code>JScrollBar</code> with a horizontal orientation </returns>
		''' <seealso cref= JScrollBar </seealso>
		Public Overridable Function createHorizontalScrollBar() As JScrollBar
			Return New ScrollBar(Me, JScrollBar.HORIZONTAL)
		End Function


		''' <summary>
		''' Returns the horizontal scroll bar that controls the viewport's
		''' horizontal view position.
		''' </summary>
		''' <returns> the <code>horizontalScrollBar</code> property </returns>
		''' <seealso cref= #setHorizontalScrollBar </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property horizontalScrollBar As JScrollBar
			Get
				Return horizontalScrollBar
			End Get
			Set(ByVal horizontalScrollBar As JScrollBar)
				Dim old As JScrollBar = horizontalScrollBar
				Me.horizontalScrollBar = horizontalScrollBar
				If horizontalScrollBar IsNot Nothing Then
					add(horizontalScrollBar, HORIZONTAL_SCROLLBAR)
				ElseIf old IsNot Nothing Then
					remove(old)
				End If
				firePropertyChange("horizontalScrollBar", old, horizontalScrollBar)
    
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Returns a <code>JScrollPane.ScrollBar</code> by default.  Subclasses
		''' may override this method to force <code>ScrollPaneUI</code>
		''' implementations to use a <code>JScrollBar</code> subclass.
		''' Used by <code>ScrollPaneUI</code> implementations to create the
		''' vertical scrollbar.
		''' </summary>
		''' <returns> a <code>JScrollBar</code> with a vertical orientation </returns>
		''' <seealso cref= JScrollBar </seealso>
		Public Overridable Function createVerticalScrollBar() As JScrollBar
			Return New ScrollBar(Me, JScrollBar.VERTICAL)
		End Function


		''' <summary>
		''' Returns the vertical scroll bar that controls the viewports
		''' vertical view position.
		''' </summary>
		''' <returns> the <code>verticalScrollBar</code> property </returns>
		''' <seealso cref= #setVerticalScrollBar </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property verticalScrollBar As JScrollBar
			Get
				Return verticalScrollBar
			End Get
			Set(ByVal verticalScrollBar As JScrollBar)
				Dim old As JScrollBar = verticalScrollBar
				Me.verticalScrollBar = verticalScrollBar
				add(verticalScrollBar, VERTICAL_SCROLLBAR)
				firePropertyChange("verticalScrollBar", old, verticalScrollBar)
    
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Returns a new <code>JViewport</code> by default.
		''' Used to create the
		''' viewport (as needed) in <code>setViewportView</code>,
		''' <code>setRowHeaderView</code>, and <code>setColumnHeaderView</code>.
		''' Subclasses may override this method to return a subclass of
		''' <code>JViewport</code>.
		''' </summary>
		''' <returns> a new <code>JViewport</code> </returns>
		Protected Friend Overridable Function createViewport() As JViewport
			Return New JViewport
		End Function


		''' <summary>
		''' Returns the current <code>JViewport</code>.
		''' </summary>
		''' <seealso cref= #setViewport </seealso>
		''' <returns> the <code>viewport</code> property </returns>
		Public Overridable Property viewport As JViewport
			Get
				Return viewport
			End Get
			Set(ByVal viewport As JViewport)
				Dim old As JViewport = viewport
				Me.viewport = viewport
				If viewport IsNot Nothing Then
					add(viewport, VIEWPORT)
				ElseIf old IsNot Nothing Then
					remove(old)
				End If
				firePropertyChange("viewport", old, viewport)
    
				If accessibleContext IsNot Nothing Then CType(accessibleContext, AccessibleJScrollPane).resetViewPort()
    
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Creates a viewport if necessary and then sets its view.  Applications
		''' that don't provide the view directly to the <code>JScrollPane</code>
		''' constructor
		''' should use this method to specify the scrollable child that's going
		''' to be displayed in the scrollpane. For example:
		''' <pre>
		''' JScrollPane scrollpane = new JScrollPane();
		''' scrollpane.setViewportView(myBigComponentToScroll);
		''' </pre>
		''' Applications should not add children directly to the scrollpane.
		''' </summary>
		''' <param name="view"> the component to add to the viewport </param>
		''' <seealso cref= #setViewport </seealso>
		''' <seealso cref= JViewport#setView </seealso>
		Public Overridable Property viewportView As java.awt.Component
			Set(ByVal view As java.awt.Component)
				If viewport Is Nothing Then viewport = createViewport()
				viewport.view = view
			End Set
		End Property



		''' <summary>
		''' Returns the row header. </summary>
		''' <returns> the <code>rowHeader</code> property </returns>
		''' <seealso cref= #setRowHeader </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property rowHeader As JViewport
			Get
				Return rowHeader
			End Get
			Set(ByVal rowHeader As JViewport)
				Dim old As JViewport = rowHeader
				Me.rowHeader = rowHeader
				If rowHeader IsNot Nothing Then
					add(rowHeader, ROW_HEADER)
				ElseIf old IsNot Nothing Then
					remove(old)
				End If
				firePropertyChange("rowHeader", old, rowHeader)
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Creates a row-header viewport if necessary, sets
		''' its view and then adds the row-header viewport
		''' to the scrollpane.  For example:
		''' <pre>
		''' JScrollPane scrollpane = new JScrollPane();
		''' scrollpane.setViewportView(myBigComponentToScroll);
		''' scrollpane.setRowHeaderView(myBigComponentsRowHeader);
		''' </pre>
		''' </summary>
		''' <seealso cref= #setRowHeader </seealso>
		''' <seealso cref= JViewport#setView </seealso>
		''' <param name="view"> the component to display as the row header </param>
		Public Overridable Property rowHeaderView As java.awt.Component
			Set(ByVal view As java.awt.Component)
				If rowHeader Is Nothing Then rowHeader = createViewport()
				rowHeader.view = view
			End Set
		End Property



		''' <summary>
		''' Returns the column header. </summary>
		''' <returns> the <code>columnHeader</code> property </returns>
		''' <seealso cref= #setColumnHeader </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property columnHeader As JViewport
			Get
				Return columnHeader
			End Get
			Set(ByVal columnHeader As JViewport)
				Dim old As JViewport = columnHeader
				Me.columnHeader = columnHeader
				If columnHeader IsNot Nothing Then
					add(columnHeader, COLUMN_HEADER)
				ElseIf old IsNot Nothing Then
					remove(old)
				End If
				firePropertyChange("columnHeader", old, columnHeader)
    
				revalidate()
				repaint()
			End Set
		End Property





		''' <summary>
		''' Creates a column-header viewport if necessary, sets
		''' its view, and then adds the column-header viewport
		''' to the scrollpane.  For example:
		''' <pre>
		''' JScrollPane scrollpane = new JScrollPane();
		''' scrollpane.setViewportView(myBigComponentToScroll);
		''' scrollpane.setColumnHeaderView(myBigComponentsColumnHeader);
		''' </pre>
		''' </summary>
		''' <seealso cref= #setColumnHeader </seealso>
		''' <seealso cref= JViewport#setView
		''' </seealso>
		''' <param name="view"> the component to display as the column header </param>
		Public Overridable Property columnHeaderView As java.awt.Component
			Set(ByVal view As java.awt.Component)
				If columnHeader Is Nothing Then columnHeader = createViewport()
				columnHeader.view = view
			End Set
		End Property


		''' <summary>
		''' Returns the component at the specified corner. The
		''' <code>key</code> value specifying the corner is one of:
		''' <ul>
		''' <li>ScrollPaneConstants.LOWER_LEFT_CORNER
		''' <li>ScrollPaneConstants.LOWER_RIGHT_CORNER
		''' <li>ScrollPaneConstants.UPPER_LEFT_CORNER
		''' <li>ScrollPaneConstants.UPPER_RIGHT_CORNER
		''' <li>ScrollPaneConstants.LOWER_LEADING_CORNER
		''' <li>ScrollPaneConstants.LOWER_TRAILING_CORNER
		''' <li>ScrollPaneConstants.UPPER_LEADING_CORNER
		''' <li>ScrollPaneConstants.UPPER_TRAILING_CORNER
		''' </ul>
		''' </summary>
		''' <param name="key"> one of the values as shown above </param>
		''' <returns> the corner component (which may be <code>null</code>)
		'''         identified by the given key, or <code>null</code>
		'''         if the key is invalid </returns>
		''' <seealso cref= #setCorner </seealso>
		Public Overridable Function getCorner(ByVal key As String) As java.awt.Component
			Dim isLeftToRight As Boolean = componentOrientation.leftToRight
			If key.Equals(LOWER_LEADING_CORNER) Then
				key = If(isLeftToRight, LOWER_LEFT_CORNER, LOWER_RIGHT_CORNER)
			ElseIf key.Equals(LOWER_TRAILING_CORNER) Then
				key = If(isLeftToRight, LOWER_RIGHT_CORNER, LOWER_LEFT_CORNER)
			ElseIf key.Equals(UPPER_LEADING_CORNER) Then
				key = If(isLeftToRight, UPPER_LEFT_CORNER, UPPER_RIGHT_CORNER)
			ElseIf key.Equals(UPPER_TRAILING_CORNER) Then
				key = If(isLeftToRight, UPPER_RIGHT_CORNER, UPPER_LEFT_CORNER)
			End If
			If key.Equals(LOWER_LEFT_CORNER) Then
				Return lowerLeft
			ElseIf key.Equals(LOWER_RIGHT_CORNER) Then
				Return lowerRight
			ElseIf key.Equals(UPPER_LEFT_CORNER) Then
				Return upperLeft
			ElseIf key.Equals(UPPER_RIGHT_CORNER) Then
				Return upperRight
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Adds a child that will appear in one of the scroll panes
		''' corners, if there's room.   For example with both scrollbars
		''' showing (on the right and bottom edges of the scrollpane)
		''' the lower left corner component will be shown in the space
		''' between ends of the two scrollbars. Legal values for
		''' the <b>key</b> are:
		''' <ul>
		''' <li>ScrollPaneConstants.LOWER_LEFT_CORNER
		''' <li>ScrollPaneConstants.LOWER_RIGHT_CORNER
		''' <li>ScrollPaneConstants.UPPER_LEFT_CORNER
		''' <li>ScrollPaneConstants.UPPER_RIGHT_CORNER
		''' <li>ScrollPaneConstants.LOWER_LEADING_CORNER
		''' <li>ScrollPaneConstants.LOWER_TRAILING_CORNER
		''' <li>ScrollPaneConstants.UPPER_LEADING_CORNER
		''' <li>ScrollPaneConstants.UPPER_TRAILING_CORNER
		''' </ul>
		''' <p>
		''' Although "corner" doesn't match any beans property
		''' signature, <code>PropertyChange</code> events are generated with the
		''' property name set to the corner key.
		''' </summary>
		''' <param name="key"> identifies which corner the component will appear in </param>
		''' <param name="corner"> one of the following components:
		''' <ul>
		''' <li>lowerLeft
		''' <li>lowerRight
		''' <li>upperLeft
		''' <li>upperRight
		''' </ul> </param>
		''' <exception cref="IllegalArgumentException"> if corner key is invalid </exception>
		Public Overridable Sub setCorner(ByVal key As String, ByVal corner As java.awt.Component)
			Dim old As java.awt.Component
			Dim isLeftToRight As Boolean = componentOrientation.leftToRight
			If key.Equals(LOWER_LEADING_CORNER) Then
				key = If(isLeftToRight, LOWER_LEFT_CORNER, LOWER_RIGHT_CORNER)
			ElseIf key.Equals(LOWER_TRAILING_CORNER) Then
				key = If(isLeftToRight, LOWER_RIGHT_CORNER, LOWER_LEFT_CORNER)
			ElseIf key.Equals(UPPER_LEADING_CORNER) Then
				key = If(isLeftToRight, UPPER_LEFT_CORNER, UPPER_RIGHT_CORNER)
			ElseIf key.Equals(UPPER_TRAILING_CORNER) Then
				key = If(isLeftToRight, UPPER_RIGHT_CORNER, UPPER_LEFT_CORNER)
			End If
			If key.Equals(LOWER_LEFT_CORNER) Then
				old = lowerLeft
				lowerLeft = corner
			ElseIf key.Equals(LOWER_RIGHT_CORNER) Then
				old = lowerRight
				lowerRight = corner
			ElseIf key.Equals(UPPER_LEFT_CORNER) Then
				old = upperLeft
				upperLeft = corner
			ElseIf key.Equals(UPPER_RIGHT_CORNER) Then
				old = upperRight
				upperRight = corner
			Else
				Throw New System.ArgumentException("invalid corner key")
			End If
			If old IsNot Nothing Then remove(old)
			If corner IsNot Nothing Then add(corner, key)
			firePropertyChange(key, old, corner)
			revalidate()
			repaint()
		End Sub

		''' <summary>
		''' Sets the orientation for the vertical and horizontal
		''' scrollbars as determined by the
		''' <code>ComponentOrientation</code> argument.
		''' </summary>
		''' <param name="co"> one of the following values:
		''' <ul>
		''' <li>java.awt.ComponentOrientation.LEFT_TO_RIGHT
		''' <li>java.awt.ComponentOrientation.RIGHT_TO_LEFT
		''' <li>java.awt.ComponentOrientation.UNKNOWN
		''' </ul> </param>
		''' <seealso cref= java.awt.ComponentOrientation </seealso>
		Public Overridable Property componentOrientation As java.awt.ComponentOrientation
			Set(ByVal co As java.awt.ComponentOrientation)
				MyBase.componentOrientation = co
				If verticalScrollBar IsNot Nothing Then verticalScrollBar.componentOrientation = co
				If horizontalScrollBar IsNot Nothing Then horizontalScrollBar.componentOrientation = co
			End Set
		End Property

		''' <summary>
		''' Indicates whether or not scrolling will take place in response to the
		''' mouse wheel.  Wheel scrolling is enabled by default.
		''' </summary>
		''' <seealso cref= #setWheelScrollingEnabled
		''' @since 1.4
		''' @beaninfo
		'''       bound: true
		''' description: Flag for enabling/disabling mouse wheel scrolling </seealso>
		Public Overridable Property wheelScrollingEnabled As Boolean
			Get
				Return wheelScrollState
			End Get
			Set(ByVal handleWheel As Boolean)
				Dim old As Boolean = wheelScrollState
				wheelScrollState = handleWheel
				firePropertyChange("wheelScrollingEnabled", old, handleWheel)
			End Set
		End Property


		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
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
		''' Returns a string representation of this <code>JScrollPane</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JScrollPane</code>. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim viewportBorderString As String = (If(viewportBorder IsNot Nothing, viewportBorder.ToString(), ""))
			Dim viewportString As String = (If(viewport IsNot Nothing, viewport.ToString(), ""))
			Dim verticalScrollBarPolicyString As String
			If verticalScrollBarPolicy = VERTICAL_SCROLLBAR_AS_NEEDED Then
				verticalScrollBarPolicyString = "VERTICAL_SCROLLBAR_AS_NEEDED"
			ElseIf verticalScrollBarPolicy = VERTICAL_SCROLLBAR_NEVER Then
				verticalScrollBarPolicyString = "VERTICAL_SCROLLBAR_NEVER"
			ElseIf verticalScrollBarPolicy = VERTICAL_SCROLLBAR_ALWAYS Then
				verticalScrollBarPolicyString = "VERTICAL_SCROLLBAR_ALWAYS"
			Else
				verticalScrollBarPolicyString = ""
			End If
			Dim horizontalScrollBarPolicyString As String
			If horizontalScrollBarPolicy = HORIZONTAL_SCROLLBAR_AS_NEEDED Then
				horizontalScrollBarPolicyString = "HORIZONTAL_SCROLLBAR_AS_NEEDED"
			ElseIf horizontalScrollBarPolicy = HORIZONTAL_SCROLLBAR_NEVER Then
				horizontalScrollBarPolicyString = "HORIZONTAL_SCROLLBAR_NEVER"
			ElseIf horizontalScrollBarPolicy = HORIZONTAL_SCROLLBAR_ALWAYS Then
				horizontalScrollBarPolicyString = "HORIZONTAL_SCROLLBAR_ALWAYS"
			Else
				horizontalScrollBarPolicyString = ""
			End If
			Dim horizontalScrollBarString As String = (If(horizontalScrollBar IsNot Nothing, horizontalScrollBar.ToString(), ""))
			Dim verticalScrollBarString As String = (If(verticalScrollBar IsNot Nothing, verticalScrollBar.ToString(), ""))
			Dim columnHeaderString As String = (If(columnHeader IsNot Nothing, columnHeader.ToString(), ""))
			Dim rowHeaderString As String = (If(rowHeader IsNot Nothing, rowHeader.ToString(), ""))
			Dim lowerLeftString As String = (If(lowerLeft IsNot Nothing, lowerLeft.ToString(), ""))
			Dim lowerRightString As String = (If(lowerRight IsNot Nothing, lowerRight.ToString(), ""))
			Dim upperLeftString As String = (If(upperLeft IsNot Nothing, upperLeft.ToString(), ""))
			Dim upperRightString As String = (If(upperRight IsNot Nothing, upperRight.ToString(), ""))

			Return MyBase.paramString() & ",columnHeader=" & columnHeaderString & ",horizontalScrollBar=" & horizontalScrollBarString & ",horizontalScrollBarPolicy=" & horizontalScrollBarPolicyString & ",lowerLeft=" & lowerLeftString & ",lowerRight=" & lowerRightString & ",rowHeader=" & rowHeaderString & ",upperLeft=" & upperLeftString & ",upperRight=" & upperRightString & ",verticalScrollBar=" & verticalScrollBarString & ",verticalScrollBarPolicy=" & verticalScrollBarPolicyString & ",viewport=" & viewportString & ",viewportBorder=" & viewportBorderString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JScrollPane.
		''' For scroll panes, the AccessibleContext takes the form of an
		''' AccessibleJScrollPane.
		''' A new AccessibleJScrollPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJScrollPane that serves as the
		'''         AccessibleContext of this JScrollPane </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJScrollPane(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JScrollPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to scroll pane user-interface
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
		Protected Friend Class AccessibleJScrollPane
			Inherits AccessibleJComponent
			Implements ChangeListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As JScrollPane


			Protected Friend viewPort As JViewport = Nothing

	'        
	'         * Resets the viewport ChangeListener and PropertyChangeListener
	'         
			Public Overridable Sub resetViewPort()
				If viewPort IsNot Nothing Then
					viewPort.removeChangeListener(Me)
					viewPort.removePropertyChangeListener(Me)
				End If
				viewPort = outerInstance.viewport
				If viewPort IsNot Nothing Then
					viewPort.addChangeListener(Me)
					viewPort.addPropertyChangeListener(Me)
				End If
			End Sub

			''' <summary>
			''' AccessibleJScrollPane constructor
			''' </summary>
			Public Sub New(ByVal outerInstance As JScrollPane)
					Me.outerInstance = outerInstance
				MyBase.New()

				resetViewPort()

				' initialize the AccessibleRelationSets for the JScrollPane
				' and JScrollBar(s)
				Dim scrollBar As JScrollBar = outerInstance.horizontalScrollBar
				If scrollBar IsNot Nothing Then scrollBarRelations = scrollBar
				scrollBar = outerInstance.verticalScrollBar
				If scrollBar IsNot Nothing Then scrollBarRelations = scrollBar
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SCROLL_PANE
				End Get
			End Property

			''' <summary>
			''' Invoked when the target of the listener has changed its state.
			''' </summary>
			''' <param name="e">  a <code>ChangeEvent</code> object. Must not be null.
			''' </param>
			''' <exception cref="NullPointerException"> if the parameter is null. </exception>
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If e Is Nothing Then Throw New NullPointerException
				outerInstance.firePropertyChange(ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
			End Sub

			''' <summary>
			''' This method gets called when a bound property is changed. </summary>
			''' <param name="e"> A <code>PropertyChangeEvent</code> object describing
			''' the event source and the property that has changed. Must not be null.
			''' </param>
			''' <exception cref="NullPointerException"> if the parameter is null.
			''' @since 1.5 </exception>
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim propertyName As String = e.propertyName
				If propertyName = "horizontalScrollBar" OrElse propertyName = "verticalScrollBar" Then

					If TypeOf e.newValue Is JScrollBar Then scrollBarRelations = CType(e.newValue, JScrollBar)
				End If
			End Sub


	'        
	'         * Sets the CONTROLLER_FOR and CONTROLLED_BY AccessibleRelations for
	'         * the JScrollPane and JScrollBar. JScrollBar must not be null.
	'         
			Friend Overridable Property scrollBarRelations As JScrollBar
				Set(ByVal scrollBar As JScrollBar)
		'            
		'             * The JScrollBar is a CONTROLLER_FOR the JScrollPane.
		'             * The JScrollPane is CONTROLLED_BY the JScrollBar.
		'             
					Dim controlledBy As New AccessibleRelation(AccessibleRelation.CONTROLLED_BY, scrollBar)
					Dim controllerFor As New AccessibleRelation(AccessibleRelation.CONTROLLER_FOR, JScrollPane.this)
    
					' set the relation set for the scroll bar
					Dim ac As AccessibleContext = scrollBar.accessibleContext
					ac.accessibleRelationSet.add(controllerFor)
    
					' set the relation set for the scroll pane
					accessibleRelationSet.add(controlledBy)
				End Set
			End Property
		End Class
	End Class

End Namespace
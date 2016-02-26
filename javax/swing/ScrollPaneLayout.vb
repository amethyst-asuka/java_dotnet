Imports System
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

Namespace javax.swing




	''' <summary>
	''' The layout manager used by <code>JScrollPane</code>.
	''' <code>JScrollPaneLayout</code> is
	''' responsible for nine components: a viewport, two scrollbars,
	''' a row header, a column header, and four "corner" components.
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
	''' <seealso cref= JScrollPane </seealso>
	''' <seealso cref= JViewport
	''' 
	''' @author Hans Muller </seealso>
	<Serializable> _
	Public Class ScrollPaneLayout
		Implements java.awt.LayoutManager, ScrollPaneConstants

		''' <summary>
		''' The scrollpane's viewport child.
		''' Default is an empty <code>JViewport</code>. </summary>
		''' <seealso cref= JScrollPane#setViewport </seealso>
		Protected Friend viewport As JViewport


		''' <summary>
		''' The scrollpane's vertical scrollbar child.
		''' Default is a <code>JScrollBar</code>. </summary>
		''' <seealso cref= JScrollPane#setVerticalScrollBar </seealso>
		Protected Friend vsb As JScrollBar


		''' <summary>
		''' The scrollpane's horizontal scrollbar child.
		''' Default is a <code>JScrollBar</code>. </summary>
		''' <seealso cref= JScrollPane#setHorizontalScrollBar </seealso>
		Protected Friend hsb As JScrollBar


		''' <summary>
		''' The row header child.  Default is <code>null</code>. </summary>
		''' <seealso cref= JScrollPane#setRowHeader </seealso>
		Protected Friend rowHead As JViewport


		''' <summary>
		''' The column header child.  Default is <code>null</code>. </summary>
		''' <seealso cref= JScrollPane#setColumnHeader </seealso>
		Protected Friend colHead As JViewport


		''' <summary>
		''' The component to display in the lower left corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= JScrollPane#setCorner </seealso>
		Protected Friend lowerLeft As java.awt.Component


		''' <summary>
		''' The component to display in the lower right corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= JScrollPane#setCorner </seealso>
		Protected Friend lowerRight As java.awt.Component


		''' <summary>
		''' The component to display in the upper left corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= JScrollPane#setCorner </seealso>
		Protected Friend upperLeft As java.awt.Component


		''' <summary>
		''' The component to display in the upper right corner.
		''' Default is <code>null</code>. </summary>
		''' <seealso cref= JScrollPane#setCorner </seealso>
		Protected Friend upperRight As java.awt.Component


		''' <summary>
		''' The display policy for the vertical scrollbar.
		''' The default is <code>ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED</code>.
		''' <p>
		''' This field is obsolete, please use the <code>JScrollPane</code> field instead.
		''' </summary>
		''' <seealso cref= JScrollPane#setVerticalScrollBarPolicy </seealso>
		Protected Friend vsbPolicy As Integer = VERTICAL_SCROLLBAR_AS_NEEDED


		''' <summary>
		''' The display policy for the horizontal scrollbar.
		''' The default is <code>ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED</code>.
		''' <p>
		''' This field is obsolete, please use the <code>JScrollPane</code> field instead.
		''' </summary>
		''' <seealso cref= JScrollPane#setHorizontalScrollBarPolicy </seealso>
		Protected Friend hsbPolicy As Integer = HORIZONTAL_SCROLLBAR_AS_NEEDED


		''' <summary>
		''' This method is invoked after the ScrollPaneLayout is set as the
		''' LayoutManager of a <code>JScrollPane</code>.
		''' It initializes all of the internal fields that
		''' are ordinarily set by <code>addLayoutComponent</code>.  For example:
		''' <pre>
		''' ScrollPaneLayout mySPLayout = new ScrollPanelLayout() {
		'''     public void layoutContainer(Container p) {
		'''         super.layoutContainer(p);
		'''         // do some extra work here ...
		'''     }
		''' };
		''' scrollpane.setLayout(mySPLayout):
		''' </pre>
		''' </summary>
		Public Overridable Sub syncWithScrollPane(ByVal sp As JScrollPane)
			viewport = sp.viewport
			vsb = sp.verticalScrollBar
			hsb = sp.horizontalScrollBar
			rowHead = sp.rowHeader
			colHead = sp.columnHeader
			lowerLeft = sp.getCorner(LOWER_LEFT_CORNER)
			lowerRight = sp.getCorner(LOWER_RIGHT_CORNER)
			upperLeft = sp.getCorner(UPPER_LEFT_CORNER)
			upperRight = sp.getCorner(UPPER_RIGHT_CORNER)
			vsbPolicy = sp.verticalScrollBarPolicy
			hsbPolicy = sp.horizontalScrollBarPolicy
		End Sub


		''' <summary>
		''' Removes an existing component.  When a new component, such as
		''' the left corner, or vertical scrollbar, is added, the old one,
		''' if it exists, must be removed.
		''' <p>
		''' This method returns <code>newC</code>. If <code>oldC</code> is
		''' not equal to <code>newC</code> and is non-<code>null</code>,
		''' it will be removed from its parent.
		''' </summary>
		''' <param name="oldC"> the <code>Component</code> to replace </param>
		''' <param name="newC"> the <code>Component</code> to add </param>
		''' <returns> the <code>newC</code> </returns>
		Protected Friend Overridable Function addSingletonComponent(ByVal oldC As java.awt.Component, ByVal newC As java.awt.Component) As java.awt.Component
			If (oldC IsNot Nothing) AndAlso (oldC IsNot newC) Then oldC.parent.remove(oldC)
			Return newC
		End Function


		''' <summary>
		''' Adds the specified component to the layout. The layout is
		''' identified using one of:
		''' <ul>
		''' <li>ScrollPaneConstants.VIEWPORT
		''' <li>ScrollPaneConstants.VERTICAL_SCROLLBAR
		''' <li>ScrollPaneConstants.HORIZONTAL_SCROLLBAR
		''' <li>ScrollPaneConstants.ROW_HEADER
		''' <li>ScrollPaneConstants.COLUMN_HEADER
		''' <li>ScrollPaneConstants.LOWER_LEFT_CORNER
		''' <li>ScrollPaneConstants.LOWER_RIGHT_CORNER
		''' <li>ScrollPaneConstants.UPPER_LEFT_CORNER
		''' <li>ScrollPaneConstants.UPPER_RIGHT_CORNER
		''' </ul>
		''' </summary>
		''' <param name="s"> the component identifier </param>
		''' <param name="c"> the the component to be added </param>
		''' <exception cref="IllegalArgumentException"> if <code>s</code> is an invalid key </exception>
		Public Overridable Sub addLayoutComponent(ByVal s As String, ByVal c As java.awt.Component)
			If s.Equals(VIEWPORT) Then
				viewport = CType(addSingletonComponent(viewport, c), JViewport)
			ElseIf s.Equals(VERTICAL_SCROLLBAR) Then
				vsb = CType(addSingletonComponent(vsb, c), JScrollBar)
			ElseIf s.Equals(HORIZONTAL_SCROLLBAR) Then
				hsb = CType(addSingletonComponent(hsb, c), JScrollBar)
			ElseIf s.Equals(ROW_HEADER) Then
				rowHead = CType(addSingletonComponent(rowHead, c), JViewport)
			ElseIf s.Equals(COLUMN_HEADER) Then
				colHead = CType(addSingletonComponent(colHead, c), JViewport)
			ElseIf s.Equals(LOWER_LEFT_CORNER) Then
				lowerLeft = addSingletonComponent(lowerLeft, c)
			ElseIf s.Equals(LOWER_RIGHT_CORNER) Then
				lowerRight = addSingletonComponent(lowerRight, c)
			ElseIf s.Equals(UPPER_LEFT_CORNER) Then
				upperLeft = addSingletonComponent(upperLeft, c)
			ElseIf s.Equals(UPPER_RIGHT_CORNER) Then
				upperRight = addSingletonComponent(upperRight, c)
			Else
				Throw New System.ArgumentException("invalid layout key " & s)
			End If
		End Sub


		''' <summary>
		''' Removes the specified component from the layout.
		''' </summary>
		''' <param name="c"> the component to remove </param>
		Public Overridable Sub removeLayoutComponent(ByVal c As java.awt.Component)
			If c Is viewport Then
				viewport = Nothing
			ElseIf c Is vsb Then
				vsb = Nothing
			ElseIf c Is hsb Then
				hsb = Nothing
			ElseIf c Is rowHead Then
				rowHead = Nothing
			ElseIf c Is colHead Then
				colHead = Nothing
			ElseIf c Is lowerLeft Then
				lowerLeft = Nothing
			ElseIf c Is lowerRight Then
				lowerRight = Nothing
			ElseIf c Is upperLeft Then
				upperLeft = Nothing
			ElseIf c Is upperRight Then
				upperRight = Nothing
			End If
		End Sub


		''' <summary>
		''' Returns the vertical scrollbar-display policy.
		''' </summary>
		''' <returns> an integer giving the display policy </returns>
		''' <seealso cref= #setVerticalScrollBarPolicy </seealso>
		Public Overridable Property verticalScrollBarPolicy As Integer
			Get
				Return vsbPolicy
			End Get
			Set(ByVal x As Integer)
				Select Case x
				Case VERTICAL_SCROLLBAR_AS_NEEDED, VERTICAL_SCROLLBAR_NEVER, VERTICAL_SCROLLBAR_ALWAYS
						vsbPolicy = x
				Case Else
					Throw New System.ArgumentException("invalid verticalScrollBarPolicy")
				End Select
			End Set
		End Property




		''' <summary>
		''' Returns the horizontal scrollbar-display policy.
		''' </summary>
		''' <returns> an integer giving the display policy </returns>
		''' <seealso cref= #setHorizontalScrollBarPolicy </seealso>
		Public Overridable Property horizontalScrollBarPolicy As Integer
			Get
				Return hsbPolicy
			End Get
			Set(ByVal x As Integer)
				Select Case x
				Case HORIZONTAL_SCROLLBAR_AS_NEEDED, HORIZONTAL_SCROLLBAR_NEVER, HORIZONTAL_SCROLLBAR_ALWAYS
						hsbPolicy = x
				Case Else
					Throw New System.ArgumentException("invalid horizontalScrollBarPolicy")
				End Select
			End Set
		End Property



		''' <summary>
		''' Returns the <code>JViewport</code> object that displays the
		''' scrollable contents. </summary>
		''' <returns> the <code>JViewport</code> object that displays the scrollable contents </returns>
		''' <seealso cref= JScrollPane#getViewport </seealso>
		Public Overridable Property viewport As JViewport
			Get
				Return viewport
			End Get
		End Property


		''' <summary>
		''' Returns the <code>JScrollBar</code> object that handles horizontal scrolling. </summary>
		''' <returns> the <code>JScrollBar</code> object that handles horizontal scrolling </returns>
		''' <seealso cref= JScrollPane#getHorizontalScrollBar </seealso>
		Public Overridable Property horizontalScrollBar As JScrollBar
			Get
				Return hsb
			End Get
		End Property

		''' <summary>
		''' Returns the <code>JScrollBar</code> object that handles vertical scrolling. </summary>
		''' <returns> the <code>JScrollBar</code> object that handles vertical scrolling </returns>
		''' <seealso cref= JScrollPane#getVerticalScrollBar </seealso>
		Public Overridable Property verticalScrollBar As JScrollBar
			Get
				Return vsb
			End Get
		End Property


		''' <summary>
		''' Returns the <code>JViewport</code> object that is the row header. </summary>
		''' <returns> the <code>JViewport</code> object that is the row header </returns>
		''' <seealso cref= JScrollPane#getRowHeader </seealso>
		Public Overridable Property rowHeader As JViewport
			Get
				Return rowHead
			End Get
		End Property


		''' <summary>
		''' Returns the <code>JViewport</code> object that is the column header. </summary>
		''' <returns> the <code>JViewport</code> object that is the column header </returns>
		''' <seealso cref= JScrollPane#getColumnHeader </seealso>
		Public Overridable Property columnHeader As JViewport
			Get
				Return colHead
			End Get
		End Property


		''' <summary>
		''' Returns the <code>Component</code> at the specified corner. </summary>
		''' <param name="key"> the <code>String</code> specifying the corner </param>
		''' <returns> the <code>Component</code> at the specified corner, as defined in
		'''         <seealso cref="ScrollPaneConstants"/>; if <code>key</code> is not one of the
		'''          four corners, <code>null</code> is returned </returns>
		''' <seealso cref= JScrollPane#getCorner </seealso>
		Public Overridable Function getCorner(ByVal key As String) As java.awt.Component
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
		''' The preferred size of a <code>ScrollPane</code> is the size of the insets,
		''' plus the preferred size of the viewport, plus the preferred size of
		''' the visible headers, plus the preferred size of the scrollbars
		''' that will appear given the current view and the current
		''' scrollbar displayPolicies.
		''' <p>Note that the rowHeader is calculated as part of the preferred width
		''' and the colHeader is calculated as part of the preferred size.
		''' </summary>
		''' <param name="parent"> the <code>Container</code> that will be laid out </param>
		''' <returns> a <code>Dimension</code> object specifying the preferred size of the
		'''         viewport and any scrollbars </returns>
		''' <seealso cref= ViewportLayout </seealso>
		''' <seealso cref= LayoutManager </seealso>
		Public Overridable Function preferredLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
	'         Sync the (now obsolete) policy fields with the
	'         * JScrollPane.
	'         
			Dim scrollPane As JScrollPane = CType(parent, JScrollPane)
			vsbPolicy = scrollPane.verticalScrollBarPolicy
			hsbPolicy = scrollPane.horizontalScrollBarPolicy

			Dim insets As java.awt.Insets = parent.insets
			Dim prefWidth As Integer = insets.left + insets.right
			Dim prefHeight As Integer = insets.top + insets.bottom

	'         Note that viewport.getViewSize() is equivalent to
	'         * viewport.getView().getPreferredSize() modulo a null
	'         * view or a view whose size was explicitly set.
	'         

			Dim extentSize As java.awt.Dimension = Nothing
			Dim viewSize As java.awt.Dimension = Nothing
			Dim view As java.awt.Component = Nothing

			If viewport IsNot Nothing Then
				extentSize = viewport.preferredSize
				view = viewport.view
				If view IsNot Nothing Then
					viewSize = view.preferredSize
				Else
					viewSize = New java.awt.Dimension(0, 0)
				End If
			End If

	'         If there's a viewport add its preferredSize.
	'         

			If extentSize IsNot Nothing Then
				prefWidth += extentSize.width
				prefHeight += extentSize.height
			End If

	'         If there's a JScrollPane.viewportBorder, add its insets.
	'         

			Dim viewportBorder As Border = scrollPane.viewportBorder
			If viewportBorder IsNot Nothing Then
				Dim vpbInsets As java.awt.Insets = viewportBorder.getBorderInsets(parent)
				prefWidth += vpbInsets.left + vpbInsets.right
				prefHeight += vpbInsets.top + vpbInsets.bottom
			End If

	'         If a header exists and it's visible, factor its
	'         * preferred size in.
	'         

			If (rowHead IsNot Nothing) AndAlso rowHead.visible Then prefWidth += rowHead.preferredSize.width

			If (colHead IsNot Nothing) AndAlso colHead.visible Then prefHeight += colHead.preferredSize.height

	'         If a scrollbar is going to appear, factor its preferred size in.
	'         * If the scrollbars policy is AS_NEEDED, this can be a little
	'         * tricky:
	'         *
	'         * - If the view is a Scrollable then scrollableTracksViewportWidth
	'         * and scrollableTracksViewportHeight can be used to effectively
	'         * disable scrolling (if they're true) in their respective dimensions.
	'         *
	'         * - Assuming that a scrollbar hasn't been disabled by the
	'         * previous constraint, we need to decide if the scrollbar is going
	'         * to appear to correctly compute the JScrollPanes preferred size.
	'         * To do this we compare the preferredSize of the viewport (the
	'         * extentSize) to the preferredSize of the view.  Although we're
	'         * not responsible for laying out the view we'll assume that the
	'         * JViewport will always give it its preferredSize.
	'         

			If (vsb IsNot Nothing) AndAlso (vsbPolicy <> VERTICAL_SCROLLBAR_NEVER) Then
				If vsbPolicy = VERTICAL_SCROLLBAR_ALWAYS Then
					prefWidth += vsb.preferredSize.width
				ElseIf (viewSize IsNot Nothing) AndAlso (extentSize IsNot Nothing) Then
					Dim canScroll As Boolean = True
					If TypeOf view Is Scrollable Then canScroll = Not CType(view, Scrollable).scrollableTracksViewportHeight
					If canScroll AndAlso (viewSize.height > extentSize.height) Then prefWidth += vsb.preferredSize.width
				End If
			End If

			If (hsb IsNot Nothing) AndAlso (hsbPolicy <> HORIZONTAL_SCROLLBAR_NEVER) Then
				If hsbPolicy = HORIZONTAL_SCROLLBAR_ALWAYS Then
					prefHeight += hsb.preferredSize.height
				ElseIf (viewSize IsNot Nothing) AndAlso (extentSize IsNot Nothing) Then
					Dim canScroll As Boolean = True
					If TypeOf view Is Scrollable Then canScroll = Not CType(view, Scrollable).scrollableTracksViewportWidth
					If canScroll AndAlso (viewSize.width > extentSize.width) Then prefHeight += hsb.preferredSize.height
				End If
			End If

			Return New java.awt.Dimension(prefWidth, prefHeight)
		End Function


		''' <summary>
		''' The minimum size of a <code>ScrollPane</code> is the size of the insets
		''' plus minimum size of the viewport, plus the scrollpane's
		''' viewportBorder insets, plus the minimum size
		''' of the visible headers, plus the minimum size of the
		''' scrollbars whose displayPolicy isn't NEVER.
		''' </summary>
		''' <param name="parent"> the <code>Container</code> that will be laid out </param>
		''' <returns> a <code>Dimension</code> object specifying the minimum size </returns>
		Public Overridable Function minimumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
	'         Sync the (now obsolete) policy fields with the
	'         * JScrollPane.
	'         
			Dim scrollPane As JScrollPane = CType(parent, JScrollPane)
			vsbPolicy = scrollPane.verticalScrollBarPolicy
			hsbPolicy = scrollPane.horizontalScrollBarPolicy

			Dim insets As java.awt.Insets = parent.insets
			Dim minWidth As Integer = insets.left + insets.right
			Dim minHeight As Integer = insets.top + insets.bottom

	'         If there's a viewport add its minimumSize.
	'         

			If viewport IsNot Nothing Then
				Dim size As java.awt.Dimension = viewport.minimumSize
				minWidth += size.width
				minHeight += size.height
			End If

	'         If there's a JScrollPane.viewportBorder, add its insets.
	'         

			Dim viewportBorder As Border = scrollPane.viewportBorder
			If viewportBorder IsNot Nothing Then
				Dim vpbInsets As java.awt.Insets = viewportBorder.getBorderInsets(parent)
				minWidth += vpbInsets.left + vpbInsets.right
				minHeight += vpbInsets.top + vpbInsets.bottom
			End If

	'         If a header exists and it's visible, factor its
	'         * minimum size in.
	'         

			If (rowHead IsNot Nothing) AndAlso rowHead.visible Then
				Dim size As java.awt.Dimension = rowHead.minimumSize
				minWidth += size.width
				minHeight = Math.Max(minHeight, size.height)
			End If

			If (colHead IsNot Nothing) AndAlso colHead.visible Then
				Dim size As java.awt.Dimension = colHead.minimumSize
				minWidth = Math.Max(minWidth, size.width)
				minHeight += size.height
			End If

	'         If a scrollbar might appear, factor its minimum
	'         * size in.
	'         

			If (vsb IsNot Nothing) AndAlso (vsbPolicy <> VERTICAL_SCROLLBAR_NEVER) Then
				Dim size As java.awt.Dimension = vsb.minimumSize
				minWidth += size.width
				minHeight = Math.Max(minHeight, size.height)
			End If

			If (hsb IsNot Nothing) AndAlso (hsbPolicy <> HORIZONTAL_SCROLLBAR_NEVER) Then
				Dim size As java.awt.Dimension = hsb.minimumSize
				minWidth = Math.Max(minWidth, size.width)
				minHeight += size.height
			End If

			Return New java.awt.Dimension(minWidth, minHeight)
		End Function


		''' <summary>
		''' Lays out the scrollpane. The positioning of components depends on
		''' the following constraints:
		''' <ul>
		''' <li> The row header, if present and visible, gets its preferred
		''' width and the viewport's height.
		''' 
		''' <li> The column header, if present and visible, gets its preferred
		''' height and the viewport's width.
		''' 
		''' <li> If a vertical scrollbar is needed, i.e. if the viewport's extent
		''' height is smaller than its view height or if the <code>displayPolicy</code>
		''' is ALWAYS, it's treated like the row header with respect to its
		''' dimensions and is made visible.
		''' 
		''' <li> If a horizontal scrollbar is needed, it is treated like the
		''' column header (see the paragraph above regarding the vertical scrollbar).
		''' 
		''' <li> If the scrollpane has a non-<code>null</code>
		''' <code>viewportBorder</code>, then space is allocated for that.
		''' 
		''' <li> The viewport gets the space available after accounting for
		''' the previous constraints.
		''' 
		''' <li> The corner components, if provided, are aligned with the
		''' ends of the scrollbars and headers. If there is a vertical
		''' scrollbar, the right corners appear; if there is a horizontal
		''' scrollbar, the lower corners appear; a row header gets left
		''' corners, and a column header gets upper corners.
		''' </ul>
		''' </summary>
		''' <param name="parent"> the <code>Container</code> to lay out </param>
		Public Overridable Sub layoutContainer(ByVal parent As java.awt.Container)
	'         Sync the (now obsolete) policy fields with the
	'         * JScrollPane.
	'         
			Dim scrollPane As JScrollPane = CType(parent, JScrollPane)
			vsbPolicy = scrollPane.verticalScrollBarPolicy
			hsbPolicy = scrollPane.horizontalScrollBarPolicy

			Dim availR As java.awt.Rectangle = scrollPane.bounds
				availR.y = 0
				availR.x = availR.y

			Dim insets As java.awt.Insets = parent.insets
			availR.x = insets.left
			availR.y = insets.top
			availR.width -= insets.left + insets.right
			availR.height -= insets.top + insets.bottom

	'         Get the scrollPane's orientation.
	'         
			Dim leftToRight As Boolean = SwingUtilities.isLeftToRight(scrollPane)

	'         If there's a visible column header remove the space it
	'         * needs from the top of availR.  The column header is treated
	'         * as if it were fixed height, arbitrary width.
	'         

			Dim colHeadR As New java.awt.Rectangle(0, availR.y, 0, 0)

			If (colHead IsNot Nothing) AndAlso (colHead.visible) Then
				Dim colHeadHeight As Integer = Math.Min(availR.height, colHead.preferredSize.height)
				colHeadR.height = colHeadHeight
				availR.y += colHeadHeight
				availR.height -= colHeadHeight
			End If

	'         If there's a visible row header remove the space it needs
	'         * from the left or right of availR.  The row header is treated
	'         * as if it were fixed width, arbitrary height.
	'         

			Dim rowHeadR As New java.awt.Rectangle(0, 0, 0, 0)

			If (rowHead IsNot Nothing) AndAlso (rowHead.visible) Then
				Dim rowHeadWidth As Integer = Math.Min(availR.width, rowHead.preferredSize.width)
				rowHeadR.width = rowHeadWidth
				availR.width -= rowHeadWidth
				If leftToRight Then
					rowHeadR.x = availR.x
					availR.x += rowHeadWidth
				Else
					rowHeadR.x = availR.x + availR.width
				End If
			End If

	'         If there's a JScrollPane.viewportBorder, remove the
	'         * space it occupies for availR.
	'         

			Dim viewportBorder As Border = scrollPane.viewportBorder
			Dim vpbInsets As java.awt.Insets
			If viewportBorder IsNot Nothing Then
				vpbInsets = viewportBorder.getBorderInsets(parent)
				availR.x += vpbInsets.left
				availR.y += vpbInsets.top
				availR.width -= vpbInsets.left + vpbInsets.right
				availR.height -= vpbInsets.top + vpbInsets.bottom
			Else
				vpbInsets = New java.awt.Insets(0,0,0,0)
			End If


	'         At this point availR is the space available for the viewport
	'         * and scrollbars. rowHeadR is correct except for its height and y
	'         * and colHeadR is correct except for its width and x.  Once we're
	'         * through computing the dimensions  of these three parts we can
	'         * go back and set the dimensions of rowHeadR.height, rowHeadR.y,
	'         * colHeadR.width, colHeadR.x and the bounds for the corners.
	'         *
	'         * We'll decide about putting up scrollbars by comparing the
	'         * viewport views preferred size with the viewports extent
	'         * size (generally just its size).  Using the preferredSize is
	'         * reasonable because layout proceeds top down - so we expect
	'         * the viewport to be laid out next.  And we assume that the
	'         * viewports layout manager will give the view it's preferred
	'         * size.  One exception to this is when the view implements
	'         * Scrollable and Scrollable.getViewTracksViewport{Width,Height}
	'         * methods return true.  If the view is tracking the viewports
	'         * width we don't bother with a horizontal scrollbar, similarly
	'         * if view.getViewTracksViewport(Height) is true we don't bother
	'         * with a vertical scrollbar.
	'         

			Dim view As java.awt.Component = If(viewport IsNot Nothing, viewport.view, Nothing)
			Dim viewPrefSize As java.awt.Dimension = If(view IsNot Nothing, view.preferredSize, New java.awt.Dimension(0,0))

			Dim extentSize As java.awt.Dimension = If(viewport IsNot Nothing, viewport.toViewCoordinates(availR.size), New java.awt.Dimension(0,0))

			Dim viewTracksViewportWidth As Boolean = False
			Dim viewTracksViewportHeight As Boolean = False
			Dim isEmpty As Boolean = (availR.width < 0 OrElse availR.height < 0)
			Dim sv As Scrollable
			' Don't bother checking the Scrollable methods if there is no room
			' for the viewport, we aren't going to show any scrollbars in this
			' case anyway.
			If (Not isEmpty) AndAlso TypeOf view Is Scrollable Then
				sv = CType(view, Scrollable)
				viewTracksViewportWidth = sv.scrollableTracksViewportWidth
				viewTracksViewportHeight = sv.scrollableTracksViewportHeight
			Else
				sv = Nothing
			End If

	'         If there's a vertical scrollbar and we need one, allocate
	'         * space for it (we'll make it visible later). A vertical
	'         * scrollbar is considered to be fixed width, arbitrary height.
	'         

			Dim vsbR As New java.awt.Rectangle(0, availR.y - vpbInsets.top, 0, 0)

			Dim vsbNeeded As Boolean
			If isEmpty Then
				vsbNeeded = False
			ElseIf vsbPolicy = VERTICAL_SCROLLBAR_ALWAYS Then
				vsbNeeded = True
			ElseIf vsbPolicy = VERTICAL_SCROLLBAR_NEVER Then
				vsbNeeded = False
			Else ' vsbPolicy == VERTICAL_SCROLLBAR_AS_NEEDED
				vsbNeeded = (Not viewTracksViewportHeight) AndAlso (viewPrefSize.height > extentSize.height)
			End If


			If (vsb IsNot Nothing) AndAlso vsbNeeded Then
				adjustForVSB(True, availR, vsbR, vpbInsets, leftToRight)
				extentSize = viewport.toViewCoordinates(availR.size)
			End If

	'         If there's a horizontal scrollbar and we need one, allocate
	'         * space for it (we'll make it visible later). A horizontal
	'         * scrollbar is considered to be fixed height, arbitrary width.
	'         

			Dim hsbR As New java.awt.Rectangle(availR.x - vpbInsets.left, 0, 0, 0)
			Dim hsbNeeded As Boolean
			If isEmpty Then
				hsbNeeded = False
			ElseIf hsbPolicy = HORIZONTAL_SCROLLBAR_ALWAYS Then
				hsbNeeded = True
			ElseIf hsbPolicy = HORIZONTAL_SCROLLBAR_NEVER Then
				hsbNeeded = False
			Else ' hsbPolicy == HORIZONTAL_SCROLLBAR_AS_NEEDED
				hsbNeeded = (Not viewTracksViewportWidth) AndAlso (viewPrefSize.width > extentSize.width)
			End If

			If (hsb IsNot Nothing) AndAlso hsbNeeded Then
				adjustForHSB(True, availR, hsbR, vpbInsets)

	'             If we added the horizontal scrollbar then we've implicitly
	'             * reduced  the vertical space available to the viewport.
	'             * As a consequence we may have to add the vertical scrollbar,
	'             * if that hasn't been done so already.  Of course we
	'             * don't bother with any of this if the vsbPolicy is NEVER.
	'             
				If (vsb IsNot Nothing) AndAlso (Not vsbNeeded) AndAlso (vsbPolicy <> VERTICAL_SCROLLBAR_NEVER) Then

					extentSize = viewport.toViewCoordinates(availR.size)
					vsbNeeded = viewPrefSize.height > extentSize.height

					If vsbNeeded Then adjustForVSB(True, availR, vsbR, vpbInsets, leftToRight)
				End If
			End If

	'         Set the size of the viewport first, and then recheck the Scrollable
	'         * methods. Some components base their return values for the Scrollable
	'         * methods on the size of the Viewport, so that if we don't
	'         * ask after resetting the bounds we may have gotten the wrong
	'         * answer.
	'         

			If viewport IsNot Nothing Then
				viewport.bounds = availR

				If sv IsNot Nothing Then
					extentSize = viewport.toViewCoordinates(availR.size)

					Dim oldHSBNeeded As Boolean = hsbNeeded
					Dim oldVSBNeeded As Boolean = vsbNeeded
					viewTracksViewportWidth = sv.scrollableTracksViewportWidth
					viewTracksViewportHeight = sv.scrollableTracksViewportHeight
					If vsb IsNot Nothing AndAlso vsbPolicy = VERTICAL_SCROLLBAR_AS_NEEDED Then
						Dim newVSBNeeded As Boolean = (Not viewTracksViewportHeight) AndAlso (viewPrefSize.height > extentSize.height)
						If newVSBNeeded <> vsbNeeded Then
							vsbNeeded = newVSBNeeded
							adjustForVSB(vsbNeeded, availR, vsbR, vpbInsets, leftToRight)
							extentSize = viewport.toViewCoordinates(availR.size)
						End If
					End If
					If hsb IsNot Nothing AndAlso hsbPolicy =HORIZONTAL_SCROLLBAR_AS_NEEDED Then
						Dim newHSBbNeeded As Boolean = (Not viewTracksViewportWidth) AndAlso (viewPrefSize.width > extentSize.width)
						If newHSBbNeeded <> hsbNeeded Then
							hsbNeeded = newHSBbNeeded
							adjustForHSB(hsbNeeded, availR, hsbR, vpbInsets)
							If (vsb IsNot Nothing) AndAlso (Not vsbNeeded) AndAlso (vsbPolicy <> VERTICAL_SCROLLBAR_NEVER) Then

								extentSize = viewport.toViewCoordinates(availR.size)
								vsbNeeded = viewPrefSize.height > extentSize.height

								If vsbNeeded Then adjustForVSB(True, availR, vsbR, vpbInsets, leftToRight)
							End If
						End If
					End If
					If oldHSBNeeded <> hsbNeeded OrElse oldVSBNeeded <> vsbNeeded Then viewport.bounds = availR
				End If
			End If

	'         We now have the final size of the viewport: availR.
	'         * Now fixup the header and scrollbar widths/heights.
	'         
			vsbR.height = availR.height + vpbInsets.top + vpbInsets.bottom
			hsbR.width = availR.width + vpbInsets.left + vpbInsets.right
			rowHeadR.height = availR.height + vpbInsets.top + vpbInsets.bottom
			rowHeadR.y = availR.y - vpbInsets.top
			colHeadR.width = availR.width + vpbInsets.left + vpbInsets.right
			colHeadR.x = availR.x - vpbInsets.left

	'         Set the bounds of the remaining components.  The scrollbars
	'         * are made invisible if they're not needed.
	'         

			If rowHead IsNot Nothing Then rowHead.bounds = rowHeadR

			If colHead IsNot Nothing Then colHead.bounds = colHeadR

			If vsb IsNot Nothing Then
				If vsbNeeded Then
					If colHead IsNot Nothing AndAlso UIManager.getBoolean("ScrollPane.fillUpperCorner") Then
						If (leftToRight AndAlso upperRight Is Nothing) OrElse ((Not leftToRight) AndAlso upperLeft Is Nothing) Then
							' This is used primarily for GTK L&F, which needs to
							' extend the vertical scrollbar to fill the upper
							' corner near the column header.  Note that we skip
							' this step (and use the default behavior) if the
							' user has set a custom corner component.
							vsbR.y = colHeadR.y
							vsbR.height += colHeadR.height
						End If
					End If
					vsb.visible = True
					vsb.bounds = vsbR
				Else
					vsb.visible = False
				End If
			End If

			If hsb IsNot Nothing Then
				If hsbNeeded Then
					If rowHead IsNot Nothing AndAlso UIManager.getBoolean("ScrollPane.fillLowerCorner") Then
						If (leftToRight AndAlso lowerLeft Is Nothing) OrElse ((Not leftToRight) AndAlso lowerRight Is Nothing) Then
							' This is used primarily for GTK L&F, which needs to
							' extend the horizontal scrollbar to fill the lower
							' corner near the row header.  Note that we skip
							' this step (and use the default behavior) if the
							' user has set a custom corner component.
							If leftToRight Then hsbR.x = rowHeadR.x
							hsbR.width += rowHeadR.width
						End If
					End If
					hsb.visible = True
					hsb.bounds = hsbR
				Else
					hsb.visible = False
				End If
			End If

			If lowerLeft IsNot Nothing Then lowerLeft.boundsnds(If(leftToRight, rowHeadR.x, vsbR.x), hsbR.y,If(leftToRight, rowHeadR.width, vsbR.width), hsbR.height)

			If lowerRight IsNot Nothing Then lowerRight.boundsnds(If(leftToRight, vsbR.x, rowHeadR.x), hsbR.y,If(leftToRight, vsbR.width, rowHeadR.width), hsbR.height)

			If upperLeft IsNot Nothing Then upperLeft.boundsnds(If(leftToRight, rowHeadR.x, vsbR.x), colHeadR.y,If(leftToRight, rowHeadR.width, vsbR.width), colHeadR.height)

			If upperRight IsNot Nothing Then upperRight.boundsnds(If(leftToRight, vsbR.x, rowHeadR.x), colHeadR.y,If(leftToRight, vsbR.width, rowHeadR.width), colHeadR.height)
		End Sub

		''' <summary>
		''' Adjusts the <code>Rectangle</code> <code>available</code> based on if
		''' the vertical scrollbar is needed (<code>wantsVSB</code>).
		''' The location of the vsb is updated in <code>vsbR</code>, and
		''' the viewport border insets (<code>vpbInsets</code>) are used to offset
		''' the vsb. This is only called when <code>wantsVSB</code> has
		''' changed, eg you shouldn't invoke adjustForVSB(true) twice.
		''' </summary>
		Private Sub adjustForVSB(ByVal wantsVSB As Boolean, ByVal available As java.awt.Rectangle, ByVal vsbR As java.awt.Rectangle, ByVal vpbInsets As java.awt.Insets, ByVal leftToRight As Boolean)
			Dim oldWidth As Integer = vsbR.width
			If wantsVSB Then
				Dim vsbWidth As Integer = Math.Max(0, Math.Min(vsb.preferredSize.width, available.width))

				available.width -= vsbWidth
				vsbR.width = vsbWidth

				If leftToRight Then
					vsbR.x = available.x + available.width + vpbInsets.right
				Else
					vsbR.x = available.x - vpbInsets.left
					available.x += vsbWidth
				End If
			Else
				available.width += oldWidth
			End If
		End Sub

		''' <summary>
		''' Adjusts the <code>Rectangle</code> <code>available</code> based on if
		''' the horizontal scrollbar is needed (<code>wantsHSB</code>).
		''' The location of the hsb is updated in <code>hsbR</code>, and
		''' the viewport border insets (<code>vpbInsets</code>) are used to offset
		''' the hsb.  This is only called when <code>wantsHSB</code> has
		''' changed, eg you shouldn't invoked adjustForHSB(true) twice.
		''' </summary>
		Private Sub adjustForHSB(ByVal wantsHSB As Boolean, ByVal available As java.awt.Rectangle, ByVal hsbR As java.awt.Rectangle, ByVal vpbInsets As java.awt.Insets)
			Dim oldHeight As Integer = hsbR.height
			If wantsHSB Then
				Dim hsbHeight As Integer = Math.Max(0, Math.Min(available.height, hsb.preferredSize.height))

				available.height -= hsbHeight
				hsbR.y = available.y + available.height + vpbInsets.bottom
				hsbR.height = hsbHeight
			Else
				available.height += oldHeight
			End If
		End Sub



		''' <summary>
		''' Returns the bounds of the border around the specified scroll pane's
		''' viewport.
		''' </summary>
		''' <returns> the size and position of the viewport border </returns>
		''' @deprecated As of JDK version Swing1.1
		'''    replaced by <code>JScrollPane.getViewportBorderBounds()</code>. 
		<Obsolete("As of JDK version Swing1.1")> _
		Public Overridable Function getViewportBorderBounds(ByVal scrollpane As JScrollPane) As java.awt.Rectangle
			Return scrollpane.viewportBorderBounds
		End Function

		''' <summary>
		''' The UI resource version of <code>ScrollPaneLayout</code>.
		''' </summary>
		Public Class UIResource
			Inherits ScrollPaneLayout
			Implements javax.swing.plaf.UIResource

		End Class
	End Class

End Namespace
Imports System

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
	''' The <code>GridLayout</code> class is a layout manager that
	''' lays out a container's components in a rectangular grid.
	''' The container is divided into equal-sized rectangles,
	''' and one component is placed in each rectangle.
	''' For example, the following is an applet that lays out six buttons
	''' into three rows and two columns:
	''' 
	''' <hr><blockquote>
	''' <pre>
	''' import java.awt.*;
	''' import java.applet.Applet;
	''' public class ButtonGrid extends Applet {
	'''     public void init() {
	'''         setLayout(new GridLayout(3,2));
	'''         add(new Button("1"));
	'''         add(new Button("2"));
	'''         add(new Button("3"));
	'''         add(new Button("4"));
	'''         add(new Button("5"));
	'''         add(new Button("6"));
	'''     }
	''' }
	''' </pre></blockquote><hr>
	''' <p>
	''' If the container's <code>ComponentOrientation</code> property is horizontal
	''' and left-to-right, the above example produces the output shown in Figure 1.
	''' If the container's <code>ComponentOrientation</code> property is horizontal
	''' and right-to-left, the example produces the output shown in Figure 2.
	''' 
	''' <table style="float:center" WIDTH=600 summary="layout">
	''' <tr ALIGN=CENTER>
	''' <td><img SRC="doc-files/GridLayout-1.gif"
	'''      alt="Shows 6 buttons in rows of 2. Row 1 shows buttons 1 then 2.
	''' Row 2 shows buttons 3 then 4. Row 3 shows buttons 5 then 6.">
	''' </td>
	''' 
	''' <td ALIGN=CENTER><img SRC="doc-files/GridLayout-2.gif"
	'''                   alt="Shows 6 buttons in rows of 2. Row 1 shows buttons 2 then 1.
	''' Row 2 shows buttons 4 then 3. Row 3 shows buttons 6 then 5.">
	''' </td>
	''' </tr>
	''' 
	''' <tr ALIGN=CENTER>
	''' <td>Figure 1: Horizontal, Left-to-Right</td>
	''' 
	''' <td>Figure 2: Horizontal, Right-to-Left</td>
	''' </tr>
	''' </table>
	''' <p>
	''' When both the number of rows and the number of columns have
	''' been set to non-zero values, either by a constructor or
	''' by the <tt>setRows</tt> and <tt>setColumns</tt> methods, the number of
	''' columns specified is ignored.  Instead, the number of
	''' columns is determined from the specified number of rows
	''' and the total number of components in the layout. So, for
	''' example, if three rows and two columns have been specified
	''' and nine components are added to the layout, they will
	''' be displayed as three rows of three columns.  Specifying
	''' the number of columns affects the layout only when the
	''' number of rows is set to zero.
	''' 
	''' @author  Arthur van Hoff
	''' @since   JDK1.0
	''' </summary>
	<Serializable> _
	Public Class GridLayout
		Implements LayoutManager

	'    
	'     * serialVersionUID
	'     
		Private Const serialVersionUID As Long = -7411804673224730901L

		''' <summary>
		''' This is the horizontal gap (in pixels) which specifies the space
		''' between columns.  They can be changed at any time.
		''' This should be a non-negative  java.lang.[Integer].
		''' 
		''' @serial </summary>
		''' <seealso cref= #getHgap() </seealso>
		''' <seealso cref= #setHgap(int) </seealso>
		Friend hgap As Integer
		''' <summary>
		''' This is the vertical gap (in pixels) which specifies the space
		''' between rows.  They can be changed at any time.
		''' This should be a non negative  java.lang.[Integer].
		''' 
		''' @serial </summary>
		''' <seealso cref= #getVgap() </seealso>
		''' <seealso cref= #setVgap(int) </seealso>
		Friend vgap As Integer
		''' <summary>
		''' This is the number of rows specified for the grid.  The number
		''' of rows can be changed at any time.
		''' This should be a non negative integer, where '0' means
		''' 'any number' meaning that the number of Rows in that
		''' dimension depends on the other dimension.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getRows() </seealso>
		''' <seealso cref= #setRows(int) </seealso>
		Friend rows As Integer
		''' <summary>
		''' This is the number of columns specified for the grid.  The number
		''' of columns can be changed at any time.
		''' This should be a non negative integer, where '0' means
		''' 'any number' meaning that the number of Columns in that
		''' dimension depends on the other dimension.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getColumns() </seealso>
		''' <seealso cref= #setColumns(int) </seealso>
		Friend cols As Integer

		''' <summary>
		''' Creates a grid layout with a default of one column per component,
		''' in a single row.
		''' @since JDK1.1
		''' </summary>
		Public Sub New()
			Me.New(1, 0, 0, 0)
		End Sub

		''' <summary>
		''' Creates a grid layout with the specified number of rows and
		''' columns. All components in the layout are given equal size.
		''' <p>
		''' One, but not both, of <code>rows</code> and <code>cols</code> can
		''' be zero, which means that any number of objects can be placed in a
		''' row or in a column. </summary>
		''' <param name="rows">   the rows, with the value zero meaning
		'''                   any number of rows. </param>
		''' <param name="cols">   the columns, with the value zero meaning
		'''                   any number of columns. </param>
		Public Sub New(ByVal rows As Integer, ByVal cols As Integer)
			Me.New(rows, cols, 0, 0)
		End Sub

		''' <summary>
		''' Creates a grid layout with the specified number of rows and
		''' columns. All components in the layout are given equal size.
		''' <p>
		''' In addition, the horizontal and vertical gaps are set to the
		''' specified values. Horizontal gaps are placed between each
		''' of the columns. Vertical gaps are placed between each of
		''' the rows.
		''' <p>
		''' One, but not both, of <code>rows</code> and <code>cols</code> can
		''' be zero, which means that any number of objects can be placed in a
		''' row or in a column.
		''' <p>
		''' All <code>GridLayout</code> constructors defer to this one. </summary>
		''' <param name="rows">   the rows, with the value zero meaning
		'''                   any number of rows </param>
		''' <param name="cols">   the columns, with the value zero meaning
		'''                   any number of columns </param>
		''' <param name="hgap">   the horizontal gap </param>
		''' <param name="vgap">   the vertical gap </param>
		''' <exception cref="IllegalArgumentException">  if the value of both
		'''                  <code>rows</code> and <code>cols</code> is
		'''                  set to zero </exception>
		Public Sub New(ByVal rows As Integer, ByVal cols As Integer, ByVal hgap As Integer, ByVal vgap As Integer)
			If (rows = 0) AndAlso (cols = 0) Then Throw New IllegalArgumentException("rows and cols cannot both be zero")
			Me.rows = rows
			Me.cols = cols
			Me.hgap = hgap
			Me.vgap = vgap
		End Sub

		''' <summary>
		''' Gets the number of rows in this layout. </summary>
		''' <returns>    the number of rows in this layout
		''' @since     JDK1.1 </returns>
		Public Overridable Property rows As Integer
			Get
				Return rows
			End Get
			Set(ByVal rows As Integer)
				If (rows = 0) AndAlso (Me.cols = 0) Then Throw New IllegalArgumentException("rows and cols cannot both be zero")
				Me.rows = rows
			End Set
		End Property


		''' <summary>
		''' Gets the number of columns in this layout. </summary>
		''' <returns>     the number of columns in this layout
		''' @since      JDK1.1 </returns>
		Public Overridable Property columns As Integer
			Get
				Return cols
			End Get
			Set(ByVal cols As Integer)
				If (cols = 0) AndAlso (Me.rows = 0) Then Throw New IllegalArgumentException("rows and cols cannot both be zero")
				Me.cols = cols
			End Set
		End Property


		''' <summary>
		''' Gets the horizontal gap between components. </summary>
		''' <returns>       the horizontal gap between components
		''' @since        JDK1.1 </returns>
		Public Overridable Property hgap As Integer
			Get
				Return hgap
			End Get
			Set(ByVal hgap As Integer)
				Me.hgap = hgap
			End Set
		End Property


		''' <summary>
		''' Gets the vertical gap between components. </summary>
		''' <returns>       the vertical gap between components
		''' @since        JDK1.1 </returns>
		Public Overridable Property vgap As Integer
			Get
				Return vgap
			End Get
			Set(ByVal vgap As Integer)
				Me.vgap = vgap
			End Set
		End Property


		''' <summary>
		''' Adds the specified component with the specified name to the layout. </summary>
		''' <param name="name"> the name of the component </param>
		''' <param name="comp"> the component to be added </param>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component) Implements LayoutManager.addLayoutComponent
		End Sub

		''' <summary>
		''' Removes the specified component from the layout. </summary>
		''' <param name="comp"> the component to be removed </param>
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component) Implements LayoutManager.removeLayoutComponent
		End Sub

		''' <summary>
		''' Determines the preferred size of the container argument using
		''' this grid layout.
		''' <p>
		''' The preferred width of a grid layout is the largest preferred
		''' width of all of the components in the container times the number of
		''' columns, plus the horizontal padding times the number of columns
		''' minus one, plus the left and right insets of the target container.
		''' <p>
		''' The preferred height of a grid layout is the largest preferred
		''' height of all of the components in the container times the number of
		''' rows, plus the vertical padding times the number of rows minus one,
		''' plus the top and bottom insets of the target container.
		''' </summary>
		''' <param name="parent">   the container in which to do the layout </param>
		''' <returns>    the preferred dimensions to lay out the
		'''                      subcomponents of the specified container </returns>
		''' <seealso cref=       java.awt.GridLayout#minimumLayoutSize </seealso>
		''' <seealso cref=       java.awt.Container#getPreferredSize() </seealso>
		Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension Implements LayoutManager.preferredLayoutSize
		  SyncLock parent.treeLock
			Dim insets_Renamed As Insets = parent.insets
			Dim ncomponents As Integer = parent.componentCount
			Dim nrows As Integer = rows
			Dim ncols As Integer = cols

			If nrows > 0 Then
				ncols = (ncomponents + nrows - 1) \ nrows
			Else
				nrows = (ncomponents + ncols - 1) \ ncols
			End If
			Dim w As Integer = 0
			Dim h As Integer = 0
			For i As Integer = 0 To ncomponents - 1
				Dim comp As Component = parent.getComponent(i)
				Dim d As Dimension = comp.preferredSize
				If w < d.width Then w = d.width
				If h < d.height Then h = d.height
			Next i
			Return New Dimension(insets_Renamed.left + insets_Renamed.right + ncols*w + (ncols-1)*hgap, insets_Renamed.top + insets_Renamed.bottom + nrows*h + (nrows-1)*vgap)
		  End SyncLock
		End Function

		''' <summary>
		''' Determines the minimum size of the container argument using this
		''' grid layout.
		''' <p>
		''' The minimum width of a grid layout is the largest minimum width
		''' of all of the components in the container times the number of columns,
		''' plus the horizontal padding times the number of columns minus one,
		''' plus the left and right insets of the target container.
		''' <p>
		''' The minimum height of a grid layout is the largest minimum height
		''' of all of the components in the container times the number of rows,
		''' plus the vertical padding times the number of rows minus one, plus
		''' the top and bottom insets of the target container.
		''' </summary>
		''' <param name="parent">   the container in which to do the layout </param>
		''' <returns>      the minimum dimensions needed to lay out the
		'''                      subcomponents of the specified container </returns>
		''' <seealso cref=         java.awt.GridLayout#preferredLayoutSize </seealso>
		''' <seealso cref=         java.awt.Container#doLayout </seealso>
		Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension Implements LayoutManager.minimumLayoutSize
		  SyncLock parent.treeLock
			Dim insets_Renamed As Insets = parent.insets
			Dim ncomponents As Integer = parent.componentCount
			Dim nrows As Integer = rows
			Dim ncols As Integer = cols

			If nrows > 0 Then
				ncols = (ncomponents + nrows - 1) \ nrows
			Else
				nrows = (ncomponents + ncols - 1) \ ncols
			End If
			Dim w As Integer = 0
			Dim h As Integer = 0
			For i As Integer = 0 To ncomponents - 1
				Dim comp As Component = parent.getComponent(i)
				Dim d As Dimension = comp.minimumSize
				If w < d.width Then w = d.width
				If h < d.height Then h = d.height
			Next i
			Return New Dimension(insets_Renamed.left + insets_Renamed.right + ncols*w + (ncols-1)*hgap, insets_Renamed.top + insets_Renamed.bottom + nrows*h + (nrows-1)*vgap)
		  End SyncLock
		End Function

		''' <summary>
		''' Lays out the specified container using this layout.
		''' <p>
		''' This method reshapes the components in the specified target
		''' container in order to satisfy the constraints of the
		''' <code>GridLayout</code> object.
		''' <p>
		''' The grid layout manager determines the size of individual
		''' components by dividing the free space in the container into
		''' equal-sized portions according to the number of rows and columns
		''' in the layout. The container's free space equals the container's
		''' size minus any insets and any specified horizontal or vertical
		''' gap. All components in a grid layout are given the same size.
		''' </summary>
		''' <param name="parent">   the container in which to do the layout </param>
		''' <seealso cref=        java.awt.Container </seealso>
		''' <seealso cref=        java.awt.Container#doLayout </seealso>
		Public Overridable Sub layoutContainer(ByVal parent As Container) Implements LayoutManager.layoutContainer
		  SyncLock parent.treeLock
			Dim insets_Renamed As Insets = parent.insets
			Dim ncomponents As Integer = parent.componentCount
			Dim nrows As Integer = rows
			Dim ncols As Integer = cols
			Dim ltr As Boolean = parent.componentOrientation.leftToRight

			If ncomponents = 0 Then Return
			If nrows > 0 Then
				ncols = (ncomponents + nrows - 1) \ nrows
			Else
				nrows = (ncomponents + ncols - 1) \ ncols
			End If
			' 4370316. To position components in the center we should:
			' 1. get an amount of extra space within Container
			' 2. incorporate half of that value to the left/top position
			' Note that we use trancating division for widthOnComponent
			' The reminder goes to extraWidthAvailable
			Dim totalGapsWidth As Integer = (ncols - 1) * hgap
			Dim widthWOInsets As Integer = parent.width - (insets_Renamed.left + insets_Renamed.right)
			Dim widthOnComponent As Integer = (widthWOInsets - totalGapsWidth) \ ncols
			Dim extraWidthAvailable As Integer = (widthWOInsets - (widthOnComponent * ncols + totalGapsWidth)) \ 2

			Dim totalGapsHeight As Integer = (nrows - 1) * vgap
			Dim heightWOInsets As Integer = parent.height - (insets_Renamed.top + insets_Renamed.bottom)
			Dim heightOnComponent As Integer = (heightWOInsets - totalGapsHeight) \ nrows
			Dim extraHeightAvailable As Integer = (heightWOInsets - (heightOnComponent * nrows + totalGapsHeight)) \ 2
			If ltr Then
				Dim c As Integer = 0
				Dim x As Integer = insets_Renamed.left + extraWidthAvailable
				Do While c < ncols
					Dim r As Integer = 0
					Dim y As Integer = insets_Renamed.top + extraHeightAvailable
					Do While r < nrows
						Dim i As Integer = r * ncols + c
						If i < ncomponents Then parent.getComponent(i).boundsnds(x, y, widthOnComponent, heightOnComponent)
						r += 1
						y += heightOnComponent + vgap
					Loop
					c += 1
					x += widthOnComponent + hgap
				Loop
			Else
				Dim c As Integer = 0
				Dim x As Integer = (parent.width - insets_Renamed.right - widthOnComponent) - extraWidthAvailable
				Do While c < ncols
					Dim r As Integer = 0
					Dim y As Integer = insets_Renamed.top + extraHeightAvailable
					Do While r < nrows
						Dim i As Integer = r * ncols + c
						If i < ncomponents Then parent.getComponent(i).boundsnds(x, y, widthOnComponent, heightOnComponent)
						r += 1
						y += heightOnComponent + vgap
					Loop
					c += 1
					x -= widthOnComponent + hgap
				Loop
			End If
		  End SyncLock
		End Sub

		''' <summary>
		''' Returns the string representation of this grid layout's values. </summary>
		''' <returns>     a string representation of this grid layout </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[hgap=" & hgap & ",vgap=" & vgap & ",rows=" & rows & ",cols=" & cols & "]"
		End Function
	End Class

End Namespace
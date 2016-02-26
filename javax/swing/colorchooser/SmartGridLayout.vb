Imports System
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.text

'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.colorchooser



	''' <summary>
	''' A better GridLayout class
	'''  
	''' @author Steve Wilson
	''' </summary>
	<Serializable> _
	Friend Class SmartGridLayout
		Implements LayoutManager

	  Friend rows As Integer = 2
	  Friend columns As Integer = 2
	  Friend xGap As Integer = 2
	  Friend yGap As Integer = 2
	  Friend componentCount As Integer = 0
	  Friend layoutGrid As Component()()


	  Public Sub New(ByVal numColumns As Integer, ByVal numRows As Integer)
		rows = numRows
		columns = numColumns
		layoutGrid = RectangularArrays.ReturnRectangularComponentArray(numColumns, numRows)

	  End Sub


	  Public Overridable Sub layoutContainer(ByVal c As Container)

		buildLayoutGrid(c)

		Dim rowHeights As Integer() = New Integer(rows - 1){}
		Dim columnWidths As Integer() = New Integer(columns - 1){}

		For row As Integer = 0 To rows - 1
			rowHeights(row) = computeRowHeight(row)
		Next row

		For column As Integer = 0 To columns - 1
			columnWidths(column) = computeColumnWidth(column)
		Next column


		Dim insets As Insets = c.insets

		If c.componentOrientation.leftToRight Then
			Dim horizLoc As Integer = insets.left
			For column As Integer = 0 To columns - 1
			  Dim vertLoc As Integer = insets.top

			  For row As Integer = 0 To rows - 1
				Dim current As Component = layoutGrid(column)(row)

				current.boundsnds(horizLoc, vertLoc, columnWidths(column), rowHeights(row))
				'  System.out.println(current.getBounds());
				vertLoc += (rowHeights(row) + yGap)
			  Next row
			  horizLoc += (columnWidths(column) + xGap)
			Next column
		Else
			Dim horizLoc As Integer = c.width - insets.right
			For column As Integer = 0 To columns - 1
			  Dim vertLoc As Integer = insets.top
			  horizLoc -= columnWidths(column)

			  For row As Integer = 0 To rows - 1
				Dim current As Component = layoutGrid(column)(row)

				current.boundsnds(horizLoc, vertLoc, columnWidths(column), rowHeights(row))
				'  System.out.println(current.getBounds());
				vertLoc += (rowHeights(row) + yGap)
			  Next row
			  horizLoc -= xGap
			Next column
		End If



	  End Sub

	  Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension

		buildLayoutGrid(c)
		Dim insets As Insets = c.insets



		Dim height As Integer = 0
		Dim width As Integer = 0

		For row As Integer = 0 To rows - 1
			height += computeRowHeight(row)
		Next row

		For column As Integer = 0 To columns - 1
			width += computeColumnWidth(column)
		Next column

		height += (yGap * (rows - 1)) + insets.top + insets.bottom
		width += (xGap * (columns - 1)) + insets.right + insets.left

		Return New Dimension(width, height)


	  End Function

	  Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
		  Return minimumLayoutSize(c)
	  End Function


	  Public Overridable Sub addLayoutComponent(ByVal s As String, ByVal c As Component)
	  End Sub

	  Public Overridable Sub removeLayoutComponent(ByVal c As Component)
	  End Sub


	  Private Sub buildLayoutGrid(ByVal c As Container)

		  Dim children As Component() = c.components

		  For componentCount As Integer = 0 To children.Length - 1
			'      System.out.println("Children: " +componentCount);
			Dim row As Integer = 0
			Dim column As Integer = 0

			If componentCount <> 0 Then
			  column = componentCount Mod columns
			  row = (componentCount - column) \ columns
			End If

			'      System.out.println("inserting into: "+ column +  " " + row);

			layoutGrid(column)(row) = children(componentCount)
		  Next componentCount
	  End Sub

	  Private Function computeColumnWidth(ByVal columnNum As Integer) As Integer
		Dim maxWidth As Integer = 1
		For row As Integer = 0 To rows - 1
		  Dim width As Integer = layoutGrid(columnNum)(row).preferredSize.width
		  If width > maxWidth Then maxWidth = width
		Next row
		Return maxWidth
	  End Function

	  Private Function computeRowHeight(ByVal rowNum As Integer) As Integer
		Dim maxHeight As Integer = 1
		For column As Integer = 0 To columns - 1
		  Dim height As Integer = layoutGrid(column)(rowNum).preferredSize.height
		  If height > maxHeight Then maxHeight = height
		Next column
		Return maxHeight
	  End Function

	End Class

End Namespace

'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class provides the logic to simulate Java rectangular arrays, which are jagged
'	arrays with inner arrays of the same length.
'----------------------------------------------------------------------------------------
Partial Friend Class RectangularArrays
    Friend Shared Function ReturnRectangularComponentArray(ByVal Size1 As Integer, ByVal Size2 As Integer) As Component()()
        Dim Array As Component()() = New Component(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Component(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class
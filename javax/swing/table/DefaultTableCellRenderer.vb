Imports System
Imports javax.swing
Imports javax.swing.border

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The standard class for rendering (displaying) individual cells
	''' in a <code>JTable</code>.
	''' <p>
	''' 
	''' <strong><a name="override">Implementation Note:</a></strong>
	''' This class inherits from <code>JLabel</code>, a standard component class.
	''' However <code>JTable</code> employs a unique mechanism for rendering
	''' its cells and therefore requires some slightly modified behavior
	''' from its cell renderer.
	''' The table class defines a single cell renderer and uses it as a
	''' as a rubber-stamp for rendering all cells in the table;
	''' it renders the first cell,
	''' changes the contents of that cell renderer,
	''' shifts the origin to the new location, re-draws it, and so on.
	''' The standard <code>JLabel</code> component was not
	''' designed to be used this way and we want to avoid
	''' triggering a <code>revalidate</code> each time the
	''' cell is drawn. This would greatly decrease performance because the
	''' <code>revalidate</code> message would be
	''' passed up the hierarchy of the container to determine whether any other
	''' components would be affected.
	''' As the renderer is only parented for the lifetime of a painting operation
	''' we similarly want to avoid the overhead associated with walking the
	''' hierarchy for painting operations.
	''' So this class
	''' overrides the <code>validate</code>, <code>invalidate</code>,
	''' <code>revalidate</code>, <code>repaint</code>, and
	''' <code>firePropertyChange</code> methods to be
	''' no-ops and override the <code>isOpaque</code> method solely to improve
	''' performance.  If you write your own renderer,
	''' please keep this performance consideration in mind.
	''' <p>
	''' 
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Philip Milne </summary>
	''' <seealso cref= JTable </seealso>
	<Serializable> _
	Public Class DefaultTableCellRenderer
		Inherits JLabel
		Implements TableCellRenderer

	   ''' <summary>
	   ''' An empty <code>Border</code>. This field might not be used. To change the
	   ''' <code>Border</code> used by this renderer override the
	   ''' <code>getTableCellRendererComponent</code> method and set the border
	   ''' of the returned component directly.
	   ''' </summary>
		Private Shared ReadOnly SAFE_NO_FOCUS_BORDER As Border = New EmptyBorder(1, 1, 1, 1)
		Private Shared ReadOnly DEFAULT_NO_FOCUS_BORDER As Border = New EmptyBorder(1, 1, 1, 1)
		Protected Friend Shared noFocusBorder As Border = DEFAULT_NO_FOCUS_BORDER

		' We need a place to store the color the JLabel should be returned
		' to after its foreground and background colors have been set
		' to the selection background color.
		' These ivars will be made protected when their names are finalized.
		Private unselectedForeground As java.awt.Color
		Private unselectedBackground As java.awt.Color

		''' <summary>
		''' Creates a default table cell renderer.
		''' </summary>
		Public Sub New()
			MyBase.New()
			opaque = True
			border = noFocusBorder
			name = "Table.cellRenderer"
		End Sub

		Private Property noFocusBorder As Border
			Get
				Dim ___border As Border = sun.swing.DefaultLookup.getBorder(Me, ui, "Table.cellNoFocusBorder")
				If System.securityManager IsNot Nothing Then
					If ___border IsNot Nothing Then Return ___border
					Return SAFE_NO_FOCUS_BORDER
				ElseIf ___border IsNot Nothing Then
					If noFocusBorder Is Nothing OrElse noFocusBorder Is DEFAULT_NO_FOCUS_BORDER Then Return ___border
				End If
				Return noFocusBorder
			End Get
		End Property

		''' <summary>
		''' Overrides <code>JComponent.setForeground</code> to assign
		''' the unselected-foreground color to the specified color.
		''' </summary>
		''' <param name="c"> set the foreground color to this value </param>
		Public Overridable Property foreground As java.awt.Color
			Set(ByVal c As java.awt.Color)
				MyBase.foreground = c
				unselectedForeground = c
			End Set
		End Property

		''' <summary>
		''' Overrides <code>JComponent.setBackground</code> to assign
		''' the unselected-background color to the specified color.
		''' </summary>
		''' <param name="c"> set the background color to this value </param>
		Public Overridable Property background As java.awt.Color
			Set(ByVal c As java.awt.Color)
				MyBase.background = c
				unselectedBackground = c
			End Set
		End Property

		''' <summary>
		''' Notification from the <code>UIManager</code> that the look and feel
		''' [L&amp;F] has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			MyBase.updateUI()
			foreground = Nothing
			background = Nothing
		End Sub

		' implements javax.swing.table.TableCellRenderer
		''' 
		''' <summary>
		''' Returns the default table cell renderer.
		''' <p>
		''' During a printing operation, this method will be called with
		''' <code>isSelected</code> and <code>hasFocus</code> values of
		''' <code>false</code> to prevent selection and focus from appearing
		''' in the printed output. To do other customization based on whether
		''' or not the table is being printed, check the return value from
		''' <seealso cref="javax.swing.JComponent#isPaintingForPrint()"/>.
		''' </summary>
		''' <param name="table">  the <code>JTable</code> </param>
		''' <param name="value">  the value to assign to the cell at
		'''                  <code>[row, column]</code> </param>
		''' <param name="isSelected"> true if cell is selected </param>
		''' <param name="hasFocus"> true if cell has focus </param>
		''' <param name="row">  the row of the cell to render </param>
		''' <param name="column"> the column of the cell to render </param>
		''' <returns> the default table cell renderer </returns>
		''' <seealso cref= javax.swing.JComponent#isPaintingForPrint() </seealso>
		Public Overridable Function getTableCellRendererComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As java.awt.Component Implements TableCellRenderer.getTableCellRendererComponent
			If table Is Nothing Then Return Me

			Dim fg As java.awt.Color = Nothing
			Dim bg As java.awt.Color = Nothing

			Dim ___dropLocation As JTable.DropLocation = table.dropLocation
			If ___dropLocation IsNot Nothing AndAlso (Not ___dropLocation.insertRow) AndAlso (Not ___dropLocation.insertColumn) AndAlso ___dropLocation.row = row AndAlso ___dropLocation.column = column Then

				fg = sun.swing.DefaultLookup.getColor(Me, ui, "Table.dropCellForeground")
				bg = sun.swing.DefaultLookup.getColor(Me, ui, "Table.dropCellBackground")

				isSelected = True
			End If

			If isSelected Then
				MyBase.foreground = If(fg Is Nothing, table.selectionForeground, fg)
				MyBase.background = If(bg Is Nothing, table.selectionBackground, bg)
			Else
				Dim ___background As java.awt.Color = If(unselectedBackground IsNot Nothing, unselectedBackground, table.background)
				If ___background Is Nothing OrElse TypeOf ___background Is javax.swing.plaf.UIResource Then
					Dim alternateColor As java.awt.Color = sun.swing.DefaultLookup.getColor(Me, ui, "Table.alternateRowColor")
					If alternateColor IsNot Nothing AndAlso row Mod 2 <> 0 Then ___background = alternateColor
				End If
				MyBase.foreground = If(unselectedForeground IsNot Nothing, unselectedForeground, table.foreground)
				MyBase.background = ___background
			End If

			font = table.font

			If hasFocus Then
				Dim ___border As Border = Nothing
				If isSelected Then ___border = sun.swing.DefaultLookup.getBorder(Me, ui, "Table.focusSelectedCellHighlightBorder")
				If ___border Is Nothing Then ___border = sun.swing.DefaultLookup.getBorder(Me, ui, "Table.focusCellHighlightBorder")
				border = ___border

				If (Not isSelected) AndAlso table.isCellEditable(row, column) Then
					Dim col As java.awt.Color
					col = sun.swing.DefaultLookup.getColor(Me, ui, "Table.focusCellForeground")
					If col IsNot Nothing Then MyBase.foreground = col
					col = sun.swing.DefaultLookup.getColor(Me, ui, "Table.focusCellBackground")
					If col IsNot Nothing Then MyBase.background = col
				End If
			Else
				border = noFocusBorder
			End If

			value = value

			Return Me
		End Function

	'    
	'     * The following methods are overridden as a performance measure to
	'     * to prune code-paths are often called in the case of renders
	'     * but which we know are unnecessary.  Great care should be taken
	'     * when writing your own renderer to weigh the benefits and
	'     * drawbacks of overriding methods like these.
	'     

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Public Property Overrides opaque As Boolean
			Get
				Dim back As java.awt.Color = background
				Dim p As java.awt.Component = parent
				If p IsNot Nothing Then p = p.parent
    
				' p should now be the JTable.
				Dim colorMatch As Boolean = (back IsNot Nothing) AndAlso (p IsNot Nothing) AndAlso back.Equals(p.background) AndAlso p.opaque
				Return (Not colorMatch) AndAlso MyBase.opaque
			End Get
		End Property

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' 
		''' @since 1.5
		''' </summary>
		Public Overridable Sub invalidate()
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Public Overridable Sub validate()
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Public Overrides Sub revalidate()
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Public Overrides Sub repaint(ByVal tm As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Public Overridable Sub repaint(ByVal r As java.awt.Rectangle)
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' 
		''' @since 1.5
		''' </summary>
		Public Overridable Sub repaint()
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			' Strings get interned...
			If propertyName="text" OrElse propertyName = "labelFor" OrElse propertyName = "displayedMnemonic" OrElse ((propertyName = "font" OrElse propertyName = "foreground") AndAlso oldValue IsNot newValue AndAlso getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey) IsNot Nothing) Then MyBase.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' </summary>
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Boolean, ByVal newValue As Boolean)
		End Sub


		''' <summary>
		''' Sets the <code>String</code> object for the cell being rendered to
		''' <code>value</code>.
		''' </summary>
		''' <param name="value">  the string value for this cell; if value is
		'''          <code>null</code> it sets the text value to an empty string </param>
		''' <seealso cref= JLabel#setText
		'''  </seealso>
		Protected Friend Overridable Property value As Object
			Set(ByVal value As Object)
				text = If(value Is Nothing, "", value.ToString())
			End Set
		End Property


		''' <summary>
		''' A subclass of <code>DefaultTableCellRenderer</code> that
		''' implements <code>UIResource</code>.
		''' <code>DefaultTableCellRenderer</code> doesn't implement
		''' <code>UIResource</code>
		''' directly so that applications can safely override the
		''' <code>cellRenderer</code> property with
		''' <code>DefaultTableCellRenderer</code> subclasses.
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
		Public Class UIResource
			Inherits DefaultTableCellRenderer
			Implements javax.swing.plaf.UIResource

		End Class

	End Class

End Namespace
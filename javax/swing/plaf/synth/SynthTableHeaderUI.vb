Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
Imports javax.swing.table
Imports sun.swing.table

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth

	''' <summary>
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.table.JTableHeader"/>.
	''' 
	''' @author Alan Chung
	''' @author Philip Milne
	''' @since 1.7
	''' </summary>
	Public Class SynthTableHeaderUI
		Inherits BasicTableHeaderUI
		Implements PropertyChangeListener, SynthUI

	'
	' Instance Variables
	'

		Private prevRenderer As TableCellRenderer = Nothing

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="h"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal h As JComponent) As ComponentUI
			Return New SynthTableHeaderUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			prevRenderer = header.defaultRenderer
			If TypeOf prevRenderer Is UIResource Then header.defaultRenderer = New HeaderRenderer(Me)
			updateStyle(header)
		End Sub

		Private Sub updateStyle(ByVal c As JTableHeader)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			header.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			If TypeOf header.defaultRenderer Is HeaderRenderer Then header.defaultRenderer = prevRenderer

			Dim ___context As SynthContext = getContext(header, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			header.removePropertyChangeListener(Me)
			MyBase.uninstallListeners()
		End Sub

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintTableHeaderBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			MyBase.paint(g, context.component)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintTableHeaderBorder(context, g, x, y, w, h)
		End Sub
	'
	' SynthUI
	'
		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub rolloverColumnUpdated(ByVal oldColumn As Integer, ByVal newColumn As Integer)
			header.repaint(header.getHeaderRect(oldColumn))
			header.repaint(header.getHeaderRect(newColumn))
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JTableHeader))
		End Sub

		Private Class HeaderRenderer
			Inherits DefaultTableCellHeaderRenderer

			Private ReadOnly outerInstance As SynthTableHeaderUI

			Friend Sub New(ByVal outerInstance As SynthTableHeaderUI)
					Me.outerInstance = outerInstance
				horizontalAlignment = JLabel.LEADING
				name = "TableHeader.renderer"
			End Sub

			Public Overrides Function getTableCellRendererComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As Component

				Dim hasRollover As Boolean = (column = outerInstance.rolloverColumn)
				If isSelected OrElse hasRollover OrElse hasFocus Then
					SynthLookAndFeel.selectedUIdUI(CType(SynthLookAndFeel.getUIOfType(uI, GetType(SynthLabelUI)), SynthLabelUI), isSelected, hasFocus, table.enabled, hasRollover)
				Else
					SynthLookAndFeel.resetSelectedUI()
				End If

				'stuff a variable into the client property of this renderer indicating the sort order,
				'so that different rendering can be done for the header based on sorted state.
				Dim rs As RowSorter = If(table Is Nothing, Nothing, table.rowSorter)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim sortKeys As IList(Of ? As RowSorter.SortKey) = If(rs Is Nothing, Nothing, rs.sortKeys)
				If sortKeys IsNot Nothing AndAlso sortKeys.Count > 0 AndAlso sortKeys(0).column = table.convertColumnIndexToModel(column) Then
					Select Case sortKeys(0).sortOrder
						Case ASCENDING
							putClientProperty("Table.sortOrder", "ASCENDING")
						Case DESCENDING
							putClientProperty("Table.sortOrder", "DESCENDING")
						Case UNSORTED
							putClientProperty("Table.sortOrder", "UNSORTED")
						Case Else
							Throw New AssertionError("Cannot happen")
					End Select
				Else
					putClientProperty("Table.sortOrder", "UNSORTED")
				End If

				MyBase.getTableCellRendererComponent(table, value, isSelected, hasFocus, row, column)

				Return Me
			End Function

			Public Overrides Property border As Border
				Set(ByVal border As Border)
					If TypeOf border Is SynthBorder Then MyBase.border = border
				End Set
			End Property
		End Class
	End Class

End Namespace
Imports System
Imports javax.swing
Imports javax.swing.event
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

Namespace javax.swing




	''' <summary>
	''' Renders an item in a list.
	''' <p>
	''' <strong><a name="override">Implementation Note:</a></strong>
	''' This class overrides
	''' <code>invalidate</code>,
	''' <code>validate</code>,
	''' <code>revalidate</code>,
	''' <code>repaint</code>,
	''' <code>isOpaque</code>,
	''' and
	''' <code>firePropertyChange</code>
	''' solely to improve performance.
	''' If not overridden, these frequently called methods would execute code paths
	''' that are unnecessary for the default list cell renderer.
	''' If you write your own renderer,
	''' take care to weigh the benefits and
	''' drawbacks of overriding these methods.
	''' 
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
	''' @author Philip Milne
	''' @author Hans Muller
	''' </summary>
	<Serializable> _
	Public Class DefaultListCellRenderer
		Inherits JLabel
		Implements ListCellRenderer(Of Object)

	   ''' <summary>
	   ''' An empty <code>Border</code>. This field might not be used. To change the
	   ''' <code>Border</code> used by this renderer override the
	   ''' <code>getListCellRendererComponent</code> method and set the border
	   ''' of the returned component directly.
	   ''' </summary>
		Private Shared ReadOnly SAFE_NO_FOCUS_BORDER As Border = New EmptyBorder(1, 1, 1, 1)
		Private Shared ReadOnly DEFAULT_NO_FOCUS_BORDER As Border = New EmptyBorder(1, 1, 1, 1)
		Protected Friend Shared noFocusBorder As Border = DEFAULT_NO_FOCUS_BORDER

		''' <summary>
		''' Constructs a default renderer object for an item
		''' in a list.
		''' </summary>
		Public Sub New()
			MyBase.New()
			opaque = True
			border = noFocusBorder
			name = "List.cellRenderer"
		End Sub

		Private Property noFocusBorder As Border
			Get
				Dim ___border As Border = sun.swing.DefaultLookup.getBorder(Me, ui, "List.cellNoFocusBorder")
				If System.securityManager IsNot Nothing Then
					If ___border IsNot Nothing Then Return ___border
					Return SAFE_NO_FOCUS_BORDER
				Else
					If ___border IsNot Nothing AndAlso (noFocusBorder Is Nothing OrElse noFocusBorder Is DEFAULT_NO_FOCUS_BORDER) Then Return ___border
					Return noFocusBorder
				End If
			End Get
		End Property

		Public Overridable Function getListCellRendererComponent(Of T1)(ByVal list As JList(Of T1), ByVal value As Object, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As java.awt.Component
			componentOrientation = list.componentOrientation

			Dim bg As java.awt.Color = Nothing
			Dim fg As java.awt.Color = Nothing

			Dim ___dropLocation As JList.DropLocation = list.dropLocation
			If ___dropLocation IsNot Nothing AndAlso (Not ___dropLocation.insert) AndAlso ___dropLocation.index = index Then

				bg = sun.swing.DefaultLookup.getColor(Me, ui, "List.dropCellBackground")
				fg = sun.swing.DefaultLookup.getColor(Me, ui, "List.dropCellForeground")

				isSelected = True
			End If

			If isSelected Then
				background = If(bg Is Nothing, list.selectionBackground, bg)
				foreground = If(fg Is Nothing, list.selectionForeground, fg)
			Else
				background = list.background
				foreground = list.foreground
			End If

			If TypeOf value Is Icon Then
				icon = CType(value, Icon)
				text = ""
			Else
				icon = Nothing
				text = If(value Is Nothing, "", value.ToString())
			End If

			enabled = list.enabled
			font = list.font

			Dim ___border As Border = Nothing
			If cellHasFocus Then
				If isSelected Then ___border = sun.swing.DefaultLookup.getBorder(Me, ui, "List.focusSelectedCellHighlightBorder")
				If ___border Is Nothing Then ___border = sun.swing.DefaultLookup.getBorder(Me, ui, "List.focusCellHighlightBorder")
			Else
				___border = noFocusBorder
			End If
			border = ___border

			Return Me
		End Function

		''' <summary>
		''' Overridden for performance reasons.
		''' See the <a href="#override">Implementation Note</a>
		''' for more information.
		''' 
		''' @since 1.5 </summary>
		''' <returns> <code>true</code> if the background is completely opaque
		'''         and differs from the JList's background;
		'''         <code>false</code> otherwise </returns>
		Public Property Overrides opaque As Boolean
			Get
				Dim back As java.awt.Color = background
				Dim p As java.awt.Component = parent
				If p IsNot Nothing Then p = p.parent
				' p should now be the JList.
				Dim colorMatch As Boolean = (back IsNot Nothing) AndAlso (p IsNot Nothing) AndAlso back.Equals(p.background) AndAlso p.opaque
				Return (Not colorMatch) AndAlso MyBase.opaque
			End Get
		End Property

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub validate()
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' 
	   ''' @since 1.5
	   ''' </summary>
		End Sub
		Public Overrides Sub invalidate()
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' 
	   ''' @since 1.5
	   ''' </summary>
		End Sub
		Public Overrides Sub repaint()
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub revalidate()
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub repaint(ByVal tm As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub repaint(ByVal r As java.awt.Rectangle)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Protected Friend Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			' Strings get interned...
			If propertyName = "text" OrElse ((propertyName = "font" OrElse propertyName = "foreground") AndAlso oldValue IsNot newValue AndAlso getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey) IsNot Nothing) Then MyBase.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As SByte, ByVal newValue As SByte)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Char, ByVal newValue As Char)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Short, ByVal newValue As Short)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Integer, ByVal newValue As Integer)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Long, ByVal newValue As Long)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Single, ByVal newValue As Single)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Double, ByVal newValue As Double)
	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		End Sub
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Boolean, ByVal newValue As Boolean)
		End Sub

		''' <summary>
		''' A subclass of DefaultListCellRenderer that implements UIResource.
		''' DefaultListCellRenderer doesn't implement UIResource
		''' directly so that applications can safely override the
		''' cellRenderer property with DefaultListCellRenderer subclasses.
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
			Inherits DefaultListCellRenderer
			Implements javax.swing.plaf.UIResource

		End Class
	End Class

End Namespace
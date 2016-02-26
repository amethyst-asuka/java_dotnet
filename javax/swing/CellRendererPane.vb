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
	''' This class is inserted in between cell renderers and the components that
	''' use them.  It just exists to thwart the repaint() and invalidate() methods
	''' which would otherwise propagate up the tree when the renderer was configured.
	''' It's used by the implementations of JTable, JTree, and JList.  For example,
	''' here's how CellRendererPane is used in the code the paints each row
	''' in a JList:
	''' <pre>
	'''   cellRendererPane = new CellRendererPane();
	'''   ...
	'''   Component rendererComponent = renderer.getListCellRendererComponent();
	'''   renderer.configureListCellRenderer(dataModel.getElementAt(row), row);
	'''   cellRendererPane.paintComponent(g, rendererComponent, this, x, y, w, h);
	''' </pre>
	''' <p>
	''' A renderer component must override isShowing() and unconditionally return
	''' true to work correctly because the Swing paint does nothing for components
	''' with isShowing false.
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
	''' @author Hans Muller
	''' </summary>
	Public Class CellRendererPane
		Inherits Container
		Implements Accessible

		''' <summary>
		''' Construct a CellRendererPane object.
		''' </summary>
		Public Sub New()
			MyBase.New()
			layout = Nothing
			visible = False
		End Sub

		''' <summary>
		''' Overridden to avoid propagating a invalidate up the tree when the
		''' cell renderer child is configured.
		''' </summary>
		Public Overridable Sub invalidate()
		End Sub


		''' <summary>
		''' Shouldn't be called.
		''' </summary>
		Public Overridable Sub paint(ByVal g As Graphics)
		End Sub


		''' <summary>
		''' Shouldn't be called.
		''' </summary>
		Public Overridable Sub update(ByVal g As Graphics)
		End Sub


		''' <summary>
		''' If the specified component is already a child of this then we don't
		''' bother doing anything - stacking order doesn't matter for cell
		''' renderer components (CellRendererPane doesn't paint anyway).
		''' </summary>
		Protected Friend Overridable Sub addImpl(ByVal x As Component, ByVal constraints As Object, ByVal index As Integer)
			If x.parent Is Me Then
				Return
			Else
				MyBase.addImpl(x, constraints, index)
			End If
		End Sub


		''' <summary>
		''' Paint a cell renderer component c on graphics object g.  Before the component
		''' is drawn it's reparented to this (if that's necessary), it's bounds
		''' are set to w,h and the graphics object is (effectively) translated to x,y.
		''' If it's a JComponent, double buffering is temporarily turned off. After
		''' the component is painted it's bounds are reset to -w, -h, 0, 0 so that, if
		''' it's the last renderer component painted, it will not start consuming input.
		''' The Container p is the component we're actually drawing on, typically it's
		''' equal to this.getParent(). If shouldValidate is true the component c will be
		''' validated before painted.
		''' </summary>
		Public Overridable Sub paintComponent(ByVal g As Graphics, ByVal c As Component, ByVal p As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal shouldValidate As Boolean)
			If c Is Nothing Then
				If p IsNot Nothing Then
					Dim oldColor As Color = g.color
					g.color = p.background
					g.fillRect(x, y, w, h)
					g.color = oldColor
				End If
				Return
			End If

			If c.parent IsNot Me Then Me.add(c)

			c.boundsnds(x, y, w, h)

			If shouldValidate Then c.validate()

			Dim wasDoubleBuffered As Boolean = False
			If (TypeOf c Is JComponent) AndAlso CType(c, JComponent).doubleBuffered Then
				wasDoubleBuffered = True
				CType(c, JComponent).doubleBuffered = False
			End If

			Dim cg As Graphics = g.create(x, y, w, h)
			Try
				c.paint(cg)
			Finally
				cg.Dispose()
			End Try

			If wasDoubleBuffered AndAlso (TypeOf c Is JComponent) Then CType(c, JComponent).doubleBuffered = True

			c.boundsnds(-w, -h, 0, 0)
		End Sub


		''' <summary>
		''' Calls this.paintComponent(g, c, p, x, y, w, h, false).
		''' </summary>
		Public Overridable Sub paintComponent(ByVal g As Graphics, ByVal c As Component, ByVal p As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintComponent(g, c, p, x, y, w, h, False)
		End Sub


		''' <summary>
		''' Calls this.paintComponent() with the rectangles x,y,width,height fields.
		''' </summary>
		Public Overridable Sub paintComponent(ByVal g As Graphics, ByVal c As Component, ByVal p As Container, ByVal r As Rectangle)
			paintComponent(g, c, p, r.x, r.y, r.width, r.height)
		End Sub


		Private Sub writeObject(ByVal s As ObjectOutputStream)
			removeAll()
			s.defaultWriteObject()
		End Sub


	'///////////////
	' Accessibility support
	'//////////////

		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this CellRendererPane.
		''' For CellRendererPanes, the AccessibleContext takes the form of an
		''' AccessibleCellRendererPane.
		''' A new AccessibleCellRendererPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleCellRendererPane that serves as the
		'''         AccessibleContext of this CellRendererPane </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleCellRendererPane(Me)
				Return ___accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>CellRendererPane</code> class.
		''' </summary>
		Protected Friend Class AccessibleCellRendererPane
			Inherits AccessibleAWTContainer

			Private ReadOnly outerInstance As CellRendererPane

			Public Sub New(ByVal outerInstance As CellRendererPane)
				Me.outerInstance = outerInstance
			End Sub

			' AccessibleContext methods
			'
			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PANEL
				End Get
			End Property
		End Class ' inner class AccessibleCellRendererPane
	End Class

End Namespace
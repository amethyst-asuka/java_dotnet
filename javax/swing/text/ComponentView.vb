Imports javax.swing.event

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
Namespace javax.swing.text

	''' <summary>
	''' Component decorator that implements the view interface.  The
	''' entire element is used to represent the component.  This acts
	''' as a gateway from the display-only View implementations to
	''' interactive lightweight components (ie it allows components
	''' to be embedded into the View hierarchy).
	''' <p>
	''' The component is placed relative to the text baseline
	''' according to the value returned by
	''' <code>Component.getAlignmentY</code>.  For Swing components
	''' this value can be conveniently set using the method
	''' <code>JComponent.setAlignmentY</code>.  For example, setting
	''' a value of <code>0.75</code> will cause 75 percent of the
	''' component to be above the baseline, and 25 percent of the
	''' component to be below the baseline.
	''' <p>
	''' This class is implemented to do the extra work necessary to
	''' work properly in the presence of multiple threads (i.e. from
	''' asynchronous notification of model changes for example) by
	''' ensuring that all component access is done on the event thread.
	''' <p>
	''' The component used is determined by the return value of the
	''' createComponent method.  The default implementation of this
	''' method is to return the component held as an attribute of
	''' the element (by calling StyleConstants.getComponent).  A
	''' limitation of this behavior is that the component cannot
	''' be used by more than one text component (i.e. with a shared
	''' model).  Subclasses can remove this constraint by implementing
	''' the createComponent to actually create a component based upon
	''' some kind of specification contained in the attributes.  The
	''' ObjectView class in the html package is an example of a
	''' ComponentView implementation that supports multiple component
	''' views of a shared model.
	''' 
	''' @author Timothy Prinzing
	''' </summary>
	Public Class ComponentView
		Inherits View

		''' <summary>
		''' Creates a new ComponentView object.
		''' </summary>
		''' <param name="elem"> the element to decorate </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Create the component that is associated with
		''' this view.  This will be called when it has
		''' been determined that a new component is needed.
		''' This would result from a call to setParent or
		''' as a result of being notified that attributes
		''' have changed.
		''' </summary>
		Protected Friend Overridable Function createComponent() As Component
			Dim attr As AttributeSet = element.attributes
			Dim comp As Component = StyleConstants.getComponent(attr)
			Return comp
		End Function

		''' <summary>
		''' Fetch the component associated with the view.
		''' </summary>
		Public Property component As Component
			Get
				Return createdC
			End Get
		End Property

		' --- View methods ---------------------------------------------

		''' <summary>
		''' The real paint behavior occurs naturally from the association
		''' that the component has with its parent container (the same
		''' container hosting this view).  This is implemented to do nothing.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="a"> the shape </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			If c IsNot Nothing Then
				Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
				c.boundsnds(alloc.x, alloc.y, alloc.width, alloc.height)
			End If
		End Sub

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.  This is implemented to return the value
		''' returned by Component.getPreferredSize along the
		''' axis of interest.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;=0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			If (axis <> X_AXIS) AndAlso (axis <> Y_AXIS) Then Throw New System.ArgumentException("Invalid axis: " & axis)
			If c IsNot Nothing Then
				Dim ___size As Dimension = c.preferredSize
				If axis = View.X_AXIS Then
					Return ___size.width
				Else
					Return ___size.height
				End If
			End If
			Return 0
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.  This is implemented to return the value
		''' returned by Component.getMinimumSize along the
		''' axis of interest.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;=0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			If (axis <> X_AXIS) AndAlso (axis <> Y_AXIS) Then Throw New System.ArgumentException("Invalid axis: " & axis)
			If c IsNot Nothing Then
				Dim ___size As Dimension = c.minimumSize
				If axis = View.X_AXIS Then
					Return ___size.width
				Else
					Return ___size.height
				End If
			End If
			Return 0
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.  This is implemented to return the value
		''' returned by Component.getMaximumSize along the
		''' axis of interest.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;=0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			If (axis <> X_AXIS) AndAlso (axis <> Y_AXIS) Then Throw New System.ArgumentException("Invalid axis: " & axis)
			If c IsNot Nothing Then
				Dim ___size As Dimension = c.maximumSize
				If axis = View.X_AXIS Then
					Return ___size.width
				Else
					Return ___size.height
				End If
			End If
			Return 0
		End Function

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  This is implemented to give the alignment of the
		''' embedded component.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns> the desired alignment.  This should be a value
		'''   between 0.0 and 1.0 where 0 indicates alignment at the
		'''   origin and 1.0 indicates alignment to the full span
		'''   away from the origin.  An alignment of 0.5 would be the
		'''   center of the view. </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			If c IsNot Nothing Then
				Select Case axis
				Case View.X_AXIS
					Return c.alignmentX
				Case View.Y_AXIS
					Return c.alignmentY
				End Select
			End If
			Return MyBase.getAlignment(axis)
		End Function

		''' <summary>
		''' Sets the parent for a child view.
		''' The parent calls this on the child to tell it who its
		''' parent is, giving the view access to things like
		''' the hosting Container.  The superclass behavior is
		''' executed, followed by a call to createComponent if
		''' the parent view parameter is non-null and a component
		''' has not yet been created. The embedded components parent
		''' is then set to the value returned by <code>getContainer</code>.
		''' If the parent view parameter is null, this view is being
		''' cleaned up, thus the component is removed from its parent.
		''' <p>
		''' The changing of the component hierarchy will
		''' touch the component lock, which is the one thing
		''' that is not safe from the View hierarchy.  Therefore,
		''' this functionality is executed immediately if on the
		''' event thread, or is queued on the event queue if
		''' called from another thread (notification of change
		''' from an asynchronous update).
		''' </summary>
		''' <param name="p"> the parent </param>
		Public Overrides Property parent As View
			Set(ByVal p As View)
				MyBase.parent = p
				If javax.swing.SwingUtilities.eventDispatchThread Then
					componentParentent()
				Else
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'				Runnable callSetComponentParent = New Runnable()
		'			{
		'				public void run()
		'				{
		'					Document doc = getDocument();
		'					try
		'					{
		'						if (doc instanceof AbstractDocument)
		'						{
		'							((AbstractDocument)doc).readLock();
		'						}
		'						setComponentParent();
		'						Container host = getContainer();
		'						if (host != Nothing)
		'						{
		'							preferenceChanged(Nothing, True, True);
		'							host.repaint();
		'						}
		'					}
		'					finally
		'					{
		'						if (doc instanceof AbstractDocument)
		'						{
		'							((AbstractDocument)doc).readUnlock();
		'						}
		'					}
		'				}
		'			};
					javax.swing.SwingUtilities.invokeLater(callSetComponentParent)
				End If
			End Set
		End Property

		''' <summary>
		''' Set the parent of the embedded component
		''' with assurance that it is thread-safe.
		''' </summary>
		Friend Overridable Sub setComponentParent()
			Dim p As View = parent
			If p IsNot Nothing Then
				Dim ___parent As Container = container
				If ___parent IsNot Nothing Then
					If c Is Nothing Then
						' try to build a component
						Dim comp As Component = createComponent()
						If comp IsNot Nothing Then
							createdC = comp
							c = New Invalidator(Me, comp)
						End If
					End If
					If c IsNot Nothing Then
						If c.parent Is Nothing Then
							' components associated with the View tree are added
							' to the hosting container with the View as a constraint.
							___parent.add(c, Me)
							___parent.addPropertyChangeListener("enabled", c)
						End If
					End If
				End If
			Else
				If c IsNot Nothing Then
					Dim ___parent As Container = c.parent
					If ___parent IsNot Nothing Then
						' remove the component from its hosting container
						___parent.remove(c)
						___parent.removePropertyChangeListener("enabled", c)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Provides a mapping from the coordinate space of the model to
		''' that of the view.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;=0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position is returned </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			If (pos >= p0) AndAlso (pos <= p1) Then
				Dim r As Rectangle = a.bounds
				If pos = p1 Then r.x += r.width
				r.width = 0
				Return r
			End If
			Throw New BadLocationException(pos & " not in range " & p0 & "," & p1, pos)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;=0 </param>
		''' <param name="y"> the Y coordinate &gt;=0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents
		'''    the given point in the view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			Dim alloc As Rectangle = CType(a, Rectangle)
			If x < alloc.x + (alloc.width / 2) Then
				bias(0) = Position.Bias.Forward
				Return startOffset
			End If
			bias(0) = Position.Bias.Backward
			Return endOffset
		End Function

		' --- member variables ------------------------------------------------

		Private createdC As Component
		Private c As Invalidator

		''' <summary>
		''' This class feeds the invalidate back to the
		''' hosting View.  This is needed to get the View
		''' hierarchy to consider giving the component
		''' a different size (i.e. layout may have been
		''' cached between the associated view and the
		''' container hosting this component).
		''' </summary>
		Friend Class Invalidator
			Inherits Container
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As ComponentView


			' NOTE: When we remove this class we are going to have to some
			' how enforce setting of the focus traversal keys on the children
			' so that they don't inherit them from the JEditorPane. We need
			' to do this as JEditorPane has abnormal bindings (it is a focus cycle
			' root) and the children typically don't want these bindings as well.

			Friend Sub New(ByVal outerInstance As ComponentView, ByVal child As Component)
					Me.outerInstance = outerInstance
				layout = Nothing
				add(child)
				cacheChildSizes()
			End Sub

			''' <summary>
			''' The components invalid layout needs
			''' to be propagated through the view hierarchy
			''' so the views (which position the component)
			''' can have their layout recomputed.
			''' </summary>
			Public Overridable Sub invalidate()
				MyBase.invalidate()
				If outerInstance.parent IsNot Nothing Then outerInstance.preferenceChanged(Nothing, True, True)
			End Sub

			Public Overridable Sub doLayout()
				cacheChildSizes()
			End Sub

			Public Overridable Sub setBounds(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				MyBase.boundsnds(x, y, w, h)
				If componentCount > 0 Then getComponent(0).sizeize(w, h)
				cacheChildSizes()
			End Sub

			Public Overridable Sub validateIfNecessary()
				If Not valid Then validate()
			End Sub

			Private Sub cacheChildSizes()
				If componentCount > 0 Then
					Dim child As Component = getComponent(0)
					min = child.minimumSize
					pref = child.preferredSize
					max = child.maximumSize
					yalign = child.alignmentY
					xalign = child.alignmentX
				Else
						max = New Dimension(0, 0)
							pref = max
							min = pref
				End If
			End Sub

			''' <summary>
			''' Shows or hides this component depending on the value of parameter
			''' <code>b</code>. </summary>
			''' <param name="b"> If <code>true</code>, shows this component;
			''' otherwise, hides this component. </param>
			''' <seealso cref= #isVisible
			''' @since JDK1.1 </seealso>
			Public Overridable Property visible As Boolean
				Set(ByVal b As Boolean)
					MyBase.visible = b
					If componentCount > 0 Then getComponent(0).visible = b
				End Set
			End Property

			''' <summary>
			''' Overridden to fix 4759054. Must return true so that content
			''' is painted when inside a CellRendererPane which is normally
			''' invisible.
			''' </summary>
			Public Overridable Property showing As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overridable Property minimumSize As Dimension
				Get
					validateIfNecessary()
					Return min
				End Get
			End Property

			Public Overridable Property preferredSize As Dimension
				Get
					validateIfNecessary()
					Return pref
				End Get
			End Property

			Public Overridable Property maximumSize As Dimension
				Get
					validateIfNecessary()
					Return max
				End Get
			End Property

			Public Overridable Property alignmentX As Single
				Get
					validateIfNecessary()
					Return xalign
				End Get
			End Property

			Public Overridable Property alignmentY As Single
				Get
					validateIfNecessary()
					Return yalign
				End Get
			End Property

			Public Overridable Function getFocusTraversalKeys(ByVal id As Integer) As java.util.Set(Of AWTKeyStroke)
				Return KeyboardFocusManager.currentKeyboardFocusManager.getDefaultFocusTraversalKeys(id)
			End Function

			Public Overridable Sub propertyChange(ByVal ev As java.beans.PropertyChangeEvent)
				Dim enable As Boolean? = CBool(ev.newValue)
				If componentCount > 0 Then getComponent(0).enabled = enable
			End Sub

			Friend min As Dimension
			Friend pref As Dimension
			Friend max As Dimension
			Friend yalign As Single
			Friend xalign As Single

		End Class

	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System.Collections.Generic
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
	''' <code>JLayeredPane</code> adds depth to a JFC/Swing container,
	''' allowing components to overlap each other when needed.
	''' An <code>Integer</code> object specifies each component's depth in the
	''' container, where higher-numbered components sit &quot;on top&quot; of other
	''' components.
	''' For task-oriented documentation and examples of using layered panes see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/layeredpane.html">How to Use a Layered Pane</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <TABLE STYLE="FLOAT:RIGHT" BORDER="0" SUMMARY="layout">
	''' <TR>
	'''   <TD ALIGN="CENTER">
	'''     <P STYLE="TEXT-ALIGN:CENTER"><IMG SRC="doc-files/JLayeredPane-1.gif"
	'''     alt="The following text describes this image."
	'''     WIDTH="269" HEIGHT="264" STYLE="FLOAT:BOTTOM; BORDER=0">
	'''   </TD>
	''' </TR>
	''' </TABLE>
	''' For convenience, <code>JLayeredPane</code> divides the depth-range
	''' into several different layers. Putting a component into one of those
	''' layers makes it easy to ensure that components overlap properly,
	''' without having to worry about specifying numbers for specific depths:
	''' <DL>
	'''    <DT><FONT SIZE="2">DEFAULT_LAYER</FONT></DT>
	'''         <DD>The standard layer, where most components go. This the bottommost
	'''         layer.
	'''    <DT><FONT SIZE="2">PALETTE_LAYER</FONT></DT>
	'''         <DD>The palette layer sits over the default layer. Useful for floating
	'''         toolbars and palettes, so they can be positioned above other components.
	'''    <DT><FONT SIZE="2">MODAL_LAYER</FONT></DT>
	'''         <DD>The layer used for modal dialogs. They will appear on top of any
	'''         toolbars, palettes, or standard components in the container.
	'''    <DT><FONT SIZE="2">POPUP_LAYER</FONT></DT>
	'''         <DD>The popup layer displays above dialogs. That way, the popup windows
	'''         associated with combo boxes, tooltips, and other help text will appear
	'''         above the component, palette, or dialog that generated them.
	'''    <DT><FONT SIZE="2">DRAG_LAYER</FONT></DT>
	'''         <DD>When dragging a component, reassigning it to the drag layer ensures
	'''         that it is positioned over every other component in the container. When
	'''         finished dragging, it can be reassigned to its normal layer.
	''' </DL>
	''' The <code>JLayeredPane</code> methods <code>moveToFront(Component)</code>,
	''' <code>moveToBack(Component)</code> and <code>setPosition</code> can be used
	''' to reposition a component within its layer. The <code>setLayer</code> method
	''' can also be used to change the component's current layer.
	''' 
	''' <h2>Details</h2>
	''' <code>JLayeredPane</code> manages its list of children like
	''' <code>Container</code>, but allows for the definition of a several
	''' layers within itself. Children in the same layer are managed exactly
	''' like the normal <code>Container</code> object,
	''' with the added feature that when children components overlap, children
	''' in higher layers display above the children in lower layers.
	''' <p>
	''' Each layer is a distinct integer number. The layer attribute can be set
	''' on a <code>Component</code> by passing an <code>Integer</code>
	''' object during the add call.<br> For example:
	''' <PRE>
	'''     layeredPane.add(child, JLayeredPane.DEFAULT_LAYER);
	''' or
	'''     layeredPane.add(child, new Integer(10));
	''' </PRE>
	''' The layer attribute can also be set on a Component by calling<PRE>
	'''     layeredPaneParent.setLayer(child, 10)</PRE>
	''' on the <code>JLayeredPane</code> that is the parent of component. The layer
	''' should be set <i>before</i> adding the child to the parent.
	''' <p>
	''' Higher number layers display above lower number layers. So, using
	''' numbers for the layers and letters for individual components, a
	''' representative list order would look like this:<PRE>
	'''       5a, 5b, 5c, 2a, 2b, 2c, 1a </PRE>
	''' where the leftmost components are closest to the top of the display.
	''' <p>
	''' A component can be moved to the top or bottom position within its
	''' layer by calling <code>moveToFront</code> or <code>moveToBack</code>.
	''' <p>
	''' The position of a component within a layer can also be specified directly.
	''' Valid positions range from 0 up to one less than the number of
	''' components in that layer. A value of -1 indicates the bottommost
	''' position. A value of 0 indicates the topmost position. Unlike layer
	''' numbers, higher position values are <i>lower</i> in the display.
	''' <blockquote>
	''' <b>Note:</b> This sequence (defined by java.awt.Container) is the reverse
	''' of the layer numbering sequence. Usually though, you will use <code>moveToFront</code>,
	''' <code>moveToBack</code>, and <code>setLayer</code>.
	''' </blockquote>
	''' Here are some examples using the method add(Component, layer, position):
	''' Calling add(5x, 5, -1) results in:<PRE>
	'''       5a, 5b, 5c, 5x, 2a, 2b, 2c, 1a </PRE>
	''' 
	''' Calling add(5z, 5, 2) results in:<PRE>
	'''       5a, 5b, 5z, 5c, 5x, 2a, 2b, 2c, 1a </PRE>
	''' 
	''' Calling add(3a, 3, 7) results in:<PRE>
	'''       5a, 5b, 5z, 5c, 5x, 3a, 2a, 2b, 2c, 1a </PRE>
	''' 
	''' Using normal paint/event mechanics results in 1a appearing at the bottom
	''' and 5a being above all other components.
	''' <p>
	''' <b>Note:</b> that these layers are simply a logical construct and LayoutManagers
	''' will affect all child components of this container without regard for
	''' layer settings.
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
	''' 
	''' @author David Kloba
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JLayeredPane
		Inherits JComponent
		Implements Accessible

		'/ Watch the values in getObjectForLayer()
		''' <summary>
		''' Convenience object defining the Default layer. Equivalent to new Integer(0). </summary>
		Public Shared ReadOnly DEFAULT_LAYER As Integer? = New Integer?(0)
		''' <summary>
		''' Convenience object defining the Palette layer. Equivalent to new Integer(100). </summary>
		Public Shared ReadOnly PALETTE_LAYER As Integer? = New Integer?(100)
		''' <summary>
		''' Convenience object defining the Modal layer. Equivalent to new Integer(200). </summary>
		Public Shared ReadOnly MODAL_LAYER As Integer? = New Integer?(200)
		''' <summary>
		''' Convenience object defining the Popup layer. Equivalent to new Integer(300). </summary>
		Public Shared ReadOnly POPUP_LAYER As Integer? = New Integer?(300)
		''' <summary>
		''' Convenience object defining the Drag layer. Equivalent to new Integer(400). </summary>
		Public Shared ReadOnly DRAG_LAYER As Integer? = New Integer?(400)
		''' <summary>
		''' Convenience object defining the Frame Content layer.
		''' This layer is normally only use to position the contentPane and menuBar
		''' components of JFrame.
		''' Equivalent to new Integer(-30000). </summary>
		''' <seealso cref= JFrame </seealso>
		Public Shared ReadOnly FRAME_CONTENT_LAYER As Integer? = New Integer?(-30000)

		''' <summary>
		''' Bound property </summary>
		Public Const LAYER_PROPERTY As String = "layeredContainerLayer"
		' Hashtable to store layer values for non-JComponent components
		Private componentToLayer As Dictionary(Of java.awt.Component, Integer?)
		Private optimizedDrawingPossible As Boolean = True


	'////////////////////////////////////////////////////////////////////////////
	'// Container Override methods
	'////////////////////////////////////////////////////////////////////////////
		''' <summary>
		''' Create a new JLayeredPane </summary>
		Public Sub New()
			layout = Nothing
		End Sub

		Private Sub validateOptimizedDrawing()
			Dim layeredComponentFound As Boolean = False
			SyncLock treeLock
				Dim ___layer As Integer?

				For Each c As java.awt.Component In components
					___layer = Nothing

					___layer = CInt(Fix(CType(c, JComponent).getClientProperty(LAYER_PROPERTY)))
					If sun.awt.SunToolkit.isInstanceOf(c, "javax.swing.JInternalFrame") OrElse (TypeOf c Is JComponent AndAlso ___layer IsNot Nothing) Then
						If ___layer IsNot Nothing AndAlso ___layer.Equals(FRAME_CONTENT_LAYER) Then Continue For
						layeredComponentFound = True
						Exit For
					End If
				Next c
			End SyncLock

			If layeredComponentFound Then
				optimizedDrawingPossible = False
			Else
				optimizedDrawingPossible = True
			End If
		End Sub

		Protected Friend Overridable Sub addImpl(ByVal comp As java.awt.Component, ByVal constraints As Object, ByVal index As Integer)
			Dim ___layer As Integer
			Dim pos As Integer

			If TypeOf constraints Is Integer? Then
				___layer = CInt(Fix(constraints))
				layeryer(comp, ___layer)
			Else
				___layer = getLayer(comp)
			End If

			pos = insertIndexForLayer(___layer, index)
			MyBase.addImpl(comp, constraints, pos)
			comp.validate()
			comp.repaint()
			validateOptimizedDrawing()
		End Sub

		''' <summary>
		''' Remove the indexed component from this pane.
		''' This is the absolute index, ignoring layers.
		''' </summary>
		''' <param name="index">  an int specifying the component to remove </param>
		''' <seealso cref= #getIndexOf </seealso>
		Public Overridable Sub remove(ByVal index As Integer)
			Dim c As java.awt.Component = getComponent(index)
			MyBase.remove(index)
			If c IsNot Nothing AndAlso Not(TypeOf c Is JComponent) Then componentToLayer.Remove(c)
			validateOptimizedDrawing()
		End Sub

		''' <summary>
		''' Removes all the components from this container.
		''' 
		''' @since 1.5
		''' </summary>
		Public Overridable Sub removeAll()
			Dim children As java.awt.Component() = components
			Dim cToL As Dictionary(Of java.awt.Component, Integer?) = componentToLayer
			For counter As Integer = children.Length - 1 To 0 Step -1
				Dim c As java.awt.Component = children(counter)
				If c IsNot Nothing AndAlso Not(TypeOf c Is JComponent) Then cToL.Remove(c)
			Next counter
			MyBase.removeAll()
		End Sub

		''' <summary>
		''' Returns false if components in the pane can overlap, which makes
		''' optimized drawing impossible. Otherwise, returns true.
		''' </summary>
		''' <returns> false if components can overlap, else true </returns>
		''' <seealso cref= JComponent#isOptimizedDrawingEnabled </seealso>
		Public Property Overrides optimizedDrawingEnabled As Boolean
			Get
				Return optimizedDrawingPossible
			End Get
		End Property


	'////////////////////////////////////////////////////////////////////////////
	'// New methods for managing layers
	'////////////////////////////////////////////////////////////////////////////
		''' <summary>
		''' Sets the layer property on a JComponent. This method does not cause
		''' any side effects like setLayer() (painting, add/remove, etc).
		''' Normally you should use the instance method setLayer(), in order to
		''' get the desired side-effects (like repainting).
		''' </summary>
		''' <param name="c">      the JComponent to move </param>
		''' <param name="layer">  an int specifying the layer to move it to </param>
		''' <seealso cref= #setLayer </seealso>
		Public Shared Sub putLayer(ByVal c As JComponent, ByVal layer As Integer)
			'/ MAKE SURE THIS AND setLayer(Component c, int layer, int position)  are SYNCED
			Dim layerObj As Integer?

			layerObj = New Integer?(layer)
			c.putClientProperty(LAYER_PROPERTY, layerObj)
		End Sub

		''' <summary>
		''' Gets the layer property for a JComponent, it
		''' does not cause any side effects like setLayer(). (painting, add/remove, etc)
		''' Normally you should use the instance method getLayer().
		''' </summary>
		''' <param name="c">  the JComponent to check </param>
		''' <returns>   an int specifying the component's layer </returns>
		Public Shared Function getLayer(ByVal c As JComponent) As Integer
			Dim i As Integer?
			i = CInt(Fix(c.getClientProperty(LAYER_PROPERTY)))
			If i IsNot Nothing Then Return i
			Return DEFAULT_LAYER
		End Function

		''' <summary>
		''' Convenience method that returns the first JLayeredPane which
		''' contains the specified component. Note that all JFrames have a
		''' JLayeredPane at their root, so any component in a JFrame will
		''' have a JLayeredPane parent.
		''' </summary>
		''' <param name="c"> the Component to check </param>
		''' <returns> the JLayeredPane that contains the component, or
		'''         null if no JLayeredPane is found in the component
		'''         hierarchy </returns>
		''' <seealso cref= JFrame </seealso>
		''' <seealso cref= JRootPane </seealso>
		Public Shared Function getLayeredPaneAbove(ByVal c As java.awt.Component) As JLayeredPane
			If c Is Nothing Then Return Nothing

			Dim parent As java.awt.Component = c.parent
			Do While parent IsNot Nothing AndAlso Not(TypeOf parent Is JLayeredPane)
				parent = parent.parent
			Loop
			Return CType(parent, JLayeredPane)
		End Function

		''' <summary>
		''' Sets the layer attribute on the specified component,
		''' making it the bottommost component in that layer.
		''' Should be called before adding to parent.
		''' </summary>
		''' <param name="c">     the Component to set the layer for </param>
		''' <param name="layer"> an int specifying the layer to set, where
		'''              lower numbers are closer to the bottom </param>
		Public Overridable Sub setLayer(ByVal c As java.awt.Component, ByVal layer As Integer)
			layeryer(c, layer, -1)
		End Sub

		''' <summary>
		''' Sets the layer attribute for the specified component and
		''' also sets its position within that layer.
		''' </summary>
		''' <param name="c">         the Component to set the layer for </param>
		''' <param name="layer">     an int specifying the layer to set, where
		'''                  lower numbers are closer to the bottom </param>
		''' <param name="position">  an int specifying the position within the
		'''                  layer, where 0 is the topmost position and -1
		'''                  is the bottommost position </param>
		Public Overridable Sub setLayer(ByVal c As java.awt.Component, ByVal layer As Integer, ByVal position As Integer)
			Dim layerObj As Integer?
			layerObj = getObjectForLayer(layer)

			If layer = getLayer(c) AndAlso position = getPosition(c) Then
					repaint(c.bounds)
				Return
			End If

			'/ MAKE SURE THIS AND putLayer(JComponent c, int layer) are SYNCED
			If TypeOf c Is JComponent Then
				CType(c, JComponent).putClientProperty(LAYER_PROPERTY, layerObj)
			Else
				componentToLayer(c) = layerObj
			End If

			If c.parent Is Nothing OrElse c.parent IsNot Me Then
				repaint(c.bounds)
				Return
			End If

			Dim index As Integer = insertIndexForLayer(c, layer, position)

			componentZOrderder(c, index)
			repaint(c.bounds)
		End Sub

		''' <summary>
		''' Returns the layer attribute for the specified Component.
		''' </summary>
		''' <param name="c">  the Component to check </param>
		''' <returns> an int specifying the component's current layer </returns>
		Public Overridable Function getLayer(ByVal c As java.awt.Component) As Integer
			Dim i As Integer?
			If TypeOf c Is JComponent Then
				i = CInt(Fix(CType(c, JComponent).getClientProperty(LAYER_PROPERTY)))
			Else
				i = componentToLayer(c)
			End If

			If i Is Nothing Then Return DEFAULT_LAYER
			Return i
		End Function

		''' <summary>
		''' Returns the index of the specified Component.
		''' This is the absolute index, ignoring layers.
		''' Index numbers, like position numbers, have the topmost component
		''' at index zero. Larger numbers are closer to the bottom.
		''' </summary>
		''' <param name="c">  the Component to check </param>
		''' <returns> an int specifying the component's index </returns>
		Public Overridable Function getIndexOf(ByVal c As java.awt.Component) As Integer
			Dim i, count As Integer

			count = componentCount
			For i = 0 To count - 1
				If c Is getComponent(i) Then Return i
			Next i
			Return -1
		End Function
		''' <summary>
		''' Moves the component to the top of the components in its current layer
		''' (position 0).
		''' </summary>
		''' <param name="c"> the Component to move </param>
		''' <seealso cref= #setPosition(Component, int) </seealso>
		Public Overridable Sub moveToFront(ByVal c As java.awt.Component)
			positionion(c, 0)
		End Sub

		''' <summary>
		''' Moves the component to the bottom of the components in its current layer
		''' (position -1).
		''' </summary>
		''' <param name="c"> the Component to move </param>
		''' <seealso cref= #setPosition(Component, int) </seealso>
		Public Overridable Sub moveToBack(ByVal c As java.awt.Component)
			positionion(c, -1)
		End Sub

		''' <summary>
		''' Moves the component to <code>position</code> within its current layer,
		''' where 0 is the topmost position within the layer and -1 is the bottommost
		''' position.
		''' <p>
		''' <b>Note:</b> Position numbering is defined by java.awt.Container, and
		''' is the opposite of layer numbering. Lower position numbers are closer
		''' to the top (0 is topmost), and higher position numbers are closer to
		''' the bottom.
		''' </summary>
		''' <param name="c">         the Component to move </param>
		''' <param name="position">  an int in the range -1..N-1, where N is the number of
		'''                  components in the component's current layer </param>
		Public Overridable Sub setPosition(ByVal c As java.awt.Component, ByVal position As Integer)
			layeryer(c, getLayer(c), position)
		End Sub

		''' <summary>
		''' Get the relative position of the component within its layer.
		''' </summary>
		''' <param name="c">  the Component to check </param>
		''' <returns> an int giving the component's position, where 0 is the
		'''         topmost position and the highest index value = the count
		'''         count of components at that layer, minus 1
		''' </returns>
		''' <seealso cref= #getComponentCountInLayer </seealso>
		Public Overridable Function getPosition(ByVal c As java.awt.Component) As Integer
			Dim i As Integer, startLayer As Integer, curLayer As Integer, startLocation As Integer, pos As Integer = 0

			componentCount
			startLocation = getIndexOf(c)

			If startLocation = -1 Then Return -1

			startLayer = getLayer(c)
			For i = startLocation - 1 To 0 Step -1
				curLayer = getLayer(getComponent(i))
				If curLayer = startLayer Then
					pos += 1
				Else
					Return pos
				End If
			Next i
			Return pos
		End Function

		''' <summary>
		''' Returns the highest layer value from all current children.
		''' Returns 0 if there are no children.
		''' </summary>
		''' <returns> an int indicating the layer of the topmost component in the
		'''         pane, or zero if there are no children </returns>
		Public Overridable Function highestLayer() As Integer
			If componentCount > 0 Then Return getLayer(getComponent(0))
			Return 0
		End Function

		''' <summary>
		''' Returns the lowest layer value from all current children.
		''' Returns 0 if there are no children.
		''' </summary>
		''' <returns> an int indicating the layer of the bottommost component in the
		'''         pane, or zero if there are no children </returns>
		Public Overridable Function lowestLayer() As Integer
			Dim count As Integer = componentCount
			If count > 0 Then Return getLayer(getComponent(count-1))
			Return 0
		End Function

		''' <summary>
		''' Returns the number of children currently in the specified layer.
		''' </summary>
		''' <param name="layer">  an int specifying the layer to check </param>
		''' <returns> an int specifying the number of components in that layer </returns>
		Public Overridable Function getComponentCountInLayer(ByVal layer As Integer) As Integer
			Dim i, count, curLayer As Integer
			Dim layerCount As Integer = 0

			count = componentCount
			For i = 0 To count - 1
				curLayer = getLayer(getComponent(i))
				If curLayer = layer Then
					layerCount += 1
				'/ Short circut the counting when we have them all
				ElseIf layerCount > 0 OrElse curLayer < layer Then
					Exit For
				End If
			Next i

			Return layerCount
		End Function

		''' <summary>
		''' Returns an array of the components in the specified layer.
		''' </summary>
		''' <param name="layer">  an int specifying the layer to check </param>
		''' <returns> an array of Components contained in that layer </returns>
		Public Overridable Function getComponentsInLayer(ByVal layer As Integer) As java.awt.Component()
			Dim i, count, curLayer As Integer
			Dim layerCount As Integer = 0
			Dim results As java.awt.Component()

			results = New java.awt.Component(getComponentCountInLayer(layer) - 1){}
			count = componentCount
			For i = 0 To count - 1
				curLayer = getLayer(getComponent(i))
				If curLayer = layer Then
					results(layerCount) = getComponent(i)
					layerCount += 1
				'/ Short circut the counting when we have them all
				ElseIf layerCount > 0 OrElse curLayer < layer Then
					Exit For
				End If
			Next i

			Return results
		End Function

		''' <summary>
		''' Paints this JLayeredPane within the specified graphics context.
		''' </summary>
		''' <param name="g">  the Graphics context within which to paint </param>
		Public Overridable Sub paint(ByVal g As java.awt.Graphics)
			If opaque Then
				Dim r As java.awt.Rectangle = g.clipBounds
				Dim c As java.awt.Color = background
				If c Is Nothing Then c = java.awt.Color.lightGray
				g.color = c
				If r IsNot Nothing Then
					g.fillRect(r.x, r.y, r.width, r.height)
				Else
					g.fillRect(0, 0, width, height)
				End If
			End If
			MyBase.paint(g)
		End Sub

	'////////////////////////////////////////////////////////////////////////////
	'// Implementation Details
	'////////////////////////////////////////////////////////////////////////////

		''' <summary>
		''' Returns the hashtable that maps components to layers.
		''' </summary>
		''' <returns> the Hashtable used to map components to their layers </returns>
		Protected Friend Overridable Property componentToLayer As Dictionary(Of java.awt.Component, Integer?)
			Get
				If componentToLayer Is Nothing Then componentToLayer = New Dictionary(Of java.awt.Component, Integer?)(4)
				Return componentToLayer
			End Get
		End Property

		''' <summary>
		''' Returns the Integer object associated with a specified layer.
		''' </summary>
		''' <param name="layer"> an int specifying the layer </param>
		''' <returns> an Integer object for that layer </returns>
		Protected Friend Overridable Function getObjectForLayer(ByVal layer As Integer) As Integer?
			Dim layerObj As Integer?
			Select Case layer
			Case 0
				layerObj = DEFAULT_LAYER
			Case 100
				layerObj = PALETTE_LAYER
			Case 200
				layerObj = MODAL_LAYER
			Case 300
				layerObj = POPUP_LAYER
			Case 400
				layerObj = DRAG_LAYER
			Case Else
				layerObj = New Integer?(layer)
			End Select
			Return layerObj
		End Function

		''' <summary>
		''' Primitive method that determines the proper location to
		''' insert a new child based on layer and position requests.
		''' </summary>
		''' <param name="layer">     an int specifying the layer </param>
		''' <param name="position">  an int specifying the position within the layer </param>
		''' <returns> an int giving the (absolute) insertion-index
		''' </returns>
		''' <seealso cref= #getIndexOf </seealso>
		Protected Friend Overridable Function insertIndexForLayer(ByVal layer As Integer, ByVal position As Integer) As Integer
			Return insertIndexForLayer(Nothing, layer, position)
		End Function

		''' <summary>
		''' This method is an extended version of insertIndexForLayer()
		''' to support setLayer which uses Container.setZOrder which does
		''' not remove the component from the containment hierarchy though
		''' we need to ignore it when calculating the insertion index.
		''' </summary>
		''' <param name="comp">      component to ignore when determining index </param>
		''' <param name="layer">     an int specifying the layer </param>
		''' <param name="position">  an int specifying the position within the layer </param>
		''' <returns> an int giving the (absolute) insertion-index
		''' </returns>
		''' <seealso cref= #getIndexOf </seealso>
		Private Function insertIndexForLayer(ByVal comp As java.awt.Component, ByVal layer As Integer, ByVal position As Integer) As Integer
			Dim i, count, curLayer As Integer
			Dim layerStart As Integer = -1
			Dim layerEnd As Integer = -1
			Dim componentCount As Integer = componentCount

			Dim compList As New List(Of java.awt.Component)(componentCount)
			For index As Integer = 0 To componentCount - 1
				If getComponent(index) IsNot comp Then compList.Add(getComponent(index))
			Next index

			count = compList.Count
			For i = 0 To count - 1
				curLayer = getLayer(compList(i))
				If layerStart = -1 AndAlso curLayer = layer Then layerStart = i
				If curLayer < layer Then
					If i = 0 Then
						' layer is greater than any current layer
						' [ ASSERT(layer > highestLayer()) ]
						layerStart = 0
						layerEnd = 0
					Else
						layerEnd = i
					End If
					Exit For
				End If
			Next i

			' layer requested is lower than any current layer
			' [ ASSERT(layer < lowestLayer()) ]
			' put it on the bottom of the stack
			If layerStart = -1 AndAlso layerEnd = -1 Then Return count

			' In the case of a single layer entry handle the degenerative cases
			If layerStart <> -1 AndAlso layerEnd = -1 Then layerEnd = count

			If layerEnd <> -1 AndAlso layerStart = -1 Then layerStart = layerEnd

			' If we are adding to the bottom, return the last element
			If position = -1 Then Return layerEnd

			' Otherwise make sure the requested position falls in the
			' proper range
			If position > -1 AndAlso layerStart + position <= layerEnd Then Return layerStart + position

			' Otherwise return the end of the layer
			Return layerEnd
		End Function

		''' <summary>
		''' Returns a string representation of this JLayeredPane. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JLayeredPane. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim optimizedDrawingPossibleString As String = (If(optimizedDrawingPossible, "true", "false"))

			Return MyBase.paramString() & ",optimizedDrawingPossible=" & optimizedDrawingPossibleString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JLayeredPane.
		''' For layered panes, the AccessibleContext takes the form of an
		''' AccessibleJLayeredPane.
		''' A new AccessibleJLayeredPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJLayeredPane that serves as the
		'''         AccessibleContext of this JLayeredPane </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJLayeredPane(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JLayeredPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to layered pane user-interface
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJLayeredPane
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JLayeredPane

			Public Sub New(ByVal outerInstance As JLayeredPane)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.LAYERED_PANE
				End Get
			End Property
		End Class
	End Class

End Namespace
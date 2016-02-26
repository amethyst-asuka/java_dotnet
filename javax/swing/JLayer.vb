Imports System
Imports System.Collections.Generic
Imports javax.accessibility

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' {@code JLayer} is a universal decorator for Swing components
	''' which enables you to implement various advanced painting effects as well as
	''' receive notifications of all {@code AWTEvent}s generated within its borders.
	''' <p>
	''' {@code JLayer} delegates the handling of painting and input events to a
	''' <seealso cref="javax.swing.plaf.LayerUI"/> object, which performs the actual decoration.
	''' <p>
	''' The custom painting implemented in the {@code LayerUI} and events notification
	''' work for the JLayer itself and all its subcomponents.
	''' This combination enables you to enrich existing components
	''' by adding new advanced functionality such as temporary locking of a hierarchy,
	''' data tips for compound components, enhanced mouse scrolling etc and so on.
	''' <p>
	''' {@code JLayer} is a good solution if you only need to do custom painting
	''' over compound component or catch input events from its subcomponents.
	''' <pre>
	''' import javax.swing.*;
	''' import javax.swing.plaf.LayerUI;
	''' import java.awt.*;
	''' 
	''' public class JLayerSample {
	''' 
	'''     private static JLayer&lt;JComponent&gt; createLayer() {
	'''         // This custom layerUI will fill the layer with translucent green
	'''         // and print out all mouseMotion events generated within its borders
	'''         LayerUI&lt;JComponent&gt; layerUI = new LayerUI&lt;JComponent&gt;() {
	''' 
	'''             public void paint(Graphics g, JComponent c) {
	'''                 // paint the layer as is
	'''                 super.paint(g, c);
	'''                 // fill it with the translucent green
	'''                 g.setColor(new Color(0, 128, 0, 128));
	'''                 g.fillRect(0, 0, c.getWidth(), c.getHeight());
	'''             }
	''' 
	'''             public void installUI(JComponent c) {
	'''                 super.installUI(c);
	'''                 // enable mouse motion events for the layer's subcomponents
	'''                 ((JLayer) c).setLayerEventMask(AWTEvent.MOUSE_MOTION_EVENT_MASK);
	'''             }
	''' 
	'''             public void uninstallUI(JComponent c) {
	'''                 super.uninstallUI(c);
	'''                 // reset the layer event mask
	'''                 ((JLayer) c).setLayerEventMask(0);
	'''             }
	''' 
	'''             // overridden method which catches MouseMotion events
	'''             public void eventDispatched(AWTEvent e, JLayer&lt;? extends JComponent&gt; l) {
	'''                 System.out.println("AWTEvent detected: " + e);
	'''             }
	'''         };
	'''         // create a component to be decorated with the layer
	'''         JPanel panel = new JPanel();
	'''         panel.add(new JButton("JButton"));
	''' 
	'''         // create the layer for the panel using our custom layerUI
	'''         return new JLayer&lt;JComponent&gt;(panel, layerUI);
	'''     }
	''' 
	'''     private static void createAndShowGUI() {
	'''         final JFrame frame = new JFrame();
	'''         frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	''' 
	'''         // work with the layer as with any other Swing component
	'''         frame.add(createLayer());
	''' 
	'''         frame.setSize(200, 200);
	'''         frame.setLocationRelativeTo(null);
	'''         frame.setVisible(true);
	'''     }
	''' 
	'''     public static void main(String[] args) throws Exception {
	'''         SwingUtilities.invokeAndWait(new Runnable() {
	'''             public void run() {
	'''                 createAndShowGUI();
	'''             }
	'''         });
	'''     }
	''' }
	''' </pre>
	''' 
	''' <b>Note:</b> {@code JLayer} doesn't support the following methods:
	''' <ul>
	''' <li><seealso cref="Container#add(java.awt.Component)"/></li>
	''' <li><seealso cref="Container#add(String, java.awt.Component)"/></li>
	''' <li><seealso cref="Container#add(java.awt.Component, int)"/></li>
	''' <li><seealso cref="Container#add(java.awt.Component, Object)"/></li>
	''' <li><seealso cref="Container#add(java.awt.Component, Object, int)"/></li>
	''' </ul>
	''' using any of of them will cause {@code UnsupportedOperationException} to be thrown,
	''' to add a component to {@code JLayer}
	''' use <seealso cref="#setView(Component)"/> or <seealso cref="#setGlassPane(JPanel)"/>.
	''' </summary>
	''' @param <V> the type of {@code JLayer}'s view component
	''' </param>
	''' <seealso cref= #JLayer(Component) </seealso>
	''' <seealso cref= #setView(Component) </seealso>
	''' <seealso cref= #getView() </seealso>
	''' <seealso cref= javax.swing.plaf.LayerUI </seealso>
	''' <seealso cref= #JLayer(Component, LayerUI) </seealso>
	''' <seealso cref= #setUI(javax.swing.plaf.LayerUI) </seealso>
	''' <seealso cref= #getUI()
	''' @since 1.7
	''' 
	''' @author Alexander Potochkin </seealso>
	Public NotInheritable Class JLayer(Of V As Component)
		Inherits JComponent
		Implements Scrollable, java.beans.PropertyChangeListener, Accessible

		Private view As V
		' this field is necessary because JComponent.ui is transient
		' when layerUI is serializable
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private layerUI As javax.swing.plaf.LayerUI(Of ?)
		Private glassPane As JPanel
		Private eventMask As Long
		<NonSerialized> _
		Private isPainting As Boolean
		<NonSerialized> _
		Private isPaintingImmediately As Boolean

		Private Shared ReadOnly eventController As New LayerEventController

		''' <summary>
		''' Creates a new {@code JLayer} object with a {@code null} view component
		''' and default <seealso cref="javax.swing.plaf.LayerUI"/>.
		''' </summary>
		''' <seealso cref= #setView </seealso>
		''' <seealso cref= #setUI </seealso>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Creates a new {@code JLayer} object
		''' with default <seealso cref="javax.swing.plaf.LayerUI"/>.
		''' </summary>
		''' <param name="view"> the component to be decorated by this {@code JLayer}
		''' </param>
		''' <seealso cref= #setUI </seealso>
		Public Sub New(ByVal view As V)
			Me.New(view, New javax.swing.plaf.LayerUI(Of V))
		End Sub

		''' <summary>
		''' Creates a new {@code JLayer} object with the specified view component
		''' and <seealso cref="javax.swing.plaf.LayerUI"/> object.
		''' </summary>
		''' <param name="view"> the component to be decorated </param>
		''' <param name="ui"> the <seealso cref="javax.swing.plaf.LayerUI"/> delegate
		''' to be used by this {@code JLayer} </param>
		Public Sub New(ByVal view As V, ByVal ui As javax.swing.plaf.LayerUI(Of V))
			glassPane = createGlassPane()
			view = view
			uI = ui
		End Sub

		''' <summary>
		''' Returns the {@code JLayer}'s view component or {@code null}.
		''' <br>This is a bound property.
		''' </summary>
		''' <returns> the {@code JLayer}'s view component
		'''         or {@code null} if none exists
		''' </returns>
		''' <seealso cref= #setView(Component) </seealso>
		Public Property view As V
			Get
				Return view
			End Get
			Set(ByVal view As V)
				Dim oldView As Component = view
				If oldView IsNot Nothing Then MyBase.remove(oldView)
				If view IsNot Nothing Then MyBase.addImpl(view, Nothing, componentCount)
				Me.view = view
				firePropertyChange("view", oldView, view)
				revalidate()
				repaint()
			End Set
		End Property


		''' <summary>
		''' Sets the <seealso cref="javax.swing.plaf.LayerUI"/> which will perform painting
		''' and receive input events for this {@code JLayer}.
		''' </summary>
		''' <param name="ui"> the <seealso cref="javax.swing.plaf.LayerUI"/> for this {@code JLayer} </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Property uI(Of T1) As javax.swing.plaf.LayerUI(Of T1)
			Set(ByVal ui As javax.swing.plaf.LayerUI(Of T1))
				Me.layerUI = ui
				MyBase.uI = ui
			End Set
			Get
				Return layerUI
			End Get
		End Property

		''' <summary>
		''' Returns the <seealso cref="javax.swing.plaf.LayerUI"/> for this {@code JLayer}.
		''' </summary>
		''' <returns> the {@code LayerUI} for this {@code JLayer} </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

		''' <summary>
		''' Returns the {@code JLayer}'s glassPane component or {@code null}.
		''' <br>This is a bound property.
		''' </summary>
		''' <returns> the {@code JLayer}'s glassPane component
		'''         or {@code null} if none exists
		''' </returns>
		''' <seealso cref= #setGlassPane(JPanel) </seealso>
		Public Property glassPane As JPanel
			Get
				Return glassPane
			End Get
			Set(ByVal glassPane As JPanel)
				Dim oldGlassPane As Component = glassPane
				Dim isGlassPaneVisible As Boolean = False
				If oldGlassPane IsNot Nothing Then
					isGlassPaneVisible = oldGlassPane.visible
					MyBase.remove(oldGlassPane)
				End If
				If glassPane IsNot Nothing Then
					sun.awt.AWTAccessor.componentAccessor.mixingCutoutShapeape(glassPane, New Rectangle)
					glassPane.visible = isGlassPaneVisible
					MyBase.addImpl(glassPane, Nothing, 0)
				End If
				Me.glassPane = glassPane
				firePropertyChange("glassPane", oldGlassPane, glassPane)
				revalidate()
				repaint()
			End Set
		End Property


		''' <summary>
		''' Called by the constructor methods to create a default {@code glassPane}.
		''' By default this method creates a new JPanel with visibility set to true
		''' and opacity set to false.
		''' </summary>
		''' <returns> the default {@code glassPane} </returns>
		Public Function createGlassPane() As JPanel
			Return New DefaultLayerGlassPane
		End Function

		''' <summary>
		''' Sets the layout manager for this container.  This method is
		''' overridden to prevent the layout manager from being set.
		''' <p>Note:  If {@code mgr} is non-{@code null}, this
		''' method will throw an exception as layout managers are not supported on
		''' a {@code JLayer}.
		''' </summary>
		''' <param name="mgr"> the specified layout manager </param>
		''' <exception cref="IllegalArgumentException"> this method is not supported </exception>
		Public Property layout As LayoutManager
			Set(ByVal mgr As LayoutManager)
				If mgr IsNot Nothing Then Throw New System.ArgumentException("JLayer.setLayout() not supported")
			End Set
		End Property

		''' <summary>
		''' A non-{@code null} border, or non-zero insets, isn't supported, to prevent the geometry
		''' of this component from becoming complex enough to inhibit
		''' subclassing of {@code LayerUI} class.  To create a {@code JLayer} with a border,
		''' add it to a {@code JPanel} that has a border.
		''' <p>Note:  If {@code border} is non-{@code null}, this
		''' method will throw an exception as borders are not supported on
		''' a {@code JLayer}.
		''' </summary>
		''' <param name="border"> the {@code Border} to set </param>
		''' <exception cref="IllegalArgumentException"> this method is not supported </exception>
		Public Property border As javax.swing.border.Border
			Set(ByVal border As javax.swing.border.Border)
				If border IsNot Nothing Then Throw New System.ArgumentException("JLayer.setBorder() not supported")
			End Set
		End Property

		''' <summary>
		''' This method is not supported by {@code JLayer}
		''' and always throws {@code UnsupportedOperationException}
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> this method is not supported
		''' </exception>
		''' <seealso cref= #setView(Component) </seealso>
		''' <seealso cref= #setGlassPane(JPanel) </seealso>
		Protected Friend Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			Throw New System.NotSupportedException("Adding components to JLayer is not supported, " & "use setView() or setGlassPane() instead")
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Sub remove(ByVal comp As Component)
			If comp Is Nothing Then
				MyBase.remove(comp)
			ElseIf comp Is view Then
				view = Nothing
			ElseIf comp Is glassPane Then
				glassPane = Nothing
			Else
				MyBase.remove(comp)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Sub removeAll()
			If view IsNot Nothing Then view = Nothing
			If glassPane IsNot Nothing Then glassPane = Nothing
		End Sub

		''' <summary>
		''' Always returns {@code true} to cause painting to originate from {@code JLayer},
		''' or one of its ancestors.
		''' </summary>
		''' <returns> true </returns>
		''' <seealso cref= JComponent#isPaintingOrigin() </seealso>
		Protected Friend Property Overrides paintingOrigin As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Delegates its functionality to the
		''' <seealso cref="javax.swing.plaf.LayerUI#paintImmediately(int, int, int, int, JLayer)"/> method,
		''' if {@code LayerUI} is set.
		''' </summary>
		''' <param name="x">  the x value of the region to be painted </param>
		''' <param name="y">  the y value of the region to be painted </param>
		''' <param name="w">  the width of the region to be painted </param>
		''' <param name="h">  the height of the region to be painted </param>
		Public Overrides Sub paintImmediately(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If (Not isPaintingImmediately) AndAlso uI IsNot Nothing Then
				isPaintingImmediately = True
				Try
					uI.paintImmediately(x, y, w, h, Me)
				Finally
					isPaintingImmediately = False
				End Try
			Else
				MyBase.paintImmediately(x, y, w, h)
			End If
		End Sub

		''' <summary>
		''' Delegates all painting to the <seealso cref="javax.swing.plaf.LayerUI"/> object.
		''' </summary>
		''' <param name="g"> the {@code Graphics} to render to </param>
		Public Overrides Sub paint(ByVal g As Graphics)
			If Not isPainting Then
				isPainting = True
				Try
					MyBase.paintComponent(g)
				Finally
					isPainting = False
				End Try
			Else
				MyBase.paint(g)
			End If
		End Sub

		''' <summary>
		''' This method is empty, because all painting is done by
		''' <seealso cref="#paint(Graphics)"/> and
		''' <seealso cref="javax.swing.plaf.LayerUI#update(Graphics, JComponent)"/> methods
		''' </summary>
		Protected Friend Overrides Sub paintComponent(ByVal g As Graphics)
		End Sub

		''' <summary>
		''' The {@code JLayer} overrides the default implementation of
		''' this method (in {@code JComponent}) to return {@code false}.
		''' This ensures
		''' that the drawing machinery will call the {@code JLayer}'s
		''' {@code paint}
		''' implementation rather than messaging the {@code JLayer}'s
		''' children directly.
		''' </summary>
		''' <returns> false </returns>
		Public Property Overrides optimizedDrawingEnabled As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			If uI IsNot Nothing Then uI.applyPropertyChange(evt, Me)
		End Sub

		''' <summary>
		''' Enables the events from JLayer and <b>all its descendants</b>
		''' defined by the specified event mask parameter
		''' to be delivered to the
		''' <seealso cref="LayerUI#eventDispatched(AWTEvent, JLayer)"/> method.
		''' <p>
		''' Events are delivered provided that {@code LayerUI} is set
		''' for this {@code JLayer} and the {@code JLayer}
		''' is displayable.
		''' <p>
		''' The following example shows how to correctly use this method
		''' in the {@code LayerUI} implementations:
		''' <pre>
		'''    public void installUI(JComponent c) {
		'''       super.installUI(c);
		'''       JLayer l = (JLayer) c;
		'''       // this LayerUI will receive only key and focus events
		'''       l.setLayerEventMask(AWTEvent.KEY_EVENT_MASK | AWTEvent.FOCUS_EVENT_MASK);
		'''    }
		''' 
		'''    public void uninstallUI(JComponent c) {
		'''       super.uninstallUI(c);
		'''       JLayer l = (JLayer) c;
		'''       // JLayer must be returned to its initial state
		'''       l.setLayerEventMask(0);
		'''    }
		''' </pre>
		''' 
		''' By default {@code JLayer} receives no events and its event mask is {@code 0}.
		''' </summary>
		''' <param name="layerEventMask"> the bitmask of event types to receive
		''' </param>
		''' <seealso cref= #getLayerEventMask() </seealso>
		''' <seealso cref= LayerUI#eventDispatched(AWTEvent, JLayer) </seealso>
		''' <seealso cref= Component#isDisplayable() </seealso>
		Public Property layerEventMask As Long
			Set(ByVal layerEventMask As Long)
				Dim oldEventMask As Long = layerEventMask
				Me.eventMask = layerEventMask
				firePropertyChange("layerEventMask", oldEventMask, layerEventMask)
				If layerEventMask <> oldEventMask Then
					disableEvents(oldEventMask)
					enableEvents(eventMask)
					If displayable Then eventController.updateAWTEventListener(oldEventMask, layerEventMask)
				End If
			End Set
			Get
				Return eventMask
			End Get
		End Property


		''' <summary>
		''' Delegates its functionality to the <seealso cref="javax.swing.plaf.LayerUI#updateUI(JLayer)"/> method,
		''' if {@code LayerUI} is set.
		''' </summary>
		Public Overrides Sub updateUI()
			If uI IsNot Nothing Then uI.updateUI(Me)
		End Sub

		''' <summary>
		''' Returns the preferred size of the viewport for a view component.
		''' <p>
		''' If the view component of this layer implements <seealso cref="Scrollable"/>, this method delegates its
		''' implementation to the view component.
		''' </summary>
		''' <returns> the preferred size of the viewport for a view component
		''' </returns>
		''' <seealso cref= Scrollable </seealso>
		Public Property preferredScrollableViewportSize As Dimension
			Get
				If TypeOf view Is Scrollable Then Return CType(view, Scrollable).preferredScrollableViewportSize
				Return preferredSize
			End Get
		End Property

		''' <summary>
		''' Returns a scroll increment, which is required for components
		''' that display logical rows or columns in order to completely expose
		''' one block of rows or columns, depending on the value of orientation.
		''' <p>
		''' If the view component of this layer implements <seealso cref="Scrollable"/>, this method delegates its
		''' implementation to the view component.
		''' </summary>
		''' <returns> the "block" increment for scrolling in the specified direction
		''' </returns>
		''' <seealso cref= Scrollable </seealso>
		Public Function getScrollableBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			If TypeOf view Is Scrollable Then Return CType(view, Scrollable).getScrollableBlockIncrement(visibleRect, orientation, direction)
			Return If(orientation = SwingConstants.VERTICAL, visibleRect.height, visibleRect.width)
		End Function

		''' <summary>
		''' Returns {@code false} to indicate that the height of the viewport does not
		''' determine the height of the layer, unless the preferred height
		''' of the layer is smaller than the height of the viewport.
		''' <p>
		''' If the view component of this layer implements <seealso cref="Scrollable"/>, this method delegates its
		''' implementation to the view component.
		''' </summary>
		''' <returns> whether the layer should track the height of the viewport
		''' </returns>
		''' <seealso cref= Scrollable </seealso>
		Public Property scrollableTracksViewportHeight As Boolean Implements Scrollable.getScrollableTracksViewportHeight
			Get
				If TypeOf view Is Scrollable Then Return CType(view, Scrollable).scrollableTracksViewportHeight
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns {@code false} to indicate that the width of the viewport does not
		''' determine the width of the layer, unless the preferred width
		''' of the layer is smaller than the width of the viewport.
		''' <p>
		''' If the view component of this layer implements <seealso cref="Scrollable"/>, this method delegates its
		''' implementation to the view component.
		''' </summary>
		''' <returns> whether the layer should track the width of the viewport
		''' </returns>
		''' <seealso cref= Scrollable </seealso>
		Public Property scrollableTracksViewportWidth As Boolean Implements Scrollable.getScrollableTracksViewportWidth
			Get
				If TypeOf view Is Scrollable Then Return CType(view, Scrollable).scrollableTracksViewportWidth
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns a scroll increment, which is required for components
		''' that display logical rows or columns in order to completely expose
		''' one new row or column, depending on the value of orientation.
		''' Ideally, components should handle a partially exposed row or column
		''' by returning the distance required to completely expose the item.
		''' <p>
		''' Scrolling containers, like {@code JScrollPane}, will use this method
		''' each time the user requests a unit scroll.
		''' <p>
		''' If the view component of this layer implements <seealso cref="Scrollable"/>, this method delegates its
		''' implementation to the view component.
		''' </summary>
		''' <returns> The "unit" increment for scrolling in the specified direction.
		'''         This value should always be positive.
		''' </returns>
		''' <seealso cref= Scrollable </seealso>
		Public Function getScrollableUnitIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			If TypeOf view Is Scrollable Then Return CType(view, Scrollable).getScrollableUnitIncrement(visibleRect, orientation, direction)
			Return 1
		End Function

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If layerUI IsNot Nothing Then uI = layerUI
			If eventMask <> 0 Then eventController.updateAWTEventListener(0, eventMask)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub addNotify()
			MyBase.addNotify()
			eventController.updateAWTEventListener(0, eventMask)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub removeNotify()
			MyBase.removeNotify()
			eventController.updateAWTEventListener(eventMask, 0)
		End Sub

		''' <summary>
		''' Delegates its functionality to the <seealso cref="javax.swing.plaf.LayerUI#doLayout(JLayer)"/> method,
		''' if {@code LayerUI} is set.
		''' </summary>
		Public Sub doLayout()
			If uI IsNot Nothing Then uI.doLayout(Me)
		End Sub

		''' <summary>
		''' Gets the AccessibleContext associated with this {@code JLayer}.
		''' </summary>
		''' <returns> the AccessibleContext associated with this {@code JLayer}. </returns>
		Public Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJComponentAnonymousInnerClassHelper
				Return accessibleContext
			End Get
		End Property

		Private Class AccessibleJComponentAnonymousInnerClassHelper
			Inherits AccessibleJComponent

			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PANEL
				End Get
			End Property
		End Class

		''' <summary>
		''' static AWTEventListener to be shared with all AbstractLayerUIs
		''' </summary>
		Private Class LayerEventController
			Implements AWTEventListener

			Private layerMaskList As New List(Of Long?)

			Private currentEventMask As Long

			Private Shared ReadOnly ACCEPTED_EVENTS As Long = AWTEvent.COMPONENT_EVENT_MASK Or AWTEvent.CONTAINER_EVENT_MASK Or AWTEvent.FOCUS_EVENT_MASK Or AWTEvent.KEY_EVENT_MASK Or AWTEvent.MOUSE_WHEEL_EVENT_MASK Or AWTEvent.MOUSE_MOTION_EVENT_MASK Or AWTEvent.MOUSE_EVENT_MASK Or AWTEvent.INPUT_METHOD_EVENT_MASK Or AWTEvent.HIERARCHY_EVENT_MASK Or AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Sub eventDispatched(ByVal [event] As AWTEvent)
				Dim source As Object = [event].source
				If TypeOf source Is Component Then
					Dim component As Component = CType(source, Component)
					Do While component IsNot Nothing
						If TypeOf component Is JLayer Then
							Dim l As JLayer = CType(component, JLayer)
							Dim ui As javax.swing.plaf.LayerUI = l.uI
							If ui IsNot Nothing AndAlso isEventEnabled(l.layerEventMask, [event].iD) AndAlso (Not(TypeOf [event] Is InputEvent) OrElse (Not CType([event], InputEvent).consumed)) Then ui.eventDispatched([event], l)
						End If
						component = component.parent
					Loop
				End If
			End Sub

			Private Sub updateAWTEventListener(ByVal oldEventMask As Long, ByVal newEventMask As Long)
				If oldEventMask <> 0 Then layerMaskList.RemoveAt(oldEventMask)
				If newEventMask <> 0 Then layerMaskList.Add(newEventMask)
				Dim combinedMask As Long = 0
				For Each mask As Long? In layerMaskList
					combinedMask = combinedMask Or mask
				Next mask
				' filter out all unaccepted events
				combinedMask = combinedMask And ACCEPTED_EVENTS
				If combinedMask = 0 Then
					removeAWTEventListener()
				ElseIf currentEventMask <> combinedMask Then
					removeAWTEventListener()
					addAWTEventListener(combinedMask)
				End If
				currentEventMask = combinedMask
			End Sub

			Private Property currentEventMask As Long
				Get
					Return currentEventMask
				End Get
			End Property

			Private Sub addAWTEventListener(ByVal eventMask As Long)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Void>()
	'			{
	'				public Void run()
	'				{
	'					Toolkit.getDefaultToolkit().addAWTEventListener(LayerEventController.this, eventMask);
	'					Return Nothing;
	'				}
	'			});

			End Sub

			Private Sub removeAWTEventListener()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Void>()
	'			{
	'				public Void run()
	'				{
	'					Toolkit.getDefaultToolkit().removeAWTEventListener(LayerEventController.this);
	'					Return Nothing;
	'				}
	'			});
			End Sub

			Private Function isEventEnabled(ByVal eventMask As Long, ByVal id As Integer) As Boolean
				Return (((eventMask And AWTEvent.COMPONENT_EVENT_MASK) <> 0 AndAlso id >= ComponentEvent.COMPONENT_FIRST AndAlso id <= ComponentEvent.COMPONENT_LAST) OrElse ((eventMask And AWTEvent.CONTAINER_EVENT_MASK) <> 0 AndAlso id >= ContainerEvent.CONTAINER_FIRST AndAlso id <= ContainerEvent.CONTAINER_LAST) OrElse ((eventMask And AWTEvent.FOCUS_EVENT_MASK) <> 0 AndAlso id >= FocusEvent.FOCUS_FIRST AndAlso id <= FocusEvent.FOCUS_LAST) OrElse ((eventMask And AWTEvent.KEY_EVENT_MASK) <> 0 AndAlso id >= KeyEvent.KEY_FIRST AndAlso id <= KeyEvent.KEY_LAST) OrElse ((eventMask And AWTEvent.MOUSE_WHEEL_EVENT_MASK) <> 0 AndAlso id = MouseEvent.MOUSE_WHEEL) OrElse ((eventMask And AWTEvent.MOUSE_MOTION_EVENT_MASK) <> 0 AndAlso (id = MouseEvent.MOUSE_MOVED OrElse id = MouseEvent.MOUSE_DRAGGED)) OrElse ((eventMask And AWTEvent.MOUSE_EVENT_MASK) <> 0 AndAlso id <> MouseEvent.MOUSE_MOVED AndAlso id <> MouseEvent.MOUSE_DRAGGED AndAlso id <> MouseEvent.MOUSE_WHEEL AndAlso id >= MouseEvent.MOUSE_FIRST AndAlso id <= MouseEvent.MOUSE_LAST) OrElse ((eventMask And AWTEvent.INPUT_METHOD_EVENT_MASK) <> 0 AndAlso id >= InputMethodEvent.INPUT_METHOD_FIRST AndAlso id <= InputMethodEvent.INPUT_METHOD_LAST) OrElse ((eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 AndAlso id = HierarchyEvent.HIERARCHY_CHANGED) OrElse ((eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 AndAlso (id = HierarchyEvent.ANCESTOR_MOVED OrElse id = HierarchyEvent.ANCESTOR_RESIZED)))
			End Function
		End Class

		''' <summary>
		''' The default glassPane for the <seealso cref="javax.swing.JLayer"/>.
		''' It is a subclass of {@code JPanel} which is non opaque by default.
		''' </summary>
		Private Class DefaultLayerGlassPane
			Inherits JPanel

			''' <summary>
			''' Creates a new <seealso cref="DefaultLayerGlassPane"/>
			''' </summary>
			Public Sub New()
				opaque = False
			End Sub

			''' <summary>
			''' First, implementation of this method iterates through
			''' glassPane's child components and returns {@code true}
			''' if any of them is visible and contains passed x,y point.
			''' After that it checks if no mouseListeners is attached to this component
			''' and no mouse cursor is set, then it returns {@code false},
			''' otherwise calls the super implementation of this method.
			''' </summary>
			''' <param name="x"> the <i>x</i> coordinate of the point </param>
			''' <param name="y"> the <i>y</i> coordinate of the point </param>
			''' <returns> true if this component logically contains x,y </returns>
			Public Overrides Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
				For i As Integer = 0 To componentCount - 1
					Dim c As Component = getComponent(i)
					Dim point As Point = SwingUtilities.convertPoint(Me, New Point(x, y), c)
					If c.visible AndAlso c.contains(point) Then Return True
				Next i
				If mouseListeners.length = 0 AndAlso mouseMotionListeners.length = 0 AndAlso mouseWheelListeners.length = 0 AndAlso (Not cursorSet) Then Return False
				Return MyBase.contains(x, y)
			End Function
		End Class
	End Class

End Namespace
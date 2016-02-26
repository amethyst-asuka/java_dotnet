Imports System
Imports System.Threading
Imports javax.accessibility
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
	''' A lightweight container used behind the scenes by
	''' <code>JFrame</code>, <code>JDialog</code>, <code>JWindow</code>,
	''' <code>JApplet</code>, and <code>JInternalFrame</code>.
	''' For task-oriented information on functionality provided by root panes
	''' see <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/rootpane.html">How to Use Root Panes</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' The following image shows the relationships between
	''' the classes that use root panes.
	''' <p style="text-align:center"><img src="doc-files/JRootPane-1.gif"
	''' alt="The following text describes this graphic."
	''' HEIGHT=484 WIDTH=629></p>
	''' The &quot;heavyweight&quot; components (those that delegate to a peer, or native
	''' component on the host system) are shown with a darker, heavier box. The four
	''' heavyweight JFC/Swing containers (<code>JFrame</code>, <code>JDialog</code>,
	''' <code>JWindow</code>, and <code>JApplet</code>) are
	''' shown in relation to the AWT classes they extend.
	''' These four components are the
	''' only heavyweight containers in the Swing library. The lightweight container
	''' <code>JInternalFrame</code> is also shown.
	''' All five of these JFC/Swing containers implement the
	''' <code>RootPaneContainer</code> interface,
	''' and they all delegate their operations to a
	''' <code>JRootPane</code> (shown with a little "handle" on top).
	''' <blockquote>
	''' <b>Note:</b> The <code>JComponent</code> method <code>getRootPane</code>
	''' can be used to obtain the <code>JRootPane</code> that contains
	''' a given component.
	''' </blockquote>
	''' <table style="float:right" border="0" summary="layout">
	''' <tr>
	''' <td align="center">
	''' <img src="doc-files/JRootPane-2.gif"
	''' alt="The following text describes this graphic." HEIGHT=386 WIDTH=349>
	''' </td>
	''' </tr>
	''' </table>
	''' The diagram at right shows the structure of a <code>JRootPane</code>.
	''' A <code>JRootpane</code> is made up of a <code>glassPane</code>,
	''' an optional <code>menuBar</code>, and a <code>contentPane</code>.
	''' (The <code>JLayeredPane</code> manages the <code>menuBar</code>
	''' and the <code>contentPane</code>.)
	''' The <code>glassPane</code> sits over the top of everything,
	''' where it is in a position to intercept mouse movements.
	''' Since the <code>glassPane</code> (like the <code>contentPane</code>)
	''' can be an arbitrary component, it is also possible to set up the
	''' <code>glassPane</code> for drawing. Lines and images on the
	''' <code>glassPane</code> can then range
	''' over the frames underneath without being limited by their boundaries.
	''' <p>
	''' Although the <code>menuBar</code> component is optional,
	''' the <code>layeredPane</code>, <code>contentPane</code>,
	''' and <code>glassPane</code> always exist.
	''' Attempting to set them to <code>null</code> generates an exception.
	''' <p>
	''' To add components to the <code>JRootPane</code> (other than the
	''' optional menu bar), you add the object to the <code>contentPane</code>
	''' of the <code>JRootPane</code>, like this:
	''' <pre>
	'''       rootPane.getContentPane().add(child);
	''' </pre>
	''' The same principle holds true for setting layout managers, removing
	''' components, listing children, etc. All these methods are invoked on
	''' the <code>contentPane</code> instead of on the <code>JRootPane</code>.
	''' <blockquote>
	''' <b>Note:</b> The default layout manager for the <code>contentPane</code> is
	'''  a <code>BorderLayout</code> manager. However, the <code>JRootPane</code>
	'''  uses a custom <code>LayoutManager</code>.
	'''  So, when you want to change the layout manager for the components you added
	'''  to a <code>JRootPane</code>, be sure to use code like this:
	''' <pre>
	'''    rootPane.getContentPane().setLayout(new BoxLayout());
	''' </pre></blockquote>
	''' If a <code>JMenuBar</code> component is set on the <code>JRootPane</code>,
	''' it is positioned along the upper edge of the frame.
	''' The <code>contentPane</code> is adjusted in location and size to
	''' fill the remaining area.
	''' (The <code>JMenuBar</code> and the <code>contentPane</code> are added to the
	''' <code>layeredPane</code> component at the
	''' <code>JLayeredPane.FRAME_CONTENT_LAYER</code> layer.)
	''' <p>
	''' The <code>layeredPane</code> is the parent of all children in the
	''' <code>JRootPane</code> -- both as the direct parent of the menu and
	''' the grandparent of all components added to the <code>contentPane</code>.
	''' It is an instance of <code>JLayeredPane</code>,
	''' which provides the ability to add components at several layers.
	''' This capability is very useful when working with menu popups,
	''' dialog boxes, and dragging -- situations in which you need to place
	''' a component on top of all other components in the pane.
	''' <p>
	''' The <code>glassPane</code> sits on top of all other components in the
	''' <code>JRootPane</code>.
	''' That provides a convenient place to draw above all other components,
	''' and makes it possible to intercept mouse events,
	''' which is useful both for dragging and for drawing.
	''' Developers can use <code>setVisible</code> on the <code>glassPane</code>
	''' to control when the <code>glassPane</code> displays over the other children.
	''' By default the <code>glassPane</code> is not visible.
	''' <p>
	''' The custom <code>LayoutManager</code> used by <code>JRootPane</code>
	''' ensures that:
	''' <OL>
	''' <LI>The <code>glassPane</code> fills the entire viewable
	'''     area of the <code>JRootPane</code> (bounds - insets).
	''' <LI>The <code>layeredPane</code> fills the entire viewable area of the
	'''     <code>JRootPane</code>. (bounds - insets)
	''' <LI>The <code>menuBar</code> is positioned at the upper edge of the
	'''     <code>layeredPane</code>.
	''' <LI>The <code>contentPane</code> fills the entire viewable area,
	'''     minus the <code>menuBar</code>, if present.
	''' </OL>
	''' Any other views in the <code>JRootPane</code> view hierarchy are ignored.
	''' <p>
	''' If you replace the <code>LayoutManager</code> of the <code>JRootPane</code>,
	''' you are responsible for managing all of these views.
	''' So ordinarily you will want to be sure that you
	''' change the layout manager for the <code>contentPane</code> rather than
	''' for the <code>JRootPane</code> itself!
	''' <p>
	''' The painting architecture of Swing requires an opaque
	''' <code>JComponent</code>
	''' to exist in the containment hierarchy above all other components. This is
	''' typically provided by way of the content pane. If you replace the content
	''' pane, it is recommended that you make the content pane opaque
	''' by way of <code>setOpaque(true)</code>. Additionally, if the content pane
	''' overrides <code>paintComponent</code>, it
	''' will need to completely fill in the background in an opaque color in
	''' <code>paintComponent</code>.
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
	''' </summary>
	''' <seealso cref= JLayeredPane </seealso>
	''' <seealso cref= JMenuBar </seealso>
	''' <seealso cref= JWindow </seealso>
	''' <seealso cref= JFrame </seealso>
	''' <seealso cref= JDialog </seealso>
	''' <seealso cref= JApplet </seealso>
	''' <seealso cref= JInternalFrame </seealso>
	''' <seealso cref= JComponent </seealso>
	''' <seealso cref= BoxLayout
	''' </seealso>
	''' <seealso cref= <a href="http://java.sun.com/products/jfc/tsc/articles/mixing/">
	''' Mixing Heavy and Light Components</a>
	''' 
	''' @author David Kloba </seealso>
	'/ PENDING(klobad) Who should be opaque in this component?
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JRootPane
		Inherits JComponent
		Implements Accessible

		Private Const uiClassID As String = "RootPaneUI"

		''' <summary>
		''' Whether or not we should dump the stack when true double buffering
		''' is disabled. Default is false.
		''' </summary>
		Private Shared ReadOnly LOG_DISABLE_TRUE_DOUBLE_BUFFERING As Boolean

		''' <summary>
		''' Whether or not we should ignore requests to disable true double
		''' buffering. Default is false.
		''' </summary>
		Private Shared ReadOnly IGNORE_DISABLE_TRUE_DOUBLE_BUFFERING As Boolean

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should not provide any sort of
		''' Window decorations.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const NONE As Integer = 0

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Frame.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const FRAME As Integer = 1

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const PLAIN_DIALOG As Integer = 2

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog used to display an informational message.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const INFORMATION_DIALOG As Integer = 3

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog used to display an error message.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const ERROR_DIALOG As Integer = 4

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog used to display a <code>JColorChooser</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const COLOR_CHOOSER_DIALOG As Integer = 5

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog used to display a <code>JFileChooser</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const FILE_CHOOSER_DIALOG As Integer = 6

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog used to present a question to the user.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const QUESTION_DIALOG As Integer = 7

		''' <summary>
		''' Constant used for the windowDecorationStyle property. Indicates that
		''' the <code>JRootPane</code> should provide decorations appropriate for
		''' a Dialog used to display a warning message.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const WARNING_DIALOG As Integer = 8

		Private windowDecorationStyle As Integer

		''' <summary>
		''' The menu bar. </summary>
		Protected Friend menuBar As JMenuBar

		''' <summary>
		''' The content pane. </summary>
		Protected Friend contentPane As Container

		''' <summary>
		''' The layered pane that manages the menu bar and content pane. </summary>
		Protected Friend layeredPane As JLayeredPane

		''' <summary>
		''' The glass pane that overlays the menu bar and content pane,
		'''  so it can intercept mouse movements and such.
		''' </summary>
		Protected Friend glassPane As Component
		''' <summary>
		''' The button that gets activated when the pane has the focus and
		''' a UI-specific action like pressing the <b>Enter</b> key occurs.
		''' </summary>
		Protected Friend defaultButton As JButton
		''' <summary>
		''' As of Java 2 platform v1.3 this unusable field is no longer used.
		''' To override the default button you should replace the <code>Action</code>
		''' in the <code>JRootPane</code>'s <code>ActionMap</code>. Please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		'''  <seealso cref= #defaultButton </seealso>
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend defaultPressAction As DefaultAction
		''' <summary>
		''' As of Java 2 platform v1.3 this unusable field is no longer used.
		''' To override the default button you should replace the <code>Action</code>
		''' in the <code>JRootPane</code>'s <code>ActionMap</code>. Please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		'''  <seealso cref= #defaultButton </seealso>
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend defaultReleaseAction As DefaultAction

		''' <summary>
		''' Whether or not true double buffering should be used.  This is typically
		''' true, but may be set to false in special situations.  For example,
		''' heavy weight popups (backed by a window) set this to false.
		''' </summary>
		Friend useTrueDoubleBuffering As Boolean = True

		Shared Sub New()
			LOG_DISABLE_TRUE_DOUBLE_BUFFERING = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("swing.logDoubleBufferingDisable"))
			IGNORE_DISABLE_TRUE_DOUBLE_BUFFERING = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("swing.ignoreDoubleBufferingDisable"))
		End Sub

		''' <summary>
		''' Creates a <code>JRootPane</code>, setting up its
		''' <code>glassPane</code>, <code>layeredPane</code>,
		''' and <code>contentPane</code>.
		''' </summary>
		Public Sub New()
			glassPane = createGlassPane()
			layeredPane = createLayeredPane()
			contentPane = createContentPane()
			layout = createRootLayout()
			doubleBuffered = True
			updateUI()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overrides Property doubleBuffered As Boolean
			Set(ByVal aFlag As Boolean)
				If doubleBuffered <> aFlag Then
					MyBase.doubleBuffered = aFlag
					RepaintManager.currentManager(Me).doubleBufferingChanged(Me)
				End If
			End Set
		End Property

		''' <summary>
		''' Returns a constant identifying the type of Window decorations the
		''' <code>JRootPane</code> is providing.
		''' </summary>
		''' <returns> One of <code>NONE</code>, <code>FRAME</code>,
		'''        <code>PLAIN_DIALOG</code>, <code>INFORMATION_DIALOG</code>,
		'''        <code>ERROR_DIALOG</code>, <code>COLOR_CHOOSER_DIALOG</code>,
		'''        <code>FILE_CHOOSER_DIALOG</code>, <code>QUESTION_DIALOG</code> or
		'''        <code>WARNING_DIALOG</code>. </returns>
		''' <seealso cref= #setWindowDecorationStyle
		''' @since 1.4 </seealso>
		Public Overridable Property windowDecorationStyle As Integer
			Get
				Return windowDecorationStyle
			End Get
			Set(ByVal windowDecorationStyle As Integer)
				If windowDecorationStyle < 0 OrElse windowDecorationStyle > WARNING_DIALOG Then Throw New System.ArgumentException("Invalid decoration style")
				Dim oldWindowDecorationStyle As Integer = windowDecorationStyle
				Me.windowDecorationStyle = windowDecorationStyle
				firePropertyChange("windowDecorationStyle", oldWindowDecorationStyle, windowDecorationStyle)
			End Set
		End Property


		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> <code>LabelUI</code> object
		''' @since 1.3 </returns>
		Public Overridable Property uI As javax.swing.plaf.RootPaneUI
			Get
				Return CType(ui, javax.swing.plaf.RootPaneUI)
			End Get
			Set(ByVal ui As javax.swing.plaf.RootPaneUI)
				MyBase.uI = ui
			End Set
		End Property



		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.RootPaneUI)
		End Sub


		''' <summary>
		''' Returns a string that specifies the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> the string "RootPaneUI"
		''' </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Called by the constructor methods to create the default
		''' <code>layeredPane</code>.
		''' Bt default it creates a new <code>JLayeredPane</code>. </summary>
		''' <returns> the default <code>layeredPane</code> </returns>
		Protected Friend Overridable Function createLayeredPane() As JLayeredPane
			Dim p As New JLayeredPane
			p.name = Me.name & ".layeredPane"
			Return p
		End Function

		''' <summary>
		''' Called by the constructor methods to create the default
		''' <code>contentPane</code>.
		''' By default this method creates a new <code>JComponent</code> add sets a
		''' <code>BorderLayout</code> as its <code>LayoutManager</code>. </summary>
		''' <returns> the default <code>contentPane</code> </returns>
		Protected Friend Overridable Function createContentPane() As Container
			Dim c As JComponent = New JPanel
			c.name = Me.name & ".contentPane"
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			c.setLayout(New BorderLayout()
	'		{
	''             This BorderLayout subclass maps a null constraint to CENTER.
	''             * Although the reference BorderLayout also does this, some VMs
	''             * throw an IllegalArgumentException.
	''             
	'			public void addLayoutComponent(Component comp, Object constraints)
	'			{
	'				if (constraints == Nothing)
	'				{
	'					constraints = BorderLayout.CENTER;
	'				}
	'				MyBase.addLayoutComponent(comp, constraints);
	'			}
	'		});
			Return c
		End Function

		''' <summary>
		''' Called by the constructor methods to create the default
		''' <code>glassPane</code>.
		''' By default this method creates a new <code>JComponent</code>
		''' with visibility set to false. </summary>
		''' <returns> the default <code>glassPane</code> </returns>
		Protected Friend Overridable Function createGlassPane() As Component
			Dim c As JComponent = New JPanel
			c.name = Me.name & ".glassPane"
			c.visible = False
			CType(c, JPanel).opaque = False
			Return c
		End Function

		''' <summary>
		''' Called by the constructor methods to create the default
		''' <code>layoutManager</code>. </summary>
		''' <returns> the default <code>layoutManager</code>. </returns>
		Protected Friend Overridable Function createRootLayout() As LayoutManager
			Return New RootLayout(Me)
		End Function

		''' <summary>
		''' Adds or changes the menu bar used in the layered pane. </summary>
		''' <param name="menu"> the <code>JMenuBar</code> to add </param>
		Public Overridable Property jMenuBar As JMenuBar
			Set(ByVal menu As JMenuBar)
				If menuBar IsNot Nothing AndAlso menuBar.parent Is layeredPane Then layeredPane.remove(menuBar)
				menuBar = menu
    
				If menuBar IsNot Nothing Then layeredPane.add(menuBar, JLayeredPane.FRAME_CONTENT_LAYER)
			End Set
			Get
				Return menuBar
			End Get
		End Property

		''' <summary>
		''' Specifies the menu bar value. </summary>
		''' @deprecated As of Swing version 1.0.3
		'''  replaced by <code>setJMenuBar(JMenuBar menu)</code>. 
		''' <param name="menu"> the <code>JMenuBar</code> to add. </param>
		<Obsolete("As of Swing version 1.0.3")> _
		Public Overridable Property menuBar As JMenuBar
			Set(ByVal menu As JMenuBar)
				If menuBar IsNot Nothing AndAlso menuBar.parent Is layeredPane Then layeredPane.remove(menuBar)
				menuBar = menu
    
				If menuBar IsNot Nothing Then layeredPane.add(menuBar, JLayeredPane.FRAME_CONTENT_LAYER)
			End Set
			Get
				Return menuBar
			End Get
		End Property



		''' <summary>
		''' Sets the content pane -- the container that holds the components
		''' parented by the root pane.
		''' <p>
		''' Swing's painting architecture requires an opaque <code>JComponent</code>
		''' in the containment hierarchy. This is typically provided by the
		''' content pane. If you replace the content pane it is recommended you
		''' replace it with an opaque <code>JComponent</code>.
		''' </summary>
		''' <param name="content"> the <code>Container</code> to use for component-contents </param>
		''' <exception cref="java.awt.IllegalComponentStateException"> (a runtime
		'''            exception) if the content pane parameter is <code>null</code> </exception>
		Public Overridable Property contentPane As Container
			Set(ByVal content As Container)
				If content Is Nothing Then Throw New IllegalComponentStateException("contentPane cannot be set to null.")
				If contentPane IsNot Nothing AndAlso contentPane.parent Is layeredPane Then layeredPane.remove(contentPane)
				contentPane = content
    
				layeredPane.add(contentPane, JLayeredPane.FRAME_CONTENT_LAYER)
			End Set
			Get
				Return contentPane
			End Get
		End Property


	' PENDING(klobad) Should this reparent the contentPane and MenuBar?
		''' <summary>
		''' Sets the layered pane for the root pane. The layered pane
		''' typically holds a content pane and an optional <code>JMenuBar</code>.
		''' </summary>
		''' <param name="layered">  the <code>JLayeredPane</code> to use </param>
		''' <exception cref="java.awt.IllegalComponentStateException"> (a runtime
		'''            exception) if the layered pane parameter is <code>null</code> </exception>
		Public Overridable Property layeredPane As JLayeredPane
			Set(ByVal layered As JLayeredPane)
				If layered Is Nothing Then Throw New IllegalComponentStateException("layeredPane cannot be set to null.")
				If layeredPane IsNot Nothing AndAlso layeredPane.parent Is Me Then Me.remove(layeredPane)
				layeredPane = layered
    
				Me.add(layeredPane, -1)
			End Set
			Get
				Return layeredPane
			End Get
		End Property

		''' <summary>
		''' Sets a specified <code>Component</code> to be the glass pane for this
		''' root pane.  The glass pane should normally be a lightweight,
		''' transparent component, because it will be made visible when
		''' ever the root pane needs to grab input events.
		''' <p>
		''' The new glass pane's visibility is changed to match that of
		''' the current glass pane.  An implication of this is that care
		''' must be taken when you want to replace the glass pane and
		''' make it visible.  Either of the following will work:
		''' <pre>
		'''   root.setGlassPane(newGlassPane);
		'''   newGlassPane.setVisible(true);
		''' </pre>
		''' or:
		''' <pre>
		'''   root.getGlassPane().setVisible(true);
		'''   root.setGlassPane(newGlassPane);
		''' </pre>
		''' </summary>
		''' <param name="glass"> the <code>Component</code> to use as the glass pane
		'''              for this <code>JRootPane</code> </param>
		''' <exception cref="NullPointerException"> if the <code>glass</code> parameter is
		'''          <code>null</code> </exception>
		Public Overridable Property glassPane As Component
			Set(ByVal glass As Component)
				If glass Is Nothing Then Throw New NullPointerException("glassPane cannot be set to null.")
    
				sun.awt.AWTAccessor.componentAccessor.mixingCutoutShapeape(glass, New Rectangle)
    
				Dim ___visible As Boolean = False
				If glassPane IsNot Nothing AndAlso glassPane.parent Is Me Then
					Me.remove(glassPane)
					___visible = glassPane.visible
				End If
    
				glass.visible = ___visible
				glassPane = glass
				Me.add(glassPane, 0)
				If ___visible Then repaint()
			End Set
			Get
				Return glassPane
			End Get
		End Property


		''' <summary>
		''' If a descendant of this <code>JRootPane</code> calls
		''' <code>revalidate</code>, validate from here on down.
		''' <p>
		''' Deferred requests to layout a component and its descendents again.
		''' For example, calls to <code>revalidate</code>, are pushed upwards to
		''' either a <code>JRootPane</code> or a <code>JScrollPane</code>
		''' because both classes override <code>isValidateRoot</code> to return true.
		''' </summary>
		''' <seealso cref= JComponent#isValidateRoot </seealso>
		''' <seealso cref= java.awt.Container#isValidateRoot </seealso>
		''' <returns> true </returns>
		Public Property Overrides validateRoot As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' The <code>glassPane</code> and <code>contentPane</code>
		''' have the same bounds, which means <code>JRootPane</code>
		''' does not tiles its children and this should return false.
		''' On the other hand, the <code>glassPane</code>
		''' is normally not visible, and so this can return true if the
		''' <code>glassPane</code> isn't visible. Therefore, the
		''' return value here depends upon the visibility of the
		''' <code>glassPane</code>.
		''' </summary>
		''' <returns> true if this component's children don't overlap </returns>
		Public Property Overrides optimizedDrawingEnabled As Boolean
			Get
				Return Not glassPane.visible
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub addNotify()
			MyBase.addNotify()
			enableEvents(AWTEvent.KEY_EVENT_MASK)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub removeNotify()
			MyBase.removeNotify()
		End Sub


		''' <summary>
		''' Sets the <code>defaultButton</code> property,
		''' which determines the current default button for this <code>JRootPane</code>.
		''' The default button is the button which will be activated
		''' when a UI-defined activation event (typically the <b>Enter</b> key)
		''' occurs in the root pane regardless of whether or not the button
		''' has keyboard focus (unless there is another component within
		''' the root pane which consumes the activation event,
		''' such as a <code>JTextPane</code>).
		''' For default activation to work, the button must be an enabled
		''' descendent of the root pane when activation occurs.
		''' To remove a default button from this root pane, set this
		''' property to <code>null</code>.
		''' </summary>
		''' <seealso cref= JButton#isDefaultButton </seealso>
		''' <param name="defaultButton"> the <code>JButton</code> which is to be the default button
		''' 
		''' @beaninfo
		'''  description: The button activated by default in this root pane </param>
		Public Overridable Property defaultButton As JButton
			Set(ByVal defaultButton As JButton)
				Dim oldDefault As JButton = Me.defaultButton
    
				If oldDefault IsNot defaultButton Then
					Me.defaultButton = defaultButton
    
					If oldDefault IsNot Nothing Then oldDefault.repaint()
					If defaultButton IsNot Nothing Then defaultButton.repaint()
				End If
    
				firePropertyChange("defaultButton", oldDefault, defaultButton)
			End Set
			Get
				Return defaultButton
			End Get
		End Property


		Friend Property useTrueDoubleBuffering As Boolean
			Set(ByVal useTrueDoubleBuffering As Boolean)
				Me.useTrueDoubleBuffering = useTrueDoubleBuffering
			End Set
			Get
				Return useTrueDoubleBuffering
			End Get
		End Property


		Friend Sub disableTrueDoubleBuffering()
			If useTrueDoubleBuffering Then
				If Not IGNORE_DISABLE_TRUE_DOUBLE_BUFFERING Then
					If LOG_DISABLE_TRUE_DOUBLE_BUFFERING Then
						Console.WriteLine("Disabling true double buffering for " & Me)
						Thread.dumpStack()
					End If
					useTrueDoubleBuffering = False
					RepaintManager.currentManager(Me).doubleBufferingChanged(Me)
				End If
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Class DefaultAction
			Inherits AbstractAction

			Friend owner As JButton
			Friend root As JRootPane
			Friend press As Boolean
			Friend Sub New(ByVal root As JRootPane, ByVal press As Boolean)
				Me.root = root
				Me.press = press
			End Sub
			Public Overridable Property owner As JButton
				Set(ByVal owner As JButton)
					Me.owner = owner
				End Set
			End Property
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If owner IsNot Nothing AndAlso SwingUtilities.getRootPane(owner) Is root Then
					Dim model As ButtonModel = owner.model
					If press Then
						model.armed = True
						model.pressed = True
					Else
						model.pressed = False
					End If
				End If
			End Sub
			Public Property Overrides enabled As Boolean
				Get
					Return owner.model.enabled
				End Get
			End Property
		End Class


		''' <summary>
		''' Overridden to enforce the position of the glass component as
		''' the zero child.
		''' </summary>
		''' <param name="comp"> the component to be enhanced </param>
		''' <param name="constraints"> the constraints to be respected </param>
		''' <param name="index"> the index </param>
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			MyBase.addImpl(comp, constraints, index)

			'/ We are making sure the glassPane is on top.
			If glassPane IsNot Nothing AndAlso glassPane.parent Is Me AndAlso getComponent(0) IsNot glassPane Then add(glassPane, 0)
		End Sub


	'/////////////////////////////////////////////////////////////////////////////
	'// Begin Inner Classes
	'/////////////////////////////////////////////////////////////////////////////


		''' <summary>
		''' A custom layout manager that is responsible for the layout of
		''' layeredPane, glassPane, and menuBar.
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
		<Serializable> _
		Protected Friend Class RootLayout
			Implements LayoutManager2

			Private ReadOnly outerInstance As JRootPane

			Public Sub New(ByVal outerInstance As JRootPane)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Returns the amount of space the layout would like to have.
			''' </summary>
			''' <param name="parent"> the Container for which this layout manager
			''' is being used </param>
			''' <returns> a Dimension object containing the layout's preferred size </returns>
			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Dim rd, mbd As Dimension
				Dim i As Insets = outerInstance.insets

				If outerInstance.contentPane IsNot Nothing Then
					rd = outerInstance.contentPane.preferredSize
				Else
					rd = parent.size
				End If
				If outerInstance.menuBar IsNot Nothing AndAlso outerInstance.menuBar.visible Then
					mbd = outerInstance.menuBar.preferredSize
				Else
					mbd = New Dimension(0, 0)
				End If
				Return New Dimension(Math.Max(rd.width, mbd.width) + i.left + i.right, rd.height + mbd.height + i.top + i.bottom)
			End Function

			''' <summary>
			''' Returns the minimum amount of space the layout needs.
			''' </summary>
			''' <param name="parent"> the Container for which this layout manager
			''' is being used </param>
			''' <returns> a Dimension object containing the layout's minimum size </returns>
			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Dim rd, mbd As Dimension
				Dim i As Insets = outerInstance.insets
				If outerInstance.contentPane IsNot Nothing Then
					rd = outerInstance.contentPane.minimumSize
				Else
					rd = parent.size
				End If
				If outerInstance.menuBar IsNot Nothing AndAlso outerInstance.menuBar.visible Then
					mbd = outerInstance.menuBar.minimumSize
				Else
					mbd = New Dimension(0, 0)
				End If
				Return New Dimension(Math.Max(rd.width, mbd.width) + i.left + i.right, rd.height + mbd.height + i.top + i.bottom)
			End Function

			''' <summary>
			''' Returns the maximum amount of space the layout can use.
			''' </summary>
			''' <param name="target"> the Container for which this layout manager
			''' is being used </param>
			''' <returns> a Dimension object containing the layout's maximum size </returns>
			Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension
				Dim rd, mbd As Dimension
				Dim i As Insets = outerInstance.insets
				If outerInstance.menuBar IsNot Nothing AndAlso outerInstance.menuBar.visible Then
					mbd = outerInstance.menuBar.maximumSize
				Else
					mbd = New Dimension(0, 0)
				End If
				If outerInstance.contentPane IsNot Nothing Then
					rd = outerInstance.contentPane.maximumSize
				Else
					' This is silly, but should stop an overflow error
					rd = New Dimension(Integer.MaxValue, Integer.MaxValue - i.top - i.bottom - mbd.height - 1)
				End If
				Return New Dimension(Math.Min(rd.width, mbd.width) + i.left + i.right, rd.height + mbd.height + i.top + i.bottom)
			End Function

			''' <summary>
			''' Instructs the layout manager to perform the layout for the specified
			''' container.
			''' </summary>
			''' <param name="parent"> the Container for which this layout manager
			''' is being used </param>
			Public Overridable Sub layoutContainer(ByVal parent As Container)
				Dim b As Rectangle = parent.bounds
				Dim i As Insets = outerInstance.insets
				Dim contentY As Integer = 0
				Dim w As Integer = b.width - i.right - i.left
				Dim h As Integer = b.height - i.top - i.bottom

				If outerInstance.layeredPane IsNot Nothing Then outerInstance.layeredPane.boundsnds(i.left, i.top, w, h)
				If outerInstance.glassPane IsNot Nothing Then outerInstance.glassPane.boundsnds(i.left, i.top, w, h)
				' Note: This is laying out the children in the layeredPane,
				' technically, these are not our children.
				If outerInstance.menuBar IsNot Nothing AndAlso outerInstance.menuBar.visible Then
					Dim mbd As Dimension = outerInstance.menuBar.preferredSize
					outerInstance.menuBar.boundsnds(0, 0, w, mbd.height)
					contentY += mbd.height
				End If
				If outerInstance.contentPane IsNot Nothing Then outerInstance.contentPane.boundsnds(0, contentY, w, h - contentY)
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			End Sub
			Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			End Sub
			Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object)
			End Sub
			Public Overridable Function getLayoutAlignmentX(ByVal target As Container) As Single
				Return 0.0f
			End Function
			Public Overridable Function getLayoutAlignmentY(ByVal target As Container) As Single
				Return 0.0f
			End Function
			Public Overridable Sub invalidateLayout(ByVal target As Container)
			End Sub
		End Class

		''' <summary>
		''' Returns a string representation of this <code>JRootPane</code>.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JRootPane</code>. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JRootPane</code>. For root panes, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJRootPane</code>.
		''' A new <code>AccessibleJRootPane</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJRootPane</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>JRootPane</code> </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJRootPane(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JRootPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to root pane user-interface elements.
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
		Protected Friend Class AccessibleJRootPane
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JRootPane

			Public Sub New(ByVal outerInstance As JRootPane)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of
			''' the object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.ROOT_PANE
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children of the object.
			''' </summary>
			''' <returns> the number of accessible children of the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return MyBase.accessibleChildrenCount
				End Get
			End Property

			''' <summary>
			''' Returns the specified Accessible child of the object.  The Accessible
			''' children of an Accessible object are zero-based, so the first child
			''' of an Accessible child is at index 0, the second child is at index 1,
			''' and so on.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the Accessible child of the object </returns>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				Return MyBase.getAccessibleChild(i)
			End Function
		End Class ' inner class AccessibleJRootPane
	End Class

End Namespace
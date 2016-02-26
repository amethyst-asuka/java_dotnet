Imports System
Imports System.Text
Imports javax.swing.plaf
Imports javax.accessibility

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' A lightweight object that provides many of the features of
	''' a native frame, including dragging, closing, becoming an icon,
	''' resizing, title display, and support for a menu bar.
	''' For task-oriented documentation and examples of using internal frames,
	''' see <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/internalframe.html" target="_top">How to Use Internal Frames</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' 
	''' Generally,
	''' you add <code>JInternalFrame</code>s to a <code>JDesktopPane</code>. The UI
	''' delegates the look-and-feel-specific actions to the
	''' <code>DesktopManager</code>
	''' object maintained by the <code>JDesktopPane</code>.
	''' <p>
	''' The <code>JInternalFrame</code> content pane
	''' is where you add child components.
	''' As a convenience, the {@code add}, {@code remove}, and {@code setLayout}
	''' methods of this class are overridden, so that they delegate calls
	''' to the corresponding methods of the {@code ContentPane}.
	''' For example, you can add a child component to an internal frame as follows:
	''' <pre>
	'''       internalFrame.add(child);
	''' </pre>
	''' And the child will be added to the contentPane.
	''' The content pane is actually managed by an instance of
	''' <code>JRootPane</code>,
	''' which also manages a layout pane, glass pane, and
	''' optional menu bar for the internal frame. Please see the
	''' <code>JRootPane</code>
	''' documentation for a complete description of these components.
	''' Refer to <seealso cref="javax.swing.RootPaneContainer"/>
	''' for details on adding, removing and setting the <code>LayoutManager</code>
	''' of a <code>JInternalFrame</code>.
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
	''' <seealso cref= InternalFrameEvent </seealso>
	''' <seealso cref= JDesktopPane </seealso>
	''' <seealso cref= DesktopManager </seealso>
	''' <seealso cref= JInternalFrame.JDesktopIcon </seealso>
	''' <seealso cref= JRootPane </seealso>
	''' <seealso cref= javax.swing.RootPaneContainer
	''' 
	''' @author David Kloba
	''' @author Rich Schiavi
	''' @beaninfo
	'''      attribute: isContainer true
	'''      attribute: containerDelegate getContentPane
	'''      description: A frame container which is contained within
	'''                   another window. </seealso>
	Public Class JInternalFrame
		Inherits JComponent
		Implements Accessible, WindowConstants, RootPaneContainer

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "InternalFrameUI"

		''' <summary>
		''' The <code>JRootPane</code> instance that manages the
		''' content pane
		''' and optional menu bar for this internal frame, as well as the
		''' glass pane.
		''' </summary>
		''' <seealso cref= JRootPane </seealso>
		''' <seealso cref= RootPaneContainer </seealso>
		Protected Friend rootPane As JRootPane

		''' <summary>
		''' If true then calls to <code>add</code> and <code>setLayout</code>
		''' will be forwarded to the <code>contentPane</code>. This is initially
		''' false, but is set to true when the <code>JInternalFrame</code> is
		''' constructed.
		''' </summary>
		''' <seealso cref= #isRootPaneCheckingEnabled </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend rootPaneCheckingEnabled As Boolean = False

		''' <summary>
		''' The frame can be closed. </summary>
		Protected Friend closable As Boolean
		''' <summary>
		''' The frame has been closed. </summary>
		Protected Friend ___isClosed As Boolean
		''' <summary>
		''' The frame can be expanded to the size of the desktop pane. </summary>
		Protected Friend maximizable As Boolean
		''' <summary>
		''' The frame has been expanded to its maximum size. </summary>
		''' <seealso cref= #maximizable </seealso>
		Protected Friend ___isMaximum As Boolean
		''' <summary>
		''' The frame can "iconified" (shrunk down and displayed as
		''' an icon-image). </summary>
		''' <seealso cref= JInternalFrame.JDesktopIcon </seealso>
		''' <seealso cref= #setIconifiable </seealso>
		Protected Friend iconable As Boolean
		''' <summary>
		''' The frame has been iconified. </summary>
		''' <seealso cref= #isIcon() </seealso>
		Protected Friend ___isIcon As Boolean
		''' <summary>
		''' The frame's size can be changed. </summary>
		Protected Friend resizable As Boolean
		''' <summary>
		''' The frame is currently selected. </summary>
		Protected Friend ___isSelected As Boolean
		''' <summary>
		''' The icon shown in the top-left corner of this internal frame. </summary>
		Protected Friend frameIcon As Icon
		''' <summary>
		''' The title displayed in this internal frame's title bar. </summary>
		Protected Friend title As String
		''' <summary>
		''' The icon that is displayed when this internal frame is iconified. </summary>
		''' <seealso cref= #iconable </seealso>
		Protected Friend desktopIcon As JDesktopIcon

		Private lastCursor As Cursor

		Private opened As Boolean

		Private normalBounds As Rectangle = Nothing

		Private defaultCloseOperation As Integer = DISPOSE_ON_CLOSE

		''' <summary>
		''' Contains the Component that focus is to go when
		''' <code>restoreSubcomponentFocus</code> is invoked, that is,
		''' <code>restoreSubcomponentFocus</code> sets this to the value returned
		''' from <code>getMostRecentFocusOwner</code>.
		''' </summary>
		Private lastFocusOwner As Component

		''' <summary>
		''' Bound property name. </summary>
		Public Const CONTENT_PANE_PROPERTY As String = "contentPane"
		''' <summary>
		''' Bound property name. </summary>
		Public Const MENU_BAR_PROPERTY As String = "JMenuBar"
		''' <summary>
		''' Bound property name. </summary>
		Public Const TITLE_PROPERTY As String = "title"
		''' <summary>
		''' Bound property name. </summary>
		Public Const LAYERED_PANE_PROPERTY As String = "layeredPane"
		''' <summary>
		''' Bound property name. </summary>
		Public Const ROOT_PANE_PROPERTY As String = "rootPane"
		''' <summary>
		''' Bound property name. </summary>
		Public Const GLASS_PANE_PROPERTY As String = "glassPane"
		''' <summary>
		''' Bound property name. </summary>
		Public Const FRAME_ICON_PROPERTY As String = "frameIcon"

		''' <summary>
		''' Constrained property name indicated that this frame has
		''' selected status.
		''' </summary>
		Public Const IS_SELECTED_PROPERTY As String = "selected"
		''' <summary>
		''' Constrained property name indicating that the internal frame is closed. </summary>
		Public Const IS_CLOSED_PROPERTY As String = "closed"
		''' <summary>
		''' Constrained property name indicating that the internal frame is maximized. </summary>
		Public Const IS_MAXIMUM_PROPERTY As String = "maximum"
		''' <summary>
		''' Constrained property name indicating that the internal frame is iconified. </summary>
		Public Const IS_ICON_PROPERTY As String = "icon"

		Private Shared ReadOnly PROPERTY_CHANGE_LISTENER_KEY As Object = New StringBuilder("InternalFramePropertyChangeListener")

		Private Shared Sub addPropertyChangeListenerIfNecessary()
			If sun.awt.AppContext.appContext.get(PROPERTY_CHANGE_LISTENER_KEY) Is Nothing Then
				Dim focusListener As java.beans.PropertyChangeListener = New FocusPropertyChangeListener

				sun.awt.AppContext.appContext.put(PROPERTY_CHANGE_LISTENER_KEY, focusListener)

				KeyboardFocusManager.currentKeyboardFocusManager.addPropertyChangeListener(focusListener)
			End If
		End Sub

		Private Class FocusPropertyChangeListener
			Implements java.beans.PropertyChangeListener

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				If e.propertyName = "permanentFocusOwner" Then updateLastFocusOwner(CType(e.newValue, Component))
			End Sub
		End Class

		Private Shared Sub updateLastFocusOwner(ByVal component As Component)
			If component IsNot Nothing Then
				Dim parent As Component = component
				Do While parent IsNot Nothing AndAlso Not(TypeOf parent Is Window)
					If TypeOf parent Is JInternalFrame Then CType(parent, JInternalFrame).lastFocusOwner = component
					parent = parent.parent
				Loop
			End If
		End Sub

		''' <summary>
		''' Creates a non-resizable, non-closable, non-maximizable,
		''' non-iconifiable <code>JInternalFrame</code> with no title.
		''' </summary>
		Public Sub New()
			Me.New("", False, False, False, False)
		End Sub

		''' <summary>
		''' Creates a non-resizable, non-closable, non-maximizable,
		''' non-iconifiable <code>JInternalFrame</code> with the specified title.
		''' Note that passing in a <code>null</code> <code>title</code> results in
		''' unspecified behavior and possibly an exception.
		''' </summary>
		''' <param name="title">  the non-<code>null</code> <code>String</code>
		'''     to display in the title bar </param>
		Public Sub New(ByVal title As String)
			Me.New(title, False, False, False, False)
		End Sub

		''' <summary>
		''' Creates a non-closable, non-maximizable, non-iconifiable
		''' <code>JInternalFrame</code> with the specified title
		''' and resizability.
		''' </summary>
		''' <param name="title">      the <code>String</code> to display in the title bar </param>
		''' <param name="resizable">  if <code>true</code>, the internal frame can be resized </param>
		Public Sub New(ByVal title As String, ByVal resizable As Boolean)
			Me.New(title, resizable, False, False, False)
		End Sub

		''' <summary>
		''' Creates a non-maximizable, non-iconifiable <code>JInternalFrame</code>
		''' with the specified title, resizability, and
		''' closability.
		''' </summary>
		''' <param name="title">      the <code>String</code> to display in the title bar </param>
		''' <param name="resizable">  if <code>true</code>, the internal frame can be resized </param>
		''' <param name="closable">   if <code>true</code>, the internal frame can be closed </param>
		Public Sub New(ByVal title As String, ByVal resizable As Boolean, ByVal closable As Boolean)
			Me.New(title, resizable, closable, False, False)
		End Sub

		''' <summary>
		''' Creates a non-iconifiable <code>JInternalFrame</code>
		''' with the specified title,
		''' resizability, closability, and maximizability.
		''' </summary>
		''' <param name="title">       the <code>String</code> to display in the title bar </param>
		''' <param name="resizable">   if <code>true</code>, the internal frame can be resized </param>
		''' <param name="closable">    if <code>true</code>, the internal frame can be closed </param>
		''' <param name="maximizable"> if <code>true</code>, the internal frame can be maximized </param>
		Public Sub New(ByVal title As String, ByVal resizable As Boolean, ByVal closable As Boolean, ByVal maximizable As Boolean)
			Me.New(title, resizable, closable, maximizable, False)
		End Sub

		''' <summary>
		''' Creates a <code>JInternalFrame</code> with the specified title,
		''' resizability, closability, maximizability, and iconifiability.
		''' All <code>JInternalFrame</code> constructors use this one.
		''' </summary>
		''' <param name="title">       the <code>String</code> to display in the title bar </param>
		''' <param name="resizable">   if <code>true</code>, the internal frame can be resized </param>
		''' <param name="closable">    if <code>true</code>, the internal frame can be closed </param>
		''' <param name="maximizable"> if <code>true</code>, the internal frame can be maximized </param>
		''' <param name="iconifiable"> if <code>true</code>, the internal frame can be iconified </param>
		Public Sub New(ByVal title As String, ByVal resizable As Boolean, ByVal closable As Boolean, ByVal maximizable As Boolean, ByVal iconifiable As Boolean)

			rootPane = createRootPane()
			layout = New BorderLayout
			Me.title = title
			Me.resizable = resizable
			Me.closable = closable
			Me.maximizable = maximizable
			___isMaximum = False
			Me.iconable = iconifiable
			___isIcon = False
			visible = False
			rootPaneCheckingEnabled = True
			desktopIcon = New JDesktopIcon(Me)
			updateUI()
			sun.awt.SunToolkit.checkAndSetPolicy(Me)
			addPropertyChangeListenerIfNecessary()
		End Sub

		''' <summary>
		''' Called by the constructor to set up the <code>JRootPane</code>. </summary>
		''' <returns>  a new <code>JRootPane</code> </returns>
		''' <seealso cref= JRootPane </seealso>
		Protected Friend Overridable Function createRootPane() As JRootPane
			Return New JRootPane
		End Function

		''' <summary>
		''' Returns the look-and-feel object that renders this component.
		''' </summary>
		''' <returns> the <code>InternalFrameUI</code> object that renders
		'''          this component </returns>
		Public Overridable Property uI As InternalFrameUI
			Get
				Return CType(ui, InternalFrameUI)
			End Get
			Set(ByVal ui As InternalFrameUI)
				Dim checkingEnabled As Boolean = rootPaneCheckingEnabled
				Try
					rootPaneCheckingEnabled = False
					MyBase.uI = ui
				Finally
					rootPaneCheckingEnabled = checkingEnabled
				End Try
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the look and feel
		''' has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), InternalFrameUI)
			invalidate()
			If desktopIcon IsNot Nothing Then desktopIcon.updateUIWhenHidden()
		End Sub

	'     This method is called if <code>updateUI</code> was called
	'     * on the associated
	'     * JDesktopIcon.  It's necessary to avoid infinite recursion.
	'     
		Friend Overridable Sub updateUIWhenHidden()
			uI = CType(UIManager.getUI(Me), InternalFrameUI)
			invalidate()
			Dim children As Component() = components
			If children IsNot Nothing Then
				For Each child As Component In children
					SwingUtilities.updateComponentTreeUI(child)
				Next child
			End If
		End Sub


		''' <summary>
		''' Returns the name of the look-and-feel
		''' class that renders this component.
		''' </summary>
		''' <returns> the string "InternalFrameUI"
		''' </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' 
		''' @beaninfo
		'''     description: UIClassID </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Returns whether calls to <code>add</code> and
		''' <code>setLayout</code> are forwarded to the <code>contentPane</code>.
		''' </summary>
		''' <returns> true if <code>add</code> and <code>setLayout</code>
		'''         are forwarded; false otherwise
		''' </returns>
		''' <seealso cref= #addImpl </seealso>
		''' <seealso cref= #setLayout </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend Overridable Property rootPaneCheckingEnabled As Boolean
			Get
				Return rootPaneCheckingEnabled
			End Get
			Set(ByVal enabled As Boolean)
				rootPaneCheckingEnabled = enabled
			End Set
		End Property


		''' <summary>
		''' Adds the specified child <code>Component</code>.
		''' This method is overridden to conditionally forward calls to the
		''' <code>contentPane</code>.
		''' By default, children are added to the <code>contentPane</code> instead
		''' of the frame, refer to <seealso cref="javax.swing.RootPaneContainer"/> for
		''' details.
		''' </summary>
		''' <param name="comp"> the component to be enhanced </param>
		''' <param name="constraints"> the constraints to be respected </param>
		''' <param name="index"> the index </param>
		''' <exception cref="IllegalArgumentException"> if <code>index</code> is invalid </exception>
		''' <exception cref="IllegalArgumentException"> if adding the container's parent
		'''                  to itself </exception>
		''' <exception cref="IllegalArgumentException"> if adding a window to a container
		''' </exception>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			If rootPaneCheckingEnabled Then
				contentPane.add(comp, constraints, index)
			Else
				MyBase.addImpl(comp, constraints, index)
			End If
		End Sub

		''' <summary>
		''' Removes the specified component from the container. If
		''' <code>comp</code> is not a child of the <code>JInternalFrame</code>
		''' this will forward the call to the <code>contentPane</code>.
		''' </summary>
		''' <param name="comp"> the component to be removed </param>
		''' <exception cref="NullPointerException"> if <code>comp</code> is null </exception>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Public Overridable Sub remove(ByVal comp As Component)
			Dim oldCount As Integer = componentCount
			MyBase.remove(comp)
			If oldCount = componentCount Then contentPane.remove(comp)
		End Sub


		''' <summary>
		''' Ensures that, by default, the layout of this component cannot be set.
		''' Overridden to conditionally forward the call to the
		''' <code>contentPane</code>.
		''' Refer to <seealso cref="javax.swing.RootPaneContainer"/> for
		''' more information.
		''' </summary>
		''' <param name="manager"> the <code>LayoutManager</code> </param>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		Public Overridable Property layout As LayoutManager
			Set(ByVal manager As LayoutManager)
				If rootPaneCheckingEnabled Then
					contentPane.layout = manager
				Else
					MyBase.layout = manager
				End If
			End Set
		End Property


	'////////////////////////////////////////////////////////////////////////
	'/ Property Methods
	'////////////////////////////////////////////////////////////////////////

		''' <summary>
		''' Returns the current <code>JMenuBar</code> for this
		''' <code>JInternalFrame</code>, or <code>null</code>
		''' if no menu bar has been set. </summary>
		''' <returns> the current menu bar, or <code>null</code> if none has been set
		''' </returns>
		''' @deprecated As of Swing version 1.0.3,
		''' replaced by <code>getJMenuBar()</code>. 
		<Obsolete("As of Swing version 1.0.3,")> _
		Public Overridable Property menuBar As JMenuBar
			Get
			  Return rootPane.menuBar
			End Get
			Set(ByVal m As JMenuBar)
				Dim oldValue As JMenuBar = menuBar
				rootPane.jMenuBar = m
				firePropertyChange(MENU_BAR_PROPERTY, oldValue, m)
			End Set
		End Property

		''' <summary>
		''' Returns the current <code>JMenuBar</code> for this
		''' <code>JInternalFrame</code>, or <code>null</code>
		''' if no menu bar has been set.
		''' </summary>
		''' <returns>  the <code>JMenuBar</code> used by this internal frame </returns>
		''' <seealso cref= #setJMenuBar </seealso>
		Public Overridable Property jMenuBar As JMenuBar
			Get
				Return rootPane.jMenuBar
			End Get
			Set(ByVal m As JMenuBar)
				Dim oldValue As JMenuBar = menuBar
				rootPane.jMenuBar = m
				firePropertyChange(MENU_BAR_PROPERTY, oldValue, m)
			End Set
		End Property



		' implements javax.swing.RootPaneContainer
		''' <summary>
		''' Returns the content pane for this internal frame. </summary>
		''' <returns> the content pane </returns>
		Public Overridable Property contentPane As Container
			Get
				Return rootPane.contentPane
			End Get
			Set(ByVal c As Container)
				Dim oldValue As Container = contentPane
				rootPane.contentPane = c
				firePropertyChange(CONTENT_PANE_PROPERTY, oldValue, c)
			End Set
		End Property



		''' <summary>
		''' Returns the layered pane for this internal frame.
		''' </summary>
		''' <returns> a <code>JLayeredPane</code> object </returns>
		''' <seealso cref= RootPaneContainer#setLayeredPane </seealso>
		''' <seealso cref= RootPaneContainer#getLayeredPane </seealso>
		Public Overridable Property layeredPane As JLayeredPane Implements RootPaneContainer.getLayeredPane
			Get
				Return rootPane.layeredPane
			End Get
			Set(ByVal layered As JLayeredPane)
				Dim oldValue As JLayeredPane = layeredPane
				rootPane.layeredPane = layered
				firePropertyChange(LAYERED_PANE_PROPERTY, oldValue, layered)
			End Set
		End Property


		''' <summary>
		''' Returns the glass pane for this internal frame.
		''' </summary>
		''' <returns> the glass pane </returns>
		''' <seealso cref= RootPaneContainer#setGlassPane </seealso>
		Public Overridable Property glassPane As Component
			Get
				Return rootPane.glassPane
			End Get
			Set(ByVal glass As Component)
				Dim oldValue As Component = glassPane
				rootPane.glassPane = glass
				firePropertyChange(GLASS_PANE_PROPERTY, oldValue, glass)
			End Set
		End Property


		''' <summary>
		''' Returns the <code>rootPane</code> object for this internal frame.
		''' </summary>
		''' <returns> the <code>rootPane</code> property </returns>
		''' <seealso cref= RootPaneContainer#getRootPane </seealso>
		Public Property Overrides rootPane As JRootPane Implements RootPaneContainer.getRootPane
			Get
				Return rootPane
			End Get
			Set(ByVal root As JRootPane)
				If rootPane IsNot Nothing Then remove(rootPane)
				Dim oldValue As JRootPane = rootPane
				rootPane = root
				If rootPane IsNot Nothing Then
					Dim checkingEnabled As Boolean = rootPaneCheckingEnabled
					Try
						rootPaneCheckingEnabled = False
						add(rootPane, BorderLayout.CENTER)
					Finally
						rootPaneCheckingEnabled = checkingEnabled
					End Try
				End If
				firePropertyChange(ROOT_PANE_PROPERTY, oldValue, root)
			End Set
		End Property



		''' <summary>
		''' Sets whether this <code>JInternalFrame</code> can be closed by
		''' some user action. </summary>
		''' <param name="b"> a boolean value, where <code>true</code> means this internal frame can be closed
		''' @beaninfo
		'''     preferred: true
		'''           bound: true
		'''     description: Indicates whether this internal frame can be closed. </param>
		Public Overridable Property closable As Boolean
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean? = If(closable, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				closable = b
				firePropertyChange("closable", oldValue, newValue)
			End Set
			Get
				Return closable
			End Get
		End Property


		''' <summary>
		''' Returns whether this <code>JInternalFrame</code> is currently closed. </summary>
		''' <returns> <code>true</code> if this internal frame is closed, <code>false</code> otherwise </returns>
		Public Overridable Property closed As Boolean
			Get
				Return ___isClosed
			End Get
			Set(ByVal b As Boolean)
				If ___isClosed = b Then Return
    
				Dim oldValue As Boolean? = If(___isClosed, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				If b Then fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_CLOSING)
				fireVetoableChange(IS_CLOSED_PROPERTY, oldValue, newValue)
				___isClosed = b
				If ___isClosed Then visible = False
				firePropertyChange(IS_CLOSED_PROPERTY, oldValue, newValue)
				If ___isClosed Then
				  Dispose()
				ElseIf Not opened Then
		'           this bogus -- we haven't defined what
		'             setClosed(false) means. 
				  '        fireInternalFrameEvent(InternalFrameEvent.INTERNAL_FRAME_OPENED);
				  '            opened = true;
				End If
			End Set
		End Property


		''' <summary>
		''' Sets whether the <code>JInternalFrame</code> can be resized by some
		''' user action.
		''' </summary>
		''' <param name="b">  a boolean, where <code>true</code> means this internal frame can be resized
		''' @beaninfo
		'''     preferred: true
		'''           bound: true
		'''     description: Determines whether this internal frame can be resized
		'''                  by the user. </param>
		Public Overridable Property resizable As Boolean
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean? = If(resizable, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				resizable = b
				firePropertyChange("resizable", oldValue, newValue)
			End Set
			Get
				' don't allow resizing when maximized.
				Return If(___isMaximum, False, resizable)
			End Get
		End Property


		''' <summary>
		''' Sets the <code>iconable</code> property,
		''' which must be <code>true</code>
		''' for the user to be able to
		''' make the <code>JInternalFrame</code> an icon.
		''' Some look and feels might not implement iconification;
		''' they will ignore this property.
		''' </summary>
		''' <param name="b">  a boolean, where <code>true</code> means this internal frame can be iconified
		''' @beaninfo
		'''     preferred: true
		'''           bound: true
		'''     description: Determines whether this internal frame can be iconified. </param>
		Public Overridable Property iconifiable As Boolean
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean? = If(iconable, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				iconable = b
				firePropertyChange("iconable", oldValue, newValue)
			End Set
			Get
				Return iconable
			End Get
		End Property


		''' <summary>
		''' Returns whether the <code>JInternalFrame</code> is currently iconified.
		''' </summary>
		''' <returns> <code>true</code> if this internal frame is iconified </returns>
		Public Overridable Property icon As Boolean
			Get
				Return ___isIcon
			End Get
			Set(ByVal b As Boolean)
				If ___isIcon = b Then Return
    
		'         If an internal frame is being iconified before it has a
		'           parent, (e.g., client wants it to start iconic), create the
		'           parent if possible so that we can place the icon in its
		'           proper place on the desktop. I am not sure the call to
		'           validate() is necessary, since we are not going to display
		'           this frame yet 
				firePropertyChange("ancestor", Nothing, parent)
    
				Dim oldValue As Boolean? = If(___isIcon, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				fireVetoableChange(IS_ICON_PROPERTY, oldValue, newValue)
				___isIcon = b
				firePropertyChange(IS_ICON_PROPERTY, oldValue, newValue)
				If b Then
				  fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_ICONIFIED)
				Else
				  fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_DEICONIFIED)
				End If
			End Set
		End Property


		''' <summary>
		''' Sets the <code>maximizable</code> property,
		''' which determines whether the <code>JInternalFrame</code>
		''' can be maximized by
		''' some user action.
		''' Some look and feels might not support maximizing internal frames;
		''' they will ignore this property.
		''' </summary>
		''' <param name="b"> <code>true</code> to specify that this internal frame should be maximizable; <code>false</code> to specify that it should not be
		''' @beaninfo
		'''         bound: true
		'''     preferred: true
		'''     description: Determines whether this internal frame can be maximized. </param>
		Public Overridable Property maximizable As Boolean
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean? = If(maximizable, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				maximizable = b
				firePropertyChange("maximizable", oldValue, newValue)
			End Set
			Get
				Return maximizable
			End Get
		End Property


		''' <summary>
		''' Returns whether the <code>JInternalFrame</code> is currently maximized.
		''' </summary>
		''' <returns> <code>true</code> if this internal frame is maximized, <code>false</code> otherwise </returns>
		Public Overridable Property maximum As Boolean
			Get
				Return ___isMaximum
			End Get
			Set(ByVal b As Boolean)
				If ___isMaximum = b Then Return
    
				Dim oldValue As Boolean? = If(___isMaximum, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(b, Boolean.TRUE, Boolean.FALSE)
				fireVetoableChange(IS_MAXIMUM_PROPERTY, oldValue, newValue)
		'         setting isMaximum above the event firing means that
		'           property listeners that, for some reason, test it will
		'           get it wrong... See, for example, getNormalBounds() 
				___isMaximum = b
				firePropertyChange(IS_MAXIMUM_PROPERTY, oldValue, newValue)
			End Set
		End Property


		''' <summary>
		''' Returns the title of the <code>JInternalFrame</code>.
		''' </summary>
		''' <returns> a <code>String</code> containing this internal frame's title </returns>
		''' <seealso cref= #setTitle </seealso>
		Public Overridable Property title As String
			Get
				Return title
			End Get
			Set(ByVal title As String)
				Dim oldValue As String = Me.title
				Me.title = title
				firePropertyChange(TITLE_PROPERTY, oldValue, title)
			End Set
		End Property


		''' <summary>
		''' Selects or deselects the internal frame
		''' if it's showing.
		''' A <code>JInternalFrame</code> normally draws its title bar
		''' differently if it is
		''' the selected frame, which indicates to the user that this
		''' internal frame has the focus.
		''' When this method changes the state of the internal frame
		''' from deselected to selected, it fires an
		''' <code>InternalFrameEvent.INTERNAL_FRAME_ACTIVATED</code> event.
		''' If the change is from selected to deselected,
		''' an <code>InternalFrameEvent.INTERNAL_FRAME_DEACTIVATED</code> event
		''' is fired.
		''' </summary>
		''' <param name="selected">  a boolean, where <code>true</code> means this internal frame
		'''                  should become selected (currently active)
		'''                  and <code>false</code> means it should become deselected </param>
		''' <exception cref="PropertyVetoException"> when the attempt to set the
		'''            property is vetoed by the <code>JInternalFrame</code>
		''' </exception>
		''' <seealso cref= #isShowing </seealso>
		''' <seealso cref= InternalFrameEvent#INTERNAL_FRAME_ACTIVATED </seealso>
		''' <seealso cref= InternalFrameEvent#INTERNAL_FRAME_DEACTIVATED
		''' 
		''' @beaninfo
		'''     constrained: true
		'''           bound: true
		'''     description: Indicates whether this internal frame is currently
		'''                  the active frame. </seealso>
		Public Overridable Property selected As Boolean
			Set(ByVal selected As Boolean)
			   ' The InternalFrame may already be selected, but the focus
			   ' may be outside it, so restore the focus to the subcomponent
			   ' which previously had it. See Bug 4302764.
				If selected AndAlso ___isSelected Then
					restoreSubcomponentFocus()
					Return
				End If
				' The internal frame or the desktop icon must be showing to allow
				' selection.  We may deselect even if neither is showing.
				If (___isSelected = selected) OrElse (selected AndAlso (If(___isIcon, (Not desktopIcon.showing), (Not showing)))) Then Return
    
				Dim oldValue As Boolean? = If(___isSelected, Boolean.TRUE, Boolean.FALSE)
				Dim newValue As Boolean? = If(selected, Boolean.TRUE, Boolean.FALSE)
				fireVetoableChange(IS_SELECTED_PROPERTY, oldValue, newValue)
    
		'         We don't want to leave focus in the previously selected
		'           frame, so we have to set it to *something* in case it
		'           doesn't get set in some other way (as if a user clicked on
		'           a component that doesn't request focus).  If this call is
		'           happening because the user clicked on a component that will
		'           want focus, then it will get transfered there later.
		'
		'           We test for parent.isShowing() above, because AWT throws a
		'           NPE if you try to request focus on a lightweight before its
		'           parent has been made visible 
    
				If selected Then restoreSubcomponentFocus()
    
				___isSelected = selected
				firePropertyChange(IS_SELECTED_PROPERTY, oldValue, newValue)
				If ___isSelected Then
				  fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_ACTIVATED)
				Else
				  fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_DEACTIVATED)
				End If
				repaint()
			End Set
			Get
				Return ___isSelected
			End Get
		End Property


		''' <summary>
		''' Sets an image to be displayed in the titlebar of this internal frame (usually
		''' in the top-left corner).
		''' This image is not the <code>desktopIcon</code> object, which
		''' is the image displayed in the <code>JDesktop</code> when
		''' this internal frame is iconified.
		''' 
		''' Passing <code>null</code> to this function is valid,
		''' but the look and feel
		''' can choose the
		''' appropriate behavior for that situation, such as displaying no icon
		''' or a default icon for the look and feel.
		''' </summary>
		''' <param name="icon"> the <code>Icon</code> to display in the title bar </param>
		''' <seealso cref= #getFrameIcon
		''' @beaninfo
		'''           bound: true
		'''     description: The icon shown in the top-left corner of this internal frame. </seealso>
	  Public Overridable Property frameIcon As Icon
		  Set(ByVal icon As Icon)
				Dim oldIcon As Icon = frameIcon
				frameIcon = icon
				firePropertyChange(FRAME_ICON_PROPERTY, oldIcon, icon)
		  End Set
		  Get
				Return frameIcon
			End Get
	  End Property


		''' <summary>
		''' Convenience method that moves this component to position 0 if its
		''' parent is a <code>JLayeredPane</code>.
		''' </summary>
		Public Overridable Sub moveToFront()
			If icon Then
				If TypeOf desktopIcon.parent Is JLayeredPane Then CType(desktopIcon.parent, JLayeredPane).moveToFront(desktopIcon)
			ElseIf TypeOf parent Is JLayeredPane Then
				CType(parent, JLayeredPane).moveToFront(Me)
			End If
		End Sub

		''' <summary>
		''' Convenience method that moves this component to position -1 if its
		''' parent is a <code>JLayeredPane</code>.
		''' </summary>
		Public Overridable Sub moveToBack()
			If icon Then
				If TypeOf desktopIcon.parent Is JLayeredPane Then CType(desktopIcon.parent, JLayeredPane).moveToBack(desktopIcon)
			ElseIf TypeOf parent Is JLayeredPane Then
				CType(parent, JLayeredPane).moveToBack(Me)
			End If
		End Sub

		''' <summary>
		''' Returns the last <code>Cursor</code> that was set by the
		''' <code>setCursor</code> method that is not a resizable
		''' <code>Cursor</code>.
		''' </summary>
		''' <returns> the last non-resizable <code>Cursor</code>
		''' @since 1.6 </returns>
		Public Overridable Property lastCursor As Cursor
			Get
				Return lastCursor
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overridable Property cursor As Cursor
			Set(ByVal cursor As Cursor)
				If cursor Is Nothing Then
					lastCursor = Nothing
					MyBase.cursor = cursor
					Return
				End If
				Dim type As Integer = cursor.type
				If Not(type = Cursor.SW_RESIZE_CURSOR OrElse type = Cursor.SE_RESIZE_CURSOR OrElse type = Cursor.NW_RESIZE_CURSOR OrElse type = Cursor.NE_RESIZE_CURSOR OrElse type = Cursor.N_RESIZE_CURSOR OrElse type = Cursor.S_RESIZE_CURSOR OrElse type = Cursor.W_RESIZE_CURSOR OrElse type = Cursor.E_RESIZE_CURSOR) Then lastCursor = cursor
				MyBase.cursor = cursor
			End Set
		End Property

		''' <summary>
		''' Convenience method for setting the layer attribute of this component.
		''' </summary>
		''' <param name="layer">  an <code>Integer</code> object specifying this
		'''          frame's desktop layer </param>
		''' <seealso cref= JLayeredPane
		''' @beaninfo
		'''     expert: true
		'''     description: Specifies what desktop layer is used. </seealso>
		Public Overridable Property layer As Integer?
			Set(ByVal layer As Integer?)
				If parent IsNot Nothing AndAlso TypeOf parent Is JLayeredPane Then
					' Normally we want to do this, as it causes the LayeredPane
					' to draw properly.
					Dim p As JLayeredPane = CType(parent, JLayeredPane)
					p.layeryer(Me, layer, p.getPosition(Me))
				Else
					 ' Try to do the right thing
					 JLayeredPane.putLayer(Me, layer)
					 If parent IsNot Nothing Then parent.repaint(x, y, width, height)
				End If
			End Set
			Get
				Return JLayeredPane.getLayer(Me)
			End Get
		End Property

		''' <summary>
		''' Convenience method for setting the layer attribute of this component.
		''' The method <code>setLayer(Integer)</code> should be used for
		''' layer values predefined in <code>JLayeredPane</code>.
		''' When using <code>setLayer(int)</code>, care must be taken not to
		''' accidentally clash with those values.
		''' </summary>
		''' <param name="layer">  an integer specifying this internal frame's desktop layer
		''' 
		''' @since 1.3
		''' </param>
		''' <seealso cref= #setLayer(Integer) </seealso>
		''' <seealso cref= JLayeredPane
		''' @beaninfo
		'''     expert: true
		'''     description: Specifies what desktop layer is used. </seealso>
		Public Overridable Property layer As Integer
			Set(ByVal layer As Integer)
			  Me.layer = Convert.ToInt32(layer)
			End Set
		End Property


		''' <summary>
		''' Convenience method that searches the ancestor hierarchy for a
		''' <code>JDesktop</code> instance. If <code>JInternalFrame</code>
		''' finds none, the <code>desktopIcon</code> tree is searched.
		''' </summary>
		''' <returns> the <code>JDesktopPane</code> this internal frame belongs to,
		'''         or <code>null</code> if none is found </returns>
		Public Overridable Property desktopPane As JDesktopPane
			Get
				Dim p As Container
    
				' Search upward for desktop
				p = parent
				Do While p IsNot Nothing AndAlso Not(TypeOf p Is JDesktopPane)
					p = p.parent
				Loop
    
				If p Is Nothing Then
				   ' search its icon parent for desktop
				   p = desktopIcon.parent
				   Do While p IsNot Nothing AndAlso Not(TypeOf p Is JDesktopPane)
						p = p.parent
				   Loop
				End If
    
				Return CType(p, JDesktopPane)
			End Get
		End Property

		''' <summary>
		''' Sets the <code>JDesktopIcon</code> associated with this
		''' <code>JInternalFrame</code>.
		''' </summary>
		''' <param name="d"> the <code>JDesktopIcon</code> to display on the desktop </param>
		''' <seealso cref= #getDesktopIcon
		''' @beaninfo
		'''           bound: true
		'''     description: The icon shown when this internal frame is minimized. </seealso>
		Public Overridable Property desktopIcon As JDesktopIcon
			Set(ByVal d As JDesktopIcon)
				Dim oldValue As JDesktopIcon = desktopIcon
				desktopIcon = d
				firePropertyChange("desktopIcon", oldValue, d)
			End Set
			Get
				Return desktopIcon
			End Get
		End Property


		''' <summary>
		''' If the <code>JInternalFrame</code> is not in maximized state, returns
		''' <code>getBounds()</code>; otherwise, returns the bounds that the
		''' <code>JInternalFrame</code> would be restored to.
		''' </summary>
		''' <returns> a <code>Rectangle</code> containing the bounds of this
		'''          frame when in the normal state
		''' @since 1.3 </returns>
		Public Overridable Property normalBounds As Rectangle
			Get
    
		'       we used to test (!isMaximum) here, but since this
		'         method is used by the property listener for the
		'         IS_MAXIMUM_PROPERTY, it ended up getting the wrong
		'         answer... Since normalBounds get set to null when the
		'         frame is restored, this should work better 
    
			  If normalBounds IsNot Nothing Then
				Return normalBounds
			  Else
				Return bounds
			  End If
			End Get
			Set(ByVal r As Rectangle)
				normalBounds = r
			End Set
		End Property


		''' <summary>
		''' If this <code>JInternalFrame</code> is active,
		''' returns the child that has focus.
		''' Otherwise, returns <code>null</code>.
		''' </summary>
		''' <returns> the component with focus, or <code>null</code> if no children have focus
		''' @since 1.3 </returns>
		Public Overridable Property focusOwner As Component
			Get
				If selected Then Return lastFocusOwner
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the child component of this <code>JInternalFrame</code>
		''' that will receive the
		''' focus when this <code>JInternalFrame</code> is selected.
		''' If this <code>JInternalFrame</code> is
		''' currently selected, this method returns the same component as
		''' the <code>getFocusOwner</code> method.
		''' If this <code>JInternalFrame</code> is not selected,
		''' then the child component that most recently requested focus will be
		''' returned. If no child component has ever requested focus, then this
		''' <code>JInternalFrame</code>'s initial focusable component is returned.
		''' If no such
		''' child exists, then this <code>JInternalFrame</code>'s default component
		''' to focus is returned.
		''' </summary>
		''' <returns> the child component that will receive focus when this
		'''         <code>JInternalFrame</code> is selected </returns>
		''' <seealso cref= #getFocusOwner </seealso>
		''' <seealso cref= #isSelected
		''' @since 1.4 </seealso>
		Public Overridable Property mostRecentFocusOwner As Component
			Get
				If selected Then Return focusOwner
    
				If lastFocusOwner IsNot Nothing Then Return lastFocusOwner
    
				Dim policy As FocusTraversalPolicy = focusTraversalPolicy
				If TypeOf policy Is InternalFrameFocusTraversalPolicy Then Return CType(policy, InternalFrameFocusTraversalPolicy).getInitialComponent(Me)
    
				Dim toFocus As Component = policy.getDefaultComponent(Me)
				If toFocus IsNot Nothing Then Return toFocus
				Return contentPane
			End Get
		End Property

		''' <summary>
		''' Requests the internal frame to restore focus to the
		''' last subcomponent that had focus. This is used by the UI when
		''' the user selected this internal frame --
		''' for example, by clicking on the title bar.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Sub restoreSubcomponentFocus()
			If icon Then
				sun.swing.SwingUtilities2.compositeRequestFocus(desktopIcon)
			Else
				Dim component As Component = KeyboardFocusManager.currentKeyboardFocusManager.permanentFocusOwner
				If (component Is Nothing) OrElse (Not SwingUtilities.isDescendingFrom(component, Me)) Then
					' FocusPropertyChangeListener will eventually update
					' lastFocusOwner. As focus requests are asynchronous
					' lastFocusOwner may be accessed before it has been correctly
					' updated. To avoid any problems, lastFocusOwner is immediately
					' set, assuming the request will succeed.
					lastFocusOwner = mostRecentFocusOwner
					If lastFocusOwner Is Nothing Then lastFocusOwner = contentPane
					lastFocusOwner.requestFocus()
				End If
			End If
		End Sub

		Private Property lastFocusOwner As Component
			Set(ByVal component As Component)
				lastFocusOwner = component
			End Set
		End Property

		''' <summary>
		''' Moves and resizes this component.  Unlike other components,
		''' this implementation also forces re-layout, so that frame
		''' decorations such as the title bar are always redisplayed.
		''' </summary>
		''' <param name="x">  an integer giving the component's new horizontal position
		'''           measured in pixels from the left of its container </param>
		''' <param name="y">  an integer giving the component's new vertical position,
		'''           measured in pixels from the bottom of its container </param>
		''' <param name="width">  an integer giving the component's new width in pixels </param>
		''' <param name="height"> an integer giving the component's new height in pixels </param>
		Public Overrides Sub reshape(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			MyBase.reshape(x, y, width, height)
			validate()
			repaint()
		End Sub

	'/////////////////////////
	' Frame/Window equivalents
	'/////////////////////////

		''' <summary>
		''' Adds the specified listener to receive internal
		''' frame events from this internal frame.
		''' </summary>
		''' <param name="l"> the internal frame listener </param>
		Public Overridable Sub addInternalFrameListener(ByVal l As javax.swing.event.InternalFrameListener) ' remind: sync ??
		  listenerList.add(GetType(javax.swing.event.InternalFrameListener), l)
		  ' remind: needed?
		  enableEvents(0) ' turn on the newEventsOnly flag in Component.
		End Sub

		''' <summary>
		''' Removes the specified internal frame listener so that it no longer
		''' receives internal frame events from this internal frame.
		''' </summary>
		''' <param name="l"> the internal frame listener </param>
		Public Overridable Sub removeInternalFrameListener(ByVal l As javax.swing.event.InternalFrameListener) ' remind: sync??
		  listenerList.remove(GetType(javax.swing.event.InternalFrameListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>InternalFrameListener</code>s added
		''' to this <code>JInternalFrame</code> with
		''' <code>addInternalFrameListener</code>.
		''' </summary>
		''' <returns> all of the <code>InternalFrameListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4
		''' </returns>
		''' <seealso cref= #addInternalFrameListener </seealso>
		Public Overridable Property internalFrameListeners As javax.swing.event.InternalFrameListener()
			Get
				Return listenerList.getListeners(GetType(javax.swing.event.InternalFrameListener))
			End Get
		End Property

		' remind: name ok? all one method ok? need to be synchronized?
		''' <summary>
		''' Fires an internal frame event.
		''' </summary>
		''' <param name="id">  the type of the event being fired; one of the following:
		''' <ul>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_OPENED</code>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_CLOSING</code>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_CLOSED</code>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_ICONIFIED</code>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_DEICONIFIED</code>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_ACTIVATED</code>
		''' <li><code>InternalFrameEvent.INTERNAL_FRAME_DEACTIVATED</code>
		''' </ul>
		''' If the event type is not one of the above, nothing happens. </param>
		Protected Friend Overridable Sub fireInternalFrameEvent(ByVal id As Integer)
		  Dim ___listeners As Object() = listenerList.listenerList
		  Dim e As javax.swing.event.InternalFrameEvent = Nothing
		  For i As Integer = ___listeners.Length -2 To 0 Step -2
			If ___listeners(i) Is GetType(javax.swing.event.InternalFrameListener) Then
			  If e Is Nothing Then e = New javax.swing.event.InternalFrameEvent(Me, id)
			  Select Case e.iD
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_OPENED
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameOpened(e)
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_CLOSING
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameClosing(e)
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_CLOSED
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameClosed(e)
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_ICONIFIED
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameIconified(e)
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_DEICONIFIED
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameDeiconified(e)
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_ACTIVATED
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameActivated(e)
			  Case javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_DEACTIVATED
				CType(___listeners(i+1), javax.swing.event.InternalFrameListener).internalFrameDeactivated(e)
			  Case Else
			  End Select
			End If
		  Next i
	'       we could do it off the event, but at the moment, that's not how
	'         I'm implementing it 
		  '      if (id == InternalFrameEvent.INTERNAL_FRAME_CLOSING) {
		  '          doDefaultCloseAction();
		  '      }
		End Sub

		''' <summary>
		''' Fires an
		''' <code>INTERNAL_FRAME_CLOSING</code> event
		''' and then performs the action specified by
		''' the internal frame's default close operation.
		''' This method is typically invoked by the
		''' look-and-feel-implemented action handler
		''' for the internal frame's close button.
		''' 
		''' @since 1.3 </summary>
		''' <seealso cref= #setDefaultCloseOperation </seealso>
		''' <seealso cref= javax.swing.event.InternalFrameEvent#INTERNAL_FRAME_CLOSING </seealso>
		Public Overridable Sub doDefaultCloseAction()
			fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_CLOSING)
			Select Case defaultCloseOperation
			  Case DO_NOTHING_ON_CLOSE
			  Case HIDE_ON_CLOSE
				visible = False
				If selected Then
					Try
						selected = False
					Catch pve As java.beans.PropertyVetoException
					End Try
				End If

	'             should this activate the next frame? that's really
	'               desktopmanager's policy... 
			  Case DISPOSE_ON_CLOSE
				  Try
					fireVetoableChange(IS_CLOSED_PROPERTY, Boolean.FALSE, Boolean.TRUE)
					___isClosed = True
					visible = False
					firePropertyChange(IS_CLOSED_PROPERTY, Boolean.FALSE, Boolean.TRUE)
					Dispose()
				  Catch pve As java.beans.PropertyVetoException
				  End Try
			  Case Else
			End Select
		End Sub

		''' <summary>
		''' Sets the operation that will happen by default when
		''' the user initiates a "close" on this internal frame.
		''' The possible choices are:
		''' <br><br>
		''' <dl>
		''' <dt><code>DO_NOTHING_ON_CLOSE</code>
		''' <dd> Do nothing.
		'''      This requires the program to handle the operation
		'''      in the <code>internalFrameClosing</code> method
		'''      of a registered <code>InternalFrameListener</code> object.
		''' <dt><code>HIDE_ON_CLOSE</code>
		''' <dd> Automatically make the internal frame invisible.
		''' <dt><code>DISPOSE_ON_CLOSE</code>
		''' <dd> Automatically dispose of the internal frame.
		''' </dl>
		''' <p>
		''' The default value is <code>DISPOSE_ON_CLOSE</code>.
		''' Before performing the specified close operation,
		''' the internal frame fires
		''' an <code>INTERNAL_FRAME_CLOSING</code> event.
		''' </summary>
		''' <param name="operation"> one of the following constants defined in
		'''                  <code>javax.swing.WindowConstants</code>
		'''                  (an interface implemented by
		'''                  <code>JInternalFrame</code>):
		'''                  <code>DO_NOTHING_ON_CLOSE</code>,
		'''                  <code>HIDE_ON_CLOSE</code>, or
		'''                  <code>DISPOSE_ON_CLOSE</code>
		''' </param>
		''' <seealso cref= #addInternalFrameListener </seealso>
		''' <seealso cref= #getDefaultCloseOperation </seealso>
		''' <seealso cref= #setVisible </seealso>
		''' <seealso cref= #dispose </seealso>
		''' <seealso cref= InternalFrameEvent#INTERNAL_FRAME_CLOSING </seealso>
		Public Overridable Property defaultCloseOperation As Integer
			Set(ByVal operation As Integer)
				Me.defaultCloseOperation = operation
			End Set
			Get
				Return defaultCloseOperation
			End Get
		End Property


		''' <summary>
		''' Causes subcomponents of this <code>JInternalFrame</code>
		''' to be laid out at their preferred size.  Internal frames that are
		''' iconized or maximized are first restored and then packed.  If the
		''' internal frame is unable to be restored its state is not changed
		''' and will not be packed.
		''' </summary>
		''' <seealso cref=       java.awt.Window#pack </seealso>
		Public Overridable Sub pack()
			Try
				If icon Then
					icon = False
				ElseIf maximum Then
					maximum = False
				End If
			Catch e As java.beans.PropertyVetoException
				Return
			End Try
			size = preferredSize
			validate()
		End Sub

		''' <summary>
		''' If the internal frame is not visible,
		''' brings the internal frame to the front,
		''' makes it visible,
		''' and attempts to select it.
		''' The first time the internal frame is made visible,
		''' this method also fires an <code>INTERNAL_FRAME_OPENED</code> event.
		''' This method does nothing if the internal frame is already visible.
		''' Invoking this method
		''' has the same result as invoking
		''' <code>setVisible(true)</code>.
		''' </summary>
		''' <seealso cref= #moveToFront </seealso>
		''' <seealso cref= #setSelected </seealso>
		''' <seealso cref= InternalFrameEvent#INTERNAL_FRAME_OPENED </seealso>
		''' <seealso cref= #setVisible </seealso>
		Public Overridable Sub show()
			' bug 4312922
			If visible Then Return

			' bug 4149505
			If Not opened Then
			  fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_OPENED)
			  opened = True
			End If

	'         icon default visibility is false; set it to true so that it shows
	'           up when user iconifies frame 
			desktopIcon.visible = True

			toFront()
			MyBase.show()

			If ___isIcon Then Return

			If Not selected Then
				Try
					selected = True
				Catch pve As java.beans.PropertyVetoException
				End Try
			End If
		End Sub

		Public Overrides Sub hide()
			If icon Then desktopIcon.visible = False
			MyBase.hide()
		End Sub

		''' <summary>
		''' Makes this internal frame
		''' invisible, unselected, and closed.
		''' If the frame is not already closed,
		''' this method fires an
		''' <code>INTERNAL_FRAME_CLOSED</code> event.
		''' The results of invoking this method are similar to
		''' <code>setClosed(true)</code>,
		''' but <code>dispose</code> always succeeds in closing
		''' the internal frame and does not fire
		''' an <code>INTERNAL_FRAME_CLOSING</code> event.
		''' </summary>
		''' <seealso cref= javax.swing.event.InternalFrameEvent#INTERNAL_FRAME_CLOSED </seealso>
		''' <seealso cref= #setVisible </seealso>
		''' <seealso cref= #setSelected </seealso>
		''' <seealso cref= #setClosed </seealso>
		Public Overridable Sub dispose()
			If visible Then visible = False
			If selected Then
				Try
					selected = False
				Catch pve As java.beans.PropertyVetoException
				End Try
			End If
			If Not ___isClosed Then
			  firePropertyChange(IS_CLOSED_PROPERTY, Boolean.FALSE, Boolean.TRUE)
			  ___isClosed = True
			End If
			fireInternalFrameEvent(javax.swing.event.InternalFrameEvent.INTERNAL_FRAME_CLOSED)
		End Sub

		''' <summary>
		''' Brings this internal frame to the front.
		''' Places this internal frame  at the top of the stacking order
		''' and makes the corresponding adjustment to other visible internal
		''' frames.
		''' </summary>
		''' <seealso cref=       java.awt.Window#toFront </seealso>
		''' <seealso cref=       #moveToFront </seealso>
		Public Overridable Sub toFront()
			moveToFront()
		End Sub

		''' <summary>
		''' Sends this internal frame to the back.
		''' Places this internal frame at the bottom of the stacking order
		''' and makes the corresponding adjustment to other visible
		''' internal frames.
		''' </summary>
		''' <seealso cref=       java.awt.Window#toBack </seealso>
		''' <seealso cref=       #moveToBack </seealso>
		Public Overridable Sub toBack()
			moveToBack()
		End Sub

		''' <summary>
		''' Does nothing because <code>JInternalFrame</code>s must always be roots of a focus
		''' traversal cycle.
		''' </summary>
		''' <param name="focusCycleRoot"> this value is ignored </param>
		''' <seealso cref= #isFocusCycleRoot </seealso>
		''' <seealso cref= java.awt.Container#setFocusTraversalPolicy </seealso>
		''' <seealso cref= java.awt.Container#getFocusTraversalPolicy
		''' @since 1.4 </seealso>
		Public Property focusCycleRoot As Boolean
			Set(ByVal focusCycleRoot As Boolean)
			End Set
			Get
				Return True
			End Get
		End Property


		''' <summary>
		''' Always returns <code>null</code> because <code>JInternalFrame</code>s
		''' must always be roots of a focus
		''' traversal cycle.
		''' </summary>
		''' <returns> <code>null</code> </returns>
		''' <seealso cref= java.awt.Container#isFocusCycleRoot()
		''' @since 1.4 </seealso>
		Public Property focusCycleRootAncestor As Container
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the warning string that is displayed with this internal frame.
		''' Since an internal frame is always secure (since it's fully
		''' contained within a window that might need a warning string)
		''' this method always returns <code>null</code>. </summary>
		''' <returns>    <code>null</code> </returns>
		''' <seealso cref=       java.awt.Window#getWarningString </seealso>
		Public Property warningString As String
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code>
		''' in <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then
					Dim old As Boolean = rootPaneCheckingEnabled
					Try
						rootPaneCheckingEnabled = False
						ui.installUI(Me)
					Finally
						rootPaneCheckingEnabled = old
					End Try
				End If
			End If
		End Sub

	'     Called from the JComponent's EnableSerializationFocusListener to
	'     * do any Swing-specific pre-serialization configuration.
	'     
		Friend Overrides Sub compWriteObjectNotify()
		  ' need to disable rootpane checking for InternalFrame: 4172083
		  Dim old As Boolean = rootPaneCheckingEnabled
		  Try
			rootPaneCheckingEnabled = False
			MyBase.compWriteObjectNotify()
		  Finally
			rootPaneCheckingEnabled = old
		  End Try
		End Sub

		''' <summary>
		''' Returns a string representation of this <code>JInternalFrame</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JInternalFrame</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim rootPaneString As String = (If(rootPane IsNot Nothing, rootPane.ToString(), ""))
			Dim rootPaneCheckingEnabledString As String = (If(rootPaneCheckingEnabled, "true", "false"))
			Dim closableString As String = (If(closable, "true", "false"))
			Dim isClosedString As String = (If(___isClosed, "true", "false"))
			Dim maximizableString As String = (If(maximizable, "true", "false"))
			Dim isMaximumString As String = (If(___isMaximum, "true", "false"))
			Dim iconableString As String = (If(iconable, "true", "false"))
			Dim isIconString As String = (If(___isIcon, "true", "false"))
			Dim resizableString As String = (If(resizable, "true", "false"))
			Dim isSelectedString As String = (If(___isSelected, "true", "false"))
			Dim frameIconString As String = (If(frameIcon IsNot Nothing, frameIcon.ToString(), ""))
			Dim titleString As String = (If(title IsNot Nothing, title, ""))
			Dim desktopIconString As String = (If(desktopIcon IsNot Nothing, desktopIcon.ToString(), ""))
			Dim openedString As String = (If(opened, "true", "false"))
			Dim defaultCloseOperationString As String
			If defaultCloseOperation = HIDE_ON_CLOSE Then
				defaultCloseOperationString = "HIDE_ON_CLOSE"
			ElseIf defaultCloseOperation = DISPOSE_ON_CLOSE Then
				defaultCloseOperationString = "DISPOSE_ON_CLOSE"
			ElseIf defaultCloseOperation = DO_NOTHING_ON_CLOSE Then
				defaultCloseOperationString = "DO_NOTHING_ON_CLOSE"
			Else
				defaultCloseOperationString = ""
			End If

			Return MyBase.paramString() & ",closable=" & closableString & ",defaultCloseOperation=" & defaultCloseOperationString & ",desktopIcon=" & desktopIconString & ",frameIcon=" & frameIconString & ",iconable=" & iconableString & ",isClosed=" & isClosedString & ",isIcon=" & isIconString & ",isMaximum=" & isMaximumString & ",isSelected=" & isSelectedString & ",maximizable=" & maximizableString & ",opened=" & openedString & ",resizable=" & resizableString & ",rootPane=" & rootPaneString & ",rootPaneCheckingEnabled=" & rootPaneCheckingEnabledString & ",title=" & titleString
		End Function

		' ======= begin optimized frame dragging defence code ==============

		Friend isDragging As Boolean = False
		Friend danger As Boolean = False

		''' <summary>
		''' Overridden to allow optimized painting when the
		''' internal frame is being dragged.
		''' </summary>
		Protected Friend Overrides Sub paintComponent(ByVal g As Graphics)
		  If isDragging Then danger = True

		  MyBase.paintComponent(g)
		End Sub

		' ======= end optimized frame dragging defence code ==============

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JInternalFrame</code>.
		''' For internal frames, the <code>AccessibleContext</code>
		''' takes the form of an
		''' <code>AccessibleJInternalFrame</code> object.
		''' A new <code>AccessibleJInternalFrame</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJInternalFrame</code> that serves as the
		'''         <code>AccessibleContext</code> of this
		'''         <code>JInternalFrame</code> </returns>
		''' <seealso cref= AccessibleJInternalFrame </seealso>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJInternalFrame(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JInternalFrame</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to internal frame user-interface
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
		Protected Friend Class AccessibleJInternalFrame
			Inherits AccessibleJComponent
			Implements AccessibleValue

			Private ReadOnly outerInstance As JInternalFrame

			Public Sub New(ByVal outerInstance As JInternalFrame)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be <code>null</code> if this
			''' object does not have a name </returns>
			''' <seealso cref= #setAccessibleName </seealso>
			Public Overridable Property accessibleName As String
				Get
					Dim name As String = accessibleName
    
					If name Is Nothing Then name = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
					If name Is Nothing Then name = outerInstance.title
					Return name
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.INTERNAL_FRAME
				End Get
			End Property

			''' <summary>
			''' Gets the AccessibleValue associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' returns this object, which is responsible for implementing the
			''' <code>AccessibleValue</code> interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleValue As AccessibleValue
				Get
					Return Me
				End Get
			End Property


			'
			' AccessibleValue methods
			'

			''' <summary>
			''' Get the value of this object as a Number.
			''' </summary>
			''' <returns> value of the object -- can be <code>null</code> if this object does not
			''' have a value </returns>
			Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.layer)
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <returns> <code>true</code> if the value was set </returns>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
				' TIGER - 4422535
				If n Is Nothing Then Return False
				outerInstance.layer = New Integer?(n)
				Return True
			End Function

			''' <summary>
			''' Get the minimum value of this object as a Number.
			''' </summary>
			''' <returns> Minimum value of the object; <code>null</code> if this object does not
			''' have a minimum value </returns>
			Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
				Get
					Return Integer.MinValue
				End Get
			End Property

			''' <summary>
			''' Get the maximum value of this object as a Number.
			''' </summary>
			''' <returns> Maximum value of the object; <code>null</code> if this object does not
			''' have a maximum value </returns>
			Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
				Get
					Return Integer.MaxValue
				End Get
			End Property

		End Class ' AccessibleJInternalFrame

		''' <summary>
		''' This component represents an iconified version of a
		''' <code>JInternalFrame</code>.
		''' This API should NOT BE USED by Swing applications, as it will go
		''' away in future versions of Swing as its functionality is moved into
		''' <code>JInternalFrame</code>.  This class is public only so that
		''' UI objects can display a desktop icon.  If an application
		''' wants to display a desktop icon, it should create a
		''' <code>JInternalFrame</code> instance and iconify it.
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
		Public Class JDesktopIcon
			Inherits JComponent
			Implements Accessible

			Friend internalFrame As JInternalFrame

			''' <summary>
			''' Creates an icon for an internal frame.
			''' </summary>
			''' <param name="f">  the <code>JInternalFrame</code>
			'''              for which the icon is created </param>
			Public Sub New(ByVal f As JInternalFrame)
				visible = False
				internalFrame = f
				updateUI()
			End Sub

			''' <summary>
			''' Returns the look-and-feel object that renders this component.
			''' </summary>
			''' <returns> the <code>DesktopIconUI</code> object that renders
			'''              this component </returns>
			Public Overridable Property uI As DesktopIconUI
				Get
					Return CType(ui, DesktopIconUI)
				End Get
				Set(ByVal ui As DesktopIconUI)
					MyBase.uI = ui
				End Set
			End Property


			''' <summary>
			''' Returns the <code>JInternalFrame</code> that this
			''' <code>DesktopIcon</code> is associated with.
			''' </summary>
			''' <returns> the <code>JInternalFrame</code> with which this icon
			'''              is associated </returns>
			Public Overridable Property internalFrame As JInternalFrame
				Get
					Return internalFrame
				End Get
				Set(ByVal f As JInternalFrame)
					internalFrame = f
				End Set
			End Property


			''' <summary>
			''' Convenience method to ask the icon for the <code>Desktop</code>
			''' object it belongs to.
			''' </summary>
			''' <returns> the <code>JDesktopPane</code> that contains this
			'''           icon's internal frame, or <code>null</code> if none found </returns>
			Public Overridable Property desktopPane As JDesktopPane
				Get
					If internalFrame IsNot Nothing Then Return internalFrame.desktopPane
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Notification from the <code>UIManager</code> that the look and feel
			''' has changed.
			''' Replaces the current UI object with the latest version from the
			''' <code>UIManager</code>.
			''' </summary>
			''' <seealso cref= JComponent#updateUI </seealso>
			Public Overrides Sub updateUI()
				Dim hadUI As Boolean = (ui IsNot Nothing)
				uI = CType(UIManager.getUI(Me), DesktopIconUI)
				invalidate()

				Dim r As Dimension = preferredSize
				sizeize(r.width, r.height)


				If internalFrame IsNot Nothing AndAlso internalFrame.uI IsNot Nothing Then ' don't do this if UI not created yet SwingUtilities.updateComponentTreeUI(internalFrame)
			End Sub

	'         This method is called if updateUI was called on the associated
	'         * JInternalFrame.  It's necessary to avoid infinite recursion.
	'         
			Friend Overridable Sub updateUIWhenHidden()
				' Update this UI and any associated internal frame 
				uI = CType(UIManager.getUI(Me), DesktopIconUI)

				Dim r As Dimension = preferredSize
				sizeize(r.width, r.height)

				invalidate()
				Dim children As Component() = components
				If children IsNot Nothing Then
					For Each child As Component In children
						SwingUtilities.updateComponentTreeUI(child)
					Next child
				End If
			End Sub

			''' <summary>
			''' Returns the name of the look-and-feel
			''' class that renders this component.
			''' </summary>
			''' <returns> the string "DesktopIconUI" </returns>
			''' <seealso cref= JComponent#getUIClassID </seealso>
			''' <seealso cref= UIDefaults#getUI </seealso>
			Public Property Overrides uIClassID As String
				Get
					Return "DesktopIconUI"
				End Get
			End Property
			'//////////////
			' Serialization support
			'//////////////
			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				s.defaultWriteObject()
				If uIClassID.Equals("DesktopIconUI") Then
					Dim count As SByte = JComponent.getWriteObjCounter(Me)
					count -= 1
					JComponent.writeObjCounterter(Me, count)
					If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
				End If
			End Sub

		   '///////////////
		   ' Accessibility support
		   '//////////////

			''' <summary>
			''' Gets the AccessibleContext associated with this JDesktopIcon.
			''' For desktop icons, the AccessibleContext takes the form of an
			''' AccessibleJDesktopIcon.
			''' A new AccessibleJDesktopIcon instance is created if necessary.
			''' </summary>
			''' <returns> an AccessibleJDesktopIcon that serves as the
			'''         AccessibleContext of this JDesktopIcon </returns>
			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					If accessibleContext Is Nothing Then accessibleContext = New AccessibleJDesktopIcon(Me)
					Return accessibleContext
				End Get
			End Property

			''' <summary>
			''' This class implements accessibility support for the
			''' <code>JInternalFrame.JDesktopIcon</code> class.  It provides an
			''' implementation of the Java Accessibility API appropriate to
			''' desktop icon user-interface elements.
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
			Protected Friend Class AccessibleJDesktopIcon
				Inherits AccessibleJComponent
				Implements AccessibleValue

				Private ReadOnly outerInstance As JInternalFrame.JDesktopIcon

				Public Sub New(ByVal outerInstance As JInternalFrame.JDesktopIcon)
					Me.outerInstance = outerInstance
				End Sub


				''' <summary>
				''' Gets the role of this object.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of the
				''' object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Overridable Property accessibleRole As AccessibleRole
					Get
						Return AccessibleRole.DESKTOP_ICON
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleValue associated with this object.  In the
				''' implementation of the Java Accessibility API for this class,
				''' returns this object, which is responsible for implementing the
				''' <code>AccessibleValue</code> interface on behalf of itself.
				''' </summary>
				''' <returns> this object </returns>
				Public Overridable Property accessibleValue As AccessibleValue
					Get
						Return Me
					End Get
				End Property

				'
				' AccessibleValue methods
				'

				''' <summary>
				''' Gets the value of this object as a <code>Number</code>.
				''' </summary>
				''' <returns> value of the object -- can be <code>null</code> if this object does not
				''' have a value </returns>
				Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
					Get
						Dim a As AccessibleContext = outerInstance.internalFrame.accessibleContext
						Dim v As AccessibleValue = a.accessibleValue
						If v IsNot Nothing Then
							Return v.currentAccessibleValue
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Sets the value of this object as a <code>Number</code>.
				''' </summary>
				''' <returns> <code>true</code> if the value was set </returns>
				Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
					' TIGER - 4422535
					If n Is Nothing Then Return False
					Dim a As AccessibleContext = outerInstance.internalFrame.accessibleContext
					Dim v As AccessibleValue = a.accessibleValue
					If v IsNot Nothing Then
						Return v.currentAccessibleValuelue(n)
					Else
						Return False
					End If
				End Function

				''' <summary>
				''' Gets the minimum value of this object as a <code>Number</code>.
				''' </summary>
				''' <returns> minimum value of the object; <code>null</code> if this object does not
				''' have a minimum value </returns>
				Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
					Get
						Dim a As AccessibleContext = outerInstance.internalFrame.accessibleContext
						If TypeOf a Is AccessibleValue Then
							Return CType(a, AccessibleValue).minimumAccessibleValue
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the maximum value of this object as a <code>Number</code>.
				''' </summary>
				''' <returns> maximum value of the object; <code>null</code> if this object does not
				''' have a maximum value </returns>
				Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
					Get
						Dim a As AccessibleContext = outerInstance.internalFrame.accessibleContext
						If TypeOf a Is AccessibleValue Then
							Return CType(a, AccessibleValue).maximumAccessibleValue
						Else
							Return Nothing
						End If
					End Get
				End Property

			End Class ' AccessibleJDesktopIcon
		End Class
	End Class

End Namespace
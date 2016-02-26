Imports System
Imports System.Text

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
	''' An extended version of <code>java.awt.Frame</code> that adds support for
	''' the JFC/Swing component architecture.
	''' You can find task-oriented documentation about using <code>JFrame</code>
	''' in <em>The Java Tutorial</em>, in the section
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/frame.html">How to Make Frames</a>.
	''' 
	''' <p>
	''' The <code>JFrame</code> class is slightly incompatible with <code>Frame</code>.
	''' Like all other JFC/Swing top-level containers,
	''' a <code>JFrame</code> contains a <code>JRootPane</code> as its only child.
	''' The <b>content pane</b> provided by the root pane should,
	''' as a rule, contain
	''' all the non-menu components displayed by the <code>JFrame</code>.
	''' This is different from the AWT <code>Frame</code> case.
	''' As a convenience, the {@code add}, {@code remove}, and {@code setLayout}
	''' methods of this class are overridden, so that they delegate calls
	''' to the corresponding methods of the {@code ContentPane}.
	''' For example, you can add a child component to a frame as follows:
	''' <pre>
	'''       frame.add(child);
	''' </pre>
	''' And the child will be added to the contentPane.
	''' The content pane will
	''' always be non-null. Attempting to set it to null will cause the JFrame
	''' to throw an exception. The default content pane will have a BorderLayout
	''' manager set on it.
	''' Refer to <seealso cref="javax.swing.RootPaneContainer"/>
	''' for details on adding, removing and setting the <code>LayoutManager</code>
	''' of a <code>JFrame</code>.
	''' <p>
	''' Unlike a <code>Frame</code>, a <code>JFrame</code> has some notion of how to
	''' respond when the user attempts to close the window. The default behavior
	''' is to simply hide the JFrame when the user closes the window. To change the
	''' default behavior, you invoke the method
	''' <seealso cref="#setDefaultCloseOperation"/>.
	''' To make the <code>JFrame</code> behave the same as a <code>Frame</code>
	''' instance, use
	''' <code>setDefaultCloseOperation(WindowConstants.DO_NOTHING_ON_CLOSE)</code>.
	''' <p>
	''' For more information on content panes
	''' and other features that root panes provide,
	''' see <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/toplevel.html">Using Top-Level Containers</a> in <em>The Java Tutorial</em>.
	''' <p>
	''' In a multi-screen environment, you can create a <code>JFrame</code>
	''' on a different screen device.  See <seealso cref="java.awt.Frame"/> for more
	''' information.
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
	''' <seealso cref= JRootPane </seealso>
	''' <seealso cref= #setDefaultCloseOperation </seealso>
	''' <seealso cref= java.awt.event.WindowListener#windowClosing </seealso>
	''' <seealso cref= javax.swing.RootPaneContainer
	''' 
	''' @beaninfo
	'''      attribute: isContainer true
	'''      attribute: containerDelegate getContentPane
	'''    description: A toplevel window which can be minimized to an icon.
	''' 
	''' @author Jeff Dinkins
	''' @author Georges Saab
	''' @author David Kloba </seealso>
	Public Class JFrame
		Inherits java.awt.Frame
		Implements WindowConstants, javax.accessibility.Accessible, RootPaneContainer, TransferHandler.HasGetTransferHandler

		''' <summary>
		''' The exit application default window close operation. If a window
		''' has this set as the close operation and is closed in an applet,
		''' a <code>SecurityException</code> may be thrown.
		''' It is recommended you only use this in an application.
		''' <p>
		''' @since 1.3
		''' </summary>
		Public Const EXIT_ON_CLOSE As Integer = 3

		''' <summary>
		''' Key into the AppContext, used to check if should provide decorations
		''' by default.
		''' </summary>
		Private Shared ReadOnly defaultLookAndFeelDecoratedKey As Object = New StringBuilder("JFrame.defaultLookAndFeelDecorated")

		Private defaultCloseOperation As Integer = HIDE_ON_CLOSE

		''' <summary>
		''' The <code>TransferHandler</code> for this frame.
		''' </summary>
		Private transferHandler As TransferHandler

		''' <summary>
		''' The <code>JRootPane</code> instance that manages the
		''' <code>contentPane</code>
		''' and optional <code>menuBar</code> for this frame, as well as the
		''' <code>glassPane</code>.
		''' </summary>
		''' <seealso cref= JRootPane </seealso>
		''' <seealso cref= RootPaneContainer </seealso>
		Protected Friend rootPane As JRootPane

		''' <summary>
		''' If true then calls to <code>add</code> and <code>setLayout</code>
		''' will be forwarded to the <code>contentPane</code>. This is initially
		''' false, but is set to true when the <code>JFrame</code> is constructed.
		''' </summary>
		''' <seealso cref= #isRootPaneCheckingEnabled </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend rootPaneCheckingEnabled As Boolean = False


		''' <summary>
		''' Constructs a new frame that is initially invisible.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New()
			MyBase.New()
			frameInit()
		End Sub

		''' <summary>
		''' Creates a <code>Frame</code> in the specified
		''' <code>GraphicsConfiguration</code> of
		''' a screen device and a blank title.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> that is used
		'''          to construct the new <code>Frame</code>;
		'''          if <code>gc</code> is <code>null</code>, the system
		'''          default <code>GraphicsConfiguration</code> is assumed </param>
		''' <exception cref="IllegalArgumentException"> if <code>gc</code> is not from
		'''          a screen device.  This exception is always thrown when
		'''      GraphicsEnvironment.isHeadless() returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' @since     1.3 </seealso>
		Public Sub New(ByVal gc As java.awt.GraphicsConfiguration)
			MyBase.New(gc)
			frameInit()
		End Sub

		''' <summary>
		''' Creates a new, initially invisible <code>Frame</code> with the
		''' specified title.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="title"> the title for the frame </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal title As String)
			MyBase.New(title)
			frameInit()
		End Sub

		''' <summary>
		''' Creates a <code>JFrame</code> with the specified title and the
		''' specified <code>GraphicsConfiguration</code> of a screen device.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="title"> the title to be displayed in the
		'''          frame's border. A <code>null</code> value is treated as
		'''          an empty string, "". </param>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> that is used
		'''          to construct the new <code>JFrame</code> with;
		'''          if <code>gc</code> is <code>null</code>, the system
		'''          default <code>GraphicsConfiguration</code> is assumed </param>
		''' <exception cref="IllegalArgumentException"> if <code>gc</code> is not from
		'''          a screen device.  This exception is always thrown when
		'''      GraphicsEnvironment.isHeadless() returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' @since     1.3 </seealso>
		Public Sub New(ByVal title As String, ByVal gc As java.awt.GraphicsConfiguration)
			MyBase.New(title, gc)
			frameInit()
		End Sub

		''' <summary>
		''' Called by the constructors to init the <code>JFrame</code> properly. </summary>
		Protected Friend Overridable Sub frameInit()
			enableEvents(java.awt.AWTEvent.KEY_EVENT_MASK Or java.awt.AWTEvent.WINDOW_EVENT_MASK)
			locale = JComponent.defaultLocale
			rootPane = createRootPane()
			background = UIManager.getColor("control")
			rootPaneCheckingEnabled = True
			If JFrame.defaultLookAndFeelDecorated Then
				Dim supportsWindowDecorations As Boolean = UIManager.lookAndFeel.supportsWindowDecorations
				If supportsWindowDecorations Then
					undecorated = True
					rootPane.windowDecorationStyle = JRootPane.FRAME
				End If
			End If
			sun.awt.SunToolkit.checkAndSetPolicy(Me)
		End Sub

		''' <summary>
		''' Called by the constructor methods to create the default
		''' <code>rootPane</code>.
		''' </summary>
		Protected Friend Overridable Function createRootPane() As JRootPane
			Dim rp As New JRootPane
			' NOTE: this uses setOpaque vs LookAndFeel.installProperty as there
			' is NO reason for the RootPane not to be opaque. For painting to
			' work the contentPane must be opaque, therefor the RootPane can
			' also be opaque.
			rp.opaque = True
			Return rp
		End Function

		''' <summary>
		''' Processes window events occurring on this component.
		''' Hides the window or disposes of it, as specified by the setting
		''' of the <code>defaultCloseOperation</code> property.
		''' </summary>
		''' <param name="e">  the window event </param>
		''' <seealso cref=    #setDefaultCloseOperation </seealso>
		''' <seealso cref=    java.awt.Window#processWindowEvent </seealso>
		Protected Friend Overridable Sub processWindowEvent(ByVal e As java.awt.event.WindowEvent)
			MyBase.processWindowEvent(e)

			If e.iD = java.awt.event.WindowEvent.WINDOW_CLOSING Then
				Select Case defaultCloseOperation
					Case HIDE_ON_CLOSE
						visible = False
					Case DISPOSE_ON_CLOSE
						Dispose()
					Case EXIT_ON_CLOSE
						' This needs to match the checkExit call in
						' setDefaultCloseOperation
						Environment.Exit(0)
					Case Else
				End Select
			End If
		End Sub

		''' <summary>
		''' Sets the operation that will happen by default when
		''' the user initiates a "close" on this frame.
		''' You must specify one of the following choices:
		''' <br><br>
		''' <ul>
		''' <li><code>DO_NOTHING_ON_CLOSE</code>
		''' (defined in <code>WindowConstants</code>):
		''' Don't do anything; require the
		''' program to handle the operation in the <code>windowClosing</code>
		''' method of a registered <code>WindowListener</code> object.
		''' 
		''' <li><code>HIDE_ON_CLOSE</code>
		''' (defined in <code>WindowConstants</code>):
		''' Automatically hide the frame after
		''' invoking any registered <code>WindowListener</code>
		''' objects.
		''' 
		''' <li><code>DISPOSE_ON_CLOSE</code>
		''' (defined in <code>WindowConstants</code>):
		''' Automatically hide and dispose the
		''' frame after invoking any registered <code>WindowListener</code>
		''' objects.
		''' 
		''' <li><code>EXIT_ON_CLOSE</code>
		''' (defined in <code>JFrame</code>):
		''' Exit the application using the <code>System</code>
		''' <code>exit</code> method.  Use this only in applications.
		''' </ul>
		''' <p>
		''' The value is set to <code>HIDE_ON_CLOSE</code> by default. Changes
		''' to the value of this property cause the firing of a property
		''' change event, with property name "defaultCloseOperation".
		''' <p>
		''' <b>Note</b>: When the last displayable window within the
		''' Java virtual machine (VM) is disposed of, the VM may
		''' terminate.  See <a href="../../java/awt/doc-files/AWTThreadIssues.html">
		''' AWT Threading Issues</a> for more information.
		''' </summary>
		''' <param name="operation"> the operation which should be performed when the
		'''        user closes the frame </param>
		''' <exception cref="IllegalArgumentException"> if defaultCloseOperation value
		'''             isn't one of the above valid values </exception>
		''' <seealso cref= #addWindowListener </seealso>
		''' <seealso cref= #getDefaultCloseOperation </seealso>
		''' <seealso cref= WindowConstants </seealso>
		''' <exception cref="SecurityException">
		'''        if <code>EXIT_ON_CLOSE</code> has been specified and the
		'''        <code>SecurityManager</code> will
		'''        not allow the caller to invoke <code>System.exit</code> </exception>
		''' <seealso cref=        java.lang.Runtime#exit(int)
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		'''        enum: DO_NOTHING_ON_CLOSE WindowConstants.DO_NOTHING_ON_CLOSE
		'''              HIDE_ON_CLOSE       WindowConstants.HIDE_ON_CLOSE
		'''              DISPOSE_ON_CLOSE    WindowConstants.DISPOSE_ON_CLOSE
		'''              EXIT_ON_CLOSE       WindowConstants.EXIT_ON_CLOSE
		''' description: The frame's default close operation. </seealso>
		Public Overridable Property defaultCloseOperation As Integer
			Set(ByVal operation As Integer)
				If operation <> DO_NOTHING_ON_CLOSE AndAlso operation <> HIDE_ON_CLOSE AndAlso operation <> DISPOSE_ON_CLOSE AndAlso operation <> EXIT_ON_CLOSE Then Throw New System.ArgumentException("defaultCloseOperation must be one of: DO_NOTHING_ON_CLOSE, HIDE_ON_CLOSE, DISPOSE_ON_CLOSE, or EXIT_ON_CLOSE")
    
				If operation = EXIT_ON_CLOSE Then
					Dim security As SecurityManager = System.securityManager
					If security IsNot Nothing Then security.checkExit(0)
				End If
				If Me.defaultCloseOperation <> operation Then
					Dim oldValue As Integer = Me.defaultCloseOperation
					Me.defaultCloseOperation = operation
					firePropertyChange("defaultCloseOperation", oldValue, operation)
				End If
			End Set
			Get
				Return defaultCloseOperation
			End Get
		End Property



		''' <summary>
		''' Sets the {@code transferHandler} property, which is a mechanism to
		''' support transfer of data into this component. Use {@code null}
		''' if the component does not support data transfer operations.
		''' <p>
		''' If the system property {@code suppressSwingDropSupport} is {@code false}
		''' (the default) and the current drop target on this component is either
		''' {@code null} or not a user-set drop target, this method will change the
		''' drop target as follows: If {@code newHandler} is {@code null} it will
		''' clear the drop target. If not {@code null} it will install a new
		''' {@code DropTarget}.
		''' <p>
		''' Note: When used with {@code JFrame}, {@code TransferHandler} only
		''' provides data import capability, as the data export related methods
		''' are currently typed to {@code JComponent}.
		''' <p>
		''' Please see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
		''' How to Use Drag and Drop and Data Transfer</a>, a section in
		''' <em>The Java Tutorial</em>, for more information.
		''' </summary>
		''' <param name="newHandler"> the new {@code TransferHandler}
		''' </param>
		''' <seealso cref= TransferHandler </seealso>
		''' <seealso cref= #getTransferHandler </seealso>
		''' <seealso cref= java.awt.Component#setDropTarget
		''' @since 1.6
		''' 
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''  description: Mechanism for transfer of data into the component </seealso>
		Public Overridable Property transferHandler As TransferHandler
			Set(ByVal newHandler As TransferHandler)
				Dim oldHandler As TransferHandler = transferHandler
				transferHandler = newHandler
				SwingUtilities.installSwingDropTargetAsNecessary(Me, transferHandler)
				firePropertyChange("transferHandler", oldHandler, newHandler)
			End Set
			Get
				Return transferHandler
			End Get
		End Property


		''' <summary>
		''' Just calls <code>paint(g)</code>.  This method was overridden to
		''' prevent an unnecessary call to clear the background.
		''' </summary>
		''' <param name="g"> the Graphics context in which to paint </param>
		Public Overridable Sub update(ByVal g As java.awt.Graphics)
			paint(g)
		End Sub

	   ''' <summary>
	   ''' Sets the menubar for this frame. </summary>
	   ''' <param name="menubar"> the menubar being placed in the frame
	   ''' </param>
	   ''' <seealso cref= #getJMenuBar
	   ''' 
	   ''' @beaninfo
	   '''      hidden: true
	   ''' description: The menubar for accessing pulldown menus from this frame. </seealso>
		Public Overridable Property jMenuBar As JMenuBar
			Set(ByVal menubar As JMenuBar)
				rootPane.menuBar = menubar
			End Set
			Get
				Return rootPane.menuBar
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
		Protected Friend Overridable Sub addImpl(ByVal comp As java.awt.Component, ByVal constraints As Object, ByVal index As Integer)
			If rootPaneCheckingEnabled Then
				contentPane.add(comp, constraints, index)
			Else
				MyBase.addImpl(comp, constraints, index)
			End If
		End Sub

		''' <summary>
		''' Removes the specified component from the container. If
		''' <code>comp</code> is not the <code>rootPane</code>, this will forward
		''' the call to the <code>contentPane</code>. This will do nothing if
		''' <code>comp</code> is not a child of the <code>JFrame</code> or
		''' <code>contentPane</code>.
		''' </summary>
		''' <param name="comp"> the component to be removed </param>
		''' <exception cref="NullPointerException"> if <code>comp</code> is null </exception>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Public Overridable Sub remove(ByVal comp As java.awt.Component)
			If comp Is rootPane Then
				MyBase.remove(comp)
			Else
				contentPane.remove(comp)
			End If
		End Sub


		''' <summary>
		''' Sets the <code>LayoutManager</code>.
		''' Overridden to conditionally forward the call to the
		''' <code>contentPane</code>.
		''' Refer to <seealso cref="javax.swing.RootPaneContainer"/> for
		''' more information.
		''' </summary>
		''' <param name="manager"> the <code>LayoutManager</code> </param>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Public Overridable Property layout As java.awt.LayoutManager
			Set(ByVal manager As java.awt.LayoutManager)
				If rootPaneCheckingEnabled Then
					contentPane.layout = manager
				Else
					MyBase.layout = manager
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the <code>rootPane</code> object for this frame. </summary>
		''' <returns> the <code>rootPane</code> property
		''' </returns>
		''' <seealso cref= #setRootPane </seealso>
		''' <seealso cref= RootPaneContainer#getRootPane </seealso>
		Public Overridable Property rootPane As JRootPane Implements RootPaneContainer.getRootPane
			Get
				Return rootPane
			End Get
			Set(ByVal root As JRootPane)
				If rootPane IsNot Nothing Then remove(rootPane)
				rootPane = root
				If rootPane IsNot Nothing Then
					Dim checkingEnabled As Boolean = rootPaneCheckingEnabled
					Try
						rootPaneCheckingEnabled = False
						add(rootPane, java.awt.BorderLayout.CENTER)
					Finally
						rootPaneCheckingEnabled = checkingEnabled
					End Try
				End If
			End Set
		End Property



		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property iconImage As java.awt.Image
			Set(ByVal image As java.awt.Image)
				MyBase.iconImage = image
			End Set
		End Property

		''' <summary>
		''' Returns the <code>contentPane</code> object for this frame. </summary>
		''' <returns> the <code>contentPane</code> property
		''' </returns>
		''' <seealso cref= #setContentPane </seealso>
		''' <seealso cref= RootPaneContainer#getContentPane </seealso>
		Public Overridable Property contentPane As java.awt.Container Implements RootPaneContainer.getContentPane
			Get
				Return rootPane.contentPane
			End Get
			Set(ByVal contentPane As java.awt.Container)
				rootPane.contentPane = contentPane
			End Set
		End Property


		''' <summary>
		''' Returns the <code>layeredPane</code> object for this frame. </summary>
		''' <returns> the <code>layeredPane</code> property
		''' </returns>
		''' <seealso cref= #setLayeredPane </seealso>
		''' <seealso cref= RootPaneContainer#getLayeredPane </seealso>
		Public Overridable Property layeredPane As JLayeredPane Implements RootPaneContainer.getLayeredPane
			Get
				Return rootPane.layeredPane
			End Get
			Set(ByVal layeredPane As JLayeredPane)
				rootPane.layeredPane = layeredPane
			End Set
		End Property


		''' <summary>
		''' Returns the <code>glassPane</code> object for this frame. </summary>
		''' <returns> the <code>glassPane</code> property
		''' </returns>
		''' <seealso cref= #setGlassPane </seealso>
		''' <seealso cref= RootPaneContainer#getGlassPane </seealso>
		Public Overridable Property glassPane As java.awt.Component Implements RootPaneContainer.getGlassPane
			Get
				Return rootPane.glassPane
			End Get
			Set(ByVal glassPane As java.awt.Component)
				rootPane.glassPane = glassPane
			End Set
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.6
		''' </summary>
		Public Overridable Property graphics As java.awt.Graphics
			Get
				JComponent.getGraphicsInvoked(Me)
				Return MyBase.graphics
			End Get
		End Property

		''' <summary>
		''' Repaints the specified rectangle of this component within
		''' <code>time</code> milliseconds.  Refer to <code>RepaintManager</code>
		''' for details on how the repaint is handled.
		''' </summary>
		''' <param name="time">   maximum time in milliseconds before update </param>
		''' <param name="x">    the <i>x</i> coordinate </param>
		''' <param name="y">    the <i>y</i> coordinate </param>
		''' <param name="width">    the width </param>
		''' <param name="height">   the height </param>
		''' <seealso cref=       RepaintManager
		''' @since     1.6 </seealso>
		Public Overridable Sub repaint(ByVal time As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			If RepaintManager.HANDLE_TOP_LEVEL_PAINT Then
				RepaintManager.currentManager(Me).addDirtyRegion(Me, x, y, width, height)
			Else
				MyBase.repaint(time, x, y, width, height)
			End If
		End Sub

		''' <summary>
		''' Provides a hint as to whether or not newly created <code>JFrame</code>s
		''' should have their Window decorations (such as borders, widgets to
		''' close the window, title...) provided by the current look
		''' and feel. If <code>defaultLookAndFeelDecorated</code> is true,
		''' the current <code>LookAndFeel</code> supports providing window
		''' decorations, and the current window manager supports undecorated
		''' windows, then newly created <code>JFrame</code>s will have their
		''' Window decorations provided by the current <code>LookAndFeel</code>.
		''' Otherwise, newly created <code>JFrame</code>s will have their
		''' Window decorations provided by the current window manager.
		''' <p>
		''' You can get the same effect on a single JFrame by doing the following:
		''' <pre>
		'''    JFrame frame = new JFrame();
		'''    frame.setUndecorated(true);
		'''    frame.getRootPane().setWindowDecorationStyle(JRootPane.FRAME);
		''' </pre>
		''' </summary>
		''' <param name="defaultLookAndFeelDecorated"> A hint as to whether or not current
		'''        look and feel should provide window decorations </param>
		''' <seealso cref= javax.swing.LookAndFeel#getSupportsWindowDecorations
		''' @since 1.4 </seealso>
		Public Shared Property defaultLookAndFeelDecorated As Boolean
			Set(ByVal defaultLookAndFeelDecorated As Boolean)
				If defaultLookAndFeelDecorated Then
					SwingUtilities.appContextPut(defaultLookAndFeelDecoratedKey, Boolean.TRUE)
				Else
					SwingUtilities.appContextPut(defaultLookAndFeelDecoratedKey, Boolean.FALSE)
				End If
			End Set
			Get
				Dim ___defaultLookAndFeelDecorated As Boolean? = CBool(SwingUtilities.appContextGet(defaultLookAndFeelDecoratedKey))
				If ___defaultLookAndFeelDecorated Is Nothing Then ___defaultLookAndFeelDecorated = Boolean.FALSE
				Return ___defaultLookAndFeelDecorated
			End Get
		End Property



		''' <summary>
		''' Returns a string representation of this <code>JFrame</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JFrame</code> </returns>
		Protected Friend Overridable Function paramString() As String
			Dim defaultCloseOperationString As String
			If defaultCloseOperation = HIDE_ON_CLOSE Then
				defaultCloseOperationString = "HIDE_ON_CLOSE"
			ElseIf defaultCloseOperation = DISPOSE_ON_CLOSE Then
				defaultCloseOperationString = "DISPOSE_ON_CLOSE"
			ElseIf defaultCloseOperation = DO_NOTHING_ON_CLOSE Then
				defaultCloseOperationString = "DO_NOTHING_ON_CLOSE"
			ElseIf defaultCloseOperation = 3 Then
				defaultCloseOperationString = "EXIT_ON_CLOSE"
			Else
				defaultCloseOperationString = ""
			End If
			Dim rootPaneString As String = (If(rootPane IsNot Nothing, rootPane.ToString(), ""))
			Dim rootPaneCheckingEnabledString As String = (If(rootPaneCheckingEnabled, "true", "false"))

			Return MyBase.paramString() & ",defaultCloseOperation=" & defaultCloseOperationString & ",rootPane=" & rootPaneString & ",rootPaneCheckingEnabled=" & rootPaneCheckingEnabledString
		End Function



	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' The accessible context property. </summary>
		Protected Friend accessibleContext As javax.accessibility.AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this JFrame.
		''' For JFrames, the AccessibleContext takes the form of an
		''' AccessibleJFrame.
		''' A new AccessibleJFrame instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJFrame that serves as the
		'''         AccessibleContext of this JFrame </returns>
		Public Overridable Property accessibleContext As javax.accessibility.AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJFrame(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JFrame</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to frame user-interface
		''' elements.
		''' </summary>
		Protected Friend Class AccessibleJFrame
			Inherits AccessibleAWTFrame

			Private ReadOnly outerInstance As JFrame

			Public Sub New(ByVal outerInstance As JFrame)
				Me.outerInstance = outerInstance
			End Sub


			' AccessibleContext methods
			''' <summary>
			''' Get the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be null if this
			''' object does not have a name </returns>
			Public Overridable Property accessibleName As String
				Get
					If accessibleName IsNot Nothing Then
						Return accessibleName
					Else
						If title Is Nothing Then
							Return MyBase.accessibleName
						Else
							Return title
						End If
					End If
				End Get
			End Property

			''' <summary>
			''' Get the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As javax.accessibility.AccessibleStateSet
				Get
					Dim states As javax.accessibility.AccessibleStateSet = MyBase.accessibleStateSet
    
					If resizable Then states.add(javax.accessibility.AccessibleState.RESIZABLE)
					If focusOwner IsNot Nothing Then states.add(javax.accessibility.AccessibleState.ACTIVE)
					' FIXME:  [[[WDW - should also return ICONIFIED and ICONIFIABLE
					' if we can ever figure these out]]]
					Return states
				End Get
			End Property
		End Class ' inner class AccessibleJFrame
	End Class

End Namespace
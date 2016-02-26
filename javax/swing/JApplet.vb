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
	''' An extended version of <code>java.applet.Applet</code> that adds support for
	''' the JFC/Swing component architecture.
	''' You can find task-oriented documentation about using <code>JApplet</code>
	''' in <em>The Java Tutorial</em>,
	''' in the section
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/applet.html">How to Make Applets</a>.
	''' <p>
	''' The <code>JApplet</code> class is slightly incompatible with
	''' <code>java.applet.Applet</code>.  <code>JApplet</code> contains a
	''' <code>JRootPane</code> as its only child.  The <code>contentPane</code>
	''' should be the parent of any children of the <code>JApplet</code>.
	''' As a convenience, the {@code add}, {@code remove}, and {@code setLayout}
	''' methods of this class are overridden, so that they delegate calls
	''' to the corresponding methods of the {@code ContentPane}.
	''' For example, you can add a child component to an applet as follows:
	''' <pre>
	'''       applet.add(child);
	''' </pre>
	''' 
	''' And the child will be added to the <code>contentPane</code>.
	''' The <code>contentPane</code> will always be non-<code>null</code>.
	''' Attempting to set it to <code>null</code> will cause the
	''' <code>JApplet</code> to throw an exception. The default
	''' <code>contentPane</code> will have a <code>BorderLayout</code>
	''' manager set on it.
	''' Refer to <seealso cref="javax.swing.RootPaneContainer"/>
	''' for details on adding, removing and setting the <code>LayoutManager</code>
	''' of a <code>JApplet</code>.
	''' <p>
	''' Please see the <code>JRootPane</code> documentation for a
	''' complete description of the <code>contentPane</code>, <code>glassPane</code>,
	''' and <code>layeredPane</code> properties.
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
	''' <seealso cref= javax.swing.RootPaneContainer
	''' @beaninfo
	'''      attribute: isContainer true
	'''      attribute: containerDelegate getContentPane
	'''    description: Swing's Applet subclass.
	''' 
	''' @author Arnaud Weber </seealso>
	Public Class JApplet
		Inherits java.applet.Applet
		Implements Accessible, RootPaneContainer, TransferHandler.HasGetTransferHandler

		''' <seealso cref= #getRootPane </seealso>
		''' <seealso cref= #setRootPane </seealso>
		Protected Friend rootPane As JRootPane

		''' <summary>
		''' If true then calls to <code>add</code> and <code>setLayout</code>
		''' will be forwarded to the <code>contentPane</code>. This is initially
		''' false, but is set to true when the <code>JApplet</code> is constructed.
		''' </summary>
		''' <seealso cref= #isRootPaneCheckingEnabled </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend rootPaneCheckingEnabled As Boolean = False

		''' <summary>
		''' The <code>TransferHandler</code> for this applet.
		''' </summary>
		Private transferHandler As TransferHandler

		''' <summary>
		''' Creates a swing applet instance.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New()
			MyBase.New()
			' Check the timerQ and restart if necessary.
			Dim q As TimerQueue = TimerQueue.sharedInstance()
			If q IsNot Nothing Then q.startIfNeeded()

	'         Workaround for bug 4155072.  The shared double buffer image
	'         * may hang on to a reference to this applet; unfortunately
	'         * Image.getGraphics() will continue to call JApplet.getForeground()
	'         * and getBackground() even after this applet has been destroyed.
	'         * So we ensure that these properties are non-null here.
	'         
			foreground = Color.black
			background = Color.white

			locale = JComponent.defaultLocale
			layout = New BorderLayout
			rootPane = createRootPane()
			rootPaneCheckingEnabled = True

			focusTraversalPolicyProvider = True
			sun.awt.SunToolkit.checkAndSetPolicy(Me)

			enableEvents(AWTEvent.KEY_EVENT_MASK)
		End Sub


		''' <summary>
		''' Called by the constructor methods to create the default rootPane. </summary>
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
		''' Note: When used with {@code JApplet}, {@code TransferHandler} only
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
		Public Overridable Sub update(ByVal g As Graphics)
			paint(g)
		End Sub

	   ''' <summary>
	   ''' Sets the menubar for this applet. </summary>
	   ''' <param name="menuBar"> the menubar being placed in the applet
	   ''' </param>
	   ''' <seealso cref= #getJMenuBar
	   ''' 
	   ''' @beaninfo
	   '''      hidden: true
	   ''' description: The menubar for accessing pulldown menus from this applet. </seealso>
		Public Overridable Property jMenuBar As JMenuBar
			Set(ByVal menuBar As JMenuBar)
				rootPane.menuBar = menuBar
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
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
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
		Public Overridable Sub remove(ByVal comp As Component)
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
		Public Overridable Property layout As LayoutManager
			Set(ByVal manager As LayoutManager)
				If rootPaneCheckingEnabled Then
					contentPane.layout = manager
				Else
					MyBase.layout = manager
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the rootPane object for this applet.
		''' </summary>
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
						add(rootPane, BorderLayout.CENTER)
					Finally
						rootPaneCheckingEnabled = checkingEnabled
					End Try
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the contentPane object for this applet.
		''' </summary>
		''' <seealso cref= #setContentPane </seealso>
		''' <seealso cref= RootPaneContainer#getContentPane </seealso>
		Public Overridable Property contentPane As Container
			Get
				Return rootPane.contentPane
			End Get
			Set(ByVal contentPane As Container)
				rootPane.contentPane = contentPane
			End Set
		End Property


		''' <summary>
		''' Returns the layeredPane object for this applet.
		''' </summary>
		''' <exception cref="java.awt.IllegalComponentStateException"> (a runtime
		'''            exception) if the layered pane parameter is null </exception>
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
		''' Returns the glassPane object for this applet.
		''' </summary>
		''' <seealso cref= #setGlassPane </seealso>
		''' <seealso cref= RootPaneContainer#getGlassPane </seealso>
		Public Overridable Property glassPane As Component
			Get
				Return rootPane.glassPane
			End Get
			Set(ByVal glassPane As Component)
				rootPane.glassPane = glassPane
			End Set
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.6
		''' </summary>
		Public Overridable Property graphics As Graphics
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
		''' Returns a string representation of this JApplet. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JApplet. </returns>
		Protected Friend Overridable Function paramString() As String
			Dim rootPaneString As String = (If(rootPane IsNot Nothing, rootPane.ToString(), ""))
			Dim rootPaneCheckingEnabledString As String = (If(rootPaneCheckingEnabled, "true", "false"))

			Return MyBase.paramString() & ",rootPane=" & rootPaneString & ",rootPaneCheckingEnabled=" & rootPaneCheckingEnabledString
		End Function



	'///////////////
	' Accessibility support
	'//////////////

		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this JApplet.
		''' For JApplets, the AccessibleContext takes the form of an
		''' AccessibleJApplet.
		''' A new AccessibleJApplet instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJApplet that serves as the
		'''         AccessibleContext of this JApplet </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleJApplet(Me)
				Return ___accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JApplet</code> class.
		''' </summary>
		Protected Friend Class AccessibleJApplet
			Inherits AccessibleApplet

			Private ReadOnly outerInstance As JApplet

			Public Sub New(ByVal outerInstance As JApplet)
				Me.outerInstance = outerInstance
			End Sub

			' everything moved to new parent, AccessibleApplet
		End Class
	End Class

End Namespace
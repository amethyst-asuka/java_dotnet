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
	''' A <code>JWindow</code> is a container that can be displayed anywhere on the
	''' user's desktop. It does not have the title bar, window-management buttons,
	''' or other trimmings associated with a <code>JFrame</code>, but it is still a
	''' "first-class citizen" of the user's desktop, and can exist anywhere
	''' on it.
	''' <p>
	''' The <code>JWindow</code> component contains a <code>JRootPane</code>
	''' as its only child.  The <code>contentPane</code> should be the parent
	''' of any children of the <code>JWindow</code>.
	''' As a convenience, the {@code add}, {@code remove}, and {@code setLayout}
	''' methods of this class are overridden, so that they delegate calls
	''' to the corresponding methods of the {@code ContentPane}.
	''' For example, you can add a child component to a window as follows:
	''' <pre>
	'''       window.add(child);
	''' </pre>
	''' And the child will be added to the contentPane.
	''' The <code>contentPane</code> will always be non-<code>null</code>.
	''' Attempting to set it to <code>null</code> will cause the <code>JWindow</code>
	''' to throw an exception. The default <code>contentPane</code> will have a
	''' <code>BorderLayout</code> manager set on it.
	''' Refer to <seealso cref="javax.swing.RootPaneContainer"/>
	''' for details on adding, removing and setting the <code>LayoutManager</code>
	''' of a <code>JWindow</code>.
	''' <p>
	''' Please see the <seealso cref="JRootPane"/> documentation for a complete description of
	''' the <code>contentPane</code>, <code>glassPane</code>, and
	''' <code>layeredPane</code> components.
	''' <p>
	''' In a multi-screen environment, you can create a <code>JWindow</code>
	''' on a different screen device.  See <seealso cref="java.awt.Window"/> for more
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
	''' <seealso cref= JRootPane
	''' 
	''' @beaninfo
	'''      attribute: isContainer true
	'''      attribute: containerDelegate getContentPane
	'''    description: A toplevel window which has no system border or controls.
	''' 
	''' @author David Kloba </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JWindow
		Inherits Window
		Implements Accessible, RootPaneContainer, TransferHandler.HasGetTransferHandler

		''' <summary>
		''' The <code>JRootPane</code> instance that manages the
		''' <code>contentPane</code>
		''' and optional <code>menuBar</code> for this frame, as well as the
		''' <code>glassPane</code>.
		''' </summary>
		''' <seealso cref= #getRootPane </seealso>
		''' <seealso cref= #setRootPane </seealso>
		Protected Friend rootPane As JRootPane

		''' <summary>
		''' If true then calls to <code>add</code> and <code>setLayout</code>
		''' will be forwarded to the <code>contentPane</code>. This is initially
		''' false, but is set to true when the <code>JWindow</code> is constructed.
		''' </summary>
		''' <seealso cref= #isRootPaneCheckingEnabled </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend rootPaneCheckingEnabled As Boolean = False

		''' <summary>
		''' The <code>TransferHandler</code> for this window.
		''' </summary>
		Private transferHandler As TransferHandler

		''' <summary>
		''' Creates a window with no specified owner. This window will not be
		''' focusable.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <exception cref="HeadlessException"> if
		'''         <code>GraphicsEnvironment.isHeadless()</code> returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New()
			Me.New(CType(Nothing, Frame))
		End Sub

		''' <summary>
		''' Creates a window with the specified <code>GraphicsConfiguration</code>
		''' of a screen device. This window will not be focusable.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> that is used
		'''          to construct the new window with; if gc is <code>null</code>,
		'''          the system default <code>GraphicsConfiguration</code>
		'''          is assumed </param>
		''' <exception cref="HeadlessException"> If
		'''         <code>GraphicsEnvironment.isHeadless()</code> returns true. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>gc</code> is not from
		'''         a screen device.
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since  1.3 </seealso>
		Public Sub New(ByVal gc As GraphicsConfiguration)
			Me.New(Nothing, gc)
			MyBase.focusableWindowState = False
		End Sub

		''' <summary>
		''' Creates a window with the specified owner frame.
		''' If <code>owner</code> is <code>null</code>, the shared owner
		''' will be used and this window will not be focusable. Also,
		''' this window will not be focusable unless its owner is showing
		''' on the screen.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="owner"> the frame from which the window is displayed </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		'''            returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Frame)
			MyBase.New(If(owner Is Nothing, SwingUtilities.sharedOwnerFrame, owner))
			If owner Is Nothing Then
				Dim ownerShutdownListener As WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
				addWindowListener(ownerShutdownListener)
			End If
			windowInit()
		End Sub

		''' <summary>
		''' Creates a window with the specified owner window. This window
		''' will not be focusable unless its owner is showing on the screen.
		''' If <code>owner</code> is <code>null</code>, the shared owner
		''' will be used and this window will not be focusable.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="owner"> the window from which the window is displayed </param>
		''' <exception cref="HeadlessException"> if
		'''         <code>GraphicsEnvironment.isHeadless()</code> returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Window)
			MyBase.New(If(owner Is Nothing, CType(SwingUtilities.sharedOwnerFrame, Window), owner))
			If owner Is Nothing Then
				Dim ownerShutdownListener As WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
				addWindowListener(ownerShutdownListener)
			End If
			windowInit()
		End Sub

		''' <summary>
		''' Creates a window with the specified owner window and
		''' <code>GraphicsConfiguration</code> of a screen device. If
		''' <code>owner</code> is <code>null</code>, the shared owner will be used
		''' and this window will not be focusable.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <param name="owner"> the window from which the window is displayed </param>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> that is used
		'''          to construct the new window with; if gc is <code>null</code>,
		'''          the system default <code>GraphicsConfiguration</code>
		'''          is assumed, unless <code>owner</code> is also null, in which
		'''          case the <code>GraphicsConfiguration</code> from the
		'''          shared owner frame will be used. </param>
		''' <exception cref="HeadlessException"> if
		'''         <code>GraphicsEnvironment.isHeadless()</code> returns true. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>gc</code> is not from
		'''         a screen device.
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since  1.3 </seealso>
		Public Sub New(ByVal owner As Window, ByVal gc As GraphicsConfiguration)
			MyBase.New(If(owner Is Nothing, CType(SwingUtilities.sharedOwnerFrame, Window), owner), gc)
			If owner Is Nothing Then
				Dim ownerShutdownListener As WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
				addWindowListener(ownerShutdownListener)
			End If
			windowInit()
		End Sub

		''' <summary>
		''' Called by the constructors to init the <code>JWindow</code> properly.
		''' </summary>
		Protected Friend Overridable Sub windowInit()
			locale = JComponent.defaultLocale
			rootPane = createRootPane()
			rootPaneCheckingEnabled = True
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
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overridable Function isRootPaneCheckingEnabled() As Boolean 'JavaToDotNetTempPropertyGetrootPaneCheckingEnabled
		Protected Friend Overridable Property rootPaneCheckingEnabled As Boolean
			Get
				Return rootPaneCheckingEnabled
			End Get
			Set(ByVal enabled As Boolean)
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
		''' Note: When used with {@code JWindow}, {@code TransferHandler} only
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
		''' Calls <code>paint(g)</code>.  This method was overridden to
		''' prevent an unnecessary call to clear the background.
		''' </summary>
		''' <param name="g">  the <code>Graphics</code> context in which to paint </param>
		Public Overridable Sub update(ByVal g As Graphics)
			paint(g)
		End Sub

			rootPaneCheckingEnabled = enabled
		End Sub


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
		''' <code>comp</code> is not a child of the <code>JWindow</code> or
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
		''' Returns the <code>rootPane</code> object for this window. </summary>
		''' <returns> the <code>rootPane</code> property for this window
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
						add(rootPane, BorderLayout.CENTER)
					Finally
						rootPaneCheckingEnabled = checkingEnabled
					End Try
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the <code>Container</code> which is the <code>contentPane</code>
		''' for this window.
		''' </summary>
		''' <returns> the <code>contentPane</code> property </returns>
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
		''' Returns the <code>layeredPane</code> object for this window.
		''' </summary>
		''' <returns> the <code>layeredPane</code> property </returns>
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
		''' Returns the <code>glassPane Component</code> for this window.
		''' </summary>
		''' <returns> the <code>glassPane</code> property </returns>
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
		''' Returns a string representation of this <code>JWindow</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JWindow</code> </returns>
		Protected Friend Overridable Function paramString() As String
			Dim rootPaneCheckingEnabledString As String = (If(rootPaneCheckingEnabled, "true", "false"))

			Return MyBase.paramString() & ",rootPaneCheckingEnabled=" & rootPaneCheckingEnabledString
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' The accessible context property. </summary>
		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this JWindow.
		''' For JWindows, the AccessibleContext takes the form of an
		''' AccessibleJWindow.
		''' A new AccessibleJWindow instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJWindow that serves as the
		'''         AccessibleContext of this JWindow </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleJWindow(Me)
				Return ___accessibleContext
			End Get
		End Property


		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JWindow</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to window user-interface
		''' elements.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJWindow
			Inherits AccessibleAWTWindow

			Private ReadOnly outerInstance As JWindow

			Public Sub New(ByVal outerInstance As JWindow)
				Me.outerInstance = outerInstance
			End Sub

			' everything is in the new parent, AccessibleAWTWindow
		End Class

	End Class

End Namespace
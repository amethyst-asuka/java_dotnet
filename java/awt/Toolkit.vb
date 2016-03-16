Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports java.util
Imports [event]
Imports java.awt.peer

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt





    ''' <summary>
    ''' This class is the abstract superclass of all actual
    ''' implementations of the Abstract Window Toolkit. Subclasses of
    ''' the <code>Toolkit</code> class are used to bind the various components
    ''' to particular native toolkit implementations.
    ''' <p>
    ''' Many GUI events may be delivered to user
    ''' asynchronously, if the opposite is not specified explicitly.
    ''' As well as
    ''' many GUI operations may be performed asynchronously.
    ''' This fact means that if the state of a component is set, and then
    ''' the state immediately queried, the returned value may not yet
    ''' reflect the requested change.  This behavior includes, but is not
    ''' limited to:
    ''' <ul>
    ''' <li>Scrolling to a specified position.
    ''' <br>For example, calling <code>ScrollPane.setScrollPosition</code>
    '''     and then <code>getScrollPosition</code> may return an incorrect
    '''     value if the original request has not yet been processed.
    ''' 
    ''' <li>Moving the focus from one component to another.
    ''' <br>For more information, see
    ''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html#transferTiming">Timing
    ''' Focus Transfers</a>, a section in
    ''' <a href="http://java.sun.com/docs/books/tutorial/uiswing/">The Swing
    ''' Tutorial</a>.
    ''' 
    ''' <li>Making a top-level container visible.
    ''' <br>Calling <code>setVisible(true)</code> on a <code>Window</code>,
    '''     <code>Frame</code> or <code>Dialog</code> may occur
    '''     asynchronously.
    ''' 
    ''' <li>Setting the size or location of a top-level container.
    ''' <br>Calls to <code>setSize</code>, <code>setBounds</code> or
    '''     <code>setLocation</code> on a <code>Window</code>,
    '''     <code>Frame</code> or <code>Dialog</code> are forwarded
    '''     to the underlying window management system and may be
    '''     ignored or modified.  See <seealso cref="java.awt.Window"/> for
    '''     more information.
    ''' </ul>
    ''' <p>
    ''' Most applications should not call any of the methods in this
    ''' class directly. The methods defined by <code>Toolkit</code> are
    ''' the "glue" that joins the platform-independent classes in the
    ''' <code>java.awt</code> package with their counterparts in
    ''' <code>java.awt.peer</code>. Some methods defined by
    ''' <code>Toolkit</code> query the native operating system directly.
    ''' 
    ''' @author      Sami Shaio
    ''' @author      Arthur van Hoff
    ''' @author      Fred Ecks
    ''' @since       JDK1.0
    ''' </summary>
    Public MustInherit Class Toolkit

        ''' <summary>
        ''' Creates this toolkit's implementation of the <code>Desktop</code>
        ''' using the specified peer interface. </summary>
        ''' <param name="target"> the desktop to be implemented </param>
        ''' <returns>    this toolkit's implementation of the <code>Desktop</code> </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Desktop </seealso>
        ''' <seealso cref=       java.awt.peer.DesktopPeer
        ''' @since 1.6 </seealso>
        Protected Friend MustOverride Function createDesktopPeer(ByVal target As Desktop) As DesktopPeer


        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Button</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the button to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Button</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Button </seealso>
        ''' <seealso cref=       java.awt.peer.ButtonPeer </seealso>
        Protected Friend MustOverride Function createButton(ByVal target As Button) As ButtonPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>TextField</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the text field to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>TextField</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.TextField </seealso>
        ''' <seealso cref=       java.awt.peer.TextFieldPeer </seealso>
        Protected Friend MustOverride Function createTextField(ByVal target As TextField) As TextFieldPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Label</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the label to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Label</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Label </seealso>
        ''' <seealso cref=       java.awt.peer.LabelPeer </seealso>
        Protected Friend MustOverride Function createLabel(ByVal target As Label) As LabelPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>List</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the list to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>List</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.List </seealso>
        ''' <seealso cref=       java.awt.peer.ListPeer </seealso>
        Protected Friend MustOverride Function createList(ByVal target As java.awt.List) As ListPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Checkbox</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the check box to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Checkbox</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Checkbox </seealso>
        ''' <seealso cref=       java.awt.peer.CheckboxPeer </seealso>
        Protected Friend MustOverride Function createCheckbox(ByVal target As Checkbox) As CheckboxPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Scrollbar</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the scroll bar to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Scrollbar</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Scrollbar </seealso>
        ''' <seealso cref=       java.awt.peer.ScrollbarPeer </seealso>
        Protected Friend MustOverride Function createScrollbar(ByVal target As Scrollbar) As ScrollbarPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>ScrollPane</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the scroll pane to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>ScrollPane</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.ScrollPane </seealso>
        ''' <seealso cref=       java.awt.peer.ScrollPanePeer
        ''' @since     JDK1.1 </seealso>
        Protected Friend MustOverride Function createScrollPane(ByVal target As ScrollPane) As ScrollPanePeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>TextArea</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the text area to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>TextArea</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.TextArea </seealso>
        ''' <seealso cref=       java.awt.peer.TextAreaPeer </seealso>
        Protected Friend MustOverride Function createTextArea(ByVal target As TextArea) As TextAreaPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Choice</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the choice to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Choice</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Choice </seealso>
        ''' <seealso cref=       java.awt.peer.ChoicePeer </seealso>
        Protected Friend MustOverride Function createChoice(ByVal target As Choice) As ChoicePeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Frame</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the frame to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Frame</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Frame </seealso>
        ''' <seealso cref=       java.awt.peer.FramePeer </seealso>
        Protected Friend MustOverride Function createFrame(ByVal target As Frame) As FramePeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Canvas</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the canvas to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Canvas</code>. </returns>
        ''' <seealso cref=       java.awt.Canvas </seealso>
        ''' <seealso cref=       java.awt.peer.CanvasPeer </seealso>
        Protected Friend MustOverride Function createCanvas(ByVal target As Canvas) As CanvasPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Panel</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the panel to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Panel</code>. </returns>
        ''' <seealso cref=       java.awt.Panel </seealso>
        ''' <seealso cref=       java.awt.peer.PanelPeer </seealso>
        Protected Friend MustOverride Function createPanel(ByVal target As Panel) As PanelPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Window</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the window to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Window</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Window </seealso>
        ''' <seealso cref=       java.awt.peer.WindowPeer </seealso>
        Protected Friend MustOverride Function createWindow(ByVal target As Window) As WindowPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Dialog</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the dialog to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Dialog</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Dialog </seealso>
        ''' <seealso cref=       java.awt.peer.DialogPeer </seealso>
        Protected Friend MustOverride Function createDialog(ByVal target As Dialog) As DialogPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>MenuBar</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the menu bar to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>MenuBar</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.MenuBar </seealso>
        ''' <seealso cref=       java.awt.peer.MenuBarPeer </seealso>
        Protected Friend MustOverride Function createMenuBar(ByVal target As MenuBar) As MenuBarPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Menu</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the menu to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>Menu</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.Menu </seealso>
        ''' <seealso cref=       java.awt.peer.MenuPeer </seealso>
        Protected Friend MustOverride Function createMenu(ByVal target As Menu) As MenuPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>PopupMenu</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the popup menu to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>PopupMenu</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.PopupMenu </seealso>
        ''' <seealso cref=       java.awt.peer.PopupMenuPeer
        ''' @since     JDK1.1 </seealso>
        Protected Friend MustOverride Function createPopupMenu(ByVal target As PopupMenu) As PopupMenuPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>MenuItem</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the menu item to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>MenuItem</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.MenuItem </seealso>
        ''' <seealso cref=       java.awt.peer.MenuItemPeer </seealso>
        Protected Friend MustOverride Function createMenuItem(ByVal target As MenuItem) As MenuItemPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>FileDialog</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the file dialog to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>FileDialog</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.FileDialog </seealso>
        ''' <seealso cref=       java.awt.peer.FileDialogPeer </seealso>
        Protected Friend MustOverride Function createFileDialog(ByVal target As FileDialog) As FileDialogPeer

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>CheckboxMenuItem</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="target"> the checkbox menu item to be implemented. </param>
        ''' <returns>    this toolkit's implementation of <code>CheckboxMenuItem</code>. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.CheckboxMenuItem </seealso>
        ''' <seealso cref=       java.awt.peer.CheckboxMenuItemPeer </seealso>
        Protected Friend MustOverride Function createCheckboxMenuItem(ByVal target As CheckboxMenuItem) As CheckboxMenuItemPeer

        ''' <summary>
        ''' Obtains this toolkit's implementation of helper class for
        ''' <code>MouseInfo</code> operations. </summary>
        ''' <returns>    this toolkit's implementation of  helper for <code>MouseInfo</code> </returns>
        ''' <exception cref="UnsupportedOperationException"> if this operation is not implemented </exception>
        ''' <seealso cref=       java.awt.peer.MouseInfoPeer </seealso>
        ''' <seealso cref=       java.awt.MouseInfo
        ''' @since 1.5 </seealso>
        Protected Friend Overridable Property mouseInfoPeer As MouseInfoPeer
            Get
                Throw New UnsupportedOperationException("Not implemented")
            End Get
        End Property

        Private Shared lightweightMarker As LightweightPeer

        ''' <summary>
        ''' Creates a peer for a component or container.  This peer is windowless
        ''' and allows the Component and Container classes to be extended directly
        ''' to create windowless components that are defined entirely in java.
        ''' </summary>
        ''' <param name="target"> The Component to be created. </param>
        Protected Friend Overridable Function createComponent(ByVal target As Component) As LightweightPeer
            If lightweightMarker Is Nothing Then lightweightMarker = New sun.awt.NullComponentPeer
            Return lightweightMarker
        End Function

        ''' <summary>
        ''' Creates this toolkit's implementation of <code>Font</code> using
        ''' the specified peer interface. </summary>
        ''' <param name="name"> the font to be implemented </param>
        ''' <param name="style"> the style of the font, such as <code>PLAIN</code>,
        '''            <code>BOLD</code>, <code>ITALIC</code>, or a combination </param>
        ''' <returns>    this toolkit's implementation of <code>Font</code> </returns>
        ''' <seealso cref=       java.awt.Font </seealso>
        ''' <seealso cref=       java.awt.peer.FontPeer </seealso>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#getAllFonts </seealso>
        ''' @deprecated  see java.awt.GraphicsEnvironment#getAllFonts 
        <Obsolete(" see java.awt.GraphicsEnvironment#getAllFonts")>
        Protected Friend MustOverride Function getFontPeer(ByVal name As String, ByVal style As Integer) As FontPeer

        ' The following method is called by the private method
        ' <code>updateSystemColors</code> in <code>SystemColor</code>.

        ''' <summary>
        ''' Fills in the integer array that is supplied as an argument
        ''' with the current system color values.
        ''' </summary>
        ''' <param name="systemColors"> an integer array. </param>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     JDK1.1 </seealso>
        Protected Friend Overridable Sub loadSystemColors(ByVal systemColors As Integer())
            GraphicsEnvironment.checkHeadless()
        End Sub

        ''' <summary>
        ''' Controls whether the layout of Containers is validated dynamically
        ''' during resizing, or statically, after resizing is complete.
        ''' Use {@code isDynamicLayoutActive()} to detect if this feature enabled
        ''' in this program and is supported by this operating system
        ''' and/or window manager.
        ''' Note that this feature is supported not on all platforms, and
        ''' conversely, that this feature cannot be turned off on some platforms.
        ''' On these platforms where dynamic layout during resizing is not supported
        ''' (or is always supported), setting this property has no effect.
        ''' Note that this feature can be set or unset as a property of the
        ''' operating system or window manager on some platforms.  On such
        ''' platforms, the dynamic resize property must be set at the operating
        ''' system or window manager level before this method can take effect.
        ''' This method does not change support or settings of the underlying
        ''' operating system or
        ''' window manager.  The OS/WM support can be
        ''' queried using getDesktopProperty("awt.dynamicLayoutSupported") method.
        ''' </summary>
        ''' <param name="dynamic">  If true, Containers should re-layout their
        '''            components as the Container is being resized.  If false,
        '''            the layout will be validated after resizing is completed. </param>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        '''            returns true </exception>
        ''' <seealso cref=       #isDynamicLayoutSet() </seealso>
        ''' <seealso cref=       #isDynamicLayoutActive() </seealso>
        ''' <seealso cref=       #getDesktopProperty(String propertyName) </seealso>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.4 </seealso>
        Public Overridable Property dynamicLayout As Boolean
            Set(ByVal dynamic As Boolean)
                GraphicsEnvironment.checkHeadless()
                If Me IsNot defaultToolkit Then defaultToolkit.dynamicLayout = dynamic
            End Set
        End Property

        ''' <summary>
        ''' Returns whether the layout of Containers is validated dynamically
        ''' during resizing, or statically, after resizing is complete.
        ''' Note: this method returns the value that was set programmatically;
        ''' it does not reflect support at the level of the operating system
        ''' or window manager for dynamic layout on resizing, or the current
        ''' operating system or window manager settings.  The OS/WM support can
        ''' be queried using getDesktopProperty("awt.dynamicLayoutSupported").
        ''' </summary>
        ''' <returns>    true if validation of Containers is done dynamically,
        '''            false if validation is done after resizing is finished. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        '''            returns true </exception>
        ''' <seealso cref=       #setDynamicLayout(boolean dynamic) </seealso>
        ''' <seealso cref=       #isDynamicLayoutActive() </seealso>
        ''' <seealso cref=       #getDesktopProperty(String propertyName) </seealso>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.4 </seealso>
        Protected Friend Overridable ReadOnly Property dynamicLayoutSet As Boolean
            Get
                GraphicsEnvironment.checkHeadless()

                If Me IsNot Toolkit.defaultToolkit Then
                    Return Toolkit.defaultToolkit.dynamicLayoutSet
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns whether dynamic layout of Containers on resize is
        ''' currently active (both set in program
        ''' ( {@code isDynamicLayoutSet()} )
        ''' , and supported
        ''' by the underlying operating system and/or window manager).
        ''' If dynamic layout is currently inactive then Containers
        ''' re-layout their components when resizing is completed. As a result
        ''' the {@code Component.validate()} method will be invoked only
        ''' once per resize.
        ''' If dynamic layout is currently active then Containers
        ''' re-layout their components on every native resize event and
        ''' the {@code validate()} method will be invoked each time.
        ''' The OS/WM support can be queried using
        ''' the getDesktopProperty("awt.dynamicLayoutSupported") method.
        ''' </summary>
        ''' <returns>    true if dynamic layout of Containers on resize is
        '''            currently active, false otherwise. </returns>
        ''' <exception cref="HeadlessException"> if the GraphicsEnvironment.isHeadless()
        '''            method returns true </exception>
        ''' <seealso cref=       #setDynamicLayout(boolean dynamic) </seealso>
        ''' <seealso cref=       #isDynamicLayoutSet() </seealso>
        ''' <seealso cref=       #getDesktopProperty(String propertyName) </seealso>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.4 </seealso>
        Public Overridable ReadOnly Property dynamicLayoutActive As Boolean
            Get
                GraphicsEnvironment.checkHeadless()

                If Me IsNot Toolkit.defaultToolkit Then
                    Return Toolkit.defaultToolkit.dynamicLayoutActive
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the size of the screen.  On systems with multiple displays, the
        ''' primary display is used.  Multi-screen aware display dimensions are
        ''' available from <code>GraphicsConfiguration</code> and
        ''' <code>GraphicsDevice</code>. </summary>
        ''' <returns>    the size of this toolkit's screen, in pixels. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsConfiguration#getBounds </seealso>
        ''' <seealso cref=       java.awt.GraphicsDevice#getDisplayMode </seealso>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        Public MustOverride ReadOnly Property screenSize As Dimension

        ''' <summary>
        ''' Returns the screen resolution in dots-per-inch. </summary>
        ''' <returns>    this toolkit's screen resolution, in dots-per-inch. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        Public MustOverride ReadOnly Property screenResolution As Integer

        ''' <summary>
        ''' Gets the insets of the screen. </summary>
        ''' <param name="gc"> a <code>GraphicsConfiguration</code> </param>
        ''' <returns>    the insets of this toolkit's screen, in pixels. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.4 </seealso>
        Public Overridable Function getScreenInsets(ByVal gc As GraphicsConfiguration) As Insets
            GraphicsEnvironment.checkHeadless()
            If Me IsNot Toolkit.defaultToolkit Then
                Return Toolkit.defaultToolkit.getScreenInsets(gc)
            Else
                Return New Insets(0, 0, 0, 0)
            End If
        End Function

        ''' <summary>
        ''' Determines the color model of this toolkit's screen.
        ''' <p>
        ''' <code>ColorModel</code> is an abstract class that
        ''' encapsulates the ability to translate between the
        ''' pixel values of an image and its red, green, blue,
        ''' and alpha components.
        ''' <p>
        ''' This toolkit method is called by the
        ''' <code>getColorModel</code> method
        ''' of the <code>Component</code> class. </summary>
        ''' <returns>    the color model of this toolkit's screen. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.image.ColorModel </seealso>
        ''' <seealso cref=       java.awt.Component#getColorModel </seealso>
        Public MustOverride ReadOnly Property colorModel As java.awt.image.ColorModel

        ''' <summary>
        ''' Returns the names of the available fonts in this toolkit.<p>
        ''' For 1.1, the following font names are deprecated (the replacement
        ''' name follows):
        ''' <ul>
        ''' <li>TimesRoman (use Serif)
        ''' <li>Helvetica (use SansSerif)
        ''' <li>Courier (use Monospaced)
        ''' </ul><p>
        ''' The ZapfDingbats fontname is also deprecated in 1.1 but the characters
        ''' are defined in Unicode starting at 0x2700, and as of 1.1 Java supports
        ''' those characters. </summary>
        ''' <returns>    the names of the available fonts in this toolkit. </returns>
        ''' @deprecated see <seealso cref="java.awt.GraphicsEnvironment#getAvailableFontFamilyNames()"/> 
        ''' <seealso cref= java.awt.GraphicsEnvironment#getAvailableFontFamilyNames() </seealso>
        <Obsolete("see <seealso cref="java.awt.GraphicsEnvironment#getAvailableFontFamilyNames()"/>")> _
		Public MustOverride ReadOnly Property fontList As String()

		''' <summary>
		''' Gets the screen device metrics for rendering of the font. </summary>
		''' <param name="font">   a font </param>
		''' <returns>    the screen metrics of the specified font in this toolkit </returns>
		''' @deprecated  As of JDK version 1.2, replaced by the <code>Font</code>
		'''          method <code>getLineMetrics</code>. 
		''' <seealso cref= java.awt.font.LineMetrics </seealso>
		''' <seealso cref= java.awt.Font#getLineMetrics </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#getScreenDevices </seealso>
		<Obsolete(" As of JDK version 1.2, replaced by the <code>Font</code>")>
        Public MustOverride Function getFontMetrics(ByVal font As font) As FontMetrics

        ''' <summary>
        ''' Synchronizes this toolkit's graphics state. Some window systems
        ''' may do buffering of graphics events.
        ''' <p>
        ''' This method ensures that the display is up-to-date. It is useful
        ''' for animation.
        ''' </summary>
        Public MustOverride Sub sync()

        ''' <summary>
        ''' The default toolkit.
        ''' </summary>
        Private Shared toolkit_Renamed As Toolkit

        ''' <summary>
        ''' Used internally by the assistive technologies functions; set at
        ''' init time and used at load time
        ''' </summary>
        Private Shared atNames As String

        ''' <summary>
        ''' Initializes properties related to assistive technologies.
        ''' These properties are used both in the loadAssistiveProperties()
        ''' function below, as well as other classes in the jdk that depend
        ''' on the properties (such as the use of the screen_magnifier_present
        ''' property in Java2D hardware acceleration initialization).  The
        ''' initialization of the properties must be done before the platform-
        ''' specific Toolkit class is instantiated so that all necessary
        ''' properties are set up properly before any classes dependent upon them
        ''' are initialized.
        ''' </summary>
        Private Shared Sub initAssistiveTechnologies()

            ' Get accessibility properties
            Dim sep As String = File.separator
            Dim properties As New Properties


            atNames = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As String

                ' Try loading the per-user accessibility properties file.
                Try
                    Dim propsFile As New File(System.getProperty("user.home") + sep & ".accessibility.properties")
                    Dim [in] As New java.io.FileInputStream(propsFile)

                    ' Inputstream has been buffered in Properties class
                    Properties.load([in])
                    [in].close()
                Catch e As Exception
                    ' Per-user accessibility properties file does not exist
                End Try

                ' Try loading the system-wide accessibility properties
                ' file only if a per-user accessibility properties
                ' file does not exist or is empty.
                If Properties.size() = 0 Then
                    Try
                        Dim propsFile As New File(System.getProperty("java.home") + sep & "lib" & sep & "accessibility.properties")
                        Dim [in] As New java.io.FileInputStream(propsFile)

                        ' Inputstream has been buffered in Properties class
                        Properties.load([in])
                        [in].close()
                    Catch e As Exception
                        ' System-wide accessibility properties file does
                        ' not exist;
                    End Try
                End If

                ' Get whether a screen magnifier is present.  First check
                ' the system property and then check the properties file.
                Dim magPresent As String = System.getProperty("javax.accessibility.screen_magnifier_present")
                If magPresent Is Nothing Then
                    magPresent = Properties.getProperty("screen_magnifier_present", Nothing)
                    If magPresent IsNot Nothing Then System.propertyrty("javax.accessibility.screen_magnifier_present", magPresent)
                End If

                ' Get the names of any assistive technolgies to load.  First
                ' check the system property and then check the properties
                ' file.
                Dim classNames As String = System.getProperty("javax.accessibility.assistive_technologies")
                If classNames Is Nothing Then
                    classNames = Properties.getProperty("assistive_technologies", Nothing)
                    If classNames IsNot Nothing Then System.propertyrty("javax.accessibility.assistive_technologies", classNames)
                End If
                Return classNames
            End Function
        End Class

        ''' <summary>
        ''' Loads additional classes into the VM, using the property
        ''' 'assistive_technologies' specified in the Sun reference
        ''' implementation by a line in the 'accessibility.properties'
        ''' file.  The form is "assistive_technologies=..." where
        ''' the "..." is a comma-separated list of assistive technology
        ''' classes to load.  Each class is loaded in the order given
        ''' and a single instance of each is created using
        ''' Class.forName(class).newInstance().  All errors are handled
        ''' via an AWTError exception.
        ''' 
        ''' <p>The assumption is made that assistive technology classes are supplied
        ''' as part of INSTALLED (as opposed to: BUNDLED) extensions or specified
        ''' on the class path
        ''' (and therefore can be loaded using the class loader returned by
        ''' a call to <code>ClassLoader.getSystemClassLoader</code>, whose
        ''' delegation parent is the extension class loader for installed
        ''' extensions).
        ''' </summary>
        Private Shared Sub loadAssistiveTechnologies()
            ' Load any assistive technologies
            If atNames IsNot Nothing Then
                Dim cl As ClassLoader = ClassLoader.systemClassLoader
                Dim parser As New StringTokenizer(atNames, " ,")
                Dim atName As String
                Do While parser.hasMoreTokens()
                    atName = parser.nextToken()
                    Try
                        Dim clazz As [Class]
                        If cl IsNot Nothing Then
                            clazz = cl.loadClass(atName)
                        Else
                            clazz = Type.GetType(atName)
                        End If
                        clazz.newInstance()
                    Catch e As ClassNotFoundException
                        Throw New AWTError("Assistive Technology not found: " & atName)
                    Catch e As InstantiationException
                        Throw New AWTError("Could not instantiate Assistive" & " Technology: " & atName)
                    Catch e As IllegalAccessException
                        Throw New AWTError("Could not access Assistive" & " Technology: " & atName)
                    Catch e As Exception
                        Throw New AWTError("Error trying to install Assistive" & " Technology: " & atName & " " & e)
                    End Try
                Loop
            End If
        End Sub

        ''' <summary>
        ''' Gets the default toolkit.
        ''' <p>
        ''' If a system property named <code>"java.awt.headless"</code> is set
        ''' to <code>true</code> then the headless implementation
        ''' of <code>Toolkit</code> is used.
        ''' <p>
        ''' If there is no <code>"java.awt.headless"</code> or it is set to
        ''' <code>false</code> and there is a system property named
        ''' <code>"awt.toolkit"</code>,
        ''' that property is treated as the name of a class that is a subclass
        ''' of <code>Toolkit</code>;
        ''' otherwise the default platform-specific implementation of
        ''' <code>Toolkit</code> is used.
        ''' <p>
        ''' Also loads additional classes into the VM, using the property
        ''' 'assistive_technologies' specified in the Sun reference
        ''' implementation by a line in the 'accessibility.properties'
        ''' file.  The form is "assistive_technologies=..." where
        ''' the "..." is a comma-separated list of assistive technology
        ''' classes to load.  Each class is loaded in the order given
        ''' and a single instance of each is created using
        ''' Class.forName(class).newInstance().  This is done just after
        ''' the AWT toolkit is created.  All errors are handled via an
        ''' AWTError exception. </summary>
        ''' <returns>    the default toolkit. </returns>
        ''' <exception cref="AWTError">  if a toolkit could not be found, or
        '''                 if one could not be accessed or instantiated. </exception>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Shared ReadOnly Property defaultToolkit As Toolkit
            Get
                If toolkit_Renamed Is Nothing Then
                    java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T))
                    loadAssistiveTechnologies()
                End If
                Return toolkit_Renamed
            End Get
        End Property

        Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                Dim cls As [Class] = Nothing
                Dim nm As String = System.getProperty("awt.toolkit")
                Try
                    cls = Type.GetType(nm)
                Catch e As ClassNotFoundException
                    Dim cl As ClassLoader = ClassLoader.systemClassLoader
                    If cl IsNot Nothing Then
                        Try
                            cls = cl.loadClass(nm)
                        Catch ignored As ClassNotFoundException
                            Throw New AWTError("Toolkit not found: " & nm)
                        End Try
                    End If
                End Try
                Try
                    If cls IsNot Nothing Then
                        toolkit_Renamed = CType(cls.newInstance(), Toolkit)
                        If GraphicsEnvironment.headless Then toolkit_Renamed = New sun.awt.HeadlessToolkit(toolkit_Renamed)
                    End If
                Catch ignored As InstantiationException
                    Throw New AWTError("Could not instantiate Toolkit: " & nm)
                Catch ignored As IllegalAccessException
                    Throw New AWTError("Could not access Toolkit: " & nm)
                End Try
                Return Nothing
            End Function
        End Class

        ''' <summary>
        ''' Returns an image which gets pixel data from the specified file,
        ''' whose format can be either GIF, JPEG or PNG.
        ''' The underlying toolkit attempts to resolve multiple requests
        ''' with the same filename to the same returned Image.
        ''' <p>
        ''' Since the mechanism required to facilitate this sharing of
        ''' <code>Image</code> objects may continue to hold onto images
        ''' that are no longer in use for an indefinite period of time,
        ''' developers are encouraged to implement their own caching of
        ''' images by using the <seealso cref="#createImage(java.lang.String) createImage"/>
        ''' variant wherever available.
        ''' If the image data contained in the specified file changes,
        ''' the <code>Image</code> object returned from this method may
        ''' still contain stale information which was loaded from the
        ''' file after a prior call.
        ''' Previously loaded image data can be manually discarded by
        ''' calling the <seealso cref="Image#flush flush"/> method on the
        ''' returned <code>Image</code>.
        ''' <p>
        ''' This method first checks if there is a security manager installed.
        ''' If so, the method calls the security manager's
        ''' <code>checkRead</code> method with the file specified to ensure
        ''' that the access to the image is allowed. </summary>
        ''' <param name="filename">   the name of a file containing pixel data
        '''                         in a recognized file format. </param>
        ''' <returns>    an image which gets its pixel data from
        '''                         the specified file. </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''                            checkRead method doesn't allow the operation. </exception>
        ''' <seealso cref= #createImage(java.lang.String) </seealso>
        Public MustOverride Function getImage(ByVal filename As String) As image

        ''' <summary>
        ''' Returns an image which gets pixel data from the specified URL.
        ''' The pixel data referenced by the specified URL must be in one
        ''' of the following formats: GIF, JPEG or PNG.
        ''' The underlying toolkit attempts to resolve multiple requests
        ''' with the same URL to the same returned Image.
        ''' <p>
        ''' Since the mechanism required to facilitate this sharing of
        ''' <code>Image</code> objects may continue to hold onto images
        ''' that are no longer in use for an indefinite period of time,
        ''' developers are encouraged to implement their own caching of
        ''' images by using the <seealso cref="#createImage(java.net.URL) createImage"/>
        ''' variant wherever available.
        ''' If the image data stored at the specified URL changes,
        ''' the <code>Image</code> object returned from this method may
        ''' still contain stale information which was fetched from the
        ''' URL after a prior call.
        ''' Previously loaded image data can be manually discarded by
        ''' calling the <seealso cref="Image#flush flush"/> method on the
        ''' returned <code>Image</code>.
        ''' <p>
        ''' This method first checks if there is a security manager installed.
        ''' If so, the method calls the security manager's
        ''' <code>checkPermission</code> method with the
        ''' url.openConnection().getPermission() permission to ensure
        ''' that the access to the image is allowed. For compatibility
        ''' with pre-1.2 security managers, if the access is denied with
        ''' <code>FilePermission</code> or <code>SocketPermission</code>,
        ''' the method throws the <code>SecurityException</code>
        ''' if the corresponding 1.1-style SecurityManager.checkXXX method
        ''' also denies permission. </summary>
        ''' <param name="url">   the URL to use in fetching the pixel data. </param>
        ''' <returns>    an image which gets its pixel data from
        '''                         the specified URL. </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''                            checkPermission method doesn't allow
        '''                            the operation. </exception>
        ''' <seealso cref= #createImage(java.net.URL) </seealso>
        Public MustOverride Function getImage(ByVal url As java.net.URL) As image

        ''' <summary>
        ''' Returns an image which gets pixel data from the specified file.
        ''' The returned Image is a new object which will not be shared
        ''' with any other caller of this method or its getImage variant.
        ''' <p>
        ''' This method first checks if there is a security manager installed.
        ''' If so, the method calls the security manager's
        ''' <code>checkRead</code> method with the specified file to ensure
        ''' that the image creation is allowed. </summary>
        ''' <param name="filename">   the name of a file containing pixel data
        '''                         in a recognized file format. </param>
        ''' <returns>    an image which gets its pixel data from
        '''                         the specified file. </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''                            checkRead method doesn't allow the operation. </exception>
        ''' <seealso cref= #getImage(java.lang.String) </seealso>
        Public MustOverride Function createImage(ByVal filename As String) As image

        ''' <summary>
        ''' Returns an image which gets pixel data from the specified URL.
        ''' The returned Image is a new object which will not be shared
        ''' with any other caller of this method or its getImage variant.
        ''' <p>
        ''' This method first checks if there is a security manager installed.
        ''' If so, the method calls the security manager's
        ''' <code>checkPermission</code> method with the
        ''' url.openConnection().getPermission() permission to ensure
        ''' that the image creation is allowed. For compatibility
        ''' with pre-1.2 security managers, if the access is denied with
        ''' <code>FilePermission</code> or <code>SocketPermission</code>,
        ''' the method throws <code>SecurityException</code>
        ''' if the corresponding 1.1-style SecurityManager.checkXXX method
        ''' also denies permission. </summary>
        ''' <param name="url">   the URL to use in fetching the pixel data. </param>
        ''' <returns>    an image which gets its pixel data from
        '''                         the specified URL. </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''                            checkPermission method doesn't allow
        '''                            the operation. </exception>
        ''' <seealso cref= #getImage(java.net.URL) </seealso>
        Public MustOverride Function createImage(ByVal url As java.net.URL) As image

        ''' <summary>
        ''' Prepares an image for rendering.
        ''' <p>
        ''' If the values of the width and height arguments are both
        ''' <code>-1</code>, this method prepares the image for rendering
        ''' on the default screen; otherwise, this method prepares an image
        ''' for rendering on the default screen at the specified width and height.
        ''' <p>
        ''' The image data is downloaded asynchronously in another thread,
        ''' and an appropriately scaled screen representation of the image is
        ''' generated.
        ''' <p>
        ''' This method is called by components <code>prepareImage</code>
        ''' methods.
        ''' <p>
        ''' Information on the flags returned by this method can be found
        ''' with the definition of the <code>ImageObserver</code> interface.
        ''' </summary>
        ''' <param name="image">      the image for which to prepare a
        '''                           screen representation. </param>
        ''' <param name="width">      the width of the desired screen
        '''                           representation, or <code>-1</code>. </param>
        ''' <param name="height">     the height of the desired screen
        '''                           representation, or <code>-1</code>. </param>
        ''' <param name="observer">   the <code>ImageObserver</code>
        '''                           object to be notified as the
        '''                           image is being prepared. </param>
        ''' <returns>    <code>true</code> if the image has already been
        '''                 fully prepared; <code>false</code> otherwise. </returns>
        ''' <seealso cref=       java.awt.Component#prepareImage(java.awt.Image,
        '''                 java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=       java.awt.Component#prepareImage(java.awt.Image,
        '''                 int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=       java.awt.image.ImageObserver </seealso>
        Public MustOverride Function prepareImage(ByVal image As image, ByVal width As Integer, ByVal height As Integer, ByVal observer As java.awt.image.ImageObserver) As Boolean

        ''' <summary>
        ''' Indicates the construction status of a specified image that is
        ''' being prepared for display.
        ''' <p>
        ''' If the values of the width and height arguments are both
        ''' <code>-1</code>, this method returns the construction status of
        ''' a screen representation of the specified image in this toolkit.
        ''' Otherwise, this method returns the construction status of a
        ''' scaled representation of the image at the specified width
        ''' and height.
        ''' <p>
        ''' This method does not cause the image to begin loading.
        ''' An application must call <code>prepareImage</code> to force
        ''' the loading of an image.
        ''' <p>
        ''' This method is called by the component's <code>checkImage</code>
        ''' methods.
        ''' <p>
        ''' Information on the flags returned by this method can be found
        ''' with the definition of the <code>ImageObserver</code> interface. </summary>
        ''' <param name="image">   the image whose status is being checked. </param>
        ''' <param name="width">   the width of the scaled version whose status is
        '''                 being checked, or <code>-1</code>. </param>
        ''' <param name="height">  the height of the scaled version whose status
        '''                 is being checked, or <code>-1</code>. </param>
        ''' <param name="observer">   the <code>ImageObserver</code> object to be
        '''                 notified as the image is being prepared. </param>
        ''' <returns>    the bitwise inclusive <strong>OR</strong> of the
        '''                 <code>ImageObserver</code> flags for the
        '''                 image data that is currently available. </returns>
        ''' <seealso cref=       java.awt.Toolkit#prepareImage(java.awt.Image,
        '''                 int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=       java.awt.Component#checkImage(java.awt.Image,
        '''                 java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=       java.awt.Component#checkImage(java.awt.Image,
        '''                 int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=       java.awt.image.ImageObserver </seealso>
        Public MustOverride Function checkImage(ByVal image As image, ByVal width As Integer, ByVal height As Integer, ByVal observer As java.awt.image.ImageObserver) As Integer

        ''' <summary>
        ''' Creates an image with the specified image producer. </summary>
        ''' <param name="producer"> the image producer to be used. </param>
        ''' <returns>    an image with the specified image producer. </returns>
        ''' <seealso cref=       java.awt.Image </seealso>
        ''' <seealso cref=       java.awt.image.ImageProducer </seealso>
        ''' <seealso cref=       java.awt.Component#createImage(java.awt.image.ImageProducer) </seealso>
        Public MustOverride Function createImage(ByVal producer As java.awt.image.ImageProducer) As image

        ''' <summary>
        ''' Creates an image which decodes the image stored in the specified
        ''' byte array.
        ''' <p>
        ''' The data must be in some image format, such as GIF or JPEG,
        ''' that is supported by this toolkit. </summary>
        ''' <param name="imagedata">   an array of bytes, representing
        '''                         image data in a supported image format. </param>
        ''' <returns>    an image.
        ''' @since     JDK1.1 </returns>
        Public Overridable Function createImage(ByVal imagedata As SByte()) As image
            Return createImage(imagedata, 0, imagedata.Length)
        End Function

        ''' <summary>
        ''' Creates an image which decodes the image stored in the specified
        ''' byte array, and at the specified offset and length.
        ''' The data must be in some image format, such as GIF or JPEG,
        ''' that is supported by this toolkit. </summary>
        ''' <param name="imagedata">   an array of bytes, representing
        '''                         image data in a supported image format. </param>
        ''' <param name="imageoffset">  the offset of the beginning
        '''                         of the data in the array. </param>
        ''' <param name="imagelength">  the length of the data in the array. </param>
        ''' <returns>    an image.
        ''' @since     JDK1.1 </returns>
        Public MustOverride Function createImage(ByVal imagedata As SByte(), ByVal imageoffset As Integer, ByVal imagelength As Integer) As image

        ''' <summary>
        ''' Gets a <code>PrintJob</code> object which is the result of initiating
        ''' a print operation on the toolkit's platform.
        ''' <p>
        ''' Each actual implementation of this method should first check if there
        ''' is a security manager installed. If there is, the method should call
        ''' the security manager's <code>checkPrintJobAccess</code> method to
        ''' ensure initiation of a print operation is allowed. If the default
        ''' implementation of <code>checkPrintJobAccess</code> is used (that is,
        ''' that method is not overriden), then this results in a call to the
        ''' security manager's <code>checkPermission</code> method with a <code>
        ''' RuntimePermission("queuePrintJob")</code> permission.
        ''' </summary>
        ''' <param name="frame"> the parent of the print dialog. May not be null. </param>
        ''' <param name="jobtitle"> the title of the PrintJob. A null title is equivalent
        '''          to "". </param>
        ''' <param name="props"> a Properties object containing zero or more properties.
        '''          Properties are not standardized and are not consistent across
        '''          implementations. Because of this, PrintJobs which require job
        '''          and page control should use the version of this function which
        '''          takes JobAttributes and PageAttributes objects. This object
        '''          may be updated to reflect the user's job choices on exit. May
        '''          be null. </param>
        ''' <returns>  a <code>PrintJob</code> object, or <code>null</code> if the
        '''          user cancelled the print job. </returns>
        ''' <exception cref="NullPointerException"> if frame is null </exception>
        ''' <exception cref="SecurityException"> if this thread is not allowed to initiate a
        '''          print job request </exception>
        ''' <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=     java.awt.PrintJob </seealso>
        ''' <seealso cref=     java.lang.RuntimePermission
        ''' @since   JDK1.1 </seealso>
        Public MustOverride Function getPrintJob(ByVal frame As Frame, ByVal jobtitle As String, ByVal props As Properties) As PrintJob

        ''' <summary>
        ''' Gets a <code>PrintJob</code> object which is the result of initiating
        ''' a print operation on the toolkit's platform.
        ''' <p>
        ''' Each actual implementation of this method should first check if there
        ''' is a security manager installed. If there is, the method should call
        ''' the security manager's <code>checkPrintJobAccess</code> method to
        ''' ensure initiation of a print operation is allowed. If the default
        ''' implementation of <code>checkPrintJobAccess</code> is used (that is,
        ''' that method is not overriden), then this results in a call to the
        ''' security manager's <code>checkPermission</code> method with a <code>
        ''' RuntimePermission("queuePrintJob")</code> permission.
        ''' </summary>
        ''' <param name="frame"> the parent of the print dialog. May not be null. </param>
        ''' <param name="jobtitle"> the title of the PrintJob. A null title is equivalent
        '''          to "". </param>
        ''' <param name="jobAttributes"> a set of job attributes which will control the
        '''          PrintJob. The attributes will be updated to reflect the user's
        '''          choices as outlined in the JobAttributes documentation. May be
        '''          null. </param>
        ''' <param name="pageAttributes"> a set of page attributes which will control the
        '''          PrintJob. The attributes will be applied to every page in the
        '''          job. The attributes will be updated to reflect the user's
        '''          choices as outlined in the PageAttributes documentation. May be
        '''          null. </param>
        ''' <returns>  a <code>PrintJob</code> object, or <code>null</code> if the
        '''          user cancelled the print job. </returns>
        ''' <exception cref="NullPointerException"> if frame is null </exception>
        ''' <exception cref="IllegalArgumentException"> if pageAttributes specifies differing
        '''          cross feed and feed resolutions. Also if this thread has
        '''          access to the file system and jobAttributes specifies
        '''          print to file, and the specified destination file exists but
        '''          is a directory rather than a regular file, does not exist but
        '''          cannot be created, or cannot be opened for any other reason.
        '''          However in the case of print to file, if a dialog is also
        '''          requested to be displayed then the user will be given an
        '''          opportunity to select a file and proceed with printing.
        '''          The dialog will ensure that the selected output file
        '''          is valid before returning from this method. </exception>
        ''' <exception cref="SecurityException"> if this thread is not allowed to initiate a
        '''          print job request, or if jobAttributes specifies print to file,
        '''          and this thread is not allowed to access the file system </exception>
        ''' <seealso cref=     java.awt.PrintJob </seealso>
        ''' <seealso cref=     java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=     java.lang.RuntimePermission </seealso>
        ''' <seealso cref=     java.awt.JobAttributes </seealso>
        ''' <seealso cref=     java.awt.PageAttributes
        ''' @since   1.3 </seealso>
        Public Overridable Function getPrintJob(ByVal frame_Renamed As Frame, ByVal jobtitle As String, ByVal jobAttributes As JobAttributes, ByVal pageAttributes As PageAttributes) As PrintJob
            ' Override to add printing support with new job/page control classes

            If Me IsNot Toolkit.defaultToolkit Then
                Return Toolkit.defaultToolkit.getPrintJob(frame_Renamed, jobtitle, jobAttributes, pageAttributes)
            Else
                Return getPrintJob(frame_Renamed, jobtitle, Nothing)
            End If
        End Function

        ''' <summary>
        ''' Emits an audio beep depending on native system settings and hardware
        ''' capabilities.
        ''' @since     JDK1.1
        ''' </summary>
        Public MustOverride Sub beep()

        ''' <summary>
        ''' Gets the singleton instance of the system Clipboard which interfaces
        ''' with clipboard facilities provided by the native platform. This
        ''' clipboard enables data transfer between Java programs and native
        ''' applications which use native clipboard facilities.
        ''' <p>
        ''' In addition to any and all formats specified in the flavormap.properties
        ''' file, or other file specified by the <code>AWT.DnD.flavorMapFileURL
        ''' </code> Toolkit property, text returned by the system Clipboard's <code>
        ''' getTransferData()</code> method is available in the following flavors:
        ''' <ul>
        ''' <li>DataFlavor.stringFlavor</li>
        ''' <li>DataFlavor.plainTextFlavor (<b>deprecated</b>)</li>
        ''' </ul>
        ''' As with <code>java.awt.datatransfer.StringSelection</code>, if the
        ''' requested flavor is <code>DataFlavor.plainTextFlavor</code>, or an
        ''' equivalent flavor, a Reader is returned. <b>Note:</b> The behavior of
        ''' the system Clipboard's <code>getTransferData()</code> method for <code>
        ''' DataFlavor.plainTextFlavor</code>, and equivalent DataFlavors, is
        ''' inconsistent with the definition of <code>DataFlavor.plainTextFlavor
        ''' </code>. Because of this, support for <code>
        ''' DataFlavor.plainTextFlavor</code>, and equivalent flavors, is
        ''' <b>deprecated</b>.
        ''' <p>
        ''' Each actual implementation of this method should first check if there
        ''' is a security manager installed. If there is, the method should call
        ''' the security manager's {@link SecurityManager#checkPermission
        ''' checkPermission} method to check {@code AWTPermission("accessClipboard")}.
        ''' </summary>
        ''' <returns>    the system Clipboard </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.datatransfer.Clipboard </seealso>
        ''' <seealso cref=       java.awt.datatransfer.StringSelection </seealso>
        ''' <seealso cref=       java.awt.datatransfer.DataFlavor#stringFlavor </seealso>
        ''' <seealso cref=       java.awt.datatransfer.DataFlavor#plainTextFlavor </seealso>
        ''' <seealso cref=       java.io.Reader </seealso>
        ''' <seealso cref=       java.awt.AWTPermission
        ''' @since     JDK1.1 </seealso>
        Public MustOverride ReadOnly Property systemClipboard As java.awt.datatransfer.Clipboard

        ''' <summary>
        ''' Gets the singleton instance of the system selection as a
        ''' <code>Clipboard</code> object. This allows an application to read and
        ''' modify the current, system-wide selection.
        ''' <p>
        ''' An application is responsible for updating the system selection whenever
        ''' the user selects text, using either the mouse or the keyboard.
        ''' Typically, this is implemented by installing a
        ''' <code>FocusListener</code> on all <code>Component</code>s which support
        ''' text selection, and, between <code>FOCUS_GAINED</code> and
        ''' <code>FOCUS_LOST</code> events delivered to that <code>Component</code>,
        ''' updating the system selection <code>Clipboard</code> when the selection
        ''' changes inside the <code>Component</code>. Properly updating the system
        ''' selection ensures that a Java application will interact correctly with
        ''' native applications and other Java applications running simultaneously
        ''' on the system. Note that <code>java.awt.TextComponent</code> and
        ''' <code>javax.swing.text.JTextComponent</code> already adhere to this
        ''' policy. When using these classes, and their subclasses, developers need
        ''' not write any additional code.
        ''' <p>
        ''' Some platforms do not support a system selection <code>Clipboard</code>.
        ''' On those platforms, this method will return <code>null</code>. In such a
        ''' case, an application is absolved from its responsibility to update the
        ''' system selection <code>Clipboard</code> as described above.
        ''' <p>
        ''' Each actual implementation of this method should first check if there
        ''' is a security manager installed. If there is, the method should call
        ''' the security manager's {@link SecurityManager#checkPermission
        ''' checkPermission} method to check {@code AWTPermission("accessClipboard")}.
        ''' </summary>
        ''' <returns> the system selection as a <code>Clipboard</code>, or
        '''         <code>null</code> if the native platform does not support a
        '''         system selection <code>Clipboard</code> </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        '''            returns true
        ''' </exception>
        ''' <seealso cref= java.awt.datatransfer.Clipboard </seealso>
        ''' <seealso cref= java.awt.event.FocusListener </seealso>
        ''' <seealso cref= java.awt.event.FocusEvent#FOCUS_GAINED </seealso>
        ''' <seealso cref= java.awt.event.FocusEvent#FOCUS_LOST </seealso>
        ''' <seealso cref= TextComponent </seealso>
        ''' <seealso cref= javax.swing.text.JTextComponent </seealso>
        ''' <seealso cref= AWTPermission </seealso>
        ''' <seealso cref= GraphicsEnvironment#isHeadless
        ''' @since 1.4 </seealso>
        Public Overridable Property systemSelection As java.awt.datatransfer.Clipboard
            Get
                GraphicsEnvironment.checkHeadless()

                If Me IsNot Toolkit.defaultToolkit Then
                    Return Toolkit.defaultToolkit.systemSelection
                Else
                    GraphicsEnvironment.checkHeadless()
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Determines which modifier key is the appropriate accelerator
        ''' key for menu shortcuts.
        ''' <p>
        ''' Menu shortcuts, which are embodied in the
        ''' <code>MenuShortcut</code> [Class], are handled by the
        ''' <code>MenuBar</code> class.
        ''' <p>
        ''' By default, this method returns <code>Event.CTRL_MASK</code>.
        ''' Toolkit implementations should override this method if the
        ''' <b>Control</b> key isn't the correct key for accelerators. </summary>
        ''' <returns>    the modifier mask on the <code>Event</code> class
        '''                 that is used for menu shortcuts on this toolkit. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.MenuBar </seealso>
        ''' <seealso cref=       java.awt.MenuShortcut
        ''' @since     JDK1.1 </seealso>
        Public Overridable Property menuShortcutKeyMask As Integer
            Get
                GraphicsEnvironment.checkHeadless()

                Return Event.CTRL_MASK
			End Get
        End Property

        ''' <summary>
        ''' Returns whether the given locking key on the keyboard is currently in
        ''' its "on" state.
        ''' Valid key codes are
        ''' <seealso cref="java.awt.event.KeyEvent#VK_CAPS_LOCK VK_CAPS_LOCK"/>,
        ''' <seealso cref="java.awt.event.KeyEvent#VK_NUM_LOCK VK_NUM_LOCK"/>,
        ''' <seealso cref="java.awt.event.KeyEvent#VK_SCROLL_LOCK VK_SCROLL_LOCK"/>, and
        ''' <seealso cref="java.awt.event.KeyEvent#VK_KANA_LOCK VK_KANA_LOCK"/>.
        ''' </summary>
        ''' <exception cref="java.lang.IllegalArgumentException"> if <code>keyCode</code>
        ''' is not one of the valid key codes </exception>
        ''' <exception cref="java.lang.UnsupportedOperationException"> if the host system doesn't
        ''' allow getting the state of this key programmatically, or if the keyboard
        ''' doesn't have this key </exception>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since 1.3 </seealso>
        Public Overridable Function getLockingKeyState(ByVal keyCode As Integer) As Boolean
            GraphicsEnvironment.checkHeadless()

            If Not (keyCode = KeyEvent.VK_CAPS_LOCK OrElse keyCode = KeyEvent.VK_NUM_LOCK OrElse keyCode = KeyEvent.VK_SCROLL_LOCK OrElse keyCode = KeyEvent.VK_KANA_LOCK) Then Throw New IllegalArgumentException("invalid key for Toolkit.getLockingKeyState")
            Throw New UnsupportedOperationException("Toolkit.getLockingKeyState")
        End Function

        ''' <summary>
        ''' Sets the state of the given locking key on the keyboard.
        ''' Valid key codes are
        ''' <seealso cref="java.awt.event.KeyEvent#VK_CAPS_LOCK VK_CAPS_LOCK"/>,
        ''' <seealso cref="java.awt.event.KeyEvent#VK_NUM_LOCK VK_NUM_LOCK"/>,
        ''' <seealso cref="java.awt.event.KeyEvent#VK_SCROLL_LOCK VK_SCROLL_LOCK"/>, and
        ''' <seealso cref="java.awt.event.KeyEvent#VK_KANA_LOCK VK_KANA_LOCK"/>.
        ''' <p>
        ''' Depending on the platform, setting the state of a locking key may
        ''' involve event processing and therefore may not be immediately
        ''' observable through getLockingKeyState.
        ''' </summary>
        ''' <exception cref="java.lang.IllegalArgumentException"> if <code>keyCode</code>
        ''' is not one of the valid key codes </exception>
        ''' <exception cref="java.lang.UnsupportedOperationException"> if the host system doesn't
        ''' allow setting the state of this key programmatically, or if the keyboard
        ''' doesn't have this key </exception>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since 1.3 </seealso>
        Public Overridable Sub setLockingKeyState(ByVal keyCode As Integer, ByVal [on] As Boolean)
            GraphicsEnvironment.checkHeadless()

            If Not (keyCode = KeyEvent.VK_CAPS_LOCK OrElse keyCode = KeyEvent.VK_NUM_LOCK OrElse keyCode = KeyEvent.VK_SCROLL_LOCK OrElse keyCode = KeyEvent.VK_KANA_LOCK) Then Throw New IllegalArgumentException("invalid key for Toolkit.setLockingKeyState")
            Throw New UnsupportedOperationException("Toolkit.setLockingKeyState")
        End Sub

        ''' <summary>
        ''' Give native peers the ability to query the native container
        ''' given a native component (eg the direct parent may be lightweight).
        ''' </summary>
        Protected Friend Shared Function getNativeContainer(ByVal c As Component) As Container
            Return c.nativeContainer
        End Function

        ''' <summary>
        ''' Creates a new custom cursor object.
        ''' If the image to display is invalid, the cursor will be hidden (made
        ''' completely transparent), and the hotspot will be set to (0, 0).
        ''' 
        ''' <p>Note that multi-frame images are invalid and may cause this
        ''' method to hang.
        ''' </summary>
        ''' <param name="cursor"> the image to display when the cursor is activated </param>
        ''' <param name="hotSpot"> the X and Y of the large cursor's hot spot; the
        '''   hotSpot values must be less than the Dimension returned by
        '''   <code>getBestCursorSize</code> </param>
        ''' <param name="name"> a localized description of the cursor, for Java Accessibility use </param>
        ''' <exception cref="IndexOutOfBoundsException"> if the hotSpot values are outside
        '''   the bounds of the cursor </exception>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.2 </seealso>
        Public Overridable Function createCustomCursor(ByVal cursor_Renamed As image, ByVal hotSpot As Point, ByVal name As String) As Cursor
            ' Override to implement custom cursor support.
            If Me IsNot Toolkit.defaultToolkit Then
                Return Toolkit.defaultToolkit.createCustomCursor(cursor_Renamed, hotSpot, name)
            Else
                Return New Cursor(Cursor.DEFAULT_CURSOR)
            End If
        End Function

        ''' <summary>
        ''' Returns the supported cursor dimension which is closest to the desired
        ''' sizes.  Systems which only support a single cursor size will return that
        ''' size regardless of the desired sizes.  Systems which don't support custom
        ''' cursors will return a dimension of 0, 0. <p>
        ''' Note:  if an image is used whose dimensions don't match a supported size
        ''' (as returned by this method), the Toolkit implementation will attempt to
        ''' resize the image to a supported size.
        ''' Since converting low-resolution images is difficult,
        ''' no guarantees are made as to the quality of a cursor image which isn't a
        ''' supported size.  It is therefore recommended that this method
        ''' be called and an appropriate image used so no image conversion is made.
        ''' </summary>
        ''' <param name="preferredWidth"> the preferred cursor width the component would like
        ''' to use. </param>
        ''' <param name="preferredHeight"> the preferred cursor height the component would like
        ''' to use. </param>
        ''' <returns>    the closest matching supported cursor size, or a dimension of 0,0 if
        ''' the Toolkit implementation doesn't support custom cursors. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.2 </seealso>
        Public Overridable Function getBestCursorSize(ByVal preferredWidth As Integer, ByVal preferredHeight As Integer) As Dimension
            GraphicsEnvironment.checkHeadless()

            ' Override to implement custom cursor support.
            If Me IsNot Toolkit.defaultToolkit Then
                Return Toolkit.defaultToolkit.getBestCursorSize(preferredWidth, preferredHeight)
            Else
                Return New Dimension(0, 0)
            End If
        End Function

        ''' <summary>
        ''' Returns the maximum number of colors the Toolkit supports in a custom cursor
        ''' palette.<p>
        ''' Note: if an image is used which has more colors in its palette than
        ''' the supported maximum, the Toolkit implementation will attempt to flatten the
        ''' palette to the maximum.  Since converting low-resolution images is difficult,
        ''' no guarantees are made as to the quality of a cursor image which has more
        ''' colors than the system supports.  It is therefore recommended that this method
        ''' be called and an appropriate image used so no image conversion is made.
        ''' </summary>
        ''' <returns>    the maximum number of colors, or zero if custom cursors are not
        ''' supported by this Toolkit implementation. </returns>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
        ''' returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.2 </seealso>
        Public Overridable Property maximumCursorColors As Integer
            Get
                GraphicsEnvironment.checkHeadless()

                ' Override to implement custom cursor support.
                If Me IsNot Toolkit.defaultToolkit Then
                    Return Toolkit.defaultToolkit.maximumCursorColors
                Else
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns whether Toolkit supports this state for
        ''' <code>Frame</code>s.  This method tells whether the <em>UI
        ''' concept</em> of, say, maximization or iconification is
        ''' supported.  It will always return false for "compound" states
        ''' like <code>Frame.ICONIFIED|Frame.MAXIMIZED_VERT</code>.
        ''' In other words, the rule of thumb is that only queries with a
        ''' single frame state constant as an argument are meaningful.
        ''' <p>Note that supporting a given concept is a platform-
        ''' dependent feature. Due to native limitations the Toolkit
        ''' object may report a particular state as supported, however at
        ''' the same time the Toolkit object will be unable to apply the
        ''' state to a given frame.  This circumstance has two following
        ''' consequences:
        ''' <ul>
        ''' <li>Only the return value of {@code false} for the present
        ''' method actually indicates that the given state is not
        ''' supported. If the method returns {@code true} the given state
        ''' may still be unsupported and/or unavailable for a particular
        ''' frame.
        ''' <li>The developer should consider examining the value of the
        ''' <seealso cref="java.awt.event.WindowEvent#getNewState"/> method of the
        ''' {@code WindowEvent} received through the {@link
        ''' java.awt.event.WindowStateListener}, rather than assuming
        ''' that the state given to the {@code setExtendedState()} method
        ''' will be definitely applied. For more information see the
        ''' documentation for the <seealso cref="Frame#setExtendedState"/> method.
        ''' </ul>
        ''' </summary>
        ''' <param name="state"> one of named frame state constants. </param>
        ''' <returns> <code>true</code> is this frame state is supported by
        '''     this Toolkit implementation, <code>false</code> otherwise. </returns>
        ''' <exception cref="HeadlessException">
        '''     if <code>GraphicsEnvironment.isHeadless()</code>
        '''     returns <code>true</code>. </exception>
        ''' <seealso cref= java.awt.Window#addWindowStateListener
        ''' @since   1.4 </seealso>
        Public Overridable Function isFrameStateSupported(ByVal state As Integer) As Boolean
            GraphicsEnvironment.checkHeadless()

            If Me IsNot Toolkit.defaultToolkit Then
                Return Toolkit.defaultToolkit.isFrameStateSupported(state)
            Else
                Return (state = Frame.NORMAL) ' others are not guaranteed
            End If
        End Function

        ''' <summary>
        ''' Support for I18N: any visible strings should be stored in
        ''' sun.awt.resources.awt.properties.  The ResourceBundle is stored
        ''' here, so that only one copy is maintained.
        ''' </summary>
        Private Shared resources As ResourceBundle
        Private Shared platformResources As ResourceBundle

        ' called by platform toolkit
        Private Shared Property platformResources As ResourceBundle
            Set(ByVal bundle As ResourceBundle)
                platformResources = bundle
            End Set
        End Property

        ''' <summary>
        ''' Initialize JNI field and method ids
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub initIDs()
        End Sub

        ''' <summary>
        ''' WARNING: This is a temporary workaround for a problem in the
        ''' way the AWT loads native libraries. A number of classes in the
        ''' AWT package have a native method, initIDs(), which initializes
        ''' the JNI field and method ids used in the native portion of
        ''' their implementation.
        ''' 
        ''' Since the use and storage of these ids is done by the
        ''' implementation libraries, the implementation of these method is
        ''' provided by the particular AWT implementations (for example,
        ''' "Toolkit"s/Peer), such as Motif, Microsoft Windows, or Tiny. The
        ''' problem is that this means that the native libraries must be
        ''' loaded by the java.* classes, which do not necessarily know the
        ''' names of the libraries to load. A better way of doing this
        ''' would be to provide a separate library which defines java.awt.*
        ''' initIDs, and exports the relevant symbols out to the
        ''' implementation libraries.
        ''' 
        ''' For now, we know it's done by the implementation, and we assume
        ''' that the name of the library is "awt".  -br.
        ''' 
        ''' If you change loadLibraries(), please add the change to
        ''' java.awt.image.ColorModel.loadLibraries(). Unfortunately,
        ''' classes can be loaded in java.awt.image that depend on
        ''' libawt and there is no way to call Toolkit.loadLibraries()
        ''' directly.  -hung
        ''' </summary>
        Private Shared loaded As Boolean = False
        Friend Shared Sub loadLibraries()
            If Not loaded Then
                java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T))
                loaded = True
            End If
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
                '				System.loadLibrary("awt")
                Return Nothing
            End Function
        End Class

        Shared Sub New()
            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			sun.awt.AWTAccessor.setToolkitAccessor(New sun.awt.AWTAccessor.ToolkitAccessor()
            '		{
            '					@Override public  Sub  setPlatformResources(ResourceBundle bundle)
            '					{
            '						Toolkit.setPlatformResources(bundle);
            '					}
            '				});

            java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper4(Of T))

            ' ensure that the proper libraries are loaded
            loadLibraries()
            initAssistiveTechnologies()
            If Not GraphicsEnvironment.headless Then initIDs()
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper4(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                Try
                    resources = ResourceBundle.getBundle("sun.awt.resources.awt", sun.util.CoreResourceBundleControl.rBControlInstance)
                Catch e As MissingResourceException
                    ' No resource file; defaults will be used.
                End Try
                Return Nothing
            End Function
        End Class

        ''' <summary>
        ''' Gets a property with the specified key and default.
        ''' This method returns defaultValue if the property is not found.
        ''' </summary>
        Public Shared Function getProperty(ByVal key As String, ByVal defaultValue As String) As String
            ' first try platform specific bundle
            If platformResources IsNot Nothing Then
                Try
                    Return platformResources.getString(key)
                Catch e As MissingResourceException
                End Try
            End If

            ' then shared one
            If resources IsNot Nothing Then
                Try
                    Return resources.getString(key)
                Catch e As MissingResourceException
                End Try
            End If

            Return defaultValue
        End Function

        ''' <summary>
        ''' Get the application's or applet's EventQueue instance.
        ''' Depending on the Toolkit implementation, different EventQueues
        ''' may be returned for different applets.  Applets should
        ''' therefore not assume that the EventQueue instance returned
        ''' by this method will be shared by other applets or the system.
        ''' 
        ''' <p> If there is a security manager then its
        ''' <seealso cref="SecurityManager#checkPermission checkPermission"/> method
        ''' is called to check {@code AWTPermission("accessEventQueue")}.
        ''' </summary>
        ''' <returns>    the <code>EventQueue</code> object </returns>
        ''' <exception cref="SecurityException">
        '''          if a security manager is set and it denies access to
        '''          the {@code EventQueue} </exception>
        ''' <seealso cref=     java.awt.AWTPermission </seealso>
        Public ReadOnly Property systemEventQueue As EventQueue
            Get
                Dim security As SecurityManager = System.securityManager
                If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.CHECK_AWT_EVENTQUEUE_PERMISSION)
                Return systemEventQueueImpl
            End Get
        End Property

        ''' <summary>
        ''' Gets the application's or applet's <code>EventQueue</code>
        ''' instance, without checking access.  For security reasons,
        ''' this can only be called from a <code>Toolkit</code> subclass. </summary>
        ''' <returns> the <code>EventQueue</code> object </returns>
        Protected Friend MustOverride ReadOnly Property systemEventQueueImpl As EventQueue

        ' Accessor method for use by AWT package routines. 
        Friend Shared ReadOnly Property eventQueue As EventQueue
            Get
                Return defaultToolkit.systemEventQueueImpl
            End Get
        End Property

        ''' <summary>
        ''' Creates the peer for a DragSourceContext.
        ''' Always throws InvalidDndOperationException if
        ''' GraphicsEnvironment.isHeadless() returns true. </summary>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
        Public MustOverride Function createDragSourceContextPeer(ByVal dge As java.awt.dnd.DragGestureEvent) As java.awt.dnd.peer.DragSourceContextPeer

        ''' <summary>
        ''' Creates a concrete, platform dependent, subclass of the abstract
        ''' DragGestureRecognizer class requested, and associates it with the
        ''' DragSource, Component and DragGestureListener specified.
        ''' 
        ''' subclasses should override this to provide their own implementation
        ''' </summary>
        ''' <param name="abstractRecognizerClass"> The abstract class of the required recognizer </param>
        ''' <param name="ds">                      The DragSource </param>
        ''' <param name="c">                       The Component target for the DragGestureRecognizer </param>
        ''' <param name="srcActions">              The actions permitted for the gesture </param>
        ''' <param name="dgl">                     The DragGestureListener
        ''' </param>
        ''' <returns> the new object or null.  Always returns null if
        ''' GraphicsEnvironment.isHeadless() returns true. </returns>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
        Public Overridable Function createDragGestureRecognizer(Of T As java.awt.dnd.DragGestureRecognizer)(ByVal abstractRecognizerClass As [Class], ByVal ds As java.awt.dnd.DragSource, ByVal c As Component, ByVal srcActions As Integer, ByVal dgl As java.awt.dnd.DragGestureListener) As T
            Return Nothing
        End Function

        ''' <summary>
        ''' Obtains a value for the specified desktop property.
        ''' 
        ''' A desktop property is a uniquely named value for a resource that
        ''' is Toolkit global in nature. Usually it also is an abstract
        ''' representation for an underlying platform dependent desktop setting.
        ''' For more information on desktop properties supported by the AWT see
        ''' <a href="doc-files/DesktopProperties.html">AWT Desktop Properties</a>.
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Function getDesktopProperty(ByVal propertyName As String) As Object
            ' This is a workaround for headless toolkits.  It would be
            ' better to override this method but it is declared final.
            ' "this instanceof" syntax defeats polymorphism.
            ' --mm, 03/03/00
            If TypeOf Me Is sun.awt.HeadlessToolkit Then Return CType(Me, sun.awt.HeadlessToolkit).underlyingToolkit.getDesktopProperty(propertyName)

            If desktopProperties.empty Then initializeDesktopProperties()

            Dim value As Object

            ' This property should never be cached
            If propertyName.Equals("awt.dynamicLayoutSupported") Then Return defaultToolkit.lazilyLoadDesktopProperty(propertyName)

            value = desktopProperties.get(propertyName)

            If value Is Nothing Then
                value = lazilyLoadDesktopProperty(propertyName)

                If value IsNot Nothing Then desktopPropertyrty(propertyName, value)
            End If

            ' for property "awt.font.desktophints" 
            If TypeOf value Is RenderingHints Then value = CType(value, RenderingHints).clone()

            Return value
        End Function

        ''' <summary>
        ''' Sets the named desktop property to the specified value and fires a
        ''' property change event to notify any listeners that the value has changed.
        ''' </summary>
        Protected Friend Sub setDesktopProperty(ByVal name As String, ByVal newValue As Object)
            ' This is a workaround for headless toolkits.  It would be
            ' better to override this method but it is declared final.
            ' "this instanceof" syntax defeats polymorphism.
            ' --mm, 03/03/00
            If TypeOf Me Is sun.awt.HeadlessToolkit Then
                CType(Me, sun.awt.HeadlessToolkit).underlyingToolkit.desktopPropertyrty(name, newValue)
                Return
            End If
            Dim oldValue As Object

            SyncLock Me
                oldValue = desktopProperties.get(name)
                desktopProperties.put(name, newValue)
            End SyncLock

            ' Don't fire change event if old and new values are null.
            ' It helps to avoid recursive resending of WM_THEMECHANGED
            If oldValue IsNot Nothing OrElse newValue IsNot Nothing Then desktopPropsSupport.firePropertyChange(name, oldValue, newValue)
        End Sub

        ''' <summary>
        ''' an opportunity to lazily evaluate desktop property values.
        ''' </summary>
        Protected Friend Overridable Function lazilyLoadDesktopProperty(ByVal name As String) As Object
            Return Nothing
        End Function

        ''' <summary>
        ''' initializeDesktopProperties
        ''' </summary>
        Protected Friend Overridable Sub initializeDesktopProperties()
        End Sub

        ''' <summary>
        ''' Adds the specified property change listener for the named desktop
        ''' property. When a <seealso cref="java.beans.PropertyChangeListenerProxy"/> object is added,
        ''' its property name is ignored, and the wrapped listener is added.
        ''' If {@code name} is {@code null} or {@code pcl} is {@code null},
        ''' no exception is thrown and no action is performed.
        ''' </summary>
        ''' <param name="name"> The name of the property to listen for </param>
        ''' <param name="pcl"> The property change listener </param>
        ''' <seealso cref= PropertyChangeSupport#addPropertyChangeListener(String,
        '''            PropertyChangeListener)
        ''' @since   1.2 </seealso>
        Public Overridable Sub addPropertyChangeListener(ByVal name As String, ByVal pcl As java.beans.PropertyChangeListener)
            desktopPropsSupport.addPropertyChangeListener(name, pcl)
        End Sub

        ''' <summary>
        ''' Removes the specified property change listener for the named
        ''' desktop property. When a <seealso cref="java.beans.PropertyChangeListenerProxy"/> object
        ''' is removed, its property name is ignored, and
        ''' the wrapped listener is removed.
        ''' If {@code name} is {@code null} or {@code pcl} is {@code null},
        ''' no exception is thrown and no action is performed.
        ''' </summary>
        ''' <param name="name"> The name of the property to remove </param>
        ''' <param name="pcl"> The property change listener </param>
        ''' <seealso cref= PropertyChangeSupport#removePropertyChangeListener(String,
        '''            PropertyChangeListener)
        ''' @since   1.2 </seealso>
        Public Overridable Sub removePropertyChangeListener(ByVal name As String, ByVal pcl As java.beans.PropertyChangeListener)
            desktopPropsSupport.removePropertyChangeListener(name, pcl)
        End Sub

        ''' <summary>
        ''' Returns an array of all the property change listeners
        ''' registered on this toolkit. The returned array
        ''' contains <seealso cref="java.beans.PropertyChangeListenerProxy"/> objects
        ''' that associate listeners with the names of desktop properties.
        ''' </summary>
        ''' <returns> all of this toolkit's <seealso cref="PropertyChangeListener"/>
        '''         objects wrapped in {@code java.beans.PropertyChangeListenerProxy} objects
        '''         or an empty array  if no listeners are added
        ''' </returns>
        ''' <seealso cref= PropertyChangeSupport#getPropertyChangeListeners()
        ''' @since 1.4 </seealso>
        Public Overridable ReadOnly Property propertyChangeListeners As java.beans.PropertyChangeListener()
            Get
                Return desktopPropsSupport.propertyChangeListeners
            End Get
        End Property

        ''' <summary>
        ''' Returns an array of all property change listeners
        ''' associated with the specified name of a desktop property.
        ''' </summary>
        ''' <param name="propertyName"> the named property </param>
        ''' <returns> all of the {@code PropertyChangeListener} objects
        '''         associated with the specified name of a desktop property
        '''         or an empty array if no such listeners are added
        ''' </returns>
        ''' <seealso cref= PropertyChangeSupport#getPropertyChangeListeners(String)
        ''' @since 1.4 </seealso>
        Public Overridable Function getPropertyChangeListeners(ByVal propertyName As String) As java.beans.PropertyChangeListener()
            Return desktopPropsSupport.getPropertyChangeListeners(propertyName)
        End Function

        Protected Friend ReadOnly desktopProperties As Map(Of String, Object) = New HashMap(Of String, Object)
        Protected Friend ReadOnly desktopPropsSupport As java.beans.PropertyChangeSupport = Toolkit.createPropertyChangeSupport(Me)

        ''' <summary>
        ''' Returns whether the always-on-top mode is supported by this toolkit.
        ''' To detect whether the always-on-top mode is supported for a
        ''' particular Window, use <seealso cref="Window#isAlwaysOnTopSupported"/>. </summary>
        ''' <returns> <code>true</code>, if current toolkit supports the always-on-top mode,
        '''     otherwise returns <code>false</code> </returns>
        ''' <seealso cref= Window#isAlwaysOnTopSupported </seealso>
        ''' <seealso cref= Window#setAlwaysOnTop(boolean)
        ''' @since 1.6 </seealso>
        Public Overridable ReadOnly Property alwaysOnTopSupported As Boolean
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Returns whether the given modality type is supported by this toolkit. If
        ''' a dialog with unsupported modality type is created, then
        ''' <code>Dialog.ModalityType.MODELESS</code> is used instead.
        ''' </summary>
        ''' <param name="modalityType"> modality type to be checked for support by this toolkit
        ''' </param>
        ''' <returns> <code>true</code>, if current toolkit supports given modality
        '''     type, <code>false</code> otherwise
        ''' </returns>
        ''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
        ''' <seealso cref= java.awt.Dialog#getModalityType </seealso>
        ''' <seealso cref= java.awt.Dialog#setModalityType
        ''' 
        ''' @since 1.6 </seealso>
        Public MustOverride Function isModalityTypeSupported(ByVal modalityType As Dialog.ModalityType) As Boolean

        ''' <summary>
        ''' Returns whether the given modal exclusion type is supported by this
        ''' toolkit. If an unsupported modal exclusion type property is set on a window,
        ''' then <code>Dialog.ModalExclusionType.NO_EXCLUDE</code> is used instead.
        ''' </summary>
        ''' <param name="modalExclusionType"> modal exclusion type to be checked for support by this toolkit
        ''' </param>
        ''' <returns> <code>true</code>, if current toolkit supports given modal exclusion
        '''     type, <code>false</code> otherwise
        ''' </returns>
        ''' <seealso cref= java.awt.Dialog.ModalExclusionType </seealso>
        ''' <seealso cref= java.awt.Window#getModalExclusionType </seealso>
        ''' <seealso cref= java.awt.Window#setModalExclusionType
        ''' 
        ''' @since 1.6 </seealso>
        Public MustOverride Function isModalExclusionTypeSupported(ByVal modalExclusionType As Dialog.ModalExclusionType) As Boolean

        ' 8014718: logging has been removed from SunToolkit

        Private Const LONG_BITS As Integer = 64
        Private calls As Integer() = New Integer(LONG_BITS - 1) {}
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        Private Shared enabledOnToolkitMask As Long
        Private eventListener As AWTEventListener = Nothing
        Private listener2SelectiveListener As New WeakHashMap(Of AWTEventListener, SelectiveAWTEventListener)

        '    
        '     * Extracts a "pure" AWTEventListener from a AWTEventListenerProxy,
        '     * if the listener is proxied.
        '     
        Private Shared Function deProxyAWTEventListener(ByVal l As AWTEventListener) As AWTEventListener
            Dim localL As AWTEventListener = l

            If localL Is Nothing Then Return Nothing
            ' if user passed in a AWTEventListenerProxy object, extract
            ' the listener
            If TypeOf l Is AWTEventListenerProxy Then localL = CType(l, AWTEventListenerProxy).listener
            Return localL
        End Function

        ''' <summary>
        ''' Adds an AWTEventListener to receive all AWTEvents dispatched
        ''' system-wide that conform to the given <code>eventMask</code>.
        ''' <p>
        ''' First, if there is a security manager, its <code>checkPermission</code>
        ''' method is called with an
        ''' <code>AWTPermission("listenToAllAWTEvents")</code> permission.
        ''' This may result in a SecurityException.
        ''' <p>
        ''' <code>eventMask</code> is a bitmask of event types to receive.
        ''' It is constructed by bitwise OR-ing together the event masks
        ''' defined in <code>AWTEvent</code>.
        ''' <p>
        ''' Note:  event listener use is not recommended for normal
        ''' application use, but are intended solely to support special
        ''' purpose facilities including support for accessibility,
        ''' event record/playback, and diagnostic tracing.
        ''' 
        ''' If listener is null, no exception is thrown and no action is performed.
        ''' </summary>
        ''' <param name="listener">   the event listener. </param>
        ''' <param name="eventMask">  the bitmask of event types to receive </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow the operation. </exception>
        ''' <seealso cref=      #removeAWTEventListener </seealso>
        ''' <seealso cref=      #getAWTEventListeners </seealso>
        ''' <seealso cref=      SecurityManager#checkPermission </seealso>
        ''' <seealso cref=      java.awt.AWTEvent </seealso>
        ''' <seealso cref=      java.awt.AWTPermission </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListener </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListenerProxy
        ''' @since    1.2 </seealso>
        Public Overridable Sub addAWTEventListener(ByVal listener As AWTEventListener, ByVal eventMask As Long)
            Dim localL As AWTEventListener = deProxyAWTEventListener(listener)

            If localL Is Nothing Then Return
            Dim security As SecurityManager = System.securityManager
            If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION)
            SyncLock Me
                Dim selectiveListener As SelectiveAWTEventListener = listener2SelectiveListener.get(localL)

                If selectiveListener Is Nothing Then
                    ' Create a new selectiveListener.
                    selectiveListener = New SelectiveAWTEventListener(Me, localL, eventMask)
                    listener2SelectiveListener.put(localL, selectiveListener)
                    eventListener = ToolkitEventMulticaster.add(eventListener, selectiveListener)
                End If
                ' OR the eventMask into the selectiveListener's event mask.
                selectiveListener.orEventMasks(eventMask)

                enabledOnToolkitMask = enabledOnToolkitMask Or eventMask

                Dim mask As Long = eventMask
                For i As Integer = 0 To LONG_BITS - 1
                    ' If no bits are set, break out of loop.
                    If mask = 0 Then Exit For
                    If (mask And 1L) <> 0 Then ' Always test bit 0. calls(i) += 1
                        mask >>>= 1 ' Right shift, fill with zeros on left.
                Next i
            End SyncLock
        End Sub

        ''' <summary>
        ''' Removes an AWTEventListener from receiving dispatched AWTEvents.
        ''' <p>
        ''' First, if there is a security manager, its <code>checkPermission</code>
        ''' method is called with an
        ''' <code>AWTPermission("listenToAllAWTEvents")</code> permission.
        ''' This may result in a SecurityException.
        ''' <p>
        ''' Note:  event listener use is not recommended for normal
        ''' application use, but are intended solely to support special
        ''' purpose facilities including support for accessibility,
        ''' event record/playback, and diagnostic tracing.
        ''' 
        ''' If listener is null, no exception is thrown and no action is performed.
        ''' </summary>
        ''' <param name="listener">   the event listener. </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow the operation. </exception>
        ''' <seealso cref=      #addAWTEventListener </seealso>
        ''' <seealso cref=      #getAWTEventListeners </seealso>
        ''' <seealso cref=      SecurityManager#checkPermission </seealso>
        ''' <seealso cref=      java.awt.AWTEvent </seealso>
        ''' <seealso cref=      java.awt.AWTPermission </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListener </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListenerProxy
        ''' @since    1.2 </seealso>
        Public Overridable Sub removeAWTEventListener(ByVal listener As AWTEventListener)
            Dim localL As AWTEventListener = deProxyAWTEventListener(listener)

            If listener Is Nothing Then Return
            Dim security As SecurityManager = System.securityManager
            If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION)

            SyncLock Me
                Dim selectiveListener As SelectiveAWTEventListener = listener2SelectiveListener.get(localL)

                If selectiveListener IsNot Nothing Then
                    listener2SelectiveListener.remove(localL)
                    Dim listenerCalls As Integer() = selectiveListener.calls
                    For i As Integer = 0 To LONG_BITS - 1
                        calls(i) -= listenerCalls(i)
                        Debug.Assert(calls(i) >= 0, "Negative Listeners count")

                        If calls(i) = 0 Then enabledOnToolkitMask = enabledOnToolkitMask And Not (1L << i)
                    Next i
                End If
                eventListener = ToolkitEventMulticaster.remove(eventListener, If(selectiveListener Is Nothing, localL, selectiveListener))
            End SyncLock
        End Sub

        Friend Shared Function enabledOnToolkit(ByVal eventMask As Long) As Boolean
            Return (enabledOnToolkitMask And eventMask) <> 0
        End Function

        <MethodImpl(MethodImplOptions.Synchronized)>
        Friend Overridable Function countAWTEventListeners(ByVal eventMask As Long) As Integer
            Dim ci As Integer = 0
            Do While eventMask <> 0
                eventMask >>>= 1
                ci += 1
            Loop
            ci -= 1
            Return calls(ci)
        End Function
        ''' <summary>
        ''' Returns an array of all the <code>AWTEventListener</code>s
        ''' registered on this toolkit.
        ''' If there is a security manager, its {@code checkPermission}
        ''' method is called with an
        ''' {@code AWTPermission("listenToAllAWTEvents")} permission.
        ''' This may result in a SecurityException.
        ''' Listeners can be returned
        ''' within <code>AWTEventListenerProxy</code> objects, which also contain
        ''' the event mask for the given listener.
        ''' Note that listener objects
        ''' added multiple times appear only once in the returned array.
        ''' </summary>
        ''' <returns> all of the <code>AWTEventListener</code>s or an empty
        '''         array if no listeners are currently registered </returns>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow the operation. </exception>
        ''' <seealso cref=      #addAWTEventListener </seealso>
        ''' <seealso cref=      #removeAWTEventListener </seealso>
        ''' <seealso cref=      SecurityManager#checkPermission </seealso>
        ''' <seealso cref=      java.awt.AWTEvent </seealso>
        ''' <seealso cref=      java.awt.AWTPermission </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListener </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListenerProxy
        ''' @since 1.4 </seealso>
        Public Overridable ReadOnly Property aWTEventListeners As AWTEventListener()
            Get
                Dim security As SecurityManager = System.securityManager
                If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION)
                SyncLock Me
                    Dim la As EventListener() = ToolkitEventMulticaster.getListeners(eventListener, GetType(AWTEventListener))

                    Dim ret As AWTEventListener() = New AWTEventListener(la.Length - 1) {}
                    For i As Integer = 0 To la.Length - 1
                        Dim sael As SelectiveAWTEventListener = CType(la(i), SelectiveAWTEventListener)
                        Dim tempL As AWTEventListener = sael.listener
                        'assert tempL is not an AWTEventListenerProxy - we should
                        ' have weeded them all out
                        ' don't want to wrap a proxy inside a proxy
                        ret(i) = New AWTEventListenerProxy(sael.eventMask, tempL)
                    Next i
                    Return ret
                End SyncLock
            End Get
        End Property

        ''' <summary>
        ''' Returns an array of all the <code>AWTEventListener</code>s
        ''' registered on this toolkit which listen to all of the event
        ''' types specified in the {@code eventMask} argument.
        ''' If there is a security manager, its {@code checkPermission}
        ''' method is called with an
        ''' {@code AWTPermission("listenToAllAWTEvents")} permission.
        ''' This may result in a SecurityException.
        ''' Listeners can be returned
        ''' within <code>AWTEventListenerProxy</code> objects, which also contain
        ''' the event mask for the given listener.
        ''' Note that listener objects
        ''' added multiple times appear only once in the returned array.
        ''' </summary>
        ''' <param name="eventMask"> the bitmask of event types to listen for </param>
        ''' <returns> all of the <code>AWTEventListener</code>s registered
        '''         on this toolkit for the specified
        '''         event types, or an empty array if no such listeners
        '''         are currently registered </returns>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow the operation. </exception>
        ''' <seealso cref=      #addAWTEventListener </seealso>
        ''' <seealso cref=      #removeAWTEventListener </seealso>
        ''' <seealso cref=      SecurityManager#checkPermission </seealso>
        ''' <seealso cref=      java.awt.AWTEvent </seealso>
        ''' <seealso cref=      java.awt.AWTPermission </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListener </seealso>
        ''' <seealso cref=      java.awt.event.AWTEventListenerProxy
        ''' @since 1.4 </seealso>
        Public Overridable Function getAWTEventListeners(ByVal eventMask As Long) As AWTEventListener()
            Dim security As SecurityManager = System.securityManager
            If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.ALL_AWT_EVENTS_PERMISSION)
            SyncLock Me
                Dim la As EventListener() = ToolkitEventMulticaster.getListeners(eventListener, GetType(AWTEventListener))

                Dim list_Renamed As IList(Of AWTEventListenerProxy) = New List(Of AWTEventListenerProxy)(la.Length)

                For i As Integer = 0 To la.Length - 1
                    Dim sael As SelectiveAWTEventListener = CType(la(i), SelectiveAWTEventListener)
                    If (sael.eventMask And eventMask) = eventMask Then list_Renamed.Add(New AWTEventListenerProxy(sael.eventMask, sael.listener))
                Next i
                Return list_Renamed.ToArray()
            End SyncLock
        End Function

        '    
        '     * This method notifies any AWTEventListeners that an event
        '     * is about to be dispatched.
        '     *
        '     * @param theEvent the event which will be dispatched.
        '     
        Friend Overridable Sub notifyAWTEventListeners(ByVal theEvent As AWTEvent)
            ' This is a workaround for headless toolkits.  It would be
            ' better to override this method but it is declared package private.
            ' "this instanceof" syntax defeats polymorphism.
            ' --mm, 03/03/00
            If TypeOf Me Is sun.awt.HeadlessToolkit Then
                CType(Me, sun.awt.HeadlessToolkit).underlyingToolkit.notifyAWTEventListeners(theEvent)
                Return
            End If

            Dim eventListener As AWTEventListener = Me.eventListener
            If eventListener IsNot Nothing Then eventListener.eventDispatched(theEvent)
        End Sub

        Private Class ToolkitEventMulticaster
            Inherits AWTEventMulticaster
            Implements AWTEventListener

            ' Implementation cloned from AWTEventMulticaster.

            Friend Sub New(ByVal a As AWTEventListener, ByVal b As AWTEventListener)
                MyBase.New(a, b)
            End Sub

            Friend Shared Function add(ByVal a As AWTEventListener, ByVal b As AWTEventListener) As AWTEventListener
                If a Is Nothing Then Return b
                If b Is Nothing Then Return a
                Return New ToolkitEventMulticaster(a, b)
            End Function

            Friend Shared Function remove(ByVal l As AWTEventListener, ByVal oldl As AWTEventListener) As AWTEventListener
                Return CType(removeInternal(l, oldl), AWTEventListener)
            End Function

            ' #4178589: must overload remove(EventListener) to call our add()
            ' instead of the static addInternal() so we allocate a
            ' ToolkitEventMulticaster instead of an AWTEventMulticaster.
            ' Note: this method is called by AWTEventListener.removeInternal(),
            ' so its method signature must match AWTEventListener.remove().
            Protected Friend Overridable Function remove(ByVal oldl As EventListener) As EventListener
                If oldl Is a Then Return b
                If oldl Is b Then Return a
                Dim a2 As AWTEventListener = CType(removeInternal(a, oldl), AWTEventListener)
                Dim b2 As AWTEventListener = CType(removeInternal(b, oldl), AWTEventListener)
                If a2 Is a AndAlso b2 Is b Then Return Me ' it's not here
                Return add(a2, b2)
            End Function

            Public Overridable Sub eventDispatched(ByVal [event] As AWTEvent)
                CType(a, AWTEventListener).eventDispatched(event_Renamed)
                CType(b, AWTEventListener).eventDispatched(event_Renamed)
            End Sub
        End Class

        Private Class SelectiveAWTEventListener
            Implements AWTEventListener

            Private ReadOnly outerInstance As Toolkit

            Friend listener As AWTEventListener
            Private eventMask As Long
            ' This array contains the number of times to call the eventlistener
            ' for each event type.
            Friend calls As Integer() = New Integer(Toolkit.LONG_BITS - 1) {}

            Public Overridable ReadOnly Property listener As AWTEventListener
                Get
                    Return listener
                End Get
            End Property
            Public Overridable ReadOnly Property eventMask As Long
                Get
                    Return eventMask
                End Get
            End Property
            Public Overridable ReadOnly Property calls As Integer()
                Get
                    Return calls
                End Get
            End Property

            Public Overridable Sub orEventMasks(ByVal mask As Long)
                eventMask = eventMask Or mask
                ' For each event bit set in mask, increment its call count.
                For i As Integer = 0 To Toolkit.LONG_BITS - 1
                    ' If no bits are set, break out of loop.
                    If mask = 0 Then Exit For
                    If (mask And 1L) <> 0 Then ' Always test bit 0. calls(i) += 1
                        mask >>>= 1 ' Right shift, fill with zeros on left.
                Next i
            End Sub

            Friend Sub New(ByVal outerInstance As Toolkit, ByVal l As AWTEventListener, ByVal mask As Long)
                Me.outerInstance = outerInstance
                listener = l
                eventMask = mask
            End Sub

            Public Overridable Sub eventDispatched(ByVal [event] As AWTEvent)
                Dim eventBit As Long = 0 ' Used to save the bit of the event type.
                eventBit = eventMask And AWTEvent.COMPONENT_EVENT_MASK
                eventBit = eventMask And AWTEvent.CONTAINER_EVENT_MASK
                eventBit = eventMask And AWTEvent.FOCUS_EVENT_MASK
                eventBit = eventMask And AWTEvent.KEY_EVENT_MASK
                eventBit = eventMask And AWTEvent.MOUSE_WHEEL_EVENT_MASK
                eventBit = eventMask And AWTEvent.MOUSE_MOTION_EVENT_MASK
                eventBit = eventMask And AWTEvent.MOUSE_EVENT_MASK
                eventBit = eventMask And AWTEvent.WINDOW_EVENT_MASK
                eventBit = eventMask And AWTEvent.ACTION_EVENT_MASK
                eventBit = eventMask And AWTEvent.ADJUSTMENT_EVENT_MASK
                eventBit = eventMask And AWTEvent.ITEM_EVENT_MASK
                eventBit = eventMask And AWTEvent.TEXT_EVENT_MASK
                eventBit = eventMask And AWTEvent.INPUT_METHOD_EVENT_MASK
                eventBit = eventMask And AWTEvent.PAINT_EVENT_MASK
                eventBit = eventMask And AWTEvent.INVOCATION_EVENT_MASK
                eventBit = eventMask And AWTEvent.HIERARCHY_EVENT_MASK
                eventBit = eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK
                eventBit = eventMask And AWTEvent.WINDOW_STATE_EVENT_MASK
                eventBit = eventMask And AWTEvent.WINDOW_FOCUS_EVENT_MASK
                eventBit = eventMask And sun.awt.SunToolkit.GRAB_EVENT_MASK
                If (eventBit <> 0 AndAlso event_Renamed.id >= ComponentEvent.COMPONENT_FIRST AndAlso event_Renamed.id <= ComponentEvent.COMPONENT_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= ContainerEvent.CONTAINER_FIRST AndAlso event_Renamed.id <= ContainerEvent.CONTAINER_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= FocusEvent.FOCUS_FIRST AndAlso event_Renamed.id <= FocusEvent.FOCUS_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= KeyEvent.KEY_FIRST AndAlso event_Renamed.id <= KeyEvent.KEY_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id = MouseEvent.MOUSE_WHEEL) OrElse (eventBit <> 0 AndAlso (event_Renamed.id = MouseEvent.MOUSE_MOVED OrElse event_Renamed.id = MouseEvent.MOUSE_DRAGGED)) OrElse (eventBit <> 0 AndAlso event_Renamed.id <> MouseEvent.MOUSE_MOVED AndAlso event_Renamed.id <> MouseEvent.MOUSE_DRAGGED AndAlso event_Renamed.id <> MouseEvent.MOUSE_WHEEL AndAlso event_Renamed.id >= MouseEvent.MOUSE_FIRST AndAlso event_Renamed.id <= MouseEvent.MOUSE_LAST) OrElse (eventBit <> 0 AndAlso (event_Renamed.id >= WindowEvent.WINDOW_FIRST AndAlso event_Renamed.id <= WindowEvent.WINDOW_LAST)) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= ActionEvent.ACTION_FIRST AndAlso event_Renamed.id <= ActionEvent.ACTION_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= AdjustmentEvent.ADJUSTMENT_FIRST AndAlso event_Renamed.id <= AdjustmentEvent.ADJUSTMENT_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= ItemEvent.ITEM_FIRST AndAlso event_Renamed.id <= ItemEvent.ITEM_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= TextEvent.TEXT_FIRST AndAlso event_Renamed.id <= TextEvent.TEXT_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= InputMethodEvent.INPUT_METHOD_FIRST AndAlso event_Renamed.id <= InputMethodEvent.INPUT_METHOD_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= PaintEvent.PAINT_FIRST AndAlso event_Renamed.id <= PaintEvent.PAINT_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id >= InvocationEvent.INVOCATION_FIRST AndAlso event_Renamed.id <= InvocationEvent.INVOCATION_LAST) OrElse (eventBit <> 0 AndAlso event_Renamed.id = HierarchyEvent.HIERARCHY_CHANGED) OrElse (eventBit <> 0 AndAlso (event_Renamed.id = HierarchyEvent.ANCESTOR_MOVED OrElse event_Renamed.id = HierarchyEvent.ANCESTOR_RESIZED)) OrElse (eventBit <> 0 AndAlso event_Renamed.id = WindowEvent.WINDOW_STATE_CHANGED) OrElse (eventBit <> 0 AndAlso (event_Renamed.id = WindowEvent.WINDOW_GAINED_FOCUS OrElse event_Renamed.id = WindowEvent.WINDOW_LOST_FOCUS)) OrElse (eventBit <> 0 AndAlso (TypeOf event_Renamed Is sun.awt.UngrabEvent)) Then
                    ' Get the index of the call count for this event type.
                    ' Instead of using System.Math.log(...) we will calculate it with
                    ' bit shifts. That's what previous implementation looked like:
                    '
                    ' int ci = (int)  (System.Math.log(eventBit)/Math.log(2));
                    Dim ci As Integer = 0
                    Dim eMask As Long = eventBit
                    Do While eMask <> 0
                        eMask >>>= 1
                        ci += 1
                    Loop
                    ci -= 1
                    ' Call the listener as many times as it was added for this
                    ' event type.
                    For i As Integer = 0 To calls(ci) - 1
                        listener.eventDispatched(event_Renamed)
                    Next i
                End If
            End Sub
        End Class

        ''' <summary>
        ''' Returns a map of visual attributes for the abstract level description
        ''' of the given input method highlight, or null if no mapping is found.
        ''' The style field of the input method highlight is ignored. The map
        ''' returned is unmodifiable. </summary>
        ''' <param name="highlight"> input method highlight </param>
        ''' <returns> style attribute map, or <code>null</code> </returns>
        ''' <exception cref="HeadlessException"> if
        '''     <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless
        ''' @since 1.3 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public MustOverride Function mapInputMethodHighlight(ByVal highlight As java.awt.im.InputMethodHighlight) As Map(Of java.awt.font.TextAttribute, ?)

        Private Shared Function createPropertyChangeSupport(ByVal toolkit_Renamed As Toolkit) As java.beans.PropertyChangeSupport
            If TypeOf toolkit_Renamed Is sun.awt.SunToolkit OrElse TypeOf toolkit_Renamed Is sun.awt.HeadlessToolkit Then
                Return New DesktopPropertyChangeSupport(toolkit_Renamed)
            Else
                Return New java.beans.PropertyChangeSupport(toolkit_Renamed)
            End If
        End Function

        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Private Class DesktopPropertyChangeSupport
            Inherits java.beans.PropertyChangeSupport

            Private Shared ReadOnly PROP_CHANGE_SUPPORT_KEY As New StringBuilder("desktop property change support key")
            Private ReadOnly source As Object

            Public Sub New(ByVal sourceBean As Object)
                MyBase.New(sourceBean)
                source = sourceBean
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Overrides Sub addPropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
                Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                If Nothing Is pcs Then
                    pcs = New java.beans.PropertyChangeSupport(source)
                    sun.awt.AppContext.appContext.put(PROP_CHANGE_SUPPORT_KEY, pcs)
                End If
                pcs.addPropertyChangeListener(propertyName, listener)
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Overrides Sub removePropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
                Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                If Nothing IsNot pcs Then pcs.removePropertyChangeListener(propertyName, listener)
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Property Overrides propertyChangeListeners As java.beans.PropertyChangeListener()
                Get
                    Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                    If Nothing IsNot pcs Then
                        Return pcs.propertyChangeListeners
                    Else
                        Return New java.beans.PropertyChangeListener() {}
                    End If
                End Get
            End Property

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Overrides Function getPropertyChangeListeners(ByVal propertyName As String) As java.beans.PropertyChangeListener()
                Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                If Nothing IsNot pcs Then
                    Return pcs.getPropertyChangeListeners(propertyName)
                Else
                    Return New java.beans.PropertyChangeListener() {}
                End If
            End Function

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Overrides Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
                Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                If Nothing Is pcs Then
                    pcs = New java.beans.PropertyChangeSupport(source)
                    sun.awt.AppContext.appContext.put(PROP_CHANGE_SUPPORT_KEY, pcs)
                End If
                pcs.addPropertyChangeListener(listener)
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Overrides Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
                Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                If Nothing IsNot pcs Then pcs.removePropertyChangeListener(listener)
            End Sub

            '        
            '         * we do expect that all other fireXXX() methods of java.beans.PropertyChangeSupport
            '         * use this method.  If this will be changed we will need to change this class.
            '         
            Public Overrides Sub firePropertyChange(ByVal evt As java.beans.PropertyChangeEvent)
                Dim oldValue As Object = evt.oldValue
                Dim newValue As Object = evt.newValue
                Dim propertyName As String = evt.propertyName
                If oldValue IsNot Nothing AndAlso newValue IsNot Nothing AndAlso oldValue.Equals(newValue) Then Return
                Dim updater As Runnable = New RunnableAnonymousInnerClassHelper
                Dim currentAppContext As sun.awt.AppContext = sun.awt.AppContext.appContext
                For Each appContext As sun.awt.AppContext In sun.awt.AppContext.appContexts
                    If Nothing Is appContext OrElse appContext.disposed Then Continue For
                    If currentAppContext Is appContext Then
                        updater.run()
                    Else
                        Dim e As New sun.awt.PeerEvent(source, updater, sun.awt.PeerEvent.ULTIMATE_PRIORITY_EVENT)
                        sun.awt.SunToolkit.postEvent(appContext, e)
                    End If
                Next appContext
            End Sub

            Private Class RunnableAnonymousInnerClassHelper
                Implements Runnable

                Public Overridable Sub run() Implements Runnable.run
                    Dim pcs As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(PROP_CHANGE_SUPPORT_KEY), java.beans.PropertyChangeSupport)
                    If Nothing IsNot pcs Then pcs.firePropertyChange(evt)
                End Sub
            End Class
        End Class

        ''' <summary>
        ''' Reports whether events from extra mouse buttons are allowed to be processed and posted into
        ''' {@code EventQueue}.
        ''' <br>
        ''' To change the returned value it is necessary to set the {@code sun.awt.enableExtraMouseButtons}
        ''' property before the {@code Toolkit} class initialization. This setting could be done on the application
        ''' startup by the following command:
        ''' <pre>
        ''' java -Dsun.awt.enableExtraMouseButtons=false Application
        ''' </pre>
        ''' Alternatively, the property could be set in the application by using the following code:
        ''' <pre>
        ''' System.setProperty("sun.awt.enableExtraMouseButtons", "true");
        ''' </pre>
        ''' before the {@code Toolkit} class initialization.
        ''' If not set by the time of the {@code Toolkit} class initialization, this property will be
        ''' initialized with {@code true}.
        ''' Changing this value after the {@code Toolkit} class initialization will have no effect.
        ''' <p> </summary>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
        ''' <returns> {@code true} if events from extra mouse buttons are allowed to be processed and posted;
        '''         {@code false} otherwise </returns>
        ''' <seealso cref= System#getProperty(String propertyName) </seealso>
        ''' <seealso cref= System#setProperty(String propertyName, String value) </seealso>
        ''' <seealso cref= java.awt.EventQueue
        ''' @since 1.7 </seealso>
        Public Overridable Function areExtraMouseButtonsEnabled() As Boolean
            GraphicsEnvironment.checkHeadless()

            Return Toolkit.defaultToolkit.areExtraMouseButtonsEnabled()
        End Function
    End Class

End Namespace
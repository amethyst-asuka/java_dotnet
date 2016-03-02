Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports javax.accessibility
Imports sun.java2d.pipe.hw.ExtendedBufferCapabilities.VSyncType
Imports [event]
Imports [event]

'
' * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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
    ''' A <em>component</em> is an object having a graphical representation
    ''' that can be displayed on the screen and that can interact with the
    ''' user. Examples of components are the buttons, checkboxes, and scrollbars
    ''' of a typical graphical user interface. <p>
    ''' The <code>Component</code> class is the abstract superclass of
    ''' the nonmenu-related Abstract Window Toolkit components. Class
    ''' <code>Component</code> can also be extended directly to create a
    ''' lightweight component. A lightweight component is a component that is
    ''' not associated with a native window. On the contrary, a heavyweight
    ''' component is associated with a native window. The <seealso cref="#isLightweight()"/>
    ''' method may be used to distinguish between the two kinds of the components.
    ''' <p>
    ''' Lightweight and heavyweight components may be mixed in a single component
    ''' hierarchy. However, for correct operating of such a mixed hierarchy of
    ''' components, the whole hierarchy must be valid. When the hierarchy gets
    ''' invalidated, like after changing the bounds of components, or
    ''' adding/removing components to/from containers, the whole hierarchy must be
    ''' validated afterwards by means of the <seealso cref="Container#validate()"/> method
    ''' invoked on the top-most invalid container of the hierarchy.
    ''' 
    ''' <h3>Serialization</h3>
    ''' It is important to note that only AWT listeners which conform
    ''' to the <code>Serializable</code> protocol will be saved when
    ''' the object is stored.  If an AWT object has listeners that
    ''' aren't marked serializable, they will be dropped at
    ''' <code>writeObject</code> time.  Developers will need, as always,
    ''' to consider the implications of making an object serializable.
    ''' One situation to watch out for is this:
    ''' <pre>
    '''    import java.awt.*;
    '''    import java.awt.event.*;
    '''    import java.io.Serializable;
    ''' 
    '''    class MyApp implements ActionListener, Serializable
    '''    {
    '''        BigObjectThatShouldNotBeSerializedWithAButton bigOne;
    '''        Button aButton = new Button();
    ''' 
    '''        MyApp()
    '''        {
    '''            // Oops, now aButton has a listener with a reference
    '''            // to bigOne!
    '''            aButton.addActionListener(this);
    '''        }
    ''' 
    '''        public void actionPerformed(ActionEvent e)
    '''        {
    '''            System.out.println("Hello There");
    '''        }
    '''    }
    ''' </pre>
    ''' In this example, serializing <code>aButton</code> by itself
    ''' will cause <code>MyApp</code> and everything it refers to
    ''' to be serialized as well.  The problem is that the listener
    ''' is serializable by coincidence, not by design.  To separate
    ''' the decisions about <code>MyApp</code> and the
    ''' <code>ActionListener</code> being serializable one can use a
    ''' nested [Class], as in the following example:
    ''' <pre>
    '''    import java.awt.*;
    '''    import java.awt.event.*;
    '''    import java.io.Serializable;
    ''' 
    '''    class MyApp implements java.io.Serializable
    '''    {
    '''         BigObjectThatShouldNotBeSerializedWithAButton bigOne;
    '''         Button aButton = new Button();
    ''' 
    '''         static class MyActionListener implements ActionListener
    '''         {
    '''             public void actionPerformed(ActionEvent e)
    '''             {
    '''                 System.out.println("Hello There");
    '''             }
    '''         }
    ''' 
    '''         MyApp()
    '''         {
    '''             aButton.addActionListener(new MyActionListener());
    '''         }
    '''    }
    ''' </pre>
    ''' <p>
    ''' <b>Note</b>: For more information on the paint mechanisms utilitized
    ''' by AWT and Swing, including information on how to write the most
    ''' efficient painting code, see
    ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
    ''' <p>
    ''' For details on the focus subsystem, see
    ''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
    ''' How to Use the Focus Subsystem</a>,
    ''' a section in <em>The Java Tutorial</em>, and the
    ''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
    ''' for more information.
    ''' 
    ''' @author      Arthur van Hoff
    ''' @author      Sami Shaio
    ''' </summary>
    <Serializable>
    Public MustInherit Class Component : Inherits java.lang.Object
        Implements java.awt.image.ImageObserver, MenuContainer

        Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.Component")
        Private Shared ReadOnly eventLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.event.Component")
        Private Shared ReadOnly focusLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.focus.Component")
        Private Shared ReadOnly mixingLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.mixing.Component")

        ''' <summary>
        ''' The peer of the component. The peer implements the component's
        ''' behavior. The peer is set when the <code>Component</code> is
        ''' added to a container that also is a peer. </summary>
        ''' <seealso cref= #addNotify </seealso>
        ''' <seealso cref= #removeNotify </seealso>
        <NonSerialized>
        Friend peer As java.awt.peer.ComponentPeer

        ''' <summary>
        ''' The parent of the object. It may be <code>null</code>
        ''' for top-level components. </summary>
        ''' <seealso cref= #getParent </seealso>
        <NonSerialized>
        Friend parent As Container

        ''' <summary>
        ''' The <code>AppContext</code> of the component. Applets/Plugin may
        ''' change the AppContext.
        ''' </summary>
        <NonSerialized>
        Friend appContext As sun.awt.AppContext

        ''' <summary>
        ''' The x position of the component in the parent's coordinate system.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getLocation </seealso>
        Friend x As Integer

        ''' <summary>
        ''' The y position of the component in the parent's coordinate system.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getLocation </seealso>
        Friend y As Integer

        ''' <summary>
        ''' The width of the component.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getSize </seealso>
        Friend width As Integer

        ''' <summary>
        ''' The height of the component.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getSize </seealso>
        Friend height As Integer

        ''' <summary>
        ''' The foreground color for this component.
        ''' <code>foreground</code> can be <code>null</code>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getForeground </seealso>
        ''' <seealso cref= #setForeground </seealso>
        Friend foreground As color

        ''' <summary>
        ''' The background color for this component.
        ''' <code>background</code> can be <code>null</code>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getBackground </seealso>
        ''' <seealso cref= #setBackground </seealso>
        Friend background As color

        ''' <summary>
        ''' The font used by this component.
        ''' The <code>font</code> can be <code>null</code>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getFont </seealso>
        ''' <seealso cref= #setFont </seealso>
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        Friend font As font

        ''' <summary>
        ''' The font which the peer is currently using.
        ''' (<code>null</code> if no peer exists.)
        ''' </summary>
        Friend peerFont As font

        ''' <summary>
        ''' The cursor displayed when pointer is over this component.
        ''' This value can be <code>null</code>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getCursor </seealso>
        ''' <seealso cref= #setCursor </seealso>
        Friend cursor As Cursor

        ''' <summary>
        ''' The locale for the component.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getLocale </seealso>
        ''' <seealso cref= #setLocale </seealso>
        Friend locale As java.util.Locale

        ''' <summary>
        ''' A reference to a <code>GraphicsConfiguration</code> object
        ''' used to describe the characteristics of a graphics
        ''' destination.
        ''' This value can be <code>null</code>.
        ''' 
        ''' @since 1.3
        ''' @serial </summary>
        ''' <seealso cref= GraphicsConfiguration </seealso>
        ''' <seealso cref= #getGraphicsConfiguration </seealso>
        <NonSerialized>
        Private graphicsConfig As GraphicsConfiguration = Nothing

        ''' <summary>
        ''' A reference to a <code>BufferStrategy</code> object
        ''' used to manipulate the buffers on this component.
        ''' 
        ''' @since 1.4 </summary>
        ''' <seealso cref= java.awt.image.BufferStrategy </seealso>
        ''' <seealso cref= #getBufferStrategy() </seealso>
        <NonSerialized>
        Friend _bufferStrategy As java.awt.image.BufferStrategy = Nothing

        ''' <summary>
        ''' True when the object should ignore all repaint events.
        ''' 
        ''' @since 1.4
        ''' @serial </summary>
        ''' <seealso cref= #setIgnoreRepaint </seealso>
        ''' <seealso cref= #getIgnoreRepaint </seealso>
        Friend ignoreRepaint As Boolean = False

        ''' <summary>
        ''' True when the object is visible. An object that is not
        ''' visible is not drawn on the screen.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #isVisible </seealso>
        ''' <seealso cref= #setVisible </seealso>
        Friend visible As Boolean = True

        ''' <summary>
        ''' True when the object is enabled. An object that is not
        ''' enabled does not interact with the user.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #isEnabled </seealso>
        ''' <seealso cref= #setEnabled </seealso>
        Friend enabled As Boolean = True

        ''' <summary>
        ''' True when the object is valid. An invalid object needs to
        ''' be layed out. This flag is set to false when the object
        ''' size is changed.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #isValid </seealso>
        ''' <seealso cref= #validate </seealso>
        ''' <seealso cref= #invalidate </seealso>
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        Private valid As Boolean = False

        ''' <summary>
        ''' The <code>DropTarget</code> associated with this component.
        ''' 
        ''' @since 1.2
        ''' @serial </summary>
        ''' <seealso cref= #setDropTarget </seealso>
        ''' <seealso cref= #getDropTarget </seealso>
        Friend dropTarget As java.awt.dnd.DropTarget

        ''' <summary>
        ''' @serial </summary>
        ''' <seealso cref= #add </seealso>
        Friend popups As List(Of PopupMenu)

        ''' <summary>
        ''' A component's name.
        ''' This field can be <code>null</code>.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getName </seealso>
        ''' <seealso cref= #setName(String) </seealso>
        Private name As String

        ''' <summary>
        ''' A bool to determine whether the name has
        ''' been set explicitly. <code>nameExplicitlySet</code> will
        ''' be false if the name has not been set and
        ''' true if it has.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getName </seealso>
        ''' <seealso cref= #setName(String) </seealso>
        Private nameExplicitlySet As Boolean = False

        ''' <summary>
        ''' Indicates whether this Component can be focused.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #setFocusable </seealso>
        ''' <seealso cref= #isFocusable
        ''' @since 1.4 </seealso>
        Private focusable As Boolean = True

        Private Const FOCUS_TRAVERSABLE_UNKNOWN As Integer = 0
        Private Const FOCUS_TRAVERSABLE_DEFAULT As Integer = 1
        Private Const FOCUS_TRAVERSABLE_SET As Integer = 2

        ''' <summary>
        ''' Tracks whether this Component is relying on default focus travesability.
        ''' 
        ''' @serial
        ''' @since 1.4
        ''' </summary>
        Private isFocusTraversableOverridden_Renamed As Integer = FOCUS_TRAVERSABLE_UNKNOWN

        ''' <summary>
        ''' The focus traversal keys. These keys will generate focus traversal
        ''' behavior for Components for which focus traversal keys are enabled. If a
        ''' value of null is specified for a traversal key, this Component inherits
        ''' that traversal key from its parent. If all ancestors of this Component
        ''' have null specified for that traversal key, then the current
        ''' KeyboardFocusManager's default traversal key is used.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #setFocusTraversalKeys </seealso>
        ''' <seealso cref= #getFocusTraversalKeys
        ''' @since 1.4 </seealso>
        Friend focusTraversalKeys As java.util.Set(Of AWTKeyStroke)()

        Private Shared ReadOnly focusTraversalKeyPropertyNames As String() = {"forwardFocusTraversalKeys", "backwardFocusTraversalKeys", "upCycleFocusTraversalKeys", "downCycleFocusTraversalKeys"}

        ''' <summary>
        ''' Indicates whether focus traversal keys are enabled for this Component.
        ''' Components for which focus traversal keys are disabled receive key
        ''' events for focus traversal keys. Components for which focus traversal
        ''' keys are enabled do not see these events; instead, the events are
        ''' automatically converted to traversal operations.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #setFocusTraversalKeysEnabled </seealso>
        ''' <seealso cref= #getFocusTraversalKeysEnabled
        ''' @since 1.4 </seealso>
        Private focusTraversalKeysEnabled As Boolean = True

        ''' <summary>
        ''' The locking object for AWT component-tree and layout operations.
        ''' </summary>
        ''' <seealso cref= #getTreeLock </seealso>
        Friend Shared ReadOnly LOCK As Object = New AWTTreeLock
        Friend Class AWTTreeLock
        End Class

        '    
        '     * The component's AccessControlContext.
        '     
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private acc As java.security.AccessControlContext = java.security.AccessController.context

        ''' <summary>
        ''' Minimum size.
        ''' (This field perhaps should have been transient).
        ''' 
        ''' @serial
        ''' </summary>
        Friend minSize As Dimension

        ''' <summary>
        ''' Whether or not setMinimumSize has been invoked with a non-null value.
        ''' </summary>
        Friend minSizeSet As Boolean

        ''' <summary>
        ''' Preferred size.
        ''' (This field perhaps should have been transient).
        ''' 
        ''' @serial
        ''' </summary>
        Friend prefSize As Dimension

        ''' <summary>
        ''' Whether or not setPreferredSize has been invoked with a non-null value.
        ''' </summary>
        Friend prefSizeSet As Boolean

        ''' <summary>
        ''' Maximum size
        ''' 
        ''' @serial
        ''' </summary>
        Friend maxSize As Dimension

        ''' <summary>
        ''' Whether or not setMaximumSize has been invoked with a non-null value.
        ''' </summary>
        Friend maxSizeSet As Boolean

        ''' <summary>
        ''' The orientation for this component. </summary>
        ''' <seealso cref= #getComponentOrientation </seealso>
        ''' <seealso cref= #setComponentOrientation </seealso>
        <NonSerialized>
        Friend _componentOrientation As ComponentOrientation = ComponentOrientation.UNKNOWN

        ''' <summary>
        ''' <code>newEventsOnly</code> will be true if the event is
        ''' one of the event types enabled for the component.
        ''' It will then allow for normal processing to
        ''' continue.  If it is false the event is passed
        ''' to the component's parent and up the ancestor
        ''' tree until the event has been consumed.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #dispatchEvent </seealso>
        Friend newEventsOnly As Boolean = False
        <NonSerialized>
        Friend componentListener As ComponentListener
        <NonSerialized>
        Friend focusListener As FocusListener
        <NonSerialized>
        Friend hierarchyListener As HierarchyListener
        <NonSerialized>
        Friend hierarchyBoundsListener As HierarchyBoundsListener
        <NonSerialized>
        Friend keyListener As KeyListener
        <NonSerialized>
        Friend mouseListener As MouseListener
        <NonSerialized>
        Friend mouseMotionListener As MouseMotionListener
        <NonSerialized>
        Friend mouseWheelListener As MouseWheelListener
        <NonSerialized>
        Friend inputMethodListener As InputMethodListener

        <NonSerialized>
        Friend windowClosingException As RuntimeException = Nothing

        ''' <summary>
        ''' Internal, constants for serialization </summary>
        Friend Const actionListenerK As String = "actionL"
        Friend Const adjustmentListenerK As String = "adjustmentL"
        Friend Const componentListenerK As String = "componentL"
        Friend Const containerListenerK As String = "containerL"
        Friend Const focusListenerK As String = "focusL"
        Friend Const itemListenerK As String = "itemL"
        Friend Const keyListenerK As String = "keyL"
        Friend Const mouseListenerK As String = "mouseL"
        Friend Const mouseMotionListenerK As String = "mouseMotionL"
        Friend Const mouseWheelListenerK As String = "mouseWheelL"
        Friend Const textListenerK As String = "textL"
        Friend Const ownedWindowK As String = "ownedL"
        Friend Const windowListenerK As String = "windowL"
        Friend Const inputMethodListenerK As String = "inputMethodL"
        Friend Const hierarchyListenerK As String = "hierarchyL"
        Friend Const hierarchyBoundsListenerK As String = "hierarchyBoundsL"
        Friend Const windowStateListenerK As String = "windowStateL"
        Friend Const windowFocusListenerK As String = "windowFocusL"

        ''' <summary>
        ''' The <code>eventMask</code> is ONLY set by subclasses via
        ''' <code>enableEvents</code>.
        ''' The mask should NOT be set when listeners are registered
        ''' so that we can distinguish the difference between when
        ''' listeners request events and subclasses request them.
        ''' One bit is used to indicate whether input methods are
        ''' enabled; this bit is set by <code>enableInputMethods</code> and is
        ''' on by default.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #enableInputMethods </seealso>
        ''' <seealso cref= AWTEvent </seealso>
        Friend eventMask As Long = AWTEvent.INPUT_METHODS_ENABLED_MASK

        ''' <summary>
        ''' Static properties for incremental drawing. </summary>
        ''' <seealso cref= #imageUpdate </seealso>
        Friend Shared isInc As Boolean
        Friend Shared incRate As Integer
        Shared Sub New()
            ' ensure that the necessary native libraries are loaded 
            Toolkit.loadLibraries()
            ' initialize JNI field and method ids 
            If Not GraphicsEnvironment.headless Then initIDs()

            Dim s As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("awt.image.incrementaldraw"))
            isInc = (s Is Nothing OrElse s.Equals("true"))

            s = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("awt.image.redrawrate"))
            incRate = If(s IsNot Nothing, Convert.ToInt32(s), 100)
            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			sun.awt.AWTAccessor.setComponentAccessor(New sun.awt.AWTAccessor.ComponentAccessor()
            '		{
            '			public void setBackgroundEraseDisabled(Component comp, boolean disabled)
            '			{
            '				comp.backgroundEraseDisabled = disabled;
            '			}
            '			public boolean getBackgroundEraseDisabled(Component comp)
            '			{
            '				Return comp.backgroundEraseDisabled;
            '			}
            '			public Rectangle getBounds(Component comp)
            '			{
            '				Return New Rectangle(comp.x, comp.y, comp.width, comp.height);
            '			}
            '			public void setMixingCutoutShape(Component comp, Shape shape)
            '			{
            '				Region region = shape == Nothing ? Nothing : Region.getInstance(shape, Nothing);
            '
            '				synchronized(comp.getTreeLock())
            '				{
            '					boolean needShowing = False;
            '					boolean needHiding = False;
            '
            '					if (!comp.isNonOpaqueForMixing())
            '					{
            '						needHiding = True;
            '					}
            '
            '					comp.mixingCutoutRegion = region;
            '
            '					if (!comp.isNonOpaqueForMixing())
            '					{
            '						needShowing = True;
            '					}
            '
            '					if (comp.isMixingNeeded())
            '					{
            '						if (needHiding)
            '						{
            '							comp.mixOnHiding(comp.isLightweight());
            '						}
            '						if (needShowing)
            '						{
            '							comp.mixOnShowing();
            '						}
            '					}
            '				}
            '			}
            '
            '			public void setGraphicsConfiguration(Component comp, GraphicsConfiguration gc)
            '			{
            '				comp.setGraphicsConfiguration(gc);
            '			}
            '			public boolean requestFocus(Component comp, CausedFocusEvent.Cause cause)
            '			{
            '				Return comp.requestFocus(cause);
            '			}
            '			public boolean canBeFocusOwner(Component comp)
            '			{
            '				Return comp.canBeFocusOwner();
            '			}
            '
            '			public boolean isVisible(Component comp)
            '			{
            '				Return comp.isVisible_NoClientCode();
            '			}
            '			public void setRequestFocusController(RequestFocusController requestController)
            '			{
            '				 Component.setRequestFocusController(requestController);
            '			}
            '			public AppContext getAppContext(Component comp)
            '			{
            '				 Return comp.appContext;
            '			}
            '			public void setAppContext(Component comp, AppContext appContext)
            '			{
            '				 comp.appContext = appContext;
            '			}
            '			public Container getParent(Component comp)
            '			{
            '				Return comp.getParent_NoClientCode();
            '			}
            '			public void setParent(Component comp, Container parent)
            '			{
            '				comp.parent = parent;
            '			}
            '			public void setSize(Component comp, int width, int height)
            '			{
            '				comp.width = width;
            '				comp.height = height;
            '			}
            '			public Point getLocation(Component comp)
            '			{
            '				Return comp.location_NoClientCode();
            '			}
            '			public void setLocation(Component comp, int x, int y)
            '			{
            '				comp.x = x;
            '				comp.y = y;
            '			}
            '			public boolean isEnabled(Component comp)
            '			{
            '				Return comp.isEnabledImpl();
            '			}
            '			public boolean isDisplayable(Component comp)
            '			{
            '				Return comp.peer != Nothing;
            '			}
            '			public Cursor getCursor(Component comp)
            '			{
            '				Return comp.getCursor_NoClientCode();
            '			}
            '			public ComponentPeer getPeer(Component comp)
            '			{
            '				Return comp.peer;
            '			}
            '			public void setPeer(Component comp, ComponentPeer peer)
            '			{
            '				comp.peer = peer;
            '			}
            '			public boolean isLightweight(Component comp)
            '			{
            '				Return (comp.peer instanceof LightweightPeer);
            '			}
            '			public boolean getIgnoreRepaint(Component comp)
            '			{
            '				Return comp.ignoreRepaint;
            '			}
            '			public int getWidth(Component comp)
            '			{
            '				Return comp.width;
            '			}
            '			public int getHeight(Component comp)
            '			{
            '				Return comp.height;
            '			}
            '			public int getX(Component comp)
            '			{
            '				Return comp.x;
            '			}
            '			public int getY(Component comp)
            '			{
            '				Return comp.y;
            '			}
            '			public Color getForeground(Component comp)
            '			{
            '				Return comp.foreground;
            '			}
            '			public Color getBackground(Component comp)
            '			{
            '				Return comp.background;
            '			}
            '			public void setBackground(Component comp, Color background)
            '			{
            '				comp.background = background;
            '			}
            '			public Font getFont(Component comp)
            '			{
            '				Return comp.getFont_NoClientCode();
            '			}
            '			public void processEvent(Component comp, AWTEvent e)
            '			{
            '				comp.processEvent(e);
            '			}
            '
            '			public AccessControlContext getAccessControlContext(Component comp)
            '			{
            '				Return comp.getAccessControlContext();
            '			}
            '
            '			public void revalidateSynchronously(Component comp)
            '			{
            '				comp.revalidateSynchronously();
            '			}
            '		});
        End Sub

        ''' <summary>
        ''' Ease-of-use constant for <code>getAlignmentY()</code>.
        ''' Specifies an alignment to the top of the component. </summary>
        ''' <seealso cref=     #getAlignmentY </seealso>
        Public Const TOP_ALIGNMENT As Single = 0.0F

        ''' <summary>
        ''' Ease-of-use constant for <code>getAlignmentY</code> and
        ''' <code>getAlignmentX</code>. Specifies an alignment to
        ''' the center of the component </summary>
        ''' <seealso cref=     #getAlignmentX </seealso>
        ''' <seealso cref=     #getAlignmentY </seealso>
        Public Const CENTER_ALIGNMENT As Single = 0.5F

        ''' <summary>
        ''' Ease-of-use constant for <code>getAlignmentY</code>.
        ''' Specifies an alignment to the bottom of the component. </summary>
        ''' <seealso cref=     #getAlignmentY </seealso>
        Public Const BOTTOM_ALIGNMENT As Single = 1.0F

        ''' <summary>
        ''' Ease-of-use constant for <code>getAlignmentX</code>.
        ''' Specifies an alignment to the left side of the component. </summary>
        ''' <seealso cref=     #getAlignmentX </seealso>
        Public Const LEFT_ALIGNMENT As Single = 0.0F

        ''' <summary>
        ''' Ease-of-use constant for <code>getAlignmentX</code>.
        ''' Specifies an alignment to the right side of the component. </summary>
        ''' <seealso cref=     #getAlignmentX </seealso>
        Public Const RIGHT_ALIGNMENT As Single = 1.0F

        '    
        '     * JDK 1.1 serialVersionUID
        '     
        Private Const serialVersionUID As Long = -7644114512714619750L

        ''' <summary>
        ''' If any <code>PropertyChangeListeners</code> have been registered,
        ''' the <code>changeSupport</code> field describes them.
        ''' 
        ''' @serial
        ''' @since 1.2 </summary>
        ''' <seealso cref= #addPropertyChangeListener </seealso>
        ''' <seealso cref= #removePropertyChangeListener </seealso>
        ''' <seealso cref= #firePropertyChange </seealso>
        Private changeSupport As java.beans.PropertyChangeSupport

        '    
        '     * In some cases using "this" as an object to synchronize by
        '     * can lead to a deadlock if client code also uses synchronization
        '     * by a component object. For every such situation revealed we should
        '     * consider possibility of replacing "this" with the package private
        '     * objectLock object introduced below. So far there're 3 issues known:
        '     * - CR 6708322 (the getName/setName methods);
        '     * - CR 6608764 (the PropertyChangeListener machinery);
        '     * - CR 7108598 (the Container.paint/KeyboardFocusManager.clearMostRecentFocusOwner methods).
        '     *
        '     * Note: this field is considered final, though readObject() prohibits
        '     * initializing final fields.
        '     
        <NonSerialized>
        Private _objectLock As New Object
        Friend Overridable ReadOnly Property objectLock As Object
            Get
                Return _objectLock
            End Get
        End Property

        '    
        '     * Returns the acc this component was constructed with.
        '     
        Friend ReadOnly Property accessControlContext As java.security.AccessControlContext
            Get
                If acc Is Nothing Then Throw New SecurityException("Component is missing AccessControlContext")
                Return acc
            End Get
        End Property

        Friend isPacked As Boolean = False

        ''' <summary>
        ''' Pseudoparameter for direct Geometry API (setLocation, setBounds setSize
        ''' to signal setBounds what's changing. Should be used under TreeLock.
        ''' This is only needed due to the inability to change the cross-calling
        ''' order of public and deprecated methods.
        ''' </summary>
        Private boundsOp As Integer = java.awt.peer.ComponentPeer.DEFAULT_OPERATION

        ''' <summary>
        ''' Enumeration of the common ways the baseline of a component can
        ''' change as the size changes.  The baseline resize behavior is
        ''' primarily for layout managers that need to know how the
        ''' position of the baseline changes as the component size changes.
        ''' In general the baseline resize behavior will be valid for sizes
        ''' greater than or equal to the minimum size (the actual minimum
        ''' size; not a developer specified minimum size).  For sizes
        ''' smaller than the minimum size the baseline may change in a way
        ''' other than the baseline resize behavior indicates.  Similarly,
        ''' as the size approaches <code> java.lang.[Integer].MAX_VALUE</code> and/or
        ''' <code> java.lang.[Short].MAX_VALUE</code> the baseline may change in a way
        ''' other than the baseline resize behavior indicates.
        ''' </summary>
        ''' <seealso cref= #getBaselineResizeBehavior </seealso>
        ''' <seealso cref= #getBaseline(int,int)
        ''' @since 1.6 </seealso>
        Public Enum BaselineResizeBehavior
            ''' <summary>
            ''' Indicates the baseline remains fixed relative to the
            ''' y-origin.  That is, <code>getBaseline</code> returns
            ''' the same value regardless of the height or width.  For example, a
            ''' <code>JLabel</code> containing non-empty text with a
            ''' vertical alignment of <code>TOP</code> should have a
            ''' baseline type of <code>CONSTANT_ASCENT</code>.
            ''' </summary>
            CONSTANT_ASCENT

            ''' <summary>
            ''' Indicates the baseline remains fixed relative to the height
            ''' and does not change as the width is varied.  That is, for
            ''' any height H the difference between H and
            ''' <code>getBaseline(w, H)</code> is the same.  For example, a
            ''' <code>JLabel</code> containing non-empty text with a
            ''' vertical alignment of <code>BOTTOM</code> should have a
            ''' baseline type of <code>CONSTANT_DESCENT</code>.
            ''' </summary>
            CONSTANT_DESCENT

            ''' <summary>
            ''' Indicates the baseline remains a fixed distance from
            ''' the center of the component.  That is, for any height H the
            ''' difference between <code>getBaseline(w, H)</code> and
            ''' <code>H / 2</code> is the same (plus or minus one depending upon
            ''' rounding error).
            ''' <p>
            ''' Because of possible rounding errors it is recommended
            ''' you ask for the baseline with two consecutive heights and use
            ''' the return value to determine if you need to pad calculations
            ''' by 1.  The following shows how to calculate the baseline for
            ''' any height:
            ''' <pre>
            '''   Dimension preferredSize = component.getPreferredSize();
            '''   int baseline = getBaseline(preferredSize.width,
            '''                              preferredSize.height);
            '''   int nextBaseline = getBaseline(preferredSize.width,
            '''                                  preferredSize.height + 1);
            '''   // Amount to add to height when calculating where baseline
            '''   // lands for a particular height:
            '''   int padding = 0;
            '''   // Where the baseline is relative to the mid point
            '''   int baselineOffset = baseline - height / 2;
            '''   if (preferredSize.height % 2 == 0 &amp;&amp;
            '''       baseline != nextBaseline) {
            '''       padding = 1;
            '''   }
            '''   else if (preferredSize.height % 2 == 1 &amp;&amp;
            '''            baseline == nextBaseline) {
            '''       baselineOffset--;
            '''       padding = 1;
            '''   }
            '''   // The following calculates where the baseline lands for
            '''   // the height z:
            '''   int calculatedBaseline = (z + padding) / 2 + baselineOffset;
            ''' </pre>
            ''' </summary>
            CENTER_OFFSET

            ''' <summary>
            ''' Indicates the baseline resize behavior can not be expressed using
            ''' any of the other constants.  This may also indicate the baseline
            ''' varies with the width of the component.  This is also returned
            ''' by components that do not have a baseline.
            ''' </summary>
            OTHER
        End Enum

        '    
        '     * The shape set with the applyCompoundShape() method. It uncludes the result
        '     * of the HW/LW mixing related shape computation. It may also include
        '     * the user-specified shape of the component.
        '     * The 'null' value means the component has normal shape (or has no shape at all)
        '     * and applyCompoundShape() will skip the following shape identical to normal.
        '     
        <NonSerialized>
        Private compoundShape As sun.java2d.pipe.Region = Nothing

        '    
        '     * Represents the shape of this lightweight component to be cut out from
        '     * heavyweight components should they intersect. Possible values:
        '     *    1. null - consider the shape rectangular
        '     *    2. EMPTY_REGION - nothing gets cut out (children still get cut out)
        '     *    3. non-empty - this shape gets cut out.
        '     
        <NonSerialized>
        Private mixingCutoutRegion As sun.java2d.pipe.Region = Nothing

        '    
        '     * Indicates whether addNotify() is complete
        '     * (i.e. the peer is created).
        '     
        <NonSerialized>
        Private isAddNotifyComplete As Boolean = False

        ''' <summary>
        ''' Should only be used in subclass getBounds to check that part of bounds
        ''' is actualy changing
        ''' </summary>
        Friend Overridable Property boundsOp As Integer
            Get
                Debug.Assert(Thread.holdsLock(treeLock))
                Return boundsOp
            End Get
            Set(ByVal op As Integer)
                Debug.Assert(Thread.holdsLock(treeLock))
                If op = java.awt.peer.ComponentPeer.RESET_OPERATION Then
                    boundsOp = java.awt.peer.ComponentPeer.DEFAULT_OPERATION
                Else
                    If boundsOp = java.awt.peer.ComponentPeer.DEFAULT_OPERATION Then boundsOp = op
                End If
            End Set
        End Property


        ' Whether this Component has had the background erase flag
        ' specified via SunToolkit.disableBackgroundErase(). This is
        ' needed in order to make this function work on X11 platforms,
        ' where currently there is no chance to interpose on the creation
        ' of the peer and therefore the call to XSetBackground.
        <NonSerialized>
        Friend backgroundEraseDisabled As Boolean


        ''' <summary>
        ''' Constructs a new component. Class <code>Component</code> can be
        ''' extended directly to create a lightweight component that does not
        ''' utilize an opaque native window. A lightweight component must be
        ''' hosted by a native container somewhere higher up in the component
        ''' tree (for example, by a <code>Frame</code> object).
        ''' </summary>
        Protected Friend Sub New()
            appContext = sun.awt.AppContext.appContext
        End Sub

        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Friend Overridable Sub initializeFocusTraversalKeys()
            focusTraversalKeys = New java.util.Set(2) {}
        End Sub

        ''' <summary>
        ''' Constructs a name for this component.  Called by <code>getName</code>
        ''' when the name is <code>null</code>.
        ''' </summary>
        Friend Overridable Function constructComponentName() As String
            Return Nothing ' For strict compliance with prior platform versions, a Component
            ' that doesn't set its name should return null from
            ' getName()
        End Function

        ''' <summary>
        ''' Gets the name of the component. </summary>
        ''' <returns> this component's name </returns>
        ''' <seealso cref=    #setName
        ''' @since JDK1.1 </seealso>
        Public Overridable Property name As String
            Get
                If name Is Nothing AndAlso (Not nameExplicitlySet) Then
                    SyncLock objectLock
                        If name Is Nothing AndAlso (Not nameExplicitlySet) Then name = constructComponentName()
                    End SyncLock
                End If
                Return name
            End Get
            Set(ByVal name As String)
                Dim oldName As String
                SyncLock objectLock
                    oldName = Me.name
                    Me.name = name
                    nameExplicitlySet = True
                End SyncLock
                firePropertyChange("name", oldName, name)
            End Set
        End Property


        ''' <summary>
        ''' Gets the parent of this component. </summary>
        ''' <returns> the parent container of this component
        ''' @since JDK1.0 </returns>
        Public Overridable Property parent As Container
            Get
                Return parent_NoClientCode
            End Get
        End Property

        ' NOTE: This method may be called by privileged threads.
        '       This functionality is implemented in a package-private method
        '       to insure that it cannot be overridden by client subclasses.
        '       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
        Friend Property parent_NoClientCode As Container
            Get
                Return parent
            End Get
        End Property

        ' This method is overridden in the Window class to return null,
        '    because the parent field of the Window object contains
        '    the owner of the window, not its parent.
        Friend Overridable Property container As Container
            Get
                Return parent_NoClientCode
            End Get
        End Property

        ''' @deprecated As of JDK version 1.1,
        ''' programs should not directly manipulate peers;
        ''' replaced by <code>boolean isDisplayable()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Property peer As java.awt.peer.ComponentPeer
            Get
                Return peer
            End Get
        End Property

        ''' <summary>
        ''' Associate a <code>DropTarget</code> with this component.
        ''' The <code>Component</code> will receive drops only if it
        ''' is enabled.
        ''' </summary>
        ''' <seealso cref= #isEnabled </seealso>
        ''' <param name="dt"> The DropTarget </param>

        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property dropTarget As java.awt.dnd.DropTarget
            Set(ByVal dt As java.awt.dnd.DropTarget)
                If dt Is dropTarget OrElse (dropTarget IsNot Nothing AndAlso dropTarget.Equals(dt)) Then Return

                Dim old As java.awt.dnd.DropTarget

                old = dropTarget
                If old IsNot Nothing Then
                    If peer IsNot Nothing Then dropTarget.removeNotify(peer)

                    Dim t As java.awt.dnd.DropTarget = dropTarget

                    dropTarget = Nothing

                    Try
                        t.component = Nothing
                    Catch iae As IllegalArgumentException
                        ' ignore it.
                    End Try
                End If

                ' if we have a new one, and we have a peer, add it!

                dropTarget = dt
                If dropTarget IsNot Nothing Then
                    Try
                        dropTarget.component = Me
                        If peer IsNot Nothing Then dropTarget.addNotify(peer)
                    Catch iae As IllegalArgumentException
                        If old IsNot Nothing Then
                            Try
                                old.component = Me
                                If peer IsNot Nothing Then dropTarget.addNotify(peer)
                            Catch iae1 As IllegalArgumentException
                                ' ignore it!
                            End Try
                        End If
                    End Try
                End If
            End Set
            Get
                Return dropTarget
            End Get
        End Property



        ''' <summary>
        ''' Gets the <code>GraphicsConfiguration</code> associated with this
        ''' <code>Component</code>.
        ''' If the <code>Component</code> has not been assigned a specific
        ''' <code>GraphicsConfiguration</code>,
        ''' the <code>GraphicsConfiguration</code> of the
        ''' <code>Component</code> object's top-level container is
        ''' returned.
        ''' If the <code>Component</code> has been created, but not yet added
        ''' to a <code>Container</code>, this method returns <code>null</code>.
        ''' </summary>
        ''' <returns> the <code>GraphicsConfiguration</code> used by this
        '''          <code>Component</code> or <code>null</code>
        ''' @since 1.3 </returns>
        Public Overridable Property graphicsConfiguration As GraphicsConfiguration
            Get
                SyncLock treeLock
                    Return graphicsConfiguration_NoClientCode
                End SyncLock
            End Get
            Set(ByVal gc As GraphicsConfiguration)
                SyncLock treeLock
                    If updateGraphicsData(gc) Then
                        removeNotify()
                        addNotify()
                    End If
                End SyncLock
            End Set
        End Property

        Friend Property graphicsConfiguration_NoClientCode As GraphicsConfiguration
            Get
                Return graphicsConfig
            End Get
        End Property


        Friend Overridable Function updateGraphicsData(ByVal gc As GraphicsConfiguration) As Boolean
            checkTreeLock()

            If graphicsConfig Is gc Then Return False

            graphicsConfig = gc

            Dim peer_Renamed As java.awt.peer.ComponentPeer = peer
            If peer_Renamed IsNot Nothing Then Return peer_Renamed.updateGraphicsData(gc)
            Return False
        End Function

        ''' <summary>
        ''' Checks that this component's <code>GraphicsDevice</code>
        ''' <code>idString</code> matches the string argument.
        ''' </summary>
        Friend Overridable Sub checkGD(ByVal stringID As String)
            If graphicsConfig IsNot Nothing Then
                If Not graphicsConfig.device.iDstring.Equals(stringID) Then Throw New IllegalArgumentException("adding a container to a container on a different GraphicsDevice")
            End If
        End Sub

        ''' <summary>
        ''' Gets this component's locking object (the object that owns the thread
        ''' synchronization monitor) for AWT component-tree and layout
        ''' operations. </summary>
        ''' <returns> this component's locking object </returns>
        Public Property treeLock As Object
            Get
                Return LOCK
            End Get
        End Property

        Friend Sub checkTreeLock()
            If Not Thread.holdsLock(treeLock) Then Throw New IllegalStateException("This function should be called while holding treeLock")
        End Sub

        ''' <summary>
        ''' Gets the toolkit of this component. Note that
        ''' the frame that contains a component controls which
        ''' toolkit is used by that component. Therefore if the component
        ''' is moved from one frame to another, the toolkit it uses may change. </summary>
        ''' <returns>  the toolkit of this component
        ''' @since JDK1.0 </returns>
        Public Overridable ReadOnly Property toolkit As Toolkit
            Get
                Return toolkitImpl
            End Get
        End Property

        '    
        '     * This is called by the native code, so client code can't
        '     * be called on the toolkit thread.
        '     
        Friend ReadOnly Property toolkitImpl As Toolkit
            Get
                Dim parent_Renamed As Container = Me.parent
                If parent_Renamed IsNot Nothing Then Return parent_Renamed.toolkitImpl
                Return toolkit.defaultToolkit
            End Get
        End Property

        ''' <summary>
        ''' Determines whether this component is valid. A component is valid
        ''' when it is correctly sized and positioned within its parent
        ''' container and all its children are also valid.
        ''' In order to account for peers' size requirements, components are invalidated
        ''' before they are first shown on the screen. By the time the parent container
        ''' is fully realized, all its components will be valid. </summary>
        ''' <returns> <code>true</code> if the component is valid, <code>false</code>
        ''' otherwise </returns>
        ''' <seealso cref= #validate </seealso>
        ''' <seealso cref= #invalidate
        ''' @since JDK1.0 </seealso>
        Public Overridable ReadOnly Property valid As Boolean
            Get
                Return (peer IsNot Nothing) AndAlso valid
            End Get
        End Property

        ''' <summary>
        ''' Determines whether this component is displayable. A component is
        ''' displayable when it is connected to a native screen resource.
        ''' <p>
        ''' A component is made displayable either when it is added to
        ''' a displayable containment hierarchy or when its containment
        ''' hierarchy is made displayable.
        ''' A containment hierarchy is made displayable when its ancestor
        ''' window is either packed or made visible.
        ''' <p>
        ''' A component is made undisplayable either when it is removed from
        ''' a displayable containment hierarchy or when its containment hierarchy
        ''' is made undisplayable.  A containment hierarchy is made
        ''' undisplayable when its ancestor window is disposed.
        ''' </summary>
        ''' <returns> <code>true</code> if the component is displayable,
        ''' <code>false</code> otherwise </returns>
        ''' <seealso cref= Container#add(Component) </seealso>
        ''' <seealso cref= Window#pack </seealso>
        ''' <seealso cref= Window#show </seealso>
        ''' <seealso cref= Container#remove(Component) </seealso>
        ''' <seealso cref= Window#dispose
        ''' @since 1.2 </seealso>
        Public Overridable ReadOnly Property displayable As Boolean
            Get
                Return peer IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Determines whether this component should be visible when its
        ''' parent is visible. Components are
        ''' initially visible, with the exception of top level components such
        ''' as <code>Frame</code> objects. </summary>
        ''' <returns> <code>true</code> if the component is visible,
        ''' <code>false</code> otherwise </returns>
        ''' <seealso cref= #setVisible
        ''' @since JDK1.0 </seealso>
        Public Overridable ReadOnly Property visible As Boolean
            Get
                Return visible_NoClientCode
            End Get
        End Property

        Friend ReadOnly Property visible_NoClientCode As Boolean
            Get
                Return visible
            End Get
        End Property

        ''' <summary>
        ''' Determines whether this component will be displayed on the screen. </summary>
        ''' <returns> <code>true</code> if the component and all of its ancestors
        '''          until a toplevel window or null parent are visible,
        '''          <code>false</code> otherwise </returns>
        Friend Overridable ReadOnly Property recursivelyVisible As Boolean
            Get
                Return visible AndAlso (parent Is Nothing OrElse parent.recursivelyVisible)
            End Get
        End Property

        ''' <summary>
        ''' Determines the bounds of a visible part of the component relative to its
        ''' parent.
        ''' </summary>
        ''' <returns> the visible part of bounds </returns>
        Private ReadOnly Property recursivelyVisibleBounds As Rectangle
            Get
                Dim container_Renamed As Component = container
                Dim bounds_Renamed As Rectangle = bounds
                If container_Renamed Is Nothing Then Return bounds_Renamed
                ' translate the container's bounds to our coordinate space
                Dim parentsBounds As Rectangle = container_Renamed.recursivelyVisibleBounds
                parentsBounds.locationion(0, 0)
                Return parentsBounds.intersection(bounds_Renamed)
            End Get
        End Property

        ''' <summary>
        ''' Translates absolute coordinates into coordinates in the coordinate
        ''' space of this component.
        ''' </summary>
        Friend Overridable Function pointRelativeToComponent(ByVal absolute As Point) As Point
            Dim compCoords As Point = locationOnScreen
            Return New Point(absolute.x - compCoords.x, absolute.y - compCoords.y)
        End Function

        ''' <summary>
        ''' Assuming that mouse location is stored in PointerInfo passed
        ''' to this method, it finds a Component that is in the same
        ''' Window as this Component and is located under the mouse pointer.
        ''' If no such Component exists, null is returned.
        ''' NOTE: this method should be called under the protection of
        ''' tree lock, as it is done in Component.getMousePosition() and
        ''' Container.getMousePosition(boolean).
        ''' </summary>
        Friend Overridable Function findUnderMouseInWindow(ByVal pi As PointerInfo) As Component
            If Not showing Then Return Nothing
            Dim win As Window = containingWindow
            If Not toolkit.defaultToolkit.mouseInfoPeer.isWindowUnderMouse(win) Then Return Nothing
            Const INCLUDE_DISABLED As Boolean = True
            Dim relativeToWindow As Point = win.pointRelativeToComponent(pi.location)
            Dim inTheSameWindow As Component = win.findComponentAt(relativeToWindow.x, relativeToWindow.y, INCLUDE_DISABLED)
            Return inTheSameWindow
        End Function

        ''' <summary>
        ''' Returns the position of the mouse pointer in this <code>Component</code>'s
        ''' coordinate space if the <code>Component</code> is directly under the mouse
        ''' pointer, otherwise returns <code>null</code>.
        ''' If the <code>Component</code> is not showing on the screen, this method
        ''' returns <code>null</code> even if the mouse pointer is above the area
        ''' where the <code>Component</code> would be displayed.
        ''' If the <code>Component</code> is partially or fully obscured by other
        ''' <code>Component</code>s or native windows, this method returns a non-null
        ''' value only if the mouse pointer is located above the unobscured part of the
        ''' <code>Component</code>.
        ''' <p>
        ''' For <code>Container</code>s it returns a non-null value if the mouse is
        ''' above the <code>Container</code> itself or above any of its descendants.
        ''' Use <seealso cref="Container#getMousePosition(boolean)"/> if you need to exclude children.
        ''' <p>
        ''' Sometimes the exact mouse coordinates are not important, and the only thing
        ''' that matters is whether a specific <code>Component</code> is under the mouse
        ''' pointer. If the return value of this method is <code>null</code>, mouse
        ''' pointer is not directly above the <code>Component</code>.
        ''' </summary>
        ''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless() returns true </exception>
        ''' <seealso cref=       #isShowing </seealso>
        ''' <seealso cref=       Container#getMousePosition </seealso>
        ''' <returns>    mouse coordinates relative to this <code>Component</code>, or null
        ''' @since     1.5 </returns>
        Public Overridable Property mousePosition As Point
            Get
                If GraphicsEnvironment.headless Then Throw New HeadlessException

                Dim pi As PointerInfo = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
                                                                            )

                SyncLock treeLock
                    Dim inTheSameWindow As Component = findUnderMouseInWindow(pi)
                    If Not isSameOrAncestorOf(inTheSameWindow, True) Then Return Nothing
                    Return pointRelativeToComponent(pi.location)
                End SyncLock
            End Get
        End Property

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As PointerInfo
                Return MouseInfo.pointerInfo
            End Function
        End Class

        ''' <summary>
        ''' Overridden in Container. Must be called under TreeLock.
        ''' </summary>
        Friend Overridable Function isSameOrAncestorOf(ByVal comp As Component, ByVal allowChildren As Boolean) As Boolean
            Return comp Is Me
        End Function

        ''' <summary>
        ''' Determines whether this component is showing on screen. This means
        ''' that the component must be visible, and it must be in a container
        ''' that is visible and showing.
        ''' <p>
        ''' <strong>Note:</strong> sometimes there is no way to detect whether the
        ''' {@code Component} is actually visible to the user.  This can happen when:
        ''' <ul>
        ''' <li>the component has been added to a visible {@code ScrollPane} but
        ''' the {@code Component} is not currently in the scroll pane's view port.
        ''' <li>the {@code Component} is obscured by another {@code Component} or
        ''' {@code Container}.
        ''' </ul> </summary>
        ''' <returns> <code>true</code> if the component is showing,
        '''          <code>false</code> otherwise </returns>
        ''' <seealso cref= #setVisible
        ''' @since JDK1.0 </seealso>
        Public Overridable Property showing As Boolean
            Get
                If visible AndAlso (peer IsNot Nothing) Then
                    Dim parent_Renamed As Container = Me.parent
                    Return (parent_Renamed Is Nothing) OrElse parent_Renamed.showing
                End If
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Determines whether this component is enabled. An enabled component
        ''' can respond to user input and generate events. Components are
        ''' enabled initially by default. A component may be enabled or disabled by
        ''' calling its <code>setEnabled</code> method. </summary>
        ''' <returns> <code>true</code> if the component is enabled,
        '''          <code>false</code> otherwise </returns>
        ''' <seealso cref= #setEnabled
        ''' @since JDK1.0 </seealso>
        Public Overridable Property enabled As Boolean
            Get
                Return enabledImpl
            End Get
            Set(ByVal b As Boolean)
                enable(b)
            End Set
        End Property

        '    
        '     * This is called by the native code, so client code can't
        '     * be called on the toolkit thread.
        '     
        Friend ReadOnly Property enabledImpl As Boolean
            Get
                Return enabled
            End Get
        End Property


        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setEnabled(boolean)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub enable()
            If Not enabled Then
                SyncLock treeLock
                    enabled = True
                    Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                    If peer_Renamed IsNot Nothing Then
                        peer_Renamed.enabled = True
                        If visible AndAlso (Not recursivelyVisibleBounds.empty) Then updateCursorImmediately()
                    End If
                End SyncLock
                If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.ENABLED)
            End If
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setEnabled(boolean)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub enable(ByVal b As Boolean)
            If b Then
                enable()
            Else
                disable()
            End If
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setEnabled(boolean)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub disable()
            If enabled Then
                KeyboardFocusManager.clearMostRecentFocusOwner(Me)
                SyncLock treeLock
                    enabled = False
                    ' A disabled lw container is allowed to contain a focus owner.
                    If (focusOwner OrElse (containsFocus() AndAlso (Not lightweight))) AndAlso KeyboardFocusManager.autoFocusTransferEnabled Then transferFocus(False)
                    Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                    If peer_Renamed IsNot Nothing Then
                        peer_Renamed.enabled = False
                        If visible AndAlso (Not recursivelyVisibleBounds.empty) Then updateCursorImmediately()
                    End If
                End SyncLock
                If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.ENABLED)
            End If
        End Sub

        ''' <summary>
        ''' Returns true if this component is painted to an offscreen image
        ''' ("buffer") that's copied to the screen later.  Component
        ''' subclasses that support double buffering should override this
        ''' method to return true if double buffering is enabled.
        ''' </summary>
        ''' <returns> false by default </returns>
        Public Overridable Property doubleBuffered As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Enables or disables input method support for this component. If input
        ''' method support is enabled and the component also processes key events,
        ''' incoming events are offered to
        ''' the current input method and will only be processed by the component or
        ''' dispatched to its listeners if the input method does not consume them.
        ''' By default, input method support is enabled.
        ''' </summary>
        ''' <param name="enable"> true to enable, false to disable </param>
        ''' <seealso cref= #processKeyEvent
        ''' @since 1.2 </seealso>
        Public Overridable Sub enableInputMethods(ByVal enable As Boolean)
            If enable Then
                If (eventMask And AWTEvent.INPUT_METHODS_ENABLED_MASK) <> 0 Then Return

                ' If this component already has focus, then activate the
                ' input method by dispatching a synthesized focus gained
                ' event.
                If focusOwner Then
                    Dim inputContext_Renamed As java.awt.im.InputContext = inputContext
                    If inputContext_Renamed IsNot Nothing Then
                        Dim focusGainedEvent As New FocusEvent(Me, FocusEvent.FOCUS_GAINED)
                        inputContext_Renamed.dispatchEvent(focusGainedEvent)
                    End If
                End If

                eventMask = eventMask Or AWTEvent.INPUT_METHODS_ENABLED_MASK
            Else
                If (eventMask And AWTEvent.INPUT_METHODS_ENABLED_MASK) <> 0 Then
                    Dim inputContext_Renamed As java.awt.im.InputContext = inputContext
                    If inputContext_Renamed IsNot Nothing Then
                        inputContext_Renamed.endComposition()
                        inputContext_Renamed.removeNotify(Me)
                    End If
                End If
                eventMask = eventMask And Not AWTEvent.INPUT_METHODS_ENABLED_MASK
            End If
        End Sub

        ''' <summary>
        ''' Shows or hides this component depending on the value of parameter
        ''' <code>b</code>.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' </summary>
        ''' <param name="b">  if <code>true</code>, shows this component;
        ''' otherwise, hides this component </param>
        ''' <seealso cref= #isVisible </seealso>
        ''' <seealso cref= #invalidate
        ''' @since JDK1.1 </seealso>
        Public Overridable Property visible As Boolean
            Set(ByVal b As Boolean)
                show(b)
            End Set
        End Property

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setVisible(boolean)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub show()
            If Not visible Then
                SyncLock treeLock
                    visible = True
                    mixOnShowing()
                    Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                    If peer_Renamed IsNot Nothing Then
                        peer_Renamed.visible = True
                        createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, Me, parent, HierarchyEvent.SHOWING_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
                        If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then repaint()
                        updateCursorImmediately()
                    End If

                    If componentListener IsNot Nothing OrElse (eventMask And AWTEvent.COMPONENT_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK) Then
                        Dim e As New ComponentEvent(Me, ComponentEvent.COMPONENT_SHOWN)
                        toolkit.eventQueue.postEvent(e)
                    End If
                End SyncLock
                Dim parent_Renamed As Container = Me.parent
                If parent_Renamed IsNot Nothing Then parent_Renamed.invalidate()
            End If
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setVisible(boolean)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub show(ByVal b As Boolean)
            If b Then
                show()
            Else
                hide()
            End If
        End Sub

        Friend Overridable Function containsFocus() As Boolean
            Return focusOwner
        End Function

        Friend Overridable Sub clearMostRecentFocusOwnerOnHide()
            KeyboardFocusManager.clearMostRecentFocusOwner(Me)
        End Sub

        Friend Overridable Sub clearCurrentFocusCycleRootOnHide()
            ' do nothing 
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setVisible(boolean)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub hide()
            isPacked = False

            If visible Then
                clearCurrentFocusCycleRootOnHide()
                clearMostRecentFocusOwnerOnHide()
                SyncLock treeLock
                    visible = False
                    mixOnHiding(lightweight)
                    If containsFocus() AndAlso KeyboardFocusManager.autoFocusTransferEnabled Then transferFocus(True)
                    Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                    If peer_Renamed IsNot Nothing Then
                        peer_Renamed.visible = False
                        createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, Me, parent, HierarchyEvent.SHOWING_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
                        If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then repaint()
                        updateCursorImmediately()
                    End If
                    If componentListener IsNot Nothing OrElse (eventMask And AWTEvent.COMPONENT_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK) Then
                        Dim e As New ComponentEvent(Me, ComponentEvent.COMPONENT_HIDDEN)
                        toolkit.eventQueue.postEvent(e)
                    End If
                End SyncLock
                Dim parent_Renamed As Container = Me.parent
                If parent_Renamed IsNot Nothing Then parent_Renamed.invalidate()
            End If
        End Sub

        ''' <summary>
        ''' Gets the foreground color of this component. </summary>
        ''' <returns> this component's foreground color; if this component does
        ''' not have a foreground color, the foreground color of its parent
        ''' is returned </returns>
        ''' <seealso cref= #setForeground
        ''' @since JDK1.0
        ''' @beaninfo
        '''       bound: true </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overridable Property foreground As color
            Get
                Dim foreground_Renamed As color = Me.foreground
                If foreground_Renamed IsNot Nothing Then Return foreground_Renamed
                Dim parent_Renamed As Container = Me.parent
                Return If(parent_Renamed IsNot Nothing, parent_Renamed.foreground, Nothing)
            End Get
            Set(ByVal c As color)
                Dim oldColor As color = foreground
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                foreground = c
                If peer_Renamed IsNot Nothing Then
                    c = foreground
                    If c IsNot Nothing Then peer_Renamed.foreground = c
                End If
                ' This is a bound property, so report the change to
                ' any registered listeners.  (Cheap if there are none.)
                firePropertyChange("foreground", oldColor, c)
            End Set
        End Property


        ''' <summary>
        ''' Returns whether the foreground color has been explicitly set for this
        ''' Component. If this method returns <code>false</code>, this Component is
        ''' inheriting its foreground color from an ancestor.
        ''' </summary>
        ''' <returns> <code>true</code> if the foreground color has been explicitly
        '''         set for this Component; <code>false</code> otherwise.
        ''' @since 1.4 </returns>
        Public Overridable Property foregroundSet As Boolean
            Get
                Return (foreground IsNot Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Gets the background color of this component. </summary>
        ''' <returns> this component's background color; if this component does
        '''          not have a background color,
        '''          the background color of its parent is returned </returns>
        ''' <seealso cref= #setBackground
        ''' @since JDK1.0 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overridable Property background As color
            Get
                Dim background_Renamed As color = Me.background
                If background_Renamed IsNot Nothing Then Return background_Renamed
                Dim parent_Renamed As Container = Me.parent
                Return If(parent_Renamed IsNot Nothing, parent_Renamed.background, Nothing)
            End Get
            Set(ByVal c As color)
                Dim oldColor As color = background
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                background = c
                If peer_Renamed IsNot Nothing Then
                    c = background
                    If c IsNot Nothing Then peer_Renamed.background = c
                End If
                ' This is a bound property, so report the change to
                ' any registered listeners.  (Cheap if there are none.)
                firePropertyChange("background", oldColor, c)
            End Set
        End Property


        ''' <summary>
        ''' Returns whether the background color has been explicitly set for this
        ''' Component. If this method returns <code>false</code>, this Component is
        ''' inheriting its background color from an ancestor.
        ''' </summary>
        ''' <returns> <code>true</code> if the background color has been explicitly
        '''         set for this Component; <code>false</code> otherwise.
        ''' @since 1.4 </returns>
        Public Overridable Property backgroundSet As Boolean
            Get
                Return (background IsNot Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Gets the font of this component. </summary>
        ''' <returns> this component's font; if a font has not been set
        ''' for this component, the font of its parent is returned </returns>
        ''' <seealso cref= #setFont
        ''' @since JDK1.0 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overridable Property font As font Implements MenuContainer.getFont
            Get
                Return font_NoClientCode
            End Get
            Set(ByVal f As font)
                Dim oldFont, newFont As font
                SyncLock treeLock
                    oldFont = font
                    font = f
                    newFont = font
                    Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                    If peer_Renamed IsNot Nothing Then
                        f = font
                        If f IsNot Nothing Then
                            peer_Renamed.font = f
                            peerFont = f
                        End If
                    End If
                End SyncLock
                ' This is a bound property, so report the change to
                ' any registered listeners.  (Cheap if there are none.)
                firePropertyChange("font", oldFont, newFont)

                ' This could change the preferred size of the Component.
                ' Fix for 6213660. Should compare old and new fonts and do not
                ' call invalidate() if they are equal.
                If f IsNot oldFont AndAlso (oldFont Is Nothing OrElse (Not oldFont.Equals(f))) Then invalidateIfValid()
            End Set
        End Property

        ' NOTE: This method may be called by privileged threads.
        '       This functionality is implemented in a package-private method
        '       to insure that it cannot be overridden by client subclasses.
        '       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
        Friend Property font_NoClientCode As font
            Get
                Dim font_Renamed As font = Me.font
                If font_Renamed IsNot Nothing Then Return font_Renamed
                Dim parent_Renamed As Container = Me.parent
                Return If(parent_Renamed IsNot Nothing, parent_Renamed.font_NoClientCode, Nothing)
            End Get
        End Property


        ''' <summary>
        ''' Returns whether the font has been explicitly set for this Component. If
        ''' this method returns <code>false</code>, this Component is inheriting its
        ''' font from an ancestor.
        ''' </summary>
        ''' <returns> <code>true</code> if the font has been explicitly set for this
        '''         Component; <code>false</code> otherwise.
        ''' @since 1.4 </returns>
        Public Overridable Property fontSet As Boolean
            Get
                Return (font IsNot Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Gets the locale of this component. </summary>
        ''' <returns> this component's locale; if this component does not
        '''          have a locale, the locale of its parent is returned </returns>
        ''' <seealso cref= #setLocale </seealso>
        ''' <exception cref="IllegalComponentStateException"> if the <code>Component</code>
        '''          does not have its own locale and has not yet been added to
        '''          a containment hierarchy such that the locale can be determined
        '''          from the containing parent
        ''' @since  JDK1.1 </exception>
        Public Overridable Property locale As java.util.Locale
            Get
                Dim locale_Renamed As java.util.Locale = Me.locale
                If locale_Renamed IsNot Nothing Then Return locale_Renamed
                Dim parent_Renamed As Container = Me.parent

                If parent_Renamed Is Nothing Then
                    Throw New IllegalComponentStateException("This component must have a parent in order to determine its locale")
                Else
                    Return parent_Renamed.locale
                End If
            End Get
            Set(ByVal l As java.util.Locale)
                Dim oldValue As java.util.Locale = locale
                locale = l

                ' This is a bound property, so report the change to
                ' any registered listeners.  (Cheap if there are none.)
                firePropertyChange("locale", oldValue, l)

                ' This could change the preferred size of the Component.
                invalidateIfValid()
            End Set
        End Property


        ''' <summary>
        ''' Gets the instance of <code>ColorModel</code> used to display
        ''' the component on the output device. </summary>
        ''' <returns> the color model used by this component </returns>
        ''' <seealso cref= java.awt.image.ColorModel </seealso>
        ''' <seealso cref= java.awt.peer.ComponentPeer#getColorModel() </seealso>
        ''' <seealso cref= Toolkit#getColorModel()
        ''' @since JDK1.0 </seealso>
        Public Overridable Property colorModel As java.awt.image.ColorModel
            Get
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                If (peer_Renamed IsNot Nothing) AndAlso Not (TypeOf peer_Renamed Is java.awt.peer.LightweightPeer) Then
                    Return peer_Renamed.colorModel
                ElseIf GraphicsEnvironment.headless Then
                    Return java.awt.image.ColorModel.rGBdefault
                End If ' else
                Return toolkit.colorModel
            End Get
        End Property

        ''' <summary>
        ''' Gets the location of this component in the form of a
        ''' point specifying the component's top-left corner.
        ''' The location will be relative to the parent's coordinate space.
        ''' <p>
        ''' Due to the asynchronous nature of native event handling, this
        ''' method can return outdated values (for instance, after several calls
        ''' of <code>setLocation()</code> in rapid succession).  For this
        ''' reason, the recommended method of obtaining a component's position is
        ''' within <code>java.awt.event.ComponentListener.componentMoved()</code>,
        ''' which is called after the operating system has finished moving the
        ''' component.
        ''' </p> </summary>
        ''' <returns> an instance of <code>Point</code> representing
        '''          the top-left corner of the component's bounds in
        '''          the coordinate space of the component's parent </returns>
        ''' <seealso cref= #setLocation </seealso>
        ''' <seealso cref= #getLocationOnScreen
        ''' @since JDK1.1 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getLocation() As Point 'JavaToDotNetTempPropertyGetlocation
        Public Overridable Property location As Point
            Get
                Return location()
            End Get
            Set(ByVal p As Point)
        End Property

        ''' <summary>
        ''' Gets the location of this component in the form of a point
        ''' specifying the component's top-left corner in the screen's
        ''' coordinate space. </summary>
        ''' <returns> an instance of <code>Point</code> representing
        '''          the top-left corner of the component's bounds in the
        '''          coordinate space of the screen </returns>
        ''' <exception cref="IllegalComponentStateException"> if the
        '''          component is not showing on the screen </exception>
        ''' <seealso cref= #setLocation </seealso>
        ''' <seealso cref= #getLocation </seealso>
        Public Overridable Property locationOnScreen As Point
            Get
                SyncLock treeLock
                    Return locationOnScreen_NoTreeLock
                End SyncLock
            End Get
        End Property

        '    
        '     * a package private version of getLocationOnScreen
        '     * used by GlobalCursormanager to update cursor
        '     
        Friend Property locationOnScreen_NoTreeLock As Point
            Get

                If peer IsNot Nothing AndAlso showing Then
                    If TypeOf peer Is java.awt.peer.LightweightPeer Then
                        ' lightweight component location needs to be translated
                        ' relative to a native component.
                        Dim host As Container = nativeContainer
                        Dim pt As Point = host.peer.locationOnScreen
                        Dim c As Component = Me
                        Do While c IsNot host
                            pt.x += c.x
                            pt.y += c.y
                            c = c.parent
                        Loop
                        Return pt
                    Else
                        Dim pt As Point = peer.locationOnScreen
                        Return pt
                    End If
                Else
                    Throw New IllegalComponentStateException("component must be showing on the screen to determine its location")
                End If
            End Get
        End Property


        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>getLocation()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function location() As Point
            Return location_NoClientCode()
        End Function

        Private Function location_NoClientCode() As Point
            Return New Point(x, y)
        End Function

        ''' <summary>
        ''' Moves this component to a new location. The top-left corner of
        ''' the new location is specified by the <code>x</code> and <code>y</code>
        ''' parameters in the coordinate space of this component's parent.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' </summary>
        ''' <param name="x"> the <i>x</i>-coordinate of the new location's
        '''          top-left corner in the parent's coordinate space </param>
        ''' <param name="y"> the <i>y</i>-coordinate of the new location's
        '''          top-left corner in the parent's coordinate space </param>
        ''' <seealso cref= #getLocation </seealso>
        ''' <seealso cref= #setBounds </seealso>
        ''' <seealso cref= #invalidate
        ''' @since JDK1.1 </seealso>
        Public Overridable Sub setLocation(ByVal x As Integer, ByVal y As Integer)
            move(x, y)
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setLocation(int, int)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub move(ByVal x As Integer, ByVal y As Integer)
            SyncLock treeLock
                boundsOp = java.awt.peer.ComponentPeer.SET_LOCATION
                boundsnds(x, y, width, height)
            End SyncLock
        End Sub

			locationion(p.x, p.y)
		End Sub

        ''' <summary>
        ''' Returns the size of this component in the form of a
        ''' <code>Dimension</code> object. The <code>height</code>
        ''' field of the <code>Dimension</code> object contains
        ''' this component's height, and the <code>width</code>
        ''' field of the <code>Dimension</code> object contains
        ''' this component's width. </summary>
        ''' <returns> a <code>Dimension</code> object that indicates the
        '''          size of this component </returns>
        ''' <seealso cref= #setSize
        ''' @since JDK1.1 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getSize() As Dimension 'JavaToDotNetTempPropertyGetsize
        Public Overridable Property size As Dimension
            Get
                Return size()
            End Get
            Set(ByVal d As Dimension)
        End Property

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>getSize()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function size() As Dimension
            Return New Dimension(width, height)
        End Function

        ''' <summary>
        ''' Resizes this component so that it has width <code>width</code>
        ''' and height <code>height</code>.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' </summary>
        ''' <param name="width"> the new width of this component in pixels </param>
        ''' <param name="height"> the new height of this component in pixels </param>
        ''' <seealso cref= #getSize </seealso>
        ''' <seealso cref= #setBounds </seealso>
        ''' <seealso cref= #invalidate
        ''' @since JDK1.1 </seealso>
        Public Overridable Sub setSize(ByVal width As Integer, ByVal height As Integer)
            resize(width, height)
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setSize(int, int)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub resize(ByVal width As Integer, ByVal height As Integer)
            SyncLock treeLock
                boundsOp = java.awt.peer.ComponentPeer.SET_SIZE
                boundsnds(x, y, width, height)
            End SyncLock
        End Sub

			resize(d)
		End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setSize(Dimension)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub resize(ByVal d As Dimension)
            sizeize(d.width, d.height)
        End Sub

        ''' <summary>
        ''' Gets the bounds of this component in the form of a
        ''' <code>Rectangle</code> object. The bounds specify this
        ''' component's width, height, and location relative to
        ''' its parent. </summary>
        ''' <returns> a rectangle indicating this component's bounds </returns>
        ''' <seealso cref= #setBounds </seealso>
        ''' <seealso cref= #getLocation </seealso>
        ''' <seealso cref= #getSize </seealso>
        Public Overridable Property bounds As Rectangle
            Get
                Return bounds()
            End Get
            Set(ByVal r As Rectangle)
                boundsnds(r.x, r.y, r.width, r.height)
            End Set
        End Property

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>getBounds()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function bounds() As Rectangle
            Return New Rectangle(x, y, width, height)
        End Function

        ''' <summary>
        ''' Moves and resizes this component. The new location of the top-left
        ''' corner is specified by <code>x</code> and <code>y</code>, and the
        ''' new size is specified by <code>width</code> and <code>height</code>.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' </summary>
        ''' <param name="x"> the new <i>x</i>-coordinate of this component </param>
        ''' <param name="y"> the new <i>y</i>-coordinate of this component </param>
        ''' <param name="width"> the new <code>width</code> of this component </param>
        ''' <param name="height"> the new <code>height</code> of this
        '''          component </param>
        ''' <seealso cref= #getBounds </seealso>
        ''' <seealso cref= #setLocation(int, int) </seealso>
        ''' <seealso cref= #setLocation(Point) </seealso>
        ''' <seealso cref= #setSize(int, int) </seealso>
        ''' <seealso cref= #setSize(Dimension) </seealso>
        ''' <seealso cref= #invalidate
        ''' @since JDK1.1 </seealso>
        Public Overridable Sub setBounds(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
            reshape(x, y, width, height)
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>setBounds(int, int, int, int)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub reshape(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
            SyncLock treeLock
                Try
                    boundsOp = java.awt.peer.ComponentPeer.SET_BOUNDS
                    Dim resized As Boolean = (Me.width <> width) OrElse (Me.height <> height)
                    Dim moved As Boolean = (Me.x <> x) OrElse (Me.y <> y)
                    If (Not resized) AndAlso (Not moved) Then Return
                    Dim oldX As Integer = Me.x
                    Dim oldY As Integer = Me.y
                    Dim oldWidth As Integer = Me.width
                    Dim oldHeight As Integer = Me.height
                    Me.x = x
                    Me.y = y
                    Me.width = width
                    Me.height = height

                    If resized Then isPacked = False

                    Dim needNotify As Boolean = True
                    mixOnReshaping()
                    If peer IsNot Nothing Then
                        ' LightwightPeer is an empty stub so can skip peer.reshape
                        If Not (TypeOf peer Is java.awt.peer.LightweightPeer) Then
                            reshapeNativePeer(x, y, width, height, boundsOp)
                            ' Check peer actualy changed coordinates
                            resized = (oldWidth <> Me.width) OrElse (oldHeight <> Me.height)
                            moved = (oldX <> Me.x) OrElse (oldY <> Me.y)
                            ' fix for 5025858: do not send ComponentEvents for toplevel
                            ' windows here as it is done from peer or native code when
                            ' the window is really resized or moved, otherwise some
                            ' events may be sent twice
                            If TypeOf Me Is Window Then needNotify = False
                        End If
                        If resized Then invalidate()
                        If parent IsNot Nothing Then parent.invalidateIfValid()
                    End If
                    If needNotify Then notifyNewBounds(resized, moved)
                    repaintParentIfNeeded(oldX, oldY, oldWidth, oldHeight)
                Finally
                    boundsOp = java.awt.peer.ComponentPeer.RESET_OPERATION
                End Try
            End SyncLock
        End Sub

        Private Sub repaintParentIfNeeded(ByVal oldX As Integer, ByVal oldY As Integer, ByVal oldWidth As Integer, ByVal oldHeight As Integer)
            If parent IsNot Nothing AndAlso TypeOf peer Is java.awt.peer.LightweightPeer AndAlso showing Then
                ' Have the parent redraw the area this component occupied.
                parent.repaint(oldX, oldY, oldWidth, oldHeight)
                ' Have the parent redraw the area this component *now* occupies.
                repaint()
            End If
        End Sub

        Private Sub reshapeNativePeer(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal op As Integer)
            ' native peer might be offset by more than direct
            ' parent since parent might be lightweight.
            Dim nativeX As Integer = x
            Dim nativeY As Integer = y
            Dim c As Component = parent
            Do While (c IsNot Nothing) AndAlso (TypeOf c.peer Is java.awt.peer.LightweightPeer)
                nativeX += c.x
                nativeY += c.y
                c = c.parent
            Loop
            peer.boundsnds(nativeX, nativeY, width, height, op)
        End Sub

        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Private Sub notifyNewBounds(ByVal resized As Boolean, ByVal moved As Boolean)
            If componentListener IsNot Nothing OrElse (eventMask And AWTEvent.COMPONENT_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK) Then
                If resized Then
                    Dim e As New ComponentEvent(Me, ComponentEvent.COMPONENT_RESIZED)
                    toolkit.eventQueue.postEvent(e)
                End If
                If moved Then
                    Dim e As New ComponentEvent(Me, ComponentEvent.COMPONENT_MOVED)
                    toolkit.eventQueue.postEvent(e)
                End If
            Else
                If TypeOf Me Is Container AndAlso CType(Me, Container).countComponents() > 0 Then
                    Dim enabledOnToolkit As Boolean = Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK)
                    If resized Then CType(Me, Container).createChildHierarchyEvents(HierarchyEvent.ANCESTOR_RESIZED, 0, enabledOnToolkit)
                    If moved Then CType(Me, Container).createChildHierarchyEvents(HierarchyEvent.ANCESTOR_MOVED, 0, enabledOnToolkit)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Returns the current x coordinate of the components origin.
        ''' This method is preferable to writing
        ''' <code>component.getBounds().x</code>,
        ''' or <code>component.getLocation().x</code> because it doesn't
        ''' cause any heap allocations.
        ''' </summary>
        ''' <returns> the current x coordinate of the components origin
        ''' @since 1.2 </returns>
        Public Overridable Property x As Integer
            Get
                Return x
            End Get
        End Property


        ''' <summary>
        ''' Returns the current y coordinate of the components origin.
        ''' This method is preferable to writing
        ''' <code>component.getBounds().y</code>,
        ''' or <code>component.getLocation().y</code> because it
        ''' doesn't cause any heap allocations.
        ''' </summary>
        ''' <returns> the current y coordinate of the components origin
        ''' @since 1.2 </returns>
        Public Overridable Property y As Integer
            Get
                Return y
            End Get
        End Property


        ''' <summary>
        ''' Returns the current width of this component.
        ''' This method is preferable to writing
        ''' <code>component.getBounds().width</code>,
        ''' or <code>component.getSize().width</code> because it
        ''' doesn't cause any heap allocations.
        ''' </summary>
        ''' <returns> the current width of this component
        ''' @since 1.2 </returns>
        Public Overridable Property width As Integer
            Get
                Return width
            End Get
        End Property


        ''' <summary>
        ''' Returns the current height of this component.
        ''' This method is preferable to writing
        ''' <code>component.getBounds().height</code>,
        ''' or <code>component.getSize().height</code> because it
        ''' doesn't cause any heap allocations.
        ''' </summary>
        ''' <returns> the current height of this component
        ''' @since 1.2 </returns>
        Public Overridable Property height As Integer
            Get
                Return height
            End Get
        End Property

        ''' <summary>
        ''' Stores the bounds of this component into "return value" <b>rv</b> and
        ''' return <b>rv</b>.  If rv is <code>null</code> a new
        ''' <code>Rectangle</code> is allocated.
        ''' This version of <code>getBounds</code> is useful if the caller
        ''' wants to avoid allocating a new <code>Rectangle</code> object
        ''' on the heap.
        ''' </summary>
        ''' <param name="rv"> the return value, modified to the components bounds </param>
        ''' <returns> rv </returns>
        Public Overridable Function getBounds(ByVal rv As Rectangle) As Rectangle
            If rv Is Nothing Then
                Return New Rectangle(x, y, width, height)
            Else
                rv.boundsnds(x, y, width, height)
                Return rv
            End If
        End Function

        ''' <summary>
        ''' Stores the width/height of this component into "return value" <b>rv</b>
        ''' and return <b>rv</b>.   If rv is <code>null</code> a new
        ''' <code>Dimension</code> object is allocated.  This version of
        ''' <code>getSize</code> is useful if the caller wants to avoid
        ''' allocating a new <code>Dimension</code> object on the heap.
        ''' </summary>
        ''' <param name="rv"> the return value, modified to the components size </param>
        ''' <returns> rv </returns>
        Public Overridable Function getSize(ByVal rv As Dimension) As Dimension
            If rv Is Nothing Then
                Return New Dimension(width, height)
            Else
                rv.sizeize(width, height)
                Return rv
            End If
        End Function

        ''' <summary>
        ''' Stores the x,y origin of this component into "return value" <b>rv</b>
        ''' and return <b>rv</b>.   If rv is <code>null</code> a new
        ''' <code>Point</code> is allocated.
        ''' This version of <code>getLocation</code> is useful if the
        ''' caller wants to avoid allocating a new <code>Point</code>
        ''' object on the heap.
        ''' </summary>
        ''' <param name="rv"> the return value, modified to the components location </param>
        ''' <returns> rv </returns>
        Public Overridable Function getLocation(ByVal rv As Point) As Point
            If rv Is Nothing Then
                Return New Point(x, y)
            Else
                rv.locationion(x, y)
                Return rv
            End If
        End Function

        ''' <summary>
        ''' Returns true if this component is completely opaque, returns
        ''' false by default.
        ''' <p>
        ''' An opaque component paints every pixel within its
        ''' rectangular region. A non-opaque component paints only some of
        ''' its pixels, allowing the pixels underneath it to "show through".
        ''' A component that does not fully paint its pixels therefore
        ''' provides a degree of transparency.
        ''' <p>
        ''' Subclasses that guarantee to always completely paint their
        ''' contents should override this method and return true.
        ''' </summary>
        ''' <returns> true if this component is completely opaque </returns>
        ''' <seealso cref= #isLightweight
        ''' @since 1.2 </seealso>
        Public Overridable Property opaque As Boolean
            Get
                If peer Is Nothing Then
                    Return False
                Else
                    Return Not lightweight
                End If
            End Get
        End Property


        ''' <summary>
        ''' A lightweight component doesn't have a native toolkit peer.
        ''' Subclasses of <code>Component</code> and <code>Container</code>,
        ''' other than the ones defined in this package like <code>Button</code>
        ''' or <code>Scrollbar</code>, are lightweight.
        ''' All of the Swing components are lightweights.
        ''' <p>
        ''' This method will always return <code>false</code> if this component
        ''' is not displayable because it is impossible to determine the
        ''' weight of an undisplayable component.
        ''' </summary>
        ''' <returns> true if this component has a lightweight peer; false if
        '''         it has a native peer or no peer </returns>
        ''' <seealso cref= #isDisplayable
        ''' @since 1.2 </seealso>
        Public Overridable Property lightweight As Boolean
            Get
                Return TypeOf peer Is java.awt.peer.LightweightPeer
            End Get
        End Property


        ''' <summary>
        ''' Sets the preferred size of this component to a constant
        ''' value.  Subsequent calls to <code>getPreferredSize</code> will always
        ''' return this value.  Setting the preferred size to <code>null</code>
        ''' restores the default behavior.
        ''' </summary>
        ''' <param name="preferredSize"> The new preferred size, or null </param>
        ''' <seealso cref= #getPreferredSize </seealso>
        ''' <seealso cref= #isPreferredSizeSet
        ''' @since 1.5 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setPreferredSize(ByVal preferredSize As Dimension) 'JavaToDotNetTempPropertySetpreferredSize
        Public Overridable Property preferredSize As Dimension
            Set(ByVal preferredSize As Dimension)
                Dim old As Dimension
                ' If the preferred size was set, use it as the old value, otherwise
                ' use null to indicate we didn't previously have a set preferred
                ' size.
                If prefSizeSet Then
                    old = Me.prefSize
                Else
                    old = Nothing
                End If
                Me.prefSize = preferredSize
                prefSizeSet = (preferredSize IsNot Nothing)
                firePropertyChange("preferredSize", old, preferredSize)
            End Set
            Get
        End Property


        ''' <summary>
        ''' Returns true if the preferred size has been set to a
        ''' non-<code>null</code> value otherwise returns false.
        ''' </summary>
        ''' <returns> true if <code>setPreferredSize</code> has been invoked
        '''         with a non-null value.
        ''' @since 1.5 </returns>
        Public Overridable Property preferredSizeSet As Boolean
            Get
                Return prefSizeSet
            End Get
        End Property


        Return preferredSize()
        End Function


        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>getPreferredSize()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function preferredSize() As Dimension
            '         Avoid grabbing the lock if a reasonable cached size value
            '         * is available.
            '         
            Dim [dim] As Dimension = prefSize
            If [dim] Is Nothing OrElse Not (preferredSizeSet OrElse valid) Then
                SyncLock treeLock
                    prefSize = If(peer IsNot Nothing, peer.preferredSize, minimumSize)
                    [dim] = prefSize
                End SyncLock
            End If
            Return New Dimension([dim])
        End Function

        ''' <summary>
        ''' Sets the minimum size of this component to a constant
        ''' value.  Subsequent calls to <code>getMinimumSize</code> will always
        ''' return this value.  Setting the minimum size to <code>null</code>
        ''' restores the default behavior.
        ''' </summary>
        ''' <param name="minimumSize"> the new minimum size of this component </param>
        ''' <seealso cref= #getMinimumSize </seealso>
        ''' <seealso cref= #isMinimumSizeSet
        ''' @since 1.5 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setMinimumSize(ByVal minimumSize As Dimension) 'JavaToDotNetTempPropertySetminimumSize
        Public Overridable Property minimumSize As Dimension
            Set(ByVal minimumSize As Dimension)
                Dim old As Dimension
                ' If the minimum size was set, use it as the old value, otherwise
                ' use null to indicate we didn't previously have a set minimum
                ' size.
                If minSizeSet Then
                    old = Me.minSize
                Else
                    old = Nothing
                End If
                Me.minSize = minimumSize
                minSizeSet = (minimumSize IsNot Nothing)
                firePropertyChange("minimumSize", old, minimumSize)
            End Set
            Get
        End Property

        ''' <summary>
        ''' Returns whether or not <code>setMinimumSize</code> has been
        ''' invoked with a non-null value.
        ''' </summary>
        ''' <returns> true if <code>setMinimumSize</code> has been invoked with a
        '''              non-null value.
        ''' @since 1.5 </returns>
        Public Overridable Property minimumSizeSet As Boolean
            Get
                Return minSizeSet
            End Get
        End Property

        Return minimumSize()
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>getMinimumSize()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function minimumSize() As Dimension
            '         Avoid grabbing the lock if a reasonable cached size value
            '         * is available.
            '         
            Dim [dim] As Dimension = minSize
            If [dim] Is Nothing OrElse Not (minimumSizeSet OrElse valid) Then
                SyncLock treeLock
                    minSize = If(peer IsNot Nothing, peer.minimumSize, size())
                    [dim] = minSize
                End SyncLock
            End If
            Return New Dimension([dim])
        End Function

        ''' <summary>
        ''' Sets the maximum size of this component to a constant
        ''' value.  Subsequent calls to <code>getMaximumSize</code> will always
        ''' return this value.  Setting the maximum size to <code>null</code>
        ''' restores the default behavior.
        ''' </summary>
        ''' <param name="maximumSize"> a <code>Dimension</code> containing the
        '''          desired maximum allowable size </param>
        ''' <seealso cref= #getMaximumSize </seealso>
        ''' <seealso cref= #isMaximumSizeSet
        ''' @since 1.5 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setMaximumSize(ByVal maximumSize As Dimension) 'JavaToDotNetTempPropertySetmaximumSize
        Public Overridable Property maximumSize As Dimension
            Set(ByVal maximumSize As Dimension)
                ' If the maximum size was set, use it as the old value, otherwise
                ' use null to indicate we didn't previously have a set maximum
                ' size.
                Dim old As Dimension
                If maxSizeSet Then
                    old = Me.maxSize
                Else
                    old = Nothing
                End If
                Me.maxSize = maximumSize
                maxSizeSet = (maximumSize IsNot Nothing)
                firePropertyChange("maximumSize", old, maximumSize)
            End Set
            Get
        End Property

        ''' <summary>
        ''' Returns true if the maximum size has been set to a non-<code>null</code>
        ''' value otherwise returns false.
        ''' </summary>
        ''' <returns> true if <code>maximumSize</code> is non-<code>null</code>,
        '''          false otherwise
        ''' @since 1.5 </returns>
        Public Overridable Property maximumSizeSet As Boolean
            Get
                Return maxSizeSet
            End Get
        End Property

        If maximumSizeSet Then Return New Dimension(maxSize)
			Return New Dimension(java.lang.[Short].Max_Value, java.lang.[Short].Max_Value)
        End Function

        ''' <summary>
        ''' Returns the alignment along the x axis.  This specifies how
        ''' the component would like to be aligned relative to other
        ''' components.  The value should be a number between 0 and 1
        ''' where 0 represents alignment along the origin, 1 is aligned
        ''' the furthest away from the origin, 0.5 is centered, etc.
        ''' </summary>
        Public Overridable Property alignmentX As Single
            Get
                Return CENTER_ALIGNMENT
            End Get
        End Property

        ''' <summary>
        ''' Returns the alignment along the y axis.  This specifies how
        ''' the component would like to be aligned relative to other
        ''' components.  The value should be a number between 0 and 1
        ''' where 0 represents alignment along the origin, 1 is aligned
        ''' the furthest away from the origin, 0.5 is centered, etc.
        ''' </summary>
        Public Overridable Property alignmentY As Single
            Get
                Return CENTER_ALIGNMENT
            End Get
        End Property

        ''' <summary>
        ''' Returns the baseline.  The baseline is measured from the top of
        ''' the component.  This method is primarily meant for
        ''' <code>LayoutManager</code>s to align components along their
        ''' baseline.  A return value less than 0 indicates this component
        ''' does not have a reasonable baseline and that
        ''' <code>LayoutManager</code>s should not align this component on
        ''' its baseline.
        ''' <p>
        ''' The default implementation returns -1.  Subclasses that support
        ''' baseline should override appropriately.  If a value &gt;= 0 is
        ''' returned, then the component has a valid baseline for any
        ''' size &gt;= the minimum size and <code>getBaselineResizeBehavior</code>
        ''' can be used to determine how the baseline changes with size.
        ''' </summary>
        ''' <param name="width"> the width to get the baseline for </param>
        ''' <param name="height"> the height to get the baseline for </param>
        ''' <returns> the baseline or &lt; 0 indicating there is no reasonable
        '''         baseline </returns>
        ''' <exception cref="IllegalArgumentException"> if width or height is &lt; 0 </exception>
        ''' <seealso cref= #getBaselineResizeBehavior </seealso>
        ''' <seealso cref= java.awt.FontMetrics
        ''' @since 1.6 </seealso>
        Public Overridable Function getBaseline(ByVal width As Integer, ByVal height As Integer) As Integer
            If width < 0 OrElse height < 0 Then Throw New IllegalArgumentException("Width and height must be >= 0")
            Return -1
        End Function

        ''' <summary>
        ''' Returns an enum indicating how the baseline of the component
        ''' changes as the size changes.  This method is primarily meant for
        ''' layout managers and GUI builders.
        ''' <p>
        ''' The default implementation returns
        ''' <code>BaselineResizeBehavior.OTHER</code>.  Subclasses that have a
        ''' baseline should override appropriately.  Subclasses should
        ''' never return <code>null</code>; if the baseline can not be
        ''' calculated return <code>BaselineResizeBehavior.OTHER</code>.  Callers
        ''' should first ask for the baseline using
        ''' <code>getBaseline</code> and if a value &gt;= 0 is returned use
        ''' this method.  It is acceptable for this method to return a
        ''' value other than <code>BaselineResizeBehavior.OTHER</code> even if
        ''' <code>getBaseline</code> returns a value less than 0.
        ''' </summary>
        ''' <returns> an enum indicating how the baseline changes as the component
        '''         size changes </returns>
        ''' <seealso cref= #getBaseline(int, int)
        ''' @since 1.6 </seealso>
        Public Overridable Property baselineResizeBehavior As BaselineResizeBehavior
            Get
                Return BaselineResizeBehavior.OTHER
            End Get
        End Property

        ''' <summary>
        ''' Prompts the layout manager to lay out this component. This is
        ''' usually called when the component (more specifically, container)
        ''' is validated. </summary>
        ''' <seealso cref= #validate </seealso>
        ''' <seealso cref= LayoutManager </seealso>
        Public Overridable Sub doLayout()
            layout()
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>doLayout()</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub layout()
        End Sub

        ''' <summary>
        ''' Validates this component.
        ''' <p>
        ''' The meaning of the term <i>validating</i> is defined by the ancestors of
        ''' this class. See <seealso cref="Container#validate"/> for more details.
        ''' </summary>
        ''' <seealso cref=       #invalidate </seealso>
        ''' <seealso cref=       #doLayout() </seealso>
        ''' <seealso cref=       LayoutManager </seealso>
        ''' <seealso cref=       Container#validate
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub validate()
            SyncLock treeLock
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                Dim wasValid As Boolean = valid
                If (Not wasValid) AndAlso peer_Renamed IsNot Nothing Then
                    Dim newfont As font = font
                    Dim oldfont As font = peerFont
                    If newfont IsNot oldfont AndAlso (oldfont Is Nothing OrElse (Not oldfont.Equals(newfont))) Then
                        peer_Renamed.font = newfont
                        peerFont = newfont
                    End If
                    peer_Renamed.layout()
                End If
                valid = True
                If Not wasValid Then mixOnValidating()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Invalidates this component and its ancestors.
        ''' <p>
        ''' By default, all the ancestors of the component up to the top-most
        ''' container of the hierarchy are marked invalid. If the {@code
        ''' java.awt.smartInvalidate} system property is set to {@code true},
        ''' invalidation stops on the nearest validate root of this component.
        ''' Marking a container <i>invalid</i> indicates that the container needs to
        ''' be laid out.
        ''' <p>
        ''' This method is called automatically when any layout-related information
        ''' changes (e.g. setting the bounds of the component, or adding the
        ''' component to a container).
        ''' <p>
        ''' This method might be called often, so it should work fast.
        ''' </summary>
        ''' <seealso cref=       #validate </seealso>
        ''' <seealso cref=       #doLayout </seealso>
        ''' <seealso cref=       LayoutManager </seealso>
        ''' <seealso cref=       java.awt.Container#isValidateRoot
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub invalidate()
            SyncLock treeLock
                '             Nullify cached layout and size information.
                '             * For efficiency, propagate invalidate() upwards only if
                '             * some other component hasn't already done so first.
                '             
                valid = False
                If Not preferredSizeSet Then prefSize = Nothing
                If Not minimumSizeSet Then minSize = Nothing
                If Not maximumSizeSet Then maxSize = Nothing
                invalidateParent()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Invalidates the parent of this component if any.
        ''' 
        ''' This method MUST BE invoked under the TreeLock.
        ''' </summary>
        Friend Overridable Sub invalidateParent()
            If parent IsNot Nothing Then parent.invalidateIfValid()
        End Sub

        ''' <summary>
        ''' Invalidates the component unless it is already invalid.
        ''' </summary>
        Friend Sub invalidateIfValid()
            If valid Then invalidate()
        End Sub

        ''' <summary>
        ''' Revalidates the component hierarchy up to the nearest validate root.
        ''' <p>
        ''' This method first invalidates the component hierarchy starting from this
        ''' component up to the nearest validate root. Afterwards, the component
        ''' hierarchy is validated starting from the nearest validate root.
        ''' <p>
        ''' This is a convenience method supposed to help application developers
        ''' avoid looking for validate roots manually. Basically, it's equivalent to
        ''' first calling the <seealso cref="#invalidate()"/> method on this component, and
        ''' then calling the <seealso cref="#validate()"/> method on the nearest validate
        ''' root.
        ''' </summary>
        ''' <seealso cref= Container#isValidateRoot
        ''' @since 1.7 </seealso>
        Public Overridable Sub revalidate()
            revalidateSynchronously()
        End Sub

        ''' <summary>
        ''' Revalidates the component synchronously.
        ''' </summary>
        Friend Sub revalidateSynchronously()
            SyncLock treeLock
                invalidate()

                Dim root As Container = container
                If root Is Nothing Then
                    ' There's no parents. Just validate itself.
                    validate()
                Else
                    Do While Not root.validateRoot
                        If root.container Is Nothing Then Exit Do

                        root = root.container
                    Loop

                    root.validate()
                End If
            End SyncLock
        End Sub

        ''' <summary>
        ''' Creates a graphics context for this component. This method will
        ''' return <code>null</code> if this component is currently not
        ''' displayable. </summary>
        ''' <returns> a graphics context for this component, or <code>null</code>
        '''             if it has none </returns>
        ''' <seealso cref=       #paint
        ''' @since     JDK1.0 </seealso>
        Public Overridable Property graphics As Graphics
            Get
                If TypeOf peer Is java.awt.peer.LightweightPeer Then
                    ' This is for a lightweight component, need to
                    ' translate coordinate spaces and clip relative
                    ' to the parent.
                    If parent Is Nothing Then Return Nothing
                    Dim g As Graphics = parent.graphics
                    If g Is Nothing Then Return Nothing
                    If TypeOf g Is sun.awt.ConstrainableGraphics Then
                        CType(g, sun.awt.ConstrainableGraphics).constrain(x, y, width, height)
                    Else
                        g.translate(x, y)
                        g.cliplip(0, 0, width, height)
                    End If
                    g.font = font
                    Return g
                Else
                    Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                    Return If(peer_Renamed IsNot Nothing, peer_Renamed.graphics, Nothing)
                End If
            End Get
        End Property

        Friend Property graphics_NoClientCode As Graphics
            Get
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then
                    ' This is for a lightweight component, need to
                    ' translate coordinate spaces and clip relative
                    ' to the parent.
                    Dim parent_Renamed As Container = Me.parent
                    If parent_Renamed Is Nothing Then Return Nothing
                    Dim g As Graphics = parent_Renamed.graphics_NoClientCode
                    If g Is Nothing Then Return Nothing
                    If TypeOf g Is sun.awt.ConstrainableGraphics Then
                        CType(g, sun.awt.ConstrainableGraphics).constrain(x, y, width, height)
                    Else
                        g.translate(x, y)
                        g.cliplip(0, 0, width, height)
                    End If
                    g.font = font_NoClientCode
                    Return g
                Else
                    Return If(peer_Renamed IsNot Nothing, peer_Renamed.graphics, Nothing)
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the font metrics for the specified font.
        ''' Warning: Since Font metrics are affected by the
        ''' <seealso cref="java.awt.font.FontRenderContext FontRenderContext"/> and
        ''' this method does not provide one, it can return only metrics for
        ''' the default render context which may not match that used when
        ''' rendering on the Component if <seealso cref="Graphics2D"/> functionality is being
        ''' used. Instead metrics can be obtained at rendering time by calling
        ''' <seealso cref="Graphics#getFontMetrics()"/> or text measurement APIs on the
        ''' <seealso cref="Font Font"/> class. </summary>
        ''' <param name="font"> the font for which font metrics is to be
        '''          obtained </param>
        ''' <returns> the font metrics for <code>font</code> </returns>
        ''' <seealso cref=       #getFont </seealso>
        ''' <seealso cref=       #getPeer </seealso>
        ''' <seealso cref=       java.awt.peer.ComponentPeer#getFontMetrics(Font) </seealso>
        ''' <seealso cref=       Toolkit#getFontMetrics(Font)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Function getFontMetrics(ByVal font_Renamed As font) As FontMetrics
            ' This is an unsupported hack, but left in for a customer.
            ' Do not remove.
            Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
            If TypeOf fm Is sun.font.SunFontManager AndAlso CType(fm, sun.font.SunFontManager).usePlatformFontMetrics() Then

                If peer IsNot Nothing AndAlso Not (TypeOf peer Is java.awt.peer.LightweightPeer) Then Return peer.getFontMetrics(font_Renamed)
            End If
            Return sun.font.FontDesignMetrics.getMetrics(font_Renamed)
        End Function

        ''' <summary>
        ''' Sets the cursor image to the specified cursor.  This cursor
        ''' image is displayed when the <code>contains</code> method for
        ''' this component returns true for the current cursor location, and
        ''' this Component is visible, displayable, and enabled. Setting the
        ''' cursor of a <code>Container</code> causes that cursor to be displayed
        ''' within all of the container's subcomponents, except for those
        ''' that have a non-<code>null</code> cursor.
        ''' <p>
        ''' The method may have no visual effect if the Java platform
        ''' implementation and/or the native system do not support
        ''' changing the mouse cursor shape. </summary>
        ''' <param name="cursor"> One of the constants defined
        '''          by the <code>Cursor</code> class;
        '''          if this parameter is <code>null</code>
        '''          then this component will inherit
        '''          the cursor of its parent </param>
        ''' <seealso cref=       #isEnabled </seealso>
        ''' <seealso cref=       #isShowing </seealso>
        ''' <seealso cref=       #getCursor </seealso>
        ''' <seealso cref=       #contains </seealso>
        ''' <seealso cref=       Toolkit#createCustomCursor </seealso>
        ''' <seealso cref=       Cursor
        ''' @since     JDK1.1 </seealso>
        Public Overridable Property cursor As Cursor
            Set(ByVal cursor_Renamed As Cursor)
                Me.cursor = cursor_Renamed
                updateCursorImmediately()
            End Set
            Get
                Return cursor_NoClientCode
            End Get
        End Property

        ''' <summary>
        ''' Updates the cursor.  May not be invoked from the native
        ''' message pump.
        ''' </summary>
        Friend Sub updateCursorImmediately()
            If TypeOf peer Is java.awt.peer.LightweightPeer Then
                Dim nativeContainer_Renamed As Container = nativeContainer

                If nativeContainer_Renamed Is Nothing Then Return

                Dim cPeer As java.awt.peer.ComponentPeer = nativeContainer_Renamed.peer

                If cPeer IsNot Nothing Then cPeer.updateCursorImmediately()
            ElseIf peer IsNot Nothing Then
                peer.updateCursorImmediately()
            End If
        End Sub


        Friend Property cursor_NoClientCode As Cursor
            Get
                Dim cursor_Renamed As Cursor = Me.cursor
                If cursor_Renamed IsNot Nothing Then Return cursor_Renamed
                Dim parent_Renamed As Container = Me.parent
                If parent_Renamed IsNot Nothing Then
                    Return parent_Renamed.cursor_NoClientCode
                Else
                    Return cursor.getPredefinedCursor(cursor.DEFAULT_CURSOR)
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns whether the cursor has been explicitly set for this Component.
        ''' If this method returns <code>false</code>, this Component is inheriting
        ''' its cursor from an ancestor.
        ''' </summary>
        ''' <returns> <code>true</code> if the cursor has been explicitly set for this
        '''         Component; <code>false</code> otherwise.
        ''' @since 1.4 </returns>
        Public Overridable Property cursorSet As Boolean
            Get
                Return (cursor IsNot Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Paints this component.
        ''' <p>
        ''' This method is called when the contents of the component should
        ''' be painted; such as when the component is first being shown or
        ''' is damaged and in need of repair.  The clip rectangle in the
        ''' <code>Graphics</code> parameter is set to the area
        ''' which needs to be painted.
        ''' Subclasses of <code>Component</code> that override this
        ''' method need not call <code>super.paint(g)</code>.
        ''' <p>
        ''' For performance reasons, <code>Component</code>s with zero width
        ''' or height aren't considered to need painting when they are first shown,
        ''' and also aren't considered to need repair.
        ''' <p>
        ''' <b>Note</b>: For more information on the paint mechanisms utilitized
        ''' by AWT and Swing, including information on how to write the most
        ''' efficient painting code, see
        ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
        ''' </summary>
        ''' <param name="g"> the graphics context to use for painting </param>
        ''' <seealso cref=       #update
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub paint(ByVal g As Graphics)
        End Sub

        ''' <summary>
        ''' Updates this component.
        ''' <p>
        ''' If this component is not a lightweight component, the
        ''' AWT calls the <code>update</code> method in response to
        ''' a call to <code>repaint</code>.  You can assume that
        ''' the background is not cleared.
        ''' <p>
        ''' The <code>update</code> method of <code>Component</code>
        ''' calls this component's <code>paint</code> method to redraw
        ''' this component.  This method is commonly overridden by subclasses
        ''' which need to do additional work in response to a call to
        ''' <code>repaint</code>.
        ''' Subclasses of Component that override this method should either
        ''' call <code>super.update(g)</code>, or call <code>paint(g)</code>
        ''' directly from their <code>update</code> method.
        ''' <p>
        ''' The origin of the graphics context, its
        ''' (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
        ''' top-left corner of this component. The clipping region of the
        ''' graphics context is the bounding rectangle of this component.
        ''' 
        ''' <p>
        ''' <b>Note</b>: For more information on the paint mechanisms utilitized
        ''' by AWT and Swing, including information on how to write the most
        ''' efficient painting code, see
        ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
        ''' </summary>
        ''' <param name="g"> the specified context to use for updating </param>
        ''' <seealso cref=       #paint </seealso>
        ''' <seealso cref=       #repaint()
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub update(ByVal g As Graphics)
            paint(g)
        End Sub

        ''' <summary>
        ''' Paints this component and all of its subcomponents.
        ''' <p>
        ''' The origin of the graphics context, its
        ''' (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
        ''' top-left corner of this component. The clipping region of the
        ''' graphics context is the bounding rectangle of this component.
        ''' </summary>
        ''' <param name="g">   the graphics context to use for painting </param>
        ''' <seealso cref=       #paint
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub paintAll(ByVal g As Graphics)
            If showing Then GraphicsCallback.PeerPaintCallback.instance.runOneComponent(Me, New Rectangle(0, 0, width, height), g, g.clip, GraphicsCallback.LIGHTWEIGHTS Or GraphicsCallback.HEAVYWEIGHTS)
        End Sub

        ''' <summary>
        ''' Simulates the peer callbacks into java.awt for painting of
        ''' lightweight Components. </summary>
        ''' <param name="g">   the graphics context to use for painting </param>
        ''' <seealso cref=       #paintAll </seealso>
        Friend Overridable Sub lightweightPaint(ByVal g As Graphics)
            paint(g)
        End Sub

        ''' <summary>
        ''' Paints all the heavyweight subcomponents.
        ''' </summary>
        Friend Overridable Sub paintHeavyweightComponents(ByVal g As Graphics)
        End Sub

        ''' <summary>
        ''' Repaints this component.
        ''' <p>
        ''' If this component is a lightweight component, this method
        ''' causes a call to this component's <code>paint</code>
        ''' method as soon as possible.  Otherwise, this method causes
        ''' a call to this component's <code>update</code> method as soon
        ''' as possible.
        ''' <p>
        ''' <b>Note</b>: For more information on the paint mechanisms utilitized
        ''' by AWT and Swing, including information on how to write the most
        ''' efficient painting code, see
        ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
        ''' 
        ''' </summary>
        ''' <seealso cref=       #update(Graphics)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub repaint()
            repaint(0, 0, 0, width, height)
        End Sub

        ''' <summary>
        ''' Repaints the component.  If this component is a lightweight
        ''' component, this results in a call to <code>paint</code>
        ''' within <code>tm</code> milliseconds.
        ''' <p>
        ''' <b>Note</b>: For more information on the paint mechanisms utilitized
        ''' by AWT and Swing, including information on how to write the most
        ''' efficient painting code, see
        ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
        ''' </summary>
        ''' <param name="tm"> maximum time in milliseconds before update </param>
        ''' <seealso cref= #paint </seealso>
        ''' <seealso cref= #update(Graphics)
        ''' @since JDK1.0 </seealso>
        Public Overridable Sub repaint(ByVal tm As Long)
            repaint(tm, 0, 0, width, height)
        End Sub

        ''' <summary>
        ''' Repaints the specified rectangle of this component.
        ''' <p>
        ''' If this component is a lightweight component, this method
        ''' causes a call to this component's <code>paint</code> method
        ''' as soon as possible.  Otherwise, this method causes a call to
        ''' this component's <code>update</code> method as soon as possible.
        ''' <p>
        ''' <b>Note</b>: For more information on the paint mechanisms utilitized
        ''' by AWT and Swing, including information on how to write the most
        ''' efficient painting code, see
        ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
        ''' </summary>
        ''' <param name="x">   the <i>x</i> coordinate </param>
        ''' <param name="y">   the <i>y</i> coordinate </param>
        ''' <param name="width">   the width </param>
        ''' <param name="height">  the height </param>
        ''' <seealso cref=       #update(Graphics)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub repaint(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
            repaint(0, x, y, width, height)
        End Sub

        ''' <summary>
        ''' Repaints the specified rectangle of this component within
        ''' <code>tm</code> milliseconds.
        ''' <p>
        ''' If this component is a lightweight component, this method causes
        ''' a call to this component's <code>paint</code> method.
        ''' Otherwise, this method causes a call to this component's
        ''' <code>update</code> method.
        ''' <p>
        ''' <b>Note</b>: For more information on the paint mechanisms utilitized
        ''' by AWT and Swing, including information on how to write the most
        ''' efficient painting code, see
        ''' <a href="http://www.oracle.com/technetwork/java/painting-140037.html">Painting in AWT and Swing</a>.
        ''' </summary>
        ''' <param name="tm">   maximum time in milliseconds before update </param>
        ''' <param name="x">    the <i>x</i> coordinate </param>
        ''' <param name="y">    the <i>y</i> coordinate </param>
        ''' <param name="width">    the width </param>
        ''' <param name="height">   the height </param>
        ''' <seealso cref=       #update(Graphics)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub repaint(ByVal tm As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
            If TypeOf Me.peer Is java.awt.peer.LightweightPeer Then
                ' Needs to be translated to parent coordinates since
                ' a parent native container provides the actual repaint
                ' services.  Additionally, the request is restricted to
                ' the bounds of the component.
                If parent IsNot Nothing Then
                    If x < 0 Then
                        width += x
                        x = 0
                    End If
                    If y < 0 Then
                        height += y
                        y = 0
                    End If

                    Dim pwidth As Integer = If(width > Me.width, Me.width, width)
                    Dim pheight As Integer = If(height > Me.height, Me.height, height)

                    If pwidth <= 0 OrElse pheight <= 0 Then Return

                    Dim px As Integer = Me.x + x
                    Dim py As Integer = Me.y + y
                    parent.repaint(tm, px, py, pwidth, pheight)
                End If
            Else
                If visible AndAlso (Me.peer IsNot Nothing) AndAlso (width > 0) AndAlso (height > 0) Then
                    Dim e As New PaintEvent(Me, PaintEvent.UPDATE, New Rectangle(x, y, width, height))
                    sun.awt.SunToolkit.postEvent(sun.awt.SunToolkit.targetToAppContext(Me), e)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Prints this component. Applications should override this method
        ''' for components that must do special processing before being
        ''' printed or should be printed differently than they are painted.
        ''' <p>
        ''' The default implementation of this method calls the
        ''' <code>paint</code> method.
        ''' <p>
        ''' The origin of the graphics context, its
        ''' (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
        ''' top-left corner of this component. The clipping region of the
        ''' graphics context is the bounding rectangle of this component. </summary>
        ''' <param name="g">   the graphics context to use for printing </param>
        ''' <seealso cref=       #paint(Graphics)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub print(ByVal g As Graphics)
            paint(g)
        End Sub

        ''' <summary>
        ''' Prints this component and all of its subcomponents.
        ''' <p>
        ''' The origin of the graphics context, its
        ''' (<code>0</code>,&nbsp;<code>0</code>) coordinate point, is the
        ''' top-left corner of this component. The clipping region of the
        ''' graphics context is the bounding rectangle of this component. </summary>
        ''' <param name="g">   the graphics context to use for printing </param>
        ''' <seealso cref=       #print(Graphics)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub printAll(ByVal g As Graphics)
            If showing Then GraphicsCallback.PeerPrintCallback.instance.runOneComponent(Me, New Rectangle(0, 0, width, height), g, g.clip, GraphicsCallback.LIGHTWEIGHTS Or GraphicsCallback.HEAVYWEIGHTS)
        End Sub

        ''' <summary>
        ''' Simulates the peer callbacks into java.awt for printing of
        ''' lightweight Components. </summary>
        ''' <param name="g">   the graphics context to use for printing </param>
        ''' <seealso cref=       #printAll </seealso>
        Friend Overridable Sub lightweightPrint(ByVal g As Graphics)
            print(g)
        End Sub

        ''' <summary>
        ''' Prints all the heavyweight subcomponents.
        ''' </summary>
        Friend Overridable Sub printHeavyweightComponents(ByVal g As Graphics)
        End Sub

        Private Property insets_NoClientCode As Insets
            Get
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                If TypeOf peer_Renamed Is java.awt.peer.ContainerPeer Then Return CType(CType(peer_Renamed, java.awt.peer.ContainerPeer).insets.clone(), Insets)
                Return New Insets(0, 0, 0, 0)
            End Get
        End Property

        ''' <summary>
        ''' Repaints the component when the image has changed.
        ''' This <code>imageUpdate</code> method of an <code>ImageObserver</code>
        ''' is called when more information about an
        ''' image which had been previously requested using an asynchronous
        ''' routine such as the <code>drawImage</code> method of
        ''' <code>Graphics</code> becomes available.
        ''' See the definition of <code>imageUpdate</code> for
        ''' more information on this method and its arguments.
        ''' <p>
        ''' The <code>imageUpdate</code> method of <code>Component</code>
        ''' incrementally draws an image on the component as more of the bits
        ''' of the image are available.
        ''' <p>
        ''' If the system property <code>awt.image.incrementaldraw</code>
        ''' is missing or has the value <code>true</code>, the image is
        ''' incrementally drawn. If the system property has any other value,
        ''' then the image is not drawn until it has been completely loaded.
        ''' <p>
        ''' Also, if incremental drawing is in effect, the value of the
        ''' system property <code>awt.image.redrawrate</code> is interpreted
        ''' as an integer to give the maximum redraw rate, in milliseconds. If
        ''' the system property is missing or cannot be interpreted as an
        ''' integer, the redraw rate is once every 100ms.
        ''' <p>
        ''' The interpretation of the <code>x</code>, <code>y</code>,
        ''' <code>width</code>, and <code>height</code> arguments depends on
        ''' the value of the <code>infoflags</code> argument.
        ''' </summary>
        ''' <param name="img">   the image being observed </param>
        ''' <param name="infoflags">   see <code>imageUpdate</code> for more information </param>
        ''' <param name="x">   the <i>x</i> coordinate </param>
        ''' <param name="y">   the <i>y</i> coordinate </param>
        ''' <param name="w">   the width </param>
        ''' <param name="h">   the height </param>
        ''' <returns>    <code>false</code> if the infoflags indicate that the
        '''            image is completely loaded; <code>true</code> otherwise.
        ''' </returns>
        ''' <seealso cref=     java.awt.image.ImageObserver </seealso>
        ''' <seealso cref=     Graphics#drawImage(Image, int, int, Color, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=     Graphics#drawImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=     Graphics#drawImage(Image, int, int, int, int, Color, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=     Graphics#drawImage(Image, int, int, int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=     java.awt.image.ImageObserver#imageUpdate(java.awt.Image, int, int, int, int, int)
        ''' @since   JDK1.0 </seealso>
        Public Overridable Function imageUpdate(ByVal img As image, ByVal infoflags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
            Dim rate As Integer = -1
            If (infoflags And (FRAMEBITS Or ALLBITS)) <> 0 Then
                rate = 0
            ElseIf (infoflags And SOMEBITS) <> 0 Then
                If isInc Then
                    rate = incRate
                    If rate < 0 Then rate = 0
                End If
            End If
            If rate >= 0 Then repaint(rate, 0, 0, width, height)
            Return (infoflags And (ALLBITS Or ABORT)) = 0
        End Function

        ''' <summary>
        ''' Creates an image from the specified image producer. </summary>
        ''' <param name="producer">  the image producer </param>
        ''' <returns>    the image produced
        ''' @since     JDK1.0 </returns>
        Public Overridable Function createImage(ByVal producer As java.awt.image.ImageProducer) As image
            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            If (peer_Renamed IsNot Nothing) AndAlso Not (TypeOf peer_Renamed Is java.awt.peer.LightweightPeer) Then Return peer_Renamed.createImage(producer)
            Return toolkit.createImage(producer)
        End Function

        ''' <summary>
        ''' Creates an off-screen drawable image
        '''     to be used for double buffering. </summary>
        ''' <param name="width"> the specified width </param>
        ''' <param name="height"> the specified height </param>
        ''' <returns>    an off-screen drawable image, which can be used for double
        '''    buffering.  The return value may be <code>null</code> if the
        '''    component is not displayable.  This will always happen if
        '''    <code>GraphicsEnvironment.isHeadless()</code> returns
        '''    <code>true</code>. </returns>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= GraphicsEnvironment#isHeadless
        ''' @since     JDK1.0 </seealso>
        Public Overridable Function createImage(ByVal width As Integer, ByVal height As Integer) As image
            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then
                If parent IsNot Nothing Then
                    Return parent.createImage(width, height)
                Else
                    Return Nothing
                End If
            Else
                Return If(peer_Renamed IsNot Nothing, peer_Renamed.createImage(width, height), Nothing)
            End If
        End Function

        ''' <summary>
        ''' Creates a volatile off-screen drawable image
        '''     to be used for double buffering. </summary>
        ''' <param name="width"> the specified width. </param>
        ''' <param name="height"> the specified height. </param>
        ''' <returns>    an off-screen drawable image, which can be used for double
        '''    buffering.  The return value may be <code>null</code> if the
        '''    component is not displayable.  This will always happen if
        '''    <code>GraphicsEnvironment.isHeadless()</code> returns
        '''    <code>true</code>. </returns>
        ''' <seealso cref= java.awt.image.VolatileImage </seealso>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= GraphicsEnvironment#isHeadless
        ''' @since     1.4 </seealso>
        Public Overridable Function createVolatileImage(ByVal width As Integer, ByVal height As Integer) As java.awt.image.VolatileImage
            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then
                If parent IsNot Nothing Then
                    Return parent.createVolatileImage(width, height)
                Else
                    Return Nothing
                End If
            Else
                Return If(peer_Renamed IsNot Nothing, peer_Renamed.createVolatileImage(width, height), Nothing)
            End If
        End Function

        ''' <summary>
        ''' Creates a volatile off-screen drawable image, with the given capabilities.
        ''' The contents of this image may be lost at any time due
        ''' to operating system issues, so the image must be managed
        ''' via the <code>VolatileImage</code> interface. </summary>
        ''' <param name="width"> the specified width. </param>
        ''' <param name="height"> the specified height. </param>
        ''' <param name="caps"> the image capabilities </param>
        ''' <exception cref="AWTException"> if an image with the specified capabilities cannot
        ''' be created </exception>
        ''' <returns> a VolatileImage object, which can be used
        ''' to manage surface contents loss and capabilities. </returns>
        ''' <seealso cref= java.awt.image.VolatileImage
        ''' @since 1.4 </seealso>
        Public Overridable Function createVolatileImage(ByVal width As Integer, ByVal height As Integer, ByVal caps As ImageCapabilities) As java.awt.image.VolatileImage
            ' REMIND : check caps
            Return createVolatileImage(width, height)
        End Function

        ''' <summary>
        ''' Prepares an image for rendering on this component.  The image
        ''' data is downloaded asynchronously in another thread and the
        ''' appropriate screen representation of the image is generated. </summary>
        ''' <param name="image">   the <code>Image</code> for which to
        '''                    prepare a screen representation </param>
        ''' <param name="observer">   the <code>ImageObserver</code> object
        '''                       to be notified as the image is being prepared </param>
        ''' <returns>    <code>true</code> if the image has already been fully
        '''           prepared; <code>false</code> otherwise
        ''' @since     JDK1.0 </returns>
        Public Overridable Function prepareImage(ByVal image_Renamed As image, ByVal observer As java.awt.image.ImageObserver) As Boolean
            Return prepareImage(image_Renamed, -1, -1, observer)
        End Function

        ''' <summary>
        ''' Prepares an image for rendering on this component at the
        ''' specified width and height.
        ''' <p>
        ''' The image data is downloaded asynchronously in another thread,
        ''' and an appropriately scaled screen representation of the image is
        ''' generated. </summary>
        ''' <param name="image">    the instance of <code>Image</code>
        '''            for which to prepare a screen representation </param>
        ''' <param name="width">    the width of the desired screen representation </param>
        ''' <param name="height">   the height of the desired screen representation </param>
        ''' <param name="observer">   the <code>ImageObserver</code> object
        '''            to be notified as the image is being prepared </param>
        ''' <returns>    <code>true</code> if the image has already been fully
        '''          prepared; <code>false</code> otherwise </returns>
        ''' <seealso cref=       java.awt.image.ImageObserver
        ''' @since     JDK1.0 </seealso>
        Public Overridable Function prepareImage(ByVal image_Renamed As image, ByVal width As Integer, ByVal height As Integer, ByVal observer As java.awt.image.ImageObserver) As Boolean
            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then
                Return If(parent IsNot Nothing, parent.prepareImage(image_Renamed, width, height, observer), toolkit.prepareImage(image_Renamed, width, height, observer))
            Else
                Return If(peer_Renamed IsNot Nothing, peer_Renamed.prepareImage(image_Renamed, width, height, observer), toolkit.prepareImage(image_Renamed, width, height, observer))
            End If
        End Function

        ''' <summary>
        ''' Returns the status of the construction of a screen representation
        ''' of the specified image.
        ''' <p>
        ''' This method does not cause the image to begin loading. An
        ''' application must use the <code>prepareImage</code> method
        ''' to force the loading of an image.
        ''' <p>
        ''' Information on the flags returned by this method can be found
        ''' with the discussion of the <code>ImageObserver</code> interface. </summary>
        ''' <param name="image">   the <code>Image</code> object whose status
        '''            is being checked </param>
        ''' <param name="observer">   the <code>ImageObserver</code>
        '''            object to be notified as the image is being prepared </param>
        ''' <returns>  the bitwise inclusive <b>OR</b> of
        '''            <code>ImageObserver</code> flags indicating what
        '''            information about the image is currently available </returns>
        ''' <seealso cref=      #prepareImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=      Toolkit#checkImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=      java.awt.image.ImageObserver
        ''' @since    JDK1.0 </seealso>
        Public Overridable Function checkImage(ByVal image_Renamed As image, ByVal observer As java.awt.image.ImageObserver) As Integer
            Return checkImage(image_Renamed, -1, -1, observer)
        End Function

        ''' <summary>
        ''' Returns the status of the construction of a screen representation
        ''' of the specified image.
        ''' <p>
        ''' This method does not cause the image to begin loading. An
        ''' application must use the <code>prepareImage</code> method
        ''' to force the loading of an image.
        ''' <p>
        ''' The <code>checkImage</code> method of <code>Component</code>
        ''' calls its peer's <code>checkImage</code> method to calculate
        ''' the flags. If this component does not yet have a peer, the
        ''' component's toolkit's <code>checkImage</code> method is called
        ''' instead.
        ''' <p>
        ''' Information on the flags returned by this method can be found
        ''' with the discussion of the <code>ImageObserver</code> interface. </summary>
        ''' <param name="image">   the <code>Image</code> object whose status
        '''                    is being checked </param>
        ''' <param name="width">   the width of the scaled version
        '''                    whose status is to be checked </param>
        ''' <param name="height">  the height of the scaled version
        '''                    whose status is to be checked </param>
        ''' <param name="observer">   the <code>ImageObserver</code> object
        '''                    to be notified as the image is being prepared </param>
        ''' <returns>    the bitwise inclusive <b>OR</b> of
        '''            <code>ImageObserver</code> flags indicating what
        '''            information about the image is currently available </returns>
        ''' <seealso cref=      #prepareImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=      Toolkit#checkImage(Image, int, int, java.awt.image.ImageObserver) </seealso>
        ''' <seealso cref=      java.awt.image.ImageObserver
        ''' @since    JDK1.0 </seealso>
        Public Overridable Function checkImage(ByVal image_Renamed As image, ByVal width As Integer, ByVal height As Integer, ByVal observer As java.awt.image.ImageObserver) As Integer
            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            If TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then
                Return If(parent IsNot Nothing, parent.checkImage(image_Renamed, width, height, observer), toolkit.checkImage(image_Renamed, width, height, observer))
            Else
                Return If(peer_Renamed IsNot Nothing, peer_Renamed.checkImage(image_Renamed, width, height, observer), toolkit.checkImage(image_Renamed, width, height, observer))
            End If
        End Function

        ''' <summary>
        ''' Creates a new strategy for multi-buffering on this component.
        ''' Multi-buffering is useful for rendering performance.  This method
        ''' attempts to create the best strategy available with the number of
        ''' buffers supplied.  It will always create a <code>BufferStrategy</code>
        ''' with that number of buffers.
        ''' A page-flipping strategy is attempted first, then a blitting strategy
        ''' using accelerated buffers.  Finally, an unaccelerated blitting
        ''' strategy is used.
        ''' <p>
        ''' Each time this method is called,
        ''' the existing buffer strategy for this component is discarded. </summary>
        ''' <param name="numBuffers"> number of buffers to create, including the front buffer </param>
        ''' <exception cref="IllegalArgumentException"> if numBuffers is less than 1. </exception>
        ''' <exception cref="IllegalStateException"> if the component is not displayable </exception>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= Window#getBufferStrategy() </seealso>
        ''' <seealso cref= Canvas#getBufferStrategy()
        ''' @since 1.4 </seealso>
        Friend Overridable Sub createBufferStrategy(ByVal numBuffers As Integer)
            Dim bufferCaps As BufferCapabilities
            If numBuffers > 1 Then
                ' Try to create a page-flipping strategy
                bufferCaps = New BufferCapabilities(New ImageCapabilities(True), New ImageCapabilities(True), BufferCapabilities.flipContents.UNDEFINED)
                Try
                    createBufferStrategy(numBuffers, bufferCaps)
                    Return ' Success
                Catch e As AWTException
                    ' Failed
                End Try
            End If
            ' Try a blitting (but still accelerated) strategy
            bufferCaps = New BufferCapabilities(New ImageCapabilities(True), New ImageCapabilities(True), Nothing)
            Try
                createBufferStrategy(numBuffers, bufferCaps)
                Return ' Success
            Catch e As AWTException
                ' Failed
            End Try
            ' Try an unaccelerated blitting strategy
            bufferCaps = New BufferCapabilities(New ImageCapabilities(False), New ImageCapabilities(False), Nothing)
            Try
                createBufferStrategy(numBuffers, bufferCaps)
                Return ' Success
            Catch e As AWTException
                ' Code should never reach here (an unaccelerated blitting
                ' strategy should always work)
                Throw New InternalError("Could not create a buffer strategy", e)
            End Try
        End Sub

        ''' <summary>
        ''' Creates a new strategy for multi-buffering on this component with the
        ''' required buffer capabilities.  This is useful, for example, if only
        ''' accelerated memory or page flipping is desired (as specified by the
        ''' buffer capabilities).
        ''' <p>
        ''' Each time this method
        ''' is called, <code>dispose</code> will be invoked on the existing
        ''' <code>BufferStrategy</code>. </summary>
        ''' <param name="numBuffers"> number of buffers to create </param>
        ''' <param name="caps"> the required capabilities for creating the buffer strategy;
        ''' cannot be <code>null</code> </param>
        ''' <exception cref="AWTException"> if the capabilities supplied could not be
        ''' supported or met; this may happen, for example, if there is not enough
        ''' accelerated memory currently available, or if page flipping is specified
        ''' but not possible. </exception>
        ''' <exception cref="IllegalArgumentException"> if numBuffers is less than 1, or if
        ''' caps is <code>null</code> </exception>
        ''' <seealso cref= Window#getBufferStrategy() </seealso>
        ''' <seealso cref= Canvas#getBufferStrategy()
        ''' @since 1.4 </seealso>
        Friend Overridable Sub createBufferStrategy(ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
            ' Check arguments
            If numBuffers < 1 Then Throw New IllegalArgumentException("Number of buffers must be at least 1")
            If caps Is Nothing Then Throw New IllegalArgumentException("No capabilities specified")
            ' Destroy old buffers
            If bufferStrategy IsNot Nothing Then bufferStrategy.dispose()
            If numBuffers = 1 Then
                bufferStrategy = New SingleBufferStrategy(Me, caps)
            Else
                Dim sge As sun.java2d.SunGraphicsEnvironment = CType(GraphicsEnvironment.localGraphicsEnvironment, sun.java2d.SunGraphicsEnvironment)
                If (Not caps.pageFlipping) AndAlso sge.isFlipStrategyPreferred(peer) Then caps = New ProxyCapabilities(Me, caps)
                ' assert numBuffers > 1;
                If caps.pageFlipping Then
                    bufferStrategy = New FlipSubRegionBufferStrategy(Me, numBuffers, caps)
                Else
                    bufferStrategy = New BltSubRegionBufferStrategy(Me, numBuffers, caps)
                End If
            End If
        End Sub

        ''' <summary>
        ''' This is a proxy capabilities class used when a FlipBufferStrategy
        ''' is created instead of the requested Blit strategy.
        ''' </summary>
        ''' <seealso cref= sun.java2d.SunGraphicsEnvironment#isFlipStrategyPreferred(ComponentPeer) </seealso>
        Private Class ProxyCapabilities
            Inherits sun.java2d.pipe.hw.ExtendedBufferCapabilities

            Private ReadOnly outerInstance As Component

            Private orig As BufferCapabilities
            Private Sub New(ByVal outerInstance As Component, ByVal orig As BufferCapabilities)
                Me.outerInstance = outerInstance
                MyBase.New(orig.frontBufferCapabilities, orig.backBufferCapabilities, If(orig.flipContents Is BufferCapabilities.flipContents.BACKGROUND, BufferCapabilities.flipContents.BACKGROUND, BufferCapabilities.flipContents.COPIED))
                Me.orig = orig
            End Sub
        End Class

        ''' <returns> the buffer strategy used by this component </returns>
        ''' <seealso cref= Window#createBufferStrategy </seealso>
        ''' <seealso cref= Canvas#createBufferStrategy
        ''' @since 1.4 </seealso>
        Friend Overridable Property bufferStrategy As java.awt.image.BufferStrategy
            Get
                Return bufferStrategy
            End Get
        End Property

        ''' <returns> the back buffer currently used by this component's
        ''' BufferStrategy.  If there is no BufferStrategy or no
        ''' back buffer, this method returns null. </returns>
        Friend Overridable Property backBuffer As image
            Get
                If bufferStrategy IsNot Nothing Then
                    If TypeOf bufferStrategy Is BltBufferStrategy Then
                        Dim bltBS As BltBufferStrategy = CType(bufferStrategy, BltBufferStrategy)
                        Return bltBS.backBuffer
                    ElseIf TypeOf bufferStrategy Is FlipBufferStrategy Then
                        Dim flipBS As FlipBufferStrategy = CType(bufferStrategy, FlipBufferStrategy)
                        Return flipBS.backBuffer
                    End If
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Inner class for flipping buffers on a component.  That component must
        ''' be a <code>Canvas</code> or <code>Window</code>. </summary>
        ''' <seealso cref= Canvas </seealso>
        ''' <seealso cref= Window </seealso>
        ''' <seealso cref= java.awt.image.BufferStrategy
        ''' @author Michael Martak
        ''' @since 1.4 </seealso>
        Protected Friend Class FlipBufferStrategy
            Inherits java.awt.image.BufferStrategy

            Private ReadOnly outerInstance As Component

            ''' <summary>
            ''' The number of buffers
            ''' </summary>
            Protected Friend numBuffers As Integer ' = 0
            ''' <summary>
            ''' The buffering capabilities
            ''' </summary>
            Protected Friend caps As BufferCapabilities ' = null
            ''' <summary>
            ''' The drawing buffer
            ''' </summary>
            Protected Friend drawBuffer As image ' = null
            ''' <summary>
            ''' The drawing buffer as a volatile image
            ''' </summary>
            Protected Friend drawVBuffer As java.awt.image.VolatileImage ' = null
            ''' <summary>
            ''' Whether or not the drawing buffer has been recently restored from
            ''' a lost state.
            ''' </summary>
            Protected Friend validatedContents As Boolean ' = false
            ''' <summary>
            ''' Size of the back buffers.  (Note: these fields were added in 6.0
            ''' but kept package-private to avoid exposing them in the spec.
            ''' None of these fields/methods really should have been marked
            ''' protected when they were introduced in 1.4, but now we just have
            ''' to live with that decision.)
            ''' </summary>
            Friend width As Integer
            Friend height As Integer

            ''' <summary>
            ''' Creates a new flipping buffer strategy for this component.
            ''' The component must be a <code>Canvas</code> or <code>Window</code>. </summary>
            ''' <seealso cref= Canvas </seealso>
            ''' <seealso cref= Window </seealso>
            ''' <param name="numBuffers"> the number of buffers </param>
            ''' <param name="caps"> the capabilities of the buffers </param>
            ''' <exception cref="AWTException"> if the capabilities supplied could not be
            ''' supported or met </exception>
            ''' <exception cref="ClassCastException"> if the component is not a canvas or
            ''' window. </exception>
            ''' <exception cref="IllegalStateException"> if the component has no peer </exception>
            ''' <exception cref="IllegalArgumentException"> if {@code numBuffers} is less than two,
            ''' or if {@code BufferCapabilities.isPageFlipping} is not
            ''' {@code true}. </exception>
            ''' <seealso cref= #createBuffers(int, BufferCapabilities) </seealso>
            Protected Friend Sub New(ByVal outerInstance As Component, ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
                Me.outerInstance = outerInstance
                If Not (TypeOf Component.this Is Window) AndAlso Not (TypeOf Component.this Is Canvas) Then Throw New ClassCastException("Component must be a Canvas or Window")
                Me.numBuffers = numBuffers
                Me.caps = caps
                createBuffers(numBuffers, caps)
            End Sub

            ''' <summary>
            ''' Creates one or more complex, flipping buffers with the given
            ''' capabilities. </summary>
            ''' <param name="numBuffers"> number of buffers to create; must be greater than
            ''' one </param>
            ''' <param name="caps"> the capabilities of the buffers.
            ''' <code>BufferCapabilities.isPageFlipping</code> must be
            ''' <code>true</code>. </param>
            ''' <exception cref="AWTException"> if the capabilities supplied could not be
            ''' supported or met </exception>
            ''' <exception cref="IllegalStateException"> if the component has no peer </exception>
            ''' <exception cref="IllegalArgumentException"> if numBuffers is less than two,
            ''' or if <code>BufferCapabilities.isPageFlipping</code> is not
            ''' <code>true</code>. </exception>
            ''' <seealso cref= java.awt.BufferCapabilities#isPageFlipping() </seealso>
            Protected Friend Overridable Sub createBuffers(ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
                If numBuffers < 2 Then
                    Throw New IllegalArgumentException("Number of buffers cannot be less than two")
                ElseIf outerInstance.peer Is Nothing Then
                    Throw New IllegalStateException("Component must have a valid peer")
                ElseIf caps Is Nothing OrElse (Not caps.pageFlipping) Then
                    Throw New IllegalArgumentException("Page flipping capabilities must be specified")
                End If

                ' save the current bounds
                width = outerInstance.width
                height = outerInstance.height

                If drawBuffer IsNot Nothing Then
                    ' dispose the existing backbuffers
                    drawBuffer = Nothing
                    drawVBuffer = Nothing
                    destroyBuffers()
                    ' ... then recreate the backbuffers
                End If

                If TypeOf caps Is sun.java2d.pipe.hw.ExtendedBufferCapabilities Then
                    Dim ebc As sun.java2d.pipe.hw.ExtendedBufferCapabilities = CType(caps, sun.java2d.pipe.hw.ExtendedBufferCapabilities)
                    If ebc.vSync = VSYNC_ON Then
                        ' if this buffer strategy is not allowed to be v-synced,
                        ' change the caps that we pass to the peer but keep on
                        ' trying to create v-synced buffers;
                        ' do not throw IAE here in case it is disallowed, see
                        ' ExtendedBufferCapabilities for more info
                        If Not sun.awt.image.VSyncedBSManager.vsyncAllowed(Me) Then caps = ebc.derive(VSYNC_DEFAULT)
                    End If
                End If

                outerInstance.peer.createBuffers(numBuffers, caps)
                updateInternalBuffers()
            End Sub

            ''' <summary>
            ''' Updates internal buffers (both volatile and non-volatile)
            ''' by requesting the back-buffer from the peer.
            ''' </summary>
            Private Sub updateInternalBuffers()
                ' get the images associated with the draw buffer
                drawBuffer = backBuffer
                If TypeOf drawBuffer Is java.awt.image.VolatileImage Then
                    drawVBuffer = CType(drawBuffer, java.awt.image.VolatileImage)
                Else
                    drawVBuffer = Nothing
                End If
            End Sub

            ''' <returns> direct access to the back buffer, as an image. </returns>
            ''' <exception cref="IllegalStateException"> if the buffers have not yet
            ''' been created </exception>
            Protected Friend Overridable Property backBuffer As image
                Get
                    If outerInstance.peer IsNot Nothing Then
                        Return outerInstance.peer.backBuffer
                    Else
                        Throw New IllegalStateException("Component must have a valid peer")
                    End If
                End Get
            End Property

            ''' <summary>
            ''' Flipping moves the contents of the back buffer to the front buffer,
            ''' either by copying or by moving the video pointer. </summary>
            ''' <param name="flipAction"> an integer value describing the flipping action
            ''' for the contents of the back buffer.  This should be one of the
            ''' values of the <code>BufferCapabilities.FlipContents</code>
            ''' property. </param>
            ''' <exception cref="IllegalStateException"> if the buffers have not yet
            ''' been created </exception>
            ''' <seealso cref= java.awt.BufferCapabilities#getFlipContents() </seealso>
            Protected Friend Overridable Sub flip(ByVal flipAction As BufferCapabilities.FlipContents)
                If outerInstance.peer IsNot Nothing Then
                    Dim backBuffer_Renamed As image = backBuffer
                    If backBuffer_Renamed IsNot Nothing Then outerInstance.peer.flip(0, 0, backBuffer_Renamed.getWidth(Nothing), backBuffer_Renamed.getHeight(Nothing), flipAction)
                Else
                    Throw New IllegalStateException("Component must have a valid peer")
                End If
            End Sub

            Friend Overridable Sub flipSubRegion(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal flipAction As BufferCapabilities.FlipContents)
                If outerInstance.peer IsNot Nothing Then
                    outerInstance.peer.flip(x1, y1, x2, y2, flipAction)
                Else
                    Throw New IllegalStateException("Component must have a valid peer")
                End If
            End Sub

            ''' <summary>
            ''' Destroys the buffers created through this object
            ''' </summary>
            Protected Friend Overridable Sub destroyBuffers()
                sun.awt.image.VSyncedBSManager.releaseVsync(Me)
                If outerInstance.peer IsNot Nothing Then
                    outerInstance.peer.destroyBuffers()
                Else
                    Throw New IllegalStateException("Component must have a valid peer")
                End If
            End Sub

            ''' <returns> the buffering capabilities of this strategy </returns>
            Public Property Overrides capabilities As BufferCapabilities
                Get
                    If TypeOf caps Is ProxyCapabilities Then
                        Return CType(caps, ProxyCapabilities).orig
                    Else
                        Return caps
                    End If
                End Get
            End Property

            ''' <returns> the graphics on the drawing buffer.  This method may not
            ''' be synchronized for performance reasons; use of this method by multiple
            ''' threads should be handled at the application level.  Disposal of the
            ''' graphics object must be handled by the application. </returns>
            Public Property Overrides drawGraphics As Graphics
                Get
                    revalidate()
                    Return drawBuffer.graphics
                End Get
            End Property

            ''' <summary>
            ''' Restore the drawing buffer if it has been lost
            ''' </summary>
            Protected Friend Overridable Sub revalidate()
                revalidate(True)
            End Sub

            Friend Overridable Sub revalidate(ByVal checkSize As Boolean)
                validatedContents = False

                If checkSize AndAlso (outerInstance.width <> width OrElse outerInstance.height <> height) Then
                    ' component has been resized; recreate the backbuffers
                    Try
                        createBuffers(numBuffers, caps)
                    Catch e As AWTException
                        ' shouldn't be possible
                    End Try
                    validatedContents = True
                End If

                ' get the buffers from the peer every time since they
                ' might have been replaced in response to a display change event
                updateInternalBuffers()

                ' now validate the backbuffer
                If drawVBuffer IsNot Nothing Then
                    Dim gc As GraphicsConfiguration = outerInstance.graphicsConfiguration_NoClientCode
                    Dim returnCode As Integer = drawVBuffer.validate(gc)
                    If returnCode = java.awt.image.VolatileImage.IMAGE_INCOMPATIBLE Then
                        Try
                            createBuffers(numBuffers, caps)
                        Catch e As AWTException
                            ' shouldn't be possible
                        End Try
                        If drawVBuffer IsNot Nothing Then drawVBuffer.validate(gc)
                        validatedContents = True
                    ElseIf returnCode = java.awt.image.VolatileImage.IMAGE_RESTORED Then
                        validatedContents = True
                    End If
                End If
            End Sub

            ''' <returns> whether the drawing buffer was lost since the last call to
            ''' <code>getDrawGraphics</code> </returns>
            Public Overrides Function contentsLost() As Boolean
                If drawVBuffer Is Nothing Then Return False
                Return drawVBuffer.contentsLost()
            End Function

            ''' <returns> whether the drawing buffer was recently restored from a lost
            ''' state and reinitialized to the default background color (white) </returns>
            Public Overrides Function contentsRestored() As Boolean
                Return validatedContents
            End Function

            ''' <summary>
            ''' Makes the next available buffer visible by either blitting or
            ''' flipping.
            ''' </summary>
            Public Overrides Sub show()
                flip(caps.flipContents)
            End Sub

            ''' <summary>
            ''' Makes specified region of the the next available buffer visible
            ''' by either blitting or flipping.
            ''' </summary>
            Friend Overridable Sub showSubRegion(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
                flipSubRegion(x1, y1, x2, y2, caps.flipContents)
            End Sub

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.6
            ''' </summary>
            Public Overrides Sub dispose()
                If outerInstance.bufferStrategy Is Me Then
                    outerInstance.bufferStrategy = Nothing
                    If outerInstance.peer IsNot Nothing Then destroyBuffers()
                End If
            End Sub

        End Class ' Inner class FlipBufferStrategy

        ''' <summary>
        ''' Inner class for blitting offscreen surfaces to a component.
        ''' 
        ''' @author Michael Martak
        ''' @since 1.4
        ''' </summary>
        Protected Friend Class BltBufferStrategy
            Inherits java.awt.image.BufferStrategy

            Private ReadOnly outerInstance As Component


            ''' <summary>
            ''' The buffering capabilities
            ''' </summary>
            Protected Friend caps As BufferCapabilities ' = null
            ''' <summary>
            ''' The back buffers
            ''' </summary>
            Protected Friend backBuffers As java.awt.image.VolatileImage() ' = null
            ''' <summary>
            ''' Whether or not the drawing buffer has been recently restored from
            ''' a lost state.
            ''' </summary>
            Protected Friend validatedContents As Boolean ' = false
            ''' <summary>
            ''' Size of the back buffers
            ''' </summary>
            Protected Friend width As Integer
            Protected Friend height As Integer

            ''' <summary>
            ''' Insets for the hosting Component.  The size of the back buffer
            ''' is constrained by these.
            ''' </summary>
            Private insets As Insets

            ''' <summary>
            ''' Creates a new blt buffer strategy around a component </summary>
            ''' <param name="numBuffers"> number of buffers to create, including the
            ''' front buffer </param>
            ''' <param name="caps"> the capabilities of the buffers </param>
            Protected Friend Sub New(ByVal outerInstance As Component, ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
                Me.outerInstance = outerInstance
                Me.caps = caps
                createBackBuffers(numBuffers - 1)
            End Sub

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.6
            ''' </summary>
            Public Overrides Sub dispose()
                If backBuffers IsNot Nothing Then
                    For counter As Integer = backBuffers.Length - 1 To 0 Step -1
                        If backBuffers(counter) IsNot Nothing Then
                            backBuffers(counter).flush()
                            backBuffers(counter) = Nothing
                        End If
                    Next counter
                End If
                If outerInstance.bufferStrategy Is Me Then outerInstance.bufferStrategy = Nothing
            End Sub

            ''' <summary>
            ''' Creates the back buffers
            ''' </summary>
            Protected Friend Overridable Sub createBackBuffers(ByVal numBuffers As Integer)
                If numBuffers = 0 Then
                    backBuffers = Nothing
                Else
                    ' save the current bounds
                    width = outerInstance.width
                    height = outerInstance.height
                    insets = outerInstance.insets_NoClientCode
                    Dim iWidth As Integer = width - insets.left - insets.right
                    Dim iHeight As Integer = height - insets.top - insets.bottom

                    ' It is possible for the component's width and/or height
                    ' to be 0 here.  Force the size of the backbuffers to
                    ' be > 0 so that creating the image won't fail.
                    iWidth = System.Math.Max(1, iWidth)
                    iHeight = System.Math.Max(1, iHeight)
                    If backBuffers Is Nothing Then
                        backBuffers = New java.awt.image.VolatileImage(numBuffers - 1) {}
                    Else
                        ' flush any existing backbuffers
                        For i As Integer = 0 To numBuffers - 1
                            If backBuffers(i) IsNot Nothing Then
                                backBuffers(i).flush()
                                backBuffers(i) = Nothing
                            End If
                        Next i
                    End If

                    ' create the backbuffers
                    For i As Integer = 0 To numBuffers - 1
                        backBuffers(i) = outerInstance.createVolatileImage(iWidth, iHeight)
                    Next i
                End If
            End Sub

            ''' <returns> the buffering capabilities of this strategy </returns>
            Public Property Overrides capabilities As BufferCapabilities
                Get
                    Return caps
                End Get
            End Property

            ''' <returns> the draw graphics </returns>
            Public Property Overrides drawGraphics As Graphics
                Get
                    revalidate()
                    Dim backBuffer_Renamed As image = backBuffer
                    If backBuffer_Renamed Is Nothing Then Return outerInstance.graphics
                    Dim g As sun.java2d.SunGraphics2D = CType(backBuffer_Renamed.graphics, sun.java2d.SunGraphics2D)
                    g.constrain(-insets.left, -insets.top, backBuffer_Renamed.getWidth(Nothing) + insets.left, backBuffer_Renamed.getHeight(Nothing) + insets.top)
                    Return g
                End Get
            End Property

            ''' <returns> direct access to the back buffer, as an image.
            ''' If there is no back buffer, returns null. </returns>
            Friend Overridable Property backBuffer As image
                Get
                    If backBuffers IsNot Nothing Then
                        Return backBuffers(backBuffers.Length - 1)
                    Else
                        Return Nothing
                    End If
                End Get
            End Property

            ''' <summary>
            ''' Makes the next available buffer visible.
            ''' </summary>
            Public Overrides Sub show()
                showSubRegion(insets.left, insets.top, width - insets.right, height - insets.bottom)
            End Sub

            ''' <summary>
            ''' Package-private method to present a specific rectangular area
            ''' of this buffer.  This class currently shows only the entire
            ''' buffer, by calling showSubRegion() with the full dimensions of
            ''' the buffer.  Subclasses (e.g., BltSubRegionBufferStrategy
            ''' and FlipSubRegionBufferStrategy) may have region-specific show
            ''' methods that call this method with actual sub regions of the
            ''' buffer.
            ''' </summary>
            Friend Overridable Sub showSubRegion(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
                If backBuffers Is Nothing Then Return
                ' Adjust location to be relative to client area.
                x1 -= insets.left
                x2 -= insets.left
                y1 -= insets.top
                y2 -= insets.top
                Dim g As Graphics = outerInstance.graphics_NoClientCode
                If g Is Nothing Then Return
                Try
                    ' First image copy is in terms of Frame's coordinates, need
                    ' to translate to client area.
                    g.translate(insets.left, insets.top)
                    For i As Integer = 0 To backBuffers.Length - 1
                        g.drawImage(backBuffers(i), x1, y1, x2, y2, x1, y1, x2, y2, Nothing)
                        g.Dispose()
                        g = Nothing
                        g = backBuffers(i).graphics
                    Next i
                Finally
                    If g IsNot Nothing Then g.Dispose()
                End Try
            End Sub

            ''' <summary>
            ''' Restore the drawing buffer if it has been lost
            ''' </summary>
            Protected Friend Overridable Sub revalidate()
                revalidate(True)
            End Sub

            Friend Overridable Sub revalidate(ByVal checkSize As Boolean)
                validatedContents = False

                If backBuffers Is Nothing Then Return

                If checkSize Then
                    Dim insets_Renamed As Insets = outerInstance.insets_NoClientCode
                    If outerInstance.width <> width OrElse outerInstance.height <> height OrElse (Not insets_Renamed.Equals(Me.insets)) Then
                        ' component has been resized; recreate the backbuffers
                        createBackBuffers(backBuffers.Length)
                        validatedContents = True
                    End If
                End If

                ' now validate the backbuffer
                Dim gc As GraphicsConfiguration = outerInstance.graphicsConfiguration_NoClientCode
                Dim returnCode As Integer = backBuffers(backBuffers.Length - 1).validate(gc)
                If returnCode = java.awt.image.VolatileImage.IMAGE_INCOMPATIBLE Then
                    If checkSize Then
                        createBackBuffers(backBuffers.Length)
                        ' backbuffers were recreated, so validate again
                        backBuffers(backBuffers.Length - 1).validate(gc)
                    End If
                    ' else case means we're called from Swing on the toolkit
                    ' thread, don't recreate buffers as that'll deadlock
                    ' (creating VolatileImages invokes getting GraphicsConfig
                    ' which grabs treelock).
                    validatedContents = True
                ElseIf returnCode = java.awt.image.VolatileImage.IMAGE_RESTORED Then
                    validatedContents = True
                End If
            End Sub

            ''' <returns> whether the drawing buffer was lost since the last call to
            ''' <code>getDrawGraphics</code> </returns>
            Public Overrides Function contentsLost() As Boolean
                If backBuffers Is Nothing Then
                    Return False
                Else
                    Return backBuffers(backBuffers.Length - 1).contentsLost()
                End If
            End Function

            ''' <returns> whether the drawing buffer was recently restored from a lost
            ''' state and reinitialized to the default background color (white) </returns>
            Public Overrides Function contentsRestored() As Boolean
                Return validatedContents
            End Function
        End Class ' Inner class BltBufferStrategy

        ''' <summary>
        ''' Private class to perform sub-region flipping.
        ''' </summary>
        Private Class FlipSubRegionBufferStrategy
            Inherits FlipBufferStrategy
            Implements sun.awt.SubRegionShowable

            Private ReadOnly outerInstance As Component


            Protected Friend Sub New(ByVal outerInstance As Component, ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
                Me.outerInstance = outerInstance
                MyBase.New(numBuffers, caps)
            End Sub

            Public Overridable Sub show(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
                showSubRegion(x1, y1, x2, y2)
            End Sub

            ' This is invoked by Swing on the toolkit thread.
            Public Overridable Function showIfNotLost(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer) As Boolean
                If Not contentsLost() Then
                    showSubRegion(x1, y1, x2, y2)
                    Return Not contentsLost()
                End If
                Return False
            End Function
        End Class

        ''' <summary>
        ''' Private class to perform sub-region blitting.  Swing will use
        ''' this subclass via the SubRegionShowable interface in order to
        ''' copy only the area changed during a repaint.
        ''' See javax.swing.BufferStrategyPaintManager.
        ''' </summary>
        Private Class BltSubRegionBufferStrategy
            Inherits BltBufferStrategy
            Implements sun.awt.SubRegionShowable

            Private ReadOnly outerInstance As Component


            Protected Friend Sub New(ByVal outerInstance As Component, ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
                Me.outerInstance = outerInstance
                MyBase.New(numBuffers, caps)
            End Sub

            Public Overridable Sub show(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
                showSubRegion(x1, y1, x2, y2)
            End Sub

            ' This method is called by Swing on the toolkit thread.
            Public Overridable Function showIfNotLost(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer) As Boolean
                If Not contentsLost() Then
                    showSubRegion(x1, y1, x2, y2)
                    Return Not contentsLost()
                End If
                Return False
            End Function
        End Class

        ''' <summary>
        ''' Inner class for flipping buffers on a component.  That component must
        ''' be a <code>Canvas</code> or <code>Window</code>. </summary>
        ''' <seealso cref= Canvas </seealso>
        ''' <seealso cref= Window </seealso>
        ''' <seealso cref= java.awt.image.BufferStrategy
        ''' @author Michael Martak
        ''' @since 1.4 </seealso>
        Private Class SingleBufferStrategy
            Inherits java.awt.image.BufferStrategy

            Private ReadOnly outerInstance As Component


            Private caps As BufferCapabilities

            Public Sub New(ByVal outerInstance As Component, ByVal caps As BufferCapabilities)
                Me.outerInstance = outerInstance
                Me.caps = caps
            End Sub
            Public Property Overrides capabilities As BufferCapabilities
                Get
                    Return caps
                End Get
            End Property
            Public Property Overrides drawGraphics As Graphics
                Get
                    Return outerInstance.graphics
                End Get
            End Property
            Public Overrides Function contentsLost() As Boolean
                Return False
            End Function
            Public Overrides Function contentsRestored() As Boolean
                Return False
            End Function
            Public Overrides Sub show()
                ' Do nothing
            End Sub
        End Class ' Inner class SingleBufferStrategy

        ''' <summary>
        ''' Sets whether or not paint messages received from the operating system
        ''' should be ignored.  This does not affect paint events generated in
        ''' software by the AWT, unless they are an immediate response to an
        ''' OS-level paint message.
        ''' <p>
        ''' This is useful, for example, if running under full-screen mode and
        ''' better performance is desired, or if page-flipping is used as the
        ''' buffer strategy.
        ''' 
        ''' @since 1.4 </summary>
        ''' <seealso cref= #getIgnoreRepaint </seealso>
        ''' <seealso cref= Canvas#createBufferStrategy </seealso>
        ''' <seealso cref= Window#createBufferStrategy </seealso>
        ''' <seealso cref= java.awt.image.BufferStrategy </seealso>
        ''' <seealso cref= GraphicsDevice#setFullScreenWindow </seealso>
        Public Overridable Property ignoreRepaint As Boolean
            Set(ByVal ignoreRepaint As Boolean)
                Me.ignoreRepaint = ignoreRepaint
            End Set
            Get
                Return ignoreRepaint
            End Get
        End Property


        ''' <summary>
        ''' Checks whether this component "contains" the specified point,
        ''' where <code>x</code> and <code>y</code> are defined to be
        ''' relative to the coordinate system of this component. </summary>
        ''' <param name="x">   the <i>x</i> coordinate of the point </param>
        ''' <param name="y">   the <i>y</i> coordinate of the point </param>
        ''' <seealso cref=       #getComponentAt(int, int)
        ''' @since     JDK1.1 </seealso>
        Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return inside(x, y)
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by contains(int, int). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function inside(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return (x >= 0) AndAlso (x < width) AndAlso (y >= 0) AndAlso (y < height)
        End Function

        ''' <summary>
        ''' Checks whether this component "contains" the specified point,
        ''' where the point's <i>x</i> and <i>y</i> coordinates are defined
        ''' to be relative to the coordinate system of this component. </summary>
        ''' <param name="p">     the point </param>
        ''' <exception cref="NullPointerException"> if {@code p} is {@code null} </exception>
        ''' <seealso cref=       #getComponentAt(Point)
        ''' @since     JDK1.1 </seealso>
        Public Overridable Function contains(ByVal p As Point) As Boolean
            Return contains(p.x, p.y)
        End Function

        ''' <summary>
        ''' Determines if this component or one of its immediate
        ''' subcomponents contains the (<i>x</i>,&nbsp;<i>y</i>) location,
        ''' and if so, returns the containing component. This method only
        ''' looks one level deep. If the point (<i>x</i>,&nbsp;<i>y</i>) is
        ''' inside a subcomponent that itself has subcomponents, it does not
        ''' go looking down the subcomponent tree.
        ''' <p>
        ''' The <code>locate</code> method of <code>Component</code> simply
        ''' returns the component itself if the (<i>x</i>,&nbsp;<i>y</i>)
        ''' coordinate location is inside its bounding box, and <code>null</code>
        ''' otherwise. </summary>
        ''' <param name="x">   the <i>x</i> coordinate </param>
        ''' <param name="y">   the <i>y</i> coordinate </param>
        ''' <returns>    the component or subcomponent that contains the
        '''                (<i>x</i>,&nbsp;<i>y</i>) location;
        '''                <code>null</code> if the location
        '''                is outside this component </returns>
        ''' <seealso cref=       #contains(int, int)
        ''' @since     JDK1.0 </seealso>
        Public Overridable Function getComponentAt(ByVal x As Integer, ByVal y As Integer) As Component
            Return locate(x, y)
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by getComponentAt(int, int). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function locate(ByVal x As Integer, ByVal y As Integer) As Component
            Return If(contains(x, y), Me, Nothing)
        End Function

        ''' <summary>
        ''' Returns the component or subcomponent that contains the
        ''' specified point. </summary>
        ''' <param name="p">   the point </param>
        ''' <seealso cref=       java.awt.Component#contains
        ''' @since     JDK1.1 </seealso>
        Public Overridable Function getComponentAt(ByVal p As Point) As Component
            Return getComponentAt(p.x, p.y)
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by <code>dispatchEvent(AWTEvent e)</code>. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub deliverEvent(ByVal e As [event])
            postEvent(e)
        End Sub

        ''' <summary>
        ''' Dispatches an event to this component or one of its sub components.
        ''' Calls <code>processEvent</code> before returning for 1.1-style
        ''' events which have been enabled for the <code>Component</code>. </summary>
        ''' <param name="e"> the event </param>
        Public Sub dispatchEvent(ByVal e As AWTEvent)
            dispatchEventImpl(e)
        End Sub

        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Friend Overridable Sub dispatchEventImpl(ByVal e As AWTEvent)
            Dim id As Integer = e.id

            ' Check that this component belongs to this app-context
            Dim compContext As sun.awt.AppContext = appContext
            If compContext IsNot Nothing AndAlso (Not compContext.Equals(sun.awt.AppContext.appContext)) Then
                If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("Event " & e & " is being dispatched on the wrong AppContext")
            End If

            If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then eventLog.finest("{0}", e)

            '        
            '         * 0. Set timestamp and modifiers of current event.
            '         
            If Not (TypeOf e Is KeyEvent) Then EventQueue.currentEventAndMostRecentTime = e

            '        
            '         * 1. Pre-dispatchers. Do any necessary retargeting/reordering here
            '         *    before we notify AWTEventListeners.
            '         

            If TypeOf e Is sun.awt.dnd.SunDropTargetEvent Then
                CType(e, sun.awt.dnd.SunDropTargetEvent).dispatch()
                Return
            End If

            If Not e.focusManagerIsDispatching Then
                ' Invoke the private focus retargeting method which provides
                ' lightweight Component support
                If e.isPosted Then
                    e = KeyboardFocusManager.retargetFocusEvent(e)
                    e.isPosted = True
                End If

                ' Now, with the event properly targeted to a lightweight
                ' descendant if necessary, invoke the public focus retargeting
                ' and dispatching function
                If KeyboardFocusManager.currentKeyboardFocusManager.dispatchEvent(e) Then Return
            End If
            If (TypeOf e Is FocusEvent) AndAlso focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("" & e)
            ' MouseWheel may need to be retargeted here so that
            ' AWTEventListener sees the event go to the correct
            ' Component.  If the MouseWheelEvent needs to go to an ancestor,
            ' the event is dispatched to the ancestor, and dispatching here
            ' stops.
            If id = MouseEvent.MOUSE_WHEEL AndAlso ((Not eventTypeEnabled(id))) AndAlso (peer IsNot Nothing AndAlso (Not peer.handlesWheelScrolling())) AndAlso (dispatchMouseWheelToAncestor(CType(e, MouseWheelEvent))) Then Return

            '        
            '         * 2. Allow the Toolkit to pass this to AWTEventListeners.
            '         
            Dim toolkit_Renamed As Toolkit = toolkit.defaultToolkit
            toolkit_Renamed.notifyAWTEventListeners(e)


            '        
            '         * 3. If no one has consumed a key event, allow the
            '         *    KeyboardFocusManager to process it.
            '         
            If Not e.consumed Then
                If TypeOf e Is java.awt.event.KeyEvent Then
                    KeyboardFocusManager.currentKeyboardFocusManager.processKeyEvent(Me, CType(e, KeyEvent))
                    If e.consumed Then Return
                End If
            End If

            '        
            '         * 4. Allow input methods to process the event
            '         
            If areInputMethodsEnabled() Then
                ' We need to pass on InputMethodEvents since some host
                ' input method adapters send them through the Java
                ' event queue instead of directly to the component,
                ' and the input context also handles the Java composition window
                If ((TypeOf e Is InputMethodEvent) AndAlso Not (TypeOf Me Is sun.awt.im.CompositionArea)) OrElse (TypeOf e Is InputEvent) OrElse (TypeOf e Is FocusEvent) Then
                    ' Otherwise, we only pass on input and focus events, because
                    ' a) input methods shouldn't know about semantic or component-level events
                    ' b) passing on the events takes time
                    ' c) isConsumed() is always true for semantic events.
                    Dim inputContext_Renamed As java.awt.im.InputContext = inputContext


                    If inputContext_Renamed IsNot Nothing Then
                        inputContext_Renamed.dispatchEvent(e)
                        If e.consumed Then
                            If (TypeOf e Is FocusEvent) AndAlso focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("3579: Skipping " & e)
                            Return
                        End If
                    End If
                End If
            Else
                ' When non-clients get focus, we need to explicitly disable the native
                ' input method. The native input method is actually not disabled when
                ' the active/passive/peered clients loose focus.
                If id = FocusEvent.FOCUS_GAINED Then
                    Dim inputContext_Renamed As java.awt.im.InputContext = inputContext
                    If inputContext_Renamed IsNot Nothing AndAlso TypeOf inputContext_Renamed Is sun.awt.im.InputContext Then CType(inputContext_Renamed, sun.awt.im.InputContext).disableNativeIM()
                End If
            End If


            '        
            '         * 5. Pre-process any special events before delivery
            '         
            Select Case id
                ' Handling of the PAINT and UPDATE events is now done in the
                ' peer's handleEvent() method so the background can be cleared
                ' selectively for non-native components on Windows only.
                ' - Fred.Ecks@Eng.sun.com, 5-8-98

                Case KeyEvent.KEY_PRESSED, KeyEvent.KEY_RELEASED
                    Dim p As Container = CType(If(TypeOf Me Is Container, Me, parent), Container)
                    If p IsNot Nothing Then
                        p.preProcessKeyEvent(CType(e, KeyEvent))
                        If e.consumed Then
                            If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Pre-process consumed event")
                            Return
                        End If
                    End If

                Case WindowEvent.WINDOW_CLOSING
                    If TypeOf toolkit_Renamed Is sun.awt.WindowClosingListener Then
                        windowClosingException = CType(toolkit_Renamed, sun.awt.WindowClosingListener).windowClosingNotify(CType(e, WindowEvent))
                        If checkWindowClosingException() Then Return
                    End If

                Case Else
            End Select

            '        
            '         * 6. Deliver event for normal processing
            '         
            If newEventsOnly Then
                ' Filtering needs to really be moved to happen at a lower
                ' level in order to get maximum performance gain;  it is
                ' here temporarily to ensure the API spec is honored.
                '
                If eventEnabled(e) Then processEvent(e)
            ElseIf id = MouseEvent.MOUSE_WHEEL Then
                ' newEventsOnly will be false for a listenerless ScrollPane, but
                ' MouseWheelEvents still need to be dispatched to it so scrolling
                ' can be done.
                autoProcessMouseWheel(CType(e, MouseWheelEvent))
            ElseIf Not (TypeOf e Is MouseEvent AndAlso (Not postsOldMouseEvents())) Then
                '
                ' backward compatibility
                '
                Dim olde As [event] = e.convertToOld()
                If olde IsNot Nothing Then
                    Dim key As Integer = olde.key
                    Dim modifiers As Integer = olde.modifiers

                    postEvent(olde)
                    If olde.consumed Then e.consume()
                    ' if target changed key or modifier values, copy them
                    ' back to original event
                    '
                    Select Case olde.id
                        Case Event.KEY_PRESS, Event.KEY_RELEASE, Event.KEY_ACTION, Event.KEY_ACTION_RELEASE
						  If olde.key <> key Then CType(e, KeyEvent).keyChar = olde.keyEventChar
                            If olde.modifiers <> modifiers Then CType(e, KeyEvent).modifiers = olde.modifiers
                        Case Else
                    End Select
                End If
            End If

            '        
            '         * 8. Special handling for 4061116 : Hook for browser to close modal
            '         *    dialogs.
            '         
            If id = WindowEvent.WINDOW_CLOSING AndAlso (Not e.consumed) Then
                If TypeOf toolkit_Renamed Is sun.awt.WindowClosingListener Then
                    windowClosingException = CType(toolkit_Renamed, sun.awt.WindowClosingListener).windowClosingDelivered(CType(e, WindowEvent))
                    If checkWindowClosingException() Then Return
                End If
            End If

            '        
            '         * 9. Allow the peer to process the event.
            '         * Except KeyEvents, they will be processed by peer after
            '         * all KeyEventPostProcessors
            '         * (see DefaultKeyboardFocusManager.dispatchKeyEvent())
            '         
            If Not (TypeOf e Is KeyEvent) Then
                Dim tpeer As java.awt.peer.ComponentPeer = peer
                If TypeOf e Is FocusEvent AndAlso (tpeer Is Nothing OrElse TypeOf tpeer Is java.awt.peer.LightweightPeer) Then
                    ' if focus owner is lightweight then its native container
                    ' processes event
                    Dim source As Component = CType(e.source, Component)
                    If source IsNot Nothing Then
                        Dim target As Container = source.nativeContainer
                        If target IsNot Nothing Then tpeer = target.peer
                    End If
                End If
                If tpeer IsNot Nothing Then tpeer.handleEvent(e)
            End If
        End Sub ' dispatchEventImpl()

        '    
        '     * If newEventsOnly is false, method is called so that ScrollPane can
        '     * override it and handle common-case mouse wheel scrolling.  NOP
        '     * for Component.
        '     
        Friend Overridable Sub autoProcessMouseWheel(ByVal e As MouseWheelEvent)
        End Sub

        '    
        '     * Dispatch given MouseWheelEvent to the first ancestor for which
        '     * MouseWheelEvents are enabled.
        '     *
        '     * Returns whether or not event was dispatched to an ancestor
        '     
        Friend Overridable Function dispatchMouseWheelToAncestor(ByVal e As MouseWheelEvent) As Boolean
            Dim newX, newY As Integer
            newX = e.x + x ' Coordinates take into account at least
            newY = e.y + y ' the cursor's position relative to this
            ' Component (e.getX()), and this Component's
            ' position relative to its parent.
            Dim newMWE As MouseWheelEvent

            If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then
                eventLog.finest("dispatchMouseWheelToAncestor")
                eventLog.finest("orig event src is of " & e.source.GetType())
            End If

            '         parent field for Window refers to the owning Window.
            '         * MouseWheelEvents should NOT be propagated into owning Windows
            '         
            SyncLock treeLock
                Dim anc As Container = parent
                Do While anc IsNot Nothing AndAlso Not anc.eventEnabled(e)
                    ' fix coordinates to be relative to new event source
                    newX += anc.x
                    newY += anc.y

                    If Not (TypeOf anc Is Window) Then
                        anc = anc.parent
                    Else
                        Exit Do
                    End If
                Loop

                If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then eventLog.finest("new event src is " & anc.GetType())

                If anc IsNot Nothing AndAlso anc.eventEnabled(e) Then
                    ' Change event to be from new source, with new x,y
                    ' For now, just create a new event - yucky

                    newMWE = New MouseWheelEvent(anc, e.iD, e.when, e.modifiers, newX, newY, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, e.scrollType, e.scrollAmount, e.wheelRotation, e.preciseWheelRotation) ' y relative to new source -  x relative to new source -  new source
                    CType(e, AWTEvent).copyPrivateDataInto(newMWE)
                    ' When dispatching a wheel event to
                    ' ancestor, there is no need trying to find descendant
                    ' lightweights to dispatch event to.
                    ' If we dispatch the event to toplevel ancestor,
                    ' this could encolse the loop: 6480024.
                    anc.dispatchEventToSelf(newMWE)
                    If newMWE.consumed Then e.consume()
                    Return True
                End If
            End SyncLock
            Return False
        End Function

        Friend Overridable Function checkWindowClosingException() As Boolean
            If windowClosingException IsNot Nothing Then
                If TypeOf Me Is Dialog Then
                    CType(Me, Dialog).interruptBlocking()
                Else
                    windowClosingException.fillInStackTrace()
                    Console.WriteLine(windowClosingException.ToString())
                    Console.Write(windowClosingException.stackTrace)
                    windowClosingException = Nothing
                End If
                Return True
            End If
            Return False
        End Function

        Friend Overridable Function areInputMethodsEnabled() As Boolean
            ' in 1.2, we assume input method support is required for all
            ' components that handle key events, but components can turn off
            ' input methods by calling enableInputMethods(false).
            Return ((eventMask And AWTEvent.INPUT_METHODS_ENABLED_MASK) <> 0) AndAlso ((eventMask And AWTEvent.KEY_EVENT_MASK) <> 0 OrElse keyListener IsNot Nothing)
        End Function

        ' REMIND: remove when filtering is handled at lower level
        Friend Overridable Function eventEnabled(ByVal e As AWTEvent) As Boolean
            Return eventTypeEnabled(e.id)
        End Function

        Friend Overridable Function eventTypeEnabled(ByVal type As Integer) As Boolean
            Select Case type
                Case ComponentEvent.COMPONENT_MOVED, ComponentEvent.COMPONENT_RESIZED, ComponentEvent.COMPONENT_SHOWN, ComponentEvent.COMPONENT_HIDDEN
                    If (eventMask And AWTEvent.COMPONENT_EVENT_MASK) <> 0 OrElse componentListener IsNot Nothing Then Return True
                Case FocusEvent.FOCUS_GAINED, FocusEvent.FOCUS_LOST
                    If (eventMask And AWTEvent.FOCUS_EVENT_MASK) <> 0 OrElse focusListener IsNot Nothing Then Return True
                Case KeyEvent.KEY_PRESSED, KeyEvent.KEY_RELEASED, KeyEvent.KEY_TYPED
                    If (eventMask And AWTEvent.KEY_EVENT_MASK) <> 0 OrElse keyListener IsNot Nothing Then Return True
                Case MouseEvent.MOUSE_PRESSED, MouseEvent.MOUSE_RELEASED, MouseEvent.MOUSE_ENTERED, MouseEvent.MOUSE_EXITED, MouseEvent.MOUSE_CLICKED
                    If (eventMask And AWTEvent.MOUSE_EVENT_MASK) <> 0 OrElse mouseListener IsNot Nothing Then Return True
                Case MouseEvent.MOUSE_MOVED, MouseEvent.MOUSE_DRAGGED
                    If (eventMask And AWTEvent.MOUSE_MOTION_EVENT_MASK) <> 0 OrElse mouseMotionListener IsNot Nothing Then Return True
                Case MouseEvent.MOUSE_WHEEL
                    If (eventMask And AWTEvent.MOUSE_WHEEL_EVENT_MASK) <> 0 OrElse mouseWheelListener IsNot Nothing Then Return True
                Case InputMethodEvent.INPUT_METHOD_TEXT_CHANGED, InputMethodEvent.CARET_POSITION_CHANGED
                    If (eventMask And AWTEvent.INPUT_METHOD_EVENT_MASK) <> 0 OrElse inputMethodListener IsNot Nothing Then Return True
                Case HierarchyEvent.HIERARCHY_CHANGED
                    If (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 OrElse hierarchyListener IsNot Nothing Then Return True
                Case HierarchyEvent.ANCESTOR_MOVED, HierarchyEvent.ANCESTOR_RESIZED
                    If (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 OrElse hierarchyBoundsListener IsNot Nothing Then Return True
                Case ActionEvent.ACTION_PERFORMED
                    If (eventMask And AWTEvent.ACTION_EVENT_MASK) <> 0 Then Return True
                Case TextEvent.TEXT_VALUE_CHANGED
                    If (eventMask And AWTEvent.TEXT_EVENT_MASK) <> 0 Then Return True
                Case ItemEvent.ITEM_STATE_CHANGED
                    If (eventMask And AWTEvent.ITEM_EVENT_MASK) <> 0 Then Return True
                Case AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED
                    If (eventMask And AWTEvent.ADJUSTMENT_EVENT_MASK) <> 0 Then Return True
                Case Else
            End Select
            '
            ' Always pass on events defined by external programs.
            '
            If type > AWTEvent.RESERVED_ID_MAX Then Return True
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by dispatchEvent(AWTEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function postEvent(ByVal e As [event]) As Boolean Implements MenuContainer.postEvent
            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer

            If handleEvent(e) Then
                e.consume()
                Return True
            End If

            Dim parent_Renamed As Component = Me.parent
            Dim eventx As Integer = e.x
            Dim eventy As Integer = e.y
            If parent_Renamed IsNot Nothing Then
                e.translate(x, y)
                If parent_Renamed.postEvent(e) Then
                    e.consume()
                    Return True
                End If
                ' restore coords
                e.x = eventx
                e.y = eventy
            End If
            Return False
        End Function

        ' Event source interfaces

        ''' <summary>
        ''' Adds the specified component listener to receive component events from
        ''' this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the component listener </param>
        ''' <seealso cref=      java.awt.event.ComponentEvent </seealso>
        ''' <seealso cref=      java.awt.event.ComponentListener </seealso>
        ''' <seealso cref=      #removeComponentListener </seealso>
        ''' <seealso cref=      #getComponentListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addComponentListener(ByVal l As ComponentListener)
            If l Is Nothing Then Return
            componentListener = AWTEventMulticaster.add(componentListener, l)
            newEventsOnly = True
        End Sub

        ''' <summary>
        ''' Removes the specified component listener so that it no longer
        ''' receives component events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model. </summary>
        ''' <param name="l">   the component listener </param>
        ''' <seealso cref=      java.awt.event.ComponentEvent </seealso>
        ''' <seealso cref=      java.awt.event.ComponentListener </seealso>
        ''' <seealso cref=      #addComponentListener </seealso>
        ''' <seealso cref=      #getComponentListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeComponentListener(ByVal l As ComponentListener)
            If l Is Nothing Then Return
            componentListener = AWTEventMulticaster.remove(componentListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the component listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all <code>ComponentListener</code>s of this component
        '''         or an empty array if no component
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref= #addComponentListener </seealso>
        ''' <seealso cref= #removeComponentListener
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property componentListeners As ComponentListener()
            Get
                Return getListeners(GetType(ComponentListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified focus listener to receive focus events from
        ''' this component when this component gains input focus.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the focus listener </param>
        ''' <seealso cref=      java.awt.event.FocusEvent </seealso>
        ''' <seealso cref=      java.awt.event.FocusListener </seealso>
        ''' <seealso cref=      #removeFocusListener </seealso>
        ''' <seealso cref=      #getFocusListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addFocusListener(ByVal l As FocusListener)
            If l Is Nothing Then Return
            focusListener = AWTEventMulticaster.add(focusListener, l)
            newEventsOnly = True

            ' if this is a lightweight component, enable focus events
            ' in the native container.
            If TypeOf peer Is java.awt.peer.LightweightPeer Then parent.proxyEnableEvents(AWTEvent.FOCUS_EVENT_MASK)
        End Sub

        ''' <summary>
        ''' Removes the specified focus listener so that it no longer
        ''' receives focus events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the focus listener </param>
        ''' <seealso cref=      java.awt.event.FocusEvent </seealso>
        ''' <seealso cref=      java.awt.event.FocusListener </seealso>
        ''' <seealso cref=      #addFocusListener </seealso>
        ''' <seealso cref=      #getFocusListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeFocusListener(ByVal l As FocusListener)
            If l Is Nothing Then Return
            focusListener = AWTEventMulticaster.remove(focusListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the focus listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>FocusListener</code>s
        '''         or an empty array if no component
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref= #addFocusListener </seealso>
        ''' <seealso cref= #removeFocusListener
        ''' @since 1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property focusListeners As FocusListener()
            Get
                Return getListeners(GetType(FocusListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified hierarchy listener to receive hierarchy changed
        ''' events from this component when the hierarchy to which this container
        ''' belongs changes.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the hierarchy listener </param>
        ''' <seealso cref=      java.awt.event.HierarchyEvent </seealso>
        ''' <seealso cref=      java.awt.event.HierarchyListener </seealso>
        ''' <seealso cref=      #removeHierarchyListener </seealso>
        ''' <seealso cref=      #getHierarchyListeners
        ''' @since    1.3 </seealso>
        Public Overridable Sub addHierarchyListener(ByVal l As HierarchyListener)
            If l Is Nothing Then Return
            Dim notifyAncestors As Boolean
            SyncLock Me
                notifyAncestors = (hierarchyListener Is Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) = 0)
                hierarchyListener = AWTEventMulticaster.add(hierarchyListener, l)
                notifyAncestors = (notifyAncestors AndAlso hierarchyListener IsNot Nothing)
                newEventsOnly = True
            End SyncLock
            If notifyAncestors Then
                SyncLock treeLock
                    adjustListeningChildrenOnParent(AWTEvent.HIERARCHY_EVENT_MASK, 1)
                End SyncLock
            End If
        End Sub

        ''' <summary>
        ''' Removes the specified hierarchy listener so that it no longer
        ''' receives hierarchy changed events from this component. This method
        ''' performs no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the hierarchy listener </param>
        ''' <seealso cref=      java.awt.event.HierarchyEvent </seealso>
        ''' <seealso cref=      java.awt.event.HierarchyListener </seealso>
        ''' <seealso cref=      #addHierarchyListener </seealso>
        ''' <seealso cref=      #getHierarchyListeners
        ''' @since    1.3 </seealso>
        Public Overridable Sub removeHierarchyListener(ByVal l As HierarchyListener)
            If l Is Nothing Then Return
            Dim notifyAncestors As Boolean
            SyncLock Me
                notifyAncestors = (hierarchyListener IsNot Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) = 0)
                hierarchyListener = AWTEventMulticaster.remove(hierarchyListener, l)
                notifyAncestors = (notifyAncestors AndAlso hierarchyListener Is Nothing)
            End SyncLock
            If notifyAncestors Then
                SyncLock treeLock
                    adjustListeningChildrenOnParent(AWTEvent.HIERARCHY_EVENT_MASK, -1)
                End SyncLock
            End If
        End Sub

        ''' <summary>
        ''' Returns an array of all the hierarchy listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>HierarchyListener</code>s
        '''         or an empty array if no hierarchy
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addHierarchyListener </seealso>
        ''' <seealso cref=      #removeHierarchyListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property hierarchyListeners As HierarchyListener()
            Get
                Return getListeners(GetType(HierarchyListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified hierarchy bounds listener to receive hierarchy
        ''' bounds events from this component when the hierarchy to which this
        ''' container belongs changes.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the hierarchy bounds listener </param>
        ''' <seealso cref=      java.awt.event.HierarchyEvent </seealso>
        ''' <seealso cref=      java.awt.event.HierarchyBoundsListener </seealso>
        ''' <seealso cref=      #removeHierarchyBoundsListener </seealso>
        ''' <seealso cref=      #getHierarchyBoundsListeners
        ''' @since    1.3 </seealso>
        Public Overridable Sub addHierarchyBoundsListener(ByVal l As HierarchyBoundsListener)
            If l Is Nothing Then Return
            Dim notifyAncestors As Boolean
            SyncLock Me
                notifyAncestors = (hierarchyBoundsListener Is Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) = 0)
                hierarchyBoundsListener = AWTEventMulticaster.add(hierarchyBoundsListener, l)
                notifyAncestors = (notifyAncestors AndAlso hierarchyBoundsListener IsNot Nothing)
                newEventsOnly = True
            End SyncLock
            If notifyAncestors Then
                SyncLock treeLock
                    adjustListeningChildrenOnParent(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, 1)
                End SyncLock
            End If
        End Sub

        ''' <summary>
        ''' Removes the specified hierarchy bounds listener so that it no longer
        ''' receives hierarchy bounds events from this component. This method
        ''' performs no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the hierarchy bounds listener </param>
        ''' <seealso cref=      java.awt.event.HierarchyEvent </seealso>
        ''' <seealso cref=      java.awt.event.HierarchyBoundsListener </seealso>
        ''' <seealso cref=      #addHierarchyBoundsListener </seealso>
        ''' <seealso cref=      #getHierarchyBoundsListeners
        ''' @since    1.3 </seealso>
        Public Overridable Sub removeHierarchyBoundsListener(ByVal l As HierarchyBoundsListener)
            If l Is Nothing Then Return
            Dim notifyAncestors As Boolean
            SyncLock Me
                notifyAncestors = (hierarchyBoundsListener IsNot Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) = 0)
                hierarchyBoundsListener = AWTEventMulticaster.remove(hierarchyBoundsListener, l)
                notifyAncestors = (notifyAncestors AndAlso hierarchyBoundsListener Is Nothing)
            End SyncLock
            If notifyAncestors Then
                SyncLock treeLock
                    adjustListeningChildrenOnParent(AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK, -1)
                End SyncLock
            End If
        End Sub

        ' Should only be called while holding the tree lock
        Friend Overridable Function numListening(ByVal mask As Long) As Integer
            ' One mask or the other, but not neither or both.
            If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then
                If (mask <> AWTEvent.HIERARCHY_EVENT_MASK) AndAlso (mask <> AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) Then eventLog.fine("Assertion failed")
            End If
            If (mask = AWTEvent.HIERARCHY_EVENT_MASK AndAlso (hierarchyListener IsNot Nothing OrElse (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0)) OrElse (mask = AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK AndAlso (hierarchyBoundsListener IsNot Nothing OrElse (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0)) Then
                Return 1
            Else
                Return 0
            End If
        End Function

        ' Should only be called while holding tree lock
        Friend Overridable Function countHierarchyMembers() As Integer
            Return 1
        End Function
        ' Should only be called while holding the tree lock
        Friend Overridable Function createHierarchyEvents(ByVal id As Integer, ByVal changed As Component, ByVal changedParent As Container, ByVal changeFlags As Long, ByVal enabledOnToolkit As Boolean) As Integer
            Select Case id
                Case HierarchyEvent.HIERARCHY_CHANGED
                    If hierarchyListener IsNot Nothing OrElse (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 OrElse enabledOnToolkit Then
                        Dim e As New HierarchyEvent(Me, id, changed, changedParent, changeFlags)
                        dispatchEvent(e)
                        Return 1
                    End If
                Case HierarchyEvent.ANCESTOR_MOVED, HierarchyEvent.ANCESTOR_RESIZED
                    If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then
                        If changeFlags <> 0 Then eventLog.fine("Assertion (changeFlags == 0) failed")
                    End If
                    If hierarchyBoundsListener IsNot Nothing OrElse (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 OrElse enabledOnToolkit Then
                        Dim e As New HierarchyEvent(Me, id, changed, changedParent)
                        dispatchEvent(e)
                        Return 1
                    End If
                Case Else
                    ' assert false
                    If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("This code must never be reached")
            End Select
            Return 0
        End Function

        ''' <summary>
        ''' Returns an array of all the hierarchy bounds listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>HierarchyBoundsListener</code>s
        '''         or an empty array if no hierarchy bounds
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addHierarchyBoundsListener </seealso>
        ''' <seealso cref=      #removeHierarchyBoundsListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property hierarchyBoundsListeners As HierarchyBoundsListener()
            Get
                Return getListeners(GetType(HierarchyBoundsListener))
            End Get
        End Property

        '    
        '     * Should only be called while holding the tree lock.
        '     * It's added only for overriding in java.awt.Window
        '     * because parent in Window is owner.
        '     
        Friend Overridable Sub adjustListeningChildrenOnParent(ByVal mask As Long, ByVal num As Integer)
            If parent IsNot Nothing Then parent.adjustListeningChildren(mask, num)
        End Sub

        ''' <summary>
        ''' Adds the specified key listener to receive key events from
        ''' this component.
        ''' If l is null, no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the key listener. </param>
        ''' <seealso cref=      java.awt.event.KeyEvent </seealso>
        ''' <seealso cref=      java.awt.event.KeyListener </seealso>
        ''' <seealso cref=      #removeKeyListener </seealso>
        ''' <seealso cref=      #getKeyListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addKeyListener(ByVal l As KeyListener)
            If l Is Nothing Then Return
            keyListener = AWTEventMulticaster.add(keyListener, l)
            newEventsOnly = True

            ' if this is a lightweight component, enable key events
            ' in the native container.
            If TypeOf peer Is java.awt.peer.LightweightPeer Then parent.proxyEnableEvents(AWTEvent.KEY_EVENT_MASK)
        End Sub

        ''' <summary>
        ''' Removes the specified key listener so that it no longer
        ''' receives key events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the key listener </param>
        ''' <seealso cref=      java.awt.event.KeyEvent </seealso>
        ''' <seealso cref=      java.awt.event.KeyListener </seealso>
        ''' <seealso cref=      #addKeyListener </seealso>
        ''' <seealso cref=      #getKeyListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeKeyListener(ByVal l As KeyListener)
            If l Is Nothing Then Return
            keyListener = AWTEventMulticaster.remove(keyListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the key listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>KeyListener</code>s
        '''         or an empty array if no key
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addKeyListener </seealso>
        ''' <seealso cref=      #removeKeyListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property keyListeners As KeyListener()
            Get
                Return getListeners(GetType(KeyListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified mouse listener to receive mouse events from
        ''' this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the mouse listener </param>
        ''' <seealso cref=      java.awt.event.MouseEvent </seealso>
        ''' <seealso cref=      java.awt.event.MouseListener </seealso>
        ''' <seealso cref=      #removeMouseListener </seealso>
        ''' <seealso cref=      #getMouseListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addMouseListener(ByVal l As MouseListener)
            If l Is Nothing Then Return
            mouseListener = AWTEventMulticaster.add(mouseListener, l)
            newEventsOnly = True

            ' if this is a lightweight component, enable mouse events
            ' in the native container.
            If TypeOf peer Is java.awt.peer.LightweightPeer Then parent.proxyEnableEvents(AWTEvent.MOUSE_EVENT_MASK)
        End Sub

        ''' <summary>
        ''' Removes the specified mouse listener so that it no longer
        ''' receives mouse events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the mouse listener </param>
        ''' <seealso cref=      java.awt.event.MouseEvent </seealso>
        ''' <seealso cref=      java.awt.event.MouseListener </seealso>
        ''' <seealso cref=      #addMouseListener </seealso>
        ''' <seealso cref=      #getMouseListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeMouseListener(ByVal l As MouseListener)
            If l Is Nothing Then Return
            mouseListener = AWTEventMulticaster.remove(mouseListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the mouse listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>MouseListener</code>s
        '''         or an empty array if no mouse
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addMouseListener </seealso>
        ''' <seealso cref=      #removeMouseListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property mouseListeners As MouseListener()
            Get
                Return getListeners(GetType(MouseListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified mouse motion listener to receive mouse motion
        ''' events from this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the mouse motion listener </param>
        ''' <seealso cref=      java.awt.event.MouseEvent </seealso>
        ''' <seealso cref=      java.awt.event.MouseMotionListener </seealso>
        ''' <seealso cref=      #removeMouseMotionListener </seealso>
        ''' <seealso cref=      #getMouseMotionListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addMouseMotionListener(ByVal l As MouseMotionListener)
            If l Is Nothing Then Return
            mouseMotionListener = AWTEventMulticaster.add(mouseMotionListener, l)
            newEventsOnly = True

            ' if this is a lightweight component, enable mouse events
            ' in the native container.
            If TypeOf peer Is java.awt.peer.LightweightPeer Then parent.proxyEnableEvents(AWTEvent.MOUSE_MOTION_EVENT_MASK)
        End Sub

        ''' <summary>
        ''' Removes the specified mouse motion listener so that it no longer
        ''' receives mouse motion events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the mouse motion listener </param>
        ''' <seealso cref=      java.awt.event.MouseEvent </seealso>
        ''' <seealso cref=      java.awt.event.MouseMotionListener </seealso>
        ''' <seealso cref=      #addMouseMotionListener </seealso>
        ''' <seealso cref=      #getMouseMotionListeners
        ''' @since    JDK1.1 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeMouseMotionListener(ByVal l As MouseMotionListener)
            If l Is Nothing Then Return
            mouseMotionListener = AWTEventMulticaster.remove(mouseMotionListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the mouse motion listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>MouseMotionListener</code>s
        '''         or an empty array if no mouse motion
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addMouseMotionListener </seealso>
        ''' <seealso cref=      #removeMouseMotionListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property mouseMotionListeners As MouseMotionListener()
            Get
                Return getListeners(GetType(MouseMotionListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified mouse wheel listener to receive mouse wheel events
        ''' from this component.  Containers also receive mouse wheel events from
        ''' sub-components.
        ''' <p>
        ''' For information on how mouse wheel events are dispatched, see
        ''' the class description for <seealso cref="MouseWheelEvent"/>.
        ''' <p>
        ''' If l is <code>null</code>, no exception is thrown and no
        ''' action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the mouse wheel listener </param>
        ''' <seealso cref=      java.awt.event.MouseWheelEvent </seealso>
        ''' <seealso cref=      java.awt.event.MouseWheelListener </seealso>
        ''' <seealso cref=      #removeMouseWheelListener </seealso>
        ''' <seealso cref=      #getMouseWheelListeners
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addMouseWheelListener(ByVal l As MouseWheelListener)
            If l Is Nothing Then Return
            mouseWheelListener = AWTEventMulticaster.add(mouseWheelListener, l)
            newEventsOnly = True

            ' if this is a lightweight component, enable mouse events
            ' in the native container.
            If TypeOf peer Is java.awt.peer.LightweightPeer Then parent.proxyEnableEvents(AWTEvent.MOUSE_WHEEL_EVENT_MASK)
        End Sub

        ''' <summary>
        ''' Removes the specified mouse wheel listener so that it no longer
        ''' receives mouse wheel events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If l is null, no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the mouse wheel listener. </param>
        ''' <seealso cref=      java.awt.event.MouseWheelEvent </seealso>
        ''' <seealso cref=      java.awt.event.MouseWheelListener </seealso>
        ''' <seealso cref=      #addMouseWheelListener </seealso>
        ''' <seealso cref=      #getMouseWheelListeners
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeMouseWheelListener(ByVal l As MouseWheelListener)
            If l Is Nothing Then Return
            mouseWheelListener = AWTEventMulticaster.remove(mouseWheelListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the mouse wheel listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>MouseWheelListener</code>s
        '''         or an empty array if no mouse wheel
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addMouseWheelListener </seealso>
        ''' <seealso cref=      #removeMouseWheelListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property mouseWheelListeners As MouseWheelListener()
            Get
                Return getListeners(GetType(MouseWheelListener))
            End Get
        End Property

        ''' <summary>
        ''' Adds the specified input method listener to receive
        ''' input method events from this component. A component will
        ''' only receive input method events from input methods
        ''' if it also overrides <code>getInputMethodRequests</code> to return an
        ''' <code>InputMethodRequests</code> instance.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="{@docRoot}/java/awt/doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the input method listener </param>
        ''' <seealso cref=      java.awt.event.InputMethodEvent </seealso>
        ''' <seealso cref=      java.awt.event.InputMethodListener </seealso>
        ''' <seealso cref=      #removeInputMethodListener </seealso>
        ''' <seealso cref=      #getInputMethodListeners </seealso>
        ''' <seealso cref=      #getInputMethodRequests
        ''' @since    1.2 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub addInputMethodListener(ByVal l As InputMethodListener)
            If l Is Nothing Then Return
            inputMethodListener = AWTEventMulticaster.add(inputMethodListener, l)
            newEventsOnly = True
        End Sub

        ''' <summary>
        ''' Removes the specified input method listener so that it no longer
        ''' receives input method events from this component. This method performs
        ''' no function, nor does it throw an exception, if the listener
        ''' specified by the argument was not previously added to this component.
        ''' If listener <code>l</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
        ''' >AWT Threading Issues</a> for details on AWT's threading model.
        ''' </summary>
        ''' <param name="l">   the input method listener </param>
        ''' <seealso cref=      java.awt.event.InputMethodEvent </seealso>
        ''' <seealso cref=      java.awt.event.InputMethodListener </seealso>
        ''' <seealso cref=      #addInputMethodListener </seealso>
        ''' <seealso cref=      #getInputMethodListeners
        ''' @since    1.2 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Sub removeInputMethodListener(ByVal l As InputMethodListener)
            If l Is Nothing Then Return
            inputMethodListener = AWTEventMulticaster.remove(inputMethodListener, l)
        End Sub

        ''' <summary>
        ''' Returns an array of all the input method listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>InputMethodListener</code>s
        '''         or an empty array if no input method
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addInputMethodListener </seealso>
        ''' <seealso cref=      #removeInputMethodListener
        ''' @since    1.4 </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overridable Property inputMethodListeners As InputMethodListener()
            Get
                Return getListeners(GetType(InputMethodListener))
            End Get
        End Property

        ''' <summary>
        ''' Returns an array of all the objects currently registered
        ''' as <code><em>Foo</em>Listener</code>s
        ''' upon this <code>Component</code>.
        ''' <code><em>Foo</em>Listener</code>s are registered using the
        ''' <code>add<em>Foo</em>Listener</code> method.
        ''' 
        ''' <p>
        ''' You can specify the <code>listenerType</code> argument
        ''' with a class literal, such as
        ''' <code><em>Foo</em>Listener.class</code>.
        ''' For example, you can query a
        ''' <code>Component</code> <code>c</code>
        ''' for its mouse listeners with the following code:
        ''' 
        ''' <pre>MouseListener[] mls = (MouseListener[])(c.getListeners(MouseListener.class));</pre>
        ''' 
        ''' If no such listeners exist, this method returns an empty array.
        ''' </summary>
        ''' <param name="listenerType"> the type of listeners requested; this parameter
        '''          should specify an interface that descends from
        '''          <code>java.util.EventListener</code> </param>
        ''' <returns> an array of all objects registered as
        '''          <code><em>Foo</em>Listener</code>s on this component,
        '''          or an empty array if no such listeners have been added </returns>
        ''' <exception cref="ClassCastException"> if <code>listenerType</code>
        '''          doesn't specify a class or interface that implements
        '''          <code>java.util.EventListener</code> </exception>
        ''' <exception cref="NullPointerException"> if {@code listenerType} is {@code null} </exception>
        ''' <seealso cref= #getComponentListeners </seealso>
        ''' <seealso cref= #getFocusListeners </seealso>
        ''' <seealso cref= #getHierarchyListeners </seealso>
        ''' <seealso cref= #getHierarchyBoundsListeners </seealso>
        ''' <seealso cref= #getKeyListeners </seealso>
        ''' <seealso cref= #getMouseListeners </seealso>
        ''' <seealso cref= #getMouseMotionListeners </seealso>
        ''' <seealso cref= #getMouseWheelListeners </seealso>
        ''' <seealso cref= #getInputMethodListeners </seealso>
        ''' <seealso cref= #getPropertyChangeListeners
        ''' 
        ''' @since 1.3 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As [Class]) As T()
            Dim l As java.util.EventListener = Nothing
            If listenerType Is GetType(ComponentListener) Then
                l = componentListener
            ElseIf listenerType Is GetType(FocusListener) Then
                l = focusListener
            ElseIf listenerType Is GetType(HierarchyListener) Then
                l = hierarchyListener
            ElseIf listenerType Is GetType(HierarchyBoundsListener) Then
                l = hierarchyBoundsListener
            ElseIf listenerType Is GetType(KeyListener) Then
                l = keyListener
            ElseIf listenerType Is GetType(MouseListener) Then
                l = mouseListener
            ElseIf listenerType Is GetType(MouseMotionListener) Then
                l = mouseMotionListener
            ElseIf listenerType Is GetType(MouseWheelListener) Then
                l = mouseWheelListener
            ElseIf listenerType Is GetType(InputMethodListener) Then
                l = inputMethodListener
            ElseIf listenerType Is GetType(java.beans.PropertyChangeListener) Then
                Return CType(propertyChangeListeners, T())
            End If
            Return AWTEventMulticaster.getListeners(l, listenerType)
        End Function

        ''' <summary>
        ''' Gets the input method request handler which supports
        ''' requests from input methods for this component. A component
        ''' that supports on-the-spot text input must override this
        ''' method to return an <code>InputMethodRequests</code> instance.
        ''' At the same time, it also has to handle input method events.
        ''' </summary>
        ''' <returns> the input method request handler for this component,
        '''          <code>null</code> by default </returns>
        ''' <seealso cref= #addInputMethodListener
        ''' @since 1.2 </seealso>
        Public Overridable Property inputMethodRequests As java.awt.im.InputMethodRequests
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets the input context used by this component for handling
        ''' the communication with input methods when text is entered
        ''' in this component. By default, the input context used for
        ''' the parent component is returned. Components may
        ''' override this to return a private input context.
        ''' </summary>
        ''' <returns> the input context used by this component;
        '''          <code>null</code> if no context can be determined
        ''' @since 1.2 </returns>
        Public Overridable Property inputContext As java.awt.im.InputContext
            Get
                Dim parent_Renamed As Container = Me.parent
                If parent_Renamed Is Nothing Then
                    Return Nothing
                Else
                    Return parent_Renamed.inputContext
                End If
            End Get
        End Property

        ''' <summary>
        ''' Enables the events defined by the specified event mask parameter
        ''' to be delivered to this component.
        ''' <p>
        ''' Event types are automatically enabled when a listener for
        ''' that event type is added to the component.
        ''' <p>
        ''' This method only needs to be invoked by subclasses of
        ''' <code>Component</code> which desire to have the specified event
        ''' types delivered to <code>processEvent</code> regardless of whether
        ''' or not a listener is registered. </summary>
        ''' <param name="eventsToEnable">   the event mask defining the event types </param>
        ''' <seealso cref=        #processEvent </seealso>
        ''' <seealso cref=        #disableEvents </seealso>
        ''' <seealso cref=        AWTEvent
        ''' @since      JDK1.1 </seealso>
        Protected Friend Sub enableEvents(ByVal eventsToEnable As Long)
            Dim notifyAncestors As Long = 0
            SyncLock Me
                If (eventsToEnable And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 AndAlso hierarchyListener Is Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) = 0 Then notifyAncestors = notifyAncestors Or AWTEvent.HIERARCHY_EVENT_MASK
                If (eventsToEnable And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 AndAlso hierarchyBoundsListener Is Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) = 0 Then notifyAncestors = notifyAncestors Or AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK
                eventMask = eventMask Or eventsToEnable
                newEventsOnly = True
            End SyncLock

            ' if this is a lightweight component, enable mouse events
            ' in the native container.
            If TypeOf peer Is java.awt.peer.LightweightPeer Then parent.proxyEnableEvents(eventMask)
            If notifyAncestors <> 0 Then
                SyncLock treeLock
                    adjustListeningChildrenOnParent(notifyAncestors, 1)
                End SyncLock
            End If
        End Sub

        ''' <summary>
        ''' Disables the events defined by the specified event mask parameter
        ''' from being delivered to this component. </summary>
        ''' <param name="eventsToDisable">   the event mask defining the event types </param>
        ''' <seealso cref=        #enableEvents
        ''' @since      JDK1.1 </seealso>
        Protected Friend Sub disableEvents(ByVal eventsToDisable As Long)
            Dim notifyAncestors As Long = 0
            SyncLock Me
                If (eventsToDisable And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 AndAlso hierarchyListener Is Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 Then notifyAncestors = notifyAncestors Or AWTEvent.HIERARCHY_EVENT_MASK
                If (eventsToDisable And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 AndAlso hierarchyBoundsListener Is Nothing AndAlso (eventMask And AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK) <> 0 Then notifyAncestors = notifyAncestors Or AWTEvent.HIERARCHY_BOUNDS_EVENT_MASK
                eventMask = eventMask And Not eventsToDisable
            End SyncLock
            If notifyAncestors <> 0 Then
                SyncLock treeLock
                    adjustListeningChildrenOnParent(notifyAncestors, -1)
                End SyncLock
            End If
        End Sub

        <NonSerialized>
        Friend eventCache As sun.awt.EventQueueItem()

        ''' <seealso cref= #isCoalescingEnabled </seealso>
        ''' <seealso cref= #checkCoalescing </seealso>
        <NonSerialized>
        Private coalescingEnabled As Boolean = checkCoalescing()

        ''' <summary>
        ''' Weak map of known coalesceEvent overriders.
        ''' Value indicates whether overriden.
        ''' Bootstrap classes are not included.
        ''' </summary>
        Private Shared ReadOnly coalesceMap As IDictionary(Of [Class], Boolean?) = New java.util.WeakHashMap(Of [Class], Boolean?)

        ''' <summary>
        ''' Indicates whether this class overrides coalesceEvents.
        ''' It is assumed that all classes that are loaded from the bootstrap
        '''   do not.
        ''' The boostrap class loader is assumed to be represented by null.
        ''' We do not check that the method really overrides
        '''   (it might be static, private or package private).
        ''' </summary>
        Private Function checkCoalescing() As Boolean
            If Me.GetType().classLoader Is Nothing Then Return False
            Dim clazz As [Class] = Me.GetType()
            SyncLock coalesceMap
                ' Check cache.
                Dim value As Boolean? = coalesceMap(clazz)
                If value IsNot Nothing Then Return value

                ' Need to check non-bootstraps.
                Dim enabled_Renamed As Boolean? = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
                   )
                coalesceMap(clazz) = enabled_Renamed
                Return enabled_Renamed
            End SyncLock
        End Function

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Boolean?
                Return isCoalesceEventsOverriden(clazz)
            End Function
        End Class

        ''' <summary>
        ''' Parameter types of coalesceEvents(AWTEvent,AWTEVent).
        ''' </summary>
        Private Shared ReadOnly coalesceEventsParams As [Class]() = {GetType(AWTEvent), GetType(AWTEvent)}

        ''' <summary>
        ''' Indicates whether a class or its superclasses override coalesceEvents.
        ''' Must be called with lock on coalesceMap and privileged. </summary>
        ''' <seealso cref= checkCoalescing </seealso>
        Private Shared Function isCoalesceEventsOverriden(ByVal clazz As [Class]) As Boolean
            Debug.Assert(Thread.holdsLock(coalesceMap))

            ' First check superclass - we may not need to bother ourselves.
            Dim superclass As [Class] = clazz.BaseType
            If superclass Is Nothing Then Return False
            If superclass.classLoader IsNot Nothing Then
                Dim value As Boolean? = coalesceMap(superclass)
                If value Is Nothing Then
                    ' Not done already - recurse.
                    If isCoalesceEventsOverriden(superclass) Then
                        coalesceMap(superclass) = True
                        Return True
                    End If
                ElseIf value Then
                    Return True
                End If
            End If

            Try
                ' Throws if not overriden.
                clazz.getDeclaredMethod("coalesceEvents", coalesceEventsParams)
                Return True
            Catch e As NoSuchMethodException
                ' Not present in this class.
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Indicates whether coalesceEvents may do something.
        ''' </summary>
        Friend Property coalescingEnabled As Boolean
            Get
                Return coalescingEnabled
            End Get
        End Property


        ''' <summary>
        ''' Potentially coalesce an event being posted with an existing
        ''' event.  This method is called by <code>EventQueue.postEvent</code>
        ''' if an event with the same ID as the event to be posted is found in
        ''' the queue (both events must have this component as their source).
        ''' This method either returns a coalesced event which replaces
        ''' the existing event (and the new event is then discarded), or
        ''' <code>null</code> to indicate that no combining should be done
        ''' (add the second event to the end of the queue).  Either event
        ''' parameter may be modified and returned, as the other one is discarded
        ''' unless <code>null</code> is returned.
        ''' <p>
        ''' This implementation of <code>coalesceEvents</code> coalesces
        ''' two event types: mouse move (and drag) events,
        ''' and paint (and update) events.
        ''' For mouse move events the last event is always returned, causing
        ''' intermediate moves to be discarded.  For paint events, the new
        ''' event is coalesced into a complex <code>RepaintArea</code> in the peer.
        ''' The new <code>AWTEvent</code> is always returned.
        ''' </summary>
        ''' <param name="existingEvent">  the event already on the <code>EventQueue</code> </param>
        ''' <param name="newEvent">       the event being posted to the
        '''          <code>EventQueue</code> </param>
        ''' <returns> a coalesced event, or <code>null</code> indicating that no
        '''          coalescing was done </returns>
        Protected Friend Overridable Function coalesceEvents(ByVal existingEvent As AWTEvent, ByVal newEvent As AWTEvent) As AWTEvent
            Return Nothing
        End Function

        ''' <summary>
        ''' Processes events occurring on this component. By default this
        ''' method calls the appropriate
        ''' <code>process&lt;event&nbsp;type&gt;Event</code>
        ''' method for the given class of event.
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the event </param>
        ''' <seealso cref=       #processComponentEvent </seealso>
        ''' <seealso cref=       #processFocusEvent </seealso>
        ''' <seealso cref=       #processKeyEvent </seealso>
        ''' <seealso cref=       #processMouseEvent </seealso>
        ''' <seealso cref=       #processMouseMotionEvent </seealso>
        ''' <seealso cref=       #processInputMethodEvent </seealso>
        ''' <seealso cref=       #processHierarchyEvent </seealso>
        ''' <seealso cref=       #processMouseWheelEvent
        ''' @since     JDK1.1 </seealso>
        Protected Friend Overridable Sub processEvent(ByVal e As AWTEvent)
            If TypeOf e Is FocusEvent Then
                processFocusEvent(CType(e, FocusEvent))

            ElseIf TypeOf e Is MouseEvent Then
                Select Case e.id
                    Case MouseEvent.MOUSE_PRESSED, MouseEvent.MOUSE_RELEASED, MouseEvent.MOUSE_CLICKED, MouseEvent.MOUSE_ENTERED, MouseEvent.MOUSE_EXITED
                        processMouseEvent(CType(e, MouseEvent))
                    Case MouseEvent.MOUSE_MOVED, MouseEvent.MOUSE_DRAGGED
                        processMouseMotionEvent(CType(e, MouseEvent))
                    Case MouseEvent.MOUSE_WHEEL
                        processMouseWheelEvent(CType(e, MouseWheelEvent))
                End Select

            ElseIf TypeOf e Is KeyEvent Then
                processKeyEvent(CType(e, KeyEvent))

            ElseIf TypeOf e Is ComponentEvent Then
                processComponentEvent(CType(e, ComponentEvent))
            ElseIf TypeOf e Is InputMethodEvent Then
                processInputMethodEvent(CType(e, InputMethodEvent))
            ElseIf TypeOf e Is HierarchyEvent Then
                Select Case e.id
                    Case HierarchyEvent.HIERARCHY_CHANGED
                        processHierarchyEvent(CType(e, HierarchyEvent))
                    Case HierarchyEvent.ANCESTOR_MOVED, HierarchyEvent.ANCESTOR_RESIZED
                        processHierarchyBoundsEvent(CType(e, HierarchyEvent))
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes component events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>ComponentListener</code> objects.
        ''' <p>
        ''' This method is not called unless component events are
        ''' enabled for this component. Component events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>A <code>ComponentListener</code> object is registered
        ''' via <code>addComponentListener</code>.
        ''' <li>Component events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the component event </param>
        ''' <seealso cref=         java.awt.event.ComponentEvent </seealso>
        ''' <seealso cref=         java.awt.event.ComponentListener </seealso>
        ''' <seealso cref=         #addComponentListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       JDK1.1 </seealso>
        Protected Friend Overridable Sub processComponentEvent(ByVal e As ComponentEvent)
            Dim listener As ComponentListener = componentListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case ComponentEvent.COMPONENT_RESIZED
                        listener.componentResized(e)
                    Case ComponentEvent.COMPONENT_MOVED
                        listener.componentMoved(e)
                    Case ComponentEvent.COMPONENT_SHOWN
                        listener.componentShown(e)
                    Case ComponentEvent.COMPONENT_HIDDEN
                        listener.componentHidden(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes focus events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>FocusListener</code> objects.
        ''' <p>
        ''' This method is not called unless focus events are
        ''' enabled for this component. Focus events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>A <code>FocusListener</code> object is registered
        ''' via <code>addFocusListener</code>.
        ''' <li>Focus events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>
        ''' If focus events are enabled for a <code>Component</code>,
        ''' the current <code>KeyboardFocusManager</code> determines
        ''' whether or not a focus event should be dispatched to
        ''' registered <code>FocusListener</code> objects.  If the
        ''' events are to be dispatched, the <code>KeyboardFocusManager</code>
        ''' calls the <code>Component</code>'s <code>dispatchEvent</code>
        ''' method, which results in a call to the <code>Component</code>'s
        ''' <code>processFocusEvent</code> method.
        ''' <p>
        ''' If focus events are enabled for a <code>Component</code>, calling
        ''' the <code>Component</code>'s <code>dispatchEvent</code> method
        ''' with a <code>FocusEvent</code> as the argument will result in a
        ''' call to the <code>Component</code>'s <code>processFocusEvent</code>
        ''' method regardless of the current <code>KeyboardFocusManager</code>.
        ''' 
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the focus event </param>
        ''' <seealso cref=         java.awt.event.FocusEvent </seealso>
        ''' <seealso cref=         java.awt.event.FocusListener </seealso>
        ''' <seealso cref=         java.awt.KeyboardFocusManager </seealso>
        ''' <seealso cref=         #addFocusListener </seealso>
        ''' <seealso cref=         #enableEvents </seealso>
        ''' <seealso cref=         #dispatchEvent
        ''' @since       JDK1.1 </seealso>
        Protected Friend Overridable Sub processFocusEvent(ByVal e As FocusEvent)
            Dim listener As FocusListener = focusListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case FocusEvent.FOCUS_GAINED
                        listener.focusGained(e)
                    Case FocusEvent.FOCUS_LOST
                        listener.focusLost(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes key events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>KeyListener</code> objects.
        ''' <p>
        ''' This method is not called unless key events are
        ''' enabled for this component. Key events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>A <code>KeyListener</code> object is registered
        ''' via <code>addKeyListener</code>.
        ''' <li>Key events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' 
        ''' <p>
        ''' If key events are enabled for a <code>Component</code>,
        ''' the current <code>KeyboardFocusManager</code> determines
        ''' whether or not a key event should be dispatched to
        ''' registered <code>KeyListener</code> objects.  The
        ''' <code>DefaultKeyboardFocusManager</code> will not dispatch
        ''' key events to a <code>Component</code> that is not the focus
        ''' owner or is not showing.
        ''' <p>
        ''' As of J2SE 1.4, <code>KeyEvent</code>s are redirected to
        ''' the focus owner. Please see the
        ''' <a href="doc-files/FocusSpec.html">Focus Specification</a>
        ''' for further information.
        ''' <p>
        ''' Calling a <code>Component</code>'s <code>dispatchEvent</code>
        ''' method with a <code>KeyEvent</code> as the argument will
        ''' result in a call to the <code>Component</code>'s
        ''' <code>processKeyEvent</code> method regardless of the
        ''' current <code>KeyboardFocusManager</code> as long as the
        ''' component is showing, focused, and enabled, and key events
        ''' are enabled on it.
        ''' <p>If the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the key event </param>
        ''' <seealso cref=         java.awt.event.KeyEvent </seealso>
        ''' <seealso cref=         java.awt.event.KeyListener </seealso>
        ''' <seealso cref=         java.awt.KeyboardFocusManager </seealso>
        ''' <seealso cref=         java.awt.DefaultKeyboardFocusManager </seealso>
        ''' <seealso cref=         #processEvent </seealso>
        ''' <seealso cref=         #dispatchEvent </seealso>
        ''' <seealso cref=         #addKeyListener </seealso>
        ''' <seealso cref=         #enableEvents </seealso>
        ''' <seealso cref=         #isShowing
        ''' @since       JDK1.1 </seealso>
        Protected Friend Overridable Sub processKeyEvent(ByVal e As KeyEvent)
            Dim listener As KeyListener = keyListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case KeyEvent.KEY_TYPED
                        listener.keyTyped(e)
                    Case KeyEvent.KEY_PRESSED
                        listener.keyPressed(e)
                    Case KeyEvent.KEY_RELEASED
                        listener.keyReleased(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes mouse events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>MouseListener</code> objects.
        ''' <p>
        ''' This method is not called unless mouse events are
        ''' enabled for this component. Mouse events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>A <code>MouseListener</code> object is registered
        ''' via <code>addMouseListener</code>.
        ''' <li>Mouse events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the mouse event </param>
        ''' <seealso cref=         java.awt.event.MouseEvent </seealso>
        ''' <seealso cref=         java.awt.event.MouseListener </seealso>
        ''' <seealso cref=         #addMouseListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       JDK1.1 </seealso>
        Protected Friend Overridable Sub processMouseEvent(ByVal e As MouseEvent)
            Dim listener As MouseListener = mouseListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case MouseEvent.MOUSE_PRESSED
                        listener.mousePressed(e)
                    Case MouseEvent.MOUSE_RELEASED
                        listener.mouseReleased(e)
                    Case MouseEvent.MOUSE_CLICKED
                        listener.mouseClicked(e)
                    Case MouseEvent.MOUSE_EXITED
                        listener.mouseExited(e)
                    Case MouseEvent.MOUSE_ENTERED
                        listener.mouseEntered(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes mouse motion events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>MouseMotionListener</code> objects.
        ''' <p>
        ''' This method is not called unless mouse motion events are
        ''' enabled for this component. Mouse motion events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>A <code>MouseMotionListener</code> object is registered
        ''' via <code>addMouseMotionListener</code>.
        ''' <li>Mouse motion events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the mouse motion event </param>
        ''' <seealso cref=         java.awt.event.MouseEvent </seealso>
        ''' <seealso cref=         java.awt.event.MouseMotionListener </seealso>
        ''' <seealso cref=         #addMouseMotionListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       JDK1.1 </seealso>
        Protected Friend Overridable Sub processMouseMotionEvent(ByVal e As MouseEvent)
            Dim listener As MouseMotionListener = mouseMotionListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case MouseEvent.MOUSE_MOVED
                        listener.mouseMoved(e)
                    Case MouseEvent.MOUSE_DRAGGED
                        listener.mouseDragged(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes mouse wheel events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>MouseWheelListener</code> objects.
        ''' <p>
        ''' This method is not called unless mouse wheel events are
        ''' enabled for this component. Mouse wheel events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>A <code>MouseWheelListener</code> object is registered
        ''' via <code>addMouseWheelListener</code>.
        ''' <li>Mouse wheel events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>
        ''' For information on how mouse wheel events are dispatched, see
        ''' the class description for <seealso cref="MouseWheelEvent"/>.
        ''' <p>
        ''' Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the mouse wheel event </param>
        ''' <seealso cref=         java.awt.event.MouseWheelEvent </seealso>
        ''' <seealso cref=         java.awt.event.MouseWheelListener </seealso>
        ''' <seealso cref=         #addMouseWheelListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       1.4 </seealso>
        Protected Friend Overridable Sub processMouseWheelEvent(ByVal e As MouseWheelEvent)
            Dim listener As MouseWheelListener = mouseWheelListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case MouseEvent.MOUSE_WHEEL
                        listener.mouseWheelMoved(e)
                End Select
            End If
        End Sub

        Friend Overridable Function postsOldMouseEvents() As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Processes input method events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>InputMethodListener</code> objects.
        ''' <p>
        ''' This method is not called unless input method events
        ''' are enabled for this component. Input method events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>An <code>InputMethodListener</code> object is registered
        ''' via <code>addInputMethodListener</code>.
        ''' <li>Input method events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the input method event </param>
        ''' <seealso cref=         java.awt.event.InputMethodEvent </seealso>
        ''' <seealso cref=         java.awt.event.InputMethodListener </seealso>
        ''' <seealso cref=         #addInputMethodListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       1.2 </seealso>
        Protected Friend Overridable Sub processInputMethodEvent(ByVal e As InputMethodEvent)
            Dim listener As InputMethodListener = inputMethodListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case InputMethodEvent.INPUT_METHOD_TEXT_CHANGED
                        listener.inputMethodTextChanged(e)
                    Case InputMethodEvent.CARET_POSITION_CHANGED
                        listener.caretPositionChanged(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes hierarchy events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>HierarchyListener</code> objects.
        ''' <p>
        ''' This method is not called unless hierarchy events
        ''' are enabled for this component. Hierarchy events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>An <code>HierarchyListener</code> object is registered
        ''' via <code>addHierarchyListener</code>.
        ''' <li>Hierarchy events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the hierarchy event </param>
        ''' <seealso cref=         java.awt.event.HierarchyEvent </seealso>
        ''' <seealso cref=         java.awt.event.HierarchyListener </seealso>
        ''' <seealso cref=         #addHierarchyListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       1.3 </seealso>
        Protected Friend Overridable Sub processHierarchyEvent(ByVal e As HierarchyEvent)
            Dim listener As HierarchyListener = hierarchyListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case HierarchyEvent.HIERARCHY_CHANGED
                        listener.hierarchyChanged(e)
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Processes hierarchy bounds events occurring on this component by
        ''' dispatching them to any registered
        ''' <code>HierarchyBoundsListener</code> objects.
        ''' <p>
        ''' This method is not called unless hierarchy bounds events
        ''' are enabled for this component. Hierarchy bounds events are enabled
        ''' when one of the following occurs:
        ''' <ul>
        ''' <li>An <code>HierarchyBoundsListener</code> object is registered
        ''' via <code>addHierarchyBoundsListener</code>.
        ''' <li>Hierarchy bounds events are enabled via <code>enableEvents</code>.
        ''' </ul>
        ''' <p>Note that if the event parameter is <code>null</code>
        ''' the behavior is unspecified and may result in an
        ''' exception.
        ''' </summary>
        ''' <param name="e"> the hierarchy event </param>
        ''' <seealso cref=         java.awt.event.HierarchyEvent </seealso>
        ''' <seealso cref=         java.awt.event.HierarchyBoundsListener </seealso>
        ''' <seealso cref=         #addHierarchyBoundsListener </seealso>
        ''' <seealso cref=         #enableEvents
        ''' @since       1.3 </seealso>
        Protected Friend Overridable Sub processHierarchyBoundsEvent(ByVal e As HierarchyEvent)
            Dim listener As HierarchyBoundsListener = hierarchyBoundsListener
            If listener IsNot Nothing Then
                Dim id As Integer = e.iD
                Select Case id
                    Case HierarchyEvent.ANCESTOR_MOVED
                        listener.ancestorMoved(e)
                    Case HierarchyEvent.ANCESTOR_RESIZED
                        listener.ancestorResized(e)
                End Select
            End If
        End Sub

        ''' @deprecated As of JDK version 1.1
        ''' replaced by processEvent(AWTEvent). 
        <Obsolete("As of JDK version 1.1")>
        Public Overridable Function handleEvent(ByVal evt As [event]) As Boolean
            Select Case evt.id
                Case Event.MOUSE_ENTER
				  Return mouseEnter(evt, evt.x, evt.y)

                Case Event.MOUSE_EXIT
				  Return mouseExit(evt, evt.x, evt.y)

                Case Event.MOUSE_MOVE
				  Return mouseMove(evt, evt.x, evt.y)

                Case Event.MOUSE_DOWN
				  Return mouseDown(evt, evt.x, evt.y)

                Case Event.MOUSE_DRAG
				  Return mouseDrag(evt, evt.x, evt.y)

                Case Event.MOUSE_UP
				  Return mouseUp(evt, evt.x, evt.y)

                Case Event.KEY_PRESS, Event.KEY_ACTION
				  Return keyDown(evt, evt.key)

                Case Event.KEY_RELEASE, Event.KEY_ACTION_RELEASE
				  Return keyUp(evt, evt.key)

                Case Event.ACTION_EVENT
				  Return action(evt, evt.arg)
                Case Event.GOT_FOCUS
				  Return gotFocus(evt, evt.arg)
                Case Event.LOST_FOCUS
				  Return lostFocus(evt, evt.arg)
            End Select
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processMouseEvent(MouseEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function mouseDown(ByVal evt As [event], ByVal x As Integer, ByVal y As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processMouseMotionEvent(MouseEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function mouseDrag(ByVal evt As [event], ByVal x As Integer, ByVal y As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processMouseEvent(MouseEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function mouseUp(ByVal evt As [event], ByVal x As Integer, ByVal y As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processMouseMotionEvent(MouseEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function mouseMove(ByVal evt As [event], ByVal x As Integer, ByVal y As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processMouseEvent(MouseEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function mouseEnter(ByVal evt As [event], ByVal x As Integer, ByVal y As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processMouseEvent(MouseEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function mouseExit(ByVal evt As [event], ByVal x As Integer, ByVal y As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processKeyEvent(KeyEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function keyDown(ByVal evt As [event], ByVal key As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processKeyEvent(KeyEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function keyUp(ByVal evt As [event], ByVal key As Integer) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' should register this component as ActionListener on component
        ''' which fires action events. 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function action(ByVal evt As [event], ByVal what As Object) As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Makes this <code>Component</code> displayable by connecting it to a
        ''' native screen resource.
        ''' This method is called internally by the toolkit and should
        ''' not be called directly by programs.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' </summary>
        ''' <seealso cref=       #isDisplayable </seealso>
        ''' <seealso cref=       #removeNotify </seealso>
        ''' <seealso cref= #invalidate
        ''' @since JDK1.0 </seealso>
        Public Overridable Sub addNotify()
            SyncLock treeLock
                Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
                If peer_Renamed Is Nothing OrElse TypeOf peer_Renamed Is java.awt.peer.LightweightPeer Then
                    If peer_Renamed Is Nothing Then
                        ' Update both the Component's peer variable and the local
                        ' variable we use for thread safety.
                        peer_Renamed = toolkit.createComponent(Me)
                        Me.peer = peer_Renamed
                    End If

                    ' This is a lightweight component which means it won't be
                    ' able to get window-related events by itself.  If any
                    ' have been enabled, then the nearest native container must
                    ' be enabled.
                    If parent IsNot Nothing Then
                        Dim mask As Long = 0
                        If (mouseListener IsNot Nothing) OrElse ((eventMask And AWTEvent.MOUSE_EVENT_MASK) <> 0) Then mask = mask Or AWTEvent.MOUSE_EVENT_MASK
                        If (mouseMotionListener IsNot Nothing) OrElse ((eventMask And AWTEvent.MOUSE_MOTION_EVENT_MASK) <> 0) Then mask = mask Or AWTEvent.MOUSE_MOTION_EVENT_MASK
                        If (mouseWheelListener IsNot Nothing) OrElse ((eventMask And AWTEvent.MOUSE_WHEEL_EVENT_MASK) <> 0) Then mask = mask Or AWTEvent.MOUSE_WHEEL_EVENT_MASK
                        If focusListener IsNot Nothing OrElse (eventMask And AWTEvent.FOCUS_EVENT_MASK) <> 0 Then mask = mask Or AWTEvent.FOCUS_EVENT_MASK
                        If keyListener IsNot Nothing OrElse (eventMask And AWTEvent.KEY_EVENT_MASK) <> 0 Then mask = mask Or AWTEvent.KEY_EVENT_MASK
                        If mask <> 0 Then parent.proxyEnableEvents(mask)
                    End If
                Else
                    ' It's native. If the parent is lightweight it will need some
                    ' help.
                    Dim parent_Renamed As Container = container
                    If parent_Renamed IsNot Nothing AndAlso parent_Renamed.lightweight Then
                        relocateComponent()
                        If Not parent_Renamed.recursivelyVisibleUpToHeavyweightContainer Then peer_Renamed.visible = False
                    End If
                End If
                invalidate()

                Dim npopups As Integer = (If(popups IsNot Nothing, popups.Count, 0))
                For i As Integer = 0 To npopups - 1
                    Dim popup As PopupMenu = popups(i)
                    popup.addNotify()
                Next i

                If dropTarget IsNot Nothing Then dropTarget.addNotify(peer_Renamed)

                peerFont = font

                If container IsNot Nothing AndAlso (Not isAddNotifyComplete) Then container.increaseComponentCount(Me)


                ' Update stacking order
                updateZOrder()

                If Not isAddNotifyComplete Then mixOnShowing()

                isAddNotifyComplete = True

                If hierarchyListener IsNot Nothing OrElse (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK) Then
                    Dim e As New HierarchyEvent(Me, HierarchyEvent.HIERARCHY_CHANGED, Me, parent, HierarchyEvent.DISPLAYABILITY_CHANGED Or (If(recursivelyVisible, HierarchyEvent.SHOWING_CHANGED, 0)))
                    dispatchEvent(e)
                End If
            End SyncLock
        End Sub

        ''' <summary>
        ''' Makes this <code>Component</code> undisplayable by destroying it native
        ''' screen resource.
        ''' <p>
        ''' This method is called by the toolkit internally and should
        ''' not be called directly by programs. Code overriding
        ''' this method should call <code>super.removeNotify</code> as
        ''' the first line of the overriding method.
        ''' </summary>
        ''' <seealso cref=       #isDisplayable </seealso>
        ''' <seealso cref=       #addNotify
        ''' @since JDK1.0 </seealso>
        Public Overridable Sub removeNotify()
            KeyboardFocusManager.clearMostRecentFocusOwner(Me)
            If KeyboardFocusManager.currentKeyboardFocusManager.permanentFocusOwner Is Me Then KeyboardFocusManager.currentKeyboardFocusManager.globalPermanentFocusOwner = Nothing

            SyncLock treeLock
                If focusOwner AndAlso KeyboardFocusManager.isAutoFocusTransferEnabledFor(Me) Then transferFocus(True)

                If container IsNot Nothing AndAlso isAddNotifyComplete Then container.decreaseComponentCount(Me)

                Dim npopups As Integer = (If(popups IsNot Nothing, popups.Count, 0))
                For i As Integer = 0 To npopups - 1
                    Dim popup As PopupMenu = popups(i)
                    popup.removeNotify()
                Next i
                ' If there is any input context for this component, notify
                ' that this component is being removed. (This has to be done
                ' before hiding peer.)
                If (eventMask And AWTEvent.INPUT_METHODS_ENABLED_MASK) <> 0 Then
                    Dim inputContext_Renamed As java.awt.im.InputContext = inputContext
                    If inputContext_Renamed IsNot Nothing Then inputContext_Renamed.removeNotify(Me)
                End If

                Dim p As java.awt.peer.ComponentPeer = peer
                If p IsNot Nothing Then
                    Dim isLightweight As Boolean = lightweight

                    If TypeOf bufferStrategy Is FlipBufferStrategy Then CType(bufferStrategy, FlipBufferStrategy).destroyBuffers()

                    If dropTarget IsNot Nothing Then dropTarget.removeNotify(peer)

                    ' Hide peer first to stop system events such as cursor moves.
                    If visible Then p.visible = False

                    peer = Nothing ' Stop peer updates.
                    peerFont = Nothing

                    toolkit.eventQueue.removeSourceEvents(Me, False)
                    KeyboardFocusManager.currentKeyboardFocusManager.discardKeyEvents(Me)

                    p.dispose()

                    mixOnHiding(isLightweight)

                    isAddNotifyComplete = False
                    ' Nullifying compoundShape means that the component has normal shape
                    ' (or has no shape at all).
                    Me.compoundShape = Nothing
                End If

                If hierarchyListener IsNot Nothing OrElse (eventMask And AWTEvent.HIERARCHY_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK) Then
                    Dim e As New HierarchyEvent(Me, HierarchyEvent.HIERARCHY_CHANGED, Me, parent, HierarchyEvent.DISPLAYABILITY_CHANGED Or (If(recursivelyVisible, HierarchyEvent.SHOWING_CHANGED, 0)))
                    dispatchEvent(e)
                End If
            End SyncLock
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processFocusEvent(FocusEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function gotFocus(ByVal evt As [event], ByVal what As Object) As Boolean
            Return False
        End Function

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by processFocusEvent(FocusEvent). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Function lostFocus(ByVal evt As [event], ByVal what As Object) As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Returns whether this <code>Component</code> can become the focus
        ''' owner.
        ''' </summary>
        ''' <returns> <code>true</code> if this <code>Component</code> is
        ''' focusable; <code>false</code> otherwise </returns>
        ''' <seealso cref= #setFocusable
        ''' @since JDK1.1 </seealso>
        ''' @deprecated As of 1.4, replaced by <code>isFocusable()</code>. 
        <Obsolete("As of 1.4, replaced by <code>isFocusable()</code>.")>
        Public Overridable Property focusTraversable As Boolean
            Get
                If isFocusTraversableOverridden_Renamed = FOCUS_TRAVERSABLE_UNKNOWN Then isFocusTraversableOverridden_Renamed = FOCUS_TRAVERSABLE_DEFAULT
                Return focusable
            End Get
        End Property

        ''' <summary>
        ''' Returns whether this Component can be focused.
        ''' </summary>
        ''' <returns> <code>true</code> if this Component is focusable;
        '''         <code>false</code> otherwise. </returns>
        ''' <seealso cref= #setFocusable
        ''' @since 1.4 </seealso>
        Public Overridable Property focusable As Boolean
            Get
                Return focusTraversable
            End Get
            Set(ByVal focusable As Boolean)
                Dim oldFocusable As Boolean
                SyncLock Me
                    oldFocusable = Me.focusable
                    Me.focusable = focusable
                End SyncLock
                isFocusTraversableOverridden_Renamed = FOCUS_TRAVERSABLE_SET

                firePropertyChange("focusable", oldFocusable, focusable)
                If oldFocusable AndAlso (Not focusable) Then
                    If focusOwner AndAlso KeyboardFocusManager.autoFocusTransferEnabled Then transferFocus(True)
                    KeyboardFocusManager.clearMostRecentFocusOwner(Me)
                End If
            End Set
        End Property


        Friend Property focusTraversableOverridden As Boolean
            Get
                Return (isFocusTraversableOverridden_Renamed <> FOCUS_TRAVERSABLE_DEFAULT)
            End Get
        End Property

        ''' <summary>
        ''' Sets the focus traversal keys for a given traversal operation for this
        ''' Component.
        ''' <p>
        ''' The default values for a Component's focus traversal keys are
        ''' implementation-dependent. Sun recommends that all implementations for a
        ''' particular native platform use the same default values. The
        ''' recommendations for Windows and Unix are listed below. These
        ''' recommendations are used in the Sun AWT implementations.
        ''' 
        ''' <table border=1 summary="Recommended default values for a Component's focus traversal keys">
        ''' <tr>
        '''    <th>Identifier</th>
        '''    <th>Meaning</th>
        '''    <th>Default</th>
        ''' </tr>
        ''' <tr>
        '''    <td>KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS</td>
        '''    <td>Normal forward keyboard traversal</td>
        '''    <td>TAB on KEY_PRESSED, CTRL-TAB on KEY_PRESSED</td>
        ''' </tr>
        ''' <tr>
        '''    <td>KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS</td>
        '''    <td>Normal reverse keyboard traversal</td>
        '''    <td>SHIFT-TAB on KEY_PRESSED, CTRL-SHIFT-TAB on KEY_PRESSED</td>
        ''' </tr>
        ''' <tr>
        '''    <td>KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS</td>
        '''    <td>Go up one focus traversal cycle</td>
        '''    <td>none</td>
        ''' </tr>
        ''' </table>
        ''' 
        ''' To disable a traversal key, use an empty Set; Collections.EMPTY_SET is
        ''' recommended.
        ''' <p>
        ''' Using the AWTKeyStroke API, client code can specify on which of two
        ''' specific KeyEvents, KEY_PRESSED or KEY_RELEASED, the focus traversal
        ''' operation will occur. Regardless of which KeyEvent is specified,
        ''' however, all KeyEvents related to the focus traversal key, including the
        ''' associated KEY_TYPED event, will be consumed, and will not be dispatched
        ''' to any Component. It is a runtime error to specify a KEY_TYPED event as
        ''' mapping to a focus traversal operation, or to map the same event to
        ''' multiple default focus traversal operations.
        ''' <p>
        ''' If a value of null is specified for the Set, this Component inherits the
        ''' Set from its parent. If all ancestors of this Component have null
        ''' specified for the Set, then the current KeyboardFocusManager's default
        ''' Set is used.
        ''' <p>
        ''' This method may throw a {@code ClassCastException} if any {@code Object}
        ''' in {@code keystrokes} is not an {@code AWTKeyStroke}.
        ''' </summary>
        ''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
        '''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
        '''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
        ''' <param name="keystrokes"> the Set of AWTKeyStroke for the specified operation </param>
        ''' <seealso cref= #getFocusTraversalKeys </seealso>
        ''' <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
        ''' <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
        ''' <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
        ''' <exception cref="IllegalArgumentException"> if id is not one of
        '''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
        '''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
        '''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or if keystrokes
        '''         contains null, or if any keystroke represents a KEY_TYPED event,
        '''         or if any keystroke already maps to another focus traversal
        '''         operation for this Component
        ''' @since 1.4
        ''' @beaninfo
        '''       bound: true </exception>
        Public Overridable Sub setFocusTraversalKeys(Of T1 As AWTKeyStroke)(ByVal id As Integer, ByVal keystrokes As java.util.Set(Of T1))
            If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH - 1 Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

            focusTraversalKeys_NoIDCheckeck(id, keystrokes)
        End Sub

        ''' <summary>
        ''' Returns the Set of focus traversal keys for a given traversal operation
        ''' for this Component. (See
        ''' <code>setFocusTraversalKeys</code> for a full description of each key.)
        ''' <p>
        ''' If a Set of traversal keys has not been explicitly defined for this
        ''' Component, then this Component's parent's Set is returned. If no Set
        ''' has been explicitly defined for any of this Component's ancestors, then
        ''' the current KeyboardFocusManager's default Set is returned.
        ''' </summary>
        ''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
        '''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
        '''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
        ''' <returns> the Set of AWTKeyStrokes for the specified operation. The Set
        '''         will be unmodifiable, and may be empty. null will never be
        '''         returned. </returns>
        ''' <seealso cref= #setFocusTraversalKeys </seealso>
        ''' <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
        ''' <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
        ''' <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
        ''' <exception cref="IllegalArgumentException"> if id is not one of
        '''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
        '''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
        '''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS
        ''' @since 1.4 </exception>
        Public Overridable Function getFocusTraversalKeys(ByVal id As Integer) As java.util.Set(Of AWTKeyStroke)
            If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH - 1 Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

            Return getFocusTraversalKeys_NoIDCheck(id)
        End Function

        ' We define these methods so that Container does not need to repeat this
        ' code. Container cannot call super.<method> because Container allows
        ' DOWN_CYCLE_TRAVERSAL_KEY while Component does not. The Component method
        ' would erroneously generate an IllegalArgumentException for
        ' DOWN_CYCLE_TRAVERSAL_KEY.
        Friend Sub setFocusTraversalKeys_NoIDCheck(Of T1 As AWTKeyStroke)(ByVal id As Integer, ByVal keystrokes As java.util.Set(Of T1))
            Dim oldKeys As java.util.Set(Of AWTKeyStroke)

            SyncLock Me
                If focusTraversalKeys Is Nothing Then initializeFocusTraversalKeys()

                If keystrokes IsNot Nothing Then
                    For Each keystroke As AWTKeyStroke In keystrokes

                        If keystroke Is Nothing Then Throw New IllegalArgumentException("cannot set null focus traversal key")

                        If keystroke.keyChar <> KeyEvent.CHAR_UNDEFINED Then Throw New IllegalArgumentException("focus traversal keys cannot map to KEY_TYPED events")

                        For i As Integer = 0 To focusTraversalKeys.Length - 1
                            If i = id Then Continue For

                            If getFocusTraversalKeys_NoIDCheck(i).contains(keystroke) Then Throw New IllegalArgumentException("focus traversal keys must be unique for a Component")
                        Next i
                    Next keystroke
                End If

                oldKeys = focusTraversalKeys(id)
                focusTraversalKeys(id) = If(keystrokes IsNot Nothing, java.util.Collections.unmodifiableSet(New HashSet(Of AWTKeyStroke)(keystrokes)), Nothing)
            End SyncLock

            firePropertyChange(focusTraversalKeyPropertyNames(id), oldKeys, keystrokes)
        End Sub
        Friend Function getFocusTraversalKeys_NoIDCheck(ByVal id As Integer) As java.util.Set(Of AWTKeyStroke)
            ' Okay to return Set directly because it is an unmodifiable view
            'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            Dim keystrokes As java.util.Set(Of AWTKeyStroke) = If(focusTraversalKeys IsNot Nothing, focusTraversalKeys(id), Nothing)

            If keystrokes IsNot Nothing Then
                Return keystrokes
            Else
                Dim parent_Renamed As Container = Me.parent
                If parent_Renamed IsNot Nothing Then
                    Return parent_Renamed.getFocusTraversalKeys(id)
                Else
                    Return KeyboardFocusManager.currentKeyboardFocusManager.getDefaultFocusTraversalKeys(id)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns whether the Set of focus traversal keys for the given focus
        ''' traversal operation has been explicitly defined for this Component. If
        ''' this method returns <code>false</code>, this Component is inheriting the
        ''' Set from an ancestor, or from the current KeyboardFocusManager.
        ''' </summary>
        ''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
        '''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
        '''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
        ''' <returns> <code>true</code> if the the Set of focus traversal keys for the
        '''         given focus traversal operation has been explicitly defined for
        '''         this Component; <code>false</code> otherwise. </returns>
        ''' <exception cref="IllegalArgumentException"> if id is not one of
        '''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
        '''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
        '''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS
        ''' @since 1.4 </exception>
        Public Overridable Function areFocusTraversalKeysSet(ByVal id As Integer) As Boolean
            If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH - 1 Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

            Return (focusTraversalKeys IsNot Nothing AndAlso focusTraversalKeys(id) IsNot Nothing)
        End Function

        ''' <summary>
        ''' Sets whether focus traversal keys are enabled for this Component.
        ''' Components for which focus traversal keys are disabled receive key
        ''' events for focus traversal keys. Components for which focus traversal
        ''' keys are enabled do not see these events; instead, the events are
        ''' automatically converted to traversal operations.
        ''' </summary>
        ''' <param name="focusTraversalKeysEnabled"> whether focus traversal keys are
        '''        enabled for this Component </param>
        ''' <seealso cref= #getFocusTraversalKeysEnabled </seealso>
        ''' <seealso cref= #setFocusTraversalKeys </seealso>
        ''' <seealso cref= #getFocusTraversalKeys
        ''' @since 1.4
        ''' @beaninfo
        '''       bound: true </seealso>
        Public Overridable Property focusTraversalKeysEnabled As Boolean
            Set(ByVal focusTraversalKeysEnabled As Boolean)
                Dim oldFocusTraversalKeysEnabled As Boolean
                SyncLock Me
                    oldFocusTraversalKeysEnabled = Me.focusTraversalKeysEnabled
                    Me.focusTraversalKeysEnabled = focusTraversalKeysEnabled
                End SyncLock
                firePropertyChange("focusTraversalKeysEnabled", oldFocusTraversalKeysEnabled, focusTraversalKeysEnabled)
            End Set
            Get
                Return focusTraversalKeysEnabled
            End Get
        End Property


        ''' <summary>
        ''' Requests that this Component get the input focus, and that this
        ''' Component's top-level ancestor become the focused Window. This
        ''' component must be displayable, focusable, visible and all of
        ''' its ancestors (with the exception of the top-level Window) must
        ''' be visible for the request to be granted. Every effort will be
        ''' made to honor the request; however, in some cases it may be
        ''' impossible to do so. Developers must never assume that this
        ''' Component is the focus owner until this Component receives a
        ''' FOCUS_GAINED event. If this request is denied because this
        ''' Component's top-level Window cannot become the focused Window,
        ''' the request will be remembered and will be granted when the
        ''' Window is later focused by the user.
        ''' <p>
        ''' This method cannot be used to set the focus owner to no Component at
        ''' all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner()</code>
        ''' instead.
        ''' <p>
        ''' Because the focus behavior of this method is platform-dependent,
        ''' developers are strongly encouraged to use
        ''' <code>requestFocusInWindow</code> when possible.
        ''' 
        ''' <p>Note: Not all focus transfers result from invoking this method. As
        ''' such, a component may receive focus without this or any of the other
        ''' {@code requestFocus} methods of {@code Component} being invoked.
        ''' </summary>
        ''' <seealso cref= #requestFocusInWindow </seealso>
        ''' <seealso cref= java.awt.event.FocusEvent </seealso>
        ''' <seealso cref= #addFocusListener </seealso>
        ''' <seealso cref= #isFocusable </seealso>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
        ''' @since JDK1.0 </seealso>
        Public Overridable Sub requestFocus()
            requestFocusHelper(False, True)
        End Sub

        Friend Overridable Function requestFocus(ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
            Return requestFocusHelper(False, True, cause)
        End Function

        ''' <summary>
        ''' Requests that this <code>Component</code> get the input focus,
        ''' and that this <code>Component</code>'s top-level ancestor
        ''' become the focused <code>Window</code>. This component must be
        ''' displayable, focusable, visible and all of its ancestors (with
        ''' the exception of the top-level Window) must be visible for the
        ''' request to be granted. Every effort will be made to honor the
        ''' request; however, in some cases it may be impossible to do
        ''' so. Developers must never assume that this component is the
        ''' focus owner until this component receives a FOCUS_GAINED
        ''' event. If this request is denied because this component's
        ''' top-level window cannot become the focused window, the request
        ''' will be remembered and will be granted when the window is later
        ''' focused by the user.
        ''' <p>
        ''' This method returns a boolean value. If <code>false</code> is returned,
        ''' the request is <b>guaranteed to fail</b>. If <code>true</code> is
        ''' returned, the request will succeed <b>unless</b> it is vetoed, or an
        ''' extraordinary event, such as disposal of the component's peer, occurs
        ''' before the request can be granted by the native windowing system. Again,
        ''' while a return value of <code>true</code> indicates that the request is
        ''' likely to succeed, developers must never assume that this component is
        ''' the focus owner until this component receives a FOCUS_GAINED event.
        ''' <p>
        ''' This method cannot be used to set the focus owner to no component at
        ''' all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner</code>
        ''' instead.
        ''' <p>
        ''' Because the focus behavior of this method is platform-dependent,
        ''' developers are strongly encouraged to use
        ''' <code>requestFocusInWindow</code> when possible.
        ''' <p>
        ''' Every effort will be made to ensure that <code>FocusEvent</code>s
        ''' generated as a
        ''' result of this request will have the specified temporary value. However,
        ''' because specifying an arbitrary temporary state may not be implementable
        ''' on all native windowing systems, correct behavior for this method can be
        ''' guaranteed only for lightweight <code>Component</code>s.
        ''' This method is not intended
        ''' for general use, but exists instead as a hook for lightweight component
        ''' libraries, such as Swing.
        ''' 
        ''' <p>Note: Not all focus transfers result from invoking this method. As
        ''' such, a component may receive focus without this or any of the other
        ''' {@code requestFocus} methods of {@code Component} being invoked.
        ''' </summary>
        ''' <param name="temporary"> true if the focus change is temporary,
        '''        such as when the window loses the focus; for
        '''        more information on temporary focus changes see the
        ''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a> </param>
        ''' <returns> <code>false</code> if the focus change request is guaranteed to
        '''         fail; <code>true</code> if it is likely to succeed </returns>
        ''' <seealso cref= java.awt.event.FocusEvent </seealso>
        ''' <seealso cref= #addFocusListener </seealso>
        ''' <seealso cref= #isFocusable </seealso>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
        ''' @since 1.4 </seealso>
        Protected Friend Overridable Function requestFocus(ByVal temporary As Boolean) As Boolean
            Return requestFocusHelper(temporary, True)
        End Function

        Friend Overridable Function requestFocus(ByVal temporary As Boolean, ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
            Return requestFocusHelper(temporary, True, cause)
        End Function
        ''' <summary>
        ''' Requests that this Component get the input focus, if this
        ''' Component's top-level ancestor is already the focused
        ''' Window. This component must be displayable, focusable, visible
        ''' and all of its ancestors (with the exception of the top-level
        ''' Window) must be visible for the request to be granted. Every
        ''' effort will be made to honor the request; however, in some
        ''' cases it may be impossible to do so. Developers must never
        ''' assume that this Component is the focus owner until this
        ''' Component receives a FOCUS_GAINED event.
        ''' <p>
        ''' This method returns a boolean value. If <code>false</code> is returned,
        ''' the request is <b>guaranteed to fail</b>. If <code>true</code> is
        ''' returned, the request will succeed <b>unless</b> it is vetoed, or an
        ''' extraordinary event, such as disposal of the Component's peer, occurs
        ''' before the request can be granted by the native windowing system. Again,
        ''' while a return value of <code>true</code> indicates that the request is
        ''' likely to succeed, developers must never assume that this Component is
        ''' the focus owner until this Component receives a FOCUS_GAINED event.
        ''' <p>
        ''' This method cannot be used to set the focus owner to no Component at
        ''' all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner()</code>
        ''' instead.
        ''' <p>
        ''' The focus behavior of this method can be implemented uniformly across
        ''' platforms, and thus developers are strongly encouraged to use this
        ''' method over <code>requestFocus</code> when possible. Code which relies
        ''' on <code>requestFocus</code> may exhibit different focus behavior on
        ''' different platforms.
        ''' 
        ''' <p>Note: Not all focus transfers result from invoking this method. As
        ''' such, a component may receive focus without this or any of the other
        ''' {@code requestFocus} methods of {@code Component} being invoked.
        ''' </summary>
        ''' <returns> <code>false</code> if the focus change request is guaranteed to
        '''         fail; <code>true</code> if it is likely to succeed </returns>
        ''' <seealso cref= #requestFocus </seealso>
        ''' <seealso cref= java.awt.event.FocusEvent </seealso>
        ''' <seealso cref= #addFocusListener </seealso>
        ''' <seealso cref= #isFocusable </seealso>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
        ''' @since 1.4 </seealso>
        Public Overridable Function requestFocusInWindow() As Boolean
            Return requestFocusHelper(False, False)
        End Function

        Friend Overridable Function requestFocusInWindow(ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
            Return requestFocusHelper(False, False, cause)
        End Function

        ''' <summary>
        ''' Requests that this <code>Component</code> get the input focus,
        ''' if this <code>Component</code>'s top-level ancestor is already
        ''' the focused <code>Window</code>.  This component must be
        ''' displayable, focusable, visible and all of its ancestors (with
        ''' the exception of the top-level Window) must be visible for the
        ''' request to be granted. Every effort will be made to honor the
        ''' request; however, in some cases it may be impossible to do
        ''' so. Developers must never assume that this component is the
        ''' focus owner until this component receives a FOCUS_GAINED event.
        ''' <p>
        ''' This method returns a boolean value. If <code>false</code> is returned,
        ''' the request is <b>guaranteed to fail</b>. If <code>true</code> is
        ''' returned, the request will succeed <b>unless</b> it is vetoed, or an
        ''' extraordinary event, such as disposal of the component's peer, occurs
        ''' before the request can be granted by the native windowing system. Again,
        ''' while a return value of <code>true</code> indicates that the request is
        ''' likely to succeed, developers must never assume that this component is
        ''' the focus owner until this component receives a FOCUS_GAINED event.
        ''' <p>
        ''' This method cannot be used to set the focus owner to no component at
        ''' all. Use <code>KeyboardFocusManager.clearGlobalFocusOwner</code>
        ''' instead.
        ''' <p>
        ''' The focus behavior of this method can be implemented uniformly across
        ''' platforms, and thus developers are strongly encouraged to use this
        ''' method over <code>requestFocus</code> when possible. Code which relies
        ''' on <code>requestFocus</code> may exhibit different focus behavior on
        ''' different platforms.
        ''' <p>
        ''' Every effort will be made to ensure that <code>FocusEvent</code>s
        ''' generated as a
        ''' result of this request will have the specified temporary value. However,
        ''' because specifying an arbitrary temporary state may not be implementable
        ''' on all native windowing systems, correct behavior for this method can be
        ''' guaranteed only for lightweight components. This method is not intended
        ''' for general use, but exists instead as a hook for lightweight component
        ''' libraries, such as Swing.
        ''' 
        ''' <p>Note: Not all focus transfers result from invoking this method. As
        ''' such, a component may receive focus without this or any of the other
        ''' {@code requestFocus} methods of {@code Component} being invoked.
        ''' </summary>
        ''' <param name="temporary"> true if the focus change is temporary,
        '''        such as when the window loses the focus; for
        '''        more information on temporary focus changes see the
        ''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a> </param>
        ''' <returns> <code>false</code> if the focus change request is guaranteed to
        '''         fail; <code>true</code> if it is likely to succeed </returns>
        ''' <seealso cref= #requestFocus </seealso>
        ''' <seealso cref= java.awt.event.FocusEvent </seealso>
        ''' <seealso cref= #addFocusListener </seealso>
        ''' <seealso cref= #isFocusable </seealso>
        ''' <seealso cref= #isDisplayable </seealso>
        ''' <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner
        ''' @since 1.4 </seealso>
        Protected Friend Overridable Function requestFocusInWindow(ByVal temporary As Boolean) As Boolean
            Return requestFocusHelper(temporary, False)
        End Function

        Friend Overridable Function requestFocusInWindow(ByVal temporary As Boolean, ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
            Return requestFocusHelper(temporary, False, cause)
        End Function

        Friend Function requestFocusHelper(ByVal temporary As Boolean, ByVal focusedWindowChangeAllowed As Boolean) As Boolean
            Return requestFocusHelper(temporary, focusedWindowChangeAllowed, sun.awt.CausedFocusEvent.Cause.UNKNOWN)
        End Function

        Friend Function requestFocusHelper(ByVal temporary As Boolean, ByVal focusedWindowChangeAllowed As Boolean, ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
            ' 1) Check if the event being dispatched is a system-generated mouse event.
            Dim currentEvent As AWTEvent = EventQueue.currentEvent
            If TypeOf currentEvent Is MouseEvent AndAlso sun.awt.SunToolkit.isSystemGenerated(currentEvent) Then
                ' 2) Sanity check: if the mouse event component source belongs to the same containing window.
                Dim source As Component = CType(currentEvent, MouseEvent).component
                If source Is Nothing OrElse source.containingWindow Is containingWindow Then
                    focusLog.finest("requesting focus by mouse event ""in window""")

                    ' If both the conditions are fulfilled the focus request should be strictly
                    ' bounded by the toplevel window. It's assumed that the mouse event activates
                    ' the window (if it wasn't active) and this makes it possible for a focus
                    ' request with a strong in-window requirement to change focus in the bounds
                    ' of the toplevel. If, by any means, due to asynchronous nature of the event
                    ' dispatching mechanism, the window happens to be natively inactive by the time
                    ' this focus request is eventually handled, it should not re-activate the
                    ' toplevel. Otherwise the result may not meet user expectations. See 6981400.
                    focusedWindowChangeAllowed = False
                End If
            End If
            If Not isRequestFocusAccepted(temporary, focusedWindowChangeAllowed, cause) Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("requestFocus is not accepted")
                Return False
            End If
            ' Update most-recent map
            KeyboardFocusManager.mostRecentFocusOwner = Me

            Dim window_Renamed As Component = Me
            Do While (window_Renamed IsNot Nothing) AndAlso Not (TypeOf window_Renamed Is Window)
                If Not window_Renamed.visible Then
                    If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("component is recurively invisible")
                    Return False
                End If
                window_Renamed = window_Renamed.parent
            Loop

            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            Dim heavyweight As Component = If(TypeOf peer_Renamed Is java.awt.peer.LightweightPeer, nativeContainer, Me)
            If heavyweight Is Nothing OrElse (Not heavyweight.visible) Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Component is not a part of visible hierarchy")
                Return False
            End If
            peer_Renamed = heavyweight.peer
            If peer_Renamed Is Nothing Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Peer is null")
                Return False
            End If

            ' Focus this Component
            Dim time As Long = 0
            If EventQueue.dispatchThread Then
                time = toolkit.eventQueue.mostRecentKeyEventTime
            Else
                ' A focus request made from outside EDT should not be associated with any event
                ' and so its time stamp is simply set to the current time.
                time = System.currentTimeMillis()
            End If

            Dim success As Boolean = peer_Renamed.requestFocus(Me, temporary, focusedWindowChangeAllowed, time, cause)
            If Not success Then
                KeyboardFocusManager.getCurrentKeyboardFocusManager(appContext).dequeueKeyEvents(time, Me)
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Peer request failed")
            Else
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Pass for " & Me)
            End If
            Return success
        End Function

        Private Function isRequestFocusAccepted(ByVal temporary As Boolean, ByVal focusedWindowChangeAllowed As Boolean, ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
            If (Not focusable) OrElse (Not visible) Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Not focusable or not visible")
                Return False
            End If

            Dim peer_Renamed As java.awt.peer.ComponentPeer = Me.peer
            If peer_Renamed Is Nothing Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("peer is null")
                Return False
            End If

            Dim window_Renamed As Window = containingWindow
            If window_Renamed Is Nothing OrElse (Not window_Renamed.focusableWindow) Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Component doesn't have toplevel")
                Return False
            End If

            ' We have passed all regular checks for focus request,
            ' now let's call RequestFocusController and see what it says.
            Dim focusOwner_Renamed As Component = KeyboardFocusManager.getMostRecentFocusOwner(window_Renamed)
            If focusOwner_Renamed Is Nothing Then
                ' sometimes most recent focus owner may be null, but focus owner is not
                ' e.g. we reset most recent focus owner if user removes focus owner
                focusOwner_Renamed = KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
                If focusOwner_Renamed IsNot Nothing AndAlso focusOwner_Renamed.containingWindow IsNot window_Renamed Then focusOwner_Renamed = Nothing
            End If

            If focusOwner_Renamed Is Me OrElse focusOwner_Renamed Is Nothing Then
                ' Controller is supposed to verify focus transfers and for this it
                ' should know both from and to components.  And it shouldn't verify
                ' transfers from when these components are equal.
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("focus owner is null or this")
                Return True
            End If

            If sun.awt.CausedFocusEvent.Cause.ACTIVATION Is cause Then
                ' we shouldn't call RequestFocusController in case we are
                ' in activation.  We do request focus on component which
                ' has got temporary focus lost and then on component which is
                ' most recent focus owner.  But most recent focus owner can be
                ' changed by requestFocsuXXX() call only, so this transfer has
                ' been already approved.
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("cause is activation")
                Return True
            End If

            Dim ret As Boolean = Component.requestFocusController.acceptRequestFocus(focusOwner_Renamed, Me, temporary, focusedWindowChangeAllowed, cause)
            If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("RequestFocusController returns {0}", ret)

            Return ret
        End Function

        Private Shared requestFocusController As sun.awt.RequestFocusController = New DummyRequestFocusController

        ' Swing access this method through reflection to implement InputVerifier's functionality.
        ' Perhaps, we should make this method public (later ;)
        Private Class DummyRequestFocusController
            Implements sun.awt.RequestFocusController

            Public Overridable Function acceptRequestFocus(ByVal [from] As Component, ByVal [to] As Component, ByVal temporary As Boolean, ByVal focusedWindowChangeAllowed As Boolean, ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean
                Return True
            End Function
        End Class

        <MethodImpl(MethodImplOptions.Synchronized)>
        Friend Shared Property requestFocusController As sun.awt.RequestFocusController
            Set(ByVal requestController As sun.awt.RequestFocusController)
                If requestController Is Nothing Then
                    requestFocusController = New DummyRequestFocusController
                Else
                    requestFocusController = requestController
                End If
            End Set
        End Property

        ''' <summary>
        ''' Returns the Container which is the focus cycle root of this Component's
        ''' focus traversal cycle. Each focus traversal cycle has only a single
        ''' focus cycle root and each Component which is not a Container belongs to
        ''' only a single focus traversal cycle. Containers which are focus cycle
        ''' roots belong to two cycles: one rooted at the Container itself, and one
        ''' rooted at the Container's nearest focus-cycle-root ancestor. For such
        ''' Containers, this method will return the Container's nearest focus-cycle-
        ''' root ancestor.
        ''' </summary>
        ''' <returns> this Component's nearest focus-cycle-root ancestor </returns>
        ''' <seealso cref= Container#isFocusCycleRoot()
        ''' @since 1.4 </seealso>
        Public Overridable Property focusCycleRootAncestor As Container
            Get
                Dim rootAncestor As Container = Me.parent
                Do While rootAncestor IsNot Nothing AndAlso Not rootAncestor.focusCycleRoot
                    rootAncestor = rootAncestor.parent
                Loop
                Return rootAncestor
            End Get
        End Property

        ''' <summary>
        ''' Returns whether the specified Container is the focus cycle root of this
        ''' Component's focus traversal cycle. Each focus traversal cycle has only
        ''' a single focus cycle root and each Component which is not a Container
        ''' belongs to only a single focus traversal cycle.
        ''' </summary>
        ''' <param name="container"> the Container to be tested </param>
        ''' <returns> <code>true</code> if the specified Container is a focus-cycle-
        '''         root of this Component; <code>false</code> otherwise </returns>
        ''' <seealso cref= Container#isFocusCycleRoot()
        ''' @since 1.4 </seealso>
        Public Overridable Function isFocusCycleRoot(ByVal container_Renamed As Container) As Boolean
            Dim rootAncestor As Container = focusCycleRootAncestor
            Return (rootAncestor Is container_Renamed)
        End Function

        Friend Overridable Property traversalRoot As Container
            Get
                Return focusCycleRootAncestor
            End Get
        End Property

        ''' <summary>
        ''' Transfers the focus to the next component, as though this Component were
        ''' the focus owner. </summary>
        ''' <seealso cref=       #requestFocus()
        ''' @since     JDK1.1 </seealso>
        Public Overridable Sub transferFocus()
            nextFocus()
        End Sub

        ''' @deprecated As of JDK version 1.1,
        ''' replaced by transferFocus(). 
        <Obsolete("As of JDK version 1.1,")>
        Public Overridable Sub nextFocus()
            transferFocus(False)
        End Sub

        Friend Overridable Function transferFocus(ByVal clearOnFailure As Boolean) As Boolean
            If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("clearOnFailure = " & clearOnFailure)
            Dim toFocus As Component = nextFocusCandidate
            Dim res As Boolean = False
            If toFocus IsNot Nothing AndAlso (Not toFocus.focusOwner) AndAlso toFocus IsNot Me Then res = toFocus.requestFocusInWindow(sun.awt.CausedFocusEvent.Cause.TRAVERSAL_FORWARD)
            If clearOnFailure AndAlso (Not res) Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("clear global focus owner")
                KeyboardFocusManager.currentKeyboardFocusManager.clearGlobalFocusOwnerPriv()
            End If
            If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("returning result: " & res)
            Return res
        End Function

        Friend Property nextFocusCandidate As Component
            Get
                Dim rootAncestor As Container = traversalRoot
                Dim comp As Component = Me
                Do While rootAncestor IsNot Nothing AndAlso Not (rootAncestor.showing AndAlso rootAncestor.canBeFocusOwner())
                    comp = rootAncestor
                    rootAncestor = comp.focusCycleRootAncestor
                Loop
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("comp = " & comp & ", root = " & rootAncestor)
                Dim candidate As Component = Nothing
                If rootAncestor IsNot Nothing Then
                    Dim policy As FocusTraversalPolicy = rootAncestor.focusTraversalPolicy
                    Dim toFocus As Component = policy.getComponentAfter(rootAncestor, comp)
                    If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("component after is " & toFocus)
                    If toFocus Is Nothing Then
                        toFocus = policy.getDefaultComponent(rootAncestor)
                        If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("default component is " & toFocus)
                    End If
                    If toFocus Is Nothing Then
                        Dim applet As java.applet.Applet = sun.awt.EmbeddedFrame.getAppletIfAncestorOf(Me)
                        If applet IsNot Nothing Then toFocus = applet
                    End If
                    candidate = toFocus
                End If
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("Focus transfer candidate: " & candidate)
                Return candidate
            End Get
        End Property

        ''' <summary>
        ''' Transfers the focus to the previous component, as though this Component
        ''' were the focus owner. </summary>
        ''' <seealso cref=       #requestFocus()
        ''' @since     1.4 </seealso>
        Public Overridable Sub transferFocusBackward()
            transferFocusBackward(False)
        End Sub

        Friend Overridable Function transferFocusBackward(ByVal clearOnFailure As Boolean) As Boolean
            Dim rootAncestor As Container = traversalRoot
            Dim comp As Component = Me
            Do While rootAncestor IsNot Nothing AndAlso Not (rootAncestor.showing AndAlso rootAncestor.canBeFocusOwner())
                comp = rootAncestor
                rootAncestor = comp.focusCycleRootAncestor
            Loop
            Dim res As Boolean = False
            If rootAncestor IsNot Nothing Then
                Dim policy As FocusTraversalPolicy = rootAncestor.focusTraversalPolicy
                Dim toFocus As Component = policy.getComponentBefore(rootAncestor, comp)
                If toFocus Is Nothing Then toFocus = policy.getDefaultComponent(rootAncestor)
                If toFocus IsNot Nothing Then res = toFocus.requestFocusInWindow(sun.awt.CausedFocusEvent.Cause.TRAVERSAL_BACKWARD)
            End If
            If clearOnFailure AndAlso (Not res) Then
                If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("clear global focus owner")
                KeyboardFocusManager.currentKeyboardFocusManager.clearGlobalFocusOwnerPriv()
            End If
            If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("returning result: " & res)
            Return res
        End Function

        ''' <summary>
        ''' Transfers the focus up one focus traversal cycle. Typically, the focus
        ''' owner is set to this Component's focus cycle root, and the current focus
        ''' cycle root is set to the new focus owner's focus cycle root. If,
        ''' however, this Component's focus cycle root is a Window, then the focus
        ''' owner is set to the focus cycle root's default Component to focus, and
        ''' the current focus cycle root is unchanged.
        ''' </summary>
        ''' <seealso cref=       #requestFocus() </seealso>
        ''' <seealso cref=       Container#isFocusCycleRoot() </seealso>
        ''' <seealso cref=       Container#setFocusCycleRoot(boolean)
        ''' @since     1.4 </seealso>
        Public Overridable Sub transferFocusUpCycle()
            Dim rootAncestor As Container
            rootAncestor = focusCycleRootAncestor
            Do While rootAncestor IsNot Nothing AndAlso Not (rootAncestor.showing AndAlso rootAncestor.focusable AndAlso rootAncestor.enabled)
                rootAncestor = rootAncestor.focusCycleRootAncestor
            Loop

            If rootAncestor IsNot Nothing Then
                Dim rootAncestorRootAncestor As Container = rootAncestor.focusCycleRootAncestor
                Dim fcr As Container = If(rootAncestorRootAncestor IsNot Nothing, rootAncestorRootAncestor, rootAncestor)

                KeyboardFocusManager.currentKeyboardFocusManager.globalCurrentFocusCycleRootPriv = fcr
                rootAncestor.requestFocus(sun.awt.CausedFocusEvent.Cause.TRAVERSAL_UP)
            Else
                Dim window_Renamed As Window = containingWindow

                If window_Renamed IsNot Nothing Then
                    Dim toFocus As Component = window_Renamed.focusTraversalPolicy.getDefaultComponent(window_Renamed)
                    If toFocus IsNot Nothing Then
                        KeyboardFocusManager.currentKeyboardFocusManager.globalCurrentFocusCycleRootPriv = window_Renamed
                        toFocus.requestFocus(sun.awt.CausedFocusEvent.Cause.TRAVERSAL_UP)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Returns <code>true</code> if this <code>Component</code> is the
        ''' focus owner.  This method is obsolete, and has been replaced by
        ''' <code>isFocusOwner()</code>.
        ''' </summary>
        ''' <returns> <code>true</code> if this <code>Component</code> is the
        '''         focus owner; <code>false</code> otherwise
        ''' @since 1.2 </returns>
        Public Overridable Function hasFocus() As Boolean
            Return (KeyboardFocusManager.currentKeyboardFocusManager.focusOwner Is Me)
        End Function

        ''' <summary>
        ''' Returns <code>true</code> if this <code>Component</code> is the
        '''    focus owner.
        ''' </summary>
        ''' <returns> <code>true</code> if this <code>Component</code> is the
        '''     focus owner; <code>false</code> otherwise
        ''' @since 1.4 </returns>
        Public Overridable Property focusOwner As Boolean
            Get
                Return hasFocus()
            End Get
        End Property

        '    
        '     * Used to disallow auto-focus-transfer on disposal of the focus owner
        '     * in the process of disposing its parent container.
        '     
        Private autoFocusTransferOnDisposal As Boolean = True

        Friend Overridable Property autoFocusTransferOnDisposal As Boolean
            Set(ByVal value As Boolean)
                autoFocusTransferOnDisposal = value
            End Set
            Get
                Return autoFocusTransferOnDisposal
            End Get
        End Property


        ''' <summary>
        ''' Adds the specified popup menu to the component. </summary>
        ''' <param name="popup"> the popup menu to be added to the component. </param>
        ''' <seealso cref=       #remove(MenuComponent) </seealso>
        ''' <exception cref="NullPointerException"> if {@code popup} is {@code null}
        ''' @since     JDK1.1 </exception>
        Public Overridable Sub add(ByVal popup As PopupMenu)
            SyncLock treeLock
                If popup.parent IsNot Nothing Then popup.parent.remove(popup)
                If popups Is Nothing Then popups = New List(Of PopupMenu)
                popups.Add(popup)
                popup.parent = Me

                If peer IsNot Nothing Then
                    If popup.peer Is Nothing Then popup.addNotify()
                End If
            End SyncLock
        End Sub

        ''' <summary>
        ''' Removes the specified popup menu from the component. </summary>
        ''' <param name="popup"> the popup menu to be removed </param>
        ''' <seealso cref=       #add(PopupMenu)
        ''' @since     JDK1.1 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overridable Sub remove(ByVal popup As MenuComponent) Implements MenuContainer.remove
            SyncLock treeLock
                If popups Is Nothing Then Return
                Dim index As Integer = popups.IndexOf(popup)
                If index >= 0 Then
                    Dim pmenu As PopupMenu = CType(popup, PopupMenu)
                    If pmenu.peer IsNot Nothing Then pmenu.removeNotify()
                    pmenu.parent = Nothing
                    popups.RemoveAt(index)
                    If popups.Count = 0 Then popups = Nothing
                End If
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns a string representing the state of this component. This
        ''' method is intended to be used only for debugging purposes, and the
        ''' content and format of the returned string may vary between
        ''' implementations. The returned string may be empty but may not be
        ''' <code>null</code>.
        ''' </summary>
        ''' <returns>  a string representation of this component's state
        ''' @since     JDK1.0 </returns>
        Protected Friend Overridable Function paramString() As String
            Dim thisName As String = java.util.Objects.ToString(name, "")
            Dim invalid As String = If(valid, "", ",invalid")
            Dim hidden As String = If(visible, "", ",hidden")
            Dim disabled As String = If(enabled, "", ",disabled")
            Return thisName + AscW(","c) + x + AscW(","c) + y + AscW(","c) + width + AscW("x"c) + height + invalid + hidden + disabled
        End Function

        ''' <summary>
        ''' Returns a string representation of this component and its values. </summary>
        ''' <returns>    a string representation of this component
        ''' @since     JDK1.0 </returns>
        Public Overrides Function ToString() As String
            Return Me.GetType().Name + AscW("["c) + paramString() + AscW("]"c)
        End Function

        ''' <summary>
        ''' Prints a listing of this component to the standard system output
        ''' stream <code>System.out</code>. </summary>
        ''' <seealso cref=       java.lang.System#out
        ''' @since     JDK1.0 </seealso>
        Public Overridable Sub list()
            list(System.out, 0)
        End Sub

        ''' <summary>
        ''' Prints a listing of this component to the specified output
        ''' stream. </summary>
        ''' <param name="out">   a print stream </param>
        ''' <exception cref="NullPointerException"> if {@code out} is {@code null}
        ''' @since    JDK1.0 </exception>
        Public Overridable Sub list(ByVal out As java.io.PrintStream)
            list(out, 0)
        End Sub

        ''' <summary>
        ''' Prints out a list, starting at the specified indentation, to the
        ''' specified print stream. </summary>
        ''' <param name="out">      a print stream </param>
        ''' <param name="indent">   number of spaces to indent </param>
        ''' <seealso cref=       java.io.PrintStream#println(java.lang.Object) </seealso>
        ''' <exception cref="NullPointerException"> if {@code out} is {@code null}
        ''' @since     JDK1.0 </exception>
        Public Overridable Sub list(ByVal out As java.io.PrintStream, ByVal indent As Integer)
            For i As Integer = 0 To indent - 1
                out.print(" ")
            Next i
            out.println(Me)
        End Sub

        ''' <summary>
        ''' Prints a listing to the specified print writer. </summary>
        ''' <param name="out">  the print writer to print to </param>
        ''' <exception cref="NullPointerException"> if {@code out} is {@code null}
        ''' @since JDK1.1 </exception>
        Public Overridable Sub list(ByVal out As java.io.PrintWriter)
            list(out, 0)
        End Sub

        ''' <summary>
        ''' Prints out a list, starting at the specified indentation, to
        ''' the specified print writer. </summary>
        ''' <param name="out"> the print writer to print to </param>
        ''' <param name="indent"> the number of spaces to indent </param>
        ''' <exception cref="NullPointerException"> if {@code out} is {@code null} </exception>
        ''' <seealso cref=       java.io.PrintStream#println(java.lang.Object)
        ''' @since JDK1.1 </seealso>
        Public Overridable Sub list(ByVal out As java.io.PrintWriter, ByVal indent As Integer)
            For i As Integer = 0 To indent - 1
                out.print(" ")
            Next i
            out.println(Me)
        End Sub

        '    
        '     * Fetches the native container somewhere higher up in the component
        '     * tree that contains this component.
        '     
        Friend Property nativeContainer As Container
            Get
                Dim p As Container = container
                Do While p IsNot Nothing AndAlso TypeOf p.peer Is java.awt.peer.LightweightPeer
                    p = p.container
                Loop
                Return p
            End Get
        End Property

        ''' <summary>
        ''' Adds a PropertyChangeListener to the listener list. The listener is
        ''' registered for all bound properties of this [Class], including the
        ''' following:
        ''' <ul>
        '''    <li>this Component's font ("font")</li>
        '''    <li>this Component's background color ("background")</li>
        '''    <li>this Component's foreground color ("foreground")</li>
        '''    <li>this Component's focusability ("focusable")</li>
        '''    <li>this Component's focus traversal keys enabled state
        '''        ("focusTraversalKeysEnabled")</li>
        '''    <li>this Component's Set of FORWARD_TRAVERSAL_KEYS
        '''        ("forwardFocusTraversalKeys")</li>
        '''    <li>this Component's Set of BACKWARD_TRAVERSAL_KEYS
        '''        ("backwardFocusTraversalKeys")</li>
        '''    <li>this Component's Set of UP_CYCLE_TRAVERSAL_KEYS
        '''        ("upCycleFocusTraversalKeys")</li>
        '''    <li>this Component's preferred size ("preferredSize")</li>
        '''    <li>this Component's minimum size ("minimumSize")</li>
        '''    <li>this Component's maximum size ("maximumSize")</li>
        '''    <li>this Component's name ("name")</li>
        ''' </ul>
        ''' Note that if this <code>Component</code> is inheriting a bound property, then no
        ''' event will be fired in response to a change in the inherited property.
        ''' <p>
        ''' If <code>listener</code> is <code>null</code>,
        ''' no exception is thrown and no action is performed.
        ''' </summary>
        ''' <param name="listener">  the property change listener to be added
        ''' </param>
        ''' <seealso cref= #removePropertyChangeListener </seealso>
        ''' <seealso cref= #getPropertyChangeListeners </seealso>
        ''' <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
            SyncLock objectLock
                If listener Is Nothing Then Return
                If changeSupport Is Nothing Then changeSupport = New java.beans.PropertyChangeSupport(Me)
                changeSupport.addPropertyChangeListener(listener)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Removes a PropertyChangeListener from the listener list. This method
        ''' should be used to remove PropertyChangeListeners that were registered
        ''' for all bound properties of this class.
        ''' <p>
        ''' If listener is null, no exception is thrown and no action is performed.
        ''' </summary>
        ''' <param name="listener"> the PropertyChangeListener to be removed
        ''' </param>
        ''' <seealso cref= #addPropertyChangeListener </seealso>
        ''' <seealso cref= #getPropertyChangeListeners </seealso>
        ''' <seealso cref= #removePropertyChangeListener(java.lang.String,java.beans.PropertyChangeListener) </seealso>
        Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
            SyncLock objectLock
                If listener Is Nothing OrElse changeSupport Is Nothing Then Return
                changeSupport.removePropertyChangeListener(listener)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns an array of all the property change listeners
        ''' registered on this component.
        ''' </summary>
        ''' <returns> all of this component's <code>PropertyChangeListener</code>s
        '''         or an empty array if no property change
        '''         listeners are currently registered
        ''' </returns>
        ''' <seealso cref=      #addPropertyChangeListener </seealso>
        ''' <seealso cref=      #removePropertyChangeListener </seealso>
        ''' <seealso cref=      #getPropertyChangeListeners(java.lang.String) </seealso>
        ''' <seealso cref=      java.beans.PropertyChangeSupport#getPropertyChangeListeners
        ''' @since    1.4 </seealso>
        Public Overridable Property propertyChangeListeners As java.beans.PropertyChangeListener()
            Get
                SyncLock objectLock
                    If changeSupport Is Nothing Then Return New java.beans.PropertyChangeListener() {}
                    Return changeSupport.propertyChangeListeners
                End SyncLock
            End Get
        End Property

        ''' <summary>
        ''' Adds a PropertyChangeListener to the listener list for a specific
        ''' property. The specified property may be user-defined, or one of the
        ''' following:
        ''' <ul>
        '''    <li>this Component's font ("font")</li>
        '''    <li>this Component's background color ("background")</li>
        '''    <li>this Component's foreground color ("foreground")</li>
        '''    <li>this Component's focusability ("focusable")</li>
        '''    <li>this Component's focus traversal keys enabled state
        '''        ("focusTraversalKeysEnabled")</li>
        '''    <li>this Component's Set of FORWARD_TRAVERSAL_KEYS
        '''        ("forwardFocusTraversalKeys")</li>
        '''    <li>this Component's Set of BACKWARD_TRAVERSAL_KEYS
        '''        ("backwardFocusTraversalKeys")</li>
        '''    <li>this Component's Set of UP_CYCLE_TRAVERSAL_KEYS
        '''        ("upCycleFocusTraversalKeys")</li>
        ''' </ul>
        ''' Note that if this <code>Component</code> is inheriting a bound property, then no
        ''' event will be fired in response to a change in the inherited property.
        ''' <p>
        ''' If <code>propertyName</code> or <code>listener</code> is <code>null</code>,
        ''' no exception is thrown and no action is taken.
        ''' </summary>
        ''' <param name="propertyName"> one of the property names listed above </param>
        ''' <param name="listener"> the property change listener to be added
        ''' </param>
        ''' <seealso cref= #removePropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        ''' <seealso cref= #getPropertyChangeListeners(java.lang.String) </seealso>
        ''' <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        Public Overridable Sub addPropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
            SyncLock objectLock
                If listener Is Nothing Then Return
                If changeSupport Is Nothing Then changeSupport = New java.beans.PropertyChangeSupport(Me)
                changeSupport.addPropertyChangeListener(propertyName, listener)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Removes a <code>PropertyChangeListener</code> from the listener
        ''' list for a specific property. This method should be used to remove
        ''' <code>PropertyChangeListener</code>s
        ''' that were registered for a specific bound property.
        ''' <p>
        ''' If <code>propertyName</code> or <code>listener</code> is <code>null</code>,
        ''' no exception is thrown and no action is taken.
        ''' </summary>
        ''' <param name="propertyName"> a valid property name </param>
        ''' <param name="listener"> the PropertyChangeListener to be removed
        ''' </param>
        ''' <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        ''' <seealso cref= #getPropertyChangeListeners(java.lang.String) </seealso>
        ''' <seealso cref= #removePropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
        Public Overridable Sub removePropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
            SyncLock objectLock
                If listener Is Nothing OrElse changeSupport Is Nothing Then Return
                changeSupport.removePropertyChangeListener(propertyName, listener)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns an array of all the listeners which have been associated
        ''' with the named property.
        ''' </summary>
        ''' <returns> all of the <code>PropertyChangeListener</code>s associated with
        '''         the named property; if no such listeners have been added or
        '''         if <code>propertyName</code> is <code>null</code>, an empty
        '''         array is returned
        ''' </returns>
        ''' <seealso cref= #addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        ''' <seealso cref= #removePropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        ''' <seealso cref= #getPropertyChangeListeners
        ''' @since 1.4 </seealso>
        Public Overridable Function getPropertyChangeListeners(ByVal propertyName As String) As java.beans.PropertyChangeListener()
            SyncLock objectLock
                If changeSupport Is Nothing Then Return New java.beans.PropertyChangeListener() {}
                Return changeSupport.getPropertyChangeListeners(propertyName)
            End SyncLock
        End Function

        ''' <summary>
        ''' Support for reporting bound property changes for Object properties.
        ''' This method can be called when a bound property has changed and it will
        ''' send the appropriate PropertyChangeEvent to any registered
        ''' PropertyChangeListeners.
        ''' </summary>
        ''' <param name="propertyName"> the property whose value has changed </param>
        ''' <param name="oldValue"> the property's previous value </param>
        ''' <param name="newValue"> the property's new value </param>
        Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
            Dim changeSupport As java.beans.PropertyChangeSupport
            SyncLock objectLock
                changeSupport = Me.changeSupport
            End SyncLock
            If changeSupport Is Nothing OrElse (oldValue IsNot Nothing AndAlso newValue IsNot Nothing AndAlso oldValue.Equals(newValue)) Then Return
            changeSupport.firePropertyChange(propertyName, oldValue, newValue)
        End Sub

        ''' <summary>
        ''' Support for reporting bound property changes for boolean properties.
        ''' This method can be called when a bound property has changed and it will
        ''' send the appropriate PropertyChangeEvent to any registered
        ''' PropertyChangeListeners.
        ''' </summary>
        ''' <param name="propertyName"> the property whose value has changed </param>
        ''' <param name="oldValue"> the property's previous value </param>
        ''' <param name="newValue"> the property's new value
        ''' @since 1.4 </param>
        Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Boolean, ByVal newValue As Boolean)
            Dim changeSupport As java.beans.PropertyChangeSupport = Me.changeSupport
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            changeSupport.firePropertyChange(propertyName, oldValue, newValue)
        End Sub

        ''' <summary>
        ''' Support for reporting bound property changes for integer properties.
        ''' This method can be called when a bound property has changed and it will
        ''' send the appropriate PropertyChangeEvent to any registered
        ''' PropertyChangeListeners.
        ''' </summary>
        ''' <param name="propertyName"> the property whose value has changed </param>
        ''' <param name="oldValue"> the property's previous value </param>
        ''' <param name="newValue"> the property's new value
        ''' @since 1.4 </param>
        Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Integer, ByVal newValue As Integer)
            Dim changeSupport As java.beans.PropertyChangeSupport = Me.changeSupport
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            changeSupport.firePropertyChange(propertyName, oldValue, newValue)
        End Sub

        ''' <summary>
        ''' Reports a bound property change.
        ''' </summary>
        ''' <param name="propertyName"> the programmatic name of the property
        '''          that was changed </param>
        ''' <param name="oldValue"> the old value of the property (as a byte) </param>
        ''' <param name="newValue"> the new value of the property (as a byte) </param>
        ''' <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
        '''          java.lang.Object)
        ''' @since 1.5 </seealso>
        Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As SByte, ByVal newValue As SByte)
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            firePropertyChange(propertyName, Convert.ToByte(oldValue), Convert.ToByte(newValue))
        End Sub

        ''' <summary>
        ''' Reports a bound property change.
        ''' </summary>
        ''' <param name="propertyName"> the programmatic name of the property
        '''          that was changed </param>
        ''' <param name="oldValue"> the old value of the property (as a char) </param>
        ''' <param name="newValue"> the new value of the property (as a char) </param>
        ''' <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
        '''          java.lang.Object)
        ''' @since 1.5 </seealso>
        Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Char, ByVal newValue As Char)
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            firePropertyChange(propertyName, New Character(oldValue), New Character(newValue))
        End Sub

        ''' <summary>
        ''' Reports a bound property change.
        ''' </summary>
        ''' <param name="propertyName"> the programmatic name of the property
        '''          that was changed </param>
        ''' <param name="oldValue"> the old value of the property (as a short) </param>
        ''' <param name="newValue"> the old value of the property (as a short) </param>
        ''' <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
        '''          java.lang.Object)
        ''' @since 1.5 </seealso>
        Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Short, ByVal newValue As Short)
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            firePropertyChange(propertyName, Convert.ToInt16(oldValue), Convert.ToInt16(newValue))
        End Sub


        ''' <summary>
        ''' Reports a bound property change.
        ''' </summary>
        ''' <param name="propertyName"> the programmatic name of the property
        '''          that was changed </param>
        ''' <param name="oldValue"> the old value of the property (as a long) </param>
        ''' <param name="newValue"> the new value of the property (as a long) </param>
        ''' <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
        '''          java.lang.Object)
        ''' @since 1.5 </seealso>
        Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Long, ByVal newValue As Long)
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            firePropertyChange(propertyName, Convert.ToInt64(oldValue), Convert.ToInt64(newValue))
        End Sub

        ''' <summary>
        ''' Reports a bound property change.
        ''' </summary>
        ''' <param name="propertyName"> the programmatic name of the property
        '''          that was changed </param>
        ''' <param name="oldValue"> the old value of the property (as a float) </param>
        ''' <param name="newValue"> the new value of the property (as a float) </param>
        ''' <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
        '''          java.lang.Object)
        ''' @since 1.5 </seealso>
        Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Single, ByVal newValue As Single)
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            firePropertyChange(propertyName, Convert.ToSingle(oldValue), Convert.ToSingle(newValue))
        End Sub

        ''' <summary>
        ''' Reports a bound property change.
        ''' </summary>
        ''' <param name="propertyName"> the programmatic name of the property
        '''          that was changed </param>
        ''' <param name="oldValue"> the old value of the property (as a double) </param>
        ''' <param name="newValue"> the new value of the property (as a double) </param>
        ''' <seealso cref= #firePropertyChange(java.lang.String, java.lang.Object,
        '''          java.lang.Object)
        ''' @since 1.5 </seealso>
        Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Double, ByVal newValue As Double)
            If changeSupport Is Nothing OrElse oldValue = newValue Then Return
            firePropertyChange(propertyName, Convert.ToDouble(oldValue), Convert.ToDouble(newValue))
        End Sub


        ' Serialization support.

        ''' <summary>
        ''' Component Serialized Data Version.
        ''' 
        ''' @serial
        ''' </summary>
        Private componentSerializedDataVersion As Integer = 4

        ''' <summary>
        ''' This hack is for Swing serialization. It will invoke
        ''' the Swing package private method <code>compWriteObjectNotify</code>.
        ''' </summary>
        Private Sub doSwingSerialization()
            Dim swingPackage As Package = Package.getPackage("javax.swing")
            ' For Swing serialization to correctly work Swing needs to
            ' be notified before Component does it's serialization.  This
            ' hack accomodates this.
            '
            ' Swing classes MUST be loaded by the bootstrap class loader,
            ' otherwise we don't consider them.
            Dim klass As [Class] = outerInstance.GetType()
            Do While klass IsNot Nothing
                If klass.Assembly Is swingPackage AndAlso klass.classLoader Is Nothing Then
                    Dim swingClass As [Class] = klass
                    ' Find the first override of the compWriteObjectNotify method
                    Dim methods As Method() = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
                    For counter As Integer = methods.Length - 1 To 0 Step -1
                        Dim method As Method = methods(counter)
                        If method.name.Equals("compWriteObjectNotify") Then
                            ' We found it, use doPrivileged to make it accessible
                            ' to use.
                            java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
                            ' Invoke the method
                            Try
                                method.invoke(Me, CType(Nothing, Object()))
                            Catch iae As IllegalAccessException
                            Catch ite As InvocationTargetException
                            End Try
                            ' We're done, bail.
                            Return
                        End If
                    Next counter
                End If
                klass = klass.BaseType
            Loop
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Method()
                Return swingClass.declaredMethods
            End Function
        End Class

        Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                method.accessible = True
                Return Nothing
            End Function
        End Class

        ''' <summary>
        ''' Writes default serializable fields to stream.  Writes
        ''' a variety of serializable listeners as optional data.
        ''' The non-serializable listeners are detected and
        ''' no attempt is made to serialize them.
        ''' </summary>
        ''' <param name="s"> the <code>ObjectOutputStream</code> to write
        ''' @serialData <code>null</code> terminated sequence of
        '''   0 or more pairs; the pair consists of a <code>String</code>
        '''   and an <code>Object</code>; the <code>String</code> indicates
        '''   the type of object and is one of the following (as of 1.4):
        '''   <code>componentListenerK</code> indicating an
        '''     <code>ComponentListener</code> object;
        '''   <code>focusListenerK</code> indicating an
        '''     <code>FocusListener</code> object;
        '''   <code>keyListenerK</code> indicating an
        '''     <code>KeyListener</code> object;
        '''   <code>mouseListenerK</code> indicating an
        '''     <code>MouseListener</code> object;
        '''   <code>mouseMotionListenerK</code> indicating an
        '''     <code>MouseMotionListener</code> object;
        '''   <code>inputMethodListenerK</code> indicating an
        '''     <code>InputMethodListener</code> object;
        '''   <code>hierarchyListenerK</code> indicating an
        '''     <code>HierarchyListener</code> object;
        '''   <code>hierarchyBoundsListenerK</code> indicating an
        '''     <code>HierarchyBoundsListener</code> object;
        '''   <code>mouseWheelListenerK</code> indicating an
        '''     <code>MouseWheelListener</code> object
        ''' @serialData an optional <code>ComponentOrientation</code>
        '''    (after <code>inputMethodListener</code>, as of 1.2)
        ''' </param>
        ''' <seealso cref= AWTEventMulticaster#save(java.io.ObjectOutputStream, java.lang.String, java.util.EventListener) </seealso>
        ''' <seealso cref= #componentListenerK </seealso>
        ''' <seealso cref= #focusListenerK </seealso>
        ''' <seealso cref= #keyListenerK </seealso>
        ''' <seealso cref= #mouseListenerK </seealso>
        ''' <seealso cref= #mouseMotionListenerK </seealso>
        ''' <seealso cref= #inputMethodListenerK </seealso>
        ''' <seealso cref= #hierarchyListenerK </seealso>
        ''' <seealso cref= #hierarchyBoundsListenerK </seealso>
        ''' <seealso cref= #mouseWheelListenerK </seealso>
        ''' <seealso cref= #readObject(ObjectInputStream) </seealso>
        Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
            doSwingSerialization()

            s.defaultWriteObject()

            AWTEventMulticaster.save(s, componentListenerK, componentListener)
            AWTEventMulticaster.save(s, focusListenerK, focusListener)
            AWTEventMulticaster.save(s, keyListenerK, keyListener)
            AWTEventMulticaster.save(s, mouseListenerK, mouseListener)
            AWTEventMulticaster.save(s, mouseMotionListenerK, mouseMotionListener)
            AWTEventMulticaster.save(s, inputMethodListenerK, inputMethodListener)

            s.writeObject(Nothing)
            s.writeObject(componentOrientation)

            AWTEventMulticaster.save(s, hierarchyListenerK, hierarchyListener)
            AWTEventMulticaster.save(s, hierarchyBoundsListenerK, hierarchyBoundsListener)
            s.writeObject(Nothing)

            AWTEventMulticaster.save(s, mouseWheelListenerK, mouseWheelListener)
            s.writeObject(Nothing)

        End Sub

        ''' <summary>
        ''' Reads the <code>ObjectInputStream</code> and if it isn't
        ''' <code>null</code> adds a listener to receive a variety
        ''' of events fired by the component.
        ''' Unrecognized keys or values will be ignored.
        ''' </summary>
        ''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
        ''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            objectLock = New Object

            acc = java.security.AccessController.context

            s.defaultReadObject()

            appContext = sun.awt.AppContext.appContext
            coalescingEnabled = checkCoalescing()
            If componentSerializedDataVersion < 4 Then
                ' These fields are non-transient and rely on default
                ' serialization. However, the default values are insufficient,
                ' so we need to set them explicitly for object data streams prior
                ' to 1.4.
                focusable = True
                isFocusTraversableOverridden_Renamed = FOCUS_TRAVERSABLE_UNKNOWN
                initializeFocusTraversalKeys()
                focusTraversalKeysEnabled = True
            End If

            Dim keyOrNull As Object
            keyOrNull = s.readObject()
            Do While Nothing IsNot keyOrNull
                Dim key As String = CStr(keyOrNull).Intern()

                If componentListenerK = key Then
                    addComponentListener(CType(s.readObject(), ComponentListener))

                ElseIf focusListenerK = key Then
                    addFocusListener(CType(s.readObject(), FocusListener))

                ElseIf keyListenerK = key Then
                    addKeyListener(CType(s.readObject(), KeyListener))

                ElseIf mouseListenerK = key Then
                    addMouseListener(CType(s.readObject(), MouseListener))

                ElseIf mouseMotionListenerK = key Then
                    addMouseMotionListener(CType(s.readObject(), MouseMotionListener))

                ElseIf inputMethodListenerK = key Then
                    addInputMethodListener(CType(s.readObject(), InputMethodListener))

                Else ' skip value for unrecognized key
                    s.readObject()
                End If

                keyOrNull = s.readObject()
            Loop

            ' Read the component's orientation if it's present
            Dim orient As Object = Nothing

            Try
                orient = s.readObject()
            Catch e As java.io.OptionalDataException
                ' JDK 1.1 instances will not have this optional data.
                ' e.eof will be true to indicate that there is no more
                ' data available for this object.
                ' If e.eof is not true, throw the exception as it
                ' might have been caused by reasons unrelated to
                ' componentOrientation.

                If Not e.eof Then Throw (e)
            End Try

            If orient IsNot Nothing Then
                componentOrientation = CType(orient, ComponentOrientation)
            Else
                componentOrientation = ComponentOrientation.UNKNOWN
            End If

            Try
                keyOrNull = s.readObject()
                Do While Nothing IsNot keyOrNull
                    Dim key As String = CStr(keyOrNull).Intern()

                    If hierarchyListenerK = key Then
                        addHierarchyListener(CType(s.readObject(), HierarchyListener))
                    ElseIf hierarchyBoundsListenerK = key Then
                        addHierarchyBoundsListener(CType(s.readObject(), HierarchyBoundsListener))
                    Else
                        ' skip value for unrecognized key
                        s.readObject()
                    End If
                    keyOrNull = s.readObject()
                Loop
            Catch e As java.io.OptionalDataException
                ' JDK 1.1/1.2 instances will not have this optional data.
                ' e.eof will be true to indicate that there is no more
                ' data available for this object.
                ' If e.eof is not true, throw the exception as it
                ' might have been caused by reasons unrelated to
                ' hierarchy and hierarchyBounds listeners.

                If Not e.eof Then Throw (e)
            End Try

            Try
                keyOrNull = s.readObject()
                Do While Nothing IsNot keyOrNull
                    Dim key As String = CStr(keyOrNull).Intern()

                    If mouseWheelListenerK = key Then
                        addMouseWheelListener(CType(s.readObject(), MouseWheelListener))
                    Else
                        ' skip value for unrecognized key
                        s.readObject()
                    End If
                    keyOrNull = s.readObject()
                Loop
            Catch e As java.io.OptionalDataException
                ' pre-1.3 instances will not have this optional data.
                ' e.eof will be true to indicate that there is no more
                ' data available for this object.
                ' If e.eof is not true, throw the exception as it
                ' might have been caused by reasons unrelated to
                ' mouse wheel listeners

                If Not e.eof Then Throw (e)
            End Try

            If popups IsNot Nothing Then
                Dim npopups As Integer = popups.Count
                For i As Integer = 0 To npopups - 1
                    Dim popup As PopupMenu = popups(i)
                    popup.parent = Me
                Next i
            End If
        End Sub

        ''' <summary>
        ''' Sets the language-sensitive orientation that is to be used to order
        ''' the elements or text within this component.  Language-sensitive
        ''' <code>LayoutManager</code> and <code>Component</code>
        ''' subclasses will use this property to
        ''' determine how to lay out and draw components.
        ''' <p>
        ''' At construction time, a component's orientation is set to
        ''' <code>ComponentOrientation.UNKNOWN</code>,
        ''' indicating that it has not been specified
        ''' explicitly.  The UNKNOWN orientation behaves the same as
        ''' <code>ComponentOrientation.LEFT_TO_RIGHT</code>.
        ''' <p>
        ''' To set the orientation of a single component, use this method.
        ''' To set the orientation of an entire component
        ''' hierarchy, use
        ''' <seealso cref="#applyComponentOrientation applyComponentOrientation"/>.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' 
        ''' </summary>
        ''' <seealso cref= ComponentOrientation </seealso>
        ''' <seealso cref= #invalidate
        ''' 
        ''' @author Laura Werner, IBM
        ''' @beaninfo
        '''       bound: true </seealso>
        Public Overridable Property componentOrientation As ComponentOrientation
            Set(ByVal o As ComponentOrientation)
                Dim oldValue As ComponentOrientation = componentOrientation
                componentOrientation = o

                ' This is a bound property, so report the change to
                ' any registered listeners.  (Cheap if there are none.)
                firePropertyChange("componentOrientation", oldValue, o)

                ' This could change the preferred size of the Component.
                invalidateIfValid()
            End Set
            Get
                Return componentOrientation
            End Get
        End Property


        ''' <summary>
        ''' Sets the <code>ComponentOrientation</code> property of this component
        ''' and all components contained within it.
        ''' <p>
        ''' This method changes layout-related information, and therefore,
        ''' invalidates the component hierarchy.
        ''' 
        ''' </summary>
        ''' <param name="orientation"> the new component orientation of this component and
        '''        the components contained within it. </param>
        ''' <exception cref="NullPointerException"> if <code>orientation</code> is null. </exception>
        ''' <seealso cref= #setComponentOrientation </seealso>
        ''' <seealso cref= #getComponentOrientation </seealso>
        ''' <seealso cref= #invalidate
        ''' @since 1.4 </seealso>
        Public Overridable Sub applyComponentOrientation(ByVal orientation As ComponentOrientation)
            If orientation Is Nothing Then Throw New NullPointerException
            componentOrientation = orientation
        End Sub

        Friend Function canBeFocusOwner() As Boolean
            ' It is enabled, visible, focusable.
            If enabled AndAlso displayable AndAlso visible AndAlso focusable Then Return True
            Return False
        End Function

        ''' <summary>
        ''' Checks that this component meets the prerequesites to be focus owner:
        ''' - it is enabled, visible, focusable
        ''' - it's parents are all enabled and showing
        ''' - top-level window is focusable
        ''' - if focus cycle root has DefaultFocusTraversalPolicy then it also checks that this policy accepts
        ''' this component as focus owner
        ''' @since 1.5
        ''' </summary>
        Friend Function canBeFocusOwnerRecursively() As Boolean
            ' - it is enabled, visible, focusable
            If Not canBeFocusOwner() Then Return False

            ' - it's parents are all enabled and showing
            SyncLock treeLock
                If parent IsNot Nothing Then Return parent.canContainFocusOwner(Me)
            End SyncLock
            Return True
        End Function

        ''' <summary>
        ''' Fix the location of the HW component in a LW container hierarchy.
        ''' </summary>
        Friend Sub relocateComponent()
            SyncLock treeLock
                If peer Is Nothing Then Return
                Dim nativeX As Integer = x
                Dim nativeY As Integer = y
                Dim cont As Component = container
                Do While cont IsNot Nothing AndAlso cont.lightweight
                    nativeX += cont.x
                    nativeY += cont.y
                    cont = cont.container
                Loop
                peer.boundsnds(nativeX, nativeY, width, height, java.awt.peer.ComponentPeer.SET_LOCATION)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns the <code>Window</code> ancestor of the component. </summary>
        ''' <returns> Window ancestor of the component or component by itself if it is Window;
        '''         null, if component is not a part of window hierarchy </returns>
        Friend Overridable Property containingWindow As Window
            Get
                Return sun.awt.SunToolkit.getContainingWindow(Me)
            End Get
        End Property

        ''' <summary>
        ''' Initialize JNI field and method IDs
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub initIDs()
        End Sub

        '    
        '     * --- Accessibility Support ---
        '     *
        '     *  Component will contain all of the methods in interface Accessible,
        '     *  though it won't actually implement the interface - that will be up
        '     *  to the individual objects which extend Component.
        '     

        ''' <summary>
        ''' The {@code AccessibleContext} associated with this {@code Component}.
        ''' </summary>
        Protected Friend accessibleContext As AccessibleContext = Nothing

        ''' <summary>
        ''' Gets the <code>AccessibleContext</code> associated
        ''' with this <code>Component</code>.
        ''' The method implemented by this base
        ''' class returns null.  Classes that extend <code>Component</code>
        ''' should implement this method to return the
        ''' <code>AccessibleContext</code> associated with the subclass.
        ''' 
        ''' </summary>
        ''' <returns> the <code>AccessibleContext</code> of this
        '''    <code>Component</code>
        ''' @since 1.3 </returns>
        Public Overridable Property accessibleContext As AccessibleContext
            Get
                Return accessibleContext
            End Get
        End Property

        ''' <summary>
        ''' Inner class of Component used to provide default support for
        ''' accessibility.  This class is not meant to be used directly by
        ''' application developers, but is instead meant only to be
        ''' subclassed by component developers.
        ''' <p>
        ''' The class used to obtain the accessible role for this object.
        ''' @since 1.3
        ''' </summary>
        <Serializable>
        Protected Friend MustInherit Class AccessibleAWTComponent
            Inherits AccessibleContext
            Implements AccessibleComponent

            Private ReadOnly outerInstance As Component


            Private Const serialVersionUID As Long = 642321655757800191L

            ''' <summary>
            ''' Though the class is abstract, this should be called by
            ''' all sub-classes.
            ''' </summary>
            Protected Friend Sub New(ByVal outerInstance As Component)
                Me.outerInstance = outerInstance
            End Sub

            ''' <summary>
            ''' Number of PropertyChangeListener objects registered. It's used
            ''' to add/remove ComponentListener and FocusListener to track
            ''' target Component's state.
            ''' </summary>
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            <NonSerialized>
            Private propertyListenersCount As Integer = 0

            Protected Friend accessibleAWTComponentHandler As ComponentListener = Nothing
            Protected Friend accessibleAWTFocusHandler As FocusListener = Nothing

            ''' <summary>
            ''' Fire PropertyChange listener, if one is registered,
            ''' when shown/hidden..
            ''' @since 1.3
            ''' </summary>
            Protected Friend Class AccessibleAWTComponentHandler
                Implements ComponentListener

                Private ReadOnly outerInstance As Component.AccessibleAWTComponent

                Public Sub New(ByVal outerInstance As Component.AccessibleAWTComponent)
                    Me.outerInstance = outerInstance
                End Sub

                Public Overridable Sub componentHidden(ByVal e As ComponentEvent) Implements ComponentListener.componentHidden
                    If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.VISIBLE, Nothing)
                End Sub

                Public Overridable Sub componentShown(ByVal e As ComponentEvent) Implements ComponentListener.componentShown
                    If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.VISIBLE)
                End Sub

                Public Overridable Sub componentMoved(ByVal e As ComponentEvent) Implements ComponentListener.componentMoved
                End Sub

                Public Overridable Sub componentResized(ByVal e As ComponentEvent) Implements ComponentListener.componentResized
                End Sub
            End Class ' inner class AccessibleAWTComponentHandler


            ''' <summary>
            ''' Fire PropertyChange listener, if one is registered,
            ''' when focus events happen
            ''' @since 1.3
            ''' </summary>
            Protected Friend Class AccessibleAWTFocusHandler
                Implements FocusListener

                Private ReadOnly outerInstance As Component.AccessibleAWTComponent

                Public Sub New(ByVal outerInstance As Component.AccessibleAWTComponent)
                    Me.outerInstance = outerInstance
                End Sub

                Public Overridable Sub focusGained(ByVal [event] As FocusEvent) Implements FocusListener.focusGained
                    If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.FOCUSED)
                End Sub
                Public Overridable Sub focusLost(ByVal [event] As FocusEvent) Implements FocusListener.focusLost
                    If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.FOCUSED, Nothing)
                End Sub
            End Class ' inner class AccessibleAWTFocusHandler


            ''' <summary>
            ''' Adds a <code>PropertyChangeListener</code> to the listener list.
            ''' </summary>
            ''' <param name="listener">  the property change listener to be added </param>
            Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
                If accessibleAWTComponentHandler Is Nothing Then accessibleAWTComponentHandler = New AccessibleAWTComponentHandler(Me)
                If accessibleAWTFocusHandler Is Nothing Then accessibleAWTFocusHandler = New AccessibleAWTFocusHandler(Me)
                Dim tempVar As Boolean = propertyListenersCount = 0
                propertyListenersCount += 1
                If tempVar Then
                    outerInstance.addComponentListener(accessibleAWTComponentHandler)
                    outerInstance.addFocusListener(accessibleAWTFocusHandler)
                End If
                MyBase.addPropertyChangeListener(listener)
            End Sub

            ''' <summary>
            ''' Remove a PropertyChangeListener from the listener list.
            ''' This removes a PropertyChangeListener that was registered
            ''' for all properties.
            ''' </summary>
            ''' <param name="listener">  The PropertyChangeListener to be removed </param>
            Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
                propertyListenersCount -= 1
                If propertyListenersCount = 0 Then
                    outerInstance.removeComponentListener(accessibleAWTComponentHandler)
                    outerInstance.removeFocusListener(accessibleAWTFocusHandler)
                End If
                MyBase.removePropertyChangeListener(listener)
            End Sub

            ' AccessibleContext methods
            '
            ''' <summary>
            ''' Gets the accessible name of this object.  This should almost never
            ''' return <code>java.awt.Component.getName()</code>,
            ''' as that generally isn't a localized name,
            ''' and doesn't have meaning for the user.  If the
            ''' object is fundamentally a text object (e.g. a menu item), the
            ''' accessible name should be the text of the object (e.g. "save").
            ''' If the object has a tooltip, the tooltip text may also be an
            ''' appropriate String to return.
            ''' </summary>
            ''' <returns> the localized name of the object -- can be
            '''         <code>null</code> if this
            '''         object does not have a name </returns>
            ''' <seealso cref= javax.accessibility.AccessibleContext#setAccessibleName </seealso>
            Public Overridable Property accessibleName As String
                Get
                    Return accessibleName
                End Get
            End Property

            ''' <summary>
            ''' Gets the accessible description of this object.  This should be
            ''' a concise, localized description of what this object is - what
            ''' is its meaning to the user.  If the object has a tooltip, the
            ''' tooltip text may be an appropriate string to return, assuming
            ''' it contains a concise description of the object (instead of just
            ''' the name of the object - e.g. a "Save" icon on a toolbar that
            ''' had "save" as the tooltip text shouldn't return the tooltip
            ''' text as the description, but something like "Saves the current
            ''' text document" instead).
            ''' </summary>
            ''' <returns> the localized description of the object -- can be
            '''        <code>null</code> if this object does not have a description </returns>
            ''' <seealso cref= javax.accessibility.AccessibleContext#setAccessibleDescription </seealso>
            Public Overridable Property accessibleDescription As String
                Get
                    Return accessibleDescription
                End Get
            End Property

            ''' <summary>
            ''' Gets the role of this object.
            ''' </summary>
            ''' <returns> an instance of <code>AccessibleRole</code>
            '''      describing the role of the object </returns>
            ''' <seealso cref= javax.accessibility.AccessibleRole </seealso>
            Public Overridable Property accessibleRole As AccessibleRole
                Get
                    Return accessibleRole.AWT_COMPONENT
                End Get
            End Property

            ''' <summary>
            ''' Gets the state of this object.
            ''' </summary>
            ''' <returns> an instance of <code>AccessibleStateSet</code>
            '''       containing the current state set of the object </returns>
            ''' <seealso cref= javax.accessibility.AccessibleState </seealso>
            Public Overridable Property accessibleStateSet As AccessibleStateSet
                Get
                    Return outerInstance.accessibleStateSet
                End Get
            End Property

            ''' <summary>
            ''' Gets the <code>Accessible</code> parent of this object.
            ''' If the parent of this object implements <code>Accessible</code>,
            ''' this method should simply return <code>getParent</code>.
            ''' </summary>
            ''' <returns> the <code>Accessible</code> parent of this
            '''      object -- can be <code>null</code> if this
            '''      object does not have an <code>Accessible</code> parent </returns>
            Public Overridable Property accessibleParent As Accessible
                Get
                    If accessibleParent IsNot Nothing Then
                        Return accessibleParent
                    Else
                        Dim parent As Container = outerInstance.parent
                        If TypeOf parent Is Accessible Then Return CType(parent, Accessible)
                    End If
                    Return Nothing
                End Get
            End Property

            ''' <summary>
            ''' Gets the index of this object in its accessible parent.
            ''' </summary>
            ''' <returns> the index of this object in its parent; or -1 if this
            '''    object does not have an accessible parent </returns>
            ''' <seealso cref= #getAccessibleParent </seealso>
            Public Overridable Property accessibleIndexInParent As Integer
                Get
                    Return outerInstance.accessibleIndexInParent
                End Get
            End Property

            ''' <summary>
            ''' Returns the number of accessible children in the object.  If all
            ''' of the children of this object implement <code>Accessible</code>,
            ''' then this method should return the number of children of this object.
            ''' </summary>
            ''' <returns> the number of accessible children in the object </returns>
            Public Overridable Property accessibleChildrenCount As Integer
                Get
                    Return 0 ' Components don't have children
                End Get
            End Property

            ''' <summary>
            ''' Returns the nth <code>Accessible</code> child of the object.
            ''' </summary>
            ''' <param name="i"> zero-based index of child </param>
            ''' <returns> the nth <code>Accessible</code> child of the object </returns>
            Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
                Return Nothing ' Components don't have children
            End Function

            ''' <summary>
            ''' Returns the locale of this object.
            ''' </summary>
            ''' <returns> the locale of this object </returns>
            Public Overridable Property locale As java.util.Locale
                Get
                    Return outerInstance.locale
                End Get
            End Property

            ''' <summary>
            ''' Gets the <code>AccessibleComponent</code> associated
            ''' with this object if one exists.
            ''' Otherwise return <code>null</code>.
            ''' </summary>
            ''' <returns> the component </returns>
            Public Overridable Property accessibleComponent As AccessibleComponent
                Get
                    Return Me
                End Get
            End Property


            ' AccessibleComponent methods
            '
            ''' <summary>
            ''' Gets the background color of this object.
            ''' </summary>
            ''' <returns> the background color, if supported, of the object;
            '''      otherwise, <code>null</code> </returns>
            Public Overridable Property background As color
                Get
                    Return outerInstance.background
                End Get
                Set(ByVal c As color)
                    outerInstance.background = c
                End Set
            End Property


            ''' <summary>
            ''' Gets the foreground color of this object.
            ''' </summary>
            ''' <returns> the foreground color, if supported, of the object;
            '''     otherwise, <code>null</code> </returns>
            Public Overridable Property foreground As color
                Get
                    Return outerInstance.foreground
                End Get
                Set(ByVal c As color)
                    outerInstance.foreground = c
                End Set
            End Property


            ''' <summary>
            ''' Gets the <code>Cursor</code> of this object.
            ''' </summary>
            ''' <returns> the <code>Cursor</code>, if supported,
            '''     of the object; otherwise, <code>null</code> </returns>
            Public Overridable Property cursor As Cursor
                Get
                    Return outerInstance.cursor
                End Get
                Set(ByVal cursor_Renamed As Cursor)
                    outerInstance.cursor = cursor_Renamed
                End Set
            End Property


            ''' <summary>
            ''' Gets the <code>Font</code> of this object.
            ''' </summary>
            ''' <returns> the <code>Font</code>, if supported,
            '''    for the object; otherwise, <code>null</code> </returns>
            Public Overridable Property font As font
                Get
                    Return outerInstance.font
                End Get
                Set(ByVal f As font)
                    outerInstance.font = f
                End Set
            End Property


            ''' <summary>
            ''' Gets the <code>FontMetrics</code> of this object.
            ''' </summary>
            ''' <param name="f"> the <code>Font</code> </param>
            ''' <returns> the <code>FontMetrics</code>, if supported,
            '''     the object; otherwise, <code>null</code> </returns>
            ''' <seealso cref= #getFont </seealso>
            Public Overridable Function getFontMetrics(ByVal f As font) As FontMetrics
                If f Is Nothing Then
                    Return Nothing
                Else
                    Return outerInstance.getFontMetrics(f)
                End If
            End Function

            ''' <summary>
            ''' Determines if the object is enabled.
            ''' </summary>
            ''' <returns> true if object is enabled; otherwise, false </returns>
            Public Overridable Property enabled As Boolean
                Get
                    Return outerInstance.enabled
                End Get
                Set(ByVal b As Boolean)
                    Dim old As Boolean = outerInstance.enabled
                    outerInstance.enabled = b
                    If b <> old Then
                        If outerInstance.accessibleContext IsNot Nothing Then
                            If b Then
                                outerInstance.accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.ENABLED)
                            Else
                                outerInstance.accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.ENABLED, Nothing)
                            End If
                        End If
                    End If
                End Set
            End Property


            ''' <summary>
            ''' Determines if the object is visible.  Note: this means that the
            ''' object intends to be visible; however, it may not in fact be
            ''' showing on the screen because one of the objects that this object
            ''' is contained by is not visible.  To determine if an object is
            ''' showing on the screen, use <code>isShowing</code>.
            ''' </summary>
            ''' <returns> true if object is visible; otherwise, false </returns>
            Public Overridable Property visible As Boolean
                Get
                    Return outerInstance.visible
                End Get
                Set(ByVal b As Boolean)
                    Dim old As Boolean = outerInstance.visible
                    outerInstance.visible = b
                    If b <> old Then
                        If outerInstance.accessibleContext IsNot Nothing Then
                            If b Then
                                outerInstance.accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.VISIBLE)
                            Else
                                outerInstance.accessibleContext.firePropertyChange(accessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.VISIBLE, Nothing)
                            End If
                        End If
                    End If
                End Set
            End Property


            ''' <summary>
            ''' Determines if the object is showing.  This is determined by checking
            ''' the visibility of the object and ancestors of the object.  Note:
            ''' this will return true even if the object is obscured by another
            ''' (for example, it happens to be underneath a menu that was pulled
            ''' down).
            ''' </summary>
            ''' <returns> true if object is showing; otherwise, false </returns>
            Public Overridable Property showing As Boolean
                Get
                    Return outerInstance.showing
                End Get
            End Property

            ''' <summary>
            ''' Checks whether the specified point is within this object's bounds,
            ''' where the point's x and y coordinates are defined to be relative to
            ''' the coordinate system of the object.
            ''' </summary>
            ''' <param name="p"> the <code>Point</code> relative to the
            '''     coordinate system of the object </param>
            ''' <returns> true if object contains <code>Point</code>; otherwise false </returns>
            Public Overridable Function contains(ByVal p As Point) As Boolean
                Return outerInstance.contains(p)
            End Function

            ''' <summary>
            ''' Returns the location of the object on the screen.
            ''' </summary>
            ''' <returns> location of object on screen -- can be
            '''    <code>null</code> if this object is not on the screen </returns>
            Public Overridable Property locationOnScreen As Point
                Get
                    SyncLock outerInstance.treeLock
                        If outerInstance.showing Then
                            Return outerInstance.locationOnScreen
                        Else
                            Return Nothing
                        End If
                    End SyncLock
                End Get
            End Property

            ''' <summary>
            ''' Gets the location of the object relative to the parent in the form
            ''' of a point specifying the object's top-left corner in the screen's
            ''' coordinate space.
            ''' </summary>
            ''' <returns> an instance of Point representing the top-left corner of
            ''' the object's bounds in the coordinate space of the screen;
            ''' <code>null</code> if this object or its parent are not on the screen </returns>
            Public Overridable Property location As Point
                Get
                    Return outerInstance.location
                End Get
                Set(ByVal p As Point)
                    outerInstance.location = p
                End Set
            End Property


            ''' <summary>
            ''' Gets the bounds of this object in the form of a Rectangle object.
            ''' The bounds specify this object's width, height, and location
            ''' relative to its parent.
            ''' </summary>
            ''' <returns> a rectangle indicating this component's bounds;
            '''   <code>null</code> if this object is not on the screen </returns>
            Public Overridable Property bounds As Rectangle
                Get
                    Return outerInstance.bounds
                End Get
                Set(ByVal r As Rectangle)
                    outerInstance.bounds = r
                End Set
            End Property


            ''' <summary>
            ''' Returns the size of this object in the form of a
            ''' <code>Dimension</code> object. The height field of the
            ''' <code>Dimension</code> object contains this objects's
            ''' height, and the width field of the <code>Dimension</code>
            ''' object contains this object's width.
            ''' </summary>
            ''' <returns> a <code>Dimension</code> object that indicates
            '''     the size of this component; <code>null</code> if
            '''     this object is not on the screen </returns>
            Public Overridable Property size As Dimension
                Get
                    Return outerInstance.size
                End Get
                Set(ByVal d As Dimension)
                    outerInstance.size = d
                End Set
            End Property


            ''' <summary>
            ''' Returns the <code>Accessible</code> child,
            ''' if one exists, contained at the local
            ''' coordinate <code>Point</code>.  Otherwise returns
            ''' <code>null</code>.
            ''' </summary>
            ''' <param name="p"> the point defining the top-left corner of
            '''      the <code>Accessible</code>, given in the
            '''      coordinate space of the object's parent </param>
            ''' <returns> the <code>Accessible</code>, if it exists,
            '''      at the specified location; else <code>null</code> </returns>
            Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
                Return Nothing ' Components don't have children
            End Function

            ''' <summary>
            ''' Returns whether this object can accept focus or not.
            ''' </summary>
            ''' <returns> true if object can accept focus; otherwise false </returns>
            Public Overridable Property focusTraversable As Boolean
                Get
                    Return outerInstance.focusTraversable
                End Get
            End Property

            ''' <summary>
            ''' Requests focus for this object.
            ''' </summary>
            Public Overridable Sub requestFocus()
                outerInstance.requestFocus()
            End Sub

            ''' <summary>
            ''' Adds the specified focus listener to receive focus events from this
            ''' component.
            ''' </summary>
            ''' <param name="l"> the focus listener </param>
            Public Overridable Sub addFocusListener(ByVal l As FocusListener)
                outerInstance.addFocusListener(l)
            End Sub

            ''' <summary>
            ''' Removes the specified focus listener so it no longer receives focus
            ''' events from this component.
            ''' </summary>
            ''' <param name="l"> the focus listener </param>
            Public Overridable Sub removeFocusListener(ByVal l As FocusListener)
                outerInstance.removeFocusListener(l)
            End Sub

        End Class ' inner class AccessibleAWTComponent


        ''' <summary>
        ''' Gets the index of this object in its accessible parent.
        ''' If this object does not have an accessible parent, returns
        ''' -1.
        ''' </summary>
        ''' <returns> the index of this object in its accessible parent </returns>
        Friend Overridable Property accessibleIndexInParent As Integer
            Get
                SyncLock treeLock
                    Dim index As Integer = -1
                    Dim parent_Renamed As Container = Me.parent
                    If parent_Renamed IsNot Nothing AndAlso TypeOf parent_Renamed Is Accessible Then
                        Dim ca As Component() = parent_Renamed.components
                        For i As Integer = 0 To ca.Length - 1
                            If TypeOf ca(i) Is Accessible Then index += 1
                            If Me.Equals(ca(i)) Then Return index
                        Next i
                    End If
                    Return -1
                End SyncLock
            End Get
        End Property

        ''' <summary>
        ''' Gets the current state set of this object.
        ''' </summary>
        ''' <returns> an instance of <code>AccessibleStateSet</code>
        '''    containing the current state set of the object </returns>
        ''' <seealso cref= AccessibleState </seealso>
        Friend Overridable Property accessibleStateSet As AccessibleStateSet
            Get
                SyncLock treeLock
                    Dim states As New AccessibleStateSet
                    If Me.enabled Then states.add(AccessibleState.ENABLED)
                    If Me.focusTraversable Then states.add(AccessibleState.FOCUSABLE)
                    If Me.visible Then states.add(AccessibleState.VISIBLE)
                    If Me.showing Then states.add(AccessibleState.SHOWING)
                    If Me.focusOwner Then states.add(AccessibleState.FOCUSED)
                    If TypeOf Me Is Accessible Then
                        Dim ac As AccessibleContext = CType(Me, Accessible).accessibleContext
                        If ac IsNot Nothing Then
                            Dim ap As Accessible = ac.accessibleParent
                            If ap IsNot Nothing Then
                                Dim pac As AccessibleContext = ap.accessibleContext
                                If pac IsNot Nothing Then
                                    Dim [as] As AccessibleSelection = pac.accessibleSelection
                                    If [as] IsNot Nothing Then
                                        states.add(AccessibleState.SELECTABLE)
                                        Dim i As Integer = ac.accessibleIndexInParent
                                        If i >= 0 Then
                                            If [as].isAccessibleChildSelected(i) Then states.add(AccessibleState.SELECTED)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If Component.isInstanceOf(Me, "javax.swing.JComponent") Then
                        If CType(Me, javax.swing.JComponent).opaque Then states.add(AccessibleState.OPAQUE)
                    End If
                    Return states
                End SyncLock
            End Get
        End Property

        ''' <summary>
        ''' Checks that the given object is instance of the given class. </summary>
        ''' <param name="obj"> Object to be checked </param>
        ''' <param name="className"> The name of the class. Must be fully-qualified class name. </param>
        ''' <returns> true, if this object is instanceof given [Class],
        '''         false, otherwise, or if obj or className is null </returns>
        Friend Shared Function isInstanceOf(ByVal obj As Object, ByVal className As String) As Boolean
            If obj Is Nothing Then Return False
            If className Is Nothing Then Return False

            Dim cls As [Class] = obj.GetType()
            Do While cls IsNot Nothing
                If cls.name.Equals(className) Then Return True
                cls = cls.BaseType
            Loop
            Return False
        End Function


        ' ************************** MIXING CODE *******************************

        ''' <summary>
        ''' Check whether we can trust the current bounds of the component.
        ''' The return value of false indicates that the container of the
        ''' component is invalid, and therefore needs to be layed out, which would
        ''' probably mean changing the bounds of its children.
        ''' Null-layout of the container or absence of the container mean
        ''' the bounds of the component are final and can be trusted.
        ''' </summary>
        Friend Function areBoundsValid() As Boolean
            Dim cont As Container = container
            Return cont Is Nothing OrElse cont.valid OrElse cont.layout Is Nothing
        End Function

        ''' <summary>
        ''' Applies the shape to the component </summary>
        ''' <param name="shape"> Shape to be applied to the component </param>
        Friend Overridable Sub applyCompoundShape(ByVal shape As sun.java2d.pipe.Region)
            checkTreeLock()

            If Not areBoundsValid() Then
                If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; areBoundsValid = " & areBoundsValid())
                Return
            End If

            If Not lightweight Then
                Dim peer_Renamed As java.awt.peer.ComponentPeer = peer
                If peer_Renamed IsNot Nothing Then
                    ' The Region class has some optimizations. That's why
                    ' we should manually check whether it's empty and
                    ' substitute the object ourselves. Otherwise we end up
                    ' with some incorrect Region object with loX being
                    ' greater than the hiX for instance.
                    If shape.empty Then shape = sun.java2d.pipe.Region.EMPTY_REGION


                    ' Note: the shape is not really copied/cloned. We create
                    ' the Region object ourselves, so there's no any possibility
                    ' to modify the object outside of the mixing code.
                    ' Nullifying compoundShape means that the component has normal shape
                    ' (or has no shape at all).
                    If shape.Equals(normalShape) Then
                        If Me.compoundShape Is Nothing Then Return
                        Me.compoundShape = Nothing
                        peer_Renamed.applyShape(Nothing)
                    Else
                        If shape.Equals(appliedShape) Then Return
                        Me.compoundShape = shape
                        Dim compAbsolute As Point = locationOnWindow
                        If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then mixingLog.fine("this = " & Me & "; compAbsolute=" & compAbsolute & "; shape=" & shape)
                        peer_Renamed.applyShape(shape.getTranslatedRegion(-compAbsolute.x, -compAbsolute.y))
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Returns the shape previously set with applyCompoundShape().
        ''' If the component is LW or no shape was applied yet,
        ''' the method returns the normal shape.
        ''' </summary>
        Private Property appliedShape As sun.java2d.pipe.Region
            Get
                checkTreeLock()
                'XXX: if we allow LW components to have a shape, this must be changed
                Return If(Me.compoundShape Is Nothing OrElse lightweight, normalShape, Me.compoundShape)
            End Get
        End Property

        Friend Overridable Property locationOnWindow As Point
            Get
                checkTreeLock()
                Dim curLocation As Point = location

                Dim parent_Renamed As Container = container
                Do While parent_Renamed IsNot Nothing AndAlso Not (TypeOf parent_Renamed Is Window)
                    curLocation.x += parent_Renamed.x
                    curLocation.y += parent_Renamed.y
                    parent_Renamed = parent_Renamed.container
                Loop

                Return curLocation
            End Get
        End Property

        ''' <summary>
        ''' Returns the full shape of the component located in window coordinates
        ''' </summary>
        Friend Property normalShape As sun.java2d.pipe.Region
            Get
                checkTreeLock()
                'XXX: we may take into account a user-specified shape for this component
                Dim compAbsolute As Point = locationOnWindow
                Return sun.java2d.pipe.Region.getInstanceXYWH(compAbsolute.x, compAbsolute.y, width, height)
            End Get
        End Property

        ''' <summary>
        ''' Returns the "opaque shape" of the component.
        ''' 
        ''' The opaque shape of a lightweight components is the actual shape that
        ''' needs to be cut off of the heavyweight components in order to mix this
        ''' lightweight component correctly with them.
        ''' 
        ''' The method is overriden in the java.awt.Container to handle non-opaque
        ''' containers containing opaque children.
        ''' 
        ''' See 6637655 for details.
        ''' </summary>
        Friend Overridable Property opaqueShape As sun.java2d.pipe.Region
            Get
                checkTreeLock()
                If mixingCutoutRegion IsNot Nothing Then
                    Return mixingCutoutRegion
                Else
                    Return normalShape
                End If
            End Get
        End Property

        Friend Property siblingIndexAbove As Integer
            Get
                checkTreeLock()
                Dim parent_Renamed As Container = container
                If parent_Renamed Is Nothing Then Return -1

                Dim nextAbove As Integer = parent_Renamed.getComponentZOrder(Me) - 1

                Return If(nextAbove < 0, -1, nextAbove)
            End Get
        End Property

        Friend Property hWPeerAboveMe As java.awt.peer.ComponentPeer
            Get
                checkTreeLock()

                Dim cont As Container = container
                Dim indexAbove As Integer = siblingIndexAbove

                Do While cont IsNot Nothing
                    For i As Integer = indexAbove To 0 Step -1
                        Dim comp As Component = cont.getComponent(i)
                        If comp IsNot Nothing AndAlso comp.displayable AndAlso (Not comp.lightweight) Then Return comp.peer
                    Next i
                    ' traversing the hierarchy up to the closest HW container;
                    ' further traversing may return a component that is not actually
                    ' a native sibling of this component and this kind of z-order
                    ' request may not be allowed by the underlying system (6852051).
                    If Not cont.lightweight Then Exit Do

                    indexAbove = cont.siblingIndexAbove
                    cont = cont.container
                Loop

                Return Nothing
            End Get
        End Property

        Friend Property siblingIndexBelow As Integer
            Get
                checkTreeLock()
                Dim parent_Renamed As Container = container
                If parent_Renamed Is Nothing Then Return -1

                Dim nextBelow As Integer = parent_Renamed.getComponentZOrder(Me) + 1

                Return If(nextBelow >= parent_Renamed.componentCount, -1, nextBelow)
            End Get
        End Property

        Friend Property nonOpaqueForMixing As Boolean
            Get
                Return mixingCutoutRegion IsNot Nothing AndAlso mixingCutoutRegion.empty
            End Get
        End Property

        Private Function calculateCurrentShape() As sun.java2d.pipe.Region
            checkTreeLock()
            Dim s As sun.java2d.pipe.Region = normalShape

            If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; normalShape=" & s)

            If container IsNot Nothing Then
                Dim comp As Component = Me
                Dim cont As Container = comp.container

                Do While cont IsNot Nothing
                    Dim index As Integer = comp.siblingIndexAbove
                    Do While index <> -1
                        '                     It is assumed that:
                        '                     *
                        '                     *    getComponent(getContainer().getComponentZOrder(comp)) == comp
                        '                     *
                        '                     * The assumption has been made according to the current
                        '                     * implementation of the Container class.
                        '                     
                        Dim c As Component = cont.getComponent(index)
                        If c.lightweight AndAlso c.showing Then s = s.getDifference(c.opaqueShape)
                        index -= 1
                    Loop

                    If cont.lightweight Then
                        s = s.getIntersection(cont.normalShape)
                    Else
                        Exit Do
                    End If

                    comp = cont
                    cont = cont.container
                Loop
            End If

            If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("currentShape=" & s)

            Return s
        End Function

        Friend Overridable Sub applyCurrentShape()
            checkTreeLock()
            If Not areBoundsValid() Then
                If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; areBoundsValid = " & areBoundsValid())
                Return ' Because applyCompoundShape() ignores such components anyway
            End If
            If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me)
            applyCompoundShape(calculateCurrentShape())
        End Sub

        Friend Sub subtractAndApplyShape(ByVal s As sun.java2d.pipe.Region)
            checkTreeLock()

            If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; s=" & s)

            applyCompoundShape(appliedShape.getDifference(s))
        End Sub

        Private Sub applyCurrentShapeBelowMe()
            checkTreeLock()
            Dim parent_Renamed As Container = container
            If parent_Renamed IsNot Nothing AndAlso parent_Renamed.showing Then
                ' First, reapply shapes of my siblings
                parent_Renamed.recursiveApplyCurrentShape(siblingIndexBelow)

                ' Second, if my container is non-opaque, reapply shapes of siblings of my container
                Dim parent2 As Container = parent_Renamed.container
                Do While (Not parent_Renamed.opaque) AndAlso parent2 IsNot Nothing
                    parent2.recursiveApplyCurrentShape(parent_Renamed.siblingIndexBelow)

                    parent_Renamed = parent2
                    parent2 = parent_Renamed.container
                Loop
            End If
        End Sub

        Friend Sub subtractAndApplyShapeBelowMe()
            checkTreeLock()
            Dim parent_Renamed As Container = container
            If parent_Renamed IsNot Nothing AndAlso showing Then
                Dim opaqueShape_Renamed As sun.java2d.pipe.Region = opaqueShape

                ' First, cut my siblings
                parent_Renamed.recursiveSubtractAndApplyShape(opaqueShape_Renamed, siblingIndexBelow)

                ' Second, if my container is non-opaque, cut siblings of my container
                Dim parent2 As Container = parent_Renamed.container
                Do While (Not parent_Renamed.opaque) AndAlso parent2 IsNot Nothing
                    parent2.recursiveSubtractAndApplyShape(opaqueShape_Renamed, parent_Renamed.siblingIndexBelow)

                    parent_Renamed = parent2
                    parent2 = parent_Renamed.container
                Loop
            End If
        End Sub

        Friend Overridable Sub mixOnShowing()
            SyncLock treeLock
                If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me)
                If Not mixingNeeded Then Return
                If lightweight Then
                    subtractAndApplyShapeBelowMe()
                Else
                    applyCurrentShape()
                End If
            End SyncLock
        End Sub

        Friend Overridable Sub mixOnHiding(ByVal isLightweight As Boolean)
            ' We cannot be sure that the peer exists at this point, so we need the argument
            '    to find out whether the hiding component is (well, actually was) a LW or a HW.
            SyncLock treeLock
                If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; isLightweight = " & isLightweight)
                If Not mixingNeeded Then Return
                If isLightweight Then applyCurrentShapeBelowMe()
            End SyncLock
        End Sub

        Friend Overridable Sub mixOnReshaping()
            SyncLock treeLock
                If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me)
                If Not mixingNeeded Then Return
                If lightweight Then
                    applyCurrentShapeBelowMe()
                Else
                    applyCurrentShape()
                End If
            End SyncLock
        End Sub

        Friend Overridable Sub mixOnZOrderChanging(ByVal oldZorder As Integer, ByVal newZorder As Integer)
            SyncLock treeLock
                Dim becameHigher As Boolean = newZorder < oldZorder
                Dim parent_Renamed As Container = container

                If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; oldZorder=" & oldZorder & "; newZorder=" & newZorder & "; parent=" & parent_Renamed)
                If Not mixingNeeded Then Return
                If lightweight Then
                    If becameHigher Then
                        If parent_Renamed IsNot Nothing AndAlso showing Then parent_Renamed.recursiveSubtractAndApplyShape(opaqueShape, siblingIndexBelow, oldZorder)
                    Else
                        If parent_Renamed IsNot Nothing Then parent_Renamed.recursiveApplyCurrentShape(oldZorder, newZorder)
                    End If
                Else
                    If becameHigher Then
                        applyCurrentShape()
                    Else
                        If parent_Renamed IsNot Nothing Then
                            Dim shape As sun.java2d.pipe.Region = appliedShape

                            For index As Integer = oldZorder To newZorder - 1
                                Dim c As Component = parent_Renamed.getComponent(index)
                                If c.lightweight AndAlso c.showing Then shape = shape.getDifference(c.opaqueShape)
                            Next index
                            applyCompoundShape(shape)
                        End If
                    End If
                End If
            End SyncLock
        End Sub

        Friend Overridable Sub mixOnValidating()
            ' This method gets overriden in the Container. Obviously, a plain
            ' non-container components don't need to handle validation.
        End Sub

        Friend Property mixingNeeded As Boolean
            Get
                If sun.awt.SunToolkit.sunAwtDisableMixing Then
                    If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then mixingLog.finest("this = " & Me & "; Mixing disabled via sun.awt.disableMixing")
                    Return False
                End If
                If Not areBoundsValid() Then
                    If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; areBoundsValid = " & areBoundsValid())
                    Return False
                End If
                Dim window_Renamed As Window = containingWindow
                If window_Renamed IsNot Nothing Then
                    If (Not window_Renamed.hasHeavyweightDescendants()) OrElse (Not window_Renamed.hasLightweightDescendants()) OrElse window_Renamed.disposing Then
                        If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("containing window = " & window_Renamed & "; has h/w descendants = " & window_Renamed.hasHeavyweightDescendants() & "; has l/w descendants = " & window_Renamed.hasLightweightDescendants() & "; disposing = " & window_Renamed.disposing)
                        Return False
                    End If
                Else
                    If mixingLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then mixingLog.fine("this = " & Me & "; containing window is null")
                    Return False
                End If
                Return True
            End Get
        End Property

        ' ****************** END OF MIXING CODE ********************************

        ' Note that the method is overriden in the Window [Class],
        ' a window doesn't need to be updated in the Z-order.
        Friend Overridable Sub updateZOrder()
            peer.zOrder = hWPeerAboveMe
        End Sub

    End Class

End Namespace
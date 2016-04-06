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


Namespace java.awt



	''' <summary>
	''' The <code>GraphicsDevice</code> class describes the graphics devices
	''' that might be available in a particular graphics environment.  These
	''' include screen and printer devices. Note that there can be many screens
	''' and many printers in an instance of <seealso cref="GraphicsEnvironment"/>. Each
	''' graphics device has one or more <seealso cref="GraphicsConfiguration"/> objects
	''' associated with it.  These objects specify the different configurations
	''' in which the <code>GraphicsDevice</code> can be used.
	''' <p>
	''' In a multi-screen environment, the <code>GraphicsConfiguration</code>
	''' objects can be used to render components on multiple screens.  The
	''' following code sample demonstrates how to create a <code>JFrame</code>
	''' object for each <code>GraphicsConfiguration</code> on each screen
	''' device in the <code>GraphicsEnvironment</code>:
	''' <pre>{@code
	'''   GraphicsEnvironment ge = GraphicsEnvironment.
	'''   getLocalGraphicsEnvironment();
	'''   GraphicsDevice[] gs = ge.getScreenDevices();
	'''   for (int j = 0; j < gs.length; j++) {
	'''      GraphicsDevice gd = gs[j];
	'''      GraphicsConfiguration[] gc =
	'''      gd.getConfigurations();
	'''      for (int i=0; i < gc.length; i++) {
	'''         JFrame f = new
	'''         JFrame(gs[j].getDefaultConfiguration());
	'''         Canvas c = new Canvas(gc[i]);
	'''         Rectangle gcBounds = gc[i].getBounds();
	'''         int xoffs = gcBounds.x;
	'''         int yoffs = gcBounds.y;
	'''         f.getContentPane().add(c);
	'''         f.setLocation((i*50)+xoffs, (i*60)+yoffs);
	'''         f.show();
	'''      }
	'''   }
	''' }</pre>
	''' <p>
	''' For more information on full-screen exclusive mode API, see the
	''' <a href="https://docs.oracle.com/javase/tutorial/extra/fullscreen/index.html">
	''' Full-Screen Exclusive Mode API Tutorial</a>.
	''' </summary>
	''' <seealso cref= GraphicsEnvironment </seealso>
	''' <seealso cref= GraphicsConfiguration </seealso>
	Public MustInherit Class GraphicsDevice

		Private fullScreenWindow As Window
		Private fullScreenAppContext As sun.awt.AppContext ' tracks which AppContext
												 ' created the FS window
		' this lock is used for making synchronous changes to the AppContext's
		' current full screen window
		Private ReadOnly fsAppContextLock As New Object

		Private windowedModeBounds As Rectangle

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Instances must be obtained from a suitable factory or query method. </summary>
		''' <seealso cref= GraphicsEnvironment#getScreenDevices </seealso>
		''' <seealso cref= GraphicsEnvironment#getDefaultScreenDevice </seealso>
		''' <seealso cref= GraphicsConfiguration#getDevice </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Device is a raster screen.
		''' </summary>
		Public Const TYPE_RASTER_SCREEN As Integer = 0

		''' <summary>
		''' Device is a printer.
		''' </summary>
		Public Const TYPE_PRINTER As Integer = 1

		''' <summary>
		''' Device is an image buffer.  This buffer can reside in device
		''' or system memory but it is not physically viewable by the user.
		''' </summary>
		Public Const TYPE_IMAGE_BUFFER As Integer = 2

		''' <summary>
		''' Kinds of translucency supported by the underlying system.
		''' </summary>
		''' <seealso cref= #isWindowTranslucencySupported
		''' 
		''' @since 1.7 </seealso>
		Public Enum WindowTranslucency
			''' <summary>
			''' Represents support in the underlying system for windows each pixel
			''' of which is guaranteed to be either completely opaque, with
			''' an alpha value of 1.0, or completely transparent, with an alpha
			''' value of 0.0.
			''' </summary>
			PERPIXEL_TRANSPARENT
			''' <summary>
			''' Represents support in the underlying system for windows all of
			''' the pixels of which have the same alpha value between or including
			''' 0.0 and 1.0.
			''' </summary>
			TRANSLUCENT
			''' <summary>
			''' Represents support in the underlying system for windows that
			''' contain or might contain pixels with arbitrary alpha values
			''' between and including 0.0 and 1.0.
			''' </summary>
			PERPIXEL_TRANSLUCENT
		End Enum

		''' <summary>
		''' Returns the type of this <code>GraphicsDevice</code>. </summary>
		''' <returns> the type of this <code>GraphicsDevice</code>, which can
		''' either be TYPE_RASTER_SCREEN, TYPE_PRINTER or TYPE_IMAGE_BUFFER. </returns>
		''' <seealso cref= #TYPE_RASTER_SCREEN </seealso>
		''' <seealso cref= #TYPE_PRINTER </seealso>
		''' <seealso cref= #TYPE_IMAGE_BUFFER </seealso>
		Public MustOverride Function [getType]() As Integer

		''' <summary>
		''' Returns the identification string associated with this
		''' <code>GraphicsDevice</code>.
		''' <p>
		''' A particular program might use more than one
		''' <code>GraphicsDevice</code> in a <code>GraphicsEnvironment</code>.
		''' This method returns a <code>String</code> identifying a
		''' particular <code>GraphicsDevice</code> in the local
		''' <code>GraphicsEnvironment</code>.  Although there is
		''' no public method to set this <code>String</code>, a programmer can
		''' use the <code>String</code> for debugging purposes.  Vendors of
		''' the Java&trade; Runtime Environment can
		''' format the return value of the <code>String</code>.  To determine
		''' how to interpret the value of the <code>String</code>, contact the
		''' vendor of your Java Runtime.  To find out who the vendor is, from
		''' your program, call the
		''' <seealso cref="System#getProperty(String) getProperty"/> method of the
		''' System class with "java.vendor". </summary>
		''' <returns> a <code>String</code> that is the identification
		''' of this <code>GraphicsDevice</code>. </returns>
		Public MustOverride ReadOnly Property iDstring As String

		''' <summary>
		''' Returns all of the <code>GraphicsConfiguration</code>
		''' objects associated with this <code>GraphicsDevice</code>. </summary>
		''' <returns> an array of <code>GraphicsConfiguration</code>
		''' objects that are associated with this
		''' <code>GraphicsDevice</code>. </returns>
		Public MustOverride ReadOnly Property configurations As GraphicsConfiguration()

		''' <summary>
		''' Returns the default <code>GraphicsConfiguration</code>
		''' associated with this <code>GraphicsDevice</code>. </summary>
		''' <returns> the default <code>GraphicsConfiguration</code>
		''' of this <code>GraphicsDevice</code>. </returns>
		Public MustOverride ReadOnly Property defaultConfiguration As GraphicsConfiguration

		''' <summary>
		''' Returns the "best" configuration possible that passes the
		''' criteria defined in the <seealso cref="GraphicsConfigTemplate"/>. </summary>
		''' <param name="gct"> the <code>GraphicsConfigTemplate</code> object
		''' used to obtain a valid <code>GraphicsConfiguration</code> </param>
		''' <returns> a <code>GraphicsConfiguration</code> that passes
		''' the criteria defined in the specified
		''' <code>GraphicsConfigTemplate</code>. </returns>
		''' <seealso cref= GraphicsConfigTemplate </seealso>
		Public Overridable Function getBestConfiguration(  gct As GraphicsConfigTemplate) As GraphicsConfiguration
			Dim configs As GraphicsConfiguration() = configurations
			Return gct.getBestConfiguration(configs)
		End Function

		''' <summary>
		''' Returns <code>true</code> if this <code>GraphicsDevice</code>
		''' supports full-screen exclusive mode.
		''' If a SecurityManager is installed, its
		''' <code>checkPermission</code> method will be called
		''' with <code>AWTPermission("fullScreenExclusive")</code>.
		''' <code>isFullScreenSupported</code> returns true only if
		''' that permission is granted. </summary>
		''' <returns> whether full-screen exclusive mode is available for
		''' this graphics device </returns>
		''' <seealso cref= java.awt.AWTPermission
		''' @since 1.4 </seealso>
		Public Overridable Property fullScreenSupported As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Enter full-screen mode, or return to windowed mode.  The entered
		''' full-screen mode may be either exclusive or simulated.  Exclusive
		''' mode is only available if <code>isFullScreenSupported</code>
		''' returns <code>true</code>.
		''' <p>
		''' Exclusive mode implies:
		''' <ul>
		''' <li>Windows cannot overlap the full-screen window.  All other application
		''' windows will always appear beneath the full-screen window in the Z-order.
		''' <li>There can be only one full-screen window on a device at any time,
		''' so calling this method while there is an existing full-screen Window
		''' will cause the existing full-screen window to
		''' return to windowed mode.
		''' <li>Input method windows are disabled.  It is advisable to call
		''' <code>Component.enableInputMethods(false)</code> to make a component
		''' a non-client of the input method framework.
		''' </ul>
		''' <p>
		''' The simulated full-screen mode places and resizes the window to the maximum
		''' possible visible area of the screen. However, the native windowing system
		''' may modify the requested geometry-related data, so that the {@code Window} object
		''' is placed and sized in a way that corresponds closely to the desktop settings.
		''' <p>
		''' When entering full-screen mode, if the window to be used as a
		''' full-screen window is not visible, this method will make it visible.
		''' It will remain visible when returning to windowed mode.
		''' <p>
		''' When entering full-screen mode, all the translucency effects are reset for
		''' the window. Its shape is set to {@code null}, the opacity value is set to
		''' 1.0f, and the background color alpha is set to 255 (completely opaque).
		''' These values are not restored when returning to windowed mode.
		''' <p>
		''' It is unspecified and platform-dependent how decorated windows operate
		''' in full-screen mode. For this reason, it is recommended to turn off
		''' the decorations in a {@code Frame} or {@code Dialog} object by using the
		''' {@code setUndecorated} method.
		''' <p>
		''' When returning to windowed mode from an exclusive full-screen window,
		''' any display changes made by calling {@code setDisplayMode} are
		''' automatically restored to their original state.
		''' </summary>
		''' <param name="w"> a window to use as the full-screen window; {@code null}
		''' if returning to windowed mode.  Some platforms expect the
		''' fullscreen window to be a top-level component (i.e., a {@code Frame});
		''' therefore it is preferable to use a {@code Frame} here rather than a
		''' {@code Window}.
		''' </param>
		''' <seealso cref= #isFullScreenSupported </seealso>
		''' <seealso cref= #getFullScreenWindow </seealso>
		''' <seealso cref= #setDisplayMode </seealso>
		''' <seealso cref= Component#enableInputMethods </seealso>
		''' <seealso cref= Component#setVisible </seealso>
		''' <seealso cref= Frame#setUndecorated </seealso>
		''' <seealso cref= Dialog#setUndecorated
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property fullScreenWindow As Window
			Set(  w As Window)
				If w IsNot Nothing Then
					If w.shape IsNot Nothing Then w.shape = Nothing
					If w.opacity < 1.0f Then w.opacity = 1.0f
					If Not w.opaque Then
						Dim bgColor As Color = w.background
						bgColor = New Color(bgColor.red, bgColor.green, bgColor.blue, 255)
						w.background = bgColor
					End If
					' Check if this window is in fullscreen mode on another device.
					Dim gc As GraphicsConfiguration = w.graphicsConfiguration
					If gc IsNot Nothing AndAlso gc.device IsNot Me AndAlso gc.device.fullScreenWindow Is w Then gc.device.fullScreenWindow = Nothing
				End If
				If fullScreenWindow IsNot Nothing AndAlso windowedModeBounds IsNot Nothing Then
					' if the window went into fs mode before it was realized it may
					' have (0,0) dimensions
					If windowedModeBounds.width = 0 Then windowedModeBounds.width = 1
					If windowedModeBounds.height = 0 Then windowedModeBounds.height = 1
					fullScreenWindow.bounds = windowedModeBounds
				End If
				' Set the full screen window
				SyncLock fsAppContextLock
					' Associate fullscreen window with current AppContext
					If w Is Nothing Then
						fullScreenAppContext = Nothing
					Else
						fullScreenAppContext = sun.awt.AppContext.appContext
					End If
					fullScreenWindow = w
				End SyncLock
				If fullScreenWindow IsNot Nothing Then
					windowedModeBounds = fullScreenWindow.bounds
					' Note that we use the graphics configuration of the device,
					' not the window's, because we're setting the fs window for
					' this device.
					Dim gc As GraphicsConfiguration = defaultConfiguration
					Dim screenBounds As Rectangle = gc.bounds
					If sun.awt.SunToolkit.isDispatchThreadForAppContext(fullScreenWindow) Then fullScreenWindow.graphicsConfiguration = gc
					fullScreenWindow.boundsnds(screenBounds.x, screenBounds.y, screenBounds.width, screenBounds.height)
					fullScreenWindow.visible = True
					fullScreenWindow.toFront()
				End If
			End Set
			Get
				Dim returnWindow As Window = Nothing
				SyncLock fsAppContextLock
					' Only return a handle to the current fs window if we are in the
					' same AppContext that set the fs window
					If fullScreenAppContext Is sun.awt.AppContext.appContext Then returnWindow = fullScreenWindow
				End SyncLock
				Return returnWindow
			End Get
		End Property


		''' <summary>
		''' Returns <code>true</code> if this <code>GraphicsDevice</code>
		''' supports low-level display changes.
		''' On some platforms low-level display changes may only be allowed in
		''' full-screen exclusive mode (i.e., if <seealso cref="#isFullScreenSupported()"/>
		''' returns {@code true} and the application has already entered
		''' full-screen mode using <seealso cref="#setFullScreenWindow"/>). </summary>
		''' <returns> whether low-level display changes are supported for this
		''' graphics device. </returns>
		''' <seealso cref= #isFullScreenSupported </seealso>
		''' <seealso cref= #setDisplayMode </seealso>
		''' <seealso cref= #setFullScreenWindow
		''' @since 1.4 </seealso>
		Public Overridable Property displayChangeSupported As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Sets the display mode of this graphics device. This is only allowed
		''' if <seealso cref="#isDisplayChangeSupported()"/> returns {@code true} and may
		''' require first entering full-screen exclusive mode using
		''' <seealso cref="#setFullScreenWindow"/> providing that full-screen exclusive mode is
		''' supported (i.e., <seealso cref="#isFullScreenSupported()"/> returns
		''' {@code true}).
		''' <p>
		''' 
		''' The display mode must be one of the display modes returned by
		''' <seealso cref="#getDisplayModes()"/>, with one exception: passing a display mode
		''' with <seealso cref="DisplayMode#REFRESH_RATE_UNKNOWN"/> refresh rate will result in
		''' selecting a display mode from the list of available display modes with
		''' matching width, height and bit depth.
		''' However, passing a display mode with <seealso cref="DisplayMode#BIT_DEPTH_MULTI"/>
		''' for bit depth is only allowed if such mode exists in the list returned by
		''' <seealso cref="#getDisplayModes()"/>.
		''' <p>
		''' Example code:
		''' <pre><code>
		''' Frame frame;
		''' DisplayMode newDisplayMode;
		''' GraphicsDevice gd;
		''' // create a Frame, select desired DisplayMode from the list of modes
		''' // returned by gd.getDisplayModes() ...
		''' 
		''' if (gd.isFullScreenSupported()) {
		'''     gd.setFullScreenWindow(frame);
		''' } else {
		'''    // proceed in non-full-screen mode
		'''    frame.setSize(...);
		'''    frame.setLocation(...);
		'''    frame.setVisible(true);
		''' }
		''' 
		''' if (gd.isDisplayChangeSupported()) {
		'''     gd.setDisplayMode(newDisplayMode);
		''' }
		''' </code></pre>
		''' </summary>
		''' <param name="dm"> The new display mode of this graphics device. </param>
		''' <exception cref="IllegalArgumentException"> if the <code>DisplayMode</code>
		''' supplied is <code>null</code>, or is not available in the array returned
		''' by <code>getDisplayModes</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if
		''' <code>isDisplayChangeSupported</code> returns <code>false</code> </exception>
		''' <seealso cref= #getDisplayMode </seealso>
		''' <seealso cref= #getDisplayModes </seealso>
		''' <seealso cref= #isDisplayChangeSupported
		''' @since 1.4 </seealso>
		Public Overridable Property displayMode As DisplayMode
			Set(  dm As DisplayMode)
				Throw New UnsupportedOperationException("Cannot change display mode")
			End Set
			Get
				Dim gc As GraphicsConfiguration = defaultConfiguration
				Dim r As Rectangle = gc.bounds
				Dim cm As java.awt.image.ColorModel = gc.colorModel
				Return New DisplayMode(r.width, r.height, cm.pixelSize, 0)
			End Get
		End Property


		''' <summary>
		''' Returns all display modes available for this
		''' <code>GraphicsDevice</code>.
		''' The returned display modes are allowed to have a refresh rate
		''' <seealso cref="DisplayMode#REFRESH_RATE_UNKNOWN"/> if it is indeterminate.
		''' Likewise, the returned display modes are allowed to have a bit depth
		''' <seealso cref="DisplayMode#BIT_DEPTH_MULTI"/> if it is indeterminate or if multiple
		''' bit depths are supported. </summary>
		''' <returns> all of the display modes available for this graphics device.
		''' @since 1.4 </returns>
		Public Overridable Property displayModes As DisplayMode()
			Get
				Return New DisplayMode() { displayMode }
			End Get
		End Property

		''' <summary>
		''' This method returns the number of bytes available in
		''' accelerated memory on this device.
		''' Some images are created or cached
		''' in accelerated memory on a first-come,
		''' first-served basis.  On some operating systems,
		''' this memory is a finite resource.  Calling this method
		''' and scheduling the creation and flushing of images carefully may
		''' enable applications to make the most efficient use of
		''' that finite resource.
		''' <br>
		''' Note that the number returned is a snapshot of how much
		''' memory is available; some images may still have problems
		''' being allocated into that memory.  For example, depending
		''' on operating system, driver, memory configuration, and
		''' thread situations, the full extent of the size reported
		''' may not be available for a given image.  There are further
		''' inquiry methods on the <seealso cref="ImageCapabilities"/> object
		''' associated with a VolatileImage that can be used to determine
		''' whether a particular VolatileImage has been created in accelerated
		''' memory. </summary>
		''' <returns> number of bytes available in accelerated memory.
		''' A negative return value indicates that the amount of accelerated memory
		''' on this GraphicsDevice is indeterminate. </returns>
		''' <seealso cref= java.awt.image.VolatileImage#flush </seealso>
		''' <seealso cref= ImageCapabilities#isAccelerated
		''' @since 1.4 </seealso>
		Public Overridable Property availableAcceleratedMemory As Integer
			Get
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns whether the given level of translucency is supported by
		''' this graphics device.
		''' </summary>
		''' <param name="translucencyKind"> a kind of translucency support </param>
		''' <returns> whether the given translucency kind is supported
		''' 
		''' @since 1.7 </returns>
		Public Overridable Function isWindowTranslucencySupported(  translucencyKind As WindowTranslucency) As Boolean
			Select Case translucencyKind
				Case WindowTranslucency.PERPIXEL_TRANSPARENT
					Return windowShapingSupported
				Case WindowTranslucency.TRANSLUCENT
					Return windowOpacitySupported
				Case WindowTranslucency.PERPIXEL_TRANSLUCENT
					Return windowPerpixelTranslucencySupported
			End Select
			Return False
		End Function

		''' <summary>
		''' Returns whether the windowing system supports changing the shape
		''' of top-level windows.
		''' Note that this method may sometimes return true, but the native
		''' windowing system may still not support the concept of
		''' shaping (due to the bugs in the windowing system).
		''' </summary>
		FriendShared ReadOnly PropertywindowShapingSupported As Boolean
			Get
				Dim curToolkit As Toolkit = Toolkit.defaultToolkit
				If Not(TypeOf curToolkit Is sun.awt.SunToolkit) Then Return False
				Return CType(curToolkit, sun.awt.SunToolkit).windowShapingSupported
			End Get
		End Property

		''' <summary>
		''' Returns whether the windowing system supports changing the opacity
		''' value of top-level windows.
		''' Note that this method may sometimes return true, but the native
		''' windowing system may still not support the concept of
		''' translucency (due to the bugs in the windowing system).
		''' </summary>
		FriendShared ReadOnly PropertywindowOpacitySupported As Boolean
			Get
				Dim curToolkit As Toolkit = Toolkit.defaultToolkit
				If Not(TypeOf curToolkit Is sun.awt.SunToolkit) Then Return False
				Return CType(curToolkit, sun.awt.SunToolkit).windowOpacitySupported
			End Get
		End Property

		Friend Overridable Property windowPerpixelTranslucencySupported As Boolean
			Get
		'        
		'         * Per-pixel alpha is supported if all the conditions are TRUE:
		'         *    1. The toolkit is a sort of SunToolkit
		'         *    2. The toolkit supports translucency in general
		'         *        (isWindowTranslucencySupported())
		'         *    3. There's at least one translucency-capable
		'         *        GraphicsConfiguration
		'         
				Dim curToolkit As Toolkit = Toolkit.defaultToolkit
				If Not(TypeOf curToolkit Is sun.awt.SunToolkit) Then Return False
				If Not CType(curToolkit, sun.awt.SunToolkit).windowTranslucencySupported Then Return False
    
				' TODO: cache translucency capable GC
				Return translucencyCapableGC IsNot Nothing
			End Get
		End Property

		Friend Overridable Property translucencyCapableGC As GraphicsConfiguration
			Get
				' If the default GC supports translucency return true.
				' It is important to optimize the verification this way,
				' see CR 6661196 for more details.
				Dim defaultGC As GraphicsConfiguration = defaultConfiguration
				If defaultGC.translucencyCapable Then Return defaultGC
    
				' ... otherwise iterate through all the GCs.
				Dim configs As GraphicsConfiguration() = configurations
				For j As Integer = 0 To configs.Length - 1
					If configs(j).translucencyCapable Then Return configs(j)
				Next j
    
				Return Nothing
			End Get
		End Property
	End Class

End Namespace
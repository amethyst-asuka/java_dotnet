Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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



	''' 
	''' <summary>
	''' The <code>GraphicsEnvironment</code> class describes the collection
	''' of <seealso cref="GraphicsDevice"/> objects and <seealso cref="java.awt.Font"/> objects
	''' available to a Java(tm) application on a particular platform.
	''' The resources in this <code>GraphicsEnvironment</code> might be local
	''' or on a remote machine.  <code>GraphicsDevice</code> objects can be
	''' screens, printers or image buffers and are the destination of
	''' <seealso cref="Graphics2D"/> drawing methods.  Each <code>GraphicsDevice</code>
	''' has a number of <seealso cref="GraphicsConfiguration"/> objects associated with
	''' it.  These objects specify the different configurations in which the
	''' <code>GraphicsDevice</code> can be used. </summary>
	''' <seealso cref= GraphicsDevice </seealso>
	''' <seealso cref= GraphicsConfiguration </seealso>

	Public MustInherit Class GraphicsEnvironment
		Private Shared localEnv As GraphicsEnvironment

		''' <summary>
		''' The headless state of the Toolkit and GraphicsEnvironment
		''' </summary>
		Private Shared headless As Boolean?

		''' <summary>
		''' The headless state assumed by default
		''' </summary>
		Private Shared defaultHeadless As Boolean?

		''' <summary>
		''' This is an abstract class and cannot be instantiated directly.
		''' Instances must be obtained from a suitable factory or query method.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the local <code>GraphicsEnvironment</code>. </summary>
		''' <returns> the local <code>GraphicsEnvironment</code> </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared localGraphicsEnvironment As GraphicsEnvironment
			Get
				If localEnv Is Nothing Then localEnv = createGE()
    
				Return localEnv
			End Get
		End Property

		''' <summary>
		''' Creates and returns the GraphicsEnvironment, according to the
		''' system property 'java.awt.graphicsenv'.
		''' </summary>
		''' <returns> the graphics environment </returns>
		Private Shared Function createGE() As GraphicsEnvironment
			Dim ge As GraphicsEnvironment
			Dim nm As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.awt.graphicsenv", Nothing))
			Try
	'          long t0 = System.currentTimeMillis();
				Dim geCls As Class
				Try
					' First we try if the bootclassloader finds the requested
					' class. This way we can avoid to run in a privileged block.
					geCls = CType(Type.GetType(nm), [Class])
				Catch ex As ClassNotFoundException
					' If the bootclassloader fails, we try again with the
					' application classloader.
					Dim cl As ClassLoader = ClassLoader.systemClassLoader
					geCls = CType(Type.GetType(nm, True, cl), [Class])
				End Try
				ge = geCls.newInstance()
	'          long t1 = System.currentTimeMillis();
	'          System.out.println("GE creation took " + (t1-t0)+ "ms.");
				If headless Then ge = New sun.java2d.HeadlessGraphicsEnvironment(ge)
			Catch e As ClassNotFoundException
				Throw New [Error]("Could not find class: " & nm)
			Catch e As InstantiationException
				Throw New [Error]("Could not instantiate Graphics Environment: " & nm)
			Catch e As IllegalAccessException
				Throw New [Error]("Could not access Graphics Environment: " & nm)
			End Try
			Return ge
		End Function

		''' <summary>
		''' Tests whether or not a display, keyboard, and mouse can be
		''' supported in this environment.  If this method returns true,
		''' a HeadlessException is thrown from areas of the Toolkit
		''' and GraphicsEnvironment that are dependent on a display,
		''' keyboard, or mouse. </summary>
		''' <returns> <code>true</code> if this environment cannot support
		''' a display, keyboard, and mouse; <code>false</code>
		''' otherwise </returns>
		''' <seealso cref= java.awt.HeadlessException
		''' @since 1.4 </seealso>
		Public Property Shared headless As Boolean
			Get
				Return headlessProperty
			End Get
		End Property

		''' <returns> warning message if headless state is assumed by default;
		''' null otherwise
		''' @since 1.5 </returns>
		Friend Property Shared headlessMessage As String
			Get
				If headless Is Nothing Then headlessProperty ' initialize the values
				Return If(defaultHeadless IsNot Boolean.TRUE, Nothing, vbLf & "No X11 DISPLAY variable was set, " & "but this program performed an operation which requires it.")
			End Get
		End Property

		''' <returns> the value of the property "java.awt.headless"
		''' @since 1.4 </returns>
		Private Property Shared headlessProperty As Boolean
			Get
				If headless Is Nothing Then java.security.AccessController.doPrivileged(CType(, java.security.PrivilegedAction(Of Void)) -> { String nm = System.getProperty("java.awt.headless"); if(nm Is Nothing) { if(System.getProperty("javaplugin.version") IsNot Nothing) { headless = defaultHeadless = Boolean.FALSE; } else { String osName = System.getProperty("os.name"); if(osName.contains("OS X") AndAlso "sun.awt.HToolkit".Equals(System.getProperty("awt.toolkit"))) { headless = defaultHeadless = Boolean.TRUE; } else { final String display = System.getenv("DISPLAY"); headless = defaultHeadless = ("Linux".Equals(osName) OrElse "SunOS".Equals(osName) OrElse "FreeBSD".Equals(osName) OrElse "NetBSD".Equals(osName) OrElse "OpenBSD".Equals(osName) OrElse "AIX".Equals(osName)) AndAlso (display Is Nothing OrElse display.Trim().empty); } } } else { headless = Convert.ToBoolean(nm); } Return Nothing; })
				Return headless
			End Get
		End Property

		''' <summary>
		''' Check for headless state and throw HeadlessException if headless
		''' @since 1.4
		''' </summary>
		Friend Shared Sub checkHeadless()
			If headless Then Throw New HeadlessException
		End Sub

		''' <summary>
		''' Returns whether or not a display, keyboard, and mouse can be
		''' supported in this graphics environment.  If this returns true,
		''' <code>HeadlessException</code> will be thrown from areas of the
		''' graphics environment that are dependent on a display, keyboard, or
		''' mouse. </summary>
		''' <returns> <code>true</code> if a display, keyboard, and mouse
		''' can be supported in this environment; <code>false</code>
		''' otherwise </returns>
		''' <seealso cref= java.awt.HeadlessException </seealso>
		''' <seealso cref= #isHeadless
		''' @since 1.4 </seealso>
		Public Overridable Property headlessInstance As Boolean
			Get
				' By default (local graphics environment), simply check the
				' headless property.
				Return headlessProperty
			End Get
		End Property

		''' <summary>
		''' Returns an array of all of the screen <code>GraphicsDevice</code>
		''' objects. </summary>
		''' <returns> an array containing all the <code>GraphicsDevice</code>
		''' objects that represent screen devices </returns>
		''' <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		''' <seealso cref= #isHeadless() </seealso>
		Public MustOverride ReadOnly Property screenDevices As GraphicsDevice()

		''' <summary>
		''' Returns the default screen <code>GraphicsDevice</code>. </summary>
		''' <returns> the <code>GraphicsDevice</code> that represents the
		''' default screen device </returns>
		''' <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		''' <seealso cref= #isHeadless() </seealso>
		Public MustOverride ReadOnly Property defaultScreenDevice As GraphicsDevice

		''' <summary>
		''' Returns a <code>Graphics2D</code> object for rendering into the
		''' specified <seealso cref="BufferedImage"/>. </summary>
		''' <param name="img"> the specified <code>BufferedImage</code> </param>
		''' <returns> a <code>Graphics2D</code> to be used for rendering into
		''' the specified <code>BufferedImage</code> </returns>
		''' <exception cref="NullPointerException"> if <code>img</code> is null </exception>
		Public MustOverride Function createGraphics(ByVal img As java.awt.image.BufferedImage) As Graphics2D

		''' <summary>
		''' Returns an array containing a one-point size instance of all fonts
		''' available in this <code>GraphicsEnvironment</code>.  Typical usage
		''' would be to allow a user to select a particular font.  Then, the
		''' application can size the font and set various font attributes by
		''' calling the <code>deriveFont</code> method on the chosen instance.
		''' <p>
		''' This method provides for the application the most precise control
		''' over which <code>Font</code> instance is used to render text.
		''' If a font in this <code>GraphicsEnvironment</code> has multiple
		''' programmable variations, only one
		''' instance of that <code>Font</code> is returned in the array, and
		''' other variations must be derived by the application.
		''' <p>
		''' If a font in this environment has multiple programmable variations,
		''' such as Multiple-Master fonts, only one instance of that font is
		''' returned in the <code>Font</code> array.  The other variations
		''' must be derived by the application.
		''' </summary>
		''' <returns> an array of <code>Font</code> objects </returns>
		''' <seealso cref= #getAvailableFontFamilyNames </seealso>
		''' <seealso cref= java.awt.Font </seealso>
		''' <seealso cref= java.awt.Font#deriveFont </seealso>
		''' <seealso cref= java.awt.Font#getFontName
		''' @since 1.2 </seealso>
		Public MustOverride ReadOnly Property allFonts As Font()

		''' <summary>
		''' Returns an array containing the names of all font families in this
		''' <code>GraphicsEnvironment</code> localized for the default locale,
		''' as returned by <code>Locale.getDefault()</code>.
		''' <p>
		''' Typical usage would be for presentation to a user for selection of
		''' a particular family name. An application can then specify this name
		''' when creating a font, in conjunction with a style, such as bold or
		''' italic, giving the font system flexibility in choosing its own best
		''' match among multiple fonts in the same font family.
		''' </summary>
		''' <returns> an array of <code>String</code> containing font family names
		''' localized for the default locale, or a suitable alternative
		''' name if no name exists for this locale. </returns>
		''' <seealso cref= #getAllFonts </seealso>
		''' <seealso cref= java.awt.Font </seealso>
		''' <seealso cref= java.awt.Font#getFamily
		''' @since 1.2 </seealso>
		Public MustOverride ReadOnly Property availableFontFamilyNames As String()

		''' <summary>
		''' Returns an array containing the names of all font families in this
		''' <code>GraphicsEnvironment</code> localized for the specified locale.
		''' <p>
		''' Typical usage would be for presentation to a user for selection of
		''' a particular family name. An application can then specify this name
		''' when creating a font, in conjunction with a style, such as bold or
		''' italic, giving the font system flexibility in choosing its own best
		''' match among multiple fonts in the same font family.
		''' </summary>
		''' <param name="l"> a <seealso cref="Locale"/> object that represents a
		''' particular geographical, political, or cultural region.
		''' Specifying <code>null</code> is equivalent to
		''' specifying <code>Locale.getDefault()</code>. </param>
		''' <returns> an array of <code>String</code> containing font family names
		''' localized for the specified <code>Locale</code>, or a
		''' suitable alternative name if no name exists for the specified locale. </returns>
		''' <seealso cref= #getAllFonts </seealso>
		''' <seealso cref= java.awt.Font </seealso>
		''' <seealso cref= java.awt.Font#getFamily
		''' @since 1.2 </seealso>
		Public MustOverride Function getAvailableFontFamilyNames(ByVal l As java.util.Locale) As String()

		''' <summary>
		''' Registers a <i>created</i> <code>Font</code>in this
		''' <code>GraphicsEnvironment</code>.
		''' A created font is one that was returned from calling
		''' <seealso cref="Font#createFont"/>, or derived from a created font by
		''' calling <seealso cref="Font#deriveFont"/>.
		''' After calling this method for such a font, it is available to
		''' be used in constructing new <code>Font</code>s by name or family name,
		''' and is enumerated by <seealso cref="#getAvailableFontFamilyNames"/> and
		''' <seealso cref="#getAllFonts"/> within the execution context of this
		''' application or applet. This means applets cannot register fonts in
		''' a way that they are visible to other applets.
		''' <p>
		''' Reasons that this method might not register the font and therefore
		''' return <code>false</code> are:
		''' <ul>
		''' <li>The font is not a <i>created</i> <code>Font</code>.
		''' <li>The font conflicts with a non-created <code>Font</code> already
		''' in this <code>GraphicsEnvironment</code>. For example if the name
		''' is that of a system font, or a logical font as described in the
		''' documentation of the <seealso cref="Font"/> class. It is implementation dependent
		''' whether a font may also conflict if it has the same family name
		''' as a system font.
		''' <p>Notice that an application can supersede the registration
		''' of an earlier created font with a new one.
		''' </ul> </summary>
		''' <returns> true if the <code>font</code> is successfully
		''' registered in this <code>GraphicsEnvironment</code>. </returns>
		''' <exception cref="NullPointerException"> if <code>font</code> is null
		''' @since 1.6 </exception>
		Public Overridable Function registerFont(ByVal font_Renamed As Font) As Boolean
			If font_Renamed Is Nothing Then Throw New NullPointerException("font cannot be null.")
			Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
			Return fm.registerFont(font_Renamed)
		End Function

		''' <summary>
		''' Indicates a preference for locale-specific fonts in the mapping of
		''' logical fonts to physical fonts. Calling this method indicates that font
		''' rendering should primarily use fonts specific to the primary writing
		''' system (the one indicated by the default encoding and the initial
		''' default locale). For example, if the primary writing system is
		''' Japanese, then characters should be rendered using a Japanese font
		''' if possible, and other fonts should only be used for characters for
		''' which the Japanese font doesn't have glyphs.
		''' <p>
		''' The actual change in font rendering behavior resulting from a call
		''' to this method is implementation dependent; it may have no effect at
		''' all, or the requested behavior may already match the default behavior.
		''' The behavior may differ between font rendering in lightweight
		''' and peered components.  Since calling this method requests a
		''' different font, clients should expect different metrics, and may need
		''' to recalculate window sizes and layout. Therefore this method should
		''' be called before user interface initialisation.
		''' @since 1.5
		''' </summary>
		Public Overridable Sub preferLocaleFonts()
			Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
			fm.preferLocaleFonts()
		End Sub

		''' <summary>
		''' Indicates a preference for proportional over non-proportional (e.g.
		''' dual-spaced CJK fonts) fonts in the mapping of logical fonts to
		''' physical fonts. If the default mapping contains fonts for which
		''' proportional and non-proportional variants exist, then calling
		''' this method indicates the mapping should use a proportional variant.
		''' <p>
		''' The actual change in font rendering behavior resulting from a call to
		''' this method is implementation dependent; it may have no effect at all.
		''' The behavior may differ between font rendering in lightweight and
		''' peered components. Since calling this method requests a
		''' different font, clients should expect different metrics, and may need
		''' to recalculate window sizes and layout. Therefore this method should
		''' be called before user interface initialisation.
		''' @since 1.5
		''' </summary>
		Public Overridable Sub preferProportionalFonts()
			Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
			fm.preferProportionalFonts()
		End Sub

		''' <summary>
		''' Returns the Point where Windows should be centered.
		''' It is recommended that centered Windows be checked to ensure they fit
		''' within the available display area using getMaximumWindowBounds(). </summary>
		''' <returns> the point where Windows should be centered
		''' </returns>
		''' <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		''' <seealso cref= #getMaximumWindowBounds
		''' @since 1.4 </seealso>
		Public Overridable Property centerPoint As Point
			Get
			' Default implementation: return the center of the usable bounds of the
			' default screen device.
				Dim usableBounds As Rectangle = sun.java2d.SunGraphicsEnvironment.getUsableBounds(defaultScreenDevice)
				Return New Point((usableBounds.width \ 2) + usableBounds.x, (usableBounds.height \ 2) + usableBounds.y)
			End Get
		End Property

		''' <summary>
		''' Returns the maximum bounds for centered Windows.
		''' These bounds account for objects in the native windowing system such as
		''' task bars and menu bars.  The returned bounds will reside on a single
		''' display with one exception: on multi-screen systems where Windows should
		''' be centered across all displays, this method returns the bounds of the
		''' entire display area.
		''' <p>
		''' To get the usable bounds of a single display, use
		''' <code>GraphicsConfiguration.getBounds()</code> and
		''' <code>Toolkit.getScreenInsets()</code>. </summary>
		''' <returns>  the maximum bounds for centered Windows
		''' </returns>
		''' <exception cref="HeadlessException"> if isHeadless() returns true </exception>
		''' <seealso cref= #getCenterPoint </seealso>
		''' <seealso cref= GraphicsConfiguration#getBounds </seealso>
		''' <seealso cref= Toolkit#getScreenInsets
		''' @since 1.4 </seealso>
		Public Overridable Property maximumWindowBounds As Rectangle
			Get
			' Default implementation: return the usable bounds of the default screen
			' device.  This is correct for Microsoft Windows and non-Xinerama X11.
				Return sun.java2d.SunGraphicsEnvironment.getUsableBounds(defaultScreenDevice)
			End Get
		End Property
	End Class

End Namespace
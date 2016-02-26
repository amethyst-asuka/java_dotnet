Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

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
	''' {@code UIManager} manages the current look and feel, the set of
	''' available look and feels, {@code PropertyChangeListeners} that
	''' are notified when the look and feel changes, look and feel defaults, and
	''' convenience methods for obtaining various default values.
	''' 
	''' <h3>Specifying the look and feel</h3>
	''' 
	''' The look and feel can be specified in two distinct ways: by
	''' specifying the fully qualified name of the class for the look and
	''' feel, or by creating an instance of {@code LookAndFeel} and passing
	''' it to {@code setLookAndFeel}. The following example illustrates
	''' setting the look and feel to the system look and feel:
	''' <pre>
	'''   UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
	''' </pre>
	''' The following example illustrates setting the look and feel based on
	''' class name:
	''' <pre>
	'''   UIManager.setLookAndFeel("javax.swing.plaf.metal.MetalLookAndFeel");
	''' </pre>
	''' Once the look and feel has been changed it is imperative to invoke
	''' {@code updateUI} on all {@code JComponents}. The method {@link
	''' SwingUtilities#updateComponentTreeUI} makes it easy to apply {@code
	''' updateUI} to a containment hierarchy. Refer to it for
	''' details. The exact behavior of not invoking {@code
	''' updateUI} after changing the look and feel is
	''' unspecified. It is very possible to receive unexpected exceptions,
	''' painting problems, or worse.
	''' 
	''' <h3>Default look and feel</h3>
	''' 
	''' The class used for the default look and feel is chosen in the following
	''' manner:
	''' <ol>
	'''   <li>If the system property <code>swing.defaultlaf</code> is
	'''       {@code non-null}, use its value as the default look and feel class
	'''       name.
	'''   <li>If the <seealso cref="java.util.Properties"/> file <code>swing.properties</code>
	'''       exists and contains the key <code>swing.defaultlaf</code>,
	'''       use its value as the default look and feel class name. The location
	'''       that is checked for <code>swing.properties</code> may vary depending
	'''       upon the implementation of the Java platform. Typically the
	'''       <code>swing.properties</code> file is located in the <code>lib</code>
	'''       subdirectory of the Java installation directory.
	'''       Refer to the release notes of the implementation being used for
	'''       further details.
	'''   <li>Otherwise use the cross platform look and feel.
	''' </ol>
	''' 
	''' <h3>Defaults</h3>
	''' 
	''' {@code UIManager} manages three sets of {@code UIDefaults}. In order, they
	''' are:
	''' <ol>
	'''   <li>Developer defaults. With few exceptions Swing does not
	'''       alter the developer defaults; these are intended to be modified
	'''       and used by the developer.
	'''   <li>Look and feel defaults. The look and feel defaults are
	'''       supplied by the look and feel at the time it is installed as the
	'''       current look and feel ({@code setLookAndFeel()} is invoked). The
	'''       look and feel defaults can be obtained using the {@code
	'''       getLookAndFeelDefaults()} method.
	'''   <li>System defaults. The system defaults are provided by Swing.
	''' </ol>
	''' Invoking any of the various {@code get} methods
	''' results in checking each of the defaults, in order, returning
	''' the first {@code non-null} value. For example, invoking
	''' {@code UIManager.getString("Table.foreground")} results in first
	''' checking developer defaults. If the developer defaults contain
	''' a value for {@code "Table.foreground"} it is returned, otherwise
	''' the look and feel defaults are checked, followed by the system defaults.
	''' <p>
	''' It's important to note that {@code getDefaults} returns a custom
	''' instance of {@code UIDefaults} with this resolution logic built into it.
	''' For example, {@code UIManager.getDefaults().getString("Table.foreground")}
	''' is equivalent to {@code UIManager.getString("Table.foreground")}. Both
	''' resolve using the algorithm just described. In many places the
	''' documentation uses the word defaults to refer to the custom instance
	''' of {@code UIDefaults} with the resolution logic as previously described.
	''' <p>
	''' When the look and feel is changed, {@code UIManager} alters only the
	''' look and feel defaults; the developer and system defaults are not
	''' altered by the {@code UIManager} in any way.
	''' <p>
	''' The set of defaults a particular look and feel supports is defined
	''' and documented by that look and feel. In addition, each look and
	''' feel, or {@code ComponentUI} provided by a look and feel, may
	''' access the defaults at different times in their life cycle. Some
	''' look and feels may aggressively look up defaults, so that changing a
	''' default may not have an effect after installing the look and feel.
	''' Other look and feels may lazily access defaults so that a change to
	''' the defaults may effect an existing look and feel. Finally, other look
	''' and feels might not configure themselves from the defaults table in
	''' any way. None-the-less it is usually the case that a look and feel
	''' expects certain defaults, so that in general
	''' a {@code ComponentUI} provided by one look and feel will not
	''' work with another look and feel.
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
	''' @author Thomas Ball
	''' @author Hans Muller
	''' </summary>
	<Serializable> _
	Public Class UIManager
		''' <summary>
		''' This class defines the state managed by the <code>UIManager</code>.  For
		''' Swing applications the fields in this class could just as well
		''' be static members of <code>UIManager</code> however we give them
		''' "AppContext"
		''' scope instead so that applets (and potentially multiple lightweight
		''' applications running in a single VM) have their own state. For example,
		''' an applet can alter its look and feel, see <code>setLookAndFeel</code>.
		''' Doing so has no affect on other applets (or the browser).
		''' </summary>
		Private Class LAFState
			Friend swingProps As java.util.Properties
			Private tables As UIDefaults() = New UIDefaults(1){}

			Friend initialized As Boolean = False
			Friend focusPolicyInitialized As Boolean = False
			Friend multiUIDefaults As New MultiUIDefaults(tables)
			Friend ___lookAndFeel As LookAndFeel
			Friend multiLookAndFeel As LookAndFeel = Nothing
			Friend auxLookAndFeels As List(Of LookAndFeel) = Nothing
			Friend changeSupport As javax.swing.event.SwingPropertyChangeSupport

			Friend installedLAFs As LookAndFeelInfo()

			Friend Overridable Property lookAndFeelDefaults As UIDefaults
				Get
					Return tables(0)
				End Get
				Set(ByVal x As UIDefaults)
					tables(0) = x
				End Set
			End Property

			Friend Overridable Property systemDefaults As UIDefaults
				Get
					Return tables(1)
				End Get
				Set(ByVal x As UIDefaults)
					tables(1) = x
				End Set
			End Property

			''' <summary>
			''' Returns the SwingPropertyChangeSupport for the current
			''' AppContext.  If <code>create</code> is a true, a non-null
			''' <code>SwingPropertyChangeSupport</code> will be returned, if
			''' <code>create</code> is false and this has not been invoked
			''' with true, null will be returned.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function getPropertyChangeSupport(ByVal create As Boolean) As javax.swing.event.SwingPropertyChangeSupport
				If create AndAlso changeSupport Is Nothing Then changeSupport = New javax.swing.event.SwingPropertyChangeSupport(GetType(UIManager))
				Return changeSupport
			End Function
		End Class




	'     Lock object used in place of class object for synchronization. (4187686)
	'     
		Private Shared ReadOnly classLock As New Object

		''' <summary>
		''' Return the <code>LAFState</code> object, lazily create one if necessary.
		''' All access to the <code>LAFState</code> fields is done via this method,
		''' for example:
		''' <pre>
		'''     getLAFState().initialized = true;
		''' </pre>
		''' </summary>
		Private Property Shared lAFState As LAFState
			Get
				Dim rv As LAFState = CType(SwingUtilities.appContextGet(sun.swing.SwingUtilities2.LAF_STATE_KEY), LAFState)
				If rv Is Nothing Then
					SyncLock classLock
						rv = CType(SwingUtilities.appContextGet(sun.swing.SwingUtilities2.LAF_STATE_KEY), LAFState)
						If rv Is Nothing Then SwingUtilities.appContextPut(sun.swing.SwingUtilities2.LAF_STATE_KEY, (rv = New LAFState))
					End SyncLock
				End If
				Return rv
			End Get
		End Property


	'     Keys used in the <code>swing.properties</code> properties file.
	'     * See loadUserProperties(), initialize().
	'     

		Private Const defaultLAFKey As String = "swing.defaultlaf"
		Private Const auxiliaryLAFsKey As String = "swing.auxiliarylaf"
		Private Const multiplexingLAFKey As String = "swing.plaf.multiplexinglaf"
		Private Const installedLAFsKey As String = "swing.installedlafs"
		Private Const disableMnemonicKey As String = "swing.disablenavaids"

		''' <summary>
		''' Return a <code>swing.properties</code> file key for the attribute of specified
		''' look and feel.  The attr is either "name" or "class", a typical
		''' key would be: "swing.installedlaf.windows.name"
		''' </summary>
		Private Shared Function makeInstalledLAFKey(ByVal laf As String, ByVal attr As String) As String
			Return "swing.installedlaf." & laf & "." & attr
		End Function

		''' <summary>
		''' The location of the <code>swing.properties</code> property file is
		''' implementation-specific.
		''' It is typically located in the <code>lib</code> subdirectory of the Java
		''' installation directory. This method returns a bogus filename
		''' if <code>java.home</code> isn't defined.
		''' </summary>
		Private Shared Function makeSwingPropertiesFilename() As String
			Dim sep As String = File.separator
			' No need to wrap this in a doPrivileged as it's called from
			' a doPrivileged.
			Dim javaHome As String = System.getProperty("java.home")
			If javaHome Is Nothing Then javaHome = "<java.home undefined>"
			Return javaHome + sep & "lib" & sep & "swing.properties"
		End Function


		''' <summary>
		''' Provides a little information about an installed
		''' <code>LookAndFeel</code> for the sake of configuring a menu or
		''' for initial application set up.
		''' </summary>
		''' <seealso cref= UIManager#getInstalledLookAndFeels </seealso>
		''' <seealso cref= LookAndFeel </seealso>
		Public Class LookAndFeelInfo
			Private name As String
			Private className As String

			''' <summary>
			''' Constructs a <code>UIManager</code>s
			''' <code>LookAndFeelInfo</code> object.
			''' </summary>
			''' <param name="name">      a <code>String</code> specifying the name of
			'''                      the look and feel </param>
			''' <param name="className"> a <code>String</code> specifying the name of
			'''                      the class that implements the look and feel </param>
			Public Sub New(ByVal name As String, ByVal className As String)
				Me.name = name
				Me.className = className
			End Sub

			''' <summary>
			''' Returns the name of the look and feel in a form suitable
			''' for a menu or other presentation </summary>
			''' <returns> a <code>String</code> containing the name </returns>
			''' <seealso cref= LookAndFeel#getName </seealso>
			Public Overridable Property name As String
				Get
					Return name
				End Get
			End Property

			''' <summary>
			''' Returns the name of the class that implements this look and feel. </summary>
			''' <returns> the name of the class that implements this
			'''              <code>LookAndFeel</code> </returns>
			''' <seealso cref= LookAndFeel </seealso>
			Public Overridable Property className As String
				Get
					Return className
				End Get
			End Property

			''' <summary>
			''' Returns a string that displays and identifies this
			''' object's properties.
			''' </summary>
			''' <returns> a <code>String</code> representation of this object </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[" & name & " " & className & "]"
			End Function
		End Class


		''' <summary>
		''' The default value of <code>installedLAFS</code> is used when no
		''' <code>swing.properties</code>
		''' file is available or if the file doesn't contain a "swing.installedlafs"
		''' property.
		''' </summary>
		''' <seealso cref= #initializeInstalledLAFs </seealso>
		Private Shared installedLAFs As LookAndFeelInfo()

		Shared Sub New()
			Dim iLAFs As New List(Of LookAndFeelInfo)(4)
			iLAFs.Add(New LookAndFeelInfo("Metal", "javax.swing.plaf.metal.MetalLookAndFeel"))
			iLAFs.Add(New LookAndFeelInfo("Nimbus", "javax.swing.plaf.nimbus.NimbusLookAndFeel"))
			iLAFs.Add(New LookAndFeelInfo("CDE/Motif", "com.sun.java.swing.plaf.motif.MotifLookAndFeel"))

			' Only include windows on Windows boxs.
			Dim osType As sun.awt.OSInfo.OSType = java.security.AccessController.doPrivileged(sun.awt.OSInfo.oSTypeAction)
			If osType Is sun.awt.OSInfo.OSType.WINDOWS Then
				iLAFs.Add(New LookAndFeelInfo("Windows", "com.sun.java.swing.plaf.windows.WindowsLookAndFeel"))
				If java.awt.Toolkit.defaultToolkit.getDesktopProperty("win.xpstyle.themeActive") IsNot Nothing Then iLAFs.Add(New LookAndFeelInfo("Windows Classic", "com.sun.java.swing.plaf.windows.WindowsClassicLookAndFeel"))
			ElseIf osType Is sun.awt.OSInfo.OSType.MACOSX Then
				iLAFs.Add(New LookAndFeelInfo("Mac OS X", "com.apple.laf.AquaLookAndFeel"))
			Else
				' GTK is not shipped on Windows.
				iLAFs.Add(New LookAndFeelInfo("GTK+", "com.sun.java.swing.plaf.gtk.GTKLookAndFeel"))
			End If
			installedLAFs = iLAFs.ToArray()
		End Sub


		''' <summary>
		''' Returns an array of {@code LookAndFeelInfo}s representing the
		''' {@code LookAndFeel} implementations currently available. The
		''' <code>LookAndFeelInfo</code> objects can be used by an
		''' application to construct a menu of look and feel options for
		''' the user, or to determine which look and feel to set at startup
		''' time. To avoid the penalty of creating numerous {@code
		''' LookAndFeel} objects, {@code LookAndFeelInfo} maintains the
		''' class name of the {@code LookAndFeel} class, not the actual
		''' {@code LookAndFeel} instance.
		''' <p>
		''' The following example illustrates setting the current look and feel
		''' from an instance of {@code LookAndFeelInfo}:
		''' <pre>
		'''   UIManager.setLookAndFeel(info.getClassName());
		''' </pre>
		''' </summary>
		''' <returns> an array of <code>LookAndFeelInfo</code> objects </returns>
		''' <seealso cref= #setLookAndFeel </seealso>
		Public Property Shared installedLookAndFeels As LookAndFeelInfo()
			Get
				maybeInitialize()
				Dim ilafs As LookAndFeelInfo() = lAFState.installedLAFs
				If ilafs Is Nothing Then ilafs = installedLAFs
				Dim rv As LookAndFeelInfo() = New LookAndFeelInfo(ilafs.Length - 1){}
				Array.Copy(ilafs, 0, rv, 0, ilafs.Length)
				Return rv
			End Get
			Set(ByVal infos As LookAndFeelInfo())
				maybeInitialize()
				Dim newInfos As LookAndFeelInfo() = New LookAndFeelInfo(infos.Length - 1){}
				Array.Copy(infos, 0, newInfos, 0, infos.Length)
				lAFState.installedLAFs = newInfos
			End Set
		End Property




		''' <summary>
		''' Adds the specified look and feel to the set of available look
		''' and feels. While this method allows a {@code null} {@code info},
		''' it is strongly recommended that a {@code non-null} value be used.
		''' </summary>
		''' <param name="info"> a <code>LookAndFeelInfo</code> object that names the
		'''          look and feel and identifies the class that implements it </param>
		''' <seealso cref= #setInstalledLookAndFeels </seealso>
		Public Shared Sub installLookAndFeel(ByVal info As LookAndFeelInfo)
			Dim infos As LookAndFeelInfo() = installedLookAndFeels
			Dim newInfos As LookAndFeelInfo() = New LookAndFeelInfo(infos.Length){}
			Array.Copy(infos, 0, newInfos, 0, infos.Length)
			newInfos(infos.Length) = info
			installedLookAndFeels = newInfos
		End Sub


		''' <summary>
		''' Adds the specified look and feel to the set of available look
		''' and feels. While this method does not check the
		''' arguments in any way, it is strongly recommended that {@code
		''' non-null} values be supplied.
		''' </summary>
		''' <param name="name"> descriptive name of the look and feel </param>
		''' <param name="className"> name of the class that implements the look and feel </param>
		''' <seealso cref= #setInstalledLookAndFeels </seealso>
		Public Shared Sub installLookAndFeel(ByVal name As String, ByVal className As String)
			installLookAndFeel(New LookAndFeelInfo(name, className))
		End Sub


		''' <summary>
		''' Returns the current look and feel or <code>null</code>.
		''' </summary>
		''' <returns> current look and feel, or <code>null</code> </returns>
		''' <seealso cref= #setLookAndFeel </seealso>
		Public Property Shared lookAndFeel As LookAndFeel
			Get
				maybeInitialize()
				Return lAFState.___lookAndFeel
			End Get
			Set(ByVal newLookAndFeel As LookAndFeel)
				If (newLookAndFeel IsNot Nothing) AndAlso (Not newLookAndFeel.supportedLookAndFeel) Then
					Dim s As String = newLookAndFeel.ToString() & " not supported on this platform"
					Throw New UnsupportedLookAndFeelException(s)
				End If
    
				Dim ___lafState As LAFState = lAFState
				Dim oldLookAndFeel As LookAndFeel = ___lafState.___lookAndFeel
				If oldLookAndFeel IsNot Nothing Then oldLookAndFeel.uninitialize()
    
				___lafState.___lookAndFeel = newLookAndFeel
				If newLookAndFeel IsNot Nothing Then
					sun.swing.DefaultLookup.defaultLookup = Nothing
					newLookAndFeel.initialize()
					___lafState.lookAndFeelDefaults = newLookAndFeel.defaults
				Else
					___lafState.lookAndFeelDefaults = Nothing
				End If
    
				Dim changeSupport As javax.swing.event.SwingPropertyChangeSupport = ___lafState.getPropertyChangeSupport(False)
				If changeSupport IsNot Nothing Then changeSupport.firePropertyChange("lookAndFeel", oldLookAndFeel, newLookAndFeel)
			End Set
		End Property




		''' <summary>
		''' Loads the {@code LookAndFeel} specified by the given class
		''' name, using the current thread's context class loader, and
		''' passes it to {@code setLookAndFeel(LookAndFeel)}.
		''' </summary>
		''' <param name="className">  a string specifying the name of the class that implements
		'''        the look and feel </param>
		''' <exception cref="ClassNotFoundException"> if the <code>LookAndFeel</code>
		'''           class could not be found </exception>
		''' <exception cref="InstantiationException"> if a new instance of the class
		'''          couldn't be created </exception>
		''' <exception cref="IllegalAccessException"> if the class or initializer isn't accessible </exception>
		''' <exception cref="UnsupportedLookAndFeelException"> if
		'''          <code>lnf.isSupportedLookAndFeel()</code> is false </exception>
		''' <exception cref="ClassCastException"> if {@code className} does not identify
		'''         a class that extends {@code LookAndFeel} </exception>
		Public Shared Property lookAndFeel As String
			Set(ByVal className As String)
				If "javax.swing.plaf.metal.MetalLookAndFeel".Equals(className) Then
					' Avoid reflection for the common case of metal.
					lookAndFeel = New javax.swing.plaf.metal.MetalLookAndFeel
				Else
					Dim lnfClass As Type = SwingUtilities.loadSystemClass(className)
					lookAndFeel = CType(lnfClass.newInstance(), LookAndFeel)
				End If
			End Set
		End Property

		''' <summary>
		''' Returns the name of the <code>LookAndFeel</code> class that implements
		''' the native system look and feel if there is one, otherwise
		''' the name of the default cross platform <code>LookAndFeel</code>
		''' class. This value can be overriden by setting the
		''' <code>swing.systemlaf</code> system property.
		''' </summary>
		''' <returns> the <code>String</code> of the <code>LookAndFeel</code>
		'''          class
		''' </returns>
		''' <seealso cref= #setLookAndFeel </seealso>
		''' <seealso cref= #getCrossPlatformLookAndFeelClassName </seealso>
		Public Property Shared systemLookAndFeelClassName As String
			Get
				Dim systemLAF As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.systemlaf"))
				If systemLAF IsNot Nothing Then Return systemLAF
				Dim osType As sun.awt.OSInfo.OSType = java.security.AccessController.doPrivileged(sun.awt.OSInfo.oSTypeAction)
				If osType Is sun.awt.OSInfo.OSType.WINDOWS Then
					Return "com.sun.java.swing.plaf.windows.WindowsLookAndFeel"
				Else
					Dim desktop As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("sun.desktop"))
					Dim toolkit As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
					If "gnome".Equals(desktop) AndAlso TypeOf toolkit Is sun.awt.SunToolkit AndAlso CType(toolkit, sun.awt.SunToolkit).nativeGTKAvailable Then Return "com.sun.java.swing.plaf.gtk.GTKLookAndFeel"
					If osType Is sun.awt.OSInfo.OSType.MACOSX Then
						If toolkit.GetType().name.Equals("sun.lwawt.macosx.LWCToolkit") Then Return "com.apple.laf.AquaLookAndFeel"
					End If
					If osType Is sun.awt.OSInfo.OSType.SOLARIS Then Return "com.sun.java.swing.plaf.motif.MotifLookAndFeel"
				End If
				Return crossPlatformLookAndFeelClassName
			End Get
		End Property


		''' <summary>
		''' Returns the name of the <code>LookAndFeel</code> class that implements
		''' the default cross platform look and feel -- the Java
		''' Look and Feel (JLF).  This value can be overriden by setting the
		''' <code>swing.crossplatformlaf</code> system property.
		''' </summary>
		''' <returns>  a string with the JLF implementation-class </returns>
		''' <seealso cref= #setLookAndFeel </seealso>
		''' <seealso cref= #getSystemLookAndFeelClassName </seealso>
		Public Property Shared crossPlatformLookAndFeelClassName As String
			Get
				Dim laf As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.crossplatformlaf"))
				If laf IsNot Nothing Then Return laf
				Return "javax.swing.plaf.metal.MetalLookAndFeel"
			End Get
		End Property


		''' <summary>
		''' Returns the defaults. The returned defaults resolve using the
		''' logic specified in the class documentation.
		''' </summary>
		''' <returns> a <code>UIDefaults</code> object containing the default values </returns>
		Public Property Shared defaults As UIDefaults
			Get
				maybeInitialize()
				Return lAFState.multiUIDefaults
			End Get
		End Property

		''' <summary>
		''' Returns a font from the defaults. If the value for {@code key} is
		''' not a {@code Font}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the font </param>
		''' <returns> the <code>Font</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getFont(ByVal key As Object) As java.awt.Font
			Return defaults.getFont(key)
		End Function

		''' <summary>
		''' Returns a font from the defaults that is appropriate
		''' for the given locale. If the value for {@code key} is
		''' not a {@code Font}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the font </param>
		''' <param name="l"> the <code>Locale</code> for which the font is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Font</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getFont(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Font
			Return defaults.getFont(key,l)
		End Function

		''' <summary>
		''' Returns a color from the defaults. If the value for {@code key} is
		''' not a {@code Color}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the color </param>
		''' <returns> the <code>Color</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getColor(ByVal key As Object) As java.awt.Color
			Return defaults.getColor(key)
		End Function

		''' <summary>
		''' Returns a color from the defaults that is appropriate
		''' for the given locale. If the value for {@code key} is
		''' not a {@code Color}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the color </param>
		''' <param name="l"> the <code>Locale</code> for which the color is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Color</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getColor(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Color
			Return defaults.getColor(key,l)
		End Function

		''' <summary>
		''' Returns an <code>Icon</code> from the defaults. If the value for
		''' {@code key} is not an {@code Icon}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the icon </param>
		''' <returns> the <code>Icon</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getIcon(ByVal key As Object) As Icon
			Return defaults.getIcon(key)
		End Function

		''' <summary>
		''' Returns an <code>Icon</code> from the defaults that is appropriate
		''' for the given locale. If the value for
		''' {@code key} is not an {@code Icon}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the icon </param>
		''' <param name="l"> the <code>Locale</code> for which the icon is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Icon</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getIcon(ByVal key As Object, ByVal l As java.util.Locale) As Icon
			Return defaults.getIcon(key,l)
		End Function

		''' <summary>
		''' Returns a border from the defaults. If the value for
		''' {@code key} is not a {@code Border}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the border </param>
		''' <returns> the <code>Border</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getBorder(ByVal key As Object) As javax.swing.border.Border
			Return defaults.getBorder(key)
		End Function

		''' <summary>
		''' Returns a border from the defaults that is appropriate
		''' for the given locale.  If the value for
		''' {@code key} is not a {@code Border}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the border </param>
		''' <param name="l"> the <code>Locale</code> for which the border is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Border</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getBorder(ByVal key As Object, ByVal l As java.util.Locale) As javax.swing.border.Border
			Return defaults.getBorder(key,l)
		End Function

		''' <summary>
		''' Returns a string from the defaults. If the value for
		''' {@code key} is not a {@code String}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the string </param>
		''' <returns> the <code>String</code> </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getString(ByVal key As Object) As String
			Return defaults.getString(key)
		End Function

		''' <summary>
		''' Returns a string from the defaults that is appropriate for the
		''' given locale.  If the value for
		''' {@code key} is not a {@code String}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the string </param>
		''' <param name="l"> the <code>Locale</code> for which the string is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>String</code>
		''' @since 1.4 </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getString(ByVal key As Object, ByVal l As java.util.Locale) As String
			Return defaults.getString(key,l)
		End Function

		''' <summary>
		''' Returns a string from the defaults that is appropriate for the
		''' given locale.  If the value for
		''' {@code key} is not a {@code String}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the string </param>
		''' <param name="c"> {@code Component} used to determine the locale;
		'''          {@code null} implies the default locale as
		'''          returned by {@code Locale.getDefault()} </param>
		''' <returns> the <code>String</code> </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Friend Shared Function getString(ByVal key As Object, ByVal c As java.awt.Component) As String
			Dim l As java.util.Locale = If(c Is Nothing, java.util.Locale.default, c.locale)
			Return getString(key, l)
		End Function

		''' <summary>
		''' Returns an integer from the defaults. If the value for
		''' {@code key} is not an {@code Integer}, or does not exist,
		''' {@code 0} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the int </param>
		''' <returns> the int </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getInt(ByVal key As Object) As Integer
			Return defaults.getInt(key)
		End Function

		''' <summary>
		''' Returns an integer from the defaults that is appropriate
		''' for the given locale. If the value for
		''' {@code key} is not an {@code Integer}, or does not exist,
		''' {@code 0} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the int </param>
		''' <param name="l"> the <code>Locale</code> for which the int is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the int </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getInt(ByVal key As Object, ByVal l As java.util.Locale) As Integer
			Return defaults.getInt(key,l)
		End Function

		''' <summary>
		''' Returns a boolean from the defaults which is associated with
		''' the key value. If the key is not found or the key doesn't represent
		''' a boolean value then {@code false} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the key for the desired boolean value </param>
		''' <returns> the boolean value corresponding to the key </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getBoolean(ByVal key As Object) As Boolean
			Return defaults.getBoolean(key)
		End Function

		''' <summary>
		''' Returns a boolean from the defaults which is associated with
		''' the key value and the given <code>Locale</code>. If the key is not
		''' found or the key doesn't represent
		''' a boolean value then {@code false} will be returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the key for the desired
		'''             boolean value </param>
		''' <param name="l"> the <code>Locale</code> for which the boolean is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the boolean value corresponding to the key </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getBoolean(ByVal key As Object, ByVal l As java.util.Locale) As Boolean
			Return defaults.getBoolean(key,l)
		End Function

		''' <summary>
		''' Returns an <code>Insets</code> object from the defaults. If the value
		''' for {@code key} is not an {@code Insets}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the <code>Insets</code> object </param>
		''' <returns> the <code>Insets</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getInsets(ByVal key As Object) As java.awt.Insets
			Return defaults.getInsets(key)
		End Function

		''' <summary>
		''' Returns an <code>Insets</code> object from the defaults that is
		''' appropriate for the given locale. If the value
		''' for {@code key} is not an {@code Insets}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the <code>Insets</code> object </param>
		''' <param name="l"> the <code>Locale</code> for which the object is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Insets</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getInsets(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Insets
			Return defaults.getInsets(key,l)
		End Function

		''' <summary>
		''' Returns a dimension from the defaults. If the value
		''' for {@code key} is not a {@code Dimension}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the dimension object </param>
		''' <returns> the <code>Dimension</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function getDimension(ByVal key As Object) As java.awt.Dimension
			Return defaults.getDimension(key)
		End Function

		''' <summary>
		''' Returns a dimension from the defaults that is appropriate
		''' for the given locale. If the value
		''' for {@code key} is not a {@code Dimension}, {@code null} is returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the dimension object </param>
		''' <param name="l"> the <code>Locale</code> for which the object is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Dimension</code> object </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function getDimension(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Dimension
			Return defaults.getDimension(key,l)
		End Function

		''' <summary>
		''' Returns an object from the defaults.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the desired object </param>
		''' <returns> the <code>Object</code> </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		Public Shared Function [get](ByVal key As Object) As Object
			Return defaults(key)
		End Function

		''' <summary>
		''' Returns an object from the defaults that is appropriate for
		''' the given locale.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the desired object </param>
		''' <param name="l"> the <code>Locale</code> for which the object is desired; refer
		'''        to {@code UIDefaults} for details on how a {@code null}
		'''        {@code Locale} is handled </param>
		''' <returns> the <code>Object</code> </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null}
		''' @since 1.4 </exception>
		Public Shared Function [get](ByVal key As Object, ByVal l As java.util.Locale) As Object
			Return defaults.get(key,l)
		End Function

		''' <summary>
		''' Stores an object in the developer defaults. This is a cover method
		''' for {@code getDefaults().put(key, value)}. This only effects the
		''' developer defaults, not the system or look and feel defaults.
		''' </summary>
		''' <param name="key">    an <code>Object</code> specifying the retrieval key </param>
		''' <param name="value">  the <code>Object</code> to store; refer to
		'''               {@code UIDefaults} for details on how {@code null} is
		'''               handled </param>
		''' <returns> the <code>Object</code> returned by <seealso cref="UIDefaults#put"/> </returns>
		''' <exception cref="NullPointerException"> if {@code key} is {@code null} </exception>
		''' <seealso cref= UIDefaults#put </seealso>
		Public Shared Function put(ByVal key As Object, ByVal value As Object) As Object
				defaults(key) = value
				Return defaults(key)
		End Function

		''' <summary>
		''' Returns the appropriate {@code ComponentUI} implementation for
		''' {@code target}. Typically, this is a cover for
		''' {@code getDefaults().getUI(target)}. However, if an auxiliary
		''' look and feel has been installed, this first invokes
		''' {@code getUI(target)} on the multiplexing look and feel's
		''' defaults, and returns that value if it is {@code non-null}.
		''' </summary>
		''' <param name="target"> the <code>JComponent</code> to return the
		'''        {@code ComponentUI} for </param>
		''' <returns> the <code>ComponentUI</code> object for {@code target} </returns>
		''' <exception cref="NullPointerException"> if {@code target} is {@code null} </exception>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Shared Function getUI(ByVal target As JComponent) As javax.swing.plaf.ComponentUI
			maybeInitialize()
			maybeInitializeFocusPolicy(target)
			Dim ___ui As javax.swing.plaf.ComponentUI = Nothing
			Dim multiLAF As LookAndFeel = lAFState.multiLookAndFeel
			If multiLAF IsNot Nothing Then ___ui = multiLAF.defaults.getUI(target)
			If ___ui Is Nothing Then ___ui = defaults.getUI(target)
			Return ___ui
		End Function


		''' <summary>
		''' Returns the {@code UIDefaults} from the current look and feel,
		''' that were obtained at the time the look and feel was installed.
		''' <p>
		''' In general, developers should use the {@code UIDefaults} returned from
		''' {@code getDefaults()}. As the current look and feel may expect
		''' certain values to exist, altering the {@code UIDefaults} returned
		''' from this method could have unexpected results.
		''' </summary>
		''' <returns> <code>UIDefaults</code> from the current look and feel </returns>
		''' <seealso cref= #getDefaults </seealso>
		''' <seealso cref= #setLookAndFeel(LookAndFeel) </seealso>
		''' <seealso cref= LookAndFeel#getDefaults </seealso>
		Public Property Shared lookAndFeelDefaults As UIDefaults
			Get
				maybeInitialize()
				Return lAFState.lookAndFeelDefaults
			End Get
		End Property

		''' <summary>
		''' Finds the Multiplexing <code>LookAndFeel</code>.
		''' </summary>
		Private Property Shared multiLookAndFeel As LookAndFeel
			Get
				Dim ___multiLookAndFeel As LookAndFeel = lAFState.multiLookAndFeel
				If ___multiLookAndFeel Is Nothing Then
					Dim defaultName As String = "javax.swing.plaf.multi.MultiLookAndFeel"
					Dim className As String = lAFState.swingProps.getProperty(multiplexingLAFKey, defaultName)
					Try
						Dim lnfClass As Type = SwingUtilities.loadSystemClass(className)
						___multiLookAndFeel = CType(lnfClass.newInstance(), LookAndFeel)
					Catch exc As Exception
						Console.Error.WriteLine("UIManager: failed loading " & className)
					End Try
				End If
				Return ___multiLookAndFeel
			End Get
		End Property

		''' <summary>
		''' Adds a <code>LookAndFeel</code> to the list of auxiliary look and feels.
		''' The auxiliary look and feels tell the multiplexing look and feel what
		''' other <code>LookAndFeel</code> classes for a component instance are to be used
		''' in addition to the default <code>LookAndFeel</code> class when creating a
		''' multiplexing UI.  The change will only take effect when a new
		''' UI class is created or when the default look and feel is changed
		''' on a component instance.
		''' <p>Note these are not the same as the installed look and feels.
		''' </summary>
		''' <param name="laf"> the <code>LookAndFeel</code> object </param>
		''' <seealso cref= #removeAuxiliaryLookAndFeel </seealso>
		''' <seealso cref= #setLookAndFeel </seealso>
		''' <seealso cref= #getAuxiliaryLookAndFeels </seealso>
		''' <seealso cref= #getInstalledLookAndFeels </seealso>
		Public Shared Sub addAuxiliaryLookAndFeel(ByVal laf As LookAndFeel)
			maybeInitialize()

			If Not laf.supportedLookAndFeel Then Return
			Dim v As List(Of LookAndFeel) = lAFState.auxLookAndFeels
			If v Is Nothing Then v = New List(Of LookAndFeel)

			If Not v.Contains(laf) Then
				v.Add(laf)
				laf.initialize()
				lAFState.auxLookAndFeels = v

				If lAFState.multiLookAndFeel Is Nothing Then lAFState.multiLookAndFeel = multiLookAndFeel
			End If
		End Sub

		''' <summary>
		''' Removes a <code>LookAndFeel</code> from the list of auxiliary look and feels.
		''' The auxiliary look and feels tell the multiplexing look and feel what
		''' other <code>LookAndFeel</code> classes for a component instance are to be used
		''' in addition to the default <code>LookAndFeel</code> class when creating a
		''' multiplexing UI.  The change will only take effect when a new
		''' UI class is created or when the default look and feel is changed
		''' on a component instance.
		''' <p>Note these are not the same as the installed look and feels. </summary>
		''' <returns> true if the <code>LookAndFeel</code> was removed from the list </returns>
		''' <seealso cref= #removeAuxiliaryLookAndFeel </seealso>
		''' <seealso cref= #getAuxiliaryLookAndFeels </seealso>
		''' <seealso cref= #setLookAndFeel </seealso>
		''' <seealso cref= #getInstalledLookAndFeels </seealso>
		Public Shared Function removeAuxiliaryLookAndFeel(ByVal laf As LookAndFeel) As Boolean
			maybeInitialize()

			Dim result As Boolean

			Dim v As List(Of LookAndFeel) = lAFState.auxLookAndFeels
			If (v Is Nothing) OrElse (v.Count = 0) Then Return False

			result = v.Remove(laf)
			If result Then
				If v.Count = 0 Then
					lAFState.auxLookAndFeels = Nothing
					lAFState.multiLookAndFeel = Nothing
				Else
					lAFState.auxLookAndFeels = v
				End If
			End If
			laf.uninitialize()

			Return result
		End Function

		''' <summary>
		''' Returns the list of auxiliary look and feels (can be <code>null</code>).
		''' The auxiliary look and feels tell the multiplexing look and feel what
		''' other <code>LookAndFeel</code> classes for a component instance are
		''' to be used in addition to the default LookAndFeel class when creating a
		''' multiplexing UI.
		''' <p>Note these are not the same as the installed look and feels.
		''' </summary>
		''' <returns> list of auxiliary <code>LookAndFeel</code>s or <code>null</code> </returns>
		''' <seealso cref= #addAuxiliaryLookAndFeel </seealso>
		''' <seealso cref= #removeAuxiliaryLookAndFeel </seealso>
		''' <seealso cref= #setLookAndFeel </seealso>
		''' <seealso cref= #getInstalledLookAndFeels </seealso>
		Public Property Shared auxiliaryLookAndFeels As LookAndFeel()
			Get
				maybeInitialize()
    
				Dim v As List(Of LookAndFeel) = lAFState.auxLookAndFeels
				If (v Is Nothing) OrElse (v.Count = 0) Then
					Return Nothing
				Else
					Dim rv As LookAndFeel() = New LookAndFeel(v.Count - 1){}
					For i As Integer = 0 To rv.Length - 1
						rv(i) = v(i)
					Next i
					Return rv
				End If
			End Get
		End Property


		''' <summary>
		''' Adds a <code>PropertyChangeListener</code> to the listener list.
		''' The listener is registered for all properties.
		''' </summary>
		''' <param name="listener">  the <code>PropertyChangeListener</code> to be added </param>
		''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
		Public Shared Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			SyncLock classLock
				lAFState.getPropertyChangeSupport(True).addPropertyChangeListener(listener)
			End SyncLock
		End Sub


		''' <summary>
		''' Removes a <code>PropertyChangeListener</code> from the listener list.
		''' This removes a <code>PropertyChangeListener</code> that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the <code>PropertyChangeListener</code> to be removed </param>
		''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
		Public Shared Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			SyncLock classLock
				lAFState.getPropertyChangeSupport(True).removePropertyChangeListener(listener)
			End SyncLock
		End Sub


		''' <summary>
		''' Returns an array of all the <code>PropertyChangeListener</code>s added
		''' to this UIManager with addPropertyChangeListener().
		''' </summary>
		''' <returns> all of the <code>PropertyChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Property Shared propertyChangeListeners As java.beans.PropertyChangeListener()
			Get
				SyncLock classLock
					Return lAFState.getPropertyChangeSupport(True).propertyChangeListeners
				End SyncLock
			End Get
		End Property

		Private Shared Function loadSwingProperties() As java.util.Properties
	'         Don't bother checking for Swing properties if untrusted, as
	'         * there's no way to look them up without triggering SecurityExceptions.
	'         
			If GetType(UIManager).classLoader IsNot Nothing Then
				Return New java.util.Properties
			Else
				Dim props As New java.util.Properties

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'			{
	'				public Object run()
	'				{
	'					OSInfo.OSType osType = AccessController.doPrivileged(OSInfo.getOSTypeAction());
	'					if (osType == OSInfo.OSType.MACOSX)
	'					{
	'						props.put(defaultLAFKey, getSystemLookAndFeelClassName());
	'					}
	'
	'					try
	'					{
	'						File file = New File(makeSwingPropertiesFilename());
	'
	'						if (file.exists())
	'						{
	'							' InputStream has been buffered in Properties
	'							' class
	'							FileInputStream ins = New FileInputStream(file);
	'							props.load(ins);
	'							ins.close();
	'						}
	'					}
	'					catch (Exception e)
	'					{
	'						' No such file, or file is otherwise non-readable.
	'					}
	'
	'					' Check whether any properties were overridden at the
	'					' command line.
	'					checkProperty(props, defaultLAFKey);
	'					checkProperty(props, auxiliaryLAFsKey);
	'					checkProperty(props, multiplexingLAFKey);
	'					checkProperty(props, installedLAFsKey);
	'					checkProperty(props, disableMnemonicKey);
	'					' Don't care about return value.
	'					Return Nothing;
	'				}
	'			});
				Return props
			End If
		End Function

		Private Shared Sub checkProperty(ByVal props As java.util.Properties, ByVal key As String)
			' No need to do catch the SecurityException here, this runs
			' in a doPrivileged.
			Dim value As String = System.getProperty(key)
			If value IsNot Nothing Then props.put(key, value)
		End Sub


		''' <summary>
		''' If a <code>swing.properties</code> file exist and it has a
		''' <code>swing.installedlafs</code> property
		''' then initialize the <code>installedLAFs</code> field.
		''' </summary>
		''' <seealso cref= #getInstalledLookAndFeels </seealso>
		Private Shared Sub initializeInstalledLAFs(ByVal swingProps As java.util.Properties)
			Dim ilafsString As String = swingProps.getProperty(installedLAFsKey)
			If ilafsString Is Nothing Then Return

	'         Create a vector that contains the value of the swing.installedlafs
	'         * property.  For example given "swing.installedlafs=motif,windows"
	'         * lafs = {"motif", "windows"}.
	'         
			Dim lafs As New List(Of String)
			Dim st As New java.util.StringTokenizer(ilafsString, ",", False)
			Do While st.hasMoreTokens()
				lafs.Add(st.nextToken())
			Loop

	'         Look up the name and class for each name in the "swing.installedlafs"
	'         * list.  If they both exist then add a LookAndFeelInfo to
	'         * the installedLafs array.
	'         
			Dim ilafs As New List(Of LookAndFeelInfo)(lafs.Count)
			For Each laf As String In lafs
				Dim name As String = swingProps.getProperty(makeInstalledLAFKey(laf, "name"), laf)
				Dim cls As String = swingProps.getProperty(makeInstalledLAFKey(laf, "class"))
				If cls IsNot Nothing Then ilafs.Add(New LookAndFeelInfo(name, cls))
			Next laf

			Dim installedLAFs As LookAndFeelInfo() = New LookAndFeelInfo(ilafs.Count - 1){}
			For i As Integer = 0 To ilafs.Count - 1
				installedLAFs(i) = ilafs(i)
			Next i
			lAFState.installedLAFs = installedLAFs
		End Sub


		''' <summary>
		''' If the user has specified a default look and feel, use that.
		''' Otherwise use the look and feel that's native to this platform.
		''' If this code is called after the application has explicitly
		''' set it's look and feel, do nothing.
		''' </summary>
		''' <seealso cref= #maybeInitialize </seealso>
		Private Shared Sub initializeDefaultLAF(ByVal swingProps As java.util.Properties)
			If lAFState.___lookAndFeel IsNot Nothing Then Return

			' Try to get default LAF from system property, then from AppContext
			' (6653395), then use cross-platform one by default.
			Dim lafName As String = Nothing
			Dim lafData As Hashtable = CType(sun.awt.AppContext.appContext.remove("swing.lafdata"), Hashtable)
			If lafData IsNot Nothing Then lafName = CStr(lafData.Remove("defaultlaf"))
			If lafName Is Nothing Then lafName = crossPlatformLookAndFeelClassName
			lafName = swingProps.getProperty(defaultLAFKey, lafName)

			Try
				lookAndFeel = lafName
			Catch e As Exception
				Throw New Exception("Cannot load " & lafName)
			End Try

			' Set any properties passed through AppContext (6653395).
			If lafData IsNot Nothing Then
				For Each key As Object In lafData.Keys
					UIManager.put(key, lafData(key))
				Next key
			End If
		End Sub


		Private Shared Sub initializeAuxiliaryLAFs(ByVal swingProps As java.util.Properties)
			Dim auxLookAndFeelNames As String = swingProps.getProperty(auxiliaryLAFsKey)
			If auxLookAndFeelNames Is Nothing Then Return

			Dim auxLookAndFeels As New List(Of LookAndFeel)

			Dim p As New java.util.StringTokenizer(auxLookAndFeelNames,",")
			Dim factoryName As String

	'         Try to load each LookAndFeel subclass in the list.
	'         

			Do While p.hasMoreTokens()
				Dim className As String = p.nextToken()
				Try
					Dim lnfClass As Type = SwingUtilities.loadSystemClass(className)
					Dim newLAF As LookAndFeel = CType(lnfClass.newInstance(), LookAndFeel)
					newLAF.initialize()
					auxLookAndFeels.Add(newLAF)
				Catch e As Exception
					Console.Error.WriteLine("UIManager: failed loading auxiliary look and feel " & className)
				End Try
			Loop

	'         If there were problems and no auxiliary look and feels were
	'         * loaded, make sure we reset auxLookAndFeels to null.
	'         * Otherwise, we are going to use the MultiLookAndFeel to get
	'         * all component UI's, so we need to load it now.
	'         
			If auxLookAndFeels.Count = 0 Then
				auxLookAndFeels = Nothing
			Else
				lAFState.multiLookAndFeel = multiLookAndFeel
				If lAFState.multiLookAndFeel Is Nothing Then auxLookAndFeels = Nothing
			End If

			lAFState.auxLookAndFeels = auxLookAndFeels
		End Sub


		Private Shared Sub initializeSystemDefaults(ByVal swingProps As java.util.Properties)
			lAFState.swingProps = swingProps
		End Sub


	'    
	'     * This method is called before any code that depends on the
	'     * <code>AppContext</code> specific LAFState object runs.  When the AppContext
	'     * corresponds to a set of applets it's possible for this method
	'     * to be re-entered, which is why we grab a lock before calling
	'     * initialize().
	'     
		Private Shared Sub maybeInitialize()
			SyncLock classLock
				If Not lAFState.initialized Then
					lAFState.initialized = True
					initialize()
				End If
			End SyncLock
		End Sub

	'    
	'     * Sets default swing focus traversal policy.
	'     
		Private Shared Sub maybeInitializeFocusPolicy(ByVal comp As JComponent)
			' Check for JRootPane which indicates that a swing toplevel
			' is coming, in which case a swing default focus policy
			' should be instatiated. See 7125044.
			If TypeOf comp Is JRootPane Then
				SyncLock classLock
					If Not lAFState.focusPolicyInitialized Then
						lAFState.focusPolicyInitialized = True

						If FocusManager.focusManagerEnabled Then java.awt.KeyboardFocusManager.currentKeyboardFocusManager.defaultFocusTraversalPolicy = New LayoutFocusTraversalPolicy
					End If
				End SyncLock
			End If
		End Sub

	'    
	'     * Only called by maybeInitialize().
	'     
		Private Shared Sub initialize()
			Dim swingProps As java.util.Properties = loadSwingProperties()
			initializeSystemDefaults(swingProps)
			initializeDefaultLAF(swingProps)
			initializeAuxiliaryLAFs(swingProps)
			initializeInstalledLAFs(swingProps)

			' Install Swing's PaintEventDispatcher
			If RepaintManager.HANDLE_TOP_LEVEL_PAINT Then sun.awt.PaintEventDispatcher.paintEventDispatcher = New SwingPaintEventDispatcher
			' Install a hook that will be invoked if no one consumes the
			' KeyEvent.  If the source isn't a JComponent this will process
			' key bindings, if the source is a JComponent it implies that
			' processKeyEvent was already invoked and thus no need to process
			' the bindings again, unless the Component is disabled, in which
			' case KeyEvents will no longer be dispatched to it so that we
			' handle it here.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.awt.KeyboardFocusManager.getCurrentKeyboardFocusManager().addKeyEventPostProcessor(New java.awt.KeyEventPostProcessor()
	'		{
	'					public boolean postProcessKeyEvent(KeyEvent e)
	'					{
	'						Component c = e.getComponent();
	'
	'						if ((!(c instanceof JComponent) || (c != Nothing && !c.isEnabled())) && JComponent.KeyboardState.shouldProcess(e) && SwingUtilities.processKeyBindings(e))
	'						{
	'							e.consume();
	'							Return True;
	'						}
	'						Return False;
	'					}
	'				});
			sun.awt.AWTAccessor.componentAccessor.requestFocusController = JComponent.focusController
		End Sub
	End Class

End Namespace
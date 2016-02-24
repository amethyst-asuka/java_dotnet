Imports System
Imports javax.accessibility

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
Namespace java.applet

	''' <summary>
	''' An applet is a small program that is intended not to be run on
	''' its own, but rather to be embedded inside another application.
	''' <p>
	''' The <code>Applet</code> class must be the superclass of any
	''' applet that is to be embedded in a Web page or viewed by the Java
	''' Applet Viewer. The <code>Applet</code> class provides a standard
	''' interface between applets and their environment.
	''' 
	''' @author      Arthur van Hoff
	''' @author      Chris Warth
	''' @since       JDK1.0
	''' </summary>
	Public Class Applet
		Inherits Panel

		''' <summary>
		''' Constructs a new Applet.
		''' <p>
		''' Note: Many methods in <code>java.applet.Applet</code>
		''' may be invoked by the applet only after the applet is
		''' fully constructed; applet should avoid calling methods
		''' in <code>java.applet.Applet</code> in the constructor.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since 1.4 </seealso>
		Public Sub New()
			If GraphicsEnvironment.headless Then Throw New HeadlessException
		End Sub

		''' <summary>
		''' Applets can be serialized but the following conventions MUST be followed:
		''' 
		''' Before Serialization:
		''' An applet must be in STOPPED state.
		''' 
		''' After Deserialization:
		''' The applet will be restored in STOPPED state (and most clients will
		''' likely move it into RUNNING state).
		''' The stub field will be restored by the reader.
		''' </summary>
		<NonSerialized> _
		Private stub As AppletStub

		' version ID for serialized form. 
		Private Const serialVersionUID As Long = -5836846270535785031L

		''' <summary>
		''' Read an applet from an object input stream. </summary>
		''' <exception cref="HeadlessException"> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns
		''' <code>true</code>
		''' @serial </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since 1.4 </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			If GraphicsEnvironment.headless Then Throw New HeadlessException
			s.defaultReadObject()
		End Sub

		''' <summary>
		''' Sets this applet's stub. This is done automatically by the system.
		''' <p>If there is a security manager, its <code> checkPermission </code>
		''' method is called with the
		''' <code>AWTPermission("setAppletStub")</code>
		''' permission if a stub has already been set. </summary>
		''' <param name="stub">   the new stub. </param>
		''' <exception cref="SecurityException"> if the caller cannot set the stub </exception>
		Public Property stub As AppletStub
			Set(ByVal stub As AppletStub)
				If Me.stub IsNot Nothing Then
					Dim s As SecurityManager = System.securityManager
					If s IsNot Nothing Then s.checkPermission(New AWTPermission("setAppletStub"))
				End If
				Me.stub = stub
			End Set
		End Property

		''' <summary>
		''' Determines if this applet is active. An applet is marked active
		''' just before its <code>start</code> method is called. It becomes
		''' inactive just before its <code>stop</code> method is called.
		''' </summary>
		''' <returns>  <code>true</code> if the applet is active;
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.applet.Applet#start() </seealso>
		''' <seealso cref=     java.applet.Applet#stop() </seealso>
		Public Overridable Property active As Boolean
			Get
				If stub IsNot Nothing Then
					Return stub.active ' If stub field not filled in, applet never active
				Else
					Return False
				End If
			End Get
		End Property

		''' <summary>
		''' Gets the URL of the document in which this applet is embedded.
		''' For example, suppose an applet is contained
		''' within the document:
		''' <blockquote><pre>
		'''    http://www.oracle.com/technetwork/java/index.html
		''' </pre></blockquote>
		''' The document base is:
		''' <blockquote><pre>
		'''    http://www.oracle.com/technetwork/java/index.html
		''' </pre></blockquote>
		''' </summary>
		''' <returns>  the <seealso cref="java.net.URL"/> of the document that contains this
		'''          applet. </returns>
		''' <seealso cref=     java.applet.Applet#getCodeBase() </seealso>
		Public Overridable Property documentBase As java.net.URL
			Get
				Return stub.documentBase
			End Get
		End Property

		''' <summary>
		''' Gets the base URL. This is the URL of the directory which contains this applet.
		''' </summary>
		''' <returns>  the base <seealso cref="java.net.URL"/> of
		'''          the directory which contains this applet. </returns>
		''' <seealso cref=     java.applet.Applet#getDocumentBase() </seealso>
		Public Overridable Property codeBase As java.net.URL
			Get
				Return stub.codeBase
			End Get
		End Property

		''' <summary>
		''' Returns the value of the named parameter in the HTML tag. For
		''' example, if this applet is specified as
		''' <blockquote><pre>
		''' &lt;applet code="Clock" width=50 height=50&gt;
		''' &lt;param name=Color value="blue"&gt;
		''' &lt;/applet&gt;
		''' </pre></blockquote>
		''' <p>
		''' then a call to <code>getParameter("Color")</code> returns the
		''' value <code>"blue"</code>.
		''' <p>
		''' The <code>name</code> argument is case insensitive.
		''' </summary>
		''' <param name="name">   a parameter name. </param>
		''' <returns>  the value of the named parameter,
		'''          or <code>null</code> if not set. </returns>
		 Public Overridable Function getParameter(ByVal name As String) As String
			 Return stub.getParameter(name)
		 End Function

		''' <summary>
		''' Determines this applet's context, which allows the applet to
		''' query and affect the environment in which it runs.
		''' <p>
		''' This environment of an applet represents the document that
		''' contains the applet.
		''' </summary>
		''' <returns>  the applet's context. </returns>
		Public Overridable Property appletContext As AppletContext
			Get
				Return stub.appletContext
			End Get
		End Property

		''' <summary>
		''' Requests that this applet be resized.
		''' </summary>
		''' <param name="width">    the new requested width for the applet. </param>
		''' <param name="height">   the new requested height for the applet. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Sub resize(ByVal width As Integer, ByVal height As Integer)
			Dim d As Dimension = size()
			If (d.width <> width) OrElse (d.height <> height) Then
				MyBase.resize(width, height)
				If stub IsNot Nothing Then stub.appletResize(width, height)
			End If
		End Sub

		''' <summary>
		''' Requests that this applet be resized.
		''' </summary>
		''' <param name="d">   an object giving the new width and height. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Sub resize(ByVal d As Dimension)
			resize(d.width, d.height)
		End Sub

		''' <summary>
		''' Indicates if this container is a validate root.
		''' <p>
		''' {@code Applet} objects are the validate roots, and, therefore, they
		''' override this method to return {@code true}.
		''' </summary>
		''' <returns> {@code true}
		''' @since 1.7 </returns>
		''' <seealso cref= java.awt.Container#isValidateRoot </seealso>
		Public Property Overrides validateRoot As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Requests that the argument string be displayed in the
		''' "status window". Many browsers and applet viewers
		''' provide such a window, where the application can inform users of
		''' its current state.
		''' </summary>
		''' <param name="msg">   a string to display in the status window. </param>
		Public Overridable Sub showStatus(ByVal msg As String)
			appletContext.showStatus(msg)
		End Sub

		''' <summary>
		''' Returns an <code>Image</code> object that can then be painted on
		''' the screen. The <code>url</code> that is passed as an argument
		''' must specify an absolute URL.
		''' <p>
		''' This method always returns immediately, whether or not the image
		''' exists. When this applet attempts to draw the image on the screen,
		''' the data will be loaded. The graphics primitives that draw the
		''' image will incrementally paint on the screen.
		''' </summary>
		''' <param name="url">   an absolute URL giving the location of the image. </param>
		''' <returns>  the image at the specified URL. </returns>
		''' <seealso cref=     java.awt.Image </seealso>
		Public Overridable Function getImage(ByVal url As java.net.URL) As Image
			Return appletContext.getImage(url)
		End Function

		''' <summary>
		''' Returns an <code>Image</code> object that can then be painted on
		''' the screen. The <code>url</code> argument must specify an absolute
		''' URL. The <code>name</code> argument is a specifier that is
		''' relative to the <code>url</code> argument.
		''' <p>
		''' This method always returns immediately, whether or not the image
		''' exists. When this applet attempts to draw the image on the screen,
		''' the data will be loaded. The graphics primitives that draw the
		''' image will incrementally paint on the screen.
		''' </summary>
		''' <param name="url">    an absolute URL giving the base location of the image. </param>
		''' <param name="name">   the location of the image, relative to the
		'''                 <code>url</code> argument. </param>
		''' <returns>  the image at the specified URL. </returns>
		''' <seealso cref=     java.awt.Image </seealso>
		Public Overridable Function getImage(ByVal url As java.net.URL, ByVal name As String) As Image
			Try
				Return getImage(New java.net.URL(url, name))
			Catch e As java.net.MalformedURLException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Get an audio clip from the given URL.
		''' </summary>
		''' <param name="url"> points to the audio clip </param>
		''' <returns> the audio clip at the specified URL.
		''' 
		''' @since       1.2 </returns>
		Public Shared Function newAudioClip(ByVal url As java.net.URL) As AudioClip
			Return New sun.applet.AppletAudioClip(url)
		End Function

		''' <summary>
		''' Returns the <code>AudioClip</code> object specified by the
		''' <code>URL</code> argument.
		''' <p>
		''' This method always returns immediately, whether or not the audio
		''' clip exists. When this applet attempts to play the audio clip, the
		''' data will be loaded.
		''' </summary>
		''' <param name="url">  an absolute URL giving the location of the audio clip. </param>
		''' <returns>  the audio clip at the specified URL. </returns>
		''' <seealso cref=     java.applet.AudioClip </seealso>
		Public Overridable Function getAudioClip(ByVal url As java.net.URL) As AudioClip
			Return appletContext.getAudioClip(url)
		End Function

		''' <summary>
		''' Returns the <code>AudioClip</code> object specified by the
		''' <code>URL</code> and <code>name</code> arguments.
		''' <p>
		''' This method always returns immediately, whether or not the audio
		''' clip exists. When this applet attempts to play the audio clip, the
		''' data will be loaded.
		''' </summary>
		''' <param name="url">    an absolute URL giving the base location of the
		'''                 audio clip. </param>
		''' <param name="name">   the location of the audio clip, relative to the
		'''                 <code>url</code> argument. </param>
		''' <returns>  the audio clip at the specified URL. </returns>
		''' <seealso cref=     java.applet.AudioClip </seealso>
		Public Overridable Function getAudioClip(ByVal url As java.net.URL, ByVal name As String) As AudioClip
			Try
				Return getAudioClip(New java.net.URL(url, name))
			Catch e As java.net.MalformedURLException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Returns information about this applet. An applet should override
		''' this method to return a <code>String</code> containing information
		''' about the author, version, and copyright of the applet.
		''' <p>
		''' The implementation of this method provided by the
		''' <code>Applet</code> class returns <code>null</code>.
		''' </summary>
		''' <returns>  a string containing information about the author, version, and
		'''          copyright of the applet. </returns>
		Public Overridable Property appletInfo As String
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the locale of the applet. It allows the applet
		''' to maintain its own locale separated from the locale
		''' of the browser or appletviewer.
		''' </summary>
		''' <returns>  the locale of the applet; if no locale has
		'''          been set, the default locale is returned.
		''' @since   JDK1.1 </returns>
		Public Property Overrides locale As java.util.Locale
			Get
			  Dim locale_Renamed As java.util.Locale = MyBase.locale
			  If locale_Renamed Is Nothing Then Return java.util.Locale.default
			  Return locale_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns information about the parameters that are understood by
		''' this applet. An applet should override this method to return an
		''' array of <code>Strings</code> describing these parameters.
		''' <p>
		''' Each element of the array should be a set of three
		''' <code>Strings</code> containing the name, the type, and a
		''' description. For example:
		''' <blockquote><pre>
		''' String pinfo[][] = {
		'''   {"fps",    "1-10",    "frames per second"},
		'''   {"repeat", "boolean", "repeat image loop"},
		'''   {"imgs",   "url",     "images directory"}
		''' };
		''' </pre></blockquote>
		''' <p>
		''' The implementation of this method provided by the
		''' <code>Applet</code> class returns <code>null</code>.
		''' </summary>
		''' <returns>  an array describing the parameters this applet looks for. </returns>
		Public Overridable Property parameterInfo As String()()
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Plays the audio clip at the specified absolute URL. Nothing
		''' happens if the audio clip cannot be found.
		''' </summary>
		''' <param name="url">   an absolute URL giving the location of the audio clip. </param>
		Public Overridable Sub play(ByVal url As java.net.URL)
			Dim clip As AudioClip = getAudioClip(url)
			If clip IsNot Nothing Then clip.play()
		End Sub

		''' <summary>
		''' Plays the audio clip given the URL and a specifier that is
		''' relative to it. Nothing happens if the audio clip cannot be found.
		''' </summary>
		''' <param name="url">    an absolute URL giving the base location of the
		'''                 audio clip. </param>
		''' <param name="name">   the location of the audio clip, relative to the
		'''                 <code>url</code> argument. </param>
		Public Overridable Sub play(ByVal url As java.net.URL, ByVal name As String)
			Dim clip As AudioClip = getAudioClip(url, name)
			If clip IsNot Nothing Then clip.play()
		End Sub

		''' <summary>
		''' Called by the browser or applet viewer to inform
		''' this applet that it has been loaded into the system. It is always
		''' called before the first time that the <code>start</code> method is
		''' called.
		''' <p>
		''' A subclass of <code>Applet</code> should override this method if
		''' it has initialization to perform. For example, an applet with
		''' threads would use the <code>init</code> method to create the
		''' threads and the <code>destroy</code> method to kill them.
		''' <p>
		''' The implementation of this method provided by the
		''' <code>Applet</code> class does nothing.
		''' </summary>
		''' <seealso cref=     java.applet.Applet#destroy() </seealso>
		''' <seealso cref=     java.applet.Applet#start() </seealso>
		''' <seealso cref=     java.applet.Applet#stop() </seealso>
		Public Overridable Sub init()
		End Sub

		''' <summary>
		''' Called by the browser or applet viewer to inform
		''' this applet that it should start its execution. It is called after
		''' the <code>init</code> method and each time the applet is revisited
		''' in a Web page.
		''' <p>
		''' A subclass of <code>Applet</code> should override this method if
		''' it has any operation that it wants to perform each time the Web
		''' page containing it is visited. For example, an applet with
		''' animation might want to use the <code>start</code> method to
		''' resume animation, and the <code>stop</code> method to suspend the
		''' animation.
		''' <p>
		''' Note: some methods, such as <code>getLocationOnScreen</code>, can only
		''' provide meaningful results if the applet is showing.  Because
		''' <code>isShowing</code> returns <code>false</code> when the applet's
		''' <code>start</code> is first called, methods requiring
		''' <code>isShowing</code> to return <code>true</code> should be called from
		''' a <code>ComponentListener</code>.
		''' <p>
		''' The implementation of this method provided by the
		''' <code>Applet</code> class does nothing.
		''' </summary>
		''' <seealso cref=     java.applet.Applet#destroy() </seealso>
		''' <seealso cref=     java.applet.Applet#init() </seealso>
		''' <seealso cref=     java.applet.Applet#stop() </seealso>
		''' <seealso cref=     java.awt.Component#isShowing() </seealso>
		''' <seealso cref=     java.awt.event.ComponentListener#componentShown(java.awt.event.ComponentEvent) </seealso>
		Public Overridable Sub start()
		End Sub

		''' <summary>
		''' Called by the browser or applet viewer to inform
		''' this applet that it should stop its execution. It is called when
		''' the Web page that contains this applet has been replaced by
		''' another page, and also just before the applet is to be destroyed.
		''' <p>
		''' A subclass of <code>Applet</code> should override this method if
		''' it has any operation that it wants to perform each time the Web
		''' page containing it is no longer visible. For example, an applet
		''' with animation might want to use the <code>start</code> method to
		''' resume animation, and the <code>stop</code> method to suspend the
		''' animation.
		''' <p>
		''' The implementation of this method provided by the
		''' <code>Applet</code> class does nothing.
		''' </summary>
		''' <seealso cref=     java.applet.Applet#destroy() </seealso>
		''' <seealso cref=     java.applet.Applet#init() </seealso>
		Public Overridable Sub [stop]()
		End Sub

		''' <summary>
		''' Called by the browser or applet viewer to inform
		''' this applet that it is being reclaimed and that it should destroy
		''' any resources that it has allocated. The <code>stop</code> method
		''' will always be called before <code>destroy</code>.
		''' <p>
		''' A subclass of <code>Applet</code> should override this method if
		''' it has any operation that it wants to perform before it is
		''' destroyed. For example, an applet with threads would use the
		''' <code>init</code> method to create the threads and the
		''' <code>destroy</code> method to kill them.
		''' <p>
		''' The implementation of this method provided by the
		''' <code>Applet</code> class does nothing.
		''' </summary>
		''' <seealso cref=     java.applet.Applet#init() </seealso>
		''' <seealso cref=     java.applet.Applet#start() </seealso>
		''' <seealso cref=     java.applet.Applet#stop() </seealso>
		Public Overridable Sub destroy()
		End Sub

		'
		' Accessibility support
		'

		Friend Shadows accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this Applet.
		''' For applets, the AccessibleContext takes the form of an
		''' AccessibleApplet.
		''' A new AccessibleApplet instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleApplet that serves as the
		'''         AccessibleContext of this Applet
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleApplet(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Applet</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to applet user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleApplet
			Inherits AccessibleAWTPanel

			Private ReadOnly outerInstance As Applet

			Public Sub New(ByVal outerInstance As Applet)
				Me.outerInstance = outerInstance
			End Sub


			Private Const serialVersionUID As Long = 8127374778187708896L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.FRAME
				End Get
			End Property

			''' <summary>
			''' Get the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					states.add(AccessibleState.ACTIVE)
					Return states
				End Get
			End Property

		End Class
	End Class

End Namespace
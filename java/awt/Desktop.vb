Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The {@code Desktop} class allows a Java application to launch
	''' associated applications registered on the native desktop to handle
	''' a <seealso cref="java.net.URI"/> or a file.
	''' 
	''' <p> Supported operations include:
	''' <ul>
	'''   <li>launching the user-default browser to show a specified
	'''       URI;</li>
	'''   <li>launching the user-default mail client with an optional
	'''       {@code mailto} URI;</li>
	'''   <li>launching a registered application to open, edit or print a
	'''       specified file.</li>
	''' </ul>
	''' 
	''' <p> This class provides methods corresponding to these
	''' operations. The methods look for the associated application
	''' registered on the current platform, and launch it to handle a URI
	''' or file. If there is no associated application or the associated
	''' application fails to be launched, an exception is thrown.
	''' 
	''' <p> An application is registered to a URI or file type; for
	''' example, the {@code "sxi"} file extension is typically registered
	''' to StarOffice.  The mechanism of registering, accessing, and
	''' launching the associated application is platform-dependent.
	''' 
	''' <p> Each operation is an action type represented by the {@link
	''' Desktop.Action} class.
	''' 
	''' <p> Note: when some action is invoked and the associated
	''' application is executed, it will be executed on the same system as
	''' the one on which the Java application was launched.
	''' 
	''' @since 1.6
	''' @author Armin Chen
	''' @author George Zhang
	''' </summary>
	Public Class Desktop

		''' <summary>
		''' Represents an action type.  Each platform supports a different
		''' set of actions.  You may use the <seealso cref="Desktop#isSupported"/>
		''' method to determine if the given action is supported by the
		''' current platform. </summary>
		''' <seealso cref= java.awt.Desktop#isSupported(java.awt.Desktop.Action)
		''' @since 1.6 </seealso>
		Public Enum Action
			''' <summary>
			''' Represents an "open" action. </summary>
			''' <seealso cref= Desktop#open(java.io.File) </seealso>
			OPEN
			''' <summary>
			''' Represents an "edit" action. </summary>
			''' <seealso cref= Desktop#edit(java.io.File) </seealso>
			EDIT
			''' <summary>
			''' Represents a "print" action. </summary>
			''' <seealso cref= Desktop#print(java.io.File) </seealso>
			PRINT
			''' <summary>
			''' Represents a "mail" action. </summary>
			''' <seealso cref= Desktop#mail() </seealso>
			''' <seealso cref= Desktop#mail(java.net.URI) </seealso>
			MAIL
			''' <summary>
			''' Represents a "browse" action. </summary>
			''' <seealso cref= Desktop#browse(java.net.URI) </seealso>
			BROWSE
		End Enum

		Private peer As java.awt.peer.DesktopPeer

		''' <summary>
		''' Suppresses default constructor for noninstantiability.
		''' </summary>
		Private Sub New()
			peer = Toolkit.defaultToolkit.createDesktopPeer(Me)
		End Sub

        ''' <summary>
        ''' Returns the <code>Desktop</code> instance of the current
        ''' browser context.  On some platforms the Desktop API may not be
        ''' supported; use the <seealso cref="#isDesktopSupported"/> method to
        ''' determine if the current desktop is supported. </summary>
        ''' <returns> the Desktop instance of the current browser context </returns>
        ''' <exception cref="HeadlessException"> if {@link
        ''' GraphicsEnvironment#isHeadless()} returns {@code true} </exception>
        ''' <exception cref="UnsupportedOperationException"> if this class is not
        ''' supported on the current platform </exception>
        ''' <seealso cref= #isDesktopSupported() </seealso>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Shared ReadOnly Property desktop As Desktop
            Get
                If java.awt.GraphicsEnvironment.headless Then Throw New java.awt.HeadlessException
                If Not Desktop.desktopSupported Then Throw New UnsupportedOperationException("Desktop API is not " & "supported on the current platform")

                Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
                Dim desktop_Renamed As Desktop = CType(context.get(GetType(Desktop)), Desktop)

                If desktop_Renamed Is Nothing Then
                    desktop_Renamed = New Desktop
                    context.put(GetType(Desktop), desktop_Renamed)
                End If

                Return desktop_Renamed
            End Get
        End Property

        ''' <summary>
        ''' Tests whether this class is supported on the current platform.
        ''' If it's supported, use <seealso cref="#getDesktop()"/> to retrieve an
        ''' instance.
        ''' </summary>
        ''' <returns> <code>true</code> if this class is supported on the
        '''         current platform; <code>false</code> otherwise </returns>
        ''' <seealso cref= #getDesktop() </seealso>
        Public Shared ReadOnly Property desktopSupported As Boolean
            Get
                Dim defaultToolkit As Toolkit = Toolkit.defaultToolkit
                If TypeOf defaultToolkit Is sun.awt.SunToolkit Then Return CType(defaultToolkit, sun.awt.SunToolkit).desktopSupported
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Tests whether an action is supported on the current platform.
        ''' 
        ''' <p>Even when the platform supports an action, a file or URI may
        ''' not have a registered application for the action.  For example,
        ''' most of the platforms support the <seealso cref="Desktop.Action#OPEN"/>
        ''' action.  But for a specific file, there may not be an
        ''' application registered to open it.  In this case, {@link
        ''' #isSupported} may return {@code true}, but the corresponding
        ''' action method will throw an <seealso cref="IOException"/>.
        ''' </summary>
        ''' <param name="action"> the specified <seealso cref="Action"/> </param>
        ''' <returns> <code>true</code> if the specified action is supported on
        '''         the current platform; <code>false</code> otherwise </returns>
        ''' <seealso cref= Desktop.Action </seealso>
        Public Overridable Function isSupported(ByVal action As Action) As Boolean
			Return peer.isSupported(action)
		End Function

		''' <summary>
		''' Checks if the file is a valid file and readable.
		''' </summary>
		''' <exception cref="SecurityException"> If a security manager exists and its
		'''         <seealso cref="SecurityManager#checkRead(java.lang.String)"/> method
		'''         denies read access to the file </exception>
		''' <exception cref="NullPointerException"> if file is null </exception>
		''' <exception cref="IllegalArgumentException"> if file doesn't exist </exception>
		Private Shared Sub checkFileValidation(ByVal file As java.io.File)
			If file Is Nothing Then Throw New NullPointerException("File must not be null")

			If Not file.exists() Then Throw New IllegalArgumentException("The file: " & file.path & " doesn't exist.")

			file.canRead()
		End Sub

		''' <summary>
		''' Checks if the action type is supported.
		''' </summary>
		''' <param name="actionType"> the action type in question </param>
		''' <exception cref="UnsupportedOperationException"> if the specified action type is not
		'''         supported on the current platform </exception>
		Private Sub checkActionSupport(ByVal actionType As Action)
			If Not isSupported(actionType) Then Throw New UnsupportedOperationException("The " & actionType.name() & " action is not supported on the current platform!")
		End Sub


		''' <summary>
		'''  Calls to the security manager's <code>checkPermission</code> method with
		'''  an <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		'''  permission.
		''' </summary>
		Private Sub checkAWTPermission()
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New java.awt.AWTPermission("showWindowWithoutWarningBanner"))
		End Sub

		''' <summary>
		''' Launches the associated application to open the file.
		''' 
		''' <p> If the specified file is a directory, the file manager of
		''' the current platform is launched to open it.
		''' </summary>
		''' <param name="file"> the file to be opened with the associated application </param>
		''' <exception cref="NullPointerException"> if {@code file} is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if the specified file doesn't
		''' exist </exception>
		''' <exception cref="UnsupportedOperationException"> if the current platform
		''' does not support the <seealso cref="Desktop.Action#OPEN"/> action </exception>
		''' <exception cref="IOException"> if the specified file has no associated
		''' application or the associated application fails to be launched </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		''' <seealso cref="java.lang.SecurityManager#checkRead(java.lang.String)"/>
		''' method denies read access to the file, or it denies the
		''' <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		''' permission, or the calling thread is not allowed to create a
		''' subprocess </exception>
		''' <seealso cref= java.awt.AWTPermission </seealso>
		Public Overridable Sub open(ByVal file As java.io.File)
			checkAWTPermission()
			checkExec()
			checkActionSupport(Action.OPEN)
			checkFileValidation(file)

			peer.open(file)
		End Sub

		''' <summary>
		''' Launches the associated editor application and opens a file for
		''' editing.
		''' </summary>
		''' <param name="file"> the file to be opened for editing </param>
		''' <exception cref="NullPointerException"> if the specified file is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if the specified file doesn't
		''' exist </exception>
		''' <exception cref="UnsupportedOperationException"> if the current platform
		''' does not support the <seealso cref="Desktop.Action#EDIT"/> action </exception>
		''' <exception cref="IOException"> if the specified file has no associated
		''' editor, or the associated application fails to be launched </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		''' <seealso cref="java.lang.SecurityManager#checkRead(java.lang.String)"/>
		''' method denies read access to the file, or {@link
		''' java.lang.SecurityManager#checkWrite(java.lang.String)} method
		''' denies write access to the file, or it denies the
		''' <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		''' permission, or the calling thread is not allowed to create a
		''' subprocess </exception>
		''' <seealso cref= java.awt.AWTPermission </seealso>
		Public Overridable Sub edit(ByVal file As java.io.File)
			checkAWTPermission()
			checkExec()
			checkActionSupport(Action.EDIT)
			file.canWrite()
			checkFileValidation(file)

			peer.edit(file)
		End Sub

		''' <summary>
		''' Prints a file with the native desktop printing facility, using
		''' the associated application's print command.
		''' </summary>
		''' <param name="file"> the file to be printed </param>
		''' <exception cref="NullPointerException"> if the specified file is {@code
		''' null} </exception>
		''' <exception cref="IllegalArgumentException"> if the specified file doesn't
		''' exist </exception>
		''' <exception cref="UnsupportedOperationException"> if the current platform
		'''         does not support the <seealso cref="Desktop.Action#PRINT"/> action </exception>
		''' <exception cref="IOException"> if the specified file has no associated
		''' application that can be used to print it </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		''' <seealso cref="java.lang.SecurityManager#checkRead(java.lang.String)"/>
		''' method denies read access to the file, or its {@link
		''' java.lang.SecurityManager#checkPrintJobAccess()} method denies
		''' the permission to print the file, or the calling thread is not
		''' allowed to create a subprocess </exception>
		Public Overridable Sub print(ByVal file As java.io.File)
			checkExec()
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPrintJobAccess()
			checkActionSupport(Action.PRINT)
			checkFileValidation(file)

			peer.print(file)
		End Sub

		''' <summary>
		''' Launches the default browser to display a {@code URI}.
		''' If the default browser is not able to handle the specified
		''' {@code URI}, the application registered for handling
		''' {@code URIs} of the specified type is invoked. The application
		''' is determined from the protocol and path of the {@code URI}, as
		''' defined by the {@code URI} class.
		''' <p>
		''' If the calling thread does not have the necessary permissions,
		''' and this is invoked from within an applet,
		''' {@code AppletContext.showDocument()} is used. Similarly, if the calling
		''' does not have the necessary permissions, and this is invoked from within
		''' a Java Web Started application, {@code BasicService.showDocument()}
		''' is used.
		''' </summary>
		''' <param name="uri"> the URI to be displayed in the user default browser </param>
		''' <exception cref="NullPointerException"> if {@code uri} is {@code null} </exception>
		''' <exception cref="UnsupportedOperationException"> if the current platform
		''' does not support the <seealso cref="Desktop.Action#BROWSE"/> action </exception>
		''' <exception cref="IOException"> if the user default browser is not found,
		''' or it fails to be launched, or the default handler application
		''' failed to be launched </exception>
		''' <exception cref="SecurityException"> if a security manager exists and it
		''' denies the
		''' <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		''' permission, or the calling thread is not allowed to create a
		''' subprocess; and not invoked from within an applet or Java Web Started
		''' application </exception>
		''' <exception cref="IllegalArgumentException"> if the necessary permissions
		''' are not available and the URI can not be converted to a {@code URL} </exception>
		''' <seealso cref= java.net.URI </seealso>
		''' <seealso cref= java.awt.AWTPermission </seealso>
		''' <seealso cref= java.applet.AppletContext </seealso>
		Public Overridable Sub browse(ByVal uri As java.net.URI)
			Dim securityException As SecurityException = Nothing
			Try
				checkAWTPermission()
				checkExec()
			Catch e As SecurityException
				securityException = e
			End Try
			checkActionSupport(Action.BROWSE)
			If uri Is Nothing Then Throw New NullPointerException
			If securityException Is Nothing Then
				peer.browse(uri)
				Return
			End If

			' Calling thread doesn't have necessary priviledges.
			' Delegate to DesktopBrowse so that it can work in
			' applet/webstart.
			Dim url As java.net.URL = Nothing
			Try
				url = uri.toURL()
			Catch e As java.net.MalformedURLException
				Throw New IllegalArgumentException("Unable to convert URI to URL", e)
			End Try
			Dim db As sun.awt.DesktopBrowse = sun.awt.DesktopBrowse.instance
			If db Is Nothing Then Throw securityException
			db.browse(url)
		End Sub

		''' <summary>
		''' Launches the mail composing window of the user default mail
		''' client.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the current platform
		''' does not support the <seealso cref="Desktop.Action#MAIL"/> action </exception>
		''' <exception cref="IOException"> if the user default mail client is not
		''' found, or it fails to be launched </exception>
		''' <exception cref="SecurityException"> if a security manager exists and it
		''' denies the
		''' <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		''' permission, or the calling thread is not allowed to create a
		''' subprocess </exception>
		''' <seealso cref= java.awt.AWTPermission </seealso>
		Public Overridable Sub mail()
			checkAWTPermission()
			checkExec()
			checkActionSupport(Action.MAIL)
			Dim mailtoURI As java.net.URI = Nothing
			Try
				mailtoURI = New java.net.URI("mailto:?")
				peer.mail(mailtoURI)
			Catch e As java.net.URISyntaxException
				' won't reach here.
			End Try
		End Sub

		''' <summary>
		''' Launches the mail composing window of the user default mail
		''' client, filling the message fields specified by a {@code
		''' mailto:} URI.
		''' 
		''' <p> A <code>mailto:</code> URI can specify message fields
		''' including <i>"to"</i>, <i>"cc"</i>, <i>"subject"</i>,
		''' <i>"body"</i>, etc.  See <a
		''' href="http://www.ietf.org/rfc/rfc2368.txt">The mailto URL
		''' scheme (RFC 2368)</a> for the {@code mailto:} URI specification
		''' details.
		''' </summary>
		''' <param name="mailtoURI"> the specified {@code mailto:} URI </param>
		''' <exception cref="NullPointerException"> if the specified URI is {@code
		''' null} </exception>
		''' <exception cref="IllegalArgumentException"> if the URI scheme is not
		'''         <code>"mailto"</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if the current platform
		''' does not support the <seealso cref="Desktop.Action#MAIL"/> action </exception>
		''' <exception cref="IOException"> if the user default mail client is not
		''' found or fails to be launched </exception>
		''' <exception cref="SecurityException"> if a security manager exists and it
		''' denies the
		''' <code>AWTPermission("showWindowWithoutWarningBanner")</code>
		''' permission, or the calling thread is not allowed to create a
		''' subprocess </exception>
		''' <seealso cref= java.net.URI </seealso>
		''' <seealso cref= java.awt.AWTPermission </seealso>
		Public Overridable Sub mail(ByVal mailtoURI As java.net.URI)
			checkAWTPermission()
			checkExec()
			checkActionSupport(Action.MAIL)
			If mailtoURI Is Nothing Then Throw New NullPointerException

			If Not "mailto".equalsIgnoreCase(mailtoURI.scheme) Then Throw New IllegalArgumentException("URI scheme is not ""mailto""")

			peer.mail(mailtoURI)
		End Sub

		Private Sub checkExec()
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New java.io.FilePermission("<<ALL FILES>>", sun.security.util.SecurityConstants.FILE_EXECUTE_ACTION))
		End Sub
	End Class

End Namespace
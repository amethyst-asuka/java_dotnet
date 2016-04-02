Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The splash screen can be displayed at application startup, before the
	''' Java Virtual Machine (JVM) starts. The splash screen is displayed as an
	''' undecorated window containing an image. You can use GIF, JPEG, or PNG files
	''' for the image. Animation is supported for the GIF format, while transparency
	''' is supported both for GIF and PNG.  The window is positioned at the center
	''' of the screen. The position on multi-monitor systems is not specified. It is
	''' platform and implementation dependent.  The splash screen window is closed
	''' automatically as soon as the first window is displayed by Swing/AWT (may be
	''' also closed manually using the Java API, see below).
	''' <P>
	''' If your application is packaged in a jar file, you can use the
	''' "SplashScreen-Image" option in a manifest file to show a splash screen.
	''' Place the image in the jar archive and specify the path in the option.
	''' The path should not have a leading slash.
	''' <BR>
	''' For example, in the <code>manifest.mf</code> file:
	''' <PRE>
	''' Manifest-Version: 1.0
	''' Main-Class: Test
	''' SplashScreen-Image: filename.gif
	''' </PRE>
	''' <P>
	''' If the Java implementation provides the command-line interface and you run
	''' your application by using the command line or a shortcut, use the Java
	''' application launcher option to show a splash screen. The Oracle reference
	''' implementation allows you to specify the splash screen image location with
	''' the {@code -splash:} option.
	''' <BR>
	''' For example:
	''' <PRE>
	''' java -splash:filename.gif Test
	''' </PRE>
	''' The command line interface has higher precedence over the manifest
	''' setting.
	''' <p>
	''' The splash screen will be displayed as faithfully as possible to present the
	''' whole splash screen image given the limitations of the target platform and
	''' display.
	''' <p>
	''' It is implied that the specified image is presented on the screen "as is",
	''' i.e. preserving the exact color values as specified in the image file. Under
	''' certain circumstances, though, the presented image may differ, e.g. when
	''' applying color dithering to present a 32 bits per pixel (bpp) image on a 16
	''' or 8 bpp screen. The native platform display configuration may also affect
	''' the colors of the displayed image (e.g.  color profiles, etc.)
	''' <p>
	''' The {@code SplashScreen} class provides the API for controlling the splash
	''' screen. This class may be used to close the splash screen, change the splash
	''' screen image, get the splash screen native window position/size, and paint
	''' in the splash screen. It cannot be used to create the splash screen. You
	''' should use the options provided by the Java implementation for that.
	''' <p>
	''' This class cannot be instantiated. Only a single instance of this class
	''' can exist, and it may be obtained by using the <seealso cref="#getSplashScreen()"/>
	''' static method. In case the splash screen has not been created at
	''' application startup via the command line or manifest file option,
	''' the <code>getSplashScreen</code> method returns <code>null</code>.
	''' 
	''' @author Oleg Semenov
	''' @since 1.6
	''' </summary>
	Public NotInheritable Class SplashScreen

		Friend Sub New(ByVal ptr As Long) ' non-public constructor
			splashPtr = ptr
		End Sub

		''' <summary>
		''' Returns the {@code SplashScreen} object used for
		''' Java startup splash screen control on systems that support display.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the splash screen feature is not
		'''         supported by the current toolkit </exception>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''         returns true </exception>
		''' <returns> the <seealso cref="SplashScreen"/> instance, or <code>null</code> if there is
		'''         none or it has already been closed </returns>
		PublicShared ReadOnly PropertysplashScreen As SplashScreen
			Get
				SyncLock GetType(SplashScreen)
					If GraphicsEnvironment.headless Then Throw New HeadlessException
					' SplashScreen class is now a singleton
					If (Not wasClosed) AndAlso theInstance Is Nothing Then
						java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
						Dim ptr As Long = _getInstance()
						If ptr <> 0 AndAlso _isVisible(ptr) Then theInstance = New SplashScreen(ptr)
					End If
					Return theInstance
				End SyncLock
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Void
'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
'				System.loadLibrary("splashscreen")
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Changes the splash screen image. The new image is loaded from the
		''' specified URL; GIF, JPEG and PNG image formats are supported.
		''' The method returns after the image has finished loading and the window
		''' has been updated.
		''' The splash screen window is resized according to the size of
		''' the image and is centered on the screen.
		''' </summary>
		''' <param name="imageURL"> the non-<code>null</code> URL for the new
		'''        splash screen image </param>
		''' <exception cref="NullPointerException"> if {@code imageURL} is <code>null</code> </exception>
		''' <exception cref="IOException"> if there was an error while loading the image </exception>
		''' <exception cref="IllegalStateException"> if the splash screen has already been
		'''         closed </exception>
		Public Property imageURL As java.net.URL
			Set(ByVal imageURL As java.net.URL)
				checkVisible()
				Dim connection As java.net.URLConnection = imageURL.openConnection()
				connection.connect()
				Dim length As Integer = connection.contentLength
				Dim stream As java.io.InputStream = connection.inputStream
				Dim buf As SByte() = New SByte(length - 1){}
				Dim [off] As Integer = 0
				Do
					' check for available data
					Dim available As Integer = stream.available()
					If available <= 0 Then available = 1
					' check for enough room in buffer, realloc if needed
					' the buffer always grows in size 2x minimum
					If [off] + available > length Then
						length = [off]*2
						If [off] + available > length Then length = available+[off]
						Dim oldBuf As SByte() = buf
						buf = New SByte(length - 1){}
						Array.Copy(oldBuf, 0, buf, 0, [off])
					End If
					' now read the data
					Dim result As Integer = stream.read(buf, [off], available)
					If result < 0 Then Exit Do
					[off] += result
				Loop
				SyncLock GetType(SplashScreen)
					checkVisible()
					If Not _setImageData(splashPtr, buf) Then Throw New java.io.IOException("Bad image format or i/o error when loading image")
					Me.imageURL = imageURL
				End SyncLock
			End Set
			Get
				SyncLock GetType(SplashScreen)
					checkVisible()
					If imageURL Is Nothing Then
						Try
							Dim fileName As String = _getImageFileName(splashPtr)
							Dim jarName As String = _getImageJarName(splashPtr)
							If fileName IsNot Nothing Then
								If jarName IsNot Nothing Then
									imageURL = New java.net.URL("jar:" & ((New File(jarName)).toURL().ToString()) & "!/" & fileName)
								Else
									imageURL = (New File(fileName)).toURL()
								End If
							End If
						Catch e As java.net.MalformedURLException
							If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("MalformedURLException caught in the getImageURL() method", e)
						End Try
					End If
					Return imageURL
				End SyncLock
			End Get
		End Property

		Private Sub checkVisible()
			If Not visible Then Throw New IllegalStateException("no splash screen available")
		End Sub

		''' <summary>
		''' Returns the bounds of the splash screen window as a <seealso cref="Rectangle"/>.
		''' This may be useful if, for example, you want to replace the splash
		''' screen with your window at the same location.
		''' <p>
		''' You cannot control the size or position of the splash screen.
		''' The splash screen size is adjusted automatically when the image changes.
		''' <p>
		''' The image may contain transparent areas, and thus the reported bounds may
		''' be larger than the visible splash screen image on the screen.
		''' </summary>
		''' <returns> a {@code Rectangle} containing the splash screen bounds </returns>
		''' <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
		Public Property bounds As Rectangle
			Get
				SyncLock GetType(SplashScreen)
					checkVisible()
					Dim scale As Single = _getScaleFactor(splashPtr)
					Dim bounds_Renamed As Rectangle = _getBounds(splashPtr)
					Debug.Assert(scale > 0)
					If scale > 0 AndAlso scale <> 1 Then bounds_Renamed.sizeize(CInt(Fix(bounds_Renamed.width / scale)), CInt(Fix(bounds_Renamed.width / scale)))
					Return bounds_Renamed
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the size of the splash screen window as a <seealso cref="Dimension"/>.
		''' This may be useful if, for example,
		''' you want to draw on the splash screen overlay surface.
		''' <p>
		''' You cannot control the size or position of the splash screen.
		''' The splash screen size is adjusted automatically when the image changes.
		''' <p>
		''' The image may contain transparent areas, and thus the reported size may
		''' be larger than the visible splash screen image on the screen.
		''' </summary>
		''' <returns> a <seealso cref="Dimension"/> object indicating the splash screen size </returns>
		''' <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
		Public Property size As Dimension
			Get
				Return bounds.size
			End Get
		End Property

		''' <summary>
		''' Creates a graphics context (as a <seealso cref="Graphics2D"/> object) for the splash
		''' screen overlay image, which allows you to draw over the splash screen.
		''' Note that you do not draw on the main image but on the image that is
		''' displayed over the main image using alpha blending. Also note that drawing
		''' on the overlay image does not necessarily update the contents of splash
		''' screen window. You should call {@code update()} on the
		''' <code>SplashScreen</code> when you want the splash screen to be
		''' updated immediately.
		''' <p>
		''' The pixel (0, 0) in the coordinate space of the graphics context
		''' corresponds to the origin of the splash screen native window bounds (see
		''' <seealso cref="#getBounds()"/>).
		''' </summary>
		''' <returns> graphics context for the splash screen overlay surface </returns>
		''' <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
		Public Function createGraphics() As Graphics2D
			SyncLock GetType(SplashScreen)
				checkVisible()
				If image_Renamed Is Nothing Then
					' get unscaled splash image size
					Dim [dim] As Dimension = _getBounds(splashPtr).size
					image_Renamed = New BufferedImage([dim].width, [dim].height, BufferedImage.TYPE_INT_ARGB)
				End If
				Dim scale As Single = _getScaleFactor(splashPtr)
				Dim g As Graphics2D = image_Renamed.createGraphics()
				assert(scale > 0)
				If scale <= 0 Then scale = 1
				g.scale(scale, scale)
				Return g
			End SyncLock
		End Function

		''' <summary>
		''' Updates the splash window with current contents of the overlay image.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the overlay image does not exist;
		'''         for example, if {@code createGraphics} has never been called,
		'''         or if the splash screen has already been closed </exception>
		Public Sub update()
			Dim image_Renamed As BufferedImage
			SyncLock GetType(SplashScreen)
				checkVisible()
				image_Renamed = Me.image_Renamed
			End SyncLock
			If image_Renamed Is Nothing Then Throw New IllegalStateException("no overlay image available")
			Dim buf As DataBuffer = image_Renamed.raster.dataBuffer
			If Not(TypeOf buf Is DataBufferInt) Then Throw New AssertionError("Overlay image DataBuffer is of invalid type == " & buf.GetType().name)
			Dim numBanks As Integer = buf.numBanks
			If numBanks<>1 Then Throw New AssertionError("Invalid number of banks ==" & numBanks & " in overlay image DataBuffer")
			If Not(TypeOf image_Renamed.sampleModel Is SinglePixelPackedSampleModel) Then Throw New AssertionError("Overlay image has invalid sample model == " & image_Renamed.sampleModel.GetType().name)
			Dim sm As SinglePixelPackedSampleModel = CType(image_Renamed.sampleModel, SinglePixelPackedSampleModel)
			Dim scanlineStride As Integer = sm.scanlineStride
			Dim rect As Rectangle = image_Renamed.raster.bounds
			' Note that we steal the data array here, but just for reading
			' so we do not need to mark the DataBuffer dirty...
			Dim data As Integer() = sun.awt.image.SunWritableRaster.stealData(CType(buf, DataBufferInt), 0)
			SyncLock GetType(SplashScreen)
				checkVisible()
				_update(splashPtr, data, rect.x, rect.y, rect.width, rect.height, scanlineStride)
			End SyncLock
		End Sub

		''' <summary>
		''' Hides the splash screen, closes the window, and releases all associated
		''' resources.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the splash screen has already been closed </exception>
		Public Sub close()
			SyncLock GetType(SplashScreen)
				checkVisible()
				_close(splashPtr)
				image_Renamed = Nothing
				SplashScreen.markClosed()
			End SyncLock
		End Sub

		Friend Shared Sub markClosed()
			SyncLock GetType(SplashScreen)
				wasClosed = True
				theInstance = Nothing
			End SyncLock
		End Sub


		''' <summary>
		''' Determines whether the splash screen is visible. The splash screen may
		''' be hidden using <seealso cref="#close()"/>, it is also hidden automatically when
		''' the first AWT/Swing window is made visible.
		''' <p>
		''' Note that the native platform may delay presenting the splash screen
		''' native window on the screen. The return value of {@code true} for this
		''' method only guarantees that the conditions to hide the splash screen
		''' window have not occurred yet.
		''' </summary>
		''' <returns> true if the splash screen is visible (has not been closed yet),
		'''         false otherwise </returns>
		Public Property visible As Boolean
			Get
				SyncLock GetType(SplashScreen)
					Return (Not wasClosed) AndAlso _isVisible(splashPtr)
				End SyncLock
			End Get
		End Property

		Private image_Renamed As BufferedImage ' overlay image

		Private ReadOnly splashPtr As Long ' pointer to native Splash structure
		Private Shared wasClosed As Boolean = False

		Private imageURL As java.net.URL

		''' <summary>
		''' The instance reference for the singleton.
		''' (<code>null</code> if no instance exists yet.)
		''' </summary>
		''' <seealso cref= #getSplashScreen </seealso>
		''' <seealso cref= #close </seealso>
		Private Shared theInstance As SplashScreen = Nothing

		Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.SplashScreen")

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub _update(ByVal splashPtr As Long, ByVal data As Integer(), ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal scanlineStride As Integer)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _isVisible(ByVal splashPtr As Long) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _getBounds(ByVal splashPtr As Long) As Rectangle
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _getInstance() As Long
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub _close(ByVal splashPtr As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _getImageFileName(ByVal splashPtr As Long) As String
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _getImageJarName(ByVal SplashPtr As Long) As String
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _setImageData(ByVal SplashPtr As Long, ByVal data As SByte()) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function _getScaleFactor(ByVal SplashPtr As Long) As Single
		End Function

	End Class

End Namespace
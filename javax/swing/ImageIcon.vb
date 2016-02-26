Imports System
Imports System.Runtime.CompilerServices
Imports System.Text
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
	''' An implementation of the Icon interface that paints Icons
	''' from Images. Images that are created from a URL, filename or byte array
	''' are preloaded using MediaTracker to monitor the loaded state
	''' of the image.
	''' 
	''' <p>
	''' For further information and examples of using image icons, see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/icon.html">How to Use Icons</a>
	''' in <em>The Java Tutorial.</em>
	''' 
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
	''' @author Jeff Dinkins
	''' @author Lynn Monsanto
	''' </summary>
	<Serializable> _
	Public Class ImageIcon
		Implements Icon, Accessible

	'     Keep references to the filename and location so that
	'     * alternate persistence schemes have the option to archive
	'     * images symbolically rather than including the image data
	'     * in the archive.
	'     
		<NonSerialized> _
		Private filename As String
		<NonSerialized> _
		Private location As java.net.URL

		<NonSerialized> _
		Friend image As Image
		<NonSerialized> _
		Friend loadStatus As Integer = 0
		Friend imageObserver As ImageObserver
		Friend description As String = Nothing

		''' <summary>
		''' Do not use this shared component, which is used to track image loading.
		''' It is left for backward compatibility only. </summary>
		''' @deprecated since 1.8 
		<Obsolete("since 1.8")> _
		Protected Friend Shared ReadOnly component As Component

		''' <summary>
		''' Do not use this shared media tracker, which is used to load images.
		''' It is left for backward compatibility only. </summary>
		''' @deprecated since 1.8 
		<Obsolete("since 1.8")> _
		Protected Friend Shared ReadOnly tracker As MediaTracker

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			component = AccessController.doPrivileged(New PrivilegedAction<Component>()
	'		{
	'			public Component run()
	'			{
	'				try
	'				{
	'					final Component component = createNoPermsComponent();
	'
	'					' 6482575 - clear the appContext field so as not to leak it
	'					Field appContextField = Component.class.getDeclaredField("appContext");
	'					appContextField.setAccessible(True);
	'					appContextField.set(component, Nothing);
	'
	'					Return component;
	'				}
	'				catch (Throwable e)
	'				{
	'					' We don't care about component.
	'					' So don't prevent class initialisation.
	'					e.printStackTrace();
	'					Return Nothing;
	'				}
	'			}
	'		});
			tracker = New MediaTracker(component)
		End Sub

		Private Shared Function createNoPermsComponent() As Component
			' 7020198 - set acc field to no permissions and no subject
			' Note, will have appContext set.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return AccessController.doPrivileged(New PrivilegedAction<Component>()
	'		{
	'					public Component run()
	'					{
	'						Return New Component()
	'						{
	'						};
	'					}
	'				},
					New AccessControlContext(New ProtectionDomain(){ New ProtectionDomain(Nothing, Nothing)
					})
		   )
		End Function

		''' <summary>
		''' Id used in loading images from MediaTracker.
		''' </summary>
		Private Shared mediaTrackerID As Integer

		Private Shared ReadOnly TRACKER_KEY As Object = New StringBuilder("TRACKER_KEY")

		Friend width As Integer = -1
		Friend height As Integer = -1

		''' <summary>
		''' Creates an ImageIcon from the specified file. The image will
		''' be preloaded by using MediaTracker to monitor the loading state
		''' of the image. </summary>
		''' <param name="filename"> the name of the file containing the image </param>
		''' <param name="description"> a brief textual description of the image </param>
		''' <seealso cref= #ImageIcon(String) </seealso>
		Public Sub New(ByVal filename As String, ByVal description As String)
			image = Toolkit.defaultToolkit.getImage(filename)
			If image Is Nothing Then Return
			Me.filename = filename
			Me.description = description
			loadImage(image)
		End Sub

		''' <summary>
		''' Creates an ImageIcon from the specified file. The image will
		''' be preloaded by using MediaTracker to monitor the loading state
		''' of the image. The specified String can be a file name or a
		''' file path. When specifying a path, use the Internet-standard
		''' forward-slash ("/") as a separator.
		''' (The string is converted to an URL, so the forward-slash works
		''' on all systems.)
		''' For example, specify:
		''' <pre>
		'''    new ImageIcon("images/myImage.gif") </pre>
		''' The description is initialized to the <code>filename</code> string.
		''' </summary>
		''' <param name="filename"> a String specifying a filename or path </param>
		''' <seealso cref= #getDescription </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal filename As String)
			Me.New(filename, filename)
		End Sub

		''' <summary>
		''' Creates an ImageIcon from the specified URL. The image will
		''' be preloaded by using MediaTracker to monitor the loaded state
		''' of the image. </summary>
		''' <param name="location"> the URL for the image </param>
		''' <param name="description"> a brief textual description of the image </param>
		''' <seealso cref= #ImageIcon(String) </seealso>
		Public Sub New(ByVal location As java.net.URL, ByVal description As String)
			image = Toolkit.defaultToolkit.getImage(location)
			If image Is Nothing Then Return
			Me.location = location
			Me.description = description
			loadImage(image)
		End Sub

		''' <summary>
		''' Creates an ImageIcon from the specified URL. The image will
		''' be preloaded by using MediaTracker to monitor the loaded state
		''' of the image.
		''' The icon's description is initialized to be
		''' a string representation of the URL. </summary>
		''' <param name="location"> the URL for the image </param>
		''' <seealso cref= #getDescription </seealso>
		Public Sub New(ByVal location As java.net.URL)
			Me.New(location, location.toExternalForm())
		End Sub

		''' <summary>
		''' Creates an ImageIcon from the image. </summary>
		''' <param name="image"> the image </param>
		''' <param name="description"> a brief textual description of the image </param>
		Public Sub New(ByVal image As Image, ByVal description As String)
			Me.New(image)
			Me.description = description
		End Sub

		''' <summary>
		''' Creates an ImageIcon from an image object.
		''' If the image has a "comment" property that is a string,
		''' then the string is used as the description of this icon. </summary>
		''' <param name="image"> the image </param>
		''' <seealso cref= #getDescription </seealso>
		''' <seealso cref= java.awt.Image#getProperty </seealso>
		Public Sub New(ByVal image As Image)
			Me.image = image
			Dim o As Object = image.getProperty("comment", imageObserver)
			If TypeOf o Is String Then description = CStr(o)
			loadImage(image)
		End Sub

		''' <summary>
		''' Creates an ImageIcon from an array of bytes which were
		''' read from an image file containing a supported image format,
		''' such as GIF, JPEG, or (as of 1.3) PNG.
		''' Normally this array is created
		''' by reading an image using Class.getResourceAsStream(), but
		''' the byte array may also be statically stored in a class.
		''' </summary>
		''' <param name="imageData"> an array of pixels in an image format supported
		'''         by the AWT Toolkit, such as GIF, JPEG, or (as of 1.3) PNG </param>
		''' <param name="description"> a brief textual description of the image </param>
		''' <seealso cref=    java.awt.Toolkit#createImage </seealso>
		Public Sub New(ByVal imageData As SByte(), ByVal description As String)
			Me.image = Toolkit.defaultToolkit.createImage(imageData)
			If image Is Nothing Then Return
			Me.description = description
			loadImage(image)
		End Sub

		''' <summary>
		''' Creates an ImageIcon from an array of bytes which were
		''' read from an image file containing a supported image format,
		''' such as GIF, JPEG, or (as of 1.3) PNG.
		''' Normally this array is created
		''' by reading an image using Class.getResourceAsStream(), but
		''' the byte array may also be statically stored in a class.
		''' If the resulting image has a "comment" property that is a string,
		''' then the string is used as the description of this icon.
		''' </summary>
		''' <param name="imageData"> an array of pixels in an image format supported by
		'''             the AWT Toolkit, such as GIF, JPEG, or (as of 1.3) PNG </param>
		''' <seealso cref=    java.awt.Toolkit#createImage </seealso>
		''' <seealso cref= #getDescription </seealso>
		''' <seealso cref= java.awt.Image#getProperty </seealso>
		Public Sub New(ByVal imageData As SByte())
			Me.image = Toolkit.defaultToolkit.createImage(imageData)
			If image Is Nothing Then Return
			Dim o As Object = image.getProperty("comment", imageObserver)
			If TypeOf o Is String Then description = CStr(o)
			loadImage(image)
		End Sub

		''' <summary>
		''' Creates an uninitialized image icon.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Loads the image, returning only when the image is loaded. </summary>
		''' <param name="image"> the image </param>
		Protected Friend Overridable Sub loadImage(ByVal image As Image)
			Dim mTracker As MediaTracker = tracker
			SyncLock mTracker
				Dim id As Integer = nextID

				mTracker.addImage(image, id)
				Try
					mTracker.waitForID(id, 0)
				Catch e As InterruptedException
					Console.WriteLine("INTERRUPTED while loading Image")
				End Try
				loadStatus = mTracker.statusID(id, False)
				mTracker.removeImage(image, id)

				width = image.getWidth(imageObserver)
				height = image.getHeight(imageObserver)
			End SyncLock
		End Sub

		''' <summary>
		''' Returns an ID to use with the MediaTracker in loading an image.
		''' </summary>
		Private Property nextID As Integer
			Get
				SyncLock tracker
						mediaTrackerID += 1
						Return mediaTrackerID
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the MediaTracker for the current AppContext, creating a new
		''' MediaTracker if necessary.
		''' </summary>
		Private Property tracker As MediaTracker
			Get
				Dim trackerObj As Object
				Dim ac As sun.awt.AppContext = sun.awt.AppContext.appContext
				' Opt: Only synchronize if trackerObj comes back null?
				' If null, synchronize, re-check for null, and put new tracker
				SyncLock ac
					trackerObj = ac.get(TRACKER_KEY)
					If trackerObj Is Nothing Then
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'					Component comp = New Component()
		'				{
		'				};
						trackerObj = New MediaTracker(comp)
						ac.put(TRACKER_KEY, trackerObj)
					End If
				End SyncLock
				Return CType(trackerObj, MediaTracker)
			End Get
		End Property

		''' <summary>
		''' Returns the status of the image loading operation. </summary>
		''' <returns> the loading status as defined by java.awt.MediaTracker </returns>
		''' <seealso cref= java.awt.MediaTracker#ABORTED </seealso>
		''' <seealso cref= java.awt.MediaTracker#ERRORED </seealso>
		''' <seealso cref= java.awt.MediaTracker#COMPLETE </seealso>
		Public Overridable Property imageLoadStatus As Integer
			Get
				Return loadStatus
			End Get
		End Property

		''' <summary>
		''' Returns this icon's <code>Image</code>. </summary>
		''' <returns> the <code>Image</code> object for this <code>ImageIcon</code> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property image As Image
			Get
				Return image
			End Get
			Set(ByVal image As Image)
				Me.image = image
				loadImage(image)
			End Set
		End Property


		''' <summary>
		''' Gets the description of the image.  This is meant to be a brief
		''' textual description of the object.  For example, it might be
		''' presented to a blind user to give an indication of the purpose
		''' of the image.
		''' The description may be null.
		''' </summary>
		''' <returns> a brief textual description of the image </returns>
		Public Overridable Property description As String
			Get
				Return description
			End Get
			Set(ByVal description As String)
				Me.description = description
			End Set
		End Property


		''' <summary>
		''' Paints the icon.
		''' The top-left corner of the icon is drawn at
		''' the point (<code>x</code>, <code>y</code>)
		''' in the coordinate space of the graphics context <code>g</code>.
		''' If this icon has no image observer,
		''' this method uses the <code>c</code> component
		''' as the observer.
		''' </summary>
		''' <param name="c"> the component to be used as the observer
		'''          if this icon has no image observer </param>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the X coordinate of the icon's top-left corner </param>
		''' <param name="y"> the Y coordinate of the icon's top-left corner </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			If imageObserver Is Nothing Then
			   g.drawImage(image, x, y, c)
			Else
			   g.drawImage(image, x, y, imageObserver)
			End If
		End Sub

		''' <summary>
		''' Gets the width of the icon.
		''' </summary>
		''' <returns> the width in pixels of this icon </returns>
		Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
			Get
				Return width
			End Get
		End Property

		''' <summary>
		''' Gets the height of the icon.
		''' </summary>
		''' <returns> the height in pixels of this icon </returns>
		Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
			Get
				Return height
			End Get
		End Property

		''' <summary>
		''' Sets the image observer for the image.  Set this
		''' property if the ImageIcon contains an animated GIF, so
		''' the observer is notified to update its display.
		''' For example:
		''' <pre>
		'''     icon = new ImageIcon(...)
		'''     button.setIcon(icon);
		'''     icon.setImageObserver(button);
		''' </pre>
		''' </summary>
		''' <param name="observer"> the image observer </param>
		Public Overridable Property imageObserver As ImageObserver
			Set(ByVal observer As ImageObserver)
				imageObserver = observer
			End Set
			Get
				Return imageObserver
			End Get
		End Property

		''' <summary>
		''' Returns the image observer for the image.
		''' </summary>
		''' <returns> the image observer, which may be null </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Returns a string representation of this image.
		''' </summary>
		''' <returns> a string representing this image </returns>
		Public Overrides Function ToString() As String
			If description IsNot Nothing Then Return description
			Return MyBase.ToString()
		End Function

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()

			Dim w As Integer = s.readInt()
			Dim h As Integer = s.readInt()
			Dim pixels As Integer() = CType(s.readObject(), Integer())

			If pixels IsNot Nothing Then
				Dim tk As Toolkit = Toolkit.defaultToolkit
				Dim cm As ColorModel = ColorModel.rGBdefault
				image = tk.createImage(New MemoryImageSource(w, h, cm, pixels, 0, w))
				loadImage(image)
			End If
		End Sub


		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			Dim w As Integer = iconWidth
			Dim h As Integer = iconHeight
			Dim pixels As Integer() = If(image IsNot Nothing, New Integer(w * h - 1){}, Nothing)

			If image IsNot Nothing Then
				Try
					Dim pg As New PixelGrabber(image, 0, 0, w, h, pixels, 0, w)
					pg.grabPixels()
					If (pg.status And ImageObserver.ABORT) <> 0 Then Throw New java.io.IOException("failed to load image contents")
				Catch e As InterruptedException
					Throw New java.io.IOException("image load interrupted")
				End Try
			End If

			s.writeInt(w)
			s.writeInt(h)
			s.writeObject(pixels)
		End Sub

		''' <summary>
		''' --- Accessibility Support ---
		''' </summary>

		Private accessibleContext As AccessibleImageIcon = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this ImageIcon.
		''' For image icons, the AccessibleContext takes the form of an
		''' AccessibleImageIcon.
		''' A new AccessibleImageIcon instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleImageIcon that serves as the
		'''         AccessibleContext of this ImageIcon
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this ImageIcon.
		''' @since 1.3 </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleImageIcon
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>ImageIcon</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to image icon user-interface
		''' elements.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' @since 1.3
		''' </summary>
		<Serializable> _
		Protected Friend Class AccessibleImageIcon
			Inherits AccessibleContext
			Implements AccessibleIcon

			Private ReadOnly outerInstance As ImageIcon

			Public Sub New(ByVal outerInstance As ImageIcon)
				Me.outerInstance = outerInstance
			End Sub


	'        
	'         * AccessibleContest implementation -----------------
	'         

			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Property Overrides accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.ICON
				End Get
			End Property

			''' <summary>
			''' Gets the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Property Overrides accessibleStateSet As AccessibleStateSet
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the Accessible parent of this object.  If the parent of this
			''' object implements Accessible, this method should simply return
			''' getParent().
			''' </summary>
			''' <returns> the Accessible parent of this object -- can be null if this
			''' object does not have an Accessible parent </returns>
			Public Property Overrides accessibleParent As Accessible
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the index of this object in its accessible parent.
			''' </summary>
			''' <returns> the index of this object in its parent; -1 if this
			''' object does not have an accessible parent. </returns>
			''' <seealso cref= #getAccessibleParent </seealso>
			Public Property Overrides accessibleIndexInParent As Integer
				Get
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement Accessible, than this
			''' method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Property Overrides accessibleChildrenCount As Integer
				Get
					Return 0
				End Get
			End Property

			''' <summary>
			''' Returns the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
				Return Nothing
			End Function

			''' <summary>
			''' Returns the locale of this object.
			''' </summary>
			''' <returns> the locale of this object </returns>
			Public Property Overrides locale As java.util.Locale
				Get
					Return Nothing
				End Get
			End Property

	'        
	'         * AccessibleIcon implementation -----------------
	'         

			''' <summary>
			''' Gets the description of the icon.  This is meant to be a brief
			''' textual description of the object.  For example, it might be
			''' presented to a blind user to give an indication of the purpose
			''' of the icon.
			''' </summary>
			''' <returns> the description of the icon </returns>
			Public Overridable Property accessibleIconDescription As String Implements AccessibleIcon.getAccessibleIconDescription
				Get
					Return outerInstance.description
				End Get
				Set(ByVal description As String)
					outerInstance.description = description
				End Set
			End Property


			''' <summary>
			''' Gets the height of the icon.
			''' </summary>
			''' <returns> the height of the icon </returns>
			Public Overridable Property accessibleIconHeight As Integer Implements AccessibleIcon.getAccessibleIconHeight
				Get
					Return outerInstance.height
				End Get
			End Property

			''' <summary>
			''' Gets the width of the icon.
			''' </summary>
			''' <returns> the width of the icon </returns>
			Public Overridable Property accessibleIconWidth As Integer Implements AccessibleIcon.getAccessibleIconWidth
				Get
					Return outerInstance.width
				End Get
			End Property

			Private Sub readObject(ByVal s As java.io.ObjectInputStream)
				s.defaultReadObject()
			End Sub

			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				s.defaultWriteObject()
			End Sub
		End Class ' AccessibleImageIcon
	End Class

End Namespace
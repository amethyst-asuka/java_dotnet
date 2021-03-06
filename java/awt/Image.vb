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
	''' The abstract class <code>Image</code> is the superclass of all
	''' classes that represent graphical images. The image must be
	''' obtained in a platform-specific manner.
	''' 
	''' @author      Sami Shaio
	''' @author      Arthur van Hoff
	''' @since       JDK1.0
	''' </summary>
	Public MustInherit Class Image

		''' <summary>
		''' convenience object; we can use this single static object for
		''' all images that do not create their own image caps; it holds the
		''' default (unaccelerated) properties.
		''' </summary>
		Private Shared defaultImageCaps As New ImageCapabilities(False)

		''' <summary>
		''' Priority for accelerating this image.  Subclasses are free to
		''' set different default priorities and applications are free to
		''' set the priority for specific images via the
		''' <code>setAccelerationPriority(float)</code> method.
		''' @since 1.5
		''' </summary>
		Protected Friend accelerationPriority As Single =.5f

		''' <summary>
		''' Determines the width of the image. If the width is not yet known,
		''' this method returns <code>-1</code> and the specified
		''' <code>ImageObserver</code> object is notified later. </summary>
		''' <param name="observer">   an object waiting for the image to be loaded. </param>
		''' <returns>    the width of this image, or <code>-1</code>
		'''                   if the width is not yet known. </returns>
		''' <seealso cref=       java.awt.Image#getHeight </seealso>
		''' <seealso cref=       java.awt.image.ImageObserver </seealso>
		Public MustOverride Function getWidth(  observer As java.awt.image.ImageObserver) As Integer

		''' <summary>
		''' Determines the height of the image. If the height is not yet known,
		''' this method returns <code>-1</code> and the specified
		''' <code>ImageObserver</code> object is notified later. </summary>
		''' <param name="observer">   an object waiting for the image to be loaded. </param>
		''' <returns>    the height of this image, or <code>-1</code>
		'''                   if the height is not yet known. </returns>
		''' <seealso cref=       java.awt.Image#getWidth </seealso>
		''' <seealso cref=       java.awt.image.ImageObserver </seealso>
		Public MustOverride Function getHeight(  observer As java.awt.image.ImageObserver) As Integer

		''' <summary>
		''' Gets the object that produces the pixels for the image.
		''' This method is called by the image filtering classes and by
		''' methods that perform image conversion and scaling. </summary>
		''' <returns>     the image producer that produces the pixels
		'''                                  for this image. </returns>
		''' <seealso cref=        java.awt.image.ImageProducer </seealso>
		Public MustOverride ReadOnly Property source As java.awt.image.ImageProducer

		''' <summary>
		''' Creates a graphics context for drawing to an off-screen image.
		''' This method can only be called for off-screen images. </summary>
		''' <returns>  a graphics context to draw to the off-screen image. </returns>
		''' <exception cref="UnsupportedOperationException"> if called for a
		'''            non-off-screen image. </exception>
		''' <seealso cref=     java.awt.Graphics </seealso>
		''' <seealso cref=     java.awt.Component#createImage(int, int) </seealso>
		Public MustOverride ReadOnly Property graphics As Graphics

		''' <summary>
		''' Gets a property of this image by name.
		''' <p>
		''' Individual property names are defined by the various image
		''' formats. If a property is not defined for a particular image, this
		''' method returns the <code>UndefinedProperty</code> object.
		''' <p>
		''' If the properties for this image are not yet known, this method
		''' returns <code>null</code>, and the <code>ImageObserver</code>
		''' object is notified later.
		''' <p>
		''' The property name <code>"comment"</code> should be used to store
		''' an optional comment which can be presented to the application as a
		''' description of the image, its source, or its author. </summary>
		''' <param name="name">   a property name. </param>
		''' <param name="observer">   an object waiting for this image to be loaded. </param>
		''' <returns>      the value of the named property. </returns>
		''' <exception cref="NullPointerException"> if the property name is null. </exception>
		''' <seealso cref=         java.awt.image.ImageObserver </seealso>
		''' <seealso cref=         java.awt.Image#UndefinedProperty </seealso>
		Public MustOverride Function getProperty(  name As String,   observer As java.awt.image.ImageObserver) As Object

		''' <summary>
		''' The <code>UndefinedProperty</code> object should be returned whenever a
		''' property which was not defined for a particular image is fetched.
		''' </summary>
		Public Shared ReadOnly UndefinedProperty As New Object

		''' <summary>
		''' Creates a scaled version of this image.
		''' A new <code>Image</code> object is returned which will render
		''' the image at the specified <code>width</code> and
		''' <code>height</code> by default.  The new <code>Image</code> object
		''' may be loaded asynchronously even if the original source image
		''' has already been loaded completely.
		''' 
		''' <p>
		''' 
		''' If either <code>width</code>
		''' or <code>height</code> is a negative number then a value is
		''' substituted to maintain the aspect ratio of the original image
		''' dimensions. If both <code>width</code> and <code>height</code>
		''' are negative, then the original image dimensions are used.
		''' </summary>
		''' <param name="width"> the width to which to scale the image. </param>
		''' <param name="height"> the height to which to scale the image. </param>
		''' <param name="hints"> flags to indicate the type of algorithm to use
		''' for image resampling. </param>
		''' <returns>     a scaled version of the image. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>width</code>
		'''             or <code>height</code> is zero. </exception>
		''' <seealso cref=        java.awt.Image#SCALE_DEFAULT </seealso>
		''' <seealso cref=        java.awt.Image#SCALE_FAST </seealso>
		''' <seealso cref=        java.awt.Image#SCALE_SMOOTH </seealso>
		''' <seealso cref=        java.awt.Image#SCALE_REPLICATE </seealso>
		''' <seealso cref=        java.awt.Image#SCALE_AREA_AVERAGING
		''' @since      JDK1.1 </seealso>
		Public Overridable Function getScaledInstance(  width As Integer,   height As Integer,   hints As Integer) As Image
			Dim filter As java.awt.image.ImageFilter
			If (hints And (SCALE_SMOOTH Or SCALE_AREA_AVERAGING)) <> 0 Then
				filter = New java.awt.image.AreaAveragingScaleFilter(width, height)
			Else
				filter = New java.awt.image.ReplicateScaleFilter(width, height)
			End If
			Dim prod As java.awt.image.ImageProducer
			prod = New java.awt.image.FilteredImageSource(source, filter)
			Return Toolkit.defaultToolkit.createImage(prod)
		End Function

		''' <summary>
		''' Use the default image-scaling algorithm.
		''' @since JDK1.1
		''' </summary>
		Public Const SCALE_DEFAULT As Integer = 1

		''' <summary>
		''' Choose an image-scaling algorithm that gives higher priority
		''' to scaling speed than smoothness of the scaled image.
		''' @since JDK1.1
		''' </summary>
		Public Const SCALE_FAST As Integer = 2

		''' <summary>
		''' Choose an image-scaling algorithm that gives higher priority
		''' to image smoothness than scaling speed.
		''' @since JDK1.1
		''' </summary>
		Public Const SCALE_SMOOTH As Integer = 4

		''' <summary>
		''' Use the image scaling algorithm embodied in the
		''' <code>ReplicateScaleFilter</code> class.
		''' The <code>Image</code> object is free to substitute a different filter
		''' that performs the same algorithm yet integrates more efficiently
		''' into the imaging infrastructure supplied by the toolkit. </summary>
		''' <seealso cref=        java.awt.image.ReplicateScaleFilter
		''' @since      JDK1.1 </seealso>
		Public Const SCALE_REPLICATE As Integer = 8

		''' <summary>
		''' Use the Area Averaging image scaling algorithm.  The
		''' image object is free to substitute a different filter that
		''' performs the same algorithm yet integrates more efficiently
		''' into the image infrastructure supplied by the toolkit. </summary>
		''' <seealso cref= java.awt.image.AreaAveragingScaleFilter
		''' @since JDK1.1 </seealso>
		Public Const SCALE_AREA_AVERAGING As Integer = 16

		''' <summary>
		''' Flushes all reconstructable resources being used by this Image object.
		''' This includes any pixel data that is being cached for rendering to
		''' the screen as well as any system resources that are being used
		''' to store data or pixels for the image if they can be recreated.
		''' The image is reset to a state similar to when it was first created
		''' so that if it is again rendered, the image data will have to be
		''' recreated or fetched again from its source.
		''' <p>
		''' Examples of how this method affects specific types of Image object:
		''' <ul>
		''' <li>
		''' BufferedImage objects leave the primary Raster which stores their
		''' pixels untouched, but flush any information cached about those
		''' pixels such as copies uploaded to the display hardware for
		''' accelerated blits.
		''' <li>
		''' Image objects created by the Component methods which take a
		''' width and height leave their primary buffer of pixels untouched,
		''' but have all cached information released much like is done for
		''' BufferedImage objects.
		''' <li>
		''' VolatileImage objects release all of their pixel resources
		''' including their primary copy which is typically stored on
		''' the display hardware where resources are scarce.
		''' These objects can later be restored using their
		''' <seealso cref="java.awt.image.VolatileImage#validate validate"/>
		''' method.
		''' <li>
		''' Image objects created by the Toolkit and Component classes which are
		''' loaded from files, URLs or produced by an <seealso cref="ImageProducer"/>
		''' are unloaded and all local resources are released.
		''' These objects can later be reloaded from their original source
		''' as needed when they are rendered, just as when they were first
		''' created.
		''' </ul>
		''' </summary>
		Public Overridable Sub flush()
			If surfaceManager IsNot Nothing Then surfaceManager.flush()
		End Sub

		''' <summary>
		''' Returns an ImageCapabilities object which can be
		''' inquired as to the capabilities of this
		''' Image on the specified GraphicsConfiguration.
		''' This allows programmers to find
		''' out more runtime information on the specific Image
		''' object that they have created.  For example, the user
		''' might create a BufferedImage but the system may have
		''' no video memory left for creating an image of that
		''' size on the given GraphicsConfiguration, so although the object
		''' may be acceleratable in general, it
		''' does not have that capability on this GraphicsConfiguration. </summary>
		''' <param name="gc"> a <code>GraphicsConfiguration</code> object.  A value of null
		''' for this parameter will result in getting the image capabilities
		''' for the default <code>GraphicsConfiguration</code>. </param>
		''' <returns> an <code>ImageCapabilities</code> object that contains
		''' the capabilities of this <code>Image</code> on the specified
		''' GraphicsConfiguration. </returns>
		''' <seealso cref= java.awt.image.VolatileImage#getCapabilities()
		''' VolatileImage.getCapabilities()
		''' @since 1.5 </seealso>
		Public Overridable Function getCapabilities(  gc As GraphicsConfiguration) As ImageCapabilities
			If surfaceManager IsNot Nothing Then Return surfaceManager.getCapabilities(gc)
			' Note: this is just a default object that gets returned in the
			' absence of any more specific information from a surfaceManager.
			' Subclasses of Image should either override this method or
			' make sure that they always have a non-null SurfaceManager
			' to return an ImageCapabilities object that is appropriate
			' for their given subclass type.
			Return defaultImageCaps
		End Function

		''' <summary>
		''' Sets a hint for this image about how important acceleration is.
		''' This priority hint is used to compare to the priorities of other
		''' Image objects when determining how to use scarce acceleration
		''' resources such as video memory.  When and if it is possible to
		''' accelerate this Image, if there are not enough resources available
		''' to provide that acceleration but enough can be freed up by
		''' de-accelerating some other image of lower priority, then that other
		''' Image may be de-accelerated in deference to this one.  Images
		''' that have the same priority take up resources on a first-come,
		''' first-served basis. </summary>
		''' <param name="priority"> a value between 0 and 1, inclusive, where higher
		''' values indicate more importance for acceleration.  A value of 0
		''' means that this Image should never be accelerated.  Other values
		''' are used simply to determine acceleration priority relative to other
		''' Images. </param>
		''' <exception cref="IllegalArgumentException"> if <code>priority</code> is less
		''' than zero or greater than 1.
		''' @since 1.5 </exception>
		Public Overridable Property accelerationPriority As Single
			Set(  priority As Single)
				If priority < 0 OrElse priority > 1 Then Throw New IllegalArgumentException("Priority must be a value " & "between 0 and 1, inclusive")
				accelerationPriority = priority
				If surfaceManager IsNot Nothing Then surfaceManager.accelerationPriority = accelerationPriority
			End Set
			Get
				Return accelerationPriority
			End Get
		End Property


		Friend surfaceManager As sun.awt.image.SurfaceManager

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.image.SurfaceManager.setImageAccessor(New sun.awt.image.SurfaceManager.ImageAccessor()
	'		{
	'			public SurfaceManager getSurfaceManager(Image img)
	'			{
	'				Return img.surfaceManager;
	'			}
	'			public  Sub  setSurfaceManager(Image img, SurfaceManager mgr)
	'			{
	'				img.surfaceManager = mgr;
	'			}
	'		});
		End Sub
	End Class

End Namespace
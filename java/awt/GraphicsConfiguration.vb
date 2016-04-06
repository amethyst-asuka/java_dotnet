Imports System.Diagnostics

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
	''' The <code>GraphicsConfiguration</code> class describes the
	''' characteristics of a graphics destination such as a printer or monitor.
	''' There can be many <code>GraphicsConfiguration</code> objects associated
	''' with a single graphics device, representing different drawing modes or
	''' capabilities.  The corresponding native structure will vary from platform
	''' to platform.  For example, on X11 windowing systems,
	''' each visual is a different <code>GraphicsConfiguration</code>.
	''' On Microsoft Windows, <code>GraphicsConfiguration</code>s represent
	''' PixelFormats available in the current resolution and color depth.
	''' <p>
	''' In a virtual device multi-screen environment in which the desktop
	''' area could span multiple physical screen devices, the bounds of the
	''' <code>GraphicsConfiguration</code> objects are relative to the
	''' virtual coordinate system.  When setting the location of a
	''' component, use <seealso cref="#getBounds() getBounds"/> to get the bounds of
	''' the desired <code>GraphicsConfiguration</code> and offset the location
	''' with the coordinates of the <code>GraphicsConfiguration</code>,
	''' as the following code sample illustrates:
	''' </p>
	''' 
	''' <pre>
	'''      Frame f = new Frame(gc);  // where gc is a GraphicsConfiguration
	'''      Rectangle bounds = gc.getBounds();
	'''      f.setLocation(10 + bounds.x, 10 + bounds.y); </pre>
	''' 
	''' <p>
	''' To determine if your environment is a virtual device
	''' environment, call <code>getBounds</code> on all of the
	''' <code>GraphicsConfiguration</code> objects in your system.  If
	''' any of the origins of the returned bounds is not (0,&nbsp;0),
	''' your environment is a virtual device environment.
	''' 
	''' <p>
	''' You can also use <code>getBounds</code> to determine the bounds
	''' of the virtual device.  To do this, first call <code>getBounds</code> on all
	''' of the <code>GraphicsConfiguration</code> objects in your
	''' system.  Then calculate the union of all of the bounds returned
	''' from the calls to <code>getBounds</code>.  The union is the
	''' bounds of the virtual device.  The following code sample
	''' calculates the bounds of the virtual device.
	''' 
	''' <pre>{@code
	'''      Rectangle virtualBounds = new Rectangle();
	'''      GraphicsEnvironment ge = GraphicsEnvironment.
	'''              getLocalGraphicsEnvironment();
	'''      GraphicsDevice[] gs =
	'''              ge.getScreenDevices();
	'''      for (int j = 0; j < gs.length; j++) {
	'''          GraphicsDevice gd = gs[j];
	'''          GraphicsConfiguration[] gc =
	'''              gd.getConfigurations();
	'''          for (int i=0; i < gc.length; i++) {
	'''              virtualBounds =
	'''                  virtualBounds.union(gc[i].getBounds());
	'''          }
	'''      } }</pre>
	''' </summary>
	''' <seealso cref= Window </seealso>
	''' <seealso cref= Frame </seealso>
	''' <seealso cref= GraphicsEnvironment </seealso>
	''' <seealso cref= GraphicsDevice </seealso>
	'
	' * REMIND:  What to do about capabilities?
	' * The
	' * capabilities of the device can be determined by enumerating the possible
	' * capabilities and checking if the GraphicsConfiguration
	' * implements the interface for that capability.
	' *
	' 


	Public MustInherit Class GraphicsConfiguration

		Private Shared defaultBufferCaps As BufferCapabilities
		Private Shared defaultImageCaps As ImageCapabilities

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Instances must be obtained from a suitable factory or query method.
		''' </summary>
		''' <seealso cref= GraphicsDevice#getConfigurations </seealso>
		''' <seealso cref= GraphicsDevice#getDefaultConfiguration </seealso>
		''' <seealso cref= GraphicsDevice#getBestConfiguration </seealso>
		''' <seealso cref= Graphics2D#getDeviceConfiguration </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the <seealso cref="GraphicsDevice"/> associated with this
		''' <code>GraphicsConfiguration</code>. </summary>
		''' <returns> a <code>GraphicsDevice</code> object that is
		''' associated with this <code>GraphicsConfiguration</code>. </returns>
		Public MustOverride ReadOnly Property device As GraphicsDevice

		''' <summary>
		''' Returns a <seealso cref="BufferedImage"/> with a data layout and color model
		''' compatible with this <code>GraphicsConfiguration</code>.  This
		''' method has nothing to do with memory-mapping
		''' a device.  The returned <code>BufferedImage</code> has
		''' a layout and color model that is closest to this native device
		''' configuration and can therefore be optimally blitted to this
		''' device. </summary>
		''' <param name="width"> the width of the returned <code>BufferedImage</code> </param>
		''' <param name="height"> the height of the returned <code>BufferedImage</code> </param>
		''' <returns> a <code>BufferedImage</code> whose data layout and color
		''' model is compatible with this <code>GraphicsConfiguration</code>. </returns>
		Public Overridable Function createCompatibleImage(  width As Integer,   height As Integer) As java.awt.image.BufferedImage
			Dim model As java.awt.image.ColorModel = colorModel
			Dim raster As java.awt.image.WritableRaster = model.createCompatibleWritableRaster(width, height)
			Return New java.awt.image.BufferedImage(model, raster, model.alphaPremultiplied, Nothing)
		End Function

		''' <summary>
		''' Returns a <code>BufferedImage</code> that supports the specified
		''' transparency and has a data layout and color model
		''' compatible with this <code>GraphicsConfiguration</code>.  This
		''' method has nothing to do with memory-mapping
		''' a device. The returned <code>BufferedImage</code> has a layout and
		''' color model that can be optimally blitted to a device
		''' with this <code>GraphicsConfiguration</code>. </summary>
		''' <param name="width"> the width of the returned <code>BufferedImage</code> </param>
		''' <param name="height"> the height of the returned <code>BufferedImage</code> </param>
		''' <param name="transparency"> the specified transparency mode </param>
		''' <returns> a <code>BufferedImage</code> whose data layout and color
		''' model is compatible with this <code>GraphicsConfiguration</code>
		''' and also supports the specified transparency. </returns>
		''' <exception cref="IllegalArgumentException"> if the transparency is not a valid value </exception>
		''' <seealso cref= Transparency#OPAQUE </seealso>
		''' <seealso cref= Transparency#BITMASK </seealso>
		''' <seealso cref= Transparency#TRANSLUCENT </seealso>
		Public Overridable Function createCompatibleImage(  width As Integer,   height As Integer,   transparency As Integer) As java.awt.image.BufferedImage
			If colorModel.transparency = transparency Then Return createCompatibleImage(width, height)

			Dim cm As java.awt.image.ColorModel = getColorModel(transparency)
			If cm Is Nothing Then Throw New IllegalArgumentException("Unknown transparency: " & transparency)
			Dim wr As java.awt.image.WritableRaster = cm.createCompatibleWritableRaster(width, height)
			Return New java.awt.image.BufferedImage(cm, wr, cm.alphaPremultiplied, Nothing)
		End Function


		''' <summary>
		''' Returns a <seealso cref="VolatileImage"/> with a data layout and color model
		''' compatible with this <code>GraphicsConfiguration</code>.
		''' The returned <code>VolatileImage</code>
		''' may have data that is stored optimally for the underlying graphics
		''' device and may therefore benefit from platform-specific rendering
		''' acceleration. </summary>
		''' <param name="width"> the width of the returned <code>VolatileImage</code> </param>
		''' <param name="height"> the height of the returned <code>VolatileImage</code> </param>
		''' <returns> a <code>VolatileImage</code> whose data layout and color
		''' model is compatible with this <code>GraphicsConfiguration</code>. </returns>
		''' <seealso cref= Component#createVolatileImage(int, int)
		''' @since 1.4 </seealso>
		Public Overridable Function createCompatibleVolatileImage(  width As Integer,   height As Integer) As java.awt.image.VolatileImage
			Dim vi As java.awt.image.VolatileImage = Nothing
			Try
				vi = createCompatibleVolatileImage(width, height, Nothing, Transparency.OPAQUE)
			Catch e As AWTException
				' shouldn't happen: we're passing in null caps
				Debug.Assert(False)
			End Try
			Return vi
		End Function

		''' <summary>
		''' Returns a <seealso cref="VolatileImage"/> with a data layout and color model
		''' compatible with this <code>GraphicsConfiguration</code>.
		''' The returned <code>VolatileImage</code>
		''' may have data that is stored optimally for the underlying graphics
		''' device and may therefore benefit from platform-specific rendering
		''' acceleration. </summary>
		''' <param name="width"> the width of the returned <code>VolatileImage</code> </param>
		''' <param name="height"> the height of the returned <code>VolatileImage</code> </param>
		''' <param name="transparency"> the specified transparency mode </param>
		''' <returns> a <code>VolatileImage</code> whose data layout and color
		''' model is compatible with this <code>GraphicsConfiguration</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if the transparency is not a valid value </exception>
		''' <seealso cref= Transparency#OPAQUE </seealso>
		''' <seealso cref= Transparency#BITMASK </seealso>
		''' <seealso cref= Transparency#TRANSLUCENT </seealso>
		''' <seealso cref= Component#createVolatileImage(int, int)
		''' @since 1.5 </seealso>
		Public Overridable Function createCompatibleVolatileImage(  width As Integer,   height As Integer,   transparency As Integer) As java.awt.image.VolatileImage
			Dim vi As java.awt.image.VolatileImage = Nothing
			Try
				vi = createCompatibleVolatileImage(width, height, Nothing, transparency)
			Catch e As AWTException
				' shouldn't happen: we're passing in null caps
				Debug.Assert(False)
			End Try
			Return vi
		End Function

		''' <summary>
		''' Returns a <seealso cref="VolatileImage"/> with a data layout and color model
		''' compatible with this <code>GraphicsConfiguration</code>, using
		''' the specified image capabilities.
		''' If the <code>caps</code> parameter is null, it is effectively ignored
		''' and this method will create a VolatileImage without regard to
		''' <code>ImageCapabilities</code> constraints.
		''' 
		''' The returned <code>VolatileImage</code> has
		''' a layout and color model that is closest to this native device
		''' configuration and can therefore be optimally blitted to this
		''' device. </summary>
		''' <returns> a <code>VolatileImage</code> whose data layout and color
		''' model is compatible with this <code>GraphicsConfiguration</code>. </returns>
		''' <param name="width"> the width of the returned <code>VolatileImage</code> </param>
		''' <param name="height"> the height of the returned <code>VolatileImage</code> </param>
		''' <param name="caps"> the image capabilities </param>
		''' <exception cref="AWTException"> if the supplied image capabilities could not
		''' be met by this graphics configuration
		''' @since 1.4 </exception>
		Public Overridable Function createCompatibleVolatileImage(  width As Integer,   height As Integer,   caps As ImageCapabilities) As java.awt.image.VolatileImage
			Return createCompatibleVolatileImage(width, height, caps, Transparency.OPAQUE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="VolatileImage"/> with a data layout and color model
		''' compatible with this <code>GraphicsConfiguration</code>, using
		''' the specified image capabilities and transparency value.
		''' If the <code>caps</code> parameter is null, it is effectively ignored
		''' and this method will create a VolatileImage without regard to
		''' <code>ImageCapabilities</code> constraints.
		''' 
		''' The returned <code>VolatileImage</code> has
		''' a layout and color model that is closest to this native device
		''' configuration and can therefore be optimally blitted to this
		''' device. </summary>
		''' <param name="width"> the width of the returned <code>VolatileImage</code> </param>
		''' <param name="height"> the height of the returned <code>VolatileImage</code> </param>
		''' <param name="caps"> the image capabilities </param>
		''' <param name="transparency"> the specified transparency mode </param>
		''' <returns> a <code>VolatileImage</code> whose data layout and color
		''' model is compatible with this <code>GraphicsConfiguration</code>. </returns>
		''' <seealso cref= Transparency#OPAQUE </seealso>
		''' <seealso cref= Transparency#BITMASK </seealso>
		''' <seealso cref= Transparency#TRANSLUCENT </seealso>
		''' <exception cref="IllegalArgumentException"> if the transparency is not a valid value </exception>
		''' <exception cref="AWTException"> if the supplied image capabilities could not
		''' be met by this graphics configuration </exception>
		''' <seealso cref= Component#createVolatileImage(int, int)
		''' @since 1.5 </seealso>
		Public Overridable Function createCompatibleVolatileImage(  width As Integer,   height As Integer,   caps As ImageCapabilities,   transparency As Integer) As java.awt.image.VolatileImage
			Dim vi As java.awt.image.VolatileImage = New sun.awt.image.SunVolatileImage(Me, width, height, transparency, caps)
			If caps IsNot Nothing AndAlso caps.accelerated AndAlso (Not vi.capabilities.accelerated) Then Throw New AWTException("Supplied image capabilities could not " & "be met by this graphics configuration.")
			Return vi
		End Function

		''' <summary>
		''' Returns the <seealso cref="ColorModel"/> associated with this
		''' <code>GraphicsConfiguration</code>. </summary>
		''' <returns> a <code>ColorModel</code> object that is associated with
		''' this <code>GraphicsConfiguration</code>. </returns>
		Public MustOverride ReadOnly Property colorModel As java.awt.image.ColorModel

		''' <summary>
		''' Returns the <code>ColorModel</code> associated with this
		''' <code>GraphicsConfiguration</code> that supports the specified
		''' transparency. </summary>
		''' <param name="transparency"> the specified transparency mode </param>
		''' <returns> a <code>ColorModel</code> object that is associated with
		''' this <code>GraphicsConfiguration</code> and supports the
		''' specified transparency or null if the transparency is not a valid
		''' value. </returns>
		''' <seealso cref= Transparency#OPAQUE </seealso>
		''' <seealso cref= Transparency#BITMASK </seealso>
		''' <seealso cref= Transparency#TRANSLUCENT </seealso>
		Public MustOverride Function getColorModel(  transparency As Integer) As java.awt.image.ColorModel

		''' <summary>
		''' Returns the default <seealso cref="AffineTransform"/> for this
		''' <code>GraphicsConfiguration</code>. This
		''' <code>AffineTransform</code> is typically the Identity transform
		''' for most normal screens.  The default <code>AffineTransform</code>
		''' maps coordinates onto the device such that 72 user space
		''' coordinate units measure approximately 1 inch in device
		''' space.  The normalizing transform can be used to make
		''' this mapping more exact.  Coordinates in the coordinate space
		''' defined by the default <code>AffineTransform</code> for screen and
		''' printer devices have the origin in the upper left-hand corner of
		''' the target region of the device, with X coordinates
		''' increasing to the right and Y coordinates increasing downwards.
		''' For image buffers not associated with a device, such as those not
		''' created by <code>createCompatibleImage</code>,
		''' this <code>AffineTransform</code> is the Identity transform. </summary>
		''' <returns> the default <code>AffineTransform</code> for this
		''' <code>GraphicsConfiguration</code>. </returns>
		Public MustOverride ReadOnly Property defaultTransform As java.awt.geom.AffineTransform

		''' 
		''' <summary>
		''' Returns a <code>AffineTransform</code> that can be concatenated
		''' with the default <code>AffineTransform</code>
		''' of a <code>GraphicsConfiguration</code> so that 72 units in user
		''' space equals 1 inch in device space.
		''' <p>
		''' For a particular <seealso cref="Graphics2D"/>, g, one
		''' can reset the transformation to create
		''' such a mapping by using the following pseudocode:
		''' <pre>
		'''      GraphicsConfiguration gc = g.getDeviceConfiguration();
		''' 
		'''      g.setTransform(gc.getDefaultTransform());
		'''      g.transform(gc.getNormalizingTransform());
		''' </pre>
		''' Note that sometimes this <code>AffineTransform</code> is identity,
		''' such as for printers or metafile output, and that this
		''' <code>AffineTransform</code> is only as accurate as the information
		''' supplied by the underlying system.  For image buffers not
		''' associated with a device, such as those not created by
		''' <code>createCompatibleImage</code>, this
		''' <code>AffineTransform</code> is the Identity transform
		''' since there is no valid distance measurement. </summary>
		''' <returns> an <code>AffineTransform</code> to concatenate to the
		''' default <code>AffineTransform</code> so that 72 units in user
		''' space is mapped to 1 inch in device space. </returns>
		Public MustOverride ReadOnly Property normalizingTransform As java.awt.geom.AffineTransform

		''' <summary>
		''' Returns the bounds of the <code>GraphicsConfiguration</code>
		''' in the device coordinates. In a multi-screen environment
		''' with a virtual device, the bounds can have negative X
		''' or Y origins. </summary>
		''' <returns> the bounds of the area covered by this
		''' <code>GraphicsConfiguration</code>.
		''' @since 1.3 </returns>
		Public MustOverride ReadOnly Property bounds As Rectangle

		Private Class DefaultBufferCapabilities
			Inherits BufferCapabilities

			Public Sub New(  imageCaps As ImageCapabilities)
				MyBase.New(imageCaps, imageCaps, Nothing)
			End Sub
		End Class

		''' <summary>
		''' Returns the buffering capabilities of this
		''' <code>GraphicsConfiguration</code>. </summary>
		''' <returns> the buffering capabilities of this graphics
		''' configuration object
		''' @since 1.4 </returns>
		Public Overridable Property bufferCapabilities As BufferCapabilities
			Get
				If defaultBufferCaps Is Nothing Then defaultBufferCaps = New DefaultBufferCapabilities(imageCapabilities)
				Return defaultBufferCaps
			End Get
		End Property

		''' <summary>
		''' Returns the image capabilities of this
		''' <code>GraphicsConfiguration</code>. </summary>
		''' <returns> the image capabilities of this graphics
		''' configuration object
		''' @since 1.4 </returns>
		Public Overridable Property imageCapabilities As ImageCapabilities
			Get
				If defaultImageCaps Is Nothing Then defaultImageCaps = New ImageCapabilities(False)
				Return defaultImageCaps
			End Get
		End Property

		''' <summary>
		''' Returns whether this {@code GraphicsConfiguration} supports
		''' the {@link GraphicsDevice.WindowTranslucency#PERPIXEL_TRANSLUCENT
		''' PERPIXEL_TRANSLUCENT} kind of translucency.
		''' </summary>
		''' <returns> whether the given GraphicsConfiguration supports
		'''         the translucency effects.
		''' </returns>
		''' <seealso cref= Window#setBackground(Color)
		''' 
		''' @since 1.7 </seealso>
		Public Overridable Property translucencyCapable As Boolean
			Get
				' Overridden in subclasses
				Return False
			End Get
		End Property
	End Class

End Namespace
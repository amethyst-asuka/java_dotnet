Imports Microsoft.VisualBasic

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

'
' **********************************************************************
' **********************************************************************
' **********************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
' *** As  an unpublished  work pursuant to Title 17 of the United    ***
' *** States Code.  All rights reserved.                             ***
' **********************************************************************
' **********************************************************************
' *********************************************************************

Namespace java.awt.image


	''' <summary>
	''' This class performs a pixel-by-pixel color conversion of the data in
	''' the source image.  The resulting color values are scaled to the precision
	''' of the destination image.  Color conversion can be specified
	''' via an array of ColorSpace objects or an array of ICC_Profile objects.
	''' <p>
	''' If the source is a BufferedImage with premultiplied alpha, the
	''' color components are divided by the alpha component before color conversion.
	''' If the destination is a BufferedImage with premultiplied alpha, the
	''' color components are multiplied by the alpha component after conversion.
	''' Rasters are treated as having no alpha channel, i.e. all bands are
	''' color bands.
	''' <p>
	''' If a RenderingHints object is specified in the constructor, the
	''' color rendering hint and the dithering hint may be used to control
	''' color conversion.
	''' <p>
	''' Note that Source and Destination may be the same object. </summary>
	''' <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	Public Class ColorConvertOp
		Implements BufferedImageOp, RasterOp

		Friend profileList As ICC_Profile()
		Friend CSList As ColorSpace()
		Friend thisTransform, thisRasterTransform As sun.java2d.cmm.ColorTransform
		Friend thisSrcProfile, thisDestProfile As ICC_Profile
		Friend hints As java.awt.RenderingHints
		Friend gotProfiles As Boolean
		Friend srcMinVals, srcMaxVals, dstMinVals, dstMaxVals As Single()

		' the class initializer 
		Shared Sub New()
			If sun.java2d.cmm.ProfileDeferralMgr.deferring Then sun.java2d.cmm.ProfileDeferralMgr.activateProfiles()
		End Sub

		''' <summary>
		''' Constructs a new ColorConvertOp which will convert
		''' from a source color space to a destination color space.
		''' The RenderingHints argument may be null.
		''' This Op can be used only with BufferedImages, and will convert
		''' directly from the ColorSpace of the source image to that of the
		''' destination.  The destination argument of the filter method
		''' cannot be specified as null. </summary>
		''' <param name="hints"> the <code>RenderingHints</code> object used to control
		'''        the color conversion, or <code>null</code> </param>
		Public Sub New(ByVal hints As java.awt.RenderingHints)
			profileList = New ICC_Profile (){} ' 0 length list
			Me.hints = hints
		End Sub

		''' <summary>
		''' Constructs a new ColorConvertOp from a ColorSpace object.
		''' The RenderingHints argument may be null.  This
		''' Op can be used only with BufferedImages, and is primarily useful
		''' when the <seealso cref="#filter(BufferedImage, BufferedImage) filter"/>
		''' method is invoked with a destination argument of null.
		''' In that case, the ColorSpace defines the destination color space
		''' for the destination created by the filter method.  Otherwise, the
		''' ColorSpace defines an intermediate space to which the source is
		''' converted before being converted to the destination space. </summary>
		''' <param name="cspace"> defines the destination <code>ColorSpace</code> or an
		'''        intermediate <code>ColorSpace</code> </param>
		''' <param name="hints"> the <code>RenderingHints</code> object used to control
		'''        the color conversion, or <code>null</code> </param>
		''' <exception cref="NullPointerException"> if cspace is null </exception>
		Public Sub New(ByVal cspace As ColorSpace, ByVal hints As java.awt.RenderingHints)
			If cspace Is Nothing Then Throw New NullPointerException("ColorSpace cannot be null")
			If TypeOf cspace Is ICC_ColorSpace Then
				profileList = New ICC_Profile (0){} ' 1 profile in the list

				profileList (0) = CType(cspace, ICC_ColorSpace).profile
			Else
				CSList = New ColorSpace(0){} ' non-ICC case: 1 ColorSpace in list
				CSList(0) = cspace
			End If
			Me.hints = hints
		End Sub


		''' <summary>
		''' Constructs a new ColorConvertOp from two ColorSpace objects.
		''' The RenderingHints argument may be null.
		''' This Op is primarily useful for calling the filter method on
		''' Rasters, in which case the two ColorSpaces define the operation
		''' to be performed on the Rasters.  In that case, the number of bands
		''' in the source Raster must match the number of components in
		''' srcCspace, and the number of bands in the destination Raster
		''' must match the number of components in dstCspace.  For BufferedImages,
		''' the two ColorSpaces define intermediate spaces through which the
		''' source is converted before being converted to the destination space. </summary>
		''' <param name="srcCspace"> the source <code>ColorSpace</code> </param>
		''' <param name="dstCspace"> the destination <code>ColorSpace</code> </param>
		''' <param name="hints"> the <code>RenderingHints</code> object used to control
		'''        the color conversion, or <code>null</code> </param>
		''' <exception cref="NullPointerException"> if either srcCspace or dstCspace is null </exception>
		Public Sub New(ByVal srcCspace As ColorSpace, ByVal dstCspace As ColorSpace, ByVal hints As java.awt.RenderingHints)
			If (srcCspace Is Nothing) OrElse (dstCspace Is Nothing) Then Throw New NullPointerException("ColorSpaces cannot be null")
			If (TypeOf srcCspace Is ICC_ColorSpace) AndAlso (TypeOf dstCspace Is ICC_ColorSpace) Then
				profileList = New ICC_Profile (1){} ' 2 profiles in the list

				profileList (0) = CType(srcCspace, ICC_ColorSpace).profile
				profileList (1) = CType(dstCspace, ICC_ColorSpace).profile

				getMinMaxValsFromColorSpaces(srcCspace, dstCspace)
			Else
				' non-ICC case: 2 ColorSpaces in list 
				CSList = New ColorSpace(1){}
				CSList(0) = srcCspace
				CSList(1) = dstCspace
			End If
			Me.hints = hints
		End Sub


		 ''' <summary>
		 ''' Constructs a new ColorConvertOp from an array of ICC_Profiles.
		 ''' The RenderingHints argument may be null.
		 ''' The sequence of profiles may include profiles that represent color
		 ''' spaces, profiles that represent effects, etc.  If the whole sequence
		 ''' does not represent a well-defined color conversion, an exception is
		 ''' thrown.
		 ''' <p>For BufferedImages, if the ColorSpace
		 ''' of the source BufferedImage does not match the requirements of the
		 ''' first profile in the array,
		 ''' the first conversion is to an appropriate ColorSpace.
		 ''' If the requirements of the last profile in the array are not met
		 ''' by the ColorSpace of the destination BufferedImage,
		 ''' the last conversion is to the destination's ColorSpace.
		 ''' <p>For Rasters, the number of bands in the source Raster must match
		 ''' the requirements of the first profile in the array, and the
		 ''' number of bands in the destination Raster must match the requirements
		 ''' of the last profile in the array.  The array must have at least two
		 ''' elements or calling the filter method for Rasters will throw an
		 ''' IllegalArgumentException. </summary>
		 ''' <param name="profiles"> the array of <code>ICC_Profile</code> objects </param>
		 ''' <param name="hints"> the <code>RenderingHints</code> object used to control
		 '''        the color conversion, or <code>null</code> </param>
		 ''' <exception cref="IllegalArgumentException"> when the profile sequence does not
		 '''             specify a well-defined color conversion </exception>
		 ''' <exception cref="NullPointerException"> if profiles is null </exception>
		Public Sub New(ByVal profiles As ICC_Profile(), ByVal hints As java.awt.RenderingHints)
			If profiles Is Nothing Then Throw New NullPointerException("Profiles cannot be null")
			gotProfiles = True
			profileList = New ICC_Profile(profiles.Length - 1){}
			For i1 As Integer = 0 To profiles.Length - 1
				profileList(i1) = profiles(i1)
			Next i1
			Me.hints = hints
		End Sub


		''' <summary>
		''' Returns the array of ICC_Profiles used to construct this ColorConvertOp.
		''' Returns null if the ColorConvertOp was not constructed from such an
		''' array. </summary>
		''' <returns> the array of <code>ICC_Profile</code> objects of this
		'''         <code>ColorConvertOp</code>, or <code>null</code> if this
		'''         <code>ColorConvertOp</code> was not constructed with an
		'''         array of <code>ICC_Profile</code> objects. </returns>
		Public Property iCC_Profiles As ICC_Profile()
			Get
				If gotProfiles Then
					Dim profiles As ICC_Profile() = New ICC_Profile(profileList.Length - 1){}
					For i1 As Integer = 0 To profileList.Length - 1
						profiles(i1) = profileList(i1)
					Next i1
					Return profiles
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' ColorConverts the source BufferedImage.
		''' If the destination image is null,
		''' a BufferedImage will be created with an appropriate ColorModel. </summary>
		''' <param name="src"> the source <code>BufferedImage</code> to be converted </param>
		''' <param name="dest"> the destination <code>BufferedImage</code>,
		'''        or <code>null</code> </param>
		''' <returns> <code>dest</code> color converted from <code>src</code>
		'''         or a new, converted <code>BufferedImage</code>
		'''         if <code>dest</code> is <code>null</code> </returns>
		''' <exception cref="IllegalArgumentException"> if dest is null and this op was
		'''             constructed using the constructor which takes only a
		'''             RenderingHints argument, since the operation is ill defined. </exception>
		Public Function filter(ByVal src As BufferedImage, ByVal dest As BufferedImage) As BufferedImage Implements BufferedImageOp.filter
			Dim srcColorSpace, destColorSpace As ColorSpace
			Dim savdest As BufferedImage = Nothing

			If TypeOf src.colorModel Is IndexColorModel Then
				Dim icm As IndexColorModel = CType(src.colorModel, IndexColorModel)
				src = icm.convertToIntDiscrete(src.raster, True)
			End If
			srcColorSpace = src.colorModel.colorSpace
			If dest IsNot Nothing Then
				If TypeOf dest.colorModel Is IndexColorModel Then
					savdest = dest
					dest = Nothing
					destColorSpace = Nothing
				Else
					destColorSpace = dest.colorModel.colorSpace
				End If
			Else
				destColorSpace = Nothing
			End If

			If (CSList IsNot Nothing) OrElse (Not(TypeOf srcColorSpace Is ICC_ColorSpace)) OrElse ((dest IsNot Nothing) AndAlso (Not(TypeOf destColorSpace Is ICC_ColorSpace))) Then
				' non-ICC case 
				dest = nonICCBIFilter(src, srcColorSpace, dest, destColorSpace)
			Else
				dest = ICCBIFilter(src, srcColorSpace, dest, destColorSpace)
			End If

			If savdest IsNot Nothing Then
				Dim big As java.awt.Graphics2D = savdest.createGraphics()
				Try
					big.drawImage(dest, 0, 0, Nothing)
				Finally
					big.Dispose()
				End Try
				Return savdest
			Else
				Return dest
			End If
		End Function

		Private Function ICCBIFilter(ByVal src As BufferedImage, ByVal srcColorSpace As ColorSpace, ByVal dest As BufferedImage, ByVal destColorSpace As ColorSpace) As BufferedImage
		Dim nProfiles As Integer = profileList.Length
		Dim srcProfile As ICC_Profile = Nothing, destProfile As ICC_Profile = Nothing

			srcProfile = CType(srcColorSpace, ICC_ColorSpace).profile

			If dest Is Nothing Then
	'         last profile in the list defines
	'                                      the output color space 
				If nProfiles = 0 Then Throw New IllegalArgumentException("Destination ColorSpace is undefined")
				destProfile = profileList (nProfiles - 1)
				dest = createCompatibleDestImage(src, Nothing)
			Else
				If src.height <> dest.height OrElse src.width <> dest.width Then Throw New IllegalArgumentException("Width or height of BufferedImages do not match")
				destProfile = CType(destColorSpace, ICC_ColorSpace).profile
			End If

	'         Checking if all profiles in the transform sequence are the same.
	'         * If so, performing just copying the data.
	'         
			If srcProfile Is destProfile Then
				Dim noTrans As Boolean = True
				For i As Integer = 0 To nProfiles - 1
					If srcProfile IsNot profileList(i) Then
						noTrans = False
						Exit For
					End If
				Next i
				If noTrans Then
					Dim g As java.awt.Graphics2D = dest.createGraphics()
					Try
						g.drawImage(src, 0, 0, Nothing)
					Finally
						g.Dispose()
					End Try

					Return dest
				End If
			End If

			' make a new transform if needed 
			If (thisTransform Is Nothing) OrElse (thisSrcProfile IsNot srcProfile) OrElse (thisDestProfile IsNot destProfile) Then updateBITransform(srcProfile, destProfile)

			' color convert the image 
			thisTransform.colorConvert(src, dest)

			Return dest
		End Function

		Private Sub updateBITransform(ByVal srcProfile As ICC_Profile, ByVal destProfile As ICC_Profile)
			Dim theProfiles As ICC_Profile()
			Dim i1, nProfiles, nTransforms, whichTrans, renderState As Integer
			Dim theTransforms As sun.java2d.cmm.ColorTransform()
			Dim useSrc As Boolean = False, useDest As Boolean = False

			nProfiles = profileList.Length
			nTransforms = nProfiles
			If (nProfiles = 0) OrElse (srcProfile IsNot profileList(0)) Then
				nTransforms += 1
				useSrc = True
			End If
			If (nProfiles = 0) OrElse (destProfile IsNot profileList(nProfiles - 1)) OrElse (nTransforms < 2) Then
				nTransforms += 1
				useDest = True
			End If

			' make the profile list 
			theProfiles = New ICC_Profile(nTransforms - 1){} ' the list of profiles
	'                                                       for this Op 

			Dim idx As Integer = 0
			If useSrc Then
				' insert source as first profile 
				theProfiles(idx) = srcProfile
				idx += 1
			End If

			For i1 = 0 To nProfiles - 1
									   ' insert profiles defined in this Op 
				theProfiles(idx) = profileList (i1)
				idx += 1
			Next i1

			If useDest Then theProfiles(idx) = destProfile

			' make the transform list 
			theTransforms = New sun.java2d.cmm.ColorTransform (nTransforms - 1){}

			' initialize transform get loop 
			If theProfiles(0).profileClass = ICC_Profile.CLASS_OUTPUT Then
	'                                         if first profile is a printer
	'                                           render as colorimetric 
				renderState = ICC_Profile.icRelativeColorimetric
			Else
				renderState = ICC_Profile.icPerceptual ' render any other
	'                                                       class perceptually 
			End If

			whichTrans = sun.java2d.cmm.ColorTransform.In

			Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module

			' get the transforms from each profile 
			For i1 = 0 To nTransforms - 1
				If i1 = nTransforms -1 Then ' last profile?
					whichTrans = sun.java2d.cmm.ColorTransform.Out ' get output transform
				Else ' check for abstract profile
					If (whichTrans = sun.java2d.cmm.ColorTransform.Simulation) AndAlso (theProfiles(i1).profileClass = ICC_Profile.CLASS_ABSTRACT) Then
					renderState = ICC_Profile.icPerceptual
						whichTrans = sun.java2d.cmm.ColorTransform.In
					End If
				End If

				theTransforms(i1) = mdl.createTransform(theProfiles(i1), renderState, whichTrans)

	'             get this profile's rendering intent to select transform
	'               from next profile 
				renderState = getRenderingIntent(theProfiles(i1))

				' "middle" profiles use simulation transform 
				whichTrans = sun.java2d.cmm.ColorTransform.Simulation
			Next i1

			' make the net transform 
			thisTransform = mdl.createTransform(theTransforms)

			' update corresponding source and dest profiles 
			thisSrcProfile = srcProfile
			thisDestProfile = destProfile
		End Sub

		''' <summary>
		''' ColorConverts the image data in the source Raster.
		''' If the destination Raster is null, a new Raster will be created.
		''' The number of bands in the source and destination Rasters must
		''' meet the requirements explained above.  The constructor used to
		''' create this ColorConvertOp must have provided enough information
		''' to define both source and destination color spaces.  See above.
		''' Otherwise, an exception is thrown. </summary>
		''' <param name="src"> the source <code>Raster</code> to be converted </param>
		''' <param name="dest"> the destination <code>WritableRaster</code>,
		'''        or <code>null</code> </param>
		''' <returns> <code>dest</code> color converted from <code>src</code>
		'''         or a new, converted <code>WritableRaster</code>
		'''         if <code>dest</code> is <code>null</code> </returns>
		''' <exception cref="IllegalArgumentException"> if the number of source or
		'''             destination bands is incorrect, the source or destination
		'''             color spaces are undefined, or this op was constructed
		'''             with one of the constructors that applies only to
		'''             operations on BufferedImages. </exception>
		Public Function filter(ByVal src As Raster, ByVal dest As WritableRaster) As WritableRaster Implements RasterOp.filter

			If CSList IsNot Nothing Then Return nonICCRasterFilter(src, dest)
			Dim nProfiles As Integer = profileList.Length
			If nProfiles < 2 Then Throw New IllegalArgumentException("Source or Destination ColorSpace is undefined")
			If src.numBands <> profileList(0).numComponents Then Throw New IllegalArgumentException("Numbers of source Raster bands and source color space " & "components do not match")
			If dest Is Nothing Then
				dest = createCompatibleDestRaster(src)
			Else
				If src.height <> dest.height OrElse src.width <> dest.width Then Throw New IllegalArgumentException("Width or height of Rasters do not match")
				If dest.numBands <> profileList(nProfiles-1).numComponents Then Throw New IllegalArgumentException("Numbers of destination Raster bands and destination " & "color space components do not match")
			End If

			' make a new transform if needed 
			If thisRasterTransform Is Nothing Then
				Dim i1, whichTrans, renderState As Integer
				Dim theTransforms As sun.java2d.cmm.ColorTransform()

				' make the transform list 
				theTransforms = New sun.java2d.cmm.ColorTransform (nProfiles - 1){}

				' initialize transform get loop 
				If profileList(0).profileClass = ICC_Profile.CLASS_OUTPUT Then
	'                                             if first profile is a printer
	'                                               render as colorimetric 
					renderState = ICC_Profile.icRelativeColorimetric
				Else
					renderState = ICC_Profile.icPerceptual ' render any other
	'                                                           class perceptually 
				End If

				whichTrans = sun.java2d.cmm.ColorTransform.In

				Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module

				' get the transforms from each profile 
				For i1 = 0 To nProfiles - 1
					If i1 = nProfiles -1 Then ' last profile?
						whichTrans = sun.java2d.cmm.ColorTransform.Out ' get output transform
					Else ' check for abstract profile
						If (whichTrans = sun.java2d.cmm.ColorTransform.Simulation) AndAlso (profileList(i1).profileClass = ICC_Profile.CLASS_ABSTRACT) Then
							renderState = ICC_Profile.icPerceptual
							whichTrans = sun.java2d.cmm.ColorTransform.In
						End If
					End If

					theTransforms(i1) = mdl.createTransform(profileList(i1), renderState, whichTrans)

	'                 get this profile's rendering intent to select transform
	'                   from next profile 
					renderState = getRenderingIntent(profileList(i1))

					' "middle" profiles use simulation transform 
					whichTrans = sun.java2d.cmm.ColorTransform.Simulation
				Next i1

				' make the net transform 
				thisRasterTransform = mdl.createTransform(theTransforms)
			End If

			Dim srcTransferType As Integer = src.transferType
			Dim dstTransferType As Integer = dest.transferType
			If (srcTransferType = DataBuffer.TYPE_FLOAT) OrElse (srcTransferType = DataBuffer.TYPE_DOUBLE) OrElse (dstTransferType = DataBuffer.TYPE_FLOAT) OrElse (dstTransferType = DataBuffer.TYPE_DOUBLE) Then
				If srcMinVals Is Nothing Then getMinMaxValsFromProfiles(profileList(0), profileList(nProfiles-1))
				' color convert the raster 
				thisRasterTransform.colorConvert(src, dest, srcMinVals, srcMaxVals, dstMinVals, dstMaxVals)
			Else
				' color convert the raster 
				thisRasterTransform.colorConvert(src, dest)
			End If


			Return dest
		End Function

		''' <summary>
		''' Returns the bounding box of the destination, given this source.
		''' Note that this will be the same as the the bounding box of the
		''' source. </summary>
		''' <param name="src"> the source <code>BufferedImage</code> </param>
		''' <returns> a <code>Rectangle2D</code> that is the bounding box
		'''         of the destination, given the specified <code>src</code> </returns>
		Public Function getBounds2D(ByVal src As BufferedImage) As java.awt.geom.Rectangle2D Implements BufferedImageOp.getBounds2D
			Return getBounds2D(src.raster)
		End Function

		''' <summary>
		''' Returns the bounding box of the destination, given this source.
		''' Note that this will be the same as the the bounding box of the
		''' source. </summary>
		''' <param name="src"> the source <code>Raster</code> </param>
		''' <returns> a <code>Rectangle2D</code> that is the bounding box
		'''         of the destination, given the specified <code>src</code> </returns>
		Public Function getBounds2D(ByVal src As Raster) As java.awt.geom.Rectangle2D Implements RasterOp.getBounds2D
	'                return new Rectangle (src.getXOffset(),
	'                              src.getYOffset(),
	'                              src.getWidth(), src.getHeight()); 
			Return src.bounds
		End Function

		''' <summary>
		''' Creates a zeroed destination image with the correct size and number of
		''' bands, given this source. </summary>
		''' <param name="src">       Source image for the filter operation. </param>
		''' <param name="destCM">    ColorModel of the destination.  If null, an
		'''                  appropriate ColorModel will be used. </param>
		''' <returns> a <code>BufferedImage</code> with the correct size and
		''' number of bands from the specified <code>src</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>destCM</code> is
		'''         <code>null</code> and this <code>ColorConvertOp</code> was
		'''         created without any <code>ICC_Profile</code> or
		'''         <code>ColorSpace</code> defined for the destination </exception>
		Public Overridable Function createCompatibleDestImage(ByVal src As BufferedImage, ByVal destCM As ColorModel) As BufferedImage Implements BufferedImageOp.createCompatibleDestImage
			Dim cs As ColorSpace = Nothing
			If destCM Is Nothing Then
				If CSList Is Nothing Then
					' ICC case 
					Dim nProfiles As Integer = profileList.Length
					If nProfiles = 0 Then Throw New IllegalArgumentException("Destination ColorSpace is undefined")
					Dim destProfile As ICC_Profile = profileList(nProfiles - 1)
					cs = New ICC_ColorSpace(destProfile)
				Else
					' non-ICC case 
					Dim nSpaces As Integer = CSList.Length
					cs = CSList(nSpaces - 1)
				End If
			End If
			Return createCompatibleDestImage(src, destCM, cs)
		End Function

		Private Function createCompatibleDestImage(ByVal src As BufferedImage, ByVal destCM As ColorModel, ByVal destCS As ColorSpace) As BufferedImage
			Dim image_Renamed As BufferedImage
			If destCM Is Nothing Then
				Dim srcCM As ColorModel = src.colorModel
				Dim nbands As Integer = destCS.numComponents
				Dim hasAlpha As Boolean = srcCM.hasAlpha()
				If hasAlpha Then nbands += 1
				Dim nbits As Integer() = New Integer(nbands - 1){}
				For i As Integer = 0 To nbands - 1
					nbits(i) = 8
				Next i
				destCM = New ComponentColorModel(destCS, nbits, hasAlpha, srcCM.alphaPremultiplied, srcCM.transparency, DataBuffer.TYPE_BYTE)
			End If
			Dim w As Integer = src.width
			Dim h As Integer = src.height
			image_Renamed = New BufferedImage(destCM, destCM.createCompatibleWritableRaster(w, h), destCM.alphaPremultiplied, Nothing)
			Return image_Renamed
		End Function


		''' <summary>
		''' Creates a zeroed destination Raster with the correct size and number of
		''' bands, given this source. </summary>
		''' <param name="src"> the specified <code>Raster</code> </param>
		''' <returns> a <code>WritableRaster</code> with the correct size and number
		'''         of bands from the specified <code>src</code> </returns>
		''' <exception cref="IllegalArgumentException"> if this <code>ColorConvertOp</code>
		'''         was created without sufficient information to define the
		'''         <code>dst</code> and <code>src</code> color spaces </exception>
		Public Overridable Function createCompatibleDestRaster(ByVal src As Raster) As WritableRaster Implements RasterOp.createCompatibleDestRaster
			Dim ncomponents As Integer

			If CSList IsNot Nothing Then
				' non-ICC case 
				If CSList.Length <> 2 Then Throw New IllegalArgumentException("Destination ColorSpace is undefined")
				ncomponents = CSList(1).numComponents
			Else
				' ICC case 
				Dim nProfiles As Integer = profileList.Length
				If nProfiles < 2 Then Throw New IllegalArgumentException("Destination ColorSpace is undefined")
				ncomponents = profileList(nProfiles-1).numComponents
			End If

			Dim dest As WritableRaster = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, src.width, src.height, ncomponents, New java.awt.Point(src.minX, src.minY))
			Return dest
		End Function

		''' <summary>
		''' Returns the location of the destination point given a
		''' point in the source.  If <code>dstPt</code> is non-null,
		''' it will be used to hold the return value.  Note that
		''' for this class, the destination point will be the same
		''' as the source point. </summary>
		''' <param name="srcPt"> the specified source <code>Point2D</code> </param>
		''' <param name="dstPt"> the destination <code>Point2D</code> </param>
		''' <returns> <code>dstPt</code> after setting its location to be
		'''         the same as <code>srcPt</code> </returns>
		Public Function getPoint2D(ByVal srcPt As java.awt.geom.Point2D, ByVal dstPt As java.awt.geom.Point2D) As java.awt.geom.Point2D Implements BufferedImageOp.getPoint2D, RasterOp.getPoint2D
			If dstPt Is Nothing Then dstPt = New java.awt.geom.Point2D.Float
			dstPt.locationion(srcPt.x, srcPt.y)

			Return dstPt
		End Function


		''' <summary>
		''' Returns the RenderingIntent from the specified ICC Profile.
		''' </summary>
		Private Function getRenderingIntent(ByVal profile As ICC_Profile) As Integer
			Dim header As SByte() = profile.getData(ICC_Profile.icSigHead)
			Dim index As Integer = ICC_Profile.icHdrRenderingIntent

	'         According to ICC spec, only the least-significant 16 bits shall be
	'         * used to encode the rendering intent. The most significant 16 bits
	'         * shall be set to zero. Thus, we are ignoring two most significant
	'         * bytes here.
	'         *
	'         *  See http://www.color.org/ICC1v42_2006-05.pdf, section 7.2.15.
	'         
			Return ((header(index+2) And &Hff) << 8) Or (header(index+3) And &Hff)
		End Function

		''' <summary>
		''' Returns the rendering hints used by this op. </summary>
		''' <returns> the <code>RenderingHints</code> object of this
		'''         <code>ColorConvertOp</code> </returns>
		Public Property renderingHints As java.awt.RenderingHints Implements BufferedImageOp.getRenderingHints, RasterOp.getRenderingHints
			Get
				Return hints
			End Get
		End Property

		Private Function nonICCBIFilter(ByVal src As BufferedImage, ByVal srcColorSpace As ColorSpace, ByVal dst As BufferedImage, ByVal dstColorSpace As ColorSpace) As BufferedImage

			Dim w As Integer = src.width
			Dim h As Integer = src.height
			Dim ciespace As ICC_ColorSpace = CType(ColorSpace.getInstance(ColorSpace.CS_CIEXYZ), ICC_ColorSpace)
			If dst Is Nothing Then
				dst = createCompatibleDestImage(src, Nothing)
				dstColorSpace = dst.colorModel.colorSpace
			Else
				If (h <> dst.height) OrElse (w <> dst.width) Then Throw New IllegalArgumentException("Width or height of BufferedImages do not match")
			End If
			Dim srcRas As Raster = src.raster
			Dim dstRas As WritableRaster = dst.raster
			Dim srcCM As ColorModel = src.colorModel
			Dim dstCM As ColorModel = dst.colorModel
			Dim srcNumComp As Integer = srcCM.numColorComponents
			Dim dstNumComp As Integer = dstCM.numColorComponents
			Dim dstHasAlpha As Boolean = dstCM.hasAlpha()
			Dim needSrcAlpha As Boolean = srcCM.hasAlpha() AndAlso dstHasAlpha
			Dim list_Renamed As ColorSpace()
			If (CSList Is Nothing) AndAlso (profileList.Length <> 0) Then
				' possible non-ICC src, some profiles, possible non-ICC dst 
				Dim nonICCSrc, nonICCDst As Boolean
				Dim srcProfile, dstProfile As ICC_Profile
				If Not(TypeOf srcColorSpace Is ICC_ColorSpace) Then
					nonICCSrc = True
					srcProfile = ciespace.profile
				Else
					nonICCSrc = False
					srcProfile = CType(srcColorSpace, ICC_ColorSpace).profile
				End If
				If Not(TypeOf dstColorSpace Is ICC_ColorSpace) Then
					nonICCDst = True
					dstProfile = ciespace.profile
				Else
					nonICCDst = False
					dstProfile = CType(dstColorSpace, ICC_ColorSpace).profile
				End If
				' make a new transform if needed 
				If (thisTransform Is Nothing) OrElse (thisSrcProfile IsNot srcProfile) OrElse (thisDestProfile IsNot dstProfile) Then updateBITransform(srcProfile, dstProfile)
				' process per scanline
				Dim maxNum As Single = 65535.0f ' use 16-bit precision in CMM
				Dim cs As ColorSpace
				Dim iccSrcNumComp As Integer
				If nonICCSrc Then
					cs = ciespace
					iccSrcNumComp = 3
				Else
					cs = srcColorSpace
					iccSrcNumComp = srcNumComp
				End If
				Dim srcMinVal As Single() = New Single(iccSrcNumComp - 1){}
				Dim srcInvDiffMinMax As Single() = New Single(iccSrcNumComp - 1){}
				For i As Integer = 0 To srcNumComp - 1
					srcMinVal(i) = cs.getMinValue(i)
					srcInvDiffMinMax(i) = maxNum / (cs.getMaxValue(i) - srcMinVal(i))
				Next i
				Dim iccDstNumComp As Integer
				If nonICCDst Then
					cs = ciespace
					iccDstNumComp = 3
				Else
					cs = dstColorSpace
					iccDstNumComp = dstNumComp
				End If
				Dim dstMinVal As Single() = New Single(iccDstNumComp - 1){}
				Dim dstDiffMinMax As Single() = New Single(iccDstNumComp - 1){}
				For i As Integer = 0 To dstNumComp - 1
					dstMinVal(i) = cs.getMinValue(i)
					dstDiffMinMax(i) = (cs.getMaxValue(i) - dstMinVal(i)) / maxNum
				Next i
				Dim dstColor As Single()
				If dstHasAlpha Then
					Dim size As Integer = If((dstNumComp + 1) > 3, (dstNumComp + 1), 3)
					dstColor = New Single(size - 1){}
				Else
					Dim size As Integer = If(dstNumComp > 3, dstNumComp, 3)
					dstColor = New Single(size - 1){}
				End If
				Dim srcLine As Short() = New Short(w * iccSrcNumComp - 1){}
				Dim dstLine As Short() = New Short(w * iccDstNumComp - 1){}
				Dim pixel As Object
				Dim color_Renamed As Single()
				Dim alpha As Single() = Nothing
				If needSrcAlpha Then alpha = New Single(w - 1){}
				Dim idx As Integer
				' process each scanline
				For y As Integer = 0 To h - 1
					' convert src scanline
					pixel = Nothing
					color_Renamed = Nothing
					idx = 0
					For x As Integer = 0 To w - 1
						pixel = srcRas.getDataElements(x, y, pixel)
						color_Renamed = srcCM.getNormalizedComponents(pixel, color_Renamed, 0)
						If needSrcAlpha Then alpha(x) = color_Renamed(srcNumComp)
						If nonICCSrc Then color_Renamed = srcColorSpace.toCIEXYZ(color_Renamed)
						For i As Integer = 0 To iccSrcNumComp - 1
							srcLine(idx) = CShort(Fix((color_Renamed(i) - srcMinVal(i)) * srcInvDiffMinMax(i) + 0.5f))
							idx += 1
						Next i
					Next x
					' color convert srcLine to dstLine
					thisTransform.colorConvert(srcLine, dstLine)
					' convert dst scanline
					pixel = Nothing
					idx = 0
					For x As Integer = 0 To w - 1
						For i As Integer = 0 To iccDstNumComp - 1
							dstColor(i) = (CSng(dstLine(idx) And &Hffff)) * dstDiffMinMax(i) + dstMinVal(i)
							idx += 1
						Next i
						If nonICCDst Then
							color_Renamed = srcColorSpace.fromCIEXYZ(dstColor)
							For i As Integer = 0 To dstNumComp - 1
								dstColor(i) = color_Renamed(i)
							Next i
						End If
						If needSrcAlpha Then
							dstColor(dstNumComp) = alpha(x)
						ElseIf dstHasAlpha Then
							dstColor(dstNumComp) = 1.0f
						End If
						pixel = dstCM.getDataElements(dstColor, 0, pixel)
						dstRas.dataElementsnts(x, y, pixel)
					Next x
				Next y
			Else
				' possible non-ICC src, possible CSList, possible non-ICC dst 
				' process per pixel
				Dim numCS As Integer
				If CSList Is Nothing Then
					numCS = 0
				Else
					numCS = CSList.Length
				End If
				Dim dstColor As Single()
				If dstHasAlpha Then
					dstColor = New Single(dstNumComp){}
				Else
					dstColor = New Single(dstNumComp - 1){}
				End If
				Dim spixel As Object = Nothing
				Dim dpixel As Object = Nothing
				Dim color_Renamed As Single() = Nothing
				Dim tmpColor As Single()
				' process each pixel
				For y As Integer = 0 To h - 1
					For x As Integer = 0 To w - 1
						spixel = srcRas.getDataElements(x, y, spixel)
						color_Renamed = srcCM.getNormalizedComponents(spixel, color_Renamed, 0)
						tmpColor = srcColorSpace.toCIEXYZ(color_Renamed)
						For i As Integer = 0 To numCS - 1
							tmpColor = CSList(i).fromCIEXYZ(tmpColor)
							tmpColor = CSList(i).toCIEXYZ(tmpColor)
						Next i
						tmpColor = dstColorSpace.fromCIEXYZ(tmpColor)
						For i As Integer = 0 To dstNumComp - 1
							dstColor(i) = tmpColor(i)
						Next i
						If needSrcAlpha Then
							dstColor(dstNumComp) = color_Renamed(srcNumComp)
						ElseIf dstHasAlpha Then
							dstColor(dstNumComp) = 1.0f
						End If
						dpixel = dstCM.getDataElements(dstColor, 0, dpixel)
						dstRas.dataElementsnts(x, y, dpixel)

					Next x
				Next y
			End If

			Return dst
		End Function

	'     color convert a Raster - handles byte, ushort, int, short, float,
	'       or double transferTypes 
		Private Function nonICCRasterFilter(ByVal src As Raster, ByVal dst As WritableRaster) As WritableRaster

			If CSList.Length <> 2 Then Throw New IllegalArgumentException("Destination ColorSpace is undefined")
			If src.numBands <> CSList(0).numComponents Then Throw New IllegalArgumentException("Numbers of source Raster bands and source color space " & "components do not match")
			If dst Is Nothing Then
				dst = createCompatibleDestRaster(src)
			Else
				If src.height <> dst.height OrElse src.width <> dst.width Then Throw New IllegalArgumentException("Width or height of Rasters do not match")
				If dst.numBands <> CSList(1).numComponents Then Throw New IllegalArgumentException("Numbers of destination Raster bands and destination " & "color space components do not match")
			End If

			If srcMinVals Is Nothing Then getMinMaxValsFromColorSpaces(CSList(0), CSList(1))

			Dim srcSM As SampleModel = src.sampleModel
			Dim dstSM As SampleModel = dst.sampleModel
			Dim srcIsFloat, dstIsFloat As Boolean
			Dim srcTransferType As Integer = src.transferType
			Dim dstTransferType As Integer = dst.transferType
			If (srcTransferType = DataBuffer.TYPE_FLOAT) OrElse (srcTransferType = DataBuffer.TYPE_DOUBLE) Then
				srcIsFloat = True
			Else
				srcIsFloat = False
			End If
			If (dstTransferType = DataBuffer.TYPE_FLOAT) OrElse (dstTransferType = DataBuffer.TYPE_DOUBLE) Then
				dstIsFloat = True
			Else
				dstIsFloat = False
			End If
			Dim w As Integer = src.width
			Dim h As Integer = src.height
			Dim srcNumBands As Integer = src.numBands
			Dim dstNumBands As Integer = dst.numBands
			Dim srcScaleFactor As Single() = Nothing
			Dim dstScaleFactor As Single() = Nothing
			If Not srcIsFloat Then
				srcScaleFactor = New Single(srcNumBands - 1){}
				For i As Integer = 0 To srcNumBands - 1
					If srcTransferType = DataBuffer.TYPE_SHORT Then
						srcScaleFactor(i) = (srcMaxVals(i) - srcMinVals(i)) / 32767.0f
					Else
						srcScaleFactor(i) = (srcMaxVals(i) - srcMinVals(i)) / (CSng((1 << srcSM.getSampleSize(i)) - 1))
					End If
				Next i
			End If
			If Not dstIsFloat Then
				dstScaleFactor = New Single(dstNumBands - 1){}
				For i As Integer = 0 To dstNumBands - 1
					If dstTransferType = DataBuffer.TYPE_SHORT Then
						dstScaleFactor(i) = 32767.0f / (dstMaxVals(i) - dstMinVals(i))
					Else
						dstScaleFactor(i) = (CSng((1 << dstSM.getSampleSize(i)) - 1)) / (dstMaxVals(i) - dstMinVals(i))
					End If
				Next i
			End If
			Dim ys As Integer = src.minY
			Dim yd As Integer = dst.minY
			Dim xs, xd As Integer
			Dim sample As Single
			Dim color_Renamed As Single() = New Single(srcNumBands - 1){}
			Dim tmpColor As Single()
			Dim srcColorSpace As ColorSpace = CSList(0)
			Dim dstColorSpace As ColorSpace = CSList(1)
			' process each pixel
			Dim y As Integer = 0
			Do While y < h
				' get src scanline
				xs = src.minX
				xd = dst.minX
				Dim x As Integer = 0
				Do While x < w
					For i As Integer = 0 To srcNumBands - 1
						sample = src.getSampleFloat(xs, ys, i)
						If Not srcIsFloat Then sample = sample * srcScaleFactor(i) + srcMinVals(i)
						color_Renamed(i) = sample
					Next i
					tmpColor = srcColorSpace.toCIEXYZ(color_Renamed)
					tmpColor = dstColorSpace.fromCIEXYZ(tmpColor)
					For i As Integer = 0 To dstNumBands - 1
						sample = tmpColor(i)
						If Not dstIsFloat Then sample = (sample - dstMinVals(i)) * dstScaleFactor(i)
						dst.sampleple(xd, yd, i, sample)
					Next i
					x += 1
					xs += 1
					xd += 1
				Loop
				y += 1
				ys += 1
				yd += 1
			Loop
			Return dst
		End Function

		Private Sub getMinMaxValsFromProfiles(ByVal srcProfile As ICC_Profile, ByVal dstProfile As ICC_Profile)
			Dim type As Integer = srcProfile.colorSpaceType
			Dim nc As Integer = srcProfile.numComponents
			srcMinVals = New Single(nc - 1){}
			srcMaxVals = New Single(nc - 1){}
			minMaxMax(type, nc, srcMinVals, srcMaxVals)
			type = dstProfile.colorSpaceType
			nc = dstProfile.numComponents
			dstMinVals = New Single(nc - 1){}
			dstMaxVals = New Single(nc - 1){}
			minMaxMax(type, nc, dstMinVals, dstMaxVals)
		End Sub

		Private Sub setMinMax(ByVal type As Integer, ByVal nc As Integer, ByVal minVals As Single(), ByVal maxVals As Single())
			If type = ColorSpace.TYPE_Lab Then
				minVals(0) = 0.0f ' L
				maxVals(0) = 100.0f
				minVals(1) = -128.0f ' a
				maxVals(1) = 127.0f
				minVals(2) = -128.0f ' b
				maxVals(2) = 127.0f
			ElseIf type = ColorSpace.TYPE_XYZ Then
					minVals(2) = 0.0f
						minVals(1) = minVals(2)
						minVals(0) = minVals(1)
					maxVals(2) = 1.0f + (32767.0f/ 32768.0f)
						maxVals(1) = maxVals(2)
						maxVals(0) = maxVals(1)
			Else
				For i As Integer = 0 To nc - 1
					minVals(i) = 0.0f
					maxVals(i) = 1.0f
				Next i
			End If
		End Sub

		Private Sub getMinMaxValsFromColorSpaces(ByVal srcCspace As ColorSpace, ByVal dstCspace As ColorSpace)
			Dim nc As Integer = srcCspace.numComponents
			srcMinVals = New Single(nc - 1){}
			srcMaxVals = New Single(nc - 1){}
			For i As Integer = 0 To nc - 1
				srcMinVals(i) = srcCspace.getMinValue(i)
				srcMaxVals(i) = srcCspace.getMaxValue(i)
			Next i
			nc = dstCspace.numComponents
			dstMinVals = New Single(nc - 1){}
			dstMaxVals = New Single(nc - 1){}
			For i As Integer = 0 To nc - 1
				dstMinVals(i) = dstCspace.getMinValue(i)
				dstMaxVals(i) = dstCspace.getMaxValue(i)
			Next i
		End Sub

	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System

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

Namespace java.awt.Icolor



    ''' 
    ''' <summary>
    ''' The ICC_ColorSpace class is an implementation of the abstract
    ''' ColorSpace class.  This representation of
    ''' device independent and device dependent color spaces is based on the
    ''' International Color Consortium Specification ICC.1:2001-12, File Format for
    ''' Color Profiles (see <A href="http://www.color.org">http://www.color.org</A>).
    ''' <p>
    ''' Typically, a Color or ColorModel would be associated with an ICC
    ''' Profile which is either an input, display, or output profile (see
    ''' the ICC specification).  There are other types of ICC Profiles, e.g.
    ''' abstract profiles, device link profiles, and named color profiles,
    ''' which do not contain information appropriate for representing the color
    ''' space of a color, image, or device (see ICC_Profile).
    ''' Attempting to create an ICC_ColorSpace object from an inappropriate ICC
    ''' Profile is an error.
    ''' <p>
    ''' ICC Profiles represent transformations from the color space of
    ''' the profile (e.g. a monitor) to a Profile Connection Space (PCS).
    ''' Profiles of interest for tagging images or colors have a
    ''' PCS which is one of the device independent
    ''' spaces (one CIEXYZ space and two CIELab spaces) defined in the
    ''' ICC Profile Format Specification.  Most profiles of interest
    ''' either have invertible transformations or explicitly specify
    ''' transformations going both directions.  Should an ICC_ColorSpace
    ''' object be used in a way requiring a conversion from PCS to
    ''' the profile's native space and there is inadequate data to
    ''' correctly perform the conversion, the ICC_ColorSpace object will
    ''' produce output in the specified type of color space (e.g. TYPE_RGB,
    ''' TYPE_CMYK, etc.), but the specific color values of the output data
    ''' will be undefined.
    ''' <p>
    ''' The details of this class are not important for simple applets,
    ''' which draw in a default color space or manipulate and display
    ''' imported images with a known color space.  At most, such applets
    ''' would need to get one of the default color spaces via
    ''' ColorSpace.getInstance(). </summary>
    ''' <seealso cref= ColorSpace </seealso>
    ''' <seealso cref= ICC_Profile </seealso>



    Public Class ICC_ColorSpace
        Inherits ColorSpace

        Friend Shadows Const serialVersionUID As Long = 3455889114070431483L

        Private thisProfile As ICC_Profile
        Private minVal As Single()
        Private maxVal As Single()
        Private diffMinMax As Single()
        Private invDiffMinMax As Single()
        Private needScaleInit As Boolean = True

        ' {to,from}{RGB,CIEXYZ} methods create and cache these when needed
        <NonSerialized>
        Private this2srgb As sun.java2d.cmm.ColorTransform
        <NonSerialized>
        Private srgb2this As sun.java2d.cmm.ColorTransform
        <NonSerialized>
        Private this2xyz As sun.java2d.cmm.ColorTransform
        <NonSerialized>
        Private xyz2this As sun.java2d.cmm.ColorTransform


        ''' <summary>
        ''' Constructs a new ICC_ColorSpace from an ICC_Profile object. </summary>
        ''' <param name="profile"> the specified ICC_Profile object </param>
        ''' <exception cref="IllegalArgumentException"> if profile is inappropriate for
        '''            representing a ColorSpace. </exception>
        Public Sub New(  profile As ICC_Profile)
            MyBase.New(profile.colorSpaceType, profile.numComponents)

            Dim profileClass As Integer = profile.profileClass

            ' REMIND - is NAMEDCOLOR OK? 
            If (profileClass <> ICC_Profile.CLASS_INPUT) AndAlso (profileClass <> ICC_Profile.CLASS_DISPLAY) AndAlso (profileClass <> ICC_Profile.CLASS_OUTPUT) AndAlso (profileClass <> ICC_Profile.CLASS_COLORSPACECONVERSION) AndAlso (profileClass <> ICC_Profile.CLASS_NAMEDCOLOR) AndAlso (profileClass <> ICC_Profile.CLASS_ABSTRACT) Then Throw New IllegalArgumentException("Invalid profile type")

            thisProfile = profile
            minMaxMax()
        End Sub

        ''' <summary>
        ''' Returns the ICC_Profile for this ICC_ColorSpace. </summary>
        ''' <returns> the ICC_Profile for this ICC_ColorSpace. </returns>
        Public Overridable ReadOnly Property profile As ICC_Profile
            Get
                Return thisProfile
            End Get
        End Property

        ''' <summary>
        ''' Transforms a color value assumed to be in this ColorSpace
        ''' into a value in the default CS_sRGB color space.
        ''' <p>
        ''' This method transforms color values using algorithms designed
        ''' to produce the best perceptual match between input and output
        ''' colors.  In order to do colorimetric conversion of color values,
        ''' you should use the <code>toCIEXYZ</code>
        ''' method of this color space to first convert from the input
        ''' color space to the CS_CIEXYZ color space, and then use the
        ''' <code>fromCIEXYZ</code> method of the CS_sRGB color space to
        ''' convert from CS_CIEXYZ to the output color space.
        ''' See <seealso cref="#toCIEXYZ(float[]) toCIEXYZ"/> and
        ''' <seealso cref="#fromCIEXYZ(float[]) fromCIEXYZ"/> for further information.
        ''' <p> </summary>
        ''' <param name="colorvalue"> a float array with length of at least the number
        '''      of components in this ColorSpace. </param>
        ''' <returns> a float array of length 3. </returns>
        ''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
        ''' at least the number of components in this ColorSpace. </exception>
        Public Overrides Function toRGB(  colorvalue As Single()) As Single()

            If this2srgb Is Nothing Then
                Dim transformList As sun.java2d.cmm.ColorTransform() = New sun.java2d.cmm.ColorTransform(1) {}
                Dim srgbCS As ICC_ColorSpace = CType(ColorSpace.getInstance(CS_sRGB), ICC_ColorSpace)
                Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module
                transformList(0) = mdl.createTransform(thisProfile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.In)
                transformList(1) = mdl.createTransform(srgbCS.profile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.Out)
                this2srgb = mdl.createTransform(transformList)
                If needScaleInit Then componentScalinging()
            End If

            Dim nc As Integer = Me.numComponents
            Dim tmp As Short() = New Short(nc - 1) {}
            For i As Integer = 0 To nc - 1
                tmp(i) = CShort(Fix((colorvalue(i) - minVal(i)) * invDiffMinMax(i) + 0.5F))
            Next i
            tmp = this2srgb.colorConvert(tmp, Nothing)
            Dim result As Single() = New Single(2) {}
            For i As Integer = 0 To 2
                result(i) = (CSng(tmp(i) And &HFFFF)) / 65535.0F
            Next i
            Return result
        End Function

        ''' <summary>
        ''' Transforms a color value assumed to be in the default CS_sRGB
        ''' color space into this ColorSpace.
        ''' <p>
        ''' This method transforms color values using algorithms designed
        ''' to produce the best perceptual match between input and output
        ''' colors.  In order to do colorimetric conversion of color values,
        ''' you should use the <code>toCIEXYZ</code>
        ''' method of the CS_sRGB color space to first convert from the input
        ''' color space to the CS_CIEXYZ color space, and then use the
        ''' <code>fromCIEXYZ</code> method of this color space to
        ''' convert from CS_CIEXYZ to the output color space.
        ''' See <seealso cref="#toCIEXYZ(float[]) toCIEXYZ"/> and
        ''' <seealso cref="#fromCIEXYZ(float[]) fromCIEXYZ"/> for further information.
        ''' <p> </summary>
        ''' <param name="rgbvalue"> a float array with length of at least 3. </param>
        ''' <returns> a float array with length equal to the number of
        '''       components in this ColorSpace. </returns>
        ''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
        ''' at least 3. </exception>
        Public Overrides Function fromRGB(  rgbvalue As Single()) As Single()

            If srgb2this Is Nothing Then
                Dim transformList As sun.java2d.cmm.ColorTransform() = New sun.java2d.cmm.ColorTransform(1) {}
                Dim srgbCS As ICC_ColorSpace = CType(ColorSpace.getInstance(CS_sRGB), ICC_ColorSpace)
                Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module
                transformList(0) = mdl.createTransform(srgbCS.profile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.In)
                transformList(1) = mdl.createTransform(thisProfile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.Out)
                srgb2this = mdl.createTransform(transformList)
                If needScaleInit Then componentScalinging()
            End If

            Dim tmp As Short() = New Short(2) {}
            For i As Integer = 0 To 2
                tmp(i) = CShort(Fix((rgbvalue(i) * 65535.0F) + 0.5F))
            Next i
            tmp = srgb2this.colorConvert(tmp, Nothing)
            Dim nc As Integer = Me.numComponents
            Dim result As Single() = New Single(nc - 1) {}
            For i As Integer = 0 To nc - 1
                result(i) = ((CSng(tmp(i) And &HFFFF)) / 65535.0F) * diffMinMax(i) + minVal(i)
            Next i
            Return result
        End Function


        ''' <summary>
        ''' Transforms a color value assumed to be in this ColorSpace
        ''' into the CS_CIEXYZ conversion color space.
        ''' <p>
        ''' This method transforms color values using relative colorimetry,
        ''' as defined by the ICC Specification.  This
        ''' means that the XYZ values returned by this method are represented
        ''' relative to the D50 white point of the CS_CIEXYZ color space.
        ''' This representation is useful in a two-step color conversion
        ''' process in which colors are transformed from an input color
        ''' space to CS_CIEXYZ and then to an output color space.  This
        ''' representation is not the same as the XYZ values that would
        ''' be measured from the given color value by a colorimeter.
        ''' A further transformation is necessary to compute the XYZ values
        ''' that would be measured using current CIE recommended practices.
        ''' The paragraphs below explain this in more detail.
        ''' <p>
        ''' The ICC standard uses a device independent color space (DICS) as the
        ''' mechanism for converting color from one device to another device.  In
        ''' this architecture, colors are converted from the source device's color
        ''' space to the ICC DICS and then from the ICC DICS to the destination
        ''' device's color space.  The ICC standard defines device profiles which
        ''' contain transforms which will convert between a device's color space
        ''' and the ICC DICS.  The overall conversion of colors from a source
        ''' device to colors of a destination device is done by connecting the
        ''' device-to-DICS transform of the profile for the source device to the
        ''' DICS-to-device transform of the profile for the destination device.
        ''' For this reason, the ICC DICS is commonly referred to as the profile
        ''' connection space (PCS).  The color space used in the methods
        ''' toCIEXYZ and fromCIEXYZ is the CIEXYZ PCS defined by the ICC
        ''' Specification.  This is also the color space represented by
        ''' ColorSpace.CS_CIEXYZ.
        ''' <p>
        ''' The XYZ values of a color are often represented as relative to some
        ''' white point, so the actual meaning of the XYZ values cannot be known
        ''' without knowing the white point of those values.  This is known as
        ''' relative colorimetry.  The PCS uses a white point of D50, so the XYZ
        ''' values of the PCS are relative to D50.  For example, white in the PCS
        ''' will have the XYZ values of D50, which is defined to be X=.9642,
        ''' Y=1.000, and Z=0.8249.  This white point is commonly used for graphic
        ''' arts applications, but others are often used in other applications.
        ''' <p>
        ''' To quantify the color characteristics of a device such as a printer
        ''' or monitor, measurements of XYZ values for particular device colors
        ''' are typically made.  For purposes of this discussion, the term
        ''' device XYZ values is used to mean the XYZ values that would be
        ''' measured from device colors using current CIE recommended practices.
        ''' <p>
        ''' Converting between device XYZ values and the PCS XYZ values returned
        ''' by this method corresponds to converting between the device's color
        ''' space, as represented by CIE colorimetric values, and the PCS.  There
        ''' are many factors involved in this process, some of which are quite
        ''' subtle.  The most important, however, is the adjustment made to account
        ''' for differences between the device's white point and the white point of
        ''' the PCS.  There are many techniques for doing this and it is the
        ''' subject of much current research and controversy.  Some commonly used
        ''' methods are XYZ scaling, the von Kries transform, and the Bradford
        ''' transform.  The proper method to use depends upon each particular
        ''' application.
        ''' <p>
        ''' The simplest method is XYZ scaling.  In this method each device XYZ
        ''' value is  converted to a PCS XYZ value by multiplying it by the ratio
        ''' of the PCS white point (D50) to the device white point.
        ''' <pre>
        ''' 
        ''' Xd, Yd, Zd are the device XYZ values
        ''' Xdw, Ydw, Zdw are the device XYZ white point values
        ''' Xp, Yp, Zp are the PCS XYZ values
        ''' Xd50, Yd50, Zd50 are the PCS XYZ white point values
        ''' 
        ''' Xp = Xd * (Xd50 / Xdw)
        ''' Yp = Yd * (Yd50 / Ydw)
        ''' Zp = Zd * (Zd50 / Zdw)
        ''' 
        ''' </pre>
        ''' <p>
        ''' Conversion from the PCS to the device would be done by inverting these
        ''' equations:
        ''' <pre>
        ''' 
        ''' Xd = Xp * (Xdw / Xd50)
        ''' Yd = Yp * (Ydw / Yd50)
        ''' Zd = Zp * (Zdw / Zd50)
        ''' 
        ''' </pre>
        ''' <p>
        ''' Note that the media white point tag in an ICC profile is not the same
        ''' as the device white point.  The media white point tag is expressed in
        ''' PCS values and is used to represent the difference between the XYZ of
        ''' device illuminant and the XYZ of the device media when measured under
        ''' that illuminant.  The device white point is expressed as the device
        ''' XYZ values corresponding to white displayed on the device.  For
        ''' example, displaying the RGB color (1.0, 1.0, 1.0) on an sRGB device
        ''' will result in a measured device XYZ value of D65.  This will not
        ''' be the same as the media white point tag XYZ value in the ICC
        ''' profile for an sRGB device.
        ''' <p> </summary>
        ''' <param name="colorvalue"> a float array with length of at least the number
        '''        of components in this ColorSpace. </param>
        ''' <returns> a float array of length 3. </returns>
        ''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
        ''' at least the number of components in this ColorSpace. </exception>
        Public Overrides Function toCIEXYZ(  colorvalue As Single()) As Single()

            If this2xyz Is Nothing Then
                Dim transformList As sun.java2d.cmm.ColorTransform() = New sun.java2d.cmm.ColorTransform(1) {}
                Dim xyzCS As ICC_ColorSpace = CType(ColorSpace.getInstance(CS_CIEXYZ), ICC_ColorSpace)
                Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module
                Try
                    transformList(0) = mdl.createTransform(thisProfile, ICC_Profile.icRelativeColorimetric, sun.java2d.cmm.ColorTransform.In)
                Catch e As CMMException
                    transformList(0) = mdl.createTransform(thisProfile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.In)
                End Try
                transformList(1) = mdl.createTransform(xyzCS.profile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.Out)
                this2xyz = mdl.createTransform(transformList)
                If needScaleInit Then componentScalinging()
            End If

            Dim nc As Integer = Me.numComponents
            Dim tmp As Short() = New Short(nc - 1) {}
            For i As Integer = 0 To nc - 1
                tmp(i) = CShort(Fix((colorvalue(i) - minVal(i)) * invDiffMinMax(i) + 0.5F))
            Next i
            tmp = this2xyz.colorConvert(tmp, Nothing)
            Dim ALMOST_TWO As Single = 1.0F + (32767.0F / 32768.0F)
            ' For CIEXYZ, min = 0.0, max = ALMOST_TWO for all components
            Dim result As Single() = New Single(2) {}
            For i As Integer = 0 To 2
                result(i) = ((CSng(tmp(i) And &HFFFF)) / 65535.0F) * ALMOST_TWO
            Next i
            Return result
        End Function


        ''' <summary>
        ''' Transforms a color value assumed to be in the CS_CIEXYZ conversion
        ''' color space into this ColorSpace.
        ''' <p>
        ''' This method transforms color values using relative colorimetry,
        ''' as defined by the ICC Specification.  This
        ''' means that the XYZ argument values taken by this method are represented
        ''' relative to the D50 white point of the CS_CIEXYZ color space.
        ''' This representation is useful in a two-step color conversion
        ''' process in which colors are transformed from an input color
        ''' space to CS_CIEXYZ and then to an output color space.  The color
        ''' values returned by this method are not those that would produce
        ''' the XYZ value passed to the method when measured by a colorimeter.
        ''' If you have XYZ values corresponding to measurements made using
        ''' current CIE recommended practices, they must be converted to D50
        ''' relative values before being passed to this method.
        ''' The paragraphs below explain this in more detail.
        ''' <p>
        ''' The ICC standard uses a device independent color space (DICS) as the
        ''' mechanism for converting color from one device to another device.  In
        ''' this architecture, colors are converted from the source device's color
        ''' space to the ICC DICS and then from the ICC DICS to the destination
        ''' device's color space.  The ICC standard defines device profiles which
        ''' contain transforms which will convert between a device's color space
        ''' and the ICC DICS.  The overall conversion of colors from a source
        ''' device to colors of a destination device is done by connecting the
        ''' device-to-DICS transform of the profile for the source device to the
        ''' DICS-to-device transform of the profile for the destination device.
        ''' For this reason, the ICC DICS is commonly referred to as the profile
        ''' connection space (PCS).  The color space used in the methods
        ''' toCIEXYZ and fromCIEXYZ is the CIEXYZ PCS defined by the ICC
        ''' Specification.  This is also the color space represented by
        ''' ColorSpace.CS_CIEXYZ.
        ''' <p>
        ''' The XYZ values of a color are often represented as relative to some
        ''' white point, so the actual meaning of the XYZ values cannot be known
        ''' without knowing the white point of those values.  This is known as
        ''' relative colorimetry.  The PCS uses a white point of D50, so the XYZ
        ''' values of the PCS are relative to D50.  For example, white in the PCS
        ''' will have the XYZ values of D50, which is defined to be X=.9642,
        ''' Y=1.000, and Z=0.8249.  This white point is commonly used for graphic
        ''' arts applications, but others are often used in other applications.
        ''' <p>
        ''' To quantify the color characteristics of a device such as a printer
        ''' or monitor, measurements of XYZ values for particular device colors
        ''' are typically made.  For purposes of this discussion, the term
        ''' device XYZ values is used to mean the XYZ values that would be
        ''' measured from device colors using current CIE recommended practices.
        ''' <p>
        ''' Converting between device XYZ values and the PCS XYZ values taken as
        ''' arguments by this method corresponds to converting between the device's
        ''' color space, as represented by CIE colorimetric values, and the PCS.
        ''' There are many factors involved in this process, some of which are quite
        ''' subtle.  The most important, however, is the adjustment made to account
        ''' for differences between the device's white point and the white point of
        ''' the PCS.  There are many techniques for doing this and it is the
        ''' subject of much current research and controversy.  Some commonly used
        ''' methods are XYZ scaling, the von Kries transform, and the Bradford
        ''' transform.  The proper method to use depends upon each particular
        ''' application.
        ''' <p>
        ''' The simplest method is XYZ scaling.  In this method each device XYZ
        ''' value is  converted to a PCS XYZ value by multiplying it by the ratio
        ''' of the PCS white point (D50) to the device white point.
        ''' <pre>
        ''' 
        ''' Xd, Yd, Zd are the device XYZ values
        ''' Xdw, Ydw, Zdw are the device XYZ white point values
        ''' Xp, Yp, Zp are the PCS XYZ values
        ''' Xd50, Yd50, Zd50 are the PCS XYZ white point values
        ''' 
        ''' Xp = Xd * (Xd50 / Xdw)
        ''' Yp = Yd * (Yd50 / Ydw)
        ''' Zp = Zd * (Zd50 / Zdw)
        ''' 
        ''' </pre>
        ''' <p>
        ''' Conversion from the PCS to the device would be done by inverting these
        ''' equations:
        ''' <pre>
        ''' 
        ''' Xd = Xp * (Xdw / Xd50)
        ''' Yd = Yp * (Ydw / Yd50)
        ''' Zd = Zp * (Zdw / Zd50)
        ''' 
        ''' </pre>
        ''' <p>
        ''' Note that the media white point tag in an ICC profile is not the same
        ''' as the device white point.  The media white point tag is expressed in
        ''' PCS values and is used to represent the difference between the XYZ of
        ''' device illuminant and the XYZ of the device media when measured under
        ''' that illuminant.  The device white point is expressed as the device
        ''' XYZ values corresponding to white displayed on the device.  For
        ''' example, displaying the RGB color (1.0, 1.0, 1.0) on an sRGB device
        ''' will result in a measured device XYZ value of D65.  This will not
        ''' be the same as the media white point tag XYZ value in the ICC
        ''' profile for an sRGB device.
        ''' <p> </summary>
        ''' <param name="colorvalue"> a float array with length of at least 3. </param>
        ''' <returns> a float array with length equal to the number of
        '''         components in this ColorSpace. </returns>
        ''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
        ''' at least 3. </exception>
        Public Overrides Function fromCIEXYZ(  colorvalue As Single()) As Single()

            If xyz2this Is Nothing Then
                Dim transformList As sun.java2d.cmm.ColorTransform() = New sun.java2d.cmm.ColorTransform(1) {}
                Dim xyzCS As ICC_ColorSpace = CType(ColorSpace.getInstance(CS_CIEXYZ), ICC_ColorSpace)
                Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module
                transformList(0) = mdl.createTransform(xyzCS.profile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.In)
                Try
                    transformList(1) = mdl.createTransform(thisProfile, ICC_Profile.icRelativeColorimetric, sun.java2d.cmm.ColorTransform.Out)
                Catch e As CMMException
                    transformList(1) = sun.java2d.cmm.CMSManager.module.createTransform(thisProfile, sun.java2d.cmm.ColorTransform.Any, sun.java2d.cmm.ColorTransform.Out)
                End Try
                xyz2this = mdl.createTransform(transformList)
                If needScaleInit Then componentScalinging()
            End If

            Dim tmp As Short() = New Short(2) {}
            Dim ALMOST_TWO As Single = 1.0F + (32767.0F / 32768.0F)
            Dim factor As Single = 65535.0F / ALMOST_TWO
            ' For CIEXYZ, min = 0.0, max = ALMOST_TWO for all components
            For i As Integer = 0 To 2
                tmp(i) = CShort(Fix((colorvalue(i) * factor) + 0.5F))
            Next i
            tmp = xyz2this.colorConvert(tmp, Nothing)
            Dim nc As Integer = Me.numComponents
            Dim result As Single() = New Single(nc - 1) {}
            For i As Integer = 0 To nc - 1
                result(i) = ((CSng(tmp(i) And &HFFFF)) / 65535.0F) * diffMinMax(i) + minVal(i)
            Next i
            Return result
        End Function

        ''' <summary>
        ''' Returns the minimum normalized color component value for the
        ''' specified component.  For TYPE_XYZ spaces, this method returns
        ''' minimum values of 0.0 for all components.  For TYPE_Lab spaces,
        ''' this method returns 0.0 for L and -128.0 for a and b components.
        ''' This is consistent with the encoding of the XYZ and Lab Profile
        ''' Connection Spaces in the ICC specification.  For all other types, this
        ''' method returns 0.0 for all components.  When using an ICC_ColorSpace
        ''' with a profile that requires different minimum component values,
        ''' it is necessary to subclass this class and override this method. </summary>
        ''' <param name="component"> The component index. </param>
        ''' <returns> The minimum normalized component value. </returns>
        ''' <exception cref="IllegalArgumentException"> if component is less than 0 or
        '''         greater than numComponents - 1.
        ''' @since 1.4 </exception>
        Public Overrides Function getMinValue(  component_Renamed As Integer) As Single
            If (component_Renamed < 0) OrElse (component_Renamed > Me.numComponents - 1) Then Throw New IllegalArgumentException("Component index out of range: + component")
            Return minVal(component_Renamed)
        End Function

        ''' <summary>
        ''' Returns the maximum normalized color component value for the
        ''' specified component.  For TYPE_XYZ spaces, this method returns
        ''' maximum values of 1.0 + (32767.0 / 32768.0) for all components.
        ''' For TYPE_Lab spaces,
        ''' this method returns 100.0 for L and 127.0 for a and b components.
        ''' This is consistent with the encoding of the XYZ and Lab Profile
        ''' Connection Spaces in the ICC specification.  For all other types, this
        ''' method returns 1.0 for all components.  When using an ICC_ColorSpace
        ''' with a profile that requires different maximum component values,
        ''' it is necessary to subclass this class and override this method. </summary>
        ''' <param name="component"> The component index. </param>
        ''' <returns> The maximum normalized component value. </returns>
        ''' <exception cref="IllegalArgumentException"> if component is less than 0 or
        '''         greater than numComponents - 1.
        ''' @since 1.4 </exception>
        Public Overrides Function getMaxValue(  component_Renamed As Integer) As Single
            If (component_Renamed < 0) OrElse (component_Renamed > Me.numComponents - 1) Then Throw New IllegalArgumentException("Component index out of range: + component")
            Return maxVal(component_Renamed)
        End Function

        Private Sub setMinMax()
            Dim nc As Integer = Me.numComponents
            Dim type_Renamed As Integer = Me.type
            minVal = New Single(nc - 1) {}
            maxVal = New Single(nc - 1) {}
            If type_Renamed = ColorSpace.TYPE_Lab Then
                minVal(0) = 0.0F ' L
                maxVal(0) = 100.0F
                minVal(1) = -128.0F ' a
                maxVal(1) = 127.0F
                minVal(2) = -128.0F ' b
                maxVal(2) = 127.0F
            ElseIf type_Renamed = ColorSpace.TYPE_XYZ Then
                minVal(2) = 0.0F
                minVal(1) = minVal(2)
                minVal(0) = minVal(1)
                maxVal(2) = 1.0F + (32767.0F / 32768.0F)
                maxVal(1) = maxVal(2)
                maxVal(0) = maxVal(1)
            Else
                For i As Integer = 0 To nc - 1
                    minVal(i) = 0.0F
                    maxVal(i) = 1.0F
                Next i
            End If
        End Sub

        Private Sub setComponentScaling()
            Dim nc As Integer = Me.numComponents
            diffMinMax = New Single(nc - 1) {}
            invDiffMinMax = New Single(nc - 1) {}
            For i As Integer = 0 To nc - 1
                minVal(i) = Me.getMinValue(i) ' in case getMinVal is overridden
                maxVal(i) = Me.getMaxValue(i) ' in case getMaxVal is overridden
                diffMinMax(i) = maxVal(i) - minVal(i)
                invDiffMinMax(i) = 65535.0F / diffMinMax(i)
            Next i
            needScaleInit = False
        End Sub

    End Class

End Namespace
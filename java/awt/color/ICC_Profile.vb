Imports Microsoft.VisualBasic
Imports System
Imports java.security
Imports java.io

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






    ''' <summary>
    ''' A representation of color profile data for device independent and
    ''' device dependent color spaces based on the International Color
    ''' Consortium Specification ICC.1:2001-12, File Format for Color Profiles,
    ''' (see <A href="http://www.color.org"> http://www.color.org</A>).
    ''' <p>
    ''' An ICC_ColorSpace object can be constructed from an appropriate
    ''' ICC_Profile.
    ''' Typically, an ICC_ColorSpace would be associated with an ICC
    ''' Profile which is either an input, display, or output profile (see
    ''' the ICC specification).  There are also device link, abstract,
    ''' color space conversion, and named color profiles.  These are less
    ''' useful for tagging a color or image, but are useful for other
    ''' purposes (in particular device link profiles can provide improved
    ''' performance for converting from one device's color space to
    ''' another's).
    ''' <p>
    ''' ICC Profiles represent transformations from the color space of
    ''' the profile (e.g. a monitor) to a Profile Connection Space (PCS).
    ''' Profiles of interest for tagging images or colors have a PCS
    ''' which is one of the two specific device independent
    ''' spaces (one CIEXYZ space and one CIELab space) defined in the
    ''' ICC Profile Format Specification.  Most profiles of interest
    ''' either have invertible transformations or explicitly specify
    ''' transformations going both directions. </summary>
    ''' <seealso cref= ICC_ColorSpace </seealso>


    <Serializable>
    Public Class ICC_Profile

        Private Const serialVersionUID As Long = -3938515861990936766L

        <NonSerialized>
        Private cmmProfile As sun.java2d.cmm.Profile

        <NonSerialized>
        Private deferralInfo As sun.java2d.cmm.ProfileDeferralInfo
        <NonSerialized>
        Private profileActivator As sun.java2d.cmm.ProfileActivator

        ' Registry of singleton profile objects for specific color spaces
        ' defined in the ColorSpace class (e.g. CS_sRGB), see
        ' getInstance(int cspace) factory method.
        Private Shared sRGBprofile As ICC_Profile
        Private Shared XYZprofile As ICC_Profile
        Private Shared PYCCprofile As ICC_Profile
        Private Shared GRAYprofile As ICC_Profile
        Private Shared LINEAR_RGBprofile As ICC_Profile


        ''' <summary>
        ''' Profile class is input.
        ''' </summary>
        Public Const CLASS_INPUT As Integer = 0

        ''' <summary>
        ''' Profile class is display.
        ''' </summary>
        Public Const CLASS_DISPLAY As Integer = 1

        ''' <summary>
        ''' Profile class is output.
        ''' </summary>
        Public Const CLASS_OUTPUT As Integer = 2

        ''' <summary>
        ''' Profile class is device link.
        ''' </summary>
        Public Const CLASS_DEVICELINK As Integer = 3

        ''' <summary>
        ''' Profile class is color space conversion.
        ''' </summary>
        Public Const CLASS_COLORSPACECONVERSION As Integer = 4

        ''' <summary>
        ''' Profile class is abstract.
        ''' </summary>
        Public Const CLASS_ABSTRACT As Integer = 5

        ''' <summary>
        ''' Profile class is named color.
        ''' </summary>
        Public Const CLASS_NAMEDCOLOR As Integer = 6


        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'XYZ '.
        ''' </summary>
        Public Const icSigXYZData As Integer = &H58595A20 ' 'XYZ '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'Lab '.
        ''' </summary>
        Public Const icSigLabData As Integer = &H4C616220 ' 'Lab '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'Luv '.
        ''' </summary>
        Public Const icSigLuvData As Integer = &H4C757620 ' 'Luv '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'YCbr'.
        ''' </summary>
        Public Const icSigYCbCrData As Integer = &H59436272 ' 'YCbr'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'Yxy '.
        ''' </summary>
        Public Const icSigYxyData As Integer = &H59787920 ' 'Yxy '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'RGB '.
        ''' </summary>
        Public Const icSigRgbData As Integer = &H52474220 ' 'RGB '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'GRAY'.
        ''' </summary>
        Public Const icSigGrayData As Integer = &H47524159 ' 'GRAY'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'HSV'.
        ''' </summary>
        Public Const icSigHsvData As Integer = &H48535620 ' 'HSV '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'HLS'.
        ''' </summary>
        Public Const icSigHlsData As Integer = &H484C5320 ' 'HLS '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'CMYK'.
        ''' </summary>
        Public Const icSigCmykData As Integer = &H434D594B ' 'CMYK'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'CMY '.
        ''' </summary>
        Public Const icSigCmyData As Integer = &H434D5920 ' 'CMY '

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '2CLR'.
        ''' </summary>
        Public Const icSigSpace2CLR As Integer = &H32434C52 ' '2CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '3CLR'.
        ''' </summary>
        Public Const icSigSpace3CLR As Integer = &H33434C52 ' '3CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '4CLR'.
        ''' </summary>
        Public Const icSigSpace4CLR As Integer = &H34434C52 ' '4CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '5CLR'.
        ''' </summary>
        Public Const icSigSpace5CLR As Integer = &H35434C52 ' '5CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '6CLR'.
        ''' </summary>
        Public Const icSigSpace6CLR As Integer = &H36434C52 ' '6CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '7CLR'.
        ''' </summary>
        Public Const icSigSpace7CLR As Integer = &H37434C52 ' '7CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '8CLR'.
        ''' </summary>
        Public Const icSigSpace8CLR As Integer = &H38434C52 ' '8CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: '9CLR'.
        ''' </summary>
        Public Const icSigSpace9CLR As Integer = &H39434C52 ' '9CLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'ACLR'.
        ''' </summary>
        Public Const icSigSpaceACLR As Integer = &H41434C52 ' 'ACLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'BCLR'.
        ''' </summary>
        Public Const icSigSpaceBCLR As Integer = &H42434C52 ' 'BCLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'CCLR'.
        ''' </summary>
        Public Const icSigSpaceCCLR As Integer = &H43434C52 ' 'CCLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'DCLR'.
        ''' </summary>
        Public Const icSigSpaceDCLR As Integer = &H44434C52 ' 'DCLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'ECLR'.
        ''' </summary>
        Public Const icSigSpaceECLR As Integer = &H45434C52 ' 'ECLR'

        ''' <summary>
        ''' ICC Profile Color Space Type Signature: 'FCLR'.
        ''' </summary>
        Public Const icSigSpaceFCLR As Integer = &H46434C52 ' 'FCLR'


        ''' <summary>
        ''' ICC Profile Class Signature: 'scnr'.
        ''' </summary>
        Public Const icSigInputClass As Integer = &H73636E72 ' 'scnr'

        ''' <summary>
        ''' ICC Profile Class Signature: 'mntr'.
        ''' </summary>
        Public Const icSigDisplayClass As Integer = &H6D6E7472 ' 'mntr'

        ''' <summary>
        ''' ICC Profile Class Signature: 'prtr'.
        ''' </summary>
        Public Const icSigOutputClass As Integer = &H70727472 ' 'prtr'

        ''' <summary>
        ''' ICC Profile Class Signature: 'link'.
        ''' </summary>
        Public Const icSigLinkClass As Integer = &H6C696E6B ' 'link'

        ''' <summary>
        ''' ICC Profile Class Signature: 'abst'.
        ''' </summary>
        Public Const icSigAbstractClass As Integer = &H61627374 ' 'abst'

        ''' <summary>
        ''' ICC Profile Class Signature: 'spac'.
        ''' </summary>
        Public Const icSigColorSpaceClass As Integer = &H73706163 ' 'spac'

        ''' <summary>
        ''' ICC Profile Class Signature: 'nmcl'.
        ''' </summary>
        Public Const icSigNamedColorClass As Integer = &H6E6D636C ' 'nmcl'


        ''' <summary>
        ''' ICC Profile Rendering Intent: Perceptual.
        ''' </summary>
        Public Const icPerceptual As Integer = 0

        ''' <summary>
        ''' ICC Profile Rendering Intent: RelativeColorimetric.
        ''' </summary>
        Public Const icRelativeColorimetric As Integer = 1

        ''' <summary>
        ''' ICC Profile Rendering Intent: Media-RelativeColorimetric.
        ''' @since 1.5
        ''' </summary>
        Public Const icMediaRelativeColorimetric As Integer = 1

        ''' <summary>
        ''' ICC Profile Rendering Intent: Saturation.
        ''' </summary>
        Public Const icSaturation As Integer = 2

        ''' <summary>
        ''' ICC Profile Rendering Intent: AbsoluteColorimetric.
        ''' </summary>
        Public Const icAbsoluteColorimetric As Integer = 3

        ''' <summary>
        ''' ICC Profile Rendering Intent: ICC-AbsoluteColorimetric.
        ''' @since 1.5
        ''' </summary>
        Public Const icICCAbsoluteColorimetric As Integer = 3


        ''' <summary>
        ''' ICC Profile Tag Signature: 'head' - special.
        ''' </summary>
        Public Const icSigHead As Integer = &H68656164 ' 'head' - special

        ''' <summary>
        ''' ICC Profile Tag Signature: 'A2B0'.
        ''' </summary>
        Public Const icSigAToB0Tag As Integer = &H41324230 ' 'A2B0'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'A2B1'.
        ''' </summary>
        Public Const icSigAToB1Tag As Integer = &H41324231 ' 'A2B1'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'A2B2'.
        ''' </summary>
        Public Const icSigAToB2Tag As Integer = &H41324232 ' 'A2B2'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'bXYZ'.
        ''' </summary>
        Public Const icSigBlueColorantTag As Integer = &H6258595A ' 'bXYZ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'bXYZ'.
        ''' @since 1.5
        ''' </summary>
        Public Const icSigBlueMatrixColumnTag As Integer = &H6258595A ' 'bXYZ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'bTRC'.
        ''' </summary>
        Public Const icSigBlueTRCTag As Integer = &H62545243 ' 'bTRC'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'B2A0'.
        ''' </summary>
        Public Const icSigBToA0Tag As Integer = &H42324130 ' 'B2A0'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'B2A1'.
        ''' </summary>
        Public Const icSigBToA1Tag As Integer = &H42324131 ' 'B2A1'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'B2A2'.
        ''' </summary>
        Public Const icSigBToA2Tag As Integer = &H42324132 ' 'B2A2'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'calt'.
        ''' </summary>
        Public Const icSigCalibrationDateTimeTag As Integer = &H63616C74
        ' 'calt' 

        ''' <summary>
        ''' ICC Profile Tag Signature: 'targ'.
        ''' </summary>
        Public Const icSigCharTargetTag As Integer = &H74617267 ' 'targ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'cprt'.
        ''' </summary>
        Public Const icSigCopyrightTag As Integer = &H63707274 ' 'cprt'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'crdi'.
        ''' </summary>
        Public Const icSigCrdInfoTag As Integer = &H63726469 ' 'crdi'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'dmnd'.
        ''' </summary>
        Public Const icSigDeviceMfgDescTag As Integer = &H646D6E64 ' 'dmnd'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'dmdd'.
        ''' </summary>
        Public Const icSigDeviceModelDescTag As Integer = &H646D6464 ' 'dmdd'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'devs'.
        ''' </summary>
        Public Const icSigDeviceSettingsTag As Integer = &H64657673 ' 'devs'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'gamt'.
        ''' </summary>
        Public Const icSigGamutTag As Integer = &H67616D74 ' 'gamt'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'kTRC'.
        ''' </summary>
        Public Const icSigGrayTRCTag As Integer = &H6B545243 ' 'kTRC'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'gXYZ'.
        ''' </summary>
        Public Const icSigGreenColorantTag As Integer = &H6758595A ' 'gXYZ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'gXYZ'.
        ''' @since 1.5
        ''' </summary>
        Public Const icSigGreenMatrixColumnTag As Integer = &H6758595A ' 'gXYZ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'gTRC'.
        ''' </summary>
        Public Const icSigGreenTRCTag As Integer = &H67545243 ' 'gTRC'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'lumi'.
        ''' </summary>
        Public Const icSigLuminanceTag As Integer = &H6C756R69 ' 'lumi'

		''' <summary>
		''' ICC Profile Tag Signature: 'meas'.
		''' </summary>
		Public Const icSigMeasurementTag As Integer = &H6D656173 ' 'meas'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'bkpt'.
        ''' </summary>
        Public Const icSigMediaBlackPointTag As Integer = &H626B7074 ' 'bkpt'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'wtpt'.
        ''' </summary>
        Public Const icSigMediaWhitePointTag As Integer = &H77747074 ' 'wtpt'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'ncl2'.
        ''' </summary>
        Public Const icSigNamedColor2Tag As Integer = &H6E636C32 ' 'ncl2'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'resp'.
        ''' </summary>
        Public Const icSigOutputResponseTag As Integer = &H72657370 ' 'resp'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'pre0'.
        ''' </summary>
        Public Const icSigPreview0Tag As Integer = &H70726530 ' 'pre0'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'pre1'.
        ''' </summary>
        Public Const icSigPreview1Tag As Integer = &H70726531 ' 'pre1'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'pre2'.
        ''' </summary>
        Public Const icSigPreview2Tag As Integer = &H70726532 ' 'pre2'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'desc'.
        ''' </summary>
        Public Const icSigProfileDescriptionTag As Integer = &H64657363
        ' 'desc' 

        ''' <summary>
        ''' ICC Profile Tag Signature: 'pseq'.
        ''' </summary>
        Public Const icSigProfileSequenceDescTag As Integer = &H70736571
        ' 'pseq' 

        ''' <summary>
        ''' ICC Profile Tag Signature: 'psd0'.
        ''' </summary>
        Public Const icSigPs2CRD0Tag As Integer = &H70736430 ' 'psd0'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'psd1'.
        ''' </summary>
        Public Const icSigPs2CRD1Tag As Integer = &H70736431 ' 'psd1'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'psd2'.
        ''' </summary>
        Public Const icSigPs2CRD2Tag As Integer = &H70736432 ' 'psd2'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'psd3'.
        ''' </summary>
        Public Const icSigPs2CRD3Tag As Integer = &H70736433 ' 'psd3'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'ps2s'.
        ''' </summary>
        Public Const icSigPs2CSATag As Integer = &H70733273 ' 'ps2s'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'ps2i'.
        ''' </summary>
        Public Const icSigPs2RenderingIntentTag As Integer = &H70733269
        ' 'ps2i' 

        ''' <summary>
        ''' ICC Profile Tag Signature: 'rXYZ'.
        ''' </summary>
        Public Const icSigRedColorantTag As Integer = &H7258595A ' 'rXYZ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'rXYZ'.
        ''' @since 1.5
        ''' </summary>
        Public Const icSigRedMatrixColumnTag As Integer = &H7258595A ' 'rXYZ'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'rTRC'.
        ''' </summary>
        Public Const icSigRedTRCTag As Integer = &H72545243 ' 'rTRC'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'scrd'.
        ''' </summary>
        Public Const icSigScreeningDescTag As Integer = &H73637264 ' 'scrd'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'scrn'.
        ''' </summary>
        Public Const icSigScreeningTag As Integer = &H7363726E ' 'scrn'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'tech'.
        ''' </summary>
        Public Const icSigTechnologyTag As Integer = &H74656368 ' 'tech'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'bfd '.
        ''' </summary>
        Public Const icSigUcrBgTag As Integer = &H62666420 ' 'bfd '

        ''' <summary>
        ''' ICC Profile Tag Signature: 'vued'.
        ''' </summary>
        Public Const icSigViewingCondDescTag As Integer = &H76756564 ' 'vued'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'view'.
        ''' </summary>
        Public Const icSigViewingConditionsTag As Integer = &H76696577 ' 'view'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'chrm'.
        ''' </summary>
        Public Const icSigChromaticityTag As Integer = &H6368726D ' 'chrm'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'chad'.
        ''' @since 1.5
        ''' </summary>
        Public Const icSigChromaticAdaptationTag As Integer = &H63686164 ' 'chad'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'clro'.
        ''' @since 1.5
        ''' </summary>
        Public Const icSigColorantOrderTag As Integer = &H636C726F ' 'clro'

        ''' <summary>
        ''' ICC Profile Tag Signature: 'clrt'.
        ''' @since 1.5
        ''' </summary>
        Public Const icSigColorantTableTag As Integer = &H636C7274 ' 'clrt'


        ''' <summary>
        ''' ICC Profile Header Location: profile size in bytes.
        ''' </summary>
        Public Const icHdrSize As Integer = 0 ' Profile size in bytes

        ''' <summary>
        ''' ICC Profile Header Location: CMM for this profile.
        ''' </summary>
        Public Const icHdrCmmId As Integer = 4 ' CMM for this profile

        ''' <summary>
        ''' ICC Profile Header Location: format version number.
        ''' </summary>
        Public Const icHdrVersion As Integer = 8 ' Format version number

        ''' <summary>
        ''' ICC Profile Header Location: type of profile.
        ''' </summary>
        Public Const icHdrDeviceClass As Integer = 12 ' Type of profile

        ''' <summary>
        ''' ICC Profile Header Location: color space of data.
        ''' </summary>
        Public Const icHdrColorSpace As Integer = 16 ' Color space of data

        ''' <summary>
        ''' ICC Profile Header Location: PCS - XYZ or Lab only.
        ''' </summary>
        Public Const icHdrPcs As Integer = 20 ' PCS - XYZ or Lab only

        ''' <summary>
        ''' ICC Profile Header Location: date profile was created.
        ''' </summary>
        Public Const icHdrDate As Integer = 24 ' Date profile was created

        ''' <summary>
        ''' ICC Profile Header Location: icMagicNumber.
        ''' </summary>
        Public Const icHdrMagic As Integer = 36 ' icMagicNumber

        ''' <summary>
        ''' ICC Profile Header Location: primary platform.
        ''' </summary>
        Public Const icHdrPlatform As Integer = 40 ' Primary Platform

        ''' <summary>
        ''' ICC Profile Header Location: various bit settings.
        ''' </summary>
        Public Const icHdrFlags As Integer = 44 ' Various bit settings

        ''' <summary>
        ''' ICC Profile Header Location: device manufacturer.
        ''' </summary>
        Public Const icHdrManufacturer As Integer = 48 ' Device manufacturer

        ''' <summary>
        ''' ICC Profile Header Location: device model number.
        ''' </summary>
        Public Const icHdrModel As Integer = 52 ' Device model number

        ''' <summary>
        ''' ICC Profile Header Location: device attributes.
        ''' </summary>
        Public Const icHdrAttributes As Integer = 56 ' Device attributes

        ''' <summary>
        ''' ICC Profile Header Location: rendering intent.
        ''' </summary>
        Public Const icHdrRenderingIntent As Integer = 64 ' Rendering intent

        ''' <summary>
        ''' ICC Profile Header Location: profile illuminant.
        ''' </summary>
        Public Const icHdrIlluminant As Integer = 68 ' Profile illuminant

        ''' <summary>
        ''' ICC Profile Header Location: profile creator.
        ''' </summary>
        Public Const icHdrCreator As Integer = 80 ' Profile creator

        ''' <summary>
        ''' ICC Profile Header Location: profile's ID.
        ''' @since 1.5
        ''' </summary>
        Public Const icHdrProfileID As Integer = 84 ' Profile's ID


        ''' <summary>
        ''' ICC Profile Constant: tag type signaturE.
        ''' </summary>
        Public Const icTagType As Integer = 0 ' tag type signature

        ''' <summary>
        ''' ICC Profile Constant: reserved.
        ''' </summary>
        Public Const icTagReserved As Integer = 4 ' reserved

        ''' <summary>
        ''' ICC Profile Constant: curveType count.
        ''' </summary>
        Public Const icCurveCount As Integer = 8 ' curveType count

        ''' <summary>
        ''' ICC Profile Constant: curveType data.
        ''' </summary>
        Public Const icCurveData As Integer = 12 ' curveType data

        ''' <summary>
        ''' ICC Profile Constant: XYZNumber X.
        ''' </summary>
        Public Const icXYZNumberX As Integer = 8 ' XYZNumber X


        ''' <summary>
        ''' Constructs an ICC_Profile object with a given ID.
        ''' </summary>
        Friend Sub New(ByVal p As sun.java2d.cmm.Profile)
            Me.cmmProfile = p
        End Sub


        ''' <summary>
        ''' Constructs an ICC_Profile object whose loading will be deferred.
        ''' The ID will be 0 until the profile is loaded.
        ''' </summary>
        Friend Sub New(ByVal pdi As sun.java2d.cmm.ProfileDeferralInfo)
            Me.deferralInfo = pdi
            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			Me.profileActivator = New sun.java2d.cmm.ProfileActivator()
            '		{
            '			public  Sub  activate() throws ProfileDataException
            '			{
            '				activateDeferredProfile();
            '			}
            '		};
            sun.java2d.cmm.ProfileDeferralMgr.registerDeferral(Me.profileActivator)
        End Sub


        ''' <summary>
        ''' Frees the resources associated with an ICC_Profile object.
        ''' </summary>
        Protected Overrides Sub Finalize()
            If cmmProfile IsNot Nothing Then
                sun.java2d.cmm.CMSManager.module.freeProfile(cmmProfile)
            ElseIf profileActivator IsNot Nothing Then
                sun.java2d.cmm.ProfileDeferralMgr.unregisterDeferral(profileActivator)
            End If
        End Sub


        ''' <summary>
        ''' Constructs an ICC_Profile object corresponding to the data in
        ''' a byte array.  Throws an IllegalArgumentException if the data
        ''' does not correspond to a valid ICC Profile. </summary>
        ''' <param name="data"> the specified ICC Profile data </param>
        ''' <returns> an <code>ICC_Profile</code> object corresponding to
        '''          the data in the specified <code>data</code> array. </returns>
        Public Shared Function getInstance(ByVal data As SByte()) As ICC_Profile
            Dim thisProfile As ICC_Profile

            Dim p As sun.java2d.cmm.Profile = Nothing

            If sun.java2d.cmm.ProfileDeferralMgr.deferring Then sun.java2d.cmm.ProfileDeferralMgr.activateProfiles()

            sun.java2d.cmm.ProfileDataVerifier.verify(data)

            Try
                p = sun.java2d.cmm.CMSManager.module.loadProfile(data)
            Catch c As CMMException
                Throw New IllegalArgumentException("Invalid ICC Profile Data")
            End Try

            Try
                If (getColorSpaceType(p) = ColorSpace.TYPE_GRAY) AndAlso (getData(p, icSigMediaWhitePointTag) IsNot Nothing) AndAlso (getData(p, icSigGrayTRCTag) IsNot Nothing) Then
                    thisProfile = New ICC_ProfileGray(p)
                ElseIf (getColorSpaceType(p) = ColorSpace.TYPE_RGB) AndAlso (getData(p, icSigMediaWhitePointTag) IsNot Nothing) AndAlso (getData(p, icSigRedColorantTag) IsNot Nothing) AndAlso (getData(p, icSigGreenColorantTag) IsNot Nothing) AndAlso (getData(p, icSigBlueColorantTag) IsNot Nothing) AndAlso (getData(p, icSigRedTRCTag) IsNot Nothing) AndAlso (getData(p, icSigGreenTRCTag) IsNot Nothing) AndAlso (getData(p, icSigBlueTRCTag) IsNot Nothing) Then
                    thisProfile = New ICC_ProfileRGB(p)
                Else
                    thisProfile = New ICC_Profile(p)
                End If
            Catch c As CMMException
                thisProfile = New ICC_Profile(p)
            End Try
            Return thisProfile
        End Function



        ''' <summary>
        ''' Constructs an ICC_Profile corresponding to one of the specific color
        ''' spaces defined by the ColorSpace class (for example CS_sRGB).
        ''' Throws an IllegalArgumentException if cspace is not one of the
        ''' defined color spaces.
        ''' </summary>
        ''' <param name="cspace"> the type of color space to create a profile for.
        ''' The specified type is one of the color
        ''' space constants defined in the  <CODE>ColorSpace</CODE> class.
        ''' </param>
        ''' <returns> an <code>ICC_Profile</code> object corresponding to
        '''          the specified <code>ColorSpace</code> type. </returns>
        ''' <exception cref="IllegalArgumentException"> If <CODE>cspace</CODE> is not
        ''' one of the predefined color space types. </exception>
        Public Shared Function getInstance(ByVal cspace As Integer) As ICC_Profile
            Dim thisProfile As ICC_Profile = Nothing
            Dim fileName As String

            Select Case cspace
                Case ColorSpace.CS_sRGB
                    SyncLock GetType(ICC_Profile)
                        If sRGBprofile Is Nothing Then
                            '                    
                            '                     * Deferral is only used for standard profiles.
                            '                     * Enabling the appropriate access privileges is handled
                            '                     * at a lower level.
                            '                     
                            Dim pInfo As New sun.java2d.cmm.ProfileDeferralInfo("sRGB.pf", ColorSpace.TYPE_RGB, 3, CLASS_DISPLAY)
                            sRGBprofile = getDeferredInstance(pInfo)
                        End If
                        thisProfile = sRGBprofile
                    End SyncLock


                Case ColorSpace.CS_CIEXYZ
                    SyncLock GetType(ICC_Profile)
                        If XYZprofile Is Nothing Then
                            Dim pInfo As New sun.java2d.cmm.ProfileDeferralInfo("CIEXYZ.pf", ColorSpace.TYPE_XYZ, 3, CLASS_DISPLAY)
                            XYZprofile = getDeferredInstance(pInfo)
                        End If
                        thisProfile = XYZprofile
                    End SyncLock


                Case ColorSpace.CS_PYCC
                    SyncLock GetType(ICC_Profile)
                        If PYCCprofile Is Nothing Then
                            If standardProfileExists("PYCC.pf") Then
                                Dim pInfo As New sun.java2d.cmm.ProfileDeferralInfo("PYCC.pf", ColorSpace.TYPE_3CLR, 3, CLASS_DISPLAY)
                                PYCCprofile = getDeferredInstance(pInfo)
                            Else
                                Throw New IllegalArgumentException("Can't load standard profile: PYCC.pf")
                            End If
                        End If
                        thisProfile = PYCCprofile
                    End SyncLock


                Case ColorSpace.CS_GRAY
                    SyncLock GetType(ICC_Profile)
                        If GRAYprofile Is Nothing Then
                            Dim pInfo As New sun.java2d.cmm.ProfileDeferralInfo("GRAY.pf", ColorSpace.TYPE_GRAY, 1, CLASS_DISPLAY)
                            GRAYprofile = getDeferredInstance(pInfo)
                        End If
                        thisProfile = GRAYprofile
                    End SyncLock


                Case ColorSpace.CS_LINEAR_RGB
                    SyncLock GetType(ICC_Profile)
                        If LINEAR_RGBprofile Is Nothing Then
                            Dim pInfo As New sun.java2d.cmm.ProfileDeferralInfo("LINEAR_RGB.pf", ColorSpace.TYPE_RGB, 3, CLASS_DISPLAY)
                            LINEAR_RGBprofile = getDeferredInstance(pInfo)
                        End If
                        thisProfile = LINEAR_RGBprofile
                    End SyncLock


                Case Else
                    Throw New IllegalArgumentException("Unknown color space")
            End Select

            Return thisProfile
        End Function

        '     This asserts system privileges, so is used only for the
        '     * standard profiles.
        '     
        Private Shared Function getStandardProfile(ByVal name As String) As ICC_Profile
            Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of ICC_Profile)(name))
        End Function

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T As ICC_Profile)
            Implements java.security.PrivilegedAction(Of T)

            Public ReadOnly Property name As String

            Sub New(name As String)
                Me.name = name
            End Sub

            Public Overridable Function run() As T Implements PrivilegedAction(Of T).run
                Dim p As ICC_Profile = Nothing
                Try
                    p = getInstance(name)
                Catch ex As java.io.IOException
                    Throw New IllegalArgumentException("Can't load standard profile: " & name)
                End Try
                Return p
            End Function
        End Class

        ''' <summary>
        ''' Constructs an ICC_Profile corresponding to the data in a file.
        ''' fileName may be an absolute or a relative file specification.
        ''' Relative file names are looked for in several places: first, relative
        ''' to any directories specified by the java.iccprofile.path property;
        ''' second, relative to any directories specified by the java.class.path
        ''' property; finally, in a directory used to store profiles always
        ''' available, such as the profile for sRGB.  Built-in profiles use .pf as
        ''' the file name extension for profiles, e.g. sRGB.pf.
        ''' This method throws an IOException if the specified file cannot be
        ''' opened or if an I/O error occurs while reading the file.  It throws
        ''' an IllegalArgumentException if the file does not contain valid ICC
        ''' Profile data. </summary>
        ''' <param name="fileName"> The file that contains the data for the profile.
        ''' </param>
        ''' <returns> an <code>ICC_Profile</code> object corresponding to
        '''          the data in the specified file. </returns>
        ''' <exception cref="IOException"> If the specified file cannot be opened or
        ''' an I/O error occurs while reading the file.
        ''' </exception>
        ''' <exception cref="IllegalArgumentException"> If the file does not
        ''' contain valid ICC Profile data.
        ''' </exception>
        ''' <exception cref="SecurityException"> If a security manager is installed
        ''' and it does not permit read access to the given file. </exception>
        Public Shared Function getInstance(ByVal fileName As String) As ICC_Profile
            Dim thisProfile As ICC_Profile
            Dim fis As java.io.FileInputStream = Nothing


            Dim f As File = getProfileFile(fileName)
            If f IsNot Nothing Then fis = New java.io.FileInputStream(f)
            If fis Is Nothing Then Throw New java.io.IOException("Cannot open file " & fileName)

            thisProfile = getInstance(fis)

            fis.close() ' close the file

            Return thisProfile
        End Function


        ''' <summary>
        ''' Constructs an ICC_Profile corresponding to the data in an InputStream.
        ''' This method throws an IllegalArgumentException if the stream does not
        ''' contain valid ICC Profile data.  It throws an IOException if an I/O
        ''' error occurs while reading the stream. </summary>
        ''' <param name="s"> The input stream from which to read the profile data.
        ''' </param>
        ''' <returns> an <CODE>ICC_Profile</CODE> object corresponding to the
        '''     data in the specified <code>InputStream</code>.
        ''' </returns>
        ''' <exception cref="IOException"> If an I/O error occurs while reading the stream.
        ''' </exception>
        ''' <exception cref="IllegalArgumentException"> If the stream does not
        ''' contain valid ICC Profile data. </exception>
        Public Shared Function getInstance(ByVal s As java.io.InputStream) As ICC_Profile
            Dim profileData As SByte()

            If TypeOf s Is sun.java2d.cmm.ProfileDeferralInfo Then Return getDeferredInstance(CType(s, sun.java2d.cmm.ProfileDeferralInfo))

            profileData = getProfileDataFromStream(s)
            If profileData Is Nothing Then Throw New IllegalArgumentException("Invalid ICC Profile Data")

            Return getInstance(profileData)
        End Function


        Friend Shared Function getProfileDataFromStream(ByVal s As java.io.InputStream) As SByte()
            Dim profileData As SByte()
            Dim profileSize As Integer

            Dim header As SByte() = New SByte(127) {}
            Dim bytestoread As Integer = 128
            Dim bytesread As Integer = 0
            Dim n As Integer

            Do While bytestoread <> 0
                n = s.read(header, bytesread, bytestoread)
                If n < 0 Then Return Nothing
                bytesread += n
                bytestoread -= n
            Loop
            If header(36) <> &H61 OrElse header(37) <> &H63 OrElse header(38) <> &H73 OrElse header(39) <> &H70 Then Return Nothing ' not a valid profile
            profileSize = ((header(0) And &HFF) << 24) Or ((header(1) And &HFF) << 16) Or ((header(2) And &HFF) << 8) Or (header(3) And &HFF)
            profileData = New SByte(profileSize - 1) {}
            Array.Copy(header, 0, profileData, 0, 128)
            bytestoread = profileSize - 128
            bytesread = 128
            Do While bytestoread <> 0
                n = s.read(profileData, bytesread, bytestoread)
                If n < 0 Then Return Nothing
                bytesread += n
                bytestoread -= n
            Loop

            Return profileData
        End Function


        ''' <summary>
        ''' Constructs an ICC_Profile for which the actual loading of the
        ''' profile data from a file and the initialization of the CMM should
        ''' be deferred as long as possible.
        ''' Deferral is only used for standard profiles.
        ''' If deferring is disabled, then getStandardProfile() ensures
        ''' that all of the appropriate access privileges are granted
        ''' when loading this profile.
        ''' If deferring is enabled, then the deferred activation
        ''' code will take care of access privileges. </summary>
        ''' <seealso cref= activateDeferredProfile() </seealso>
        Shared Function getDeferredInstance(ByVal pdi As sun.java2d.cmm.ProfileDeferralInfo) As ICC_Profile
            If Not sun.java2d.cmm.ProfileDeferralMgr.deferring Then Return getStandardProfile(pdi.filename)
            If pdi.colorSpaceType = ColorSpace.TYPE_RGB Then
                Return New ICC_ProfileRGB(pdi)
            ElseIf pdi.colorSpaceType = ColorSpace.TYPE_GRAY Then
                Return New ICC_ProfileGray(pdi)
            Else
                Return New ICC_Profile(pdi)
            End If
        End Function


        Friend Overridable Sub activateDeferredProfile()
            Dim profileData As SByte()
            Dim fis As java.io.FileInputStream
            Dim fileName As String = deferralInfo.filename

            profileActivator = Nothing
            deferralInfo = Nothing
            Dim pa As java.security.PrivilegedAction(Of java.io.FileInputStream) = New PrivilegedActionAnonymousInnerClassHelper2(Of T)
            fis = java.security.AccessController.doPrivileged(pa)
            If fis Is Nothing Then Throw New ProfileDataException("Cannot open file " & fileName)
            Try
                profileData = getProfileDataFromStream(fis)
                fis.close() ' close the file
            Catch e As java.io.IOException
                Dim pde As New ProfileDataException("Invalid ICC Profile Data" & fileName)
                pde.initCause(e)
                Throw pde
            End Try
            If profileData Is Nothing Then Throw New ProfileDataException("Invalid ICC Profile Data" & fileName)
            Try
                cmmProfile = sun.java2d.cmm.CMSManager.module.loadProfile(profileData)
            Catch c As CMMException
                Dim pde As New ProfileDataException("Invalid ICC Profile Data" & fileName)
                pde.initCause(c)
                Throw pde
            End Try
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As java.io.FileInputStream
                Dim f As File = getStandardProfileFile(fileName)
                If f IsNot Nothing Then
                    Try
                        Return New java.io.FileInputStream(f)
                    Catch e As java.io.FileNotFoundException
                    End Try
                End If
                Return Nothing
            End Function
        End Class


        ''' <summary>
        ''' Returns profile major version. </summary>
        ''' <returns>  The major version of the profile. </returns>
        Public Overridable Property majorVersion As Integer
            Get
                Dim theHeader As SByte()

                theHeader = getData(icSigHead) ' getData will activate deferred
                '                                           profiles if necessary 

                Return CInt(theHeader(8))
            End Get
        End Property

        ''' <summary>
        ''' Returns profile minor version. </summary>
        ''' <returns> The minor version of the profile. </returns>
        Public Overridable Property minorVersion As Integer
            Get
                Dim theHeader As SByte()

                theHeader = getData(icSigHead) ' getData will activate deferred
                '                                           profiles if necessary 

                Return CInt(theHeader(9))
            End Get
        End Property

        ''' <summary>
        ''' Returns the profile class. </summary>
        ''' <returns> One of the predefined profile class constants. </returns>
        Public Overridable Property profileClass As Integer
            Get
                Dim theHeader As SByte()
                Dim theClassSig, theClass As Integer

                If deferralInfo IsNot Nothing Then Return deferralInfo.profileClass ' Need to have this info for

                theHeader = getData(icSigHead)

                theClassSig = intFromBigEndian(theHeader, icHdrDeviceClass)

                Select Case theClassSig
                    Case icSigInputClass
                        theClass = CLASS_INPUT

                    Case icSigDisplayClass
                        theClass = CLASS_DISPLAY

                    Case icSigOutputClass
                        theClass = CLASS_OUTPUT

                    Case icSigLinkClass
                        theClass = CLASS_DEVICELINK

                    Case icSigColorSpaceClass
                        theClass = CLASS_COLORSPACECONVERSION

                    Case icSigAbstractClass
                        theClass = CLASS_ABSTRACT

                    Case icSigNamedColorClass
                        theClass = CLASS_NAMEDCOLOR

                    Case Else
                        Throw New IllegalArgumentException("Unknown profile class")
                End Select

                Return theClass
            End Get
        End Property

        ''' <summary>
        ''' Returns the color space type.  Returns one of the color space type
        ''' constants defined by the ColorSpace class.  This is the
        ''' "input" color space of the profile.  The type defines the
        ''' number of components of the color space and the interpretation,
        ''' e.g. TYPE_RGB identifies a color space with three components - red,
        ''' green, and blue.  It does not define the particular color
        ''' characteristics of the space, e.g. the chromaticities of the
        ''' primaries. </summary>
        ''' <returns> One of the color space type constants defined in the
        ''' <CODE>ColorSpace</CODE> class. </returns>
        Public Overridable Property colorSpaceType As Integer
            Get
                If deferralInfo IsNot Nothing Then Return deferralInfo.colorSpaceType ' Need to have this info for
                Return getColorSpaceType(cmmProfile)
            End Get
        End Property

        Friend Shared Function getColorSpaceType(ByVal p As sun.java2d.cmm.Profile) As Integer
            Dim theHeader As SByte()
            Dim theColorSpaceSig, theColorSpace As Integer

            theHeader = getData(p, icSigHead)
            theColorSpaceSig = intFromBigEndian(theHeader, icHdrColorSpace)
            theColorSpace = iccCStoJCS(theColorSpaceSig)
            Return theColorSpace
        End Function

        ''' <summary>
        ''' Returns the color space type of the Profile Connection Space (PCS).
        ''' Returns one of the color space type constants defined by the
        ''' ColorSpace class.  This is the "output" color space of the
        ''' profile.  For an input, display, or output profile useful
        ''' for tagging colors or images, this will be either TYPE_XYZ or
        ''' TYPE_Lab and should be interpreted as the corresponding specific
        ''' color space defined in the ICC specification.  For a device
        ''' link profile, this could be any of the color space type constants. </summary>
        ''' <returns> One of the color space type constants defined in the
        ''' <CODE>ColorSpace</CODE> class. </returns>
        Public Overridable Property pCSType As Integer
            Get
                If sun.java2d.cmm.ProfileDeferralMgr.deferring Then sun.java2d.cmm.ProfileDeferralMgr.activateProfiles()
                Return getPCSType(cmmProfile)
            End Get
        End Property


        Friend Shared Function getPCSType(ByVal p As sun.java2d.cmm.Profile) As Integer
            Dim theHeader As SByte()
            Dim thePCSSig, thePCS As Integer

            theHeader = getData(p, icSigHead)
            thePCSSig = intFromBigEndian(theHeader, icHdrPcs)
            thePCS = iccCStoJCS(thePCSSig)
            Return thePCS
        End Function


        ''' <summary>
        ''' Write this ICC_Profile to a file.
        ''' </summary>
        ''' <param name="fileName"> The file to write the profile data to.
        ''' </param>
        ''' <exception cref="IOException"> If the file cannot be opened for writing
        ''' or an I/O error occurs while writing to the file. </exception>
        Public Overridable Sub write(ByVal fileName As String)
            Dim outputFile As java.io.FileOutputStream
            Dim profileData As SByte()

            profileData = data ' this will activate deferred
            '                                    profiles if necessary 
            outputFile = New java.io.FileOutputStream(fileName)
            outputFile.write(profileData)
            outputFile.close()
        End Sub


        ''' <summary>
        ''' Write this ICC_Profile to an OutputStream.
        ''' </summary>
        ''' <param name="s"> The stream to write the profile data to.
        ''' </param>
        ''' <exception cref="IOException"> If an I/O error occurs while writing to the
        ''' stream. </exception>
        Public Overridable Sub write(ByVal s As java.io.OutputStream)
            Dim profileData As SByte()

            profileData = data ' this will activate deferred
            '                                    profiles if necessary 
            s.write(profileData)
        End Sub


        ''' <summary>
        ''' Returns a byte array corresponding to the data of this ICC_Profile. </summary>
        ''' <returns> A byte array that contains the profile data. </returns>
        ''' <seealso cref= #setData(int, byte[]) </seealso>
        Public Overridable ReadOnly Property data As SByte()
            Get
                Dim profileSize As Integer
                Dim profileData As SByte()

                If sun.java2d.cmm.ProfileDeferralMgr.deferring Then sun.java2d.cmm.ProfileDeferralMgr.activateProfiles()

                Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module

                ' get the number of bytes needed for this profile 
                profileSize = mdl.getProfileSize(cmmProfile)

                profileData = New SByte(profileSize - 1) {}

                ' get the data for the profile 
                mdl.getProfileData(cmmProfile, profileData)

                Return profileData
            End Get
        End Property


        ''' <summary>
        ''' Returns a particular tagged data element from the profile as
        ''' a byte array.  Elements are identified by signatures
        ''' as defined in the ICC specification.  The signature
        ''' icSigHead can be used to get the header.  This method is useful
        ''' for advanced applets or applications which need to access
        ''' profile data directly.
        ''' </summary>
        ''' <param name="tagSignature"> The ICC tag signature for the data element you
        ''' want to get.
        ''' </param>
        ''' <returns> A byte array that contains the tagged data element. Returns
        ''' <code>null</code> if the specified tag doesn't exist. </returns>
        ''' <seealso cref= #setData(int, byte[]) </seealso>
        Public Overridable Function getData(ByVal tagSignature As Integer) As SByte()

            If sun.java2d.cmm.ProfileDeferralMgr.deferring Then sun.java2d.cmm.ProfileDeferralMgr.activateProfiles()

            Return getData(cmmProfile, tagSignature)
        End Function


        Friend Shared Function getData(ByVal p As sun.java2d.cmm.Profile, ByVal tagSignature As Integer) As SByte()
            Dim tagSize As Integer
            Dim tagData As SByte()

            Try
                Dim mdl As sun.java2d.cmm.PCMM = sun.java2d.cmm.CMSManager.module

                ' get the number of bytes needed for this tag 
                tagSize = mdl.getTagSize(p, tagSignature)

                tagData = New SByte(tagSize - 1) {} ' get an array for the tag

                ' get the tag's data 
                mdl.getTagData(p, tagSignature, tagData)
            Catch c As CMMException
                tagData = Nothing
            End Try

            Return tagData
        End Function

        ''' <summary>
        ''' Sets a particular tagged data element in the profile from
        ''' a byte array. The array should contain data in a format, corresponded
        ''' to the {@code tagSignature} as defined in the ICC specification, section 10.
        ''' This method is useful for advanced applets or applications which need to
        ''' access profile data directly.
        ''' </summary>
        ''' <param name="tagSignature"> The ICC tag signature for the data element
        ''' you want to set. </param>
        ''' <param name="tagData"> the data to set for the specified tag signature </param>
        ''' <exception cref="IllegalArgumentException"> if {@code tagSignature} is not a signature
        '''         as defined in the ICC specification. </exception>
        ''' <exception cref="IllegalArgumentException"> if a content of the {@code tagData}
        '''         array can not be interpreted as valid tag data, corresponding
        '''         to the {@code tagSignature}. </exception>
        ''' <seealso cref= #getData </seealso>
        Public Overridable Sub setData(ByVal tagSignature As Integer, ByVal tagData As SByte())

            If sun.java2d.cmm.ProfileDeferralMgr.deferring Then sun.java2d.cmm.ProfileDeferralMgr.activateProfiles()

            sun.java2d.cmm.CMSManager.module.tagDataata(cmmProfile, tagSignature, tagData)
        End Sub

        ''' <summary>
        ''' Sets the rendering intent of the profile.
        ''' This is used to select the proper transform from a profile that
        ''' has multiple transforms.
        ''' </summary>
        Friend Overridable Property renderingIntent As Integer
            Set(ByVal renderingIntent As Integer)
                Dim theHeader As SByte() = getData(icSigHead) ' getData will activate deferred
                '                                                 profiles if necessary 
                intToBigEndian(renderingIntent, theHeader, icHdrRenderingIntent)
                ' set the rendering intent 
                dataata(icSigHead, theHeader)
            End Set
            Get
                Dim theHeader As SByte() = getData(icSigHead) ' getData will activate deferred
                '                                                 profiles if necessary 

                Dim renderingIntent_Renamed As Integer = intFromBigEndian(theHeader, icHdrRenderingIntent)
                ' set the rendering intent 

                '         According to ICC spec, only the least-significant 16 bits shall be
                '         * used to encode the rendering intent. The most significant 16 bits
                '         * shall be set to zero. Thus, we are ignoring two most significant
                '         * bytes here.
                '         *
                '         *  See http://www.color.org/ICC1v42_2006-05.pdf, section 7.2.15.
                '         
                Return (&HFFFF And renderingIntent_Renamed)
            End Get
        End Property




        ''' <summary>
        ''' Returns the number of color components in the "input" color
        ''' space of this profile.  For example if the color space type
        ''' of this profile is TYPE_RGB, then this method will return 3.
        ''' </summary>
        ''' <returns> The number of color components in the profile's input
        ''' color space.
        ''' </returns>
        ''' <exception cref="ProfileDataException"> if color space is in the profile
        '''         is invalid </exception>
        Public Overridable ReadOnly Property numComponents As Integer
            Get
                Dim theHeader As SByte()
                Dim theColorSpaceSig, theNumComponents As Integer

                If deferralInfo IsNot Nothing Then Return deferralInfo.numComponents ' Need to have this info for
                theHeader = getData(icSigHead)

                theColorSpaceSig = intFromBigEndian(theHeader, icHdrColorSpace)

                Select Case theColorSpaceSig
                    Case icSigGrayData
                        theNumComponents = 1

                    Case icSigSpace2CLR
                        theNumComponents = 2

                    Case icSigXYZData, icSigLabData, icSigLuvData, icSigYCbCrData, icSigYxyData, icSigRgbData, icSigHsvData, icSigHlsData, icSigCmyData, icSigSpace3CLR
                        theNumComponents = 3

                    Case icSigCmykData, icSigSpace4CLR
                        theNumComponents = 4

                    Case icSigSpace5CLR
                        theNumComponents = 5

                    Case icSigSpace6CLR
                        theNumComponents = 6

                    Case icSigSpace7CLR
                        theNumComponents = 7

                    Case icSigSpace8CLR
                        theNumComponents = 8

                    Case icSigSpace9CLR
                        theNumComponents = 9

                    Case icSigSpaceACLR
                        theNumComponents = 10

                    Case icSigSpaceBCLR
                        theNumComponents = 11

                    Case icSigSpaceCCLR
                        theNumComponents = 12

                    Case icSigSpaceDCLR
                        theNumComponents = 13

                    Case icSigSpaceECLR
                        theNumComponents = 14

                    Case icSigSpaceFCLR
                        theNumComponents = 15

                    Case Else
                        Throw New ProfileDataException("invalid ICC color space")
                End Select

                Return theNumComponents
            End Get
        End Property


        ''' <summary>
        ''' Returns a float array of length 3 containing the X, Y, and Z
        ''' components of the mediaWhitePointTag in the ICC profile.
        ''' </summary>
        Public Overridable ReadOnly Property mediaWhitePoint As Single()
            Get
                Return getXYZTag(icSigMediaWhitePointTag)
                ' get the media white point tag 
            End Get
        End Property


        ''' <summary>
        ''' Returns a float array of length 3 containing the X, Y, and Z
        ''' components encoded in an XYZType tag.
        ''' </summary>
        Friend Overridable Function getXYZTag(ByVal theTagSignature As Integer) As Single()
            Dim theData As SByte()
            Dim theXYZNumber As Single()
            Dim i1, i2, theS15Fixed16 As Integer

            theData = getData(theTagSignature) ' get the tag data
            '                                             getData will activate deferred
            '                                               profiles if necessary 

            theXYZNumber = New Single(2) {} ' array to return

            ' convert s15Fixed16Number to float 
            i1 = 0
            i2 = icXYZNumberX
            Do While i1 < 3
                theS15Fixed16 = intFromBigEndian(theData, i2)
                theXYZNumber(i1) = (CSng(theS15Fixed16)) / 65536.0F
                i1 += 1
                i2 += 4
            Loop
            Return theXYZNumber
        End Function


        ''' <summary>
        ''' Returns a gamma value representing a tone reproduction
        ''' curve (TRC).  If the profile represents the TRC as a table rather
        ''' than a single gamma value, then an exception is thrown.  In this
        ''' case the actual table can be obtained via getTRC().
        ''' theTagSignature should be one of icSigGrayTRCTag, icSigRedTRCTag,
        ''' icSigGreenTRCTag, or icSigBlueTRCTag. </summary>
        ''' <returns> the gamma value as a float. </returns>
        ''' <exception cref="ProfileDataException"> if the profile does not specify
        '''            the TRC as a single gamma value. </exception>
        Public Overridable Function getGamma(ByVal theTagSignature As Integer) As Single
            Dim theTRCData As SByte()
            Dim theGamma As Single
            Dim theU8Fixed8 As Integer

            theTRCData = getData(theTagSignature) ' get the TRC
            '                                                getData will activate deferred
            '                                                  profiles if necessary 

            If intFromBigEndian(theTRCData, icCurveCount) <> 1 Then Throw New ProfileDataException("TRC is not a gamma")

            ' convert u8Fixed8 to float 
            theU8Fixed8 = (shortFromBigEndian(theTRCData, icCurveData)) And &HFFFF

            theGamma = (CSng(theU8Fixed8)) / 256.0F

            Return theGamma
        End Function


        ''' <summary>
        ''' Returns the TRC as an array of shorts.  If the profile has
        ''' specified the TRC as linear (gamma = 1.0) or as a simple gamma
        ''' value, this method throws an exception, and the getGamma() method
        ''' should be used to get the gamma value.  Otherwise the short array
        ''' returned here represents a lookup table where the input Gray value
        ''' is conceptually in the range [0.0, 1.0].  Value 0.0 maps
        ''' to array index 0 and value 1.0 maps to array index length-1.
        ''' Interpolation may be used to generate output values for
        ''' input values which do not map exactly to an index in the
        ''' array.  Output values also map linearly to the range [0.0, 1.0].
        ''' Value 0.0 is represented by an array value of 0x0000 and
        ''' value 1.0 by 0xFFFF, i.e. the values are really unsigned
        ''' short values, although they are returned in a short array.
        ''' theTagSignature should be one of icSigGrayTRCTag, icSigRedTRCTag,
        ''' icSigGreenTRCTag, or icSigBlueTRCTag. </summary>
        ''' <returns> a short array representing the TRC. </returns>
        ''' <exception cref="ProfileDataException"> if the profile does not specify
        '''            the TRC as a table. </exception>
        Public Overridable Function getTRC(ByVal theTagSignature As Integer) As Short()
            Dim theTRCData As SByte()
            Dim theTRC As Short()
            Dim i1, i2, nElements, theU8Fixed8 As Integer

            theTRCData = getData(theTagSignature) ' get the TRC
            '                                                getData will activate deferred
            '                                                  profiles if necessary 

            nElements = intFromBigEndian(theTRCData, icCurveCount)

            If nElements = 1 Then Throw New ProfileDataException("TRC is not a table")

            ' make the short array 
            theTRC = New Short(nElements - 1) {}

            i1 = 0
            i2 = icCurveData
            Do While i1 < nElements
                theTRC(i1) = shortFromBigEndian(theTRCData, i2)
                i1 += 1
                i2 += 2
            Loop

            Return theTRC
        End Function


        ' convert an ICC color space signature into a Java color space type 
        Friend Shared Function iccCStoJCS(ByVal theColorSpaceSig As Integer) As Integer
            Dim theColorSpace As Integer

            Select Case theColorSpaceSig
                Case icSigXYZData
                    theColorSpace = ColorSpace.TYPE_XYZ

                Case icSigLabData
                    theColorSpace = ColorSpace.TYPE_Lab

                Case icSigLuvData
                    theColorSpace = ColorSpace.TYPE_Luv

                Case icSigYCbCrData
                    theColorSpace = ColorSpace.TYPE_YCbCr

                Case icSigYxyData
                    theColorSpace = ColorSpace.TYPE_Yxy

                Case icSigRgbData
                    theColorSpace = ColorSpace.TYPE_RGB

                Case icSigGrayData
                    theColorSpace = ColorSpace.TYPE_GRAY

                Case icSigHsvData
                    theColorSpace = ColorSpace.TYPE_HSV

                Case icSigHlsData
                    theColorSpace = ColorSpace.TYPE_HLS

                Case icSigCmykData
                    theColorSpace = ColorSpace.TYPE_CMYK

                Case icSigCmyData
                    theColorSpace = ColorSpace.TYPE_CMY

                Case icSigSpace2CLR
                    theColorSpace = ColorSpace.TYPE_2CLR

                Case icSigSpace3CLR
                    theColorSpace = ColorSpace.TYPE_3CLR

                Case icSigSpace4CLR
                    theColorSpace = ColorSpace.TYPE_4CLR

                Case icSigSpace5CLR
                    theColorSpace = ColorSpace.TYPE_5CLR

                Case icSigSpace6CLR
                    theColorSpace = ColorSpace.TYPE_6CLR

                Case icSigSpace7CLR
                    theColorSpace = ColorSpace.TYPE_7CLR

                Case icSigSpace8CLR
                    theColorSpace = ColorSpace.TYPE_8CLR

                Case icSigSpace9CLR
                    theColorSpace = ColorSpace.TYPE_9CLR

                Case icSigSpaceACLR
                    theColorSpace = ColorSpace.TYPE_ACLR

                Case icSigSpaceBCLR
                    theColorSpace = ColorSpace.TYPE_BCLR

                Case icSigSpaceCCLR
                    theColorSpace = ColorSpace.TYPE_CCLR

                Case icSigSpaceDCLR
                    theColorSpace = ColorSpace.TYPE_DCLR

                Case icSigSpaceECLR
                    theColorSpace = ColorSpace.TYPE_ECLR

                Case icSigSpaceFCLR
                    theColorSpace = ColorSpace.TYPE_FCLR

                Case Else
                    Throw New IllegalArgumentException("Unknown color space")
            End Select

            Return theColorSpace
        End Function


        Friend Shared Function intFromBigEndian(ByVal array As SByte(), ByVal index As Integer) As Integer
            Return (((array(index) And &HFF) << 24) Or ((array(index + 1) And &HFF) << 16) Or ((array(index + 2) And &HFF) << 8) Or (array(index + 3) And &HFF))
        End Function


        Friend Shared Sub intToBigEndian(ByVal value As Integer, ByVal array As SByte(), ByVal index As Integer)
            array(index) = CByte(value >> 24)
            array(index + 1) = CByte(value >> 16)
            array(index + 2) = CByte(value >> 8)
            array(index + 3) = CByte(value)
        End Sub


        Friend Shared Function shortFromBigEndian(ByVal array As SByte(), ByVal index As Integer) As Short
            Return CShort(Fix(((array(index) And &HFF) << 8) Or (array(index + 1) And &HFF)))
        End Function


        Friend Shared Sub shortToBigEndian(ByVal value As Short, ByVal array As SByte(), ByVal index As Integer)
            array(index) = CByte(value >> 8)
            array(index + 1) = CByte(value)
        End Sub


        '    
        '     * fileName may be an absolute or a relative file specification.
        '     * Relative file names are looked for in several places: first, relative
        '     * to any directories specified by the java.iccprofile.path property;
        '     * second, relative to any directories specified by the java.class.path
        '     * property; finally, in a directory used to store profiles always
        '     * available, such as a profile for sRGB.  Built-in profiles use .pf as
        '     * the file name extension for profiles, e.g. sRGB.pf.
        '     
        Private Shared Function getProfileFile(ByVal fileName As String) As java.io.File
            Dim path, dir, fullPath As String

            Dim f As New File(fileName) ' try absolute file name
            If f.absolute Then Return If(f.file, f, Nothing)
            path = System.getProperty("java.iccprofile.path")
            If ((Not f.file)) AndAlso (path IsNot Nothing) Then
                ' try relative to java.iccprofile.path 
                Dim st As New java.util.StringTokenizer(path, File.pathSeparator)
                Do While st.hasMoreTokens() AndAlso ((f Is Nothing) OrElse ((Not f.file)))
                    dir = st.nextToken()
                    fullPath = dir + System.IO.Path.DirectorySeparatorChar + fileName
                    f = New File(fullPath)
                    If Not isChildOf(f, dir) Then f = Nothing
                Loop
            End If

            path = System.getProperty("java.class.path")
            If ((f Is Nothing) OrElse ((Not f.file))) AndAlso (path IsNot Nothing) Then
                ' try relative to java.class.path 
                Dim st As New java.util.StringTokenizer(path, File.pathSeparator)
                Do While st.hasMoreTokens() AndAlso ((f Is Nothing) OrElse ((Not f.file)))
                    dir = st.nextToken()
                    fullPath = dir + System.IO.Path.DirectorySeparatorChar + fileName
                    f = New File(fullPath)
                Loop
            End If

            If (f Is Nothing) OrElse ((Not f.file)) Then f = getStandardProfileFile(fileName)
            If f IsNot Nothing AndAlso f.file Then Return f
            Return Nothing
        End Function

        ''' <summary>
        ''' Returns a file object corresponding to a built-in profile
        ''' specified by fileName.
        ''' If there is no built-in profile with such name, then the method
        ''' returns null.
        ''' </summary>
        Private Shared Function getStandardProfileFile(ByVal fileName As String) As java.io.File
            Dim dir As String = System.getProperty("java.home") + System.IO.Path.DirectorySeparatorChar & "lib" & System.IO.Path.DirectorySeparatorChar & "cmm"
            Dim fullPath As String = dir + System.IO.Path.DirectorySeparatorChar + fileName
            Dim f As New File(fullPath)
            Return If(f.file AndAlso isChildOf(f, dir), f, Nothing)
        End Function

        ''' <summary>
        ''' Checks whether given file resides inside give directory.
        ''' </summary>
        Private Shared Function isChildOf(ByVal f As java.io.File, ByVal dirName As String) As Boolean
            Try
                Dim dir As New File(dirName)
                Dim canonicalDirName As String = dir.canonicalPath
                If Not canonicalDirName.EndsWith(File.separator) Then canonicalDirName += File.separator
                Dim canonicalFileName As String = f.canonicalPath
                Return canonicalFileName.StartsWith(canonicalDirName)
            Catch e As java.io.IOException
                '             we do not expect the IOException here, because invocation
                '             * of this function is always preceeded by isFile() call.
                '             
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Checks whether built-in profile specified by fileName exists.
        ''' </summary>
        Private Shared Function standardProfileExists(ByVal fileName As String) As Boolean
            Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T)
        End Function

        Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Boolean?
                Return getStandardProfileFile(fileName) IsNot Nothing
            End Function
        End Class


        '    
        '     * Serialization support.
        '     *
        '     * Directly deserialized profiles are useless since they are not
        '     * registered with CMM.  We don't allow constructor to be called
        '     * directly and instead have clients to call one of getInstance
        '     * factory methods that will register the profile with CMM.  For
        '     * deserialization we implement readResolve method that will
        '     * resolve the bogus deserialized profile object with one obtained
        '     * with getInstance as well.
        '     *
        '     * There're two primary factory methods for construction of ICC
        '     * profiles: getInstance(int cspace) and getInstance(byte[] data).
        '     * This implementation of ICC_Profile uses the former to return a
        '     * cached singleton profile object, other implementations will
        '     * likely use this technique too.  To preserve the singleton
        '     * pattern across serialization we serialize cached singleton
        '     * profiles in such a way that deserializing VM could call
        '     * getInstance(int cspace) method that will resolve deserialized
        '     * object into the corresponding singleton as well.
        '     *
        '     * Since the singletons are private to ICC_Profile the readResolve
        '     * method have to be `protected' instead of `private' so that
        '     * singletons that are instances of subclasses of ICC_Profile
        '     * could be correctly deserialized.
        '     


        ''' <summary>
        ''' Version of the format of additional serialized data in the
        ''' stream.  Version&nbsp;<code>1</code> corresponds to Java&nbsp;2
        ''' Platform,&nbsp;v1.3.
        ''' @since 1.3
        ''' @serial
        ''' </summary>
        Private iccProfileSerializedDataVersion As Integer = 1


        ''' <summary>
        ''' Writes default serializable fields to the stream.  Writes a
        ''' string and an array of bytes to the stream as additional data.
        ''' </summary>
        ''' <param name="s"> stream used for serialization. </param>
        ''' <exception cref="IOException">
        '''     thrown by <code>ObjectInputStream</code>.
        ''' @serialData
        '''     The <code>String</code> is the name of one of
        '''     <code>CS_<var>*</var></code> constants defined in the
        '''     <seealso cref="ColorSpace"/> class if the profile object is a profile
        '''     for a predefined color space (for example
        '''     <code>"CS_sRGB"</code>).  The string is <code>null</code>
        '''     otherwise.
        '''     <p>
        '''     The <code>byte[]</code> array is the profile data for the
        '''     profile.  For predefined color spaces <code>null</code> is
        '''     written instead of the profile data.  If in the future
        '''     versions of Java API new predefined color spaces will be
        '''     added, future versions of this class may choose to write
        '''     for new predefined color spaces not only the color space
        '''     name, but the profile data as well so that older versions
        '''     could still deserialize the object. </exception>
        Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
            s.defaultWriteObject()

            Dim csName As String = Nothing
            If Me Is sRGBprofile Then
                csName = "CS_sRGB"
            ElseIf Me Is XYZprofile Then
                csName = "CS_CIEXYZ"
            ElseIf Me Is PYCCprofile Then
                csName = "CS_PYCC"
            ElseIf Me Is GRAYprofile Then
                csName = "CS_GRAY"
            ElseIf Me Is LINEAR_RGBprofile Then
                csName = "CS_LINEAR_RGB"
            End If

            ' Future versions may choose to write profile data for new
            ' predefined color spaces as well, if any will be introduced,
            ' so that old versions that don't recognize the new CS name
            ' may fall back to constructing profile from the data.
            Dim data_Renamed As SByte() = Nothing
            If csName Is Nothing Then data_Renamed = data

            s.writeObject(csName)
            s.writeObject(data_Renamed)
        End Sub

        ' Temporary storage used by readObject to store resolved profile
        ' (obtained with getInstance) for readResolve to return.
        <NonSerialized>
        Private resolvedDeserializedProfile As ICC_Profile

        ''' <summary>
        ''' Reads default serializable fields from the stream.  Reads from
        ''' the stream a string and an array of bytes as additional data.
        ''' </summary>
        ''' <param name="s"> stream used for deserialization. </param>
        ''' <exception cref="IOException">
        '''     thrown by <code>ObjectInputStream</code>. </exception>
        ''' <exception cref="ClassNotFoundException">
        '''     thrown by <code>ObjectInputStream</code>.
        ''' @serialData
        '''     The <code>String</code> is the name of one of
        '''     <code>CS_<var>*</var></code> constants defined in the
        '''     <seealso cref="ColorSpace"/> class if the profile object is a profile
        '''     for a predefined color space (for example
        '''     <code>"CS_sRGB"</code>).  The string is <code>null</code>
        '''     otherwise.
        '''     <p>
        '''     The <code>byte[]</code> array is the profile data for the
        '''     profile.  It will usually be <code>null</code> for the
        '''     predefined profiles.
        '''     <p>
        '''     If the string is recognized as a constant name for
        '''     predefined color space the object will be resolved into
        '''     profile obtained with
        '''     <code>getInstance(int&nbsp;cspace)</code> and the profile
        '''     data are ignored.  Otherwise the object will be resolved
        '''     into profile obtained with
        '''     <code>getInstance(byte[]&nbsp;data)</code>. </exception>
        ''' <seealso cref= #readResolve() </seealso>
        ''' <seealso cref= #getInstance(int) </seealso>
        ''' <seealso cref= #getInstance(byte[]) </seealso>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            s.defaultReadObject()

            Dim csName As String = CStr(s.readObject())
            Dim data_Renamed As SByte() = CType(s.readObject(), SByte())

            Dim cspace As Integer = 0 ' ColorSpace.CS_* constant if known
            Dim isKnownPredefinedCS As Boolean = False
            If csName IsNot Nothing Then
                isKnownPredefinedCS = True
                If csName.Equals("CS_sRGB") Then
                    cspace = ColorSpace.CS_sRGB
                ElseIf csName.Equals("CS_CIEXYZ") Then
                    cspace = ColorSpace.CS_CIEXYZ
                ElseIf csName.Equals("CS_PYCC") Then
                    cspace = ColorSpace.CS_PYCC
                ElseIf csName.Equals("CS_GRAY") Then
                    cspace = ColorSpace.CS_GRAY
                ElseIf csName.Equals("CS_LINEAR_RGB") Then
                    cspace = ColorSpace.CS_LINEAR_RGB
                Else
                    isKnownPredefinedCS = False
                End If
            End If

            If isKnownPredefinedCS Then
                resolvedDeserializedProfile = getInstance(cspace)
            Else
                resolvedDeserializedProfile = getInstance(data_Renamed)
            End If
        End Sub

        ''' <summary>
        ''' Resolves instances being deserialized into instances registered
        ''' with CMM. </summary>
        ''' <returns> ICC_Profile object for profile registered with CMM. </returns>
        ''' <exception cref="ObjectStreamException">
        '''     never thrown, but mandated by the serialization spec.
        ''' @since 1.3 </exception>
        Protected Friend Overridable Function readResolve() As Object
            Return resolvedDeserializedProfile
        End Function
    End Class

End Namespace
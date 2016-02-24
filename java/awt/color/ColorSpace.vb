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

Namespace java.awt.color




	''' <summary>
	''' This abstract class is used to serve as a color space tag to identify the
	''' specific color space of a Color object or, via a ColorModel object,
	''' of an Image, a BufferedImage, or a GraphicsDevice.  It contains
	''' methods that transform colors in a specific color space to/from sRGB
	''' and to/from a well-defined CIEXYZ color space.
	''' <p>
	''' For purposes of the methods in this class, colors are represented as
	''' arrays of color components represented as floats in a normalized range
	''' defined by each ColorSpace.  For many ColorSpaces (e.g. sRGB), this
	''' range is 0.0 to 1.0.  However, some ColorSpaces have components whose
	''' values have a different range.  Methods are provided to inquire per
	''' component minimum and maximum normalized values.
	''' <p>
	''' Several variables are defined for purposes of referring to color
	''' space types (e.g. TYPE_RGB, TYPE_XYZ, etc.) and to refer to specific
	''' color spaces (e.g. CS_sRGB and CS_CIEXYZ).
	''' sRGB is a proposed standard RGB color space.  For more information,
	''' see <A href="http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html">
	''' http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html
	''' </A>.
	''' <p>
	''' The purpose of the methods to transform to/from the well-defined
	''' CIEXYZ color space is to support conversions between any two color
	''' spaces at a reasonably high degree of accuracy.  It is expected that
	''' particular implementations of subclasses of ColorSpace (e.g.
	''' ICC_ColorSpace) will support high performance conversion based on
	''' underlying platform color management systems.
	''' <p>
	''' The CS_CIEXYZ space used by the toCIEXYZ/fromCIEXYZ methods can be
	''' described as follows:
	''' <pre>
	''' 
	''' &nbsp;     CIEXYZ
	''' &nbsp;     viewing illuminance: 200 lux
	''' &nbsp;     viewing white point: CIE D50
	''' &nbsp;     media white point: "that of a perfectly reflecting diffuser" -- D50
	''' &nbsp;     media black point: 0 lux or 0 Reflectance
	''' &nbsp;     flare: 1 percent
	''' &nbsp;     surround: 20percent of the media white point
	''' &nbsp;     media description: reflection print (i.e., RLAB, Hunt viewing media)
	''' &nbsp;     note: For developers creating an ICC profile for this conversion
	''' &nbsp;           space, the following is applicable.  Use a simple Von Kries
	''' &nbsp;           white point adaptation folded into the 3X3 matrix parameters
	''' &nbsp;           and fold the flare and surround effects into the three
	''' &nbsp;           one-dimensional lookup tables (assuming one uses the minimal
	''' &nbsp;           model for monitors).
	''' 
	''' </pre>
	''' </summary>
	''' <seealso cref= ICC_ColorSpace </seealso>

	<Serializable> _
	Public MustInherit Class ColorSpace

		Friend Const serialVersionUID As Long = -409452704308689724L

		Private type As Integer
		Private numComponents As Integer
		<NonSerialized> _
		Private compName As String() = Nothing

		' Cache of singletons for the predefined color spaces.
		Private Shared sRGBspace As ColorSpace
		Private Shared XYZspace As ColorSpace
		Private Shared PYCCspace As ColorSpace
		Private Shared GRAYspace As ColorSpace
		Private Shared LINEAR_RGBspace As ColorSpace

		''' <summary>
		''' Any of the family of XYZ color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_XYZ As Integer = 0

		''' <summary>
		''' Any of the family of Lab color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_Lab As Integer = 1

		''' <summary>
		''' Any of the family of Luv color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_Luv As Integer = 2

		''' <summary>
		''' Any of the family of YCbCr color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_YCbCr As Integer = 3

		''' <summary>
		''' Any of the family of Yxy color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_Yxy As Integer = 4

		''' <summary>
		''' Any of the family of RGB color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_RGB As Integer = 5

		''' <summary>
		''' Any of the family of GRAY color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_GRAY As Integer = 6

		''' <summary>
		''' Any of the family of HSV color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_HSV As Integer = 7

		''' <summary>
		''' Any of the family of HLS color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_HLS As Integer = 8

		''' <summary>
		''' Any of the family of CMYK color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_CMYK As Integer = 9

		''' <summary>
		''' Any of the family of CMY color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_CMY As Integer = 11

		''' <summary>
		''' Generic 2 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_2CLR As Integer = 12

		''' <summary>
		''' Generic 3 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_3CLR As Integer = 13

		''' <summary>
		''' Generic 4 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_4CLR As Integer = 14

		''' <summary>
		''' Generic 5 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_5CLR As Integer = 15

		''' <summary>
		''' Generic 6 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_6CLR As Integer = 16

		''' <summary>
		''' Generic 7 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_7CLR As Integer = 17

		''' <summary>
		''' Generic 8 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_8CLR As Integer = 18

		''' <summary>
		''' Generic 9 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_9CLR As Integer = 19

		''' <summary>
		''' Generic 10 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_ACLR As Integer = 20

		''' <summary>
		''' Generic 11 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_BCLR As Integer = 21

		''' <summary>
		''' Generic 12 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_CCLR As Integer = 22

		''' <summary>
		''' Generic 13 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_DCLR As Integer = 23

		''' <summary>
		''' Generic 14 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_ECLR As Integer = 24

		''' <summary>
		''' Generic 15 component color spaces.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_FCLR As Integer = 25

		''' <summary>
		''' The sRGB color space defined at
		''' <A href="http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html">
		''' http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html
		''' </A>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CS_sRGB As Integer = 1000

		''' <summary>
		''' A built-in linear RGB color space.  This space is based on the
		''' same RGB primaries as CS_sRGB, but has a linear tone reproduction curve.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CS_LINEAR_RGB As Integer = 1004

		''' <summary>
		''' The CIEXYZ conversion color space defined above.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CS_CIEXYZ As Integer = 1001

		''' <summary>
		''' The Photo YCC conversion color space.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CS_PYCC As Integer = 1002

		''' <summary>
		''' The built-in linear gray scale color space.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CS_GRAY As Integer = 1003


		''' <summary>
		''' Constructs a ColorSpace object given a color space type
		''' and the number of components. </summary>
		''' <param name="type"> one of the <CODE>ColorSpace</CODE> type constants </param>
		''' <param name="numcomponents"> the number of components in the color space </param>
		Protected Friend Sub New(ByVal type As Integer, ByVal numcomponents As Integer)
			Me.type = type
			Me.numComponents = numcomponents
		End Sub


		''' <summary>
		''' Returns a ColorSpace representing one of the specific
		''' predefined color spaces. </summary>
		''' <param name="colorspace"> a specific color space identified by one of
		'''        the predefined class constants (e.g. CS_sRGB, CS_LINEAR_RGB,
		'''        CS_CIEXYZ, CS_GRAY, or CS_PYCC) </param>
		''' <returns> the requested <CODE>ColorSpace</CODE> object </returns>
		' NOTE: This method may be called by privileged threads.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Public Shared Function getInstance(ByVal colorspace As Integer) As ColorSpace
		Dim theColorSpace As ColorSpace

			Select Case colorspace
			Case CS_sRGB
				SyncLock GetType(ColorSpace)
					If sRGBspace Is Nothing Then
						Dim theProfile As ICC_Profile = ICC_Profile.getInstance(CS_sRGB)
						sRGBspace = New ICC_ColorSpace(theProfile)
					End If

					theColorSpace = sRGBspace
				End SyncLock

			Case CS_CIEXYZ
				SyncLock GetType(ColorSpace)
					If XYZspace Is Nothing Then
						Dim theProfile As ICC_Profile = ICC_Profile.getInstance(CS_CIEXYZ)
						XYZspace = New ICC_ColorSpace(theProfile)
					End If

					theColorSpace = XYZspace
				End SyncLock

			Case CS_PYCC
				SyncLock GetType(ColorSpace)
					If PYCCspace Is Nothing Then
						Dim theProfile As ICC_Profile = ICC_Profile.getInstance(CS_PYCC)
						PYCCspace = New ICC_ColorSpace(theProfile)
					End If

					theColorSpace = PYCCspace
				End SyncLock


			Case CS_GRAY
				SyncLock GetType(ColorSpace)
					If GRAYspace Is Nothing Then
						Dim theProfile As ICC_Profile = ICC_Profile.getInstance(CS_GRAY)
						GRAYspace = New ICC_ColorSpace(theProfile)
						' to allow access from java.awt.ColorModel 
						sun.java2d.cmm.CMSManager.GRAYspace = GRAYspace
					End If

					theColorSpace = GRAYspace
				End SyncLock


			Case CS_LINEAR_RGB
				SyncLock GetType(ColorSpace)
					If LINEAR_RGBspace Is Nothing Then
						Dim theProfile As ICC_Profile = ICC_Profile.getInstance(CS_LINEAR_RGB)
						LINEAR_RGBspace = New ICC_ColorSpace(theProfile)
						' to allow access from java.awt.ColorModel 
						sun.java2d.cmm.CMSManager.LINEAR_RGBspace = LINEAR_RGBspace
					End If

					theColorSpace = LINEAR_RGBspace
				End SyncLock


			Case Else
				Throw New IllegalArgumentException("Unknown color space")
			End Select

			Return theColorSpace
		End Function


		''' <summary>
		''' Returns true if the ColorSpace is CS_sRGB. </summary>
		''' <returns> <CODE>true</CODE> if this is a <CODE>CS_sRGB</CODE> color
		'''         space, <code>false</code> if it is not </returns>
		Public Overridable Property cS_sRGB As Boolean
			Get
				' REMIND - make sure we know sRGBspace exists already 
				Return (Me Is sRGBspace)
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
		'''        of components in this ColorSpace </param>
		''' <returns> a float array of length 3 </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		'''         at least the number of components in this ColorSpace </exception>
		Public MustOverride Function toRGB(ByVal colorvalue As Single()) As Single()


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
		''' <param name="rgbvalue"> a float array with length of at least 3 </param>
		''' <returns> a float array with length equal to the number of
		'''         components in this ColorSpace </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		'''         at least 3 </exception>
		Public MustOverride Function fromRGB(ByVal rgbvalue As Single()) As Single()


		''' <summary>
		''' Transforms a color value assumed to be in this ColorSpace
		''' into the CS_CIEXYZ conversion color space.
		''' <p>
		''' This method transforms color values using relative colorimetry,
		''' as defined by the International Color Consortium standard.  This
		''' means that the XYZ values returned by this method are represented
		''' relative to the D50 white point of the CS_CIEXYZ color space.
		''' This representation is useful in a two-step color conversion
		''' process in which colors are transformed from an input color
		''' space to CS_CIEXYZ and then to an output color space.  This
		''' representation is not the same as the XYZ values that would
		''' be measured from the given color value by a colorimeter.
		''' A further transformation is necessary to compute the XYZ values
		''' that would be measured using current CIE recommended practices.
		''' See the <seealso cref="ICC_ColorSpace#toCIEXYZ(float[]) toCIEXYZ"/> method of
		''' <code>ICC_ColorSpace</code> for further information.
		''' <p> </summary>
		''' <param name="colorvalue"> a float array with length of at least the number
		'''        of components in this ColorSpace </param>
		''' <returns> a float array of length 3 </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		'''         at least the number of components in this ColorSpace. </exception>
		Public MustOverride Function toCIEXYZ(ByVal colorvalue As Single()) As Single()


		''' <summary>
		''' Transforms a color value assumed to be in the CS_CIEXYZ conversion
		''' color space into this ColorSpace.
		''' <p>
		''' This method transforms color values using relative colorimetry,
		''' as defined by the International Color Consortium standard.  This
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
		''' See the <seealso cref="ICC_ColorSpace#fromCIEXYZ(float[]) fromCIEXYZ"/> method of
		''' <code>ICC_ColorSpace</code> for further information.
		''' <p> </summary>
		''' <param name="colorvalue"> a float array with length of at least 3 </param>
		''' <returns> a float array with length equal to the number of
		'''         components in this ColorSpace </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if array length is not
		'''         at least 3 </exception>
		Public MustOverride Function fromCIEXYZ(ByVal colorvalue As Single()) As Single()

		''' <summary>
		''' Returns the color space type of this ColorSpace (for example
		''' TYPE_RGB, TYPE_XYZ, ...).  The type defines the
		''' number of components of the color space and the interpretation,
		''' e.g. TYPE_RGB identifies a color space with three components - red,
		''' green, and blue.  It does not define the particular color
		''' characteristics of the space, e.g. the chromaticities of the
		''' primaries.
		''' </summary>
		''' <returns> the type constant that represents the type of this
		'''         <CODE>ColorSpace</CODE> </returns>
		Public Overridable Property type As Integer
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Returns the number of components of this ColorSpace. </summary>
		''' <returns> The number of components in this <CODE>ColorSpace</CODE>. </returns>
		Public Overridable Property numComponents As Integer
			Get
				Return numComponents
			End Get
		End Property

		''' <summary>
		''' Returns the name of the component given the component index.
		''' </summary>
		''' <param name="idx"> the component index </param>
		''' <returns> the name of the component at the specified index </returns>
		''' <exception cref="IllegalArgumentException"> if <code>idx</code> is
		'''         less than 0 or greater than numComponents - 1 </exception>
		Public Overridable Function getName(ByVal idx As Integer) As String
			' REMIND - handle common cases here 
			If (idx < 0) OrElse (idx > numComponents - 1) Then Throw New IllegalArgumentException("Component index out of range: " & idx)

			If compName Is Nothing Then
				Select Case type
					Case ColorSpace.TYPE_XYZ
						compName = New String() {"X", "Y", "Z"}
					Case ColorSpace.TYPE_Lab
						compName = New String() {"L", "a", "b"}
					Case ColorSpace.TYPE_Luv
						compName = New String() {"L", "u", "v"}
					Case ColorSpace.TYPE_YCbCr
						compName = New String() {"Y", "Cb", "Cr"}
					Case ColorSpace.TYPE_Yxy
						compName = New String() {"Y", "x", "y"}
					Case ColorSpace.TYPE_RGB
						compName = New String() {"Red", "Green", "Blue"}
					Case ColorSpace.TYPE_GRAY
						compName = New String() {"Gray"}
					Case ColorSpace.TYPE_HSV
						compName = New String() {"Hue", "Saturation", "Value"}
					Case ColorSpace.TYPE_HLS
						compName = New String() {"Hue", "Lightness", "Saturation"}
					Case ColorSpace.TYPE_CMYK
						compName = New String() {"Cyan", "Magenta", "Yellow", "Black"}
					Case ColorSpace.TYPE_CMY
						compName = New String() {"Cyan", "Magenta", "Yellow"}
					Case Else
						Dim tmp As String() = New String(numComponents - 1){}
						For i As Integer = 0 To tmp.Length - 1
							tmp(i) = "Unnamed color component(" & i & ")"
						Next i
						compName = tmp
				End Select
			End If
			Return compName(idx)
		End Function

		''' <summary>
		''' Returns the minimum normalized color component value for the
		''' specified component.  The default implementation in this abstract
		''' class returns 0.0 for all components.  Subclasses should override
		''' this method if necessary.
		''' </summary>
		''' <param name="component"> the component index </param>
		''' <returns> the minimum normalized component value </returns>
		''' <exception cref="IllegalArgumentException"> if component is less than 0 or
		'''         greater than numComponents - 1
		''' @since 1.4 </exception>
		Public Overridable Function getMinValue(ByVal component_Renamed As Integer) As Single
			If (component_Renamed < 0) OrElse (component_Renamed > numComponents - 1) Then Throw New IllegalArgumentException("Component index out of range: " & component_Renamed)
			Return 0.0f
		End Function

		''' <summary>
		''' Returns the maximum normalized color component value for the
		''' specified component.  The default implementation in this abstract
		''' class returns 1.0 for all components.  Subclasses should override
		''' this method if necessary.
		''' </summary>
		''' <param name="component"> the component index </param>
		''' <returns> the maximum normalized component value </returns>
		''' <exception cref="IllegalArgumentException"> if component is less than 0 or
		'''         greater than numComponents - 1
		''' @since 1.4 </exception>
		Public Overridable Function getMaxValue(ByVal component_Renamed As Integer) As Single
			If (component_Renamed < 0) OrElse (component_Renamed > numComponents - 1) Then Throw New IllegalArgumentException("Component index out of range: " & component_Renamed)
			Return 1.0f
		End Function

	'     Returns true if cspace is the XYZspace.
	'     
		Shared Function isCS_CIEXYZ(ByVal cspace As ColorSpace) As Boolean
			Return (cspace Is XYZspace)
		End Function
	End Class

End Namespace
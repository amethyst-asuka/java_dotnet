Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices
Imports color

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
    ''' The <code>Color</code> class is used to encapsulate colors in the default
    ''' sRGB color space or colors in arbitrary color spaces identified by a
    ''' <seealso cref="ColorSpace"/>.  Every color has an implicit alpha value of 1.0 or
    ''' an explicit one provided in the constructor.  The alpha value
    ''' defines the transparency of a color and can be represented by
    ''' a float value in the range 0.0&nbsp;-&nbsp;1.0 or 0&nbsp;-&nbsp;255.
    ''' An alpha value of 1.0 or 255 means that the color is completely
    ''' opaque and an alpha value of 0 or 0.0 means that the color is
    ''' completely transparent.
    ''' When constructing a <code>Color</code> with an explicit alpha or
    ''' getting the color/alpha components of a <code>Color</code>, the color
    ''' components are never premultiplied by the alpha component.
    ''' <p>
    ''' The default color space for the Java 2D(tm) API is sRGB, a proposed
    ''' standard RGB color space.  For further information on sRGB,
    ''' see <A href="http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html">
    ''' http://www.w3.org/pub/WWW/Graphics/Color/sRGB.html
    ''' </A>.
    ''' <p>
    ''' @version     10 Feb 1997
    ''' @author      Sami Shaio
    ''' @author      Arthur van Hoff </summary>
    ''' <seealso cref=         ColorSpace </seealso>
    ''' <seealso cref=         AlphaComposite </seealso>
    <Serializable>
    Public Class Color : Inherits java.lang.Object
        Implements Paint

        ''' <summary>
        ''' The color white.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly white As New Color(255, 255, 255)

        ''' <summary>
        ''' The color white.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly WHITE As Color = white

        ''' <summary>
        ''' The color light gray.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly lightGray As New Color(192, 192, 192)

        ''' <summary>
        ''' The color light gray.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly LIGHT_GRAY As Color = lightGray

        ''' <summary>
        ''' The color gray.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly gray As New Color(128, 128, 128)

        ''' <summary>
        ''' The color gray.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly GRAY As Color = gray

        ''' <summary>
        ''' The color dark gray.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly darkGray As New Color(64, 64, 64)

        ''' <summary>
        ''' The color dark gray.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly DARK_GRAY As Color = darkGray

        ''' <summary>
        ''' The color black.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly black As New Color(0, 0, 0)

        ''' <summary>
        ''' The color black.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly BLACK As Color = black

        ''' <summary>
        ''' The color red.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly red As New Color(255, 0, 0)

        ''' <summary>
        ''' The color red.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly RED As Color = red

        ''' <summary>
        ''' The color pink.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly pink As New Color(255, 175, 175)

        ''' <summary>
        ''' The color pink.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly PINK As Color = pink

        ''' <summary>
        ''' The color orange.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly orange As New Color(255, 200, 0)

        ''' <summary>
        ''' The color orange.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly ORANGE As Color = orange

        ''' <summary>
        ''' The color yellow.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly yellow As New Color(255, 255, 0)

        ''' <summary>
        ''' The color yellow.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly YELLOW As Color = yellow

        ''' <summary>
        ''' The color green.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly green As New Color(0, 255, 0)

        ''' <summary>
        ''' The color green.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly GREEN As Color = green

        ''' <summary>
        ''' The color magenta.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly magenta As New Color(255, 0, 255)

        ''' <summary>
        ''' The color magenta.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly MAGENTA As Color = magenta

        ''' <summary>
        ''' The color cyan.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly cyan As New Color(0, 255, 255)

        ''' <summary>
        ''' The color cyan.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly CYAN As Color = cyan

        ''' <summary>
        ''' The color blue.  In the default sRGB space.
        ''' </summary>
        Public Shared ReadOnly blue As New Color(0, 0, 255)

        ''' <summary>
        ''' The color blue.  In the default sRGB space.
        ''' @since 1.4
        ''' </summary>
        Public Shared ReadOnly BLUE As Color = blue

        ''' <summary>
        ''' The color value.
        ''' @serial </summary>
        ''' <seealso cref= #getRGB </seealso>
        Friend value As Integer

        ''' <summary>
        ''' The color value in the default sRGB <code>ColorSpace</code> as
        ''' <code>float</code> components (no alpha).
        ''' If <code>null</code> after object construction, this must be an
        ''' sRGB color constructed with 8-bit precision, so compute from the
        ''' <code>int</code> color value.
        ''' @serial </summary>
        ''' <seealso cref= #getRGBColorComponents </seealso>
        ''' <seealso cref= #getRGBComponents </seealso>
        Private frgbvalue As Single() = Nothing

        ''' <summary>
        ''' The color value in the native <code>ColorSpace</code> as
        ''' <code>float</code> components (no alpha).
        ''' If <code>null</code> after object construction, this must be an
        ''' sRGB color constructed with 8-bit precision, so compute from the
        ''' <code>int</code> color value.
        ''' @serial </summary>
        ''' <seealso cref= #getRGBColorComponents </seealso>
        ''' <seealso cref= #getRGBComponents </seealso>
        Private fvalue As Single() = Nothing

        ''' <summary>
        ''' The alpha value as a <code>float</code> component.
        ''' If <code>frgbvalue</code> is <code>null</code>, this is not valid
        ''' data, so compute from the <code>int</code> color value.
        ''' @serial </summary>
        ''' <seealso cref= #getRGBComponents </seealso>
        ''' <seealso cref= #getComponents </seealso>
        Private falpha As Single = 0.0F

        ''' <summary>
        ''' The <code>ColorSpace</code>.  If <code>null</code>, then it's
        ''' default is sRGB.
        ''' @serial </summary>
        ''' <seealso cref= #getColor </seealso>
        ''' <seealso cref= #getColorSpace </seealso>
        ''' <seealso cref= #getColorComponents </seealso>
        Private cs As java.awt.Color.colorSpace = Nothing

        '
        '     * JDK 1.1 serialVersionUID
        '
        Private Const serialVersionUID As Long = 118526816881161077L

        ''' <summary>
        ''' Initialize JNI field and method IDs
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub initIDs()
        End Sub

        Shared Sub New()
            ''' <summary>
            ''' 4112352 - Calling getDefaultToolkit()
            ''' ** here can cause this class to be accessed before it is fully
            ''' ** initialized. DON'T DO IT!!!
            ''' **
            ''' ** Toolkit.getDefaultToolkit();
            '''
            ''' </summary>

            ' ensure that the necessary native libraries are loaded
            Toolkit.loadLibraries()
            If Not GraphicsEnvironment.headless Then initIDs()
        End Sub

        ''' <summary>
        ''' Checks the color integer components supplied for validity.
        ''' Throws an <seealso cref="IllegalArgumentException"/> if the value is out of
        ''' range. </summary>
        ''' <param name="r"> the Red component </param>
        ''' <param name="g"> the Green component </param>
        ''' <param name="b"> the Blue component
        '''  </param>
        Private Shared Sub testColorValueRange(r As Integer, g As Integer, b As Integer, a As Integer)
            Dim rangeError As Boolean = False
            Dim badComponentString As String = ""

            If a < 0 OrElse a > 255 Then
                rangeError = True
                badComponentString = badComponentString & " Alpha"
            End If
            If r < 0 OrElse r > 255 Then
                rangeError = True
                badComponentString = badComponentString & " Red"
            End If
            If g < 0 OrElse g > 255 Then
                rangeError = True
                badComponentString = badComponentString & " Green"
            End If
            If b < 0 OrElse b > 255 Then
                rangeError = True
                badComponentString = badComponentString & " Blue"
            End If
            If rangeError = True Then Throw New IllegalArgumentException("Color parameter outside of expected range:" & badComponentString)
        End Sub

        ''' <summary>
        ''' Checks the color <code>float</code> components supplied for
        ''' validity.
        ''' Throws an <code>IllegalArgumentException</code> if the value is out
        ''' of range. </summary>
        ''' <param name="r"> the Red component </param>
        ''' <param name="g"> the Green component </param>
        ''' <param name="b"> the Blue component
        '''  </param>
        Private Shared Sub testColorValueRange(r As Single, g As Single, b As Single, a As Single)
            Dim rangeError As Boolean = False
            Dim badComponentString As String = ""
            If a < 0.0 OrElse a > 1.0 Then
                rangeError = True
                badComponentString = badComponentString & " Alpha"
            End If
            If r < 0.0 OrElse r > 1.0 Then
                rangeError = True
                badComponentString = badComponentString & " Red"
            End If
            If g < 0.0 OrElse g > 1.0 Then
                rangeError = True
                badComponentString = badComponentString & " Green"
            End If
            If b < 0.0 OrElse b > 1.0 Then
                rangeError = True
                badComponentString = badComponentString & " Blue"
            End If
            If rangeError = True Then Throw New IllegalArgumentException("Color parameter outside of expected range:" & badComponentString)
        End Sub

        ''' <summary>
        ''' Creates an opaque sRGB color with the specified red, green,
        ''' and blue values in the range (0 - 255).
        ''' The actual color used in rendering depends
        ''' on finding the best match given the color space
        ''' available for a given output device.
        ''' Alpha is defaulted to 255.
        ''' </summary>
        ''' <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>
        '''        or <code>b</code> are outside of the range
        '''        0 to 255, inclusive </exception>
        ''' <param name="r"> the red component </param>
        ''' <param name="g"> the green component </param>
        ''' <param name="b"> the blue component </param>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue </seealso>
        ''' <seealso cref= #getRGB </seealso>
        Public Sub New(r As Integer, g As Integer, b As Integer)
            Me.New(r, g, b, 255)
        End Sub

        ''' <summary>
        ''' Creates an sRGB color with the specified red, green, blue, and alpha
        ''' values in the range (0 - 255).
        ''' </summary>
        ''' <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>,
        '''        <code>b</code> or <code>a</code> are outside of the range
        '''        0 to 255, inclusive </exception>
        ''' <param name="r"> the red component </param>
        ''' <param name="g"> the green component </param>
        ''' <param name="b"> the blue component </param>
        ''' <param name="a"> the alpha component </param>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue </seealso>
        ''' <seealso cref= #getAlpha </seealso>
        ''' <seealso cref= #getRGB </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Sub New(r As Integer, g As Integer, b As Integer, a As Integer)
            value = ((a And &HFF) << 24) Or ((r And &HFF) << 16) Or ((g And &HFF) << 8) Or ((b And &HFF) << 0)
            testColorValueRange(r, g, b, a)
        End Sub

        ''' <summary>
        ''' Creates an opaque sRGB color with the specified combined RGB value
        ''' consisting of the red component in bits 16-23, the green component
        ''' in bits 8-15, and the blue component in bits 0-7.  The actual color
        ''' used in rendering depends on finding the best match given the
        ''' color space available for a particular output device.  Alpha is
        ''' defaulted to 255.
        ''' </summary>
        ''' <param name="rgb"> the combined RGB components </param>
        ''' <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue </seealso>
        ''' <seealso cref= #getRGB </seealso>
        Public Sub New(rgb As Integer)
            value = &HFF000000L Or rgb
        End Sub

        ''' <summary>
        ''' Creates an sRGB color with the specified combined RGBA value consisting
        ''' of the alpha component in bits 24-31, the red component in bits 16-23,
        ''' the green component in bits 8-15, and the blue component in bits 0-7.
        ''' If the <code>hasalpha</code> argument is <code>false</code>, alpha
        ''' is defaulted to 255.
        ''' </summary>
        ''' <param name="rgba"> the combined RGBA components </param>
        ''' <param name="hasalpha"> <code>true</code> if the alpha bits are valid;
        '''        <code>false</code> otherwise </param>
        ''' <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue </seealso>
        ''' <seealso cref= #getAlpha </seealso>
        ''' <seealso cref= #getRGB </seealso>
        Public Sub New(rgba As Integer, hasalpha As Boolean)
            If hasalpha Then
                value = rgba
            Else
                value = &HFF000000L Or rgba
            End If
        End Sub

        ''' <summary>
        ''' Creates an opaque sRGB color with the specified red, green, and blue
        ''' values in the range (0.0 - 1.0).  Alpha is defaulted to 1.0.  The
        ''' actual color used in rendering depends on finding the best
        ''' match given the color space available for a particular output
        ''' device.
        ''' </summary>
        ''' <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>
        '''        or <code>b</code> are outside of the range
        '''        0.0 to 1.0, inclusive </exception>
        ''' <param name="r"> the red component </param>
        ''' <param name="g"> the green component </param>
        ''' <param name="b"> the blue component </param>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue </seealso>
        ''' <seealso cref= #getRGB </seealso>
        Public Sub New(r As Single, g As Single, b As Single)
            Me.New(CInt(Fix(r * 255 + 0.5)), CInt(Fix(g * 255 + 0.5)), CInt(Fix(b * 255 + 0.5)))
            testColorValueRange(r, g, b, 1.0F)
            frgbvalue = New Single(2) {}
            frgbvalue(0) = r
            frgbvalue(1) = g
            frgbvalue(2) = b
            falpha = 1.0F
            fvalue = frgbvalue
        End Sub

        ''' <summary>
        ''' Creates an sRGB color with the specified red, green, blue, and
        ''' alpha values in the range (0.0 - 1.0).  The actual color
        ''' used in rendering depends on finding the best match given the
        ''' color space available for a particular output device. </summary>
        ''' <exception cref="IllegalArgumentException"> if <code>r</code>, <code>g</code>
        '''        <code>b</code> or <code>a</code> are outside of the range
        '''        0.0 to 1.0, inclusive </exception>
        ''' <param name="r"> the red component </param>
        ''' <param name="g"> the green component </param>
        ''' <param name="b"> the blue component </param>
        ''' <param name="a"> the alpha component </param>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue </seealso>
        ''' <seealso cref= #getAlpha </seealso>
        ''' <seealso cref= #getRGB </seealso>
        Public Sub New(r As Single, g As Single, b As Single, a As Single)
            Me.New(CInt(Fix(r * 255 + 0.5)), CInt(Fix(g * 255 + 0.5)), CInt(Fix(b * 255 + 0.5)), CInt(Fix(a * 255 + 0.5)))
            frgbvalue = New Single(2) {}
            frgbvalue(0) = r
            frgbvalue(1) = g
            frgbvalue(2) = b
            falpha = a
            fvalue = frgbvalue
        End Sub

        ''' <summary>
        ''' Creates a color in the specified <code>ColorSpace</code>
        ''' with the color components specified in the <code>float</code>
        ''' array and the specified alpha.  The number of components is
        ''' determined by the type of the <code>ColorSpace</code>.  For
        ''' example, RGB requires 3 components, but CMYK requires 4
        ''' components. </summary>
        ''' <param name="cspace"> the <code>ColorSpace</code> to be used to
        '''                  interpret the components </param>
        ''' <param name="components"> an arbitrary number of color components
        '''                      that is compatible with the <code>ColorSpace</code> </param>
        ''' <param name="alpha"> alpha value </param>
        ''' <exception cref="IllegalArgumentException"> if any of the values in the
        '''         <code>components</code> array or <code>alpha</code> is
        '''         outside of the range 0.0 to 1.0 </exception>
        ''' <seealso cref= #getComponents </seealso>
        ''' <seealso cref= #getColorComponents </seealso>
        Sub New(cspace As ColorSpace, components() As Float, alpha As Float)
            Dim rangeError As Boolean = False
            Dim badComponentString As String = ""
            Dim n As Integer = cspace.numComponents
            fvalue = New Single(n - 1) {}
            For i As Integer = 0 To n - 1
                If components(i) < 0.0 OrElse components(i) > 1.0 Then
                    rangeError = True
                    badComponentString = badComponentString & "Component " & i & " "
                Else
                    fvalue(i) = components(i)
                End If
            Next i
            If alpha < 0.0 OrElse alpha > 1.0 Then
                rangeError = True
                badComponentString = badComponentString & "Alpha"
            Else
                falpha = alpha
            End If
            If rangeError Then Throw New IllegalArgumentException("Color parameter outside of expected range: " & badComponentString)
            frgbvalue = cspace.toRGB(fvalue)
            cs = cspace
            value = (((CInt(Fix(falpha * 255))) And &HFF) << 24) Or (((CInt(Fix(frgbvalue(0) * 255))) And &HFF) << 16) Or (((CInt(Fix(frgbvalue(1) * 255))) And &HFF) << 8) Or (((CInt(Fix(frgbvalue(2) * 255))) And &HFF) << 0)
        End Sub

        ''' <summary>
        ''' Returns the red component in the range 0-255 in the default sRGB
        ''' space. </summary>
        ''' <returns> the red component. </returns>
        ''' <seealso cref= #getRGB </seealso>
        Public Function red() As Integer
            Return (rGB() >> 16) And &HFF
        End Function

        ''' <summary>
        ''' Returns the green component in the range 0-255 in the default sRGB
        ''' space. </summary>
        ''' <returns> the green component. </returns>
        ''' <seealso cref= #getRGB </seealso>
        Public Function green() As Integer
            Return (rGB() >> 8) And &HFF
        End Function

        ''' <summary>
        ''' Returns the blue component in the range 0-255 in the default sRGB
        ''' space. </summary>
        ''' <returns> the blue component. </returns>
        ''' <seealso cref= #getRGB </seealso>
        Public Function blue() As Integer
            Return (rGB() >> 0) And &HFF
        End Function

        ''' <summary>
        ''' Returns the alpha component in the range 0-255. </summary>
        ''' <returns> the alpha component. </returns>
        ''' <seealso cref= #getRGB </seealso>
        Public Function alpha() As Integer
            Return (rGB() >> 24) And &HFF
        End Function

        ''' <summary>
        ''' Returns the RGB value representing the color in the default sRGB
        ''' <seealso cref="ColorModel"/>.
        ''' (Bits 24-31 are alpha, 16-23 are red, 8-15 are green, 0-7 are
        ''' blue). </summary>
        ''' <returns> the RGB value of the color in the default sRGB
        '''         <code>ColorModel</code>. </returns>
        ''' <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
        ''' <seealso cref= #getRed </seealso>
        ''' <seealso cref= #getGreen </seealso>
        ''' <seealso cref= #getBlue
        ''' @since JDK1.0 </seealso>
        Public Function rGB() As Integer
            Return value
        End Function

        Private Const FACTOR As Double = 0.7

        ''' <summary>
        ''' Creates a new <code>Color</code> that is a brighter version of this
        ''' <code>Color</code>.
        ''' <p>
        ''' This method applies an arbitrary scale factor to each of the three RGB
        ''' components of this <code>Color</code> to create a brighter version
        ''' of this <code>Color</code>.
        ''' The {@code alpha} value is preserved.
        ''' Although <code>brighter</code> and
        ''' <code>darker</code> are inverse operations, the results of a
        ''' series of invocations of these two methods might be inconsistent
        ''' because of rounding errors. </summary>
        ''' <returns>     a new <code>Color</code> object that is
        '''                 a brighter version of this <code>Color</code>
        '''                 with the same {@code alpha} value. </returns>
        ''' <seealso cref=        java.awt.Color#darker
        ''' @since      JDK1.0 </seealso>
        Public Function brighter() As Color
            Dim r As Integer = red
            Dim g As Integer = green
            Dim b As Integer = blue
            Dim alpha_Renamed As Integer = alpha()

            '         From 2D group:
            '         * 1. black.brighter() should return grey
            '         * 2. applying brighter to blue will always return blue, brighter
            '         * 3. non pure color (non zero rgb) will eventually return white
            '
            Dim i As Integer = CInt(Fix(1.0 / (1.0 - FACTOR)))
            If r = 0 AndAlso g = 0 AndAlso b = 0 Then Return New Color(i, i, i, alpha_Renamed)
            If r > 0 AndAlso r < i Then r = i
            If g > 0 AndAlso g < i Then g = i
            If b > 0 AndAlso b < i Then b = i

            Return New Color(System.Math.Min(CInt(Fix(r / FACTOR)), 255), System.Math.Min(CInt(Fix(g / FACTOR)), 255), System.Math.Min(CInt(Fix(b / FACTOR)), 255), alpha_Renamed)
        End Function

        ''' <summary>
        ''' Creates a new <code>Color</code> that is a darker version of this
        ''' <code>Color</code>.
        ''' <p>
        ''' This method applies an arbitrary scale factor to each of the three RGB
        ''' components of this <code>Color</code> to create a darker version of
        ''' this <code>Color</code>.
        ''' The {@code alpha} value is preserved.
        ''' Although <code>brighter</code> and
        ''' <code>darker</code> are inverse operations, the results of a series
        ''' of invocations of these two methods might be inconsistent because
        ''' of rounding errors. </summary>
        ''' <returns>  a new <code>Color</code> object that is
        '''                    a darker version of this <code>Color</code>
        '''                    with the same {@code alpha} value. </returns>
        ''' <seealso cref=        java.awt.Color#brighter
        ''' @since      JDK1.0 </seealso>
        Public Function darker() As Color
            Return New Color(System.Math.Max(CInt(Fix(red * FACTOR)), 0), System.Math.Max(CInt(Fix(green * FACTOR)), 0), System.Math.Max(CInt(Fix(blue * FACTOR)), 0), alpha)
        End Function

        ''' <summary>
        ''' Computes the hash code for this <code>Color</code>. </summary>
        ''' <returns>     a hash code value for this object.
        ''' @since      JDK1.0 </returns>
        Public Overrides Function GetHashCode() As Integer
            Return value
        End Function

        ''' <summary>
        ''' Determines whether another object is equal to this
        ''' <code>Color</code>.
        ''' <p>
        ''' The result is <code>true</code> if and only if the argument is not
        ''' <code>null</code> and is a <code>Color</code> object that has the same
        ''' red, green, blue, and alpha values as this object. </summary>
        ''' <param name="obj">   the object to test for equality with this
        '''                          <code>Color</code> </param>
        ''' <returns>      <code>true</code> if the objects are the same;
        '''                             <code>false</code> otherwise.
        ''' @since   JDK1.0 </returns>
        Public Overrides Function Equals(obj As Object) As Boolean
            Return TypeOf obj Is Color AndAlso CType(obj, Color).rGB = Me.rGB
        End Function

        ''' <summary>
        ''' Returns a string representation of this <code>Color</code>. This
        ''' method is intended to be used only for debugging purposes.  The
        ''' content and format of the returned string might vary between
        ''' implementations. The returned string might be empty but cannot
        ''' be <code>null</code>.
        ''' </summary>
        ''' <returns>  a string representation of this <code>Color</code>. </returns>
        Public Overrides Function ToString() As String
            Return Me.GetType().Name & "[r=" & red & ",g=" & green & ",b=" & blue & "]"
        End Function

        ''' <summary>
        ''' Converts a <code>String</code> to an integer and returns the
        ''' specified opaque <code>Color</code>. This method handles string
        ''' formats that are used to represent octal and hexadecimal numbers. </summary>
        ''' <param name="nm"> a <code>String</code> that represents
        '''                            an opaque color as a 24-bit integer </param>
        ''' <returns>     the new <code>Color</code> object. </returns>
        ''' <seealso cref=        java.lang.Integer#decode </seealso>
        ''' <exception cref="NumberFormatException">  if the specified string cannot
        '''                      be interpreted as a decimal,
        '''                      octal, or hexadecimal  java.lang.[Integer].
        ''' @since      JDK1.1 </exception>
        Public Shared Function decode(nm As String) As Color 'throws NumberFormatException
            Dim intval As Integer? = java.lang.[Integer].decode(nm)
            Dim i As Integer = intval
            Return New Color((i >> 16) And &HFF, (i >> 8) And &HFF, i And &HFF)
        End Function

        ''' <summary>
        ''' Finds a color in the system properties.
        ''' <p>
        ''' The argument is treated as the name of a system property to
        ''' be obtained. The string value of this property is then interpreted
        ''' as an integer which is then converted to a <code>Color</code>
        ''' object.
        ''' <p>
        ''' If the specified property is not found or could not be parsed as
        ''' an integer then <code>null</code> is returned. </summary>
        ''' <param name="nm"> the name of the color property </param>
        ''' <returns>   the <code>Color</code> converted from the system
        '''          property. </returns>
        ''' <seealso cref=      java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=      java.lang.Integer#getInteger(java.lang.String) </seealso>
        ''' <seealso cref=      java.awt.Color#Color(int)
        ''' @since    JDK1.0 </seealso>
        Public Shared Function getColor(nm As String) As Color
            Return getColor(nm, Nothing)
        End Function

        ''' <summary>
        ''' Finds a color in the system properties.
        ''' <p>
        ''' The first argument is treated as the name of a system property to
        ''' be obtained. The string value of this property is then interpreted
        ''' as an integer which is then converted to a <code>Color</code>
        ''' object.
        ''' <p>
        ''' If the specified property is not found or cannot be parsed as
        ''' an integer then the <code>Color</code> specified by the second
        ''' argument is returned instead. </summary>
        ''' <param name="nm"> the name of the color property </param>
        ''' <param name="v">    the default <code>Color</code> </param>
        ''' <returns>   the <code>Color</code> converted from the system
        '''          property, or the specified <code>Color</code>. </returns>
        ''' <seealso cref=      java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=      java.lang.Integer#getInteger(java.lang.String) </seealso>
        ''' <seealso cref=      java.awt.Color#Color(int)
        ''' @since    JDK1.0 </seealso>
        Public Shared Function getColor(nm As String, y As Color) As Color
            Dim intval As Integer? = java.lang.[Integer].getInteger(nm)
            If intval Is Nothing Then Return v
            Dim i As Integer = intval
            Return New Color((i >> 16) And &HFF, (i >> 8) And &HFF, i And &HFF)
        End Function

        ''' <summary>
        ''' Finds a color in the system properties.
        ''' <p>
        ''' The first argument is treated as the name of a system property to
        ''' be obtained. The string value of this property is then interpreted
        ''' as an integer which is then converted to a <code>Color</code>
        ''' object.
        ''' <p>
        ''' If the specified property is not found or could not be parsed as
        ''' an integer then the integer value <code>v</code> is used instead,
        ''' and is converted to a <code>Color</code> object. </summary>
        ''' <param name="nm">  the name of the color property </param>
        ''' <param name="v">   the default color value, as an integer </param>
        ''' <returns>   the <code>Color</code> converted from the system
        '''          property or the <code>Color</code> converted from
        '''          the specified  java.lang.[Integer]. </returns>
        ''' <seealso cref=      java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=      java.lang.Integer#getInteger(java.lang.String) </seealso>
        ''' <seealso cref=      java.awt.Color#Color(int)
        ''' @since    JDK1.0 </seealso>
        Public Shared Function getColor(nm As String, y As Integer) As Color
            Dim intval As Integer? = java.lang.[Integer].getInteger(nm)
            Dim i As Integer = If(intval IsNot Nothing, intval, v)
            Return New Color((i >> 16) And &HFF, (i >> 8) And &HFF, (i >> 0) And &HFF)
        End Function
        ''' <summary>
        ''' Converts the components of a color, as specified by the HSB
        ''' model, to an equivalent set of values for the default RGB model.
        ''' <p>
        ''' The <code>saturation</code> and <code>brightness</code> components
        ''' should be floating-point values between zero and one
        ''' (numbers in the range 0.0-1.0).  The <code>hue</code> component
        ''' can be any floating-point number.  The floor of this number is
        ''' subtracted from it to create a fraction between 0 and 1.  This
        ''' fractional number is then multiplied by 360 to produce the hue
        ''' angle in the HSB color model.
        ''' <p>
        ''' The integer that is returned by <code>HSBtoRGB</code> encodes the
        ''' value of a color in bits 0-23 of an integer value that is the same
        ''' format used by the method <seealso cref="#getRGB() getRGB"/>.
        ''' This integer can be supplied as an argument to the
        ''' <code>Color</code> constructor that takes a single integer argument. </summary>
        ''' <param name="hue">   the hue component of the color </param>
        ''' <param name="saturation">   the saturation of the color </param>
        ''' <param name="brightness">   the brightness of the color </param>
        ''' <returns>    the RGB value of the color with the indicated hue,
        '''                            saturation, and brightness. </returns>
        ''' <seealso cref=       java.awt.Color#getRGB() </seealso>
        ''' <seealso cref=       java.awt.Color#Color(int) </seealso>
        ''' <seealso cref=       java.awt.image.ColorModel#getRGBdefault()
        ''' @since     JDK1.0 </seealso>
        Public Shared Function HSBtoRGB(hue As Single, saturation As Single, brightness As Single) As Integer
            Dim r As Integer = 0, g As Integer = 0, b As Integer = 0
            If saturation = 0 Then
                b = CInt(Fix(brightness * 255.0F + 0.5F))
                g = b
                r = g
            Else
                Dim h As Single = (hue - CSng(System.Math.Floor(hue))) * 6.0F
                Dim f As Single = h - CSng(System.Math.Floor(h))
                Dim p As Single = brightness * (1.0F - saturation)
                Dim q As Single = brightness * (1.0F - saturation * f)
                Dim t As Single = brightness * (1.0F - (saturation * (1.0F - f)))
                Select Case CInt(Fix(h))
                    Case 0
                        r = CInt(Fix(brightness * 255.0F + 0.5F))
                        g = CInt(Fix(t * 255.0F + 0.5F))
                        b = CInt(Fix(p * 255.0F + 0.5F))
                    Case 1
                        r = CInt(Fix(q * 255.0F + 0.5F))
                        g = CInt(Fix(brightness * 255.0F + 0.5F))
                        b = CInt(Fix(p * 255.0F + 0.5F))
                    Case 2
                        r = CInt(Fix(p * 255.0F + 0.5F))
                        g = CInt(Fix(brightness * 255.0F + 0.5F))
                        b = CInt(Fix(t * 255.0F + 0.5F))
                    Case 3
                        r = CInt(Fix(p * 255.0F + 0.5F))
                        g = CInt(Fix(q * 255.0F + 0.5F))
                        b = CInt(Fix(brightness * 255.0F + 0.5F))
                    Case 4
                        r = CInt(Fix(t * 255.0F + 0.5F))
                        g = CInt(Fix(p * 255.0F + 0.5F))
                        b = CInt(Fix(brightness * 255.0F + 0.5F))
                    Case 5
                        r = CInt(Fix(brightness * 255.0F + 0.5F))
                        g = CInt(Fix(p * 255.0F + 0.5F))
                        b = CInt(Fix(q * 255.0F + 0.5F))
                End Select
            End If
            Return &HFF000000L Or (r << 16) Or (g << 8) Or (b << 0)
        End Function
        ''' <summary>
        ''' Converts the components of a color, as specified by the default RGB
        ''' model, to an equivalent set of values for hue, saturation, and
        ''' brightness that are the three components of the HSB model.
        ''' <p>
        ''' If the <code>hsbvals</code> argument is <code>null</code>, then a
        ''' new array is allocated to return the result. Otherwise, the method
        ''' returns the array <code>hsbvals</code>, with the values put into
        ''' that array. </summary>
        ''' <param name="r">   the red component of the color </param>
        ''' <param name="g">   the green component of the color </param>
        ''' <param name="b">   the blue component of the color </param>
        ''' <param name="hsbvals">  the array used to return the
        '''                     three HSB values, or <code>null</code> </param>
        ''' <returns>    an array of three elements containing the hue, saturation,
        '''                     and brightness (in that order), of the color with
        '''                     the indicated red, green, and blue components. </returns>
        ''' <seealso cref=       java.awt.Color#getRGB() </seealso>
        ''' <seealso cref=       java.awt.Color#Color(int) </seealso>
        ''' <seealso cref=       java.awt.image.ColorModel#getRGBdefault()
        ''' @since     JDK1.0 </seealso>
        Public Shared Function RGBtoHSB(r As Integer, g As Integer, b As Integer, hsbvals As Single()) As Single()
            Dim hue, saturation, brightness As Single
            If hsbvals Is Nothing Then hsbvals = New Single(2) {}
            Dim cmax As Integer = If(r > g, r, g)
            If b > cmax Then cmax = b
            Dim cmin As Integer = If(r < g, r, g)
            If b < cmin Then cmin = b

            brightness = (CSng(cmax)) / 255.0F
            If cmax <> 0 Then
                saturation = (CSng(cmax - cmin)) / (CSng(cmax))
            Else
                saturation = 0
            End If
            If saturation = 0 Then
                hue = 0
            Else
                Dim redc As Single = (CSng(cmax - r)) / (CSng(cmax - cmin))
                Dim greenc As Single = (CSng(cmax - g)) / (CSng(cmax - cmin))
                Dim bluec As Single = (CSng(cmax - b)) / (CSng(cmax - cmin))
                If r = cmax Then
                    hue = bluec - greenc
                ElseIf g = cmax Then
                    hue = 2.0F + redc - bluec
                Else
                    hue = 4.0F + greenc - redc
                End If
                hue = hue / 6.0F
                If hue < 0 Then hue = hue + 1.0F
            End If
            hsbvals(0) = hue
            hsbvals(1) = saturation
            hsbvals(2) = brightness
            Return hsbvals
        End Function
        ''' <summary>
        ''' Creates a <code>Color</code> object based on the specified values
        ''' for the HSB color model.
        ''' <p>
        ''' The <code>s</code> and <code>b</code> components should be
        ''' floating-point values between zero and one
        ''' (numbers in the range 0.0-1.0).  The <code>h</code> component
        ''' can be any floating-point number.  The floor of this number is
        ''' subtracted from it to create a fraction between 0 and 1.  This
        ''' fractional number is then multiplied by 360 to produce the hue
        ''' angle in the HSB color model. </summary>
        ''' <param name="h">   the hue component </param>
        ''' <param name="s">   the saturation of the color </param>
        ''' <param name="b">   the brightness of the color </param>
        ''' <returns>  a <code>Color</code> object with the specified hue,
        '''                                 saturation, and brightness.
        ''' @since   JDK1.0 </returns>
        Public Shared Function getHSBColor(h As Single, s As Single, b As Single) As Color
            Return New Color(HSBtoRGB(h, s, b))
        End Function
        ''' <summary>
        ''' Returns a <code>float</code> array containing the color and alpha
        ''' components of the <code>Color</code>, as represented in the default
        ''' sRGB color space.
        ''' If <code>compArray</code> is <code>null</code>, an array of length
        ''' 4 is created for the return value.  Otherwise,
        ''' <code>compArray</code> must have length 4 or greater,
        ''' and it is filled in with the components and returned. </summary>
        ''' <param name="compArray"> an array that this method fills with
        '''                  color and alpha components and returns </param>
        ''' <returns> the RGBA components in a <code>float</code> array. </returns>
        Public Function getRGBComponents(compArray As Single()) As Single()
            Dim f As Single()
            If compArray Is Nothing Then
                f = New Single(3) {}
            Else
                f = compArray
            End If
            If frgbvalue Is Nothing Then
                f(0) = (CSng(red)) / 255.0F
                f(1) = (CSng(green)) / 255.0F
                f(2) = (CSng(blue)) / 255.0F
                f(3) = (CSng(alpha())) / 255.0F
            Else
                f(0) = frgbvalue(0)
                f(1) = frgbvalue(1)
                f(2) = frgbvalue(2)
                f(3) = falpha
            End If
            Return f
        End Function
        ''' <summary>
        ''' Returns a <code>float</code> array containing only the color
        ''' components of the <code>Color</code>, in the default sRGB color
        ''' space.  If <code>compArray</code> is <code>null</code>, an array of
        ''' length 3 is created for the return value.  Otherwise,
        ''' <code>compArray</code> must have length 3 or greater, and it is
        ''' filled in with the components and returned. </summary>
        ''' <param name="compArray"> an array that this method fills with color
        '''          components and returns </param>
        ''' <returns> the RGB components in a <code>float</code> array. </returns>
        Public Function getRGBColorComponents(compArray As Single()) As Single()
            Dim f As Single()
            If compArray Is Nothing Then
                f = New Single(2) {}
            Else
                f = compArray
            End If
            If frgbvalue Is Nothing Then
                f(0) = (CSng(red)) / 255.0F
                f(1) = (CSng(green)) / 255.0F
                f(2) = (CSng(blue)) / 255.0F
            Else
                f(0) = frgbvalue(0)
                f(1) = frgbvalue(1)
                f(2) = frgbvalue(2)
            End If
            Return f
        End Function
        ''' <summary>
        ''' Returns a <code>float</code> array containing the color and alpha
        ''' components of the <code>Color</code>, in the
        ''' <code>ColorSpace</code> of the <code>Color</code>.
        ''' If <code>compArray</code> is <code>null</code>, an array with
        ''' length equal to the number of components in the associated
        ''' <code>ColorSpace</code> plus one is created for
        ''' the return value.  Otherwise, <code>compArray</code> must have at
        ''' least this length and it is filled in with the components and
        ''' returned. </summary>
        ''' <param name="compArray"> an array that this method fills with the color and
        '''          alpha components of this <code>Color</code> in its
        '''          <code>ColorSpace</code> and returns </param>
        ''' <returns> the color and alpha components in a <code>float</code>
        '''          array. </returns>
        Public Function getComponents(compArray As Single()) As Single()
            If fvalue Is Nothing Then Return getRGBComponents(compArray)
            Dim f As Single()
            Dim n As Integer = fvalue.Length
            If compArray Is Nothing Then
                f = New Single(n) {}
            Else
                f = compArray
            End If
            For i As Integer = 0 To n - 1
                f(i) = fvalue(i)
            Next i
            f(n) = falpha
            Return f
        End Function

        ''' <summary>
        ''' Returns a <code>float</code> array containing only the color
        ''' components of the <code>Color</code>, in the
        ''' <code>ColorSpace</code> of the <code>Color</code>.
        ''' If <code>compArray</code> is <code>null</code>, an array with
        ''' length equal to the number of components in the associated
        ''' <code>ColorSpace</code> is created for
        ''' the return value.  Otherwise, <code>compArray</code> must have at
        ''' least this length and it is filled in with the components and
        ''' returned. </summary>
        ''' <param name="compArray"> an array that this method fills with the color
        '''          components of this <code>Color</code> in its
        '''          <code>ColorSpace</code> and returns </param>
        ''' <returns> the color components in a <code>float</code> array. </returns>
        Public Function getColorComponents(compArray As Single()) As Single()
            If fvalue Is Nothing Then Return getRGBColorComponents(compArray)
            Dim f As Single()
            Dim n As Integer = fvalue.Length
            If compArray Is Nothing Then
                f = New Single(n - 1) {}
            Else
                f = compArray
            End If
            For i As Integer = 0 To n - 1
                f(i) = fvalue(i)
            Next i
            Return f
        End Function

        ''' <summary>
        ''' Returns a <code>float</code> array containing the color and alpha
        ''' components of the <code>Color</code>, in the
        ''' <code>ColorSpace</code> specified by the <code>cspace</code>
        ''' parameter.  If <code>compArray</code> is <code>null</code>, an
        ''' array with length equal to the number of components in
        ''' <code>cspace</code> plus one is created for the return value.
        ''' Otherwise, <code>compArray</code> must have at least this
        ''' length, and it is filled in with the components and returned. </summary>
        ''' <param name="cspace"> a specified <code>ColorSpace</code> </param>
        ''' <param name="compArray"> an array that this method fills with the
        '''          color and alpha components of this <code>Color</code> in
        '''          the specified <code>ColorSpace</code> and returns </param>
        ''' <returns> the color and alpha components in a <code>float</code>
        '''          array. </returns>
        Public Function getComponents(cspace As java.awt.Color.colorSpace, compArray As Single()) As Single()
            If cs Is Nothing Then cs = java.awt.Color.colorSpace.getInstance(java.awt.Color.colorSpace.CS_sRGB)
            Dim f As Single()
            If fvalue Is Nothing Then
                f = New Single(2) {}
                f(0) = (CSng(red)) / 255.0F
                f(1) = (CSng(green)) / 255.0F
                f(2) = (CSng(blue)) / 255.0F
            Else
                f = fvalue
            End If
            Dim tmp As Single() = cs.toCIEXYZ(f)
            Dim tmpout As Single() = cspace.fromCIEXYZ(tmp)
            If compArray Is Nothing Then compArray = New Single(tmpout.Length) {}
            For i As Integer = 0 To tmpout.Length - 1
                compArray(i) = tmpout(i)
            Next i
            If fvalue Is Nothing Then
                compArray(tmpout.Length) = (CSng(alpha())) / 255.0F
            Else
                compArray(tmpout.Length) = falpha
            End If
            Return compArray
        End Function

        ''' <summary>
        ''' Returns a <code>float</code> array containing only the color
        ''' components of the <code>Color</code> in the
        ''' <code>ColorSpace</code> specified by the <code>cspace</code>
        ''' parameter. If <code>compArray</code> is <code>null</code>, an array
        ''' with length equal to the number of components in
        ''' <code>cspace</code> is created for the return value.  Otherwise,
        ''' <code>compArray</code> must have at least this length, and it is
        ''' filled in with the components and returned. </summary>
        ''' <param name="cspace"> a specified <code>ColorSpace</code> </param>
        ''' <param name="compArray"> an array that this method fills with the color
        '''          components of this <code>Color</code> in the specified
        '''          <code>ColorSpace</code> </param>
        ''' <returns> the color components in a <code>float</code> array. </returns>
        Public Function getColorComponents(cspace As java.awt.Color.colorSpace, compArray As Single()) As Single()
            If cs Is Nothing Then cs = java.awt.Color.colorSpace.getInstance(java.awt.Color.colorSpace.CS_sRGB)
            Dim f As Single()
            If fvalue Is Nothing Then
                f = New Single(2) {}
                f(0) = (CSng(red)) / 255.0F
                f(1) = (CSng(green)) / 255.0F
                f(2) = (CSng(blue)) / 255.0F
            Else
                f = fvalue
            End If
            Dim tmp As Single() = cs.toCIEXYZ(f)
            Dim tmpout As Single() = cspace.fromCIEXYZ(tmp)
            If compArray Is Nothing Then Return tmpout
            For i As Integer = 0 To tmpout.Length - 1
                compArray(i) = tmpout(i)
            Next i
            Return compArray
        End Function
        ''' <summary>
        ''' Returns the <code>ColorSpace</code> of this <code>Color</code>. </summary>
        ''' <returns> this <code>Color</code> object's <code>ColorSpace</code>. </returns>
        Public Function colorSpace() As java.awt.Color.colorSpace
            If cs Is Nothing Then cs = java.awt.Color.colorSpace.getInstance(java.awt.Color.colorSpace.CS_sRGB)
            Return cs
        End Function
        ''' <summary>
        ''' Creates and returns a <seealso cref="PaintContext"/> used to
        ''' generate a solid color field pattern.
        ''' See the <seealso cref="Paint#createContext specification"/> of the
        ''' method in the <seealso cref="Paint"/> interface for information
        ''' on null parameter handling.
        ''' </summary>
        ''' <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
        '''           format for the caller to receive the pixel data, or {@code null}
        '''           if there is no preference. </param>
        ''' <param name="r"> the device space bounding box
        '''                     of the graphics primitive being rendered. </param>
        ''' <param name="r2d"> the user space bounding box
        '''                   of the graphics primitive being rendered. </param>
        ''' <param name="xform"> the <seealso cref="AffineTransform"/> from user
        '''              space into device space. </param>
        ''' <param name="hints"> the set of hints that the context object can use to
        '''              choose between rendering alternatives. </param>
        ''' <returns> the {@code PaintContext} for
        '''         generating color patterns. </returns>
        ''' <seealso cref= Paint </seealso>
        ''' <seealso cref= PaintContext </seealso>
        ''' <seealso cref= ColorModel </seealso>
        ''' <seealso cref= Rectangle </seealso>
        ''' <seealso cref= Rectangle2D </seealso>
        ''' <seealso cref= AffineTransform </seealso>
        ''' <seealso cref= RenderingHints </seealso>
        Public Function createContext(cm As java.awt.image.ColorModel, r As Rectangle, r2d As java.awt.geom.Rectangle2D, xform As java.awt.geom.AffineTransform, hints As RenderingHints) As PaintContext
            Return New ColorPaintContext(rGB, cm)
        End Function

        ''' <summary>
        ''' Returns the transparency mode for this <code>Color</code>.  This is
        ''' required to implement the <code>Paint</code> interface. </summary>
        ''' <returns> this <code>Color</code> object's transparency mode. </returns>
        ''' <seealso cref= Paint </seealso>
        ''' <seealso cref= Transparency </seealso>
        ''' <seealso cref= #createContext </seealso>
        Public Function transparency() As Integer
            Dim alpha_Renamed As Integer = alpha()

            If alpha_Renamed = &HFF Then
                Return transparency.OPAQUE
            ElseIf alpha_Renamed = 0 Then
                Return transparency.BITMASK
            Else
                Return transparency.TRANSLUCENT
            End If
        End Function
    End Class

End Namespace
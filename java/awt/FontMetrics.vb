Imports System
Imports int = System.Int32

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
    ''' The <code>FontMetrics</code> class defines a font metrics object, which
    ''' encapsulates information about the rendering of a particular font on a
    ''' particular screen.
    ''' <p>
    ''' <b>Note to subclassers</b>: Since many of these methods form closed,
    ''' mutually recursive loops, you must take care that you implement
    ''' at least one of the methods in each such loop to prevent
    ''' infinite recursion when your subclass is used.
    ''' In particular, the following is the minimal suggested set of methods
    ''' to override in order to ensure correctness and prevent infinite
    ''' recursion (though other subsets are equally feasible):
    ''' <ul>
    ''' <li><seealso cref="#getAscent()"/>
    ''' <li><seealso cref="#getLeading()"/>
    ''' <li><seealso cref="#getMaxAdvance()"/>
    ''' <li><seealso cref="#charWidth(char)"/>
    ''' <li><seealso cref="#charsWidth(char[], int, int)"/>
    ''' </ul>
    ''' <p>
    ''' <img src="doc-files/FontMetrics-1.gif" alt="The letter 'p' showing its 'reference point'"
    ''' style="border:15px; float:right; margin: 7px 10px;">
    ''' Note that the implementations of these methods are
    ''' inefficient, so they are usually overridden with more efficient
    ''' toolkit-specific implementations.
    ''' <p>
    ''' When an application asks to place a character at the position
    ''' (<i>x</i>,&nbsp;<i>y</i>), the character is placed so that its
    ''' reference point (shown as the dot in the accompanying image) is
    ''' put at that position. The reference point specifies a horizontal
    ''' line called the <i>baseline</i> of the character. In normal
    ''' printing, the baselines of characters should align.
    ''' <p>
    ''' In addition, every character in a font has an <i>ascent</i>, a
    ''' <i>descent</i>, and an <i>advance width</i>. The ascent is the
    ''' amount by which the character ascends above the baseline. The
    ''' descent is the amount by which the character descends below the
    ''' baseline. The advance width indicates the position at which AWT
    ''' should place the next character.
    ''' <p>
    ''' An array of characters or a string can also have an ascent, a
    ''' descent, and an advance width. The ascent of the array is the
    ''' maximum ascent of any character in the array. The descent is the
    ''' maximum descent of any character in the array. The advance width
    ''' is the sum of the advance widths of each of the characters in the
    ''' character array.  The advance of a <code>String</code> is the
    ''' distance along the baseline of the <code>String</code>.  This
    ''' distance is the width that should be used for centering or
    ''' right-aligning the <code>String</code>.
    ''' <p>Note that the advance of a <code>String</code> is not necessarily
    ''' the sum of the advances of its characters measured in isolation
    ''' because the width of a character can vary depending on its context.
    ''' For example, in Arabic text, the shape of a character can change
    ''' in order to connect to other characters.  Also, in some scripts,
    ''' certain character sequences can be represented by a single shape,
    ''' called a <em>ligature</em>.  Measuring characters individually does
    ''' not account for these transformations.
    ''' <p>Font metrics are baseline-relative, meaning that they are
    ''' generally independent of the rotation applied to the font (modulo
    ''' possible grid hinting effects).  See <seealso cref="java.awt.Font Font"/>.
    ''' 
    ''' @author      Jim Graham </summary>
    ''' <seealso cref=         java.awt.Font
    ''' @since       JDK1.0 </seealso>
    <Serializable>
    Public MustInherit Class FontMetrics : Inherits java.lang.Object

        Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		Private Shared ReadOnly DEFAULT_FRC As New java.awt.font.FontRenderContext(Nothing, False, False)

        '    
        '     * JDK 1.1 serialVersionUID
        '     
        Private Const serialVersionUID As Long = 1681126225205050147L

        ''' <summary>
        ''' Creates a new <code>FontMetrics</code> object for finding out
        ''' height and width information about the specified <code>Font</code>
        ''' and specific character glyphs in that <code>Font</code>. </summary>
        ''' <param name="font"> the <code>Font</code> </param>
        ''' <seealso cref=       java.awt.Font </seealso>
        Protected Friend Sub New(ByVal font As font)
            Me.font = font
        End Sub

        ''' <summary>
        ''' Gets the <code>Font</code> described by this
        ''' <code>FontMetrics</code> object. </summary>
        ''' <returns>    the <code>Font</code> described by this
        ''' <code>FontMetrics</code> object. </returns>
        Public Overridable ReadOnly Property font As font

        ''' <summary>
        ''' Gets the <code>FontRenderContext</code> used by this
        ''' <code>FontMetrics</code> object to measure text.
        ''' <p>
        ''' Note that methods in this class which take a <code>Graphics</code>
        ''' parameter measure text using the <code>FontRenderContext</code>
        ''' of that <code>Graphics</code> object, and not this
        ''' <code>FontRenderContext</code> </summary>
        ''' <returns>    the <code>FontRenderContext</code> used by this
        ''' <code>FontMetrics</code> object.
        ''' @since 1.6 </returns>
        Public Overridable ReadOnly Property fontRenderContext As java.awt.font.FontRenderContext
            Get
                Return DEFAULT_FRC
            End Get
        End Property

        ''' <summary>
        ''' Determines the <em>standard leading</em> of the
        ''' <code>Font</code> described by this <code>FontMetrics</code>
        ''' object.  The standard leading, or
        ''' interline spacing, is the logical amount of space to be reserved
        ''' between the descent of one line of text and the ascent of the next
        ''' line. The height metric is calculated to include this extra space. </summary>
        ''' <returns>    the standard leading of the <code>Font</code>. </returns>
        ''' <seealso cref=   #getHeight() </seealso>
        ''' <seealso cref=   #getAscent() </seealso>
        ''' <seealso cref=   #getDescent() </seealso>
        Public Overridable ReadOnly Property leading As Integer
            Get
                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Determines the <em>font ascent</em> of the <code>Font</code>
        ''' described by this <code>FontMetrics</code> object. The font ascent
        ''' is the distance from the font's baseline to the top of most
        ''' alphanumeric characters. Some characters in the <code>Font</code>
        ''' might extend above the font ascent line. </summary>
        ''' <returns>     the font ascent of the <code>Font</code>. </returns>
        ''' <seealso cref=        #getMaxAscent() </seealso>
        Public Overridable ReadOnly Property ascent As Integer
            Get
                Return font.size
            End Get
        End Property

        ''' <summary>
        ''' Determines the <em>font descent</em> of the <code>Font</code>
        ''' described by this
        ''' <code>FontMetrics</code> object. The font descent is the distance
        ''' from the font's baseline to the bottom of most alphanumeric
        ''' characters with descenders. Some characters in the
        ''' <code>Font</code> might extend
        ''' below the font descent line. </summary>
        ''' <returns>     the font descent of the <code>Font</code>. </returns>
        ''' <seealso cref=        #getMaxDescent() </seealso>
        Public Overridable ReadOnly Property descent As Integer
            Get
                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard height of a line of text in this font.  This
        ''' is the distance between the baseline of adjacent lines of text.
        ''' It is the sum of the leading + ascent + descent. Due to rounding
        ''' this may not be the same as getAscent() + getDescent() + getLeading().
        ''' There is no guarantee that lines of text spaced at this distance are
        ''' disjoint; such lines may overlap if some characters overshoot
        ''' either the standard ascent or the standard descent metric. </summary>
        ''' <returns>    the standard height of the font. </returns>
        ''' <seealso cref=       #getLeading() </seealso>
        ''' <seealso cref=       #getAscent() </seealso>
        ''' <seealso cref=       #getDescent() </seealso>
        Public Overridable ReadOnly Property height As Integer
            Get
                Return leading + ascent + descent
            End Get
        End Property

        ''' <summary>
        ''' Determines the maximum ascent of the <code>Font</code>
        ''' described by this <code>FontMetrics</code> object.  No character
        ''' extends further above the font's baseline than this height. </summary>
        ''' <returns>    the maximum ascent of any character in the
        ''' <code>Font</code>. </returns>
        ''' <seealso cref=       #getAscent() </seealso>
        Public Overridable ReadOnly Property maxAscent As Integer
            Get
                Return ascent
            End Get
        End Property

        ''' <summary>
        ''' Determines the maximum descent of the <code>Font</code>
        ''' described by this <code>FontMetrics</code> object.  No character
        ''' extends further below the font's baseline than this height. </summary>
        ''' <returns>    the maximum descent of any character in the
        ''' <code>Font</code>. </returns>
        ''' <seealso cref=       #getDescent() </seealso>
        Public Overridable ReadOnly Property maxDescent As Integer
            Get
                Return descent
            End Get
        End Property

        ''' <summary>
        ''' For backward compatibility only. </summary>
        ''' <returns>    the maximum descent of any character in the
        ''' <code>Font</code>. </returns>
        ''' <seealso cref= #getMaxDescent() </seealso>
        ''' @deprecated As of JDK version 1.1.1,
        ''' replaced by <code>getMaxDescent()</code>. 
        <Obsolete("As of JDK version 1.1.1,")>
        Public Overridable ReadOnly Property maxDecent As Integer
            Get
                Return maxDescent
            End Get
        End Property

        ''' <summary>
        ''' Gets the maximum advance width of any character in this
        ''' <code>Font</code>.  The advance is the
        ''' distance from the leftmost point to the rightmost point on the
        ''' string's baseline.  The advance of a <code>String</code> is
        ''' not necessarily the sum of the advances of its characters. </summary>
        ''' <returns>    the maximum advance width of any character
        '''            in the <code>Font</code>, or <code>-1</code> if the
        '''            maximum advance width is not known. </returns>
        Public Overridable ReadOnly Property maxAdvance As Integer
            Get
                Return -1
            End Get
        End Property

        ''' <summary>
        ''' Returns the advance width of the specified character in this
        ''' <code>Font</code>.  The advance is the
        ''' distance from the leftmost point to the rightmost point on the
        ''' character's baseline.  Note that the advance of a
        ''' <code>String</code> is not necessarily the sum of the advances
        ''' of its characters.
        ''' 
        ''' <p>This method doesn't validate the specified character to be a
        ''' valid Unicode code point. The caller must validate the
        ''' character value using {@link
        ''' java.lang.Character#isValidCodePoint(int)
        ''' Character.isValidCodePoint} if necessary.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be measured </param>
        ''' <returns>    the advance width of the specified character
        '''            in the <code>Font</code> described by this
        '''            <code>FontMetrics</code> object. </returns>
        ''' <seealso cref=   #charsWidth(char[], int, int) </seealso>
        ''' <seealso cref=   #stringWidth(String) </seealso>
        Public Overridable Function charWidth(ByVal codePoint As Integer) As Integer
			If Not Character.isValidCodePoint(codePoint) Then codePoint = &Hffff ' substitute missing glyph width

			If codePoint < 256 Then
				Return widths(codePoint)
			Else
				Dim buffer As Char() = New Char(1){}
				Dim len As Integer = Character.toChars(codePoint, buffer, 0)
				Return charsWidth(buffer, 0, len)
			End If
		End Function

		''' <summary>
		''' Returns the advance width of the specified character in this
		''' <code>Font</code>.  The advance is the
		''' distance from the leftmost point to the rightmost point on the
		''' character's baseline.  Note that the advance of a
		''' <code>String</code> is not necessarily the sum of the advances
		''' of its characters.
		''' 
		''' <p><b>Note:</b> This method cannot handle <a
		''' href="../lang/Character.html#supplementary"> supplementary
		''' characters</a>. To support all Unicode characters, including
		''' supplementary characters, use the <seealso cref="#charWidth(int)"/> method.
		''' </summary>
		''' <param name="ch"> the character to be measured </param>
		''' <returns>     the advance width of the specified character
		'''                  in the <code>Font</code> described by this
		'''                  <code>FontMetrics</code> object. </returns>
		''' <seealso cref=        #charsWidth(char[], int, int) </seealso>
		''' <seealso cref=        #stringWidth(String) </seealso>
		Public Overridable Function charWidth(ByVal ch As Char) As Integer
			If AscW(ch) < 256 Then Return widths(AscW(ch))
			Dim data As Char() = {ch}
			Return charsWidth(data, 0, 1)
		End Function

		''' <summary>
		''' Returns the total advance width for showing the specified
		''' <code>String</code> in this <code>Font</code>.  The advance
		''' is the distance from the leftmost point to the rightmost point
		''' on the string's baseline.
		''' <p>
		''' Note that the advance of a <code>String</code> is
		''' not necessarily the sum of the advances of its characters. </summary>
		''' <param name="str"> the <code>String</code> to be measured </param>
		''' <returns>    the advance width of the specified <code>String</code>
		'''                  in the <code>Font</code> described by this
		'''                  <code>FontMetrics</code>. </returns>
		''' <exception cref="NullPointerException"> if str is null. </exception>
		''' <seealso cref=       #bytesWidth(byte[], int, int) </seealso>
		''' <seealso cref=       #charsWidth(char[], int, int) </seealso>
		''' <seealso cref=       #getStringBounds(String, Graphics) </seealso>
		Public Overridable Function stringWidth(ByVal str As String) As Integer
			Dim len As Integer = str.length()
			Dim data As Char() = New Char(len - 1){}
			str.getChars(0, len, data, 0)
			Return charsWidth(data, 0, len)
		End Function

        ''' <summary>
        ''' Returns the total advance width for showing the specified array
        ''' of characters in this <code>Font</code>.  The advance is the
        ''' distance from the leftmost point to the rightmost point on the
        ''' string's baseline.  The advance of a <code>String</code>
        ''' is not necessarily the sum of the advances of its characters.
        ''' This is equivalent to measuring a <code>String</code> of the
        ''' characters in the specified range. </summary>
        ''' <param name="data"> the array of characters to be measured </param>
        ''' <param name="off"> the start offset of the characters in the array </param>
        ''' <param name="len"> the number of characters to be measured from the array </param>
        ''' <returns>    the advance width of the subarray of the specified
        '''               <code>char</code> array in the font described by
        '''               this <code>FontMetrics</code> object. </returns>
        ''' <exception cref="NullPointerException"> if <code>data</code> is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the <code>off</code>
        '''            and <code>len</code> arguments index characters outside
        '''            the bounds of the <code>data</code> array. </exception>
        ''' <seealso cref=       #charWidth(int) </seealso>
        ''' <seealso cref=       #charWidth(char) </seealso>
        ''' <seealso cref=       #bytesWidth(byte[], int, int) </seealso>
        ''' <seealso cref=       #stringWidth(String) </seealso>
        Public Function charsWidth(data() As Char, off As int, len As int) As int
            Return stringWidth(New [String](data, off, len))
        End Function

        ''' <summary>
        ''' Returns the total advance width for showing the specified array
        ''' of bytes in this <code>Font</code>.  The advance is the
        ''' distance from the leftmost point to the rightmost point on the
        ''' string's baseline.  The advance of a <code>String</code>
        ''' is not necessarily the sum of the advances of its characters.
        ''' This is equivalent to measuring a <code>String</code> of the
        ''' characters in the specified range. </summary>
        ''' <param name="data"> the array of bytes to be measured </param>
        ''' <param name="off"> the start offset of the bytes in the array </param>
        ''' <param name="len"> the number of bytes to be measured from the array </param>
        ''' <returns>    the advance width of the subarray of the specified
        '''               <code>byte</code> array in the <code>Font</code>
        '''                  described by
        '''               this <code>FontMetrics</code> object. </returns>
        ''' <exception cref="NullPointerException"> if <code>data</code> is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the <code>off</code>
        '''            and <code>len</code> arguments index bytes outside
        '''            the bounds of the <code>data</code> array. </exception>
        ''' <seealso cref=       #charsWidth(char[], int, int) </seealso>
        ''' <seealso cref=       #stringWidth(String) </seealso>
        Public Function bytesWidth(data() As SByte, off As Int32, len As int) As Integer
            Return stringWidth(New [String](data, 0, off, len))
        End Function

        ''' <summary>
        ''' Gets the advance widths of the first 256 characters in the
        ''' <code>Font</code>.  The advance is the
        ''' distance from the leftmost point to the rightmost point on the
        ''' character's baseline.  Note that the advance of a
        ''' <code>String</code> is not necessarily the sum of the advances
        ''' of its characters. </summary>
        ''' <returns>    an array storing the advance widths of the
        '''                 characters in the <code>Font</code>
        '''                 described by this <code>FontMetrics</code> object. </returns>
        Public Function widths() As Integer()
            Dim widths_Renamed As Integer() = New Integer(255) {}
            For ch As Integer = 0 To 255
                widths_Renamed(ch) = charWidth(ch)
            Next ch
            Return widths_Renamed
        End Function
        ''' <summary>
        ''' Checks to see if the <code>Font</code> has uniform line metrics.  A
        ''' composite font may consist of several different fonts to cover
        ''' various character sets.  In such cases, the
        ''' <code>FontLineMetrics</code> objects are not uniform.
        ''' Different fonts may have a different ascent, descent, metrics and
        ''' so on.  This information is sometimes necessary for line
        ''' measuring and line breaking. </summary>
        ''' <returns> <code>true</code> if the font has uniform line metrics;
        ''' <code>false</code> otherwise. </returns>
        ''' <seealso cref= java.awt.Font#hasUniformLineMetrics() </seealso>
        Public Function hasUniformLineMetrics() As Boolean
            Return font.hasUniformLineMetrics()
        End Function

        ''' <summary>
        ''' Returns the <seealso cref="LineMetrics"/> object for the specified
        ''' <code>String</code> in the specified <seealso cref="Graphics"/> context. </summary>
        ''' <param name="str"> the specified <code>String</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>LineMetrics</code> object created with the
        ''' specified <code>String</code> and <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getLineMetrics(String, FontRenderContext) </seealso>
        Public Function getLineMetrics(str As String, context As Graphics) As java.awt.font.LineMetrics
            Return font.getLineMetrics(str, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the <seealso cref="LineMetrics"/> object for the specified
        ''' <code>String</code> in the specified <seealso cref="Graphics"/> context. </summary>
        ''' <param name="str"> the specified <code>String</code> </param>
        ''' <param name="beginIndex"> the initial offset of <code>str</code> </param>
        ''' <param name="limit"> the end offset of <code>str</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>LineMetrics</code> object created with the
        ''' specified <code>String</code> and <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getLineMetrics(String, int, int, FontRenderContext) </seealso>
        Public Function getLineMetrics(str As String, beginIndex As int, limit As int, context As Graphics) As java.awt.font.LineMetrics
            Return font.getLineMetrics(str, beginIndex, limit, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the <seealso cref="LineMetrics"/> object for the specified
        ''' character array in the specified <seealso cref="Graphics"/> context. </summary>
        ''' <param name="chars"> the specified character array </param>
        ''' <param name="beginIndex"> the initial offset of <code>chars</code> </param>
        ''' <param name="limit"> the end offset of <code>chars</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>LineMetrics</code> object created with the
        ''' specified character array and <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getLineMetrics(char[], int, int, FontRenderContext) </seealso>
        Public Function getLineMetrics(chars As Char(), beginIndex As Integer, limit As Integer, context As Graphics) As java.awt.font.LineMetrics
            Return font.getLineMetrics(chars, beginIndex, limit, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the <seealso cref="LineMetrics"/> object for the specified
        ''' <seealso cref="CharacterIterator"/> in the specified <seealso cref="Graphics"/>
        ''' context. </summary>
        ''' <param name="ci"> the specified <code>CharacterIterator</code> </param>
        ''' <param name="beginIndex"> the initial offset in <code>ci</code> </param>
        ''' <param name="limit"> the end index of <code>ci</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>LineMetrics</code> object created with the
        ''' specified arguments. </returns>
        ''' <seealso cref= java.awt.Font#getLineMetrics(CharacterIterator, int, int, FontRenderContext) </seealso>
        Public Function getLineMetrics(ci As java.text.CharacterIterator, beginIndex As Integer, limit As Integer, context As Graphics) As java.awt.font.LineMetrics
            Return font.getLineMetrics(ci, beginIndex, limit, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the bounds of the specified <code>String</code> in the
        ''' specified <code>Graphics</code> context.  The bounds is used
        ''' to layout the <code>String</code>.
        ''' <p>Note: The returned bounds is in baseline-relative coordinates
        ''' (see <seealso cref="java.awt.FontMetrics class notes"/>). </summary>
        ''' <param name="str"> the specified <code>String</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <seealso cref="Rectangle2D"/> that is the bounding box of the
        ''' specified <code>String</code> in the specified
        ''' <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getStringBounds(String, FontRenderContext) </seealso>
        Public Function getStringBounds(str As String, context As Graphics) As java.awt.geom.Rectangle2D
            Return font.getStringBounds(str, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the bounds of the specified <code>String</code> in the
        ''' specified <code>Graphics</code> context.  The bounds is used
        ''' to layout the <code>String</code>.
        ''' <p>Note: The returned bounds is in baseline-relative coordinates
        ''' (see <seealso cref="java.awt.FontMetrics class notes"/>). </summary>
        ''' <param name="str"> the specified <code>String</code> </param>
        ''' <param name="beginIndex"> the offset of the beginning of <code>str</code> </param>
        ''' <param name="limit"> the end offset of <code>str</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>Rectangle2D</code> that is the bounding box of the
        ''' specified <code>String</code> in the specified
        ''' <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getStringBounds(String, int, int, FontRenderContext) </seealso>
        Public Function getStringBounds(str As String, beginIndex As Integer, limit As Integer, context As Graphics) As java.awt.geom.Rectangle2D
            Return font.getStringBounds(str, beginIndex, limit, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the bounds of the specified array of characters
        ''' in the specified <code>Graphics</code> context.
        ''' The bounds is used to layout the <code>String</code>
        ''' created with the specified array of characters,
        ''' <code>beginIndex</code> and <code>limit</code>.
        ''' <p>Note: The returned bounds is in baseline-relative coordinates
        ''' (see <seealso cref="java.awt.FontMetrics class notes"/>). </summary>
        ''' <param name="chars"> an array of characters </param>
        ''' <param name="beginIndex"> the initial offset of the array of
        ''' characters </param>
        ''' <param name="limit"> the end offset of the array of characters </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>Rectangle2D</code> that is the bounding box of the
        ''' specified character array in the specified
        ''' <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getStringBounds(char[], int, int, FontRenderContext) </seealso>
        Public Function getStringBounds(chars As Char(), beginIndex As Integer, limit As Integer, context As Graphics) As java.awt.geom.Rectangle2D
            Return font.getStringBounds(chars, beginIndex, limit, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the bounds of the characters indexed in the specified
        ''' <code>CharacterIterator</code> in the
        ''' specified <code>Graphics</code> context.
        ''' <p>Note: The returned bounds is in baseline-relative coordinates
        ''' (see <seealso cref="java.awt.FontMetrics class notes"/>). </summary>
        ''' <param name="ci"> the specified <code>CharacterIterator</code> </param>
        ''' <param name="beginIndex"> the initial offset in <code>ci</code> </param>
        ''' <param name="limit"> the end index of <code>ci</code> </param>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>Rectangle2D</code> that is the bounding box of the
        ''' characters indexed in the specified <code>CharacterIterator</code>
        ''' in the specified <code>Graphics</code> context. </returns>
        ''' <seealso cref= java.awt.Font#getStringBounds(CharacterIterator, int, int, FontRenderContext) </seealso>
        Public Function getStringBounds(ci As java.text.CharacterIterator, beginIndex As Integer, limit As Integer, context As Graphics) As java.awt.geom.Rectangle2D
            Return font.getStringBounds(ci, beginIndex, limit, myFRC(context))
        End Function

        ''' <summary>
        ''' Returns the bounds for the character with the maximum bounds
        ''' in the specified <code>Graphics</code> context. </summary>
        ''' <param name="context"> the specified <code>Graphics</code> context </param>
        ''' <returns> a <code>Rectangle2D</code> that is the
        ''' bounding box for the character with the maximum bounds. </returns>
        ''' <seealso cref= java.awt.Font#getMaxCharBounds(FontRenderContext) </seealso>
        Public Function getMaxCharBounds(context As Graphics) As java.awt.geom.Rectangle2D
            Return font.getMaxCharBounds(myFRC(context))
        End Function

        Private Function myFRC(context As Graphics) As java.awt.font.FontRenderContext
            If TypeOf context Is java.awt.Graphics2D Then Return CType(context, java.awt.Graphics2D).fontRenderContext
            Return DEFAULT_FRC
        End Function

        ''' <summary>
        ''' Returns a representation of this <code>FontMetrics</code>
        ''' object's values as a <code>String</code>. </summary>
        ''' <returns>    a <code>String</code> representation of this
        ''' <code>FontMetrics</code> object.
        ''' @since     JDK1.0. </returns>
        Public Overrides Function ToString() As String
            Return Me.GetType().name & "[font=" & font & "ascent=" & ascent & ", descent=" & descent & ", height=" & height & "]"
        End Function

        ''' <summary>
        ''' Initialize JNI field and method IDs
        ''' </summary>
        Private Shared Sub initIDs()

        End Sub
    End Class

End Namespace
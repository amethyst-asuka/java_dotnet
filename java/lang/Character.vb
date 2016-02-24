Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports java.lang.Character

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang


    ''' <summary>
    ''' The {@code Character} class wraps a value of the primitive
    ''' type {@code char} in an object. An object of type
    ''' {@code Character} contains a single field whose type is
    ''' {@code char}.
    ''' <p>
    ''' In addition, this class provides several methods for determining
    ''' a character's category (lowercase letter, digit, etc.) and for converting
    ''' characters from uppercase to lowercase and vice versa.
    ''' <p>
    ''' Character information is based on the Unicode Standard, version 6.2.0.
    ''' <p>
    ''' The methods and data of class {@code Character} are defined by
    ''' the information in the <i>UnicodeData</i> file that is part of the
    ''' Unicode Character Database maintained by the Unicode
    ''' Consortium. This file specifies various properties including name
    ''' and general category for every defined Unicode code point or
    ''' character range.
    ''' <p>
    ''' The file and its description are available from the Unicode Consortium at:
    ''' <ul>
    ''' <li><a href="http://www.unicode.org">http://www.unicode.org</a>
    ''' </ul>
    ''' 
    ''' <h3><a name="unicode">Unicode Character Representations</a></h3>
    ''' 
    ''' <p>The {@code char} data type (and therefore the value that a
    ''' {@code Character} object encapsulates) are based on the
    ''' original Unicode specification, which defined characters as
    ''' fixed-width 16-bit entities. The Unicode Standard has since been
    ''' changed to allow for characters whose representation requires more
    ''' than 16 bits.  The range of legal <em>code point</em>s is now
    ''' U+0000 to U+10FFFF, known as <em>Unicode scalar value</em>.
    ''' (Refer to the <a
    ''' href="http://www.unicode.org/reports/tr27/#notation"><i>
    ''' definition</i></a> of the U+<i>n</i> notation in the Unicode
    ''' Standard.)
    ''' 
    ''' <p><a name="BMP">The set of characters from U+0000 to U+FFFF</a> is
    ''' sometimes referred to as the <em>Basic Multilingual Plane (BMP)</em>.
    ''' <a name="supplementary">Characters</a> whose code points are greater
    ''' than U+FFFF are called <em>supplementary character</em>s.  The Java
    ''' platform uses the UTF-16 representation in {@code char} arrays and
    ''' in the {@code String} and {@code StringBuffer} classes. In
    ''' this representation, supplementary characters are represented as a pair
    ''' of {@code char} values, the first from the <em>high-surrogates</em>
    ''' range, (&#92;uD800-&#92;uDBFF), the second from the
    ''' <em>low-surrogates</em> range (&#92;uDC00-&#92;uDFFF).
    ''' 
    ''' <p>A {@code char} value, therefore, represents Basic
    ''' Multilingual Plane (BMP) code points, including the surrogate
    ''' code points, or code units of the UTF-16 encoding. An
    ''' {@code int} value represents all Unicode code points,
    ''' including supplementary code points. The lower (least significant)
    ''' 21 bits of {@code int} are used to represent Unicode code
    ''' points and the upper (most significant) 11 bits must be zero.
    ''' Unless otherwise specified, the behavior with respect to
    ''' supplementary characters and surrogate {@code char} values is
    ''' as follows:
    ''' 
    ''' <ul>
    ''' <li>The methods that only accept a {@code char} value cannot support
    ''' supplementary characters. They treat {@code char} values from the
    ''' surrogate ranges as undefined characters. For example,
    ''' {@code Character.isLetter('\u005CuD840')} returns {@code false}, even though
    ''' this specific value if followed by any low-surrogate value in a string
    ''' would represent a letter.
    ''' 
    ''' <li>The methods that accept an {@code int} value support all
    ''' Unicode characters, including supplementary characters. For
    ''' example, {@code Character.isLetter(0x2F81A)} returns
    ''' {@code true} because the code point value represents a letter
    ''' (a CJK ideograph).
    ''' </ul>
    ''' 
    ''' <p>In the Java SE API documentation, <em>Unicode code point</em> is
    ''' used for character values in the range between U+0000 and U+10FFFF,
    ''' and <em>Unicode code unit</em> is used for 16-bit
    ''' {@code char} values that are code units of the <em>UTF-16</em>
    ''' encoding. For more information on Unicode terminology, refer to the
    ''' <a href="http://www.unicode.org/glossary/">Unicode Glossary</a>.
    ''' 
    ''' @author  Lee Boynton
    ''' @author  Guy Steele
    ''' @author  Akira Tanaka
    ''' @author  Martin Buchholz
    ''' @author  Ulf Zibis
    ''' @since   1.0
    ''' </summary>
    <Serializable>
    Public NotInheritable Class Character
        Implements Comparable(Of Character)

        ''' <summary>
        ''' The minimum radix available for conversion to and from strings.
        ''' The constant value of this field is the smallest value permitted
        ''' for the radix argument in radix-conversion methods such as the
        ''' {@code digit} method, the {@code forDigit} method, and the
        ''' {@code toString} method of class {@code Integer}.
        ''' </summary>
        ''' <seealso cref=     Character#digit(char, int) </seealso>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Integer#toString(int, int) </seealso>
        ''' <seealso cref=     Integer#valueOf(String) </seealso>
        Public Const MIN_RADIX As Integer = 2

        ''' <summary>
        ''' The maximum radix available for conversion to and from strings.
        ''' The constant value of this field is the largest value permitted
        ''' for the radix argument in radix-conversion methods such as the
        ''' {@code digit} method, the {@code forDigit} method, and the
        ''' {@code toString} method of class {@code Integer}.
        ''' </summary>
        ''' <seealso cref=     Character#digit(char, int) </seealso>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Integer#toString(int, int) </seealso>
        ''' <seealso cref=     Integer#valueOf(String) </seealso>
        Public Const MAX_RADIX As Integer = 36

        ''' <summary>
        ''' The constant value of this field is the smallest value of type
        ''' {@code char}, {@code '\u005Cu0000'}.
        ''' 
        ''' @since   1.0.2
        ''' </summary>
        Public Shared ReadOnly MIN_VALUE As Char = ChrW(&H0)

        ''' <summary>
        ''' The constant value of this field is the largest value of type
        ''' {@code char}, {@code '\u005CuFFFF'}.
        ''' 
        ''' @since   1.0.2
        ''' </summary>
        Public Shared ReadOnly MAX_VALUE As Char = ChrW(&HFFFF)

        ''' <summary>
        ''' The {@code Class} instance representing the primitive type
        ''' {@code char}.
        ''' 
        ''' @since   1.1
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared ReadOnly TYPE As [Class] = CType([Class].getPrimitiveClass("char"), [Class])

        '    
        '     * Normative general types
        '     

        '    
        '     * General character types
        '     

        ''' <summary>
        ''' General category "Cn" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const UNASSIGNED As SByte = 0

        ''' <summary>
        ''' General category "Lu" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const UPPERCASE_LETTER As SByte = 1

        ''' <summary>
        ''' General category "Ll" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const LOWERCASE_LETTER As SByte = 2

        ''' <summary>
        ''' General category "Lt" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const TITLECASE_LETTER As SByte = 3

        ''' <summary>
        ''' General category "Lm" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const MODIFIER_LETTER As SByte = 4

        ''' <summary>
        ''' General category "Lo" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const OTHER_LETTER As SByte = 5

        ''' <summary>
        ''' General category "Mn" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const NON_SPACING_MARK As SByte = 6

        ''' <summary>
        ''' General category "Me" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const ENCLOSING_MARK As SByte = 7

        ''' <summary>
        ''' General category "Mc" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const COMBINING_SPACING_MARK As SByte = 8

        ''' <summary>
        ''' General category "Nd" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const DECIMAL_DIGIT_NUMBER As SByte = 9

        ''' <summary>
        ''' General category "Nl" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const LETTER_NUMBER As SByte = 10

        ''' <summary>
        ''' General category "No" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const OTHER_NUMBER As SByte = 11

        ''' <summary>
        ''' General category "Zs" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const SPACE_SEPARATOR As SByte = 12

        ''' <summary>
        ''' General category "Zl" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const LINE_SEPARATOR As SByte = 13

        ''' <summary>
        ''' General category "Zp" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const PARAGRAPH_SEPARATOR As SByte = 14

        ''' <summary>
        ''' General category "Cc" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const CONTROL As SByte = 15

        ''' <summary>
        ''' General category "Cf" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const FORMAT As SByte = 16

        ''' <summary>
        ''' General category "Co" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const PRIVATE_USE As SByte = 18

        ''' <summary>
        ''' General category "Cs" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const SURROGATE As SByte = 19

        ''' <summary>
        ''' General category "Pd" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const DASH_PUNCTUATION As SByte = 20

        ''' <summary>
        ''' General category "Ps" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const START_PUNCTUATION As SByte = 21

        ''' <summary>
        ''' General category "Pe" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const END_PUNCTUATION As SByte = 22

        ''' <summary>
        ''' General category "Pc" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const CONNECTOR_PUNCTUATION As SByte = 23

        ''' <summary>
        ''' General category "Po" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const OTHER_PUNCTUATION As SByte = 24

        ''' <summary>
        ''' General category "Sm" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const MATH_SYMBOL As SByte = 25

        ''' <summary>
        ''' General category "Sc" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const CURRENCY_SYMBOL As SByte = 26

        ''' <summary>
        ''' General category "Sk" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const MODIFIER_SYMBOL As SByte = 27

        ''' <summary>
        ''' General category "So" in the Unicode specification.
        ''' @since   1.1
        ''' </summary>
        Public Const OTHER_SYMBOL As SByte = 28

        ''' <summary>
        ''' General category "Pi" in the Unicode specification.
        ''' @since   1.4
        ''' </summary>
        Public Const INITIAL_QUOTE_PUNCTUATION As SByte = 29

        ''' <summary>
        ''' General category "Pf" in the Unicode specification.
        ''' @since   1.4
        ''' </summary>
        Public Const FINAL_QUOTE_PUNCTUATION As SByte = 30

        ''' <summary>
        ''' Error flag. Use int (code point) to avoid confusion with U+FFFF.
        ''' </summary>
        Friend Const [ERROR] As Integer = &HFFFFFFFFL


        ''' <summary>
        ''' Undefined bidirectional character type. Undefined {@code char}
        ''' values have undefined directionality in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_UNDEFINED As SByte = -1

        ''' <summary>
        ''' Strong bidirectional character type "L" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_LEFT_TO_RIGHT As SByte = 0

        ''' <summary>
        ''' Strong bidirectional character type "R" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_RIGHT_TO_LEFT As SByte = 1

        ''' <summary>
        ''' Strong bidirectional character type "AL" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC As SByte = 2

        ''' <summary>
        ''' Weak bidirectional character type "EN" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_EUROPEAN_NUMBER As SByte = 3

        ''' <summary>
        ''' Weak bidirectional character type "ES" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR As SByte = 4

        ''' <summary>
        ''' Weak bidirectional character type "ET" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR As SByte = 5

        ''' <summary>
        ''' Weak bidirectional character type "AN" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_ARABIC_NUMBER As SByte = 6

        ''' <summary>
        ''' Weak bidirectional character type "CS" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_COMMON_NUMBER_SEPARATOR As SByte = 7

        ''' <summary>
        ''' Weak bidirectional character type "NSM" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_NONSPACING_MARK As SByte = 8

        ''' <summary>
        ''' Weak bidirectional character type "BN" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_BOUNDARY_NEUTRAL As SByte = 9

        ''' <summary>
        ''' Neutral bidirectional character type "B" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_PARAGRAPH_SEPARATOR As SByte = 10

        ''' <summary>
        ''' Neutral bidirectional character type "S" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_SEGMENT_SEPARATOR As SByte = 11

        ''' <summary>
        ''' Neutral bidirectional character type "WS" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_WHITESPACE As SByte = 12

        ''' <summary>
        ''' Neutral bidirectional character type "ON" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_OTHER_NEUTRALS As SByte = 13

        ''' <summary>
        ''' Strong bidirectional character type "LRE" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING As SByte = 14

        ''' <summary>
        ''' Strong bidirectional character type "LRO" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE As SByte = 15

        ''' <summary>
        ''' Strong bidirectional character type "RLE" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING As SByte = 16

        ''' <summary>
        ''' Strong bidirectional character type "RLO" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE As SByte = 17

        ''' <summary>
        ''' Weak bidirectional character type "PDF" in the Unicode specification.
        ''' @since 1.4
        ''' </summary>
        Public Const DIRECTIONALITY_POP_DIRECTIONAL_FORMAT As SByte = 18

        ''' <summary>
        ''' The minimum value of a
        ''' <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
        ''' Unicode high-surrogate code unit</a>
        ''' in the UTF-16 encoding, constant {@code '\u005CuD800'}.
        ''' A high-surrogate is also known as a <i>leading-surrogate</i>.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Shared ReadOnly MIN_HIGH_SURROGATE As Char = ChrW(&HD800)

        ''' <summary>
        ''' The maximum value of a
        ''' <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
        ''' Unicode high-surrogate code unit</a>
        ''' in the UTF-16 encoding, constant {@code '\u005CuDBFF'}.
        ''' A high-surrogate is also known as a <i>leading-surrogate</i>.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Shared ReadOnly MAX_HIGH_SURROGATE As Char = ChrW(&HDBFF)

        ''' <summary>
        ''' The minimum value of a
        ''' <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
        ''' Unicode low-surrogate code unit</a>
        ''' in the UTF-16 encoding, constant {@code '\u005CuDC00'}.
        ''' A low-surrogate is also known as a <i>trailing-surrogate</i>.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Shared ReadOnly MIN_LOW_SURROGATE As Char = ChrW(&HDC00)

        ''' <summary>
        ''' The maximum value of a
        ''' <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
        ''' Unicode low-surrogate code unit</a>
        ''' in the UTF-16 encoding, constant {@code '\u005CuDFFF'}.
        ''' A low-surrogate is also known as a <i>trailing-surrogate</i>.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Shared ReadOnly MAX_LOW_SURROGATE As Char = ChrW(&HDFFF)

        ''' <summary>
        ''' The minimum value of a Unicode surrogate code unit in the
        ''' UTF-16 encoding, constant {@code '\u005CuD800'}.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Shared ReadOnly MIN_SURROGATE As Char = MIN_HIGH_SURROGATE

        ''' <summary>
        ''' The maximum value of a Unicode surrogate code unit in the
        ''' UTF-16 encoding, constant {@code '\u005CuDFFF'}.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Shared ReadOnly MAX_SURROGATE As Char = MAX_LOW_SURROGATE

        ''' <summary>
        ''' The minimum value of a
        ''' <a href="http://www.unicode.org/glossary/#supplementary_code_point">
        ''' Unicode supplementary code point</a>, constant {@code U+10000}.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Const MIN_SUPPLEMENTARY_CODE_POINT As Integer = &H10000

        ''' <summary>
        ''' The minimum value of a
        ''' <a href="http://www.unicode.org/glossary/#code_point">
        ''' Unicode code point</a>, constant {@code U+0000}.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Const MIN_CODE_POINT As Integer = &H0

        ''' <summary>
        ''' The maximum value of a
        ''' <a href="http://www.unicode.org/glossary/#code_point">
        ''' Unicode code point</a>, constant {@code U+10FFFF}.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Const MAX_CODE_POINT As Integer = &H10FFFF


        ''' <summary>
        ''' Instances of this class represent particular subsets of the Unicode
        ''' character set.  The only family of subsets defined in the
        ''' {@code Character} class is <seealso cref="Character.UnicodeBlock"/>.
        ''' Other portions of the Java API may define other subsets for their
        ''' own purposes.
        ''' 
        ''' @since 1.2
        ''' </summary>
        Public Class Subset

            Private name As String

            ''' <summary>
            ''' Constructs a new {@code Subset} instance.
            ''' </summary>
            ''' <param name="name">  The name of this subset </param>
            ''' <exception cref="NullPointerException"> if name is {@code null} </exception>
            Protected Friend Sub New(ByVal name As String)
                If name Is Nothing Then Throw New NullPointerException("name")
                Me.name = name
            End Sub

            ''' <summary>
            ''' Compares two {@code Subset} objects for equality.
            ''' This method returns {@code true} if and only if
            ''' {@code this} and the argument refer to the same
            ''' object; since this method is {@code final}, this
            ''' guarantee holds for all subclasses.
            ''' </summary>
            Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
                Return (Me Is obj)
            End Function

            ''' <summary>
            ''' Returns the standard hash code as defined by the
            ''' <seealso cref="Object#hashCode"/> method.  This method
            ''' is {@code final} in order to ensure that the
            ''' {@code equals} and {@code hashCode} methods will
            ''' be consistent in all subclasses.
            ''' </summary>
            Public NotOverridable Overrides Function GetHashCode() As Integer
                Return MyBase.GetHashCode()
            End Function

            ''' <summary>
            ''' Returns the name of this subset.
            ''' </summary>
            Public NotOverridable Overrides Function ToString() As String
                Return name
            End Function
        End Class

        ' See http://www.unicode.org/Public/UNIDATA/Blocks.txt
        ' for the latest specification of Unicode Blocks.

        ''' <summary>
        ''' A family of character subsets representing the character blocks in the
        ''' Unicode specification. Character blocks generally define characters
        ''' used for a specific script or purpose. A character is contained by
        ''' at most one Unicode block.
        ''' 
        ''' @since 1.2
        ''' </summary>
        Public NotInheritable Class UnicodeBlock
            Inherits Subset

            Private Shared map As IDictionary(Of String, UnicodeBlock) = New Dictionary(Of String, UnicodeBlock)(256)

            ''' <summary>
            ''' Creates a UnicodeBlock with the given identifier name.
            ''' This name must be the same as the block identifier.
            ''' </summary>
            Private Sub New(ByVal idName As String)
                MyBase.New(idName)
                map(idName) = Me
            End Sub

            ''' <summary>
            ''' Creates a UnicodeBlock with the given identifier name and
            ''' alias name.
            ''' </summary>
            Private Sub New(ByVal idName As String, ByVal [alias] As String)
                Me.New(idName)
                map([alias]) = Me
            End Sub

            ''' <summary>
            ''' Creates a UnicodeBlock with the given identifier name and
            ''' alias names.
            ''' </summary>
            Private Sub New(ByVal idName As String, ParamArray ByVal aliases As String())
                Me.New(idName)
                For Each [alias] As String In aliases
                    map([alias]) = Me
                Next [alias]
            End Sub

            ''' <summary>
            ''' Constant for the "Basic Latin" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly BASIC_LATIN As New UnicodeBlock("BASIC_LATIN", "BASIC LATIN", "BASICLATIN")

            ''' <summary>
            ''' Constant for the "Latin-1 Supplement" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly LATIN_1_SUPPLEMENT As New UnicodeBlock("LATIN_1_SUPPLEMENT", "LATIN-1 SUPPLEMENT", "LATIN-1SUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Latin Extended-A" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly LATIN_EXTENDED_A As New UnicodeBlock("LATIN_EXTENDED_A", "LATIN EXTENDED-A", "LATINEXTENDED-A")

            ''' <summary>
            ''' Constant for the "Latin Extended-B" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly LATIN_EXTENDED_B As New UnicodeBlock("LATIN_EXTENDED_B", "LATIN EXTENDED-B", "LATINEXTENDED-B")

            ''' <summary>
            ''' Constant for the "IPA Extensions" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly IPA_EXTENSIONS As New UnicodeBlock("IPA_EXTENSIONS", "IPA EXTENSIONS", "IPAEXTENSIONS")

            ''' <summary>
            ''' Constant for the "Spacing Modifier Letters" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly SPACING_MODIFIER_LETTERS As New UnicodeBlock("SPACING_MODIFIER_LETTERS", "SPACING MODIFIER LETTERS", "SPACINGMODIFIERLETTERS")

            ''' <summary>
            ''' Constant for the "Combining Diacritical Marks" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly COMBINING_DIACRITICAL_MARKS As New UnicodeBlock("COMBINING_DIACRITICAL_MARKS", "COMBINING DIACRITICAL MARKS", "COMBININGDIACRITICALMARKS")

            ''' <summary>
            ''' Constant for the "Greek and Coptic" Unicode character block.
            ''' <p>
            ''' This block was previously known as the "Greek" block.
            ''' 
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GREEK As New UnicodeBlock("GREEK", "GREEK AND COPTIC", "GREEKANDCOPTIC")

            ''' <summary>
            ''' Constant for the "Cyrillic" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CYRILLIC As New UnicodeBlock("CYRILLIC")

            ''' <summary>
            ''' Constant for the "Armenian" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ARMENIAN As New UnicodeBlock("ARMENIAN")

            ''' <summary>
            ''' Constant for the "Hebrew" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly HEBREW As New UnicodeBlock("HEBREW")

            ''' <summary>
            ''' Constant for the "Arabic" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ARABIC As New UnicodeBlock("ARABIC")

            ''' <summary>
            ''' Constant for the "Devanagari" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly DEVANAGARI As New UnicodeBlock("DEVANAGARI")

            ''' <summary>
            ''' Constant for the "Bengali" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly BENGALI As New UnicodeBlock("BENGALI")

            ''' <summary>
            ''' Constant for the "Gurmukhi" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GURMUKHI As New UnicodeBlock("GURMUKHI")

            ''' <summary>
            ''' Constant for the "Gujarati" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GUJARATI As New UnicodeBlock("GUJARATI")

            ''' <summary>
            ''' Constant for the "Oriya" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ORIYA As New UnicodeBlock("ORIYA")

            ''' <summary>
            ''' Constant for the "Tamil" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly TAMIL As New UnicodeBlock("TAMIL")

            ''' <summary>
            ''' Constant for the "Telugu" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly TELUGU As New UnicodeBlock("TELUGU")

            ''' <summary>
            ''' Constant for the "Kannada" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly KANNADA As New UnicodeBlock("KANNADA")

            ''' <summary>
            ''' Constant for the "Malayalam" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly MALAYALAM As New UnicodeBlock("MALAYALAM")

            ''' <summary>
            ''' Constant for the "Thai" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly THAI As New UnicodeBlock("THAI")

            ''' <summary>
            ''' Constant for the "Lao" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly LAO As New UnicodeBlock("LAO")

            ''' <summary>
            ''' Constant for the "Tibetan" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly TIBETAN As New UnicodeBlock("TIBETAN")

            ''' <summary>
            ''' Constant for the "Georgian" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GEORGIAN As New UnicodeBlock("GEORGIAN")

            ''' <summary>
            ''' Constant for the "Hangul Jamo" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly HANGUL_JAMO As New UnicodeBlock("HANGUL_JAMO", "HANGUL JAMO", "HANGULJAMO")

            ''' <summary>
            ''' Constant for the "Latin Extended Additional" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly LATIN_EXTENDED_ADDITIONAL As New UnicodeBlock("LATIN_EXTENDED_ADDITIONAL", "LATIN EXTENDED ADDITIONAL", "LATINEXTENDEDADDITIONAL")

            ''' <summary>
            ''' Constant for the "Greek Extended" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GREEK_EXTENDED As New UnicodeBlock("GREEK_EXTENDED", "GREEK EXTENDED", "GREEKEXTENDED")

            ''' <summary>
            ''' Constant for the "General Punctuation" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GENERAL_PUNCTUATION As New UnicodeBlock("GENERAL_PUNCTUATION", "GENERAL PUNCTUATION", "GENERALPUNCTUATION")

            ''' <summary>
            ''' Constant for the "Superscripts and Subscripts" Unicode character
            ''' block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly SUPERSCRIPTS_AND_SUBSCRIPTS As New UnicodeBlock("SUPERSCRIPTS_AND_SUBSCRIPTS", "SUPERSCRIPTS AND SUBSCRIPTS", "SUPERSCRIPTSANDSUBSCRIPTS")

            ''' <summary>
            ''' Constant for the "Currency Symbols" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CURRENCY_SYMBOLS As New UnicodeBlock("CURRENCY_SYMBOLS", "CURRENCY SYMBOLS", "CURRENCYSYMBOLS")

            ''' <summary>
            ''' Constant for the "Combining Diacritical Marks for Symbols" Unicode
            ''' character block.
            ''' <p>
            ''' This block was previously known as "Combining Marks for Symbols".
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly COMBINING_MARKS_FOR_SYMBOLS As New UnicodeBlock("COMBINING_MARKS_FOR_SYMBOLS", "COMBINING DIACRITICAL MARKS FOR SYMBOLS", "COMBININGDIACRITICALMARKSFORSYMBOLS", "COMBINING MARKS FOR SYMBOLS", "COMBININGMARKSFORSYMBOLS")

            ''' <summary>
            ''' Constant for the "Letterlike Symbols" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly LETTERLIKE_SYMBOLS As New UnicodeBlock("LETTERLIKE_SYMBOLS", "LETTERLIKE SYMBOLS", "LETTERLIKESYMBOLS")

            ''' <summary>
            ''' Constant for the "Number Forms" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly NUMBER_FORMS As New UnicodeBlock("NUMBER_FORMS", "NUMBER FORMS", "NUMBERFORMS")

            ''' <summary>
            ''' Constant for the "Arrows" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ARROWS As New UnicodeBlock("ARROWS")

            ''' <summary>
            ''' Constant for the "Mathematical Operators" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly MATHEMATICAL_OPERATORS As New UnicodeBlock("MATHEMATICAL_OPERATORS", "MATHEMATICAL OPERATORS", "MATHEMATICALOPERATORS")

            ''' <summary>
            ''' Constant for the "Miscellaneous Technical" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly MISCELLANEOUS_TECHNICAL As New UnicodeBlock("MISCELLANEOUS_TECHNICAL", "MISCELLANEOUS TECHNICAL", "MISCELLANEOUSTECHNICAL")

            ''' <summary>
            ''' Constant for the "Control Pictures" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CONTROL_PICTURES As New UnicodeBlock("CONTROL_PICTURES", "CONTROL PICTURES", "CONTROLPICTURES")

            ''' <summary>
            ''' Constant for the "Optical Character Recognition" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly OPTICAL_CHARACTER_RECOGNITION As New UnicodeBlock("OPTICAL_CHARACTER_RECOGNITION", "OPTICAL CHARACTER RECOGNITION", "OPTICALCHARACTERRECOGNITION")

            ''' <summary>
            ''' Constant for the "Enclosed Alphanumerics" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ENCLOSED_ALPHANUMERICS As New UnicodeBlock("ENCLOSED_ALPHANUMERICS", "ENCLOSED ALPHANUMERICS", "ENCLOSEDALPHANUMERICS")

            ''' <summary>
            ''' Constant for the "Box Drawing" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly BOX_DRAWING As New UnicodeBlock("BOX_DRAWING", "BOX DRAWING", "BOXDRAWING")

            ''' <summary>
            ''' Constant for the "Block Elements" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly BLOCK_ELEMENTS As New UnicodeBlock("BLOCK_ELEMENTS", "BLOCK ELEMENTS", "BLOCKELEMENTS")

            ''' <summary>
            ''' Constant for the "Geometric Shapes" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly GEOMETRIC_SHAPES As New UnicodeBlock("GEOMETRIC_SHAPES", "GEOMETRIC SHAPES", "GEOMETRICSHAPES")

            ''' <summary>
            ''' Constant for the "Miscellaneous Symbols" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly MISCELLANEOUS_SYMBOLS As New UnicodeBlock("MISCELLANEOUS_SYMBOLS", "MISCELLANEOUS SYMBOLS", "MISCELLANEOUSSYMBOLS")

            ''' <summary>
            ''' Constant for the "Dingbats" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly DINGBATS As New UnicodeBlock("DINGBATS")

            ''' <summary>
            ''' Constant for the "CJK Symbols and Punctuation" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CJK_SYMBOLS_AND_PUNCTUATION As New UnicodeBlock("CJK_SYMBOLS_AND_PUNCTUATION", "CJK SYMBOLS AND PUNCTUATION", "CJKSYMBOLSANDPUNCTUATION")

            ''' <summary>
            ''' Constant for the "Hiragana" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly HIRAGANA As New UnicodeBlock("HIRAGANA")

            ''' <summary>
            ''' Constant for the "Katakana" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly KATAKANA As New UnicodeBlock("KATAKANA")

            ''' <summary>
            ''' Constant for the "Bopomofo" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly BOPOMOFO As New UnicodeBlock("BOPOMOFO")

            ''' <summary>
            ''' Constant for the "Hangul Compatibility Jamo" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly HANGUL_COMPATIBILITY_JAMO As New UnicodeBlock("HANGUL_COMPATIBILITY_JAMO", "HANGUL COMPATIBILITY JAMO", "HANGULCOMPATIBILITYJAMO")

            ''' <summary>
            ''' Constant for the "Kanbun" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly KANBUN As New UnicodeBlock("KANBUN")

            ''' <summary>
            ''' Constant for the "Enclosed CJK Letters and Months" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ENCLOSED_CJK_LETTERS_AND_MONTHS As New UnicodeBlock("ENCLOSED_CJK_LETTERS_AND_MONTHS", "ENCLOSED CJK LETTERS AND MONTHS", "ENCLOSEDCJKLETTERSANDMONTHS")

            ''' <summary>
            ''' Constant for the "CJK Compatibility" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CJK_COMPATIBILITY As New UnicodeBlock("CJK_COMPATIBILITY", "CJK COMPATIBILITY", "CJKCOMPATIBILITY")

            ''' <summary>
            ''' Constant for the "CJK Unified Ideographs" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CJK_UNIFIED_IDEOGRAPHS As New UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS", "CJK UNIFIED IDEOGRAPHS", "CJKUNIFIEDIDEOGRAPHS")

            ''' <summary>
            ''' Constant for the "Hangul Syllables" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly HANGUL_SYLLABLES As New UnicodeBlock("HANGUL_SYLLABLES", "HANGUL SYLLABLES", "HANGULSYLLABLES")

            ''' <summary>
            ''' Constant for the "Private Use Area" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly PRIVATE_USE_AREA As New UnicodeBlock("PRIVATE_USE_AREA", "PRIVATE USE AREA", "PRIVATEUSEAREA")

            ''' <summary>
            ''' Constant for the "CJK Compatibility Ideographs" Unicode character
            ''' block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CJK_COMPATIBILITY_IDEOGRAPHS As New UnicodeBlock("CJK_COMPATIBILITY_IDEOGRAPHS", "CJK COMPATIBILITY IDEOGRAPHS", "CJKCOMPATIBILITYIDEOGRAPHS")

            ''' <summary>
            ''' Constant for the "Alphabetic Presentation Forms" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ALPHABETIC_PRESENTATION_FORMS As New UnicodeBlock("ALPHABETIC_PRESENTATION_FORMS", "ALPHABETIC PRESENTATION FORMS", "ALPHABETICPRESENTATIONFORMS")

            ''' <summary>
            ''' Constant for the "Arabic Presentation Forms-A" Unicode character
            ''' block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ARABIC_PRESENTATION_FORMS_A As New UnicodeBlock("ARABIC_PRESENTATION_FORMS_A", "ARABIC PRESENTATION FORMS-A", "ARABICPRESENTATIONFORMS-A")

            ''' <summary>
            ''' Constant for the "Combining Half Marks" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly COMBINING_HALF_MARKS As New UnicodeBlock("COMBINING_HALF_MARKS", "COMBINING HALF MARKS", "COMBININGHALFMARKS")

            ''' <summary>
            ''' Constant for the "CJK Compatibility Forms" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly CJK_COMPATIBILITY_FORMS As New UnicodeBlock("CJK_COMPATIBILITY_FORMS", "CJK COMPATIBILITY FORMS", "CJKCOMPATIBILITYFORMS")

            ''' <summary>
            ''' Constant for the "Small Form Variants" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly SMALL_FORM_VARIANTS As New UnicodeBlock("SMALL_FORM_VARIANTS", "SMALL FORM VARIANTS", "SMALLFORMVARIANTS")

            ''' <summary>
            ''' Constant for the "Arabic Presentation Forms-B" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly ARABIC_PRESENTATION_FORMS_B As New UnicodeBlock("ARABIC_PRESENTATION_FORMS_B", "ARABIC PRESENTATION FORMS-B", "ARABICPRESENTATIONFORMS-B")

            ''' <summary>
            ''' Constant for the "Halfwidth and Fullwidth Forms" Unicode character
            ''' block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly HALFWIDTH_AND_FULLWIDTH_FORMS As New UnicodeBlock("HALFWIDTH_AND_FULLWIDTH_FORMS", "HALFWIDTH AND FULLWIDTH FORMS", "HALFWIDTHANDFULLWIDTHFORMS")

            ''' <summary>
            ''' Constant for the "Specials" Unicode character block.
            ''' @since 1.2
            ''' </summary>
            Public Shared ReadOnly SPECIALS As New UnicodeBlock("SPECIALS")

            ''' @deprecated As of J2SE 5, use <seealso cref="#HIGH_SURROGATES"/>,
            '''             <seealso cref="#HIGH_PRIVATE_USE_SURROGATES"/>, and
            '''             <seealso cref="#LOW_SURROGATES"/>. These new constants match
            '''             the block definitions of the Unicode Standard.
            '''             The <seealso cref="#of(char)"/> and <seealso cref="#of(int)"/> methods
            '''             return the new constants, not SURROGATES_AREA. 
            <Obsolete("As of J2SE 5, use <seealso cref="#HIGH_SURROGATES"/>,")>
            Public Shared ReadOnly SURROGATES_AREA As New UnicodeBlock("SURROGATES_AREA")

            ''' <summary>
            ''' Constant for the "Syriac" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly SYRIAC As New UnicodeBlock("SYRIAC")

            ''' <summary>
            ''' Constant for the "Thaana" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly THAANA As New UnicodeBlock("THAANA")

            ''' <summary>
            ''' Constant for the "Sinhala" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly SINHALA As New UnicodeBlock("SINHALA")

            ''' <summary>
            ''' Constant for the "Myanmar" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly MYANMAR As New UnicodeBlock("MYANMAR")

            ''' <summary>
            ''' Constant for the "Ethiopic" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly ETHIOPIC As New UnicodeBlock("ETHIOPIC")

            ''' <summary>
            ''' Constant for the "Cherokee" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly CHEROKEE As New UnicodeBlock("CHEROKEE")

            ''' <summary>
            ''' Constant for the "Unified Canadian Aboriginal Syllabics" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS As New UnicodeBlock("UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS", "UNIFIED CANADIAN ABORIGINAL SYLLABICS", "UNIFIEDCANADIANABORIGINALSYLLABICS")

            ''' <summary>
            ''' Constant for the "Ogham" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly OGHAM As New UnicodeBlock("OGHAM")

            ''' <summary>
            ''' Constant for the "Runic" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly RUNIC As New UnicodeBlock("RUNIC")

            ''' <summary>
            ''' Constant for the "Khmer" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly KHMER As New UnicodeBlock("KHMER")

            ''' <summary>
            ''' Constant for the "Mongolian" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly MONGOLIAN As New UnicodeBlock("MONGOLIAN")

            ''' <summary>
            ''' Constant for the "Braille Patterns" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly BRAILLE_PATTERNS As New UnicodeBlock("BRAILLE_PATTERNS", "BRAILLE PATTERNS", "BRAILLEPATTERNS")

            ''' <summary>
            ''' Constant for the "CJK Radicals Supplement" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly CJK_RADICALS_SUPPLEMENT As New UnicodeBlock("CJK_RADICALS_SUPPLEMENT", "CJK RADICALS SUPPLEMENT", "CJKRADICALSSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Kangxi Radicals" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly KANGXI_RADICALS As New UnicodeBlock("KANGXI_RADICALS", "KANGXI RADICALS", "KANGXIRADICALS")

            ''' <summary>
            ''' Constant for the "Ideographic Description Characters" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly IDEOGRAPHIC_DESCRIPTION_CHARACTERS As New UnicodeBlock("IDEOGRAPHIC_DESCRIPTION_CHARACTERS", "IDEOGRAPHIC DESCRIPTION CHARACTERS", "IDEOGRAPHICDESCRIPTIONCHARACTERS")

            ''' <summary>
            ''' Constant for the "Bopomofo Extended" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly BOPOMOFO_EXTENDED As New UnicodeBlock("BOPOMOFO_EXTENDED", "BOPOMOFO EXTENDED", "BOPOMOFOEXTENDED")

            ''' <summary>
            ''' Constant for the "CJK Unified Ideographs Extension A" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A As New UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A", "CJK UNIFIED IDEOGRAPHS EXTENSION A", "CJKUNIFIEDIDEOGRAPHSEXTENSIONA")

            ''' <summary>
            ''' Constant for the "Yi Syllables" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly YI_SYLLABLES As New UnicodeBlock("YI_SYLLABLES", "YI SYLLABLES", "YISYLLABLES")

            ''' <summary>
            ''' Constant for the "Yi Radicals" Unicode character block.
            ''' @since 1.4
            ''' </summary>
            Public Shared ReadOnly YI_RADICALS As New UnicodeBlock("YI_RADICALS", "YI RADICALS", "YIRADICALS")

            ''' <summary>
            ''' Constant for the "Cyrillic Supplementary" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly CYRILLIC_SUPPLEMENTARY As New UnicodeBlock("CYRILLIC_SUPPLEMENTARY", "CYRILLIC SUPPLEMENTARY", "CYRILLICSUPPLEMENTARY", "CYRILLIC SUPPLEMENT", "CYRILLICSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Tagalog" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly TAGALOG As New UnicodeBlock("TAGALOG")

            ''' <summary>
            ''' Constant for the "Hanunoo" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly HANUNOO As New UnicodeBlock("HANUNOO")

            ''' <summary>
            ''' Constant for the "Buhid" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly BUHID As New UnicodeBlock("BUHID")

            ''' <summary>
            ''' Constant for the "Tagbanwa" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly TAGBANWA As New UnicodeBlock("TAGBANWA")

            ''' <summary>
            ''' Constant for the "Limbu" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly LIMBU As New UnicodeBlock("LIMBU")

            ''' <summary>
            ''' Constant for the "Tai Le" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly TAI_LE As New UnicodeBlock("TAI_LE", "TAI LE", "TAILE")

            ''' <summary>
            ''' Constant for the "Khmer Symbols" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly KHMER_SYMBOLS As New UnicodeBlock("KHMER_SYMBOLS", "KHMER SYMBOLS", "KHMERSYMBOLS")

            ''' <summary>
            ''' Constant for the "Phonetic Extensions" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly PHONETIC_EXTENSIONS As New UnicodeBlock("PHONETIC_EXTENSIONS", "PHONETIC EXTENSIONS", "PHONETICEXTENSIONS")

            ''' <summary>
            ''' Constant for the "Miscellaneous Mathematical Symbols-A" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A As New UnicodeBlock("MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A", "MISCELLANEOUS MATHEMATICAL SYMBOLS-A", "MISCELLANEOUSMATHEMATICALSYMBOLS-A")

            ''' <summary>
            ''' Constant for the "Supplemental Arrows-A" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly SUPPLEMENTAL_ARROWS_A As New UnicodeBlock("SUPPLEMENTAL_ARROWS_A", "SUPPLEMENTAL ARROWS-A", "SUPPLEMENTALARROWS-A")

            ''' <summary>
            ''' Constant for the "Supplemental Arrows-B" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly SUPPLEMENTAL_ARROWS_B As New UnicodeBlock("SUPPLEMENTAL_ARROWS_B", "SUPPLEMENTAL ARROWS-B", "SUPPLEMENTALARROWS-B")

            ''' <summary>
            ''' Constant for the "Miscellaneous Mathematical Symbols-B" Unicode
            ''' character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B As New UnicodeBlock("MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B", "MISCELLANEOUS MATHEMATICAL SYMBOLS-B", "MISCELLANEOUSMATHEMATICALSYMBOLS-B")

            ''' <summary>
            ''' Constant for the "Supplemental Mathematical Operators" Unicode
            ''' character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly SUPPLEMENTAL_MATHEMATICAL_OPERATORS As New UnicodeBlock("SUPPLEMENTAL_MATHEMATICAL_OPERATORS", "SUPPLEMENTAL MATHEMATICAL OPERATORS", "SUPPLEMENTALMATHEMATICALOPERATORS")

            ''' <summary>
            ''' Constant for the "Miscellaneous Symbols and Arrows" Unicode character
            ''' block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly MISCELLANEOUS_SYMBOLS_AND_ARROWS As New UnicodeBlock("MISCELLANEOUS_SYMBOLS_AND_ARROWS", "MISCELLANEOUS SYMBOLS AND ARROWS", "MISCELLANEOUSSYMBOLSANDARROWS")

            ''' <summary>
            ''' Constant for the "Katakana Phonetic Extensions" Unicode character
            ''' block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly KATAKANA_PHONETIC_EXTENSIONS As New UnicodeBlock("KATAKANA_PHONETIC_EXTENSIONS", "KATAKANA PHONETIC EXTENSIONS", "KATAKANAPHONETICEXTENSIONS")

            ''' <summary>
            ''' Constant for the "Yijing Hexagram Symbols" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly YIJING_HEXAGRAM_SYMBOLS As New UnicodeBlock("YIJING_HEXAGRAM_SYMBOLS", "YIJING HEXAGRAM SYMBOLS", "YIJINGHEXAGRAMSYMBOLS")

            ''' <summary>
            ''' Constant for the "Variation Selectors" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly VARIATION_SELECTORS As New UnicodeBlock("VARIATION_SELECTORS", "VARIATION SELECTORS", "VARIATIONSELECTORS")

            ''' <summary>
            ''' Constant for the "Linear B Syllabary" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly LINEAR_B_SYLLABARY As New UnicodeBlock("LINEAR_B_SYLLABARY", "LINEAR B SYLLABARY", "LINEARBSYLLABARY")

            ''' <summary>
            ''' Constant for the "Linear B Ideograms" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly LINEAR_B_IDEOGRAMS As New UnicodeBlock("LINEAR_B_IDEOGRAMS", "LINEAR B IDEOGRAMS", "LINEARBIDEOGRAMS")

            ''' <summary>
            ''' Constant for the "Aegean Numbers" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly AEGEAN_NUMBERS As New UnicodeBlock("AEGEAN_NUMBERS", "AEGEAN NUMBERS", "AEGEANNUMBERS")

            ''' <summary>
            ''' Constant for the "Old Italic" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly OLD_ITALIC As New UnicodeBlock("OLD_ITALIC", "OLD ITALIC", "OLDITALIC")

            ''' <summary>
            ''' Constant for the "Gothic" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly GOTHIC As New UnicodeBlock("GOTHIC")

            ''' <summary>
            ''' Constant for the "Ugaritic" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly UGARITIC As New UnicodeBlock("UGARITIC")

            ''' <summary>
            ''' Constant for the "Deseret" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly DESERET As New UnicodeBlock("DESERET")

            ''' <summary>
            ''' Constant for the "Shavian" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly SHAVIAN As New UnicodeBlock("SHAVIAN")

            ''' <summary>
            ''' Constant for the "Osmanya" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly OSMANYA As New UnicodeBlock("OSMANYA")

            ''' <summary>
            ''' Constant for the "Cypriot Syllabary" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly CYPRIOT_SYLLABARY As New UnicodeBlock("CYPRIOT_SYLLABARY", "CYPRIOT SYLLABARY", "CYPRIOTSYLLABARY")

            ''' <summary>
            ''' Constant for the "Byzantine Musical Symbols" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly BYZANTINE_MUSICAL_SYMBOLS As New UnicodeBlock("BYZANTINE_MUSICAL_SYMBOLS", "BYZANTINE MUSICAL SYMBOLS", "BYZANTINEMUSICALSYMBOLS")

            ''' <summary>
            ''' Constant for the "Musical Symbols" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly MUSICAL_SYMBOLS As New UnicodeBlock("MUSICAL_SYMBOLS", "MUSICAL SYMBOLS", "MUSICALSYMBOLS")

            ''' <summary>
            ''' Constant for the "Tai Xuan Jing Symbols" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly TAI_XUAN_JING_SYMBOLS As New UnicodeBlock("TAI_XUAN_JING_SYMBOLS", "TAI XUAN JING SYMBOLS", "TAIXUANJINGSYMBOLS")

            ''' <summary>
            ''' Constant for the "Mathematical Alphanumeric Symbols" Unicode
            ''' character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly MATHEMATICAL_ALPHANUMERIC_SYMBOLS As New UnicodeBlock("MATHEMATICAL_ALPHANUMERIC_SYMBOLS", "MATHEMATICAL ALPHANUMERIC SYMBOLS", "MATHEMATICALALPHANUMERICSYMBOLS")

            ''' <summary>
            ''' Constant for the "CJK Unified Ideographs Extension B" Unicode
            ''' character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B As New UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B", "CJK UNIFIED IDEOGRAPHS EXTENSION B", "CJKUNIFIEDIDEOGRAPHSEXTENSIONB")

            ''' <summary>
            ''' Constant for the "CJK Compatibility Ideographs Supplement" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT As New UnicodeBlock("CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT", "CJK COMPATIBILITY IDEOGRAPHS SUPPLEMENT", "CJKCOMPATIBILITYIDEOGRAPHSSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Tags" Unicode character block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly TAGS As New UnicodeBlock("TAGS")

            ''' <summary>
            ''' Constant for the "Variation Selectors Supplement" Unicode character
            ''' block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly VARIATION_SELECTORS_SUPPLEMENT As New UnicodeBlock("VARIATION_SELECTORS_SUPPLEMENT", "VARIATION SELECTORS SUPPLEMENT", "VARIATIONSELECTORSSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Supplementary Private Use Area-A" Unicode character
            ''' block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly SUPPLEMENTARY_PRIVATE_USE_AREA_A As New UnicodeBlock("SUPPLEMENTARY_PRIVATE_USE_AREA_A", "SUPPLEMENTARY PRIVATE USE AREA-A", "SUPPLEMENTARYPRIVATEUSEAREA-A")

            ''' <summary>
            ''' Constant for the "Supplementary Private Use Area-B" Unicode character
            ''' block.
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly SUPPLEMENTARY_PRIVATE_USE_AREA_B As New UnicodeBlock("SUPPLEMENTARY_PRIVATE_USE_AREA_B", "SUPPLEMENTARY PRIVATE USE AREA-B", "SUPPLEMENTARYPRIVATEUSEAREA-B")

            ''' <summary>
            ''' Constant for the "High Surrogates" Unicode character block.
            ''' This block represents codepoint values in the high surrogate
            ''' range: U+D800 through U+DB7F
            ''' 
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly HIGH_SURROGATES As New UnicodeBlock("HIGH_SURROGATES", "HIGH SURROGATES", "HIGHSURROGATES")

            ''' <summary>
            ''' Constant for the "High Private Use Surrogates" Unicode character
            ''' block.
            ''' This block represents codepoint values in the private use high
            ''' surrogate range: U+DB80 through U+DBFF
            ''' 
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly HIGH_PRIVATE_USE_SURROGATES As New UnicodeBlock("HIGH_PRIVATE_USE_SURROGATES", "HIGH PRIVATE USE SURROGATES", "HIGHPRIVATEUSESURROGATES")

            ''' <summary>
            ''' Constant for the "Low Surrogates" Unicode character block.
            ''' This block represents codepoint values in the low surrogate
            ''' range: U+DC00 through U+DFFF
            ''' 
            ''' @since 1.5
            ''' </summary>
            Public Shared ReadOnly LOW_SURROGATES As New UnicodeBlock("LOW_SURROGATES", "LOW SURROGATES", "LOWSURROGATES")

            ''' <summary>
            ''' Constant for the "Arabic Supplement" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ARABIC_SUPPLEMENT As New UnicodeBlock("ARABIC_SUPPLEMENT", "ARABIC SUPPLEMENT", "ARABICSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "NKo" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly NKO As New UnicodeBlock("NKO")

            ''' <summary>
            ''' Constant for the "Samaritan" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly SAMARITAN As New UnicodeBlock("SAMARITAN")

            ''' <summary>
            ''' Constant for the "Mandaic" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly MANDAIC As New UnicodeBlock("MANDAIC")

            ''' <summary>
            ''' Constant for the "Ethiopic Supplement" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ETHIOPIC_SUPPLEMENT As New UnicodeBlock("ETHIOPIC_SUPPLEMENT", "ETHIOPIC SUPPLEMENT", "ETHIOPICSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Unified Canadian Aboriginal Syllabics Extended"
            ''' Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED As New UnicodeBlock("UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED", "UNIFIED CANADIAN ABORIGINAL SYLLABICS EXTENDED", "UNIFIEDCANADIANABORIGINALSYLLABICSEXTENDED")

            ''' <summary>
            ''' Constant for the "New Tai Lue" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly NEW_TAI_LUE As New UnicodeBlock("NEW_TAI_LUE", "NEW TAI LUE", "NEWTAILUE")

            ''' <summary>
            ''' Constant for the "Buginese" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly BUGINESE As New UnicodeBlock("BUGINESE")

            ''' <summary>
            ''' Constant for the "Tai Tham" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly TAI_THAM As New UnicodeBlock("TAI_THAM", "TAI THAM", "TAITHAM")

            ''' <summary>
            ''' Constant for the "Balinese" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly BALINESE As New UnicodeBlock("BALINESE")

            ''' <summary>
            ''' Constant for the "Sundanese" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly SUNDANESE As New UnicodeBlock("SUNDANESE")

            ''' <summary>
            ''' Constant for the "Batak" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly BATAK As New UnicodeBlock("BATAK")

            ''' <summary>
            ''' Constant for the "Lepcha" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly LEPCHA As New UnicodeBlock("LEPCHA")

            ''' <summary>
            ''' Constant for the "Ol Chiki" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly OL_CHIKI As New UnicodeBlock("OL_CHIKI", "OL CHIKI", "OLCHIKI")

            ''' <summary>
            ''' Constant for the "Vedic Extensions" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly VEDIC_EXTENSIONS As New UnicodeBlock("VEDIC_EXTENSIONS", "VEDIC EXTENSIONS", "VEDICEXTENSIONS")

            ''' <summary>
            ''' Constant for the "Phonetic Extensions Supplement" Unicode character
            ''' block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly PHONETIC_EXTENSIONS_SUPPLEMENT As New UnicodeBlock("PHONETIC_EXTENSIONS_SUPPLEMENT", "PHONETIC EXTENSIONS SUPPLEMENT", "PHONETICEXTENSIONSSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Combining Diacritical Marks Supplement" Unicode
            ''' character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly COMBINING_DIACRITICAL_MARKS_SUPPLEMENT As New UnicodeBlock("COMBINING_DIACRITICAL_MARKS_SUPPLEMENT", "COMBINING DIACRITICAL MARKS SUPPLEMENT", "COMBININGDIACRITICALMARKSSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Glagolitic" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly GLAGOLITIC As New UnicodeBlock("GLAGOLITIC")

            ''' <summary>
            ''' Constant for the "Latin Extended-C" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly LATIN_EXTENDED_C As New UnicodeBlock("LATIN_EXTENDED_C", "LATIN EXTENDED-C", "LATINEXTENDED-C")

            ''' <summary>
            ''' Constant for the "Coptic" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly COPTIC As New UnicodeBlock("COPTIC")

            ''' <summary>
            ''' Constant for the "Georgian Supplement" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly GEORGIAN_SUPPLEMENT As New UnicodeBlock("GEORGIAN_SUPPLEMENT", "GEORGIAN SUPPLEMENT", "GEORGIANSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Tifinagh" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly TIFINAGH As New UnicodeBlock("TIFINAGH")

            ''' <summary>
            ''' Constant for the "Ethiopic Extended" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ETHIOPIC_EXTENDED As New UnicodeBlock("ETHIOPIC_EXTENDED", "ETHIOPIC EXTENDED", "ETHIOPICEXTENDED")

            ''' <summary>
            ''' Constant for the "Cyrillic Extended-A" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CYRILLIC_EXTENDED_A As New UnicodeBlock("CYRILLIC_EXTENDED_A", "CYRILLIC EXTENDED-A", "CYRILLICEXTENDED-A")

            ''' <summary>
            ''' Constant for the "Supplemental Punctuation" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly SUPPLEMENTAL_PUNCTUATION As New UnicodeBlock("SUPPLEMENTAL_PUNCTUATION", "SUPPLEMENTAL PUNCTUATION", "SUPPLEMENTALPUNCTUATION")

            ''' <summary>
            ''' Constant for the "CJK Strokes" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CJK_STROKES As New UnicodeBlock("CJK_STROKES", "CJK STROKES", "CJKSTROKES")

            ''' <summary>
            ''' Constant for the "Lisu" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly LISU As New UnicodeBlock("LISU")

            ''' <summary>
            ''' Constant for the "Vai" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly VAI As New UnicodeBlock("VAI")

            ''' <summary>
            ''' Constant for the "Cyrillic Extended-B" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CYRILLIC_EXTENDED_B As New UnicodeBlock("CYRILLIC_EXTENDED_B", "CYRILLIC EXTENDED-B", "CYRILLICEXTENDED-B")

            ''' <summary>
            ''' Constant for the "Bamum" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly BAMUM As New UnicodeBlock("BAMUM")

            ''' <summary>
            ''' Constant for the "Modifier Tone Letters" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly MODIFIER_TONE_LETTERS As New UnicodeBlock("MODIFIER_TONE_LETTERS", "MODIFIER TONE LETTERS", "MODIFIERTONELETTERS")

            ''' <summary>
            ''' Constant for the "Latin Extended-D" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly LATIN_EXTENDED_D As New UnicodeBlock("LATIN_EXTENDED_D", "LATIN EXTENDED-D", "LATINEXTENDED-D")

            ''' <summary>
            ''' Constant for the "Syloti Nagri" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly SYLOTI_NAGRI As New UnicodeBlock("SYLOTI_NAGRI", "SYLOTI NAGRI", "SYLOTINAGRI")

            ''' <summary>
            ''' Constant for the "Common Indic Number Forms" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly COMMON_INDIC_NUMBER_FORMS As New UnicodeBlock("COMMON_INDIC_NUMBER_FORMS", "COMMON INDIC NUMBER FORMS", "COMMONINDICNUMBERFORMS")

            ''' <summary>
            ''' Constant for the "Phags-pa" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly PHAGS_PA As New UnicodeBlock("PHAGS_PA", "PHAGS-PA")

            ''' <summary>
            ''' Constant for the "Saurashtra" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly SAURASHTRA As New UnicodeBlock("SAURASHTRA")

            ''' <summary>
            ''' Constant for the "Devanagari Extended" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly DEVANAGARI_EXTENDED As New UnicodeBlock("DEVANAGARI_EXTENDED", "DEVANAGARI EXTENDED", "DEVANAGARIEXTENDED")

            ''' <summary>
            ''' Constant for the "Kayah Li" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly KAYAH_LI As New UnicodeBlock("KAYAH_LI", "KAYAH LI", "KAYAHLI")

            ''' <summary>
            ''' Constant for the "Rejang" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly REJANG As New UnicodeBlock("REJANG")

            ''' <summary>
            ''' Constant for the "Hangul Jamo Extended-A" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly HANGUL_JAMO_EXTENDED_A As New UnicodeBlock("HANGUL_JAMO_EXTENDED_A", "HANGUL JAMO EXTENDED-A", "HANGULJAMOEXTENDED-A")

            ''' <summary>
            ''' Constant for the "Javanese" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly JAVANESE As New UnicodeBlock("JAVANESE")

            ''' <summary>
            ''' Constant for the "Cham" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CHAM As New UnicodeBlock("CHAM")

            ''' <summary>
            ''' Constant for the "Myanmar Extended-A" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly MYANMAR_EXTENDED_A As New UnicodeBlock("MYANMAR_EXTENDED_A", "MYANMAR EXTENDED-A", "MYANMAREXTENDED-A")

            ''' <summary>
            ''' Constant for the "Tai Viet" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly TAI_VIET As New UnicodeBlock("TAI_VIET", "TAI VIET", "TAIVIET")

            ''' <summary>
            ''' Constant for the "Ethiopic Extended-A" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ETHIOPIC_EXTENDED_A As New UnicodeBlock("ETHIOPIC_EXTENDED_A", "ETHIOPIC EXTENDED-A", "ETHIOPICEXTENDED-A")

            ''' <summary>
            ''' Constant for the "Meetei Mayek" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly MEETEI_MAYEK As New UnicodeBlock("MEETEI_MAYEK", "MEETEI MAYEK", "MEETEIMAYEK")

            ''' <summary>
            ''' Constant for the "Hangul Jamo Extended-B" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly HANGUL_JAMO_EXTENDED_B As New UnicodeBlock("HANGUL_JAMO_EXTENDED_B", "HANGUL JAMO EXTENDED-B", "HANGULJAMOEXTENDED-B")

            ''' <summary>
            ''' Constant for the "Vertical Forms" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly VERTICAL_FORMS As New UnicodeBlock("VERTICAL_FORMS", "VERTICAL FORMS", "VERTICALFORMS")

            ''' <summary>
            ''' Constant for the "Ancient Greek Numbers" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ANCIENT_GREEK_NUMBERS As New UnicodeBlock("ANCIENT_GREEK_NUMBERS", "ANCIENT GREEK NUMBERS", "ANCIENTGREEKNUMBERS")

            ''' <summary>
            ''' Constant for the "Ancient Symbols" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ANCIENT_SYMBOLS As New UnicodeBlock("ANCIENT_SYMBOLS", "ANCIENT SYMBOLS", "ANCIENTSYMBOLS")

            ''' <summary>
            ''' Constant for the "Phaistos Disc" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly PHAISTOS_DISC As New UnicodeBlock("PHAISTOS_DISC", "PHAISTOS DISC", "PHAISTOSDISC")

            ''' <summary>
            ''' Constant for the "Lycian" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly LYCIAN As New UnicodeBlock("LYCIAN")

            ''' <summary>
            ''' Constant for the "Carian" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CARIAN As New UnicodeBlock("CARIAN")

            ''' <summary>
            ''' Constant for the "Old Persian" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly OLD_PERSIAN As New UnicodeBlock("OLD_PERSIAN", "OLD PERSIAN", "OLDPERSIAN")

            ''' <summary>
            ''' Constant for the "Imperial Aramaic" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly IMPERIAL_ARAMAIC As New UnicodeBlock("IMPERIAL_ARAMAIC", "IMPERIAL ARAMAIC", "IMPERIALARAMAIC")

            ''' <summary>
            ''' Constant for the "Phoenician" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly PHOENICIAN As New UnicodeBlock("PHOENICIAN")

            ''' <summary>
            ''' Constant for the "Lydian" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly LYDIAN As New UnicodeBlock("LYDIAN")

            ''' <summary>
            ''' Constant for the "Kharoshthi" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly KHAROSHTHI As New UnicodeBlock("KHAROSHTHI")

            ''' <summary>
            ''' Constant for the "Old South Arabian" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly OLD_SOUTH_ARABIAN As New UnicodeBlock("OLD_SOUTH_ARABIAN", "OLD SOUTH ARABIAN", "OLDSOUTHARABIAN")

            ''' <summary>
            ''' Constant for the "Avestan" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly AVESTAN As New UnicodeBlock("AVESTAN")

            ''' <summary>
            ''' Constant for the "Inscriptional Parthian" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly INSCRIPTIONAL_PARTHIAN As New UnicodeBlock("INSCRIPTIONAL_PARTHIAN", "INSCRIPTIONAL PARTHIAN", "INSCRIPTIONALPARTHIAN")

            ''' <summary>
            ''' Constant for the "Inscriptional Pahlavi" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly INSCRIPTIONAL_PAHLAVI As New UnicodeBlock("INSCRIPTIONAL_PAHLAVI", "INSCRIPTIONAL PAHLAVI", "INSCRIPTIONALPAHLAVI")

            ''' <summary>
            ''' Constant for the "Old Turkic" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly OLD_TURKIC As New UnicodeBlock("OLD_TURKIC", "OLD TURKIC", "OLDTURKIC")

            ''' <summary>
            ''' Constant for the "Rumi Numeral Symbols" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly RUMI_NUMERAL_SYMBOLS As New UnicodeBlock("RUMI_NUMERAL_SYMBOLS", "RUMI NUMERAL SYMBOLS", "RUMINUMERALSYMBOLS")

            ''' <summary>
            ''' Constant for the "Brahmi" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly BRAHMI As New UnicodeBlock("BRAHMI")

            ''' <summary>
            ''' Constant for the "Kaithi" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly KAITHI As New UnicodeBlock("KAITHI")

            ''' <summary>
            ''' Constant for the "Cuneiform" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CUNEIFORM As New UnicodeBlock("CUNEIFORM")

            ''' <summary>
            ''' Constant for the "Cuneiform Numbers and Punctuation" Unicode
            ''' character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CUNEIFORM_NUMBERS_AND_PUNCTUATION As New UnicodeBlock("CUNEIFORM_NUMBERS_AND_PUNCTUATION", "CUNEIFORM NUMBERS AND PUNCTUATION", "CUNEIFORMNUMBERSANDPUNCTUATION")

            ''' <summary>
            ''' Constant for the "Egyptian Hieroglyphs" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly EGYPTIAN_HIEROGLYPHS As New UnicodeBlock("EGYPTIAN_HIEROGLYPHS", "EGYPTIAN HIEROGLYPHS", "EGYPTIANHIEROGLYPHS")

            ''' <summary>
            ''' Constant for the "Bamum Supplement" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly BAMUM_SUPPLEMENT As New UnicodeBlock("BAMUM_SUPPLEMENT", "BAMUM SUPPLEMENT", "BAMUMSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Kana Supplement" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly KANA_SUPPLEMENT As New UnicodeBlock("KANA_SUPPLEMENT", "KANA SUPPLEMENT", "KANASUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Ancient Greek Musical Notation" Unicode character
            ''' block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ANCIENT_GREEK_MUSICAL_NOTATION As New UnicodeBlock("ANCIENT_GREEK_MUSICAL_NOTATION", "ANCIENT GREEK MUSICAL NOTATION", "ANCIENTGREEKMUSICALNOTATION")

            ''' <summary>
            ''' Constant for the "Counting Rod Numerals" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly COUNTING_ROD_NUMERALS As New UnicodeBlock("COUNTING_ROD_NUMERALS", "COUNTING ROD NUMERALS", "COUNTINGRODNUMERALS")

            ''' <summary>
            ''' Constant for the "Mahjong Tiles" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly MAHJONG_TILES As New UnicodeBlock("MAHJONG_TILES", "MAHJONG TILES", "MAHJONGTILES")

            ''' <summary>
            ''' Constant for the "Domino Tiles" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly DOMINO_TILES As New UnicodeBlock("DOMINO_TILES", "DOMINO TILES", "DOMINOTILES")

            ''' <summary>
            ''' Constant for the "Playing Cards" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly PLAYING_CARDS As New UnicodeBlock("PLAYING_CARDS", "PLAYING CARDS", "PLAYINGCARDS")

            ''' <summary>
            ''' Constant for the "Enclosed Alphanumeric Supplement" Unicode character
            ''' block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ENCLOSED_ALPHANUMERIC_SUPPLEMENT As New UnicodeBlock("ENCLOSED_ALPHANUMERIC_SUPPLEMENT", "ENCLOSED ALPHANUMERIC SUPPLEMENT", "ENCLOSEDALPHANUMERICSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Enclosed Ideographic Supplement" Unicode character
            ''' block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ENCLOSED_IDEOGRAPHIC_SUPPLEMENT As New UnicodeBlock("ENCLOSED_IDEOGRAPHIC_SUPPLEMENT", "ENCLOSED IDEOGRAPHIC SUPPLEMENT", "ENCLOSEDIDEOGRAPHICSUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Miscellaneous Symbols And Pictographs" Unicode
            ''' character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS As New UnicodeBlock("MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS", "MISCELLANEOUS SYMBOLS AND PICTOGRAPHS", "MISCELLANEOUSSYMBOLSANDPICTOGRAPHS")

            ''' <summary>
            ''' Constant for the "Emoticons" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly EMOTICONS As New UnicodeBlock("EMOTICONS")

            ''' <summary>
            ''' Constant for the "Transport And Map Symbols" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly TRANSPORT_AND_MAP_SYMBOLS As New UnicodeBlock("TRANSPORT_AND_MAP_SYMBOLS", "TRANSPORT AND MAP SYMBOLS", "TRANSPORTANDMAPSYMBOLS")

            ''' <summary>
            ''' Constant for the "Alchemical Symbols" Unicode character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly ALCHEMICAL_SYMBOLS As New UnicodeBlock("ALCHEMICAL_SYMBOLS", "ALCHEMICAL SYMBOLS", "ALCHEMICALSYMBOLS")

            ''' <summary>
            ''' Constant for the "CJK Unified Ideographs Extension C" Unicode
            ''' character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C As New UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C", "CJK UNIFIED IDEOGRAPHS EXTENSION C", "CJKUNIFIEDIDEOGRAPHSEXTENSIONC")

            ''' <summary>
            ''' Constant for the "CJK Unified Ideographs Extension D" Unicode
            ''' character block.
            ''' @since 1.7
            ''' </summary>
            Public Shared ReadOnly CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D As New UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D", "CJK UNIFIED IDEOGRAPHS EXTENSION D", "CJKUNIFIEDIDEOGRAPHSEXTENSIOND")

            ''' <summary>
            ''' Constant for the "Arabic Extended-A" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly ARABIC_EXTENDED_A As New UnicodeBlock("ARABIC_EXTENDED_A", "ARABIC EXTENDED-A", "ARABICEXTENDED-A")

            ''' <summary>
            ''' Constant for the "Sundanese Supplement" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly SUNDANESE_SUPPLEMENT As New UnicodeBlock("SUNDANESE_SUPPLEMENT", "SUNDANESE SUPPLEMENT", "SUNDANESESUPPLEMENT")

            ''' <summary>
            ''' Constant for the "Meetei Mayek Extensions" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly MEETEI_MAYEK_EXTENSIONS As New UnicodeBlock("MEETEI_MAYEK_EXTENSIONS", "MEETEI MAYEK EXTENSIONS", "MEETEIMAYEKEXTENSIONS")

            ''' <summary>
            ''' Constant for the "Meroitic Hieroglyphs" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly MEROITIC_HIEROGLYPHS As New UnicodeBlock("MEROITIC_HIEROGLYPHS", "MEROITIC HIEROGLYPHS", "MEROITICHIEROGLYPHS")

            ''' <summary>
            ''' Constant for the "Meroitic Cursive" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly MEROITIC_CURSIVE As New UnicodeBlock("MEROITIC_CURSIVE", "MEROITIC CURSIVE", "MEROITICCURSIVE")

            ''' <summary>
            ''' Constant for the "Sora Sompeng" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly SORA_SOMPENG As New UnicodeBlock("SORA_SOMPENG", "SORA SOMPENG", "SORASOMPENG")

            ''' <summary>
            ''' Constant for the "Chakma" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly CHAKMA As New UnicodeBlock("CHAKMA")

            ''' <summary>
            ''' Constant for the "Sharada" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly SHARADA As New UnicodeBlock("SHARADA")

            ''' <summary>
            ''' Constant for the "Takri" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly TAKRI As New UnicodeBlock("TAKRI")

            ''' <summary>
            ''' Constant for the "Miao" Unicode character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly MIAO As New UnicodeBlock("MIAO")

            ''' <summary>
            ''' Constant for the "Arabic Mathematical Alphabetic Symbols" Unicode
            ''' character block.
            ''' @since 1.8
            ''' </summary>
            Public Shared ReadOnly ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS As New UnicodeBlock("ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS", "ARABIC MATHEMATICAL ALPHABETIC SYMBOLS", "ARABICMATHEMATICALALPHABETICSYMBOLS")

            Private Shared ReadOnly blockStarts As Integer() = {&H0, &H80, &H100, &H180, &H250, &H2B0, &H300, &H370, &H400, &H500, &H530, &H590, &H600, &H700, &H750, &H780, &H7C0, &H800, &H840, &H860, &H8A0, &H900, &H980, &HA00, &HA80, &HB00, &HB80, &HC00, &HC80, &HD00, &HD80, &HE00, &HE80, &HF00, &H1000, &H10A0, &H1100, &H1200, &H1380, &H13A0, &H1400, &H1680, &H16A0, &H1700, &H1720, &H1740, &H1760, &H1780, &H1800, &H18B0, &H1900, &H1950, &H1980, &H19E0, &H1A00, &H1A20, &H1AB0, &H1B00, &H1B80, &H1BC0, &H1C00, &H1C50, &H1C80, &H1CC0, &H1CD0, &H1D00, &H1D80, &H1DC0, &H1E00, &H1F00, &H2000, &H2070, &H20A0, &H20D0, &H2100, &H2150, &H2190, &H2200, &H2300, &H2400, &H2440, &H2460, &H2500, &H2580, &H25A0, &H2600, &H2700, &H27C0, &H27F0, &H2800, &H2900, &H2980, &H2A00, &H2B00, &H2C00, &H2C60, &H2C80, &H2D00, &H2D30, &H2D80, &H2DE0, &H2E00, &H2E80, &H2F00, &H2FE0, &H2FF0, &H3000, &H3040, &H30A0, &H3100, &H3130, &H3190, &H31A0, &H31C0, &H31F0, &H3200, &H3300, &H3400, &H4DC0, &H4E00, &HA000, &HA490, &HA4R0, &HA500, &HA640, &HA6A0, &HA700, &HA720, &HA800, &HA830, &HA840, &HA880, &HA8E0, &HA900, &HA930, &HA960, &HA980, &HA9E0, &HAA00, &HAA60, &HAA80, &HAAE0, &HAB00, &HAB30, &HABC0, &HAC00, &HD7B0, &HD800, &HDB80, &HDC00, &HE000, &HF900, &HFB00, &HFB50, &HFE00, &HFE10, &HFE20, &HFE30, &HFE50, &HFE70, &HFF00, &HFFF0, &H10000, &H10080, &H10100, &H10140, &H10190, &H101D0, &H10200, &H10280, &H102A0, &H102E0, &H10300, &H10330, &H10350, &H10380, &H103A0, &H103E0, &H10400, &H10450, &H10480, &H104B0, &H10800, &H10840, &H10860, &H10900, &H10920, &H10940, &H10980, &H109A0, &H10A00, &H10A60, &H10A80, &H10B00, &H10B40, &H10B60, &H10B80, &H10C00, &H10C50, &H10E60, &H10E80, &H11000, &H11080, &H110D0, &H11100, &H11150, &H11180, &H111E0, &H11680, &H116D0, &H12000, &H12400, &H12480, &H13000, &H13430, &H16800, &H16A40, &H16F00, &H16FA0, &H1B000, &H1B100, &H1D000, &H1D100, &H1D200, &H1D250, &H1D300, &H1D360, &H1D380, &H1D400, &H1D800, &H1EE00, &H1EF00, &H1F000, &H1F030, &H1F0A0, &H1F100, &H1F200, &H1F300, &H1F600, &H1F650, &H1F680, &H1F700, &H1F780, &H20000, &H2A6E0, &H2A700, &H2B740, &H2B820, &H2F800, &H2FA20, &HE0000, &HE0080, &HE0100, &HE01F0, &HF0000, &H100000 }

            Private Shared ReadOnly blocks As UnicodeBlock() = {BASIC_LATIN, LATIN_1_SUPPLEMENT, LATIN_EXTENDED_A, LATIN_EXTENDED_B, IPA_EXTENSIONS, SPACING_MODIFIER_LETTERS, COMBINING_DIACRITICAL_MARKS, GREEK, CYRILLIC, CYRILLIC_SUPPLEMENTARY, ARMENIAN, HEBREW, ARABIC, SYRIAC, ARABIC_SUPPLEMENT, THAANA, NKO, SAMARITAN, MANDAIC, Nothing, ARABIC_EXTENDED_A, DEVANAGARI, BENGALI, GURMUKHI, GUJARATI, ORIYA, TAMIL, TELUGU, KANNADA, MALAYALAM, SINHALA, THAI, LAO, TIBETAN, MYANMAR, GEORGIAN, HANGUL_JAMO, ETHIOPIC, ETHIOPIC_SUPPLEMENT, CHEROKEE, UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS, OGHAM, RUNIC, TAGALOG, HANUNOO, BUHID, TAGBANWA, KHMER, MONGOLIAN, UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS_EXTENDED, LIMBU, TAI_LE, NEW_TAI_LUE, KHMER_SYMBOLS, BUGINESE, TAI_THAM, Nothing, BALINESE, SUNDANESE, BATAK, LEPCHA, OL_CHIKI, Nothing, SUNDANESE_SUPPLEMENT, VEDIC_EXTENSIONS, PHONETIC_EXTENSIONS, PHONETIC_EXTENSIONS_SUPPLEMENT, COMBINING_DIACRITICAL_MARKS_SUPPLEMENT, LATIN_EXTENDED_ADDITIONAL, GREEK_EXTENDED, GENERAL_PUNCTUATION, SUPERSCRIPTS_AND_SUBSCRIPTS, CURRENCY_SYMBOLS, COMBINING_MARKS_FOR_SYMBOLS, LETTERLIKE_SYMBOLS, NUMBER_FORMS, ARROWS, MATHEMATICAL_OPERATORS, MISCELLANEOUS_TECHNICAL, CONTROL_PICTURES, OPTICAL_CHARACTER_RECOGNITION, ENCLOSED_ALPHANUMERICS, BOX_DRAWING, BLOCK_ELEMENTS, GEOMETRIC_SHAPES, MISCELLANEOUS_SYMBOLS, DINGBATS, MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A, SUPPLEMENTAL_ARROWS_A, BRAILLE_PATTERNS, SUPPLEMENTAL_ARROWS_B, MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B, SUPPLEMENTAL_MATHEMATICAL_OPERATORS, MISCELLANEOUS_SYMBOLS_AND_ARROWS, GLAGOLITIC, LATIN_EXTENDED_C, COPTIC, GEORGIAN_SUPPLEMENT, TIFINAGH, ETHIOPIC_EXTENDED, CYRILLIC_EXTENDED_A, SUPPLEMENTAL_PUNCTUATION, CJK_RADICALS_SUPPLEMENT, KANGXI_RADICALS, Nothing, IDEOGRAPHIC_DESCRIPTION_CHARACTERS, CJK_SYMBOLS_AND_PUNCTUATION, HIRAGANA, KATAKANA, BOPOMOFO, HANGUL_COMPATIBILITY_JAMO, KANBUN, BOPOMOFO_EXTENDED, CJK_STROKES, KATAKANA_PHONETIC_EXTENSIONS, ENCLOSED_CJK_LETTERS_AND_MONTHS, CJK_COMPATIBILITY, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A, YIJING_HEXAGRAM_SYMBOLS, CJK_UNIFIED_IDEOGRAPHS, YI_SYLLABLES, YI_RADICALS, LISU, VAI, CYRILLIC_EXTENDED_B, BAMUM, MODIFIER_TONE_LETTERS, LATIN_EXTENDED_D, SYLOTI_NAGRI, COMMON_INDIC_NUMBER_FORMS, PHAGS_PA, SAURASHTRA, DEVANAGARI_EXTENDED, KAYAH_LI, REJANG, HANGUL_JAMO_EXTENDED_A, JAVANESE, Nothing, CHAM, MYANMAR_EXTENDED_A, TAI_VIET, MEETEI_MAYEK_EXTENSIONS, ETHIOPIC_EXTENDED_A, Nothing, MEETEI_MAYEK, HANGUL_SYLLABLES, HANGUL_JAMO_EXTENDED_B, HIGH_SURROGATES, HIGH_PRIVATE_USE_SURROGATES, LOW_SURROGATES, PRIVATE_USE_AREA, CJK_COMPATIBILITY_IDEOGRAPHS, ALPHABETIC_PRESENTATION_FORMS, ARABIC_PRESENTATION_FORMS_A, VARIATION_SELECTORS, VERTICAL_FORMS, COMBINING_HALF_MARKS, CJK_COMPATIBILITY_FORMS, SMALL_FORM_VARIANTS, ARABIC_PRESENTATION_FORMS_B, HALFWIDTH_AND_FULLWIDTH_FORMS, SPECIALS, LINEAR_B_SYLLABARY, LINEAR_B_IDEOGRAMS, AEGEAN_NUMBERS, ANCIENT_GREEK_NUMBERS, ANCIENT_SYMBOLS, PHAISTOS_DISC, Nothing, LYCIAN, CARIAN, Nothing, OLD_ITALIC, GOTHIC, Nothing, UGARITIC, OLD_PERSIAN, Nothing, DESERET, SHAVIAN, OSMANYA, Nothing, CYPRIOT_SYLLABARY, IMPERIAL_ARAMAIC, Nothing, PHOENICIAN, LYDIAN, Nothing, MEROITIC_HIEROGLYPHS, MEROITIC_CURSIVE, KHAROSHTHI, OLD_SOUTH_ARABIAN, Nothing, AVESTAN, INSCRIPTIONAL_PARTHIAN, INSCRIPTIONAL_PAHLAVI, Nothing, OLD_TURKIC, Nothing, RUMI_NUMERAL_SYMBOLS, Nothing, BRAHMI, KAITHI, SORA_SOMPENG, CHAKMA, Nothing, SHARADA, Nothing, TAKRI, Nothing, CUNEIFORM, CUNEIFORM_NUMBERS_AND_PUNCTUATION, Nothing, EGYPTIAN_HIEROGLYPHS, Nothing, BAMUM_SUPPLEMENT, Nothing, MIAO, Nothing, KANA_SUPPLEMENT, Nothing, BYZANTINE_MUSICAL_SYMBOLS, MUSICAL_SYMBOLS, ANCIENT_GREEK_MUSICAL_NOTATION, Nothing, TAI_XUAN_JING_SYMBOLS, COUNTING_ROD_NUMERALS, Nothing, MATHEMATICAL_ALPHANUMERIC_SYMBOLS, Nothing, ARABIC_MATHEMATICAL_ALPHABETIC_SYMBOLS, Nothing, MAHJONG_TILES, DOMINO_TILES, PLAYING_CARDS, ENCLOSED_ALPHANUMERIC_SUPPLEMENT, ENCLOSED_IDEOGRAPHIC_SUPPLEMENT, MISCELLANEOUS_SYMBOLS_AND_PICTOGRAPHS, EMOTICONS, Nothing, TRANSPORT_AND_MAP_SYMBOLS, ALCHEMICAL_SYMBOLS, Nothing, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B, Nothing, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_C, CJK_UNIFIED_IDEOGRAPHS_EXTENSION_D, Nothing, CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT, Nothing, TAGS, Nothing, VARIATION_SELECTORS_SUPPLEMENT, Nothing, SUPPLEMENTARY_PRIVATE_USE_AREA_A, SUPPLEMENTARY_PRIVATE_USE_AREA_B}


            ''' <summary>
            ''' Returns the object representing the Unicode block containing the
            ''' given character, or {@code null} if the character is not a
            ''' member of a defined block.
            ''' 
            ''' <p><b>Note:</b> This method cannot handle
            ''' <a href="Character.html#supplementary"> supplementary
            ''' characters</a>.  To support all Unicode characters, including
            ''' supplementary characters, use the <seealso cref="#of(int)"/> method.
            ''' </summary>
            ''' <param name="c">  The character in question </param>
            ''' <returns>  The {@code UnicodeBlock} instance representing the
            '''          Unicode block of which this character is a member, or
            '''          {@code null} if the character is not a member of any
            '''          Unicode block </returns>
            Public Shared Function [of](ByVal c As Char) As UnicodeBlock
                Return [of](AscW(c))
            End Function

            ''' <summary>
            ''' Returns the object representing the Unicode block
            ''' containing the given character (Unicode code point), or
            ''' {@code null} if the character is not a member of a
            ''' defined block.
            ''' </summary>
            ''' <param name="codePoint"> the character (Unicode code point) in question. </param>
            ''' <returns>  The {@code UnicodeBlock} instance representing the
            '''          Unicode block of which this character is a member, or
            '''          {@code null} if the character is not a member of any
            '''          Unicode block </returns>
            ''' <exception cref="IllegalArgumentException"> if the specified
            ''' {@code codePoint} is an invalid Unicode code point. </exception>
            ''' <seealso cref= Character#isValidCodePoint(int)
            ''' @since   1.5 </seealso>
            Public Shared Function [of](ByVal codePoint As Integer) As UnicodeBlock
                If Not isValidCodePoint(codePoint) Then Throw New IllegalArgumentException

                Dim top, bottom, current As Integer
                bottom = 0
                top = blockStarts.Length
                current = top \ 2

                ' invariant: top > current >= bottom && codePoint >= unicodeBlockStarts[bottom]
                Do While top - bottom > 1
                    If codePoint >= blockStarts(current) Then
                        bottom = current
                    Else
                        top = current
                    End If
                    current = (top + bottom) \ 2
                Loop
                Return blocks(current)
            End Function

            ''' <summary>
            ''' Returns the UnicodeBlock with the given name. Block
            ''' names are determined by The Unicode Standard. The file
            ''' Blocks-&lt;version&gt;.txt defines blocks for a particular
            ''' version of the standard. The <seealso cref="Character"/> class specifies
            ''' the version of the standard that it supports.
            ''' <p>
            ''' This method accepts block names in the following forms:
            ''' <ol>
            ''' <li> Canonical block names as defined by the Unicode Standard.
            ''' For example, the standard defines a "Basic Latin" block. Therefore, this
            ''' method accepts "Basic Latin" as a valid block name. The documentation of
            ''' each UnicodeBlock provides the canonical name.
            ''' <li>Canonical block names with all spaces removed. For example, "BasicLatin"
            ''' is a valid block name for the "Basic Latin" block.
            ''' <li>The text representation of each constant UnicodeBlock identifier.
            ''' For example, this method will return the <seealso cref="#BASIC_LATIN"/> block if
            ''' provided with the "BASIC_LATIN" name. This form replaces all spaces and
            ''' hyphens in the canonical name with underscores.
            ''' </ol>
            ''' Finally, character case is ignored for all of the valid block name forms.
            ''' For example, "BASIC_LATIN" and "basic_latin" are both valid block names.
            ''' The en_US locale's case mapping rules are used to provide case-insensitive
            ''' string comparisons for block name validation.
            ''' <p>
            ''' If the Unicode Standard changes block names, both the previous and
            ''' current names will be accepted.
            ''' </summary>
            ''' <param name="blockName"> A {@code UnicodeBlock} name. </param>
            ''' <returns> The {@code UnicodeBlock} instance identified
            '''         by {@code blockName} </returns>
            ''' <exception cref="IllegalArgumentException"> if {@code blockName} is an
            '''         invalid name </exception>
            ''' <exception cref="NullPointerException"> if {@code blockName} is null
            ''' @since 1.5 </exception>
            Public Shared Function forName(ByVal blockName As String) As UnicodeBlock
                Dim block As UnicodeBlock = map(blockName.ToUpper(java.util.Locale.US))
                If block Is Nothing Then Throw New IllegalArgumentException
                Return block
            End Function
        End Class


        ''' <summary>
        ''' A family of character subsets representing the character scripts
        ''' defined in the <a href="http://www.unicode.org/reports/tr24/">
        ''' <i>Unicode Standard Annex #24: Script Names</i></a>. Every Unicode
        ''' character is assigned to a single Unicode script, either a specific
        ''' script, such as <seealso cref="Character.UnicodeScript#LATIN Latin"/>, or
        ''' one of the following three special values,
        ''' <seealso cref="Character.UnicodeScript#INHERITED Inherited"/>,
        ''' <seealso cref="Character.UnicodeScript#COMMON Common"/> or
        ''' <seealso cref="Character.UnicodeScript#UNKNOWN Unknown"/>.
        ''' 
        ''' @since 1.7
        ''' </summary>
        Public Enum UnicodeScript
            ''' <summary>
            ''' Unicode script "Common".
            ''' </summary>
            COMMON

            ''' <summary>
            ''' Unicode script "Latin".
            ''' </summary>
            LATIN

            ''' <summary>
            ''' Unicode script "Greek".
            ''' </summary>
            GREEK

            ''' <summary>
            ''' Unicode script "Cyrillic".
            ''' </summary>
            CYRILLIC

            ''' <summary>
            ''' Unicode script "Armenian".
            ''' </summary>
            ARMENIAN

            ''' <summary>
            ''' Unicode script "Hebrew".
            ''' </summary>
            HEBREW

            ''' <summary>
            ''' Unicode script "Arabic".
            ''' </summary>
            ARABIC

            ''' <summary>
            ''' Unicode script "Syriac".
            ''' </summary>
            SYRIAC

            ''' <summary>
            ''' Unicode script "Thaana".
            ''' </summary>
            THAANA

            ''' <summary>
            ''' Unicode script "Devanagari".
            ''' </summary>
            DEVANAGARI

            ''' <summary>
            ''' Unicode script "Bengali".
            ''' </summary>
            BENGALI

            ''' <summary>
            ''' Unicode script "Gurmukhi".
            ''' </summary>
            GURMUKHI

            ''' <summary>
            ''' Unicode script "Gujarati".
            ''' </summary>
            GUJARATI

            ''' <summary>
            ''' Unicode script "Oriya".
            ''' </summary>
            ORIYA

            ''' <summary>
            ''' Unicode script "Tamil".
            ''' </summary>
            TAMIL

            ''' <summary>
            ''' Unicode script "Telugu".
            ''' </summary>
            TELUGU

            ''' <summary>
            ''' Unicode script "Kannada".
            ''' </summary>
            KANNADA

            ''' <summary>
            ''' Unicode script "Malayalam".
            ''' </summary>
            MALAYALAM

            ''' <summary>
            ''' Unicode script "Sinhala".
            ''' </summary>
            SINHALA

            ''' <summary>
            ''' Unicode script "Thai".
            ''' </summary>
            THAI

            ''' <summary>
            ''' Unicode script "Lao".
            ''' </summary>
            LAO

            ''' <summary>
            ''' Unicode script "Tibetan".
            ''' </summary>
            TIBETAN

            ''' <summary>
            ''' Unicode script "Myanmar".
            ''' </summary>
            MYANMAR

            ''' <summary>
            ''' Unicode script "Georgian".
            ''' </summary>
            GEORGIAN

            ''' <summary>
            ''' Unicode script "Hangul".
            ''' </summary>
            HANGUL

            ''' <summary>
            ''' Unicode script "Ethiopic".
            ''' </summary>
            ETHIOPIC

            ''' <summary>
            ''' Unicode script "Cherokee".
            ''' </summary>
            CHEROKEE

            ''' <summary>
            ''' Unicode script "Canadian_Aboriginal".
            ''' </summary>
            CANADIAN_ABORIGINAL

            ''' <summary>
            ''' Unicode script "Ogham".
            ''' </summary>
            OGHAM

            ''' <summary>
            ''' Unicode script "Runic".
            ''' </summary>
            RUNIC

            ''' <summary>
            ''' Unicode script "Khmer".
            ''' </summary>
            KHMER

            ''' <summary>
            ''' Unicode script "Mongolian".
            ''' </summary>
            MONGOLIAN

            ''' <summary>
            ''' Unicode script "Hiragana".
            ''' </summary>
            HIRAGANA

            ''' <summary>
            ''' Unicode script "Katakana".
            ''' </summary>
            KATAKANA

            ''' <summary>
            ''' Unicode script "Bopomofo".
            ''' </summary>
            BOPOMOFO

            ''' <summary>
            ''' Unicode script "Han".
            ''' </summary>
            HAN

            ''' <summary>
            ''' Unicode script "Yi".
            ''' </summary>
            YI

            ''' <summary>
            ''' Unicode script "Old_Italic".
            ''' </summary>
            OLD_ITALIC

            ''' <summary>
            ''' Unicode script "Gothic".
            ''' </summary>
            GOTHIC

            ''' <summary>
            ''' Unicode script "Deseret".
            ''' </summary>
            DESERET

            ''' <summary>
            ''' Unicode script "Inherited".
            ''' </summary>
            INHERITED

            ''' <summary>
            ''' Unicode script "Tagalog".
            ''' </summary>
            TAGALOG

            ''' <summary>
            ''' Unicode script "Hanunoo".
            ''' </summary>
            HANUNOO

            ''' <summary>
            ''' Unicode script "Buhid".
            ''' </summary>
            BUHID

            ''' <summary>
            ''' Unicode script "Tagbanwa".
            ''' </summary>
            TAGBANWA

            ''' <summary>
            ''' Unicode script "Limbu".
            ''' </summary>
            LIMBU

            ''' <summary>
            ''' Unicode script "Tai_Le".
            ''' </summary>
            TAI_LE

            ''' <summary>
            ''' Unicode script "Linear_B".
            ''' </summary>
            LINEAR_B

            ''' <summary>
            ''' Unicode script "Ugaritic".
            ''' </summary>
            UGARITIC

            ''' <summary>
            ''' Unicode script "Shavian".
            ''' </summary>
            SHAVIAN

            ''' <summary>
            ''' Unicode script "Osmanya".
            ''' </summary>
            OSMANYA

            ''' <summary>
            ''' Unicode script "Cypriot".
            ''' </summary>
            CYPRIOT

            ''' <summary>
            ''' Unicode script "Braille".
            ''' </summary>
            BRAILLE

            ''' <summary>
            ''' Unicode script "Buginese".
            ''' </summary>
            BUGINESE

            ''' <summary>
            ''' Unicode script "Coptic".
            ''' </summary>
            COPTIC

            ''' <summary>
            ''' Unicode script "New_Tai_Lue".
            ''' </summary>
            NEW_TAI_LUE

            ''' <summary>
            ''' Unicode script "Glagolitic".
            ''' </summary>
            GLAGOLITIC

            ''' <summary>
            ''' Unicode script "Tifinagh".
            ''' </summary>
            TIFINAGH

            ''' <summary>
            ''' Unicode script "Syloti_Nagri".
            ''' </summary>
            SYLOTI_NAGRI

            ''' <summary>
            ''' Unicode script "Old_Persian".
            ''' </summary>
            OLD_PERSIAN

            ''' <summary>
            ''' Unicode script "Kharoshthi".
            ''' </summary>
            KHAROSHTHI

            ''' <summary>
            ''' Unicode script "Balinese".
            ''' </summary>
            BALINESE

            ''' <summary>
            ''' Unicode script "Cuneiform".
            ''' </summary>
            CUNEIFORM

            ''' <summary>
            ''' Unicode script "Phoenician".
            ''' </summary>
            PHOENICIAN

            ''' <summary>
            ''' Unicode script "Phags_Pa".
            ''' </summary>
            PHAGS_PA

            ''' <summary>
            ''' Unicode script "Nko".
            ''' </summary>
            NKO

            ''' <summary>
            ''' Unicode script "Sundanese".
            ''' </summary>
            SUNDANESE

            ''' <summary>
            ''' Unicode script "Batak".
            ''' </summary>
            BATAK

            ''' <summary>
            ''' Unicode script "Lepcha".
            ''' </summary>
            LEPCHA

            ''' <summary>
            ''' Unicode script "Ol_Chiki".
            ''' </summary>
            OL_CHIKI

            ''' <summary>
            ''' Unicode script "Vai".
            ''' </summary>
            VAI

            ''' <summary>
            ''' Unicode script "Saurashtra".
            ''' </summary>
            SAURASHTRA

            ''' <summary>
            ''' Unicode script "Kayah_Li".
            ''' </summary>
            KAYAH_LI

            ''' <summary>
            ''' Unicode script "Rejang".
            ''' </summary>
            REJANG

            ''' <summary>
            ''' Unicode script "Lycian".
            ''' </summary>
            LYCIAN

            ''' <summary>
            ''' Unicode script "Carian".
            ''' </summary>
            CARIAN

            ''' <summary>
            ''' Unicode script "Lydian".
            ''' </summary>
            LYDIAN

            ''' <summary>
            ''' Unicode script "Cham".
            ''' </summary>
            CHAM

            ''' <summary>
            ''' Unicode script "Tai_Tham".
            ''' </summary>
            TAI_THAM

            ''' <summary>
            ''' Unicode script "Tai_Viet".
            ''' </summary>
            TAI_VIET

            ''' <summary>
            ''' Unicode script "Avestan".
            ''' </summary>
            AVESTAN

            ''' <summary>
            ''' Unicode script "Egyptian_Hieroglyphs".
            ''' </summary>
            EGYPTIAN_HIEROGLYPHS

            ''' <summary>
            ''' Unicode script "Samaritan".
            ''' </summary>
            SAMARITAN

            ''' <summary>
            ''' Unicode script "Mandaic".
            ''' </summary>
            MANDAIC

            ''' <summary>
            ''' Unicode script "Lisu".
            ''' </summary>
            LISU

            ''' <summary>
            ''' Unicode script "Bamum".
            ''' </summary>
            BAMUM

            ''' <summary>
            ''' Unicode script "Javanese".
            ''' </summary>
            JAVANESE

            ''' <summary>
            ''' Unicode script "Meetei_Mayek".
            ''' </summary>
            MEETEI_MAYEK

            ''' <summary>
            ''' Unicode script "Imperial_Aramaic".
            ''' </summary>
            IMPERIAL_ARAMAIC

            ''' <summary>
            ''' Unicode script "Old_South_Arabian".
            ''' </summary>
            OLD_SOUTH_ARABIAN

            ''' <summary>
            ''' Unicode script "Inscriptional_Parthian".
            ''' </summary>
            INSCRIPTIONAL_PARTHIAN

            ''' <summary>
            ''' Unicode script "Inscriptional_Pahlavi".
            ''' </summary>
            INSCRIPTIONAL_PAHLAVI

            ''' <summary>
            ''' Unicode script "Old_Turkic".
            ''' </summary>
            OLD_TURKIC

            ''' <summary>
            ''' Unicode script "Brahmi".
            ''' </summary>
            BRAHMI

            ''' <summary>
            ''' Unicode script "Kaithi".
            ''' </summary>
            KAITHI

            ''' <summary>
            ''' Unicode script "Meroitic Hieroglyphs".
            ''' </summary>
            MEROITIC_HIEROGLYPHS

            ''' <summary>
            ''' Unicode script "Meroitic Cursive".
            ''' </summary>
            MEROITIC_CURSIVE

            ''' <summary>
            ''' Unicode script "Sora Sompeng".
            ''' </summary>
            SORA_SOMPENG

            ''' <summary>
            ''' Unicode script "Chakma".
            ''' </summary>
            CHAKMA

            ''' <summary>
            ''' Unicode script "Sharada".
            ''' </summary>
            SHARADA

            ''' <summary>
            ''' Unicode script "Takri".
            ''' </summary>
            TAKRI

            ''' <summary>
            ''' Unicode script "Miao".
            ''' </summary>
            MIAO

            ''' <summary>
            ''' Unicode script "Unknown".
            ''' </summary>
            UNKNOWN

            'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
            '			private static final int[] scriptStarts = { &H0, &H41, &H5B, &H61, &H7B, &HAA, &HAB, &HBA, &HBB, &HC0, &HD7, &HD8, &HF7, &HF8, &H2B9, &H2E0, &H2E5, &H2EA, &H2EC, &H300, &H370, &H374, &H375, &H37E, &H384, &H385, &H386, &H387, &H388, &H3E2, &H3F0, &H400, &H485, &H487, &H531, &H589, &H58A, &H591, &H600, &H60C, &H60D, &H61B, &H61E, &H61F, &H620, &H640, &H641, &H64B, &H656, &H660, &H66A, &H670, &H671, &H6DD, &H6DE, &H700, &H750, &H780, &H7C0, &H800, &H840, &H8A0, &H900, &H951, &H953, &H964, &H966, &H981, &HA01, &HA81, &HB01, &HB82, &HC01, &HC82, &HD02, &HD82, &HE01, &HE3F, &HE40, &HE81, &HF00, &HFD5, &HFD9, &H1000, &H10A0, &H10FB, &H10FC, &H1100, &H1200, &H13A0, &H1400, &H1680, &H16A0, &H16EB, &H16EE, &H1700, &H1720, &H1735, &H1740, &H1760, &H1780, &H1800, &H1802, &H1804, &H1805, &H1806, &H18B0, &H1900, &H1950, &H1980, &H19E0, &H1A00, &H1A20, &H1B00, &H1B80, &H1BC0, &H1C00, &H1C50, &H1CC0, &H1CD0, &H1CD3, &H1CD4, &H1CE1, &H1CE2, &H1CE9, &H1CED, &H1CEE, &H1CF4, &H1CF5, &H1D00, &H1D26, &H1D2B, &H1D2C, &H1D5D, &H1D62, &H1D66, &H1D6B, &H1D78, &H1D79, &H1DBF, &H1DC0, &H1E00, &H1F00, &H2000, &H200C, &H200E, &H2071, &H2074, &H207F, &H2080, &H2090, &H20A0, &H20D0, &H2100, &H2126, &H2127, &H212A, &H212C, &H2132, &H2133, &H214E, &H214F, &H2160, &H2189, &H2800, &H2900, &H2C00, &H2C60, &H2C80, &H2D00, &H2D30, &H2D80, &H2DE0, &H2E00, &H2E80, &H2FF0, &H3005, &H3006, &H3007, &H3008, &H3021, &H302A, &H302E, &H3030, &H3038, &H303C, &H3041, &H3099, &H309B, &H309D, &H30A0, &H30A1, &H30FB, &H30FD, &H3105, &H3131, &H3190, &H31A0, &H31C0, &H31F0, &H3200, &H3220, &H3260, &H327F, &H32D0, &H3358, &H3400, &H4DC0, &H4E00, &HA000, &HA4D0, &HA500, &HA640, &HA6A0, &HA700, &HA722, &HA788, &HA78B, &HA800, &HA830, &HA840, &HA880, &HA8E0, &HA900, &HA930, &HA960, &HA980, &HAA00, &HAA60, &HAA80, &HAAE0, &HAB01, &HABC0, &HAC00, &HD7FC, &HF900, &HFB00, &HFB13, &HFB1D, &HFB50, &HFD3E, &HFD50, &HFDFD, &HFE00, &HFE10, &HFE20, &HFE30, &HFE70, &HFEFF, &HFF21, &HFF3B, &HFF41, &HFF5B, &HFF66, &HFF70, &HFF71, &HFF9E, &HFFA0, &HFFE0, &H10000, &H10100, &H10140, &H10190, &H101FD, &H10280, &H102A0, &H10300, &H10330, &H10380, &H103A0, &H10400, &H10450, &H10480, &H10800, &H10840, &H10900, &H10920, &H10980, &H109A0, &H10A00, &H10A60, &H10B00, &H10B40, &H10B60, &H10C00, &H10E60, &H11000, &H11080, &H110D0, &H11100, &H11180, &H11680, &H12000, &H13000, &H16800, &H16F00, &H1B000, &H1B001, &H1D000, &H1D167, &H1D16A, &H1D17B, &H1D183, &H1D185, &H1D18C, &H1D1AA, &H1D1AE, &H1D200, &H1D300, &H1EE00, &H1F000, &H1F200, &H1F201, &H20000, &HE0001, &HE0100, &HE01F0
        End Enum

        Private Shared ReadOnly scripts As UnicodeScript() = {Common, LATIN, Common, LATIN, Common, LATIN, Common, LATIN, Common, LATIN, Common, LATIN, Common, LATIN, Common, LATIN, Common, BOPOMOFO, Common, INHERITED, GREEK, Common, GREEK, Common, GREEK, Common, GREEK, Common, GREEK, COPTIC, GREEK, CYRILLIC, INHERITED, CYRILLIC, ARMENIAN, Common, ARMENIAN, HEBREW, ARABIC, Common, ARABIC, Common, ARABIC, Common, ARABIC, Common, ARABIC, INHERITED, ARABIC, Common, ARABIC, INHERITED, ARABIC, Common, ARABIC, SYRIAC, ARABIC, THAANA, NKO, SAMARITAN, MANDAIC, ARABIC, DEVANAGARI, INHERITED, DEVANAGARI, Common, DEVANAGARI, BENGALI, GURMUKHI, GUJARATI, ORIYA, TAMIL, TELUGU, KANNADA, MALAYALAM, SINHALA, THAI, Common, THAI, LAO, TIBETAN, Common, TIBETAN, MYANMAR, GEORGIAN, Common, GEORGIAN, HANGUL, ETHIOPIC, CHEROKEE, CANADIAN_ABORIGINAL, OGHAM, RUNIC, Common, RUNIC, TAGALOG, HANUNOO, Common, BUHID, TAGBANWA, KHMER, MONGOLIAN, Common, MONGOLIAN, Common, MONGOLIAN, CANADIAN_ABORIGINAL, LIMBU, TAI_LE, NEW_TAI_LUE, KHMER, BUGINESE, TAI_THAM, BALINESE, SUNDANESE, BATAK, LEPCHA, OL_CHIKI, SUNDANESE, INHERITED, Common, INHERITED, Common, INHERITED, Common, INHERITED, Common, INHERITED, Common, LATIN, GREEK, CYRILLIC, LATIN, GREEK, LATIN, GREEK, LATIN, CYRILLIC, LATIN, GREEK, INHERITED, LATIN, GREEK, Common, INHERITED, Common, LATIN, Common, LATIN, Common, LATIN, Common, INHERITED, Common, GREEK, Common, LATIN, Common, LATIN, Common, LATIN, Common, LATIN, Common, BRAILLE, Common, GLAGOLITIC, LATIN, COPTIC, GEORGIAN, TIFINAGH, ETHIOPIC, CYRILLIC, Common, HAN, Common, HAN, Common, HAN, Common, HAN, INHERITED, HANGUL, Common, HAN, Common, HIRAGANA, INHERITED, Common, HIRAGANA, Common, KATAKANA, Common, KATAKANA, BOPOMOFO, HANGUL, Common, BOPOMOFO, Common, KATAKANA, HANGUL, Common, HANGUL, Common, KATAKANA, Common, HAN, Common, HAN, YI, LISU, VAI, CYRILLIC, BAMUM, Common, LATIN, Common, LATIN, SYLOTI_NAGRI, Common, PHAGS_PA, SAURASHTRA, DEVANAGARI, KAYAH_LI, REJANG, HANGUL, JAVANESE, CHAM, MYANMAR, TAI_VIET, MEETEI_MAYEK, ETHIOPIC, MEETEI_MAYEK, HANGUL, UNKNOWN, HAN, LATIN, ARMENIAN, HEBREW, ARABIC, Common, ARABIC, Common, INHERITED, Common, INHERITED, Common, ARABIC, Common, LATIN, Common, LATIN, Common, KATAKANA, Common, KATAKANA, Common, HANGUL, Common, LINEAR_B, Common, GREEK, Common, INHERITED, LYCIAN, CARIAN, OLD_ITALIC, GOTHIC, UGARITIC, OLD_PERSIAN, DESERET, SHAVIAN, OSMANYA, CYPRIOT, IMPERIAL_ARAMAIC, PHOENICIAN, LYDIAN, MEROITIC_HIEROGLYPHS, MEROITIC_CURSIVE, KHAROSHTHI, OLD_SOUTH_ARABIAN, AVESTAN, INSCRIPTIONAL_PARTHIAN, INSCRIPTIONAL_PAHLAVI, OLD_TURKIC, ARABIC, BRAHMI, KAITHI, SORA_SOMPENG, CHAKMA, SHARADA, TAKRI, CUNEIFORM, EGYPTIAN_HIEROGLYPHS, BAMUM, MIAO, KATAKANA, HIRAGANA, Common, INHERITED, Common, INHERITED, Common, INHERITED, Common, INHERITED, Common, GREEK, Common, ARABIC, Common, HIRAGANA, Common, HAN, Common, INHERITED, UNKNOWN}

        Private Shared aliases As Dictionary(Of String, Character.UnicodeScript)
        Shared Sub New()
            aliases = New Dictionary(Of )(128)
            aliases("ARAB") = ARABIC
            aliases("ARMI") = IMPERIAL_ARAMAIC
            aliases("ARMN") = ARMENIAN
            aliases("AVST") = AVESTAN
            aliases("BALI") = BALINESE
            aliases("BAMU") = BAMUM
            aliases("BATK") = BATAK
            aliases("BENG") = BENGALI
            aliases("BOPO") = BOPOMOFO
            aliases("BRAI") = BRAILLE
            aliases("BRAH") = BRAHMI
            aliases("BUGI") = BUGINESE
            aliases("BUHD") = BUHID
            aliases("CAKM") = CHAKMA
            aliases("CANS") = CANADIAN_ABORIGINAL
            aliases("CARI") = CARIAN
            aliases("CHAM") = CHAM
            aliases("CHER") = CHEROKEE
            aliases("COPT") = COPTIC
            aliases("CPRT") = CYPRIOT
            aliases("CYRL") = CYRILLIC
            aliases("DEVA") = DEVANAGARI
            aliases("DSRT") = DESERET
            aliases("EGYP") = EGYPTIAN_HIEROGLYPHS
            aliases("ETHI") = ETHIOPIC
            aliases("GEOR") = GEORGIAN
            aliases("GLAG") = GLAGOLITIC
            aliases("GOTH") = GOTHIC
            aliases("GREK") = GREEK
            aliases("GUJR") = GUJARATI
            aliases("GURU") = GURMUKHI
            aliases("HANG") = HANGUL
            aliases("HANI") = HAN
            aliases("HANO") = HANUNOO
            aliases("HEBR") = HEBREW
            aliases("HIRA") = HIRAGANA
            ' it appears we don't have the KATAKANA_OR_HIRAGANA
            'aliases.put("HRKT", KATAKANA_OR_HIRAGANA);
            aliases("ITAL") = OLD_ITALIC
            aliases("JAVA") = JAVANESE
            aliases("KALI") = KAYAH_LI
            aliases("KANA") = KATAKANA
            aliases("KHAR") = KHAROSHTHI
            aliases("KHMR") = KHMER
            aliases("KNDA") = KANNADA
            aliases("KTHI") = KAITHI
            aliases("LANA") = TAI_THAM
            aliases("LAOO") = LAO
            aliases("LATN") = LATIN
            aliases("LEPC") = LEPCHA
            aliases("LIMB") = LIMBU
            aliases("LINB") = LINEAR_B
            aliases("LISU") = LISU
            aliases("LYCI") = LYCIAN
            aliases("LYDI") = LYDIAN
            aliases("MAND") = MANDAIC
            aliases("MERC") = MEROITIC_CURSIVE
            aliases("MERO") = MEROITIC_HIEROGLYPHS
            aliases("MLYM") = MALAYALAM
            aliases("MONG") = MONGOLIAN
            aliases("MTEI") = MEETEI_MAYEK
            aliases("MYMR") = MYANMAR
            aliases("NKOO") = NKO
            aliases("OGAM") = OGHAM
            aliases("OLCK") = OL_CHIKI
            aliases("ORKH") = OLD_TURKIC
            aliases("ORYA") = ORIYA
            aliases("OSMA") = OSMANYA
            aliases("PHAG") = PHAGS_PA
            aliases("PLRD") = MIAO
            aliases("PHLI") = INSCRIPTIONAL_PAHLAVI
            aliases("PHNX") = PHOENICIAN
            aliases("PRTI") = INSCRIPTIONAL_PARTHIAN
            aliases("RJNG") = REJANG
            aliases("RUNR") = RUNIC
            aliases("SAMR") = SAMARITAN
            aliases("SARB") = OLD_SOUTH_ARABIAN
            aliases("SAUR") = SAURASHTRA
            aliases("SHAW") = SHAVIAN
            aliases("SHRD") = SHARADA
            aliases("SINH") = SINHALA
            aliases("SORA") = SORA_SOMPENG
            aliases("SUND") = SUNDANESE
            aliases("SYLO") = SYLOTI_NAGRI
            aliases("SYRC") = SYRIAC
            aliases("TAGB") = TAGBANWA
            aliases("TALE") = TAI_LE
            aliases("TAKR") = TAKRI
            aliases("TALU") = NEW_TAI_LUE
            aliases("TAML") = TAMIL
            aliases("TAVT") = TAI_VIET
            aliases("TELU") = TELUGU
            aliases("TFNG") = TIFINAGH
            aliases("TGLG") = TAGALOG
            aliases("THAA") = THAANA
            aliases("THAI") = THAI
            aliases("TIBT") = TIBETAN
            aliases("UGAR") = UGARITIC
            aliases("VAII") = VAI
            aliases("XPEO") = OLD_PERSIAN
            aliases("XSUX") = CUNEIFORM
            aliases("YIII") = YI
            aliases("ZINH") = INHERITED
            aliases("ZYYY") = Common
            aliases("ZZZZ") = UNKNOWN
        End Sub

        ''' <summary>
        ''' Returns the enum constant representing the Unicode script of which
        ''' the given character (Unicode code point) is assigned to.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) in question. </param>
        ''' <returns>  The {@code UnicodeScript} constant representing the
        '''          Unicode script of which this character is assigned to.
        ''' </returns>
        ''' <exception cref="IllegalArgumentException"> if the specified
        ''' {@code codePoint} is an invalid Unicode code point. </exception>
        ''' <seealso cref= Character#isValidCodePoint(int)
        '''  </seealso>
        Public Shared Function [of](ByVal codePoint As Integer) As UnicodeScript
            If Not isValidCodePoint(codePoint) Then Throw New IllegalArgumentException
            Dim type_Renamed As Integer = GetType(codePoint)
            ' leave SURROGATE and PRIVATE_USE for table lookup
            If type_Renamed = UNASSIGNED Then Return UNKNOWN
            Dim index As Integer = java.util.Arrays.binarySearch(scriptStarts, codePoint)
            If index < 0 Then index = -index - 2
            Return scripts(index)
        End Function

        ''' <summary>
        ''' Returns the UnicodeScript constant with the given Unicode script
        ''' name or the script name alias. Script names and their aliases are
        ''' determined by The Unicode Standard. The files Scripts&lt;version&gt;.txt
        ''' and PropertyValueAliases&lt;version&gt;.txt define script names
        ''' and the script name aliases for a particular version of the
        ''' standard. The <seealso cref="Character"/> class specifies the version of
        ''' the standard that it supports.
        ''' <p>
        ''' Character case is ignored for all of the valid script names.
        ''' The en_US locale's case mapping rules are used to provide
        ''' case-insensitive string comparisons for script name validation.
        ''' <p>
        ''' </summary>
        ''' <param name="scriptName"> A {@code UnicodeScript} name. </param>
        ''' <returns> The {@code UnicodeScript} constant identified
        '''         by {@code scriptName} </returns>
        ''' <exception cref="IllegalArgumentException"> if {@code scriptName} is an
        '''         invalid name </exception>
        ''' <exception cref="NullPointerException"> if {@code scriptName} is null </exception>
        Public Shared Function forName(ByVal scriptName As String) As UnicodeScript
            scriptName = scriptName.ToUpper(java.util.Locale.ENGLISH)
            '.replace(' ', '_'));
            Dim sc As UnicodeScript = aliases(scriptName)
            If sc IsNot Nothing Then Return sc
            Return valueOf(scriptName)
        End Function
    End Class

    ''' <summary>
    ''' The value of the {@code Character}.
    ''' 
    ''' @serial
    ''' </summary>
    Private ReadOnly value As Char

    ''' <summary>
    ''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
    Private Const serialVersionUID As Long = 3786198910865385080L

    ''' <summary>
    ''' Constructs a newly allocated {@code Character} object that
    ''' represents the specified {@code char} value.
    ''' </summary>
    ''' <param name="value">   the value to be represented by the
    '''                  {@code Character} object. </param>
    'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
    Sub New(value As Char)
        Me.value = value
    End Sub

    Private Static Class CharacterCache
        Private CharacterCache()

        Static final Character cache() = New Character(127){}

			Static ImpliedClass()
        For i As Integer = 0 To cache.length - 1
					cache(i) = New Character(ChrW(i))
				Next i

        ''' <summary>
        ''' Returns a <tt>Character</tt> instance representing the specified
        ''' <tt>char</tt> value.
        ''' If a new <tt>Character</tt> instance is not required, this method
        ''' should generally be used in preference to the constructor
        ''' <seealso cref="#Character(char)"/>, as this method is likely to yield
        ''' significantly better space and time performance by caching
        ''' frequently requested values.
        ''' 
        ''' This method will always cache values in the range {@code
        ''' '\u005Cu0000'} to {@code '\u005Cu007F'}, inclusive, and may
        ''' cache other values outside of this range.
        ''' </summary>
        ''' <param name="c"> a char value. </param>
        ''' <returns> a <tt>Character</tt> instance representing <tt>c</tt>.
        ''' @since  1.5 </returns>
        Public Static Character valueOf(Char c)
			If c <= 127 Then ' must cache Return CharacterCache.cache(CInt(Fix(c)))
        Return New Character(c)

        ''' <summary>
        ''' Returns the value of this {@code Character} object. </summary>
        ''' <returns>  the primitive {@code char} value represented by
        '''          this object. </returns>
        Public Char charValue()
			Return value

        ''' <summary>
        ''' Returns a hash code for this {@code Character}; equal to the result
        ''' of invoking {@code charValue()}.
        ''' </summary>
        ''' <returns> a hash code value for this {@code Character} </returns>
        Public Integer GetHashCode()
			Return Character.hashCode(value)

        ''' <summary>
        ''' Returns a hash code for a {@code char} value; compatible with
        ''' {@code Character.hashCode()}.
        ''' 
        ''' @since 1.8
        ''' </summary>
        ''' <param name="value"> The {@code char} for which to return a hash code. </param>
        ''' <returns> a hash code value for a {@code char} value. </returns>
        Public Static Integer hashCode(Char value)
			Return AscW(value)

        ''' <summary>
        ''' Compares this object against the specified object.
        ''' The result is {@code true} if and only if the argument is not
        ''' {@code null} and is a {@code Character} object that
        ''' represents the same {@code char} value as this object.
        ''' </summary>
        ''' <param name="obj">   the object to compare with. </param>
        ''' <returns>  {@code true} if the objects are the same;
        '''          {@code false} otherwise. </returns>
        Public Boolean Equals(Object obj)
			If TypeOf obj Is Character Then Return value = CChar(obj)
			Return False

        ''' <summary>
        ''' Returns a {@code String} object representing this
        ''' {@code Character}'s value.  The result is a string of
        ''' length 1 whose sole component is the primitive
        ''' {@code char} value represented by this
        ''' {@code Character} object.
        ''' </summary>
        ''' <returns>  a string representation of this object. </returns>
        Public String ToString()
			Dim buf As Char() = {value()}
        Return Convert.ToString(buf)

        ''' <summary>
        ''' Returns a {@code String} object representing the
        ''' specified {@code char}.  The result is a string of length
        ''' 1 consisting solely of the specified {@code char}.
        ''' </summary>
        ''' <param name="c"> the {@code char} to be converted </param>
        ''' <returns> the string representation of the specified {@code char}
        ''' @since 1.4 </returns>
        Public Static String ToString(Char c)
			Return Convert.ToString(c)

        ''' <summary>
        ''' Determines whether the specified code point is a valid
        ''' <a href="http://www.unicode.org/glossary/#code_point">
        ''' Unicode code point value</a>.
        ''' </summary>
        ''' <param name="codePoint"> the Unicode code point to be tested </param>
        ''' <returns> {@code true} if the specified code point value is between
        '''         <seealso cref="#MIN_CODE_POINT"/> and
        '''         <seealso cref="#MAX_CODE_POINT"/> inclusive;
        '''         {@code false} otherwise.
        ''' @since  1.5 </returns>
        Public Static Boolean isValidCodePoint(Integer codePoint)
			' Optimized form of:
			'     codePoint >= MIN_CODE_POINT && codePoint <= MAX_CODE_POINT
			Dim plane As Integer = CInt(CUInt(codePoint) >> 16)
        Return plane < (CInt(CUInt((MAX_CODE_POINT + 1)) >> 16))

        ''' <summary>
        ''' Determines whether the specified character (Unicode code point)
        ''' is in the <a href="#BMP">Basic Multilingual Plane (BMP)</a>.
        ''' Such code points can be represented using a single {@code char}.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested </param>
        ''' <returns> {@code true} if the specified code point is between
        '''         <seealso cref="#MIN_VALUE"/> and <seealso cref="#MAX_VALUE"/> inclusive;
        '''         {@code false} otherwise.
        ''' @since  1.7 </returns>
        Public Static Boolean isBmpCodePoint(Integer codePoint)
			Return CInt(CUInt(codePoint) >> 16 = 0)
        ' Optimized form of:
        '     codePoint >= MIN_VALUE && codePoint <= MAX_VALUE
        ' We consistently use logical shift (>>>) to facilitate
        ' additional runtime optimizations.

        ''' <summary>
        ''' Determines whether the specified character (Unicode code point)
        ''' is in the <a href="#supplementary">supplementary character</a> range.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested </param>
        ''' <returns> {@code true} if the specified code point is between
        '''         <seealso cref="#MIN_SUPPLEMENTARY_CODE_POINT"/> and
        '''         <seealso cref="#MAX_CODE_POINT"/> inclusive;
        '''         {@code false} otherwise.
        ''' @since  1.5 </returns>
        Public Static Boolean isSupplementaryCodePoint(Integer codePoint)
			Return codePoint >= MIN_SUPPLEMENTARY_CODE_POINT AndAlso codePoint < MAX_CODE_POINT + 1

        ''' <summary>
        ''' Determines if the given {@code char} value is a
        ''' <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
        ''' Unicode high-surrogate code unit</a>
        ''' (also known as <i>leading-surrogate code unit</i>).
        ''' 
        ''' <p>Such values do not represent characters by themselves,
        ''' but are used in the representation of
        ''' <a href="#supplementary">supplementary characters</a>
        ''' in the UTF-16 encoding.
        ''' </summary>
        ''' <param name="ch"> the {@code char} value to be tested. </param>
        ''' <returns> {@code true} if the {@code char} value is between
        '''         <seealso cref="#MIN_HIGH_SURROGATE"/> and
        '''         <seealso cref="#MAX_HIGH_SURROGATE"/> inclusive;
        '''         {@code false} otherwise. </returns>
        ''' <seealso cref=    Character#isLowSurrogate(char) </seealso>
        ''' <seealso cref=    Character.UnicodeBlock#of(int)
        ''' @since  1.5 </seealso>
        Public Static Boolean isHighSurrogate(Char ch)
			' Help VM constant-fold; MAX_HIGH_SURROGATE + 1 == MIN_LOW_SURROGATE
			Return ch >= MIN_HIGH_SURROGATE AndAlso ch < (MAX_HIGH_SURROGATE + 1)

        ''' <summary>
        ''' Determines if the given {@code char} value is a
        ''' <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
        ''' Unicode low-surrogate code unit</a>
        ''' (also known as <i>trailing-surrogate code unit</i>).
        ''' 
        ''' <p>Such values do not represent characters by themselves,
        ''' but are used in the representation of
        ''' <a href="#supplementary">supplementary characters</a>
        ''' in the UTF-16 encoding.
        ''' </summary>
        ''' <param name="ch"> the {@code char} value to be tested. </param>
        ''' <returns> {@code true} if the {@code char} value is between
        '''         <seealso cref="#MIN_LOW_SURROGATE"/> and
        '''         <seealso cref="#MAX_LOW_SURROGATE"/> inclusive;
        '''         {@code false} otherwise. </returns>
        ''' <seealso cref=    Character#isHighSurrogate(char)
        ''' @since  1.5 </seealso>
        Public Static Boolean isLowSurrogate(Char ch)
			Return ch >= MIN_LOW_SURROGATE AndAlso ch < (MAX_LOW_SURROGATE + 1)

        ''' <summary>
        ''' Determines if the given {@code char} value is a Unicode
        ''' <i>surrogate code unit</i>.
        ''' 
        ''' <p>Such values do not represent characters by themselves,
        ''' but are used in the representation of
        ''' <a href="#supplementary">supplementary characters</a>
        ''' in the UTF-16 encoding.
        ''' 
        ''' <p>A char value is a surrogate code unit if and only if it is either
        ''' a <seealso cref="#isLowSurrogate(char) low-surrogate code unit"/> or
        ''' a <seealso cref="#isHighSurrogate(char) high-surrogate code unit"/>.
        ''' </summary>
        ''' <param name="ch"> the {@code char} value to be tested. </param>
        ''' <returns> {@code true} if the {@code char} value is between
        '''         <seealso cref="#MIN_SURROGATE"/> and
        '''         <seealso cref="#MAX_SURROGATE"/> inclusive;
        '''         {@code false} otherwise.
        ''' @since  1.7 </returns>
        Public Static Boolean isSurrogate(Char ch)
			Return ch >= MIN_SURROGATE AndAlso ch < (MAX_SURROGATE + 1)

        ''' <summary>
        ''' Determines whether the specified pair of {@code char}
        ''' values is a valid
        ''' <a href="http://www.unicode.org/glossary/#surrogate_pair">
        ''' Unicode surrogate pair</a>.
        ''' 
        ''' <p>This method is equivalent to the expression:
        ''' <blockquote><pre>{@code
        ''' isHighSurrogate(high) && isLowSurrogate(low)
        ''' }</pre></blockquote>
        ''' </summary>
        ''' <param name="high"> the high-surrogate code value to be tested </param>
        ''' <param name="low"> the low-surrogate code value to be tested </param>
        ''' <returns> {@code true} if the specified high and
        ''' low-surrogate code values represent a valid surrogate pair;
        ''' {@code false} otherwise.
        ''' @since  1.5 </returns>
        Public Static Boolean isSurrogatePair(Char high, Char low)
			Return isHighSurrogate(high) AndAlso isLowSurrogate(low)

        ''' <summary>
        ''' Determines the number of {@code char} values needed to
        ''' represent the specified character (Unicode code point). If the
        ''' specified character is equal to or greater than 0x10000, then
        ''' the method returns 2. Otherwise, the method returns 1.
        ''' 
        ''' <p>This method doesn't validate the specified character to be a
        ''' valid Unicode code point. The caller must validate the
        ''' character value using <seealso cref="#isValidCodePoint(int) isValidCodePoint"/>
        ''' if necessary.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  2 if the character is a valid supplementary character; 1 otherwise. </returns>
        ''' <seealso cref=     Character#isSupplementaryCodePoint(int)
        ''' @since   1.5 </seealso>
        Public Static Integer charCount(Integer codePoint)
			Return If(codePoint >= MIN_SUPPLEMENTARY_CODE_POINT, 2, 1)

        ''' <summary>
        ''' Converts the specified surrogate pair to its supplementary code
        ''' point value. This method does not validate the specified
        ''' surrogate pair. The caller must validate it using {@link
        ''' #isSurrogatePair(char, char) isSurrogatePair} if necessary.
        ''' </summary>
        ''' <param name="high"> the high-surrogate code unit </param>
        ''' <param name="low"> the low-surrogate code unit </param>
        ''' <returns> the supplementary code point composed from the
        '''         specified surrogate pair.
        ''' @since  1.5 </returns>
        Public Static Integer toCodePoint(Char high, Char low)
			' Optimized form of:
			' return ((high - MIN_HIGH_SURROGATE) << 10)
			'         + (low - MIN_LOW_SURROGATE)
			'         + MIN_SUPPLEMENTARY_CODE_POINT;
			Return ((high << 10) + low) + (MIN_SUPPLEMENTARY_CODE_POINT - (MIN_HIGH_SURROGATE << 10) - MIN_LOW_SURROGATE)

        ''' <summary>
        ''' Returns the code point at the given index of the
        ''' {@code CharSequence}. If the {@code char} value at
        ''' the given index in the {@code CharSequence} is in the
        ''' high-surrogate range, the following index is less than the
        ''' length of the {@code CharSequence}, and the
        ''' {@code char} value at the following index is in the
        ''' low-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at the given index is returned.
        ''' </summary>
        ''' <param name="seq"> a sequence of {@code char} values (Unicode code
        ''' units) </param>
        ''' <param name="index"> the index to the {@code char} values (Unicode
        ''' code units) in {@code seq} to be converted </param>
        ''' <returns> the Unicode code point at the given index </returns>
        ''' <exception cref="NullPointerException"> if {@code seq} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the value
        ''' {@code index} is negative or not less than
        ''' <seealso cref="CharSequence#length() seq.length()"/>.
        ''' @since  1.5 </exception>
        Public Static Integer codePointAt(CharSequence seq, Integer index)
			Dim c1 As Char = seq.Chars(Index)
			index += 1
			If isHighSurrogate(c1) AndAlso index < seq.length() Then
        Dim c2 As Char = seq.Chars(Index)
        If isLowSurrogate(c2) Then Return toCodePoint(c1, c2)
			End If
        Return c1

        ''' <summary>
        ''' Returns the code point at the given index of the
        ''' {@code char} array. If the {@code char} value at
        ''' the given index in the {@code char} array is in the
        ''' high-surrogate range, the following index is less than the
        ''' length of the {@code char} array, and the
        ''' {@code char} value at the following index is in the
        ''' low-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at the given index is returned.
        ''' </summary>
        ''' <param name="a"> the {@code char} array </param>
        ''' <param name="index"> the index to the {@code char} values (Unicode
        ''' code units) in the {@code char} array to be converted </param>
        ''' <returns> the Unicode code point at the given index </returns>
        ''' <exception cref="NullPointerException"> if {@code a} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the value
        ''' {@code index} is negative or not less than
        ''' the length of the {@code char} array.
        ''' @since  1.5 </exception>
        Public Static Integer codePointAt(Char() a, Integer index)
			Return codePointAtImpl(a, index, a.length)

        ''' <summary>
        ''' Returns the code point at the given index of the
        ''' {@code char} array, where only array elements with
        ''' {@code index} less than {@code limit} can be used. If
        ''' the {@code char} value at the given index in the
        ''' {@code char} array is in the high-surrogate range, the
        ''' following index is less than the {@code limit}, and the
        ''' {@code char} value at the following index is in the
        ''' low-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at the given index is returned.
        ''' </summary>
        ''' <param name="a"> the {@code char} array </param>
        ''' <param name="index"> the index to the {@code char} values (Unicode
        ''' code units) in the {@code char} array to be converted </param>
        ''' <param name="limit"> the index after the last array element that
        ''' can be used in the {@code char} array </param>
        ''' <returns> the Unicode code point at the given index </returns>
        ''' <exception cref="NullPointerException"> if {@code a} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the {@code index}
        ''' argument is negative or not less than the {@code limit}
        ''' argument, or if the {@code limit} argument is negative or
        ''' greater than the length of the {@code char} array.
        ''' @since  1.5 </exception>
        Public Static Integer codePointAt(Char() a, Integer index, Integer limit)
			If index >= limit OrElse limit < 0 OrElse limit > a.length Then Throw New IndexOutOfBoundsException
			Return codePointAtImpl(a, index, limit)

        ' throws ArrayIndexOutOfBoundsException if index out of bounds
        Static Integer codePointAtImpl(Char() a, Integer index, Integer limit)
			Dim c1 As Char = A(Index)
			index += 1
			If isHighSurrogate(c1) AndAlso index < limit Then
        Dim c2 As Char = A(Index)
        If isLowSurrogate(c2) Then Return toCodePoint(c1, c2)
			End If
        Return c1

        ''' <summary>
        ''' Returns the code point preceding the given index of the
        ''' {@code CharSequence}. If the {@code char} value at
        ''' {@code (index - 1)} in the {@code CharSequence} is in
        ''' the low-surrogate range, {@code (index - 2)} is not
        ''' negative, and the {@code char} value at {@code (index - 2)}
        ''' in the {@code CharSequence} is in the
        ''' high-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at {@code (index - 1)} is
        ''' returned.
        ''' </summary>
        ''' <param name="seq"> the {@code CharSequence} instance </param>
        ''' <param name="index"> the index following the code point that should be returned </param>
        ''' <returns> the Unicode code point value before the given index. </returns>
        ''' <exception cref="NullPointerException"> if {@code seq} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the {@code index}
        ''' argument is less than 1 or greater than {@link
        ''' CharSequence#length() seq.length()}.
        ''' @since  1.5 </exception>
        Public Static Integer codePointBefore(CharSequence seq, Integer index)
			index -= 1
			Dim c2 As Char = seq.Chars(Index)
        If isLowSurrogate(c2) AndAlso index > 0 Then
				index -= 1
				Dim c1 As Char = seq.Chars(Index)
        If isHighSurrogate(c1) Then Return toCodePoint(c1, c2)
			End If
        Return c2

        ''' <summary>
        ''' Returns the code point preceding the given index of the
        ''' {@code char} array. If the {@code char} value at
        ''' {@code (index - 1)} in the {@code char} array is in
        ''' the low-surrogate range, {@code (index - 2)} is not
        ''' negative, and the {@code char} value at {@code (index - 2)}
        ''' in the {@code char} array is in the
        ''' high-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at {@code (index - 1)} is
        ''' returned.
        ''' </summary>
        ''' <param name="a"> the {@code char} array </param>
        ''' <param name="index"> the index following the code point that should be returned </param>
        ''' <returns> the Unicode code point value before the given index. </returns>
        ''' <exception cref="NullPointerException"> if {@code a} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the {@code index}
        ''' argument is less than 1 or greater than the length of the
        ''' {@code char} array
        ''' @since  1.5 </exception>
        Public Static Integer codePointBefore(Char() a, Integer index)
			Return codePointBeforeImpl(a, index, 0)

        ''' <summary>
        ''' Returns the code point preceding the given index of the
        ''' {@code char} array, where only array elements with
        ''' {@code index} greater than or equal to {@code start}
        ''' can be used. If the {@code char} value at {@code (index - 1)}
        ''' in the {@code char} array is in the
        ''' low-surrogate range, {@code (index - 2)} is not less than
        ''' {@code start}, and the {@code char} value at
        ''' {@code (index - 2)} in the {@code char} array is in
        ''' the high-surrogate range, then the supplementary code point
        ''' corresponding to this surrogate pair is returned. Otherwise,
        ''' the {@code char} value at {@code (index - 1)} is
        ''' returned.
        ''' </summary>
        ''' <param name="a"> the {@code char} array </param>
        ''' <param name="index"> the index following the code point that should be returned </param>
        ''' <param name="start"> the index of the first array element in the
        ''' {@code char} array </param>
        ''' <returns> the Unicode code point value before the given index. </returns>
        ''' <exception cref="NullPointerException"> if {@code a} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the {@code index}
        ''' argument is not greater than the {@code start} argument or
        ''' is greater than the length of the {@code char} array, or
        ''' if the {@code start} argument is negative or not less than
        ''' the length of the {@code char} array.
        ''' @since  1.5 </exception>
        Public Static Integer codePointBefore(Char() a, Integer index, Integer start)
			If index <= start OrElse start < 0 OrElse start >= a.length Then Throw New IndexOutOfBoundsException
			Return codePointBeforeImpl(a, index, start)

        ' throws ArrayIndexOutOfBoundsException if index-1 out of bounds
        Static Integer codePointBeforeImpl(Char() a, Integer index, Integer start)
			index -= 1
			Dim c2 As Char = A(Index)
        If isLowSurrogate(c2) AndAlso index > start Then
				index -= 1
				Dim c1 As Char = A(Index)
        If isHighSurrogate(c1) Then Return toCodePoint(c1, c2)
			End If
        Return c2

        ''' <summary>
        ''' Returns the leading surrogate (a
        ''' <a href="http://www.unicode.org/glossary/#high_surrogate_code_unit">
        ''' high surrogate code unit</a>) of the
        ''' <a href="http://www.unicode.org/glossary/#surrogate_pair">
        ''' surrogate pair</a>
        ''' representing the specified supplementary character (Unicode
        ''' code point) in the UTF-16 encoding.  If the specified character
        ''' is not a
        ''' <a href="Character.html#supplementary">supplementary character</a>,
        ''' an unspecified {@code char} is returned.
        ''' 
        ''' <p>If
        ''' <seealso cref="#isSupplementaryCodePoint isSupplementaryCodePoint(x)"/>
        ''' is {@code true}, then
        ''' <seealso cref="#isHighSurrogate isHighSurrogate"/>{@code (highSurrogate(x))} and
        ''' <seealso cref="#toCodePoint toCodePoint"/>{@code (highSurrogate(x), }<seealso cref="#lowSurrogate lowSurrogate"/>{@code (x)) == x}
        ''' are also always {@code true}.
        ''' </summary>
        ''' <param name="codePoint"> a supplementary character (Unicode code point) </param>
        ''' <returns>  the leading surrogate code unit used to represent the
        '''          character in the UTF-16 encoding
        ''' @since   1.7 </returns>
        Public Static Char highSurrogate(Integer codePoint)
			Return CChar((CInt(CUInt(codePoint) >> 10)) + (MIN_HIGH_SURROGATE - (CInt(CUInt(MIN_SUPPLEMENTARY_CODE_POINT) >> 10))))

        ''' <summary>
        ''' Returns the trailing surrogate (a
        ''' <a href="http://www.unicode.org/glossary/#low_surrogate_code_unit">
        ''' low surrogate code unit</a>) of the
        ''' <a href="http://www.unicode.org/glossary/#surrogate_pair">
        ''' surrogate pair</a>
        ''' representing the specified supplementary character (Unicode
        ''' code point) in the UTF-16 encoding.  If the specified character
        ''' is not a
        ''' <a href="Character.html#supplementary">supplementary character</a>,
        ''' an unspecified {@code char} is returned.
        ''' 
        ''' <p>If
        ''' <seealso cref="#isSupplementaryCodePoint isSupplementaryCodePoint(x)"/>
        ''' is {@code true}, then
        ''' <seealso cref="#isLowSurrogate isLowSurrogate"/>{@code (lowSurrogate(x))} and
        ''' <seealso cref="#toCodePoint toCodePoint"/>{@code (}<seealso cref="#highSurrogate highSurrogate"/>{@code (x), lowSurrogate(x)) == x}
        ''' are also always {@code true}.
        ''' </summary>
        ''' <param name="codePoint"> a supplementary character (Unicode code point) </param>
        ''' <returns>  the trailing surrogate code unit used to represent the
        '''          character in the UTF-16 encoding
        ''' @since   1.7 </returns>
        Public Static Char lowSurrogate(Integer codePoint)
			Return CChar((codePoint And &H3FF) + MIN_LOW_SURROGATE)

        ''' <summary>
        ''' Converts the specified character (Unicode code point) to its
        ''' UTF-16 representation. If the specified code point is a BMP
        ''' (Basic Multilingual Plane or Plane 0) value, the same value is
        ''' stored in {@code dst[dstIndex]}, and 1 is returned. If the
        ''' specified code point is a supplementary character, its
        ''' surrogate values are stored in {@code dst[dstIndex]}
        ''' (high-surrogate) and {@code dst[dstIndex+1]}
        ''' (low-surrogate), and 2 is returned.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be converted. </param>
        ''' <param name="dst"> an array of {@code char} in which the
        ''' {@code codePoint}'s UTF-16 value is stored. </param>
        ''' <param name="dstIndex"> the start index into the {@code dst}
        ''' array where the converted value is stored. </param>
        ''' <returns> 1 if the code point is a BMP code point, 2 if the
        ''' code point is a supplementary code point. </returns>
        ''' <exception cref="IllegalArgumentException"> if the specified
        ''' {@code codePoint} is not a valid Unicode code point. </exception>
        ''' <exception cref="NullPointerException"> if the specified {@code dst} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if {@code dstIndex}
        ''' is negative or not less than {@code dst.length}, or if
        ''' {@code dst} at {@code dstIndex} doesn't have enough
        ''' array element(s) to store the resulting {@code char}
        ''' value(s). (If {@code dstIndex} is equal to
        ''' {@code dst.length-1} and the specified
        ''' {@code codePoint} is a supplementary character, the
        ''' high-surrogate value is not stored in
        ''' {@code dst[dstIndex]}.)
        ''' @since  1.5 </exception>
        Public Static Integer toChars(Integer codePoint, Char() dst, Integer dstIndex)
			If isBmpCodePoint(codePoint) Then
				dst(dstIndex) = CChar(codePoint)
				Return 1
        ElseIf isValidCodePoint(codePoint) Then
				toSurrogates(codePoint, dst, dstIndex)
				Return 2
        Else
        Throw New IllegalArgumentException
        End If

        ''' <summary>
        ''' Converts the specified character (Unicode code point) to its
        ''' UTF-16 representation stored in a {@code char} array. If
        ''' the specified code point is a BMP (Basic Multilingual Plane or
        ''' Plane 0) value, the resulting {@code char} array has
        ''' the same value as {@code codePoint}. If the specified code
        ''' point is a supplementary code point, the resulting
        ''' {@code char} array has the corresponding surrogate pair.
        ''' </summary>
        ''' <param name="codePoint"> a Unicode code point </param>
        ''' <returns> a {@code char} array having
        '''         {@code codePoint}'s UTF-16 representation. </returns>
        ''' <exception cref="IllegalArgumentException"> if the specified
        ''' {@code codePoint} is not a valid Unicode code point.
        ''' @since  1.5 </exception>
        Public Static Char() toChars(Integer codePoint)
			If isBmpCodePoint(codePoint) Then
        Return New Char() {CChar(codePoint)}
        ElseIf isValidCodePoint(codePoint) Then
        Dim result As Char() = New Char(1) {}
				toSurrogates(codePoint, result, 0)
				Return result
        Else
        Throw New IllegalArgumentException
        End If

        Static void toSurrogates(Integer codePoint, Char() dst, Integer index)
			' We write elements "backwards" to guarantee all-or-nothing
			dst(index+1) = lowSurrogate(codePoint)
			dst(index) = highSurrogate(codePoint)

		''' <summary>
		''' Returns the number of Unicode code points in the text range of
		''' the specified char sequence. The text range begins at the
		''' specified {@code beginIndex} and extends to the
		''' {@code char} at index {@code endIndex - 1}. Thus the
		''' length (in {@code char}s) of the text range is
		''' {@code endIndex-beginIndex}. Unpaired surrogates within
		''' the text range count as one code point each.
		''' </summary>
		''' <param name="seq"> the char sequence </param>
		''' <param name="beginIndex"> the index to the first {@code char} of
		''' the text range. </param>
		''' <param name="endIndex"> the index after the last {@code char} of
		''' the text range. </param>
		''' <returns> the number of Unicode code points in the specified text
		''' range </returns>
		''' <exception cref="NullPointerException"> if {@code seq} is null. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the
		''' {@code beginIndex} is negative, or {@code endIndex}
		''' is larger than the length of the given sequence, or
		''' {@code beginIndex} is larger than {@code endIndex}.
		''' @since  1.5 </exception>
		Public Static Integer codePointCount(CharSequence seq, Integer beginIndex, Integer endIndex)
			Dim length As Integer = seq.length()
        If beginIndex < 0 OrElse endIndex > length OrElse beginIndex > endIndex Then Throw New IndexOutOfBoundsException
			Dim n As Integer = endIndex - beginIndex
        Dim i As Integer = beginIndex
        Do While i < endIndex
        Dim tempVar As Boolean = isHighSurrogate(seq.Chars(i)) AndAlso i < endIndex AndAlso isLowSurrogate(seq.Chars(i))
				i += 1
				If tempVar Then
					n -= 1
					i += 1
				End If
        Loop
        Return n

        ''' <summary>
        ''' Returns the number of Unicode code points in a subarray of the
        ''' {@code char} array argument. The {@code offset}
        ''' argument is the index of the first {@code char} of the
        ''' subarray and the {@code count} argument specifies the
        ''' length of the subarray in {@code char}s. Unpaired
        ''' surrogates within the subarray count as one code point each.
        ''' </summary>
        ''' <param name="a"> the {@code char} array </param>
        ''' <param name="offset"> the index of the first {@code char} in the
        ''' given {@code char} array </param>
        ''' <param name="count"> the length of the subarray in {@code char}s </param>
        ''' <returns> the number of Unicode code points in the specified subarray </returns>
        ''' <exception cref="NullPointerException"> if {@code a} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if {@code offset} or
        ''' {@code count} is negative, or if {@code offset +
        ''' count} is larger than the length of the given array.
        ''' @since  1.5 </exception>
        Public Static Integer codePointCount(Char() a, Integer offset, Integer count)
			If count > a.length - offset OrElse offset < 0 OrElse count < 0 Then Throw New IndexOutOfBoundsException
			Return codePointCountImpl(a, offset, count)

        Static Integer codePointCountImpl(Char() a, Integer offset, Integer count)
			Dim endIndex As Integer = OffSet() + Count()
        Dim n As Integer = Count()
        Dim i As Integer = OffSet()
        Do While i < endIndex
        Dim tempVar2 As Boolean = isHighSurrogate(A(i)) AndAlso i < endIndex AndAlso isLowSurrogate(A(i))
				i += 1
				If tempVar2 Then
					n -= 1
					i += 1
				End If
        Loop
        Return n

        ''' <summary>
        ''' Returns the index within the given char sequence that is offset
        ''' from the given {@code index} by {@code codePointOffset}
        ''' code points. Unpaired surrogates within the text range given by
        ''' {@code index} and {@code codePointOffset} count as
        ''' one code point each.
        ''' </summary>
        ''' <param name="seq"> the char sequence </param>
        ''' <param name="index"> the index to be offset </param>
        ''' <param name="codePointOffset"> the offset in code points </param>
        ''' <returns> the index within the char sequence </returns>
        ''' <exception cref="NullPointerException"> if {@code seq} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if {@code index}
        '''   is negative or larger then the length of the char sequence,
        '''   or if {@code codePointOffset} is positive and the
        '''   subsequence starting with {@code index} has fewer than
        '''   {@code codePointOffset} code points, or if
        '''   {@code codePointOffset} is negative and the subsequence
        '''   before {@code index} has fewer than the absolute value
        '''   of {@code codePointOffset} code points.
        ''' @since 1.5 </exception>
        Public Static Integer offsetByCodePoints(CharSequence seq, Integer index, Integer codePointOffset)
			Dim length As Integer = seq.length()
        If index < 0 OrElse index > length Then Throw New IndexOutOfBoundsException

			Dim x As Integer = Index
        If codePointOffset >= 0 Then
        Dim i As Integer
				i = 0
				Do While x < length AndAlso i < codePointOffset
        Dim tempVar3 As Boolean = isHighSurrogate(seq.Chars(x)) AndAlso x < length AndAlso isLowSurrogate(seq.Chars(x))
					x += 1
					If tempVar3 Then x += 1
					i += 1
				Loop
        If i < codePointOffset Then Throw New IndexOutOfBoundsException
			Else
        Dim i As Integer
				i = codePointOffset
				Do While x > 0 AndAlso i < 0
					x -= 1
					If isLowSurrogate(seq.Chars(x)) AndAlso x > 0 AndAlso isHighSurrogate(seq.Chars(x - 1)) Then x -= 1
					i += 1
				Loop
        If i < 0 Then Throw New IndexOutOfBoundsException
			End If
        Return x

        ''' <summary>
        ''' Returns the index within the given {@code char} subarray
        ''' that is offset from the given {@code index} by
        ''' {@code codePointOffset} code points. The
        ''' {@code start} and {@code count} arguments specify a
        ''' subarray of the {@code char} array. Unpaired surrogates
        ''' within the text range given by {@code index} and
        ''' {@code codePointOffset} count as one code point each.
        ''' </summary>
        ''' <param name="a"> the {@code char} array </param>
        ''' <param name="start"> the index of the first {@code char} of the
        ''' subarray </param>
        ''' <param name="count"> the length of the subarray in {@code char}s </param>
        ''' <param name="index"> the index to be offset </param>
        ''' <param name="codePointOffset"> the offset in code points </param>
        ''' <returns> the index within the subarray </returns>
        ''' <exception cref="NullPointerException"> if {@code a} is null. </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''   if {@code start} or {@code count} is negative,
        '''   or if {@code start + count} is larger than the length of
        '''   the given array,
        '''   or if {@code index} is less than {@code start} or
        '''   larger then {@code start + count},
        '''   or if {@code codePointOffset} is positive and the text range
        '''   starting with {@code index} and ending with {@code start + count - 1}
        '''   has fewer than {@code codePointOffset} code
        '''   points,
        '''   or if {@code codePointOffset} is negative and the text range
        '''   starting with {@code start} and ending with {@code index - 1}
        '''   has fewer than the absolute value of
        '''   {@code codePointOffset} code points.
        ''' @since 1.5 </exception>
        Public Static Integer offsetByCodePoints(Char() a, Integer start, Integer count, Integer index, Integer codePointOffset)
			If count > a.length - start OrElse start < 0 OrElse count < 0 OrElse index < start OrElse index > start + count Then Throw New IndexOutOfBoundsException
			Return offsetByCodePointsImpl(a, start, count, index, codePointOffset)

        Static Integer offsetByCodePointsImpl(Char() a, Integer start, Integer count, Integer index, Integer codePointOffset)
			Dim x As Integer = Index
        If codePointOffset >= 0 Then
        Dim limit As Integer = start + Count()
        Dim i As Integer
				i = 0
				Do While x < limit AndAlso i < codePointOffset
        Dim tempVar4 As Boolean = isHighSurrogate(A(x)) AndAlso x < limit AndAlso isLowSurrogate(A(x))
					x += 1
					If tempVar4 Then x += 1
					i += 1
				Loop
        If i < codePointOffset Then Throw New IndexOutOfBoundsException
			Else
        Dim i As Integer
				i = codePointOffset
				Do While x > start AndAlso i < 0
					x -= 1
					If isLowSurrogate(a(x)) AndAlso x > start AndAlso isHighSurrogate(a(x - 1)) Then x -= 1
					i += 1
				Loop
        If i < 0 Then Throw New IndexOutOfBoundsException
			End If
        Return x

        ''' <summary>
        ''' Determines if the specified character is a lowercase character.
        ''' <p>
        ''' A character is lowercase if its general category type, provided
        ''' by {@code Character.getType(ch)}, is
        ''' {@code LOWERCASE_LETTER}, or it has contributory property
        ''' Other_Lowercase as defined by the Unicode Standard.
        ''' <p>
        ''' The following are examples of lowercase characters:
        ''' <blockquote><pre>
        ''' a b c d e f g h i j k l m n o p q r s t u v w x y z
        ''' '&#92;u00DF' '&#92;u00E0' '&#92;u00E1' '&#92;u00E2' '&#92;u00E3' '&#92;u00E4' '&#92;u00E5' '&#92;u00E6'
        ''' '&#92;u00E7' '&#92;u00E8' '&#92;u00E9' '&#92;u00EA' '&#92;u00EB' '&#92;u00EC' '&#92;u00ED' '&#92;u00EE'
        ''' '&#92;u00EF' '&#92;u00F0' '&#92;u00F1' '&#92;u00F2' '&#92;u00F3' '&#92;u00F4' '&#92;u00F5' '&#92;u00F6'
        ''' '&#92;u00F8' '&#92;u00F9' '&#92;u00FA' '&#92;u00FB' '&#92;u00FC' '&#92;u00FD' '&#92;u00FE' '&#92;u00FF'
        ''' </pre></blockquote>
        ''' <p> Many other Unicode characters are lowercase too.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isLowerCase(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>  {@code true} if the character is lowercase;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     Character#isTitleCase(char) </seealso>
        ''' <seealso cref=     Character#toLowerCase(char) </seealso>
        ''' <seealso cref=     Character#getType(char) </seealso>
        Public Static Boolean isLowerCase(Char ch)
			Return isLowerCase(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a
        ''' lowercase character.
        ''' <p>
        ''' A character is lowercase if its general category type, provided
        ''' by <seealso cref="Character#getType getType(codePoint)"/>, is
        ''' {@code LOWERCASE_LETTER}, or it has contributory property
        ''' Other_Lowercase as defined by the Unicode Standard.
        ''' <p>
        ''' The following are examples of lowercase characters:
        ''' <blockquote><pre>
        ''' a b c d e f g h i j k l m n o p q r s t u v w x y z
        ''' '&#92;u00DF' '&#92;u00E0' '&#92;u00E1' '&#92;u00E2' '&#92;u00E3' '&#92;u00E4' '&#92;u00E5' '&#92;u00E6'
        ''' '&#92;u00E7' '&#92;u00E8' '&#92;u00E9' '&#92;u00EA' '&#92;u00EB' '&#92;u00EC' '&#92;u00ED' '&#92;u00EE'
        ''' '&#92;u00EF' '&#92;u00F0' '&#92;u00F1' '&#92;u00F2' '&#92;u00F3' '&#92;u00F4' '&#92;u00F5' '&#92;u00F6'
        ''' '&#92;u00F8' '&#92;u00F9' '&#92;u00FA' '&#92;u00FB' '&#92;u00FC' '&#92;u00FD' '&#92;u00FE' '&#92;u00FF'
        ''' </pre></blockquote>
        ''' <p> Many other Unicode characters are lowercase too.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is lowercase;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isLowerCase(int) </seealso>
        ''' <seealso cref=     Character#isTitleCase(int) </seealso>
        ''' <seealso cref=     Character#toLowerCase(int) </seealso>
        ''' <seealso cref=     Character#getType(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isLowerCase(Integer codePoint)
			Return [getType](codePoint) = Character.LOWERCASE_LETTER OrElse CharacterData.of(codePoint).isOtherLowercase(codePoint)

        ''' <summary>
        ''' Determines if the specified character is an uppercase character.
        ''' <p>
        ''' A character is uppercase if its general category type, provided by
        ''' {@code Character.getType(ch)}, is {@code UPPERCASE_LETTER}.
        ''' or it has contributory property Other_Uppercase as defined by the Unicode Standard.
        ''' <p>
        ''' The following are examples of uppercase characters:
        ''' <blockquote><pre>
        ''' A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
        ''' '&#92;u00C0' '&#92;u00C1' '&#92;u00C2' '&#92;u00C3' '&#92;u00C4' '&#92;u00C5' '&#92;u00C6' '&#92;u00C7'
        ''' '&#92;u00C8' '&#92;u00C9' '&#92;u00CA' '&#92;u00CB' '&#92;u00CC' '&#92;u00CD' '&#92;u00CE' '&#92;u00CF'
        ''' '&#92;u00D0' '&#92;u00D1' '&#92;u00D2' '&#92;u00D3' '&#92;u00D4' '&#92;u00D5' '&#92;u00D6' '&#92;u00D8'
        ''' '&#92;u00D9' '&#92;u00DA' '&#92;u00DB' '&#92;u00DC' '&#92;u00DD' '&#92;u00DE'
        ''' </pre></blockquote>
        ''' <p> Many other Unicode characters are uppercase too.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isUpperCase(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>  {@code true} if the character is uppercase;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     Character#isTitleCase(char) </seealso>
        ''' <seealso cref=     Character#toUpperCase(char) </seealso>
        ''' <seealso cref=     Character#getType(char)
        ''' @since   1.0 </seealso>
        Public Static Boolean isUpperCase(Char ch)
			Return isUpperCase(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is an uppercase character.
        ''' <p>
        ''' A character is uppercase if its general category type, provided by
        ''' <seealso cref="Character#getType(int) getType(codePoint)"/>, is {@code UPPERCASE_LETTER},
        ''' or it has contributory property Other_Uppercase as defined by the Unicode Standard.
        ''' <p>
        ''' The following are examples of uppercase characters:
        ''' <blockquote><pre>
        ''' A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
        ''' '&#92;u00C0' '&#92;u00C1' '&#92;u00C2' '&#92;u00C3' '&#92;u00C4' '&#92;u00C5' '&#92;u00C6' '&#92;u00C7'
        ''' '&#92;u00C8' '&#92;u00C9' '&#92;u00CA' '&#92;u00CB' '&#92;u00CC' '&#92;u00CD' '&#92;u00CE' '&#92;u00CF'
        ''' '&#92;u00D0' '&#92;u00D1' '&#92;u00D2' '&#92;u00D3' '&#92;u00D4' '&#92;u00D5' '&#92;u00D6' '&#92;u00D8'
        ''' '&#92;u00D9' '&#92;u00DA' '&#92;u00DB' '&#92;u00DC' '&#92;u00DD' '&#92;u00DE'
        ''' </pre></blockquote>
        ''' <p> Many other Unicode characters are uppercase too.<p>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is uppercase;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isLowerCase(int) </seealso>
        ''' <seealso cref=     Character#isTitleCase(int) </seealso>
        ''' <seealso cref=     Character#toUpperCase(int) </seealso>
        ''' <seealso cref=     Character#getType(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isUpperCase(Integer codePoint)
			Return [getType](codePoint) = Character.UPPERCASE_LETTER OrElse CharacterData.of(codePoint).isOtherUppercase(codePoint)

        ''' <summary>
        ''' Determines if the specified character is a titlecase character.
        ''' <p>
        ''' A character is a titlecase character if its general
        ''' category type, provided by {@code Character.getType(ch)},
        ''' is {@code TITLECASE_LETTER}.
        ''' <p>
        ''' Some characters look like pairs of Latin letters. For example, there
        ''' is an uppercase letter that looks like "LJ" and has a corresponding
        ''' lowercase letter that looks like "lj". A third form, which looks like "Lj",
        ''' is the appropriate form to use when rendering a word in lowercase
        ''' with initial capitals, as for a book title.
        ''' <p>
        ''' These are some of the Unicode characters for which this method returns
        ''' {@code true}:
        ''' <ul>
        ''' <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON}
        ''' <li>{@code LATIN CAPITAL LETTER L WITH SMALL LETTER J}
        ''' <li>{@code LATIN CAPITAL LETTER N WITH SMALL LETTER J}
        ''' <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z}
        ''' </ul>
        ''' <p> Many other Unicode characters are titlecase too.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isTitleCase(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>  {@code true} if the character is titlecase;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     Character#isUpperCase(char) </seealso>
        ''' <seealso cref=     Character#toTitleCase(char) </seealso>
        ''' <seealso cref=     Character#getType(char)
        ''' @since   1.0.2 </seealso>
        Public Static Boolean isTitleCase(Char ch)
			Return isTitleCase(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a titlecase character.
        ''' <p>
        ''' A character is a titlecase character if its general
        ''' category type, provided by <seealso cref="Character#getType(int) getType(codePoint)"/>,
        ''' is {@code TITLECASE_LETTER}.
        ''' <p>
        ''' Some characters look like pairs of Latin letters. For example, there
        ''' is an uppercase letter that looks like "LJ" and has a corresponding
        ''' lowercase letter that looks like "lj". A third form, which looks like "Lj",
        ''' is the appropriate form to use when rendering a word in lowercase
        ''' with initial capitals, as for a book title.
        ''' <p>
        ''' These are some of the Unicode characters for which this method returns
        ''' {@code true}:
        ''' <ul>
        ''' <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON}
        ''' <li>{@code LATIN CAPITAL LETTER L WITH SMALL LETTER J}
        ''' <li>{@code LATIN CAPITAL LETTER N WITH SMALL LETTER J}
        ''' <li>{@code LATIN CAPITAL LETTER D WITH SMALL LETTER Z}
        ''' </ul>
        ''' <p> Many other Unicode characters are titlecase too.<p>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is titlecase;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isLowerCase(int) </seealso>
        ''' <seealso cref=     Character#isUpperCase(int) </seealso>
        ''' <seealso cref=     Character#toTitleCase(int) </seealso>
        ''' <seealso cref=     Character#getType(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isTitleCase(Integer codePoint)
			Return [getType](codePoint) = Character.TITLECASE_LETTER

        ''' <summary>
        ''' Determines if the specified character is a digit.
        ''' <p>
        ''' A character is a digit if its general category type, provided
        ''' by {@code Character.getType(ch)}, is
        ''' {@code DECIMAL_DIGIT_NUMBER}.
        ''' <p>
        ''' Some Unicode character ranges that contain digits:
        ''' <ul>
        ''' <li>{@code '\u005Cu0030'} through {@code '\u005Cu0039'},
        '''     ISO-LATIN-1 digits ({@code '0'} through {@code '9'})
        ''' <li>{@code '\u005Cu0660'} through {@code '\u005Cu0669'},
        '''     Arabic-Indic digits
        ''' <li>{@code '\u005Cu06F0'} through {@code '\u005Cu06F9'},
        '''     Extended Arabic-Indic digits
        ''' <li>{@code '\u005Cu0966'} through {@code '\u005Cu096F'},
        '''     Devanagari digits
        ''' <li>{@code '\u005CuFF10'} through {@code '\u005CuFF19'},
        '''     Fullwidth digits
        ''' </ul>
        ''' 
        ''' Many other character ranges contain digits as well.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isDigit(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>  {@code true} if the character is a digit;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#digit(char, int) </seealso>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Character#getType(char) </seealso>
        Public Static Boolean isDigit(Char ch)
			Return isDigit(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a digit.
        ''' <p>
        ''' A character is a digit if its general category type, provided
        ''' by <seealso cref="Character#getType(int) getType(codePoint)"/>, is
        ''' {@code DECIMAL_DIGIT_NUMBER}.
        ''' <p>
        ''' Some Unicode character ranges that contain digits:
        ''' <ul>
        ''' <li>{@code '\u005Cu0030'} through {@code '\u005Cu0039'},
        '''     ISO-LATIN-1 digits ({@code '0'} through {@code '9'})
        ''' <li>{@code '\u005Cu0660'} through {@code '\u005Cu0669'},
        '''     Arabic-Indic digits
        ''' <li>{@code '\u005Cu06F0'} through {@code '\u005Cu06F9'},
        '''     Extended Arabic-Indic digits
        ''' <li>{@code '\u005Cu0966'} through {@code '\u005Cu096F'},
        '''     Devanagari digits
        ''' <li>{@code '\u005CuFF10'} through {@code '\u005CuFF19'},
        '''     Fullwidth digits
        ''' </ul>
        ''' 
        ''' Many other character ranges contain digits as well.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is a digit;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Character#getType(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isDigit(Integer codePoint)
			Return [getType](codePoint) = Character.DECIMAL_DIGIT_NUMBER

        ''' <summary>
        ''' Determines if a character is defined in Unicode.
        ''' <p>
        ''' A character is defined if at least one of the following is true:
        ''' <ul>
        ''' <li>It has an entry in the UnicodeData file.
        ''' <li>It has a value in a range defined by the UnicodeData file.
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isDefined(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested </param>
        ''' <returns>  {@code true} if the character has a defined meaning
        '''          in Unicode; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isDigit(char) </seealso>
        ''' <seealso cref=     Character#isLetter(char) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     Character#isTitleCase(char) </seealso>
        ''' <seealso cref=     Character#isUpperCase(char)
        ''' @since   1.0.2 </seealso>
        Public Static Boolean isDefined(Char ch)
			Return isDefined(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if a character (Unicode code point) is defined in Unicode.
        ''' <p>
        ''' A character is defined if at least one of the following is true:
        ''' <ul>
        ''' <li>It has an entry in the UnicodeData file.
        ''' <li>It has a value in a range defined by the UnicodeData file.
        ''' </ul>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character has a defined meaning
        '''          in Unicode; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isDigit(int) </seealso>
        ''' <seealso cref=     Character#isLetter(int) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(int) </seealso>
        ''' <seealso cref=     Character#isLowerCase(int) </seealso>
        ''' <seealso cref=     Character#isTitleCase(int) </seealso>
        ''' <seealso cref=     Character#isUpperCase(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isDefined(Integer codePoint)
			Return [getType](codePoint) <> Character.UNASSIGNED

        ''' <summary>
        ''' Determines if the specified character is a letter.
        ''' <p>
        ''' A character is considered to be a letter if its general
        ''' category type, provided by {@code Character.getType(ch)},
        ''' is any of the following:
        ''' <ul>
        ''' <li> {@code UPPERCASE_LETTER}
        ''' <li> {@code LOWERCASE_LETTER}
        ''' <li> {@code TITLECASE_LETTER}
        ''' <li> {@code MODIFIER_LETTER}
        ''' <li> {@code OTHER_LETTER}
        ''' </ul>
        ''' 
        ''' Not all letters have case. Many characters are
        ''' letters but are neither uppercase nor lowercase nor titlecase.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isLetter(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>  {@code true} if the character is a letter;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isDigit(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
        ''' <seealso cref=     Character#isJavaLetter(char) </seealso>
        ''' <seealso cref=     Character#isJavaLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     Character#isTitleCase(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(char) </seealso>
        ''' <seealso cref=     Character#isUpperCase(char) </seealso>
        Public Static Boolean isLetter(Char ch)
			Return isLetter(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a letter.
        ''' <p>
        ''' A character is considered to be a letter if its general
        ''' category type, provided by <seealso cref="Character#getType(int) getType(codePoint)"/>,
        ''' is any of the following:
        ''' <ul>
        ''' <li> {@code UPPERCASE_LETTER}
        ''' <li> {@code LOWERCASE_LETTER}
        ''' <li> {@code TITLECASE_LETTER}
        ''' <li> {@code MODIFIER_LETTER}
        ''' <li> {@code OTHER_LETTER}
        ''' </ul>
        ''' 
        ''' Not all letters have case. Many characters are
        ''' letters but are neither uppercase nor lowercase nor titlecase.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is a letter;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isDigit(int) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierStart(int) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(int) </seealso>
        ''' <seealso cref=     Character#isLowerCase(int) </seealso>
        ''' <seealso cref=     Character#isTitleCase(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(int) </seealso>
        ''' <seealso cref=     Character#isUpperCase(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isLetter(Integer codePoint)
			Return ((((1 << Character.UPPERCASE_LETTER) Or (1 << Character.LOWERCASE_LETTER) Or (1 << Character.TITLECASE_LETTER) Or (1 << Character.MODIFIER_LETTER) Or (1 << Character.OTHER_LETTER)) >> [getType](codePoint)) And 1) <> 0

        ''' <summary>
        ''' Determines if the specified character is a letter or digit.
        ''' <p>
        ''' A character is considered to be a letter or digit if either
        ''' {@code Character.isLetter(char ch)} or
        ''' {@code Character.isDigit(char ch)} returns
        ''' {@code true} for the character.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isLetterOrDigit(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>  {@code true} if the character is a letter or digit;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isDigit(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isJavaLetter(char) </seealso>
        ''' <seealso cref=     Character#isJavaLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isLetter(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(char)
        ''' @since   1.0.2 </seealso>
        Public Static Boolean isLetterOrDigit(Char ch)
			Return isLetterOrDigit(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a letter or digit.
        ''' <p>
        ''' A character is considered to be a letter or digit if either
        ''' <seealso cref="#isLetter(int) isLetter(codePoint)"/> or
        ''' <seealso cref="#isDigit(int) isDigit(codePoint)"/> returns
        ''' {@code true} for the character.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is a letter or digit;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isDigit(int) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
        ''' <seealso cref=     Character#isLetter(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isLetterOrDigit(Integer codePoint)
			Return ((((1 << Character.UPPERCASE_LETTER) Or (1 << Character.LOWERCASE_LETTER) Or (1 << Character.TITLECASE_LETTER) Or (1 << Character.MODIFIER_LETTER) Or (1 << Character.OTHER_LETTER) Or (1 << Character.DECIMAL_DIGIT_NUMBER)) >> [getType](codePoint)) And 1) <> 0

        ''' <summary>
        ''' Determines if the specified character is permissible as the first
        ''' character in a Java identifier.
        ''' <p>
        ''' A character may start a Java identifier if and only if
        ''' one of the following is true:
        ''' <ul>
        ''' <li> <seealso cref="#isLetter(char) isLetter(ch)"/> returns {@code true}
        ''' <li> <seealso cref="#getType(char) getType(ch)"/> returns {@code LETTER_NUMBER}
        ''' <li> {@code ch} is a currency symbol (such as {@code '$'})
        ''' <li> {@code ch} is a connecting punctuation character (such as {@code '_'}).
        ''' </ul>
        ''' </summary>
        ''' <param name="ch"> the character to be tested. </param>
        ''' <returns>  {@code true} if the character may start a Java
        '''          identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isLetter(char) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(char)
        ''' @since   1.02 </seealso>
        ''' @deprecated Replaced by isJavaIdentifierStart(char). 
        <Obsolete("Replaced by isJavaIdentifierStart(char).")>
        Public Static Boolean isJavaLetter(Char ch)
			Return isJavaIdentifierStart(ch)

        ''' <summary>
        ''' Determines if the specified character may be part of a Java
        ''' identifier as other than the first character.
        ''' <p>
        ''' A character may be part of a Java identifier if and only if any
        ''' of the following are true:
        ''' <ul>
        ''' <li>  it is a letter
        ''' <li>  it is a currency symbol (such as {@code '$'})
        ''' <li>  it is a connecting punctuation character (such as {@code '_'})
        ''' <li>  it is a digit
        ''' <li>  it is a numeric letter (such as a Roman numeral character)
        ''' <li>  it is a combining mark
        ''' <li>  it is a non-spacing mark
        ''' <li> {@code isIdentifierIgnorable} returns
        ''' {@code true} for the character.
        ''' </ul>
        ''' </summary>
        ''' <param name="ch"> the character to be tested. </param>
        ''' <returns>  {@code true} if the character may be part of a
        '''          Java identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaLetter(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isLetter(char) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isIdentifierIgnorable(char)
        ''' @since   1.02 </seealso>
        ''' @deprecated Replaced by isJavaIdentifierPart(char). 
        <Obsolete("Replaced by isJavaIdentifierPart(char).")>
        Public Static Boolean isJavaLetterOrDigit(Char ch)
			Return isJavaIdentifierPart(ch)

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is an alphabet.
        ''' <p>
        ''' A character is considered to be alphabetic if its general category type,
        ''' provided by <seealso cref="Character#getType(int) getType(codePoint)"/>, is any of
        ''' the following:
        ''' <ul>
        ''' <li> <code>UPPERCASE_LETTER</code>
        ''' <li> <code>LOWERCASE_LETTER</code>
        ''' <li> <code>TITLECASE_LETTER</code>
        ''' <li> <code>MODIFIER_LETTER</code>
        ''' <li> <code>OTHER_LETTER</code>
        ''' <li> <code>LETTER_NUMBER</code>
        ''' </ul>
        ''' or it has contributory property Other_Alphabetic as defined by the
        ''' Unicode Standard.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  <code>true</code> if the character is a Unicode alphabet
        '''          character, <code>false</code> otherwise.
        ''' @since   1.7 </returns>
        Public Static Boolean isAlphabetic(Integer codePoint)
			Return (((((1 << Character.UPPERCASE_LETTER) Or (1 << Character.LOWERCASE_LETTER) Or (1 << Character.TITLECASE_LETTER) Or (1 << Character.MODIFIER_LETTER) Or (1 << Character.OTHER_LETTER) Or (1 << Character.LETTER_NUMBER)) >> [getType](codePoint)) And 1) <> 0) OrElse CharacterData.of(codePoint).isOtherAlphabetic(codePoint)

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a CJKV
        ''' (Chinese, Japanese, Korean and Vietnamese) ideograph, as defined by
        ''' the Unicode Standard.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  <code>true</code> if the character is a Unicode ideograph
        '''          character, <code>false</code> otherwise.
        ''' @since   1.7 </returns>
        Public Static Boolean isIdeographic(Integer codePoint)
			Return CharacterData.of(codePoint).isIdeographic(codePoint)

        ''' <summary>
        ''' Determines if the specified character is
        ''' permissible as the first character in a Java identifier.
        ''' <p>
        ''' A character may start a Java identifier if and only if
        ''' one of the following conditions is true:
        ''' <ul>
        ''' <li> <seealso cref="#isLetter(char) isLetter(ch)"/> returns {@code true}
        ''' <li> <seealso cref="#getType(char) getType(ch)"/> returns {@code LETTER_NUMBER}
        ''' <li> {@code ch} is a currency symbol (such as {@code '$'})
        ''' <li> {@code ch} is a connecting punctuation character (such as {@code '_'}).
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isJavaIdentifierStart(int)"/> method.
        ''' </summary>
        ''' <param name="ch"> the character to be tested. </param>
        ''' <returns>  {@code true} if the character may start a Java identifier;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isLetter(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(char) </seealso>
        ''' <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
        ''' @since   1.1 </seealso>
        Public Static Boolean isJavaIdentifierStart(Char ch)
			Return isJavaIdentifierStart(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the character (Unicode code point) is
        ''' permissible as the first character in a Java identifier.
        ''' <p>
        ''' A character may start a Java identifier if and only if
        ''' one of the following conditions is true:
        ''' <ul>
        ''' <li> <seealso cref="#isLetter(int) isLetter(codePoint)"/>
        '''      returns {@code true}
        ''' <li> <seealso cref="#getType(int) getType(codePoint)"/>
        '''      returns {@code LETTER_NUMBER}
        ''' <li> the referenced character is a currency symbol (such as {@code '$'})
        ''' <li> the referenced character is a connecting punctuation character
        '''      (such as {@code '_'}).
        ''' </ul>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character may start a Java identifier;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
        ''' <seealso cref=     Character#isLetter(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(int) </seealso>
        ''' <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
        ''' @since   1.5 </seealso>
        Public Static Boolean isJavaIdentifierStart(Integer codePoint)
			Return CharacterData.of(codePoint).isJavaIdentifierStart(codePoint)

        ''' <summary>
        ''' Determines if the specified character may be part of a Java
        ''' identifier as other than the first character.
        ''' <p>
        ''' A character may be part of a Java identifier if any of the following
        ''' are true:
        ''' <ul>
        ''' <li>  it is a letter
        ''' <li>  it is a currency symbol (such as {@code '$'})
        ''' <li>  it is a connecting punctuation character (such as {@code '_'})
        ''' <li>  it is a digit
        ''' <li>  it is a numeric letter (such as a Roman numeral character)
        ''' <li>  it is a combining mark
        ''' <li>  it is a non-spacing mark
        ''' <li> {@code isIdentifierIgnorable} returns
        ''' {@code true} for the character
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isJavaIdentifierPart(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns> {@code true} if the character may be part of a
        '''          Java identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isIdentifierIgnorable(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(char) </seealso>
        ''' <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
        ''' @since   1.1 </seealso>
        Public Static Boolean isJavaIdentifierPart(Char ch)
			Return isJavaIdentifierPart(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the character (Unicode code point) may be part of a Java
        ''' identifier as other than the first character.
        ''' <p>
        ''' A character may be part of a Java identifier if any of the following
        ''' are true:
        ''' <ul>
        ''' <li>  it is a letter
        ''' <li>  it is a currency symbol (such as {@code '$'})
        ''' <li>  it is a connecting punctuation character (such as {@code '_'})
        ''' <li>  it is a digit
        ''' <li>  it is a numeric letter (such as a Roman numeral character)
        ''' <li>  it is a combining mark
        ''' <li>  it is a non-spacing mark
        ''' <li> {@link #isIdentifierIgnorable(int)
        ''' isIdentifierIgnorable(codePoint)} returns {@code true} for
        ''' the character
        ''' </ul>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns> {@code true} if the character may be part of a
        '''          Java identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isIdentifierIgnorable(int) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierStart(int) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(int) </seealso>
        ''' <seealso cref=     javax.lang.model.SourceVersion#isIdentifier(CharSequence)
        ''' @since   1.5 </seealso>
        Public Static Boolean isJavaIdentifierPart(Integer codePoint)
			Return CharacterData.of(codePoint).isJavaIdentifierPart(codePoint)

        ''' <summary>
        ''' Determines if the specified character is permissible as the
        ''' first character in a Unicode identifier.
        ''' <p>
        ''' A character may start a Unicode identifier if and only if
        ''' one of the following conditions is true:
        ''' <ul>
        ''' <li> <seealso cref="#isLetter(char) isLetter(ch)"/> returns {@code true}
        ''' <li> <seealso cref="#getType(char) getType(ch)"/> returns
        '''      {@code LETTER_NUMBER}.
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isUnicodeIdentifierStart(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns>  {@code true} if the character may start a Unicode
        '''          identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaIdentifierStart(char) </seealso>
        ''' <seealso cref=     Character#isLetter(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(char)
        ''' @since   1.1 </seealso>
        Public Static Boolean isUnicodeIdentifierStart(Char ch)
			Return isUnicodeIdentifierStart(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is permissible as the
        ''' first character in a Unicode identifier.
        ''' <p>
        ''' A character may start a Unicode identifier if and only if
        ''' one of the following conditions is true:
        ''' <ul>
        ''' <li> <seealso cref="#isLetter(int) isLetter(codePoint)"/>
        '''      returns {@code true}
        ''' <li> <seealso cref="#getType(int) getType(codePoint)"/>
        '''      returns {@code LETTER_NUMBER}.
        ''' </ul> </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character may start a Unicode
        '''          identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaIdentifierStart(int) </seealso>
        ''' <seealso cref=     Character#isLetter(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isUnicodeIdentifierStart(Integer codePoint)
			Return CharacterData.of(codePoint).isUnicodeIdentifierStart(codePoint)

        ''' <summary>
        ''' Determines if the specified character may be part of a Unicode
        ''' identifier as other than the first character.
        ''' <p>
        ''' A character may be part of a Unicode identifier if and only if
        ''' one of the following statements is true:
        ''' <ul>
        ''' <li>  it is a letter
        ''' <li>  it is a connecting punctuation character (such as {@code '_'})
        ''' <li>  it is a digit
        ''' <li>  it is a numeric letter (such as a Roman numeral character)
        ''' <li>  it is a combining mark
        ''' <li>  it is a non-spacing mark
        ''' <li> {@code isIdentifierIgnorable} returns
        ''' {@code true} for this character.
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isUnicodeIdentifierPart(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns>  {@code true} if the character may be part of a
        '''          Unicode identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isIdentifierIgnorable(char) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(char)
        ''' @since   1.1 </seealso>
        Public Static Boolean isUnicodeIdentifierPart(Char ch)
			Return isUnicodeIdentifierPart(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) may be part of a Unicode
        ''' identifier as other than the first character.
        ''' <p>
        ''' A character may be part of a Unicode identifier if and only if
        ''' one of the following statements is true:
        ''' <ul>
        ''' <li>  it is a letter
        ''' <li>  it is a connecting punctuation character (such as {@code '_'})
        ''' <li>  it is a digit
        ''' <li>  it is a numeric letter (such as a Roman numeral character)
        ''' <li>  it is a combining mark
        ''' <li>  it is a non-spacing mark
        ''' <li> {@code isIdentifierIgnorable} returns
        ''' {@code true} for this character.
        ''' </ul> </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character may be part of a
        '''          Unicode identifier; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isIdentifierIgnorable(int) </seealso>
        ''' <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
        ''' <seealso cref=     Character#isLetterOrDigit(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierStart(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isUnicodeIdentifierPart(Integer codePoint)
			Return CharacterData.of(codePoint).isUnicodeIdentifierPart(codePoint)

        ''' <summary>
        ''' Determines if the specified character should be regarded as
        ''' an ignorable character in a Java identifier or a Unicode identifier.
        ''' <p>
        ''' The following Unicode characters are ignorable in a Java identifier
        ''' or a Unicode identifier:
        ''' <ul>
        ''' <li>ISO control characters that are not whitespace
        ''' <ul>
        ''' <li>{@code '\u005Cu0000'} through {@code '\u005Cu0008'}
        ''' <li>{@code '\u005Cu000E'} through {@code '\u005Cu001B'}
        ''' <li>{@code '\u005Cu007F'} through {@code '\u005Cu009F'}
        ''' </ul>
        ''' 
        ''' <li>all characters that have the {@code FORMAT} general
        ''' category value
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isIdentifierIgnorable(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns>  {@code true} if the character is an ignorable control
        '''          character that may be part of a Java or Unicode identifier;
        '''           {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaIdentifierPart(char) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(char)
        ''' @since   1.1 </seealso>
        Public Static Boolean isIdentifierIgnorable(Char ch)
			Return isIdentifierIgnorable(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) should be regarded as
        ''' an ignorable character in a Java identifier or a Unicode identifier.
        ''' <p>
        ''' The following Unicode characters are ignorable in a Java identifier
        ''' or a Unicode identifier:
        ''' <ul>
        ''' <li>ISO control characters that are not whitespace
        ''' <ul>
        ''' <li>{@code '\u005Cu0000'} through {@code '\u005Cu0008'}
        ''' <li>{@code '\u005Cu000E'} through {@code '\u005Cu001B'}
        ''' <li>{@code '\u005Cu007F'} through {@code '\u005Cu009F'}
        ''' </ul>
        ''' 
        ''' <li>all characters that have the {@code FORMAT} general
        ''' category value
        ''' </ul>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is an ignorable control
        '''          character that may be part of a Java or Unicode identifier;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isJavaIdentifierPart(int) </seealso>
        ''' <seealso cref=     Character#isUnicodeIdentifierPart(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isIdentifierIgnorable(Integer codePoint)
			Return CharacterData.of(codePoint).isIdentifierIgnorable(codePoint)

        ''' <summary>
        ''' Converts the character argument to lowercase using case
        ''' mapping information from the UnicodeData file.
        ''' <p>
        ''' Note that
        ''' {@code Character.isLowerCase(Character.toLowerCase(ch))}
        ''' does not always return {@code true} for some ranges of
        ''' characters, particularly those that are symbols or ideographs.
        ''' 
        ''' <p>In general, <seealso cref="String#toLowerCase()"/> should be used to map
        ''' characters to lowercase. {@code String} case mapping methods
        ''' have several benefits over {@code Character} case mapping methods.
        ''' {@code String} case mapping methods can perform locale-sensitive
        ''' mappings, context-sensitive mappings, and 1:M character mappings, whereas
        ''' the {@code Character} case mapping methods cannot.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#toLowerCase(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be converted. </param>
        ''' <returns>  the lowercase equivalent of the character, if any;
        '''          otherwise, the character itself. </returns>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     String#toLowerCase() </seealso>
        Public Static Char ToLower(Char ch)
			Return CChar(ToLower(CInt(Fix(ch))))

        ''' <summary>
        ''' Converts the character (Unicode code point) argument to
        ''' lowercase using case mapping information from the UnicodeData
        ''' file.
        ''' 
        ''' <p> Note that
        ''' {@code Character.isLowerCase(Character.toLowerCase(codePoint))}
        ''' does not always return {@code true} for some ranges of
        ''' characters, particularly those that are symbols or ideographs.
        ''' 
        ''' <p>In general, <seealso cref="String#toLowerCase()"/> should be used to map
        ''' characters to lowercase. {@code String} case mapping methods
        ''' have several benefits over {@code Character} case mapping methods.
        ''' {@code String} case mapping methods can perform locale-sensitive
        ''' mappings, context-sensitive mappings, and 1:M character mappings, whereas
        ''' the {@code Character} case mapping methods cannot.
        ''' </summary>
        ''' <param name="codePoint">   the character (Unicode code point) to be converted. </param>
        ''' <returns>  the lowercase equivalent of the character (Unicode code
        '''          point), if any; otherwise, the character itself. </returns>
        ''' <seealso cref=     Character#isLowerCase(int) </seealso>
        ''' <seealso cref=     String#toLowerCase()
        ''' 
        ''' @since   1.5 </seealso>
        Public Static Integer ToLower(Integer codePoint)
			Return CharacterData.of(codePoint).ToLower(codePoint)

        ''' <summary>
        ''' Converts the character argument to uppercase using case mapping
        ''' information from the UnicodeData file.
        ''' <p>
        ''' Note that
        ''' {@code Character.isUpperCase(Character.toUpperCase(ch))}
        ''' does not always return {@code true} for some ranges of
        ''' characters, particularly those that are symbols or ideographs.
        ''' 
        ''' <p>In general, <seealso cref="String#toUpperCase()"/> should be used to map
        ''' characters to uppercase. {@code String} case mapping methods
        ''' have several benefits over {@code Character} case mapping methods.
        ''' {@code String} case mapping methods can perform locale-sensitive
        ''' mappings, context-sensitive mappings, and 1:M character mappings, whereas
        ''' the {@code Character} case mapping methods cannot.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#toUpperCase(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be converted. </param>
        ''' <returns>  the uppercase equivalent of the character, if any;
        '''          otherwise, the character itself. </returns>
        ''' <seealso cref=     Character#isUpperCase(char) </seealso>
        ''' <seealso cref=     String#toUpperCase() </seealso>
        Public Static Char ToUpper(Char ch)
			Return CChar(ToUpper(CInt(Fix(ch))))

        ''' <summary>
        ''' Converts the character (Unicode code point) argument to
        ''' uppercase using case mapping information from the UnicodeData
        ''' file.
        ''' 
        ''' <p>Note that
        ''' {@code Character.isUpperCase(Character.toUpperCase(codePoint))}
        ''' does not always return {@code true} for some ranges of
        ''' characters, particularly those that are symbols or ideographs.
        ''' 
        ''' <p>In general, <seealso cref="String#toUpperCase()"/> should be used to map
        ''' characters to uppercase. {@code String} case mapping methods
        ''' have several benefits over {@code Character} case mapping methods.
        ''' {@code String} case mapping methods can perform locale-sensitive
        ''' mappings, context-sensitive mappings, and 1:M character mappings, whereas
        ''' the {@code Character} case mapping methods cannot.
        ''' </summary>
        ''' <param name="codePoint">   the character (Unicode code point) to be converted. </param>
        ''' <returns>  the uppercase equivalent of the character, if any;
        '''          otherwise, the character itself. </returns>
        ''' <seealso cref=     Character#isUpperCase(int) </seealso>
        ''' <seealso cref=     String#toUpperCase()
        ''' 
        ''' @since   1.5 </seealso>
        Public Static Integer ToUpper(Integer codePoint)
			Return CharacterData.of(codePoint).ToUpper(codePoint)

        ''' <summary>
        ''' Converts the character argument to titlecase using case mapping
        ''' information from the UnicodeData file. If a character has no
        ''' explicit titlecase mapping and is not itself a titlecase char
        ''' according to UnicodeData, then the uppercase mapping is
        ''' returned as an equivalent titlecase mapping. If the
        ''' {@code char} argument is already a titlecase
        ''' {@code char}, the same {@code char} value will be
        ''' returned.
        ''' <p>
        ''' Note that
        ''' {@code Character.isTitleCase(Character.toTitleCase(ch))}
        ''' does not always return {@code true} for some ranges of
        ''' characters.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#toTitleCase(int)"/> method.
        ''' </summary>
        ''' <param name="ch">   the character to be converted. </param>
        ''' <returns>  the titlecase equivalent of the character, if any;
        '''          otherwise, the character itself. </returns>
        ''' <seealso cref=     Character#isTitleCase(char) </seealso>
        ''' <seealso cref=     Character#toLowerCase(char) </seealso>
        ''' <seealso cref=     Character#toUpperCase(char)
        ''' @since   1.0.2 </seealso>
        Public Static Char toTitleCase(Char ch)
			Return CChar(toTitleCase(CInt(Fix(ch))))

        ''' <summary>
        ''' Converts the character (Unicode code point) argument to titlecase using case mapping
        ''' information from the UnicodeData file. If a character has no
        ''' explicit titlecase mapping and is not itself a titlecase char
        ''' according to UnicodeData, then the uppercase mapping is
        ''' returned as an equivalent titlecase mapping. If the
        ''' character argument is already a titlecase
        ''' character, the same character value will be
        ''' returned.
        ''' 
        ''' <p>Note that
        ''' {@code Character.isTitleCase(Character.toTitleCase(codePoint))}
        ''' does not always return {@code true} for some ranges of
        ''' characters.
        ''' </summary>
        ''' <param name="codePoint">   the character (Unicode code point) to be converted. </param>
        ''' <returns>  the titlecase equivalent of the character, if any;
        '''          otherwise, the character itself. </returns>
        ''' <seealso cref=     Character#isTitleCase(int) </seealso>
        ''' <seealso cref=     Character#toLowerCase(int) </seealso>
        ''' <seealso cref=     Character#toUpperCase(int)
        ''' @since   1.5 </seealso>
        Public Static Integer toTitleCase(Integer codePoint)
			Return CharacterData.of(codePoint).toTitleCase(codePoint)

        ''' <summary>
        ''' Returns the numeric value of the character {@code ch} in the
        ''' specified radix.
        ''' <p>
        ''' If the radix is not in the range {@code MIN_RADIX} &le;
        ''' {@code radix} &le; {@code MAX_RADIX} or if the
        ''' value of {@code ch} is not a valid digit in the specified
        ''' radix, {@code -1} is returned. A character is a valid digit
        ''' if at least one of the following is true:
        ''' <ul>
        ''' <li>The method {@code isDigit} is {@code true} of the character
        '''     and the Unicode decimal digit value of the character (or its
        '''     single-character decomposition) is less than the specified radix.
        '''     In this case the decimal digit value is returned.
        ''' <li>The character is one of the uppercase Latin letters
        '''     {@code 'A'} through {@code 'Z'} and its code is less than
        '''     {@code radix + 'A' - 10}.
        '''     In this case, {@code ch - 'A' + 10}
        '''     is returned.
        ''' <li>The character is one of the lowercase Latin letters
        '''     {@code 'a'} through {@code 'z'} and its code is less than
        '''     {@code radix + 'a' - 10}.
        '''     In this case, {@code ch - 'a' + 10}
        '''     is returned.
        ''' <li>The character is one of the fullwidth uppercase Latin letters A
        '''     ({@code '\u005CuFF21'}) through Z ({@code '\u005CuFF3A'})
        '''     and its code is less than
        '''     {@code radix + '\u005CuFF21' - 10}.
        '''     In this case, {@code ch - '\u005CuFF21' + 10}
        '''     is returned.
        ''' <li>The character is one of the fullwidth lowercase Latin letters a
        '''     ({@code '\u005CuFF41'}) through z ({@code '\u005CuFF5A'})
        '''     and its code is less than
        '''     {@code radix + '\u005CuFF41' - 10}.
        '''     In this case, {@code ch - '\u005CuFF41' + 10}
        '''     is returned.
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#digit(int, int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be converted. </param>
        ''' <param name="radix">   the radix. </param>
        ''' <returns>  the numeric value represented by the character in the
        '''          specified radix. </returns>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Character#isDigit(char) </seealso>
        Public Static Integer digit(Char ch, Integer radix)
			Return digit(CInt(Fix(ch)), radix)

        ''' <summary>
        ''' Returns the numeric value of the specified character (Unicode
        ''' code point) in the specified radix.
        ''' 
        ''' <p>If the radix is not in the range {@code MIN_RADIX} &le;
        ''' {@code radix} &le; {@code MAX_RADIX} or if the
        ''' character is not a valid digit in the specified
        ''' radix, {@code -1} is returned. A character is a valid digit
        ''' if at least one of the following is true:
        ''' <ul>
        ''' <li>The method <seealso cref="#isDigit(int) isDigit(codePoint)"/> is {@code true} of the character
        '''     and the Unicode decimal digit value of the character (or its
        '''     single-character decomposition) is less than the specified radix.
        '''     In this case the decimal digit value is returned.
        ''' <li>The character is one of the uppercase Latin letters
        '''     {@code 'A'} through {@code 'Z'} and its code is less than
        '''     {@code radix + 'A' - 10}.
        '''     In this case, {@code codePoint - 'A' + 10}
        '''     is returned.
        ''' <li>The character is one of the lowercase Latin letters
        '''     {@code 'a'} through {@code 'z'} and its code is less than
        '''     {@code radix + 'a' - 10}.
        '''     In this case, {@code codePoint - 'a' + 10}
        '''     is returned.
        ''' <li>The character is one of the fullwidth uppercase Latin letters A
        '''     ({@code '\u005CuFF21'}) through Z ({@code '\u005CuFF3A'})
        '''     and its code is less than
        '''     {@code radix + '\u005CuFF21' - 10}.
        '''     In this case,
        '''     {@code codePoint - '\u005CuFF21' + 10}
        '''     is returned.
        ''' <li>The character is one of the fullwidth lowercase Latin letters a
        '''     ({@code '\u005CuFF41'}) through z ({@code '\u005CuFF5A'})
        '''     and its code is less than
        '''     {@code radix + '\u005CuFF41'- 10}.
        '''     In this case,
        '''     {@code codePoint - '\u005CuFF41' + 10}
        '''     is returned.
        ''' </ul>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be converted. </param>
        ''' <param name="radix">   the radix. </param>
        ''' <returns>  the numeric value represented by the character in the
        '''          specified radix. </returns>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Character#isDigit(int)
        ''' @since   1.5 </seealso>
        Public Static Integer digit(Integer codePoint, Integer radix)
			Return CharacterData.of(codePoint).digit(codePoint, radix)

        ''' <summary>
        ''' Returns the {@code int} value that the specified Unicode
        ''' character represents. For example, the character
        ''' {@code '\u005Cu216C'} (the roman numeral fifty) will return
        ''' an int with a value of 50.
        ''' <p>
        ''' The letters A-Z in their uppercase ({@code '\u005Cu0041'} through
        ''' {@code '\u005Cu005A'}), lowercase
        ''' ({@code '\u005Cu0061'} through {@code '\u005Cu007A'}), and
        ''' full width variant ({@code '\u005CuFF21'} through
        ''' {@code '\u005CuFF3A'} and {@code '\u005CuFF41'} through
        ''' {@code '\u005CuFF5A'}) forms have numeric values from 10
        ''' through 35. This is independent of the Unicode specification,
        ''' which does not assign numeric values to these {@code char}
        ''' values.
        ''' <p>
        ''' If the character does not have a numeric value, then -1 is returned.
        ''' If the character has a numeric value that cannot be represented as a
        ''' nonnegative integer (for example, a fractional value), then -2
        ''' is returned.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#getNumericValue(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be converted. </param>
        ''' <returns>  the numeric value of the character, as a nonnegative {@code int}
        '''           value; -2 if the character has a numeric value that is not a
        '''          nonnegative integer; -1 if the character has no numeric value. </returns>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Character#isDigit(char)
        ''' @since   1.1 </seealso>
        Public Static Integer getNumericValue(Char ch)
			Return getNumericValue(CInt(Fix(ch)))

        ''' <summary>
        ''' Returns the {@code int} value that the specified
        ''' character (Unicode code point) represents. For example, the character
        ''' {@code '\u005Cu216C'} (the Roman numeral fifty) will return
        ''' an {@code int} with a value of 50.
        ''' <p>
        ''' The letters A-Z in their uppercase ({@code '\u005Cu0041'} through
        ''' {@code '\u005Cu005A'}), lowercase
        ''' ({@code '\u005Cu0061'} through {@code '\u005Cu007A'}), and
        ''' full width variant ({@code '\u005CuFF21'} through
        ''' {@code '\u005CuFF3A'} and {@code '\u005CuFF41'} through
        ''' {@code '\u005CuFF5A'}) forms have numeric values from 10
        ''' through 35. This is independent of the Unicode specification,
        ''' which does not assign numeric values to these {@code char}
        ''' values.
        ''' <p>
        ''' If the character does not have a numeric value, then -1 is returned.
        ''' If the character has a numeric value that cannot be represented as a
        ''' nonnegative integer (for example, a fractional value), then -2
        ''' is returned.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be converted. </param>
        ''' <returns>  the numeric value of the character, as a nonnegative {@code int}
        '''          value; -2 if the character has a numeric value that is not a
        '''          nonnegative integer; -1 if the character has no numeric value. </returns>
        ''' <seealso cref=     Character#forDigit(int, int) </seealso>
        ''' <seealso cref=     Character#isDigit(int)
        ''' @since   1.5 </seealso>
        Public Static Integer getNumericValue(Integer codePoint)
			Return CharacterData.of(codePoint).getNumericValue(codePoint)

        ''' <summary>
        ''' Determines if the specified character is ISO-LATIN-1 white space.
        ''' This method returns {@code true} for the following five
        ''' characters only:
        ''' <table summary="truechars">
        ''' <tr><td>{@code '\t'}</td>            <td>{@code U+0009}</td>
        '''     <td>{@code HORIZONTAL TABULATION}</td></tr>
        ''' <tr><td>{@code '\n'}</td>            <td>{@code U+000A}</td>
        '''     <td>{@code NEW LINE}</td></tr>
        ''' <tr><td>{@code '\f'}</td>            <td>{@code U+000C}</td>
        '''     <td>{@code FORM FEED}</td></tr>
        ''' <tr><td>{@code '\r'}</td>            <td>{@code U+000D}</td>
        '''     <td>{@code CARRIAGE RETURN}</td></tr>
        ''' <tr><td>{@code '&nbsp;'}</td>  <td>{@code U+0020}</td>
        '''     <td>{@code SPACE}</td></tr>
        ''' </table>
        ''' </summary>
        ''' <param name="ch">   the character to be tested. </param>
        ''' <returns>     {@code true} if the character is ISO-LATIN-1 white
        '''             space; {@code false} otherwise. </returns>
        ''' <seealso cref=        Character#isSpaceChar(char) </seealso>
        ''' <seealso cref=        Character#isWhitespace(char) </seealso>
        ''' @deprecated Replaced by isWhitespace(char). 
        <Obsolete("Replaced by isWhitespace(char).")>
        Public Static Boolean isSpace(Char ch)
			Return (ch <= &H20) AndAlso (((((1L << &H9) Or (1L << &HA) Or (1L << &HC) Or (1L << &HD) Or (1L << &H20)) >> ch) And 1L) <> 0)


        ''' <summary>
        ''' Determines if the specified character is a Unicode space character.
        ''' A character is considered to be a space character if and only if
        ''' it is specified to be a space character by the Unicode Standard. This
        ''' method returns true if the character's general category type is any of
        ''' the following:
        ''' <ul>
        ''' <li> {@code SPACE_SEPARATOR}
        ''' <li> {@code LINE_SEPARATOR}
        ''' <li> {@code PARAGRAPH_SEPARATOR}
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isSpaceChar(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns>  {@code true} if the character is a space character;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isWhitespace(char)
        ''' @since   1.1 </seealso>
        Public Static Boolean isSpaceChar(Char ch)
			Return isSpaceChar(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is a
        ''' Unicode space character.  A character is considered to be a
        ''' space character if and only if it is specified to be a space
        ''' character by the Unicode Standard. This method returns true if
        ''' the character's general category type is any of the following:
        ''' 
        ''' <ul>
        ''' <li> <seealso cref="#SPACE_SEPARATOR"/>
        ''' <li> <seealso cref="#LINE_SEPARATOR"/>
        ''' <li> <seealso cref="#PARAGRAPH_SEPARATOR"/>
        ''' </ul>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is a space character;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isWhitespace(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isSpaceChar(Integer codePoint)
			Return ((((1 << Character.SPACE_SEPARATOR) Or (1 << Character.LINE_SEPARATOR) Or (1 << Character.PARAGRAPH_SEPARATOR)) >> [getType](codePoint)) And 1) <> 0

        ''' <summary>
        ''' Determines if the specified character is white space according to Java.
        ''' A character is a Java whitespace character if and only if it satisfies
        ''' one of the following criteria:
        ''' <ul>
        ''' <li> It is a Unicode space character ({@code SPACE_SEPARATOR},
        '''      {@code LINE_SEPARATOR}, or {@code PARAGRAPH_SEPARATOR})
        '''      but is not also a non-breaking space ({@code '\u005Cu00A0'},
        '''      {@code '\u005Cu2007'}, {@code '\u005Cu202F'}).
        ''' <li> It is {@code '\u005Ct'}, U+0009 HORIZONTAL TABULATION.
        ''' <li> It is {@code '\u005Cn'}, U+000A LINE FEED.
        ''' <li> It is {@code '\u005Cu000B'}, U+000B VERTICAL TABULATION.
        ''' <li> It is {@code '\u005Cf'}, U+000C FORM FEED.
        ''' <li> It is {@code '\u005Cr'}, U+000D CARRIAGE RETURN.
        ''' <li> It is {@code '\u005Cu001C'}, U+001C FILE SEPARATOR.
        ''' <li> It is {@code '\u005Cu001D'}, U+001D GROUP SEPARATOR.
        ''' <li> It is {@code '\u005Cu001E'}, U+001E RECORD SEPARATOR.
        ''' <li> It is {@code '\u005Cu001F'}, U+001F UNIT SEPARATOR.
        ''' </ul>
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isWhitespace(int)"/> method.
        ''' </summary>
        ''' <param name="ch"> the character to be tested. </param>
        ''' <returns>  {@code true} if the character is a Java whitespace
        '''          character; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isSpaceChar(char)
        ''' @since   1.1 </seealso>
        Public Static Boolean isWhitespace(Char ch)
			Return isWhitespace(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the specified character (Unicode code point) is
        ''' white space according to Java.  A character is a Java
        ''' whitespace character if and only if it satisfies one of the
        ''' following criteria:
        ''' <ul>
        ''' <li> It is a Unicode space character (<seealso cref="#SPACE_SEPARATOR"/>,
        '''      <seealso cref="#LINE_SEPARATOR"/>, or <seealso cref="#PARAGRAPH_SEPARATOR"/>)
        '''      but is not also a non-breaking space ({@code '\u005Cu00A0'},
        '''      {@code '\u005Cu2007'}, {@code '\u005Cu202F'}).
        ''' <li> It is {@code '\u005Ct'}, U+0009 HORIZONTAL TABULATION.
        ''' <li> It is {@code '\u005Cn'}, U+000A LINE FEED.
        ''' <li> It is {@code '\u005Cu000B'}, U+000B VERTICAL TABULATION.
        ''' <li> It is {@code '\u005Cf'}, U+000C FORM FEED.
        ''' <li> It is {@code '\u005Cr'}, U+000D CARRIAGE RETURN.
        ''' <li> It is {@code '\u005Cu001C'}, U+001C FILE SEPARATOR.
        ''' <li> It is {@code '\u005Cu001D'}, U+001D GROUP SEPARATOR.
        ''' <li> It is {@code '\u005Cu001E'}, U+001E RECORD SEPARATOR.
        ''' <li> It is {@code '\u005Cu001F'}, U+001F UNIT SEPARATOR.
        ''' </ul>
        ''' <p>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is a Java whitespace
        '''          character; {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isSpaceChar(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isWhitespace(Integer codePoint)
			Return CharacterData.of(codePoint).isWhitespace(codePoint)

        ''' <summary>
        ''' Determines if the specified character is an ISO control
        ''' character.  A character is considered to be an ISO control
        ''' character if its code is in the range {@code '\u005Cu0000'}
        ''' through {@code '\u005Cu001F'} or in the range
        ''' {@code '\u005Cu007F'} through {@code '\u005Cu009F'}.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isISOControl(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns>  {@code true} if the character is an ISO control character;
        '''          {@code false} otherwise.
        ''' </returns>
        ''' <seealso cref=     Character#isSpaceChar(char) </seealso>
        ''' <seealso cref=     Character#isWhitespace(char)
        ''' @since   1.1 </seealso>
        Public Static Boolean isISOControl(Char ch)
			Return isISOControl(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines if the referenced character (Unicode code point) is an ISO control
        ''' character.  A character is considered to be an ISO control
        ''' character if its code is in the range {@code '\u005Cu0000'}
        ''' through {@code '\u005Cu001F'} or in the range
        ''' {@code '\u005Cu007F'} through {@code '\u005Cu009F'}.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is an ISO control character;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     Character#isSpaceChar(int) </seealso>
        ''' <seealso cref=     Character#isWhitespace(int)
        ''' @since   1.5 </seealso>
        Public Static Boolean isISOControl(Integer codePoint)
			' Optimized form of:
			'     (codePoint >= 0x00 && codePoint <= 0x1F) ||
			'     (codePoint >= 0x7F && codePoint <= 0x9F);
			Return codePoint <= &H9F AndAlso (codePoint >= &H7F OrElse (CInt(CUInt(codePoint) >> 5 = 0)))

        ''' <summary>
        ''' Returns a value indicating a character's general category.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#getType(int)"/> method.
        ''' </summary>
        ''' <param name="ch">      the character to be tested. </param>
        ''' <returns>  a value of type {@code int} representing the
        '''          character's general category. </returns>
        ''' <seealso cref=     Character#COMBINING_SPACING_MARK </seealso>
        ''' <seealso cref=     Character#CONNECTOR_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#CONTROL </seealso>
        ''' <seealso cref=     Character#CURRENCY_SYMBOL </seealso>
        ''' <seealso cref=     Character#DASH_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#DECIMAL_DIGIT_NUMBER </seealso>
        ''' <seealso cref=     Character#ENCLOSING_MARK </seealso>
        ''' <seealso cref=     Character#END_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#FINAL_QUOTE_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#FORMAT </seealso>
        ''' <seealso cref=     Character#INITIAL_QUOTE_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#LETTER_NUMBER </seealso>
        ''' <seealso cref=     Character#LINE_SEPARATOR </seealso>
        ''' <seealso cref=     Character#LOWERCASE_LETTER </seealso>
        ''' <seealso cref=     Character#MATH_SYMBOL </seealso>
        ''' <seealso cref=     Character#MODIFIER_LETTER </seealso>
        ''' <seealso cref=     Character#MODIFIER_SYMBOL </seealso>
        ''' <seealso cref=     Character#NON_SPACING_MARK </seealso>
        ''' <seealso cref=     Character#OTHER_LETTER </seealso>
        ''' <seealso cref=     Character#OTHER_NUMBER </seealso>
        ''' <seealso cref=     Character#OTHER_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#OTHER_SYMBOL </seealso>
        ''' <seealso cref=     Character#PARAGRAPH_SEPARATOR </seealso>
        ''' <seealso cref=     Character#PRIVATE_USE </seealso>
        ''' <seealso cref=     Character#SPACE_SEPARATOR </seealso>
        ''' <seealso cref=     Character#START_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#SURROGATE </seealso>
        ''' <seealso cref=     Character#TITLECASE_LETTER </seealso>
        ''' <seealso cref=     Character#UNASSIGNED </seealso>
        ''' <seealso cref=     Character#UPPERCASE_LETTER
        ''' @since   1.1 </seealso>
        Public Static Integer [getType](Char ch)
			Return [getType](CInt(Fix(ch)))

        ''' <summary>
        ''' Returns a value indicating a character's general category.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  a value of type {@code int} representing the
        '''          character's general category. </returns>
        ''' <seealso cref=     Character#COMBINING_SPACING_MARK COMBINING_SPACING_MARK </seealso>
        ''' <seealso cref=     Character#CONNECTOR_PUNCTUATION CONNECTOR_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#CONTROL CONTROL </seealso>
        ''' <seealso cref=     Character#CURRENCY_SYMBOL CURRENCY_SYMBOL </seealso>
        ''' <seealso cref=     Character#DASH_PUNCTUATION DASH_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#DECIMAL_DIGIT_NUMBER DECIMAL_DIGIT_NUMBER </seealso>
        ''' <seealso cref=     Character#ENCLOSING_MARK ENCLOSING_MARK </seealso>
        ''' <seealso cref=     Character#END_PUNCTUATION END_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#FINAL_QUOTE_PUNCTUATION FINAL_QUOTE_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#FORMAT FORMAT </seealso>
        ''' <seealso cref=     Character#INITIAL_QUOTE_PUNCTUATION INITIAL_QUOTE_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#LETTER_NUMBER LETTER_NUMBER </seealso>
        ''' <seealso cref=     Character#LINE_SEPARATOR LINE_SEPARATOR </seealso>
        ''' <seealso cref=     Character#LOWERCASE_LETTER LOWERCASE_LETTER </seealso>
        ''' <seealso cref=     Character#MATH_SYMBOL MATH_SYMBOL </seealso>
        ''' <seealso cref=     Character#MODIFIER_LETTER MODIFIER_LETTER </seealso>
        ''' <seealso cref=     Character#MODIFIER_SYMBOL MODIFIER_SYMBOL </seealso>
        ''' <seealso cref=     Character#NON_SPACING_MARK NON_SPACING_MARK </seealso>
        ''' <seealso cref=     Character#OTHER_LETTER OTHER_LETTER </seealso>
        ''' <seealso cref=     Character#OTHER_NUMBER OTHER_NUMBER </seealso>
        ''' <seealso cref=     Character#OTHER_PUNCTUATION OTHER_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#OTHER_SYMBOL OTHER_SYMBOL </seealso>
        ''' <seealso cref=     Character#PARAGRAPH_SEPARATOR PARAGRAPH_SEPARATOR </seealso>
        ''' <seealso cref=     Character#PRIVATE_USE PRIVATE_USE </seealso>
        ''' <seealso cref=     Character#SPACE_SEPARATOR SPACE_SEPARATOR </seealso>
        ''' <seealso cref=     Character#START_PUNCTUATION START_PUNCTUATION </seealso>
        ''' <seealso cref=     Character#SURROGATE SURROGATE </seealso>
        ''' <seealso cref=     Character#TITLECASE_LETTER TITLECASE_LETTER </seealso>
        ''' <seealso cref=     Character#UNASSIGNED UNASSIGNED </seealso>
        ''' <seealso cref=     Character#UPPERCASE_LETTER UPPERCASE_LETTER
        ''' @since   1.5 </seealso>
        Public Overloads Shared Function [getType](codePoint As Integer) As Integer
            Return CharacterData.of(codePoint).getType(codePoint)
        End Function
        ''' <summary>
        ''' Determines the character representation for a specific digit in
        ''' the specified radix. If the value of {@code radix} is not a
        ''' valid radix, or the value of {@code digit} is not a valid
        ''' digit in the specified radix, the null character
        ''' ({@code '\u005Cu0000'}) is returned.
        ''' <p>
        ''' The {@code radix} argument is valid if it is greater than or
        ''' equal to {@code MIN_RADIX} and less than or equal to
        ''' {@code MAX_RADIX}. The {@code digit} argument is valid if
        ''' {@code 0 <= digit < radix}.
        ''' <p>
        ''' If the digit is less than 10, then
        ''' {@code '0' + digit} is returned. Otherwise, the value
        ''' {@code 'a' + digit - 10} is returned.
        ''' </summary>
        ''' <param name="digit">   the number to convert to a character. </param>
        ''' <param name="radix">   the radix. </param>
        ''' <returns>  the {@code char} representation of the specified digit
        '''          in the specified radix. </returns>
        ''' <seealso cref=     Character#MIN_RADIX </seealso>
        ''' <seealso cref=     Character#MAX_RADIX </seealso>
        ''' <seealso cref=     Character#digit(char, int) </seealso>
        Public Static Char forDigit(Integer digit, Integer radix)
			If (digit >= radix) OrElse (digit < 0) Then Return ControlChars.NullChar
			If (radix < Character.MIN_RADIX) OrElse (radix > Character.MAX_RADIX) Then Return ControlChars.NullChar
			If digit < 10 Then Return ChrW(AscW("0"c) + digit)
			Return ChrW(AscW("a"c) - 10 + digit)

        ''' <summary>
        ''' Returns the Unicode directionality property for the given
        ''' character.  Character directionality is used to calculate the
        ''' visual ordering of text. The directionality value of undefined
        ''' {@code char} values is {@code DIRECTIONALITY_UNDEFINED}.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#getDirectionality(int)"/> method.
        ''' </summary>
        ''' <param name="ch"> {@code char} for which the directionality property
        '''            is requested. </param>
        ''' <returns> the directionality property of the {@code char} value.
        ''' </returns>
        ''' <seealso cref= Character#DIRECTIONALITY_UNDEFINED </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_ARABIC_NUMBER </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_COMMON_NUMBER_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_NONSPACING_MARK </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_BOUNDARY_NEUTRAL </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_PARAGRAPH_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_SEGMENT_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_WHITESPACE </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_OTHER_NEUTRALS </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_POP_DIRECTIONAL_FORMAT
        ''' @since 1.4 </seealso>
        Public Static SByte getDirectionality(Char ch)
			Return getDirectionality(CInt(Fix(ch)))

        ''' <summary>
        ''' Returns the Unicode directionality property for the given
        ''' character (Unicode code point).  Character directionality is
        ''' used to calculate the visual ordering of text. The
        ''' directionality value of undefined character is {@link
        ''' #DIRECTIONALITY_UNDEFINED}.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) for which
        '''          the directionality property is requested. </param>
        ''' <returns> the directionality property of the character.
        ''' </returns>
        ''' <seealso cref= Character#DIRECTIONALITY_UNDEFINED DIRECTIONALITY_UNDEFINED </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT DIRECTIONALITY_LEFT_TO_RIGHT </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT DIRECTIONALITY_RIGHT_TO_LEFT </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER DIRECTIONALITY_EUROPEAN_NUMBER </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_ARABIC_NUMBER DIRECTIONALITY_ARABIC_NUMBER </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_COMMON_NUMBER_SEPARATOR DIRECTIONALITY_COMMON_NUMBER_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_NONSPACING_MARK DIRECTIONALITY_NONSPACING_MARK </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_BOUNDARY_NEUTRAL DIRECTIONALITY_BOUNDARY_NEUTRAL </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_PARAGRAPH_SEPARATOR DIRECTIONALITY_PARAGRAPH_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_SEGMENT_SEPARATOR DIRECTIONALITY_SEGMENT_SEPARATOR </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_WHITESPACE DIRECTIONALITY_WHITESPACE </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_OTHER_NEUTRALS DIRECTIONALITY_OTHER_NEUTRALS </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE </seealso>
        ''' <seealso cref= Character#DIRECTIONALITY_POP_DIRECTIONAL_FORMAT DIRECTIONALITY_POP_DIRECTIONAL_FORMAT
        ''' @since    1.5 </seealso>
        Public Static SByte getDirectionality(Integer codePoint)
			Return CharacterData.of(codePoint).getDirectionality(codePoint)

        ''' <summary>
        ''' Determines whether the character is mirrored according to the
        ''' Unicode specification.  Mirrored characters should have their
        ''' glyphs horizontally mirrored when displayed in text that is
        ''' right-to-left.  For example, {@code '\u005Cu0028'} LEFT
        ''' PARENTHESIS is semantically defined to be an <i>opening
        ''' parenthesis</i>.  This will appear as a "(" in text that is
        ''' left-to-right but as a ")" in text that is right-to-left.
        ''' 
        ''' <p><b>Note:</b> This method cannot handle <a
        ''' href="#supplementary"> supplementary characters</a>. To support
        ''' all Unicode characters, including supplementary characters, use
        ''' the <seealso cref="#isMirrored(int)"/> method.
        ''' </summary>
        ''' <param name="ch"> {@code char} for which the mirrored property is requested </param>
        ''' <returns> {@code true} if the char is mirrored, {@code false}
        '''         if the {@code char} is not mirrored or is not defined.
        ''' @since 1.4 </returns>
        Public Static Boolean isMirrored(Char ch)
			Return isMirrored(CInt(Fix(ch)))

        ''' <summary>
        ''' Determines whether the specified character (Unicode code point)
        ''' is mirrored according to the Unicode specification.  Mirrored
        ''' characters should have their glyphs horizontally mirrored when
        ''' displayed in text that is right-to-left.  For example,
        ''' {@code '\u005Cu0028'} LEFT PARENTHESIS is semantically
        ''' defined to be an <i>opening parenthesis</i>.  This will appear
        ''' as a "(" in text that is left-to-right but as a ")" in text
        ''' that is right-to-left.
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point) to be tested. </param>
        ''' <returns>  {@code true} if the character is mirrored, {@code false}
        '''          if the character is not mirrored or is not defined.
        ''' @since   1.5 </returns>
        Public Static Boolean isMirrored(Integer codePoint)
			Return CharacterData.of(codePoint).isMirrored(codePoint)

        ''' <summary>
        ''' Compares two {@code Character} objects numerically.
        ''' </summary>
        ''' <param name="anotherCharacter">   the {@code Character} to be compared.
        ''' </param>
        ''' <returns>  the value {@code 0} if the argument {@code Character}
        '''          is equal to this {@code Character}; a value less than
        '''          {@code 0} if this {@code Character} is numerically less
        '''          than the {@code Character} argument; and a value greater than
        '''          {@code 0} if this {@code Character} is numerically greater
        '''          than the {@code Character} argument (unsigned comparison).
        '''          Note that this is strictly a numerical comparison; it is not
        '''          locale-dependent.
        ''' @since   1.2 </returns>
        Public Function compareTo(anotherCharacter As Character) As Integer
            Return compare(Me.value, anotherCharacter.value)
        End Function

        ''' <summary>
        ''' Compares two {@code char} values numerically.
        ''' The value returned is identical to what would be returned by:
        ''' <pre>
        '''    Character.valueOf(x).compareTo(Character.valueOf(y))
        ''' </pre>
        ''' </summary>
        ''' <param name="x"> the first {@code char} to compare </param>
        ''' <param name="y"> the second {@code char} to compare </param>
        ''' <returns> the value {@code 0} if {@code x == y};
        '''         a value less than {@code 0} if {@code x < y}; and
        '''         a value greater than {@code 0} if {@code x > y}
        ''' @since 1.7 </returns>
        Public Shared Function compare(x As [Char], y As [Char]) As Integer
            Return x - y
        End Function
        ''' <summary>
        ''' Converts the character (Unicode code point) argument to uppercase using
        ''' information from the UnicodeData file.
        ''' <p>
        ''' </summary>
        ''' <param name="codePoint">   the character (Unicode code point) to be converted. </param>
        ''' <returns>  either the uppercase equivalent of the character, if
        '''          any, or an error flag ({@code Character.ERROR})
        '''          that indicates that a 1:M {@code char} mapping exists. </returns>
        ''' <seealso cref=     Character#isLowerCase(char) </seealso>
        ''' <seealso cref=     Character#isUpperCase(char) </seealso>
        ''' <seealso cref=     Character#toLowerCase(char) </seealso>
        ''' <seealso cref=     Character#toTitleCase(char)
        ''' @since 1.4 </seealso>
        Static Integer toUpperCaseEx(Integer codePoint)
			Debug.Assert(isValidCodePoint(codePoint))
			Return CharacterData.of(codePoint).toUpperCaseEx(codePoint)

        ''' <summary>
        ''' Converts the character (Unicode code point) argument to uppercase using case
        ''' mapping information from the SpecialCasing file in the Unicode
        ''' specification. If a character has no explicit uppercase
        ''' mapping, then the {@code char} itself is returned in the
        ''' {@code char[]}.
        ''' </summary>
        ''' <param name="codePoint">   the character (Unicode code point) to be converted. </param>
        ''' <returns> a {@code char[]} with the uppercased character.
        ''' @since 1.4 </returns>
        Static Char() toUpperCaseCharArray(Integer codePoint)
			' As of Unicode 6.0, 1:M uppercasings only happen in the BMP.
			Debug.Assert(isBmpCodePoint(codePoint))
			Return CharacterData.of(codePoint).toUpperCaseCharArray(codePoint)

        ''' <summary>
        ''' The number of bits used to represent a <tt>char</tt> value in unsigned
        ''' binary form, constant {@code 16}.
        ''' 
        ''' @since 1.5
        ''' </summary>
        Public Static final Integer SIZE = 16

		''' <summary>
		''' The number of bytes used to represent a {@code char} value in unsigned
		''' binary form.
		''' 
		''' @since 1.8
		''' </summary>
		Public Static final Integer BYTES = SIZE \ Byte.SIZE

		''' <summary>
		''' Returns the value obtained by reversing the order of the bytes in the
		''' specified <tt>char</tt> value.
		''' </summary>
		''' <param name="ch"> The {@code char} of which to reverse the byte order. </param>
		''' <returns> the value obtained by reversing (or, equivalently, swapping)
		'''     the bytes in the specified <tt>char</tt> value.
		''' @since 1.5 </returns>
		Public Shared Function reverseBytes(ch As [Char]) As Char
            Return CChar(((ch And &HFF00) >> 8) Or (ch << 8))
        End Function
        ''' <summary>
        ''' Returns the Unicode name of the specified character
        ''' {@code codePoint}, or null if the code point is
        ''' <seealso cref="#UNASSIGNED unassigned"/>.
        ''' <p>
        ''' Note: if the specified character is not assigned a name by
        ''' the <i>UnicodeData</i> file (part of the Unicode Character
        ''' Database maintained by the Unicode Consortium), the returned
        ''' name is the same as the result of expression.
        ''' 
        ''' <blockquote>{@code
        '''     Character.UnicodeBlock.of(codePoint).toString().replace('_', ' ')
        '''     + " "
        '''     + Integer.toHexString(codePoint).toUpperCase(Locale.ENGLISH);
        ''' 
        ''' }</blockquote>
        ''' </summary>
        ''' <param name="codePoint"> the character (Unicode code point)
        ''' </param>
        ''' <returns> the Unicode name of the specified character, or null if
        '''         the code point is unassigned.
        ''' </returns>
        ''' <exception cref="IllegalArgumentException"> if the specified
        '''            {@code codePoint} is not a valid Unicode
        '''            code point.
        ''' 
        ''' @since 1.7 </exception>
        Public Shared Function getName(codePoint As Integer) As String
            If Not isValidCodePoint(codePoint) Then Throw New IllegalArgumentException
            Dim name_Renamed As String = CharacterName.get(codePoint)
            If name_Renamed IsNot Nothing Then Return name_Renamed
            If [getType](codePoint) = UNASSIGNED Then Return Nothing
            Dim block As UnicodeBlock = UnicodeBlock.of(codePoint)
            If block IsNot Nothing Then Return block.ToString().Replace("_"c, " "c) & " " & Integer.toHexString(codePoint).ToUpper(java.util.Locale.ENGLISH)
            ' should never come here
            Return Integer.toHexString(codePoint).ToUpper(java.util.Locale.ENGLISH)
        End Function
    End Class
    End Class
End Namespace
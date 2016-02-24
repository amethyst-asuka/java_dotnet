Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports sun.font.EAttribute

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
	''' The <code>Font</code> class represents fonts, which are used to
	''' render text in a visible way.
	''' A font provides the information needed to map sequences of
	''' <em>characters</em> to sequences of <em>glyphs</em>
	''' and to render sequences of glyphs on <code>Graphics</code> and
	''' <code>Component</code> objects.
	''' 
	''' <h3>Characters and Glyphs</h3>
	''' 
	''' A <em>character</em> is a symbol that represents an item such as a letter,
	''' a digit, or punctuation in an abstract way. For example, <code>'g'</code>,
	''' LATIN SMALL LETTER G, is a character.
	''' <p>
	''' A <em>glyph</em> is a shape used to render a character or a sequence of
	''' characters. In simple writing systems, such as Latin, typically one glyph
	''' represents one character. In general, however, characters and glyphs do not
	''' have one-to-one correspondence. For example, the character '&aacute;'
	''' LATIN SMALL LETTER A WITH ACUTE, can be represented by
	''' two glyphs: one for 'a' and one for '&acute;'. On the other hand, the
	''' two-character string "fi" can be represented by a single glyph, an
	''' "fi" ligature. In complex writing systems, such as Arabic or the South
	''' and South-East Asian writing systems, the relationship between characters
	''' and glyphs can be more complicated and involve context-dependent selection
	''' of glyphs as well as glyph reordering.
	''' 
	''' A font encapsulates the collection of glyphs needed to render a selected set
	''' of characters as well as the tables needed to map sequences of characters to
	''' corresponding sequences of glyphs.
	''' 
	''' <h3>Physical and Logical Fonts</h3>
	''' 
	''' The Java Platform distinguishes between two kinds of fonts:
	''' <em>physical</em> fonts and <em>logical</em> fonts.
	''' <p>
	''' <em>Physical</em> fonts are the actual font libraries containing glyph data
	''' and tables to map from character sequences to glyph sequences, using a font
	''' technology such as TrueType or PostScript Type 1.
	''' All implementations of the Java Platform must support TrueType fonts;
	''' support for other font technologies is implementation dependent.
	''' Physical fonts may use names such as Helvetica, Palatino, HonMincho, or
	''' any number of other font names.
	''' Typically, each physical font supports only a limited set of writing
	''' systems, for example, only Latin characters or only Japanese and Basic
	''' Latin.
	''' The set of available physical fonts varies between configurations.
	''' Applications that require specific fonts can bundle them and instantiate
	''' them using the <seealso cref="#createFont createFont"/> method.
	''' <p>
	''' <em>Logical</em> fonts are the five font families defined by the Java
	''' platform which must be supported by any Java runtime environment:
	''' Serif, SansSerif, Monospaced, Dialog, and DialogInput.
	''' These logical fonts are not actual font libraries. Instead, the logical
	''' font names are mapped to physical fonts by the Java runtime environment.
	''' The mapping is implementation and usually locale dependent, so the look
	''' and the metrics provided by them vary.
	''' Typically, each logical font name maps to several physical fonts in order to
	''' cover a large range of characters.
	''' <p>
	''' Peered AWT components, such as <seealso cref="Label Label"/> and
	''' <seealso cref="TextField TextField"/>, can only use logical fonts.
	''' <p>
	''' For a discussion of the relative advantages and disadvantages of using
	''' physical or logical fonts, see the
	''' <a href="http://www.oracle.com/technetwork/java/javase/tech/faq-jsp-138165.html">Internationalization FAQ</a>
	''' document.
	''' 
	''' <h3>Font Faces and Names</h3>
	''' 
	''' A <code>Font</code>
	''' can have many faces, such as heavy, medium, oblique, gothic and
	''' regular. All of these faces have similar typographic design.
	''' <p>
	''' There are three different names that you can get from a
	''' <code>Font</code> object.  The <em>logical font name</em> is simply the
	''' name that was used to construct the font.
	''' The <em>font face name</em>, or just <em>font name</em> for
	''' short, is the name of a particular font face, like Helvetica Bold. The
	''' <em>family name</em> is the name of the font family that determines the
	''' typographic design across several faces, like Helvetica.
	''' <p>
	''' The <code>Font</code> class represents an instance of a font face from
	''' a collection of  font faces that are present in the system resources
	''' of the host system.  As examples, Arial Bold and Courier Bold Italic
	''' are font faces.  There can be several <code>Font</code> objects
	''' associated with a font face, each differing in size, style, transform
	''' and font features.
	''' <p>
	''' The <seealso cref="GraphicsEnvironment#getAllFonts() getAllFonts"/> method
	''' of the <code>GraphicsEnvironment</code> class returns an
	''' array of all font faces available in the system. These font faces are
	''' returned as <code>Font</code> objects with a size of 1, identity
	''' transform and default font features. These
	''' base fonts can then be used to derive new <code>Font</code> objects
	''' with varying sizes, styles, transforms and font features via the
	''' <code>deriveFont</code> methods in this class.
	''' 
	''' <h3>Font and TextAttribute</h3>
	''' 
	''' <p><code>Font</code> supports most
	''' <code>TextAttribute</code>s.  This makes some operations, such as
	''' rendering underlined text, convenient since it is not
	''' necessary to explicitly construct a <code>TextLayout</code> object.
	''' Attributes can be set on a Font by constructing or deriving it
	''' using a <code>Map</code> of <code>TextAttribute</code> values.
	''' 
	''' <p>The values of some <code>TextAttributes</code> are not
	''' serializable, and therefore attempting to serialize an instance of
	''' <code>Font</code> that has such values will not serialize them.
	''' This means a Font deserialized from such a stream will not compare
	''' equal to the original Font that contained the non-serializable
	''' attributes.  This should very rarely pose a problem
	''' since these attributes are typically used only in special
	''' circumstances and are unlikely to be serialized.
	''' 
	''' <ul>
	''' <li><code>FOREGROUND</code> and <code>BACKGROUND</code> use
	''' <code>Paint</code> values. The subclass <code>Color</code> is
	''' serializable, while <code>GradientPaint</code> and
	''' <code>TexturePaint</code> are not.</li>
	''' <li><code>CHAR_REPLACEMENT</code> uses
	''' <code>GraphicAttribute</code> values.  The subclasses
	''' <code>ShapeGraphicAttribute</code> and
	''' <code>ImageGraphicAttribute</code> are not serializable.</li>
	''' <li><code>INPUT_METHOD_HIGHLIGHT</code> uses
	''' <code>InputMethodHighlight</code> values, which are
	''' not serializable.  See <seealso cref="java.awt.im.InputMethodHighlight"/>.</li>
	''' </ul>
	''' 
	''' <p>Clients who create custom subclasses of <code>Paint</code> and
	''' <code>GraphicAttribute</code> can make them serializable and
	''' avoid this problem.  Clients who use input method highlights can
	''' convert these to the platform-specific attributes for that
	''' highlight on the current platform and set them on the Font as
	''' a workaround.
	''' 
	''' <p>The <code>Map</code>-based constructor and
	''' <code>deriveFont</code> APIs ignore the FONT attribute, and it is
	''' not retained by the Font; the static <seealso cref="#getFont"/> method should
	''' be used if the FONT attribute might be present.  See {@link
	''' java.awt.font.TextAttribute#FONT} for more information.</p>
	''' 
	''' <p>Several attributes will cause additional rendering overhead
	''' and potentially invoke layout.  If a <code>Font</code> has such
	''' attributes, the <code><seealso cref="#hasLayoutAttributes()"/></code> method
	''' will return true.</p>
	''' 
	''' <p>Note: Font rotations can cause text baselines to be rotated.  In
	''' order to account for this (rare) possibility, font APIs are
	''' specified to return metrics and take parameters 'in
	''' baseline-relative coordinates'.  This maps the 'x' coordinate to
	''' the advance along the baseline, (positive x is forward along the
	''' baseline), and the 'y' coordinate to a distance along the
	''' perpendicular to the baseline at 'x' (positive y is 90 degrees
	''' clockwise from the baseline vector).  APIs for which this is
	''' especially important are called out as having 'baseline-relative
	''' coordinates.'
	''' </summary>
	<Serializable> _
	Public Class Font
		Private Class FontAccessImpl
			Inherits sun.font.FontAccess

			Public Overridable Function getFont2D(ByVal font_Renamed As Font) As sun.font.Font2D
				Return font_Renamed.font2D
			End Function

			Public Overridable Sub setFont2D(ByVal font_Renamed As Font, ByVal handle As sun.font.Font2DHandle)
				font_Renamed.font2DHandle = handle
			End Sub

			Public Overridable Property createdFont As Font
				Set(ByVal font_Renamed As Font)
					font_Renamed.createdFont = True
				End Set
			End Property

			Public Overridable Function isCreatedFont(ByVal font_Renamed As Font) As Boolean
				Return font_Renamed.createdFont
			End Function
		End Class

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			initIDs()
			sun.font.FontAccess.fontAccess = New FontAccessImpl
		End Sub

		''' <summary>
		''' This is now only used during serialization.  Typically
		''' it is null.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getAttributes() </seealso>
		Private fRequestedAttributes As Dictionary(Of Object, Object)

	'    
	'     * Constants to be used for logical font family names.
	'     

		''' <summary>
		''' A String constant for the canonical family name of the
		''' logical font "Dialog". It is useful in Font construction
		''' to provide compile-time verification of the name.
		''' @since 1.6
		''' </summary>
		Public Const DIALOG As String = "Dialog"

		''' <summary>
		''' A String constant for the canonical family name of the
		''' logical font "DialogInput". It is useful in Font construction
		''' to provide compile-time verification of the name.
		''' @since 1.6
		''' </summary>
		Public Const DIALOG_INPUT As String = "DialogInput"

		''' <summary>
		''' A String constant for the canonical family name of the
		''' logical font "SansSerif". It is useful in Font construction
		''' to provide compile-time verification of the name.
		''' @since 1.6
		''' </summary>
		Public Const SANS_SERIF As String = "SansSerif"

		''' <summary>
		''' A String constant for the canonical family name of the
		''' logical font "Serif". It is useful in Font construction
		''' to provide compile-time verification of the name.
		''' @since 1.6
		''' </summary>
		Public Const SERIF As String = "Serif"

		''' <summary>
		''' A String constant for the canonical family name of the
		''' logical font "Monospaced". It is useful in Font construction
		''' to provide compile-time verification of the name.
		''' @since 1.6
		''' </summary>
		Public Const MONOSPACED As String = "Monospaced"

	'    
	'     * Constants to be used for styles. Can be combined to mix
	'     * styles.
	'     

		''' <summary>
		''' The plain style constant.
		''' </summary>
		Public Const PLAIN As Integer = 0

		''' <summary>
		''' The bold style constant.  This can be combined with the other style
		''' constants (except PLAIN) for mixed styles.
		''' </summary>
		Public Const BOLD As Integer = 1

		''' <summary>
		''' The italicized style constant.  This can be combined with the other
		''' style constants (except PLAIN) for mixed styles.
		''' </summary>
		Public Const ITALIC As Integer = 2

		''' <summary>
		''' The baseline used in most Roman scripts when laying out text.
		''' </summary>
		Public Const ROMAN_BASELINE As Integer = 0

		''' <summary>
		''' The baseline used in ideographic scripts like Chinese, Japanese,
		''' and Korean when laying out text.
		''' </summary>
		Public Const CENTER_BASELINE As Integer = 1

		''' <summary>
		''' The baseline used in Devanigiri and similar scripts when laying
		''' out text.
		''' </summary>
		Public Const HANGING_BASELINE As Integer = 2

		''' <summary>
		''' Identify a font resource of type TRUETYPE.
		''' Used to specify a TrueType font resource to the
		''' <seealso cref="#createFont"/> method.
		''' The TrueType format was extended to become the OpenType
		''' format, which adds support for fonts with Postscript outlines,
		''' this tag therefore references these fonts, as well as those
		''' with TrueType outlines.
		''' @since 1.3
		''' </summary>

		Public Const TRUETYPE_FONT As Integer = 0

		''' <summary>
		''' Identify a font resource of type TYPE1.
		''' Used to specify a Type1 font resource to the
		''' <seealso cref="#createFont"/> method.
		''' @since 1.5
		''' </summary>
		Public Const TYPE1_FONT As Integer = 1

		''' <summary>
		''' The logical name of this <code>Font</code>, as passed to the
		''' constructor.
		''' @since JDK1.0
		''' 
		''' @serial </summary>
		''' <seealso cref= #getName </seealso>
		Protected Friend name As String

		''' <summary>
		''' The style of this <code>Font</code>, as passed to the constructor.
		''' This style can be PLAIN, BOLD, ITALIC, or BOLD+ITALIC.
		''' @since JDK1.0
		''' 
		''' @serial </summary>
		''' <seealso cref= #getStyle() </seealso>
		Protected Friend style As Integer

		''' <summary>
		''' The point size of this <code>Font</code>, rounded to integer.
		''' @since JDK1.0
		''' 
		''' @serial </summary>
		''' <seealso cref= #getSize() </seealso>
		Protected Friend size As Integer

		''' <summary>
		''' The point size of this <code>Font</code> in <code>float</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getSize() </seealso>
		''' <seealso cref= #getSize2D() </seealso>
		Protected Friend pointSize As Single

		''' <summary>
		''' The platform specific font information.
		''' </summary>
		<NonSerialized> _
		Private peer As java.awt.peer.FontPeer
		<NonSerialized> _
		Private pData As Long ' native JDK1.1 font pointer
		<NonSerialized> _
		Private font2DHandle As sun.font.Font2DHandle

		<NonSerialized> _
		Private values As sun.font.AttributeValues
		<NonSerialized> _
		Private hasLayoutAttributes_Renamed As Boolean

	'    
	'     * If the origin of a Font is a created font then this attribute
	'     * must be set on all derived fonts too.
	'     
		<NonSerialized> _
		Private createdFont As Boolean = False

	'    
	'     * This is true if the font transform is not identity.  It
	'     * is used to avoid unnecessary instantiation of an AffineTransform.
	'     
		<NonSerialized> _
		Private nonIdentityTx As Boolean

	'    
	'     * A cached value used when a transform is required for internal
	'     * use.  This must not be exposed to callers since AffineTransform
	'     * is mutable.
	'     
		Private Shared ReadOnly identityTx As New java.awt.geom.AffineTransform

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -4206021311591459213L

		''' <summary>
		''' Gets the peer of this <code>Font</code>. </summary>
		''' <returns>  the peer of the <code>Font</code>.
		''' @since JDK1.1 </returns>
		''' @deprecated Font rendering is now platform independent. 
		<Obsolete("Font rendering is now platform independent.")> _
		Public Overridable Property peer As java.awt.peer.FontPeer
			Get
				Return peer_NoClientCode
			End Get
		End Property
		' NOTE: This method is called by privileged threads.
		'       We implement this functionality in a package-private method
		'       to insure that it cannot be overridden by client subclasses.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Property peer_NoClientCode As java.awt.peer.FontPeer
			Get
				If peer Is Nothing Then
					Dim tk As Toolkit = Toolkit.defaultToolkit
					Me.peer = tk.getFontPeer(name, style)
				End If
				Return peer
			End Get
		End Property

		''' <summary>
		''' Return the AttributeValues object associated with this
		''' font.  Most of the time, the internal object is null.
		''' If required, it will be created from the 'standard'
		''' state on the font.  Only non-default values will be
		''' set in the AttributeValues object.
		''' 
		''' <p>Since the AttributeValues object is mutable, and it
		''' is cached in the font, care must be taken to ensure that
		''' it is not mutated.
		''' </summary>
		Private Property attributeValues As sun.font.AttributeValues
			Get
				If values Is Nothing Then
					Dim valuesTmp As New sun.font.AttributeValues
					valuesTmp.family = name
					valuesTmp.size = pointSize ' expects the float value.
    
					If (style And BOLD) <> 0 Then valuesTmp.weight = 2 ' WEIGHT_BOLD
    
					If (style And ITALIC) <> 0 Then valuesTmp.posture = .2f ' POSTURE_OBLIQUE
					valuesTmp.defineAll(PRIMARY_MASK) ' for streaming compatibility
					values = valuesTmp
				End If
    
				Return values
			End Get
		End Property

		Private Property font2D As sun.font.Font2D
			Get
				Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
				If fm.usingPerAppContextComposites() AndAlso font2DHandle IsNot Nothing AndAlso TypeOf font2DHandle.font2D Is sun.font.CompositeFont AndAlso CType(font2DHandle.font2D, sun.font.CompositeFont).stdComposite Then
					Return fm.findFont2D(name, style, sun.font.FontManager.LOGICAL_FALLBACK)
				ElseIf font2DHandle Is Nothing Then
					font2DHandle = fm.findFont2D(name, style, sun.font.FontManager.LOGICAL_FALLBACK).handle
				End If
		'         Do not cache the de-referenced font2D. It must be explicitly
		'         * de-referenced to pick up a valid font in the event that the
		'         * original one is marked invalid
		'         
				Return font2DHandle.font2D
			End Get
		End Property

		''' <summary>
		''' Creates a new <code>Font</code> from the specified name, style and
		''' point size.
		''' <p>
		''' The font name can be a font face name or a font family name.
		''' It is used together with the style to find an appropriate font face.
		''' When a font family name is specified, the style argument is used to
		''' select the most appropriate face from the family. When a font face
		''' name is specified, the face's style and the style argument are
		''' merged to locate the best matching font from the same family.
		''' For example if face name "Arial Bold" is specified with style
		''' <code>Font.ITALIC</code>, the font system looks for a face in the
		''' "Arial" family that is bold and italic, and may associate the font
		''' instance with the physical font face "Arial Bold Italic".
		''' The style argument is merged with the specified face's style, not
		''' added or subtracted.
		''' This means, specifying a bold face and a bold style does not
		''' double-embolden the font, and specifying a bold face and a plain
		''' style does not lighten the font.
		''' <p>
		''' If no face for the requested style can be found, the font system
		''' may apply algorithmic styling to achieve the desired style.
		''' For example, if <code>ITALIC</code> is requested, but no italic
		''' face is available, glyphs from the plain face may be algorithmically
		''' obliqued (slanted).
		''' <p>
		''' Font name lookup is case insensitive, using the case folding
		''' rules of the US locale.
		''' <p>
		''' If the <code>name</code> parameter represents something other than a
		''' logical font, i.e. is interpreted as a physical font face or family, and
		''' this cannot be mapped by the implementation to a physical font or a
		''' compatible alternative, then the font system will map the Font
		''' instance to "Dialog", such that for example, the family as reported
		''' by <seealso cref="#getFamily() getFamily"/> will be "Dialog".
		''' <p>
		''' </summary>
		''' <param name="name"> the font name.  This can be a font face name or a font
		''' family name, and may represent either a logical font or a physical
		''' font found in this {@code GraphicsEnvironment}.
		''' The family names for logical fonts are: Dialog, DialogInput,
		''' Monospaced, Serif, or SansSerif. Pre-defined String constants exist
		''' for all of these names, for example, {@code DIALOG}. If {@code name} is
		''' {@code null}, the <em>logical font name</em> of the new
		''' {@code Font} as returned by {@code getName()} is set to
		''' the name "Default". </param>
		''' <param name="style"> the style constant for the {@code Font}
		''' The style argument is an integer bitmask that may
		''' be {@code PLAIN}, or a bitwise union of {@code BOLD} and/or
		''' {@code ITALIC} (for example, {@code ITALIC} or {@code BOLD|ITALIC}).
		''' If the style argument does not conform to one of the expected
		''' integer bitmasks then the style is set to {@code PLAIN}. </param>
		''' <param name="size"> the point size of the {@code Font} </param>
		''' <seealso cref= GraphicsEnvironment#getAllFonts </seealso>
		''' <seealso cref= GraphicsEnvironment#getAvailableFontFamilyNames
		''' @since JDK1.0 </seealso>
		Public Sub New(ByVal name As String, ByVal style As Integer, ByVal size As Integer)
			Me.name = If(name IsNot Nothing, name, "Default")
			Me.style = If((style And (Not &H3)) = 0, style, 0)
			Me.size = size
			Me.pointSize = size
		End Sub

		Private Sub New(ByVal name As String, ByVal style As Integer, ByVal sizePts As Single)
			Me.name = If(name IsNot Nothing, name, "Default")
			Me.style = If((style And (Not &H3)) = 0, style, 0)
			Me.size = CInt(Fix(sizePts + 0.5))
			Me.pointSize = sizePts
		End Sub

		' This constructor is used by deriveFont when attributes is null 
		Private Sub New(ByVal name As String, ByVal style As Integer, ByVal sizePts As Single, ByVal created As Boolean, ByVal handle As sun.font.Font2DHandle)
			Me.New(name, style, sizePts)
			Me.createdFont = created
	'         Fonts created from a stream will use the same font2D instance
	'         * as the parent.
	'         * One exception is that if the derived font is requested to be
	'         * in a different style, then also check if its a CompositeFont
	'         * and if so build a new CompositeFont from components of that style.
	'         * CompositeFonts can only be marked as "created" if they are used
	'         * to add fall backs to a physical font. And non-composites are
	'         * always from "Font.createFont()" and shouldn't get this treatment.
	'         
			If created Then
				If TypeOf handle.font2D Is sun.font.CompositeFont AndAlso handle.font2D.style <> style Then
					Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
					Me.font2DHandle = fm.getNewComposite(Nothing, style, handle)
				Else
					Me.font2DHandle = handle
				End If
			End If
		End Sub

		' used to implement Font.createFont 
		Private Sub New(ByVal fontFile As File, ByVal fontFormat As Integer, ByVal isCopy As Boolean, ByVal tracker As sun.font.CreatedFontTracker)
			Me.createdFont = True
	'         Font2D instances created by this method track their font file
	'         * so that when the Font2D is GC'd it can also remove the file.
	'         
			Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
			Me.font2DHandle = fm.createFont2D(fontFile, fontFormat, isCopy, tracker).handle
			Me.name = Me.font2DHandle.font2D.getFontName(java.util.Locale.default)
			Me.style = Font.PLAIN
			Me.size = 1
			Me.pointSize = 1f
		End Sub

	'     This constructor is used when one font is derived from another.
	'     * Fonts created from a stream will use the same font2D instance as the
	'     * parent. They can be distinguished because the "created" argument
	'     * will be "true". Since there is no way to recreate these fonts they
	'     * need to have the handle to the underlying font2D passed in.
	'     * "created" is also true when a special composite is referenced by the
	'     * handle for essentially the same reasons.
	'     * But when deriving a font in these cases two particular attributes
	'     * need special attention: family/face and style.
	'     * The "composites" in these cases need to be recreated with optimal
	'     * fonts for the new values of family and style.
	'     * For fonts created with createFont() these are treated differently.
	'     * JDK can often synthesise a different style (bold from plain
	'     * for example). For fonts created with "createFont" this is a reasonable
	'     * solution but its also possible (although rare) to derive a font with a
	'     * different family attribute. In this case JDK needs
	'     * to break the tie with the original Font2D and find a new Font.
	'     * The oldName and oldStyle are supplied so they can be compared with
	'     * what the Font2D and the values. To speed things along :
	'     * oldName == null will be interpreted as the name is unchanged.
	'     * oldStyle = -1 will be interpreted as the style is unchanged.
	'     * In these cases there is no need to interrogate "values".
	'     
		Private Sub New(ByVal values As sun.font.AttributeValues, ByVal oldName As String, ByVal oldStyle As Integer, ByVal created As Boolean, ByVal handle As sun.font.Font2DHandle)

			Me.createdFont = created
			If created Then
				Me.font2DHandle = handle

				Dim newName As String = Nothing
				If oldName IsNot Nothing Then
					newName = values.family
					If oldName.Equals(newName) Then newName = Nothing
				End If
				Dim newStyle As Integer = 0
				If oldStyle = -1 Then
					newStyle = -1
				Else
					If values.weight >= 2f Then newStyle = BOLD
					If values.posture >=.2f Then newStyle = newStyle Or ITALIC
					If oldStyle = newStyle Then newStyle = -1
				End If
				If TypeOf handle.font2D Is sun.font.CompositeFont Then
					If newStyle <> -1 OrElse newName IsNot Nothing Then
						Dim fm As sun.font.FontManager = sun.font.FontManagerFactory.instance
						Me.font2DHandle = fm.getNewComposite(newName, newStyle, handle)
					End If
				ElseIf newName IsNot Nothing Then
					Me.createdFont = False
					Me.font2DHandle = Nothing
				End If
			End If
			initFromValues(values)
		End Sub

		''' <summary>
		''' Creates a new <code>Font</code> with the specified attributes.
		''' Only keys defined in <seealso cref="java.awt.font.TextAttribute TextAttribute"/>
		''' are recognized.  In addition the FONT attribute is
		'''  not recognized by this constructor
		''' (see <seealso cref="#getAvailableAttributes"/>). Only attributes that have
		''' values of valid types will affect the new <code>Font</code>.
		''' <p>
		''' If <code>attributes</code> is <code>null</code>, a new
		''' <code>Font</code> is initialized with default values. </summary>
		''' <seealso cref= java.awt.font.TextAttribute </seealso>
		''' <param name="attributes"> the attributes to assign to the new
		'''          <code>Font</code>, or <code>null</code> </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal attributes As IDictionary(Of T1))
			initFromValues(sun.font.AttributeValues.fromMap(attributes, RECOGNIZED_MASK))
		End Sub

		''' <summary>
		''' Creates a new <code>Font</code> from the specified <code>font</code>.
		''' This constructor is intended for use by subclasses. </summary>
		''' <param name="font"> from which to create this <code>Font</code>. </param>
		''' <exception cref="NullPointerException"> if <code>font</code> is null
		''' @since 1.6 </exception>
		Protected Friend Sub New(ByVal font_Renamed As Font)
			If font_Renamed.values IsNot Nothing Then
				initFromValues(font_Renamed.attributeValues.clone())
			Else
				Me.name = font_Renamed.name
				Me.style = font_Renamed.style
				Me.size = font_Renamed.size
				Me.pointSize = font_Renamed.pointSize
			End If
			Me.font2DHandle = font_Renamed.font2DHandle
			Me.createdFont = font_Renamed.createdFont
		End Sub

		''' <summary>
		''' Font recognizes all attributes except FONT.
		''' </summary>
		Private Shared ReadOnly RECOGNIZED_MASK As Integer = sun.font.AttributeValues.MASK_ALL And Not sun.font.AttributeValues.getMask(EFONT)

		''' <summary>
		''' These attributes are considered primary by the FONT attribute.
		''' </summary>
		Private Shared ReadOnly PRIMARY_MASK As Integer = sun.font.AttributeValues.getMask(EFAMILY, EWEIGHT, EWIDTH, EPOSTURE, ESIZE, ETRANSFORM, ESUPERSCRIPT, ETRACKING)

		''' <summary>
		''' These attributes are considered secondary by the FONT attribute.
		''' </summary>
		Private Shared ReadOnly SECONDARY_MASK As Integer = RECOGNIZED_MASK And Not PRIMARY_MASK

		''' <summary>
		''' These attributes are handled by layout.
		''' </summary>
		Private Shared ReadOnly LAYOUT_MASK As Integer = sun.font.AttributeValues.getMask(ECHAR_REPLACEMENT, EFOREGROUND, EBACKGROUND, EUNDERLINE, ESTRIKETHROUGH, ERUN_DIRECTION, EBIDI_EMBEDDING, EJUSTIFICATION, EINPUT_METHOD_HIGHLIGHT, EINPUT_METHOD_UNDERLINE, ESWAP_COLORS, ENUMERIC_SHAPING, EKERNING, ELIGATURES, ETRACKING, ESUPERSCRIPT)

		Private Shared ReadOnly EXTRA_MASK As Integer = sun.font.AttributeValues.getMask(ETRANSFORM, ESUPERSCRIPT, EWIDTH)

		''' <summary>
		''' Initialize the standard Font fields from the values object.
		''' </summary>
		Private Sub initFromValues(ByVal values As sun.font.AttributeValues)
			Me.values = values
			values.defineAll(PRIMARY_MASK) ' for 1.5 streaming compatibility

			Me.name = values.family
			Me.pointSize = values.size
			Me.size = CInt(Fix(values.size + 0.5))
			If values.weight >= 2f Then ' not == 2f Me.style = Me.style Or BOLD
			If values.posture >=.2f Then ' not  == .2f Me.style = Me.style Or ITALIC

			Me.nonIdentityTx = values.anyNonDefault(EXTRA_MASK)
			Me.hasLayoutAttributes_Renamed = values.anyNonDefault(LAYOUT_MASK)
		End Sub

		''' <summary>
		''' Returns a <code>Font</code> appropriate to the attributes.
		''' If <code>attributes</code>contains a <code>FONT</code> attribute
		''' with a valid <code>Font</code> as its value, it will be
		''' merged with any remaining attributes.  See
		''' <seealso cref="java.awt.font.TextAttribute#FONT"/> for more
		''' information.
		''' </summary>
		''' <param name="attributes"> the attributes to assign to the new
		'''          <code>Font</code> </param>
		''' <returns> a new <code>Font</code> created with the specified
		'''          attributes </returns>
		''' <exception cref="NullPointerException"> if <code>attributes</code> is null.
		''' @since 1.2 </exception>
		''' <seealso cref= java.awt.font.TextAttribute </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function getFont(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal attributes As IDictionary(Of T1)) As Font
			' optimize for two cases:
			' 1) FONT attribute, and nothing else
			' 2) attributes, but no FONT

			' avoid turning the attributemap into a regular map for no reason
			If TypeOf attributes Is sun.font.AttributeMap AndAlso CType(attributes, sun.font.AttributeMap).values IsNot Nothing Then
				Dim values As sun.font.AttributeValues = CType(attributes, sun.font.AttributeMap).values
				If values.isNonDefault(EFONT) Then
					Dim font_Renamed As Font = values.font
					If Not values.anyDefined(SECONDARY_MASK) Then Return font_Renamed
					' merge
					values = font_Renamed.attributeValues.clone()
					values.merge(attributes, SECONDARY_MASK)
					Return New Font(values, font_Renamed.name, font_Renamed.style, font_Renamed.createdFont, font_Renamed.font2DHandle)
				End If
				Return New Font(attributes)
			End If

			Dim font_Renamed As Font = CType(attributes(java.awt.font.TextAttribute.FONT), Font)
			If font_Renamed IsNot Nothing Then
				If attributes.Count > 1 Then ' oh well, check for anything else
					Dim values As sun.font.AttributeValues = font_Renamed.attributeValues.clone()
					values.merge(attributes, SECONDARY_MASK)
					Return New Font(values, font_Renamed.name, font_Renamed.style, font_Renamed.createdFont, font_Renamed.font2DHandle)
				End If

				Return font_Renamed
			End If

			Return New Font(attributes)
		End Function

		''' <summary>
		''' Used with the byte count tracker for fonts created from streams.
		''' If a thread can create temp files anyway, no point in counting
		''' font bytes.
		''' </summary>
		Private Shared Function hasTempPermission() As Boolean

			If System.securityManager Is Nothing Then Return True
			Dim f As File = Nothing
			Dim hasPerm As Boolean = False
			Try
				f = java.nio.file.Files.createTempFile("+~JT", ".tmp").toFile()
				f.delete()
				f = Nothing
				hasPerm = True
			Catch t As Throwable
				' inc. any kind of SecurityException 
			End Try
			Return hasPerm
		End Function

		''' <summary>
		''' Returns a new <code>Font</code> using the specified font type
		''' and input data.  The new <code>Font</code> is
		''' created with a point size of 1 and style <seealso cref="#PLAIN PLAIN"/>.
		''' This base font can then be used with the <code>deriveFont</code>
		''' methods in this class to derive new <code>Font</code> objects with
		''' varying sizes, styles, transforms and font features.  This
		''' method does not close the <seealso cref="InputStream"/>.
		''' <p>
		''' To make the <code>Font</code> available to Font constructors the
		''' returned <code>Font</code> must be registered in the
		''' <code>GraphicsEnviroment</code> by calling
		''' <seealso cref="GraphicsEnvironment#registerFont(Font) registerFont(Font)"/>. </summary>
		''' <param name="fontFormat"> the type of the <code>Font</code>, which is
		''' <seealso cref="#TRUETYPE_FONT TRUETYPE_FONT"/> if a TrueType resource is specified.
		''' or <seealso cref="#TYPE1_FONT TYPE1_FONT"/> if a Type 1 resource is specified. </param>
		''' <param name="fontStream"> an <code>InputStream</code> object representing the
		''' input data for the font. </param>
		''' <returns> a new <code>Font</code> created with the specified font type. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>fontFormat</code> is not
		'''     <code>TRUETYPE_FONT</code>or<code>TYPE1_FONT</code>. </exception>
		''' <exception cref="FontFormatException"> if the <code>fontStream</code> data does
		'''     not contain the required font tables for the specified format. </exception>
		''' <exception cref="IOException"> if the <code>fontStream</code>
		'''     cannot be completely read. </exception>
		''' <seealso cref= GraphicsEnvironment#registerFont(Font)
		''' @since 1.3 </seealso>
		Public Shared Function createFont(ByVal fontFormat As Integer, ByVal fontStream As InputStream) As Font

			If hasTempPermission() Then Return createFont0(fontFormat, fontStream, Nothing)

			' Otherwise, be extra conscious of pending temp file creation and
			' resourcefully handle the temp file resources, among other things.
			Dim tracker As sun.font.CreatedFontTracker = sun.font.CreatedFontTracker.tracker
			Dim acquired As Boolean = False
			Try
				acquired = tracker.acquirePermit()
				If Not acquired Then Throw New IOException("Timed out waiting for resources.")
				Return createFont0(fontFormat, fontStream, tracker)
			Catch e As InterruptedException
				Throw New IOException("Problem reading font data.")
			Finally
				If acquired Then tracker.releasePermit()
			End Try
		End Function

		Private Shared Function createFont0(ByVal fontFormat As Integer, ByVal fontStream As InputStream, ByVal tracker As sun.font.CreatedFontTracker) As Font

			If fontFormat <> Font.TRUETYPE_FONT AndAlso fontFormat <> Font.TYPE1_FONT Then Throw New IllegalArgumentException("font format not recognized")
			Dim copiedFontData As Boolean = False
			Try
				Dim tFile As File = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			   )
				If tracker IsNot Nothing Then tracker.add(tFile)

				Dim totalSize As Integer = 0
				Try
					Dim outStream As OutputStream = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
					   )
					If tracker IsNot Nothing Then tracker.set(tFile, outStream)
					Try
						Dim buf As SByte() = New SByte(8191){}
						Do
							Dim bytesRead As Integer = fontStream.read(buf)
							If bytesRead < 0 Then Exit Do
							If tracker IsNot Nothing Then
								If totalSize+bytesRead > sun.font.CreatedFontTracker.MAX_FILE_SIZE Then Throw New IOException("File too big.")
								If totalSize+tracker.numBytes > sun.font.CreatedFontTracker.MAX_TOTAL_BYTES Then Throw New IOException("Total files too big.")
								totalSize += bytesRead
								tracker.addBytes(bytesRead)
							End If
							outStream.write(buf, 0, bytesRead)
						Loop
						' don't close the input stream 
					Finally
						outStream.close()
					End Try
	'                 After all references to a Font2D are dropped, the file
	'                 * will be removed. To support long-lived AppContexts,
	'                 * we need to then decrement the byte count by the size
	'                 * of the file.
	'                 * If the data isn't a valid font, the implementation will
	'                 * delete the tmp file and decrement the byte count
	'                 * in the tracker object before returning from the
	'                 * constructor, so we can set 'copiedFontData' to true here
	'                 * without waiting for the results of that constructor.
	'                 
					copiedFontData = True
					Dim font_Renamed As New Font(tFile, fontFormat, True, tracker)
					Return font_Renamed
				Finally
					If tracker IsNot Nothing Then tracker.remove(tFile)
					If Not copiedFontData Then
						If tracker IsNot Nothing Then tracker.subBytes(totalSize)
						java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper3(Of T)
					   )
					End If
				End Try
			Catch t As Throwable
				If TypeOf t Is FontFormatException Then Throw CType(t, FontFormatException)
				If TypeOf t Is IOException Then Throw CType(t, IOException)
				Dim cause As Throwable = t.cause
				If TypeOf cause Is FontFormatException Then Throw CType(cause, FontFormatException)
				Throw New IOException("Problem reading font data.")
			End Try
		End Function

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As File
				Return java.nio.file.Files.createTempFile("+~JF", ".tmp").toFile()
			End Function
		End Class

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As OutputStream
				Return New FileOutputStream(tFile)
			End Function
		End Class

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper3(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Void
				tFile.delete()
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Returns a new <code>Font</code> using the specified font type
		''' and the specified font file.  The new <code>Font</code> is
		''' created with a point size of 1 and style <seealso cref="#PLAIN PLAIN"/>.
		''' This base font can then be used with the <code>deriveFont</code>
		''' methods in this class to derive new <code>Font</code> objects with
		''' varying sizes, styles, transforms and font features. </summary>
		''' <param name="fontFormat"> the type of the <code>Font</code>, which is
		''' <seealso cref="#TRUETYPE_FONT TRUETYPE_FONT"/> if a TrueType resource is
		''' specified or <seealso cref="#TYPE1_FONT TYPE1_FONT"/> if a Type 1 resource is
		''' specified.
		''' So long as the returned font, or its derived fonts are referenced
		''' the implementation may continue to access <code>fontFile</code>
		''' to retrieve font data. Thus the results are undefined if the file
		''' is changed, or becomes inaccessible.
		''' <p>
		''' To make the <code>Font</code> available to Font constructors the
		''' returned <code>Font</code> must be registered in the
		''' <code>GraphicsEnviroment</code> by calling
		''' <seealso cref="GraphicsEnvironment#registerFont(Font) registerFont(Font)"/>. </param>
		''' <param name="fontFile"> a <code>File</code> object representing the
		''' input data for the font. </param>
		''' <returns> a new <code>Font</code> created with the specified font type. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>fontFormat</code> is not
		'''     <code>TRUETYPE_FONT</code>or<code>TYPE1_FONT</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>fontFile</code> is null. </exception>
		''' <exception cref="IOException"> if the <code>fontFile</code> cannot be read. </exception>
		''' <exception cref="FontFormatException"> if <code>fontFile</code> does
		'''     not contain the required font tables for the specified format. </exception>
		''' <exception cref="SecurityException"> if the executing code does not have
		''' permission to read from the file. </exception>
		''' <seealso cref= GraphicsEnvironment#registerFont(Font)
		''' @since 1.5 </seealso>
		Public Shared Function createFont(ByVal fontFormat As Integer, ByVal fontFile As File) As Font

			fontFile = New File(fontFile.path)

			If fontFormat <> Font.TRUETYPE_FONT AndAlso fontFormat <> Font.TYPE1_FONT Then Throw New IllegalArgumentException("font format not recognized")
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Dim filePermission As New FilePermission(fontFile.path, "read")
				sm.checkPermission(filePermission)
			End If
			If Not fontFile.canRead() Then Throw New IOException("Can't read " & fontFile)
			Return New Font(fontFile, fontFormat, False, Nothing)
		End Function

		''' <summary>
		''' Returns a copy of the transform associated with this
		''' <code>Font</code>.  This transform is not necessarily the one
		''' used to construct the font.  If the font has algorithmic
		''' superscripting or width adjustment, this will be incorporated
		''' into the returned <code>AffineTransform</code>.
		''' <p>
		''' Typically, fonts will not be transformed.  Clients generally
		''' should call <seealso cref="#isTransformed"/> first, and only call this
		''' method if <code>isTransformed</code> returns true.
		''' </summary>
		''' <returns> an <seealso cref="AffineTransform"/> object representing the
		'''          transform attribute of this <code>Font</code> object. </returns>
		Public Overridable Property transform As java.awt.geom.AffineTransform
			Get
		'         The most common case is the identity transform.  Most callers
		'         * should call isTransformed() first, to decide if they need to
		'         * get the transform, but some may not.  Here we check to see
		'         * if we have a nonidentity transform, and only do the work to
		'         * fetch and/or compute it if so, otherwise we return a new
		'         * identity transform.
		'         *
		'         * Note that the transform is _not_ necessarily the same as
		'         * the transform passed in as an Attribute in a Map, as the
		'         * transform returned will also reflect the effects of WIDTH and
		'         * SUPERSCRIPT attributes.  Clients who want the actual transform
		'         * need to call getRequestedAttributes.
		'         
				If nonIdentityTx Then
					Dim values As sun.font.AttributeValues = attributeValues
    
					Dim at As java.awt.geom.AffineTransform = If(values.isNonDefault(ETRANSFORM), New java.awt.geom.AffineTransform(values.transform), New java.awt.geom.AffineTransform)
    
					If values.superscript <> 0 Then
						' can't get ascent and descent here, recursive call to this fn,
						' so use pointsize
						' let users combine super- and sub-scripting
    
						Dim superscript As Integer = values.superscript
    
						Dim trans As Double = 0
						Dim n As Integer = 0
						Dim up As Boolean = superscript > 0
						Dim sign As Integer = If(up, -1, 1)
						Dim ss As Integer = If(up, superscript, -superscript)
    
						Do While (ss And 7) > n
							Dim newn As Integer = ss And 7
							trans += sign * (ssinfo(newn) - ssinfo(n))
							ss >>= 3
							sign = -sign
							n = newn
						Loop
						trans *= pointSize
						Dim scale As Double = Math.Pow(2.0/3.0, n)
    
						at.preConcatenate(java.awt.geom.AffineTransform.getTranslateInstance(0, trans))
						at.scale(scale, scale)
    
						' note on placement and italics
						' We preconcatenate the transform because we don't want to translate along
						' the italic angle, but purely perpendicular to the baseline.  While this
						' looks ok for superscripts, it can lead subscripts to stack on each other
						' and bring the following text too close.  The way we deal with potential
						' collisions that can occur in the case of italics is by adjusting the
						' horizontal spacing of the adjacent glyphvectors.  Examine the italic
						' angle of both vectors, if one is non-zero, compute the minimum ascent
						' and descent, and then the x position at each for each vector along its
						' italic angle starting from its (offset) baseline.  Compute the difference
						' between the x positions and use the maximum difference to adjust the
						' position of the right gv.
					End If
    
					If values.isNonDefault(EWIDTH) Then at.scale(values.width, 1f)
    
					Return at
				End If
    
				Return New java.awt.geom.AffineTransform
			End Get
		End Property

		' x = r^0 + r^1 + r^2... r^n
		' rx = r^1 + r^2 + r^3... r^(n+1)
		' x - rx = r^0 - r^(n+1)
		' x (1 - r) = r^0 - r^(n+1)
		' x = (r^0 - r^(n+1)) / (1 - r)
		' x = (1 - r^(n+1)) / (1 - r)

		' scale ratio is 2/3
		' trans = 1/2 of ascent * x
		' assume ascent is 3/4 of point size

		Private Shared ReadOnly ssinfo As Single() = { 0.0f, 0.375f, 0.625f, 0.7916667f, 0.9027778f, 0.9768519f, 1.0262346f, 1.0591564f }

		''' <summary>
		''' Returns the family name of this <code>Font</code>.
		''' 
		''' <p>The family name of a font is font specific. Two fonts such as
		''' Helvetica Italic and Helvetica Bold have the same family name,
		''' <i>Helvetica</i>, whereas their font face names are
		''' <i>Helvetica Bold</i> and <i>Helvetica Italic</i>. The list of
		''' available family names may be obtained by using the
		''' <seealso cref="GraphicsEnvironment#getAvailableFontFamilyNames()"/> method.
		''' 
		''' <p>Use <code>getName</code> to get the logical name of the font.
		''' Use <code>getFontName</code> to get the font face name of the font. </summary>
		''' <returns> a <code>String</code> that is the family name of this
		'''          <code>Font</code>.
		''' </returns>
		''' <seealso cref= #getName </seealso>
		''' <seealso cref= #getFontName
		''' @since JDK1.1 </seealso>
		Public Overridable Property family As String
			Get
				Return family_NoClientCode
			End Get
		End Property
		' NOTE: This method is called by privileged threads.
		'       We implement this functionality in a package-private
		'       method to insure that it cannot be overridden by client
		'       subclasses.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Friend Property family_NoClientCode As String
			Get
				Return getFamily(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Returns the family name of this <code>Font</code>, localized for
		''' the specified locale.
		''' 
		''' <p>The family name of a font is font specific. Two fonts such as
		''' Helvetica Italic and Helvetica Bold have the same family name,
		''' <i>Helvetica</i>, whereas their font face names are
		''' <i>Helvetica Bold</i> and <i>Helvetica Italic</i>. The list of
		''' available family names may be obtained by using the
		''' <seealso cref="GraphicsEnvironment#getAvailableFontFamilyNames()"/> method.
		''' 
		''' <p>Use <code>getFontName</code> to get the font face name of the font. </summary>
		''' <param name="l"> locale for which to get the family name </param>
		''' <returns> a <code>String</code> representing the family name of the
		'''          font, localized for the specified locale. </returns>
		''' <seealso cref= #getFontName </seealso>
		''' <seealso cref= java.util.Locale
		''' @since 1.2 </seealso>
		Public Overridable Function getFamily(ByVal l As java.util.Locale) As String
			If l Is Nothing Then Throw New NullPointerException("null locale doesn't mean default")
			Return font2D.getFamilyName(l)
		End Function

		''' <summary>
		''' Returns the postscript name of this <code>Font</code>.
		''' Use <code>getFamily</code> to get the family name of the font.
		''' Use <code>getFontName</code> to get the font face name of the font. </summary>
		''' <returns> a <code>String</code> representing the postscript name of
		'''          this <code>Font</code>.
		''' @since 1.2 </returns>
		Public Overridable Property pSName As String
			Get
				Return font2D.postscriptName
			End Get
		End Property

		''' <summary>
		''' Returns the logical name of this <code>Font</code>.
		''' Use <code>getFamily</code> to get the family name of the font.
		''' Use <code>getFontName</code> to get the font face name of the font. </summary>
		''' <returns> a <code>String</code> representing the logical name of
		'''          this <code>Font</code>. </returns>
		''' <seealso cref= #getFamily </seealso>
		''' <seealso cref= #getFontName
		''' @since JDK1.0 </seealso>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the font face name of this <code>Font</code>.  For example,
		''' Helvetica Bold could be returned as a font face name.
		''' Use <code>getFamily</code> to get the family name of the font.
		''' Use <code>getName</code> to get the logical name of the font. </summary>
		''' <returns> a <code>String</code> representing the font face name of
		'''          this <code>Font</code>. </returns>
		''' <seealso cref= #getFamily </seealso>
		''' <seealso cref= #getName
		''' @since 1.2 </seealso>
		Public Overridable Property fontName As String
			Get
			  Return getFontName(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Returns the font face name of the <code>Font</code>, localized
		''' for the specified locale. For example, Helvetica Fett could be
		''' returned as the font face name.
		''' Use <code>getFamily</code> to get the family name of the font. </summary>
		''' <param name="l"> a locale for which to get the font face name </param>
		''' <returns> a <code>String</code> representing the font face name,
		'''          localized for the specified locale. </returns>
		''' <seealso cref= #getFamily </seealso>
		''' <seealso cref= java.util.Locale </seealso>
		Public Overridable Function getFontName(ByVal l As java.util.Locale) As String
			If l Is Nothing Then Throw New NullPointerException("null locale doesn't mean default")
			Return font2D.getFontName(l)
		End Function

		''' <summary>
		''' Returns the style of this <code>Font</code>.  The style can be
		''' PLAIN, BOLD, ITALIC, or BOLD+ITALIC. </summary>
		''' <returns> the style of this <code>Font</code> </returns>
		''' <seealso cref= #isPlain </seealso>
		''' <seealso cref= #isBold </seealso>
		''' <seealso cref= #isItalic
		''' @since JDK1.0 </seealso>
		Public Overridable Property style As Integer
			Get
				Return style
			End Get
		End Property

		''' <summary>
		''' Returns the point size of this <code>Font</code>, rounded to
		''' an integer.
		''' Most users are familiar with the idea of using <i>point size</i> to
		''' specify the size of glyphs in a font. This point size defines a
		''' measurement between the baseline of one line to the baseline of the
		''' following line in a single spaced text document. The point size is
		''' based on <i>typographic points</i>, approximately 1/72 of an inch.
		''' <p>
		''' The Java(tm)2D API adopts the convention that one point is
		''' equivalent to one unit in user coordinates.  When using a
		''' normalized transform for converting user space coordinates to
		''' device space coordinates 72 user
		''' space units equal 1 inch in device space.  In this case one point
		''' is 1/72 of an inch. </summary>
		''' <returns> the point size of this <code>Font</code> in 1/72 of an
		'''          inch units. </returns>
		''' <seealso cref= #getSize2D </seealso>
		''' <seealso cref= GraphicsConfiguration#getDefaultTransform </seealso>
		''' <seealso cref= GraphicsConfiguration#getNormalizingTransform
		''' @since JDK1.0 </seealso>
		Public Overridable Property size As Integer
			Get
				Return size
			End Get
		End Property

		''' <summary>
		''' Returns the point size of this <code>Font</code> in
		''' <code>float</code> value. </summary>
		''' <returns> the point size of this <code>Font</code> as a
		''' <code>float</code> value. </returns>
		''' <seealso cref= #getSize
		''' @since 1.2 </seealso>
		Public Overridable Property size2D As Single
			Get
				Return pointSize
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not this <code>Font</code> object's style is
		''' PLAIN. </summary>
		''' <returns>    <code>true</code> if this <code>Font</code> has a
		'''            PLAIN style;
		'''            <code>false</code> otherwise. </returns>
		''' <seealso cref=       java.awt.Font#getStyle
		''' @since     JDK1.0 </seealso>
		Public Overridable Property plain As Boolean
			Get
				Return style = 0
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not this <code>Font</code> object's style is
		''' BOLD. </summary>
		''' <returns>    <code>true</code> if this <code>Font</code> object's
		'''            style is BOLD;
		'''            <code>false</code> otherwise. </returns>
		''' <seealso cref=       java.awt.Font#getStyle
		''' @since     JDK1.0 </seealso>
		Public Overridable Property bold As Boolean
			Get
				Return (style And BOLD) <> 0
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not this <code>Font</code> object's style is
		''' ITALIC. </summary>
		''' <returns>    <code>true</code> if this <code>Font</code> object's
		'''            style is ITALIC;
		'''            <code>false</code> otherwise. </returns>
		''' <seealso cref=       java.awt.Font#getStyle
		''' @since     JDK1.0 </seealso>
		Public Overridable Property italic As Boolean
			Get
				Return (style And ITALIC) <> 0
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not this <code>Font</code> object has a
		''' transform that affects its size in addition to the Size
		''' attribute. </summary>
		''' <returns>  <code>true</code> if this <code>Font</code> object
		'''          has a non-identity AffineTransform attribute.
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.awt.Font#getTransform
		''' @since   1.4 </seealso>
		Public Overridable Property transformed As Boolean
			Get
				Return nonIdentityTx
			End Get
		End Property

		''' <summary>
		''' Return true if this Font contains attributes that require extra
		''' layout processing. </summary>
		''' <returns> true if the font has layout attributes
		''' @since 1.6 </returns>
		Public Overridable Function hasLayoutAttributes() As Boolean
			Return hasLayoutAttributes_Renamed
		End Function

		''' <summary>
		''' Returns a <code>Font</code> object from the system properties list.
		''' <code>nm</code> is treated as the name of a system property to be
		''' obtained.  The <code>String</code> value of this property is then
		''' interpreted as a <code>Font</code> object according to the
		''' specification of <code>Font.decode(String)</code>
		''' If the specified property is not found, or the executing code does
		''' not have permission to read the property, null is returned instead.
		''' </summary>
		''' <param name="nm"> the property name </param>
		''' <returns> a <code>Font</code> object that the property name
		'''          describes, or null if no such property exists. </returns>
		''' <exception cref="NullPointerException"> if nm is null.
		''' @since 1.2 </exception>
		''' <seealso cref= #decode(String) </seealso>
		Public Shared Function getFont(ByVal nm As String) As Font
			Return getFont(nm, Nothing)
		End Function

		''' <summary>
		''' Returns the <code>Font</code> that the <code>str</code>
		''' argument describes.
		''' To ensure that this method returns the desired Font,
		''' format the <code>str</code> parameter in
		''' one of these ways
		''' 
		''' <ul>
		''' <li><em>fontname-style-pointsize</em>
		''' <li><em>fontname-pointsize</em>
		''' <li><em>fontname-style</em>
		''' <li><em>fontname</em>
		''' <li><em>fontname style pointsize</em>
		''' <li><em>fontname pointsize</em>
		''' <li><em>fontname style</em>
		''' <li><em>fontname</em>
		''' </ul>
		''' in which <i>style</i> is one of the four
		''' case-insensitive strings:
		''' <code>"PLAIN"</code>, <code>"BOLD"</code>, <code>"BOLDITALIC"</code>, or
		''' <code>"ITALIC"</code>, and pointsize is a positive decimal integer
		''' representation of the point size.
		''' For example, if you want a font that is Arial, bold, with
		''' a point size of 18, you would call this method with:
		''' "Arial-BOLD-18".
		''' This is equivalent to calling the Font constructor :
		''' <code>new Font("Arial", Font.BOLD, 18);</code>
		''' and the values are interpreted as specified by that constructor.
		''' <p>
		''' A valid trailing decimal field is always interpreted as the pointsize.
		''' Therefore a fontname containing a trailing decimal value should not
		''' be used in the fontname only form.
		''' <p>
		''' If a style name field is not one of the valid style strings, it is
		''' interpreted as part of the font name, and the default style is used.
		''' <p>
		''' Only one of ' ' or '-' may be used to separate fields in the input.
		''' The identified separator is the one closest to the end of the string
		''' which separates a valid pointsize, or a valid style name from
		''' the rest of the string.
		''' Null (empty) pointsize and style fields are treated
		''' as valid fields with the default value for that field.
		''' <p>
		''' Some font names may include the separator characters ' ' or '-'.
		''' If <code>str</code> is not formed with 3 components, e.g. such that
		''' <code>style</code> or <code>pointsize</code> fields are not present in
		''' <code>str</code>, and <code>fontname</code> also contains a
		''' character determined to be the separator character
		''' then these characters where they appear as intended to be part of
		''' <code>fontname</code> may instead be interpreted as separators
		''' so the font name may not be properly recognised.
		''' 
		''' <p>
		''' The default size is 12 and the default style is PLAIN.
		''' If <code>str</code> does not specify a valid size, the returned
		''' <code>Font</code> has a size of 12.  If <code>str</code> does not
		''' specify a valid style, the returned Font has a style of PLAIN.
		''' If you do not specify a valid font name in
		''' the <code>str</code> argument, this method will return
		''' a font with the family name "Dialog".
		''' To determine what font family names are available on
		''' your system, use the
		''' <seealso cref="GraphicsEnvironment#getAvailableFontFamilyNames()"/> method.
		''' If <code>str</code> is <code>null</code>, a new <code>Font</code>
		''' is returned with the family name "Dialog", a size of 12 and a
		''' PLAIN style. </summary>
		''' <param name="str"> the name of the font, or <code>null</code> </param>
		''' <returns> the <code>Font</code> object that <code>str</code>
		'''          describes, or a new default <code>Font</code> if
		'''          <code>str</code> is <code>null</code>. </returns>
		''' <seealso cref= #getFamily
		''' @since JDK1.1 </seealso>
		Public Shared Function decode(ByVal str As String) As Font
			Dim fontName_Renamed As String = str
			Dim styleName As String = ""
			Dim fontSize As Integer = 12
			Dim fontStyle As Integer = Font.PLAIN

			If str Is Nothing Then Return New Font(DIALOG, fontStyle, fontSize)

			Dim lastHyphen As Integer = str.LastIndexOf("-"c)
			Dim lastSpace As Integer = str.LastIndexOf(" "c)
			Dim sepChar As Char = If(lastHyphen > lastSpace, "-"c, " "c)
			Dim sizeIndex As Integer = str.LastIndexOf(sepChar)
			Dim styleIndex As Integer = str.LastIndexOf(sepChar, sizeIndex-1)
			Dim strlen As Integer = str.length()

			If sizeIndex > 0 AndAlso sizeIndex+1 < strlen Then
				Try
					fontSize = Convert.ToInt32(str.Substring(sizeIndex+1))
					If fontSize <= 0 Then fontSize = 12
				Catch e As NumberFormatException
	'                 It wasn't a valid size, if we didn't also find the
	'                 * start of the style string perhaps this is the style 
					styleIndex = sizeIndex
					sizeIndex = strlen
					If str.Chars(sizeIndex-1) = sepChar Then sizeIndex -= 1
				End Try
			End If

			If styleIndex >= 0 AndAlso styleIndex+1 < strlen Then
				styleName = str.Substring(styleIndex+1, sizeIndex - (styleIndex+1))
				styleName = styleName.ToLower(java.util.Locale.ENGLISH)
				If styleName.Equals("bolditalic") Then
					fontStyle = Font.BOLD Or Font.ITALIC
				ElseIf styleName.Equals("italic") Then
					fontStyle = Font.ITALIC
				ElseIf styleName.Equals("bold") Then
					fontStyle = Font.BOLD
				ElseIf styleName.Equals("plain") Then
					fontStyle = Font.PLAIN
				Else
	'                 this string isn't any of the expected styles, so
	'                 * assume its part of the font name
	'                 
					styleIndex = sizeIndex
					If str.Chars(styleIndex-1) = sepChar Then styleIndex -= 1
				End If
				fontName_Renamed = str.Substring(0, styleIndex)

			Else
				Dim fontEnd As Integer = strlen
				If styleIndex > 0 Then
					fontEnd = styleIndex
				ElseIf sizeIndex > 0 Then
					fontEnd = sizeIndex
				End If
				If fontEnd > 0 AndAlso str.Chars(fontEnd-1) = sepChar Then fontEnd -= 1
				fontName_Renamed = str.Substring(0, fontEnd)
			End If

			Return New Font(fontName_Renamed, fontStyle, fontSize)
		End Function

		''' <summary>
		''' Gets the specified <code>Font</code> from the system properties
		''' list.  As in the <code>getProperty</code> method of
		''' <code>System</code>, the first
		''' argument is treated as the name of a system property to be
		''' obtained.  The <code>String</code> value of this property is then
		''' interpreted as a <code>Font</code> object.
		''' <p>
		''' The property value should be one of the forms accepted by
		''' <code>Font.decode(String)</code>
		''' If the specified property is not found, or the executing code does not
		''' have permission to read the property, the <code>font</code>
		''' argument is returned instead. </summary>
		''' <param name="nm"> the case-insensitive property name </param>
		''' <param name="font"> a default <code>Font</code> to return if property
		'''          <code>nm</code> is not defined </param>
		''' <returns>    the <code>Font</code> value of the property. </returns>
		''' <exception cref="NullPointerException"> if nm is null. </exception>
		''' <seealso cref= #decode(String) </seealso>
		Public Shared Function getFont(ByVal nm As String, ByVal font_Renamed As Font) As Font
			Dim str As String = Nothing
			Try
				str =System.getProperty(nm)
			Catch e As SecurityException
			End Try
			If str Is Nothing Then Return font_Renamed
			Return decode(str)
		End Function

		<NonSerialized> _
		Friend hash As Integer
		''' <summary>
		''' Returns a hashcode for this <code>Font</code>. </summary>
		''' <returns>     a hashcode value for this <code>Font</code>.
		''' @since      JDK1.0 </returns>
		Public Overrides Function GetHashCode() As Integer
			If hash = 0 Then
				hash = name.GetHashCode() Xor style Xor size
	'             It is possible many fonts differ only in transform.
	'             * So include the transform in the hash calculation.
	'             * nonIdentityTx is set whenever there is a transform in
	'             * 'values'. The tests for null are required because it can
	'             * also be set for other reasons.
	'             
				If nonIdentityTx AndAlso values IsNot Nothing AndAlso values.transform IsNot Nothing Then hash = hash Xor values.transform.GetHashCode()
			End If
			Return hash
		End Function

		''' <summary>
		''' Compares this <code>Font</code> object to the specified
		''' <code>Object</code>. </summary>
		''' <param name="obj"> the <code>Object</code> to compare </param>
		''' <returns> <code>true</code> if the objects are the same
		'''          or if the argument is a <code>Font</code> object
		'''          describing the same font as this object;
		'''          <code>false</code> otherwise.
		''' @since JDK1.0 </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If obj IsNot Nothing Then
				Try
					Dim font_Renamed As Font = CType(obj, Font)
					If size = font_Renamed.size AndAlso style = font_Renamed.style AndAlso nonIdentityTx = font_Renamed.nonIdentityTx AndAlso hasLayoutAttributes_Renamed = font_Renamed.hasLayoutAttributes_Renamed AndAlso pointSize = font_Renamed.pointSize AndAlso name.Equals(font_Renamed.name) Then

	'                     'values' is usually initialized lazily, except when
	'                     * the font is constructed from a Map, or derived using
	'                     * a Map or other values. So if only one font has
	'                     * the field initialized we need to initialize it in
	'                     * the other instance and compare.
	'                     
						If values Is Nothing Then
							If font_Renamed.values Is Nothing Then
								Return True
							Else
								Return attributeValues.Equals(font_Renamed.values)
							End If
						Else
							Return values.Equals(font_Renamed.attributeValues)
						End If
					End If
				Catch e As ClassCastException
				End Try
			End If
			Return False
		End Function

		''' <summary>
		''' Converts this <code>Font</code> object to a <code>String</code>
		''' representation. </summary>
		''' <returns>     a <code>String</code> representation of this
		'''          <code>Font</code> object.
		''' @since      JDK1.0 </returns>
		' NOTE: This method may be called by privileged threads.
		'       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		Public Overrides Function ToString() As String
			Dim strStyle As String

			If bold Then
				strStyle = If(italic, "bolditalic", "bold")
			Else
				strStyle = If(italic, "italic", "plain")
			End If

			Return Me.GetType().name & "[family=" & family & ",name=" & name & ",style=" & strStyle & ",size=" & size & "]"
		End Function ' toString()


		''' <summary>
		''' Serialization support.  A <code>readObject</code>
		'''  method is neccessary because the constructor creates
		'''  the font's peer, and we can't serialize the peer.
		'''  Similarly the computed font "family" may be different
		'''  at <code>readObject</code> time than at
		'''  <code>writeObject</code> time.  An integer version is
		'''  written so that future versions of this class will be
		'''  able to recognize serialized output from this one.
		''' </summary>
		''' <summary>
		''' The <code>Font</code> Serializable Data Form.
		''' 
		''' @serial
		''' </summary>
		Private fontSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to a stream.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= #readObject(java.io.ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			If values IsNot Nothing Then
			  SyncLock values
				' transient
				fRequestedAttributes = values.toSerializableHashtable()
				s.defaultWriteObject()
				fRequestedAttributes = Nothing
			  End SyncLock
			Else
			  s.defaultWriteObject()
			End If
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code>.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read
		''' @serial </param>
		''' <seealso cref= #writeObject(java.io.ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If pointSize = 0 Then pointSize = CSng(size)

			' Handle fRequestedAttributes.
			' in 1.5, we always streamed out the font values plus
			' TRANSFORM, SUPERSCRIPT, and WIDTH, regardless of whether the
			' values were default or not.  In 1.6 we only stream out
			' defined values.  So, 1.6 streams in from a 1.5 stream,
			' it check each of these values and 'undefines' it if the
			' value is the default.

			If fRequestedAttributes IsNot Nothing Then
				values = attributeValues ' init
				Dim extras As sun.font.AttributeValues = sun.font.AttributeValues.fromSerializableHashtable(fRequestedAttributes)
				If Not sun.font.AttributeValues.is16Hashtable(fRequestedAttributes) Then extras.unsetDefault() ' if legacy stream, undefine these
				values = attributeValues.merge(extras)
				Me.nonIdentityTx = values.anyNonDefault(EXTRA_MASK)
				Me.hasLayoutAttributes_Renamed = values.anyNonDefault(LAYOUT_MASK)

				fRequestedAttributes = Nothing ' don't need it any more
			End If
		End Sub

		''' <summary>
		''' Returns the number of glyphs in this <code>Font</code>. Glyph codes
		''' for this <code>Font</code> range from 0 to
		''' <code>getNumGlyphs()</code> - 1. </summary>
		''' <returns> the number of glyphs in this <code>Font</code>.
		''' @since 1.2 </returns>
		Public Overridable Property numGlyphs As Integer
			Get
				Return font2D.numGlyphs
			End Get
		End Property

		''' <summary>
		''' Returns the glyphCode which is used when this <code>Font</code>
		''' does not have a glyph for a specified unicode code point. </summary>
		''' <returns> the glyphCode of this <code>Font</code>.
		''' @since 1.2 </returns>
		Public Overridable Property missingGlyphCode As Integer
			Get
				Return font2D.missingGlyphCode
			End Get
		End Property

		''' <summary>
		''' Returns the baseline appropriate for displaying this character.
		''' <p>
		''' Large fonts can support different writing systems, and each system can
		''' use a different baseline.
		''' The character argument determines the writing system to use. Clients
		''' should not assume all characters use the same baseline.
		''' </summary>
		''' <param name="c"> a character used to identify the writing system </param>
		''' <returns> the baseline appropriate for the specified character. </returns>
		''' <seealso cref= LineMetrics#getBaselineOffsets </seealso>
		''' <seealso cref= #ROMAN_BASELINE </seealso>
		''' <seealso cref= #CENTER_BASELINE </seealso>
		''' <seealso cref= #HANGING_BASELINE
		''' @since 1.2 </seealso>
		Public Overridable Function getBaselineFor(ByVal c As Char) As SByte
			Return font2D.getBaselineFor(c)
		End Function

		''' <summary>
		''' Returns a map of font attributes available in this
		''' <code>Font</code>.  Attributes include things like ligatures and
		''' glyph substitution. </summary>
		''' <returns> the attributes map of this <code>Font</code>. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property attributes As IDictionary(Of java.awt.font.TextAttribute, ?)
			Get
				Return New sun.font.AttributeMap(attributeValues)
			End Get
		End Property

		''' <summary>
		''' Returns the keys of all the attributes supported by this
		''' <code>Font</code>.  These attributes can be used to derive other
		''' fonts. </summary>
		''' <returns> an array containing the keys of all the attributes
		'''          supported by this <code>Font</code>.
		''' @since 1.2 </returns>
		Public Overridable Property availableAttributes As java.text.AttributedCharacterIterator.Attribute()
			Get
				' FONT is not supported by Font
    
				Dim attributes_Renamed As java.text.AttributedCharacterIterator.Attribute() = { java.awt.font.TextAttribute.FAMILY, java.awt.font.TextAttribute.WEIGHT, java.awt.font.TextAttribute.WIDTH, java.awt.font.TextAttribute.POSTURE, java.awt.font.TextAttribute.SIZE, java.awt.font.TextAttribute.TRANSFORM, java.awt.font.TextAttribute.SUPERSCRIPT, java.awt.font.TextAttribute.CHAR_REPLACEMENT, java.awt.font.TextAttribute.FOREGROUND, java.awt.font.TextAttribute.BACKGROUND, java.awt.font.TextAttribute.UNDERLINE, java.awt.font.TextAttribute.STRIKETHROUGH, java.awt.font.TextAttribute.RUN_DIRECTION, java.awt.font.TextAttribute.BIDI_EMBEDDING, java.awt.font.TextAttribute.JUSTIFICATION, java.awt.font.TextAttribute.INPUT_METHOD_HIGHLIGHT, java.awt.font.TextAttribute.INPUT_METHOD_UNDERLINE, java.awt.font.TextAttribute.SWAP_COLORS, java.awt.font.TextAttribute.NUMERIC_SHAPING, java.awt.font.TextAttribute.KERNING, java.awt.font.TextAttribute.LIGATURES, java.awt.font.TextAttribute.TRACKING }
    
				Return attributes_Renamed
			End Get
		End Property

		''' <summary>
		''' Creates a new <code>Font</code> object by replicating this
		''' <code>Font</code> object and applying a new style and size. </summary>
		''' <param name="style"> the style for the new <code>Font</code> </param>
		''' <param name="size"> the size for the new <code>Font</code> </param>
		''' <returns> a new <code>Font</code> object.
		''' @since 1.2 </returns>
		Public Overridable Function deriveFont(ByVal style As Integer, ByVal size As Single) As Font
			If values Is Nothing Then Return New Font(name, style, size, createdFont, font2DHandle)
			Dim newValues As sun.font.AttributeValues = attributeValues.clone()
			Dim oldStyle As Integer = If(Me.style <> style, Me.style, -1)
			applyStyle(style, newValues)
			newValues.size = size
			Return New Font(newValues, Nothing, oldStyle, createdFont, font2DHandle)
		End Function

		''' <summary>
		''' Creates a new <code>Font</code> object by replicating this
		''' <code>Font</code> object and applying a new style and transform. </summary>
		''' <param name="style"> the style for the new <code>Font</code> </param>
		''' <param name="trans"> the <code>AffineTransform</code> associated with the
		''' new <code>Font</code> </param>
		''' <returns> a new <code>Font</code> object. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>trans</code> is
		'''         <code>null</code>
		''' @since 1.2 </exception>
		Public Overridable Function deriveFont(ByVal style As Integer, ByVal trans As java.awt.geom.AffineTransform) As Font
			Dim newValues As sun.font.AttributeValues = attributeValues.clone()
			Dim oldStyle As Integer = If(Me.style <> style, Me.style, -1)
			applyStyle(style, newValues)
			applyTransform(trans, newValues)
			Return New Font(newValues, Nothing, oldStyle, createdFont, font2DHandle)
		End Function

		''' <summary>
		''' Creates a new <code>Font</code> object by replicating the current
		''' <code>Font</code> object and applying a new size to it. </summary>
		''' <param name="size"> the size for the new <code>Font</code>. </param>
		''' <returns> a new <code>Font</code> object.
		''' @since 1.2 </returns>
		Public Overridable Function deriveFont(ByVal size As Single) As Font
			If values Is Nothing Then Return New Font(name, style, size, createdFont, font2DHandle)
			Dim newValues As sun.font.AttributeValues = attributeValues.clone()
			newValues.size = size
			Return New Font(newValues, Nothing, -1, createdFont, font2DHandle)
		End Function

		''' <summary>
		''' Creates a new <code>Font</code> object by replicating the current
		''' <code>Font</code> object and applying a new transform to it. </summary>
		''' <param name="trans"> the <code>AffineTransform</code> associated with the
		''' new <code>Font</code> </param>
		''' <returns> a new <code>Font</code> object. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>trans</code> is
		'''         <code>null</code>
		''' @since 1.2 </exception>
		Public Overridable Function deriveFont(ByVal trans As java.awt.geom.AffineTransform) As Font
			Dim newValues As sun.font.AttributeValues = attributeValues.clone()
			applyTransform(trans, newValues)
			Return New Font(newValues, Nothing, -1, createdFont, font2DHandle)
		End Function

		''' <summary>
		''' Creates a new <code>Font</code> object by replicating the current
		''' <code>Font</code> object and applying a new style to it. </summary>
		''' <param name="style"> the style for the new <code>Font</code> </param>
		''' <returns> a new <code>Font</code> object.
		''' @since 1.2 </returns>
		Public Overridable Function deriveFont(ByVal style As Integer) As Font
			If values Is Nothing Then Return New Font(name, style, size, createdFont, font2DHandle)
			Dim newValues As sun.font.AttributeValues = attributeValues.clone()
			Dim oldStyle As Integer = If(Me.style <> style, Me.style, -1)
			applyStyle(style, newValues)
			Return New Font(newValues, Nothing, oldStyle, createdFont, font2DHandle)
		End Function

		''' <summary>
		''' Creates a new <code>Font</code> object by replicating the current
		''' <code>Font</code> object and applying a new set of font attributes
		''' to it.
		''' </summary>
		''' <param name="attributes"> a map of attributes enabled for the new
		''' <code>Font</code> </param>
		''' <returns> a new <code>Font</code> object.
		''' @since 1.2 </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function deriveFont(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal attributes As IDictionary(Of T1)) As Font
			If attributes Is Nothing Then Return Me
			Dim newValues As sun.font.AttributeValues = attributeValues.clone()
			newValues.merge(attributes, RECOGNIZED_MASK)

			Return New Font(newValues, name, style, createdFont, font2DHandle)
		End Function

		''' <summary>
		''' Checks if this <code>Font</code> has a glyph for the specified
		''' character.
		''' 
		''' <p> <b>Note:</b> This method cannot handle <a
		''' href="../../java/lang/Character.html#supplementary"> supplementary
		''' characters</a>. To support all Unicode characters, including
		''' supplementary characters, use the <seealso cref="#canDisplay(int)"/>
		''' method or <code>canDisplayUpTo</code> methods.
		''' </summary>
		''' <param name="c"> the character for which a glyph is needed </param>
		''' <returns> <code>true</code> if this <code>Font</code> has a glyph for this
		'''          character; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overridable Function canDisplay(ByVal c As Char) As Boolean
			Return font2D.canDisplay(c)
		End Function

		''' <summary>
		''' Checks if this <code>Font</code> has a glyph for the specified
		''' character.
		''' </summary>
		''' <param name="codePoint"> the character (Unicode code point) for which a glyph
		'''        is needed. </param>
		''' <returns> <code>true</code> if this <code>Font</code> has a glyph for the
		'''          character; <code>false</code> otherwise. </returns>
		''' <exception cref="IllegalArgumentException"> if the code point is not a valid Unicode
		'''          code point. </exception>
		''' <seealso cref= Character#isValidCodePoint(int)
		''' @since 1.5 </seealso>
		Public Overridable Function canDisplay(ByVal codePoint As Integer) As Boolean
			If Not Character.isValidCodePoint(codePoint) Then Throw New IllegalArgumentException("invalid code point: " & Integer.toHexString(codePoint))
			Return font2D.canDisplay(codePoint)
		End Function

		''' <summary>
		''' Indicates whether or not this <code>Font</code> can display a
		''' specified <code>String</code>.  For strings with Unicode encoding,
		''' it is important to know if a particular font can display the
		''' string. This method returns an offset into the <code>String</code>
		''' <code>str</code> which is the first character this
		''' <code>Font</code> cannot display without using the missing glyph
		''' code. If the <code>Font</code> can display all characters, -1 is
		''' returned. </summary>
		''' <param name="str"> a <code>String</code> object </param>
		''' <returns> an offset into <code>str</code> that points
		'''          to the first character in <code>str</code> that this
		'''          <code>Font</code> cannot display; or <code>-1</code> if
		'''          this <code>Font</code> can display all characters in
		'''          <code>str</code>.
		''' @since 1.2 </returns>
		Public Overridable Function canDisplayUpTo(ByVal str As String) As Integer
			Dim font2d_Renamed As sun.font.Font2D = font2D
			Dim len As Integer = str.length()
			For i As Integer = 0 To len - 1
				Dim c As Char = str.Chars(i)
				If font2d_Renamed.canDisplay(c) Then Continue For
				If Not Char.IsHighSurrogate(c) Then Return i
				If Not font2d_Renamed.canDisplay(str.codePointAt(i)) Then Return i
				i += 1
			Next i
			Return -1
		End Function

		''' <summary>
		''' Indicates whether or not this <code>Font</code> can display
		''' the characters in the specified <code>text</code>
		''' starting at <code>start</code> and ending at
		''' <code>limit</code>.  This method is a convenience overload. </summary>
		''' <param name="text"> the specified array of <code>char</code> values </param>
		''' <param name="start"> the specified starting offset (in
		'''              <code>char</code>s) into the specified array of
		'''              <code>char</code> values </param>
		''' <param name="limit"> the specified ending offset (in
		'''              <code>char</code>s) into the specified array of
		'''              <code>char</code> values </param>
		''' <returns> an offset into <code>text</code> that points
		'''          to the first character in <code>text</code> that this
		'''          <code>Font</code> cannot display; or <code>-1</code> if
		'''          this <code>Font</code> can display all characters in
		'''          <code>text</code>.
		''' @since 1.2 </returns>
		Public Overridable Function canDisplayUpTo(ByVal text As Char(), ByVal start As Integer, ByVal limit As Integer) As Integer
			Dim font2d_Renamed As sun.font.Font2D = font2D
			For i As Integer = start To limit - 1
				Dim c As Char = text(i)
				If font2d_Renamed.canDisplay(c) Then Continue For
				If Not Char.IsHighSurrogate(c) Then Return i
				If Not font2d_Renamed.canDisplay(Character.codePointAt(text, i, limit)) Then Return i
				i += 1
			Next i
			Return -1
		End Function

		''' <summary>
		''' Indicates whether or not this <code>Font</code> can display the
		''' text specified by the <code>iter</code> starting at
		''' <code>start</code> and ending at <code>limit</code>.
		''' </summary>
		''' <param name="iter">  a <seealso cref="CharacterIterator"/> object </param>
		''' <param name="start"> the specified starting offset into the specified
		'''              <code>CharacterIterator</code>. </param>
		''' <param name="limit"> the specified ending offset into the specified
		'''              <code>CharacterIterator</code>. </param>
		''' <returns> an offset into <code>iter</code> that points
		'''          to the first character in <code>iter</code> that this
		'''          <code>Font</code> cannot display; or <code>-1</code> if
		'''          this <code>Font</code> can display all characters in
		'''          <code>iter</code>.
		''' @since 1.2 </returns>
		Public Overridable Function canDisplayUpTo(ByVal iter As java.text.CharacterIterator, ByVal start As Integer, ByVal limit As Integer) As Integer
			Dim font2d_Renamed As sun.font.Font2D = font2D
			Dim c As Char = iter.indexdex(start)
			Dim i As Integer = start
			Do While i < limit
				If font2d_Renamed.canDisplay(c) Then
					i += 1
				c = iter.next()
					Continue Do
				End If
				If Not Char.IsHighSurrogate(c) Then Return i
				Dim c2 As Char = iter.next()
				' c2 could be CharacterIterator.DONE which is not a low surrogate.
				If Not Char.IsLowSurrogate(c2) Then Return i
				If Not font2d_Renamed.canDisplay(Character.toCodePoint(c, c2)) Then Return i
				i += 1
				i += 1
				c = iter.next()
			Loop
			Return -1
		End Function

		''' <summary>
		''' Returns the italic angle of this <code>Font</code>.  The italic angle
		''' is the inverse slope of the caret which best matches the posture of this
		''' <code>Font</code>. </summary>
		''' <seealso cref= TextAttribute#POSTURE </seealso>
		''' <returns> the angle of the ITALIC style of this <code>Font</code>. </returns>
		Public Overridable Property italicAngle As Single
			Get
				Return getItalicAngle(Nothing)
			End Get
		End Property

	'     The FRC hints don't affect the value of the italic angle but
	'     * we need to pass them in to look up a strike.
	'     * If we can pass in ones already being used it can prevent an extra
	'     * strike from being allocated. Note that since italic angle is
	'     * a property of the font, the font transform is needed not the
	'     * device transform. Finally, this is private but the only caller of this
	'     * in the JDK - and the only likely caller - is in this same class.
	'     
		Private Function getItalicAngle(ByVal frc As java.awt.font.FontRenderContext) As Single
			Dim aa, fm As Object
			If frc Is Nothing Then
				aa = RenderingHints.VALUE_TEXT_ANTIALIAS_OFF
				fm = RenderingHints.VALUE_FRACTIONALMETRICS_OFF
			Else
				aa = frc.antiAliasingHint
				fm = frc.fractionalMetricsHint
			End If
			Return font2D.getItalicAngle(Me, identityTx, aa, fm)
		End Function

		''' <summary>
		''' Checks whether or not this <code>Font</code> has uniform
		''' line metrics.  A logical <code>Font</code> might be a
		''' composite font, which means that it is composed of different
		''' physical fonts to cover different code ranges.  Each of these
		''' fonts might have different <code>LineMetrics</code>.  If the
		''' logical <code>Font</code> is a single
		''' font then the metrics would be uniform. </summary>
		''' <returns> <code>true</code> if this <code>Font</code> has
		''' uniform line metrics; <code>false</code> otherwise. </returns>
		Public Overridable Function hasUniformLineMetrics() As Boolean
			Return False ' REMIND always safe, but prevents caller optimize
		End Function

		<NonSerialized> _
		Private flmref As SoftReference(Of sun.font.FontLineMetrics)
		Private Function defaultLineMetrics(ByVal frc As java.awt.font.FontRenderContext) As sun.font.FontLineMetrics
			Dim flm As sun.font.FontLineMetrics = Nothing
			flm = flmref.get()
			If flmref Is Nothing OrElse flm Is Nothing OrElse (Not flm.frc.Equals(frc)) Then

	'             The device transform in the frc is not used in obtaining line
	'             * metrics, although it probably should be: REMIND find why not?
	'             * The font transform is used but its applied in getFontMetrics, so
	'             * just pass identity here
	'             
				Dim metrics As Single() = New Single(7){}
				font2D.getFontMetrics(Me, identityTx, frc.antiAliasingHint, frc.fractionalMetricsHint, metrics)
				Dim ascent As Single = metrics(0)
				Dim descent As Single = metrics(1)
				Dim leading As Single = metrics(2)
				Dim ssOffset As Single = 0
				If values IsNot Nothing AndAlso values.superscript <> 0 Then
					ssOffset = CSng(transform.translateY)
					ascent -= ssOffset
					descent += ssOffset
				End If
				Dim height As Single = ascent + descent + leading

				Dim baselineIndex As Integer = 0 ' need real index, assumes roman for everything
				' need real baselines eventually
				Dim baselineOffsets As Single() = { 0, (descent/2f - ascent) / 2f, -ascent }

				Dim strikethroughOffset As Single = metrics(4)
				Dim strikethroughThickness As Single = metrics(5)

				Dim underlineOffset As Single = metrics(6)
				Dim underlineThickness As Single = metrics(7)

				Dim italicAngle_Renamed As Single = getItalicAngle(frc)

				If transformed Then
					Dim ctx As java.awt.geom.AffineTransform = values.charTransform ' extract rotation
					If ctx IsNot Nothing Then
						Dim pt As New java.awt.geom.Point2D.Float
						pt.locationion(0, strikethroughOffset)
						ctx.deltaTransform(pt, pt)
						strikethroughOffset = pt.y
						pt.locationion(0, strikethroughThickness)
						ctx.deltaTransform(pt, pt)
						strikethroughThickness = pt.y
						pt.locationion(0, underlineOffset)
						ctx.deltaTransform(pt, pt)
						underlineOffset = pt.y
						pt.locationion(0, underlineThickness)
						ctx.deltaTransform(pt, pt)
						underlineThickness = pt.y
					End If
				End If
				strikethroughOffset += ssOffset
				underlineOffset += ssOffset

				Dim cm As New sun.font.CoreMetrics(ascent, descent, leading, height, baselineIndex, baselineOffsets, strikethroughOffset, strikethroughThickness, underlineOffset, underlineThickness, ssOffset, italicAngle_Renamed)

				flm = New sun.font.FontLineMetrics(0, cm, frc)
				flmref = New SoftReference(Of sun.font.FontLineMetrics)(flm)
			End If

			Return CType(flm.clone(), sun.font.FontLineMetrics)
		End Function

		''' <summary>
		''' Returns a <seealso cref="LineMetrics"/> object created with the specified
		''' <code>String</code> and <seealso cref="FontRenderContext"/>. </summary>
		''' <param name="str"> the specified <code>String</code> </param>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <returns> a <code>LineMetrics</code> object created with the
		''' specified <code>String</code> and <seealso cref="FontRenderContext"/>. </returns>
		Public Overridable Function getLineMetrics(ByVal str As String, ByVal frc As java.awt.font.FontRenderContext) As java.awt.font.LineMetrics
			Dim flm As sun.font.FontLineMetrics = defaultLineMetrics(frc)
			flm.numchars = str.length()
			Return flm
		End Function

		''' <summary>
		''' Returns a <code>LineMetrics</code> object created with the
		''' specified arguments. </summary>
		''' <param name="str"> the specified <code>String</code> </param>
		''' <param name="beginIndex"> the initial offset of <code>str</code> </param>
		''' <param name="limit"> the end offset of <code>str</code> </param>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <returns> a <code>LineMetrics</code> object created with the
		''' specified arguments. </returns>
		Public Overridable Function getLineMetrics(ByVal str As String, ByVal beginIndex As Integer, ByVal limit As Integer, ByVal frc As java.awt.font.FontRenderContext) As java.awt.font.LineMetrics
			Dim flm As sun.font.FontLineMetrics = defaultLineMetrics(frc)
			Dim numChars As Integer = limit - beginIndex
			flm.numchars = If(numChars < 0, 0, numChars)
			Return flm
		End Function

		''' <summary>
		''' Returns a <code>LineMetrics</code> object created with the
		''' specified arguments. </summary>
		''' <param name="chars"> an array of characters </param>
		''' <param name="beginIndex"> the initial offset of <code>chars</code> </param>
		''' <param name="limit"> the end offset of <code>chars</code> </param>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <returns> a <code>LineMetrics</code> object created with the
		''' specified arguments. </returns>
		Public Overridable Function getLineMetrics(ByVal chars As Char (), ByVal beginIndex As Integer, ByVal limit As Integer, ByVal frc As java.awt.font.FontRenderContext) As java.awt.font.LineMetrics
			Dim flm As sun.font.FontLineMetrics = defaultLineMetrics(frc)
			Dim numChars As Integer = limit - beginIndex
			flm.numchars = If(numChars < 0, 0, numChars)
			Return flm
		End Function

		''' <summary>
		''' Returns a <code>LineMetrics</code> object created with the
		''' specified arguments. </summary>
		''' <param name="ci"> the specified <code>CharacterIterator</code> </param>
		''' <param name="beginIndex"> the initial offset in <code>ci</code> </param>
		''' <param name="limit"> the end offset of <code>ci</code> </param>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <returns> a <code>LineMetrics</code> object created with the
		''' specified arguments. </returns>
		Public Overridable Function getLineMetrics(ByVal ci As java.text.CharacterIterator, ByVal beginIndex As Integer, ByVal limit As Integer, ByVal frc As java.awt.font.FontRenderContext) As java.awt.font.LineMetrics
			Dim flm As sun.font.FontLineMetrics = defaultLineMetrics(frc)
			Dim numChars As Integer = limit - beginIndex
			flm.numchars = If(numChars < 0, 0, numChars)
			Return flm
		End Function

		''' <summary>
		''' Returns the logical bounds of the specified <code>String</code> in
		''' the specified <code>FontRenderContext</code>.  The logical bounds
		''' contains the origin, ascent, advance, and height, which includes
		''' the leading.  The logical bounds does not always enclose all the
		''' text.  For example, in some languages and in some fonts, accent
		''' marks can be positioned above the ascent or below the descent.
		''' To obtain a visual bounding box, which encloses all the text,
		''' use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
		''' <code>TextLayout</code>.
		''' <p>Note: The returned bounds is in baseline-relative coordinates
		''' (see <seealso cref="java.awt.Font class notes"/>). </summary>
		''' <param name="str"> the specified <code>String</code> </param>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <returns> a <seealso cref="Rectangle2D"/> that is the bounding box of the
		''' specified <code>String</code> in the specified
		''' <code>FontRenderContext</code>. </returns>
		''' <seealso cref= FontRenderContext </seealso>
		''' <seealso cref= Font#createGlyphVector
		''' @since 1.2 </seealso>
		Public Overridable Function getStringBounds(ByVal str As String, ByVal frc As java.awt.font.FontRenderContext) As java.awt.geom.Rectangle2D
			Dim array As Char() = str.ToCharArray()
			Return getStringBounds(array, 0, array.Length, frc)
		End Function

	   ''' <summary>
	   ''' Returns the logical bounds of the specified <code>String</code> in
	   ''' the specified <code>FontRenderContext</code>.  The logical bounds
	   ''' contains the origin, ascent, advance, and height, which includes
	   ''' the leading.  The logical bounds does not always enclose all the
	   ''' text.  For example, in some languages and in some fonts, accent
	   ''' marks can be positioned above the ascent or below the descent.
	   ''' To obtain a visual bounding box, which encloses all the text,
	   ''' use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
	   ''' <code>TextLayout</code>.
	   ''' <p>Note: The returned bounds is in baseline-relative coordinates
	   ''' (see <seealso cref="java.awt.Font class notes"/>). </summary>
	   ''' <param name="str"> the specified <code>String</code> </param>
	   ''' <param name="beginIndex"> the initial offset of <code>str</code> </param>
	   ''' <param name="limit"> the end offset of <code>str</code> </param>
	   ''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
	   ''' <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   ''' specified <code>String</code> in the specified
	   ''' <code>FontRenderContext</code>. </returns>
	   ''' <exception cref="IndexOutOfBoundsException"> if <code>beginIndex</code> is
	   '''         less than zero, or <code>limit</code> is greater than the
	   '''         length of <code>str</code>, or <code>beginIndex</code>
	   '''         is greater than <code>limit</code>. </exception>
	   ''' <seealso cref= FontRenderContext </seealso>
	   ''' <seealso cref= Font#createGlyphVector
	   ''' @since 1.2 </seealso>
		Public Overridable Function getStringBounds(ByVal str As String, ByVal beginIndex As Integer, ByVal limit As Integer, ByVal frc As java.awt.font.FontRenderContext) As java.awt.geom.Rectangle2D
			Dim substr As String = str.Substring(beginIndex, limit - beginIndex)
			Return getStringBounds(substr, frc)
		End Function

	   ''' <summary>
	   ''' Returns the logical bounds of the specified array of characters
	   ''' in the specified <code>FontRenderContext</code>.  The logical
	   ''' bounds contains the origin, ascent, advance, and height, which
	   ''' includes the leading.  The logical bounds does not always enclose
	   ''' all the text.  For example, in some languages and in some fonts,
	   ''' accent marks can be positioned above the ascent or below the
	   ''' descent.  To obtain a visual bounding box, which encloses all the
	   ''' text, use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
	   ''' <code>TextLayout</code>.
	   ''' <p>Note: The returned bounds is in baseline-relative coordinates
	   ''' (see <seealso cref="java.awt.Font class notes"/>). </summary>
	   ''' <param name="chars"> an array of characters </param>
	   ''' <param name="beginIndex"> the initial offset in the array of
	   ''' characters </param>
	   ''' <param name="limit"> the end offset in the array of characters </param>
	   ''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
	   ''' <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   ''' specified array of characters in the specified
	   ''' <code>FontRenderContext</code>. </returns>
	   ''' <exception cref="IndexOutOfBoundsException"> if <code>beginIndex</code> is
	   '''         less than zero, or <code>limit</code> is greater than the
	   '''         length of <code>chars</code>, or <code>beginIndex</code>
	   '''         is greater than <code>limit</code>. </exception>
	   ''' <seealso cref= FontRenderContext </seealso>
	   ''' <seealso cref= Font#createGlyphVector
	   ''' @since 1.2 </seealso>
		Public Overridable Function getStringBounds(ByVal chars As Char (), ByVal beginIndex As Integer, ByVal limit As Integer, ByVal frc As java.awt.font.FontRenderContext) As java.awt.geom.Rectangle2D
			If beginIndex < 0 Then Throw New IndexOutOfBoundsException("beginIndex: " & beginIndex)
			If limit > chars.Length Then Throw New IndexOutOfBoundsException("limit: " & limit)
			If beginIndex > limit Then Throw New IndexOutOfBoundsException("range length: " & (limit - beginIndex))

			' this code should be in textlayout
			' quick check for simple text, assume GV ok to use if simple

			Dim simple As Boolean = values Is Nothing OrElse (values.kerning = 0 AndAlso values.ligatures = 0 AndAlso values.baselineTransform Is Nothing)
			If simple Then simple = Not sun.font.FontUtilities.isComplexText(chars, beginIndex, limit)

			If simple Then
				Dim gv As java.awt.font.GlyphVector = New sun.font.StandardGlyphVector(Me, chars, beginIndex, limit - beginIndex, frc)
				Return gv.logicalBounds
			Else
				' need char array constructor on textlayout
				Dim str As New String(chars, beginIndex, limit - beginIndex)
				Dim tl As New java.awt.font.TextLayout(str, Me, frc)
				Return New java.awt.geom.Rectangle2D.Float(0, -tl.ascent, tl.advance, tl.ascent + tl.descent + tl.leading)
			End If
		End Function

	   ''' <summary>
	   ''' Returns the logical bounds of the characters indexed in the
	   ''' specified <seealso cref="CharacterIterator"/> in the
	   ''' specified <code>FontRenderContext</code>.  The logical bounds
	   ''' contains the origin, ascent, advance, and height, which includes
	   ''' the leading.  The logical bounds does not always enclose all the
	   ''' text.  For example, in some languages and in some fonts, accent
	   ''' marks can be positioned above the ascent or below the descent.
	   ''' To obtain a visual bounding box, which encloses all the text,
	   ''' use the <seealso cref="TextLayout#getBounds() getBounds"/> method of
	   ''' <code>TextLayout</code>.
	   ''' <p>Note: The returned bounds is in baseline-relative coordinates
	   ''' (see <seealso cref="java.awt.Font class notes"/>). </summary>
	   ''' <param name="ci"> the specified <code>CharacterIterator</code> </param>
	   ''' <param name="beginIndex"> the initial offset in <code>ci</code> </param>
	   ''' <param name="limit"> the end offset in <code>ci</code> </param>
	   ''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
	   ''' <returns> a <code>Rectangle2D</code> that is the bounding box of the
	   ''' characters indexed in the specified <code>CharacterIterator</code>
	   ''' in the specified <code>FontRenderContext</code>. </returns>
	   ''' <seealso cref= FontRenderContext </seealso>
	   ''' <seealso cref= Font#createGlyphVector
	   ''' @since 1.2 </seealso>
	   ''' <exception cref="IndexOutOfBoundsException"> if <code>beginIndex</code> is
	   '''         less than the start index of <code>ci</code>, or
	   '''         <code>limit</code> is greater than the end index of
	   '''         <code>ci</code>, or <code>beginIndex</code> is greater
	   '''         than <code>limit</code> </exception>
		Public Overridable Function getStringBounds(ByVal ci As java.text.CharacterIterator, ByVal beginIndex As Integer, ByVal limit As Integer, ByVal frc As java.awt.font.FontRenderContext) As java.awt.geom.Rectangle2D
			Dim start As Integer = ci.beginIndex
			Dim [end] As Integer = ci.endIndex

			If beginIndex < start Then Throw New IndexOutOfBoundsException("beginIndex: " & beginIndex)
			If limit > [end] Then Throw New IndexOutOfBoundsException("limit: " & limit)
			If beginIndex > limit Then Throw New IndexOutOfBoundsException("range length: " & (limit - beginIndex))

			Dim arr As Char() = New Char(limit - beginIndex - 1){}

			ci.index = beginIndex
			For idx As Integer = 0 To arr.Length - 1
				arr(idx) = ci.current()
				ci.next()
			Next idx

			Return getStringBounds(arr,0,arr.Length,frc)
		End Function

		''' <summary>
		''' Returns the bounds for the character with the maximum
		''' bounds as defined in the specified <code>FontRenderContext</code>.
		''' <p>Note: The returned bounds is in baseline-relative coordinates
		''' (see <seealso cref="java.awt.Font class notes"/>). </summary>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <returns> a <code>Rectangle2D</code> that is the bounding box
		''' for the character with the maximum bounds. </returns>
		Public Overridable Function getMaxCharBounds(ByVal frc As java.awt.font.FontRenderContext) As java.awt.geom.Rectangle2D
			Dim metrics As Single() = New Single(3){}

			font2D.getFontMetrics(Me, frc, metrics)

			Return New java.awt.geom.Rectangle2D.Float(0, -metrics(0), metrics(3), metrics(0) + metrics(1) + metrics(2))
		End Function

		''' <summary>
		''' Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		''' mapping characters to glyphs one-to-one based on the
		''' Unicode cmap in this <code>Font</code>.  This method does no other
		''' processing besides the mapping of glyphs to characters.  This
		''' means that this method is not useful for some scripts, such
		''' as Arabic, Hebrew, Thai, and Indic, that require reordering,
		''' shaping, or ligature substitution. </summary>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <param name="str"> the specified <code>String</code> </param>
		''' <returns> a new <code>GlyphVector</code> created with the
		''' specified <code>String</code> and the specified
		''' <code>FontRenderContext</code>. </returns>
		Public Overridable Function createGlyphVector(ByVal frc As java.awt.font.FontRenderContext, ByVal str As String) As java.awt.font.GlyphVector
			Return CType(New sun.font.StandardGlyphVector(Me, str, frc), java.awt.font.GlyphVector)
		End Function

		''' <summary>
		''' Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		''' mapping characters to glyphs one-to-one based on the
		''' Unicode cmap in this <code>Font</code>.  This method does no other
		''' processing besides the mapping of glyphs to characters.  This
		''' means that this method is not useful for some scripts, such
		''' as Arabic, Hebrew, Thai, and Indic, that require reordering,
		''' shaping, or ligature substitution. </summary>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <param name="chars"> the specified array of characters </param>
		''' <returns> a new <code>GlyphVector</code> created with the
		''' specified array of characters and the specified
		''' <code>FontRenderContext</code>. </returns>
		Public Overridable Function createGlyphVector(ByVal frc As java.awt.font.FontRenderContext, ByVal chars As Char()) As java.awt.font.GlyphVector
			Return CType(New sun.font.StandardGlyphVector(Me, chars, frc), java.awt.font.GlyphVector)
		End Function

		''' <summary>
		''' Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		''' mapping the specified characters to glyphs one-to-one based on the
		''' Unicode cmap in this <code>Font</code>.  This method does no other
		''' processing besides the mapping of glyphs to characters.  This
		''' means that this method is not useful for some scripts, such
		''' as Arabic, Hebrew, Thai, and Indic, that require reordering,
		''' shaping, or ligature substitution. </summary>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <param name="ci"> the specified <code>CharacterIterator</code> </param>
		''' <returns> a new <code>GlyphVector</code> created with the
		''' specified <code>CharacterIterator</code> and the specified
		''' <code>FontRenderContext</code>. </returns>
		Public Overridable Function createGlyphVector(ByVal frc As java.awt.font.FontRenderContext, ByVal ci As java.text.CharacterIterator) As java.awt.font.GlyphVector
			Return CType(New sun.font.StandardGlyphVector(Me, ci, frc), java.awt.font.GlyphVector)
		End Function

		''' <summary>
		''' Creates a <seealso cref="java.awt.font.GlyphVector GlyphVector"/> by
		''' mapping characters to glyphs one-to-one based on the
		''' Unicode cmap in this <code>Font</code>.  This method does no other
		''' processing besides the mapping of glyphs to characters.  This
		''' means that this method is not useful for some scripts, such
		''' as Arabic, Hebrew, Thai, and Indic, that require reordering,
		''' shaping, or ligature substitution. </summary>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <param name="glyphCodes"> the specified integer array </param>
		''' <returns> a new <code>GlyphVector</code> created with the
		''' specified integer array and the specified
		''' <code>FontRenderContext</code>. </returns>
		Public Overridable Function createGlyphVector(ByVal frc As java.awt.font.FontRenderContext, ByVal glyphCodes As Integer ()) As java.awt.font.GlyphVector
			Return CType(New sun.font.StandardGlyphVector(Me, glyphCodes, frc), java.awt.font.GlyphVector)
		End Function

		''' <summary>
		''' Returns a new <code>GlyphVector</code> object, performing full
		''' layout of the text if possible.  Full layout is required for
		''' complex text, such as Arabic or Hindi.  Support for different
		''' scripts depends on the font and implementation.
		''' <p>
		''' Layout requires bidi analysis, as performed by
		''' <code>Bidi</code>, and should only be performed on text that
		''' has a uniform direction.  The direction is indicated in the
		''' flags parameter,by using LAYOUT_RIGHT_TO_LEFT to indicate a
		''' right-to-left (Arabic and Hebrew) run direction, or
		''' LAYOUT_LEFT_TO_RIGHT to indicate a left-to-right (English)
		''' run direction.
		''' <p>
		''' In addition, some operations, such as Arabic shaping, require
		''' context, so that the characters at the start and limit can have
		''' the proper shapes.  Sometimes the data in the buffer outside
		''' the provided range does not have valid data.  The values
		''' LAYOUT_NO_START_CONTEXT and LAYOUT_NO_LIMIT_CONTEXT can be
		''' added to the flags parameter to indicate that the text before
		''' start, or after limit, respectively, should not be examined
		''' for context.
		''' <p>
		''' All other values for the flags parameter are reserved.
		''' </summary>
		''' <param name="frc"> the specified <code>FontRenderContext</code> </param>
		''' <param name="text"> the text to layout </param>
		''' <param name="start"> the start of the text to use for the <code>GlyphVector</code> </param>
		''' <param name="limit"> the limit of the text to use for the <code>GlyphVector</code> </param>
		''' <param name="flags"> control flags as described above </param>
		''' <returns> a new <code>GlyphVector</code> representing the text between
		''' start and limit, with glyphs chosen and positioned so as to best represent
		''' the text </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if start or limit is
		''' out of bounds </exception>
		''' <seealso cref= java.text.Bidi </seealso>
		''' <seealso cref= #LAYOUT_LEFT_TO_RIGHT </seealso>
		''' <seealso cref= #LAYOUT_RIGHT_TO_LEFT </seealso>
		''' <seealso cref= #LAYOUT_NO_START_CONTEXT </seealso>
		''' <seealso cref= #LAYOUT_NO_LIMIT_CONTEXT
		''' @since 1.4 </seealso>
		Public Overridable Function layoutGlyphVector(ByVal frc As java.awt.font.FontRenderContext, ByVal text As Char(), ByVal start As Integer, ByVal limit As Integer, ByVal flags As Integer) As java.awt.font.GlyphVector

			Dim gl As sun.font.GlyphLayout = sun.font.GlyphLayout.get(Nothing) ' !!! no custom layout engines
			Dim gv As sun.font.StandardGlyphVector = gl.layout(Me, frc, text, start, limit-start, flags, Nothing)
			sun.font.GlyphLayout.done(gl)
			Return gv
		End Function

		''' <summary>
		''' A flag to layoutGlyphVector indicating that text is left-to-right as
		''' determined by Bidi analysis.
		''' </summary>
		Public Const LAYOUT_LEFT_TO_RIGHT As Integer = 0

		''' <summary>
		''' A flag to layoutGlyphVector indicating that text is right-to-left as
		''' determined by Bidi analysis.
		''' </summary>
		Public Const LAYOUT_RIGHT_TO_LEFT As Integer = 1

		''' <summary>
		''' A flag to layoutGlyphVector indicating that text in the char array
		''' before the indicated start should not be examined.
		''' </summary>
		Public Const LAYOUT_NO_START_CONTEXT As Integer = 2

		''' <summary>
		''' A flag to layoutGlyphVector indicating that text in the char array
		''' after the indicated limit should not be examined.
		''' </summary>
		Public Const LAYOUT_NO_LIMIT_CONTEXT As Integer = 4


		Private Shared Sub applyTransform(ByVal trans As java.awt.geom.AffineTransform, ByVal values As sun.font.AttributeValues)
			If trans Is Nothing Then Throw New IllegalArgumentException("transform must not be null")
			values.transform = trans
		End Sub

		Private Shared Sub applyStyle(ByVal style As Integer, ByVal values As sun.font.AttributeValues)
			' WEIGHT_BOLD, WEIGHT_REGULAR
			values.weight = If((style And BOLD) <> 0, 2f, 1f)
			' POSTURE_OBLIQUE, POSTURE_REGULAR
			values.posture = If((style And ITALIC) <> 0, .2f, 0f)
		End Sub

	'    
	'     * Initialize JNI field and method IDs
	'     
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
	End Class

End Namespace
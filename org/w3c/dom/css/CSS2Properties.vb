'
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
' *
' *
' *
' *
' *
' * Copyright (c) 2000 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.css


	''' <summary>
	'''  The <code>CSS2Properties</code> interface represents a convenience
	''' mechanism for retrieving and setting properties within a
	''' <code>CSSStyleDeclaration</code>. The attributes of this interface
	''' correspond to all the properties specified in CSS2. Getting an attribute
	''' of this interface is equivalent to calling the
	''' <code>getPropertyValue</code> method of the
	''' <code>CSSStyleDeclaration</code> interface. Setting an attribute of this
	''' interface is equivalent to calling the <code>setProperty</code> method of
	''' the <code>CSSStyleDeclaration</code> interface.
	''' <p> A conformant implementation of the CSS module is not required to
	''' implement the <code>CSS2Properties</code> interface. If an implementation
	''' does implement this interface, the expectation is that language-specific
	''' methods can be used to cast from an instance of the
	''' <code>CSSStyleDeclaration</code> interface to the
	''' <code>CSS2Properties</code> interface.
	''' <p> If an implementation does implement this interface, it is expected to
	''' understand the specific syntax of the shorthand properties, and apply
	''' their semantics; when the <code>margin</code> property is set, for
	''' example, the <code>marginTop</code>, <code>marginRight</code>,
	''' <code>marginBottom</code> and <code>marginLeft</code> properties are
	''' actually being set by the underlying implementation.
	''' <p> When dealing with CSS "shorthand" properties, the shorthand properties
	''' should be decomposed into their component longhand properties as
	''' appropriate, and when querying for their value, the form returned should
	''' be the shortest form exactly equivalent to the declarations made in the
	''' ruleset. However, if there is no shorthand declaration that could be
	''' added to the ruleset without changing in any way the rules already
	''' declared in the ruleset (i.e., by adding longhand rules that were
	''' previously not declared in the ruleset), then the empty string should be
	''' returned for the shorthand property.
	''' <p> For example, querying for the <code>font</code> property should not
	''' return "normal normal normal 14pt/normal Arial, sans-serif", when "14pt
	''' Arial, sans-serif" suffices. (The normals are initial values, and are
	''' implied by use of the longhand property.)
	''' <p> If the values for all the longhand properties that compose a particular
	''' string are the initial values, then a string consisting of all the
	''' initial values should be returned (e.g. a <code>border-width</code> value
	''' of "medium" should be returned as such, not as "").
	''' <p> For some shorthand properties that take missing values from other
	''' sides, such as the <code>margin</code>, <code>padding</code>, and
	''' <code>border-[width|style|color]</code> properties, the minimum number of
	''' sides possible should be used; i.e., "0px 10px" will be returned instead
	''' of "0px 10px 0px 10px".
	''' <p> If the value of a shorthand property can not be decomposed into its
	''' component longhand properties, as is the case for the <code>font</code>
	''' property with a value of "menu", querying for the values of the component
	''' longhand properties should return the empty string.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Style-20001113'>Document Object Model (DOM) Level 2 Style Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface CSS2Properties
		''' <summary>
		'''  See the azimuth property definition in CSS2.
		''' </summary>
		Property azimuth As String

		''' <summary>
		'''  See the background property definition in CSS2.
		''' </summary>
		Property background As String

		''' <summary>
		'''  See the background-attachment property definition in CSS2.
		''' </summary>
		Property backgroundAttachment As String

		''' <summary>
		'''  See the background-color property definition in CSS2.
		''' </summary>
		Property backgroundColor As String

		''' <summary>
		'''  See the background-image property definition in CSS2.
		''' </summary>
		Property backgroundImage As String

		''' <summary>
		'''  See the background-position property definition in CSS2.
		''' </summary>
		Property backgroundPosition As String

		''' <summary>
		'''  See the background-repeat property definition in CSS2.
		''' </summary>
		Property backgroundRepeat As String

		''' <summary>
		'''  See the border property definition in CSS2.
		''' </summary>
		Property border As String

		''' <summary>
		'''  See the border-collapse property definition in CSS2.
		''' </summary>
		Property borderCollapse As String

		''' <summary>
		'''  See the border-color property definition in CSS2.
		''' </summary>
		Property borderColor As String

		''' <summary>
		'''  See the border-spacing property definition in CSS2.
		''' </summary>
		Property borderSpacing As String

		''' <summary>
		'''  See the border-style property definition in CSS2.
		''' </summary>
		Property borderStyle As String

		''' <summary>
		'''  See the border-top property definition in CSS2.
		''' </summary>
		Property borderTop As String

		''' <summary>
		'''  See the border-right property definition in CSS2.
		''' </summary>
		Property borderRight As String

		''' <summary>
		'''  See the border-bottom property definition in CSS2.
		''' </summary>
		Property borderBottom As String

		''' <summary>
		'''  See the border-left property definition in CSS2.
		''' </summary>
		Property borderLeft As String

		''' <summary>
		'''  See the border-top-color property definition in CSS2.
		''' </summary>
		Property borderTopColor As String

		''' <summary>
		'''  See the border-right-color property definition in CSS2.
		''' </summary>
		Property borderRightColor As String

		''' <summary>
		'''  See the border-bottom-color property definition in CSS2.
		''' </summary>
		Property borderBottomColor As String

		''' <summary>
		'''  See the border-left-color property definition in CSS2.
		''' </summary>
		Property borderLeftColor As String

		''' <summary>
		'''  See the border-top-style property definition in CSS2.
		''' </summary>
		Property borderTopStyle As String

		''' <summary>
		'''  See the border-right-style property definition in CSS2.
		''' </summary>
		Property borderRightStyle As String

		''' <summary>
		'''  See the border-bottom-style property definition in CSS2.
		''' </summary>
		Property borderBottomStyle As String

		''' <summary>
		'''  See the border-left-style property definition in CSS2.
		''' </summary>
		Property borderLeftStyle As String

		''' <summary>
		'''  See the border-top-width property definition in CSS2.
		''' </summary>
		Property borderTopWidth As String

		''' <summary>
		'''  See the border-right-width property definition in CSS2.
		''' </summary>
		Property borderRightWidth As String

		''' <summary>
		'''  See the border-bottom-width property definition in CSS2.
		''' </summary>
		Property borderBottomWidth As String

		''' <summary>
		'''  See the border-left-width property definition in CSS2.
		''' </summary>
		Property borderLeftWidth As String

		''' <summary>
		'''  See the border-width property definition in CSS2.
		''' </summary>
		Property borderWidth As String

		''' <summary>
		'''  See the bottom property definition in CSS2.
		''' </summary>
		Property bottom As String

		''' <summary>
		'''  See the caption-side property definition in CSS2.
		''' </summary>
		Property captionSide As String

		''' <summary>
		'''  See the clear property definition in CSS2.
		''' </summary>
		Property clear As String

		''' <summary>
		'''  See the clip property definition in CSS2.
		''' </summary>
		Property clip As String

		''' <summary>
		'''  See the color property definition in CSS2.
		''' </summary>
		Property color As String

		''' <summary>
		'''  See the content property definition in CSS2.
		''' </summary>
		Property content As String

		''' <summary>
		'''  See the counter-increment property definition in CSS2.
		''' </summary>
		Property counterIncrement As String

		''' <summary>
		'''  See the counter-reset property definition in CSS2.
		''' </summary>
		Property counterReset As String

		''' <summary>
		'''  See the cue property definition in CSS2.
		''' </summary>
		Property cue As String

		''' <summary>
		'''  See the cue-after property definition in CSS2.
		''' </summary>
		Property cueAfter As String

		''' <summary>
		'''  See the cue-before property definition in CSS2.
		''' </summary>
		Property cueBefore As String

		''' <summary>
		'''  See the cursor property definition in CSS2.
		''' </summary>
		Property cursor As String

		''' <summary>
		'''  See the direction property definition in CSS2.
		''' </summary>
		Property direction As String

		''' <summary>
		'''  See the display property definition in CSS2.
		''' </summary>
		Property display As String

		''' <summary>
		'''  See the elevation property definition in CSS2.
		''' </summary>
		Property elevation As String

		''' <summary>
		'''  See the empty-cells property definition in CSS2.
		''' </summary>
		Property emptyCells As String

		''' <summary>
		'''  See the float property definition in CSS2.
		''' </summary>
		Property cssFloat As String

		''' <summary>
		'''  See the font property definition in CSS2.
		''' </summary>
		Property font As String

		''' <summary>
		'''  See the font-family property definition in CSS2.
		''' </summary>
		Property fontFamily As String

		''' <summary>
		'''  See the font-size property definition in CSS2.
		''' </summary>
		Property fontSize As String

		''' <summary>
		'''  See the font-size-adjust property definition in CSS2.
		''' </summary>
		Property fontSizeAdjust As String

		''' <summary>
		'''  See the font-stretch property definition in CSS2.
		''' </summary>
		Property fontStretch As String

		''' <summary>
		'''  See the font-style property definition in CSS2.
		''' </summary>
		Property fontStyle As String

		''' <summary>
		'''  See the font-variant property definition in CSS2.
		''' </summary>
		Property fontVariant As String

		''' <summary>
		'''  See the font-weight property definition in CSS2.
		''' </summary>
		Property fontWeight As String

		''' <summary>
		'''  See the height property definition in CSS2.
		''' </summary>
		Property height As String

		''' <summary>
		'''  See the left property definition in CSS2.
		''' </summary>
		Property left As String

		''' <summary>
		'''  See the letter-spacing property definition in CSS2.
		''' </summary>
		Property letterSpacing As String

		''' <summary>
		'''  See the line-height property definition in CSS2.
		''' </summary>
		Property lineHeight As String

		''' <summary>
		'''  See the list-style property definition in CSS2.
		''' </summary>
		Property listStyle As String

		''' <summary>
		'''  See the list-style-image property definition in CSS2.
		''' </summary>
		Property listStyleImage As String

		''' <summary>
		'''  See the list-style-position property definition in CSS2.
		''' </summary>
		Property listStylePosition As String

		''' <summary>
		'''  See the list-style-type property definition in CSS2.
		''' </summary>
		Property listStyleType As String

		''' <summary>
		'''  See the margin property definition in CSS2.
		''' </summary>
		Property margin As String

		''' <summary>
		'''  See the margin-top property definition in CSS2.
		''' </summary>
		Property marginTop As String

		''' <summary>
		'''  See the margin-right property definition in CSS2.
		''' </summary>
		Property marginRight As String

		''' <summary>
		'''  See the margin-bottom property definition in CSS2.
		''' </summary>
		Property marginBottom As String

		''' <summary>
		'''  See the margin-left property definition in CSS2.
		''' </summary>
		Property marginLeft As String

		''' <summary>
		'''  See the marker-offset property definition in CSS2.
		''' </summary>
		Property markerOffset As String

		''' <summary>
		'''  See the marks property definition in CSS2.
		''' </summary>
		Property marks As String

		''' <summary>
		'''  See the max-height property definition in CSS2.
		''' </summary>
		Property maxHeight As String

		''' <summary>
		'''  See the max-width property definition in CSS2.
		''' </summary>
		Property maxWidth As String

		''' <summary>
		'''  See the min-height property definition in CSS2.
		''' </summary>
		Property minHeight As String

		''' <summary>
		'''  See the min-width property definition in CSS2.
		''' </summary>
		Property minWidth As String

		''' <summary>
		'''  See the orphans property definition in CSS2.
		''' </summary>
		Property orphans As String

		''' <summary>
		'''  See the outline property definition in CSS2.
		''' </summary>
		Property outline As String

		''' <summary>
		'''  See the outline-color property definition in CSS2.
		''' </summary>
		Property outlineColor As String

		''' <summary>
		'''  See the outline-style property definition in CSS2.
		''' </summary>
		Property outlineStyle As String

		''' <summary>
		'''  See the outline-width property definition in CSS2.
		''' </summary>
		Property outlineWidth As String

		''' <summary>
		'''  See the overflow property definition in CSS2.
		''' </summary>
		Property overflow As String

		''' <summary>
		'''  See the padding property definition in CSS2.
		''' </summary>
		Property padding As String

		''' <summary>
		'''  See the padding-top property definition in CSS2.
		''' </summary>
		Property paddingTop As String

		''' <summary>
		'''  See the padding-right property definition in CSS2.
		''' </summary>
		Property paddingRight As String

		''' <summary>
		'''  See the padding-bottom property definition in CSS2.
		''' </summary>
		Property paddingBottom As String

		''' <summary>
		'''  See the padding-left property definition in CSS2.
		''' </summary>
		Property paddingLeft As String

		''' <summary>
		'''  See the page property definition in CSS2.
		''' </summary>
		Property page As String

		''' <summary>
		'''  See the page-break-after property definition in CSS2.
		''' </summary>
		Property pageBreakAfter As String

		''' <summary>
		'''  See the page-break-before property definition in CSS2.
		''' </summary>
		Property pageBreakBefore As String

		''' <summary>
		'''  See the page-break-inside property definition in CSS2.
		''' </summary>
		Property pageBreakInside As String

		''' <summary>
		'''  See the pause property definition in CSS2.
		''' </summary>
		Property pause As String

		''' <summary>
		'''  See the pause-after property definition in CSS2.
		''' </summary>
		Property pauseAfter As String

		''' <summary>
		'''  See the pause-before property definition in CSS2.
		''' </summary>
		Property pauseBefore As String

		''' <summary>
		'''  See the pitch property definition in CSS2.
		''' </summary>
		Property pitch As String

		''' <summary>
		'''  See the pitch-range property definition in CSS2.
		''' </summary>
		Property pitchRange As String

		''' <summary>
		'''  See the play-during property definition in CSS2.
		''' </summary>
		Property playDuring As String

		''' <summary>
		'''  See the position property definition in CSS2.
		''' </summary>
		Property position As String

		''' <summary>
		'''  See the quotes property definition in CSS2.
		''' </summary>
		Property quotes As String

		''' <summary>
		'''  See the richness property definition in CSS2.
		''' </summary>
		Property richness As String

		''' <summary>
		'''  See the right property definition in CSS2.
		''' </summary>
		Property right As String

		''' <summary>
		'''  See the size property definition in CSS2.
		''' </summary>
		Property size As String

		''' <summary>
		'''  See the speak property definition in CSS2.
		''' </summary>
		Property speak As String

		''' <summary>
		'''  See the speak-header property definition in CSS2.
		''' </summary>
		Property speakHeader As String

		''' <summary>
		'''  See the speak-numeral property definition in CSS2.
		''' </summary>
		Property speakNumeral As String

		''' <summary>
		'''  See the speak-punctuation property definition in CSS2.
		''' </summary>
		Property speakPunctuation As String

		''' <summary>
		'''  See the speech-rate property definition in CSS2.
		''' </summary>
		Property speechRate As String

		''' <summary>
		'''  See the stress property definition in CSS2.
		''' </summary>
		Property stress As String

		''' <summary>
		'''  See the table-layout property definition in CSS2.
		''' </summary>
		Property tableLayout As String

		''' <summary>
		'''  See the text-align property definition in CSS2.
		''' </summary>
		Property textAlign As String

		''' <summary>
		'''  See the text-decoration property definition in CSS2.
		''' </summary>
		Property textDecoration As String

		''' <summary>
		'''  See the text-indent property definition in CSS2.
		''' </summary>
		Property textIndent As String

		''' <summary>
		'''  See the text-shadow property definition in CSS2.
		''' </summary>
		Property textShadow As String

		''' <summary>
		'''  See the text-transform property definition in CSS2.
		''' </summary>
		Property textTransform As String

		''' <summary>
		'''  See the top property definition in CSS2.
		''' </summary>
		Property top As String

		''' <summary>
		'''  See the unicode-bidi property definition in CSS2.
		''' </summary>
		Property unicodeBidi As String

		''' <summary>
		'''  See the vertical-align property definition in CSS2.
		''' </summary>
		Property verticalAlign As String

		''' <summary>
		'''  See the visibility property definition in CSS2.
		''' </summary>
		Property visibility As String

		''' <summary>
		'''  See the voice-family property definition in CSS2.
		''' </summary>
		Property voiceFamily As String

		''' <summary>
		'''  See the volume property definition in CSS2.
		''' </summary>
		Property volume As String

		''' <summary>
		'''  See the white-space property definition in CSS2.
		''' </summary>
		Property whiteSpace As String

		''' <summary>
		'''  See the widows property definition in CSS2.
		''' </summary>
		Property widows As String

		''' <summary>
		'''  See the width property definition in CSS2.
		''' </summary>
		Property width As String

		''' <summary>
		'''  See the word-spacing property definition in CSS2.
		''' </summary>
		Property wordSpacing As String

		''' <summary>
		'''  See the z-index property definition in CSS2.
		''' </summary>
		Property zIndex As String

	End Interface

End Namespace
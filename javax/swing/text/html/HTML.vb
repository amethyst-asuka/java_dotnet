Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' Constants used in the <code>HTMLDocument</code>.  These
	''' are basically tag and attribute definitions.
	''' 
	''' @author  Timothy Prinzing
	''' @author  Sunita Mani
	''' 
	''' </summary>
	Public Class HTML

		''' <summary>
		''' Typesafe enumeration for an HTML tag.  Although the
		''' set of HTML tags is a closed set, we have left the
		''' set open so that people can add their own tag types
		''' to their custom parser and still communicate to the
		''' reader.
		''' </summary>
		Public Class Tag

			''' <summary>
			''' @since 1.3 </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Creates a new <code>Tag</code> with the specified <code>id</code>,
			''' and with <code>causesBreak</code> and <code>isBlock</code>
			''' set to <code>false</code>.
			''' </summary>
			''' <param name="id">  the id of the new tag </param>
			Protected Friend Sub New(ByVal id As String)
				Me.New(id, False, False)
			End Sub

			''' <summary>
			''' Creates a new <code>Tag</code> with the specified <code>id</code>;
			''' <code>causesBreak</code> and <code>isBlock</code> are defined
			''' by the user.
			''' </summary>
			''' <param name="id"> the id of the new tag </param>
			''' <param name="causesBreak">  <code>true</code> if this tag
			'''    causes a break to the flow of data </param>
			''' <param name="isBlock"> <code>true</code> if the tag is used
			'''    to add structure to a document </param>
			Protected Friend Sub New(ByVal id As String, ByVal causesBreak As Boolean, ByVal isBlock As Boolean)
				name = id
				Me.breakTag = causesBreak
				Me.blockTag = isBlock
			End Sub

			''' <summary>
			''' Returns <code>true</code> if this tag is a block
			''' tag, which is a tag used to add structure to a
			''' document.
			''' </summary>
			''' <returns> <code>true</code> if this tag is a block
			'''   tag, otherwise returns <code>false</code> </returns>
			Public Overridable Property block As Boolean
				Get
					Return blockTag
				End Get
			End Property

			''' <summary>
			''' Returns <code>true</code> if this tag causes a
			''' line break to the flow of data, otherwise returns
			''' <code>false</code>.
			''' </summary>
			''' <returns> <code>true</code> if this tag causes a
			'''   line break to the flow of data, otherwise returns
			'''   <code>false</code> </returns>
			Public Overridable Function breaksFlow() As Boolean
				Return breakTag
			End Function

			''' <summary>
			''' Returns <code>true</code> if this tag is pre-formatted,
			''' which is true if the tag is either <code>PRE</code> or
			''' <code>TEXTAREA</code>.
			''' </summary>
			''' <returns> <code>true</code> if this tag is pre-formatted,
			'''   otherwise returns <code>false</code> </returns>
			Public Overridable Property preformatted As Boolean
				Get
					Return (Me Is PRE OrElse Me Is TEXTAREA)
				End Get
			End Property

			''' <summary>
			''' Returns the string representation of the
			''' tag.
			''' </summary>
			''' <returns> the <code>String</code> representation of the tag </returns>
			Public Overrides Function ToString() As String
				Return name
			End Function

			''' <summary>
			''' Returns <code>true</code> if this tag is considered to be a paragraph
			''' in the internal HTML model. <code>false</code> - otherwise.
			''' </summary>
			''' <returns> <code>true</code> if this tag is considered to be a paragraph
			'''         in the internal HTML model. <code>false</code> - otherwise. </returns>
			''' <seealso cref= HTMLDocument.HTMLReader.ParagraphAction </seealso>
			Friend Overridable Property paragraph As Boolean
				Get
					Return (Me Is P OrElse Me Is IMPLIED OrElse Me Is DT OrElse Me Is H1 OrElse Me Is H2 OrElse Me Is H3 OrElse Me Is H4 OrElse Me Is H5 OrElse Me Is H6)
				End Get
			End Property

			Friend blockTag As Boolean
			Friend breakTag As Boolean
			Friend name As String
			Friend unknown As Boolean

			' --- Tag Names -----------------------------------

			Public Shared ReadOnly A As New Tag("a")
			Public Shared ReadOnly ADDRESS As New Tag("address")
			Public Shared ReadOnly APPLET As New Tag("applet")
			Public Shared ReadOnly AREA As New Tag("area")
			Public Shared ReadOnly B As New Tag("b")
			Public Shared ReadOnly BASE As New Tag("base")
			Public Shared ReadOnly BASEFONT As New Tag("basefont")
			Public Shared ReadOnly BIG As New Tag("big")
			Public Shared ReadOnly BLOCKQUOTE As New Tag("blockquote", True, True)
			Public Shared ReadOnly BODY As New Tag("body", True, True)
			Public Shared ReadOnly BR As New Tag("br", True, False)
			Public Shared ReadOnly CAPTION As New Tag("caption")
			Public Shared ReadOnly CENTER As New Tag("center", True, False)
			Public Shared ReadOnly CITE As New Tag("cite")
			Public Shared ReadOnly CODE As New Tag("code")
			Public Shared ReadOnly DD As New Tag("dd", True, True)
			Public Shared ReadOnly DFN As New Tag("dfn")
			Public Shared ReadOnly DIR As New Tag("dir", True, True)
			Public Shared ReadOnly DIV As New Tag("div", True, True)
			Public Shared ReadOnly DL As New Tag("dl", True, True)
			Public Shared ReadOnly DT As New Tag("dt", True, True)
			Public Shared ReadOnly EM As New Tag("em")
			Public Shared ReadOnly FONT As New Tag("font")
			Public Shared ReadOnly FORM As New Tag("form", True, False)
			Public Shared ReadOnly FRAME As New Tag("frame")
			Public Shared ReadOnly FRAMESET As New Tag("frameset")
			Public Shared ReadOnly H1 As New Tag("h1", True, True)
			Public Shared ReadOnly H2 As New Tag("h2", True, True)
			Public Shared ReadOnly H3 As New Tag("h3", True, True)
			Public Shared ReadOnly H4 As New Tag("h4", True, True)
			Public Shared ReadOnly H5 As New Tag("h5", True, True)
			Public Shared ReadOnly H6 As New Tag("h6", True, True)
			Public Shared ReadOnly HEAD As New Tag("head", True, True)
			Public Shared ReadOnly HR As New Tag("hr", True, False)
			Public Shared ReadOnly HTML As New Tag("html", True, False)
			Public Shared ReadOnly I As New Tag("i")
			Public Shared ReadOnly IMG As New Tag("img")
			Public Shared ReadOnly INPUT As New Tag("input")
			Public Shared ReadOnly ISINDEX As New Tag("isindex", True, False)
			Public Shared ReadOnly KBD As New Tag("kbd")
			Public Shared ReadOnly LI As New Tag("li", True, True)
			Public Shared ReadOnly LINK As New Tag("link")
			Public Shared ReadOnly MAP As New Tag("map")
			Public Shared ReadOnly MENU As New Tag("menu", True, True)
			Public Shared ReadOnly META As New Tag("meta")
			'public
	 Friend Shared ReadOnly NOBR As New Tag("nobr")
			Public Shared ReadOnly NOFRAMES As New Tag("noframes", True, True)
			Public Shared ReadOnly [OBJECT] As New Tag("object")
			Public Shared ReadOnly OL As New Tag("ol", True, True)
			Public Shared ReadOnly [OPTION] As New Tag("option")
			Public Shared ReadOnly P As New Tag("p", True, True)
			Public Shared ReadOnly PARAM As New Tag("param")
			Public Shared ReadOnly PRE As New Tag("pre", True, True)
			Public Shared ReadOnly SAMP As New Tag("samp")
			Public Shared ReadOnly SCRIPT As New Tag("script")
			Public Shared ReadOnly [SELECT] As New Tag("select")
			Public Shared ReadOnly SMALL As New Tag("small")
			Public Shared ReadOnly SPAN As New Tag("span")
			Public Shared ReadOnly STRIKE As New Tag("strike")
			Public Shared ReadOnly S As New Tag("s")
			Public Shared ReadOnly STRONG As New Tag("strong")
			Public Shared ReadOnly STYLE As New Tag("style")
			Public Shared ReadOnly [SUB] As New Tag("sub")
			Public Shared ReadOnly SUP As New Tag("sup")
			Public Shared ReadOnly TABLE As New Tag("table", False, True)
			Public Shared ReadOnly TD As New Tag("td", True, True)
			Public Shared ReadOnly TEXTAREA As New Tag("textarea")
			Public Shared ReadOnly TH As New Tag("th", True, True)
			Public Shared ReadOnly TITLE As New Tag("title", True, True)
			Public Shared ReadOnly TR As New Tag("tr", False, True)
			Public Shared ReadOnly TT As New Tag("tt")
			Public Shared ReadOnly U As New Tag("u")
			Public Shared ReadOnly UL As New Tag("ul", True, True)
			Public Shared ReadOnly VAR As New Tag("var")

			''' <summary>
			''' All text content must be in a paragraph element.
			''' If a paragraph didn't exist when content was
			''' encountered, a paragraph is manufactured.
			''' <p>
			''' This is a tag synthesized by the HTML reader.
			''' Since elements are identified by their tag type,
			''' we create a some fake tag types to mark the elements
			''' that were manufactured.
			''' </summary>
			Public Shared ReadOnly IMPLIED As New Tag("p-implied")

			''' <summary>
			''' All text content is labeled with this tag.
			''' <p>
			''' This is a tag synthesized by the HTML reader.
			''' Since elements are identified by their tag type,
			''' we create a some fake tag types to mark the elements
			''' that were manufactured.
			''' </summary>
			Public Shared ReadOnly CONTENT As New Tag("content")

			''' <summary>
			''' All comments are labeled with this tag.
			''' <p>
			''' This is a tag synthesized by the HTML reader.
			''' Since elements are identified by their tag type,
			''' we create a some fake tag types to mark the elements
			''' that were manufactured.
			''' </summary>
			Public Shared ReadOnly COMMENT As New Tag("comment")

			Friend Shared ReadOnly allTags As Tag() = { A, ADDRESS, APPLET, AREA, B, BASE, BASEFONT, BIG, BLOCKQUOTE, BODY, BR, CAPTION, CENTER, CITE, CODE, DD, DFN, DIR, DIV, DL, DT, EM, FONT, FORM, FRAME, FRAMESET, H1, H2, H3, H4, H5, H6, HEAD, HR, HTML, I, IMG, INPUT, ISINDEX, KBD, LI, LINK, MAP, MENU, META, NOBR, NOFRAMES, [OBJECT], OL, [OPTION], P, PARAM, PRE, SAMP, SCRIPT, [SELECT], SMALL, SPAN, STRIKE, S, STRONG, STYLE, [SUB], SUP, TABLE, TD, TEXTAREA, TH, TITLE, TR, TT, U, UL, VAR }

			Shared Sub New()
				' Force HTMLs static initialize to be loaded.
				getTag("html")
			End Sub
		End Class

		' There is no unique instance of UnknownTag, so we allow it to be
		' Serializable.
		<Serializable> _
		Public Class UnknownTag
			Inherits Tag

			''' <summary>
			''' Creates a new <code>UnknownTag</code> with the specified
			''' <code>id</code>. </summary>
			''' <param name="id"> the id of the new tag </param>
			Public Sub New(ByVal id As String)
				MyBase.New(id)
			End Sub

			''' <summary>
			''' Returns the hash code which corresponds to the string
			''' for this tag.
			''' </summary>
			Public Overrides Function GetHashCode() As Integer
				Return ToString().GetHashCode()
			End Function

			''' <summary>
			''' Compares this object to the specified object.
			''' The result is <code>true</code> if and only if the argument is not
			''' <code>null</code> and is an <code>UnknownTag</code> object
			''' with the same name.
			''' </summary>
			''' <param name="obj">   the object to compare this tag with </param>
			''' <returns>    <code>true</code> if the objects are equal;
			'''            <code>false</code> otherwise </returns>
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is UnknownTag Then Return ToString().Equals(obj.ToString())
				Return False
			End Function

			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				s.defaultWriteObject()
				s.writeBoolean(blockTag)
				s.writeBoolean(breakTag)
				s.writeBoolean(unknown)
				s.writeObject(name)
			End Sub

			Private Sub readObject(ByVal s As ObjectInputStream)
				s.defaultReadObject()
				blockTag = s.readBoolean()
				breakTag = s.readBoolean()
				unknown = s.readBoolean()
				name = CStr(s.readObject())
			End Sub
		End Class

		''' <summary>
		''' Typesafe enumeration representing an HTML
		''' attribute.
		''' </summary>
		Public NotInheritable Class Attribute

			''' <summary>
			''' Creates a new <code>Attribute</code> with the specified
			''' <code>id</code>.
			''' </summary>
			''' <param name="id"> the id of the new <code>Attribute</code> </param>
			Friend Sub New(ByVal id As String)
				name = id
			End Sub

			''' <summary>
			''' Returns the string representation of this attribute. </summary>
			''' <returns> the string representation of this attribute </returns>
			Public Overrides Function ToString() As String
				Return name
			End Function

			Private name As String

			Public Shared ReadOnly SIZE As New Attribute("size")
			Public Shared ReadOnly COLOR As New Attribute("color")
			Public Shared ReadOnly CLEAR As New Attribute("clear")
			Public Shared ReadOnly BACKGROUND As New Attribute("background")
			Public Shared ReadOnly BGCOLOR As New Attribute("bgcolor")
			Public Shared ReadOnly TEXT As New Attribute("text")
			Public Shared ReadOnly LINK As New Attribute("link")
			Public Shared ReadOnly VLINK As New Attribute("vlink")
			Public Shared ReadOnly ALINK As New Attribute("alink")
			Public Shared ReadOnly WIDTH As New Attribute("width")
			Public Shared ReadOnly HEIGHT As New Attribute("height")
			Public Shared ReadOnly ALIGN As New Attribute("align")
			Public Shared ReadOnly NAME As New Attribute("name")
			Public Shared ReadOnly HREF As New Attribute("href")
			Public Shared ReadOnly REL As New Attribute("rel")
			Public Shared ReadOnly REV As New Attribute("rev")
			Public Shared ReadOnly TITLE As New Attribute("title")
			Public Shared ReadOnly TARGET As New Attribute("target")
			Public Shared ReadOnly SHAPE As New Attribute("shape")
			Public Shared ReadOnly COORDS As New Attribute("coords")
			Public Shared ReadOnly ISMAP As New Attribute("ismap")
			Public Shared ReadOnly NOHREF As New Attribute("nohref")
			Public Shared ReadOnly ALT As New Attribute("alt")
			Public Shared ReadOnly ID As New Attribute("id")
			Public Shared ReadOnly SRC As New Attribute("src")
			Public Shared ReadOnly HSPACE As New Attribute("hspace")
			Public Shared ReadOnly VSPACE As New Attribute("vspace")
			Public Shared ReadOnly USEMAP As New Attribute("usemap")
			Public Shared ReadOnly LOWSRC As New Attribute("lowsrc")
			Public Shared ReadOnly CODEBASE As New Attribute("codebase")
			Public Shared ReadOnly CODE As New Attribute("code")
			Public Shared ReadOnly ARCHIVE As New Attribute("archive")
			Public Shared ReadOnly VALUE As New Attribute("value")
			Public Shared ReadOnly VALUETYPE As New Attribute("valuetype")
			Public Shared ReadOnly TYPE As New Attribute("type")
			Public Shared ReadOnly [CLASS] As New Attribute("class")
			Public Shared ReadOnly STYLE As New Attribute("style")
			Public Shared ReadOnly LANG As New Attribute("lang")
			Public Shared ReadOnly FACE As New Attribute("face")
			Public Shared ReadOnly DIR As New Attribute("dir")
			Public Shared ReadOnly [DECLARE] As New Attribute("declare")
			Public Shared ReadOnly CLASSID As New Attribute("classid")
			Public Shared ReadOnly DATA As New Attribute("data")
			Public Shared ReadOnly CODETYPE As New Attribute("codetype")
			Public Shared ReadOnly STANDBY As New Attribute("standby")
			Public Shared ReadOnly BORDER As New Attribute("border")
			Public Shared ReadOnly SHAPES As New Attribute("shapes")
			Public Shared ReadOnly NOSHADE As New Attribute("noshade")
			Public Shared ReadOnly COMPACT As New Attribute("compact")
			Public Shared ReadOnly START As New Attribute("start")
			Public Shared ReadOnly ACTION As New Attribute("action")
			Public Shared ReadOnly METHOD As New Attribute("method")
			Public Shared ReadOnly ENCTYPE As New Attribute("enctype")
			Public Shared ReadOnly CHECKED As New Attribute("checked")
			Public Shared ReadOnly MAXLENGTH As New Attribute("maxlength")
			Public Shared ReadOnly MULTIPLE As New Attribute("multiple")
			Public Shared ReadOnly SELECTED As New Attribute("selected")
			Public Shared ReadOnly ROWS As New Attribute("rows")
			Public Shared ReadOnly COLS As New Attribute("cols")
			Public Shared ReadOnly DUMMY As New Attribute("dummy")
			Public Shared ReadOnly CELLSPACING As New Attribute("cellspacing")
			Public Shared ReadOnly CELLPADDING As New Attribute("cellpadding")
			Public Shared ReadOnly VALIGN As New Attribute("valign")
			Public Shared ReadOnly HALIGN As New Attribute("halign")
			Public Shared ReadOnly NOWRAP As New Attribute("nowrap")
			Public Shared ReadOnly ROWSPAN As New Attribute("rowspan")
			Public Shared ReadOnly COLSPAN As New Attribute("colspan")
			Public Shared ReadOnly PROMPT As New Attribute("prompt")
			Public Shared ReadOnly HTTPEQUIV As New Attribute("http-equiv")
			Public Shared ReadOnly CONTENT As New Attribute("content")
			Public Shared ReadOnly LANGUAGE As New Attribute("language")
			Public Shared ReadOnly VERSION As New Attribute("version")
			Public Shared ReadOnly N As New Attribute("n")
			Public Shared ReadOnly FRAMEBORDER As New Attribute("frameborder")
			Public Shared ReadOnly MARGINWIDTH As New Attribute("marginwidth")
			Public Shared ReadOnly MARGINHEIGHT As New Attribute("marginheight")
			Public Shared ReadOnly SCROLLING As New Attribute("scrolling")
			Public Shared ReadOnly NORESIZE As New Attribute("noresize")
			Public Shared ReadOnly ENDTAG As New Attribute("endtag")
			Public Shared ReadOnly COMMENT As New Attribute("comment")
			Friend Shared ReadOnly MEDIA As New Attribute("media")

			Friend Shared ReadOnly allAttributes As Attribute() = { FACE, COMMENT, SIZE, COLOR, CLEAR, BACKGROUND, BGCOLOR, TEXT, LINK, VLINK, ALINK, WIDTH, HEIGHT, ALIGN, NAME, HREF, REL, REV, TITLE, TARGET, SHAPE, COORDS, ISMAP, NOHREF, ALT, ID, SRC, HSPACE, VSPACE, USEMAP, LOWSRC, CODEBASE, CODE, ARCHIVE, VALUE, VALUETYPE, TYPE, [CLASS], STYLE, LANG, DIR, [DECLARE], CLASSID, DATA, CODETYPE, STANDBY, BORDER, SHAPES, NOSHADE, COMPACT, START, ACTION, METHOD, ENCTYPE, CHECKED, MAXLENGTH, MULTIPLE, SELECTED, ROWS, COLS, DUMMY, CELLSPACING, CELLPADDING, VALIGN, HALIGN, NOWRAP, ROWSPAN, COLSPAN, PROMPT, HTTPEQUIV, CONTENT, LANGUAGE, VERSION, N, FRAMEBORDER, MARGINWIDTH, MARGINHEIGHT, SCROLLING, NORESIZE, MEDIA, ENDTAG }
		End Class

		' The secret to 73, is that, given that the Hashtable contents
		' never change once the static initialization happens, the initial size
		' that the hashtable grew to was determined, and then that very size
		' is used.
		'
		Private Shared ReadOnly tagHashtable As New Dictionary(Of String, Tag)(73)

		''' <summary>
		''' Maps from StyleConstant key to HTML.Tag. </summary>
		Private Shared ReadOnly scMapping As New Dictionary(Of Object, Tag)(8)

		Shared Sub New()

			For i As Integer = 0 To Tag.allTags.Length - 1
				tagHashtable(Tag.allTags(i).ToString()) = Tag.allTags(i)
				javax.swing.text.StyleContext.registerStaticAttributeKey(Tag.allTags(i))
			Next i
			javax.swing.text.StyleContext.registerStaticAttributeKey(Tag.IMPLIED)
			javax.swing.text.StyleContext.registerStaticAttributeKey(Tag.CONTENT)
			javax.swing.text.StyleContext.registerStaticAttributeKey(Tag.COMMENT)
			For i As Integer = 0 To Attribute.allAttributes.Length - 1
				javax.swing.text.StyleContext.registerStaticAttributeKey(Attribute.allAttributes(i))
			Next i
			javax.swing.text.StyleContext.registerStaticAttributeKey(HTML.NULL_ATTRIBUTE_VALUE)
			scMapping(javax.swing.text.StyleConstants.Bold) = Tag.B
			scMapping(javax.swing.text.StyleConstants.Italic) = Tag.I
			scMapping(javax.swing.text.StyleConstants.Underline) = Tag.U
			scMapping(javax.swing.text.StyleConstants.StrikeThrough) = Tag.STRIKE
			scMapping(javax.swing.text.StyleConstants.Superscript) = Tag.SUP
			scMapping(javax.swing.text.StyleConstants.Subscript) = Tag.SUB
			scMapping(javax.swing.text.StyleConstants.FontFamily) = Tag.FONT
			scMapping(javax.swing.text.StyleConstants.FontSize) = Tag.FONT

			For i As Integer = 0 To Attribute.allAttributes.Length - 1
				attHashtable(Attribute.allAttributes(i).ToString()) = Attribute.allAttributes(i)
			Next i
		End Sub

		''' <summary>
		''' Returns the set of actual HTML tags that
		''' are recognized by the default HTML reader.
		''' This set does not include tags that are
		''' manufactured by the reader.
		''' </summary>
		Public Property Shared allTags As Tag()
			Get
				Dim tags As Tag() = New Tag(Tag.allTags.Length - 1){}
				Array.Copy(Tag.allTags, 0, tags, 0, Tag.allTags.Length)
				Return tags
			End Get
		End Property

		''' <summary>
		''' Fetches a tag constant for a well-known tag name (i.e. one of
		''' the tags in the set {A, ADDRESS, APPLET, AREA, B,
		''' BASE, BASEFONT, BIG,
		''' BLOCKQUOTE, BODY, BR, CAPTION, CENTER, CITE, CODE,
		''' DD, DFN, DIR, DIV, DL, DT, EM, FONT, FORM, FRAME,
		''' FRAMESET, H1, H2, H3, H4, H5, H6, HEAD, HR, HTML,
		''' I, IMG, INPUT, ISINDEX, KBD, LI, LINK, MAP, MENU,
		''' META, NOBR, NOFRAMES, OBJECT, OL, OPTION, P, PARAM,
		''' PRE, SAMP, SCRIPT, SELECT, SMALL, SPAN, STRIKE, S,
		''' STRONG, STYLE, SUB, SUP, TABLE, TD, TEXTAREA,
		''' TH, TITLE, TR, TT, U, UL, VAR}.  If the given
		''' name does not represent one of the well-known tags, then
		''' <code>null</code> will be returned.
		''' </summary>
		''' <param name="tagName"> the <code>String</code> name requested </param>
		''' <returns> a tag constant corresponding to the <code>tagName</code>,
		'''    or <code>null</code> if not found </returns>
		Public Shared Function getTag(ByVal tagName As String) As Tag

			Dim t As Tag = tagHashtable(tagName)
			Return (If(t Is Nothing, Nothing, t))
		End Function

		''' <summary>
		''' Returns the HTML <code>Tag</code> associated with the
		''' <code>StyleConstants</code> key <code>sc</code>.
		''' If no matching <code>Tag</code> is found, returns
		''' <code>null</code>.
		''' </summary>
		''' <param name="sc"> the <code>StyleConstants</code> key </param>
		''' <returns> tag which corresponds to <code>sc</code>, or
		'''   <code>null</code> if not found </returns>
		Friend Shared Function getTagForStyleConstantsKey(ByVal sc As javax.swing.text.StyleConstants) As Tag
			Return scMapping(sc)
		End Function

		''' <summary>
		''' Fetches an integer attribute value.  Attribute values
		''' are stored as a string, and this is a convenience method
		''' to convert to an actual integer.
		''' </summary>
		''' <param name="attr"> the set of attributes to use to try to fetch a value </param>
		''' <param name="key"> the key to use to fetch the value </param>
		''' <param name="def"> the default value to use if the attribute isn't
		'''  defined or there is an error converting to an integer </param>
		Public Shared Function getIntegerAttributeValue(ByVal attr As javax.swing.text.AttributeSet, ByVal key As Attribute, ByVal def As Integer) As Integer
			Dim value As Integer = def
			Dim istr As String = CStr(attr.getAttribute(key))
			If istr IsNot Nothing Then
				Try
					value = Convert.ToInt32(istr)
				Catch e As NumberFormatException
					value = def
				End Try
			End If
			Return value
		End Function

		'  This is used in cases where the value for the attribute has not
		'  been specified.
		'
		Public Const NULL_ATTRIBUTE_VALUE As String = "#DEFAULT"

		' size determined similar to size of tagHashtable
		Private Shared ReadOnly attHashtable As New Dictionary(Of String, Attribute)(77)


		''' <summary>
		''' Returns the set of HTML attributes recognized. </summary>
		''' <returns> the set of HTML attributes recognized </returns>
		Public Property Shared allAttributeKeys As Attribute()
			Get
				Dim attributes As Attribute() = New Attribute(Attribute.allAttributes.Length - 1){}
				Array.Copy(Attribute.allAttributes, 0, attributes, 0, Attribute.allAttributes.Length)
				Return attributes
			End Get
		End Property

		''' <summary>
		''' Fetches an attribute constant for a well-known attribute name
		''' (i.e. one of the attributes in the set {FACE, COMMENT, SIZE,
		''' COLOR, CLEAR, BACKGROUND, BGCOLOR, TEXT, LINK, VLINK, ALINK,
		''' WIDTH, HEIGHT, ALIGN, NAME, HREF, REL, REV, TITLE, TARGET,
		''' SHAPE, COORDS, ISMAP, NOHREF, ALT, ID, SRC, HSPACE, VSPACE,
		''' USEMAP, LOWSRC, CODEBASE, CODE, ARCHIVE, VALUE, VALUETYPE,
		''' TYPE, CLASS, STYLE, LANG, DIR, DECLARE, CLASSID, DATA, CODETYPE,
		''' STANDBY, BORDER, SHAPES, NOSHADE, COMPACT, START, ACTION, METHOD,
		''' ENCTYPE, CHECKED, MAXLENGTH, MULTIPLE, SELECTED, ROWS, COLS,
		''' DUMMY, CELLSPACING, CELLPADDING, VALIGN, HALIGN, NOWRAP, ROWSPAN,
		''' COLSPAN, PROMPT, HTTPEQUIV, CONTENT, LANGUAGE, VERSION, N,
		''' FRAMEBORDER, MARGINWIDTH, MARGINHEIGHT, SCROLLING, NORESIZE,
		''' MEDIA, ENDTAG}).
		''' If the given name does not represent one of the well-known attributes,
		''' then <code>null</code> will be returned.
		''' </summary>
		''' <param name="attName"> the <code>String</code> requested </param>
		''' <returns> the <code>Attribute</code> corresponding to <code>attName</code> </returns>
		Public Shared Function getAttributeKey(ByVal attName As String) As Attribute
			Dim a As Attribute = attHashtable(attName)
			If a Is Nothing Then Return Nothing
			Return a
		End Function

	End Class

End Namespace
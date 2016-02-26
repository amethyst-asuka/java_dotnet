Imports System

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

Namespace javax.swing.text.html.parser



	''' <summary>
	''' A Parser for HTML Documents (actually, you can specify a DTD, but
	''' you should really only use this class with the html dtd in swing).
	''' Reads an InputStream of HTML and
	''' invokes the appropriate methods in the ParserCallback class. This
	''' is the default parser used by HTMLEditorKit to parse HTML url's.
	''' <p>This will message the callback for all valid tags, as well as
	''' tags that are implied but not explicitly specified. For example, the
	''' html string (&lt;p&gt;blah) only has a p tag defined. The callback
	''' will see the following methods:
	''' <ol><li><i>handleStartTag(html, ...)</i></li>
	'''     <li><i>handleStartTag(head, ...)</i></li>
	'''     <li><i>handleEndTag(head)</i></li>
	'''     <li><i>handleStartTag(body, ...)</i></li>
	'''     <li><i>handleStartTag(p, ...)</i></li>
	'''     <li><i>handleText(...)</i></li>
	'''     <li><i>handleEndTag(p)</i></li>
	'''     <li><i>handleEndTag(body)</i></li>
	'''     <li><i>handleEndTag(html)</i></li>
	''' </ol>
	''' The items in <i>italic</i> are implied, that is, although they were not
	''' explicitly specified, to be correct html they should have been present
	''' (head isn't necessary, but it is still generated). For tags that
	''' are implied, the AttributeSet argument will have a value of
	''' <code>Boolean.TRUE</code> for the key
	''' <code>HTMLEditorKit.ParserCallback.IMPLIED</code>.
	''' <p>HTML.Attributes defines a type safe enumeration of html attributes.
	''' If an attribute key of a tag is defined in HTML.Attribute, the
	''' HTML.Attribute will be used as the key, otherwise a String will be used.
	''' For example &lt;p foo=bar class=neat&gt; has two attributes. foo is
	''' not defined in HTML.Attribute, where as class is, therefore the
	''' AttributeSet will have two values in it, HTML.Attribute.CLASS with
	''' a String value of 'neat' and the String key 'foo' with a String value of
	''' 'bar'.
	''' <p>The position argument will indicate the start of the tag, comment
	''' or text. Similar to arrays, the first character in the stream has a
	''' position of 0. For tags that are
	''' implied the position will indicate
	''' the location of the next encountered tag. In the first example,
	''' the implied start body and html tags will have the same position as the
	''' p tag, and the implied end p, html and body tags will all have the same
	''' position.
	''' <p>As html skips whitespace the position for text will be the position
	''' of the first valid character, eg in the string '\n\n\nblah'
	''' the text 'blah' will have a position of 3, the newlines are skipped.
	''' <p>
	''' For attributes that do not have a value, eg in the html
	''' string <code>&lt;foo blah&gt;</code> the attribute <code>blah</code>
	''' does not have a value, there are two possible values that will be
	''' placed in the AttributeSet's value:
	''' <ul>
	''' <li>If the DTD does not contain an definition for the element, or the
	'''     definition does not have an explicit value then the value in the
	'''     AttributeSet will be <code>HTML.NULL_ATTRIBUTE_VALUE</code>.
	''' <li>If the DTD contains an explicit value, as in:
	'''     <code>&lt;!ATTLIST OPTION selected (selected) #IMPLIED&gt;</code>
	'''     this value from the dtd (in this case selected) will be used.
	''' </ul>
	''' <p>
	''' Once the stream has been parsed, the callback is notified of the most
	''' likely end of line string. The end of line string will be one of
	''' \n, \r or \r\n, which ever is encountered the most in parsing the
	''' stream.
	''' 
	''' @author      Sunita Mani
	''' </summary>
	Public Class DocumentParser
		Inherits javax.swing.text.html.parser.Parser

		Private inbody As Integer
		Private intitle As Integer
		Private inhead As Integer
		Private instyle As Integer
		Private inscript As Integer
		Private seentitle As Boolean
		Private callback As javax.swing.text.html.HTMLEditorKit.ParserCallback = Nothing
		Private ignoreCharSet As Boolean = False
		Private Const debugFlag As Boolean = False

		Public Sub New(ByVal dtd As DTD)
			MyBase.New(dtd)
		End Sub

		Public Overridable Sub parse(ByVal [in] As Reader, ByVal callback As javax.swing.text.html.HTMLEditorKit.ParserCallback, ByVal ignoreCharSet As Boolean)
			Me.ignoreCharSet = ignoreCharSet
			Me.callback = callback
			parse([in])
			' end of line
			callback.handleEndOfLineString(endOfLineString)
		End Sub

		''' <summary>
		''' Handle Start Tag.
		''' </summary>
		Protected Friend Overrides Sub handleStartTag(ByVal tag As TagElement)

			Dim elem As Element = tag.element
			If elem Is dtd.body Then
				inbody += 1
			ElseIf elem Is dtd.html Then
			ElseIf elem Is dtd.head Then
				inhead += 1
			ElseIf elem Is dtd.title Then
				intitle += 1
			ElseIf elem Is dtd.style Then
				instyle += 1
			ElseIf elem Is dtd.script Then
				inscript += 1
			End If
			If debugFlag Then
				If tag.fictional() Then
					debug("Start Tag: " & tag.hTMLTag & " pos: " & currentPos)
				Else
					debug("Start Tag: " & tag.hTMLTag & " attributes: " & attributes & " pos: " & currentPos)
				End If
			End If
			If tag.fictional() Then
				Dim attrs As New javax.swing.text.SimpleAttributeSet
				attrs.addAttribute(javax.swing.text.html.HTMLEditorKit.ParserCallback.IMPLIED, Boolean.TRUE)
				callback.handleStartTag(tag.hTMLTag, attrs, blockStartPosition)
			Else
				callback.handleStartTag(tag.hTMLTag, attributes, blockStartPosition)
				flushAttributes()
			End If
		End Sub


		Protected Friend Overrides Sub handleComment(ByVal text As Char())
			If debugFlag Then debug("comment: ->" & New String(text) & "<-" & " pos: " & currentPos)
			callback.handleComment(text, blockStartPosition)
		End Sub

		''' <summary>
		''' Handle Empty Tag.
		''' </summary>
		Protected Friend Overrides Sub handleEmptyTag(ByVal tag As TagElement)

			Dim elem As Element = tag.element
			If elem Is dtd.meta AndAlso (Not ignoreCharSet) Then
				Dim atts As javax.swing.text.SimpleAttributeSet = attributes
				If atts IsNot Nothing Then
					Dim content As String = CStr(atts.getAttribute(javax.swing.text.html.HTML.Attribute.CONTENT))
					If content IsNot Nothing Then
						If "content-type".equalsIgnoreCase(CStr(atts.getAttribute(javax.swing.text.html.HTML.Attribute.HTTPEQUIV))) Then
							If (Not content.ToUpper()) = "text/html".ToUpper() AndAlso (Not content.ToUpper()) = "text/plain".ToUpper() Then Throw New javax.swing.text.ChangedCharSetException(content, False)
						ElseIf "charset".equalsIgnoreCase(CStr(atts.getAttribute(javax.swing.text.html.HTML.Attribute.HTTPEQUIV))) Then
							Throw New javax.swing.text.ChangedCharSetException(content, True)
						End If
					End If
				End If
			End If
			If inbody <> 0 OrElse elem Is dtd.meta OrElse elem Is dtd.base OrElse elem Is dtd.isindex OrElse elem Is dtd.style OrElse elem Is dtd.link Then
				If debugFlag Then
					If tag.fictional() Then
						debug("Empty Tag: " & tag.hTMLTag & " pos: " & currentPos)
					Else
						debug("Empty Tag: " & tag.hTMLTag & " attributes: " & attributes & " pos: " & currentPos)
					End If
				End If
				If tag.fictional() Then
					Dim attrs As New javax.swing.text.SimpleAttributeSet
					attrs.addAttribute(javax.swing.text.html.HTMLEditorKit.ParserCallback.IMPLIED, Boolean.TRUE)
					callback.handleSimpleTag(tag.hTMLTag, attrs, blockStartPosition)
				Else
					callback.handleSimpleTag(tag.hTMLTag, attributes, blockStartPosition)
					flushAttributes()
				End If
			End If
		End Sub

		''' <summary>
		''' Handle End Tag.
		''' </summary>
		Protected Friend Overrides Sub handleEndTag(ByVal tag As TagElement)
			Dim elem As Element = tag.element
			If elem Is dtd.body Then
				inbody -= 1
			ElseIf elem Is dtd.title Then
				intitle -= 1
				seentitle = True
			ElseIf elem Is dtd.head Then
				inhead -= 1
			ElseIf elem Is dtd.style Then
				instyle -= 1
			ElseIf elem Is dtd.script Then
				inscript -= 1
			End If
			If debugFlag Then debug("End Tag: " & tag.hTMLTag & " pos: " & currentPos)
			callback.handleEndTag(tag.hTMLTag, blockStartPosition)

		End Sub

		''' <summary>
		''' Handle Text.
		''' </summary>
		Protected Friend Overrides Sub handleText(ByVal data As Char())
			If data IsNot Nothing Then
				If inscript <> 0 Then
					callback.handleComment(data, blockStartPosition)
					Return
				End If
				If inbody <> 0 OrElse ((instyle <> 0) OrElse ((intitle <> 0) AndAlso (Not seentitle))) Then
					If debugFlag Then debug("text:  ->" & New String(data) & "<-" & " pos: " & currentPos)
					callback.handleText(data, blockStartPosition)
				End If
			End If
		End Sub

	'    
	'     * Error handling.
	'     
		Protected Friend Overrides Sub handleError(ByVal ln As Integer, ByVal errorMsg As String)
			If debugFlag Then debug("Error: ->" & errorMsg & "<-" & " pos: " & currentPos)
			' PENDING: need to improve the error string. 
			callback.handleError(errorMsg, currentPos)
		End Sub


	'    
	'     * debug messages
	'     
		Private Sub debug(ByVal msg As String)
			Console.WriteLine(msg)
		End Sub
	End Class

End Namespace
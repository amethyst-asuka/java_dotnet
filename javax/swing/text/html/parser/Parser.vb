Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading

'
' * Copyright (c) 1998, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A simple DTD-driven HTML parser. The parser reads an
	''' HTML file from an InputStream and calls various methods
	''' (which should be overridden in a subclass) when tags and
	''' data are encountered.
	''' <p>
	''' Unfortunately there are many badly implemented HTML parsers
	''' out there, and as a result there are many badly formatted
	''' HTML files. This parser attempts to parse most HTML files.
	''' This means that the implementation sometimes deviates from
	''' the SGML specification in favor of HTML.
	''' <p>
	''' The parser treats \r and \r\n as \n. Newlines after starttags
	''' and before end tags are ignored just as specified in the SGML/HTML
	''' specification.
	''' <p>
	''' The html spec does not specify how spaces are to be coalesced very well.
	''' Specifically, the following scenarios are not discussed (note that a
	''' space should be used here, but I am using &amp;nbsp to force the space to
	''' be displayed):
	''' <p>
	''' '&lt;b&gt;blah&nbsp;&lt;i&gt;&nbsp;&lt;strike&gt;&nbsp;foo' which can be treated as:
	''' '&lt;b&gt;blah&nbsp;&lt;i&gt;&lt;strike&gt;foo'
	''' <p>as well as:
	''' '&lt;p&gt;&lt;a href="xx"&gt;&nbsp;&lt;em&gt;Using&lt;/em&gt;&lt;/a&gt;&lt;/p&gt;'
	''' which appears to be treated as:
	''' '&lt;p&gt;&lt;a href="xx"&gt;&lt;em&gt;Using&lt;/em&gt;&lt;/a&gt;&lt;/p&gt;'
	''' <p>
	''' If <code>strict</code> is false, when a tag that breaks flow,
	''' (<code>TagElement.breaksFlows</code>) or trailing whitespace is
	''' encountered, all whitespace will be ignored until a non whitespace
	''' character is encountered. This appears to give behavior closer to
	''' the popular browsers.
	''' </summary>
	''' <seealso cref= DTD </seealso>
	''' <seealso cref= TagElement </seealso>
	''' <seealso cref= SimpleAttributeSet
	''' @author Arthur van Hoff
	''' @author Sunita Mani </seealso>
	Public Class Parser
		Implements DTDConstants

		Private text As Char() = New Char(1023){}
		Private textpos As Integer = 0
		Private last As TagElement
		Private space As Boolean

		Private str As Char() = New Char(127){}
		Private strpos As Integer = 0

		Protected Friend dtd As DTD = Nothing

		Private ch As Integer
		Private ln As Integer
		Private [in] As Reader

		Private recent As Element
		Private stack As TagStack
		Private skipTag As Boolean = False
		Private lastFormSent As TagElement = Nothing
		Private attributes As New javax.swing.text.SimpleAttributeSet

		' State for <html>, <head> and <body>.  Since people like to slap
		' together HTML documents without thinking, occasionally they
		' have multiple instances of these tags.  These booleans track
		' the first sightings of these tags so they can be safely ignored
		' by the parser if repeated.
		Private seenHtml As Boolean = False
		Private seenHead As Boolean = False
		Private seenBody As Boolean = False

		''' <summary>
		''' The html spec does not specify how spaces are coalesced very well.
		''' If strict == false, ignoreSpace is used to try and mimic the behavior
		''' of the popular browsers.
		''' <p>
		''' The problematic scenarios are:
		''' '&lt;b>blah &lt;i> &lt;strike> foo' which can be treated as:
		''' '&lt;b>blah &lt;i>&lt;strike>foo'
		''' as well as:
		''' '&lt;p>&lt;a href="xx"> &lt;em>Using&lt;/em>&lt;/a>&lt;/p>'
		''' which appears to be treated as:
		''' '&lt;p>&lt;a href="xx">&lt;em>Using&lt;/em>&lt;/a>&lt;/p>'
		''' <p>
		''' When a tag that breaks flow, or trailing whitespace is encountered
		''' ignoreSpace is set to true. From then on, all whitespace will be
		''' ignored.
		''' ignoreSpace will be set back to false the first time a
		''' non whitespace character is encountered. This appears to give
		''' behavior closer to the popular browsers.
		''' </summary>
		Private ignoreSpace As Boolean

		''' <summary>
		''' This flag determines whether or not the Parser will be strict
		''' in enforcing SGML compatibility.  If false, it will be lenient
		''' with certain common classes of erroneous HTML constructs.
		''' Strict or not, in either case an error will be recorded.
		''' 
		''' </summary>
		Protected Friend [strict] As Boolean = False


		''' <summary>
		''' Number of \r\n's encountered. </summary>
		Private crlfCount As Integer
		''' <summary>
		''' Number of \r's encountered. A \r\n will not increment this. </summary>
		Private crCount As Integer
		''' <summary>
		''' Number of \n's encountered. A \r\n will not increment this. </summary>
		Private lfCount As Integer

		'
		' To correctly identify the start of a tag/comment/text we need two
		' ivars. Two are needed as handleText isn't invoked until the tag
		' after the text has been parsed, that is the parser parses the text,
		' then a tag, then invokes handleText followed by handleStart.
		'
		''' <summary>
		''' The start position of the current block. Block is overloaded here,
		''' it really means the current start position for the current comment,
		''' tag, text. Use getBlockStartPosition to access this. 
		''' </summary>
		Private currentBlockStartPos As Integer
		''' <summary>
		''' Start position of the last block. </summary>
		Private lastBlockStartPos As Integer

		''' <summary>
		''' array for mapping numeric references in range
		''' 130-159 to displayable Unicode characters.
		''' </summary>
		Private Shared ReadOnly cp1252Map As Char() = { 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 352, 8249, 338, 141, 142, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 8482, 353, 8250, 339, 157, 158, 376 }

		Public Sub New(ByVal dtd As DTD)
			Me.dtd = dtd
		End Sub


		''' <returns> the line number of the line currently being parsed </returns>
		Protected Friend Overridable Property currentLine As Integer
			Get
				Return ln
			End Get
		End Property

		''' <summary>
		''' Returns the start position of the current block. Block is
		''' overloaded here, it really means the current start position for
		''' the current comment tag, text, block.... This is provided for
		''' subclassers that wish to know the start of the current block when
		''' called with one of the handleXXX methods.
		''' </summary>
		Friend Overridable Property blockStartPosition As Integer
			Get
				Return Math.Max(0, lastBlockStartPos - 1)
			End Get
		End Property

		''' <summary>
		''' Makes a TagElement.
		''' </summary>
		Protected Friend Overridable Function makeTag(ByVal elem As Element, ByVal fictional As Boolean) As TagElement
			Return New TagElement(elem, fictional)
		End Function

		Protected Friend Overridable Function makeTag(ByVal elem As Element) As TagElement
			Return makeTag(elem, False)
		End Function

		Protected Friend Overridable Property attributes As javax.swing.text.SimpleAttributeSet
			Get
				Return attributes
			End Get
		End Property

		Protected Friend Overridable Sub flushAttributes()
			attributes.removeAttributes(attributes)
		End Sub

		''' <summary>
		''' Called when PCDATA is encountered.
		''' </summary>
		Protected Friend Overridable Sub handleText(ByVal text As Char())
		End Sub

		''' <summary>
		''' Called when an HTML title tag is encountered.
		''' </summary>
		Protected Friend Overridable Sub handleTitle(ByVal text As Char())
			' default behavior is to call handleText. Subclasses
			' can override if necessary.
			handleText(text)
		End Sub

		''' <summary>
		''' Called when an HTML comment is encountered.
		''' </summary>
		Protected Friend Overridable Sub handleComment(ByVal text As Char())
		End Sub

		Protected Friend Overridable Sub handleEOFInComment()
			' We've reached EOF.  Our recovery strategy is to
			' see if we have more than one line in the comment;
			' if so, we pretend that the comment was an unterminated
			' single line comment, and reparse the lines after the
			' first line as normal HTML content.

			Dim commentEndPos As Integer = strIndexOf(ControlChars.Lf)
			If commentEndPos >= 0 Then
				handleComment(getChars(0, commentEndPos))
				Try
					[in].close()
					[in] = New CharArrayReader(getChars(commentEndPos + 1))
					ch = AscW(">"c)
				Catch e As IOException
					[error]("ioexception")
				End Try

				resetStrBuffer()
			Else
				' no newline, so signal an error
				[error]("eof.comment")
			End If
		End Sub

		''' <summary>
		''' Called when an empty tag is encountered.
		''' </summary>
		Protected Friend Overridable Sub handleEmptyTag(ByVal tag As TagElement)
		End Sub

		''' <summary>
		''' Called when a start tag is encountered.
		''' </summary>
		Protected Friend Overridable Sub handleStartTag(ByVal tag As TagElement)
		End Sub

		''' <summary>
		''' Called when an end tag is encountered.
		''' </summary>
		Protected Friend Overridable Sub handleEndTag(ByVal tag As TagElement)
		End Sub

		''' <summary>
		''' An error has occurred.
		''' </summary>
		Protected Friend Overridable Sub handleError(ByVal ln As Integer, ByVal msg As String)
	'        
	'        Thread.dumpStack();
	'        System.out.println("**** " + stack);
	'        System.out.println("line " + ln + ": error: " + msg);
	'        System.out.println();
	'        
		End Sub

		''' <summary>
		''' Output text.
		''' </summary>
		Friend Overridable Sub handleText(ByVal tag As TagElement)
			If tag.breaksFlow() Then
				space = False
				If Not [strict] Then ignoreSpace = True
			End If
			If textpos = 0 Then
				If ((Not space)) OrElse (stack Is Nothing) OrElse last.breaksFlow() OrElse (Not stack.advance(dtd.pcdata)) Then
					last = tag
					space = False
					lastBlockStartPos = currentBlockStartPos
					Return
				End If
			End If
			If space Then
				If Not ignoreSpace Then
					' enlarge buffer if needed
					If textpos + 1 > text.Length Then
						Dim newtext As Char() = New Char(text.Length + 200 - 1){}
						Array.Copy(text, 0, newtext, 0, text.Length)
						text = newtext
					End If

					' output pending space
					text(textpos) = " "c
					textpos += 1
					If (Not [strict]) AndAlso (Not tag.element.empty) Then ignoreSpace = True
				End If
				space = False
			End If
			Dim newtext As Char() = New Char(textpos - 1){}
			Array.Copy(text, 0, newtext, 0, textpos)
			' Handles cases of bad html where the title tag
			' was getting lost when we did error recovery.
			If tag.element.name.Equals("title") Then
				handleTitle(newtext)
			Else
				handleText(newtext)
			End If
			lastBlockStartPos = currentBlockStartPos
			textpos = 0
			last = tag
			space = False
		End Sub

		''' <summary>
		''' Invoke the error handler.
		''' </summary>
		Protected Friend Overridable Sub [error](ByVal err As String, ByVal arg1 As String, ByVal arg2 As String, ByVal arg3 As String)
			handleError(ln, err & " " & arg1 & " " & arg2 & " " & arg3)
		End Sub

		Protected Friend Overridable Sub [error](ByVal err As String, ByVal arg1 As String, ByVal arg2 As String)
			[error](err, arg1, arg2, "?")
		End Sub
		Protected Friend Overridable Sub [error](ByVal err As String, ByVal arg1 As String)
			[error](err, arg1, "?", "?")
		End Sub
		Protected Friend Overridable Sub [error](ByVal err As String)
			[error](err, "?", "?", "?")
		End Sub


		''' <summary>
		''' Handle a start tag. The new tag is pushed
		''' onto the tag stack. The attribute list is
		''' checked for required attributes.
		''' </summary>
		Protected Friend Overridable Sub startTag(ByVal tag As TagElement)
			Dim elem As Element = tag.element

			' If the tag is an empty tag and texpos != 0
			' this implies that there is text before the
			' start tag that needs to be processed before
			' handling the tag.
			'
			If (Not elem.empty) OrElse ((last IsNot Nothing) AndAlso (Not last.breaksFlow())) OrElse (textpos <> 0) Then
				handleText(tag)
			Else
				' this variable gets updated in handleText().
				' Since in this case we do not call handleText()
				' we need to update it here.
				'
				last = tag
				' Note that we should really check last.breakFlows before
				' assuming this should be false.
				space = False
			End If
			lastBlockStartPos = currentBlockStartPos

			' check required attributes
			Dim a As AttributeList = elem.atts
			Do While a IsNot Nothing
				If (a.modifier = REQUIRED) AndAlso ((attributes.empty) OrElse (((Not attributes.isDefined(a.name))) AndAlso ((Not attributes.isDefined(javax.swing.text.html.HTML.getAttributeKey(a.name)))))) Then [error]("req.att ", a.name, elem.name)
				a = a.next
			Loop

			If elem.empty Then
				handleEmptyTag(tag)
	'            
	'        } else if (elem.getName().equals("form")) {
	'            handleStartTag(tag);
	'            
			Else
				recent = elem
				stack = New TagStack(tag, stack)
				handleStartTag(tag)
			End If
		End Sub

		''' <summary>
		''' Handle an end tag. The end tag is popped
		''' from the tag stack.
		''' </summary>
		Protected Friend Overridable Sub endTag(ByVal omitted As Boolean)
			handleText(stack.tag)

			If omitted AndAlso (Not stack.elem.omitEnd()) Then
				[error]("end.missing", stack.elem.name)
			ElseIf Not stack.terminate() Then
				[error]("end.unexpected", stack.elem.name)
			End If

			' handle the tag
			handleEndTag(stack.tag)
			stack = stack.next
			recent = If(stack IsNot Nothing, stack.elem, Nothing)
		End Sub


		Friend Overridable Function ignoreElement(ByVal elem As Element) As Boolean

			Dim stackElement As String = stack.elem.name
			Dim elemName As String = elem.name
	'         We ignore all elements that are not valid in the context of
	'           a table except <td>, <th> (these we handle in
	'           legalElementContext()) and #pcdata.  We also ignore the
	'           <font> tag in the context of <ul> and <ol> We additonally
	'           ignore the <meta> and the <style> tag if the body tag has
	'           been seen. *
			If (elemName.Equals("html") AndAlso seenHtml) OrElse (elemName.Equals("head") AndAlso seenHead) OrElse (elemName.Equals("body") AndAlso seenBody) Then Return True
			If elemName.Equals("dt") OrElse elemName.Equals("dd") Then
				Dim s As TagStack = stack
				Do While s IsNot Nothing AndAlso Not s.elem.name.Equals("dl")
					s = s.next
				Loop
				If s Is Nothing Then Return True
			End If

			If ((stackElement.Equals("table")) AndAlso ((Not elemName.Equals("#pcdata"))) AndAlso ((Not elemName.Equals("input")))) OrElse ((elemName.Equals("font")) AndAlso (stackElement.Equals("ul") OrElse stackElement.Equals("ol"))) OrElse (elemName.Equals("meta") AndAlso stack IsNot Nothing) OrElse (elemName.Equals("style") AndAlso seenBody) OrElse (stackElement.Equals("table") AndAlso elemName.Equals("a")) Then Return True
			Return False
		End Function


		''' <summary>
		''' Marks the first time a tag has been seen in a document
		''' </summary>

		Protected Friend Overridable Sub markFirstTime(ByVal elem As Element)
			Dim elemName As String = elem.name
			If elemName.Equals("html") Then
				seenHtml = True
			ElseIf elemName.Equals("head") Then
				seenHead = True
			ElseIf elemName.Equals("body") Then
				If buf.Length = 1 Then
					' Refer to note in definition of buf for details on this.
					Dim newBuf As Char() = New Char(255){}

					newBuf(0) = buf(0)
					buf = newBuf
				End If
				seenBody = True
			End If
		End Sub

		''' <summary>
		''' Create a legal content for an element.
		''' </summary>
		Friend Overridable Function legalElementContext(ByVal elem As Element) As Boolean

			' System.out.println("-- legalContext -- " + elem);

			' Deal with the empty stack
			If stack Is Nothing Then
				' System.out.println("-- stack is empty");
				If elem IsNot dtd.html Then
					' System.out.println("-- pushing html");
					startTag(makeTag(dtd.html, True))
					Return legalElementContext(elem)
				End If
				Return True
			End If

			' Is it allowed in the current context
			If stack.advance(elem) Then
				' System.out.println("-- legal context");
				markFirstTime(elem)
				Return True
			End If
			Dim insertTag As Boolean = False

			' The use of all error recovery strategies are contingent
			' on the value of the strict property.
			'
			' These are commonly occurring errors.  if insertTag is true,
			' then we want to adopt an error recovery strategy that
			' involves attempting to insert an additional tag to
			' legalize the context.  The two errors addressed here
			' are:
			' 1) when a <td> or <th> is seen soon after a <table> tag.
			'    In this case we insert a <tr>.
			' 2) when any other tag apart from a <tr> is seen
			'    in the context of a <tr>.  In this case we would
			'    like to add a <td>.  If a <tr> is seen within a
			'    <tr> context, then we will close out the current
			'    <tr>.
			'
			' This insertion strategy is handled later in the method.
			' The reason for checking this now, is that in other cases
			' we would like to apply other error recovery strategies for example
			' ignoring tags.
			'
			' In certain cases it is better to ignore a tag than try to
			' fix the situation.  So the first test is to see if this
			' is what we need to do.
			'
			Dim stackElemName As String = stack.elem.name
			Dim elemName As String = elem.name


			If (Not [strict]) AndAlso ((stackElemName.Equals("table") AndAlso elemName.Equals("td")) OrElse (stackElemName.Equals("table") AndAlso elemName.Equals("th")) OrElse (stackElemName.Equals("tr") AndAlso (Not elemName.Equals("tr")))) Then insertTag = True


			If (Not [strict]) AndAlso (Not insertTag) AndAlso (stack.elem.name <> elem.name OrElse elem.name.Equals("body")) Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				If skipTag = ignoreElement(elem) Then
					[error]("tag.ignore", elem.name)
					Return skipTag
				End If
			End If

			' Check for anything after the start of the table besides tr, td, th
			' or caption, and if those aren't there, insert the <tr> and call
			' legalElementContext again.
			If (Not [strict]) AndAlso stackElemName.Equals("table") AndAlso (Not elemName.Equals("tr")) AndAlso (Not elemName.Equals("td")) AndAlso (Not elemName.Equals("th")) AndAlso (Not elemName.Equals("caption")) Then
				Dim e As Element = dtd.getElement("tr")
				Dim t As TagElement = makeTag(e, True)
				legalTagContext(t)
				startTag(t)
				[error]("start.missing", elem.name)
				Return legalElementContext(elem)
			End If

			' They try to find a legal context by checking if the current
			' tag is valid in an enclosing context.  If so
			' close out the tags by outputing end tags and then
			' insert the current tag.  If the tags that are
			' being closed out do not have an optional end tag
			' specification in the DTD then an html error is
			' reported.
			'
			If (Not insertTag) AndAlso stack.terminate() AndAlso ((Not [strict]) OrElse stack.elem.omitEnd()) Then
				Dim s As TagStack = stack.next
				Do While s IsNot Nothing
					If s.advance(elem) Then
						Do While stack IsNot s
							endTag(True)
						Loop
						Return True
					End If
					If (Not s.terminate()) OrElse ([strict] AndAlso (Not s.elem.omitEnd())) Then Exit Do
					s = s.next
				Loop
			End If

			' Check if we know what tag is expected next.
			' If so insert the tag.  Report an error if the
			' tag does not have its start tag spec in the DTD as optional.
			'
			Dim [next] As Element = stack.first()
			If [next] IsNot Nothing AndAlso ((Not [strict]) OrElse [next].omitStart()) AndAlso Not([next] Is dtd.head AndAlso elem Is dtd.pcdata) Then
				' System.out.println("-- omitting start tag: " + next);
				Dim t As TagElement = makeTag([next], True)
				legalTagContext(t)
				startTag(t)
				If Not [next].omitStart() Then [error]("start.missing", elem.name)
				Return legalElementContext(elem)
			End If


			' Traverse the list of expected elements and determine if adding
			' any of these elements would make for a legal context.
			'

			If Not [strict] Then
				Dim content As ContentModel = stack.contentModel()
				Dim elemVec As New List(Of Element)
				If content IsNot Nothing Then
					content.getElements(elemVec)
					For Each e As Element In elemVec
						' Ensure that this element has not been included as
						' part of the exclusions in the DTD.
						'
						If stack.excluded(e.index) Then Continue For

						Dim reqAtts As Boolean = False

						Dim a As AttributeList = e.attributes
						Do While a IsNot Nothing
							If a.modifier = REQUIRED Then
								reqAtts = True
								Exit Do
							End If
							a = a.next
						Loop
						' Ensure that no tag that has required attributes
						' gets inserted.
						'
						If reqAtts Then Continue For

						Dim m As ContentModel = e.content
						If m IsNot Nothing AndAlso m.first(elem) Then
							' System.out.println("-- adding a legal tag: " + e);
							Dim t As TagElement = makeTag(e, True)
							legalTagContext(t)
							startTag(t)
							[error]("start.missing", e.name)
							Return legalElementContext(elem)
						End If
					Next e
				End If
			End If

			' Check if the stack can be terminated.  If so add the appropriate
			' end tag.  Report an error if the tag being ended does not have its
			' end tag spec in the DTD as optional.
			'
			If stack.terminate() AndAlso (stack.elem IsNot dtd.body) AndAlso ((Not [strict]) OrElse stack.elem.omitEnd()) Then
				' System.out.println("-- omitting end tag: " + stack.elem);
				If Not stack.elem.omitEnd() Then [error]("end.missing", elem.name)

				endTag(True)
				Return legalElementContext(elem)
			End If

			' At this point we know that something is screwed up.
			Return False
		End Function

		''' <summary>
		''' Create a legal context for a tag.
		''' </summary>
		Friend Overridable Sub legalTagContext(ByVal tag As TagElement)
			If legalElementContext(tag.element) Then
				markFirstTime(tag.element)
				Return
			End If

			' Avoid putting a block tag in a flow tag.
			If tag.breaksFlow() AndAlso (stack IsNot Nothing) AndAlso (Not stack.tag.breaksFlow()) Then
				endTag(True)
				legalTagContext(tag)
				Return
			End If

			' Avoid putting something wierd in the head of the document.
			Dim s As TagStack = stack
			Do While s IsNot Nothing
				If s.tag.element Is dtd.head Then
					Do While stack IsNot s
						endTag(True)
					Loop
					endTag(True)
					legalTagContext(tag)
					Return
				End If
				s = s.next
			Loop

			' Everything failed
			[error]("tag.unexpected", tag.element.name)
		End Sub

		''' <summary>
		''' Error context. Something went wrong, make sure we are in
		''' the document's body context
		''' </summary>
		Friend Overridable Sub errorContext()
			Do While (stack IsNot Nothing) AndAlso (stack.tag.element IsNot dtd.body)
				handleEndTag(stack.tag)
				stack = stack.next
			Loop
			If stack Is Nothing Then
				legalElementContext(dtd.body)
				startTag(makeTag(dtd.body, True))
			End If
		End Sub

		''' <summary>
		''' Add a char to the string buffer.
		''' </summary>
		Friend Overridable Sub addString(ByVal c As Integer)
			If strpos = str.Length Then
				Dim newstr As Char() = New Char(str.Length + 128 - 1){}
				Array.Copy(str, 0, newstr, 0, str.Length)
				str = newstr
			End If
			str(strpos) = ChrW(c)
			strpos += 1
		End Sub

		''' <summary>
		''' Get the string that's been accumulated.
		''' </summary>
		Friend Overridable Function getString(ByVal pos As Integer) As String
			Dim newStr As Char() = New Char(strpos - pos - 1){}
			Array.Copy(str, pos, newStr, 0, strpos - pos)
			strpos = pos
			Return New String(newStr)
		End Function

		Friend Overridable Function getChars(ByVal pos As Integer) As Char()
			Dim newStr As Char() = New Char(strpos - pos - 1){}
			Array.Copy(str, pos, newStr, 0, strpos - pos)
			strpos = pos
			Return newStr
		End Function

		Friend Overridable Function getChars(ByVal pos As Integer, ByVal endPos As Integer) As Char()
			Dim newStr As Char() = New Char(endPos - pos - 1){}
			Array.Copy(str, pos, newStr, 0, endPos - pos)
			' REMIND: it's not clear whether this version should set strpos or not
			' strpos = pos;
			Return newStr
		End Function

		Friend Overridable Sub resetStrBuffer()
			strpos = 0
		End Sub

		Friend Overridable Function strIndexOf(ByVal target As Char) As Integer
			For i As Integer = 0 To strpos - 1
				If str(i) = target Then Return i
			Next i

			Return -1
		End Function

		''' <summary>
		''' Skip space.
		''' [5] 297:5
		''' </summary>
		Friend Overridable Sub skipSpace()
			Do
				Select Case ch
				  Case ControlChars.Lf
					ln += 1
					ch = readCh()
					lfCount += 1

				  Case ControlChars.Cr
					ln += 1
					ch = readCh()
					If ch = ControlChars.Lf Then
						ch = readCh()
						crlfCount += 1
					Else
						crCount += 1
					End If
				  Case " "c, ControlChars.Tab
					ch = readCh()

				  Case Else
					Return
				End Select
			Loop
		End Sub

		''' <summary>
		''' Parse identifier. Uppercase characters are folded
		''' to lowercase when lower is true. Returns falsed if
		''' no identifier is found. [55] 346:17
		''' </summary>
		Friend Overridable Function parseIdentifier(ByVal lower As Boolean) As Boolean
			Select Case ch
			  Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c
				If lower Then ch = AscW("a"c) + (ch - AscW("A"c))

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			  Case "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c

			  Case Else
				Return False
			End Select

			Do
				addString(ch)

'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Select Case ch = readCh()
				  Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c
					If lower Then ch = AscW("a"c) + (ch - AscW("A"c))

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				  Case "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "."c, "-"c, "_"c




				  Case Else
					Return True
				End Select
			Loop
		End Function

		''' <summary>
		''' Parse an entity reference. [59] 350:17
		''' </summary>
		Private Function parseEntityReference() As Char()
			Dim pos As Integer = strpos

			ch = readCh()
			If ch = AscW("#"c) Then
				Dim n As Integer = 0
				ch = readCh()
				If (ch >= "0"c) AndAlso (ch <= "9"c) OrElse ch = AscW("x"c) OrElse ch = AscW("X"c) Then

					If (ch >= "0"c) AndAlso (ch <= "9"c) Then
						' parse decimal reference
						Do While (ch >= "0"c) AndAlso (ch <= "9"c)
							n = (n * 10) + ch - AscW("0"c)
							ch = readCh()
						Loop
					Else
						' parse hexadecimal reference
						ch = readCh()
						Dim lch As Char = CChar(Char.ToLower(ch))
						Do While (lch >= "0"c) AndAlso (lch <= "9"c) OrElse (lch >= "a"c) AndAlso (lch <= "f"c)
							If lch >= "0"c AndAlso lch <= "9"c Then
								n = (n * 16) + AscW(lch) - AscW("0"c)
							Else
								n = (n * 16) + AscW(lch) - AscW("a"c) + 10
							End If
							ch = readCh()
							lch = CChar(Char.ToLower(ch))
						Loop
					End If
					Select Case ch
						Case ControlChars.Lf
							ln += 1
							ch = readCh()
							lfCount += 1

						Case ControlChars.Cr
							ln += 1
							ch = readCh()
							If ch = ControlChars.Lf Then
								ch = readCh()
								crlfCount += 1
							Else
								crCount += 1
							End If

						Case ";"c
							ch = readCh()
					End Select
					Dim data As Char() = mapNumericReference(n)
					Return data
				End If
				addString("#"c)
				If Not parseIdentifier(False) Then
					[error]("ident.expected")
					strpos = pos
					Dim data As Char() = {"&"c, "#"c}
					Return data
				End If
			ElseIf Not parseIdentifier(False) Then
				Dim data As Char() = {"&"c}
				Return data
			End If

			Dim semicolon As Boolean = False

			Select Case ch
			  Case ControlChars.Lf
				ln += 1
				ch = readCh()
				lfCount += 1

			  Case ControlChars.Cr
				ln += 1
				ch = readCh()
				If ch = ControlChars.Lf Then
					ch = readCh()
					crlfCount += 1
				Else
					crCount += 1
				End If

			  Case ";"c
				semicolon = True

				ch = readCh()
			End Select

			Dim nm As String = getString(pos)
			Dim ent As Entity = dtd.getEntity(nm)

			' entities are case sensitive - however if strict
			' is false then we will try to make a match by
			' converting the string to all lowercase.
			'
			If (Not [strict]) AndAlso (ent Is Nothing) Then ent = dtd.getEntity(nm.ToLower())
			If (ent Is Nothing) OrElse (Not ent.general) Then

				If nm.Length = 0 Then
					[error]("invalid.entref", nm)
					Return New Char(){}
				End If
				' given that there is not a match restore the entity reference 
				Dim str As String = "&" & nm + (If(semicolon, ";", ""))

				Dim b As Char() = New Char(str.Length - 1){}
				str.getChars(0, b.Length, b, 0)
				Return b
			End If
			Return ent.data
		End Function

		''' <summary>
		''' Converts numeric character reference to char array.
		''' 
		''' Normally the code in a reference should be always converted
		''' to the Unicode character with the same code, but due to
		''' wide usage of Cp1252 charset most browsers map numeric references
		''' in the range 130-159 (which are control chars in Unicode set)
		''' to displayable characters with other codes.
		''' </summary>
		''' <param name="c"> the code of numeric character reference. </param>
		''' <returns> a char array corresponding to the reference code. </returns>
		Private Function mapNumericReference(ByVal c As Integer) As Char()
			Dim data As Char()
			If c >= &Hffff Then ' outside unicode BMP.
				Try
					data = Char.toChars(c)
				Catch e As System.ArgumentException
					data = New Char(){}
				End Try
			Else
				data = New Char(0){}
				data(0) = If(c < 130 OrElse c > 159, ChrW(c), cp1252Map(c - 130))
			End If
			Return data
		End Function

		''' <summary>
		''' Parse a comment. [92] 391:7
		''' </summary>
		Friend Overridable Sub parseComment()

			Do
				Dim c As Integer = ch
				Select Case c
				  Case "-"c
					  ''' <summary>
					  ''' Presuming that the start string of a comment "<!--" has
					  '''    already been parsed, the '-' character is valid only as
					  '''    part of a comment termination and further more it must
					  '''    be present in even numbers. Hence if strict is true, we
					  '''    presume the comment has been terminated and return.
					  '''    However if strict is false, then there is no even number
					  '''    requirement and this character can appear anywhere in the
					  '''    comment.  The parser reads on until it sees the following
					  '''    pattern: "-->" or "--!>".
					  ''' 
					  ''' </summary>
					If (Not [strict]) AndAlso (strpos <> 0) AndAlso (str(strpos - 1) = "-"c) Then
						ch = readCh()
						If ch = AscW(">"c) Then Return
						If ch = AscW("!"c) Then
							ch = readCh()
							If ch = AscW(">"c) Then
								Return
							Else
								' to account for extra read()'s that happened 
								addString("-"c)
								addString("!"c)
								Continue Do
							End If
						End If
						Exit Select
					End If

					ch = readCh()
					If ch = AscW("-"c) Then
						ch = readCh()
						If [strict] OrElse ch = AscW(">"c) Then Return
						If ch = AscW("!"c) Then
							ch = readCh()
							If ch = AscW(">"c) Then
								Return
							Else
								' to account for extra read()'s that happened 
								addString("-"c)
								addString("!"c)
								Continue Do
							End If
						End If
						' to account for the extra read() 
						addString("-"c)
					End If

				  Case -1
					  handleEOFInComment()
					  Return

				  Case ControlChars.Lf
					ln += 1
					ch = readCh()
					lfCount += 1

				  Case ">"c
					ch = readCh()

				  Case ControlChars.Cr
					ln += 1
					ch = readCh()
					If ch = ControlChars.Lf Then
						ch = readCh()
						crlfCount += 1
					Else
						crCount += 1
					End If
					c = ControlChars.Lf
				  Case Else
					ch = readCh()
				End Select

				addString(c)
			Loop
		End Sub

		''' <summary>
		''' Parse literal content. [46] 343:1 and [47] 344:1
		''' </summary>
		Friend Overridable Sub parseLiteral(ByVal replace As Boolean)
			Do
				Dim c As Integer = ch
				Select Case c
				  Case -1
					[error]("eof.literal", stack.elem.name)
					endTag(True)
					Return

				  Case ">"c
					ch = readCh()
					Dim i As Integer = textpos - (stack.elem.name.length() + 2), j As Integer = 0

					' match end tag
					Dim tempVar As Boolean = (i >= 0) AndAlso (text(i) = "<"c) AndAlso (text(i) = "/"c)
					i += 1
					If tempVar Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Dim tempVar2 As Boolean = (++i < textpos) AndAlso (Char.ToLower(text(i)) = stack.elem.name.Chars(j++))
						Do While tempVar2

							tempVar2 = (++i < textpos) AndAlso (Char.ToLower(text(i)) = stack.elem.name.Chars(j++))
						Loop
						If i = textpos Then
							textpos -= (stack.elem.name.length() + 2)
							If (textpos > 0) AndAlso (text(textpos-1) = ControlChars.Lf) Then textpos -= 1
							endTag(False)
							Return
						End If
					End If

				  Case "&"c
					Dim data As Char() = parseEntityReference()
					If textpos + data.Length > text.Length Then
						Dim newtext As Char() = New Char(Math.Max(textpos + data.Length + 128, text.Length * 2) - 1){}
						Array.Copy(text, 0, newtext, 0, text.Length)
						text = newtext
					End If
					Array.Copy(data, 0, text, textpos, data.Length)
					textpos += data.Length
					Continue Do

				  Case ControlChars.Lf
					ln += 1
					ch = readCh()
					lfCount += 1

				  Case ControlChars.Cr
					ln += 1
					ch = readCh()
					If ch = ControlChars.Lf Then
						ch = readCh()
						crlfCount += 1
					Else
						crCount += 1
					End If
					c = ControlChars.Lf
				  Case Else
					ch = readCh()
				End Select

				' output character
				If textpos = text.Length Then
					Dim newtext As Char() = New Char(text.Length + 128 - 1){}
					Array.Copy(text, 0, newtext, 0, text.Length)
					text = newtext
				End If
				text(textpos) = ChrW(c)
				textpos += 1
			Loop
		End Sub

		''' <summary>
		''' Parse attribute value. [33] 331:1
		''' </summary>
		Friend Overridable Function parseAttributeValue(ByVal lower As Boolean) As String
			Dim delim As Integer = -1

			' Check for a delimiter
			Select Case ch
			  Case "'"c, """"c
				delim = ch
				ch = readCh()
			End Select

			' Parse the rest of the value
			Do
				Dim c As Integer = ch

				Select Case c
				  Case ControlChars.Lf
					ln += 1
					ch = readCh()
					lfCount += 1
					If delim < 0 Then Return getString(0)

				  Case ControlChars.Cr
					ln += 1

					ch = readCh()
					If ch = ControlChars.Lf Then
						ch = readCh()
						crlfCount += 1
					Else
						crCount += 1
					End If
					If delim < 0 Then Return getString(0)

				  Case ControlChars.Tab
					  If delim < 0 Then c = AscW(" "c)
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				  Case " "c
					ch = readCh()
					If delim < 0 Then Return getString(0)

				  Case ">"c, "<"c
					If delim < 0 Then Return getString(0)
					ch = readCh()

				  Case "'"c, """"c
					ch = readCh()
					If c = delim Then
						Return getString(0)
					ElseIf delim = -1 Then
						[error]("attvalerr")
						If [strict] OrElse ch = AscW(" "c) Then
							Return getString(0)
						Else
							Continue Do
						End If
					End If

				Case "="c
					If delim < 0 Then
	'                     In SGML a construct like <img src=/cgi-bin/foo?x=1>
	'                       is considered invalid since an = sign can only be contained
	'                       in an attributes value if the string is quoted.
	'                       
						[error]("attvalerr")
	'                     If strict is true then we return with the string we have thus far.
	'                       Otherwise we accept the = sign as part of the attribute's value and
	'                       process the rest of the img tag. 
						If [strict] Then Return getString(0)
					End If
					ch = readCh()

				  Case "&"c
					If [strict] AndAlso delim < 0 Then
						ch = readCh()
						Exit Select
					End If

					Dim data As Char() = parseEntityReference()
					For i As Integer = 0 To data.Length - 1
						c = AscW(data(i))
						addString(If(lower AndAlso (c >= "A"c) AndAlso (c <= "Z"c), AscW("a"c) + c - AscW("A"c), c))
					Next i
					Continue Do

				  Case -1
					Return getString(0)

				  Case Else
					If lower AndAlso (c >= "A"c) AndAlso (c <= "Z"c) Then c = AscW("a"c) + c - AscW("A"c)
					ch = readCh()
				End Select
				addString(c)
			Loop
		End Function


		''' <summary>
		''' Parse attribute specification List. [31] 327:17
		''' </summary>
		Friend Overridable Sub parseAttributeSpecificationList(ByVal elem As Element)

			Do
				skipSpace()

				Select Case ch
				  Case "/"c, ">"c, "<"c, -1
					Return

				  Case "-"c
					ch = readCh()
					If ch = AscW("-"c) Then
						ch = readCh()
						parseComment()
						strpos = 0
					Else
						[error]("invalid.tagchar", "-", elem.name)
						ch = readCh()
					End If
					Continue Do
				End Select

				Dim att As AttributeList
				Dim attname As String
				Dim attvalue As String

				If parseIdentifier(True) Then
					attname = getString(0)
					skipSpace()
					If ch = AscW("="c) Then
						ch = readCh()
						skipSpace()
						att = elem.getAttribute(attname)
	'  Bug ID 4102750
	'  Load the NAME of an Attribute Case Sensitive
	'  The case of the NAME  must be intact
	'  MG 021898
						attvalue = parseAttributeValue((att IsNot Nothing) AndAlso (att.type <> CDATA) AndAlso (att.type <> NOTATION) AndAlso (att.type <> NAME))
	'                  attvalue = parseAttributeValue((att != null) && (att.type != CDATA) && (att.type != NOTATION));
					Else
						attvalue = attname
						att = elem.getAttributeByValue(attvalue)
						If att Is Nothing Then
							att = elem.getAttribute(attname)
							If att IsNot Nothing Then
								attvalue = att.value
							Else
								' Make it null so that NULL_ATTRIBUTE_VALUE is
								' used
								attvalue = Nothing
							End If
						End If
					End If ' allows for comma separated attribute-value pairs
				ElseIf (Not [strict]) AndAlso ch = AscW(","c) Then
					ch = readCh()
					Continue Do
				ElseIf (Not [strict]) AndAlso ch = """"c Then ' allows for quoted attributes
					ch = readCh()
					skipSpace()
					If parseIdentifier(True) Then
						attname = getString(0)
						If ch = """"c Then ch = readCh()
						skipSpace()
						If ch = AscW("="c) Then
							ch = readCh()
							skipSpace()
							att = elem.getAttribute(attname)
							attvalue = parseAttributeValue((att IsNot Nothing) AndAlso (att.type <> CDATA) AndAlso (att.type <> NOTATION))
						Else
							attvalue = attname
							att = elem.getAttributeByValue(attvalue)
							If att Is Nothing Then
								att = elem.getAttribute(attname)
								If att IsNot Nothing Then attvalue = att.value
							End If
						End If
					Else
						Dim str As Char() = {ChrW(ch)}
						[error]("invalid.tagchar", New String(str), elem.name)
						ch = readCh()
						Continue Do
					End If
				ElseIf (Not [strict]) AndAlso (attributes.empty) AndAlso (ch = AscW("="c)) Then
					ch = readCh()
					skipSpace()
					attname = elem.name
					att = elem.getAttribute(attname)
					attvalue = parseAttributeValue((att IsNot Nothing) AndAlso (att.type <> CDATA) AndAlso (att.type <> NOTATION))
				ElseIf (Not [strict]) AndAlso (ch = AscW("="c)) Then
					ch = readCh()
					skipSpace()
					attvalue = parseAttributeValue(True)
					[error]("attvalerr")
					Return
				Else
					Dim str As Char() = {ChrW(ch)}
					[error]("invalid.tagchar", New String(str), elem.name)
					If Not [strict] Then
						ch = readCh()
						Continue Do
					Else
						Return
					End If
				End If

				If att IsNot Nothing Then
					attname = att.name
				Else
					[error]("invalid.tagatt", attname, elem.name)
				End If

				' Check out the value
				If attributes.isDefined(attname) Then [error]("multi.tagatt", attname, elem.name)
				If attvalue Is Nothing Then
					attvalue = If((att IsNot Nothing) AndAlso (att.value IsNot Nothing), att.value, javax.swing.text.html.HTML.NULL_ATTRIBUTE_VALUE)
				ElseIf (att IsNot Nothing) AndAlso (att.values IsNot Nothing) AndAlso (Not att.values.Contains(attvalue)) Then
					[error]("invalid.tagattval", attname, elem.name)
				End If
				Dim attkey As javax.swing.text.html.HTML.Attribute = javax.swing.text.html.HTML.getAttributeKey(attname)
				If attkey Is Nothing Then
					attributes.addAttribute(attname, attvalue)
				Else
					attributes.addAttribute(attkey, attvalue)
				End If
			Loop
		End Sub

		''' <summary>
		''' Parses th Document Declaration Type markup declaration.
		''' Currently ignores it.
		''' </summary>
		Public Overridable Function parseDTDMarkup() As String

			Dim strBuff As New StringBuilder
			ch = readCh()
			Do
				Select Case ch
				Case ">"c
					ch = readCh()
					Return strBuff.ToString()
				Case -1
					[error]("invalid.markup")
					Return strBuff.ToString()
				Case ControlChars.Lf
					ln += 1
					ch = readCh()
					lfCount += 1
				Case """"c
					ch = readCh()
				Case ControlChars.Cr
					ln += 1
					ch = readCh()
					If ch = ControlChars.Lf Then
						ch = readCh()
						crlfCount += 1
					Else
						crCount += 1
					End If
				Case Else
					strBuff.Append(CChar(ch And &HFF))
					ch = readCh()
				End Select
			Loop
		End Function

		''' <summary>
		''' Parse markup declarations.
		''' Currently only handles the Document Type Declaration markup.
		''' Returns true if it is a markup declaration false otherwise.
		''' </summary>
		Protected Friend Overridable Function parseMarkupDeclarations(ByVal strBuff As StringBuilder) As Boolean

			' Currently handles only the DOCTYPE 
			If (strBuff.Length = "DOCTYPE".length()) AndAlso (strBuff.ToString().ToUpper().Equals("DOCTYPE")) Then
				parseDTDMarkup()
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Parse an invalid tag.
		''' </summary>
		Friend Overridable Sub parseInvalidTag()
			' ignore all data upto the close bracket '>'
			Do
				skipSpace()
				Select Case ch
				  Case ">"c, -1
					  ch = readCh()
					Return
				  Case "<"c
					  Return
				  Case Else
					  ch = readCh()

				End Select
			Loop
		End Sub

		''' <summary>
		''' Parse a start or end tag.
		''' </summary>
		Friend Overridable Sub parseTag()
			Dim elem As Element
			Dim net As Boolean = False
			Dim warned As Boolean = False
			Dim unknown As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Select Case ch = readCh()
			  Case "!"c
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Select Case ch = readCh()
				  Case "-"c
					' Parse comment. [92] 391:7
					Do
						If ch = AscW("-"c) Then
							ch = readCh()
							If (Not [strict]) OrElse (ch = AscW("-"c)) Then
								ch = readCh()
								If (Not [strict]) AndAlso ch = AscW("-"c) Then ch = readCh()
								' send over any text you might see
								' before parsing and sending the
								' comment
								If textpos <> 0 Then
									Dim newtext As Char() = New Char(textpos - 1){}
									Array.Copy(text, 0, newtext, 0, textpos)
									handleText(newtext)
									lastBlockStartPos = currentBlockStartPos
									textpos = 0
								End If
								parseComment()
								last = makeTag(dtd.getElement("comment"), True)
								handleComment(getChars(0))
								Continue Do
							ElseIf Not warned Then
								warned = True
								[error]("invalid.commentchar", "-")
							End If
						End If
						skipSpace()
						Select Case ch
						  Case "-"c
							Continue Do
						  Case ">"c
							ch = readCh()
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						  Case -1
							Return
						  Case Else
							ch = readCh()
							If Not warned Then
								warned = True
								[error]("invalid.commentchar", Convert.ToString(ChrW(ch)))
							End If
						End Select
					Loop

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				  Case Else
					' deal with marked sections
					Dim strBuff As New StringBuilder
					Do
						strBuff.Append(ChrW(ch))
						If parseMarkupDeclarations(strBuff) Then Return
						Select Case ch
						  Case ">"c
							ch = readCh()
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						  Case -1
							[error]("invalid.markup")
							Return
						  Case ControlChars.Lf
							ln += 1
							ch = readCh()
							lfCount += 1
						  Case ControlChars.Cr
							ln += 1
							ch = readCh()
							If ch = ControlChars.Lf Then
								ch = readCh()
								crlfCount += 1
							Else
								crCount += 1
							End If

						  Case Else
							ch = readCh()
						End Select
					Loop
				End Select

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			  Case "/"c
				' parse end tag [19] 317:4
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Select Case ch = readCh()
				  Case ">"c
					ch = readCh()
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				  Case "<"c
					' empty end tag. either </> or </<
					If recent Is Nothing Then
						[error]("invalid.shortend")
						Return
					End If
					elem = recent

				  Case Else
					If Not parseIdentifier(True) Then
						[error]("expected.endtagname")
						Return
					End If
					skipSpace()
					Select Case ch
					  Case ">"c
						ch = readCh()
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					  Case "<"c

					  Case Else
						[error]("expected", "'>'")
						Do While (ch <> -1) AndAlso (ch <> ControlChars.Lf) AndAlso (ch <> AscW(">"c))
							ch = readCh()
						Loop
						If ch = AscW(">"c) Then ch = readCh()
					End Select
					Dim elemStr As String = getString(0)
					If Not dtd.elementExists(elemStr) Then
						[error]("end.unrecognized", elemStr)
						' Ignore RE before end tag
						If (textpos > 0) AndAlso (text(textpos-1) = ControlChars.Lf) Then textpos -= 1
						elem = dtd.getElement("unknown")
						elem.name = elemStr
						unknown = True
					Else
						elem = dtd.getElement(elemStr)
					End If
				End Select


				' If the stack is null, we're seeing end tags without any begin
				' tags.  Ignore them.

				If stack Is Nothing Then
					[error]("end.extra.tag", elem.name)
					Return
				End If

				' Ignore RE before end tag
				If (textpos > 0) AndAlso (text(textpos-1) = ControlChars.Lf) Then
					' In a pre tag, if there are blank lines
					' we do not want to remove the newline
					' before the end tag.  Hence this code.
					'
					If stack.pre Then
						If (textpos > 1) AndAlso (text(textpos-2) <> ControlChars.Lf) Then textpos -= 1
					Else
						textpos -= 1
					End If
				End If

				' If the end tag is a form, since we did not put it
				' on the tag stack, there is no corresponding start
				' start tag to find. Hence do not touch the tag stack.
				'

	'            
	'            if (!strict && elem.getName().equals("form")) {
	'                if (lastFormSent != null) {
	'                    handleEndTag(lastFormSent);
	'                    return;
	'                } else {
	'                    // do nothing.
	'                    return;
	'                }
	'            }
	'            

				If unknown Then
					' we will not see a corresponding start tag
					' on the the stack.  If we are seeing an
					' end tag, lets send this on as an empty
					' tag with the end tag attribute set to
					' true.
					Dim t As TagElement = makeTag(elem)
					handleText(t)
					attributes.addAttribute(javax.swing.text.html.HTML.Attribute.ENDTAG, "true")
					handleEmptyTag(makeTag(elem))
					unknown = False
					Return
				End If

				' find the corresponding start tag

				' A commonly occurring error appears to be the insertion
				' of extra end tags in a table.  The intent here is ignore
				' such extra end tags.
				'
				If Not [strict] Then
					Dim stackElem As String = stack.elem.name

					If stackElem.Equals("table") Then
						' If it is not a valid end tag ignore it and return
						'
						If Not elem.name.Equals(stackElem) Then
							[error]("tag.ignore", elem.name)
							Return
						End If
					End If



					If stackElem.Equals("tr") OrElse stackElem.Equals("td") Then
						If ((Not elem.name.Equals("table"))) AndAlso ((Not elem.name.Equals(stackElem))) Then
							[error]("tag.ignore", elem.name)
							Return
						End If
					End If
				End If
				Dim sp As TagStack = stack

				Do While (sp IsNot Nothing) AndAlso (elem IsNot sp.elem)
					sp = sp.next
				Loop
				If sp Is Nothing Then
					[error]("unmatched.endtag", elem.name)
					Return
				End If

				' People put font ending tags in the darndest places.
				' Don't close other contexts based on them being between
				' a font tag and the corresponding end tag.  Instead,
				' ignore the end tag like it doesn't exist and allow the end
				' of the document to close us out.
				Dim elemName As String = elem.name
				If stack IsNot sp AndAlso (elemName.Equals("font") OrElse elemName.Equals("center")) Then

					' Since closing out a center tag can have real wierd
					' effects on the formatting,  make sure that tags
					' for which omitting an end tag is legimitate
					' get closed out.
					'
					If elemName.Equals("center") Then
						Do While stack.elem.omitEnd() AndAlso stack IsNot sp
							endTag(True)
						Loop
						If stack.elem Is elem Then endTag(False)
					End If
					Return
				End If
				' People do the same thing with center tags.  In this
				' case we would like to close off the center tag but
				' not necessarily all enclosing tags.



				' end tags
				Do While stack IsNot sp
					endTag(True)
				Loop

				endTag(False)
				Return

			  Case -1
				[error]("eof")
				Return
			End Select

			' start tag [14] 314:1
			If Not parseIdentifier(True) Then
				elem = recent
				If (ch <> AscW(">"c)) OrElse (elem Is Nothing) Then
					[error]("expected.tagname")
					Return
				End If
			Else
				Dim elemStr As String = getString(0)

				If elemStr.Equals("image") Then elemStr = "img"

				' determine if this element is part of the dtd. 

				If Not dtd.elementExists(elemStr) Then
					'              parseInvalidTag();
					[error]("tag.unrecognized ", elemStr)
					elem = dtd.getElement("unknown")
					elem.name = elemStr
					unknown = True
				Else
					elem = dtd.getElement(elemStr)
				End If
			End If

			' Parse attributes
			parseAttributeSpecificationList(elem)

			Select Case ch
			  Case "/"c
				net = True
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			  Case ">"c
				ch = readCh()
				If ch = AscW(">"c) AndAlso net Then ch = readCh()
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			  Case "<"c

			  Case Else
				[error]("expected", "'>'")
			End Select

			If Not [strict] Then
			  If elem.name.Equals("script") Then [error]("javascript.unsupported")
			End If

			' ignore RE after start tag
			'
			If Not elem.empty Then
				If ch = ControlChars.Lf Then
					ln += 1
					lfCount += 1
					ch = readCh()
				ElseIf ch = ControlChars.Cr Then
					ln += 1
					ch = readCh()
					If ch = ControlChars.Lf Then
						ch = readCh()
						crlfCount += 1
					Else
						crCount += 1
					End If
				End If
			End If

			' ensure a legal context for the tag
			Dim tag As TagElement = makeTag(elem, False)


			''' <summary>
			''' In dealing with forms, we have decided to treat
			'''    them as legal in any context.  Also, even though
			'''    they do have a start and an end tag, we will
			'''    not put this tag on the stack.  This is to deal
			'''    several pages in the web oasis that choose to
			'''    start and end forms in any possible location. *
			''' </summary>

	'        
	'        if (!strict && elem.getName().equals("form")) {
	'            if (lastFormSent == null) {
	'                lastFormSent = tag;
	'            } else {
	'                handleEndTag(lastFormSent);
	'                lastFormSent = tag;
	'            }
	'        } else {
	'        
				' Smlly, if a tag is unknown, we will apply
				' no legalTagContext logic to it.
				'
				If Not unknown Then
					legalTagContext(tag)

					' If skip tag is true,  this implies that
					' the tag was illegal and that the error
					' recovery strategy adopted is to ignore
					' the tag.
					If (Not [strict]) AndAlso skipTag Then
						skipTag = False
						Return
					End If
				End If
	'            
	'        }
	'            

			startTag(tag)

			If Not elem.empty Then
				Select Case elem.type
				  Case CDATA
					parseLiteral(False)
				  Case RCDATA
					parseLiteral(True)
				  Case Else
					If stack IsNot Nothing Then stack.net = net
				End Select
			End If
		End Sub

		Private Const START_COMMENT As String = "<!--"
		Private Const END_COMMENT As String = "-->"
		Private Shared ReadOnly SCRIPT_END_TAG As Char() = "</script>".toCharArray()
		Private Shared ReadOnly SCRIPT_END_TAG_UPPER_CASE As Char() = "</SCRIPT>".toCharArray()

		Friend Overridable Sub parseScript()
			Dim charsToAdd As Char() = New Char(SCRIPT_END_TAG.Length - 1){}
			Dim insideComment As Boolean = False

			' Here, ch should be the first character after <script> 
			Do
				Dim i As Integer = 0
				Do While (Not insideComment) AndAlso i < SCRIPT_END_TAG.Length AndAlso (AscW(SCRIPT_END_TAG(i)) = ch OrElse AscW(SCRIPT_END_TAG_UPPER_CASE(i)) = ch)
					charsToAdd(i) = ChrW(ch)
					ch = readCh()
					i += 1
				Loop
				If i = SCRIPT_END_TAG.Length Then

					'  '</script>' tag detected 
					' Here, ch == the first character after </script> 
					Return
				Else

					' To account for extra read()'s that happened 
					For j As Integer = 0 To i - 1
						addString(charsToAdd(j))
					Next j

					Select Case ch
					Case -1
						[error]("eof.script")
						Return
					Case ControlChars.Lf
						ln += 1
						ch = readCh()
						lfCount += 1
						addString(ControlChars.Lf)
					Case ControlChars.Cr
						ln += 1
						ch = readCh()
						If ch = ControlChars.Lf Then
							ch = readCh()
							crlfCount += 1
						Else
							crCount += 1
						End If
						addString(ControlChars.Lf)
					Case Else
						addString(ch)
						Dim str As New String(getChars(0, strpos))
						If (Not insideComment) AndAlso str.EndsWith(START_COMMENT) Then insideComment = True
						If insideComment AndAlso str.EndsWith(END_COMMENT) Then insideComment = False
						ch = readCh()
						Exit Select
					End Select ' switch
				End If
			Loop ' while
		End Sub

		''' <summary>
		''' Parse Content. [24] 320:1
		''' </summary>
		Friend Overridable Sub parseContent()
			Dim curThread As Thread = Thread.CurrentThread

			Do
				If curThread.interrupted Then
					curThread.Interrupt() ' resignal the interrupt
					Exit Do
				End If

				Dim c As Integer = ch
				currentBlockStartPos = currentPosition

				If recent Is dtd.script Then ' means: if after starting <script> tag

					' Here, ch has to be the first character after <script> 
					parseScript()
					last = makeTag(dtd.getElement("comment"), True)

					' Remove leading and trailing HTML comment declarations 
					Dim str As (New String(getChars(0))).Trim()
					Dim minLength As Integer = START_COMMENT.Length + END_COMMENT.Length
					If str.StartsWith(START_COMMENT) AndAlso str.EndsWith(END_COMMENT) AndAlso str.Length >= CType(Then, minLength)
						str = str.Substring(START_COMMENT.Length, str.Length - END_COMMENT.Length - (START_COMMENT.Length))
					End If

					' Handle resulting chars as comment 
					handleComment(str.ToCharArray())
					endTag(False)
					lastBlockStartPos = currentPosition

					Continue Do
				Else
					Select Case c
					  Case "<"c
						parseTag()
						lastBlockStartPos = currentPosition
						Continue Do

					  Case "/"c
						ch = readCh()
						If (stack IsNot Nothing) AndAlso stack.net Then
							' null end tag.
							endTag(False)
							Continue Do
						ElseIf textpos = 0 Then
							If Not legalElementContext(dtd.pcdata) Then [error]("unexpected.pcdata")
							If last.breaksFlow() Then space = False
						End If

					  Case -1
						Return

					  Case "&"c
						If textpos = 0 Then
							If Not legalElementContext(dtd.pcdata) Then [error]("unexpected.pcdata")
							If last.breaksFlow() Then space = False
						End If
						Dim data As Char() = parseEntityReference()
						If textpos + data.Length + 1 > text.Length Then
							Dim newtext As Char() = New Char(Math.Max(textpos + data.Length + 128, text.Length * 2) - 1){}
							Array.Copy(text, 0, newtext, 0, text.Length)
							text = newtext
						End If
						If space Then
							space = False
							text(textpos) = " "c
							textpos += 1
						End If
						Array.Copy(data, 0, text, textpos, data.Length)
						textpos += data.Length
						ignoreSpace = False
						Continue Do

					  Case ControlChars.Lf
						ln += 1
						lfCount += 1
						ch = readCh()
						If (stack IsNot Nothing) AndAlso stack.pre Then Exit Select
						If textpos = 0 Then lastBlockStartPos = currentPosition
						If Not ignoreSpace Then space = True
						Continue Do

					  Case ControlChars.Cr
						ln += 1
						c = ControlChars.Lf
						ch = readCh()
						If ch = ControlChars.Lf Then
							ch = readCh()
							crlfCount += 1
						Else
							crCount += 1
						End If
						If (stack IsNot Nothing) AndAlso stack.pre Then Exit Select
						If textpos = 0 Then lastBlockStartPos = currentPosition
						If Not ignoreSpace Then space = True
						Continue Do


					  Case ControlChars.Tab, " "c
						ch = readCh()
						If (stack IsNot Nothing) AndAlso stack.pre Then Exit Select
						If textpos = 0 Then lastBlockStartPos = currentPosition
						If Not ignoreSpace Then space = True
						Continue Do

					  Case Else
						If textpos = 0 Then
							If Not legalElementContext(dtd.pcdata) Then [error]("unexpected.pcdata")
							If last.breaksFlow() Then space = False
						End If
						ch = readCh()
					End Select
				End If

				' enlarge buffer if needed
				If textpos + 2 > text.Length Then
					Dim newtext As Char() = New Char(text.Length + 128 - 1){}
					Array.Copy(text, 0, newtext, 0, text.Length)
					text = newtext
				End If

				' output pending space
				If space Then
					If textpos = 0 Then lastBlockStartPos -= 1
					text(textpos) = " "c
					textpos += 1
					space = False
				End If
				text(textpos) = ChrW(c)
				textpos += 1
				ignoreSpace = False
			Loop
		End Sub

		''' <summary>
		''' Returns the end of line string. This will return the end of line
		''' string that has been encountered the most, one of \r, \n or \r\n.
		''' </summary>
		Friend Overridable Property endOfLineString As String
			Get
				If crlfCount >= crCount Then
					If lfCount >= crlfCount Then
						Return vbLf
					Else
						Return vbCrLf
					End If
				Else
					If crCount > lfCount Then
						Return vbCr
					Else
						Return vbLf
					End If
				End If
			End Get
		End Property

		''' <summary>
		''' Parse an HTML stream, given a DTD.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub parse(ByVal [in] As Reader)
			Me.in = [in]

			Me.ln = 1

			seenHtml = False
			seenHead = False
			seenBody = False

				crlfCount = 0
					lfCount = crlfCount
					crCount = lfCount

			Try
				ch = readCh()
				text = New Char(1023){}
				str = New Char(127){}

				parseContent()
				' NOTE: interruption may have occurred.  Control flows out
				' of here normally.
				Do While stack IsNot Nothing
					endTag(True)
				Loop
				[in].close()
			Catch e As IOException
				errorContext()
				[error]("ioexception")
				Throw e
			Catch e As Exception
				errorContext()
				[error]("exception", e.GetType().name, e.Message)
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			Catch e As ThreadDeath
				errorContext()
				[error]("terminated")
				e.printStackTrace()
				Throw e
			Finally
				Do While stack IsNot Nothing
					handleEndTag(stack.tag)
					stack = stack.next
				Loop

				text = Nothing
				str = Nothing
			End Try

		End Sub


	'    
	'     * Input cache.  This is much faster than calling down to a synchronized
	'     * method of BufferedReader for each byte.  Measurements done 5/30/97
	'     * show that there's no point in having a bigger buffer:  Increasing
	'     * the buffer to 8192 had no measurable impact for a program discarding
	'     * one character at a time (reading from an http URL to a local machine).
	'     * NOTE: If the current encoding is bogus, and we read too much
	'     * (past the content-type) we may suffer a MalformedInputException. For
	'     * this reason the initial size is 1 and when the body is encountered the
	'     * size is adjusted to 256.
	'     
		Private buf As Char() = New Char(0){}
		Private pos As Integer
		Private len As Integer
	'    
	'        tracks position relative to the beginning of the
	'        document.
	'    
		Private currentPosition As Integer


		Private Function readCh() As Integer

			If pos >= len Then

				' This loop allows us to ignore interrupts if the flag
				' says so
				Do
					Try
						len = [in].read(buf)
						Exit Do
					Catch ex As InterruptedIOException
						Throw ex
					End Try
				Loop

				If len <= 0 Then Return -1 ' eof
				pos = 0
			End If
			currentPosition += 1

				Dim tempVar As Integer = pos
				pos += 1
				Return buf(tempVar)
		End Function


		Protected Friend Overridable Property currentPos As Integer
			Get
				Return currentPosition
			End Get
		End Property
	End Class

End Namespace
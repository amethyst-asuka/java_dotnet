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
' * PURPOSE. See W3C License http://www.w3.org/Consortium/Legal/ for more
' * details.
' 

Namespace org.w3c.dom.html


	''' <summary>
	'''  An <code>HTMLDocument</code> is the root of the HTML hierarchy and holds
	''' the entire content. Besides providing access to the hierarchy, it also
	''' provides some convenience methods for accessing certain sets of
	''' information from the document.
	''' <p> The following properties have been deprecated in favor of the
	''' corresponding ones for the <code>BODY</code> element: alinkColor background
	'''  bgColor fgColor linkColor vlinkColor In DOM Level 2, the method
	''' <code>getElementById</code> is inherited from the <code>Document</code>
	''' interface where it was moved.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLDocument
		Inherits org.w3c.dom.Document

		''' <summary>
		'''  The title of a document as specified by the <code>TITLE</code> element
		''' in the head of the document.
		''' </summary>
		Property title As String

		''' <summary>
		'''  Returns the URI  of the page that linked to this page. The value is an
		''' empty string if the user navigated to the page directly (not through a
		''' link, but, for example, via a bookmark).
		''' </summary>
		ReadOnly Property referrer As String

		''' <summary>
		'''  The domain name of the server that served the document, or
		''' <code>null</code> if the server cannot be identified by a domain name.
		''' </summary>
		ReadOnly Property domain As String

		''' <summary>
		'''  The complete URI  of the document.
		''' </summary>
		ReadOnly Property uRL As String

		''' <summary>
		'''  The element that contains the content for the document. In documents
		''' with <code>BODY</code> contents, returns the <code>BODY</code>
		''' element. In frameset documents, this returns the outermost
		''' <code>FRAMESET</code> element.
		''' </summary>
		Property body As HTMLElement

		''' <summary>
		'''  A collection of all the <code>IMG</code> elements in a document. The
		''' behavior is limited to <code>IMG</code> elements for backwards
		''' compatibility.
		''' </summary>
		ReadOnly Property images As HTMLCollection

		''' <summary>
		'''  A collection of all the <code>OBJECT</code> elements that include
		''' applets and <code>APPLET</code> ( deprecated ) elements in a document.
		''' </summary>
		ReadOnly Property applets As HTMLCollection

		''' <summary>
		'''  A collection of all <code>AREA</code> elements and anchor (
		''' <code>A</code> ) elements in a document with a value for the
		''' <code>href</code> attribute.
		''' </summary>
		ReadOnly Property links As HTMLCollection

		''' <summary>
		'''  A collection of all the forms of a document.
		''' </summary>
		ReadOnly Property forms As HTMLCollection

		''' <summary>
		'''  A collection of all the anchor (<code>A</code> ) elements in a document
		'''  with a value for the <code>name</code> attribute. Note. For reasons
		''' of backwards compatibility, the returned set of anchors only contains
		''' those anchors created with the <code>name</code>  attribute, not those
		''' created with the <code>id</code> attribute.
		''' </summary>
		ReadOnly Property anchors As HTMLCollection

		''' <summary>
		'''  The cookies associated with this document. If there are none, the
		''' value is an empty string. Otherwise, the value is a string: a
		''' semicolon-delimited list of "name, value" pairs for all the cookies
		''' associated with the page. For example,
		''' <code>name=value;expires=date</code> .
		''' </summary>
		Property cookie As String

		''' <summary>
		'''  Note. This method and the ones following  allow a user to add to or
		''' replace the structure model of a document using strings of unparsed
		''' HTML. At the time of  writing alternate methods for providing similar
		''' functionality for  both HTML and XML documents were being considered.
		''' The following methods may be deprecated at some point in the future in
		''' favor of a more general-purpose mechanism.
		''' <br> Open a document stream for writing. If a document exists in the
		''' target, this method clears it.
		''' </summary>
		Sub open()

		''' <summary>
		'''  Closes a document stream opened by <code>open()</code> and forces
		''' rendering.
		''' </summary>
		Sub close()

		''' <summary>
		'''  Write a string of text to a document stream opened by
		''' <code>open()</code> . The text is parsed into the document's structure
		''' model. </summary>
		''' <param name="text">  The string to be parsed into some structure in the
		'''   document structure model. </param>
		Sub write(ByVal text As String)

		''' <summary>
		'''  Write a string of text followed by a newline character to a document
		''' stream opened by <code>open()</code> . The text is parsed into the
		''' document's structure model. </summary>
		''' <param name="text">  The string to be parsed into some structure in the
		'''   document structure model. </param>
		Sub writeln(ByVal text As String)

		''' <summary>
		'''  Returns the (possibly empty) collection of elements whose
		''' <code>name</code> value is given by <code>elementName</code> . </summary>
		''' <param name="elementName">  The <code>name</code> attribute value for an
		'''   element. </param>
		''' <returns>  The matching elements. </returns>
		Function getElementsByName(ByVal elementName As String) As org.w3c.dom.NodeList

	End Interface

End Namespace
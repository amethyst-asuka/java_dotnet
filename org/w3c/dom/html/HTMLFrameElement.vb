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
	'''  Create a frame. See the  FRAME element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLFrameElement
		Inherits HTMLElement

		''' <summary>
		'''  Request frame borders. See the  frameborder attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property frameBorder As String

		''' <summary>
		'''  URI designating a long description of this image or frame. See the
		''' longdesc attribute definition in HTML 4.0.
		''' </summary>
		Property longDesc As String

		''' <summary>
		'''  Frame margin height, in pixels. See the  marginheight attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property marginHeight As String

		''' <summary>
		'''  Frame margin width, in pixels. See the  marginwidth attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property marginWidth As String

		''' <summary>
		'''  The frame name (object of the <code>target</code> attribute). See the
		''' name attribute definition in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  When true, forbid user from resizing frame. See the  noresize
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property noResize As Boolean

		''' <summary>
		'''  Specify whether or not the frame should have scrollbars. See the
		''' scrolling attribute definition in HTML 4.0.
		''' </summary>
		Property scrolling As String

		''' <summary>
		'''  A URI designating the initial frame contents. See the  src attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property src As String

		''' <summary>
		'''  The document this frame contains, if there is any and it is available,
		''' or <code>null</code> otherwise.
		''' @since DOM Level 2
		''' </summary>
		ReadOnly Property contentDocument As org.w3c.dom.Document

	End Interface

End Namespace
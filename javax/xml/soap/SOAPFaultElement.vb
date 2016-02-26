'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap

	''' <summary>
	''' A representation of the contents in
	''' a <code>SOAPFault</code> object.  The <code>Detail</code> interface
	''' is a <code>SOAPFaultElement</code>.
	''' <P>
	''' Content is added to a <code>SOAPFaultElement</code> using the
	''' <code>SOAPElement</code> method <code>addTextNode</code>.
	''' </summary>
	Public Interface SOAPFaultElement
		Inherits SOAPElement

	End Interface

End Namespace
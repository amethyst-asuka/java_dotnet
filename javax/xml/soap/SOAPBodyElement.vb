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
	''' A <code>SOAPBodyElement</code> object represents the contents in
	''' a <code>SOAPBody</code> object.  The <code>SOAPFault</code> interface
	''' is a <code>SOAPBodyElement</code> object that has been defined.
	''' <P>
	''' A new <code>SOAPBodyElement</code> object can be created and added
	''' to a <code>SOAPBody</code> object with the <code>SOAPBody</code>
	''' method <code>addBodyElement</code>. In the following line of code,
	''' <code>sb</code> is a <code>SOAPBody</code> object, and
	''' <code>myName</code> is a <code>Name</code> object.
	''' <PRE>
	'''    SOAPBodyElement sbe = sb.addBodyElement(myName);
	''' </PRE>
	''' </summary>
	Public Interface SOAPBodyElement
		Inherits SOAPElement

	End Interface

End Namespace
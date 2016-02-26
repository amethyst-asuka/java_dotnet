'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind

	''' <summary>
	''' This event indicates that a problem was encountered while converting a
	''' string from the XML data into a value of the target Java data type.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= ValidationEvent </seealso>
	''' <seealso cref= ValidationEventHandler </seealso>
	''' <seealso cref= Unmarshaller
	''' @since JAXB1.0 </seealso>
	Public Interface ParseConversionEvent
		Inherits ValidationEvent

	End Interface

End Namespace
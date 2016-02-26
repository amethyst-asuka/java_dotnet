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
	''' This event indicates that a problem was encountered while converting data
	''' from the Java content tree into its lexical representation.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= ValidationEvent </seealso>
	''' <seealso cref= ValidationEventHandler </seealso>
	''' <seealso cref= Marshaller
	''' @since JAXB1.0 </seealso>
	Public Interface PrintConversionEvent
		Inherits ValidationEvent

	End Interface

End Namespace
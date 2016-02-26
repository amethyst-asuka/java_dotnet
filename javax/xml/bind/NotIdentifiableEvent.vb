'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This event indicates that a problem was encountered resolving an ID/IDREF.
	''' 
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= Validator </seealso>
	''' <seealso cref= ValidationEventHandler
	''' @since JAXB1.0 </seealso>
	Public Interface NotIdentifiableEvent
		Inherits ValidationEvent

	End Interface

End Namespace
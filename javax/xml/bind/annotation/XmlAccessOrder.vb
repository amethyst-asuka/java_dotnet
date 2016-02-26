'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation

	''' <summary>
	''' Used by XmlAccessorOrder to control the ordering of properties and
	''' fields in a JAXB bound class.
	''' 
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc.
	''' @since JAXB2.0 </summary>
	''' <seealso cref= XmlAccessorOrder </seealso>

	Public Enum XmlAccessOrder
		''' <summary>
		''' The ordering of fields and properties in a class is undefined.
		''' </summary>
		UNDEFINED
		''' <summary>
		''' The ordering of fields and properties in a class is in
		''' alphabetical order as determined by the
		''' method java.lang.String.compareTo(String anotherString).
		''' </summary>
		ALPHABETICAL
	End Enum

End Namespace
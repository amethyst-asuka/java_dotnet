'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.attribute

	''' <summary>
	''' Interface PrintServiceAttribute is a tagging interface which a printing
	''' attribute class implements to indicate the attribute describes the status
	''' of a Print Service or some other characteristic of a Print Service. A Print
	''' Service instance adds a number of PrintServiceAttributes to a Print
	''' service's attribute set to report the Print Service's status.
	''' <P>
	''' </summary>
	''' <seealso cref= PrintServiceAttributeSet
	''' 
	''' @author  Alan Kaminsky </seealso>
	Public Interface PrintServiceAttribute
		Inherits Attribute

	End Interface

End Namespace
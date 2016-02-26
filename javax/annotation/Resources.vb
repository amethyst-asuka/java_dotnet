'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation

	''' <summary>
	''' This class is used to allow multiple resources declarations.
	''' </summary>
	''' <seealso cref= javax.annotation.Resource
	''' @since Common Annotations 1.0 </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class Resources
		Inherits System.Attribute

	   ''' <summary>
	   ''' Array used for multiple resource declarations.
	   ''' </summary>
	   Resource() value()
	End Class

End Namespace
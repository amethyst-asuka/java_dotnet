'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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


Namespace java.lang.reflect

	''' <summary>
	''' Thrown when a semantically malformed parameterized type is
	''' encountered by a reflective method that needs to instantiate it.
	''' For example, if the number of type arguments to a parameterized type
	''' is wrong.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MalformedParameterizedTypeException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -5696557788586220964L
	End Class

End Namespace
'
' * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.invoke


	''' <summary>
	''' Internal marker for some methods in the JSR 292 implementation.
	''' </summary>
	'non-public
	<AttributeUsage(AttributeTargets.Method Or AttributeTargets.Constructor, AllowMultiple := False, Inherited := False> _
	Friend Class ForceInline
		Inherits System.Attribute

	End Class

End Namespace
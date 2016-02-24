'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.ref

	''' <summary>
	''' Final references, used to implement finalization
	''' </summary>
	Friend Class FinalReference(Of T)
		Inherits Reference(Of T)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal referent As T, ByVal q As ReferenceQueue(Of T1))
			MyBase.New(referent, q)
		End Sub
	End Class

End Namespace
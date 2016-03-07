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

        Public Sub New(ByVal referent As T, ByVal q As ReferenceQueue(Of T))
            MyBase.New(referent, q)
        End Sub
    End Class
End Namespace
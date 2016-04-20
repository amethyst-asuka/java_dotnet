Imports System.Linq
Imports System.Runtime.CompilerServices
Imports java.lang
Imports java.math
Imports javax.lang.model.type

Module InnerExtensions

    <Extension> Public Function value(s As String) As Char()
        If String.IsNullOrEmpty(s) Then
            Return New Char() {}  ' Returns nothing???
        Else
            Return s.ToArray
        End If
    End Function

    ''' <summary>
    ''' Returns {@code true} if this kind corresponds to a primitive
    ''' type and {@code false} otherwise. </summary>
    ''' <returns> {@code true} if this kind corresponds to a primitive type </returns>
    <Extension> Public Function isPrimitive([me] As TypeKind) As Boolean
        Select Case [me]

            Case TypeKind.BOOLEAN, TypeKind.BYTE, TypeKind.SHORT, TypeKind.INT, TypeKind.LONG, TypeKind.CHAR, TypeKind.FLOAT, TypeKind.DOUBLE
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Function Values(Of T)() As T()
        Return Enums(Of T)()
    End Function
End Module

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports java.lang
Imports java.math

Module InnerExtensions

    <Extension> Public Function value(s As String) As Char()
        If String.IsNullOrEmpty(s) Then
            Return New Char() {}  ' Returns nothing???
        Else
            Return s.ToArray
        End If
    End Function
End Module

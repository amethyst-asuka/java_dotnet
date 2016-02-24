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

    ''' <summary>
    ''' Returns the {@code RoundingMode} object corresponding to a
    ''' legacy integer rounding mode constant in <seealso cref="BigDecimal"/>.
    ''' </summary>
    ''' <param name="rm"> legacy integer rounding mode to convert </param>
    ''' <returns> {@code RoundingMode} corresponding to the given integer. </returns>
    ''' <exception cref="IllegalArgumentException"> integer is out of range </exception>
    Public Function valueOf(rm As Integer) As RoundingMode

        Select Case rm
            Case BigDecimal.ROUND_UP
                Return RoundingMode.UP

            Case BigDecimal.ROUND_DOWN
                Return RoundingMode.DOWN

            Case BigDecimal.ROUND_CEILING
                Return RoundingMode.CEILING

            Case BigDecimal.ROUND_FLOOR
                Return RoundingMode.FLOOR

            Case BigDecimal.ROUND_HALF_UP
                Return RoundingMode.HALF_UP

            Case BigDecimal.ROUND_HALF_DOWN
                Return RoundingMode.HALF_DOWN

            Case BigDecimal.ROUND_HALF_EVEN
                Return RoundingMode.HALF_EVEN

            Case BigDecimal.ROUND_UNNECESSARY
                Return RoundingMode.UNNECESSARY

            Case Else
                Throw New IllegalArgumentException("argument out of range")
        End Select
    End Function
End Module

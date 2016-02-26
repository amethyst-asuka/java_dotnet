Imports System
Imports java.lang

'
' * Copyright (c) 2003, 2007, Oracle and/or its affiliates. All rights reserved.
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

'
' * Portions Copyright IBM Corporation, 1997, 2001. All Rights Reserved.
' 

Namespace java.math

    ''' <summary>
    ''' Immutable objects which encapsulate the context settings which
    ''' describe certain rules for numerical operators, such as those
    ''' implemented by the <seealso cref="BigDecimal"/> class.
    ''' 
    ''' <p>The base-independent settings are:
    ''' <ol>
    ''' <li>{@code precision}:
    ''' the number of digits to be used for an operation; results are
    ''' rounded to this precision
    ''' 
    ''' <li>{@code roundingMode}:
    ''' a <seealso cref="RoundingMode"/> object which specifies the algorithm to be
    ''' used for rounding.
    ''' </ol>
    ''' </summary>
    ''' <seealso cref=     BigDecimal </seealso>
    ''' <seealso cref=     RoundingMode
    ''' @author  Mike Cowlishaw
    ''' @author  Joseph D. Darcy
    ''' @since 1.5 </seealso>

    <Serializable>
    Public NotInheritable Class MathContext

        ' ----- Constants ----- 

        ' defaults for constructors
        Private Const DEFAULT_DIGITS As Integer = 9
        Private Const DEFAULT_ROUNDINGMODE As RoundingMode = roundingMode.HALF_UP
        ' Smallest values for digits (Maximum is  [Integer].MAX_VALUE)
        Private Const MIN_DIGITS As Integer = 0

        ' Serialization version
        Private Const serialVersionUID As Long = 5579720004786848255L

        ' ----- Public Properties ----- 
        ''' <summary>
        '''  A {@code MathContext} object whose settings have the values
        '''  required for unlimited precision arithmetic.
        '''  The values of the settings are:
        '''  <code>
        '''  precision=0 roundingMode=HALF_UP
        '''  </code>
        ''' </summary>
        Public Shared ReadOnly UNLIMITED As New MathContext(0, roundingMode.HALF_UP)

        ''' <summary>
        '''  A {@code MathContext} object with a precision setting
        '''  matching the IEEE 754R Decimal32 format, 7 digits, and a
        '''  rounding mode of <seealso cref="RoundingMode#HALF_EVEN HALF_EVEN"/>, the
        '''  IEEE 754R default.
        ''' </summary>
        Public Shared ReadOnly DECIMAL32 As New MathContext(7, roundingMode.HALF_EVEN)

        ''' <summary>
        '''  A {@code MathContext} object with a precision setting
        '''  matching the IEEE 754R Decimal64 format, 16 digits, and a
        '''  rounding mode of <seealso cref="RoundingMode#HALF_EVEN HALF_EVEN"/>, the
        '''  IEEE 754R default.
        ''' </summary>
        Public Shared ReadOnly DECIMAL64 As New MathContext(16, roundingMode.HALF_EVEN)

        ''' <summary>
        '''  A {@code MathContext} object with a precision setting
        '''  matching the IEEE 754R Decimal128 format, 34 digits, and a
        '''  rounding mode of <seealso cref="RoundingMode#HALF_EVEN HALF_EVEN"/>, the
        '''  IEEE 754R default.
        ''' </summary>
        Public Shared ReadOnly DECIMAL128 As New MathContext(34, roundingMode.HALF_EVEN)

        ' ----- Shared Properties ----- 
        ''' <summary>
        ''' The number of digits to be used for an operation.  A value of 0
        ''' indicates that unlimited precision (as many digits as are
        ''' required) will be used.  Note that leading zeros (in the
        ''' coefficient of a number) are never significant.
        ''' 
        ''' <p>{@code precision} will always be non-negative.
        ''' 
        ''' @serial
        ''' </summary>
        Friend ReadOnly _precision As Integer

        ''' <summary>
        ''' The rounding algorithm to be used for an operation.
        ''' </summary>
        ''' <seealso cref= RoundingMode
        ''' @serial </seealso>
        Friend ReadOnly _roundingMode As RoundingMode

        ' ----- Constructors ----- 

        ''' <summary>
        ''' Constructs a new {@code MathContext} with the specified
        ''' precision and the <seealso cref="RoundingMode#HALF_UP HALF_UP"/> rounding
        ''' mode.
        ''' </summary>
        ''' <param name="setPrecision"> The non-negative {@code int} precision setting. </param>
        ''' <exception cref="IllegalArgumentException"> if the {@code setPrecision} parameter is less
        '''         than zero. </exception>
        Public Sub New(ByVal setPrecision As Integer)
            Me.New(precisionion, DEFAULT_ROUNDINGMODE)
            Return
        End Sub

        ''' <summary>
        ''' Constructs a new {@code MathContext} with a specified
        ''' precision and rounding mode.
        ''' </summary>
        ''' <param name="setPrecision"> The non-negative {@code int} precision setting. </param>
        ''' <param name="setRoundingMode"> The rounding mode to use. </param>
        ''' <exception cref="IllegalArgumentException"> if the {@code setPrecision} parameter is less
        '''         than zero. </exception>
        ''' <exception cref="NullPointerException"> if the rounding mode argument is {@code null} </exception>
        Public Sub New(ByVal setPrecision As Integer, ByVal setRoundingMode As RoundingMode)
            If precisionion < MIN_DIGITS Then Throw New IllegalArgumentException("Digits < 0")
            If roundingModeode Is Nothing Then Throw New NullPointerException("null RoundingMode")

            precision = precisionion
            roundingMode = roundingModeode
            Return
        End Sub

        ''' <summary>
        ''' Constructs a new {@code MathContext} from a string.
        ''' 
        ''' The string must be in the same format as that produced by the
        ''' <seealso cref="#toString"/> method.
        ''' 
        ''' <p>An {@code IllegalArgumentException} is thrown if the precision
        ''' section of the string is out of range ({@code < 0}) or the string is
        ''' not in the format created by the <seealso cref="#toString"/> method.
        ''' </summary>
        ''' <param name="val"> The string to be parsed </param>
        ''' <exception cref="IllegalArgumentException"> if the precision section is out of range
        ''' or of incorrect format </exception>
        ''' <exception cref="NullPointerException"> if the argument is {@code null} </exception>
        Public Sub New(ByVal val As String)
            Dim bad As Boolean = False
            Dim precisionion As Integer
            If val Is Nothing Then Throw New NullPointerException("null String")
            Try ' any error here is a string format problem
                If Not val.StartsWith("precision=") Then Throw New RuntimeException
                Dim fence As Integer = val.IndexOf(" "c) ' could be -1
                Dim [off] As Integer = 10 ' where value starts
                precisionion = Convert.ToInt32(val.Substring(10, fence - 10))

                If Not val.StartsWith("roundingMode=", fence + 1) Then Throw New RuntimeException
                [off] = fence + 1 + 13
                Dim str As String = val.Substring([off], val.Length() - [off])
                roundingMode = System.Enum.Parse(GetType(RoundingMode), str)
            Catch re As RuntimeException
                Throw New IllegalArgumentException("bad string format")
            End Try

            If precisionion < MIN_DIGITS Then Throw New IllegalArgumentException("Digits < 0")
            ' the other parameters cannot be invalid if we got here
            precision = precisionion
        End Sub

        ''' <summary>
        ''' Returns the {@code precision} setting.
        ''' This value is always non-negative.
        ''' </summary>
        ''' <returns> an {@code int} which is the value of the {@code precision}
        '''         setting </returns>
        Public ReadOnly Property precision As Integer
            Get
                Return _precision
            End Get
        End Property

        ''' <summary>
        ''' Returns the roundingMode setting.
        ''' This will be one of
        ''' <seealso cref=" RoundingMode#CEILING"/>,
        ''' <seealso cref=" RoundingMode#DOWN"/>,
        ''' <seealso cref=" RoundingMode#FLOOR"/>,
        ''' <seealso cref=" RoundingMode#HALF_DOWN"/>,
        ''' <seealso cref=" RoundingMode#HALF_EVEN"/>,
        ''' <seealso cref=" RoundingMode#HALF_UP"/>,
        ''' <seealso cref=" RoundingMode#UNNECESSARY"/>, or
        ''' <seealso cref=" RoundingMode#UP"/>.
        ''' </summary>
        ''' <returns> a {@code RoundingMode} object which is the value of the
        '''         {@code roundingMode} setting </returns>

        Public ReadOnly Property roundingMode As RoundingMode
            Get
                Return _roundingMode
            End Get
        End Property

        ''' <summary>
        ''' Compares this {@code MathContext} with the specified
        ''' {@code Object} for equality.
        ''' </summary>
        ''' <param name="x"> {@code Object} to which this {@code MathContext} is to
        '''         be compared. </param>
        ''' <returns> {@code true} if and only if the specified {@code Object} is
        '''         a {@code MathContext} object which has exactly the same
        '''         settings as this object </returns>
        Public Overrides Function Equals(ByVal x As Object) As Boolean
            Dim mc As MathContext
            If Not (TypeOf x Is MathContext) Then Return False
            mc = CType(x, MathContext)
            Return mc.precision = Me.precision AndAlso mc.roundingMode = Me.roundingMode ' no need for .equals()
        End Function

        ''' <summary>
        ''' Returns the hash code for this {@code MathContext}.
        ''' </summary>
        ''' <returns> hash code for this {@code MathContext} </returns>
        Public Overrides Function GetHashCode() As Integer
            Return Me.precision + roundingMode.GetHashCode() * 59
        End Function

        ''' <summary>
        ''' Returns the string representation of this {@code MathContext}.
        ''' The {@code String} returned represents the settings of the
        ''' {@code MathContext} object as two space-delimited words
        ''' (separated by a single space character, <tt>'&#92;u0020'</tt>,
        ''' and with no leading or trailing white space), as follows:
        ''' <ol>
        ''' <li>
        ''' The string {@code "precision="}, immediately followed
        ''' by the value of the precision setting as a numeric string as if
        ''' generated by the <seealso cref="Integer#toString(int) Integer.toString"/>
        ''' method.
        ''' 
        ''' <li>
        ''' The string {@code "roundingMode="}, immediately
        ''' followed by the value of the {@code roundingMode} setting as a
        ''' word.  This word will be the same as the name of the
        ''' corresponding public constant in the <seealso cref="RoundingMode"/>
        ''' enum.
        ''' </ol>
        ''' <p>
        ''' For example:
        ''' <pre>
        ''' precision=9 roundingMode=HALF_UP
        ''' </pre>
        ''' 
        ''' Additional words may be appended to the result of
        ''' {@code toString} in the future if more properties are added to
        ''' this class.
        ''' </summary>
        ''' <returns> a {@code String} representing the context settings </returns>
        Public Overrides Function ToString() As String
            Return "precision=" & precision & " " & "roundingMode=" & roundingMode.ToString()
        End Function

        ' Private methods

        ''' <summary>
        ''' Reconstitute the {@code MathContext} instance from a stream (that is,
        ''' deserialize it).
        ''' </summary>
        ''' <param name="s"> the stream being read. </param>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            s.defaultReadObject() ' read in all fields
            ' validate possibly bad fields
            If precision < MIN_DIGITS Then
                Dim message As String = "MathContext: invalid digits in stream"
                Throw New java.io.StreamCorruptedException(message)
            End If
            If roundingMode Is Nothing Then
                Dim message As String = "MathContext: null roundingMode in stream"
                Throw New java.io.StreamCorruptedException(message)
            End If
        End Sub

    End Class

End Namespace
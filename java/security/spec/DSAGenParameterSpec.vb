'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Imports java.lang

Namespace java.security.spec

    ''' <summary>
    ''' This immutable class specifies the set of parameters used for
    ''' generating DSA parameters as specified in
    ''' <a href="http://csrc.nist.gov/publications/fips/fips186-3/fips_186-3.pdf">FIPS 186-3 Digital Signature Standard (DSS)</a>.
    ''' </summary>
    ''' <seealso cref= AlgorithmParameterSpec
    ''' 
    ''' @since 8 </seealso>
    Public NotInheritable Class DSAGenParameterSpec
        Implements AlgorithmParameterSpec

        Private ReadOnly pLen As Integer
        Private ReadOnly qLen As Integer
        Private ReadOnly seedLen As Integer

        ''' <summary>
        ''' Creates a domain parameter specification for DSA parameter
        ''' generation using {@code primePLen} and {@code subprimeQLen}.
        ''' The value of {@code subprimeQLen} is also used as the default
        ''' length of the domain parameter seed in bits. </summary>
        ''' <param name="primePLen"> the desired length of the prime P in bits. </param>
        ''' <param name="subprimeQLen"> the desired length of the sub-prime Q in bits. </param>
        ''' <exception cref="IllegalArgumentException"> if {@code primePLen}
        ''' or {@code subprimeQLen} is illegal per the specification of
        ''' FIPS 186-3. </exception>
        Public Sub New(ByVal primePLen As Integer, ByVal subprimeQLen As Integer)
            Me.New(primePLen, subprimeQLen, subprimeQLen)
        End Sub

        ''' <summary>
        ''' Creates a domain parameter specification for DSA parameter
        ''' generation using {@code primePLen}, {@code subprimeQLen},
        ''' and {@code seedLen}. </summary>
        ''' <param name="primePLen"> the desired length of the prime P in bits. </param>
        ''' <param name="subprimeQLen"> the desired length of the sub-prime Q in bits. </param>
        ''' <param name="seedLen"> the desired length of the domain parameter seed in bits,
        ''' shall be equal to or greater than {@code subprimeQLen}. </param>
        ''' <exception cref="IllegalArgumentException"> if {@code primePLenLen},
        ''' {@code subprimeQLen}, or {@code seedLen} is illegal per the
        ''' specification of FIPS 186-3. </exception>
        Public Sub New(ByVal primePLen As Integer, ByVal subprimeQLen As Integer, ByVal seedLen As Integer)
            Select Case primePLen
                Case 1024
                    If subprimeQLen <> 160 Then Throw New IllegalArgumentException("subprimeQLen must be 160 when primePLen=1024")
                Case 2048
                    If subprimeQLen <> 224 AndAlso subprimeQLen <> 256 Then Throw New IllegalArgumentException("subprimeQLen must be 224 or 256 when primePLen=2048")
                Case 3072
                    If subprimeQLen <> 256 Then Throw New IllegalArgumentException("subprimeQLen must be 256 when primePLen=3072")
                Case Else
                    Throw New IllegalArgumentException("primePLen must be 1024, 2048, or 3072")
            End Select
            If seedLen < subprimeQLen Then Throw New IllegalArgumentException("seedLen must be equal to or greater than subprimeQLen")
            Me.pLen = primePLen
            Me.qLen = subprimeQLen
            Me.seedLen = seedLen
        End Sub

        ''' <summary>
        ''' Returns the desired length of the prime P of the
        ''' to-be-generated DSA domain parameters in bits. </summary>
        ''' <returns> the length of the prime P. </returns>
        Public Property primePLength As Integer
            Get
                Return pLen
            End Get
        End Property

        ''' <summary>
        ''' Returns the desired length of the sub-prime Q of the
        ''' to-be-generated DSA domain parameters in bits. </summary>
        ''' <returns> the length of the sub-prime Q. </returns>
        Public Property subprimeQLength As Integer
            Get
                Return qLen
            End Get
        End Property

        ''' <summary>
        ''' Returns the desired length of the domain parameter seed in bits. </summary>
        ''' <returns> the length of the domain parameter seed. </returns>
        Public Property seedLength As Integer
            Get
                Return seedLen
            End Get
        End Property
    End Class

End Namespace
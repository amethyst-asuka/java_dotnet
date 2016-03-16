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

Namespace java.awt

    Public Module ITransparency
        ''' <summary>
        ''' Represents image data that is guaranteed to be completely opaque,
        ''' meaning that all pixels have an alpha value of 1.0.
        ''' </summary>
        Public Const OPAQUE As Integer = 1

        ''' <summary>
        ''' Represents image data that is guaranteed to be either completely
        ''' opaque, with an alpha value of 1.0, or completely transparent,
        ''' with an alpha value of 0.0.
        ''' </summary>

        Public Const BITMASK As Integer = 2

        ''' <summary>
        ''' Represents image data that contains or might contain arbitrary
        ''' alpha values between and including 0.0 and 1.0.
        ''' </summary>
        Public Const TRANSLUCENT As Integer = 3
    End Module

    ''' <summary>
    ''' The <code>Transparency</code> interface defines the common transparency
    ''' modes for implementing classes.
    ''' </summary>
    Public Interface Transparency

        ''' <summary>
        ''' Returns the type of this <code>Transparency</code>. </summary>
        ''' <returns> the field type of this <code>Transparency</code>, which is
        '''          either OPAQUE, BITMASK or TRANSLUCENT. </returns>
        ReadOnly Property transparency As Integer
	End Interface

End Namespace
Imports System

'
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
'    Created by gbp, October 25, 1997
'
' *
' 
'
' **********************************************************************
' **********************************************************************
' **********************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
' *** As  an unpublished  work pursuant to Title 17 of the United    ***
' *** States Code.  All rights reserved.                             ***
' **********************************************************************
' **********************************************************************
' *********************************************************************


Namespace java.awt.Icolor


    ''' <summary>
    ''' This exception is thrown if the native CMM returns an error.
    ''' </summary>

    Public Class CMMException
        Inherits Exception

        ''' <summary>
        '''  Constructs a CMMException with the specified detail message. </summary>
        '''  <param name="s"> the specified detail message </param>
        Public Sub New(ByVal s As String)
            MyBase.New(s)
        End Sub
    End Class

End Namespace
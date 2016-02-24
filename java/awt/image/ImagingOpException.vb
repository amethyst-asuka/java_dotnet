Imports System

'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image


	''' <summary>
	''' The <code>ImagingOpException</code> is thrown if one of the
	''' <seealso cref="BufferedImageOp"/> or <seealso cref="RasterOp"/> filter methods cannot
	''' process the image.
	''' </summary>
	Public Class ImagingOpException
		Inherits Exception

		''' <summary>
		''' Constructs an <code>ImagingOpException</code> object with the
		''' specified message. </summary>
		''' <param name="s"> the message to generate when a
		''' <code>ImagingOpException</code> is thrown </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
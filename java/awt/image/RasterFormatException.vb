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
	''' The <code>RasterFormatException</code> is thrown if there is
	''' invalid layout information in the <seealso cref="Raster"/>.
	''' </summary>
	Public Class RasterFormatException
		Inherits Exception

		''' <summary>
		''' Constructs a new <code>RasterFormatException</code> with the
		''' specified message. </summary>
		''' <param name="s"> the message to generate when a
		''' <code>RasterFormatException</code> is thrown </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
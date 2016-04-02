'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.print

	''' <summary>
	''' The <code>PrinterIOException</code> class is a subclass of
	''' <seealso cref="PrinterException"/> and is used to indicate that an IO error
	''' of some sort has occurred while printing.
	''' 
	''' <p>As of release 1.4, this exception has been retrofitted to conform to
	''' the general purpose exception-chaining mechanism.  The
	''' "<code>IOException</code> that terminated the print job"
	''' that is provided at construction time and accessed via the
	''' <seealso cref="#getIOException()"/> method is now known as the <i>cause</i>,
	''' and may be accessed via the <seealso cref="Throwable#getCause()"/> method,
	''' as well as the aforementioned "legacy method."
	''' </summary>
	Public Class PrinterIOException
		Inherits PrinterException

		Friend Shadows Const serialVersionUID As Long = 5850870712125932846L

		''' <summary>
		''' The IO error that terminated the print job.
		''' @serial
		''' </summary>
		Private mException As java.io.IOException

		''' <summary>
		''' Constructs a new <code>PrinterIOException</code>
		''' with the string representation of the specified
		''' <seealso cref="IOException"/>. </summary>
		''' <param name="exception"> the specified <code>IOException</code> </param>
		Public Sub New(ByVal exception_Renamed As java.io.IOException)
			initCause(Nothing) ' Disallow subsequent initCause
			mException = exception_Renamed
		End Sub

		''' <summary>
		''' Returns the <code>IOException</code> that terminated
		''' the print job.
		''' 
		''' <p>This method predates the general-purpose exception chaining facility.
		''' The <seealso cref="Throwable#getCause()"/> method is now the preferred means of
		''' obtaining this information.
		''' </summary>
		''' <returns> the <code>IOException</code> that terminated
		''' the print job. </returns>
		''' <seealso cref= IOException </seealso>
		Public Overridable Property iOException As java.io.IOException
			Get
				Return mException
			End Get
		End Property

		''' <summary>
		''' Returns the the cause of this exception (the <code>IOException</code>
		''' that terminated the print job).
		''' </summary>
		''' <returns>  the cause of this exception.
		''' @since   1.4 </returns>
		Public  Overrides ReadOnly Property  cause As Throwable
			Get
				Return mException
			End Get
		End Property
	End Class

End Namespace
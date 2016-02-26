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
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream

	''' <summary>
	''' This interface is used to report non-fatal errors.
	''' Only warnings should be echoed through this interface.
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface XMLReporter

		''' 
		''' <summary>
		''' Report the desired message in an application specific format.
		''' 
		''' Only warnings and non-fatal errors should be reported through
		''' 
		''' this interface.
		''' 
		''' Fatal errors should be thrown as XMLStreamException.
		''' 
		''' 
		''' </summary>
		''' <param name="message"> the error message
		''' </param>
		''' <param name="errorType"> an implementation defined error type
		''' </param>
		''' <param name="relatedInformation"> information related to the error, if available
		''' </param>
		''' <param name="location"> the location of the error, if available
		''' </param>
		''' <exception cref="XMLStreamException">
		'''  </exception>
		Sub report(ByVal message As String, ByVal errorType As String, ByVal relatedInformation As Object, ByVal location As Location)
	End Interface

End Namespace
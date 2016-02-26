'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform

	''' <summary>
	''' An object that implements this interface contains the information
	''' needed to act as source input (XML source or transformation instructions).
	''' </summary>
	Public Interface Source

		''' <summary>
		''' Set the system identifier for this Source.
		''' 
		''' <p>The system identifier is optional if the source does not
		''' get its data from a URL, but it may still be useful to provide one.
		''' The application can use a system identifier, for example, to resolve
		''' relative URIs and to include in error messages and warnings.</p>
		''' </summary>
		''' <param name="systemId"> The system identifier as a URL string. </param>
		Property systemId As String

	End Interface

End Namespace
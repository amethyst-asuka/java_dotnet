'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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


Namespace java.security

	''' <summary>
	''' A parameter that contains a URI pointing to data intended for a
	''' PolicySpi or ConfigurationSpi implementation.
	''' 
	''' @since 1.6
	''' </summary>
	Public Class URIParameter
		Implements Policy.Parameters, javax.security.auth.login.Configuration.Parameters

		Private uri As java.net.URI

		''' <summary>
		''' Constructs a URIParameter with the URI pointing to
		''' data intended for an SPI implementation.
		''' </summary>
		''' <param name="uri"> the URI pointing to the data.
		''' </param>
		''' <exception cref="NullPointerException"> if the specified URI is null. </exception>
		Public Sub New(  uri As java.net.URI)
			If uri Is Nothing Then Throw New NullPointerException("invalid null URI")
			Me.uri = uri
		End Sub

		''' <summary>
		''' Returns the URI.
		''' </summary>
		''' <returns> uri the URI. </returns>
		Public Overridable Property uRI As java.net.URI
			Get
				Return uri
			End Get
		End Property
	End Class

End Namespace
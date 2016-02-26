'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.http


	''' <summary>
	''' The <code>HTTPException</code> exception represents a
	'''  XML/HTTP fault.
	''' 
	'''  <p>Since there is no standard format for faults or exceptions
	'''  in XML/HTTP messaging, only the HTTP status code is captured.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Class HTTPException
		Inherits javax.xml.ws.ProtocolException

	  Private statusCode As Integer

	  ''' <summary>
	  ''' Constructor for the HTTPException </summary>
	  '''  <param name="statusCode">   <code>int</code> for the HTTP status code
	  '''  </param>
	  Public Sub New(ByVal statusCode As Integer)
		MyBase.New()
		Me.statusCode = statusCode
	  End Sub

	  ''' <summary>
	  ''' Gets the HTTP status code.
	  ''' </summary>
	  '''  <returns> HTTP status code
	  '''  </returns>
	  Public Overridable Property statusCode As Integer
		  Get
			Return statusCode
		  End Get
	  End Property
	End Class

End Namespace
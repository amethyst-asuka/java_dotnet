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
'
' * $Id: OctetStreamData.java,v 1.3 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto


	''' <summary>
	''' A representation of a <code>Data</code> type containing an octet stream.
	''' 
	''' @since 1.6
	''' </summary>
	Public Class OctetStreamData
		Implements Data

		Private octetStream As java.io.InputStream
		Private uri As String
		Private mimeType As String

		''' <summary>
		''' Creates a new <code>OctetStreamData</code>.
		''' </summary>
		''' <param name="octetStream"> the input stream containing the octets </param>
		''' <exception cref="NullPointerException"> if <code>octetStream</code> is
		'''    <code>null</code> </exception>
		Public Sub New(ByVal octetStream As java.io.InputStream)
			If octetStream Is Nothing Then Throw New NullPointerException("octetStream is null")
			Me.octetStream = octetStream
		End Sub

		''' <summary>
		''' Creates a new <code>OctetStreamData</code>.
		''' </summary>
		''' <param name="octetStream"> the input stream containing the octets </param>
		''' <param name="uri"> the URI String identifying the data object (may be
		'''    <code>null</code>) </param>
		''' <param name="mimeType"> the MIME type associated with the data object (may be
		'''    <code>null</code>) </param>
		''' <exception cref="NullPointerException"> if <code>octetStream</code> is
		'''    <code>null</code> </exception>
		Public Sub New(ByVal octetStream As java.io.InputStream, ByVal uri As String, ByVal mimeType As String)
			If octetStream Is Nothing Then Throw New NullPointerException("octetStream is null")
			Me.octetStream = octetStream
			Me.uri = uri
			Me.mimeType = mimeType
		End Sub

		''' <summary>
		''' Returns the input stream of this <code>OctetStreamData</code>.
		''' </summary>
		''' <returns> the input stream of this <code>OctetStreamData</code>. </returns>
		Public Overridable Property octetStream As java.io.InputStream
			Get
				Return octetStream
			End Get
		End Property

		''' <summary>
		''' Returns the URI String identifying the data object represented by this
		''' <code>OctetStreamData</code>.
		''' </summary>
		''' <returns> the URI String or <code>null</code> if not applicable </returns>
		Public Overridable Property uRI As String
			Get
				Return uri
			End Get
		End Property

		''' <summary>
		''' Returns the MIME type associated with the data object represented by this
		''' <code>OctetStreamData</code>.
		''' </summary>
		''' <returns> the MIME type or <code>null</code> if not applicable </returns>
		Public Overridable Property mimeType As String
			Get
				Return mimeType
			End Get
		End Property
	End Class

End Namespace
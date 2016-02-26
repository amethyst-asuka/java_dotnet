'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap


	''' <summary>
	''' An object that stores a MIME header name and its value. One or more
	''' <code>MimeHeader</code> objects may be contained in a <code>MimeHeaders</code>
	''' object.
	''' </summary>
	''' <seealso cref= MimeHeaders </seealso>
	Public Class MimeHeader

	   Private name As String
	   Private value As String

	   ''' <summary>
	   ''' Constructs a <code>MimeHeader</code> object initialized with the given
	   ''' name and value.
	   ''' </summary>
	   ''' <param name="name"> a <code>String</code> giving the name of the header </param>
	   ''' <param name="value"> a <code>String</code> giving the value of the header </param>
		Public Sub New(ByVal name As String, ByVal value As String)
			Me.name = name
			Me.value = value
		End Sub

		''' <summary>
		''' Returns the name of this <code>MimeHeader</code> object.
		''' </summary>
		''' <returns> the name of the header as a <code>String</code> </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the value of this <code>MimeHeader</code> object.
		''' </summary>
		''' <returns>  the value of the header as a <code>String</code> </returns>
		Public Overridable Property value As String
			Get
				Return value
			End Get
		End Property
	End Class

End Namespace
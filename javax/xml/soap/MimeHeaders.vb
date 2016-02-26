Imports System.Collections

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
	''' A container for <code>MimeHeader</code> objects, which represent
	''' the MIME headers present in a MIME part of a message.
	''' 
	''' <p>This class is used primarily when an application wants to
	''' retrieve specific attachments based on certain MIME headers and
	''' values. This class will most likely be used by implementations of
	''' <code>AttachmentPart</code> and other MIME dependent parts of the SAAJ
	''' API. </summary>
	''' <seealso cref= SOAPMessage#getAttachments </seealso>
	''' <seealso cref= AttachmentPart </seealso>
	Public Class MimeHeaders
		Private headers As ArrayList

	   ''' <summary>
	   ''' Constructs a default <code>MimeHeaders</code> object initialized with
	   ''' an empty <code>Vector</code> object.
	   ''' </summary>
		Public Sub New()
			headers = New ArrayList
		End Sub

		''' <summary>
		''' Returns all of the values for the specified header as an array of
		''' <code>String</code> objects.
		''' </summary>
		''' <param name="name"> the name of the header for which values will be returned </param>
		''' <returns> a <code>String</code> array with all of the values for the
		'''         specified header </returns>
		''' <seealso cref= #setHeader </seealso>
		Public Overridable Function getHeader(ByVal name As String) As String()
			Dim values As New ArrayList

			For i As Integer = 0 To headers.Count - 1
				Dim hdr As MimeHeader = CType(headers(i), MimeHeader)
				If hdr.name.ToUpper() = name.ToUpper() AndAlso hdr.value IsNot Nothing Then values.Add(hdr.value)
			Next i

			If values.Count = 0 Then Return Nothing

			Dim r As String() = New String(values.Count - 1){}
			values.CopyTo(r)
			Return r
		End Function

		''' <summary>
		''' Replaces the current value of the first header entry whose name matches
		''' the given name with the given value, adding a new header if no existing header
		''' name matches. This method also removes all matching headers after the first one.
		''' <P>
		''' Note that RFC822 headers can contain only US-ASCII characters.
		''' </summary>
		''' <param name="name"> a <code>String</code> with the name of the header for
		'''          which to search </param>
		''' <param name="value"> a <code>String</code> with the value that will replace the
		'''          current value of the specified header
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem in the
		''' mime header name or the value being set </exception>
		''' <seealso cref= #getHeader </seealso>
		Public Overridable Sub setHeader(ByVal name As String, ByVal value As String)
			Dim found As Boolean = False

			If (name Is Nothing) OrElse name.Equals("") Then Throw New System.ArgumentException("Illegal MimeHeader name")

			For i As Integer = 0 To headers.Count - 1
				Dim hdr As MimeHeader = CType(headers(i), MimeHeader)
				If hdr.name.ToUpper() = name.ToUpper() Then
					If Not found Then
						headers(i) = New MimeHeader(hdr.name, value)
						found = True
					Else
						headers.RemoveAt(i)
						i -= 1
					End If
				End If
			Next i

			If Not found Then addHeader(name, value)
		End Sub

		''' <summary>
		''' Adds a <code>MimeHeader</code> object with the specified name and value
		''' to this <code>MimeHeaders</code> object's list of headers.
		''' <P>
		''' Note that RFC822 headers can contain only US-ASCII characters.
		''' </summary>
		''' <param name="name"> a <code>String</code> with the name of the header to
		'''          be added </param>
		''' <param name="value"> a <code>String</code> with the value of the header to
		'''          be added
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem in the
		''' mime header name or value being added </exception>
		Public Overridable Sub addHeader(ByVal name As String, ByVal value As String)
			If (name Is Nothing) OrElse name.Equals("") Then Throw New System.ArgumentException("Illegal MimeHeader name")

			Dim pos As Integer = headers.Count

			For i As Integer = pos - 1 To 0 Step -1
				Dim hdr As MimeHeader = CType(headers(i), MimeHeader)
				If hdr.name.ToUpper() = name.ToUpper() Then
					headers.Insert(i+1, New MimeHeader(name, value))
					Return
				End If
			Next i
			headers.Add(New MimeHeader(name, value))
		End Sub

		''' <summary>
		''' Remove all <code>MimeHeader</code> objects whose name matches the
		''' given name.
		''' </summary>
		''' <param name="name"> a <code>String</code> with the name of the header for
		'''          which to search </param>
		Public Overridable Sub removeHeader(ByVal name As String)
			For i As Integer = 0 To headers.Count - 1
				Dim hdr As MimeHeader = CType(headers(i), MimeHeader)
				If hdr.name.ToUpper() = name.ToUpper() Then
					headers.RemoveAt(i)
					i -= 1
				End If
			Next i
		End Sub

		''' <summary>
		''' Removes all the header entries from this <code>MimeHeaders</code> object.
		''' </summary>
		Public Overridable Sub removeAllHeaders()
			headers.Clear()
		End Sub


		''' <summary>
		''' Returns all the <code>MimeHeader</code>s in this <code>MimeHeaders</code> object.
		''' </summary>
		''' <returns>  an <code>Iterator</code> object over this <code>MimeHeaders</code>
		'''          object's list of <code>MimeHeader</code> objects </returns>
		Public Overridable Property allHeaders As IEnumerator
			Get
				Return headers.GetEnumerator()
			End Get
		End Property

		Friend Class MatchingIterator
			Implements IEnumerator

			Private ReadOnly outerInstance As MimeHeaders

			Private match As Boolean
			Private [iterator] As IEnumerator
			Private names As String()
			Private nextHeader As Object

			Friend Sub New(ByVal outerInstance As MimeHeaders, ByVal names As String(), ByVal match As Boolean)
					Me.outerInstance = outerInstance
				Me.match = match
				Me.names = names
				Me.iterator = outerInstance.headers.GetEnumerator()
			End Sub

			Private Function nextMatch() As Object
			next:
				Do While [iterator].hasNext()
					Dim hdr As MimeHeader = CType([iterator].next(), MimeHeader)

					If names Is Nothing Then Return If(match, Nothing, hdr)

					For i As Integer = 0 To names.Length - 1
						If hdr.name.ToUpper() = names(i).ToUpper() Then
							If match Then
								Return hdr
							Else
								GoTo next
							End If
						End If
					Next i
					If Not match Then Return hdr
				Loop
				Return Nothing
			End Function


			Public Overridable Function hasNext() As Boolean
				If nextHeader Is Nothing Then nextHeader = nextMatch()
				Return nextHeader IsNot Nothing
			End Function

			Public Overridable Function [next]() As Object
				' hasNext should've prefetched the header for us,
				' return it.
				If nextHeader IsNot Nothing Then
					Dim ret As Object = nextHeader
					nextHeader = Nothing
					Return ret
				End If
				If hasNext() Then Return nextHeader
				Return Nothing
			End Function

			Public Overridable Sub remove()
				[iterator].remove()
			End Sub
		End Class


		''' <summary>
		''' Returns all the <code>MimeHeader</code> objects whose name matches
		''' a name in the given array of names.
		''' </summary>
		''' <param name="names"> an array of <code>String</code> objects with the names
		'''         for which to search </param>
		''' <returns>  an <code>Iterator</code> object over the <code>MimeHeader</code>
		'''          objects whose name matches one of the names in the given list </returns>
		Public Overridable Function getMatchingHeaders(ByVal names As String()) As IEnumerator
			Return New MatchingIterator(Me, names, True)
		End Function

		''' <summary>
		''' Returns all of the <code>MimeHeader</code> objects whose name does not
		''' match a name in the given array of names.
		''' </summary>
		''' <param name="names"> an array of <code>String</code> objects with the names
		'''         for which to search </param>
		''' <returns>  an <code>Iterator</code> object over the <code>MimeHeader</code>
		'''          objects whose name does not match one of the names in the given list </returns>
		Public Overridable Function getNonMatchingHeaders(ByVal names As String()) As IEnumerator
			Return New MatchingIterator(Me, names, False)
		End Function
	End Class

End Namespace
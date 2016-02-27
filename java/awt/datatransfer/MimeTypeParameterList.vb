Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer



	''' <summary>
	''' An object that encapsulates the parameter list of a MimeType
	''' as defined in RFC 2045 and 2046.
	''' 
	''' @author jeff.dunn@eng.sun.com
	''' </summary>
	Friend Class MimeTypeParameterList
		Implements Cloneable

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			parameters = New Dictionary(Of )
		End Sub

		Public Sub New(ByVal rawdata As String)
			parameters = New Dictionary(Of )

			'    now parse rawdata
			parse(rawdata)
		End Sub

		Public Overrides Function GetHashCode() As Integer
			Dim code As Integer =  java.lang.[Integer].Max_Value/45 ' "random" value for empty lists
			Dim paramName As String = Nothing
			Dim enum_ As System.Collections.IEnumerator(Of String) = Me.names

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While enum_.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				paramName = enum_.nextElement()
				code += paramName.GetHashCode()
				code += Me.get(paramName).GetHashCode()
			Loop

			Return code
		End Function ' hashCode()

		''' <summary>
		''' Two parameter lists are considered equal if they have exactly
		''' the same set of parameter names and associated values. The
		''' order of the parameters is not considered.
		''' </summary>
		Public Overrides Function Equals(ByVal thatObject As Object) As Boolean
			'System.out.println("MimeTypeParameterList.equals("+this+","+thatObject+")");
			If Not(TypeOf thatObject Is MimeTypeParameterList) Then Return False
			Dim that As MimeTypeParameterList = CType(thatObject, MimeTypeParameterList)
			If Me.size() <> that.size() Then Return False
			Dim name As String = Nothing
			Dim thisValue As String = Nothing
			Dim thatValue As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Dim entries As java.util.Set(Of KeyValuePair(Of String, String)) = parameters.entrySet()
			Dim [iterator] As IEnumerator(Of KeyValuePair(Of String, String)) = parameters.GetEnumerator()
			Dim entry As KeyValuePair(Of String, String) = Nothing
			Do While [iterator].MoveNext()
				entry = [iterator].Current
				name = entry.Key
				thisValue = entry.Value
				thatValue = that.parameters(name)
				If (thisValue Is Nothing) OrElse (thatValue Is Nothing) Then
					' both null -> equal, only one null -> not equal
					If thisValue <> thatValue Then Return False
				ElseIf Not thisValue.Equals(thatValue) Then
					Return False
				End If
			Loop ' while iterator

			Return True
		End Function ' equals()

		''' <summary>
		''' A routine for parsing the parameter list out of a String.
		''' </summary>
		Protected Friend Overridable Sub parse(ByVal rawdata As String)
			Dim length As Integer = rawdata.length()
			If length > 0 Then
				Dim currentIndex As Integer = skipWhiteSpace(rawdata, 0)
				Dim lastIndex As Integer = 0

				If currentIndex < length Then
					Dim currentChar As Char = rawdata.Chars(currentIndex)
					Do While (currentIndex < length) AndAlso (currentChar = ";"c)
						Dim name As String
						Dim value As String
						Dim foundit As Boolean

						'    eat the ';'
						currentIndex += 1

						'    now parse the parameter name

						'    skip whitespace
						currentIndex = skipWhiteSpace(rawdata, currentIndex)

						If currentIndex < length Then
							'    find the end of the token char run
							lastIndex = currentIndex
							currentChar = rawdata.Chars(currentIndex)
							Do While (currentIndex < length) AndAlso isTokenChar(currentChar)
								currentIndex += 1
								currentChar = rawdata.Chars(currentIndex)
							Loop
							name = rawdata.Substring(lastIndex, currentIndex - lastIndex).ToLower()

							'    now parse the '=' that separates the name from the value

							'    skip whitespace
							currentIndex = skipWhiteSpace(rawdata, currentIndex)

							If (currentIndex < length) AndAlso (rawdata.Chars(currentIndex) = "="c) Then
								'    eat it and parse the parameter value
								currentIndex += 1

								'    skip whitespace
								currentIndex = skipWhiteSpace(rawdata, currentIndex)

								If currentIndex < length Then
									'    now find out whether or not we have a quoted value
									currentChar = rawdata.Chars(currentIndex)
									If currentChar = """"c Then
										'    yup it's quoted so eat it and capture the quoted string
										currentIndex += 1
										lastIndex = currentIndex

										If currentIndex < length Then
											'    find the next unescqped quote
											foundit = False
											Do While (currentIndex < length) AndAlso Not foundit
												currentChar = rawdata.Chars(currentIndex)
												If currentChar = "\"c Then
													'    found an escape sequence so pass this and the next character
													currentIndex += 2
												ElseIf currentChar = """"c Then
													'    foundit!
													foundit = True
												Else
													currentIndex += 1
												End If
											Loop
											If currentChar = """"c Then
												value = unquote(rawdata.Substring(lastIndex, currentIndex - lastIndex))
												'    eat the quote
												currentIndex += 1
											Else
												Throw New MimeTypeParseException("Encountered unterminated quoted parameter value.")
											End If
										Else
											Throw New MimeTypeParseException("Encountered unterminated quoted parameter value.")
										End If
									ElseIf isTokenChar(currentChar) Then
										'    nope it's an ordinary token so it ends with a non-token char
										lastIndex = currentIndex
										foundit = False
										Do While (currentIndex < length) AndAlso Not foundit
											currentChar = rawdata.Chars(currentIndex)

											If isTokenChar(currentChar) Then
												currentIndex += 1
											Else
												foundit = True
											End If
										Loop
										value = rawdata.Substring(lastIndex, currentIndex - lastIndex)
									Else
										'    it ain't a value
										Throw New MimeTypeParseException("Unexpected character encountered at index " & currentIndex)
									End If

									'    now put the data into the hashtable
									parameters(name) = value
								Else
									Throw New MimeTypeParseException("Couldn't find a value for parameter named " & name)
								End If
							Else
								Throw New MimeTypeParseException("Couldn't find the '=' that separates a parameter name from its value.")
							End If
						Else
							Throw New MimeTypeParseException("Couldn't find parameter name")
						End If

						'    setup the next iteration
						currentIndex = skipWhiteSpace(rawdata, currentIndex)
						If currentIndex < length Then currentChar = rawdata.Chars(currentIndex)
					Loop
					If currentIndex < length Then Throw New MimeTypeParseException("More characters encountered in input than expected.")
				End If
			End If
		End Sub

		''' <summary>
		''' return the number of name-value pairs in this list.
		''' </summary>
		Public Overridable Function size() As Integer
			Return parameters.Count
		End Function

		''' <summary>
		''' Determine whether or not this list is empty.
		''' </summary>
		Public Overridable Property empty As Boolean
			Get
				Return parameters.Count = 0
			End Get
		End Property

		''' <summary>
		''' Retrieve the value associated with the given name, or null if there
		''' is no current association.
		''' </summary>
		Public Overridable Function [get](ByVal name As String) As String
			Return parameters(name.Trim().ToLower())
		End Function

		''' <summary>
		''' Set the value to be associated with the given name, replacing
		''' any previous association.
		''' </summary>
		Public Overridable Sub [set](ByVal name As String, ByVal value As String)
			parameters(name.Trim().ToLower()) = value
		End Sub

		''' <summary>
		''' Remove any value associated with the given name.
		''' </summary>
		Public Overridable Sub remove(ByVal name As String)
			parameters.Remove(name.Trim().ToLower())
		End Sub

		''' <summary>
		''' Retrieve an enumeration of all the names in this list.
		''' </summary>
		Public Overridable Property names As System.Collections.IEnumerator(Of String)
			Get
				Return parameters.Keys.GetEnumerator()
			End Get
		End Property

		Public Overrides Function ToString() As String
			' Heuristic: 8 characters per field
			Dim buffer As New StringBuilder(parameters.Count * 16)

			Dim keys As System.Collections.IEnumerator(Of String) = parameters.Keys.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While keys.hasMoreElements()
				buffer.append("; ")

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim key As String = keys.nextElement()
				buffer.append(key)
				buffer.append("="c)
				   buffer.append(quote(parameters(key)))
			Loop

			Return buffer.ToString()
		End Function

		''' <returns> a clone of this object </returns>

		 Public Overridable Function clone() As Object
			 Dim newObj As MimeTypeParameterList = Nothing
			 Try
				 newObj = CType(MyBase.clone(), MimeTypeParameterList)
			 Catch cannotHappen As CloneNotSupportedException
			 End Try
			 newObj.parameters = CType(parameters.clone(), Hashtable)
			 Return newObj
		 End Function

		Private parameters As Dictionary(Of String, String)

		'    below here be scary parsing related things

		''' <summary>
		''' Determine whether or not a given character belongs to a legal token.
		''' </summary>
		Private Shared Function isTokenChar(ByVal c As Char) As Boolean
			Return ((AscW(c) > &O40) AndAlso (AscW(c) < &O177)) AndAlso (TSPECIALS.IndexOf(c) < 0)
		End Function

		''' <summary>
		''' return the index of the first non white space character in
		''' rawdata at or after index i.
		''' </summary>
		Private Shared Function skipWhiteSpace(ByVal rawdata As String, ByVal i As Integer) As Integer
			Dim length As Integer = rawdata.length()
			If i < length Then
				Dim c As Char = rawdata.Chars(i)
				Do While (i < length) AndAlso Char.IsWhiteSpace(c)
					i += 1
					c = rawdata.Chars(i)
				Loop
			End If

			Return i
		End Function

		''' <summary>
		''' A routine that knows how and when to quote and escape the given value.
		''' </summary>
		Private Shared Function quote(ByVal value As String) As String
			Dim needsQuotes As Boolean = False

			'    check to see if we actually have to quote this thing
			Dim length As Integer = value.length()
			Dim i As Integer = 0
			Do While (i < length) AndAlso Not needsQuotes
				needsQuotes = Not isTokenChar(value.Chars(i))
				i += 1
			Loop

			If needsQuotes Then
				Dim buffer As New StringBuilder(CInt(Fix(length * 1.5)))

				'    add the initial quote
				buffer.append(""""c)

				'    add the properly escaped text
				For i As Integer = 0 To length - 1
					Dim c As Char = value.Chars(i)
					If (c = "\"c) OrElse (c = """"c) Then buffer.append("\"c)
					buffer.append(c)
				Next i

				'    add the closing quote
				buffer.append(""""c)

				Return buffer.ToString()
			Else
				Return value
			End If
		End Function

		''' <summary>
		''' A routine that knows how to strip the quotes and escape sequences from the given value.
		''' </summary>
		Private Shared Function unquote(ByVal value As String) As String
			Dim valueLength As Integer = value.length()
			Dim buffer As New StringBuilder(valueLength)

			Dim escaped As Boolean = False
			For i As Integer = 0 To valueLength - 1
				Dim currentChar As Char = value.Chars(i)
				If (Not escaped) AndAlso (currentChar <> "\"c) Then
					buffer.append(currentChar)
				ElseIf escaped Then
					buffer.append(currentChar)
					escaped = False
				Else
					escaped = True
				End If
			Next i

			Return buffer.ToString()
		End Function

		''' <summary>
		''' A string that holds all the special chars.
		''' </summary>
		Private Shared ReadOnly TSPECIALS As String = "()<>@,;:\""/[]?="

	End Class

End Namespace
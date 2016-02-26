Imports System
Imports System.Collections
Imports System.Text

'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print



	''' <summary>
	''' Class MimeType encapsulates a Multipurpose Internet Mail Extensions (MIME)
	''' media type as defined in <A HREF="http://www.ietf.org/rfc/rfc2045.txt">RFC
	''' 2045</A> and <A HREF="http://www.ietf.org/rfc/rfc2046.txt">RFC 2046</A>. A
	''' MIME type object is part of a <seealso cref="DocFlavor DocFlavor"/> object and
	''' specifies the format of the print data.
	''' <P>
	''' Class MimeType is similar to the like-named
	''' class in package <seealso cref="java.awt.datatransfer java.awt.datatransfer"/>. Class
	''' java.awt.datatransfer.MimeType is not used in the Jini Print Service API
	''' for two reasons:
	''' <OL TYPE=1>
	''' <LI>
	''' Since not all Java profiles include the AWT, the Jini Print Service should
	''' not depend on an AWT class.
	''' <P>
	''' <LI>
	''' The implementation of class java.awt.datatransfer.MimeType does not
	''' guarantee
	''' that equivalent MIME types will have the same serialized representation.
	''' Thus, since the Jini Lookup Service (JLUS) matches service attributes based
	''' on equality of serialized representations, JLUS searches involving MIME
	''' types encapsulated in class java.awt.datatransfer.MimeType may incorrectly
	''' fail to match.
	''' </OL>
	''' <P>
	''' Class MimeType's serialized representation is based on the following
	''' canonical form of a MIME type string. Thus, two MIME types that are not
	''' identical but that are equivalent (that have the same canonical form) will
	''' be considered equal by the JLUS's matching algorithm.
	''' <UL>
	''' <LI> The media type, media subtype, and parameters are retained, but all
	'''      comments and whitespace characters are discarded.
	''' <LI> The media type, media subtype, and parameter names are converted to
	'''      lowercase.
	''' <LI> The parameter values retain their original case, except a charset
	'''      parameter value for a text media type is converted to lowercase.
	''' <LI> Quote characters surrounding parameter values are removed.
	''' <LI> Quoting backslash characters inside parameter values are removed.
	''' <LI> The parameters are arranged in ascending order of parameter name.
	''' </UL>
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Friend Class MimeType
		Implements ICloneable

		Private Const serialVersionUID As Long = -2785720609362367683L

		''' <summary>
		''' Array of strings that hold pieces of this MIME type's canonical form.
		''' If the MIME type has <I>n</I> parameters, <I>n</I> &gt;= 0, then the
		''' strings in the array are:
		''' <BR>Index 0 -- Media type.
		''' <BR>Index 1 -- Media subtype.
		''' <BR>Index 2<I>i</I>+2 -- Name of parameter <I>i</I>,
		''' <I>i</I>=0,1,...,<I>n</I>-1.
		''' <BR>Index 2<I>i</I>+3 -- Value of parameter <I>i</I>,
		''' <I>i</I>=0,1,...,<I>n</I>-1.
		''' <BR>Parameters are arranged in ascending order of parameter name.
		''' @serial
		''' </summary>
		Private myPieces As String()

		''' <summary>
		''' String value for this MIME type. Computed when needed and cached.
		''' </summary>
		<NonSerialized> _
		Private myStringValue As String = Nothing

		''' <summary>
		''' Parameter map entry set. Computed when needed and cached.
		''' </summary>
		<NonSerialized> _
		Private myEntrySet As ParameterMapEntrySet = Nothing

		''' <summary>
		''' Parameter map. Computed when needed and cached.
		''' </summary>
		<NonSerialized> _
		Private myParameterMap As ParameterMap = Nothing

		''' <summary>
		''' Parameter map entry.
		''' </summary>
		Private Class ParameterMapEntry
			Implements DictionaryEntry

			Private ReadOnly outerInstance As MimeType

			Private myIndex As Integer
			Public Sub New(ByVal outerInstance As MimeType, ByVal theIndex As Integer)
					Me.outerInstance = outerInstance
				myIndex = theIndex
			End Sub
			Public Overridable Property key As Object
				Get
					Return outerInstance.myPieces(myIndex)
				End Get
			End Property
			Public Overridable Property value As Object
				Get
					Return outerInstance.myPieces(myIndex+1)
				End Get
			End Property
			Public Overridable Function setValue(ByVal value As Object) As Object
				Throw New System.NotSupportedException
			End Function
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return (o IsNot Nothing AndAlso TypeOf o Is DictionaryEntry AndAlso key.Equals(CType(o, DictionaryEntry).Key) AndAlso value.Equals(CType(o, DictionaryEntry).Value))
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return key.GetHashCode() Xor value.GetHashCode()
			End Function
		End Class

		''' <summary>
		''' Parameter map entry set iterator.
		''' </summary>
		Private Class ParameterMapEntrySetIterator
			Implements IEnumerator

			Private ReadOnly outerInstance As MimeType

			Public Sub New(ByVal outerInstance As MimeType)
				Me.outerInstance = outerInstance
			End Sub

			Private myIndex As Integer = 2
			Public Overridable Function hasNext() As Boolean
				Return myIndex < outerInstance.myPieces.Length
			End Function
			Public Overridable Function [next]() As Object
				If hasNext() Then
					Dim result As New ParameterMapEntry(myIndex)
					myIndex += 2
					Return result
				Else
					Throw New java.util.NoSuchElementException
				End If
			End Function
			Public Overridable Sub remove()
				Throw New System.NotSupportedException
			End Sub
		End Class

		''' <summary>
		''' Parameter map entry set.
		''' </summary>
		Private Class ParameterMapEntrySet
			Inherits java.util.AbstractSet

			Private ReadOnly outerInstance As MimeType

			Public Sub New(ByVal outerInstance As MimeType)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function [iterator]() As IEnumerator
				Return New ParameterMapEntrySetIterator
			End Function
			Public Overridable Function size() As Integer
				Return (outerInstance.myPieces.Length - 2) \ 2
			End Function
		End Class

		''' <summary>
		''' Parameter map.
		''' </summary>
		Private Class ParameterMap
			Inherits java.util.AbstractMap

			Private ReadOnly outerInstance As MimeType

			Public Sub New(ByVal outerInstance As MimeType)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function entrySet() As java.util.Set
				If outerInstance.myEntrySet Is Nothing Then outerInstance.myEntrySet = New ParameterMapEntrySet
				Return outerInstance.myEntrySet
			End Function
		End Class

		''' <summary>
		''' Construct a new MIME type object from the given string. The given
		''' string is converted into canonical form and stored internally.
		''' </summary>
		''' <param name="s">  MIME media type string.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>s</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>s</CODE> does not obey the
		'''     syntax for a MIME media type string. </exception>
		Public Sub New(ByVal s As String)
			parse(s)
		End Sub

		''' <summary>
		''' Returns this MIME type object's MIME type string based on the canonical
		''' form. Each parameter value is enclosed in quotes.
		''' </summary>
		Public Overridable Property mimeType As String
			Get
				Return stringValue
			End Get
		End Property

		''' <summary>
		''' Returns this MIME type object's media type.
		''' </summary>
		Public Overridable Property mediaType As String
			Get
				Return myPieces(0)
			End Get
		End Property

		''' <summary>
		''' Returns this MIME type object's media subtype.
		''' </summary>
		Public Overridable Property mediaSubtype As String
			Get
				Return myPieces(1)
			End Get
		End Property

		''' <summary>
		''' Returns an unmodifiable map view of the parameters in this MIME type
		''' object. Each entry in the parameter map view consists of a parameter
		''' name String (key) mapping to a parameter value String. If this MIME
		''' type object has no parameters, an empty map is returned.
		''' </summary>
		''' <returns>  Parameter map for this MIME type object. </returns>
		Public Overridable Property parameterMap As IDictionary
			Get
				If myParameterMap Is Nothing Then myParameterMap = New ParameterMap(Me)
				Return myParameterMap
			End Get
		End Property

		''' <summary>
		''' Converts this MIME type object to a string.
		''' </summary>
		''' <returns>  MIME type string based on the canonical form. Each parameter
		'''          value is enclosed in quotes. </returns>
		Public Overrides Function ToString() As String
			Return stringValue
		End Function

		''' <summary>
		''' Returns a hash code for this MIME type object.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return stringValue.GetHashCode()
		End Function

		''' <summary>
		''' Determine if this MIME type object is equal to the given object. The two
		''' are equal if the given object is not null, is an instance of class
		''' net.jini.print.data.MimeType, and has the same canonical form as this
		''' MIME type object (that is, has the same type, subtype, and parameters).
		''' Thus, if two MIME type objects are the same except for comments, they are
		''' considered equal. However, "text/plain" and "text/plain;
		''' charset=us-ascii" are not considered equal, even though they represent
		''' the same media type (because the default character set for plain text is
		''' US-ASCII).
		''' </summary>
		''' <param name="obj">  Object to test.
		''' </param>
		''' <returns>  True if this MIME type object equals <CODE>obj</CODE>, false
		'''          otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return (obj IsNot Nothing AndAlso TypeOf obj Is MimeType AndAlso stringValue.Equals(CType(obj, MimeType).stringValue))
		End Function

		''' <summary>
		''' Returns this MIME type's string value in canonical form.
		''' </summary>
		Private Property stringValue As String
			Get
				If myStringValue Is Nothing Then
					Dim result As New StringBuilder
					result.Append(myPieces(0))
					result.Append("/"c)
					result.Append(myPieces(1))
					Dim n As Integer = myPieces.Length
					For i As Integer = 2 To n - 1 Step 2
						result.Append(";"c)
						result.Append(" "c)
						result.Append(myPieces(i))
						result.Append("="c)
						result.Append(addQuotes(myPieces(i+1)))
					Next i
					myStringValue = result.ToString()
				End If
				Return myStringValue
			End Get
		End Property

	' Hidden classes, constants, and operations for parsing a MIME media type
	' string.

		' Lexeme types.
		Private Const TOKEN_LEXEME As Integer = 0
		Private Const QUOTED_STRING_LEXEME As Integer = 1
		Private Const TSPECIAL_LEXEME As Integer = 2
		Private Const EOF_LEXEME As Integer = 3
		Private Const ILLEGAL_LEXEME As Integer = 4

		' Class for a lexical analyzer.
		Private Class LexicalAnalyzer
			Protected Friend mySource As String
			Protected Friend mySourceLength As Integer
			Protected Friend myCurrentIndex As Integer
			Protected Friend myLexemeType As Integer
			Protected Friend myLexemeBeginIndex As Integer
			Protected Friend myLexemeEndIndex As Integer

			Public Sub New(ByVal theSource As String)
				mySource = theSource
				mySourceLength = theSource.Length
				myCurrentIndex = 0
				nextLexeme()
			End Sub

			Public Overridable Property lexemeType As Integer
				Get
					Return myLexemeType
				End Get
			End Property

			Public Overridable Property lexeme As String
				Get
					Return (If(myLexemeBeginIndex >= mySourceLength, Nothing, mySource.Substring(myLexemeBeginIndex, myLexemeEndIndex - myLexemeBeginIndex)))
				End Get
			End Property

			Public Overridable Property lexemeFirstCharacter As Char
				Get
					Return (If(myLexemeBeginIndex >= mySourceLength, ChrW(&H0000), mySource.Chars(myLexemeBeginIndex)))
				End Get
			End Property

			Public Overridable Sub nextLexeme()
				Dim state As Integer = 0
				Dim commentLevel As Integer = 0
				Dim c As Char
				Do While state >= 0
					Select Case state
						' Looking for a token, quoted string, or tspecial
					Case 0
						If myCurrentIndex >= mySourceLength Then
							myLexemeType = EOF_LEXEME
							myLexemeBeginIndex = mySourceLength
							myLexemeEndIndex = mySourceLength
							state = -1
						Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim tempVar As Boolean = Char.IsWhiteSpace(c = mySource.Chars(myCurrentIndex ))
							myCurrentIndex += 1
							If tempVar Then
								state = 0
							ElseIf c = """"c Then
								myLexemeType = QUOTED_STRING_LEXEME
								myLexemeBeginIndex = myCurrentIndex
								state = 1
							ElseIf c = "("c Then
								commentLevel += 1
								state = 3
							ElseIf c = "/"c OrElse c = ";"c OrElse c = "="c OrElse c = ")"c OrElse c = "<"c OrElse c = ">"c OrElse c = "@"c OrElse c = ","c OrElse c = ":"c OrElse c = "\"c OrElse c = "["c OrElse c = "]"c OrElse c = "?"c Then
								myLexemeType = TSPECIAL_LEXEME
								myLexemeBeginIndex = myCurrentIndex - 1
								myLexemeEndIndex = myCurrentIndex
								state = -1
							Else
								myLexemeType = TOKEN_LEXEME
							End If
							myLexemeBeginIndex = myCurrentIndex - 1
							state = 5
							End If
						' In a quoted string
					Case 1
						If myCurrentIndex >= mySourceLength Then
							myLexemeType = ILLEGAL_LEXEME
							myLexemeBeginIndex = mySourceLength
							myLexemeEndIndex = mySourceLength
							state = -1
						Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim tempVar2 As Boolean = (c = mySource.Chars(myCurrentIndex )) = """"c
							myCurrentIndex += 1
							If tempVar2 Then
								myLexemeEndIndex = myCurrentIndex - 1
								state = -1
							ElseIf c = "\"c Then
								state = 2
							Else
								state = 1
							End If
							End If
						' In a quoted string, backslash seen
					Case 2
						If myCurrentIndex >= mySourceLength Then
							myLexemeType = ILLEGAL_LEXEME
							myLexemeBeginIndex = mySourceLength
							myLexemeEndIndex = mySourceLength
							state = -1
						Else
							myCurrentIndex += 1
							state = 1
						End If
						' In a comment
					Case 3
						If myCurrentIndex >= mySourceLength Then
						myLexemeType = ILLEGAL_LEXEME
						myLexemeBeginIndex = mySourceLength
						myLexemeEndIndex = mySourceLength
						state = -1
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Dim tempVar3 As Boolean = (c = mySource.Chars(myCurrentIndex )) = "("c
						myCurrentIndex += 1
						If tempVar3 Then
							commentLevel += 1
							state = 3
						ElseIf c = ")"c Then
							commentLevel -= 1
							state = If(commentLevel = 0, 0, 3)
						ElseIf c = "\"c Then
							state = 4
						Else
							state = 3
						End If
						End If
					' In a comment, backslash seen
					Case 4
						If myCurrentIndex >= mySourceLength Then
							myLexemeType = ILLEGAL_LEXEME
							myLexemeBeginIndex = mySourceLength
							myLexemeEndIndex = mySourceLength
							state = -1
						Else
							myCurrentIndex += 1
							state = 3
						End If
						' In a token
					Case 5
						If myCurrentIndex >= mySourceLength Then
							myLexemeEndIndex = myCurrentIndex
							state = -1
						Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim tempVar4 As Boolean = Char.IsWhiteSpace(c = mySource.Chars(myCurrentIndex ))
							myCurrentIndex += 1
							If tempVar4 Then
								myLexemeEndIndex = myCurrentIndex - 1
								state = -1
							ElseIf c = """"c OrElse c = "("c OrElse c = "/"c OrElse c = ";"c OrElse c = "="c OrElse c = ")"c OrElse c = "<"c OrElse c = ">"c OrElse c = "@"c OrElse c = ","c OrElse c = ":"c OrElse c = "\"c OrElse c = "["c OrElse c = "]"c OrElse c = "?"c Then
								myCurrentIndex -= 1
								myLexemeEndIndex = myCurrentIndex
								state = -1
							Else
								state = 5
							End If
							End If
					End Select
				Loop

			End Sub

		End Class

		''' <summary>
		''' Returns a lowercase version of the given string. The lowercase version
		''' is constructed by applying Character.toLowerCase() to each character of
		''' the given string, which maps characters to lowercase using the rules of
		''' Unicode. This mapping is the same regardless of locale, whereas the
		''' mapping of String.toLowerCase() may be different depending on the
		''' default locale.
		''' </summary>
		Private Shared Function toUnicodeLowerCase(ByVal s As String) As String
			Dim n As Integer = s.Length
			Dim result As Char() = New Char (n - 1){}
			For i As Integer = 0 To n - 1
				result(i) = Char.ToLower(s.Chars(i))
			Next i
			Return New String(result)
		End Function

		''' <summary>
		''' Returns a version of the given string with backslashes removed.
		''' </summary>
		Private Shared Function removeBackslashes(ByVal s As String) As String
			Dim n As Integer = s.Length
			Dim result As Char() = New Char (n - 1){}
			Dim i As Integer
			Dim j As Integer = 0
			Dim c As Char
			For i = 0 To n - 1
				c = s.Chars(i)
				If c = "\"c Then
					i += 1
					c = s.Chars(i)
				End If
				result(j) = c
				j += 1
			Next i
			Return New String(result, 0, j)
		End Function

		''' <summary>
		''' Returns a version of the string surrounded by quotes and with interior
		''' quotes preceded by a backslash.
		''' </summary>
		Private Shared Function addQuotes(ByVal s As String) As String
			Dim n As Integer = s.Length
			Dim i As Integer
			Dim c As Char
			Dim result As New StringBuilder(n+2)
			result.Append(""""c)
			For i = 0 To n - 1
				c = s.Chars(i)
				If c = """"c Then result.Append("\"c)
				result.Append(c)
			Next i
			result.Append(""""c)
			Return result.ToString()
		End Function

		''' <summary>
		''' Parses the given string into canonical pieces and stores the pieces in
		''' <seealso cref="#myPieces <CODE>myPieces</CODE>"/>.
		''' <P>
		''' Special rules applied:
		''' <UL>
		''' <LI> If the media type is text, the value of a charset parameter is
		'''      converted to lowercase.
		''' </UL>
		''' </summary>
		''' <param name="s">  MIME media type string.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>s</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>s</CODE> does not obey the
		'''     syntax for a MIME media type string. </exception>
		Private Sub parse(ByVal s As String)
			' Initialize.
			If s Is Nothing Then Throw New NullPointerException
			Dim theLexer As New LexicalAnalyzer(s)
			Dim theLexemeType As Integer
			Dim thePieces As New ArrayList
			Dim mediaTypeIsText As Boolean = False
			Dim parameterNameIsCharset As Boolean = False

			' Parse media type.
			If theLexer.lexemeType = TOKEN_LEXEME Then
				Dim mt As String = toUnicodeLowerCase(theLexer.lexeme)
				thePieces.Add(mt)
				theLexer.nextLexeme()
				mediaTypeIsText = mt.Equals("text")
			Else
				Throw New System.ArgumentException
			End If
			' Parse slash.
			If theLexer.lexemeType = TSPECIAL_LEXEME AndAlso theLexer.lexemeFirstCharacter = "/"c Then
				theLexer.nextLexeme()
			Else
				Throw New System.ArgumentException
			End If
			If theLexer.lexemeType = TOKEN_LEXEME Then
				thePieces.Add(toUnicodeLowerCase(theLexer.lexeme))
				theLexer.nextLexeme()
			Else
				Throw New System.ArgumentException
			End If
			' Parse zero or more parameters.
			Do While theLexer.lexemeType = TSPECIAL_LEXEME AndAlso theLexer.lexemeFirstCharacter = ";"c
				' Parse semicolon.
				theLexer.nextLexeme()

				' Parse parameter name.
				If theLexer.lexemeType = TOKEN_LEXEME Then
					Dim pn As String = toUnicodeLowerCase(theLexer.lexeme)
					thePieces.Add(pn)
					theLexer.nextLexeme()
					parameterNameIsCharset = pn.Equals("charset")
				Else
					Throw New System.ArgumentException
				End If

				' Parse equals.
				If theLexer.lexemeType = TSPECIAL_LEXEME AndAlso theLexer.lexemeFirstCharacter = "="c Then
					theLexer.nextLexeme()
				Else
					Throw New System.ArgumentException
				End If

				' Parse parameter value.
				If theLexer.lexemeType = TOKEN_LEXEME Then
					Dim pv As String = theLexer.lexeme
					thePieces.Add(If(mediaTypeIsText AndAlso parameterNameIsCharset, toUnicodeLowerCase(pv), pv))
					theLexer.nextLexeme()
				ElseIf theLexer.lexemeType = QUOTED_STRING_LEXEME Then
					Dim pv As String = removeBackslashes(theLexer.lexeme)
					thePieces.Add(If(mediaTypeIsText AndAlso parameterNameIsCharset, toUnicodeLowerCase(pv), pv))
					theLexer.nextLexeme()
				Else
					Throw New System.ArgumentException
				End If
			Loop

			' Make sure we've consumed everything.
			If theLexer.lexemeType <> EOF_LEXEME Then Throw New System.ArgumentException

			' Save the pieces. Parameters are not in ascending order yet.
			Dim n As Integer = thePieces.Count
			myPieces = CType(thePieces.ToArray(GetType(String)), String())

			' Sort the parameters into ascending order using an insertion sort.
			Dim i, j As Integer
			Dim temp As String
			For i = 4 To n - 1 Step 2
				j = 2
				Do While j < i AndAlso myPieces(j).CompareTo(myPieces(i)) <= 0
					j += 2
				Loop
				Do While j < i
					temp = myPieces(j)
					myPieces(j) = myPieces(i)
					myPieces(i) = temp
					temp = myPieces(j+1)
					myPieces(j+1) = myPieces(i+1)
					myPieces(i+1) = temp
					j += 2
				Loop
			Next i
		End Sub
	End Class

End Namespace
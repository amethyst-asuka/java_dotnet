Imports System.Text

'
' * Copyright (c) 1997, 2010, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf


	''' <summary>
	''' A generic superclass for streams which read and parse text
	''' consisting of runs of characters interspersed with occasional
	''' ``specials'' (formatting characters).
	''' 
	''' <p> Most of the functionality
	''' of this class would be redundant except that the
	''' <code>ByteToChar</code> converters
	''' are suddenly private API. Presumably this class will disappear
	''' when the API is made public again. (sigh) That will also let us handle
	''' multibyte character sets...
	''' 
	''' <P> A subclass should override at least <code>write(char)</code>
	''' and <code>writeSpecial(int)</code>. For efficiency's sake it's a
	''' good idea to override <code>write(String)</code> as well. The subclass'
	''' initializer may also install appropriate translation and specials tables.
	''' </summary>
	''' <seealso cref= OutputStream </seealso>
	Friend MustInherit Class AbstractFilter
		Inherits OutputStream

		''' <summary>
		''' A table mapping bytes to characters </summary>
		Protected Friend translationTable As Char()
		''' <summary>
		''' A table indicating which byte values should be interpreted as
		'''  characters and which should be treated as formatting codes 
		''' </summary>
		Protected Friend specialsTable As Boolean()

		''' <summary>
		''' A translation table which does ISO Latin-1 (trivial) </summary>
		Friend Shared ReadOnly latin1TranslationTable As Char()
		''' <summary>
		''' A specials table which indicates that no characters are special </summary>
		Friend Shared ReadOnly noSpecialsTable As Boolean()
		''' <summary>
		''' A specials table which indicates that all characters are special </summary>
		Friend Shared ReadOnly allSpecialsTable As Boolean()

		Shared Sub New()
		  Dim i As Integer

		  noSpecialsTable = New Boolean(255){}
		  For i = 0 To 255
			noSpecialsTable(i) = False
		  Next i

		  allSpecialsTable = New Boolean(255){}
		  For i = 0 To 255
			allSpecialsTable(i) = True
		  Next i

		  latin1TranslationTable = New Char(255){}
		  For i = 0 To 255
			latin1TranslationTable(i) = ChrW(i)
		  Next i
		End Sub

		''' <summary>
		''' A convenience method that reads text from a FileInputStream
		''' and writes it to the receiver.
		''' The format in which the file
		''' is read is determined by the concrete subclass of
		''' AbstractFilter to which this method is sent.
		''' <p>This method does not close the receiver after reaching EOF on
		''' the input stream.
		''' The user must call <code>close()</code> to ensure that all
		''' data are processed.
		''' </summary>
		''' <param name="in">      An InputStream providing text. </param>
		Public Overridable Sub readFromStream(ByVal [in] As InputStream)
			Dim buf As SByte()
			Dim count As Integer

			buf = New SByte(16383){}

			Do
				count = [in].read(buf)
				If count < 0 Then Exit Do

				Me.write(buf, 0, count)
			Loop
		End Sub

		Public Overridable Sub readFromReader(ByVal [in] As Reader)
			Dim buf As Char()
			Dim count As Integer

			buf = New Char(2047){}

			Do
				count = [in].read(buf)
				If count < 0 Then Exit Do
				For i As Integer = 0 To count - 1
				  Me.write(buf(i))
				Next i
			Loop
		End Sub

		Public Sub New()
			translationTable = latin1TranslationTable
			specialsTable = noSpecialsTable
		End Sub

		''' <summary>
		''' Implements the abstract method of OutputStream, of which this class
		''' is a subclass.
		''' </summary>
		Public Overridable Sub write(ByVal b As Integer)
		  If b < 0 Then b += 256
		  If specialsTable(b) Then
			writeSpecial(b)
		  Else
			Dim ch As Char = translationTable(b)
			If ch <> ChrW(0) Then write(ch)
		  End If
		End Sub

		''' <summary>
		''' Implements the buffer-at-a-time write method for greater
		''' efficiency.
		''' 
		''' <p> <strong>PENDING:</strong> Does <code>write(byte[])</code>
		''' call <code>write(byte[], int, int)</code> or is it the other way
		''' around?
		''' </summary>
		Public Overridable Sub write(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer)
		  Dim accumulator As StringBuilder = Nothing
		  Do While len > 0
			Dim b As Short = CShort(buf([off]))

			' stupid signed bytes
			If b < 0 Then b += 256

			If specialsTable(b) Then
			  If accumulator IsNot Nothing Then
				write(accumulator.ToString())
				accumulator = Nothing
			  End If
			  writeSpecial(b)
			Else
			  Dim ch As Char = translationTable(b)
			  If ch <> ChrW(0) Then
				If accumulator Is Nothing Then accumulator = New StringBuilder
				accumulator.Append(ch)
			  End If
			End If

			len -= 1
			[off] += 1
		  Loop

		  If accumulator IsNot Nothing Then write(accumulator.ToString())
		End Sub

		''' <summary>
		''' Hopefully, all subclasses will override this method to accept strings
		''' of text, but if they don't, AbstractFilter's implementation
		''' will spoon-feed them via <code>write(char)</code>.
		''' </summary>
		''' <param name="s"> The string of non-special characters written to the
		'''          OutputStream. </param>
		Public Overridable Sub write(ByVal s As String)
		  Dim index, length As Integer

		  length = s.Length
		  For index = 0 To length - 1
			write(s.Chars(index))
		  Next index
		End Sub

		''' <summary>
		''' Subclasses must provide an implementation of this method which
		''' accepts a single (non-special) character.
		''' </summary>
		''' <param name="ch"> The character written to the OutputStream. </param>
		Protected Friend MustOverride Sub write(ByVal ch As Char)

		''' <summary>
		''' Subclasses must provide an implementation of this method which
		''' accepts a single special byte. No translation is performed
		''' on specials.
		''' </summary>
		''' <param name="b"> The byte written to the OutputStream. </param>
		Protected Friend MustOverride Sub writeSpecial(ByVal b As Integer)
	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql.rowset.serial


	''' <summary>
	''' A serialized mapping in the Java programming language of an SQL
	''' <code>CLOB</code> value.
	''' <P>
	''' The <code>SerialClob</code> class provides a constructor for creating
	''' an instance from a <code>Clob</code> object.  Note that the <code>Clob</code>
	''' object should have brought the SQL <code>CLOB</code> value's data over
	''' to the client before a <code>SerialClob</code> object
	''' is constructed from it.  The data of an SQL <code>CLOB</code> value can
	''' be materialized on the client as a stream of Unicode characters.
	''' <P>
	''' <code>SerialClob</code> methods make it possible to get a substring
	''' from a <code>SerialClob</code> object or to locate the start of
	''' a pattern of characters.
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' <p> A SerialClob is not safe for use by multiple concurrent threads.  If a
	''' SerialClob is to be used by more than one thread then access to the SerialClob
	''' should be controlled by appropriate synchronization.
	''' @author Jonathan Bruce
	''' </summary>
	<Serializable> _
	Public Class SerialClob
		Implements Clob, ICloneable

		''' <summary>
		''' A serialized array of characters containing the data of the SQL
		''' <code>CLOB</code> value that this <code>SerialClob</code> object
		''' represents.
		''' 
		''' @serial
		''' </summary>
		Private buf As Char()

		''' <summary>
		''' Internal Clob representation if SerialClob is initialized with a
		''' Clob. Null if SerialClob is initialized with a char[].
		''' </summary>
		Private clob As Clob

		''' <summary>
		''' The length in characters of this <code>SerialClob</code> object's
		''' internal array of characters.
		''' 
		''' @serial
		''' </summary>
		Private len As Long

		''' <summary>
		''' The original length in characters of this <code>SerialClob</code>
		''' object's internal array of characters.
		''' 
		''' @serial
		''' </summary>
		Private origLen As Long

		''' <summary>
		''' Constructs a <code>SerialClob</code> object that is a serialized version of
		''' the given <code>char</code> array.
		''' <p>
		''' The new <code>SerialClob</code> object is initialized with the data from the
		''' <code>char</code> array, thus allowing disconnected <code>RowSet</code>
		''' objects to establish a serialized <code>Clob</code> object without touching
		''' the data source.
		''' </summary>
		''' <param name="ch"> the char array representing the <code>Clob</code> object to be
		'''         serialized </param>
		''' <exception cref="SerialException"> if an error occurs during serialization </exception>
		''' <exception cref="SQLException"> if a SQL error occurs </exception>
		Public Sub New(ByVal ch As Char())

			' %%% JMB. Agreed. Add code here to throw a SQLException if no
			' support is available for locatorsUpdateCopy=false
			' Serializing locators is not supported.

			len = ch.Length
			buf = New Char(CInt(len) - 1){}
			For i As Integer = 0 To len - 1
			   buf(i) = ch(i)
			Next i
			origLen = len
			clob = Nothing
		End Sub

		''' <summary>
		''' Constructs a <code>SerialClob</code> object that is a serialized
		''' version of the given <code>Clob</code> object.
		''' <P>
		''' The new <code>SerialClob</code> object is initialized with the
		''' data from the <code>Clob</code> object; therefore, the
		''' <code>Clob</code> object should have previously brought the
		''' SQL <code>CLOB</code> value's data over to the client from
		''' the database. Otherwise, the new <code>SerialClob</code> object
		''' object will contain no data.
		''' <p>
		''' Note: The <code>Clob</code> object supplied to this constructor must
		''' return non-null for both the <code>Clob.getCharacterStream()</code>
		''' and <code>Clob.getAsciiStream</code> methods. This <code>SerialClob</code>
		''' constructor cannot serialize a <code>Clob</code> object in this instance
		''' and will throw an <code>SQLException</code> object.
		''' </summary>
		''' <param name="clob"> the <code>Clob</code> object from which this
		'''     <code>SerialClob</code> object is to be constructed; cannot be null </param>
		''' <exception cref="SerialException"> if an error occurs during serialization </exception>
		''' <exception cref="SQLException"> if a SQL error occurs in capturing the CLOB;
		'''     if the <code>Clob</code> object is a null; or if either of the
		'''     <code>Clob.getCharacterStream()</code> and <code>Clob.getAsciiStream()</code>
		'''     methods on the <code>Clob</code> returns a null </exception>
		''' <seealso cref= java.sql.Clob </seealso>
		Public Sub New(ByVal clob As Clob)

			If clob Is Nothing Then Throw New SQLException("Cannot instantiate a SerialClob " & "object with a null Clob object")
			len = clob.length()
			Me.clob = clob
			buf = New Char(CInt(len) - 1){}
			Dim read As Integer = 0
			Dim offset As Integer = 0

			Using charStream As Reader = clob.characterStream
					Try
					If charStream Is Nothing Then Throw New SQLException("Invalid Clob object. The call to getCharacterStream " & "returned null which cannot be serialized.")
        
					' Note: get an ASCII stream in order to null-check it,
					' even though we don't do anything with it.
					Using asciiStream As InputStream = clob.asciiStream
						If asciiStream Is Nothing Then Throw New SQLException("Invalid Clob object. The call to getAsciiStream " & "returned null which cannot be serialized.")
					End Using
        
					Using reader As Reader = New BufferedReader(charStream)
						Do
							read = reader.read(buf, offset, CInt(len - offset))
							offset += read
						Loop While read > 0
					End Using
				Catch ex As java.io.IOException
					Throw New SerialException("SerialClob: " & ex.Message)
				End Try
			End Using

			origLen = len
		End Sub

		''' <summary>
		''' Retrieves the number of characters in this <code>SerialClob</code>
		''' object's array of characters.
		''' </summary>
		''' <returns> a <code>long</code> indicating the length in characters of this
		'''         <code>SerialClob</code> object's array of character </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function length() As Long
			valid
			Return len
		End Function

		''' <summary>
		''' Returns this <code>SerialClob</code> object's data as a stream
		''' of Unicode characters. Unlike the related method, <code>getAsciiStream</code>,
		''' a stream is produced regardless of whether the <code>SerialClob</code> object
		''' was created with a <code>Clob</code> object or a <code>char</code> array.
		''' </summary>
		''' <returns> a <code>java.io.Reader</code> object containing this
		'''         <code>SerialClob</code> object's data </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Property characterStream As java.io.Reader
			Get
				valid
				Return CType(New CharArrayReader(buf), java.io.Reader)
			End Get
		End Property

		''' <summary>
		''' Retrieves the <code>CLOB</code> value designated by this <code>SerialClob</code>
		''' object as an ascii stream. This method forwards the <code>getAsciiStream</code>
		''' call to the underlying <code>Clob</code> object in the event that this
		''' <code>SerialClob</code> object is instantiated with a <code>Clob</code>
		''' object. If this <code>SerialClob</code> object is instantiated with
		''' a <code>char</code> array, a <code>SerialException</code> object is thrown.
		''' </summary>
		''' <returns> a <code>java.io.InputStream</code> object containing
		'''     this <code>SerialClob</code> object's data </returns>
		''' <exception cref="SerialException"> if this {@code SerialClob} object was not
		''' instantiated with a <code>Clob</code> object;
		''' if {@code free} had previously been called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''     <code>CLOB</code> value represented by the <code>Clob</code> object
		''' that was used to create this <code>SerialClob</code> object </exception>
		Public Overridable Property asciiStream As java.io.InputStream
			Get
				valid
				If Me.clob IsNot Nothing Then
					Return Me.clob.asciiStream
				Else
					Throw New SerialException("Unsupported operation. SerialClob cannot " & "return a the CLOB value as an ascii stream, unless instantiated " & "with a fully implemented Clob object.")
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the substring contained in this
		''' <code>SerialClob</code> object, starting at the given position
		''' and continuing for the specified number or characters.
		''' </summary>
		''' <param name="pos"> the position of the first character in the substring
		'''            to be copied; the first character of the
		'''            <code>SerialClob</code> object is at position
		'''            <code>1</code>; must not be less than <code>1</code>,
		'''            and the sum of the starting position and the length
		'''            of the substring must be less than the length of this
		'''            <code>SerialClob</code> object </param>
		''' <param name="length"> the number of characters in the substring to be
		'''               returned; must not be greater than the length of
		'''               this <code>SerialClob</code> object, and the
		'''               sum of the starting position and the length
		'''               of the substring must be less than the length of this
		'''               <code>SerialClob</code> object </param>
		''' <returns> a <code>String</code> object containing a substring of
		'''         this <code>SerialClob</code> object beginning at the
		'''         given position and containing the specified number of
		'''         consecutive characters </returns>
		''' <exception cref="SerialException"> if either of the arguments is out of bounds;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function getSubString(ByVal pos As Long, ByVal length As Integer) As String

			valid
			If pos < 1 OrElse pos > Me.length() Then Throw New SerialException("Invalid position in SerialClob object set")

			If (pos-1) + length > Me.length() Then Throw New SerialException("Invalid position and substring length")

			Try
				Return New String(buf, CInt(pos) - 1, length)

			Catch e As StringIndexOutOfBoundsException
				Throw New SerialException("StringIndexOutOfBoundsException: " & e.Message)
			End Try

		End Function

		''' <summary>
		''' Returns the position in this <code>SerialClob</code> object
		''' where the given <code>String</code> object begins, starting
		''' the search at the specified position. This method returns
		''' <code>-1</code> if the pattern is not found.
		''' </summary>
		''' <param name="searchStr"> the <code>String</code> object for which to
		'''                  search </param>
		''' <param name="start"> the position in this <code>SerialClob</code> object
		'''         at which to start the search; the first position is
		'''         <code>1</code>; must not be less than <code>1</code> nor
		'''         greater than the length of this <code>SerialClob</code> object </param>
		''' <returns> the position at which the given <code>String</code> object
		'''         begins, starting the search at the specified position;
		'''         <code>-1</code> if the given <code>String</code> object is
		'''         not found or the starting position is out of bounds; position
		'''         numbering for the return value starts at <code>1</code> </returns>
		''' <exception cref="SerialException">  if the {@code free} method had been
		''' previously called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the Clob value
		'''         from the database. </exception>
		Public Overridable Function position(ByVal searchStr As String, ByVal start As Long) As Long
			valid
			If start < 1 OrElse start > len Then Return -1

			Dim pattern As Char() = searchStr.ToCharArray()

			Dim pos As Integer = CInt(start)-1
			Dim i As Integer = 0
			Dim patlen As Long = pattern.Length

			Do While pos < len
				If pattern(i) = buf(pos) Then
					If i + 1 = patlen Then Return (pos + 1) - (patlen - 1)
					i += 1 ' increment pos, and i
					pos += 1

				ElseIf pattern(i) <> buf(pos) Then
					pos += 1 ' increment pos only
				End If
			Loop
			Return -1 ' not found
		End Function

		''' <summary>
		''' Returns the position in this <code>SerialClob</code> object
		''' where the given <code>Clob</code> signature begins, starting
		''' the search at the specified position. This method returns
		''' <code>-1</code> if the pattern is not found.
		''' </summary>
		''' <param name="searchStr"> the <code>Clob</code> object for which to search </param>
		''' <param name="start"> the position in this <code>SerialClob</code> object
		'''        at which to begin the search; the first position is
		'''         <code>1</code>; must not be less than <code>1</code> nor
		'''         greater than the length of this <code>SerialClob</code> object </param>
		''' <returns> the position at which the given <code>Clob</code>
		'''         object begins in this <code>SerialClob</code> object,
		'''         at or after the specified starting position </returns>
		''' <exception cref="SerialException"> if an error occurs locating the Clob signature;
		''' if the {@code free} method had been previously called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the Clob value
		'''         from the database </exception>
		Public Overridable Function position(ByVal searchStr As Clob, ByVal start As Long) As Long
			valid
			Return position(searchStr.getSubString(1,CInt(searchStr.length())), start)
		End Function

		''' <summary>
		''' Writes the given Java <code>String</code> to the <code>CLOB</code>
		''' value that this <code>SerialClob</code> object represents, at the position
		''' <code>pos</code>.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to the <code>CLOB</code>
		'''         value that this <code>SerialClob</code> object represents; the first
		'''         position is <code>1</code>; must not be less than <code>1</code> nor
		'''         greater than the length of this <code>SerialClob</code> object </param>
		''' <param name="str"> the string to be written to the <code>CLOB</code>
		'''        value that this <code>SerialClob</code> object represents </param>
		''' <returns> the number of characters written </returns>
		''' <exception cref="SerialException"> if there is an error accessing the
		'''     <code>CLOB</code> value; if an invalid position is set; if an
		'''     invalid offset value is set; if number of bytes to be written
		'''     is greater than the <code>SerialClob</code> length; or the combined
		'''     values of the length and offset is greater than the Clob buffer;
		''' if the {@code free} method had been previously called on this object </exception>
		Public Overridable Function setString(ByVal pos As Long, ByVal str As String) As Integer
			Return (stringing(pos, str, 0, str.Length))
		End Function

		''' <summary>
		''' Writes <code>len</code> characters of <code>str</code>, starting
		''' at character <code>offset</code>, to the <code>CLOB</code> value
		''' that this <code>Clob</code> represents.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to the <code>CLOB</code>
		'''         value that this <code>SerialClob</code> object represents; the first
		'''         position is <code>1</code>; must not be less than <code>1</code> nor
		'''         greater than the length of this <code>SerialClob</code> object </param>
		''' <param name="str"> the string to be written to the <code>CLOB</code>
		'''        value that this <code>Clob</code> object represents </param>
		''' <param name="offset"> the offset into <code>str</code> to start reading
		'''        the characters to be written </param>
		''' <param name="length"> the number of characters to be written </param>
		''' <returns> the number of characters written </returns>
		''' <exception cref="SerialException"> if there is an error accessing the
		'''     <code>CLOB</code> value; if an invalid position is set; if an
		'''     invalid offset value is set; if number of bytes to be written
		'''     is greater than the <code>SerialClob</code> length; or the combined
		'''     values of the length and offset is greater than the Clob buffer;
		''' if the {@code free} method had been previously called on this object </exception>
		Public Overridable Function setString(ByVal pos As Long, ByVal str As String, ByVal offset As Integer, ByVal length As Integer) As Integer
			valid
			Dim temp As String = str.Substring(offset)
			Dim cPattern As Char() = temp.ToCharArray()

			If offset < 0 OrElse offset > str.Length Then Throw New SerialException("Invalid offset in byte array set")

			If pos < 1 OrElse pos > Me.length() Then Throw New SerialException("Invalid position in Clob object set")

			If CLng(length) > origLen Then Throw New SerialException("Buffer is not sufficient to hold the value")

			If (length + offset) > str.Length Then Throw New SerialException("Invalid OffSet. Cannot have combined offset " & " and length that is greater that the Blob buffer")

			Dim i As Integer = 0
			pos -= 1 'values in the array are at position one less
			Do While i < length OrElse (offset + i +1) < (str.Length - offset)
				Me.buf(CInt(pos) + i) = cPattern(offset + i)
				i += 1
			Loop
			Return i
		End Function

		''' <summary>
		''' Retrieves a stream to be used to write Ascii characters to the
		''' <code>CLOB</code> value that this <code>SerialClob</code> object represents,
		''' starting at position <code>pos</code>. This method forwards the
		''' <code>setAsciiStream()</code> call to the underlying <code>Clob</code> object in
		''' the event that this <code>SerialClob</code> object is instantiated with a
		''' <code>Clob</code> object. If this <code>SerialClob</code> object is instantiated
		'''  with a <code>char</code> array, a <code>SerialException</code> object is thrown.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to the
		'''        <code>CLOB</code> object </param>
		''' <returns> the stream to which ASCII encoded characters can be written </returns>
		''' <exception cref="SerialException"> if SerialClob is not instantiated with a
		'''     Clob object;
		''' if the {@code free} method had been previously called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''     <code>CLOB</code> value </exception>
		''' <seealso cref= #getAsciiStream </seealso>
		Public Overridable Function setAsciiStream(ByVal pos As Long) As java.io.OutputStream
			valid
			 If Me.clob IsNot Nothing Then
				 Return Me.clob.asciiStreameam(pos)
			 Else
				 Throw New SerialException("Unsupported operation. SerialClob cannot " & "return a writable ascii stream" & vbLf & " unless instantiated with a Clob object " & "that has a setAsciiStream() implementation")
			 End If
		End Function

		''' <summary>
		''' Retrieves a stream to be used to write a stream of Unicode characters
		''' to the <code>CLOB</code> value that this <code>SerialClob</code> object
		''' represents, at position <code>pos</code>. This method forwards the
		''' <code>setCharacterStream()</code> call to the underlying <code>Clob</code>
		''' object in the event that this <code>SerialClob</code> object is instantiated with a
		''' <code>Clob</code> object. If this <code>SerialClob</code> object is instantiated with
		''' a <code>char</code> array, a <code>SerialException</code> is thrown.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to the
		'''        <code>CLOB</code> value
		''' </param>
		''' <returns> a stream to which Unicode encoded characters can be written </returns>
		''' <exception cref="SerialException"> if the SerialClob is not instantiated with
		'''     a Clob object;
		''' if the {@code free} method had been previously called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>CLOB</code> value </exception>
		''' <seealso cref= #getCharacterStream </seealso>
		Public Overridable Function setCharacterStream(ByVal pos As Long) As java.io.Writer
			valid
			If Me.clob IsNot Nothing Then
				Return Me.clob.characterStreameam(pos)
			Else
				Throw New SerialException("Unsupported operation. SerialClob cannot " & "return a writable character stream" & vbLf & " unless instantiated with a Clob object " & "that has a setCharacterStream implementation")
			End If
		End Function

		''' <summary>
		''' Truncates the <code>CLOB</code> value that this <code>SerialClob</code>
		''' object represents so that it has a length of <code>len</code>
		''' characters.
		''' <p>
		''' Truncating a <code>SerialClob</code> object to length 0 has the effect of
		''' clearing its contents.
		''' </summary>
		''' <param name="length"> the length, in bytes, to which the <code>CLOB</code>
		'''        value should be truncated </param>
		''' <exception cref="SerialException"> if there is an error accessing the
		'''        <code>CLOB</code> value;
		''' if the {@code free} method had been previously called on this object </exception>
		Public Overridable Sub truncate(ByVal length As Long)
			valid
			If length > len Then
			   Throw New SerialException("Length more than what can be truncated")
			Else
				 len = length
				 ' re-size the buffer

				 If len = 0 Then
					buf = New Char() {}
				 Else
					buf = (Me.getSubString(1, CInt(len))).ToCharArray()
				 End If
			End If
		End Sub


		''' <summary>
		''' Returns a {@code Reader} object that contains a partial
		''' {@code SerialClob} value, starting
		''' with the character specified by pos, which is length characters in length.
		''' </summary>
		''' <param name="pos"> the offset to the first character of the partial value to
		''' be retrieved.  The first character in the {@code SerialClob} is at position 1. </param>
		''' <param name="length"> the length in characters of the partial value to be retrieved. </param>
		''' <returns> {@code Reader} through which the partial {@code SerialClob}
		''' value can be read. </returns>
		''' <exception cref="SQLException"> if pos is less than 1 or if pos is greater than the
		''' number of characters in the {@code SerialClob} or if pos + length
		''' is greater than the number of characters in the {@code SerialClob}; </exception>
		''' <exception cref="SerialException"> if the {@code free} method had been previously
		''' called on this object
		''' @since 1.6 </exception>
		Public Overridable Function getCharacterStream(ByVal pos As Long, ByVal length As Long) As Reader
			valid
			If pos < 1 OrElse pos > len Then Throw New SerialException("Invalid position in Clob object set")

			If (pos-1) + length > len Then Throw New SerialException("Invalid position and substring length")
			If length <= 0 Then Throw New SerialException("Invalid length specified")
			Return New CharArrayReader(buf, CInt(pos), CInt(length))
		End Function

		''' <summary>
		''' This method frees the {@code SeriableClob} object and releases the
		''' resources that it holds.
		''' The object is invalid once the {@code free} method is called.
		''' <p>
		''' If {@code free} is called multiple times, the subsequent
		''' calls to {@code free} are treated as a no-op.
		''' </P> </summary>
		''' <exception cref="SQLException"> if an error occurs releasing
		''' the Clob's resources
		''' @since 1.6 </exception>
		Public Overridable Sub free()
			If buf IsNot Nothing Then
				buf = Nothing
				If clob IsNot Nothing Then clob.free()
				clob = Nothing
			End If
		End Sub

		''' <summary>
		''' Compares this SerialClob to the specified object.  The result is {@code
		''' true} if and only if the argument is not {@code null} and is a {@code
		''' SerialClob} object that represents the same sequence of characters as this
		''' object.
		''' </summary>
		''' <param name="obj"> The object to compare this {@code SerialClob} against
		''' </param>
		''' <returns>  {@code true} if the given object represents a {@code SerialClob}
		'''          equivalent to this SerialClob, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is SerialClob Then
				Dim sc As SerialClob = CType(obj, SerialClob)
				If Me.len = sc.len Then Return java.util.Arrays.Equals(buf, sc.buf)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this {@code SerialClob}. </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
		   Return ((31 + java.util.Arrays.hashCode(buf)) * 31 + CInt(len)) * 31 + CInt(origLen)
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialClob}. The copy will contain a
		''' reference to a clone of the internal character array, not a reference
		''' to the original internal character array of this {@code SerialClob} object.
		''' The underlying {@code Clob} object will be set to null.
		''' </summary>
		''' <returns>  a clone of this SerialClob </returns>
		Public Overridable Function clone() As Object
			Try
				Dim sc As SerialClob = CType(MyBase.clone(), SerialClob)
				sc.buf = If(buf IsNot Nothing, java.util.Arrays.copyOf(buf, CInt(len)), Nothing)
				sc.clob = Nothing
				Return sc
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try
		End Function

		''' <summary>
		''' readObject is called to restore the state of the SerialClob from
		''' a stream.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)

			Dim fields As ObjectInputStream.GetField = s.readFields()
		   Dim tmp As Char() = CType(fields.get("buf", Nothing), Char())
		   If tmp Is Nothing Then Throw New InvalidObjectException("buf is null and should not be!")
		   buf = tmp.clone()
		   len = fields.get("len", 0L)
		   If buf.Length <> len Then Throw New InvalidObjectException("buf is not the expected size")
		   origLen = fields.get("origLen", 0L)
		   clob = CType(fields.get("clob", Nothing), Clob)
		End Sub

		''' <summary>
		''' writeObject is called to save the state of the SerialClob
		''' to a stream.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)

			Dim fields As ObjectOutputStream.PutField = s.putFields()
			fields.put("buf", buf)
			fields.put("len", len)
			fields.put("origLen", origLen)
			' Note: this check to see if it is an instance of Serializable
			' is for backwards compatibiity
			fields.put("clob",If(TypeOf clob Is Serializable, clob, Nothing))
			s.writeFields()
		End Sub

		''' <summary>
		''' Check to see if this object had previously had its {@code free} method
		''' called
		''' </summary>
		''' <exception cref="SerialException"> </exception>
		Private Sub isValid()
			If buf Is Nothing Then Throw New SerialException("Error: You cannot call a method on a " & "SerialClob instance once free() has been called.")
		End Sub

		''' <summary>
		''' The identifier that assists in the serialization of this {@code SerialClob}
		''' object.
		''' </summary>
		Friend Const serialVersionUID As Long = -1662519690087375313L
	End Class

End Namespace
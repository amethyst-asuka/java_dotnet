Imports System

'
' * Copyright (c) 2003, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>BLOB</code> value.
	''' <P>
	''' The <code>SerialBlob</code> class provides a constructor for creating
	''' an instance from a <code>Blob</code> object.  Note that the
	''' <code>Blob</code>
	''' object should have brought the SQL <code>BLOB</code> value's data over
	''' to the client before a <code>SerialBlob</code> object
	''' is constructed from it.  The data of an SQL <code>BLOB</code> value can
	''' be materialized on the client as an array of bytes (using the method
	''' <code>Blob.getBytes</code>) or as a stream of uninterpreted bytes
	''' (using the method <code>Blob.getBinaryStream</code>).
	''' <P>
	''' <code>SerialBlob</code> methods make it possible to make a copy of a
	''' <code>SerialBlob</code> object as an array of bytes or as a stream.
	''' They also make it possible to locate a given pattern of bytes or a
	''' <code>Blob</code> object within a <code>SerialBlob</code> object
	''' and to update or truncate a <code>Blob</code> object.
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' <p> A SerialBlob is not safe for use by multiple concurrent threads.  If a
	''' SerialBlob is to be used by more than one thread then access to the SerialBlob
	''' should be controlled by appropriate synchronization.
	''' 
	''' @author Jonathan Bruce
	''' </summary>
	<Serializable> _
	Public Class SerialBlob
		Implements Blob, ICloneable

		''' <summary>
		''' A serialized array of uninterpreted bytes representing the
		''' value of this <code>SerialBlob</code> object.
		''' @serial
		''' </summary>
		Private buf As SByte()

		''' <summary>
		''' The internal representation of the <code>Blob</code> object on which this
		''' <code>SerialBlob</code> object is based.
		''' </summary>
		Private blob As Blob

		''' <summary>
		''' The number of bytes in this <code>SerialBlob</code> object's
		''' array of bytes.
		''' @serial
		''' </summary>
		Private len As Long

		''' <summary>
		''' The original number of bytes in this <code>SerialBlob</code> object's
		''' array of bytes when it was first established.
		''' @serial
		''' </summary>
		Private origLen As Long

		''' <summary>
		''' Constructs a <code>SerialBlob</code> object that is a serialized version of
		''' the given <code>byte</code> array.
		''' <p>
		''' The new <code>SerialBlob</code> object is initialized with the data from the
		''' <code>byte</code> array, thus allowing disconnected <code>RowSet</code>
		''' objects to establish serialized <code>Blob</code> objects without
		''' touching the data source.
		''' </summary>
		''' <param name="b"> the <code>byte</code> array containing the data for the
		'''        <code>Blob</code> object to be serialized </param>
		''' <exception cref="SerialException"> if an error occurs during serialization </exception>
		''' <exception cref="SQLException"> if a SQL errors occurs </exception>
		Public Sub New(ByVal b As SByte())

			len = b.Length
			buf = New SByte(CInt(len) - 1){}
			For i As Integer = 0 To len - 1
				buf(i) = b(i)
			Next i
			origLen = len
		End Sub


		''' <summary>
		''' Constructs a <code>SerialBlob</code> object that is a serialized
		''' version of the given <code>Blob</code> object.
		''' <P>
		''' The new <code>SerialBlob</code> object is initialized with the
		''' data from the <code>Blob</code> object; therefore, the
		''' <code>Blob</code> object should have previously brought the
		''' SQL <code>BLOB</code> value's data over to the client from
		''' the database. Otherwise, the new <code>SerialBlob</code> object
		''' will contain no data.
		''' </summary>
		''' <param name="blob"> the <code>Blob</code> object from which this
		'''     <code>SerialBlob</code> object is to be constructed;
		'''     cannot be null. </param>
		''' <exception cref="SerialException"> if an error occurs during serialization </exception>
		''' <exception cref="SQLException"> if the <code>Blob</code> passed to this
		'''     to this constructor is a <code>null</code>. </exception>
		''' <seealso cref= java.sql.Blob </seealso>
		Public Sub New(ByVal blob As Blob)

			If blob Is Nothing Then Throw New SQLException("Cannot instantiate a SerialBlob object with a null Blob object")

			len = blob.length()
			buf = blob.getBytes(1, CInt(len))
			Me.blob = blob
			origLen = len
		End Sub

		''' <summary>
		''' Copies the specified number of bytes, starting at the given
		''' position, from this <code>SerialBlob</code> object to
		''' another array of bytes.
		''' <P>
		''' Note that if the given number of bytes to be copied is larger than
		''' the length of this <code>SerialBlob</code> object's array of
		''' bytes, the given number will be shortened to the array's length.
		''' </summary>
		''' <param name="pos"> the ordinal position of the first byte in this
		'''            <code>SerialBlob</code> object to be copied;
		'''            numbering starts at <code>1</code>; must not be less
		'''            than <code>1</code> and must be less than or equal
		'''            to the length of this <code>SerialBlob</code> object </param>
		''' <param name="length"> the number of bytes to be copied </param>
		''' <returns> an array of bytes that is a copy of a region of this
		'''         <code>SerialBlob</code> object, starting at the given
		'''         position and containing the given number of consecutive bytes </returns>
		''' <exception cref="SerialException"> if the given starting position is out of bounds;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function getBytes(ByVal pos As Long, ByVal length As Integer) As SByte()
			valid
			If length > len Then length = CInt(len)

			If pos < 1 OrElse len - pos < 0 Then Throw New SerialException("Invalid arguments: position cannot be " & "less than 1 or greater than the length of the SerialBlob")

			pos -= 1 ' correct pos to array index

			Dim b As SByte() = New SByte(length - 1){}

			For i As Integer = 0 To length - 1
				b(i) = Me.buf(CInt(pos))
				pos += 1
			Next i
			Return b
		End Function

		''' <summary>
		''' Retrieves the number of bytes in this <code>SerialBlob</code>
		''' object's array of bytes.
		''' </summary>
		''' <returns> a <code>long</code> indicating the length in bytes of this
		'''         <code>SerialBlob</code> object's array of bytes </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function length() As Long
			valid
			Return len
		End Function

		''' <summary>
		''' Returns this <code>SerialBlob</code> object as an input stream.
		''' Unlike the related method, <code>setBinaryStream</code>,
		''' a stream is produced regardless of whether the <code>SerialBlob</code>
		''' was created with a <code>Blob</code> object or a <code>byte</code> array.
		''' </summary>
		''' <returns> a <code>java.io.InputStream</code> object that contains
		'''         this <code>SerialBlob</code> object's array of bytes </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		''' <seealso cref= #setBinaryStream </seealso>
		Public Overridable Property binaryStream As java.io.InputStream
			Get
				valid
				Dim stream As InputStream = New ByteArrayInputStream(buf)
				Return stream
			End Get
		End Property

		''' <summary>
		''' Returns the position in this <code>SerialBlob</code> object where
		''' the given pattern of bytes begins, starting the search at the
		''' specified position.
		''' </summary>
		''' <param name="pattern"> the pattern of bytes for which to search </param>
		''' <param name="start"> the position of the byte in this
		'''              <code>SerialBlob</code> object from which to begin
		'''              the search; the first position is <code>1</code>;
		'''              must not be less than <code>1</code> nor greater than
		'''              the length of this <code>SerialBlob</code> object </param>
		''' <returns> the position in this <code>SerialBlob</code> object
		'''         where the given pattern begins, starting at the specified
		'''         position; <code>-1</code> if the pattern is not found
		'''         or the given starting position is out of bounds; position
		'''         numbering for the return value starts at <code>1</code> </returns>
		''' <exception cref="SerialException"> if an error occurs when serializing the blob;
		''' if {@code free} had previously been called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the <code>BLOB</code>
		'''         value from the database </exception>
		Public Overridable Function position(ByVal pattern As SByte(), ByVal start As Long) As Long

			valid
			If start < 1 OrElse start > len Then Return -1

			Dim pos As Integer = CInt(start)-1 ' internally Blobs are stored as arrays.
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
		''' Returns the position in this <code>SerialBlob</code> object where
		''' the given <code>Blob</code> object begins, starting the search at the
		''' specified position.
		''' </summary>
		''' <param name="pattern"> the <code>Blob</code> object for which to search; </param>
		''' <param name="start"> the position of the byte in this
		'''              <code>SerialBlob</code> object from which to begin
		'''              the search; the first position is <code>1</code>;
		'''              must not be less than <code>1</code> nor greater than
		'''              the length of this <code>SerialBlob</code> object </param>
		''' <returns> the position in this <code>SerialBlob</code> object
		'''         where the given <code>Blob</code> object begins, starting
		'''         at the specified position; <code>-1</code> if the pattern is
		'''         not found or the given starting position is out of bounds;
		'''         position numbering for the return value starts at <code>1</code> </returns>
		''' <exception cref="SerialException"> if an error occurs when serializing the blob;
		''' if {@code free} had previously been called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the <code>BLOB</code>
		'''         value from the database </exception>
		Public Overridable Function position(ByVal pattern As Blob, ByVal start As Long) As Long
			valid
			Return position(pattern.getBytes(1, CInt(pattern.length())), start)
		End Function

		''' <summary>
		''' Writes the given array of bytes to the <code>BLOB</code> value that
		''' this <code>Blob</code> object represents, starting at position
		''' <code>pos</code>, and returns the number of bytes written.
		''' </summary>
		''' <param name="pos"> the position in the SQL <code>BLOB</code> value at which
		'''     to start writing. The first position is <code>1</code>;
		'''     must not be less than <code>1</code> nor greater than
		'''     the length of this <code>SerialBlob</code> object. </param>
		''' <param name="bytes"> the array of bytes to be written to the <code>BLOB</code>
		'''        value that this <code>Blob</code> object represents </param>
		''' <returns> the number of bytes written </returns>
		''' <exception cref="SerialException"> if there is an error accessing the
		'''     <code>BLOB</code> value; or if an invalid position is set; if an
		'''     invalid offset value is set;
		''' if {@code free} had previously been called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the <code>BLOB</code>
		'''         value from the database </exception>
		''' <seealso cref= #getBytes </seealso>
		Public Overridable Function setBytes(ByVal pos As Long, ByVal bytes As SByte()) As Integer
			Return bytestes(pos, bytes, 0, bytes.Length)
		End Function

		''' <summary>
		''' Writes all or part of the given <code>byte</code> array to the
		''' <code>BLOB</code> value that this <code>Blob</code> object represents
		''' and returns the number of bytes written.
		''' Writing starts at position <code>pos</code> in the <code>BLOB</code>
		''' value; <i>len</i> bytes from the given byte array are written.
		''' </summary>
		''' <param name="pos"> the position in the <code>BLOB</code> object at which
		'''     to start writing. The first position is <code>1</code>;
		'''     must not be less than <code>1</code> nor greater than
		'''     the length of this <code>SerialBlob</code> object. </param>
		''' <param name="bytes"> the array of bytes to be written to the <code>BLOB</code>
		'''     value </param>
		''' <param name="offset"> the offset in the <code>byte</code> array at which
		'''     to start reading the bytes. The first offset position is
		'''     <code>0</code>; must not be less than <code>0</code> nor greater
		'''     than the length of the <code>byte</code> array </param>
		''' <param name="length"> the number of bytes to be written to the
		'''     <code>BLOB</code> value from the array of bytes <i>bytes</i>.
		''' </param>
		''' <returns> the number of bytes written </returns>
		''' <exception cref="SerialException"> if there is an error accessing the
		'''     <code>BLOB</code> value; if an invalid position is set; if an
		'''     invalid offset value is set; if number of bytes to be written
		'''     is greater than the <code>SerialBlob</code> length; or the combined
		'''     values of the length and offset is greater than the Blob buffer;
		''' if {@code free} had previously been called on this object </exception>
		''' <exception cref="SQLException"> if there is an error accessing the <code>BLOB</code>
		'''         value from the database. </exception>
		''' <seealso cref= #getBytes </seealso>
		Public Overridable Function setBytes(ByVal pos As Long, ByVal bytes As SByte(), ByVal offset As Integer, ByVal length As Integer) As Integer

			valid
			If offset < 0 OrElse offset > bytes.Length Then Throw New SerialException("Invalid offset in byte array set")

			If pos < 1 OrElse pos > Me.length() Then Throw New SerialException("Invalid position in BLOB object set")

			If CLng(length) > origLen Then Throw New SerialException("Buffer is not sufficient to hold the value")

			If (length + offset) > bytes.Length Then Throw New SerialException("Invalid OffSet. Cannot have combined offset " & "and length that is greater that the Blob buffer")

			Dim i As Integer = 0
			pos -= 1 ' correct to array indexing
			Do While i < length OrElse (offset + i +1) < (bytes.Length-offset)
				Me.buf(CInt(pos) + i) = bytes(offset + i)
				i += 1
			Loop
			Return i
		End Function

		''' <summary>
		''' Retrieves a stream that can be used to write to the <code>BLOB</code>
		''' value that this <code>Blob</code> object represents.  The stream begins
		''' at position <code>pos</code>. This method forwards the
		''' <code>setBinaryStream()</code> call to the underlying <code>Blob</code> in
		''' the event that this <code>SerialBlob</code> object is instantiated with a
		''' <code>Blob</code>. If this <code>SerialBlob</code> is instantiated with
		''' a <code>byte</code> array, a <code>SerialException</code> is thrown.
		''' </summary>
		''' <param name="pos"> the position in the <code>BLOB</code> value at which
		'''        to start writing </param>
		''' <returns> a <code>java.io.OutputStream</code> object to which data can
		'''         be written </returns>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>BLOB</code> value </exception>
		''' <exception cref="SerialException"> if the SerialBlob in not instantiated with a
		'''     <code>Blob</code> object that supports <code>setBinaryStream()</code>;
		''' if {@code free} had previously been called on this object </exception>
		''' <seealso cref= #getBinaryStream </seealso>
		Public Overridable Function setBinaryStream(ByVal pos As Long) As java.io.OutputStream

			valid
			If Me.blob IsNot Nothing Then
				Return Me.blob.binaryStreameam(pos)
			Else
				Throw New SerialException("Unsupported operation. SerialBlob cannot " & "return a writable binary stream, unless instantiated with a Blob object " & "that provides a setBinaryStream() implementation")
			End If
		End Function

		''' <summary>
		''' Truncates the <code>BLOB</code> value that this <code>Blob</code>
		''' object represents to be <code>len</code> bytes in length.
		''' </summary>
		''' <param name="length"> the length, in bytes, to which the <code>BLOB</code>
		'''        value that this <code>Blob</code> object represents should be
		'''        truncated </param>
		''' <exception cref="SerialException"> if there is an error accessing the Blob value;
		'''     or the length to truncate is greater that the SerialBlob length;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Sub truncate(ByVal length As Long)
			valid
			If length > len Then
				Throw New SerialException("Length more than what can be truncated")
			ElseIf CInt(length) = 0 Then
				buf = New SByte(){}
				len = length
			Else
				len = length
				buf = Me.getBytes(1, CInt(len))
			End If
		End Sub


		''' <summary>
		''' Returns an
		''' <code>InputStream</code> object that contains a partial
		''' {@code Blob} value, starting with the byte specified by pos, which is
		''' length bytes in length.
		''' </summary>
		''' <param name="pos"> the offset to the first byte of the partial value to be
		''' retrieved. The first byte in the {@code Blob} is at position 1 </param>
		''' <param name="length"> the length in bytes of the partial value to be retrieved
		''' @return
		''' <code>InputStream</code> through which the partial {@code Blob} value can
		''' be read. </param>
		''' <exception cref="SQLException"> if pos is less than 1 or if pos is greater than the
		''' number of bytes in the {@code Blob} or if pos + length is greater than
		''' the number of bytes in the {@code Blob} </exception>
		''' <exception cref="SerialException"> if the {@code free} method had been previously
		''' called on this object
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function getBinaryStream(ByVal pos As Long, ByVal length As Long) As InputStream
			valid
			If pos < 1 OrElse pos > Me.length() Then Throw New SerialException("Invalid position in BLOB object set")
			If length < 1 OrElse length > len - pos + 1 Then Throw New SerialException("length is < 1 or pos + length > total number of bytes")
			Return New ByteArrayInputStream(buf, CInt(pos) - 1, CInt(length))
		End Function


		''' <summary>
		''' This method frees the {@code SeriableBlob} object and releases the
		''' resources that it holds. The object is invalid once the {@code free}
		''' method is called. <p> If {@code free} is called multiple times, the
		''' subsequent calls to {@code free} are treated as a no-op. </P>
		''' </summary>
		''' <exception cref="SQLException"> if an error occurs releasing the Blob's resources
		''' @since 1.6 </exception>
		Public Overridable Sub free()
			If buf IsNot Nothing Then
				buf = Nothing
				If blob IsNot Nothing Then blob.free()
				blob = Nothing
			End If
		End Sub

		''' <summary>
		''' Compares this SerialBlob to the specified object.  The result is {@code
		''' true} if and only if the argument is not {@code null} and is a {@code
		''' SerialBlob} object that represents the same sequence of bytes as this
		''' object.
		''' </summary>
		''' <param name="obj"> The object to compare this {@code SerialBlob} against
		''' </param>
		''' <returns> {@code true} if the given object represents a {@code SerialBlob}
		'''          equivalent to this SerialBlob, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is SerialBlob Then
				Dim sb As SerialBlob = CType(obj, SerialBlob)
				If Me.len = sb.len Then Return java.util.Arrays.Equals(buf, sb.buf)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this {@code SerialBlob}. </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
		   Return ((31 + java.util.Arrays.hashCode(buf)) * 31 + CInt(len)) * 31 + CInt(origLen)
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialBlob}. The copy will contain a
		''' reference to a clone of the internal byte array, not a reference
		''' to the original internal byte array of this {@code SerialBlob} object.
		''' The underlying {@code Blob} object will be set to null.
		''' </summary>
		''' <returns>  a clone of this SerialBlob </returns>
		Public Overridable Function clone() As Object
			Try
				Dim sb As SerialBlob = CType(MyBase.clone(), SerialBlob)
				sb.buf = If(buf IsNot Nothing, java.util.Arrays.copyOf(buf, CInt(len)), Nothing)
				sb.blob = Nothing
				Return sb
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try
		End Function

		''' <summary>
		''' readObject is called to restore the state of the SerialBlob from
		''' a stream.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)

			Dim fields As ObjectInputStream.GetField = s.readFields()
			Dim tmp As SByte() = CType(fields.get("buf", Nothing), SByte())
			If tmp Is Nothing Then Throw New InvalidObjectException("buf is null and should not be!")
			buf = tmp.clone()
			len = fields.get("len", 0L)
			If buf.Length <> len Then Throw New InvalidObjectException("buf is not the expected size")
			origLen = fields.get("origLen", 0L)
			blob = CType(fields.get("blob", Nothing), Blob)
		End Sub

		''' <summary>
		''' writeObject is called to save the state of the SerialBlob
		''' to a stream.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)

			Dim fields As ObjectOutputStream.PutField = s.putFields()
			fields.put("buf", buf)
			fields.put("len", len)
			fields.put("origLen", origLen)
			' Note: this check to see if it is an instance of Serializable
			' is for backwards compatibiity
			fields.put("blob",If(TypeOf blob Is Serializable, blob, Nothing))
			s.writeFields()
		End Sub

		''' <summary>
		''' Check to see if this object had previously had its {@code free} method
		''' called
		''' </summary>
		''' <exception cref="SerialException"> </exception>
		Private Sub isValid()
			If buf Is Nothing Then Throw New SerialException("Error: You cannot call a method on a " & "SerialBlob instance once free() has been called.")
		End Sub

		''' <summary>
		''' The identifier that assists in the serialization of this
		''' {@code SerialBlob} object.
		''' </summary>
		Friend Const serialVersionUID As Long = -8144641928112860441L
	End Class

End Namespace
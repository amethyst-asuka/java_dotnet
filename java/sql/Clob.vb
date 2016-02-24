'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.sql


	''' <summary>
	''' The mapping in the Java&trade; programming language
	''' for the SQL <code>CLOB</code> type.
	''' An SQL <code>CLOB</code> is a built-in type
	''' that stores a Character Large Object as a column value in a row of
	''' a database table.
	''' By default drivers implement a <code>Clob</code> object using an SQL
	''' <code>locator(CLOB)</code>, which means that a <code>Clob</code> object
	''' contains a logical pointer to the SQL <code>CLOB</code> data rather than
	''' the data itself. A <code>Clob</code> object is valid for the duration
	''' of the transaction in which it was created.
	''' <P>The <code>Clob</code> interface provides methods for getting the
	''' length of an SQL <code>CLOB</code> (Character Large Object) value,
	''' for materializing a <code>CLOB</code> value on the client, and for
	''' searching for a substring or <code>CLOB</code> object within a
	''' <code>CLOB</code> value.
	''' Methods in the interfaces <seealso cref="ResultSet"/>,
	''' <seealso cref="CallableStatement"/>, and <seealso cref="PreparedStatement"/>, such as
	''' <code>getClob</code> and <code>setClob</code> allow a programmer to
	''' access an SQL <code>CLOB</code> value.  In addition, this interface
	''' has methods for updating a <code>CLOB</code> value.
	''' <p>
	''' All methods on the <code>Clob</code> interface must be fully implemented if the
	''' JDBC driver supports the data type.
	''' 
	''' @since 1.2
	''' </summary>

	Public Interface Clob

	  ''' <summary>
	  ''' Retrieves the number of characters
	  ''' in the <code>CLOB</code> value
	  ''' designated by this <code>Clob</code> object.
	  ''' </summary>
	  ''' <returns> length of the <code>CLOB</code> in characters </returns>
	  ''' <exception cref="SQLException"> if there is an error accessing the
	  '''            length of the <code>CLOB</code> value </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Function length() As Long

	  ''' <summary>
	  ''' Retrieves a copy of the specified substring
	  ''' in the <code>CLOB</code> value
	  ''' designated by this <code>Clob</code> object.
	  ''' The substring begins at position
	  ''' <code>pos</code> and has up to <code>length</code> consecutive
	  ''' characters.
	  ''' </summary>
	  ''' <param name="pos"> the first character of the substring to be extracted.
	  '''            The first character is at position 1. </param>
	  ''' <param name="length"> the number of consecutive characters to be copied;
	  ''' the value for length must be 0 or greater </param>
	  ''' <returns> a <code>String</code> that is the specified substring in
	  '''         the <code>CLOB</code> value designated by this <code>Clob</code> object </returns>
	  ''' <exception cref="SQLException"> if there is an error accessing the
	  '''            <code>CLOB</code> value; if pos is less than 1 or length is
	  ''' less than 0 </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Function getSubString(ByVal pos As Long, ByVal length As Integer) As String

	  ''' <summary>
	  ''' Retrieves the <code>CLOB</code> value designated by this <code>Clob</code>
	  ''' object as a <code>java.io.Reader</code> object (or as a stream of
	  ''' characters).
	  ''' </summary>
	  ''' <returns> a <code>java.io.Reader</code> object containing the
	  '''         <code>CLOB</code> data </returns>
	  ''' <exception cref="SQLException"> if there is an error accessing the
	  '''            <code>CLOB</code> value </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method </exception>
	  ''' <seealso cref= #setCharacterStream
	  ''' @since 1.2 </seealso>
	  ReadOnly Property characterStream As java.io.Reader

	  ''' <summary>
	  ''' Retrieves the <code>CLOB</code> value designated by this <code>Clob</code>
	  ''' object as an ascii stream.
	  ''' </summary>
	  ''' <returns> a <code>java.io.InputStream</code> object containing the
	  '''         <code>CLOB</code> data </returns>
	  ''' <exception cref="SQLException"> if there is an error accessing the
	  '''            <code>CLOB</code> value </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method </exception>
	  ''' <seealso cref= #setAsciiStream
	  ''' @since 1.2 </seealso>
	  ReadOnly Property asciiStream As java.io.InputStream

	  ''' <summary>
	  ''' Retrieves the character position at which the specified substring
	  ''' <code>searchstr</code> appears in the SQL <code>CLOB</code> value
	  ''' represented by this <code>Clob</code> object.  The search
	  ''' begins at position <code>start</code>.
	  ''' </summary>
	  ''' <param name="searchstr"> the substring for which to search </param>
	  ''' <param name="start"> the position at which to begin searching; the first position
	  '''              is 1 </param>
	  ''' <returns> the position at which the substring appears or -1 if it is not
	  '''         present; the first position is 1 </returns>
	  ''' <exception cref="SQLException"> if there is an error accessing the
	  '''            <code>CLOB</code> value or if pos is less than 1 </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Function position(ByVal searchstr As String, ByVal start As Long) As Long

	  ''' <summary>
	  ''' Retrieves the character position at which the specified
	  ''' <code>Clob</code> object <code>searchstr</code> appears in this
	  ''' <code>Clob</code> object.  The search begins at position
	  ''' <code>start</code>.
	  ''' </summary>
	  ''' <param name="searchstr"> the <code>Clob</code> object for which to search </param>
	  ''' <param name="start"> the position at which to begin searching; the first
	  '''              position is 1 </param>
	  ''' <returns> the position at which the <code>Clob</code> object appears
	  '''              or -1 if it is not present; the first position is 1 </returns>
	  ''' <exception cref="SQLException"> if there is an error accessing the
	  '''            <code>CLOB</code> value or if start is less than 1 </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Function position(ByVal searchstr As Clob, ByVal start As Long) As Long

		'---------------------------- jdbc 3.0 -----------------------------------

		''' <summary>
		''' Writes the given Java <code>String</code> to the <code>CLOB</code>
		''' value that this <code>Clob</code> object designates at the position
		''' <code>pos</code>. The string will overwrite the existing characters
		''' in the <code>Clob</code> object starting at the position
		''' <code>pos</code>.  If the end of the <code>Clob</code> value is reached
		''' while writing the given string, then the length of the <code>Clob</code>
		''' value will be increased to accommodate the extra characters.
		''' <p>
		''' <b>Note:</b> If the value specified for <code>pos</code>
		''' is greater then the length+1 of the <code>CLOB</code> value then the
		''' behavior is undefined. Some JDBC drivers may throw a
		''' <code>SQLException</code> while other drivers may support this
		''' operation.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to the <code>CLOB</code>
		'''         value that this <code>Clob</code> object represents;
		''' The first position is 1 </param>
		''' <param name="str"> the string to be written to the <code>CLOB</code>
		'''        value that this <code>Clob</code> designates </param>
		''' <returns> the number of characters written </returns>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>CLOB</code> value or if pos is less than 1
		''' </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.4 </exception>
		Function setString(ByVal pos As Long, ByVal str As String) As Integer

		''' <summary>
		''' Writes <code>len</code> characters of <code>str</code>, starting
		''' at character <code>offset</code>, to the <code>CLOB</code> value
		''' that this <code>Clob</code> represents.  The string will overwrite the existing characters
		''' in the <code>Clob</code> object starting at the position
		''' <code>pos</code>.  If the end of the <code>Clob</code> value is reached
		''' while writing the given string, then the length of the <code>Clob</code>
		''' value will be increased to accommodate the extra characters.
		''' <p>
		''' <b>Note:</b> If the value specified for <code>pos</code>
		''' is greater then the length+1 of the <code>CLOB</code> value then the
		''' behavior is undefined. Some JDBC drivers may throw a
		''' <code>SQLException</code> while other drivers may support this
		''' operation.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to this
		'''        <code>CLOB</code> object; The first position  is 1 </param>
		''' <param name="str"> the string to be written to the <code>CLOB</code>
		'''        value that this <code>Clob</code> object represents </param>
		''' <param name="offset"> the offset into <code>str</code> to start reading
		'''        the characters to be written </param>
		''' <param name="len"> the number of characters to be written </param>
		''' <returns> the number of characters written </returns>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>CLOB</code> value or if pos is less than 1
		''' </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.4 </exception>
		Function setString(ByVal pos As Long, ByVal str As String, ByVal offset As Integer, ByVal len As Integer) As Integer

		''' <summary>
		''' Retrieves a stream to be used to write Ascii characters to the
		''' <code>CLOB</code> value that this <code>Clob</code> object represents,
		''' starting at position <code>pos</code>.  Characters written to the stream
		''' will overwrite the existing characters
		''' in the <code>Clob</code> object starting at the position
		''' <code>pos</code>.  If the end of the <code>Clob</code> value is reached
		''' while writing characters to the stream, then the length of the <code>Clob</code>
		''' value will be increased to accommodate the extra characters.
		''' <p>
		''' <b>Note:</b> If the value specified for <code>pos</code>
		''' is greater then the length+1 of the <code>CLOB</code> value then the
		''' behavior is undefined. Some JDBC drivers may throw a
		''' <code>SQLException</code> while other drivers may support this
		''' operation.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to this
		'''        <code>CLOB</code> object; The first position is 1 </param>
		''' <returns> the stream to which ASCII encoded characters can be written </returns>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>CLOB</code> value or if pos is less than 1 </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method </exception>
		''' <seealso cref= #getAsciiStream
		''' 
		''' @since 1.4 </seealso>
		Function setAsciiStream(ByVal pos As Long) As java.io.OutputStream

		''' <summary>
		''' Retrieves a stream to be used to write a stream of Unicode characters
		''' to the <code>CLOB</code> value that this <code>Clob</code> object
		''' represents, at position <code>pos</code>. Characters written to the stream
		''' will overwrite the existing characters
		''' in the <code>Clob</code> object starting at the position
		''' <code>pos</code>.  If the end of the <code>Clob</code> value is reached
		''' while writing characters to the stream, then the length of the <code>Clob</code>
		''' value will be increased to accommodate the extra characters.
		''' <p>
		''' <b>Note:</b> If the value specified for <code>pos</code>
		''' is greater then the length+1 of the <code>CLOB</code> value then the
		''' behavior is undefined. Some JDBC drivers may throw a
		''' <code>SQLException</code> while other drivers may support this
		''' operation.
		''' </summary>
		''' <param name="pos"> the position at which to start writing to the
		'''        <code>CLOB</code> value; The first position is 1
		''' </param>
		''' <returns> a stream to which Unicode encoded characters can be written </returns>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>CLOB</code> value or if pos is less than 1 </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method </exception>
		''' <seealso cref= #getCharacterStream
		''' 
		''' @since 1.4 </seealso>
		Function setCharacterStream(ByVal pos As Long) As java.io.Writer

		''' <summary>
		''' Truncates the <code>CLOB</code> value that this <code>Clob</code>
		''' designates to have a length of <code>len</code>
		''' characters.
		''' <p>
		''' <b>Note:</b> If the value specified for <code>pos</code>
		''' is greater then the length+1 of the <code>CLOB</code> value then the
		''' behavior is undefined. Some JDBC drivers may throw a
		''' <code>SQLException</code> while other drivers may support this
		''' operation.
		''' </summary>
		''' <param name="len"> the length, in characters, to which the <code>CLOB</code> value
		'''        should be truncated </param>
		''' <exception cref="SQLException"> if there is an error accessing the
		'''            <code>CLOB</code> value or if len is less than 0
		''' </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.4 </exception>
		Sub truncate(ByVal len As Long)

		''' <summary>
		''' This method frees the <code>Clob</code> object and releases the resources the resources
		''' that it holds.  The object is invalid once the <code>free</code> method
		''' is called.
		''' <p>
		''' After <code>free</code> has been called, any attempt to invoke a
		''' method other than <code>free</code> will result in a <code>SQLException</code>
		''' being thrown.  If <code>free</code> is called multiple times, the subsequent
		''' calls to <code>free</code> are treated as a no-op.
		''' <p> </summary>
		''' <exception cref="SQLException"> if an error occurs releasing
		''' the Clob's resources
		''' </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.6 </exception>
		Sub free()

		''' <summary>
		''' Returns a <code>Reader</code> object that contains a partial <code>Clob</code> value, starting
		''' with the character specified by pos, which is length characters in length.
		''' </summary>
		''' <param name="pos"> the offset to the first character of the partial value to
		''' be retrieved.  The first character in the Clob is at position 1. </param>
		''' <param name="length"> the length in characters of the partial value to be retrieved. </param>
		''' <returns> <code>Reader</code> through which the partial <code>Clob</code> value can be read. </returns>
		''' <exception cref="SQLException"> if pos is less than 1 or if pos is greater than the number of
		''' characters in the <code>Clob</code> or if pos + length is greater than the number of
		''' characters in the <code>Clob</code>
		''' </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.6 </exception>
		Function getCharacterStream(ByVal pos As Long, ByVal length As Long) As java.io.Reader

	End Interface

End Namespace
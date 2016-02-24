'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security


	''' <summary>
	''' A transparent stream that updates the associated message digest using
	''' the bits going through the stream.
	''' 
	''' <p>To complete the message digest computation, call one of the
	''' {@code digest} methods on the associated message
	''' digest after your calls to one of this digest output stream's
	''' <seealso cref="#write(int) write"/> methods.
	''' 
	''' <p>It is possible to turn this stream on or off (see
	''' <seealso cref="#on(boolean) on"/>). When it is on, a call to one of the
	''' {@code write} methods results in
	''' an update on the message digest.  But when it is off, the message
	''' digest is not updated. The default is for the stream to be on.
	''' </summary>
	''' <seealso cref= MessageDigest </seealso>
	''' <seealso cref= DigestInputStream
	''' 
	''' @author Benjamin Renaud </seealso>
	Public Class DigestOutputStream
		Inherits java.io.FilterOutputStream

		Private on_Renamed As Boolean = True

		''' <summary>
		''' The message digest associated with this stream.
		''' </summary>
		Protected Friend digest As MessageDigest

		''' <summary>
		''' Creates a digest output stream, using the specified output stream
		''' and message digest.
		''' </summary>
		''' <param name="stream"> the output stream.
		''' </param>
		''' <param name="digest"> the message digest to associate with this stream. </param>
		Public Sub New(ByVal stream As java.io.OutputStream, ByVal digest As MessageDigest)
			MyBase.New(stream)
			messageDigest = digest
		End Sub

		''' <summary>
		''' Returns the message digest associated with this stream.
		''' </summary>
		''' <returns> the message digest associated with this stream. </returns>
		''' <seealso cref= #setMessageDigest(java.security.MessageDigest) </seealso>
		Public Overridable Property messageDigest As MessageDigest
			Get
				Return digest
			End Get
			Set(ByVal digest As MessageDigest)
				Me.digest = digest
			End Set
		End Property


		''' <summary>
		''' Updates the message digest (if the digest function is on) using
		''' the specified byte, and in any case writes the byte
		''' to the output stream. That is, if the digest function is on
		''' (see <seealso cref="#on(boolean) on"/>), this method calls
		''' {@code update} on the message digest associated with this
		''' stream, passing it the byte {@code b}. This method then
		''' writes the byte to the output stream, blocking until the byte
		''' is actually written.
		''' </summary>
		''' <param name="b"> the byte to be used for updating and writing to the
		''' output stream.
		''' </param>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= MessageDigest#update(byte) </seealso>
		Public Overrides Sub write(ByVal b As Integer)
			out.write(b)
			If on_Renamed Then digest.update(CByte(b))
		End Sub

		''' <summary>
		''' Updates the message digest (if the digest function is on) using
		''' the specified subarray, and in any case writes the subarray to
		''' the output stream. That is, if the digest function is on (see
		''' <seealso cref="#on(boolean) on"/>), this method calls {@code update}
		''' on the message digest associated with this stream, passing it
		''' the subarray specifications. This method then writes the subarray
		''' bytes to the output stream, blocking until the bytes are actually
		''' written.
		''' </summary>
		''' <param name="b"> the array containing the subarray to be used for updating
		''' and writing to the output stream.
		''' </param>
		''' <param name="off"> the offset into {@code b} of the first byte to
		''' be updated and written.
		''' </param>
		''' <param name="len"> the number of bytes of data to be updated and written
		''' from {@code b}, starting at offset {@code off}.
		''' </param>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= MessageDigest#update(byte[], int, int) </seealso>
		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			out.write(b, [off], len)
			If on_Renamed Then digest.update(b, [off], len)
		End Sub

		''' <summary>
		''' Turns the digest function on or off. The default is on.  When
		''' it is on, a call to one of the {@code write} methods results in an
		''' update on the message digest.  But when it is off, the message
		''' digest is not updated.
		''' </summary>
		''' <param name="on"> true to turn the digest function on, false to turn it
		''' off. </param>
		Public Overridable Sub [on](ByVal [on] As Boolean)
			Me.on_Renamed = [on]
		End Sub

		''' <summary>
		''' Prints a string representation of this digest output stream and
		''' its associated message digest object.
		''' </summary>
		 Public Overrides Function ToString() As String
			 Return "[Digest Output Stream] " & digest.ToString()
		 End Function
	End Class

End Namespace
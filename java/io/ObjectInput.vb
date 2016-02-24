'
' * Copyright (c) 1996, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' ObjectInput extends the DataInput interface to include the reading of
	''' objects. DataInput includes methods for the input of primitive types,
	''' ObjectInput extends that interface to include objects, arrays, and Strings.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref= java.io.InputStream </seealso>
	''' <seealso cref= java.io.ObjectOutputStream </seealso>
	''' <seealso cref= java.io.ObjectInputStream
	''' @since   JDK1.1 </seealso>
	Public Interface ObjectInput
		Inherits DataInput, AutoCloseable

		''' <summary>
		''' Read and return an object. The class that implements this interface
		''' defines where the object is "read" from.
		''' </summary>
		''' <returns> the object read from the stream </returns>
		''' <exception cref="java.lang.ClassNotFoundException"> If the class of a serialized
		'''      object cannot be found. </exception>
		''' <exception cref="IOException"> If any of the usual Input/Output
		''' related exceptions occur. </exception>
		Function readObject() As Object

		''' <summary>
		''' Reads a byte of data. This method will block if no input is
		''' available. </summary>
		''' <returns>  the byte read, or -1 if the end of the
		'''          stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Function read() As Integer

		''' <summary>
		''' Reads into an array of bytes.  This method will
		''' block until some input is available. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <returns>  the actual number of bytes read, -1 is
		'''          returned when the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Function read(ByVal b As SByte()) As Integer

		''' <summary>
		''' Reads into an array of bytes.  This method will
		''' block until some input is available. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <returns>  the actual number of bytes read, -1 is
		'''          returned when the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Function read(SByte ByVal  As b(), ByVal [off] As Integer, ByVal len As Integer) As Integer

		''' <summary>
		''' Skips n bytes of input. </summary>
		''' <param name="n"> the number of bytes to be skipped </param>
		''' <returns>  the actual number of bytes skipped. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Function skip(ByVal n As Long) As Long

		''' <summary>
		''' Returns the number of bytes that can be read
		''' without blocking. </summary>
		''' <returns> the number of available bytes. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Function available() As Integer

		''' <summary>
		''' Closes the input stream. Must be called
		''' to release any resources associated with
		''' the stream. </summary>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Sub close()
	End Interface

End Namespace
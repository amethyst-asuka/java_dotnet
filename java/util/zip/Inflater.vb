Imports System.Diagnostics
Imports System.Runtime.InteropServices

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

Namespace java.util.zip

	''' <summary>
	''' This class provides support for general purpose decompression using the
	''' popular ZLIB compression library. The ZLIB compression library was
	''' initially developed as part of the PNG graphics standard and is not
	''' protected by patents. It is fully described in the specifications at
	''' the <a href="package-summary.html#package_description">java.util.zip
	''' package description</a>.
	''' 
	''' <p>The following code fragment demonstrates a trivial compression
	''' and decompression of a string using <tt>Deflater</tt> and
	''' <tt>Inflater</tt>.
	''' 
	''' <blockquote><pre>
	''' try {
	'''     // Encode a String into bytes
	'''     String inputString = "blahblahblah\u20AC\u20AC";
	'''     byte[] input = inputString.getBytes("UTF-8");
	''' 
	'''     // Compress the bytes
	'''     byte[] output = new byte[100];
	'''     Deflater compresser = new Deflater();
	'''     compresser.setInput(input);
	'''     compresser.finish();
	'''     int compressedDataLength = compresser.deflate(output);
	''' 
	'''     // Decompress the bytes
	'''     Inflater decompresser = new Inflater();
	'''     decompresser.setInput(output, 0, compressedDataLength);
	'''     byte[] result = new byte[100];
	'''     int resultLength = decompresser.inflate(result);
	'''     decompresser.end();
	''' 
	'''     // Decode the bytes into a String
	'''     String outputString = new String(result, 0, resultLength, "UTF-8");
	''' } catch(java.io.UnsupportedEncodingException ex) {
	'''     // handle
	''' } catch (java.util.zip.DataFormatException ex) {
	'''     // handle
	''' }
	''' </pre></blockquote>
	''' </summary>
	''' <seealso cref=         Deflater
	''' @author      David Connelly
	'''  </seealso>
	Public Class Inflater

		Private ReadOnly zsRef As ZStreamRef
		Private buf As SByte() = defaultBuf
		Private [off], len As Integer
		Private finished_Renamed As Boolean
		Private needDict As Boolean
		Private bytesRead As Long
		Private bytesWritten As Long

		Private Shared ReadOnly defaultBuf As SByte() = New SByte(){}

		Shared Sub New()
			' Zip library is loaded from System.initializeSystemClass 
			initIDs()
		End Sub

		''' <summary>
		''' Creates a new decompressor. If the parameter 'nowrap' is true then
		''' the ZLIB header and checksum fields will not be used. This provides
		''' compatibility with the compression format used by both GZIP and PKZIP.
		''' <p>
		''' Note: When using the 'nowrap' option it is also necessary to provide
		''' an extra "dummy" byte as input. This is required by the ZLIB native
		''' library in order to support certain optimizations.
		''' </summary>
		''' <param name="nowrap"> if true then support GZIP compatible compression </param>
		Public Sub New(  nowrap As Boolean)
			zsRef = New ZStreamRef(init(nowrap))
		End Sub

		''' <summary>
		''' Creates a new decompressor.
		''' </summary>
		Public Sub New()
			Me.New(False)
		End Sub

		''' <summary>
		''' Sets input data for decompression. Should be called whenever
		''' needsInput() returns true indicating that more input data is
		''' required. </summary>
		''' <param name="b"> the input data bytes </param>
		''' <param name="off"> the start offset of the input data </param>
		''' <param name="len"> the length of the input data </param>
		''' <seealso cref= Inflater#needsInput </seealso>
		Public Overridable Sub setInput(  b As SByte(),   [off] As Integer,   len As Integer)
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			SyncLock zsRef
				Me.buf = b
				Me.off = [off]
				Me.len = len
			End SyncLock
		End Sub

		''' <summary>
		''' Sets input data for decompression. Should be called whenever
		''' needsInput() returns true indicating that more input data is
		''' required. </summary>
		''' <param name="b"> the input data bytes </param>
		''' <seealso cref= Inflater#needsInput </seealso>
		Public Overridable Property input As SByte()
			Set(  b As SByte())
				inputput(b, 0, b.Length)
			End Set
		End Property

		''' <summary>
		''' Sets the preset dictionary to the given array of bytes. Should be
		''' called when inflate() returns 0 and needsDictionary() returns true
		''' indicating that a preset dictionary is required. The method getAdler()
		''' can be used to get the Adler-32 value of the dictionary needed. </summary>
		''' <param name="b"> the dictionary data bytes </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the length of the data </param>
		''' <seealso cref= Inflater#needsDictionary </seealso>
		''' <seealso cref= Inflater#getAdler </seealso>
		Public Overridable Sub setDictionary(  b As SByte(),   [off] As Integer,   len As Integer)
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			SyncLock zsRef
				ensureOpen()
				dictionaryary(zsRef.address(), b, [off], len)
				needDict = False
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the preset dictionary to the given array of bytes. Should be
		''' called when inflate() returns 0 and needsDictionary() returns true
		''' indicating that a preset dictionary is required. The method getAdler()
		''' can be used to get the Adler-32 value of the dictionary needed. </summary>
		''' <param name="b"> the dictionary data bytes </param>
		''' <seealso cref= Inflater#needsDictionary </seealso>
		''' <seealso cref= Inflater#getAdler </seealso>
		Public Overridable Property dictionary As SByte()
			Set(  b As SByte())
				dictionaryary(b, 0, b.Length)
			End Set
		End Property

		''' <summary>
		''' Returns the total number of bytes remaining in the input buffer.
		''' This can be used to find out what bytes still remain in the input
		''' buffer after decompression has finished. </summary>
		''' <returns> the total number of bytes remaining in the input buffer </returns>
		Public Overridable Property remaining As Integer
			Get
				SyncLock zsRef
					Return len
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns true if no data remains in the input buffer. This can
		''' be used to determine if #setInput should be called in order
		''' to provide more input. </summary>
		''' <returns> true if no data remains in the input buffer </returns>
		Public Overridable Function needsInput() As Boolean
			SyncLock zsRef
				Return len <= 0
			End SyncLock
		End Function

		''' <summary>
		''' Returns true if a preset dictionary is needed for decompression. </summary>
		''' <returns> true if a preset dictionary is needed for decompression </returns>
		''' <seealso cref= Inflater#setDictionary </seealso>
		Public Overridable Function needsDictionary() As Boolean
			SyncLock zsRef
				Return needDict
			End SyncLock
		End Function

		''' <summary>
		''' Returns true if the end of the compressed data stream has been
		''' reached. </summary>
		''' <returns> true if the end of the compressed data stream has been
		''' reached </returns>
		Public Overridable Function finished() As Boolean
			SyncLock zsRef
				Return finished_Renamed
			End SyncLock
		End Function

		''' <summary>
		''' Uncompresses bytes into specified buffer. Returns actual number
		''' of bytes uncompressed. A return value of 0 indicates that
		''' needsInput() or needsDictionary() should be called in order to
		''' determine if more input data or a preset dictionary is required.
		''' In the latter case, getAdler() can be used to get the Adler-32
		''' value of the dictionary required. </summary>
		''' <param name="b"> the buffer for the uncompressed data </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of uncompressed bytes </param>
		''' <returns> the actual number of uncompressed bytes </returns>
		''' <exception cref="DataFormatException"> if the compressed data format is invalid </exception>
		''' <seealso cref= Inflater#needsInput </seealso>
		''' <seealso cref= Inflater#needsDictionary </seealso>
		Public Overridable Function inflate(  b As SByte(),   [off] As Integer,   len As Integer) As Integer
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			SyncLock zsRef
				ensureOpen()
				Dim thisLen As Integer = Me.len
				Dim n As Integer = inflateBytes(zsRef.address(), b, [off], len)
				bytesWritten += n
				bytesRead += (thisLen - Me.len)
				Return n
			End SyncLock
		End Function

		''' <summary>
		''' Uncompresses bytes into specified buffer. Returns actual number
		''' of bytes uncompressed. A return value of 0 indicates that
		''' needsInput() or needsDictionary() should be called in order to
		''' determine if more input data or a preset dictionary is required.
		''' In the latter case, getAdler() can be used to get the Adler-32
		''' value of the dictionary required. </summary>
		''' <param name="b"> the buffer for the uncompressed data </param>
		''' <returns> the actual number of uncompressed bytes </returns>
		''' <exception cref="DataFormatException"> if the compressed data format is invalid </exception>
		''' <seealso cref= Inflater#needsInput </seealso>
		''' <seealso cref= Inflater#needsDictionary </seealso>
		Public Overridable Function inflate(  b As SByte()) As Integer
			Return inflate(b, 0, b.Length)
		End Function

		''' <summary>
		''' Returns the ADLER-32 value of the uncompressed data. </summary>
		''' <returns> the ADLER-32 value of the uncompressed data </returns>
		Public Overridable Property adler As Integer
			Get
				SyncLock zsRef
					ensureOpen()
					Return getAdler(zsRef.address())
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the total number of compressed bytes input so far.
		''' 
		''' <p>Since the number of bytes may be greater than
		'''  java.lang.[Integer].MAX_VALUE, the <seealso cref="#getBytesRead()"/> method is now
		''' the preferred means of obtaining this information.</p>
		''' </summary>
		''' <returns> the total number of compressed bytes input so far </returns>
		Public Overridable Property totalIn As Integer
			Get
				Return CInt(bytesRead)
			End Get
		End Property

		''' <summary>
		''' Returns the total number of compressed bytes input so far.
		''' </summary>
		''' <returns> the total (non-negative) number of compressed bytes input so far
		''' @since 1.5 </returns>
		Public Overridable Property bytesRead As Long
			Get
				SyncLock zsRef
					ensureOpen()
					Return bytesRead
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the total number of uncompressed bytes output so far.
		''' 
		''' <p>Since the number of bytes may be greater than
		'''  java.lang.[Integer].MAX_VALUE, the <seealso cref="#getBytesWritten()"/> method is now
		''' the preferred means of obtaining this information.</p>
		''' </summary>
		''' <returns> the total number of uncompressed bytes output so far </returns>
		Public Overridable Property totalOut As Integer
			Get
				Return CInt(bytesWritten)
			End Get
		End Property

		''' <summary>
		''' Returns the total number of uncompressed bytes output so far.
		''' </summary>
		''' <returns> the total (non-negative) number of uncompressed bytes output so far
		''' @since 1.5 </returns>
		Public Overridable Property bytesWritten As Long
			Get
				SyncLock zsRef
					ensureOpen()
					Return bytesWritten
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Resets inflater so that a new set of input data can be processed.
		''' </summary>
		Public Overridable Sub reset()
			SyncLock zsRef
				ensureOpen()
				reset(zsRef.address())
				buf = defaultBuf
				finished_Renamed = False
				needDict = False
					len = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					= len
					bytesWritten = 0
					bytesRead = bytesWritten
			End SyncLock
		End Sub

		''' <summary>
		''' Closes the decompressor and discards any unprocessed input.
		''' This method should be called when the decompressor is no longer
		''' being used, but will also be called automatically by the finalize()
		''' method. Once this method is called, the behavior of the Inflater
		''' object is undefined.
		''' </summary>
		Public Overridable Sub [end]()
			SyncLock zsRef
				Dim addr As Long = zsRef.address()
				zsRef.clear()
				If addr <> 0 Then
					end(addr)
					buf = Nothing
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Closes the decompressor when garbage is collected.
		''' </summary>
		Protected Overrides Sub Finalize()
			[end]()
		End Sub

		Private Sub ensureOpen()
			Debug.Assert(Thread.holdsLock(zsRef))
			If zsRef.address() = 0 Then Throw New NullPointerException("Inflater has been closed")
		End Sub

		Friend Overridable Function ended() As Boolean
			SyncLock zsRef
				Return zsRef.address() = 0
			End SyncLock
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function init(  nowrap As Boolean) As Long
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub setDictionary(  addr As Long,   b As SByte(),   [off] As Integer,   len As Integer)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function inflateBytes(  addr As Long,   b As SByte(),   [off] As Integer,   len As Integer) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getAdler(  addr As Long) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub reset(  addr As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub [end](  addr As Long)
		End Sub
	End Class

End Namespace
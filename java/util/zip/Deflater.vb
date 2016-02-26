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
	''' This class provides support for general purpose compression using the
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
	'''     String inputString = "blahblahblah";
	'''     byte[] input = inputString.getBytes("UTF-8");
	''' 
	'''     // Compress the bytes
	'''     byte[] output = new byte[100];
	'''     Deflater compresser = new Deflater();
	'''     compresser.setInput(input);
	'''     compresser.finish();
	'''     int compressedDataLength = compresser.deflate(output);
	'''     compresser.end();
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
	''' <seealso cref=         Inflater
	''' @author      David Connelly </seealso>
	Public Class Deflater

		Private ReadOnly zsRef As ZStreamRef
		Private buf As SByte() = New SByte(){}
		Private [off], len As Integer
		Private level, strategy As Integer
		Private paramsams As Boolean
		Private finish_Renamed, finished_Renamed As Boolean
		Private bytesRead As Long
		Private bytesWritten As Long

		''' <summary>
		''' Compression method for the deflate algorithm (the only one currently
		''' supported).
		''' </summary>
		Public Const DEFLATED As Integer = 8

		''' <summary>
		''' Compression level for no compression.
		''' </summary>
		Public Const NO_COMPRESSION As Integer = 0

		''' <summary>
		''' Compression level for fastest compression.
		''' </summary>
		Public Const BEST_SPEED As Integer = 1

		''' <summary>
		''' Compression level for best compression.
		''' </summary>
		Public Const BEST_COMPRESSION As Integer = 9

		''' <summary>
		''' Default compression level.
		''' </summary>
		Public Const DEFAULT_COMPRESSION As Integer = -1

		''' <summary>
		''' Compression strategy best used for data consisting mostly of small
		''' values with a somewhat random distribution. Forces more Huffman coding
		''' and less string matching.
		''' </summary>
		Public Const FILTERED As Integer = 1

		''' <summary>
		''' Compression strategy for Huffman coding only.
		''' </summary>
		Public Const HUFFMAN_ONLY As Integer = 2

		''' <summary>
		''' Default compression strategy.
		''' </summary>
		Public Const DEFAULT_STRATEGY As Integer = 0

		''' <summary>
		''' Compression flush mode used to achieve best compression result.
		''' </summary>
		''' <seealso cref= Deflater#deflate(byte[], int, int, int)
		''' @since 1.7 </seealso>
		Public Const NO_FLUSH As Integer = 0

		''' <summary>
		''' Compression flush mode used to flush out all pending output; may
		''' degrade compression for some compression algorithms.
		''' </summary>
		''' <seealso cref= Deflater#deflate(byte[], int, int, int)
		''' @since 1.7 </seealso>
		Public Const SYNC_FLUSH As Integer = 2

		''' <summary>
		''' Compression flush mode used to flush out all pending output and
		''' reset the deflater. Using this mode too often can seriously degrade
		''' compression.
		''' </summary>
		''' <seealso cref= Deflater#deflate(byte[], int, int, int)
		''' @since 1.7 </seealso>
		Public Const FULL_FLUSH As Integer = 3

		Shared Sub New()
			' Zip library is loaded from System.initializeSystemClass 
			initIDs()
		End Sub

		''' <summary>
		''' Creates a new compressor using the specified compression level.
		''' If 'nowrap' is true then the ZLIB header and checksum fields will
		''' not be used in order to support the compression format used in
		''' both GZIP and PKZIP. </summary>
		''' <param name="level"> the compression level (0-9) </param>
		''' <param name="nowrap"> if true then use GZIP compatible compression </param>
		Public Sub New(ByVal level As Integer, ByVal nowrap As Boolean)
			Me.level = level
			Me.strategy = DEFAULT_STRATEGY
			Me.zsRef = New ZStreamRef(init(level, DEFAULT_STRATEGY, nowrap))
		End Sub

		''' <summary>
		''' Creates a new compressor using the specified compression level.
		''' Compressed data will be generated in ZLIB format. </summary>
		''' <param name="level"> the compression level (0-9) </param>
		Public Sub New(ByVal level As Integer)
			Me.New(level, False)
		End Sub

		''' <summary>
		''' Creates a new compressor with the default compression level.
		''' Compressed data will be generated in ZLIB format.
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_COMPRESSION, False)
		End Sub

		''' <summary>
		''' Sets input data for compression. This should be called whenever
		''' needsInput() returns true indicating that more input data is required. </summary>
		''' <param name="b"> the input data bytes </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the length of the data </param>
		''' <seealso cref= Deflater#needsInput </seealso>
		Public Overridable Sub setInput(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			SyncLock zsRef
				Me.buf = b
				Me.off = [off]
				Me.len = len
			End SyncLock
		End Sub

		''' <summary>
		''' Sets input data for compression. This should be called whenever
		''' needsInput() returns true indicating that more input data is required. </summary>
		''' <param name="b"> the input data bytes </param>
		''' <seealso cref= Deflater#needsInput </seealso>
		Public Overridable Property input As SByte()
			Set(ByVal b As SByte())
				inputput(b, 0, b.Length)
			End Set
		End Property

		''' <summary>
		''' Sets preset dictionary for compression. A preset dictionary is used
		''' when the history buffer can be predetermined. When the data is later
		''' uncompressed with Inflater.inflate(), Inflater.getAdler() can be called
		''' in order to get the Adler-32 value of the dictionary required for
		''' decompression. </summary>
		''' <param name="b"> the dictionary data bytes </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the length of the data </param>
		''' <seealso cref= Inflater#inflate </seealso>
		''' <seealso cref= Inflater#getAdler </seealso>
		Public Overridable Sub setDictionary(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			SyncLock zsRef
				ensureOpen()
				dictionaryary(zsRef.address(), b, [off], len)
			End SyncLock
		End Sub

		''' <summary>
		''' Sets preset dictionary for compression. A preset dictionary is used
		''' when the history buffer can be predetermined. When the data is later
		''' uncompressed with Inflater.inflate(), Inflater.getAdler() can be called
		''' in order to get the Adler-32 value of the dictionary required for
		''' decompression. </summary>
		''' <param name="b"> the dictionary data bytes </param>
		''' <seealso cref= Inflater#inflate </seealso>
		''' <seealso cref= Inflater#getAdler </seealso>
		Public Overridable Property dictionary As SByte()
			Set(ByVal b As SByte())
				dictionaryary(b, 0, b.Length)
			End Set
		End Property

		''' <summary>
		''' Sets the compression strategy to the specified value.
		''' 
		''' <p> If the compression strategy is changed, the next invocation
		''' of {@code deflate} will compress the input available so far with
		''' the old strategy (and may be flushed); the new strategy will take
		''' effect only after that invocation.
		''' </summary>
		''' <param name="strategy"> the new compression strategy </param>
		''' <exception cref="IllegalArgumentException"> if the compression strategy is
		'''                                     invalid </exception>
		Public Overridable Property strategy As Integer
			Set(ByVal strategy As Integer)
				Select Case strategy
				  Case DEFAULT_STRATEGY, FILTERED, HUFFMAN_ONLY
				  Case Else
					Throw New IllegalArgumentException
				End Select
				SyncLock zsRef
					If Me.strategy <> strategy Then
						Me.strategy = strategy
						paramsams = True
					End If
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Sets the compression level to the specified value.
		''' 
		''' <p> If the compression level is changed, the next invocation
		''' of {@code deflate} will compress the input available so far
		''' with the old level (and may be flushed); the new level will
		''' take effect only after that invocation.
		''' </summary>
		''' <param name="level"> the new compression level (0-9) </param>
		''' <exception cref="IllegalArgumentException"> if the compression level is invalid </exception>
		Public Overridable Property level As Integer
			Set(ByVal level As Integer)
				If (level < 0 OrElse level > 9) AndAlso level <> DEFAULT_COMPRESSION Then Throw New IllegalArgumentException("invalid compression level")
				SyncLock zsRef
					If Me.level <> level Then
						Me.level = level
						paramsams = True
					End If
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Returns true if the input data buffer is empty and setInput()
		''' should be called in order to provide more input. </summary>
		''' <returns> true if the input data buffer is empty and setInput()
		''' should be called in order to provide more input </returns>
		Public Overridable Function needsInput() As Boolean
			Return len <= 0
		End Function

		''' <summary>
		''' When called, indicates that compression should end with the current
		''' contents of the input buffer.
		''' </summary>
		Public Overridable Sub finish()
			SyncLock zsRef
				finish_Renamed = True
			End SyncLock
		End Sub

		''' <summary>
		''' Returns true if the end of the compressed data output stream has
		''' been reached. </summary>
		''' <returns> true if the end of the compressed data output stream has
		''' been reached </returns>
		Public Overridable Function finished() As Boolean
			SyncLock zsRef
				Return finished_Renamed
			End SyncLock
		End Function

		''' <summary>
		''' Compresses the input data and fills specified buffer with compressed
		''' data. Returns actual number of bytes of compressed data. A return value
		''' of 0 indicates that <seealso cref="#needsInput() needsInput"/> should be called
		''' in order to determine if more input data is required.
		''' 
		''' <p>This method uses <seealso cref="#NO_FLUSH"/> as its compression flush mode.
		''' An invocation of this method of the form {@code deflater.deflate(b, off, len)}
		''' yields the same result as the invocation of
		''' {@code deflater.deflate(b, off, len, Deflater.NO_FLUSH)}.
		''' </summary>
		''' <param name="b"> the buffer for the compressed data </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes of compressed data </param>
		''' <returns> the actual number of bytes of compressed data written to the
		'''         output buffer </returns>
		Public Overridable Function deflate(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			Return deflate(b, [off], len, NO_FLUSH)
		End Function

		''' <summary>
		''' Compresses the input data and fills specified buffer with compressed
		''' data. Returns actual number of bytes of compressed data. A return value
		''' of 0 indicates that <seealso cref="#needsInput() needsInput"/> should be called
		''' in order to determine if more input data is required.
		''' 
		''' <p>This method uses <seealso cref="#NO_FLUSH"/> as its compression flush mode.
		''' An invocation of this method of the form {@code deflater.deflate(b)}
		''' yields the same result as the invocation of
		''' {@code deflater.deflate(b, 0, b.length, Deflater.NO_FLUSH)}.
		''' </summary>
		''' <param name="b"> the buffer for the compressed data </param>
		''' <returns> the actual number of bytes of compressed data written to the
		'''         output buffer </returns>
		Public Overridable Function deflate(ByVal b As SByte()) As Integer
			Return deflate(b, 0, b.Length, NO_FLUSH)
		End Function

		''' <summary>
		''' Compresses the input data and fills the specified buffer with compressed
		''' data. Returns actual number of bytes of data compressed.
		''' 
		''' <p>Compression flush mode is one of the following three modes:
		''' 
		''' <ul>
		''' <li><seealso cref="#NO_FLUSH"/>: allows the deflater to decide how much data
		''' to accumulate, before producing output, in order to achieve the best
		''' compression (should be used in normal use scenario). A return value
		''' of 0 in this flush mode indicates that <seealso cref="#needsInput()"/> should
		''' be called in order to determine if more input data is required.
		''' 
		''' <li><seealso cref="#SYNC_FLUSH"/>: all pending output in the deflater is flushed,
		''' to the specified output buffer, so that an inflater that works on
		''' compressed data can get all input data available so far (In particular
		''' the <seealso cref="#needsInput()"/> returns {@code true} after this invocation
		''' if enough output space is provided). Flushing with <seealso cref="#SYNC_FLUSH"/>
		''' may degrade compression for some compression algorithms and so it
		''' should be used only when necessary.
		''' 
		''' <li><seealso cref="#FULL_FLUSH"/>: all pending output is flushed out as with
		''' <seealso cref="#SYNC_FLUSH"/>. The compression state is reset so that the inflater
		''' that works on the compressed output data can restart from this point
		''' if previous compressed data has been damaged or if random access is
		''' desired. Using <seealso cref="#FULL_FLUSH"/> too often can seriously degrade
		''' compression.
		''' </ul>
		''' 
		''' <p>In the case of <seealso cref="#FULL_FLUSH"/> or <seealso cref="#SYNC_FLUSH"/>, if
		''' the return value is {@code len}, the space available in output
		''' buffer {@code b}, this method should be invoked again with the same
		''' {@code flush} parameter and more output space.
		''' </summary>
		''' <param name="b"> the buffer for the compressed data </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes of compressed data </param>
		''' <param name="flush"> the compression flush mode </param>
		''' <returns> the actual number of bytes of compressed data written to
		'''         the output buffer
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if the flush mode is invalid
		''' @since 1.7 </exception>
		Public Overridable Function deflate(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal flush As Integer) As Integer
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			SyncLock zsRef
				ensureOpen()
				If flush = NO_FLUSH OrElse flush = SYNC_FLUSH OrElse flush = FULL_FLUSH Then
					Dim thisLen As Integer = Me.len
					Dim n As Integer = deflateBytes(zsRef.address(), b, [off], len, flush)
					bytesWritten += n
					bytesRead += (thisLen - Me.len)
					Return n
				End If
				Throw New IllegalArgumentException
			End SyncLock
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
		''' Returns the total number of uncompressed bytes input so far.
		''' 
		''' <p>Since the number of bytes may be greater than
		'''  [Integer].MAX_VALUE, the <seealso cref="#getBytesRead()"/> method is now
		''' the preferred means of obtaining this information.</p>
		''' </summary>
		''' <returns> the total number of uncompressed bytes input so far </returns>
		Public Overridable Property totalIn As Integer
			Get
				Return CInt(bytesRead)
			End Get
		End Property

		''' <summary>
		''' Returns the total number of uncompressed bytes input so far.
		''' </summary>
		''' <returns> the total (non-negative) number of uncompressed bytes input so far
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
		''' Returns the total number of compressed bytes output so far.
		''' 
		''' <p>Since the number of bytes may be greater than
		'''  [Integer].MAX_VALUE, the <seealso cref="#getBytesWritten()"/> method is now
		''' the preferred means of obtaining this information.</p>
		''' </summary>
		''' <returns> the total number of compressed bytes output so far </returns>
		Public Overridable Property totalOut As Integer
			Get
				Return CInt(bytesWritten)
			End Get
		End Property

		''' <summary>
		''' Returns the total number of compressed bytes output so far.
		''' </summary>
		''' <returns> the total (non-negative) number of compressed bytes output so far
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
		''' Resets deflater so that a new set of input data can be processed.
		''' Keeps current compression level and strategy settings.
		''' </summary>
		Public Overridable Sub reset()
			SyncLock zsRef
				ensureOpen()
				reset(zsRef.address())
				finish_Renamed = False
				finished_Renamed = False
					len = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					= len
					bytesWritten = 0
					bytesRead = bytesWritten
			End SyncLock
		End Sub

		''' <summary>
		''' Closes the compressor and discards any unprocessed input.
		''' This method should be called when the compressor is no longer
		''' being used, but will also be called automatically by the
		''' finalize() method. Once this method is called, the behavior
		''' of the Deflater object is undefined.
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
		''' Closes the compressor when garbage is collected.
		''' </summary>
		Protected Overrides Sub Finalize()
			[end]()
		End Sub

		Private Sub ensureOpen()
			Debug.Assert(Thread.holdsLock(zsRef))
			If zsRef.address() = 0 Then Throw New NullPointerException("Deflater has been closed")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function init(ByVal level As Integer, ByVal strategy As Integer, ByVal nowrap As Boolean) As Long
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub setDictionary(ByVal addr As Long, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function deflateBytes(ByVal addr As Long, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal flush As Integer) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getAdler(ByVal addr As Long) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub reset(ByVal addr As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub [end](ByVal addr As Long)
		End Sub
	End Class

End Namespace
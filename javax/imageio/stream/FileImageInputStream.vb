'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.stream


	''' <summary>
	''' An implementation of <code>ImageInputStream</code> that gets its
	''' input from a <code>File</code> or <code>RandomAccessFile</code>.
	''' The file contents are assumed to be stable during the lifetime of
	''' the object.
	''' 
	''' </summary>
	Public Class FileImageInputStream
		Inherits ImageInputStreamImpl

		Private raf As java.io.RandomAccessFile

		''' <summary>
		''' The referent to be registered with the Disposer. </summary>
		Private ReadOnly disposerReferent As Object

		''' <summary>
		''' The DisposerRecord that closes the underlying RandomAccessFile. </summary>
		Private ReadOnly disposerRecord As com.sun.imageio.stream.CloseableDisposerRecord

		''' <summary>
		''' Constructs a <code>FileImageInputStream</code> that will read
		''' from a given <code>File</code>.
		''' 
		''' <p> The file contents must not change between the time this
		''' object is constructed and the time of the last call to a read
		''' method.
		''' </summary>
		''' <param name="f"> a <code>File</code> to read from.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>f</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="SecurityException"> if a security manager exists
		''' and does not allow read access to the file. </exception>
		''' <exception cref="FileNotFoundException"> if <code>f</code> is a
		''' directory or cannot be opened for reading for any other reason. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Public Sub New(ByVal f As java.io.File)
			Me.New(If(f Is Nothing, Nothing, New java.io.RandomAccessFile(f, "r")))
		End Sub

		''' <summary>
		''' Constructs a <code>FileImageInputStream</code> that will read
		''' from a given <code>RandomAccessFile</code>.
		''' 
		''' <p> The file contents must not change between the time this
		''' object is constructed and the time of the last call to a read
		''' method.
		''' </summary>
		''' <param name="raf"> a <code>RandomAccessFile</code> to read from.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>raf</code> is
		''' <code>null</code>. </exception>
		Public Sub New(ByVal raf As java.io.RandomAccessFile)
			If raf Is Nothing Then Throw New System.ArgumentException("raf == null!")
			Me.raf = raf

			disposerRecord = New com.sun.imageio.stream.CloseableDisposerRecord(raf)
			If Me.GetType() = GetType(FileImageInputStream) Then
				disposerReferent = New Object
				sun.java2d.Disposer.addRecord(disposerReferent, disposerRecord)
			Else
				disposerReferent = New com.sun.imageio.stream.StreamFinalizer(Me)
			End If
		End Sub

		Public Overrides Function read() As Integer
			checkClosed()
			bitOffset = 0
			Dim val As Integer = raf.read()
			If val <> -1 Then streamPos += 1
			Return val
		End Function

		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			checkClosed()
			bitOffset = 0
			Dim nbytes As Integer = raf.read(b, [off], len)
			If nbytes <> -1 Then streamPos += nbytes
			Return nbytes
		End Function

		''' <summary>
		''' Returns the length of the underlying file, or <code>-1</code>
		''' if it is unknown.
		''' </summary>
		''' <returns> the file length as a <code>long</code>, or
		''' <code>-1</code>. </returns>
		Public Overrides Function length() As Long
			Try
				checkClosed()
				Return raf.length()
			Catch e As java.io.IOException
				Return -1L
			End Try
		End Function

		Public Overrides Sub seek(ByVal pos As Long)
			checkClosed()
			If pos < flushedPos Then Throw New System.IndexOutOfRangeException("pos < flushedPos!")
			bitOffset = 0
			raf.seek(pos)
			streamPos = raf.filePointer
		End Sub

		Public Overrides Sub close()
			MyBase.close()
			disposerRecord.Dispose() ' this closes the RandomAccessFile
			raf = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Overrides Sub Finalize()
			' Empty finalizer: for performance reasons we instead use the
			' Disposer mechanism for ensuring that the underlying
			' RandomAccessFile is closed prior to garbage collection
		End Sub
	End Class

End Namespace
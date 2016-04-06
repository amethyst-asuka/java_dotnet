Imports System

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.jar


	''' <summary>
	''' The <code>JarOutputStream</code> class is used to write the contents
	''' of a JAR file to any output stream. It extends the class
	''' <code>java.util.zip.ZipOutputStream</code> with support
	''' for writing an optional <code>Manifest</code> entry. The
	''' <code>Manifest</code> can be used to specify meta-information about
	''' the JAR file and its entries.
	''' 
	''' @author  David Connelly </summary>
	''' <seealso cref=     Manifest </seealso>
	''' <seealso cref=     java.util.zip.ZipOutputStream
	''' @since   1.2 </seealso>
	Public Class JarOutputStream
		Inherits ZipOutputStream

		Private Const JAR_MAGIC As Integer = &HCAFE

		''' <summary>
		''' Creates a new <code>JarOutputStream</code> with the specified
		''' <code>Manifest</code>. The manifest is written as the first
		''' entry to the output stream.
		''' </summary>
		''' <param name="out"> the actual output stream </param>
		''' <param name="man"> the optional <code>Manifest</code> </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(  out As OutputStream,   man As Manifest)
			MyBase.New(out)
			If man Is Nothing Then Throw New NullPointerException("man")
			Dim e As New ZipEntry(JarFile.MANIFEST_NAME)
			putNextEntry(e)
			man.write(New BufferedOutputStream(Me))
			closeEntry()
		End Sub

		''' <summary>
		''' Creates a new <code>JarOutputStream</code> with no manifest. </summary>
		''' <param name="out"> the actual output stream </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(  out As OutputStream)
			MyBase.New(out)
		End Sub

		''' <summary>
		''' Begins writing a new JAR file entry and positions the stream
		''' to the start of the entry data. This method will also close
		''' any previous entry. The default compression method will be
		''' used if no compression method was specified for the entry.
		''' The current time will be used if the entry has no set modification
		''' time.
		''' </summary>
		''' <param name="ze"> the ZIP/JAR entry to be written </param>
		''' <exception cref="ZipException"> if a ZIP error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub putNextEntry(  ze As ZipEntry)
			If firstEntry Then
				' Make sure that extra field data for first JAR
				' entry includes JAR magic number id.
				Dim edata As SByte() = ze.extra
				If edata Is Nothing OrElse (Not hasMagic(edata)) Then
					If edata Is Nothing Then
						edata = New SByte(3){}
					Else
						' Prepend magic to existing extra data
						Dim tmp As SByte() = New SByte(edata.Length + 4 - 1){}
						Array.Copy(edata, 0, tmp, 4, edata.Length)
						edata = tmp
					End If
					set16(edata, 0, JAR_MAGIC) ' extra field id
					set16(edata, 2, 0) ' extra field size
					ze.extra = edata
				End If
				firstEntry = False
			End If
			MyBase.putNextEntry(ze)
		End Sub

		Private firstEntry As Boolean = True

	'    
	'     * Returns true if specified byte array contains the
	'     * jar magic extra field id.
	'     
		Private Shared Function hasMagic(  edata As SByte()) As Boolean
			Try
				Dim i As Integer = 0
				Do While i < edata.Length
					If get16(edata, i) = JAR_MAGIC Then Return True
					i += get16(edata, i + 2) + 4
				Loop
			Catch e As ArrayIndexOutOfBoundsException
				' Invalid extra field data
			End Try
			Return False
		End Function

	'    
	'     * Fetches unsigned 16-bit value from byte array at specified offset.
	'     * The bytes are assumed to be in Intel (little-endian) byte order.
	'     
		Private Shared Function get16(  b As SByte(),   [off] As Integer) As Integer
			Return java.lang.[Byte].toUnsignedInt(b([off])) Or (Byte.toUnsignedInt(b([off]+1)) << 8)
		End Function

	'    
	'     * Sets 16-bit value at specified offset. The bytes are assumed to
	'     * be in Intel (little-endian) byte order.
	'     
		Private Shared Sub set16(  b As SByte(),   [off] As Integer,   value As Integer)
			b([off]+0) = CByte(value)
			b([off]+1) = CByte(value >> 8)
		End Sub
	End Class

End Namespace
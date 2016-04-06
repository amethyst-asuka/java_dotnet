Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The Manifest class is used to maintain Manifest entry names and their
	''' associated Attributes. There are main Manifest Attributes as well as
	''' per-entry Attributes. For information on the Manifest format, please
	''' see the
	''' <a href="../../../../technotes/guides/jar/jar.html">
	''' Manifest format specification</a>.
	''' 
	''' @author  David Connelly </summary>
	''' <seealso cref=     Attributes
	''' @since   1.2 </seealso>
	Public Class Manifest
		Implements Cloneable

		' manifest main attributes
		Private attr As New Attributes

		' manifest entries
		Private entries As IDictionary(Of String, Attributes) = New Dictionary(Of String, Attributes)

		''' <summary>
		''' Constructs a new, empty Manifest.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new Manifest from the specified input stream.
		''' </summary>
		''' <param name="is"> the input stream containing manifest data </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(  [is] As java.io.InputStream)
			read([is])
		End Sub

		''' <summary>
		''' Constructs a new Manifest that is a copy of the specified Manifest.
		''' </summary>
		''' <param name="man"> the Manifest to copy </param>
		Public Sub New(  man As Manifest)
			attr.putAll(man.mainAttributes)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			entries.putAll(man.entries)
		End Sub

		''' <summary>
		''' Returns the main Attributes for the Manifest. </summary>
		''' <returns> the main Attributes for the Manifest </returns>
		Public Overridable Property mainAttributes As Attributes
			Get
				Return attr
			End Get
		End Property

		''' <summary>
		''' Returns a Map of the entries contained in this Manifest. Each entry
		''' is represented by a String name (key) and associated Attributes (value).
		''' The Map permits the {@code null} key, but no entry with a null key is
		''' created by <seealso cref="#read"/>, nor is such an entry written by using {@link
		''' #write}.
		''' </summary>
		''' <returns> a Map of the entries contained in this Manifest </returns>
		Public Overridable Property entries As IDictionary(Of String, Attributes)
			Get
				Return entries
			End Get
		End Property

		''' <summary>
		''' Returns the Attributes for the specified entry name.
		''' This method is defined as:
		''' <pre>
		'''      return (Attributes)getEntries().get(name)
		''' </pre>
		''' Though {@code null} is a valid {@code name}, when
		''' {@code getAttributes(null)} is invoked on a {@code Manifest}
		''' obtained from a jar file, {@code null} will be returned.  While jar
		''' files themselves do not allow {@code null}-named attributes, it is
		''' possible to invoke <seealso cref="#getEntries"/> on a {@code Manifest}, and
		''' on that result, invoke {@code put} with a null key and an
		''' arbitrary value.  Subsequent invocations of
		''' {@code getAttributes(null)} will return the just-{@code put}
		''' value.
		''' <p>
		''' Note that this method does not return the manifest's main attributes;
		''' see <seealso cref="#getMainAttributes"/>.
		''' </summary>
		''' <param name="name"> entry name </param>
		''' <returns> the Attributes for the specified entry name </returns>
		Public Overridable Function getAttributes(  name As String) As Attributes
			Return entries(name)
		End Function

		''' <summary>
		''' Clears the main Attributes as well as the entries in this Manifest.
		''' </summary>
		Public Overridable Sub clear()
			attr.clear()
			entries.Clear()
		End Sub

		''' <summary>
		''' Writes the Manifest to the specified OutputStream.
		''' Attributes.Name.MANIFEST_VERSION must be set in
		''' MainAttributes prior to invoking this method.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <seealso cref= #getMainAttributes </seealso>
		Public Overridable Sub write(  out As java.io.OutputStream)
			Dim dos As New java.io.DataOutputStream(out)
			' Write out the main attributes for the manifest
			attr.writeMain(dos)
			' Now write out the pre-entry attributes
			Dim it As IEnumerator(Of KeyValuePair(Of String, Attributes)) = entries.GetEnumerator()
			Do While it.MoveNext()
				Dim e As KeyValuePair(Of String, Attributes) = it.Current
				Dim buffer As New StringBuffer("Name: ")
				Dim value As String = e.Key
				If value IsNot Nothing Then
					Dim vb As SByte() = value.getBytes("UTF8")
					value = New String(vb, 0, 0, vb.Length)
				End If
				buffer.append(value)
				buffer.append(vbCrLf)
				make72Safe(buffer)
				dos.writeBytes(buffer.ToString())
				e.Value.write(dos)
			Loop
			dos.flush()
		End Sub

		''' <summary>
		''' Adds line breaks to enforce a maximum 72 bytes per line.
		''' </summary>
		Friend Shared Sub make72Safe(  line As StringBuffer)
			Dim length As Integer = line.length()
			If length > 72 Then
				Dim index As Integer = 70
				Do While index < length - 2
					line.insert(index, vbCrLf & " ")
					index += 72
					length += 3
				Loop
			End If
			Return
		End Sub

		''' <summary>
		''' Reads the Manifest from the specified InputStream. The entry
		''' names and attributes read will be merged in with the current
		''' manifest entries.
		''' </summary>
		''' <param name="is"> the input stream </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Sub read(  [is] As java.io.InputStream)
			' Buffered input stream for reading manifest data
			Dim fis As New FastInputStream([is])
			' Line buffer
			Dim lbuf As SByte() = New SByte(511){}
			' Read the main attributes for the manifest
			attr.read(fis, lbuf)
			' Total number of entries, attributes read
			Dim ecount As Integer = 0, acount As Integer = 0
			' Average size of entry attributes
			Dim asize As Integer = 2
			' Now parse the manifest entries
			Dim len As Integer
			Dim name As String = Nothing
			Dim skipEmptyLines As Boolean = True
			Dim lastline As SByte() = Nothing

			len = fis.readLine(lbuf)
			Do While len <> -1
				len -= 1
				If lbuf(len) <> ControlChars.Lf Then Throw New java.io.IOException("manifest line too long")
				If len > 0 AndAlso lbuf(len-1) = ControlChars.Cr Then len -= 1
				If len = 0 AndAlso skipEmptyLines Then
					len = fis.readLine(lbuf)
					Continue Do
				End If
				skipEmptyLines = False

				If name Is Nothing Then
					name = parseName(lbuf, len)
					If name Is Nothing Then Throw New java.io.IOException("invalid manifest format")
					If fis.peek() = AscW(" "c) Then
						' name is wrapped
						lastline = New SByte(len - 6 - 1){}
						Array.Copy(lbuf, 6, lastline, 0, len - 6)
						len = fis.readLine(lbuf)
						Continue Do
					End If
				Else
					' continuation line
					Dim buf As SByte() = New SByte(lastline.Length + len - 2){}
					Array.Copy(lastline, 0, buf, 0, lastline.Length)
					Array.Copy(lbuf, 1, buf, lastline.Length, len - 1)
					If fis.peek() = AscW(" "c) Then
						' name is wrapped
						lastline = buf
						len = fis.readLine(lbuf)
						Continue Do
					End If
					name = New String(buf, 0, buf.Length, "UTF8")
					lastline = Nothing
				End If
				Dim attr As Attributes = getAttributes(name)
				If attr Is Nothing Then
					attr = New Attributes(asize)
					entries(name) = attr
				End If
				attr.read(fis, lbuf)
				ecount += 1
				acount += attr.size()
				'XXX: Fix for when the average is 0. When it is 0,
				' you get an Attributes object with an initial
				' capacity of 0, which tickles a bug in HashMap.
				asize = System.Math.Max(2, acount \ ecount)

				name = Nothing
				skipEmptyLines = True
				len = fis.readLine(lbuf)
			Loop
		End Sub

		Private Function parseName(  lbuf As SByte(),   len As Integer) As String
			If toLower(lbuf(0)) = AscW("n"c) AndAlso toLower(lbuf(1)) = AscW("a"c) AndAlso toLower(lbuf(2)) = AscW("m"c) AndAlso toLower(lbuf(3)) = AscW("e"c) AndAlso lbuf(4) = AscW(":"c) AndAlso lbuf(5) = AscW(" "c) Then
				Try
					Return New String(lbuf, 6, len - 6, "UTF8")
				Catch e As Exception
				End Try
			End If
			Return Nothing
		End Function

		Private Function toLower(  c As Integer) As Integer
			Return If(c >= "A"c AndAlso c <= "Z"c, AscW("a"c) + (c - AscW("A"c)), c)
		End Function

		''' <summary>
		''' Returns true if the specified Object is also a Manifest and has
		''' the same main Attributes and entries.
		''' </summary>
		''' <param name="o"> the object to be compared </param>
		''' <returns> true if the specified Object is also a Manifest and has
		''' the same main Attributes and entries </returns>
		Public Overrides Function Equals(  o As Object) As Boolean
			If TypeOf o Is Manifest Then
				Dim m As Manifest = CType(o, Manifest)
				Return attr.Equals(m.mainAttributes) AndAlso entries.Equals(m.entries)
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns the hash code for this Manifest.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return attr.GetHashCode() + entries.GetHashCode()
		End Function

		''' <summary>
		''' Returns a shallow copy of this Manifest.  The shallow copy is
		''' implemented as follows:
		''' <pre>
		'''     public Object clone() { return new Manifest(this); }
		''' </pre> </summary>
		''' <returns> a shallow copy of this Manifest </returns>
		Public Overridable Function clone() As Object
			Return New Manifest(Me)
		End Function

	'    
	'     * A fast buffered input stream for parsing manifest files.
	'     
		Friend Class FastInputStream
			Inherits java.io.FilterInputStream

			Private buf As SByte()
			Private count As Integer = 0
			Private pos As Integer = 0

			Friend Sub New(  [in] As java.io.InputStream)
				Me.New([in], 8192)
			End Sub

			Friend Sub New(  [in] As java.io.InputStream,   size As Integer)
				MyBase.New([in])
				buf = New SByte(size - 1){}
			End Sub

			Public Overrides Function read() As Integer
				If pos >= count Then
					fill()
					If pos >= count Then Return -1
				End If
					Dim tempVar As Integer = pos
					pos += 1
					Return java.lang.[Byte].toUnsignedInt(buf(tempVar))
			End Function

			Public Overrides Function read(  b As SByte(),   [off] As Integer,   len As Integer) As Integer
				Dim avail As Integer = count - pos
				If avail <= 0 Then
					If len >= buf.Length Then Return [in].read(b, [off], len)
					fill()
					avail = count - pos
					If avail <= 0 Then Return -1
				End If
				If len > avail Then len = avail
				Array.Copy(buf, pos, b, [off], len)
				pos += len
				Return len
			End Function

	'        
	'         * Reads 'len' bytes from the input stream, or until an end-of-line
	'         * is reached. Returns the number of bytes read.
	'         
			Public Overridable Function readLine(  b As SByte(),   [off] As Integer,   len As Integer) As Integer
				Dim tbuf As SByte() = Me.buf
				Dim total As Integer = 0
				Do While total < len
					Dim avail As Integer = count - pos
					If avail <= 0 Then
						fill()
						avail = count - pos
						If avail <= 0 Then Return -1
					End If
					Dim n As Integer = len - total
					If n > avail Then n = avail
					Dim tpos As Integer = pos
					Dim maxpos As Integer = tpos + n
					Dim tempVar As Boolean = tpos < maxpos AndAlso tbuf(tpos) <> ControlChars.Lf
					tpos += 1
					Do While tempVar

						tempVar = tpos < maxpos AndAlso tbuf(tpos) <> ControlChars.Lf
						tpos += 1
					Loop
					n = tpos - pos
					Array.Copy(tbuf, pos, b, [off], n)
					[off] += n
					total += n
					pos = tpos
					If tbuf(tpos-1) = ControlChars.Lf Then Exit Do
				Loop
				Return total
			End Function

			Public Overridable Function peek() As SByte
				If pos = count Then fill()
				If pos = count Then Return -1 ' nothing left in buffer
				Return buf(pos)
			End Function

			Public Overridable Function readLine(  b As SByte()) As Integer
				Return readLine(b, 0, b.Length)
			End Function

			Public Overrides Function skip(  n As Long) As Long
				If n <= 0 Then Return 0
				Dim avail As Long = count - pos
				If avail <= 0 Then Return [in].skip(n)
				If n > avail Then n = avail
				pos += n
				Return n
			End Function

			Public Overrides Function available() As Integer
				Return (count - pos) + [in].available()
			End Function

			Public Overrides Sub close()
				If [in] IsNot Nothing Then
					[in].close()
					[in] = Nothing
					buf = Nothing
				End If
			End Sub

			Private Sub fill()
					pos = 0
					count = pos
				Dim n As Integer = [in].read(buf, 0, buf.Length)
				If n > 0 Then count = n
			End Sub
		End Class
	End Class

End Namespace
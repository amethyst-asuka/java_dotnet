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
	''' The Attributes class maps Manifest attribute names to associated string
	''' values. Valid attribute names are case-insensitive, are restricted to
	''' the ASCII characters in the set [0-9a-zA-Z_-], and cannot exceed 70
	''' characters in length. Attribute values can contain any characters and
	''' will be UTF8-encoded when written to the output stream.  See the
	''' <a href="../../../../technotes/guides/jar/jar.html">JAR File Specification</a>
	''' for more information about valid attribute names and values.
	''' 
	''' @author  David Connelly </summary>
	''' <seealso cref=     Manifest
	''' @since   1.2 </seealso>
	Public Class Attributes
		Implements IDictionary(Of Object, Object), Cloneable

		''' <summary>
		''' The attribute name-value mappings.
		''' </summary>
		Protected Friend map As IDictionary(Of Object, Object)

		''' <summary>
		''' Constructs a new, empty Attributes object with default size.
		''' </summary>
		Public Sub New()
			Me.New(11)
		End Sub

		''' <summary>
		''' Constructs a new, empty Attributes object with the specified
		''' initial size.
		''' </summary>
		''' <param name="size"> the initial number of attributes </param>
		Public Sub New(  size As Integer)
			map = New Dictionary(Of )(size)
		End Sub

		''' <summary>
		''' Constructs a new Attributes object with the same attribute name-value
		''' mappings as in the specified Attributes.
		''' </summary>
		''' <param name="attr"> the specified Attributes </param>
		Public Sub New(  attr As Attributes)
			map = New Dictionary(Of )(attr)
		End Sub


		''' <summary>
		''' Returns the value of the specified attribute name, or null if the
		''' attribute name was not found.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		''' <returns> the value of the specified attribute name, or null if
		'''         not found. </returns>
		Public Overridable Function [get](  name_Renamed As Object) As Object
			Return map(name_Renamed)
		End Function

		''' <summary>
		''' Returns the value of the specified attribute name, specified as
		''' a string, or null if the attribute was not found. The attribute
		''' name is case-insensitive.
		''' <p>
		''' This method is defined as:
		''' <pre>
		'''      return (String)get(new Attributes.Name((String)name));
		''' </pre>
		''' </summary>
		''' <param name="name"> the attribute name as a string </param>
		''' <returns> the String value of the specified attribute name, or null if
		'''         not found. </returns>
		''' <exception cref="IllegalArgumentException"> if the attribute name is invalid </exception>
		Public Overridable Function getValue(  name_Renamed As String) As String
			Return CStr([get](New Attributes.Name(name_Renamed)))
		End Function

		''' <summary>
		''' Returns the value of the specified Attributes.Name, or null if the
		''' attribute was not found.
		''' <p>
		''' This method is defined as:
		''' <pre>
		'''     return (String)get(name);
		''' </pre>
		''' </summary>
		''' <param name="name"> the Attributes.Name object </param>
		''' <returns> the String value of the specified Attribute.Name, or null if
		'''         not found. </returns>
		Public Overridable Function getValue(  name_Renamed As Name) As String
			Return CStr([get](name_Renamed))
		End Function

		''' <summary>
		''' Associates the specified value with the specified attribute name
		''' (key) in this Map. If the Map previously contained a mapping for
		''' the attribute name, the old value is replaced.
		''' </summary>
		''' <param name="name"> the attribute name </param>
		''' <param name="value"> the attribute value </param>
		''' <returns> the previous value of the attribute, or null if none </returns>
		''' <exception cref="ClassCastException"> if the name is not a Attributes.Name
		'''            or the value is not a String </exception>
		Public Overridable Function put(  name_Renamed As Object,   value As Object) As Object
				map(CType(name_Renamed, Attributes.Name)) = CStr(value)
				Return map(CType(name_Renamed, Attributes.Name))
		End Function

		''' <summary>
		''' Associates the specified value with the specified attribute name,
		''' specified as a String. The attributes name is case-insensitive.
		''' If the Map previously contained a mapping for the attribute name,
		''' the old value is replaced.
		''' <p>
		''' This method is defined as:
		''' <pre>
		'''      return (String)put(new Attributes.Name(name), value);
		''' </pre>
		''' </summary>
		''' <param name="name"> the attribute name as a string </param>
		''' <param name="value"> the attribute value </param>
		''' <returns> the previous value of the attribute, or null if none </returns>
		''' <exception cref="IllegalArgumentException"> if the attribute name is invalid </exception>
		Public Overridable Function putValue(  name_Renamed As String,   value As String) As String
			Return CStr(put(New Name(name_Renamed), value))
		End Function

		''' <summary>
		''' Removes the attribute with the specified name (key) from this Map.
		''' Returns the previous attribute value, or null if none.
		''' </summary>
		''' <param name="name"> attribute name </param>
		''' <returns> the previous value of the attribute, or null if none </returns>
		Public Overridable Function remove(  name_Renamed As Object) As Object
			Return map.Remove(name_Renamed)
		End Function

		''' <summary>
		''' Returns true if this Map maps one or more attribute names (keys)
		''' to the specified value.
		''' </summary>
		''' <param name="value"> the attribute value </param>
		''' <returns> true if this Map maps one or more attribute names to
		'''         the specified value </returns>
		Public Overridable Function containsValue(  value As Object) As Boolean
			Return map.ContainsValue(value)
		End Function

		''' <summary>
		''' Returns true if this Map contains the specified attribute name (key).
		''' </summary>
		''' <param name="name"> the attribute name </param>
		''' <returns> true if this Map contains the specified attribute name </returns>
		Public Overridable Function containsKey(  name_Renamed As Object) As Boolean
			Return map.ContainsKey(name_Renamed)
		End Function

		''' <summary>
		''' Copies all of the attribute name-value mappings from the specified
		''' Attributes to this Map. Duplicate mappings will be replaced.
		''' </summary>
		''' <param name="attr"> the Attributes to be stored in this map </param>
		''' <exception cref="ClassCastException"> if attr is not an Attributes </exception>
		Public Overridable Sub putAll(Of T1)(  attr As IDictionary(Of T1))
			' ## javac bug?
			If Not GetType(Attributes).isInstance(attr) Then Throw New ClassCastException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each [me] As KeyValuePair(Of ?, ?) In (attr).entrySet()
				put([me].Key, [me].Value)
			Next [me]
		End Sub

		''' <summary>
		''' Removes all attributes from this Map.
		''' </summary>
		Public Overridable Sub clear()
			map.Clear()
		End Sub

		''' <summary>
		''' Returns the number of attributes in this Map.
		''' </summary>
		Public Overridable Function size() As Integer
			Return map.Count
		End Function

		''' <summary>
		''' Returns true if this Map contains no attributes.
		''' </summary>
		Public Overridable Property empty As Boolean
			Get
				Return map.Count = 0
			End Get
		End Property

		''' <summary>
		''' Returns a Set view of the attribute names (keys) contained in this Map.
		''' </summary>
		Public Overridable Function keySet() As java.util.Set(Of Object)
			Return map.Keys
		End Function

		''' <summary>
		''' Returns a Collection view of the attribute values contained in this Map.
		''' </summary>
		Public Overridable Function values() As ICollection(Of Object)
			Return map.Values
		End Function

		''' <summary>
		''' Returns a Collection view of the attribute name-value mappings
		''' contained in this Map.
		''' </summary>
		Public Overridable Function entrySet() As java.util.Set(Of KeyValuePair(Of Object, Object))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Return map.entrySet()
		End Function

		''' <summary>
		''' Compares the specified Attributes object with this Map for equality.
		''' Returns true if the given object is also an instance of Attributes
		''' and the two Attributes objects represent the same mappings.
		''' </summary>
		''' <param name="o"> the Object to be compared </param>
		''' <returns> true if the specified Object is equal to this Map </returns>
		Public Overrides Function Equals(  o As Object) As Boolean
			Return map.Equals(o)
		End Function

		''' <summary>
		''' Returns the hash code value for this Map.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return map.GetHashCode()
		End Function

		''' <summary>
		''' Returns a copy of the Attributes, implemented as follows:
		''' <pre>
		'''     public Object clone() { return new Attributes(this); }
		''' </pre>
		''' Since the attribute names and values are themselves immutable,
		''' the Attributes returned can be safely modified without affecting
		''' the original.
		''' </summary>
		Public Overridable Function clone() As Object
			Return New Attributes(Me)
		End Function

	'    
	'     * Writes the current attributes to the specified data output stream.
	'     * XXX Need to handle UTF8 values and break up lines longer than 72 bytes
	'     
		 Friend Overridable Sub write(  os As java.io.DataOutputStream)
			Dim it As IEnumerator(Of KeyValuePair(Of Object, Object)) = entrySet().GetEnumerator()
			Do While it.MoveNext()
				Dim e As KeyValuePair(Of Object, Object) = it.Current
				Dim buffer As New StringBuffer(CType(e.Key, Name).ToString())
				buffer.append(": ")

				Dim value_Renamed As String = CStr(e.Value)
				If value_Renamed IsNot Nothing Then
					Dim vb As SByte() = value_Renamed.getBytes("UTF8")
					value_Renamed = New String(vb, 0, 0, vb.Length)
				End If
				buffer.append(value_Renamed)

				buffer.append(vbCrLf)
				Manifest.make72Safe(buffer)
				os.writeBytes(buffer.ToString())
			Loop
			os.writeBytes(vbCrLf)
		 End Sub

	'    
	'     * Writes the current attributes to the specified data output stream,
	'     * make sure to write out the MANIFEST_VERSION or SIGNATURE_VERSION
	'     * attributes first.
	'     *
	'     * XXX Need to handle UTF8 values and break up lines longer than 72 bytes
	'     
		Friend Overridable Sub writeMain(  out As java.io.DataOutputStream)
			' write out the *-Version header first, if it exists
			Dim vername As String = Name.MANIFEST_VERSION.ToString()
			Dim version As String = getValue(vername)
			If version Is Nothing Then
				vername = Name.SIGNATURE_VERSION.ToString()
				version = getValue(vername)
			End If

			If version IsNot Nothing Then out.writeBytes(vername & ": " & version & vbCrLf)

			' write out all attributes except for the version
			' we wrote out earlier
			Dim it As IEnumerator(Of KeyValuePair(Of Object, Object)) = entrySet().GetEnumerator()
			Do While it.MoveNext()
				Dim e As KeyValuePair(Of Object, Object) = it.Current
				Dim name_Renamed As String = CType(e.Key, Name).ToString()
				If (version IsNot Nothing) AndAlso Not(name_Renamed.equalsIgnoreCase(vername)) Then

					Dim buffer As New StringBuffer(name_Renamed)
					buffer.append(": ")

					Dim value_Renamed As String = CStr(e.Value)
					If value_Renamed IsNot Nothing Then
						Dim vb As SByte() = value_Renamed.getBytes("UTF8")
						value_Renamed = New String(vb, 0, 0, vb.Length)
					End If
					buffer.append(value_Renamed)

					buffer.append(vbCrLf)
					Manifest.make72Safe(buffer)
					out.writeBytes(buffer.ToString())
				End If
			Loop
			out.writeBytes(vbCrLf)
		End Sub

	'    
	'     * Reads attributes from the specified input stream.
	'     * XXX Need to handle UTF8 values.
	'     
		Friend Overridable Sub read(  [is] As Manifest.FastInputStream,   lbuf As SByte())
			Dim name_Renamed As String = Nothing, value_Renamed As String = Nothing
			Dim lastline As SByte() = Nothing

			Dim len As Integer
			len = [is].readLine(lbuf)
			Do While len <> -1
				Dim lineContinued As Boolean = False
				len -= 1
				If lbuf(len) <> ControlChars.Lf Then Throw New java.io.IOException("line too long")
				If len > 0 AndAlso lbuf(len-1) = ControlChars.Cr Then len -= 1
				If len = 0 Then Exit Do
				Dim i As Integer = 0
				If lbuf(0) = AscW(" "c) Then
					' continuation of previous line
					If name_Renamed Is Nothing Then Throw New java.io.IOException("misplaced continuation line")
					lineContinued = True
					Dim buf As SByte() = New SByte(lastline.Length + len - 2){}
					Array.Copy(lastline, 0, buf, 0, lastline.Length)
					Array.Copy(lbuf, 1, buf, lastline.Length, len - 1)
					If [is].peek() = AscW(" "c) Then
						lastline = buf
						len = [is].readLine(lbuf)
						Continue Do
					End If
					value_Renamed = New String(buf, 0, buf.Length, "UTF8")
					lastline = Nothing
				Else
					Dim tempVar As Boolean = lbuf(i) <> AscW(":"c)
					i += 1
					Do While tempVar
						If i >= len Then Throw New java.io.IOException("invalid header field")
						tempVar = lbuf(i) <> AscW(":"c)
						i += 1
					Loop
					Dim tempVar2 As Boolean = lbuf(i) <> AscW(" "c)
					i += 1
					If tempVar2 Then Throw New java.io.IOException("invalid header field")
					name_Renamed = New String(lbuf, 0, 0, i - 2)
					If [is].peek() = AscW(" "c) Then
						lastline = New SByte(len - i - 1){}
						Array.Copy(lbuf, i, lastline, 0, len - i)
						len = [is].readLine(lbuf)
						Continue Do
					End If
					value_Renamed = New String(lbuf, i, len - i, "UTF8")
				End If
				Try
					If (putValue(name_Renamed, value_Renamed) IsNot Nothing) AndAlso ((Not lineContinued)) Then sun.util.logging.PlatformLogger.getLogger("java.util.jar").warning("Duplicate name in Manifest: " & name_Renamed & "." & vbLf & "Ensure that the manifest does not " & "have duplicate entries, and" & vbLf & "that blank lines separate " & "individual sections in both your" & vbLf & "manifest and in the META-INF/MANIFEST.MF " & "entry in the jar file.")
				Catch e As IllegalArgumentException
					Throw New java.io.IOException("invalid header field name: " & name_Renamed)
				End Try
				len = [is].readLine(lbuf)
			Loop
		End Sub

		''' <summary>
		''' The Attributes.Name class represents an attribute name stored in
		''' this Map. Valid attribute names are case-insensitive, are restricted
		''' to the ASCII characters in the set [0-9a-zA-Z_-], and cannot exceed
		''' 70 characters in length. Attribute values can contain any characters
		''' and will be UTF8-encoded when written to the output stream.  See the
		''' <a href="../../../../technotes/guides/jar/jar.html">JAR File Specification</a>
		''' for more information about valid attribute names and values.
		''' </summary>
		Public Class Name
			Private name_Renamed As String
			Private hashCode_Renamed As Integer = -1

			''' <summary>
			''' Constructs a new attribute name using the given string name.
			''' </summary>
			''' <param name="name"> the attribute string name </param>
			''' <exception cref="IllegalArgumentException"> if the attribute name was
			'''            invalid </exception>
			''' <exception cref="NullPointerException"> if the attribute name was null </exception>
			Public Sub New(  name_Renamed As String)
				If name_Renamed Is Nothing Then Throw New NullPointerException("name")
				If Not isValid(name_Renamed) Then Throw New IllegalArgumentException(name_Renamed)
				Me.name_Renamed = name_Renamed.intern()
			End Sub

			Private Shared Function isValid(  name_Renamed As String) As Boolean
				Dim len As Integer = name_Renamed.length()
				If len > 70 OrElse len = 0 Then Return False
				For i As Integer = 0 To len - 1
					If Not isValid(name_Renamed.Chars(i)) Then Return False
				Next i
				Return True
			End Function

			Private Shared Function isValid(  c As Char) As Boolean
				Return isAlpha(c) OrElse isDigit(c) OrElse c = "_"c OrElse c = "-"c
			End Function

			Private Shared Function isAlpha(  c As Char) As Boolean
				Return (c >= "a"c AndAlso c <= "z"c) OrElse (c >= "A"c AndAlso c <= "Z"c)
			End Function

			Private Shared Function isDigit(  c As Char) As Boolean
				Return c >= "0"c AndAlso c <= "9"c
			End Function

			''' <summary>
			''' Compares this attribute name to another for equality. </summary>
			''' <param name="o"> the object to compare </param>
			''' <returns> true if this attribute name is equal to the
			'''         specified attribute object </returns>
			Public Overrides Function Equals(  o As Object) As Boolean
				If TypeOf o Is Name Then
					Dim c As IComparer(Of String) = sun.misc.ASCIICaseInsensitiveComparator.CASE_INSENSITIVE_ORDER
					Return c.Compare(name_Renamed, CType(o, Name).name_Renamed) = 0
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Computes the hash value for this attribute name.
			''' </summary>
			Public Overrides Function GetHashCode() As Integer
				If hashCode_Renamed = -1 Then hashCode_Renamed = sun.misc.ASCIICaseInsensitiveComparator.lowerCaseHashCode(name_Renamed)
				Return hashCode_Renamed
			End Function

			''' <summary>
			''' Returns the attribute name as a String.
			''' </summary>
			Public Overrides Function ToString() As String
				Return name_Renamed
			End Function

			''' <summary>
			''' <code>Name</code> object for <code>Manifest-Version</code>
			''' manifest attribute. This attribute indicates the version number
			''' of the manifest standard to which a JAR file's manifest conforms. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#JAR_Manifest">
			'''      Manifest and Signature Specification</a> </seealso>
			Public Shared ReadOnly MANIFEST_VERSION As New Name("Manifest-Version")

			''' <summary>
			''' <code>Name</code> object for <code>Signature-Version</code>
			''' manifest attribute used when signing JAR files. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#JAR_Manifest">
			'''      Manifest and Signature Specification</a> </seealso>
			Public Shared ReadOnly SIGNATURE_VERSION As New Name("Signature-Version")

			''' <summary>
			''' <code>Name</code> object for <code>Content-Type</code>
			''' manifest attribute.
			''' </summary>
			Public Shared ReadOnly CONTENT_TYPE As New Name("Content-Type")

			''' <summary>
			''' <code>Name</code> object for <code>Class-Path</code>
			''' manifest attribute. Bundled extensions can use this attribute
			''' to find other JAR files containing needed classes. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#classpath">
			'''      JAR file specification</a> </seealso>
			Public Shared ReadOnly CLASS_PATH As New Name("Class-Path")

			''' <summary>
			''' <code>Name</code> object for <code>Main-Class</code> manifest
			''' attribute used for launching applications packaged in JAR files.
			''' The <code>Main-Class</code> attribute is used in conjunction
			''' with the <code>-jar</code> command-line option of the
			''' <tt>java</tt> application launcher.
			''' </summary>
			Public Shared ReadOnly MAIN_CLASS As New Name("Main-Class")

			''' <summary>
			''' <code>Name</code> object for <code>Sealed</code> manifest attribute
			''' used for sealing. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/jar/jar.html#sealing">
			'''      Package Sealing</a> </seealso>
			Public Shared ReadOnly SEALED As New Name("Sealed")

		   ''' <summary>
		   ''' <code>Name</code> object for <code>Extension-List</code> manifest attribute
		   ''' used for declaring dependencies on installed extensions. </summary>
		   ''' <seealso cref= <a href="../../../../technotes/guides/extensions/spec.html#dependency">
		   '''      Installed extension dependency</a> </seealso>
			Public Shared ReadOnly EXTENSION_LIST As New Name("Extension-List")

			''' <summary>
			''' <code>Name</code> object for <code>Extension-Name</code> manifest attribute
			''' used for declaring dependencies on installed extensions. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/extensions/spec.html#dependency">
			'''      Installed extension dependency</a> </seealso>
			Public Shared ReadOnly EXTENSION_NAME As New Name("Extension-Name")

			''' <summary>
			''' <code>Name</code> object for <code>Extension-Name</code> manifest attribute
			''' used for declaring dependencies on installed extensions. </summary>
			''' @deprecated Extension mechanism will be removed in a future release.
			'''             Use class path instead. 
			''' <seealso cref= <a href="../../../../technotes/guides/extensions/spec.html#dependency">
			'''      Installed extension dependency</a> </seealso>
			<Obsolete("Extension mechanism will be removed in a future release.")> _
			Public Shared ReadOnly EXTENSION_INSTALLATION As New Name("Extension-Installation")

			''' <summary>
			''' <code>Name</code> object for <code>Implementation-Title</code>
			''' manifest attribute used for package versioning. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			'''      Java Product Versioning Specification</a> </seealso>
			Public Shared ReadOnly IMPLEMENTATION_TITLE As New Name("Implementation-Title")

			''' <summary>
			''' <code>Name</code> object for <code>Implementation-Version</code>
			''' manifest attribute used for package versioning. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			'''      Java Product Versioning Specification</a> </seealso>
			Public Shared ReadOnly IMPLEMENTATION_VERSION As New Name("Implementation-Version")

			''' <summary>
			''' <code>Name</code> object for <code>Implementation-Vendor</code>
			''' manifest attribute used for package versioning. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			'''      Java Product Versioning Specification</a> </seealso>
			Public Shared ReadOnly IMPLEMENTATION_VENDOR As New Name("Implementation-Vendor")

			''' <summary>
			''' <code>Name</code> object for <code>Implementation-Vendor-Id</code>
			''' manifest attribute used for package versioning. </summary>
			''' @deprecated Extension mechanism will be removed in a future release.
			'''             Use class path instead. 
			''' <seealso cref= <a href="../../../../technotes/guides/extensions/versioning.html#applet">
			'''      Optional Package Versioning</a> </seealso>
			<Obsolete("Extension mechanism will be removed in a future release.")> _
			Public Shared ReadOnly IMPLEMENTATION_VENDOR_ID As New Name("Implementation-Vendor-Id")

		   ''' <summary>
		   ''' <code>Name</code> object for <code>Implementation-URL</code>
		   ''' manifest attribute used for package versioning. </summary>
		   ''' @deprecated Extension mechanism will be removed in a future release.
		   '''             Use class path instead. 
		   ''' <seealso cref= <a href="../../../../technotes/guides/extensions/versioning.html#applet">
		   '''      Optional Package Versioning</a> </seealso>
			<Obsolete("Extension mechanism will be removed in a future release.")> _
			Public Shared ReadOnly IMPLEMENTATION_URL As New Name("Implementation-URL")

			''' <summary>
			''' <code>Name</code> object for <code>Specification-Title</code>
			''' manifest attribute used for package versioning. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			'''      Java Product Versioning Specification</a> </seealso>
			Public Shared ReadOnly SPECIFICATION_TITLE As New Name("Specification-Title")

			''' <summary>
			''' <code>Name</code> object for <code>Specification-Version</code>
			''' manifest attribute used for package versioning. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			'''      Java Product Versioning Specification</a> </seealso>
			Public Shared ReadOnly SPECIFICATION_VERSION As New Name("Specification-Version")

			''' <summary>
			''' <code>Name</code> object for <code>Specification-Vendor</code>
			''' manifest attribute used for package versioning. </summary>
			''' <seealso cref= <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
			'''      Java Product Versioning Specification</a> </seealso>
			Public Shared ReadOnly SPECIFICATION_VENDOR As New Name("Specification-Vendor")
		End Class
	End Class

End Namespace
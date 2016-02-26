Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A serialized version of an <code>Array</code>
	''' object, which is the mapping in the Java programming language of an SQL
	''' <code>ARRAY</code> value.
	''' <P>
	''' The <code>SerialArray</code> class provides a constructor for creating
	''' a <code>SerialArray</code> instance from an <code>Array</code> object,
	''' methods for getting the base type and the SQL name for the base type, and
	''' methods for copying all or part of a <code>SerialArray</code> object.
	''' <P>
	''' 
	''' Note: In order for this class to function correctly, a connection to the
	''' data source
	''' must be available in order for the SQL <code>Array</code> object to be
	''' materialized (have all of its elements brought to the client server)
	''' if necessary. At this time, logical pointers to the data in the data source,
	''' such as locators, are not currently supported.
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' A SerialArray is not safe for use by multiple concurrent threads.  If a
	''' SerialArray is to be used by more than one thread then access to the
	''' SerialArray should be controlled by appropriate synchronization.
	''' 
	''' </summary>
	<Serializable> _
	Public Class SerialArray
		Implements Array, ICloneable

		''' <summary>
		''' A serialized array in which each element is an <code>Object</code>
		''' in the Java programming language that represents an element
		''' in the SQL <code>ARRAY</code> value.
		''' @serial
		''' </summary>
		Private elements As Object()

		''' <summary>
		''' The SQL type of the elements in this <code>SerialArray</code> object.  The
		''' type is expressed as one of the constants from the class
		''' <code>java.sql.Types</code>.
		''' @serial
		''' </summary>
		Private baseType As Integer

		''' <summary>
		''' The type name used by the DBMS for the elements in the SQL <code>ARRAY</code>
		''' value that this <code>SerialArray</code> object represents.
		''' @serial
		''' </summary>
		Private baseTypeName As String

		''' <summary>
		''' The number of elements in this <code>SerialArray</code> object, which
		''' is also the number of elements in the SQL <code>ARRAY</code> value
		''' that this <code>SerialArray</code> object represents.
		''' @serial
		''' </summary>
		Private len As Integer

		''' <summary>
		''' Constructs a new <code>SerialArray</code> object from the given
		''' <code>Array</code> object, using the given type map for the custom
		''' mapping of each element when the elements are SQL UDTs.
		''' <P>
		''' This method does custom mapping if the array elements are a UDT
		''' and the given type map has an entry for that UDT.
		''' Custom mapping is recursive,
		''' meaning that if, for instance, an element of an SQL structured type
		''' is an SQL structured type that itself has an element that is an SQL
		''' structured type, each structured type that has a custom mapping will be
		''' mapped according to the given type map.
		''' <P>
		''' The new <code>SerialArray</code>
		''' object contains the same elements as the <code>Array</code> object
		''' from which it is built, except when the base type is the SQL type
		''' <code>STRUCT</code>, <code>ARRAY</code>, <code>BLOB</code>,
		''' <code>CLOB</code>, <code>DATALINK</code> or <code>JAVA_OBJECT</code>.
		''' In this case, each element in the new
		''' <code>SerialArray</code> object is the appropriate serialized form,
		''' that is, a <code>SerialStruct</code>, <code>SerialArray</code>,
		''' <code>SerialBlob</code>, <code>SerialClob</code>,
		''' <code>SerialDatalink</code>, or <code>SerialJavaObject</code> object.
		''' <P>
		''' Note: (1) The <code>Array</code> object from which a <code>SerialArray</code>
		''' object is created must have materialized the SQL <code>ARRAY</code> value's
		''' data on the client before it is passed to the constructor.  Otherwise,
		''' the new <code>SerialArray</code> object will contain no data.
		''' <p>
		''' Note: (2) If the <code>Array</code> contains <code>java.sql.Types.JAVA_OBJECT</code>
		''' types, the <code>SerialJavaObject</code> constructor is called where checks
		''' are made to ensure this object is serializable.
		''' <p>
		''' Note: (3) The <code>Array</code> object supplied to this constructor cannot
		''' return <code>null</code> for any <code>Array.getArray()</code> methods.
		''' <code>SerialArray</code> cannot serialize null array values.
		''' 
		''' </summary>
		''' <param name="array"> the <code>Array</code> object to be serialized </param>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT (an SQL structured type or
		'''        distinct type) and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped. The <i>map</i>
		'''        parameter does not have any effect for <code>Blob</code>,
		'''        <code>Clob</code>, <code>DATALINK</code>, or
		'''        <code>JAVA_OBJECT</code> types. </param>
		''' <exception cref="SerialException"> if an error occurs serializing the
		'''        <code>Array</code> object </exception>
		''' <exception cref="SQLException"> if a database access error occurs or if the
		'''        <i>array</i> or the <i>map</i> values are <code>null</code> </exception>
		 Public Sub New(ByVal array As Array, ByVal map As IDictionary(Of String, Type))

			If (array Is Nothing) OrElse (map Is Nothing) Then Throw New SQLException("Cannot instantiate a SerialArray " & "object with null parameters")

			elements = CType(array.array, Object())
			If elements Is Nothing Then Throw New SQLException("Invalid Array object. Calls to Array.getArray() " & "return null value which cannot be serialized")

			elements = CType(array.getArray(map), Object())
			baseType = array.baseType
			baseTypeName = array.baseTypeName
			len = elements.Length

			Select Case baseType
				Case java.sql.Types.STRUCT
					For i As Integer = 0 To len - 1
						elements(i) = New SerialStruct(CType(elements(i), Struct), map)
					Next i

				Case java.sql.Types.ARRAY
					For i As Integer = 0 To len - 1
						elements(i) = New SerialArray(CType(elements(i), Array), map)
					Next i

				Case java.sql.Types.BLOB
				For i As Integer = 0 To len - 1
					elements(i) = New SerialBlob(CType(elements(i), Blob))
				Next i

				Case java.sql.Types.CLOB
					For i As Integer = 0 To len - 1
						elements(i) = New SerialClob(CType(elements(i), Clob))
					Next i

				Case java.sql.Types.DATALINK
					For i As Integer = 0 To len - 1
						elements(i) = New SerialDatalink(CType(elements(i), java.net.URL))
					Next i

				Case java.sql.Types.JAVA_OBJECT
					For i As Integer = 0 To len - 1
					elements(i) = New SerialJavaObject(elements(i))
					Next i
			End Select
		 End Sub

		''' <summary>
		''' This method frees the {@code SeriableArray} object and releases the
		''' resources that it holds. The object is invalid once the {@code free}
		''' method is called. <p> If {@code free} is called multiple times, the
		''' subsequent calls to {@code free} are treated as a no-op. </P>
		''' </summary>
		''' <exception cref="SQLException"> if an error occurs releasing the SerialArray's resources
		''' @since 1.6 </exception>
		Public Overridable Sub free()
			If elements IsNot Nothing Then
				elements = Nothing
				baseTypeName= Nothing
			End If
		End Sub

		''' <summary>
		''' Constructs a new <code>SerialArray</code> object from the given
		''' <code>Array</code> object.
		''' <P>
		''' This constructor does not do custom mapping.  If the base type of the array
		''' is an SQL structured type and custom mapping is desired, the constructor
		''' <code>SerialArray(Array array, Map map)</code> should be used.
		''' <P>
		''' The new <code>SerialArray</code>
		''' object contains the same elements as the <code>Array</code> object
		''' from which it is built, except when the base type is the SQL type
		''' <code>BLOB</code>,
		''' <code>CLOB</code>, <code>DATALINK</code> or <code>JAVA_OBJECT</code>.
		''' In this case, each element in the new
		''' <code>SerialArray</code> object is the appropriate serialized form,
		''' that is, a <code>SerialBlob</code>, <code>SerialClob</code>,
		''' <code>SerialDatalink</code>, or <code>SerialJavaObject</code> object.
		''' <P>
		''' Note: (1) The <code>Array</code> object from which a <code>SerialArray</code>
		''' object is created must have materialized the SQL <code>ARRAY</code> value's
		''' data on the client before it is passed to the constructor.  Otherwise,
		''' the new <code>SerialArray</code> object will contain no data.
		''' <p>
		''' Note: (2) The <code>Array</code> object supplied to this constructor cannot
		''' return <code>null</code> for any <code>Array.getArray()</code> methods.
		''' <code>SerialArray</code> cannot serialize <code>null</code> array values.
		''' </summary>
		''' <param name="array"> the <code>Array</code> object to be serialized </param>
		''' <exception cref="SerialException"> if an error occurs serializing the
		'''     <code>Array</code> object </exception>
		''' <exception cref="SQLException"> if a database access error occurs or the
		'''     <i>array</i> parameter is <code>null</code>. </exception>
		 Public Sub New(ByVal array As Array)
			 If array Is Nothing Then Throw New SQLException("Cannot instantiate a SerialArray " & "object with a null Array object")

			 elements = CType(array.array, Object())
			 If elements Is Nothing Then Throw New SQLException("Invalid Array object. Calls to Array.getArray() " & "return null value which cannot be serialized")

			 'elements = (Object[])array.getArray();
			 baseType = array.baseType
			 baseTypeName = array.baseTypeName
			 len = elements.Length

			Select Case baseType

			Case java.sql.Types.BLOB
				For i As Integer = 0 To len - 1
					elements(i) = New SerialBlob(CType(elements(i), Blob))
				Next i

			Case java.sql.Types.CLOB
				For i As Integer = 0 To len - 1
					elements(i) = New SerialClob(CType(elements(i), Clob))
				Next i

			Case java.sql.Types.DATALINK
				For i As Integer = 0 To len - 1
					elements(i) = New SerialDatalink(CType(elements(i), java.net.URL))
				Next i

			Case java.sql.Types.JAVA_OBJECT
				For i As Integer = 0 To len - 1
					elements(i) = New SerialJavaObject(elements(i))
				Next i

			End Select


		 End Sub

		''' <summary>
		''' Returns a new array that is a copy of this <code>SerialArray</code>
		''' object.
		''' </summary>
		''' <returns> a copy of this <code>SerialArray</code> object as an
		'''         <code>Object</code> in the Java programming language </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Property array As Object
			Get
				valid
				Dim dst As Object = New Object(len - 1){}
				Array.Copy(CObj(elements), 0, dst, 0, len)
				Return dst
			End Get
		End Property

	 '[if an error occurstype map used??]
		''' <summary>
		''' Returns a new array that is a copy of this <code>SerialArray</code>
		''' object, using the given type map for the custom
		''' mapping of each element when the elements are SQL UDTs.
		''' <P>
		''' This method does custom mapping if the array elements are a UDT
		''' and the given type map has an entry for that UDT.
		''' Custom mapping is recursive,
		''' meaning that if, for instance, an element of an SQL structured type
		''' is an SQL structured type that itself has an element that is an SQL
		''' structured type, each structured type that has a custom mapping will be
		''' mapped according to the given type map.
		''' </summary>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <returns> a copy of this <code>SerialArray</code> object as an
		'''         <code>Object</code> in the Java programming language </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function getArray(ByVal map As IDictionary(Of String, Type)) As Object
			valid
			Dim dst As Object() = New Object(len - 1){}
			Array.Copy(CObj(elements), 0, dst, 0, len)
			Return dst
		End Function

		''' <summary>
		''' Returns a new array that is a copy of a slice
		''' of this <code>SerialArray</code> object, starting with the
		''' element at the given index and containing the given number
		''' of consecutive elements.
		''' </summary>
		''' <param name="index"> the index into this <code>SerialArray</code> object
		'''              of the first element to be copied;
		'''              the index of the first element is <code>0</code> </param>
		''' <param name="count"> the number of consecutive elements to be copied, starting
		'''              at the given index </param>
		''' <returns> a copy of the designated elements in this <code>SerialArray</code>
		'''         object as an <code>Object</code> in the Java programming language </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function getArray(ByVal index As Long, ByVal count As Integer) As Object
			valid
			Dim dst As Object = New Object(count - 1){}
			Array.Copy(CObj(elements), CInt(index), dst, 0, count)
			Return dst
		End Function

		''' <summary>
		''' Returns a new array that is a copy of a slice
		''' of this <code>SerialArray</code> object, starting with the
		''' element at the given index and containing the given number
		''' of consecutive elements.
		''' <P>
		''' This method does custom mapping if the array elements are a UDT
		''' and the given type map has an entry for that UDT.
		''' Custom mapping is recursive,
		''' meaning that if, for instance, an element of an SQL structured type
		''' is an SQL structured type that itself has an element that is an SQL
		''' structured type, each structured type that has a custom mapping will be
		''' mapped according to the given type map.
		''' </summary>
		''' <param name="index"> the index into this <code>SerialArray</code> object
		'''              of the first element to be copied; the index of the
		'''              first element in the array is <code>0</code> </param>
		''' <param name="count"> the number of consecutive elements to be copied, starting
		'''              at the given index </param>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <returns> a copy of the designated elements in this <code>SerialArray</code>
		'''         object as an <code>Object</code> in the Java programming language </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Function getArray(ByVal index As Long, ByVal count As Integer, ByVal map As IDictionary(Of String, Type)) As Object
			valid
			Dim dst As Object = New Object(count - 1){}
			Array.Copy(CObj(elements), CInt(index), dst, 0, count)
			Return dst
		End Function

		''' <summary>
		''' Retrieves the SQL type of the elements in this <code>SerialArray</code>
		''' object.  The <code>int</code> returned is one of the constants in the class
		''' <code>java.sql.Types</code>.
		''' </summary>
		''' <returns> one of the constants in <code>java.sql.Types</code>, indicating
		'''         the SQL type of the elements in this <code>SerialArray</code> object </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Property baseType As Integer
			Get
				valid
				Return baseType
			End Get
		End Property

		''' <summary>
		''' Retrieves the DBMS-specific type name for the elements in this
		''' <code>SerialArray</code> object.
		''' </summary>
		''' <returns> the SQL type name used by the DBMS for the base type of this
		'''         <code>SerialArray</code> object </returns>
		''' <exception cref="SerialException"> if an error occurs;
		''' if {@code free} had previously been called on this object </exception>
		Public Overridable Property baseTypeName As String
			Get
				valid
				Return baseTypeName
			End Get
		End Property

		''' <summary>
		''' Retrieves a <code>ResultSet</code> object holding the elements of
		''' the subarray that starts at
		''' index <i>index</i> and contains up to <i>count</i> successive elements.
		''' This method uses the connection's type map to map the elements of
		''' the array if the map contains
		''' an entry for the base type. Otherwise, the standard mapping is used.
		''' </summary>
		''' <param name="index"> the index into this <code>SerialArray</code> object
		'''         of the first element to be copied; the index of the
		'''         first element in the array is <code>0</code> </param>
		''' <param name="count"> the number of consecutive elements to be copied, starting
		'''         at the given index </param>
		''' <returns> a <code>ResultSet</code> object containing the designated
		'''         elements in this <code>SerialArray</code> object, with a
		'''         separate row for each element </returns>
		''' <exception cref="SerialException"> if called with the cause set to
		'''         {@code UnsupportedOperationException} </exception>
		Public Overridable Function getResultSet(ByVal index As Long, ByVal count As Integer) As ResultSet
			Dim se As New SerialException
			se.initCause(New System.NotSupportedException)
			Throw se
		End Function

		''' 
		''' <summary>
		''' Retrieves a <code>ResultSet</code> object that contains all of
		''' the elements of the SQL <code>ARRAY</code>
		''' value represented by this <code>SerialArray</code> object. This method uses
		''' the specified map for type map customizations unless the base type of the
		''' array does not match a user-defined type (UDT) in <i>map</i>, in
		''' which case it uses the
		''' standard mapping. This version of the method <code>getResultSet</code>
		''' uses either the given type map or the standard mapping; it never uses the
		''' type map associated with the connection.
		''' </summary>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <returns> a <code>ResultSet</code> object containing all of the
		'''         elements in this <code>SerialArray</code> object, with a
		'''         separate row for each element </returns>
		''' <exception cref="SerialException"> if called with the cause set to
		'''         {@code UnsupportedOperationException} </exception>
		Public Overridable Function getResultSet(ByVal map As IDictionary(Of String, Type)) As ResultSet
			Dim se As New SerialException
			se.initCause(New System.NotSupportedException)
			Throw se
		End Function

		''' <summary>
		''' Retrieves a <code>ResultSet</code> object that contains all of
		''' the elements in the <code>ARRAY</code> value that this
		''' <code>SerialArray</code> object represents.
		''' If appropriate, the elements of the array are mapped using the connection's
		''' type map; otherwise, the standard mapping is used.
		''' </summary>
		''' <returns> a <code>ResultSet</code> object containing all of the
		'''         elements in this <code>SerialArray</code> object, with a
		'''         separate row for each element </returns>
		''' <exception cref="SerialException"> if called with the cause set to
		'''         {@code UnsupportedOperationException} </exception>
		Public Overridable Property resultSet As ResultSet
			Get
				Dim se As New SerialException
				se.initCause(New System.NotSupportedException)
				Throw se
			End Get
		End Property


		''' <summary>
		''' Retrieves a result set holding the elements of the subarray that starts at
		''' Retrieves a <code>ResultSet</code> object that contains a subarray of the
		''' elements in this <code>SerialArray</code> object, starting at
		''' index <i>index</i> and containing up to <i>count</i> successive
		''' elements. This method uses
		''' the specified map for type map customizations unless the base type of the
		''' array does not match a user-defined type (UDT) in <i>map</i>, in
		''' which case it uses the
		''' standard mapping. This version of the method <code>getResultSet</code> uses
		''' either the given type map or the standard mapping; it never uses the type
		''' map associated with the connection.
		''' </summary>
		''' <param name="index"> the index into this <code>SerialArray</code> object
		'''              of the first element to be copied; the index of the
		'''              first element in the array is <code>0</code> </param>
		''' <param name="count"> the number of consecutive elements to be copied, starting
		'''              at the given index </param>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <returns> a <code>ResultSet</code> object containing the designated
		'''         elements in this <code>SerialArray</code> object, with a
		'''         separate row for each element </returns>
		''' <exception cref="SerialException"> if called with the cause set to
		'''         {@code UnsupportedOperationException} </exception>
		Public Overridable Function getResultSet(ByVal index As Long, ByVal count As Integer, ByVal map As IDictionary(Of String, Type)) As ResultSet
			Dim se As New SerialException
			se.initCause(New System.NotSupportedException)
			Throw se
		End Function


		''' <summary>
		''' Compares this SerialArray to the specified object.  The result is {@code
		''' true} if and only if the argument is not {@code null} and is a {@code
		''' SerialArray} object whose elements are identical to this object's elements
		''' </summary>
		''' <param name="obj"> The object to compare this {@code SerialArray} against
		''' </param>
		''' <returns>  {@code true} if the given object represents a {@code SerialArray}
		'''          equivalent to this SerialArray, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True

			If TypeOf obj Is SerialArray Then
				Dim sa As SerialArray = CType(obj, SerialArray)
				Return baseType = sa.baseType AndAlso baseTypeName.Equals(sa.baseTypeName) AndAlso java.util.Arrays.Equals(elements, sa.elements)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this SerialArray. The hash code for a
		''' {@code SerialArray} object is computed using the hash codes
		''' of the elements of the  {@code SerialArray} object
		''' </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (((31 + java.util.Arrays.hashCode(elements)) * 31 + len) * 31 + baseType) * 31 + baseTypeName.GetHashCode()
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialArray}. The copy will contain a
		''' reference to a clone of the underlying objects array, not a reference
		''' to the original underlying object array of this {@code SerialArray} object.
		''' </summary>
		''' <returns> a clone of this SerialArray </returns>
		Public Overridable Function clone() As Object
			Try
				Dim sa As SerialArray = CType(MyBase.clone(), SerialArray)
				sa.elements = If(elements IsNot Nothing, java.util.Arrays.copyOf(elements, len), Nothing)
				Return sa
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try

		End Function

		''' <summary>
		''' readObject is called to restore the state of the {@code SerialArray} from
		''' a stream.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)

		   Dim fields As ObjectInputStream.GetField = s.readFields()
		   Dim tmp As Object() = CType(fields.get("elements", Nothing), Object())
		   If tmp Is Nothing Then Throw New InvalidObjectException("elements is null and should not be!")
		   elements = tmp.clone()
		   len = fields.get("len", 0)
		   If elements.Length <> len Then Throw New InvalidObjectException("elements is not the expected size")

		   baseType = fields.get("baseType", 0)
		   baseTypeName = CStr(fields.get("baseTypeName", Nothing))
		End Sub

		''' <summary>
		''' writeObject is called to save the state of the {@code SerialArray}
		''' to a stream.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)

			Dim fields As ObjectOutputStream.PutField = s.putFields()
			fields.put("elements", elements)
			fields.put("len", len)
			fields.put("baseType", baseType)
			fields.put("baseTypeName", baseTypeName)
			s.writeFields()
		End Sub

		''' <summary>
		''' Check to see if this object had previously had its {@code free} method
		''' called
		''' </summary>
		''' <exception cref="SerialException"> </exception>
		Private Sub isValid()
			If elements Is Nothing Then Throw New SerialException("Error: You cannot call a method on a " & "SerialArray instance once free() has been called.")
		End Sub

		''' <summary>
		''' The identifier that assists in the serialization of this <code>SerialArray</code>
		''' object.
		''' </summary>
		Friend Const serialVersionUID As Long = -8466174297270688520L
	End Class

End Namespace
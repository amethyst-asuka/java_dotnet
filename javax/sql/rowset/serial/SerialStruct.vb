Imports System
Imports System.Collections.Generic
Imports javax.sql
Imports javax.sql.rowset

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
	''' A serialized mapping in the Java programming language of an SQL
	''' structured type. Each attribute that is not already serialized
	''' is mapped to a serialized form, and if an attribute is itself
	''' a structured type, each of its attributes that is not already
	''' serialized is mapped to a serialized form.
	''' <P>
	''' In addition, the structured type is custom mapped to a class in the
	''' Java programming language if there is such a mapping, as are
	''' its attributes, if appropriate.
	''' <P>
	''' The <code>SerialStruct</code> class provides a constructor for creating
	''' an instance from a <code>Struct</code> object, a method for retrieving
	''' the SQL type name of the SQL structured type in the database, and methods
	''' for retrieving its attribute values.
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' A SerialStruct is not safe for use by multiple concurrent threads.  If a
	''' SerialStruct is to be used by more than one thread then access to the
	''' SerialStruct should be controlled by appropriate synchronization.
	''' 
	''' </summary>
	<Serializable> _
	Public Class SerialStruct
		Implements Struct, ICloneable


		''' <summary>
		''' The SQL type name for the structured type that this
		''' <code>SerialStruct</code> object represents.  This is the name
		''' used in the SQL definition of the SQL structured type.
		''' 
		''' @serial
		''' </summary>
		Private SQLTypeName As String

		''' <summary>
		''' An array of <code>Object</code> instances in  which each
		''' element is an attribute of the SQL structured type that this
		''' <code>SerialStruct</code> object represents.  The attributes are
		''' ordered according to their order in the definition of the
		''' SQL structured type.
		''' 
		''' @serial
		''' </summary>
		Private attribs As Object()

		''' <summary>
		''' Constructs a <code>SerialStruct</code> object from the given
		''' <code>Struct</code> object, using the given <code>java.util.Map</code>
		''' object for custom mapping the SQL structured type or any of its
		''' attributes that are SQL structured types.
		''' </summary>
		''' <param name="in"> an instance of {@code Struct} </param>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <exception cref="SerialException"> if an error occurs </exception>
		''' <seealso cref= java.sql.Struct </seealso>
		 Public Sub New(ByVal [in] As Struct, ByVal map As IDictionary(Of String, Type))

			Try

			' get the type name
			SQLTypeName = [in].sQLTypeName
			Console.WriteLine("SQLTypeName: " & SQLTypeName)

			' get the attributes of the struct
			attribs = [in].getAttributes(map)

	'        
	'         * the array may contain further Structs
	'         * and/or classes that have been mapped,
	'         * other types that we have to serialize
	'         
			mapToSerial(map)

			Catch e As SQLException
				Throw New SerialException(e.Message)
			End Try
		 End Sub

		 ''' <summary>
		 ''' Constructs a <code>SerialStruct</code> object from the
		 ''' given <code>SQLData</code> object, using the given type
		 ''' map to custom map it to a class in the Java programming
		 ''' language.  The type map gives the SQL type and the class
		 ''' to which it is mapped.  The <code>SQLData</code> object
		 ''' defines the class to which the SQL type will be mapped.
		 ''' </summary>
		 ''' <param name="in"> an instance of the <code>SQLData</code> class
		 '''           that defines the mapping of the SQL structured
		 '''           type to one or more objects in the Java programming language </param>
		 ''' <param name="map"> a <code>java.util.Map</code> object in which
		 '''        each entry consists of 1) a <code>String</code> object
		 '''        giving the fully qualified name of a UDT and 2) the
		 '''        <code>Class</code> object for the <code>SQLData</code> implementation
		 '''        that defines how the UDT is to be mapped </param>
		 ''' <exception cref="SerialException"> if an error occurs </exception>
		Public Sub New(ByVal [in] As SQLData, ByVal map As IDictionary(Of String, Type))

			Try

			'set the type name
			SQLTypeName = [in].sQLTypeName

			Dim tmp As New List(Of Object)
			[in].writeSQL(New SQLOutputImpl(tmp, map))
			attribs = tmp.ToArray()

			Catch e As SQLException
				Throw New SerialException(e.Message)
			End Try
		End Sub


		''' <summary>
		''' Retrieves the SQL type name for this <code>SerialStruct</code>
		''' object. This is the name used in the SQL definition of the
		''' structured type
		''' </summary>
		''' <returns> a <code>String</code> object representing the SQL
		'''         type name for the SQL structured type that this
		'''         <code>SerialStruct</code> object represents </returns>
		''' <exception cref="SerialException"> if an error occurs </exception>
		Public Overridable Property sQLTypeName As String
			Get
				Return SQLTypeName
			End Get
		End Property

		''' <summary>
		''' Retrieves an array of <code>Object</code> values containing the
		''' attributes of the SQL structured type that this
		''' <code>SerialStruct</code> object represents.
		''' </summary>
		''' <returns> an array of <code>Object</code> values, with each
		'''         element being an attribute of the SQL structured type
		'''         that this <code>SerialStruct</code> object represents </returns>
		''' <exception cref="SerialException"> if an error occurs </exception>
		Public Overridable Property attributes As Object()
			Get
				Dim val As Object() = Me.attribs
				Return If(val Is Nothing, Nothing, java.util.Arrays.copyOf(val, val.Length))
			End Get
		End Property

		''' <summary>
		''' Retrieves the attributes for the SQL structured type that
		''' this <code>SerialStruct</code> represents as an array of
		''' <code>Object</code> values, using the given type map for
		''' custom mapping if appropriate.
		''' </summary>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <returns> an array of <code>Object</code> values, with each
		'''         element being an attribute of the SQL structured
		'''         type that this <code>SerialStruct</code> object
		'''         represents </returns>
		''' <exception cref="SerialException"> if an error occurs </exception>
		Public Overridable Function getAttributes(ByVal map As IDictionary(Of String, Type)) As Object()
			Dim val As Object() = Me.attribs
			Return If(val Is Nothing, Nothing, java.util.Arrays.copyOf(val, val.Length))
		End Function


		''' <summary>
		''' Maps attributes of an SQL structured type that are not
		''' serialized to a serialized form, using the given type map
		''' for custom mapping when appropriate.  The following types
		''' in the Java programming language are mapped to their
		''' serialized forms:  <code>Struct</code>, <code>SQLData</code>,
		''' <code>Ref</code>, <code>Blob</code>, <code>Clob</code>, and
		''' <code>Array</code>.
		''' <P>
		''' This method is called internally and is not used by an
		''' application programmer.
		''' </summary>
		''' <param name="map"> a <code>java.util.Map</code> object in which
		'''        each entry consists of 1) a <code>String</code> object
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <exception cref="SerialException"> if an error occurs </exception>
		Private Sub mapToSerial(ByVal map As IDictionary(Of String, Type))

			Try

			For i As Integer = 0 To attribs.Length - 1
				If TypeOf attribs(i) Is Struct Then
					attribs(i) = New SerialStruct(CType(attribs(i), Struct), map)
				ElseIf TypeOf attribs(i) Is SQLData Then
					attribs(i) = New SerialStruct(CType(attribs(i), SQLData), map)
				ElseIf TypeOf attribs(i) Is Blob Then
					attribs(i) = New SerialBlob(CType(attribs(i), Blob))
				ElseIf TypeOf attribs(i) Is Clob Then
					attribs(i) = New SerialClob(CType(attribs(i), Clob))
				ElseIf TypeOf attribs(i) Is Ref Then
					attribs(i) = New SerialRef(CType(attribs(i), Ref))
				ElseIf TypeOf attribs(i) Is java.sql.Array Then
					attribs(i) = New SerialArray(CType(attribs(i), java.sql.Array), map)
				End If
			Next i

			Catch e As SQLException
				Throw New SerialException(e.Message)
			End Try
			Return
		End Sub

		''' <summary>
		''' Compares this SerialStruct to the specified object.  The result is
		''' {@code true} if and only if the argument is not {@code null} and is a
		''' {@code SerialStruct} object whose attributes are identical to this
		''' object's attributes
		''' </summary>
		''' <param name="obj"> The object to compare this {@code SerialStruct} against
		''' </param>
		''' <returns> {@code true} if the given object represents a {@code SerialStruct}
		'''          equivalent to this SerialStruct, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is SerialStruct Then
				Dim ss As SerialStruct = CType(obj, SerialStruct)
				Return SQLTypeName.Equals(ss.SQLTypeName) AndAlso java.util.Arrays.Equals(attribs, ss.attribs)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this {@code SerialStruct}. The hash code for a
		''' {@code SerialStruct} object is computed using the hash codes
		''' of the attributes of the {@code SerialStruct} object and its
		''' {@code SQLTypeName}
		''' </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return ((31 + java.util.Arrays.hashCode(attribs)) * 31) * 31 + SQLTypeName.GetHashCode()
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialStruct}. The copy will contain a
		''' reference to a clone of the underlying attribs array, not a reference
		''' to the original underlying attribs array of this {@code SerialStruct} object.
		''' </summary>
		''' <returns>  a clone of this SerialStruct </returns>
		Public Overridable Function clone() As Object
			Try
				Dim ss As SerialStruct = CType(MyBase.clone(), SerialStruct)
				ss.attribs = java.util.Arrays.copyOf(attribs, attribs.Length)
				Return ss
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try

		End Function

		''' <summary>
		''' readObject is called to restore the state of the {@code SerialStruct} from
		''' a stream.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)

		   Dim fields As ObjectInputStream.GetField = s.readFields()
		   Dim tmp As Object() = CType(fields.get("attribs", Nothing), Object())
		   attribs = If(tmp Is Nothing, Nothing, tmp.clone())
		   SQLTypeName = CStr(fields.get("SQLTypeName", Nothing))
		End Sub

		''' <summary>
		''' writeObject is called to save the state of the {@code SerialStruct}
		''' to a stream.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)

			Dim fields As ObjectOutputStream.PutField = s.putFields()
			fields.put("attribs", attribs)
			fields.put("SQLTypeName", SQLTypeName)
			s.writeFields()
		End Sub

		''' <summary>
		''' The identifier that assists in the serialization of this
		''' <code>SerialStruct</code> object.
		''' </summary>
		Friend Const serialVersionUID As Long = -8322445504027483372L
	End Class

End Namespace
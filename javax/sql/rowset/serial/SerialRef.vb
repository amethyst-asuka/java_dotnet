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
	''' A serialized mapping of a <code>Ref</code> object, which is the mapping in the
	''' Java programming language of an SQL <code>REF</code> value.
	''' <p>
	''' The <code>SerialRef</code> class provides a constructor  for
	''' creating a <code>SerialRef</code> instance from a <code>Ref</code>
	''' object and provides methods for getting and setting the <code>Ref</code> object.
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' A SerialRef is not safe for use by multiple concurrent threads.  If a
	''' SerialRef is to be used by more than one thread then access to the SerialRef
	''' should be controlled by appropriate synchronization.
	''' 
	''' </summary>
	<Serializable> _
	Public Class SerialRef
		Implements Ref, ICloneable

		''' <summary>
		''' String containing the base type name.
		''' @serial
		''' </summary>
		Private baseTypeName As String

		''' <summary>
		''' This will store the type <code>Ref</code> as an <code>Object</code>.
		''' </summary>
		Private [object] As Object

		''' <summary>
		''' Private copy of the Ref reference.
		''' </summary>
		Private reference As Ref

		''' <summary>
		''' Constructs a <code>SerialRef</code> object from the given <code>Ref</code>
		''' object.
		''' </summary>
		''' <param name="ref"> a Ref object; cannot be <code>null</code> </param>
		''' <exception cref="SQLException"> if a database access occurs; if <code>ref</code>
		'''     is <code>null</code>; or if the <code>Ref</code> object returns a
		'''     <code>null</code> value base type name. </exception>
		''' <exception cref="SerialException"> if an error occurs serializing the <code>Ref</code>
		'''     object </exception>
		Public Sub New(ByVal ref As Ref)
			If ref Is Nothing Then Throw New SQLException("Cannot instantiate a SerialRef object " & "with a null Ref object")
			reference = ref
			[object] = ref
			If ref.baseTypeName Is Nothing Then
				Throw New SQLException("Cannot instantiate a SerialRef object " & "that returns a null base type name")
			Else
				baseTypeName = ref.baseTypeName
			End If
		End Sub

		''' <summary>
		''' Returns a string describing the base type name of the <code>Ref</code>.
		''' </summary>
		''' <returns> a string of the base type name of the Ref </returns>
		''' <exception cref="SerialException"> in no Ref object has been set </exception>
		Public Overridable Property baseTypeName As String
			Get
				Return baseTypeName
			End Get
		End Property

		''' <summary>
		''' Returns an <code>Object</code> representing the SQL structured type
		''' to which this <code>SerialRef</code> object refers.  The attributes
		''' of the structured type are mapped according to the given type map.
		''' </summary>
		''' <param name="map"> a <code>java.util.Map</code> object containing zero or
		'''        more entries, with each entry consisting of 1) a <code>String</code>
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <returns> an object instance resolved from the Ref reference and mapped
		'''        according to the supplied type map </returns>
		''' <exception cref="SerialException"> if an error is encountered in the reference
		'''        resolution </exception>
		Public Overridable Function getObject(ByVal map As IDictionary(Of String, Type)) As Object
			map = New Dictionary(Of String, Type)(map)
			If [object] IsNot Nothing Then
				Return map([object])
			Else
				Throw New SerialException("The object is not set")
			End If
		End Function

		''' <summary>
		''' Returns an <code>Object</code> representing the SQL structured type
		''' to which this <code>SerialRef</code> object refers.
		''' </summary>
		''' <returns> an object instance resolved from the Ref reference </returns>
		''' <exception cref="SerialException"> if an error is encountered in the reference
		'''         resolution </exception>
		Public Overridable Property [object] As Object
			Get
    
				If reference IsNot Nothing Then
					Try
						Return reference.object
					Catch e As SQLException
						Throw New SerialException("SQLException: " & e.Message)
					End Try
				End If
    
				If [object] IsNot Nothing Then Return [object]
    
    
				Throw New SerialException("The object is not set")
    
			End Get
			Set(ByVal obj As Object)
				Try
					reference.object = obj
				Catch e As SQLException
					Throw New SerialException("SQLException: " & e.Message)
				End Try
				[object] = obj
			End Set
		End Property


		''' <summary>
		''' Compares this SerialRef to the specified object.  The result is {@code
		''' true} if and only if the argument is not {@code null} and is a {@code
		''' SerialRef} object that represents the same object as this
		''' object.
		''' </summary>
		''' <param name="obj"> The object to compare this {@code SerialRef} against
		''' </param>
		''' <returns>  {@code true} if the given object represents a {@code SerialRef}
		'''          equivalent to this SerialRef, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is SerialRef Then
				Dim ref As SerialRef = CType(obj, SerialRef)
				Return baseTypeName.Equals(ref.baseTypeName) AndAlso [object].Equals(ref.object)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this {@code SerialRef}. </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (31 + [object].GetHashCode()) * 31 + baseTypeName.GetHashCode()
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialRef}.
		''' The underlying {@code Ref} object will be set to null.
		''' </summary>
		''' <returns>  a clone of this SerialRef </returns>
		Public Overridable Function clone() As Object
			Try
				Dim ref As SerialRef = CType(MyBase.clone(), SerialRef)
				ref.reference = Nothing
				Return ref
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try

		End Function

		''' <summary>
		''' readObject is called to restore the state of the SerialRef from
		''' a stream.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)
			Dim fields As ObjectInputStream.GetField = s.readFields()
			[object] = fields.get("object", Nothing)
			baseTypeName = CStr(fields.get("baseTypeName", Nothing))
			reference = CType(fields.get("reference", Nothing), Ref)
		End Sub

		''' <summary>
		''' writeObject is called to save the state of the SerialRef
		''' to a stream.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)

			Dim fields As ObjectOutputStream.PutField = s.putFields()
			fields.put("baseTypeName", baseTypeName)
			fields.put("object", [object])
			' Note: this check to see if it is an instance of Serializable
			' is for backwards compatibiity
			fields.put("reference",If(TypeOf reference Is Serializable, reference, Nothing))
			s.writeFields()
		End Sub

		''' <summary>
		''' The identifier that assists in the serialization of this <code>SerialRef</code>
		''' object.
		''' </summary>
		Friend Const serialVersionUID As Long = -4727123500609662274L


	End Class

End Namespace
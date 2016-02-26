Imports System

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
	''' <code>DATALINK</code> value. A <code>DATALINK</code> value
	''' references a file outside of the underlying data source that the
	''' data source manages.
	''' <P>
	''' <code>RowSet</code> implementations can use the method <code>RowSet.getURL</code>
	''' to retrieve a <code>java.net.URL</code> object, which can be used
	''' to manipulate the external data.
	''' <pre>
	'''      java.net.URL url = rowset.getURL(1);
	''' </pre>
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' A SerialDatalink is not safe for use by multiple concurrent threads.  If a
	''' SerialDatalink is to be used by more than one thread then access to the
	''' SerialDatalink should be controlled by appropriate synchronization.
	''' </summary>
	<Serializable> _
	Public Class SerialDatalink
		Implements ICloneable

		''' <summary>
		''' The extracted URL field retrieved from the DATALINK field.
		''' @serial
		''' </summary>
		Private url As java.net.URL

		''' <summary>
		''' The SQL type of the elements in this <code>SerialDatalink</code>
		''' object.  The type is expressed as one of the contants from the
		''' class <code>java.sql.Types</code>.
		''' @serial
		''' </summary>
		Private baseType As Integer

		''' <summary>
		''' The type name used by the DBMS for the elements in the SQL
		''' <code>DATALINK</code> value that this SerialDatalink object
		''' represents.
		''' @serial
		''' </summary>
		Private baseTypeName As String

		''' <summary>
		''' Constructs a new <code>SerialDatalink</code> object from the given
		''' <code>java.net.URL</code> object.
		''' <P> </summary>
		''' <param name="url"> the {@code URL} to create the {@code SerialDataLink} from </param>
		''' <exception cref="SerialException"> if url parameter is a null </exception>
		Public Sub New(ByVal url As java.net.URL)
			If url Is Nothing Then Throw New SerialException("Cannot serialize empty URL instance")
			Me.url = url
		End Sub

		''' <summary>
		''' Returns a new URL that is a copy of this <code>SerialDatalink</code>
		''' object.
		''' </summary>
		''' <returns> a copy of this <code>SerialDatalink</code> object as a
		''' <code>URL</code> object in the Java programming language. </returns>
		''' <exception cref="SerialException"> if the <code>URL</code> object cannot be de-serialized </exception>
		Public Overridable Property datalink As java.net.URL
			Get
    
				Dim aURL As java.net.URL = Nothing
    
				Try
					aURL = New java.net.URL((Me.url).ToString())
				Catch e As java.net.MalformedURLException
					Throw New SerialException("MalformedURLException: " & e.Message)
				End Try
				Return aURL
			End Get
		End Property

		''' <summary>
		''' Compares this {@code SerialDatalink} to the specified object.
		''' The result is {@code true} if and only if the argument is not
		''' {@code null} and is a {@code SerialDatalink} object whose URL is
		''' identical to this object's URL
		''' </summary>
		''' <param name="obj"> The object to compare this {@code SerialDatalink} against
		''' </param>
		''' <returns>  {@code true} if the given object represents a {@code SerialDatalink}
		'''          equivalent to this SerialDatalink, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is SerialDatalink Then
				Dim sdl As SerialDatalink = CType(obj, SerialDatalink)
				Return url.Equals(sdl.url)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this {@code SerialDatalink}. The hash code for a
		''' {@code SerialDatalink} object is taken as the hash code of
		''' the {@code URL} it stores
		''' </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return 31 + url.GetHashCode()
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialDatalink}.
		''' </summary>
		''' <returns>  a clone of this SerialDatalink </returns>
		Public Overridable Function clone() As Object
			Try
				Dim sdl As SerialDatalink = CType(MyBase.clone(), SerialDatalink)
				Return sdl
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try
		End Function

		''' <summary>
		''' readObject and writeObject are called to restore the state
		''' of the {@code SerialDatalink}
		''' from a stream. Note: we leverage the default Serialized form
		''' </summary>

		''' <summary>
		''' The identifier that assists in the serialization of this
		'''  {@code SerialDatalink} object.
		''' </summary>
		Friend Const serialVersionUID As Long = 2826907821828733626L
	End Class

End Namespace
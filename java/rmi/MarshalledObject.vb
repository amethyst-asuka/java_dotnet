Imports System

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

Namespace java.rmi


	''' <summary>
	''' A <code>MarshalledObject</code> contains a byte stream with the serialized
	''' representation of an object given to its constructor.  The <code>get</code>
	''' method returns a new copy of the original object, as deserialized from
	''' the contained byte stream.  The contained object is serialized and
	''' deserialized with the same serialization semantics used for marshaling
	''' and unmarshaling parameters and return values of RMI calls:  When the
	''' serialized form is created:
	''' 
	''' <ul>
	''' <li> classes are annotated with a codebase URL from where the class
	'''      can be loaded (if available), and
	''' <li> any remote object in the <code>MarshalledObject</code> is
	'''      represented by a serialized instance of its stub.
	''' </ul>
	''' 
	''' <p>When copy of the object is retrieved (via the <code>get</code> method),
	''' if the class is not available locally, it will be loaded from the
	''' appropriate location (specified the URL annotated with the class descriptor
	''' when the class was serialized.
	''' 
	''' <p><code>MarshalledObject</code> facilitates passing objects in RMI calls
	''' that are not automatically deserialized immediately by the remote peer.
	''' </summary>
	''' @param <T> the type of the object contained in this
	''' <code>MarshalledObject</code>
	''' 
	''' @author  Ann Wollrath
	''' @author  Peter Jones
	''' @since   1.2 </param>
	<Serializable> _
	Public NotInheritable Class MarshalledObject(Of T)
		''' <summary>
		''' @serial Bytes of serialized representation.  If <code>objBytes</code> is
		''' <code>null</code> then the object marshalled was a <code>null</code>
		''' reference.
		''' </summary>
		Private objBytes As SByte() = Nothing

		''' <summary>
		''' @serial Bytes of location annotations, which are ignored by
		''' <code>equals</code>.  If <code>locBytes</code> is null, there were no
		''' non-<code>null</code> annotations during marshalling.
		''' </summary>
		Private locBytes As SByte() = Nothing

		''' <summary>
		''' @serial Stored hash code of contained object.
		''' </summary>
		''' <seealso cref= #hashCode </seealso>
		Private hash As Integer

		''' <summary>
		''' Indicate compatibility with 1.2 version of class. </summary>
		Private Const serialVersionUID As Long = 8988374069173025854L

		''' <summary>
		''' Creates a new <code>MarshalledObject</code> that contains the
		''' serialized representation of the current state of the supplied object.
		''' The object is serialized with the semantics used for marshaling
		''' parameters for RMI calls.
		''' </summary>
		''' <param name="obj"> the object to be serialized (must be serializable) </param>
		''' <exception cref="IOException"> if an <code>IOException</code> occurs; an
		''' <code>IOException</code> may occur if <code>obj</code> is not
		''' serializable.
		''' @since 1.2 </exception>
		Public Sub New(  obj As T)
			If obj Is Nothing Then
				hash = 13
				Return
			End If

			Dim bout As New java.io.ByteArrayOutputStream
			Dim lout As New java.io.ByteArrayOutputStream
			Dim out As New MarshalledObjectOutputStream(bout, lout)
			out.writeObject(obj)
			out.flush()
			objBytes = bout.toByteArray()
			' locBytes is null if no annotations
			locBytes = (If(out.hadAnnotations(), lout.toByteArray(), Nothing))

	'        
	'         * Calculate hash from the marshalled representation of object
	'         * so the hashcode will be comparable when sent between VMs.
	'         
			Dim h As Integer = 0
			For i As Integer = 0 To objBytes.Length - 1
				h = 31 * h + objBytes(i)
			Next i
			hash = h
		End Sub

		''' <summary>
		''' Returns a new copy of the contained marshalledobject.  The internal
		''' representation is deserialized with the semantics used for
		''' unmarshaling parameters for RMI calls.
		''' </summary>
		''' <returns> a copy of the contained object </returns>
		''' <exception cref="IOException"> if an <code>IOException</code> occurs while
		''' deserializing the object from its internal representation. </exception>
		''' <exception cref="ClassNotFoundException"> if a
		''' <code>ClassNotFoundException</code> occurs while deserializing the
		''' object from its internal representation.
		''' could not be found
		''' @since 1.2 </exception>
		Public Function [get]() As T
			If objBytes Is Nothing Then ' must have been a null object Return Nothing

			Dim bin As New java.io.ByteArrayInputStream(objBytes)
			' locBytes is null if no annotations
			Dim lin As java.io.ByteArrayInputStream = (If(locBytes Is Nothing, Nothing, New java.io.ByteArrayInputStream(locBytes)))
			Dim [in] As New MarshalledObjectInputStream(bin, lin)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim obj As T = CType([in].readObject(), T)
			[in].close()
			Return obj
		End Function

		''' <summary>
		''' Return a hash code for this <code>MarshalledObject</code>.
		''' </summary>
		''' <returns> a hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return hash
		End Function

		''' <summary>
		''' Compares this <code>MarshalledObject</code> to another object.
		''' Returns true if and only if the argument refers to a
		''' <code>MarshalledObject</code> that contains exactly the same
		''' serialized representation of an object as this one does. The
		''' comparison ignores any class codebase annotation, meaning that
		''' two objects are equivalent if they have the same serialized
		''' representation <i>except</i> for the codebase of each class
		''' in the serialized representation.
		''' </summary>
		''' <param name="obj"> the object to compare with this <code>MarshalledObject</code> </param>
		''' <returns> <code>true</code> if the argument contains an equivalent
		''' serialized object; <code>false</code> otherwise
		''' @since 1.2 </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Me Then Return True

			If obj IsNot Nothing AndAlso TypeOf obj Is MarshalledObject Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim other As MarshalledObject(Of ?) = CType(obj, MarshalledObject(Of ?))

				' if either is a ref to null, both must be
				If objBytes Is Nothing OrElse other.objBytes Is Nothing Then Return objBytes = other.objBytes

				' quick, easy test
				If objBytes.Length <> other.objBytes.Length Then Return False

				'!! There is talk about adding an array comparision method
				'!! at 1.2 -- if so, this should be rewritten.  -arnold
				For i As Integer = 0 To objBytes.Length - 1
					If objBytes(i) <> other.objBytes(i) Then Return False
				Next i
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' This class is used to marshal objects for
		''' <code>MarshalledObject</code>.  It places the location annotations
		''' to one side so that two <code>MarshalledObject</code>s can be
		''' compared for equality if they differ only in location
		''' annotations.  Objects written using this stream should be read back
		''' from a <code>MarshalledObjectInputStream</code>.
		''' </summary>
		''' <seealso cref= java.rmi.MarshalledObject </seealso>
		''' <seealso cref= MarshalledObjectInputStream </seealso>
		Private Class MarshalledObjectOutputStream
			Inherits sun.rmi.server.MarshalOutputStream

			''' <summary>
			''' The stream on which location objects are written. </summary>
			Private locOut As java.io.ObjectOutputStream

			''' <summary>
			''' <code>true</code> if non-<code>null</code> annotations are
			'''  written.
			''' </summary>
			Private hadAnnotations_Renamed As Boolean

			''' <summary>
			''' Creates a new <code>MarshalledObjectOutputStream</code> whose
			''' non-location bytes will be written to <code>objOut</code> and whose
			''' location annotations (if any) will be written to
			''' <code>locOut</code>.
			''' </summary>
			Friend Sub New(  objOut As java.io.OutputStream,   locOut As java.io.OutputStream)
				MyBase.New(objOut)
				Me.useProtocolVersion(java.io.ObjectStreamConstants.PROTOCOL_VERSION_2)
				Me.locOut = New java.io.ObjectOutputStream(locOut)
				hadAnnotations_Renamed = False
			End Sub

			''' <summary>
			''' Returns <code>true</code> if any non-<code>null</code> location
			''' annotations have been written to this stream.
			''' </summary>
			Friend Overridable Function hadAnnotations() As Boolean
				Return hadAnnotations_Renamed
			End Function

			''' <summary>
			''' Overrides MarshalOutputStream.writeLocation implementation to write
			''' annotations to the location stream.
			''' </summary>
			Protected Friend Overridable Sub writeLocation(  loc As String)
				hadAnnotations_Renamed = hadAnnotations_Renamed Or (loc IsNot Nothing)
				locOut.writeObject(loc)
			End Sub


			Public Overridable Sub flush()
				MyBase.flush()
				locOut.flush()
			End Sub
		End Class

		''' <summary>
		''' The counterpart to <code>MarshalledObjectOutputStream</code>.
		''' </summary>
		''' <seealso cref= MarshalledObjectOutputStream </seealso>
		Private Class MarshalledObjectInputStream
			Inherits sun.rmi.server.MarshalInputStream

			''' <summary>
			''' The stream from which annotations will be read.  If this is
			''' <code>null</code>, then all annotations were <code>null</code>.
			''' </summary>
			Private locIn As java.io.ObjectInputStream

			''' <summary>
			''' Creates a new <code>MarshalledObjectInputStream</code> that
			''' reads its objects from <code>objIn</code> and annotations
			''' from <code>locIn</code>.  If <code>locIn</code> is
			''' <code>null</code>, then all annotations will be
			''' <code>null</code>.
			''' </summary>
			Friend Sub New(  objIn As java.io.InputStream,   locIn As java.io.InputStream)
				MyBase.New(objIn)
				Me.locIn = (If(locIn Is Nothing, Nothing, New java.io.ObjectInputStream(locIn)))
			End Sub

			''' <summary>
			''' Overrides MarshalInputStream.readLocation to return locations from
			''' the stream we were given, or <code>null</code> if we were given a
			''' <code>null</code> location stream.
			''' </summary>
			Protected Friend Overridable Function readLocation() As Object
				Return (If(locIn Is Nothing, Nothing, locIn.readObject()))
			End Function
		End Class

	End Class

End Namespace
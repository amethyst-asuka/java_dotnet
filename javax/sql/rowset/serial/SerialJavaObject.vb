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
	''' A serializable mapping in the Java programming language of an SQL
	''' <code>JAVA_OBJECT</code> value. Assuming the Java object
	''' implements the <code>Serializable</code> interface, this class simply wraps the
	''' serialization process.
	''' <P>
	''' If however, the serialization is not possible because
	''' the Java object is not immediately serializable, this class will
	''' attempt to serialize all non-static members to permit the object
	''' state to be serialized.
	''' Static or transient fields cannot be serialized; an attempt to serialize
	''' them will result in a <code>SerialException</code> object being thrown.
	''' 
	''' <h3> Thread safety </h3>
	''' 
	''' A SerialJavaObject is not safe for use by multiple concurrent threads.  If a
	''' SerialJavaObject is to be used by more than one thread then access to the
	''' SerialJavaObject should be controlled by appropriate synchronization.
	''' 
	''' @author Jonathan Bruce
	''' </summary>
	<Serializable> _
	Public Class SerialJavaObject
		Implements ICloneable

		''' <summary>
		''' Placeholder for object to be serialized.
		''' </summary>
		Private obj As Object


	   ''' <summary>
	   ''' Placeholder for all fields in the <code>JavaObject</code> being serialized.
	   ''' </summary>
		<NonSerialized> _
		Private fields As Field()

		''' <summary>
		''' Constructor for <code>SerialJavaObject</code> helper class.
		''' <p>
		''' </summary>
		''' <param name="obj"> the Java <code>Object</code> to be serialized </param>
		''' <exception cref="SerialException"> if the object is found not to be serializable </exception>
		Public Sub New(ByVal obj As Object)

			' if any static fields are found, an exception
			' should be thrown


			' get Class. Object instance should always be available
			Dim c As Type = obj.GetType()

			' determine if object implements Serializable i/f
			If Not(TypeOf obj Is java.io.Serializable) Then warning = New javax.sql.rowset.RowSetWarning("Warning, the object passed to the constructor does not implement Serializable")

			' can only determine public fields (obviously). If
			' any of these are static, this should invalidate
			' the action of attempting to persist these fields
			' in a serialized form
			fields = c.GetFields()

			If hasStaticFields(fields) Then Throw New SerialException("Located static fields in " & "object instance. Cannot serialize")

			Me.obj = obj
		End Sub

		''' <summary>
		''' Returns an <code>Object</code> that is a copy of this <code>SerialJavaObject</code>
		''' object.
		''' </summary>
		''' <returns> a copy of this <code>SerialJavaObject</code> object as an
		'''         <code>Object</code> in the Java programming language </returns>
		''' <exception cref="SerialException"> if the instance is corrupt </exception>
		Public Overridable Property [object] As Object
			Get
				Return Me.obj
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>Field</code> objects that contains each
		''' field of the object that this helper class is serializing.
		''' </summary>
		''' <returns> an array of <code>Field</code> objects </returns>
		''' <exception cref="SerialException"> if an error is encountered accessing
		''' the serialized object </exception>
		''' <exception cref="SecurityException">  If a security manager, <i>s</i>, is present
		''' and the caller's class loader is not the same as or an
		''' ancestor of the class loader for the class of the
		''' <seealso cref="#getObject object"/> being serialized
		''' and invocation of {@link SecurityManager#checkPackageAccess
		''' s.checkPackageAccess()} denies access to the package
		''' of that class. </exception>
		''' <seealso cref= Class#getFields </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property fields As Field()
			Get
				If fields IsNot Nothing Then
					Dim c As Type = Me.obj.GetType()
					Dim sm As SecurityManager = System.securityManager
					If sm IsNot Nothing Then
		'                
		'                 * Check if the caller is allowed to access the specified class's package.
		'                 * If access is denied, throw a SecurityException.
		'                 
						Dim caller As Type = sun.reflect.Reflection.callerClass
						If sun.reflect.misc.ReflectUtil.needsPackageAccessCheck(caller.classLoader, c.classLoader) Then sun.reflect.misc.ReflectUtil.checkPackageAccess(c)
					End If
					Return c.GetFields()
				Else
					Throw New SerialException("SerialJavaObject does not contain" & " a serialized object instance")
				End If
			End Get
		End Property

		''' <summary>
		''' The identifier that assists in the serialization of this
		''' <code>SerialJavaObject</code> object.
		''' </summary>
		Friend Const serialVersionUID As Long = -1465795139032831023L

		''' <summary>
		''' A container for the warnings issued on this <code>SerialJavaObject</code>
		''' object. When there are multiple warnings, each warning is chained to the
		''' previous warning.
		''' </summary>
		Friend chain As List(Of javax.sql.rowset.RowSetWarning)

		''' <summary>
		''' Compares this SerialJavaObject to the specified object.
		''' The result is {@code true} if and only if the argument
		''' is not {@code null} and is a {@code SerialJavaObject}
		''' object that is identical to this object
		''' </summary>
		''' <param name="o"> The object to compare this {@code SerialJavaObject} against
		''' </param>
		''' <returns>  {@code true} if the given object represents a {@code SerialJavaObject}
		'''          equivalent to this SerialJavaObject, {@code false} otherwise
		'''  </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then Return True
			If TypeOf o Is SerialJavaObject Then
				Dim sjo As SerialJavaObject = CType(o, SerialJavaObject)
				Return obj.Equals(sjo.obj)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this SerialJavaObject. The hash code for a
		''' {@code SerialJavaObject} object is taken as the hash code of
		''' the {@code Object} it stores
		''' </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return 31 + obj.GetHashCode()
		End Function

		''' <summary>
		''' Returns a clone of this {@code SerialJavaObject}.
		''' </summary>
		''' <returns>  a clone of this SerialJavaObject </returns>

		Public Overridable Function clone() As Object
			Try
				Dim sjo As SerialJavaObject = CType(MyBase.clone(), SerialJavaObject)
				sjo.fields = java.util.Arrays.copyOf(fields, fields.Length)
				If chain IsNot Nothing Then sjo.chain = New List(Of )(chain)
				Return sjo
			Catch ex As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError
			End Try
		End Function

		''' <summary>
		''' Registers the given warning.
		''' </summary>
		Private Property warning As javax.sql.rowset.RowSetWarning
			Set(ByVal e As javax.sql.rowset.RowSetWarning)
				If chain Is Nothing Then chain = New List(Of )
				chain.Add(e)
			End Set
		End Property

		''' <summary>
		''' readObject is called to restore the state of the {@code SerialJavaObject}
		''' from a stream.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)

			Dim fields1 As ObjectInputStream.GetField = s.readFields()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim tmp As List(Of javax.sql.rowset.RowSetWarning) = CType(fields1.get("chain", Nothing), List(Of javax.sql.rowset.RowSetWarning))
			If tmp IsNot Nothing Then chain = New List(Of )(tmp)

			obj = fields1.get("obj", Nothing)
			If obj IsNot Nothing Then
				fields = obj.GetType().GetFields()
				If hasStaticFields(fields) Then Throw New IOException("Located static fields in " & "object instance. Cannot serialize")
			Else
				Throw New IOException("Object cannot be null!")
			End If

		End Sub

		''' <summary>
		''' writeObject is called to save the state of the {@code SerialJavaObject}
		''' to a stream.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			Dim ___fields As ObjectOutputStream.PutField = s.putFields()
			___fields.put("obj", obj)
			___fields.put("chain", chain)
			s.writeFields()
		End Sub

	'    
	'     * Check to see if there are any Static Fields in this object
	'     
		Private Shared Function hasStaticFields(ByVal fields As Field()) As Boolean
			For Each field As Field In fields
				If field.modifiers = Modifier.STATIC Then Return True
			Next field
			Return False
		End Function
	End Class

End Namespace
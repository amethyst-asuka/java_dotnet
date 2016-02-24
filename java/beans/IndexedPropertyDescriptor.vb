Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	''' <summary>
	''' An IndexedPropertyDescriptor describes a property that acts like an
	''' array and has an indexed read and/or indexed write method to access
	''' specific elements of the array.
	''' <p>
	''' An indexed property may also provide simple non-indexed read and write
	''' methods.  If these are present, they read and write arrays of the type
	''' returned by the indexed read method.
	''' </summary>

	Public Class IndexedPropertyDescriptor
		Inherits PropertyDescriptor

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private indexedPropertyTypeRef As Reference(Of ? As Class)
		Private ReadOnly indexedReadMethodRef As New MethodRef
		Private ReadOnly indexedWriteMethodRef As New MethodRef

		Private indexedReadMethodName As String
		Private indexedWriteMethodName As String

		''' <summary>
		''' This constructor constructs an IndexedPropertyDescriptor for a property
		''' that follows the standard Java conventions by having getFoo and setFoo
		''' accessor methods, for both indexed access and array access.
		''' <p>
		''' Thus if the argument name is "fred", it will assume that there
		''' is an indexed reader method "getFred", a non-indexed (array) reader
		''' method also called "getFred", an indexed writer method "setFred",
		''' and finally a non-indexed writer method "setFred".
		''' </summary>
		''' <param name="propertyName"> The programmatic name of the property. </param>
		''' <param name="beanClass"> The Class object for the target bean. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(ByVal propertyName As String, ByVal beanClass As Class)
			Me.New(propertyName, beanClass, Introspector.GET_PREFIX + NameGenerator.capitalize(propertyName), Introspector.SET_PREFIX + NameGenerator.capitalize(propertyName), Introspector.GET_PREFIX + NameGenerator.capitalize(propertyName), Introspector.SET_PREFIX + NameGenerator.capitalize(propertyName))
		End Sub

		''' <summary>
		''' This constructor takes the name of a simple property, and method
		''' names for reading and writing the property, both indexed
		''' and non-indexed.
		''' </summary>
		''' <param name="propertyName"> The programmatic name of the property. </param>
		''' <param name="beanClass">  The Class object for the target bean. </param>
		''' <param name="readMethodName"> The name of the method used for reading the property
		'''           values as an array.  May be null if the property is write-only
		'''           or must be indexed. </param>
		''' <param name="writeMethodName"> The name of the method used for writing the property
		'''           values as an array.  May be null if the property is read-only
		'''           or must be indexed. </param>
		''' <param name="indexedReadMethodName"> The name of the method used for reading
		'''          an indexed property value.
		'''          May be null if the property is write-only. </param>
		''' <param name="indexedWriteMethodName"> The name of the method used for writing
		'''          an indexed property value.
		'''          May be null if the property is read-only. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(ByVal propertyName As String, ByVal beanClass As Class, ByVal readMethodName As String, ByVal writeMethodName As String, ByVal indexedReadMethodName As String, ByVal indexedWriteMethodName As String)
			MyBase.New(propertyName, beanClass, readMethodName, writeMethodName)

			Me.indexedReadMethodName = indexedReadMethodName
			If indexedReadMethodName IsNot Nothing AndAlso indexedReadMethod Is Nothing Then Throw New IntrospectionException("Method not found: " & indexedReadMethodName)

			Me.indexedWriteMethodName = indexedWriteMethodName
			If indexedWriteMethodName IsNot Nothing AndAlso indexedWriteMethod Is Nothing Then Throw New IntrospectionException("Method not found: " & indexedWriteMethodName)
			' Implemented only for type checking.
			findIndexedPropertyType(indexedReadMethod, indexedWriteMethod)
		End Sub

		''' <summary>
		''' This constructor takes the name of a simple property, and Method
		''' objects for reading and writing the property.
		''' </summary>
		''' <param name="propertyName"> The programmatic name of the property. </param>
		''' <param name="readMethod"> The method used for reading the property values as an array.
		'''          May be null if the property is write-only or must be indexed. </param>
		''' <param name="writeMethod"> The method used for writing the property values as an array.
		'''          May be null if the property is read-only or must be indexed. </param>
		''' <param name="indexedReadMethod"> The method used for reading an indexed property value.
		'''          May be null if the property is write-only. </param>
		''' <param name="indexedWriteMethod"> The method used for writing an indexed property value.
		'''          May be null if the property is read-only. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(ByVal propertyName As String, ByVal readMethod As Method, ByVal writeMethod As Method, ByVal indexedReadMethod As Method, ByVal indexedWriteMethod As Method)
			MyBase.New(propertyName, readMethod, writeMethod)

			indexedReadMethod0 = indexedReadMethod
			indexedWriteMethod0 = indexedWriteMethod

			' Type checking
			indexedPropertyType = findIndexedPropertyType(indexedReadMethod, indexedWriteMethod)
		End Sub

		''' <summary>
		''' Creates <code>PropertyDescriptor</code> for the specified bean
		''' with the specified name and methods to read/write the property value.
		''' </summary>
		''' <param name="bean">          the type of the target bean </param>
		''' <param name="base">          the base name of the property (the rest of the method name) </param>
		''' <param name="read">          the method used for reading the property value </param>
		''' <param name="write">         the method used for writing the property value </param>
		''' <param name="readIndexed">   the method used for reading an indexed property value </param>
		''' <param name="writeIndexed">  the method used for writing an indexed property value </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during introspection
		''' 
		''' @since 1.7 </exception>
		Friend Sub New(ByVal bean As Class, ByVal base As String, ByVal read As Method, ByVal write As Method, ByVal readIndexed As Method, ByVal writeIndexed As Method)
			MyBase.New(bean, base, read, write)

			indexedReadMethod0 = readIndexed
			indexedWriteMethod0 = writeIndexed

			' Type checking
			indexedPropertyType = findIndexedPropertyType(readIndexed, writeIndexed)
		End Sub

		''' <summary>
		''' Gets the method that should be used to read an indexed
		''' property value.
		''' </summary>
		''' <returns> The method that should be used to read an indexed
		''' property value.
		''' May return null if the property isn't indexed or is write-only. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property indexedReadMethod As Method
			Get
				Dim indexedReadMethod_Renamed As Method = Me.indexedReadMethodRef.get()
				If indexedReadMethod_Renamed Is Nothing Then
					Dim cls As Class = class0
					If cls Is Nothing OrElse (indexedReadMethodName Is Nothing AndAlso (Not Me.indexedReadMethodRef.set)) Then Return Nothing
					Dim nextMethodName As String = Introspector.GET_PREFIX + baseName
					If indexedReadMethodName Is Nothing Then
						Dim type As Class = indexedPropertyType0
						If type Is GetType(Boolean) OrElse type Is Nothing Then
							indexedReadMethodName = Introspector.IS_PREFIX + baseName
						Else
							indexedReadMethodName = nextMethodName
						End If
					End If
    
					Dim args As Class() = { GetType(Integer) }
					indexedReadMethod_Renamed = Introspector.findMethod(cls, indexedReadMethodName, 1, args)
					If (indexedReadMethod_Renamed Is Nothing) AndAlso (Not indexedReadMethodName.Equals(nextMethodName)) Then
						' no "is" method, so look for a "get" method.
						indexedReadMethodName = nextMethodName
						indexedReadMethod_Renamed = Introspector.findMethod(cls, indexedReadMethodName, 1, args)
					End If
					indexedReadMethod0 = indexedReadMethod_Renamed
				End If
				Return indexedReadMethod_Renamed
			End Get
			Set(ByVal readMethod As Method)
    
				' the indexed property type is set by the reader.
				indexedPropertyType = findIndexedPropertyType(readMethod, Me.indexedWriteMethodRef.get())
				indexedReadMethod0 = readMethod
			End Set
		End Property


		Private Property indexedReadMethod0 As Method
			Set(ByVal readMethod As Method)
				Me.indexedReadMethodRef.set(readMethod)
				If readMethod Is Nothing Then
					indexedReadMethodName = Nothing
					Return
				End If
				class0 = readMethod.declaringClass
    
				indexedReadMethodName = readMethod.name
				transient = readMethod.getAnnotation(GetType(Transient))
			End Set
		End Property


		''' <summary>
		''' Gets the method that should be used to write an indexed property value.
		''' </summary>
		''' <returns> The method that should be used to write an indexed
		''' property value.
		''' May return null if the property isn't indexed or is read-only. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property indexedWriteMethod As Method
			Get
				Dim indexedWriteMethod_Renamed As Method = Me.indexedWriteMethodRef.get()
				If indexedWriteMethod_Renamed Is Nothing Then
					Dim cls As Class = class0
					If cls Is Nothing OrElse (indexedWriteMethodName Is Nothing AndAlso (Not Me.indexedWriteMethodRef.set)) Then Return Nothing
    
					' We need the indexed type to ensure that we get the correct method.
					' Cannot use the getIndexedPropertyType method since that could
					' result in an infinite loop.
					Dim type As Class = indexedPropertyType0
					If type Is Nothing Then
						Try
							type = findIndexedPropertyType(indexedReadMethod, Nothing)
							indexedPropertyType = type
						Catch ex As IntrospectionException
							' Set iprop type to be the classic type
							Dim propType As Class = propertyType
							If propType.array Then type = propType.componentType
						End Try
					End If
    
					If indexedWriteMethodName Is Nothing Then indexedWriteMethodName = Introspector.SET_PREFIX + baseName
    
					Dim args As Class() = If(type Is Nothing, Nothing, New [Class]()){ GetType(Integer), type }
					indexedWriteMethod_Renamed = Introspector.findMethod(cls, indexedWriteMethodName, 2, args)
					If indexedWriteMethod_Renamed IsNot Nothing Then
						If Not indexedWriteMethod_Renamed.returnType.Equals(GetType(void)) Then indexedWriteMethod_Renamed = Nothing
					End If
					indexedWriteMethod0 = indexedWriteMethod_Renamed
				End If
				Return indexedWriteMethod_Renamed
			End Get
			Set(ByVal writeMethod As Method)
    
				' If the indexed property type has not been set, then set it.
				Dim type As Class = findIndexedPropertyType(indexedReadMethod, writeMethod)
				indexedPropertyType = type
				indexedWriteMethod0 = writeMethod
			End Set
		End Property


		Private Property indexedWriteMethod0 As Method
			Set(ByVal writeMethod As Method)
				Me.indexedWriteMethodRef.set(writeMethod)
				If writeMethod Is Nothing Then
					indexedWriteMethodName = Nothing
					Return
				End If
				class0 = writeMethod.declaringClass
    
				indexedWriteMethodName = writeMethod.name
				transient = writeMethod.getAnnotation(GetType(Transient))
			End Set
		End Property

		''' <summary>
		''' Returns the Java type info for the indexed property.
		''' Note that the {@code Class} object may describe
		''' primitive Java types such as {@code int}.
		''' This type is returned by the indexed read method
		''' or is used as the parameter type of the indexed write method.
		''' </summary>
		''' <returns> the {@code Class} object that represents the Java type info,
		'''         or {@code null} if the type cannot be determined </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property indexedPropertyType As Class
			Get
				Dim type As Class = indexedPropertyType0
				If type Is Nothing Then
					Try
						type = findIndexedPropertyType(indexedReadMethod, indexedWriteMethod)
						indexedPropertyType = type
					Catch ex As IntrospectionException
						' fall
					End Try
				End If
				Return type
			End Get
			Set(ByVal type As Class)
				Me.indexedPropertyTypeRef = getWeakReference(type)
			End Set
		End Property

		' Private methods which set get/set the Reference objects


		Private Property indexedPropertyType0 As Class
			Get
				Return If(Me.indexedPropertyTypeRef IsNot Nothing, Me.indexedPropertyTypeRef.get(), Nothing)
			End Get
		End Property

		Private Function findIndexedPropertyType(ByVal indexedReadMethod As Method, ByVal indexedWriteMethod As Method) As Class
			Dim indexedPropertyType_Renamed As Class = Nothing

			If indexedReadMethod IsNot Nothing Then
				Dim params As Class() = getParameterTypes(class0, indexedReadMethod)
				If params.Length <> 1 Then Throw New IntrospectionException("bad indexed read method arg count")
				If params(0) IsNot Integer.TYPE Then Throw New IntrospectionException("non int index to indexed read method")
				indexedPropertyType_Renamed = getReturnType(class0, indexedReadMethod)
				If indexedPropertyType_Renamed Is Void.TYPE Then Throw New IntrospectionException("indexed read method returns void")
			End If
			If indexedWriteMethod IsNot Nothing Then
				Dim params As Class() = getParameterTypes(class0, indexedWriteMethod)
				If params.Length <> 2 Then Throw New IntrospectionException("bad indexed write method arg count")
				If params(0) IsNot Integer.TYPE Then Throw New IntrospectionException("non int index to indexed write method")
				If indexedPropertyType_Renamed Is Nothing OrElse indexedPropertyType_Renamed.IsSubclassOf(params(1)) Then
					indexedPropertyType_Renamed = params(1)
				ElseIf Not params(1).IsSubclassOf(indexedPropertyType_Renamed) Then
					Throw New IntrospectionException("type mismatch between indexed read and indexed write methods: " & name)
				End If
			End If
			Dim propertyType_Renamed As Class = propertyType
			If propertyType_Renamed IsNot Nothing AndAlso ((Not propertyType_Renamed.array) OrElse propertyType_Renamed.componentType IsNot indexedPropertyType_Renamed) Then Throw New IntrospectionException("type mismatch between indexed and non-indexed methods: " & name)
			Return indexedPropertyType_Renamed
		End Function

		''' <summary>
		''' Compares this <code>PropertyDescriptor</code> against the specified object.
		''' Returns true if the objects are the same. Two <code>PropertyDescriptor</code>s
		''' are the same if the read, write, property types, property editor and
		''' flags  are equivalent.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			' Note: This would be identical to PropertyDescriptor but they don't
			' share the same fields.
			If Me Is obj Then Return True

			If obj IsNot Nothing AndAlso TypeOf obj Is IndexedPropertyDescriptor Then
				Dim other As IndexedPropertyDescriptor = CType(obj, IndexedPropertyDescriptor)
				Dim otherIndexedReadMethod As Method = other.indexedReadMethod
				Dim otherIndexedWriteMethod As Method = other.indexedWriteMethod

				If Not compareMethods(indexedReadMethod, otherIndexedReadMethod) Then Return False

				If Not compareMethods(indexedWriteMethod, otherIndexedWriteMethod) Then Return False

				If indexedPropertyType IsNot other.indexedPropertyType Then Return False
				Return MyBase.Equals(obj)
			End If
			Return False
		End Function

		''' <summary>
		''' Package-private constructor.
		''' Merge two property descriptors.  Where they conflict, give the
		''' second argument (y) priority over the first argumnnt (x).
		''' </summary>
		''' <param name="x">  The first (lower priority) PropertyDescriptor </param>
		''' <param name="y">  The second (higher priority) PropertyDescriptor </param>

		Friend Sub New(ByVal x As PropertyDescriptor, ByVal y As PropertyDescriptor)
			MyBase.New(x,y)
			If TypeOf x Is IndexedPropertyDescriptor Then
				Dim ix As IndexedPropertyDescriptor = CType(x, IndexedPropertyDescriptor)
				Try
					Dim xr As Method = ix.indexedReadMethod
					If xr IsNot Nothing Then indexedReadMethod = xr

					Dim xw As Method = ix.indexedWriteMethod
					If xw IsNot Nothing Then indexedWriteMethod = xw
				Catch ex As IntrospectionException
					' Should not happen
					Throw New AssertionError(ex)
				End Try
			End If
			If TypeOf y Is IndexedPropertyDescriptor Then
				Dim iy As IndexedPropertyDescriptor = CType(y, IndexedPropertyDescriptor)
				Try
					Dim yr As Method = iy.indexedReadMethod
					If yr IsNot Nothing AndAlso yr.declaringClass Is class0 Then indexedReadMethod = yr

					Dim yw As Method = iy.indexedWriteMethod
					If yw IsNot Nothing AndAlso yw.declaringClass Is class0 Then indexedWriteMethod = yw
				Catch ex As IntrospectionException
					' Should not happen
					Throw New AssertionError(ex)
				End Try
			End If
		End Sub

	'    
	'     * Package-private dup constructor
	'     * This must isolate the new object from any changes to the old object.
	'     
		Friend Sub New(ByVal old As IndexedPropertyDescriptor)
			MyBase.New(old)
			Me.indexedReadMethodRef.set(old.indexedReadMethodRef.get())
			Me.indexedWriteMethodRef.set(old.indexedWriteMethodRef.get())
			indexedPropertyTypeRef = old.indexedPropertyTypeRef
			indexedWriteMethodName = old.indexedWriteMethodName
			indexedReadMethodName = old.indexedReadMethodName
		End Sub

		Friend Overrides Sub updateGenericsFor(ByVal type As Class)
			MyBase.updateGenericsFor(type)
			Try
				indexedPropertyType = findIndexedPropertyType(Me.indexedReadMethodRef.get(), Me.indexedWriteMethodRef.get())
			Catch exception_Renamed As IntrospectionException
				indexedPropertyType = Nothing
			End Try
		End Sub

		''' <summary>
		''' Returns a hash code value for the object.
		''' See <seealso cref="java.lang.Object#hashCode"/> for a complete description.
		''' </summary>
		''' <returns> a hash code value for this object.
		''' @since 1.5 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()

			result = 37 * result + (If(indexedWriteMethodName Is Nothing, 0, indexedWriteMethodName.GetHashCode()))
			result = 37 * result + (If(indexedReadMethodName Is Nothing, 0, indexedReadMethodName.GetHashCode()))
			result = 37 * result + (If(indexedPropertyType Is Nothing, 0, indexedPropertyType.GetHashCode()))

			Return result
		End Function

		Friend Overrides Sub appendTo(ByVal sb As StringBuilder)
			MyBase.appendTo(sb)
			appendTo(sb, "indexedPropertyType", Me.indexedPropertyTypeRef)
			appendTo(sb, "indexedReadMethod", Me.indexedReadMethodRef.get())
			appendTo(sb, "indexedWriteMethod", Me.indexedWriteMethodRef.get())
		End Sub
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' A PropertyDescriptor describes one property that a Java Bean
	''' exports via a pair of accessor methods.
	''' </summary>
	Public Class PropertyDescriptor
		Inherits FeatureDescriptor

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private propertyTypeRef As Reference(Of ? As [Class])
		Private ReadOnly readMethodRef As New MethodRef
		Private ReadOnly writeMethodRef As New MethodRef
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private propertyEditorClassRef As Reference(Of ? As [Class])

		Private bound As Boolean
		Private constrained As Boolean

		' The base name of the method name which will be prefixed with the
		' read and write method. If name == "foo" then the baseName is "Foo"
		Private baseName As String

		Private writeMethodName As String
		Private readMethodName As String

		''' <summary>
		''' Constructs a PropertyDescriptor for a property that follows
		''' the standard Java convention by having getFoo and setFoo
		''' accessor methods.  Thus if the argument name is "fred", it will
		''' assume that the writer method is "setFred" and the reader method
		''' is "getFred" (or "isFred" for a boolean property).  Note that the
		''' property name should start with a lower case character, which will
		''' be capitalized in the method names.
		''' </summary>
		''' <param name="propertyName"> The programmatic name of the property. </param>
		''' <param name="beanClass"> The Class object for the target bean.  For
		'''          example sun.beans.OurButton.class. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(  propertyName As String,   beanClass As [Class])
			Me.New(propertyName, beanClass, Introspector.IS_PREFIX + NameGenerator.capitalize(propertyName), Introspector.SET_PREFIX + NameGenerator.capitalize(propertyName))
		End Sub

		''' <summary>
		''' This constructor takes the name of a simple property, and method
		''' names for reading and writing the property.
		''' </summary>
		''' <param name="propertyName"> The programmatic name of the property. </param>
		''' <param name="beanClass"> The Class object for the target bean.  For
		'''          example sun.beans.OurButton.class. </param>
		''' <param name="readMethodName"> The name of the method used for reading the property
		'''           value.  May be null if the property is write-only. </param>
		''' <param name="writeMethodName"> The name of the method used for writing the property
		'''           value.  May be null if the property is read-only. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(  propertyName As String,   beanClass As [Class],   readMethodName As String,   writeMethodName As String)
			If beanClass Is Nothing Then Throw New IntrospectionException("Target Bean class is null")
			If propertyName Is Nothing OrElse propertyName.length() = 0 Then Throw New IntrospectionException("bad property name")
			If "".Equals(readMethodName) OrElse "".Equals(writeMethodName) Then Throw New IntrospectionException("read or write method name should not be the empty string")
			name = propertyName
			class0 = beanClass

			Me.readMethodName = readMethodName
			If readMethodName IsNot Nothing AndAlso readMethod Is Nothing Then Throw New IntrospectionException("Method not found: " & readMethodName)
			Me.writeMethodName = writeMethodName
			If writeMethodName IsNot Nothing AndAlso writeMethod Is Nothing Then Throw New IntrospectionException("Method not found: " & writeMethodName)
			' If this class or one of its base classes allow PropertyChangeListener,
			' then we assume that any properties we discover are "bound".
			' See Introspector.getTargetPropertyInfo() method.
			Dim args As  [Class]() = { GetType(PropertyChangeListener) }
			Me.bound = Nothing IsNot Introspector.findMethod(beanClass, "addPropertyChangeListener", args.Length, args)
		End Sub

		''' <summary>
		''' This constructor takes the name of a simple property, and Method
		''' objects for reading and writing the property.
		''' </summary>
		''' <param name="propertyName"> The programmatic name of the property. </param>
		''' <param name="readMethod"> The method used for reading the property value.
		'''          May be null if the property is write-only. </param>
		''' <param name="writeMethod"> The method used for writing the property value.
		'''          May be null if the property is read-only. </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during
		'''              introspection. </exception>
		Public Sub New(  propertyName As String,   readMethod As Method,   writeMethod As Method)
			If propertyName Is Nothing OrElse propertyName.length() = 0 Then Throw New IntrospectionException("bad property name")
			name = propertyName
			readMethod = readMethod
			writeMethod = writeMethod
		End Sub

		''' <summary>
		''' Creates <code>PropertyDescriptor</code> for the specified bean
		''' with the specified name and methods to read/write the property value.
		''' </summary>
		''' <param name="bean">   the type of the target bean </param>
		''' <param name="base">   the base name of the property (the rest of the method name) </param>
		''' <param name="read">   the method used for reading the property value </param>
		''' <param name="write">  the method used for writing the property value </param>
		''' <exception cref="IntrospectionException"> if an exception occurs during introspection
		''' 
		''' @since 1.7 </exception>
		Friend Sub New(  bean As [Class],   base As String,   read As Method,   write As Method)
			If bean Is Nothing Then Throw New IntrospectionException("Target Bean class is null")
			class0 = bean
			name = Introspector.decapitalize(base)
			readMethod = read
			writeMethod = write
			Me.baseName = base
		End Sub

		''' <summary>
		''' Returns the Java type info for the property.
		''' Note that the {@code Class} object may describe
		''' primitive Java types such as {@code int}.
		''' This type is returned by the read method
		''' or is used as the parameter type of the write method.
		''' Returns {@code null} if the type is an indexed property
		''' that does not support non-indexed access.
		''' </summary>
		''' <returns> the {@code Class} object that represents the Java type info,
		'''         or {@code null} if the type cannot be determined </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property propertyType As  [Class]
			Get
				Dim type As  [Class] = propertyType0
				If type Is Nothing Then
					Try
						type = findPropertyType(readMethod, writeMethod)
						propertyType = type
					Catch ex As IntrospectionException
						' Fall
					End Try
				End If
				Return type
			End Get
			Set(  type As [Class])
				Me.propertyTypeRef = getWeakReference(type)
			End Set
		End Property


		Private Property propertyType0 As  [Class]
			Get
				Return If(Me.propertyTypeRef IsNot Nothing, Me.propertyTypeRef.get(), Nothing)
			End Get
		End Property

		''' <summary>
		''' Gets the method that should be used to read the property value.
		''' </summary>
		''' <returns> The method that should be used to read the property value.
		''' May return null if the property can't be read. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property readMethod As Method
			Get
				Dim readMethod_Renamed As Method = Me.readMethodRef.get()
				If readMethod_Renamed Is Nothing Then
					Dim cls As  [Class] = class0
					If cls Is Nothing OrElse (readMethodName Is Nothing AndAlso (Not Me.readMethodRef.set)) Then Return Nothing
					Dim nextMethodName As String = Introspector.GET_PREFIX + baseName
					If readMethodName Is Nothing Then
						Dim type As  [Class] = propertyType0
						If type Is GetType(Boolean) OrElse type Is Nothing Then
							readMethodName = Introspector.IS_PREFIX + baseName
						Else
							readMethodName = nextMethodName
						End If
					End If
    
					' Since there can be multiple write methods but only one getter
					' method, find the getter method first so that you know what the
					' property type is.  For booleans, there can be "is" and "get"
					' methods.  If an "is" method exists, this is the official
					' reader method so look for this one first.
					readMethod_Renamed = Introspector.findMethod(cls, readMethodName, 0)
					If (readMethod_Renamed Is Nothing) AndAlso (Not readMethodName.Equals(nextMethodName)) Then
						readMethodName = nextMethodName
						readMethod_Renamed = Introspector.findMethod(cls, readMethodName, 0)
					End If
					Try
						readMethod = readMethod_Renamed
					Catch ex As IntrospectionException
						' fall
					End Try
				End If
				Return readMethod_Renamed
			End Get
			Set(  readMethod As Method)
				Me.readMethodRef.set(readMethod)
				If readMethod Is Nothing Then
					readMethodName = Nothing
					Return
				End If
				' The property type is determined by the read method.
				propertyType = findPropertyType(readMethod, Me.writeMethodRef.get())
				class0 = readMethod.declaringClass
    
				readMethodName = readMethod.name
				transient = readMethod.getAnnotation(GetType(Transient))
			End Set
		End Property


		''' <summary>
		''' Gets the method that should be used to write the property value.
		''' </summary>
		''' <returns> The method that should be used to write the property value.
		''' May return null if the property can't be written. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property writeMethod As Method
			Get
				Dim writeMethod_Renamed As Method = Me.writeMethodRef.get()
				If writeMethod_Renamed Is Nothing Then
					Dim cls As  [Class] = class0
					If cls Is Nothing OrElse (writeMethodName Is Nothing AndAlso (Not Me.writeMethodRef.set)) Then Return Nothing
    
					' We need the type to fetch the correct method.
					Dim type As  [Class] = propertyType0
					If type Is Nothing Then
						Try
							' Can't use getPropertyType since it will lead to recursive loop.
							type = findPropertyType(readMethod, Nothing)
							propertyType = type
						Catch ex As IntrospectionException
							' Without the correct property type we can't be guaranteed
							' to find the correct method.
							Return Nothing
						End Try
					End If
    
					If writeMethodName Is Nothing Then writeMethodName = Introspector.SET_PREFIX + baseName
    
					Dim args As  [Class]() = If(type Is Nothing, Nothing, New [Class]()){ type }
					writeMethod_Renamed = Introspector.findMethod(cls, writeMethodName, 1, args)
					If writeMethod_Renamed IsNot Nothing Then
						If Not writeMethod_Renamed.returnType.Equals(GetType(void)) Then writeMethod_Renamed = Nothing
					End If
					Try
						writeMethod = writeMethod_Renamed
					Catch ex As IntrospectionException
						' fall through
					End Try
				End If
				Return writeMethod_Renamed
			End Get
			Set(  writeMethod As Method)
				Me.writeMethodRef.set(writeMethod)
				If writeMethod Is Nothing Then
					writeMethodName = Nothing
					Return
				End If
				' Set the property type - which validates the method
				propertyType = findPropertyType(readMethod, writeMethod)
				class0 = writeMethod.declaringClass
    
				writeMethodName = writeMethod.name
				transient = writeMethod.getAnnotation(GetType(Transient))
			End Set
		End Property


		''' <summary>
		''' Overridden to ensure that a super class doesn't take precedent
		''' </summary>
		Friend Overrides Property class0 As  [Class]
			Set(  clz As [Class])
				If class0 IsNot Nothing AndAlso class0.IsSubclassOf(clz) Then Return
				MyBase.class0 = clz
			End Set
		End Property

		''' <summary>
		''' Updates to "bound" properties will cause a "PropertyChange" event to
		''' get fired when the property is changed.
		''' </summary>
		''' <returns> True if this is a bound property. </returns>
		Public Overridable Property bound As Boolean
			Get
				Return bound
			End Get
			Set(  bound As Boolean)
				Me.bound = bound
			End Set
		End Property


		''' <summary>
		''' Attempted updates to "Constrained" properties will cause a "VetoableChange"
		''' event to get fired when the property is changed.
		''' </summary>
		''' <returns> True if this is a constrained property. </returns>
		Public Overridable Property constrained As Boolean
			Get
				Return constrained
			End Get
			Set(  constrained As Boolean)
				Me.constrained = constrained
			End Set
		End Property



		''' <summary>
		''' Normally PropertyEditors will be found using the PropertyEditorManager.
		''' However if for some reason you want to associate a particular
		''' PropertyEditor with a given property, then you can do it with
		''' this method.
		''' </summary>
		''' <param name="propertyEditorClass">  The Class for the desired PropertyEditor. </param>
		Public Overridable Property propertyEditorClass As  [Class]
			Set(  propertyEditorClass As [Class])
				Me.propertyEditorClassRef = getWeakReference(propertyEditorClass)
			End Set
			Get
				Return If(Me.propertyEditorClassRef IsNot Nothing, Me.propertyEditorClassRef.get(), Nothing)
			End Get
		End Property


		''' <summary>
		''' Constructs an instance of a property editor using the current
		''' property editor class.
		''' <p>
		''' If the property editor class has a public constructor that takes an
		''' Object argument then it will be invoked using the bean parameter
		''' as the argument. Otherwise, the default constructor will be invoked.
		''' </summary>
		''' <param name="bean"> the source object </param>
		''' <returns> a property editor instance or null if a property editor has
		'''         not been defined or cannot be created
		''' @since 1.5 </returns>
		Public Overridable Function createPropertyEditor(  bean As Object) As PropertyEditor
			Dim editor As Object = Nothing

			Dim cls As  [Class] = propertyEditorClass
			If cls IsNot Nothing AndAlso cls.IsSubclassOf(GetType(PropertyEditor)) AndAlso sun.reflect.misc.ReflectUtil.isPackageAccessible(cls) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ctor As Constructor(Of ?) = Nothing
				If bean IsNot Nothing Then
					Try
						ctor = cls.getConstructor(New [Class]() { GetType(Object) })
					Catch ex As Exception
						' Fall through
					End Try
				End If
				Try
					If ctor Is Nothing Then
						editor = cls.newInstance()
					Else
						editor = ctor.newInstance(New Object() { bean })
					End If
				Catch ex As Exception
					' Fall through
				End Try
			End If
			Return CType(editor, PropertyEditor)
		End Function


		''' <summary>
		''' Compares this <code>PropertyDescriptor</code> against the specified object.
		''' Returns true if the objects are the same. Two <code>PropertyDescriptor</code>s
		''' are the same if the read, write, property types, property editor and
		''' flags  are equivalent.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If obj IsNot Nothing AndAlso TypeOf obj Is PropertyDescriptor Then
				Dim other As PropertyDescriptor = CType(obj, PropertyDescriptor)
				Dim otherReadMethod As Method = other.readMethod
				Dim otherWriteMethod As Method = other.writeMethod

				If Not compareMethods(readMethod, otherReadMethod) Then Return False

				If Not compareMethods(writeMethod, otherWriteMethod) Then Return False

				If propertyType Is other.propertyType AndAlso propertyEditorClass Is other.propertyEditorClass AndAlso bound = other.bound AndAlso constrained = other.constrained AndAlso writeMethodName = other.writeMethodName AndAlso readMethodName = other.readMethodName Then Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Package private helper method for Descriptor .equals methods.
		''' </summary>
		''' <param name="a"> first method to compare </param>
		''' <param name="b"> second method to compare </param>
		''' <returns> boolean to indicate that the methods are equivalent </returns>
		Friend Overridable Function compareMethods(  a As Method,   b As Method) As Boolean
			' Note: perhaps this should be a protected method in FeatureDescriptor
			If (a Is Nothing) <> (b Is Nothing) Then Return False

			If a IsNot Nothing AndAlso b IsNot Nothing Then
				If Not a.Equals(b) Then Return False
			End If
			Return True
		End Function

		''' <summary>
		''' Package-private constructor.
		''' Merge two property descriptors.  Where they conflict, give the
		''' second argument (y) priority over the first argument (x).
		''' </summary>
		''' <param name="x">  The first (lower priority) PropertyDescriptor </param>
		''' <param name="y">  The second (higher priority) PropertyDescriptor </param>
		Friend Sub New(  x As PropertyDescriptor,   y As PropertyDescriptor)
			MyBase.New(x,y)

			If y.baseName IsNot Nothing Then
				baseName = y.baseName
			Else
				baseName = x.baseName
			End If

			If y.readMethodName IsNot Nothing Then
				readMethodName = y.readMethodName
			Else
				readMethodName = x.readMethodName
			End If

			If y.writeMethodName IsNot Nothing Then
				writeMethodName = y.writeMethodName
			Else
				writeMethodName = x.writeMethodName
			End If

			If y.propertyTypeRef IsNot Nothing Then
				propertyTypeRef = y.propertyTypeRef
			Else
				propertyTypeRef = x.propertyTypeRef
			End If

			' Figure out the merged read method.
			Dim xr As Method = x.readMethod
			Dim yr As Method = y.readMethod

			' Normally give priority to y's readMethod.
			Try
				If isAssignable(xr, yr) Then
					readMethod = yr
				Else
					readMethod = xr
				End If
			Catch ex As IntrospectionException
				' fall through
			End Try

			' However, if both x and y reference read methods in the same [Class],
			' give priority to a boolean "is" method over a boolean "get" method.
			If xr IsNot Nothing AndAlso yr IsNot Nothing AndAlso xr.declaringClass = yr.declaringClass AndAlso getReturnType(class0, xr) Is GetType(Boolean) AndAlso getReturnType(class0, yr) Is GetType(Boolean) AndAlso xr.name.IndexOf(Introspector.IS_PREFIX) = 0 AndAlso yr.name.IndexOf(Introspector.GET_PREFIX) = 0 Then
				Try
					readMethod = xr
				Catch ex As IntrospectionException
					' fall through
				End Try
			End If

			Dim xw As Method = x.writeMethod
			Dim yw As Method = y.writeMethod

			Try
				If yw IsNot Nothing Then
					writeMethod = yw
				Else
					writeMethod = xw
				End If
			Catch ex As IntrospectionException
				' Fall through
			End Try

			If y.propertyEditorClass IsNot Nothing Then
				propertyEditorClass = y.propertyEditorClass
			Else
				propertyEditorClass = x.propertyEditorClass
			End If


			bound = x.bound Or y.bound
			constrained = x.constrained Or y.constrained
		End Sub

	'    
	'     * Package-private dup constructor.
	'     * This must isolate the new object from any changes to the old object.
	'     
		Friend Sub New(  old As PropertyDescriptor)
			MyBase.New(old)
			propertyTypeRef = old.propertyTypeRef
			Me.readMethodRef.set(old.readMethodRef.get())
			Me.writeMethodRef.set(old.writeMethodRef.get())
			propertyEditorClassRef = old.propertyEditorClassRef

			writeMethodName = old.writeMethodName
			readMethodName = old.readMethodName
			baseName = old.baseName

			bound = old.bound
			constrained = old.constrained
		End Sub

		Friend Overridable Sub updateGenericsFor(  type As [Class])
			class0 = type
			Try
				propertyType = findPropertyType(Me.readMethodRef.get(), Me.writeMethodRef.get())
			Catch exception_Renamed As IntrospectionException
				propertyType = Nothing
			End Try
		End Sub

		''' <summary>
		''' Returns the property type that corresponds to the read and write method.
		''' The type precedence is given to the readMethod.
		''' </summary>
		''' <returns> the type of the property descriptor or null if both
		'''         read and write methods are null. </returns>
		''' <exception cref="IntrospectionException"> if the read or write method is invalid </exception>
		Private Function findPropertyType(  readMethod As Method,   writeMethod As Method) As  [Class]
			Dim propertyType_Renamed As  [Class] = Nothing
			Try
				If readMethod IsNot Nothing Then
					Dim params As  [Class]() = getParameterTypes(class0, readMethod)
					If params.Length <> 0 Then Throw New IntrospectionException("bad read method arg count: " & readMethod)
					propertyType_Renamed = getReturnType(class0, readMethod)
					If propertyType_Renamed Is Void.TYPE Then Throw New IntrospectionException("read method " & readMethod.name & " returns void")
				End If
				If writeMethod IsNot Nothing Then
					Dim params As  [Class]() = getParameterTypes(class0, writeMethod)
					If params.Length <> 1 Then Throw New IntrospectionException("bad write method arg count: " & writeMethod)
					If propertyType_Renamed IsNot Nothing AndAlso (Not propertyType_Renamed.IsSubclassOf(params(0))) Then Throw New IntrospectionException("type mismatch between read and write methods")
					propertyType_Renamed = params(0)
				End If
			Catch ex As IntrospectionException
				Throw ex
			End Try
			Return propertyType_Renamed
		End Function


		''' <summary>
		''' Returns a hash code value for the object.
		''' See <seealso cref="java.lang.Object#hashCode"/> for a complete description.
		''' </summary>
		''' <returns> a hash code value for this object.
		''' @since 1.5 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 7

			result = 37 * result + (If(propertyType Is Nothing, 0, propertyType.GetHashCode()))
			result = 37 * result + (If(readMethod Is Nothing, 0, readMethod.GetHashCode()))
			result = 37 * result + (If(writeMethod Is Nothing, 0, writeMethod.GetHashCode()))
			result = 37 * result + (If(propertyEditorClass Is Nothing, 0, propertyEditorClass.GetHashCode()))
			result = 37 * result + (If(writeMethodName Is Nothing, 0, writeMethodName.GetHashCode()))
			result = 37 * result + (If(readMethodName Is Nothing, 0, readMethodName.GetHashCode()))
			result = 37 * result + name.GetHashCode()
			result = 37 * result + (If(bound = False, 0, 1))
			result = 37 * result + (If(constrained = False, 0, 1))

			Return result
		End Function

		' Calculate once since capitalize() is expensive.
		Friend Overridable Property baseName As String
			Get
				If baseName Is Nothing Then baseName = NameGenerator.capitalize(name)
				Return baseName
			End Get
		End Property

		Friend Overrides Sub appendTo(  sb As StringBuilder)
			appendTo(sb, "bound", Me.bound)
			appendTo(sb, "constrained", Me.constrained)
			appendTo(sb, "propertyEditorClass", Me.propertyEditorClassRef)
			appendTo(sb, "propertyType", Me.propertyTypeRef)
			appendTo(sb, "readMethod", Me.readMethodRef.get())
			appendTo(sb, "writeMethod", Me.writeMethodRef.get())
		End Sub

		Private Function isAssignable(  m1 As Method,   m2 As Method) As Boolean
			If m1 Is Nothing Then Return True ' choose second method
			If m2 Is Nothing Then Return False ' choose first method
			If Not m1.name.Equals(m2.name) Then Return True ' choose second method by default
			Dim type1 As  [Class] = m1.declaringClass
			Dim type2 As  [Class] = m2.declaringClass
			If Not type2.IsSubclassOf(type1) Then Return False ' choose first method: it declared later
			type1 = getReturnType(class0, m1)
			type2 = getReturnType(class0, m2)
			If Not type2.IsSubclassOf(type1) Then Return False ' choose first method: it overrides return type
			Dim args1 As  [Class]() = getParameterTypes(class0, m1)
			Dim args2 As  [Class]() = getParameterTypes(class0, m2)
			If args1.Length <> args2.Length Then Return True ' choose second method by default
			For i As Integer = 0 To args1.Length - 1
				If Not args2(i).IsSubclassOf(args1(i)) Then Return False ' choose first method: it overrides parameter
			Next i
			Return True ' choose second method
		End Function
	End Class

End Namespace
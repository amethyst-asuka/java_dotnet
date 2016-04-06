Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

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

Namespace java.lang.reflect


	''' <summary>
	''' A {@code Field} provides information about, and dynamic access to, a
	''' single field of a class or an interface.  The reflected field may
	''' be a class (static) field or an instance field.
	''' 
	''' <p>A {@code Field} permits widening conversions to occur during a get or
	''' set access operation, but throws an {@code IllegalArgumentException} if a
	''' narrowing conversion would occur.
	''' </summary>
	''' <seealso cref= Member </seealso>
	''' <seealso cref= java.lang.Class </seealso>
	''' <seealso cref= java.lang.Class#getFields() </seealso>
	''' <seealso cref= java.lang.Class#getField(String) </seealso>
	''' <seealso cref= java.lang.Class#getDeclaredFields() </seealso>
	''' <seealso cref= java.lang.Class#getDeclaredField(String)
	''' 
	''' @author Kenneth Russell
	''' @author Nakul Saraiya </seealso>
	Public NotInheritable Class Field
		Inherits AccessibleObject
		Implements Member

		Private clazz As  [Class]
		Private slot As Integer
		' This is guaranteed to be interned by the VM in the 1.4
		' reflection implementation
		Private name As String
		Private type As  [Class]
		Private modifiers As Integer
		' Generics and annotations support
		<NonSerialized> _
		Private signature As String
		' generic info repository; lazily initialized
		<NonSerialized> _
		Private genericInfo As sun.reflect.generics.repository.FieldRepository
		Private annotations As SByte()
		' Cached field accessor created without override
		Private fieldAccessor As sun.reflect.FieldAccessor
		' Cached field accessor created with override
		Private overrideFieldAccessor As sun.reflect.FieldAccessor
		' For sharing of FieldAccessors. This branching structure is
		' currently only two levels deep (i.e., one root Field and
		' potentially many Field objects pointing to it.)
		'
		' If this branching structure would ever contain cycles, deadlocks can
		' occur in annotation code.
		Private root As Field

		' Generics infrastructure

		Private Property genericSignature As String
			Get
				Return signature
			End Get
		End Property

		' Accessor for factory
		Private Property factory As sun.reflect.generics.factory.GenericsFactory
			Get
				Dim c As  [Class] = declaringClass
				' create scope and factory
				Return sun.reflect.generics.factory.CoreReflectionFactory.make(c, sun.reflect.generics.scope.ClassScope.make(c))
			End Get
		End Property

		' Accessor for generic info repository
		Private Property genericInfo As sun.reflect.generics.repository.FieldRepository
			Get
				' lazily initialize repository if necessary
				If genericInfo Is Nothing Then genericInfo = sun.reflect.generics.repository.FieldRepository.make(genericSignature, factory)
				Return genericInfo 'return cached repository
			End Get
		End Property


		''' <summary>
		''' Package-private constructor used by ReflectAccess to enable
		''' instantiation of these objects in Java code from the java.lang
		''' package via sun.reflect.LangReflectAccess.
		''' </summary>
		Friend Sub New(  declaringClass As [Class],   name As String,   type As [Class],   modifiers As Integer,   slot As Integer,   signature As String,   annotations As SByte())
			Me.clazz = declaringClass
			Me.name = name
			Me.type = type
			Me.modifiers = modifiers
			Me.slot = slot
			Me.signature = signature
			Me.annotations = annotations
		End Sub

		''' <summary>
		''' Package-private routine (exposed to java.lang.Class via
		''' ReflectAccess) which returns a copy of this Field. The copy's
		''' "root" field points to this Field.
		''' </summary>
		Friend Function copy() As Field
			' This routine enables sharing of FieldAccessor objects
			' among Field objects which refer to the same underlying
			' method in the VM. (All of this contortion is only necessary
			' because of the "accessibility" bit in AccessibleObject,
			' which implicitly requires that new java.lang.reflect
			' objects be fabricated for each reflective call on Class
			' objects.)
			If Me.root IsNot Nothing Then Throw New IllegalArgumentException("Can not copy a non-root Field")

			Dim res As New Field(clazz, name, type, modifiers, slot, signature, annotations)
			res.root = Me
			' Might as well eagerly propagate this if already present
			res.fieldAccessor = fieldAccessor
			res.overrideFieldAccessor = overrideFieldAccessor

			Return res
		End Function

		''' <summary>
		''' Returns the {@code Class} object representing the class or interface
		''' that declares the field represented by this {@code Field} object.
		''' </summary>
		Public Property declaringClass As  [Class] Implements Member.getDeclaringClass
			Get
				Return clazz
			End Get
		End Property

		''' <summary>
		''' Returns the name of the field represented by this {@code Field} object.
		''' </summary>
		Public Property name As String Implements Member.getName
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the Java language modifiers for the field represented
		''' by this {@code Field} object, as an  java.lang.[Integer]. The {@code Modifier} class should
		''' be used to decode the modifiers.
		''' </summary>
		''' <seealso cref= Modifier </seealso>
		Public Property modifiers As Integer Implements Member.getModifiers
			Get
				Return modifiers
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this field represents an element of
		''' an enumerated type; returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if and only if this field represents an element of
		''' an enumerated type.
		''' @since 1.5 </returns>
		Public Property enumConstant As Boolean
			Get
				Return (modifiers And Modifier.ENUM) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this field is a synthetic
		''' field; returns {@code false} otherwise.
		''' </summary>
		''' <returns> true if and only if this field is a synthetic
		''' field as defined by the Java Language Specification.
		''' @since 1.5 </returns>
		Public Property synthetic As Boolean Implements Member.isSynthetic
			Get
				Return Modifier.isSynthetic(modifiers)
			End Get
		End Property

		''' <summary>
		''' Returns a {@code Class} object that identifies the
		''' declared type for the field represented by this
		''' {@code Field} object.
		''' </summary>
		''' <returns> a {@code Class} object identifying the declared
		''' type of the field represented by this object </returns>
		Public Property type As  [Class]
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Returns a {@code Type} object that represents the declared type for
		''' the field represented by this {@code Field} object.
		''' 
		''' <p>If the {@code Type} is a parameterized type, the
		''' {@code Type} object returned must accurately reflect the
		''' actual type parameters used in the source code.
		''' 
		''' <p>If the type of the underlying field is a type variable or a
		''' parameterized type, it is created. Otherwise, it is resolved.
		''' </summary>
		''' <returns> a {@code Type} object that represents the declared type for
		'''     the field represented by this {@code Field} object </returns>
		''' <exception cref="GenericSignatureFormatError"> if the generic field
		'''     signature does not conform to the format specified in
		'''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		''' <exception cref="TypeNotPresentException"> if the generic type
		'''     signature of the underlying field refers to a non-existent
		'''     type declaration </exception>
		''' <exception cref="MalformedParameterizedTypeException"> if the generic
		'''     signature of the underlying field refers to a parameterized type
		'''     that cannot be instantiated for any reason
		''' @since 1.5 </exception>
		Public Property genericType As Type
			Get
				If genericSignature IsNot Nothing Then
					Return genericInfo.genericType
				Else
					Return type
				End If
			End Get
		End Property


		''' <summary>
		''' Compares this {@code Field} against the specified object.  Returns
		''' true if the objects are the same.  Two {@code Field} objects are the same if
		''' they were declared by the same class and have the same name
		''' and type.
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj IsNot Nothing AndAlso TypeOf obj Is Field Then
				Dim other As Field = CType(obj, Field)
				Return (declaringClass Is other.declaringClass) AndAlso (name = other.name) AndAlso (type Is other.type)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hashcode for this {@code Field}.  This is computed as the
		''' exclusive-or of the hashcodes for the underlying field's
		''' declaring class name and its name.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return declaringClass.name.GetHashCode() Xor name.GetHashCode()
		End Function

		''' <summary>
		''' Returns a string describing this {@code Field}.  The format is
		''' the access modifiers for the field, if any, followed
		''' by the field type, followed by a space, followed by
		''' the fully-qualified name of the class declaring the field,
		''' followed by a period, followed by the name of the field.
		''' For example:
		''' <pre>
		'''    Public Shared final int java.lang.Thread.MIN_PRIORITY
		'''    private int java.io.FileDescriptor.fd
		''' </pre>
		''' 
		''' <p>The modifiers are placed in canonical order as specified by
		''' "The Java Language Specification".  This is {@code public},
		''' {@code protected} or {@code private} first, and then other
		''' modifiers in the following order: {@code static}, {@code final},
		''' {@code transient}, {@code volatile}.
		''' </summary>
		''' <returns> a string describing this {@code Field}
		''' @jls 8.3.1 Field Modifiers </returns>
		Public Overrides Function ToString() As String
			Dim [mod] As Integer = modifiers
			Return ((If([mod] = 0, "", (Modifier.ToString([mod]) & " "))) + type.typeName & " " & declaringClass.typeName & "." & name)
		End Function

		''' <summary>
		''' Returns a string describing this {@code Field}, including
		''' its generic type.  The format is the access modifiers for the
		''' field, if any, followed by the generic field type, followed by
		''' a space, followed by the fully-qualified name of the class
		''' declaring the field, followed by a period, followed by the name
		''' of the field.
		''' 
		''' <p>The modifiers are placed in canonical order as specified by
		''' "The Java Language Specification".  This is {@code public},
		''' {@code protected} or {@code private} first, and then other
		''' modifiers in the following order: {@code static}, {@code final},
		''' {@code transient}, {@code volatile}.
		''' </summary>
		''' <returns> a string describing this {@code Field}, including
		''' its generic type
		''' 
		''' @since 1.5
		''' @jls 8.3.1 Field Modifiers </returns>
		Public Function toGenericString() As String
			Dim [mod] As Integer = modifiers
			Dim fieldType As Type = genericType
			Return ((If([mod] = 0, "", (Modifier.ToString([mod]) & " "))) + fieldType.typeName & " " & declaringClass.typeName & "." & name)
		End Function

		''' <summary>
		''' Returns the value of the field represented by this {@code Field}, on
		''' the specified object. The value is automatically wrapped in an
		''' object if it has a primitive type.
		''' 
		''' <p>The underlying field's value is obtained as follows:
		''' 
		''' <p>If the underlying field is a static field, the {@code obj} argument
		''' is ignored; it may be null.
		''' 
		''' <p>Otherwise, the underlying field is an instance field.  If the
		''' specified {@code obj} argument is null, the method throws a
		''' {@code NullPointerException}. If the specified object is not an
		''' instance of the class or interface declaring the underlying
		''' field, the method throws an {@code IllegalArgumentException}.
		''' 
		''' <p>If this {@code Field} object is enforcing Java language access control, and
		''' the underlying field is inaccessible, the method throws an
		''' {@code IllegalAccessException}.
		''' If the underlying field is static, the class that declared the
		''' field is initialized if it has not already been initialized.
		''' 
		''' <p>Otherwise, the value is retrieved from the underlying instance
		''' or static field.  If the field has a primitive type, the value
		''' is wrapped in an object before being returned, otherwise it is
		''' returned as is.
		''' 
		''' <p>If the field is hidden in the type of {@code obj},
		''' the field's value is obtained according to the preceding rules.
		''' </summary>
		''' <param name="obj"> object from which the represented field's value is
		''' to be extracted </param>
		''' <returns> the value of the represented field in object
		''' {@code obj}; primitive values are wrapped in an appropriate
		''' object before being returned
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof). </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function [get](  obj As Object) As Object
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).get(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance {@code boolean} field.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code boolean} value
		''' from </param>
		''' <returns> the value of the {@code boolean} field
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code boolean} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getBoolean(  obj As Object) As Boolean
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getBoolean(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance {@code byte} field.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code byte} value
		''' from </param>
		''' <returns> the value of the {@code byte} field
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code byte} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getByte(  obj As Object) As SByte
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getByte(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance field of type
		''' {@code char} or of another primitive type convertible to
		''' type {@code char} via a widening conversion.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code char} value
		''' from </param>
		''' <returns> the value of the field converted to type {@code char}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code char} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref= Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getChar(  obj As Object) As Char
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getChar(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance field of type
		''' {@code short} or of another primitive type convertible to
		''' type {@code short} via a widening conversion.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code short} value
		''' from </param>
		''' <returns> the value of the field converted to type {@code short}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code short} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getShort(  obj As Object) As Short
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getShort(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance field of type
		''' {@code int} or of another primitive type convertible to
		''' type {@code int} via a widening conversion.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code int} value
		''' from </param>
		''' <returns> the value of the field converted to type {@code int}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code int} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getInt(  obj As Object) As Integer
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getInt(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance field of type
		''' {@code long} or of another primitive type convertible to
		''' type {@code long} via a widening conversion.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code long} value
		''' from </param>
		''' <returns> the value of the field converted to type {@code long}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code long} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getLong(  obj As Object) As Long
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getLong(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance field of type
		''' {@code float} or of another primitive type convertible to
		''' type {@code float} via a widening conversion.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code float} value
		''' from </param>
		''' <returns> the value of the field converted to type {@code float}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code float} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref= Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getFloat(  obj As Object) As Single
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getFloat(obj)
		End Function

		''' <summary>
		''' Gets the value of a static or instance field of type
		''' {@code double} or of another primitive type convertible to
		''' type {@code double} via a widening conversion.
		''' </summary>
		''' <param name="obj"> the object to extract the {@code double} value
		''' from </param>
		''' <returns> the value of the field converted to type {@code double}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not
		'''              an instance of the class or interface declaring the
		'''              underlying field (or a subclass or implementor
		'''              thereof), or if the field value cannot be
		'''              converted to the type {@code double} by a
		'''              widening conversion. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#get </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getDouble(  obj As Object) As Double
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Return getFieldAccessor(obj).getDouble(obj)
		End Function

		''' <summary>
		''' Sets the field represented by this {@code Field} object on the
		''' specified object argument to the specified new value. The new
		''' value is automatically unwrapped if the underlying field has a
		''' primitive type.
		''' 
		''' <p>The operation proceeds as follows:
		''' 
		''' <p>If the underlying field is static, the {@code obj} argument is
		''' ignored; it may be null.
		''' 
		''' <p>Otherwise the underlying field is an instance field.  If the
		''' specified object argument is null, the method throws a
		''' {@code NullPointerException}.  If the specified object argument is not
		''' an instance of the class or interface declaring the underlying
		''' field, the method throws an {@code IllegalArgumentException}.
		''' 
		''' <p>If this {@code Field} object is enforcing Java language access control, and
		''' the underlying field is inaccessible, the method throws an
		''' {@code IllegalAccessException}.
		''' 
		''' <p>If the underlying field is final, the method throws an
		''' {@code IllegalAccessException} unless {@code setAccessible(true)}
		''' has succeeded for this {@code Field} object
		''' and the field is non-static. Setting a final field in this way
		''' is meaningful only during deserialization or reconstruction of
		''' instances of classes with blank final fields, before they are
		''' made available for access by other parts of a program. Use in
		''' any other context may have unpredictable effects, including cases
		''' in which other parts of a program continue to use the original
		''' value of this field.
		''' 
		''' <p>If the underlying field is of a primitive type, an unwrapping
		''' conversion is attempted to convert the new value to a value of
		''' a primitive type.  If this attempt fails, the method throws an
		''' {@code IllegalArgumentException}.
		''' 
		''' <p>If, after possible unwrapping, the new value cannot be
		''' converted to the type of the underlying field by an identity or
		''' widening conversion, the method throws an
		''' {@code IllegalArgumentException}.
		''' 
		''' <p>If the underlying field is static, the class that declared the
		''' field is initialized if it has not already been initialized.
		''' 
		''' <p>The field is set to the possibly unwrapped and widened new value.
		''' 
		''' <p>If the field is hidden in the type of {@code obj},
		''' the field's value is set according to the preceding rules.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="value"> the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub [set](  obj As Object,   value As Object)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).set(obj, value)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code boolean} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, zObj)},
		''' where {@code zObj} is a {@code Boolean} object and
		''' {@code zObj.booleanValue() == z}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="z">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setBoolean(  obj As Object,   z As Boolean)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).booleanean(obj, z)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code byte} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, bObj)},
		''' where {@code bObj} is a {@code Byte} object and
		''' {@code bObj.byteValue() == b}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="b">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setByte(  obj As Object,   b As SByte)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).byteyte(obj, b)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code char} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, cObj)},
		''' where {@code cObj} is a {@code Character} object and
		''' {@code cObj.charValue() == c}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="c">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setChar(  obj As Object,   c As Char)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).charhar(obj, c)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code short} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, sObj)},
		''' where {@code sObj} is a {@code Short} object and
		''' {@code sObj.shortValue() == s}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="s">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setShort(  obj As Object,   s As Short)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).shortort(obj, s)
		End Sub

		''' <summary>
		''' Sets the value of a field as an {@code int} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, iObj)},
		''' where {@code iObj} is a {@code Integer} object and
		''' {@code iObj.intValue() == i}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="i">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setInt(  obj As Object,   i As Integer)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).intInt(obj, i)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code long} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, lObj)},
		''' where {@code lObj} is a {@code Long} object and
		''' {@code lObj.longValue() == l}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="l">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setLong(  obj As Object,   l As Long)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).longong(obj, l)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code float} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, fObj)},
		''' where {@code fObj} is a {@code Float} object and
		''' {@code fObj.floatValue() == f}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="f">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setFloat(  obj As Object,   f As Single)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).floatoat(obj, f)
		End Sub

		''' <summary>
		''' Sets the value of a field as a {@code double} on the specified object.
		''' This method is equivalent to
		''' {@code set(obj, dObj)},
		''' where {@code dObj} is a {@code Double} object and
		''' {@code dObj.doubleValue() == d}.
		''' </summary>
		''' <param name="obj"> the object whose field should be modified </param>
		''' <param name="d">   the new value for the field of {@code obj}
		''' being modified
		''' </param>
		''' <exception cref="IllegalAccessException">    if this {@code Field} object
		'''              is enforcing Java language access control and the underlying
		'''              field is either inaccessible or final. </exception>
		''' <exception cref="IllegalArgumentException">  if the specified object is not an
		'''              instance of the class or interface declaring the underlying
		'''              field (or a subclass or implementor thereof),
		'''              or if an unwrapping conversion fails. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the field is an instance field. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
		''' <seealso cref=       Field#set </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub setDouble(  obj As Object,   d As Double)
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			getFieldAccessor(obj).doubleble(obj, d)
		End Sub

		' security check is done before calling this method
		Private Function getFieldAccessor(  obj As Object) As sun.reflect.FieldAccessor
			Dim ov As Boolean = override
			Dim a As sun.reflect.FieldAccessor = If(ov, overrideFieldAccessor, fieldAccessor)
			Return If(a IsNot Nothing, a, acquireFieldAccessor(ov))
		End Function

		' NOTE that there is no synchronization used here. It is correct
		' (though not efficient) to generate more than one FieldAccessor
		' for a given Field. However, avoiding synchronization will
		' probably make the implementation more scalable.
		Private Function acquireFieldAccessor(  overrideFinalCheck As Boolean) As sun.reflect.FieldAccessor
			' First check to see if one has been created yet, and take it
			' if so
			Dim tmp As sun.reflect.FieldAccessor = Nothing
			If root IsNot Nothing Then tmp = root.getFieldAccessor(overrideFinalCheck)
			If tmp IsNot Nothing Then
				If overrideFinalCheck Then
					overrideFieldAccessor = tmp
				Else
					fieldAccessor = tmp
				End If
			Else
				' Otherwise fabricate one and propagate it up to the root
				tmp = reflectionFactory.newFieldAccessor(Me, overrideFinalCheck)
				fieldAccessorsor(tmp, overrideFinalCheck)
			End If

			Return tmp
		End Function

		' Returns FieldAccessor for this Field object, not looking up
		' the chain to the root
		Private Function getFieldAccessor(  overrideFinalCheck As Boolean) As sun.reflect.FieldAccessor
			Return If(overrideFinalCheck, overrideFieldAccessor, fieldAccessor)
		End Function

		' Sets the FieldAccessor for this Field object and
		' (recursively) its root
		Private Sub setFieldAccessor(  accessor As sun.reflect.FieldAccessor,   overrideFinalCheck As Boolean)
			If overrideFinalCheck Then
				overrideFieldAccessor = accessor
			Else
				fieldAccessor = accessor
			End If
			' Propagate up
			If root IsNot Nothing Then root.fieldAccessorsor(accessor, overrideFinalCheck)
		End Sub

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overrides Function getAnnotation(Of T As Annotation)(  annotationClass As [Class]) As T
			java.util.Objects.requireNonNull(annotationClass)
			Return annotationClass.cast(declaredAnnotations().get(annotationClass))
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getAnnotationsByType(Of T As Annotation)(  annotationClass As [Class]) As T()
			java.util.Objects.requireNonNull(annotationClass)

			Return sun.reflect.annotation.AnnotationSupport.getDirectlyAndIndirectlyPresent(declaredAnnotations(), annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public  Overrides ReadOnly Property  declaredAnnotations As Annotation()
			Get
				Return sun.reflect.annotation.AnnotationParser.ToArray(declaredAnnotations())
			End Get
		End Property

		<NonSerialized> _
		Private declaredAnnotations_Renamed As IDictionary(Of [Class], Annotation)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function declaredAnnotations() As IDictionary(Of [Class], Annotation)
			If declaredAnnotations_Renamed Is Nothing Then
				Dim root As Field = Me.root
				If root IsNot Nothing Then
					declaredAnnotations_Renamed = root.declaredAnnotations()
				Else
					declaredAnnotations_Renamed = sun.reflect.annotation.AnnotationParser.parseAnnotations(annotations, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), declaringClass)
				End If
			End If
			Return declaredAnnotations_Renamed
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function getTypeAnnotationBytes0() As SByte()
		End Function

		''' <summary>
		''' Returns an AnnotatedType object that represents the use of a type to specify
		''' the declared type of the field represented by this Field. </summary>
		''' <returns> an object representing the declared type of the field
		''' represented by this Field
		''' 
		''' @since 1.8 </returns>
		Public Property annotatedType As AnnotatedType
			Get
				Return sun.reflect.annotation.TypeAnnotationParser.buildAnnotatedType(typeAnnotationBytes0, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), Me, declaringClass, genericType, sun.reflect.annotation.TypeAnnotation.TypeAnnotationTarget.FIELD)
			End Get
		End Property
	End Class

End Namespace
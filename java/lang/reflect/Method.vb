Imports System

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
	''' A {@code Method} provides information about, and access to, a single method
	''' on a class or interface.  The reflected method may be a class method
	''' or an instance method (including an abstract method).
	''' 
	''' <p>A {@code Method} permits widening conversions to occur when matching the
	''' actual parameters to invoke with the underlying method's formal
	''' parameters, but it throws an {@code IllegalArgumentException} if a
	''' narrowing conversion would occur.
	''' </summary>
	''' <seealso cref= Member </seealso>
	''' <seealso cref= java.lang.Class </seealso>
	''' <seealso cref= java.lang.Class#getMethods() </seealso>
	''' <seealso cref= java.lang.Class#getMethod(String, Class[]) </seealso>
	''' <seealso cref= java.lang.Class#getDeclaredMethods() </seealso>
	''' <seealso cref= java.lang.Class#getDeclaredMethod(String, Class[])
	''' 
	''' @author Kenneth Russell
	''' @author Nakul Saraiya </seealso>
	Public NotInheritable Class Method
		Inherits Executable

		Private clazz As Class
		Private slot As Integer
		' This is guaranteed to be interned by the VM in the 1.4
		' reflection implementation
		Private name As String
		Private returnType As Class
		Private parameterTypes As Class()
		Private exceptionTypes As Class()
		Private modifiers As Integer
		' Generics and annotations support
		<NonSerialized> _
		Private signature As String
		' generic info repository; lazily initialized
		<NonSerialized> _
		Private genericInfo As sun.reflect.generics.repository.MethodRepository
		Private annotations As SByte()
		Private parameterAnnotations As SByte()
		Private annotationDefault As SByte()
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private methodAccessor As sun.reflect.MethodAccessor
		' For sharing of MethodAccessors. This branching structure is
		' currently only two levels deep (i.e., one root Method and
		' potentially many Method objects pointing to it.)
		'
		' If this branching structure would ever contain cycles, deadlocks can
		' occur in annotation code.
		Private root As Method

		' Generics infrastructure
		Private Property genericSignature As String
			Get
				Return signature
			End Get
		End Property

		' Accessor for factory
		Private Property factory As sun.reflect.generics.factory.GenericsFactory
			Get
				' create scope and factory
				Return sun.reflect.generics.factory.CoreReflectionFactory.make(Me, sun.reflect.generics.scope.MethodScope.make(Me))
			End Get
		End Property

		' Accessor for generic info repository
		Friend Property Overrides genericInfo As sun.reflect.generics.repository.MethodRepository
			Get
				' lazily initialize repository if necessary
				If genericInfo Is Nothing Then genericInfo = sun.reflect.generics.repository.MethodRepository.make(genericSignature, factory)
				Return genericInfo 'return cached repository
			End Get
		End Property

		''' <summary>
		''' Package-private constructor used by ReflectAccess to enable
		''' instantiation of these objects in Java code from the java.lang
		''' package via sun.reflect.LangReflectAccess.
		''' </summary>
		Friend Sub New(ByVal declaringClass As Class, ByVal name As String, ByVal parameterTypes As Class(), ByVal returnType As Class, ByVal checkedExceptions As Class(), ByVal modifiers As Integer, ByVal slot As Integer, ByVal signature As String, ByVal annotations As SByte(), ByVal parameterAnnotations As SByte(), ByVal annotationDefault As SByte())
			Me.clazz = declaringClass
			Me.name = name
			Me.parameterTypes = parameterTypes
			Me.returnType = returnType
			Me.exceptionTypes = checkedExceptions
			Me.modifiers = modifiers
			Me.slot = slot
			Me.signature = signature
			Me.annotations = annotations
			Me.parameterAnnotations = parameterAnnotations
			Me.annotationDefault = annotationDefault
		End Sub

		''' <summary>
		''' Package-private routine (exposed to java.lang.Class via
		''' ReflectAccess) which returns a copy of this Method. The copy's
		''' "root" field points to this Method.
		''' </summary>
		Friend Function copy() As Method
			' This routine enables sharing of MethodAccessor objects
			' among Method objects which refer to the same underlying
			' method in the VM. (All of this contortion is only necessary
			' because of the "accessibility" bit in AccessibleObject,
			' which implicitly requires that new java.lang.reflect
			' objects be fabricated for each reflective call on Class
			' objects.)
			If Me.root IsNot Nothing Then Throw New IllegalArgumentException("Can not copy a non-root Method")

			Dim res As New Method(clazz, name, parameterTypes, returnType, exceptionTypes, modifiers, slot, signature, annotations, parameterAnnotations, annotationDefault)
			res.root = Me
			' Might as well eagerly propagate this if already present
			res.methodAccessor = methodAccessor
			Return res
		End Function

		''' <summary>
		''' Used by Excecutable for annotation sharing.
		''' </summary>
		Friend Property Overrides root As Executable
			Get
				Return root
			End Get
		End Property

		Friend Overrides Function hasGenericInformation() As Boolean
			Return (genericSignature IsNot Nothing)
		End Function

		Friend Property Overrides annotationBytes As SByte()
			Get
				Return annotations
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides declaringClass As Class
			Get
				Return clazz
			End Get
		End Property

		''' <summary>
		''' Returns the name of the method represented by this {@code Method}
		''' object, as a {@code String}.
		''' </summary>
		Public Property Overrides name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides modifiers As Integer
			Get
				Return modifiers
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="GenericSignatureFormatError"> {@inheritDoc}
		''' @since 1.5 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property Overrides typeParameters As TypeVariable(Of Method)()
			Get
				If genericSignature IsNot Nothing Then
					Return CType(genericInfo.typeParameters, TypeVariable(Of Method)())
				Else
					Return CType(New TypeVariable(){}, TypeVariable(Of Method)())
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a {@code Class} object that represents the formal return type
		''' of the method represented by this {@code Method} object.
		''' </summary>
		''' <returns> the return type for the method this object represents </returns>
		Public Property returnType As Class
			Get
				Return returnType
			End Get
		End Property

		''' <summary>
		''' Returns a {@code Type} object that represents the formal return
		''' type of the method represented by this {@code Method} object.
		''' 
		''' <p>If the return type is a parameterized type,
		''' the {@code Type} object returned must accurately reflect
		''' the actual type parameters used in the source code.
		''' 
		''' <p>If the return type is a type variable or a parameterized type, it
		''' is created. Otherwise, it is resolved.
		''' </summary>
		''' <returns>  a {@code Type} object that represents the formal return
		'''     type of the underlying  method </returns>
		''' <exception cref="GenericSignatureFormatError">
		'''     if the generic method signature does not conform to the format
		'''     specified in
		'''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		''' <exception cref="TypeNotPresentException"> if the underlying method's
		'''     return type refers to a non-existent type declaration </exception>
		''' <exception cref="MalformedParameterizedTypeException"> if the
		'''     underlying method's return typed refers to a parameterized
		'''     type that cannot be instantiated for any reason
		''' @since 1.5 </exception>
		Public Property genericReturnType As Type
			Get
			  If genericSignature IsNot Nothing Then
				Return genericInfo.returnType
			  Else
				  Return returnType
			  End If
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides parameterTypes As Class()
			Get
				Return parameterTypes.clone()
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides parameterCount As Integer
			Get
				Return parameterTypes.Length
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="GenericSignatureFormatError"> {@inheritDoc} </exception>
		''' <exception cref="TypeNotPresentException"> {@inheritDoc} </exception>
		''' <exception cref="MalformedParameterizedTypeException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Property Overrides genericParameterTypes As Type()
			Get
				Return MyBase.genericParameterTypes
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides exceptionTypes As Class()
			Get
				Return exceptionTypes.clone()
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="GenericSignatureFormatError"> {@inheritDoc} </exception>
		''' <exception cref="TypeNotPresentException"> {@inheritDoc} </exception>
		''' <exception cref="MalformedParameterizedTypeException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Property Overrides genericExceptionTypes As Type()
			Get
				Return MyBase.genericExceptionTypes
			End Get
		End Property

		''' <summary>
		''' Compares this {@code Method} against the specified object.  Returns
		''' true if the objects are the same.  Two {@code Methods} are the same if
		''' they were declared by the same class and have the same name
		''' and formal parameter types and return type.
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj IsNot Nothing AndAlso TypeOf obj Is Method Then
				Dim other As Method = CType(obj, Method)
				If (declaringClass Is other.declaringClass) AndAlso (name = other.name) Then
					If Not returnType.Equals(other.returnType) Then Return False
					Return equalParamTypes(parameterTypes, other.parameterTypes)
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hashcode for this {@code Method}.  The hashcode is computed
		''' as the exclusive-or of the hashcodes for the underlying
		''' method's declaring class name and the method's name.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return declaringClass.name.GetHashCode() Xor name.GetHashCode()
		End Function

		''' <summary>
		''' Returns a string describing this {@code Method}.  The string is
		''' formatted as the method access modifiers, if any, followed by
		''' the method return type, followed by a space, followed by the
		''' class declaring the method, followed by a period, followed by
		''' the method name, followed by a parenthesized, comma-separated
		''' list of the method's formal parameter types. If the method
		''' throws checked exceptions, the parameter list is followed by a
		''' space, followed by the word throws followed by a
		''' comma-separated list of the thrown exception types.
		''' For example:
		''' <pre>
		'''    public boolean java.lang.Object.equals(java.lang.Object)
		''' </pre>
		''' 
		''' <p>The access modifiers are placed in canonical order as
		''' specified by "The Java Language Specification".  This is
		''' {@code public}, {@code protected} or {@code private} first,
		''' and then other modifiers in the following order:
		''' {@code abstract}, {@code default}, {@code static}, {@code final},
		''' {@code synchronized}, {@code native}, {@code strictfp}.
		''' </summary>
		''' <returns> a string describing this {@code Method}
		''' 
		''' @jls 8.4.3 Method Modifiers </returns>
		Public Overrides Function ToString() As String
			Return sharedToString(Modifier.methodModifiers(), [default], parameterTypes, exceptionTypes)
		End Function

		Friend Overrides Sub specificToStringHeader(ByVal sb As StringBuilder)
			sb.append(returnType.typeName).append(" "c)
			sb.append(declaringClass.typeName).append("."c)
			sb.append(name)
		End Sub

		''' <summary>
		''' Returns a string describing this {@code Method}, including
		''' type parameters.  The string is formatted as the method access
		''' modifiers, if any, followed by an angle-bracketed
		''' comma-separated list of the method's type parameters, if any,
		''' followed by the method's generic return type, followed by a
		''' space, followed by the class declaring the method, followed by
		''' a period, followed by the method name, followed by a
		''' parenthesized, comma-separated list of the method's generic
		''' formal parameter types.
		''' 
		''' If this method was declared to take a variable number of
		''' arguments, instead of denoting the last parameter as
		''' "<tt><i>Type</i>[]</tt>", it is denoted as
		''' "<tt><i>Type</i>...</tt>".
		''' 
		''' A space is used to separate access modifiers from one another
		''' and from the type parameters or return type.  If there are no
		''' type parameters, the type parameter list is elided; if the type
		''' parameter list is present, a space separates the list from the
		''' class name.  If the method is declared to throw exceptions, the
		''' parameter list is followed by a space, followed by the word
		''' throws followed by a comma-separated list of the generic thrown
		''' exception types.
		''' 
		''' <p>The access modifiers are placed in canonical order as
		''' specified by "The Java Language Specification".  This is
		''' {@code public}, {@code protected} or {@code private} first,
		''' and then other modifiers in the following order:
		''' {@code abstract}, {@code default}, {@code static}, {@code final},
		''' {@code synchronized}, {@code native}, {@code strictfp}.
		''' </summary>
		''' <returns> a string describing this {@code Method},
		''' include type parameters
		''' 
		''' @since 1.5
		''' 
		''' @jls 8.4.3 Method Modifiers </returns>
		Public Overrides Function toGenericString() As String
			Return sharedToGenericString(Modifier.methodModifiers(), [default])
		End Function

		Friend Overrides Sub specificToGenericStringHeader(ByVal sb As StringBuilder)
			Dim genRetType As Type = genericReturnType
			sb.append(genRetType.typeName).append(" "c)
			sb.append(declaringClass.typeName).append("."c)
			sb.append(name)
		End Sub

		''' <summary>
		''' Invokes the underlying method represented by this {@code Method}
		''' object, on the specified object with the specified parameters.
		''' Individual parameters are automatically unwrapped to match
		''' primitive formal parameters, and both primitive and reference
		''' parameters are subject to method invocation conversions as
		''' necessary.
		''' 
		''' <p>If the underlying method is static, then the specified {@code obj}
		''' argument is ignored. It may be null.
		''' 
		''' <p>If the number of formal parameters required by the underlying method is
		''' 0, the supplied {@code args} array may be of length 0 or null.
		''' 
		''' <p>If the underlying method is an instance method, it is invoked
		''' using dynamic method lookup as documented in The Java Language
		''' Specification, Second Edition, section 15.12.4.4; in particular,
		''' overriding based on the runtime type of the target object will occur.
		''' 
		''' <p>If the underlying method is static, the class that declared
		''' the method is initialized if it has not already been initialized.
		''' 
		''' <p>If the method completes normally, the value it returns is
		''' returned to the caller of invoke; if the value has a primitive
		''' type, it is first appropriately wrapped in an object. However,
		''' if the value has the type of an array of a primitive type, the
		''' elements of the array are <i>not</i> wrapped in objects; in
		''' other words, an array of primitive type is returned.  If the
		''' underlying method return type is void, the invocation returns
		''' null.
		''' </summary>
		''' <param name="obj">  the object the underlying method is invoked from </param>
		''' <param name="args"> the arguments used for the method call </param>
		''' <returns> the result of dispatching the method represented by
		''' this object on {@code obj} with parameters
		''' {@code args}
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Method} object
		'''              is enforcing Java language access control and the underlying
		'''              method is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the method is an
		'''              instance method and the specified object argument
		'''              is not an instance of the class or interface
		'''              declaring the underlying method (or of a subclass
		'''              or implementor thereof); if the number of actual
		'''              and formal parameters differ; if an unwrapping
		'''              conversion for primitive arguments fails; or if,
		'''              after possible unwrapping, a parameter value
		'''              cannot be converted to the corresponding formal
		'''              parameter type by a method invocation conversion. </exception>
		''' <exception cref="InvocationTargetException"> if the underlying method
		'''              throws an exception. </exception>
		''' <exception cref="NullPointerException">      if the specified object is null
		'''              and the method is an instance method. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization
		''' provoked by this method fails. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function invoke(ByVal obj As Object, ParamArray ByVal args As Object()) As Object
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As Class = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, obj, modifiers)
				End If
			End If
			Dim ma As sun.reflect.MethodAccessor = methodAccessor ' read volatile
			If ma Is Nothing Then ma = acquireMethodAccessor()
			Return ma.invoke(obj, args)
		End Function

		''' <summary>
		''' Returns {@code true} if this method is a bridge
		''' method; returns {@code false} otherwise.
		''' </summary>
		''' <returns> true if and only if this method is a bridge
		''' method as defined by the Java Language Specification.
		''' @since 1.5 </returns>
		Public Property bridge As Boolean
			Get
				Return (modifiers And Modifier.BRIDGE) <> 0
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.5
		''' </summary>
		Public Property Overrides varArgs As Boolean
			Get
				Return MyBase.varArgs
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @jls 13.1 The Form of a Binary
		''' @since 1.5
		''' </summary>
		Public Property Overrides synthetic As Boolean
			Get
				Return MyBase.synthetic
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this method is a default
		''' method; returns {@code false} otherwise.
		''' 
		''' A default method is a public non-abstract instance method, that
		''' is, a non-static method with a body, declared in an interface
		''' type.
		''' </summary>
		''' <returns> true if and only if this method is a default
		''' method as defined by the Java Language Specification.
		''' @since 1.8 </returns>
		Public Property [default] As Boolean
			Get
				' Default methods are public non-abstract instance methods
				' declared in an interface.
				Return ((modifiers And (Modifier.ABSTRACT Or Modifier.PUBLIC Or Modifier.STATIC)) = Modifier.PUBLIC) AndAlso declaringClass.interface
			End Get
		End Property

		' NOTE that there is no synchronization used here. It is correct
		' (though not efficient) to generate more than one MethodAccessor
		' for a given Method. However, avoiding synchronization will
		' probably make the implementation more scalable.
		Private Function acquireMethodAccessor() As sun.reflect.MethodAccessor
			' First check to see if one has been created yet, and take it
			' if so
			Dim tmp As sun.reflect.MethodAccessor = Nothing
			If root IsNot Nothing Then tmp = root.methodAccessor
			If tmp IsNot Nothing Then
				methodAccessor = tmp
			Else
				' Otherwise fabricate one and propagate it up to the root
				tmp = reflectionFactory.newMethodAccessor(Me)
				methodAccessor = tmp
			End If

			Return tmp
		End Function

		' Returns MethodAccessor for this Method object, not looking up
		' the chain to the root
		Friend Property methodAccessor As sun.reflect.MethodAccessor
			Get
				Return methodAccessor
			End Get
			Set(ByVal accessor As sun.reflect.MethodAccessor)
				methodAccessor = accessor
				' Propagate up
				If root IsNot Nothing Then root.methodAccessor = accessor
			End Set
		End Property

		' Sets the MethodAccessor for this Method object and
		' (recursively) its root

		''' <summary>
		''' Returns the default value for the annotation member represented by
		''' this {@code Method} instance.  If the member is of a primitive type,
		''' an instance of the corresponding wrapper type is returned. Returns
		''' null if no default is associated with the member, or if the method
		''' instance does not represent a declared member of an annotation type.
		''' </summary>
		''' <returns> the default value for the annotation member represented
		'''     by this {@code Method} instance. </returns>
		''' <exception cref="TypeNotPresentException"> if the annotation is of type
		'''     <seealso cref="Class"/> and no definition can be found for the
		'''     default class value.
		''' @since  1.5 </exception>
		Public Property defaultValue As Object
			Get
				If annotationDefault Is Nothing Then Return Nothing
				Dim memberType As Class = sun.reflect.annotation.AnnotationType.invocationHandlerReturnType(returnType)
				Dim result As Object = sun.reflect.annotation.AnnotationParser.parseMemberValue(memberType, java.nio.ByteBuffer.wrap(annotationDefault), sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), declaringClass)
				If TypeOf result Is sun.reflect.annotation.ExceptionProxy Then Throw New AnnotationFormatError("Invalid default: " & Me)
				Return result
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException">  {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overrides Function getAnnotation(Of T As Annotation)(ByVal annotationClass As Class) As T
			Return MyBase.getAnnotation(annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.5
		''' </summary>
		Public Property Overrides declaredAnnotations As Annotation()
			Get
				Return MyBase.GetCustomAttributes(False)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.5
		''' </summary>
		Public Property Overrides parameterAnnotations As Annotation()()
			Get
				Return sharedGetParameterAnnotations(parameterTypes, parameterAnnotations)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.8
		''' </summary>
		Public Property Overrides annotatedReturnType As AnnotatedType
			Get
				Return getAnnotatedReturnType0(genericReturnType)
			End Get
		End Property

		Friend Overrides Sub handleParameterNumberMismatch(ByVal resultLength As Integer, ByVal numParameters As Integer)
			Throw New AnnotationFormatError("Parameter annotations don't match number of parameters")
		End Sub
	End Class

End Namespace
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
	''' {@code Constructor} provides information about, and access to, a single
	''' constructor for a class.
	''' 
	''' <p>{@code Constructor} permits widening conversions to occur when matching the
	''' actual parameters to newInstance() with the underlying
	''' constructor's formal parameters, but throws an
	''' {@code IllegalArgumentException} if a narrowing conversion would occur.
	''' </summary>
	''' @param <T> the class in which the constructor is declared
	''' </param>
	''' <seealso cref= Member </seealso>
	''' <seealso cref= java.lang.Class </seealso>
	''' <seealso cref= java.lang.Class#getConstructors() </seealso>
	''' <seealso cref= java.lang.Class#getConstructor(Class[]) </seealso>
	''' <seealso cref= java.lang.Class#getDeclaredConstructors()
	''' 
	''' @author      Kenneth Russell
	''' @author      Nakul Saraiya </seealso>
	Public NotInheritable Class Constructor(Of T)
		Inherits Executable

		Private clazz As  [Class]
		Private slot As Integer
		Private parameterTypes As  [Class]()
		Private exceptionTypes As  [Class]()
		Private modifiers As Integer
		' Generics and annotations support
		<NonSerialized> _
		Private signature As String
		' generic info repository; lazily initialized
		<NonSerialized> _
		Private genericInfo As sun.reflect.generics.repository.ConstructorRepository
		Private annotations As SByte()
		Private parameterAnnotations As SByte()

		' Generics infrastructure
		' Accessor for factory
		Private Property factory As sun.reflect.generics.factory.GenericsFactory
			Get
				' create scope and factory
				Return sun.reflect.generics.factory.CoreReflectionFactory.make(Me, sun.reflect.generics.scope.ConstructorScope.make(Me))
			End Get
		End Property

		' Accessor for generic info repository
		Friend  Overrides ReadOnly Property  genericInfo As sun.reflect.generics.repository.ConstructorRepository
			Get
				' lazily initialize repository if necessary
				If genericInfo Is Nothing Then genericInfo = sun.reflect.generics.repository.ConstructorRepository.make(signature, factory)
				Return genericInfo 'return cached repository
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private constructorAccessor As sun.reflect.ConstructorAccessor
		' For sharing of ConstructorAccessors. This branching structure
		' is currently only two levels deep (i.e., one root Constructor
		' and potentially many Constructor objects pointing to it.)
		'
		' If this branching structure would ever contain cycles, deadlocks can
		' occur in annotation code.
		Private root As Constructor(Of T)

		''' <summary>
		''' Used by Excecutable for annotation sharing.
		''' </summary>
		Friend  Overrides ReadOnly Property  root As Executable
			Get
				Return root
			End Get
		End Property

		''' <summary>
		''' Package-private constructor used by ReflectAccess to enable
		''' instantiation of these objects in Java code from the java.lang
		''' package via sun.reflect.LangReflectAccess.
		''' </summary>
		Friend Sub New(ByVal declaringClass As [Class], ByVal parameterTypes As  [Class](), ByVal checkedExceptions As  [Class](), ByVal modifiers As Integer, ByVal slot As Integer, ByVal signature As String, ByVal annotations As SByte(), ByVal parameterAnnotations As SByte())
			Me.clazz = declaringClass
			Me.parameterTypes = parameterTypes
			Me.exceptionTypes = checkedExceptions
			Me.modifiers = modifiers
			Me.slot = slot
			Me.signature = signature
			Me.annotations = annotations
			Me.parameterAnnotations = parameterAnnotations
		End Sub

		''' <summary>
		''' Package-private routine (exposed to java.lang.Class via
		''' ReflectAccess) which returns a copy of this Constructor. The copy's
		''' "root" field points to this Constructor.
		''' </summary>
		Friend Function copy() As Constructor(Of T)
			' This routine enables sharing of ConstructorAccessor objects
			' among Constructor objects which refer to the same underlying
			' method in the VM. (All of this contortion is only necessary
			' because of the "accessibility" bit in AccessibleObject,
			' which implicitly requires that new java.lang.reflect
			' objects be fabricated for each reflective call on Class
			' objects.)
			If Me.root IsNot Nothing Then Throw New IllegalArgumentException("Can not copy a non-root Constructor")

			Dim res As New Constructor(Of T)(clazz, parameterTypes, exceptionTypes, modifiers, slot, signature, annotations, parameterAnnotations)
			res.root = Me
			' Might as well eagerly propagate this if already present
			res.constructorAccessor = constructorAccessor
			Return res
		End Function

		Friend Overrides Function hasGenericInformation() As Boolean
			Return (signature IsNot Nothing)
		End Function

		Friend  Overrides ReadOnly Property  annotationBytes As SByte()
			Get
				Return annotations
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public  Overrides ReadOnly Property  declaringClass As  [Class]
			Get
				Return clazz
			End Get
		End Property

		''' <summary>
		''' Returns the name of this constructor, as a string.  This is
		''' the binary name of the constructor's declaring class.
		''' </summary>
		Public  Overrides ReadOnly Property  name As String
			Get
				Return declaringClass.name
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public  Overrides ReadOnly Property  modifiers As Integer
			Get
				Return modifiers
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="GenericSignatureFormatError"> {@inheritDoc}
		''' @since 1.5 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public  Overrides ReadOnly Property  typeParameters As TypeVariable(Of Constructor(Of T))()
			Get
			  If signature IsNot Nothing Then
				Return CType(genericInfo.typeParameters, TypeVariable(Of Constructor(Of T))())
			  Else
				  Return CType(New TypeVariable(){}, TypeVariable(Of Constructor(Of T))())
			  End If
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public  Overrides ReadOnly Property  parameterTypes As  [Class]()
			Get
				Return parameterTypes.clone()
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public  Overrides ReadOnly Property  parameterCount As Integer
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
		Public  Overrides ReadOnly Property  genericParameterTypes As Type()
			Get
				Return MyBase.genericParameterTypes
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public  Overrides ReadOnly Property  exceptionTypes As  [Class]()
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
		Public  Overrides ReadOnly Property  genericExceptionTypes As Type()
			Get
				Return MyBase.genericExceptionTypes
			End Get
		End Property

		''' <summary>
		''' Compares this {@code Constructor} against the specified object.
		''' Returns true if the objects are the same.  Two {@code Constructor} objects are
		''' the same if they were declared by the same class and have the
		''' same formal parameter types.
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj IsNot Nothing AndAlso TypeOf obj Is Constructor Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim other As Constructor(Of ?) = CType(obj, Constructor(Of ?))
				If declaringClass Is other.declaringClass Then Return equalParamTypes(parameterTypes, other.parameterTypes)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hashcode for this {@code Constructor}. The hashcode is
		''' the same as the hashcode for the underlying constructor's
		''' declaring class name.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return declaringClass.name.GetHashCode()
		End Function

		''' <summary>
		''' Returns a string describing this {@code Constructor}.  The string is
		''' formatted as the constructor access modifiers, if any,
		''' followed by the fully-qualified name of the declaring [Class],
		''' followed by a parenthesized, comma-separated list of the
		''' constructor's formal parameter types.  For example:
		''' <pre>
		'''    public java.util.Hashtable(int,float)
		''' </pre>
		''' 
		''' <p>The only possible modifiers for constructors are the access
		''' modifiers {@code public}, {@code protected} or
		''' {@code private}.  Only one of these may appear, or none if the
		''' constructor has default (package) access.
		''' </summary>
		''' <returns> a string describing this {@code Constructor}
		''' @jls 8.8.3. Constructor Modifiers </returns>
		Public Overrides Function ToString() As String
			Return sharedToString(Modifier.constructorModifiers(), False, parameterTypes, exceptionTypes)
		End Function

		Friend Overrides Sub specificToStringHeader(ByVal sb As StringBuilder)
			sb.append(declaringClass.typeName)
		End Sub

		''' <summary>
		''' Returns a string describing this {@code Constructor},
		''' including type parameters.  The string is formatted as the
		''' constructor access modifiers, if any, followed by an
		''' angle-bracketed comma separated list of the constructor's type
		''' parameters, if any, followed by the fully-qualified name of the
		''' declaring [Class], followed by a parenthesized, comma-separated
		''' list of the constructor's generic formal parameter types.
		''' 
		''' If this constructor was declared to take a variable number of
		''' arguments, instead of denoting the last parameter as
		''' "<tt><i>Type</i>[]</tt>", it is denoted as
		''' "<tt><i>Type</i>...</tt>".
		''' 
		''' A space is used to separate access modifiers from one another
		''' and from the type parameters or return type.  If there are no
		''' type parameters, the type parameter list is elided; if the type
		''' parameter list is present, a space separates the list from the
		''' class name.  If the constructor is declared to throw
		''' exceptions, the parameter list is followed by a space, followed
		''' by the word "{@code throws}" followed by a
		''' comma-separated list of the thrown exception types.
		''' 
		''' <p>The only possible modifiers for constructors are the access
		''' modifiers {@code public}, {@code protected} or
		''' {@code private}.  Only one of these may appear, or none if the
		''' constructor has default (package) access.
		''' </summary>
		''' <returns> a string describing this {@code Constructor},
		''' include type parameters
		''' 
		''' @since 1.5
		''' @jls 8.8.3. Constructor Modifiers </returns>
		Public Overrides Function toGenericString() As String
			Return sharedToGenericString(Modifier.constructorModifiers(), False)
		End Function

		Friend Overrides Sub specificToGenericStringHeader(ByVal sb As StringBuilder)
			specificToStringHeader(sb)
		End Sub

		''' <summary>
		''' Uses the constructor represented by this {@code Constructor} object to
		''' create and initialize a new instance of the constructor's
		''' declaring [Class], with the specified initialization parameters.
		''' Individual parameters are automatically unwrapped to match
		''' primitive formal parameters, and both primitive and reference
		''' parameters are subject to method invocation conversions as necessary.
		''' 
		''' <p>If the number of formal parameters required by the underlying constructor
		''' is 0, the supplied {@code initargs} array may be of length 0 or null.
		''' 
		''' <p>If the constructor's declaring class is an inner class in a
		''' non-static context, the first argument to the constructor needs
		''' to be the enclosing instance; see section 15.9.3 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' 
		''' <p>If the required access and argument checks succeed and the
		''' instantiation will proceed, the constructor's declaring class
		''' is initialized if it has not already been initialized.
		''' 
		''' <p>If the constructor completes normally, returns the newly
		''' created and initialized instance.
		''' </summary>
		''' <param name="initargs"> array of objects to be passed as arguments to
		''' the constructor call; values of primitive types are wrapped in
		''' a wrapper object of the appropriate type (e.g. a {@code float}
		''' in a <seealso cref="java.lang.Float Float"/>)
		''' </param>
		''' <returns> a new object created by calling the constructor
		''' this object represents
		''' </returns>
		''' <exception cref="IllegalAccessException">    if this {@code Constructor} object
		'''              is enforcing Java language access control and the underlying
		'''              constructor is inaccessible. </exception>
		''' <exception cref="IllegalArgumentException">  if the number of actual
		'''              and formal parameters differ; if an unwrapping
		'''              conversion for primitive arguments fails; or if,
		'''              after possible unwrapping, a parameter value
		'''              cannot be converted to the corresponding formal
		'''              parameter type by a method invocation conversion; if
		'''              this constructor pertains to an enum type. </exception>
		''' <exception cref="InstantiationException">    if the class that declares the
		'''              underlying constructor represents an abstract class. </exception>
		''' <exception cref="InvocationTargetException"> if the underlying constructor
		'''              throws an exception. </exception>
		''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
		'''              by this method fails. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function newInstance(ParamArray ByVal initargs As Object()) As T
			If Not override Then
				If Not sun.reflect.Reflection.quickCheckMemberAccess(clazz, modifiers) Then
					Dim caller As  [Class] = sun.reflect.Reflection.callerClass
					checkAccess(caller, clazz, Nothing, modifiers)
				End If
			End If
			If (clazz.modifiers And Modifier.ENUM) <> 0 Then Throw New IllegalArgumentException("Cannot reflectively create enum objects")
			Dim ca As sun.reflect.ConstructorAccessor = constructorAccessor ' read volatile
			If ca Is Nothing Then ca = acquireConstructorAccessor()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim inst As T = CType(ca.newInstance(initargs), T)
			Return inst
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.5
		''' </summary>
		Public  Overrides ReadOnly Property  varArgs As Boolean
			Get
				Return MyBase.varArgs
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @jls 13.1 The Form of a Binary
		''' @since 1.5
		''' </summary>
		Public  Overrides ReadOnly Property  synthetic As Boolean
			Get
				Return MyBase.synthetic
			End Get
		End Property

		' NOTE that there is no synchronization used here. It is correct
		' (though not efficient) to generate more than one
		' ConstructorAccessor for a given Constructor. However, avoiding
		' synchronization will probably make the implementation more
		' scalable.
		Private Function acquireConstructorAccessor() As sun.reflect.ConstructorAccessor
			' First check to see if one has been created yet, and take it
			' if so.
			Dim tmp As sun.reflect.ConstructorAccessor = Nothing
			If root IsNot Nothing Then tmp = root.constructorAccessor
			If tmp IsNot Nothing Then
				constructorAccessor = tmp
			Else
				' Otherwise fabricate one and propagate it up to the root
				tmp = reflectionFactory.newConstructorAccessor(Me)
				constructorAccessor = tmp
			End If

			Return tmp
		End Function

		' Returns ConstructorAccessor for this Constructor object, not
		' looking up the chain to the root
		Friend Property constructorAccessor As sun.reflect.ConstructorAccessor
			Get
				Return constructorAccessor
			End Get
			Set(ByVal accessor As sun.reflect.ConstructorAccessor)
				constructorAccessor = accessor
				' Propagate up
				If root IsNot Nothing Then root.constructorAccessor = accessor
			End Set
		End Property

		' Sets the ConstructorAccessor for this Constructor object and
		' (recursively) its root

		Friend Property slot As Integer
			Get
				Return slot
			End Get
		End Property

		Friend Property signature As String
			Get
				Return signature
			End Get
		End Property

		Friend Property rawAnnotations As SByte()
			Get
				Return annotations
			End Get
		End Property

		Friend Property rawParameterAnnotations As SByte()
			Get
				Return parameterAnnotations
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException">  {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overrides Function getAnnotation(Of T As Annotation)(ByVal annotationClass As [Class]) As T
			Return MyBase.getAnnotation(annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.5
		''' </summary>
		Public  Overrides ReadOnly Property  declaredAnnotations As Annotation()
			Get
				Return MyBase.GetCustomAttributes(False)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.5
		''' </summary>
		Public  Overrides ReadOnly Property  parameterAnnotations As Annotation()()
			Get
				Return sharedGetParameterAnnotations(parameterTypes, parameterAnnotations)
			End Get
		End Property

		Friend Overrides Sub handleParameterNumberMismatch(ByVal resultLength As Integer, ByVal numParameters As Integer)
			Dim declaringClass_Renamed As  [Class] = declaringClass
			If declaringClass_Renamed.enum OrElse declaringClass_Renamed.anonymousClass OrElse declaringClass_Renamed.localClass Then
				Return ' Can't do reliable parameter counting
			Else
				If (Not declaringClass_Renamed.memberClass) OrElse (declaringClass_Renamed.memberClass AndAlso ((declaringClass_Renamed.modifiers And Modifier.STATIC) = 0) AndAlso resultLength + 1 <> numParameters) Then ' top-level Throw New AnnotationFormatError("Parameter annotations don't match number of parameters")
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.8
		''' </summary>
		Public  Overrides ReadOnly Property  annotatedReturnType As AnnotatedType
			Get
				Return getAnnotatedReturnType0(declaringClass)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.8
		''' </summary>
		Public  Overrides ReadOnly Property  annotatedReceiverType As AnnotatedType
			Get
				If declaringClass.enclosingClass Is Nothing Then Return MyBase.annotatedReceiverType
    
				Return sun.reflect.annotation.TypeAnnotationParser.buildAnnotatedType(typeAnnotationBytes0, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), Me, declaringClass, declaringClass.enclosingClass, sun.reflect.annotation.TypeAnnotation.TypeAnnotationTarget.METHOD_RECEIVER)
			End Get
		End Property
	End Class

End Namespace
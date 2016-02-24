Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A shared superclass for the common functionality of <seealso cref="Method"/>
	''' and <seealso cref="Constructor"/>.
	''' 
	''' @since 1.8
	''' </summary>
	Public MustInherit Class Executable
		Inherits AccessibleObject
		Implements Member, GenericDeclaration

	'    
	'     * Only grant package-visibility to the constructor.
	'     
		Friend Sub New()
		End Sub

		''' <summary>
		''' Accessor method to allow code sharing
		''' </summary>
		Friend MustOverride ReadOnly Property annotationBytes As SByte()

		''' <summary>
		''' Accessor method to allow code sharing
		''' </summary>
		Friend MustOverride ReadOnly Property root As Executable

		''' <summary>
		''' Does the Executable have generic information.
		''' </summary>
		Friend MustOverride Function hasGenericInformation() As Boolean

		Friend MustOverride ReadOnly Property genericInfo As sun.reflect.generics.repository.ConstructorRepository

		Friend Overridable Function equalParamTypes(ByVal params1 As Class(), ByVal params2 As Class()) As Boolean
			' Avoid unnecessary cloning 
			If params1.Length = params2.Length Then
				For i As Integer = 0 To params1.Length - 1
					If params1(i) IsNot params2(i) Then Return False
				Next i
				Return True
			End If
			Return False
		End Function

		Friend Overridable Function parseParameterAnnotations(ByVal parameterAnnotations As SByte()) As Annotation()()
			Return sun.reflect.annotation.AnnotationParser.parseParameterAnnotations(parameterAnnotations, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), declaringClass)
		End Function

		Friend Overridable Sub separateWithCommas(ByVal types As Class(), ByVal sb As StringBuilder)
			For j As Integer = 0 To types.Length - 1
				sb.append(types(j).typeName)
				If j < (types.Length - 1) Then sb.append(",")
			Next j

		End Sub

		Friend Overridable Sub printModifiersIfNonzero(ByVal sb As StringBuilder, ByVal mask As Integer, ByVal isDefault As Boolean)
			Dim [mod] As Integer = modifiers And mask

			If [mod] <> 0 AndAlso (Not isDefault) Then
				sb.append(Modifier.ToString([mod])).append(" "c)
			Else
				Dim access_mod As Integer = [mod] And Modifier.ACCESS_MODIFIERS
				If access_mod <> 0 Then sb.append(Modifier.ToString(access_mod)).append(" "c)
				If isDefault Then sb.append("default ")
				[mod] = ([mod] And (Not Modifier.ACCESS_MODIFIERS))
				If [mod] <> 0 Then sb.append(Modifier.ToString([mod])).append(" "c)
			End If
		End Sub

		Friend Overridable Function sharedToString(ByVal modifierMask As Integer, ByVal isDefault As Boolean, ByVal parameterTypes As Class(), ByVal exceptionTypes As Class()) As String
			Try
				Dim sb As New StringBuilder

				printModifiersIfNonzero(sb, modifierMask, isDefault)
				specificToStringHeader(sb)

				sb.append("("c)
				separateWithCommas(parameterTypes, sb)
				sb.append(")"c)
				If exceptionTypes.Length > 0 Then
					sb.append(" throws ")
					separateWithCommas(exceptionTypes, sb)
				End If
				Return sb.ToString()
			Catch e As Exception
				Return "<" & e & ">"
			End Try
		End Function

		''' <summary>
		''' Generate toString header information specific to a method or
		''' constructor.
		''' </summary>
		Friend MustOverride Sub specificToStringHeader(ByVal sb As StringBuilder)

		Friend Overridable Function sharedToGenericString(ByVal modifierMask As Integer, ByVal isDefault As Boolean) As String
			Try
				Dim sb As New StringBuilder

				printModifiersIfNonzero(sb, modifierMask, isDefault)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim typeparms As TypeVariable(Of ?)() = typeParameters
				If typeparms.Length > 0 Then
					Dim first As Boolean = True
					sb.append("<"c)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					For Each typeparm As TypeVariable(Of ?) In typeparms
						If Not first Then sb.append(","c)
						' Class objects can't occur here; no need to test
						' and call Class.getName().
						sb.append(typeparm.ToString())
						first = False
					Next typeparm
					sb.append("> ")
				End If

				specificToGenericStringHeader(sb)

				sb.append("("c)
				Dim params As Type() = genericParameterTypes
				For j As Integer = 0 To params.Length - 1
					Dim param As String = params(j).typeName
					If varArgs AndAlso (j = params.Length - 1) Then ' replace T[] with T... param = param.replaceFirst("\[\]$", "...")
					sb.append(param)
					If j < (params.Length - 1) Then sb.append(","c)
				Next j
				sb.append(")"c)
				Dim exceptions As Type() = genericExceptionTypes
				If exceptions.Length > 0 Then
					sb.append(" throws ")
					For k As Integer = 0 To exceptions.Length - 1
						sb.append(If(TypeOf exceptions(k) Is Class, CType(exceptions(k), [Class]).name, exceptions(k).ToString()))
						If k < (exceptions.Length - 1) Then sb.append(","c)
					Next k
				End If
				Return sb.ToString()
			Catch e As Exception
				Return "<" & e & ">"
			End Try
		End Function

		''' <summary>
		''' Generate toGenericString header information specific to a
		''' method or constructor.
		''' </summary>
		Friend MustOverride Sub specificToGenericStringHeader(ByVal sb As StringBuilder)

		''' <summary>
		''' Returns the {@code Class} object representing the class or interface
		''' that declares the executable represented by this object.
		''' </summary>
		Public MustOverride ReadOnly Property declaringClass As Class Implements Member.getDeclaringClass

		''' <summary>
		''' Returns the name of the executable represented by this object.
		''' </summary>
		Public MustOverride ReadOnly Property name As String Implements Member.getName

		''' <summary>
		''' Returns the Java language <seealso cref="Modifier modifiers"/> for
		''' the executable represented by this object.
		''' </summary>
		Public MustOverride ReadOnly Property modifiers As Integer Implements Member.getModifiers

		''' <summary>
		''' Returns an array of {@code TypeVariable} objects that represent the
		''' type variables declared by the generic declaration represented by this
		''' {@code GenericDeclaration} object, in declaration order.  Returns an
		''' array of length 0 if the underlying generic declaration declares no type
		''' variables.
		''' </summary>
		''' <returns> an array of {@code TypeVariable} objects that represent
		'''     the type variables declared by this generic declaration </returns>
		''' <exception cref="GenericSignatureFormatError"> if the generic
		'''     signature of this generic declaration does not conform to
		'''     the format specified in
		'''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride ReadOnly Property typeParameters As TypeVariable(Of ?)()

		''' <summary>
		''' Returns an array of {@code Class} objects that represent the formal
		''' parameter types, in declaration order, of the executable
		''' represented by this object.  Returns an array of length
		''' 0 if the underlying executable takes no parameters.
		''' </summary>
		''' <returns> the parameter types for the executable this object
		''' represents </returns>
		Public MustOverride ReadOnly Property parameterTypes As Class()

		''' <summary>
		''' Returns the number of formal parameters (whether explicitly
		''' declared or implicitly declared or neither) for the executable
		''' represented by this object.
		''' 
		''' @since 1.8 </summary>
		''' <returns> The number of formal parameters for the executable this
		''' object represents </returns>
		Public Overridable Property parameterCount As Integer
			Get
				Throw New AbstractMethodError
			End Get
		End Property

		''' <summary>
		''' Returns an array of {@code Type} objects that represent the formal
		''' parameter types, in declaration order, of the executable represented by
		''' this object. Returns an array of length 0 if the
		''' underlying executable takes no parameters.
		''' 
		''' <p>If a formal parameter type is a parameterized type,
		''' the {@code Type} object returned for it must accurately reflect
		''' the actual type parameters used in the source code.
		''' 
		''' <p>If a formal parameter type is a type variable or a parameterized
		''' type, it is created. Otherwise, it is resolved.
		''' </summary>
		''' <returns> an array of {@code Type}s that represent the formal
		'''     parameter types of the underlying executable, in declaration order </returns>
		''' <exception cref="GenericSignatureFormatError">
		'''     if the generic method signature does not conform to the format
		'''     specified in
		'''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		''' <exception cref="TypeNotPresentException"> if any of the parameter
		'''     types of the underlying executable refers to a non-existent type
		'''     declaration </exception>
		''' <exception cref="MalformedParameterizedTypeException"> if any of
		'''     the underlying executable's parameter types refer to a parameterized
		'''     type that cannot be instantiated for any reason </exception>
		Public Overridable Property genericParameterTypes As Type()
			Get
				If hasGenericInformation() Then
					Return genericInfo.parameterTypes
				Else
					Return parameterTypes
				End If
			End Get
		End Property

		''' <summary>
		''' Behaves like {@code getGenericParameterTypes}, but returns type
		''' information for all parameters, including synthetic parameters.
		''' </summary>
		Friend Overridable Property allGenericParameterTypes As Type()
			Get
				Dim genericInfo_Renamed As Boolean = hasGenericInformation()
    
				' Easy case: we don't have generic parameter information.  In
				' this case, we just return the result of
				' getParameterTypes().
				If Not genericInfo_Renamed Then
					Return parameterTypes
				Else
					Dim realParamData As Boolean = hasRealParameterData()
					Dim genericParamTypes As Type() = genericParameterTypes
					Dim nonGenericParamTypes As Type() = parameterTypes
					Dim out As Type() = New Type(nonGenericParamTypes.Length - 1){}
					Dim params As Parameter() = parameters
					Dim fromidx As Integer = 0
					' If we have real parameter data, then we use the
					' synthetic and mandate flags to our advantage.
					If realParamData Then
						For i As Integer = 0 To out.Length - 1
							Dim param As Parameter = params(i)
							If param.synthetic OrElse param.implicit Then
								' If we hit a synthetic or mandated parameter,
								' use the non generic parameter info.
								out(i) = nonGenericParamTypes(i)
							Else
								' Otherwise, use the generic parameter info.
								out(i) = genericParamTypes(fromidx)
								fromidx += 1
							End If
						Next i
					Else
						' Otherwise, use the non-generic parameter data.
						' Without method parameter reflection data, we have
						' no way to figure out which parameters are
						' synthetic/mandated, thus, no way to match up the
						' indexes.
						Return If(genericParamTypes.Length = nonGenericParamTypes.Length, genericParamTypes, nonGenericParamTypes)
					End If
					Return out
				End If
			End Get
		End Property

		''' <summary>
		''' Returns an array of {@code Parameter} objects that represent
		''' all the parameters to the underlying executable represented by
		''' this object.  Returns an array of length 0 if the executable
		''' has no parameters.
		''' 
		''' <p>The parameters of the underlying executable do not necessarily
		''' have unique names, or names that are legal identifiers in the
		''' Java programming language (JLS 3.8).
		''' 
		''' @since 1.8 </summary>
		''' <exception cref="MalformedParametersException"> if the class file contains
		''' a MethodParameters attribute that is improperly formatted. </exception>
		''' <returns> an array of {@code Parameter} objects representing all
		''' the parameters to the executable this object represents. </returns>
		Public Overridable Property parameters As Parameter()
			Get
				' TODO: This may eventually need to be guarded by security
				' mechanisms similar to those in Field, Method, etc.
				'
				' Need to copy the cached array to prevent users from messing
				' with it.  Since parameters are immutable, we can
				' shallow-copy.
				Return privateGetParameters().clone()
			End Get
		End Property

		Private Function synthesizeAllParams() As Parameter()
			Dim realparams As Integer = parameterCount
			Dim out As Parameter() = New Parameter(realparams - 1){}
			For i As Integer = 0 To realparams - 1
				' TODO: is there a way to synthetically derive the
				' modifiers?  Probably not in the general case, since
				' we'd have no way of knowing about them, but there
				' may be specific cases.
				out(i) = New Parameter("arg" & i, 0, Me, i)
			Next i
			Return out
		End Function

		Private Sub verifyParameters(ByVal parameters As Parameter())
			Dim mask As Integer = Modifier.FINAL Or Modifier.SYNTHETIC Or Modifier.MANDATED

			If parameterTypes.Length <> parameters.Length Then Throw New MalformedParametersException("Wrong number of parameters in MethodParameters attribute")

			For Each parameter As Parameter In parameters
				Dim name_Renamed As String = parameter.realName
				Dim mods As Integer = parameter.modifiers

				If name_Renamed IsNot Nothing Then
					If name_Renamed.empty OrElse name_Renamed.IndexOf("."c) <> -1 OrElse name_Renamed.IndexOf(";"c) <> -1 OrElse name_Renamed.IndexOf("["c) <> -1 OrElse name_Renamed.IndexOf("/"c) <> -1 Then Throw New MalformedParametersException("Invalid parameter name """ & name_Renamed & """")
				End If

				If mods <> (mods And mask) Then Throw New MalformedParametersException("Invalid parameter modifiers")
			Next parameter
		End Sub

		Private Function privateGetParameters() As Parameter()
			' Use tmp to avoid multiple writes to a volatile.
			Dim tmp As Parameter() = parameters

			If tmp Is Nothing Then

				' Otherwise, go to the JVM to get them
				Try
					tmp = parameters0
				Catch e As IllegalArgumentException
					' Rethrow ClassFormatErrors
					Throw New MalformedParametersException("Invalid constant pool index")
				End Try

				' If we get back nothing, then synthesize parameters
				If tmp Is Nothing Then
					hasRealParameterData_Renamed = False
					tmp = synthesizeAllParams()
				Else
					hasRealParameterData_Renamed = True
					verifyParameters(tmp)
				End If

				parameters = tmp
			End If

			Return tmp
		End Function

		Friend Overridable Function hasRealParameterData() As Boolean
			' If this somehow gets called before parameters gets
			' initialized, force it into existence.
			If parameters Is Nothing Then privateGetParameters()
			Return hasRealParameterData_Renamed
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private hasRealParameterData_Renamed As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private parameters As Parameter()

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function getParameters0() As Parameter()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Function getTypeAnnotationBytes0() As SByte()
		End Function

		' Needed by reflectaccess
		Friend Overridable Property typeAnnotationBytes As SByte()
			Get
				Return typeAnnotationBytes0
			End Get
		End Property

		''' <summary>
		''' Returns an array of {@code Class} objects that represent the
		''' types of exceptions declared to be thrown by the underlying
		''' executable represented by this object.  Returns an array of
		''' length 0 if the executable declares no exceptions in its {@code
		''' throws} clause.
		''' </summary>
		''' <returns> the exception types declared as being thrown by the
		''' executable this object represents </returns>
		Public MustOverride ReadOnly Property exceptionTypes As Class()

		''' <summary>
		''' Returns an array of {@code Type} objects that represent the
		''' exceptions declared to be thrown by this executable object.
		''' Returns an array of length 0 if the underlying executable declares
		''' no exceptions in its {@code throws} clause.
		''' 
		''' <p>If an exception type is a type variable or a parameterized
		''' type, it is created. Otherwise, it is resolved.
		''' </summary>
		''' <returns> an array of Types that represent the exception types
		'''     thrown by the underlying executable </returns>
		''' <exception cref="GenericSignatureFormatError">
		'''     if the generic method signature does not conform to the format
		'''     specified in
		'''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
		''' <exception cref="TypeNotPresentException"> if the underlying executable's
		'''     {@code throws} clause refers to a non-existent type declaration </exception>
		''' <exception cref="MalformedParameterizedTypeException"> if
		'''     the underlying executable's {@code throws} clause refers to a
		'''     parameterized type that cannot be instantiated for any reason </exception>
		Public Overridable Property genericExceptionTypes As Type()
			Get
				Dim result As Type()
				result = genericInfo.exceptionTypes
				If hasGenericInformation() AndAlso (result .length > 0) Then
					Return result
				Else
					Return exceptionTypes
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a string describing this {@code Executable}, including
		''' any type parameters. </summary>
		''' <returns> a string describing this {@code Executable}, including
		''' any type parameters </returns>
		Public MustOverride Function toGenericString() As String

		''' <summary>
		''' Returns {@code true} if this executable was declared to take a
		''' variable number of arguments; returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if an only if this executable was declared
		''' to take a variable number of arguments. </returns>
		Public Overridable Property varArgs As Boolean
			Get
				Return (modifiers And Modifier.VARARGS) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this executable is a synthetic
		''' construct; returns {@code false} otherwise.
		''' </summary>
		''' <returns> true if and only if this executable is a synthetic
		''' construct as defined by
		''' <cite>The Java&trade; Language Specification</cite>.
		''' @jls 13.1 The Form of a Binary </returns>
		Public Overridable Property synthetic As Boolean Implements Member.isSynthetic
			Get
				Return Modifier.isSynthetic(modifiers)
			End Get
		End Property

		''' <summary>
		''' Returns an array of arrays of {@code Annotation}s that
		''' represent the annotations on the formal parameters, in
		''' declaration order, of the {@code Executable} represented by
		''' this object.  Synthetic and mandated parameters (see
		''' explanation below), such as the outer "this" parameter to an
		''' inner class constructor will be represented in the returned
		''' array.  If the executable has no parameters (meaning no formal,
		''' no synthetic, and no mandated parameters), a zero-length array
		''' will be returned.  If the {@code Executable} has one or more
		''' parameters, a nested array of length zero is returned for each
		''' parameter with no annotations. The annotation objects contained
		''' in the returned arrays are serializable.  The caller of this
		''' method is free to modify the returned arrays; it will have no
		''' effect on the arrays returned to other callers.
		''' 
		''' A compiler may add extra parameters that are implicitly
		''' declared in source ("mandated"), as well as parameters that
		''' are neither implicitly nor explicitly declared in source
		''' ("synthetic") to the parameter list for a method.  See {@link
		''' java.lang.reflect.Parameter} for more information.
		''' </summary>
		''' <seealso cref= java.lang.reflect.Parameter </seealso>
		''' <seealso cref= java.lang.reflect.Parameter#getAnnotations </seealso>
		''' <returns> an array of arrays that represent the annotations on
		'''    the formal and implicit parameters, in declaration order, of
		'''    the executable represented by this object </returns>
		Public MustOverride ReadOnly Property parameterAnnotations As Annotation()()

		Friend Overridable Function sharedGetParameterAnnotations(ByVal parameterTypes As Class(), ByVal parameterAnnotations As SByte()) As Annotation()()
			Dim numParameters As Integer = parameterTypes.Length
			If parameterAnnotations Is Nothing Then Return New Annotation(numParameters - 1)(0){}

			Dim result As Annotation()() = parseParameterAnnotations(parameterAnnotations)

			If result.Length <> numParameters Then handleParameterNumberMismatch(result.Length, numParameters)
			Return result
		End Function

		Friend MustOverride Sub handleParameterNumberMismatch(ByVal resultLength As Integer, ByVal numParameters As Integer)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException">  {@inheritDoc} </exception>
		Public Overrides Function getAnnotation(Of T As Annotation)(ByVal annotationClass As Class) As T Implements AnnotatedElement.getAnnotation
			java.util.Objects.requireNonNull(annotationClass)
			Return annotationClass.cast(declaredAnnotations().get(annotationClass))
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getAnnotationsByType(Of T As Annotation)(ByVal annotationClass As Class) As T() Implements AnnotatedElement.getAnnotationsByType
			java.util.Objects.requireNonNull(annotationClass)

			Return sun.reflect.annotation.AnnotationSupport.getDirectlyAndIndirectlyPresent(declaredAnnotations(), annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides declaredAnnotations As Annotation() Implements AnnotatedElement.getDeclaredAnnotations
			Get
				Return sun.reflect.annotation.AnnotationParser.ToArray(declaredAnnotations())
			End Get
		End Property

		<NonSerialized> _
		Private declaredAnnotations_Renamed As IDictionary(Of [Class], Annotation)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function declaredAnnotations() As IDictionary(Of [Class], Annotation)
			If declaredAnnotations_Renamed Is Nothing Then
				Dim root_Renamed As Executable = root
				If root_Renamed IsNot Nothing Then
					declaredAnnotations_Renamed = root_Renamed.declaredAnnotations()
				Else
					declaredAnnotations_Renamed = sun.reflect.annotation.AnnotationParser.parseAnnotations(annotationBytes, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), declaringClass)
				End If
			End If
			Return declaredAnnotations_Renamed
		End Function

		''' <summary>
		''' Returns an {@code AnnotatedType} object that represents the use of a type to
		''' specify the return type of the method/constructor represented by this
		''' Executable.
		''' 
		''' If this {@code Executable} object represents a constructor, the {@code
		''' AnnotatedType} object represents the type of the constructed object.
		''' 
		''' If this {@code Executable} object represents a method, the {@code
		''' AnnotatedType} object represents the use of a type to specify the return
		''' type of the method.
		''' </summary>
		''' <returns> an object representing the return type of the method
		''' or constructor represented by this {@code Executable}
		''' 
		''' @since 1.8 </returns>
		Public MustOverride ReadOnly Property annotatedReturnType As AnnotatedType

	'     Helper for subclasses of Executable.
	'     *
	'     * Returns an AnnotatedType object that represents the use of a type to
	'     * specify the return type of the method/constructor represented by this
	'     * Executable.
	'     *
	'     * @since 1.8
	'     
		Friend Overridable Function getAnnotatedReturnType0(ByVal returnType As Type) As AnnotatedType
			Return sun.reflect.annotation.TypeAnnotationParser.buildAnnotatedType(typeAnnotationBytes0, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), Me, declaringClass, returnType, sun.reflect.annotation.TypeAnnotation.TypeAnnotationTarget.METHOD_RETURN)
		End Function

		''' <summary>
		''' Returns an {@code AnnotatedType} object that represents the use of a
		''' type to specify the receiver type of the method/constructor represented
		''' by this Executable object. The receiver type of a method/constructor is
		''' available only if the method/constructor has a <em>receiver
		''' parameter</em> (JLS 8.4.1).
		''' 
		''' If this {@code Executable} object represents a constructor or instance
		''' method that does not have a receiver parameter, or has a receiver
		''' parameter with no annotations on its type, then the return value is an
		''' {@code AnnotatedType} object representing an element with no
		''' annotations.
		''' 
		''' If this {@code Executable} object represents a static method, then the
		''' return value is null.
		''' </summary>
		''' <returns> an object representing the receiver type of the method or
		''' constructor represented by this {@code Executable}
		''' 
		''' @since 1.8 </returns>
		Public Overridable Property annotatedReceiverType As AnnotatedType
			Get
				If Modifier.isStatic(Me.modifiers) Then Return Nothing
				Return sun.reflect.annotation.TypeAnnotationParser.buildAnnotatedType(typeAnnotationBytes0, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), Me, declaringClass, declaringClass, sun.reflect.annotation.TypeAnnotation.TypeAnnotationTarget.METHOD_RECEIVER)
			End Get
		End Property

		''' <summary>
		''' Returns an array of {@code AnnotatedType} objects that represent the use
		''' of types to specify formal parameter types of the method/constructor
		''' represented by this Executable. The order of the objects in the array
		''' corresponds to the order of the formal parameter types in the
		''' declaration of the method/constructor.
		''' 
		''' Returns an array of length 0 if the method/constructor declares no
		''' parameters.
		''' </summary>
		''' <returns> an array of objects representing the types of the
		''' formal parameters of the method or constructor represented by this
		''' {@code Executable}
		''' 
		''' @since 1.8 </returns>
		Public Overridable Property annotatedParameterTypes As AnnotatedType()
			Get
				Return sun.reflect.annotation.TypeAnnotationParser.buildAnnotatedTypes(typeAnnotationBytes0, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), Me, declaringClass, allGenericParameterTypes, sun.reflect.annotation.TypeAnnotation.TypeAnnotationTarget.METHOD_FORMAL_PARAMETER)
			End Get
		End Property

		''' <summary>
		''' Returns an array of {@code AnnotatedType} objects that represent the use
		''' of types to specify the declared exceptions of the method/constructor
		''' represented by this Executable. The order of the objects in the array
		''' corresponds to the order of the exception types in the declaration of
		''' the method/constructor.
		''' 
		''' Returns an array of length 0 if the method/constructor declares no
		''' exceptions.
		''' </summary>
		''' <returns> an array of objects representing the declared
		''' exceptions of the method or constructor represented by this {@code
		''' Executable}
		''' 
		''' @since 1.8 </returns>
		Public Overridable Property annotatedExceptionTypes As AnnotatedType()
			Get
				Return sun.reflect.annotation.TypeAnnotationParser.buildAnnotatedTypes(typeAnnotationBytes0, sun.misc.SharedSecrets.javaLangAccess.getConstantPool(declaringClass), Me, declaringClass, genericExceptionTypes, sun.reflect.annotation.TypeAnnotation.TypeAnnotationTarget.THROWS)
			End Get
		End Property

	End Class

End Namespace
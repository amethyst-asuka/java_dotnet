Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Information about method parameters.
	''' 
	''' A {@code Parameter} provides information about method parameters,
	''' including its name and modifiers.  It also provides an alternate
	''' means of obtaining attributes for the parameter.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class Parameter
		Implements AnnotatedElement

		Private ReadOnly name As String
		Private ReadOnly modifiers As Integer
		Private ReadOnly executable As Executable
		Private ReadOnly index As Integer

		''' <summary>
		''' Package-private constructor for {@code Parameter}.
		''' 
		''' If method parameter data is present in the classfile, then the
		''' JVM creates {@code Parameter} objects directly.  If it is
		''' absent, however, then {@code Executable} uses this constructor
		''' to synthesize them.
		''' </summary>
		''' <param name="name"> The name of the parameter. </param>
		''' <param name="modifiers"> The modifier flags for the parameter. </param>
		''' <param name="executable"> The executable which defines this parameter. </param>
		''' <param name="index"> The index of the parameter. </param>
		Friend Sub New(ByVal name As String, ByVal modifiers As Integer, ByVal executable As Executable, ByVal index As Integer)
			Me.name = name
			Me.modifiers = modifiers
			Me.executable = executable
			Me.index = index
		End Sub

		''' <summary>
		''' Compares based on the executable and the index.
		''' </summary>
		''' <param name="obj"> The object to compare. </param>
		''' <returns> Whether or not this is equal to the argument. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is Parameter Then
				Dim other As Parameter = CType(obj, Parameter)
				Return (other.executable.Equals(executable) AndAlso other.index = index)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code based on the executable's hash code and the
		''' index.
		''' </summary>
		''' <returns> A hash code based on the executable's hash code. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return executable.GetHashCode() Xor index
		End Function

		''' <summary>
		''' Returns true if the parameter has a name according to the class
		''' file; returns false otherwise. Whether a parameter has a name
		''' is determined by the {@literal MethodParameters} attribute of
		''' the method which declares the parameter.
		''' </summary>
		''' <returns> true if and only if the parameter has a name according
		''' to the class file. </returns>
		Public Property namePresent As Boolean
			Get
				Return executable.hasRealParameterData() AndAlso name IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Returns a string describing this parameter.  The format is the
		''' modifiers for the parameter, if any, in canonical order as
		''' recommended by <cite>The Java&trade; Language
		''' Specification</cite>, followed by the fully- qualified type of
		''' the parameter (excluding the last [] if the parameter is
		''' variable arity), followed by "..." if the parameter is variable
		''' arity, followed by a space, followed by the name of the
		''' parameter.
		''' </summary>
		''' <returns> A string representation of the parameter and associated
		''' information. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			Dim type_Renamed As Type = parameterizedType
			Dim typename As String = type_Renamed.typeName

			sb.append(Modifier.ToString(modifiers))

			If 0 <> modifiers Then sb.append(" "c)

			If varArgs Then
				sb.append(typename.replaceFirst("\[\]$", "..."))
			Else
				sb.append(typename)
			End If

			sb.append(" "c)
			sb.append(name)

			Return sb.ToString()
		End Function

		''' <summary>
		''' Return the {@code Executable} which declares this parameter.
		''' </summary>
		''' <returns> The {@code Executable} declaring this parameter. </returns>
		Public Property declaringExecutable As Executable
			Get
				Return executable
			End Get
		End Property

		''' <summary>
		''' Get the modifier flags for this the parameter represented by
		''' this {@code Parameter} object.
		''' </summary>
		''' <returns> The modifier flags for this parameter. </returns>
		Public Property modifiers As Integer
			Get
				Return modifiers
			End Get
		End Property

		''' <summary>
		''' Returns the name of the parameter.  If the parameter's name is
		''' <seealso cref="#isNamePresent() present"/>, then this method returns
		''' the name provided by the class file. Otherwise, this method
		''' synthesizes a name of the form argN, where N is the index of
		''' the parameter in the descriptor of the method which declares
		''' the parameter.
		''' </summary>
		''' <returns> The name of the parameter, either provided by the class
		'''         file or synthesized if the class file does not provide
		'''         a name. </returns>
		Public Property name As String
			Get
				' Note: empty strings as paramete names are now outlawed.
				' The .equals("") is for compatibility with current JVM
				' behavior.  It may be removed at some point.
				If name Is Nothing OrElse name.Equals("") Then
					Return "arg" & index
				Else
					Return name
				End If
			End Get
		End Property

		' Package-private accessor to the real name field.
		Friend Property realName As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns a {@code Type} object that identifies the parameterized
		''' type for the parameter represented by this {@code Parameter}
		''' object.
		''' </summary>
		''' <returns> a {@code Type} object identifying the parameterized
		''' type of the parameter represented by this object </returns>
		Public Property parameterizedType As Type
			Get
				Dim tmp As Type = parameterTypeCache
				If Nothing Is tmp Then
					tmp = executable.allGenericParameterTypes(index)
					parameterTypeCache = tmp
				End If
    
				Return tmp
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private parameterTypeCache As Type = Nothing

		''' <summary>
		''' Returns a {@code Class} object that identifies the
		''' declared type for the parameter represented by this
		''' {@code Parameter} object.
		''' </summary>
		''' <returns> a {@code Class} object identifying the declared
		''' type of the parameter represented by this object </returns>
		Public Property type As Class
			Get
				Dim tmp As Class = parameterClassCache
				If Nothing Is tmp Then
					tmp = executable.parameterTypes(index)
					parameterClassCache = tmp
				End If
				Return tmp
			End Get
		End Property

		''' <summary>
		''' Returns an AnnotatedType object that represents the use of a type to
		''' specify the type of the formal parameter represented by this Parameter.
		''' </summary>
		''' <returns> an {@code AnnotatedType} object representing the use of a type
		'''         to specify the type of the formal parameter represented by this
		'''         Parameter </returns>
		Public Property annotatedType As AnnotatedType
			Get
				' no caching for now
				Return executable.annotatedParameterTypes(index)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private parameterClassCache As Class = Nothing

		''' <summary>
		''' Returns {@code true} if this parameter is implicitly declared
		''' in source code; returns {@code false} otherwise.
		''' </summary>
		''' <returns> true if and only if this parameter is implicitly
		''' declared as defined by <cite>The Java&trade; Language
		''' Specification</cite>. </returns>
		Public Property implicit As Boolean
			Get
				Return Modifier.isMandated(modifiers)
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this parameter is neither implicitly
		''' nor explicitly declared in source code; returns {@code false}
		''' otherwise.
		''' 
		''' @jls 13.1 The Form of a Binary </summary>
		''' <returns> true if and only if this parameter is a synthetic
		''' construct as defined by
		''' <cite>The Java&trade; Language Specification</cite>. </returns>
		Public Property synthetic As Boolean
			Get
				Return Modifier.isSynthetic(modifiers)
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this parameter represents a variable
		''' argument list; returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if an only if this parameter represents a
		''' variable argument list. </returns>
		Public Property varArgs As Boolean
			Get
				Return executable.varArgs AndAlso index = executable.parameterCount - 1
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Function getAnnotation(Of T As Annotation)(ByVal annotationClass As Class) As T Implements AnnotatedElement.getAnnotation
			java.util.Objects.requireNonNull(annotationClass)
			Return annotationClass.cast(declaredAnnotations().get(annotationClass))
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overrides Function getAnnotationsByType(Of T As Annotation)(ByVal annotationClass As Class) As T() Implements AnnotatedElement.getAnnotationsByType
			java.util.Objects.requireNonNull(annotationClass)

			Return sun.reflect.annotation.AnnotationSupport.getDirectlyAndIndirectlyPresent(declaredAnnotations(), annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property declaredAnnotations As Annotation() Implements AnnotatedElement.getDeclaredAnnotations
			Get
				Return executable.parameterAnnotations(index)
			End Get
		End Property

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Function getDeclaredAnnotation(Of T As Annotation)(ByVal annotationClass As Class) As T Implements AnnotatedElement.getDeclaredAnnotation
			' Only annotations on classes are inherited, for all other
			' objects getDeclaredAnnotation is the same as
			' getAnnotation.
			Return getAnnotation(annotationClass)
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overrides Function getDeclaredAnnotationsByType(Of T As Annotation)(ByVal annotationClass As Class) As T() Implements AnnotatedElement.getDeclaredAnnotationsByType
			' Only annotations on classes are inherited, for all other
			' objects getDeclaredAnnotations is the same as
			' getAnnotations.
			Return getAnnotationsByType(annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property annotations As Annotation() Implements AnnotatedElement.getAnnotations
			Get
				Return declaredAnnotations
			End Get
		End Property

		<NonSerialized> _
		Private declaredAnnotations_Renamed As IDictionary(Of [Class], Annotation)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function declaredAnnotations() As IDictionary(Of [Class], Annotation)
			If Nothing Is declaredAnnotations_Renamed Then
				declaredAnnotations_Renamed = New Dictionary(Of [Class], Annotation)
				Dim ann As Annotation() = declaredAnnotations
				For i As Integer = 0 To ann.Length - 1
					declaredAnnotations_Renamed(ann(i).annotationType()) = ann(i)
				Next i
			End If
			Return declaredAnnotations_Renamed
		End Function

	End Class

End Namespace
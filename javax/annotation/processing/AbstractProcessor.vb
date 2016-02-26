Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports javax.lang.model.element

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation.processing


	''' <summary>
	''' An abstract annotation processor designed to be a convenient
	''' superclass for most concrete annotation processors.  This class
	''' examines annotation values to compute the {@linkplain
	''' #getSupportedOptions options}, {@linkplain
	''' #getSupportedAnnotationTypes annotation types}, and {@linkplain
	''' #getSupportedSourceVersion source version} supported by its
	''' subtypes.
	''' 
	''' <p>The getter methods may {@link Messager#printMessage issue
	''' warnings} about noteworthy conditions using the facilities available
	''' after the processor has been {@link #isInitialized
	''' initialized}.
	''' 
	''' <p>Subclasses are free to override the implementation and
	''' specification of any of the methods in this class as long as the
	''' general <seealso cref="javax.annotation.processing.Processor Processor"/>
	''' contract for that method is obeyed.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public MustInherit Class AbstractProcessor
		Implements Processor

		''' <summary>
		''' Processing environment providing by the tool framework.
		''' </summary>
		Protected Friend processingEnv As ProcessingEnvironment
		Private initialized As Boolean = False

		''' <summary>
		''' Constructor for subclasses to call.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' If the processor class is annotated with {@link
		''' SupportedOptions}, return an unmodifiable set with the same set
		''' of strings as the annotation.  If the class is not so
		''' annotated, an empty set is returned.
		''' </summary>
		''' <returns> the options recognized by this processor, or an empty
		''' set if none </returns>
		readonly Public Overridable Property supportedOptions As java.util.Set(Of String) Implements Processor.getSupportedOptions
			Get
				Dim so As SupportedOptions = Me.GetType().getAnnotation(GetType(SupportedOptions))
				If so Is Nothing Then
					Return java.util.Collections.emptySet()
				Else
					Return arrayToSet(so.value())
				End If
			End Get
		End Property

		''' <summary>
		''' If the processor class is annotated with {@link
		''' SupportedAnnotationTypes}, return an unmodifiable set with the
		''' same set of strings as the annotation.  If the class is not so
		''' annotated, an empty set is returned.
		''' </summary>
		''' <returns> the names of the annotation types supported by this
		''' processor, or an empty set if none </returns>
		 readonly Public Overridable Property supportedAnnotationTypes As java.util.Set(Of String) Implements Processor.getSupportedAnnotationTypes
			Get
					Dim sat As SupportedAnnotationTypes = Me.GetType().getAnnotation(GetType(SupportedAnnotationTypes))
					If sat Is Nothing Then
						If initialized Then processingEnv.messager.printMessage(javax.tools.Diagnostic.Kind.WARNING, "No SupportedAnnotationTypes annotation " & "found on " & Me.GetType().name & ", returning an empty set.")
						Return java.util.Collections.emptySet()
					Else
						Return arrayToSet(sat.value())
					End If
			End Get
		End Property

		''' <summary>
		''' If the processor class is annotated with {@link
		''' SupportedSourceVersion}, return the source version in the
		''' annotation.  If the class is not so annotated, {@link
		''' SourceVersion#RELEASE_6} is returned.
		''' </summary>
		''' <returns> the latest source version supported by this processor </returns>
		Public Overridable  readonly Property supportedSourceVersion As javax.lang.model.SourceVersion Implements Processor.getSupportedSourceVersion
			Get
				Dim ssv As SupportedSourceVersion = Me.GetType().getAnnotation(GetType(SupportedSourceVersion))
				Dim sv As javax.lang.model.SourceVersion = Nothing
				If ssv Is Nothing Then
					sv = javax.lang.model.SourceVersion.RELEASE_6
					If initialized Then processingEnv.messager.printMessage(javax.tools.Diagnostic.Kind.WARNING, "No SupportedSourceVersion annotation " & "found on " & Me.GetType().name & ", returning " & sv & ".")
				Else
					sv = ssv.value()
				End If
				Return sv
			End Get
		End Property


		''' <summary>
		''' Initializes the processor with the processing environment by
		''' setting the {@code processingEnv} field to the value of the
		''' {@code processingEnv} argument.  An {@code
		''' IllegalStateException} will be thrown if this method is called
		''' more than once on the same object.
		''' </summary>
		''' <param name="processingEnv"> environment to access facilities the tool framework
		''' provides to the processor </param>
		''' <exception cref="IllegalStateException"> if this method is called more than once. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub init(ByVal processingEnv As ProcessingEnvironment) Implements Processor.init
			If initialized Then Throw New IllegalStateException("Cannot call init more than once.")
			java.util.Objects.requireNonNull(processingEnv, "Tool provided null ProcessingEnvironment")

			Me.processingEnv = processingEnv
			initialized = True
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public MustOverride Function process(Of T1 As TypeElement)(ByVal annotations As java.util.Set(Of T1), ByVal roundEnv As RoundEnvironment) As Boolean Implements Processor.process

		''' <summary>
		''' Returns an empty iterable of completions.
		''' </summary>
		''' <param name="element"> {@inheritDoc} </param>
		''' <param name="annotation"> {@inheritDoc} </param>
		''' <param name="member"> {@inheritDoc} </param>
		''' <param name="userText"> {@inheritDoc} </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function getCompletions(ByVal element As Element, ByVal annotation As AnnotationMirror, ByVal member As ExecutableElement, ByVal userText As String) As IEnumerable(Of ? As Completion) Implements Processor.getCompletions
			Return java.util.Collections.emptyList()
		End Function

		''' <summary>
		''' Returns {@code true} if this object has been {@link #init
		''' initialized}, {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if this object has been initialized,
		''' {@code false} otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend  readonly Overridable Property initialized As Boolean
			Get
				Return initialized
			End Get
		End Property

		Private Shared Function arrayToSet(ByVal array As String()) As java.util.Set(Of String)
			Debug.Assert(array IsNot Nothing)
			Dim [set] As java.util.Set(Of String) = New HashSet(Of String)(array.Length)
			For Each s As String In array
				[set].add(s)
			Next s
			Return java.util.Collections.unmodifiableSet([set])
		End Function
	End Class

End Namespace
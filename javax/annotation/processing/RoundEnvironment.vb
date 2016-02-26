Imports System

'
' * Copyright (c) 2005, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' An annotation processing tool framework will {@linkplain
	''' Processor#process provide an annotation processor with an object
	''' implementing this interface} so that the processor can query for
	''' information about a round of annotation processing.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Interface RoundEnvironment
		''' <summary>
		''' Returns {@code true} if types generated by this round will not
		''' be subject to a subsequent round of annotation processing;
		''' returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if types generated by this round will not
		''' be subject to a subsequent round of annotation processing;
		''' returns {@code false} otherwise </returns>
		Function processingOver() As Boolean

		''' <summary>
		''' Returns {@code true} if an error was raised in the prior round
		''' of processing; returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if an error was raised in the prior round
		''' of processing; returns {@code false} otherwise </returns>
		Function errorRaised() As Boolean

		''' <summary>
		''' Returns the root elements for annotation processing generated
		''' by the prior round.
		''' </summary>
		''' <returns> the root elements for annotation processing generated
		''' by the prior round, or an empty set if there were none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property rootElements As java.util.Set(Of ? As javax.lang.model.element.Element)

		''' <summary>
		''' Returns the elements annotated with the given annotation type.
		''' The annotation may appear directly or be inherited.  Only
		''' package elements and type elements <i>included</i> in this
		''' round of annotation processing, or declarations of members,
		''' constructors, parameters, or type parameters declared within
		''' those, are returned.  Included type elements are {@linkplain
		''' #getRootElements root types} and any member types nested within
		''' them.  Elements in a package are not considered included simply
		''' because a {@code package-info} file for that package was
		''' created.
		''' </summary>
		''' <param name="a">  annotation type being requested </param>
		''' <returns> the elements annotated with the given annotation type,
		''' or an empty set if there are none </returns>
		''' <exception cref="IllegalArgumentException"> if the argument does not
		''' represent an annotation type </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getElementsAnnotatedWith(ByVal a As javax.lang.model.element.TypeElement) As java.util.Set(Of ? As javax.lang.model.element.Element)

		''' <summary>
		''' Returns the elements annotated with the given annotation type.
		''' The annotation may appear directly or be inherited.  Only
		''' package elements and type elements <i>included</i> in this
		''' round of annotation processing, or declarations of members,
		''' constructors, parameters, or type parameters declared within
		''' those, are returned.  Included type elements are {@linkplain
		''' #getRootElements root types} and any member types nested within
		''' them.  Elements in a package are not considered included simply
		''' because a {@code package-info} file for that package was
		''' created.
		''' </summary>
		''' <param name="a">  annotation type being requested </param>
		''' <returns> the elements annotated with the given annotation type,
		''' or an empty set if there are none </returns>
		''' <exception cref="IllegalArgumentException"> if the argument does not
		''' represent an annotation type </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getElementsAnnotatedWith(ByVal a As Type) As java.util.Set(Of ? As javax.lang.model.element.Element)
	End Interface

End Namespace
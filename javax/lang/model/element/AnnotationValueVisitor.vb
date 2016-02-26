Imports System.Collections.Generic

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

Namespace javax.lang.model.element





	''' <summary>
	''' A visitor of the values of annotation type elements, using a
	''' variant of the visitor design pattern.  Unlike a standard visitor
	''' which dispatches based on the concrete type of a member of a type
	''' hierarchy, this visitor dispatches based on the type of data
	''' stored; there are no distinct subclasses for storing, for example,
	''' {@code boolean} values versus {@code int} values.  Classes
	''' implementing this interface are used to operate on a value when the
	''' type of that value is unknown at compile time.  When a visitor is
	''' passed to a value's <seealso cref="AnnotationValue#accept accept"/> method,
	''' the <tt>visit<i>XYZ</i></tt> method applicable to that value is
	''' invoked.
	''' 
	''' <p> Classes implementing this interface may or may not throw a
	''' {@code NullPointerException} if the additional parameter {@code p}
	''' is {@code null}; see documentation of the implementing class for
	''' details.
	''' 
	''' <p> <b>WARNING:</b> It is possible that methods will be added to
	''' this interface to accommodate new, currently unknown, language
	''' structures added to future versions of the Java&trade; programming
	''' language.  Therefore, visitor classes directly implementing this
	''' interface may be source incompatible with future versions of the
	''' platform.  To avoid this source incompatibility, visitor
	''' implementations are encouraged to instead extend the appropriate
	''' abstract visitor class that implements this interface.  However, an
	''' API should generally use this visitor interface as the type for
	''' parameters, return type, etc. rather than one of the abstract
	''' classes.
	''' 
	''' <p>Note that methods to accommodate new language constructs could
	''' be added in a source <em>compatible</em> way if they were added as
	''' <em>default methods</em>.  However, default methods are only
	''' available on Java SE 8 and higher releases and the {@code
	''' javax.lang.model.*} packages bundled in Java SE 8 are required to
	''' also be runnable on Java SE 7.  Therefore, default methods
	''' <em>cannot</em> be used when extending {@code javax.lang.model.*}
	''' to cover Java SE 8 language features.  However, default methods may
	''' be used in subsequent revisions of the {@code javax.lang.model.*}
	''' packages that are only required to run on Java SE 8 and higher
	''' platform versions.
	''' </summary>
	''' @param <R> the return type of this visitor's methods </param>
	''' @param <P> the type of the additional parameter to this visitor's methods.
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public Interface AnnotationValueVisitor(Of R, P)
		''' <summary>
		''' Visits an annotation value. </summary>
		''' <param name="av"> the value to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visit(ByVal av As AnnotationValue, ByVal p As P) As R

		''' <summary>
		''' A convenience method equivalent to {@code v.visit(av, null)}. </summary>
		''' <param name="av"> the value to visit </param>
		''' <returns>  a visitor-specified result </returns>
		Function visit(ByVal av As AnnotationValue) As R

		''' <summary>
		''' Visits a {@code boolean} value in an annotation. </summary>
		''' <param name="b"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitBoolean(ByVal b As Boolean, ByVal p As P) As R

		''' <summary>
		''' Visits a {@code byte} value in an annotation. </summary>
		''' <param name="b"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitByte(ByVal b As SByte, ByVal p As P) As R

		''' <summary>
		''' Visits a {@code char} value in an annotation. </summary>
		''' <param name="c"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitChar(ByVal c As Char, ByVal p As P) As R

		''' <summary>
		''' Visits a {@code double} value in an annotation. </summary>
		''' <param name="d"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitDouble(ByVal d As Double, ByVal p As P) As R

		''' <summary>
		''' Visits a {@code float} value in an annotation. </summary>
		''' <param name="f"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitFloat(ByVal f As Single, ByVal p As P) As R

		''' <summary>
		''' Visits an {@code int} value in an annotation. </summary>
		''' <param name="i"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitInt(ByVal i As Integer, ByVal p As P) As R

		''' <summary>
		''' Visits a {@code long} value in an annotation. </summary>
		''' <param name="i"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitLong(ByVal i As Long, ByVal p As P) As R

		''' <summary>
		''' Visits a {@code short} value in an annotation. </summary>
		''' <param name="s"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitShort(ByVal s As Short, ByVal p As P) As R

		''' <summary>
		''' Visits a string value in an annotation. </summary>
		''' <param name="s"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitString(ByVal s As String, ByVal p As P) As R

		''' <summary>
		''' Visits a type value in an annotation. </summary>
		''' <param name="t"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitType(ByVal t As javax.lang.model.type.TypeMirror, ByVal p As P) As R

		''' <summary>
		''' Visits an {@code enum} value in an annotation. </summary>
		''' <param name="c"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitEnumConstant(ByVal c As VariableElement, ByVal p As P) As R

		''' <summary>
		''' Visits an annotation value in an annotation. </summary>
		''' <param name="a"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitAnnotation(ByVal a As AnnotationMirror, ByVal p As P) As R

		''' <summary>
		''' Visits an array value in an annotation. </summary>
		''' <param name="vals"> the value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		Function visitArray(Of T1 As AnnotationValue)(ByVal vals As IList(Of T1), ByVal p As P) As R

		''' <summary>
		''' Visits an unknown kind of annotation value.
		''' This can occur if the language evolves and new kinds
		''' of value can be stored in an annotation. </summary>
		''' <param name="av"> the unknown value being visited </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> the result of the visit </returns>
		''' <exception cref="UnknownAnnotationValueException">
		'''  a visitor implementation may optionally throw this exception </exception>
		Function visitUnknown(ByVal av As AnnotationValue, ByVal p As P) As R
	End Interface

End Namespace
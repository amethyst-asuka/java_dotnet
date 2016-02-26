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

Namespace javax.lang.model.type

	''' <summary>
	''' A visitor of types, in the style of the
	''' visitor design pattern.  Classes implementing this
	''' interface are used to operate on a type when the kind of
	''' type is unknown at compile time.  When a visitor is passed to a
	''' type's <seealso cref="TypeMirror#accept accept"/> method, the <tt>visit<i>XYZ</i></tt>
	''' method most applicable to that type is invoked.
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
	''' @param <R> the return type of this visitor's methods.  Use {@link
	'''            Void} for visitors that do not need to return results. </param>
	''' @param <P> the type of the additional parameter to this visitor's
	'''            methods.  Use {@code Void} for visitors that do not need an
	'''            additional parameter.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public Interface TypeVisitor(Of R, P)
		''' <summary>
		''' Visits a type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visit(ByVal t As TypeMirror, ByVal p As P) As R

		''' <summary>
		''' A convenience method equivalent to {@code v.visit(t, null)}. </summary>
		''' <param name="t"> the element to visit </param>
		''' <returns>  a visitor-specified result </returns>
		Function visit(ByVal t As TypeMirror) As R

		''' <summary>
		''' Visits a primitive type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitPrimitive(ByVal t As PrimitiveType, ByVal p As P) As R

		''' <summary>
		''' Visits the null type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitNull(ByVal t As NullType, ByVal p As P) As R

		''' <summary>
		''' Visits an array type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitArray(ByVal t As ArrayType, ByVal p As P) As R

		''' <summary>
		''' Visits a declared type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitDeclared(ByVal t As DeclaredType, ByVal p As P) As R

		''' <summary>
		''' Visits an error type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitError(ByVal t As ErrorType, ByVal p As P) As R

		''' <summary>
		''' Visits a type variable. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitTypeVariable(ByVal t As TypeVariable, ByVal p As P) As R

		''' <summary>
		''' Visits a wildcard type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitWildcard(ByVal t As WildcardType, ByVal p As P) As R

		''' <summary>
		''' Visits an executable type. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitExecutable(ByVal t As ExecutableType, ByVal p As P) As R

		''' <summary>
		''' Visits a <seealso cref="NoType"/> instance. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		Function visitNoType(ByVal t As NoType, ByVal p As P) As R

		''' <summary>
		''' Visits an unknown kind of type.
		''' This can occur if the language evolves and new kinds
		''' of types are added to the {@code TypeMirror} hierarchy. </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result </returns>
		''' <exception cref="UnknownTypeException">
		'''  a visitor implementation may optionally throw this exception </exception>
		Function visitUnknown(ByVal t As TypeMirror, ByVal p As P) As R

		''' <summary>
		''' Visits a union type.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result
		''' @since 1.7 </returns>
		Function visitUnion(ByVal t As UnionType, ByVal p As P) As R

		''' <summary>
		''' Visits an intersection type.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  a visitor-specified result
		''' @since 1.8 </returns>
		Function visitIntersection(ByVal t As IntersectionType, ByVal p As P) As R
	End Interface

End Namespace
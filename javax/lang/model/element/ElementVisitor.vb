Imports javax.lang.model.util

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
	''' A visitor of program elements, in the style of the visitor design
	''' pattern.  Classes implementing this interface are used to operate
	''' on an element when the kind of element is unknown at compile time.
	''' When a visitor is passed to an element's {@link Element#accept
	''' accept} method, the <tt>visit<i>XYZ</i></tt> method most applicable
	''' to that element is invoked.
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
	''' @author Peter von der Ah&eacute; </param>
	''' <seealso cref= AbstractElementVisitor6 </seealso>
	''' <seealso cref= AbstractElementVisitor7
	''' @since 1.6 </seealso>
	Public Interface ElementVisitor(Of R, P)
		''' <summary>
		''' Visits an element. </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Function visit(ByVal e As Element, ByVal p As P) As R

		''' <summary>
		''' A convenience method equivalent to {@code v.visit(e, null)}. </summary>
		''' <param name="e">  the element to visit </param>
		''' <returns> a visitor-specified result </returns>
		Function visit(ByVal e As Element) As R

		''' <summary>
		''' Visits a package element. </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Function visitPackage(ByVal e As PackageElement, ByVal p As P) As R

		''' <summary>
		''' Visits a type element. </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Function visitType(ByVal e As TypeElement, ByVal p As P) As R

		''' <summary>
		''' Visits a variable element. </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Function visitVariable(ByVal e As VariableElement, ByVal p As P) As R

		''' <summary>
		''' Visits an executable element. </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Function visitExecutable(ByVal e As ExecutableElement, ByVal p As P) As R

		''' <summary>
		''' Visits a type parameter element. </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Function visitTypeParameter(ByVal e As TypeParameterElement, ByVal p As P) As R

		''' <summary>
		''' Visits an unknown kind of element.
		''' This can occur if the language evolves and new kinds
		''' of elements are added to the {@code Element} hierarchy.
		''' </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		''' <exception cref="UnknownElementException">
		'''  a visitor implementation may optionally throw this exception </exception>
		Function visitUnknown(ByVal e As Element, ByVal p As P) As R
	End Interface

End Namespace
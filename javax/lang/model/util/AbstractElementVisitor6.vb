Imports javax.lang.model.element
Imports javax.lang.model.SourceVersion

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

Namespace javax.lang.model.util


	''' <summary>
	''' A skeletal visitor of program elements with default behavior
	''' appropriate for the <seealso cref="SourceVersion#RELEASE_6 RELEASE_6"/>
	''' source version.
	''' 
	''' <p> <b>WARNING:</b> The {@code ElementVisitor} interface
	''' implemented by this class may have methods added to it in the
	''' future to accommodate new, currently unknown, language structures
	''' added to future versions of the Java&trade; programming language.
	''' Therefore, methods whose names begin with {@code "visit"} may be
	''' added to this class in the future; to avoid incompatibilities,
	''' classes which extend this class should not declare any instance
	''' methods with names beginning with {@code "visit"}.
	''' 
	''' <p>When such a new visit method is added, the default
	''' implementation in this class will be to call the {@link
	''' #visitUnknown visitUnknown} method.  A new abstract element visitor
	''' class will also be introduced to correspond to the new language
	''' level; this visitor will have different default behavior for the
	''' visit method in question.  When the new visitor is introduced, all
	''' or portions of this visitor may be deprecated.
	''' 
	''' <p>Note that adding a default implementation of a new visit method
	''' in a visitor class will occur instead of adding a <em>default
	''' method</em> directly in the visitor interface since a Java SE 8
	''' language feature cannot be used to this version of the API since
	''' this version is required to be runnable on Java SE 7
	''' implementations.  Future versions of the API that are only required
	''' to run on Java SE 8 and later may take advantage of default methods
	''' in this situation.
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
	''' </param>
	''' <seealso cref= AbstractElementVisitor7 </seealso>
	''' <seealso cref= AbstractElementVisitor8
	''' @since 1.6 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public MustInherit Class AbstractElementVisitor6(Of R, P)
		Implements ElementVisitor(Of R, P)

		''' <summary>
		''' Constructor for concrete subclasses to call.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Visits any program element as if by passing itself to that
		''' element's <seealso cref="Element#accept accept"/> method.  The invocation
		''' {@code v.visit(elem)} is equivalent to {@code elem.accept(v,
		''' p)}.
		''' </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		Public Function visit(ByVal e As Element, ByVal p As P) As R Implements ElementVisitor(Of R, P).visit
			Return e.accept(Me, p)
		End Function

		''' <summary>
		''' Visits any program element as if by passing itself to that
		''' element's <seealso cref="Element#accept accept"/> method and passing
		''' {@code null} for the additional parameter.  The invocation
		''' {@code v.visit(elem)} is equivalent to {@code elem.accept(v,
		''' null)}.
		''' </summary>
		''' <param name="e">  the element to visit </param>
		''' <returns> a visitor-specified result </returns>
		Public Function visit(ByVal e As Element) As R Implements ElementVisitor(Of R, P).visit
			Return e.accept(Me, Nothing)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p> The default implementation of this method in
		''' {@code AbstractElementVisitor6} will always throw
		''' {@code UnknownElementException}.
		''' This behavior is not required of a subclass.
		''' </summary>
		''' <param name="e">  the element to visit </param>
		''' <param name="p">  a visitor-specified parameter </param>
		''' <returns> a visitor-specified result </returns>
		''' <exception cref="UnknownElementException">
		'''          a visitor implementation may optionally throw this exception </exception>
		Public Overridable Function visitUnknown(ByVal e As Element, ByVal p As P) As R Implements ElementVisitor(Of R, P).visitUnknown
			Throw New UnknownElementException(e, p)
		End Function
	End Class

End Namespace
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
	''' A scanning visitor of program elements with default behavior
	''' appropriate for the <seealso cref="SourceVersion#RELEASE_6 RELEASE_6"/>
	''' source version.  The <tt>visit<i>XYZ</i></tt> methods in this
	''' class scan their component elements by calling {@code scan} on
	''' their <seealso cref="Element#getEnclosedElements enclosed elements"/>,
	''' <seealso cref="ExecutableElement#getParameters parameters"/>, etc., as
	''' indicated in the individual method specifications.  A subclass can
	''' control the order elements are visited by overriding the
	''' <tt>visit<i>XYZ</i></tt> methods.  Note that clients of a scanner
	''' may get the desired behavior be invoking {@code v.scan(e, p)} rather
	''' than {@code v.visit(e, p)} on the root objects of interest.
	''' 
	''' <p>When a subclass overrides a <tt>visit<i>XYZ</i></tt> method, the
	''' new method can cause the enclosed elements to be scanned in the
	''' default way by calling <tt>super.visit<i>XYZ</i></tt>.  In this
	''' fashion, the concrete visitor can control the ordering of traversal
	''' over the component elements with respect to the additional
	''' processing; for example, consistently calling
	''' <tt>super.visit<i>XYZ</i></tt> at the start of the overridden
	''' methods will yield a preorder traversal, etc.  If the component
	''' elements should be traversed in some other order, instead of
	''' calling <tt>super.visit<i>XYZ</i></tt>, an overriding visit method
	''' should call {@code scan} with the elements in the desired order.
	''' 
	''' <p> Methods in this class may be overridden subject to their
	''' general contract.  Note that annotating methods in concrete
	''' subclasses with <seealso cref="java.lang.Override @Override"/> will help
	''' ensure that methods are overridden as intended.
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
	''' #visitUnknown visitUnknown} method.  A new element scanner visitor
	''' class will also be introduced to correspond to the new language
	''' level; this visitor will have different default behavior for the
	''' visit method in question.  When the new visitor is introduced, all
	''' or portions of this visitor may be deprecated.
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
	''' <seealso cref= ElementScanner7 </seealso>
	''' <seealso cref= ElementScanner8
	''' @since 1.6 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ElementScanner6(Of R, P)
		Inherits AbstractElementVisitor6(Of R, P)

		''' <summary>
		''' The specified default value.
		''' </summary>
		Protected Friend ReadOnly DEFAULT_VALUE As R

		''' <summary>
		''' Constructor for concrete subclasses; uses {@code null} for the
		''' default value.
		''' </summary>
		Protected Friend Sub New()
			DEFAULT_VALUE = Nothing
		End Sub

		''' <summary>
		''' Constructor for concrete subclasses; uses the argument for the
		''' default value.
		''' </summary>
		''' <param name="defaultValue"> the default value </param>
		Protected Friend Sub New(ByVal defaultValue As R)
			DEFAULT_VALUE = defaultValue
		End Sub

		''' <summary>
		''' Iterates over the given elements and calls {@link
		''' #scan(Element, Object) scan(Element, P)} on each one.  Returns
		''' the result of the last call to {@code scan} or {@code
		''' DEFAULT_VALUE} for an empty iterable.
		''' </summary>
		''' <param name="iterable"> the elements to scan </param>
		''' <param name="p"> additional parameter </param>
		''' <returns> the scan of the last element or {@code DEFAULT_VALUE} if no elements </returns>
		Public Function scan(Of T1 As Element)(ByVal iterable As IEnumerable(Of T1), ByVal p As P) As R
			Dim result As R = DEFAULT_VALUE
			For Each e As Element In iterable
				result = scan(e, p)
			Next e
			Return result
		End Function

		''' <summary>
		''' Processes an element by calling {@code e.accept(this, p)};
		''' this method may be overridden by subclasses.
		''' </summary>
		''' <param name="e"> the element to scan </param>
		''' <param name="p"> a scanner-specified parameter </param>
		''' <returns> the result of visiting {@code e}. </returns>
		Public Overridable Function scan(ByVal e As Element, ByVal p As P) As R
			Return e.accept(Me, p)
		End Function

		''' <summary>
		''' Convenience method equivalent to {@code v.scan(e, null)}.
		''' </summary>
		''' <param name="e"> the element to scan </param>
		''' <returns> the result of scanning {@code e}. </returns>
		Public Function scan(ByVal e As Element) As R
			Return scan(e, Nothing)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation scans the enclosed elements.
		''' </summary>
		''' <param name="e">  {@inheritDoc} </param>
		''' <param name="p">  {@inheritDoc} </param>
		''' <returns> the result of scanning </returns>
		Public Overridable Function visitPackage(ByVal e As PackageElement, ByVal p As P) As R
			Return scan(e.enclosedElements, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation scans the enclosed elements.
		''' </summary>
		''' <param name="e">  {@inheritDoc} </param>
		''' <param name="p">  {@inheritDoc} </param>
		''' <returns> the result of scanning </returns>
		Public Overridable Function visitType(ByVal e As TypeElement, ByVal p As P) As R
			Return scan(e.enclosedElements, p)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' This implementation scans the enclosed elements, unless the
		''' element is a {@code RESOURCE_VARIABLE} in which case {@code
		''' visitUnknown} is called.
		''' </summary>
		''' <param name="e">  {@inheritDoc} </param>
		''' <param name="p">  {@inheritDoc} </param>
		''' <returns> the result of scanning </returns>
		Public Overridable Function visitVariable(ByVal e As VariableElement, ByVal p As P) As R
			If e.kind <> ElementKind.RESOURCE_VARIABLE Then
				Return scan(e.enclosedElements, p)
			Else
				Return visitUnknown(e, p)
			End If
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation scans the parameters.
		''' </summary>
		''' <param name="e">  {@inheritDoc} </param>
		''' <param name="p">  {@inheritDoc} </param>
		''' <returns> the result of scanning </returns>
		Public Overridable Function visitExecutable(ByVal e As ExecutableElement, ByVal p As P) As R
			Return scan(e.parameters, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation scans the enclosed elements.
		''' </summary>
		''' <param name="e">  {@inheritDoc} </param>
		''' <param name="p">  {@inheritDoc} </param>
		''' <returns> the result of scanning </returns>
		Public Overridable Function visitTypeParameter(ByVal e As TypeParameterElement, ByVal p As P) As R
			Return scan(e.enclosedElements, p)
		End Function
	End Class

End Namespace
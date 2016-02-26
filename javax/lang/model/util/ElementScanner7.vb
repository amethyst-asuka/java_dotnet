Imports javax.lang.model.element
Imports javax.lang.model.SourceVersion

'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' appropriate for the <seealso cref="SourceVersion#RELEASE_7 RELEASE_7"/>
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
	''' </param>
	''' <seealso cref= ElementScanner6 </seealso>
	''' <seealso cref= ElementScanner8
	''' @since 1.7 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ElementScanner7(Of R, P)
		Inherits ElementScanner6(Of R, P)

		''' <summary>
		''' Constructor for concrete subclasses; uses {@code null} for the
		''' default value.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New(Nothing)
		End Sub

		''' <summary>
		''' Constructor for concrete subclasses; uses the argument for the
		''' default value.
		''' </summary>
		''' <param name="defaultValue"> the default value </param>
		Protected Friend Sub New(ByVal defaultValue As R)
			MyBase.New(defaultValue)
		End Sub

		''' <summary>
		''' This implementation scans the enclosed elements.
		''' </summary>
		''' <param name="e">  {@inheritDoc} </param>
		''' <param name="p">  {@inheritDoc} </param>
		''' <returns> the result of scanning </returns>
		Public Overrides Function visitVariable(ByVal e As VariableElement, ByVal p As P) As R
			Return scan(e.enclosedElements, p)
		End Function
	End Class

End Namespace
Imports javax.lang.model.SourceVersion

'
' * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A skeletal visitor for annotation values with default behavior
	''' appropriate for the <seealso cref="SourceVersion#RELEASE_8 RELEASE_8"/>
	''' source version.
	''' 
	''' <p> <b>WARNING:</b> The {@code AnnotationValueVisitor} interface
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
	''' #visitUnknown visitUnknown} method.  A new abstract annotation
	''' value visitor class will also be introduced to correspond to the
	''' new language level; this visitor will have different default
	''' behavior for the visit method in question.  When the new visitor is
	''' introduced, all or portions of this visitor may be deprecated.
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
	''' @param <R> the return type of this visitor's methods </param>
	''' @param <P> the type of the additional parameter to this visitor's methods.
	''' </param>
	''' <seealso cref= AbstractAnnotationValueVisitor6 </seealso>
	''' <seealso cref= AbstractAnnotationValueVisitor7
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public MustInherit Class AbstractAnnotationValueVisitor8(Of R, P)
		Inherits AbstractAnnotationValueVisitor7(Of R, P)

		''' <summary>
		''' Constructor for concrete subclasses to call.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New()
		End Sub
	End Class

End Namespace
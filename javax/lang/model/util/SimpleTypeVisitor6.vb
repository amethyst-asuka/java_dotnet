Imports javax.lang.model.type
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
	''' A simple visitor of types with default behavior appropriate for the
	''' <seealso cref="SourceVersion#RELEASE_6 RELEASE_6"/> source version.
	''' 
	''' Visit methods corresponding to {@code RELEASE_6} language
	''' constructs call <seealso cref="#defaultAction defaultAction"/>, passing their
	''' arguments to {@code defaultAction}'s corresponding parameters.
	''' 
	''' For constructs introduced in {@code RELEASE_7} and later, {@code
	''' visitUnknown} is called instead.
	''' 
	''' <p> Methods in this class may be overridden subject to their
	''' general contract.  Note that annotating methods in concrete
	''' subclasses with <seealso cref="java.lang.Override @Override"/> will help
	''' ensure that methods are overridden as intended.
	''' 
	''' <p> <b>WARNING:</b> The {@code TypeVisitor} interface implemented
	''' by this class may have methods added to it in the future to
	''' accommodate new, currently unknown, language structures added to
	''' future versions of the Java&trade; programming language.
	''' Therefore, methods whose names begin with {@code "visit"} may be
	''' added to this class in the future; to avoid incompatibilities,
	''' classes which extend this class should not declare any instance
	''' methods with names beginning with {@code "visit"}.
	''' 
	''' <p>When such a new visit method is added, the default
	''' implementation in this class will be to call the {@link
	''' #visitUnknown visitUnknown} method.  A new simple type visitor
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
	''' <seealso cref= SimpleTypeVisitor7 </seealso>
	''' <seealso cref= SimpleTypeVisitor8
	''' @since 1.6 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class SimpleTypeVisitor6(Of R, P)
		Inherits AbstractTypeVisitor6(Of R, P)

		''' <summary>
		''' Default value to be returned; {@link #defaultAction
		''' defaultAction} returns this value unless the method is
		''' overridden.
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
		''' <param name="defaultValue"> the value to assign to <seealso cref="#DEFAULT_VALUE"/> </param>
		Protected Friend Sub New(ByVal defaultValue As R)
			DEFAULT_VALUE = defaultValue
		End Sub

		''' <summary>
		''' The default action for visit methods.  The implementation in
		''' this class just returns <seealso cref="#DEFAULT_VALUE"/>; subclasses will
		''' commonly override this method.
		''' </summary>
		''' <param name="e"> the type to process </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns> {@code DEFAULT_VALUE} unless overridden </returns>
		Protected Friend Overridable Function defaultAction(ByVal e As TypeMirror, ByVal p As P) As R
			Return DEFAULT_VALUE
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitive(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitNull(ByVal t As NullType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitArray(ByVal t As ArrayType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitDeclared(ByVal t As DeclaredType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitError(ByVal t As ErrorType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitTypeVariable(ByVal t As TypeVariable, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitWildcard(ByVal t As WildcardType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitExecutable(ByVal t As ExecutableType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' {@inheritDoc} This implementation calls {@code defaultAction}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitNoType(ByVal t As NoType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function
	End Class

End Namespace
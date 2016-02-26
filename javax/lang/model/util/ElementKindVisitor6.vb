Imports System.Diagnostics
Imports javax.lang.model.element
Imports javax.lang.model.element.ElementKind
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
	''' A visitor of program elements based on their {@linkplain
	''' ElementKind kind} with default behavior appropriate for the {@link
	''' SourceVersion#RELEASE_6 RELEASE_6} source version.  For {@linkplain
	''' Element elements} <tt><i>XYZ</i></tt> that may have more than one
	''' kind, the <tt>visit<i>XYZ</i></tt> methods in this class delegate
	''' to the <tt>visit<i>XYZKind</i></tt> method corresponding to the
	''' first argument's kind.  The <tt>visit<i>XYZKind</i></tt> methods
	''' call <seealso cref="#defaultAction defaultAction"/>, passing their arguments
	''' to {@code defaultAction}'s corresponding parameters.
	''' 
	''' <p> Methods in this class may be overridden subject to their
	''' general contract.  Note that annotating methods in concrete
	''' subclasses with <seealso cref="java.lang.Override @Override"/> will help
	''' ensure that methods are overridden as intended.
	''' 
	''' <p> <b>WARNING:</b> The {@code ElementVisitor} interface
	''' implemented by this class may have methods added to it or the
	''' {@code ElementKind} {@code enum} used in this case may have
	''' constants added to it in the future to accommodate new, currently
	''' unknown, language structures added to future versions of the
	''' Java&trade; programming language.  Therefore, methods whose names
	''' begin with {@code "visit"} may be added to this class in the
	''' future; to avoid incompatibilities, classes which extend this class
	''' should not declare any instance methods with names beginning with
	''' {@code "visit"}.
	''' 
	''' <p>When such a new visit method is added, the default
	''' implementation in this class will be to call the {@link
	''' #visitUnknown visitUnknown} method.  A new abstract element kind
	''' visitor class will also be introduced to correspond to the new
	''' language level; this visitor will have different default behavior
	''' for the visit method in question.  When the new visitor is
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
	''' <seealso cref= ElementKindVisitor7 </seealso>
	''' <seealso cref= ElementKindVisitor8
	''' @since 1.6 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ElementKindVisitor6(Of R, P)
		Inherits SimpleElementVisitor6(Of R, P)

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
		''' <param name="defaultValue"> the value to assign to <seealso cref="#DEFAULT_VALUE"/> </param>
		Protected Friend Sub New(ByVal defaultValue As R)
			MyBase.New(defaultValue)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' The element argument has kind {@code PACKAGE}.
		''' </summary>
		''' <param name="e"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  {@inheritDoc} </returns>
		Public Overrides Function visitPackage(ByVal e As PackageElement, ByVal p As P) As R
			Debug.Assert(e.kind = PACKAGE, "Bad kind on PackageElement")
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a type element, dispatching to the visit method for the
		''' specific <seealso cref="ElementKind kind"/> of type, {@code
		''' ANNOTATION_TYPE}, {@code CLASS}, {@code ENUM}, or {@code
		''' INTERFACE}.
		''' </summary>
		''' <param name="e"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of the kind-specific visit method </returns>
		Public Overrides Function visitType(ByVal e As TypeElement, ByVal p As P) As R
			Dim k As ElementKind = e.kind
			Select Case k
			Case ElementKind.ANNOTATION_TYPE
				Return visitTypeAsAnnotationType(e, p)

			Case ElementKind.CLASS
				Return visitTypeAsClass(e, p)

			Case ElementKind.ENUM
				Return visitTypeAsEnum(e, p)

			Case ElementKind.INTERFACE
				Return visitTypeAsInterface(e, p)

			Case Else
				Throw New AssertionError("Bad kind " & k & " for TypeElement" & e)
			End Select
		End Function

		''' <summary>
		''' Visits an {@code ANNOTATION_TYPE} type element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitTypeAsAnnotationType(ByVal e As TypeElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code CLASS} type element by calling {@code
		''' defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitTypeAsClass(ByVal e As TypeElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits an {@code ENUM} type element by calling {@code
		''' defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitTypeAsEnum(ByVal e As TypeElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits an {@code INTERFACE} type element by calling {@code
		''' defaultAction}.
		''' . </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitTypeAsInterface(ByVal e As TypeElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a variable element, dispatching to the visit method for
		''' the specific <seealso cref="ElementKind kind"/> of variable, {@code
		''' ENUM_CONSTANT}, {@code EXCEPTION_PARAMETER}, {@code FIELD},
		''' {@code LOCAL_VARIABLE}, {@code PARAMETER}, or {@code RESOURCE_VARIABLE}.
		''' </summary>
		''' <param name="e"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of the kind-specific visit method </returns>
		Public Overrides Function visitVariable(ByVal e As VariableElement, ByVal p As P) As R
			Dim k As ElementKind = e.kind
			Select Case k
			Case ElementKind.ENUM_CONSTANT
				Return visitVariableAsEnumConstant(e, p)

			Case ElementKind.EXCEPTION_PARAMETER
				Return visitVariableAsExceptionParameter(e, p)

			Case ElementKind.FIELD
				Return visitVariableAsField(e, p)

			Case ElementKind.LOCAL_VARIABLE
				Return visitVariableAsLocalVariable(e, p)

			Case ElementKind.PARAMETER
				Return visitVariableAsParameter(e, p)

			Case ElementKind.RESOURCE_VARIABLE
				Return visitVariableAsResourceVariable(e, p)

			Case Else
				Throw New AssertionError("Bad kind " & k & " for VariableElement" & e)
			End Select
		End Function

		''' <summary>
		''' Visits an {@code ENUM_CONSTANT} variable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitVariableAsEnumConstant(ByVal e As VariableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits an {@code EXCEPTION_PARAMETER} variable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitVariableAsExceptionParameter(ByVal e As VariableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code FIELD} variable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitVariableAsField(ByVal e As VariableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code LOCAL_VARIABLE} variable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitVariableAsLocalVariable(ByVal e As VariableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code PARAMETER} variable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitVariableAsParameter(ByVal e As VariableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code RESOURCE_VARIABLE} variable element by calling
		''' {@code visitUnknown}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code visitUnknown}
		''' 
		''' @since 1.7 </returns>
		Public Overridable Function visitVariableAsResourceVariable(ByVal e As VariableElement, ByVal p As P) As R
			Return visitUnknown(e, p)
		End Function

		''' <summary>
		''' Visits an executable element, dispatching to the visit method
		''' for the specific <seealso cref="ElementKind kind"/> of executable,
		''' {@code CONSTRUCTOR}, {@code INSTANCE_INIT}, {@code METHOD}, or
		''' {@code STATIC_INIT}.
		''' </summary>
		''' <param name="e"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of the kind-specific visit method </returns>
		Public Overrides Function visitExecutable(ByVal e As ExecutableElement, ByVal p As P) As R
			Dim k As ElementKind = e.kind
			Select Case k
			Case ElementKind.CONSTRUCTOR
				Return visitExecutableAsConstructor(e, p)

			Case ElementKind.INSTANCE_INIT
				Return visitExecutableAsInstanceInit(e, p)

			Case ElementKind.METHOD
				Return visitExecutableAsMethod(e, p)

			Case ElementKind.STATIC_INIT
				Return visitExecutableAsStaticInit(e, p)

			Case Else
				Throw New AssertionError("Bad kind " & k & " for ExecutableElement" & e)
			End Select
		End Function

		''' <summary>
		''' Visits a {@code CONSTRUCTOR} executable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitExecutableAsConstructor(ByVal e As ExecutableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits an {@code INSTANCE_INIT} executable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitExecutableAsInstanceInit(ByVal e As ExecutableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code METHOD} executable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitExecutableAsMethod(ByVal e As ExecutableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function

		''' <summary>
		''' Visits a {@code STATIC_INIT} executable element by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="e"> the element to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitExecutableAsStaticInit(ByVal e As ExecutableElement, ByVal p As P) As R
			Return defaultAction(e, p)
		End Function


		''' <summary>
		''' {@inheritDoc}
		''' 
		''' The element argument has kind {@code TYPE_PARAMETER}.
		''' </summary>
		''' <param name="e"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  {@inheritDoc} </returns>
		Public Overrides Function visitTypeParameter(ByVal e As TypeParameterElement, ByVal p As P) As R
			Debug.Assert(e.kind = TYPE_PARAMETER, "Bad kind on TypeParameterElement")
			Return defaultAction(e, p)
		End Function
	End Class

End Namespace
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
	''' A visitor of types based on their <seealso cref="TypeKind kind"/> with
	''' default behavior appropriate for the {@link SourceVersion#RELEASE_6
	''' RELEASE_6} source version.  For {@linkplain
	''' TypeMirror types} <tt><i>XYZ</i></tt> that may have more than one
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
	''' #visitUnknown visitUnknown} method.  A new type kind visitor class
	''' will also be introduced to correspond to the new language level;
	''' this visitor will have different default behavior for the visit
	''' method in question.  When the new visitor is introduced, all or
	''' portions of this visitor may be deprecated.
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
	''' <seealso cref= TypeKindVisitor7 </seealso>
	''' <seealso cref= TypeKindVisitor8
	''' @since 1.6 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class TypeKindVisitor6(Of R, P)
		Inherits SimpleTypeVisitor6(Of R, P)

		''' <summary>
		''' Constructor for concrete subclasses to call; uses {@code null}
		''' for the default value.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New(Nothing)
		End Sub


		''' <summary>
		''' Constructor for concrete subclasses to call; uses the argument
		''' for the default value.
		''' </summary>
		''' <param name="defaultValue"> the value to assign to <seealso cref="#DEFAULT_VALUE"/> </param>
		Protected Friend Sub New(ByVal defaultValue As R)
			MyBase.New(defaultValue)
		End Sub

		''' <summary>
		''' Visits a primitive type, dispatching to the visit method for
		''' the specific <seealso cref="TypeKind kind"/> of primitive type:
		''' {@code BOOLEAN}, {@code BYTE}, etc.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of the kind-specific visit method </returns>
		Public Overrides Function visitPrimitive(ByVal t As PrimitiveType, ByVal p As P) As R
			Dim k As TypeKind = t.kind
			Select Case k
			Case TypeKind.BOOLEAN
				Return visitPrimitiveAsBoolean(t, p)

			Case TypeKind.BYTE
				Return visitPrimitiveAsByte(t, p)

			Case TypeKind.SHORT
				Return visitPrimitiveAsShort(t, p)

			Case TypeKind.INT
				Return visitPrimitiveAsInt(t, p)

			Case TypeKind.LONG
				Return visitPrimitiveAsLong(t, p)

			Case TypeKind.CHAR
				Return visitPrimitiveAsChar(t, p)

			Case TypeKind.FLOAT
				Return visitPrimitiveAsFloat(t, p)

			Case TypeKind.DOUBLE
				Return visitPrimitiveAsDouble(t, p)

			Case Else
				Throw New AssertionError("Bad kind " & k & " for PrimitiveType" & t)
			End Select
		End Function

		''' <summary>
		''' Visits a {@code BOOLEAN} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsBoolean(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a {@code BYTE} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsByte(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a {@code SHORT} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsShort(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits an {@code INT} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsInt(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a {@code LONG} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsLong(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a {@code CHAR} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsChar(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a {@code FLOAT} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsFloat(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a {@code DOUBLE} primitive type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitPrimitiveAsDouble(ByVal t As PrimitiveType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a <seealso cref="NoType"/> instance, dispatching to the visit method for
		''' the specific <seealso cref="TypeKind kind"/> of pseudo-type:
		''' {@code VOID}, {@code PACKAGE}, or {@code NONE}.
		''' </summary>
		''' <param name="t"> {@inheritDoc} </param>
		''' <param name="p"> {@inheritDoc} </param>
		''' <returns>  the result of the kind-specific visit method </returns>
		Public Overrides Function visitNoType(ByVal t As NoType, ByVal p As P) As R
			Dim k As TypeKind = t.kind
			Select Case k
			Case TypeKind.VOID
				Return visitNoTypeAsVoid(t, p)

			Case TypeKind.PACKAGE
				Return visitNoTypeAsPackage(t, p)

			Case TypeKind.NONE
				Return visitNoTypeAsNone(t, p)

			Case Else
				Throw New AssertionError("Bad kind " & k & " for NoType" & t)
			End Select
		End Function

		''' <summary>
		''' Visits a <seealso cref="TypeKind#VOID VOID"/> pseudo-type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitNoTypeAsVoid(ByVal t As NoType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a <seealso cref="TypeKind#PACKAGE PACKAGE"/> pseudo-type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitNoTypeAsPackage(ByVal t As NoType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function

		''' <summary>
		''' Visits a <seealso cref="TypeKind#NONE NONE"/> pseudo-type by calling
		''' {@code defaultAction}.
		''' </summary>
		''' <param name="t"> the type to visit </param>
		''' <param name="p"> a visitor-specified parameter </param>
		''' <returns>  the result of {@code defaultAction} </returns>
		Public Overridable Function visitNoTypeAsNone(ByVal t As NoType, ByVal p As P) As R
			Return defaultAction(t, p)
		End Function
	End Class

End Namespace
'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect


	''' <summary>
	''' The Modifier class provides {@code static} methods and
	''' constants to decode class and member access modifiers.  The sets of
	''' modifiers are represented as integers with distinct bit positions
	''' representing different modifiers.  The values for the constants
	''' representing the modifiers are taken from the tables in sections 4.1, 4.4, 4.5, and 4.7 of
	''' <cite>The Java&trade; Virtual Machine Specification</cite>.
	''' </summary>
	''' <seealso cref= Class#getModifiers() </seealso>
	''' <seealso cref= Member#getModifiers()
	''' 
	''' @author Nakul Saraiya
	''' @author Kenneth Russell </seealso>
	Public Class Modifier

	'    
	'     * Bootstrapping protocol between java.lang and java.lang.reflect
	'     *  packages
	'     
		Shared Sub New()
			Dim factory As sun.reflect.ReflectionFactory = java.security.AccessController.doPrivileged(New sun.reflect.ReflectionFactory.GetReflectionFactoryAction)
			factory.langReflectAccess = New java.lang.reflect.ReflectAccess
		End Sub

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code public} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code public} modifier; {@code false} otherwise. </returns>
		Public Shared Function isPublic(  [mod] As Integer) As Boolean
			Return ([mod] And [PUBLIC]) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code private} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code private} modifier; {@code false} otherwise. </returns>
		Public Shared Function isPrivate(  [mod] As Integer) As Boolean
			Return ([mod] And [PRIVATE]) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code protected} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code protected} modifier; {@code false} otherwise. </returns>
		Public Shared Function isProtected(  [mod] As Integer) As Boolean
			Return ([mod] And [PROTECTED]) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code static} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code static} modifier; {@code false} otherwise. </returns>
		Public Shared Function isStatic(  [mod] As Integer) As Boolean
			Return ([mod] And [STATIC]) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code final} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code final} modifier; {@code false} otherwise. </returns>
		Public Shared Function isFinal(  [mod] As Integer) As Boolean
			Return ([mod] And FINAL) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code synchronized} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code synchronized} modifier; {@code false} otherwise. </returns>
		Public Shared Function isSynchronized(  [mod] As Integer) As Boolean
			Return ([mod] And SYNCHRONIZED) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code volatile} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code volatile} modifier; {@code false} otherwise. </returns>
		Public Shared Function isVolatile(  [mod] As Integer) As Boolean
			Return ([mod] And VOLATILE) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code transient} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code transient} modifier; {@code false} otherwise. </returns>
		Public Shared Function isTransient(  [mod] As Integer) As Boolean
			Return ([mod] And TRANSIENT) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code native} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code native} modifier; {@code false} otherwise. </returns>
		Public Shared Function isNative(  [mod] As Integer) As Boolean
			Return ([mod] And NATIVE) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code interface} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code interface} modifier; {@code false} otherwise. </returns>
		Public Shared Function isInterface(  [mod] As Integer) As Boolean
			Return ([mod] And [INTERFACE]) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code abstract} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code abstract} modifier; {@code false} otherwise. </returns>
		Public Shared Function isAbstract(  [mod] As Integer) As Boolean
			Return ([mod] And ABSTRACT) <> 0
		End Function

		''' <summary>
		''' Return {@code true} if the integer argument includes the
		''' {@code strictfp} modifier, {@code false} otherwise.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns> {@code true} if {@code mod} includes the
		''' {@code strictfp} modifier; {@code false} otherwise. </returns>
		Public Shared Function isStrict(  [mod] As Integer) As Boolean
			Return ([mod] And [STRICT]) <> 0
		End Function

		''' <summary>
		''' Return a string describing the access modifier flags in
		''' the specified modifier. For example:
		''' <blockquote><pre>
		'''    public final synchronized strictfp
		''' </pre></blockquote>
		''' The modifier names are returned in an order consistent with the
		''' suggested modifier orderings given in sections 8.1.1, 8.3.1, 8.4.3, 8.8.3, and 9.1.1 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' The full modifier ordering used by this method is:
		''' <blockquote> {@code
		''' public protected private abstract static final transient
		''' volatile synchronized native strictfp
		''' interface } </blockquote>
		''' The {@code interface} modifier discussed in this class is
		''' not a true modifier in the Java language and it appears after
		''' all other modifiers listed by this method.  This method may
		''' return a string of modifiers that are not valid modifiers of a
		''' Java entity; in other words, no checking is done on the
		''' possible validity of the combination of modifiers represented
		''' by the input.
		''' 
		''' Note that to perform such checking for a known kind of entity,
		''' such as a constructor or method, first AND the argument of
		''' {@code toString} with the appropriate mask from a method like
		''' <seealso cref="#constructorModifiers"/> or <seealso cref="#methodModifiers"/>.
		''' </summary>
		''' <param name="mod"> a set of modifiers </param>
		''' <returns>  a string representation of the set of modifiers
		''' represented by {@code mod} </returns>
		Public Shared Function ToString(  [mod] As Integer) As String
			Dim sb As New StringBuilder
			Dim len As Integer

			If ([mod] And [PUBLIC]) <> 0 Then sb.append("public ")
			If ([mod] And [PROTECTED]) <> 0 Then sb.append("protected ")
			If ([mod] And [PRIVATE]) <> 0 Then sb.append("private ")

			' Canonical order 
			If ([mod] And ABSTRACT) <> 0 Then sb.append("abstract ")
			If ([mod] And [STATIC]) <> 0 Then sb.append("static ")
			If ([mod] And FINAL) <> 0 Then sb.append("final ")
			If ([mod] And TRANSIENT) <> 0 Then sb.append("transient ")
			If ([mod] And VOLATILE) <> 0 Then sb.append("volatile ")
			If ([mod] And SYNCHRONIZED) <> 0 Then sb.append("synchronized ")
			If ([mod] And NATIVE) <> 0 Then sb.append("native ")
			If ([mod] And [STRICT]) <> 0 Then sb.append("strictfp ")
			If ([mod] And [INTERFACE]) <> 0 Then sb.append("interface ")

			len = sb.length()
			If len > 0 Then ' trim trailing space Return sb.ToString().Substring(0, len-1)
			Return ""
		End Function

	'    
	'     * Access modifier flag constants from tables 4.1, 4.4, 4.5, and 4.7 of
	'     * <cite>The Java&trade; Virtual Machine Specification</cite>
	'     

		''' <summary>
		''' The {@code int} value representing the {@code public}
		''' modifier.
		''' </summary>
		Public Const [PUBLIC] As Integer = &H1

		''' <summary>
		''' The {@code int} value representing the {@code private}
		''' modifier.
		''' </summary>
		Public Const [PRIVATE] As Integer = &H2

		''' <summary>
		''' The {@code int} value representing the {@code protected}
		''' modifier.
		''' </summary>
		Public Const [PROTECTED] As Integer = &H4

		''' <summary>
		''' The {@code int} value representing the {@code static}
		''' modifier.
		''' </summary>
		Public Const [STATIC] As Integer = &H8

		''' <summary>
		''' The {@code int} value representing the {@code final}
		''' modifier.
		''' </summary>
		Public Const FINAL As Integer = &H10

		''' <summary>
		''' The {@code int} value representing the {@code synchronized}
		''' modifier.
		''' </summary>
		Public Const SYNCHRONIZED As Integer = &H20

		''' <summary>
		''' The {@code int} value representing the {@code volatile}
		''' modifier.
		''' </summary>
		Public Const VOLATILE As Integer = &H40

		''' <summary>
		''' The {@code int} value representing the {@code transient}
		''' modifier.
		''' </summary>
		Public Const TRANSIENT As Integer = &H80

		''' <summary>
		''' The {@code int} value representing the {@code native}
		''' modifier.
		''' </summary>
		Public Const NATIVE As Integer = &H100

		''' <summary>
		''' The {@code int} value representing the {@code interface}
		''' modifier.
		''' </summary>
		Public Const [INTERFACE] As Integer = &H200

		''' <summary>
		''' The {@code int} value representing the {@code abstract}
		''' modifier.
		''' </summary>
		Public Const ABSTRACT As Integer = &H400

		''' <summary>
		''' The {@code int} value representing the {@code strictfp}
		''' modifier.
		''' </summary>
		Public Const [STRICT] As Integer = &H800

		' Bits not (yet) exposed in the public API either because they
		' have different meanings for fields and methods and there is no
		' way to distinguish between the two in this [Class], or because
		' they are not Java programming language keywords
		Friend Const BRIDGE As Integer = &H40
		Friend Const VARARGS As Integer = &H80
		Friend Const SYNTHETIC As Integer = &H1000
		Friend Const ANNOTATION As Integer = &H2000
		Friend Const [ENUM] As Integer = &H4000
		Friend Const MANDATED As Integer = &H8000
		Friend Shared Function isSynthetic(  [mod] As Integer) As Boolean
		  Return ([mod] And SYNTHETIC) <> 0
		End Function

		Friend Shared Function isMandated(  [mod] As Integer) As Boolean
		  Return ([mod] And MANDATED) <> 0
		End Function

		' Note on the FOO_MODIFIERS fields and fooModifiers() methods:
		' the sets of modifiers are not guaranteed to be constants
		' across time and Java SE releases. Therefore, it would not be
		' appropriate to expose an external interface to this information
		' that would allow the values to be treated as Java-level
		' constants since the values could be constant folded and updates
		' to the sets of modifiers missed. Thus, the fooModifiers()
		' methods return an unchanging values for a given release, but a
		' value that can potentially change over time.

		''' <summary>
		''' The Java source modifiers that can be applied to a class.
		''' @jls 8.1.1 Class Modifiers
		''' </summary>
		Private Const CLASS_MODIFIERS As Integer = Modifier.PUBLIC Or Modifier.PROTECTED Or Modifier.PRIVATE Or Modifier.ABSTRACT Or Modifier.STATIC Or Modifier.FINAL Or Modifier.STRICT

		''' <summary>
		''' The Java source modifiers that can be applied to an interface.
		''' @jls 9.1.1 Interface Modifiers
		''' </summary>
		Private Const INTERFACE_MODIFIERS As Integer = Modifier.PUBLIC Or Modifier.PROTECTED Or Modifier.PRIVATE Or Modifier.ABSTRACT Or Modifier.STATIC Or Modifier.STRICT


		''' <summary>
		''' The Java source modifiers that can be applied to a constructor.
		''' @jls 8.8.3 Constructor Modifiers
		''' </summary>
		Private Const CONSTRUCTOR_MODIFIERS As Integer = Modifier.PUBLIC Or Modifier.PROTECTED Or Modifier.PRIVATE

		''' <summary>
		''' The Java source modifiers that can be applied to a method.
		''' @jls8.4.3  Method Modifiers
		''' </summary>
		Private Const METHOD_MODIFIERS As Integer = Modifier.PUBLIC Or Modifier.PROTECTED Or Modifier.PRIVATE Or Modifier.ABSTRACT Or Modifier.STATIC Or Modifier.FINAL Or Modifier.SYNCHRONIZED Or Modifier.NATIVE Or Modifier.STRICT

		''' <summary>
		''' The Java source modifiers that can be applied to a field.
		''' @jls 8.3.1  Field Modifiers
		''' </summary>
		Private Const FIELD_MODIFIERS As Integer = Modifier.PUBLIC Or Modifier.PROTECTED Or Modifier.PRIVATE Or Modifier.STATIC Or Modifier.FINAL Or Modifier.TRANSIENT Or Modifier.VOLATILE

		''' <summary>
		''' The Java source modifiers that can be applied to a method or constructor parameter.
		''' @jls 8.4.1 Formal Parameters
		''' </summary>
		Private Const PARAMETER_MODIFIERS As Integer = Modifier.FINAL

		''' 
		Friend Const ACCESS_MODIFIERS As Integer = Modifier.PUBLIC Or Modifier.PROTECTED Or Modifier.PRIVATE

		''' <summary>
		''' Return an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a class. </summary>
		''' <returns> an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a class.
		''' 
		''' @jls 8.1.1 Class Modifiers
		''' @since 1.7 </returns>
		Public Shared Function classModifiers() As Integer
			Return CLASS_MODIFIERS
		End Function

		''' <summary>
		''' Return an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to an interface. </summary>
		''' <returns> an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to an interface.
		''' 
		''' @jls 9.1.1 Interface Modifiers
		''' @since 1.7 </returns>
		Public Shared Function interfaceModifiers() As Integer
			Return INTERFACE_MODIFIERS
		End Function

		''' <summary>
		''' Return an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a constructor. </summary>
		''' <returns> an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a constructor.
		''' 
		''' @jls 8.8.3 Constructor Modifiers
		''' @since 1.7 </returns>
		Public Shared Function constructorModifiers() As Integer
			Return CONSTRUCTOR_MODIFIERS
		End Function

		''' <summary>
		''' Return an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a method. </summary>
		''' <returns> an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a method.
		''' 
		''' @jls 8.4.3 Method Modifiers
		''' @since 1.7 </returns>
		Public Shared Function methodModifiers() As Integer
			Return METHOD_MODIFIERS
		End Function

		''' <summary>
		''' Return an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a field. </summary>
		''' <returns> an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a field.
		''' 
		''' @jls 8.3.1 Field Modifiers
		''' @since 1.7 </returns>
		Public Shared Function fieldModifiers() As Integer
			Return FIELD_MODIFIERS
		End Function

		''' <summary>
		''' Return an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a parameter. </summary>
		''' <returns> an {@code int} value OR-ing together the source language
		''' modifiers that can be applied to a parameter.
		''' 
		''' @jls 8.4.1 Formal Parameters
		''' @since 1.8 </returns>
		Public Shared Function parameterModifiers() As Integer
			Return PARAMETER_MODIFIERS
		End Function
	End Class

End Namespace
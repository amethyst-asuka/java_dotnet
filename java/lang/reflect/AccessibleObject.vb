'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' The AccessibleObject class is the base class for Field, Method and
	''' Constructor objects.  It provides the ability to flag a reflected
	''' object as suppressing default Java language access control checks
	''' when it is used.  The access checks--for public, default (package)
	''' access, protected, and private members--are performed when Fields,
	''' Methods or Constructors are used to set or get fields, to invoke
	''' methods, or to create and initialize new instances of classes,
	''' respectively.
	''' 
	''' <p>Setting the {@code accessible} flag in a reflected object
	''' permits sophisticated applications with sufficient privilege, such
	''' as Java Object Serialization or other persistence mechanisms, to
	''' manipulate objects in a manner that would normally be prohibited.
	''' 
	''' <p>By default, a reflected object is <em>not</em> accessible.
	''' </summary>
	''' <seealso cref= Field </seealso>
	''' <seealso cref= Method </seealso>
	''' <seealso cref= Constructor </seealso>
	''' <seealso cref= ReflectPermission
	''' 
	''' @since 1.2 </seealso>
	Public Class AccessibleObject
		Implements AnnotatedElement

		''' <summary>
		''' The Permission object that is used to check whether a client
		''' has sufficient privilege to defeat Java language access
		''' control checks.
		''' </summary>
		Private Shared ReadOnly ACCESS_PERMISSION As java.security.Permission = New ReflectPermission("suppressAccessChecks")

		''' <summary>
		''' Convenience method to set the {@code accessible} flag for an
		''' array of objects with a single security check (for efficiency).
		''' 
		''' <p>First, if there is a security manager, its
		''' {@code checkPermission} method is called with a
		''' {@code ReflectPermission("suppressAccessChecks")} permission.
		''' 
		''' <p>A {@code SecurityException} is raised if {@code flag} is
		''' {@code true} but accessibility of any of the elements of the input
		''' {@code array} may not be changed (for example, if the element
		''' object is a <seealso cref="Constructor"/> object for the class {@link
		''' java.lang.Class}).  In the event of such a SecurityException, the
		''' accessibility of objects is set to {@code flag} for array elements
		''' upto (and excluding) the element for which the exception occurred; the
		''' accessibility of elements beyond (and including) the element for which
		''' the exception occurred is unchanged.
		''' </summary>
		''' <param name="array"> the array of AccessibleObjects </param>
		''' <param name="flag">  the new value for the {@code accessible} flag
		'''              in each object </param>
		''' <exception cref="SecurityException"> if the request is denied. </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.lang.RuntimePermission </seealso>
		Public Shared Sub setAccessible(ByVal array_Renamed As AccessibleObject(), ByVal flag As Boolean)
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(ACCESS_PERMISSION)
			For i As Integer = 0 To array_Renamed.Length - 1
				accessible0le0(array_Renamed(i), flag)
			Next i
		End Sub

		''' <summary>
		''' Set the {@code accessible} flag for this object to
		''' the indicated boolean value.  A value of {@code true} indicates that
		''' the reflected object should suppress Java language access
		''' checking when it is used.  A value of {@code false} indicates
		''' that the reflected object should enforce Java language access checks.
		''' 
		''' <p>First, if there is a security manager, its
		''' {@code checkPermission} method is called with a
		''' {@code ReflectPermission("suppressAccessChecks")} permission.
		''' 
		''' <p>A {@code SecurityException} is raised if {@code flag} is
		''' {@code true} but accessibility of this object may not be changed
		''' (for example, if this element object is a <seealso cref="Constructor"/> object for
		''' the class <seealso cref="java.lang.Class"/>).
		''' 
		''' <p>A {@code SecurityException} is raised if this object is a {@link
		''' java.lang.reflect.Constructor} object for the class
		''' {@code java.lang.Class}, and {@code flag} is true.
		''' </summary>
		''' <param name="flag"> the new value for the {@code accessible} flag </param>
		''' <exception cref="SecurityException"> if the request is denied. </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.lang.RuntimePermission </seealso>
		Public Overridable Property accessible As Boolean
			Set(ByVal flag As Boolean)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(ACCESS_PERMISSION)
				accessible0le0(Me, flag)
			End Set
			Get
				Return override
			End Get
		End Property

	'     Check that you aren't exposing java.lang.Class.<init> or sensitive
	'       fields in java.lang.Class. 
		Private Shared Sub setAccessible0(ByVal obj As AccessibleObject, ByVal flag As Boolean)
			If TypeOf obj Is Constructor AndAlso flag = True Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As Constructor(Of ?) = CType(obj, Constructor(Of ?))
				If c.declaringClass Is GetType(Class) Then Throw New SecurityException("Cannot make a java.lang.Class" & " constructor accessible")
			End If
			obj.override = flag
		End Sub


		''' <summary>
		''' Constructor: only used by the Java Virtual Machine.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		' Indicates whether language-level access checks are overridden
		' by this object. Initializes to "false". This field is used by
		' Field, Method, and Constructor.
		'
		' NOTE: for security purposes, this field must not be visible
		' outside this package.
		Friend override As Boolean

		' Reflection factory used by subclasses for creating field,
		' method, and constructor accessors. Note that this is called
		' very early in the bootstrapping process.
		Friend Shared ReadOnly reflectionFactory As sun.reflect.ReflectionFactory = java.security.AccessController.doPrivileged(New sun.reflect.ReflectionFactory.GetReflectionFactoryAction)

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overridable Function getAnnotation(Of T As Annotation)(ByVal annotationClass As Class) As T Implements AnnotatedElement.getAnnotation
			Throw New AssertionError("All subclasses should override this method")
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overrides Function isAnnotationPresent(ByVal annotationClass As Class) As Boolean Implements AnnotatedElement.isAnnotationPresent
			Return outerInstance.isAnnotationPresent(annotationClass)
		End Function

	   ''' <exception cref="NullPointerException"> {@inheritDoc}
	   ''' @since 1.8 </exception>
		Public Overrides Function getAnnotationsByType(Of T As Annotation)(ByVal annotationClass As Class) As T() Implements AnnotatedElement.getAnnotationsByType
			Throw New AssertionError("All subclasses should override this method")
		End Function

		''' <summary>
		''' @since 1.5
		''' </summary>
		Public Overridable Property annotations As Annotation() Implements AnnotatedElement.getAnnotations
			Get
				Return declaredAnnotations
			End Get
		End Property

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getDeclaredAnnotation(Of T As Annotation)(ByVal annotationClass As Class) As T Implements AnnotatedElement.getDeclaredAnnotation
			' Only annotations on classes are inherited, for all other
			' objects getDeclaredAnnotation is the same as
			' getAnnotation.
			Return getAnnotation(annotationClass)
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getDeclaredAnnotationsByType(Of T As Annotation)(ByVal annotationClass As Class) As T() Implements AnnotatedElement.getDeclaredAnnotationsByType
			' Only annotations on classes are inherited, for all other
			' objects getDeclaredAnnotationsByType is the same as
			' getAnnotationsByType.
			Return getAnnotationsByType(annotationClass)
		End Function

		''' <summary>
		''' @since 1.5
		''' </summary>
		Public Overridable Property declaredAnnotations As Annotation() Implements AnnotatedElement.getDeclaredAnnotations
			Get
				Throw New AssertionError("All subclasses should override this method")
			End Get
		End Property


		' Shared access checking logic.

		' For non-public members or members in package-private classes,
		' it is necessary to perform somewhat expensive security checks.
		' If the security check succeeds for a given class, it will
		' always succeed (it is not affected by the granting or revoking
		' of permissions); we speed up the check in the common case by
		' remembering the last Class for which the check succeeded.
		'
		' The simple security check for Constructor is to see if
		' the caller has already been seen, verified, and cached.
		' (See also Class.newInstance(), which uses a similar method.)
		'
		' A more complicated security check cache is needed for Method and Field
		' The cache can be either null (empty cache), a 2-array of {caller,target},
		' or a caller (with target implicitly equal to this.clazz).
		' In the 2-array case, the target is always different from the clazz.
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend securityCheckCache As Object

		Friend Overridable Sub checkAccess(ByVal caller As Class, ByVal clazz As Class, ByVal obj As Object, ByVal modifiers As Integer)
			If caller Is clazz Then ' quick check Return ' ACCESS IS OK
			Dim cache As Object = securityCheckCache ' read volatile
			Dim targetClass As Class = clazz
			targetClass = obj.GetType()
			If obj IsNot Nothing AndAlso Modifier.isProtected(modifiers) AndAlso (targetClass IsNot clazz) Then
				' Must match a 2-list of { caller, targetClass }.
				If TypeOf cache Is Class() Then
					Dim cache2 As Class() = CType(cache, Class())
					If cache2(1) Is targetClass AndAlso cache2(0) Is caller Then Return ' ACCESS IS OK
					' (Test cache[1] first since range check for [1]
					' subsumes range check for [0].)
				End If
			ElseIf cache Is caller Then
				' Non-protected case (or obj.class == this.clazz).
				Return ' ACCESS IS OK
			End If

			' If no return, fall through to the slow path.
			slowCheckMemberAccess(caller, clazz, obj, modifiers, targetClass)
		End Sub

		' Keep all this slow stuff out of line:
		Friend Overridable Sub slowCheckMemberAccess(ByVal caller As Class, ByVal clazz As Class, ByVal obj As Object, ByVal modifiers As Integer, ByVal targetClass As Class)
			sun.reflect.Reflection.ensureMemberAccess(caller, clazz, obj, modifiers)

			' Success: Update the cache.
			Dim cache As Object = (If(targetClass Is clazz, caller, New [Class]()){ caller, targetClass })

			' Note:  The two cache elements are not volatile,
			' but they are effectively final.  The Java memory model
			' guarantees that the initializing stores for the cache
			' elements will occur before the volatile write.
			securityCheckCache = cache ' write volatile
		End Sub
	End Class

End Namespace
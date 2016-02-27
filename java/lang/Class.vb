Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports java.lang.reflect
Imports sun.reflect.annotation

'
' * Copyright (c) 1994, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang


    ''' <summary>
    ''' Instances of the class {@code Class} represent classes and
    ''' interfaces in a running Java application.  An enum is a kind of
    ''' class and an annotation is a kind of interface.  Every array also
    ''' belongs to a class that is reflected as a {@code Class} object
    ''' that is shared by all arrays with the same element type and number
    ''' of dimensions.  The primitive Java types ({@code boolean},
    ''' {@code byte}, {@code char}, {@code short},
    ''' {@code int}, {@code long}, {@code float}, and
    ''' {@code double}), and the keyword {@code void} are also
    ''' represented as {@code Class} objects.
    ''' 
    ''' <p> {@code Class} has no public constructor. Instead {@code Class}
    ''' objects are constructed automatically by the Java Virtual Machine As  [Class]es
    ''' are loaded and by calls to the {@code defineClass} method in the class
    ''' loader.
    ''' 
    ''' <p> The following example uses a {@code Class} object to print the
    ''' class name of an object:
    ''' 
    ''' <blockquote><pre>
    '''     void printClassName(Object obj) {
    '''         System.out.println("The class of " + obj +
    '''                            " is " + obj.getClass().getName());
    '''     }
    ''' </pre></blockquote>
    ''' 
    ''' <p> It is also possible to get the {@code Class} object for a named
    ''' type (or for void) using a class literal.  See Section 15.8.2 of
    ''' <cite>The Java&trade; Language Specification</cite>.
    ''' For example:
    ''' 
    ''' <blockquote>
    '''     {@code System.out.println("The name of class Foo is: "+Foo.class.getName());}
    ''' </blockquote>
    ''' </summary>
    ''' @param <T> the type of the class modeled by this {@code Class}
    ''' object.  For example, the type of {@code String.class} is {@code
    ''' Class<String>}.  Use {@code Class<?>} if the class being modeled is
    ''' unknown.
    ''' 
    ''' @author  unascribed </param>
    ''' <seealso cref=     java.lang.ClassLoader#defineClass(byte[], int, int)
    ''' @since   JDK1.0 </seealso>
    <Serializable>
    Public NotInheritable Class [Class] : Inherits java.lang.Object
        Implements GenericDeclaration, Type, AnnotatedElement

        Private Const ANNOTATION As Integer = &H2000
        Private Const [ENUM] As Integer = &H4000
        Private Const SYNTHETIC As Integer = &H1000

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub registerNatives()
        End Sub
        Shared Sub New()
            registerNatives()
            Dim fields_Renamed As Field() = GetType(Class).getDeclaredFields0(False) ' bypass caches
				reflectionDataOffset = objectFieldOffset(fields_Renamed, "reflectionData")
            annotationTypeOffset = objectFieldOffset(fields_Renamed, "annotationType")
            annotationDataOffset = objectFieldOffset(fields_Renamed, "annotationData")
        End Sub

        '    
        '     * Private constructor. Only the Java Virtual Machine creates Class objects.
        '     * This constructor is not used and prevents the default constructor being
        '     * generated.
        '     
        Private Sub New(ByVal loader As  [Class]Loader)
            ' Initialize final field for classLoader.  The initialization value of non-null
            ' prevents future JIT optimizations from assuming this final field is null.
            classLoader = loader
        End Sub

        ''' <summary>
        ''' Converts the object to a string. The string representation is the
        ''' string "class" or "interface", followed by a space, and then by the
        ''' fully qualified name of the class in the format returned by
        ''' {@code getName}.  If this {@code Class} object represents a
        ''' primitive type, this method returns the name of the primitive type.  If
        ''' this {@code Class} object represents void this method returns
        ''' "void".
        ''' </summary>
        ''' <returns> a string representation of this class object. </returns>
        Public Overrides Function ToString() As String
            Return (If([interface], "interface ", (If(primitive, "", "class ")))) + name
        End Function

        ''' <summary>
        ''' Returns a string describing this {@code Class}, including
        ''' information about modifiers and type parameters.
        ''' 
        ''' The string is formatted as a list of type modifiers, if any,
        ''' followed by the kind of type (empty string for primitive types
        ''' and {@code class}, {@code enum}, {@code interface}, or
        ''' <code>&#64;</code>{@code interface}, as appropriate), followed
        ''' by the type's name, followed by an angle-bracketed
        ''' comma-separated list of the type's type parameters, if any.
        ''' 
        ''' A space is used to separate modifiers from one another and to
        ''' separate any modifiers from the kind of type. The modifiers
        ''' occur in canonical order. If there are no type parameters, the
        ''' type parameter list is elided.
        ''' 
        ''' <p>Note that since information about the runtime representation
        ''' of a type is being generated, modifiers not present on the
        ''' originating source code or illegal on the originating source
        ''' code may be present.
        ''' </summary>
        ''' <returns> a string describing this {@code Class}, including
        ''' information about modifiers and type parameters
        ''' 
        ''' @since 1.8 </returns>
        Public Function toGenericString() As String
            If primitive Then
                Return ToString()
            Else
                Dim sb As New StringBuilder

                ' Class modifiers are a superset of interface modifiers
                Dim modifiers As Integer = modifiers And Modifier.classModifiers()
                If modifiers <> 0 Then
                    sb.append(Modifier.ToString(modifiers))
                    sb.append(" "c)
                End If

                If ANNOTATION Then sb.append("@"c)
                If [interface] Then ' Note: all annotation types are interfaces
                    sb.append("interface")
                Else
                    If [ENUM] Then
                        sb.append("enum")
                    Else
                        sb.append("class")
                    End If
                End If
                sb.append(" "c)
                sb.append(name)

                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                Dim typeparms As TypeVariable(Of ?)() = typeParameters
                If typeparms.Length > 0 Then
                    Dim first As Boolean = True
                    sb.append("<"c)
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    For Each typeparm As TypeVariable(Of ?) In typeparms
                        If Not first Then sb.append(","c)
                        sb.append(typeparm.typeName)
                        first = False
                    Next typeparm
                    sb.append(">"c)
                End If

                Return sb.ToString()
            End If
        End Function

        ''' <summary>
        ''' Returns the {@code Class} object associated with the class or
        ''' interface with the given string name.  Invoking this method is
        ''' equivalent to:
        ''' 
        ''' <blockquote>
        '''  {@code Class.forName(className, true, currentLoader)}
        ''' </blockquote>
        ''' 
        ''' where {@code currentLoader} denotes the defining class loader of
        ''' the current class.
        ''' 
        ''' <p> For example, the following code fragment returns the
        ''' runtime {@code Class} descriptor for the class named
        ''' {@code java.lang.Thread}:
        ''' 
        ''' <blockquote>
        '''   {@code Class t = Class.forName("java.lang.Thread")}
        ''' </blockquote>
        ''' <p>
        ''' A call to {@code forName("X")} causes the class named
        ''' {@code X} to be initialized.
        ''' </summary>
        ''' <param name="className">   the fully qualified name of the desired class. </param>
        ''' <returns>     the {@code Class} object for the class with the
        '''             specified name. </returns>
        ''' <exception cref="LinkageError"> if the linkage fails </exception>
        ''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
        '''            by this method fails </exception>
        ''' <exception cref="ClassNotFoundException"> if the class cannot be located </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared Function forName(ByVal className As String) As [Class]
            Dim caller As [Class] = sun.reflect.Reflection.callerClass
            Return forName0(className, True, classLoader.getClassLoader(caller), caller)
        End Function


        ''' <summary>
        ''' Returns the {@code Class} object associated with the class or
        ''' interface with the given string name, using the given class loader.
        ''' Given the fully qualified name for a class or interface (in the same
        ''' format returned by {@code getName}) this method attempts to
        ''' locate, load, and link the class or interface.  The specified class
        ''' loader is used to load the class or interface.  If the parameter
        ''' {@code loader} is null, the class is loaded through the bootstrap
        ''' class loader.  The class is initialized only if the
        ''' {@code initialize} parameter is {@code true} and if it has
        ''' not been initialized earlier.
        ''' 
        ''' <p> If {@code name} denotes a primitive type or void, an attempt
        ''' will be made to locate a user-defined class in the unnamed package whose
        ''' name is {@code name}. Therefore, this method cannot be used to
        ''' obtain any of the {@code Class} objects representing primitive
        ''' types or void.
        ''' 
        ''' <p> If {@code name} denotes an array [Class], the component type of
        ''' the array class is loaded but not initialized.
        ''' 
        ''' <p> For example, in an instance method the expression:
        ''' 
        ''' <blockquote>
        '''  {@code Class.forName("Foo")}
        ''' </blockquote>
        ''' 
        ''' is equivalent to:
        ''' 
        ''' <blockquote>
        '''  {@code Class.forName("Foo", true, this.getClass().getClassLoader())}
        ''' </blockquote>
        ''' 
        ''' Note that this method throws errors related to loading, linking or
        ''' initializing as specified in Sections 12.2, 12.3 and 12.4 of <em>The
        ''' Java Language Specification</em>.
        ''' Note that this method does not check whether the requested class
        ''' is accessible to its caller.
        ''' 
        ''' <p> If the {@code loader} is {@code null}, and a security
        ''' manager is present, and the caller's class loader is not null, then this
        ''' method calls the security manager's {@code checkPermission} method
        ''' with a {@code RuntimePermission("getClassLoader")} permission to
        ''' ensure it's ok to access the bootstrap class loader.
        ''' </summary>
        ''' <param name="name">       fully qualified name of the desired class </param>
        ''' <param name="initialize"> if {@code true} the class will be initialized.
        '''                   See Section 12.4 of <em>The Java Language Specification</em>. </param>
        ''' <param name="loader">     class loader from which the class must be loaded </param>
        ''' <returns>           class object representing the desired class
        ''' </returns>
        ''' <exception cref="LinkageError"> if the linkage fails </exception>
        ''' <exception cref="ExceptionInInitializerError"> if the initialization provoked
        '''            by this method fails </exception>
        ''' <exception cref="ClassNotFoundException"> if the class cannot be located by
        '''            the specified class loader
        ''' </exception>
        ''' <seealso cref=       java.lang.Class#forName(String) </seealso>
        ''' <seealso cref=       java.lang.ClassLoader
        ''' @since     1.2 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared Function forName(ByVal name As String, ByVal initialize As Boolean, ByVal loader As  [Class]Loader) As [Class]
            Dim caller As [Class] = Nothing
            Dim sm As SecurityManager = System.securityManager
            If sm IsNot Nothing Then
                ' Reflective call to get caller class is only needed if a security manager
                ' is present.  Avoid the overhead of making this call otherwise.
                caller = sun.reflect.Reflection.callerClass
                If sun.misc.VM.isSystemDomainLoader(loader) Then
                    Dim ccl As  [Class]Loader = classLoader.getClassLoader(caller)
                    If Not sun.misc.VM.isSystemDomainLoader(ccl) Then sm.checkPermission(sun.security.util.SecurityConstants.GET_CLASSLOADER_PERMISSION)
                End If
            End If
            Return forName0(name, initialize, loader, caller)
        End Function

        ''' <summary>
        ''' Called after security check for system loader access checks have been made. </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Function forName0(ByVal name As String, ByVal initialize As Boolean, ByVal loader As  [Class]Loader, ByVal caller As [Class]) As  [Class]
		End Function

        ''' <summary>
        ''' Creates a new instance of the class represented by this {@code Class}
        ''' object.  The class is instantiated as if by a {@code new}
        ''' expression with an empty argument list.  The class is initialized if it
        ''' has not already been initialized.
        ''' 
        ''' <p>Note that this method propagates any exception thrown by the
        ''' nullary constructor, including a checked exception.  Use of
        ''' this method effectively bypasses the compile-time exception
        ''' checking that would otherwise be performed by the compiler.
        ''' The {@link
        ''' java.lang.reflect.Constructor#newInstance(java.lang.Object...)
        ''' Constructor.newInstance} method avoids this problem by wrapping
        ''' any exception thrown by the constructor in a (checked) {@link
        ''' java.lang.reflect.InvocationTargetException}.
        ''' </summary>
        ''' <returns>  a newly allocated instance of the class represented by this
        '''          object. </returns>
        ''' <exception cref="IllegalAccessException">  if the class or its nullary
        '''          constructor is not accessible. </exception>
        ''' <exception cref="InstantiationException">
        '''          if this {@code Class} represents an abstract [Class],
        '''          an interface, an array [Class], a primitive type, or void;
        '''          or if the class has no nullary constructor;
        '''          or if the instantiation fails for some other reason. </exception>
        ''' <exception cref="ExceptionInInitializerError"> if the initialization
        '''          provoked by this method fails. </exception>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and
        '''          the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class. </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function newInstance() As T
            If System.securityManager IsNot Nothing Then checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, False)

            ' NOTE: the following code may not be strictly correct under
            ' the current Java memory model.

            ' Constructor lookup
            If cachedConstructor Is Nothing Then
                If Me Is GetType([Class]) Then Throw New IllegalAccessException("Can not call newInstance() on the Class for java.lang.Class")
                Try
                    Dim empty As [Class]() = {}
                    Dim c As Constructor(Of T) = getConstructor0(empty, Member.DECLARED)
                    ' Disable accessibility checks on the constructor
                    ' since we have to do the security check here anyway
                    ' (the stack depth is wrong for the Constructor's
                    ' security check to work)
                    java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
                    cachedConstructor = c
                Catch e As NoSuchMethodException
                    Throw CType((New InstantiationException(name)).initCause(e), InstantiationException)
                End Try
            End If
            Dim tmpConstructor As Constructor(Of T) = cachedConstructor
            ' Security check (same as in java.lang.reflect.Constructor)
            Dim modifiers As Integer = tmpConstructor.modifiers
            If Not sun.reflect.Reflection.quickCheckMemberAccess(Me, modifiers) Then
                Dim caller As [Class] = sun.reflect.Reflection.callerClass
                If newInstanceCallerCache IsNot caller Then
                    sun.reflect.Reflection.ensureMemberAccess(caller, Me, Nothing, modifiers)
                    newInstanceCallerCache = caller
                End If
            End If
            ' Run constructor
            Try
                Return tmpConstructor.newInstance(CType(Nothing, Object()))
            Catch e As InvocationTargetException
                sun.misc.Unsafe.unsafe.throwException(e.targetException)
                ' Not reached
                Return Nothing
            End Try
        End Function

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                c.accessible = True
                Return Nothing
            End Function
        End Class
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private cachedConstructor As Constructor(Of T)
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private newInstanceCallerCache As [Class]


        ''' <summary>
        ''' Determines if the specified {@code Object} is assignment-compatible
        ''' with the object represented by this {@code Class}.  This method is
        ''' the dynamic equivalent of the Java language {@code instanceof}
        ''' operator. The method returns {@code true} if the specified
        ''' {@code Object} argument is non-null and can be cast to the
        ''' reference type represented by this {@code Class} object without
        ''' raising a {@code ClassCastException.} It returns {@code false}
        ''' otherwise.
        ''' 
        ''' <p> Specifically, if this {@code Class} object represents a
        ''' declared [Class], this method returns {@code true} if the specified
        ''' {@code Object} argument is an instance of the represented class (or
        ''' of any of its subclasses); it returns {@code false} otherwise. If
        ''' this {@code Class} object represents an array [Class], this method
        ''' returns {@code true} if the specified {@code Object} argument
        ''' can be converted to an object of the array class by an identity
        ''' conversion or by a widening reference conversion; it returns
        ''' {@code false} otherwise. If this {@code Class} object
        ''' represents an interface, this method returns {@code true} if the
        ''' class or any superclass of the specified {@code Object} argument
        ''' implements this interface; it returns {@code false} otherwise. If
        ''' this {@code Class} object represents a primitive type, this method
        ''' returns {@code false}.
        ''' </summary>
        ''' <param name="obj"> the object to check </param>
        ''' <returns>  true if {@code obj} is an instance of this class
        ''' 
        ''' @since JDK1.1 </returns>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function isInstance(ByVal obj As Object) As Boolean
        End Function


        ''' <summary>
        ''' Determines if the class or interface represented by this
        ''' {@code Class} object is either the same as, or is a superclass or
        ''' superinterface of, the class or interface represented by the specified
        ''' {@code Class} parameter. It returns {@code true} if so;
        ''' otherwise it returns {@code false}. If this {@code Class}
        ''' object represents a primitive type, this method returns
        ''' {@code true} if the specified {@code Class} parameter is
        ''' exactly this {@code Class} object; otherwise it returns
        ''' {@code false}.
        ''' 
        ''' <p> Specifically, this method tests whether the type represented by the
        ''' specified {@code Class} parameter can be converted to the type
        ''' represented by this {@code Class} object via an identity conversion
        ''' or via a widening reference conversion. See <em>The Java Language
        ''' Specification</em>, sections 5.1.1 and 5.1.4 , for details.
        ''' </summary>
        ''' <param name="cls"> the {@code Class} object to be checked </param>
        ''' <returns> the {@code boolean} value indicating whether objects of the
        ''' type {@code cls} can be assigned to objects of this class </returns>
        ''' <exception cref="NullPointerException"> if the specified Class parameter is
        '''            null.
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function isAssignableFrom(ByVal cls As [Class]) As Boolean
        End Function


        ''' <summary>
        ''' Determines if the specified {@code Class} object represents an
        ''' interface type.
        ''' </summary>
        ''' <returns>  {@code true} if this object represents an interface;
        '''          {@code false} otherwise. </returns>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function isInterface() As Boolean
        End Function


        ''' <summary>
        ''' Determines if this {@code Class} object represents an array class.
        ''' </summary>
        ''' <returns>  {@code true} if this object represents an array class;
        '''          {@code false} otherwise.
        ''' @since   JDK1.1 </returns>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function isArray() As Boolean
        End Function


        ''' <summary>
        ''' Determines if the specified {@code Class} object represents a
        ''' primitive type.
        ''' 
        ''' <p> There are nine predefined {@code Class} objects to represent
        ''' the eight primitive types and void.  These are created by the Java
        ''' Virtual Machine, and have the same names as the primitive types that
        ''' they represent, namely {@code boolean}, {@code byte},
        ''' {@code char}, {@code short}, {@code int},
        ''' {@code long}, {@code float}, and {@code double}.
        ''' 
        ''' <p> These objects may only be accessed via the following public static
        ''' final variables, and are the only {@code Class} objects for which
        ''' this method returns {@code true}.
        ''' </summary>
        ''' <returns> true if and only if this class represents a primitive type
        ''' </returns>
        ''' <seealso cref=     java.lang.Boolean#TYPE </seealso>
        ''' <seealso cref=     java.lang.Character#TYPE </seealso>
        ''' <seealso cref=     java.lang.Byte#TYPE </seealso>
        ''' <seealso cref=     java.lang.Short#TYPE </seealso>
        ''' <seealso cref=     java.lang.Integer#TYPE </seealso>
        ''' <seealso cref=     java.lang.Long#TYPE </seealso>
        ''' <seealso cref=     java.lang.Float#TYPE </seealso>
        ''' <seealso cref=     java.lang.Double#TYPE </seealso>
        ''' <seealso cref=     java.lang.Void#TYPE
        ''' @since JDK1.1 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function isPrimitive() As Boolean
        End Function

        ''' <summary>
        ''' Returns true if this {@code Class} object represents an annotation
        ''' type.  Note that if this method returns true, <seealso cref="#isInterface()"/>
        ''' would also return true, as all annotation types are also interfaces.
        ''' </summary>
        ''' <returns> {@code true} if this class object represents an annotation
        '''      type; {@code false} otherwise
        ''' @since 1.5 </returns>
        Public ReadOnly Property annotation As Boolean
            Get
                Return (modifiers And annotation) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns {@code true} if this class is a synthetic class;
        ''' returns {@code false} otherwise. </summary>
        ''' <returns> {@code true} if and only if this class is a synthetic class as
        '''         defined by the Java Language Specification.
        ''' @jls 13.1 The Form of a Binary
        ''' @since 1.5 </returns>
        Public ReadOnly Property synthetic As Boolean
            Get
                Return (modifiers And synthetic) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns the  name of the entity (class, interface, array [Class],
        ''' primitive type, or void) represented by this {@code Class} object,
        ''' as a {@code String}.
        ''' 
        ''' <p> If this class object represents a reference type that is not an
        ''' array type then the binary name of the class is returned, as specified
        ''' by
        ''' <cite>The Java&trade; Language Specification</cite>.
        ''' 
        ''' <p> If this class object represents a primitive type or void, then the
        ''' name returned is a {@code String} equal to the Java language
        ''' keyword corresponding to the primitive type or void.
        ''' 
        ''' <p> If this class object represents a class of arrays, then the internal
        ''' form of the name consists of the name of the element type preceded by
        ''' one or more '{@code [}' characters representing the depth of the array
        ''' nesting.  The encoding of element type names is as follows:
        ''' 
        ''' <blockquote><table summary="Element types and encodings">
        ''' <tr><th> Element Type <th> &nbsp;&nbsp;&nbsp; <th> Encoding
        ''' <tr><td> boolean      <td> &nbsp;&nbsp;&nbsp; <td align=center> Z
        ''' <tr><td> byte         <td> &nbsp;&nbsp;&nbsp; <td align=center> B
        ''' <tr><td> char         <td> &nbsp;&nbsp;&nbsp; <td align=center> C
        ''' <tr><td> class or interface
        '''                       <td> &nbsp;&nbsp;&nbsp; <td align=center> L<i>classname</i>;
        ''' <tr><td> double       <td> &nbsp;&nbsp;&nbsp; <td align=center> D
        ''' <tr><td> float        <td> &nbsp;&nbsp;&nbsp; <td align=center> F
        ''' <tr><td> int          <td> &nbsp;&nbsp;&nbsp; <td align=center> I
        ''' <tr><td> long         <td> &nbsp;&nbsp;&nbsp; <td align=center> J
        ''' <tr><td> short        <td> &nbsp;&nbsp;&nbsp; <td align=center> S
        ''' </table></blockquote>
        ''' 
        ''' <p> The class or interface name <i>classname</i> is the binary name of
        ''' the class specified above.
        ''' 
        ''' <p> Examples:
        ''' <blockquote><pre>
        ''' String.class.getName()
        '''     returns "java.lang.String"
        ''' java.lang.[Byte].class.getName()
        '''     returns "byte"
        ''' (new Object[3]).getClass().getName()
        '''     returns "[Ljava.lang.Object;"
        ''' (new int[3][4][5][6][7][8][9]).getClass().getName()
        '''     returns "[[[[[[[I"
        ''' </pre></blockquote>
        ''' </summary>
        ''' <returns>  the name of the class or interface
        '''          represented by this object. </returns>
        Public ReadOnly Property name As String
            Get
                Dim name_Renamed As String = Me._name
                If name_Renamed Is Nothing Then
                    name_Renamed = name0
                    Me._name = name_Renamed
                End If
                Return name_Renamed
            End Get
        End Property

        ' cache the name to reduce the number of calls into the VM
        <NonSerialized>
        Private _name As String
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getName0() As String
        End Function

        ''' <summary>
        ''' Returns the class loader for the class.  Some implementations may use
        ''' null to represent the bootstrap class loader. This method will return
        ''' null in such implementations if this class was loaded by the bootstrap
        ''' class loader.
        ''' 
        ''' <p> If a security manager is present, and the caller's class loader is
        ''' not null and the caller's class loader is not the same as or an ancestor of
        ''' the class loader for the class whose class loader is requested, then
        ''' this method calls the security manager's {@code checkPermission}
        ''' method with a {@code RuntimePermission("getClassLoader")}
        ''' permission to ensure it's ok to access the class loader for the class.
        ''' 
        ''' <p>If this object
        ''' represents a primitive type or void, null is returned.
        ''' </summary>
        ''' <returns>  the class loader that loaded the class or interface
        '''          represented by this object. </returns>
        ''' <exception cref="SecurityException">
        '''    if a security manager exists and its
        '''    {@code checkPermission} method denies
        '''    access to the class loader for the class. </exception>
        ''' <seealso cref= java.lang.ClassLoader </seealso>
        ''' <seealso cref= SecurityManager#checkPermission </seealso>
        ''' <seealso cref= java.lang.RuntimePermission </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property classLoader As  [Class]Loader
            Get
                Dim cl As  [Class]Loader = classLoader0
                If cl Is Nothing Then Return Nothing
                Dim sm As SecurityManager = System.securityManager
                If sm IsNot Nothing Then classLoader.checkClassLoaderPermission(cl, sun.reflect.Reflection.callerClass)
                Return cl
            End Get
        End Property

        ' Package-private to allow ClassLoader access
        Friend ReadOnly Property classLoader0 As  [Class]Loader
            Get
                Return classLoader
            End Get
        End Property

        ' Initialized in JVM not by private constructor
        ' This field is filtered from reflection access, i.e. getDeclaredField
        ' will throw NoSuchFieldException
        Private ReadOnly classLoader As  [Class]Loader

        ''' <summary>
        ''' Returns an array of {@code TypeVariable} objects that represent the
        ''' type variables declared by the generic declaration represented by this
        ''' {@code GenericDeclaration} object, in declaration order.  Returns an
        ''' array of length 0 if the underlying generic declaration declares no type
        ''' variables.
        ''' </summary>
        ''' <returns> an array of {@code TypeVariable} objects that represent
        '''     the type variables declared by this generic declaration </returns>
        ''' <exception cref="java.lang.reflect.GenericSignatureFormatError"> if the generic
        '''     signature of this generic declaration does not conform to
        '''     the format specified in
        '''     <cite>The Java&trade; Virtual Machine Specification</cite>
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property typeParameters As TypeVariable(Of [Class])()
            Get
                Dim info As sun.reflect.generics.repository.ClassRepository = genericInfo
                If info IsNot Nothing Then
                    Return CType(info.typeParameters, TypeVariable(Of [Class])())
                Else
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    Return CType(New TypeVariable(Of ?)() {}, TypeVariable(Of [Class])())
                End If
            End Get
        End Property


        ''' <summary>
        ''' Returns the {@code Class} representing the superclass of the entity
        ''' (class, interface, primitive type or void) represented by this
        ''' {@code Class}.  If this {@code Class} represents either the
        ''' {@code Object} [Class], an interface, a primitive type, or void, then
        ''' null is returned.  If this object represents an array class then the
        ''' {@code Class} object representing the {@code Object} class is
        ''' returned.
        ''' </summary>
        ''' <returns> the superclass of the class represented by this object. </returns>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function getSuperclass() As [Class]
        End Function


        ''' <summary>
        ''' Returns the {@code Type} representing the direct superclass of
        ''' the entity (class, interface, primitive type or void) represented by
        ''' this {@code Class}.
        ''' 
        ''' <p>If the superclass is a parameterized type, the {@code Type}
        ''' object returned must accurately reflect the actual type
        ''' parameters used in the source code. The parameterized type
        ''' representing the superclass is created if it had not been
        ''' created before. See the declaration of {@link
        ''' java.lang.reflect.ParameterizedType ParameterizedType} for the
        ''' semantics of the creation process for parameterized types.  If
        ''' this {@code Class} represents either the {@code Object}
        ''' [Class], an interface, a primitive type, or void, then null is
        ''' returned.  If this object represents an array class then the
        ''' {@code Class} object representing the {@code Object} class is
        ''' returned.
        ''' </summary>
        ''' <exception cref="java.lang.reflect.GenericSignatureFormatError"> if the generic
        '''     class signature does not conform to the format specified in
        '''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
        ''' <exception cref="TypeNotPresentException"> if the generic superclass
        '''     refers to a non-existent type declaration </exception>
        ''' <exception cref="java.lang.reflect.MalformedParameterizedTypeException"> if the
        '''     generic superclass refers to a parameterized type that cannot be
        '''     instantiated  for any reason </exception>
        ''' <returns> the superclass of the class represented by this object
        ''' @since 1.5 </returns>
        Public ReadOnly Property genericSuperclass As Type
            Get
                Dim info As sun.reflect.generics.repository.ClassRepository = genericInfo
                If info Is Nothing Then Return superclass

                ' Historical irregularity:
                ' Generic signature marks interfaces with superclass = Object
                ' but this API returns null for interfaces
                If [interface] Then Return Nothing

                Return info.BaseType
            End Get
        End Property

        ''' <summary>
        ''' Gets the package for this class.  The class loader of this class is used
        ''' to find the package.  If the class was loaded by the bootstrap class
        ''' loader the set of packages loaded from CLASSPATH is searched to find the
        ''' package of the class. Null is returned if no package object was created
        ''' by the class loader of this class.
        ''' 
        ''' <p> Packages have attributes for versions and specifications only if the
        ''' information was defined in the manifests that accompany the classes, and
        ''' if the class loader created the package instance with the attributes
        ''' from the manifest.
        ''' </summary>
        ''' <returns> the package of the [Class], or null if no package
        '''         information is available from the archive or codebase. </returns>
        Public ReadOnly Property package As Package
            Get
                Return package.getPackage(Me)
            End Get
        End Property


        ''' <summary>
        ''' Determines the interfaces implemented by the class or interface
        ''' represented by this object.
        ''' 
        ''' <p> If this object represents a [Class], the return value is an array
        ''' containing objects representing all interfaces implemented by the
        ''' class. The order of the interface objects in the array corresponds to
        ''' the order of the interface names in the {@code implements} clause
        ''' of the declaration of the class represented by this object. For
        ''' example, given the declaration:
        ''' <blockquote>
        ''' {@code class Shimmer implements FloorWax, DessertTopping { ... }}
        ''' </blockquote>
        ''' suppose the value of {@code s} is an instance of
        ''' {@code Shimmer}; the value of the expression:
        ''' <blockquote>
        ''' {@code s.getClass().getInterfaces()[0]}
        ''' </blockquote>
        ''' is the {@code Class} object that represents interface
        ''' {@code FloorWax}; and the value of:
        ''' <blockquote>
        ''' {@code s.getClass().getInterfaces()[1]}
        ''' </blockquote>
        ''' is the {@code Class} object that represents interface
        ''' {@code DessertTopping}.
        ''' 
        ''' <p> If this object represents an interface, the array contains objects
        ''' representing all interfaces extended by the interface. The order of the
        ''' interface objects in the array corresponds to the order of the interface
        ''' names in the {@code extends} clause of the declaration of the
        ''' interface represented by this object.
        ''' 
        ''' <p> If this object represents a class or interface that implements no
        ''' interfaces, the method returns an array of length 0.
        ''' 
        ''' <p> If this object represents a primitive type or void, the method
        ''' returns an array of length 0.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, the
        ''' interfaces {@code Cloneable} and {@code java.io.Serializable} are
        ''' returned in that order.
        ''' </summary>
        ''' <returns> an array of interfaces implemented by this class. </returns>
        Public ReadOnly Property interfaces As [Class]()
            Get
                Dim rd As ReflectionData(Of T) = reflectionData()
                If rd Is Nothing Then
                    ' no cloning required
                    Return interfaces0
                Else
                    Dim interfaces_Renamed As  [Class]() = rd.interfaces
                    If interfaces_Renamed Is Nothing Then
                        interfaces_Renamed = interfaces0
                        rd.interfaces = interfaces_Renamed
                    End If
                    ' defensively copy before handing over to user code
                    Return interfaces_Renamed.clone()
                End If
            End Get
        End Property

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getInterfaces0() As [Class]()
        End Function

        ''' <summary>
        ''' Returns the {@code Type}s representing the interfaces
        ''' directly implemented by the class or interface represented by
        ''' this object.
        ''' 
        ''' <p>If a superinterface is a parameterized type, the
        ''' {@code Type} object returned for it must accurately reflect
        ''' the actual type parameters used in the source code. The
        ''' parameterized type representing each superinterface is created
        ''' if it had not been created before. See the declaration of
        ''' <seealso cref="java.lang.reflect.ParameterizedType ParameterizedType"/>
        ''' for the semantics of the creation process for parameterized
        ''' types.
        ''' 
        ''' <p> If this object represents a [Class], the return value is an
        ''' array containing objects representing all interfaces
        ''' implemented by the class. The order of the interface objects in
        ''' the array corresponds to the order of the interface names in
        ''' the {@code implements} clause of the declaration of the class
        ''' represented by this object.  In the case of an array [Class], the
        ''' interfaces {@code Cloneable} and {@code Serializable} are
        ''' returned in that order.
        ''' 
        ''' <p>If this object represents an interface, the array contains
        ''' objects representing all interfaces directly extended by the
        ''' interface.  The order of the interface objects in the array
        ''' corresponds to the order of the interface names in the
        ''' {@code extends} clause of the declaration of the interface
        ''' represented by this object.
        ''' 
        ''' <p>If this object represents a class or interface that
        ''' implements no interfaces, the method returns an array of length
        ''' 0.
        ''' 
        ''' <p>If this object represents a primitive type or void, the
        ''' method returns an array of length 0.
        ''' </summary>
        ''' <exception cref="java.lang.reflect.GenericSignatureFormatError">
        '''     if the generic class signature does not conform to the format
        '''     specified in
        '''     <cite>The Java&trade; Virtual Machine Specification</cite> </exception>
        ''' <exception cref="TypeNotPresentException"> if any of the generic
        '''     superinterfaces refers to a non-existent type declaration </exception>
        ''' <exception cref="java.lang.reflect.MalformedParameterizedTypeException">
        '''     if any of the generic superinterfaces refer to a parameterized
        '''     type that cannot be instantiated for any reason </exception>
        ''' <returns> an array of interfaces implemented by this class
        ''' @since 1.5 </returns>
        Public ReadOnly Property genericInterfaces As Type()
            Get
                Dim info As sun.reflect.generics.repository.ClassRepository = genericInfo
                Return If(info Is Nothing, interfaces, info.superInterfaces)
            End Get
        End Property


        ''' <summary>
        ''' Returns the {@code Class} representing the component type of an
        ''' array.  If this class does not represent an array class this method
        ''' returns null.
        ''' </summary>
        ''' <returns> the {@code Class} representing the component type of this
        ''' class if this class is an array </returns>
        ''' <seealso cref=     java.lang.reflect.Array
        ''' @since JDK1.1 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function getComponentType() As [Class]
        End Function


        ''' <summary>
        ''' Returns the Java language modifiers for this class or interface, encoded
        ''' in an  java.lang.[Integer]. The modifiers consist of the Java Virtual Machine's
        ''' constants for {@code public}, {@code protected},
        ''' {@code private}, {@code final}, {@code static},
        ''' {@code abstract} and {@code interface}; they should be decoded
        ''' using the methods of class {@code Modifier}.
        ''' 
        ''' <p> If the underlying class is an array [Class], then its
        ''' {@code public}, {@code private} and {@code protected}
        ''' modifiers are the same as those of its component type.  If this
        ''' {@code Class} represents a primitive type or void, its
        ''' {@code public} modifier is always {@code true}, and its
        ''' {@code protected} and {@code private} modifiers are always
        ''' {@code false}. If this object represents an array [Class], a
        ''' primitive type or void, then its {@code final} modifier is always
        ''' {@code true} and its interface modifier is always
        ''' {@code false}. The values of its other modifiers are not determined
        ''' by this specification.
        ''' 
        ''' <p> The modifier encodings are defined in <em>The Java Virtual Machine
        ''' Specification</em>, table 4.1.
        ''' </summary>
        ''' <returns> the {@code int} representing the modifiers for this class </returns>
        ''' <seealso cref=     java.lang.reflect.Modifier
        ''' @since JDK1.1 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function getModifiers() As Integer
        End Function


        ''' <summary>
        ''' Gets the signers of this class.
        ''' </summary>
        ''' <returns>  the signers of this [Class], or null if there are no signers.  In
        '''          particular, this method returns null if this object represents
        '''          a primitive type or void.
        ''' @since   JDK1.1 </returns>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Function getSigners() As Object()
        End Function


        ''' <summary>
        ''' Set the signers of this class.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Friend Sub setSigners(ByVal signers As Object())
        End Sub


        ''' <summary>
        ''' If this {@code Class} object represents a local or anonymous
        ''' class within a method, returns a {@link
        ''' java.lang.reflect.Method Method} object representing the
        ''' immediately enclosing method of the underlying class. Returns
        ''' {@code null} otherwise.
        ''' 
        ''' In particular, this method returns {@code null} if the underlying
        ''' class is a local or anonymous class immediately enclosed by a type
        ''' declaration, instance initializer or static initializer.
        ''' </summary>
        ''' <returns> the immediately enclosing method of the underlying [Class], if
        '''     that class is a local or anonymous class; otherwise {@code null}.
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and any of the
        '''         following conditions is met:
        ''' 
        '''         <ul>
        ''' 
        '''         <li> the caller's class loader is not the same as the
        '''         class loader of the enclosing class and invocation of
        '''         {@link SecurityManager#checkPermission
        '''         s.checkPermission} method with
        '''         {@code RuntimePermission("accessDeclaredMembers")}
        '''         denies access to the methods within the enclosing class
        ''' 
        '''         <li> the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the enclosing class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of the enclosing class
        ''' 
        '''         </ul>
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property enclosingMethod As Method
            Get
                Dim enclosingInfo As EnclosingMethodInfo = enclosingMethodInfo

                If enclosingInfo Is Nothing Then
                    Return Nothing
                Else
                    If Not enclosingInfo.method Then Return Nothing

                    Dim typeInfo As sun.reflect.generics.repository.MethodRepository = sun.reflect.generics.repository.MethodRepository.make(enclosingInfo.descriptor, factory)
                    Dim returnType As [Class] = toClass(typeInfo.returnType)
                    Dim parameterTypes As Type() = typeInfo.parameterTypes
                    Dim parameterClasses As [Class]() = New [Class](parameterTypes.Length - 1) {}

                    ' Convert Types to Classes; returned types *should*
                    ' be class objects since the methodDescriptor's used
                    ' don't have generics information
                    For i As Integer = 0 To parameterClasses.Length - 1
                        parameterClasses(i) = toClass(parameterTypes(i))
                    Next i

                    ' Perform access check
                    Dim enclosingCandidate As [Class] = enclosingInfo.enclosingClass
                    enclosingCandidate.checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
                    '            
                    '             * Loop over all declared methods; match method name,
                    '             * number of and type of parameters, *and* return
                    '             * type.  Matching return type is also necessary
                    '             * because of covariant returns, etc.
                    '             
                    For Each m As Method In enclosingCandidate.declaredMethods
                        If m.name.Equals(enclosingInfo.name) Then
                            Dim candidateParamClasses As  [Class]() = m.parameterTypes
                            If candidateParamClasses.Length = parameterClasses.Length Then
                                Dim matches As Boolean = True
                                For i As Integer = 0 To candidateParamClasses.Length - 1
                                    If Not candidateParamClasses(i).Equals(parameterClasses(i)) Then
                                        matches = False
                                        Exit For
                                    End If
                                Next i

                                If matches Then ' finally, check return type
                                    If m.returnType.Equals(returnType) Then Return m
                                End If
                            End If
                        End If
                    Next m

                    Throw New InternalError("Enclosing method not found")
                End If
            End Get
        End Property

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getEnclosingMethod0() As Object()
        End Function

        Private ReadOnly Property enclosingMethodInfo As EnclosingMethodInfo
            Get
                Dim enclosingInfo As Object() = enclosingMethod0
                If enclosingInfo Is Nothing Then
                    Return Nothing
                Else
                    Return New EnclosingMethodInfo(enclosingInfo)
                End If
            End Get
        End Property

        Private NotInheritable Class EnclosingMethodInfo
            Private enclosingClass As [Class]
            Private name As String
            Private descriptor As String

            Private Sub New(ByVal enclosingInfo As Object())
                If enclosingInfo.Length <> 3 Then Throw New InternalError("Malformed enclosing method information")
                Try
                    ' The array is expected to have three elements:

                    ' the immediately enclosing class
                    enclosingClass = CType(enclosingInfo(0), [Class])
                    Assert(enclosingClass IsNot Nothing)

                    ' the immediately enclosing method or constructor's
                    ' name (can be null).
                    name = CStr(enclosingInfo(1))

                    ' the immediately enclosing method or constructor's
                    ' descriptor (null iff name is).
                    descriptor = CStr(enclosingInfo(2))
                    Assert((name IsNot Nothing AndAlso descriptor IsNot Nothing) OrElse name = descriptor)
                Catch cce As  [Class]CastException
                    Throw New InternalError("Invalid type in enclosing method information", cce)
                End Try
            End Sub

            Friend ReadOnly Property [partial] As Boolean
                Get
                    Return enclosingClass Is Nothing OrElse name Is Nothing OrElse descriptor Is Nothing
                End Get
            End Property

            Friend ReadOnly Property constructor As Boolean
                Get
                    Return (Not [partial]) AndAlso "<init>".Equals(name)
                End Get
            End Property

            Friend ReadOnly Property method As Boolean
                Get
                    Return (Not [partial]) AndAlso (Not constructor) AndAlso Not "<clinit>".Equals(name)
                End Get
            End Property

            Friend ReadOnly Property enclosingClass As [Class]
                Get
                    Return enclosingClass
                End Get
            End Property

            Friend ReadOnly Property name As String
                Get
                    Return name
                End Get
            End Property

            Friend ReadOnly Property descriptor As String
                Get
                    Return descriptor
                End Get
            End Property

        End Class

        Private Shared Function toClass(ByVal o As Type) As [Class]
            If TypeOf o Is GenericArrayType Then Return Array.newInstance(toClass(CType(o, GenericArrayType).genericComponentType), 0).GetType()
            Return CType(o, [Class])
        End Function

        ''' <summary>
        ''' If this {@code Class} object represents a local or anonymous
        ''' class within a constructor, returns a {@link
        ''' java.lang.reflect.Constructor Constructor} object representing
        ''' the immediately enclosing constructor of the underlying
        ''' class. Returns {@code null} otherwise.  In particular, this
        ''' method returns {@code null} if the underlying class is a local
        ''' or anonymous class immediately enclosed by a type declaration,
        ''' instance initializer or static initializer.
        ''' </summary>
        ''' <returns> the immediately enclosing constructor of the underlying [Class], if
        '''     that class is a local or anonymous class; otherwise {@code null}. </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and any of the
        '''         following conditions is met:
        ''' 
        '''         <ul>
        ''' 
        '''         <li> the caller's class loader is not the same as the
        '''         class loader of the enclosing class and invocation of
        '''         {@link SecurityManager#checkPermission
        '''         s.checkPermission} method with
        '''         {@code RuntimePermission("accessDeclaredMembers")}
        '''         denies access to the constructors within the enclosing class
        ''' 
        '''         <li> the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the enclosing class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of the enclosing class
        ''' 
        '''         </ul>
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public ReadOnly Property enclosingConstructor As Constructor(Of ?)
            Get
                Dim enclosingInfo As EnclosingMethodInfo = enclosingMethodInfo

                If enclosingInfo Is Nothing Then
                    Return Nothing
                Else
                    If Not enclosingInfo.constructor Then Return Nothing

                    Dim typeInfo As sun.reflect.generics.repository.ConstructorRepository = sun.reflect.generics.repository.ConstructorRepository.make(enclosingInfo.descriptor, factory)
                    Dim parameterTypes As Type() = typeInfo.parameterTypes
                    Dim parameterClasses As  [Class]() = New [Class](parameterTypes.Length - 1) {}

                    ' Convert Types to Classes; returned types *should*
                    ' be class objects since the methodDescriptor's used
                    ' don't have generics information
                    For i As Integer = 0 To parameterClasses.Length - 1
                        parameterClasses(i) = toClass(parameterTypes(i))
                    Next i

                    ' Perform access check
                    Dim enclosingCandidate As  [Class] = enclosingInfo.enclosingClass
                    enclosingCandidate.checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
                    '            
                    '             * Loop over all declared constructors; match number
                    '             * of and type of parameters.
                    '             
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    For Each c As Constructor(Of ?) In enclosingCandidate.declaredConstructors
                        Dim candidateParamClasses As  [Class]() = c.parameterTypes
                        If candidateParamClasses.Length = parameterClasses.Length Then
                            Dim matches As Boolean = True
                            For i As Integer = 0 To candidateParamClasses.Length - 1
                                If Not candidateParamClasses(i).Equals(parameterClasses(i)) Then
                                    matches = False
                                    Exit For
                                End If
                            Next i

                            If matches Then Return c
                        End If
                    Next c

                    Throw New InternalError("Enclosing constructor not found")
                End If
            End Get
        End Property


        ''' <summary>
        ''' If the class or interface represented by this {@code Class} object
        ''' is a member of another [Class], returns the {@code Class} object
        ''' representing the class in which it was declared.  This method returns
        ''' null if this class or interface is not a member of any other class.  If
        ''' this {@code Class} object represents an array [Class], a primitive
        ''' type, or void,then this method returns null.
        ''' </summary>
        ''' <returns> the declaring class for this class </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and the caller's
        '''         class loader is not the same as or an ancestor of the class
        '''         loader for the declaring class and invocation of {@link
        '''         SecurityManager#checkPackageAccess s.checkPackageAccess()}
        '''         denies access to the package of the declaring class
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Property declaringClass As [Class]
            Get
                Dim candidate As [Class] = declaringClass0

                If candidate IsNot Nothing Then candidate.checkPackageAccess(classLoader.getClassLoader(sun.reflect.Reflection.callerClass), True)
                Return candidate
            End Get
        End Property

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getDeclaringClass0() As [Class]
        End Function


        ''' <summary>
        ''' Returns the immediately enclosing class of the underlying
        ''' class.  If the underlying class is a top level class this
        ''' method returns {@code null}. </summary>
        ''' <returns> the immediately enclosing class of the underlying class </returns>
        ''' <exception cref="SecurityException">
        '''             If a security manager, <i>s</i>, is present and the caller's
        '''             class loader is not the same as or an ancestor of the class
        '''             loader for the enclosing class and invocation of {@link
        '''             SecurityManager#checkPackageAccess s.checkPackageAccess()}
        '''             denies access to the package of the enclosing class
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Property enclosingClass As [Class]
            Get
                ' There are five kinds of classes (or interfaces):
                ' a) Top level classes
                ' b) Nested classes (static member classes)
                ' c) Inner classes (non-static member classes)
                ' d) Local classes (named classes declared within a method)
                ' e) Anonymous classes


                ' JVM Spec 4.8.6: A class must have an EnclosingMethod
                ' attribute if and only if it is a local class or an
                ' anonymous class.
                Dim enclosingInfo As EnclosingMethodInfo = enclosingMethodInfo
                Dim enclosingCandidate As  [Class]
    
				If enclosingInfo Is Nothing Then
                    ' This is a top level or a nested class or an inner class (a, b, or c)
                    enclosingCandidate = declaringClass
                Else
                    Dim enclosingClass_Renamed As  [Class] = enclosingInfo.enclosingClass
                    ' This is a local class or an anonymous class (d or e)
                    If enclosingClass_Renamed Is Me OrElse enclosingClass_Renamed Is Nothing Then
                        Throw New InternalError("Malformed enclosing method information")
                    Else
                        enclosingCandidate = enclosingClass_Renamed
                    End If
                End If

                If enclosingCandidate IsNot Nothing Then enclosingCandidate.checkPackageAccess(classLoader.getClassLoader(sun.reflect.Reflection.callerClass), True)
                Return enclosingCandidate
            End Get
        End Property

        ''' <summary>
        ''' Returns the simple name of the underlying class as given in the
        ''' source code. Returns an empty string if the underlying class is
        ''' anonymous.
        ''' 
        ''' <p>The simple name of an array is the simple name of the
        ''' component type with "[]" appended.  In particular the simple
        ''' name of an array whose component type is anonymous is "[]".
        ''' </summary>
        ''' <returns> the simple name of the underlying class
        ''' @since 1.5 </returns>
        Public Property simpleName As String
            Get
                If Array Then Return componentType.simpleName & "[]"

                Dim simpleName_Renamed As String = simpleBinaryName
                If simpleName_Renamed Is Nothing Then ' top level class
                    simpleName_Renamed = name
                    Return simpleName_Renamed.Substring(simpleName_Renamed.LastIndexOf(".") + 1) ' strip the package name
                End If
                ' According to JLS3 "Binary Compatibility" (13.1) the binary
                ' name of non-package classes (not top level) is the binary
                ' name of the immediately enclosing class followed by a '$' followed by:
                ' (for nested and inner classes): the simple name.
                ' (for local classes): 1 or more digits followed by the simple name.
                ' (for anonymous classes): 1 or more digits.

                ' Since getSimpleBinaryName() will strip the binary name of
                ' the immediatly enclosing [Class], we are now looking at a
                ' string that matches the regular expression "\$[0-9]*"
                ' followed by a simple name (considering the simple of an
                ' anonymous class to be the empty string).

                ' Remove leading "\$[0-9]*" from the name
                Dim length As Integer = simpleName_Renamed.Length()
                If length < 1 OrElse simpleName_Renamed.Chars(0) <> "$"c Then Throw New InternalError("Malformed class name")
                Dim index As Integer = 1
                Do While index < length AndAlso isAsciiDigit(simpleName_Renamed.Chars(index))
                    index += 1
                Loop
                ' Eventually, this is the empty string iff this is an anonymous class
                Return simpleName_Renamed.Substring(index)
            End Get
        End Property

        ''' <summary>
        ''' Return an informative string for the name of this type.
        ''' </summary>
        ''' <returns> an informative string for the name of this type
        ''' @since 1.8 </returns>
        Public Property typeName As String Implements Type.getTypeName
            Get
                If Array Then
                    Try
                        Dim cl As [Class] = Me
                        Dim dimensions As Integer = 0
                        Do While cl.array
                            dimensions += 1
                            cl = cl.componentType
                        Loop
                        Dim sb As New StringBuilder
                        sb.append(cl.name)
                        For i As Integer = 0 To dimensions - 1
                            sb.append("[]")
                        Next i
                        Return sb.ToString() 'FALLTHRU
                    Catch e As Throwable
                    End Try
                End If
                Return name
            End Get
        End Property

        ''' <summary>
        ''' Character.isDigit answers {@code true} to some non-ascii
        ''' digits.  This one does not.
        ''' </summary>
        Private Shared Function isAsciiDigit(ByVal c As Char) As Boolean
            Return "0"c <= c AndAlso c <= "9"c
        End Function

        ''' <summary>
        ''' Returns the canonical name of the underlying class as
        ''' defined by the Java Language Specification.  Returns null if
        ''' the underlying class does not have a canonical name (i.e., if
        ''' it is a local or anonymous class or an array whose component
        ''' type does not have a canonical name). </summary>
        ''' <returns> the canonical name of the underlying class if it exists, and
        ''' {@code null} otherwise.
        ''' @since 1.5 </returns>
        Public Property canonicalName As String
            Get
                If Array Then
                    Dim canonicalName_Renamed As String = componentType.canonicalName
                    If canonicalName_Renamed IsNot Nothing Then
                        Return canonicalName_Renamed & "[]"
                    Else
                        Return Nothing
                    End If
                End If
                If localOrAnonymousClass Then Return Nothing
                Dim enclosingClass_Renamed As  [Class] = enclosingClass
                If enclosingClass_Renamed Is Nothing Then ' top level class
                    Return name
                Else
                    Dim enclosingName As String = enclosingClass_Renamed.canonicalName
                    If enclosingName Is Nothing Then Return Nothing
                    Return enclosingName & "." & simpleName
                End If
            End Get
        End Property

        ''' <summary>
        ''' Returns {@code true} if and only if the underlying class
        ''' is an anonymous class.
        ''' </summary>
        ''' <returns> {@code true} if and only if this class is an anonymous class.
        ''' @since 1.5 </returns>
        Public ReadOnly Property anonymousClass As Boolean
            Get
                Return "".Equals(simpleName)
            End Get
        End Property

        ''' <summary>
        ''' Returns {@code true} if and only if the underlying class
        ''' is a local class.
        ''' </summary>
        ''' <returns> {@code true} if and only if this class is a local class.
        ''' @since 1.5 </returns>
        Public ReadOnly Property localClass As Boolean
            Get
                Return localOrAnonymousClass AndAlso Not anonymousClass
            End Get
        End Property

        ''' <summary>
        ''' Returns {@code true} if and only if the underlying class
        ''' is a member class.
        ''' </summary>
        ''' <returns> {@code true} if and only if this class is a member class.
        ''' @since 1.5 </returns>
        Public ReadOnly Property memberClass As Boolean
            Get
                Return simpleBinaryName IsNot Nothing AndAlso Not localOrAnonymousClass
            End Get
        End Property

        ''' <summary>
        ''' Returns the "simple binary name" of the underlying [Class], i.e.,
        ''' the binary name without the leading enclosing class name.
        ''' Returns {@code null} if the underlying class is a top level
        ''' class.
        ''' </summary>
        Private ReadOnly Property simpleBinaryName As String
            Get
                Dim enclosingClass_Renamed As [Class] = enclosingClass
                If enclosingClass_Renamed Is Nothing Then ' top level class Return Nothing
                    ' Otherwise, strip the enclosing class' name
                    Try
                        Return name.Substring(enclosingClass_Renamed.name.length())
                    Catch ex As IndexOutOfBoundsException
                        Throw New InternalError("Malformed class name", ex)
                    End Try
            End Get
        End Property

        ''' <summary>
        ''' Returns {@code true} if this is a local class or an anonymous
        ''' class.  Returns {@code false} otherwise.
        ''' </summary>
        Private ReadOnly Property localOrAnonymousClass As Boolean
            Get
                ' JVM Spec 4.8.6: A class must have an EnclosingMethod
                ' attribute if and only if it is a local class or an
                ' anonymous class.
                Return enclosingMethodInfo IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Returns an array containing {@code Class} objects representing all
        ''' the public classes and interfaces that are members of the class
        ''' represented by this {@code Class} object.  This includes public
        ''' class and interface members inherited from superclasses and public class
        ''' and interface members declared by the class.  This method returns an
        ''' array of length 0 if this {@code Class} object has no public member
        ''' classes or interfaces.  This method also returns an array of length 0 if
        ''' this {@code Class} object represents a primitive type, an array
        ''' [Class], or void.
        ''' </summary>
        ''' <returns> the array of {@code Class} objects representing the public
        '''         members of this class </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property classes As [Class]()
            Get
                checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, False)

                ' Privileged so this implementation can look at DECLARED classes,
                ' something the caller might not have privilege to do.  The code here
                ' is allowed to look at DECLARED classes because (1) it does not hand
                ' out anything other than public members and (2) public member access
                ' has already been ok'd by the SecurityManager.

                Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
            End Get
        End Property

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As  [Class]()
				Dim list As IList(Of [Class]) = New List(Of [Class])
                Dim currentClass As  [Class] = Class.this
				Do While currentClass IsNot Nothing
                    Dim members As  [Class]() = currentClass.declaredClasses
                    For i As Integer = 0 To members.Length - 1
                        If Modifier.isPublic(members(i).modifiers) Then list.Add(members(i))
                    Next i
                    currentClass = currentClass.BaseType
                Loop
                Return list.ToArray()
            End Function
        End Class


        ''' <summary>
        ''' Returns an array containing {@code Field} objects reflecting all
        ''' the accessible public fields of the class or interface represented by
        ''' this {@code Class} object.
        ''' 
        ''' <p> If this {@code Class} object represents a class or interface with no
        ''' no accessible public fields, then this method returns an array of length
        ''' 0.
        ''' 
        ''' <p> If this {@code Class} object represents a [Class], then this method
        ''' returns the public fields of the class and of all its superclasses.
        ''' 
        ''' <p> If this {@code Class} object represents an interface, then this
        ''' method returns the fields of the interface and of all its
        ''' superinterfaces.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, a primitive
        ''' type, or void, then this method returns an array of length 0.
        ''' 
        ''' <p> The elements in the returned array are not sorted and are not in any
        ''' particular order.
        ''' </summary>
        ''' <returns> the array of {@code Field} objects representing the
        '''         public fields </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @since JDK1.1
        ''' @jls 8.2 Class Members
        ''' @jls 8.3 Field Declarations </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property fields As Field()
            Get
                checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, True)
                Return copyFields(privateGetPublicFields(Nothing))
            End Get
        End Property


        ''' <summary>
        ''' Returns an array containing {@code Method} objects reflecting all the
        ''' public methods of the class or interface represented by this {@code
        ''' Class} object, including those declared by the class or interface and
        ''' those inherited from superclasses and superinterfaces.
        ''' 
        ''' <p> If this {@code Class} object represents a type that has multiple
        ''' public methods with the same name and parameter types, but different
        ''' return types, then the returned array has a {@code Method} object for
        ''' each such method.
        ''' 
        ''' <p> If this {@code Class} object represents a type with a class
        ''' initialization method {@code <clinit>}, then the returned array does
        ''' <em>not</em> have a corresponding {@code Method} object.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, then the
        ''' returned array has a {@code Method} object for each of the public
        ''' methods inherited by the array type from {@code Object}. It does not
        ''' contain a {@code Method} object for {@code clone()}.
        ''' 
        ''' <p> If this {@code Class} object represents an interface then the
        ''' returned array does not contain any implicitly declared methods from
        ''' {@code Object}. Therefore, if no methods are explicitly declared in
        ''' this interface or any of its superinterfaces then the returned array
        ''' has length 0. (Note that a {@code Class} object which represents a class
        ''' always has public methods, inherited from {@code Object}.)
        ''' 
        ''' <p> If this {@code Class} object represents a primitive type or void,
        ''' then the returned array has length 0.
        ''' 
        ''' <p> Static methods declared in superinterfaces of the class or interface
        ''' represented by this {@code Class} object are not considered members of
        ''' the class or interface.
        ''' 
        ''' <p> The elements in the returned array are not sorted and are not in any
        ''' particular order.
        ''' </summary>
        ''' <returns> the array of {@code Method} objects representing the
        '''         public methods of this class </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @jls 8.2 Class Members
        ''' @jls 8.4 Method Declarations
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public ReadOnly Property methods As Method()
            Get
                checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, True)
                Return copyMethods(privateGetPublicMethods())
            End Get
        End Property


        ''' <summary>
        ''' Returns an array containing {@code Constructor} objects reflecting
        ''' all the public constructors of the class represented by this
        ''' {@code Class} object.  An array of length 0 is returned if the
        ''' class has no public constructors, or if the class is an array [Class], or
        ''' if the class reflects a primitive type or void.
        ''' 
        ''' Note that while this method returns an array of {@code
        ''' Constructor<T>} objects (that is an array of constructors from
        ''' this [Class]), the return type of this method is {@code
        ''' Constructor<?>[]} and <em>not</em> {@code Constructor<T>[]} as
        ''' might be expected.  This less informative return type is
        ''' necessary since after being returned from this method, the
        ''' array could be modified to hold {@code Constructor} objects for
        ''' different classes, which would violate the type guarantees of
        ''' {@code Constructor<T>[]}.
        ''' </summary>
        ''' <returns> the array of {@code Constructor} objects representing the
        '''         public constructors of this class </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public ReadOnly Property constructors As Constructor(Of ?)()
            Get
                checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, True)
                Return copyConstructors(privateGetDeclaredConstructors(True))
            End Get
        End Property


        ''' <summary>
        ''' Returns a {@code Field} object that reflects the specified public member
        ''' field of the class or interface represented by this {@code Class}
        ''' object. The {@code name} parameter is a {@code String} specifying the
        ''' simple name of the desired field.
        ''' 
        ''' <p> The field to be reflected is determined by the algorithm that
        ''' follows.  Let C be the class or interface represented by this object:
        ''' 
        ''' <OL>
        ''' <LI> If C declares a public field with the name specified, that is the
        '''      field to be reflected.</LI>
        ''' <LI> If no field was found in step 1 above, this algorithm is applied
        '''      recursively to each direct superinterface of C. The direct
        '''      superinterfaces are searched in the order they were declared.</LI>
        ''' <LI> If no field was found in steps 1 and 2 above, and C has a
        '''      superclass S, then this algorithm is invoked recursively upon S.
        '''      If C has no superclass, then a {@code NoSuchFieldException}
        '''      is thrown.</LI>
        ''' </OL>
        ''' 
        ''' <p> If this {@code Class} object represents an array type, then this
        ''' method does not find the {@code length} field of the array type.
        ''' </summary>
        ''' <param name="name"> the field name </param>
        ''' <returns> the {@code Field} object of this class specified by
        '''         {@code name} </returns>
        ''' <exception cref="NoSuchFieldException"> if a field with the specified name is
        '''         not found. </exception>
        ''' <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @since JDK1.1
        ''' @jls 8.2 Class Members
        ''' @jls 8.3 Field Declarations </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getField(ByVal name As String) As Field
            checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, True)
            Dim field_Renamed As Field = getField0(name)
            If field_Renamed Is Nothing Then Throw New NoSuchFieldException(name)
            Return field_Renamed
        End Function


        ''' <summary>
        ''' Returns a {@code Method} object that reflects the specified public
        ''' member method of the class or interface represented by this
        ''' {@code Class} object. The {@code name} parameter is a
        ''' {@code String} specifying the simple name of the desired method. The
        ''' {@code parameterTypes} parameter is an array of {@code Class}
        ''' objects that identify the method's formal parameter types, in declared
        ''' order. If {@code parameterTypes} is {@code null}, it is
        ''' treated as if it were an empty array.
        ''' 
        ''' <p> If the {@code name} is "{@code <init>}" or "{@code <clinit>}" a
        ''' {@code NoSuchMethodException} is raised. Otherwise, the method to
        ''' be reflected is determined by the algorithm that follows.  Let C be the
        ''' class or interface represented by this object:
        ''' <OL>
        ''' <LI> C is searched for a <I>matching method</I>, as defined below. If a
        '''      matching method is found, it is reflected.</LI>
        ''' <LI> If no matching method is found by step 1 then:
        '''   <OL TYPE="a">
        '''   <LI> If C is a class other than {@code Object}, then this algorithm is
        '''        invoked recursively on the superclass of C.</LI>
        '''   <LI> If C is the class {@code Object}, or if C is an interface, then
        '''        the superinterfaces of C (if any) are searched for a matching
        '''        method. If any such method is found, it is reflected.</LI>
        '''   </OL></LI>
        ''' </OL>
        ''' 
        ''' <p> To find a matching method in a class or interface C:&nbsp; If C
        ''' declares exactly one public method with the specified name and exactly
        ''' the same formal parameter types, that is the method reflected. If more
        ''' than one such method is found in C, and one of these methods has a
        ''' return type that is more specific than any of the others, that method is
        ''' reflected; otherwise one of the methods is chosen arbitrarily.
        ''' 
        ''' <p>Note that there may be more than one matching method in a
        ''' class because while the Java language forbids a class to
        ''' declare multiple methods with the same signature but different
        ''' return types, the Java virtual machine does not.  This
        ''' increased flexibility in the virtual machine can be used to
        ''' implement various language features.  For example, covariant
        ''' returns can be implemented with {@linkplain
        ''' java.lang.reflect.Method#isBridge bridge methods}; the bridge
        ''' method and the method being overridden would have the same
        ''' signature but different return types.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, then this
        ''' method does not find the {@code clone()} method.
        ''' 
        ''' <p> Static methods declared in superinterfaces of the class or interface
        ''' represented by this {@code Class} object are not considered members of
        ''' the class or interface.
        ''' </summary>
        ''' <param name="name"> the name of the method </param>
        ''' <param name="parameterTypes"> the list of parameters </param>
        ''' <returns> the {@code Method} object that matches the specified
        '''         {@code name} and {@code parameterTypes} </returns>
        ''' <exception cref="NoSuchMethodException"> if a matching method is not found
        '''         or if the name is "&lt;init&gt;"or "&lt;clinit&gt;". </exception>
        ''' <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @jls 8.2 Class Members
        ''' @jls 8.4 Method Declarations
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getMethod(ByVal name As String, ParamArray ByVal parameterTypes As  [Class]()) As Method
			checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, True)
            Dim method_Renamed As Method = getMethod0(name, parameterTypes, True)
            If method_Renamed Is Nothing Then Throw New NoSuchMethodException(name & "." & name + argumentTypesToString(parameterTypes))
            Return method_Renamed
        End Function


        ''' <summary>
        ''' Returns a {@code Constructor} object that reflects the specified
        ''' public constructor of the class represented by this {@code Class}
        ''' object. The {@code parameterTypes} parameter is an array of
        ''' {@code Class} objects that identify the constructor's formal
        ''' parameter types, in declared order.
        ''' 
        ''' If this {@code Class} object represents an inner class
        ''' declared in a non-static context, the formal parameter types
        ''' include the explicit enclosing instance as the first parameter.
        ''' 
        ''' <p> The constructor to reflect is the public constructor of the class
        ''' represented by this {@code Class} object whose formal parameter
        ''' types match those specified by {@code parameterTypes}.
        ''' </summary>
        ''' <param name="parameterTypes"> the parameter array </param>
        ''' <returns> the {@code Constructor} object of the public constructor that
        '''         matches the specified {@code parameterTypes} </returns>
        ''' <exception cref="NoSuchMethodException"> if a matching method is not found. </exception>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and
        '''         the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class.
        ''' 
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getConstructor(ParamArray ByVal parameterTypes As  [Class]()) As Constructor(Of T)
			checkMemberAccess(Member.PUBLIC, sun.reflect.Reflection.callerClass, True)
            Return getConstructor0(parameterTypes, Member.PUBLIC)
        End Function


        ''' <summary>
        ''' Returns an array of {@code Class} objects reflecting all the
        ''' classes and interfaces declared as members of the class represented by
        ''' this {@code Class} object. This includes public, protected, default
        ''' (package) access, and private classes and interfaces declared by the
        ''' [Class], but excludes inherited classes and interfaces.  This method
        ''' returns an array of length 0 if the class declares no classes or
        ''' interfaces as members, or if this {@code Class} object represents a
        ''' primitive type, an array [Class], or void.
        ''' </summary>
        ''' <returns> the array of {@code Class} objects representing all the
        '''         declared members of this class </returns>
        ''' <exception cref="SecurityException">
        '''         If a security manager, <i>s</i>, is present and any of the
        '''         following conditions is met:
        ''' 
        '''         <ul>
        ''' 
        '''         <li> the caller's class loader is not the same as the
        '''         class loader of this class and invocation of
        '''         {@link SecurityManager#checkPermission
        '''         s.checkPermission} method with
        '''         {@code RuntimePermission("accessDeclaredMembers")}
        '''         denies access to the declared classes within this class
        ''' 
        '''         <li> the caller's class loader is not the same as or an
        '''         ancestor of the class loader for the current class and
        '''         invocation of {@link SecurityManager#checkPackageAccess
        '''         s.checkPackageAccess()} denies access to the package
        '''         of this class
        ''' 
        '''         </ul>
        ''' 
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Property declaredClasses As  [Class]()
			Get
                checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, False)
                Return declaredClasses0
            End Get
        End Property


        ''' <summary>
        ''' Returns an array of {@code Field} objects reflecting all the fields
        ''' declared by the class or interface represented by this
        ''' {@code Class} object. This includes public, protected, default
        ''' (package) access, and private fields, but excludes inherited fields.
        ''' 
        ''' <p> If this {@code Class} object represents a class or interface with no
        ''' declared fields, then this method returns an array of length 0.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, a primitive
        ''' type, or void, then this method returns an array of length 0.
        ''' 
        ''' <p> The elements in the returned array are not sorted and are not in any
        ''' particular order.
        ''' </summary>
        ''' <returns>  the array of {@code Field} objects representing all the
        '''          declared fields of this class </returns>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and any of the
        '''          following conditions is met:
        ''' 
        '''          <ul>
        ''' 
        '''          <li> the caller's class loader is not the same as the
        '''          class loader of this class and invocation of
        '''          {@link SecurityManager#checkPermission
        '''          s.checkPermission} method with
        '''          {@code RuntimePermission("accessDeclaredMembers")}
        '''          denies access to the declared fields within this class
        ''' 
        '''          <li> the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class
        ''' 
        '''          </ul>
        ''' 
        ''' @since JDK1.1
        ''' @jls 8.2 Class Members
        ''' @jls 8.3 Field Declarations </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Property declaredFields As Field()
            Get
                checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
                Return copyFields(privateGetDeclaredFields(False))
            End Get
        End Property


        ''' 
        ''' <summary>
        ''' Returns an array containing {@code Method} objects reflecting all the
        ''' declared methods of the class or interface represented by this {@code
        ''' Class} object, including public, protected, default (package)
        ''' access, and private methods, but excluding inherited methods.
        ''' 
        ''' <p> If this {@code Class} object represents a type that has multiple
        ''' declared methods with the same name and parameter types, but different
        ''' return types, then the returned array has a {@code Method} object for
        ''' each such method.
        ''' 
        ''' <p> If this {@code Class} object represents a type that has a class
        ''' initialization method {@code <clinit>}, then the returned array does
        ''' <em>not</em> have a corresponding {@code Method} object.
        ''' 
        ''' <p> If this {@code Class} object represents a class or interface with no
        ''' declared methods, then the returned array has length 0.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, a primitive
        ''' type, or void, then the returned array has length 0.
        ''' 
        ''' <p> The elements in the returned array are not sorted and are not in any
        ''' particular order.
        ''' </summary>
        ''' <returns>  the array of {@code Method} objects representing all the
        '''          declared methods of this class </returns>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and any of the
        '''          following conditions is met:
        ''' 
        '''          <ul>
        ''' 
        '''          <li> the caller's class loader is not the same as the
        '''          class loader of this class and invocation of
        '''          {@link SecurityManager#checkPermission
        '''          s.checkPermission} method with
        '''          {@code RuntimePermission("accessDeclaredMembers")}
        '''          denies access to the declared methods within this class
        ''' 
        '''          <li> the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class
        ''' 
        '''          </ul>
        ''' 
        ''' @jls 8.2 Class Members
        ''' @jls 8.4 Method Declarations
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Property declaredMethods As Method()
            Get
                checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
                Return copyMethods(privateGetDeclaredMethods(False))
            End Get
        End Property


        ''' <summary>
        ''' Returns an array of {@code Constructor} objects reflecting all the
        ''' constructors declared by the class represented by this
        ''' {@code Class} object. These are public, protected, default
        ''' (package) access, and private constructors.  The elements in the array
        ''' returned are not sorted and are not in any particular order.  If the
        ''' class has a default constructor, it is included in the returned array.
        ''' This method returns an array of length 0 if this {@code Class}
        ''' object represents an interface, a primitive type, an array [Class], or
        ''' void.
        ''' 
        ''' <p> See <em>The Java Language Specification</em>, section 8.2.
        ''' </summary>
        ''' <returns>  the array of {@code Constructor} objects representing all the
        '''          declared constructors of this class </returns>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and any of the
        '''          following conditions is met:
        ''' 
        '''          <ul>
        ''' 
        '''          <li> the caller's class loader is not the same as the
        '''          class loader of this class and invocation of
        '''          {@link SecurityManager#checkPermission
        '''          s.checkPermission} method with
        '''          {@code RuntimePermission("accessDeclaredMembers")}
        '''          denies access to the declared constructors within this class
        ''' 
        '''          <li> the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class
        ''' 
        '''          </ul>
        ''' 
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public Property declaredConstructors As Constructor(Of ?)()
            Get
                checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
                Return copyConstructors(privateGetDeclaredConstructors(False))
            End Get
        End Property


        ''' <summary>
        ''' Returns a {@code Field} object that reflects the specified declared
        ''' field of the class or interface represented by this {@code Class}
        ''' object. The {@code name} parameter is a {@code String} that specifies
        ''' the simple name of the desired field.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, then this
        ''' method does not find the {@code length} field of the array type.
        ''' </summary>
        ''' <param name="name"> the name of the field </param>
        ''' <returns>  the {@code Field} object for the specified field in this
        '''          class </returns>
        ''' <exception cref="NoSuchFieldException"> if a field with the specified name is
        '''          not found. </exception>
        ''' <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and any of the
        '''          following conditions is met:
        ''' 
        '''          <ul>
        ''' 
        '''          <li> the caller's class loader is not the same as the
        '''          class loader of this class and invocation of
        '''          {@link SecurityManager#checkPermission
        '''          s.checkPermission} method with
        '''          {@code RuntimePermission("accessDeclaredMembers")}
        '''          denies access to the declared field
        ''' 
        '''          <li> the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class
        ''' 
        '''          </ul>
        ''' 
        ''' @since JDK1.1
        ''' @jls 8.2 Class Members
        ''' @jls 8.3 Field Declarations </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getDeclaredField(ByVal name As String) As Field
            checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
            Dim field_Renamed As Field = searchFields(privateGetDeclaredFields(False), name)
            If field_Renamed Is Nothing Then Throw New NoSuchFieldException(name)
            Return field_Renamed
        End Function


        ''' <summary>
        ''' Returns a {@code Method} object that reflects the specified
        ''' declared method of the class or interface represented by this
        ''' {@code Class} object. The {@code name} parameter is a
        ''' {@code String} that specifies the simple name of the desired
        ''' method, and the {@code parameterTypes} parameter is an array of
        ''' {@code Class} objects that identify the method's formal parameter
        ''' types, in declared order.  If more than one method with the same
        ''' parameter types is declared in a [Class], and one of these methods has a
        ''' return type that is more specific than any of the others, that method is
        ''' returned; otherwise one of the methods is chosen arbitrarily.  If the
        ''' name is "&lt;init&gt;"or "&lt;clinit&gt;" a {@code NoSuchMethodException}
        ''' is raised.
        ''' 
        ''' <p> If this {@code Class} object represents an array type, then this
        ''' method does not find the {@code clone()} method.
        ''' </summary>
        ''' <param name="name"> the name of the method </param>
        ''' <param name="parameterTypes"> the parameter array </param>
        ''' <returns>  the {@code Method} object for the method of this class
        '''          matching the specified name and parameters </returns>
        ''' <exception cref="NoSuchMethodException"> if a matching method is not found. </exception>
        ''' <exception cref="NullPointerException"> if {@code name} is {@code null} </exception>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and any of the
        '''          following conditions is met:
        ''' 
        '''          <ul>
        ''' 
        '''          <li> the caller's class loader is not the same as the
        '''          class loader of this class and invocation of
        '''          {@link SecurityManager#checkPermission
        '''          s.checkPermission} method with
        '''          {@code RuntimePermission("accessDeclaredMembers")}
        '''          denies access to the declared method
        ''' 
        '''          <li> the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class
        ''' 
        '''          </ul>
        ''' 
        ''' @jls 8.2 Class Members
        ''' @jls 8.4 Method Declarations
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getDeclaredMethod(ByVal name As String, ParamArray ByVal parameterTypes As  [Class]()) As Method
			checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
            Dim method_Renamed As Method = searchMethods(privateGetDeclaredMethods(False), name, parameterTypes)
            If method_Renamed Is Nothing Then Throw New NoSuchMethodException(name & "." & name + argumentTypesToString(parameterTypes))
            Return method_Renamed
        End Function


        ''' <summary>
        ''' Returns a {@code Constructor} object that reflects the specified
        ''' constructor of the class or interface represented by this
        ''' {@code Class} object.  The {@code parameterTypes} parameter is
        ''' an array of {@code Class} objects that identify the constructor's
        ''' formal parameter types, in declared order.
        ''' 
        ''' If this {@code Class} object represents an inner class
        ''' declared in a non-static context, the formal parameter types
        ''' include the explicit enclosing instance as the first parameter.
        ''' </summary>
        ''' <param name="parameterTypes"> the parameter array </param>
        ''' <returns>  The {@code Constructor} object for the constructor with the
        '''          specified parameter list </returns>
        ''' <exception cref="NoSuchMethodException"> if a matching method is not found. </exception>
        ''' <exception cref="SecurityException">
        '''          If a security manager, <i>s</i>, is present and any of the
        '''          following conditions is met:
        ''' 
        '''          <ul>
        ''' 
        '''          <li> the caller's class loader is not the same as the
        '''          class loader of this class and invocation of
        '''          {@link SecurityManager#checkPermission
        '''          s.checkPermission} method with
        '''          {@code RuntimePermission("accessDeclaredMembers")}
        '''          denies access to the declared constructor
        ''' 
        '''          <li> the caller's class loader is not the same as or an
        '''          ancestor of the class loader for the current class and
        '''          invocation of {@link SecurityManager#checkPackageAccess
        '''          s.checkPackageAccess()} denies access to the package
        '''          of this class
        ''' 
        '''          </ul>
        ''' 
        ''' @since JDK1.1 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getDeclaredConstructor(ParamArray ByVal parameterTypes As  [Class]()) As Constructor(Of T)
			checkMemberAccess(Member.DECLARED, sun.reflect.Reflection.callerClass, True)
            Return getConstructor0(parameterTypes, Member.DECLARED)
        End Function

        ''' <summary>
        ''' Finds a resource with a given name.  The rules for searching resources
        ''' associated with a given class are implemented by the defining
        ''' <seealso cref="ClassLoader class loader"/> of the class.  This method
        ''' delegates to this object's class loader.  If this object was loaded by
        ''' the bootstrap class loader, the method delegates to {@link
        ''' ClassLoader#getSystemResourceAsStream}.
        ''' 
        ''' <p> Before delegation, an absolute resource name is constructed from the
        ''' given resource name using this algorithm:
        ''' 
        ''' <ul>
        ''' 
        ''' <li> If the {@code name} begins with a {@code '/'}
        ''' (<tt>'&#92;u002f'</tt>), then the absolute name of the resource is the
        ''' portion of the {@code name} following the {@code '/'}.
        ''' 
        ''' <li> Otherwise, the absolute name is of the following form:
        ''' 
        ''' <blockquote>
        '''   {@code modified_package_name/name}
        ''' </blockquote>
        ''' 
        ''' <p> Where the {@code modified_package_name} is the package name of this
        ''' object with {@code '/'} substituted for {@code '.'}
        ''' (<tt>'&#92;u002e'</tt>).
        ''' 
        ''' </ul>
        ''' </summary>
        ''' <param name="name"> name of the desired resource </param>
        ''' <returns>      A <seealso cref="java.io.InputStream"/> object or {@code null} if
        '''              no resource with this name is found </returns>
        ''' <exception cref="NullPointerException"> If {@code name} is {@code null}
        ''' @since  JDK1.1 </exception>
        Public Function getResourceAsStream(ByVal name As String) As java.io.InputStream
            name = resolveName(name)
            Dim cl As  [Class]Loader = classLoader0
            If cl Is Nothing Then Return classLoader.getSystemResourceAsStream(name)
            Return cl.getResourceAsStream(name)
        End Function

        ''' <summary>
        ''' Finds a resource with a given name.  The rules for searching resources
        ''' associated with a given class are implemented by the defining
        ''' <seealso cref="ClassLoader class loader"/> of the class.  This method
        ''' delegates to this object's class loader.  If this object was loaded by
        ''' the bootstrap class loader, the method delegates to {@link
        ''' ClassLoader#getSystemResource}.
        ''' 
        ''' <p> Before delegation, an absolute resource name is constructed from the
        ''' given resource name using this algorithm:
        ''' 
        ''' <ul>
        ''' 
        ''' <li> If the {@code name} begins with a {@code '/'}
        ''' (<tt>'&#92;u002f'</tt>), then the absolute name of the resource is the
        ''' portion of the {@code name} following the {@code '/'}.
        ''' 
        ''' <li> Otherwise, the absolute name is of the following form:
        ''' 
        ''' <blockquote>
        '''   {@code modified_package_name/name}
        ''' </blockquote>
        ''' 
        ''' <p> Where the {@code modified_package_name} is the package name of this
        ''' object with {@code '/'} substituted for {@code '.'}
        ''' (<tt>'&#92;u002e'</tt>).
        ''' 
        ''' </ul>
        ''' </summary>
        ''' <param name="name"> name of the desired resource </param>
        ''' <returns>      A  <seealso cref="java.net.URL"/> object or {@code null} if no
        '''              resource with this name is found
        ''' @since  JDK1.1 </returns>
        Public Function getResource(ByVal name As String) As java.net.URL
            name = resolveName(name)
            Dim cl As  [Class]Loader = classLoader0
            If cl Is Nothing Then Return classLoader.getSystemResource(name)
            Return cl.getResource(name)
        End Function



        ''' <summary>
        ''' protection domain returned when the internal domain is null </summary>
        Private Shared allPermDomain As java.security.ProtectionDomain


        ''' <summary>
        ''' Returns the {@code ProtectionDomain} of this class.  If there is a
        ''' security manager installed, this method first calls the security
        ''' manager's {@code checkPermission} method with a
        ''' {@code RuntimePermission("getProtectionDomain")} permission to
        ''' ensure it's ok to get the
        ''' {@code ProtectionDomain}.
        ''' </summary>
        ''' <returns> the ProtectionDomain of this class
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        {@code checkPermission} method doesn't allow
        '''        getting the ProtectionDomain.
        ''' </exception>
        ''' <seealso cref= java.security.ProtectionDomain </seealso>
        ''' <seealso cref= SecurityManager#checkPermission </seealso>
        ''' <seealso cref= java.lang.RuntimePermission
        ''' @since 1.2 </seealso>
        Public Property protectionDomain As java.security.ProtectionDomain
            Get
                Dim sm As SecurityManager = System.securityManager
                If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.GET_PD_PERMISSION)
                Dim pd As java.security.ProtectionDomain = protectionDomain0
                If pd Is Nothing Then
                    If allPermDomain Is Nothing Then
                        Dim perms As New java.security.Permissions
                        perms.add(sun.security.util.SecurityConstants.ALL_PERMISSION)
                        allPermDomain = New java.security.ProtectionDomain(Nothing, perms)
                    End If
                    pd = allPermDomain
                End If
                Return pd
            End Get
        End Property


        ''' <summary>
        ''' Returns the ProtectionDomain of this class.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getProtectionDomain0() As java.security.ProtectionDomain
        End Function

        '    
        '     * Return the Virtual Machine's Class object for the named
        '     * primitive type.
        '     
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Friend Shared Function getPrimitiveClass(ByVal name As String) As  [Class]
		End Function

        '    
        '     * Check if client is allowed to access members.  If access is denied,
        '     * throw a SecurityException.
        '     *
        '     * This method also enforces package access.
        '     *
        '     * <p> Default policy: allow all clients access with normal Java access
        '     * control.
        '     
        Private Sub checkMemberAccess(ByVal which As Integer, ByVal caller As [Class], ByVal checkProxyInterfaces As Boolean)
            Dim s As SecurityManager = System.securityManager
            If s IsNot Nothing Then
                '             Default policy allows access to all {@link Member#PUBLIC} members,
                '             * as well as access to classes that have the same class loader as the caller.
                '             * In all other cases, it requires RuntimePermission("accessDeclaredMembers")
                '             * permission.
                '             
                Dim ccl As  [Class]Loader = classLoader.getClassLoader(caller)
                Dim cl As  [Class]Loader = classLoader0
                If which <> Member.PUBLIC Then
                    If ccl IsNot cl Then s.checkPermission(sun.security.util.SecurityConstants.CHECK_MEMBER_ACCESS_PERMISSION)
                End If
                Me.checkPackageAccess(ccl, checkProxyInterfaces)
            End If
        End Sub

        '    
        '     * Checks if a client loaded in ClassLoader ccl is allowed to access this
        '     * class under the current package access policy. If access is denied,
        '     * throw a SecurityException.
        '     
        Private Sub checkPackageAccess(ByVal ccl As  [Class]Loader, ByVal checkProxyInterfaces As Boolean)
            Dim s As SecurityManager = System.securityManager
            If s IsNot Nothing Then
                Dim cl As  [Class]Loader = classLoader0

                If sun.reflect.misc.ReflectUtil.needsPackageAccessCheck(ccl, cl) Then
                    Dim name_Renamed As String = Me.name
                    Dim i As Integer = name_Renamed.LastIndexOf("."c)
                    If i <> -1 Then
                        ' skip the package access check on a proxy class in default proxy package
                        Dim pkg As String = name_Renamed.Substring(0, i)
                        If (Not Proxy.isProxyClass(Me)) OrElse sun.reflect.misc.ReflectUtil.isNonPublicProxyClass(Me) Then s.checkPackageAccess(pkg)
                    End If
                End If
                ' check package access on the proxy interfaces
                If checkProxyInterfaces AndAlso Proxy.isProxyClass(Me) Then sun.reflect.misc.ReflectUtil.checkProxyPackageAccess(ccl, Me.interfaces)
            End If
        End Sub

        ''' <summary>
        ''' Add a package name prefix if the name is not absolute Remove leading "/"
        ''' if name is absolute
        ''' </summary>
        Private Function resolveName(ByVal name As String) As String
            If name Is Nothing Then Return name
            If Not name.StartsWith("/") Then
                Dim c As  [Class] = Me
                Do While c.array
                    c = c.componentType
                Loop
                Dim baseName As String = c.name
                Dim index As Integer = baseName.LastIndexOf("."c)
                If index <> -1 Then name = baseName.Substring(0, index).Replace("."c, "/"c) & "/" & name
            Else
                name = name.Substring(1)
            End If
            Return name
        End Function

        ''' <summary>
        ''' Atomic operations support.
        ''' </summary>
        Private Class Atomic
            ' initialize Unsafe machinery here, since we need to call Class.class instance method
            ' and have to avoid calling it in the static initializer of the Class class...
            Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
            ' offset of Class.reflectionData instance field
            Private Shared ReadOnly reflectionDataOffset As Long
            ' offset of Class.annotationType instance field
            Private Shared ReadOnly annotationTypeOffset As Long
            ' offset of Class.annotationData instance field
            Private Shared ReadOnly annotationDataOffset As Long


            Private Shared Function objectFieldOffset(ByVal fields As Field(), ByVal fieldName As String) As Long
                Dim field As Field = searchFields(fields, fieldName)
                If field Is Nothing Then Throw New [Error]("No " & fieldName & " field found in java.lang.Class")
                Return unsafe.objectFieldOffset(field)
            End Function

            Friend Shared Function casReflectionData(Of T)(ByVal clazz As [Class], ByVal oldData As SoftReference(Of ReflectionData(Of T)), ByVal newData As SoftReference(Of ReflectionData(Of T))) As Boolean
                Return unsafe.compareAndSwapObject(clazz, reflectionDataOffset, oldData, newData)
            End Function

            Friend Shared Function casAnnotationType(Of T)(ByVal clazz As [Class], ByVal oldType As AnnotationType, ByVal newType As AnnotationType) As Boolean
                Return unsafe.compareAndSwapObject(clazz, annotationTypeOffset, oldType, newType)
            End Function

            Friend Shared Function casAnnotationData(Of T)(ByVal clazz As [Class], ByVal oldData As AnnotationData, ByVal newData As AnnotationData) As Boolean
                Return unsafe.compareAndSwapObject(clazz, annotationDataOffset, oldData, newData)
            End Function
        End Class

        ''' <summary>
        ''' Reflection support.
        ''' </summary>

        ' Caches for certain reflective results
        Private Shared useCaches As Boolean = True

        ' reflection data that might get invalidated when JVM TI RedefineClasses() is called
        Private Class ReflectionData(Of T)
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend declaredFields As Field()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend publicFields As Field()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend declaredMethods As Method()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend publicMethods As Method()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend declaredConstructors As Constructor(Of T)()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend publicConstructors As Constructor(Of T)()
            ' Intermediate results for getFields and getMethods
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend declaredPublicFields As Field()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend declaredPublicMethods As Method()
            'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
            Friend interfaces As  [Class]()

			' Value of classRedefinedCount when we created this ReflectionData instance
			Friend ReadOnly redefinedCount As Integer

            Friend Sub New(ByVal redefinedCount As Integer)
                Me.redefinedCount = redefinedCount
            End Sub
        End Class

        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private reflectionData_Renamed As SoftReference(Of ReflectionData(Of T))

        ' Incremented by the VM on each call to JVM TI RedefineClasses()
        ' that redefines this class or a superclass.
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private classRedefinedCount As Integer = 0

        ' Lazily create and cache ReflectionData
        Private Function reflectionData() As ReflectionData(Of T)
            Dim reflectionData_Renamed As SoftReference(Of ReflectionData(Of T)) = Me.reflectionData_Renamed
            Dim classRedefinedCount As Integer = Me.classRedefinedCount
            Dim rd As ReflectionData(Of T)
            rd = reflectionData_Renamed.get()
            If useCaches AndAlso reflectionData_Renamed IsNot Nothing AndAlso rd IsNot Nothing AndAlso rd.redefinedCount = classRedefinedCount Then Return rd
            ' else no SoftReference or cleared SoftReference or stale ReflectionData
            ' -> create and replace new instance
            Return newReflectionData(reflectionData_Renamed, classRedefinedCount)
        End Function

        Private Function newReflectionData(ByVal oldReflectionData As SoftReference(Of ReflectionData(Of T)), ByVal classRedefinedCount As Integer) As ReflectionData(Of T)
            If Not useCaches Then Return Nothing

            Do
                Dim rd As New ReflectionData(Of T)(classRedefinedCount)
                ' try to CAS it...
                If Atomic.casReflectionData(Me, oldReflectionData, New SoftReference(Of )(rd)) Then Return rd
                ' else retry
                oldReflectionData = Me.reflectionData_Renamed
                classRedefinedCount = Me.classRedefinedCount
                rd = oldReflectionData.get()
                If oldReflectionData IsNot Nothing AndAlso rd IsNot Nothing AndAlso rd.redefinedCount = classRedefinedCount Then Return rd
            Loop
        End Function

        ' Generic signature handling
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getGenericSignature0() As String
        End Function

        ' Generic info repository; lazily initialized
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private genericInfo As sun.reflect.generics.repository.ClassRepository

        ' accessor for factory
        Private Property factory As sun.reflect.generics.factory.GenericsFactory
            Get
                ' create scope and factory
                Return sun.reflect.generics.factory.CoreReflectionFactory.make(Me, sun.reflect.generics.scope.ClassScope.make(Me))
            End Get
        End Property

        ' accessor for generic info repository;
        ' generic info is lazily initialized
        Private Property genericInfo As sun.reflect.generics.repository.ClassRepository
            Get
                Dim genericInfo_Renamed As sun.reflect.generics.repository.ClassRepository = Me.genericInfo
                If genericInfo_Renamed Is Nothing Then
                    Dim signature As String = genericSignature0
                    If signature Is Nothing Then
                        genericInfo_Renamed = sun.reflect.generics.repository.ClassRepository.NONE
                    Else
                        genericInfo_Renamed = sun.reflect.generics.repository.ClassRepository.make(signature, factory)
                    End If
                    Me.genericInfo = genericInfo_Renamed
                End If
                Return If(genericInfo_Renamed IsNot sun.reflect.generics.repository.ClassRepository.NONE, genericInfo_Renamed, Nothing)
            End Get
        End Property

        ' Annotations handling
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Friend Function getRawAnnotations() As SByte()
        End Function
        ' Since 1.8
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Friend Function getRawTypeAnnotations() As SByte()
        End Function
        Friend Shared Function getExecutableTypeAnnotationBytes(ByVal ex As Executable) As SByte()
            Return reflectionFactory.getExecutableTypeAnnotationBytes(ex)
        End Function

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Friend Function getConstantPool() As sun.reflect.ConstantPool
        End Function

        '
        '
        ' java.lang.reflect.Field handling
        '
        '

        ' Returns an array of "root" fields. These Field objects must NOT
        ' be propagated to the outside world, but must instead be copied
        ' via ReflectionFactory.copyField.
        Private Function privateGetDeclaredFields(ByVal publicOnly As Boolean) As Field()
            checkInitted()
            Dim res As Field()
            Dim rd As ReflectionData(Of T) = reflectionData()
            If rd IsNot Nothing Then
                res = If(publicOnly, rd.declaredPublicFields, rd.declaredFields)
                If res IsNot Nothing Then Return res
            End If
            ' No cached value available; request value from VM
            res = sun.reflect.Reflection.filterFields(Me, getDeclaredFields0(publicOnly))
            If rd IsNot Nothing Then
                If publicOnly Then
                    rd.declaredPublicFields = res
                Else
                    rd.declaredFields = res
                End If
            End If
            Return res
        End Function

        ' Returns an array of "root" fields. These Field objects must NOT
        ' be propagated to the outside world, but must instead be copied
        ' via ReflectionFactory.copyField.
        Private Function privateGetPublicFields(ByVal traversedInterfaces As java.util.Set(Of [Class])) As Field()
            checkInitted()
            Dim res As Field()
            Dim rd As ReflectionData(Of T) = reflectionData()
            If rd IsNot Nothing Then
                res = rd.publicFields
                If res IsNot Nothing Then Return res
            End If

            ' No cached value available; compute value recursively.
            ' Traverse in correct order for getField().
            Dim fields_Renamed As IList(Of Field) = New List(Of Field)
            If traversedInterfaces Is Nothing Then traversedInterfaces = New HashSet(Of )

            ' Local fields
            Dim tmp As Field() = privateGetDeclaredFields(True)
            addAll(fields_Renamed, tmp)

            ' Direct superinterfaces, recursively
            For Each c As  [Class] In interfaces
                If Not traversedInterfaces.contains(c) Then
                    traversedInterfaces.add(c)
                    addAll(fields_Renamed, c.privateGetPublicFields(traversedInterfaces))
                End If
            Next c

            ' Direct superclass, recursively
            If Not [interface] Then
                Dim c As  [Class] = superclass
                If c IsNot Nothing Then addAll(fields_Renamed, c.privateGetPublicFields(traversedInterfaces))
            End If

            res = New Field(fields_Renamed.Count - 1) {}
            fields_Renamed.ToArray(res)
            If rd IsNot Nothing Then rd.publicFields = res
            Return res
        End Function

        Private Shared Sub addAll(ByVal c As ICollection(Of Field), ByVal o As Field())
            For i As Integer = 0 To o.Length - 1
                c.Add(o(i))
            Next i
        End Sub


        '
        '
        ' java.lang.reflect.Constructor handling
        '
        '

        ' Returns an array of "root" constructors. These Constructor
        ' objects must NOT be propagated to the outside world, but must
        ' instead be copied via ReflectionFactory.copyConstructor.
        Private Function privateGetDeclaredConstructors(ByVal publicOnly As Boolean) As Constructor(Of T)()
            checkInitted()
            Dim res As Constructor(Of T)()
            Dim rd As ReflectionData(Of T) = reflectionData()
            If rd IsNot Nothing Then
                res = If(publicOnly, rd.publicConstructors, rd.declaredConstructors)
                If res IsNot Nothing Then Return res
            End If
            ' No cached value available; request value from VM
            If [interface] Then
                'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                Dim temporaryRes As Constructor(Of T)() = CType(New Constructor(Of ?)() {}, Constructor(Of T)())
                res = temporaryRes
            Else
                res = getDeclaredConstructors0(publicOnly)
            End If
            If rd IsNot Nothing Then
                If publicOnly Then
                    rd.publicConstructors = res
                Else
                    rd.declaredConstructors = res
                End If
            End If
            Return res
        End Function

        '
        '
        ' java.lang.reflect.Method handling
        '
        '

        ' Returns an array of "root" methods. These Method objects must NOT
        ' be propagated to the outside world, but must instead be copied
        ' via ReflectionFactory.copyMethod.
        Private Function privateGetDeclaredMethods(ByVal publicOnly As Boolean) As Method()
            checkInitted()
            Dim res As Method()
            Dim rd As ReflectionData(Of T) = reflectionData()
            If rd IsNot Nothing Then
                res = If(publicOnly, rd.declaredPublicMethods, rd.declaredMethods)
                If res IsNot Nothing Then Return res
            End If
            ' No cached value available; request value from VM
            res = sun.reflect.Reflection.filterMethods(Me, getDeclaredMethods0(publicOnly))
            If rd IsNot Nothing Then
                If publicOnly Then
                    rd.declaredPublicMethods = res
                Else
                    rd.declaredMethods = res
                End If
            End If
            Return res
        End Function

        Friend Class MethodArray
            ' Don't add or remove methods except by add() or remove() calls.
            Private methods As Method()
            Private length_Renamed As Integer
            Private defaults As Integer

            Friend Sub New()
                Me.New(20)
            End Sub

            Friend Sub New(ByVal initialSize As Integer)
                If initialSize < 2 Then Throw New IllegalArgumentException("Size should be 2 or more")

                methods = New Method(initialSize - 1) {}
                length_Renamed = 0
                defaults = 0
            End Sub

            Friend Overridable Function hasDefaults() As Boolean
                Return defaults <> 0
            End Function

            Friend Overridable Sub add(ByVal m As Method)
                If length_Renamed = methods.Length Then methods = java.util.Arrays.copyOf(methods, 2 * methods.Length)
                methods(length_Renamed) = m
                length_Renamed += 1

                If m IsNot Nothing AndAlso m.default Then defaults += 1
            End Sub

            Friend Overridable Sub addAll(ByVal ma As Method())
                For i As Integer = 0 To ma.Length - 1
                    add(ma(i))
                Next i
            End Sub

            Friend Overridable Sub addAll(ByVal ma As MethodArray)
                For i As Integer = 0 To ma.length() - 1
                    add(ma.get(i))
                Next i
            End Sub

            Friend Overridable Sub addIfNotPresent(ByVal newMethod As Method)
                For i As Integer = 0 To length_Renamed - 1
                    Dim m As Method = methods(i)
                    If m Is newMethod OrElse (m IsNot Nothing AndAlso m.Equals(newMethod)) Then Return
                Next i
                add(newMethod)
            End Sub

            Friend Overridable Sub addAllIfNotPresent(ByVal newMethods As MethodArray)
                For i As Integer = 0 To newMethods.length() - 1
                    Dim m As Method = newMethods.get(i)
                    If m IsNot Nothing Then addIfNotPresent(m)
                Next i
            End Sub

            '         Add Methods declared in an interface to this MethodArray.
            '         * Static methods declared in interfaces are not inherited.
            '         
            Friend Overridable Sub addInterfaceMethods(ByVal methods As Method())
                For Each candidate As Method In methods
                    If Not Modifier.isStatic(candidate.modifiers) Then add(candidate)
                Next candidate
            End Sub

            Friend Overridable Function length() As Integer
                Return length_Renamed
            End Function

            Friend Overridable Function [get](ByVal i As Integer) As Method
                Return methods(i)
            End Function

            Friend Overridable Property first As Method
                Get
                    For Each m As Method In methods
                        If m IsNot Nothing Then Return m
                    Next m
                    Return Nothing
                End Get
            End Property

            Friend Overridable Sub removeByNameAndDescriptor(ByVal toRemove As Method)
                For i As Integer = 0 To length_Renamed - 1
                    Dim m As Method = methods(i)
                    If m IsNot Nothing AndAlso matchesNameAndDescriptor(m, toRemove) Then remove(i)
                Next i
            End Sub

            Private Sub remove(ByVal i As Integer)
                If methods(i) IsNot Nothing AndAlso methods(i).default Then defaults -= 1
                methods(i) = Nothing
            End Sub

            Private Function matchesNameAndDescriptor(ByVal m1 As Method, ByVal m2 As Method) As Boolean
                Return m1.returnType = m2.returnType AndAlso m1.name = m2.name AndAlso arrayContentsEq(m1.parameterTypes, m2.parameterTypes) ' name is guaranteed to be interned
            End Function

            Friend Overridable Sub compactAndTrim()
                Dim newPos As Integer = 0
                ' Get rid of null slots
                For pos As Integer = 0 To length_Renamed - 1
                    Dim m As Method = methods(pos)
                    If m IsNot Nothing Then
                        If pos <> newPos Then methods(newPos) = m
                        newPos += 1
                    End If
                Next pos
                If newPos <> methods.Length Then methods = java.util.Arrays.copyOf(methods, newPos)
            End Sub

            '         Removes all Methods from this MethodArray that have a more specific
            '         * default Method in this MethodArray.
            '         *
            '         * Users of MethodArray are responsible for pruning Methods that have
            '         * a more specific <em>concrete</em> Method.
            '         
            Friend Overridable Sub removeLessSpecifics()
                If Not hasDefaults() Then Return

                For i As Integer = 0 To length_Renamed - 1
                    Dim m As Method = [get](i)
                    If m Is Nothing OrElse (Not m.default) Then Continue For

                    For j As Integer = 0 To length_Renamed - 1
                        If i = j Then Continue For

                        Dim candidate As Method = [get](j)
                        If candidate Is Nothing Then Continue For

                        If Not matchesNameAndDescriptor(m, candidate) Then Continue For

                        If hasMoreSpecificClass(m, candidate) Then remove(j)
                    Next j
                Next i
            End Sub

            Friend Overridable Property array As Method()
                Get
                    Return methods
                End Get
            End Property

            ' Returns true if m1 is more specific than m2
            Friend Shared Function hasMoreSpecificClass(ByVal m1 As Method, ByVal m2 As Method) As Boolean
                Dim m1Class As  [Class] = m1.declaringClass
                Dim m2Class As  [Class] = m2.declaringClass
                Return m1Class IsNot m2Class AndAlso m1Class.IsSubclassOf(m2Class)
            End Function
        End Class


        ' Returns an array of "root" methods. These Method objects must NOT
        ' be propagated to the outside world, but must instead be copied
        ' via ReflectionFactory.copyMethod.
        Private Function privateGetPublicMethods() As Method()
            checkInitted()
            Dim res As Method()
            Dim rd As ReflectionData(Of T) = reflectionData()
            If rd IsNot Nothing Then
                res = rd.publicMethods
                If res IsNot Nothing Then Return res
            End If

            ' No cached value available; compute value recursively.
            ' Start by fetching public declared methods
            Dim methods_Renamed As New MethodArray
            Dim tmp As Method() = privateGetDeclaredMethods(True)
            methods_Renamed.addAll(tmp)
            ' Now recur over superclass and direct superinterfaces.
            ' Go over superinterfaces first so we can more easily filter
            ' out concrete implementations inherited from superclasses at
            ' the end.
            Dim inheritedMethods As New MethodArray
            For Each i As  [Class] In interfaces
                inheritedMethods.addInterfaceMethods(i.privateGetPublicMethods())
            Next i
            If Not [interface] Then
                Dim c As  [Class] = superclass
                If c IsNot Nothing Then
                    Dim supers As New MethodArray
                    supers.addAll(c.privateGetPublicMethods())
                    ' Filter out concrete implementations of any
                    ' interface methods
                    For i As Integer = 0 To supers.length() - 1
                        Dim m As Method = supers.get(i)
                        If m IsNot Nothing AndAlso (Not Modifier.isAbstract(m.modifiers)) AndAlso (Not m.default) Then inheritedMethods.removeByNameAndDescriptor(m)
                    Next i
                    ' Insert superclass's inherited methods before
                    ' superinterfaces' to satisfy getMethod's search
                    ' order
                    supers.addAll(inheritedMethods)
                    inheritedMethods = supers
                End If
            End If
            ' Filter out all local methods from inherited ones
            For i As Integer = 0 To methods_Renamed.length() - 1
                Dim m As Method = methods_Renamed.get(i)
                inheritedMethods.removeByNameAndDescriptor(m)
            Next i
            methods_Renamed.addAllIfNotPresent(inheritedMethods)
            methods_Renamed.removeLessSpecifics()
            methods_Renamed.compactAndTrim()
            res = methods_Renamed.array
            If rd IsNot Nothing Then rd.publicMethods = res
            Return res
        End Function


        '
        ' Helpers for fetchers of one field, method, or constructor
        '

        Private Shared Function searchFields(ByVal fields As Field(), ByVal name As String) As Field
            Dim internedName As String = name.Intern()
            For i As Integer = 0 To fields.Length - 1
                If fields(i).name = internedName Then Return reflectionFactory.copyField(fields(i))
            Next i
            Return Nothing
        End Function

        Private Function getField0(ByVal name As String) As Field
            ' Note: the intent is that the search algorithm this routine
            ' uses be equivalent to the ordering imposed by
            ' privateGetPublicFields(). It fetches only the declared
            ' public fields for each [Class], however, to reduce the number
            ' of Field objects which have to be created for the common
            ' case where the field being requested is declared in the
            ' class which is being queried.
            Dim res As Field
            ' Search declared public fields
            res = searchFields(privateGetDeclaredFields(True), name)
            If res IsNot Nothing Then Return res
            ' Direct superinterfaces, recursively
            Dim interfaces_Renamed As  [Class]() = interfaces
            For i As Integer = 0 To interfaces_Renamed.Length - 1
                Dim c As  [Class] = interfaces_Renamed(i)
                res = c.getField0(name)
                If res IsNot Nothing Then Return res
            Next i
            ' Direct superclass, recursively
            If Not [interface] Then
                Dim c As  [Class] = superclass
                If c IsNot Nothing Then
                    res = c.getField0(name)
                    If res IsNot Nothing Then Return res
                End If
            End If
            Return Nothing
        End Function

        Private Shared Function searchMethods(ByVal methods As Method(), ByVal name As String, ByVal parameterTypes As  [Class]()) As Method
			Dim res As Method = Nothing
            Dim internedName As String = name.Intern()
            For i As Integer = 0 To methods.Length - 1
                Dim m As Method = methods(i)
                If m.name = internedName AndAlso arrayContentsEq(parameterTypes, m.parameterTypes) AndAlso (res Is Nothing OrElse m.returnType.IsSubclassOf(res.returnType)) Then res = m
            Next i

            Return (If(res Is Nothing, res, reflectionFactory.copyMethod(res)))
        End Function

        Private Function getMethod0(ByVal name As String, ByVal parameterTypes As  [Class](), ByVal includeStaticMethods As Boolean) As Method
			Dim interfaceCandidates As New MethodArray(2)
            Dim res As Method = privateGetMethodRecursive(name, parameterTypes, includeStaticMethods, interfaceCandidates)
            If res IsNot Nothing Then Return res

            ' Not found on class or superclass directly
            interfaceCandidates.removeLessSpecifics()
            Return interfaceCandidates.first ' may be null
        End Function

        Private Function privateGetMethodRecursive(ByVal name As String, ByVal parameterTypes As  [Class](), ByVal includeStaticMethods As Boolean, ByVal allInterfaceCandidates As MethodArray) As Method
			' Note: the intent is that the search algorithm this routine
			' uses be equivalent to the ordering imposed by
			' privateGetPublicMethods(). It fetches only the declared
			' public methods for each [Class], however, to reduce the
			' number of Method objects which have to be created for the
			' common case where the method being requested is declared in
			' the class which is being queried.
			'
			' Due to default methods, unless a method is found on a superclass,
			' methods declared in any superinterface needs to be considered.
			' Collect all candidates declared in superinterfaces in {@code
			' allInterfaceCandidates} and select the most specific if no match on
			' a superclass is found.

			' Must _not_ return root methods
			Dim res As Method
            ' Search declared public methods
            res = searchMethods(privateGetDeclaredMethods(True), name, parameterTypes)
            If res IsNot Nothing Then
                If includeStaticMethods OrElse (Not Modifier.isStatic(res.modifiers)) Then Return res
            End If
            ' Search superclass's methods
            If Not [interface] Then
                Dim c As  [Class] = superclass
                If c IsNot Nothing Then
                    res = c.getMethod0(name, parameterTypes, True)
                    If res IsNot Nothing Then Return res
                End If
            End If
            ' Search superinterfaces' methods
            Dim interfaces_Renamed As  [Class]() = interfaces
            For Each c As  [Class] In interfaces_Renamed
                res = c.getMethod0(name, parameterTypes, False)
                If res IsNot Nothing Then allInterfaceCandidates.add(res)
            Next c
            ' Not found
            Return Nothing
        End Function

        Private Function getConstructor0(ByVal parameterTypes As  [Class](), ByVal which As Integer) As Constructor(Of T)
			Dim constructors_Renamed As Constructor(Of T)() = privateGetDeclaredConstructors((which = Member.PUBLIC))
            For Each constructor_Renamed As Constructor(Of T) In constructors_Renamed
                If arrayContentsEq(parameterTypes, constructor_Renamed.parameterTypes) Then Return reflectionFactory.copyConstructor(constructor_Renamed)
            Next constructor_Renamed
            Throw New NoSuchMethodException(name & ".<init>" & argumentTypesToString(parameterTypes))
        End Function

        '
        ' Other helpers and base implementation
        '

        Private Shared Function arrayContentsEq(ByVal a1 As Object(), ByVal a2 As Object()) As Boolean
            If a1 Is Nothing Then Return a2 Is Nothing OrElse a2.Length = 0

            If a2 Is Nothing Then Return a1.Length = 0

            If a1.Length <> a2.Length Then Return False

            For i As Integer = 0 To a1.Length - 1
                If a1(i) IsNot a2(i) Then Return False
            Next i

            Return True
        End Function

        Private Shared Function copyFields(ByVal arg As Field()) As Field()
            Dim out As Field() = New Field(arg.Length - 1) {}
            Dim fact As sun.reflect.ReflectionFactory = reflectionFactory
            For i As Integer = 0 To arg.Length - 1
                out(i) = fact.copyField(arg(i))
            Next i
            Return out
        End Function

        Private Shared Function copyMethods(ByVal arg As Method()) As Method()
            Dim out As Method() = New Method(arg.Length - 1) {}
            Dim fact As sun.reflect.ReflectionFactory = reflectionFactory
            For i As Integer = 0 To arg.Length - 1
                out(i) = fact.copyMethod(arg(i))
            Next i
            Return out
        End Function

        Private Shared Function copyConstructors(Of U)(ByVal arg As Constructor(Of U)()) As Constructor(Of U)()
            Dim out As Constructor(Of U)() = arg.Clone()
            Dim fact As sun.reflect.ReflectionFactory = reflectionFactory
            For i As Integer = 0 To out.Length - 1
                out(i) = fact.copyConstructor(out(i))
            Next i
            Return out
        End Function

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getDeclaredFields0(ByVal publicOnly As Boolean) As Field()
        End Function
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getDeclaredMethods0(ByVal publicOnly As Boolean) As Method()
        End Function
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getDeclaredConstructors0(ByVal publicOnly As Boolean) As Constructor(Of T)()
        End Function
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Function getDeclaredClasses0() As  [Class]()
		End Function

        Private Shared Function argumentTypesToString(ByVal argTypes As  [Class]()) As String
			Dim buf As New StringBuilder
            buf.append("(")
            If argTypes IsNot Nothing Then
                For i As Integer = 0 To argTypes.Length - 1
                    If i > 0 Then buf.append(", ")
                    Dim c As  [Class] = argTypes(i)
                    buf.append(If(c Is Nothing, "null", c.name))
                Next i
            End If
            buf.append(")")
            Return buf.ToString()
        End Function

        ''' <summary>
        ''' use serialVersionUID from JDK 1.1 for interoperability </summary>
        Private Const serialVersionUID As Long = 3206093459760846163L


        ''' <summary>
        ''' Class Class is special cased within the Serialization Stream Protocol.
        ''' 
        ''' A Class instance is written initially into an ObjectOutputStream in the
        ''' following format:
        ''' <pre>
        '''      {@code TC_CLASS} ClassDescriptor
        '''      A ClassDescriptor is a special cased serialization of
        '''      a {@code java.io.ObjectStreamClass} instance.
        ''' </pre>
        ''' A new handle is generated for the initial time the class descriptor
        ''' is written into the stream. Future references to the class descriptor
        ''' are written as references to the initial class descriptor instance.
        ''' </summary>
        ''' <seealso cref= java.io.ObjectStreamClass </seealso>
        Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = New java.io.ObjectStreamField() {}


        ''' <summary>
        ''' Returns the assertion status that would be assigned to this
        ''' class if it were to be initialized at the time this method is invoked.
        ''' If this class has had its assertion status set, the most recent
        ''' setting will be returned; otherwise, if any package default assertion
        ''' status pertains to this [Class], the most recent setting for the most
        ''' specific pertinent package default assertion status is returned;
        ''' otherwise, if this class is not a system class (i.e., it has a
        ''' class loader) its class loader's default assertion status is returned;
        ''' otherwise, the system class default assertion status is returned.
        ''' <p>
        ''' Few programmers will have any need for this method; it is provided
        ''' for the benefit of the JRE itself.  (It allows a class to determine at
        ''' the time that it is initialized whether assertions should be enabled.)
        ''' Note that this method is not guaranteed to return the actual
        ''' assertion status that was (or will be) associated with the specified
        ''' class when it was (or will be) initialized.
        ''' </summary>
        ''' <returns> the desired assertion status of the specified class. </returns>
        ''' <seealso cref=    java.lang.ClassLoader#setClassAssertionStatus </seealso>
        ''' <seealso cref=    java.lang.ClassLoader#setPackageAssertionStatus </seealso>
        ''' <seealso cref=    java.lang.ClassLoader#setDefaultAssertionStatus
        ''' @since  1.4 </seealso>
        Public Function desiredAssertionStatus() As Boolean
            Dim loader As  [Class]Loader = classLoader
            ' If the loader is null this is a system [Class], so ask the VM
            If loader Is Nothing Then Return desiredAssertionStatus0(Me)

            ' If the classloader has been initialized with the assertion
            ' directives, ask it. Otherwise, ask the VM.
            SyncLock loader.assertionLock
                If loader.classAssertionStatus IsNot Nothing Then Return loader.desiredAssertionStatus(name)
            End SyncLock
            Return desiredAssertionStatus0(Me)
        End Function

        ' Retrieves the desired assertion status of this class from the VM
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Function desiredAssertionStatus0(ByVal clazz As [Class]) As Boolean
        End Function

        ''' <summary>
        ''' Returns true if and only if this class was declared as an enum in the
        ''' source code.
        ''' </summary>
        ''' <returns> true if and only if this class was declared as an enum in the
        '''     source code
        ''' @since 1.5 </returns>
        Public Property [enum] As Boolean
            Get
                ' An enum must both directly extend java.lang.Enum and have
                ' the ENUM bit set; classes for specialized enum constants
                ' don't do the former.
                Return (Me.modifiers And [enum]) <> 0 AndAlso Me.BaseType = GetType(java.lang.Enum)
            End Get
        End Property

        ' Fetches the factory for reflective objects
        Private Property Shared reflectionFactory As sun.reflect.ReflectionFactory
            Get
                If reflectionFactory Is Nothing Then reflectionFactory = java.security.AccessController.doPrivileged(New sun.reflect.ReflectionFactory.GetReflectionFactoryAction)
                Return reflectionFactory
            End Get
        End Property
        Private Shared reflectionFactory As sun.reflect.ReflectionFactory

        ' To be able to query system properties as soon as they're available
        Private Shared initted As Boolean = False
        Private Shared Sub checkInitted()
            If initted Then Return
            java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                ' Tests to ensure the system properties table is fully
                ' initialized. This is needed because reflection code is
                ' called very early in the initialization process (before
                ' command-line arguments have been parsed and therefore
                ' these user-settable properties installed.) We assume that
                ' if System.out is non-null then the System class has been
                ' fully initialized and that the bulk of the startup code
                ' has been run.

                If System.out Is Nothing Then Return Nothing

                ' Doesn't use  java.lang.[Boolean].getBoolean to avoid class init.
                Dim val As String = System.getProperty("sun.reflect.noCaches")
                If val IsNot Nothing AndAlso val.Equals("true") Then useCaches = False

                initted = True
                Return Nothing
            End Function
        End Class

        ''' <summary>
        ''' Returns the elements of this enum class or null if this
        ''' Class object does not represent an enum type.
        ''' </summary>
        ''' <returns> an array containing the values comprising the enum class
        '''     represented by this Class object in the order they're
        '''     declared, or null if this Class object does not
        '''     represent an enum type
        ''' @since 1.5 </returns>
        Public Property enumConstants As T()
            Get
                Dim values As T() = enumConstantsShared
                Return If(values IsNot Nothing, values.Clone(), Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Returns the elements of this enum class or null if this
        ''' Class object does not represent an enum type;
        ''' identical to getEnumConstants except that the result is
        ''' uncloned, cached, and shared by all callers.
        ''' </summary>
        Friend Property enumConstantsShared As T()
            Get
                If enumConstants Is Nothing Then
                    If Not [ENUM] Then Return Nothing
                    Try
                        Dim values As Method = getMethod("values")
                        java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
    'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                        Dim temporaryConstants As T() = CType(values.invoke(Nothing), T())
                        enumConstants = temporaryConstants
                        ' These can happen when users concoct enum-like classes
                        ' that don't comply with the enum spec.
                        'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
                    Catch InvocationTargetException Or NoSuchMethodException Or IllegalAccessException ex
						Return Nothing
                    End Try
                End If
                Return enumConstants
            End Get
        End Property

        Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                values.accessible = True
                Return Nothing
            End Function
        End Class
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private enumConstants As T() = Nothing

        ''' <summary>
        ''' Returns a map from simple name to enum constant.  This package-private
        ''' method is used internally by Enum to implement
        ''' {@code public static <T extends Enum<T>> T valueOf(Class<T>, String)}
        ''' efficiently.  Note that the map is returned by this method is
        ''' created lazily on first use.  Typically it won't ever get created.
        ''' </summary>
        Friend Function enumConstantDirectory() As IDictionary(Of String, T)
            If enumConstantDirectory_Renamed Is Nothing Then
                Dim universe As T() = enumConstantsShared
                If universe Is Nothing Then Throw New IllegalArgumentException(name & " is not an enum type")
                Dim m As IDictionary(Of String, T) = New Dictionary(Of String, T)(2 * universe.Length)
                For Each constant As T In universe
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    m(CType(constant, Enum(Of ?)).name()) = constant
                Next constant
                enumConstantDirectory_Renamed = m
            End If
            Return enumConstantDirectory_Renamed
        End Function
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private enumConstantDirectory_Renamed As IDictionary(Of String, T) = Nothing

        ''' <summary>
        ''' Casts an object to the class or interface represented
        ''' by this {@code Class} object.
        ''' </summary>
        ''' <param name="obj"> the object to be cast </param>
        ''' <returns> the object after casting, or null if obj is null
        ''' </returns>
        ''' <exception cref="ClassCastException"> if the object is not
        ''' null and is not assignable to the type T.
        ''' 
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function cast(ByVal obj As Object) As T
            If obj IsNot Nothing AndAlso (Not isInstance(obj)) Then Throw New ClassCastException(cannotCastMsg(obj))
            Return CType(obj, T)
        End Function

        Private Function cannotCastMsg(ByVal obj As Object) As String
            Return "Cannot cast " & obj.GetType().Name & " to " & name
        End Function

        ''' <summary>
        ''' Casts this {@code Class} object to represent a subclass of the class
        ''' represented by the specified class object.  Checks that the cast
        ''' is valid, and throws a {@code ClassCastException} if it is not.  If
        ''' this method succeeds, it always returns a reference to this class object.
        ''' 
        ''' <p>This method is useful when a client needs to "narrow" the type of
        ''' a {@code Class} object to pass it to an API that restricts the
        ''' {@code Class} objects that it is willing to accept.  A cast would
        ''' generate a compile-time warning, as the correctness of the cast
        ''' could not be checked at runtime (because generic types are implemented
        ''' by erasure).
        ''' </summary>
        ''' @param <U> the type to cast this class object to </param>
        ''' <param name="clazz"> the class of the type to cast this class object to </param>
        ''' <returns> this {@code Class} object, cast to represent a subclass of
        '''    the specified class object. </returns>
        ''' <exception cref="ClassCastException"> if this {@code Class} object does not
        '''    represent a subclass of the specified class (here "subclass" includes
        '''    the class itself).
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function asSubclass(Of U)(ByVal clazz As [Class]) As  [Class]
			If Me.IsSubclassOf(clazz) Then
                Return CType(Me, [Class])
            Else
                Throw New ClassCastException(Me.ToString())
            End If
        End Function

        ''' <exception cref="NullPointerException"> {@inheritDoc}
        ''' @since 1.5 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Function getAnnotation(Of A As annotation)(ByVal annotationClass As [Class]) As A
            java.util.Objects.requireNonNull(annotationClass)

            Return CType(annotationData().annotations(annotationClass), A)
        End Function

        ''' <summary>
        ''' {@inheritDoc} </summary>
        ''' <exception cref="NullPointerException"> {@inheritDoc}
        ''' @since 1.5 </exception>
        Public Overrides Function isAnnotationPresent(ByVal annotationClass As [Class]) As Boolean Implements AnnotatedElement.isAnnotationPresent
            Return outerInstance.isAnnotationPresent(annotationClass)
        End Function

        ''' <exception cref="NullPointerException"> {@inheritDoc}
        ''' @since 1.8 </exception>
        Public Overrides Function getAnnotationsByType(Of A As annotation)(ByVal annotationClass As [Class]) As A()
            java.util.Objects.requireNonNull(annotationClass)

            Dim annotationData As AnnotationData = annotationData()
            Return AnnotationSupport.getAssociatedAnnotations(annotationData.declaredAnnotations, Me, annotationClass)
        End Function

        ''' <summary>
        ''' @since 1.5
        ''' </summary>
        Public Property annotations As annotation() Implements AnnotatedElement.getAnnotations
            Get
                Return AnnotationParser.ToArray(annotationData().annotations)
            End Get
        End Property

        ''' <exception cref="NullPointerException"> {@inheritDoc}
        ''' @since 1.8 </exception>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overrides Function getDeclaredAnnotation(Of A As annotation)(ByVal annotationClass As [Class]) As A
            java.util.Objects.requireNonNull(annotationClass)

            Return CType(annotationData().declaredAnnotations(annotationClass), A)
        End Function

        ''' <exception cref="NullPointerException"> {@inheritDoc}
        ''' @since 1.8 </exception>
        Public Overrides Function getDeclaredAnnotationsByType(Of A As annotation)(ByVal annotationClass As [Class]) As A()
            java.util.Objects.requireNonNull(annotationClass)

            Return AnnotationSupport.getDirectlyAndIndirectlyPresent(annotationData().declaredAnnotations, annotationClass)
        End Function

        ''' <summary>
        ''' @since 1.5
        ''' </summary>
        Public Property declaredAnnotations As annotation() Implements AnnotatedElement.getDeclaredAnnotations
            Get
                Return AnnotationParser.ToArray(annotationData().declaredAnnotations)
            End Get
        End Property

        ' annotation data that might get invalidated when JVM TI RedefineClasses() is called
        Private Class AnnotationData
            Friend ReadOnly annotations As IDictionary(Of [Class], annotation)
            Friend ReadOnly declaredAnnotations As IDictionary(Of [Class], annotation)

            ' Value of classRedefinedCount when we created this AnnotationData instance
            Friend ReadOnly redefinedCount As Integer

            Friend Sub New(ByVal annotations As IDictionary(Of [Class], annotation), ByVal declaredAnnotations As IDictionary(Of [Class], annotation), ByVal redefinedCount As Integer)
                Me.annotations = annotations
                Me.declaredAnnotations = declaredAnnotations
                Me.redefinedCount = redefinedCount
            End Sub
        End Class

        ' Annotations cache
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private annotationData_Renamed As AnnotationData

        Private Function annotationData() As AnnotationData
            Do ' retry loop
                Dim annotationData_Renamed As AnnotationData = Me.annotationData_Renamed
                Dim classRedefinedCount As Integer = Me.classRedefinedCount
                If annotationData_Renamed IsNot Nothing AndAlso annotationData_Renamed.redefinedCount = classRedefinedCount Then Return annotationData_Renamed
                ' null or stale annotationData -> optimistically create new instance
                Dim newAnnotationData As AnnotationData = createAnnotationData(classRedefinedCount)
                ' try to install it
                If Atomic.casAnnotationData(Me, annotationData_Renamed, newAnnotationData) Then Return newAnnotationData
            Loop
        End Function

        Private Function createAnnotationData(ByVal classRedefinedCount As Integer) As AnnotationData
            Dim declaredAnnotations_Renamed As IDictionary(Of [Class], annotation) = AnnotationParser.parseAnnotations(rawAnnotations, constantPool, Me)
            Dim superClass As  [Class] = superClass
            Dim annotations_Renamed As IDictionary(Of [Class], annotation) = Nothing
            If superClass IsNot Nothing Then
                Dim superAnnotations As IDictionary(Of [Class], annotation) = superClass.annotationData().annotations
                For Each e As KeyValuePair(Of [Class], annotation) In superAnnotations
                    Dim annotationClass As  [Class] = e.Key
                    If annotationType.getInstance(annotationClass).inherited Then
                        If annotations_Renamed Is Nothing Then ' lazy construction annotations_Renamed = New java.util.LinkedHashMap(Of )( (System.Math.Max(declaredAnnotations_Renamed.Count, System.Math.Min(12, declaredAnnotations_Renamed.Count + superAnnotations.Count)) * 4 + 2) / 3)
                            annotations_Renamed(annotationClass) = e.Value
                        End If
                Next e
            End If
            If annotations_Renamed Is Nothing Then
                ' no inherited annotations -> share the Map with declaredAnnotations
                annotations_Renamed = declaredAnnotations_Renamed
            Else
                ' at least one inherited annotation -> declared may override inherited
                'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
                annotations_Renamed.putAll(declaredAnnotations_Renamed)
            End If
            Return New AnnotationData(annotations_Renamed, declaredAnnotations_Renamed, classRedefinedCount)
        End Function

        ' Annotation types cache their internal (AnnotationType) form

        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        <NonSerialized>
        Private annotationType As AnnotationType

        Friend Function casAnnotationType(ByVal oldType As AnnotationType, ByVal newType As AnnotationType) As Boolean
            Return Atomic.casAnnotationType(Me, oldType, newType)
        End Function

        Friend Property annotationType As AnnotationType
            Get
                Return annotationType
            End Get
        End Property

        Friend Property declaredAnnotationMap As IDictionary(Of [Class], annotation)
            Get
                Return annotationData().declaredAnnotations
            End Get
        End Property

        '     Backing store of user-defined values pertaining to this class.
        '     * Maintained by the ClassValue class.
        '     
        <NonSerialized>
        Friend classValueMap As  [Class]Value.ClassValueMap

        ''' <summary>
        ''' Returns an {@code AnnotatedType} object that represents the use of a
        ''' type to specify the superclass of the entity represented by this {@code
        ''' Class} object. (The <em>use</em> of type Foo to specify the superclass
        ''' in '...  extends Foo' is distinct from the <em>declaration</em> of type
        ''' Foo.)
        ''' 
        ''' <p> If this {@code Class} object represents a type whose declaration
        ''' does not explicitly indicate an annotated superclass, then the return
        ''' value is an {@code AnnotatedType} object representing an element with no
        ''' annotations.
        ''' 
        ''' <p> If this {@code Class} represents either the {@code Object} [Class], an
        ''' interface type, an array type, a primitive type, or void, the return
        ''' value is {@code null}.
        ''' </summary>
        ''' <returns> an object representing the superclass
        ''' @since 1.8 </returns>
        Public Property annotatedSuperclass As AnnotatedType
            Get
                If Me Is GetType(Object) OrElse [interface] OrElse Array OrElse primitive OrElse Me Is Void.TYPE Then Return Nothing

                Return TypeAnnotationParser.buildAnnotatedSuperclass(rawTypeAnnotations, constantPool, Me)
            End Get
        End Property

        ''' <summary>
        ''' Returns an array of {@code AnnotatedType} objects that represent the use
        ''' of types to specify superinterfaces of the entity represented by this
        ''' {@code Class} object. (The <em>use</em> of type Foo to specify a
        ''' superinterface in '... implements Foo' is distinct from the
        ''' <em>declaration</em> of type Foo.)
        ''' 
        ''' <p> If this {@code Class} object represents a [Class], the return value is
        ''' an array containing objects representing the uses of interface types to
        ''' specify interfaces implemented by the class. The order of the objects in
        ''' the array corresponds to the order of the interface types used in the
        ''' 'implements' clause of the declaration of this {@code Class} object.
        ''' 
        ''' <p> If this {@code Class} object represents an interface, the return
        ''' value is an array containing objects representing the uses of interface
        ''' types to specify interfaces directly extended by the interface. The
        ''' order of the objects in the array corresponds to the order of the
        ''' interface types used in the 'extends' clause of the declaration of this
        ''' {@code Class} object.
        ''' 
        ''' <p> If this {@code Class} object represents a class or interface whose
        ''' declaration does not explicitly indicate any annotated superinterfaces,
        ''' the return value is an array of length 0.
        ''' 
        ''' <p> If this {@code Class} object represents either the {@code Object}
        ''' [Class], an array type, a primitive type, or void, the return value is an
        ''' array of length 0.
        ''' </summary>
        ''' <returns> an array representing the superinterfaces
        ''' @since 1.8 </returns>
        Public Property annotatedInterfaces As AnnotatedType()
            Get
                Return TypeAnnotationParser.buildAnnotatedInterfaces(rawTypeAnnotations, constantPool, Me)
            End Get
        End Property
    End Class

End Namespace
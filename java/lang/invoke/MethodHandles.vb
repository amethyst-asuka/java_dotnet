Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports java.lang.invoke

'
' * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.invoke




    ''' <summary>
    ''' This class consists exclusively of static methods that operate on or return
    ''' method handles. They fall into several categories:
    ''' <ul>
    ''' <li>Lookup methods which help create method handles for methods and fields.
    ''' <li>Combinator methods, which combine or transform pre-existing method handles into new ones.
    ''' <li>Other factory methods to create method handles that emulate other common JVM operations or control flow patterns.
    ''' </ul>
    ''' <p>
    ''' @author John Rose, JSR 292 EG
    ''' @since 1.7
    ''' </summary>
    Public Class MethodHandles

        Private Sub New() ' do not instantiate
        End Sub

        Private Shared ReadOnly IMPL_NAMES As MemberName.Factory = MemberName.Factory
        Shared Sub New()
            MethodHandleImpl.initStatics()
            IMPL_NAMES.GetType()
        End Sub
        ' See IMPL_LOOKUP below.

        '// Method handle creation from ordinary methods.

        ''' <summary>
        ''' Returns a <seealso cref="Lookup lookup object"/> with
        ''' full capabilities to emulate all supported bytecode behaviors of the caller.
        ''' These capabilities include <a href="MethodHandles.Lookup.html#privacc">private access</a> to the caller.
        ''' Factory methods on the lookup object can create
        ''' <a href="MethodHandleInfo.html#directmh">direct method handles</a>
        ''' for any member that the caller has access to via bytecodes,
        ''' including protected and private fields and methods.
        ''' This lookup object is a <em>capability</em> which may be delegated to trusted agents.
        ''' Do not store it in place where untrusted code can access it.
        ''' <p>
        ''' This method is caller sensitive, which means that it may return different
        ''' values to different callers.
        ''' <p>
        ''' For any given caller class {@code C}, the lookup object returned by this call
        ''' has equivalent capabilities to any lookup object
        ''' supplied by the JVM to the bootstrap method of an
        ''' <a href="package-summary.html#indyinsn">invokedynamic instruction</a>
        ''' executing in the same caller class {@code C}. </summary>
        ''' <returns> a lookup object for the caller of this method, with private access </returns>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared Function lookup() As Lookup
            Return New Lookup(sun.reflect.Reflection.callerClass)
        End Function

        ''' <summary>
        ''' Returns a <seealso cref="Lookup lookup object"/> which is trusted minimally.
        ''' It can only be used to create method handles to
        ''' publicly accessible fields and methods.
        ''' <p>
        ''' As a matter of pure convention, the <seealso cref="Lookup#lookupClass lookup class"/>
        ''' of this lookup object will be <seealso cref="java.lang.Object"/>.
        ''' 
        ''' <p style="font-size:smaller;">
        ''' <em>Discussion:</em>
        ''' The lookup class can be changed to any other class {@code C} using an expression of the form
        ''' <seealso cref="Lookup#in publicLookup().in(C.class)"/>.
        ''' Since all classes have equal access to public names,
        ''' such a change would confer no new access rights.
        ''' A public lookup object is always subject to
        ''' <a href="MethodHandles.Lookup.html#secmgr">security manager checks</a>.
        ''' Also, it cannot access
        ''' <a href="MethodHandles.Lookup.html#callsens">caller sensitive methods</a>. </summary>
        ''' <returns> a lookup object which is trusted minimally </returns>
        Public Shared Function publicLookup() As Lookup
            Return lookup.PUBLIC_LOOKUP
        End Function

        ''' <summary>
        ''' Performs an unchecked "crack" of a
        ''' <a href="MethodHandleInfo.html#directmh">direct method handle</a>.
        ''' The result is as if the user had obtained a lookup object capable enough
        ''' to crack the target method handle, called
        ''' <seealso cref="java.lang.invoke.MethodHandles.Lookup#revealDirect Lookup.revealDirect"/>
        ''' on the target to obtain its symbolic reference, and then called
        ''' <seealso cref="java.lang.invoke.MethodHandleInfo#reflectAs MethodHandleInfo.reflectAs"/>
        ''' to resolve the symbolic reference to a member.
        ''' <p>
        ''' If there is a security manager, its {@code checkPermission} method
        ''' is called with a {@code ReflectPermission("suppressAccessChecks")} permission. </summary>
        ''' @param <T> the desired type of the result, either <seealso cref="Member"/> or a subtype </param>
        ''' <param name="target"> a direct method handle to crack into symbolic reference components </param>
        ''' <param name="expected"> a class object representing the desired result type {@code T} </param>
        ''' <returns> a reference to the method, constructor, or field object </returns>
        ''' <exception cref="SecurityException"> if the caller is not privileged to call {@code setAccessible} </exception>
        ''' <exception cref="NullPointerException"> if either argument is {@code null} </exception>
        ''' <exception cref="IllegalArgumentException"> if the target is not a direct method handle </exception>
        ''' <exception cref="ClassCastException"> if the member is not of the expected type
        ''' @since 1.8 </exception>
        Public Shared Function reflectAs(Of T As Member)(ByVal expected As [Class], ByVal target As MethodHandle) As T
            Dim smgr As SecurityManager = System.securityManager
            If smgr IsNot Nothing Then smgr.checkPermission(ACCESS_PERMISSION)
            Dim lookup_Renamed As Lookup = lookup.IMPL_LOOKUP ' use maximally privileged lookup
            Return lookup_Renamed.revealDirect(target).reflectAs(expected, lookup_Renamed)
        End Function
        ' Copied from AccessibleObject, as used by Method.setAccessible, etc.:
        Private Shared ReadOnly ACCESS_PERMISSION As java.security.Permission = New ReflectPermission("suppressAccessChecks")

        ''' <summary>
        ''' A <em>lookup object</em> is a factory for creating method handles,
        ''' when the creation requires access checking.
        ''' Method handles do not perform
        ''' access checks when they are called, but rather when they are created.
        ''' Therefore, method handle access
        ''' restrictions must be enforced when a method handle is created.
        ''' The caller class against which those restrictions are enforced
        ''' is known as the <seealso cref="#lookupClass lookup class"/>.
        ''' <p>
        ''' A lookup class which needs to create method handles will call
        ''' <seealso cref="MethodHandles#lookup MethodHandles.lookup"/> to create a factory for itself.
        ''' When the {@code Lookup} factory object is created, the identity of the lookup class is
        ''' determined, and securely stored in the {@code Lookup} object.
        ''' The lookup class (or its delegates) may then use factory methods
        ''' on the {@code Lookup} object to create method handles for access-checked members.
        ''' This includes all methods, constructors, and fields which are allowed to the lookup [Class],
        ''' even private ones.
        ''' 
        ''' <h1><a name="lookups"></a>Lookup Factory Methods</h1>
        ''' The factory methods on a {@code Lookup} object correspond to all major
        ''' use cases for methods, constructors, and fields.
        ''' Each method handle created by a factory method is the functional
        ''' equivalent of a particular <em>bytecode behavior</em>.
        ''' (Bytecode behaviors are described in section 5.4.3.5 of the Java Virtual Machine Specification.)
        ''' Here is a summary of the correspondence between these factory methods and
        ''' the behavior the resulting method handles:
        ''' <table border=1 cellpadding=5 summary="lookup method behaviors">
        ''' <tr>
        '''     <th><a name="equiv"></a>lookup expression</th>
        '''     <th>member</th>
        '''     <th>bytecode behavior</th>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findGetter lookup.findGetter(C.class,"f",FT.class)"/></td>
        '''     <td>{@code FT f;}</td><td>{@code (T) this.f;}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findStaticGetter lookup.findStaticGetter(C.class,"f",FT.class)"/></td>
        '''     <td>{@code static}<br>{@code FT f;}</td><td>{@code (T) C.f;}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findSetter lookup.findSetter(C.class,"f",FT.class)"/></td>
        '''     <td>{@code FT f;}</td><td>{@code this.f = x;}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findStaticSetter lookup.findStaticSetter(C.class,"f",FT.class)"/></td>
        '''     <td>{@code static}<br>{@code FT f;}</td><td>{@code C.f = arg;}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findVirtual lookup.findVirtual(C.class,"m",MT)"/></td>
        '''     <td>{@code T m(A*);}</td><td>{@code (T) this.m(arg*);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findStatic lookup.findStatic(C.class,"m",MT)"/></td>
        '''     <td>{@code static}<br>{@code T m(A*);}</td><td>{@code (T) C.m(arg*);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findSpecial lookup.findSpecial(C.class,"m",MT,this.class)"/></td>
        '''     <td>{@code T m(A*);}</td><td>{@code (T) super.m(arg*);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#findConstructor lookup.findConstructor(C.class,MT)"/></td>
        '''     <td>{@code C(A*);}</td><td>{@code new C(arg*);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflectGetter lookup.unreflectGetter(aField)"/></td>
        '''     <td>({@code static})?<br>{@code FT f;}</td><td>{@code (FT) aField.get(thisOrNull);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflectSetter lookup.unreflectSetter(aField)"/></td>
        '''     <td>({@code static})?<br>{@code FT f;}</td><td>{@code aField.set(thisOrNull, arg);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflect lookup.unreflect(aMethod)"/></td>
        '''     <td>({@code static})?<br>{@code T m(A*);}</td><td>{@code (T) aMethod.invoke(thisOrNull, arg*);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflectConstructor lookup.unreflectConstructor(aConstructor)"/></td>
        '''     <td>{@code C(A*);}</td><td>{@code (C) aConstructor.newInstance(arg*);}</td>
        ''' </tr>
        ''' <tr>
        '''     <td><seealso cref="java.lang.invoke.MethodHandles.Lookup#unreflect lookup.unreflect(aMethod)"/></td>
        '''     <td>({@code static})?<br>{@code T m(A*);}</td><td>{@code (T) aMethod.invoke(thisOrNull, arg*);}</td>
        ''' </tr>
        ''' </table>
        ''' 
        ''' Here, the type {@code C} is the class or interface being searched for a member,
        ''' documented as a parameter named {@code refc} in the lookup methods.
        ''' The method type {@code MT} is composed from the return type {@code T}
        ''' and the sequence of argument types {@code A*}.
        ''' The constructor also has a sequence of argument types {@code A*} and
        ''' is deemed to return the newly-created object of type {@code C}.
        ''' Both {@code MT} and the field type {@code FT} are documented as a parameter named {@code type}.
        ''' The formal parameter {@code this} stands for the self-reference of type {@code C};
        ''' if it is present, it is always the leading argument to the method handle invocation.
        ''' (In the case of some {@code protected} members, {@code this} may be
        ''' restricted in type to the lookup class; see below.)
        ''' The name {@code arg} stands for all the other method handle arguments.
        ''' In the code examples for the Core Reflection API, the name {@code thisOrNull}
        ''' stands for a null reference if the accessed method or field is static,
        ''' and {@code this} otherwise.
        ''' The names {@code aMethod}, {@code aField}, and {@code aConstructor} stand
        ''' for reflective objects corresponding to the given members.
        ''' <p>
        ''' In cases where the given member is of variable arity (i.e., a method or constructor)
        ''' the returned method handle will also be of <seealso cref="MethodHandle#asVarargsCollector variable arity"/>.
        ''' In all other cases, the returned method handle will be of fixed arity.
        ''' <p style="font-size:smaller;">
        ''' <em>Discussion:</em>
        ''' The equivalence between looked-up method handles and underlying
        ''' class members and bytecode behaviors
        ''' can break down in a few ways:
        ''' <ul style="font-size:smaller;">
        ''' <li>If {@code C} is not symbolically accessible from the lookup class's loader,
        ''' the lookup can still succeed, even when there is no equivalent
        ''' Java expression or bytecoded constant.
        ''' <li>Likewise, if {@code T} or {@code MT}
        ''' is not symbolically accessible from the lookup class's loader,
        ''' the lookup can still succeed.
        ''' For example, lookups for {@code MethodHandle.invokeExact} and
        ''' {@code MethodHandle.invoke} will always succeed, regardless of requested type.
        ''' <li>If there is a security manager installed, it can forbid the lookup
        ''' on various grounds (<a href="MethodHandles.Lookup.html#secmgr">see below</a>).
        ''' By contrast, the {@code ldc} instruction on a {@code CONSTANT_MethodHandle}
        ''' constant is not subject to security manager checks.
        ''' <li>If the looked-up method has a
        ''' <a href="MethodHandle.html#maxarity">very large arity</a>,
        ''' the method handle creation may fail, due to the method handle
        ''' type having too many parameters.
        ''' </ul>
        ''' 
        ''' <h1><a name="access"></a>Access checking</h1>
        ''' Access checks are applied in the factory methods of {@code Lookup},
        ''' when a method handle is created.
        ''' This is a key difference from the Core Reflection API, since
        ''' <seealso cref="java.lang.reflect.Method#invoke java.lang.reflect.Method.invoke"/>
        ''' performs access checking against every caller, on every call.
        ''' <p>
        ''' All access checks start from a {@code Lookup} object, which
        ''' compares its recorded lookup class against all requests to
        ''' create method handles.
        ''' A single {@code Lookup} object can be used to create any number
        ''' of access-checked method handles, all checked against a single
        ''' lookup class.
        ''' <p>
        ''' A {@code Lookup} object can be shared with other trusted code,
        ''' such as a metaobject protocol.
        ''' A shared {@code Lookup} object delegates the capability
        ''' to create method handles on private members of the lookup class.
        ''' Even if privileged code uses the {@code Lookup} object,
        ''' the access checking is confined to the privileges of the
        ''' original lookup class.
        ''' <p>
        ''' A lookup can fail, because
        ''' the containing class is not accessible to the lookup [Class], or
        ''' because the desired class member is missing, or because the
        ''' desired class member is not accessible to the lookup [Class], or
        ''' because the lookup object is not trusted enough to access the member.
        ''' In any of these cases, a {@code ReflectiveOperationException} will be
        ''' thrown from the attempted lookup.  The exact class will be one of
        ''' the following:
        ''' <ul>
        ''' <li>NoSuchMethodException &mdash; if a method is requested but does not exist
        ''' <li>NoSuchFieldException &mdash; if a field is requested but does not exist
        ''' <li>IllegalAccessException &mdash; if the member exists but an access check fails
        ''' </ul>
        ''' <p>
        ''' In general, the conditions under which a method handle may be
        ''' looked up for a method {@code M} are no more restrictive than the conditions
        ''' under which the lookup class could have compiled, verified, and resolved a call to {@code M}.
        ''' Where the JVM would raise exceptions like {@code NoSuchMethodError},
        ''' a method handle lookup will generally raise a corresponding
        ''' checked exception, such as {@code NoSuchMethodException}.
        ''' And the effect of invoking the method handle resulting from the lookup
        ''' is <a href="MethodHandles.Lookup.html#equiv">exactly equivalent</a>
        ''' to executing the compiled, verified, and resolved call to {@code M}.
        ''' The same point is true of fields and constructors.
        ''' <p style="font-size:smaller;">
        ''' <em>Discussion:</em>
        ''' Access checks only apply to named and reflected methods,
        ''' constructors, and fields.
        ''' Other method handle creation methods, such as
        ''' <seealso cref="MethodHandle#asType MethodHandle.asType"/>,
        ''' do not require any access checks, and are used
        ''' independently of any {@code Lookup} object.
        ''' <p>
        ''' If the desired member is {@code protected}, the usual JVM rules apply,
        ''' including the requirement that the lookup class must be either be in the
        ''' same package as the desired member, or must inherit that member.
        ''' (See the Java Virtual Machine Specification, sections 4.9.2, 5.4.3.5, and 6.4.)
        ''' In addition, if the desired member is a non-static field or method
        ''' in a different package, the resulting method handle may only be applied
        ''' to objects of the lookup class or one of its subclasses.
        ''' This requirement is enforced by narrowing the type of the leading
        ''' {@code this} parameter from {@code C}
        ''' (which will necessarily be a superclass of the lookup [Class])
        ''' to the lookup class itself.
        ''' <p>
        ''' The JVM imposes a similar requirement on {@code invokespecial} instruction,
        ''' that the receiver argument must match both the resolved method <em>and</em>
        ''' the current class.  Again, this requirement is enforced by narrowing the
        ''' type of the leading parameter to the resulting method handle.
        ''' (See the Java Virtual Machine Specification, section 4.10.1.9.)
        ''' <p>
        ''' The JVM represents constructors and static initializer blocks as internal methods
        ''' with special names ({@code "<init>"} and {@code "<clinit>"}).
        ''' The internal syntax of invocation instructions allows them to refer to such internal
        ''' methods as if they were normal methods, but the JVM bytecode verifier rejects them.
        ''' A lookup of such an internal method will produce a {@code NoSuchMethodException}.
        ''' <p>
        ''' In some cases, access between nested classes is obtained by the Java compiler by creating
        ''' an wrapper method to access a private method of another class
        ''' in the same top-level declaration.
        ''' For example, a nested class {@code C.D}
        ''' can access private members within other related classes such as
        ''' {@code C}, {@code C.D.E}, or {@code C.B},
        ''' but the Java compiler may need to generate wrapper methods in
        ''' those related classes.  In such cases, a {@code Lookup} object on
        ''' {@code C.E} would be unable to those private members.
        ''' A workaround for this limitation is the <seealso cref="Lookup#in Lookup.in"/> method,
        ''' which can transform a lookup on {@code C.E} into one on any of those other
        ''' classes, without special elevation of privilege.
        ''' <p>
        ''' The accesses permitted to a given lookup object may be limited,
        ''' according to its set of <seealso cref="#lookupModes lookupModes"/>,
        ''' to a subset of members normally accessible to the lookup class.
        ''' For example, the <seealso cref="MethodHandles#publicLookup publicLookup"/>
        ''' method produces a lookup object which is only allowed to access
        ''' public members in public classes.
        ''' The caller sensitive method <seealso cref="MethodHandles#lookup lookup"/>
        ''' produces a lookup object with full capabilities relative to
        ''' its caller [Class], to emulate all supported bytecode behaviors.
        ''' Also, the <seealso cref="Lookup#in Lookup.in"/> method may produce a lookup object
        ''' with fewer access modes than the original lookup object.
        ''' 
        ''' <p style="font-size:smaller;">
        ''' <a name="privacc"></a>
        ''' <em>Discussion of private access:</em>
        ''' We say that a lookup has <em>private access</em>
        ''' if its <seealso cref="#lookupModes lookup modes"/>
        ''' include the possibility of accessing {@code private} members.
        ''' As documented in the relevant methods elsewhere,
        ''' only lookups with private access possess the following capabilities:
        ''' <ul style="font-size:smaller;">
        ''' <li>access private fields, methods, and constructors of the lookup class
        ''' <li>create method handles which invoke <a href="MethodHandles.Lookup.html#callsens">caller sensitive</a> methods,
        '''     such as {@code Class.forName}
        ''' <li>create method handles which <seealso cref="Lookup#findSpecial emulate invokespecial"/> instructions
        ''' <li>avoid <a href="MethodHandles.Lookup.html#secmgr">package access checks</a>
        '''     for classes accessible to the lookup class
        ''' <li>create <seealso cref="Lookup#in delegated lookup objects"/> which have private access to other classes
        '''     within the same package member
        ''' </ul>
        ''' <p style="font-size:smaller;">
        ''' Each of these permissions is a consequence of the fact that a lookup object
        ''' with private access can be securely traced back to an originating [Class],
        ''' whose <a href="MethodHandles.Lookup.html#equiv">bytecode behaviors</a> and Java language access permissions
        ''' can be reliably determined and emulated by method handles.
        ''' 
        ''' <h1><a name="secmgr"></a>Security manager interactions</h1>
        ''' Although bytecode instructions can only refer to classes in
        ''' a related class loader, this API can search for methods in any
        ''' [Class], as long as a reference to its {@code Class} object is
        ''' available.  Such cross-loader references are also possible with the
        ''' Core Reflection API, and are impossible to bytecode instructions
        ''' such as {@code invokestatic} or {@code getfield}.
        ''' There is a <seealso cref="java.lang.SecurityManager security manager API"/>
        ''' to allow applications to check such cross-loader references.
        ''' These checks apply to both the {@code MethodHandles.Lookup} API
        ''' and the Core Reflection API
        ''' (as found on <seealso cref="java.lang.Class Class"/>).
        ''' <p>
        ''' If a security manager is present, member lookups are subject to
        ''' additional checks.
        ''' From one to three calls are made to the security manager.
        ''' Any of these calls can refuse access by throwing a
        ''' <seealso cref="java.lang.SecurityException SecurityException"/>.
        ''' Define {@code smgr} as the security manager,
        ''' {@code lookc} as the lookup class of the current lookup object,
        ''' {@code refc} as the containing class in which the member
        ''' is being sought, and {@code defc} as the class in which the
        ''' member is actually defined.
        ''' The value {@code lookc} is defined as <em>not present</em>
        ''' if the current lookup object does not have
        ''' <a href="MethodHandles.Lookup.html#privacc">private access</a>.
        ''' The calls are made according to the following rules:
        ''' <ul>
        ''' <li><b>Step 1:</b>
        '''     If {@code lookc} is not present, or if its class loader is not
        '''     the same as or an ancestor of the class loader of {@code refc},
        '''     then {@link SecurityManager#checkPackageAccess
        '''     smgr.checkPackageAccess(refcPkg)} is called,
        '''     where {@code refcPkg} is the package of {@code refc}.
        ''' <li><b>Step 2:</b>
        '''     If the retrieved member is not public and
        '''     {@code lookc} is not present, then
        '''     <seealso cref="SecurityManager#checkPermission smgr.checkPermission"/>
        '''     with {@code RuntimePermission("accessDeclaredMembers")} is called.
        ''' <li><b>Step 3:</b>
        '''     If the retrieved member is not public,
        '''     and if {@code lookc} is not present,
        '''     and if {@code defc} and {@code refc} are different,
        '''     then {@link SecurityManager#checkPackageAccess
        '''     smgr.checkPackageAccess(defcPkg)} is called,
        '''     where {@code defcPkg} is the package of {@code defc}.
        ''' </ul>
        ''' Security checks are performed after other access checks have passed.
        ''' Therefore, the above rules presuppose a member that is public,
        ''' or else that is being accessed from a lookup class that has
        ''' rights to access the member.
        ''' 
        ''' <h1><a name="callsens"></a>Caller sensitive methods</h1>
        ''' A small number of Java methods have a special property called caller sensitivity.
        ''' A <em>caller-sensitive</em> method can behave differently depending on the
        ''' identity of its immediate caller.
        ''' <p>
        ''' If a method handle for a caller-sensitive method is requested,
        ''' the general rules for <a href="MethodHandles.Lookup.html#equiv">bytecode behaviors</a> apply,
        ''' but they take account of the lookup class in a special way.
        ''' The resulting method handle behaves as if it were called
        ''' from an instruction contained in the lookup [Class],
        ''' so that the caller-sensitive method detects the lookup class.
        ''' (By contrast, the invoker of the method handle is disregarded.)
        ''' Thus, in the case of caller-sensitive methods,
        ''' different lookup classes may give rise to
        ''' differently behaving method handles.
        ''' <p>
        ''' In cases where the lookup object is
        ''' <seealso cref="MethodHandles#publicLookup() publicLookup()"/>,
        ''' or some other lookup object without
        ''' <a href="MethodHandles.Lookup.html#privacc">private access</a>,
        ''' the lookup class is disregarded.
        ''' In such cases, no caller-sensitive method handle can be created,
        ''' access is forbidden, and the lookup fails with an
        ''' {@code IllegalAccessException}.
        ''' <p style="font-size:smaller;">
        ''' <em>Discussion:</em>
        ''' For example, the caller-sensitive method
        ''' <seealso cref="java.lang.Class#forName(String) Class.forName(x)"/>
        ''' can return varying classes or throw varying exceptions,
        ''' depending on the class loader of the class that calls it.
        ''' A public lookup of {@code Class.forName} will fail, because
        ''' there is no reasonable way to determine its bytecode behavior.
        ''' <p style="font-size:smaller;">
        ''' If an application caches method handles for broad sharing,
        ''' it should use {@code publicLookup()} to create them.
        ''' If there is a lookup of {@code Class.forName}, it will fail,
        ''' and the application must take appropriate action in that case.
        ''' It may be that a later lookup, perhaps during the invocation of a
        ''' bootstrap method, can incorporate the specific identity
        ''' of the caller, making the method accessible.
        ''' <p style="font-size:smaller;">
        ''' The function {@code MethodHandles.lookup} is caller sensitive
        ''' so that there can be a secure foundation for lookups.
        ''' Nearly all other methods in the JSR 292 API rely on lookup
        ''' objects to check access requests.
        ''' </summary>
        Public NotInheritable Class Lookup
            ''' <summary>
            ''' The class on behalf of whom the lookup is being performed. </summary>
            Private ReadOnly lookupClass_Renamed As [Class]

            ''' <summary>
            ''' The allowed sorts of members which may be looked up (PUBLIC, etc.). </summary>
            Private ReadOnly allowedModes As Integer

            ''' <summary>
            ''' A single-bit mask representing {@code public} access,
            '''  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
            '''  The value, {@code 0x01}, happens to be the same as the value of the
            '''  {@code public} <seealso cref="java.lang.reflect.Modifier#PUBLIC modifier bit"/>.
            ''' </summary>
            Public Const [PUBLIC] As Integer = Modifier.PUBLIC

            ''' <summary>
            ''' A single-bit mask representing {@code private} access,
            '''  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
            '''  The value, {@code 0x02}, happens to be the same as the value of the
            '''  {@code private} <seealso cref="java.lang.reflect.Modifier#PRIVATE modifier bit"/>.
            ''' </summary>
            Public Shared ReadOnly [PRIVATE] As Integer = Modifier.PRIVATE

            ''' <summary>
            ''' A single-bit mask representing {@code protected} access,
            '''  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
            '''  The value, {@code 0x04}, happens to be the same as the value of the
            '''  {@code protected} <seealso cref="java.lang.reflect.Modifier#PROTECTED modifier bit"/>.
            ''' </summary>
            Public Const [PROTECTED] As Integer = Modifier.PROTECTED

            ''' <summary>
            ''' A single-bit mask representing {@code package} access (default access),
            '''  which may contribute to the result of <seealso cref="#lookupModes lookupModes"/>.
            '''  The value is {@code 0x08}, which does not correspond meaningfully to
            '''  any particular <seealso cref="java.lang.reflect.Modifier modifier bit"/>.
            ''' </summary>
            Public Const PACKAGE As Integer = Modifier.STATIC

            Private Shared ReadOnly ALL_MODES As Integer = ([PUBLIC] Or [PRIVATE] Or [PROTECTED] Or PACKAGE)
            Private Const TRUSTED As Integer = -1

            Private Shared Function fixmods(ByVal mods As Integer) As Integer
                mods = mods And (ALL_MODES - PACKAGE)
                Return If(mods <> 0, mods, PACKAGE)
            End Function

            ''' <summary>
            ''' Tells which class is performing the lookup.  It is this class against
            '''  which checks are performed for visibility and access permissions.
            '''  <p>
            '''  The class implies a maximum level of access permission,
            '''  but the permissions may be additionally limited by the bitmask
            '''  <seealso cref="#lookupModes lookupModes"/>, which controls whether non-public members
            '''  can be accessed. </summary>
            '''  <returns> the lookup [Class], on behalf of which this lookup object finds members </returns>
            Public Function lookupClass() As [Class]
                Return lookupClass_Renamed
            End Function

            ' This is just for calling out to MethodHandleImpl.
            Private Function lookupClassOrNull() As [Class]
                Return If(allowedModes = TRUSTED, Nothing, lookupClass_Renamed)
            End Function

            ''' <summary>
            ''' Tells which access-protection classes of members this lookup object can produce.
            '''  The result is a bit-mask of the bits
            '''  <seealso cref="#PUBLIC PUBLIC (0x01)"/>,
            '''  <seealso cref="#PRIVATE PRIVATE (0x02)"/>,
            '''  <seealso cref="#PROTECTED PROTECTED (0x04)"/>,
            '''  and <seealso cref="#PACKAGE PACKAGE (0x08)"/>.
            '''  <p>
            '''  A freshly-created lookup object
            '''  on the <seealso cref="java.lang.invoke.MethodHandles#lookup() caller's class"/>
            '''  has all possible bits set, since the caller class can access all its own members.
            '''  A lookup object on a new lookup class
            '''  <seealso cref="java.lang.invoke.MethodHandles.Lookup#in created from a previous lookup object"/>
            '''  may have some mode bits set to zero.
            '''  The purpose of this is to restrict access via the new lookup object,
            '''  so that it can access only names which can be reached by the original
            '''  lookup object, and also by the new lookup class. </summary>
            '''  <returns> the lookup modes, which limit the kinds of access performed by this lookup object </returns>
            Public Function lookupModes() As Integer
                Return allowedModes And ALL_MODES
            End Function

            ''' <summary>
            ''' Embody the current class (the lookupClass) as a lookup class
            ''' for method handle creation.
            ''' Must be called by from a method in this package,
            ''' which in turn is called by a method not in this package.
            ''' </summary>
            Friend Sub New(ByVal lookupClass As [Class])
                Me.New(lookupClass, ALL_MODES)
                ' make sure we haven't accidentally picked up a privileged class:
                checkUnprivilegedlookupClass(lookupClass, ALL_MODES)
            End Sub

            Private Sub New(ByVal lookupClass As [Class], ByVal allowedModes As Integer)
                Me.lookupClass_Renamed = lookupClass
                Me.allowedModes = allowedModes
            End Sub

            ''' <summary>
            ''' Creates a lookup on the specified new lookup class.
            ''' The resulting object will report the specified
            ''' class as its own <seealso cref="#lookupClass lookupClass"/>.
            ''' <p>
            ''' However, the resulting {@code Lookup} object is guaranteed
            ''' to have no more access capabilities than the original.
            ''' In particular, access capabilities can be lost as follows:<ul>
            ''' <li>If the new lookup class differs from the old one,
            ''' protected members will not be accessible by virtue of inheritance.
            ''' (Protected members may continue to be accessible because of package sharing.)
            ''' <li>If the new lookup class is in a different package
            ''' than the old one, protected and default (package) members will not be accessible.
            ''' <li>If the new lookup class is not within the same package member
            ''' as the old one, private members will not be accessible.
            ''' <li>If the new lookup class is not accessible to the old lookup [Class],
            ''' then no members, not even public members, will be accessible.
            ''' (In all other cases, public members will continue to be accessible.)
            ''' </ul>
            ''' </summary>
            ''' <param name="requestedLookupClass"> the desired lookup class for the new lookup object </param>
            ''' <returns> a lookup object which reports the desired lookup class </returns>
            ''' <exception cref="NullPointerException"> if the argument is null </exception>
            Public Function [in](ByVal requestedLookupClass As [Class]) As Lookup
                requestedLookupClass.GetType() ' null check
                If allowedModes = TRUSTED Then ' IMPL_LOOKUP can make any lookup at all Return New Lookup(requestedLookupClass, ALL_MODES)
                    If requestedLookupClass Is Me.lookupClass_Renamed Then Return Me ' keep same capabilities
                    Dim newModes As Integer = (allowedModes And (ALL_MODES And (Not [PROTECTED])))
                    If (newModes And PACKAGE) <> 0 AndAlso (Not sun.invoke.util.VerifyAccess.isSamePackage(Me.lookupClass_Renamed, requestedLookupClass)) Then newModes = newModes And Not (PACKAGE Or [PRIVATE])
                    ' Allow nestmate lookups to be created without special privilege:
                    If (newModes And [PRIVATE]) <> 0 AndAlso (Not sun.invoke.util.VerifyAccess.isSamePackageMember(Me.lookupClass_Renamed, requestedLookupClass)) Then newModes = newModes And Not [PRIVATE]
                    If (newModes And [PUBLIC]) <> 0 AndAlso (Not sun.invoke.util.VerifyAccess.isClassAccessible(requestedLookupClass, Me.lookupClass_Renamed, allowedModes)) Then newModes = 0
                    checkUnprivilegedlookupClass(requestedLookupClass, newModes)
                    Return New Lookup(requestedLookupClass, newModes)
            End Function

            ' Make sure outer class is initialized first.

            ''' <summary>
            ''' Version of lookup which is trusted minimally.
            '''  It can only be used to create method handles to
            '''  publicly accessible members.
            ''' </summary>
            Friend Shared ReadOnly PUBLIC_LOOKUP As New Lookup(GetType(Object), [PUBLIC])

            ''' <summary>
            ''' Package-private version of lookup which is trusted. </summary>
            Friend Shared ReadOnly IMPL_LOOKUP As New Lookup(GetType(Object), TRUSTED)

            Private Shared Sub checkUnprivilegedlookupClass(ByVal lookupClass As [Class], ByVal allowedModes As Integer)
                Dim name As String = lookupClass.name
                If name.StartsWith("java.lang.invoke.") Then Throw New IllegalArgumentException("illegal lookupClass: " & lookupClass)

                ' For caller-sensitive MethodHandles.lookup()
                ' disallow lookup more restricted packages
                If allowedModes = ALL_MODES AndAlso lookupClass.classLoader Is Nothing Then
                    If name.StartsWith("java.") OrElse (name.StartsWith("sun.") AndAlso (Not name.StartsWith("sun.invoke."))) Then
                        Throw New IllegalArgumentException("illegal lookupClass: " & lookupClass)
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Displays the name of the class from which lookups are to be made.
            ''' (The name is the one reported by <seealso cref="java.lang.Class#getName() Class.getName"/>.)
            ''' If there are restrictions on the access permitted to this lookup,
            ''' this is indicated by adding a suffix to the class name, consisting
            ''' of a slash and a keyword.  The keyword represents the strongest
            ''' allowed access, and is chosen as follows:
            ''' <ul>
            ''' <li>If no access is allowed, the suffix is "/noaccess".
            ''' <li>If only public access is allowed, the suffix is "/public".
            ''' <li>If only public and package access are allowed, the suffix is "/package".
            ''' <li>If only public, package, and private access are allowed, the suffix is "/private".
            ''' </ul>
            ''' If none of the above cases apply, it is the case that full
            ''' access (public, package, private, and protected) is allowed.
            ''' In this case, no suffix is added.
            ''' This is true only of an object obtained originally from
            ''' <seealso cref="java.lang.invoke.MethodHandles#lookup MethodHandles.lookup"/>.
            ''' Objects created by <seealso cref="java.lang.invoke.MethodHandles.Lookup#in Lookup.in"/>
            ''' always have restricted access, and will display a suffix.
            ''' <p>
            ''' (It may seem strange that protected access should be
            ''' stronger than private access.  Viewed independently from
            ''' package access, protected access is the first to be lost,
            ''' because it requires a direct subclass relationship between
            ''' caller and callee.) </summary>
            ''' <seealso cref= #in </seealso>
            Public Overrides Function ToString() As String
                Dim cname As String = lookupClass_Renamed.name
                Select Case allowedModes
                    Case 0 ' no privileges
                        Return cname & "/noaccess"
                    Case [PUBLIC]
                        Return cname & "/public"
                    Case [PUBLIC] Or PACKAGE
                        Return cname & "/package"
                    Case ALL_MODES And Not [PROTECTED]
                        Return cname & "/private"
                    Case ALL_MODES
                        Return cname
                    Case TRUSTED
                        Return "/trusted" ' internal only; not exported
                    Case Else ' Should not happen, but it's a bitfield...
                        cname = cname & "/" & java.lang.[Integer].toHexString(allowedModes)
                        Assert(False) : cname
                        Return cname
                End Select
            End Function

            ''' <summary>
            ''' Produces a method handle for a static method.
            ''' The type of the method handle will be that of the method.
            ''' (Since static methods do not take receivers, there is no
            ''' additional receiver argument inserted into the method handle type,
            ''' as there would be with <seealso cref="#findVirtual findVirtual"/> or <seealso cref="#findSpecial findSpecial"/>.)
            ''' The method and all its argument types must be accessible to the lookup object.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the method's variable arity modifier bit ({@code 0x0080}) is set.
            ''' <p>
            ''' If the returned method handle is invoked, the method's class will
            ''' be initialized, if it has not already been initialized.
            ''' <p><b>Example:</b>
            ''' <blockquote><pre>{@code
            ''' import static java.lang.invoke.MethodHandles.*;
            ''' import static java.lang.invoke.MethodType.*;
            ''' ...
            ''' MethodHandle MH_asList = publicLookup().findStatic(Arrays.class,
            ''' "asList", methodType(List.class, Object[].class));
            ''' assertEquals("[x, y]", MH_asList.invoke("x", "y").toString());
            ''' }</pre></blockquote> </summary>
            ''' <param name="refc"> the class from which the method is accessed </param>
            ''' <param name="name"> the name of the method </param>
            ''' <param name="type"> the type of the method </param>
            ''' <returns> the desired method handle </returns>
            ''' <exception cref="NoSuchMethodException"> if the method does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails,
            '''                                or if the method is not {@code static},
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findStatic(ByVal refc As [Class], ByVal name As String, ByVal type As MethodType) As MethodHandle
                Dim method As MemberName = resolveOrFail(REF_invokeStatic, refc, name, type)
                Return getDirectMethod(REF_invokeStatic, refc, method, findBoundCallerClass(method))
            End Function

            ''' <summary>
            ''' Produces a method handle for a virtual method.
            ''' The type of the method handle will be that of the method,
            ''' with the receiver type (usually {@code refc}) prepended.
            ''' The method and all its argument types must be accessible to the lookup object.
            ''' <p>
            ''' When called, the handle will treat the first argument as a receiver
            ''' and dispatch on the receiver's type to determine which method
            ''' implementation to enter.
            ''' (The dispatching action is identical with that performed by an
            ''' {@code invokevirtual} or {@code invokeinterface} instruction.)
            ''' <p>
            ''' The first argument will be of type {@code refc} if the lookup
            ''' class has full privileges to access the member.  Otherwise
            ''' the member must be {@code protected} and the first argument
            ''' will be restricted in type to the lookup class.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the method's variable arity modifier bit ({@code 0x0080}) is set.
            ''' <p>
            ''' Because of the general <a href="MethodHandles.Lookup.html#equiv">equivalence</a> between {@code invokevirtual}
            ''' instructions and method handles produced by {@code findVirtual},
            ''' if the class is {@code MethodHandle} and the name string is
            ''' {@code invokeExact} or {@code invoke}, the resulting
            ''' method handle is equivalent to one produced by
            ''' <seealso cref="java.lang.invoke.MethodHandles#exactInvoker MethodHandles.exactInvoker"/> or
            ''' <seealso cref="java.lang.invoke.MethodHandles#invoker MethodHandles.invoker"/>
            ''' with the same {@code type} argument.
            ''' 
            ''' <b>Example:</b>
            ''' <blockquote><pre>{@code
            ''' import static java.lang.invoke.MethodHandles.*;
            ''' import static java.lang.invoke.MethodType.*;
            ''' ...
            ''' MethodHandle MH_concat = publicLookup().findVirtual(String.class,
            ''' "concat", methodType(String.class, String.class));
            ''' MethodHandle MH_hashCode = publicLookup().findVirtual(Object.class,
            ''' "hashCode", methodType(int.class));
            ''' MethodHandle MH_hashCode_String = publicLookup().findVirtual(String.class,
            ''' "hashCode", methodType(int.class));
            ''' assertEquals("xy", (String) MH_concat.invokeExact("x", "y"));
            ''' assertEquals("xy".hashCode(), (int) MH_hashCode.invokeExact((Object)"xy"));
            ''' assertEquals("xy".hashCode(), (int) MH_hashCode_String.invokeExact("xy"));
            ''' // interface method:
            ''' MethodHandle MH_subSequence = publicLookup().findVirtual(CharSequence.class,
            ''' "subSequence", methodType(CharSequence.class, int.class, int.class));
            ''' assertEquals("def", MH_subSequence.invoke("abcdefghi", 3, 6).toString());
            ''' // constructor "internal method" must be accessed differently:
            ''' MethodType MT_newString = methodType(void.class); //()V for new String()
            ''' try { assertEquals("impossible", lookup()
            ''' .findVirtual(String.class, "<init>", MT_newString));
            ''' } catch (NoSuchMethodException ex) { } // OK
            ''' MethodHandle MH_newString = publicLookup()
            ''' .findConstructor(String.class, MT_newString);
            ''' assertEquals("", (String) MH_newString.invokeExact());
            ''' }</pre></blockquote>
            ''' </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="name"> the name of the method </param>
            ''' <param name="type"> the type of the method, with the receiver argument omitted </param>
            ''' <returns> the desired method handle </returns>
            ''' <exception cref="NoSuchMethodException"> if the method does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails,
            '''                                or if the method is {@code static}
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findVirtual(ByVal refc As [Class], ByVal name As String, ByVal type As MethodType) As MethodHandle
                If refc Is GetType(MethodHandle) Then
                    Dim mh As MethodHandle = findVirtualForMH(name, type)
                    If mh IsNot Nothing Then Return mh
                End If
                Dim refKind As SByte = (If(refc.interface, REF_invokeInterface, REF_invokeVirtual))
                Dim method As MemberName = resolveOrFail(refKind, refc, name, type)
                Return getDirectMethod(refKind, refc, method, findBoundCallerClass(method))
            End Function
            Private Function findVirtualForMH(ByVal name As String, ByVal type As MethodType) As MethodHandle
                ' these names require special lookups because of the implicit MethodType argument
                If "invoke".Equals(name) Then Return invoker(type)
                If "invokeExact".Equals(name) Then Return exactInvoker(type)
                If "invokeBasic".Equals(name) Then Return basicInvoker(type)
                Assert((Not MemberName.isMethodHandleInvokeName(name)))
                Return Nothing
            End Function

            ''' <summary>
            ''' Produces a method handle which creates an object and initializes it, using
            ''' the constructor of the specified type.
            ''' The parameter types of the method handle will be those of the constructor,
            ''' while the return type will be a reference to the constructor's class.
            ''' The constructor and all its argument types must be accessible to the lookup object.
            ''' <p>
            ''' The requested type must have a return type of {@code void}.
            ''' (This is consistent with the JVM's treatment of constructor type descriptors.)
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the constructor's variable arity modifier bit ({@code 0x0080}) is set.
            ''' <p>
            ''' If the returned method handle is invoked, the constructor's class will
            ''' be initialized, if it has not already been initialized.
            ''' <p><b>Example:</b>
            ''' <blockquote><pre>{@code
            ''' import static java.lang.invoke.MethodHandles.*;
            ''' import static java.lang.invoke.MethodType.*;
            ''' ...
            ''' MethodHandle MH_newArrayList = publicLookup().findConstructor(
            ''' ArrayList.class, methodType(void.class, Collection.class));
            ''' Collection orig = Arrays.asList("x", "y");
            ''' Collection copy = (ArrayList) MH_newArrayList.invokeExact(orig);
            ''' assert(orig != copy);
            ''' assertEquals(orig, copy);
            ''' // a variable-arity constructor:
            ''' MethodHandle MH_newProcessBuilder = publicLookup().findConstructor(
            ''' ProcessBuilder.class, methodType(void.class, String[].class));
            ''' ProcessBuilder pb = (ProcessBuilder)
            ''' MH_newProcessBuilder.invoke("x", "y", "z");
            ''' assertEquals("[x, y, z]", pb.command().toString());
            ''' }</pre></blockquote> </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="type"> the type of the method, with the receiver argument omitted, and a void return type </param>
            ''' <returns> the desired method handle </returns>
            ''' <exception cref="NoSuchMethodException"> if the constructor does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findConstructor(ByVal refc As [Class], ByVal type As MethodType) As MethodHandle
                Dim name As String = "<init>"
                Dim ctor As MemberName = resolveOrFail(REF_newInvokeSpecial, refc, name, type)
                Return getDirectConstructor(refc, ctor)
            End Function

            ''' <summary>
            ''' Produces an early-bound method handle for a virtual method.
            ''' It will bypass checks for overriding methods on the receiver,
            ''' <a href="MethodHandles.Lookup.html#equiv">as if called</a> from an {@code invokespecial}
            ''' instruction from within the explicitly specified {@code specialCaller}.
            ''' The type of the method handle will be that of the method,
            ''' with a suitably restricted receiver type prepended.
            ''' (The receiver type will be {@code specialCaller} or a subtype.)
            ''' The method and all its argument types must be accessible
            ''' to the lookup object.
            ''' <p>
            ''' Before method resolution,
            ''' if the explicitly specified caller class is not identical with the
            ''' lookup [Class], or if this lookup object does not have
            ''' <a href="MethodHandles.Lookup.html#privacc">private access</a>
            ''' privileges, the access fails.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the method's variable arity modifier bit ({@code 0x0080}) is set.
            ''' <p style="font-size:smaller;">
            ''' <em>(Note:  JVM internal methods named {@code "<init>"} are not visible to this API,
            ''' even though the {@code invokespecial} instruction can refer to them
            ''' in special circumstances.  Use <seealso cref="#findConstructor findConstructor"/>
            ''' to access instance initialization methods in a safe manner.)</em>
            ''' <p><b>Example:</b>
            ''' <blockquote><pre>{@code
            ''' import static java.lang.invoke.MethodHandles.*;
            ''' import static java.lang.invoke.MethodType.*;
            ''' ...
            ''' static class Listie extends ArrayList {
            ''' public String toString() { return "[wee Listie]"; }
            ''' static Lookup lookup() { return MethodHandles.lookup(); }
            ''' }
            ''' ...
            ''' // no access to constructor via invokeSpecial:
            ''' MethodHandle MH_newListie = Listie.lookup()
            ''' .findConstructor(Listie.class, methodType(void.class));
            ''' Listie l = (Listie) MH_newListie.invokeExact();
            ''' try { assertEquals("impossible", Listie.lookup().findSpecial(
            ''' Listie.class, "<init>", methodType(void.class), Listie.class));
            ''' } catch (NoSuchMethodException ex) { } // OK
            ''' // access to super and self methods via invokeSpecial:
            ''' MethodHandle MH_super = Listie.lookup().findSpecial(
            ''' ArrayList.class, "toString" , methodType(String.class), Listie.class);
            ''' MethodHandle MH_this = Listie.lookup().findSpecial(
            ''' Listie.class, "toString" , methodType(String.class), Listie.class);
            ''' MethodHandle MH_duper = Listie.lookup().findSpecial(
            ''' Object.class, "toString" , methodType(String.class), Listie.class);
            ''' assertEquals("[]", (String) MH_super.invokeExact(l));
            ''' assertEquals(""+l, (String) MH_this.invokeExact(l));
            ''' assertEquals("[]", (String) MH_duper.invokeExact(l)); // ArrayList method
            ''' try { assertEquals("inaccessible", Listie.lookup().findSpecial(
            ''' String.class, "toString", methodType(String.class), Listie.class));
            ''' } catch (IllegalAccessException ex) { } // OK
            ''' Listie subl = new Listie() { public String toString() { return "[subclass]"; } };
            ''' assertEquals(""+l, (String) MH_this.invokeExact(subl)); // Listie method
            ''' }</pre></blockquote>
            ''' </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="name"> the name of the method (which must not be "&lt;init&gt;") </param>
            ''' <param name="type"> the type of the method, with the receiver argument omitted </param>
            ''' <param name="specialCaller"> the proposed calling class to perform the {@code invokespecial} </param>
            ''' <returns> the desired method handle </returns>
            ''' <exception cref="NoSuchMethodException"> if the method does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findSpecial(ByVal refc As [Class], ByVal name As String, ByVal type As MethodType, ByVal specialCaller As [Class]) As MethodHandle
                checkSpecialCaller(specialCaller)
                Dim specialLookup As Lookup = Me.in(specialCaller)
                Dim method As MemberName = specialLookup.resolveOrFail(REF_invokeSpecial, refc, name, type)
                Return specialLookup.getDirectMethod(REF_invokeSpecial, refc, method, findBoundCallerClass(method))
            End Function

            ''' <summary>
            ''' Produces a method handle giving read access to a non-static field.
            ''' The type of the method handle will have a return type of the field's
            ''' value type.
            ''' The method handle's single argument will be the instance containing
            ''' the field.
            ''' Access checking is performed immediately on behalf of the lookup class. </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="name"> the field's name </param>
            ''' <param name="type"> the field's type </param>
            ''' <returns> a method handle which can load values from the field </returns>
            ''' <exception cref="NoSuchFieldException"> if the field does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails, or if the field is {@code static} </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findGetter(ByVal refc As [Class], ByVal name As String, ByVal type As [Class]) As MethodHandle
                Dim field As MemberName = resolveOrFail(REF_getField, refc, name, type)
                Return getDirectField(REF_getField, refc, field)
            End Function

            ''' <summary>
            ''' Produces a method handle giving write access to a non-static field.
            ''' The type of the method handle will have a void return type.
            ''' The method handle will take two arguments, the instance containing
            ''' the field, and the value to be stored.
            ''' The second argument will be of the field's value type.
            ''' Access checking is performed immediately on behalf of the lookup class. </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="name"> the field's name </param>
            ''' <param name="type"> the field's type </param>
            ''' <returns> a method handle which can store values into the field </returns>
            ''' <exception cref="NoSuchFieldException"> if the field does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails, or if the field is {@code static} </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findSetter(ByVal refc As [Class], ByVal name As String, ByVal type As [Class]) As MethodHandle
                Dim field As MemberName = resolveOrFail(REF_putField, refc, name, type)
                Return getDirectField(REF_putField, refc, field)
            End Function

            ''' <summary>
            ''' Produces a method handle giving read access to a static field.
            ''' The type of the method handle will have a return type of the field's
            ''' value type.
            ''' The method handle will take no arguments.
            ''' Access checking is performed immediately on behalf of the lookup class.
            ''' <p>
            ''' If the returned method handle is invoked, the field's class will
            ''' be initialized, if it has not already been initialized. </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="name"> the field's name </param>
            ''' <param name="type"> the field's type </param>
            ''' <returns> a method handle which can load values from the field </returns>
            ''' <exception cref="NoSuchFieldException"> if the field does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails, or if the field is not {@code static} </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findStaticGetter(ByVal refc As [Class], ByVal name As String, ByVal type As [Class]) As MethodHandle
                Dim field As MemberName = resolveOrFail(REF_getStatic, refc, name, type)
                Return getDirectField(REF_getStatic, refc, field)
            End Function

            ''' <summary>
            ''' Produces a method handle giving write access to a static field.
            ''' The type of the method handle will have a void return type.
            ''' The method handle will take a single
            ''' argument, of the field's value type, the value to be stored.
            ''' Access checking is performed immediately on behalf of the lookup class.
            ''' <p>
            ''' If the returned method handle is invoked, the field's class will
            ''' be initialized, if it has not already been initialized. </summary>
            ''' <param name="refc"> the class or interface from which the method is accessed </param>
            ''' <param name="name"> the field's name </param>
            ''' <param name="type"> the field's type </param>
            ''' <returns> a method handle which can store values into the field </returns>
            ''' <exception cref="NoSuchFieldException"> if the field does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails, or if the field is not {@code static} </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function findStaticSetter(ByVal refc As [Class], ByVal name As String, ByVal type As [Class]) As MethodHandle
                Dim field As MemberName = resolveOrFail(REF_putStatic, refc, name, type)
                Return getDirectField(REF_putStatic, refc, field)
            End Function

            ''' <summary>
            ''' Produces an early-bound method handle for a non-static method.
            ''' The receiver must have a supertype {@code defc} in which a method
            ''' of the given name and type is accessible to the lookup class.
            ''' The method and all its argument types must be accessible to the lookup object.
            ''' The type of the method handle will be that of the method,
            ''' without any insertion of an additional receiver parameter.
            ''' The given receiver will be bound into the method handle,
            ''' so that every call to the method handle will invoke the
            ''' requested method on the given receiver.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the method's variable arity modifier bit ({@code 0x0080}) is set
            ''' <em>and</em> the trailing array argument is not the only argument.
            ''' (If the trailing array argument is the only argument,
            ''' the given receiver value will be bound to it.)
            ''' <p>
            ''' This is equivalent to the following code:
            ''' <blockquote><pre>{@code
            ''' import static java.lang.invoke.MethodHandles.*;
            ''' import static java.lang.invoke.MethodType.*;
            ''' ...
            ''' MethodHandle mh0 = lookup().findVirtual(defc, name, type);
            ''' MethodHandle mh1 = mh0.bindTo(receiver);
            ''' MethodType mt1 = mh1.type();
            ''' if (mh0.isVarargsCollector())
            ''' mh1 = mh1.asVarargsCollector(mt1.parameterType(mt1.parameterCount()-1));
            ''' return mh1;
            ''' }</pre></blockquote>
            ''' where {@code defc} is either {@code receiver.getClass()} or a super
            ''' type of that [Class], in which the requested method is accessible
            ''' to the lookup class.
            ''' (Note that {@code bindTo} does not preserve variable arity.) </summary>
            ''' <param name="receiver"> the object from which the method is accessed </param>
            ''' <param name="name"> the name of the method </param>
            ''' <param name="type"> the type of the method, with the receiver argument omitted </param>
            ''' <returns> the desired method handle </returns>
            ''' <exception cref="NoSuchMethodException"> if the method does not exist </exception>
            ''' <exception cref="IllegalAccessException"> if access checking fails
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            ''' <seealso cref= MethodHandle#bindTo </seealso>
            ''' <seealso cref= #findVirtual </seealso>
            Public Function bind(ByVal receiver As Object, ByVal name As String, ByVal type As MethodType) As MethodHandle
                Dim refc As [Class] = receiver.GetType() ' may get NPE
                Dim method As MemberName = resolveOrFail(REF_invokeSpecial, refc, name, type)
                Dim mh As MethodHandle = getDirectMethodNoRestrict(REF_invokeSpecial, refc, method, findBoundCallerClass(method))
                Return mh.bindArgumentL(0, receiver).varargsrgs(method)
            End Function

            ''' <summary>
            ''' Makes a <a href="MethodHandleInfo.html#directmh">direct method handle</a>
            ''' to <i>m</i>, if the lookup class has permission.
            ''' If <i>m</i> is non-static, the receiver argument is treated as an initial argument.
            ''' If <i>m</i> is virtual, overriding is respected on every call.
            ''' Unlike the Core Reflection API, exceptions are <em>not</em> wrapped.
            ''' The type of the method handle will be that of the method,
            ''' with the receiver type prepended (but only if it is non-static).
            ''' If the method's {@code accessible} flag is not set,
            ''' access checking is performed immediately on behalf of the lookup class.
            ''' If <i>m</i> is not public, do not share the resulting handle with untrusted parties.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the method's variable arity modifier bit ({@code 0x0080}) is set.
            ''' <p>
            ''' If <i>m</i> is static, and
            ''' if the returned method handle is invoked, the method's class will
            ''' be initialized, if it has not already been initialized. </summary>
            ''' <param name="m"> the reflected method </param>
            ''' <returns> a method handle which can invoke the reflected method </returns>
            ''' <exception cref="IllegalAccessException"> if access checking fails
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="NullPointerException"> if the argument is null </exception>
            Public Function unreflect(ByVal m As Method) As MethodHandle
                If m.declaringClass Is GetType(MethodHandle) Then
                    Dim mh As MethodHandle = unreflectForMH(m)
                    If mh IsNot Nothing Then Return mh
                End If
                Dim method As New MemberName(m)
                Dim refKind As SByte = method.referenceKind
                If refKind = REF_invokeSpecial Then refKind = REF_invokeVirtual
                Assert(method.method)
                Dim lookup_Renamed As Lookup = If(m.accessible, IMPL_LOOKUP, Me)
                Return lookup_Renamed.getDirectMethodNoSecurityManager(refKind, method.declaringClass, method, findBoundCallerClass(method))
            End Function
            Private Function unreflectForMH(ByVal m As Method) As MethodHandle
                ' these names require special lookups because they throw UnsupportedOperationException
                If MemberName.isMethodHandleInvokeName(m.name) Then Return MethodHandleImpl.fakeMethodHandleInvoke(New MemberName(m))
                Return Nothing
            End Function

            ''' <summary>
            ''' Produces a method handle for a reflected method.
            ''' It will bypass checks for overriding methods on the receiver,
            ''' <a href="MethodHandles.Lookup.html#equiv">as if called</a> from an {@code invokespecial}
            ''' instruction from within the explicitly specified {@code specialCaller}.
            ''' The type of the method handle will be that of the method,
            ''' with a suitably restricted receiver type prepended.
            ''' (The receiver type will be {@code specialCaller} or a subtype.)
            ''' If the method's {@code accessible} flag is not set,
            ''' access checking is performed immediately on behalf of the lookup [Class],
            ''' as if {@code invokespecial} instruction were being linked.
            ''' <p>
            ''' Before method resolution,
            ''' if the explicitly specified caller class is not identical with the
            ''' lookup [Class], or if this lookup object does not have
            ''' <a href="MethodHandles.Lookup.html#privacc">private access</a>
            ''' privileges, the access fails.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the method's variable arity modifier bit ({@code 0x0080}) is set. </summary>
            ''' <param name="m"> the reflected method </param>
            ''' <param name="specialCaller"> the class nominally calling the method </param>
            ''' <returns> a method handle which can invoke the reflected method </returns>
            ''' <exception cref="IllegalAccessException"> if access checking fails
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="NullPointerException"> if any argument is null </exception>
            Public Function unreflectSpecial(ByVal m As Method, ByVal specialCaller As [Class]) As MethodHandle
                checkSpecialCaller(specialCaller)
                Dim specialLookup As Lookup = Me.in(specialCaller)
                Dim method As New MemberName(m, True)
                Assert(method.method)
                ' ignore m.isAccessible:  this is a new kind of access
                Return specialLookup.getDirectMethodNoSecurityManager(REF_invokeSpecial, method.declaringClass, method, findBoundCallerClass(method))
            End Function

            ''' <summary>
            ''' Produces a method handle for a reflected constructor.
            ''' The type of the method handle will be that of the constructor,
            ''' with the return type changed to the declaring class.
            ''' The method handle will perform a {@code newInstance} operation,
            ''' creating a new instance of the constructor's class on the
            ''' arguments passed to the method handle.
            ''' <p>
            ''' If the constructor's {@code accessible} flag is not set,
            ''' access checking is performed immediately on behalf of the lookup class.
            ''' <p>
            ''' The returned method handle will have
            ''' <seealso cref="MethodHandle#asVarargsCollector variable arity"/> if and only if
            ''' the constructor's variable arity modifier bit ({@code 0x0080}) is set.
            ''' <p>
            ''' If the returned method handle is invoked, the constructor's class will
            ''' be initialized, if it has not already been initialized. </summary>
            ''' <param name="c"> the reflected constructor </param>
            ''' <returns> a method handle which can invoke the reflected constructor </returns>
            ''' <exception cref="IllegalAccessException"> if access checking fails
            '''                                or if the method's variable arity modifier bit
            '''                                is set and {@code asVarargsCollector} fails </exception>
            ''' <exception cref="NullPointerException"> if the argument is null </exception>
            Public Function unreflectConstructor(Of T1)(ByVal c As Constructor(Of T1)) As MethodHandle
                Dim ctor As New MemberName(c)
                Assert(ctor.constructor)
                Dim lookup_Renamed As Lookup = If(c.accessible, IMPL_LOOKUP, Me)
                Return lookup_Renamed.getDirectConstructorNoSecurityManager(ctor.declaringClass, ctor)
            End Function

            ''' <summary>
            ''' Produces a method handle giving read access to a reflected field.
            ''' The type of the method handle will have a return type of the field's
            ''' value type.
            ''' If the field is static, the method handle will take no arguments.
            ''' Otherwise, its single argument will be the instance containing
            ''' the field.
            ''' If the field's {@code accessible} flag is not set,
            ''' access checking is performed immediately on behalf of the lookup class.
            ''' <p>
            ''' If the field is static, and
            ''' if the returned method handle is invoked, the field's class will
            ''' be initialized, if it has not already been initialized. </summary>
            ''' <param name="f"> the reflected field </param>
            ''' <returns> a method handle which can load values from the reflected field </returns>
            ''' <exception cref="IllegalAccessException"> if access checking fails </exception>
            ''' <exception cref="NullPointerException"> if the argument is null </exception>
            Public Function unreflectGetter(ByVal f As Field) As MethodHandle
                Return unreflectField(f, False)
            End Function
            Private Function unreflectField(ByVal f As Field, ByVal isSetter As Boolean) As MethodHandle
                Dim field As New MemberName(f, isSetter)
                Assert(If(isSetter, MethodHandleNatives.refKindIsSetter(field.referenceKind), MethodHandleNatives.refKindIsGetter(field.referenceKind)))
                Dim lookup_Renamed As Lookup = If(f.accessible, IMPL_LOOKUP, Me)
                Return lookup_Renamed.getDirectFieldNoSecurityManager(field.referenceKind, f.declaringClass, field)
            End Function

            ''' <summary>
            ''' Produces a method handle giving write access to a reflected field.
            ''' The type of the method handle will have a void return type.
            ''' If the field is static, the method handle will take a single
            ''' argument, of the field's value type, the value to be stored.
            ''' Otherwise, the two arguments will be the instance containing
            ''' the field, and the value to be stored.
            ''' If the field's {@code accessible} flag is not set,
            ''' access checking is performed immediately on behalf of the lookup class.
            ''' <p>
            ''' If the field is static, and
            ''' if the returned method handle is invoked, the field's class will
            ''' be initialized, if it has not already been initialized. </summary>
            ''' <param name="f"> the reflected field </param>
            ''' <returns> a method handle which can store values into the reflected field </returns>
            ''' <exception cref="IllegalAccessException"> if access checking fails </exception>
            ''' <exception cref="NullPointerException"> if the argument is null </exception>
            Public Function unreflectSetter(ByVal f As Field) As MethodHandle
                Return unreflectField(f, True)
            End Function

            ''' <summary>
            ''' Cracks a <a href="MethodHandleInfo.html#directmh">direct method handle</a>
            ''' created by this lookup object or a similar one.
            ''' Security and access checks are performed to ensure that this lookup object
            ''' is capable of reproducing the target method handle.
            ''' This means that the cracking may fail if target is a direct method handle
            ''' but was created by an unrelated lookup object.
            ''' This can happen if the method handle is <a href="MethodHandles.Lookup.html#callsens">caller sensitive</a>
            ''' and was created by a lookup object for a different class. </summary>
            ''' <param name="target"> a direct method handle to crack into symbolic reference components </param>
            ''' <returns> a symbolic reference which can be used to reconstruct this method handle from this lookup object </returns>
            ''' <exception cref="SecurityException"> if a security manager is present and it
            '''                              <a href="MethodHandles.Lookup.html#secmgr">refuses access</a> </exception>
            ''' <exception cref="IllegalArgumentException"> if the target is not a direct method handle or if access checking fails </exception>
            ''' <exception cref="NullPointerException"> if the target is {@code null} </exception>
            ''' <seealso cref= MethodHandleInfo
            ''' @since 1.8 </seealso>
            Public Function revealDirect(ByVal target As MethodHandle) As MethodHandleInfo
                Dim member As MemberName = target.internalMemberName()
                If member Is Nothing OrElse ((Not member.resolved) AndAlso (Not member.methodHandleInvoke)) Then Throw newIllegalArgumentException("not a direct method handle")
                Dim defc As [Class] = member.declaringClass
                Dim refKind As SByte = member.referenceKind
                Assert(MethodHandleNatives.refKindIsValid(refKind))
                If refKind = REF_invokeSpecial AndAlso (Not target.invokeSpecial) Then refKind = REF_invokeVirtual
                If refKind = REF_invokeVirtual AndAlso defc.interface Then refKind = REF_invokeInterface
                ' Check SM permissions and member access before cracking.
                Try
                    checkAccess(refKind, defc, member)
                    checkSecurityManager(defc, member)
                Catch ex As IllegalAccessException
                    Throw New IllegalArgumentException(ex)
                End Try
                If allowedModes <> TRUSTED AndAlso member.callerSensitive Then
                    Dim callerClass As [Class] = target.internalCallerClass()
                    If (Not hasPrivateAccess()) OrElse callerClass IsNot lookupClass() Then Throw New IllegalArgumentException("method handle is caller sensitive: " & callerClass)
                End If
                ' Produce the handle to the results.
                Return New InfoFromMemberName(Me, member, refKind)
            End Function

            '/ Helper methods, all package-private.

            Friend Function resolveOrFail(ByVal refKind As SByte, ByVal refc As [Class], ByVal name As String, ByVal type As [Class]) As MemberName
                checkSymbolicClass(refc) ' do this before attempting to resolve
                name.GetType() ' NPE
                type.GetType() ' NPE
                Return IMPL_NAMES.resolveOrFail(refKind, New MemberName(refc, name, type, refKind), lookupClassOrNull(), GetType(NoSuchFieldException))
            End Function

            Friend Function resolveOrFail(ByVal refKind As SByte, ByVal refc As [Class], ByVal name As String, ByVal type As MethodType) As MemberName
                checkSymbolicClass(refc) ' do this before attempting to resolve
                name.GetType() ' NPE
                type.GetType() ' NPE
                checkMethodName(refKind, name) ' NPE check on name
                Return IMPL_NAMES.resolveOrFail(refKind, New MemberName(refc, name, type, refKind), lookupClassOrNull(), GetType(NoSuchMethodException))
            End Function

            Friend Function resolveOrFail(ByVal refKind As SByte, ByVal member As MemberName) As MemberName
                checkSymbolicClass(member.declaringClass) ' do this before attempting to resolve
                member.name.GetType() ' NPE
                member.type.GetType() ' NPE
                Return IMPL_NAMES.resolveOrFail(refKind, member, lookupClassOrNull(), GetType(ReflectiveOperationException))
            End Function

            Friend Sub checkSymbolicClass(ByVal refc As [Class])
                refc.GetType() ' NPE
                Dim caller As [Class] = lookupClassOrNull()
                If caller IsNot Nothing AndAlso (Not sun.invoke.util.VerifyAccess.isClassAccessible(refc, caller, allowedModes)) Then Throw (New MemberName(refc)).makeAccessException("symbolic reference class is not public", Me)
            End Sub

            ''' <summary>
            ''' Check name for an illegal leading "&lt;" character. </summary>
            Friend Sub checkMethodName(ByVal refKind As SByte, ByVal name As String)
                If name.StartsWith("<") AndAlso refKind <> REF_newInvokeSpecial Then Throw New NoSuchMethodException("illegal method name: " & name)
            End Sub


            ''' <summary>
            ''' Find my trustable caller class if m is a caller sensitive method.
            ''' If this lookup object has private access, then the caller class is the lookupClass.
            ''' Otherwise, if m is caller-sensitive, throw IllegalAccessException.
            ''' </summary>
            Friend Function findBoundCallerClass(ByVal m As MemberName) As [Class]
                Dim callerClass As [Class] = Nothing
                If MethodHandleNatives.isCallerSensitive(m) Then
                    ' Only lookups with private access are allowed to resolve caller-sensitive methods
                    If hasPrivateAccess() Then
                        callerClass = lookupClass_Renamed
                    Else
                        Throw New IllegalAccessException("Attempt to lookup caller-sensitive method using restricted lookup object")
                    End If
                End If
                Return callerClass
            End Function

            Private Function hasPrivateAccess() As Boolean
                Return (allowedModes And [PRIVATE]) <> 0
            End Function

            ''' <summary>
            ''' Perform necessary <a href="MethodHandles.Lookup.html#secmgr">access checks</a>.
            ''' Determines a trustable caller class to compare with refc, the symbolic reference class.
            ''' If this lookup object has private access, then the caller class is the lookupClass.
            ''' </summary>
            Friend Sub checkSecurityManager(ByVal refc As [Class], ByVal m As MemberName)
                Dim smgr As SecurityManager = System.securityManager
                If smgr Is Nothing Then Return
                If allowedModes = TRUSTED Then Return

                ' Step 1:
                Dim fullPowerLookup As Boolean = hasPrivateAccess()
                If (Not fullPowerLookup) OrElse (Not sun.invoke.util.VerifyAccess.classLoaderIsAncestor(lookupClass_Renamed, refc)) Then sun.reflect.misc.ReflectUtil.checkPackageAccess(refc)

                ' Step 2:
                If m.public Then Return
                If Not fullPowerLookup Then smgr.checkPermission(sun.security.util.SecurityConstants.CHECK_MEMBER_ACCESS_PERMISSION)

                ' Step 3:
                Dim defc As [Class] = m.declaringClass
                If (Not fullPowerLookup) AndAlso defc IsNot refc Then sun.reflect.misc.ReflectUtil.checkPackageAccess(defc)
            End Sub

            Friend Sub checkMethod(ByVal refKind As SByte, ByVal refc As [Class], ByVal m As MemberName)
                Dim wantStatic As Boolean = (refKind = REF_invokeStatic)
                Dim message As String
                If m.constructor Then
                    message = "expected a method, not a constructor"
                ElseIf Not m.method Then
                    message = "expected a method"
                ElseIf wantStatic <> m.static Then
                    message = If(wantStatic, "expected a static method", "expected a non-static method")
                Else
                    checkAccess(refKind, refc, m)
                    Return
                End If
                Throw m.makeAccessException(message, Me)
            End Sub

            Friend Sub checkField(ByVal refKind As SByte, ByVal refc As [Class], ByVal m As MemberName)
                Dim wantStatic As Boolean = Not MethodHandleNatives.refKindHasReceiver(refKind)
                Dim message As String
                If wantStatic <> m.static Then
                    message = If(wantStatic, "expected a static field", "expected a non-static field")
                Else
                    checkAccess(refKind, refc, m)
                    Return
                End If
                Throw m.makeAccessException(message, Me)
            End Sub

            ''' <summary>
            ''' Check public/protected/private bits on the symbolic reference class and its member. </summary>
            Friend Sub checkAccess(ByVal refKind As SByte, ByVal refc As [Class], ByVal m As MemberName)
                Assert(m.referenceKindIsConsistentWith(refKind) AndAlso MethodHandleNatives.refKindIsValid(refKind) AndAlso (MethodHandleNatives.refKindIsField(refKind) = m.field))
                Dim allowedModes As Integer = Me.allowedModes
                If allowedModes = TRUSTED Then Return
                Dim mods As Integer = m.modifiers
                If Modifier.isProtected(mods) AndAlso refKind = REF_invokeVirtual AndAlso m.declaringClass Is GetType(Object) AndAlso m.name.Equals("clone") AndAlso refc.array Then mods = mods Xor Modifier.PROTECTED Or Modifier.PUBLIC
                If Modifier.isProtected(mods) AndAlso refKind = REF_newInvokeSpecial Then mods = mods Xor Modifier.PROTECTED
                If Modifier.isFinal(mods) AndAlso MethodHandleNatives.refKindIsSetter(refKind) Then Throw m.makeAccessException("unexpected set of a final field", Me)
                If Modifier.isPublic(mods) AndAlso Modifier.isPublic(refc.modifiers) AndAlso allowedModes <> 0 Then Return ' common case
                Dim requestedModes As Integer = fixmods(mods) ' adjust 0 => PACKAGE
                If (requestedModes And allowedModes) <> 0 Then
                    If sun.invoke.util.VerifyAccess.isMemberAccessible(refc, m.declaringClass, mods, lookupClass(), allowedModes) Then Return
                Else
                    ' Protected members can also be checked as if they were package-private.
                    If (requestedModes And [PROTECTED]) <> 0 AndAlso (allowedModes And PACKAGE) <> 0 AndAlso sun.invoke.util.VerifyAccess.isSamePackage(m.declaringClass, lookupClass()) Then Return
                End If
                Throw m.makeAccessException(accessFailedMessage(refc, m), Me)
            End Sub

            Friend Function accessFailedMessage(ByVal refc As [Class], ByVal m As MemberName) As String
                Dim defc As [Class] = m.declaringClass
                Dim mods As Integer = m.modifiers
                ' check the class first:
                Dim classOK As Boolean = (Modifier.isPublic(defc.modifiers) AndAlso (defc Is refc OrElse Modifier.isPublic(refc.modifiers)))
                If (Not classOK) AndAlso (allowedModes And PACKAGE) <> 0 Then classOK = (sun.invoke.util.VerifyAccess.isClassAccessible(defc, lookupClass(), ALL_MODES) AndAlso (defc Is refc OrElse sun.invoke.util.VerifyAccess.isClassAccessible(refc, lookupClass(), ALL_MODES)))
                If Not classOK Then Return "class is not public"
                If Modifier.isPublic(mods) Then Return "access to public member failed" ' (how?)
                If Modifier.isPrivate(mods) Then Return "member is private"
                If Modifier.isProtected(mods) Then Return "member is protected"
                Return "member is private to package"
            End Function

            Private Const ALLOW_NESTMATE_ACCESS As Boolean = False

            Private Sub checkSpecialCaller(ByVal specialCaller As [Class])
                Dim allowedModes As Integer = Me.allowedModes
                If allowedModes = TRUSTED Then Return
                If (Not hasPrivateAccess()) OrElse (specialCaller IsNot lookupClass() AndAlso Not (ALLOW_NESTMATE_ACCESS AndAlso sun.invoke.util.VerifyAccess.isSamePackageMember(specialCaller, lookupClass()))) Then Throw (New MemberName(specialCaller)).makeAccessException("no private access for invokespecial", Me)
            End Sub

            Private Function restrictProtectedReceiver(ByVal method As MemberName) As Boolean
                ' The accessing class only has the right to use a protected member
                ' on itself or a subclass.  Enforce that restriction, from JVMS 5.4.4, etc.
                If (Not method.protected) OrElse method.static OrElse allowedModes = TRUSTED OrElse method.declaringClass Is lookupClass() OrElse sun.invoke.util.VerifyAccess.isSamePackage(method.declaringClass, lookupClass()) OrElse (ALLOW_NESTMATE_ACCESS AndAlso sun.invoke.util.VerifyAccess.isSamePackageMember(method.declaringClass, lookupClass())) Then Return False
                Return True
            End Function
            Private Function restrictReceiver(ByVal method As MemberName, ByVal mh As DirectMethodHandle, ByVal caller As [Class]) As MethodHandle
                Assert((Not method.static))
                ' receiver type of mh is too wide; narrow to caller
                If Not caller.IsSubclassOf(method.declaringClass) Then Throw method.makeAccessException("caller class must be a subclass below the method", caller)
                Dim rawType As MethodType = mh.type()
                If rawType.parameterType(0) Is caller Then Return mh
                Dim narrowType As MethodType = rawType.changeParameterType(0, caller)
                Assert((Not mh.varargsCollector)) ' viewAsType will lose varargs-ness
                Assert(mh.viewAsTypeChecks(narrowType, True))
                Return mh.copyWith(narrowType, mh.form)
            End Function

            ''' <summary>
            ''' Check access and get the requested method. </summary>
            Private Function getDirectMethod(ByVal refKind As SByte, ByVal refc As [Class], ByVal method As MemberName, ByVal callerClass As [Class]) As MethodHandle
                Const doRestrict As Boolean = True
                Const checkSecurity As Boolean = True
                Return getDirectMethodCommon(refKind, refc, method, checkSecurity, doRestrict, callerClass)
            End Function
            ''' <summary>
            ''' Check access and get the requested method, eliding receiver narrowing rules. </summary>
            Private Function getDirectMethodNoRestrict(ByVal refKind As SByte, ByVal refc As [Class], ByVal method As MemberName, ByVal callerClass As [Class]) As MethodHandle
                Const doRestrict As Boolean = False
                Const checkSecurity As Boolean = True
                Return getDirectMethodCommon(refKind, refc, method, checkSecurity, doRestrict, callerClass)
            End Function
            ''' <summary>
            ''' Check access and get the requested method, eliding security manager checks. </summary>
            Private Function getDirectMethodNoSecurityManager(ByVal refKind As SByte, ByVal refc As [Class], ByVal method As MemberName, ByVal callerClass As [Class]) As MethodHandle
                Const doRestrict As Boolean = True
                Const checkSecurity As Boolean = False ' not needed for reflection or for linking CONSTANT_MH constants
                Return getDirectMethodCommon(refKind, refc, method, checkSecurity, doRestrict, callerClass)
            End Function
            ''' <summary>
            ''' Common code for all methods; do not call directly except from immediately above. </summary>
            Private Function getDirectMethodCommon(ByVal refKind As SByte, ByVal refc As [Class], ByVal method As MemberName, ByVal checkSecurity As Boolean, ByVal doRestrict As Boolean, ByVal callerClass As [Class]) As MethodHandle
                checkMethod(refKind, refc, method)
                ' Optionally check with the security manager; this isn't needed for unreflect* calls.
                If checkSecurity Then checkSecurityManager(refc, method)
                Assert((Not method.methodHandleInvoke))

                If refKind = REF_invokeSpecial AndAlso refc IsNot lookupClass() AndAlso (Not refc.interface) AndAlso refc IsNot lookupClass().BaseType AndAlso lookupClass().IsSubclassOf(refc) Then
                    Assert((Not method.name.Equals("<init>"))) ' not this code path
                    ' Per JVMS 6.5, desc. of invokespecial instruction:
                    ' If the method is in a superclass of the LC,
                    ' and if our original search was above LC.super,
                    ' repeat the search (symbolic lookup) from LC.super
                    ' and continue with the direct superclass of that [Class],
                    ' and so forth, until a match is found or no further superclasses exist.
                    ' FIXME: MemberName.resolve should handle this instead.
                    Dim refcAsSuper As [Class] = lookupClass()
                    Dim m2 As MemberName
                    Do
                        refcAsSuper = refcAsSuper.BaseType
                        m2 = New MemberName(refcAsSuper, method.name, method.methodType, REF_invokeSpecial)
                        m2 = IMPL_NAMES.resolveOrNull(refKind, m2, lookupClassOrNull())
                    Loop While m2 Is Nothing AndAlso refc IsNot refcAsSuper ' search up to refc -  no method is found yet
                    If m2 Is Nothing Then Throw New InternalError(method.ToString())
                    method = m2
                    refc = refcAsSuper
                    ' redo basic checks
                    checkMethod(refKind, refc, method)
                End If

                Dim dmh As DirectMethodHandle = DirectMethodHandle.make(refKind, refc, method)
                Dim mh As MethodHandle = dmh
                ' Optionally narrow the receiver argument to refc using restrictReceiver.
                If doRestrict AndAlso (refKind = REF_invokeSpecial OrElse (MethodHandleNatives.refKindHasReceiver(refKind) AndAlso restrictProtectedReceiver(method))) Then mh = restrictReceiver(method, dmh, lookupClass())
                mh = maybeBindCaller(method, mh, callerClass)
                mh = mh.varargsrgs(method)
                Return mh
            End Function
            Private Function maybeBindCaller(ByVal method As MemberName, ByVal mh As MethodHandle, ByVal callerClass As [Class]) As MethodHandle
                If allowedModes = TRUSTED OrElse (Not MethodHandleNatives.isCallerSensitive(method)) Then Return mh
                Dim hostClass As [Class] = lookupClass_Renamed
                If Not hasPrivateAccess() Then ' caller must have private access hostClass = callerClass ' callerClass came from a security manager style stack walk
                    Dim cbmh As MethodHandle = MethodHandleImpl.bindCaller(mh, hostClass)
                    ' Note: caller will apply varargs after this step happens.
                    Return cbmh
            End Function
            ''' <summary>
            ''' Check access and get the requested field. </summary>
            Private Function getDirectField(ByVal refKind As SByte, ByVal refc As [Class], ByVal field As MemberName) As MethodHandle
                Const checkSecurity As Boolean = True
                Return getDirectFieldCommon(refKind, refc, field, checkSecurity)
            End Function
            ''' <summary>
            ''' Check access and get the requested field, eliding security manager checks. </summary>
            Private Function getDirectFieldNoSecurityManager(ByVal refKind As SByte, ByVal refc As [Class], ByVal field As MemberName) As MethodHandle
                Const checkSecurity As Boolean = False ' not needed for reflection or for linking CONSTANT_MH constants
                Return getDirectFieldCommon(refKind, refc, field, checkSecurity)
            End Function
            ''' <summary>
            ''' Common code for all fields; do not call directly except from immediately above. </summary>
            Private Function getDirectFieldCommon(ByVal refKind As SByte, ByVal refc As [Class], ByVal field As MemberName, ByVal checkSecurity As Boolean) As MethodHandle
                checkField(refKind, refc, field)
                ' Optionally check with the security manager; this isn't needed for unreflect* calls.
                If checkSecurity Then checkSecurityManager(refc, field)
                Dim dmh As DirectMethodHandle = DirectMethodHandle.make(refc, field)
                Dim doRestrict As Boolean = (MethodHandleNatives.refKindHasReceiver(refKind) AndAlso restrictProtectedReceiver(field))
                If doRestrict Then Return restrictReceiver(field, dmh, lookupClass())
                Return dmh
            End Function
            ''' <summary>
            ''' Check access and get the requested constructor. </summary>
            Private Function getDirectConstructor(ByVal refc As [Class], ByVal ctor As MemberName) As MethodHandle
                Const checkSecurity As Boolean = True
                Return getDirectConstructorCommon(refc, ctor, checkSecurity)
            End Function
            ''' <summary>
            ''' Check access and get the requested constructor, eliding security manager checks. </summary>
            Private Function getDirectConstructorNoSecurityManager(ByVal refc As [Class], ByVal ctor As MemberName) As MethodHandle
                Const checkSecurity As Boolean = False ' not needed for reflection or for linking CONSTANT_MH constants
                Return getDirectConstructorCommon(refc, ctor, checkSecurity)
            End Function
            ''' <summary>
            ''' Common code for all constructors; do not call directly except from immediately above. </summary>
            Private Function getDirectConstructorCommon(ByVal refc As [Class], ByVal ctor As MemberName, ByVal checkSecurity As Boolean) As MethodHandle
                Assert(ctor.constructor)
                checkAccess(REF_newInvokeSpecial, refc, ctor)
                ' Optionally check with the security manager; this isn't needed for unreflect* calls.
                If checkSecurity Then checkSecurityManager(refc, ctor)
                Assert((Not MethodHandleNatives.isCallerSensitive(ctor))) ' maybeBindCaller not relevant here
                Return DirectMethodHandle.make(ctor).varargsrgs(ctor)
            End Function

            ''' <summary>
            ''' Hook called from the JVM (via MethodHandleNatives) to link MH constants:
            ''' </summary>
            'non-public
            Friend Function linkMethodHandleConstant(ByVal refKind As SByte, ByVal defc As [Class], ByVal name As String, ByVal type As Object) As MethodHandle
                If Not (TypeOf type Is Class OrElse TypeOf type Is MethodType) Then Throw New InternalError("unresolved MemberName")
                Dim member As New MemberName(refKind, defc, name, type)
                Dim mh As MethodHandle = LOOKASIDE_TABLE(member)
                If mh IsNot Nothing Then
                    checkSymbolicClass(defc)
                    Return mh
                End If
                ' Treat MethodHandle.invoke and invokeExact specially.
                If defc Is GetType(MethodHandle) AndAlso refKind = REF_invokeVirtual Then
                    mh = findVirtualForMH(member.name, member.methodType)
                    If mh IsNot Nothing Then Return mh
                End If
                Dim resolved As MemberName = resolveOrFail(refKind, member)
                mh = getDirectMethodForConstant(refKind, defc, resolved)
                If TypeOf mh Is DirectMethodHandle AndAlso canBeCached(refKind, defc, resolved) Then
                    Dim key As MemberName = mh.internalMemberName()
                    If key IsNot Nothing Then key = key.asNormalOriginal()
                    If member.Equals(key) Then ' better safe than sorry LOOKASIDE_TABLE(key) = CType(mh, DirectMethodHandle)
                    End If
                    Return mh
            End Function
            Private Function canBeCached(ByVal refKind As SByte, ByVal defc As [Class], ByVal member As MemberName) As Boolean
                If refKind = REF_invokeSpecial Then Return False
                If (Not Modifier.isPublic(defc.modifiers)) OrElse (Not Modifier.isPublic(member.declaringClass.modifiers)) OrElse (Not member.public) OrElse member.callerSensitive Then Return False
                Dim loader As ClassLoader = defc.classLoader
                If Not sun.misc.VM.isSystemDomainLoader(loader) Then
                    Dim sysl As ClassLoader = ClassLoader.systemClassLoader
                    Dim found As Boolean = False
                    Do While sysl IsNot Nothing
                        If loader Is sysl Then
                            found = True
                            Exit Do
                        End If
                        sysl = sysl.parent
                    Loop
                    If Not found Then Return False
                End If
                Try
                    Dim resolved2 As MemberName = publicLookup().resolveOrFail(refKind, New MemberName(refKind, defc, member.name, member.type))
                    checkSecurityManager(defc, resolved2)
                    'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
                Catch ReflectiveOperationException Or SecurityException ex
					Return False
                End Try
                Return True
            End Function
            Private Function getDirectMethodForConstant(ByVal refKind As SByte, ByVal defc As [Class], ByVal member As MemberName) As MethodHandle
                If MethodHandleNatives.refKindIsField(refKind) Then
                    Return getDirectFieldNoSecurityManager(refKind, defc, member)
                ElseIf MethodHandleNatives.refKindIsMethod(refKind) Then
                    Return getDirectMethodNoSecurityManager(refKind, defc, member, lookupClass_Renamed)
                ElseIf refKind = REF_newInvokeSpecial Then
                    Return getDirectConstructorNoSecurityManager(defc, member)
                End If
                ' oops
                Throw newIllegalArgumentException("bad MethodHandle constant #" & member)
            End Function

            Friend Shared LOOKASIDE_TABLE As New ConcurrentDictionary(Of MemberName, DirectMethodHandle)
        End Class

        ''' <summary>
        ''' Produces a method handle giving read access to elements of an array.
        ''' The type of the method handle will have a return type of the array's
        ''' element type.  Its first argument will be the array type,
        ''' and the second will be {@code int}. </summary>
        ''' <param name="arrayClass"> an array type </param>
        ''' <returns> a method handle which can load values from the given array type </returns>
        ''' <exception cref="NullPointerException"> if the argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if arrayClass is not an array type </exception>
        Public Shared Function arrayElementGetter(ByVal arrayClass As [Class]) As MethodHandle
            Return MethodHandleImpl.makeArrayElementAccessor(arrayClass, False)
        End Function

        ''' <summary>
        ''' Produces a method handle giving write access to elements of an array.
        ''' The type of the method handle will have a void return type.
        ''' Its last argument will be the array's element type.
        ''' The first and second arguments will be the array type and int. </summary>
        ''' <param name="arrayClass"> the class of an array </param>
        ''' <returns> a method handle which can store values into the array type </returns>
        ''' <exception cref="NullPointerException"> if the argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if arrayClass is not an array type </exception>
        Public Shared Function arrayElementSetter(ByVal arrayClass As [Class]) As MethodHandle
            Return MethodHandleImpl.makeArrayElementAccessor(arrayClass, True)
        End Function

        '/ method handle invocation (reflective style)

        ''' <summary>
        ''' Produces a method handle which will invoke any method handle of the
        ''' given {@code type}, with a given number of trailing arguments replaced by
        ''' a single trailing {@code Object[]} array.
        ''' The resulting invoker will be a method handle with the following
        ''' arguments:
        ''' <ul>
        ''' <li>a single {@code MethodHandle} target
        ''' <li>zero or more leading values (counted by {@code leadingArgCount})
        ''' <li>an {@code Object[]} array containing trailing arguments
        ''' </ul>
        ''' <p>
        ''' The invoker will invoke its target like a call to <seealso cref="MethodHandle#invoke invoke"/> with
        ''' the indicated {@code type}.
        ''' That is, if the target is exactly of the given {@code type}, it will behave
        ''' like {@code invokeExact}; otherwise it behave as if <seealso cref="MethodHandle#asType asType"/>
        ''' is used to convert the target to the required {@code type}.
        ''' <p>
        ''' The type of the returned invoker will not be the given {@code type}, but rather
        ''' will have all parameters except the first {@code leadingArgCount}
        ''' replaced by a single array of type {@code Object[]}, which will be
        ''' the final parameter.
        ''' <p>
        ''' Before invoking its target, the invoker will spread the final array, apply
        ''' reference casts as necessary, and unbox and widen primitive arguments.
        ''' If, when the invoker is called, the supplied array argument does
        ''' not have the correct number of elements, the invoker will throw
        ''' an <seealso cref="IllegalArgumentException"/> instead of invoking the target.
        ''' <p>
        ''' This method is equivalent to the following code (though it may be more efficient):
        ''' <blockquote><pre>{@code
        ''' MethodHandle invoker = MethodHandles.invoker(type);
        ''' int spreadArgCount = type.parameterCount() - leadingArgCount;
        ''' invoker = invoker.asSpreader(Object[].class, spreadArgCount);
        ''' return invoker;
        ''' }</pre></blockquote>
        ''' This method throws no reflective or security exceptions. </summary>
        ''' <param name="type"> the desired target type </param>
        ''' <param name="leadingArgCount"> number of fixed arguments, to be passed unchanged to the target </param>
        ''' <returns> a method handle suitable for invoking any method handle of the given type </returns>
        ''' <exception cref="NullPointerException"> if {@code type} is null </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code leadingArgCount} is not in
        '''                  the range from 0 to {@code type.parameterCount()} inclusive,
        '''                  or if the resulting method handle's type would have
        '''          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
        Public Shared Function spreadInvoker(ByVal type As MethodType, ByVal leadingArgCount As Integer) As MethodHandle
            If leadingArgCount < 0 OrElse leadingArgCount > type.parameterCount() Then Throw newIllegalArgumentException("bad argument count", leadingArgCount)
            type = type.asSpreaderType(GetType(Object()), type.parameterCount() - leadingArgCount)
            Return type.invokers().spreadInvoker(leadingArgCount)
        End Function

        ''' <summary>
        ''' Produces a special <em>invoker method handle</em> which can be used to
        ''' invoke any method handle of the given type, as if by <seealso cref="MethodHandle#invokeExact invokeExact"/>.
        ''' The resulting invoker will have a type which is
        ''' exactly equal to the desired type, except that it will accept
        ''' an additional leading argument of type {@code MethodHandle}.
        ''' <p>
        ''' This method is equivalent to the following code (though it may be more efficient):
        ''' {@code publicLookup().findVirtual(MethodHandle.class, "invokeExact", type)}
        ''' 
        ''' <p style="font-size:smaller;">
        ''' <em>Discussion:</em>
        ''' Invoker method handles can be useful when working with variable method handles
        ''' of unknown types.
        ''' For example, to emulate an {@code invokeExact} call to a variable method
        ''' handle {@code M}, extract its type {@code T},
        ''' look up the invoker method {@code X} for {@code T},
        ''' and call the invoker method, as {@code X.invoke(T, A...)}.
        ''' (It would not work to call {@code X.invokeExact}, since the type {@code T}
        ''' is unknown.)
        ''' If spreading, collecting, or other argument transformations are required,
        ''' they can be applied once to the invoker {@code X} and reused on many {@code M}
        ''' method handle values, as long as they are compatible with the type of {@code X}.
        ''' <p style="font-size:smaller;">
        ''' <em>(Note:  The invoker method is not available via the Core Reflection API.
        ''' An attempt to call <seealso cref="java.lang.reflect.Method#invoke java.lang.reflect.Method.invoke"/>
        ''' on the declared {@code invokeExact} or {@code invoke} method will raise an
        ''' <seealso cref="java.lang.UnsupportedOperationException UnsupportedOperationException"/>.)</em>
        ''' <p>
        ''' This method throws no reflective or security exceptions. </summary>
        ''' <param name="type"> the desired target type </param>
        ''' <returns> a method handle suitable for invoking any method handle of the given type </returns>
        ''' <exception cref="IllegalArgumentException"> if the resulting method handle's type would have
        '''          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
        Public Shared Function exactInvoker(ByVal type As MethodType) As MethodHandle
            Return type.invokers().exactInvoker()
        End Function

        ''' <summary>
        ''' Produces a special <em>invoker method handle</em> which can be used to
        ''' invoke any method handle compatible with the given type, as if by <seealso cref="MethodHandle#invoke invoke"/>.
        ''' The resulting invoker will have a type which is
        ''' exactly equal to the desired type, except that it will accept
        ''' an additional leading argument of type {@code MethodHandle}.
        ''' <p>
        ''' Before invoking its target, if the target differs from the expected type,
        ''' the invoker will apply reference casts as
        ''' necessary and box, unbox, or widen primitive values, as if by <seealso cref="MethodHandle#asType asType"/>.
        ''' Similarly, the return value will be converted as necessary.
        ''' If the target is a <seealso cref="MethodHandle#asVarargsCollector variable arity method handle"/>,
        ''' the required arity conversion will be made, again as if by <seealso cref="MethodHandle#asType asType"/>.
        ''' <p>
        ''' This method is equivalent to the following code (though it may be more efficient):
        ''' {@code publicLookup().findVirtual(MethodHandle.class, "invoke", type)}
        ''' <p style="font-size:smaller;">
        ''' <em>Discussion:</em>
        ''' A <seealso cref="MethodType#genericMethodType general method type"/> is one which
        ''' mentions only {@code Object} arguments and return values.
        ''' An invoker for such a type is capable of calling any method handle
        ''' of the same arity as the general type.
        ''' <p style="font-size:smaller;">
        ''' <em>(Note:  The invoker method is not available via the Core Reflection API.
        ''' An attempt to call <seealso cref="java.lang.reflect.Method#invoke java.lang.reflect.Method.invoke"/>
        ''' on the declared {@code invokeExact} or {@code invoke} method will raise an
        ''' <seealso cref="java.lang.UnsupportedOperationException UnsupportedOperationException"/>.)</em>
        ''' <p>
        ''' This method throws no reflective or security exceptions. </summary>
        ''' <param name="type"> the desired target type </param>
        ''' <returns> a method handle suitable for invoking any method handle convertible to the given type </returns>
        ''' <exception cref="IllegalArgumentException"> if the resulting method handle's type would have
        '''          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
        Public Shared Function invoker(ByVal type As MethodType) As MethodHandle
            Return type.invokers().genericInvoker()
        End Function

        Friend Shared Function basicInvoker(ByVal type As MethodType) As MethodHandle 'non-public
            Return type.invokers().basicInvoker()
        End Function

        '/ method handle modification (creation from other method handles)

        ''' <summary>
        ''' Produces a method handle which adapts the type of the
        ''' given method handle to a new type by pairwise argument and return type conversion.
        ''' The original type and new type must have the same number of arguments.
        ''' The resulting method handle is guaranteed to report a type
        ''' which is equal to the desired new type.
        ''' <p>
        ''' If the original type and new type are equal, returns target.
        ''' <p>
        ''' The same conversions are allowed as for <seealso cref="MethodHandle#asType MethodHandle.asType"/>,
        ''' and some additional conversions are also applied if those conversions fail.
        ''' Given types <em>T0</em>, <em>T1</em>, one of the following conversions is applied
        ''' if possible, before or instead of any conversions done by {@code asType}:
        ''' <ul>
        ''' <li>If <em>T0</em> and <em>T1</em> are references, and <em>T1</em> is an interface type,
        '''     then the value of type <em>T0</em> is passed as a <em>T1</em> without a cast.
        '''     (This treatment of interfaces follows the usage of the bytecode verifier.)
        ''' <li>If <em>T0</em> is boolean and <em>T1</em> is another primitive,
        '''     the boolean is converted to a byte value, 1 for true, 0 for false.
        '''     (This treatment follows the usage of the bytecode verifier.)
        ''' <li>If <em>T1</em> is boolean and <em>T0</em> is another primitive,
        '''     <em>T0</em> is converted to byte via Java casting conversion (JLS 5.5),
        '''     and the low order bit of the result is tested, as if by {@code (x & 1) != 0}.
        ''' <li>If <em>T0</em> and <em>T1</em> are primitives other than boolean,
        '''     then a Java casting conversion (JLS 5.5) is applied.
        '''     (Specifically, <em>T0</em> will convert to <em>T1</em> by
        '''     widening and/or narrowing.)
        ''' <li>If <em>T0</em> is a reference and <em>T1</em> a primitive, an unboxing
        '''     conversion will be applied at runtime, possibly followed
        '''     by a Java casting conversion (JLS 5.5) on the primitive value,
        '''     possibly followed by a conversion from byte to boolean by testing
        '''     the low-order bit.
        ''' <li>If <em>T0</em> is a reference and <em>T1</em> a primitive,
        '''     and if the reference is null at runtime, a zero value is introduced.
        ''' </ul> </summary>
        ''' <param name="target"> the method handle to invoke after arguments are retyped </param>
        ''' <param name="newType"> the expected type of the new method handle </param>
        ''' <returns> a method handle which delegates to the target after performing
        '''           any necessary argument conversions, and arranges for any
        '''           necessary return value conversions </returns>
        ''' <exception cref="NullPointerException"> if either argument is null </exception>
        ''' <exception cref="WrongMethodTypeException"> if the conversion cannot be made </exception>
        ''' <seealso cref= MethodHandle#asType </seealso>
        Public Shared Function explicitCastArguments(ByVal target As MethodHandle, ByVal newType As MethodType) As MethodHandle
            explicitCastArgumentsChecks(target, newType)
            ' use the asTypeCache when possible:
            Dim oldType As MethodType = target.type()
            If oldType Is newType Then Return target
            If oldType.explicitCastEquivalentToAsType(newType) Then Return target.asFixedArity().asType(newType)
            Return MethodHandleImpl.makePairwiseConvert(target, newType, False)
        End Function

        Private Shared Sub explicitCastArgumentsChecks(ByVal target As MethodHandle, ByVal newType As MethodType)
            If target.type().parameterCount() <> newType.parameterCount() Then Throw New WrongMethodTypeException("cannot explicitly cast " & target & " to " & newType)
        End Sub

        ''' <summary>
        ''' Produces a method handle which adapts the calling sequence of the
        ''' given method handle to a new type, by reordering the arguments.
        ''' The resulting method handle is guaranteed to report a type
        ''' which is equal to the desired new type.
        ''' <p>
        ''' The given array controls the reordering.
        ''' Call {@code #I} the number of incoming parameters (the value
        ''' {@code newType.parameterCount()}, and call {@code #O} the number
        ''' of outgoing parameters (the value {@code target.type().parameterCount()}).
        ''' Then the length of the reordering array must be {@code #O},
        ''' and each element must be a non-negative number less than {@code #I}.
        ''' For every {@code N} less than {@code #O}, the {@code N}-th
        ''' outgoing argument will be taken from the {@code I}-th incoming
        ''' argument, where {@code I} is {@code reorder[N]}.
        ''' <p>
        ''' No argument or return value conversions are applied.
        ''' The type of each incoming argument, as determined by {@code newType},
        ''' must be identical to the type of the corresponding outgoing parameter
        ''' or parameters in the target method handle.
        ''' The return type of {@code newType} must be identical to the return
        ''' type of the original target.
        ''' <p>
        ''' The reordering array need not specify an actual permutation.
        ''' An incoming argument will be duplicated if its index appears
        ''' more than once in the array, and an incoming argument will be dropped
        ''' if its index does not appear in the array.
        ''' As in the case of <seealso cref="#dropArguments(MethodHandle,int,List) dropArguments"/>,
        ''' incoming arguments which are not mentioned in the reordering array
        ''' are may be any type, as determined only by {@code newType}.
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodType intfn1 = methodType(int.class, int.class);
        ''' MethodType intfn2 = methodType(int.class, int.class, int.class);
        ''' MethodHandle sub = ... (int x, int y) -> (x-y) ...;
        ''' assert(sub.type().equals(intfn2));
        ''' MethodHandle sub1 = permuteArguments(sub, intfn2, 0, 1);
        ''' MethodHandle rsub = permuteArguments(sub, intfn2, 1, 0);
        ''' assert((int)rsub.invokeExact(1, 100) == 99);
        ''' MethodHandle add = ... (int x, int y) -> (x+y) ...;
        ''' assert(add.type().equals(intfn2));
        ''' MethodHandle twice = permuteArguments(add, intfn1, 0, 0);
        ''' assert(twice.type().equals(intfn1));
        ''' assert((int)twice.invokeExact(21) == 42);
        ''' }</pre></blockquote> </summary>
        ''' <param name="target"> the method handle to invoke after arguments are reordered </param>
        ''' <param name="newType"> the expected type of the new method handle </param>
        ''' <param name="reorder"> an index array which controls the reordering </param>
        ''' <returns> a method handle which delegates to the target after it
        '''           drops unused arguments and moves and/or duplicates the other arguments </returns>
        ''' <exception cref="NullPointerException"> if any argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the index array length is not equal to
        '''                  the arity of the target, or if any index array element
        '''                  not a valid index for a parameter of {@code newType},
        '''                  or if two corresponding parameter types in
        '''                  {@code target.type()} and {@code newType} are not identical, </exception>
        Public Shared Function permuteArguments(ByVal target As MethodHandle, ByVal newType As MethodType, ParamArray ByVal reorder As Integer()) As MethodHandle
            reorder = reorder.Clone() ' get a private copy
            Dim oldType As MethodType = target.type()
            permuteArgumentChecks(reorder, newType, oldType)
            ' first detect dropped arguments and handle them separately
            Dim originalReorder As Integer() = reorder
            Dim result As BoundMethodHandle = target.rebind()
            Dim form As LambdaForm = result.form
            Dim newArity As Integer = newType.parameterCount()
            ' Normalize the reordering into a real permutation,
            ' by removing duplicates and adding dropped elements.
            ' This somewhat improves lambda form caching, as well
            ' as simplifying the transform by breaking it up into steps.
            Dim ddIdx As Integer
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            Do While (ddIdx = findFirstDupOrDrop(reorder, newArity)) <> 0
                If ddIdx > 0 Then
                    ' We found a duplicated entry at reorder[ddIdx].
                    ' Example:  (x,y,z)->asList(x,y,z)
                    ' permuted by [1*,0,1] => (a0,a1)=>asList(a1,a0,a1)
                    ' permuted by [0,1,0*] => (a0,a1)=>asList(a0,a1,a0)
                    ' The starred element corresponds to the argument
                    ' deleted by the dupArgumentForm transform.
                    Dim srcPos As Integer = ddIdx, dstPos As Integer = srcPos, dupVal As Integer = reorder(srcPos)
                    Dim killFirst As Boolean = False
                    Dim val As Integer
                    'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                    Do While (val = reorder(dstPos -= 1)) <> dupVal
                        ' Set killFirst if the dup is larger than an intervening position.
                        ' This will remove at least one inversion from the permutation.
                        If dupVal > val Then killFirst = True
                    Loop
                    If Not killFirst Then
                        srcPos = dstPos
                        dstPos = ddIdx
                    End If
                    form = form.editor().dupArgumentForm(1 + srcPos, 1 + dstPos)
                    Assert(reorder(srcPos) = reorder(dstPos))
                    oldType = oldType.dropParameterTypes(dstPos, dstPos + 1)
                    ' contract the reordering by removing the element at dstPos
                    Dim tailPos As Integer = dstPos + 1
                    Array.Copy(reorder, tailPos, reorder, dstPos, reorder.Length - tailPos)
                    reorder = java.util.Arrays.copyOf(reorder, reorder.Length - 1)
                Else
                    Dim dropVal As Integer = (Not ddIdx), insPos As Integer = 0
                    Do While insPos < reorder.Length AndAlso reorder(insPos) < dropVal
                        ' Find first element of reorder larger than dropVal.
                        ' This is where we will insert the dropVal.
                        insPos += 1
                    Loop
                    Dim ptype As [Class] = newType.parameterType(dropVal)
                    form = form.editor().addArgumentForm(1 + insPos, BasicType.basicType(ptype))
                    oldType = oldType.insertParameterTypes(insPos, ptype)
                    ' expand the reordering by inserting an element at insPos
                    Dim tailPos As Integer = insPos + 1
                    reorder = java.util.Arrays.copyOf(reorder, reorder.Length + 1)
                    Array.Copy(reorder, insPos, reorder, tailPos, reorder.Length - tailPos)
                    reorder(insPos) = dropVal
                End If
                Assert(permuteArgumentChecks(reorder, newType, oldType))
            Loop
            Assert(reorder.Length = newArity) ' a perfect permutation
            ' Note:  This may cache too many distinct LFs. Consider backing off to varargs code.
            form = form.editor().permuteArgumentsForm(1, reorder)
            If newType Is result.type() AndAlso form Is result.internalForm() Then Return result
            Return result.copyWith(newType, form)
        End Function

        ''' <summary>
        ''' Return an indication of any duplicate or omission in reorder.
        ''' If the reorder contains a duplicate entry, return the index of the second occurrence.
        ''' Otherwise, return ~(n), for the first n in [0..newArity-1] that is not present in reorder.
        ''' Otherwise, return zero.
        ''' If an element not in [0..newArity-1] is encountered, return reorder.length.
        ''' </summary>
        Private Shared Function findFirstDupOrDrop(ByVal reorder As Integer(), ByVal newArity As Integer) As Integer
            Const BIT_LIMIT As Integer = 63 ' max number of bits in bit mask
            If newArity < BIT_LIMIT Then
                Dim mask As Long = 0
                For i As Integer = 0 To reorder.Length - 1
                    Dim arg As Integer = reorder(i)
                    If arg >= newArity Then Return reorder.Length
                    Dim bit As Long = 1L << arg
                    If (mask And bit) <> 0 Then Return i ' >0 indicates a dup
                    mask = mask Or bit
                Next i
                If mask = (1L << newArity) - 1 Then
                    Assert([Long].numberOfTrailingZeros([Long].lowestOneBit((Not mask))) = newArity)
                    Return 0
                End If
                ' find first zero
                Dim zeroBit As Long = java.lang.[Long].lowestOneBit((Not mask))
                Dim zeroPos As Integer = java.lang.[Long].numberOfTrailingZeros(zeroBit)
                Assert(zeroPos <= newArity)
                If zeroPos = newArity Then Return 0
                Return Not zeroPos
            Else
                ' same algorithm, different bit set
                Dim mask As New BitArray(newArity)
                For i As Integer = 0 To reorder.Length - 1
                    Dim arg As Integer = reorder(i)
                    If arg >= newArity Then Return reorder.Length
                    If mask.Get(arg) Then Return i ' >0 indicates a dup
                    mask.Set(arg, True)
                Next i
                Dim zeroPos As Integer = mask.nextClearBit(0)
                Assert(zeroPos <= newArity)
                If zeroPos = newArity Then Return 0
                Return Not zeroPos
            End If
        End Function

        Private Shared Function permuteArgumentChecks(ByVal reorder As Integer(), ByVal newType As MethodTypeForm, ByVal oldType As MethodType) As Boolean
            If newType.returnType() IsNot oldType.returnType() Then Throw New IllegalArgumentException("return types do not match", oldType, newType)
            If reorder.Length = oldType.parameterCount() Then
                Dim limit As Integer = newType.parameterCount()
                Dim bad As Boolean = False
                For j As Integer = 0 To reorder.Length - 1
                    Dim i As Integer = reorder(j)
                    If i < 0 OrElse i >= limit Then
                        bad = True
                        Exit For
                    End If
                    Dim src As [Class] = newType.parameterType(i)
                    Dim dst As [Class] = oldType.parameterType(j)
                    If src IsNot dst Then Throw New IllegalArgumentException("parameter types do not match after reorder", oldType, newType)
                Next j
                If Not bad Then Return True
            End If
            Throw New IllegalArgumentException("bad reorder array: " & java.util.Arrays.ToString(reorder))
        End Function

        ''' <summary>
        ''' Produces a method handle of the requested return type which returns the given
        ''' constant value every time it is invoked.
        ''' <p>
        ''' Before the method handle is returned, the passed-in value is converted to the requested type.
        ''' If the requested type is primitive, widening primitive conversions are attempted,
        ''' else reference conversions are attempted.
        ''' <p>The returned method handle is equivalent to {@code identity(type).bindTo(value)}. </summary>
        ''' <param name="type"> the return type of the desired method handle </param>
        ''' <param name="value"> the value to return </param>
        ''' <returns> a method handle of the given return type and no arguments, which always returns the given value </returns>
        ''' <exception cref="NullPointerException"> if the {@code type} argument is null </exception>
        ''' <exception cref="ClassCastException"> if the value cannot be converted to the required return type </exception>
        ''' <exception cref="IllegalArgumentException"> if the given type is {@code void.class} </exception>
        Public Shared Function constant(ByVal type As [Class], ByVal value As Object) As MethodHandle
            If type.primitive Then
                If type Is GetType(Void) Then Throw New IllegalArgumentException("void type")
                Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(type)
                value = w.convert(value, type)
                If w.zero().Equals(value) Then Return zero(w, type)
                Return insertArguments(identity(type), 0, value)
            Else
                If value Is Nothing Then Return zero(sun.invoke.util.Wrapper.OBJECT, type)
                Return identity(type).bindTo(value)
            End If
        End Function

        ''' <summary>
        ''' Produces a method handle which returns its sole argument when invoked. </summary>
        ''' <param name="type"> the type of the sole parameter and return value of the desired method handle </param>
        ''' <returns> a unary method handle which accepts and returns the given type </returns>
        ''' <exception cref="NullPointerException"> if the argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given type is {@code void.class} </exception>
        Public Shared Function identity(ByVal type As [Class]) As MethodHandle
            Dim btw As sun.invoke.util.Wrapper = (If(type.primitive, sun.invoke.util.Wrapper.forPrimitiveType(type), sun.invoke.util.Wrapper.OBJECT))
            Dim pos As Integer = btw.ordinal()
            Dim ident As MethodHandle = IDENTITY_MHS(pos)
            If ident Is Nothing Then ident = cachedMethodHandledle(IDENTITY_MHS, pos, makeIdentity(btw.primitiveType()))
            If ident.type().returnType() Is type Then Return ident
            ' something like identity(Foo.class); do not bother to intern these
            Assert(btw Is sun.invoke.util.Wrapper.OBJECT)
            Return makeIdentity(type)
        End Function
        Private Shared ReadOnly IDENTITY_MHS As MethodHandle() = New MethodHandle(sun.invoke.util.Wrapper.values().length - 1) {}
        Private Shared Function makeIdentity(ByVal ptype As [Class]) As MethodHandle
            Dim mtype As MethodType = MethodType.methodType(ptype, ptype)
            Dim lform As LambdaForm = LambdaForm.identityForm(BasicType.basicType(ptype))
            Return MethodHandleImpl.makeIntrinsic(mtype, lform, Intrinsic.IDENTITY)
        End Function

        Private Shared Function zero(ByVal btw As sun.invoke.util.Wrapper, ByVal rtype As [Class]) As MethodHandle
            Dim pos As Integer = btw.ordinal()
            Dim zero_Renamed As MethodHandle = ZERO_MHS(pos)
            If zero_Renamed Is Nothing Then zero_Renamed = cachedMethodHandledle(ZERO_MHS, pos, makeZero(btw.primitiveType()))
            If zero_Renamed.type().returnType() Is rtype Then Return zero_Renamed
            Assert(btw Is sun.invoke.util.Wrapper.OBJECT)
            Return makeZero(rtype)
        End Function
        Private Shared ReadOnly ZERO_MHS As MethodHandle() = New MethodHandle(sun.invoke.util.Wrapper.values().length - 1) {}
        Private Shared Function makeZero(ByVal rtype As [Class]) As MethodHandle
            Dim mtype As MethodType = MethodType.methodType(rtype)
            Dim lform As LambdaForm = LambdaForm.zeroForm(BasicType.basicType(rtype))
            Return MethodHandleImpl.makeIntrinsic(mtype, lform, Intrinsic.ZERO)
        End Function

        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Shared Function setCachedMethodHandle(ByVal cache As MethodHandle(), ByVal pos As Integer, ByVal value As MethodHandle) As MethodHandle
            ' Simulate a CAS, to avoid racy duplication of results.
            Dim prev As MethodHandle = cache(pos)
            If prev IsNot Nothing Then Return prev
            cache(pos) = value
            Return cache(pos)
        End Function

        ''' <summary>
        ''' Provides a target method handle with one or more <em>bound arguments</em>
        ''' in advance of the method handle's invocation.
        ''' The formal parameters to the target corresponding to the bound
        ''' arguments are called <em>bound parameters</em>.
        ''' Returns a new method handle which saves away the bound arguments.
        ''' When it is invoked, it receives arguments for any non-bound parameters,
        ''' binds the saved arguments to their corresponding parameters,
        ''' and calls the original target.
        ''' <p>
        ''' The type of the new method handle will drop the types for the bound
        ''' parameters from the original target type, since the new method handle
        ''' will no longer require those arguments to be supplied by its callers.
        ''' <p>
        ''' Each given argument object must match the corresponding bound parameter type.
        ''' If a bound parameter type is a primitive, the argument object
        ''' must be a wrapper, and will be unboxed to produce the primitive value.
        ''' <p>
        ''' The {@code pos} argument selects which parameters are to be bound.
        ''' It may range between zero and <i>N-L</i> (inclusively),
        ''' where <i>N</i> is the arity of the target method handle
        ''' and <i>L</i> is the length of the values array. </summary>
        ''' <param name="target"> the method handle to invoke after the argument is inserted </param>
        ''' <param name="pos"> where to insert the argument (zero for the first) </param>
        ''' <param name="values"> the series of arguments to insert </param>
        ''' <returns> a method handle which inserts an additional argument,
        '''         before calling the original method handle </returns>
        ''' <exception cref="NullPointerException"> if the target or the {@code values} array is null </exception>
        ''' <seealso cref= MethodHandle#bindTo </seealso>
        Public Shared Function insertArguments(ByVal target As MethodHandle, ByVal pos As Integer, ParamArray ByVal values As Object()) As MethodHandle
            Dim insCount As Integer = values.Length
            Dim ptypes As [Class]() = insertArgumentsChecks(target, insCount, pos)
            If insCount = 0 Then Return target
            Dim result As BoundMethodHandle = target.rebind()
            For i As Integer = 0 To insCount - 1
                Dim value As Object = values(i)
                Dim ptype As [Class] = ptypes(pos + i)
                If ptype.primitive Then
                    result = insertArgumentPrimitive(result, pos, ptype, value)
                Else
                    value = ptype.cast(value) ' throw CCE if needed
                    result = result.bindArgumentL(pos, value)
                End If
            Next i
            Return result
        End Function

        Private Shared Function insertArgumentPrimitive(ByVal result As BoundMethodHandle, ByVal pos As Integer, ByVal ptype As [Class], ByVal value As Object) As BoundMethodHandle
            Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(ptype)
            ' perform unboxing and/or primitive conversion
            value = w.convert(value, ptype)
            Select Case w
                Case Int()
                    Return result.bindArgumentI(pos, CInt(Fix(value)))
                Case Long
                    Return result.bindArgumentJ(pos, CLng(Fix(value)))
                Case Float
                    Return result.bindArgumentF(pos, CSng(value))
                Case Double
                    Return result.bindArgumentD(pos, CDbl(value))
                Case Else
                    Return result.bindArgumentI(pos, sun.invoke.util.ValueConversions.widenSubword(value))
            End Select
        End Function

        Private Shared Function insertArgumentsChecks(ByVal target As MethodHandle, ByVal insCount As Integer, ByVal pos As Integer) As [Class]()
            Dim oldType As MethodType = target.type()
            Dim outargs As Integer = oldType.parameterCount()
            Dim inargs As Integer = outargs - insCount
            If inargs < 0 Then Throw newIllegalArgumentException("too many values to insert")
            If pos < 0 OrElse pos > inargs Then Throw newIllegalArgumentException("no argument type to append")
            Return oldType.ptypes()
        End Function

        ''' <summary>
        ''' Produces a method handle which will discard some dummy arguments
        ''' before calling some other specified <i>target</i> method handle.
        ''' The type of the new method handle will be the same as the target's type,
        ''' except it will also include the dummy argument types,
        ''' at some given position.
        ''' <p>
        ''' The {@code pos} argument may range between zero and <i>N</i>,
        ''' where <i>N</i> is the arity of the target.
        ''' If {@code pos} is zero, the dummy arguments will precede
        ''' the target's real arguments; if {@code pos} is <i>N</i>
        ''' they will come after.
        ''' <p>
        ''' <b>Example:</b>
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodHandle cat = lookup().findVirtual(String.class,
        ''' "concat", methodType(String.class, String.class));
        ''' assertEquals("xy", (String) cat.invokeExact("x", "y"));
        ''' MethodType bigType = cat.type().insertParameterTypes(0, int.class, String.class);
        ''' MethodHandle d0 = dropArguments(cat, 0, bigType.parameterList().subList(0,2));
        ''' assertEquals(bigType, d0.type());
        ''' assertEquals("yz", (String) d0.invokeExact(123, "x", "y", "z"));
        ''' }</pre></blockquote>
        ''' <p>
        ''' This method is also equivalent to the following code:
        ''' <blockquote><pre>
        ''' <seealso cref="#dropArguments(MethodHandle,int,Class...) dropArguments"/>{@code (target, pos, valueTypes.toArray(new Class[0]))}
        ''' </pre></blockquote> </summary>
        ''' <param name="target"> the method handle to invoke after the arguments are dropped </param>
        ''' <param name="valueTypes"> the type(s) of the argument(s) to drop </param>
        ''' <param name="pos"> position of first argument to drop (zero for the leftmost) </param>
        ''' <returns> a method handle which drops arguments of the given types,
        '''         before calling the original method handle </returns>
        ''' <exception cref="NullPointerException"> if the target is null,
        '''                              or if the {@code valueTypes} list or any of its elements is null </exception>
        ''' <exception cref="IllegalArgumentException"> if any element of {@code valueTypes} is {@code void.class},
        '''                  or if {@code pos} is negative or greater than the arity of the target,
        '''                  or if the new method handle's type would have too many parameters </exception>
        Public Shared Function dropArguments(ByVal target As MethodHandle, ByVal pos As Integer, ByVal valueTypes As IList(Of [Class])) As MethodHandle
            Dim oldType As MethodType = target.type() ' get NPE
            Dim dropped As Integer = dropArgumentChecks(oldType, pos, valueTypes)
            Dim newType As MethodType = oldType.insertParameterTypes(pos, valueTypes)
            If dropped = 0 Then Return target
            Dim result As BoundMethodHandle = target.rebind()
            Dim lform As LambdaForm = result.form
            Dim insertFormArg As Integer = 1 + pos
            For Each ptype As [Class] In valueTypes
                lform = lform.editor().addArgumentForm(insertFormArg, BasicType.basicType(ptype))
                insertFormArg += 1
            Next ptype
            result = result.copyWith(newType, lform)
            Return result
        End Function

        Private Shared Function dropArgumentChecks(ByVal oldType As MethodType, ByVal pos As Integer, ByVal valueTypes As IList(Of [Class])) As Integer
            Dim dropped As Integer = valueTypes.Count
            MethodType.checkSlotCount(dropped)
            Dim outargs As Integer = oldType.parameterCount()
            Dim inargs As Integer = outargs + dropped
            If pos < 0 OrElse pos > outargs Then Throw newIllegalArgumentException("no argument type to remove" & java.util.Arrays.asList(oldType, pos, valueTypes, inargs, outargs))
            Return dropped
        End Function

        ''' <summary>
        ''' Produces a method handle which will discard some dummy arguments
        ''' before calling some other specified <i>target</i> method handle.
        ''' The type of the new method handle will be the same as the target's type,
        ''' except it will also include the dummy argument types,
        ''' at some given position.
        ''' <p>
        ''' The {@code pos} argument may range between zero and <i>N</i>,
        ''' where <i>N</i> is the arity of the target.
        ''' If {@code pos} is zero, the dummy arguments will precede
        ''' the target's real arguments; if {@code pos} is <i>N</i>
        ''' they will come after.
        ''' <p>
        ''' <b>Example:</b>
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodHandle cat = lookup().findVirtual(String.class,
        ''' "concat", methodType(String.class, String.class));
        ''' assertEquals("xy", (String) cat.invokeExact("x", "y"));
        ''' MethodHandle d0 = dropArguments(cat, 0, String.class);
        ''' assertEquals("yz", (String) d0.invokeExact("x", "y", "z"));
        ''' MethodHandle d1 = dropArguments(cat, 1, String.class);
        ''' assertEquals("xz", (String) d1.invokeExact("x", "y", "z"));
        ''' MethodHandle d2 = dropArguments(cat, 2, String.class);
        ''' assertEquals("xy", (String) d2.invokeExact("x", "y", "z"));
        ''' MethodHandle d12 = dropArguments(cat, 1, int.class,  java.lang.[Boolean].class);
        ''' assertEquals("xz", (String) d12.invokeExact("x", 12, true, "z"));
        ''' }</pre></blockquote>
        ''' <p>
        ''' This method is also equivalent to the following code:
        ''' <blockquote><pre>
        ''' <seealso cref="#dropArguments(MethodHandle,int,List) dropArguments"/>{@code (target, pos, Arrays.asList(valueTypes))}
        ''' </pre></blockquote> </summary>
        ''' <param name="target"> the method handle to invoke after the arguments are dropped </param>
        ''' <param name="valueTypes"> the type(s) of the argument(s) to drop </param>
        ''' <param name="pos"> position of first argument to drop (zero for the leftmost) </param>
        ''' <returns> a method handle which drops arguments of the given types,
        '''         before calling the original method handle </returns>
        ''' <exception cref="NullPointerException"> if the target is null,
        '''                              or if the {@code valueTypes} array or any of its elements is null </exception>
        ''' <exception cref="IllegalArgumentException"> if any element of {@code valueTypes} is {@code void.class},
        '''                  or if {@code pos} is negative or greater than the arity of the target,
        '''                  or if the new method handle's type would have
        '''                  <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
        Public Shared Function dropArguments(ByVal target As MethodHandle, ByVal pos As Integer, ParamArray ByVal valueTypes As [Class]()) As MethodHandle
            Return dropArguments(target, pos, java.util.Arrays.asList(valueTypes))
        End Function

        ''' <summary>
        ''' Adapts a target method handle by pre-processing
        ''' one or more of its arguments, each with its own unary filter function,
        ''' and then calling the target with each pre-processed argument
        ''' replaced by the result of its corresponding filter function.
        ''' <p>
        ''' The pre-processing is performed by one or more method handles,
        ''' specified in the elements of the {@code filters} array.
        ''' The first element of the filter array corresponds to the {@code pos}
        ''' argument of the target, and so on in sequence.
        ''' <p>
        ''' Null arguments in the array are treated as identity functions,
        ''' and the corresponding arguments left unchanged.
        ''' (If there are no non-null elements in the array, the original target is returned.)
        ''' Each filter is applied to the corresponding argument of the adapter.
        ''' <p>
        ''' If a filter {@code F} applies to the {@code N}th argument of
        ''' the target, then {@code F} must be a method handle which
        ''' takes exactly one argument.  The type of {@code F}'s sole argument
        ''' replaces the corresponding argument type of the target
        ''' in the resulting adapted method handle.
        ''' The return type of {@code F} must be identical to the corresponding
        ''' parameter type of the target.
        ''' <p>
        ''' It is an error if there are elements of {@code filters}
        ''' (null or not)
        ''' which do not correspond to argument positions in the target.
        ''' <p><b>Example:</b>
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodHandle cat = lookup().findVirtual(String.class,
        ''' "concat", methodType(String.class, String.class));
        ''' MethodHandle upcase = lookup().findVirtual(String.class,
        ''' "toUpperCase", methodType(String.class));
        ''' assertEquals("xy", (String) cat.invokeExact("x", "y"));
        ''' MethodHandle f0 = filterArguments(cat, 0, upcase);
        ''' assertEquals("Xy", (String) f0.invokeExact("x", "y")); // Xy
        ''' MethodHandle f1 = filterArguments(cat, 1, upcase);
        ''' assertEquals("xY", (String) f1.invokeExact("x", "y")); // xY
        ''' MethodHandle f2 = filterArguments(cat, 0, upcase, upcase);
        ''' assertEquals("XY", (String) f2.invokeExact("x", "y")); // XY
        ''' }</pre></blockquote>
        ''' <p> Here is pseudocode for the resulting adapter:
        ''' <blockquote><pre>{@code
        ''' V target(P... p, A[i]... a[i], B... b);
        ''' A[i] filter[i](V[i]);
        ''' T adapter(P... p, V[i]... v[i], B... b) {
        '''   return target(p..., f[i](v[i])..., b...);
        ''' }
        ''' }</pre></blockquote>
        ''' </summary>
        ''' <param name="target"> the method handle to invoke after arguments are filtered </param>
        ''' <param name="pos"> the position of the first argument to filter </param>
        ''' <param name="filters"> method handles to call initially on filtered arguments </param>
        ''' <returns> method handle which incorporates the specified argument filtering logic </returns>
        ''' <exception cref="NullPointerException"> if the target is null
        '''                              or if the {@code filters} array is null </exception>
        ''' <exception cref="IllegalArgumentException"> if a non-null element of {@code filters}
        '''          does not match a corresponding argument type of target as described above,
        '''          or if the {@code pos+filters.length} is greater than {@code target.type().parameterCount()},
        '''          or if the resulting method handle's type would have
        '''          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
        Public Shared Function filterArguments(ByVal target As MethodHandle, ByVal pos As Integer, ParamArray ByVal filters As MethodHandle()) As MethodHandle
            filterArgumentsCheckArity(target, pos, filters)
            Dim adapter As MethodHandle = target
            Dim curPos As Integer = pos - 1 ' pre-incremented
            For Each filter As MethodHandle In filters
                curPos += 1
                If filter Is Nothing Then ' ignore null elements of filters Continue For
                    adapter = filterArgument(adapter, curPos, filter)
            Next filter
            Return adapter
        End Function

        'non-public
        Friend Shared Function filterArgument(ByVal target As MethodHandle, ByVal pos As Integer, ByVal filter As MethodHandle) As MethodHandle
            filterArgumentChecks(target, pos, filter)
            Dim targetType As MethodType = target.type()
            Dim filterType As MethodType = filter.type()
            Dim result As BoundMethodHandle = target.rebind()
            Dim newParamType As [Class] = filterType.parameterType(0)
            Dim lform As LambdaForm = result.editor().filterArgumentForm(1 + pos, BasicType.basicType(newParamType))
            Dim newType As MethodType = targetType.changeParameterType(pos, newParamType)
            result = result.copyWithExtendL(newType, lform, filter)
            Return result
        End Function

        Private Shared Sub filterArgumentsCheckArity(ByVal target As MethodHandle, ByVal pos As Integer, ByVal filters As MethodHandle())
            Dim targetType As MethodType = target.type()
            Dim maxPos As Integer = targetType.parameterCount()
            If pos + filters.Length > maxPos Then Throw newIllegalArgumentException("too many filters")
        End Sub

        Private Shared Sub filterArgumentChecks(ByVal target As MethodHandle, ByVal pos As Integer, ByVal filter As MethodHandle)
            Dim targetType As MethodType = target.type()
            Dim filterType As MethodType = filter.type()
            If filterType.parameterCount() <> 1 OrElse filterType.returnType() IsNot targetType.parameterType(pos) Then Throw newIllegalArgumentException("target and filter types do not match", targetType, filterType)
        End Sub

        ''' <summary>
        ''' Adapts a target method handle by pre-processing
        ''' a sub-sequence of its arguments with a filter (another method handle).
        ''' The pre-processed arguments are replaced by the result (if any) of the
        ''' filter function.
        ''' The target is then called on the modified (usually shortened) argument list.
        ''' <p>
        ''' If the filter returns a value, the target must accept that value as
        ''' its argument in position {@code pos}, preceded and/or followed by
        ''' any arguments not passed to the filter.
        ''' If the filter returns void, the target must accept all arguments
        ''' not passed to the filter.
        ''' No arguments are reordered, and a result returned from the filter
        ''' replaces (in order) the whole subsequence of arguments originally
        ''' passed to the adapter.
        ''' <p>
        ''' The argument types (if any) of the filter
        ''' replace zero or one argument types of the target, at position {@code pos},
        ''' in the resulting adapted method handle.
        ''' The return type of the filter (if any) must be identical to the
        ''' argument type of the target at position {@code pos}, and that target argument
        ''' is supplied by the return value of the filter.
        ''' <p>
        ''' In all cases, {@code pos} must be greater than or equal to zero, and
        ''' {@code pos} must also be less than or equal to the target's arity.
        ''' <p><b>Example:</b>
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodHandle deepToString = publicLookup()
        ''' .findStatic(Arrays.class, "deepToString", methodType(String.class, Object[].class));
        ''' 
        ''' MethodHandle ts1 = deepToString.asCollector(String[].class, 1);
        ''' assertEquals("[strange]", (String) ts1.invokeExact("strange"));
        ''' 
        ''' MethodHandle ts2 = deepToString.asCollector(String[].class, 2);
        ''' assertEquals("[up, down]", (String) ts2.invokeExact("up", "down"));
        ''' 
        ''' MethodHandle ts3 = deepToString.asCollector(String[].class, 3);
        ''' MethodHandle ts3_ts2 = collectArguments(ts3, 1, ts2);
        ''' assertEquals("[top, [up, down], strange]",
        '''         (String) ts3_ts2.invokeExact("top", "up", "down", "strange"));
        ''' 
        ''' MethodHandle ts3_ts2_ts1 = collectArguments(ts3_ts2, 3, ts1);
        ''' assertEquals("[top, [up, down], [strange]]",
        '''         (String) ts3_ts2_ts1.invokeExact("top", "up", "down", "strange"));
        ''' 
        ''' MethodHandle ts3_ts2_ts3 = collectArguments(ts3_ts2, 1, ts3);
        ''' assertEquals("[top, [[up, down, strange], charm], bottom]",
        '''         (String) ts3_ts2_ts3.invokeExact("top", "up", "down", "strange", "charm", "bottom"));
        ''' }</pre></blockquote>
        ''' <p> Here is pseudocode for the resulting adapter:
        ''' <blockquote><pre>{@code
        ''' T target(A...,V,C...);
        ''' V filter(B...);
        ''' T adapter(A... a,B... b,C... c) {
        '''   V v = filter(b...);
        '''   return target(a...,v,c...);
        ''' }
        ''' // and if the filter has no arguments:
        ''' T target2(A...,V,C...);
        ''' V filter2();
        ''' T adapter2(A... a,C... c) {
        '''   V v = filter2();
        '''   return target2(a...,v,c...);
        ''' }
        ''' // and if the filter has a void return:
        ''' T target3(A...,C...);
        ''' void filter3(B...);
        ''' void adapter3(A... a,B... b,C... c) {
        '''   filter3(b...);
        '''   return target3(a...,c...);
        ''' }
        ''' }</pre></blockquote>
        ''' <p>
        ''' A collection adapter {@code collectArguments(mh, 0, coll)} is equivalent to
        ''' one which first "folds" the affected arguments, and then drops them, in separate
        ''' steps as follows:
        ''' <blockquote><pre>{@code
        ''' mh = MethodHandles.dropArguments(mh, 1, coll.type().parameterList()); //step 2
        ''' mh = MethodHandles.foldArguments(mh, coll); //step 1
        ''' }</pre></blockquote>
        ''' If the target method handle consumes no arguments besides than the result
        ''' (if any) of the filter {@code coll}, then {@code collectArguments(mh, 0, coll)}
        ''' is equivalent to {@code filterReturnValue(coll, mh)}.
        ''' If the filter method handle {@code coll} consumes one argument and produces
        ''' a non-void result, then {@code collectArguments(mh, N, coll)}
        ''' is equivalent to {@code filterArguments(mh, N, coll)}.
        ''' Other equivalences are possible but would require argument permutation.
        ''' </summary>
        ''' <param name="target"> the method handle to invoke after filtering the subsequence of arguments </param>
        ''' <param name="pos"> the position of the first adapter argument to pass to the filter,
        '''            and/or the target argument which receives the result of the filter </param>
        ''' <param name="filter"> method handle to call on the subsequence of arguments </param>
        ''' <returns> method handle which incorporates the specified argument subsequence filtering logic </returns>
        ''' <exception cref="NullPointerException"> if either argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the return type of {@code filter}
        '''          is non-void and is not the same as the {@code pos} argument of the target,
        '''          or if {@code pos} is not between 0 and the target's arity, inclusive,
        '''          or if the resulting method handle's type would have
        '''          <a href="MethodHandle.html#maxarity">too many parameters</a> </exception>
        ''' <seealso cref= MethodHandles#foldArguments </seealso>
        ''' <seealso cref= MethodHandles#filterArguments </seealso>
        ''' <seealso cref= MethodHandles#filterReturnValue </seealso>
        Public Shared Function collectArguments(ByVal target As MethodHandle, ByVal pos As Integer, ByVal filter As MethodHandle) As MethodHandle
            Dim newType As MethodType = collectArgumentsChecks(target, pos, filter)
            Dim collectorType As MethodType = filter.type()
            Dim result As BoundMethodHandle = target.rebind()
            Dim lform As LambdaForm
            If collectorType.returnType().array AndAlso filter.intrinsicName() Is Intrinsic.NEW_ARRAY Then
                lform = result.editor().collectArgumentArrayForm(1 + pos, filter)
                If lform IsNot Nothing Then Return result.copyWith(newType, lform)
            End If
            lform = result.editor().collectArgumentsForm(1 + pos, collectorType.basicType())
            Return result.copyWithExtendL(newType, lform, filter)
        End Function

        Private Shared Function collectArgumentsChecks(ByVal target As MethodHandle, ByVal pos As Integer, ByVal filter As MethodHandle) As MethodType
            Dim targetType As MethodType = target.type()
            Dim filterType As MethodType = filter.type()
            Dim rtype As [Class] = filterType.returnType()
            Dim filterArgs As IList(Of [Class]) = filterType.parameterList()
            If rtype Is GetType(Void) Then Return targetType.insertParameterTypes(pos, filterArgs)
            If rtype IsNot targetType.parameterType(pos) Then Throw newIllegalArgumentException("target and filter types do not match", targetType, filterType)
            Return targetType.dropParameterTypes(pos, pos + 1).insertParameterTypes(pos, filterArgs)
        End Function

        ''' <summary>
        ''' Adapts a target method handle by post-processing
        ''' its return value (if any) with a filter (another method handle).
        ''' The result of the filter is returned from the adapter.
        ''' <p>
        ''' If the target returns a value, the filter must accept that value as
        ''' its only argument.
        ''' If the target returns void, the filter must accept no arguments.
        ''' <p>
        ''' The return type of the filter
        ''' replaces the return type of the target
        ''' in the resulting adapted method handle.
        ''' The argument type of the filter (if any) must be identical to the
        ''' return type of the target.
        ''' <p><b>Example:</b>
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodHandle cat = lookup().findVirtual(String.class,
        ''' "concat", methodType(String.class, String.class));
        ''' MethodHandle length = lookup().findVirtual(String.class,
        ''' "length", methodType(int.class));
        ''' System.out.println((String) cat.invokeExact("x", "y")); // xy
        ''' MethodHandle f0 = filterReturnValue(cat, length);
        ''' System.out.println((int) f0.invokeExact("x", "y")); // 2
        ''' }</pre></blockquote>
        ''' <p> Here is pseudocode for the resulting adapter:
        ''' <blockquote><pre>{@code
        ''' V target(A...);
        ''' T filter(V);
        ''' T adapter(A... a) {
        '''   V v = target(a...);
        '''   return filter(v);
        ''' }
        ''' // and if the target has a void return:
        ''' void target2(A...);
        ''' T filter2();
        ''' T adapter2(A... a) {
        '''   target2(a...);
        '''   return filter2();
        ''' }
        ''' // and if the filter has a void return:
        ''' V target3(A...);
        ''' void filter3(V);
        ''' void adapter3(A... a) {
        '''   V v = target3(a...);
        '''   filter3(v);
        ''' }
        ''' }</pre></blockquote> </summary>
        ''' <param name="target"> the method handle to invoke before filtering the return value </param>
        ''' <param name="filter"> method handle to call on the return value </param>
        ''' <returns> method handle which incorporates the specified return value filtering logic </returns>
        ''' <exception cref="NullPointerException"> if either argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the argument list of {@code filter}
        '''          does not match the return type of target as described above </exception>
        Public Shared Function filterReturnValue(ByVal target As MethodHandle, ByVal filter As MethodHandle) As MethodHandle
            Dim targetType As MethodType = target.type()
            Dim filterType As MethodType = filter.type()
            filterReturnValueChecks(targetType, filterType)
            Dim result As BoundMethodHandle = target.rebind()
            Dim rtype As BasicType = BasicType.basicType(filterType.returnType())
            Dim lform As LambdaForm = result.editor().filterReturnForm(rtype, False)
            Dim newType As MethodType = targetType.changeReturnType(filterType.returnType())
            result = result.copyWithExtendL(newType, lform, filter)
            Return result
        End Function

        Private Shared Sub filterReturnValueChecks(ByVal targetType As MethodType, ByVal filterType As MethodType)
            Dim rtype As [Class] = targetType.returnType()
            Dim filterValues As Integer = filterType.parameterCount()
            If If(filterValues = 0, (rtype IsNot GetType(Void)), (rtype IsNot filterType.parameterType(0))) Then Throw newIllegalArgumentException("target and filter types do not match", targetType, filterType)
        End Sub

        ''' <summary>
        ''' Adapts a target method handle by pre-processing
        ''' some of its arguments, and then calling the target with
        ''' the result of the pre-processing, inserted into the original
        ''' sequence of arguments.
        ''' <p>
        ''' The pre-processing is performed by {@code combiner}, a second method handle.
        ''' Of the arguments passed to the adapter, the first {@code N} arguments
        ''' are copied to the combiner, which is then called.
        ''' (Here, {@code N} is defined as the parameter count of the combiner.)
        ''' After this, control passes to the target, with any result
        ''' from the combiner inserted before the original {@code N} incoming
        ''' arguments.
        ''' <p>
        ''' If the combiner returns a value, the first parameter type of the target
        ''' must be identical with the return type of the combiner, and the next
        ''' {@code N} parameter types of the target must exactly match the parameters
        ''' of the combiner.
        ''' <p>
        ''' If the combiner has a void return, no result will be inserted,
        ''' and the first {@code N} parameter types of the target
        ''' must exactly match the parameters of the combiner.
        ''' <p>
        ''' The resulting adapter is the same type as the target, except that the
        ''' first parameter type is dropped,
        ''' if it corresponds to the result of the combiner.
        ''' <p>
        ''' (Note that <seealso cref="#dropArguments(MethodHandle,int,List) dropArguments"/> can be used to remove any arguments
        ''' that either the combiner or the target does not wish to receive.
        ''' If some of the incoming arguments are destined only for the combiner,
        ''' consider using <seealso cref="MethodHandle#asCollector asCollector"/> instead, since those
        ''' arguments will not need to be live on the stack on entry to the
        ''' target.)
        ''' <p><b>Example:</b>
        ''' <blockquote><pre>{@code
        ''' import static java.lang.invoke.MethodHandles.*;
        ''' import static java.lang.invoke.MethodType.*;
        ''' ...
        ''' MethodHandle trace = publicLookup().findVirtual(java.io.PrintStream.class,
        ''' "println", methodType(void.class, String.class))
        ''' .bindTo(System.out);
        ''' MethodHandle cat = lookup().findVirtual(String.class,
        ''' "concat", methodType(String.class, String.class));
        ''' assertEquals("boojum", (String) cat.invokeExact("boo", "jum"));
        ''' MethodHandle catTrace = foldArguments(cat, trace);
        ''' // also prints "boo":
        ''' assertEquals("boojum", (String) catTrace.invokeExact("boo", "jum"));
        ''' }</pre></blockquote>
        ''' <p> Here is pseudocode for the resulting adapter:
        ''' <blockquote><pre>{@code
        ''' // there are N arguments in A...
        ''' T target(V, A[N]..., B...);
        ''' V combiner(A...);
        ''' T adapter(A... a, B... b) {
        '''   V v = combiner(a...);
        '''   return target(v, a..., b...);
        ''' }
        ''' // and if the combiner has a void return:
        ''' T target2(A[N]..., B...);
        ''' void combiner2(A...);
        ''' T adapter2(A... a, B... b) {
        '''   combiner2(a...);
        '''   return target2(a..., b...);
        ''' }
        ''' }</pre></blockquote> </summary>
        ''' <param name="target"> the method handle to invoke after arguments are combined </param>
        ''' <param name="combiner"> method handle to call initially on the incoming arguments </param>
        ''' <returns> method handle which incorporates the specified argument folding logic </returns>
        ''' <exception cref="NullPointerException"> if either argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code combiner}'s return type
        '''          is non-void and not the same as the first argument type of
        '''          the target, or if the initial {@code N} argument types
        '''          of the target
        '''          (skipping one matching the {@code combiner}'s return type)
        '''          are not identical with the argument types of {@code combiner} </exception>
        Public Shared Function foldArguments(ByVal target As MethodHandle, ByVal combiner As MethodHandle) As MethodHandle
            Dim foldPos As Integer = 0
            Dim targetType As MethodType = target.type()
            Dim combinerType As MethodType = combiner.type()
            Dim rtype As [Class] = foldArgumentChecks(foldPos, targetType, combinerType)
            Dim result As BoundMethodHandle = target.rebind()
            Dim dropResult As Boolean = (rtype Is GetType(Void))
            ' Note:  This may cache too many distinct LFs. Consider backing off to varargs code.
            Dim lform As LambdaForm = result.editor().foldArgumentsForm(1 + foldPos, dropResult, combinerType.basicType())
            Dim newType As MethodType = targetType
            If Not dropResult Then newType = newType.dropParameterTypes(foldPos, foldPos + 1)
            result = result.copyWithExtendL(newType, lform, combiner)
            Return result
        End Function

        Private Shared Function foldArgumentChecks(ByVal foldPos As Integer, ByVal targetType As MethodType, ByVal combinerType As MethodType) As [Class]
            Dim foldArgs As Integer = combinerType.parameterCount()
            Dim rtype As [Class] = combinerType.returnType()
            Dim foldVals As Integer = If(rtype Is GetType(Void), 0, 1)
            Dim afterInsertPos As Integer = foldPos + foldVals
            Dim ok As Boolean = (targetType.parameterCount() >= afterInsertPos + foldArgs)
            If ok AndAlso Not (combinerType.parameterList().Equals(targetType.parameterList().subList(afterInsertPos, afterInsertPos + foldArgs))) Then ok = False
            If ok AndAlso foldVals <> 0 AndAlso combinerType.returnType() IsNot targetType.parameterType(0) Then ok = False
            If Not ok Then Throw misMatchedTypes("target and combiner types", targetType, combinerType)
            Return rtype
        End Function

        ''' <summary>
        ''' Makes a method handle which adapts a target method handle,
        ''' by guarding it with a test, a boolean-valued method handle.
        ''' If the guard fails, a fallback handle is called instead.
        ''' All three method handles must have the same corresponding
        ''' argument and return types, except that the return type
        ''' of the test must be boolean, and the test is allowed
        ''' to have fewer arguments than the other two method handles.
        ''' <p> Here is pseudocode for the resulting adapter:
        ''' <blockquote><pre>{@code
        ''' boolean test(A...);
        ''' T target(A...,B...);
        ''' T fallback(A...,B...);
        ''' T adapter(A... a,B... b) {
        '''   if (test(a...))
        '''     return target(a..., b...);
        '''   else
        '''     return fallback(a..., b...);
        ''' }
        ''' }</pre></blockquote>
        ''' Note that the test arguments ({@code a...} in the pseudocode) cannot
        ''' be modified by execution of the test, and so are passed unchanged
        ''' from the caller to the target or fallback as appropriate. </summary>
        ''' <param name="test"> method handle used for test, must return boolean </param>
        ''' <param name="target"> method handle to call if test passes </param>
        ''' <param name="fallback"> method handle to call if test fails </param>
        ''' <returns> method handle which incorporates the specified if/then/else logic </returns>
        ''' <exception cref="NullPointerException"> if any argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code test} does not return boolean,
        '''          or if all three method types do not match (with the return
        '''          type of {@code test} changed to match that of the target). </exception>
        Public Shared Function guardWithTest(ByVal test As MethodHandle, ByVal target As MethodHandle, ByVal fallback As MethodHandle) As MethodHandle
            Dim gtype As MethodType = test.type()
            Dim ttype As MethodType = target.type()
            Dim ftype As MethodType = fallback.type()
            If Not ttype.Equals(ftype) Then Throw misMatchedTypes("target and fallback types", ttype, ftype)
            If gtype.returnType() IsNot GetType(Boolean) Then Throw newIllegalArgumentException("guard type is not a predicate " & gtype)
            Dim targs As IList(Of [Class]) = ttype.parameterList()
            Dim gargs As IList(Of [Class]) = gtype.parameterList()
            If Not targs.Equals(gargs) Then
                Dim gpc As Integer = gargs.Count, tpc As Integer = targs.Count
                If gpc >= tpc OrElse (Not targs.subList(0, gpc).Equals(gargs)) Then Throw misMatchedTypes("target and test types", ttype, gtype)
                test = dropArguments(test, gpc, targs.subList(gpc, tpc))
                gtype = test.type()
            End If
            Return MethodHandleImpl.makeGuardWithTest(test, target, fallback)
        End Function

        Friend Shared Function misMatchedTypes(ByVal what As String, ByVal t1 As MethodType, ByVal t2 As MethodType) As RuntimeException
            Return newIllegalArgumentException(what & " must match: " & t1 & " != " & t2)
        End Function

        ''' <summary>
        ''' Makes a method handle which adapts a target method handle,
        ''' by running it inside an exception handler.
        ''' If the target returns normally, the adapter returns that value.
        ''' If an exception matching the specified type is thrown, the fallback
        ''' handle is called instead on the exception, plus the original arguments.
        ''' <p>
        ''' The target and handler must have the same corresponding
        ''' argument and return types, except that handler may omit trailing arguments
        ''' (similarly to the predicate in <seealso cref="#guardWithTest guardWithTest"/>).
        ''' Also, the handler must have an extra leading parameter of {@code exType} or a supertype.
        ''' <p> Here is pseudocode for the resulting adapter:
        ''' <blockquote><pre>{@code
        ''' T target(A..., B...);
        ''' T handler(ExType, A...);
        ''' T adapter(A... a, B... b) {
        '''   try {
        '''     return target(a..., b...);
        '''   } catch (ExType ex) {
        '''     return handler(ex, a...);
        '''   }
        ''' }
        ''' }</pre></blockquote>
        ''' Note that the saved arguments ({@code a...} in the pseudocode) cannot
        ''' be modified by execution of the target, and so are passed unchanged
        ''' from the caller to the handler, if the handler is invoked.
        ''' <p>
        ''' The target and handler must return the same type, even if the handler
        ''' always throws.  (This might happen, for instance, because the handler
        ''' is simulating a {@code finally} clause).
        ''' To create such a throwing handler, compose the handler creation logic
        ''' with <seealso cref="#throwException throwException"/>,
        ''' in order to create a method handle of the correct return type. </summary>
        ''' <param name="target"> method handle to call </param>
        ''' <param name="exType"> the type of exception which the handler will catch </param>
        ''' <param name="handler"> method handle to call if a matching exception is thrown </param>
        ''' <returns> method handle which incorporates the specified try/catch logic </returns>
        ''' <exception cref="NullPointerException"> if any argument is null </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code handler} does not accept
        '''          the given exception type, or if the method handle types do
        '''          not match in their return types and their
        '''          corresponding parameters </exception>
        Public Shared Function catchException(ByVal target As MethodHandle, ByVal exType As [Class], ByVal handler As MethodHandle) As MethodHandle
            Dim ttype As MethodType = target.type()
            Dim htype As MethodType = handler.type()
            If htype.parameterCount() < 1 OrElse (Not exType.IsSubclassOf(htype.parameterType(0))) Then Throw newIllegalArgumentException("handler does not accept exception type " & exType)
            If htype.returnType() IsNot ttype.returnType() Then Throw misMatchedTypes("target and handler return types", ttype, htype)
            Dim targs As IList(Of [Class]) = ttype.parameterList()
            Dim hargs As IList(Of [Class]) = htype.parameterList()
            hargs = hargs.subList(1, hargs.Count) ' omit leading parameter from handler
            If Not targs.Equals(hargs) Then
                Dim hpc As Integer = hargs.Count, tpc As Integer = targs.Count
                If hpc >= tpc OrElse (Not targs.subList(0, hpc).Equals(hargs)) Then Throw misMatchedTypes("target and handler types", ttype, htype)
                handler = dropArguments(handler, 1 + hpc, targs.subList(hpc, tpc))
                htype = handler.type()
            End If
            Return MethodHandleImpl.makeGuardWithCatch(target, exType, handler)
        End Function

        ''' <summary>
        ''' Produces a method handle which will throw exceptions of the given {@code exType}.
        ''' The method handle will accept a single argument of {@code exType},
        ''' and immediately throw it as an exception.
        ''' The method type will nominally specify a return of {@code returnType}.
        ''' The return type may be anything convenient:  It doesn't matter to the
        ''' method handle's behavior, since it will never return normally. </summary>
        ''' <param name="returnType"> the return type of the desired method handle </param>
        ''' <param name="exType"> the parameter type of the desired method handle </param>
        ''' <returns> method handle which can throw the given exceptions </returns>
        ''' <exception cref="NullPointerException"> if either argument is null </exception>
        Public Shared Function throwException(ByVal returnType As [Class], ByVal exType As [Class]) As MethodHandle
            If Not exType.IsSubclassOf(GetType(Throwable)) Then Throw New ClassCastException(exType.name)
            Return MethodHandleImpl.throwException(MethodType.methodType(returnType, exType))
        End Function
    End Class

End Namespace
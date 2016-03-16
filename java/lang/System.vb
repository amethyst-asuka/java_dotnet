Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading
Imports System.Runtime.InteropServices
Imports java.io
Imports java.security

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' The <code>System</code> class contains several useful class fields
    ''' and methods. It cannot be instantiated.
    ''' 
    ''' <p>Among the facilities provided by the <code>System</code> class
    ''' are standard input, standard output, and error output streams;
    ''' access to externally defined properties and environment
    ''' variables; a means of loading files and libraries; and a utility
    ''' method for quickly copying a portion of an array.
    ''' 
    ''' @author  unascribed
    ''' @since   JDK1.0
    ''' </summary>
    Public NotInheritable Class System

        '     register the natives via the static initializer.
        '     *
        '     * VM will invoke the initializeSystemClass method to complete
        '     * the initialization for this class separated from clinit.
        '     * Note that to use properties set by the VM, see the constraints
        '     * described in the initializeSystemClass method.
        '     
        <DllImport("unknown")>
        Private Shared Sub registerNatives()
        End Sub

        Shared Sub New()
            registerNatives()
        End Sub

        ''' <summary>
        ''' Don't let anyone instantiate this class </summary>
        Private Sub New()
        End Sub

        Shared in0 As InputStream = Nothing

        ''' <summary>
        ''' The "standard" input stream. This stream is already
        ''' open and ready to supply input data. Typically this stream
        ''' corresponds to keyboard input or another input source specified by
        ''' the host environment or user.
        ''' </summary>
        Public Shared ReadOnly Property [in] As InputStream
            Get
                Return in0
            End Get
        End Property

        Shared out0 As PrintStream = Nothing

        ''' <summary>
        ''' The "standard" output stream. This stream is already
        ''' open and ready to accept output data. Typically this stream
        ''' corresponds to display output or another output destination
        ''' specified by the host environment or user.
        ''' <p>
        ''' For simple stand-alone Java applications, a typical way to write
        ''' a line of output data is:
        ''' <blockquote><pre>
        '''     System.out.println(data)
        ''' </pre></blockquote>
        ''' <p>
        ''' See the <code>println</code> methods in class <code>PrintStream</code>.
        ''' </summary>
        ''' <seealso cref=     java.io.PrintStream#println() </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(boolean) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(char) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(char[]) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(double) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(float) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(int) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(long) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(java.lang.Object) </seealso>
        ''' <seealso cref=     java.io.PrintStream#println(java.lang.String) </seealso>
        Public Shared ReadOnly Property out As PrintStream
            Get
                Return out0
            End Get
        End Property

        Shared err0 As PrintStream = Nothing

        ''' <summary>
        ''' The "standard" error output stream. This stream is already
        ''' open and ready to accept output data.
        ''' <p>
        ''' Typically this stream corresponds to display output or another
        ''' output destination specified by the host environment or user. By
        ''' convention, this output stream is used to display error messages
        ''' or other information that should come to the immediate attention
        ''' of a user even if the principal output stream, the value of the
        ''' variable <code>out</code>, has been redirected to a file or other
        ''' destination that is typically not continuously monitored.
        ''' </summary>
        Public Shared ReadOnly Property err As PrintStream
            Get
                Return err0
            End Get
        End Property

        ''' <summary>
        ''' Reassigns the "standard" input stream.
        ''' 
        ''' <p>First, if there is a security manager, its <code>checkPermission</code>
        ''' method is called with a <code>RuntimePermission("setIO")</code> permission
        '''  to see if it's ok to reassign the "standard" input stream.
        ''' <p>
        ''' </summary>
        ''' <param name="in"> the new standard input stream.
        ''' </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow
        '''        reassigning of the standard input stream.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkPermission </seealso>
        ''' <seealso cref= java.lang.RuntimePermission
        ''' 
        ''' @since   JDK1.1 </seealso>
        Public Sub setIn([in] As InputStream)
            checkIO()
            in0 = [in]
        End Sub

        ''' <summary>
        ''' Reassigns the "standard" output stream.
        ''' 
        ''' <p>First, if there is a security manager, its <code>checkPermission</code>
        ''' method is called with a <code>RuntimePermission("setIO")</code> permission
        '''  to see if it's ok to reassign the "standard" output stream.
        ''' </summary>
        ''' <param name="out"> the new standard output stream
        ''' </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow
        '''        reassigning of the standard output stream.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkPermission </seealso>
        ''' <seealso cref= java.lang.RuntimePermission
        ''' 
        ''' @since   JDK1.1 </seealso>
        Public Sub setOut(out As PrintStream)
            checkIO()
            out0 = out
        End Sub

        ''' <summary>
        ''' Reassigns the "standard" error output stream.
        ''' 
        ''' <p>First, if there is a security manager, its <code>checkPermission</code>
        ''' method is called with a <code>RuntimePermission("setIO")</code> permission
        '''  to see if it's ok to reassign the "standard" error output stream.
        ''' </summary>
        ''' <param name="err"> the new standard error output stream.
        ''' </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its
        '''        <code>checkPermission</code> method doesn't allow
        '''        reassigning of the standard error output stream.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkPermission </seealso>
        ''' <seealso cref= java.lang.RuntimePermission
        ''' 
        ''' @since   JDK1.1 </seealso>
        Public Sub setErr(err As PrintStream)
            checkIO()
            err0 = err
        End Sub

        Private Shared cons As java.io.Console = Nothing

        ''' <summary>
        ''' Returns the unique <seealso cref="java.io.Console Console"/> object associated
        ''' with the current Java virtual machine, if any.
        ''' </summary>
        ''' <returns>  The system console, if any, otherwise <tt>null</tt>.
        ''' 
        ''' @since   1.6 </returns>
        Public Shared Function console() As java.io.Console
            If cons Is Nothing Then
                SyncLock GetType(System)
                    cons = sun.misc.SharedSecrets.javaIOAccess.console()
                End SyncLock
            End If
            Return cons
        End Function

        ''' <summary>
        ''' Returns the channel inherited from the entity that created this
        ''' Java virtual machine.
        ''' 
        ''' <p> This method returns the channel obtained by invoking the
        ''' {@link java.nio.channels.spi.SelectorProvider#inheritedChannel
        ''' inheritedChannel} method of the system-wide default
        ''' <seealso cref="java.nio.channels.spi.SelectorProvider"/> object. </p>
        ''' 
        ''' <p> In addition to the network-oriented channels described in
        ''' {@link java.nio.channels.spi.SelectorProvider#inheritedChannel
        ''' inheritedChannel}, this method may return other kinds of
        ''' channels in the future.
        ''' </summary>
        ''' <returns>  The inherited channel, if any, otherwise <tt>null</tt>.
        ''' </returns>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs
        ''' </exception>
        ''' <exception cref="SecurityException">
        '''          If a security manager is present and it does not
        '''          permit access to the channel.
        ''' 
        ''' @since 1.5 </exception>
        Public Shared Function inheritedChannel() As java.nio.channels.Channel
            Return java.nio.channels.spi.SelectorProvider.provider().inheritedChannel()
        End Function

        Private Shared Sub checkIO()
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("setIO"))
        End Sub

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub setIn0(ByVal [in] As InputStream)
        End Sub
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub setOut0(ByVal out As PrintStream)
        End Sub
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub setErr0(ByVal err As PrintStream)
        End Sub

        ''' <summary>
        ''' Sets the System security.
        ''' 
        ''' <p> If there is a security manager already installed, this method first
        ''' calls the security manager's <code>checkPermission</code> method
        ''' with a <code>RuntimePermission("setSecurityManager")</code>
        ''' permission to ensure it's ok to replace the existing
        ''' security manager.
        ''' This may result in throwing a <code>SecurityException</code>.
        ''' 
        ''' <p> Otherwise, the argument is established as the current
        ''' security manager. If the argument is <code>null</code> and no
        ''' security manager has been established, then no action is taken and
        ''' the method simply returns.
        ''' </summary>
        ''' <param name="s">   the security manager. </param>
        ''' <exception cref="SecurityException">  if the security manager has already
        '''             been set and its <code>checkPermission</code> method
        '''             doesn't allow it to be replaced. </exception>
        ''' <seealso cref= #getSecurityManager </seealso>
        ''' <seealso cref= SecurityManager#checkPermission </seealso>
        ''' <seealso cref= java.lang.RuntimePermission </seealso>
        Public Shared Property securityManager As SecurityManager
            Get
                Return _securityManager
            End Get
            Set(ByVal s As SecurityManager)
                Try
                    s.checkPackageAccess("java.lang")
                Catch e As Exception
                    ' no-op
                End Try

                Dim sm As SecurityManager = _securityManager
                If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("setSecurityManager"))

                If (s IsNot Nothing) AndAlso (s.GetType().classLoader IsNot Nothing) Then
                    java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of SecurityManager))
                End If

                _securityManager = s
            End Set
        End Property

        Shared _securityManager As SecurityManager

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements java.security.PrivilegedAction(Of T)

            Public Overridable Function run() As T Implements PrivilegedAction(Of T).run
                s.GetType().protectionDomain.implies(sun.security.util.SecurityConstants.ALL_PERMISSION)
                Return Nothing
            End Function
        End Class

        ''' <summary>
        ''' Returns the current time in milliseconds.  Note that
        ''' while the unit of time of the return value is a millisecond,
        ''' the granularity of the value depends on the underlying
        ''' operating system and may be larger.  For example, many
        ''' operating systems measure time in units of tens of
        ''' milliseconds.
        ''' 
        ''' <p> See the description of the class <code>Date</code> for
        ''' a discussion of slight discrepancies that may arise between
        ''' "computer time" and coordinated universal time (UTC).
        ''' </summary>
        ''' <returns>  the difference, measured in milliseconds, between
        '''          the current time and midnight, January 1, 1970 UTC. </returns>
        ''' <seealso cref=     java.util.Date </seealso>
        <DllImport("unknown")>
        Public Shared Function currentTimeMillis() As Long
        End Function

        ''' <summary>
        ''' Returns the current value of the running Java Virtual Machine's
        ''' high-resolution time source, in nanoseconds.
        ''' 
        ''' <p>This method can only be used to measure elapsed time and is
        ''' not related to any other notion of system or wall-clock time.
        ''' The value returned represents nanoseconds since some fixed but
        ''' arbitrary <i>origin</i> time (perhaps in the future, so values
        ''' may be negative).  The same origin is used by all invocations of
        ''' this method in an instance of a Java virtual machine; other
        ''' virtual machine instances are likely to use a different origin.
        ''' 
        ''' <p>This method provides nanosecond precision, but not necessarily
        ''' nanosecond resolution (that is, how frequently the value changes)
        ''' - no guarantees are made except that the resolution is at least as
        ''' good as that of <seealso cref="#currentTimeMillis()"/>.
        ''' 
        ''' <p>Differences in successive calls that span greater than
        ''' approximately 292 years (2<sup>63</sup> nanoseconds) will not
        ''' correctly compute elapsed time due to numerical overflow.
        ''' 
        ''' <p>The values returned by this method become meaningful only when
        ''' the difference between two such values, obtained within the same
        ''' instance of a Java virtual machine, is computed.
        ''' 
        ''' <p> For example, to measure how long some code takes to execute:
        '''  <pre> {@code
        ''' long startTime = System.nanoTime();
        ''' // ... the code being measured ...
        ''' long estimatedTime = System.nanoTime() - startTime;}</pre>
        ''' 
        ''' <p>To compare two nanoTime values
        '''  <pre> {@code
        ''' long t0 = System.nanoTime();
        ''' ...
        ''' long t1 = System.nanoTime();}</pre>
        ''' 
        ''' one should use {@code t1 - t0 < 0}, not {@code t1 < t0},
        ''' because of the possibility of numerical overflow.
        ''' </summary>
        ''' <returns> the current value of the running Java Virtual Machine's
        '''         high-resolution time source, in nanoseconds
        ''' @since 1.5 </returns>
        <DllImport("unknown")>
        Public Shared Function nanoTime() As Long
        End Function

        ''' <summary>
        ''' Copies an array from the specified source array, beginning at the
        ''' specified position, to the specified position of the destination array.
        ''' A subsequence of array components are copied from the source
        ''' array referenced by <code>src</code> to the destination array
        ''' referenced by <code>dest</code>. The number of components copied is
        ''' equal to the <code>length</code> argument. The components at
        ''' positions <code>srcPos</code> through
        ''' <code>srcPos+length-1</code> in the source array are copied into
        ''' positions <code>destPos</code> through
        ''' <code>destPos+length-1</code>, respectively, of the destination
        ''' array.
        ''' <p>
        ''' If the <code>src</code> and <code>dest</code> arguments refer to the
        ''' same array object, then the copying is performed as if the
        ''' components at positions <code>srcPos</code> through
        ''' <code>srcPos+length-1</code> were first copied to a temporary
        ''' array with <code>length</code> components and then the contents of
        ''' the temporary array were copied into positions
        ''' <code>destPos</code> through <code>destPos+length-1</code> of the
        ''' destination array.
        ''' <p>
        ''' If <code>dest</code> is <code>null</code>, then a
        ''' <code>NullPointerException</code> is thrown.
        ''' <p>
        ''' If <code>src</code> is <code>null</code>, then a
        ''' <code>NullPointerException</code> is thrown and the destination
        ''' array is not modified.
        ''' <p>
        ''' Otherwise, if any of the following is true, an
        ''' <code>ArrayStoreException</code> is thrown and the destination is
        ''' not modified:
        ''' <ul>
        ''' <li>The <code>src</code> argument refers to an object that is not an
        '''     array.
        ''' <li>The <code>dest</code> argument refers to an object that is not an
        '''     array.
        ''' <li>The <code>src</code> argument and <code>dest</code> argument refer
        '''     to arrays whose component types are different primitive types.
        ''' <li>The <code>src</code> argument refers to an array with a primitive
        '''    component type and the <code>dest</code> argument refers to an array
        '''     with a reference component type.
        ''' <li>The <code>src</code> argument refers to an array with a reference
        '''    component type and the <code>dest</code> argument refers to an array
        '''     with a primitive component type.
        ''' </ul>
        ''' <p>
        ''' Otherwise, if any of the following is true, an
        ''' <code>IndexOutOfBoundsException</code> is
        ''' thrown and the destination is not modified:
        ''' <ul>
        ''' <li>The <code>srcPos</code> argument is negative.
        ''' <li>The <code>destPos</code> argument is negative.
        ''' <li>The <code>length</code> argument is negative.
        ''' <li><code>srcPos+length</code> is greater than
        '''     <code>src.length</code>, the length of the source array.
        ''' <li><code>destPos+length</code> is greater than
        '''     <code>dest.length</code>, the length of the destination array.
        ''' </ul>
        ''' <p>
        ''' Otherwise, if any actual component of the source array from
        ''' position <code>srcPos</code> through
        ''' <code>srcPos+length-1</code> cannot be converted to the component
        ''' type of the destination array by assignment conversion, an
        ''' <code>ArrayStoreException</code> is thrown. In this case, let
        ''' <b><i>k</i></b> be the smallest nonnegative integer less than
        ''' length such that <code>src[srcPos+</code><i>k</i><code>]</code>
        ''' cannot be converted to the component type of the destination
        ''' array; when the exception is thrown, source array components from
        ''' positions <code>srcPos</code> through
        ''' <code>srcPos+</code><i>k</i><code>-1</code>
        ''' will already have been copied to destination array positions
        ''' <code>destPos</code> through
        ''' <code>destPos+</code><i>k</I><code>-1</code> and no other
        ''' positions of the destination array will have been modified.
        ''' (Because of the restrictions already itemized, this
        ''' paragraph effectively applies only to the situation where both
        ''' arrays have component types that are reference types.)
        ''' </summary>
        ''' <param name="src">      the source array. </param>
        ''' <param name="srcPos">   starting position in the source array. </param>
        ''' <param name="dest">     the destination array. </param>
        ''' <param name="destPos">  starting position in the destination data. </param>
        ''' <param name="length">   the number of array elements to be copied. </param>
        ''' <exception cref="IndexOutOfBoundsException">  if copying would cause
        '''               access of data outside array bounds. </exception>
        ''' <exception cref="ArrayStoreException">  if an element in the <code>src</code>
        '''               array could not be stored into the <code>dest</code> array
        '''               because of a type mismatch. </exception>
        ''' <exception cref="NullPointerException"> if either <code>src</code> or
        '''               <code>dest</code> is <code>null</code>. </exception>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Shared Sub arraycopy(ByVal src As Object, ByVal srcPos As Integer, ByVal dest As Object, ByVal destPos As Integer, ByVal length As Integer)
        End Sub

        ''' <summary>
        ''' Returns the same hash code for the given object as
        ''' would be returned by the default method hashCode(),
        ''' whether or not the given object's class overrides
        ''' hashCode().
        ''' The hash code for the null reference is zero.
        ''' </summary>
        ''' <param name="x"> object for which the hashCode is to be calculated </param>
        ''' <returns>  the hashCode
        ''' @since   JDK1.1 </returns>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Shared Function identityHashCode(ByVal x As Object) As Integer
        End Function

        ''' <summary>
        ''' System properties. The following properties are guaranteed to be defined:
        ''' <dl>
        ''' <dt>java.version         <dd>Java version number
        ''' <dt>java.vendor          <dd>Java vendor specific string
        ''' <dt>java.vendor.url      <dd>Java vendor URL
        ''' <dt>java.home            <dd>Java installation directory
        ''' <dt>java.class.version   <dd>Java class version number
        ''' <dt>java.class.path      <dd>Java classpath
        ''' <dt>os.name              <dd>Operating System Name
        ''' <dt>os.arch              <dd>Operating System Architecture
        ''' <dt>os.version           <dd>Operating System Version
        ''' <dt>file.separator       <dd>File separator ("/" on Unix)
        ''' <dt>path.separator       <dd>Path separator (":" on Unix)
        ''' <dt>line.separator       <dd>Line separator ("\n" on Unix)
        ''' <dt>user.name            <dd>User account name
        ''' <dt>user.home            <dd>User home directory
        ''' <dt>user.dir             <dd>User's current working directory
        ''' </dl>
        ''' </summary>

        Private Shared props As java.util.Properties

        <DllImport("unknown")>
        Private Shared Function initProperties(ByVal props As java.util.Properties) As java.util.Properties
        End Function

        ''' <summary>
        ''' Determines the current system properties.
        ''' <p>
        ''' First, if there is a security manager, its
        ''' <code>checkPropertiesAccess</code> method is called with no
        ''' arguments. This may result in a security exception.
        ''' <p>
        ''' The current set of system properties for use by the
        ''' <seealso cref="#getProperty(String)"/> method is returned as a
        ''' <code>Properties</code> object. If there is no current set of
        ''' system properties, a set of system properties is first created and
        ''' initialized. This set of system properties always includes values
        ''' for the following keys:
        ''' <table summary="Shows property keys and associated values">
        ''' <tr><th>Key</th>
        '''     <th>Description of Associated Value</th></tr>
        ''' <tr><td><code>java.version</code></td>
        '''     <td>Java Runtime Environment version</td></tr>
        ''' <tr><td><code>java.vendor</code></td>
        '''     <td>Java Runtime Environment vendor</td></tr>
        ''' <tr><td><code>java.vendor.url</code></td>
        '''     <td>Java vendor URL</td></tr>
        ''' <tr><td><code>java.home</code></td>
        '''     <td>Java installation directory</td></tr>
        ''' <tr><td><code>java.vm.specification.version</code></td>
        '''     <td>Java Virtual Machine specification version</td></tr>
        ''' <tr><td><code>java.vm.specification.vendor</code></td>
        '''     <td>Java Virtual Machine specification vendor</td></tr>
        ''' <tr><td><code>java.vm.specification.name</code></td>
        '''     <td>Java Virtual Machine specification name</td></tr>
        ''' <tr><td><code>java.vm.version</code></td>
        '''     <td>Java Virtual Machine implementation version</td></tr>
        ''' <tr><td><code>java.vm.vendor</code></td>
        '''     <td>Java Virtual Machine implementation vendor</td></tr>
        ''' <tr><td><code>java.vm.name</code></td>
        '''     <td>Java Virtual Machine implementation name</td></tr>
        ''' <tr><td><code>java.specification.version</code></td>
        '''     <td>Java Runtime Environment specification  version</td></tr>
        ''' <tr><td><code>java.specification.vendor</code></td>
        '''     <td>Java Runtime Environment specification  vendor</td></tr>
        ''' <tr><td><code>java.specification.name</code></td>
        '''     <td>Java Runtime Environment specification  name</td></tr>
        ''' <tr><td><code>java.class.version</code></td>
        '''     <td>Java class format version number</td></tr>
        ''' <tr><td><code>java.class.path</code></td>
        '''     <td>Java class path</td></tr>
        ''' <tr><td><code>java.library.path</code></td>
        '''     <td>List of paths to search when loading libraries</td></tr>
        ''' <tr><td><code>java.io.tmpdir</code></td>
        '''     <td>Default temp file path</td></tr>
        ''' <tr><td><code>java.compiler</code></td>
        '''     <td>Name of JIT compiler to use</td></tr>
        ''' <tr><td><code>java.ext.dirs</code></td>
        '''     <td>Path of extension directory or directories
        '''         <b>Deprecated.</b> <i>This property, and the mechanism
        '''            which implements it, may be removed in a future
        '''            release.</i> </td></tr>
        ''' <tr><td><code>os.name</code></td>
        '''     <td>Operating system name</td></tr>
        ''' <tr><td><code>os.arch</code></td>
        '''     <td>Operating system architecture</td></tr>
        ''' <tr><td><code>os.version</code></td>
        '''     <td>Operating system version</td></tr>
        ''' <tr><td><code>file.separator</code></td>
        '''     <td>File separator ("/" on UNIX)</td></tr>
        ''' <tr><td><code>path.separator</code></td>
        '''     <td>Path separator (":" on UNIX)</td></tr>
        ''' <tr><td><code>line.separator</code></td>
        '''     <td>Line separator ("\n" on UNIX)</td></tr>
        ''' <tr><td><code>user.name</code></td>
        '''     <td>User's account name</td></tr>
        ''' <tr><td><code>user.home</code></td>
        '''     <td>User's home directory</td></tr>
        ''' <tr><td><code>user.dir</code></td>
        '''     <td>User's current working directory</td></tr>
        ''' </table>
        ''' <p>
        ''' Multiple paths in a system property value are separated by the path
        ''' separator character of the platform.
        ''' <p>
        ''' Note that even if the security manager does not permit the
        ''' <code>getProperties</code> operation, it may choose to permit the
        ''' <seealso cref="#getProperty(String)"/> operation.
        ''' </summary>
        ''' <returns>     the system properties </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkPropertiesAccess</code> method doesn't allow access
        '''              to the system properties. </exception>
        ''' <seealso cref=        #setProperties </seealso>
        ''' <seealso cref=        java.lang.SecurityException </seealso>
        ''' <seealso cref=        java.lang.SecurityManager#checkPropertiesAccess() </seealso>
        ''' <seealso cref=        java.util.Properties </seealso>
        Public Shared Property properties As java.util.Properties
            Get
                Dim sm As SecurityManager = securityManager
                If sm IsNot Nothing Then sm.checkPropertiesAccess()

                Return props
            End Get
            Set(ByVal props As java.util.Properties)
                Dim sm As SecurityManager = securityManager
                If sm IsNot Nothing Then sm.checkPropertiesAccess()
                If props Is Nothing Then
                    props = New java.util.Properties
                    initProperties(props)
                End If
                System.props = props
            End Set
        End Property

        ''' <summary>
        ''' Returns the system-dependent line separator string.  It always
        ''' returns the same value - the initial value of the {@linkplain
        ''' #getProperty(String) system property} {@code line.separator}.
        ''' 
        ''' <p>On UNIX systems, it returns {@code "\n"}; on Microsoft
        ''' Windows systems it returns {@code "\r\n"}.
        ''' </summary>
        ''' <returns> the system-dependent line separator string
        ''' @since 1.7 </returns>
        Public Shared ReadOnly Property lineSeparator() As String

        ''' <summary>
        ''' Gets the system property indicated by the specified key.
        ''' <p>
        ''' First, if there is a security manager, its
        ''' <code>checkPropertyAccess</code> method is called with the key as
        ''' its argument. This may result in a SecurityException.
        ''' <p>
        ''' If there is no current set of system properties, a set of system
        ''' properties is first created and initialized in the same manner as
        ''' for the <code>getProperties</code> method.
        ''' </summary>
        ''' <param name="key">   the name of the system property. </param>
        ''' <returns>     the string value of the system property,
        '''             or <code>null</code> if there is no property with that key.
        ''' </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkPropertyAccess</code> method doesn't allow
        '''              access to the specified system property. </exception>
        ''' <exception cref="NullPointerException"> if <code>key</code> is
        '''             <code>null</code>. </exception>
        ''' <exception cref="IllegalArgumentException"> if <code>key</code> is empty. </exception>
        ''' <seealso cref=        #setProperty </seealso>
        ''' <seealso cref=        java.lang.SecurityException </seealso>
        ''' <seealso cref=        java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
        ''' <seealso cref=        java.lang.System#getProperties() </seealso>
        Public Shared Function getProperty(ByVal key As String) As String
            checkKey(key)
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPropertyAccess(key)

            Return props.getProperty(key)
        End Function

        ''' <summary>
        ''' Gets the system property indicated by the specified key.
        ''' <p>
        ''' First, if there is a security manager, its
        ''' <code>checkPropertyAccess</code> method is called with the
        ''' <code>key</code> as its argument.
        ''' <p>
        ''' If there is no current set of system properties, a set of system
        ''' properties is first created and initialized in the same manner as
        ''' for the <code>getProperties</code> method.
        ''' </summary>
        ''' <param name="key">   the name of the system property. </param>
        ''' <param name="def">   a default value. </param>
        ''' <returns>     the string value of the system property,
        '''             or the default value if there is no property with that key.
        ''' </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkPropertyAccess</code> method doesn't allow
        '''             access to the specified system property. </exception>
        ''' <exception cref="NullPointerException"> if <code>key</code> is
        '''             <code>null</code>. </exception>
        ''' <exception cref="IllegalArgumentException"> if <code>key</code> is empty. </exception>
        ''' <seealso cref=        #setProperty </seealso>
        ''' <seealso cref=        java.lang.SecurityManager#checkPropertyAccess(java.lang.String) </seealso>
        ''' <seealso cref=        java.lang.System#getProperties() </seealso>
        Public Shared Function getProperty(ByVal key As String, ByVal def As String) As String
            checkKey(key)
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPropertyAccess(key)

            Return props.getProperty(key, def)
        End Function

        ''' <summary>
        ''' Sets the system property indicated by the specified key.
        ''' <p>
        ''' First, if a security manager exists, its
        ''' <code>SecurityManager.checkPermission</code> method
        ''' is called with a <code>PropertyPermission(key, "write")</code>
        ''' permission. This may result in a SecurityException being thrown.
        ''' If no exception is thrown, the specified property is set to the given
        ''' value.
        ''' <p>
        ''' </summary>
        ''' <param name="key">   the name of the system property. </param>
        ''' <param name="value"> the value of the system property. </param>
        ''' <returns>     the previous value of the system property,
        '''             or <code>null</code> if it did not have one.
        ''' </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkPermission</code> method doesn't allow
        '''             setting of the specified property. </exception>
        ''' <exception cref="NullPointerException"> if <code>key</code> or
        '''             <code>value</code> is <code>null</code>. </exception>
        ''' <exception cref="IllegalArgumentException"> if <code>key</code> is empty. </exception>
        ''' <seealso cref=        #getProperty </seealso>
        ''' <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
        ''' <seealso cref=        java.lang.System#getProperty(java.lang.String, java.lang.String) </seealso>
        ''' <seealso cref=        java.util.PropertyPermission </seealso>
        ''' <seealso cref=        SecurityManager#checkPermission
        ''' @since      1.2 </seealso>
        Public Shared Function setProperty(ByVal key As String, ByVal value As String) As String
            checkKey(key)
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPermission(New java.util.PropertyPermission(key, sun.security.util.SecurityConstants.PROPERTY_WRITE_ACTION))

            Return CStr(props.propertyrty(key, value))
        End Function

        ''' <summary>
        ''' Removes the system property indicated by the specified key.
        ''' <p>
        ''' First, if a security manager exists, its
        ''' <code>SecurityManager.checkPermission</code> method
        ''' is called with a <code>PropertyPermission(key, "write")</code>
        ''' permission. This may result in a SecurityException being thrown.
        ''' If no exception is thrown, the specified property is removed.
        ''' <p>
        ''' </summary>
        ''' <param name="key">   the name of the system property to be removed. </param>
        ''' <returns>     the previous string value of the system property,
        '''             or <code>null</code> if there was no property with that key.
        ''' </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkPropertyAccess</code> method doesn't allow
        '''              access to the specified system property. </exception>
        ''' <exception cref="NullPointerException"> if <code>key</code> is
        '''             <code>null</code>. </exception>
        ''' <exception cref="IllegalArgumentException"> if <code>key</code> is empty. </exception>
        ''' <seealso cref=        #getProperty </seealso>
        ''' <seealso cref=        #setProperty </seealso>
        ''' <seealso cref=        java.util.Properties </seealso>
        ''' <seealso cref=        java.lang.SecurityException </seealso>
        ''' <seealso cref=        java.lang.SecurityManager#checkPropertiesAccess()
        ''' @since 1.5 </seealso>
        Public Shared Function clearProperty(ByVal key As String) As String
            checkKey(key)
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPermission(New java.util.PropertyPermission(key, "write"))

            Return CStr(props.remove(key))
        End Function

        Private Shared Sub checkKey(ByVal key As String)
            If key Is Nothing Then Throw New NullPointerException("key can't be null")
            If key.Equals("") Then Throw New IllegalArgumentException("key can't be empty")
        End Sub

        ''' <summary>
        ''' Gets the value of the specified environment variable. An
        ''' environment variable is a system-dependent external named
        ''' value.
        ''' 
        ''' <p>If a security manager exists, its
        ''' <seealso cref="SecurityManager#checkPermission checkPermission"/>
        ''' method is called with a
        ''' <code><seealso cref="RuntimePermission"/>("getenv."+name)</code>
        ''' permission.  This may result in a <seealso cref="SecurityException"/>
        ''' being thrown.  If no exception is thrown the value of the
        ''' variable <code>name</code> is returned.
        ''' 
        ''' <p><a name="EnvironmentVSSystemProperties"><i>System
        ''' properties</i> and <i>environment variables</i></a> are both
        ''' conceptually mappings between names and values.  Both
        ''' mechanisms can be used to pass user-defined information to a
        ''' Java process.  Environment variables have a more global effect,
        ''' because they are visible to all descendants of the process
        ''' which defines them, not just the immediate Java subprocess.
        ''' They can have subtly different semantics, such as case
        ''' insensitivity, on different operating systems.  For these
        ''' reasons, environment variables are more likely to have
        ''' unintended side effects.  It is best to use system properties
        ''' where possible.  Environment variables should be used when a
        ''' global effect is desired, or when an external system interface
        ''' requires an environment variable (such as <code>PATH</code>).
        ''' 
        ''' <p>On UNIX systems the alphabetic case of <code>name</code> is
        ''' typically significant, while on Microsoft Windows systems it is
        ''' typically not.  For example, the expression
        ''' <code>System.getenv("FOO").equals(System.getenv("foo"))</code>
        ''' is likely to be true on Microsoft Windows.
        ''' </summary>
        ''' <param name="name"> the name of the environment variable </param>
        ''' <returns> the string value of the variable, or <code>null</code>
        '''         if the variable is not defined in the system environment </returns>
        ''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code> </exception>
        ''' <exception cref="SecurityException">
        '''         if a security manager exists and its
        '''         <seealso cref="SecurityManager#checkPermission checkPermission"/>
        '''         method doesn't allow access to the environment variable
        '''         <code>name</code> </exception>
        ''' <seealso cref=    #getenv() </seealso>
        ''' <seealso cref=    ProcessBuilder#environment() </seealso>
        Public Shared Function getenv(ByVal name As String) As String
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("getenv." & name))

            Return ProcessEnvironment.getenv(name)
        End Function


        ''' <summary>
        ''' Returns an unmodifiable string map view of the current system environment.
        ''' The environment is a system-dependent mapping from names to
        ''' values which is passed from parent to child processes.
        ''' 
        ''' <p>If the system does not support environment variables, an
        ''' empty map is returned.
        ''' 
        ''' <p>The returned map will never contain null keys or values.
        ''' Attempting to query the presence of a null key or value will
        ''' throw a <seealso cref="NullPointerException"/>.  Attempting to query
        ''' the presence of a key or value which is not of type
        ''' <seealso cref="String"/> will throw a <seealso cref="ClassCastException"/>.
        ''' 
        ''' <p>The returned map and its collection views may not obey the
        ''' general contract of the <seealso cref="Object#equals"/> and
        ''' <seealso cref="Object#hashCode"/> methods.
        ''' 
        ''' <p>The returned map is typically case-sensitive on all platforms.
        ''' 
        ''' <p>If a security manager exists, its
        ''' <seealso cref="SecurityManager#checkPermission checkPermission"/>
        ''' method is called with a
        ''' <code><seealso cref="RuntimePermission"/>("getenv.*")</code>
        ''' permission.  This may result in a <seealso cref="SecurityException"/> being
        ''' thrown.
        ''' 
        ''' <p>When passing information to a Java subprocess,
        ''' <a href=#EnvironmentVSSystemProperties>system properties</a>
        ''' are generally preferred over environment variables.
        ''' </summary>
        ''' <returns> the environment as a map of variable names to values </returns>
        ''' <exception cref="SecurityException">
        '''         if a security manager exists and its
        '''         <seealso cref="SecurityManager#checkPermission checkPermission"/>
        '''         method doesn't allow access to the process environment </exception>
        ''' <seealso cref=    #getenv(String) </seealso>
        ''' <seealso cref=    ProcessBuilder#environment()
        ''' @since  1.5 </seealso>
        Public Shared Function getenv() As IDictionary(Of String, String)
            Dim sm As SecurityManager = securityManager
            If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("getenv.*"))

            Return ProcessEnvironment.getenv()
        End Function

        ''' <summary>
        ''' Terminates the currently running Java Virtual Machine. The
        ''' argument serves as a status code; by convention, a nonzero status
        ''' code indicates abnormal termination.
        ''' <p>
        ''' This method calls the <code>exit</code> method in class
        ''' <code>Runtime</code>. This method never returns normally.
        ''' <p>
        ''' The call <code>System.exit(n)</code> is effectively equivalent to
        ''' the call:
        ''' <blockquote><pre>
        ''' Runtime.getRuntime().exit(n)
        ''' </pre></blockquote>
        ''' </summary>
        ''' <param name="status">   exit status. </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its <code>checkExit</code>
        '''        method doesn't allow exit with the specified status. </exception>
        ''' <seealso cref=        java.lang.Runtime#exit(int) </seealso>
        Public Shared Sub [exit](ByVal status As Integer)
            Call App.Exit(status)
        End Sub

        ''' <summary>
        ''' Runs the garbage collector.
        ''' <p>
        ''' Calling the <code>gc</code> method suggests that the Java Virtual
        ''' Machine expend effort toward recycling unused objects in order to
        ''' make the memory they currently occupy available for quick reuse.
        ''' When control returns from the method call, the Java Virtual
        ''' Machine has made a best effort to reclaim space from all discarded
        ''' objects.
        ''' <p>
        ''' The call <code>System.gc()</code> is effectively equivalent to the
        ''' call:
        ''' <blockquote><pre>
        ''' Runtime.getRuntime().gc()
        ''' </pre></blockquote>
        ''' </summary>
        ''' <seealso cref=     java.lang.Runtime#gc() </seealso>
        Public Shared Sub gc()
            Call Global.System.GC.Collect()
        End Sub

        ''' <summary>
        ''' Runs the finalization methods of any objects pending finalization.
        ''' <p>
        ''' Calling this method suggests that the Java Virtual Machine expend
        ''' effort toward running the <code>finalize</code> methods of objects
        ''' that have been found to be discarded but whose <code>finalize</code>
        ''' methods have not yet been run. When control returns from the
        ''' method call, the Java Virtual Machine has made a best effort to
        ''' complete all outstanding finalizations.
        ''' <p>
        ''' The call <code>System.runFinalization()</code> is effectively
        ''' equivalent to the call:
        ''' <blockquote><pre>
        ''' Runtime.getRuntime().runFinalization()
        ''' </pre></blockquote>
        ''' </summary>
        ''' <seealso cref=     java.lang.Runtime#runFinalization() </seealso>
        Public Shared Sub runFinalization()
            Runtime.runtime.runFinalization()
        End Sub

        ''' <summary>
        ''' Enable or disable finalization on exit; doing so specifies that the
        ''' finalizers of all objects that have finalizers that have not yet been
        ''' automatically invoked are to be run before the Java runtime exits.
        ''' By default, finalization on exit is disabled.
        ''' 
        ''' <p>If there is a security manager,
        ''' its <code>checkExit</code> method is first called
        ''' with 0 as its argument to ensure the exit is allowed.
        ''' This could result in a SecurityException.
        ''' </summary>
        ''' @deprecated  This method is inherently unsafe.  It may result in
        '''      finalizers being called on live objects while other threads are
        '''      concurrently manipulating those objects, resulting in erratic
        '''      behavior or deadlock. 
        ''' <param name="value"> indicating enabling or disabling of finalization </param>
        ''' <exception cref="SecurityException">
        '''        if a security manager exists and its <code>checkExit</code>
        '''        method doesn't allow the exit.
        ''' </exception>
        ''' <seealso cref=     java.lang.Runtime#exit(int) </seealso>
        ''' <seealso cref=     java.lang.Runtime#gc() </seealso>
        ''' <seealso cref=     java.lang.SecurityManager#checkExit(int)
        ''' @since   JDK1.1 </seealso>
        <Obsolete(" This method is inherently unsafe.  It may result in")>
        Public Shared Sub runFinalizersOnExit(ByVal value As Boolean)
            Runtime.runFinalizersOnExit(value)
        End Sub

        ''' <summary>
        ''' Loads the native library specified by the filename argument.  The filename
        ''' argument must be an absolute path name.
        ''' 
        ''' If the filename argument, when stripped of any platform-specific library
        ''' prefix, path, and file extension, indicates a library whose name is,
        ''' for example, L, and a native library called L is statically linked
        ''' with the VM, then the JNI_OnLoad_L function exported by the library
        ''' is invoked rather than attempting to load a dynamic library.
        ''' A filename matching the argument does not have to exist in the
        ''' file system.
        ''' See the JNI Specification for more details.
        ''' 
        ''' Otherwise, the filename argument is mapped to a native library image in
        ''' an implementation-dependent manner.
        ''' 
        ''' <p>
        ''' The call <code>System.load(name)</code> is effectively equivalent
        ''' to the call:
        ''' <blockquote><pre>
        ''' Runtime.getRuntime().load(name)
        ''' </pre></blockquote>
        ''' </summary>
        ''' <param name="filename">   the file to load. </param>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkLink</code> method doesn't allow
        '''             loading of the specified dynamic library </exception>
        ''' <exception cref="UnsatisfiedLinkError">  if either the filename is not an
        '''             absolute path name, the native library is not statically
        '''             linked with the VM, or the library cannot be mapped to
        '''             a native library image by the host system. </exception>
        ''' <exception cref="NullPointerException"> if <code>filename</code> is
        '''             <code>null</code> </exception>
        ''' <seealso cref=        java.lang.Runtime#load(java.lang.String) </seealso>
        ''' <seealso cref=        java.lang.SecurityManager#checkLink(java.lang.String) </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared Sub load(ByVal filename As String)
            java.lang.Runtime.runtime.load0(sun.reflect.Reflection.callerClass, filename)
        End Sub

        ''' <summary>
        ''' Loads the native library specified by the <code>libname</code>
        ''' argument.  The <code>libname</code> argument must not contain any platform
        ''' specific prefix, file extension or path. If a native library
        ''' called <code>libname</code> is statically linked with the VM, then the
        ''' JNI_OnLoad_<code>libname</code> function exported by the library is invoked.
        ''' See the JNI Specification for more details.
        ''' 
        ''' Otherwise, the libname argument is loaded from a system library
        ''' location and mapped to a native library image in an implementation-
        ''' dependent manner.
        ''' <p>
        ''' The call <code>System.loadLibrary(name)</code> is effectively
        ''' equivalent to the call
        ''' <blockquote><pre>
        ''' Runtime.getRuntime().loadLibrary(name)
        ''' </pre></blockquote>
        ''' </summary>
        ''' <param name="libname">   the name of the library. </param>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''             <code>checkLink</code> method doesn't allow
        '''             loading of the specified dynamic library </exception>
        ''' <exception cref="UnsatisfiedLinkError"> if either the libname argument
        '''             contains a file path, the native library is not statically
        '''             linked with the VM,  or the library cannot be mapped to a
        '''             native library image by the host system. </exception>
        ''' <exception cref="NullPointerException"> if <code>libname</code> is
        '''             <code>null</code> </exception>
        ''' <seealso cref=        java.lang.Runtime#loadLibrary(java.lang.String) </seealso>
        ''' <seealso cref=        java.lang.SecurityManager#checkLink(java.lang.String) </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Shared Sub loadLibrary(ByVal libname As String)
            Runtime.runtime.loadLibrary0(sun.reflect.Reflection.callerClass, libname)
        End Sub

        ''' <summary>
        ''' Maps a library name into a platform-specific string representing
        ''' a native library.
        ''' </summary>
        ''' <param name="libname"> the name of the library. </param>
        ''' <returns>     a platform-dependent native library name. </returns>
        ''' <exception cref="NullPointerException"> if <code>libname</code> is
        '''             <code>null</code> </exception>
        ''' <seealso cref=        java.lang.System#loadLibrary(java.lang.String) </seealso>
        ''' <seealso cref=        java.lang.ClassLoader#findLibrary(java.lang.String)
        ''' @since      1.2 </seealso>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Public Shared Function mapLibraryName(ByVal libname As String) As String
        End Function

        ''' <summary>
        ''' Create PrintStream for stdout/err based on encoding.
        ''' </summary>
        Private Shared Function newPrintStream(ByVal fos As FileOutputStream, ByVal enc As String) As PrintStream
            If enc IsNot Nothing Then
                Try
                    Return New PrintStream(New BufferedOutputStream(fos, 128), True, enc)
                Catch uee As UnsupportedEncodingException
                End Try
            End If
            Return New PrintStream(New BufferedOutputStream(fos, 128), True)
        End Function


        ''' <summary>
        ''' Initialize the system class.  Called after thread initialization.
        ''' </summary>
        Private Shared Sub initializeSystemClass()

            ' VM might invoke JNU_NewStringPlatform() to set those encoding
            ' sensitive properties (user.home, user.name, boot.class.path, etc.)
            ' during "props" initialization, in which it may need access, via
            ' System.getProperty(), to the related system encoding property that
            ' have been initialized (put into "props") at early stage of the
            ' initialization. So make sure the "props" is available at the
            ' very beginning of the initialization and all system properties to
            ' be put into it directly.
            props = New java.util.Properties
            initProperties(props) ' initialized by the VM

            ' There are certain system configurations that may be controlled by
            ' VM options such as the maximum amount of direct memory and
            ' Integer cache size used to support the object identity semantics
            ' of autoboxing.  Typically, the library will obtain these values
            ' from the properties set by the VM.  If the properties are for
            ' internal implementation use only, these properties should be
            ' removed from the system properties.
            '
            ' See java.lang. java.lang.[Integer].IntegerCache and the
            ' sun.misc.VM.saveAndRemoveProperties method for example.
            '
            ' Save a private copy of the system properties object that
            ' can only be accessed by the internal implementation.  Remove
            ' certain system properties that are not intended for public access.
            sun.misc.VM.saveAndRemoveProperties(props)


            _lineSeparator = props.getProperty("line.separator")
            sun.misc.Version.init()

            Dim fdIn As New FileInputStream(FileDescriptor.in)
            Dim fdOut As New FileOutputStream(FileDescriptor.out)
            Dim fdErr As New FileOutputStream(FileDescriptor.err)
            in0 = New BufferedInputStream(fdIn)
            out0 = newPrintStream(fdOut, props.getProperty("sun.stdout.encoding"))
            err0 = newPrintStream(fdErr, props.getProperty("sun.stderr.encoding"))

            ' Load the zip library now in order to keep java.util.zip.ZipFile
            ' from trying to use itself to load this library later.
            loadLibrary("zip")

            ' Setup Java signal handlers for HUP, TERM, and INT (where available).
            Terminator.setup()

            ' Initialize any miscellenous operating system settings that need to be
            ' set for the class libraries. Currently this is no-op everywhere except
            ' for Windows where the process-wide error mode is set before the java.io
            ' classes are used.
            sun.misc.VM.initializeOSEnvironment()

            ' The main thread is not added to its thread group in the same
            ' way as other threads; we must do it ourselves here.
            Dim current As Thread = Thread.currentThread
            current.threadGroup.add(current)

            ' register shared secrets
            javaLangAccessess()

            ' Subsystems that are invoked during initialization can invoke
            ' sun.misc.VM.isBooted() in order to avoid doing things that should
            ' wait until the application class loader has been set up.
            ' IMPORTANT: Ensure that this remains the last initialization action!
            sun.misc.VM.booted()
        End Sub

        Private Shared Sub setJavaLangAccess()
            ' Allow privileged classes outside of java.lang
            '			sun.misc.SharedSecrets.setJavaLangAccess(New sun.misc.JavaLangAccess()
            '		{
            '			public sun.reflect.ConstantPool getConstantPool(Class klass)
            '			{
            '				Return klass.getConstantPool();
            '			}
            '			public boolean casAnnotationType(Class klass, AnnotationType oldType, AnnotationType newType)
            '			{
            '				Return klass.casAnnotationType(oldType, newType);
            '			}
            '			public AnnotationType getAnnotationType(Class klass)
            '			{
            '				Return klass.getAnnotationType();
            '			}
            '			public Map<Class, Annotation> getDeclaredAnnotationMap(Class klass)
            '			{
            '				Return klass.getDeclaredAnnotationMap();
            '			}
            '			public byte[] getRawClassAnnotations(Class klass)
            '			{
            '				Return klass.getRawAnnotations();
            '			}
            '			public byte[] getRawClassTypeAnnotations(Class klass)
            '			{
            '				Return klass.getRawTypeAnnotations();
            '			}
            '			public byte[] getRawExecutableTypeAnnotations(Executable executable)
            '			{
            '				Return Class.getExecutableTypeAnnotationBytes(executable);
            '			}
            '			public <E extends Enum<E>> E[] getEnumConstantsShared(Class klass)
            '			{
            '				Return klass.getEnumConstantsShared();
            '			}
            '			public  Sub  blockedOn(Thread t, Interruptible b)
            '			{
            '				t.blockedOn(b);
            '			}
            '			public  Sub  registerShutdownHook(int slot, boolean registerShutdownInProgress, Runnable hook)
            '			{
            '				Shutdown.add(slot, registerShutdownInProgress, hook);
            '			}
            '			public int getStackTraceDepth(Throwable t)
            '			{
            '				Return t.getStackTraceDepth();
            '			}
            '			public StackTraceElement getStackTraceElement(Throwable t, int i)
            '			{
            '				Return t.getStackTraceElement(i);
            '			}
            '			public String newStringUnsafe(char[] chars)
            '			{
            '				Return New String(chars, True);
            '			}
            '			public Thread newThreadWithAcc(Runnable target, AccessControlContext acc)
            '			{
            '				Return New Thread(target, acc);
            '			}
            '			public  Sub  invokeFinalize(Object o) throws Throwable
            '			{
            '				o.finalize();
            '			}
            '		});
        End Sub
    End Class

End Namespace
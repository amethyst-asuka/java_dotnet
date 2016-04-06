Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is used to create operating system processes.
	''' 
	''' <p>Each {@code ProcessBuilder} instance manages a collection
	''' of process attributes.  The <seealso cref="#start()"/> method creates a new
	''' <seealso cref="Process"/> instance with those attributes.  The {@link
	''' #start()} method can be invoked repeatedly from the same instance
	''' to create new subprocesses with identical or related attributes.
	''' 
	''' <p>Each process builder manages these process attributes:
	''' 
	''' <ul>
	''' 
	''' <li>a <i>command</i>, a list of strings which signifies the
	''' external program file to be invoked and its arguments, if any.
	''' Which string lists represent a valid operating system command is
	''' system-dependent.  For example, it is common for each conceptual
	''' argument to be an element in this list, but there are operating
	''' systems where programs are expected to tokenize command line
	''' strings themselves - on such a system a Java implementation might
	''' require commands to contain exactly two elements.
	''' 
	''' <li>an <i>environment</i>, which is a system-dependent mapping from
	''' <i>variables</i> to <i>values</i>.  The initial value is a copy of
	''' the environment of the current process (see <seealso cref="System#getenv()"/>).
	''' 
	''' <li>a <i>working directory</i>.  The default value is the current
	''' working directory of the current process, usually the directory
	''' named by the system property {@code user.dir}.
	''' 
	''' <li><a name="redirect-input">a source of <i>standard input</i></a>.
	''' By default, the subprocess reads input from a pipe.  Java code
	''' can access this pipe via the output stream returned by
	''' <seealso cref="Process#getOutputStream()"/>.  However, standard input may
	''' be redirected to another source using
	''' <seealso cref="#redirectInput(Redirect) redirectInput"/>.
	''' In this case, <seealso cref="Process#getOutputStream()"/> will return a
	''' <i>null output stream</i>, for which:
	''' 
	''' <ul>
	''' <li>the <seealso cref="OutputStream#write(int) write"/> methods always
	''' throw {@code IOException}
	''' <li>the <seealso cref="OutputStream#close() close"/> method does nothing
	''' </ul>
	''' 
	''' <li><a name="redirect-output">a destination for <i>standard output</i>
	''' and <i>standard error</i></a>.  By default, the subprocess writes standard
	''' output and standard error to pipes.  Java code can access these pipes
	''' via the input streams returned by <seealso cref="Process#getInputStream()"/> and
	''' <seealso cref="Process#getErrorStream()"/>.  However, standard output and
	''' standard error may be redirected to other destinations using
	''' <seealso cref="#redirectOutput(Redirect) redirectOutput"/> and
	''' <seealso cref="#redirectError(Redirect) redirectError"/>.
	''' In this case, <seealso cref="Process#getInputStream()"/> and/or
	''' <seealso cref="Process#getErrorStream()"/> will return a <i>null input
	''' stream</i>, for which:
	''' 
	''' <ul>
	''' <li>the <seealso cref="InputStream#read() read"/> methods always return
	''' {@code -1}
	''' <li>the <seealso cref="InputStream#available() available"/> method always returns
	''' {@code 0}
	''' <li>the <seealso cref="InputStream#close() close"/> method does nothing
	''' </ul>
	''' 
	''' <li>a <i>redirectErrorStream</i> property.  Initially, this property
	''' is {@code false}, meaning that the standard output and error
	''' output of a subprocess are sent to two separate streams, which can
	''' be accessed using the <seealso cref="Process#getInputStream()"/> and {@link
	''' Process#getErrorStream()} methods.
	''' 
	''' <p>If the value is set to {@code true}, then:
	''' 
	''' <ul>
	''' <li>standard error is merged with the standard output and always sent
	''' to the same destination (this makes it easier to correlate error
	''' messages with the corresponding output)
	''' <li>the common destination of standard error and standard output can be
	''' redirected using
	''' <seealso cref="#redirectOutput(Redirect) redirectOutput"/>
	''' <li>any redirection set by the
	''' <seealso cref="#redirectError(Redirect) redirectError"/>
	''' method is ignored when creating a subprocess
	''' <li>the stream returned from <seealso cref="Process#getErrorStream()"/> will
	''' always be a <a href="#redirect-output">null input stream</a>
	''' </ul>
	''' 
	''' </ul>
	''' 
	''' <p>Modifying a process builder's attributes will affect processes
	''' subsequently started by that object's <seealso cref="#start()"/> method, but
	''' will never affect previously started processes or the Java process
	''' itself.
	''' 
	''' <p>Most error checking is performed by the <seealso cref="#start()"/> method.
	''' It is possible to modify the state of an object so that {@link
	''' #start()} will fail.  For example, setting the command attribute to
	''' an empty list will not throw an exception unless <seealso cref="#start()"/>
	''' is invoked.
	''' 
	''' <p><strong>Note that this class is not synchronized.</strong>
	''' If multiple threads access a {@code ProcessBuilder} instance
	''' concurrently, and at least one of the threads modifies one of the
	''' attributes structurally, it <i>must</i> be synchronized externally.
	''' 
	''' <p>Starting a new process which uses the default working directory
	''' and environment is easy:
	''' 
	''' <pre> {@code
	''' Process p = new ProcessBuilder("myCommand", "myArg").start();
	''' }</pre>
	''' 
	''' <p>Here is an example that starts a process with a modified working
	''' directory and environment, and redirects standard output and error
	''' to be appended to a log file:
	''' 
	''' <pre> {@code
	''' ProcessBuilder pb =
	'''   new ProcessBuilder("myCommand", "myArg1", "myArg2");
	''' Map<String, String> env = pb.environment();
	''' env.put("VAR1", "myValue");
	''' env.remove("OTHERVAR");
	''' env.put("VAR2", env.get("VAR1") + "suffix");
	''' pb.directory(new File("myDir"));
	''' File log = new File("log");
	''' pb.redirectErrorStream(true);
	''' pb.redirectOutput(Redirect.appendTo(log));
	''' Process p = pb.start();
	''' assert pb.redirectInput() == Redirect.PIPE;
	''' assert pb.redirectOutput().file() == log;
	''' assert p.getInputStream().read() == -1;
	''' }</pre>
	''' 
	''' <p>To start a process with an explicit set of environment
	''' variables, first call <seealso cref="java.util.Map#clear() Map.clear()"/>
	''' before adding environment variables.
	''' 
	''' @author Martin Buchholz
	''' @since 1.5
	''' </summary>

	Public NotInheritable Class ProcessBuilder
		Private command_Renamed As IList(Of String)
		Private directory_Renamed As java.io.File
		Private environment_Renamed As IDictionary(Of String, String)
		Private redirectErrorStream_Renamed As Boolean
		Private redirects_Renamed As Redirect()

		''' <summary>
		''' Constructs a process builder with the specified operating
		''' system program and arguments.  This constructor does <i>not</i>
		''' make a copy of the {@code command} list.  Subsequent
		''' updates to the list will be reflected in the state of the
		''' process builder.  It is not checked whether
		''' {@code command} corresponds to a valid operating system
		''' command.
		''' </summary>
		''' <param name="command"> the list containing the program and its arguments </param>
		''' <exception cref="NullPointerException"> if the argument is null </exception>
		Public Sub New(  command As IList(Of String))
			If command Is Nothing Then Throw New NullPointerException
			Me.command_Renamed = command
		End Sub

		''' <summary>
		''' Constructs a process builder with the specified operating
		''' system program and arguments.  This is a convenience
		''' constructor that sets the process builder's command to a string
		''' list containing the same strings as the {@code command}
		''' array, in the same order.  It is not checked whether
		''' {@code command} corresponds to a valid operating system
		''' command.
		''' </summary>
		''' <param name="command"> a string array containing the program and its arguments </param>
		Public Sub New(ParamArray   command As String())
			Me.command_Renamed = New List(Of )(command.Length)
			For Each arg As String In command
				Me.command_Renamed.Add(arg)
			Next arg
		End Sub

		''' <summary>
		''' Sets this process builder's operating system program and
		''' arguments.  This method does <i>not</i> make a copy of the
		''' {@code command} list.  Subsequent updates to the list will
		''' be reflected in the state of the process builder.  It is not
		''' checked whether {@code command} corresponds to a valid
		''' operating system command.
		''' </summary>
		''' <param name="command"> the list containing the program and its arguments </param>
		''' <returns> this process builder
		''' </returns>
		''' <exception cref="NullPointerException"> if the argument is null </exception>
		Public Function command(  command_Renamed As IList(Of String)) As ProcessBuilder
			If command_Renamed Is Nothing Then Throw New NullPointerException
			Me.command_Renamed = command_Renamed
			Return Me
		End Function

		''' <summary>
		''' Sets this process builder's operating system program and
		''' arguments.  This is a convenience method that sets the command
		''' to a string list containing the same strings as the
		''' {@code command} array, in the same order.  It is not
		''' checked whether {@code command} corresponds to a valid
		''' operating system command.
		''' </summary>
		''' <param name="command"> a string array containing the program and its arguments </param>
		''' <returns> this process builder </returns>
		Public Function command(ParamArray   command_Renamed As String()) As ProcessBuilder
			Me.command_Renamed = New List(Of )(command_Renamed.Length)
			For Each arg As String In command_Renamed
				Me.command_Renamed.Add(arg)
			Next arg
			Return Me
		End Function

		''' <summary>
		''' Returns this process builder's operating system program and
		''' arguments.  The returned list is <i>not</i> a copy.  Subsequent
		''' updates to the list will be reflected in the state of this
		''' process builder.
		''' </summary>
		''' <returns> this process builder's program and its arguments </returns>
		Public Function command() As IList(Of String)
			Return command_Renamed
		End Function

		''' <summary>
		''' Returns a string map view of this process builder's environment.
		''' 
		''' Whenever a process builder is created, the environment is
		''' initialized to a copy of the current process environment (see
		''' <seealso cref="System#getenv()"/>).  Subprocesses subsequently started by
		''' this object's <seealso cref="#start()"/> method will use this map as
		''' their environment.
		''' 
		''' <p>The returned object may be modified using ordinary {@link
		''' java.util.Map Map} operations.  These modifications will be
		''' visible to subprocesses started via the <seealso cref="#start()"/>
		''' method.  Two {@code ProcessBuilder} instances always
		''' contain independent process environments, so changes to the
		''' returned map will never be reflected in any other
		''' {@code ProcessBuilder} instance or the values returned by
		''' <seealso cref="System#getenv System.getenv"/>.
		''' 
		''' <p>If the system does not support environment variables, an
		''' empty map is returned.
		''' 
		''' <p>The returned map does not permit null keys or values.
		''' Attempting to insert or query the presence of a null key or
		''' value will throw a <seealso cref="NullPointerException"/>.
		''' Attempting to query the presence of a key or value which is not
		''' of type <seealso cref="String"/> will throw a <seealso cref="ClassCastException"/>.
		''' 
		''' <p>The behavior of the returned map is system-dependent.  A
		''' system may not allow modifications to environment variables or
		''' may forbid certain variable names or values.  For this reason,
		''' attempts to modify the map may fail with
		''' <seealso cref="UnsupportedOperationException"/> or
		''' <seealso cref="IllegalArgumentException"/>
		''' if the modification is not permitted by the operating system.
		''' 
		''' <p>Since the external format of environment variable names and
		''' values is system-dependent, there may not be a one-to-one
		''' mapping between them and Java's Unicode strings.  Nevertheless,
		''' the map is implemented in such a way that environment variables
		''' which are not modified by Java code will have an unmodified
		''' native representation in the subprocess.
		''' 
		''' <p>The returned map and its collection views may not obey the
		''' general contract of the <seealso cref="Object#equals"/> and
		''' <seealso cref="Object#hashCode"/> methods.
		''' 
		''' <p>The returned map is typically case-sensitive on all platforms.
		''' 
		''' <p>If a security manager exists, its
		''' <seealso cref="SecurityManager#checkPermission checkPermission"/> method
		''' is called with a
		''' <seealso cref="RuntimePermission"/>{@code ("getenv.*")} permission.
		''' This may result in a <seealso cref="SecurityException"/> being thrown.
		''' 
		''' <p>When passing information to a Java subprocess,
		''' <a href=System.html#EnvironmentVSSystemProperties>system properties</a>
		''' are generally preferred over environment variables.
		''' </summary>
		''' <returns> this process builder's environment
		''' </returns>
		''' <exception cref="SecurityException">
		'''         if a security manager exists and its
		'''         <seealso cref="SecurityManager#checkPermission checkPermission"/>
		'''         method doesn't allow access to the process environment
		''' </exception>
		''' <seealso cref=    Runtime#exec(String[],String[],java.io.File) </seealso>
		''' <seealso cref=    System#getenv() </seealso>
		Public Function environment() As IDictionary(Of String, String)
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(New RuntimePermission("getenv.*"))

			If environment_Renamed Is Nothing Then environment_Renamed = ProcessEnvironment.environment()

			Debug.Assert(environment_Renamed IsNot Nothing)

			Return environment_Renamed
		End Function

		' Only for use by Runtime.exec(...envp...)
		Friend Function environment(  envp As String()) As ProcessBuilder
			Debug.Assert(environment_Renamed Is Nothing)
			If envp IsNot Nothing Then
				environment_Renamed = ProcessEnvironment.emptyEnvironment(envp.Length)
				Debug.Assert(environment_Renamed IsNot Nothing)

				For Each envstring As String In envp
					' Before 1.5, we blindly passed invalid envstrings
					' to the child process.
					' We would like to throw an exception, but do not,
					' for compatibility with old broken code.

					' Silently discard any trailing junk.
					If envstring.IndexOf(CInt(Fix(ChrW(&H0000)))) <> -1 Then envstring = envstring.replaceFirst(ChrW(&H0000).ToString() & ".*", "")

					Dim eqlsign As Integer = envstring.IndexOf("="c, ProcessEnvironment.MIN_NAME_LENGTH)
					' Silently ignore envstrings lacking the required `='.
					If eqlsign <> -1 Then environment_Renamed(envstring.Substring(0,eqlsign)) = envstring.Substring(eqlsign+1)
				Next envstring
			End If
			Return Me
		End Function

		''' <summary>
		''' Returns this process builder's working directory.
		''' 
		''' Subprocesses subsequently started by this object's {@link
		''' #start()} method will use this as their working directory.
		''' The returned value may be {@code null} -- this means to use
		''' the working directory of the current Java process, usually the
		''' directory named by the system property {@code user.dir},
		''' as the working directory of the child process.
		''' </summary>
		''' <returns> this process builder's working directory </returns>
		Public Function directory() As java.io.File
			Return directory_Renamed
		End Function

		''' <summary>
		''' Sets this process builder's working directory.
		''' 
		''' Subprocesses subsequently started by this object's {@link
		''' #start()} method will use this as their working directory.
		''' The argument may be {@code null} -- this means to use the
		''' working directory of the current Java process, usually the
		''' directory named by the system property {@code user.dir},
		''' as the working directory of the child process.
		''' </summary>
		''' <param name="directory"> the new working directory </param>
		''' <returns> this process builder </returns>
		Public Function directory(  directory_Renamed As java.io.File) As ProcessBuilder
			Me.directory_Renamed = directory_Renamed
			Return Me
		End Function

		' ---------------- I/O Redirection ----------------

		''' <summary>
		''' Implements a <a href="#redirect-output">null input stream</a>.
		''' </summary>
		Friend Class NullInputStream
			Inherits java.io.InputStream

			Friend Shared ReadOnly INSTANCE As New NullInputStream
			Private Sub New()
			End Sub
			Public Overrides Function read() As Integer
				Return -1
			End Function
			Public Overrides Function available() As Integer
				Return 0
			End Function
		End Class

		''' <summary>
		''' Implements a <a href="#redirect-input">null output stream</a>.
		''' </summary>
		Friend Class NullOutputStream
			Inherits java.io.OutputStream

			Friend Shared ReadOnly INSTANCE As New NullOutputStream
			Private Sub New()
			End Sub
			Public Overrides Sub write(  b As Integer)
				Throw New java.io.IOException("Stream closed")
			End Sub
		End Class

		''' <summary>
		''' Represents a source of subprocess input or a destination of
		''' subprocess output.
		''' 
		''' Each {@code Redirect} instance is one of the following:
		''' 
		''' <ul>
		''' <li>the special value <seealso cref="#PIPE Redirect.PIPE"/>
		''' <li>the special value <seealso cref="#INHERIT Redirect.INHERIT"/>
		''' <li>a redirection to read from a file, created by an invocation of
		'''     <seealso cref="Redirect#from Redirect.from(File)"/>
		''' <li>a redirection to write to a file,  created by an invocation of
		'''     <seealso cref="Redirect#to Redirect.to(File)"/>
		''' <li>a redirection to append to a file, created by an invocation of
		'''     <seealso cref="Redirect#appendTo Redirect.appendTo(File)"/>
		''' </ul>
		''' 
		''' <p>Each of the above categories has an associated unique
		''' <seealso cref="Type Type"/>.
		''' 
		''' @since 1.7
		''' </summary>
		Public MustInherit Class Redirect
			''' <summary>
			''' The type of a <seealso cref="Redirect"/>.
			''' </summary>
			Public Enum Type
				''' <summary>
				''' The type of <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
				''' </summary>
				PIPE

				''' <summary>
				''' The type of <seealso cref="Redirect#INHERIT Redirect.INHERIT"/>.
				''' </summary>
				INHERIT

				''' <summary>
				''' The type of redirects returned from
				''' <seealso cref="Redirect#from Redirect.from(File)"/>.
				''' </summary>
				READ

				''' <summary>
				''' The type of redirects returned from
				''' <seealso cref="Redirect#to Redirect.to(File)"/>.
				''' </summary>
				WRITE

				''' <summary>
				''' The type of redirects returned from
				''' <seealso cref="Redirect#appendTo Redirect.appendTo(File)"/>.
				''' </summary>
				APPEND
			End Enum

			''' <summary>
			''' Returns the type of this {@code Redirect}. </summary>
			''' <returns> the type of this {@code Redirect} </returns>
			Public MustOverride Function type() As Type

			''' <summary>
			''' Indicates that subprocess I/O will be connected to the
			''' current Java process over a pipe.
			''' 
			''' This is the default handling of subprocess standard I/O.
			''' 
			''' <p>It will always be true that
			'''  <pre> {@code
			''' Redirect.PIPE.file() == null &&
			''' Redirect.PIPE.type() == Redirect.Type.PIPE
			''' }</pre>
			''' </summary>
			Public Shared ReadOnly PIPE As Redirect = New RedirectAnonymousInnerClassHelper

			Private Class RedirectAnonymousInnerClassHelper
				Inherits Redirect

				Public Overrides Function type() As Type
					Return Type.PIPE
				End Function
				Public Overrides Function ToString() As String
					Return outerInstance.type().ToString()
				End Function
			End Class

			''' <summary>
			''' Indicates that subprocess I/O source or destination will be the
			''' same as those of the current process.  This is the normal
			''' behavior of most operating system command interpreters (shells).
			''' 
			''' <p>It will always be true that
			'''  <pre> {@code
			''' Redirect.INHERIT.file() == null &&
			''' Redirect.INHERIT.type() == Redirect.Type.INHERIT
			''' }</pre>
			''' </summary>
			Public Shared ReadOnly INHERIT As Redirect = New RedirectAnonymousInnerClassHelper2

			Private Class RedirectAnonymousInnerClassHelper2
				Inherits Redirect

				Public Overrides Function type() As Type
					Return Type.INHERIT
				End Function
				Public Overrides Function ToString() As String
					Return outerInstance.type().ToString()
				End Function
			End Class

			''' <summary>
			''' Returns the <seealso cref="File"/> source or destination associated
			''' with this redirect, or {@code null} if there is no such file.
			''' </summary>
			''' <returns> the file associated with this redirect,
			'''         or {@code null} if there is no such file </returns>
			Public Overridable Function file() As java.io.File
				Return Nothing
			End Function

			''' <summary>
			''' When redirected to a destination file, indicates if the output
			''' is to be written to the end of the file.
			''' </summary>
			Friend Overridable Function append() As Boolean
				Throw New UnsupportedOperationException
			End Function

			''' <summary>
			''' Returns a redirect to read from the specified file.
			''' 
			''' <p>It will always be true that
			'''  <pre> {@code
			''' Redirect.from(file).file() == file &&
			''' Redirect.from(file).type() == Redirect.Type.READ
			''' }</pre>
			''' </summary>
			''' <param name="file"> The {@code File} for the {@code Redirect}. </param>
			''' <exception cref="NullPointerException"> if the specified file is null </exception>
			''' <returns> a redirect to read from the specified file </returns>
			Public Shared Function [from](  file As java.io.File) As Redirect
				If file Is Nothing Then Throw New NullPointerException
				Return New RedirectAnonymousInnerClassHelper3
			End Function

			Private Class RedirectAnonymousInnerClassHelper3
				Inherits Redirect

				Public Overrides Function type() As Type
					Return Type.READ
				End Function
				Public Overrides Function file() As java.io.File
					Return file
				End Function
				Public Overrides Function ToString() As String
					Return "redirect to read from file """ & file & """"
				End Function
			End Class

			''' <summary>
			''' Returns a redirect to write to the specified file.
			''' If the specified file exists when the subprocess is started,
			''' its previous contents will be discarded.
			''' 
			''' <p>It will always be true that
			'''  <pre> {@code
			''' Redirect.to(file).file() == file &&
			''' Redirect.to(file).type() == Redirect.Type.WRITE
			''' }</pre>
			''' </summary>
			''' <param name="file"> The {@code File} for the {@code Redirect}. </param>
			''' <exception cref="NullPointerException"> if the specified file is null </exception>
			''' <returns> a redirect to write to the specified file </returns>
			Public Shared Function [to](  file As java.io.File) As Redirect
				If file Is Nothing Then Throw New NullPointerException
				Return New RedirectAnonymousInnerClassHelper4
			End Function

			Private Class RedirectAnonymousInnerClassHelper4
				Inherits Redirect

				Public Overrides Function type() As Type
					Return Type.WRITE
				End Function
				Public Overrides Function file() As java.io.File
					Return file
				End Function
				Public Overrides Function ToString() As String
					Return "redirect to write to file """ & file & """"
				End Function
				Friend Overrides Function append() As Boolean
					Return False
				End Function
			End Class

			''' <summary>
			''' Returns a redirect to append to the specified file.
			''' Each write operation first advances the position to the
			''' end of the file and then writes the requested data.
			''' Whether the advancement of the position and the writing
			''' of the data are done in a single atomic operation is
			''' system-dependent and therefore unspecified.
			''' 
			''' <p>It will always be true that
			'''  <pre> {@code
			''' Redirect.appendTo(file).file() == file &&
			''' Redirect.appendTo(file).type() == Redirect.Type.APPEND
			''' }</pre>
			''' </summary>
			''' <param name="file"> The {@code File} for the {@code Redirect}. </param>
			''' <exception cref="NullPointerException"> if the specified file is null </exception>
			''' <returns> a redirect to append to the specified file </returns>
			Public Shared Function appendTo(  file As java.io.File) As Redirect
				If file Is Nothing Then Throw New NullPointerException
				Return New RedirectAnonymousInnerClassHelper5
			End Function

			Private Class RedirectAnonymousInnerClassHelper5
				Inherits Redirect

				Public Overrides Function type() As Type
					Return Type.APPEND
				End Function
				Public Overrides Function file() As java.io.File
					Return file
				End Function
				Public Overrides Function ToString() As String
					Return "redirect to append to file """ & file & """"
				End Function
				Friend Overrides Function append() As Boolean
					Return True
				End Function
			End Class

			''' <summary>
			''' Compares the specified object with this {@code Redirect} for
			''' equality.  Returns {@code true} if and only if the two
			''' objects are identical or both objects are {@code Redirect}
			''' instances of the same type associated with non-null equal
			''' {@code File} instances.
			''' </summary>
			Public Overrides Function Equals(  obj As Object) As Boolean
				If obj Is Me Then Return True
				If Not(TypeOf obj Is Redirect) Then Return False
				Dim r As Redirect = CType(obj, Redirect)
				If r.type() <> Me.type() Then Return False
				Debug.Assert(Me.file() IsNot Nothing)
				Return Me.file().Equals(r.file())
			End Function

			''' <summary>
			''' Returns a hash code value for this {@code Redirect}. </summary>
			''' <returns> a hash code value for this {@code Redirect} </returns>
			Public Overrides Function GetHashCode() As Integer
				Dim file As File = file()
				If file Is Nothing Then
					Return MyBase.GetHashCode()
				Else
					Return file.GetHashCode()
				End If
			End Function

			''' <summary>
			''' No public constructors.  Clients must use predefined
			''' static {@code Redirect} instances or factory methods.
			''' </summary>
			Private Sub New()
			End Sub
		End Class

		Private Function redirects() As Redirect()
			If redirects_Renamed Is Nothing Then redirects_Renamed = New Redirect() { Redirect.PIPE, Redirect.PIPE, Redirect.PIPE }
			Return redirects_Renamed
		End Function

		''' <summary>
		''' Sets this process builder's standard input source.
		''' 
		''' Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		''' method obtain their standard input from this source.
		''' 
		''' <p>If the source is <seealso cref="Redirect#PIPE Redirect.PIPE"/>
		''' (the initial value), then the standard input of a
		''' subprocess can be written to using the output stream
		''' returned by <seealso cref="Process#getOutputStream()"/>.
		''' If the source is set to any other value, then
		''' <seealso cref="Process#getOutputStream()"/> will return a
		''' <a href="#redirect-input">null output stream</a>.
		''' </summary>
		''' <param name="source"> the new standard input source </param>
		''' <returns> this process builder </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the redirect does not correspond to a valid source
		'''         of data, that is, has type
		'''         <seealso cref="Redirect.Type#WRITE WRITE"/> or
		'''         <seealso cref="Redirect.Type#APPEND APPEND"/>
		''' @since  1.7 </exception>
		Public Function redirectInput(  source As Redirect) As ProcessBuilder
			If source.type() = Redirect.Type.WRITE OrElse source.type() = Redirect.Type.APPEND Then Throw New IllegalArgumentException("Redirect invalid for reading: " & source)
			redirects()(0) = source
			Return Me
		End Function

		''' <summary>
		''' Sets this process builder's standard output destination.
		''' 
		''' Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		''' method send their standard output to this destination.
		''' 
		''' <p>If the destination is <seealso cref="Redirect#PIPE Redirect.PIPE"/>
		''' (the initial value), then the standard output of a subprocess
		''' can be read using the input stream returned by {@link
		''' Process#getInputStream()}.
		''' If the destination is set to any other value, then
		''' <seealso cref="Process#getInputStream()"/> will return a
		''' <a href="#redirect-output">null input stream</a>.
		''' </summary>
		''' <param name="destination"> the new standard output destination </param>
		''' <returns> this process builder </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the redirect does not correspond to a valid
		'''         destination of data, that is, has type
		'''         <seealso cref="Redirect.Type#READ READ"/>
		''' @since  1.7 </exception>
		Public Function redirectOutput(  destination As Redirect) As ProcessBuilder
			If destination.type() = Redirect.Type.READ Then Throw New IllegalArgumentException("Redirect invalid for writing: " & destination)
			redirects()(1) = destination
			Return Me
		End Function

		''' <summary>
		''' Sets this process builder's standard error destination.
		''' 
		''' Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		''' method send their standard error to this destination.
		''' 
		''' <p>If the destination is <seealso cref="Redirect#PIPE Redirect.PIPE"/>
		''' (the initial value), then the error output of a subprocess
		''' can be read using the input stream returned by {@link
		''' Process#getErrorStream()}.
		''' If the destination is set to any other value, then
		''' <seealso cref="Process#getErrorStream()"/> will return a
		''' <a href="#redirect-output">null input stream</a>.
		''' 
		''' <p>If the <seealso cref="#redirectErrorStream redirectErrorStream"/>
		''' attribute has been set {@code true}, then the redirection set
		''' by this method has no effect.
		''' </summary>
		''' <param name="destination"> the new standard error destination </param>
		''' <returns> this process builder </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if the redirect does not correspond to a valid
		'''         destination of data, that is, has type
		'''         <seealso cref="Redirect.Type#READ READ"/>
		''' @since  1.7 </exception>
		Public Function redirectError(  destination As Redirect) As ProcessBuilder
			If destination.type() = Redirect.Type.READ Then Throw New IllegalArgumentException("Redirect invalid for writing: " & destination)
			redirects()(2) = destination
			Return Me
		End Function

		''' <summary>
		''' Sets this process builder's standard input source to a file.
		''' 
		''' <p>This is a convenience method.  An invocation of the form
		''' {@code redirectInput(file)}
		''' behaves in exactly the same way as the invocation
		''' <seealso cref="#redirectInput(Redirect) redirectInput"/>
		''' {@code (Redirect.from(file))}.
		''' </summary>
		''' <param name="file"> the new standard input source </param>
		''' <returns> this process builder
		''' @since  1.7 </returns>
		Public Function redirectInput(  file As java.io.File) As ProcessBuilder
			Return redirectInput(Redirect.from(file))
		End Function

		''' <summary>
		''' Sets this process builder's standard output destination to a file.
		''' 
		''' <p>This is a convenience method.  An invocation of the form
		''' {@code redirectOutput(file)}
		''' behaves in exactly the same way as the invocation
		''' <seealso cref="#redirectOutput(Redirect) redirectOutput"/>
		''' {@code (Redirect.to(file))}.
		''' </summary>
		''' <param name="file"> the new standard output destination </param>
		''' <returns> this process builder
		''' @since  1.7 </returns>
		Public Function redirectOutput(  file As java.io.File) As ProcessBuilder
			Return redirectOutput(Redirect.to(file))
		End Function

		''' <summary>
		''' Sets this process builder's standard error destination to a file.
		''' 
		''' <p>This is a convenience method.  An invocation of the form
		''' {@code redirectError(file)}
		''' behaves in exactly the same way as the invocation
		''' <seealso cref="#redirectError(Redirect) redirectError"/>
		''' {@code (Redirect.to(file))}.
		''' </summary>
		''' <param name="file"> the new standard error destination </param>
		''' <returns> this process builder
		''' @since  1.7 </returns>
		Public Function redirectError(  file As java.io.File) As ProcessBuilder
			Return redirectError(Redirect.to(file))
		End Function

		''' <summary>
		''' Returns this process builder's standard input source.
		''' 
		''' Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		''' method obtain their standard input from this source.
		''' The initial value is <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
		''' </summary>
		''' <returns> this process builder's standard input source
		''' @since  1.7 </returns>
		Public Function redirectInput() As Redirect
			Return If(redirects_Renamed Is Nothing, Redirect.PIPE, redirects_Renamed(0))
		End Function

		''' <summary>
		''' Returns this process builder's standard output destination.
		''' 
		''' Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		''' method redirect their standard output to this destination.
		''' The initial value is <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
		''' </summary>
		''' <returns> this process builder's standard output destination
		''' @since  1.7 </returns>
		Public Function redirectOutput() As Redirect
			Return If(redirects_Renamed Is Nothing, Redirect.PIPE, redirects_Renamed(1))
		End Function

		''' <summary>
		''' Returns this process builder's standard error destination.
		''' 
		''' Subprocesses subsequently started by this object's <seealso cref="#start()"/>
		''' method redirect their standard error to this destination.
		''' The initial value is <seealso cref="Redirect#PIPE Redirect.PIPE"/>.
		''' </summary>
		''' <returns> this process builder's standard error destination
		''' @since  1.7 </returns>
		Public Function redirectError() As Redirect
			Return If(redirects_Renamed Is Nothing, Redirect.PIPE, redirects_Renamed(2))
		End Function

		''' <summary>
		''' Sets the source and destination for subprocess standard I/O
		''' to be the same as those of the current Java process.
		''' 
		''' <p>This is a convenience method.  An invocation of the form
		'''  <pre> {@code
		''' pb.inheritIO()
		''' }</pre>
		''' behaves in exactly the same way as the invocation
		'''  <pre> {@code
		''' pb.redirectInput(Redirect.INHERIT)
		'''   .redirectOutput(Redirect.INHERIT)
		'''   .redirectError(Redirect.INHERIT)
		''' }</pre>
		''' 
		''' This gives behavior equivalent to most operating system
		''' command interpreters, or the standard C library function
		''' {@code system()}.
		''' </summary>
		''' <returns> this process builder
		''' @since  1.7 </returns>
		Public Function inheritIO() As ProcessBuilder
			java.util.Arrays.fill(redirects(), Redirect.INHERIT)
			Return Me
		End Function

		''' <summary>
		''' Tells whether this process builder merges standard error and
		''' standard output.
		''' 
		''' <p>If this property is {@code true}, then any error output
		''' generated by subprocesses subsequently started by this object's
		''' <seealso cref="#start()"/> method will be merged with the standard
		''' output, so that both can be read using the
		''' <seealso cref="Process#getInputStream()"/> method.  This makes it easier
		''' to correlate error messages with the corresponding output.
		''' The initial value is {@code false}.
		''' </summary>
		''' <returns> this process builder's {@code redirectErrorStream} property </returns>
		Public Function redirectErrorStream() As Boolean
			Return redirectErrorStream_Renamed
		End Function

		''' <summary>
		''' Sets this process builder's {@code redirectErrorStream} property.
		''' 
		''' <p>If this property is {@code true}, then any error output
		''' generated by subprocesses subsequently started by this object's
		''' <seealso cref="#start()"/> method will be merged with the standard
		''' output, so that both can be read using the
		''' <seealso cref="Process#getInputStream()"/> method.  This makes it easier
		''' to correlate error messages with the corresponding output.
		''' The initial value is {@code false}.
		''' </summary>
		''' <param name="redirectErrorStream"> the new property value </param>
		''' <returns> this process builder </returns>
		Public Function redirectErrorStream(  redirectErrorStream_Renamed As Boolean) As ProcessBuilder
			Me.redirectErrorStream_Renamed = redirectErrorStream_Renamed
			Return Me
		End Function

		''' <summary>
		''' Starts a new process using the attributes of this process builder.
		''' 
		''' <p>The new process will
		''' invoke the command and arguments given by <seealso cref="#command()"/>,
		''' in a working directory as given by <seealso cref="#directory()"/>,
		''' with a process environment as given by <seealso cref="#environment()"/>.
		''' 
		''' <p>This method checks that the command is a valid operating
		''' system command.  Which commands are valid is system-dependent,
		''' but at the very least the command must be a non-empty list of
		''' non-null strings.
		''' 
		''' <p>A minimal set of system dependent environment variables may
		''' be required to start a process on some operating systems.
		''' As a result, the subprocess may inherit additional environment variable
		''' settings beyond those in the process builder's <seealso cref="#environment()"/>.
		''' 
		''' <p>If there is a security manager, its
		''' <seealso cref="SecurityManager#checkExec checkExec"/>
		''' method is called with the first component of this object's
		''' {@code command} array as its argument. This may result in
		''' a <seealso cref="SecurityException"/> being thrown.
		''' 
		''' <p>Starting an operating system process is highly system-dependent.
		''' Among the many things that can go wrong are:
		''' <ul>
		''' <li>The operating system program file was not found.
		''' <li>Access to the program file was denied.
		''' <li>The working directory does not exist.
		''' </ul>
		''' 
		''' <p>In such cases an exception will be thrown.  The exact nature
		''' of the exception is system-dependent, but it will always be a
		''' subclass of <seealso cref="IOException"/>.
		''' 
		''' <p>Subsequent modifications to this process builder will not
		''' affect the returned <seealso cref="Process"/>.
		''' </summary>
		''' <returns> a new <seealso cref="Process"/> object for managing the subprocess
		''' </returns>
		''' <exception cref="NullPointerException">
		'''         if an element of the command list is null
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''         if the command is an empty list (has size {@code 0})
		''' </exception>
		''' <exception cref="SecurityException">
		'''         if a security manager exists and
		'''         <ul>
		''' 
		'''         <li>its
		'''         <seealso cref="SecurityManager#checkExec checkExec"/>
		'''         method doesn't allow creation of the subprocess, or
		''' 
		'''         <li>the standard input to the subprocess was
		'''         <seealso cref="#redirectInput redirected from a file"/>
		'''         and the security manager's
		'''         <seealso cref="SecurityManager#checkRead checkRead"/> method
		'''         denies read access to the file, or
		''' 
		'''         <li>the standard output or standard error of the
		'''         subprocess was
		'''         <seealso cref="#redirectOutput redirected to a file"/>
		'''         and the security manager's
		'''         <seealso cref="SecurityManager#checkWrite checkWrite"/> method
		'''         denies write access to the file
		''' 
		'''         </ul>
		''' </exception>
		''' <exception cref="IOException"> if an I/O error occurs
		''' </exception>
		''' <seealso cref= Runtime#exec(String[], String[], java.io.File) </seealso>
		Public Function start() As Process
			' Must convert to array first -- a malicious user-supplied
			' list might try to circumvent the security check.
			Dim cmdarray As String() = command_Renamed.ToArray()
			cmdarray = cmdarray.clone()

			For Each arg As String In cmdarray
				If arg Is Nothing Then Throw New NullPointerException
			Next arg
			' Throws IndexOutOfBoundsException if command is empty
			Dim prog As String = cmdarray(0)

			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkExec(prog)

			Dim dir As String = If(directory_Renamed Is Nothing, Nothing, directory_Renamed.ToString())

			For i As Integer = 1 To cmdarray.Length - 1
				If cmdarray(i).IndexOf(ChrW(&H0000)) >= 0 Then Throw New java.io.IOException("invalid null character in command")
			Next i

			Try
				Return ProcessImpl.start(cmdarray, environment_Renamed, dir, redirects_Renamed, redirectErrorStream_Renamed)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch java.io.IOException Or IllegalArgumentException e
				Dim exceptionInfo As String = ": " & e.message
				Dim cause As Throwable = e
				If (TypeOf e Is java.io.IOException) AndAlso security IsNot Nothing Then
					' Can not disclose the fail reason for read-protected files.
					Try
						security.checkRead(prog)
					Catch se As SecurityException
						exceptionInfo = ""
						cause = se
					End Try
				End If
				' It's much easier for us to create a high-quality error
				' message than the low-level C code which found the problem.
				Throw New java.io.IOException("Cannot run program """ & prog & """" & (If(dir Is Nothing, "", " (in directory """ & dir & """)")) + exceptionInfo, cause)
			End Try
		End Function
	End Class

End Namespace
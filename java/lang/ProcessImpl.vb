Imports Microsoft.VisualBasic
Imports System.Collections.Generic

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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


	' This class is for the exclusive use of ProcessBuilder.start() to
	' * create new processes.
	' *
	' * @author Martin Buchholz
	' * @since   1.5
	' 

	Friend NotInheritable Class ProcessImpl
		Inherits Process

		Private Shared ReadOnly fdAccess As sun.misc.JavaIOFileDescriptorAccess = sun.misc.SharedSecrets.javaIOFileDescriptorAccess

		''' <summary>
		''' Open a file for writing. If {@code append} is {@code true} then the file
		''' is opened for atomic append directly and a FileOutputStream constructed
		''' with the resulting handle. This is because a FileOutputStream created
		''' to append to a file does not open the file in a manner that guarantees
		''' that writes by the child process will be atomic.
		''' </summary>
		Private Shared Function newFileOutputStream(ByVal f As java.io.File, ByVal append As Boolean) As java.io.FileOutputStream
			If append Then
				Dim path As String = f.path
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkWrite(path)
				Dim handle As Long = openForAtomicAppend(path)
				Dim fd As New java.io.FileDescriptor
				fdAccess.handledle(fd, handle)
				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			   )
			Else
				Return New java.io.FileOutputStream(f)
			End If
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As java.io.FileOutputStream
				Return New java.io.FileOutputStream(fd)
			End Function
		End Class

		' System-dependent portion of ProcessBuilder.start()
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static Process start(String cmdarray() , java.util.Map<String,String> environment, String dir, ProcessBuilder.Redirect() redirects, boolean redirectErrorStream) throws java.io.IOException
			Dim envblock As String = ProcessEnvironment.toEnvironmentBlock(environment)

			Dim f0 As java.io.FileInputStream = Nothing
			Dim f1 As java.io.FileOutputStream = Nothing
			Dim f2 As java.io.FileOutputStream = Nothing

			Try
				Dim stdHandles As Long()
				If redirects Is Nothing Then
					stdHandles = New Long() { -1L, -1L, -1L }
				Else
					stdHandles = New Long(2){}

					If redirects(0) = Redirect.PIPE Then
						stdHandles(0) = -1L
					ElseIf redirects(0) = Redirect.INHERIT Then
						stdHandles(0) = fdAccess.getHandle(java.io.FileDescriptor.in)
					Else
						f0 = New java.io.FileInputStream(redirects(0).file())
						stdHandles(0) = fdAccess.getHandle(f0.fD)
					End If

					If redirects(1) = Redirect.PIPE Then
						stdHandles(1) = -1L
					ElseIf redirects(1) = Redirect.INHERIT Then
						stdHandles(1) = fdAccess.getHandle(java.io.FileDescriptor.out)
					Else
						f1 = newFileOutputStream(redirects(1).file(), redirects(1).append())
						stdHandles(1) = fdAccess.getHandle(f1.fD)
					End If

					If redirects(2) = Redirect.PIPE Then
						stdHandles(2) = -1L
					ElseIf redirects(2) = Redirect.INHERIT Then
						stdHandles(2) = fdAccess.getHandle(java.io.FileDescriptor.err)
					Else
						f2 = newFileOutputStream(redirects(2).file(), redirects(2).append())
						stdHandles(2) = fdAccess.getHandle(f2.fD)
					End If
				End If

				Return New ProcessImpl(cmdarray, envblock, dir, stdHandles, redirectErrorStream)
			Finally
				' In theory, close() can throw IOException
				' (although it is rather unlikely to happen here)
				Try
					If f0 IsNot Nothing Then f0.close()
				Finally
					Try
						If f1 IsNot Nothing Then f1.close()
					Finally
						If f2 IsNot Nothing Then f2.close()
					End Try
				End Try
			End Try


		private static class LazyPattern
			' Escape-support version:
			'    "(\")((?:\\\\\\1|.)+?)\\1|([^\\s\"]+)";
			private static final java.util.regex.Pattern PATTERN = java.util.regex.Pattern.compile("[^\s""]+|""[^""]*""")

	'     Parses the command string parameter into the executable name and
	'     * program arguments.
	'     *
	'     * The command string is broken into tokens. The token separator is a space
	'     * or quota character. The space inside quotation is not a token separator.
	'     * There are no escape sequences.
	'     
		private static String() getTokensFromCommand(String command)
			Dim matchList As New List(Of String)(8)
			Dim regexMatcher As java.util.regex.Matcher = LazyPattern.PATTERN.matcher(command)
			Do While regexMatcher.find()
				matchList.Add(regexMatcher.group())
			Loop
			Return matchList.ToArray()

		private static final Integer VERIFICATION_CMD_BAT = 0
		private static final Integer VERIFICATION_WIN32 = 1
		private static final Integer VERIFICATION_LEGACY = 2
		private static final Char ESCAPE_VERIFICATION()() = { {" "c, ControlChars.Tab, "<"c, ">"c, "&"c, "|"c, "^"c}, {" "c, ControlChars.Tab, "<"c, ">"c}, {" "c, ControlChars.Tab} }

		private static String createCommandLine(Integer verificationType, final String executablePath, final String cmd())
			Dim cmdbuf As New StringBuilder(80)

			cmdbuf.append(executablePath)

			For i As Integer = 1 To cmd.length - 1
				cmdbuf.append(" "c)
				Dim s As String = cmd(i)
				If needsEscaping(verificationType, s) Then
					cmdbuf.append(""""c).append(s)

					' The code protects the [java.exe] and console command line
					' parser, that interprets the [\"] combination as an escape
					' sequence for the ["] char.
					'     http://msdn.microsoft.com/en-us/library/17w5ykft.aspx
					'
					' If the argument is an FS path, doubling of the tail [\]
					' char is not a problem for non-console applications.
					'
					' The [\"] sequence is not an escape sequence for the [cmd.exe]
					' command line parser. The case of the [""] tail escape
					' sequence could not be realized due to the argument validation
					' procedure.
					If (verificationType <> VERIFICATION_CMD_BAT) AndAlso s.EndsWith("\") Then cmdbuf.append("\"c)
					cmdbuf.append(""""c)
				Else
					cmdbuf.append(s)
				End If
			Next i
			Return cmdbuf.ToString()

		private static Boolean isQuoted(Boolean noQuotesInside, String arg, String errorMessage)
			Dim lastPos As Integer = arg.length() - 1
			If lastPos >=1 AndAlso arg.Chars(0) = """"c AndAlso arg.Chars(lastPos) = """"c Then
				' The argument has already been quoted.
				If noQuotesInside Then
					If arg.IndexOf(""""c, 1) <> lastPos Then Throw New IllegalArgumentException(errorMessage)
				End If
				Return True
			End If
			If noQuotesInside Then
				If arg.IndexOf(""""c) >= 0 Then Throw New IllegalArgumentException(errorMessage)
			End If
			Return False

		private static Boolean needsEscaping(Integer verificationType, String arg)
			' Switch off MS heuristic for internal ["].
			' Please, use the explicit [cmd.exe] call
			' if you need the internal ["].
			'    Example: "cmd.exe", "/C", "Extended_MS_Syntax"

			' For [.exe] or [.com] file the unpaired/internal ["]
			' in the argument is not a problem.
			Dim argIsQuoted As Boolean = isQuoted((verificationType = VERIFICATION_CMD_BAT), arg, "Argument has embedded quote, use the explicit CMD.EXE call.")

			If Not argIsQuoted Then
				Dim testEscape As Char() = ESCAPE_VERIFICATION(verificationType)
				For i As Integer = 0 To testEscape.Length - 1
					If arg.IndexOf(testEscape(i)) >= 0 Then Return True
				Next i
			End If
			Return False

		private static String getExecutablePath(String path) throws java.io.IOException
			Dim pathIsQuoted As Boolean = isQuoted(True, path, "Executable name has embedded quote, split the arguments")

			' Win32 CreateProcess requires path to be normalized
			Dim fileToRun As New File(If(pathIsQuoted, path.Substring(1, path.length() - 1 - 1), path))

			' From the [CreateProcess] function documentation:
			'
			' "If the file name does not contain an extension, .exe is appended.
			' Therefore, if the file name extension is .com, this parameter
			' must include the .com extension. If the file name ends in
			' a period (.) with no extension, or if the file name contains a path,
			' .exe is not appended."
			'
			' "If the file name !does not contain a directory path!,
			' the system searches for the executable file in the following
			' sequence:..."
			'
			' In practice ANY non-existent path is extended by [.exe] extension
			' in the [CreateProcess] funcion with the only exception:
			' the path ends by (.)

			Return fileToRun.path


		private Boolean isShellFile(String executablePath)
			Dim upPath As String = executablePath.ToUpper()
			Return (upPath.EndsWith(".CMD") OrElse upPath.EndsWith(".BAT"))

		private String quoteString(String arg)
			Dim argbuf As New StringBuilder(arg.length() + 2)
			Return argbuf.append(""""c).append(arg).append(""""c).ToString()


		private Long handle = 0
		private java.io.OutputStream stdin_stream
		private java.io.InputStream stdout_stream
		private java.io.InputStream stderr_stream

		private ProcessImpl(String cmd() , final String envblock, final String path, final Long() stdHandles, final Boolean redirectErrorStream) throws java.io.IOException
			Dim cmdstr As String
			Dim security As SecurityManager = System.securityManager
			Dim allowAmbiguousCommands As Boolean = False
			If security Is Nothing Then
				allowAmbiguousCommands = True
				Dim value As String = System.getProperty("jdk.lang.Process.allowAmbiguousCommands")
				If value IsNot Nothing Then allowAmbiguousCommands = Not "false".equalsIgnoreCase(value)
			End If
			If allowAmbiguousCommands Then
				' Legacy mode.

				' Normalize path if possible.
				Dim executablePath_Renamed As String = (New File(cmd(0))).path

				' No worry about internal, unpaired ["], and redirection/piping.
				If needsEscaping(VERIFICATION_LEGACY, executablePath_Renamed) Then executablePath_Renamed = quoteString(executablePath_Renamed)

				cmdstr = createCommandLine(VERIFICATION_LEGACY, executablePath_Renamed, cmd)
					'legacy mode doesn't worry about extended verification
			Else
				Dim executablePath_Renamed As String
				Try
					executablePath_Renamed = getExecutablePath(cmd(0))
				Catch e As IllegalArgumentException
					' Workaround for the calls like
					' Runtime.getRuntime().exec("\"C:\\Program Files\\foo\" bar")

					' No chance to avoid CMD/BAT injection, except to do the work
					' right from the beginning. Otherwise we have too many corner
					' cases from
					'    Runtime.getRuntime().exec(String[] cmd [, ...])
					' calls with internal ["] and escape sequences.

					' Restore original command line.
					Dim join As New StringBuilder
					' terminal space in command line is ok
					For Each s As String In cmd
						join.append(s).append(" "c)
					Next s

					' Parse the command line again.
					cmd = getTokensFromCommand(join.ToString())
					executablePath_Renamed = getExecutablePath(cmd(0))

					' Check new executable name once more
					If security IsNot Nothing Then security.checkExec(executablePath_Renamed)
				End Try

				' Quotation protects from interpretation of the [path] argument as
				' start of longer path with spaces. Quotation has no influence to
				' [.exe] extension heuristic.
				cmdstr = createCommandLine(If(isShellFile(executablePath_Renamed), VERIFICATION_CMD_BAT, VERIFICATION_WIN32), quoteString(executablePath_Renamed), cmd)
						' We need the extended verification procedure for CMD files.
			End If

			handle = create(cmdstr, envblock, path, stdHandles, redirectErrorStream)

			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)

		public java.io.OutputStream outputStream
			Return stdin_stream

		public java.io.InputStream inputStream
			Return stdout_stream

		public java.io.InputStream errorStream
			Return stderr_stream

		protected void Finalize()
			closeHandle(handle)

		private static final Integer STILL_ACTIVE = stillActive
		private static native Integer stillActive

		public Integer exitValue()
			Dim exitCode As Integer = getExitCodeProcess(handle)
			If exitCode = STILL_ACTIVE Then Throw New IllegalThreadStateException("process has not exited")
			Return exitCode
		private static native Integer getExitCodeProcess(Long handle)

		public Integer waitFor() throws InterruptedException
			waitForInterruptibly(handle)
			If Thread.interrupted() Then Throw New InterruptedException
			Return exitValue()

		private static native void waitForInterruptibly(Long handle)

		public Boolean waitFor(Long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
			If getExitCodeProcess(handle) <> STILL_ACTIVE Then Return True
			If timeout <= 0 Then Return False

			Dim remainingNanos As Long = unit.toNanos(timeout)
			Dim deadline As Long = System.nanoTime() + remainingNanos

			Do
				' Round up to next millisecond
				Dim msTimeout As Long = java.util.concurrent.TimeUnit.NANOSECONDS.toMillis(remainingNanos + 999_999L)
				waitForTimeoutInterruptibly(handle, msTimeout)
				If Thread.interrupted() Then Throw New InterruptedException
				If getExitCodeProcess(handle) <> STILL_ACTIVE Then Return True
				remainingNanos = deadline - System.nanoTime()
			Loop While remainingNanos > 0

			Return (getExitCodeProcess(handle) <> STILL_ACTIVE)

		private static native void waitForTimeoutInterruptibly(Long handle, Long timeout)

		public void destroy()
			terminateProcess(handle)

		public Process destroyForcibly()
			destroy()
			Return Me

		private static native void terminateProcess(Long handle)

		public Boolean alive
			Return isProcessAlive(handle)

		private static native Boolean isProcessAlive(Long handle)

		''' <summary>
		''' Create a process using the win32 function CreateProcess.
		''' The method is synchronized due to MS kb315939 problem.
		''' All native handles should restore the inherit flag at the end of call.
		''' </summary>
		''' <param name="cmdstr"> the Windows command line </param>
		''' <param name="envblock"> NUL-separated, double-NUL-terminated list of
		'''        environment strings in VAR=VALUE form </param>
		''' <param name="dir"> the working directory of the process, or null if
		'''        inheriting the current directory from the parent process </param>
		''' <param name="stdHandles"> array of windows HANDLEs.  Indexes 0, 1, and
		'''        2 correspond to standard input, standard output and
		'''        standard error, respectively.  On input, a value of -1
		'''        means to create a pipe to connect child and parent
		'''        processes.  On output, a value which is not -1 is the
		'''        parent pipe handle corresponding to the pipe which has
		'''        been created.  An element of this array is -1 on input
		'''        if and only if it is <em>not</em> -1 on output. </param>
		''' <param name="redirectErrorStream"> redirectErrorStream attribute </param>
		''' <returns> the native subprocess HANDLE returned by CreateProcess </returns>
		private static synchronized native Long create(String cmdstr, String envblock, String dir, Long() stdHandles, Boolean redirectErrorStream) throws java.io.IOException

		''' <summary>
		''' Opens a file for atomic append. The file is created if it doesn't
		''' already exist.
		''' </summary>
		''' <param name="file"> the file to open or create </param>
		''' <returns> the native HANDLE </returns>
		private static native Long openForAtomicAppend(String path) throws java.io.IOException

		private static native Boolean closeHandle(Long handle)
	End Class


	Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
		Implements java.security.PrivilegedAction(Of T)

		Public Overridable Function run() As Void
			If stdHandles(0) = -1L Then
				stdin_stream = ProcessBuilder.NullOutputStream.INSTANCE
			Else
				Dim stdin_fd As New java.io.FileDescriptor
				fdAccess.handledle(stdin_fd, stdHandles(0))
				stdin_stream = New java.io.BufferedOutputStream(New java.io.FileOutputStream(stdin_fd))
			End If

			If stdHandles(1) = -1L Then
				stdout_stream = ProcessBuilder.NullInputStream.INSTANCE
			Else
				Dim stdout_fd As New java.io.FileDescriptor
				fdAccess.handledle(stdout_fd, stdHandles(1))
				stdout_stream = New java.io.BufferedInputStream(New java.io.FileInputStream(stdout_fd))
			End If

			If stdHandles(2) = -1L Then
				stderr_stream = ProcessBuilder.NullInputStream.INSTANCE
			Else
				Dim stderr_fd As New java.io.FileDescriptor
				fdAccess.handledle(stderr_fd, stdHandles(2))
				stderr_stream = New java.io.FileInputStream(stderr_fd)
			End If

			Return Nothing
		End Function
	End Class
End Namespace
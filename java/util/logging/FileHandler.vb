Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.logging



	''' <summary>
	''' Simple file logging <tt>Handler</tt>.
	''' <p>
	''' The <tt>FileHandler</tt> can either write to a specified file,
	''' or it can write to a rotating set of files.
	''' <p>
	''' For a rotating set of files, as each file reaches a given size
	''' limit, it is closed, rotated out, and a new file opened.
	''' Successively older files are named by adding "0", "1", "2",
	''' etc. into the base filename.
	''' <p>
	''' By default buffering is enabled in the IO libraries but each log
	''' record is flushed out when it is complete.
	''' <p>
	''' By default the <tt>XMLFormatter</tt> class is used for formatting.
	''' <p>
	''' <b>Configuration:</b>
	''' By default each <tt>FileHandler</tt> is initialized using the following
	''' <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	''' refers to the fully-qualified class name of the handler.
	''' If properties are not defined
	''' (or have invalid values) then the specified default values are used.
	''' <ul>
	''' <li>   &lt;handler-name&gt;.level
	'''        specifies the default level for the <tt>Handler</tt>
	'''        (defaults to <tt>Level.ALL</tt>). </li>
	''' <li>   &lt;handler-name&gt;.filter
	'''        specifies the name of a <tt>Filter</tt> class to use
	'''        (defaults to no <tt>Filter</tt>). </li>
	''' <li>   &lt;handler-name&gt;.formatter
	'''        specifies the name of a <tt>Formatter</tt> class to use
	'''        (defaults to <tt>java.util.logging.XMLFormatter</tt>) </li>
	''' <li>   &lt;handler-name&gt;.encoding
	'''        the name of the character set encoding to use (defaults to
	'''        the default platform encoding). </li>
	''' <li>   &lt;handler-name&gt;.limit
	'''        specifies an approximate maximum amount to write (in bytes)
	'''        to any one file.  If this is zero, then there is no limit.
	'''        (Defaults to no limit). </li>
	''' <li>   &lt;handler-name&gt;.count
	'''        specifies how many output files to cycle through (defaults to 1). </li>
	''' <li>   &lt;handler-name&gt;.pattern
	'''        specifies a pattern for generating the output file name.  See
	'''        below for details. (Defaults to "%h/java%u.log"). </li>
	''' <li>   &lt;handler-name&gt;.append
	'''        specifies whether the FileHandler should append onto
	'''        any existing files (defaults to false). </li>
	''' </ul>
	''' <p>
	''' For example, the properties for {@code FileHandler} would be:
	''' <ul>
	''' <li>   java.util.logging.FileHandler.level=INFO </li>
	''' <li>   java.util.logging.FileHandler.formatter=java.util.logging.SimpleFormatter </li>
	''' </ul>
	''' <p>
	''' For a custom handler, e.g. com.foo.MyHandler, the properties would be:
	''' <ul>
	''' <li>   com.foo.MyHandler.level=INFO </li>
	''' <li>   com.foo.MyHandler.formatter=java.util.logging.SimpleFormatter </li>
	''' </ul>
	''' <p>
	''' A pattern consists of a string that includes the following special
	''' components that will be replaced at runtime:
	''' <ul>
	''' <li>    "/"    the local pathname separator </li>
	''' <li>     "%t"   the system temporary directory </li>
	''' <li>     "%h"   the value of the "user.home" system property </li>
	''' <li>     "%g"   the generation number to distinguish rotated logs </li>
	''' <li>     "%u"   a unique number to resolve conflicts </li>
	''' <li>     "%%"   translates to a single percent sign "%" </li>
	''' </ul>
	''' If no "%g" field has been specified and the file count is greater
	''' than one, then the generation number will be added to the end of
	''' the generated filename, after a dot.
	''' <p>
	''' Thus for example a pattern of "%t/java%g.log" with a count of 2
	''' would typically cause log files to be written on Solaris to
	''' /var/tmp/java0.log and /var/tmp/java1.log whereas on Windows 95 they
	''' would be typically written to C:\TEMP\java0.log and C:\TEMP\java1.log
	''' <p>
	''' Generation numbers follow the sequence 0, 1, 2, etc.
	''' <p>
	''' Normally the "%u" unique field is set to 0.  However, if the <tt>FileHandler</tt>
	''' tries to open the filename and finds the file is currently in use by
	''' another process it will increment the unique number field and try
	''' again.  This will be repeated until <tt>FileHandler</tt> finds a file name that
	''' is  not currently in use. If there is a conflict and no "%u" field has
	''' been specified, it will be added at the end of the filename after a dot.
	''' (This will be after any automatically added generation number.)
	''' <p>
	''' Thus if three processes were all trying to log to fred%u.%g.txt then
	''' they  might end up using fred0.0.txt, fred1.0.txt, fred2.0.txt as
	''' the first file in their rotating sequences.
	''' <p>
	''' Note that the use of unique ids to avoid conflicts is only guaranteed
	''' to work reliably when using a local disk file system.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class FileHandler
		Inherits StreamHandler

		Private meter As MeteredStream
		Private append As Boolean
		Private limit As Integer ' zero => no limit.
		Private count As Integer
		Private pattern As String
		Private lockFileName As String
		Private lockFileChannel As java.nio.channels.FileChannel
		Private files As java.io.File()
		Private Const MAX_LOCKS As Integer = 100
		Private Shared ReadOnly locks As java.util.Set(Of String) = New HashSet(Of String)

		''' <summary>
		''' A metered stream is a subclass of OutputStream that
		''' (a) forwards all its output to a target stream
		''' (b) keeps track of how many bytes have been written
		''' </summary>
		Private Class MeteredStream
			Inherits java.io.OutputStream

			Private ReadOnly outerInstance As FileHandler

			Friend ReadOnly out As java.io.OutputStream
			Friend written As Integer

			Friend Sub New(ByVal outerInstance As FileHandler, ByVal out As java.io.OutputStream, ByVal written As Integer)
					Me.outerInstance = outerInstance
				Me.out = out
				Me.written = written
			End Sub

			Public Overrides Sub write(ByVal b As Integer)
				out.write(b)
				written += 1
			End Sub

			Public Overrides Sub write(ByVal buff As SByte())
				out.write(buff)
				written += buff.Length
			End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public  Sub  write(byte buff() , int off, int len) throws java.io.IOException
				out.write(buff,off,len)
				written += len

			public  Sub  flush() throws java.io.IOException
				out.flush()

			public  Sub  close() throws java.io.IOException
				out.close()
		End Class

		private  Sub  open(File fname, Boolean append) throws java.io.IOException
			Dim len As Integer = 0
			If append Then len = CInt(fname.length())
			Dim fout As New java.io.FileOutputStream(fname.ToString(), append)
			Dim bout As New java.io.BufferedOutputStream(fout)
			meter = New MeteredStream(Me, bout, len)
			outputStream = meter

		''' <summary>
		''' Configure a FileHandler from LogManager properties and/or default values
		''' as specified in the class javadoc.
		''' </summary>
		private  Sub  configure()
			Dim manager As LogManager = LogManager.logManager

			Dim cname As String = Me.GetType().name

			pattern = manager.getStringProperty(cname & ".pattern", "%h/java%u.log")
			limit = manager.getIntProperty(cname & ".limit", 0)
			If limit < 0 Then limit = 0
			count = manager.getIntProperty(cname & ".count", 1)
			If count <= 0 Then count = 1
			append = manager.getBooleanProperty(cname & ".append", False)
			level = manager.getLevelProperty(cname & ".level", Level.ALL)
			filter = manager.getFilterProperty(cname & ".filter", Nothing)
			formatter = manager.getFormatterProperty(cname & ".formatter", New XMLFormatter)
			Try
				encoding = manager.getStringProperty(cname & ".encoding", Nothing)
			Catch ex As Exception
				Try
					encoding = Nothing
				Catch ex2 As Exception
					' doing a setEncoding with null should always work.
					' assert false;
				End Try
			End Try


		''' <summary>
		''' Construct a default <tt>FileHandler</tt>.  This will be configured
		''' entirely from <tt>LogManager</tt> properties (or their default values).
		''' <p> </summary>
		''' <exception cref="IOException"> if there are IO problems opening the files. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control"))</tt>. </exception>
		''' <exception cref="NullPointerException"> if pattern property is an empty String. </exception>
		public FileHandler() throws java.io.IOException, SecurityException
			checkPermission()
			configure()
			openFiles()

		''' <summary>
		''' Initialize a <tt>FileHandler</tt> to write to the given filename.
		''' <p>
		''' The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		''' properties (or their default values) except that the given pattern
		''' argument is used as the filename pattern, the file limit is
		''' set to no limit, and the file count is set to one.
		''' <p>
		''' There is no limit on the amount of data that may be written,
		''' so use this with care.
		''' </summary>
		''' <param name="pattern">  the name of the output file </param>
		''' <exception cref="IOException"> if there are IO problems opening the files. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if pattern is an empty string </exception>
		public FileHandler(String pattern) throws java.io.IOException, SecurityException
			If pattern.length() < 1 Then Throw New IllegalArgumentException
			checkPermission()
			configure()
			Me.pattern = pattern
			Me.limit = 0
			Me.count = 1
			openFiles()

		''' <summary>
		''' Initialize a <tt>FileHandler</tt> to write to the given filename,
		''' with optional append.
		''' <p>
		''' The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		''' properties (or their default values) except that the given pattern
		''' argument is used as the filename pattern, the file limit is
		''' set to no limit, the file count is set to one, and the append
		''' mode is set to the given <tt>append</tt> argument.
		''' <p>
		''' There is no limit on the amount of data that may be written,
		''' so use this with care.
		''' </summary>
		''' <param name="pattern">  the name of the output file </param>
		''' <param name="append">  specifies append mode </param>
		''' <exception cref="IOException"> if there are IO problems opening the files. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if pattern is an empty string </exception>
		public FileHandler(String pattern, Boolean append) throws java.io.IOException, SecurityException
			If pattern.length() < 1 Then Throw New IllegalArgumentException
			checkPermission()
			configure()
			Me.pattern = pattern
			Me.limit = 0
			Me.count = 1
			Me.append = append
			openFiles()

		''' <summary>
		''' Initialize a <tt>FileHandler</tt> to write to a set of files.  When
		''' (approximately) the given limit has been written to one file,
		''' another file will be opened.  The output will cycle through a set
		''' of count files.
		''' <p>
		''' The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		''' properties (or their default values) except that the given pattern
		''' argument is used as the filename pattern, the file limit is
		''' set to the limit argument, and the file count is set to the
		''' given count argument.
		''' <p>
		''' The count must be at least 1.
		''' </summary>
		''' <param name="pattern">  the pattern for naming the output file </param>
		''' <param name="limit">  the maximum number of bytes to write to any one file </param>
		''' <param name="count">  the number of files to use </param>
		''' <exception cref="IOException"> if there are IO problems opening the files. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code limit < 0}, or {@code count < 1}. </exception>
		''' <exception cref="IllegalArgumentException"> if pattern is an empty string </exception>
		public FileHandler(String pattern, Integer limit, Integer count) throws java.io.IOException, SecurityException
			If limit < 0 OrElse count < 1 OrElse pattern.length() < 1 Then Throw New IllegalArgumentException
			checkPermission()
			configure()
			Me.pattern = pattern
			Me.limit = limit
			Me.count = count
			openFiles()

		''' <summary>
		''' Initialize a <tt>FileHandler</tt> to write to a set of files
		''' with optional append.  When (approximately) the given limit has
		''' been written to one file, another file will be opened.  The
		''' output will cycle through a set of count files.
		''' <p>
		''' The <tt>FileHandler</tt> is configured based on <tt>LogManager</tt>
		''' properties (or their default values) except that the given pattern
		''' argument is used as the filename pattern, the file limit is
		''' set to the limit argument, and the file count is set to the
		''' given count argument, and the append mode is set to the given
		''' <tt>append</tt> argument.
		''' <p>
		''' The count must be at least 1.
		''' </summary>
		''' <param name="pattern">  the pattern for naming the output file </param>
		''' <param name="limit">  the maximum number of bytes to write to any one file </param>
		''' <param name="count">  the number of files to use </param>
		''' <param name="append">  specifies append mode </param>
		''' <exception cref="IOException"> if there are IO problems opening the files. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code limit < 0}, or {@code count < 1}. </exception>
		''' <exception cref="IllegalArgumentException"> if pattern is an empty string
		'''  </exception>
		public FileHandler(String pattern, Integer limit, Integer count, Boolean append) throws java.io.IOException, SecurityException
			If limit < 0 OrElse count < 1 OrElse pattern.length() < 1 Then Throw New IllegalArgumentException
			checkPermission()
			configure()
			Me.pattern = pattern
			Me.limit = limit
			Me.count = count
			Me.append = append
			openFiles()

		private Boolean isParentWritable(java.nio.file.Path path)
			Dim parent As java.nio.file.Path = path.parent
			If parent Is Nothing Then parent = path.toAbsolutePath().parent
			Return parent IsNot Nothing AndAlso java.nio.file.Files.isWritable(parent)

		''' <summary>
		''' Open the set of output files, based on the configured
		''' instance variables.
		''' </summary>
		private  Sub  openFiles() throws java.io.IOException
			Dim manager As LogManager = LogManager.logManager
			manager.checkPermission()
			If count < 1 Then Throw New IllegalArgumentException("file count = " & count)
			If limit < 0 Then limit = 0

			' We register our own ErrorManager during initialization
			' so we can record exceptions.
			Dim em As New InitializationErrorManager
			errorManager = em

			' Create a lock file.  This grants us exclusive access
			' to our set of output files, as long as we are alive.
			Dim unique As Integer = -1
			Do
				unique += 1
				If unique > MAX_LOCKS Then Throw New java.io.IOException("Couldn't get lock for " & pattern)
				' Generate a lock file name from the "unique" int.
				lockFileName = generate(pattern, 0, unique).ToString() & ".lck"
				' Now try to lock that filename.
				' Because some systems (e.g., Solaris) can only do file locks
				' between processes (and not within a process), we first check
				' if we ourself already have the file locked.
				SyncLock locks
					If locks.contains(lockFileName) Then Continue Do

					Dim lockFilePath As java.nio.file.Path = java.nio.file.Paths.get(lockFileName)
					Dim channel As java.nio.channels.FileChannel = Nothing
					Dim retries As Integer = -1
					Dim fileCreated As Boolean = False
					Dim tempVar As Boolean = channel Is Nothing AndAlso retries < 1
					retries += 1
					Do While tempVar
						Try
							channel = java.nio.channels.FileChannel.open(lockFilePath, CREATE_NEW, WRITE)
							fileCreated = True
						Catch ix As java.nio.file.FileAlreadyExistsException
							' This may be a zombie file left over by a previous
							' execution. Reuse it - but only if we can actually
							' write to its directory.
							' Note that this is a situation that may happen,
							' but not too frequently.
							If java.nio.file.Files.isRegularFile(lockFilePath, java.nio.file.LinkOption.NOFOLLOW_LINKS) AndAlso isParentWritable(lockFilePath) Then
								Try
									channel = java.nio.channels.FileChannel.open(lockFilePath, WRITE, APPEND)
								Catch x As java.nio.file.NoSuchFileException
									' Race condition - retry once, and if that
									' fails again just try the next name in
									' the sequence.
									tempVar = channel Is Nothing AndAlso retries < 1
						retries += 1
									Continue Do
								Catch x As java.io.IOException
									' the file may not be writable for us.
									' try the next name in the sequence
									Exit Do
								End Try
							Else
								' at this point channel should still be null.
								' break and try the next name in the sequence.
								Exit Do
							End If
						End Try
						tempVar = channel Is Nothing AndAlso retries < 1
						retries += 1
					Loop

					If channel Is Nothing Then ' try the next name; Continue Do
					lockFileChannel = channel

					Dim available As Boolean
					Try
						available = lockFileChannel.tryLock() IsNot Nothing
						' We got the lock OK.
						' At this point we could call File.deleteOnExit().
						' However, this could have undesirable side effects
						' as indicated by JDK-4872014. So we will instead
						' rely on the fact that close() will remove the lock
						' file and that whoever is creating FileHandlers should
						' be responsible for closing them.
					Catch ix As java.io.IOException
						' We got an IOException while trying to get the lock.
						' This normally indicates that locking is not supported
						' on the target directory.  We have to proceed without
						' getting a lock.   Drop through, but only if we did
						' create the file...
						available = fileCreated
					Catch x As java.nio.channels.OverlappingFileLockException
						' someone already locked this file in this VM, through
						' some other channel - that is - using something else
						' than new FileHandler(...);
						' continue searching for an available lock.
						available = False
					End Try
					If available Then
						' We got the lock.  Remember it.
						locks.add(lockFileName)
						Exit Do
					End If

					' We failed to get the lock.  Try next file.
					lockFileChannel.close()
				End SyncLock
			Loop

			files = New File(count - 1){}
			For i As Integer = 0 To count - 1
				files(i) = generate(pattern, i, unique)
			Next i

			' Create the initial log file.
			If append Then
				open(files(0), True)
			Else
				rotate()
			End If

			' Did we detect any exceptions during initialization?
			Dim ex As Exception = em.lastException
			If ex IsNot Nothing Then
				If TypeOf ex Is java.io.IOException Then
					Throw CType(ex, java.io.IOException)
				ElseIf TypeOf ex Is SecurityException Then
					Throw CType(ex, SecurityException)
				Else
					Throw New java.io.IOException("Exception: " & ex)
				End If
			End If

			' Install the normal default ErrorManager.
			errorManager = New ErrorManager

		''' <summary>
		''' Generate a file based on a user-supplied pattern, generation number,
		''' and an integer uniqueness suffix </summary>
		''' <param name="pattern"> the pattern for naming the output file </param>
		''' <param name="generation"> the generation number to distinguish rotated logs </param>
		''' <param name="unique"> a unique number to resolve conflicts </param>
		''' <returns> the generated File </returns>
		''' <exception cref="IOException"> </exception>
		private File generate(String pattern, Integer generation, Integer unique) throws java.io.IOException
			Dim file As File = Nothing
			Dim word As String = ""
			Dim ix As Integer = 0
			Dim sawg As Boolean = False
			Dim sawu As Boolean = False
			Do While ix < pattern.length()
				Dim ch As Char = pattern.Chars(ix)
				ix += 1
				Dim ch2 As Char = 0
				If ix < pattern.length() Then ch2 = Char.ToLower(pattern.Chars(ix))
				If ch = "/"c Then
					If file Is Nothing Then
						file = New File(word)
					Else
						file = New File(file, word)
					End If
					word = ""
					Continue Do
				ElseIf ch = "%"c Then
					If ch2 = "t"c Then
						Dim tmpDir As String = System.getProperty("java.io.tmpdir")
						If tmpDir Is Nothing Then tmpDir = System.getProperty("user.home")
						file = New File(tmpDir)
						ix += 1
						word = ""
						Continue Do
					ElseIf ch2 = "h"c Then
						file = New File(System.getProperty("user.home"))
						If uIDUID Then Throw New java.io.IOException("can't use %h in set UID program")
						ix += 1
						word = ""
						Continue Do
					ElseIf ch2 = "g"c Then
						word = word + generation
						sawg = True
						ix += 1
						Continue Do
					ElseIf ch2 = "u"c Then
						word = word + unique
						sawu = True
						ix += 1
						Continue Do
					ElseIf ch2 = "%"c Then
						word = word & "%"
						ix += 1
						Continue Do
					End If
				End If
				word = word + AscW(ch)
			Loop
			If count > 1 AndAlso (Not sawg) Then word = word & "." & generation
			If unique > 0 AndAlso (Not sawu) Then word = word & "." & unique
			If word.length() > 0 Then
				If file Is Nothing Then
					file = New File(word)
				Else
					file = New File(file, word)
				End If
			End If
			Return file

		''' <summary>
		''' Rotate the set of output files
		''' </summary>
		private synchronized  Sub  rotate()
			Dim oldLevel As Level = level
			level = Level.OFF

			MyBase.close()
			For i As Integer = count-2 To 0 Step -1
				Dim f1 As File = files(i)
				Dim f2 As File = files(i+1)
				If f1.exists() Then
					If f2.exists() Then f2.delete()
					f1.renameTo(f2)
				End If
			Next i
			Try
				open(files(0), False)
			Catch ix As java.io.IOException
				' We don't want to throw an exception here, but we
				' report the exception to any registered ErrorManager.
				reportError(Nothing, ix, ErrorManager.OPEN_FAILURE)

			End Try
			level = oldLevel

		''' <summary>
		''' Format and publish a <tt>LogRecord</tt>.
		''' </summary>
		''' <param name="record">  description of the log event. A null record is
		'''                 silently ignored and is not published </param>
		public synchronized  Sub  publish(LogRecord record)
			If Not isLoggable(record) Then Return
			MyBase.publish(record)
			flush()
			If limit > 0 AndAlso meter.written >= limit Then java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

		''' <summary>
		''' Close all the files.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		public synchronized  Sub  close() throws SecurityException
			MyBase.close()
			' Unlock any lock file.
			If lockFileName Is Nothing Then Return
			Try
				' Close the lock file channel (which also will free any locks)
				lockFileChannel.close()
			Catch ex As Exception
				' Problems closing the stream.  Punt.
			End Try
			SyncLock locks
				locks.remove(lockFileName)
			End SyncLock
			If System.IO.Directory.Exists(lockFileName) Then System.IO.Directory.Delete(lockFileName, True) Else System.IO.File.Delete(lockFileName)
			lockFileName = Nothing
			lockFileChannel = Nothing

		private static class InitializationErrorManager extends ErrorManager
			Dim lastException As Exception
			public  Sub  error(String msg, Exception ex, Integer code)
				lastException = ex

		''' <summary>
		''' check if we are in a set UID program.
		''' </summary>
		private static native Boolean uIDUID
	End Class


	Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
		Implements java.security.PrivilegedAction(Of T)

		Public Overrides Function run() As Object
			rotate()
			Return Nothing
		End Function
	End Class
End Namespace
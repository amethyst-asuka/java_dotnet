Imports System
Imports System.Threading

'
' * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' The <seealso cref="ProcessBuilder#start()"/> and
	''' <seealso cref="Runtime#exec(String[],String[],File) Runtime.exec"/>
	''' methods create a native process and return an instance of a
	''' subclass of {@code Process} that can be used to control the process
	''' and obtain information about it.  The class {@code Process}
	''' provides methods for performing input from the process, performing
	''' output to the process, waiting for the process to complete,
	''' checking the exit status of the process, and destroying (killing)
	''' the process.
	''' 
	''' <p>The methods that create processes may not work well for special
	''' processes on certain native platforms, such as native windowing
	''' processes, daemon processes, Win16/DOS processes on Microsoft
	''' Windows, or shell scripts.
	''' 
	''' <p>By default, the created subprocess does not have its own terminal
	''' or console.  All its standard I/O (i.e. stdin, stdout, stderr)
	''' operations will be redirected to the parent process, where they can
	''' be accessed via the streams obtained using the methods
	''' <seealso cref="#getOutputStream()"/>,
	''' <seealso cref="#getInputStream()"/>, and
	''' <seealso cref="#getErrorStream()"/>.
	''' The parent process uses these streams to feed input to and get output
	''' from the subprocess.  Because some native platforms only provide
	''' limited buffer size for standard input and output streams, failure
	''' to promptly write the input stream or read the output stream of
	''' the subprocess may cause the subprocess to block, or even deadlock.
	''' 
	''' <p>Where desired, <a href="ProcessBuilder.html#redirect-input">
	''' subprocess I/O can also be redirected</a>
	''' using methods of the <seealso cref="ProcessBuilder"/> class.
	''' 
	''' <p>The subprocess is not killed when there are no more references to
	''' the {@code Process} object, but rather the subprocess
	''' continues executing asynchronously.
	''' 
	''' <p>There is no requirement that a process represented by a {@code
	''' Process} object execute asynchronously or concurrently with respect
	''' to the Java process that owns the {@code Process} object.
	''' 
	''' <p>As of 1.5, <seealso cref="ProcessBuilder#start()"/> is the preferred way
	''' to create a {@code Process}.
	''' 
	''' @since   JDK1.0
	''' </summary>
	Public MustInherit Class Process
		''' <summary>
		''' Returns the output stream connected to the normal input of the
		''' subprocess.  Output to the stream is piped into the standard
		''' input of the process represented by this {@code Process} object.
		''' 
		''' <p>If the standard input of the subprocess has been redirected using
		''' {@link ProcessBuilder#redirectInput(Redirect)
		''' ProcessBuilder.redirectInput}
		''' then this method will return a
		''' <a href="ProcessBuilder.html#redirect-input">null output stream</a>.
		''' 
		''' <p>Implementation note: It is a good idea for the returned
		''' output stream to be buffered.
		''' </summary>
		''' <returns> the output stream connected to the normal input of the
		'''         subprocess </returns>
		Public MustOverride ReadOnly Property outputStream As OutputStream

		''' <summary>
		''' Returns the input stream connected to the normal output of the
		''' subprocess.  The stream obtains data piped from the standard
		''' output of the process represented by this {@code Process} object.
		''' 
		''' <p>If the standard output of the subprocess has been redirected using
		''' {@link ProcessBuilder#redirectOutput(Redirect)
		''' ProcessBuilder.redirectOutput}
		''' then this method will return a
		''' <a href="ProcessBuilder.html#redirect-output">null input stream</a>.
		''' 
		''' <p>Otherwise, if the standard error of the subprocess has been
		''' redirected using
		''' {@link ProcessBuilder#redirectErrorStream(boolean)
		''' ProcessBuilder.redirectErrorStream}
		''' then the input stream returned by this method will receive the
		''' merged standard output and the standard error of the subprocess.
		''' 
		''' <p>Implementation note: It is a good idea for the returned
		''' input stream to be buffered.
		''' </summary>
		''' <returns> the input stream connected to the normal output of the
		'''         subprocess </returns>
		Public MustOverride ReadOnly Property inputStream As InputStream

		''' <summary>
		''' Returns the input stream connected to the error output of the
		''' subprocess.  The stream obtains data piped from the error output
		''' of the process represented by this {@code Process} object.
		''' 
		''' <p>If the standard error of the subprocess has been redirected using
		''' {@link ProcessBuilder#redirectError(Redirect)
		''' ProcessBuilder.redirectError} or
		''' {@link ProcessBuilder#redirectErrorStream(boolean)
		''' ProcessBuilder.redirectErrorStream}
		''' then this method will return a
		''' <a href="ProcessBuilder.html#redirect-output">null input stream</a>.
		''' 
		''' <p>Implementation note: It is a good idea for the returned
		''' input stream to be buffered.
		''' </summary>
		''' <returns> the input stream connected to the error output of
		'''         the subprocess </returns>
		Public MustOverride ReadOnly Property errorStream As InputStream

		''' <summary>
		''' Causes the current thread to wait, if necessary, until the
		''' process represented by this {@code Process} object has
		''' terminated.  This method returns immediately if the subprocess
		''' has already terminated.  If the subprocess has not yet
		''' terminated, the calling thread will be blocked until the
		''' subprocess exits.
		''' </summary>
		''' <returns> the exit value of the subprocess represented by this
		'''         {@code Process} object.  By convention, the value
		'''         {@code 0} indicates normal termination. </returns>
		''' <exception cref="InterruptedException"> if the current thread is
		'''         <seealso cref="Thread#interrupt() interrupted"/> by another
		'''         thread while it is waiting, then the wait is ended and
		'''         an <seealso cref="InterruptedException"/> is thrown. </exception>
		Public MustOverride Function waitFor() As Integer

		''' <summary>
		''' Causes the current thread to wait, if necessary, until the
		''' subprocess represented by this {@code Process} object has
		''' terminated, or the specified waiting time elapses.
		''' 
		''' <p>If the subprocess has already terminated then this method returns
		''' immediately with the value {@code true}.  If the process has not
		''' terminated and the timeout value is less than, or equal to, zero, then
		''' this method returns immediately with the value {@code false}.
		''' 
		''' <p>The default implementation of this methods polls the {@code exitValue}
		''' to check if the process has terminated. Concrete implementations of this
		''' class are strongly encouraged to override this method with a more
		''' efficient implementation.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the {@code timeout} argument </param>
		''' <returns> {@code true} if the subprocess has exited and {@code false} if
		'''         the waiting time elapsed before the subprocess has exited. </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		'''         while waiting. </exception>
		''' <exception cref="NullPointerException"> if unit is null
		''' @since 1.8 </exception>
		Public Overridable Function waitFor(  timeout As Long,   unit As java.util.concurrent.TimeUnit) As Boolean
			Dim startTime As Long = System.nanoTime()
			Dim [rem] As Long = unit.toNanos(timeout)

			Do
				Try
					exitValue()
					Return True
				Catch ex As IllegalThreadStateException
					If [rem] > 0 Then Thread.Sleep (System.Math.Min(java.util.concurrent.TimeUnit.NANOSECONDS.toMillis([rem]) + 1, 100))
				End Try
				[rem] = unit.toNanos(timeout) - (System.nanoTime() - startTime)
			Loop While [rem] > 0
			Return False
		End Function

		''' <summary>
		''' Returns the exit value for the subprocess.
		''' </summary>
		''' <returns> the exit value of the subprocess represented by this
		'''         {@code Process} object.  By convention, the value
		'''         {@code 0} indicates normal termination. </returns>
		''' <exception cref="IllegalThreadStateException"> if the subprocess represented
		'''         by this {@code Process} object has not yet terminated </exception>
		Public MustOverride Function exitValue() As Integer

		''' <summary>
		''' Kills the subprocess. Whether the subprocess represented by this
		''' {@code Process} object is forcibly terminated or not is
		''' implementation dependent.
		''' </summary>
		Public MustOverride Sub destroy()

		''' <summary>
		''' Kills the subprocess. The subprocess represented by this
		''' {@code Process} object is forcibly terminated.
		''' 
		''' <p>The default implementation of this method invokes <seealso cref="#destroy"/>
		''' and so may not forcibly terminate the process. Concrete implementations
		''' of this class are strongly encouraged to override this method with a
		''' compliant implementation.  Invoking this method on {@code Process}
		''' objects returned by <seealso cref="ProcessBuilder#start"/> and
		''' <seealso cref="Runtime#exec"/> will forcibly terminate the process.
		''' 
		''' <p>Note: The subprocess may not terminate immediately.
		''' i.e. {@code isAlive()} may return true for a brief period
		''' after {@code destroyForcibly()} is called. This method
		''' may be chained to {@code waitFor()} if needed.
		''' </summary>
		''' <returns> the {@code Process} object representing the
		'''         subprocess to be forcibly destroyed.
		''' @since 1.8 </returns>
		Public Overridable Function destroyForcibly() As Process
			destroy()
			Return Me
		End Function

		''' <summary>
		''' Tests whether the subprocess represented by this {@code Process} is
		''' alive.
		''' </summary>
		''' <returns> {@code true} if the subprocess represented by this
		'''         {@code Process} object has not yet terminated.
		''' @since 1.8 </returns>
		Public Overridable Property alive As Boolean
			Get
				Try
					exitValue()
					Return False
				Catch e As IllegalThreadStateException
					Return True
				End Try
			End Get
		End Property
	End Class

End Namespace
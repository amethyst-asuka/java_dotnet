'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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



Namespace javax.swing






	''' <summary>
	''' Monitors the progress of reading from some InputStream. This ProgressMonitor
	''' is normally invoked in roughly this form:
	''' <pre>
	''' InputStream in = new BufferedInputStream(
	'''                          new ProgressMonitorInputStream(
	'''                                  parentComponent,
	'''                                  "Reading " + fileName,
	'''                                  new FileInputStream(fileName)));
	''' </pre><p>
	''' This creates a progress monitor to monitor the progress of reading
	''' the input stream.  If it's taking a while, a ProgressDialog will
	''' be popped up to inform the user.  If the user hits the Cancel button
	''' an InterruptedIOException will be thrown on the next read.
	''' All the right cleanup is done when the stream is closed.
	''' 
	''' 
	''' <p>
	''' 
	''' For further documentation and examples see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/progress.html">How to Monitor Progress</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' </summary>
	''' <seealso cref= ProgressMonitor </seealso>
	''' <seealso cref= JOptionPane
	''' @author James Gosling </seealso>
	Public Class ProgressMonitorInputStream
		Inherits FilterInputStream

		Private monitor As ProgressMonitor
		Private nread As Integer = 0
		Private size As Integer = 0


		''' <summary>
		''' Constructs an object to monitor the progress of an input stream.
		''' </summary>
		''' <param name="message"> Descriptive text to be placed in the dialog box
		'''                if one is popped up. </param>
		''' <param name="parentComponent"> The component triggering the operation
		'''                        being monitored. </param>
		''' <param name="in"> The input stream to be monitored. </param>
		Public Sub New(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal [in] As InputStream)
			MyBase.New([in])
			Try
				size = [in].available()
			Catch ioe As IOException
				size = 0
			End Try
			monitor = New ProgressMonitor(parentComponent, message, Nothing, 0, size)
		End Sub


		''' <summary>
		''' Get the ProgressMonitor object being used by this stream. Normally
		''' this isn't needed unless you want to do something like change the
		''' descriptive text partway through reading the file. </summary>
		''' <returns> the ProgressMonitor object used by this object </returns>
		Public Overridable Property progressMonitor As ProgressMonitor
			Get
				Return monitor
			End Get
		End Property


		''' <summary>
		''' Overrides <code>FilterInputStream.read</code>
		''' to update the progress monitor after the read.
		''' </summary>
		Public Overridable Function read() As Integer
			Dim c As Integer = in.read()
			If c >= 0 Then
				nread += 1
				monitor.progress = nread
			End If
			If monitor.canceled Then
				Dim exc As New InterruptedIOException("progress")
				exc.bytesTransferred = nread
				Throw exc
			End If
			Return c
		End Function


		''' <summary>
		''' Overrides <code>FilterInputStream.read</code>
		''' to update the progress monitor after the read.
		''' </summary>
		Public Overridable Function read(ByVal b As SByte()) As Integer
			Dim nr As Integer = in.read(b)
			If nr > 0 Then monitor.progress = nread += nr
			If monitor.canceled Then
				Dim exc As New InterruptedIOException("progress")
				exc.bytesTransferred = nread
				Throw exc
			End If
			Return nr
		End Function


		''' <summary>
		''' Overrides <code>FilterInputStream.read</code>
		''' to update the progress monitor after the read.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(byte b() , int off, int len) throws IOException
			Dim nr As Integer = in.read(b, off, len)
			If nr > 0 Then monitor.progress = nread += nr
			If monitor.canceled Then
				Dim exc As New InterruptedIOException("progress")
				exc.bytesTransferred = nread
				Throw exc
			End If
			Return nr


		''' <summary>
		''' Overrides <code>FilterInputStream.skip</code>
		''' to update the progress monitor after the skip.
		''' </summary>
		public Long skip(Long n) throws IOException
			Dim nr As Long = in.skip(n)
			If nr > 0 Then monitor.progress = nread += nr
			Return nr


		''' <summary>
		''' Overrides <code>FilterInputStream.close</code>
		''' to close the progress monitor as well as the stream.
		''' </summary>
		public void close() throws IOException
			in.close()
			monitor.close()


		''' <summary>
		''' Overrides <code>FilterInputStream.reset</code>
		''' to reset the progress monitor as well as the stream.
		''' </summary>
		public synchronized void reset() throws IOException
			in.reset()
			nread = size - in.available()
			monitor.progress = nread
	End Class

End Namespace
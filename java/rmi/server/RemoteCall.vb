Imports System

'
' * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.server

	''' <summary>
	''' <code>RemoteCall</code> is an abstraction used solely by the RMI runtime
	''' (in conjunction with stubs and skeletons of remote objects) to carry out a
	''' call to a remote object.  The <code>RemoteCall</code> interface is
	''' deprecated because it is only used by deprecated methods of
	''' <code>java.rmi.server.RemoteRef</code>.
	''' 
	''' @since   JDK1.1
	''' @author  Ann Wollrath
	''' @author  Roger Riggs </summary>
	''' <seealso cref=     java.rmi.server.RemoteRef </seealso>
	''' @deprecated no replacement. 
	<Obsolete("no replacement.")> _
	Public Interface RemoteCall
		''' <summary>
		''' Return the output stream the stub/skeleton should put arguments/results
		''' into.
		''' </summary>
		''' <returns> output stream for arguments/results </returns>
		''' <exception cref="java.io.IOException"> if an I/O error occurs.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		ReadOnly Property outputStream As java.io.ObjectOutput

		''' <summary>
		''' Release the output stream; in some transports this would release
		''' the stream.
		''' </summary>
		''' <exception cref="java.io.IOException"> if an I/O error occurs.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Sub releaseOutputStream()

		''' <summary>
		''' Get the InputStream that the stub/skeleton should get
		''' results/arguments from.
		''' </summary>
		''' <returns> input stream for reading arguments/results </returns>
		''' <exception cref="java.io.IOException"> if an I/O error occurs.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		ReadOnly Property inputStream As java.io.ObjectInput


		''' <summary>
		''' Release the input stream. This would allow some transports to release
		''' the channel early.
		''' </summary>
		''' <exception cref="java.io.IOException"> if an I/O error occurs.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Sub releaseInputStream()

		''' <summary>
		''' Returns an output stream (may put out header information
		''' relating to the success of the call). Should only succeed
		''' once per remote call.
		''' </summary>
		''' <param name="success"> If true, indicates normal return, else indicates
		''' exceptional return. </param>
		''' <returns> output stream for writing call result </returns>
		''' <exception cref="java.io.IOException">              if an I/O error occurs. </exception>
		''' <exception cref="java.io.StreamCorruptedException"> If already been called.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Function getResultStream(ByVal success As Boolean) As java.io.ObjectOutput

		''' <summary>
		''' Do whatever it takes to execute the call.
		''' </summary>
		''' <exception cref="java.lang.Exception"> if a general exception occurs.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Sub executeCall()

		''' <summary>
		''' Allow cleanup after the remote call has completed.
		''' </summary>
		''' <exception cref="java.io.IOException"> if an I/O error occurs.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Sub done()
	End Interface

End Namespace
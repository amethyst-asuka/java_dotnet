Imports System

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.registry


	''' <summary>
	''' <code>RegistryHandler</code> is an interface used internally by the RMI
	''' runtime in previous implementation versions.  It should never be accessed
	''' by application code.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1 </summary>
	''' @deprecated no replacement 
	<Obsolete("no replacement")> _
	Public Interface RegistryHandler
		''' <summary>
		''' Returns a "stub" for contacting a remote registry
		''' on the specified host and port.
		''' </summary>
		''' @deprecated no replacement.  As of the Java 2 platform v1.2, RMI no
		''' longer uses the <code>RegistryHandler</code> to obtain the registry's
		''' stub. 
		''' <param name="host"> name of remote registry host </param>
		''' <param name="port"> remote registry port </param>
		''' <returns> remote registry stub </returns>
		''' <exception cref="RemoteException"> if a remote error occurs </exception>
		''' <exception cref="UnknownHostException"> if unable to resolve given hostname </exception>
		<Obsolete("no replacement.  As of the Java 2 platform v1.2, RMI no")> _
		Function registryStub(  host As String,   port As Integer) As Registry

		''' <summary>
		''' Constructs and exports a Registry on the specified port.
		''' The port must be non-zero.
		''' </summary>
		''' @deprecated no replacement.  As of the Java 2 platform v1.2, RMI no
		''' longer uses the <code>RegistryHandler</code> to obtain the registry's
		''' implementation. 
		''' <param name="port"> port to export registry on </param>
		''' <returns> registry stub </returns>
		''' <exception cref="RemoteException"> if a remote error occurs </exception>
		<Obsolete("no replacement.  As of the Java 2 platform v1.2, RMI no")> _
		Function registryImpl(  port As Integer) As Registry
	End Interface

End Namespace
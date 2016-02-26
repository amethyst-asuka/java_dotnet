'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.remote

	''' <summary>
	''' <p>Implemented by objects that can have a {@code JMXServiceURL} address.
	''' All <seealso cref="JMXConnectorServer"/> objects implement this interface.
	''' Depending on the connector implementation, a <seealso cref="JMXConnector"/>
	''' object may implement this interface too.  {@code JMXConnector}
	''' objects for the RMI Connector are instances of
	''' <seealso cref="javax.management.remote.rmi.RMIConnector RMIConnector"/> which
	''' implements this interface.</p>
	''' 
	''' <p>An object implementing this interface might not have an address
	''' at a given moment.  This is indicated by a null return value from
	''' <seealso cref="#getAddress()"/>.</p>
	''' 
	''' @since 1.6
	''' </summary>
	Public Interface JMXAddressable
		''' <summary>
		''' <p>The address of this object.</p>
		''' </summary>
		''' <returns> the address of this object, or null if it
		''' does not have one. </returns>
		ReadOnly Property address As JMXServiceURL
	End Interface

End Namespace
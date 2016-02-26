'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth

	''' <summary>
	''' Objects such as credentials may optionally implement this
	''' interface to provide the capability to refresh itself.
	''' For example, a credential with a particular time-restricted lifespan
	''' may implement this interface to allow callers to refresh the time period
	''' for which it is valid.
	''' </summary>
	''' <seealso cref= javax.security.auth.Subject </seealso>
	Public Interface Refreshable

		''' <summary>
		''' Determine if this {@code Object} is current.
		''' 
		''' <p>
		''' </summary>
		''' <returns> true if this {@code Object} is currently current,
		'''          false otherwise. </returns>
		ReadOnly Property current As Boolean

		''' <summary>
		''' Update or extend the validity period for this
		''' {@code Object}.
		''' 
		''' <p>
		''' </summary>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to update or extend the validity period for this
		'''          {@code Object}. <p>
		''' </exception>
		''' <exception cref="RefreshFailedException"> if the refresh attempt failed. </exception>
		Sub refresh()
	End Interface

End Namespace
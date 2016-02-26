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
	''' Objects such as credentials may optionally implement this interface
	''' to provide the capability to destroy its contents.
	''' </summary>
	''' <seealso cref= javax.security.auth.Subject </seealso>
	Public Interface Destroyable

		''' <summary>
		''' Destroy this {@code Object}.
		''' 
		''' <p> Sensitive information associated with this {@code Object}
		''' is destroyed or cleared.  Subsequent calls to certain methods
		''' on this {@code Object} will result in an
		''' {@code IllegalStateException} being thrown.
		''' 
		''' <p>
		''' The default implementation throws {@code DestroyFailedException}.
		''' </summary>
		''' <exception cref="DestroyFailedException"> if the destroy operation fails. <p>
		''' </exception>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to destroy this {@code Object}. </exception>
		default Sub destroy()
			throw Function DestroyFailedException() As New

		''' <summary>
		''' Determine if this {@code Object} has been destroyed.
		''' 
		''' <p>
		''' The default implementation returns false.
		''' </summary>
		''' <returns> true if this {@code Object} has been destroyed,
		'''          false otherwise. </returns>
		ReadOnly Property default destroyed As Boolean
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return False;
	End Interface

End Namespace
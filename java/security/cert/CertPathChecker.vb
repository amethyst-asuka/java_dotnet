'
' * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.cert

	''' <summary>
	''' <p>Performs one or more checks on each {@code Certificate} of a
	''' {@code CertPath}.
	''' 
	''' <p>A {@code CertPathChecker} implementation is typically created to extend
	''' a certification path validation algorithm. For example, an implementation
	''' may check for and process a critical private extension of each certificate
	''' in a certification path.
	''' 
	''' @since 1.8
	''' </summary>
	Public Interface CertPathChecker

		''' <summary>
		''' Initializes the internal state of this {@code CertPathChecker}.
		''' 
		''' <p>The {@code forward} flag specifies the order that certificates will
		''' be passed to the <seealso cref="#check check"/> method (forward or reverse).
		''' </summary>
		''' <param name="forward"> the order that certificates are presented to the
		'''        {@code check} method. If {@code true}, certificates are
		'''        presented from target to trust anchor (forward); if
		'''        {@code false}, from trust anchor to target (reverse). </param>
		''' <exception cref="CertPathValidatorException"> if this {@code CertPathChecker} is
		'''         unable to check certificates in the specified order </exception>
		Sub init(  forward As Boolean)

		''' <summary>
		''' Indicates if forward checking is supported. Forward checking refers
		''' to the ability of the {@code CertPathChecker} to perform its checks
		''' when certificates are presented to the {@code check} method in the
		''' forward direction (from target to trust anchor).
		''' </summary>
		''' <returns> {@code true} if forward checking is supported, {@code false}
		'''         otherwise </returns>
		ReadOnly Property forwardCheckingSupported As Boolean

		''' <summary>
		''' Performs the check(s) on the specified certificate using its internal
		''' state. The certificates are presented in the order specified by the
		''' {@code init} method.
		''' </summary>
		''' <param name="cert"> the {@code Certificate} to be checked </param>
		''' <exception cref="CertPathValidatorException"> if the specified certificate does
		'''         not pass the check </exception>
		Sub check(  cert As Certificate)
	End Interface

End Namespace
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

Namespace java.security.cert

	''' <summary>
	''' A specification of the result of a certification path builder algorithm.
	''' All results returned by the {@link CertPathBuilder#build
	''' CertPathBuilder.build} method must implement this interface.
	''' <p>
	''' At a minimum, a {@code CertPathBuilderResult} contains the
	''' {@code CertPath} built by the {@code CertPathBuilder} instance.
	''' Implementations of this interface may add methods to return implementation
	''' or algorithm specific information, such as debugging information or
	''' certification path validation results.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this interface are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CertPathBuilder
	''' 
	''' @since       1.4
	''' @author      Sean Mullan </seealso>
	Public Interface CertPathBuilderResult
		Inherits Cloneable

		''' <summary>
		''' Returns the built certification path.
		''' </summary>
		''' <returns> the certification path (never {@code null}) </returns>
		ReadOnly Property certPath As CertPath

		''' <summary>
		''' Makes a copy of this {@code CertPathBuilderResult}. Changes to the
		''' copy will not affect the original and vice versa.
		''' </summary>
		''' <returns> a copy of this {@code CertPathBuilderResult} </returns>
		Function clone() As Object
	End Interface

End Namespace
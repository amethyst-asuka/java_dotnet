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
	''' A selector that defines a set of criteria for selecting {@code CRL}s.
	''' Classes that implement this interface are often used to specify
	''' which {@code CRL}s should be retrieved from a {@code CertStore}.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this interface are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CRL </seealso>
	''' <seealso cref= CertStore </seealso>
	''' <seealso cref= CertStore#getCRLs
	''' 
	''' @author      Steve Hanna
	''' @since       1.4 </seealso>
	Public Interface CRLSelector
		Inherits Cloneable

		''' <summary>
		''' Decides whether a {@code CRL} should be selected.
		''' </summary>
		''' <param name="crl">     the {@code CRL} to be checked </param>
		''' <returns>  {@code true} if the {@code CRL} should be selected,
		''' {@code false} otherwise </returns>
		Function match(ByVal crl As CRL) As Boolean

		''' <summary>
		''' Makes a copy of this {@code CRLSelector}. Changes to the
		''' copy will not affect the original and vice versa.
		''' </summary>
		''' <returns> a copy of this {@code CRLSelector} </returns>
		Function clone() As Object
	End Interface

End Namespace
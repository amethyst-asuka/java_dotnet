Imports Microsoft.VisualBasic
Imports System.Collections.Generic

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
	''' Parameters used as input for the Collection {@code CertStore}
	''' algorithm.
	''' <p>
	''' This class is used to provide necessary configuration parameters
	''' to implementations of the Collection {@code CertStore}
	''' algorithm. The only parameter included in this class is the
	''' {@code Collection} from which the {@code CertStore} will
	''' retrieve certificates and CRLs.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' 
	''' @since       1.4
	''' @author      Steve Hanna </summary>
	''' <seealso cref=         java.util.Collection </seealso>
	''' <seealso cref=         CertStore </seealso>
	Public Class CollectionCertStoreParameters
		Implements CertStoreParameters

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private coll As ICollection(Of ?)

		''' <summary>
		''' Creates an instance of {@code CollectionCertStoreParameters}
		''' which will allow certificates and CRLs to be retrieved from the
		''' specified {@code Collection}. If the specified
		''' {@code Collection} contains an object that is not a
		''' {@code Certificate} or {@code CRL}, that object will be
		''' ignored by the Collection {@code CertStore}.
		''' <p>
		''' The {@code Collection} is <b>not</b> copied. Instead, a
		''' reference is used. This allows the caller to subsequently add or
		''' remove {@code Certificates} or {@code CRL}s from the
		''' {@code Collection}, thus changing the set of
		''' {@code Certificates} or {@code CRL}s available to the
		''' Collection {@code CertStore}. The Collection {@code CertStore}
		''' will not modify the contents of the {@code Collection}.
		''' <p>
		''' If the {@code Collection} will be modified by one thread while
		''' another thread is calling a method of a Collection {@code CertStore}
		''' that has been initialized with this {@code Collection}, the
		''' {@code Collection} must have fail-fast iterators.
		''' </summary>
		''' <param name="collection"> a {@code Collection} of
		'''        {@code Certificate}s and {@code CRL}s </param>
		''' <exception cref="NullPointerException"> if {@code collection} is
		''' {@code null} </exception>
		Public Sub New(Of T1)(ByVal collection As ICollection(Of T1))
			If collection Is Nothing Then Throw New NullPointerException
			coll = collection
		End Sub

		''' <summary>
		''' Creates an instance of {@code CollectionCertStoreParameters} with
		''' the default parameter values (an empty and immutable
		''' {@code Collection}).
		''' </summary>
		Public Sub New()
			coll = java.util.Collections.EMPTY_SET
		End Sub

		''' <summary>
		''' Returns the {@code Collection} from which {@code Certificate}s
		''' and {@code CRL}s are retrieved. This is <b>not</b> a copy of the
		''' {@code Collection}, it is a reference. This allows the caller to
		''' subsequently add or remove {@code Certificates} or
		''' {@code CRL}s from the {@code Collection}.
		''' </summary>
		''' <returns> the {@code Collection} (never null) </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property collection As ICollection(Of ?)
			Get
				Return coll
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this object. Note that only a reference to the
		''' {@code Collection} is copied, and not the contents.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object Implements CertStoreParameters.clone
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Cannot happen 
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function

		''' <summary>
		''' Returns a formatted string describing the parameters.
		''' </summary>
		''' <returns> a formatted string describing the parameters </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("CollectionCertStoreParameters: [" & vbLf)
			sb.append("  collection: " & coll & vbLf)
			sb.append("]")
			Return sb.ToString()
		End Function
	End Class

End Namespace
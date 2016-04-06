Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2002, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' Helper class that allows the Sun CertPath provider to access
	''' implementation dependent APIs in CertPath framework.
	''' 
	''' @author Andreas Sterbenz
	''' </summary>
	Friend Class CertPathHelperImpl
		Inherits sun.security.provider.certpath.CertPathHelper

		Private Sub New()
			' empty
		End Sub

		''' <summary>
		''' Initialize the helper framework. This method must be called from
		''' the static initializer of each class that is the target of one of
		''' the methods in this class. This ensures that the helper is initialized
		''' prior to a tunneled call from the Sun provider.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub initialize()
			If sun.security.provider.certpath.CertPathHelper.instance Is Nothing Then sun.security.provider.certpath.CertPathHelper.instance = New CertPathHelperImpl
		End Sub

		Protected Friend Overridable Sub implSetPathToNames(  sel As X509CertSelector,   names As [Set](Of sun.security.x509.GeneralNameInterface))
			sel.pathToNamesInternal = names
		End Sub

		Protected Friend Overridable Sub implSetDateAndTime(  sel As X509CRLSelector,   [date] As Date,   skew As Long)
			sel.dateAndTimeime(date_Renamed, skew)
		End Sub
	End Class

End Namespace
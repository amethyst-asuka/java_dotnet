'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.ldap

	''' <summary>
	''' Requests that referral and other special LDAP objects be manipulated
	''' as normal LDAP objects. It enables the requestor to interrogate or
	''' update such objects.
	''' <p>
	''' This class implements the LDAPv3 Request Control for ManageDsaIT
	''' as defined in
	''' <a href="http://www.ietf.org/rfc/rfc3296.txt">RFC 3296</a>.
	''' 
	''' The control has no control value.
	''' 
	''' @since 1.5
	''' @author Vincent Ryan
	''' </summary>
	Public NotInheritable Class ManageReferralControl
		Inherits BasicControl

		''' <summary>
		''' The ManageReferral control's assigned object identifier
		''' is 2.16.840.1.113730.3.4.2.
		''' </summary>
		Public Const OID As String = "2.16.840.1.113730.3.4.2"

		Private Const serialVersionUID As Long = 3017756160149982566L

		''' <summary>
		''' Constructs a critical ManageReferral control.
		''' </summary>
		Public Sub New()
			MyBase.New(OID, True, Nothing)
		End Sub

		''' <summary>
		''' Constructs a ManageReferral control.
		''' </summary>
		''' <param name="criticality"> The control's criticality setting. </param>
		Public Sub New(ByVal criticality As Boolean)
			MyBase.New(OID, criticality, Nothing)
		End Sub
	End Class

End Namespace
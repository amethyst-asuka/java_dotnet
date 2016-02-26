'
' * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.kerberos


	Friend Class JavaxSecurityAuthKerberosAccessImpl
		Implements sun.security.krb5.JavaxSecurityAuthKerberosAccess

		Public Overridable Function keyTabTakeSnapshot(ByVal ktab As KeyTab) As sun.security.krb5.internal.ktab.KeyTab
			Return ktab.takeSnapshot()
		End Function
	End Class

End Namespace
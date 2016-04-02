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

Namespace java.net

	''' <summary>
	''' Choose a network interface to be the default for
	''' outgoing IPv6 traffic that does not specify a scope_id (and which needs one).
	''' 
	''' Platforms that do not require a default interface may return null
	''' which is what this implementation does.
	''' </summary>

	Friend Class DefaultInterface

		FriendShared ReadOnly Property[default] As NetworkInterface
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace
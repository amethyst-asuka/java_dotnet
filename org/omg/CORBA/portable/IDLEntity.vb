'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA.portable

	''' <summary>
	''' An interface with no members whose only purpose is to serve as a marker
	''' indicating  that an implementing class is a
	''' Java value type from IDL that has a corresponding Helper class.
	''' RMI IIOP serialization looks for such a marker to perform
	''' marshalling/unmarshalling.
	''' 
	''' </summary>
	Public Interface IDLEntity
		Inherits java.io.Serializable

	End Interface

End Namespace
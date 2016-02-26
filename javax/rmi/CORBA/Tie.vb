'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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
'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace javax.rmi.CORBA



	''' <summary>
	''' Defines methods which all RMI-IIOP server side ties must implement.
	''' </summary>
	Public Interface Tie
		Inherits org.omg.CORBA.portable.InvokeHandler

		''' <summary>
		''' Returns an object reference for the target object represented by
		''' this tie. </summary>
		''' <returns> an object reference for the target object. </returns>
		Function thisObject() As org.omg.CORBA.Object

		''' <summary>
		''' Deactivates the target object represented by this tie.
		''' </summary>
		Sub deactivate()

		''' <summary>
		''' Returns the ORB for this tie. </summary>
		''' <returns> the ORB. </returns>
		Function orb() As org.omg.CORBA.ORB

		''' <summary>
		''' Sets the ORB for this tie. </summary>
		''' <param name="orb"> the ORB. </param>
		Sub orb(ByVal orb As org.omg.CORBA.ORB)

		''' <summary>
		''' Called by <seealso cref="Util#registerTarget"/> to set the target
		''' for this tie. </summary>
		''' <param name="target"> the object to use as the target for this tie. </param>
		Property target As java.rmi.Remote

	End Interface

End Namespace
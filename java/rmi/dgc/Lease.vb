Imports System

'
' * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.rmi.dgc

	''' <summary>
	''' A lease contains a unique VM identifier and a lease duration. A
	''' Lease object is used to request and grant leases to remote object
	''' references.
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Lease

		''' <summary>
		''' @serial Virtual Machine ID with which this Lease is associated. </summary>
		''' <seealso cref= #getVMID </seealso>
		Private vmid As VMID

		''' <summary>
		''' @serial Duration of this lease. </summary>
		''' <seealso cref= #getValue </seealso>
		Private value As Long
		''' <summary>
		''' indicate compatibility with JDK 1.1.x version of class </summary>
		Private Const serialVersionUID As Long = -5713411624328831948L

		''' <summary>
		''' Constructs a lease with a specific VMID and lease duration. The
		''' vmid may be null. </summary>
		''' <param name="id"> VMID associated with this lease </param>
		''' <param name="duration"> lease duration </param>
		Public Sub New(ByVal id As VMID, ByVal duration As Long)
			vmid = id
			value = duration
		End Sub

		''' <summary>
		''' Returns the client VMID associated with the lease. </summary>
		''' <returns> client VMID </returns>
		Public Property vMID As VMID
			Get
				Return vmid
			End Get
		End Property

		''' <summary>
		''' Returns the lease duration. </summary>
		''' <returns> lease duration </returns>
		Public Property value As Long
			Get
				Return value
			End Get
		End Property
	End Class

End Namespace
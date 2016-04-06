Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A VMID is a identifier that is unique across all Java virtual
	''' machines.  VMIDs are used by the distributed garbage collector
	''' to identify client VMs.
	''' 
	''' @author      Ann Wollrath
	''' @author      Peter Jones
	''' </summary>
	<Serializable> _
	Public NotInheritable Class VMID
		''' <summary>
		''' Array of bytes uniquely identifying this host </summary>
		Private Shared ReadOnly randomBytes As SByte()

		''' <summary>
		''' @serial array of bytes uniquely identifying host created on
		''' </summary>
		Private addr As SByte()

		''' <summary>
		''' @serial unique identifier with respect to host created on
		''' </summary>
		Private uid As java.rmi.server.UID

		''' <summary>
		''' indicate compatibility with JDK 1.1.x version of class </summary>
		Private Const serialVersionUID As Long = -538642295484486218L

		Shared Sub New()
			' Generate 8 bytes of random data.
			Dim secureRandom As New java.security.SecureRandom
			Dim bytes As SByte() = New SByte(7){}
			secureRandom.nextBytes(bytes)
			randomBytes = bytes
		End Sub

		''' <summary>
		''' Create a new VMID.  Each new VMID returned from this constructor
		''' is unique for all Java virtual machines under the following
		''' conditions: a) the conditions for uniqueness for objects of
		''' the class <code>java.rmi.server.UID</code> are satisfied, and b) an
		''' address can be obtained for this host that is unique and constant
		''' for the lifetime of this object.
		''' </summary>
		Public Sub New()
			addr = randomBytes
			uid = New java.rmi.server.UID
		End Sub

		''' <summary>
		''' Return true if an accurate address can be determined for this
		''' host.  If false, reliable VMID cannot be generated from this host </summary>
		''' <returns> true if host address can be determined, false otherwise
		''' @deprecated </returns>
		<Obsolete> _
		PublicShared ReadOnly Propertyunique As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Compute hash code for this VMID.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return uid.GetHashCode()
		End Function

		''' <summary>
		''' Compare this VMID to another, and return true if they are the
		''' same identifier.
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If TypeOf obj Is VMID Then
				Dim vmid As VMID = CType(obj, VMID)
				If Not uid.Equals(vmid.uid) Then Return False
				If (addr Is Nothing) Xor (vmid.addr Is Nothing) Then Return False
				If addr IsNot Nothing Then
					If addr.Length <> vmid.addr.Length Then Return False
					For i As Integer = 0 To addr.Length - 1
						If addr(i) <> vmid.addr(i) Then Return False
					Next i
				End If
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Return string representation of this VMID.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim result As New StringBuffer
			If addr IsNot Nothing Then
				For i As Integer = 0 To addr.Length - 1
					Dim x As Integer = addr(i) And &HFF
					result.append((If(x < &H10, "0", "")) + Convert.ToString(x, 16))
				Next i
			End If
			result.append(":"c)
			result.append(uid.ToString())
			Return result.ToString()
		End Function
	End Class

End Namespace
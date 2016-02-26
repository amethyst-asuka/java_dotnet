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
	''' This class provides a basic implementation of the <tt>Control</tt>
	''' interface. It represents an LDAPv3 Control as defined in
	''' <a href="http://www.ietf.org/rfc/rfc2251.txt">RFC 2251</a>.
	''' 
	''' @since 1.5
	''' @author Vincent Ryan
	''' </summary>
	Public Class BasicControl
		Implements Control

		''' <summary>
		''' The control's object identifier string.
		''' 
		''' @serial
		''' </summary>
		Protected Friend id As String

		''' <summary>
		''' The control's criticality.
		''' 
		''' @serial
		''' </summary>
		Protected Friend criticality As Boolean = False ' default

		''' <summary>
		''' The control's ASN.1 BER encoded value.
		''' 
		''' @serial
		''' </summary>
		Protected Friend value As SByte() = Nothing

		Private Const serialVersionUID As Long = -4233907508771791687L

		''' <summary>
		''' Constructs a non-critical control.
		''' </summary>
		''' <param name="id">      The control's object identifier string.
		'''  </param>
		Public Sub New(ByVal id As String)
			Me.id = id
		End Sub

		''' <summary>
		''' Constructs a control using the supplied arguments.
		''' </summary>
		''' <param name="id">              The control's object identifier string. </param>
		''' <param name="criticality">     The control's criticality. </param>
		''' <param name="value">           The control's ASN.1 BER encoded value.
		'''                          It is not cloned - any changes to value
		'''                          will affect the contents of the control.
		'''                          It may be null. </param>
		Public Sub New(ByVal id As String, ByVal criticality As Boolean, ByVal value As SByte())
			Me.id = id
			Me.criticality = criticality
			Me.value = value
		End Sub

		''' <summary>
		''' Retrieves the control's object identifier string.
		''' </summary>
		''' <returns> The non-null object identifier string. </returns>
		Public Overridable Property iD As String Implements Control.getID
			Get
				Return id
			End Get
		End Property

		''' <summary>
		''' Determines the control's criticality.
		''' </summary>
		''' <returns> true if the control is critical; false otherwise. </returns>
		Public Overridable Property critical As Boolean Implements Control.isCritical
			Get
				Return criticality
			End Get
		End Property

		''' <summary>
		''' Retrieves the control's ASN.1 BER encoded value.
		''' The result includes the BER tag and length for the control's value but
		''' does not include the control's object identifier and criticality setting.
		''' </summary>
		''' <returns> A possibly null byte array representing the control's
		'''          ASN.1 BER encoded value. It is not cloned - any changes to the
		'''          returned value will affect the contents of the control. </returns>
		Public Overridable Property encodedValue As SByte() Implements Control.getEncodedValue
			Get
				Return value
			End Get
		End Property
	End Class

End Namespace
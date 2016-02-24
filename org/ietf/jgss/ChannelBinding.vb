Imports System

'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.ietf.jgss


	''' <summary>
	''' This class encapsulates the concept of caller-provided channel
	''' binding information. Channel bindings are used to strengthen the
	''' quality with which peer entity authentication is provided during
	''' context establishment.  They enable the GSS-API callers to bind the
	''' establishment of the security context to relevant characteristics
	''' like addresses or to application specific data.<p>
	''' 
	''' The caller initiating the security context must determine the
	''' appropriate channel binding values to set in the GSSContext object.
	''' The acceptor must provide an identical binding in order to validate
	''' that received tokens possess correct channel-related characteristics.<p>
	''' 
	''' Use of channel bindings is optional in GSS-API. ChannelBinding can be
	''' set for the <seealso cref="GSSContext GSSContext"/> using the {@link
	''' GSSContext#setChannelBinding(ChannelBinding) setChannelBinding} method
	''' before the first call to {@link GSSContext#initSecContext(byte[], int, int)
	''' initSecContext} or {@link GSSContext#acceptSecContext(byte[], int, int)
	''' acceptSecContext} has been performed.  Unless the <code>setChannelBinding</code>
	''' method has been used to set the ChannelBinding for a GSSContext object,
	''' <code>null</code> ChannelBinding will be assumed. <p>
	''' 
	''' Conceptually, the GSS-API concatenates the initiator and acceptor
	''' address information, and the application supplied byte array to form an
	''' octet string.  The mechanism calculates a MIC over this octet string and
	''' binds the MIC to the context establishment token emitted by
	''' <code>initSecContext</code> method of the <code>GSSContext</code>
	''' interface.  The same bindings are set by the context acceptor for its
	''' <code>GSSContext</code> object and during processing of the
	''' <code>acceptSecContext</code> method a MIC is calculated in the same
	''' way. The calculated MIC is compared with that found in the token, and if
	''' the MICs differ, accept will throw a <code>GSSException</code> with the
	''' major code set to <seealso cref="GSSException#BAD_BINDINGS BAD_BINDINGS"/>, and
	''' the context will not be established. Some mechanisms may include the
	''' actual channel binding data in the token (rather than just a MIC);
	''' applications should therefore not use confidential data as
	''' channel-binding components.<p>
	''' 
	'''  Individual mechanisms may impose additional constraints on addresses
	'''  that may appear in channel bindings.  For example, a mechanism may
	'''  verify that the initiator address field of the channel binding
	'''  contains the correct network address of the host system.  Portable
	'''  applications should therefore ensure that they either provide correct
	'''  information for the address fields, or omit setting of the addressing
	'''  information.
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4
	''' </summary>
	Public Class ChannelBinding

		Private initiator As java.net.InetAddress
		Private acceptor As java.net.InetAddress
		Private appData As SByte()

		''' <summary>
		''' Create a ChannelBinding object with user supplied address information
		''' and data.  <code>null</code> values can be used for any fields which the
		''' application does not want to specify.
		''' </summary>
		''' <param name="initAddr"> the address of the context initiator.
		''' <code>null</code> value can be supplied to indicate that the
		''' application does not want to set this value. </param>
		''' <param name="acceptAddr"> the address of the context
		''' acceptor. <code>null</code> value can be supplied to indicate that
		''' the application does not want to set this value. </param>
		''' <param name="appData"> application supplied data to be used as part of the
		''' channel bindings. <code>null</code> value can be supplied to
		''' indicate that the application does not want to set this value. </param>
		Public Sub New(ByVal initAddr As java.net.InetAddress, ByVal acceptAddr As java.net.InetAddress, ByVal appData As SByte())

			initiator = initAddr
			acceptor = acceptAddr

			If appData IsNot Nothing Then
				Me.appData = New SByte(appData.Length - 1){}
				Array.Copy(appData, 0, Me.appData, 0, appData.Length)
			End If
		End Sub

		''' <summary>
		''' Creates a ChannelBinding object without any addressing information.
		''' </summary>
		''' <param name="appData"> application supplied data to be used as part of the
		''' channel bindings. </param>
		Public Sub New(ByVal appData As SByte())
			Me.New(Nothing, Nothing, appData)
		End Sub

		''' <summary>
		''' Get the initiator's address for this channel binding.
		''' </summary>
		''' <returns> the initiator's address. <code>null</code> is returned if
		''' the address has not been set. </returns>
		Public Overridable Property initiatorAddress As java.net.InetAddress
			Get
				Return initiator
			End Get
		End Property

		''' <summary>
		''' Get the acceptor's address for this channel binding.
		''' </summary>
		''' <returns> the acceptor's address. null is returned if the address has
		''' not been set. </returns>
		Public Overridable Property acceptorAddress As java.net.InetAddress
			Get
				Return acceptor
			End Get
		End Property

		''' <summary>
		''' Get the application specified data for this channel binding.
		''' </summary>
		''' <returns> the application data being used as part of the
		''' ChannelBinding. <code>null</code> is returned if no application data
		''' has been specified for the channel binding. </returns>
		Public Overridable Property applicationData As SByte()
			Get
    
				If appData Is Nothing Then Return Nothing
    
				Dim retVal As SByte() = New SByte(appData.Length - 1){}
				Array.Copy(appData, 0, retVal, 0, appData.Length)
				Return retVal
			End Get
		End Property

		''' <summary>
		''' Compares two instances of ChannelBinding.
		''' </summary>
		''' <param name="obj"> another ChannelBinding to compare this one with </param>
		''' <returns> true if the two ChannelBinding's contain
		''' the same values for the initiator and acceptor addresses and the
		''' application data. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			If Me Is obj Then Return True

			If Not(TypeOf obj Is ChannelBinding) Then Return False

			Dim cb As ChannelBinding = CType(obj, ChannelBinding)

			If (initiator IsNot Nothing AndAlso cb.initiator Is Nothing) OrElse (initiator Is Nothing AndAlso cb.initiator IsNot Nothing) Then Return False

			If initiator IsNot Nothing AndAlso (Not initiator.Equals(cb.initiator)) Then Return False

			If (acceptor IsNot Nothing AndAlso cb.acceptor Is Nothing) OrElse (acceptor Is Nothing AndAlso cb.acceptor IsNot Nothing) Then Return False

			If acceptor IsNot Nothing AndAlso (Not acceptor.Equals(cb.acceptor)) Then Return False

			Return java.util.Arrays.Equals(appData, cb.appData)
		End Function

		''' <summary>
		''' Returns a hashcode value for this ChannelBinding object.
		''' </summary>
		''' <returns> a hashCode value </returns>
		Public Overrides Function GetHashCode() As Integer
			If initiator IsNot Nothing Then
				Return initiator.GetHashCode()
			ElseIf acceptor IsNot Nothing Then
				Return acceptor.GetHashCode()
			ElseIf appData IsNot Nothing Then
				Return (New String(appData)).GetHashCode()
			Else
				Return 1
			End If
		End Function
	End Class

End Namespace
Imports System

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.remote


	''' <summary>
	''' <p>The identity of a remote client of the JMX Remote API.</p>
	''' 
	''' <p>Principals such as this <code>JMXPrincipal</code>
	''' may be associated with a particular <code>Subject</code>
	''' to augment that <code>Subject</code> with an additional
	''' identity.  Refer to the <seealso cref="javax.security.auth.Subject"/>
	''' class for more information on how to achieve this.
	''' Authorization decisions can then be based upon
	''' the Principals associated with a <code>Subject</code>.
	''' </summary>
	''' <seealso cref= java.security.Principal </seealso>
	''' <seealso cref= javax.security.auth.Subject
	''' @since 1.5 </seealso>
	<Serializable> _
	Public Class JMXPrincipal
		Implements java.security.Principal

		Private Const serialVersionUID As Long = -4184480100214577411L

		''' <summary>
		''' @serial The JMX Remote API name for the identity represented by
		''' this <code>JMXPrincipal</code> object. </summary>
		''' <seealso cref= #getName() </seealso>
		Private name As String

		''' <summary>
		''' <p>Creates a JMXPrincipal for a given identity.</p>
		''' </summary>
		''' <param name="name"> the JMX Remote API name for this identity.
		''' </param>
		''' <exception cref="NullPointerException"> if the <code>name</code> is
		''' <code>null</code>. </exception>
		Public Sub New(ByVal name As String)
			validate(name)
			Me.name = name
		End Sub

		''' <summary>
		''' Returns the name of this principal.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the name of this <code>JMXPrincipal</code>. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this <code>JMXPrincipal</code>.
		''' 
		''' <p>
		''' </summary>
		''' <returns> a string representation of this <code>JMXPrincipal</code>. </returns>
		Public Overrides Function ToString() As String
			Return ("JMXPrincipal:  " & name)
		End Function

		''' <summary>
		''' Compares the specified Object with this <code>JMXPrincipal</code>
		''' for equality.  Returns true if the given object is also a
		''' <code>JMXPrincipal</code> and the two JMXPrincipals
		''' have the same name.
		''' 
		''' <p>
		''' </summary>
		''' <param name="o"> Object to be compared for equality with this
		''' <code>JMXPrincipal</code>.
		''' </param>
		''' <returns> true if the specified Object is equal to this
		''' <code>JMXPrincipal</code>. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Nothing Then Return False

			If Me Is o Then Return True

			If Not(TypeOf o Is JMXPrincipal) Then Return False
			Dim that As JMXPrincipal = CType(o, JMXPrincipal)

			Return (Me.name.Equals(that.name))
		End Function

		''' <summary>
		''' Returns a hash code for this <code>JMXPrincipal</code>.
		''' 
		''' <p>
		''' </summary>
		''' <returns> a hash code for this <code>JMXPrincipal</code>. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return name.GetHashCode()
		End Function

		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			Dim gf As java.io.ObjectInputStream.GetField = ois.readFields()
			Dim principalName As String = CStr(gf.get("name", Nothing))
			Try
				validate(principalName)
				Me.name = principalName
			Catch e As NullPointerException
				Throw New java.io.InvalidObjectException(e.Message)
			End Try
		End Sub

		Private Shared Sub validate(ByVal name As String)
			If name Is Nothing Then Throw New NullPointerException("illegal null input")
		End Sub
	End Class

End Namespace
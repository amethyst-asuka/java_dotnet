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

Namespace java.security


	''' <summary>
	''' This interface represents the abstract notion of a principal, which
	''' can be used to represent any entity, such as an individual, a
	''' corporation, and a login id.
	''' </summary>
	''' <seealso cref= java.security.cert.X509Certificate
	''' 
	''' @author Li Gong </seealso>
	Public Interface Principal

		''' <summary>
		''' Compares this principal to the specified object.  Returns true
		''' if the object passed in matches the principal represented by
		''' the implementation of this interface.
		''' </summary>
		''' <param name="another"> principal to compare with.
		''' </param>
		''' <returns> true if the principal passed in is the same as that
		''' encapsulated by this principal, and false otherwise. </returns>
		Function Equals(  another As Object) As Boolean

		''' <summary>
		''' Returns a string representation of this principal.
		''' </summary>
		''' <returns> a string representation of this principal. </returns>
		Function ToString() As String

		''' <summary>
		''' Returns a hashcode for this principal.
		''' </summary>
		''' <returns> a hashcode for this principal. </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns the name of this principal.
		''' </summary>
		''' <returns> the name of this principal. </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Returns true if the specified subject is implied by this principal.
		''' 
		''' <p>The default implementation of this method returns true if
		''' {@code subject} is non-null and contains at least one principal that
		''' is equal to this principal.
		''' 
		''' <p>Subclasses may override this with a different implementation, if
		''' necessary.
		''' </summary>
		''' <param name="subject"> the {@code Subject} </param>
		''' <returns> true if {@code subject} is non-null and is
		'''              implied by this principal, or false otherwise.
		''' @since 1.8 </returns>
		default Function implies(  subject As javax.security.auth.Subject) As Boolean
			Sub [New](subject ==   [Nothing] As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return False;
			ReadOnly Property subject.getPrincipals() As [Return]
	End Interface

End Namespace
'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.cert


	''' <summary>
	''' An exception indicating one of a variety of problems encountered when
	''' validating a certification path.
	''' <p>
	''' A {@code CertPathValidatorException} provides support for wrapping
	''' exceptions. The <seealso cref="#getCause getCause"/> method returns the throwable,
	''' if any, that caused this exception to be thrown.
	''' <p>
	''' A {@code CertPathValidatorException} may also include the
	''' certification path that was being validated when the exception was thrown,
	''' the index of the certificate in the certification path that caused the
	''' exception to be thrown, and the reason that caused the failure. Use the
	''' <seealso cref="#getCertPath getCertPath"/>, <seealso cref="#getIndex getIndex"/>, and
	''' <seealso cref="#getReason getReason"/> methods to retrieve this information.
	''' 
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CertPathValidator
	''' 
	''' @since       1.4
	''' @author      Yassir Elley </seealso>
	Public Class CertPathValidatorException
		Inherits java.security.GeneralSecurityException

		Private Shadows Const serialVersionUID As Long = -3083180014971893139L

		''' <summary>
		''' @serial the index of the certificate in the certification path
		''' that caused the exception to be thrown
		''' </summary>
		Private index As Integer = -1

		''' <summary>
		''' @serial the {@code CertPath} that was being validated when
		''' the exception was thrown
		''' </summary>
		Private certPath As CertPath

		''' <summary>
		''' @serial the reason the validation failed
		''' </summary>
		Private reason As Reason = BasicReason.UNSPECIFIED

		''' <summary>
		''' Creates a {@code CertPathValidatorException} with
		''' no detail message.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code CertPathValidatorException} with the given
		''' detail message. A detail message is a {@code String} that
		''' describes this particular exception.
		''' </summary>
		''' <param name="msg"> the detail message </param>
		Public Sub New(ByVal msg As String)
			Me.New(msg, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code CertPathValidatorException} that wraps the
		''' specified throwable. This allows any exception to be converted into a
		''' {@code CertPathValidatorException}, while retaining information
		''' about the wrapped exception, which may be useful for debugging. The
		''' detail message is set to ({@code cause==null ? null : cause.toString()})
		''' (which typically contains the class and detail message of
		''' cause).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		''' <seealso cref="#getCause getCause()"/> method). (A {@code null} value is
		''' permitted, and indicates that the cause is nonexistent or unknown.) </param>
		Public Sub New(ByVal cause As Throwable)
			Me.New((If(cause Is Nothing, Nothing, cause.ToString())), cause)
		End Sub

		''' <summary>
		''' Creates a {@code CertPathValidatorException} with the specified
		''' detail message and cause.
		''' </summary>
		''' <param name="msg"> the detail message </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		''' <seealso cref="#getCause getCause()"/> method). (A {@code null} value is
		''' permitted, and indicates that the cause is nonexistent or unknown.) </param>
		Public Sub New(ByVal msg As String, ByVal cause As Throwable)
			Me.New(msg, cause, Nothing, -1)
		End Sub

		''' <summary>
		''' Creates a {@code CertPathValidatorException} with the specified
		''' detail message, cause, certification path, and index.
		''' </summary>
		''' <param name="msg"> the detail message (or {@code null} if none) </param>
		''' <param name="cause"> the cause (or {@code null} if none) </param>
		''' <param name="certPath"> the certification path that was in the process of
		''' being validated when the error was encountered </param>
		''' <param name="index"> the index of the certificate in the certification path
		''' that caused the error (or -1 if not applicable). Note that
		''' the list of certificates in a {@code CertPath} is zero based. </param>
		''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
		''' {@code (index < -1 || (certPath != null && index >=
		''' certPath.getCertificates().size()) } </exception>
		''' <exception cref="IllegalArgumentException"> if {@code certPath} is
		''' {@code null} and {@code index} is not -1 </exception>
		Public Sub New(ByVal msg As String, ByVal cause As Throwable, ByVal certPath As CertPath, ByVal index As Integer)
			Me.New(msg, cause, certPath, index, BasicReason.UNSPECIFIED)
		End Sub

		''' <summary>
		''' Creates a {@code CertPathValidatorException} with the specified
		''' detail message, cause, certification path, index, and reason.
		''' </summary>
		''' <param name="msg"> the detail message (or {@code null} if none) </param>
		''' <param name="cause"> the cause (or {@code null} if none) </param>
		''' <param name="certPath"> the certification path that was in the process of
		''' being validated when the error was encountered </param>
		''' <param name="index"> the index of the certificate in the certification path
		''' that caused the error (or -1 if not applicable). Note that
		''' the list of certificates in a {@code CertPath} is zero based. </param>
		''' <param name="reason"> the reason the validation failed </param>
		''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
		''' {@code (index < -1 || (certPath != null && index >=
		''' certPath.getCertificates().size()) } </exception>
		''' <exception cref="IllegalArgumentException"> if {@code certPath} is
		''' {@code null} and {@code index} is not -1 </exception>
		''' <exception cref="NullPointerException"> if {@code reason} is {@code null}
		''' 
		''' @since 1.7 </exception>
		Public Sub New(ByVal msg As String, ByVal cause As Throwable, ByVal certPath As CertPath, ByVal index As Integer, ByVal reason As Reason)
			MyBase.New(msg, cause)
			If certPath Is Nothing AndAlso index <> -1 Then Throw New IllegalArgumentException
			If index < -1 OrElse (certPath IsNot Nothing AndAlso index >= certPath.certificates.Count) Then Throw New IndexOutOfBoundsException
			If reason Is Nothing Then Throw New NullPointerException("reason can't be null")
			Me.certPath = certPath
			Me.index = index
			Me.reason = reason
		End Sub

		''' <summary>
		''' Returns the certification path that was being validated when
		''' the exception was thrown.
		''' </summary>
		''' <returns> the {@code CertPath} that was being validated when
		''' the exception was thrown (or {@code null} if not specified) </returns>
		Public Overridable Property certPath As CertPath
			Get
				Return Me.certPath
			End Get
		End Property

		''' <summary>
		''' Returns the index of the certificate in the certification path
		''' that caused the exception to be thrown. Note that the list of
		''' certificates in a {@code CertPath} is zero based. If no
		''' index has been set, -1 is returned.
		''' </summary>
		''' <returns> the index that has been set, or -1 if none has been set </returns>
		Public Overridable Property index As Integer
			Get
				Return Me.index
			End Get
		End Property

		''' <summary>
		''' Returns the reason that the validation failed. The reason is
		''' associated with the index of the certificate returned by
		''' <seealso cref="#getIndex"/>.
		''' </summary>
		''' <returns> the reason that the validation failed, or
		'''    {@code BasicReason.UNSPECIFIED} if a reason has not been
		'''    specified
		''' 
		''' @since 1.7 </returns>
		Public Overridable Property reason As Reason
			Get
				Return Me.reason
			End Get
		End Property

		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			stream.defaultReadObject()
			If reason Is Nothing Then reason = BasicReason.UNSPECIFIED
			If certPath Is Nothing AndAlso index <> -1 Then Throw New java.io.InvalidObjectException("certpath is null and index != -1")
			If index < -1 OrElse (certPath IsNot Nothing AndAlso index >= certPath.certificates.Count) Then Throw New java.io.InvalidObjectException("index out of range")
		End Sub

		''' <summary>
		''' The reason the validation algorithm failed.
		''' 
		''' @since 1.7
		''' </summary>
		Public Interface Reason
			Inherits java.io.Serializable

		End Interface


		''' <summary>
		''' The BasicReason enumerates the potential reasons that a certification
		''' path of any type may be invalid.
		''' 
		''' @since 1.7
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
		Public Enum BasicReason
			''' <summary>
			''' Unspecified reason.
			''' </summary>
			UNSPECIFIED

			''' <summary>
			''' The certificate is expired.
			''' </summary>
			EXPIRED

			''' <summary>
			''' The certificate is not yet valid.
			''' </summary>
			NOT_YET_VALID

			''' <summary>
			''' The certificate is revoked.
			''' </summary>
			REVOKED

			''' <summary>
			''' The revocation status of the certificate could not be determined.
			''' </summary>
			UNDETERMINED_REVOCATION_STATUS

			''' <summary>
			''' The signature is invalid.
			''' </summary>
			INVALID_SIGNATURE

			''' <summary>
			''' The public key or the signature algorithm has been constrained.
			''' </summary>
			ALGORITHM_CONSTRAINED
		End Enum
	End Class

End Namespace
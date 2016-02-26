Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap

	''' <summary>
	''' An exception that signals that a SOAP exception has occurred. A
	''' <code>SOAPException</code> object may contain a <code>String</code>
	''' that gives the reason for the exception, an embedded
	''' <code>Throwable</code> object, or both. This class provides methods
	''' for retrieving reason messages and for retrieving the embedded
	''' <code>Throwable</code> object.
	''' 
	''' <P> Typical reasons for throwing a <code>SOAPException</code>
	''' object are problems such as difficulty setting a header, not being
	''' able to send a message, and not being able to get a connection with
	''' the provider.  Reasons for embedding a <code>Throwable</code>
	''' object include problems such as input/output errors or a parsing
	''' problem, such as an error in parsing a header.
	''' </summary>
	Public Class SOAPException
		Inherits Exception

		Private cause As Exception

		''' <summary>
		''' Constructs a <code>SOAPException</code> object with no
		''' reason or embedded <code>Throwable</code> object.
		''' </summary>
		Public Sub New()
			MyBase.New()
			Me.cause = Nothing
		End Sub

		''' <summary>
		''' Constructs a <code>SOAPException</code> object with the given
		''' <code>String</code> as the reason for the exception being thrown.
		''' </summary>
		''' <param name="reason"> a description of what caused the exception </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
			Me.cause = Nothing
		End Sub

		''' <summary>
		''' Constructs a <code>SOAPException</code> object with the given
		''' <code>String</code> as the reason for the exception being thrown
		''' and the given <code>Throwable</code> object as an embedded
		''' exception.
		''' </summary>
		''' <param name="reason"> a description of what caused the exception </param>
		''' <param name="cause"> a <code>Throwable</code> object that is to
		'''        be embedded in this <code>SOAPException</code> object </param>
		Public Sub New(ByVal reason As String, ByVal cause As Exception)
			MyBase.New(reason)
			initCause(cause)
		End Sub

		''' <summary>
		''' Constructs a <code>SOAPException</code> object initialized
		''' with the given <code>Throwable</code> object.
		''' </summary>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause.ToString())
			initCause(cause)
		End Sub

		''' <summary>
		''' Returns the detail message for this <code>SOAPException</code>
		''' object.
		''' <P>
		''' If there is an embedded <code>Throwable</code> object, and if the
		''' <code>SOAPException</code> object has no detail message of its
		''' own, this method will return the detail message from the embedded
		''' <code>Throwable</code> object.
		''' </summary>
		''' <returns> the error or warning message for this
		'''         <code>SOAPException</code> or, if it has none, the
		'''         message of the embedded <code>Throwable</code> object,
		'''         if there is one </returns>
		Public Overridable Property message As String
			Get
				Dim ___message As String = MyBase.message
				If ___message Is Nothing AndAlso cause IsNot Nothing Then
					Return cause.Message
				Else
					Return ___message
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Throwable</code> object embedded in this
		''' <code>SOAPException</code> if there is one. Otherwise, this method
		''' returns <code>null</code>.
		''' </summary>
		''' <returns> the embedded <code>Throwable</code> object or <code>null</code>
		'''         if there is none </returns>

		Public Overridable Property cause As Exception
			Get
				Return cause
			End Get
		End Property

		''' <summary>
		''' Initializes the <code>cause</code> field of this <code>SOAPException</code>
		''' object with the given <code>Throwable</code> object.
		''' <P>
		''' This method can be called at most once.  It is generally called from
		''' within the constructor or immediately after the constructor has
		''' returned a new <code>SOAPException</code> object.
		''' If this <code>SOAPException</code> object was created with the
		''' constructor <seealso cref="#SOAPException(Throwable)"/> or
		''' <seealso cref="#SOAPException(String,Throwable)"/>, meaning that its
		''' <code>cause</code> field already has a value, this method cannot be
		''' called even once.
		''' </summary>
		''' <param name="cause"> the <code>Throwable</code> object that caused this
		'''         <code>SOAPException</code> object to be thrown.  The value of this
		'''         parameter is saved for later retrieval by the
		'''         <seealso cref="#getCause()"/> method.  A <tt>null</tt> value is
		'''         permitted and indicates that the cause is nonexistent or
		'''         unknown. </param>
		''' <returns>  a reference to this <code>SOAPException</code> instance </returns>
		''' <exception cref="IllegalArgumentException"> if <code>cause</code> is this
		'''         <code>Throwable</code> object.  (A <code>Throwable</code> object
		'''         cannot be its own cause.) </exception>
		''' <exception cref="IllegalStateException"> if the cause for this <code>SOAPException</code> object
		'''         has already been initialized </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function initCause(ByVal cause As Exception) As Exception
			If Me.cause IsNot Nothing Then Throw New IllegalStateException("Can't override cause")
			If cause Is Me Then Throw New System.ArgumentException("Self-causation not permitted")
			Me.cause = cause

			Return Me
		End Function
	End Class

End Namespace
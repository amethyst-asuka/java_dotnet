Imports System

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

Namespace org.ietf.jgss

	''' <summary>
	''' This exception is thrown whenever a GSS-API error occurs, including
	''' any mechanism specific error.  It may contain both the major and the
	''' minor GSS-API status codes.  Major error codes are those defined at the
	''' GSS-API level in this class. Minor error codes are mechanism specific
	''' error codes that can provide additional information. The underlying
	''' mechanism implementation is responsible for setting appropriate minor
	''' status codes when throwing this exception.  Aside from delivering the
	''' numeric error codes to the caller, this class performs the mapping from
	''' their numeric values to textual representations. <p>
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4
	''' </summary>
	Public Class GSSException
		Inherits Exception

		Private Const serialVersionUID As Long = -2706218945227726672L

		''' <summary>
		''' Channel bindings mismatch.
		''' </summary>
		Public Const BAD_BINDINGS As Integer = 1 'start with 1

		''' <summary>
		''' Unsupported mechanism requested.
		''' </summary>
		Public Const BAD_MECH As Integer = 2

		''' <summary>
		''' Invalid name provided.
		''' </summary>
		Public Const BAD_NAME As Integer = 3

		''' <summary>
		''' Name of unsupported type provided.
		''' </summary>
		Public Const BAD_NAMETYPE As Integer = 4

		''' <summary>
		''' Invalid status code.
		''' </summary>
	'    
	'     * This is meant to be thrown by display_status which displays
	'     * major/minor status when an incorrect status type is passed in to it!
	'     
		Public Const BAD_STATUS As Integer = 5

		''' <summary>
		''' Token had invalid integrity check.
		''' </summary>
		Public Const BAD_MIC As Integer = 6

		''' <summary>
		''' Security context expired.
		''' </summary>
		Public Const CONTEXT_EXPIRED As Integer = 7

		''' <summary>
		''' Expired credentials.
		''' </summary>
		Public Const CREDENTIALS_EXPIRED As Integer = 8

		''' <summary>
		''' Defective credentials.
		''' 
		''' </summary>
		Public Const DEFECTIVE_CREDENTIAL As Integer = 9

		''' <summary>
		''' Defective token.
		''' 
		''' </summary>
		Public Const DEFECTIVE_TOKEN As Integer = 10

		''' <summary>
		''' General failure, unspecified at GSS-API level.
		''' </summary>
		Public Const FAILURE As Integer = 11

		''' <summary>
		''' Invalid security context.
		''' </summary>
		Public Const NO_CONTEXT As Integer = 12

		''' <summary>
		''' Invalid credentials.
		''' </summary>
		Public Const NO_CRED As Integer = 13

		''' <summary>
		''' Unsupported QOP value.
		''' </summary>
		Public Const BAD_QOP As Integer = 14

		''' <summary>
		''' Operation unauthorized.
		''' </summary>
		Public Const UNAUTHORIZED As Integer = 15

		''' <summary>
		''' Operation unavailable.
		''' </summary>
		Public Const UNAVAILABLE As Integer = 16

		''' <summary>
		''' Duplicate credential element requested.
		''' </summary>
		Public Const DUPLICATE_ELEMENT As Integer = 17

		''' <summary>
		''' Name contains multi-mechanism elements.
		''' </summary>
		Public Const NAME_NOT_MN As Integer = 18

		''' <summary>
		''' The token was a duplicate of an earlier token.
		''' This is a fatal error code that may occur during
		''' context establishment.  It is not used to indicate
		''' supplementary status values. The MessageProp object is
		''' used for that purpose.
		''' </summary>
		Public Const DUPLICATE_TOKEN As Integer = 19

		''' <summary>
		''' The token's validity period has expired.  This is a
		''' fatal error code that may occur during context establishment.
		''' It is not used to indicate supplementary status values.
		''' The MessageProp object is used for that purpose.
		''' </summary>
		Public Const OLD_TOKEN As Integer = 20


		''' <summary>
		''' A later token has already been processed.  This is a
		''' fatal error code that may occur during context establishment.
		''' It is not used to indicate supplementary status values.
		''' The MessageProp object is used for that purpose.
		''' </summary>
		Public Const UNSEQ_TOKEN As Integer = 21


		''' <summary>
		''' An expected per-message token was not received.  This is a
		''' fatal error code that may occur during context establishment.
		''' It is not used to indicate supplementary status values.
		''' The MessageProp object is used for that purpose.
		''' </summary>
		Public Const GAP_TOKEN As Integer = 22


		Private Shared messages As String() = { "Channel binding mismatch", "Unsupported mechanism requested", "Invalid name provided", "Name of unsupported type provided", "Invalid input status selector", "Token had invalid integrity check", "Specified security context expired", "Expired credentials detected", "Defective credential detected", "Defective token detected", "Failure unspecified at GSS-API level", "Security context init/accept not yet called or context deleted", "No valid credentials provided", "Unsupported QOP value", "Operation unauthorized", "Operation unavailable", "Duplicate credential element requested", "Name contains multi-mechanism elements", "The token was a duplicate of an earlier token", "The token's validity period has expired", "A later token has already been processed", "An expected per-message token was not received" }

	   ''' <summary>
	   ''' The major code for this exception
	   ''' 
	   ''' @serial
	   ''' </summary>
		Private major As Integer

	   ''' <summary>
	   ''' The minor code for this exception
	   ''' 
	   ''' @serial
	   ''' </summary>
		Private minor As Integer = 0

	   ''' <summary>
	   ''' The text string for minor code
	   ''' 
	   ''' @serial
	   ''' </summary>
		Private minorMessage As String = Nothing

	   ''' <summary>
	   ''' Alternate text string for major code
	   ''' 
	   ''' @serial
	   ''' </summary>

		Private majorString As String = Nothing

		''' <summary>
		'''  Creates a GSSException object with a specified major code.
		''' </summary>
		''' <param name="majorCode"> the The GSS error code for the problem causing this
		''' exception to be thrown. </param>
		Public Sub New(ByVal majorCode As Integer)

			If validateMajor(majorCode) Then
				major = majorCode
			Else
				major = FAILURE
			End If
		End Sub

		''' <summary>
		''' Construct a GSSException object with a specified major code and a
		''' specific major string for it.
		''' </summary>
		''' <param name="majorCode"> the fatal error code causing this exception. </param>
		''' <param name="majorString"> an expicit message to be included in this exception </param>
		Friend Sub New(ByVal majorCode As Integer, ByVal majorString As String)

			If validateMajor(majorCode) Then
				major = majorCode
			Else
				major = FAILURE
			End If
			Me.majorString = majorString
		End Sub


		''' <summary>
		''' Creates a GSSException object with the specified major code, minor
		''' code, and minor code textual explanation.  This constructor is to be
		''' used when the exception is originating from the underlying mechanism
		''' level. It allows the setting of both the GSS code and the mechanism
		''' code.
		''' </summary>
		''' <param name="majorCode"> the GSS error code for the problem causing this
		''' exception to be thrown. </param>
		''' <param name="minorCode"> the mechanism level error code for the problem
		''' causing this exception to be thrown. </param>
		''' <param name="minorString"> the textual explanation of the mechanism error
		''' code. </param>
		Public Sub New(ByVal majorCode As Integer, ByVal minorCode As Integer, ByVal minorString As String)

			If validateMajor(majorCode) Then
				major = majorCode
			Else
				major = FAILURE
			End If

			minor = minorCode
			minorMessage = minorString
		End Sub

		''' <summary>
		''' Returns the GSS-API level major error code for the problem causing
		''' this exception to be thrown. Major error codes are
		''' defined at the mechanism independent GSS-API level in this
		''' class. Mechanism specific error codes that might provide more
		''' information are set as the minor error code.
		''' </summary>
		''' <returns> int the GSS-API level major error code causing this exception </returns>
		''' <seealso cref= #getMajorString </seealso>
		''' <seealso cref= #getMinor </seealso>
		''' <seealso cref= #getMinorString </seealso>
		Public Overridable Property major As Integer
			Get
				Return major
			End Get
		End Property

		''' <summary>
		''' Returns the mechanism level error code for the problem causing this
		''' exception to be thrown. The minor code is set by the underlying
		''' mechanism.
		''' </summary>
		''' <returns> int the mechanism error code; 0 indicates that it has not
		''' been set. </returns>
		''' <seealso cref= #getMinorString </seealso>
		''' <seealso cref= #setMinor </seealso>
		Public Overridable Property minor As Integer
			Get
				Return minor
			End Get
		End Property

		''' <summary>
		''' Returns a string explaining the GSS-API level major error code in
		''' this exception.
		''' </summary>
		''' <returns> String explanation string for the major error code </returns>
		''' <seealso cref= #getMajor </seealso>
		''' <seealso cref= #toString </seealso>
		Public Overridable Property majorString As String
			Get
    
				If majorString IsNot Nothing Then
					Return majorString
				Else
					Return messages(major - 1)
				End If
			End Get
		End Property


		''' <summary>
		''' Returns a string explaining the mechanism specific error code.
		''' If the minor status code is 0, then no mechanism level error details
		''' will be available.
		''' </summary>
		''' <returns> String a textual explanation of mechanism error code </returns>
		''' <seealso cref= #getMinor </seealso>
		''' <seealso cref= #getMajorString </seealso>
		''' <seealso cref= #toString </seealso>
		Public Overridable Property minorString As String
			Get
    
				Return minorMessage
			End Get
		End Property


		''' <summary>
		''' Used by the exception thrower to set the mechanism
		''' level minor error code and its string explanation.  This is used by
		''' mechanism providers to indicate error details.
		''' </summary>
		''' <param name="minorCode"> the mechanism specific error code </param>
		''' <param name="message"> textual explanation of the mechanism error code </param>
		''' <seealso cref= #getMinor </seealso>
		Public Overridable Sub setMinor(ByVal minorCode As Integer, ByVal message As String)

			minor = minorCode
			minorMessage = message
		End Sub


		''' <summary>
		''' Returns a textual representation of both the major and the minor
		''' status codes.
		''' </summary>
		''' <returns> a String with the error descriptions </returns>
		Public Overrides Function ToString() As String
			Return ("GSSException: " & message)
		End Function

		''' <summary>
		''' Returns a textual representation of both the major and the minor
		''' status codes.
		''' </summary>
		''' <returns> a String with the error descriptions </returns>
		Public Overridable Property message As String
			Get
				If minor = 0 Then Return (majorString)
    
				Return (majorString & " (Mechanism level: " & minorString & ")")
			End Get
		End Property


	'    
	'     * Validates the major code in the proper range.
	'     
		Private Function validateMajor(ByVal major As Integer) As Boolean

			If major > 0 AndAlso major <= messages.Length Then Return (True)

			Return (False)
		End Function
	End Class

End Namespace
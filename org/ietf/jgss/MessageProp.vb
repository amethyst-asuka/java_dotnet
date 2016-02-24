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
	''' This is a utility class used within the per-message GSSContext
	''' methods to convey per-message properties.<p>
	''' 
	''' When used with the GSSContext interface's wrap and getMIC methods, an
	''' instance of this class is used to indicate the desired
	''' Quality-of-Protection (QOP) and to request if confidentiality services
	''' are to be applied to caller supplied data (wrap only).  To request
	''' default QOP, the value of 0 should be used for QOP.<p>
	''' 
	''' When used with the unwrap and verifyMIC methods of the GSSContext
	''' interface, an instance of this class will be used to indicate the
	''' applied QOP and confidentiality services over the supplied message.
	''' In the case of verifyMIC, the confidentiality state will always be
	''' <code>false</code>.  Upon return from these methods, this object will also
	''' contain any supplementary status values applicable to the processed
	''' token.  The supplementary status values can indicate old tokens, out
	''' of sequence tokens, gap tokens or duplicate tokens.<p>
	''' </summary>
	''' <seealso cref= GSSContext#wrap </seealso>
	''' <seealso cref= GSSContext#unwrap </seealso>
	''' <seealso cref= GSSContext#getMIC </seealso>
	''' <seealso cref= GSSContext#verifyMIC
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4 </seealso>
	Public Class MessageProp

		Private privacyState As Boolean
		Private qop As Integer
		Private dupToken As Boolean
		Private oldToken As Boolean
		Private unseqToken As Boolean
		Private gapToken As Boolean
		Private minorStatus As Integer
		Private minorString As String

	   ''' <summary>
	   ''' Constructor which sets the desired privacy state. The QOP value used
	   ''' is 0.
	   ''' </summary>
	   ''' <param name="privState"> the privacy (i.e. confidentiality) state </param>
		Public Sub New(ByVal privState As Boolean)
			Me.New(0, privState)
		End Sub

		''' <summary>
		''' Constructor which sets the values for the qop and privacy state.
		''' </summary>
		''' <param name="qop"> the QOP value </param>
		''' <param name="privState"> the privacy (i.e. confidentiality) state </param>
		Public Sub New(ByVal qop As Integer, ByVal privState As Boolean)
			Me.qop = qop
			Me.privacyState = privState
			resetStatusValues()
		End Sub

		''' <summary>
		''' Retrieves the QOP value.
		''' </summary>
		''' <returns> an int representing the QOP value </returns>
		''' <seealso cref= #setQOP </seealso>
		Public Overridable Property qOP As Integer
			Get
				Return qop
			End Get
			Set(ByVal qop As Integer)
				Me.qop = qop
			End Set
		End Property

		''' <summary>
		''' Retrieves the privacy state.
		''' </summary>
		''' <returns> true if the privacy (i.e., confidentiality) state is true,
		''' false otherwise. </returns>
		''' <seealso cref= #setPrivacy </seealso>
		Public Overridable Property privacy As Boolean
			Get
    
				Return (privacyState)
			End Get
			Set(ByVal privState As Boolean)
    
				Me.privacyState = privState
			End Set
		End Property





		''' <summary>
		''' Tests if this is a duplicate of an earlier token.
		''' </summary>
		''' <returns> true if this is a duplicate, false otherwise. </returns>
		Public Overridable Property duplicateToken As Boolean
			Get
				Return dupToken
			End Get
		End Property

		''' <summary>
		''' Tests if this token's validity period has expired, i.e., the token
		''' is too old to be checked for duplication.
		''' </summary>
		''' <returns> true if the token's validity period has expired, false
		''' otherwise. </returns>
		Public Overridable Property oldToken As Boolean
			Get
				Return oldToken
			End Get
		End Property

		''' <summary>
		''' Tests if a later token had already been processed.
		''' </summary>
		''' <returns> true if a later token had already been processed, false otherwise. </returns>
		Public Overridable Property unseqToken As Boolean
			Get
				Return unseqToken
			End Get
		End Property

		''' <summary>
		''' Tests if an expected token was not received, i.e., one or more
		''' predecessor tokens have not yet been successfully processed.
		''' </summary>
		''' <returns> true if an expected per-message token was not received,
		''' false otherwise. </returns>
		Public Overridable Property gapToken As Boolean
			Get
				Return gapToken
			End Get
		End Property

		''' <summary>
		''' Retrieves the minor status code that the underlying mechanism might
		''' have set for this per-message operation.
		''' </summary>
		''' <returns> the int minor status </returns>
		Public Overridable Property minorStatus As Integer
			Get
				Return minorStatus
			End Get
		End Property

		''' <summary>
		''' Retrieves a string explaining the minor status code.
		''' </summary>
		''' <returns> a String corresponding to the minor status
		''' code. <code>null</code> will be returned when no minor status code
		''' has been set. </returns>
		Public Overridable Property minorString As String
			Get
				Return minorString
			End Get
		End Property

		''' <summary>
		''' This method sets the state for the supplementary information flags
		''' and the minor status in MessageProp.  It is not used by the
		''' application but by the GSS implementation to return this information
		''' to the caller of a per-message context method.
		''' </summary>
		''' <param name="duplicate"> true if the token was a duplicate of an earlier
		''' token, false otherwise </param>
		''' <param name="old"> true if the token's validity period has expired, false
		''' otherwise </param>
		''' <param name="unseq"> true if a later token has already been processed, false
		''' otherwise </param>
		''' <param name="gap"> true if one or more predecessor tokens have not yet been
		''' successfully processed, false otherwise </param>
		''' <param name="minorStatus"> the int minor status code for the per-message
		''' operation </param>
		''' <param name="minorString"> the textual representation of the minorStatus value </param>
	   Public Overridable Sub setSupplementaryStates(ByVal duplicate As Boolean, ByVal old As Boolean, ByVal unseq As Boolean, ByVal gap As Boolean, ByVal minorStatus As Integer, ByVal minorString As String)
		   Me.dupToken = duplicate
		   Me.oldToken = old
		   Me.unseqToken = unseq
		   Me.gapToken = gap
		   Me.minorStatus = minorStatus
		   Me.minorString = minorString
	   End Sub

		''' <summary>
		''' Resets the supplementary status values to false.
		''' </summary>
		Private Sub resetStatusValues()
			dupToken = False
			oldToken = False
			unseqToken = False
			gapToken = False
			minorStatus = 0
			minorString = Nothing
		End Sub
	End Class

End Namespace
'
' * Copyright (c) 1999, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface represents an unsolicited notification as defined in
	''' <A HREF="http://www.ietf.org/rfc/rfc2251.txt">RFC 2251</A>.
	''' An unsolicited notification is sent by the LDAP server to the LDAP
	''' client without any provocation from the client.
	''' Its format is that of an extended response (<tt>ExtendedResponse</tt>).
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Vincent Ryan
	''' </summary>
	''' <seealso cref= ExtendedResponse </seealso>
	''' <seealso cref= UnsolicitedNotificationEvent </seealso>
	''' <seealso cref= UnsolicitedNotificationListener
	''' @since 1.3 </seealso>

	Public Interface UnsolicitedNotification
		Inherits ExtendedResponse, HasControls

		''' <summary>
		''' Retrieves the referral(s) sent by the server.
		''' </summary>
		''' <returns> A possibly null array of referrals, each of which is represented
		''' by a URL string. If null, no referral was sent by the server. </returns>
		ReadOnly Property referrals As String()

		''' <summary>
		''' Retrieves the exception as constructed using information
		''' sent by the server. </summary>
		''' <returns> A possibly null exception as constructed using information
		''' sent by the server. If null, a "success" status was indicated by
		''' the server. </returns>
		ReadOnly Property exception As javax.naming.NamingException
	End Interface

End Namespace
'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net

	''' <summary>
	''' CookiePolicy implementations decide which cookies should be accepted
	''' and which should be rejected. Three pre-defined policy implementations
	''' are provided, namely ACCEPT_ALL, ACCEPT_NONE and ACCEPT_ORIGINAL_SERVER.
	''' 
	''' <p>See RFC 2965 sec. 3.3 and 7 for more detail.
	''' 
	''' @author Edward Wang
	''' @since 1.6
	''' </summary>
	Public Interface CookiePolicy
		''' <summary>
		''' One pre-defined policy which accepts all cookies.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final CookiePolicy ACCEPT_ALL = New CookiePolicyAnonymousInnerClassHelper();

		''' <summary>
		''' One pre-defined policy which accepts no cookies.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final CookiePolicy ACCEPT_NONE = New CookiePolicyAnonymousInnerClassHelper2();

		''' <summary>
		''' One pre-defined policy which only accepts cookies from original server.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final CookiePolicy ACCEPT_ORIGINAL_SERVER = New CookiePolicyAnonymousInnerClassHelper3();


		''' <summary>
		''' Will be called to see whether or not this cookie should be accepted.
		''' </summary>
		''' <param name="uri">       the URI to consult accept policy with </param>
		''' <param name="cookie">    the HttpCookie object in question </param>
		''' <returns>          {@code true} if this cookie should be accepted;
		'''                  otherwise, {@code false} </returns>
		Function shouldAccept(  uri As URI,   cookie As HttpCookie) As Boolean
	End Interface

End Namespace
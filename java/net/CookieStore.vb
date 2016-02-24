Imports System.Collections.Generic

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
	''' A CookieStore object represents a storage for cookie. Can store and retrieve
	''' cookies.
	''' 
	''' <p><seealso cref="CookieManager"/> will call {@code CookieStore.add} to save cookies
	''' for every incoming HTTP response, and call {@code CookieStore.get} to
	''' retrieve cookie for every outgoing HTTP request. A CookieStore
	''' is responsible for removing HttpCookie instances which have expired.
	''' 
	''' @author Edward Wang
	''' @since 1.6
	''' </summary>
	Public Interface CookieStore
		''' <summary>
		''' Adds one HTTP cookie to the store. This is called for every
		''' incoming HTTP response.
		''' 
		''' <p>A cookie to store may or may not be associated with an URI. If it
		''' is not associated with an URI, the cookie's domain and path attribute
		''' will indicate where it comes from. If it is associated with an URI and
		''' its domain and path attribute are not specified, given URI will indicate
		''' where this cookie comes from.
		''' 
		''' <p>If a cookie corresponding to the given URI already exists,
		''' then it is replaced with the new one.
		''' </summary>
		''' <param name="uri">       the uri this cookie associated with.
		'''                  if {@code null}, this cookie will not be associated
		'''                  with an URI </param>
		''' <param name="cookie">    the cookie to store
		''' </param>
		''' <exception cref="NullPointerException"> if {@code cookie} is {@code null}
		''' </exception>
		''' <seealso cref= #get
		'''  </seealso>
		Sub add(ByVal uri As URI, ByVal cookie As HttpCookie)


		''' <summary>
		''' Retrieve cookies associated with given URI, or whose domain matches the
		''' given URI. Only cookies that have not expired are returned.
		''' This is called for every outgoing HTTP request.
		''' </summary>
		''' <returns>          an immutable list of HttpCookie,
		'''                  return empty list if no cookies match the given URI
		''' </returns>
		''' <param name="uri">       the uri associated with the cookies to be returned
		''' </param>
		''' <exception cref="NullPointerException"> if {@code uri} is {@code null}
		''' </exception>
		''' <seealso cref= #add
		'''  </seealso>
		Function [get](ByVal uri As URI) As IList(Of HttpCookie)


		''' <summary>
		''' Get all not-expired cookies in cookie store.
		''' </summary>
		''' <returns>          an immutable list of http cookies;
		'''                  return empty list if there's no http cookie in store </returns>
		ReadOnly Property cookies As IList(Of HttpCookie)


		''' <summary>
		''' Get all URIs which identify the cookies in this cookie store.
		''' </summary>
		''' <returns>          an immutable list of URIs;
		'''                  return empty list if no cookie in this cookie store
		'''                  is associated with an URI </returns>
		ReadOnly Property uRIs As IList(Of URI)


		''' <summary>
		''' Remove a cookie from store.
		''' </summary>
		''' <param name="uri">       the uri this cookie associated with.
		'''                  if {@code null}, the cookie to be removed is not associated
		'''                  with an URI when added; if not {@code null}, the cookie
		'''                  to be removed is associated with the given URI when added. </param>
		''' <param name="cookie">    the cookie to remove
		''' </param>
		''' <returns>          {@code true} if this store contained the specified cookie
		''' </returns>
		''' <exception cref="NullPointerException"> if {@code cookie} is {@code null} </exception>
		Function remove(ByVal uri As URI, ByVal cookie As HttpCookie) As Boolean


		''' <summary>
		''' Remove all cookies in this cookie store.
		''' </summary>
		''' <returns>          {@code true} if this store changed as a result of the call </returns>
		Function removeAll() As Boolean
	End Interface

End Namespace
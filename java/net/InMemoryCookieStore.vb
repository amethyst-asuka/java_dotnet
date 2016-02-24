Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' A simple in-memory java.net.CookieStore implementation
	''' 
	''' @author Edward Wang
	''' @since 1.6
	''' </summary>
	Friend Class InMemoryCookieStore
		Implements java.net.CookieStore

		' the in-memory representation of cookies
		Private cookieJar As IList(Of java.net.HttpCookie) = Nothing

		' the cookies are indexed by its domain and associated uri (if present)
		' CAUTION: when a cookie removed from main data structure (i.e. cookieJar),
		'          it won't be cleared in domainIndex & uriIndex. Double-check the
		'          presence of cookie when retrieve one form index store.
		Private domainIndex As IDictionary(Of String, IList(Of java.net.HttpCookie)) = Nothing
		Private uriIndex As IDictionary(Of java.net.URI, IList(Of java.net.HttpCookie)) = Nothing

		' use ReentrantLock instead of syncronized for scalability
		Private lock As java.util.concurrent.locks.ReentrantLock = Nothing


		''' <summary>
		''' The default ctor
		''' </summary>
		Public Sub New()
			cookieJar = New List(Of java.net.HttpCookie)
			domainIndex = New Dictionary(Of String, IList(Of java.net.HttpCookie))
			uriIndex = New Dictionary(Of java.net.URI, IList(Of java.net.HttpCookie))

			lock = New java.util.concurrent.locks.ReentrantLock(False)
		End Sub

		''' <summary>
		''' Add one cookie into cookie store.
		''' </summary>
		Public Overridable Sub add(ByVal uri As java.net.URI, ByVal cookie As java.net.HttpCookie)
			' pre-condition : argument can't be null
			If cookie Is Nothing Then Throw New NullPointerException("cookie is null")


			lock.lock()
			Try
				' remove the ole cookie if there has had one
				cookieJar.Remove(cookie)

				' add new cookie if it has a non-zero max-age
				If cookie.maxAge <> 0 Then
					cookieJar.Add(cookie)
					' and add it to domain index
					If cookie.domain IsNot Nothing Then addIndex(domainIndex, cookie.domain, cookie)
					If uri IsNot Nothing Then addIndex(uriIndex, getEffectiveURI(uri), cookie)
				End If
			Finally
				lock.unlock()
			End Try
		End Sub


		''' <summary>
		''' Get all cookies, which:
		'''  1) given uri domain-matches with, or, associated with
		'''     given uri when added to the cookie store.
		'''  3) not expired.
		''' See RFC 2965 sec. 3.3.4 for more detail.
		''' </summary>
		Public Overridable Function [get](ByVal uri As java.net.URI) As IList(Of java.net.HttpCookie)
			' argument can't be null
			If uri Is Nothing Then Throw New NullPointerException("uri is null")

			Dim cookies_Renamed As IList(Of java.net.HttpCookie) = New List(Of java.net.HttpCookie)
			Dim secureLink As Boolean = "https".equalsIgnoreCase(uri.scheme)
			lock.lock()
			Try
				' check domainIndex first
				getInternal1(cookies_Renamed, domainIndex, uri.host, secureLink)
				' check uriIndex then
				getInternal2(cookies_Renamed, uriIndex, getEffectiveURI(uri), secureLink)
			Finally
				lock.unlock()
			End Try

			Return cookies_Renamed
		End Function

		''' <summary>
		''' Get all cookies in cookie store, except those have expired
		''' </summary>
		Public Overridable Property cookies As IList(Of java.net.HttpCookie)
			Get
				Dim rt As IList(Of java.net.HttpCookie)
    
				lock.lock()
				Try
					Dim it As IEnumerator(Of java.net.HttpCookie) = cookieJar.GetEnumerator()
					Do While it.MoveNext()
						If it.Current.hasExpired() Then it.remove()
					Loop
				Finally
					rt = java.util.Collections.unmodifiableList(cookieJar)
					lock.unlock()
				End Try
    
				Return rt
			End Get
		End Property

		''' <summary>
		''' Get all URIs, which are associated with at least one cookie
		''' of this cookie store.
		''' </summary>
		Public Overridable Property uRIs As IList(Of java.net.URI)
			Get
				Dim uris_Renamed As IList(Of java.net.URI) = New List(Of java.net.URI)
    
				lock.lock()
				Try
					Dim it As IEnumerator(Of java.net.URI) = uriIndex.Keys.GetEnumerator()
					Do While it.MoveNext()
						Dim uri As java.net.URI = it.Current
						Dim cookies_Renamed As IList(Of java.net.HttpCookie) = uriIndex(uri)
						If cookies_Renamed Is Nothing OrElse cookies_Renamed.Count = 0 Then it.remove()
					Loop
				Finally
					uris_Renamed.AddRange(uriIndex.Keys)
					lock.unlock()
				End Try
    
				Return uris_Renamed
			End Get
		End Property


		''' <summary>
		''' Remove a cookie from store
		''' </summary>
		Public Overridable Function remove(ByVal uri As java.net.URI, ByVal ck As java.net.HttpCookie) As Boolean
			' argument can't be null
			If ck Is Nothing Then Throw New NullPointerException("cookie is null")

			Dim modified As Boolean = False
			lock.lock()
			Try
				modified = cookieJar.Remove(ck)
			Finally
				lock.unlock()
			End Try

			Return modified
		End Function


		''' <summary>
		''' Remove all cookies in this cookie store.
		''' </summary>
		Public Overridable Function removeAll() As Boolean
			lock.lock()
			Try
				If cookieJar.Count = 0 Then Return False
				cookieJar.Clear()
				domainIndex.Clear()
				uriIndex.Clear()
			Finally
				lock.unlock()
			End Try

			Return True
		End Function


		' ---------------- Private operations -------------- 


	'    
	'     * This is almost the same as HttpCookie.domainMatches except for
	'     * one difference: It won't reject cookies when the 'H' part of the
	'     * domain contains a dot ('.').
	'     * I.E.: RFC 2965 section 3.3.2 says that if host is x.y.domain.com
	'     * and the cookie domain is .domain.com, then it should be rejected.
	'     * However that's not how the real world works. Browsers don't reject and
	'     * some sites, like yahoo.com do actually expect these cookies to be
	'     * passed along.
	'     * And should be used for 'old' style cookies (aka Netscape type of cookies)
	'     
		Private Function netscapeDomainMatches(ByVal domain As String, ByVal host As String) As Boolean
			If domain Is Nothing OrElse host Is Nothing Then Return False

			' if there's no embedded dot in domain and domain is not .local
			Dim isLocalDomain As Boolean = ".local".equalsIgnoreCase(domain)
			Dim embeddedDotInDomain As Integer = domain.IndexOf("."c)
			If embeddedDotInDomain = 0 Then embeddedDotInDomain = domain.IndexOf("."c, 1)
			If (Not isLocalDomain) AndAlso (embeddedDotInDomain = -1 OrElse embeddedDotInDomain = domain.length() - 1) Then Return False

			' if the host name contains no dot and the domain name is .local
			Dim firstDotInHost As Integer = host.IndexOf("."c)
			If firstDotInHost = -1 AndAlso isLocalDomain Then Return True

			Dim domainLength As Integer = domain.length()
			Dim lengthDiff As Integer = host.length() - domainLength
			If lengthDiff = 0 Then
				' if the host name and the domain name are just string-compare euqal
				Return host.equalsIgnoreCase(domain)
			ElseIf lengthDiff > 0 Then
				' need to check H & D component
				Dim H As String = host.Substring(0, lengthDiff)
				Dim D As String = host.Substring(lengthDiff)

				Return (D.equalsIgnoreCase(domain))
			ElseIf lengthDiff = -1 Then
				' if domain is actually .host
				Return (domain.Chars(0) = "."c AndAlso host.equalsIgnoreCase(domain.Substring(1)))
			End If

			Return False
		End Function

		Private Sub getInternal1(ByVal cookies As IList(Of java.net.HttpCookie), ByVal cookieIndex As IDictionary(Of String, IList(Of java.net.HttpCookie)), ByVal host As String, ByVal secureLink As Boolean)
			' Use a separate list to handle cookies that need to be removed so
			' that there is no conflict with iterators.
			Dim toRemove As New List(Of java.net.HttpCookie)
			For Each entry As KeyValuePair(Of String, IList(Of java.net.HttpCookie)) In cookieIndex
				Dim domain As String = entry.Key
				Dim lst As IList(Of java.net.HttpCookie) = entry.Value
				For Each c As java.net.HttpCookie In lst
					If (c.version = 0 AndAlso netscapeDomainMatches(domain, host)) OrElse (c.version = 1 AndAlso java.net.HttpCookie.domainMatches(domain, host)) Then
						If (cookieJar.IndexOf(c) <> -1) Then
							' the cookie still in main cookie store
							If Not c.hasExpired() Then
								' don't add twice and make sure it's the proper
								' security level
								If (secureLink OrElse (Not c.secure)) AndAlso (Not cookies.Contains(c)) Then cookies.Add(c)
							Else
								toRemove.Add(c)
							End If
						Else
							' the cookie has beed removed from main store,
							' so also remove it from domain indexed store
							toRemove.Add(c)
						End If
					End If
				Next c
				' Clear up the cookies that need to be removed
				For Each c As java.net.HttpCookie In toRemove
					lst.Remove(c)
					cookieJar.Remove(c)

				Next c
				toRemove.Clear()
			Next entry
		End Sub

		' @param cookies           [OUT] contains the found cookies
		' @param cookieIndex       the index
		' @param comparator        the prediction to decide whether or not
		'                          a cookie in index should be returned
		Private Sub getInternal2(Of T)(ByVal cookies As IList(Of java.net.HttpCookie), ByVal cookieIndex As IDictionary(Of T, IList(Of java.net.HttpCookie)), ByVal comparator As Comparable(Of T), ByVal secureLink As Boolean)
			For Each index As T In cookieIndex.Keys
				If comparator.CompareTo(index) = 0 Then
					Dim indexedCookies As IList(Of java.net.HttpCookie) = cookieIndex(index)
					' check the list of cookies associated with this domain
					If indexedCookies IsNot Nothing Then
						Dim it As IEnumerator(Of java.net.HttpCookie) = indexedCookies.GetEnumerator()
						Do While it.MoveNext()
							Dim ck As java.net.HttpCookie = it.Current
							If cookieJar.IndexOf(ck) <> -1 Then
								' the cookie still in main cookie store
								If Not ck.hasExpired() Then
									' don't add twice
									If (secureLink OrElse (Not ck.secure)) AndAlso (Not cookies.Contains(ck)) Then cookies.Add(ck)
								Else
									it.remove()
									cookieJar.Remove(ck)
								End If
							Else
								' the cookie has beed removed from main store,
								' so also remove it from domain indexed store
								it.remove()
							End If
						Loop
					End If ' end of indexedCookies != null
				End If ' end of comparator.compareTo(index) == 0
			Next index ' end of cookieIndex iteration
		End Sub

		' add 'cookie' indexed by 'index' into 'indexStore'
		Private Sub addIndex(Of T)(ByVal indexStore As IDictionary(Of T, IList(Of java.net.HttpCookie)), ByVal index As T, ByVal cookie As java.net.HttpCookie)
			If index IsNot Nothing Then
				Dim cookies_Renamed As IList(Of java.net.HttpCookie) = indexStore(index)
				If cookies_Renamed IsNot Nothing Then
					' there may already have the same cookie, so remove it first
					cookies_Renamed.Remove(cookie)

					cookies_Renamed.Add(cookie)
				Else
					cookies_Renamed = New List(Of java.net.HttpCookie)
					cookies_Renamed.Add(cookie)
					indexStore(index) = cookies_Renamed
				End If
			End If
		End Sub


		'
		' for cookie purpose, the effective uri should only be http://host
		' the path will be taken into account when path-match algorithm applied
		'
		Private Function getEffectiveURI(ByVal uri As java.net.URI) As java.net.URI
			Dim effectiveURI_Renamed As java.net.URI = Nothing
			Try
				effectiveURI_Renamed = New java.net.URI("http", uri.host, Nothing, Nothing, Nothing) ' fragment component -  query component -  path component
			Catch ignored As java.net.URISyntaxException
				effectiveURI_Renamed = uri
			End Try

			Return effectiveURI_Renamed
		End Function
	End Class

End Namespace
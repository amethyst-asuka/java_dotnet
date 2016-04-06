'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.rmi


	''' <summary>
	''' The <code>Naming</code> class provides methods for storing and obtaining
	''' references to remote objects in a remote object registry.  Each method of
	''' the <code>Naming</code> class takes as one of its arguments a name that
	''' is a <code>java.lang.String</code> in URL format (without the
	''' scheme component) of the form:
	''' 
	''' <PRE>
	'''    //host:port/name
	''' </PRE>
	''' 
	''' <P>where <code>host</code> is the host (remote or local) where the registry
	''' is located, <code>port</code> is the port number on which the registry
	''' accepts calls, and where <code>name</code> is a simple string uninterpreted
	''' by the registry. Both <code>host</code> and <code>port</code> are optional.
	''' If <code>host</code> is omitted, the host defaults to the local host. If
	''' <code>port</code> is omitted, then the port defaults to 1099, the
	''' "well-known" port that RMI's registry, <code>rmiregistry</code>, uses.
	''' 
	''' <P><em>Binding</em> a name for a remote object is associating or
	''' registering a name for a remote object that can be used at a later time to
	''' look up that remote object.  A remote object can be associated with a name
	''' using the <code>Naming</code> class's <code>bind</code> or
	''' <code>rebind</code> methods.
	''' 
	''' <P>Once a remote object is registered (bound) with the RMI registry on the
	''' local host, callers on a remote (or local) host can lookup the remote
	''' object by name, obtain its reference, and then invoke remote methods on the
	''' object.  A registry may be shared by all servers running on a host or an
	''' individual server process may create and use its own registry if desired
	''' (see <code>java.rmi.registry.LocateRegistry.createRegistry</code> method
	''' for details).
	''' 
	''' @author  Ann Wollrath
	''' @author  Roger Riggs
	''' @since   JDK1.1 </summary>
	''' <seealso cref=     java.rmi.registry.Registry </seealso>
	''' <seealso cref=     java.rmi.registry.LocateRegistry </seealso>
	''' <seealso cref=     java.rmi.registry.LocateRegistry#createRegistry(int) </seealso>
	Public NotInheritable Class Naming
		''' <summary>
		''' Disallow anyone from creating one of these
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Returns a reference, a stub, for the remote object associated
		''' with the specified <code>name</code>.
		''' </summary>
		''' <param name="name"> a name in URL format (without the scheme component) </param>
		''' <returns> a reference for a remote object </returns>
		''' <exception cref="NotBoundException"> if name is not currently bound </exception>
		''' <exception cref="RemoteException"> if registry could not be contacted </exception>
		''' <exception cref="AccessException"> if this operation is not permitted </exception>
		''' <exception cref="MalformedURLException"> if the name is not an appropriately
		'''  formatted URL
		''' @since JDK1.1 </exception>
		Public Shared Function lookup(  name As String) As Remote
			Dim parsed As ParsedNamingURL = parseURL(name)
			Dim registry_Renamed As Registry = getRegistry(parsed)

			If parsed.name Is Nothing Then Return registry_Renamed
			Return registry_Renamed.lookup(parsed.name)
		End Function

		''' <summary>
		''' Binds the specified <code>name</code> to a remote object.
		''' </summary>
		''' <param name="name"> a name in URL format (without the scheme component) </param>
		''' <param name="obj"> a reference for the remote object (usually a stub) </param>
		''' <exception cref="AlreadyBoundException"> if name is already bound </exception>
		''' <exception cref="MalformedURLException"> if the name is not an appropriately
		'''  formatted URL </exception>
		''' <exception cref="RemoteException"> if registry could not be contacted </exception>
		''' <exception cref="AccessException"> if this operation is not permitted (if
		''' originating from a non-local host, for example)
		''' @since JDK1.1 </exception>
		Public Shared Sub bind(  name As String,   obj As Remote)
			Dim parsed As ParsedNamingURL = parseURL(name)
			Dim registry_Renamed As Registry = getRegistry(parsed)

			If obj Is Nothing Then Throw New NullPointerException("cannot bind to null")

			registry_Renamed.bind(parsed.name, obj)
		End Sub

		''' <summary>
		''' Destroys the binding for the specified name that is associated
		''' with a remote object.
		''' </summary>
		''' <param name="name"> a name in URL format (without the scheme component) </param>
		''' <exception cref="NotBoundException"> if name is not currently bound </exception>
		''' <exception cref="MalformedURLException"> if the name is not an appropriately
		'''  formatted URL </exception>
		''' <exception cref="RemoteException"> if registry could not be contacted </exception>
		''' <exception cref="AccessException"> if this operation is not permitted (if
		''' originating from a non-local host, for example)
		''' @since JDK1.1 </exception>
		Public Shared Sub unbind(  name As String)
			Dim parsed As ParsedNamingURL = parseURL(name)
			Dim registry_Renamed As Registry = getRegistry(parsed)

			registry_Renamed.unbind(parsed.name)
		End Sub

		''' <summary>
		''' Rebinds the specified name to a new remote object. Any existing
		''' binding for the name is replaced.
		''' </summary>
		''' <param name="name"> a name in URL format (without the scheme component) </param>
		''' <param name="obj"> new remote object to associate with the name </param>
		''' <exception cref="MalformedURLException"> if the name is not an appropriately
		'''  formatted URL </exception>
		''' <exception cref="RemoteException"> if registry could not be contacted </exception>
		''' <exception cref="AccessException"> if this operation is not permitted (if
		''' originating from a non-local host, for example)
		''' @since JDK1.1 </exception>
		Public Shared Sub rebind(  name As String,   obj As Remote)
			Dim parsed As ParsedNamingURL = parseURL(name)
			Dim registry_Renamed As Registry = getRegistry(parsed)

			If obj Is Nothing Then Throw New NullPointerException("cannot bind to null")

			registry_Renamed.rebind(parsed.name, obj)
		End Sub

		''' <summary>
		''' Returns an array of the names bound in the registry.  The names are
		''' URL-formatted (without the scheme component) strings. The array contains
		''' a snapshot of the names present in the registry at the time of the
		''' call.
		''' </summary>
		''' <param name="name"> a registry name in URL format (without the scheme
		'''          component) </param>
		''' <returns>  an array of names (in the appropriate format) bound
		'''          in the registry </returns>
		''' <exception cref="MalformedURLException"> if the name is not an appropriately
		'''  formatted URL </exception>
		''' <exception cref="RemoteException"> if registry could not be contacted.
		''' @since JDK1.1 </exception>
		Public Shared Function list(  name As String) As String()
			Dim parsed As ParsedNamingURL = parseURL(name)
			Dim registry_Renamed As Registry = getRegistry(parsed)

			Dim prefix As String = ""
			If parsed.port > 0 OrElse (Not parsed.host.Equals("")) Then prefix &= "//" & parsed.host
			If parsed.port > 0 Then prefix &= ":" & parsed.port
			prefix &= "/"

			Dim names As String() = registry_Renamed.list()
			For i As Integer = 0 To names.Length - 1
				names(i) = prefix + names(i)
			Next i
			Return names
		End Function

		''' <summary>
		''' Returns a registry reference obtained from information in the URL.
		''' </summary>
		Private Shared Function getRegistry(  parsed As ParsedNamingURL) As Registry
			Return LocateRegistry.getRegistry(parsed.host, parsed.port)
		End Function

		''' <summary>
		''' Dissect Naming URL strings to obtain referenced host, port and
		''' object name.
		''' </summary>
		''' <returns> an object which contains each of the above
		''' components.
		''' </returns>
		''' <exception cref="MalformedURLException"> if given url string is malformed </exception>
		Private Shared Function parseURL(  str As String) As ParsedNamingURL
			Try
				Return intParseURL(str)
			Catch ex As java.net.URISyntaxException
	'             With RFC 3986 URI handling, 'rmi://:<port>' and
	'             * '//:<port>' forms will result in a URI syntax exception
	'             * Convert the authority to a localhost:<port> form
	'             
				Dim mue As New java.net.MalformedURLException("invalid URL String: " & str)
				mue.initCause(ex)
				Dim indexSchemeEnd As Integer = str.IndexOf(":"c)
				Dim indexAuthorityBegin As Integer = str.IndexOf("//:")
				If indexAuthorityBegin < 0 Then Throw mue
				If (indexAuthorityBegin = 0) OrElse ((indexSchemeEnd > 0) AndAlso (indexAuthorityBegin = indexSchemeEnd + 1)) Then
					Dim indexHostBegin As Integer = indexAuthorityBegin + 2
					Dim newStr As String = str.Substring(0, indexHostBegin) & "localhost" & str.Substring(indexHostBegin)
					Try
						Return intParseURL(newStr)
					Catch inte As java.net.URISyntaxException
						Throw mue
					Catch inte As java.net.MalformedURLException
						Throw inte
					End Try
				End If
				Throw mue
			End Try
		End Function

		Private Shared Function intParseURL(  str As String) As ParsedNamingURL
			Dim uri As New java.net.URI(str)
			If uri.opaque Then Throw New java.net.MalformedURLException("not a hierarchical URL: " & str)
			If uri.fragment IsNot Nothing Then
				Throw New java.net.MalformedURLException("invalid character, '#', in URL name: " & str)
			ElseIf uri.query IsNot Nothing Then
				Throw New java.net.MalformedURLException("invalid character, '?', in URL name: " & str)
			ElseIf uri.userInfo IsNot Nothing Then
				Throw New java.net.MalformedURLException("invalid character, '@', in URL host: " & str)
			End If
			Dim scheme As String = uri.scheme
			If scheme IsNot Nothing AndAlso (Not scheme.Equals("rmi")) Then Throw New java.net.MalformedURLException("invalid URL scheme: " & str)

			Dim name As String = uri.path
			If name IsNot Nothing Then
				If name.StartsWith("/") Then name = name.Substring(1)
				If name.length() = 0 Then name = Nothing
			End If

			Dim host As String = uri.host
			If host Is Nothing Then
				host = ""
				Try
	'                
	'                 * With 2396 URI handling, forms such as 'rmi://host:bar'
	'                 * or 'rmi://:<port>' are parsed into a registry based
	'                 * authority. We only want to allow server based naming
	'                 * authorities.
	'                 
					uri.parseServerAuthority()
				Catch use As java.net.URISyntaxException
					' Check if the authority is of form ':<port>'
					Dim authority As String = uri.authority
					If authority IsNot Nothing AndAlso authority.StartsWith(":") Then
						' Convert the authority to 'localhost:<port>' form
						authority = "localhost" & authority
						Try
							uri = New java.net.URI(Nothing, authority, Nothing, Nothing, Nothing)
							' Make sure it now parses to a valid server based
							' naming authority
							uri.parseServerAuthority()
						Catch use2 As java.net.URISyntaxException
							Throw New java.net.MalformedURLException("invalid authority: " & str)
						End Try
					Else
						Throw New java.net.MalformedURLException("invalid authority: " & str)
					End If
				End Try
			End If
			Dim port As Integer = uri.port
			If port = -1 Then port = Registry.REGISTRY_PORT
			Return New ParsedNamingURL(host, port, name)
		End Function

		''' <summary>
		''' Simple class to enable multiple URL return values.
		''' </summary>
		Private Class ParsedNamingURL
			Friend host As String
			Friend port As Integer
			Friend name As String

			Friend Sub New(  host As String,   port As Integer,   name As String)
				Me.host = host
				Me.port = port
				Me.name = name
			End Sub
		End Class
	End Class

End Namespace
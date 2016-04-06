Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents permission to access a resource or set of resources defined by a
	''' given url, and for a given set of user-settable request methods
	''' and request headers. The <i>name</i> of the permission is the url string.
	''' The <i>actions</i> string is a concatenation of the request methods and headers.
	''' The range of method and header names is not restricted by this class.
	''' <p><b>The url</b><p>
	''' The url string has the following expected structure.
	''' <pre>
	'''     scheme : // authority [ / path ]
	''' </pre>
	''' <i>scheme</i> will typically be http or https, but is not restricted by this
	''' class.
	''' <i>authority</i> is specified as:
	''' <pre>
	'''     authority = [ userinfo @ ] hostrange [ : portrange ]
	'''     portrange = portnumber | -portnumber | portnumber-[portnumber] | *
	'''     hostrange = ([*.] dnsname) | IPv4address | IPv6address
	''' </pre>
	''' <i>dnsname</i> is a standard DNS host or domain name, ie. one or more labels
	''' separated by ".". <i>IPv4address</i> is a standard literal IPv4 address and
	''' <i>IPv6address</i> is as defined in <a href="http://www.ietf.org/rfc/rfc2732.txt">
	''' RFC 2732</a>. Literal IPv6 addresses must however, be enclosed in '[]' characters.
	''' The <i>dnsname</i> specification can be preceded by "*." which means
	''' the name will match any hostname whose right-most domain labels are the same as
	''' this name. For example, "*.oracle.com" matches "foo.bar.oracle.com"
	''' <p>
	''' <i>portrange</i> is used to specify a port number, or a bounded or unbounded range of ports
	''' that this permission applies to. If portrange is absent or invalid, then a default
	''' port number is assumed if the scheme is {@code http} (default 80) or {@code https}
	''' (default 443). No default is assumed for other schemes. A wildcard may be specified
	''' which means all ports.
	''' <p>
	''' <i>userinfo</i> is optional. A userinfo component if present, is ignored when
	''' creating a URLPermission, and has no effect on any other methods defined by this class.
	''' <p>
	''' The <i>path</i> component comprises a sequence of path segments,
	''' separated by '/' characters. <i>path</i> may also be empty. The path is specified
	''' in a similar way to the path in <seealso cref="java.io.FilePermission"/>. There are
	''' three different ways as the following examples show:
	''' <table border>
	''' <caption>URL Examples</caption>
	''' <tr><th>Example url</th><th>Description</th></tr>
	''' <tr><td style="white-space:nowrap;">http://www.oracle.com/a/b/c.html</td>
	'''   <td>A url which identifies a specific (single) resource</td>
	''' </tr>
	''' <tr><td>http://www.oracle.com/a/b/*</td>
	'''   <td>The '*' character refers to all resources in the same "directory" - in
	'''       other words all resources with the same number of path components, and
	'''       which only differ in the final path component, represented by the '*'.
	'''   </td>
	''' </tr>
	''' <tr><td>http://www.oracle.com/a/b/-</td>
	'''   <td>The '-' character refers to all resources recursively below the
	'''       preceding path (eg. http://www.oracle.com/a/b/c/d/e.html matches this
	'''       example).
	'''   </td>
	''' </tr>
	''' </table>
	''' <p>
	''' The '*' and '-' may only be specified in the final segment of a path and must be
	''' the only character in that segment. Any query or fragment components of the
	''' url are ignored when constructing URLPermissions.
	''' <p>
	''' As a special case, urls of the form, "scheme:*" are accepted to
	''' mean any url of the given scheme.
	''' <p>
	''' The <i>scheme</i> and <i>authority</i> components of the url string are handled
	''' without regard to case. This means <seealso cref="#equals(Object)"/>,
	''' <seealso cref="#hashCode()"/> and <seealso cref="#implies(Permission)"/> are case insensitive with respect
	''' to these components. If the <i>authority</i> contains a literal IP address,
	''' then the address is normalized for comparison. The path component is case sensitive.
	''' <p><b>The actions string</b><p>
	''' The actions string of a URLPermission is a concatenation of the <i>method list</i>
	''' and the <i>request headers list</i>. These are lists of the permitted request
	''' methods and permitted request headers of the permission (respectively). The two lists
	''' are separated by a colon ':' character and elements of each list are comma separated.
	''' Some examples are:
	''' <pre>
	'''         "POST,GET,DELETE"
	'''         "GET:X-Foo-Request,X-Bar-Request"
	'''         "POST,GET:Header1,Header2"
	''' </pre>
	''' The first example specifies the methods: POST, GET and DELETE, but no request headers.
	''' The second example specifies one request method and two headers. The third
	''' example specifies two request methods, and two headers.
	''' <p>
	''' The colon separator need not be present if the request headers list is empty.
	''' No white-space is permitted in the actions string. The action strings supplied to
	''' the URLPermission constructors are case-insensitive and are normalized by converting
	''' method names to upper-case and header names to the form defines in RFC2616 (lower case
	''' with initial letter of each word capitalized). Either list can contain a wild-card '*'
	''' character which signifies all request methods or headers respectively.
	''' <p>
	''' Note. Depending on the context of use, some request methods and headers may be permitted
	''' at all times, and others may not be permitted at any time. For example, the
	''' HTTP protocol handler might disallow certain headers such as Content-Length
	''' from being set by application code, regardless of whether the security policy
	''' in force, permits it.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class URLPermission
		Inherits java.security.Permission

		Private Const serialVersionUID As Long = -2702463814894478682L

		<NonSerialized> _
		Private scheme As String
		<NonSerialized> _
		Private ssp As String ' scheme specific part
		<NonSerialized> _
		Private path As String
		<NonSerialized> _
		Private methods As IList(Of String)
		<NonSerialized> _
		Private requestHeaders As IList(Of String)
		<NonSerialized> _
		Private authority As Authority

		' serialized field
		Private actions_Renamed As String

		''' <summary>
		''' Creates a new URLPermission from a url string and which permits the given
		''' request methods and user-settable request headers.
		''' The name of the permission is the url string it was created with. Only the scheme,
		''' authority and path components of the url are used internally. Any fragment or query
		''' components are ignored. The permissions action string is as specified above.
		''' </summary>
		''' <param name="url"> the url string
		''' </param>
		''' <param name="actions"> the actions string
		''' </param>
		''' <exception cref="IllegalArgumentException"> if url is invalid or if actions contains white-space. </exception>
		Public Sub New(  url As String,   actions As String)
			MyBase.New(url)
			init(actions)
		End Sub

		Private Sub init(  actions As String)
			parseURI(name)
			Dim colon As Integer = actions.IndexOf(":"c)
			If actions.LastIndexOf(":"c) <> colon Then Throw New IllegalArgumentException("invalid actions string")

			Dim methods, headers As String
			If colon = -1 Then
				methods = actions
				headers = ""
			Else
				methods = actions.Substring(0, colon)
				headers = actions.Substring(colon+1)
			End If

			Dim l As IList(Of String) = normalizeMethods(methods)
			java.util.Collections.sort(l)
			Me.methods = java.util.Collections.unmodifiableList(l)

			l = normalizeHeaders(headers)
			java.util.Collections.sort(l)
			Me.requestHeaders = java.util.Collections.unmodifiableList(l)

			Me.actions_Renamed = actions()
		End Sub

		''' <summary>
		''' Creates a URLPermission with the given url string and unrestricted
		''' methods and request headers by invoking the two argument
		''' constructor as follows: URLPermission(url, "*:*")
		''' </summary>
		''' <param name="url"> the url string
		''' </param>
		''' <exception cref="IllegalArgumentException"> if url does not result in a valid <seealso cref="URI"/> </exception>
		Public Sub New(  url As String)
			Me.New(url, "*:*")
		End Sub

		''' <summary>
		''' Returns the normalized method list and request
		''' header list, in the form:
		''' <pre>
		'''      "method-names : header-names"
		''' </pre>
		''' <p>
		''' where method-names is the list of methods separated by commas
		''' and header-names is the list of permitted headers separated by commas.
		''' There is no white space in the returned String. If header-names is empty
		''' then the colon separator will not be present.
		''' </summary>
		Public  Overrides ReadOnly Property  actions As String
			Get
				Return actions_Renamed
			End Get
		End Property

		''' <summary>
		''' Checks if this URLPermission implies the given permission.
		''' Specifically, the following checks are done as if in the
		''' following sequence:
		''' <ul>
		''' <li>if 'p' is not an instance of URLPermission return false</li>
		''' <li>if any of p's methods are not in this's method list, and if
		'''     this's method list is not equal to "*", then return false.</li>
		''' <li>if any of p's headers are not in this's request header list, and if
		'''     this's request header list is not equal to "*", then return false.</li>
		''' <li>if this's url scheme is not equal to p's url scheme return false</li>
		''' <li>if the scheme specific part of this's url is '*' return true</li>
		''' <li>if the set of hosts defined by p's url hostrange is not a subset of
		'''     this's url hostrange then return false. For example, "*.foo.oracle.com"
		'''     is a subset of "*.oracle.com". "foo.bar.oracle.com" is not
		'''     a subset of "*.foo.oracle.com"</li>
		''' <li>if the portrange defined by p's url is not a subset of the
		'''     portrange defined by this's url then return false.
		''' <li>if the path or paths specified by p's url are contained in the
		'''     set of paths specified by this's url, then return true
		''' <li>otherwise, return false</li>
		''' </ul>
		''' <p>Some examples of how paths are matched are shown below:
		''' <table border>
		''' <caption>Examples of Path Matching</caption>
		''' <tr><th>this's path</th><th>p's path</th><th>match</th></tr>
		''' <tr><td>/a/b</td><td>/a/b</td><td>yes</td></tr>
		''' <tr><td>/a/b/*</td><td>/a/b/c</td><td>yes</td></tr>
		''' <tr><td>/a/b/*</td><td>/a/b/c/d</td><td>no</td></tr>
		''' <tr><td>/a/b/-</td><td>/a/b/c/d</td><td>yes</td></tr>
		''' <tr><td>/a/b/-</td><td>/a/b/c/d/e</td><td>yes</td></tr>
		''' <tr><td>/a/b/-</td><td>/a/b/c/*</td><td>yes</td></tr>
		''' <tr><td>/a/b/*</td><td>/a/b/c/-</td><td>no</td></tr>
		''' </table>
		''' </summary>
		Public Function implies(  p As java.security.Permission) As Boolean
			If Not(TypeOf p Is URLPermission) Then Return False

			Dim that As URLPermission = CType(p, URLPermission)

			If (Not Me.methods(0).Equals("*")) AndAlso java.util.Collections.indexOfSubList(Me.methods, that.methods) = -1 Then Return False

			If Me.requestHeaders.Count = 0 AndAlso that.requestHeaders.Count > 0 Then Return False

			If Me.requestHeaders.Count > 0 AndAlso (Not Me.requestHeaders(0).Equals("*")) AndAlso java.util.Collections.indexOfSubList(Me.requestHeaders, that.requestHeaders) = -1 Then Return False

			If Not Me.scheme.Equals(that.scheme) Then Return False

			If Me.ssp.Equals("*") Then Return True

			If Not Me.authority.implies(that.authority) Then Return False

			If Me.path Is Nothing Then Return that.path Is Nothing
			If that.path Is Nothing Then Return False

			If Me.path.EndsWith("/-") Then
				Dim thisprefix As String = Me.path.Substring(0, Me.path.length() - 1)
				Return that.path.StartsWith(thisprefix)
			End If

			If Me.path.EndsWith("/*") Then
				Dim thisprefix As String = Me.path.Substring(0, Me.path.length() - 1)
				If Not that.path.StartsWith(thisprefix) Then Return False
				Dim thatsuffix As String = that.path.Substring(thisprefix.length())
				' suffix must not contain '/' chars
				If thatsuffix.IndexOf("/"c) <> -1 Then Return False
				If thatsuffix.Equals("-") Then Return False
				Return True
			End If
			Return Me.path.Equals(that.path)
		End Function


		''' <summary>
		''' Returns true if, this.getActions().equals(p.getActions())
		''' and p's url equals this's url.  Returns false otherwise.
		''' </summary>
		Public Overrides Function Equals(  p As Object) As Boolean
			If Not(TypeOf p Is URLPermission) Then Return False
			Dim that As URLPermission = CType(p, URLPermission)
			If Not Me.scheme.Equals(that.scheme) Then Return False
			If Not Me.actions.Equals(that.actions) Then Return False
			If Not Me.authority.Equals(that.authority) Then Return False
			If Me.path IsNot Nothing Then
				Return Me.path.Equals(that.path)
			Else
				Return that.path Is Nothing
			End If
		End Function

		''' <summary>
		''' Returns a hashcode calculated from the hashcode of the
		''' actions String and the url string.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return actions.GetHashCode() + scheme.GetHashCode() + authority.GetHashCode() + (If(path Is Nothing, 0, path.GetHashCode()))
		End Function


		Private Function normalizeMethods(  methods As String) As IList(Of String)
			Dim l As IList(Of String) = New List(Of String)
			Dim b As New StringBuilder
			For i As Integer = 0 To methods.length() - 1
				Dim c As Char = methods.Chars(i)
				If c = ","c Then
					Dim s As String = b.ToString()
					If s.length() > 0 Then l.Add(s)
					b = New StringBuilder
				ElseIf c = " "c OrElse c = ControlChars.Tab Then
					Throw New IllegalArgumentException("white space not allowed")
				Else
					If c >= "a"c AndAlso c <= "z"c Then AscW(c) += AscW("A"c) - AscW("a"c)
					b.append(c)
				End If
			Next i
			Dim s As String = b.ToString()
			If s.length() > 0 Then l.Add(s)
			Return l
		End Function

		Private Function normalizeHeaders(  headers As String) As IList(Of String)
			Dim l As IList(Of String) = New List(Of String)
			Dim b As New StringBuilder
			Dim capitalizeNext As Boolean = True
			For i As Integer = 0 To headers.length() - 1
				Dim c As Char = headers.Chars(i)
				If c >= "a"c AndAlso c <= "z"c Then
					If capitalizeNext Then
						AscW(c) += AscW("A"c) - AscW("a"c)
						capitalizeNext = False
					End If
					b.append(c)
				ElseIf c = " "c OrElse c = ControlChars.Tab Then
					Throw New IllegalArgumentException("white space not allowed")
				ElseIf c = "-"c Then
						capitalizeNext = True
					b.append(c)
				ElseIf c = ","c Then
					Dim s As String = b.ToString()
					If s.length() > 0 Then l.Add(s)
					b = New StringBuilder
					capitalizeNext = True
				Else
					capitalizeNext = False
					b.append(c)
				End If
			Next i
			Dim s As String = b.ToString()
			If s.length() > 0 Then l.Add(s)
			Return l
		End Function

		Private Sub parseURI(  url As String)
			Dim len As Integer = url.length()
			Dim delim As Integer = url.IndexOf(":"c)
			If delim = -1 OrElse delim + 1 = len Then Throw New IllegalArgumentException("invalid URL string")
			scheme = url.Substring(0, delim).ToLower()
			Me.ssp = url.Substring(delim + 1)

			If Not ssp.StartsWith("//") Then
				If Not ssp.Equals("*") Then Throw New IllegalArgumentException("invalid URL string")
				Me.authority = New Authority(scheme, "*")
				Return
			End If
			Dim authpath As String = ssp.Substring(2)

			delim = authpath.IndexOf("/"c)
			Dim auth As String
			If delim = -1 Then
				Me.path = ""
				auth = authpath
			Else
				auth = authpath.Substring(0, delim)
				Me.path = authpath.Substring(delim)
			End If
			Me.authority = New Authority(scheme, auth.ToLower())
		End Sub

		Private Function actions() As String
			Dim b As New StringBuilder
			For Each s As String In methods
				b.append(s)
			Next s
			b.append(":")
			For Each s As String In requestHeaders
				b.append(s)
			Next s
			Return b.ToString()
		End Function

		''' <summary>
		''' restore the state of this object from stream
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Dim fields As java.io.ObjectInputStream.GetField = s.readFields()
			Dim actions_Renamed As String = CStr(fields.get("actions", Nothing))

			init(actions_Renamed)
		End Sub

		Friend Class Authority
			Friend p As HostPortrange

			Friend Sub New(  scheme As String,   authority As String)
				Dim at As Integer = authority.IndexOf("@"c)
				If at = -1 Then
						p = New HostPortrange(scheme, authority)
				Else
						p = New HostPortrange(scheme, authority.Substring(at+1))
				End If
			End Sub

			Friend Overridable Function implies(  other As Authority) As Boolean
				Return impliesHostrange(other) AndAlso impliesPortrange(other)
			End Function

			Private Function impliesHostrange(  that As Authority) As Boolean
				Dim thishost As String = Me.p.hostname()
				Dim thathost As String = that.p.hostname()

				If p.wildcard() AndAlso thishost.Equals("") Then Return True
				If that.p.wildcard() AndAlso thathost.Equals("") Then Return False
				If thishost.Equals(thathost) Then Return True
				If Me.p.wildcard() Then Return thathost.EndsWith(thishost)
				Return False
			End Function

			Private Function impliesPortrange(  that As Authority) As Boolean
				Dim thisrange As Integer() = Me.p.portrange()
				Dim thatrange As Integer() = that.p.portrange()
				If thisrange(0) = -1 Then Return True
				Return thisrange(0) <= thatrange(0) AndAlso thisrange(1) >= thatrange(1)
			End Function

			Friend Overrides Function Equals(  that As Authority) As Boolean
				Return Me.p.Equals(that.p)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return p.GetHashCode()
			End Function
		End Class
	End Class

End Namespace
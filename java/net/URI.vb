Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics

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

Namespace java.net




	''' <summary>
	''' Represents a Uniform Resource Identifier (URI) reference.
	''' 
	''' <p> Aside from some minor deviations noted below, an instance of this
	''' class represents a URI reference as defined by
	''' <a href="http://www.ietf.org/rfc/rfc2396.txt"><i>RFC&nbsp;2396: Uniform
	''' Resource Identifiers (URI): Generic Syntax</i></a>, amended by <a
	''' href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format for
	''' Literal IPv6 Addresses in URLs</i></a>. The Literal IPv6 address format
	''' also supports scope_ids. The syntax and usage of scope_ids is described
	''' <a href="Inet6Address.html#scoped">here</a>.
	''' This class provides constructors for creating URI instances from
	''' their components or by parsing their string forms, methods for accessing the
	''' various components of an instance, and methods for normalizing, resolving,
	''' and relativizing URI instances.  Instances of this class are immutable.
	''' 
	''' 
	''' <h3> URI syntax and components </h3>
	''' 
	''' At the highest level a URI reference (hereinafter simply "URI") in string
	''' form has the syntax
	''' 
	''' <blockquote>
	''' [<i>scheme</i><b>{@code :}</b>]<i>scheme-specific-part</i>[<b>{@code #}</b><i>fragment</i>]
	''' </blockquote>
	''' 
	''' where square brackets [...] delineate optional components and the characters
	''' <b>{@code :}</b> and <b>{@code #}</b> stand for themselves.
	''' 
	''' <p> An <i>absolute</i> URI specifies a scheme; a URI that is not absolute is
	''' said to be <i>relative</i>.  URIs are also classified according to whether
	''' they are <i>opaque</i> or <i>hierarchical</i>.
	''' 
	''' <p> An <i>opaque</i> URI is an absolute URI whose scheme-specific part does
	''' not begin with a slash character ({@code '/'}).  Opaque URIs are not
	''' subject to further parsing.  Some examples of opaque URIs are:
	''' 
	''' <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	''' <tr><td>{@code mailto:java-net@java.sun.com}<td></tr>
	''' <tr><td>{@code news:comp.lang.java}<td></tr>
	''' <tr><td>{@code urn:isbn:096139210x}</td></tr>
	''' </table></blockquote>
	''' 
	''' <p> A <i>hierarchical</i> URI is either an absolute URI whose
	''' scheme-specific part begins with a slash character, or a relative URI, that
	''' is, a URI that does not specify a scheme.  Some examples of hierarchical
	''' URIs are:
	''' 
	''' <blockquote>
	''' {@code http://java.sun.com/j2se/1.3/}<br>
	''' {@code docs/guide/collections/designfaq.html#28}<br>
	''' {@code ../../../demo/jfc/SwingSet2/src/SwingSet2.java}<br>
	''' {@code file:///~/calendar}
	''' </blockquote>
	''' 
	''' <p> A hierarchical URI is subject to further parsing according to the syntax
	''' 
	''' <blockquote>
	''' [<i>scheme</i><b>{@code :}</b>][<b>{@code //}</b><i>authority</i>][<i>path</i>][<b>{@code ?}</b><i>query</i>][<b>{@code #}</b><i>fragment</i>]
	''' </blockquote>
	''' 
	''' where the characters <b>{@code :}</b>, <b>{@code /}</b>,
	''' <b>{@code ?}</b>, and <b>{@code #}</b> stand for themselves.  The
	''' scheme-specific part of a hierarchical URI consists of the characters
	''' between the scheme and fragment components.
	''' 
	''' <p> The authority component of a hierarchical URI is, if specified, either
	''' <i>server-based</i> or <i>registry-based</i>.  A server-based authority
	''' parses according to the familiar syntax
	''' 
	''' <blockquote>
	''' [<i>user-info</i><b>{@code @}</b>]<i>host</i>[<b>{@code :}</b><i>port</i>]
	''' </blockquote>
	''' 
	''' where the characters <b>{@code @}</b> and <b>{@code :}</b> stand for
	''' themselves.  Nearly all URI schemes currently in use are server-based.  An
	''' authority component that does not parse in this way is considered to be
	''' registry-based.
	''' 
	''' <p> The path component of a hierarchical URI is itself said to be absolute
	''' if it begins with a slash character ({@code '/'}); otherwise it is
	''' relative.  The path of a hierarchical URI that is either absolute or
	''' specifies an authority is always absolute.
	''' 
	''' <p> All told, then, a URI instance has the following nine components:
	''' 
	''' <blockquote><table summary="Describes the components of a URI:scheme,scheme-specific-part,authority,user-info,host,port,path,query,fragment">
	''' <tr><th><i>Component</i></th><th><i>Type</i></th></tr>
	''' <tr><td>scheme</td><td>{@code String}</td></tr>
	''' <tr><td>scheme-specific-part&nbsp;&nbsp;&nbsp;&nbsp;</td><td>{@code String}</td></tr>
	''' <tr><td>authority</td><td>{@code String}</td></tr>
	''' <tr><td>user-info</td><td>{@code String}</td></tr>
	''' <tr><td>host</td><td>{@code String}</td></tr>
	''' <tr><td>port</td><td>{@code int}</td></tr>
	''' <tr><td>path</td><td>{@code String}</td></tr>
	''' <tr><td>query</td><td>{@code String}</td></tr>
	''' <tr><td>fragment</td><td>{@code String}</td></tr>
	''' </table></blockquote>
	''' 
	''' In a given instance any particular component is either <i>undefined</i> or
	''' <i>defined</i> with a distinct value.  Undefined string components are
	''' represented by {@code null}, while undefined integer components are
	''' represented by {@code -1}.  A string component may be defined to have the
	''' empty string as its value; this is not equivalent to that component being
	''' undefined.
	''' 
	''' <p> Whether a particular component is or is not defined in an instance
	''' depends upon the type of the URI being represented.  An absolute URI has a
	''' scheme component.  An opaque URI has a scheme, a scheme-specific part, and
	''' possibly a fragment, but has no other components.  A hierarchical URI always
	''' has a path (though it may be empty) and a scheme-specific-part (which at
	''' least contains the path), and may have any of the other components.  If the
	''' authority component is present and is server-based then the host component
	''' will be defined and the user-information and port components may be defined.
	''' 
	''' 
	''' <h4> Operations on URI instances </h4>
	''' 
	''' The key operations supported by this class are those of
	''' <i>normalization</i>, <i>resolution</i>, and <i>relativization</i>.
	''' 
	''' <p> <i>Normalization</i> is the process of removing unnecessary {@code "."}
	''' and {@code ".."} segments from the path component of a hierarchical URI.
	''' Each {@code "."} segment is simply removed.  A {@code ".."} segment is
	''' removed only if it is preceded by a non-{@code ".."} segment.
	''' Normalization has no effect upon opaque URIs.
	''' 
	''' <p> <i>Resolution</i> is the process of resolving one URI against another,
	''' <i>base</i> URI.  The resulting URI is constructed from components of both
	''' URIs in the manner specified by RFC&nbsp;2396, taking components from the
	''' base URI for those not specified in the original.  For hierarchical URIs,
	''' the path of the original is resolved against the path of the base and then
	''' normalized.  The result, for example, of resolving
	''' 
	''' <blockquote>
	''' {@code docs/guide/collections/designfaq.html#28}
	''' &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	''' &nbsp;&nbsp;&nbsp;&nbsp;(1)
	''' </blockquote>
	''' 
	''' against the base URI {@code http://java.sun.com/j2se/1.3/} is the result
	''' URI
	''' 
	''' <blockquote>
	''' {@code https://docs.oracle.com/javase/1.3/docs/guide/collections/designfaq.html#28}
	''' </blockquote>
	''' 
	''' Resolving the relative URI
	''' 
	''' <blockquote>
	''' {@code ../../../demo/jfc/SwingSet2/src/SwingSet2.java}&nbsp;&nbsp;&nbsp;&nbsp;(2)
	''' </blockquote>
	''' 
	''' against this result yields, in turn,
	''' 
	''' <blockquote>
	''' {@code http://java.sun.com/j2se/1.3/demo/jfc/SwingSet2/src/SwingSet2.java}
	''' </blockquote>
	''' 
	''' Resolution of both absolute and relative URIs, and of both absolute and
	''' relative paths in the case of hierarchical URIs, is supported.  Resolving
	''' the URI {@code file:///~calendar} against any other URI simply yields the
	''' original URI, since it is absolute.  Resolving the relative URI (2) above
	''' against the relative base URI (1) yields the normalized, but still relative,
	''' URI
	''' 
	''' <blockquote>
	''' {@code demo/jfc/SwingSet2/src/SwingSet2.java}
	''' </blockquote>
	''' 
	''' <p> <i>Relativization</i>, finally, is the inverse of resolution: For any
	''' two normalized URIs <i>u</i> and&nbsp;<i>v</i>,
	''' 
	''' <blockquote>
	'''   <i>u</i>{@code .relativize(}<i>u</i>{@code .resolve(}<i>v</i>{@code )).equals(}<i>v</i>{@code )}&nbsp;&nbsp;and<br>
	'''   <i>u</i>{@code .resolve(}<i>u</i>{@code .relativize(}<i>v</i>{@code )).equals(}<i>v</i>{@code )}&nbsp;&nbsp;.<br>
	''' </blockquote>
	''' 
	''' This operation is often useful when constructing a document containing URIs
	''' that must be made relative to the base URI of the document wherever
	''' possible.  For example, relativizing the URI
	''' 
	''' <blockquote>
	''' {@code https://docs.oracle.com/javase/1.3/docs/guide/index.html}
	''' </blockquote>
	''' 
	''' against the base URI
	''' 
	''' <blockquote>
	''' {@code http://java.sun.com/j2se/1.3}
	''' </blockquote>
	''' 
	''' yields the relative URI {@code docs/guide/index.html}.
	''' 
	''' 
	''' <h4> Character categories </h4>
	''' 
	''' RFC&nbsp;2396 specifies precisely which characters are permitted in the
	''' various components of a URI reference.  The following categories, most of
	''' which are taken from that specification, are used below to describe these
	''' constraints:
	''' 
	''' <blockquote><table cellspacing=2 summary="Describes categories alpha,digit,alphanum,unreserved,punct,reserved,escaped,and other">
	'''   <tr><th valign=top><i>alpha</i></th>
	'''       <td>The US-ASCII alphabetic characters,
	'''        {@code 'A'}&nbsp;through&nbsp;{@code 'Z'}
	'''        and {@code 'a'}&nbsp;through&nbsp;{@code 'z'}</td></tr>
	'''   <tr><th valign=top><i>digit</i></th>
	'''       <td>The US-ASCII decimal digit characters,
	'''       {@code '0'}&nbsp;through&nbsp;{@code '9'}</td></tr>
	'''   <tr><th valign=top><i>alphanum</i></th>
	'''       <td>All <i>alpha</i> and <i>digit</i> characters</td></tr>
	'''   <tr><th valign=top><i>unreserved</i>&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''       <td>All <i>alphanum</i> characters together with those in the string
	'''        {@code "_-!.~'()*"}</td></tr>
	'''   <tr><th valign=top><i>punct</i></th>
	'''       <td>The characters in the string {@code ",;:$&+="}</td></tr>
	'''   <tr><th valign=top><i>reserved</i></th>
	'''       <td>All <i>punct</i> characters together with those in the string
	'''        {@code "?/[]@"}</td></tr>
	'''   <tr><th valign=top><i>escaped</i></th>
	'''       <td>Escaped octets, that is, triplets consisting of the percent
	'''           character ({@code '%'}) followed by two hexadecimal digits
	'''           ({@code '0'}-{@code '9'}, {@code 'A'}-{@code 'F'}, and
	'''           {@code 'a'}-{@code 'f'})</td></tr>
	'''   <tr><th valign=top><i>other</i></th>
	'''       <td>The Unicode characters that are not in the US-ASCII character set,
	'''           are not control characters (according to the {@link
	'''           java.lang.Character#isISOControl(char) Character.isISOControl}
	'''           method), and are not space characters (according to the {@link
	'''           java.lang.Character#isSpaceChar(char) Character.isSpaceChar}
	'''           method)&nbsp;&nbsp;<i>(<b>Deviation from RFC 2396</b>, which is
	'''           limited to US-ASCII)</i></td></tr>
	''' </table></blockquote>
	''' 
	''' <p><a name="legal-chars"></a> The set of all legal URI characters consists of
	''' the <i>unreserved</i>, <i>reserved</i>, <i>escaped</i>, and <i>other</i>
	''' characters.
	''' 
	''' 
	''' <h4> Escaped octets, quotation, encoding, and decoding </h4>
	''' 
	''' RFC 2396 allows escaped octets to appear in the user-info, path, query, and
	''' fragment components.  Escaping serves two purposes in URIs:
	''' 
	''' <ul>
	''' 
	'''   <li><p> To <i>encode</i> non-US-ASCII characters when a URI is required to
	'''   conform strictly to RFC&nbsp;2396 by not containing any <i>other</i>
	'''   characters.  </p></li>
	''' 
	'''   <li><p> To <i>quote</i> characters that are otherwise illegal in a
	'''   component.  The user-info, path, query, and fragment components differ
	'''   slightly in terms of which characters are considered legal and illegal.
	'''   </p></li>
	''' 
	''' </ul>
	''' 
	''' These purposes are served in this class by three related operations:
	''' 
	''' <ul>
	''' 
	'''   <li><p><a name="encode"></a> A character is <i>encoded</i> by replacing it
	'''   with the sequence of escaped octets that represent that character in the
	'''   UTF-8 character set.  The Euro currency symbol ({@code '\u005Cu20AC'}),
	'''   for example, is encoded as {@code "%E2%82%AC"}.  <i>(<b>Deviation from
	'''   RFC&nbsp;2396</b>, which does not specify any particular character
	'''   set.)</i> </p></li>
	''' 
	'''   <li><p><a name="quote"></a> An illegal character is <i>quoted</i> simply by
	'''   encoding it.  The space character, for example, is quoted by replacing it
	'''   with {@code "%20"}.  UTF-8 contains US-ASCII, hence for US-ASCII
	'''   characters this transformation has exactly the effect required by
	'''   RFC&nbsp;2396. </p></li>
	''' 
	'''   <li><p><a name="decode"></a>
	'''   A sequence of escaped octets is <i>decoded</i> by
	'''   replacing it with the sequence of characters that it represents in the
	'''   UTF-8 character set.  UTF-8 contains US-ASCII, hence decoding has the
	'''   effect of de-quoting any quoted US-ASCII characters as well as that of
	'''   decoding any encoded non-US-ASCII characters.  If a <a
	'''   href="../nio/charset/CharsetDecoder.html#ce">decoding error</a> occurs
	'''   when decoding the escaped octets then the erroneous octets are replaced by
	'''   {@code '\u005CuFFFD'}, the Unicode replacement character.  </p></li>
	''' 
	''' </ul>
	''' 
	''' These operations are exposed in the constructors and methods of this class
	''' as follows:
	''' 
	''' <ul>
	''' 
	'''   <li><p> The {@link #URI(java.lang.String) single-argument
	'''   constructor} requires any illegal characters in its argument to be
	'''   quoted and preserves any escaped octets and <i>other</i> characters that
	'''   are present.  </p></li>
	''' 
	'''   <li><p> The {@linkplain
	'''   #URI(java.lang.String,java.lang.String,java.lang.String,int,java.lang.String,java.lang.String,java.lang.String)
	'''   multi-argument constructors} quote illegal characters as
	'''   required by the components in which they appear.  The percent character
	'''   ({@code '%'}) is always quoted by these constructors.  Any <i>other</i>
	'''   characters are preserved.  </p></li>
	''' 
	'''   <li><p> The <seealso cref="#getRawUserInfo() getRawUserInfo"/>, {@link #getRawPath()
	'''   getRawPath}, <seealso cref="#getRawQuery() getRawQuery"/>, {@link #getRawFragment()
	'''   getRawFragment}, <seealso cref="#getRawAuthority() getRawAuthority"/>, and {@link
	'''   #getRawSchemeSpecificPart() getRawSchemeSpecificPart} methods return the
	'''   values of their corresponding components in raw form, without interpreting
	'''   any escaped octets.  The strings returned by these methods may contain
	'''   both escaped octets and <i>other</i> characters, and will not contain any
	'''   illegal characters.  </p></li>
	''' 
	'''   <li><p> The <seealso cref="#getUserInfo() getUserInfo"/>, {@link #getPath()
	'''   getPath}, <seealso cref="#getQuery() getQuery"/>, {@link #getFragment()
	'''   getFragment}, <seealso cref="#getAuthority() getAuthority"/>, and {@link
	'''   #getSchemeSpecificPart() getSchemeSpecificPart} methods decode any escaped
	'''   octets in their corresponding components.  The strings returned by these
	'''   methods may contain both <i>other</i> characters and illegal characters,
	'''   and will not contain any escaped octets.  </p></li>
	''' 
	'''   <li><p> The <seealso cref="#toString() toString"/> method returns a URI string with
	'''   all necessary quotation but which may contain <i>other</i> characters.
	'''   </p></li>
	''' 
	'''   <li><p> The <seealso cref="#toASCIIString() toASCIIString"/> method returns a fully
	'''   quoted and encoded URI string that does not contain any <i>other</i>
	'''   characters.  </p></li>
	''' 
	''' </ul>
	''' 
	''' 
	''' <h4> Identities </h4>
	''' 
	''' For any URI <i>u</i>, it is always the case that
	''' 
	''' <blockquote>
	''' {@code new URI(}<i>u</i>{@code .toString()).equals(}<i>u</i>{@code )}&nbsp;.
	''' </blockquote>
	''' 
	''' For any URI <i>u</i> that does not contain redundant syntax such as two
	''' slashes before an empty authority (as in {@code file:///tmp/}&nbsp;) or a
	''' colon following a host name but no port (as in
	''' {@code http://java.sun.com:}&nbsp;), and that does not encode characters
	''' except those that must be quoted, the following identities also hold:
	''' <pre>
	'''     new URI(<i>u</i>.getScheme(),
	'''             <i>u</i>.getSchemeSpecificPart(),
	'''             <i>u</i>.getFragment())
	'''     .equals(<i>u</i>)</pre>
	''' in all cases,
	''' <pre>
	'''     new URI(<i>u</i>.getScheme(),
	'''             <i>u</i>.getUserInfo(), <i>u</i>.getAuthority(),
	'''             <i>u</i>.getPath(), <i>u</i>.getQuery(),
	'''             <i>u</i>.getFragment())
	'''     .equals(<i>u</i>)</pre>
	''' if <i>u</i> is hierarchical, and
	''' <pre>
	'''     new URI(<i>u</i>.getScheme(),
	'''             <i>u</i>.getUserInfo(), <i>u</i>.getHost(), <i>u</i>.getPort(),
	'''             <i>u</i>.getPath(), <i>u</i>.getQuery(),
	'''             <i>u</i>.getFragment())
	'''     .equals(<i>u</i>)</pre>
	''' if <i>u</i> is hierarchical and has either no authority or a server-based
	''' authority.
	''' 
	''' 
	''' <h4> URIs, URLs, and URNs </h4>
	''' 
	''' A URI is a uniform resource <i>identifier</i> while a URL is a uniform
	''' resource <i>locator</i>.  Hence every URL is a URI, abstractly speaking, but
	''' not every URI is a URL.  This is because there is another subcategory of
	''' URIs, uniform resource <i>names</i> (URNs), which name resources but do not
	''' specify how to locate them.  The {@code mailto}, {@code news}, and
	''' {@code isbn} URIs shown above are examples of URNs.
	''' 
	''' <p> The conceptual distinction between URIs and URLs is reflected in the
	''' differences between this class and the <seealso cref="URL"/> class.
	''' 
	''' <p> An instance of this class represents a URI reference in the syntactic
	''' sense defined by RFC&nbsp;2396.  A URI may be either absolute or relative.
	''' A URI string is parsed according to the generic syntax without regard to the
	''' scheme, if any, that it specifies.  No lookup of the host, if any, is
	''' performed, and no scheme-dependent stream handler is constructed.  Equality,
	''' hashing, and comparison are defined strictly in terms of the character
	''' content of the instance.  In other words, a URI instance is little more than
	''' a structured string that supports the syntactic, scheme-independent
	''' operations of comparison, normalization, resolution, and relativization.
	''' 
	''' <p> An instance of the <seealso cref="URL"/> [Class], by contrast, represents the
	''' syntactic components of a URL together with some of the information required
	''' to access the resource that it describes.  A URL must be absolute, that is,
	''' it must always specify a scheme.  A URL string is parsed according to its
	''' scheme.  A stream handler is always established for a URL, and in fact it is
	''' impossible to create a URL instance for a scheme for which no handler is
	''' available.  Equality and hashing depend upon both the scheme and the
	''' Internet address of the host, if any; comparison is not defined.  In other
	''' words, a URL is a structured string that supports the syntactic operation of
	''' resolution as well as the network I/O operations of looking up the host and
	''' opening a connection to the specified resource.
	''' 
	''' 
	''' @author Mark Reinhold
	''' @since 1.4
	''' </summary>
	''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc2279.txt"><i>RFC&nbsp;2279: UTF-8, a
	''' transformation format of ISO 10646</i></a>, <br><a
	''' href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IPv6 Addressing
	''' Architecture</i></a>, <br><a
	''' href="http://www.ietf.org/rfc/rfc2396.txt"><i>RFC&nbsp;2396: Uniform
	''' Resource Identifiers (URI): Generic Syntax</i></a>, <br><a
	''' href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format for
	''' Literal IPv6 Addresses in URLs</i></a>, <br><a
	''' href="URISyntaxException.html">URISyntaxException</a> </seealso>

	<Serializable> _
	Public NotInheritable Class URI
		Implements Comparable(Of URI)

		' Note: Comments containing the word "ASSERT" indicate places where a
		' throw of an InternalError should be replaced by an appropriate assertion
		' statement once asserts are enabled in the build.

		Friend Const serialVersionUID As Long = -6052424284110960213L


		' -- Properties and components of this instance --

		' Components of all URIs: [<scheme>:]<scheme-specific-part>[#<fragment>]
		<NonSerialized> _
		Private scheme As String ' null ==> relative URI
		<NonSerialized> _
		Private fragment As String

		' Hierarchical URI components: [//<authority>]<path>[?<query>]
		<NonSerialized> _
		Private authority As String ' Registry or server

		' Server-based authority: [<userInfo>@]<host>[:<port>]
		<NonSerialized> _
		Private userInfo As String
		<NonSerialized> _
		Private host As String ' null ==> registry-based
		<NonSerialized> _
		Private port As Integer = -1 ' -1 ==> undefined

		' Remaining components of hierarchical URIs
		<NonSerialized> _
		Private path As String ' null ==> opaque
		<NonSerialized> _
		Private query As String

		' The remaining fields may be computed on demand

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private schemeSpecificPart As String
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private hash_Renamed As Integer ' Zero ==> undefined

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private decodedUserInfo As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private decodedAuthority As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private decodedPath As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private decodedQuery As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private decodedFragment As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private decodedSchemeSpecificPart As String = Nothing

		''' <summary>
		''' The string form of this URI.
		''' 
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private string_Renamed As String ' The only serializable field



		' -- Constructors and factories --

		Private Sub New() ' Used internally
		End Sub

		''' <summary>
		''' Constructs a URI by parsing the given string.
		''' 
		''' <p> This constructor parses the given string exactly as specified by the
		''' grammar in <a
		''' href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		''' Appendix&nbsp;A, <b><i>except for the following deviations:</i></b> </p>
		''' 
		''' <ul>
		''' 
		'''   <li><p> An empty authority component is permitted as long as it is
		'''   followed by a non-empty path, a query component, or a fragment
		'''   component.  This allows the parsing of URIs such as
		'''   {@code "file:///foo/bar"}, which seems to be the intent of
		'''   RFC&nbsp;2396 although the grammar does not permit it.  If the
		'''   authority component is empty then the user-information, host, and port
		'''   components are undefined. </p></li>
		''' 
		'''   <li><p> Empty relative paths are permitted; this seems to be the
		'''   intent of RFC&nbsp;2396 although the grammar does not permit it.  The
		'''   primary consequence of this deviation is that a standalone fragment
		'''   such as {@code "#foo"} parses as a relative URI with an empty path
		'''   and the given fragment, and can be usefully <a
		'''   href="#resolve-frag">resolved</a> against a base URI.
		''' 
		'''   <li><p> IPv4 addresses in host components are parsed rigorously, as
		'''   specified by <a
		'''   href="http://www.ietf.org/rfc/rfc2732.txt">RFC&nbsp;2732</a>: Each
		'''   element of a dotted-quad address must contain no more than three
		'''   decimal digits.  Each element is further constrained to have a value
		'''   no greater than 255. </p></li>
		''' 
		'''   <li> <p> Hostnames in host components that comprise only a single
		'''   domain label are permitted to start with an <i>alphanum</i>
		'''   character. This seems to be the intent of <a
		'''   href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>
		'''   section&nbsp;3.2.2 although the grammar does not permit it. The
		'''   consequence of this deviation is that the authority component of a
		'''   hierarchical URI such as {@code s://123}, will parse as a server-based
		'''   authority. </p></li>
		''' 
		'''   <li><p> IPv6 addresses are permitted for the host component.  An IPv6
		'''   address must be enclosed in square brackets ({@code '['} and
		'''   {@code ']'}) as specified by <a
		'''   href="http://www.ietf.org/rfc/rfc2732.txt">RFC&nbsp;2732</a>.  The
		'''   IPv6 address itself must parse according to <a
		'''   href="http://www.ietf.org/rfc/rfc2373.txt">RFC&nbsp;2373</a>.  IPv6
		'''   addresses are further constrained to describe no more than sixteen
		'''   bytes of address information, a constraint implicit in RFC&nbsp;2373
		'''   but not expressible in the grammar. </p></li>
		''' 
		'''   <li><p> Characters in the <i>other</i> category are permitted wherever
		'''   RFC&nbsp;2396 permits <i>escaped</i> octets, that is, in the
		'''   user-information, path, query, and fragment components, as well as in
		'''   the authority component if the authority is registry-based.  This
		'''   allows URIs to contain Unicode characters beyond those in the US-ASCII
		'''   character set. </p></li>
		''' 
		''' </ul>
		''' </summary>
		''' <param name="str">   The string to be parsed into a URI
		''' </param>
		''' <exception cref="NullPointerException">
		'''          If {@code str} is {@code null}
		''' </exception>
		''' <exception cref="URISyntaxException">
		'''          If the given string violates RFC&nbsp;2396, as augmented
		'''          by the above deviations </exception>
		Public Sub New(ByVal str As String)
			CType(New Parser(Me, str), Parser).parse(False)
		End Sub

		''' <summary>
		''' Constructs a hierarchical URI from the given components.
		''' 
		''' <p> If a scheme is given then the path, if also given, must either be
		''' empty or begin with a slash character ({@code '/'}).  Otherwise a
		''' component of the new URI may be left undefined by passing {@code null}
		''' for the corresponding parameter or, in the case of the {@code port}
		''' parameter, by passing {@code -1}.
		''' 
		''' <p> This constructor first builds a URI string from the given components
		''' according to the rules specified in <a
		''' href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		''' section&nbsp;5.2, step&nbsp;7: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> Initially, the result string is empty. </p></li>
		''' 
		'''   <li><p> If a scheme is given then it is appended to the result,
		'''   followed by a colon character ({@code ':'}).  </p></li>
		''' 
		'''   <li><p> If user information, a host, or a port are given then the
		'''   string {@code "//"} is appended.  </p></li>
		''' 
		'''   <li><p> If user information is given then it is appended, followed by
		'''   a commercial-at character ({@code '@'}).  Any character not in the
		'''   <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		'''   categories is <a href="#quote">quoted</a>.  </p></li>
		''' 
		'''   <li><p> If a host is given then it is appended.  If the host is a
		'''   literal IPv6 address but is not enclosed in square brackets
		'''   ({@code '['} and {@code ']'}) then the square brackets are added.
		'''   </p></li>
		''' 
		'''   <li><p> If a port number is given then a colon character
		'''   ({@code ':'}) is appended, followed by the port number in decimal.
		'''   </p></li>
		''' 
		'''   <li><p> If a path is given then it is appended.  Any character not in
		'''   the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		'''   categories, and not equal to the slash character ({@code '/'}) or the
		'''   commercial-at character ({@code '@'}), is quoted.  </p></li>
		''' 
		'''   <li><p> If a query is given then a question-mark character
		'''   ({@code '?'}) is appended, followed by the query.  Any character that
		'''   is not a <a href="#legal-chars">legal URI character</a> is quoted.
		'''   </p></li>
		''' 
		'''   <li><p> Finally, if a fragment is given then a hash character
		'''   ({@code '#'}) is appended, followed by the fragment.  Any character
		'''   that is not a legal URI character is quoted.  </p></li>
		''' 
		''' </ol>
		''' 
		''' <p> The resulting URI string is then parsed as if by invoking the {@link
		''' #URI(String)} constructor and then invoking the {@link
		''' #parseServerAuthority()} method upon the result; this may cause a {@link
		''' URISyntaxException} to be thrown.  </p>
		''' </summary>
		''' <param name="scheme">    Scheme name </param>
		''' <param name="userInfo">  User name and authorization information </param>
		''' <param name="host">      Host name </param>
		''' <param name="port">      Port number </param>
		''' <param name="path">      Path </param>
		''' <param name="query">     Query </param>
		''' <param name="fragment">  Fragment
		''' </param>
		''' <exception cref="URISyntaxException">
		'''         If both a scheme and a path are given but the path is relative,
		'''         if the URI string constructed from the given components violates
		'''         RFC&nbsp;2396, or if the authority component of the string is
		'''         present but cannot be parsed as a server-based authority </exception>
		Public Sub New(ByVal scheme As String, ByVal userInfo As String, ByVal host As String, ByVal port As Integer, ByVal path As String, ByVal query As String, ByVal fragment As String)
			Dim s As String = ToString(scheme, Nothing, Nothing, userInfo, host, port, path, query, fragment)
			checkPath(s, scheme, path)
			CType(New Parser(Me, s), Parser).parse(True)
		End Sub

		''' <summary>
		''' Constructs a hierarchical URI from the given components.
		''' 
		''' <p> If a scheme is given then the path, if also given, must either be
		''' empty or begin with a slash character ({@code '/'}).  Otherwise a
		''' component of the new URI may be left undefined by passing {@code null}
		''' for the corresponding parameter.
		''' 
		''' <p> This constructor first builds a URI string from the given components
		''' according to the rules specified in <a
		''' href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		''' section&nbsp;5.2, step&nbsp;7: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> Initially, the result string is empty.  </p></li>
		''' 
		'''   <li><p> If a scheme is given then it is appended to the result,
		'''   followed by a colon character ({@code ':'}).  </p></li>
		''' 
		'''   <li><p> If an authority is given then the string {@code "//"} is
		'''   appended, followed by the authority.  If the authority contains a
		'''   literal IPv6 address then the address must be enclosed in square
		'''   brackets ({@code '['} and {@code ']'}).  Any character not in the
		'''   <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		'''   categories, and not equal to the commercial-at character
		'''   ({@code '@'}), is <a href="#quote">quoted</a>.  </p></li>
		''' 
		'''   <li><p> If a path is given then it is appended.  Any character not in
		'''   the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, or <i>other</i>
		'''   categories, and not equal to the slash character ({@code '/'}) or the
		'''   commercial-at character ({@code '@'}), is quoted.  </p></li>
		''' 
		'''   <li><p> If a query is given then a question-mark character
		'''   ({@code '?'}) is appended, followed by the query.  Any character that
		'''   is not a <a href="#legal-chars">legal URI character</a> is quoted.
		'''   </p></li>
		''' 
		'''   <li><p> Finally, if a fragment is given then a hash character
		'''   ({@code '#'}) is appended, followed by the fragment.  Any character
		'''   that is not a legal URI character is quoted.  </p></li>
		''' 
		''' </ol>
		''' 
		''' <p> The resulting URI string is then parsed as if by invoking the {@link
		''' #URI(String)} constructor and then invoking the {@link
		''' #parseServerAuthority()} method upon the result; this may cause a {@link
		''' URISyntaxException} to be thrown.  </p>
		''' </summary>
		''' <param name="scheme">     Scheme name </param>
		''' <param name="authority">  Authority </param>
		''' <param name="path">       Path </param>
		''' <param name="query">      Query </param>
		''' <param name="fragment">   Fragment
		''' </param>
		''' <exception cref="URISyntaxException">
		'''         If both a scheme and a path are given but the path is relative,
		'''         if the URI string constructed from the given components violates
		'''         RFC&nbsp;2396, or if the authority component of the string is
		'''         present but cannot be parsed as a server-based authority </exception>
		Public Sub New(ByVal scheme As String, ByVal authority As String, ByVal path As String, ByVal query As String, ByVal fragment As String)
			Dim s As String = ToString(scheme, Nothing, authority, Nothing, Nothing, -1, path, query, fragment)
			checkPath(s, scheme, path)
			CType(New Parser(Me, s), Parser).parse(False)
		End Sub

		''' <summary>
		''' Constructs a hierarchical URI from the given components.
		''' 
		''' <p> A component may be left undefined by passing {@code null}.
		''' 
		''' <p> This convenience constructor works as if by invoking the
		''' seven-argument constructor as follows:
		''' 
		''' <blockquote>
		''' {@code new} {@link #URI(String, String, String, int, String, String, String)
		''' URI}{@code (scheme, null, host, -1, path, null, fragment);}
		''' </blockquote>
		''' </summary>
		''' <param name="scheme">    Scheme name </param>
		''' <param name="host">      Host name </param>
		''' <param name="path">      Path </param>
		''' <param name="fragment">  Fragment
		''' </param>
		''' <exception cref="URISyntaxException">
		'''          If the URI string constructed from the given components
		'''          violates RFC&nbsp;2396 </exception>
		Public Sub New(ByVal scheme As String, ByVal host As String, ByVal path As String, ByVal fragment As String)
			Me.New(scheme, Nothing, host, -1, path, Nothing, fragment)
		End Sub

		''' <summary>
		''' Constructs a URI from the given components.
		''' 
		''' <p> A component may be left undefined by passing {@code null}.
		''' 
		''' <p> This constructor first builds a URI in string form using the given
		''' components as follows:  </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> Initially, the result string is empty.  </p></li>
		''' 
		'''   <li><p> If a scheme is given then it is appended to the result,
		'''   followed by a colon character ({@code ':'}).  </p></li>
		''' 
		'''   <li><p> If a scheme-specific part is given then it is appended.  Any
		'''   character that is not a <a href="#legal-chars">legal URI character</a>
		'''   is <a href="#quote">quoted</a>.  </p></li>
		''' 
		'''   <li><p> Finally, if a fragment is given then a hash character
		'''   ({@code '#'}) is appended to the string, followed by the fragment.
		'''   Any character that is not a legal URI character is quoted.  </p></li>
		''' 
		''' </ol>
		''' 
		''' <p> The resulting URI string is then parsed in order to create the new
		''' URI instance as if by invoking the <seealso cref="#URI(String)"/> constructor;
		''' this may cause a <seealso cref="URISyntaxException"/> to be thrown.  </p>
		''' </summary>
		''' <param name="scheme">    Scheme name </param>
		''' <param name="ssp">       Scheme-specific part </param>
		''' <param name="fragment">  Fragment
		''' </param>
		''' <exception cref="URISyntaxException">
		'''          If the URI string constructed from the given components
		'''          violates RFC&nbsp;2396 </exception>
		Public Sub New(ByVal scheme As String, ByVal ssp As String, ByVal fragment As String)
			CType(New Parser(Me, ToString(scheme, ssp, Nothing, Nothing, Nothing, -1, Nothing, Nothing, fragment)), Parser).parse(False)
		End Sub

		''' <summary>
		''' Creates a URI by parsing the given string.
		''' 
		''' <p> This convenience factory method works as if by invoking the {@link
		''' #URI(String)} constructor; any <seealso cref="URISyntaxException"/> thrown by the
		''' constructor is caught and wrapped in a new {@link
		''' IllegalArgumentException} object, which is then thrown.
		''' 
		''' <p> This method is provided for use in situations where it is known that
		''' the given string is a legal URI, for example for URI constants declared
		''' within in a program, and so it would be considered a programming error
		''' for the string not to parse as such.  The constructors, which throw
		''' <seealso cref="URISyntaxException"/> directly, should be used situations where a
		''' URI is being constructed from user input or from some other source that
		''' may be prone to errors.  </p>
		''' </summary>
		''' <param name="str">   The string to be parsed into a URI </param>
		''' <returns> The new URI
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code str} is {@code null}
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the given string violates RFC&nbsp;2396 </exception>
		Public Shared Function create(ByVal str As String) As URI
			Try
				Return New URI(str)
			Catch x As URISyntaxException
				Throw New IllegalArgumentException(x.Message, x)
			End Try
		End Function


		' -- Operations --

		''' <summary>
		''' Attempts to parse this URI's authority component, if defined, into
		''' user-information, host, and port components.
		''' 
		''' <p> If this URI's authority component has already been recognized as
		''' being server-based then it will already have been parsed into
		''' user-information, host, and port components.  In this case, or if this
		''' URI has no authority component, this method simply returns this URI.
		''' 
		''' <p> Otherwise this method attempts once more to parse the authority
		''' component into user-information, host, and port components, and throws
		''' an exception describing why the authority component could not be parsed
		''' in that way.
		''' 
		''' <p> This method is provided because the generic URI syntax specified in
		''' <a href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>
		''' cannot always distinguish a malformed server-based authority from a
		''' legitimate registry-based authority.  It must therefore treat some
		''' instances of the former as instances of the latter.  The authority
		''' component in the URI string {@code "//foo:bar"}, for example, is not a
		''' legal server-based authority but it is legal as a registry-based
		''' authority.
		''' 
		''' <p> In many common situations, for example when working URIs that are
		''' known to be either URNs or URLs, the hierarchical URIs being used will
		''' always be server-based.  They therefore must either be parsed as such or
		''' treated as an error.  In these cases a statement such as
		''' 
		''' <blockquote>
		''' {@code URI }<i>u</i>{@code  = new URI(str).parseServerAuthority();}
		''' </blockquote>
		''' 
		''' <p> can be used to ensure that <i>u</i> always refers to a URI that, if
		''' it has an authority component, has a server-based authority with proper
		''' user-information, host, and port components.  Invoking this method also
		''' ensures that if the authority could not be parsed in that way then an
		''' appropriate diagnostic message can be issued based upon the exception
		''' that is thrown. </p>
		''' </summary>
		''' <returns>  A URI whose authority field has been parsed
		'''          as a server-based authority
		''' </returns>
		''' <exception cref="URISyntaxException">
		'''          If the authority component of this URI is defined
		'''          but cannot be parsed as a server-based authority
		'''          according to RFC&nbsp;2396 </exception>
		Public Function parseServerAuthority() As URI
			' We could be clever and cache the error message and index from the
			' exception thrown during the original parse, but that would require
			' either more fields or a more-obscure representation.
			If (host IsNot Nothing) OrElse (authority Is Nothing) Then Return Me
			defineString()
			CType(New Parser(Me, string_Renamed), Parser).parse(True)
			Return Me
		End Function

		''' <summary>
		''' Normalizes this URI's path.
		''' 
		''' <p> If this URI is opaque, or if its path is already in normal form,
		''' then this URI is returned.  Otherwise a new URI is constructed that is
		''' identical to this URI except that its path is computed by normalizing
		''' this URI's path in a manner consistent with <a
		''' href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		''' section&nbsp;5.2, step&nbsp;6, sub-steps&nbsp;c through&nbsp;f; that is:
		''' </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> All {@code "."} segments are removed. </p></li>
		''' 
		'''   <li><p> If a {@code ".."} segment is preceded by a non-{@code ".."}
		'''   segment then both of these segments are removed.  This step is
		'''   repeated until it is no longer applicable. </p></li>
		''' 
		'''   <li><p> If the path is relative, and if its first segment contains a
		'''   colon character ({@code ':'}), then a {@code "."} segment is
		'''   prepended.  This prevents a relative URI with a path such as
		'''   {@code "a:b/c/d"} from later being re-parsed as an opaque URI with a
		'''   scheme of {@code "a"} and a scheme-specific part of {@code "b/c/d"}.
		'''   <b><i>(Deviation from RFC&nbsp;2396)</i></b> </p></li>
		''' 
		''' </ol>
		''' 
		''' <p> A normalized path will begin with one or more {@code ".."} segments
		''' if there were insufficient non-{@code ".."} segments preceding them to
		''' allow their removal.  A normalized path will begin with a {@code "."}
		''' segment if one was inserted by step 3 above.  Otherwise, a normalized
		''' path will not contain any {@code "."} or {@code ".."} segments. </p>
		''' </summary>
		''' <returns>  A URI equivalent to this URI,
		'''          but whose path is in normal form </returns>
		Public Function normalize() As URI
			Return normalize(Me)
		End Function

		''' <summary>
		''' Resolves the given URI against this URI.
		''' 
		''' <p> If the given URI is already absolute, or if this URI is opaque, then
		''' the given URI is returned.
		''' 
		''' <p><a name="resolve-frag"></a> If the given URI's fragment component is
		''' defined, its path component is empty, and its scheme, authority, and
		''' query components are undefined, then a URI with the given fragment but
		''' with all other components equal to those of this URI is returned.  This
		''' allows a URI representing a standalone fragment reference, such as
		''' {@code "#foo"}, to be usefully resolved against a base URI.
		''' 
		''' <p> Otherwise this method constructs a new hierarchical URI in a manner
		''' consistent with <a
		''' href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		''' section&nbsp;5.2; that is: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> A new URI is constructed with this URI's scheme and the given
		'''   URI's query and fragment components. </p></li>
		''' 
		'''   <li><p> If the given URI has an authority component then the new URI's
		'''   authority and path are taken from the given URI. </p></li>
		''' 
		'''   <li><p> Otherwise the new URI's authority component is copied from
		'''   this URI, and its path is computed as follows: </p>
		''' 
		'''   <ol>
		''' 
		'''     <li><p> If the given URI's path is absolute then the new URI's path
		'''     is taken from the given URI. </p></li>
		''' 
		'''     <li><p> Otherwise the given URI's path is relative, and so the new
		'''     URI's path is computed by resolving the path of the given URI
		'''     against the path of this URI.  This is done by concatenating all but
		'''     the last segment of this URI's path, if any, with the given URI's
		'''     path and then normalizing the result as if by invoking the {@link
		'''     #normalize() normalize} method. </p></li>
		''' 
		'''   </ol></li>
		''' 
		''' </ol>
		''' 
		''' <p> The result of this method is absolute if, and only if, either this
		''' URI is absolute or the given URI is absolute.  </p>
		''' </summary>
		''' <param name="uri">  The URI to be resolved against this URI </param>
		''' <returns> The resulting URI
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code uri} is {@code null} </exception>
		Public Function resolve(ByVal uri As URI) As URI
			Return resolve(Me, uri)
		End Function

		''' <summary>
		''' Constructs a new URI by parsing the given string and then resolving it
		''' against this URI.
		''' 
		''' <p> This convenience method works as if invoking it were equivalent to
		''' evaluating the expression {@link #resolve(java.net.URI)
		''' resolve}{@code (URI.}<seealso cref="#create(String) create"/>{@code (str))}. </p>
		''' </summary>
		''' <param name="str">   The string to be parsed into a URI </param>
		''' <returns> The resulting URI
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code str} is {@code null}
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the given string violates RFC&nbsp;2396 </exception>
		Public Function resolve(ByVal str As String) As URI
			Return resolve(URI.create(str))
		End Function

		''' <summary>
		''' Relativizes the given URI against this URI.
		''' 
		''' <p> The relativization of the given URI against this URI is computed as
		''' follows: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> If either this URI or the given URI are opaque, or if the
		'''   scheme and authority components of the two URIs are not identical, or
		'''   if the path of this URI is not a prefix of the path of the given URI,
		'''   then the given URI is returned. </p></li>
		''' 
		'''   <li><p> Otherwise a new relative hierarchical URI is constructed with
		'''   query and fragment components taken from the given URI and with a path
		'''   component computed by removing this URI's path from the beginning of
		'''   the given URI's path. </p></li>
		''' 
		''' </ol>
		''' </summary>
		''' <param name="uri">  The URI to be relativized against this URI </param>
		''' <returns> The resulting URI
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code uri} is {@code null} </exception>
		Public Function relativize(ByVal uri As URI) As URI
			Return relativize(Me, uri)
		End Function

		''' <summary>
		''' Constructs a URL from this URI.
		''' 
		''' <p> This convenience method works as if invoking it were equivalent to
		''' evaluating the expression {@code new URL(this.toString())} after
		''' first checking that this URI is absolute. </p>
		''' </summary>
		''' <returns>  A URL constructed from this URI
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If this URL is not absolute
		''' </exception>
		''' <exception cref="MalformedURLException">
		'''          If a protocol handler for the URL could not be found,
		'''          or if some other error occurred while constructing the URL </exception>
		Public Function toURL() As URL
			If Not absolute Then Throw New IllegalArgumentException("URI is not absolute")
			Return New URL(ToString())
		End Function

		' -- Component access methods --

		''' <summary>
		''' Returns the scheme component of this URI.
		''' 
		''' <p> The scheme component of a URI, if defined, only contains characters
		''' in the <i>alphanum</i> category and in the string {@code "-.+"}.  A
		''' scheme always starts with an <i>alpha</i> character. <p>
		''' 
		''' The scheme component of a URI cannot contain escaped octets, hence this
		''' method does not perform any decoding.
		''' </summary>
		''' <returns>  The scheme component of this URI,
		'''          or {@code null} if the scheme is undefined </returns>
		Public Property scheme As String
			Get
				Return scheme
			End Get
		End Property

		''' <summary>
		''' Tells whether or not this URI is absolute.
		''' 
		''' <p> A URI is absolute if, and only if, it has a scheme component. </p>
		''' </summary>
		''' <returns>  {@code true} if, and only if, this URI is absolute </returns>
		Public Property absolute As Boolean
			Get
				Return scheme IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Tells whether or not this URI is opaque.
		''' 
		''' <p> A URI is opaque if, and only if, it is absolute and its
		''' scheme-specific part does not begin with a slash character ('/').
		''' An opaque URI has a scheme, a scheme-specific part, and possibly
		''' a fragment; all other components are undefined. </p>
		''' </summary>
		''' <returns>  {@code true} if, and only if, this URI is opaque </returns>
		Public Property opaque As Boolean
			Get
				Return path Is Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the raw scheme-specific part of this URI.  The scheme-specific
		''' part is never undefined, though it may be empty.
		''' 
		''' <p> The scheme-specific part of a URI only contains legal URI
		''' characters. </p>
		''' </summary>
		''' <returns>  The raw scheme-specific part of this URI
		'''          (never {@code null}) </returns>
		Public Property rawSchemeSpecificPart As String
			Get
				defineSchemeSpecificPart()
				Return schemeSpecificPart
			End Get
		End Property

		''' <summary>
		''' Returns the decoded scheme-specific part of this URI.
		''' 
		''' <p> The string returned by this method is equal to that returned by the
		''' <seealso cref="#getRawSchemeSpecificPart() getRawSchemeSpecificPart"/> method
		''' except that all sequences of escaped octets are <a
		''' href="#decode">decoded</a>.  </p>
		''' </summary>
		''' <returns>  The decoded scheme-specific part of this URI
		'''          (never {@code null}) </returns>
		Public Property schemeSpecificPart As String
			Get
				If decodedSchemeSpecificPart Is Nothing Then decodedSchemeSpecificPart = decode(rawSchemeSpecificPart)
				Return decodedSchemeSpecificPart
			End Get
		End Property

		''' <summary>
		''' Returns the raw authority component of this URI.
		''' 
		''' <p> The authority component of a URI, if defined, only contains the
		''' commercial-at character ({@code '@'}) and characters in the
		''' <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, and <i>other</i>
		''' categories.  If the authority is server-based then it is further
		''' constrained to have valid user-information, host, and port
		''' components. </p>
		''' </summary>
		''' <returns>  The raw authority component of this URI,
		'''          or {@code null} if the authority is undefined </returns>
		Public Property rawAuthority As String
			Get
				Return authority
			End Get
		End Property

		''' <summary>
		''' Returns the decoded authority component of this URI.
		''' 
		''' <p> The string returned by this method is equal to that returned by the
		''' <seealso cref="#getRawAuthority() getRawAuthority"/> method except that all
		''' sequences of escaped octets are <a href="#decode">decoded</a>.  </p>
		''' </summary>
		''' <returns>  The decoded authority component of this URI,
		'''          or {@code null} if the authority is undefined </returns>
		Public Property authority As String
			Get
				If decodedAuthority Is Nothing Then decodedAuthority = decode(authority)
				Return decodedAuthority
			End Get
		End Property

		''' <summary>
		''' Returns the raw user-information component of this URI.
		''' 
		''' <p> The user-information component of a URI, if defined, only contains
		''' characters in the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>, and
		''' <i>other</i> categories. </p>
		''' </summary>
		''' <returns>  The raw user-information component of this URI,
		'''          or {@code null} if the user information is undefined </returns>
		Public Property rawUserInfo As String
			Get
				Return userInfo
			End Get
		End Property

		''' <summary>
		''' Returns the decoded user-information component of this URI.
		''' 
		''' <p> The string returned by this method is equal to that returned by the
		''' <seealso cref="#getRawUserInfo() getRawUserInfo"/> method except that all
		''' sequences of escaped octets are <a href="#decode">decoded</a>.  </p>
		''' </summary>
		''' <returns>  The decoded user-information component of this URI,
		'''          or {@code null} if the user information is undefined </returns>
		Public Property userInfo As String
			Get
				If (decodedUserInfo Is Nothing) AndAlso (userInfo IsNot Nothing) Then decodedUserInfo = decode(userInfo)
				Return decodedUserInfo
			End Get
		End Property

		''' <summary>
		''' Returns the host component of this URI.
		''' 
		''' <p> The host component of a URI, if defined, will have one of the
		''' following forms: </p>
		''' 
		''' <ul>
		''' 
		'''   <li><p> A domain name consisting of one or more <i>labels</i>
		'''   separated by period characters ({@code '.'}), optionally followed by
		'''   a period character.  Each label consists of <i>alphanum</i> characters
		'''   as well as hyphen characters ({@code '-'}), though hyphens never
		'''   occur as the first or last characters in a label. The rightmost
		'''   label of a domain name consisting of two or more labels, begins
		'''   with an <i>alpha</i> character. </li>
		''' 
		'''   <li><p> A dotted-quad IPv4 address of the form
		'''   <i>digit</i>{@code +.}<i>digit</i>{@code +.}<i>digit</i>{@code +.}<i>digit</i>{@code +},
		'''   where no <i>digit</i> sequence is longer than three characters and no
		'''   sequence has a value larger than 255. </p></li>
		''' 
		'''   <li><p> An IPv6 address enclosed in square brackets ({@code '['} and
		'''   {@code ']'}) and consisting of hexadecimal digits, colon characters
		'''   ({@code ':'}), and possibly an embedded IPv4 address.  The full
		'''   syntax of IPv6 addresses is specified in <a
		'''   href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IPv6
		'''   Addressing Architecture</i></a>.  </p></li>
		''' 
		''' </ul>
		''' 
		''' The host component of a URI cannot contain escaped octets, hence this
		''' method does not perform any decoding.
		''' </summary>
		''' <returns>  The host component of this URI,
		'''          or {@code null} if the host is undefined </returns>
		Public Property host As String
			Get
				Return host
			End Get
		End Property

		''' <summary>
		''' Returns the port number of this URI.
		''' 
		''' <p> The port component of a URI, if defined, is a non-negative
		''' integer. </p>
		''' </summary>
		''' <returns>  The port component of this URI,
		'''          or {@code -1} if the port is undefined </returns>
		Public Property port As Integer
			Get
				Return port
			End Get
		End Property

		''' <summary>
		''' Returns the raw path component of this URI.
		''' 
		''' <p> The path component of a URI, if defined, only contains the slash
		''' character ({@code '/'}), the commercial-at character ({@code '@'}),
		''' and characters in the <i>unreserved</i>, <i>punct</i>, <i>escaped</i>,
		''' and <i>other</i> categories. </p>
		''' </summary>
		''' <returns>  The path component of this URI,
		'''          or {@code null} if the path is undefined </returns>
		Public Property rawPath As String
			Get
				Return path
			End Get
		End Property

		''' <summary>
		''' Returns the decoded path component of this URI.
		''' 
		''' <p> The string returned by this method is equal to that returned by the
		''' <seealso cref="#getRawPath() getRawPath"/> method except that all sequences of
		''' escaped octets are <a href="#decode">decoded</a>.  </p>
		''' </summary>
		''' <returns>  The decoded path component of this URI,
		'''          or {@code null} if the path is undefined </returns>
		Public Property path As String
			Get
				If (decodedPath Is Nothing) AndAlso (path IsNot Nothing) Then decodedPath = decode(path)
				Return decodedPath
			End Get
		End Property

		''' <summary>
		''' Returns the raw query component of this URI.
		''' 
		''' <p> The query component of a URI, if defined, only contains legal URI
		''' characters. </p>
		''' </summary>
		''' <returns>  The raw query component of this URI,
		'''          or {@code null} if the query is undefined </returns>
		Public Property rawQuery As String
			Get
				Return query
			End Get
		End Property

		''' <summary>
		''' Returns the decoded query component of this URI.
		''' 
		''' <p> The string returned by this method is equal to that returned by the
		''' <seealso cref="#getRawQuery() getRawQuery"/> method except that all sequences of
		''' escaped octets are <a href="#decode">decoded</a>.  </p>
		''' </summary>
		''' <returns>  The decoded query component of this URI,
		'''          or {@code null} if the query is undefined </returns>
		Public Property query As String
			Get
				If (decodedQuery Is Nothing) AndAlso (query IsNot Nothing) Then decodedQuery = decode(query)
				Return decodedQuery
			End Get
		End Property

		''' <summary>
		''' Returns the raw fragment component of this URI.
		''' 
		''' <p> The fragment component of a URI, if defined, only contains legal URI
		''' characters. </p>
		''' </summary>
		''' <returns>  The raw fragment component of this URI,
		'''          or {@code null} if the fragment is undefined </returns>
		Public Property rawFragment As String
			Get
				Return fragment
			End Get
		End Property

		''' <summary>
		''' Returns the decoded fragment component of this URI.
		''' 
		''' <p> The string returned by this method is equal to that returned by the
		''' <seealso cref="#getRawFragment() getRawFragment"/> method except that all
		''' sequences of escaped octets are <a href="#decode">decoded</a>.  </p>
		''' </summary>
		''' <returns>  The decoded fragment component of this URI,
		'''          or {@code null} if the fragment is undefined </returns>
		Public Property fragment As String
			Get
				If (decodedFragment Is Nothing) AndAlso (fragment IsNot Nothing) Then decodedFragment = decode(fragment)
				Return decodedFragment
			End Get
		End Property


		' -- Equality, comparison, hash code, toString, and serialization --

		''' <summary>
		''' Tests this URI for equality with another object.
		''' 
		''' <p> If the given object is not a URI then this method immediately
		''' returns {@code false}.
		''' 
		''' <p> For two URIs to be considered equal requires that either both are
		''' opaque or both are hierarchical.  Their schemes must either both be
		''' undefined or else be equal without regard to case. Their fragments
		''' must either both be undefined or else be equal.
		''' 
		''' <p> For two opaque URIs to be considered equal, their scheme-specific
		''' parts must be equal.
		''' 
		''' <p> For two hierarchical URIs to be considered equal, their paths must
		''' be equal and their queries must either both be undefined or else be
		''' equal.  Their authorities must either both be undefined, or both be
		''' registry-based, or both be server-based.  If their authorities are
		''' defined and are registry-based, then they must be equal.  If their
		''' authorities are defined and are server-based, then their hosts must be
		''' equal without regard to case, their port numbers must be equal, and
		''' their user-information components must be equal.
		''' 
		''' <p> When testing the user-information, path, query, fragment, authority,
		''' or scheme-specific parts of two URIs for equality, the raw forms rather
		''' than the encoded forms of these components are compared and the
		''' hexadecimal digits of escaped octets are compared without regard to
		''' case.
		''' 
		''' <p> This method satisfies the general contract of the {@link
		''' java.lang.Object#equals(Object) Object.equals} method. </p>
		''' </summary>
		''' <param name="ob">   The object to which this object is to be compared
		''' </param>
		''' <returns>  {@code true} if, and only if, the given object is a URI that
		'''          is identical to this URI </returns>
		Public Overrides Function Equals(ByVal ob As Object) As Boolean
			If ob Is Me Then Return True
			If Not(TypeOf ob Is URI) Then Return False
			Dim that As URI = CType(ob, URI)
			If Me.opaque <> that.opaque Then Return False
			If Not equalIgnoringCase(Me.scheme, that.scheme) Then Return False
			If Not equal(Me.fragment, that.fragment) Then Return False

			' Opaque
			If Me.opaque Then Return equal(Me.schemeSpecificPart, that.schemeSpecificPart)

			' Hierarchical
			If Not equal(Me.path, that.path) Then Return False
			If Not equal(Me.query, that.query) Then Return False

			' Authorities
			If Me.authority = that.authority Then Return True
			If Me.host IsNot Nothing Then
				' Server-based
				If Not equal(Me.userInfo, that.userInfo) Then Return False
				If Not equalIgnoringCase(Me.host, that.host) Then Return False
				If Me.port <> that.port Then Return False
			ElseIf Me.authority IsNot Nothing Then
				' Registry-based
				If Not equal(Me.authority, that.authority) Then Return False
			ElseIf Me.authority <> that.authority Then
				Return False
			End If

			Return True
		End Function

		''' <summary>
		''' Returns a hash-code value for this URI.  The hash code is based upon all
		''' of the URI's components, and satisfies the general contract of the
		''' <seealso cref="java.lang.Object#hashCode() Object.hashCode"/> method.
		''' </summary>
		''' <returns>  A hash-code value for this URI </returns>
		Public Overrides Function GetHashCode() As Integer
			If hash_Renamed <> 0 Then Return hash_Renamed
			Dim h As Integer = hashIgnoringCase(0, scheme)
			h = hash(h, fragment)
			If opaque Then
				h = hash(h, schemeSpecificPart)
			Else
				h = hash(h, path)
				h = hash(h, query)
				If host IsNot Nothing Then
					h = hash(h, userInfo)
					h = hashIgnoringCase(h, host)
					h += 1949 * port
				Else
					h = hash(h, authority)
				End If
			End If
			hash_Renamed = h
			Return h
		End Function

		''' <summary>
		''' Compares this URI to another object, which must be a URI.
		''' 
		''' <p> When comparing corresponding components of two URIs, if one
		''' component is undefined but the other is defined then the first is
		''' considered to be less than the second.  Unless otherwise noted, string
		''' components are ordered according to their natural, case-sensitive
		''' ordering as defined by the {@link java.lang.String#compareTo(Object)
		''' String.compareTo} method.  String components that are subject to
		''' encoding are compared by comparing their raw forms rather than their
		''' encoded forms.
		''' 
		''' <p> The ordering of URIs is defined as follows: </p>
		''' 
		''' <ul>
		''' 
		'''   <li><p> Two URIs with different schemes are ordered according the
		'''   ordering of their schemes, without regard to case. </p></li>
		''' 
		'''   <li><p> A hierarchical URI is considered to be less than an opaque URI
		'''   with an identical scheme. </p></li>
		''' 
		'''   <li><p> Two opaque URIs with identical schemes are ordered according
		'''   to the ordering of their scheme-specific parts. </p></li>
		''' 
		'''   <li><p> Two opaque URIs with identical schemes and scheme-specific
		'''   parts are ordered according to the ordering of their
		'''   fragments. </p></li>
		''' 
		'''   <li><p> Two hierarchical URIs with identical schemes are ordered
		'''   according to the ordering of their authority components: </p>
		''' 
		'''   <ul>
		''' 
		'''     <li><p> If both authority components are server-based then the URIs
		'''     are ordered according to their user-information components; if these
		'''     components are identical then the URIs are ordered according to the
		'''     ordering of their hosts, without regard to case; if the hosts are
		'''     identical then the URIs are ordered according to the ordering of
		'''     their ports. </p></li>
		''' 
		'''     <li><p> If one or both authority components are registry-based then
		'''     the URIs are ordered according to the ordering of their authority
		'''     components. </p></li>
		''' 
		'''   </ul></li>
		''' 
		'''   <li><p> Finally, two hierarchical URIs with identical schemes and
		'''   authority components are ordered according to the ordering of their
		'''   paths; if their paths are identical then they are ordered according to
		'''   the ordering of their queries; if the queries are identical then they
		'''   are ordered according to the order of their fragments. </p></li>
		''' 
		''' </ul>
		''' 
		''' <p> This method satisfies the general contract of the {@link
		''' java.lang.Comparable#compareTo(Object) Comparable.compareTo}
		''' method. </p>
		''' </summary>
		''' <param name="that">
		'''          The object to which this URI is to be compared
		''' </param>
		''' <returns>  A negative integer, zero, or a positive integer as this URI is
		'''          less than, equal to, or greater than the given URI
		''' </returns>
		''' <exception cref="ClassCastException">
		'''          If the given object is not a URI </exception>
		Public Function compareTo(ByVal that As URI) As Integer Implements Comparable(Of URI).compareTo
			Dim c As Integer

			c = compareIgnoringCase(Me.scheme, that.scheme)
			If c <> 0 Then Return c

			If Me.opaque Then
				If that.opaque Then
					' Both opaque
					c = compare(Me.schemeSpecificPart, that.schemeSpecificPart)
					If c <> 0 Then Return c
					Return compare(Me.fragment, that.fragment)
				End If
				Return +1 ' Opaque > hierarchical
			ElseIf that.opaque Then
				Return -1 ' Hierarchical < opaque
			End If

			' Hierarchical
			If (Me.host IsNot Nothing) AndAlso (that.host IsNot Nothing) Then
				' Both server-based
				c = compare(Me.userInfo, that.userInfo)
				If c <> 0 Then Return c
				c = compareIgnoringCase(Me.host, that.host)
				If c <> 0 Then Return c
				c = Me.port - that.port
				If c <> 0 Then Return c
			Else
				' If one or both authorities are registry-based then we simply
				' compare them in the usual, case-sensitive way.  If one is
				' registry-based and one is server-based then the strings are
				' guaranteed to be unequal, hence the comparison will never return
				' zero and the compareTo and equals methods will remain
				' consistent.
				c = compare(Me.authority, that.authority)
				If c <> 0 Then Return c
			End If

			c = compare(Me.path, that.path)
			If c <> 0 Then Return c
			c = compare(Me.query, that.query)
			If c <> 0 Then Return c
			Return compare(Me.fragment, that.fragment)
		End Function

		''' <summary>
		''' Returns the content of this URI as a string.
		''' 
		''' <p> If this URI was created by invoking one of the constructors in this
		''' class then a string equivalent to the original input string, or to the
		''' string computed from the originally-given components, as appropriate, is
		''' returned.  Otherwise this URI was created by normalization, resolution,
		''' or relativization, and so a string is constructed from this URI's
		''' components according to the rules specified in <a
		''' href="http://www.ietf.org/rfc/rfc2396.txt">RFC&nbsp;2396</a>,
		''' section&nbsp;5.2, step&nbsp;7. </p>
		''' </summary>
		''' <returns>  The string form of this URI </returns>
		Public Overrides Function ToString() As String
			defineString()
			Return string_Renamed
		End Function

		''' <summary>
		''' Returns the content of this URI as a US-ASCII string.
		''' 
		''' <p> If this URI does not contain any characters in the <i>other</i>
		''' category then an invocation of this method will return the same value as
		''' an invocation of the <seealso cref="#toString() toString"/> method.  Otherwise
		''' this method works as if by invoking that method and then <a
		''' href="#encode">encoding</a> the result.  </p>
		''' </summary>
		''' <returns>  The string form of this URI, encoded as needed
		'''          so that it only contains characters in the US-ASCII
		'''          charset </returns>
		Public Function toASCIIString() As String
			defineString()
			Return encode(string_Renamed)
		End Function


		' -- Serialization support --

		''' <summary>
		''' Saves the content of this URI to the given serial stream.
		''' 
		''' <p> The only serializable field of a URI instance is its {@code string}
		''' field.  That field is given a value, if it does not have one already,
		''' and then the <seealso cref="java.io.ObjectOutputStream#defaultWriteObject()"/>
		''' method of the given object-output stream is invoked. </p>
		''' </summary>
		''' <param name="os">  The object-output stream to which this object
		'''             is to be written </param>
		Private Sub writeObject(ByVal os As java.io.ObjectOutputStream)
			defineString()
			os.defaultWriteObject() ' Writes the string field only
		End Sub

		''' <summary>
		''' Reconstitutes a URI from the given serial stream.
		''' 
		''' <p> The <seealso cref="java.io.ObjectInputStream#defaultReadObject()"/> method is
		''' invoked to read the value of the {@code string} field.  The result is
		''' then parsed in the usual way.
		''' </summary>
		''' <param name="is">  The object-input stream from which this object
		'''             is being read </param>
		Private Sub readObject(ByVal [is] As java.io.ObjectInputStream)
			port = -1 ' Argh
			[is].defaultReadObject()
			Try
				CType(New Parser(Me, string_Renamed), Parser).parse(False)
			Catch x As URISyntaxException
				Dim y As java.io.IOException = New java.io.InvalidObjectException("Invalid URI")
				y.initCause(x)
				Throw y
			End Try
		End Sub


		' -- End of public methods --


		' -- Utility methods for string-field comparison and hashing --

		' These methods return appropriate values for null string arguments,
		' thereby simplifying the equals, hashCode, and compareTo methods.
		'
		' The case-ignoring methods should only be applied to strings whose
		' characters are all known to be US-ASCII.  Because of this restriction,
		' these methods are faster than the similar methods in the String class.

		' US-ASCII only
		Private Shared Function toLower(ByVal c As Char) As Integer
			If (c >= "A"c) AndAlso (c <= "Z"c) Then Return AscW(c) + (AscW("a"c) - AscW("A"c))
			Return c
		End Function

		' US-ASCII only
		Private Shared Function toUpper(ByVal c As Char) As Integer
			If (c >= "a"c) AndAlso (c <= "z"c) Then Return AscW(c) - (AscW("a"c) - AscW("A"c))
			Return c
		End Function

		Private Shared Function equal(ByVal s As String, ByVal t As String) As Boolean
			If s = t Then Return True
			If (s IsNot Nothing) AndAlso (t IsNot Nothing) Then
				If s.length() <> t.length() Then Return False
				If s.IndexOf("%"c) < 0 Then Return s.Equals(t)
				Dim n As Integer = s.length()
				Dim i As Integer = 0
				Do While i < n
					Dim c As Char = s.Chars(i)
					Dim d As Char = t.Chars(i)
					If c <> "%"c Then
						If c <> d Then Return False
						i += 1
						Continue Do
					End If
					If d <> "%"c Then Return False
					i += 1
					If toLower(s.Chars(i)) <> toLower(t.Chars(i)) Then Return False
					i += 1
					If toLower(s.Chars(i)) <> toLower(t.Chars(i)) Then Return False
					i += 1
				Loop
				Return True
			End If
			Return False
		End Function

		' US-ASCII only
		Private Shared Function equalIgnoringCase(ByVal s As String, ByVal t As String) As Boolean
			If s = t Then Return True
			If (s IsNot Nothing) AndAlso (t IsNot Nothing) Then
				Dim n As Integer = s.length()
				If t.length() <> n Then Return False
				For i As Integer = 0 To n - 1
					If toLower(s.Chars(i)) <> toLower(t.Chars(i)) Then Return False
				Next i
				Return True
			End If
			Return False
		End Function

		Private Shared Function hash(ByVal hash_Renamed As Integer, ByVal s As String) As Integer
			If s Is Nothing Then Return hash_Renamed
			Return If(s.IndexOf("%"c) < 0, hash_Renamed * 127 + s.GetHashCode(), normalizedHash(hash_Renamed, s))
		End Function


		Private Shared Function normalizedHash(ByVal hash As Integer, ByVal s As String) As Integer
			Dim h As Integer = 0
			For index As Integer = 0 To s.length() - 1
				Dim ch As Char = s.Chars(index)
				h = 31 * h + AscW(ch)
				If ch = "%"c Then
	'                
	'                 * Process the next two encoded characters
	'                 
					For i As Integer = index + 1 To index + 3 - 1
						h = 31 * h + toUpper(s.Chars(i))
					Next i
					index += 2
				End If
			Next index
			Return hash * 127 + h
		End Function

		' US-ASCII only
		Private Shared Function hashIgnoringCase(ByVal hash As Integer, ByVal s As String) As Integer
			If s Is Nothing Then Return hash
			Dim h As Integer = hash
			Dim n As Integer = s.length()
			For i As Integer = 0 To n - 1
				h = 31 * h + toLower(s.Chars(i))
			Next i
			Return h
		End Function

		Private Shared Function compare(ByVal s As String, ByVal t As String) As Integer
			If s = t Then Return 0
			If s IsNot Nothing Then
				If t IsNot Nothing Then
					Return s.CompareTo(t)
				Else
					Return +1
				End If
			Else
				Return -1
			End If
		End Function

		' US-ASCII only
		Private Shared Function compareIgnoringCase(ByVal s As String, ByVal t As String) As Integer
			If s = t Then Return 0
			If s IsNot Nothing Then
				If t IsNot Nothing Then
					Dim sn As Integer = s.length()
					Dim tn As Integer = t.length()
					Dim n As Integer = If(sn < tn, sn, tn)
					For i As Integer = 0 To n - 1
						Dim c As Integer = toLower(s.Chars(i)) - toLower(t.Chars(i))
						If c <> 0 Then Return c
					Next i
					Return sn - tn
				End If
				Return +1
			Else
				Return -1
			End If
		End Function


		' -- String construction --

		' If a scheme is given then the path, if given, must be absolute
		'
		Private Shared Sub checkPath(ByVal s As String, ByVal scheme As String, ByVal path As String)
			If scheme IsNot Nothing Then
				If (path IsNot Nothing) AndAlso ((path.length() > 0) AndAlso (path.Chars(0) <> "/"c)) Then Throw New URISyntaxException(s, "Relative path in absolute URI")
			End If
		End Sub

		Private Sub appendAuthority(ByVal sb As StringBuffer, ByVal authority As String, ByVal userInfo As String, ByVal host As String, ByVal port As Integer)
			If host IsNot Nothing Then
				sb.append("//")
				If userInfo IsNot Nothing Then
					sb.append(quote(userInfo, L_USERINFO, H_USERINFO))
					sb.append("@"c)
				End If
				Dim needBrackets As Boolean = ((host.IndexOf(":"c) >= 0) AndAlso (Not host.StartsWith("[")) AndAlso (Not host.EndsWith("]")))
				If needBrackets Then sb.append("["c)
				sb.append(host)
				If needBrackets Then sb.append("]"c)
				If port <> -1 Then
					sb.append(":"c)
					sb.append(port)
				End If
			ElseIf authority IsNot Nothing Then
				sb.append("//")
				If authority.StartsWith("[") Then
					' authority should (but may not) contain an embedded IPv6 address
					Dim [end] As Integer = authority.IndexOf("]")
					Dim doquote As String = authority, dontquote As String = ""
					If [end] <> -1 AndAlso authority.IndexOf(":") <> -1 Then
						' the authority contains an IPv6 address
						If [end] = authority.length() Then
							dontquote = authority
							doquote = ""
						Else
							dontquote = authority.Substring(0, [end] + 1)
							doquote = authority.Substring([end] + 1)
						End If
					End If
					sb.append(dontquote)
					sb.append(quote(doquote, L_REG_NAME Or L_SERVER, H_REG_NAME Or H_SERVER))
				Else
					sb.append(quote(authority, L_REG_NAME Or L_SERVER, H_REG_NAME Or H_SERVER))
				End If
			End If
		End Sub

		Private Sub appendSchemeSpecificPart(ByVal sb As StringBuffer, ByVal opaquePart As String, ByVal authority As String, ByVal userInfo As String, ByVal host As String, ByVal port As Integer, ByVal path As String, ByVal query As String)
			If opaquePart IsNot Nothing Then
	'             check if SSP begins with an IPv6 address
	'             * because we must not quote a literal IPv6 address
	'             
				If opaquePart.StartsWith("//[") Then
					Dim [end] As Integer = opaquePart.IndexOf("]")
					If [end] <> -1 AndAlso opaquePart.IndexOf(":")<>-1 Then
						Dim doquote, dontquote As String
						If [end] = opaquePart.length() Then
							dontquote = opaquePart
							doquote = ""
						Else
							dontquote = opaquePart.Substring(0,[end]+1)
							doquote = opaquePart.Substring([end]+1)
						End If
						sb.append(dontquote)
						sb.append(quote(doquote, L_URIC, H_URIC))
					End If
				Else
					sb.append(quote(opaquePart, L_URIC, H_URIC))
				End If
			Else
				appendAuthority(sb, authority, userInfo, host, port)
				If path IsNot Nothing Then sb.append(quote(path, L_PATH, H_PATH))
				If query IsNot Nothing Then
					sb.append("?"c)
					sb.append(quote(query, L_URIC, H_URIC))
				End If
			End If
		End Sub

		Private Sub appendFragment(ByVal sb As StringBuffer, ByVal fragment As String)
			If fragment IsNot Nothing Then
				sb.append("#"c)
				sb.append(quote(fragment, L_URIC, H_URIC))
			End If
		End Sub

		Private Overrides Function ToString(ByVal scheme As String, ByVal opaquePart As String, ByVal authority As String, ByVal userInfo As String, ByVal host As String, ByVal port As Integer, ByVal path As String, ByVal query As String, ByVal fragment As String) As String
			Dim sb As New StringBuffer
			If scheme IsNot Nothing Then
				sb.append(scheme)
				sb.append(":"c)
			End If
			appendSchemeSpecificPart(sb, opaquePart, authority, userInfo, host, port, path, query)
			appendFragment(sb, fragment)
			Return sb.ToString()
		End Function

		Private Sub defineSchemeSpecificPart()
			If schemeSpecificPart IsNot Nothing Then Return
			Dim sb As New StringBuffer
			appendSchemeSpecificPart(sb, Nothing, authority, userInfo, host, port, path, query)
			If sb.length() = 0 Then Return
			schemeSpecificPart = sb.ToString()
		End Sub

		Private Sub defineString()
			If string_Renamed IsNot Nothing Then Return

			Dim sb As New StringBuffer
			If scheme IsNot Nothing Then
				sb.append(scheme)
				sb.append(":"c)
			End If
			If opaque Then
				sb.append(schemeSpecificPart)
			Else
				If host IsNot Nothing Then
					sb.append("//")
					If userInfo IsNot Nothing Then
						sb.append(userInfo)
						sb.append("@"c)
					End If
					Dim needBrackets As Boolean = ((host.IndexOf(":"c) >= 0) AndAlso (Not host.StartsWith("[")) AndAlso (Not host.EndsWith("]")))
					If needBrackets Then sb.append("["c)
					sb.append(host)
					If needBrackets Then sb.append("]"c)
					If port <> -1 Then
						sb.append(":"c)
						sb.append(port)
					End If
				ElseIf authority IsNot Nothing Then
					sb.append("//")
					sb.append(authority)
				End If
				If path IsNot Nothing Then sb.append(path)
				If query IsNot Nothing Then
					sb.append("?"c)
					sb.append(query)
				End If
			End If
			If fragment IsNot Nothing Then
				sb.append("#"c)
				sb.append(fragment)
			End If
			string_Renamed = sb.ToString()
		End Sub


		' -- Normalization, resolution, and relativization --

		' RFC2396 5.2 (6)
		Private Shared Function resolvePath(ByVal base As String, ByVal child As String, ByVal absolute As Boolean) As String
			Dim i As Integer = base.LastIndexOf("/"c)
			Dim cn As Integer = child.length()
			Dim path_Renamed As String = ""

			If cn = 0 Then
				' 5.2 (6a)
				If i >= 0 Then path_Renamed = base.Substring(0, i + 1)
			Else
				Dim sb As New StringBuffer(base.length() + cn)
				' 5.2 (6a)
				If i >= 0 Then sb.append(base.Substring(0, i + 1))
				' 5.2 (6b)
				sb.append(child)
				path_Renamed = sb.ToString()
			End If

			' 5.2 (6c-f)
			Dim np As String = normalize(path_Renamed)

			' 5.2 (6g): If the result is absolute but the path begins with "../",
			' then we simply leave the path as-is

			Return np
		End Function

		' RFC2396 5.2
		Private Shared Function resolve(ByVal base As URI, ByVal child As URI) As URI
			' check if child if opaque first so that NPE is thrown
			' if child is null.
			If child.opaque OrElse base.opaque Then Return child

			' 5.2 (2): Reference to current document (lone fragment)
			If (child.scheme Is Nothing) AndAlso (child.authority Is Nothing) AndAlso child.path.Equals("") AndAlso (child.fragment IsNot Nothing) AndAlso (child.query Is Nothing) Then
				If (base.fragment IsNot Nothing) AndAlso child.fragment.Equals(base.fragment) Then Return base
				Dim ru As New URI
				ru.scheme = base.scheme
				ru.authority = base.authority
				ru.userInfo = base.userInfo
				ru.host = base.host
				ru.port = base.port
				ru.path = base.path
				ru.fragment = child.fragment
				ru.query = base.query
				Return ru
			End If

			' 5.2 (3): Child is absolute
			If child.scheme IsNot Nothing Then Return child

			Dim ru As New URI ' Resolved URI
			ru.scheme = base.scheme
			ru.query = child.query
			ru.fragment = child.fragment

			' 5.2 (4): Authority
			If child.authority Is Nothing Then
				ru.authority = base.authority
				ru.host = base.host
				ru.userInfo = base.userInfo
				ru.port = base.port

				Dim cp As String = If(child.path Is Nothing, "", child.path)
				If (cp.length() > 0) AndAlso (cp.Chars(0) = "/"c) Then
					' 5.2 (5): Child path is absolute
					ru.path = child.path
				Else
					' 5.2 (6): Resolve relative path
					ru.path = resolvePath(base.path, cp, base.absolute)
				End If
			Else
				ru.authority = child.authority
				ru.host = child.host
				ru.userInfo = child.userInfo
				ru.host = child.host
				ru.port = child.port
				ru.path = child.path
			End If

			' 5.2 (7): Recombine (nothing to do here)
			Return ru
		End Function

		' If the given URI's path is normal then return the URI;
		' o.w., return a new URI containing the normalized path.
		'
		Private Shared Function normalize(ByVal u As URI) As URI
			If u.opaque OrElse (u.path Is Nothing) OrElse (u.path.length() = 0) Then Return u

			Dim np As String = normalize(u.path)
			If np = u.path Then Return u

			Dim v As New URI
			v.scheme = u.scheme
			v.fragment = u.fragment
			v.authority = u.authority
			v.userInfo = u.userInfo
			v.host = u.host
			v.port = u.port
			v.path = np
			v.query = u.query
			Return v
		End Function

		' If both URIs are hierarchical, their scheme and authority components are
		' identical, and the base path is a prefix of the child's path, then
		' return a relative URI that, when resolved against the base, yields the
		' child; otherwise, return the child.
		'
		Private Shared Function relativize(ByVal base As URI, ByVal child As URI) As URI
			' check if child if opaque first so that NPE is thrown
			' if child is null.
			If child.opaque OrElse base.opaque Then Return child
			If (Not equalIgnoringCase(base.scheme, child.scheme)) OrElse (Not equal(base.authority, child.authority)) Then Return child

			Dim bp As String = normalize(base.path)
			Dim cp As String = normalize(child.path)
			If Not bp.Equals(cp) Then
				If Not bp.EndsWith("/") Then bp = bp & "/"
				If Not cp.StartsWith(bp) Then Return child
			End If

			Dim v As New URI
			v.path = cp.Substring(bp.length())
			v.query = child.query
			v.fragment = child.fragment
			Return v
		End Function



		' -- Path normalization --

		' The following algorithm for path normalization avoids the creation of a
		' string object for each segment, as well as the use of a string buffer to
		' compute the final result, by using a single char array and editing it in
		' place.  The array is first split into segments, replacing each slash
		' with '\0' and creating a segment-index array, each element of which is
		' the index of the first char in the corresponding segment.  We then walk
		' through both arrays, removing ".", "..", and other segments as necessary
		' by setting their entries in the index array to -1.  Finally, the two
		' arrays are used to rejoin the segments and compute the final result.
		'
		' This code is based upon src/solaris/native/java/io/canonicalize_md.c


		' Check the given path to see if it might need normalization.  A path
		' might need normalization if it contains duplicate slashes, a "."
		' segment, or a ".." segment.  Return -1 if no further normalization is
		' possible, otherwise return the number of segments found.
		'
		' This method takes a string argument rather than a char array so that
		' this test can be performed without invoking path.toCharArray().
		'
		Private Shared Function needsNormalization(ByVal path As String) As Integer
			Dim normal As Boolean = True
			Dim ns As Integer = 0 ' Number of segments
			Dim [end] As Integer = path.length() - 1 ' Index of last char in path
			Dim p As Integer = 0 ' Index of next char in path

			' Skip initial slashes
			Do While p <= [end]
				If path.Chars(p) <> "/"c Then Exit Do
				p += 1
			Loop
			If p > 1 Then normal = False

			' Scan segments
			Do While p <= [end]

				' Looking at "." or ".." ?
				If (path.Chars(p) = "."c) AndAlso ((p = [end]) OrElse ((path.Chars(p + 1) = "/"c) OrElse ((path.Chars(p + 1) = "."c) AndAlso ((p + 1 = [end]) OrElse (path.Chars(p + 2) = "/"c))))) Then normal = False
				ns += 1

				' Find beginning of next segment
				Do While p <= [end]
					Dim tempVar As Boolean = path.Chars(p) <> "/"c
					p += 1
					If tempVar Then Continue Do

					' Skip redundant slashes
					Do While p <= [end]
						If path.Chars(p) <> "/"c Then Exit Do
						normal = False
						p += 1
					Loop

					Exit Do
				Loop
			Loop

			Return If(normal, -1, ns)
		End Function


		' Split the given path into segments, replacing slashes with nulls and
		' filling in the given segment-index array.
		'
		' Preconditions:
		'   segs.length == Number of segments in path
		'
		' Postconditions:
		'   All slashes in path replaced by '\0'
		'   segs[i] == Index of first char in segment i (0 <= i < segs.length)
		'
		Private Shared Sub split(ByVal path As Char(), ByVal segs As Integer())
			Dim [end] As Integer = path.Length - 1 ' Index of last char in path
			Dim p As Integer = 0 ' Index of next char in path
			Dim i As Integer = 0 ' Index of current segment

			' Skip initial slashes
			Do While p <= [end]
				If path(p) <> "/"c Then Exit Do
				path(p) = ControlChars.NullChar
				p += 1
			Loop

			Do While p <= [end]

				' Note start of segment
				segs(i) = p
				p += 1
				i += 1

				' Find beginning of next segment
				Do While p <= [end]
					Dim tempVar As Boolean = path(p) <> "/"c
					p += 1
					If tempVar Then Continue Do
					path(p - 1) = ControlChars.NullChar

					' Skip redundant slashes
					Do While p <= [end]
						If path(p) <> "/"c Then Exit Do
						path(p) = ControlChars.NullChar
						p += 1
					Loop
					Exit Do
				Loop
			Loop

			If i <> segs.Length Then Throw New InternalError ' ASSERT
		End Sub


		' Join the segments in the given path according to the given segment-index
		' array, ignoring those segments whose index entries have been set to -1,
		' and inserting slashes as needed.  Return the length of the resulting
		' path.
		'
		' Preconditions:
		'   segs[i] == -1 implies segment i is to be ignored
		'   path computed by split, as above, with '\0' having replaced '/'
		'
		' Postconditions:
		'   path[0] .. path[return value] == Resulting path
		'
		Private Shared Function join(ByVal path As Char(), ByVal segs As Integer()) As Integer
			Dim ns As Integer = segs.Length ' Number of segments
			Dim [end] As Integer = path.Length - 1 ' Index of last char in path
			Dim p As Integer = 0 ' Index of next path char to write

			If path(p) = ControlChars.NullChar Then
				' Restore initial slash for absolute paths
				path(p) = "/"c
				p += 1
			End If

			For i As Integer = 0 To ns - 1
				Dim q As Integer = segs(i) ' Current segment
				If q = -1 Then Continue For

				If p = q Then
					' We're already at this segment, so just skip to its end
					Do While (p <= [end]) AndAlso (path(p) <> ControlChars.NullChar)
						p += 1
					Loop
					If p <= [end] Then
						' Preserve trailing slash
						path(p) = "/"c
						p += 1
					End If
				ElseIf p < q Then
					' Copy q down to p
					Do While (q <= [end]) AndAlso (path(q) <> ControlChars.NullChar)
						path(p) = path(q)
						q += 1
						p += 1
					Loop
					If q <= [end] Then
						' Preserve trailing slash
						path(p) = "/"c
						p += 1
					End If
				Else
					Throw New InternalError ' ASSERT false
				End If
			Next i

			Return p
		End Function


		' Remove "." segments from the given path, and remove segment pairs
		' consisting of a non-".." segment followed by a ".." segment.
		'
		Private Shared Sub removeDots(ByVal path As Char(), ByVal segs As Integer())
			Dim ns As Integer = segs.Length
			Dim [end] As Integer = path.Length - 1

			For i As Integer = 0 To ns - 1
				Dim dots As Integer = 0 ' Number of dots found (0, 1, or 2)

				' Find next occurrence of "." or ".."
				Do
					Dim p As Integer = segs(i)
					If path(p) = "."c Then
						If p = [end] Then
							dots = 1
							Exit Do
						ElseIf path(p + 1) = ControlChars.NullChar Then
							dots = 1
							Exit Do
						ElseIf (path(p + 1) = "."c) AndAlso ((p + 1 = [end]) OrElse (path(p + 2) = ControlChars.NullChar)) Then
							dots = 2
							Exit Do
						End If
					End If
					i += 1
				Loop While i < ns
				If (i > ns) OrElse (dots = 0) Then Exit For

				If dots = 1 Then
					' Remove this occurrence of "."
					segs(i) = -1
				Else
					' If there is a preceding non-".." segment, remove both that
					' segment and this occurrence of ".."; otherwise, leave this
					' ".." segment as-is.
					Dim j As Integer
					For j = i - 1 To 0 Step -1
						If segs(j) <> -1 Then Exit For
					Next j
					If j >= 0 Then
						Dim q As Integer = segs(j)
						If Not((path(q) = "."c) AndAlso (path(q + 1) = "."c) AndAlso (path(q + 2) = ControlChars.NullChar)) Then
							segs(i) = -1
							segs(j) = -1
						End If
					End If
				End If
			Next i
		End Sub


		' DEVIATION: If the normalized path is relative, and if the first
		' segment could be parsed as a scheme name, then prepend a "." segment
		'
		Private Shared Sub maybeAddLeadingDot(ByVal path As Char(), ByVal segs As Integer())

			If path(0) = ControlChars.NullChar Then Return

			Dim ns As Integer = segs.Length
			Dim f As Integer = 0 ' Index of first segment
			Do While f < ns
				If segs(f) >= 0 Then Exit Do
				f += 1
			Loop
			If (f >= ns) OrElse (f = 0) Then Return

			Dim p As Integer = segs(f)
			Do While (p < path.Length) AndAlso (path(p) <> ":"c) AndAlso (path(p) <> ControlChars.NullChar)
				p += 1
			Loop
			If p >= path.Length OrElse path(p) = ControlChars.NullChar Then Return

			' At this point we know that the first segment is unused,
			' hence we can insert a "." segment at that position
			path(0) = "."c
			path(1) = ControlChars.NullChar
			segs(0) = 0
		End Sub


		' Normalize the given path string.  A normal path string has no empty
		' segments (i.e., occurrences of "//"), no segments equal to ".", and no
		' segments equal to ".." that are preceded by a segment not equal to "..".
		' In contrast to Unix-style pathname normalization, for URI paths we
		' always retain trailing slashes.
		'
		Private Shared Function normalize(ByVal ps As String) As String

			' Does this path need normalization?
			Dim ns As Integer = needsNormalization(ps) ' Number of segments
			If ns < 0 Then Return ps

			Dim path_Renamed As Char() = ps.ToCharArray() ' Path in char-array form

			' Split path into segments
			Dim segs As Integer() = New Integer(ns - 1){} ' Segment-index array
			Split(path_Renamed, segs)

			' Remove dots
			removeDots(path_Renamed, segs)

			' Prevent scheme-name confusion
			maybeAddLeadingDot(path_Renamed, segs)

			' Join the remaining segments and return the result
			Dim s As New String(path_Renamed, 0, join(path_Renamed, segs))
			If s.Equals(ps) Then Return ps
			Return s
		End Function



		' -- Character classes for parsing --

		' RFC2396 precisely specifies which characters in the US-ASCII charset are
		' permissible in the various components of a URI reference.  We here
		' define a set of mask pairs to aid in enforcing these restrictions.  Each
		' mask pair consists of two longs, a low mask and a high mask.  Taken
		' together they represent a 128-bit mask, where bit i is set iff the
		' character with value i is permitted.
		'
		' This approach is more efficient than sequentially searching arrays of
		' permitted characters.  It could be made still more efficient by
		' precompiling the mask information so that a character's presence in a
		' given mask could be determined by a single table lookup.

		' Compute the low-order mask for the characters in the given string
		Private Shared Function lowMask(ByVal chars As String) As Long
			Dim n As Integer = chars.length()
			Dim m As Long = 0
			For i As Integer = 0 To n - 1
				Dim c As Char = chars.Chars(i)
				If AscW(c) < 64 Then m = m Or (1L << AscW(c))
			Next i
			Return m
		End Function

		' Compute the high-order mask for the characters in the given string
		Private Shared Function highMask(ByVal chars As String) As Long
			Dim n As Integer = chars.length()
			Dim m As Long = 0
			For i As Integer = 0 To n - 1
				Dim c As Char = chars.Chars(i)
				If (c >= 64) AndAlso (AscW(c) < 128) Then m = m Or (1L << (AscW(c) - 64))
			Next i
			Return m
		End Function

		' Compute a low-order mask for the characters
		' between first and last, inclusive
		Private Shared Function lowMask(ByVal first As Char, ByVal last As Char) As Long
			Dim m As Long = 0
			Dim f As Integer = Math.Max(Math.Min(first, 63), 0)
			Dim l As Integer = Math.Max(Math.Min(last, 63), 0)
			For i As Integer = f To l
				m = m Or 1L << i
			Next i
			Return m
		End Function

		' Compute a high-order mask for the characters
		' between first and last, inclusive
		Private Shared Function highMask(ByVal first As Char, ByVal last As Char) As Long
			Dim m As Long = 0
			Dim f As Integer = Math.Max(Math.Min(first, 127), 64) - 64
			Dim l As Integer = Math.Max(Math.Min(last, 127), 64) - 64
			For i As Integer = f To l
				m = m Or 1L << i
			Next i
			Return m
		End Function

		' Tell whether the given character is permitted by the given mask pair
		Private Shared Function match(ByVal c As Char, ByVal lowMask As Long, ByVal highMask As Long) As Boolean
			If AscW(c) = 0 Then ' 0 doesn't have a slot in the mask. So, it never matches. Return False
			If AscW(c) < 64 Then Return ((1L << AscW(c)) And lowMask) <> 0
			If AscW(c) < 128 Then Return ((1L << (AscW(c) - 64)) And highMask) <> 0
			Return False
		End Function

		' Character-class masks, in reverse order from RFC2396 because
		' initializers for static fields cannot make forward references.

		' digit    = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" |
		'            "8" | "9"
		Private Shared ReadOnly L_DIGIT As Long = lowMask("0"c, "9"c)
		Private Const H_DIGIT As Long = 0L

		' upalpha  = "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" |
		'            "J" | "K" | "L" | "M" | "N" | "O" | "P" | "Q" | "R" |
		'            "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z"
		Private Const L_UPALPHA As Long = 0L
		Private Shared ReadOnly H_UPALPHA As Long = highMask("A"c, "Z"c)

		' lowalpha = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" |
		'            "j" | "k" | "l" | "m" | "n" | "o" | "p" | "q" | "r" |
		'            "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z"
		Private Const L_LOWALPHA As Long = 0L
		Private Shared ReadOnly H_LOWALPHA As Long = highMask("a"c, "z"c)

		' alpha         = lowalpha | upalpha
		Private Shared ReadOnly L_ALPHA As Long = L_LOWALPHA Or L_UPALPHA
		Private Shared ReadOnly H_ALPHA As Long = H_LOWALPHA Or H_UPALPHA

		' alphanum      = alpha | digit
		Private Shared ReadOnly L_ALPHANUM As Long = L_DIGIT Or L_ALPHA
		Private Shared ReadOnly H_ALPHANUM As Long = H_DIGIT Or H_ALPHA

		' hex           = digit | "A" | "B" | "C" | "D" | "E" | "F" |
		'                         "a" | "b" | "c" | "d" | "e" | "f"
		Private Shared ReadOnly L_HEX As Long = L_DIGIT
		Private Shared ReadOnly H_HEX As Long = highMask("A"c, "F"c) Or highMask("a"c, "f"c)

		' mark          = "-" | "_" | "." | "!" | "~" | "*" | "'" |
		'                 "(" | ")"
		Private Shared ReadOnly L_MARK As Long = lowMask("-_.!~*'()")
		Private Shared ReadOnly H_MARK As Long = highMask("-_.!~*'()")

		' unreserved    = alphanum | mark
		Private Shared ReadOnly L_UNRESERVED As Long = L_ALPHANUM Or L_MARK
		Private Shared ReadOnly H_UNRESERVED As Long = H_ALPHANUM Or H_MARK

		' reserved      = ";" | "/" | "?" | ":" | "@" | "&" | "=" | "+" |
		'                 "$" | "," | "[" | "]"
		' Added per RFC2732: "[", "]"
		Private Shared ReadOnly L_RESERVED As Long = lowMask(";/?:@&=+$,[]")
		Private Shared ReadOnly H_RESERVED As Long = highMask(";/?:@&=+$,[]")

		' The zero'th bit is used to indicate that escape pairs and non-US-ASCII
		' characters are allowed; this is handled by the scanEscape method below.
		Private Const L_ESCAPED As Long = 1L
		Private Const H_ESCAPED As Long = 0L

		' uric          = reserved | unreserved | escaped
		Private Shared ReadOnly L_URIC As Long = L_RESERVED Or L_UNRESERVED Or L_ESCAPED
		Private Shared ReadOnly H_URIC As Long = H_RESERVED Or H_UNRESERVED Or H_ESCAPED

		' pchar         = unreserved | escaped |
		'                 ":" | "@" | "&" | "=" | "+" | "$" | ","
		Private Shared ReadOnly L_PCHAR As Long = L_UNRESERVED Or L_ESCAPED Or lowMask(":@&=+$,")
		Private Shared ReadOnly H_PCHAR As Long = H_UNRESERVED Or H_ESCAPED Or highMask(":@&=+$,")

		' All valid path characters
		Private Shared ReadOnly L_PATH As Long = L_PCHAR Or lowMask(";/")
		Private Shared ReadOnly H_PATH As Long = H_PCHAR Or highMask(";/")

		' Dash, for use in domainlabel and toplabel
		Private Shared ReadOnly L_DASH As Long = lowMask("-")
		Private Shared ReadOnly H_DASH As Long = highMask("-")

		' Dot, for use in hostnames
		Private Shared ReadOnly L_DOT As Long = lowMask(".")
		Private Shared ReadOnly H_DOT As Long = highMask(".")

		' userinfo      = *( unreserved | escaped |
		'                    ";" | ":" | "&" | "=" | "+" | "$" | "," )
		Private Shared ReadOnly L_USERINFO As Long = L_UNRESERVED Or L_ESCAPED Or lowMask(";:&=+$,")
		Private Shared ReadOnly H_USERINFO As Long = H_UNRESERVED Or H_ESCAPED Or highMask(";:&=+$,")

		' reg_name      = 1*( unreserved | escaped | "$" | "," |
		'                     ";" | ":" | "@" | "&" | "=" | "+" )
		Private Shared ReadOnly L_REG_NAME As Long = L_UNRESERVED Or L_ESCAPED Or lowMask("$,;:@&=+")
		Private Shared ReadOnly H_REG_NAME As Long = H_UNRESERVED Or H_ESCAPED Or highMask("$,;:@&=+")

		' All valid characters for server-based authorities
		Private Shared ReadOnly L_SERVER As Long = L_USERINFO Or L_ALPHANUM Or L_DASH Or lowMask(".:@[]")
		Private Shared ReadOnly H_SERVER As Long = H_USERINFO Or H_ALPHANUM Or H_DASH Or highMask(".:@[]")

		' Special case of server authority that represents an IPv6 address
		' In this case, a % does not signify an escape sequence
		Private Shared ReadOnly L_SERVER_PERCENT As Long = L_SERVER Or lowMask("%")
		Private Shared ReadOnly H_SERVER_PERCENT As Long = H_SERVER Or highMask("%")
		Private Shared ReadOnly L_LEFT_BRACKET As Long = lowMask("[")
		Private Shared ReadOnly H_LEFT_BRACKET As Long = highMask("[")

		' scheme        = alpha *( alpha | digit | "+" | "-" | "." )
		Private Shared ReadOnly L_SCHEME As Long = L_ALPHA Or L_DIGIT Or lowMask("+-.")
		Private Shared ReadOnly H_SCHEME As Long = H_ALPHA Or H_DIGIT Or highMask("+-.")

		' uric_no_slash = unreserved | escaped | ";" | "?" | ":" | "@" |
		'                 "&" | "=" | "+" | "$" | ","
		Private Shared ReadOnly L_URIC_NO_SLASH As Long = L_UNRESERVED Or L_ESCAPED Or lowMask(";?:@&=+$,")
		Private Shared ReadOnly H_URIC_NO_SLASH As Long = H_UNRESERVED Or H_ESCAPED Or highMask(";?:@&=+$,")


		' -- Escaping and encoding --

		Private Shared ReadOnly hexDigits As Char() = { "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c }

		Private Shared Sub appendEscape(ByVal sb As StringBuffer, ByVal b As SByte)
			sb.append("%"c)
			sb.append(hexDigits((b >> 4) And &Hf))
			sb.append(hexDigits((b >> 0) And &Hf))
		End Sub

		Private Shared Sub appendEncoded(ByVal sb As StringBuffer, ByVal c As Char)
			Dim bb As java.nio.ByteBuffer = Nothing
			Try
				bb = sun.nio.cs.ThreadLocalCoders.encoderFor("UTF-8").encode(java.nio.CharBuffer.wrap("" & AscW(c)))
			Catch x As java.nio.charset.CharacterCodingException
				Debug.Assert(False)
			End Try
			Do While bb.hasRemaining()
				Dim b As Integer = bb.get() And &Hff
				If b >= &H80 Then
					appendEscape(sb, CByte(b))
				Else
					sb.append(ChrW(b))
				End If
			Loop
		End Sub

		' Quote any characters in s that are not permitted
		' by the given mask pair
		'
		Private Shared Function quote(ByVal s As String, ByVal lowMask As Long, ByVal highMask As Long) As String
			Dim n As Integer = s.length()
			Dim sb As StringBuffer = Nothing
			Dim allowNonASCII As Boolean = ((lowMask And L_ESCAPED) <> 0)
			For i As Integer = 0 To s.length() - 1
				Dim c As Char = s.Chars(i)
				If c < ChrW(&H0080) Then
					If Not match(c, lowMask, highMask) Then
						If sb Is Nothing Then
							sb = New StringBuffer
							sb.append(s.Substring(0, i))
						End If
						appendEscape(sb, AscW(c))
					Else
						If sb IsNot Nothing Then sb.append(c)
					End If
				ElseIf allowNonASCII AndAlso (Character.isSpaceChar(c) OrElse Char.IsControl(c)) Then
					If sb Is Nothing Then
						sb = New StringBuffer
						sb.append(s.Substring(0, i))
					End If
					appendEncoded(sb, c)
				Else
					If sb IsNot Nothing Then sb.append(c)
				End If
			Next i
			Return If(sb Is Nothing, s, sb.ToString())
		End Function

		' Encodes all characters >= \u0080 into escaped, normalized UTF-8 octets,
		' assuming that s is otherwise legal
		'
		Private Shared Function encode(ByVal s As String) As String
			Dim n As Integer = s.length()
			If n = 0 Then Return s

			' First check whether we actually need to encode
			Dim i As Integer = 0
			Do
				If s.Chars(i) >= ChrW(&H0080) Then Exit Do
				i += 1
				If i >= n Then Return s
			Loop

			Dim ns As String = java.text.Normalizer.normalize(s, java.text.Normalizer.Form.NFC)
			Dim bb As java.nio.ByteBuffer = Nothing
			Try
				bb = sun.nio.cs.ThreadLocalCoders.encoderFor("UTF-8").encode(java.nio.CharBuffer.wrap(ns))
			Catch x As java.nio.charset.CharacterCodingException
				Debug.Assert(False)
			End Try

			Dim sb As New StringBuffer
			Do While bb.hasRemaining()
				Dim b As Integer = bb.get() And &Hff
				If b >= &H80 Then
					appendEscape(sb, CByte(b))
				Else
					sb.append(ChrW(b))
				End If
			Loop
			Return sb.ToString()
		End Function

		Private Shared Function decode(ByVal c As Char) As Integer
			If (c >= "0"c) AndAlso (c <= "9"c) Then Return AscW(c) - AscW("0"c)
			If (c >= "a"c) AndAlso (c <= "f"c) Then Return AscW(c) - AscW("a"c) + 10
			If (c >= "A"c) AndAlso (c <= "F"c) Then Return AscW(c) - AscW("A"c) + 10
			Debug.Assert(False)
			Return -1
		End Function

		Private Shared Function decode(ByVal c1 As Char, ByVal c2 As Char) As SByte
			Return CByte(((decode(c1) And &Hf) << 4) Or ((decode(c2) And &Hf) << 0))
		End Function

		' Evaluates all escapes in s, applying UTF-8 decoding if needed.  Assumes
		' that escapes are well-formed syntactically, i.e., of the form %XX.  If a
		' sequence of escaped octets is not valid UTF-8 then the erroneous octets
		' are replaced with '\uFFFD'.
		' Exception: any "%" found between "[]" is left alone. It is an IPv6 literal
		'            with a scope_id
		'
		Private Shared Function decode(ByVal s As String) As String
			If s Is Nothing Then Return s
			Dim n As Integer = s.length()
			If n = 0 Then Return s
			If s.IndexOf("%"c) < 0 Then Return s

			Dim sb As New StringBuffer(n)
			Dim bb As java.nio.ByteBuffer = java.nio.ByteBuffer.allocate(n)
			Dim cb As java.nio.CharBuffer = java.nio.CharBuffer.allocate(n)
			Dim dec As java.nio.charset.CharsetDecoder = sun.nio.cs.ThreadLocalCoders.decoderFor("UTF-8").onMalformedInput(java.nio.charset.CodingErrorAction.REPLACE).onUnmappableCharacter(java.nio.charset.CodingErrorAction.REPLACE)

			' This is not horribly efficient, but it will do for now
			Dim c As Char = s.Chars(0)
			Dim betweenBrackets As Boolean = False

			Dim i As Integer = 0
			Do While i < n
				Debug.Assert(c = s.Chars(i)) ' Loop invariant
				If c = "["c Then
					betweenBrackets = True
				ElseIf betweenBrackets AndAlso c = "]"c Then
					betweenBrackets = False
				End If
				If c <> "%"c OrElse betweenBrackets Then
					sb.append(c)
					i += 1
					If i >= n Then Exit Do
					c = s.Chars(i)
					Continue Do
				End If
				bb.clear()
				Dim ui As Integer = i
				Do
					assert(n - i >= 2)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					bb.put(decode(s.Chars(++i), s.Chars(++i)))
					i += 1
					If i >= n Then Exit Do
					c = s.Chars(i)
					If c <> "%"c Then Exit Do
				Loop
				bb.flip()
				cb.clear()
				dec.reset()
				Dim cr As java.nio.charset.CoderResult = dec.decode(bb, cb, True)
				Debug.Assert(cr.underflow)
				cr = dec.flush(cb)
				Debug.Assert(cr.underflow)
				sb.append(cb.flip().ToString())
			Loop

			Return sb.ToString()
		End Function


		' -- Parsing --

		' For convenience we wrap the input URI string in a new instance of the
		' following internal class.  This saves always having to pass the input
		' string as an argument to each internal scan/parse method.

		Private Class Parser
			Private ReadOnly outerInstance As URI


			Private input As String ' URI input string
			Private requireServerAuthority As Boolean = False

			Friend Sub New(ByVal outerInstance As URI, ByVal s As String)
					Me.outerInstance = outerInstance
				input = s
				outerInstance.string_Renamed = s
			End Sub

			' -- Methods for throwing URISyntaxException in various ways --

			Private Sub fail(ByVal reason As String)
				Throw New URISyntaxException(input, reason)
			End Sub

			Private Sub fail(ByVal reason As String, ByVal p As Integer)
				Throw New URISyntaxException(input, reason, p)
			End Sub

			Private Sub failExpecting(ByVal expected As String, ByVal p As Integer)
				fail("Expected " & expected, p)
			End Sub

			Private Sub failExpecting(ByVal expected As String, ByVal prior As String, ByVal p As Integer)
				fail("Expected " & expected & " following " & prior, p)
			End Sub


			' -- Simple access to the input string --

			' Return a substring of the input string
			'
			Private Function substring(ByVal start As Integer, ByVal [end] As Integer) As String
				Return input.Substring(start, [end] - start)
			End Function

			' Return the char at position p,
			' assuming that p < input.length()
			'
			Private Function charAt(ByVal p As Integer) As Char
				Return input.Chars(p)
			End Function

			' Tells whether start < end and, if so, whether charAt(start) == c
			'
			Private Function at(ByVal start As Integer, ByVal [end] As Integer, ByVal c As Char) As Boolean
				Return (start < [end]) AndAlso (charAt(start) = c)
			End Function

			' Tells whether start + s.length() < end and, if so,
			' whether the chars at the start position match s exactly
			'
			Private Function at(ByVal start As Integer, ByVal [end] As Integer, ByVal s As String) As Boolean
				Dim p As Integer = start
				Dim sn As Integer = s.length()
				If sn > [end] - p Then Return False
				Dim i As Integer = 0
				Do While i < sn
					Dim tempVar As Boolean = charAt(p) <> s.Chars(i)
					p += 1
					If tempVar Then Exit Do
					i += 1
				Loop
				Return (i = sn)
			End Function


			' -- Scanning --

			' The various scan and parse methods that follow use a uniform
			' convention of taking the current start position and end index as
			' their first two arguments.  The start is inclusive while the end is
			' exclusive, just as in the String [Class], i.e., a start/end pair
			' denotes the left-open interval [start, end) of the input string.
			'
			' These methods never proceed past the end position.  They may return
			' -1 to indicate outright failure, but more often they simply return
			' the position of the first char after the last char scanned.  Thus
			' a typical idiom is
			'
			'     int p = start;
			'     int q = scan(p, end, ...);
			'     if (q > p)
			'         // We scanned something
			'         ...;
			'     else if (q == p)
			'         // We scanned nothing
			'         ...;
			'     else if (q == -1)
			'         // Something went wrong
			'         ...;


			' Scan a specific char: If the char at the given start position is
			' equal to c, return the index of the next char; otherwise, return the
			' start position.
			'
			Private Function scan(ByVal start As Integer, ByVal [end] As Integer, ByVal c As Char) As Integer
				If (start < [end]) AndAlso (charAt(start) = c) Then Return start + 1
				Return start
			End Function

			' Scan forward from the given start position.  Stop at the first char
			' in the err string (in which case -1 is returned), or the first char
			' in the stop string (in which case the index of the preceding char is
			' returned), or the end of the input string (in which case the length
			' of the input string is returned).  May return the start position if
			' nothing matches.
			'
			Private Function scan(ByVal start As Integer, ByVal [end] As Integer, ByVal err As String, ByVal [stop] As String) As Integer
				Dim p As Integer = start
				Do While p < [end]
					Dim c As Char = charAt(p)
					If err.IndexOf(c) >= 0 Then Return -1
					If [stop].IndexOf(c) >= 0 Then Exit Do
					p += 1
				Loop
				Return p
			End Function

			' Scan a potential escape sequence, starting at the given position,
			' with the given first char (i.e., charAt(start) == c).
			'
			' This method assumes that if escapes are allowed then visible
			' non-US-ASCII chars are also allowed.
			'
			Private Function scanEscape(ByVal start As Integer, ByVal n As Integer, ByVal first As Char) As Integer
				Dim p As Integer = start
				Dim c As Char = first
				If c = "%"c Then
					' Process escape pair
					If (p + 3 <= n) AndAlso match(charAt(p + 1), L_HEX, H_HEX) AndAlso match(charAt(p + 2), L_HEX, H_HEX) Then Return p + 3
					fail("Malformed escape pair", p)
				ElseIf (AscW(c) > 128) AndAlso (Not Character.isSpaceChar(c)) AndAlso (Not Char.IsControl(c)) Then
					' Allow unescaped but visible non-US-ASCII chars
					Return p + 1
				End If
				Return p
			End Function

			' Scan chars that match the given mask pair
			'
			Private Function scan(ByVal start As Integer, ByVal n As Integer, ByVal lowMask As Long, ByVal highMask As Long) As Integer
				Dim p As Integer = start
				Do While p < n
					Dim c As Char = charAt(p)
					If match(c, lowMask, highMask) Then
						p += 1
						Continue Do
					End If
					If (lowMask And L_ESCAPED) <> 0 Then
						Dim q As Integer = scanEscape(p, n, c)
						If q > p Then
							p = q
							Continue Do
						End If
					End If
					Exit Do
				Loop
				Return p
			End Function

			' Check that each of the chars in [start, end) matches the given mask
			'
			Private Sub checkChars(ByVal start As Integer, ByVal [end] As Integer, ByVal lowMask As Long, ByVal highMask As Long, ByVal what As String)
				Dim p As Integer = scan(start, [end], lowMask, highMask)
				If p < [end] Then fail("Illegal character in " & what, p)
			End Sub

			' Check that the char at position p matches the given mask
			'
			Private Sub checkChar(ByVal p As Integer, ByVal lowMask As Long, ByVal highMask As Long, ByVal what As String)
				checkChars(p, p + 1, lowMask, highMask, what)
			End Sub


			' -- Parsing --

			' [<scheme>:]<scheme-specific-part>[#<fragment>]
			'
			Friend Overridable Sub parse(ByVal rsa As Boolean)
				requireServerAuthority = rsa
				Dim ssp As Integer ' Start of scheme-specific part
				Dim n As Integer = input.length()
				Dim p As Integer = scan(0, n, "/?#", ":")
				If (p >= 0) AndAlso at(p, n, ":"c) Then
					If p = 0 Then failExpecting("scheme name", 0)
					checkChar(0, L_ALPHA, H_ALPHA, "scheme name")
					checkChars(1, p, L_SCHEME, H_SCHEME, "scheme name")
					outerInstance.scheme = Substring(0, p)
					p += 1 ' Skip ':'
					ssp = p
					If at(p, n, "/"c) Then
						p = parseHierarchical(p, n)
					Else
						Dim q As Integer = scan(p, n, "", "#")
						If q <= p Then failExpecting("scheme-specific part", p)
						checkChars(p, q, L_URIC, H_URIC, "opaque part")
						p = q
					End If
				Else
					ssp = 0
					p = parseHierarchical(0, n)
				End If
				outerInstance.schemeSpecificPart = Substring(ssp, p)
				If at(p, n, "#"c) Then
					checkChars(p + 1, n, L_URIC, H_URIC, "fragment")
					outerInstance.fragment = Substring(p + 1, n)
					p = n
				End If
				If p < n Then fail("end of URI", p)
			End Sub

			' [//authority]<path>[?<query>]
			'
			' DEVIATION from RFC2396: We allow an empty authority component as
			' long as it's followed by a non-empty path, query component, or
			' fragment component.  This is so that URIs such as "file:///foo/bar"
			' will parse.  This seems to be the intent of RFC2396, though the
			' grammar does not permit it.  If the authority is empty then the
			' userInfo, host, and port components are undefined.
			'
			' DEVIATION from RFC2396: We allow empty relative paths.  This seems
			' to be the intent of RFC2396, but the grammar does not permit it.
			' The primary consequence of this deviation is that "#f" parses as a
			' relative URI with an empty path.
			'
			Private Function parseHierarchical(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				If at(p, n, "/"c) AndAlso at(p + 1, n, "/"c) Then
					p += 2
					Dim q As Integer = scan(p, n, "", "/?#")
					If q > p Then
						p = parseAuthority(p, q)
					ElseIf q < n Then
						' DEVIATION: Allow empty authority prior to non-empty
						' path, query component or fragment identifier
					Else
						failExpecting("authority", p)
					End If
				End If
				Dim q As Integer = scan(p, n, "", "?#") ' DEVIATION: May be empty
				checkChars(p, q, L_PATH, H_PATH, "path")
				outerInstance.path = Substring(p, q)
				p = q
				If at(p, n, "?"c) Then
					p += 1
					q = scan(p, n, "", "#")
					checkChars(p, q, L_URIC, H_URIC, "query")
					outerInstance.query = Substring(p, q)
					p = q
				End If
				Return p
			End Function

			' authority     = server | reg_name
			'
			' Ambiguity: An authority that is a registry name rather than a server
			' might have a prefix that parses as a server.  We use the fact that
			' the authority component is always followed by '/' or the end of the
			' input string to resolve this: If the complete authority did not
			' parse as a server then we try to parse it as a registry name.
			'
			Private Function parseAuthority(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer = p
				Dim ex As URISyntaxException = Nothing

				Dim serverChars As Boolean
				Dim regChars As Boolean

				If scan(p, n, "", "]") > p Then
					' contains a literal IPv6 address, therefore % is allowed
					serverChars = (scan(p, n, L_SERVER_PERCENT, H_SERVER_PERCENT) = n)
				Else
					serverChars = (scan(p, n, L_SERVER, H_SERVER) = n)
				End If
				regChars = (scan(p, n, L_REG_NAME, H_REG_NAME) = n)

				If regChars AndAlso (Not serverChars) Then
					' Must be a registry-based authority
					outerInstance.authority = Substring(p, n)
					Return n
				End If

				If serverChars Then
					' Might be (probably is) a server-based authority, so attempt
					' to parse it as such.  If the attempt fails, try to treat it
					' as a registry-based authority.
					Try
						q = parseServer(p, n)
						If q < n Then failExpecting("end of authority", q)
						outerInstance.authority = Substring(p, n)
					Catch x As URISyntaxException
						' Undo results of failed parse
						outerInstance.userInfo = Nothing
						outerInstance.host = Nothing
						outerInstance.port = -1
						If requireServerAuthority Then
							' If we're insisting upon a server-based authority,
							' then just re-throw the exception
							Throw x
						Else
							' Save the exception in case it doesn't parse as a
							' registry either
							ex = x
							q = p
						End If
					End Try
				End If

				If q < n Then
					If regChars Then
						' Registry-based authority
						outerInstance.authority = Substring(p, n)
					ElseIf ex IsNot Nothing Then
						' Re-throw exception; it was probably due to
						' a malformed IPv6 address
						Throw ex
					Else
						fail("Illegal character in authority", q)
					End If
				End If

				Return n
			End Function


			' [<userinfo>@]<host>[:<port>]
			'
			Private Function parseServer(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer

				' userinfo
				q = scan(p, n, "/?#", "@")
				If (q >= p) AndAlso at(q, n, "@"c) Then
					checkChars(p, q, L_USERINFO, H_USERINFO, "user info")
					outerInstance.userInfo = Substring(p, q)
					p = q + 1 ' Skip '@'
				End If

				' hostname, IPv4 address, or IPv6 address
				If at(p, n, "["c) Then
					' DEVIATION from RFC2396: Support IPv6 addresses, per RFC2732
					p += 1
					q = scan(p, n, "/?#", "]")
					If (q > p) AndAlso at(q, n, "]"c) Then
						' look for a "%" scope id
						Dim r As Integer = scan(p, q, "", "%")
						If r > p Then
							parseIPv6Reference(p, r)
							If r+1 = q Then fail("scope id expected")
							checkChars(r+1, q, L_ALPHANUM, H_ALPHANUM, "scope id")
						Else
							parseIPv6Reference(p, q)
						End If
						outerInstance.host = Substring(p-1, q+1)
						p = q + 1
					Else
						failExpecting("closing bracket for IPv6 address", q)
					End If
				Else
					q = parseIPv4Address(p, n)
					If q <= p Then q = parseHostname(p, n)
					p = q
				End If

				' port
				If at(p, n, ":"c) Then
					p += 1
					q = scan(p, n, "", "/")
					If q > p Then
						checkChars(p, q, L_DIGIT, H_DIGIT, "port number")
						Try
							outerInstance.port = Convert.ToInt32(Substring(p, q))
						Catch x As NumberFormatException
							fail("Malformed port number", p)
						End Try
						p = q
					End If
				End If
				If p < n Then failExpecting("port number", p)

				Return p
			End Function

			' Scan a string of decimal digits whose value fits in a byte
			'
			Private Function scanByte(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer = scan(p, n, L_DIGIT, H_DIGIT)
				If q <= p Then Return q
				If Convert.ToInt32(Substring(p, q)) > 255 Then Return p
				Return q
			End Function

			' Scan an IPv4 address.
			'
			' If the strict argument is true then we require that the given
			' interval contain nothing besides an IPv4 address; if it is false
			' then we only require that it start with an IPv4 address.
			'
			' If the interval does not contain or start with (depending upon the
			' strict argument) a legal IPv4 address characters then we return -1
			' immediately; otherwise we insist that these characters parse as a
			' legal IPv4 address and throw an exception on failure.
			'
			' We assume that any string of decimal digits and dots must be an IPv4
			' address.  It won't parse as a hostname anyway, so making that
			' assumption here allows more meaningful exceptions to be thrown.
			'
			Private Function scanIPv4Address(ByVal start As Integer, ByVal n As Integer, ByVal [strict] As Boolean) As Integer
				Dim p As Integer = start
				Dim q As Integer
				Dim m As Integer = scan(p, n, L_DIGIT Or L_DOT, H_DIGIT Or H_DOT)
				If (m <= p) OrElse ([strict] AndAlso (m <> n)) Then Return -1
				Do
					' Per RFC2732: At most three digits per byte
					' Further constraint: Each element fits in a byte
					q = scanByte(p, m)
					If q <= p Then Exit Do
						p = q
					q = scan(p, m, "."c)
					If q <= p Then Exit Do
						p = q
					q = scanByte(p, m)
					If q <= p Then Exit Do
						p = q
					q = scan(p, m, "."c)
					If q <= p Then Exit Do
						p = q
					q = scanByte(p, m)
					If q <= p Then Exit Do
						p = q
					q = scan(p, m, "."c)
					If q <= p Then Exit Do
						p = q
					q = scanByte(p, m)
					If q <= p Then Exit Do
						p = q
					If q < m Then Exit Do
					Return q
				Loop
				fail("Malformed IPv4 address", q)
				Return -1
			End Function

			' Take an IPv4 address: Throw an exception if the given interval
			' contains anything except an IPv4 address
			'
			Private Function takeIPv4Address(ByVal start As Integer, ByVal n As Integer, ByVal expected As String) As Integer
				Dim p As Integer = scanIPv4Address(start, n, True)
				If p <= start Then failExpecting(expected, start)
				Return p
			End Function

			' Attempt to parse an IPv4 address, returning -1 on failure but
			' allowing the given interval to contain [:<characters>] after
			' the IPv4 address.
			'
			Private Function parseIPv4Address(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer

				Try
					p = scanIPv4Address(start, n, False)
				Catch x As URISyntaxException
					Return -1
				Catch nfe As NumberFormatException
					Return -1
				End Try

				If p > start AndAlso p < n Then
					' IPv4 address is followed by something - check that
					' it's a ":" as this is the only valid character to
					' follow an address.
					If charAt(p) <> ":"c Then p = -1
				End If

				If p > start Then outerInstance.host = Substring(start, p)

				Return p
			End Function

			' hostname      = domainlabel [ "." ] | 1*( domainlabel "." ) toplabel [ "." ]
			' domainlabel   = alphanum | alphanum *( alphanum | "-" ) alphanum
			' toplabel      = alpha | alpha *( alphanum | "-" ) alphanum
			'
			Private Function parseHostname(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer
				Dim l As Integer = -1 ' Start of last parsed label

				Do
					' domainlabel = alphanum [ *( alphanum | "-" ) alphanum ]
					q = scan(p, n, L_ALPHANUM, H_ALPHANUM)
					If q <= p Then Exit Do
					l = p
					If q > p Then
						p = q
						q = scan(p, n, L_ALPHANUM Or L_DASH, H_ALPHANUM Or H_DASH)
						If q > p Then
							If charAt(q - 1) = "-"c Then fail("Illegal character in hostname", q - 1)
							p = q
						End If
					End If
					q = scan(p, n, "."c)
					If q <= p Then Exit Do
					p = q
				Loop While p < n

				If (p < n) AndAlso (Not at(p, n, ":"c)) Then fail("Illegal character in hostname", p)

				If l < 0 Then failExpecting("hostname", start)

				' for a fully qualified hostname check that the rightmost
				' label starts with an alpha character.
				If l > start AndAlso (Not match(charAt(l), L_ALPHA, H_ALPHA)) Then fail("Illegal character in hostname", l)

				outerInstance.host = Substring(start, p)
				Return p
			End Function


			' IPv6 address parsing, from RFC2373: IPv6 Addressing Architecture
			'
			' Bug: The grammar in RFC2373 Appendix B does not allow addresses of
			' the form ::12.34.56.78, which are clearly shown in the examples
			' earlier in the document.  Here is the original grammar:
			'
			'   IPv6address = hexpart [ ":" IPv4address ]
			'   hexpart     = hexseq | hexseq "::" [ hexseq ] | "::" [ hexseq ]
			'   hexseq      = hex4 *( ":" hex4)
			'   hex4        = 1*4HEXDIG
			'
			' We therefore use the following revised grammar:
			'
			'   IPv6address = hexseq [ ":" IPv4address ]
			'                 | hexseq [ "::" [ hexpost ] ]
			'                 | "::" [ hexpost ]
			'   hexpost     = hexseq | hexseq ":" IPv4address | IPv4address
			'   hexseq      = hex4 *( ":" hex4)
			'   hex4        = 1*4HEXDIG
			'
			' This covers all and only the following cases:
			'
			'   hexseq
			'   hexseq : IPv4address
			'   hexseq ::
			'   hexseq :: hexseq
			'   hexseq :: hexseq : IPv4address
			'   hexseq :: IPv4address
			'   :: hexseq
			'   :: hexseq : IPv4address
			'   :: IPv4address
			'   ::
			'
			' Additionally we constrain the IPv6 address as follows :-
			'
			'  i.  IPv6 addresses without compressed zeros should contain
			'      exactly 16 bytes.
			'
			'  ii. IPv6 addresses with compressed zeros should contain
			'      less than 16 bytes.

			Private ipv6byteCount As Integer = 0

			Private Function parseIPv6Reference(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer
				Dim compressedZeros As Boolean = False

				q = scanHexSeq(p, n)

				If q > p Then
					p = q
					If at(p, n, "::") Then
						compressedZeros = True
						p = scanHexPost(p + 2, n)
					ElseIf at(p, n, ":"c) Then
						p = takeIPv4Address(p + 1, n, "IPv4 address")
						ipv6byteCount += 4
					End If
				ElseIf at(p, n, "::") Then
					compressedZeros = True
					p = scanHexPost(p + 2, n)
				End If
				If p < n Then fail("Malformed IPv6 address", start)
				If ipv6byteCount > 16 Then fail("IPv6 address too long", start)
				If (Not compressedZeros) AndAlso ipv6byteCount < 16 Then fail("IPv6 address too short", start)
				If compressedZeros AndAlso ipv6byteCount = 16 Then fail("Malformed IPv6 address", start)

				Return p
			End Function

			Private Function scanHexPost(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer

				If p = n Then Return p

				q = scanHexSeq(p, n)
				If q > p Then
					p = q
					If at(p, n, ":"c) Then
						p += 1
						p = takeIPv4Address(p, n, "hex digits or IPv4 address")
						ipv6byteCount += 4
					End If
				Else
					p = takeIPv4Address(p, n, "hex digits or IPv4 address")
					ipv6byteCount += 4
				End If
				Return p
			End Function

			' Scan a hex sequence; return -1 if one could not be scanned
			'
			Private Function scanHexSeq(ByVal start As Integer, ByVal n As Integer) As Integer
				Dim p As Integer = start
				Dim q As Integer

				q = scan(p, n, L_HEX, H_HEX)
				If q <= p Then Return -1
				If at(q, n, "."c) Then ' Beginning of IPv4 address Return -1
				If q > p + 4 Then fail("IPv6 hexadecimal digit sequence too long", p)
				ipv6byteCount += 2
				p = q
				Do While p < n
					If Not at(p, n, ":"c) Then Exit Do
					If at(p + 1, n, ":"c) Then Exit Do ' "::"
					p += 1
					q = scan(p, n, L_HEX, H_HEX)
					If q <= p Then failExpecting("digits for an IPv6 address", p)
					If at(q, n, "."c) Then ' Beginning of IPv4 address
						p -= 1
						Exit Do
					End If
					If q > p + 4 Then fail("IPv6 hexadecimal digit sequence too long", p)
					ipv6byteCount += 2
					p = q
				Loop

				Return p
			End Function

		End Class

	End Class

End Namespace
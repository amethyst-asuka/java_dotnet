Imports System
Imports System.Collections.Generic

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

Namespace java.nio.charset



	''' <summary>
	''' A named mapping between sequences of sixteen-bit Unicode <a
	''' href="../../lang/Character.html#unicode">code units</a> and sequences of
	''' bytes.  This class defines methods for creating decoders and encoders and
	''' for retrieving the various names associated with a charset.  Instances of
	''' this class are immutable.
	''' 
	''' <p> This class also defines static methods for testing whether a particular
	''' charset is supported, for locating charset instances by name, and for
	''' constructing a map that contains every charset for which support is
	''' available in the current Java virtual machine.  Support for new charsets can
	''' be added via the service-provider interface defined in the {@link
	''' java.nio.charset.spi.CharsetProvider} class.
	''' 
	''' <p> All of the methods defined in this class are safe for use by multiple
	''' concurrent threads.
	''' 
	''' 
	''' <a name="names"></a><a name="charenc"></a>
	''' <h2>Charset names</h2>
	''' 
	''' <p> Charsets are named by strings composed of the following characters:
	''' 
	''' <ul>
	''' 
	'''   <li> The uppercase letters <tt>'A'</tt> through <tt>'Z'</tt>
	'''        (<tt>'&#92;u0041'</tt>&nbsp;through&nbsp;<tt>'&#92;u005a'</tt>),
	''' 
	'''   <li> The lowercase letters <tt>'a'</tt> through <tt>'z'</tt>
	'''        (<tt>'&#92;u0061'</tt>&nbsp;through&nbsp;<tt>'&#92;u007a'</tt>),
	''' 
	'''   <li> The digits <tt>'0'</tt> through <tt>'9'</tt>
	'''        (<tt>'&#92;u0030'</tt>&nbsp;through&nbsp;<tt>'&#92;u0039'</tt>),
	''' 
	'''   <li> The dash character <tt>'-'</tt>
	'''        (<tt>'&#92;u002d'</tt>,&nbsp;<small>HYPHEN-MINUS</small>),
	''' 
	'''   <li> The plus character <tt>'+'</tt>
	'''        (<tt>'&#92;u002b'</tt>,&nbsp;<small>PLUS SIGN</small>),
	''' 
	'''   <li> The period character <tt>'.'</tt>
	'''        (<tt>'&#92;u002e'</tt>,&nbsp;<small>FULL STOP</small>),
	''' 
	'''   <li> The colon character <tt>':'</tt>
	'''        (<tt>'&#92;u003a'</tt>,&nbsp;<small>COLON</small>), and
	''' 
	'''   <li> The underscore character <tt>'_'</tt>
	'''        (<tt>'&#92;u005f'</tt>,&nbsp;<small>LOW&nbsp;LINE</small>).
	''' 
	''' </ul>
	''' 
	''' A charset name must begin with either a letter or a digit.  The empty string
	''' is not a legal charset name.  Charset names are not case-sensitive; that is,
	''' case is always ignored when comparing charset names.  Charset names
	''' generally follow the conventions documented in <a
	''' href="http://www.ietf.org/rfc/rfc2278.txt"><i>RFC&nbsp;2278:&nbsp;IANA Charset
	''' Registration Procedures</i></a>.
	''' 
	''' <p> Every charset has a <i>canonical name</i> and may also have one or more
	''' <i>aliases</i>.  The canonical name is returned by the <seealso cref="#name() name"/> method
	''' of this class.  Canonical names are, by convention, usually in upper case.
	''' The aliases of a charset are returned by the <seealso cref="#aliases() aliases"/>
	''' method.
	''' 
	''' <p><a name="hn">Some charsets have an <i>historical name</i> that is defined for
	''' compatibility with previous versions of the Java platform.</a>  A charset's
	''' historical name is either its canonical name or one of its aliases.  The
	''' historical name is returned by the <tt>getEncoding()</tt> methods of the
	''' <seealso cref="java.io.InputStreamReader#getEncoding InputStreamReader"/> and {@link
	''' java.io.OutputStreamWriter#getEncoding OutputStreamWriter} classes.
	''' 
	''' <p><a name="iana"> </a>If a charset listed in the <a
	''' href="http://www.iana.org/assignments/character-sets"><i>IANA Charset
	''' Registry</i></a> is supported by an implementation of the Java platform then
	''' its canonical name must be the name listed in the registry. Many charsets
	''' are given more than one name in the registry, in which case the registry
	''' identifies one of the names as <i>MIME-preferred</i>.  If a charset has more
	''' than one registry name then its canonical name must be the MIME-preferred
	''' name and the other names in the registry must be valid aliases.  If a
	''' supported charset is not listed in the IANA registry then its canonical name
	''' must begin with one of the strings <tt>"X-"</tt> or <tt>"x-"</tt>.
	''' 
	''' <p> The IANA charset registry does change over time, and so the canonical
	''' name and the aliases of a particular charset may also change over time.  To
	''' ensure compatibility it is recommended that no alias ever be removed from a
	''' charset, and that if the canonical name of a charset is changed then its
	''' previous canonical name be made into an alias.
	''' 
	''' 
	''' <h2>Standard charsets</h2>
	''' 
	''' 
	''' 
	''' <p><a name="standard">Every implementation of the Java platform is required to support the
	''' following standard charsets.</a>  Consult the release documentation for your
	''' implementation to see if any other charsets are supported.  The behavior
	''' of such optional charsets may differ between implementations.
	''' 
	''' <blockquote><table width="80%" summary="Description of standard charsets">
	''' <tr><th align="left">Charset</th><th align="left">Description</th></tr>
	''' <tr><td valign=top><tt>US-ASCII</tt></td>
	'''     <td>Seven-bit ASCII, a.k.a. <tt>ISO646-US</tt>,
	'''         a.k.a. the Basic Latin block of the Unicode character set</td></tr>
	''' <tr><td valign=top><tt>ISO-8859-1&nbsp;&nbsp;</tt></td>
	'''     <td>ISO Latin Alphabet No. 1, a.k.a. <tt>ISO-LATIN-1</tt></td></tr>
	''' <tr><td valign=top><tt>UTF-8</tt></td>
	'''     <td>Eight-bit UCS Transformation Format</td></tr>
	''' <tr><td valign=top><tt>UTF-16BE</tt></td>
	'''     <td>Sixteen-bit UCS Transformation Format,
	'''         big-endian byte&nbsp;order</td></tr>
	''' <tr><td valign=top><tt>UTF-16LE</tt></td>
	'''     <td>Sixteen-bit UCS Transformation Format,
	'''         little-endian byte&nbsp;order</td></tr>
	''' <tr><td valign=top><tt>UTF-16</tt></td>
	'''     <td>Sixteen-bit UCS Transformation Format,
	'''         byte&nbsp;order identified by an optional byte-order mark</td></tr>
	''' </table></blockquote>
	''' 
	''' <p> The <tt>UTF-8</tt> charset is specified by <a
	''' href="http://www.ietf.org/rfc/rfc2279.txt"><i>RFC&nbsp;2279</i></a>; the
	''' transformation format upon which it is based is specified in
	''' Amendment&nbsp;2 of ISO&nbsp;10646-1 and is also described in the <a
	''' href="http://www.unicode.org/unicode/standard/standard.html"><i>Unicode
	''' Standard</i></a>.
	''' 
	''' <p> The <tt>UTF-16</tt> charsets are specified by <a
	''' href="http://www.ietf.org/rfc/rfc2781.txt"><i>RFC&nbsp;2781</i></a>; the
	''' transformation formats upon which they are based are specified in
	''' Amendment&nbsp;1 of ISO&nbsp;10646-1 and are also described in the <a
	''' href="http://www.unicode.org/unicode/standard/standard.html"><i>Unicode
	''' Standard</i></a>.
	''' 
	''' <p> The <tt>UTF-16</tt> charsets use sixteen-bit quantities and are
	''' therefore sensitive to byte order.  In these encodings the byte order of a
	''' stream may be indicated by an initial <i>byte-order mark</i> represented by
	''' the Unicode character <tt>'&#92;uFEFF'</tt>.  Byte-order marks are handled
	''' as follows:
	''' 
	''' <ul>
	''' 
	'''   <li><p> When decoding, the <tt>UTF-16BE</tt> and <tt>UTF-16LE</tt>
	'''   charsets interpret the initial byte-order marks as a <small>ZERO-WIDTH
	'''   NON-BREAKING SPACE</small>; when encoding, they do not write
	'''   byte-order marks. </p></li>
	''' 
	''' 
	'''   <li><p> When decoding, the <tt>UTF-16</tt> charset interprets the
	'''   byte-order mark at the beginning of the input stream to indicate the
	'''   byte-order of the stream but defaults to big-endian if there is no
	'''   byte-order mark; when encoding, it uses big-endian byte order and writes
	'''   a big-endian byte-order mark. </p></li>
	''' 
	''' </ul>
	''' 
	''' In any case, byte order marks occurring after the first element of an
	''' input sequence are not omitted since the same code is used to represent
	''' <small>ZERO-WIDTH NON-BREAKING SPACE</small>.
	''' 
	''' <p> Every instance of the Java virtual machine has a default charset, which
	''' may or may not be one of the standard charsets.  The default charset is
	''' determined during virtual-machine startup and typically depends upon the
	''' locale and charset being used by the underlying operating system. </p>
	''' 
	''' <p>The <seealso cref="StandardCharsets"/> class defines constants for each of the
	''' standard charsets.
	''' 
	''' <h2>Terminology</h2>
	''' 
	''' <p> The name of this class is taken from the terms used in
	''' <a href="http://www.ietf.org/rfc/rfc2278.txt"><i>RFC&nbsp;2278</i></a>.
	''' In that document a <i>charset</i> is defined as the combination of
	''' one or more coded character sets and a character-encoding scheme.
	''' (This definition is confusing; some other software systems define
	''' <i>charset</i> as a synonym for <i>coded character set</i>.)
	''' 
	''' <p> A <i>coded character set</i> is a mapping between a set of abstract
	''' characters and a set of integers.  US-ASCII, ISO&nbsp;8859-1,
	''' JIS&nbsp;X&nbsp;0201, and Unicode are examples of coded character sets.
	''' 
	''' <p> Some standards have defined a <i>character set</i> to be simply a
	''' set of abstract characters without an associated assigned numbering.
	''' An alphabet is an example of such a character set.  However, the subtle
	''' distinction between <i>character set</i> and <i>coded character set</i>
	''' is rarely used in practice; the former has become a short form for the
	''' latter, including in the Java API specification.
	''' 
	''' <p> A <i>character-encoding scheme</i> is a mapping between one or more
	''' coded character sets and a set of octet (eight-bit byte) sequences.
	''' UTF-8, UTF-16, ISO&nbsp;2022, and EUC are examples of
	''' character-encoding schemes.  Encoding schemes are often associated with
	''' a particular coded character set; UTF-8, for example, is used only to
	''' encode Unicode.  Some schemes, however, are associated with multiple
	''' coded character sets; EUC, for example, can be used to encode
	''' characters in a variety of Asian coded character sets.
	''' 
	''' <p> When a coded character set is used exclusively with a single
	''' character-encoding scheme then the corresponding charset is usually
	''' named for the coded character set; otherwise a charset is usually named
	''' for the encoding scheme and, possibly, the locale of the coded
	''' character sets that it supports.  Hence <tt>US-ASCII</tt> is both the
	''' name of a coded character set and of the charset that encodes it, while
	''' <tt>EUC-JP</tt> is the name of the charset that encodes the
	''' JIS&nbsp;X&nbsp;0201, JIS&nbsp;X&nbsp;0208, and JIS&nbsp;X&nbsp;0212
	''' coded character sets for the Japanese language.
	''' 
	''' <p> The native character encoding of the Java programming language is
	''' UTF-16.  A charset in the Java platform therefore defines a mapping
	''' between sequences of sixteen-bit UTF-16 code units (that is, sequences
	''' of chars) and sequences of bytes. </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>
	''' <seealso cref= CharsetDecoder </seealso>
	''' <seealso cref= CharsetEncoder </seealso>
	''' <seealso cref= java.nio.charset.spi.CharsetProvider </seealso>
	''' <seealso cref= java.lang.Character </seealso>

	Public MustInherit Class Charset
		Implements Comparable(Of Charset)

		' -- Static methods -- 

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared bugLevel As String = Nothing

		Friend Shared Function atBugLevel(ByVal bl As String) As Boolean ' package-private
			Dim level As String = bugLevel
			If level Is Nothing Then
				If Not sun.misc.VM.booted Then Return False
					level = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("sun.nio.cs.bugLevel", ""))
					bugLevel = level
			End If
			Return level.Equals(bl)
		End Function

		''' <summary>
		''' Checks that the given string is a legal charset name. </p>
		''' </summary>
		''' <param name="s">
		'''         A purported charset name
		''' </param>
		''' <exception cref="IllegalCharsetNameException">
		'''          If the given name is not a legal charset name </exception>
		Private Shared Sub checkName(ByVal s As String)
			Dim n As Integer = s.length()
			If Not atBugLevel("1.4") Then
				If n = 0 Then Throw New IllegalCharsetNameException(s)
			End If
			For i As Integer = 0 To n - 1
				Dim c As Char = s.Chars(i)
				If c >= "A"c AndAlso c <= "Z"c Then Continue For
				If c >= "a"c AndAlso c <= "z"c Then Continue For
				If c >= "0"c AndAlso c <= "9"c Then Continue For
				If c = "-"c AndAlso i <> 0 Then Continue For
				If c = "+"c AndAlso i <> 0 Then Continue For
				If c = ":"c AndAlso i <> 0 Then Continue For
				If c = "_"c AndAlso i <> 0 Then Continue For
				If c = "."c AndAlso i <> 0 Then Continue For
				Throw New IllegalCharsetNameException(s)
			Next i
		End Sub

		' The standard set of charsets 
		Private Shared standardProvider As java.nio.charset.spi.CharsetProvider = New sun.nio.cs.StandardCharsets

		' Cache of the most-recently-returned charsets,
		' along with the names that were used to find them
		'
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared cache1 As Object() = Nothing ' "Level 1" cache
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared cache2 As Object() = Nothing ' "Level 2" cache

		Private Shared Sub cache(ByVal charsetName As String, ByVal cs As Charset)
			cache2 = cache1
			cache1 = New Object() { charsetName, cs }
		End Sub

		' Creates an iterator that walks over the available providers, ignoring
		' those whose lookup or instantiation causes a security exception to be
		' thrown.  Should be invoked with full privileges.
		'
		Private Shared Function providers() As IEnumerator(Of java.nio.charset.spi.CharsetProvider)
			Return New IteratorAnonymousInnerClassHelper(Of E)
		End Function

		Private Class IteratorAnonymousInnerClassHelper(Of E)
			Implements IEnumerator(Of E)

			Friend cl As ClassLoader = ClassLoader.systemClassLoader
			Friend sl As java.util.ServiceLoader(Of java.nio.charset.spi.CharsetProvider) = java.util.ServiceLoader.load(GetType(java.nio.charset.spi.CharsetProvider), cl)
			Friend i As IEnumerator(Of java.nio.charset.spi.CharsetProvider) = sl.GetEnumerator()

			Friend next As java.nio.charset.spi.CharsetProvider = Nothing

			Private Property [next] As Boolean
				Get
					Do While next Is Nothing
						Try
							If Not i.hasNext() Then Return False
							next = i.next()
						Catch sce As java.util.ServiceConfigurationError
							If TypeOf sce.cause Is SecurityException Then Continue Do
							Throw sce
						End Try
					Loop
					Return True
				End Get
			End Property

			Public Overridable Function hasNext() As Boolean
				Return [next]
			End Function

			Public Overridable Function [next]() As java.nio.charset.spi.CharsetProvider
				If Not [next] Then Throw New java.util.NoSuchElementException
				Dim n As java.nio.charset.spi.CharsetProvider = next
				next = Nothing
				Return n
			End Function

			Public Overridable Sub remove()
				Throw New UnsupportedOperationException
			End Sub

		End Class

		' Thread-local gate to prevent recursive provider lookups
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared gate As New ThreadLocal(Of ThreadLocal(Of ?))

		Private Shared Function lookupViaProviders(ByVal charsetName As String) As Charset

			' The runtime startup sequence looks up standard charsets as a
			' consequence of the VM's invocation of System.initializeSystemClass
			' in order to, e.g., set system properties and encode filenames.  At
			' that point the application class loader has not been initialized,
			' however, so we can't look for providers because doing so will cause
			' that loader to be prematurely initialized with incomplete
			' information.
			'
			If Not sun.misc.VM.booted Then Return Nothing

			If gate.get() IsNot Nothing Then Return Nothing
			Try
				gate.set(gate)

				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			Finally
				gate.set(Nothing)
			End Try
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Charset
				Dim i As IEnumerator(Of java.nio.charset.spi.CharsetProvider) = providers()
				Do While i.MoveNext()
					Dim cp As java.nio.charset.spi.CharsetProvider = i.Current
					Dim cs As Charset = cp.charsetForName(charsetName)
					If cs IsNot Nothing Then Return cs
				Loop
				Return Nothing
			End Function
		End Class

		' The extended set of charsets 
		Private Class ExtendedProviderHolder
			Friend Shared ReadOnly extendedProvider_Renamed As java.nio.charset.spi.CharsetProvider = extendedProvider()
			' returns ExtendedProvider, if installed
			Private Shared Function extendedProvider() As java.nio.charset.spi.CharsetProvider
				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As java.nio.charset.spi.CharsetProvider
					 Try
						 Dim epc As Class = Type.GetType("sun.nio.cs.ext.ExtendedCharsets")
						 Return CType(epc.newInstance(), java.nio.charset.spi.CharsetProvider)
					 Catch x As ClassNotFoundException
						 ' Extended charsets not available
						 ' (charsets.jar not present)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
					 Catch InstantiationException Or IllegalAccessException x
					   Throw New [Error](x)
					 End Try
					 Return Nothing
				End Function
			End Class
		End Class

		Private Shared Function lookupExtendedCharset(ByVal charsetName As String) As Charset
			Dim ecp As java.nio.charset.spi.CharsetProvider = ExtendedProviderHolder.extendedProvider_Renamed
			Return If(ecp IsNot Nothing, ecp.charsetForName(charsetName), Nothing)
		End Function

		Private Shared Function lookup(ByVal charsetName As String) As Charset
			If charsetName Is Nothing Then Throw New IllegalArgumentException("Null charset name")
			Dim a As Object()
			a = cache1
			If a IsNot Nothing AndAlso charsetName.Equals(a(0)) Then Return CType(a(1), Charset)
			' We expect most programs to use one Charset repeatedly.
			' We convey a hint to this effect to the VM by putting the
			' level 1 cache miss code in a separate method.
			Return lookup2(charsetName)
		End Function

		Private Shared Function lookup2(ByVal charsetName As String) As Charset
			Dim a As Object()
			a = cache2
			If a IsNot Nothing AndAlso charsetName.Equals(a(0)) Then
				cache2 = cache1
				cache1 = a
				Return CType(a(1), Charset)
			End If
			Dim cs As Charset
			cs = standardProvider.charsetForName(charsetName)
			cs = lookupExtendedCharset(charsetName)
			cs = lookupViaProviders(charsetName)
			If cs IsNot Nothing OrElse cs IsNot Nothing OrElse cs IsNot Nothing Then
				cache(charsetName, cs)
				Return cs
			End If

			' Only need to check the name if we didn't find a charset for it 
			checkName(charsetName)
			Return Nothing
		End Function

		''' <summary>
		''' Tells whether the named charset is supported.
		''' </summary>
		''' <param name="charsetName">
		'''         The name of the requested charset; may be either
		'''         a canonical name or an alias
		''' </param>
		''' <returns>  <tt>true</tt> if, and only if, support for the named charset
		'''          is available in the current Java virtual machine
		''' </returns>
		''' <exception cref="IllegalCharsetNameException">
		'''         If the given charset name is illegal
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the given <tt>charsetName</tt> is null </exception>
		Public Shared Function isSupported(ByVal charsetName As String) As Boolean
			Return (lookup(charsetName) IsNot Nothing)
		End Function

		''' <summary>
		''' Returns a charset object for the named charset.
		''' </summary>
		''' <param name="charsetName">
		'''         The name of the requested charset; may be either
		'''         a canonical name or an alias
		''' </param>
		''' <returns>  A charset object for the named charset
		''' </returns>
		''' <exception cref="IllegalCharsetNameException">
		'''          If the given charset name is illegal
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the given <tt>charsetName</tt> is null
		''' </exception>
		''' <exception cref="UnsupportedCharsetException">
		'''          If no support for the named charset is available
		'''          in this instance of the Java virtual machine </exception>
		Public Shared Function forName(ByVal charsetName As String) As Charset
			Dim cs As Charset = lookup(charsetName)
			If cs IsNot Nothing Then Return cs
			Throw New UnsupportedCharsetException(charsetName)
		End Function

		' Fold charsets from the given iterator into the given map, ignoring
		' charsets whose names already have entries in the map.
		'
		Private Shared Sub put(ByVal i As IEnumerator(Of Charset), ByVal m As IDictionary(Of String, Charset))
			Do While i.MoveNext()
				Dim cs As Charset = i.Current
				If Not m.ContainsKey(cs.name()) Then m(cs.name()) = cs
			Loop
		End Sub

		''' <summary>
		''' Constructs a sorted map from canonical charset names to charset objects.
		''' 
		''' <p> The map returned by this method will have one entry for each charset
		''' for which support is available in the current Java virtual machine.  If
		''' two or more supported charsets have the same canonical name then the
		''' resulting map will contain just one of them; which one it will contain
		''' is not specified. </p>
		''' 
		''' <p> The invocation of this method, and the subsequent use of the
		''' resulting map, may cause time-consuming disk or network I/O operations
		''' to occur.  This method is provided for applications that need to
		''' enumerate all of the available charsets, for example to allow user
		''' charset selection.  This method is not used by the {@link #forName
		''' forName} method, which instead employs an efficient incremental lookup
		''' algorithm.
		''' 
		''' <p> This method may return different results at different times if new
		''' charset providers are dynamically made available to the current Java
		''' virtual machine.  In the absence of such changes, the charsets returned
		''' by this method are exactly those that can be retrieved via the {@link
		''' #forName forName} method.  </p>
		''' </summary>
		''' <returns> An immutable, case-insensitive map from canonical charset names
		'''         to charset objects </returns>
		Public Shared Function availableCharsets() As java.util.SortedMap(Of String, Charset)
			Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As java.util.SortedMap(Of String, Charset)
				Dim m As New SortedDictionary(Of String, Charset)(sun.misc.ASCIICaseInsensitiveComparator.CASE_INSENSITIVE_ORDER)
				put(standardProvider.charsets(), m)
				Dim ecp As java.nio.charset.spi.CharsetProvider = ExtendedProviderHolder.extendedProvider_Renamed
				If ecp IsNot Nothing Then put(ecp.charsets(), m)
				Dim i As IEnumerator(Of java.nio.charset.spi.CharsetProvider) = providers()
				Do While i.MoveNext()
					Dim cp As java.nio.charset.spi.CharsetProvider = i.Current
					put(cp.charsets(), m)
				Loop
				Return java.util.Collections.unmodifiableSortedMap(m)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared defaultCharset_Renamed As Charset

		''' <summary>
		''' Returns the default charset of this Java virtual machine.
		''' 
		''' <p> The default charset is determined during virtual-machine startup and
		''' typically depends upon the locale and charset of the underlying
		''' operating system.
		''' </summary>
		''' <returns>  A charset object for the default charset
		''' 
		''' @since 1.5 </returns>
		Public Shared Function defaultCharset() As Charset
			If defaultCharset_Renamed Is Nothing Then
				SyncLock GetType(Charset)
					Dim csn As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("file.encoding"))
					Dim cs As Charset = lookup(csn)
					If cs IsNot Nothing Then
						defaultCharset_Renamed = cs
					Else
						defaultCharset_Renamed = forName("UTF-8")
					End If
				End SyncLock
			End If
			Return defaultCharset_Renamed
		End Function


		' -- Instance fields and methods -- 

		Private ReadOnly name_Renamed As String ' tickles a bug in oldjavac
		Private ReadOnly aliases_Renamed As String() ' tickles a bug in oldjavac
		Private aliasSet As java.util.Set(Of String) = Nothing

		''' <summary>
		''' Initializes a new charset with the given canonical name and alias
		''' set.
		''' </summary>
		''' <param name="canonicalName">
		'''         The canonical name of this charset
		''' </param>
		''' <param name="aliases">
		'''         An array of this charset's aliases, or null if it has no aliases
		''' </param>
		''' <exception cref="IllegalCharsetNameException">
		'''         If the canonical name or any of the aliases are illegal </exception>
		Protected Friend Sub New(ByVal canonicalName As String, ByVal aliases As String())
			checkName(canonicalName)
			Dim [as] As String() = If(aliases Is Nothing, New String(){}, aliases)
			For i As Integer = 0 To [as].Length - 1
				checkName([as](i))
			Next i
			Me.name_Renamed = canonicalName
			Me.aliases_Renamed = [as]
		End Sub

		''' <summary>
		''' Returns this charset's canonical name.
		''' </summary>
		''' <returns>  The canonical name of this charset </returns>
		Public Function name() As String
			Return name_Renamed
		End Function

		''' <summary>
		''' Returns a set containing this charset's aliases.
		''' </summary>
		''' <returns>  An immutable set of this charset's aliases </returns>
		Public Function aliases() As java.util.Set(Of String)
			If aliasSet IsNot Nothing Then Return aliasSet
			Dim n As Integer = aliases_Renamed.Length
			Dim hs As New HashSet(Of String)(n)
			For i As Integer = 0 To n - 1
				hs.Add(aliases_Renamed(i))
			Next i
			aliasSet = java.util.Collections.unmodifiableSet(hs)
			Return aliasSet
		End Function

		''' <summary>
		''' Returns this charset's human-readable name for the default locale.
		''' 
		''' <p> The default implementation of this method simply returns this
		''' charset's canonical name.  Concrete subclasses of this class may
		''' override this method in order to provide a localized display name. </p>
		''' </summary>
		''' <returns>  The display name of this charset in the default locale </returns>
		Public Overridable Function displayName() As String
			Return name_Renamed
		End Function

		''' <summary>
		''' Tells whether or not this charset is registered in the <a
		''' href="http://www.iana.org/assignments/character-sets">IANA Charset
		''' Registry</a>.
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this charset is known by its
		'''          implementor to be registered with the IANA </returns>
		Public Property registered As Boolean
			Get
				Return (Not name_Renamed.StartsWith("X-")) AndAlso Not name_Renamed.StartsWith("x-")
			End Get
		End Property

		''' <summary>
		''' Returns this charset's human-readable name for the given locale.
		''' 
		''' <p> The default implementation of this method simply returns this
		''' charset's canonical name.  Concrete subclasses of this class may
		''' override this method in order to provide a localized display name. </p>
		''' </summary>
		''' <param name="locale">
		'''         The locale for which the display name is to be retrieved
		''' </param>
		''' <returns>  The display name of this charset in the given locale </returns>
		Public Overridable Function displayName(ByVal locale As java.util.Locale) As String
			Return name_Renamed
		End Function

		''' <summary>
		''' Tells whether or not this charset contains the given charset.
		''' 
		''' <p> A charset <i>C</i> is said to <i>contain</i> a charset <i>D</i> if,
		''' and only if, every character representable in <i>D</i> is also
		''' representable in <i>C</i>.  If this relationship holds then it is
		''' guaranteed that every string that can be encoded in <i>D</i> can also be
		''' encoded in <i>C</i> without performing any replacements.
		''' 
		''' <p> That <i>C</i> contains <i>D</i> does not imply that each character
		''' representable in <i>C</i> by a particular byte sequence is represented
		''' in <i>D</i> by the same byte sequence, although sometimes this is the
		''' case.
		''' 
		''' <p> Every charset contains itself.
		''' 
		''' <p> This method computes an approximation of the containment relation:
		''' If it returns <tt>true</tt> then the given charset is known to be
		''' contained by this charset; if it returns <tt>false</tt>, however, then
		''' it is not necessarily the case that the given charset is not contained
		''' in this charset.
		''' </summary>
		''' <param name="cs">
		'''          The given charset
		''' </param>
		''' <returns>  <tt>true</tt> if the given charset is contained in this charset </returns>
		Public MustOverride Function contains(ByVal cs As Charset) As Boolean

		''' <summary>
		''' Constructs a new decoder for this charset.
		''' </summary>
		''' <returns>  A new decoder for this charset </returns>
		Public MustOverride Function newDecoder() As CharsetDecoder

		''' <summary>
		''' Constructs a new encoder for this charset.
		''' </summary>
		''' <returns>  A new encoder for this charset
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''          If this charset does not support encoding </exception>
		Public MustOverride Function newEncoder() As CharsetEncoder

		''' <summary>
		''' Tells whether or not this charset supports encoding.
		''' 
		''' <p> Nearly all charsets support encoding.  The primary exceptions are
		''' special-purpose <i>auto-detect</i> charsets whose decoders can determine
		''' which of several possible encoding schemes is in use by examining the
		''' input byte sequence.  Such charsets do not support encoding because
		''' there is no way to determine which encoding should be used on output.
		''' Implementations of such charsets should override this method to return
		''' <tt>false</tt>. </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this charset supports encoding </returns>
		Public Overridable Function canEncode() As Boolean
			Return True
		End Function

		''' <summary>
		''' Convenience method that decodes bytes in this charset into Unicode
		''' characters.
		''' 
		''' <p> An invocation of this method upon a charset <tt>cs</tt> returns the
		''' same result as the expression
		''' 
		''' <pre>
		'''     cs.newDecoder()
		'''       .onMalformedInput(CodingErrorAction.REPLACE)
		'''       .onUnmappableCharacter(CodingErrorAction.REPLACE)
		'''       .decode(bb); </pre>
		''' 
		''' except that it is potentially more efficient because it can cache
		''' decoders between successive invocations.
		''' 
		''' <p> This method always replaces malformed-input and unmappable-character
		''' sequences with this charset's default replacement byte array.  In order
		''' to detect such sequences, use the {@link
		''' CharsetDecoder#decode(java.nio.ByteBuffer)} method directly.  </p>
		''' </summary>
		''' <param name="bb">  The byte buffer to be decoded
		''' </param>
		''' <returns>  A char buffer containing the decoded characters </returns>
		Public Function decode(ByVal bb As java.nio.ByteBuffer) As java.nio.CharBuffer
			Try
				Return sun.nio.cs.ThreadLocalCoders.decoderFor(Me).onMalformedInput(CodingErrorAction.REPLACE).onUnmappableCharacter(CodingErrorAction.REPLACE).decode(bb)
			Catch x As CharacterCodingException
				Throw New [Error](x) ' Can't happen
			End Try
		End Function

		''' <summary>
		''' Convenience method that encodes Unicode characters into bytes in this
		''' charset.
		''' 
		''' <p> An invocation of this method upon a charset <tt>cs</tt> returns the
		''' same result as the expression
		''' 
		''' <pre>
		'''     cs.newEncoder()
		'''       .onMalformedInput(CodingErrorAction.REPLACE)
		'''       .onUnmappableCharacter(CodingErrorAction.REPLACE)
		'''       .encode(bb); </pre>
		''' 
		''' except that it is potentially more efficient because it can cache
		''' encoders between successive invocations.
		''' 
		''' <p> This method always replaces malformed-input and unmappable-character
		''' sequences with this charset's default replacement string.  In order to
		''' detect such sequences, use the {@link
		''' CharsetEncoder#encode(java.nio.CharBuffer)} method directly.  </p>
		''' </summary>
		''' <param name="cb">  The char buffer to be encoded
		''' </param>
		''' <returns>  A byte buffer containing the encoded characters </returns>
		Public Function encode(ByVal cb As java.nio.CharBuffer) As java.nio.ByteBuffer
			Try
				Return sun.nio.cs.ThreadLocalCoders.encoderFor(Me).onMalformedInput(CodingErrorAction.REPLACE).onUnmappableCharacter(CodingErrorAction.REPLACE).encode(cb)
			Catch x As CharacterCodingException
				Throw New [Error](x) ' Can't happen
			End Try
		End Function

		''' <summary>
		''' Convenience method that encodes a string into bytes in this charset.
		''' 
		''' <p> An invocation of this method upon a charset <tt>cs</tt> returns the
		''' same result as the expression
		''' 
		''' <pre>
		'''     cs.encode(CharBuffer.wrap(s)); </pre>
		''' </summary>
		''' <param name="str">  The string to be encoded
		''' </param>
		''' <returns>  A byte buffer containing the encoded characters </returns>
		Public Function encode(ByVal str As String) As java.nio.ByteBuffer
			Return encode(java.nio.CharBuffer.wrap(str))
		End Function

		''' <summary>
		''' Compares this charset to another.
		''' 
		''' <p> Charsets are ordered by their canonical names, without regard to
		''' case. </p>
		''' </summary>
		''' <param name="that">
		'''         The charset to which this charset is to be compared
		''' </param>
		''' <returns> A negative integer, zero, or a positive integer as this charset
		'''         is less than, equal to, or greater than the specified charset </returns>
		Public Function compareTo(ByVal that As Charset) As Integer Implements Comparable(Of Charset).compareTo
			Return (name().compareToIgnoreCase(that.name()))
		End Function

		''' <summary>
		''' Computes a hashcode for this charset.
		''' </summary>
		''' <returns>  An integer hashcode </returns>
		Public NotOverridable Overrides Function GetHashCode() As Integer
			Return name().GetHashCode()
		End Function

		''' <summary>
		''' Tells whether or not this object is equal to another.
		''' 
		''' <p> Two charsets are equal if, and only if, they have the same canonical
		''' names.  A charset is never equal to any other type of object.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this charset is equal to the
		'''          given object </returns>
		Public NotOverridable Overrides Function Equals(ByVal ob As Object) As Boolean
			If Not(TypeOf ob Is Charset) Then Return False
			If Me Is ob Then Return True
			Return name_Renamed.Equals(CType(ob, Charset).name())
		End Function

		''' <summary>
		''' Returns a string describing this charset.
		''' </summary>
		''' <returns>  A string describing this charset </returns>
		Public NotOverridable Overrides Function ToString() As String
			Return name()
		End Function

	End Class

End Namespace
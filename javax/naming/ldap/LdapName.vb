Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents a distinguished name as specified by
	''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>.
	''' A distinguished name, or DN, is composed of an ordered list of
	''' components called <em>relative distinguished name</em>s, or RDNs.
	''' Details of a DN's syntax are described in RFC 2253.
	''' <p>
	''' This class resolves a few ambiguities found in RFC 2253
	''' as follows:
	''' <ul>
	''' <li> RFC 2253 leaves the term "whitespace" undefined. The
	'''      ASCII space character 0x20 (" ") is used in its place.
	''' <li> Whitespace is allowed on either side of ',', ';', '=', and '+'.
	'''      Such whitespace is accepted but not generated by this code,
	'''      and is ignored when comparing names.
	''' <li> AttributeValue strings containing '=' or non-leading '#'
	'''      characters (unescaped) are accepted.
	''' </ul>
	''' <p>
	''' String names passed to <code>LdapName</code> or returned by it
	''' use the full Unicode character set. They may also contain
	''' characters encoded into UTF-8 with each octet represented by a
	''' three-character substring such as "\\B4".
	''' They may not, however, contain characters encoded into UTF-8 with
	''' each octet represented by a single character in the string:  the
	''' meaning would be ambiguous.
	''' <p>
	''' <code>LdapName</code> will properly parse all valid names, but
	''' does not attempt to detect all possible violations when parsing
	''' invalid names.  It is "generous" in accepting invalid names.
	''' The "validity" of a name is determined ultimately when it
	''' is supplied to an LDAP server, which may accept or
	''' reject the name based on factors such as its schema information
	''' and interoperability considerations.
	''' <p>
	''' When names are tested for equality, attribute types, both binary
	''' and string values, are case-insensitive.
	''' String values with different but equivalent usage of quoting,
	''' escaping, or UTF8-hex-encoding are considered equal.  The order of
	''' components in multi-valued RDNs (such as "ou=Sales+cn=Bob") is not
	''' significant.
	''' <p>
	''' The components of a LDAP name, that is, RDNs, are numbered. The
	''' indexes of a LDAP name with n RDNs range from 0 to n-1.
	''' This range may be written as [0,n).
	''' The right most RDN is at index 0, and the left most RDN is at
	''' index n-1. For example, the distinguished name:
	''' "CN=Steve Kille, O=Isode Limited, C=GB" is numbered in the following
	''' sequence ranging from 0 to 2: {C=GB, O=Isode Limited, CN=Steve Kille}. An
	''' empty LDAP name is represented by an empty RDN list.
	''' <p>
	''' Concurrent multithreaded read-only access of an instance of
	''' <tt>LdapName</tt> need not be synchronized.
	''' <p>
	''' Unless otherwise noted, the behavior of passing a null argument
	''' to a constructor or method in this class will cause a
	''' NullPointerException to be thrown.
	''' 
	''' @author Scott Seligman
	''' @since 1.5
	''' </summary>

	Public Class LdapName
		Implements javax.naming.Name

		<NonSerialized> _
		Private rdns As IList(Of Rdn) ' parsed name components
		<NonSerialized> _
		Private unparsed As String ' if non-null, the DN in unparsed form
		Private Const serialVersionUID As Long = -1595520034788997356L

		''' <summary>
		''' Constructs an LDAP name from the given distinguished name.
		''' </summary>
		''' <param name="name">  This is a non-null distinguished name formatted
		''' according to the rules defined in
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>.
		''' </param>
		''' <exception cref="InvalidNameException"> if a syntax violation is detected. </exception>
		''' <seealso cref= Rdn#escapeValue(Object value) </seealso>
		Public Sub New(ByVal name As String)
			unparsed = name
			parse()
		End Sub

		''' <summary>
		''' Constructs an LDAP name given its parsed RDN components.
		''' <p>
		''' The indexing of RDNs in the list follows the numbering of
		''' RDNs described in the class description.
		''' </summary>
		''' <param name="rdns"> The non-null list of <tt>Rdn</tt>s forming this LDAP name. </param>
		Public Sub New(ByVal rdns As IList(Of Rdn))

			' if (rdns instanceof ArrayList<Rdn>) {
			'      this.rdns = rdns.clone();
			' } else if (rdns instanceof List<Rdn>) {
			'      this.rdns = new ArrayList<Rdn>(rdns);
			' } else {
			'      throw IllegalArgumentException(
			'              "Invalid entries, list entries must be of type Rdn");
			'  }

			Me.rdns = New List(Of )(rdns.Count)
			For i As Integer = 0 To rdns.Count - 1
				Dim obj As Object = rdns(i)
				If Not(TypeOf obj Is Rdn) Then Throw New System.ArgumentException("Entry:" & obj & "  not a valid type;list entries must be of type Rdn")
				Me.rdns.Add(CType(obj, Rdn))
			Next i
		End Sub

	'    
	'     * Constructs an LDAP name given its parsed components (the elements
	'     * of "rdns" in the range [beg,end)) and, optionally
	'     * (if "name" is not null), the unparsed DN.
	'     *
	'     
		Private Sub New(ByVal name As String, ByVal rdns As IList(Of Rdn), ByVal beg As Integer, ByVal [end] As Integer)
			unparsed = name
			' this.rdns = rdns.subList(beg, end);

			Dim sList As IList(Of Rdn) = rdns.subList(beg, [end])
			Me.rdns = New List(Of )(sList)
		End Sub

		''' <summary>
		''' Retrieves the number of components in this LDAP name. </summary>
		''' <returns> The non-negative number of components in this LDAP name. </returns>
		Public Overridable Function size() As Integer
			Return rdns.Count
		End Function

		''' <summary>
		''' Determines whether this LDAP name is empty.
		''' An empty name is one with zero components. </summary>
		''' <returns> true if this LDAP name is empty, false otherwise. </returns>
		Public Overridable Property empty As Boolean
			Get
				Return rdns.Count = 0
			End Get
		End Property

		''' <summary>
		''' Retrieves the components of this name as an enumeration
		''' of strings. The effect of updates to this name on this enumeration
		''' is undefined. If the name has zero components, an empty (non-null)
		''' enumeration is returned.
		''' The order of the components returned by the enumeration is same as
		''' the order in which the components are numbered as described in the
		''' class description.
		''' </summary>
		''' <returns> A non-null enumeration of the components of this LDAP name.
		''' Each element of the enumeration is of class String. </returns>
		Public Overridable Property all As System.Collections.IEnumerator(Of String)
			Get
				Dim iter As IEnumerator(Of Rdn) = rdns.GetEnumerator()
    
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return New java.util.Enumeration<String>()
		'		{
		'			public boolean hasMoreElements()
		'			{
		'				Return iter.hasNext();
		'			}
		'			public String nextElement()
		'			{
		'				Return iter.next().toString();
		'			}
		'		};
			End Get
		End Property

		''' <summary>
		''' Retrieves a component of this LDAP name as a string. </summary>
		''' <param name="posn"> The 0-based index of the component to retrieve.
		'''              Must be in the range [0,size()). </param>
		''' <returns> The non-null component at index posn. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if posn is outside the
		'''          specified range. </exception>
		Public Overridable Function [get](ByVal posn As Integer) As String
			Return rdns(posn).ToString()
		End Function

		''' <summary>
		''' Retrieves an RDN of this LDAP name as an Rdn. </summary>
		''' <param name="posn"> The 0-based index of the RDN to retrieve.
		'''          Must be in the range [0,size()). </param>
		''' <returns> The non-null RDN at index posn. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if posn is outside the
		'''            specified range. </exception>
		Public Overridable Function getRdn(ByVal posn As Integer) As Rdn
			Return rdns(posn)
		End Function

		''' <summary>
		''' Creates a name whose components consist of a prefix of the
		''' components of this LDAP name.
		''' Subsequent changes to this name will not affect the name
		''' that is returned and vice versa. </summary>
		''' <param name="posn">     The 0-based index of the component at which to stop.
		'''                  Must be in the range [0,size()]. </param>
		''' <returns>  An instance of <tt>LdapName</tt> consisting of the
		'''          components at indexes in the range [0,posn).
		'''          If posn is zero, an empty LDAP name is returned. </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''              If posn is outside the specified range. </exception>
		Public Overridable Function getPrefix(ByVal posn As Integer) As javax.naming.Name
			Try
				Return New LdapName(Nothing, rdns, 0, posn)
			Catch e As System.ArgumentException
				Throw New System.IndexOutOfRangeException("Posn: " & posn & ", Size: " & rdns.Count)
			End Try
		End Function

		''' <summary>
		''' Creates a name whose components consist of a suffix of the
		''' components in this LDAP name.
		''' Subsequent changes to this name do not affect the name that is
		''' returned and vice versa.
		''' </summary>
		''' <param name="posn">     The 0-based index of the component at which to start.
		'''                  Must be in the range [0,size()]. </param>
		''' <returns>  An instance of <tt>LdapName</tt> consisting of the
		'''          components at indexes in the range [posn,size()).
		'''          If posn is equal to size(), an empty LDAP name is
		'''          returned. </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If posn is outside the specified range. </exception>
		Public Overridable Function getSuffix(ByVal posn As Integer) As javax.naming.Name
			Try
				Return New LdapName(Nothing, rdns, posn, rdns.Count)
			Catch e As System.ArgumentException
				Throw New System.IndexOutOfRangeException("Posn: " & posn & ", Size: " & rdns.Count)
			End Try
		End Function

		''' <summary>
		''' Determines whether this LDAP name starts with a specified LDAP name
		''' prefix.
		''' A name <tt>n</tt> is a prefix if it is equal to
		''' <tt>getPrefix(n.size())</tt>--in other words this LDAP
		''' name starts with 'n'. If n is null or not a RFC2253 formatted name
		''' as described in the class description, false is returned.
		''' </summary>
		''' <param name="n"> The LDAP name to check. </param>
		''' <returns>  true if <tt>n</tt> is a prefix of this LDAP name,
		''' false otherwise. </returns>
		''' <seealso cref= #getPrefix(int posn) </seealso>
		Public Overridable Function startsWith(ByVal n As javax.naming.Name) As Boolean
			If n Is Nothing Then Return False
			Dim len1 As Integer = rdns.Count
			Dim len2 As Integer = n.size()
			Return (len1 >= len2 AndAlso matches(0, len2, n))
		End Function

		''' <summary>
		''' Determines whether the specified RDN sequence forms a prefix of this
		''' LDAP name.  Returns true if this LdapName is at least as long as rdns,
		''' and for every position p in the range [0, rdns.size()) the component
		''' getRdn(p) matches rdns.get(p). Returns false otherwise. If rdns is
		''' null, false is returned.
		''' </summary>
		''' <param name="rdns"> The sequence of <tt>Rdn</tt>s to check. </param>
		''' <returns>  true if <tt>rdns</tt> form a prefix of this LDAP name,
		'''          false otherwise. </returns>
		Public Overridable Function startsWith(ByVal rdns As IList(Of Rdn)) As Boolean
			If rdns Is Nothing Then Return False
			Dim len1 As Integer = Me.rdns.Count
			Dim len2 As Integer = rdns.Count
			Return (len1 >= len2 AndAlso doesListMatch(0, len2, rdns))
		End Function

		''' <summary>
		''' Determines whether this LDAP name ends with a specified
		''' LDAP name suffix.
		''' A name <tt>n</tt> is a suffix if it is equal to
		''' <tt>getSuffix(size()-n.size())</tt>--in other words this LDAP
		''' name ends with 'n'. If n is null or not a RFC2253 formatted name
		''' as described in the class description, false is returned.
		''' </summary>
		''' <param name="n"> The LDAP name to check. </param>
		''' <returns> true if <tt>n</tt> is a suffix of this name, false otherwise. </returns>
		''' <seealso cref= #getSuffix(int posn) </seealso>
		Public Overridable Function endsWith(ByVal n As javax.naming.Name) As Boolean
			If n Is Nothing Then Return False
			Dim len1 As Integer = rdns.Count
			Dim len2 As Integer = n.size()
			Return (len1 >= len2 AndAlso matches(len1 - len2, len1, n))
		End Function

		''' <summary>
		''' Determines whether the specified RDN sequence forms a suffix of this
		''' LDAP name.  Returns true if this LdapName is at least as long as rdns,
		''' and for every position p in the range [size() - rdns.size(), size())
		''' the component getRdn(p) matches rdns.get(p). Returns false otherwise.
		''' If rdns is null, false is returned.
		''' </summary>
		''' <param name="rdns"> The sequence of <tt>Rdn</tt>s to check. </param>
		''' <returns>  true if <tt>rdns</tt> form a suffix of this LDAP name,
		'''          false otherwise. </returns>
		Public Overridable Function endsWith(ByVal rdns As IList(Of Rdn)) As Boolean
			If rdns Is Nothing Then Return False
			Dim len1 As Integer = Me.rdns.Count
			Dim len2 As Integer = rdns.Count
			Return (len1 >= len2 AndAlso doesListMatch(len1 - len2, len1, rdns))
		End Function

		Private Function doesListMatch(ByVal beg As Integer, ByVal [end] As Integer, ByVal rdns As IList(Of Rdn)) As Boolean
			For i As Integer = beg To [end] - 1
				If Not Me.rdns(i).Equals(rdns(i - beg)) Then Return False
			Next i
			Return True
		End Function

	'    
	'     * Helper method for startsWith() and endsWith().
	'     * Returns true if components [beg,end) match the components of "n".
	'     * If "n" is not an LdapName, each of its components is parsed as
	'     * the string form of an RDN.
	'     * The following must hold:  end - beg == n.size().
	'     
		Private Function matches(ByVal beg As Integer, ByVal [end] As Integer, ByVal n As javax.naming.Name) As Boolean
			If TypeOf n Is LdapName Then
				Dim ln As LdapName = CType(n, LdapName)
				Return doesListMatch(beg, [end], ln.rdns)
			Else
				For i As Integer = beg To [end] - 1
					Dim ___rdn As Rdn
					Dim rdnString As String = n.get(i - beg)
					Try
						___rdn = (New Rfc2253Parser(rdnString)).parseRdn()
					Catch e As javax.naming.InvalidNameException
						Return False
					End Try
					If Not ___rdn.Equals(rdns(i)) Then Return False
				Next i
			End If
			Return True
		End Function

		''' <summary>
		''' Adds the components of a name -- in order -- to the end of this name.
		''' </summary>
		''' <param name="suffix"> The non-null components to add. </param>
		''' <returns>  The updated name (not a new instance).
		''' </returns>
		''' <exception cref="InvalidNameException"> if <tt>suffix</tt> is not a valid LDAP
		'''          name, or if the addition of the components would violate the
		'''          syntax rules of this LDAP name. </exception>
		Public Overridable Function addAll(ByVal suffix As javax.naming.Name) As javax.naming.Name
			 Return addAll(size(), suffix)
		End Function


		''' <summary>
		''' Adds the RDNs of a name -- in order -- to the end of this name.
		''' </summary>
		''' <param name="suffixRdns"> The non-null suffix <tt>Rdn</tt>s to add. </param>
		''' <returns>  The updated name (not a new instance). </returns>
		Public Overridable Function addAll(ByVal suffixRdns As IList(Of Rdn)) As javax.naming.Name
			Return addAll(size(), suffixRdns)
		End Function

		''' <summary>
		''' Adds the components of a name -- in order -- at a specified position
		''' within this name. Components of this LDAP name at or after the
		''' index (if any) of the first new component are shifted up
		''' (away from index 0) to accommodate the new components.
		''' </summary>
		''' <param name="suffix">    The non-null components to add. </param>
		''' <param name="posn">      The index at which to add the new component.
		'''                  Must be in the range [0,size()].
		''' </param>
		''' <returns>  The updated name (not a new instance).
		''' </returns>
		''' <exception cref="InvalidNameException"> if <tt>suffix</tt> is not a valid LDAP
		'''          name, or if the addition of the components would violate the
		'''          syntax rules of this LDAP name. </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If posn is outside the specified range. </exception>
		Public Overridable Function addAll(ByVal posn As Integer, ByVal suffix As javax.naming.Name) As javax.naming.Name
			unparsed = Nothing ' no longer valid
			If TypeOf suffix Is LdapName Then
				Dim s As LdapName = CType(suffix, LdapName)
				rdns.AddRange(posn, s.rdns)
			Else
				Dim comps As System.Collections.IEnumerator(Of String) = suffix.all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While comps.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					rdns.Insert(posn, ((New Rfc2253Parser(comps.nextElement())).parseRdn()))
					posn += 1
				Loop
			End If
			Return Me
		End Function

		''' <summary>
		''' Adds the RDNs of a name -- in order -- at a specified position
		''' within this name. RDNs of this LDAP name at or after the
		''' index (if any) of the first new RDN are shifted up (away from index 0) to
		''' accommodate the new RDNs.
		''' </summary>
		''' <param name="suffixRdns">        The non-null suffix <tt>Rdn</tt>s to add. </param>
		''' <param name="posn">              The index at which to add the suffix RDNs.
		'''                          Must be in the range [0,size()].
		''' </param>
		''' <returns>  The updated name (not a new instance). </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If posn is outside the specified range. </exception>
		Public Overridable Function addAll(ByVal posn As Integer, ByVal suffixRdns As IList(Of Rdn)) As javax.naming.Name
			unparsed = Nothing
			For i As Integer = 0 To suffixRdns.Count - 1
				Dim obj As Object = suffixRdns(i)
				If Not(TypeOf obj Is Rdn) Then Throw New System.ArgumentException("Entry:" & obj & "  not a valid type;suffix list entries must be of type Rdn")
				rdns.Insert(i + posn, CType(obj, Rdn))
			Next i
			Return Me
		End Function

		''' <summary>
		''' Adds a single component to the end of this LDAP name.
		''' </summary>
		''' <param name="comp">      The non-null component to add. </param>
		''' <returns>          The updated LdapName, not a new instance.
		'''                  Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If adding comp at end of the name
		'''                  would violate the name's syntax. </exception>
		Public Overridable Function add(ByVal comp As String) As javax.naming.Name
			Return add(size(), comp)
		End Function

		''' <summary>
		''' Adds a single RDN to the end of this LDAP name.
		''' </summary>
		''' <param name="comp">      The non-null RDN to add.
		''' </param>
		''' <returns>          The updated LdapName, not a new instance.
		'''                  Cannot be null. </returns>
		Public Overridable Function add(ByVal comp As Rdn) As javax.naming.Name
			Return add(size(), comp)
		End Function

		''' <summary>
		''' Adds a single component at a specified position within this
		''' LDAP name.
		''' Components of this LDAP name at or after the index (if any) of the new
		''' component are shifted up by one (away from index 0) to accommodate
		''' the new component.
		''' </summary>
		''' <param name="comp">     The non-null component to add. </param>
		''' <param name="posn">     The index at which to add the new component.
		'''                  Must be in the range [0,size()]. </param>
		''' <returns>          The updated LdapName, not a new instance.
		'''                  Cannot be null. </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''                  If posn is outside the specified range. </exception>
		''' <exception cref="InvalidNameException"> If adding comp at the
		'''                  specified position would violate the name's syntax. </exception>
		Public Overridable Function add(ByVal posn As Integer, ByVal comp As String) As javax.naming.Name
			Dim ___rdn As Rdn = (New Rfc2253Parser(comp)).parseRdn()
			rdns.Insert(posn, ___rdn)
			unparsed = Nothing ' no longer valid
			Return Me
		End Function

		''' <summary>
		''' Adds a single RDN at a specified position within this
		''' LDAP name.
		''' RDNs of this LDAP name at or after the index (if any) of the new
		''' RDN are shifted up by one (away from index 0) to accommodate
		''' the new RDN.
		''' </summary>
		''' <param name="comp">     The non-null RDN to add. </param>
		''' <param name="posn">     The index at which to add the new RDN.
		'''                  Must be in the range [0,size()]. </param>
		''' <returns>          The updated LdapName, not a new instance.
		'''                  Cannot be null. </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''                  If posn is outside the specified range. </exception>
		Public Overridable Function add(ByVal posn As Integer, ByVal comp As Rdn) As javax.naming.Name
			If comp Is Nothing Then Throw New NullPointerException("Cannot set comp to null")
			rdns.Insert(posn, comp)
			unparsed = Nothing ' no longer valid
			Return Me
		End Function

		''' <summary>
		''' Removes a component from this LDAP name.
		''' The component of this name at the specified position is removed.
		''' Components with indexes greater than this position (if any)
		''' are shifted down (toward index 0) by one.
		''' </summary>
		''' <param name="posn">      The index of the component to remove.
		'''                  Must be in the range [0,size()). </param>
		''' <returns>          The component removed (a String).
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''                  if posn is outside the specified range. </exception>
		''' <exception cref="InvalidNameException"> if deleting the component
		'''                  would violate the syntax rules of the name. </exception>
		Public Overridable Function remove(ByVal posn As Integer) As Object
			unparsed = Nothing ' no longer valid
			Return rdns.Remove(posn).ToString()
		End Function

		''' <summary>
		''' Retrieves the list of relative distinguished names.
		''' The contents of the list are unmodifiable.
		''' The indexing of RDNs in the returned list follows the numbering of
		''' RDNs as described in the class description.
		''' If the name has zero components, an empty list is returned.
		''' </summary>
		''' <returns>  The name as a list of RDNs which are instances of
		'''          the class <seealso cref="Rdn Rdn"/>. </returns>
		Public Overridable Property rdns As IList(Of Rdn)
			Get
				Return java.util.Collections.unmodifiableList(rdns)
			End Get
		End Property

		''' <summary>
		''' Generates a new copy of this name.
		''' Subsequent changes to the components of this name will not
		''' affect the new copy, and vice versa.
		''' </summary>
		''' <returns> A copy of the this LDAP name. </returns>
		Public Overridable Function clone() As Object
			Return New LdapName(unparsed, rdns, 0, rdns.Count)
		End Function

		''' <summary>
		''' Returns a string representation of this LDAP name in a format
		''' defined by <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
		''' and described in the class description. If the name has zero
		''' components an empty string is returned.
		''' </summary>
		''' <returns> The string representation of the LdapName. </returns>
		Public Overrides Function ToString() As String
			If unparsed IsNot Nothing Then Return unparsed
			Dim builder As New StringBuilder
			Dim size As Integer = rdns.Count
			If (size - 1) >= 0 Then builder.Append(rdns(size - 1))
			For [next] As Integer = size - 2 To 0 Step -1
				builder.Append(","c)
				builder.Append(rdns([next]))
			Next [next]
			unparsed = builder.ToString()
			Return unparsed
		End Function

		''' <summary>
		''' Determines whether two LDAP names are equal.
		''' If obj is null or not an LDAP name, false is returned.
		''' <p>
		''' Two LDAP names are equal if each RDN in one is equal
		''' to the corresponding RDN in the other. This implies
		''' both have the same number of RDNs, and each RDN's
		''' equals() test against the corresponding RDN in the other
		''' name returns true. See <seealso cref="Rdn#equals(Object obj)"/>
		''' for a definition of RDN equality.
		''' </summary>
		''' <param name="obj">      The possibly null object to compare against. </param>
		''' <returns>          true if obj is equal to this LDAP name,
		'''                  false otherwise. </returns>
		''' <seealso cref= #hashCode </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			' check possible shortcuts
			If obj Is Me Then Return True
			If Not(TypeOf obj Is LdapName) Then Return False
			Dim that As LdapName = CType(obj, LdapName)
			If rdns.Count <> that.rdns.Count Then Return False
			If unparsed IsNot Nothing AndAlso unparsed.ToUpper() = that.unparsed.ToUpper() Then Return True
			' Compare RDNs one by one for equality
			For i As Integer = 0 To rdns.Count - 1
				' Compare a single pair of RDNs.
				Dim rdn1 As Rdn = rdns(i)
				Dim rdn2 As Rdn = that.rdns(i)
				If Not rdn1.Equals(rdn2) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Compares this LdapName with the specified Object for order.
		''' Returns a negative integer, zero, or a positive integer as this
		''' Name is less than, equal to, or greater than the given Object.
		''' <p>
		''' If obj is null or not an instance of LdapName, ClassCastException
		''' is thrown.
		''' <p>
		''' Ordering of LDAP names follows the lexicographical rules for
		''' string comparison, with the extension that this applies to all
		''' the RDNs in the LDAP name. All the RDNs are lined up in their
		''' specified order and compared lexicographically.
		''' See <seealso cref="Rdn#compareTo(Object obj) Rdn.compareTo(Object obj)"/>
		''' for RDN comparison rules.
		''' <p>
		''' If this LDAP name is lexicographically lesser than obj,
		''' a negative number is returned.
		''' If this LDAP name is lexicographically greater than obj,
		''' a positive number is returned. </summary>
		''' <param name="obj"> The non-null LdapName instance to compare against.
		''' </param>
		''' <returns>  A negative integer, zero, or a positive integer as this Name
		'''          is less than, equal to, or greater than the given obj. </returns>
		''' <exception cref="ClassCastException"> if obj is null or not a LdapName. </exception>
		Public Overridable Function compareTo(ByVal obj As Object) As Integer

			If Not(TypeOf obj Is LdapName) Then Throw New ClassCastException("The obj is not a LdapName")

			' check possible shortcuts
			If obj Is Me Then Return 0
			Dim that As LdapName = CType(obj, LdapName)

			If unparsed IsNot Nothing AndAlso unparsed.ToUpper() = that.unparsed.ToUpper() Then Return 0

			' Compare RDNs one by one, lexicographically.
			Dim minSize As Integer = Math.Min(rdns.Count, that.rdns.Count)
			For i As Integer = 0 To minSize - 1
				' Compare a single pair of RDNs.
				Dim rdn1 As Rdn = rdns(i)
				Dim rdn2 As Rdn = that.rdns(i)

				Dim diff As Integer = rdn1.CompareTo(rdn2)
				If diff <> 0 Then Return diff
			Next i
			Return (rdns.Count - that.rdns.Count) ' longer DN wins
		End Function

		''' <summary>
		''' Computes the hash code of this LDAP name.
		''' The hash code is the sum of the hash codes of individual RDNs
		''' of this  name.
		''' </summary>
		''' <returns> An int representing the hash code of this name. </returns>
		''' <seealso cref= #equals </seealso>
		Public Overrides Function GetHashCode() As Integer
			' Sum up the hash codes of the components.
			Dim hash As Integer = 0

			' For each RDN...
			For i As Integer = 0 To rdns.Count - 1
				Dim ___rdn As Rdn = rdns(i)
				hash += ___rdn.GetHashCode()
			Next i
			Return hash
		End Function

		''' <summary>
		''' Serializes only the unparsed DN, for compactness and to avoid
		''' any implementation dependency.
		''' 
		''' @serialData      The DN string
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			s.writeObject(ToString())
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			unparsed = CStr(s.readObject())
			Try
				parse()
			Catch e As javax.naming.InvalidNameException
				' shouldn't happen
				Throw New java.io.StreamCorruptedException("Invalid name: " & unparsed)
			End Try
		End Sub

		Private Sub parse()
			' rdns = (ArrayList<Rdn>) (new RFC2253Parser(unparsed)).getDN();

			rdns = (New Rfc2253Parser(unparsed)).parseDn()
		End Sub
	End Class

End Namespace
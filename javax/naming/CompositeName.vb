Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming


	''' <summary>
	''' This class represents a composite name -- a sequence of
	''' component names spanning multiple namespaces.
	''' Each component is a string name from the namespace of a
	''' naming system. If the component comes from a hierarchical
	''' namespace, that component can be further parsed into
	''' its atomic parts by using the CompoundName class.
	''' <p>
	''' The components of a composite name are numbered.  The indexes of a
	''' composite name with N components range from 0 up to, but not including, N.
	''' This range may be written as [0,N).
	''' The most significant component is at index 0.
	''' An empty composite name has no components.
	''' 
	''' <h1>JNDI Composite Name Syntax</h1>
	''' JNDI defines a standard string representation for composite names. This
	''' representation is the concatenation of the components of a composite name
	''' from left to right using the component separator (a forward
	''' slash character (/)) to separate each component.
	''' The JNDI syntax defines the following meta characters:
	''' <ul>
	''' <li>escape (backward slash \),
	''' <li>quote characters  (single (') and double quotes (")), and
	''' <li>component separator (forward slash character (/)).
	''' </ul>
	''' Any occurrence of a leading quote, an escape preceding any meta character,
	''' an escape at the end of a component, or a component separator character
	''' in an unquoted component must be preceded by an escape character when
	''' that component is being composed into a composite name string.
	''' Alternatively, to avoid adding escape characters as described,
	''' the entire component can be quoted using matching single quotes
	''' or matching double quotes. A single quote occurring within a double-quoted
	''' component is not considered a meta character (and need not be escaped),
	''' and vice versa.
	''' <p>
	''' When two composite names are compared, the case of the characters
	''' is significant.
	''' <p>
	''' A leading component separator (the composite name string begins with
	''' a separator) denotes a leading empty component (a component consisting
	''' of an empty string).
	''' A trailing component separator (the composite name string ends with
	''' a separator) denotes a trailing empty component.
	''' Adjacent component separators denote an empty component.
	''' 
	''' <h1>Composite Name Examples</h1>
	''' This table shows examples of some composite names. Each row shows
	''' the string form of a composite name and its corresponding structural form
	''' (<tt>CompositeName</tt>).
	''' 
	''' <table border="1" cellpadding=3 summary="examples showing string form of composite name and its corresponding structural form (CompositeName)">
	''' 
	''' <tr>
	''' <th>String Name</th>
	''' <th>CompositeName</th>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' ""
	''' </td>
	''' <td>{} (the empty name == new CompositeName("") == new CompositeName())
	''' </td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' "x"
	''' </td>
	''' <td>{"x"}
	''' </td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' "x/y"
	''' </td>
	''' <td>{"x", "y"}</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>"x/"</td>
	''' <td>{"x", ""}</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>"/x"</td>
	''' <td>{"", "x"}</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>"/"</td>
	''' <td>{""}</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>"//"</td>
	''' <td>{"", ""}</td>
	''' </tr>
	''' 
	''' <tr><td>"/x/"</td>
	''' <td>{"", "x", ""}</td>
	''' </tr>
	''' 
	''' <tr><td>"x//y"</td>
	''' <td>{"x", "", "y"}</td>
	''' </tr>
	''' </table>
	''' 
	''' <h1>Composition Examples</h1>
	''' Here are some composition examples.  The right column shows composing
	''' string composite names while the left column shows composing the
	''' corresponding <tt>CompositeName</tt>s.  Notice that composing the
	''' string forms of two composite names simply involves concatenating
	''' their string forms together.
	''' 
	''' <table border="1" cellpadding=3 summary="composition examples showing string names and composite names">
	''' 
	''' <tr>
	''' <th>String Names</th>
	''' <th>CompositeNames</th>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' "x/y"           + "/"   = x/y/
	''' </td>
	''' <td>
	''' {"x", "y"}      + {""}  = {"x", "y", ""}
	''' </td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' ""              + "x"   = "x"
	''' </td>
	''' <td>
	''' {}              + {"x"} = {"x"}
	''' </td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' "/"             + "x"   = "/x"
	''' </td>
	''' <td>
	''' {""}            + {"x"} = {"", "x"}
	''' </td>
	''' </tr>
	''' 
	''' <tr>
	''' <td>
	''' "x"   + ""      + ""    = "x"
	''' </td>
	''' <td>
	''' {"x"} + {}      + {}    = {"x"}
	''' </td>
	''' </tr>
	''' 
	''' </table>
	''' 
	''' <h1>Multithreaded Access</h1>
	''' A <tt>CompositeName</tt> instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify a
	''' <tt>CompositeName</tt> should lock the object.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>


	Public Class CompositeName
		Implements Name

		<NonSerialized> _
		Private impl As NameImpl
		''' <summary>
		''' Constructs a new composite name instance using the components
		''' specified by 'comps'. This protected method is intended to be
		''' to be used by subclasses of CompositeName when they override
		''' methods such as clone(), getPrefix(), getSuffix().
		''' </summary>
		''' <param name="comps"> A non-null enumeration containing the components for the new
		'''              composite name. Each element is of class String.
		'''               The enumeration will be consumed to extract its
		'''               elements. </param>
		Protected Friend Sub New(ByVal comps As System.Collections.IEnumerator(Of String))
			impl = New NameImpl(Nothing, comps) ' null means use default syntax
		End Sub

		''' <summary>
		''' Constructs a new composite name instance by parsing the string n
		''' using the composite name syntax (left-to-right, slash separated).
		''' The composite name syntax is described in detail in the class
		''' description.
		''' </summary>
		''' <param name="n">       The non-null string to parse. </param>
		''' <exception cref="InvalidNameException"> If n has invalid composite name syntax. </exception>
		Public Sub New(ByVal n As String)
			impl = New NameImpl(Nothing, n) ' null means use default syntax
		End Sub

		''' <summary>
		''' Constructs a new empty composite name. Such a name returns true
		''' when <code>isEmpty()</code> is invoked on it.
		''' </summary>
		Public Sub New()
			impl = New NameImpl(Nothing) ' null means use default syntax
		End Sub

		''' <summary>
		''' Generates the string representation of this composite name.
		''' The string representation consists of enumerating in order
		''' each component of the composite name and separating
		''' each component by a forward slash character. Quoting and
		''' escape characters are applied where necessary according to
		''' the JNDI syntax, which is described in the class description.
		''' An empty component is represented by an empty string.
		'''  
		''' The string representation thus generated can be passed to
		''' the CompositeName constructor to create a new equivalent
		''' composite name.
		''' </summary>
		''' <returns> A non-null string representation of this composite name. </returns>
		Public Overrides Function ToString() As String
			Return impl.ToString()
		End Function

		''' <summary>
		''' Determines whether two composite names are equal.
		''' If obj is null or not a composite name, false is returned.
		''' Two composite names are equal if each component in one is equal
		''' to the corresponding component in the other. This implies
		''' both have the same number of components, and each component's
		''' equals() test against the corresponding component in the other name
		''' returns true.
		''' </summary>
		''' <param name="obj">     The possibly null object to compare against. </param>
		''' <returns> true if obj is equal to this composite name, false otherwise. </returns>
		''' <seealso cref= #hashCode </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return (obj IsNot Nothing AndAlso TypeOf obj Is CompositeName AndAlso impl.Equals(CType(obj, CompositeName).impl))
		End Function

		''' <summary>
		''' Computes the hash code of this composite name.
		''' The hash code is the sum of the hash codes of individual components
		''' of this composite name.
		''' </summary>
		''' <returns> An int representing the hash code of this name. </returns>
		''' <seealso cref= #equals </seealso>
		Public Overrides Function GetHashCode() As Integer
			Return impl.GetHashCode()
		End Function


		''' <summary>
		''' Compares this CompositeName with the specified Object for order.
		''' Returns a
		''' negative integer, zero, or a positive integer as this Name is less
		''' than, equal to, or greater than the given Object.
		''' <p>
		''' If obj is null or not an instance of CompositeName, ClassCastException
		''' is thrown.
		''' <p>
		''' See equals() for what it means for two composite names to be equal.
		''' If two composite names are equal, 0 is returned.
		''' <p>
		''' Ordering of composite names follows the lexicographical rules for
		''' string comparison, with the extension that this applies to all
		''' the components in the composite name. The effect is as if all the
		''' components were lined up in their specified ordered and the
		''' lexicographical rules applied over the two line-ups.
		''' If this composite name is "lexicographically" lesser than obj,
		''' a negative number is returned.
		''' If this composite name is "lexicographically" greater than obj,
		''' a positive number is returned. </summary>
		''' <param name="obj"> The non-null object to compare against.
		''' </param>
		''' <returns>  a negative integer, zero, or a positive integer as this Name
		'''          is less than, equal to, or greater than the given Object. </returns>
		''' <exception cref="ClassCastException"> if obj is not a CompositeName. </exception>
		Public Overridable Function compareTo(ByVal obj As Object) As Integer Implements Name.compareTo
			If Not(TypeOf obj Is CompositeName) Then Throw New ClassCastException("Not a CompositeName")
			Return impl.CompareTo(CType(obj, CompositeName).impl)
		End Function

		''' <summary>
		''' Generates a copy of this composite name.
		''' Changes to the components of this composite name won't
		''' affect the new copy and vice versa.
		''' </summary>
		''' <returns> A non-null copy of this composite name. </returns>
		Public Overridable Function clone() As Object Implements Name.clone
			Return (New CompositeName(all))
		End Function

		''' <summary>
		''' Retrieves the number of components in this composite name.
		''' </summary>
		''' <returns> The nonnegative number of components in this composite name. </returns>
		Public Overridable Function size() As Integer Implements Name.size
			Return (impl.size())
		End Function

		''' <summary>
		''' Determines whether this composite name is empty. A composite name
		''' is empty if it has zero components.
		''' </summary>
		''' <returns> true if this composite name is empty, false otherwise. </returns>
		Public Overridable Property empty As Boolean Implements Name.isEmpty
			Get
				Return (impl.empty)
			End Get
		End Property

		''' <summary>
		''' Retrieves the components of this composite name as an enumeration
		''' of strings.
		''' The effects of updates to this composite name on this enumeration
		''' is undefined.
		''' </summary>
		''' <returns> A non-null enumeration of the components of
		'''         this composite name. Each element of the enumeration is of
		'''         class String. </returns>
		Public Overridable Property all As System.Collections.IEnumerator(Of String) Implements Name.getAll
			Get
				Return (impl.all)
			End Get
		End Property

		''' <summary>
		''' Retrieves a component of this composite name.
		''' </summary>
		''' <param name="posn">    The 0-based index of the component to retrieve.
		'''                 Must be in the range [0,size()). </param>
		''' <returns> The non-null component at index posn. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if posn is outside the
		'''         specified range. </exception>
		Public Overridable Function [get](ByVal posn As Integer) As String Implements Name.get
			Return (impl.get(posn))
		End Function

		''' <summary>
		''' Creates a composite name whose components consist of a prefix of the
		''' components in this composite name. Subsequent changes to
		''' this composite name does not affect the name that is returned.
		''' </summary>
		''' <param name="posn">    The 0-based index of the component at which to stop.
		'''                 Must be in the range [0,size()]. </param>
		''' <returns> A composite name consisting of the components at indexes in
		'''         the range [0,posn). </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		Public Overridable Function getPrefix(ByVal posn As Integer) As Name Implements Name.getPrefix
			Dim comps As System.Collections.IEnumerator(Of String) = impl.getPrefix(posn)
			Return (New CompositeName(comps))
		End Function

		''' <summary>
		''' Creates a composite name whose components consist of a suffix of the
		''' components in this composite name. Subsequent changes to
		''' this composite name does not affect the name that is returned.
		''' </summary>
		''' <param name="posn">    The 0-based index of the component at which to start.
		'''                 Must be in the range [0,size()]. </param>
		''' <returns> A composite name consisting of the components at indexes in
		'''         the range [posn,size()).  If posn is equal to
		'''         size(), an empty composite name is returned. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		Public Overridable Function getSuffix(ByVal posn As Integer) As Name Implements Name.getSuffix
			Dim comps As System.Collections.IEnumerator(Of String) = impl.getSuffix(posn)
			Return (New CompositeName(comps))
		End Function

		''' <summary>
		''' Determines whether a composite name is a prefix of this composite name.
		''' A composite name 'n' is a prefix if it is equal to
		''' getPrefix(n.size())--in other words, this composite name
		''' starts with 'n'. If 'n' is null or not a composite name, false is returned.
		''' </summary>
		''' <param name="n">       The possibly null name to check. </param>
		''' <returns> true if n is a CompositeName and
		'''         is a prefix of this composite name, false otherwise. </returns>
		Public Overridable Function startsWith(ByVal n As Name) As Boolean Implements Name.startsWith
			If TypeOf n Is CompositeName Then
				Return (impl.StartsWith(n.size(), n.all))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Determines whether a composite name is a suffix of this composite name.
		''' A composite name 'n' is a suffix if it it is equal to
		''' getSuffix(size()-n.size())--in other words, this
		''' composite name ends with 'n'.
		''' If n is null or not a composite name, false is returned.
		''' </summary>
		''' <param name="n">       The possibly null name to check. </param>
		''' <returns> true if n is a CompositeName and
		'''         is a suffix of this composite name, false otherwise. </returns>
		Public Overridable Function endsWith(ByVal n As Name) As Boolean Implements Name.endsWith
			If TypeOf n Is CompositeName Then
				Return (impl.EndsWith(n.size(), n.all))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Adds the components of a composite name -- in order -- to the end of
		''' this composite name.
		''' </summary>
		''' <param name="suffix">   The non-null components to add. </param>
		''' <returns> The updated CompositeName, not a new one. Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If suffix is not a composite name. </exception>
		Public Overridable Function addAll(ByVal suffix As Name) As Name Implements Name.addAll
			If TypeOf suffix Is CompositeName Then
				impl.addAll(suffix.all)
				Return Me
			Else
				Throw New InvalidNameException("Not a composite name: " & suffix.ToString())
			End If
		End Function

		''' <summary>
		''' Adds the components of a composite name -- in order -- at a specified
		''' position within this composite name.
		''' Components of this composite name at or after the index of the first
		''' new component are shifted up (away from index 0)
		''' to accommodate the new components.
		''' </summary>
		''' <param name="n">        The non-null components to add. </param>
		''' <param name="posn">     The index in this name at which to add the new
		'''                 components.  Must be in the range [0,size()]. </param>
		''' <returns> The updated CompositeName, not a new one. Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If n is not a composite name. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		Public Overridable Function addAll(ByVal posn As Integer, ByVal n As Name) As Name Implements Name.addAll
			If TypeOf n Is CompositeName Then
				impl.addAll(posn, n.all)
				Return Me
			Else
				Throw New InvalidNameException("Not a composite name: " & n.ToString())
			End If
		End Function

		''' <summary>
		''' Adds a single component to the end of this composite name.
		''' </summary>
		''' <param name="comp">     The non-null component to add. </param>
		''' <returns> The updated CompositeName, not a new one. Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If adding comp at end of the name
		'''                         would violate the name's syntax. </exception>
		Public Overridable Function add(ByVal comp As String) As Name Implements Name.add
			impl.add(comp)
			Return Me
		End Function

		''' <summary>
		''' Adds a single component at a specified position within this
		''' composite name.
		''' Components of this composite name at or after the index of the new
		''' component are shifted up by one (away from index 0) to accommodate
		''' the new component.
		''' </summary>
		''' <param name="comp">    The non-null component to add. </param>
		''' <param name="posn">    The index at which to add the new component.
		'''                 Must be in the range [0,size()]. </param>
		''' <returns> The updated CompositeName, not a new one. Cannot be null. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		''' <exception cref="InvalidNameException"> If adding comp at the specified position
		'''                         would violate the name's syntax. </exception>
		Public Overridable Function add(ByVal posn As Integer, ByVal comp As String) As Name Implements Name.add
			impl.add(posn, comp)
			Return Me
		End Function

		''' <summary>
		''' Deletes a component from this composite name.
		''' The component of this composite name at position 'posn' is removed,
		''' and components at indices greater than 'posn'
		''' are shifted down (towards index 0) by one.
		''' </summary>
		''' <param name="posn">    The index of the component to delete.
		'''                 Must be in the range [0,size()). </param>
		''' <returns> The component removed (a String). </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range (includes case where
		'''         composite name is empty). </exception>
		''' <exception cref="InvalidNameException"> If deleting the component
		'''                         would violate the name's syntax. </exception>
		Public Overridable Function remove(ByVal posn As Integer) As Object Implements Name.remove
			Return impl.remove(posn)
		End Function

		''' <summary>
		''' Overridden to avoid implementation dependency.
		''' @serialData The number of components (an <tt>int</tt>) followed by
		''' the individual components (each a <tt>String</tt>).
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.writeInt(size())
			Dim comps As System.Collections.IEnumerator(Of String) = all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While comps.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				s.writeObject(comps.nextElement())
			Loop
		End Sub

		''' <summary>
		''' Overridden to avoid implementation dependency.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			impl = New NameImpl(Nothing) ' null means use default syntax
			Dim n As Integer = s.readInt() ' number of components
			Try
				n -= 1
				Do While n >= 0
					add(CStr(s.readObject()))
					n -= 1
				Loop
			Catch e As InvalidNameException
				Throw (New java.io.StreamCorruptedException("Invalid name"))
			End Try
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 1667768148915813118L

	'
	'    // %%% Test code for serialization.
	'    public static void main(String[] args) throws Exception {
	'        CompositeName c = new CompositeName("aaa/bbb");
	'        java.io.FileOutputStream f1 = new java.io.FileOutputStream("/tmp/ser");
	'        java.io.ObjectOutputStream s1 = new java.io.ObjectOutputStream(f1);
	'        s1.writeObject(c);
	'        s1.close();
	'        java.io.FileInputStream f2 = new java.io.FileInputStream("/tmp/ser");
	'        java.io.ObjectInputStream s2 = new java.io.ObjectInputStream(f2);
	'        c = (CompositeName)s2.readObject();
	'
	'        System.out.println("Size: " + c.size());
	'        System.out.println("Size: " + c.snit);
	'    }
	'

	'
	'   %%% Testing code
	'    public static void main(String[] args) {
	'        try {
	'            for (int i = 0; i < args.length; i++) {
	'                Name name;
	'                Enumeration e;
	'                System.out.println("Given name: " + args[i]);
	'                name = new CompositeName(args[i]);
	'                e = name.getComponents();
	'                while (e.hasMoreElements()) {
	'                    System.out.println("Element: " + e.nextElement());
	'                }
	'                System.out.println("Constructed name: " + name.toString());
	'            }
	'        } catch (Exception ne) {
	'            ne.printStackTrace();
	'        }
	'    }
	'
	End Class

End Namespace
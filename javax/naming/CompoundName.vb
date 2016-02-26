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
	''' This class represents a compound name -- a name from
	''' a hierarchical name space.
	''' Each component in a compound name is an atomic name.
	''' <p>
	''' The components of a compound name are numbered.  The indexes of a
	''' compound name with N components range from 0 up to, but not including, N.
	''' This range may be written as [0,N).
	''' The most significant component is at index 0.
	''' An empty compound name has no components.
	''' 
	''' <h1>Compound Name Syntax</h1>
	''' The syntax of a compound name is specified using a set of properties:
	''' <dl>
	'''  <dt>jndi.syntax.direction
	'''  <dd>Direction for parsing ("right_to_left", "left_to_right", "flat").
	'''      If unspecified, defaults to "flat", which means the namespace is flat
	'''      with no hierarchical structure.
	''' 
	'''  <dt>jndi.syntax.separator
	'''  <dd>Separator between atomic name components.
	'''      Required unless direction is "flat".
	''' 
	'''  <dt>jndi.syntax.ignorecase
	'''  <dd>If present, "true" means ignore the case when comparing name
	'''      components. If its value is not "true", or if the property is not
	'''      present, case is considered when comparing name components.
	''' 
	'''  <dt>jndi.syntax.escape
	'''  <dd>If present, specifies the escape string for overriding separator,
	'''      escapes and quotes.
	''' 
	'''  <dt>jndi.syntax.beginquote
	'''  <dd>If present, specifies the string delimiting start of a quoted string.
	''' 
	'''  <dt>jndi.syntax.endquote
	'''  <dd>String delimiting end of quoted string.
	'''      If present, specifies the string delimiting the end of a quoted string.
	'''      If not present, use syntax.beginquote as end quote.
	'''  <dt>jndi.syntax.beginquote2
	'''  <dd>Alternative set of begin/end quotes.
	''' 
	'''  <dt>jndi.syntax.endquote2
	'''  <dd>Alternative set of begin/end quotes.
	''' 
	'''  <dt>jndi.syntax.trimblanks
	'''  <dd>If present, "true" means trim any leading and trailing whitespaces
	'''      in a name component for comparison purposes. If its value is not
	'''      "true", or if the property is not present, blanks are significant.
	'''  <dt>jndi.syntax.separator.ava
	'''  <dd>If present, specifies the string that separates
	'''      attribute-value-assertions when specifying multiple attribute/value
	'''      pairs. (e.g. ","  in age=65,gender=male).
	'''  <dt>jndi.syntax.separator.typeval
	'''  <dd>If present, specifies the string that separators attribute
	'''              from value (e.g. "=" in "age=65")
	''' </dl>
	''' These properties are interpreted according to the following rules:
	''' <ol>
	''' <li>
	''' In a string without quotes or escapes, any instance of the
	''' separator delimits two atomic names. Each atomic name is referred
	''' to as a <em>component</em>.
	''' <li>
	''' A separator, quote or escape is escaped if preceded immediately
	''' (on the left) by the escape.
	''' <li>
	''' If there are two sets of quotes, a specific begin-quote must be matched
	''' by its corresponding end-quote.
	''' <li>
	''' A non-escaped begin-quote which precedes a component must be
	''' matched by a non-escaped end-quote at the end of the component.
	''' A component thus quoted is referred to as a
	''' <em>quoted component</em>. It is parsed by
	''' removing the being- and end- quotes, and by treating the intervening
	''' characters as ordinary characters unless one of the rules involving
	''' quoted components listed below applies.
	''' <li>
	''' Quotes embedded in non-quoted components are treated as ordinary strings
	''' and need not be matched.
	''' <li>
	''' A separator that is escaped or appears between non-escaped
	''' quotes is treated as an ordinary string and not a separator.
	''' <li>
	''' An escape string within a quoted component acts as an escape only when
	''' followed by the corresponding end-quote string.
	''' This can be used to embed an escaped quote within a quoted component.
	''' <li>
	''' An escaped escape string is not treated as an escape string.
	''' <li>
	''' An escape string that does not precede a meta string (quotes or separator)
	''' and is not at the end of a component is treated as an ordinary string.
	''' <li>
	''' A leading separator (the compound name string begins with
	''' a separator) denotes a leading empty atomic component (consisting
	''' of an empty string).
	''' A trailing separator (the compound name string ends with
	''' a separator) denotes a trailing empty atomic component.
	''' Adjacent separators denote an empty atomic component.
	''' </ol>
	''' <p>
	''' The string form of the compound name follows the syntax described above.
	''' When the components of the compound name are turned into their
	''' string representation, the reserved syntax rules described above are
	''' applied (e.g. embedded separators are escaped or quoted)
	''' so that when the same string is parsed, it will yield the same components
	''' of the original compound name.
	''' 
	''' <h1>Multithreaded Access</h1>
	''' A <tt>CompoundName</tt> instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify a
	''' <tt>CompoundName</tt> should lock the object.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class CompoundName
		Implements Name

		''' <summary>
		''' Implementation of this compound name.
		''' This field is initialized by the constructors and cannot be null.
		''' It should be treated as a read-only variable by subclasses.
		''' </summary>
		<NonSerialized> _
		Protected Friend impl As NameImpl
		''' <summary>
		''' Syntax properties for this compound name.
		''' This field is initialized by the constructors and cannot be null.
		''' It should be treated as a read-only variable by subclasses.
		''' Any necessary changes to mySyntax should be made within constructors
		''' and not after the compound name has been instantiated.
		''' </summary>
		<NonSerialized> _
		Protected Friend mySyntax As java.util.Properties

		''' <summary>
		''' Constructs a new compound name instance using the components
		''' specified in comps and syntax. This protected method is intended to be
		''' to be used by subclasses of CompoundName when they override
		''' methods such as clone(), getPrefix(), getSuffix().
		''' </summary>
		''' <param name="comps">  A non-null enumeration of the components to add.
		'''   Each element of the enumeration is of class String.
		'''               The enumeration will be consumed to extract its
		'''               elements. </param>
		''' <param name="syntax">   A non-null properties that specify the syntax of
		'''                 this compound name. See class description for
		'''                 contents of properties. </param>
		Protected Friend Sub New(ByVal comps As System.Collections.IEnumerator(Of String), ByVal syntax As java.util.Properties)
			If syntax Is Nothing Then Throw New NullPointerException
			mySyntax = syntax
			impl = New NameImpl(syntax, comps)
		End Sub

		''' <summary>
		''' Constructs a new compound name instance by parsing the string n
		''' using the syntax specified by the syntax properties supplied.
		''' </summary>
		''' <param name="n">       The non-null string to parse. </param>
		''' <param name="syntax">   A non-null list of properties that specify the syntax of
		'''                 this compound name.  See class description for
		'''                 contents of properties. </param>
		''' <exception cref="InvalidNameException"> If 'n' violates the syntax specified
		'''                 by <code>syntax</code>. </exception>
		Public Sub New(ByVal n As String, ByVal syntax As java.util.Properties)
			If syntax Is Nothing Then Throw New NullPointerException
			mySyntax = syntax
			impl = New NameImpl(syntax, n)
		End Sub

		''' <summary>
		''' Generates the string representation of this compound name, using
		''' the syntax rules of the compound name. The syntax rules
		''' are described in the class description.
		''' An empty component is represented by an empty string.
		'''  
		''' The string representation thus generated can be passed to
		''' the CompoundName constructor with the same syntax properties
		''' to create a new equivalent compound name.
		''' </summary>
		''' <returns> A non-null string representation of this compound name. </returns>
		Public Overrides Function ToString() As String
			Return (impl.ToString())
		End Function

		''' <summary>
		''' Determines whether obj is syntactically equal to this compound name.
		''' If obj is null or not a CompoundName, false is returned.
		''' Two compound names are equal if each component in one is "equal"
		''' to the corresponding component in the other.
		''' <p>
		''' Equality is also defined in terms of the syntax of this compound name.
		''' The default implementation of CompoundName uses the syntax properties
		''' jndi.syntax.ignorecase and jndi.syntax.trimblanks when comparing
		''' two components for equality.  If case is ignored, two strings
		''' with the same sequence of characters but with different cases
		''' are considered equal. If blanks are being trimmed, leading and trailing
		''' blanks are ignored for the purpose of the comparison.
		''' <p>
		''' Both compound names must have the same number of components.
		''' <p>
		''' Implementation note: Currently the syntax properties of the two compound
		''' names are not compared for equality. They might be in the future.
		''' </summary>
		''' <param name="obj">     The possibly null object to compare against. </param>
		''' <returns> true if obj is equal to this compound name, false otherwise. </returns>
		''' <seealso cref= #compareTo(java.lang.Object obj) </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			' %%% check syntax too?
			Return (obj IsNot Nothing AndAlso TypeOf obj Is CompoundName AndAlso impl.Equals(CType(obj, CompoundName).impl))
		End Function

		''' <summary>
		''' Computes the hash code of this compound name.
		''' The hash code is the sum of the hash codes of the "canonicalized"
		''' forms of individual components of this compound name.
		''' Each component is "canonicalized" according to the
		''' compound name's syntax before its hash code is computed.
		''' For a case-insensitive name, for example, the uppercased form of
		''' a name has the same hash code as its lowercased equivalent.
		''' </summary>
		''' <returns> An int representing the hash code of this name. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return impl.GetHashCode()
		End Function

		''' <summary>
		''' Creates a copy of this compound name.
		''' Changes to the components of this compound name won't
		''' affect the new copy and vice versa.
		''' The clone and this compound name share the same syntax.
		''' </summary>
		''' <returns> A non-null copy of this compound name. </returns>
		Public Overridable Function clone() As Object Implements Name.clone
			Return (New CompoundName(all, mySyntax))
		End Function

		''' <summary>
		''' Compares this CompoundName with the specified Object for order.
		''' Returns a
		''' negative integer, zero, or a positive integer as this Name is less
		''' than, equal to, or greater than the given Object.
		''' <p>
		''' If obj is null or not an instance of CompoundName, ClassCastException
		''' is thrown.
		''' <p>
		''' See equals() for what it means for two compound names to be equal.
		''' If two compound names are equal, 0 is returned.
		''' <p>
		''' Ordering of compound names depend on the syntax of the compound name.
		''' By default, they follow lexicographical rules for string comparison
		''' with the extension that this applies to all the components in the
		''' compound name and that comparison of individual components is
		''' affected by the jndi.syntax.ignorecase and jndi.syntax.trimblanks
		''' properties, identical to how they affect equals().
		''' If this compound name is "lexicographically" lesser than obj,
		''' a negative number is returned.
		''' If this compound name is "lexicographically" greater than obj,
		''' a positive number is returned.
		''' <p>
		''' Implementation note: Currently the syntax properties of the two compound
		''' names are not compared when checking order. They might be in the future. </summary>
		''' <param name="obj">     The non-null object to compare against. </param>
		''' <returns>  a negative integer, zero, or a positive integer as this Name
		'''          is less than, equal to, or greater than the given Object. </returns>
		''' <exception cref="ClassCastException"> if obj is not a CompoundName. </exception>
		''' <seealso cref= #equals(java.lang.Object) </seealso>
		Public Overridable Function compareTo(ByVal obj As Object) As Integer Implements Name.compareTo
			If Not(TypeOf obj Is CompoundName) Then Throw New ClassCastException("Not a CompoundName")
			Return impl.CompareTo(CType(obj, CompoundName).impl)
		End Function

		''' <summary>
		''' Retrieves the number of components in this compound name.
		''' </summary>
		''' <returns> The nonnegative number of components in this compound name. </returns>
		Public Overridable Function size() As Integer Implements Name.size
			Return (impl.size())
		End Function

		''' <summary>
		''' Determines whether this compound name is empty.
		''' A compound name is empty if it has zero components.
		''' </summary>
		''' <returns> true if this compound name is empty, false otherwise. </returns>
	 ReadOnly	Public Overridable Property empty As Boolean Implements Name.isEmpty
			Get
				Return (impl.empty)
			End Get
		End Property

		''' <summary>
		''' Retrieves the components of this compound name as an enumeration
		''' of strings.
		''' The effects of updates to this compound name on this enumeration
		''' is undefined.
		''' </summary>
		''' <returns> A non-null enumeration of the components of this
		''' compound name. Each element of the enumeration is of class String. </returns>
	ReadOnly	Public Overridable Property all As System.Collections.IEnumerator(Of String) Implements Name.getAll
			Get
				Return (impl.all)
			End Get
		End Property

		''' <summary>
		''' Retrieves a component of this compound name.
		''' </summary>
		''' <param name="posn">    The 0-based index of the component to retrieve.
		'''                 Must be in the range [0,size()). </param>
		''' <returns> The component at index posn. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if posn is outside the
		'''         specified range. </exception>
		Public Overridable Function [get](ByVal posn As Integer) As String Implements Name.get
			Return (impl.get(posn))
		End Function

		''' <summary>
		''' Creates a compound name whose components consist of a prefix of the
		''' components in this compound name.
		''' The result and this compound name share the same syntax.
		''' Subsequent changes to
		''' this compound name does not affect the name that is returned and
		''' vice versa.
		''' </summary>
		''' <param name="posn">    The 0-based index of the component at which to stop.
		'''                 Must be in the range [0,size()]. </param>
		''' <returns> A compound name consisting of the components at indexes in
		'''         the range [0,posn). </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		Public Overridable Function getPrefix(ByVal posn As Integer) As Name Implements Name.getPrefix
			Dim comps As System.Collections.IEnumerator(Of String) = impl.getPrefix(posn)
			Return (New CompoundName(comps, mySyntax))
		End Function

		''' <summary>
		''' Creates a compound name whose components consist of a suffix of the
		''' components in this compound name.
		''' The result and this compound name share the same syntax.
		''' Subsequent changes to
		''' this compound name does not affect the name that is returned.
		''' </summary>
		''' <param name="posn">    The 0-based index of the component at which to start.
		'''                 Must be in the range [0,size()]. </param>
		''' <returns> A compound name consisting of the components at indexes in
		'''         the range [posn,size()).  If posn is equal to
		'''         size(), an empty compound name is returned. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		Public Overridable Function getSuffix(ByVal posn As Integer) As Name Implements Name.getSuffix
			Dim comps As System.Collections.IEnumerator(Of String) = impl.getSuffix(posn)
			Return (New CompoundName(comps, mySyntax))
		End Function

		''' <summary>
		''' Determines whether a compound name is a prefix of this compound name.
		''' A compound name 'n' is a prefix if it is equal to
		''' getPrefix(n.size())--in other words, this compound name
		''' starts with 'n'.
		''' If n is null or not a compound name, false is returned.
		''' <p>
		''' Implementation note: Currently the syntax properties of n
		'''  are not used when doing the comparison. They might be in the future. </summary>
		''' <param name="n">       The possibly null compound name to check. </param>
		''' <returns> true if n is a CompoundName and
		'''                 is a prefix of this compound name, false otherwise. </returns>
		Public Overridable Function startsWith(ByVal n As Name) As Boolean Implements Name.startsWith
			If TypeOf n Is CompoundName Then
				Return (impl.StartsWith(n.size(), n.all))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Determines whether a compound name is a suffix of this compound name.
		''' A compound name 'n' is a suffix if it it is equal to
		''' getSuffix(size()-n.size())--in other words, this
		''' compound name ends with 'n'.
		''' If n is null or not a compound name, false is returned.
		''' <p>
		''' Implementation note: Currently the syntax properties of n
		'''  are not used when doing the comparison. They might be in the future. </summary>
		''' <param name="n">       The possibly null compound name to check. </param>
		''' <returns> true if n is a CompoundName and
		'''         is a suffix of this compound name, false otherwise. </returns>
		Public Overridable Function endsWith(ByVal n As Name) As Boolean Implements Name.endsWith
			If TypeOf n Is CompoundName Then
				Return (impl.EndsWith(n.size(), n.all))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Adds the components of a compound name -- in order -- to the end of
		''' this compound name.
		''' <p>
		''' Implementation note: Currently the syntax properties of suffix
		'''  is not used or checked. They might be in the future. </summary>
		''' <param name="suffix">   The non-null components to add. </param>
		''' <returns> The updated CompoundName, not a new one. Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If suffix is not a compound name,
		'''            or if the addition of the components violates the syntax
		'''            of this compound name (e.g. exceeding number of components). </exception>
		Public Overridable Function addAll(ByVal suffix As Name) As Name Implements Name.addAll
			If TypeOf suffix Is CompoundName Then
				impl.addAll(suffix.all)
				Return Me
			Else
				Throw New InvalidNameException("Not a compound name: " & suffix.ToString())
			End If
		End Function

		''' <summary>
		''' Adds the components of a compound name -- in order -- at a specified
		''' position within this compound name.
		''' Components of this compound name at or after the index of the first
		''' new component are shifted up (away from index 0)
		''' to accommodate the new components.
		''' <p>
		''' Implementation note: Currently the syntax properties of suffix
		'''  is not used or checked. They might be in the future.
		''' </summary>
		''' <param name="n">        The non-null components to add. </param>
		''' <param name="posn">     The index in this name at which to add the new
		'''                 components.  Must be in the range [0,size()]. </param>
		''' <returns> The updated CompoundName, not a new one. Cannot be null. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		''' <exception cref="InvalidNameException"> If n is not a compound name,
		'''            or if the addition of the components violates the syntax
		'''            of this compound name (e.g. exceeding number of components). </exception>
		Public Overridable Function addAll(ByVal posn As Integer, ByVal n As Name) As Name Implements Name.addAll
			If TypeOf n Is CompoundName Then
				impl.addAll(posn, n.all)
				Return Me
			Else
				Throw New InvalidNameException("Not a compound name: " & n.ToString())
			End If
		End Function

		''' <summary>
		''' Adds a single component to the end of this compound name.
		''' </summary>
		''' <param name="comp">     The non-null component to add. </param>
		''' <returns> The updated CompoundName, not a new one. Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If adding comp at end of the name
		'''                         would violate the compound name's syntax. </exception>
		Public Overridable Function add(ByVal comp As String) As Name Implements Name.add
			impl.add(comp)
			Return Me
		End Function

		''' <summary>
		''' Adds a single component at a specified position within this
		''' compound name.
		''' Components of this compound name at or after the index of the new
		''' component are shifted up by one (away from index 0)
		''' to accommodate the new component.
		''' </summary>
		''' <param name="comp">    The non-null component to add. </param>
		''' <param name="posn">    The index at which to add the new component.
		'''                 Must be in the range [0,size()]. </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range. </exception>
		''' <returns> The updated CompoundName, not a new one. Cannot be null. </returns>
		''' <exception cref="InvalidNameException"> If adding comp at the specified position
		'''                         would violate the compound name's syntax. </exception>
		Public Overridable Function add(ByVal posn As Integer, ByVal comp As String) As Name Implements Name.add
			impl.add(posn, comp)
			Return Me
		End Function

		''' <summary>
		''' Deletes a component from this compound name.
		''' The component of this compound name at position 'posn' is removed,
		''' and components at indices greater than 'posn'
		''' are shifted down (towards index 0) by one.
		''' </summary>
		''' <param name="posn">    The index of the component to delete.
		'''                 Must be in the range [0,size()). </param>
		''' <returns> The component removed (a String). </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         If posn is outside the specified range (includes case where
		'''         compound name is empty). </exception>
		''' <exception cref="InvalidNameException"> If deleting the component
		'''                         would violate the compound name's syntax. </exception>
		Public Overridable Function remove(ByVal posn As Integer) As Object Implements Name.remove
			Return impl.remove(posn)
		End Function

		''' <summary>
		''' Overridden to avoid implementation dependency.
		''' @serialData The syntax <tt>Properties</tt>, followed by
		''' the number of components (an <tt>int</tt>), and the individual
		''' components (each a <tt>String</tt>).
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.writeObject(mySyntax)
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
			mySyntax = CType(s.readObject(), java.util.Properties)
			impl = New NameImpl(mySyntax)
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
		Private Const serialVersionUID As Long = 3513100557083972036L

	'
	'//   For testing
	'
	'    public static void main(String[] args) {
	'        Properties dotSyntax = new Properties();
	'        dotSyntax.put("jndi.syntax.direction", "right_to_left");
	'        dotSyntax.put("jndi.syntax.separator", ".");
	'        dotSyntax.put("jndi.syntax.ignorecase", "true");
	'        dotSyntax.put("jndi.syntax.escape", "\\");
	'//      dotSyntax.put("jndi.syntax.beginquote", "\"");
	'//      dotSyntax.put("jndi.syntax.beginquote2", "'");
	'
	'        Name first = null;
	'        try {
	'            for (int i = 0; i < args.length; i++) {
	'                Name name;
	'                Enumeration e;
	'                System.out.println("Given name: " + args[i]);
	'                name = new CompoundName(args[i], dotSyntax);
	'                if (first == null) {
	'                    first = name;
	'                }
	'                e = name.getComponents();
	'                while (e.hasMoreElements()) {
	'                    System.out.println("Element: " + e.nextElement());
	'                }
	'                System.out.println("Constructed name: " + name.toString());
	'
	'                System.out.println("Compare " + first.toString() + " with "
	'                    + name.toString() + " = " + first.compareTo(name));
	'            }
	'        } catch (Exception ne) {
	'            ne.printStackTrace();
	'        }
	'    }
	'
	End Class

End Namespace
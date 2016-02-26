Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' The <tt>Name</tt> interface represents a generic name -- an ordered
	''' sequence of components.  It can be a composite name (names that
	''' span multiple namespaces), or a compound name (names that are
	''' used within individual hierarchical naming systems).
	''' 
	''' <p> There can be different implementations of <tt>Name</tt>; for example,
	''' composite names, URLs, or namespace-specific compound names.
	''' 
	''' <p> The components of a name are numbered.  The indexes of a name
	''' with N components range from 0 up to, but not including, N.  This
	''' range may be written as [0,N).
	''' The most significant component is at index 0.
	''' An empty name has no components.
	''' 
	''' <p> None of the methods in this interface accept null as a valid
	''' value for a parameter that is a name or a name component.
	''' Likewise, methods that return a name or name component never return null.
	''' 
	''' <p> An instance of a <tt>Name</tt> may not be synchronized against
	''' concurrent multithreaded access if that access is not read-only.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author R. Vasudevan
	''' @since 1.3
	''' </summary>

	Public Interface Name
		Inherits ICloneable, java.io.Serializable, IComparable(Of Object)

	   ''' <summary>
	   ''' The class fingerprint that is set to indicate
	   ''' serialization compatibility with a previous
	   ''' version of the class.
	   ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = -3617482732056931635L;

		''' <summary>
		''' Generates a new copy of this name.
		''' Subsequent changes to the components of this name will not
		''' affect the new copy, and vice versa.
		''' </summary>
		''' <returns>  a copy of this name
		''' </returns>
		''' <seealso cref= Object#clone() </seealso>
		Function clone() As Object

		''' <summary>
		''' Compares this name with another name for order.
		''' Returns a negative integer, zero, or a positive integer as this
		''' name is less than, equal to, or greater than the given name.
		''' 
		''' <p> As with <tt>Object.equals()</tt>, the notion of ordering for names
		''' depends on the class that implements this interface.
		''' For example, the ordering may be
		''' based on lexicographical ordering of the name components.
		''' Specific attributes of the name, such as how it treats case,
		''' may affect the ordering.  In general, two names of different
		''' classes may not be compared.
		''' </summary>
		''' <param name="obj"> the non-null object to compare against. </param>
		''' <returns>  a negative integer, zero, or a positive integer as this name
		'''          is less than, equal to, or greater than the given name </returns>
		''' <exception cref="ClassCastException"> if obj is not a <tt>Name</tt> of a
		'''          type that may be compared with this name
		''' </exception>
		''' <seealso cref= Comparable#compareTo(Object) </seealso>
		Function compareTo(ByVal obj As Object) As Integer

		''' <summary>
		''' Returns the number of components in this name.
		''' </summary>
		''' <returns>  the number of components in this name </returns>
		Function size() As Integer

		''' <summary>
		''' Determines whether this name is empty.
		''' An empty name is one with zero components.
		''' </summary>
		''' <returns>  true if this name is empty, false otherwise </returns>
		ReadOnly Property empty As Boolean

		''' <summary>
		''' Retrieves the components of this name as an enumeration
		''' of strings.  The effect on the enumeration of updates to
		''' this name is undefined.  If the name has zero components,
		''' an empty (non-null) enumeration is returned.
		''' </summary>
		''' <returns>  an enumeration of the components of this name, each a string </returns>
		ReadOnly Property all As System.Collections.IEnumerator(Of String)

		''' <summary>
		''' Retrieves a component of this name.
		''' </summary>
		''' <param name="posn">
		'''          the 0-based index of the component to retrieve.
		'''          Must be in the range [0,size()). </param>
		''' <returns>  the component at index posn </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if posn is outside the specified range </exception>
		Function [get](ByVal posn As Integer) As String

		''' <summary>
		''' Creates a name whose components consist of a prefix of the
		''' components of this name.  Subsequent changes to
		''' this name will not affect the name that is returned and vice versa.
		''' </summary>
		''' <param name="posn">
		'''          the 0-based index of the component at which to stop.
		'''          Must be in the range [0,size()]. </param>
		''' <returns>  a name consisting of the components at indexes in
		'''          the range [0,posn). </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if posn is outside the specified range </exception>
		Function getPrefix(ByVal posn As Integer) As Name

		''' <summary>
		''' Creates a name whose components consist of a suffix of the
		''' components in this name.  Subsequent changes to
		''' this name do not affect the name that is returned and vice versa.
		''' </summary>
		''' <param name="posn">
		'''          the 0-based index of the component at which to start.
		'''          Must be in the range [0,size()]. </param>
		''' <returns>  a name consisting of the components at indexes in
		'''          the range [posn,size()).  If posn is equal to
		'''          size(), an empty name is returned. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if posn is outside the specified range </exception>
		Function getSuffix(ByVal posn As Integer) As Name

		''' <summary>
		''' Determines whether this name starts with a specified prefix.
		''' A name <tt>n</tt> is a prefix if it is equal to
		''' <tt>getPrefix(n.size())</tt>.
		''' </summary>
		''' <param name="n">
		'''          the name to check </param>
		''' <returns>  true if <tt>n</tt> is a prefix of this name, false otherwise </returns>
		Function startsWith(ByVal n As Name) As Boolean

		''' <summary>
		''' Determines whether this name ends with a specified suffix.
		''' A name <tt>n</tt> is a suffix if it is equal to
		''' <tt>getSuffix(size()-n.size())</tt>.
		''' </summary>
		''' <param name="n">
		'''          the name to check </param>
		''' <returns>  true if <tt>n</tt> is a suffix of this name, false otherwise </returns>
		Function endsWith(ByVal n As Name) As Boolean

		''' <summary>
		''' Adds the components of a name -- in order -- to the end of this name.
		''' </summary>
		''' <param name="suffix">
		'''          the components to add </param>
		''' <returns>  the updated name (not a new one)
		''' </returns>
		''' <exception cref="InvalidNameException"> if <tt>suffix</tt> is not a valid name,
		'''          or if the addition of the components would violate the syntax
		'''          rules of this name </exception>
		Function addAll(ByVal suffix As Name) As Name

		''' <summary>
		''' Adds the components of a name -- in order -- at a specified position
		''' within this name.
		''' Components of this name at or after the index of the first new
		''' component are shifted up (away from 0) to accommodate the new
		''' components.
		''' </summary>
		''' <param name="n">
		'''          the components to add </param>
		''' <param name="posn">
		'''          the index in this name at which to add the new
		'''          components.  Must be in the range [0,size()]. </param>
		''' <returns>  the updated name (not a new one)
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if posn is outside the specified range </exception>
		''' <exception cref="InvalidNameException"> if <tt>n</tt> is not a valid name,
		'''          or if the addition of the components would violate the syntax
		'''          rules of this name </exception>
		Function addAll(ByVal posn As Integer, ByVal n As Name) As Name

		''' <summary>
		''' Adds a single component to the end of this name.
		''' </summary>
		''' <param name="comp">
		'''          the component to add </param>
		''' <returns>  the updated name (not a new one)
		''' </returns>
		''' <exception cref="InvalidNameException"> if adding <tt>comp</tt> would violate
		'''          the syntax rules of this name </exception>
		Function add(ByVal comp As String) As Name

		''' <summary>
		''' Adds a single component at a specified position within this name.
		''' Components of this name at or after the index of the new component
		''' are shifted up by one (away from index 0) to accommodate the new
		''' component.
		''' </summary>
		''' <param name="comp">
		'''          the component to add </param>
		''' <param name="posn">
		'''          the index at which to add the new component.
		'''          Must be in the range [0,size()]. </param>
		''' <returns>  the updated name (not a new one)
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if posn is outside the specified range </exception>
		''' <exception cref="InvalidNameException"> if adding <tt>comp</tt> would violate
		'''          the syntax rules of this name </exception>
		Function add(ByVal posn As Integer, ByVal comp As String) As Name

		''' <summary>
		''' Removes a component from this name.
		''' The component of this name at the specified position is removed.
		''' Components with indexes greater than this position
		''' are shifted down (toward index 0) by one.
		''' </summary>
		''' <param name="posn">
		'''          the index of the component to remove.
		'''          Must be in the range [0,size()). </param>
		''' <returns>  the component removed (a String)
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if posn is outside the specified range </exception>
		''' <exception cref="InvalidNameException"> if deleting the component
		'''          would violate the syntax rules of the name </exception>
		Function remove(ByVal posn As Integer) As Object
	End Interface

End Namespace
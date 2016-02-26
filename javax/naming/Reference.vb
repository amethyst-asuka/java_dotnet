Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents a reference to an object that is found outside of
	''' the naming/directory system.
	''' <p>
	''' Reference provides a way of recording address information about
	''' objects which themselves are not directly bound to the naming/directory system.
	''' <p>
	''' A Reference consists of an ordered list of addresses and class information
	''' about the object being referenced.
	''' Each address in the list identifies a communications endpoint
	''' for the same conceptual object.  The "communications endpoint"
	''' is information that indicates how to contact the object. It could
	''' be, for example, a network address, a location in memory on the
	''' local machine, another process on the same machine, etc.
	''' The order of the addresses in the list may be of significance
	''' to object factories that interpret the reference.
	''' <p>
	''' Multiple addresses may arise for
	''' various reasons, such as replication or the object offering interfaces
	''' over more than one communication mechanism.  The addresses are indexed
	''' starting with zero.
	''' <p>
	''' A Reference also contains information to assist in creating an instance
	''' of the object to which this Reference refers.  It contains the class name
	''' of that object, and the class name and location of the factory to be used
	''' to create the object.
	''' The class factory location is a space-separated list of URLs representing
	''' the class path used to load the factory.  When the factory class (or
	''' any class or resource upon which it depends) needs to be loaded,
	''' each URL is used (in order) to attempt to load the class.
	''' <p>
	''' A Reference instance is not synchronized against concurrent access by multiple
	''' threads. Threads that need to access a single Reference concurrently should
	''' synchronize amongst themselves and provide the necessary locking.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= RefAddr </seealso>
	''' <seealso cref= StringRefAddr </seealso>
	''' <seealso cref= BinaryRefAddr
	''' @since 1.3 </seealso>

	'  <p>
	'  * The serialized form of a Reference object consists of the class
	'  * name of the object being referenced (a String), a Vector of the
	'  * addresses (each a RefAddr), the name of the class factory (a
	'  * String), and the location of the class factory (a String).
	'


	<Serializable> _
	Public Class Reference
		Implements ICloneable

		''' <summary>
		''' Contains the fully-qualified name of the class of the object to which
		''' this Reference refers.
		''' @serial </summary>
		''' <seealso cref= java.lang.Class#getName </seealso>
		Protected Friend className As String
		''' <summary>
		''' Contains the addresses contained in this Reference.
		''' Initialized by constructor.
		''' @serial
		''' </summary>
		Protected Friend addrs As List(Of RefAddr) = Nothing

		''' <summary>
		''' Contains the name of the factory class for creating
		''' an instance of the object to which this Reference refers.
		''' Initialized to null.
		''' @serial
		''' </summary>
		Protected Friend classFactory As String = Nothing

		''' <summary>
		''' Contains the location of the factory class.
		''' Initialized to null.
		''' @serial
		''' </summary>
		Protected Friend classFactoryLocation As String = Nothing

		''' <summary>
		''' Constructs a new reference for an object with class name 'className'.
		''' Class factory and class factory location are set to null.
		''' The newly created reference contains zero addresses.
		''' </summary>
		''' <param name="className"> The non-null class name of the object to which
		''' this reference refers. </param>
		Public Sub New(ByVal className As String)
			Me.className = className
			addrs = New List(Of )
		End Sub

		''' <summary>
		''' Constructs a new reference for an object with class name 'className' and
		''' an address.
		''' Class factory and class factory location are set to null.
		''' </summary>
		''' <param name="className"> The non-null class name of the object to
		''' which this reference refers. </param>
		''' <param name="addr"> The non-null address of the object. </param>
		Public Sub New(ByVal className As String, ByVal addr As RefAddr)
			Me.className = className
			addrs = New List(Of )
			addrs.Add(addr)
		End Sub

		''' <summary>
		''' Constructs a new reference for an object with class name 'className',
		''' and the class name and location of the object's factory.
		''' </summary>
		''' <param name="className"> The non-null class name of the object to which
		'''                         this reference refers. </param>
		''' <param name="factory">  The possibly null class name of the object's factory. </param>
		''' <param name="factoryLocation">
		'''         The possibly null location from which to load
		'''         the factory (e.g. URL) </param>
		''' <seealso cref= javax.naming.spi.ObjectFactory </seealso>
		''' <seealso cref= javax.naming.spi.NamingManager#getObjectInstance </seealso>
		Public Sub New(ByVal className As String, ByVal factory As String, ByVal factoryLocation As String)
			Me.New(className)
			classFactory = factory
			classFactoryLocation = factoryLocation
		End Sub

		''' <summary>
		''' Constructs a new reference for an object with class name 'className',
		''' the class name and location of the object's factory, and the address for
		''' the object.
		''' </summary>
		''' <param name="className"> The non-null class name of the object to
		'''         which this reference refers. </param>
		''' <param name="factory">  The possibly null class name of the object's factory. </param>
		''' <param name="factoryLocation">  The possibly null location from which
		'''                         to load the factory (e.g. URL) </param>
		''' <param name="addr">     The non-null address of the object. </param>
		''' <seealso cref= javax.naming.spi.ObjectFactory </seealso>
		''' <seealso cref= javax.naming.spi.NamingManager#getObjectInstance </seealso>
		Public Sub New(ByVal className As String, ByVal addr As RefAddr, ByVal factory As String, ByVal factoryLocation As String)
			Me.New(className, addr)
			classFactory = factory
			classFactoryLocation = factoryLocation
		End Sub

		''' <summary>
		''' Retrieves the class name of the object to which this reference refers.
		''' </summary>
		''' <returns> The non-null fully-qualified class name of the object.
		'''         (e.g. "java.lang.String") </returns>
	ReadOnly	Public Overridable Property className As String
			Get
				Return _className
			End Get
		End Property

		''' <summary>
		''' Retrieves the class name of the factory of the object
		''' to which this reference refers.
		''' </summary>
		''' <returns> The possibly null fully-qualified class name of the factory.
		'''         (e.g. "java.lang.String") </returns>
	ReadOnly	Public Overridable Property factoryClassName As String
			Get
				Return classFactory
			End Get
		End Property

		''' <summary>
		''' Retrieves the location of the factory of the object
		''' to which this reference refers.
		''' If it is a codebase, then it is an ordered list of URLs,
		''' separated by spaces, listing locations from where the factory
		''' class definition should be loaded.
		''' </summary>
		''' <returns> The possibly null string containing the
		'''                 location for loading in the factory's class. </returns>
	ReadOnly	Public Overridable Property factoryClassLocation As String
			Get
				Return _classFactoryLocation
			End Get
		End Property

		''' <summary>
		''' Retrieves the first address that has the address type 'addrType'.
		''' String.compareTo() is used to test the equality of the address types.
		''' </summary>
		''' <param name="addrType"> The non-null address type for which to find the address. </param>
		''' <returns> The address in this reference with address type 'addrType;
		'''         null if no such address exist. </returns>
		Public Overridable Function [get](ByVal addrType As String) As RefAddr
			Dim len As Integer = addrs.Count
			Dim addr As RefAddr
			For i As Integer = 0 To len - 1
				addr = addrs(i)
				If addr.type.CompareTo(addrType) = 0 Then Return addr
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Retrieves the address at index posn. </summary>
		''' <param name="posn"> The index of the address to retrieve. </param>
		''' <returns> The address at the 0-based index posn. It must be in the
		'''         range [0,getAddressCount()). </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If posn not in the specified
		'''         range. </exception>
		Public Overridable Function [get](ByVal posn As Integer) As RefAddr
			Return addrs(posn)
		End Function

		''' <summary>
		''' Retrieves an enumeration of the addresses in this reference.
		''' When addresses are added, changed or removed from this reference,
		''' its effects on this enumeration are undefined.
		''' </summary>
		''' <returns> An non-null enumeration of the addresses
		'''         (<tt>RefAddr</tt>) in this reference.
		'''         If this reference has zero addresses, an enumeration with
		'''         zero elements is returned. </returns>
		Public Overridable Property all As System.Collections.IEnumerator(Of RefAddr)
			Get
				Return addrs.elements()
			End Get
		End Property

		''' <summary>
		''' Retrieves the number of addresses in this reference.
		''' </summary>
		''' <returns> The nonnegative number of addresses in this reference. </returns>
		Public Overridable Function size() As Integer
			Return addrs.Count
		End Function

		''' <summary>
		''' Adds an address to the end of the list of addresses.
		''' </summary>
		''' <param name="addr"> The non-null address to add. </param>
		Public Overridable Sub add(ByVal addr As RefAddr)
			addrs.Add(addr)
		End Sub

		''' <summary>
		''' Adds an address to the list of addresses at index posn.
		''' All addresses at index posn or greater are shifted up
		''' the list by one (away from index 0).
		''' </summary>
		''' <param name="posn"> The 0-based index of the list to insert addr. </param>
		''' <param name="addr"> The non-null address to add. </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If posn not in the specified
		'''         range. </exception>
		Public Overridable Sub add(ByVal posn As Integer, ByVal addr As RefAddr)
			addrs.Insert(posn, addr)
		End Sub

		''' <summary>
		''' Deletes the address at index posn from the list of addresses.
		''' All addresses at index greater than posn are shifted down
		''' the list by one (towards index 0).
		''' </summary>
		''' <param name="posn"> The 0-based index of in address to delete. </param>
		''' <returns> The address removed. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If posn not in the specified
		'''         range. </exception>
		Public Overridable Function remove(ByVal posn As Integer) As Object
			Dim r As Object = addrs(posn)
			addrs.RemoveAt(posn)
			Return r
		End Function

		''' <summary>
		''' Deletes all addresses from this reference.
		''' </summary>
		Public Overridable Sub clear()
			addrs.Capacity = 0
		End Sub

		''' <summary>
		''' Determines whether obj is a reference with the same addresses
		''' (in same order) as this reference.
		''' The addresses are checked using RefAddr.equals().
		''' In addition to having the same addresses, the Reference also needs to
		''' have the same class name as this reference.
		''' The class factory and class factory location are not checked.
		''' If obj is null or not an instance of Reference, null is returned.
		''' </summary>
		''' <param name="obj"> The possibly null object to check. </param>
		''' <returns> true if obj is equal to this reference; false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is Reference) Then
				Dim target As Reference = CType(obj, Reference)
				' ignore factory information
				If target.className.Equals(Me.className) AndAlso target.size() = Me.size() Then
					Dim mycomps As System.Collections.IEnumerator(Of RefAddr) = all
					Dim comps As System.Collections.IEnumerator(Of RefAddr) = target.all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While mycomps.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						If Not(mycomps.nextElement().Equals(comps.nextElement())) Then Return False
					Loop
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Computes the hash code of this reference.
		''' The hash code is the sum of the hash code of its addresses.
		''' </summary>
		''' <returns> A hash code of this reference as an int. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = className.GetHashCode()
			Dim e As System.Collections.IEnumerator(Of RefAddr) = all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				hash += e.nextElement().GetHashCode()
			Loop
			Return hash
		End Function

		''' <summary>
		''' Generates the string representation of this reference.
		''' The string consists of the class name to which this reference refers,
		''' and the string representation of each of its addresses.
		''' This representation is intended for display only and not to be parsed.
		''' </summary>
		''' <returns> The non-null string representation of this reference. </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder("Reference Class Name: " & className & vbLf)
			Dim len As Integer = addrs.Count
			For i As Integer = 0 To len - 1
				buf.Append([get](i).ToString())
			Next i

			Return buf.ToString()
		End Function

		''' <summary>
		''' Makes a copy of this reference using its class name
		''' list of addresses, class factory name and class factory location.
		''' Changes to the newly created copy does not affect this Reference
		''' and vice versa.
		''' </summary>
		Public Overridable Function clone() As Object
			Dim r As New Reference(className, classFactory, classFactoryLocation)
			Dim a As System.Collections.IEnumerator(Of RefAddr) = all
			r.addrs = New List(Of )

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While a.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				r.addrs.Add(a.nextElement())
			Loop
			Return r
		End Function
		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -1673475790065791735L
	End Class

End Namespace
Imports System

'
' * Copyright (c) 2004, 2005, Oracle and/or its affiliates. All rights reserved.
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

' Attributes2Impl.java - extended AttributesImpl
' http://www.saxproject.org
' Public Domain: no warranty.
' $Id: Attributes2Impl.java,v 1.3 2005/02/24 11:20:18 gg156739 Exp $

Namespace org.xml.sax.ext



	''' <summary>
	''' SAX2 extension helper for additional Attributes information,
	''' implementing the <seealso cref="Attributes2"/> interface.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' </blockquote>
	''' 
	''' <p>This is not part of core-only SAX2 distributions.</p>
	''' 
	''' <p>The <em>specified</em> flag for each attribute will always
	''' be true, unless it has been set to false in the copy constructor
	''' or using <seealso cref="#setSpecified"/>.
	''' Similarly, the <em>declared</em> flag for each attribute will
	''' always be false, except for defaulted attributes (<em>specified</em>
	''' is false), non-CDATA attributes, or when it is set to true using
	''' <seealso cref="#setDeclared"/>.
	''' If you change an attribute's type by hand, you may need to modify
	''' its <em>declared</em> flag to match.
	''' </p>
	''' 
	''' @since SAX 2.0 (extensions 1.1 alpha)
	''' @author David Brownell
	''' </summary>
	Public Class Attributes2Impl
		Inherits org.xml.sax.helpers.AttributesImpl
		Implements Attributes2

		Private declared As Boolean()
		Private specified As Boolean()


		''' <summary>
		''' Construct a new, empty Attributes2Impl object.
		''' </summary>
		Public Sub New()
			specified = Nothing
			declared = Nothing
		End Sub


		''' <summary>
		''' Copy an existing Attributes or Attributes2 object.
		''' If the object implements Attributes2, values of the
		''' <em>specified</em> and <em>declared</em> flags for each
		''' attribute are copied.
		''' Otherwise the flag values are defaulted to assume no DTD was used,
		''' unless there is evidence to the contrary (such as attributes with
		''' type other than CDATA, which must have been <em>declared</em>).
		''' 
		''' <p>This constructor is especially useful inside a
		''' <seealso cref="org.xml.sax.ContentHandler#startElement startElement"/> event.</p>
		''' </summary>
		''' <param name="atts"> The existing Attributes object. </param>
		Public Sub New(ByVal atts As org.xml.sax.Attributes)
			MyBase.New(atts)
		End Sub


		'//////////////////////////////////////////////////////////////////
		' Implementation of Attributes2
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Returns the current value of the attribute's "declared" flag.
		''' </summary>
		' javadoc mostly from interface
		Public Overridable Function isDeclared(ByVal index As Integer) As Boolean Implements Attributes2.isDeclared
			If index < 0 OrElse index >= length Then Throw New System.IndexOutOfRangeException("No attribute at index: " & index)
			Return declared (index)
		End Function


		''' <summary>
		''' Returns the current value of the attribute's "declared" flag.
		''' </summary>
		' javadoc mostly from interface
		Public Overridable Function isDeclared(ByVal uri As String, ByVal localName As String) As Boolean Implements Attributes2.isDeclared
			Dim index_Renamed As Integer = getIndex(uri, localName)

			If index_Renamed < 0 Then Throw New System.ArgumentException("No such attribute: local=" & localName & ", namespace=" & uri)
			Return declared (index_Renamed)
		End Function


		''' <summary>
		''' Returns the current value of the attribute's "declared" flag.
		''' </summary>
		' javadoc mostly from interface
		Public Overridable Function isDeclared(ByVal qName As String) As Boolean Implements Attributes2.isDeclared
			Dim index_Renamed As Integer = getIndex(qName)

			If index_Renamed < 0 Then Throw New System.ArgumentException("No such attribute: " & qName)
			Return declared (index_Renamed)
		End Function


		''' <summary>
		''' Returns the current value of an attribute's "specified" flag.
		''' </summary>
		''' <param name="index"> The attribute index (zero-based). </param>
		''' <returns> current flag value </returns>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not identify an attribute. </exception>
		Public Overridable Function isSpecified(ByVal index As Integer) As Boolean Implements Attributes2.isSpecified
			If index < 0 OrElse index >= length Then Throw New System.IndexOutOfRangeException("No attribute at index: " & index)
			Return specified (index)
		End Function


		''' <summary>
		''' Returns the current value of an attribute's "specified" flag.
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if
		'''        the name has no Namespace URI. </param>
		''' <param name="localName"> The attribute's local name. </param>
		''' <returns> current flag value </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> When the
		'''            supplied names do not identify an attribute. </exception>
		Public Overridable Function isSpecified(ByVal uri As String, ByVal localName As String) As Boolean Implements Attributes2.isSpecified
			Dim index_Renamed As Integer = getIndex(uri, localName)

			If index_Renamed < 0 Then Throw New System.ArgumentException("No such attribute: local=" & localName & ", namespace=" & uri)
			Return specified (index_Renamed)
		End Function


		''' <summary>
		''' Returns the current value of an attribute's "specified" flag.
		''' </summary>
		''' <param name="qName"> The XML qualified (prefixed) name. </param>
		''' <returns> current flag value </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> When the
		'''            supplied name does not identify an attribute. </exception>
		Public Overridable Function isSpecified(ByVal qName As String) As Boolean Implements Attributes2.isSpecified
			Dim index_Renamed As Integer = getIndex(qName)

			If index_Renamed < 0 Then Throw New System.ArgumentException("No such attribute: " & qName)
			Return specified (index_Renamed)
		End Function


		'//////////////////////////////////////////////////////////////////
		' Manipulators
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Copy an entire Attributes object.  The "specified" flags are
		''' assigned as true, and "declared" flags as false (except when
		''' an attribute's type is not CDATA),
		''' unless the object is an Attributes2 object.
		''' In that case those flag values are all copied.
		''' </summary>
		''' <seealso cref= AttributesImpl#setAttributes </seealso>
		Public Overrides Property attributes As org.xml.sax.Attributes
			Set(ByVal atts As org.xml.sax.Attributes)
				Dim length_Renamed As Integer = atts.length
    
				MyBase.attributes = atts
				declared = New Boolean (length_Renamed - 1){}
				specified = New Boolean (length_Renamed - 1){}
    
				If TypeOf atts Is Attributes2 Then
					Dim a2 As Attributes2 = CType(atts, Attributes2)
					For i As Integer = 0 To length_Renamed - 1
						declared (i) = a2.isDeclared(i)
						specified (i) = a2.isSpecified(i)
					Next i
				Else
					For i As Integer = 0 To length_Renamed - 1
						declared (i) = Not "CDATA".Equals(atts.getType(i))
						specified (i) = True
					Next i
				End If
			End Set
		End Property


		''' <summary>
		''' Add an attribute to the end of the list, setting its
		''' "specified" flag to true.  To set that flag's value
		''' to false, use <seealso cref="#setSpecified"/>.
		''' 
		''' <p>Unless the attribute <em>type</em> is CDATA, this attribute
		''' is marked as being declared in the DTD.  To set that flag's value
		''' to true for CDATA attributes, use <seealso cref="#setDeclared"/>.
		''' </summary>
		''' <seealso cref= AttributesImpl#addAttribute </seealso>
		Public Overrides Sub addAttribute(ByVal uri As String, ByVal localName As String, ByVal qName As String, ByVal type As String, ByVal value As String)
			MyBase.addAttribute(uri, localName, qName, type, value)


			Dim length_Renamed As Integer = length
			If specified Is Nothing Then
				specified = New Boolean(length_Renamed - 1){}
				declared = New Boolean(length_Renamed - 1){}
			ElseIf length_Renamed > specified.Length Then
				Dim newFlags As Boolean()

				newFlags = New Boolean (length_Renamed - 1){}
				Array.Copy(declared, 0, newFlags, 0, declared.Length)
				declared = newFlags

				newFlags = New Boolean (length_Renamed - 1){}
				Array.Copy(specified, 0, newFlags, 0, specified.Length)
				specified = newFlags
			End If

			specified (length_Renamed - 1) = True
			declared (length_Renamed - 1) = Not "CDATA".Equals(type)
		End Sub


		' javadoc entirely from superclass
		Public Overrides Sub removeAttribute(ByVal index As Integer)
			Dim origMax As Integer = length - 1

			MyBase.removeAttribute(index)
			If index <> origMax Then
				Array.Copy(declared, index + 1, declared, index, origMax - index)
				Array.Copy(specified, index + 1, specified, index, origMax - index)
			End If
		End Sub


		''' <summary>
		''' Assign a value to the "declared" flag of a specific attribute.
		''' This is normally needed only for attributes of type CDATA,
		''' including attributes whose type is changed to or from CDATA.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="value"> The desired flag value. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not identify an attribute. </exception>
		''' <seealso cref= #setType </seealso>
		Public Overridable Sub setDeclared(ByVal index As Integer, ByVal value As Boolean)
			If index < 0 OrElse index >= length Then Throw New System.IndexOutOfRangeException("No attribute at index: " & index)
			declared (index) = value
		End Sub


		''' <summary>
		''' Assign a value to the "specified" flag of a specific attribute.
		''' This is the only way this flag can be cleared, except clearing
		''' by initialization with the copy constructor.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="value"> The desired flag value. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not identify an attribute. </exception>
		Public Overridable Sub setSpecified(ByVal index As Integer, ByVal value As Boolean)
			If index < 0 OrElse index >= length Then Throw New System.IndexOutOfRangeException("No attribute at index: " & index)
			specified (index) = value
		End Sub
	End Class

End Namespace
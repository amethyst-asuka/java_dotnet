Imports System

'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

' AttributesImpl.java - default implementation of Attributes.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the public domain.
' $Id: AttributesImpl.java,v 1.2 2004/11/03 22:53:08 jsuttor Exp $

Namespace org.xml.sax.helpers



	''' <summary>
	''' Default implementation of the Attributes interface.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class provides a default implementation of the SAX2
	''' <seealso cref="org.xml.sax.Attributes Attributes"/> interface, with the
	''' addition of manipulators so that the list can be modified or
	''' reused.</p>
	''' 
	''' <p>There are two typical uses of this class:</p>
	''' 
	''' <ol>
	''' <li>to take a persistent snapshot of an Attributes object
	'''  in a <seealso cref="org.xml.sax.ContentHandler#startElement startElement"/> event; or</li>
	''' <li>to construct or modify an Attributes object in a SAX2 driver or filter.</li>
	''' </ol>
	''' 
	''' <p>This class replaces the now-deprecated SAX1 {@link
	''' org.xml.sax.helpers.AttributeListImpl AttributeListImpl}
	''' class; in addition to supporting the updated Attributes
	''' interface rather than the deprecated {@link org.xml.sax.AttributeList
	''' AttributeList} interface, it also includes a much more efficient
	''' implementation using a single array rather than a set of Vectors.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson
	''' </summary>
	Public Class AttributesImpl
		Implements org.xml.sax.Attributes


		'//////////////////////////////////////////////////////////////////
		' Constructors.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Construct a new, empty AttributesImpl object.
		''' </summary>
		Public Sub New()
			length = 0
			data = Nothing
		End Sub


		''' <summary>
		''' Copy an existing Attributes object.
		''' 
		''' <p>This constructor is especially useful inside a
		''' <seealso cref="org.xml.sax.ContentHandler#startElement startElement"/> event.</p>
		''' </summary>
		''' <param name="atts"> The existing Attributes object. </param>
		Public Sub New(ByVal atts As org.xml.sax.Attributes)
			attributes = atts
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.Attributes.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Return the number of attributes in the list.
		''' </summary>
		''' <returns> The number of attributes in the list. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getLength </seealso>
		Public Overridable Property length As Integer
			Get
				Return length
			End Get
		End Property


		''' <summary>
		''' Return an attribute's Namespace URI.
		''' </summary>
		''' <param name="index"> The attribute's index (zero-based). </param>
		''' <returns> The Namespace URI, the empty string if none is
		'''         available, or null if the index is out of range. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getURI </seealso>
		Public Overridable Function getURI(ByVal index As Integer) As String
			If index >= 0 AndAlso index < length Then
				Return data(index*5)
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Return an attribute's local name.
		''' </summary>
		''' <param name="index"> The attribute's index (zero-based). </param>
		''' <returns> The attribute's local name, the empty string if
		'''         none is available, or null if the index if out of range. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getLocalName </seealso>
		Public Overridable Function getLocalName(ByVal index As Integer) As String
			If index >= 0 AndAlso index < length Then
				Return data(index*5+1)
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Return an attribute's qualified (prefixed) name.
		''' </summary>
		''' <param name="index"> The attribute's index (zero-based). </param>
		''' <returns> The attribute's qualified name, the empty string if
		'''         none is available, or null if the index is out of bounds. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getQName </seealso>
		Public Overridable Function getQName(ByVal index As Integer) As String
			If index >= 0 AndAlso index < length Then
				Return data(index*5+2)
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Return an attribute's type by index.
		''' </summary>
		''' <param name="index"> The attribute's index (zero-based). </param>
		''' <returns> The attribute's type, "CDATA" if the type is unknown, or null
		'''         if the index is out of bounds. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getType(int) </seealso>
		Public Overridable Function [getType](ByVal index As Integer) As String
			If index >= 0 AndAlso index < length Then
				Return data(index*5+3)
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Return an attribute's value by index.
		''' </summary>
		''' <param name="index"> The attribute's index (zero-based). </param>
		''' <returns> The attribute's value or null if the index is out of bounds. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getValue(int) </seealso>
		Public Overridable Function getValue(ByVal index As Integer) As String
			If index >= 0 AndAlso index < length Then
				Return data(index*5+4)
			Else
				Return Nothing
			End If
		End Function


		''' <summary>
		''' Look up an attribute's index by Namespace name.
		''' 
		''' <p>In many cases, it will be more efficient to look up the name once and
		''' use the index query methods rather than using the name query methods
		''' repeatedly.</p>
		''' </summary>
		''' <param name="uri"> The attribute's Namespace URI, or the empty
		'''        string if none is available. </param>
		''' <param name="localName"> The attribute's local name. </param>
		''' <returns> The attribute's index, or -1 if none matches. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getIndex(java.lang.String,java.lang.String) </seealso>
		Public Overridable Function getIndex(ByVal uri As String, ByVal localName As String) As Integer
			Dim max As Integer = length * 5
			For i As Integer = 0 To max - 1 Step 5
				If data(i).Equals(uri) AndAlso data(i+1).Equals(localName) Then Return i \ 5
			Next i
			Return -1
		End Function


		''' <summary>
		''' Look up an attribute's index by qualified (prefixed) name.
		''' </summary>
		''' <param name="qName"> The qualified name. </param>
		''' <returns> The attribute's index, or -1 if none matches. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getIndex(java.lang.String) </seealso>
		Public Overridable Function getIndex(ByVal qName As String) As Integer
			Dim max As Integer = length * 5
			For i As Integer = 0 To max - 1 Step 5
				If data(i+2).Equals(qName) Then Return i \ 5
			Next i
			Return -1
		End Function


		''' <summary>
		''' Look up an attribute's type by Namespace-qualified name.
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string for a name
		'''        with no explicit Namespace URI. </param>
		''' <param name="localName"> The local name. </param>
		''' <returns> The attribute's type, or null if there is no
		'''         matching attribute. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getType(java.lang.String,java.lang.String) </seealso>
		Public Overridable Function [getType](ByVal uri As String, ByVal localName As String) As String
			Dim max As Integer = length * 5
			For i As Integer = 0 To max - 1 Step 5
				If data(i).Equals(uri) AndAlso data(i+1).Equals(localName) Then Return data(i+3)
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Look up an attribute's type by qualified (prefixed) name.
		''' </summary>
		''' <param name="qName"> The qualified name. </param>
		''' <returns> The attribute's type, or null if there is no
		'''         matching attribute. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getType(java.lang.String) </seealso>
		Public Overridable Function [getType](ByVal qName As String) As String
			Dim max As Integer = length * 5
			For i As Integer = 0 To max - 1 Step 5
				If data(i+2).Equals(qName) Then Return data(i+3)
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Look up an attribute's value by Namespace-qualified name.
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string for a name
		'''        with no explicit Namespace URI. </param>
		''' <param name="localName"> The local name. </param>
		''' <returns> The attribute's value, or null if there is no
		'''         matching attribute. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getValue(java.lang.String,java.lang.String) </seealso>
		Public Overridable Function getValue(ByVal uri As String, ByVal localName As String) As String
			Dim max As Integer = length * 5
			For i As Integer = 0 To max - 1 Step 5
				If data(i).Equals(uri) AndAlso data(i+1).Equals(localName) Then Return data(i+4)
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Look up an attribute's value by qualified (prefixed) name.
		''' </summary>
		''' <param name="qName"> The qualified name. </param>
		''' <returns> The attribute's value, or null if there is no
		'''         matching attribute. </returns>
		''' <seealso cref= org.xml.sax.Attributes#getValue(java.lang.String) </seealso>
		Public Overridable Function getValue(ByVal qName As String) As String
			Dim max As Integer = length * 5
			For i As Integer = 0 To max - 1 Step 5
				If data(i+2).Equals(qName) Then Return data(i+4)
			Next i
			Return Nothing
		End Function



		'//////////////////////////////////////////////////////////////////
		' Manipulators.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Clear the attribute list for reuse.
		''' 
		''' <p>Note that little memory is freed by this call:
		''' the current array is kept so it can be
		''' reused.</p>
		''' </summary>
		Public Overridable Sub clear()
			If data IsNot Nothing Then
				For i As Integer = 0 To (length * 5) - 1
					data (i) = Nothing
				Next i
			End If
			length = 0
		End Sub


		''' <summary>
		''' Copy an entire Attributes object.
		''' 
		''' <p>It may be more efficient to reuse an existing object
		''' rather than constantly allocating new ones.</p>
		''' </summary>
		''' <param name="atts"> The attributes to copy. </param>
		Public Overridable Property attributes As org.xml.sax.Attributes
			Set(ByVal atts As org.xml.sax.Attributes)
				clear()
				length = atts.length
				If length > 0 Then
					data = New String(length*5 - 1){}
					For i As Integer = 0 To length - 1
						data(i*5) = atts.getURI(i)
						data(i*5+1) = atts.getLocalName(i)
						data(i*5+2) = atts.getQName(i)
						data(i*5+3) = atts.getType(i)
						data(i*5+4) = atts.getValue(i)
					Next i
				End If
			End Set
		End Property


		''' <summary>
		''' Add an attribute to the end of the list.
		''' 
		''' <p>For the sake of speed, this method does no checking
		''' to see if the attribute is already in the list: that is
		''' the responsibility of the application.</p>
		''' </summary>
		''' <param name="uri"> The Namespace URI, or the empty string if
		'''        none is available or Namespace processing is not
		'''        being performed. </param>
		''' <param name="localName"> The local name, or the empty string if
		'''        Namespace processing is not being performed. </param>
		''' <param name="qName"> The qualified (prefixed) name, or the empty string
		'''        if qualified names are not available. </param>
		''' <param name="type"> The attribute type as a string. </param>
		''' <param name="value"> The attribute value. </param>
		Public Overridable Sub addAttribute(ByVal uri As String, ByVal localName As String, ByVal qName As String, ByVal type As String, ByVal value As String)
			ensureCapacity(length+1)
			data(length*5) = uri
			data(length*5+1) = localName
			data(length*5+2) = qName
			data(length*5+3) = type
			data(length*5+4) = value
			length += 1
		End Sub


		''' <summary>
		''' Set an attribute in the list.
		''' 
		''' <p>For the sake of speed, this method does no checking
		''' for name conflicts or well-formedness: such checks are the
		''' responsibility of the application.</p>
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="uri"> The Namespace URI, or the empty string if
		'''        none is available or Namespace processing is not
		'''        being performed. </param>
		''' <param name="localName"> The local name, or the empty string if
		'''        Namespace processing is not being performed. </param>
		''' <param name="qName"> The qualified name, or the empty string
		'''        if qualified names are not available. </param>
		''' <param name="type"> The attribute type as a string. </param>
		''' <param name="value"> The attribute value. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub setAttribute(ByVal index As Integer, ByVal uri As String, ByVal localName As String, ByVal qName As String, ByVal type As String, ByVal value As String)
			If index >= 0 AndAlso index < length Then
				data(index*5) = uri
				data(index*5+1) = localName
				data(index*5+2) = qName
				data(index*5+3) = type
				data(index*5+4) = value
			Else
				badIndex(index)
			End If
		End Sub


		''' <summary>
		''' Remove an attribute from the list.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub removeAttribute(ByVal index As Integer)
			If index >= 0 AndAlso index < length Then
				If index < length - 1 Then Array.Copy(data, (index+1)*5, data, index*5, (length-index-1)*5)
				index = (length - 1) * 5
				data (index) = Nothing
				index += 1
				data (index) = Nothing
				index += 1
				data (index) = Nothing
				index += 1
				data (index) = Nothing
				index += 1
				data (index) = Nothing
				length -= 1
			Else
				badIndex(index)
			End If
		End Sub


		''' <summary>
		''' Set the Namespace URI of a specific attribute.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="uri"> The attribute's Namespace URI, or the empty
		'''        string for none. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub setURI(ByVal index As Integer, ByVal uri As String)
			If index >= 0 AndAlso index < length Then
				data(index*5) = uri
			Else
				badIndex(index)
			End If
		End Sub


		''' <summary>
		''' Set the local name of a specific attribute.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="localName"> The attribute's local name, or the empty
		'''        string for none. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub setLocalName(ByVal index As Integer, ByVal localName As String)
			If index >= 0 AndAlso index < length Then
				data(index*5+1) = localName
			Else
				badIndex(index)
			End If
		End Sub


		''' <summary>
		''' Set the qualified name of a specific attribute.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="qName"> The attribute's qualified name, or the empty
		'''        string for none. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub setQName(ByVal index As Integer, ByVal qName As String)
			If index >= 0 AndAlso index < length Then
				data(index*5+2) = qName
			Else
				badIndex(index)
			End If
		End Sub


		''' <summary>
		''' Set the type of a specific attribute.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="type"> The attribute's type. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub setType(ByVal index As Integer, ByVal type As String)
			If index >= 0 AndAlso index < length Then
				data(index*5+3) = type
			Else
				badIndex(index)
			End If
		End Sub


		''' <summary>
		''' Set the value of a specific attribute.
		''' </summary>
		''' <param name="index"> The index of the attribute (zero-based). </param>
		''' <param name="value"> The attribute's value. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> When the
		'''            supplied index does not point to an attribute
		'''            in the list. </exception>
		Public Overridable Sub setValue(ByVal index As Integer, ByVal value As String)
			If index >= 0 AndAlso index < length Then
				data(index*5+4) = value
			Else
				badIndex(index)
			End If
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Internal methods.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Ensure the internal array's capacity.
		''' </summary>
		''' <param name="n"> The minimum number of attributes that the array must
		'''        be able to hold. </param>
		Private Sub ensureCapacity(ByVal n As Integer)
			If n <= 0 Then Return
			Dim max As Integer
			If data Is Nothing OrElse data.Length = 0 Then
				max = 25
			ElseIf data.Length >= n * 5 Then
				Return
			Else
				max = data.Length
			End If
			Do While max < n * 5
				max *= 2
			Loop

			Dim newData As String() = New String(max - 1){}
			If length > 0 Then Array.Copy(data, 0, newData, 0, length*5)
			data = newData
		End Sub


		''' <summary>
		''' Report a bad array index in a manipulator.
		''' </summary>
		''' <param name="index"> The index to report. </param>
		''' <exception cref="java.lang.ArrayIndexOutOfBoundsException"> Always. </exception>
		Private Sub badIndex(ByVal index As Integer)
			Dim msg As String = "Attempt to modify attribute at illegal index: " & index
			Throw New System.IndexOutOfRangeException(msg)
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		Friend length As Integer
		Friend data As String()

	End Class

	' end of AttributesImpl.java

End Namespace
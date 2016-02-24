Imports System.Collections

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

' SAX default implementation for AttributeList.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: AttributeListImpl.java,v 1.2 2004/11/03 22:53:08 jsuttor Exp $

Namespace org.xml.sax.helpers




	''' <summary>
	''' Default implementation for AttributeList.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>AttributeList implements the deprecated SAX1 {@link
	''' org.xml.sax.AttributeList AttributeList} interface, and has been
	''' replaced by the new SAX2 {@link org.xml.sax.helpers.AttributesImpl
	''' AttributesImpl} interface.</p>
	''' 
	''' <p>This class provides a convenience implementation of the SAX
	''' <seealso cref="org.xml.sax.AttributeList AttributeList"/> interface.  This
	''' implementation is useful both for SAX parser writers, who can use
	''' it to provide attributes to the application, and for SAX application
	''' writers, who can use it to create a persistent copy of an element's
	''' attribute specifications:</p>
	''' 
	''' <pre>
	''' private AttributeList myatts;
	''' 
	''' public void startElement (String name, AttributeList atts)
	''' {
	'''              // create a persistent copy of the attribute list
	'''              // for use outside this method
	'''   myatts = new AttributeListImpl(atts);
	'''   [...]
	''' }
	''' </pre>
	''' 
	''' <p>Please note that SAX parsers are not required to use this
	''' class to provide an implementation of AttributeList; it is
	''' supplied only as an optional convenience.  In particular,
	''' parser writers are encouraged to invent more efficient
	''' implementations.</p>
	''' </summary>
	''' @deprecated This class implements a deprecated interface,
	'''             <seealso cref="org.xml.sax.AttributeList AttributeList"/>;
	'''             that interface has been replaced by
	'''             <seealso cref="org.xml.sax.Attributes Attributes"/>,
	'''             which is implemented in the
	'''             {@link org.xml.sax.helpers.AttributesImpl
	'''            AttributesImpl} helper class.
	''' @since SAX 1.0
	''' @author David Megginson 
	''' <seealso cref= org.xml.sax.AttributeList </seealso>
	''' <seealso cref= org.xml.sax.DocumentHandler#startElement </seealso>
	Public Class AttributeListImpl
		Implements org.xml.sax.AttributeList

		''' <summary>
		''' Create an empty attribute list.
		''' 
		''' <p>This constructor is most useful for parser writers, who
		''' will use it to create a single, reusable attribute list that
		''' can be reset with the clear method between elements.</p>
		''' </summary>
		''' <seealso cref= #addAttribute </seealso>
		''' <seealso cref= #clear </seealso>
		Public Sub New()
		End Sub


		''' <summary>
		''' Construct a persistent copy of an existing attribute list.
		''' 
		''' <p>This constructor is most useful for application writers,
		''' who will use it to create a persistent copy of an existing
		''' attribute list.</p>
		''' </summary>
		''' <param name="atts"> The attribute list to copy </param>
		''' <seealso cref= org.xml.sax.DocumentHandler#startElement </seealso>
		Public Sub New(ByVal atts As org.xml.sax.AttributeList)
			attributeList = atts
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Methods specific to this class.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Set the attribute list, discarding previous contents.
		''' 
		''' <p>This method allows an application writer to reuse an
		''' attribute list easily.</p>
		''' </summary>
		''' <param name="atts"> The attribute list to copy. </param>
		Public Overridable Property attributeList As org.xml.sax.AttributeList
			Set(ByVal atts As org.xml.sax.AttributeList)
				Dim count As Integer = atts.length
    
				clear()
    
				For i As Integer = 0 To count - 1
					addAttribute(atts.getName(i), atts.getType(i), atts.getValue(i))
				Next i
			End Set
		End Property


		''' <summary>
		''' Add an attribute to an attribute list.
		''' 
		''' <p>This method is provided for SAX parser writers, to allow them
		''' to build up an attribute list incrementally before delivering
		''' it to the application.</p>
		''' </summary>
		''' <param name="name"> The attribute name. </param>
		''' <param name="type"> The attribute type ("NMTOKEN" for an enumeration). </param>
		''' <param name="value"> The attribute value (must not be null). </param>
		''' <seealso cref= #removeAttribute </seealso>
		''' <seealso cref= org.xml.sax.DocumentHandler#startElement </seealso>
		Public Overridable Sub addAttribute(ByVal name As String, ByVal type As String, ByVal value As String)
			names.Add(name)
			types.Add(type)
			values.Add(value)
		End Sub


		''' <summary>
		''' Remove an attribute from the list.
		''' 
		''' <p>SAX application writers can use this method to filter an
		''' attribute out of an AttributeList.  Note that invoking this
		''' method will change the length of the attribute list and
		''' some of the attribute's indices.</p>
		''' 
		''' <p>If the requested attribute is not in the list, this is
		''' a no-op.</p>
		''' </summary>
		''' <param name="name"> The attribute name. </param>
		''' <seealso cref= #addAttribute </seealso>
		Public Overridable Sub removeAttribute(ByVal name As String)
			Dim i As Integer = names.IndexOf(name)

			If i >= 0 Then
				names.RemoveAt(i)
				types.RemoveAt(i)
				values.RemoveAt(i)
			End If
		End Sub


		''' <summary>
		''' Clear the attribute list.
		''' 
		''' <p>SAX parser writers can use this method to reset the attribute
		''' list between DocumentHandler.startElement events.  Normally,
		''' it will make sense to reuse the same AttributeListImpl object
		''' rather than allocating a new one each time.</p>
		''' </summary>
		''' <seealso cref= org.xml.sax.DocumentHandler#startElement </seealso>
		Public Overridable Sub clear()
			names.Clear()
			types.Clear()
			values.Clear()
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.AttributeList
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Return the number of attributes in the list.
		''' </summary>
		''' <returns> The number of attributes in the list. </returns>
		''' <seealso cref= org.xml.sax.AttributeList#getLength </seealso>
		Public Overridable Property length As Integer
			Get
				Return names.Count
			End Get
		End Property


		''' <summary>
		''' Get the name of an attribute (by position).
		''' </summary>
		''' <param name="i"> The position of the attribute in the list. </param>
		''' <returns> The attribute name as a string, or null if there
		'''         is no attribute at that position. </returns>
		''' <seealso cref= org.xml.sax.AttributeList#getName(int) </seealso>
		Public Overridable Function getName(ByVal i As Integer) As String
			If i < 0 Then Return Nothing
			Try
				Return CStr(names(i))
			Catch e As System.IndexOutOfRangeException
				Return Nothing
			End Try
		End Function


		''' <summary>
		''' Get the type of an attribute (by position).
		''' </summary>
		''' <param name="i"> The position of the attribute in the list. </param>
		''' <returns> The attribute type as a string ("NMTOKEN" for an
		'''         enumeration, and "CDATA" if no declaration was
		'''         read), or null if there is no attribute at
		'''         that position. </returns>
		''' <seealso cref= org.xml.sax.AttributeList#getType(int) </seealso>
		Public Overridable Function [getType](ByVal i As Integer) As String
			If i < 0 Then Return Nothing
			Try
				Return CStr(types(i))
			Catch e As System.IndexOutOfRangeException
				Return Nothing
			End Try
		End Function


		''' <summary>
		''' Get the value of an attribute (by position).
		''' </summary>
		''' <param name="i"> The position of the attribute in the list. </param>
		''' <returns> The attribute value as a string, or null if
		'''         there is no attribute at that position. </returns>
		''' <seealso cref= org.xml.sax.AttributeList#getValue(int) </seealso>
		Public Overridable Function getValue(ByVal i As Integer) As String
			If i < 0 Then Return Nothing
			Try
				Return CStr(values(i))
			Catch e As System.IndexOutOfRangeException
				Return Nothing
			End Try
		End Function


		''' <summary>
		''' Get the type of an attribute (by name).
		''' </summary>
		''' <param name="name"> The attribute name. </param>
		''' <returns> The attribute type as a string ("NMTOKEN" for an
		'''         enumeration, and "CDATA" if no declaration was
		'''         read). </returns>
		''' <seealso cref= org.xml.sax.AttributeList#getType(java.lang.String) </seealso>
		Public Overridable Function [getType](ByVal name As String) As String
			Return [getType](names.IndexOf(name))
		End Function


		''' <summary>
		''' Get the value of an attribute (by name).
		''' </summary>
		''' <param name="name"> The attribute name. </param>
		''' <seealso cref= org.xml.sax.AttributeList#getValue(java.lang.String) </seealso>
		Public Overridable Function getValue(ByVal name As String) As String
			Return getValue(names.IndexOf(name))
		End Function



		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		Friend names As New ArrayList
		Friend types As New ArrayList
		Friend values As New ArrayList

	End Class

	' end of AttributeListImpl.java

End Namespace
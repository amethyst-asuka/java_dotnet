Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
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

Namespace javax.imageio.metadata


	''' <summary>
	''' A concrete class providing a reusable implementation of the
	''' <code>IIOMetadataFormat</code> interface.  In addition, a static
	''' instance representing the standard, plug-in neutral
	''' <code>javax_imageio_1.0</code> format is provided by the
	''' <code>getStandardFormatInstance</code> method.
	''' 
	''' <p> In order to supply localized descriptions of elements and
	''' attributes, a <code>ResourceBundle</code> with a base name of
	''' <code>this.getClass().getName() + "Resources"</code> should be
	''' supplied via the usual mechanism used by
	''' <code>ResourceBundle.getBundle</code>.  Briefly, the subclasser
	''' supplies one or more additional classes according to a naming
	''' convention (by default, the fully-qualified name of the subclass
	''' extending <code>IIMetadataFormatImpl</code>, plus the string
	''' "Resources", plus the country, language, and variant codes
	''' separated by underscores).  At run time, calls to
	''' <code>getElementDescription</code> or
	''' <code>getAttributeDescription</code> will attempt to load such
	''' classes dynamically according to the supplied locale, and will use
	''' either the element name, or the element name followed by a '/'
	''' character followed by the attribute name as a key.  This key will
	''' be supplied to the <code>ResourceBundle</code>'s
	''' <code>getString</code> method, and the resulting localized
	''' description of the node or attribute is returned.
	''' 
	''' <p> The subclass may supply a different base name for the resource
	''' bundles using the <code>setResourceBaseName</code> method.
	''' 
	''' <p> A subclass may choose its own localization mechanism, if so
	''' desired, by overriding the supplied implementations of
	''' <code>getElementDescription</code> and
	''' <code>getAttributeDescription</code>.
	''' </summary>
	''' <seealso cref= ResourceBundle#getBundle(String,Locale)
	'''  </seealso>
	Public MustInherit Class IIOMetadataFormatImpl
		Implements IIOMetadataFormat

		''' <summary>
		''' A <code>String</code> constant containing the standard format
		''' name, <code>"javax_imageio_1.0"</code>.
		''' </summary>
		Public Const standardMetadataFormatName As String = "javax_imageio_1.0"

		Private Shared standardFormat As IIOMetadataFormat = Nothing

		Private resourceBaseName As String = Me.GetType().name & "Resources"

		Private rootName As String

		' Element name (String) -> Element
		Private elementMap As New Hashtable

		Friend Class Element
			Private ReadOnly outerInstance As IIOMetadataFormatImpl

			Public Sub New(ByVal outerInstance As IIOMetadataFormatImpl)
				Me.outerInstance = outerInstance
			End Sub

			Friend elementName As String

			Friend childPolicy As Integer
			Friend minChildren As Integer = 0
			Friend maxChildren As Integer = 0

			' Child names (Strings)
			Friend childList As IList = New ArrayList

			' Parent names (Strings)
			Friend parentList As IList = New ArrayList

			' List of attribute names in the order they were added
			Friend attrList As IList = New ArrayList
			' Attr name (String) -> Attribute
			Friend attrMap As IDictionary = New Hashtable

			Friend objectValue As ObjectValue
		End Class

		Friend Class Attribute
			Private ReadOnly outerInstance As IIOMetadataFormatImpl

			Public Sub New(ByVal outerInstance As IIOMetadataFormatImpl)
				Me.outerInstance = outerInstance
			End Sub

			Friend attrName As String

			Friend valueType As Integer = VALUE_ARBITRARY
			Friend dataType As Integer
			Friend required As Boolean
			Friend defaultValue As String = Nothing

			' enumeration
			Friend enumeratedValues As IList

			' range
			Friend minValue As String
			Friend maxValue As String

			' list
			Friend listMinLength As Integer
			Friend listMaxLength As Integer
		End Class

		Friend Class ObjectValue
			Private ReadOnly outerInstance As IIOMetadataFormatImpl

			Public Sub New(ByVal outerInstance As IIOMetadataFormatImpl)
				Me.outerInstance = outerInstance
			End Sub

			Friend valueType As Integer = VALUE_NONE
			Friend classType As Type = Nothing
			Friend defaultValue As Object = Nothing

			' Meaningful only if valueType == VALUE_ENUMERATION
			Friend enumeratedValues As IList = Nothing

			' Meaningful only if valueType == VALUE_RANGE
			Friend minValue As IComparable = Nothing
			Friend maxValue As IComparable = Nothing

			' Meaningful only if valueType == VALUE_LIST
			Friend arrayMinLength As Integer = 0
			Friend arrayMaxLength As Integer = 0
		End Class

		''' <summary>
		''' Constructs a blank <code>IIOMetadataFormatImpl</code> instance,
		''' with a given root element name and child policy (other than
		''' <code>CHILD_POLICY_REPEAT</code>).  Additional elements, and
		''' their attributes and <code>Object</code> reference information
		''' may be added using the various <code>add</code> methods.
		''' </summary>
		''' <param name="rootName"> the name of the root element. </param>
		''' <param name="childPolicy"> one of the <code>CHILD_POLICY_*</code> constants,
		''' other than <code>CHILD_POLICY_REPEAT</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>rootName</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>childPolicy</code> is
		''' not one of the predefined constants. </exception>
		Public Sub New(ByVal rootName As String, ByVal childPolicy As Integer)
			If rootName Is Nothing Then Throw New System.ArgumentException("rootName == null!")
			If childPolicy < CHILD_POLICY_EMPTY OrElse childPolicy > CHILD_POLICY_MAX OrElse childPolicy = CHILD_POLICY_REPEAT Then Throw New System.ArgumentException("Invalid value for childPolicy!")

			Me.rootName = rootName

			Dim root As New Element(Me)
			root.elementName = rootName
			root.childPolicy = childPolicy

			elementMap(rootName) = root
		End Sub

		''' <summary>
		''' Constructs a blank <code>IIOMetadataFormatImpl</code> instance,
		''' with a given root element name and a child policy of
		''' <code>CHILD_POLICY_REPEAT</code>.  Additional elements, and
		''' their attributes and <code>Object</code> reference information
		''' may be added using the various <code>add</code> methods.
		''' </summary>
		''' <param name="rootName"> the name of the root element. </param>
		''' <param name="minChildren"> the minimum number of children of the node. </param>
		''' <param name="maxChildren"> the maximum number of children of the node.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>rootName</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>minChildren</code>
		''' is negative or larger than <code>maxChildren</code>. </exception>
		Public Sub New(ByVal rootName As String, ByVal minChildren As Integer, ByVal maxChildren As Integer)
			If rootName Is Nothing Then Throw New System.ArgumentException("rootName == null!")
			If minChildren < 0 Then Throw New System.ArgumentException("minChildren < 0!")
			If minChildren > maxChildren Then Throw New System.ArgumentException("minChildren > maxChildren!")

			Dim root As New Element(Me)
			root.elementName = rootName
			root.childPolicy = CHILD_POLICY_REPEAT
			root.minChildren = minChildren
			root.maxChildren = maxChildren

			Me.rootName = rootName
			elementMap(rootName) = root
		End Sub

		''' <summary>
		''' Sets a new base name for locating <code>ResourceBundle</code>s
		''' containing descriptions of elements and attributes for this
		''' format.
		''' 
		''' <p> Prior to the first time this method is called, the base
		''' name will be equal to <code>this.getClass().getName() +
		''' "Resources"</code>.
		''' </summary>
		''' <param name="resourceBaseName"> a <code>String</code> containing the new
		''' base name.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>resourceBaseName</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #getResourceBaseName </seealso>
		Protected Friend Overridable Property resourceBaseName As String
			Set(ByVal resourceBaseName As String)
				If resourceBaseName Is Nothing Then Throw New System.ArgumentException("resourceBaseName == null!")
				Me.resourceBaseName = resourceBaseName
			End Set
			Get
				Return resourceBaseName
			End Get
		End Property


		''' <summary>
		''' Utility method for locating an element.
		''' </summary>
		''' <param name="mustAppear"> if <code>true</code>, throw an
		''' <code>IllegalArgumentException</code> if no such node exists;
		''' if <code>false</code>, just return null. </param>
		Private Function getElement(ByVal elementName As String, ByVal mustAppear As Boolean) As Element
			If mustAppear AndAlso (elementName Is Nothing) Then Throw New System.ArgumentException("element name is null!")
			Dim ___element As Element = CType(elementMap(elementName), Element)
			If mustAppear AndAlso (___element Is Nothing) Then Throw New System.ArgumentException("No such element: " & elementName)
			Return ___element
		End Function

		Private Function getElement(ByVal elementName As String) As Element
			Return getElement(elementName, True)
		End Function

		' Utility method for locating an attribute
		Private Function getAttribute(ByVal elementName As String, ByVal attrName As String) As Attribute
			Dim ___element As Element = getElement(elementName)
			Dim attr As Attribute = CType(___element.attrMap(attrName), Attribute)
			If attr Is Nothing Then Throw New System.ArgumentException("No such attribute """ & attrName & """!")
			Return attr
		End Function

		' Setup

		''' <summary>
		''' Adds a new element type to this metadata document format with a
		''' child policy other than <code>CHILD_POLICY_REPEAT</code>.
		''' </summary>
		''' <param name="elementName"> the name of the new element. </param>
		''' <param name="parentName"> the name of the element that will be the
		''' parent of the new element. </param>
		''' <param name="childPolicy"> one of the <code>CHILD_POLICY_*</code>
		''' constants, other than <code>CHILD_POLICY_REPEAT</code>,
		''' indicating the child policy of the new element.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>parentName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>childPolicy</code>
		''' is not one of the predefined constants. </exception>
		Protected Friend Overridable Sub addElement(ByVal elementName As String, ByVal parentName As String, ByVal childPolicy As Integer)
			Dim parent As Element = getElement(parentName)
			If childPolicy < CHILD_POLICY_EMPTY OrElse childPolicy > CHILD_POLICY_MAX OrElse childPolicy = CHILD_POLICY_REPEAT Then Throw New System.ArgumentException("Invalid value for childPolicy!")

			Dim ___element As New Element(Me)
			___element.elementName = elementName
			___element.childPolicy = childPolicy

			parent.childList.Add(elementName)
			___element.parentList.Add(parentName)

			elementMap(elementName) = ___element
		End Sub

		''' <summary>
		''' Adds a new element type to this metadata document format with a
		''' child policy of <code>CHILD_POLICY_REPEAT</code>.
		''' </summary>
		''' <param name="elementName"> the name of the new element. </param>
		''' <param name="parentName"> the name of the element that will be the
		''' parent of the new element. </param>
		''' <param name="minChildren"> the minimum number of children of the node. </param>
		''' <param name="maxChildren"> the maximum number of children of the node.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>parentName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>minChildren</code>
		''' is negative or larger than <code>maxChildren</code>. </exception>
		Protected Friend Overridable Sub addElement(ByVal elementName As String, ByVal parentName As String, ByVal minChildren As Integer, ByVal maxChildren As Integer)
			Dim parent As Element = getElement(parentName)
			If minChildren < 0 Then Throw New System.ArgumentException("minChildren < 0!")
			If minChildren > maxChildren Then Throw New System.ArgumentException("minChildren > maxChildren!")

			Dim ___element As New Element(Me)
			___element.elementName = elementName
			___element.childPolicy = CHILD_POLICY_REPEAT
			___element.minChildren = minChildren
			___element.maxChildren = maxChildren

			parent.childList.Add(elementName)
			___element.parentList.Add(parentName)

			elementMap(elementName) = ___element
		End Sub

		''' <summary>
		''' Adds an existing element to the list of legal children for a
		''' given parent node type.
		''' </summary>
		''' <param name="parentName"> the name of the element that will be the
		''' new parent of the element. </param>
		''' <param name="elementName"> the name of the element to be added as a
		''' child.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>parentName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		Protected Friend Overridable Sub addChildElement(ByVal elementName As String, ByVal parentName As String)
			Dim parent As Element = getElement(parentName)
			Dim ___element As Element = getElement(elementName)
			parent.childList.Add(elementName)
			___element.parentList.Add(parentName)
		End Sub

		''' <summary>
		''' Removes an element from the format.  If no element with the
		''' given name was present, nothing happens and no exception is
		''' thrown.
		''' </summary>
		''' <param name="elementName"> the name of the element to be removed. </param>
		Protected Friend Overridable Sub removeElement(ByVal elementName As String)
			Dim ___element As Element = getElement(elementName, False)
			If ___element IsNot Nothing Then
				Dim iter As IEnumerator = ___element.parentList.GetEnumerator()
				Do While iter.hasNext()
					Dim parentName As String = CStr(iter.next())
					Dim parent As Element = getElement(parentName, False)
					If parent IsNot Nothing Then parent.childList.Remove(elementName)
				Loop
				elementMap.Remove(elementName)
			End If
		End Sub

		''' <summary>
		''' Adds a new attribute to a previously defined element that may
		''' be set to an arbitrary value.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute being added. </param>
		''' <param name="dataType"> the data type (string format) of the attribute,
		''' one of the <code>DATATYPE_*</code> constants. </param>
		''' <param name="required"> <code>true</code> if the attribute must be present. </param>
		''' <param name="defaultValue"> the default value for the attribute, or
		''' <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>attrName</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of the predefined constants. </exception>
		Protected Friend Overridable Sub addAttribute(ByVal elementName As String, ByVal attrName As String, ByVal dataType As Integer, ByVal required As Boolean, ByVal defaultValue As String)
			Dim ___element As Element = getElement(elementName)
			If attrName Is Nothing Then Throw New System.ArgumentException("attrName == null!")
			If dataType < DATATYPE_STRING OrElse dataType > DATATYPE_DOUBLE Then Throw New System.ArgumentException("Invalid value for dataType!")

			Dim attr As New Attribute(Me)
			attr.attrName = attrName
			attr.valueType = VALUE_ARBITRARY
			attr.dataType = dataType
			attr.required = required
			attr.defaultValue = defaultValue

			___element.attrList.Add(attrName)
			___element.attrMap(attrName) = attr
		End Sub

		''' <summary>
		''' Adds a new attribute to a previously defined element that will
		''' be defined by a set of enumerated values.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute being added. </param>
		''' <param name="dataType"> the data type (string format) of the attribute,
		''' one of the <code>DATATYPE_*</code> constants. </param>
		''' <param name="required"> <code>true</code> if the attribute must be present. </param>
		''' <param name="defaultValue"> the default value for the attribute, or
		''' <code>null</code>. </param>
		''' <param name="enumeratedValues"> a <code>List</code> of
		''' <code>String</code>s containing the legal values for the
		''' attribute.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>attrName</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of the predefined constants. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>enumeratedValues</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>enumeratedValues</code> does not contain at least one
		''' entry. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>enumeratedValues</code> contains an element that is not a
		''' <code>String</code> or is <code>null</code>. </exception>
		Protected Friend Overridable Sub addAttribute(ByVal elementName As String, ByVal attrName As String, ByVal dataType As Integer, ByVal required As Boolean, ByVal defaultValue As String, ByVal enumeratedValues As IList(Of String))
			Dim ___element As Element = getElement(elementName)
			If attrName Is Nothing Then Throw New System.ArgumentException("attrName == null!")
			If dataType < DATATYPE_STRING OrElse dataType > DATATYPE_DOUBLE Then Throw New System.ArgumentException("Invalid value for dataType!")
			If enumeratedValues Is Nothing Then Throw New System.ArgumentException("enumeratedValues == null!")
			If enumeratedValues.Count = 0 Then Throw New System.ArgumentException("enumeratedValues is empty!")
			Dim iter As IEnumerator = enumeratedValues.GetEnumerator()
			Do While iter.hasNext()
				Dim o As Object = iter.next()
				If o Is Nothing Then Throw New System.ArgumentException("enumeratedValues contains a null!")
				If Not(TypeOf o Is String) Then Throw New System.ArgumentException("enumeratedValues contains a non-String value!")
			Loop

			Dim attr As New Attribute(Me)
			attr.attrName = attrName
			attr.valueType = VALUE_ENUMERATION
			attr.dataType = dataType
			attr.required = required
			attr.defaultValue = defaultValue
			attr.enumeratedValues = enumeratedValues

			___element.attrList.Add(attrName)
			___element.attrMap(attrName) = attr
		End Sub

		''' <summary>
		''' Adds a new attribute to a previously defined element that will
		''' be defined by a range of values.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute being added. </param>
		''' <param name="dataType"> the data type (string format) of the attribute,
		''' one of the <code>DATATYPE_*</code> constants. </param>
		''' <param name="required"> <code>true</code> if the attribute must be present. </param>
		''' <param name="defaultValue"> the default value for the attribute, or
		''' <code>null</code>. </param>
		''' <param name="minValue"> the smallest (inclusive or exclusive depending
		''' on the value of <code>minInclusive</code>) legal value for the
		''' attribute, as a <code>String</code>. </param>
		''' <param name="maxValue"> the largest (inclusive or exclusive depending
		''' on the value of <code>minInclusive</code>) legal value for the
		''' attribute, as a <code>String</code>. </param>
		''' <param name="minInclusive"> <code>true</code> if <code>minValue</code>
		''' is inclusive. </param>
		''' <param name="maxInclusive"> <code>true</code> if <code>maxValue</code>
		''' is inclusive.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>attrName</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of the predefined constants. </exception>
		Protected Friend Overridable Sub addAttribute(ByVal elementName As String, ByVal attrName As String, ByVal dataType As Integer, ByVal required As Boolean, ByVal defaultValue As String, ByVal minValue As String, ByVal maxValue As String, ByVal minInclusive As Boolean, ByVal maxInclusive As Boolean)
			Dim ___element As Element = getElement(elementName)
			If attrName Is Nothing Then Throw New System.ArgumentException("attrName == null!")
			If dataType < DATATYPE_STRING OrElse dataType > DATATYPE_DOUBLE Then Throw New System.ArgumentException("Invalid value for dataType!")

			Dim attr As New Attribute(Me)
			attr.attrName = attrName
			attr.valueType = VALUE_RANGE
			If minInclusive Then attr.valueType = attr.valueType Or VALUE_RANGE_MIN_INCLUSIVE_MASK
			If maxInclusive Then attr.valueType = attr.valueType Or VALUE_RANGE_MAX_INCLUSIVE_MASK
			attr.dataType = dataType
			attr.required = required
			attr.defaultValue = defaultValue
			attr.minValue = minValue
			attr.maxValue = maxValue

			___element.attrList.Add(attrName)
			___element.attrMap(attrName) = attr
		End Sub

		''' <summary>
		''' Adds a new attribute to a previously defined element that will
		''' be defined by a list of values.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute being added. </param>
		''' <param name="dataType"> the data type (string format) of the attribute,
		''' one of the <code>DATATYPE_*</code> constants. </param>
		''' <param name="required"> <code>true</code> if the attribute must be present. </param>
		''' <param name="listMinLength"> the smallest legal number of list items. </param>
		''' <param name="listMaxLength"> the largest legal number of list items.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>attrName</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of the predefined constants. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>listMinLength</code> is negative or larger than
		''' <code>listMaxLength</code>. </exception>
		Protected Friend Overridable Sub addAttribute(ByVal elementName As String, ByVal attrName As String, ByVal dataType As Integer, ByVal required As Boolean, ByVal listMinLength As Integer, ByVal listMaxLength As Integer)
			Dim ___element As Element = getElement(elementName)
			If attrName Is Nothing Then Throw New System.ArgumentException("attrName == null!")
			If dataType < DATATYPE_STRING OrElse dataType > DATATYPE_DOUBLE Then Throw New System.ArgumentException("Invalid value for dataType!")
			If listMinLength < 0 OrElse listMinLength > listMaxLength Then Throw New System.ArgumentException("Invalid list bounds!")

			Dim attr As New Attribute(Me)
			attr.attrName = attrName
			attr.valueType = VALUE_LIST
			attr.dataType = dataType
			attr.required = required
			attr.listMinLength = listMinLength
			attr.listMaxLength = listMaxLength

			___element.attrList.Add(attrName)
			___element.attrMap(attrName) = attr
		End Sub

		''' <summary>
		''' Adds a new attribute to a previously defined element that will
		''' be defined by the enumerated values <code>TRUE</code> and
		''' <code>FALSE</code>, with a datatype of
		''' <code>DATATYPE_BOOLEAN</code>.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute being added. </param>
		''' <param name="hasDefaultValue"> <code>true</code> if a default value
		''' should be present. </param>
		''' <param name="defaultValue"> the default value for the attribute as a
		''' <code>boolean</code>, ignored if <code>hasDefaultValue</code>
		''' is <code>false</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>attrName</code> is
		''' <code>null</code>. </exception>
		Protected Friend Overridable Sub addBooleanAttribute(ByVal elementName As String, ByVal attrName As String, ByVal hasDefaultValue As Boolean, ByVal defaultValue As Boolean)
			Dim values As IList = New ArrayList
			values.Add("TRUE")
			values.Add("FALSE")

			Dim dval As String = Nothing
			If hasDefaultValue Then dval = If(defaultValue, "TRUE", "FALSE")
			addAttribute(elementName, attrName, DATATYPE_BOOLEAN, True, dval, values)
		End Sub

		''' <summary>
		''' Removes an attribute from a previously defined element.  If no
		''' attribute with the given name was present in the given element,
		''' nothing happens and no exception is thrown.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute being removed.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this format. </exception>
		Protected Friend Overridable Sub removeAttribute(ByVal elementName As String, ByVal attrName As String)
			Dim ___element As Element = getElement(elementName)
			___element.attrList.Remove(attrName)
			___element.attrMap.Remove(attrName)
		End Sub

		''' <summary>
		''' Allows an <code>Object</code> reference of a given class type
		''' to be stored in nodes implementing the named element.  The
		''' value of the <code>Object</code> is unconstrained other than by
		''' its class type.
		''' 
		''' <p> If an <code>Object</code> reference was previously allowed,
		''' the previous settings are overwritten.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="classType"> a <code>Class</code> variable indicating the
		''' legal class type for the object value. </param>
		''' <param name="required"> <code>true</code> if an object value must be present. </param>
		''' <param name="defaultValue"> the default value for the
		''' <code>Object</code> reference, or <code>null</code>. </param>
		''' @param <T> the type of the object.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this format. </exception>
		Protected Friend Overridable Sub addObjectValue(Of T)(ByVal elementName As String, ByVal classType As Type, ByVal required As Boolean, ByVal defaultValue As T)
			Dim ___element As Element = getElement(elementName)
			Dim obj As New ObjectValue(Me)
			obj.valueType = VALUE_ARBITRARY
			obj.classType = classType
			obj.defaultValue = defaultValue

			___element.objectValue = obj
		End Sub

		''' <summary>
		''' Allows an <code>Object</code> reference of a given class type
		''' to be stored in nodes implementing the named element.  The
		''' value of the <code>Object</code> must be one of the values
		''' given by <code>enumeratedValues</code>.
		''' 
		''' <p> If an <code>Object</code> reference was previously allowed,
		''' the previous settings are overwritten.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="classType"> a <code>Class</code> variable indicating the
		''' legal class type for the object value. </param>
		''' <param name="required"> <code>true</code> if an object value must be present. </param>
		''' <param name="defaultValue"> the default value for the
		''' <code>Object</code> reference, or <code>null</code>. </param>
		''' <param name="enumeratedValues"> a <code>List</code> of
		''' <code>Object</code>s containing the legal values for the
		''' object reference. </param>
		''' @param <T> the type of the object.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this format. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>enumeratedValues</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>enumeratedValues</code> does not contain at least one
		''' entry. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>enumeratedValues</code> contains an element that is not
		''' an instance of the class type denoted by <code>classType</code>
		''' or is <code>null</code>. </exception>
		Protected Friend Overridable Sub addObjectValue(Of T, T1 As T)(ByVal elementName As String, ByVal classType As Type, ByVal required As Boolean, ByVal defaultValue As T, ByVal enumeratedValues As IList(Of T1))
			Dim ___element As Element = getElement(elementName)
			If enumeratedValues Is Nothing Then Throw New System.ArgumentException("enumeratedValues == null!")
			If enumeratedValues.Count = 0 Then Throw New System.ArgumentException("enumeratedValues is empty!")
			Dim iter As IEnumerator = enumeratedValues.GetEnumerator()
			Do While iter.hasNext()
				Dim o As Object = iter.next()
				If o Is Nothing Then Throw New System.ArgumentException("enumeratedValues contains a null!")
				If Not classType.IsInstanceOfType(o) Then Throw New System.ArgumentException("enumeratedValues contains a value not of class classType!")
			Loop

			Dim obj As New ObjectValue(Me)
			obj.valueType = VALUE_ENUMERATION
			obj.classType = classType
			obj.defaultValue = defaultValue
			obj.enumeratedValues = enumeratedValues

			___element.objectValue = obj
		End Sub

		''' <summary>
		''' Allows an <code>Object</code> reference of a given class type
		''' to be stored in nodes implementing the named element.  The
		''' value of the <code>Object</code> must be within the range given
		''' by <code>minValue</code> and <code>maxValue</code>.
		''' Furthermore, the class type must implement the
		''' <code>Comparable</code> interface.
		''' 
		''' <p> If an <code>Object</code> reference was previously allowed,
		''' the previous settings are overwritten.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="classType"> a <code>Class</code> variable indicating the
		''' legal class type for the object value. </param>
		''' <param name="defaultValue"> the default value for the </param>
		''' <param name="minValue"> the smallest (inclusive or exclusive depending
		''' on the value of <code>minInclusive</code>) legal value for the
		''' object value, as a <code>String</code>. </param>
		''' <param name="maxValue"> the largest (inclusive or exclusive depending
		''' on the value of <code>minInclusive</code>) legal value for the
		''' object value, as a <code>String</code>. </param>
		''' <param name="minInclusive"> <code>true</code> if <code>minValue</code>
		''' is inclusive. </param>
		''' <param name="maxInclusive"> <code>true</code> if <code>maxValue</code>
		''' is inclusive. </param>
		''' @param <T> the type of the object.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this
		''' format. </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend Overridable Sub addObjectValue(Of T As {Object, IComparable(Of ?)}, T1, T2)(ByVal elementName As String, ByVal classType As Type, ByVal defaultValue As T, ByVal minValue As IComparable(Of T1), ByVal maxValue As IComparable(Of T2), ByVal minInclusive As Boolean, ByVal maxInclusive As Boolean)
			Dim ___element As Element = getElement(elementName)
			Dim obj As New ObjectValue(Me)
			obj.valueType = VALUE_RANGE
			If minInclusive Then obj.valueType = obj.valueType Or VALUE_RANGE_MIN_INCLUSIVE_MASK
			If maxInclusive Then obj.valueType = obj.valueType Or VALUE_RANGE_MAX_INCLUSIVE_MASK
			obj.classType = classType
			obj.defaultValue = defaultValue
			obj.minValue = minValue
			obj.maxValue = maxValue

			___element.objectValue = obj
		End Sub

		''' <summary>
		''' Allows an <code>Object</code> reference of a given class type
		''' to be stored in nodes implementing the named element.  The
		''' value of the <code>Object</code> must an array of objects of
		''' class type given by <code>classType</code>, with at least
		''' <code>arrayMinLength</code> and at most
		''' <code>arrayMaxLength</code> elements.
		''' 
		''' <p> If an <code>Object</code> reference was previously allowed,
		''' the previous settings are overwritten.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="classType"> a <code>Class</code> variable indicating the
		''' legal class type for the object value. </param>
		''' <param name="arrayMinLength"> the smallest legal length for the array. </param>
		''' <param name="arrayMaxLength"> the largest legal length for the array.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code> is
		''' not a legal element name for this format. </exception>
		Protected Friend Overridable Sub addObjectValue(ByVal elementName As String, ByVal classType As Type, ByVal arrayMinLength As Integer, ByVal arrayMaxLength As Integer)
			Dim ___element As Element = getElement(elementName)
			Dim obj As New ObjectValue(Me)
			obj.valueType = VALUE_LIST
			obj.classType = classType
			obj.arrayMinLength = arrayMinLength
			obj.arrayMaxLength = arrayMaxLength

			___element.objectValue = obj
		End Sub

		''' <summary>
		''' Disallows an <code>Object</code> reference from being stored in
		''' nodes implementing the named element.
		''' </summary>
		''' <param name="elementName"> the name of the element.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code> is
		''' not a legal element name for this format. </exception>
		Protected Friend Overridable Sub removeObjectValue(ByVal elementName As String)
			Dim ___element As Element = getElement(elementName)
			___element.objectValue = Nothing
		End Sub

		' Utility method

		' Methods from IIOMetadataFormat

		' Root

		Public Overridable Property rootName As String Implements IIOMetadataFormat.getRootName
			Get
				Return rootName
			End Get
		End Property

		' Multiplicity

		Public MustOverride Function canNodeAppear(ByVal elementName As String, ByVal imageType As javax.imageio.ImageTypeSpecifier) As Boolean Implements IIOMetadataFormat.canNodeAppear

		Public Overridable Function getElementMinChildren(ByVal elementName As String) As Integer Implements IIOMetadataFormat.getElementMinChildren
			Dim ___element As Element = getElement(elementName)
			If ___element.childPolicy <> CHILD_POLICY_REPEAT Then Throw New System.ArgumentException("Child policy not CHILD_POLICY_REPEAT!")
			Return ___element.minChildren
		End Function

		Public Overridable Function getElementMaxChildren(ByVal elementName As String) As Integer Implements IIOMetadataFormat.getElementMaxChildren
			Dim ___element As Element = getElement(elementName)
			If ___element.childPolicy <> CHILD_POLICY_REPEAT Then Throw New System.ArgumentException("Child policy not CHILD_POLICY_REPEAT!")
			Return ___element.maxChildren
		End Function

		Private Function getResource(ByVal key As String, ByVal locale As java.util.Locale) As String
			If locale Is Nothing Then locale = java.util.Locale.default

			''' <summary>
			''' If an applet supplies an implementation of IIOMetadataFormat and
			''' resource bundles, then the resource bundle will need to be
			''' accessed via the applet class loader. So first try the context
			''' class loader to locate the resource bundle.
			''' If that throws MissingResourceException, then try the
			''' system class loader.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			ClassLoader loader = (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'		{
	'				   public Object run()
	'				   {
	'					   Return Thread.currentThread().getContextClassLoader();
	'				   }
	'			});

			Dim bundle As java.util.ResourceBundle = Nothing
			Try
				bundle = java.util.ResourceBundle.getBundle(resourceBaseName, locale, loader)
			Catch mre As java.util.MissingResourceException
				Try
					bundle = java.util.ResourceBundle.getBundle(resourceBaseName, locale)
				Catch mre1 As java.util.MissingResourceException
					Return Nothing
				End Try
			End Try

			Try
				Return bundle.getString(key)
			Catch e As java.util.MissingResourceException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Returns a <code>String</code> containing a description of the
		''' named element, or <code>null</code>.  The description will be
		''' localized for the supplied <code>Locale</code> if possible.
		''' 
		''' <p> The default implementation will first locate a
		''' <code>ResourceBundle</code> using the current resource base
		''' name set by <code>setResourceBaseName</code> and the supplied
		''' <code>Locale</code>, using the fallback mechanism described in
		''' the comments for <code>ResourceBundle.getBundle</code>.  If a
		''' <code>ResourceBundle</code> is found, the element name will be
		''' used as a key to its <code>getString</code> method, and the
		''' result returned.  If no <code>ResourceBundle</code> is found,
		''' or no such key is present, <code>null</code> will be returned.
		''' 
		''' <p> If <code>locale</code> is <code>null</code>, the current
		''' default <code>Locale</code> returned by <code>Locale.getLocale</code>
		''' will be used.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="locale"> the <code>Locale</code> for which localization
		''' will be attempted.
		''' </param>
		''' <returns> the element description.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this format.
		''' </exception>
		''' <seealso cref= #setResourceBaseName </seealso>
		Public Overridable Function getElementDescription(ByVal elementName As String, ByVal locale As java.util.Locale) As String Implements IIOMetadataFormat.getElementDescription
			Dim ___element As Element = getElement(elementName)
			Return getResource(elementName, locale)
		End Function

		' Children

		Public Overridable Function getChildPolicy(ByVal elementName As String) As Integer Implements IIOMetadataFormat.getChildPolicy
			Dim ___element As Element = getElement(elementName)
			Return ___element.childPolicy
		End Function

		Public Overridable Function getChildNames(ByVal elementName As String) As String() Implements IIOMetadataFormat.getChildNames
			Dim ___element As Element = getElement(elementName)
			If ___element.childPolicy = CHILD_POLICY_EMPTY Then Return Nothing
			Return CType(___element.childList.ToArray(GetType(String)), String())
		End Function

		' Attributes

		Public Overridable Function getAttributeNames(ByVal elementName As String) As String() Implements IIOMetadataFormat.getAttributeNames
			Dim ___element As Element = getElement(elementName)
			Dim names As IList = ___element.attrList

			Dim result As String() = New String(names.Count - 1){}
			Return CType(names.ToArray(result), String())
		End Function

		Public Overridable Function getAttributeValueType(ByVal elementName As String, ByVal attrName As String) As Integer Implements IIOMetadataFormat.getAttributeValueType
			Dim attr As Attribute = getAttribute(elementName, attrName)
			Return attr.valueType
		End Function

		Public Overridable Function getAttributeDataType(ByVal elementName As String, ByVal attrName As String) As Integer Implements IIOMetadataFormat.getAttributeDataType
			Dim attr As Attribute = getAttribute(elementName, attrName)
			Return attr.dataType
		End Function

		Public Overridable Function isAttributeRequired(ByVal elementName As String, ByVal attrName As String) As Boolean Implements IIOMetadataFormat.isAttributeRequired
			Dim attr As Attribute = getAttribute(elementName, attrName)
			Return attr.required
		End Function

		Public Overridable Function getAttributeDefaultValue(ByVal elementName As String, ByVal attrName As String) As String Implements IIOMetadataFormat.getAttributeDefaultValue
			Dim attr As Attribute = getAttribute(elementName, attrName)
			Return attr.defaultValue
		End Function

		Public Overridable Function getAttributeEnumerations(ByVal elementName As String, ByVal attrName As String) As String() Implements IIOMetadataFormat.getAttributeEnumerations
			Dim attr As Attribute = getAttribute(elementName, attrName)
			If attr.valueType <> VALUE_ENUMERATION Then Throw New System.ArgumentException("Attribute not an enumeration!")

			Dim values As IList = attr.enumeratedValues
			Dim iter As IEnumerator = values.GetEnumerator()
			Dim result As String() = New String(values.Count - 1){}
			Return CType(values.ToArray(result), String())
		End Function

		Public Overridable Function getAttributeMinValue(ByVal elementName As String, ByVal attrName As String) As String Implements IIOMetadataFormat.getAttributeMinValue
			Dim attr As Attribute = getAttribute(elementName, attrName)
			If attr.valueType <> VALUE_RANGE AndAlso attr.valueType <> VALUE_RANGE_MIN_INCLUSIVE AndAlso attr.valueType <> VALUE_RANGE_MAX_INCLUSIVE AndAlso attr.valueType <> VALUE_RANGE_MIN_MAX_INCLUSIVE Then Throw New System.ArgumentException("Attribute not a range!")

			Return attr.minValue
		End Function

		Public Overridable Function getAttributeMaxValue(ByVal elementName As String, ByVal attrName As String) As String Implements IIOMetadataFormat.getAttributeMaxValue
			Dim attr As Attribute = getAttribute(elementName, attrName)
			If attr.valueType <> VALUE_RANGE AndAlso attr.valueType <> VALUE_RANGE_MIN_INCLUSIVE AndAlso attr.valueType <> VALUE_RANGE_MAX_INCLUSIVE AndAlso attr.valueType <> VALUE_RANGE_MIN_MAX_INCLUSIVE Then Throw New System.ArgumentException("Attribute not a range!")

			Return attr.maxValue
		End Function

		Public Overridable Function getAttributeListMinLength(ByVal elementName As String, ByVal attrName As String) As Integer Implements IIOMetadataFormat.getAttributeListMinLength
			Dim attr As Attribute = getAttribute(elementName, attrName)
			If attr.valueType <> VALUE_LIST Then Throw New System.ArgumentException("Attribute not a list!")

			Return attr.listMinLength
		End Function

		Public Overridable Function getAttributeListMaxLength(ByVal elementName As String, ByVal attrName As String) As Integer Implements IIOMetadataFormat.getAttributeListMaxLength
			Dim attr As Attribute = getAttribute(elementName, attrName)
			If attr.valueType <> VALUE_LIST Then Throw New System.ArgumentException("Attribute not a list!")

			Return attr.listMaxLength
		End Function

		''' <summary>
		''' Returns a <code>String</code> containing a description of the
		''' named attribute, or <code>null</code>.  The description will be
		''' localized for the supplied <code>Locale</code> if possible.
		''' 
		''' <p> The default implementation will first locate a
		''' <code>ResourceBundle</code> using the current resource base
		''' name set by <code>setResourceBaseName</code> and the supplied
		''' <code>Locale</code>, using the fallback mechanism described in
		''' the comments for <code>ResourceBundle.getBundle</code>.  If a
		''' <code>ResourceBundle</code> is found, the element name followed
		''' by a "/" character followed by the attribute name
		''' (<code>elementName + "/" + attrName</code>) will be used as a
		''' key to its <code>getString</code> method, and the result
		''' returned.  If no <code>ResourceBundle</code> is found, or no
		''' such key is present, <code>null</code> will be returned.
		''' 
		''' <p> If <code>locale</code> is <code>null</code>, the current
		''' default <code>Locale</code> returned by <code>Locale.getLocale</code>
		''' will be used.
		''' </summary>
		''' <param name="elementName"> the name of the element. </param>
		''' <param name="attrName"> the name of the attribute. </param>
		''' <param name="locale"> the <code>Locale</code> for which localization
		''' will be attempted, or <code>null</code>.
		''' </param>
		''' <returns> the attribute description.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>elementName</code>
		''' is <code>null</code>, or is not a legal element name for this format. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>attrName</code> is
		''' <code>null</code> or is not a legal attribute name for this
		''' element.
		''' </exception>
		''' <seealso cref= #setResourceBaseName </seealso>
		Public Overridable Function getAttributeDescription(ByVal elementName As String, ByVal attrName As String, ByVal locale As java.util.Locale) As String Implements IIOMetadataFormat.getAttributeDescription
			Dim ___element As Element = getElement(elementName)
			If attrName Is Nothing Then Throw New System.ArgumentException("attrName == null!")
			Dim attr As Attribute = CType(___element.attrMap(attrName), Attribute)
			If attr Is Nothing Then Throw New System.ArgumentException("No such attribute!")

			Dim key As String = elementName & "/" & attrName
			Return getResource(key, locale)
		End Function

		Private Function getObjectValue(ByVal elementName As String) As ObjectValue
			Dim ___element As Element = getElement(elementName)
			Dim objv As ObjectValue = CType(___element.objectValue, ObjectValue)
			If objv Is Nothing Then Throw New System.ArgumentException("No object within element " & elementName & "!")
			Return objv
		End Function

		Public Overridable Function getObjectValueType(ByVal elementName As String) As Integer Implements IIOMetadataFormat.getObjectValueType
			Dim ___element As Element = getElement(elementName)
			Dim objv As ObjectValue = CType(___element.objectValue, ObjectValue)
			If objv Is Nothing Then Return VALUE_NONE
			Return objv.valueType
		End Function

		Public Overridable Function getObjectClass(ByVal elementName As String) As Type Implements IIOMetadataFormat.getObjectClass
			Dim objv As ObjectValue = getObjectValue(elementName)
			Return objv.classType
		End Function

		Public Overridable Function getObjectDefaultValue(ByVal elementName As String) As Object Implements IIOMetadataFormat.getObjectDefaultValue
			Dim objv As ObjectValue = getObjectValue(elementName)
			Return objv.defaultValue
		End Function

		Public Overridable Function getObjectEnumerations(ByVal elementName As String) As Object() Implements IIOMetadataFormat.getObjectEnumerations
			Dim objv As ObjectValue = getObjectValue(elementName)
			If objv.valueType <> VALUE_ENUMERATION Then Throw New System.ArgumentException("Not an enumeration!")
			Dim vlist As IList = objv.enumeratedValues
			Dim values As Object() = New Object(vlist.Count - 1){}
			Return vlist.ToArray(values)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function getObjectMinValue(ByVal elementName As String) As IComparable(Of ?) Implements IIOMetadataFormat.getObjectMinValue
			Dim objv As ObjectValue = getObjectValue(elementName)
			If (objv.valueType And VALUE_RANGE) <> VALUE_RANGE Then Throw New System.ArgumentException("Not a range!")
			Return objv.minValue
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function getObjectMaxValue(ByVal elementName As String) As IComparable(Of ?) Implements IIOMetadataFormat.getObjectMaxValue
			Dim objv As ObjectValue = getObjectValue(elementName)
			If (objv.valueType And VALUE_RANGE) <> VALUE_RANGE Then Throw New System.ArgumentException("Not a range!")
			Return objv.maxValue
		End Function

		Public Overridable Function getObjectArrayMinLength(ByVal elementName As String) As Integer Implements IIOMetadataFormat.getObjectArrayMinLength
			Dim objv As ObjectValue = getObjectValue(elementName)
			If objv.valueType <> VALUE_LIST Then Throw New System.ArgumentException("Not a list!")
			Return objv.arrayMinLength
		End Function

		Public Overridable Function getObjectArrayMaxLength(ByVal elementName As String) As Integer Implements IIOMetadataFormat.getObjectArrayMaxLength
			Dim objv As ObjectValue = getObjectValue(elementName)
			If objv.valueType <> VALUE_LIST Then Throw New System.ArgumentException("Not a list!")
			Return objv.arrayMaxLength
		End Function

		' Standard format descriptor

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub createStandardFormat()
			If standardFormat Is Nothing Then standardFormat = New com.sun.imageio.plugins.common.StandardMetadataFormat
		End Sub

		''' <summary>
		''' Returns an <code>IIOMetadataFormat</code> object describing the
		''' standard, plug-in neutral <code>javax.imageio_1.0</code>
		''' metadata document format described in the comment of the
		''' <code>javax.imageio.metadata</code> package.
		''' </summary>
		''' <returns> a predefined <code>IIOMetadataFormat</code> instance. </returns>
		Public Property Shared standardFormatInstance As IIOMetadataFormat
			Get
				createStandardFormat()
				Return standardFormat
			End Get
		End Property
	End Class

End Namespace
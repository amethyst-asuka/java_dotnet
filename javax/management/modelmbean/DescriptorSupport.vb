Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
import static com.sun.jmx.defaults.JmxProperties.MODELMBEAN_LOGGER
import static com.sun.jmx.mbeanserver.Util.cast

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
'
' * @author    IBM Corp.
' *
' * Copyright IBM Corp. 1999-2000.  All rights reserved.
' 

Namespace javax.management.modelmbean







	''' <summary>
	''' This class represents the metadata set for a ModelMBean element.  A
	''' descriptor is part of the ModelMBeanInfo,
	''' ModelMBeanNotificationInfo, ModelMBeanAttributeInfo,
	''' ModelMBeanConstructorInfo, and ModelMBeanParameterInfo.
	''' <P>
	''' A descriptor consists of a collection of fields.  Each field is in
	''' fieldname=fieldvalue format.  Field names are not case sensitive,
	''' case will be preserved on field values.
	''' <P>
	''' All field names and values are not predefined. New fields can be
	''' defined and added by any program.  Some fields have been predefined
	''' for consistency of implementation and support by the
	''' ModelMBeanInfo, ModelMBeanAttributeInfo, ModelMBeanConstructorInfo,
	''' ModelMBeanNotificationInfo, ModelMBeanOperationInfo and ModelMBean
	''' classes.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-6292969195866300415L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class DescriptorSupport
		Implements javax.management.Descriptor ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 8071560848919417985L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -6292969195866300415L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("descriptor", GetType(Hashtable)), New java.io.ObjectStreamField("currClass", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("descriptor", GetType(Hashtable)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField descriptor HashMap The collection of fields representing this descriptor
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private Shared ReadOnly serialForm As String
		Shared Sub New()
			Dim form As String = Nothing
			Dim compat As Boolean = False
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				form = java.security.AccessController.doPrivileged(act)
				compat = "1.0".Equals(form) ' form may be null
			Catch e As Exception
				' OK: No compat with 1.0
			End Try
			serialForm = form
			If compat Then
				serialPersistentFields = oldSerialPersistentFields
				serialVersionUID = oldSerialVersionUID
			Else
				serialPersistentFields = newSerialPersistentFields
				serialVersionUID = newSerialVersionUID
			End If
			Dim maxChar As Char = 0
			For i As Integer = 0 To entities.Length - 1
				Dim c As Char = entities(i).Chars(0)
				If c > maxChar Then maxChar = c
			Next i
			charToEntityMap = New String(AscW(maxChar)){}
			For i As Integer = 0 To entities.Length - 1
				Dim c As Char = entities(i).Chars(0)
				Dim entity As String = entities(i).Substring(1)
				charToEntityMap(AscW(c)) = entity
				entityToCharMap(entity) = c
			Next i
		End Sub
		'
		' END Serialization compatibility stuff

	'     Spec says that field names are case-insensitive, but that case
	'       is preserved.  This means that we need to be able to map from a
	'       name that may differ in case to the actual name that is used in
	'       the HashMap.  Thus, descriptorMap is a TreeMap with a Comparator
	'       that ignores case.
	'
	'       Previous versions of this class had a field called "descriptor"
	'       of type HashMap where the keys were directly Strings.  This is
	'       hard to reconcile with the required semantics, so we fabricate
	'       that field virtually during serialization and deserialization
	'       but keep the real information in descriptorMap.
	'    
		<NonSerialized> _
		Private descriptorMap As java.util.SortedMap(Of String, Object)

		Private Const currClass As String = "DescriptorSupport"


		''' <summary>
		''' Descriptor default constructor.
		''' Default initial descriptor size is 20.  It will grow as needed.<br>
		''' Note that the created empty descriptor is not a valid descriptor
		''' (the method <seealso cref="#isValid isValid"/> returns <CODE>false</CODE>)
		''' </summary>
		Public Sub New()
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "DescriptorSupport()", "Constructor")
			init(Nothing)
		End Sub

		''' <summary>
		''' Descriptor constructor.  Takes as parameter the initial
		''' capacity of the Map that stores the descriptor fields.
		''' Capacity will grow as needed.<br> Note that the created empty
		''' descriptor is not a valid descriptor (the method {@link
		''' #isValid isValid} returns <CODE>false</CODE>).
		''' </summary>
		''' <param name="initNumFields"> The initial capacity of the Map that
		''' stores the descriptor fields.
		''' </param>
		''' <exception cref="RuntimeOperationsException"> for illegal value for
		''' initNumFields (&lt;= 0) </exception>
		''' <exception cref="MBeanException"> Wraps a distributed communication Exception. </exception>
		Public Sub New(ByVal initNumFields As Integer)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(initNumFields = " & initNumFields & ")", "Constructor")
			If initNumFields <= 0 Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(initNumFields)", "Illegal arguments: initNumFields <= 0")
				Dim msg As String = "Descriptor field limit invalid: " & initNumFields
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If
			init(Nothing)
		End Sub

		''' <summary>
		''' Descriptor constructor taking a Descriptor as parameter.
		''' Creates a new descriptor initialized to the values of the
		''' descriptor passed in parameter.
		''' </summary>
		''' <param name="inDescr"> the descriptor to be used to initialize the
		''' constructed descriptor. If it is null or contains no descriptor
		''' fields, an empty Descriptor will be created. </param>
		Public Sub New(ByVal inDescr As DescriptorSupport)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(Descriptor)", "Constructor")
			If inDescr Is Nothing Then
				init(Nothing)
			Else
				init(inDescr.descriptorMap)
			End If
		End Sub


		''' <summary>
		''' <p>Descriptor constructor taking an XML String.</p>
		''' 
		''' <p>The format of the XML string is not defined, but an
		''' implementation must ensure that the string returned by
		''' <seealso cref="#toXMLString() toXMLString()"/> on an existing
		''' descriptor can be used to instantiate an equivalent
		''' descriptor using this constructor.</p>
		''' 
		''' <p>In this implementation, all field values will be created
		''' as Strings.  If the field values are not Strings, the
		''' programmer will have to reset or convert these fields
		''' correctly.</p>
		''' </summary>
		''' <param name="inStr"> An XML-formatted string used to populate this
		''' Descriptor.  The format is not defined, but any
		''' implementation must ensure that the string returned by
		''' method <seealso cref="#toXMLString toXMLString"/> on an existing
		''' descriptor can be used to instantiate an equivalent
		''' descriptor when instantiated using this constructor.
		''' </param>
		''' <exception cref="RuntimeOperationsException"> If the String inStr
		''' passed in parameter is null </exception>
		''' <exception cref="XMLParseException"> XML parsing problem while parsing
		''' the input String </exception>
		''' <exception cref="MBeanException"> Wraps a distributed communication Exception. </exception>
	'     At some stage we should rewrite this code to be cleverer.  Using
	'       a StringTokenizer as we do means, first, that we accept a lot of
	'       bogus strings without noticing they are bogus, and second, that we
	'       split the string being parsed at characters like > even if they
	'       occur in the middle of a field value. 
		Public Sub New(ByVal inStr As String)
	'         parse an XML-formatted string and populate internal
	'         * structure with it 
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(String = '" & inStr & "')", "Constructor")
			If inStr Is Nothing Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(String = null)", "Illegal arguments")
				Const msg As String = "String in parameter is null"
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If

			Dim lowerInStr As String = inStr.ToLower()
			If (Not lowerInStr.StartsWith("<descriptor>")) OrElse (Not lowerInStr.EndsWith("</descriptor>")) Then Throw New XMLParseException("No <descriptor>, </descriptor> pair")

			' parse xmlstring into structures
			init(Nothing)
			' create dummy descriptor: should have same size
			' as number of fields in xmlstring
			' loop through structures and put them in descriptor

			Dim st As New java.util.StringTokenizer(inStr, "<> " & vbTab & vbLf & vbCr & vbFormFeed)

			Dim inFld As Boolean = False
			Dim inDesc As Boolean = False
			Dim fieldName As String = Nothing
			Dim ___fieldValue As String = Nothing


			Do While st.hasMoreTokens() ' loop through tokens
				Dim tok As String = st.nextToken()

				If tok.ToUpper() = "FIELD".ToUpper() Then
					inFld = True
				ElseIf tok.ToUpper() = "/FIELD".ToUpper() Then
					If (fieldName IsNot Nothing) AndAlso (___fieldValue IsNot Nothing) Then
						fieldName = fieldName.Substring(fieldName.IndexOf(""""c) + 1, fieldName.LastIndexOf(""""c) - (fieldName.IndexOf(""""c) + 1))
						Dim fieldValueObject As Object = parseQuotedFieldValue(___fieldValue)
						fieldeld(fieldName, fieldValueObject)
					End If
					fieldName = Nothing
					___fieldValue = Nothing
					inFld = False
				ElseIf tok.ToUpper() = "DESCRIPTOR".ToUpper() Then
					inDesc = True
				ElseIf tok.ToUpper() = "/DESCRIPTOR".ToUpper() Then
					inDesc = False
					fieldName = Nothing
					___fieldValue = Nothing
					inFld = False
				ElseIf inFld AndAlso inDesc Then
					' want kw=value, eg, name="myname" value="myvalue"
					Dim eq_separator As Integer = tok.IndexOf("=")
					If eq_separator > 0 Then
						Dim kwPart As String = tok.Substring(0,eq_separator)
						Dim valPart As String = tok.Substring(eq_separator+1)
						If kwPart.ToUpper() = "NAME".ToUpper() Then
							fieldName = valPart
						ElseIf kwPart.ToUpper() = "VALUE".ToUpper() Then
							___fieldValue = valPart
						Else ' xml parse exception
							Dim msg As String = "Expected `name' or `value', got `" & tok & "'"
							Throw New XMLParseException(msg)
						End If ' xml parse exception
					Else
						Dim msg As String = "Expected `keyword=value', got `" & tok & "'"
						Throw New XMLParseException(msg)
					End If
				End If
			Loop ' while tokens
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(XMLString)", "Exit")
		End Sub

		''' <summary>
		''' Constructor taking field names and field values.  Neither array
		''' can be null.
		''' </summary>
		''' <param name="fieldNames"> String array of field names.  No elements of
		''' this array can be null. </param>
		''' <param name="fieldValues"> Object array of the corresponding field
		''' values.  Elements of the array can be null. The
		''' <code>fieldValue</code> must be valid for the
		''' <code>fieldName</code> (as defined in method {@link #isValid
		''' isValid})
		''' 
		''' <p>Note: array sizes of parameters should match. If both arrays
		''' are empty, then an empty descriptor is created.</p>
		''' </param>
		''' <exception cref="RuntimeOperationsException"> for illegal value for
		''' field Names or field Values.  The array lengths must be equal.
		''' If the descriptor construction fails for any reason, this
		''' exception will be thrown.
		'''  </exception>
		Public Sub New(ByVal fieldNames As String(), ByVal fieldValues As Object())
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(fieldNames,fieldObjects)", "Constructor")

			If (fieldNames Is Nothing) OrElse (fieldValues Is Nothing) OrElse (fieldNames.Length <> fieldValues.Length) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(fieldNames,fieldObjects)", "Illegal arguments")

				Const msg As String = "Null or invalid fieldNames or fieldValues"
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If

			' populate internal structure with fields 
			init(Nothing)
			For i As Integer = 0 To fieldNames.Length - 1
				' setField will throw an exception if a fieldName is be null.
				' the fieldName and fieldValue will be validated in setField.
				fieldeld(fieldNames(i), fieldValues(i))
			Next i
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(fieldNames,fieldObjects)", "Exit")
		End Sub

		''' <summary>
		''' Constructor taking fields in the <i>fieldName=fieldValue</i>
		''' format.
		''' </summary>
		''' <param name="fields"> String array with each element containing a
		''' field name and value.  If this array is null or empty, then the
		''' default constructor will be executed. Null strings or empty
		''' strings will be ignored.
		''' 
		''' <p>All field values should be Strings.  If the field values are
		''' not Strings, the programmer will have to reset or convert these
		''' fields correctly.
		''' 
		''' <p>Note: Each string should be of the form
		''' <i>fieldName=fieldValue</i>.  The field name
		''' ends at the first {@code =} character; for example if the String
		''' is {@code a=b=c} then the field name is {@code a} and its value
		''' is {@code b=c}.
		''' </param>
		''' <exception cref="RuntimeOperationsException"> for illegal value for
		''' field Names or field Values.  The field must contain an
		''' "=". "=fieldValue", "fieldName", and "fieldValue" are illegal.
		''' FieldName cannot be null.  "fieldName=" will cause the value to
		''' be null.  If the descriptor construction fails for any reason,
		''' this exception will be thrown.
		'''  </exception>
		Public Sub New(ParamArray ByVal fields As String())
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(String... fields)", "Constructor")
			init(Nothing)
			If (fields Is Nothing) OrElse (fields.Length = 0) Then Return

			init(Nothing)

			For i As Integer = 0 To fields.Length - 1
				If (fields(i) Is Nothing) OrElse (fields(i).Equals("")) Then Continue For
				Dim eq_separator As Integer = fields(i).IndexOf("=")
				If eq_separator < 0 Then
					' illegal if no = or is first character
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(String... fields)", "Illegal arguments: field does not have " & "'=' as a name and value separator")
					Const msg As String = "Field in invalid format: no equals sign"
					Dim iae As Exception = New System.ArgumentException(msg)
					Throw New javax.management.RuntimeOperationsException(iae, msg)
				End If

				Dim fieldName As String = fields(i).Substring(0,eq_separator)
				Dim ___fieldValue As String = Nothing
				If eq_separator < fields(i).Length Then ___fieldValue = fields(i).Substring(eq_separator+1)

				If fieldName.Equals("") Then
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(String... fields)", "Illegal arguments: fieldName is empty")

					Const msg As String = "Field in invalid format: no fieldName"
					Dim iae As Exception = New System.ArgumentException(msg)
					Throw New javax.management.RuntimeOperationsException(iae, msg)
				End If

				fieldeld(fieldName,___fieldValue)
			Next i
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "Descriptor(String... fields)", "Exit")
		End Sub

		Private Sub init(Of T1)(ByVal initMap As IDictionary(Of T1))
			descriptorMap = New SortedDictionary(Of String, Object)(String.CASE_INSENSITIVE_ORDER)
			If initMap IsNot Nothing Then descriptorMap.putAll(initMap)
		End Sub

		' Implementation of the Descriptor interface


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getFieldValue(ByVal fieldName As String) As Object

			If (fieldName Is Nothing) OrElse (fieldName.Equals("")) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldValue(String fieldName)", "Illegal arguments: null field name")
				Const msg As String = "Fieldname requested is null"
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If
			Dim retValue As Object = descriptorMap.get(fieldName)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldValue(String fieldName = " & fieldName & ")", "Returns '" & retValue & "'")
			Return (retValue)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub setField(ByVal fieldName As String, ByVal fieldValue As Object)

			' field name cannot be null or empty
			If (fieldName Is Nothing) OrElse (fieldName.Equals("")) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setField(fieldName,fieldValue)", "Illegal arguments: null or empty field name")

				Const msg As String = "Field name to be set is null or empty"
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If

			If Not validateField(fieldName, fieldValue) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setField(fieldName,fieldValue)", "Illegal arguments")

				Dim msg As String = "Field value invalid: " & fieldName & "=" & fieldValue
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setField(fieldName,fieldValue)", "Entry: setting '" & fieldName & "' to '" & fieldValue & "'")

			' Since we do not remove any existing entry with this name,
			' the field will preserve whatever case it had, ignoring
			' any difference there might be in fieldName.
			descriptorMap.put(fieldName, fieldValue)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property fields As String()
			Get
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFields()", "Entry")
				Dim numberOfEntries As Integer = descriptorMap.size()
    
				Dim responseFields As String() = New String(numberOfEntries - 1){}
				Dim returnedSet As java.util.Set(Of KeyValuePair(Of String, Object)) = descriptorMap.entrySet()
    
				Dim i As Integer = 0
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFields()", "Returning " & numberOfEntries & " fields")
				Dim iter As IEnumerator(Of KeyValuePair(Of String, Object)) = descriptorMap.GetEnumerator()
				Do While iter.MoveNext()
					Dim currElement As KeyValuePair(Of String, Object) = iter.Current
    
					If currElement Is Nothing Then
						If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFields()", "Element is null")
					Else
						Dim currValue As Object = currElement.Value
						If currValue Is Nothing Then
							responseFields(i) = currElement.Key & "="
						Else
							If TypeOf currValue Is String Then
								responseFields(i) = currElement.Key & "=" & currValue.ToString()
							Else
								responseFields(i) = currElement.Key & "=(" & currValue.ToString() & ")"
							End If
						End If
					End If
					i += 1
				Loop
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFields()", "Exit")
    
				Return responseFields
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property fieldNames As String()
			Get
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldNames()", "Entry")
				Dim numberOfEntries As Integer = descriptorMap.size()
    
				Dim responseFields As String() = New String(numberOfEntries - 1){}
				Dim returnedSet As java.util.Set(Of KeyValuePair(Of String, Object)) = descriptorMap.entrySet()
    
				Dim i As Integer = 0
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldNames()", "Returning " & numberOfEntries & " fields")
    
				Dim iter As IEnumerator(Of KeyValuePair(Of String, Object)) = descriptorMap.GetEnumerator()
				Do While iter.MoveNext()
					Dim currElement As KeyValuePair(Of String, Object) = iter.Current
    
					If (currElement Is Nothing) OrElse (currElement.Key Is Nothing) Then
						If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldNames()", "Field is null")
					Else
						responseFields(i) = currElement.Key.ToString()
					End If
					i += 1
				Loop
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldNames()", "Exit")
    
				Return responseFields
			End Get
		End Property


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getFieldValues(ParamArray ByVal fieldNames As String()) As Object()
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldValues(String... fieldNames)", "Entry")
			' if fieldNames == null return all values
			' if fieldNames is String[0] return no values

			Dim numberOfEntries As Integer = If(fieldNames Is Nothing, descriptorMap.size(), fieldNames.Length)
			Dim responseFields As Object() = New Object(numberOfEntries - 1){}

			Dim i As Integer = 0

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldValues(String... fieldNames)", "Returning " & numberOfEntries & " fields")

			If fieldNames Is Nothing Then
				For Each value As Object In descriptorMap.values()
					responseFields(i) = value
					i += 1
				Next value
			Else
				For i = 0 To fieldNames.Length - 1
					If (fieldNames(i) Is Nothing) OrElse (fieldNames(i).Equals("")) Then
						responseFields(i) = Nothing
					Else
						responseFields(i) = getFieldValue(fieldNames(i))
					End If
				Next i
			End If

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "getFieldValues(String... fieldNames)", "Exit")

			Return responseFields
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub setFields(ByVal fieldNames As String(), ByVal fieldValues As Object())

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setFields(fieldNames,fieldValues)", "Entry")

			If (fieldNames Is Nothing) OrElse (fieldValues Is Nothing) OrElse (fieldNames.Length <> fieldValues.Length) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setFields(fieldNames,fieldValues)", "Illegal arguments")

				Const msg As String = "fieldNames and fieldValues are null or invalid"
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, msg)
			End If

			For i As Integer = 0 To fieldNames.Length - 1
				If (fieldNames(i) Is Nothing) OrElse (fieldNames(i).Equals("")) Then
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setFields(fieldNames,fieldValues)", "Null field name encountered at element " & i)
					Const msg As String = "fieldNames is null or invalid"
					Dim iae As Exception = New System.ArgumentException(msg)
					Throw New javax.management.RuntimeOperationsException(iae, msg)
				End If
				fieldeld(fieldNames(i), fieldValues(i))
			Next i
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "setFields(fieldNames,fieldValues)", "Exit")
		End Sub

		''' <summary>
		''' Returns a new Descriptor which is a duplicate of the Descriptor.
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> for illegal value for
		''' field Names or field Values.  If the descriptor construction
		''' fails for any reason, this exception will be thrown. </exception>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function clone() As Object
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "clone()", "Entry")
			Return (New DescriptorSupport(Me))
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeField(ByVal fieldName As String)
			If (fieldName Is Nothing) OrElse (fieldName.Equals("")) Then Return

			descriptorMap.remove(fieldName)
		End Sub

		''' <summary>
		''' Compares this descriptor to the given object.  The objects are equal if
		''' the given object is also a Descriptor, and if the two Descriptors have
		''' the same field names (possibly differing in case) and the same
		''' associated values.  The respective values for a field in the two
		''' Descriptors are equal if the following conditions hold:
		''' 
		''' <ul>
		''' <li>If one value is null then the other must be too.</li>
		''' <li>If one value is a primitive array then the other must be a primitive
		''' array of the same type with the same elements.</li>
		''' <li>If one value is an object array then the other must be too and
		''' <seealso cref="java.util.Arrays#deepEquals(Object[],Object[]) Arrays.deepEquals"/>
		''' must return true.</li>
		''' <li>Otherwise <seealso cref="Object#equals(Object)"/> must return true.</li>
		''' </ul>
		''' </summary>
		''' <param name="o"> the object to compare with.
		''' </param>
		''' <returns> {@code true} if the objects are the same; {@code false}
		''' otherwise.
		'''  </returns>
		' Note: this Javadoc is copied from javax.management.Descriptor
		'       due to 6369229.
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is javax.management.Descriptor) Then Return False
			If TypeOf o Is javax.management.ImmutableDescriptor Then Return o.Equals(Me)
			Return (New javax.management.ImmutableDescriptor(descriptorMap)).Equals(o)
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this descriptor.  The hash
		''' code is computed as the sum of the hash codes for each field in
		''' the descriptor.  The hash code of a field with name {@code n}
		''' and value {@code v} is {@code n.toLowerCase().hashCode() ^ h}.
		''' Here {@code h} is the hash code of {@code v}, computed as
		''' follows:</p>
		''' 
		''' <ul>
		''' <li>If {@code v} is null then {@code h} is 0.</li>
		''' <li>If {@code v} is a primitive array then {@code h} is computed using
		''' the appropriate overloading of {@code java.util.Arrays.hashCode}.</li>
		''' <li>If {@code v} is an object array then {@code h} is computed using
		''' <seealso cref="java.util.Arrays#deepHashCode(Object[]) Arrays.deepHashCode"/>.</li>
		''' <li>Otherwise {@code h} is {@code v.hashCode()}.</li>
		''' </ul>
		''' </summary>
		''' <returns> A hash code value for this object.
		'''  </returns>
		' Note: this Javadoc is copied from javax.management.Descriptor
		'       due to 6369229.
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function GetHashCode() As Integer
			Dim size As Integer = descriptorMap.size()
			' descriptorMap is sorted with a comparator that ignores cases.
			'
			Return com.sun.jmx.mbeanserver.Util.hashCode(descriptorMap.Keys.ToArray(New String(size - 1){}), descriptorMap.values().ToArray(New Object(size - 1){}))
		End Function

		''' <summary>
		''' Returns true if all of the fields have legal values given their
		''' names.
		''' <P>
		''' This implementation does not support  interoperating with a directory
		''' or lookup service. Thus, conforming to the specification, no checking is
		''' done on the <i>"export"</i> field.
		''' <P>
		''' Otherwise this implementation returns false if:
		''' <UL>
		''' <LI> name and descriptorType fieldNames are not defined, or
		''' null, or empty, or not String
		''' <LI> class, role, getMethod, setMethod fieldNames, if defined,
		''' are null or not String
		''' <LI> persistPeriod, currencyTimeLimit, lastUpdatedTimeStamp,
		''' lastReturnedTimeStamp if defined, are null, or not a Numeric
		''' String or not a Numeric Value {@literal >= -1}
		''' <LI> log fieldName, if defined, is null, or not a Boolean or
		''' not a String with value "t", "f", "true", "false". These String
		''' values must not be case sensitive.
		''' <LI> visibility fieldName, if defined, is null, or not a
		''' Numeric String or a not Numeric Value {@literal >= 1 and <= 4}
		''' <LI> severity fieldName, if defined, is null, or not a Numeric
		''' String or not a Numeric Value {@literal >= 0 and <= 6}<br>
		''' <LI> persistPolicy fieldName, if defined, is null, or not one of
		''' the following strings:<br>
		'''   "OnUpdate", "OnTimer", "NoMoreOftenThan", "OnUnregister", "Always",
		'''   "Never". These String values must not be case sensitive.<br>
		''' </UL>
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> If the validity checking
		''' fails for any reason, this exception will be thrown. </exception>

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property valid As Boolean
			Get
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "isValid()", "Entry")
				' verify that the descriptor is valid, by iterating over each field...
    
				Dim returnedSet As java.util.Set(Of KeyValuePair(Of String, Object)) = descriptorMap.entrySet()
    
				If returnedSet Is Nothing Then ' null descriptor, not valid
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "isValid()", "Returns false (null set)")
					Return False
				End If
				' must have a name and descriptor type field
				Dim thisName As String = CStr(Me.getFieldValue("name"))
				Dim thisDescType As String = CStr(getFieldValue("descriptorType"))
    
				If (thisName Is Nothing) OrElse (thisDescType Is Nothing) OrElse (thisName.Equals("")) OrElse (thisDescType.Equals("")) Then Return False
    
				' According to the descriptor type we validate the fields contained
    
				For Each currElement As KeyValuePair(Of String, Object) In returnedSet
					If currElement IsNot Nothing Then
						If currElement.Value IsNot Nothing Then
							' validate the field valued...
							If validateField((currElement.Key).ToString(), (currElement.Value).ToString()) Then
								Continue For
							Else
								If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "isValid()", "Field " & currElement.Key & "=" & currElement.Value & " is not valid")
								Return False
							End If
						End If
					End If
				Next currElement
    
				' fell through, all fields OK
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "isValid()", "Returns true")
				Return True
			End Get
		End Property


		' worker routine for isValid()
		' name is not null
		' descriptorType is not null
		' getMethod and setMethod are not null
		' persistPeriod is numeric
		' currencyTimeLimit is numeric
		' lastUpdatedTimeStamp is numeric
		' visibility is 1-4
		' severity is 0-6
		' log is T or F
		' role is not null
		' class is not null
		' lastReturnedTimeStamp is numeric


		Private Function validateField(ByVal fldName As String, ByVal fldValue As Object) As Boolean
			If (fldName Is Nothing) OrElse (fldName.Equals("")) Then Return False
			Dim SfldValue As String = ""
			Dim isAString As Boolean = False
			If (fldValue IsNot Nothing) AndAlso (TypeOf fldValue Is String) Then
				SfldValue = CStr(fldValue)
				isAString = True
			End If

			Dim nameOrDescriptorType As Boolean = (fldName.ToUpper() = "Name".ToUpper() OrElse fldName.ToUpper() = "DescriptorType".ToUpper())
			If nameOrDescriptorType OrElse fldName.ToUpper() = "SetMethod".ToUpper() OrElse fldName.ToUpper() = "GetMethod".ToUpper() OrElse fldName.ToUpper() = "Role".ToUpper() OrElse fldName.ToUpper() = "Class".ToUpper() Then
				If fldValue Is Nothing OrElse (Not isAString) Then Return False
				If nameOrDescriptorType AndAlso SfldValue.Equals("") Then Return False
				Return True
			ElseIf fldName.ToUpper() = "visibility".ToUpper() Then
				Dim v As Long
				If (fldValue IsNot Nothing) AndAlso (isAString) Then
					v = toNumeric(SfldValue)
				ElseIf TypeOf fldValue Is Integer? Then
					v = CInt(Fix(fldValue))
				Else
					Return False
				End If

				If v >= 1 AndAlso v <= 4 Then
					Return True
				Else
					Return False
				End If
			ElseIf fldName.ToUpper() = "severity".ToUpper() Then

				Dim v As Long
				If (fldValue IsNot Nothing) AndAlso (isAString) Then
					v = toNumeric(SfldValue)
				ElseIf TypeOf fldValue Is Integer? Then
					v = CInt(Fix(fldValue))
				Else
					Return False
				End If

				Return (v >= 0 AndAlso v <= 6)
			ElseIf fldName.ToUpper() = "PersistPolicy".ToUpper() Then
				Return (((fldValue IsNot Nothing) AndAlso (isAString)) AndAlso (SfldValue.ToUpper() = "OnUpdate".ToUpper() OrElse SfldValue.ToUpper() = "OnTimer".ToUpper() OrElse SfldValue.ToUpper() = "NoMoreOftenThan".ToUpper() OrElse SfldValue.ToUpper() = "Always".ToUpper() OrElse SfldValue.ToUpper() = "Never".ToUpper() OrElse SfldValue.ToUpper() = "OnUnregister".ToUpper()))
			ElseIf fldName.ToUpper() = "PersistPeriod".ToUpper() OrElse fldName.ToUpper() = "CurrencyTimeLimit".ToUpper() OrElse fldName.ToUpper() = "LastUpdatedTimeStamp".ToUpper() OrElse fldName.ToUpper() = "LastReturnedTimeStamp".ToUpper() Then

				Dim v As Long
				If (fldValue IsNot Nothing) AndAlso (isAString) Then
					v = toNumeric(SfldValue)
				ElseIf TypeOf fldValue Is java.lang.Number Then
					v = CType(fldValue, Number)
				Else
					Return False
				End If

				Return (v >= -1)
			ElseIf fldName.ToUpper() = "log".ToUpper() Then
				Return ((TypeOf fldValue Is Boolean?) OrElse (isAString AndAlso (SfldValue.ToUpper() = "T".ToUpper() OrElse SfldValue.ToUpper() = "true".ToUpper() OrElse SfldValue.ToUpper() = "F".ToUpper() OrElse SfldValue.ToUpper() = "false".ToUpper())))
			End If

			' default to true, it is a field we aren't validating (user etc.)
			Return True
		End Function



		''' <summary>
		''' <p>Returns an XML String representing the descriptor.</p>
		''' 
		''' <p>The format is not defined, but an implementation must
		''' ensure that the string returned by this method can be
		''' used to build an equivalent descriptor when instantiated
		''' using the constructor {@link #DescriptorSupport(String)
		''' DescriptorSupport(String inStr)}.</p>
		''' 
		''' <p>Fields which are not String objects will have toString()
		''' called on them to create the value. The value will be
		''' enclosed in parentheses.  It is not guaranteed that you can
		''' reconstruct these objects unless they have been
		''' specifically set up to support toString() in a meaningful
		''' format and have a matching constructor that accepts a
		''' String in the same format.</p>
		''' 
		''' <p>If the descriptor is empty the following String is
		''' returned: &lt;Descriptor&gt;&lt;/Descriptor&gt;</p>
		''' </summary>
		''' <returns> the XML string.
		''' </returns>
		''' <exception cref="RuntimeOperationsException"> for illegal value for
		''' field Names or field Values.  If the XML formatted string
		''' construction fails for any reason, this exception will be
		''' thrown. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function toXMLString() As String
			Dim buf As New StringBuilder("<Descriptor>")
			Dim returnedSet As java.util.Set(Of KeyValuePair(Of String, Object)) = descriptorMap.entrySet()
			For Each currElement As KeyValuePair(Of String, Object) In returnedSet
				Dim name As String = currElement.Key
				Dim value As Object = currElement.Value
				Dim valueString As String = Nothing
	'             Set valueString to non-null if and only if this is a string that
	'               cannot be confused with the encoding of an object.  If it
	'               could be so confused (surrounded by parentheses) then we
	'               call makeFieldValue as for any non-String object and end
	'               up with an encoding like "(java.lang.String/(thing))".  
				If TypeOf value Is String Then
					Dim svalue As String = CStr(value)
					If (Not svalue.StartsWith("(")) OrElse (Not svalue.EndsWith(")")) Then valueString = quote(svalue)
				End If
				If valueString Is Nothing Then valueString = makeFieldValue(value)
				buf.Append("<field name=""").append(name).append(""" value=""").append(valueString).append("""></field>")
			Next currElement
			buf.Append("</Descriptor>")
			Return buf.ToString()
		End Function

		Private Shared ReadOnly entities As String() = { " &#32;", """&quot;", "<&lt;", ">&gt;", "&&amp;", vbCr & "&#13;", vbTab & "&#9;", vbLf & "&#10;", vbFormFeed & "&#12;" }
		Private Shared ReadOnly entityToCharMap As IDictionary(Of String, Char?) = New Dictionary(Of String, Char?)
		Private Shared ReadOnly charToEntityMap As String()


		Private Shared Function isMagic(ByVal c As Char) As Boolean
			Return (AscW(c) < charToEntityMap.Length AndAlso charToEntityMap(AscW(c)) IsNot Nothing)
		End Function

	'    
	'     * Quote the string so that it will be acceptable to the (String)
	'     * constructor.  Since the parsing code in that constructor is fairly
	'     * stupid, we're obliged to quote apparently innocuous characters like
	'     * space, <, and >.  In a future version, we should rewrite the parser
	'     * and only quote " plus either \ or & (depending on the quote syntax).
	'     
		Private Shared Function quote(ByVal s As String) As String
			Dim found As Boolean = False
			For i As Integer = 0 To s.Length - 1
				If isMagic(s.Chars(i)) Then
					found = True
					Exit For
				End If
			Next i
			If Not found Then Return s
			Dim buf As New StringBuilder
			For i As Integer = 0 To s.Length - 1
				Dim c As Char = s.Chars(i)
				If isMagic(c) Then
					buf.Append(charToEntityMap(AscW(c)))
				Else
					buf.Append(c)
				End If
			Next i
			Return buf.ToString()
		End Function

		Private Shared Function unquote(ByVal s As String) As String
			If (Not s.StartsWith("""")) OrElse (Not s.EndsWith("""")) Then Throw New XMLParseException("Value must be quoted: <" & s & ">")
			Dim buf As New StringBuilder
			Dim len As Integer = s.Length - 1
			For i As Integer = 1 To len - 1
				Dim c As Char = s.Chars(i)
				Dim semi As Integer
				Dim quoted As Char?
				semi = s.IndexOf(";"c, i + 1)
				quoted = entityToCharMap(s.Substring(i, semi+1 - i))
				If c = AscW("&"c) AndAlso semi >= 0 AndAlso (quoted IsNot Nothing) Then
					buf.Append(quoted)
					i = semi
				Else
					buf.Append(c)
				End If
			Next i
			Return buf.ToString()
		End Function

		''' <summary>
		''' Make the string that will go inside "..." for a value that is not
		''' a plain String. </summary>
		''' <exception cref="RuntimeOperationsException"> if the value cannot be encoded. </exception>
		Private Shared Function makeFieldValue(ByVal value As Object) As String
			If value Is Nothing Then Return "(null)"

			Dim valueClass As Type = value.GetType()
			Try
				valueClass.GetConstructor(GetType(String))
			Catch e As NoSuchMethodException
				Dim msg As String = "Class " & valueClass & " does not have a public " & "constructor with a single string arg"
				Dim iae As Exception = New System.ArgumentException(msg)
				Throw New javax.management.RuntimeOperationsException(iae, "Cannot make XML descriptor")
			Catch e As SecurityException
				' OK: we'll pretend the constructor is there
				' too bad if it's not: we'll find out when we try to
				' reconstruct the DescriptorSupport
			End Try

			Dim quotedValueString As String = quote(value.ToString())

			Return "(" & valueClass.name & "/" & quotedValueString & ")"
		End Function

	'    
	'     * Parse a field value from the XML produced by toXMLString().
	'     * Given a descriptor XML containing <field name="nnn" value="vvv">,
	'     * the argument to this method will be "vvv" (a string including the
	'     * containing quote characters).  If vvv begins and ends with parentheses,
	'     * then it may contain:
	'     * - the characters "null", in which case the result is null;
	'     * - a value of the form "some.class.name/xxx", in which case the
	'     * result is equivalent to `new some.class.name("xxx")';
	'     * - some other string, in which case the result is that string,
	'     * without the parentheses.
	'     
		Private Shared Function parseQuotedFieldValue(ByVal s As String) As Object
			s = unquote(s)
			If s.ToUpper() = "(null)".ToUpper() Then Return Nothing
			If (Not s.StartsWith("(")) OrElse (Not s.EndsWith(")")) Then Return s
			Dim slash As Integer = s.IndexOf("/"c)
			If slash < 0 Then Return s.Substring(1, s.Length - 1 - 1)
			Dim className As String = s.Substring(1, slash - 1)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim constr As Constructor(Of ?)
			Try
				sun.reflect.misc.ReflectUtil.checkPackageAccess(className)
				Dim contextClassLoader As ClassLoader = Thread.CurrentThread.contextClassLoader
				Dim c As Type = Type.GetType(className, False, contextClassLoader)
				constr = c.GetConstructor(New Type() {GetType(String)})
			Catch e As Exception
				Throw New XMLParseException(e, "Cannot parse value: <" & s & ">")
			End Try
			Dim arg As String = s.Substring(slash + 1, s.Length - 1 - (slash + 1))
			Try
				Return constr.newInstance(New Object() {arg})
			Catch e As Exception
				Dim msg As String = "Cannot construct instance of " & className & " with arg: <" & s & ">"
				Throw New XMLParseException(e, msg)
			End Try
		End Function

		''' <summary>
		''' Returns a human readable string representing the
		''' descriptor.  The string will be in the format of
		''' "fieldName=fieldValue,fieldName2=fieldValue2,..."<br>
		''' 
		''' If there are no fields in the descriptor, then an empty String
		''' is returned.<br>
		''' 
		''' If a fieldValue is an object then the toString() method is
		''' called on it and its returned value is used as the value for
		''' the field enclosed in parenthesis.
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> for illegal value for
		''' field Names or field Values.  If the descriptor string fails
		''' for any reason, this exception will be thrown. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "toString()", "Entry")

			Dim respStr As String = ""
			Dim ___fields As String() = fields

			If (___fields Is Nothing) OrElse (___fields.Length = 0) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "toString()", "Empty Descriptor")
				Return respStr
			End If

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "toString()", "Printing " & ___fields.Length & " fields")

			For i As Integer = 0 To ___fields.Length - 1
				If i = (___fields.Length - 1) Then
					respStr = respStr + ___fields(i)
				Else
					respStr = respStr + ___fields(i) & ", "
				End If
			Next i

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINEST, GetType(DescriptorSupport).name, "toString()", "Exit returning " & respStr)

			Return respStr
		End Function

		' utility to convert to int, returns -2 if bogus.

		Private Function toNumeric(ByVal inStr As String) As Long
			Try
				Return Convert.ToInt64(inStr)
			Catch e As Exception
				Return -2
			End Try
		End Function


		''' <summary>
		''' Deserializes a <seealso cref="DescriptorSupport"/> from an {@link
		''' ObjectInputStream}.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			Dim ___fields As java.io.ObjectInputStream.GetField = [in].readFields()
			Dim descriptor As IDictionary(Of String, Object) = cast(___fields.get("descriptor", Nothing))
			init(Nothing)
			If descriptor IsNot Nothing Then descriptorMap.putAll(descriptor)
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="DescriptorSupport"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
	'     If you set jmx.serial.form to "1.2.0" or "1.2.1", then we are
	'       bug-compatible with those versions.  Specifically, field names
	'       are forced to lower-case before being written.  This
	'       contradicts the spec, which, though it does not mention
	'       serialization explicitly, does say that the case of field names
	'       is preserved.  But in 1.2.0 and 1.2.1, this requirement was not
	'       met.  Instead, field names in the descriptor map were forced to
	'       lower case.  Those versions expect this to have happened to a
	'       descriptor they deserialize and e.g. getFieldValue will not
	'       find a field whose name is spelt with a different case.
	'    
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			Dim ___fields As java.io.ObjectOutputStream.PutField = out.putFields()
			Dim compat As Boolean = "1.0".Equals(serialForm)
			If compat Then ___fields.put("currClass", currClass)

	'         Purge the field "targetObject" from the DescriptorSupport before
	'         * serializing since the referenced object is typically not
	'         * serializable.  We do this here rather than purging the "descriptor"
	'         * variable below because that HashMap doesn't do case-insensitivity.
	'         * See CR 6332962.
	'         
			Dim startMap As java.util.SortedMap(Of String, Object) = descriptorMap
			If startMap.containsKey("targetObject") Then
				startMap = New SortedDictionary(Of String, Object)(descriptorMap)
				startMap.remove("targetObject")
			End If

			Dim descriptor As Dictionary(Of String, Object)
			If compat OrElse "1.2.0".Equals(serialForm) OrElse "1.2.1".Equals(serialForm) Then
				descriptor = New Dictionary(Of String, Object)
				For Each entry As KeyValuePair(Of String, Object) In startMap.entrySet()
					descriptor(entry.Key.ToLower()) = entry.Value
				Next entry
			Else
				descriptor = New Dictionary(Of String, Object)(startMap)
			End If

			___fields.put("descriptor", descriptor)
			out.writeFields()
		End Sub

	End Class

End Namespace
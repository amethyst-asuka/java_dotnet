Imports System.Collections.Generic
Imports javax.xml.parsers
Imports javax.xml.transform
Imports javax.xml.transform.dom
Imports javax.xml.transform.stream
Imports org.xml.sax
Imports org.w3c.dom

'
' * Copyright (c) 2002, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.prefs

	''' <summary>
	''' XML Support for java.util.prefs. Methods to import and export preference
	''' nodes and subtrees.
	''' 
	''' @author  Josh Bloch and Mark Reinhold </summary>
	''' <seealso cref=     Preferences
	''' @since   1.4 </seealso>
	Friend Class XmlSupport
		' The required DTD URI for exported preferences
		Private Const PREFS_DTD_URI As String = "http://java.sun.com/dtd/preferences.dtd"

		' The actual DTD corresponding to the URI
		Private Shared ReadOnly PREFS_DTD As String = "<?xml version=""1.0"" encoding=""UTF-8""?>" & "<!-- DTD for preferences -->" & "<!ELEMENT preferences (root) >" & "<!ATTLIST preferences" & " EXTERNAL_XML_VERSION CDATA ""0.0""  >" & "<!ELEMENT root (map, node*) >" & "<!ATTLIST root" & "          type (system|user) #REQUIRED >" & "<!ELEMENT node (map, node*) >" & "<!ATTLIST node" & "          name CDATA #REQUIRED >" & "<!ELEMENT map (entry*) >" & "<!ATTLIST map" & "  MAP_XML_VERSION CDATA ""0.0""  >" & "<!ELEMENT entry EMPTY >" & "<!ATTLIST entry" & "          key CDATA #REQUIRED" & "          value CDATA #REQUIRED >"
		''' <summary>
		''' Version number for the format exported preferences files.
		''' </summary>
		Private Const EXTERNAL_XML_VERSION As String = "1.0"

	'    
	'     * Version number for the internal map files.
	'     
		Private Const MAP_XML_VERSION As String = "1.0"

		''' <summary>
		''' Export the specified preferences node and, if subTree is true, all
		''' subnodes, to the specified output stream.  Preferences are exported as
		''' an XML document conforming to the definition in the Preferences spec.
		''' </summary>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="BackingStoreException"> if preference data cannot be read from
		'''         backing store. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="Preferences#removeNode()"/> method. </exception>
		Friend Shared Sub export(  os As OutputStream,   p As Preferences,   subTree As Boolean)
			If CType(p, AbstractPreferences).removed Then Throw New IllegalStateException("Node has been removed")
			Dim doc As Document = createPrefsDoc("preferences")
			Dim preferences_Renamed As Element = doc.documentElement
			preferences_Renamed.attributeute("EXTERNAL_XML_VERSION", EXTERNAL_XML_VERSION)
			Dim xmlRoot As Element = CType(preferences_Renamed.appendChild(doc.createElement("root")), Element)
			xmlRoot.attributeute("type", (If(p.userNode, "user", "system")))

			' Get bottom-up list of nodes from p to root, excluding root
			Dim ancestors As List(Of Preferences) = New List(Of Preferences)

			Dim kid As Preferences = p
			Dim dad As Preferences = kid.parent()
			Do While dad IsNot Nothing
				ancestors.add(kid)
				kid = dad
				dad = kid.parent()
			Loop
			Dim e As Element = xmlRoot
			For i As Integer = ancestors.size()-1 To 0 Step -1
				e.appendChild(doc.createElement("map"))
				e = CType(e.appendChild(doc.createElement("node")), Element)
				e.attributeute("name", ancestors.get(i).name())
			Next i
			putPreferencesInXml(e, doc, p, subTree)

			writeDoc(doc, os)
		End Sub

		''' <summary>
		''' Put the preferences in the specified Preferences node into the
		''' specified XML element which is assumed to represent a node
		''' in the specified XML document which is assumed to conform to
		''' PREFS_DTD.  If subTree is true, create children of the specified
		''' XML node conforming to all of the children of the specified
		''' Preferences node and recurse.
		''' </summary>
		''' <exception cref="BackingStoreException"> if it is not possible to read
		'''         the preferences or children out of the specified
		'''         preferences node. </exception>
		Private Shared Sub putPreferencesInXml(  elt As Element,   doc As Document,   prefs As Preferences,   subTree As Boolean)
			Dim kidsCopy As Preferences() = Nothing
			Dim kidNames As String() = Nothing

			' Node is locked to export its contents and get a
			' copy of children, then lock is released,
			' and, if subTree = true, recursive calls are made on children
			SyncLock CType(prefs, AbstractPreferences).lock
				' Check if this node was concurrently removed. If yes
				' remove it from XML Document and return.
				If CType(prefs, AbstractPreferences).removed Then
					elt.parentNode.removeChild(elt)
					Return
				End If
				' Put map in xml element
				Dim keys As String() = prefs.keys()
				Dim map As Element = CType(elt.appendChild(doc.createElement("map")), Element)
				For i As Integer = 0 To keys.Length - 1
					Dim entry As Element = CType(map.appendChild(doc.createElement("entry")), Element)
					entry.attributeute("key", keys(i))
					' NEXT STATEMENT THROWS NULL PTR EXC INSTEAD OF ASSERT FAIL
					entry.attributeute("value", prefs.get(keys(i), Nothing))
				Next i
				' Recurse if appropriate
				If subTree Then
					' Get a copy of kids while lock is held 
					kidNames = prefs.childrenNames()
					kidsCopy = New Preferences(kidNames.Length - 1){}
					For i As Integer = 0 To kidNames.Length - 1
						kidsCopy(i) = prefs.node(kidNames(i))
					Next i
				End If
				' release lock
			End SyncLock

			If subTree Then
				For i As Integer = 0 To kidNames.Length - 1
					Dim xmlKid As Element = CType(elt.appendChild(doc.createElement("node")), Element)
					xmlKid.attributeute("name", kidNames(i))
					putPreferencesInXml(xmlKid, doc, kidsCopy(i), subTree)
				Next i
			End If
		End Sub

		''' <summary>
		''' Import preferences from the specified input stream, which is assumed
		''' to contain an XML document in the format described in the Preferences
		''' spec.
		''' </summary>
		''' <exception cref="IOException"> if reading from the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="InvalidPreferencesFormatException"> Data on input stream does not
		'''         constitute a valid XML document with the mandated document type. </exception>
		Friend Shared Sub importPreferences(  [is] As InputStream)
			Try
				Dim doc As Document = loadPrefsDoc([is])
				Dim xmlVersion As String = doc.documentElement.getAttribute("EXTERNAL_XML_VERSION")
				If xmlVersion.CompareTo(EXTERNAL_XML_VERSION) > 0 Then Throw New InvalidPreferencesFormatException("Exported preferences file format version " & xmlVersion & " is not supported. This java installation can read" & " versions " & EXTERNAL_XML_VERSION & " or older. You may need" & " to install a newer version of JDK.")

				Dim xmlRoot As Element = CType(doc.documentElement.childNodes.item(0), Element)
				Dim prefsRoot As Preferences = (If(xmlRoot.getAttribute("type").Equals("user"), Preferences.userRoot(), Preferences.systemRoot()))
				ImportSubtree(prefsRoot, xmlRoot)
			Catch e As SAXException
				Throw New InvalidPreferencesFormatException(e)
			End Try
		End Sub

		''' <summary>
		''' Create a new prefs XML document.
		''' </summary>
		Private Shared Function createPrefsDoc(  qname As String) As Document
			Try
				Dim di As DOMImplementation = DocumentBuilderFactory.newInstance().newDocumentBuilder().dOMImplementation
				Dim dt As DocumentType = di.createDocumentType(qname, Nothing, PREFS_DTD_URI)
				Return di.createDocument(Nothing, qname, dt)
			Catch e As ParserConfigurationException
				Throw New AssertionError(e)
			End Try
		End Function

		''' <summary>
		''' Load an XML document from specified input stream, which must
		''' have the requisite DTD URI.
		''' </summary>
		Private Shared Function loadPrefsDoc(  [in] As InputStream) As Document
			Dim dbf As DocumentBuilderFactory = DocumentBuilderFactory.newInstance()
			dbf.ignoringElementContentWhitespace = True
			dbf.validating = True
			dbf.coalescing = True
			dbf.ignoringComments = True
			Try
				Dim db As DocumentBuilder = dbf.newDocumentBuilder()
				db.entityResolver = New Resolver
				db.errorHandler = New EH
				Return db.parse(New InputSource([in]))
			Catch e As ParserConfigurationException
				Throw New AssertionError(e)
			End Try
		End Function

		''' <summary>
		''' Write XML document to the specified output stream.
		''' </summary>
		Private Shared Sub writeDoc(  doc As Document,   out As OutputStream)
			Try
				Dim tf As TransformerFactory = TransformerFactory.newInstance()
				Try
					tf.attributeute("indent-number", New Integer?(2))
				Catch iae As IllegalArgumentException
					'Ignore the IAE. Should not fail the writeout even the
					'transformer provider does not support "indent-number".
				End Try
				Dim t As Transformer = tf.newTransformer()
				t.outputPropertyrty(OutputKeys.DOCTYPE_SYSTEM, doc.doctype.systemId)
				t.outputPropertyrty(OutputKeys.INDENT, "yes")
				'Transformer resets the "indent" info if the "result" is a StreamResult with
				'an OutputStream object embedded, creating a Writer object on top of that
				'OutputStream object however works.
				t.transform(New DOMSource(doc), New StreamResult(New BufferedWriter(New OutputStreamWriter(out, "UTF-8"))))
			Catch e As TransformerException
				Throw New AssertionError(e)
			End Try
		End Sub

		''' <summary>
		''' Recursively traverse the specified preferences node and store
		''' the described preferences into the system or current user
		''' preferences tree, as appropriate.
		''' </summary>
		Private Shared Sub ImportSubtree(  prefsNode As Preferences,   xmlNode As Element)
			Dim xmlKids As NodeList = xmlNode.childNodes
			Dim numXmlKids As Integer = xmlKids.length
	'        
	'         * We first lock the node, import its contents and get
	'         * child nodes. Then we unlock the node and go to children
	'         * Since some of the children might have been concurrently
	'         * deleted we check for this.
	'         
			Dim prefsKids As Preferences()
			' Lock the node 
			SyncLock CType(prefsNode, AbstractPreferences).lock
				'If removed, return silently
				If CType(prefsNode, AbstractPreferences).removed Then Return

				' Import any preferences at this node
				Dim firstXmlKid As Element = CType(xmlKids.item(0), Element)
				ImportPrefs(prefsNode, firstXmlKid)
				prefsKids = New Preferences(numXmlKids - 2){}

				' Get involved children
				For i As Integer = 1 To numXmlKids - 1
					Dim xmlKid As Element = CType(xmlKids.item(i), Element)
					prefsKids(i-1) = prefsNode.node(xmlKid.getAttribute("name"))
				Next i
			End SyncLock ' unlocked the node
			' import children
			For i As Integer = 1 To numXmlKids - 1
				ImportSubtree(prefsKids(i-1), CType(xmlKids.item(i), Element))
			Next i
		End Sub

		''' <summary>
		''' Import the preferences described by the specified XML element
		''' (a map from a preferences document) into the specified
		''' preferences node.
		''' </summary>
		Private Shared Sub ImportPrefs(  prefsNode As Preferences,   map As Element)
			Dim entries As NodeList = map.childNodes
			Dim i As Integer=0
			Dim numEntries As Integer = entries.length
			Do While i < numEntries
				Dim entry As Element = CType(entries.item(i), Element)
				prefsNode.put(entry.getAttribute("key"), entry.getAttribute("value"))
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Export the specified Map<String,String> to a map document on
		''' the specified OutputStream as per the prefs DTD.  This is used
		''' as the internal (undocumented) format for FileSystemPrefs.
		''' </summary>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		Friend Shared Sub exportMap(  os As OutputStream,   map As Map(Of String, String))
			Dim doc As Document = createPrefsDoc("map")
			Dim xmlMap As Element = doc.documentElement
			xmlMap.attributeute("MAP_XML_VERSION", MAP_XML_VERSION)

			Dim i As [Iterator](Of KeyValuePair(Of String, String)) = map.entrySet().GetEnumerator()
			Do While i.MoveNext()
				Dim e As KeyValuePair(Of String, String) = i.Current
				Dim xe As Element = CType(xmlMap.appendChild(doc.createElement("entry")), Element)
				xe.attributeute("key", e.Key)
				xe.attributeute("value", e.Value)
			Loop

			writeDoc(doc, os)
		End Sub

		''' <summary>
		''' Import Map from the specified input stream, which is assumed
		''' to contain a map document as per the prefs DTD.  This is used
		''' as the internal (undocumented) format for FileSystemPrefs.  The
		''' key-value pairs specified in the XML document will be put into
		''' the specified Map.  (If this Map is empty, it will contain exactly
		''' the key-value pairs int the XML-document when this method returns.)
		''' </summary>
		''' <exception cref="IOException"> if reading from the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="InvalidPreferencesFormatException"> Data on input stream does not
		'''         constitute a valid XML document with the mandated document type. </exception>
		Friend Shared Sub importMap(  [is] As InputStream,   m As Map(Of String, String))
			Try
				Dim doc As Document = loadPrefsDoc([is])
				Dim xmlMap As Element = doc.documentElement
				' check version
				Dim mapVersion As String = xmlMap.getAttribute("MAP_XML_VERSION")
				If mapVersion.CompareTo(MAP_XML_VERSION) > 0 Then Throw New InvalidPreferencesFormatException("Preferences map file format version " & mapVersion & " is not supported. This java installation can read" & " versions " & MAP_XML_VERSION & " or older. You may need" & " to install a newer version of JDK.")

				Dim entries As NodeList = xmlMap.childNodes
				Dim i As Integer=0
				Dim numEntries As Integer=entries.length
				Do While i<numEntries
					Dim entry As Element = CType(entries.item(i), Element)
					m.put(entry.getAttribute("key"), entry.getAttribute("value"))
					i += 1
				Loop
			Catch e As SAXException
				Throw New InvalidPreferencesFormatException(e)
			End Try
		End Sub

		Private Class Resolver
			Implements EntityResolver

			Public Overridable Function resolveEntity(  pid As String,   sid As String) As InputSource
				If sid.Equals(PREFS_DTD_URI) Then
					Dim [is] As InputSource
					[is] = New InputSource(New StringReader(PREFS_DTD))
					[is].systemId = PREFS_DTD_URI
					Return [is]
				End If
				Throw New SAXException("Invalid system identifier: " & sid)
			End Function
		End Class

		Private Class EH
			Implements ErrorHandler

			Public Overridable Sub [error](  x As SAXParseException)
				Throw x
			End Sub
			Public Overridable Sub fatalError(  x As SAXParseException)
				Throw x
			End Sub
			Public Overridable Sub warning(  x As SAXParseException)
				Throw x
			End Sub
		End Class
	End Class

End Namespace
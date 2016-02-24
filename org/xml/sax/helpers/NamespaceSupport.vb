Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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

' NamespaceSupport.java - generic Namespace support for SAX.
' http://www.saxproject.org
' Written by David Megginson
' This class is in the Public Domain.  NO WARRANTY!
' $Id: NamespaceSupport.java,v 1.5 2004/11/03 22:53:09 jsuttor Exp $

Namespace org.xml.sax.helpers



	''' <summary>
	''' Encapsulate Namespace logic for use by applications using SAX,
	''' or internally by SAX drivers.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class encapsulates the logic of Namespace processing: it
	''' tracks the declarations currently in force for each context and
	''' automatically processes qualified XML names into their Namespace
	''' parts; it can also be used in reverse for generating XML qnames
	''' from Namespaces.</p>
	''' 
	''' <p>Namespace support objects are reusable, but the reset method
	''' must be invoked between each session.</p>
	''' 
	''' <p>Here is a simple session:</p>
	''' 
	''' <pre>
	''' String parts[] = new String[3];
	''' NamespaceSupport support = new NamespaceSupport();
	''' 
	''' support.pushContext();
	''' support.declarePrefix("", "http://www.w3.org/1999/xhtml");
	''' support.declarePrefix("dc", "http://www.purl.org/dc#");
	''' 
	''' parts = support.processName("p", parts, false);
	''' System.out.println("Namespace URI: " + parts[0]);
	''' System.out.println("Local name: " + parts[1]);
	''' System.out.println("Raw name: " + parts[2]);
	''' 
	''' parts = support.processName("dc:title", parts, false);
	''' System.out.println("Namespace URI: " + parts[0]);
	''' System.out.println("Local name: " + parts[1]);
	''' System.out.println("Raw name: " + parts[2]);
	''' 
	''' support.popContext();
	''' </pre>
	''' 
	''' <p>Note that this class is optimized for the use case where most
	''' elements do not contain Namespace declarations: if the same
	''' prefix/URI mapping is repeated for each context (for example), this
	''' class will be somewhat less efficient.</p>
	''' 
	''' <p>Although SAX drivers (parsers) may choose to use this class to
	''' implement namespace handling, they are not required to do so.
	''' Applications must track namespace information themselves if they
	''' want to use namespace information.
	''' 
	''' @since SAX 2.0
	''' @author David Megginson
	''' </summary>
	Public Class NamespaceSupport


		'//////////////////////////////////////////////////////////////////
		' Constants.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' The XML Namespace URI as a constant.
		''' The value is <code>http://www.w3.org/XML/1998/namespace</code>
		''' as defined in the "Namespaces in XML" * recommendation.
		''' 
		''' <p>This is the Namespace URI that is automatically mapped
		''' to the "xml" prefix.</p>
		''' </summary>
		Public Const XMLNS As String = "http://www.w3.org/XML/1998/namespace"


		''' <summary>
		''' The namespace declaration URI as a constant.
		''' The value is <code>http://www.w3.org/xmlns/2000/</code>, as defined
		''' in a backwards-incompatible erratum to the "Namespaces in XML"
		''' recommendation.  Because that erratum postdated SAX2, SAX2 defaults
		''' to the original recommendation, and does not normally use this URI.
		''' 
		''' 
		''' <p>This is the Namespace URI that is optionally applied to
		''' <em>xmlns</em> and <em>xmlns:*</em> attributes, which are used to
		''' declare namespaces.  </p>
		''' 
		''' @since SAX 2.1alpha </summary>
		''' <seealso cref= #setNamespaceDeclUris </seealso>
		''' <seealso cref= #isNamespaceDeclUris </seealso>
		Public Const NSDECL As String = "http://www.w3.org/xmlns/2000/"


		''' <summary>
		''' An empty enumeration.
		''' </summary>
		Private Shared ReadOnly EMPTY_ENUMERATION As System.Collections.IEnumerator = java.util.Collections.enumeration(New List(Of String))


		'//////////////////////////////////////////////////////////////////
		' Constructor.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Create a new Namespace support object.
		''' </summary>
		Public Sub New()
			reset()
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Context management.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Reset this Namespace support object for reuse.
		''' 
		''' <p>It is necessary to invoke this method before reusing the
		''' Namespace support object for a new session.  If namespace
		''' declaration URIs are to be supported, that flag must also
		''' be set to a non-default value.
		''' </p>
		''' </summary>
		''' <seealso cref= #setNamespaceDeclUris </seealso>
		Public Overridable Sub reset()
			contexts = New Context(31){}
			namespaceDeclUris = False
			contextPos = 0
				currentContext = New Context(Me, Me)
				contexts(contextPos) = currentContext
			currentContext.declarePrefix("xml", XMLNS)
		End Sub


		''' <summary>
		''' Start a new Namespace context.
		''' The new context will automatically inherit
		''' the declarations of its parent context, but it will also keep
		''' track of which declarations were made within this context.
		''' 
		''' <p>Event callback code should start a new context once per element.
		''' This means being ready to call this in either of two places.
		''' For elements that don't include namespace declarations, the
		''' <em>ContentHandler.startElement()</em> callback is the right place.
		''' For elements with such a declaration, it'd done in the first
		''' <em>ContentHandler.startPrefixMapping()</em> callback.
		''' A boolean flag can be used to
		''' track whether a context has been started yet.  When either of
		''' those methods is called, it checks the flag to see if a new context
		''' needs to be started.  If so, it starts the context and sets the
		''' flag.  After <em>ContentHandler.startElement()</em>
		''' does that, it always clears the flag.
		''' 
		''' <p>Normally, SAX drivers would push a new context at the beginning
		''' of each XML element.  Then they perform a first pass over the
		''' attributes to process all namespace declarations, making
		''' <em>ContentHandler.startPrefixMapping()</em> callbacks.
		''' Then a second pass is made, to determine the namespace-qualified
		''' names for all attributes and for the element name.
		''' Finally all the information for the
		''' <em>ContentHandler.startElement()</em> callback is available,
		''' so it can then be made.
		''' 
		''' <p>The Namespace support object always starts with a base context
		''' already in force: in this context, only the "xml" prefix is
		''' declared.</p>
		''' </summary>
		''' <seealso cref= org.xml.sax.ContentHandler </seealso>
		''' <seealso cref= #popContext </seealso>
		Public Overridable Sub pushContext()
			Dim max As Integer = contexts.Length

			contextPos += 1

									' Extend the array if necessary
			If contextPos >= max Then
				Dim newContexts As Context() = New Context(max*2 - 1){}
				Array.Copy(contexts, 0, newContexts, 0, max)
				max *= 2
				contexts = newContexts
			End If

									' Allocate the context if necessary.
			currentContext = contexts(contextPos)
			If currentContext Is Nothing Then
					currentContext = New Context(Me, Me)
					contexts(contextPos) = currentContext
			End If

									' Set the parent, if any.
			If contextPos > 0 Then currentContext.parent = contexts(contextPos - 1)
		End Sub


		''' <summary>
		''' Revert to the previous Namespace context.
		''' 
		''' <p>Normally, you should pop the context at the end of each
		''' XML element.  After popping the context, all Namespace prefix
		''' mappings that were previously in force are restored.</p>
		''' 
		''' <p>You must not attempt to declare additional Namespace
		''' prefixes after popping a context, unless you push another
		''' context first.</p>
		''' </summary>
		''' <seealso cref= #pushContext </seealso>
		Public Overridable Sub popContext()
			contexts(contextPos).clear()
			contextPos -= 1
			If contextPos < 0 Then Throw New java.util.EmptyStackException
			currentContext = contexts(contextPos)
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Operations within a context.
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Declare a Namespace prefix.  All prefixes must be declared
		''' before they are referenced.  For example, a SAX driver (parser)
		''' would scan an element's attributes
		''' in two passes:  first for namespace declarations,
		''' then a second pass using <seealso cref="#processName processName()"/> to
		''' interpret prefixes against (potentially redefined) prefixes.
		''' 
		''' <p>This method declares a prefix in the current Namespace
		''' context; the prefix will remain in force until this context
		''' is popped, unless it is shadowed in a descendant context.</p>
		''' 
		''' <p>To declare the default element Namespace, use the empty string as
		''' the prefix.</p>
		''' 
		''' <p>Note that there is an asymmetry in this library: {@link
		''' #getPrefix getPrefix} will not return the "" prefix,
		''' even if you have declared a default element namespace.
		''' To check for a default namespace,
		''' you have to look it up explicitly using <seealso cref="#getURI getURI"/>.
		''' This asymmetry exists to make it easier to look up prefixes
		''' for attribute names, where the default prefix is not allowed.</p>
		''' </summary>
		''' <param name="prefix"> The prefix to declare, or the empty string to
		'''  indicate the default element namespace.  This may never have
		'''  the value "xml" or "xmlns". </param>
		''' <param name="uri"> The Namespace URI to associate with the prefix. </param>
		''' <returns> true if the prefix was legal, false otherwise
		''' </returns>
		''' <seealso cref= #processName </seealso>
		''' <seealso cref= #getURI </seealso>
		''' <seealso cref= #getPrefix </seealso>
		Public Overridable Function declarePrefix(ByVal prefix As String, ByVal uri As String) As Boolean
			If prefix.Equals("xml") OrElse prefix.Equals("xmlns") Then
				Return False
			Else
				currentContext.declarePrefix(prefix, uri)
				Return True
			End If
		End Function


		''' <summary>
		''' Process a raw XML qualified name, after all declarations in the
		''' current context have been handled by {@link #declarePrefix
		''' declarePrefix()}.
		''' 
		''' <p>This method processes a raw XML qualified name in the
		''' current context by removing the prefix and looking it up among
		''' the prefixes currently declared.  The return value will be the
		''' array supplied by the caller, filled in as follows:</p>
		''' 
		''' <dl>
		''' <dt>parts[0]</dt>
		''' <dd>The Namespace URI, or an empty string if none is
		'''  in use.</dd>
		''' <dt>parts[1]</dt>
		''' <dd>The local name (without prefix).</dd>
		''' <dt>parts[2]</dt>
		''' <dd>The original raw name.</dd>
		''' </dl>
		''' 
		''' <p>All of the strings in the array will be internalized.  If
		''' the raw name has a prefix that has not been declared, then
		''' the return value will be null.</p>
		''' 
		''' <p>Note that attribute names are processed differently than
		''' element names: an unprefixed element name will receive the
		''' default Namespace (if any), while an unprefixed attribute name
		''' will not.</p>
		''' </summary>
		''' <param name="qName"> The XML qualified name to be processed. </param>
		''' <param name="parts"> An array supplied by the caller, capable of
		'''        holding at least three members. </param>
		''' <param name="isAttribute"> A flag indicating whether this is an
		'''        attribute name (true) or an element name (false). </param>
		''' <returns> The supplied array holding three internalized strings
		'''        representing the Namespace URI (or empty string), the
		'''        local name, and the XML qualified name; or null if there
		'''        is an undeclared prefix. </returns>
		''' <seealso cref= #declarePrefix </seealso>
		''' <seealso cref= java.lang.String#intern  </seealso>
		Public Overridable Function processName(ByVal qName As String, ByVal parts As String(), ByVal isAttribute As Boolean) As String()
			Dim myParts As String() = currentContext.processName(qName, isAttribute)
			If myParts Is Nothing Then
				Return Nothing
			Else
				parts(0) = myParts(0)
				parts(1) = myParts(1)
				parts(2) = myParts(2)
				Return parts
			End If
		End Function


		''' <summary>
		''' Look up a prefix and get the currently-mapped Namespace URI.
		''' 
		''' <p>This method looks up the prefix in the current context.
		''' Use the empty string ("") for the default Namespace.</p>
		''' </summary>
		''' <param name="prefix"> The prefix to look up. </param>
		''' <returns> The associated Namespace URI, or null if the prefix
		'''         is undeclared in this context. </returns>
		''' <seealso cref= #getPrefix </seealso>
		''' <seealso cref= #getPrefixes </seealso>
		Public Overridable Function getURI(ByVal prefix As String) As String
			Return currentContext.getURI(prefix)
		End Function


		''' <summary>
		''' Return an enumeration of all prefixes whose declarations are
		''' active in the current context.
		''' This includes declarations from parent contexts that have
		''' not been overridden.
		''' 
		''' <p><strong>Note:</strong> if there is a default prefix, it will not be
		''' returned in this enumeration; check for the default prefix
		''' using the <seealso cref="#getURI getURI"/> with an argument of "".</p>
		''' </summary>
		''' <returns> An enumeration of prefixes (never empty). </returns>
		''' <seealso cref= #getDeclaredPrefixes </seealso>
		''' <seealso cref= #getURI </seealso>
		Public Overridable Property prefixes As System.Collections.IEnumerator
			Get
				Return currentContext.prefixes
			End Get
		End Property


		''' <summary>
		''' Return one of the prefixes mapped to a Namespace URI.
		''' 
		''' <p>If more than one prefix is currently mapped to the same
		''' URI, this method will make an arbitrary selection; if you
		''' want all of the prefixes, use the <seealso cref="#getPrefixes"/>
		''' method instead.</p>
		''' 
		''' <p><strong>Note:</strong> this will never return the empty (default) prefix;
		''' to check for a default prefix, use the <seealso cref="#getURI getURI"/>
		''' method with an argument of "".</p>
		''' </summary>
		''' <param name="uri"> the namespace URI </param>
		''' <returns> one of the prefixes currently mapped to the URI supplied,
		'''         or null if none is mapped or if the URI is assigned to
		'''         the default namespace </returns>
		''' <seealso cref= #getPrefixes(java.lang.String) </seealso>
		''' <seealso cref= #getURI </seealso>
		Public Overridable Function getPrefix(ByVal uri As String) As String
			Return currentContext.getPrefix(uri)
		End Function


		''' <summary>
		''' Return an enumeration of all prefixes for a given URI whose
		''' declarations are active in the current context.
		''' This includes declarations from parent contexts that have
		''' not been overridden.
		''' 
		''' <p>This method returns prefixes mapped to a specific Namespace
		''' URI.  The xml: prefix will be included.  If you want only one
		''' prefix that's mapped to the Namespace URI, and you don't care
		''' which one you get, use the <seealso cref="#getPrefix getPrefix"/>
		'''  method instead.</p>
		''' 
		''' <p><strong>Note:</strong> the empty (default) prefix is <em>never</em> included
		''' in this enumeration; to check for the presence of a default
		''' Namespace, use the <seealso cref="#getURI getURI"/> method with an
		''' argument of "".</p>
		''' </summary>
		''' <param name="uri"> The Namespace URI. </param>
		''' <returns> An enumeration of prefixes (never empty). </returns>
		''' <seealso cref= #getPrefix </seealso>
		''' <seealso cref= #getDeclaredPrefixes </seealso>
		''' <seealso cref= #getURI </seealso>
		Public Overridable Function getPrefixes(ByVal uri As String) As System.Collections.IEnumerator
			Dim prefixes_Renamed As IList(Of String) = New List(Of String)
			Dim allPrefixes As System.Collections.IEnumerator = prefixes
			Do While allPrefixes.hasMoreElements()
				Dim prefix_Renamed As String = CStr(allPrefixes.nextElement())
				If uri.Equals(getURI(prefix_Renamed)) Then prefixes_Renamed.Add(prefix_Renamed)
			Loop
			Return java.util.Collections.enumeration(prefixes_Renamed)
		End Function


		''' <summary>
		''' Return an enumeration of all prefixes declared in this context.
		''' 
		''' <p>The empty (default) prefix will be included in this
		''' enumeration; note that this behaviour differs from that of
		''' <seealso cref="#getPrefix"/> and <seealso cref="#getPrefixes"/>.</p>
		''' </summary>
		''' <returns> An enumeration of all prefixes declared in this
		'''         context. </returns>
		''' <seealso cref= #getPrefixes </seealso>
		''' <seealso cref= #getURI </seealso>
		Public Overridable Property declaredPrefixes As System.Collections.IEnumerator
			Get
				Return currentContext.declaredPrefixes
			End Get
		End Property

		''' <summary>
		''' Controls whether namespace declaration attributes are placed
		''' into the <seealso cref="#NSDECL NSDECL"/> namespace
		''' by <seealso cref="#processName processName()"/>.  This may only be
		''' changed before any contexts have been pushed.
		''' 
		''' @since SAX 2.1alpha
		''' </summary>
		''' <exception cref="IllegalStateException"> when attempting to set this
		'''  after any context has been pushed. </exception>
		Public Overridable Property namespaceDeclUris As Boolean
			Set(ByVal value As Boolean)
				If contextPos <> 0 Then Throw New IllegalStateException
				If value = namespaceDeclUris Then Return
				namespaceDeclUris = value
				If value Then
					currentContext.declarePrefix("xmlns", NSDECL)
				Else
						currentContext = New Context(Me, Me)
						contexts(contextPos) = currentContext
					currentContext.declarePrefix("xml", XMLNS)
				End If
			End Set
			Get
					Return namespaceDeclUris
			End Get
		End Property




		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		Private contexts As Context()
		Private currentContext As Context
		Private contextPos As Integer
		Private namespaceDeclUris As Boolean


		'//////////////////////////////////////////////////////////////////
		' Internal classes.
		'//////////////////////////////////////////////////////////////////

		''' <summary>
		''' Internal class for a single Namespace context.
		''' 
		''' <p>This module caches and reuses Namespace contexts,
		''' so the number allocated
		''' will be equal to the element depth of the document, not to the total
		''' number of elements (i.e. 5-10 rather than tens of thousands).
		''' Also, data structures used to represent contexts are shared when
		''' possible (child contexts without declarations) to further reduce
		''' the amount of memory that's consumed.
		''' </p>
		''' </summary>
		Friend NotInheritable Class Context
			Private ReadOnly outerInstance As NamespaceSupport


			''' <summary>
			''' Create the root-level Namespace context.
			''' </summary>
			Friend Sub New(ByVal outerInstance As NamespaceSupport)
					Me.outerInstance = outerInstance
				copyTables()
			End Sub


			''' <summary>
			''' (Re)set the parent of this Namespace context.
			''' The context must either have been freshly constructed,
			''' or must have been cleared.
			''' </summary>
			''' <param name="context"> The parent Namespace context object. </param>
			Friend Property parent As Context
				Set(ByVal parent As Context)
					Me.parent = parent
					declarations = Nothing
					prefixTable = parent.prefixTable
					uriTable = parent.uriTable
					elementNameTable = parent.elementNameTable
					attributeNameTable = parent.attributeNameTable
					defaultNS = parent.defaultNS
					declSeen = False
				End Set
			End Property

			''' <summary>
			''' Makes associated state become collectible,
			''' invalidating this context.
			''' <seealso cref="#setParent"/> must be called before
			''' this context may be used again.
			''' </summary>
			Friend Sub clear()
				parent = Nothing
				prefixTable = Nothing
				uriTable = Nothing
				elementNameTable = Nothing
				attributeNameTable = Nothing
				defaultNS = Nothing
			End Sub


			''' <summary>
			''' Declare a Namespace prefix for this context.
			''' </summary>
			''' <param name="prefix"> The prefix to declare. </param>
			''' <param name="uri"> The associated Namespace URI. </param>
			''' <seealso cref= org.xml.sax.helpers.NamespaceSupport#declarePrefix </seealso>
			Friend Sub declarePrefix(ByVal prefix As String, ByVal uri As String)
									' Lazy processing...
	'          if (!declsOK)
	'              throw new IllegalStateException (
	'                  "can't declare any more prefixes in this context");
				If Not declSeen Then copyTables()
				If declarations Is Nothing Then declarations = New List(Of )

				prefix = prefix.intern()
				uri = uri.intern()
				If "".Equals(prefix) Then
					If "".Equals(uri) Then
						defaultNS = Nothing
					Else
						defaultNS = uri
					End If
				Else
					prefixTable(prefix) = uri
					uriTable(uri) = prefix ' may wipe out another prefix
				End If
				declarations.Add(prefix)
			End Sub


			''' <summary>
			''' Process an XML qualified name in this context.
			''' </summary>
			''' <param name="qName"> The XML qualified name. </param>
			''' <param name="isAttribute"> true if this is an attribute name. </param>
			''' <returns> An array of three strings containing the
			'''         URI part (or empty string), the local part,
			'''         and the raw name, all internalized, or null
			'''         if there is an undeclared prefix. </returns>
			''' <seealso cref= org.xml.sax.helpers.NamespaceSupport#processName </seealso>
			Friend Function processName(ByVal qName As String, ByVal isAttribute As Boolean) As String()
				Dim name As String()
				Dim table As IDictionary(Of String, String())

				' Select the appropriate table.
				If isAttribute Then
					table = attributeNameTable
				Else
					table = elementNameTable
				End If

				' Start by looking in the cache, and
				' return immediately if the name
				' is already known in this content
				name = CType(table(qName), String())
				If name IsNot Nothing Then Return name

				' We haven't seen this name in this
				' context before.  Maybe in the parent
				' context, but we can't assume prefix
				' bindings are the same.
				name = New String(2){}
				name(2) = qName.intern()
				Dim index As Integer = qName.IndexOf(":"c)


				' No prefix.
				If index = -1 Then
					If isAttribute Then
						If qName = "xmlns" AndAlso outerInstance.namespaceDeclUris Then
							name(0) = NSDECL
						Else
							name(0) = ""
						End If
					ElseIf defaultNS Is Nothing Then
						name(0) = ""
					Else
						name(0) = defaultNS
					End If
					name(1) = name(2)

				' Prefix
				Else
					Dim prefix_Renamed As String = qName.Substring(0, index)
					Dim local As String = qName.Substring(index+1)
					Dim uri_Renamed As String
					If "".Equals(prefix_Renamed) Then
						uri_Renamed = defaultNS
					Else
						uri_Renamed = CStr(prefixTable(prefix_Renamed))
					End If
					If uri_Renamed Is Nothing OrElse ((Not isAttribute) AndAlso "xmlns".Equals(prefix_Renamed)) Then Return Nothing
					name(0) = uri_Renamed
					name(1) = local.intern()
				End If

				' Save in the cache for future use.
				' (Could be shared with parent context...)
				table(name(2)) = name
				Return name
			End Function


			''' <summary>
			''' Look up the URI associated with a prefix in this context.
			''' </summary>
			''' <param name="prefix"> The prefix to look up. </param>
			''' <returns> The associated Namespace URI, or null if none is
			'''         declared. </returns>
			''' <seealso cref= org.xml.sax.helpers.NamespaceSupport#getURI </seealso>
			Friend Function getURI(ByVal prefix As String) As String
				If "".Equals(prefix) Then
					Return defaultNS
				ElseIf prefixTable Is Nothing Then
					Return Nothing
				Else
					Return CStr(prefixTable(prefix))
				End If
			End Function


			''' <summary>
			''' Look up one of the prefixes associated with a URI in this context.
			''' 
			''' <p>Since many prefixes may be mapped to the same URI,
			''' the return value may be unreliable.</p>
			''' </summary>
			''' <param name="uri"> The URI to look up. </param>
			''' <returns> The associated prefix, or null if none is declared. </returns>
			''' <seealso cref= org.xml.sax.helpers.NamespaceSupport#getPrefix </seealso>
			Friend Function getPrefix(ByVal uri As String) As String
				If uriTable Is Nothing Then
					Return Nothing
				Else
					Return CStr(uriTable(uri))
				End If
			End Function


			''' <summary>
			''' Return an enumeration of prefixes declared in this context.
			''' </summary>
			''' <returns> An enumeration of prefixes (possibly empty). </returns>
			''' <seealso cref= org.xml.sax.helpers.NamespaceSupport#getDeclaredPrefixes </seealso>
			Friend Property declaredPrefixes As System.Collections.IEnumerator
				Get
					If declarations Is Nothing Then
						Return EMPTY_ENUMERATION
					Else
						Return java.util.Collections.enumeration(declarations)
					End If
				End Get
			End Property

			''' <summary>
			''' Return an enumeration of all prefixes currently in force.
			''' 
			''' <p>The default prefix, if in force, is <em>not</em>
			''' returned, and will have to be checked for separately.</p>
			''' </summary>
			''' <returns> An enumeration of prefixes (never empty). </returns>
			''' <seealso cref= org.xml.sax.helpers.NamespaceSupport#getPrefixes </seealso>
			Friend Property prefixes As System.Collections.IEnumerator
				Get
					If prefixTable Is Nothing Then
						Return EMPTY_ENUMERATION
					Else
						Return java.util.Collections.enumeration(prefixTable.Keys)
					End If
				End Get
			End Property



			'//////////////////////////////////////////////////////////////
			' Internal methods.
			'//////////////////////////////////////////////////////////////


			''' <summary>
			''' Copy on write for the internal tables in this context.
			''' 
			''' <p>This class is optimized for the normal case where most
			''' elements do not contain Namespace declarations.</p>
			''' </summary>
			Private Sub copyTables()
				If prefixTable IsNot Nothing Then
					prefixTable = New Dictionary(Of )(prefixTable)
				Else
					prefixTable = New Dictionary(Of )
				End If
				If uriTable IsNot Nothing Then
					uriTable = New Dictionary(Of )(uriTable)
				Else
					uriTable = New Dictionary(Of )
				End If
				elementNameTable = New Dictionary(Of )
				attributeNameTable = New Dictionary(Of )
				declSeen = True
			End Sub



			'//////////////////////////////////////////////////////////////
			' Protected state.
			'//////////////////////////////////////////////////////////////

			Friend prefixTable As IDictionary(Of String, String)
			Friend uriTable As IDictionary(Of String, String)
			Friend elementNameTable As IDictionary(Of String, String())
			Friend attributeNameTable As IDictionary(Of String, String())
			Friend defaultNS As String = Nothing



			'//////////////////////////////////////////////////////////////
			' Internal state.
			'//////////////////////////////////////////////////////////////

			Private declarations As IList(Of String) = Nothing
			Private declSeen As Boolean = False
			Private parent As Context = Nothing
		End Class
	End Class

	' end of NamespaceSupport.java

End Namespace
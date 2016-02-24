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

' SAX default implementation for Locator.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: LocatorImpl.java,v 1.2 2004/11/03 22:53:09 jsuttor Exp $

Namespace org.xml.sax.helpers



	''' <summary>
	''' Provide an optional convenience implementation of Locator.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class is available mainly for application writers, who
	''' can use it to make a persistent snapshot of a locator at any
	''' point during a document parse:</p>
	''' 
	''' <pre>
	''' Locator locator;
	''' Locator startloc;
	''' 
	''' public void setLocator (Locator locator)
	''' {
	'''         // note the locator
	'''   this.locator = locator;
	''' }
	''' 
	''' public void startDocument ()
	''' {
	'''         // save the location of the start of the document
	'''         // for future use.
	'''   Locator startloc = new LocatorImpl(locator);
	''' }
	''' </pre>
	''' 
	''' <p>Normally, parser writers will not use this class, since it
	''' is more efficient to provide location information only when
	''' requested, rather than constantly updating a Locator object.</p>
	''' 
	''' @since SAX 1.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.Locator Locator </seealso>
	Public Class LocatorImpl
		Implements org.xml.sax.Locator


		''' <summary>
		''' Zero-argument constructor.
		''' 
		''' <p>This will not normally be useful, since the main purpose
		''' of this class is to make a snapshot of an existing Locator.</p>
		''' </summary>
		Public Sub New()
		End Sub


		''' <summary>
		''' Copy constructor.
		''' 
		''' <p>Create a persistent copy of the current state of a locator.
		''' When the original locator changes, this copy will still keep
		''' the original values (and it can be used outside the scope of
		''' DocumentHandler methods).</p>
		''' </summary>
		''' <param name="locator"> The locator to copy. </param>
		Public Sub New(ByVal locator As org.xml.sax.Locator)
			publicId = locator.publicId
			systemId = locator.systemId
			lineNumber = locator.lineNumber
			columnNumber = locator.columnNumber
		End Sub



		'//////////////////////////////////////////////////////////////////
		' Implementation of org.xml.sax.Locator
		'//////////////////////////////////////////////////////////////////


		''' <summary>
		''' Return the saved public identifier.
		''' </summary>
		''' <returns> The public identifier as a string, or null if none
		'''         is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getPublicId </seealso>
		''' <seealso cref= #setPublicId </seealso>
		Public Overridable Property publicId As String
			Get
				Return publicId
			End Get
			Set(ByVal publicId As String)
				Me.publicId = publicId
			End Set
		End Property


		''' <summary>
		''' Return the saved system identifier.
		''' </summary>
		''' <returns> The system identifier as a string, or null if none
		'''         is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getSystemId </seealso>
		''' <seealso cref= #setSystemId </seealso>
		Public Overridable Property systemId As String
			Get
				Return systemId
			End Get
			Set(ByVal systemId As String)
				Me.systemId = systemId
			End Set
		End Property


		''' <summary>
		''' Return the saved line number (1-based).
		''' </summary>
		''' <returns> The line number as an integer, or -1 if none is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getLineNumber </seealso>
		''' <seealso cref= #setLineNumber </seealso>
		Public Overridable Property lineNumber As Integer
			Get
				Return lineNumber
			End Get
			Set(ByVal lineNumber As Integer)
				Me.lineNumber = lineNumber
			End Set
		End Property


		''' <summary>
		''' Return the saved column number (1-based).
		''' </summary>
		''' <returns> The column number as an integer, or -1 if none is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getColumnNumber </seealso>
		''' <seealso cref= #setColumnNumber </seealso>
		Public Overridable Property columnNumber As Integer
			Get
				Return columnNumber
			End Get
			Set(ByVal columnNumber As Integer)
				Me.columnNumber = columnNumber
			End Set
		End Property



		'//////////////////////////////////////////////////////////////////
		' Setters for the properties (not in org.xml.sax.Locator)
		'//////////////////////////////////////////////////////////////////











		'//////////////////////////////////////////////////////////////////
		' Internal state.
		'//////////////////////////////////////////////////////////////////

		Private publicId As String
		Private systemId As String
		Private lineNumber As Integer
		Private columnNumber As Integer

	End Class

	' end of LocatorImpl.java

End Namespace
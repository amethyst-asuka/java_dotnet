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

' XMLFilter.java - filter SAX2 events.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the Public Domain.
' $Id: XMLFilter.java,v 1.2 2004/11/03 22:55:32 jsuttor Exp $

Namespace org.xml.sax


	''' <summary>
	''' Interface for an XML filter.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>An XML filter is like an XML reader, except that it obtains its
	''' events from another XML reader rather than a primary source like
	''' an XML document or database.  Filters can modify a stream of
	''' events as they pass on to the final application.</p>
	''' 
	''' <p>The XMLFilterImpl helper class provides a convenient base
	''' for creating SAX2 filters, by passing on all {@link org.xml.sax.EntityResolver
	''' EntityResolver}, <seealso cref="org.xml.sax.DTDHandler DTDHandler"/>,
	''' <seealso cref="org.xml.sax.ContentHandler ContentHandler"/> and {@link org.xml.sax.ErrorHandler
	''' ErrorHandler} events automatically.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.helpers.XMLFilterImpl </seealso>
	Public Interface XMLFilter
		Inherits XMLReader

		''' <summary>
		''' Set the parent reader.
		''' 
		''' <p>This method allows the application to link the filter to
		''' a parent reader (which may be another filter).  The argument
		''' may not be null.</p>
		''' </summary>
		''' <param name="parent"> The parent reader. </param>
		Property parent As XMLReader



	End Interface

	' end of XMLFilter.java

End Namespace
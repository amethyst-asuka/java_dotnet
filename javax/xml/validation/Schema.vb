'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.validation

	''' <summary>
	''' Immutable in-memory representation of grammar.
	''' 
	''' <p>
	''' This object represents a set of constraints that can be checked/
	''' enforced against an XML document.
	''' 
	''' <p>
	''' A <seealso cref="Schema"/> object is thread safe and applications are
	''' encouraged to share it across many parsers in many threads.
	''' 
	''' <p>
	''' A <seealso cref="Schema"/> object is immutable in the sense that it shouldn't
	''' change the set of constraints once it is created. In other words,
	''' if an application validates the same document twice against the same
	''' <seealso cref="Schema"/>, it must always produce the same result.
	''' 
	''' <p>
	''' A <seealso cref="Schema"/> object is usually created from <seealso cref="SchemaFactory"/>.
	''' 
	''' <p>
	''' Two kinds of validators can be created from a <seealso cref="Schema"/> object.
	''' One is <seealso cref="Validator"/>, which provides highly-level validation
	''' operations that cover typical use cases. The other is
	''' <seealso cref="ValidatorHandler"/>, which works on top of SAX for better
	''' modularity.
	''' 
	''' <p>
	''' This specification does not refine
	''' the <seealso cref="java.lang.Object#equals(java.lang.Object)"/> method.
	''' In other words, if you parse the same schema twice, you may
	''' still get <code>!schemaA.equals(schemaB)</code>.
	''' 
	''' @author <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a> </summary>
	''' <seealso cref= <a href="http://www.w3.org/TR/xmlschema-1/">XML Schema Part 1: Structures</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/xml11/">Extensible Markup Language (XML) 1.1</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/REC-xml">Extensible Markup Language (XML) 1.0 (Second Edition)</a>
	''' @since 1.5 </seealso>
	Public MustInherit Class Schema

		''' <summary>
		''' Constructor for the derived class.
		''' 
		''' <p>
		''' The constructor does nothing.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Creates a new <seealso cref="Validator"/> for this <seealso cref="Schema"/>.
		''' 
		''' <p>A validator enforces/checks the set of constraints this object
		''' represents.</p>
		''' 
		''' <p>Implementors should assure that the properties set on the
		''' <seealso cref="SchemaFactory"/> that created this <seealso cref="Schema"/> are also
		''' set on the <seealso cref="Validator"/> constructed.</p>
		''' 
		''' @return
		'''      Always return a non-null valid object.
		''' </summary>
		Public MustOverride Function newValidator() As Validator

		''' <summary>
		''' Creates a new <seealso cref="ValidatorHandler"/> for this <seealso cref="Schema"/>.
		''' 
		''' <p>Implementors should assure that the properties set on the
		''' <seealso cref="SchemaFactory"/> that created this <seealso cref="Schema"/> are also
		''' set on the <seealso cref="ValidatorHandler"/> constructed.</p>
		''' 
		''' @return
		'''      Always return a non-null valid object.
		''' </summary>
		Public MustOverride Function newValidatorHandler() As ValidatorHandler
	End Class

End Namespace
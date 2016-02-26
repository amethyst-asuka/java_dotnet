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

Namespace javax.xml.transform


	''' <summary>
	''' An instance of this abstract class can transform a
	''' source tree into a result tree.
	''' 
	''' <p>An instance of this class can be obtained with the
	''' <seealso cref="TransformerFactory#newTransformer TransformerFactory.newTransformer"/>
	''' method. This instance may then be used to process XML from a
	''' variety of sources and write the transformation output to a
	''' variety of sinks.</p>
	''' 
	''' <p>An object of this class may not be used in multiple threads
	''' running concurrently.  Different Transformers may be used
	''' concurrently by different threads.</p>
	''' 
	''' <p>A <code>Transformer</code> may be used multiple times.  Parameters and
	''' output properties are preserved across transformations.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public MustInherit Class Transformer

		''' <summary>
		''' Default constructor is protected on purpose.
		''' </summary>
		Protected Friend Sub New()
		End Sub

			''' <summary>
			''' <p>Reset this <code>Transformer</code> to its original configuration.</p>
			''' 
			''' <p><code>Transformer</code> is reset to the same state as when it was created with
			''' <seealso cref="TransformerFactory#newTransformer()"/>,
			''' <seealso cref="TransformerFactory#newTransformer(Source source)"/> or
			''' <seealso cref="Templates#newTransformer()"/>.
			''' <code>reset()</code> is designed to allow the reuse of existing <code>Transformer</code>s
			''' thus saving resources associated with the creation of new <code>Transformer</code>s.</p>
			''' 
			''' <p>The reset <code>Transformer</code> is not guaranteed to have the same <seealso cref="URIResolver"/>
			''' or <seealso cref="ErrorListener"/> <code>Object</code>s, e.g. <seealso cref="Object#equals(Object obj)"/>.
			''' It is guaranteed to have a functionally equal <code>URIResolver</code>
			''' and <code>ErrorListener</code>.</p>
			''' </summary>
			''' <exception cref="UnsupportedOperationException"> When implementation does not
			'''   override this method.
			''' 
			''' @since 1.5 </exception>
			Public Overridable Sub reset()

					' implementors should override this method
					Throw New System.NotSupportedException("This Transformer, """ & Me.GetType().name & """, does not support the reset functionality." & "  Specification """ & Me.GetType().Assembly.specificationTitle & """" & " version """ & Me.GetType().Assembly.specificationVersion & """")
			End Sub

		''' <summary>
		''' <p>Transform the XML <code>Source</code> to a <code>Result</code>.
		''' Specific transformation behavior is determined by the settings of the
		''' <code>TransformerFactory</code> in effect when the
		''' <code>Transformer</code> was instantiated and any modifications made to
		''' the <code>Transformer</code> instance.</p>
		''' 
		''' <p>An empty <code>Source</code> is represented as an empty document
		''' as constructed by <seealso cref="javax.xml.parsers.DocumentBuilder#newDocument()"/>.
		''' The result of transforming an empty <code>Source</code> depends on
		''' the transformation behavior; it is not always an empty
		''' <code>Result</code>.</p>
		''' </summary>
		''' <param name="xmlSource"> The XML input to transform. </param>
		''' <param name="outputTarget"> The <code>Result</code> of transforming the
		'''   <code>xmlSource</code>.
		''' </param>
		''' <exception cref="TransformerException"> If an unrecoverable error occurs
		'''   during the course of the transformation. </exception>
		Public MustOverride Sub transform(ByVal xmlSource As Source, ByVal outputTarget As Result)

		''' <summary>
		''' Add a parameter for the transformation.
		''' 
		''' <p>Pass a qualified name as a two-part string, the namespace URI
		''' enclosed in curly braces ({}), followed by the local name. If the
		''' name has a null URL, the String only contain the local name. An
		''' application can safely check for a non-null URI by testing to see if the
		''' first character of the name is a '{' character.</p>
		''' <p>For example, if a URI and local name were obtained from an element
		''' defined with &lt;xyz:foo
		''' xmlns:xyz="http://xyz.foo.com/yada/baz.html"/&gt;,
		''' then the qualified name would be "{http://xyz.foo.com/yada/baz.html}foo".
		''' Note that no prefix is used.</p>
		''' </summary>
		''' <param name="name"> The name of the parameter, which may begin with a
		''' namespace URI in curly braces ({}). </param>
		''' <param name="value"> The value object.  This can be any valid Java object. It is
		''' up to the processor to provide the proper object coersion or to simply
		''' pass the object on for use in an extension.
		''' </param>
		''' <exception cref="NullPointerException"> If value is null. </exception>
		 Public MustOverride Sub setParameter(ByVal name As String, ByVal value As Object)

		''' <summary>
		''' Get a parameter that was explicitly set with setParameter.
		''' 
		''' <p>This method does not return a default parameter value, which
		''' cannot be determined until the node context is evaluated during
		''' the transformation process.
		''' </summary>
		''' <param name="name"> of <code>Object</code> to get
		''' </param>
		''' <returns> A parameter that has been set with setParameter. </returns>
		Public MustOverride Function getParameter(ByVal name As String) As Object

		''' <summary>
		''' <p>Set a list of parameters.</p>
		''' 
		''' <p>Note that the list of parameters is specified as a
		''' <code>Properties</code> <code>Object</code> which limits the parameter
		''' values to <code>String</code>s.  Multiple calls to
		''' <seealso cref="#setParameter(String name, Object value)"/> should be used when the
		''' desired values are non-<code>String</code> <code>Object</code>s.
		''' The parameter names should conform as specified in
		''' <seealso cref="#setParameter(String name, Object value)"/>.
		''' An <code>IllegalArgumentException</code> is thrown if any names do not
		''' conform.</p>
		''' 
		''' <p>New parameters in the list are added to any existing parameters.
		''' If the name of a new parameter is equal to the name of an existing
		''' parameter as determined by <seealso cref="java.lang.Object#equals(Object obj)"/>,
		'''  the existing parameter is set to the new value.</p>
		''' </summary>
		''' <param name="params"> Parameters to set.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If any parameter names do not conform
		'''   to the naming rules. </exception>

		''' <summary>
		''' Clear all parameters set with setParameter.
		''' </summary>
		Public MustOverride Sub clearParameters()

		''' <summary>
		''' Set an object that will be used to resolve URIs used in
		''' document().
		''' 
		''' <p>If the resolver argument is null, the URIResolver value will
		''' be cleared and the transformer will no longer have a resolver.</p>
		''' </summary>
		''' <param name="resolver"> An object that implements the URIResolver interface,
		''' or null. </param>
		Public MustOverride Property uRIResolver As URIResolver


		''' <summary>
		''' Set the output properties for the transformation.  These
		''' properties will override properties set in the Templates
		''' with xsl:output.
		''' 
		''' <p>If argument to this function is null, any properties
		''' previously set are removed, and the value will revert to the value
		''' defined in the templates object.</p>
		''' 
		''' <p>Pass a qualified property key name as a two-part string, the namespace
		''' URI enclosed in curly braces ({}), followed by the local name. If the
		''' name has a null URL, the String only contain the local name. An
		''' application can safely check for a non-null URI by testing to see if the
		''' first character of the name is a '{' character.</p>
		''' <p>For example, if a URI and local name were obtained from an element
		''' defined with &lt;xyz:foo
		''' xmlns:xyz="http://xyz.foo.com/yada/baz.html"/&gt;,
		''' then the qualified name would be "{http://xyz.foo.com/yada/baz.html}foo".
		''' Note that no prefix is used.</p>
		''' An <code>IllegalArgumentException</code> is thrown  if any of the
		''' argument keys are not recognized and are not namespace qualified.
		''' </summary>
		''' <param name="oformat"> A set of output properties that will be
		'''   used to override any of the same properties in affect
		'''   for the transformation.
		''' </param>
		''' <exception cref="IllegalArgumentException"> When keys are not recognized and
		'''   are not namespace qualified.
		''' </exception>
		''' <seealso cref= javax.xml.transform.OutputKeys </seealso>
		''' <seealso cref= java.util.Properties
		'''  </seealso>
		Public MustOverride Property outputProperties As java.util.Properties


		''' <summary>
		''' Set an output property that will be in effect for the
		''' transformation.
		''' 
		''' <p>Pass a qualified property name as a two-part string, the namespace URI
		''' enclosed in curly braces ({}), followed by the local name. If the
		''' name has a null URL, the String only contain the local name. An
		''' application can safely check for a non-null URI by testing to see if the
		''' first character of the name is a '{' character.</p>
		''' <p>For example, if a URI and local name were obtained from an element
		''' defined with &lt;xyz:foo
		''' xmlns:xyz="http://xyz.foo.com/yada/baz.html"/&gt;,
		''' then the qualified name would be "{http://xyz.foo.com/yada/baz.html}foo".
		''' Note that no prefix is used.</p>
		''' 
		''' <p>The Properties object that was passed to <seealso cref="#setOutputProperties"/>
		''' won't be effected by calling this method.</p>
		''' </summary>
		''' <param name="name"> A non-null String that specifies an output
		''' property name, which may be namespace qualified. </param>
		''' <param name="value"> The non-null string value of the output property.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the property is not supported, and is
		''' not qualified with a namespace.
		''' </exception>
		''' <seealso cref= javax.xml.transform.OutputKeys </seealso>
		Public MustOverride Sub setOutputProperty(ByVal name As String, ByVal value As String)

		''' <summary>
		''' <p>Get an output property that is in effect for the transformer.</p>
		''' 
		''' <p>If a property has been set using <seealso cref="#setOutputProperty"/>,
		''' that value will be returned. Otherwise, if a property is explicitly
		''' specified in the stylesheet, that value will be returned. If
		''' the value of the property has been defaulted, that is, if no
		''' value has been set explicitly either with <seealso cref="#setOutputProperty"/> or
		''' in the stylesheet, the result may vary depending on
		''' implementation and input stylesheet.</p>
		''' </summary>
		''' <param name="name"> A non-null String that specifies an output
		''' property name, which may be namespace qualified.
		''' </param>
		''' <returns> The string value of the output property, or null
		''' if no property was found.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the property is not supported.
		''' </exception>
		''' <seealso cref= javax.xml.transform.OutputKeys </seealso>
		Public MustOverride Function getOutputProperty(ByVal name As String) As String

		''' <summary>
		''' Set the error event listener in effect for the transformation.
		''' </summary>
		''' <param name="listener"> The new error listener.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if listener is null. </exception>
		Public MustOverride Property errorListener As ErrorListener

	End Class

End Namespace
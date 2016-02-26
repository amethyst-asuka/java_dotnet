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
	''' This class provides access to the type information determined
	''' by <seealso cref="ValidatorHandler"/>.
	''' 
	''' <p>
	''' Some schema languages, such as W3C XML Schema, encourages a validator
	''' to report the "type" it assigns to each attribute/element.
	''' Those applications who wish to access this type information can invoke
	''' methods defined on this "interface" to access such type information.
	''' 
	''' <p>
	''' Implementation of this "interface" can be obtained through the
	''' <seealso cref="ValidatorHandler#getTypeInfoProvider()"/> method.
	''' 
	''' @author  <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a> </summary>
	''' <seealso cref= org.w3c.dom.TypeInfo
	''' @since 1.5 </seealso>
	Public MustInherit Class TypeInfoProvider

		''' <summary>
		''' Constructor for the derived class.
		''' 
		''' <p>
		''' The constructor does nothing.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' <p>Returns the immutable <seealso cref="TypeInfo"/> object for the current
		''' element.</p>
		''' 
		''' <p>The method may only be called by the startElement event
		''' or the endElement event
		''' of the <seealso cref="org.xml.sax.ContentHandler"/> that the application sets to
		''' the <seealso cref="ValidatorHandler"/>.</p>
		''' 
		''' <p>When W3C XML Schema validation is being performed, in the
		''' case where an element has a union type, the <seealso cref="TypeInfo"/>
		''' returned by a call to <code>getElementTypeInfo()</code> from the
		''' startElement
		''' event will be the union type. The <code>TypeInfo</code>
		''' returned by a call
		''' from the endElement event will be the actual member type used
		''' to validate the element.</p>
		''' </summary>
		''' <exception cref="IllegalStateException">
		'''      If this method is called from other <seealso cref="org.xml.sax.ContentHandler"/>
		'''      methods.
		''' @return
		'''      An immutable <seealso cref="TypeInfo"/> object that represents the
		'''      type of the current element.
		'''      Note that the caller can keep references to the obtained
		'''      <seealso cref="TypeInfo"/> longer than the callback scope.
		''' 
		'''      Otherwise, this method returns
		'''      null if the validator is unable to
		'''      determine the type of the current element for some reason
		'''      (for example, if the validator is recovering from
		'''      an earlier error.)
		'''  </exception>
		Public MustOverride ReadOnly Property elementTypeInfo As org.w3c.dom.TypeInfo

		''' <summary>
		''' Returns the immutable <seealso cref="TypeInfo"/> object for the specified
		''' attribute of the current element.
		''' 
		''' <p>
		''' The method may only be called by the startElement event of
		''' the <seealso cref="org.xml.sax.ContentHandler"/> that the application sets to the
		''' <seealso cref="ValidatorHandler"/>.</p>
		''' </summary>
		''' <param name="index">
		'''      The index of the attribute. The same index for
		'''      the <seealso cref="org.xml.sax.Attributes"/> object passed to the
		'''      <code>startElement</code> callback.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''      If the index is invalid. </exception>
		''' <exception cref="IllegalStateException">
		'''      If this method is called from other <seealso cref="org.xml.sax.ContentHandler"/>
		'''      methods.
		''' 
		''' @return
		'''      An immutable <seealso cref="TypeInfo"/> object that represents the
		'''      type of the specified attribute.
		'''      Note that the caller can keep references to the obtained
		'''      <seealso cref="TypeInfo"/> longer than the callback scope.
		''' 
		'''      Otherwise, this method returns
		'''      null if the validator is unable to
		'''      determine the type. </exception>
		Public MustOverride Function getAttributeTypeInfo(ByVal index As Integer) As org.w3c.dom.TypeInfo

		''' <summary>
		''' Returns <code>true</code> if the specified attribute is determined
		''' to be ID.
		''' 
		''' <p>
		''' Exacly how an attribute is "determined to be ID" is up to the
		''' schema language. In case of W3C XML Schema, this means
		''' that the actual type of the attribute is the built-in ID type
		''' or its derived type.
		''' 
		''' <p>
		''' A <seealso cref="javax.xml.parsers.DocumentBuilder"/> uses this information
		''' to properly implement <seealso cref="org.w3c.dom.Attr#isId()"/>.
		''' 
		''' <p>
		''' The method may only be called by the startElement event of
		''' the <seealso cref="org.xml.sax.ContentHandler"/> that the application sets to the
		''' <seealso cref="ValidatorHandler"/>.
		''' </summary>
		''' <param name="index">
		'''      The index of the attribute. The same index for
		'''      the <seealso cref="org.xml.sax.Attributes"/> object passed to the
		'''      <code>startElement</code> callback.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''      If the index is invalid. </exception>
		''' <exception cref="IllegalStateException">
		'''      If this method is called from other <seealso cref="org.xml.sax.ContentHandler"/>
		'''      methods.
		''' </exception>
		''' <returns> true
		'''      if the type of the specified attribute is ID. </returns>
		Public MustOverride Function isIdAttribute(ByVal index As Integer) As Boolean

		''' <summary>
		''' Returns <code>false</code> if the attribute was added by the validator.
		''' 
		''' <p>
		''' This method provides information necessary for
		''' a <seealso cref="javax.xml.parsers.DocumentBuilder"/> to determine what
		''' the DOM tree should return from the <seealso cref="org.w3c.dom.Attr#getSpecified()"/> method.
		''' 
		''' <p>
		''' The method may only be called by the startElement event of
		''' the <seealso cref="org.xml.sax.ContentHandler"/> that the application sets to the
		''' <seealso cref="ValidatorHandler"/>.
		''' 
		''' <p>
		''' A general guideline for validators is to return true if
		''' the attribute was originally present in the pipeline, and
		''' false if it was added by the validator.
		''' </summary>
		''' <param name="index">
		'''      The index of the attribute. The same index for
		'''      the <seealso cref="org.xml.sax.Attributes"/> object passed to the
		'''      <code>startElement</code> callback.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''      If the index is invalid. </exception>
		''' <exception cref="IllegalStateException">
		'''      If this method is called from other <seealso cref="org.xml.sax.ContentHandler"/>
		'''      methods.
		''' 
		''' @return
		'''      <code>true</code> if the attribute was present before the validator
		'''      processes input. <code>false</code> if the attribute was added
		'''      by the validator. </exception>
		Public MustOverride Function isSpecified(ByVal index As Integer) As Boolean
	End Class

End Namespace
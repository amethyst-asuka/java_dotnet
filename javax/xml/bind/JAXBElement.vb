Imports System

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind


	''' <summary>
	''' <p>JAXB representation of an Xml Element.</p>
	''' 
	''' <p>This class represents information about an Xml Element from both the element
	''' declaration within a schema and the element instance value within an xml document
	''' with the following properties
	''' <ul>
	'''   <li>element's xml tag <b><tt>name</tt></b></li>
	'''   <li><b><tt>value</tt></b> represents the element instance's atttribute(s) and content model</li>
	'''   <li>element declaration's <b><tt>declaredType</tt></b> (<tt>xs:element @type</tt> attribute)</li>
	'''   <li><b><tt>scope</tt></b> of element declaration</li>
	'''   <li>boolean <b><tt>nil</tt></b> property. (element instance's <tt><b>xsi:nil</b></tt> attribute)</li>
	''' </ul>
	''' 
	''' <p>The <tt>declaredType</tt> and <tt>scope</tt> property are the
	''' JAXB class binding for the xml type definition.
	''' </p>
	''' 
	''' <p><b><tt>Scope</tt></b> is either <seealso cref="GlobalScope"/> or the Java class representing the
	''' complex type definition containing the schema element declaration.
	''' </p>
	''' 
	''' <p>There is a property constraint that if <b><tt>value</tt></b> is <tt>null</tt>,
	''' then <tt>nil</tt> must be <tt>true</tt>. The converse is not true to enable
	''' representing a nil element with attribute(s). If <tt>nil</tt> is true, it is possible
	''' that <tt>value</tt> is non-null so it can hold the value of the attributes
	''' associated with a nil element.
	''' </p>
	''' 
	''' @author Kohsuke Kawaguchi, Joe Fialli
	''' @since JAXB 2.0
	''' </summary>

	<Serializable> _
	Public Class JAXBElement(Of T)

		''' <summary>
		''' xml element tag name </summary>
		Protected Friend ReadOnly name As javax.xml.namespace.QName

		''' <summary>
		''' Java datatype binding for xml element declaration's type. </summary>
		Protected Friend ReadOnly declaredType As Type

		''' <summary>
		''' Scope of xml element declaration representing this xml element instance.
		'''  Can be one of the following values:
		'''  - <seealso cref="GlobalScope"/> for global xml element declaration.
		'''  - local element declaration has a scope set to the Java class
		'''     representation of complex type defintion containing
		'''     xml element declaration.
		''' </summary>
		Protected Friend ReadOnly scope As Type

		''' <summary>
		''' xml element value.
		'''    Represents content model and attributes of an xml element instance. 
		''' </summary>
		Protected Friend value As T

		''' <summary>
		''' true iff the xml element instance has xsi:nil="true". </summary>
		Protected Friend nil As Boolean = False

		''' <summary>
		''' Designates global scope for an xml element.
		''' </summary>
		Public NotInheritable Class GlobalScope
		End Class

		''' <summary>
		''' <p>Construct an xml element instance.</p>
		''' </summary>
		''' <param name="name">          Java binding of xml element tag name </param>
		''' <param name="declaredType">  Java binding of xml element declaration's type </param>
		''' <param name="scope">
		'''      Java binding of scope of xml element declaration.
		'''      Passing null is the same as passing <tt>GlobalScope.class</tt> </param>
		''' <param name="value">
		'''      Java instance representing xml element's value. </param>
		''' <seealso cref= #getScope() </seealso>
		''' <seealso cref= #isTypeSubstituted() </seealso>
		Public Sub New(ByVal name As javax.xml.namespace.QName, ByVal declaredType As Type, ByVal scope As Type, ByVal value As T)
			If declaredType Is Nothing OrElse name Is Nothing Then Throw New System.ArgumentException
			Me.declaredType = declaredType
			If scope Is Nothing Then scope = GetType(GlobalScope)
			Me.scope = scope
			Me.name = name
			value = value
		End Sub

		''' <summary>
		''' Construct an xml element instance.
		''' 
		''' This is just a convenience method for <tt>new JAXBElement(name,declaredType,GlobalScope.class,value)</tt>
		''' </summary>
		Public Sub New(ByVal name As javax.xml.namespace.QName, ByVal declaredType As Type, ByVal value As T)
			Me.New(name,declaredType,GetType(GlobalScope),value)
		End Sub

		''' <summary>
		''' Returns the Java binding of the xml element declaration's type attribute.
		''' </summary>
		Public Overridable Property declaredType As Type
			Get
				Return declaredType
			End Get
		End Property

		''' <summary>
		''' Returns the xml element tag name.
		''' </summary>
		Public Overridable Property name As javax.xml.namespace.QName
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' <p>Set the content model and attributes of this xml element.</p>
		''' 
		''' <p>When this property is set to <tt>null</tt>, <tt>isNil()</tt> must by <tt>true</tt>.
		'''    Details of constraint are described at <seealso cref="#isNil()"/>.</p>
		''' </summary>
		''' <seealso cref= #isTypeSubstituted() </seealso>
		Public Overridable Property value As T
			Set(ByVal t As T)
				Me.value = t
			End Set
			Get
				Return value
			End Get
		End Property


		''' <summary>
		''' Returns scope of xml element declaration.
		''' </summary>
		''' <seealso cref= #isGlobalScope() </seealso>
		''' <returns> <tt>GlobalScope.class</tt> if this element is of global scope. </returns>
		Public Overridable Property scope As Type
			Get
				Return scope
			End Get
		End Property

		''' <summary>
		''' <p>Returns <tt>true</tt> iff this element instance content model
		''' is nil.</p>
		''' 
		''' <p>This property always returns <tt>true</tt> when <seealso cref="#getValue()"/> is null.
		''' Note that the converse is not true, when this property is <tt>true</tt>,
		''' <seealso cref="#getValue()"/> can contain a non-null value for attribute(s). It is
		''' valid for a nil xml element to have attribute(s).</p>
		''' </summary>
		Public Overridable Property nil As Boolean
			Get
				Return (value Is Nothing) OrElse nil
			End Get
			Set(ByVal value As Boolean)
				Me.nil = value
			End Set
		End Property


	'     Convenience methods
	'     * (Not necessary but they do unambiguously conceptualize
	'     *  the rationale behind this class' fields.)
	'     

		''' <summary>
		''' Returns true iff this xml element declaration is global.
		''' </summary>
		Public Overridable Property globalScope As Boolean
			Get
				Return Me.scope Is GetType(GlobalScope)
			End Get
		End Property

		''' <summary>
		''' Returns true iff this xml element instance's value has a different
		''' type than xml element declaration's declared type.
		''' </summary>
		Public Overridable Property typeSubstituted As Boolean
			Get
				If value Is Nothing Then Return False
				Return value.GetType() IsNot declaredType
			End Get
		End Property

		Private Const serialVersionUID As Long = 1L
	End Class

End Namespace
Imports System.Collections
Imports System.Collections.Generic
Imports sun.security.jca

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: TransformService.java,v 1.6.4.1 2005/09/15 12:42:11 mullan Exp $
' 
Namespace javax.xml.crypto.dsig



	''' <summary>
	''' A Service Provider Interface for transform and canonicalization algorithms.
	''' 
	''' <p>Each instance of <code>TransformService</code> supports a specific
	''' transform or canonicalization algorithm and XML mechanism type. To create a
	''' <code>TransformService</code>, call one of the static
	''' <seealso cref="#getInstance getInstance"/> methods, passing in the algorithm URI and
	''' XML mechanism type desired, for example:
	''' 
	''' <blockquote><code>
	''' TransformService ts = TransformService.getInstance(Transform.XPATH2, "DOM");
	''' </code></blockquote>
	''' 
	''' <p><code>TransformService</code> implementations are registered and loaded
	''' using the <seealso cref="java.security.Provider"/> mechanism.  Each
	''' <code>TransformService</code> service provider implementation should include
	''' a <code>MechanismType</code> service attribute that identifies the XML
	''' mechanism type that it supports. If the attribute is not specified,
	''' "DOM" is assumed. For example, a service provider that supports the
	''' XPath Filter 2 Transform and DOM mechanism would be specified in the
	''' <code>Provider</code> subclass as:
	''' <pre>
	'''     put("TransformService." + Transform.XPATH2,
	'''         "org.example.XPath2TransformService");
	'''     put("TransformService." + Transform.XPATH2 + " MechanismType", "DOM");
	''' </pre>
	''' <code>TransformService</code> implementations that support the DOM
	''' mechanism type must abide by the DOM interoperability requirements defined
	''' in the
	''' <a href="../../../../../technotes/guides/security/xmldsig/overview.html#DOM Mechanism Requirements">
	''' DOM Mechanism Requirements</a> section of the API overview. See the
	''' <a href="../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
	''' Service Providers</a> section of the API overview for a list of standard
	''' mechanism types.
	''' <p>
	''' Once a <code>TransformService</code> has been created, it can be used
	''' to process <code>Transform</code> or <code>CanonicalizationMethod</code>
	''' objects. If the <code>Transform</code> or <code>CanonicalizationMethod</code>
	''' exists in XML form (for example, when validating an existing
	''' <code>XMLSignature</code>), the <seealso cref="#init(XMLStructure, XMLCryptoContext)"/>
	''' method must be first called to initialize the transform and provide document
	''' context (even if there are no parameters). Alternatively, if the
	''' <code>Transform</code> or <code>CanonicalizationMethod</code> is being
	''' created from scratch, the <seealso cref="#init(TransformParameterSpec)"/> method
	''' is called to initialize the transform with parameters and the
	''' <seealso cref="#marshalParams marshalParams"/> method is called to marshal the
	''' parameters to XML and provide the transform with document context. Finally,
	''' the <seealso cref="#transform transform"/> method is called to perform the
	''' transformation.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>The static methods of this class are guaranteed to be thread-safe.
	''' Multiple threads may concurrently invoke the static methods defined in this
	''' class with no ill effects.
	''' 
	''' <p>However, this is not true for the non-static methods defined by this
	''' class. Unless otherwise documented by a specific provider, threads that
	''' need to access a single <code>TransformService</code> instance
	''' concurrently should synchronize amongst themselves and provide the
	''' necessary locking. Multiple threads each manipulating a different
	''' <code>TransformService</code> instance need not synchronize.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public MustInherit Class TransformService
		Implements Transform

		Private algorithm As String
		Private mechanism As String
		Private provider As java.security.Provider

		''' <summary>
		''' Default constructor, for invocation by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns a <code>TransformService</code> that supports the specified
		''' algorithm URI (ex: <seealso cref="Transform#XPATH2"/>) and mechanism type
		''' (ex: DOM).
		''' 
		''' <p>This method uses the standard JCA provider lookup mechanism to
		''' locate and instantiate a <code>TransformService</code> implementation
		''' of the desired algorithm and <code>MechanismType</code> service
		''' attribute. It traverses the list of registered security
		''' <code>Provider</code>s, starting with the most preferred
		''' <code>Provider</code>. A new <code>TransformService</code> object
		''' from the first <code>Provider</code> that supports the specified
		''' algorithm and mechanism type is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the URI of the algorithm </param>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''   representation </param>
		''' <returns> a new <code>TransformService</code> </returns>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> or
		'''   <code>mechanismType</code> is  <code>null</code> </exception>
		''' <exception cref="NoSuchAlgorithmException"> if no <code>Provider</code> supports a
		'''   <code>TransformService</code> implementation for the specified
		'''   algorithm and mechanism type </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal algorithm As String, ByVal mechanismType As String) As TransformService
			If mechanismType Is Nothing OrElse algorithm Is Nothing Then Throw New NullPointerException
			Dim dom As Boolean = False
			If mechanismType.Equals("DOM") Then dom = True
			Dim services As IList(Of java.security.Provider.Service) = GetInstance.getServices("TransformService", algorithm)
			Dim t As IEnumerator(Of java.security.Provider.Service) = services.GetEnumerator()
			Do While t.MoveNext()
				Dim s As java.security.Provider.Service = t.Current
				Dim value As String = s.getAttribute("MechanismType")
				If (value Is Nothing AndAlso dom) OrElse (value IsNot Nothing AndAlso value.Equals(mechanismType)) Then
					Dim ___instance As sun.security.jca.GetInstance.Instance = GetInstance.getInstance(s, Nothing)
					Dim ts As TransformService = CType(___instance.impl, TransformService)
					ts.algorithm = algorithm
					ts.mechanism = mechanismType
					ts.provider = ___instance.provider
					Return ts
				End If
			Loop
			Throw New java.security.NoSuchAlgorithmException(algorithm & " algorithm and " & mechanismType & " mechanism not available")
		End Function

		''' <summary>
		''' Returns a <code>TransformService</code> that supports the specified
		''' algorithm URI (ex: <seealso cref="Transform#XPATH2"/>) and mechanism type
		''' (ex: DOM) as supplied by the specified provider. Note that the specified
		''' <code>Provider</code> object does not have to be registered in the
		''' provider list.
		''' </summary>
		''' <param name="algorithm"> the URI of the algorithm </param>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''   representation </param>
		''' <param name="provider"> the <code>Provider</code> object </param>
		''' <returns> a new <code>TransformService</code> </returns>
		''' <exception cref="NullPointerException"> if <code>provider</code>,
		'''   <code>algorithm</code>, or <code>mechanismType</code> is
		'''   <code>null</code> </exception>
		''' <exception cref="NoSuchAlgorithmException"> if a <code>TransformService</code>
		'''   implementation for the specified algorithm and mechanism type is not
		'''   available from the specified <code>Provider</code> object </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal algorithm As String, ByVal mechanismType As String, ByVal provider As java.security.Provider) As TransformService
			If mechanismType Is Nothing OrElse algorithm Is Nothing OrElse provider Is Nothing Then Throw New NullPointerException

			Dim dom As Boolean = False
			If mechanismType.Equals("DOM") Then dom = True
			Dim s As java.security.Provider.Service = GetInstance.getService("TransformService", algorithm, provider)
			Dim value As String = s.getAttribute("MechanismType")
			If (value Is Nothing AndAlso dom) OrElse (value IsNot Nothing AndAlso value.Equals(mechanismType)) Then
				Dim ___instance As sun.security.jca.GetInstance.Instance = GetInstance.getInstance(s, Nothing)
				Dim ts As TransformService = CType(___instance.impl, TransformService)
				ts.algorithm = algorithm
				ts.mechanism = mechanismType
				ts.provider = ___instance.provider
				Return ts
			End If
			Throw New java.security.NoSuchAlgorithmException(algorithm & " algorithm and " & mechanismType & " mechanism not available")
		End Function

		''' <summary>
		''' Returns a <code>TransformService</code> that supports the specified
		''' algorithm URI (ex: <seealso cref="Transform#XPATH2"/>) and mechanism type
		''' (ex: DOM) as supplied by the specified provider. The specified provider
		''' must be registered in the security provider list.
		''' 
		''' <p>Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the URI of the algorithm </param>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''   representation </param>
		''' <param name="provider"> the string name of the provider </param>
		''' <returns> a new <code>TransformService</code> </returns>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''   registered in the security provider list </exception>
		''' <exception cref="NullPointerException"> if <code>provider</code>,
		'''   <code>mechanismType</code>, or <code>algorithm</code> is
		'''   <code>null</code> </exception>
		''' <exception cref="NoSuchAlgorithmException"> if a <code>TransformService</code>
		'''   implementation for the specified algorithm and mechanism type is not
		'''   available from the specified provider </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal algorithm As String, ByVal mechanismType As String, ByVal provider As String) As TransformService
			If mechanismType Is Nothing OrElse algorithm Is Nothing OrElse provider Is Nothing Then
				Throw New NullPointerException
			ElseIf provider.Length = 0 Then
				Throw New java.security.NoSuchProviderException
			End If
			Dim dom As Boolean = False
			If mechanismType.Equals("DOM") Then dom = True
			Dim s As java.security.Provider.Service = GetInstance.getService("TransformService", algorithm, provider)
			Dim value As String = s.getAttribute("MechanismType")
			If (value Is Nothing AndAlso dom) OrElse (value IsNot Nothing AndAlso value.Equals(mechanismType)) Then
				Dim ___instance As sun.security.jca.GetInstance.Instance = GetInstance.getInstance(s, Nothing)
				Dim ts As TransformService = CType(___instance.impl, TransformService)
				ts.algorithm = algorithm
				ts.mechanism = mechanismType
				ts.provider = ___instance.provider
				Return ts
			End If
			Throw New java.security.NoSuchAlgorithmException(algorithm & " algorithm and " & mechanismType & " mechanism not available")
		End Function

		Private Class MechanismMapEntry
			Implements KeyValuePair(Of String, String)

			Private ReadOnly mechanism As String
			Private ReadOnly algorithm As String
			Private ReadOnly key As String
			Friend Sub New(ByVal algorithm As String, ByVal mechanism As String)
				Me.algorithm = algorithm
				Me.mechanism = mechanism
				Me.key = "TransformService." & algorithm & " MechanismType"
			End Sub
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Return (If(key Is Nothing, e.Key Is Nothing, key.Equals(e.Key))) AndAlso (If(value Is Nothing, e.Value Is Nothing, value.Equals(e.Value)))
			End Function
			Public Overridable Property key As String
				Get
					Return key
				End Get
			End Property
			Public Overridable Property value As String
				Get
					Return mechanism
				End Get
			End Property
			Public Overridable Function setValue(ByVal value As String) As String
				Throw New System.NotSupportedException
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return (If(key Is Nothing, 0, key.GetHashCode())) Xor (If(value Is Nothing, 0, value.GetHashCode()))
			End Function
		End Class

		''' <summary>
		''' Returns the mechanism type supported by this <code>TransformService</code>.
		''' </summary>
		''' <returns> the mechanism type </returns>
		Public Property mechanismType As String
			Get
				Return mechanism
			End Get
		End Property

		''' <summary>
		''' Returns the URI of the algorithm supported by this
		''' <code>TransformService</code>.
		''' </summary>
		''' <returns> the algorithm URI </returns>
		Public Property algorithm As String Implements AlgorithmMethod.getAlgorithm
			Get
				Return algorithm
			End Get
		End Property

		''' <summary>
		''' Returns the provider of this <code>TransformService</code>.
		''' </summary>
		''' <returns> the provider </returns>
		Public Property provider As java.security.Provider
			Get
				Return provider
			End Get
		End Property

		''' <summary>
		''' Initializes this <code>TransformService</code> with the specified
		''' parameters.
		''' 
		''' <p>If the parameters exist in XML form, the
		''' <seealso cref="#init(XMLStructure, XMLCryptoContext)"/> method should be used to
		''' initialize the <code>TransformService</code>.
		''' </summary>
		''' <param name="params"> the algorithm parameters (may be <code>null</code> if
		'''   not required or optional) </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''   are invalid for this algorithm </exception>
		Public MustOverride Sub init(ByVal params As javax.xml.crypto.dsig.spec.TransformParameterSpec)

		''' <summary>
		''' Marshals the algorithm-specific parameters. If there are no parameters
		''' to be marshalled, this method returns without throwing an exception.
		''' </summary>
		''' <param name="parent"> a mechanism-specific structure containing the parent
		'''    node that the marshalled parameters should be appended to </param>
		''' <param name="context"> the <code>XMLCryptoContext</code> containing
		'''    additional context (may be <code>null</code> if not applicable) </param>
		''' <exception cref="ClassCastException"> if the type of <code>parent</code> or
		'''    <code>context</code> is not compatible with this
		'''    <code>TransformService</code> </exception>
		''' <exception cref="NullPointerException"> if <code>parent</code> is <code>null</code> </exception>
		''' <exception cref="MarshalException"> if the parameters cannot be marshalled </exception>
		Public MustOverride Sub marshalParams(ByVal parent As javax.xml.crypto.XMLStructure, ByVal context As javax.xml.crypto.XMLCryptoContext)

		''' <summary>
		''' Initializes this <code>TransformService</code> with the specified
		''' parameters and document context.
		''' </summary>
		''' <param name="parent"> a mechanism-specific structure containing the parent
		'''    structure </param>
		''' <param name="context"> the <code>XMLCryptoContext</code> containing
		'''    additional context (may be <code>null</code> if not applicable) </param>
		''' <exception cref="ClassCastException"> if the type of <code>parent</code> or
		'''    <code>context</code> is not compatible with this
		'''    <code>TransformService</code> </exception>
		''' <exception cref="NullPointerException"> if <code>parent</code> is <code>null</code> </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''   are invalid for this algorithm </exception>
		Public MustOverride Sub init(ByVal parent As javax.xml.crypto.XMLStructure, ByVal context As javax.xml.crypto.XMLCryptoContext)
	End Class

End Namespace
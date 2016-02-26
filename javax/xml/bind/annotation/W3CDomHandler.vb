'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation



	''' <summary>
	''' <seealso cref="DomHandler"/> implementation for W3C DOM (<code>org.w3c.dom</code> package.)
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB2.0
	''' </summary>
	Public Class W3CDomHandler
		Implements DomHandler(Of org.w3c.dom.Element, javax.xml.transform.dom.DOMResult)

		Private builder As javax.xml.parsers.DocumentBuilder

		''' <summary>
		''' Default constructor.
		''' 
		''' It is up to a JAXB provider to decide which DOM implementation
		''' to use or how that is configured.
		''' </summary>
		Public Sub New()
			Me.builder = Nothing
		End Sub

		''' <summary>
		''' Constructor that allows applications to specify which DOM implementation
		''' to be used.
		''' </summary>
		''' <param name="builder">
		'''      must not be null. JAXB uses this <seealso cref="DocumentBuilder"/> to create
		'''      a new element. </param>
		Public Sub New(ByVal builder As javax.xml.parsers.DocumentBuilder)
			If builder Is Nothing Then Throw New System.ArgumentException
			Me.builder = builder
		End Sub

		Public Overridable Property builder As javax.xml.parsers.DocumentBuilder
			Get
				Return builder
			End Get
			Set(ByVal builder As javax.xml.parsers.DocumentBuilder)
				Me.builder = builder
			End Set
		End Property


		Public Overridable Function createUnmarshaller(ByVal errorHandler As javax.xml.bind.ValidationEventHandler) As javax.xml.transform.dom.DOMResult
			If builder Is Nothing Then
				Return New javax.xml.transform.dom.DOMResult
			Else
				Return New javax.xml.transform.dom.DOMResult(builder.newDocument())
			End If
		End Function

		Public Overridable Function getElement(ByVal r As javax.xml.transform.dom.DOMResult) As org.w3c.dom.Element
			' JAXP spec is ambiguous about what really happens in this case,
			' so work defensively
			Dim n As org.w3c.dom.Node = r.node
			If TypeOf n Is org.w3c.dom.Document Then Return CType(n, org.w3c.dom.Document).documentElement
			If TypeOf n Is org.w3c.dom.Element Then Return CType(n, org.w3c.dom.Element)
			If TypeOf n Is org.w3c.dom.DocumentFragment Then Return CType(n.childNodes.item(0), org.w3c.dom.Element)

			' if the result object contains something strange,
			' it is not a user problem, but it is a JAXB provider's problem.
			' That's why we throw a runtime exception.
			Throw New IllegalStateException(n.ToString())
		End Function

		Public Overridable Function marshal(ByVal element As org.w3c.dom.Element, ByVal errorHandler As javax.xml.bind.ValidationEventHandler) As javax.xml.transform.Source
			Return New javax.xml.transform.dom.DOMSource(element)
		End Function
	End Class

End Namespace
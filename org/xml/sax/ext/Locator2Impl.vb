'
' * Copyright (c) 2004, 2005, Oracle and/or its affiliates. All rights reserved.
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

' Locator2Impl.java - extended LocatorImpl
' http://www.saxproject.org
' Public Domain: no warranty.
' $Id: Locator2Impl.java,v 1.2 2004/11/03 22:49:08 jsuttor Exp $

Namespace org.xml.sax.ext



	''' <summary>
	''' SAX2 extension helper for holding additional Entity information,
	''' implementing the <seealso cref="Locator2"/> interface.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' </blockquote>
	''' 
	''' <p> This is not part of core-only SAX2 distributions.</p>
	''' 
	''' @since SAX 2.0.2
	''' @author David Brownell
	''' </summary>
	Public Class Locator2Impl
		Inherits org.xml.sax.helpers.LocatorImpl
		Implements Locator2

		Private encoding As String
		Private version As String


		''' <summary>
		''' Construct a new, empty Locator2Impl object.
		''' This will not normally be useful, since the main purpose
		''' of this class is to make a snapshot of an existing Locator.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Copy an existing Locator or Locator2 object.
		''' If the object implements Locator2, values of the
		''' <em>encoding</em> and <em>version</em>strings are copied,
		''' otherwise they set to <em>null</em>.
		''' </summary>
		''' <param name="locator"> The existing Locator object. </param>
		Public Sub New(ByVal locator As org.xml.sax.Locator)
			MyBase.New(locator)
			If TypeOf locator Is Locator2 Then
				Dim l2 As Locator2 = CType(locator, Locator2)

				version = l2.xMLVersion
				encoding = l2.encoding
			End If
		End Sub

		'//////////////////////////////////////////////////////////////////
		' Locator2 method implementations
		'//////////////////////////////////////////////////////////////////

		''' <summary>
		''' Returns the current value of the version property.
		''' </summary>
		''' <seealso cref= #setXMLVersion </seealso>
		Public Overridable Property xMLVersion As String Implements Locator2.getXMLVersion
			Get
					Return version
			End Get
			Set(ByVal version As String)
					Me.version = version
			End Set
		End Property

		''' <summary>
		''' Returns the current value of the encoding property.
		''' </summary>
		''' <seealso cref= #setEncoding </seealso>
		Public Overridable Property encoding As String Implements Locator2.getEncoding
			Get
					Return encoding
			End Get
			Set(ByVal encoding As String)
					Me.encoding = encoding
			End Set
		End Property


		'//////////////////////////////////////////////////////////////////
		' Setters
		'//////////////////////////////////////////////////////////////////


	End Class

End Namespace
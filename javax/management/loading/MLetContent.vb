Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.loading


	' java import


	''' <summary>
	''' This class represents the contents of the <CODE>MLET</CODE> tag.
	''' It can be consulted by a subclass of <seealso cref="MLet"/> that overrides
	''' the <seealso cref="MLet#check MLet.check"/> method.
	''' 
	''' @since 1.6
	''' </summary>
	Public Class MLetContent


		''' <summary>
		''' A map of the attributes of the <CODE>MLET</CODE> tag
		''' and their values.
		''' </summary>
		Private attributes As IDictionary(Of String, String)

		''' <summary>
		''' An ordered list of the TYPE attributes that appeared in nested
		''' &lt;PARAM&gt; tags.
		''' </summary>
		Private types As IList(Of String)

		''' <summary>
		''' An ordered list of the VALUE attributes that appeared in nested
		''' &lt;PARAM&gt; tags.
		''' </summary>
		Private values As IList(Of String)

		''' <summary>
		''' The MLet text file's base URL.
		''' </summary>
		Private documentURL As java.net.URL
		''' <summary>
		''' The base URL.
		''' </summary>
		Private baseURL As java.net.URL


		''' <summary>
		''' Creates an <CODE>MLet</CODE> instance initialized with attributes read
		''' from an <CODE>MLET</CODE> tag in an MLet text file.
		''' </summary>
		''' <param name="url"> The URL of the MLet text file containing the
		''' <CODE>MLET</CODE> tag. </param>
		''' <param name="attributes"> A map of the attributes of the <CODE>MLET</CODE> tag.
		''' The keys in this map are the attribute names in lowercase, for
		''' example <code>codebase</code>.  The values are the associated attribute
		''' values. </param>
		''' <param name="types"> A list of the TYPE attributes that appeared in nested
		''' &lt;PARAM&gt; tags. </param>
		''' <param name="values"> A list of the VALUE attributes that appeared in nested
		''' &lt;PARAM&gt; tags. </param>
		Public Sub New(ByVal url As java.net.URL, ByVal attributes As IDictionary(Of String, String), ByVal types As IList(Of String), ByVal values As IList(Of String))
			Me.documentURL = url
			Me.attributes = java.util.Collections.unmodifiableMap(attributes)
			Me.types = java.util.Collections.unmodifiableList(types)
			Me.values = java.util.Collections.unmodifiableList(values)

			' Initialize baseURL
			'
			Dim att As String = getParameter("codebase")
			If att IsNot Nothing Then
				If Not att.EndsWith("/") Then att &= "/"
				Try
					baseURL = New java.net.URL(documentURL, att)
				Catch e As java.net.MalformedURLException
					' OK : Move to next block as baseURL could not be initialized.
				End Try
			End If
			If baseURL Is Nothing Then
				Dim file As String = documentURL.file
				Dim i As Integer = file.LastIndexOf("/"c)
				If i >= 0 AndAlso i < file.Length - 1 Then
					Try
						baseURL = New java.net.URL(documentURL, file.Substring(0, i + 1))
					Catch e As java.net.MalformedURLException
						' OK : Move to next block as baseURL could not be initialized.
					End Try
				End If
			End If
			If baseURL Is Nothing Then baseURL = documentURL

		End Sub

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the attributes of the <CODE>MLET</CODE> tag.  The keys in
		''' the returned map are the attribute names in lowercase, for
		''' example <code>codebase</code>.  The values are the associated
		''' attribute values. </summary>
		''' <returns> A map of the attributes of the <CODE>MLET</CODE> tag
		''' and their values. </returns>
		Public Overridable Property attributes As IDictionary(Of String, String)
			Get
				Return attributes
			End Get
		End Property

		''' <summary>
		''' Gets the MLet text file's base URL. </summary>
		''' <returns> The MLet text file's base URL. </returns>
		Public Overridable Property documentBase As java.net.URL
			Get
				Return documentURL
			End Get
		End Property

		''' <summary>
		''' Gets the code base URL. </summary>
		''' <returns> The code base URL. </returns>
		Public Overridable Property codeBase As java.net.URL
			Get
				Return baseURL
			End Get
		End Property

		''' <summary>
		''' Gets the list of <CODE>.jar</CODE> files specified by the <CODE>ARCHIVE</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </summary>
		''' <returns> A comma-separated list of <CODE>.jar</CODE> file names. </returns>
		Public Overridable Property jarFiles As String
			Get
				Return getParameter("archive")
			End Get
		End Property

		''' <summary>
		''' Gets the value of the <CODE>CODE</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </summary>
		''' <returns> The value of the <CODE>CODE</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </returns>
		Public Overridable Property code As String
			Get
				Return getParameter("code")
			End Get
		End Property

		''' <summary>
		''' Gets the value of the <CODE>OBJECT</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </summary>
		''' <returns> The value of the <CODE>OBJECT</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </returns>
		Public Overridable Property serializedObject As String
			Get
				Return getParameter("object")
			End Get
		End Property

		''' <summary>
		''' Gets the value of the <CODE>NAME</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </summary>
		''' <returns> The value of the <CODE>NAME</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </returns>
		Public Overridable Property name As String
			Get
				Return getParameter("name")
			End Get
		End Property


		''' <summary>
		''' Gets the value of the <CODE>VERSION</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </summary>
		''' <returns> The value of the <CODE>VERSION</CODE>
		''' attribute of the <CODE>MLET</CODE> tag. </returns>
		Public Overridable Property version As String
			Get
				Return getParameter("version")
			End Get
		End Property

		''' <summary>
		''' Gets the list of values of the <code>TYPE</code> attribute in
		''' each nested &lt;PARAM&gt; tag within the <code>MLET</code>
		''' tag. </summary>
		''' <returns> the list of types. </returns>
		Public Overridable Property parameterTypes As IList(Of String)
			Get
				Return types
			End Get
		End Property

		''' <summary>
		''' Gets the list of values of the <code>VALUE</code> attribute in
		''' each nested &lt;PARAM&gt; tag within the <code>MLET</code>
		''' tag. </summary>
		''' <returns> the list of values. </returns>
		Public Overridable Property parameterValues As IList(Of String)
			Get
				Return values
			End Get
		End Property

		''' <summary>
		''' Gets the value of the specified
		''' attribute of the <CODE>MLET</CODE> tag.
		''' </summary>
		''' <param name="name"> A string representing the name of the attribute. </param>
		''' <returns> The value of the specified
		''' attribute of the <CODE>MLET</CODE> tag. </returns>
		Private Function getParameter(ByVal name As String) As String
			Return attributes(name.ToLower())
		End Function

	End Class

End Namespace
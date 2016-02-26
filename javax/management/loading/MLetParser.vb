Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.MLET_LOGGER

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' This class is used for parsing URLs.
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class MLetParser

	'
	'  * ------------------------------------------
	'  *   PRIVATE VARIABLES
	'  * ------------------------------------------
	'  

		''' <summary>
		''' The current character
		''' </summary>
		Private c As Integer

		''' <summary>
		''' Tag to parse.
		''' </summary>
		Private Shared tag As String = "mlet"


	'  
	'  * ------------------------------------------
	'  *   CONSTRUCTORS
	'  * ------------------------------------------
	'  

		''' <summary>
		''' Create an MLet parser object
		''' </summary>
		Public Sub New()
		End Sub

	'    
	'     * ------------------------------------------
	'     *   PUBLIC METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Scan spaces.
		''' </summary>
		Public Overridable Sub skipSpace(ByVal [in] As java.io.Reader)
			Do While (c >= 0) AndAlso ((c = AscW(" "c)) OrElse (c = ControlChars.Tab) OrElse (c = ControlChars.Lf) OrElse (c = ControlChars.Cr))
				c = [in].read()
			Loop
		End Sub

		''' <summary>
		''' Scan identifier
		''' </summary>
		Public Overridable Function scanIdentifier(ByVal [in] As java.io.Reader) As String
			Dim buf As New StringBuilder
			Do
				If ((c >= "a"c) AndAlso (c <= "z"c)) OrElse ((c >= "A"c) AndAlso (c <= "Z"c)) OrElse ((c >= "0"c) AndAlso (c <= "9"c)) OrElse (c = AscW("_"c)) Then
					buf.Append(ChrW(c))
					c = [in].read()
				Else
					Return buf.ToString()
				End If
			Loop
		End Function

		''' <summary>
		''' Scan tag
		''' </summary>
		Public Overridable Function scanTag(ByVal [in] As java.io.Reader) As IDictionary(Of String, String)
			Dim atts As IDictionary(Of String, String) = New Dictionary(Of String, String)
			skipSpace([in])
			Do While c >= 0 AndAlso c <> AscW(">"c)
				If c = AscW("<"c) Then Throw New java.io.IOException("Missing '>' in tag")
				Dim att As String = scanIdentifier([in])
				Dim val As String = ""
				skipSpace([in])
				If c = AscW("="c) Then
					Dim quote As Integer = -1
					c = [in].read()
					skipSpace([in])
					If (c = AscW("'"c)) OrElse (c = """"c) Then
						quote = c
						c = [in].read()
					End If
					Dim buf As New StringBuilder
					Do While (c > 0) AndAlso (((quote < 0) AndAlso (c <> AscW(" "c)) AndAlso (c <> ControlChars.Tab) AndAlso (c <> ControlChars.Lf) AndAlso (c <> ControlChars.Cr) AndAlso (c <> AscW(">"c))) OrElse ((quote >= 0) AndAlso (c <> quote)))
						buf.Append(ChrW(c))
						c = [in].read()
					Loop
					If c = quote Then c = [in].read()
					skipSpace([in])
					val = buf.ToString()
				End If
				atts(att.ToLower()) = val
				skipSpace([in])
			Loop
			Return atts
		End Function

		''' <summary>
		''' Scan an html file for {@literal <mlet>} tags.
		''' </summary>
		Public Overridable Function parse(ByVal url As java.net.URL) As IList(Of MLetContent)
			Dim mth As String = "parse"
			' Warning Messages
			Dim requiresTypeWarning As String = "<arg type=... value=...> tag requires type parameter."
			Dim requiresValueWarning As String = "<arg type=... value=...> tag requires value parameter."
			Dim paramOutsideWarning As String = "<arg> tag outside <mlet> ... </mlet>."
			Dim requiresCodeWarning As String = "<mlet> tag requires either code or object parameter."
			Dim requiresJarsWarning As String = "<mlet> tag requires archive parameter."

			Dim conn As java.net.URLConnection

			conn = url.openConnection()
			Dim [in] As java.io.Reader = New java.io.BufferedReader(New java.io.InputStreamReader(conn.inputStream, "UTF-8"))

			' The original URL may have been redirected - this
			' sets it to whatever URL/codebase we ended up getting
			'
			url = conn.uRL

			Dim mlets As IList(Of MLetContent) = New List(Of MLetContent)
			Dim atts As IDictionary(Of String, String) = Nothing

			Dim types As IList(Of String) = New List(Of String)
			Dim values As IList(Of String) = New List(Of String)

			' debug("parse","*** Parsing " + url );
			Do
				c = [in].read()
				If c = -1 Then Exit Do
				If c = AscW("<"c) Then
					c = [in].read()
					If c = AscW("/"c) Then
						c = [in].read()
						Dim nm As String = scanIdentifier([in])
						If c <> AscW(">"c) Then Throw New java.io.IOException("Missing '>' in tag")
						If nm.ToUpper() = tag.ToUpper() Then
							If atts IsNot Nothing Then mlets.Add(New MLetContent(url, atts, types, values))
							atts = Nothing
							types = New List(Of String)
							values = New List(Of String)
						End If
					Else
						Dim nm As String = scanIdentifier([in])
						If nm.ToUpper() = "arg".ToUpper() Then
							Dim t As IDictionary(Of String, String) = scanTag([in])
							Dim att As String = t("type")
							If att Is Nothing Then
								MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLetParser).name, mth, requiresTypeWarning)
								Throw New java.io.IOException(requiresTypeWarning)
							Else
								If atts IsNot Nothing Then
									types.Add(att)
								Else
									MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLetParser).name, mth, paramOutsideWarning)
									Throw New java.io.IOException(paramOutsideWarning)
								End If
							End If
							Dim val As String = t("value")
							If val Is Nothing Then
								MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLetParser).name, mth, requiresValueWarning)
								Throw New java.io.IOException(requiresValueWarning)
							Else
								If atts IsNot Nothing Then
									values.Add(val)
								Else
									MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLetParser).name, mth, paramOutsideWarning)
									Throw New java.io.IOException(paramOutsideWarning)
								End If
							End If
						Else
							If nm.ToUpper() = tag.ToUpper() Then
								atts = scanTag([in])
								If atts("code") Is Nothing AndAlso atts("object") Is Nothing Then
									MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLetParser).name, mth, requiresCodeWarning)
									Throw New java.io.IOException(requiresCodeWarning)
								End If
								If atts("archive") Is Nothing Then
									MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLetParser).name, mth, requiresJarsWarning)
									Throw New java.io.IOException(requiresJarsWarning)
								End If
							End If
						End If
					End If
				End If
			Loop
			[in].close()
			Return mlets
		End Function

		''' <summary>
		''' Parse the document pointed by the URL urlname
		''' </summary>
		Public Overridable Function parseURL(ByVal urlname As String) As IList(Of MLetContent)
			' Parse the document
			'
			Dim url As java.net.URL
			If urlname.IndexOf(":"c) <= 1 Then
				Dim userDir As String = System.getProperty("user.dir")
				Dim prot As String
				If userDir.Chars(0) = "/"c OrElse userDir.Chars(0) = System.IO.Path.DirectorySeparatorChar Then
					prot = "file:"
				Else
					prot = "file:/"
				End If
				url = New java.net.URL(prot + userDir.Replace(System.IO.Path.DirectorySeparatorChar, "/"c) & "/")
				url = New java.net.URL(url, urlname)
			Else
				url = New java.net.URL(urlname)
			End If
			' Return list of parsed MLets
			'
			Return parse(url)
		End Function

	End Class

End Namespace
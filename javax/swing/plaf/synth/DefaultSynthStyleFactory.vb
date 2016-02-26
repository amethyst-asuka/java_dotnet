Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing
Imports sun.swing.plaf.synth

'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth


	''' <summary>
	''' Factory used for obtaining styles. Supports associating a style based on
	''' the name of the component as returned by <code>Component.getName()</code>,
	''' and the <code>Region</code> associated with the <code>JComponent</code>.
	''' Lookup is done using regular expressions.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class DefaultSynthStyleFactory
		Inherits SynthStyleFactory

		''' <summary>
		''' Used to indicate the lookup should be done based on Component name.
		''' </summary>
		Public Const NAME As Integer = 0
		''' <summary>
		''' Used to indicate the lookup should be done based on region.
		''' </summary>
		Public Const REGION As Integer = 1

		''' <summary>
		''' List containing set of StyleAssociations used in determining matching
		''' styles.
		''' </summary>
		Private _styles As IList(Of StyleAssociation)
		''' <summary>
		''' Used during lookup.
		''' </summary>
		Private _tmpList As sun.swing.BakedArrayList

		''' <summary>
		''' Maps from a List (BakedArrayList to be precise) to the merged style.
		''' </summary>
		Private _resolvedStyles As IDictionary(Of sun.swing.BakedArrayList, SynthStyle)

		''' <summary>
		''' Used if there are no styles matching a widget.
		''' </summary>
		Private _defaultStyle As SynthStyle


		Friend Sub New()
			_tmpList = New sun.swing.BakedArrayList(5)
			_styles = New List(Of StyleAssociation)
			_resolvedStyles = New Dictionary(Of sun.swing.BakedArrayList, SynthStyle)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addStyle(ByVal style As DefaultSynthStyle, ByVal path As String, ByVal type As Integer)
			If path Is Nothing Then path = ".*"
			If type = NAME Then
				_styles.Add(StyleAssociation.createStyleAssociation(path, style, type))
			ElseIf type = REGION Then
				_styles.Add(StyleAssociation.createStyleAssociation(path.ToLower(), style, type))
			End If
		End Sub

		''' <summary>
		''' Returns the style for the specified Component.
		''' </summary>
		''' <param name="c"> Component asking for </param>
		''' <param name="id"> ID of the Component </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getStyle(ByVal c As JComponent, ByVal id As Region) As SynthStyle
			Dim matches As sun.swing.BakedArrayList = _tmpList

			matches.clear()
			getMatchingStyles(matches, c, id)

			If matches.size() = 0 Then Return defaultStyle
			' Use a cached Style if possible, otherwise create a new one.
			matches.cacheHashCode()
			Dim ___style As SynthStyle = getCachedStyle(matches)

			If ___style Is Nothing Then
				___style = mergeStyles(matches)

				If ___style IsNot Nothing Then cacheStyle(matches, ___style)
			End If
			Return ___style
		End Function

		''' <summary>
		''' Returns the style to use if there are no matching styles.
		''' </summary>
		Private Property defaultStyle As SynthStyle
			Get
				If _defaultStyle Is Nothing Then
					_defaultStyle = New DefaultSynthStyle
					CType(_defaultStyle, DefaultSynthStyle).font = New javax.swing.plaf.FontUIResource(java.awt.Font.DIALOG, java.awt.Font.PLAIN,12)
				End If
				Return _defaultStyle
			End Get
		End Property

		''' <summary>
		''' Fetches any styles that match the passed into arguments into
		''' <code>matches</code>.
		''' </summary>
		Private Sub getMatchingStyles(ByVal matches As IList, ByVal c As JComponent, ByVal id As Region)
			Dim idName As String = id.lowerCaseName
			Dim cName As String = c.name

			If cName Is Nothing Then cName = ""
			For counter As Integer = _styles.Count - 1 To 0 Step -1
				Dim sa As StyleAssociation = _styles(counter)
				Dim path As String

				If sa.iD = NAME Then
					path = cName
				Else
					path = idName
				End If

				If sa.matches(path) AndAlso matches.IndexOf(sa.style) = -1 Then matches.Add(sa.style)
			Next counter
		End Sub

		''' <summary>
		''' Caches the specified style.
		''' </summary>
		Private Sub cacheStyle(ByVal styles As IList, ByVal style As SynthStyle)
			Dim cachedStyles As New sun.swing.BakedArrayList(styles)

			_resolvedStyles(cachedStyles) = style
		End Sub

		''' <summary>
		''' Returns the cached style from the passed in arguments.
		''' </summary>
		Private Function getCachedStyle(ByVal styles As IList) As SynthStyle
			If styles.Count = 0 Then Return Nothing
			Return _resolvedStyles(styles)
		End Function

		''' <summary>
		''' Creates a single Style from the passed in styles. The passed in List
		''' is reverse sorted, that is the most recently added style found to
		''' match will be first.
		''' </summary>
		Private Function mergeStyles(ByVal styles As IList) As SynthStyle
			Dim size As Integer = styles.Count

			If size = 0 Then
				Return Nothing
			ElseIf size = 1 Then
				Return CType(CType(styles(0), DefaultSynthStyle).clone(), SynthStyle)
			End If
			' NOTE: merging is done backwards as DefaultSynthStyleFactory reverses
			' order, that is, the most specific style is first.
			Dim ___style As DefaultSynthStyle = CType(styles(size - 1), DefaultSynthStyle)

			___style = CType(___style.clone(), DefaultSynthStyle)
			For counter As Integer = size - 2 To 0 Step -1
				___style = CType(styles(counter), DefaultSynthStyle).addTo(___style)
			Next counter
			Return ___style
		End Function
	End Class

End Namespace
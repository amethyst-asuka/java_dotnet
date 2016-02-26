Imports System
Imports javax.swing.event

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text

	''' <summary>
	''' A <code>LabelView</code> is a styled chunk of text
	''' that represents a view mapped over an element in the
	''' text model.  It caches the character level attributes
	''' used for rendering.
	''' 
	''' @author Timothy Prinzing
	''' </summary>
	Public Class LabelView
		Inherits GlyphView
		Implements TabableView

		''' <summary>
		''' Constructs a new view wrapped on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Synchronize the view's cached values with the model.
		''' This causes the font, metrics, color, etc to be
		''' re-cached if the cache has been invalidated.
		''' </summary>
		Friend Sub sync()
			If font Is Nothing Then propertiesFromAttributestes()
		End Sub

		''' <summary>
		''' Sets whether or not the view is underlined.
		''' Note that this setter is protected and is really
		''' only meant if you need to update some additional
		''' state when set.
		''' </summary>
		''' <param name="u"> true if the view is underlined, otherwise
		'''          false </param>
		''' <seealso cref= #isUnderline </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overridable Sub setUnderline(ByVal u As Boolean) 'JavaToDotNetTempPropertySetunderline
		Protected Friend Overridable Property underline As Boolean
			Set(ByVal u As Boolean)
				underline = u
			End Set
			Get
		End Property

		''' <summary>
		''' Sets whether or not the view has a strike/line
		''' through it.
		''' Note that this setter is protected and is really
		''' only meant if you need to update some additional
		''' state when set.
		''' </summary>
		''' <param name="s"> true if the view has a strike/line
		'''          through it, otherwise false </param>
		''' <seealso cref= #isStrikeThrough </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overridable Sub setStrikeThrough(ByVal s As Boolean) 'JavaToDotNetTempPropertySetstrikeThrough
		Protected Friend Overridable Property strikeThrough As Boolean
			Set(ByVal s As Boolean)
				strike = s
			End Set
			Get
		End Property


		''' <summary>
		''' Sets whether or not the view represents a
		''' superscript.
		''' Note that this setter is protected and is really
		''' only meant if you need to update some additional
		''' state when set.
		''' </summary>
		''' <param name="s"> true if the view represents a
		'''          superscript, otherwise false </param>
		''' <seealso cref= #isSuperscript </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overridable Sub setSuperscript(ByVal s As Boolean) 'JavaToDotNetTempPropertySetsuperscript
		Protected Friend Overridable Property superscript As Boolean
			Set(ByVal s As Boolean)
				superscript = s
			End Set
			Get
		End Property

		''' <summary>
		''' Sets whether or not the view represents a
		''' subscript.
		''' Note that this setter is protected and is really
		''' only meant if you need to update some additional
		''' state when set.
		''' </summary>
		''' <param name="s"> true if the view represents a
		'''          subscript, otherwise false </param>
		''' <seealso cref= #isSubscript </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overridable Sub setSubscript(ByVal s As Boolean) 'JavaToDotNetTempPropertySetsubscript
		Protected Friend Overridable Property subscript As Boolean
			Set(ByVal s As Boolean)
				subscript = s
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the background color for the view. This method is typically
		''' invoked as part of configuring this <code>View</code>. If you need
		''' to customize the background color you should override
		''' <code>setPropertiesFromAttributes</code> and invoke this method. A
		''' value of null indicates no background should be rendered, so that the
		''' background of the parent <code>View</code> will show through.
		''' </summary>
		''' <param name="bg"> background color, or null </param>
		''' <seealso cref= #setPropertiesFromAttributes
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overridable Sub setBackground(ByVal bg As Color) 'JavaToDotNetTempPropertySetbackground
		Protected Friend Overridable Property background As Color
			Set(ByVal bg As Color)
				Me.bg = bg
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the cached properties from the attributes.
		''' </summary>
		Protected Friend Overridable Sub setPropertiesFromAttributes()
			Dim attr As AttributeSet = attributes
			If attr IsNot Nothing Then
				Dim d As Document = document
				If TypeOf d Is StyledDocument Then
					Dim doc As StyledDocument = CType(d, StyledDocument)
					font = doc.getFont(attr)
					fg = doc.getForeground(attr)
					If attr.isDefined(StyleConstants.Background) Then
						bg = doc.getBackground(attr)
					Else
						bg = Nothing
					End If
					underline = StyleConstants.isUnderline(attr)
					strikeThrough = StyleConstants.isStrikeThrough(attr)
					superscript = StyleConstants.isSuperscript(attr)
					subscript = StyleConstants.isSubscript(attr)
				Else
					Throw New StateInvariantError("LabelView needs StyledDocument")
				End If
			End If
		End Sub

		''' <summary>
		''' Fetches the <code>FontMetrics</code> used for this view. </summary>
		''' @deprecated FontMetrics are not used for glyph rendering
		'''  when running in the JDK. 
		<Obsolete("FontMetrics are not used for glyph rendering")> _
		Protected Friend Overridable Property fontMetrics As FontMetrics
			Get
				sync()
				Dim c As Container = container
				Return If(c IsNot Nothing, c.getFontMetrics(font), Toolkit.defaultToolkit.getFontMetrics(font))
			End Get
		End Property

			sync()
			Return bg
		End Function

		''' <summary>
		''' Fetches the foreground color to use to render the glyphs.
		''' This is implemented to return a cached foreground color,
		''' which defaults to <code>null</code>.
		''' </summary>
		''' <returns> the cached foreground color
		''' @since 1.3 </returns>
		Public Property Overrides foreground As Color
			Get
				sync()
				Return fg
			End Get
		End Property

		''' <summary>
		''' Fetches the font that the glyphs should be based upon.
		''' This is implemented to return a cached font.
		''' </summary>
		''' <returns> the cached font </returns>
		 Public Property Overrides font As Font
			 Get
				sync()
				Return font
			 End Get
		 End Property

			sync()
			Return underline
		End Function

			sync()
			Return strike
		End Function

			sync()
			Return subscript
		End Function

			sync()
			Return superscript
		End Function

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			font = Nothing
			MyBase.changedUpdate(e, a, f)
		End Sub

		' --- variables ------------------------------------------------

		Private font As Font
		Private fg As Color
		Private bg As Color
		Private underline As Boolean
		Private strike As Boolean
		Private superscript As Boolean
		Private subscript As Boolean

	End Class

End Namespace
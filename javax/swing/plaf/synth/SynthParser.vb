Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2003, 2010, Oracle and/or its affiliates. All rights reserved.
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





	Friend Class SynthParser
		Inherits org.xml.sax.helpers.DefaultHandler

		'
		' Known element names
		'
		Private Const ELEMENT_SYNTH As String = "synth"
		Private Const ELEMENT_STYLE As String = "style"
		Private Const ELEMENT_STATE As String = "state"
		Private Const ELEMENT_FONT As String = "font"
		Private Const ELEMENT_COLOR As String = "color"
		Private Const ELEMENT_IMAGE_PAINTER As String = "imagePainter"
		Private Const ELEMENT_PAINTER As String = "painter"
		Private Const ELEMENT_PROPERTY As String = "property"
		Private Const ELEMENT_SYNTH_GRAPHICS As String = "graphicsUtils"
		Private Const ELEMENT_IMAGE_ICON As String = "imageIcon"
		Private Const ELEMENT_BIND As String = "bind"
		Private Const ELEMENT_BIND_KEY As String = "bindKey"
		Private Const ELEMENT_INSETS As String = "insets"
		Private Const ELEMENT_OPAQUE As String = "opaque"
		Private Const ELEMENT_DEFAULTS_PROPERTY As String = "defaultsProperty"
		Private Const ELEMENT_INPUT_MAP As String = "inputMap"

		'
		' Known attribute names
		'
		Private Const ATTRIBUTE_ACTION As String = "action"
		Private Const ATTRIBUTE_ID As String = "id"
		Private Const ATTRIBUTE_IDREF As String = "idref"
		Private Const ATTRIBUTE_CLONE As String = "clone"
		Private Const ATTRIBUTE_VALUE As String = "value"
		Private Const ATTRIBUTE_NAME As String = "name"
		Private Const ATTRIBUTE_STYLE As String = "style"
		Private Const ATTRIBUTE_SIZE As String = "size"
		Private Const ATTRIBUTE_TYPE As String = "type"
		Private Const ATTRIBUTE_TOP As String = "top"
		Private Const ATTRIBUTE_LEFT As String = "left"
		Private Const ATTRIBUTE_BOTTOM As String = "bottom"
		Private Const ATTRIBUTE_RIGHT As String = "right"
		Private Const ATTRIBUTE_KEY As String = "key"
		Private Const ATTRIBUTE_SOURCE_INSETS As String = "sourceInsets"
		Private Const ATTRIBUTE_DEST_INSETS As String = "destinationInsets"
		Private Const ATTRIBUTE_PATH As String = "path"
		Private Const ATTRIBUTE_STRETCH As String = "stretch"
		Private Const ATTRIBUTE_PAINT_CENTER As String = "paintCenter"
		Private Const ATTRIBUTE_METHOD As String = "method"
		Private Const ATTRIBUTE_DIRECTION As String = "direction"
		Private Const ATTRIBUTE_CENTER As String = "center"

		''' <summary>
		''' Lazily created, used for anything we don't understand.
		''' </summary>
		Private _handler As com.sun.beans.decoder.DocumentHandler

		''' <summary>
		''' Indicates the depth of how many elements we've encountered but don't
		''' understand. This is used when forwarding to beans persistance to know
		''' when we hsould stop forwarding.
		''' </summary>
		Private _depth As Integer

		''' <summary>
		''' Factory that new styles are added to.
		''' </summary>
		Private _factory As DefaultSynthStyleFactory

		''' <summary>
		''' Array of state infos for the current style. These are pushed to the
		''' style when </style> is received.
		''' </summary>
		Private _stateInfos As IList(Of ParsedSynthStyle.StateInfo)

		''' <summary>
		''' Current style.
		''' </summary>
		Private _style As ParsedSynthStyle

		''' <summary>
		''' Current state info.
		''' </summary>
		Private _stateInfo As ParsedSynthStyle.StateInfo

		''' <summary>
		''' Bindings for the current InputMap
		''' </summary>
		Private _inputMapBindings As IList(Of String)

		''' <summary>
		''' ID for the input map. This is cached as
		''' the InputMap is created AFTER the inputMapProperty has ended.
		''' </summary>
		Private _inputMapID As String

		''' <summary>
		''' Object references outside the scope of persistance.
		''' </summary>
		Private _mapping As IDictionary(Of String, Object)

		''' <summary>
		''' Based URL used to resolve paths.
		''' </summary>
		Private _urlResourceBase As java.net.URL

		''' <summary>
		''' Based class used to resolve paths.
		''' </summary>
		Private _classResourceBase As Type

		''' <summary>
		''' List of ColorTypes. This is populated in startColorType.
		''' </summary>
		Private _colorTypes As IList(Of ColorType)

		''' <summary>
		''' defaultsPropertys are placed here.
		''' </summary>
		Private _defaultsMap As IDictionary(Of String, Object)

		''' <summary>
		''' List of SynthStyle.Painters that will be applied to the current style.
		''' </summary>
		Private _stylePainters As IList(Of ParsedSynthStyle.PainterInfo)

		''' <summary>
		''' List of SynthStyle.Painters that will be applied to the current state.
		''' </summary>
		Private _statePainters As IList(Of ParsedSynthStyle.PainterInfo)

		Friend Sub New()
			_mapping = New Dictionary(Of String, Object)
			_stateInfos = New List(Of ParsedSynthStyle.StateInfo)
			_colorTypes = New List(Of ColorType)
			_inputMapBindings = New List(Of String)
			_stylePainters = New List(Of ParsedSynthStyle.PainterInfo)
			_statePainters = New List(Of ParsedSynthStyle.PainterInfo)
		End Sub

		''' <summary>
		''' Parses a set of styles from <code>inputStream</code>, adding the
		''' resulting styles to the passed in DefaultSynthStyleFactory.
		''' Resources are resolved either from a URL or from a Class. When calling
		''' this method, one of the URL or the Class must be null but not both at
		''' the same time.
		''' </summary>
		''' <param name="inputStream"> XML document containing the styles to read </param>
		''' <param name="factory"> DefaultSynthStyleFactory that new styles are added to </param>
		''' <param name="urlResourceBase"> the URL used to resolve any resources, such as Images </param>
		''' <param name="classResourceBase"> the Class used to resolve any resources, such as Images </param>
		''' <param name="defaultsMap"> Map that UIDefaults properties are placed in </param>
		Public Overridable Sub parse(ByVal inputStream As java.io.InputStream, ByVal factory As DefaultSynthStyleFactory, ByVal urlResourceBase As java.net.URL, ByVal classResourceBase As Type, ByVal defaultsMap As IDictionary(Of String, Object))
			If inputStream Is Nothing OrElse factory Is Nothing OrElse (urlResourceBase Is Nothing AndAlso classResourceBase Is Nothing) Then Throw New System.ArgumentException("You must supply an InputStream, StyleFactory and Class or URL")

			assert(Not(urlResourceBase IsNot Nothing AndAlso classResourceBase IsNot Nothing))

			_factory = factory
			_classResourceBase = classResourceBase
			_urlResourceBase = urlResourceBase
			_defaultsMap = defaultsMap
			Try
				Try
					Dim saxParser As javax.xml.parsers.SAXParser = javax.xml.parsers.SAXParserFactory.newInstance().newSAXParser()
					saxParser.parse(New java.io.BufferedInputStream(inputStream), Me)
				Catch e As javax.xml.parsers.ParserConfigurationException
					Throw New java.text.ParseException("Error parsing: " & e, 0)
				Catch se As org.xml.sax.SAXException
					Throw New java.text.ParseException("Error parsing: " & se & " " & se.exception, 0)
				Catch ioe As java.io.IOException
					Throw New java.text.ParseException("Error parsing: " & ioe, 0)
				End Try
			Finally
				reset()
			End Try
		End Sub

		''' <summary>
		''' Returns the path to a resource.
		''' </summary>
		Private Function getResource(ByVal path As String) As java.net.URL
			If _classResourceBase IsNot Nothing Then
				Return _classResourceBase.getResource(path)
			Else
				Try
					Return New java.net.URL(_urlResourceBase, path)
				Catch mue As java.net.MalformedURLException
					Return Nothing
				End Try
			End If
		End Function

		''' <summary>
		''' Clears our internal state.
		''' </summary>
		Private Sub reset()
			_handler = Nothing
			_depth = 0
			_mapping.Clear()
			_stateInfos.Clear()
			_colorTypes.Clear()
			_statePainters.Clear()
			_stylePainters.Clear()
		End Sub

		''' <summary>
		''' Returns true if we are forwarding to persistance.
		''' </summary>
		Private Property forwarding As Boolean
			Get
				Return (_depth > 0)
			End Get
		End Property

		''' <summary>
		''' Handles beans persistance.
		''' </summary>
		Private Property handler As com.sun.beans.decoder.DocumentHandler
			Get
				If _handler Is Nothing Then
					_handler = New com.sun.beans.decoder.DocumentHandler
					If _urlResourceBase IsNot Nothing Then
						' getHandler() is never called before parse() so it is safe
						' to create a URLClassLoader with _resourceBase.
						'
						' getResource(".") is called to ensure we have the directory
						' containing the resources in the case the resource base is a
						' .class file.
						Dim urls As java.net.URL() = { getResource(".") }
						Dim parent As ClassLoader = Thread.CurrentThread.contextClassLoader
						Dim urlLoader As ClassLoader = New java.net.URLClassLoader(urls, parent)
						_handler.classLoader = urlLoader
					Else
						_handler.classLoader = _classResourceBase.classLoader
					End If
    
					For Each key As String In _mapping.Keys
						_handler.variableble(key, _mapping(key))
					Next key
				End If
				Return _handler
			End Get
		End Property

		''' <summary>
		''' If <code>value</code> is an instance of <code>type</code> it is
		''' returned, otherwise a SAXException is thrown.
		''' </summary>
		Private Function checkCast(ByVal value As Object, ByVal type As Type) As Object
			If Not type.IsInstanceOfType(value) Then Throw New org.xml.sax.SAXException("Expected type " & type & " got " & value.GetType())
			Return value
		End Function

		''' <summary>
		''' Returns an object created with id=key. If the object is not of
		''' type type, this will throw an exception.
		''' </summary>
		Private Function lookup(ByVal key As String, ByVal type As Type) As Object
			Dim value As Object
			If _handler IsNot Nothing Then
				If _handler.hasVariable(key) Then Return checkCast(_handler.getVariable(key), type)
			End If
			value = _mapping(key)
			If value Is Nothing Then Throw New org.xml.sax.SAXException("ID " & key & " has not been defined")
			Return checkCast(value, type)
		End Function

		''' <summary>
		''' Registers an object by name. This will throw an exception if an
		''' object has already been registered under the given name.
		''' </summary>
		Private Sub register(ByVal key As String, ByVal value As Object)
			If key IsNot Nothing Then
				If _mapping(key) IsNot Nothing OrElse (_handler IsNot Nothing AndAlso _handler.hasVariable(key)) Then Throw New org.xml.sax.SAXException("ID " & key & " is already defined")
				If _handler IsNot Nothing Then
					_handler.variableble(key, value)
				Else
					_mapping(key) = value
				End If
			End If
		End Sub

		''' <summary>
		''' Convenience method to return the next int, or throw if there are no
		''' more valid ints.
		''' </summary>
		Private Function nextInt(ByVal tok As java.util.StringTokenizer, ByVal errorMsg As String) As Integer
			If Not tok.hasMoreTokens() Then Throw New org.xml.sax.SAXException(errorMsg)
			Try
				Return Convert.ToInt32(tok.nextToken())
			Catch nfe As NumberFormatException
				Throw New org.xml.sax.SAXException(errorMsg)
			End Try
		End Function

		''' <summary>
		''' Convenience method to return an Insets object.
		''' </summary>
		Private Function parseInsets(ByVal insets As String, ByVal errorMsg As String) As java.awt.Insets
			Dim tokenizer As New java.util.StringTokenizer(insets)
			Return New java.awt.Insets(nextInt(tokenizer, errorMsg), nextInt(tokenizer, errorMsg), nextInt(tokenizer, errorMsg), nextInt(tokenizer, errorMsg))
		End Function



		'
		' The following methods are invoked from startElement/stopElement
		'

		Private Sub startStyle(ByVal attributes As org.xml.sax.Attributes)
			Dim id As String = Nothing

			_style = Nothing
			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)
				If key.Equals(ATTRIBUTE_CLONE) Then
					_style = CType(CType(lookup(attributes.getValue(i), GetType(ParsedSynthStyle)), ParsedSynthStyle).clone(), ParsedSynthStyle)
				ElseIf key.Equals(ATTRIBUTE_ID) Then
					id = attributes.getValue(i)
				End If
			Next i
			If _style Is Nothing Then _style = New ParsedSynthStyle
			register(id, _style)
		End Sub

		Private Sub endStyle()
			Dim size As Integer = _stylePainters.Count
			If size > 0 Then
				_style.painters = _stylePainters.ToArray()
				_stylePainters.Clear()
			End If
			size = _stateInfos.Count
			If size > 0 Then
				_style.stateInfo = _stateInfos.ToArray()
				_stateInfos.Clear()
			End If
			_style = Nothing
		End Sub

		Private Sub startState(ByVal attributes As org.xml.sax.Attributes)
			Dim stateInfo As ParsedSynthStyle.StateInfo = Nothing
			Dim state As Integer = 0
			Dim id As String = Nothing

			_stateInfo = Nothing
			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)
				If key.Equals(ATTRIBUTE_ID) Then
					id = attributes.getValue(i)
				ElseIf key.Equals(ATTRIBUTE_IDREF) Then
					_stateInfo = CType(lookup(attributes.getValue(i), GetType(ParsedSynthStyle.StateInfo)), ParsedSynthStyle.StateInfo)
				ElseIf key.Equals(ATTRIBUTE_CLONE) Then
					_stateInfo = CType(CType(lookup(attributes.getValue(i), GetType(ParsedSynthStyle.StateInfo)), ParsedSynthStyle.StateInfo).clone(), ParsedSynthStyle.StateInfo)
				ElseIf key.Equals(ATTRIBUTE_VALUE) Then
					Dim tokenizer As New java.util.StringTokenizer(attributes.getValue(i))
					Do While tokenizer.hasMoreTokens()
						Dim stateString As String = tokenizer.nextToken().ToUpper().intern()
						If stateString = "ENABLED" Then
							state = state Or SynthConstants.ENABLED
						ElseIf stateString = "MOUSE_OVER" Then
							state = state Or SynthConstants.MOUSE_OVER
						ElseIf stateString = "PRESSED" Then
							state = state Or SynthConstants.PRESSED
						ElseIf stateString = "DISABLED" Then
							state = state Or SynthConstants.DISABLED
						ElseIf stateString = "FOCUSED" Then
							state = state Or SynthConstants.FOCUSED
						ElseIf stateString = "SELECTED" Then
							state = state Or SynthConstants.SELECTED
						ElseIf stateString = "DEFAULT" Then
							state = state Or SynthConstants.DEFAULT
						ElseIf stateString <> "AND" Then
							Throw New org.xml.sax.SAXException("Unknown state: " & state)
						End If
					Loop
				End If
			Next i
			If _stateInfo Is Nothing Then _stateInfo = New ParsedSynthStyle.StateInfo
			_stateInfo.componentState = state
			register(id, _stateInfo)
			_stateInfos.Add(_stateInfo)
		End Sub

		Private Sub endState()
			Dim size As Integer = _statePainters.Count
			If size > 0 Then
				_stateInfo.painters = _statePainters.ToArray()
				_statePainters.Clear()
			End If
			_stateInfo = Nothing
		End Sub

		Private Sub startFont(ByVal attributes As org.xml.sax.Attributes)
			Dim font As java.awt.Font = Nothing
			Dim style As Integer = java.awt.Font.PLAIN
			Dim size As Integer = 0
			Dim id As String = Nothing
			Dim name As String = Nothing

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)
				If key.Equals(ATTRIBUTE_ID) Then
					id = attributes.getValue(i)
				ElseIf key.Equals(ATTRIBUTE_IDREF) Then
					font = CType(lookup(attributes.getValue(i), GetType(java.awt.Font)), java.awt.Font)
				ElseIf key.Equals(ATTRIBUTE_NAME) Then
					name = attributes.getValue(i)
				ElseIf key.Equals(ATTRIBUTE_SIZE) Then
					Try
						size = Convert.ToInt32(attributes.getValue(i))
					Catch nfe As NumberFormatException
						Throw New org.xml.sax.SAXException("Invalid font size: " & attributes.getValue(i))
					End Try
				ElseIf key.Equals(ATTRIBUTE_STYLE) Then
					Dim tok As New java.util.StringTokenizer(attributes.getValue(i))
					Do While tok.hasMoreTokens()
						Dim token As String = tok.nextToken().intern()
						If token = "BOLD" Then
							style = ((style Or java.awt.Font.PLAIN) Xor java.awt.Font.PLAIN) Or java.awt.Font.BOLD
						ElseIf token = "ITALIC" Then
							style = style Or java.awt.Font.ITALIC
						End If
					Loop
				End If
			Next i
			If font Is Nothing Then
				If name Is Nothing Then Throw New org.xml.sax.SAXException("You must define a name for the font")
				If size = 0 Then Throw New org.xml.sax.SAXException("You must define a size for the font")
				font = New javax.swing.plaf.FontUIResource(name, style, size)
			ElseIf name IsNot Nothing OrElse size <> 0 OrElse style <> java.awt.Font.PLAIN Then
				Throw New org.xml.sax.SAXException("Name, size and style are not for use " & "with idref")
			End If
			register(id, font)
			If _stateInfo IsNot Nothing Then
				_stateInfo.font = font
			ElseIf _style IsNot Nothing Then
				_style.font = font
			End If
		End Sub

		Private Sub startColor(ByVal attributes As org.xml.sax.Attributes)
			Dim color As java.awt.Color = Nothing
			Dim id As String = Nothing

			_colorTypes.Clear()
			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)
				If key.Equals(ATTRIBUTE_ID) Then
					id = attributes.getValue(i)
				ElseIf key.Equals(ATTRIBUTE_IDREF) Then
					color = CType(lookup(attributes.getValue(i), GetType(java.awt.Color)), java.awt.Color)
				ElseIf key.Equals(ATTRIBUTE_NAME) Then
				ElseIf key.Equals(ATTRIBUTE_VALUE) Then
					Dim value As String = attributes.getValue(i)

					If value.StartsWith("#") Then
						Try
							Dim argb As Integer
							Dim hasAlpha As Boolean

							Dim length As Integer = value.Length
							If length < 8 Then
								' Just RGB, or some portion of it.
								argb = Integer.decode(value)
								hasAlpha = False
							ElseIf length = 8 Then
								' Single character alpha: #ARRGGBB.
								argb = Integer.decode(value)
								hasAlpha = True
							ElseIf length = 9 Then
								' Color has alpha and is of the form
								' #AARRGGBB.
								' The following split decoding is mandatory due to
								' Integer.decode() behavior which won't decode
								' hexadecimal values higher than #7FFFFFFF.
								' Thus, when an alpha channel is detected, it is
								' decoded separately from the RGB channels.
								Dim rgb As Integer = Integer.decode(AscW("#"c) + value.Substring(3, 6))
								Dim a As Integer = Integer.decode(value.Substring(0, 3))
								argb = (a << 24) Or rgb
								hasAlpha = True
							Else
								Throw New org.xml.sax.SAXException("Invalid Color value: " & value)
							End If

							color = New javax.swing.plaf.ColorUIResource(New java.awt.Color(argb, hasAlpha))
						Catch nfe As NumberFormatException
							Throw New org.xml.sax.SAXException("Invalid Color value: " & value)
						End Try
					Else
						Try
							color = New javax.swing.plaf.ColorUIResource(GetType(CType(java.awt.Color, java.awt.Color)).getField(value.ToUpper()).get(GetType(java.awt.Color)))
						Catch nsfe As NoSuchFieldException
							Throw New org.xml.sax.SAXException("Invalid color name: " & value)
						Catch iae As IllegalAccessException
							Throw New org.xml.sax.SAXException("Invalid color name: " & value)
						End Try
					End If
				ElseIf key.Equals(ATTRIBUTE_TYPE) Then
					Dim tokenizer As New java.util.StringTokenizer(attributes.getValue(i))
					Do While tokenizer.hasMoreTokens()
						Dim typeName As String = tokenizer.nextToken()
						Dim classIndex As Integer = typeName.LastIndexOf("."c)
						Dim typeClass As Type

						If classIndex = -1 Then
							typeClass = GetType(ColorType)
							classIndex = 0
						Else
							Try
								typeClass = sun.reflect.misc.ReflectUtil.forName(typeName.Substring(0, classIndex))
							Catch cnfe As ClassNotFoundException
								Throw New org.xml.sax.SAXException("Unknown class: " & typeName.Substring(0, classIndex))
							End Try
							classIndex += 1
						End If
						Try
							_colorTypes.Add(CType(checkCast(typeClass.GetField(typeName.Substring(classIndex)).get(typeClass), GetType(ColorType)), ColorType))
						Catch nsfe As NoSuchFieldException
							Throw New org.xml.sax.SAXException("Unable to find color type: " & typeName)
						Catch iae As IllegalAccessException
							Throw New org.xml.sax.SAXException("Unable to find color type: " & typeName)
						End Try
					Loop
				End If
			Next i
			If color Is Nothing Then Throw New org.xml.sax.SAXException("color: you must specificy a value")
			register(id, color)
			If _stateInfo IsNot Nothing AndAlso _colorTypes.Count > 0 Then
				Dim colors As java.awt.Color() = _stateInfo.colors
				Dim max As Integer = 0
				For counter As Integer = _colorTypes.Count - 1 To 0 Step -1
					max = Math.Max(max, _colorTypes(counter).iD)
				Next counter
				If colors Is Nothing OrElse colors.Length <= max Then
					Dim newColors As java.awt.Color() = New java.awt.Color(max){}
					If colors IsNot Nothing Then Array.Copy(colors, 0, newColors, 0, colors.Length)
					colors = newColors
				End If
				For counter As Integer = _colorTypes.Count - 1 To 0 Step -1
					colors(_colorTypes(counter).iD) = color
				Next counter
				_stateInfo.colors = colors
			End If
		End Sub

		Private Sub startProperty(ByVal attributes As org.xml.sax.Attributes, ByVal [property] As Object)
			Dim value As Object = Nothing
			Dim key As String = Nothing
			' Type of the value: 0=idref, 1=boolean, 2=dimension, 3=insets,
			' 4=integer,5=string
			Dim iType As Integer = 0
			Dim aValue As String = Nothing

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim aName As String = attributes.getQName(i)
				If aName.Equals(ATTRIBUTE_TYPE) Then
					Dim type As String = attributes.getValue(i).ToUpper()
					If type.Equals("IDREF") Then
						iType = 0
					ElseIf type.Equals("BOOLEAN") Then
						iType = 1
					ElseIf type.Equals("DIMENSION") Then
						iType = 2
					ElseIf type.Equals("INSETS") Then
						iType = 3
					ElseIf type.Equals("INTEGER") Then
						iType = 4
					ElseIf type.Equals("STRING") Then
						iType = 5
					Else
						Throw New org.xml.sax.SAXException([property] & " unknown type, use" & "idref, boolean, dimension, insets or integer")
					End If
				ElseIf aName.Equals(ATTRIBUTE_VALUE) Then
					aValue = attributes.getValue(i)
				ElseIf aName.Equals(ATTRIBUTE_KEY) Then
					key = attributes.getValue(i)
				End If
			Next i
			If aValue IsNot Nothing Then
				Select Case iType
				Case 0 ' idref
					value = lookup(aValue, GetType(Object))
				Case 1 ' boolean
					If aValue.ToUpper().Equals("TRUE") Then
						value = Boolean.TRUE
					Else
						value = Boolean.FALSE
					End If
				Case 2 ' dimension
					Dim tok As New java.util.StringTokenizer(aValue)
					value = New javax.swing.plaf.DimensionUIResource(nextInt(tok, "Invalid dimension"), nextInt(tok, "Invalid dimension"))
				Case 3 ' insets
					value = parseInsets(aValue, [property] & " invalid insets")
				Case 4 ' integer
					Try
						value = New Integer?(Convert.ToInt32(aValue))
					Catch nfe As NumberFormatException
						Throw New org.xml.sax.SAXException([property] & " invalid value")
					End Try
				Case 5 'string
					value = aValue
				End Select
			End If
			If value Is Nothing OrElse key Is Nothing Then Throw New org.xml.sax.SAXException([property] & ": you must supply a " & "key and value")
			If [property] Is ELEMENT_DEFAULTS_PROPERTY Then
				_defaultsMap(key) = value
			ElseIf _stateInfo IsNot Nothing Then
				If _stateInfo.data Is Nothing Then _stateInfo.data = New Hashtable
				_stateInfo.data.put(key, value)
			ElseIf _style IsNot Nothing Then
				If _style.data Is Nothing Then _style.data = New Hashtable
				_style.data.put(key, value)
			End If
		End Sub

		Private Sub startGraphics(ByVal attributes As org.xml.sax.Attributes)
			Dim graphics As SynthGraphicsUtils = Nothing

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)
				If key.Equals(ATTRIBUTE_IDREF) Then graphics = CType(lookup(attributes.getValue(i), GetType(SynthGraphicsUtils)), SynthGraphicsUtils)
			Next i
			If graphics Is Nothing Then Throw New org.xml.sax.SAXException("graphicsUtils: you must supply an idref")
			If _style IsNot Nothing Then _style.graphicsUtils = graphics
		End Sub

		Private Sub startInsets(ByVal attributes As org.xml.sax.Attributes)
			Dim top As Integer = 0
			Dim bottom As Integer = 0
			Dim left As Integer = 0
			Dim right As Integer = 0
			Dim insets As java.awt.Insets = Nothing
			Dim id As String = Nothing

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)

				Try
					If key.Equals(ATTRIBUTE_IDREF) Then
						insets = CType(lookup(attributes.getValue(i), GetType(java.awt.Insets)), java.awt.Insets)
					ElseIf key.Equals(ATTRIBUTE_ID) Then
						id = attributes.getValue(i)
					ElseIf key.Equals(ATTRIBUTE_TOP) Then
						top = Convert.ToInt32(attributes.getValue(i))
					ElseIf key.Equals(ATTRIBUTE_LEFT) Then
						left = Convert.ToInt32(attributes.getValue(i))
					ElseIf key.Equals(ATTRIBUTE_BOTTOM) Then
						bottom = Convert.ToInt32(attributes.getValue(i))
					ElseIf key.Equals(ATTRIBUTE_RIGHT) Then
						right = Convert.ToInt32(attributes.getValue(i))
					End If
				Catch nfe As NumberFormatException
					Throw New org.xml.sax.SAXException("insets: bad integer value for " & attributes.getValue(i))
				End Try
			Next i
			If insets Is Nothing Then insets = New javax.swing.plaf.InsetsUIResource(top, left, bottom, right)
			register(id, insets)
			If _style IsNot Nothing Then _style.insets = insets
		End Sub

		Private Sub startBind(ByVal attributes As org.xml.sax.Attributes)
			Dim style As ParsedSynthStyle = Nothing
			Dim path As String = Nothing
			Dim type As Integer = -1

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)

				If key.Equals(ATTRIBUTE_STYLE) Then
					style = CType(lookup(attributes.getValue(i), GetType(ParsedSynthStyle)), ParsedSynthStyle)
				ElseIf key.Equals(ATTRIBUTE_TYPE) Then
					Dim typeS As String = attributes.getValue(i).ToUpper()

					If typeS.Equals("NAME") Then
						type = DefaultSynthStyleFactory.NAME
					ElseIf typeS.Equals("REGION") Then
						type = DefaultSynthStyleFactory.REGION
					Else
						Throw New org.xml.sax.SAXException("bind: unknown type " & typeS)
					End If
				ElseIf key.Equals(ATTRIBUTE_KEY) Then
					path = attributes.getValue(i)
				End If
			Next i
			If style Is Nothing OrElse path Is Nothing OrElse type = -1 Then Throw New org.xml.sax.SAXException("bind: you must specify a style, type " & "and key")
			Try
				_factory.addStyle(style, path, type)
			Catch pse As java.util.regex.PatternSyntaxException
				Throw New org.xml.sax.SAXException("bind: " & path & " is not a valid " & "regular expression")
			End Try
		End Sub

		Private Sub startPainter(ByVal attributes As org.xml.sax.Attributes, ByVal type As String)
			Dim sourceInsets As java.awt.Insets = Nothing
			Dim destInsets As java.awt.Insets = Nothing
			Dim path As String = Nothing
			Dim paintCenter As Boolean = True
			Dim stretch As Boolean = True
			Dim painter As SynthPainter = Nothing
			Dim method As String = Nothing
			Dim id As String = Nothing
			Dim direction As Integer = -1
			Dim center As Boolean = False

			Dim stretchSpecified As Boolean = False
			Dim paintCenterSpecified As Boolean = False

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)
				Dim value As String = attributes.getValue(i)

				If key.Equals(ATTRIBUTE_ID) Then
					id = value
				ElseIf key.Equals(ATTRIBUTE_METHOD) Then
					method = value.ToLower(java.util.Locale.ENGLISH)
				ElseIf key.Equals(ATTRIBUTE_IDREF) Then
					painter = CType(lookup(value, GetType(SynthPainter)), SynthPainter)
				ElseIf key.Equals(ATTRIBUTE_PATH) Then
					path = value
				ElseIf key.Equals(ATTRIBUTE_SOURCE_INSETS) Then
					sourceInsets = parseInsets(value, type & ": sourceInsets must be top left bottom right")
				ElseIf key.Equals(ATTRIBUTE_DEST_INSETS) Then
					destInsets = parseInsets(value, type & ": destinationInsets must be top left bottom right")
				ElseIf key.Equals(ATTRIBUTE_PAINT_CENTER) Then
					paintCenter = value.ToLower().Equals("true")
					paintCenterSpecified = True
				ElseIf key.Equals(ATTRIBUTE_STRETCH) Then
					stretch = value.ToLower().Equals("true")
					stretchSpecified = True
				ElseIf key.Equals(ATTRIBUTE_DIRECTION) Then
					value = value.ToUpper().intern()
					If value = "EAST" Then
						direction = javax.swing.SwingConstants.EAST
					ElseIf value = "NORTH" Then
						direction = javax.swing.SwingConstants.NORTH
					ElseIf value = "SOUTH" Then
						direction = javax.swing.SwingConstants.SOUTH
					ElseIf value = "WEST" Then
						direction = javax.swing.SwingConstants.WEST
					ElseIf value = "TOP" Then
						direction = javax.swing.SwingConstants.TOP
					ElseIf value = "LEFT" Then
						direction = javax.swing.SwingConstants.LEFT
					ElseIf value = "BOTTOM" Then
						direction = javax.swing.SwingConstants.BOTTOM
					ElseIf value = "RIGHT" Then
						direction = javax.swing.SwingConstants.RIGHT
					ElseIf value = "HORIZONTAL" Then
						direction = javax.swing.SwingConstants.HORIZONTAL
					ElseIf value = "VERTICAL" Then
						direction = javax.swing.SwingConstants.VERTICAL
					ElseIf value = "HORIZONTAL_SPLIT" Then
						direction = javax.swing.JSplitPane.HORIZONTAL_SPLIT
					ElseIf value = "VERTICAL_SPLIT" Then
						direction = javax.swing.JSplitPane.VERTICAL_SPLIT
					Else
						Throw New org.xml.sax.SAXException(type & ": unknown direction")
					End If
				ElseIf key.Equals(ATTRIBUTE_CENTER) Then
					center = value.ToLower().Equals("true")
				End If
			Next i
			If painter Is Nothing Then
				If type = ELEMENT_PAINTER Then Throw New org.xml.sax.SAXException(type & ": you must specify an idref")
				If sourceInsets Is Nothing AndAlso (Not center) Then Throw New org.xml.sax.SAXException("property: you must specify sourceInsets")
				If path Is Nothing Then Throw New org.xml.sax.SAXException("property: you must specify a path")
				If center AndAlso (sourceInsets IsNot Nothing OrElse destInsets IsNot Nothing OrElse paintCenterSpecified OrElse stretchSpecified) Then Throw New org.xml.sax.SAXException("The attributes: sourceInsets, " & "destinationInsets, paintCenter and stretch " & " are not legal when center is true")
				painter = New ImagePainter((Not stretch), paintCenter, sourceInsets, destInsets, getResource(path), center)
			End If
			register(id, painter)
			If _stateInfo IsNot Nothing Then
				addPainterOrMerge(_statePainters, method, painter, direction)
			ElseIf _style IsNot Nothing Then
				addPainterOrMerge(_stylePainters, method, painter, direction)
			End If
		End Sub

		Private Sub addPainterOrMerge(ByVal painters As IList(Of ParsedSynthStyle.PainterInfo), ByVal method As String, ByVal painter As SynthPainter, ByVal direction As Integer)
			Dim painterInfo As ParsedSynthStyle.PainterInfo
			painterInfo = New ParsedSynthStyle.PainterInfo(method, painter, direction)

			For Each infoObject As Object In painters
				Dim info As ParsedSynthStyle.PainterInfo
				info = CType(infoObject, ParsedSynthStyle.PainterInfo)

				If painterInfo.equalsPainter(info) Then
					info.addPainter(painter)
					Return
				End If
			Next infoObject

			painters.Add(painterInfo)
		End Sub

		Private Sub startImageIcon(ByVal attributes As org.xml.sax.Attributes)
			Dim path As String = Nothing
			Dim id As String = Nothing

			For i As Integer = attributes.length - 1 To 0 Step -1
				Dim key As String = attributes.getQName(i)

				If key.Equals(ATTRIBUTE_ID) Then
					id = attributes.getValue(i)
				ElseIf key.Equals(ATTRIBUTE_PATH) Then
					path = attributes.getValue(i)
				End If
			Next i
			If path Is Nothing Then Throw New org.xml.sax.SAXException("imageIcon: you must specify a path")
			register(id, New LazyImageIcon(getResource(path)))
		End Sub

		Private Sub startOpaque(ByVal attributes As org.xml.sax.Attributes)
			If _style IsNot Nothing Then
				_style.opaque = True
				For i As Integer = attributes.length - 1 To 0 Step -1
					Dim key As String = attributes.getQName(i)

					If key.Equals(ATTRIBUTE_VALUE) Then _style.opaque = "true".Equals(attributes.getValue(i).ToLower())
				Next i
			End If
		End Sub

		Private Sub startInputMap(ByVal attributes As org.xml.sax.Attributes)
			_inputMapBindings.Clear()
			_inputMapID = Nothing
			If _style IsNot Nothing Then
				For i As Integer = attributes.length - 1 To 0 Step -1
					Dim key As String = attributes.getQName(i)

					If key.Equals(ATTRIBUTE_ID) Then _inputMapID = attributes.getValue(i)
				Next i
			End If
		End Sub

		Private Sub endInputMap()
			If _inputMapID IsNot Nothing Then register(_inputMapID, New javax.swing.UIDefaults.LazyInputMap(_inputMapBindings.ToArray()))
			_inputMapBindings.Clear()
			_inputMapID = Nothing
		End Sub

		Private Sub startBindKey(ByVal attributes As org.xml.sax.Attributes)
			If _inputMapID Is Nothing Then Return
			If _style IsNot Nothing Then
				Dim key As String = Nothing
				Dim value As String = Nothing
				For i As Integer = attributes.length - 1 To 0 Step -1
					Dim aKey As String = attributes.getQName(i)

					If aKey.Equals(ATTRIBUTE_KEY) Then
						key = attributes.getValue(i)
					ElseIf aKey.Equals(ATTRIBUTE_ACTION) Then
						value = attributes.getValue(i)
					End If
				Next i
				If key Is Nothing OrElse value Is Nothing Then Throw New org.xml.sax.SAXException("bindKey: you must supply a key and action")
				_inputMapBindings.Add(key)
				_inputMapBindings.Add(value)
			End If
		End Sub

		'
		' SAX methods, these forward to the DocumentHandler if we don't know
		' the element name.
		'

		Public Overridable Function resolveEntity(ByVal publicId As String, ByVal systemId As String) As org.xml.sax.InputSource
			If forwarding Then Return handler.resolveEntity(publicId, systemId)
			Return Nothing
		End Function

		Public Overridable Sub notationDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String)
			If forwarding Then handler.notationDecl(name, publicId, systemId)
		End Sub

		Public Overridable Sub unparsedEntityDecl(ByVal name As String, ByVal publicId As String, ByVal systemId As String, ByVal notationName As String)
			If forwarding Then handler.unparsedEntityDecl(name, publicId, systemId, notationName)
		End Sub

		Public Overridable Property documentLocator As org.xml.sax.Locator
			Set(ByVal locator As org.xml.sax.Locator)
				If forwarding Then handler.documentLocator = locator
			End Set
		End Property

		Public Overridable Sub startDocument()
			If forwarding Then handler.startDocument()
		End Sub

		Public Overridable Sub endDocument()
			If forwarding Then handler.endDocument()
		End Sub

		Public Overridable Sub startElement(ByVal uri As String, ByVal local As String, ByVal name As String, ByVal attributes As org.xml.sax.Attributes)
			name = name.intern()
			If name = ELEMENT_STYLE Then
				startStyle(attributes)
			ElseIf name = ELEMENT_STATE Then
				startState(attributes)
			ElseIf name = ELEMENT_FONT Then
				startFont(attributes)
			ElseIf name = ELEMENT_COLOR Then
				startColor(attributes)
			ElseIf name = ELEMENT_PAINTER Then
				startPainter(attributes, name)
			ElseIf name = ELEMENT_IMAGE_PAINTER Then
				startPainter(attributes, name)
			ElseIf name = ELEMENT_PROPERTY Then
				startProperty(attributes, ELEMENT_PROPERTY)
			ElseIf name = ELEMENT_DEFAULTS_PROPERTY Then
				startProperty(attributes, ELEMENT_DEFAULTS_PROPERTY)
			ElseIf name = ELEMENT_SYNTH_GRAPHICS Then
				startGraphics(attributes)
			ElseIf name = ELEMENT_INSETS Then
				startInsets(attributes)
			ElseIf name = ELEMENT_BIND Then
				startBind(attributes)
			ElseIf name = ELEMENT_BIND_KEY Then
				startBindKey(attributes)
			ElseIf name = ELEMENT_IMAGE_ICON Then
				startImageIcon(attributes)
			ElseIf name = ELEMENT_OPAQUE Then
				startOpaque(attributes)
			ElseIf name = ELEMENT_INPUT_MAP Then
				startInputMap(attributes)
			ElseIf name <> ELEMENT_SYNTH Then
				Dim tempVar As Boolean = _depth = 0
				_depth += 1
				If tempVar Then handler.startDocument()
				handler.startElement(uri, local, name, attributes)
			End If
		End Sub

		Public Overridable Sub endElement(ByVal uri As String, ByVal local As String, ByVal name As String)
			If forwarding Then
				handler.endElement(uri, local, name)
				_depth -= 1
				If Not forwarding Then handler.startDocument()
			Else
				name = name.intern()
				If name = ELEMENT_STYLE Then
					endStyle()
				ElseIf name = ELEMENT_STATE Then
					endState()
				ElseIf name = ELEMENT_INPUT_MAP Then
					endInputMap()
				End If
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void characters(char ch() , int start, int length) throws org.xml.sax.SAXException
			If forwarding Then handler.characters(ch, start, length)

		public void ignorableWhitespace(Char ch() , Integer start, Integer length) throws org.xml.sax.SAXException
			If forwarding Then handler.ignorableWhitespace(ch, start, length)

		public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
			If forwarding Then handler.processingInstruction(target, data)

		public void warning(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			If forwarding Then handler.warning(e)

		public void error(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			If forwarding Then handler.error(e)


		public void fatalError(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
			If forwarding Then handler.fatalError(e)
			Throw e


		''' <summary>
		''' ImageIcon that lazily loads the image until needed.
		''' </summary>
		private static class LazyImageIcon extends javax.swing.ImageIcon implements javax.swing.plaf.UIResource
			private java.net.URL location

			public LazyImageIcon(java.net.URL location)
				MyBase()
				Me.location = location

			public void paintIcon(java.awt.Component c, java.awt.Graphics g, Integer x, Integer y)
				If image IsNot Nothing Then MyBase.paintIcon(c, g, x, y)

			public Integer iconWidth
				If image IsNot Nothing Then Return MyBase.iconWidth
				Return 0

			public Integer iconHeight
				If image IsNot Nothing Then Return MyBase.iconHeight
				Return 0

			public java.awt.Image image
				If location IsNot Nothing Then
					image = java.awt.Toolkit.defaultToolkit.getImage(location)
					location = Nothing
				End If
				Return MyBase.image
	End Class

End Namespace
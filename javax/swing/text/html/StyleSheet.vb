Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing.border
Imports javax.swing.text

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html

	''' <summary>
	''' Support for defining the visual characteristics of
	''' HTML views being rendered.  The StyleSheet is used to
	''' translate the HTML model into visual characteristics.
	''' This enables views to be customized by a look-and-feel,
	''' multiple views over the same model can be rendered
	''' differently, etc.  This can be thought of as a CSS
	''' rule repository.  The key for CSS attributes is an
	''' object of type CSS.Attribute.  The type of the value
	''' is up to the StyleSheet implementation, but the
	''' <code>toString</code> method is required
	''' to return a string representation of CSS value.
	''' <p>
	''' The primary entry point for HTML View implementations
	''' to get their attributes is the
	''' <seealso cref="#getViewAttributes getViewAttributes"/>
	''' method.  This should be implemented to establish the
	''' desired policy used to associate attributes with the view.
	''' Each HTMLEditorKit (i.e. and therefore each associated
	''' JEditorPane) can have its own StyleSheet, but by default one
	''' sheet will be shared by all of the HTMLEditorKit instances.
	''' HTMLDocument instance can also have a StyleSheet, which
	''' holds the document-specific CSS specifications.
	''' <p>
	''' In order for Views to store less state and therefore be
	''' more lightweight, the StyleSheet can act as a factory for
	''' painters that handle some of the rendering tasks.  This allows
	''' implementations to determine what they want to cache
	''' and have the sharing potentially at the level that a
	''' selector is common to multiple views.  Since the StyleSheet
	''' may be used by views over multiple documents and typically
	''' the HTML attributes don't effect the selector being used,
	''' the potential for sharing is significant.
	''' <p>
	''' The rules are stored as named styles, and other information
	''' is stored to translate the context of an element to a
	''' rule quickly.  The following code fragment will display
	''' the named styles, and therefore the CSS rules contained.
	''' <pre><code>
	''' &nbsp;
	''' &nbsp; import java.util.*;
	''' &nbsp; import javax.swing.text.*;
	''' &nbsp; import javax.swing.text.html.*;
	''' &nbsp;
	''' &nbsp; public class ShowStyles {
	''' &nbsp;
	''' &nbsp;     public static void main(String[] args) {
	''' &nbsp;       HTMLEditorKit kit = new HTMLEditorKit();
	''' &nbsp;       HTMLDocument doc = (HTMLDocument) kit.createDefaultDocument();
	''' &nbsp;       StyleSheet styles = doc.getStyleSheet();
	''' &nbsp;
	''' &nbsp;       Enumeration rules = styles.getStyleNames();
	''' &nbsp;       while (rules.hasMoreElements()) {
	''' &nbsp;           String name = (String) rules.nextElement();
	''' &nbsp;           Style rule = styles.getStyle(name);
	''' &nbsp;           System.out.println(rule.toString());
	''' &nbsp;       }
	''' &nbsp;       System.exit(0);
	''' &nbsp;     }
	''' &nbsp; }
	''' &nbsp;
	''' </code></pre>
	''' <p>
	''' The semantics for when a CSS style should overide visual attributes
	''' defined by an element are not well defined. For example, the html
	''' <code>&lt;body bgcolor=red&gt;</code> makes the body have a red
	''' background. But if the html file also contains the CSS rule
	''' <code>body { background: blue }</code> it becomes less clear as to
	''' what color the background of the body should be. The current
	''' implementation gives visual attributes defined in the element the
	''' highest precedence, that is they are always checked before any styles.
	''' Therefore, in the previous example the background would have a
	''' red color as the body element defines the background color to be red.
	''' <p>
	''' As already mentioned this supports CSS. We don't support the full CSS
	''' spec. Refer to the javadoc of the CSS class to see what properties
	''' we support. The two major CSS parsing related
	''' concepts we do not currently
	''' support are pseudo selectors, such as <code>A:link { color: red }</code>,
	''' and the <code>important</code> modifier.
	''' <p>
	''' <font color="red">Note: This implementation is currently
	''' incomplete.  It can be replaced with alternative implementations
	''' that are complete.  Future versions of this class will provide
	''' better CSS support.</font>
	''' 
	''' @author  Timothy Prinzing
	''' @author  Sunita Mani
	''' @author  Sara Swanson
	''' @author  Jill Nakata
	''' </summary>
	Public Class StyleSheet
		Inherits StyleContext

		' As the javadoc states, this class maintains a mapping between
		' a CSS selector (such as p.bar) and a Style.
		' This consists of a number of parts:
		' . Each selector is broken down into its constituent simple selectors,
		'   and stored in an inverted graph, for example:
		'     p { color: red } ol p { font-size: 10pt } ul p { font-size: 12pt }
		'   results in the graph:
		'          root
		'           |
		'           p
		'          / \
		'         ol ul
		'   each node (an instance of SelectorMapping) has an associated
		'   specificity and potentially a Style.
		' . Every rule that is asked for (either by way of getRule(String) or
		'   getRule(HTML.Tag, Element)) results in a unique instance of
		'   ResolvedStyle. ResolvedStyles contain the AttributeSets from the
		'   SelectorMapping.
		' . When a new rule is created it is inserted into the graph, and
		'   the AttributeSets of each ResolvedStyles are updated appropriately.
		' . This class creates special AttributeSets, LargeConversionSet and
		'   SmallConversionSet, that maintain a mapping between StyleConstants
		'   and CSS so that developers that wish to use the StyleConstants
		'   methods can do so.
		' . When one of the AttributeSets is mutated by way of a
		'   StyleConstants key, all the associated CSS keys are removed. This is
		'   done so that the two representations don't get out of sync. For
		'   example, if the developer adds StyleConsants.BOLD, FALSE to an
		'   AttributeSet that contains HTML.Tag.B, the HTML.Tag.B entry will
		'   be removed.

		''' <summary>
		''' Construct a StyleSheet
		''' </summary>
		Public Sub New()
			MyBase.New()
			selectorMapping = New SelectorMapping(0)
			resolvedStyles = New Dictionary(Of String, ResolvedStyle)
			If css Is Nothing Then css = New CSS
		End Sub

		''' <summary>
		''' Fetches the style to use to render the given type
		''' of HTML tag.  The element given is representing
		''' the tag and can be used to determine the nesting
		''' for situations where the attributes will differ
		''' if nesting inside of elements.
		''' </summary>
		''' <param name="t"> the type to translate to visual attributes </param>
		''' <param name="e"> the element representing the tag; the element
		'''  can be used to determine the nesting for situations where
		'''  the attributes will differ if nested inside of other
		'''  elements </param>
		''' <returns> the set of CSS attributes to use to render
		'''  the tag </returns>
		Public Overridable Function getRule(ByVal t As HTML.Tag, ByVal e As Element) As Style
			Dim sb As SearchBuffer = SearchBuffer.obtainSearchBuffer()

			Try
				' Build an array of all the parent elements.
				Dim searchContext As List(Of Element) = sb.vector

				Dim p As Element = e
				Do While p IsNot Nothing
					searchContext.Add(p)
					p = p.parentElement
				Loop

				' Build a fully qualified selector.
				Dim n As Integer = searchContext.Count
				Dim cacheLookup As StringBuilder = sb.stringBuffer
				Dim attr As AttributeSet
				Dim eName As String
				Dim name As Object

				' >= 1 as the HTML.Tag for the 0th element is passed in.
				For counter As Integer = n - 1 To 1 Step -1
					e = searchContext(counter)
					attr = e.attributes
					name = attr.getAttribute(StyleConstants.NameAttribute)
					eName = name.ToString()
					cacheLookup.Append(eName)
					If attr IsNot Nothing Then
						If attr.isDefined(HTML.Attribute.ID) Then
							cacheLookup.Append("#"c)
							cacheLookup.Append(attr.getAttribute(HTML.Attribute.ID))
						ElseIf attr.isDefined(HTML.Attribute.CLASS) Then
							cacheLookup.Append("."c)
							cacheLookup.Append(attr.getAttribute(HTML.Attribute.CLASS))
						End If
					End If
					cacheLookup.Append(" "c)
				Next counter
				cacheLookup.Append(t.ToString())
				e = searchContext(0)
				attr = e.attributes
				If e.leaf Then
					' For leafs, we use the second tier attributes.
					Dim testAttr As Object = attr.getAttribute(t)
					If TypeOf testAttr Is AttributeSet Then
						attr = CType(testAttr, AttributeSet)
					Else
						attr = Nothing
					End If
				End If
				If attr IsNot Nothing Then
					If attr.isDefined(HTML.Attribute.ID) Then
						cacheLookup.Append("#"c)
						cacheLookup.Append(attr.getAttribute(HTML.Attribute.ID))
					ElseIf attr.isDefined(HTML.Attribute.CLASS) Then
						cacheLookup.Append("."c)
						cacheLookup.Append(attr.getAttribute(HTML.Attribute.CLASS))
					End If
				End If

				Dim ___style As Style = getResolvedStyle(cacheLookup.ToString(), searchContext, t)
				Return ___style
			Finally
				SearchBuffer.releaseSearchBuffer(sb)
			End Try
		End Function

		''' <summary>
		''' Fetches the rule that best matches the selector given
		''' in string form. Where <code>selector</code> is a space separated
		''' String of the element names. For example, <code>selector</code>
		''' might be 'html body tr td''<p>
		''' The attributes of the returned Style will change
		''' as rules are added and removed. That is if you to ask for a rule
		''' with a selector "table p" and a new rule was added with a selector
		''' of "p" the returned Style would include the new attributes from
		''' the rule "p".
		''' </summary>
		Public Overridable Function getRule(ByVal selector As String) As Style
			selector = cleanSelectorString(selector)
			If selector IsNot Nothing Then
				Dim ___style As Style = getResolvedStyle(selector)
				Return ___style
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Adds a set of rules to the sheet.  The rules are expected to
		''' be in valid CSS format.  Typically this would be called as
		''' a result of parsing a &lt;style&gt; tag.
		''' </summary>
		Public Overridable Sub addRule(ByVal rule As String)
			If rule IsNot Nothing Then
				'tweaks to control display properties
				'see BasicEditorPaneUI
				Const baseUnitsDisable As String = "BASE_SIZE_DISABLE"
				Const baseUnits As String = "BASE_SIZE "
				Const w3cLengthUnitsEnable As String = "W3C_LENGTH_UNITS_ENABLE"
				Const w3cLengthUnitsDisable As String = "W3C_LENGTH_UNITS_DISABLE"
				If rule = baseUnitsDisable Then
					sizeMap = sizeMapDefault
				ElseIf rule.StartsWith(baseUnits) Then
					rebaseSizeMap(Convert.ToInt32(rule.Substring(baseUnits.Length)))
				ElseIf rule = w3cLengthUnitsEnable Then
					w3cLengthUnits = True
				ElseIf rule = w3cLengthUnitsDisable Then
					w3cLengthUnits = False
				Else
					Dim parser As New CssParser(Me)
					Try
						parser.parse(base, New StringReader(rule), False, False)
					Catch ioe As IOException
					End Try
				End If
			End If
		End Sub

		''' <summary>
		''' Translates a CSS declaration to an AttributeSet that represents
		''' the CSS declaration.  Typically this would be called as a
		''' result of encountering an HTML style attribute.
		''' </summary>
		Public Overridable Function getDeclaration(ByVal decl As String) As AttributeSet
			If decl Is Nothing Then Return SimpleAttributeSet.EMPTY
			Dim parser As New CssParser(Me)
			Return parser.parseDeclaration(decl)
		End Function

		''' <summary>
		''' Loads a set of rules that have been specified in terms of
		''' CSS1 grammar.  If there are collisions with existing rules,
		''' the newly specified rule will win.
		''' </summary>
		''' <param name="in"> the stream to read the CSS grammar from </param>
		''' <param name="ref"> the reference URL.  This value represents the
		'''  location of the stream and may be null.  All relative
		'''  URLs specified in the stream will be based upon this
		'''  parameter. </param>
		Public Overridable Sub loadRules(ByVal [in] As Reader, ByVal ref As URL)
			Dim parser As New CssParser(Me)
			parser.parse(ref, [in], False, False)
		End Sub

		''' <summary>
		''' Fetches a set of attributes to use in the view for
		''' displaying.  This is basically a set of attributes that
		''' can be used for View.getAttributes.
		''' </summary>
		Public Overridable Function getViewAttributes(ByVal v As View) As AttributeSet
			Return New ViewAttributeSet(Me, v)
		End Function

		''' <summary>
		''' Removes a named style previously added to the document.
		''' </summary>
		''' <param name="nm">  the name of the style to remove </param>
		Public Overrides Sub removeStyle(ByVal nm As String)
			Dim aStyle As Style = getStyle(nm)

			If aStyle IsNot Nothing Then
				Dim selector As String = cleanSelectorString(nm)
				Dim selectors As String() = getSimpleSelectors(selector)
				SyncLock Me
					Dim mapping As SelectorMapping = rootSelectorMapping
					For i As Integer = selectors.Length - 1 To 0 Step -1
						mapping = mapping.getChildSelectorMapping(selectors(i), True)
					Next i
					Dim ___rule As Style = mapping.style
					If ___rule IsNot Nothing Then
						mapping.style = Nothing
						If resolvedStyles.Count > 0 Then
							Dim values As System.Collections.IEnumerator(Of ResolvedStyle) = resolvedStyles.Values.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Do While values.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								Dim ___style As ResolvedStyle = values.nextElement()
								___style.removeStyle(___rule)
							Loop
						End If
					End If
				End SyncLock
			End If
			MyBase.removeStyle(nm)
		End Sub

		''' <summary>
		''' Adds the rules from the StyleSheet <code>ss</code> to those of
		''' the receiver. <code>ss's</code> rules will override the rules of
		''' any previously added style sheets. An added StyleSheet will never
		''' override the rules of the receiving style sheet.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Sub addStyleSheet(ByVal ss As StyleSheet)
			SyncLock Me
				If linkedStyleSheets Is Nothing Then linkedStyleSheets = New List(Of StyleSheet)
				If Not linkedStyleSheets.Contains(ss) Then
					Dim index As Integer = 0
					If TypeOf ss Is javax.swing.plaf.UIResource AndAlso linkedStyleSheets.Count > 1 Then index = linkedStyleSheets.Count - 1
					linkedStyleSheets.Insert(index, ss)
					linkStyleSheetAt(ss, index)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the StyleSheet <code>ss</code> from those of the receiver.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Sub removeStyleSheet(ByVal ss As StyleSheet)
			SyncLock Me
				If linkedStyleSheets IsNot Nothing Then
					Dim index As Integer = linkedStyleSheets.IndexOf(ss)
					If index <> -1 Then
						linkedStyleSheets.RemoveAt(index)
						unlinkStyleSheet(ss, index)
						If index = 0 AndAlso linkedStyleSheets.Count = 0 Then linkedStyleSheets = Nothing
					End If
				End If
			End SyncLock
		End Sub

		'
		' The following is used to import style sheets.
		'

		''' <summary>
		''' Returns an array of the linked StyleSheets. Will return null
		''' if there are no linked StyleSheets.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property styleSheets As StyleSheet()
			Get
				Dim retValue As StyleSheet()
    
				SyncLock Me
					If linkedStyleSheets IsNot Nothing Then
						retValue = New StyleSheet(linkedStyleSheets.Count - 1){}
						linkedStyleSheets.CopyTo(retValue)
					Else
						retValue = Nothing
					End If
				End SyncLock
				Return retValue
			End Get
		End Property

		''' <summary>
		''' Imports a style sheet from <code>url</code>. The resulting rules
		''' are directly added to the receiver. If you do not want the rules
		''' to become part of the receiver, create a new StyleSheet and use
		''' addStyleSheet to link it in.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Sub importStyleSheet(ByVal url As URL)
			Try
				Dim [is] As InputStream

				[is] = url.openStream()
				Dim r As Reader = New BufferedReader(New InputStreamReader([is]))
				Dim parser As New CssParser(Me)
				parser.parse(url, r, False, True)
				r.close()
				[is].close()
			Catch e As Exception
				' on error we simply have no styles... the html
				' will look mighty wrong but still function.
			End Try
		End Sub

		''' <summary>
		''' Sets the base. All import statements that are relative, will be
		''' relative to <code>base</code>.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property base As URL
			Set(ByVal base As URL)
				Me.base = base
			End Set
			Get
				Return base
			End Get
		End Property


		''' <summary>
		''' Adds a CSS attribute to the given set.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Sub addCSSAttribute(ByVal attr As MutableAttributeSet, ByVal key As CSS.Attribute, ByVal value As String)
			css.addInternalCSSValue(attr, key, value)
		End Sub

		''' <summary>
		''' Adds a CSS attribute to the given set.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Function addCSSAttributeFromHTML(ByVal attr As MutableAttributeSet, ByVal key As CSS.Attribute, ByVal value As String) As Boolean
			Dim iValue As Object = css.getCssValue(key, value)
			If iValue IsNot Nothing Then
				attr.addAttribute(key, iValue)
				Return True
			End If
			Return False
		End Function

		' ---- Conversion functionality ---------------------------------

		''' <summary>
		''' Converts a set of HTML attributes to an equivalent
		''' set of CSS attributes.
		''' </summary>
		''' <param name="htmlAttrSet"> AttributeSet containing the HTML attributes. </param>
		Public Overridable Function translateHTMLToCSS(ByVal htmlAttrSet As AttributeSet) As AttributeSet
			Dim cssAttrSet As AttributeSet = css.translateHTMLToCSS(htmlAttrSet)

			Dim cssStyleSet As MutableAttributeSet = addStyle(Nothing, Nothing)
			cssStyleSet.addAttributes(cssAttrSet)

			Return cssStyleSet
		End Function

		''' <summary>
		''' Adds an attribute to the given set, and returns
		''' the new representative set.  This is reimplemented to
		''' convert StyleConstant attributes to CSS prior to forwarding
		''' to the superclass behavior.  The StyleConstants attribute
		''' has no corresponding CSS entry, the StyleConstants attribute
		''' is stored (but will likely be unused).
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="key"> the non-null attribute key </param>
		''' <param name="value"> the attribute value </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
		Public Overrides Function addAttribute(ByVal old As AttributeSet, ByVal key As Object, ByVal value As Object) As AttributeSet
			If css Is Nothing Then css = New CSS
			If TypeOf key Is StyleConstants Then
				Dim tag As HTML.Tag = HTML.getTagForStyleConstantsKey(CType(key, StyleConstants))

				If tag IsNot Nothing AndAlso old.isDefined(tag) Then old = removeAttribute(old, tag)

				Dim cssValue As Object = css.styleConstantsValueToCSSValue(CType(key, StyleConstants), value)
				If cssValue IsNot Nothing Then
					Dim cssKey As Object = css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then Return MyBase.addAttribute(old, cssKey, cssValue)
				End If
			End If
			Return MyBase.addAttribute(old, key, value)
		End Function

		''' <summary>
		''' Adds a set of attributes to the element.  If any of these attributes
		''' are StyleConstants attributes, they will be converted to CSS prior
		''' to forwarding to the superclass behavior.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="attr"> the attributes to add </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
		Public Overrides Function addAttributes(ByVal old As AttributeSet, ByVal attr As AttributeSet) As AttributeSet
			If Not(TypeOf attr Is HTMLDocument.TaggedAttributeSet) Then old = removeHTMLTags(old, attr)
			Return MyBase.addAttributes(old, convertAttributeSet(attr))
		End Function

		''' <summary>
		''' Removes an attribute from the set.  If the attribute is a StyleConstants
		''' attribute, the request will be converted to a CSS attribute prior to
		''' forwarding to the superclass behavior.
		''' </summary>
		''' <param name="old"> the old set of attributes </param>
		''' <param name="key"> the non-null attribute name </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#removeAttribute </seealso>
		Public Overrides Function removeAttribute(ByVal old As AttributeSet, ByVal key As Object) As AttributeSet
			If TypeOf key Is StyleConstants Then
				Dim tag As HTML.Tag = HTML.getTagForStyleConstantsKey(CType(key, StyleConstants))
				If tag IsNot Nothing Then old = MyBase.removeAttribute(old, tag)

				Dim cssKey As Object = css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
				If cssKey IsNot Nothing Then Return MyBase.removeAttribute(old, cssKey)
			End If
			Return MyBase.removeAttribute(old, key)
		End Function

		''' <summary>
		''' Removes a set of attributes for the element.  If any of the attributes
		''' is a StyleConstants attribute, the request will be converted to a CSS
		''' attribute prior to forwarding to the superclass behavior.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="names"> the attribute names </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
		Public Overrides Function removeAttributes(Of T1)(ByVal old As AttributeSet, ByVal names As System.Collections.IEnumerator(Of T1)) As AttributeSet
			' PENDING: Should really be doing something similar to
			' removeHTMLTags here, but it is rather expensive to have to
			' clone names
			Return MyBase.removeAttributes(old, names)
		End Function

		''' <summary>
		''' Removes a set of attributes. If any of the attributes
		''' is a StyleConstants attribute, the request will be converted to a CSS
		''' attribute prior to forwarding to the superclass behavior.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="attrs"> the attributes </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
		Public Overrides Function removeAttributes(ByVal old As AttributeSet, ByVal attrs As AttributeSet) As AttributeSet
			If old IsNot attrs Then old = removeHTMLTags(old, attrs)
			Return MyBase.removeAttributes(old, convertAttributeSet(attrs))
		End Function

		''' <summary>
		''' Creates a compact set of attributes that might be shared.
		''' This is a hook for subclasses that want to alter the
		''' behavior of SmallAttributeSet.  This can be reimplemented
		''' to return an AttributeSet that provides some sort of
		''' attribute conversion.
		''' </summary>
		''' <param name="a"> The set of attributes to be represented in the
		'''  the compact form. </param>
		Protected Friend Overrides Function createSmallAttributeSet(ByVal a As AttributeSet) As SmallAttributeSet
			Return New SmallConversionSet(Me, a)
		End Function

		''' <summary>
		''' Creates a large set of attributes that should trade off
		''' space for time.  This set will not be shared.  This is
		''' a hook for subclasses that want to alter the behavior
		''' of the larger attribute storage format (which is
		''' SimpleAttributeSet by default).   This can be reimplemented
		''' to return a MutableAttributeSet that provides some sort of
		''' attribute conversion.
		''' </summary>
		''' <param name="a"> The set of attributes to be represented in the
		'''  the larger form. </param>
		Protected Friend Overrides Function createLargeAttributeSet(ByVal a As AttributeSet) As MutableAttributeSet
			Return New LargeConversionSet(Me, a)
		End Function

		''' <summary>
		''' For any StyleConstants key in attr that has an associated HTML.Tag,
		''' it is removed from old. The resulting AttributeSet is then returned.
		''' </summary>
		Private Function removeHTMLTags(ByVal old As AttributeSet, ByVal attr As AttributeSet) As AttributeSet
			If Not(TypeOf attr Is LargeConversionSet) AndAlso Not(TypeOf attr Is SmallConversionSet) Then
				Dim names As System.Collections.IEnumerator = attr.attributeNames

				Do While names.hasMoreElements()
					Dim key As Object = names.nextElement()

					If TypeOf key Is StyleConstants Then
						Dim tag As HTML.Tag = HTML.getTagForStyleConstantsKey(CType(key, StyleConstants))

						If tag IsNot Nothing AndAlso old.isDefined(tag) Then old = MyBase.removeAttribute(old, tag)
					End If
				Loop
			End If
			Return old
		End Function

		''' <summary>
		''' Converts a set of attributes (if necessary) so that
		''' any attributes that were specified as StyleConstants
		''' attributes and have a CSS mapping, will be converted
		''' to CSS attributes.
		''' </summary>
		Friend Overridable Function convertAttributeSet(ByVal a As AttributeSet) As AttributeSet
			If (TypeOf a Is LargeConversionSet) OrElse (TypeOf a Is SmallConversionSet) Then Return a
			' in most cases, there are no StyleConstants attributes
			' so we iterate the collection of keys to avoid creating
			' a new set.
			Dim names As System.Collections.IEnumerator = a.attributeNames
			Do While names.hasMoreElements()
				Dim name As Object = names.nextElement()
				If TypeOf name Is StyleConstants Then
					' we really need to do a conversion, iterate again
					' building a new set.
					Dim converted As MutableAttributeSet = New LargeConversionSet(Me)
					Dim keys As System.Collections.IEnumerator = a.attributeNames
					Do While keys.hasMoreElements()
						Dim key As Object = keys.nextElement()
						Dim cssValue As Object = Nothing
						If TypeOf key Is StyleConstants Then
							' convert the StyleConstants attribute if possible
							Dim cssKey As Object = css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
							If cssKey IsNot Nothing Then
								Dim value As Object = a.getAttribute(key)
								cssValue = css.styleConstantsValueToCSSValue(CType(key, StyleConstants), value)
								If cssValue IsNot Nothing Then converted.addAttribute(cssKey, cssValue)
							End If
						End If
						If cssValue Is Nothing Then converted.addAttribute(key, a.getAttribute(key))
					Loop
					Return converted
				End If
			Loop
			Return a
		End Function

		''' <summary>
		''' Large set of attributes that does conversion of requests
		''' for attributes of type StyleConstants.
		''' </summary>
		Friend Class LargeConversionSet
			Inherits SimpleAttributeSet

			Private ReadOnly outerInstance As StyleSheet


			''' <summary>
			''' Creates a new attribute set based on a supplied set of attributes.
			''' </summary>
			''' <param name="source"> the set of attributes </param>
			Public Sub New(ByVal outerInstance As StyleSheet, ByVal source As AttributeSet)
					Me.outerInstance = outerInstance
				MyBase.New(source)
			End Sub

			Public Sub New(ByVal outerInstance As StyleSheet)
					Me.outerInstance = outerInstance
				MyBase.New()
			End Sub

			''' <summary>
			''' Checks whether a given attribute is defined.
			''' </summary>
			''' <param name="key"> the attribute key </param>
			''' <returns> true if the attribute is defined </returns>
			''' <seealso cref= AttributeSet#isDefined </seealso>
			Public Overrides Function isDefined(ByVal key As Object) As Boolean
				If TypeOf key Is StyleConstants Then
					Dim cssKey As Object = outerInstance.css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then Return MyBase.isDefined(cssKey)
				End If
				Return MyBase.isDefined(key)
			End Function

			''' <summary>
			''' Gets the value of an attribute.
			''' </summary>
			''' <param name="key"> the attribute name </param>
			''' <returns> the attribute value </returns>
			''' <seealso cref= AttributeSet#getAttribute </seealso>
			Public Overrides Function getAttribute(ByVal key As Object) As Object
				If TypeOf key Is StyleConstants Then
					Dim cssKey As Object = outerInstance.css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then
						Dim value As Object = MyBase.getAttribute(cssKey)
						If value IsNot Nothing Then Return outerInstance.css.cssValueToStyleConstantsValue(CType(key, StyleConstants), value)
					End If
				End If
				Return MyBase.getAttribute(key)
			End Function
		End Class

		''' <summary>
		''' Small set of attributes that does conversion of requests
		''' for attributes of type StyleConstants.
		''' </summary>
		Friend Class SmallConversionSet
			Inherits SmallAttributeSet

			Private ReadOnly outerInstance As StyleSheet


			''' <summary>
			''' Creates a new attribute set based on a supplied set of attributes.
			''' </summary>
			''' <param name="attrs"> the set of attributes </param>
			Public Sub New(ByVal outerInstance As StyleSheet, ByVal attrs As AttributeSet)
					Me.outerInstance = outerInstance
				MyBase.New(attrs)
			End Sub

			''' <summary>
			''' Checks whether a given attribute is defined.
			''' </summary>
			''' <param name="key"> the attribute key </param>
			''' <returns> true if the attribute is defined </returns>
			''' <seealso cref= AttributeSet#isDefined </seealso>
			Public Overridable Function isDefined(ByVal key As Object) As Boolean
				If TypeOf key Is StyleConstants Then
					Dim cssKey As Object = outerInstance.css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then Return MyBase.isDefined(cssKey)
				End If
				Return MyBase.isDefined(key)
			End Function

			''' <summary>
			''' Gets the value of an attribute.
			''' </summary>
			''' <param name="key"> the attribute name </param>
			''' <returns> the attribute value </returns>
			''' <seealso cref= AttributeSet#getAttribute </seealso>
			Public Overridable Function getAttribute(ByVal key As Object) As Object
				If TypeOf key Is StyleConstants Then
					Dim cssKey As Object = outerInstance.css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then
						Dim value As Object = MyBase.getAttribute(cssKey)
						If value IsNot Nothing Then Return outerInstance.css.cssValueToStyleConstantsValue(CType(key, StyleConstants), value)
					End If
				End If
				Return MyBase.getAttribute(key)
			End Function
		End Class

		' ---- Resource handling ----------------------------------------

		''' <summary>
		''' Fetches the font to use for the given set of attributes.
		''' </summary>
		Public Overrides Function getFont(ByVal a As AttributeSet) As Font
			Return css.getFont(Me, a, 12, Me)
		End Function

		''' <summary>
		''' Takes a set of attributes and turn it into a foreground color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.
		''' </summary>
		''' <param name="a"> the set of attributes </param>
		''' <returns> the color </returns>
		Public Overrides Function getForeground(ByVal a As AttributeSet) As Color
			Dim c As Color = css.getColor(a, CSS.Attribute.COLOR)
			If c Is Nothing Then Return Color.black
			Return c
		End Function

		''' <summary>
		''' Takes a set of attributes and turn it into a background color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.
		''' </summary>
		''' <param name="a"> the set of attributes </param>
		''' <returns> the color </returns>
		Public Overrides Function getBackground(ByVal a As AttributeSet) As Color
			Return css.getColor(a, CSS.Attribute.BACKGROUND_COLOR)
		End Function

		''' <summary>
		''' Fetches the box formatter to use for the given set
		''' of CSS attributes.
		''' </summary>
		Public Overridable Function getBoxPainter(ByVal a As AttributeSet) As BoxPainter
			Return New BoxPainter(a, css, Me)
		End Function

		''' <summary>
		''' Fetches the list formatter to use for the given set
		''' of CSS attributes.
		''' </summary>
		Public Overridable Function getListPainter(ByVal a As AttributeSet) As ListPainter
			Return New ListPainter(a, Me)
		End Function

		''' <summary>
		''' Sets the base font size, with valid values between 1 and 7.
		''' </summary>
		Public Overridable Property baseFontSize As Integer
			Set(ByVal sz As Integer)
				css.baseFontSize = sz
			End Set
		End Property

		''' <summary>
		''' Sets the base font size from the passed in String. The string
		''' can either identify a specific font size, with legal values between
		''' 1 and 7, or identify a relative font size such as +1 or -2.
		''' </summary>
		Public Overridable Property baseFontSize As String
			Set(ByVal size As String)
				css.baseFontSize = size
			End Set
		End Property

		Public Shared Function getIndexOfSize(ByVal pt As Single) As Integer
			Return CSS.getIndexOfSize(pt, sizeMapDefault)
		End Function

		''' <summary>
		''' Returns the point size, given a size index.
		''' </summary>
		Public Overridable Function getPointSize(ByVal index As Integer) As Single
			Return css.getPointSize(index, Me)
		End Function

		''' <summary>
		'''  Given a string such as "+2", "-2", or "2",
		'''  returns a point size value.
		''' </summary>
		Public Overridable Function getPointSize(ByVal size As String) As Single
			Return css.getPointSize(size, Me)
		End Function

		''' <summary>
		''' Converts a color string such as "RED" or "#NNNNNN" to a Color.
		''' Note: This will only convert the HTML3.2 color strings
		'''       or a string of length 7;
		'''       otherwise, it will return null.
		''' </summary>
		Public Overridable Function stringToColor(ByVal [string] As String) As Color
			Return CSS.stringToColor([string])
		End Function

		''' <summary>
		''' Returns the ImageIcon to draw in the background for
		''' <code>attr</code>.
		''' </summary>
		Friend Overridable Function getBackgroundImage(ByVal attr As AttributeSet) As javax.swing.ImageIcon
			Dim value As Object = attr.getAttribute(CSS.Attribute.BACKGROUND_IMAGE)

			If value IsNot Nothing Then Return CType(value, CSS.BackgroundImage).getImage(base)
			Return Nothing
		End Function

		''' <summary>
		''' Adds a rule into the StyleSheet.
		''' </summary>
		''' <param name="selector"> the selector to use for the rule.
		'''  This will be a set of simple selectors, and must
		'''  be a length of 1 or greater. </param>
		''' <param name="declaration"> the set of CSS attributes that
		'''  make up the rule. </param>
		Friend Overridable Sub addRule(ByVal selector As String(), ByVal declaration As AttributeSet, ByVal isLinked As Boolean)
			Dim n As Integer = selector.Length
			Dim sb As New StringBuilder
			sb.Append(selector(0))
			For counter As Integer = 1 To n - 1
				sb.Append(" "c)
				sb.Append(selector(counter))
			Next counter
			Dim selectorName As String = sb.ToString()
			Dim ___rule As Style = getStyle(selectorName)
			If ___rule Is Nothing Then
				' Notice how the rule is first created, and it not part of
				' the synchronized block. It is done like this as creating
				' a new rule will fire a ChangeEvent. We do not want to be
				' holding the lock when calling to other objects, it can
				' result in deadlock.
				Dim altRule As Style = addStyle(selectorName, Nothing)
				SyncLock Me
					Dim mapping As SelectorMapping = rootSelectorMapping
					For i As Integer = n - 1 To 0 Step -1
						mapping = mapping.getChildSelectorMapping(selector(i), True)
					Next i
					___rule = mapping.style
					If ___rule Is Nothing Then
						___rule = altRule
						mapping.style = ___rule
						refreshResolvedRules(selectorName, selector, ___rule, mapping.specificity)
					End If
				End SyncLock
			End If
			If isLinked Then ___rule = getLinkedStyle(___rule)
			___rule.addAttributes(declaration)
		End Sub

		'
		' The following gaggle of methods is used in maintaining the rules from
		' the sheet.
		'

		''' <summary>
		''' Updates the attributes of the rules to reference any related
		''' rules in <code>ss</code>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub linkStyleSheetAt(ByVal ss As StyleSheet, ByVal index As Integer)
			If resolvedStyles.Count > 0 Then
				Dim values As System.Collections.IEnumerator(Of ResolvedStyle) = resolvedStyles.Values.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While values.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ___rule As ResolvedStyle = values.nextElement()
					___rule.insertExtendedStyleAt(ss.getRule(___rule.name), index)
				Loop
			End If
		End Sub

		''' <summary>
		''' Removes references to the rules in <code>ss</code>.
		''' <code>index</code> gives the index the StyleSheet was at, that is
		''' how many StyleSheets had been added before it.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub unlinkStyleSheet(ByVal ss As StyleSheet, ByVal index As Integer)
			If resolvedStyles.Count > 0 Then
				Dim values As System.Collections.IEnumerator(Of ResolvedStyle) = resolvedStyles.Values.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While values.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ___rule As ResolvedStyle = values.nextElement()
					___rule.removeExtendedStyleAt(index)
				Loop
			End If
		End Sub

		''' <summary>
		''' Returns the simple selectors that comprise selector.
		''' </summary>
		' protected 
		Friend Overridable Function getSimpleSelectors(ByVal selector As String) As String()
			selector = cleanSelectorString(selector)
			Dim sb As SearchBuffer = SearchBuffer.obtainSearchBuffer()
			Dim selectors As List(Of String) = sb.vector
			Dim lastIndex As Integer = 0
			Dim length As Integer = selector.Length
			Do While lastIndex <> -1
				Dim newIndex As Integer = selector.IndexOf(" "c, lastIndex)
				If newIndex <> -1 Then
					selectors.Add(selector.Substring(lastIndex, newIndex - lastIndex))
					newIndex += 1
					If newIndex = length Then
						lastIndex = -1
					Else
						lastIndex = newIndex
					End If
				Else
					selectors.Add(selector.Substring(lastIndex))
					lastIndex = -1
				End If
			Loop
			Dim retValue As String() = New String(selectors.Count - 1){}
			selectors.CopyTo(retValue)
			SearchBuffer.releaseSearchBuffer(sb)
			Return retValue
		End Function

		''' <summary>
		''' Returns a string that only has one space between simple selectors,
		''' which may be the passed in String.
		''' </summary>
		'protected
	 Friend Overridable Function cleanSelectorString(ByVal selector As String) As String
			Dim lastWasSpace As Boolean = True
			Dim counter As Integer = 0
			Dim maxCounter As Integer = selector.Length
			Do While counter < maxCounter
				Select Case selector.Chars(counter)
				Case " "c
					If lastWasSpace Then Return _cleanSelectorString(selector)
					lastWasSpace = True
				Case ControlChars.Lf, ControlChars.Cr, ControlChars.Tab
					Return _cleanSelectorString(selector)
				Case Else
					lastWasSpace = False
				End Select
				counter += 1
			Loop
			If lastWasSpace Then Return _cleanSelectorString(selector)
			' It was fine.
			Return selector
	 End Function

		''' <summary>
		''' Returns a new String that contains only one space between non
		''' white space characters.
		''' </summary>
		Private Function _cleanSelectorString(ByVal selector As String) As String
			Dim sb As SearchBuffer = SearchBuffer.obtainSearchBuffer()
			Dim buff As StringBuilder = sb.stringBuffer
			Dim lastWasSpace As Boolean = True
			Dim lastIndex As Integer = 0
			Dim chars As Char() = selector.ToCharArray()
			Dim numChars As Integer = chars.Length
			Dim retValue As String = Nothing
			Try
				For counter As Integer = 0 To numChars - 1
					Select Case chars(counter)
					Case " "c
						If Not lastWasSpace Then
							lastWasSpace = True
							If lastIndex < counter Then buff.Append(chars, lastIndex, 1 + counter - lastIndex)
						End If
						lastIndex = counter + 1
					Case ControlChars.Lf, ControlChars.Cr, ControlChars.Tab
						If Not lastWasSpace Then
							lastWasSpace = True
							If lastIndex < counter Then
								buff.Append(chars, lastIndex, counter - lastIndex)
								buff.Append(" "c)
							End If
						End If
						lastIndex = counter + 1
					Case Else
						lastWasSpace = False
					End Select
				Next counter
				If lastWasSpace AndAlso buff.Length > 0 Then
					' Remove last space.
					buff.Length = buff.Length - 1
				ElseIf lastIndex < numChars Then
					buff.Append(chars, lastIndex, numChars - lastIndex)
				End If
				retValue = buff.ToString()
			Finally
				SearchBuffer.releaseSearchBuffer(sb)
			End Try
			Return retValue
		End Function

		''' <summary>
		''' Returns the root selector mapping that all selectors are relative
		''' to. This is an inverted graph of the selectors.
		''' </summary>
		Private Property rootSelectorMapping As SelectorMapping
			Get
				Return selectorMapping
			End Get
		End Property

		''' <summary>
		''' Returns the specificity of the passed in String. It assumes the
		''' passed in string doesn't contain junk, that is each selector is
		''' separated by a space and each selector at most contains one . or one
		''' #. A simple selector has a weight of 1, an id selector has a weight
		''' of 100, and a class selector has a weight of 10000.
		''' </summary>
		'protected
	 Friend Shared Function getSpecificity(ByVal selector As String) As Integer
			Dim ___specificity As Integer = 0
			Dim lastWasSpace As Boolean = True

			Dim counter As Integer = 0
			Dim maxCounter As Integer = selector.Length
			Do While counter < maxCounter
				Select Case selector.Chars(counter)
				Case "."c
					___specificity += 100
				Case "#"c
					___specificity += 10000
				Case " "c
					lastWasSpace = True
				Case Else
					If lastWasSpace Then
						lastWasSpace = False
						___specificity += 1
					End If
				End Select
				counter += 1
			Loop
			Return ___specificity
	 End Function

		''' <summary>
		''' Returns the style that linked attributes should be added to. This
		''' will create the style if necessary.
		''' </summary>
		Private Function getLinkedStyle(ByVal localStyle As Style) As Style
			' NOTE: This is not synchronized, and the caller of this does
			' not synchronize. There is the chance for one of the callers to
			' overwrite the existing resolved parent, but it is quite rare.
			' The reason this is left like this is because setResolveParent
			' will fire a ChangeEvent. It is really, REALLY bad for us to
			' hold a lock when calling outside of us, it may cause a deadlock.
			Dim retStyle As Style = CType(localStyle.resolveParent, Style)
			If retStyle Is Nothing Then
				retStyle = addStyle(Nothing, Nothing)
				localStyle.resolveParent = retStyle
			End If
			Return retStyle
		End Function

		''' <summary>
		''' Returns the resolved style for <code>selector</code>. This will
		''' create the resolved style, if necessary.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function getResolvedStyle(ByVal selector As String, ByVal elements As ArrayList, ByVal t As HTML.Tag) As Style
			Dim retStyle As Style = resolvedStyles(selector)
			If retStyle Is Nothing Then retStyle = createResolvedStyle(selector, elements, t)
			Return retStyle
		End Function

		''' <summary>
		''' Returns the resolved style for <code>selector</code>. This will
		''' create the resolved style, if necessary.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function getResolvedStyle(ByVal selector As String) As Style
			Dim retStyle As Style = resolvedStyles(selector)
			If retStyle Is Nothing Then retStyle = createResolvedStyle(selector)
			Return retStyle
		End Function

		''' <summary>
		''' Adds <code>mapping</code> to <code>elements</code>. It is added
		''' such that <code>elements</code> will remain ordered by
		''' specificity.
		''' </summary>
		Private Sub addSortedStyle(ByVal mapping As SelectorMapping, ByVal elements As List(Of SelectorMapping))
			Dim size As Integer = elements.Count

			If size > 0 Then
				Dim ___specificity As Integer = mapping.specificity

				For counter As Integer = 0 To size - 1
					If ___specificity >= elements(counter).specificity Then
						elements.Insert(counter, mapping)
						Return
					End If
				Next counter
			End If
			elements.Add(mapping)
		End Sub

		''' <summary>
		''' Adds <code>parentMapping</code> to <code>styles</code>, and
		''' recursively calls this method if <code>parentMapping</code> has
		''' any child mappings for any of the Elements in <code>elements</code>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub getStyles(ByVal parentMapping As SelectorMapping, ByVal styles As List(Of SelectorMapping), ByVal tags As String(), ByVal ids As String(), ByVal classes As String(), ByVal index As Integer, ByVal numElements As Integer, ByVal alreadyChecked As Dictionary(Of SelectorMapping, SelectorMapping))
			' Avoid desending the same mapping twice.
			If alreadyChecked.Contains(parentMapping) Then Return
			alreadyChecked(parentMapping) = parentMapping
			Dim ___style As Style = parentMapping.style
			If ___style IsNot Nothing Then addSortedStyle(parentMapping, styles)
			For counter As Integer = index To numElements - 1
				Dim tagString As String = tags(counter)
				If tagString IsNot Nothing Then
					Dim childMapping As SelectorMapping = parentMapping.getChildSelectorMapping(tagString, False)
					If childMapping IsNot Nothing Then getStyles(childMapping, styles, tags, ids, classes, counter + 1, numElements, alreadyChecked)
					If classes(counter) IsNot Nothing Then
						Dim className As String = classes(counter)
						childMapping = parentMapping.getChildSelectorMapping(tagString & "." & className, False)
						If childMapping IsNot Nothing Then getStyles(childMapping, styles, tags, ids, classes, counter + 1, numElements, alreadyChecked)
						childMapping = parentMapping.getChildSelectorMapping("." & className, False)
						If childMapping IsNot Nothing Then getStyles(childMapping, styles, tags, ids, classes, counter + 1, numElements, alreadyChecked)
					End If
					If ids(counter) IsNot Nothing Then
						Dim idName As String = ids(counter)
						childMapping = parentMapping.getChildSelectorMapping(tagString & "#" & idName, False)
						If childMapping IsNot Nothing Then getStyles(childMapping, styles, tags, ids, classes, counter + 1, numElements, alreadyChecked)
						childMapping = parentMapping.getChildSelectorMapping("#" & idName, False)
						If childMapping IsNot Nothing Then getStyles(childMapping, styles, tags, ids, classes, counter + 1, numElements, alreadyChecked)
					End If
				End If
			Next counter
		End Sub

		''' <summary>
		''' Creates and returns a Style containing all the rules that match
		'''  <code>selector</code>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function createResolvedStyle(ByVal selector As String, ByVal tags As String(), ByVal ids As String(), ByVal classes As String()) As Style
			Dim sb As SearchBuffer = SearchBuffer.obtainSearchBuffer()
			Dim tempVector As List(Of SelectorMapping) = sb.vector
			Dim tempHashtable As Dictionary(Of SelectorMapping, SelectorMapping) = sb.hashtable
			' Determine all the Styles that are appropriate, placing them
			' in tempVector
			Try
				Dim mapping As SelectorMapping = rootSelectorMapping
				Dim numElements As Integer = tags.Length
				Dim tagString As String = tags(0)
				Dim childMapping As SelectorMapping = mapping.getChildSelectorMapping(tagString, False)
				If childMapping IsNot Nothing Then getStyles(childMapping, tempVector, tags, ids, classes, 1, numElements, tempHashtable)
				If classes(0) IsNot Nothing Then
					Dim className As String = classes(0)
					childMapping = mapping.getChildSelectorMapping(tagString & "." & className, False)
					If childMapping IsNot Nothing Then getStyles(childMapping, tempVector, tags, ids, classes, 1, numElements, tempHashtable)
					childMapping = mapping.getChildSelectorMapping("." & className, False)
					If childMapping IsNot Nothing Then getStyles(childMapping, tempVector, tags, ids, classes, 1, numElements, tempHashtable)
				End If
				If ids(0) IsNot Nothing Then
					Dim idName As String = ids(0)
					childMapping = mapping.getChildSelectorMapping(tagString & "#" & idName, False)
					If childMapping IsNot Nothing Then getStyles(childMapping, tempVector, tags, ids, classes, 1, numElements, tempHashtable)
					childMapping = mapping.getChildSelectorMapping("#" & idName, False)
					If childMapping IsNot Nothing Then getStyles(childMapping, tempVector, tags, ids, classes, 1, numElements, tempHashtable)
				End If
				' Create a new Style that will delegate to all the matching
				' Styles.
				Dim numLinkedSS As Integer = If(linkedStyleSheets IsNot Nothing, linkedStyleSheets.Count, 0)
				Dim numStyles As Integer = tempVector.Count
				Dim attrs As AttributeSet() = New AttributeSet(numStyles + numLinkedSS - 1){}
				For counter As Integer = 0 To numStyles - 1
					attrs(counter) = tempVector(counter).style
				Next counter
				' Get the AttributeSet from linked style sheets.
				For counter As Integer = 0 To numLinkedSS - 1
					Dim attr As AttributeSet = linkedStyleSheets(counter).getRule(selector)
					If attr Is Nothing Then
						attrs(counter + numStyles) = SimpleAttributeSet.EMPTY
					Else
						attrs(counter + numStyles) = attr
					End If
				Next counter
				Dim retStyle As New ResolvedStyle(selector, attrs, numStyles)
				resolvedStyles(selector) = retStyle
				Return retStyle
			Finally
				SearchBuffer.releaseSearchBuffer(sb)
			End Try
		End Function

		''' <summary>
		''' Creates and returns a Style containing all the rules that
		''' matches <code>selector</code>.
		''' </summary>
		''' <param name="elements">  a Vector of all the Elements
		'''                  the style is being asked for. The
		'''                  first Element is the deepest Element, with the last Element
		'''                  representing the root. </param>
		''' <param name="t">         the Tag to use for
		'''                  the first Element in <code>elements</code> </param>
		Private Function createResolvedStyle(ByVal selector As String, ByVal elements As ArrayList, ByVal t As HTML.Tag) As Style
			Dim numElements As Integer = elements.Count
			' Build three arrays, one for tags, one for class's, and one for
			' id's
			Dim tags As String() = New String(numElements - 1){}
			Dim ids As String() = New String(numElements - 1){}
			Dim classes As String() = New String(numElements - 1){}
			For counter As Integer = 0 To numElements - 1
				Dim e As Element = CType(elements(counter), Element)
				Dim attr As AttributeSet = e.attributes
				If counter = 0 AndAlso e.leaf Then
					' For leafs, we use the second tier attributes.
					Dim testAttr As Object = attr.getAttribute(t)
					If TypeOf testAttr Is AttributeSet Then
						attr = CType(testAttr, AttributeSet)
					Else
						attr = Nothing
					End If
				End If
				If attr IsNot Nothing Then
					Dim tag As HTML.Tag = CType(attr.getAttribute(StyleConstants.NameAttribute), HTML.Tag)
					If tag IsNot Nothing Then
						tags(counter) = tag.ToString()
					Else
						tags(counter) = Nothing
					End If
					If attr.isDefined(HTML.Attribute.CLASS) Then
						classes(counter) = attr.getAttribute(HTML.Attribute.CLASS).ToString()
					Else
						classes(counter) = Nothing
					End If
					If attr.isDefined(HTML.Attribute.ID) Then
						ids(counter) = attr.getAttribute(HTML.Attribute.ID).ToString()
					Else
						ids(counter) = Nothing
					End If
				Else
						classes(counter) = Nothing
							ids(counter) = classes(counter)
							tags(counter) = ids(counter)
				End If
			Next counter
			tags(0) = t.ToString()
			Return createResolvedStyle(selector, tags, ids, classes)
		End Function

		''' <summary>
		''' Creates and returns a Style containing all the rules that match
		'''  <code>selector</code>. It is assumed that each simple selector
		''' in <code>selector</code> is separated by a space.
		''' </summary>
		Private Function createResolvedStyle(ByVal selector As String) As Style
			Dim sb As SearchBuffer = SearchBuffer.obtainSearchBuffer()
			' Will contain the tags, ids, and classes, in that order.
			Dim elements As List(Of String) = sb.vector
			Try
				Dim done As Boolean
				Dim dotIndex As Integer = 0
				Dim spaceIndex As Integer
				Dim poundIndex As Integer = 0
				Dim lastIndex As Integer = 0
				Dim length As Integer = selector.Length
				Do While lastIndex < length
					If dotIndex = lastIndex Then dotIndex = selector.IndexOf("."c, lastIndex)
					If poundIndex = lastIndex Then poundIndex = selector.IndexOf("#"c, lastIndex)
					spaceIndex = selector.IndexOf(" "c, lastIndex)
					If spaceIndex = -1 Then spaceIndex = length
					If dotIndex <> -1 AndAlso poundIndex <> -1 AndAlso dotIndex < spaceIndex AndAlso poundIndex < spaceIndex Then
						If poundIndex < dotIndex Then
							' #.
							If lastIndex = poundIndex Then
								elements.Add("")
							Else
								elements.Add(selector.Substring(lastIndex, poundIndex - lastIndex))
							End If
							If (dotIndex + 1) < spaceIndex Then
								elements.Add(selector.Substring(dotIndex + 1, spaceIndex - (dotIndex + 1)))
							Else
								elements.Add(Nothing)
							End If
							If (poundIndex + 1) = dotIndex Then
								elements.Add(Nothing)
							Else
								elements.Add(selector.Substring(poundIndex + 1, dotIndex - (poundIndex + 1)))
							End If
						ElseIf poundIndex < spaceIndex Then
							' .#
							If lastIndex = dotIndex Then
								elements.Add("")
							Else
								elements.Add(selector.Substring(lastIndex, dotIndex - lastIndex))
							End If
							If (dotIndex + 1) < poundIndex Then
								elements.Add(selector.Substring(dotIndex + 1, poundIndex - (dotIndex + 1)))
							Else
								elements.Add(Nothing)
							End If
							If (poundIndex + 1) = spaceIndex Then
								elements.Add(Nothing)
							Else
								elements.Add(selector.Substring(poundIndex + 1, spaceIndex - (poundIndex + 1)))
							End If
						End If
							poundIndex = spaceIndex + 1
							dotIndex = poundIndex
					ElseIf dotIndex <> -1 AndAlso dotIndex < spaceIndex Then
						' .
						If dotIndex = lastIndex Then
							elements.Add("")
						Else
							elements.Add(selector.Substring(lastIndex, dotIndex - lastIndex))
						End If
						If (dotIndex + 1) = spaceIndex Then
							elements.Add(Nothing)
						Else
							elements.Add(selector.Substring(dotIndex + 1, spaceIndex - (dotIndex + 1)))
						End If
						elements.Add(Nothing)
						dotIndex = spaceIndex + 1
					ElseIf poundIndex <> -1 AndAlso poundIndex < spaceIndex Then
						' #
						If poundIndex = lastIndex Then
							elements.Add("")
						Else
							elements.Add(selector.Substring(lastIndex, poundIndex - lastIndex))
						End If
						elements.Add(Nothing)
						If (poundIndex + 1) = spaceIndex Then
							elements.Add(Nothing)
						Else
							elements.Add(selector.Substring(poundIndex + 1, spaceIndex - (poundIndex + 1)))
						End If
						poundIndex = spaceIndex + 1
					Else
						' id
						elements.Add(selector.Substring(lastIndex, spaceIndex - lastIndex))
						elements.Add(Nothing)
						elements.Add(Nothing)
					End If
					lastIndex = spaceIndex + 1
				Loop
				' Create the tag, id, and class arrays.
				Dim total As Integer = elements.Count
				Dim numTags As Integer = total \ 3
				Dim tags As String() = New String(numTags - 1){}
				Dim ids As String() = New String(numTags - 1){}
				Dim classes As String() = New String(numTags - 1){}
				Dim index As Integer = 0
				Dim eIndex As Integer = total - 3
				Do While index < numTags
					tags(index) = elements(eIndex)
					classes(index) = elements(eIndex + 1)
					ids(index) = elements(eIndex + 2)
					index += 1
					eIndex -= 3
				Loop
				Return createResolvedStyle(selector, tags, ids, classes)
			Finally
				SearchBuffer.releaseSearchBuffer(sb)
			End Try
		End Function

		''' <summary>
		''' Should be invoked when a new rule is added that did not previously
		''' exist. Goes through and refreshes the necessary resolved
		''' rules.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub refreshResolvedRules(ByVal selectorName As String, ByVal selector As String(), ByVal newStyle As Style, ByVal specificity As Integer)
			If resolvedStyles.Count > 0 Then
				Dim values As System.Collections.IEnumerator(Of ResolvedStyle) = resolvedStyles.Values.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While values.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ___style As ResolvedStyle = values.nextElement()
					If ___style.matches(selectorName) Then ___style.insertStyle(newStyle, specificity)
				Loop
			End If
		End Sub


		''' <summary>
		''' A temporary class used to hold a Vector, a StringBuffer and a
		''' Hashtable. This is used to avoid allocing a lot of garbage when
		''' searching for rules. Use the static method obtainSearchBuffer and
		''' releaseSearchBuffer to get a SearchBuffer, and release it when
		''' done.
		''' </summary>
		Private Class SearchBuffer
			''' <summary>
			''' A stack containing instances of SearchBuffer. Used in getting
			''' rules. 
			''' </summary>
			Friend Shared searchBuffers As New Stack(Of SearchBuffer)
			' A set of temporary variables that can be used in whatever way.
			Friend vector As ArrayList = Nothing
			Friend stringBuffer As StringBuilder = Nothing
			Friend hashtable As Hashtable = Nothing

			''' <summary>
			''' Returns an instance of SearchBuffer. Be sure and issue
			''' a releaseSearchBuffer when done with it.
			''' </summary>
			Shared Function obtainSearchBuffer() As SearchBuffer
				Dim sb As SearchBuffer
				Try
					If searchBuffers.Count > 0 Then
					   sb = searchBuffers.Pop()
					Else
					   sb = New SearchBuffer
					End If
				Catch ese As EmptyStackException
					sb = New SearchBuffer
				End Try
				Return sb
			End Function

			''' <summary>
			''' Adds <code>sb</code> to the stack of SearchBuffers that can
			''' be used.
			''' </summary>
			Shared Sub releaseSearchBuffer(ByVal sb As SearchBuffer)
				sb.empty()
				searchBuffers.Push(sb)
			End Sub

			Friend Overridable Property stringBuffer As StringBuilder
				Get
					If stringBuffer Is Nothing Then stringBuffer = New StringBuilder
					Return stringBuffer
				End Get
			End Property

			Friend Overridable Property vector As ArrayList
				Get
					If vector Is Nothing Then vector = New ArrayList
					Return vector
				End Get
			End Property

			Friend Overridable Property hashtable As Hashtable
				Get
					If hashtable Is Nothing Then hashtable = New Hashtable
					Return hashtable
				End Get
			End Property

			Friend Overridable Sub empty()
				If stringBuffer IsNot Nothing Then stringBuffer.Length = 0
				If vector IsNot Nothing Then vector.Clear()
				If hashtable IsNot Nothing Then hashtable.Clear()
			End Sub
		End Class


		Friend Shared ReadOnly noBorder As Border = New EmptyBorder(0,0,0,0)

		''' <summary>
		''' Class to carry out some of the duties of
		''' CSS formatting.  Implementations of this
		''' class enable views to present the CSS formatting
		''' while not knowing anything about how the CSS values
		''' are being cached.
		''' <p>
		''' As a delegate of Views, this object is responsible for
		''' the insets of a View and making sure the background
		''' is maintained according to the CSS attributes.
		''' </summary>
		<Serializable> _
		Public Class BoxPainter

			Friend Sub New(ByVal a As AttributeSet, ByVal css As CSS, ByVal ss As StyleSheet)
				Me.ss = ss
				Me.css = css
				border = getBorder(a)
				binsets = border.getBorderInsets(Nothing)
				topMargin = getLength(CSS.Attribute.MARGIN_TOP, a)
				bottomMargin = getLength(CSS.Attribute.MARGIN_BOTTOM, a)
				leftMargin = getLength(CSS.Attribute.MARGIN_LEFT, a)
				rightMargin = getLength(CSS.Attribute.MARGIN_RIGHT, a)
				bg = ss.getBackground(a)
				If ss.getBackgroundImage(a) IsNot Nothing Then bgPainter = New BackgroundImagePainter(a, css, ss)
			End Sub

			''' <summary>
			''' Fetches a border to render for the given attributes.
			''' PENDING(prinz) This is pretty badly hacked at the
			''' moment.
			''' </summary>
			Friend Overridable Function getBorder(ByVal a As AttributeSet) As Border
				Return New CSSBorder(a)
			End Function

			''' <summary>
			''' Fetches the color to use for borders.  This will either be
			''' the value specified by the border-color attribute (which
			''' is not inherited), or it will default to the color attribute
			''' (which is inherited).
			''' </summary>
			Friend Overridable Function getBorderColor(ByVal a As AttributeSet) As Color
				Dim color As Color = css.getColor(a, CSS.Attribute.BORDER_COLOR)
				If color Is Nothing Then
					color = css.getColor(a, CSS.Attribute.COLOR)
					If color Is Nothing Then Return Color.black
				End If
				Return color
			End Function

			''' <summary>
			''' Fetches the inset needed on a given side to
			''' account for the margin, border, and padding.
			''' </summary>
			''' <param name="side"> The size of the box to fetch the
			'''  inset for.  This can be View.TOP,
			'''  View.LEFT, View.BOTTOM, or View.RIGHT. </param>
			''' <param name="v"> the view making the request.  This is
			'''  used to get the AttributeSet, and may be used to
			'''  resolve percentage arguments. </param>
			''' <exception cref="IllegalArgumentException"> for an invalid direction </exception>
			Public Overridable Function getInset(ByVal side As Integer, ByVal v As View) As Single
				Dim a As AttributeSet = v.attributes
				Dim ___inset As Single = 0
				Select Case side
				Case View.LEFT
					___inset += getOrientationMargin(HorizontalMargin.LEFT, leftMargin, a, isLeftToRight(v))
					___inset += binsets.left
					___inset += getLength(CSS.Attribute.PADDING_LEFT, a)
				Case View.RIGHT
					___inset += getOrientationMargin(HorizontalMargin.RIGHT, rightMargin, a, isLeftToRight(v))
					___inset += binsets.right
					___inset += getLength(CSS.Attribute.PADDING_RIGHT, a)
				Case View.TOP
					___inset += topMargin
					___inset += binsets.top
					___inset += getLength(CSS.Attribute.PADDING_TOP, a)
				Case View.BOTTOM
					___inset += bottomMargin
					___inset += binsets.bottom
					___inset += getLength(CSS.Attribute.PADDING_BOTTOM, a)
				Case Else
					Throw New System.ArgumentException("Invalid side: " & side)
				End Select
				Return ___inset
			End Function

			''' <summary>
			''' Paints the CSS box according to the attributes
			''' given.  This should paint the border, padding,
			''' and background.
			''' </summary>
			''' <param name="g"> the rendering surface. </param>
			''' <param name="x"> the x coordinate of the allocated area to
			'''  render into. </param>
			''' <param name="y"> the y coordinate of the allocated area to
			'''  render into. </param>
			''' <param name="w"> the width of the allocated area to render into. </param>
			''' <param name="h"> the height of the allocated area to render into. </param>
			''' <param name="v"> the view making the request.  This is
			'''  used to get the AttributeSet, and may be used to
			'''  resolve percentage arguments. </param>
			Public Overridable Sub paint(ByVal g As Graphics, ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal v As View)
				' PENDING(prinz) implement real rendering... which would
				' do full set of border and background capabilities.
				' remove margin

				Dim dx As Single = 0
				Dim dy As Single = 0
				Dim dw As Single = 0
				Dim dh As Single = 0
				Dim a As AttributeSet = v.attributes
				Dim isLeftToRight As Boolean = isLeftToRight(v)
				Dim localLeftMargin As Single = getOrientationMargin(HorizontalMargin.LEFT, leftMargin, a, isLeftToRight)
				Dim localRightMargin As Single = getOrientationMargin(HorizontalMargin.RIGHT, rightMargin, a, isLeftToRight)
				If Not(TypeOf v Is HTMLEditorKit.HTMLFactory.BodyBlockView) Then
					dx = localLeftMargin
					dy = topMargin
					dw = -(localLeftMargin + localRightMargin)
					dh = -(topMargin + bottomMargin)
				End If
				If bg IsNot Nothing Then
					g.color = bg
					g.fillRect(CInt(Fix(x + dx)), CInt(Fix(y + dy)), CInt(Fix(w + dw)), CInt(Fix(h + dh)))
				End If
				If bgPainter IsNot Nothing Then bgPainter.paint(g, x + dx, y + dy, w + dw, h + dh, v)
				x += localLeftMargin
				y += topMargin
				w -= localLeftMargin + localRightMargin
				h -= topMargin + bottomMargin
				If TypeOf border Is BevelBorder Then
					'BevelBorder does not support border width
					Dim bw As Integer = CInt(getLength(CSS.Attribute.BORDER_TOP_WIDTH, a))
					For i As Integer = bw - 1 To 0 Step -1
						border.paintBorder(Nothing, g, CInt(Fix(x)) + i, CInt(Fix(y)) + i, CInt(Fix(w)) - 2 * i, CInt(Fix(h)) - 2 * i)
					Next i
				Else
					border.paintBorder(Nothing, g, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)))
				End If
			End Sub

			Friend Overridable Function getLength(ByVal key As CSS.Attribute, ByVal a As AttributeSet) As Single
				Return css.getLength(a, key, ss)
			End Function

			Friend Shared Function isLeftToRight(ByVal v As View) As Boolean
				Dim ret As Boolean = True
				If isOrientationAware(v) Then
					Dim container As Container
					container = v.container
					If v IsNot Nothing AndAlso container IsNot Nothing Then ret = container.componentOrientation.leftToRight
				End If
				Return ret
			End Function

	'        
	'         * only certain tags are concerned about orientation
	'         * <dir>, <menu>, <ul>, <ol>
	'         * for all others we return true. It is implemented this way
	'         * for performance purposes
	'         
			Friend Shared Function isOrientationAware(ByVal v As View) As Boolean
				Dim ret As Boolean = False
				Dim attr As AttributeSet
				Dim obj As Object
				attr = v.element.attributes
				obj = attr.getAttribute(StyleConstants.NameAttribute)
				If v IsNot Nothing AndAlso attr IsNot Nothing AndAlso TypeOf obj Is HTML.Tag AndAlso (obj Is HTML.Tag.DIR OrElse obj Is HTML.Tag.MENU OrElse obj Is HTML.Tag.UL OrElse obj Is HTML.Tag.OL) Then ret = True

				Return ret
			End Function

			Friend Enum HorizontalMargin
				LEFT
				RIGHT
			End Enum

			''' <summary>
			''' for <dir>, <menu>, <ul> etc.
			''' margins are Left-To-Right/Right-To-Left depended.
			''' see 5088268 for more details
			''' margin-(left|right)-(ltr|rtl) were introduced to describe it
			''' if margin-(left|right) is present we are to use it.
			''' </summary>
			''' <param name="side"> The horizontal side to fetch margin for
			'''  This can be HorizontalMargin.LEFT or HorizontalMargin.RIGHT </param>
			''' <param name="cssMargin"> margin from css </param>
			''' <param name="a"> AttributeSet for the View we getting margin for </param>
			''' <param name="isLeftToRight"> </param>
			''' <returns> orientation depended margin </returns>
			Friend Overridable Function getOrientationMargin(ByVal side As HorizontalMargin, ByVal cssMargin As Single, ByVal a As AttributeSet, ByVal isLeftToRight As Boolean) As Single
				Dim margin As Single = cssMargin
				Dim ___orientationMargin As Single = cssMargin
				Dim cssMarginValue As Object = Nothing
				Select Case side
				Case HorizontalMargin.RIGHT
						___orientationMargin = If(isLeftToRight, getLength(CSS.Attribute.MARGIN_RIGHT_LTR, a), getLength(CSS.Attribute.MARGIN_RIGHT_RTL, a))
						cssMarginValue = a.getAttribute(CSS.Attribute.MARGIN_RIGHT)
				Case HorizontalMargin.LEFT
						___orientationMargin = If(isLeftToRight, getLength(CSS.Attribute.MARGIN_LEFT_LTR, a), getLength(CSS.Attribute.MARGIN_LEFT_RTL, a))
						cssMarginValue = a.getAttribute(CSS.Attribute.MARGIN_LEFT)
				End Select

				If cssMarginValue Is Nothing AndAlso ___orientationMargin <> Integer.MinValue Then margin = ___orientationMargin
				Return margin
			End Function

			Friend topMargin As Single
			Friend bottomMargin As Single
			Friend leftMargin As Single
			Friend rightMargin As Single
			' Bitmask, used to indicate what margins are relative:
			' bit 0 for top, 1 for bottom, 2 for left and 3 for right.
			Friend marginFlags As Short
			Friend border As Border
			Friend binsets As Insets
			Friend css As CSS
			Friend ss As StyleSheet
			Friend bg As Color
			Friend bgPainter As BackgroundImagePainter
		End Class

		''' <summary>
		''' Class to carry out some of the duties of CSS list
		''' formatting.  Implementations of this
		''' class enable views to present the CSS formatting
		''' while not knowing anything about how the CSS values
		''' are being cached.
		''' </summary>
		<Serializable> _
		Public Class ListPainter

			Friend Sub New(ByVal attr As AttributeSet, ByVal ss As StyleSheet)
				Me.ss = ss
				' Get the image to use as a list bullet 
				Dim imgstr As String = CStr(attr.getAttribute(CSS.Attribute.LIST_STYLE_IMAGE))
				type = Nothing
				If imgstr IsNot Nothing AndAlso (Not imgstr.Equals("none")) Then
					Dim tmpstr As String = Nothing
					Try
						Dim st As New StringTokenizer(imgstr, "()")
						If st.hasMoreTokens() Then tmpstr = st.nextToken()
						If st.hasMoreTokens() Then tmpstr = st.nextToken()
						Dim u As New URL(tmpstr)
						img = New javax.swing.ImageIcon(u)
					Catch e As MalformedURLException
						If tmpstr IsNot Nothing AndAlso ss IsNot Nothing AndAlso ss.base IsNot Nothing Then
							Try
								Dim u As New URL(ss.base, tmpstr)
								img = New javax.swing.ImageIcon(u)
							Catch murle As MalformedURLException
								img = Nothing
							End Try
						Else
							img = Nothing
						End If
					End Try
				End If

				' Get the type of bullet to use in the list 
				If img Is Nothing Then type = CType(attr.getAttribute(CSS.Attribute.LIST_STYLE_TYPE), CSS.Value)
				start = 1

				paintRect = New Rectangle
			End Sub

			''' <summary>
			''' Returns a string that represents the value
			''' of the HTML.Attribute.TYPE attribute.
			''' If this attributes is not defined, then
			''' then the type defaults to "disc" unless
			''' the tag is on Ordered list.  In the case
			''' of the latter, the default type is "decimal".
			''' </summary>
			Private Function getChildType(ByVal childView As View) As CSS.Value
				Dim ___childtype As CSS.Value = CType(childView.attributes.getAttribute(CSS.Attribute.LIST_STYLE_TYPE), CSS.Value)

				If ___childtype Is Nothing Then
					If type Is Nothing Then
						' Parent view.
						Dim v As View = childView.parent
						Dim doc As HTMLDocument = CType(v.document, HTMLDocument)
						If doc.matchNameAttribute(v.element.attributes, HTML.Tag.OL) Then
							___childtype = CSS.Value.DECIMAL
						Else
							___childtype = CSS.Value.DISC
						End If
					Else
						___childtype = type
					End If
				End If
				Return ___childtype
			End Function

			''' <summary>
			''' Obtains the starting index from <code>parent</code>.
			''' </summary>
			Private Sub getStart(ByVal parent As View)
				checkedForStart = True
				Dim element As Element = parent.element
				If element IsNot Nothing Then
					Dim attr As AttributeSet = element.attributes
					Dim startValue As Object
					startValue = attr.getAttribute(HTML.Attribute.START)
					If attr IsNot Nothing AndAlso attr.isDefined(HTML.Attribute.START) AndAlso startValue IsNot Nothing AndAlso (TypeOf startValue Is String) Then

						Try
							start = Convert.ToInt32(CStr(startValue))
						Catch nfe As NumberFormatException
						End Try
					End If
				End If
			End Sub

			''' <summary>
			''' Returns an integer that should be used to render the child at
			''' <code>childIndex</code> with. The retValue will usually be
			''' <code>childIndex</code> + 1, unless <code>parentView</code>
			''' has some Views that do not represent LI's, or one of the views
			''' has a HTML.Attribute.START specified.
			''' </summary>
			Private Function getRenderIndex(ByVal parentView As View, ByVal childIndex As Integer) As Integer
				If Not checkedForStart Then getStart(parentView)
				Dim retIndex As Integer = childIndex
				For counter As Integer = childIndex To 0 Step -1
					Dim [as] As AttributeSet = parentView.element.getElement(counter).attributes
					If [as].getAttribute(StyleConstants.NameAttribute) IsNot HTML.Tag.LI Then
						retIndex -= 1
					ElseIf [as].isDefined(HTML.Attribute.VALUE) Then
						Dim value As Object = [as].getAttribute(HTML.Attribute.VALUE)
						If value IsNot Nothing AndAlso (TypeOf value Is String) Then
							Try
								Dim iValue As Integer = Convert.ToInt32(CStr(value))
								Return retIndex - counter + iValue
							Catch nfe As NumberFormatException
							End Try
						End If
					End If
				Next counter
				Return retIndex + start
			End Function

			''' <summary>
			''' Paints the CSS list decoration according to the
			''' attributes given.
			''' </summary>
			''' <param name="g"> the rendering surface. </param>
			''' <param name="x"> the x coordinate of the list item allocation </param>
			''' <param name="y"> the y coordinate of the list item allocation </param>
			''' <param name="w"> the width of the list item allocation </param>
			''' <param name="h"> the height of the list item allocation </param>
			''' <param name="v"> the allocated area to paint into. </param>
			''' <param name="item"> which list item is being painted.  This
			'''  is a number greater than or equal to 0. </param>
			Public Overridable Sub paint(ByVal g As Graphics, ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal v As View, ByVal item As Integer)
				Dim cv As View = v.getView(item)
				Dim host As Container = v.container
				Dim name As Object = cv.element.attributes.getAttribute(StyleConstants.NameAttribute)
				' Only draw something if the View is a list item. This won't
				' be the case for comments.
				If Not(TypeOf name Is HTML.Tag) OrElse name IsNot HTML.Tag.LI Then Return
				' deside on what side draw bullets, etc.
				isLeftToRight = host.componentOrientation.leftToRight

				' How the list indicator is aligned is not specified, it is
				' left up to the UA. IE and NS differ on this behavior.
				' This is closer to NS where we align to the first line of text.
				' If the child is not text we draw the indicator at the
				' origin (0).
				Dim align As Single = 0
				If cv.viewCount > 0 Then
					Dim pView As View = cv.getView(0)
					Dim cName As Object = pView.element.attributes.getAttribute(StyleConstants.NameAttribute)
					If (cName Is HTML.Tag.P OrElse cName Is HTML.Tag.IMPLIED) AndAlso pView.viewCount > 0 Then
						paintRect.boundsnds(CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)))
						Dim shape As Shape = cv.getChildAllocation(0, paintRect)
						shape = pView.getView(0).getChildAllocation(0, shape)
						If shape IsNot Nothing AndAlso shape IsNot Nothing Then
							Dim rect As Rectangle = If(TypeOf shape Is Rectangle, CType(shape, Rectangle), shape.bounds)

							align = pView.getView(0).getAlignment(View.Y_AXIS)
							y = rect.y
							h = rect.height
						End If
					End If
				End If

				' set the color of a decoration
				Dim c As Color = (If(host.enabled, (If(ss IsNot Nothing, ss.getForeground(cv.attributes), host.foreground)), javax.swing.UIManager.getColor("textInactiveText")))
				g.color = c

				If img IsNot Nothing Then
					drawIcon(g, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align, host)
					Return
				End If
				Dim ___childtype As CSS.Value = getChildType(cv)
				Dim font As Font = CType(cv.document, StyledDocument).getFont(cv.attributes)
				If font IsNot Nothing Then g.font = font
				If ___childtype Is CSS.Value.SQUARE OrElse ___childtype Is CSS.Value.CIRCLE OrElse ___childtype Is CSS.Value.DISC Then
					drawShape(g, ___childtype, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align)
				ElseIf ___childtype Is CSS.Value.DECIMAL Then
					drawLetter(g, "1"c, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align, getRenderIndex(v, item))
				ElseIf ___childtype Is CSS.Value.LOWER_ALPHA Then
					drawLetter(g, "a"c, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align, getRenderIndex(v, item))
				ElseIf ___childtype Is CSS.Value.UPPER_ALPHA Then
					drawLetter(g, "A"c, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align, getRenderIndex(v, item))
				ElseIf ___childtype Is CSS.Value.LOWER_ROMAN Then
					drawLetter(g, "i"c, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align, getRenderIndex(v, item))
				ElseIf ___childtype Is CSS.Value.UPPER_ROMAN Then
					drawLetter(g, "I"c, CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)), align, getRenderIndex(v, item))
				End If
			End Sub

			''' <summary>
			''' Draws the bullet icon specified by the list-style-image argument.
			''' </summary>
			''' <param name="g">     the graphics context </param>
			''' <param name="ax">    x coordinate to place the bullet </param>
			''' <param name="ay">    y coordinate to place the bullet </param>
			''' <param name="aw">    width of the container the bullet is placed in </param>
			''' <param name="ah">    height of the container the bullet is placed in </param>
			''' <param name="align"> preferred alignment factor for the child view </param>
			Friend Overridable Sub drawIcon(ByVal g As Graphics, ByVal ax As Integer, ByVal ay As Integer, ByVal aw As Integer, ByVal ah As Integer, ByVal align As Single, ByVal c As Component)
				' Align to bottom of icon.
				Dim gap As Integer = If(isLeftToRight, - (img.iconWidth + bulletgap), (aw + bulletgap))
				Dim x As Integer = ax + gap
				Dim y As Integer = Math.Max(ay, ay + CInt(Fix(align * ah)) -img.iconHeight)

				img.paintIcon(c, g, x, y)
			End Sub

			''' <summary>
			''' Draws the graphical bullet item specified by the type argument.
			''' </summary>
			''' <param name="g">     the graphics context </param>
			''' <param name="type">  type of bullet to draw (circle, square, disc) </param>
			''' <param name="ax">    x coordinate to place the bullet </param>
			''' <param name="ay">    y coordinate to place the bullet </param>
			''' <param name="aw">    width of the container the bullet is placed in </param>
			''' <param name="ah">    height of the container the bullet is placed in </param>
			''' <param name="align"> preferred alignment factor for the child view </param>
			Friend Overridable Sub drawShape(ByVal g As Graphics, ByVal type As CSS.Value, ByVal ax As Integer, ByVal ay As Integer, ByVal aw As Integer, ByVal ah As Integer, ByVal align As Single)
				' Align to bottom of shape.
				Dim gap As Integer = If(isLeftToRight, - (bulletgap + 8), (aw + bulletgap))
				Dim x As Integer = ax + gap
				Dim y As Integer = Math.Max(ay, ay + CInt(Fix(align * ah)) - 8)

				If type Is CSS.Value.SQUARE Then
					g.drawRect(x, y, 8, 8)
				ElseIf type Is CSS.Value.CIRCLE Then
					g.drawOval(x, y, 8, 8)
				Else
					g.fillOval(x, y, 8, 8)
				End If
			End Sub

			''' <summary>
			''' Draws the letter or number for an ordered list.
			''' </summary>
			''' <param name="g">     the graphics context </param>
			''' <param name="letter"> type of ordered list to draw </param>
			''' <param name="ax">    x coordinate to place the bullet </param>
			''' <param name="ay">    y coordinate to place the bullet </param>
			''' <param name="aw">    width of the container the bullet is placed in </param>
			''' <param name="ah">    height of the container the bullet is placed in </param>
			''' <param name="index"> position of the list item in the list </param>
			Friend Overridable Sub drawLetter(ByVal g As Graphics, ByVal letter As Char, ByVal ax As Integer, ByVal ay As Integer, ByVal aw As Integer, ByVal ah As Integer, ByVal align As Single, ByVal index As Integer)
				Dim str As String = formatItemNum(index, letter)
				str = If(isLeftToRight, str & ".", "." & str)
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(Nothing, g)
				Dim stringwidth As Integer = sun.swing.SwingUtilities2.stringWidth(Nothing, fm, str)
				Dim gap As Integer = If(isLeftToRight, - (stringwidth + bulletgap), (aw + bulletgap))
				Dim x As Integer = ax + gap
				Dim y As Integer = Math.Max(ay + fm.ascent, ay + CInt(Fix(ah * align)))
				sun.swing.SwingUtilities2.drawString(Nothing, g, str, x, y)
			End Sub

			''' <summary>
			''' Converts the item number into the ordered list number
			''' (i.e.  1 2 3, i ii iii, a b c, etc.
			''' </summary>
			''' <param name="itemNum"> number to format </param>
			''' <param name="type">    type of ordered list </param>
			Friend Overridable Function formatItemNum(ByVal itemNum As Integer, ByVal type As Char) As String
				Dim numStyle As String = "1"

				Dim uppercase As Boolean = False

				Dim formattedNum As String

				Select Case type
				Case Else
					formattedNum = Convert.ToString(itemNum)

				Case "A"c
					uppercase = True
					' fall through
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case "a"c
					formattedNum = formatAlphaNumerals(itemNum)

				Case "I"c
					uppercase = True
					' fall through
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case "i"c
					formattedNum = formatRomanNumerals(itemNum)
				End Select

				If uppercase Then formattedNum = formattedNum.ToUpper()

				Return formattedNum
			End Function

			''' <summary>
			''' Converts the item number into an alphabetic character
			''' </summary>
			''' <param name="itemNum"> number to format </param>
			Friend Overridable Function formatAlphaNumerals(ByVal itemNum As Integer) As String
				Dim result As String

				If itemNum > 26 Then
					result = formatAlphaNumerals(itemNum \ 26) + formatAlphaNumerals(itemNum Mod 26)
				Else
					' -1 because item is 1 based.
					result = Convert.ToString(ChrW(AscW("a"c) + itemNum - 1))
				End If

				Return result
			End Function

			' list of roman numerals 
			Friend Shared ReadOnly romanChars As Char()() = { New Char() { "i"c, "v"c }, New Char() { "x"c, "l"c }, New Char() { "c"c, "d"c }, New Char() { "m"c, "?"c } }

			''' <summary>
			''' Converts the item number into a roman numeral
			''' </summary>
			''' <param name="num">  number to format </param>
			Friend Overridable Function formatRomanNumerals(ByVal num As Integer) As String
				Return formatRomanNumerals(0, num)
			End Function

			''' <summary>
			''' Converts the item number into a roman numeral
			''' </summary>
			''' <param name="num">  number to format </param>
			Friend Overridable Function formatRomanNumerals(ByVal level As Integer, ByVal num As Integer) As String
				If num < 10 Then
					Return formatRomanDigit(level, num)
				Else
					Return formatRomanNumerals(level + 1, num \ 10) + formatRomanDigit(level, num Mod 10)
				End If
			End Function


			''' <summary>
			''' Converts the item number into a roman numeral
			''' </summary>
			''' <param name="level"> position </param>
			''' <param name="digit"> digit to format </param>
			Friend Overridable Function formatRomanDigit(ByVal level As Integer, ByVal digit As Integer) As String
				Dim result As String = ""
				If digit = 9 Then
					result = result + AscW(romanChars(level)(0))
					result = result + AscW(romanChars(level + 1)(0))
					Return result
				ElseIf digit = 4 Then
					result = result + AscW(romanChars(level)(0))
					result = result + AscW(romanChars(level)(1))
					Return result
				ElseIf digit >= 5 Then
					result = result + AscW(romanChars(level)(1))
					digit -= 5
				End If

				For i As Integer = 0 To digit - 1
					result = result + AscW(romanChars(level)(0))
				Next i

				Return result
			End Function

			Private paintRect As Rectangle
			Private checkedForStart As Boolean
			Private start As Integer
			Private type As CSS.Value
			Friend imageurl As URL
			Private ss As StyleSheet = Nothing
			Friend img As javax.swing.Icon = Nothing
			Private bulletgap As Integer = 5
			Private isLeftToRight As Boolean
		End Class


		''' <summary>
		''' Paints the background image.
		''' </summary>
		<Serializable> _
		Friend Class BackgroundImagePainter
			Friend backgroundImage As javax.swing.ImageIcon
			Friend hPosition As Single
			Friend vPosition As Single
			' bit mask: 0 for repeat x, 1 for repeat y, 2 for horiz relative,
			' 3 for vert relative
			Friend flags As Short
			' These are used when painting, updatePaintCoordinates updates them.
			Private paintX As Integer
			Private paintY As Integer
			Private paintMaxX As Integer
			Private paintMaxY As Integer

			Friend Sub New(ByVal a As AttributeSet, ByVal css As CSS, ByVal ss As StyleSheet)
				backgroundImage = ss.getBackgroundImage(a)
				' Determine the position.
				Dim pos As CSS.BackgroundPosition = CType(a.getAttribute(CSS.Attribute.BACKGROUND_POSITION), CSS.BackgroundPosition)
				If pos IsNot Nothing Then
					hPosition = pos.horizontalPosition
					vPosition = pos.verticalPosition
					If pos.horizontalPositionRelativeToSize Then
						flags = flags Or 4
					ElseIf pos.horizontalPositionRelativeToSize Then
						hPosition *= css.getFontSize(a, 12, ss)
					End If
					If pos.verticalPositionRelativeToSize Then
						flags = flags Or 8
					ElseIf pos.verticalPositionRelativeToFontSize Then
						vPosition *= css.getFontSize(a, 12, ss)
					End If
				End If
				' Determine any repeating values.
				Dim repeats As CSS.Value = CType(a.getAttribute(CSS.Attribute.BACKGROUND_REPEAT), CSS.Value)
				If repeats Is Nothing OrElse repeats Is CSS.Value.BACKGROUND_REPEAT Then
					flags = flags Or 3
				ElseIf repeats Is CSS.Value.BACKGROUND_REPEAT_X Then
					flags = flags Or 1
				ElseIf repeats Is CSS.Value.BACKGROUND_REPEAT_Y Then
					flags = flags Or 2
				End If
			End Sub

			Friend Overridable Sub paint(ByVal g As Graphics, ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal v As View)
				Dim clip As Rectangle = g.clipRect
				If clip IsNot Nothing Then g.clipRect(CInt(Fix(x)), CInt(Fix(y)), CInt(Fix(w)), CInt(Fix(h)))
				If (flags And 3) = 0 Then
					' no repeating
					Dim width As Integer = backgroundImage.iconWidth
					Dim height As Integer = backgroundImage.iconWidth
					If (flags And 4) = 4 Then
						paintX = CInt(Fix(x + w * hPosition - CSng(width) * hPosition))
					Else
						paintX = CInt(Fix(x)) + CInt(Fix(hPosition))
					End If
					If (flags And 8) = 8 Then
						paintY = CInt(Fix(y + h * vPosition - CSng(height) * vPosition))
					Else
						paintY = CInt(Fix(y)) + CInt(Fix(vPosition))
					End If
					If clip Is Nothing OrElse Not((paintX + width <= clip.x) OrElse (paintY + height <= clip.y) OrElse (paintX >= clip.x + clip.width) OrElse (paintY >= clip.y + clip.height)) Then backgroundImage.paintIcon(Nothing, g, paintX, paintY)
				Else
					Dim width As Integer = backgroundImage.iconWidth
					Dim height As Integer = backgroundImage.iconHeight
					If width > 0 AndAlso height > 0 Then
						paintX = CInt(Fix(x))
						paintY = CInt(Fix(y))
						paintMaxX = CInt(Fix(x + w))
						paintMaxY = CInt(Fix(y + h))
						If updatePaintCoordinates(clip, width, height) Then
							Do While paintX < paintMaxX
								Dim ySpot As Integer = paintY
								Do While ySpot < paintMaxY
									backgroundImage.paintIcon(Nothing, g, paintX, ySpot)
									ySpot += height
								Loop
								paintX += width
							Loop
						End If
					End If
				End If
				If clip IsNot Nothing Then g.cliplip(clip.x, clip.y, clip.width, clip.height)
			End Sub

			Private Function updatePaintCoordinates(ByVal clip As Rectangle, ByVal width As Integer, ByVal height As Integer) As Boolean
				If (flags And 3) = 1 Then
					paintMaxY = paintY + 1
				ElseIf (flags And 3) = 2 Then
					paintMaxX = paintX + 1
				End If
				If clip IsNot Nothing Then
					If (flags And 3) = 1 AndAlso ((paintY + height <= clip.y) OrElse (paintY > clip.y + clip.height)) Then Return False
					If (flags And 3) = 2 AndAlso ((paintX + width <= clip.x) OrElse (paintX > clip.x + clip.width)) Then Return False
					If (flags And 1) = 1 Then
						If (clip.x + clip.width) < paintMaxX Then
							If (clip.x + clip.width - paintX) Mod width = 0 Then
								paintMaxX = clip.x + clip.width
							Else
								paintMaxX = ((clip.x + clip.width - paintX) / width + 1) * width + paintX
							End If
						End If
						If clip.x > paintX Then paintX = (clip.x - paintX) / width * width + paintX
					End If
					If (flags And 2) = 2 Then
						If (clip.y + clip.height) < paintMaxY Then
							If (clip.y + clip.height - paintY) Mod height = 0 Then
								paintMaxY = clip.y + clip.height
							Else
								paintMaxY = ((clip.y + clip.height - paintY) / height + 1) * height + paintY
							End If
						End If
						If clip.y > paintY Then paintY = (clip.y - paintY) / height * height + paintY
					End If
				End If
				' Valid
				Return True
			End Function
		End Class


		''' <summary>
		''' A subclass of MuxingAttributeSet that translates between
		''' CSS and HTML and StyleConstants. The AttributeSets used are
		''' the CSS rules that match the Views Elements.
		''' </summary>
		Friend Class ViewAttributeSet
			Inherits MuxingAttributeSet

			Private ReadOnly outerInstance As StyleSheet

			Friend Sub New(ByVal outerInstance As StyleSheet, ByVal v As View)
					Me.outerInstance = outerInstance
				host = v

				' PENDING(prinz) fix this up to be a more realistic
				' implementation.
				Dim doc As Document = v.document
				Dim sb As SearchBuffer = SearchBuffer.obtainSearchBuffer()
				Dim muxList As List(Of AttributeSet) = sb.vector
				Try
					If TypeOf doc Is HTMLDocument Then
						Dim styles As StyleSheet = StyleSheet.this
						Dim elem As Element = v.element
						Dim a As AttributeSet = elem.attributes
						Dim htmlAttr As AttributeSet = styles.translateHTMLToCSS(a)

						If htmlAttr.attributeCount <> 0 Then muxList.Add(htmlAttr)
						If elem.leaf Then
							Dim keys As System.Collections.IEnumerator = a.attributeNames
							Do While keys.hasMoreElements()
								Dim key As Object = keys.nextElement()
								If TypeOf key Is HTML.Tag Then
									If key Is HTML.Tag.A Then
										Dim o As Object = a.getAttribute(key)
									''' <summary>
									'''   In the case of an A tag, the css rules
									'''   apply only for tags that have their
									'''   href attribute defined and not for
									'''   anchors that only have their name attributes
									'''   defined, i.e anchors that function as
									'''   destinations.  Hence we do not add the
									'''   attributes for that latter kind of
									'''   anchors.  When CSS2 support is added,
									'''   it will be possible to specificity this
									'''   kind of conditional behaviour in the
									'''   stylesheet.
									''' 
									''' </summary>
										If o IsNot Nothing AndAlso TypeOf o Is AttributeSet Then
											Dim attr As AttributeSet = CType(o, AttributeSet)
											If attr.getAttribute(HTML.Attribute.HREF) Is Nothing Then Continue Do
										End If
									End If
									Dim cssRule As AttributeSet = styles.getRule(CType(key, HTML.Tag), elem)
									If cssRule IsNot Nothing Then muxList.Add(cssRule)
								End If
							Loop
						Else
							Dim t As HTML.Tag = CType(a.getAttribute(StyleConstants.NameAttribute), HTML.Tag)
							Dim cssRule As AttributeSet = styles.getRule(t, elem)
							If cssRule IsNot Nothing Then muxList.Add(cssRule)
						End If
					End If
					Dim attrs As AttributeSet() = New AttributeSet(muxList.Count - 1){}
					muxList.CopyTo(attrs)
					attributes = attrs
				Finally
					SearchBuffer.releaseSearchBuffer(sb)
				End Try
			End Sub

			'  --- AttributeSet methods ----------------------------

			''' <summary>
			''' Checks whether a given attribute is defined.
			''' This will convert the key over to CSS if the
			''' key is a StyleConstants key that has a CSS
			''' mapping.
			''' </summary>
			''' <param name="key"> the attribute key </param>
			''' <returns> true if the attribute is defined </returns>
			''' <seealso cref= AttributeSet#isDefined </seealso>
			Public Overrides Function isDefined(ByVal key As Object) As Boolean
				If TypeOf key Is StyleConstants Then
					Dim cssKey As Object = outerInstance.css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then key = cssKey
				End If
				Return MyBase.isDefined(key)
			End Function

			''' <summary>
			''' Gets the value of an attribute.  If the requested
			''' attribute is a StyleConstants attribute that has
			''' a CSS mapping, the request will be converted.
			''' </summary>
			''' <param name="key"> the attribute name </param>
			''' <returns> the attribute value </returns>
			''' <seealso cref= AttributeSet#getAttribute </seealso>
			Public Overrides Function getAttribute(ByVal key As Object) As Object
				If TypeOf key Is StyleConstants Then
					Dim cssKey As Object = outerInstance.css.styleConstantsKeyToCSSKey(CType(key, StyleConstants))
					If cssKey IsNot Nothing Then
						Dim value As Object = doGetAttribute(cssKey)
						If TypeOf value Is CSS.CssValue Then Return CType(value, CSS.CssValue).toStyleConstants(CType(key, StyleConstants), host)
					End If
				End If
				Return doGetAttribute(key)
			End Function

			Friend Overridable Function doGetAttribute(ByVal key As Object) As Object
				Dim retValue As Object = MyBase.getAttribute(key)
				If retValue IsNot Nothing Then Return retValue
				' didn't find it... try parent if it's a css attribute
				' that is inherited.
				If TypeOf key Is CSS.Attribute Then
					Dim css As CSS.Attribute = CType(key, CSS.Attribute)
					If css.inherited Then
						Dim parent As AttributeSet = resolveParent
						If parent IsNot Nothing Then Return parent.getAttribute(key)
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' If not overriden, the resolving parent defaults to
			''' the parent element.
			''' </summary>
			''' <returns> the attributes from the parent </returns>
			''' <seealso cref= AttributeSet#getResolveParent </seealso>
			Public Property Overrides resolveParent As AttributeSet
				Get
					If host Is Nothing Then Return Nothing
					Dim parent As View = host.parent
					Return If(parent IsNot Nothing, parent.attributes, Nothing)
				End Get
			End Property

			''' <summary>
			''' View created for. </summary>
			Friend host As View
		End Class


		''' <summary>
		''' A subclass of MuxingAttributeSet that implements Style. Currently
		''' the MutableAttributeSet methods are unimplemented, that is they
		''' do nothing.
		''' </summary>
		' PENDING(sky): Decide what to do with this. Either make it
		' contain a SimpleAttributeSet that modify methods are delegated to,
		' or change getRule to return an AttributeSet and then don't make this
		' implement Style.
		<Serializable> _
		Friend Class ResolvedStyle
			Inherits MuxingAttributeSet
			Implements Style

			Friend Sub New(ByVal name As String, ByVal attrs As AttributeSet(), ByVal extendedIndex As Integer)
				MyBase.New(attrs)
				Me.name = name
				Me.extendedIndex = extendedIndex
			End Sub

			''' <summary>
			''' Inserts a Style into the receiver so that the styles the
			''' receiver represents are still ordered by specificity.
			''' <code>style</code> will be added before any extended styles, that
			''' is before extendedIndex.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub insertStyle(ByVal style As Style, ByVal specificity As Integer)
				Dim attrs As AttributeSet() = attributes
				Dim maxCounter As Integer = attrs.Length
				Dim counter As Integer = 0
				Do While counter < extendedIndex
					If specificity > getSpecificity(CType(attrs(counter), Style).name) Then Exit Do
					counter += 1
				Loop
				insertAttributeSetAt(style, counter)
				extendedIndex += 1
			End Sub

			''' <summary>
			''' Removes a previously added style. This will do nothing if
			''' <code>style</code> is not referenced by the receiver.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub removeStyle(ByVal style As Style)
				Dim attrs As AttributeSet() = attributes

				For counter As Integer = attrs.Length - 1 To 0 Step -1
					If attrs(counter) Is style Then
						removeAttributeSetAt(counter)
						If counter < extendedIndex Then extendedIndex -= 1
						Exit For
					End If
				Next counter
			End Sub

			''' <summary>
			''' Adds <code>s</code> as one of the Attributesets to look up
			''' attributes in.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub insertExtendedStyleAt(ByVal attr As Style, ByVal index As Integer)
				insertAttributeSetAt(attr, extendedIndex + index)
			End Sub

			''' <summary>
			''' Adds <code>s</code> as one of the AttributeSets to look up
			''' attributes in. It will be the AttributeSet last checked.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub addExtendedStyle(ByVal attr As Style)
				insertAttributeSetAt(attr, attributes.length)
			End Sub

			''' <summary>
			''' Removes the style at <code>index</code> +
			''' <code>extendedIndex</code>.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub removeExtendedStyleAt(ByVal index As Integer)
				removeAttributeSetAt(extendedIndex + index)
			End Sub

			''' <summary>
			''' Returns true if the receiver matches <code>selector</code>, where
			''' a match is defined by the CSS rule matching.
			''' Each simple selector must be separated by a single space.
			''' </summary>
			Protected Friend Overridable Function matches(ByVal selector As String) As Boolean
				Dim sLast As Integer = selector.Length

				If sLast = 0 Then Return False
				Dim thisLast As Integer = name.Length
				Dim sCurrent As Integer = selector.LastIndexOf(" "c)
				Dim thisCurrent As Integer = name.LastIndexOf(" "c)
				If sCurrent >= 0 Then sCurrent += 1
				If thisCurrent >= 0 Then thisCurrent += 1
				If Not matches(selector, sCurrent, sLast, thisCurrent, thisLast) Then Return False
				Do While sCurrent <> -1
					sLast = sCurrent - 1
					sCurrent = selector.LastIndexOf(" "c, sLast - 1)
					If sCurrent >= 0 Then sCurrent += 1
					Dim match As Boolean = False
					Do While (Not match) AndAlso thisCurrent <> -1
						thisLast = thisCurrent - 1
						thisCurrent = name.LastIndexOf(" "c, thisLast - 1)
						If thisCurrent >= 0 Then thisCurrent += 1
						match = matches(selector, sCurrent, sLast, thisCurrent, thisLast)
					Loop
					If Not match Then Return False
				Loop
				Return True
			End Function

			''' <summary>
			''' Returns true if the substring of the receiver, in the range
			''' thisCurrent, thisLast matches the substring of selector in
			''' the ranme sCurrent to sLast based on CSS selector matching.
			''' </summary>
			Friend Overridable Function matches(ByVal selector As String, ByVal sCurrent As Integer, ByVal sLast As Integer, ByVal thisCurrent As Integer, ByVal thisLast As Integer) As Boolean
				sCurrent = Math.Max(sCurrent, 0)
				thisCurrent = Math.Max(thisCurrent, 0)
				Dim thisDotIndex As Integer = boundedIndexOf(name, "."c, thisCurrent, thisLast)
				Dim thisPoundIndex As Integer = boundedIndexOf(name, "#"c, thisCurrent, thisLast)
				Dim sDotIndex As Integer = boundedIndexOf(selector, "."c, sCurrent, sLast)
				Dim sPoundIndex As Integer = boundedIndexOf(selector, "#"c, sCurrent, sLast)
				If sDotIndex <> -1 Then
					' Selector has a '.', which indicates name must match it,
					' or if the '.' starts the selector than name must have
					' the same class (doesn't matter what element name).
					If thisDotIndex = -1 Then Return False
					If sCurrent = sDotIndex Then
						If (thisLast - thisDotIndex) <> (sLast - sDotIndex) OrElse (Not selector.regionMatches(sCurrent, name, thisDotIndex, (thisLast - thisDotIndex))) Then Return False
					Else
						' Has to fully match.
						If (sLast - sCurrent) <> (thisLast - thisCurrent) OrElse (Not selector.regionMatches(sCurrent, name, thisCurrent, (thisLast - thisCurrent))) Then Return False
					End If
					Return True
				End If
				If sPoundIndex <> -1 Then
					' Selector has a '#', which indicates name must match it,
					' or if the '#' starts the selector than name must have
					' the same id (doesn't matter what element name).
					If thisPoundIndex = -1 Then Return False
					If sCurrent = sPoundIndex Then
						If (thisLast - thisPoundIndex) <>(sLast - sPoundIndex) OrElse (Not selector.regionMatches(sCurrent, name, thisPoundIndex, (thisLast - thisPoundIndex))) Then Return False
					Else
						' Has to fully match.
						If (sLast - sCurrent) <> (thisLast - thisCurrent) OrElse (Not selector.regionMatches(sCurrent, name, thisCurrent, (thisLast - thisCurrent))) Then Return False
					End If
					Return True
				End If
				If thisDotIndex <> -1 Then Return (((thisDotIndex - thisCurrent) = (sLast - sCurrent)) AndAlso selector.regionMatches(sCurrent, name, thisCurrent, thisDotIndex - thisCurrent))
				If thisPoundIndex <> -1 Then Return (((thisPoundIndex - thisCurrent) =(sLast - sCurrent)) AndAlso selector.regionMatches(sCurrent, name, thisCurrent, thisPoundIndex - thisCurrent))
				' Fail through, no classes or ides, just check string.
				Return (((thisLast - thisCurrent) = (sLast - sCurrent)) AndAlso selector.regionMatches(sCurrent, name, thisCurrent, thisLast - thisCurrent))
			End Function

			''' <summary>
			''' Similar to String.indexOf, but allows an upper bound
			''' (this is slower in that it will still check string starting at
			''' start.
			''' </summary>
			Friend Overridable Function boundedIndexOf(ByVal [string] As String, ByVal search As Char, ByVal start As Integer, ByVal [end] As Integer) As Integer
				Dim retValue As Integer = [string].IndexOf(search, start)
				If retValue >= [end] Then Return -1
				Return retValue
			End Function

			Public Overridable Sub addAttribute(ByVal name As Object, ByVal value As Object) Implements MutableAttributeSet.addAttribute
			End Sub
			Public Overridable Sub addAttributes(ByVal attributes As AttributeSet) Implements MutableAttributeSet.addAttributes
			End Sub
			Public Overridable Sub removeAttribute(ByVal name As Object) Implements MutableAttributeSet.removeAttribute
			End Sub
			Public Overridable Sub removeAttributes(Of T1)(ByVal names As System.Collections.IEnumerator(Of T1))
			End Sub
			Public Overridable Sub removeAttributes(ByVal attributes As AttributeSet) Implements MutableAttributeSet.removeAttributes
			End Sub
			Public Overridable Property resolveParent Implements MutableAttributeSet.setResolveParent As AttributeSet
				Set(ByVal parent As AttributeSet)
				End Set
			End Property
			Public Overridable Property name As String Implements Style.getName
				Get
					Return name
				End Get
			End Property
			Public Overridable Sub addChangeListener(ByVal l As javax.swing.event.ChangeListener) Implements Style.addChangeListener
			End Sub
			Public Overridable Sub removeChangeListener(ByVal l As javax.swing.event.ChangeListener) Implements Style.removeChangeListener
			End Sub
			Public Overridable Property changeListeners As javax.swing.event.ChangeListener()
				Get
					Return New javax.swing.event.ChangeListener(){}
				End Get
			End Property

			''' <summary>
			''' The name of the Style, which is the selector.
			''' This will NEVER change!
			''' </summary>
			Friend name As String
			''' <summary>
			''' Start index of styles coming from other StyleSheets. </summary>
			Private extendedIndex As Integer
		End Class


		''' <summary>
		''' SelectorMapping contains a specifitiy, as an integer, and an associated
		''' Style. It can also reference children <code>SelectorMapping</code>s,
		''' so that it behaves like a tree.
		''' <p>
		''' This is not thread safe, it is assumed the caller will take the
		''' necessary precations if this is to be used in a threaded environment.
		''' </summary>
		<Serializable> _
		Friend Class SelectorMapping
			Public Sub New(ByVal specificity As Integer)
				Me.specificity = specificity
			End Sub

			''' <summary>
			''' Returns the specificity this mapping represents.
			''' </summary>
			Public Overridable Property specificity As Integer
				Get
					Return specificity
				End Get
			End Property

			''' <summary>
			''' Sets the Style associated with this mapping.
			''' </summary>
			Public Overridable Property style As Style
				Set(ByVal style As Style)
					Me.style = style
				End Set
				Get
					Return style
				End Get
			End Property


			''' <summary>
			''' Returns the child mapping identified by the simple selector
			''' <code>selector</code>. If a child mapping does not exist for
			''' <code>selector</code>, and <code>create</code> is true, a new
			''' one will be created.
			''' </summary>
			Public Overridable Function getChildSelectorMapping(ByVal selector As String, ByVal create As Boolean) As SelectorMapping
				Dim retValue As SelectorMapping = Nothing

				If children IsNot Nothing Then
					retValue = children(selector)
				ElseIf create Then
					children = New Dictionary(Of String, SelectorMapping)(7)
				End If
				If retValue Is Nothing AndAlso create Then
					Dim ___specificity As Integer = getChildSpecificity(selector)

					retValue = createChildSelectorMapping(___specificity)
					children(selector) = retValue
				End If
				Return retValue
			End Function

			''' <summary>
			''' Creates a child <code>SelectorMapping</code> with the specified
			''' <code>specificity</code>.
			''' </summary>
			Protected Friend Overridable Function createChildSelectorMapping(ByVal specificity As Integer) As SelectorMapping
				Return New SelectorMapping(specificity)
			End Function

			''' <summary>
			''' Returns the specificity for the child selector
			''' <code>selector</code>.
			''' </summary>
			Protected Friend Overridable Function getChildSpecificity(ByVal selector As String) As Integer
				' class (.) 100
				' id (#)    10000
				Dim firstChar As Char = selector.Chars(0)
				Dim ___specificity As Integer = specificity

				If firstChar = "."c Then
					___specificity += 100
				ElseIf firstChar = "#"c Then
					___specificity += 10000
				Else
					___specificity += 1
					If selector.IndexOf("."c) <> -1 Then ___specificity += 100
					If selector.IndexOf("#"c) <> -1 Then ___specificity += 10000
				End If
				Return ___specificity
			End Function

			''' <summary>
			''' The specificity for this selector.
			''' </summary>
			Private specificity As Integer
			''' <summary>
			''' Style for this selector.
			''' </summary>
			Private style As Style
			''' <summary>
			''' Any sub selectors. Key will be String, and value will be
			''' another SelectorMapping.
			''' </summary>
			Private children As Dictionary(Of String, SelectorMapping)
		End Class


		' ---- Variables ---------------------------------------------

		Friend Const DEFAULT_FONT_SIZE As Integer = 3

		Private css As CSS

		''' <summary>
		''' An inverted graph of the selectors.
		''' </summary>
		Private selectorMapping As SelectorMapping

		''' <summary>
		''' Maps from selector (as a string) to Style that includes all
		''' relevant styles. 
		''' </summary>
		Private resolvedStyles As Dictionary(Of String, ResolvedStyle)

		''' <summary>
		''' Vector of StyleSheets that the rules are to reference.
		''' </summary>
		Private linkedStyleSheets As List(Of StyleSheet)

		''' <summary>
		''' Where the style sheet was found. Used for relative imports. </summary>
		Private base As URL


		''' <summary>
		''' Default parser for CSS specifications that get loaded into
		''' the StyleSheet.<p>
		''' This class is NOT thread safe, do not ask it to parse while it is
		''' in the middle of parsing.
		''' </summary>
		Friend Class CssParser
			Implements CSSParser.CSSParserCallback

			Private ReadOnly outerInstance As StyleSheet

			Public Sub New(ByVal outerInstance As StyleSheet)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Parses the passed in CSS declaration into an AttributeSet.
			''' </summary>
			Public Overridable Function parseDeclaration(ByVal [string] As String) As AttributeSet
				Try
					Return parseDeclaration(New StringReader([string]))
				Catch ioe As IOException
				End Try
				Return Nothing
			End Function

			''' <summary>
			''' Parses the passed in CSS declaration into an AttributeSet.
			''' </summary>
			Public Overridable Function parseDeclaration(ByVal r As Reader) As AttributeSet
				parse(base, r, True, False)
				Return declaration.copyAttributes()
			End Function

			''' <summary>
			''' Parse the given CSS stream
			''' </summary>
			Public Overridable Sub parse(ByVal base As URL, ByVal r As Reader, ByVal parseDeclaration As Boolean, ByVal isLink As Boolean)
				Me.base = base
				Me.isLink = isLink
				Me.parsingDeclaration = parseDeclaration
				declaration.removeAttributes(declaration)
				selectorTokens.Clear()
				selectors.Clear()
				propertyName = Nothing
				parser.parse(r, Me, parseDeclaration)
			End Sub

			'
			' CSSParserCallback methods, public to implement the interface.
			'

			''' <summary>
			''' Invoked when a valid @import is encountered, will call
			''' <code>importStyleSheet</code> if a
			''' <code>MalformedURLException</code> is not thrown in creating
			''' the URL.
			''' </summary>
			Public Overridable Sub handleImport(ByVal importString As String) Implements CSSParser.CSSParserCallback.handleImport
				Dim url As URL = CSS.getURL(base, importString)
				If url IsNot Nothing Then outerInstance.importStyleSheet(url)
			End Sub

			''' <summary>
			''' A selector has been encountered.
			''' </summary>
			Public Overridable Sub handleSelector(ByVal selector As String) Implements CSSParser.CSSParserCallback.handleSelector
				'class and index selectors are case sensitive
				If Not(selector.StartsWith(".") OrElse selector.StartsWith("#")) Then selector = selector.ToLower()
				Dim length As Integer = selector.Length

				If selector.EndsWith(",") Then
					If length > 1 Then
						selector = selector.Substring(0, length - 1)
						selectorTokens.Add(selector)
					End If
					addSelector()
				ElseIf length > 0 Then
					selectorTokens.Add(selector)
				End If
			End Sub

			''' <summary>
			''' Invoked when the start of a rule is encountered.
			''' </summary>
			Public Overridable Sub startRule() Implements CSSParser.CSSParserCallback.startRule
				If selectorTokens.Count > 0 Then addSelector()
				propertyName = Nothing
			End Sub

			''' <summary>
			''' Invoked when a property name is encountered.
			''' </summary>
			Public Overridable Sub handleProperty(ByVal [property] As String) Implements CSSParser.CSSParserCallback.handleProperty
				propertyName = [property]
			End Sub

			''' <summary>
			''' Invoked when a property value is encountered.
			''' </summary>
			Public Overridable Sub handleValue(ByVal value As String) Implements CSSParser.CSSParserCallback.handleValue
				If propertyName IsNot Nothing AndAlso value IsNot Nothing AndAlso value.Length > 0 Then
					Dim cssKey As CSS.Attribute = CSS.getAttribute(propertyName)
					If cssKey IsNot Nothing Then
						' There is currently no mechanism to determine real
						' base that style sheet was loaded from. For the time
						' being, this maps for LIST_STYLE_IMAGE, which appear
						' to be the only one that currently matters. A more
						' general mechanism is definately needed.
						If cssKey Is CSS.Attribute.LIST_STYLE_IMAGE Then
							If value IsNot Nothing AndAlso (Not value.Equals("none")) Then
								Dim url As URL = CSS.getURL(base, value)

								If url IsNot Nothing Then value = url.ToString()
							End If
						End If
						outerInstance.addCSSAttribute(declaration, cssKey, value)
					End If
					propertyName = Nothing
				End If
			End Sub

			''' <summary>
			''' Invoked when the end of a rule is encountered.
			''' </summary>
			Public Overridable Sub endRule() Implements CSSParser.CSSParserCallback.endRule
				Dim n As Integer = selectors.Count
				For i As Integer = 0 To n - 1
					Dim selector As String() = selectors(i)
					If selector.Length > 0 Then outerInstance.addRule(selector, declaration, isLink)
				Next i
				declaration.removeAttributes(declaration)
				selectors.Clear()
			End Sub

			Private Sub addSelector()
				Dim selector As String() = New String(selectorTokens.Count - 1){}
				selectorTokens.CopyTo(selector)
				selectors.Add(selector)
				selectorTokens.Clear()
			End Sub


			Friend selectors As New List(Of String())
			Friend selectorTokens As New List(Of String)
			''' <summary>
			''' Name of the current property. </summary>
			Friend propertyName As String
			Friend declaration As MutableAttributeSet = New SimpleAttributeSet
			''' <summary>
			''' True if parsing a declaration, that is the Reader will not
			''' contain a selector. 
			''' </summary>
			Friend parsingDeclaration As Boolean
			''' <summary>
			''' True if the attributes are coming from a linked/imported style. </summary>
			Friend isLink As Boolean
			''' <summary>
			''' Where the CSS stylesheet lives. </summary>
			Friend base As URL
			Friend parser As New CSSParser
		End Class

		Friend Overridable Sub rebaseSizeMap(ByVal base As Integer)
			Const minimalFontSize As Integer = 4
			sizeMap = New Integer(sizeMapDefault.Length - 1){}
			For i As Integer = 0 To sizeMapDefault.Length - 1
				sizeMap(i) = Math.Max(base * sizeMapDefault(i) \ sizeMapDefault(CSS.baseFontSizeIndex), minimalFontSize)
			Next i

		End Sub

		Friend Overridable Property sizeMap As Integer()
			Get
				Return sizeMap
			End Get
		End Property
		Friend Overridable Property w3CLengthUnits As Boolean
			Get
				Return w3cLengthUnits
			End Get
		End Property

		''' <summary>
		''' The HTML/CSS size model has seven slots
		''' that one can assign sizes to.
		''' </summary>
		Friend Shared ReadOnly sizeMapDefault As Integer() = { 8, 10, 12, 14, 18, 24, 36 }

		Private sizeMap As Integer() = sizeMapDefault
		Private w3cLengthUnits As Boolean = False
	End Class

End Namespace
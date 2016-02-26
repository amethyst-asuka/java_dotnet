Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.text
Imports javax.accessibility

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


	'
	' * The AccessibleHTML class provide information about the contents
	' * of a HTML document to assistive technologies.
	' *
	' * @author  Lynn Monsanto
	' 
	Friend Class AccessibleHTML
		Implements Accessible

		''' <summary>
		''' The editor.
		''' </summary>
		Private editor As JEditorPane
		''' <summary>
		''' Current model.
		''' </summary>
		Private model As Document
		''' <summary>
		''' DocumentListener installed on the current model.
		''' </summary>
		Private docListener As DocumentListener
		''' <summary>
		''' PropertyChangeListener installed on the editor
		''' </summary>
		Private propChangeListener As PropertyChangeListener
		''' <summary>
		''' The root ElementInfo for the document
		''' </summary>
		Private rootElementInfo As ElementInfo
	'    
	'     * The root accessible context for the document
	'     
		Private rootHTMLAccessibleContext As RootHTMLAccessibleContext

		Public Sub New(ByVal pane As JEditorPane)
			editor = pane
			propChangeListener = New PropertyChangeHandler(Me)
			document = editor.document

			docListener = New DocumentHandler(Me)
		End Sub

		''' <summary>
		''' Sets the document.
		''' </summary>
		Private Property document As Document
			Set(ByVal document As Document)
				If model IsNot Nothing Then model.removeDocumentListener(docListener)
				If editor IsNot Nothing Then editor.removePropertyChangeListener(propChangeListener)
				Me.model = document
				If model IsNot Nothing Then
					If rootElementInfo IsNot Nothing Then rootElementInfo.invalidate(False)
					buildInfo()
					model.addDocumentListener(docListener)
				Else
					rootElementInfo = Nothing
				End If
				If editor IsNot Nothing Then editor.addPropertyChangeListener(propChangeListener)
			End Set
			Get
				Return model
			End Get
		End Property


		''' <summary>
		''' Returns the JEditorPane providing information for.
		''' </summary>
		Private Property textComponent As JEditorPane
			Get
				Return editor
			End Get
		End Property

		''' <summary>
		''' Returns the ElementInfo representing the root Element.
		''' </summary>
		Private Property rootInfo As ElementInfo
			Get
				Return rootElementInfo
			End Get
		End Property

		''' <summary>
		''' Returns the root <code>View</code> associated with the current text
		''' component.
		''' </summary>
		Private Property rootView As View
			Get
				Return textComponent.uI.getRootView(textComponent)
			End Get
		End Property

		''' <summary>
		''' Returns the bounds the root View will be rendered in.
		''' </summary>
		Private Property rootEditorRect As Rectangle
			Get
				Dim alloc As Rectangle = textComponent.bounds
				If (alloc.width > 0) AndAlso (alloc.height > 0) Then
						alloc.y = 0
						alloc.x = alloc.y
					Dim insets As Insets = editor.insets
					alloc.x += insets.left
					alloc.y += insets.top
					alloc.width -= insets.left + insets.right
					alloc.height -= insets.top + insets.bottom
					Return alloc
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' If possible acquires a lock on the Document.  If a lock has been
		''' obtained a key will be retured that should be passed to
		''' <code>unlock</code>.
		''' </summary>
		Private Function lock() As Object
			Dim ___document As Document = document

			If TypeOf ___document Is AbstractDocument Then
				CType(___document, AbstractDocument).readLock()
				Return ___document
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Releases a lock previously obtained via <code>lock</code>.
		''' </summary>
		Private Sub unlock(ByVal key As Object)
			If key IsNot Nothing Then CType(key, AbstractDocument).readUnlock()
		End Sub

		''' <summary>
		''' Rebuilds the information from the current info.
		''' </summary>
		Private Sub buildInfo()
			Dim lock As Object = lock()

			Try
				Dim doc As Document = document
				Dim root As Element = doc.defaultRootElement

				rootElementInfo = New ElementInfo(Me, root)
				rootElementInfo.validate()
			Finally
				unlock(lock)
			End Try
		End Sub

	'    
	'     * Create an ElementInfo subclass based on the passed in Element.
	'     
		Friend Overridable Function createElementInfo(ByVal e As Element, ByVal parent As ElementInfo) As ElementInfo
			Dim attrs As AttributeSet = e.attributes

			If attrs IsNot Nothing Then
				Dim name As Object = attrs.getAttribute(StyleConstants.NameAttribute)

				If name Is HTML.Tag.IMG Then
					Return New IconElementInfo(Me, e, parent)
				ElseIf name Is HTML.Tag.CONTENT OrElse name Is HTML.Tag.CAPTION Then
					Return New TextElementInfo(Me, e, parent)
				ElseIf name Is HTML.Tag.TABLE Then
					Return New TableElementInfo(Me, e, parent)
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the root AccessibleContext for the document
		''' </summary>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If rootHTMLAccessibleContext Is Nothing Then rootHTMLAccessibleContext = New RootHTMLAccessibleContext(Me, rootElementInfo)
				Return rootHTMLAccessibleContext
			End Get
		End Property

	'    
	'     * The roow AccessibleContext for the document
	'     
		Private Class RootHTMLAccessibleContext
			Inherits HTMLAccessibleContext

			Private ReadOnly outerInstance As AccessibleHTML


			Public Sub New(ByVal outerInstance As AccessibleHTML, ByVal elementInfo As ElementInfo)
					Me.outerInstance = outerInstance
				MyBase.New(elementInfo)
			End Sub

			''' <summary>
			''' Gets the accessibleName property of this object.  The accessibleName
			''' property of an object is a localized String that designates the purpose
			''' of the object.  For example, the accessibleName property of a label
			''' or button might be the text of the label or button itself.  In the
			''' case of an object that doesn't display its name, the accessibleName
			''' should still be set.  For example, in the case of a text field used
			''' to enter the name of a city, the accessibleName for the en_US locale
			''' could be 'city.'
			''' </summary>
			''' <returns> the localized name of the object; null if this
			''' object does not have a name
			''' </returns>
			''' <seealso cref= #setAccessibleName </seealso>
			Public Property Overrides accessibleName As String
				Get
					If outerInstance.model IsNot Nothing Then
						Return CStr(outerInstance.model.getProperty(Document.TitleProperty))
					Else
						Return Nothing
					End If
				End Get
			End Property

			''' <summary>
			''' Gets the accessibleDescription property of this object.  If this
			''' property isn't set, returns the content type of this
			''' <code>JEditorPane</code> instead (e.g. "plain/text", "html/text").
			''' </summary>
			''' <returns> the localized description of the object; <code>null</code>
			'''      if this object does not have a description
			''' </returns>
			''' <seealso cref= #setAccessibleName </seealso>
			Public Property Overrides accessibleDescription As String
				Get
					Return outerInstance.editor.contentType
				End Get
			End Property

			''' <summary>
			''' Gets the role of this object.  The role of the object is the generic
			''' purpose or use of the class of this object.  For example, the role
			''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
			''' AccessibleRole are provided so component developers can pick from
			''' a set of predefined roles.  This enables assistive technologies to
			''' provide a consistent interface to various tweaked subclasses of
			''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
			''' that act like a push button) as well as distinguish between subclasses
			''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
			''' and AccessibleRole.RADIO_BUTTON for radio buttons).
			''' <p>Note that the AccessibleRole class is also extensible, so
			''' custom component developers can define their own AccessibleRole's
			''' if the set of predefined roles is inadequate.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Property Overrides accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TEXT
				End Get
			End Property
		End Class

	'    
	'     * Base AccessibleContext class for HTML elements
	'     
		Protected Friend MustInherit Class HTMLAccessibleContext
			Inherits AccessibleContext
			Implements Accessible, AccessibleComponent

			Private ReadOnly outerInstance As AccessibleHTML


			Protected Friend elementInfo As ElementInfo

			Public Sub New(ByVal outerInstance As AccessibleHTML, ByVal elementInfo As ElementInfo)
					Me.outerInstance = outerInstance
				Me.elementInfo = elementInfo
			End Sub

			' begin AccessibleContext implementation ...
			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet describing the states
			''' of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Property Overrides accessibleStateSet As AccessibleStateSet
				Get
					Dim states As New AccessibleStateSet
					Dim comp As Component = outerInstance.textComponent
    
					If comp.enabled Then states.add(AccessibleState.ENABLED)
					If TypeOf comp Is JTextComponent AndAlso CType(comp, JTextComponent).editable Then
    
						states.add(AccessibleState.EDITABLE)
						states.add(AccessibleState.FOCUSABLE)
					End If
					If comp.visible Then states.add(AccessibleState.VISIBLE)
					If comp.showing Then states.add(AccessibleState.SHOWING)
					Return states
				End Get
			End Property

			''' <summary>
			''' Gets the 0-based index of this object in its accessible parent.
			''' </summary>
			''' <returns> the 0-based index of this object in its parent; -1 if this
			''' object does not have an accessible parent.
			''' </returns>
			''' <seealso cref= #getAccessibleParent </seealso>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			''' <seealso cref= #getAccessibleChild </seealso>
			Public Property Overrides accessibleIndexInParent As Integer
				Get
					Return elementInfo.indexInParent
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children of the object.
			''' </summary>
			''' <returns> the number of accessible children of the object. </returns>
			Public Property Overrides accessibleChildrenCount As Integer
				Get
					Return elementInfo.childCount
				End Get
			End Property

			''' <summary>
			''' Returns the specified Accessible child of the object.  The Accessible
			''' children of an Accessible object are zero-based, so the first child
			''' of an Accessible child is at index 0, the second child is at index 1,
			''' and so on.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the Accessible child of the object </returns>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
				Dim childInfo As ElementInfo = elementInfo.getChild(i)
				If childInfo IsNot Nothing AndAlso TypeOf childInfo Is Accessible Then
					Return CType(childInfo, Accessible)
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Gets the locale of the component. If the component does not have a
			''' locale, then the locale of its parent is returned.
			''' </summary>
			''' <returns> this component's locale.  If this component does not have
			''' a locale, the locale of its parent is returned.
			''' </returns>
			''' <exception cref="IllegalComponentStateException">
			''' If the Component does not have its own locale and has not yet been
			''' added to a containment hierarchy such that the locale can be
			''' determined from the containing parent. </exception>
			Public Property Overrides locale As Locale
				Get
					Return outerInstance.editor.locale
				End Get
			End Property
			' ... end AccessibleContext implementation

			' begin AccessibleComponent implementation ...
			Public Property Overrides accessibleComponent As AccessibleComponent
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the background color of this object.
			''' </summary>
			''' <returns> the background color, if supported, of the object;
			''' otherwise, null </returns>
			''' <seealso cref= #setBackground </seealso>
			Public Overridable Property background As Color Implements AccessibleComponent.getBackground
				Get
					Return outerInstance.textComponent.background
				End Get
				Set(ByVal c As Color)
					outerInstance.textComponent.background = c
				End Set
			End Property


			''' <summary>
			''' Gets the foreground color of this object.
			''' </summary>
			''' <returns> the foreground color, if supported, of the object;
			''' otherwise, null </returns>
			''' <seealso cref= #setForeground </seealso>
			Public Overridable Property foreground As Color Implements AccessibleComponent.getForeground
				Get
					Return outerInstance.textComponent.foreground
				End Get
				Set(ByVal c As Color)
					outerInstance.textComponent.foreground = c
				End Set
			End Property


			''' <summary>
			''' Gets the Cursor of this object.
			''' </summary>
			''' <returns> the Cursor, if supported, of the object; otherwise, null </returns>
			''' <seealso cref= #setCursor </seealso>
			Public Overridable Property cursor As Cursor Implements AccessibleComponent.getCursor
				Get
					Return outerInstance.textComponent.cursor
				End Get
				Set(ByVal cursor As Cursor)
					outerInstance.textComponent.cursor = cursor
				End Set
			End Property


			''' <summary>
			''' Gets the Font of this object.
			''' </summary>
			''' <returns> the Font,if supported, for the object; otherwise, null </returns>
			''' <seealso cref= #setFont </seealso>
			Public Overridable Property font As Font Implements AccessibleComponent.getFont
				Get
					Return outerInstance.textComponent.font
				End Get
				Set(ByVal f As Font)
					outerInstance.textComponent.font = f
				End Set
			End Property


			''' <summary>
			''' Gets the FontMetrics of this object.
			''' </summary>
			''' <param name="f"> the Font </param>
			''' <returns> the FontMetrics, if supported, the object; otherwise, null </returns>
			''' <seealso cref= #getFont </seealso>
			Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics Implements AccessibleComponent.getFontMetrics
				Return outerInstance.textComponent.getFontMetrics(f)
			End Function

			''' <summary>
			''' Determines if the object is enabled.  Objects that are enabled
			''' will also have the AccessibleState.ENABLED state set in their
			''' AccessibleStateSets.
			''' </summary>
			''' <returns> true if object is enabled; otherwise, false </returns>
			''' <seealso cref= #setEnabled </seealso>
			''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState#ENABLED </seealso>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Overridable Property enabled As Boolean Implements AccessibleComponent.isEnabled
				Get
					Return outerInstance.textComponent.enabled
				End Get
				Set(ByVal b As Boolean)
					outerInstance.textComponent.enabled = b
				End Set
			End Property


			''' <summary>
			''' Determines if the object is visible.  Note: this means that the
			''' object intends to be visible; however, it may not be
			''' showing on the screen because one of the objects that this object
			''' is contained by is currently not visible.  To determine if an object
			''' is showing on the screen, use isShowing().
			''' <p>Objects that are visible will also have the
			''' AccessibleState.VISIBLE state set in their AccessibleStateSets.
			''' </summary>
			''' <returns> true if object is visible; otherwise, false </returns>
			''' <seealso cref= #setVisible </seealso>
			''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState#VISIBLE </seealso>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Overridable Property visible As Boolean Implements AccessibleComponent.isVisible
				Get
					Return outerInstance.textComponent.visible
				End Get
				Set(ByVal b As Boolean)
					outerInstance.textComponent.visible = b
				End Set
			End Property


			''' <summary>
			''' Determines if the object is showing.  This is determined by checking
			''' the visibility of the object and its ancestors.
			''' Note: this
			''' will return true even if the object is obscured by another (for
			''' example, it is underneath a menu that was pulled down).
			''' </summary>
			''' <returns> true if object is showing; otherwise, false </returns>
			Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
				Get
					Return outerInstance.textComponent.showing
				End Get
			End Property

			''' <summary>
			''' Checks whether the specified point is within this object's bounds,
			''' where the point's x and y coordinates are defined to be relative
			''' to the coordinate system of the object.
			''' </summary>
			''' <param name="p"> the Point relative to the coordinate system of the object </param>
			''' <returns> true if object contains Point; otherwise false </returns>
			''' <seealso cref= #getBounds </seealso>
			Public Overridable Function contains(ByVal p As Point) As Boolean Implements AccessibleComponent.contains
				Dim r As Rectangle = bounds
				If r IsNot Nothing Then
					Return r.contains(p.x, p.y)
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Returns the location of the object on the screen.
			''' </summary>
			''' <returns> the location of the object on screen; null if this object
			''' is not on the screen </returns>
			''' <seealso cref= #getBounds </seealso>
			''' <seealso cref= #getLocation </seealso>
			Public Overridable Property locationOnScreen As Point Implements AccessibleComponent.getLocationOnScreen
				Get
					Dim editorLocation As Point = outerInstance.textComponent.locationOnScreen
					Dim r As Rectangle = bounds
					If r IsNot Nothing Then
						Return New Point(editorLocation.x + r.x, editorLocation.y + r.y)
					Else
						Return Nothing
					End If
				End Get
			End Property

			''' <summary>
			''' Gets the location of the object relative to the parent in the form
			''' of a point specifying the object's top-left corner in the screen's
			''' coordinate space.
			''' </summary>
			''' <returns> An instance of Point representing the top-left corner of the
			''' object's bounds in the coordinate space of the screen; null if
			''' this object or its parent are not on the screen </returns>
			''' <seealso cref= #getBounds </seealso>
			''' <seealso cref= #getLocationOnScreen </seealso>
			Public Overridable Property location As Point Implements AccessibleComponent.getLocation
				Get
					Dim r As Rectangle = bounds
					If r IsNot Nothing Then
						Return New Point(r.x, r.y)
					Else
						Return Nothing
					End If
				End Get
				Set(ByVal p As Point)
				End Set
			End Property


			''' <summary>
			''' Gets the bounds of this object in the form of a Rectangle object.
			''' The bounds specify this object's width, height, and location
			''' relative to its parent.
			''' </summary>
			''' <returns> A rectangle indicating this component's bounds; null if
			''' this object is not on the screen. </returns>
			''' <seealso cref= #contains </seealso>
			Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
				Get
					Return elementInfo.bounds
				End Get
				Set(ByVal r As Rectangle)
				End Set
			End Property


			''' <summary>
			''' Returns the size of this object in the form of a Dimension object.
			''' The height field of the Dimension object contains this object's
			''' height, and the width field of the Dimension object contains this
			''' object's width.
			''' </summary>
			''' <returns> A Dimension object that indicates the size of this component;
			''' null if this object is not on the screen </returns>
			''' <seealso cref= #setSize </seealso>
			Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
				Get
					Dim r As Rectangle = bounds
					If r IsNot Nothing Then
						Return New Dimension(r.width, r.height)
					Else
						Return Nothing
					End If
				End Get
				Set(ByVal d As Dimension)
					Dim comp As Component = outerInstance.textComponent
					comp.size = d
				End Set
			End Property


			''' <summary>
			''' Returns the Accessible child, if one exists, contained at the local
			''' coordinate Point.
			''' </summary>
			''' <param name="p"> The point relative to the coordinate system of this object. </param>
			''' <returns> the Accessible, if it exists, at the specified location;
			''' otherwise null </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible Implements AccessibleComponent.getAccessibleAt
				Dim innerMostElement As ElementInfo = getElementInfoAt(outerInstance.rootElementInfo, p)
				If TypeOf innerMostElement Is Accessible Then
					Return CType(innerMostElement, Accessible)
				Else
					Return Nothing
				End If
			End Function

			Private Function getElementInfoAt(ByVal elementInfo As ElementInfo, ByVal p As Point) As ElementInfo
				If elementInfo.bounds Is Nothing Then Return Nothing
				If elementInfo.childCount = 0 AndAlso elementInfo.bounds.contains(p) Then
					Return elementInfo

				Else
					If TypeOf elementInfo Is TableElementInfo Then
						' Handle table caption as a special case since it's the
						' only table child that is not a table row.
						Dim captionInfo As ElementInfo = CType(elementInfo, TableElementInfo).captionInfo
						If captionInfo IsNot Nothing Then
							Dim ___bounds As Rectangle = captionInfo.bounds
							If ___bounds IsNot Nothing AndAlso ___bounds.contains(p) Then Return captionInfo
						End If
					End If
					For i As Integer = 0 To elementInfo.childCount - 1
						Dim childInfo As ElementInfo = elementInfo.getChild(i)
						Dim retValue As ElementInfo = getElementInfoAt(childInfo, p)
						If retValue IsNot Nothing Then Return retValue
					Next i
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns whether this object can accept focus or not.   Objects that
			''' can accept focus will also have the AccessibleState.FOCUSABLE state
			''' set in their AccessibleStateSets.
			''' </summary>
			''' <returns> true if object can accept focus; otherwise false </returns>
			''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState#FOCUSABLE </seealso>
			''' <seealso cref= AccessibleState#FOCUSED </seealso>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Overridable Property focusTraversable As Boolean Implements AccessibleComponent.isFocusTraversable
				Get
					Dim comp As Component = outerInstance.textComponent
					If TypeOf comp Is JTextComponent Then
						If CType(comp, JTextComponent).editable Then Return True
					End If
					Return False
				End Get
			End Property

			''' <summary>
			''' Requests focus for this object.  If this object cannot accept focus,
			''' nothing will happen.  Otherwise, the object will attempt to take
			''' focus. </summary>
			''' <seealso cref= #isFocusTraversable </seealso>
			Public Overridable Sub requestFocus() Implements AccessibleComponent.requestFocus
				' TIGER - 4856191
				If Not focusTraversable Then Return

				Dim comp As Component = outerInstance.textComponent
				If TypeOf comp Is JTextComponent Then

					comp.requestFocusInWindow()

					Try
						If elementInfo.validateIfNecessary() Then
							' set the caret position to the start of this component
							Dim elem As Element = elementInfo.element
							CType(comp, JTextComponent).caretPosition = elem.startOffset

							' fire a AccessibleState.FOCUSED property change event
							Dim ac As AccessibleContext = outerInstance.editor.accessibleContext
							Dim pce As New PropertyChangeEvent(Me, AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.FOCUSED)
							ac.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, pce)
						End If
					Catch e As System.ArgumentException
						' don't fire property change event
					End Try
				End If
			End Sub

			''' <summary>
			''' Adds the specified focus listener to receive focus events from this
			''' component.
			''' </summary>
			''' <param name="l"> the focus listener </param>
			''' <seealso cref= #removeFocusListener </seealso>
			Public Overridable Sub addFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.addFocusListener
				outerInstance.textComponent.addFocusListener(l)
			End Sub

			''' <summary>
			''' Removes the specified focus listener so it no longer receives focus
			''' events from this component.
			''' </summary>
			''' <param name="l"> the focus listener </param>
			''' <seealso cref= #addFocusListener </seealso>
			Public Overridable Sub removeFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.removeFocusListener
				outerInstance.textComponent.removeFocusListener(l)
			End Sub
			' ... end AccessibleComponent implementation
		End Class ' ... end HTMLAccessibleContext



	'    
	'     * ElementInfo for text
	'     
		Friend Class TextElementInfo
			Inherits ElementInfo
			Implements Accessible

			Private ReadOnly outerInstance As AccessibleHTML


			Friend Sub New(ByVal outerInstance As AccessibleHTML, ByVal element As Element, ByVal parent As ElementInfo)
					Me.outerInstance = outerInstance
				MyBase.New(element, parent)
			End Sub

			' begin AccessibleText implementation ...
			Private ___accessibleContext As AccessibleContext

			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					If ___accessibleContext Is Nothing Then ___accessibleContext = New TextAccessibleContext(Me, Me)
					Return ___accessibleContext
				End Get
			End Property

	'        
	'         * AccessibleContext for text elements
	'         
			Public Class TextAccessibleContext
				Inherits HTMLAccessibleContext
				Implements AccessibleText

				Private ReadOnly outerInstance As AccessibleHTML.TextElementInfo


				Public Sub New(ByVal outerInstance As AccessibleHTML.TextElementInfo, ByVal elementInfo As ElementInfo)
						Me.outerInstance = outerInstance
					MyBase.New(elementInfo)
				End Sub

				Public Property Overrides accessibleText As AccessibleText
					Get
						Return Me
					End Get
				End Property

				''' <summary>
				''' Gets the accessibleName property of this object.  The accessibleName
				''' property of an object is a localized String that designates the purpose
				''' of the object.  For example, the accessibleName property of a label
				''' or button might be the text of the label or button itself.  In the
				''' case of an object that doesn't display its name, the accessibleName
				''' should still be set.  For example, in the case of a text field used
				''' to enter the name of a city, the accessibleName for the en_US locale
				''' could be 'city.'
				''' </summary>
				''' <returns> the localized name of the object; null if this
				''' object does not have a name
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleName As String
					Get
						If model IsNot Nothing Then
							Return CStr(model.getProperty(Document.TitleProperty))
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the accessibleDescription property of this object.  If this
				''' property isn't set, returns the content type of this
				''' <code>JEditorPane</code> instead (e.g. "plain/text", "html/text").
				''' </summary>
				''' <returns> the localized description of the object; <code>null</code>
				'''  if this object does not have a description
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleDescription As String
					Get
						Return editor.contentType
					End Get
				End Property

				''' <summary>
				''' Gets the role of this object.  The role of the object is the generic
				''' purpose or use of the class of this object.  For example, the role
				''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
				''' AccessibleRole are provided so component developers can pick from
				''' a set of predefined roles.  This enables assistive technologies to
				''' provide a consistent interface to various tweaked subclasses of
				''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
				''' that act like a push button) as well as distinguish between subclasses
				''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
				''' and AccessibleRole.RADIO_BUTTON for radio buttons).
				''' <p>Note that the AccessibleRole class is also extensible, so
				''' custom component developers can define their own AccessibleRole's
				''' if the set of predefined roles is inadequate.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Return AccessibleRole.TEXT
					End Get
				End Property

				''' <summary>
				''' Given a point in local coordinates, return the zero-based index
				''' of the character under that Point.  If the point is invalid,
				''' this method returns -1.
				''' </summary>
				''' <param name="p"> the Point in local coordinates </param>
				''' <returns> the zero-based index of the character under Point p; if
				''' Point is invalid returns -1. </returns>
				Public Overridable Function getIndexAtPoint(ByVal p As Point) As Integer Implements AccessibleText.getIndexAtPoint
					Dim v As View = outerInstance.view
					If v IsNot Nothing Then
						Return v.viewToModel(p.x, p.y, bounds)
					Else
						Return -1
					End If
				End Function

				''' <summary>
				''' Determine the bounding box of the character at the given
				''' index into the string.  The bounds are returned in local
				''' coordinates.  If the index is invalid an empty rectangle is
				''' returned.
				''' </summary>
				''' <param name="i"> the index into the String </param>
				''' <returns> the screen coordinates of the character's the bounding box,
				''' if index is invalid returns an empty rectangle. </returns>
				Public Overridable Function getCharacterBounds(ByVal i As Integer) As Rectangle Implements AccessibleText.getCharacterBounds
					Try
						Return editor.uI.modelToView(editor, i)
					Catch e As BadLocationException
						Return Nothing
					End Try
				End Function

				''' <summary>
				''' Return the number of characters (valid indicies)
				''' </summary>
				''' <returns> the number of characters </returns>
				Public Overridable Property charCount As Integer Implements AccessibleText.getCharCount
					Get
						If outerInstance.validateIfNecessary() Then
							Dim elem As Element = elementInfo.element
							Return elem.endOffset - elem.startOffset
						End If
						Return 0
					End Get
				End Property

				''' <summary>
				''' Return the zero-based offset of the caret.
				''' 
				''' Note: That to the right of the caret will have the same index
				''' value as the offset (the caret is between two characters). </summary>
				''' <returns> the zero-based offset of the caret. </returns>
				Public Overridable Property caretPosition As Integer Implements AccessibleText.getCaretPosition
					Get
						Dim v As View = outerInstance.view
						If v Is Nothing Then Return -1
						Dim c As Container = v.container
						If c Is Nothing Then Return -1
						If TypeOf c Is JTextComponent Then
							Return CType(c, JTextComponent).caretPosition
						Else
							Return -1
						End If
					End Get
				End Property

				''' <summary>
				''' IndexedSegment extends Segment adding the offset into the
				''' the model the <code>Segment</code> was asked for.
				''' </summary>
				Private Class IndexedSegment
					Inherits Segment

					Private ReadOnly outerInstance As AccessibleHTML.TextElementInfo.TextAccessibleContext

					Public Sub New(ByVal outerInstance As AccessibleHTML.TextElementInfo.TextAccessibleContext)
						Me.outerInstance = outerInstance
					End Sub

					''' <summary>
					''' Offset into the model that the position represents.
					''' </summary>
					Public modelOffset As Integer
				End Class

				Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAtIndex
					Return getAtIndex(part, index, 0)
				End Function


				Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAfterIndex
					Return getAtIndex(part, index, 1)
				End Function

				Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getBeforeIndex
					Return getAtIndex(part, index, -1)
				End Function

				''' <summary>
				''' Gets the word, sentence, or character at <code>index</code>.
				''' If <code>direction</code> is non-null this will find the
				''' next/previous word/sentence/character.
				''' </summary>
				Private Function getAtIndex(ByVal part As Integer, ByVal index As Integer, ByVal direction As Integer) As String
					If TypeOf model Is AbstractDocument Then CType(model, AbstractDocument).readLock()
					Try
						If index < 0 OrElse index >= model.length Then Return Nothing
						Select Case part
						Case AccessibleText.CHARACTER
							If index + direction < model.length AndAlso index + direction >= 0 Then Return model.getText(index + direction, 1)


						Case AccessibleText.WORD, AccessibleText.SENTENCE
							Dim seg As IndexedSegment = getSegmentAt(part, index)
							If seg IsNot Nothing Then
								If direction <> 0 Then
									Dim [next] As Integer


									If direction < 0 Then
										[next] = seg.modelOffset - 1
									Else
										[next] = seg.modelOffset + direction * seg.count
									End If
									If [next] >= 0 AndAlso [next] <= model.length Then
										seg = getSegmentAt(part, [next])
									Else
										seg = Nothing
									End If
								End If
								If seg IsNot Nothing Then Return New String(seg.array, seg.offset, seg.count)
							End If

						Case Else
						End Select
					Catch e As BadLocationException
					Finally
						If TypeOf model Is AbstractDocument Then CType(model, AbstractDocument).readUnlock()
					End Try
					Return Nothing
				End Function

	'            
	'             * Returns the paragraph element for the specified index.
	'             
				Private Function getParagraphElement(ByVal index As Integer) As Element
					If TypeOf model Is PlainDocument Then
						Dim sdoc As PlainDocument = CType(model, PlainDocument)
						Return sdoc.getParagraphElement(index)
					ElseIf TypeOf model Is StyledDocument Then
						Dim sdoc As StyledDocument = CType(model, StyledDocument)
						Return sdoc.getParagraphElement(index)
					Else
						Dim para As Element
						para = model.defaultRootElement
						Do While Not para.leaf
							Dim pos As Integer = para.getElementIndex(index)
							para = para.getElement(pos)
						Loop
						If para Is Nothing Then Return Nothing
						Return para.parentElement
					End If
				End Function

	'            
	'             * Returns a <code>Segment</code> containing the paragraph text
	'             * at <code>index</code>, or null if <code>index</code> isn't
	'             * valid.
	'             
				Private Function getParagraphElementText(ByVal index As Integer) As IndexedSegment
					Dim para As Element = getParagraphElement(index)


					If para IsNot Nothing Then
						Dim segment As New IndexedSegment(Me)
						Try
							Dim length As Integer = para.endOffset - para.startOffset
							model.getText(para.startOffset, length, segment)
						Catch e As BadLocationException
							Return Nothing
						End Try
						segment.modelOffset = para.startOffset
						Return segment
					End If
					Return Nothing
				End Function


				''' <summary>
				''' Returns the Segment at <code>index</code> representing either
				''' the paragraph or sentence as identified by <code>part</code>, or
				''' null if a valid paragraph/sentence can't be found. The offset
				''' will point to the start of the word/sentence in the array, and
				''' the modelOffset will point to the location of the word/sentence
				''' in the model.
				''' </summary>
				Private Function getSegmentAt(ByVal part As Integer, ByVal index As Integer) As IndexedSegment

					Dim seg As IndexedSegment = getParagraphElementText(index)
					If seg Is Nothing Then Return Nothing
					Dim [iterator] As java.text.BreakIterator
					Select Case part
					Case AccessibleText.WORD
						[iterator] = java.text.BreakIterator.getWordInstance(locale)
					Case AccessibleText.SENTENCE
						[iterator] = java.text.BreakIterator.getSentenceInstance(locale)
					Case Else
						Return Nothing
					End Select
					seg.first()
					[iterator].text = seg
					Dim [end] As Integer = [iterator].following(index - seg.modelOffset + seg.offset)
					If [end] = java.text.BreakIterator.DONE Then Return Nothing
					If [end] > seg.offset + seg.count Then Return Nothing
					Dim begin As Integer = [iterator].previous()
					If begin = java.text.BreakIterator.DONE OrElse begin >= seg.offset + seg.count Then Return Nothing
					seg.modelOffset = seg.modelOffset + begin - seg.offset
					seg.offset = begin
					seg.count = [end] - begin
					Return seg
				End Function

				''' <summary>
				''' Return the AttributeSet for a given character at a given index
				''' </summary>
				''' <param name="i"> the zero-based index into the text </param>
				''' <returns> the AttributeSet of the character </returns>
				Public Overridable Function getCharacterAttribute(ByVal i As Integer) As AttributeSet Implements AccessibleText.getCharacterAttribute
					If TypeOf model Is StyledDocument Then
						Dim doc As StyledDocument = CType(model, StyledDocument)
						Dim elem As Element = doc.getCharacterElement(i)
						If elem IsNot Nothing Then Return elem.attributes
					End If
					Return Nothing
				End Function

				''' <summary>
				''' Returns the start offset within the selected text.
				''' If there is no selection, but there is
				''' a caret, the start and end offsets will be the same.
				''' </summary>
				''' <returns> the index into the text of the start of the selection </returns>
				Public Overridable Property selectionStart As Integer Implements AccessibleText.getSelectionStart
					Get
						Return editor.selectionStart
					End Get
				End Property

				''' <summary>
				''' Returns the end offset within the selected text.
				''' If there is no selection, but there is
				''' a caret, the start and end offsets will be the same.
				''' </summary>
				''' <returns> the index into the text of the end of the selection </returns>
				Public Overridable Property selectionEnd As Integer Implements AccessibleText.getSelectionEnd
					Get
						Return editor.selectionEnd
					End Get
				End Property

				''' <summary>
				''' Returns the portion of the text that is selected.
				''' </summary>
				''' <returns> the String portion of the text that is selected </returns>
				Public Overridable Property selectedText As String Implements AccessibleText.getSelectedText
					Get
						Return editor.selectedText
					End Get
				End Property

	'            
	'             * Returns the text substring starting at the specified
	'             * offset with the specified length.
	'             
				Private Function getText(ByVal offset As Integer, ByVal length As Integer) As String

					If model IsNot Nothing AndAlso TypeOf model Is StyledDocument Then
						Dim doc As StyledDocument = CType(model, StyledDocument)
						Return model.getText(offset, length)
					Else
						Return Nothing
					End If
				End Function
			End Class
		End Class

	'    
	'     * ElementInfo for images
	'     
		Private Class IconElementInfo
			Inherits ElementInfo
			Implements Accessible

			Private ReadOnly outerInstance As AccessibleHTML


			Private width As Integer = -1
			Private height As Integer = -1

			Friend Sub New(ByVal outerInstance As AccessibleHTML, ByVal element As Element, ByVal parent As ElementInfo)
					Me.outerInstance = outerInstance
				MyBase.New(element, parent)
			End Sub

			Protected Friend Overrides Sub invalidate(ByVal first As Boolean)
				MyBase.invalidate(first)
					height = -1
					width = height
			End Sub

			Private Function getImageSize(ByVal key As Object) As Integer
				If validateIfNecessary() Then
					Dim size As Integer = getIntAttr(attributes, key, -1)

					If size = -1 Then
						Dim v As View = view

						size = 0
						If TypeOf v Is ImageView Then
							Dim img As Image = CType(v, ImageView).image
							If img IsNot Nothing Then
								If key Is HTML.Attribute.WIDTH Then
									size = img.getWidth(Nothing)
								Else
									size = img.getHeight(Nothing)
								End If
							End If
						End If
					End If
					Return size
				End If
				Return 0
			End Function

			' begin AccessibleIcon implementation ...
			Private ___accessibleContext As AccessibleContext

			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					If ___accessibleContext Is Nothing Then ___accessibleContext = New IconAccessibleContext(Me, Me)
					Return ___accessibleContext
				End Get
			End Property

	'        
	'         * AccessibleContext for images
	'         
			Protected Friend Class IconAccessibleContext
				Inherits HTMLAccessibleContext
				Implements AccessibleIcon

				Private ReadOnly outerInstance As AccessibleHTML.IconElementInfo


				Public Sub New(ByVal outerInstance As AccessibleHTML.IconElementInfo, ByVal elementInfo As ElementInfo)
						Me.outerInstance = outerInstance
					MyBase.New(elementInfo)
				End Sub

				''' <summary>
				''' Gets the accessibleName property of this object.  The accessibleName
				''' property of an object is a localized String that designates the purpose
				''' of the object.  For example, the accessibleName property of a label
				''' or button might be the text of the label or button itself.  In the
				''' case of an object that doesn't display its name, the accessibleName
				''' should still be set.  For example, in the case of a text field used
				''' to enter the name of a city, the accessibleName for the en_US locale
				''' could be 'city.'
				''' </summary>
				''' <returns> the localized name of the object; null if this
				''' object does not have a name
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleName As String
					Get
						Return accessibleIconDescription
					End Get
				End Property

				''' <summary>
				''' Gets the accessibleDescription property of this object.  If this
				''' property isn't set, returns the content type of this
				''' <code>JEditorPane</code> instead (e.g. "plain/text", "html/text").
				''' </summary>
				''' <returns> the localized description of the object; <code>null</code>
				'''  if this object does not have a description
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleDescription As String
					Get
						Return editor.contentType
					End Get
				End Property

				''' <summary>
				''' Gets the role of this object.  The role of the object is the generic
				''' purpose or use of the class of this object.  For example, the role
				''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
				''' AccessibleRole are provided so component developers can pick from
				''' a set of predefined roles.  This enables assistive technologies to
				''' provide a consistent interface to various tweaked subclasses of
				''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
				''' that act like a push button) as well as distinguish between subclasses
				''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
				''' and AccessibleRole.RADIO_BUTTON for radio buttons).
				''' <p>Note that the AccessibleRole class is also extensible, so
				''' custom component developers can define their own AccessibleRole's
				''' if the set of predefined roles is inadequate.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Return AccessibleRole.ICON
					End Get
				End Property

				Public Property Overrides accessibleIcon As AccessibleIcon()
					Get
						Dim icons As AccessibleIcon() = New AccessibleIcon(0){}
						icons(0) = Me
						Return icons
					End Get
				End Property

				''' <summary>
				''' Gets the description of the icon.  This is meant to be a brief
				''' textual description of the object.  For example, it might be
				''' presented to a blind user to give an indication of the purpose
				''' of the icon.
				''' </summary>
				''' <returns> the description of the icon </returns>
				Public Overridable Property accessibleIconDescription As String Implements AccessibleIcon.getAccessibleIconDescription
					Get
						Return CType(outerInstance.view, ImageView).altText
					End Get
					Set(ByVal description As String)
					End Set
				End Property


				''' <summary>
				''' Gets the width of the icon
				''' </summary>
				''' <returns> the width of the icon. </returns>
				Public Overridable Property accessibleIconWidth As Integer Implements AccessibleIcon.getAccessibleIconWidth
					Get
						If outerInstance.width = -1 Then outerInstance.width = outerInstance.getImageSize(HTML.Attribute.WIDTH)
						Return outerInstance.width
					End Get
				End Property

				''' <summary>
				''' Gets the height of the icon
				''' </summary>
				''' <returns> the height of the icon. </returns>
				Public Overridable Property accessibleIconHeight As Integer Implements AccessibleIcon.getAccessibleIconHeight
					Get
						If outerInstance.height = -1 Then outerInstance.height = outerInstance.getImageSize(HTML.Attribute.HEIGHT)
						Return outerInstance.height
					End Get
				End Property
			End Class
			' ... end AccessibleIconImplementation
		End Class


		''' <summary>
		''' TableElementInfo encapsulates information about a HTML.Tag.TABLE.
		''' To make access fast it crates a grid containing the children to
		''' allow for access by row, column. TableElementInfo will contain
		''' TableRowElementInfos, which will contain TableCellElementInfos.
		''' Any time one of the rows or columns becomes invalid the table is
		''' invalidated.  This is because any time one of the child attributes
		''' changes the size of the grid may have changed.
		''' </summary>
		Private Class TableElementInfo
			Inherits ElementInfo
			Implements Accessible

			Private ReadOnly outerInstance As AccessibleHTML


			Protected Friend caption As ElementInfo

			''' <summary>
			''' Allocation of the table by row x column. There may be holes (eg
			''' nulls) depending upon the html, any cell that has a rowspan/colspan
			''' > 1 will be contained multiple times in the grid.
			''' </summary>
			Private grid As TableCellElementInfo()()


			Friend Sub New(ByVal outerInstance As AccessibleHTML, ByVal e As Element, ByVal parent As ElementInfo)
					Me.outerInstance = outerInstance
				MyBase.New(e, parent)
			End Sub

			Public Overridable Property captionInfo As ElementInfo
				Get
					Return caption
				End Get
			End Property

			''' <summary>
			''' Overriden to update the grid when validating.
			''' </summary>
			Protected Friend Overrides Sub validate()
				MyBase.validate()
				updateGrid()
			End Sub

			''' <summary>
			''' Overriden to only alloc instances of TableRowElementInfos.
			''' </summary>
			Protected Friend Overrides Sub loadChildren(ByVal e As Element)

				For counter As Integer = 0 To e.elementCount - 1
					Dim ___child As Element = e.getElement(counter)
					Dim attrs As AttributeSet = ___child.attributes

					If attrs.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.TR Then
						addChild(New TableRowElementInfo(Me, ___child, Me, counter))

					ElseIf attrs.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.CAPTION Then
						' Handle captions as a special case since all other
						' children are table rows.
						caption = outerInstance.createElementInfo(___child, Me)
					End If
				Next counter
			End Sub

			''' <summary>
			''' Updates the grid.
			''' </summary>
			Private Sub updateGrid()
				' Determine the max row/col count.
				Dim delta As Integer = 0
				Dim maxCols As Integer = 0
				Dim rows As Integer
				For counter As Integer = 0 To childCount - 1
					Dim ___row As TableRowElementInfo = getRow(counter)
					Dim prev As Integer = 0
					For y As Integer = 0 To delta - 1
						prev = Math.Max(prev, getRow(counter - y - 1).getColumnCount(y + 2))
					Next y
					delta = Math.Max(___row.rowCount, delta)
					delta -= 1
					maxCols = Math.Max(maxCols, ___row.columnCount + prev)
				Next counter
				rows = childCount + delta

				' Alloc
				grid = New TableCellElementInfo(rows - 1)(){}
				For counter As Integer = 0 To rows - 1
					grid(counter) = New TableCellElementInfo(maxCols - 1){}
				Next counter
				' Update
				For counter As Integer = 0 To rows - 1
					getRow(counter).updateGrid(counter)
				Next counter
			End Sub

			''' <summary>
			''' Returns the TableCellElementInfo at the specified index.
			''' </summary>
			Public Overridable Function getRow(ByVal index As Integer) As TableRowElementInfo
				Return CType(getChild(index), TableRowElementInfo)
			End Function

			''' <summary>
			''' Returns the TableCellElementInfo by row and column.
			''' </summary>
			Public Overridable Function getCell(ByVal r As Integer, ByVal c As Integer) As TableCellElementInfo
				If validateIfNecessary() AndAlso r < grid.Length AndAlso c < grid(0).Length Then Return grid(r)(c)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the rowspan of the specified entry.
			''' </summary>
			Public Overridable Function getRowExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer
				Dim ___cell As TableCellElementInfo = getCell(r, c)

				If ___cell IsNot Nothing Then
					Dim rows As Integer = ___cell.rowCount
					Dim delta As Integer = 1

					Do While (r - delta) >= 0 AndAlso grid(r - delta)(c) Is ___cell
						delta += 1
					Loop
					Return rows - delta + 1
				End If
				Return 0
			End Function

			''' <summary>
			''' Returns the colspan of the specified entry.
			''' </summary>
			Public Overridable Function getColumnExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer
				Dim ___cell As TableCellElementInfo = getCell(r, c)

				If ___cell IsNot Nothing Then
					Dim cols As Integer = ___cell.columnCount
					Dim delta As Integer = 1

					Do While (c - delta) >= 0 AndAlso grid(r)(c - delta) Is ___cell
						delta += 1
					Loop
					Return cols - delta + 1
				End If
				Return 0
			End Function

			''' <summary>
			''' Returns the number of rows in the table.
			''' </summary>
			Public Overridable Property rowCount As Integer
				Get
					If validateIfNecessary() Then Return grid.Length
					Return 0
				End Get
			End Property

			''' <summary>
			''' Returns the number of columns in the table.
			''' </summary>
			Public Overridable Property columnCount As Integer
				Get
					If validateIfNecessary() AndAlso grid.Length > 0 Then Return grid(0).Length
					Return 0
				End Get
			End Property

			' begin AccessibleTable implementation ...
			Private ___accessibleContext As AccessibleContext

			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					If ___accessibleContext Is Nothing Then ___accessibleContext = New TableAccessibleContext(Me, Me)
					Return ___accessibleContext
				End Get
			End Property

	'        
	'         * AccessibleContext for tables
	'         
			Public Class TableAccessibleContext
				Inherits HTMLAccessibleContext
				Implements AccessibleTable

				Private ReadOnly outerInstance As AccessibleHTML.TableElementInfo


				Private rowHeadersTable As AccessibleHeadersTable

				Public Sub New(ByVal outerInstance As AccessibleHTML.TableElementInfo, ByVal elementInfo As ElementInfo)
						Me.outerInstance = outerInstance
					MyBase.New(elementInfo)
				End Sub

				''' <summary>
				''' Gets the accessibleName property of this object.  The accessibleName
				''' property of an object is a localized String that designates the purpose
				''' of the object.  For example, the accessibleName property of a label
				''' or button might be the text of the label or button itself.  In the
				''' case of an object that doesn't display its name, the accessibleName
				''' should still be set.  For example, in the case of a text field used
				''' to enter the name of a city, the accessibleName for the en_US locale
				''' could be 'city.'
				''' </summary>
				''' <returns> the localized name of the object; null if this
				''' object does not have a name
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleName As String
					Get
						' return the role of the object
						Return accessibleRole.ToString()
					End Get
				End Property

				''' <summary>
				''' Gets the accessibleDescription property of this object.  If this
				''' property isn't set, returns the content type of this
				''' <code>JEditorPane</code> instead (e.g. "plain/text", "html/text").
				''' </summary>
				''' <returns> the localized description of the object; <code>null</code>
				'''  if this object does not have a description
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleDescription As String
					Get
						Return editor.contentType
					End Get
				End Property

				''' <summary>
				''' Gets the role of this object.  The role of the object is the generic
				''' purpose or use of the class of this object.  For example, the role
				''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
				''' AccessibleRole are provided so component developers can pick from
				''' a set of predefined roles.  This enables assistive technologies to
				''' provide a consistent interface to various tweaked subclasses of
				''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
				''' that act like a push button) as well as distinguish between subclasses
				''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
				''' and AccessibleRole.RADIO_BUTTON for radio buttons).
				''' <p>Note that the AccessibleRole class is also extensible, so
				''' custom component developers can define their own AccessibleRole's
				''' if the set of predefined roles is inadequate.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Return AccessibleRole.TABLE
					End Get
				End Property

				''' <summary>
				''' Gets the 0-based index of this object in its accessible parent.
				''' </summary>
				''' <returns> the 0-based index of this object in its parent; -1 if this
				''' object does not have an accessible parent.
				''' </returns>
				''' <seealso cref= #getAccessibleParent </seealso>
				''' <seealso cref= #getAccessibleChildrenCount
				''' @gsee #getAccessibleChild </seealso>
				Public Property Overrides accessibleIndexInParent As Integer
					Get
						Return elementInfo.indexInParent
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible children of the object.
				''' </summary>
				''' <returns> the number of accessible children of the object. </returns>
				Public Property Overrides accessibleChildrenCount As Integer
					Get
						Return CType(elementInfo, TableElementInfo).rowCount * CType(elementInfo, TableElementInfo).columnCount
					End Get
				End Property

				''' <summary>
				''' Returns the specified Accessible child of the object.  The Accessible
				''' children of an Accessible object are zero-based, so the first child
				''' of an Accessible child is at index 0, the second child is at index 1,
				''' and so on.
				''' </summary>
				''' <param name="i"> zero-based index of child </param>
				''' <returns> the Accessible child of the object </returns>
				''' <seealso cref= #getAccessibleChildrenCount </seealso>
				Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
					Dim rowCount As Integer = CType(elementInfo, TableElementInfo).rowCount
					Dim columnCount As Integer = CType(elementInfo, TableElementInfo).columnCount
					Dim r As Integer = i \ rowCount
					Dim c As Integer = i Mod columnCount
					If r < 0 OrElse r >= rowCount OrElse c < 0 OrElse c >= columnCount Then
						Return Nothing
					Else
						Return getAccessibleAt(r, c)
					End If
				End Function

				Public Property Overrides accessibleTable As AccessibleTable
					Get
						Return Me
					End Get
				End Property

				''' <summary>
				''' Returns the caption for the table.
				''' </summary>
				''' <returns> the caption for the table </returns>
				Public Overridable Property accessibleCaption As Accessible Implements AccessibleTable.getAccessibleCaption
					Get
						Dim captionInfo As ElementInfo = outerInstance.captionInfo
						If TypeOf captionInfo Is Accessible Then
							Return CType(outerInstance.caption, Accessible)
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal a As Accessible)
					End Set
				End Property


				''' <summary>
				''' Returns the summary description of the table.
				''' </summary>
				''' <returns> the summary description of the table </returns>
				Public Overridable Property accessibleSummary As Accessible Implements AccessibleTable.getAccessibleSummary
					Get
						Return Nothing
					End Get
					Set(ByVal a As Accessible)
					End Set
				End Property


				''' <summary>
				''' Returns the number of rows in the table.
				''' </summary>
				''' <returns> the number of rows in the table </returns>
				Public Overridable Property accessibleRowCount As Integer Implements AccessibleTable.getAccessibleRowCount
					Get
						Return CType(elementInfo, TableElementInfo).rowCount
					End Get
				End Property

				''' <summary>
				''' Returns the number of columns in the table.
				''' </summary>
				''' <returns> the number of columns in the table </returns>
				Public Overridable Property accessibleColumnCount As Integer Implements AccessibleTable.getAccessibleColumnCount
					Get
						Return CType(elementInfo, TableElementInfo).columnCount
					End Get
				End Property

				''' <summary>
				''' Returns the Accessible at a specified row and column
				''' in the table.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the Accessible at the specified row and column </returns>
				Public Overridable Function getAccessibleAt(ByVal r As Integer, ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleAt
					Dim cellInfo As TableCellElementInfo = outerInstance.getCell(r, c)
					If cellInfo IsNot Nothing Then
						Return cellInfo.accessible
					Else
						Return Nothing
					End If
				End Function

				''' <summary>
				''' Returns the number of rows occupied by the Accessible at
				''' a specified row and column in the table.
				''' </summary>
				''' <returns> the number of rows occupied by the Accessible at a
				''' given specified (row, column) </returns>
				Public Overridable Function getAccessibleRowExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleRowExtentAt
					Return CType(elementInfo, TableElementInfo).getRowExtentAt(r, c)
				End Function

				''' <summary>
				''' Returns the number of columns occupied by the Accessible at
				''' a specified row and column in the table.
				''' </summary>
				''' <returns> the number of columns occupied by the Accessible at a
				''' given specified row and column </returns>
				Public Overridable Function getAccessibleColumnExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleColumnExtentAt
					Return CType(elementInfo, TableElementInfo).getColumnExtentAt(r, c)
				End Function

				''' <summary>
				''' Returns the row headers as an AccessibleTable.
				''' </summary>
				''' <returns> an AccessibleTable representing the row
				''' headers </returns>
				Public Overridable Property accessibleRowHeader As AccessibleTable Implements AccessibleTable.getAccessibleRowHeader
					Get
						Return rowHeadersTable
					End Get
					Set(ByVal table As AccessibleTable)
					End Set
				End Property


				''' <summary>
				''' Returns the column headers as an AccessibleTable.
				''' </summary>
				''' <returns> an AccessibleTable representing the column
				''' headers </returns>
				Public Overridable Property accessibleColumnHeader As AccessibleTable Implements AccessibleTable.getAccessibleColumnHeader
					Get
						Return Nothing
					End Get
					Set(ByVal table As AccessibleTable)
					End Set
				End Property


				''' <summary>
				''' Returns the description of the specified row in the table.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <returns> the description of the row </returns>
				Public Overridable Function getAccessibleRowDescription(ByVal r As Integer) As Accessible Implements AccessibleTable.getAccessibleRowDescription
					Return Nothing
				End Function

				''' <summary>
				''' Sets the description text of the specified row of the table.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <param name="a"> the description of the row </param>
				Public Overridable Sub setAccessibleRowDescription(ByVal r As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleRowDescription
				End Sub

				''' <summary>
				''' Returns the description text of the specified column in the table.
				''' </summary>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the text description of the column </returns>
				Public Overridable Function getAccessibleColumnDescription(ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleColumnDescription
					Return Nothing
				End Function

				''' <summary>
				''' Sets the description text of the specified column in the table.
				''' </summary>
				''' <param name="c"> zero-based column of the table </param>
				''' <param name="a"> the text description of the column </param>
				Public Overridable Sub setAccessibleColumnDescription(ByVal c As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleColumnDescription
				End Sub

				''' <summary>
				''' Returns a boolean value indicating whether the accessible at
				''' a specified row and column is selected.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the boolean value true if the accessible at the
				''' row and column is selected. Otherwise, the boolean value
				''' false </returns>
				Public Overridable Function isAccessibleSelected(ByVal r As Integer, ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleSelected
					If outerInstance.validateIfNecessary() Then
						If r < 0 OrElse r >= accessibleRowCount OrElse c < 0 OrElse c >= accessibleColumnCount Then Return False
						Dim cell As TableCellElementInfo = outerInstance.getCell(r, c)
						If cell IsNot Nothing Then
							Dim elem As Element = cell.element
							Dim start As Integer = elem.startOffset
							Dim [end] As Integer = elem.endOffset
							Return start >= editor.selectionStart AndAlso [end] <= editor.selectionEnd
						End If
					End If
					Return False
				End Function

				''' <summary>
				''' Returns a boolean value indicating whether the specified row
				''' is selected.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <returns> the boolean value true if the specified row is selected.
				''' Otherwise, false. </returns>
				Public Overridable Function isAccessibleRowSelected(ByVal r As Integer) As Boolean Implements AccessibleTable.isAccessibleRowSelected
					If outerInstance.validateIfNecessary() Then
						If r < 0 OrElse r >= accessibleRowCount Then Return False
						Dim nColumns As Integer = accessibleColumnCount

						Dim startCell As TableCellElementInfo = outerInstance.getCell(r, 0)
						If startCell Is Nothing Then Return False
						Dim start As Integer = startCell.element.startOffset

						Dim endCell As TableCellElementInfo = outerInstance.getCell(r, nColumns-1)
						If endCell Is Nothing Then Return False
						Dim [end] As Integer = endCell.element.endOffset

						Return start >= editor.selectionStart AndAlso [end] <= editor.selectionEnd
					End If
					Return False
				End Function

				''' <summary>
				''' Returns a boolean value indicating whether the specified column
				''' is selected.
				''' </summary>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the boolean value true if the specified column is selected.
				''' Otherwise, false. </returns>
				Public Overridable Function isAccessibleColumnSelected(ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleColumnSelected
					If outerInstance.validateIfNecessary() Then
						If c < 0 OrElse c >= accessibleColumnCount Then Return False
						Dim nRows As Integer = accessibleRowCount

						Dim startCell As TableCellElementInfo = outerInstance.getCell(0, c)
						If startCell Is Nothing Then Return False
						Dim start As Integer = startCell.element.startOffset

						Dim endCell As TableCellElementInfo = outerInstance.getCell(nRows-1, c)
						If endCell Is Nothing Then Return False
						Dim [end] As Integer = endCell.element.endOffset
						Return start >= editor.selectionStart AndAlso [end] <= editor.selectionEnd
					End If
					Return False
				End Function

				''' <summary>
				''' Returns the selected rows in a table.
				''' </summary>
				''' <returns> an array of selected rows where each element is a
				''' zero-based row of the table </returns>
				Public Overridable Property selectedAccessibleRows As Integer()
					Get
						If outerInstance.validateIfNecessary() Then
							Dim nRows As Integer = accessibleRowCount
							Dim vec As New List(Of Integer?)
    
							For i As Integer = 0 To nRows - 1
								If isAccessibleRowSelected(i) Then vec.Add(Convert.ToInt32(i))
							Next i
							Dim retval As Integer() = New Integer(vec.Count - 1){}
							For i As Integer = 0 To retval.Length - 1
								retval(i) = vec(i)
							Next i
							Return retval
						End If
						Return New Integer(){}
					End Get
				End Property

				''' <summary>
				''' Returns the selected columns in a table.
				''' </summary>
				''' <returns> an array of selected columns where each element is a
				''' zero-based column of the table </returns>
				Public Overridable Property selectedAccessibleColumns As Integer()
					Get
						If outerInstance.validateIfNecessary() Then
							Dim nColumns As Integer = accessibleRowCount
							Dim vec As New List(Of Integer?)
    
							For i As Integer = 0 To nColumns - 1
								If isAccessibleColumnSelected(i) Then vec.Add(Convert.ToInt32(i))
							Next i
							Dim retval As Integer() = New Integer(vec.Count - 1){}
							For i As Integer = 0 To retval.Length - 1
								retval(i) = vec(i)
							Next i
							Return retval
						End If
						Return New Integer(){}
					End Get
				End Property

				' begin AccessibleExtendedTable implementation -------------

				''' <summary>
				''' Returns the row number of an index in the table.
				''' </summary>
				''' <param name="index"> the zero-based index in the table </param>
				''' <returns> the zero-based row of the table if one exists;
				''' otherwise -1. </returns>
				Public Overridable Function getAccessibleRow(ByVal index As Integer) As Integer
					If outerInstance.validateIfNecessary() Then
						Dim numCells As Integer = accessibleColumnCount * accessibleRowCount
						If index >= numCells Then
							Return -1
						Else
							Return index \ accessibleColumnCount
						End If
					End If
					Return -1
				End Function

				''' <summary>
				''' Returns the column number of an index in the table.
				''' </summary>
				''' <param name="index"> the zero-based index in the table </param>
				''' <returns> the zero-based column of the table if one exists;
				''' otherwise -1. </returns>
				Public Overridable Function getAccessibleColumn(ByVal index As Integer) As Integer
					If outerInstance.validateIfNecessary() Then
						Dim numCells As Integer = accessibleColumnCount * accessibleRowCount
						If index >= numCells Then
							Return -1
						Else
							Return index Mod accessibleColumnCount
						End If
					End If
					Return -1
				End Function

				''' <summary>
				''' Returns the index at a row and column in the table.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the zero-based index in the table if one exists;
				''' otherwise -1. </returns>
				Public Overridable Function getAccessibleIndex(ByVal r As Integer, ByVal c As Integer) As Integer
					If outerInstance.validateIfNecessary() Then
						If r >= accessibleRowCount OrElse c >= accessibleColumnCount Then
							Return -1
						Else
							Return r * accessibleColumnCount + c
						End If
					End If
					Return -1
				End Function

				''' <summary>
				''' Returns the row header at a row in a table. </summary>
				''' <param name="r"> zero-based row of the table
				''' </param>
				''' <returns> a String representing the row header
				''' if one exists; otherwise null. </returns>
				Public Overridable Function getAccessibleRowHeader(ByVal r As Integer) As String
					If outerInstance.validateIfNecessary() Then
						Dim cellInfo As TableCellElementInfo = outerInstance.getCell(r, 0)
						If cellInfo.headerCell Then
							Dim v As View = cellInfo.view
							If v IsNot Nothing AndAlso model IsNot Nothing Then
								Try
									Return model.getText(v.startOffset, v.endOffset - v.startOffset)
								Catch e As BadLocationException
									Return Nothing
								End Try
							End If
						End If
					End If
					Return Nothing
				End Function

				''' <summary>
				''' Returns the column header at a column in a table. </summary>
				''' <param name="c"> zero-based column of the table
				''' </param>
				''' <returns> a String representing the column header
				''' if one exists; otherwise null. </returns>
				Public Overridable Function getAccessibleColumnHeader(ByVal c As Integer) As String
					If outerInstance.validateIfNecessary() Then
						Dim cellInfo As TableCellElementInfo = outerInstance.getCell(0, c)
						If cellInfo.headerCell Then
							Dim v As View = cellInfo.view
							If v IsNot Nothing AndAlso model IsNot Nothing Then
								Try
									Return model.getText(v.startOffset, v.endOffset - v.startOffset)
								Catch e As BadLocationException
									Return Nothing
								End Try
							End If
						End If
					End If
					Return Nothing
				End Function

				Public Overridable Sub addRowHeader(ByVal cellInfo As TableCellElementInfo, ByVal rowNumber As Integer)
					If rowHeadersTable Is Nothing Then rowHeadersTable = New AccessibleHeadersTable(Me)
					rowHeadersTable.addHeader(cellInfo, rowNumber)
				End Sub
				' end of AccessibleExtendedTable implementation ------------

				Protected Friend Class AccessibleHeadersTable
					Implements AccessibleTable

					Private ReadOnly outerInstance As AccessibleHTML.TableElementInfo.TableAccessibleContext

					Public Sub New(ByVal outerInstance As AccessibleHTML.TableElementInfo.TableAccessibleContext)
						Me.outerInstance = outerInstance
					End Sub


					' Header information is modeled as a Hashtable of
					' ArrayLists where each Hashtable entry represents
					' a row containing one or more headers.
					Private headers As New Dictionary(Of Integer?, List(Of TableCellElementInfo))
					Private rowCount As Integer = 0
					Private columnCount As Integer = 0

					Public Overridable Sub addHeader(ByVal cellInfo As TableCellElementInfo, ByVal rowNumber As Integer)
						Dim rowInteger As Integer? = Convert.ToInt32(rowNumber)
						Dim list As List(Of TableCellElementInfo) = headers(rowInteger)
						If list Is Nothing Then
							list = New List(Of TableCellElementInfo)
							headers(rowInteger) = list
						End If
						list.Add(cellInfo)
					End Sub

					''' <summary>
					''' Returns the caption for the table.
					''' </summary>
					''' <returns> the caption for the table </returns>
					Public Overridable Property accessibleCaption As Accessible Implements AccessibleTable.getAccessibleCaption
						Get
							Return Nothing
						End Get
						Set(ByVal a As Accessible)
						End Set
					End Property


					''' <summary>
					''' Returns the summary description of the table.
					''' </summary>
					''' <returns> the summary description of the table </returns>
					Public Overridable Property accessibleSummary As Accessible Implements AccessibleTable.getAccessibleSummary
						Get
							Return Nothing
						End Get
						Set(ByVal a As Accessible)
						End Set
					End Property


					''' <summary>
					''' Returns the number of rows in the table.
					''' </summary>
					''' <returns> the number of rows in the table </returns>
					Public Overridable Property accessibleRowCount As Integer Implements AccessibleTable.getAccessibleRowCount
						Get
							Return rowCount
						End Get
					End Property

					''' <summary>
					''' Returns the number of columns in the table.
					''' </summary>
					''' <returns> the number of columns in the table </returns>
					Public Overridable Property accessibleColumnCount As Integer Implements AccessibleTable.getAccessibleColumnCount
						Get
							Return columnCount
						End Get
					End Property

					Private Function getElementInfoAt(ByVal r As Integer, ByVal c As Integer) As TableCellElementInfo
						Dim list As List(Of TableCellElementInfo) = headers(Convert.ToInt32(r))
						If list IsNot Nothing Then
							Return list(c)
						Else
							Return Nothing
						End If
					End Function

					''' <summary>
					''' Returns the Accessible at a specified row and column
					''' in the table.
					''' </summary>
					''' <param name="r"> zero-based row of the table </param>
					''' <param name="c"> zero-based column of the table </param>
					''' <returns> the Accessible at the specified row and column </returns>
					Public Overridable Function getAccessibleAt(ByVal r As Integer, ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleAt
						Dim elementInfo As ElementInfo = getElementInfoAt(r, c)
						If TypeOf elementInfo Is Accessible Then
							Return CType(elementInfo, Accessible)
						Else
							Return Nothing
						End If
					End Function

					''' <summary>
					''' Returns the number of rows occupied by the Accessible at
					''' a specified row and column in the table.
					''' </summary>
					''' <returns> the number of rows occupied by the Accessible at a
					''' given specified (row, column) </returns>
					Public Overridable Function getAccessibleRowExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleRowExtentAt
						Dim elementInfo As TableCellElementInfo = getElementInfoAt(r, c)
						If elementInfo IsNot Nothing Then
							Return elementInfo.rowCount
						Else
							Return 0
						End If
					End Function

					''' <summary>
					''' Returns the number of columns occupied by the Accessible at
					''' a specified row and column in the table.
					''' </summary>
					''' <returns> the number of columns occupied by the Accessible at a
					''' given specified row and column </returns>
					Public Overridable Function getAccessibleColumnExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleColumnExtentAt
						Dim elementInfo As TableCellElementInfo = getElementInfoAt(r, c)
						If elementInfo IsNot Nothing Then
							Return elementInfo.rowCount
						Else
							Return 0
						End If
					End Function

					''' <summary>
					''' Returns the row headers as an AccessibleTable.
					''' </summary>
					''' <returns> an AccessibleTable representing the row
					''' headers </returns>
					Public Overridable Property accessibleRowHeader As AccessibleTable Implements AccessibleTable.getAccessibleRowHeader
						Get
							Return Nothing
						End Get
						Set(ByVal table As AccessibleTable)
						End Set
					End Property


					''' <summary>
					''' Returns the column headers as an AccessibleTable.
					''' </summary>
					''' <returns> an AccessibleTable representing the column
					''' headers </returns>
					Public Overridable Property accessibleColumnHeader As AccessibleTable Implements AccessibleTable.getAccessibleColumnHeader
						Get
							Return Nothing
						End Get
						Set(ByVal table As AccessibleTable)
						End Set
					End Property


					''' <summary>
					''' Returns the description of the specified row in the table.
					''' </summary>
					''' <param name="r"> zero-based row of the table </param>
					''' <returns> the description of the row </returns>
					Public Overridable Function getAccessibleRowDescription(ByVal r As Integer) As Accessible Implements AccessibleTable.getAccessibleRowDescription
						Return Nothing
					End Function

					''' <summary>
					''' Sets the description text of the specified row of the table.
					''' </summary>
					''' <param name="r"> zero-based row of the table </param>
					''' <param name="a"> the description of the row </param>
					Public Overridable Sub setAccessibleRowDescription(ByVal r As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleRowDescription
					End Sub

					''' <summary>
					''' Returns the description text of the specified column in the table.
					''' </summary>
					''' <param name="c"> zero-based column of the table </param>
					''' <returns> the text description of the column </returns>
					Public Overridable Function getAccessibleColumnDescription(ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleColumnDescription
						Return Nothing
					End Function

					''' <summary>
					''' Sets the description text of the specified column in the table.
					''' </summary>
					''' <param name="c"> zero-based column of the table </param>
					''' <param name="a"> the text description of the column </param>
					Public Overridable Sub setAccessibleColumnDescription(ByVal c As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleColumnDescription
					End Sub

					''' <summary>
					''' Returns a boolean value indicating whether the accessible at
					''' a specified row and column is selected.
					''' </summary>
					''' <param name="r"> zero-based row of the table </param>
					''' <param name="c"> zero-based column of the table </param>
					''' <returns> the boolean value true if the accessible at the
					''' row and column is selected. Otherwise, the boolean value
					''' false </returns>
					Public Overridable Function isAccessibleSelected(ByVal r As Integer, ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleSelected
						Return False
					End Function

					''' <summary>
					''' Returns a boolean value indicating whether the specified row
					''' is selected.
					''' </summary>
					''' <param name="r"> zero-based row of the table </param>
					''' <returns> the boolean value true if the specified row is selected.
					''' Otherwise, false. </returns>
					Public Overridable Function isAccessibleRowSelected(ByVal r As Integer) As Boolean Implements AccessibleTable.isAccessibleRowSelected
						Return False
					End Function

					''' <summary>
					''' Returns a boolean value indicating whether the specified column
					''' is selected.
					''' </summary>
					''' <param name="c"> zero-based column of the table </param>
					''' <returns> the boolean value true if the specified column is selected.
					''' Otherwise, false. </returns>
					Public Overridable Function isAccessibleColumnSelected(ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleColumnSelected
						Return False
					End Function

					''' <summary>
					''' Returns the selected rows in a table.
					''' </summary>
					''' <returns> an array of selected rows where each element is a
					''' zero-based row of the table </returns>
					Public Overridable Property selectedAccessibleRows As Integer()
						Get
							Return New Integer (){}
						End Get
					End Property

					''' <summary>
					''' Returns the selected columns in a table.
					''' </summary>
					''' <returns> an array of selected columns where each element is a
					''' zero-based column of the table </returns>
					Public Overridable Property selectedAccessibleColumns As Integer()
						Get
							Return New Integer (){}
						End Get
					End Property
				End Class
			End Class ' ... end AccessibleHeadersTable

	'        
	'         * ElementInfo for table rows
	'         
			Private Class TableRowElementInfo
				Inherits ElementInfo

				Private ReadOnly outerInstance As AccessibleHTML.TableElementInfo


				Private parent As TableElementInfo
				Private rowNumber As Integer

				Friend Sub New(ByVal outerInstance As AccessibleHTML.TableElementInfo, ByVal e As Element, ByVal parent As TableElementInfo, ByVal rowNumber As Integer)
						Me.outerInstance = outerInstance
					MyBase.New(e, parent)
					Me.parent = parent
					Me.rowNumber = rowNumber
				End Sub

				Protected Friend Overrides Sub loadChildren(ByVal e As Element)
					For x As Integer = 0 To e.elementCount - 1
						Dim attrs As AttributeSet = e.getElement(x).attributes

						If attrs.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.TH Then
							Dim headerElementInfo As New TableCellElementInfo(e.getElement(x), Me, True)
							addChild(headerElementInfo)

							Dim at As AccessibleTable = parent.accessibleContext.accessibleTable
							Dim tableElement As TableAccessibleContext = CType(at, TableAccessibleContext)
							tableElement.addRowHeader(headerElementInfo, rowNumber)

						ElseIf attrs.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.TD Then
							addChild(New TableCellElementInfo(e.getElement(x), Me, False))
						End If
					Next x
				End Sub

				''' <summary>
				''' Returns the max of the rowspans of the cells in this row.
				''' </summary>
				Public Overridable Property rowCount As Integer
					Get
						Dim ___rowCount As Integer = 1
						If validateIfNecessary() Then
							For counter As Integer = 0 To childCount - 1
    
								Dim cell As TableCellElementInfo = CType(getChild(counter), TableCellElementInfo)
    
								If cell.validateIfNecessary() Then ___rowCount = Math.Max(___rowCount, cell.rowCount)
							Next counter
						End If
						Return ___rowCount
					End Get
				End Property

				''' <summary>
				''' Returns the sum of the column spans of the individual
				''' cells in this row.
				''' </summary>
				Public Overridable Property columnCount As Integer
					Get
						Dim colCount As Integer = 0
						If validateIfNecessary() Then
							For counter As Integer = 0 To childCount - 1
								Dim cell As TableCellElementInfo = CType(getChild(counter), TableCellElementInfo)
    
								If cell.validateIfNecessary() Then colCount += cell.columnCount
							Next counter
						End If
						Return colCount
					End Get
				End Property

				''' <summary>
				''' Overriden to invalidate the table as well as
				''' TableRowElementInfo.
				''' </summary>
				Protected Friend Overrides Sub invalidate(ByVal first As Boolean)
					MyBase.invalidate(first)
					parent.invalidate(True)
				End Sub

				''' <summary>
				''' Places the TableCellElementInfos for this element in
				''' the grid.
				''' </summary>
				Private Sub updateGrid(ByVal row As Integer)
					If validateIfNecessary() Then
						Dim emptyRow As Boolean = False

						Do While Not emptyRow
							For counter As Integer = 0 To outerInstance.grid(row).Length - 1
								If outerInstance.grid(row)(counter) Is Nothing Then
									emptyRow = True
									Exit For
								End If
							Next counter
							If Not emptyRow Then row += 1
						Loop
						Dim col As Integer = 0
						Dim counter As Integer = 0
						Do While counter < childCount
							Dim cell As TableCellElementInfo = CType(getChild(counter), TableCellElementInfo)

							Do While outerInstance.grid(row)(col) IsNot Nothing
								col += 1
							Loop
							For ___rowCount As Integer = cell.rowCount - 1 To 0 Step -1
								For colCount As Integer = cell.columnCount - 1 To 0 Step -1
									outerInstance.grid(row + ___rowCount)(col + colCount) = cell
								Next colCount
							Next ___rowCount
							col += cell.columnCount
							counter += 1
						Loop
					End If
				End Sub

				''' <summary>
				''' Returns the column count of the number of columns that have
				''' a rowcount >= rowspan.
				''' </summary>
				Private Function getColumnCount(ByVal rowspan As Integer) As Integer
					If validateIfNecessary() Then
						Dim cols As Integer = 0
						For counter As Integer = 0 To childCount - 1
							Dim cell As TableCellElementInfo = CType(getChild(counter), TableCellElementInfo)

							If cell.rowCount >= rowspan Then cols += cell.columnCount
						Next counter
						Return cols
					End If
					Return 0
				End Function
			End Class

			''' <summary>
			''' TableCellElementInfo is used to represents the cells of
			''' the table.
			''' </summary>
			Private Class TableCellElementInfo
				Inherits ElementInfo

				Private ReadOnly outerInstance As AccessibleHTML.TableElementInfo


				Private accessible As Accessible
				Private ___isHeaderCell As Boolean

				Friend Sub New(ByVal outerInstance As AccessibleHTML.TableElementInfo, ByVal e As Element, ByVal parent As ElementInfo)
						Me.outerInstance = outerInstance
					MyBase.New(e, parent)
					Me.___isHeaderCell = False
				End Sub

				Friend Sub New(ByVal outerInstance As AccessibleHTML.TableElementInfo, ByVal e As Element, ByVal parent As ElementInfo, ByVal isHeaderCell As Boolean)
						Me.outerInstance = outerInstance
					MyBase.New(e, parent)
					Me.___isHeaderCell = isHeaderCell
				End Sub

	'            
	'             * Returns whether this table cell is a header
	'             
				Public Overridable Property headerCell As Boolean
					Get
						Return Me.___isHeaderCell
					End Get
				End Property

	'            
	'             * Returns the Accessible representing this table cell
	'             
				Public Overridable Property accessible As Accessible
					Get
						accessible = Nothing
						getAccessible(Me)
						Return accessible
					End Get
				End Property

	'            
	'             * Gets the outermost Accessible in the table cell
	'             
				Private Sub getAccessible(ByVal elementInfo As ElementInfo)
					If TypeOf elementInfo Is Accessible Then
						accessible = CType(elementInfo, Accessible)
					Else
						For i As Integer = 0 To elementInfo.childCount - 1
							getAccessible(elementInfo.getChild(i))
						Next i
					End If
				End Sub

				''' <summary>
				''' Returns the rowspan attribute.
				''' </summary>
				Public Overridable Property rowCount As Integer
					Get
						If validateIfNecessary() Then Return Math.Max(1, getIntAttr(attributes, HTML.Attribute.ROWSPAN, 1))
						Return 0
					End Get
				End Property

				''' <summary>
				''' Returns the colspan attribute.
				''' </summary>
				Public Overridable Property columnCount As Integer
					Get
						If validateIfNecessary() Then Return Math.Max(1, getIntAttr(attributes, HTML.Attribute.COLSPAN, 1))
						Return 0
					End Get
				End Property

				''' <summary>
				''' Overriden to invalidate the TableRowElementInfo as well as
				''' the TableCellElementInfo.
				''' </summary>
				Protected Friend Overrides Sub invalidate(ByVal first As Boolean)
					MyBase.invalidate(first)
					parent.invalidate(True)
				End Sub
			End Class
		End Class


		''' <summary>
		''' ElementInfo provides a slim down view of an Element.  Each ElementInfo
		''' can have any number of child ElementInfos that are not necessarily
		''' direct children of the Element. As the Document changes various
		''' ElementInfos become invalidated. Before accessing a particular portion
		''' of an ElementInfo you should make sure it is valid by invoking
		''' <code>validateIfNecessary</code>, this will return true if
		''' successful, on the other hand a false return value indicates the
		''' ElementInfo is not valid and can never become valid again (usually
		''' the result of the Element the ElementInfo encapsulates being removed).
		''' </summary>
		Private Class ElementInfo
			Private ReadOnly outerInstance As AccessibleHTML


			''' <summary>
			''' The children of this ElementInfo.
			''' </summary>
			Private children As List(Of ElementInfo)
			''' <summary>
			''' The Element this ElementInfo is providing information for.
			''' </summary>
			Private element As Element
			''' <summary>
			''' The parent ElementInfo, will be null for the root.
			''' </summary>
			Private parent As ElementInfo
			''' <summary>
			''' Indicates the validity of the ElementInfo.
			''' </summary>
			Private ___isValid As Boolean
			''' <summary>
			''' Indicates if the ElementInfo can become valid.
			''' </summary>
			Private canBeValid As Boolean


			''' <summary>
			''' Creates the root ElementInfo.
			''' </summary>
			Friend Sub New(ByVal outerInstance As AccessibleHTML, ByVal element As Element)
					Me.outerInstance = outerInstance
				Me.New(element, Nothing)
			End Sub

			''' <summary>
			''' Creates an ElementInfo representing <code>element</code> with
			''' the specified parent.
			''' </summary>
			Friend Sub New(ByVal outerInstance As AccessibleHTML, ByVal element As Element, ByVal parent As ElementInfo)
					Me.outerInstance = outerInstance
				Me.element = element
				Me.parent = parent
				___isValid = False
				canBeValid = True
			End Sub

			''' <summary>
			''' Validates the receiver. This recreates the children as well. This
			''' will be invoked within a <code>readLock</code>. If this is overriden
			''' it MUST invoke supers implementation first!
			''' </summary>
			Protected Friend Overridable Sub validate()
				___isValid = True
				loadChildren(element)
			End Sub

			''' <summary>
			''' Recreates the direct children of <code>info</code>.
			''' </summary>
			Protected Friend Overridable Sub loadChildren(ByVal parent As Element)
				If Not parent.leaf Then
					Dim counter As Integer = 0
					Dim maxCounter As Integer = parent.elementCount
					Do While counter < maxCounter
						Dim e As Element = parent.getElement(counter)
						Dim childInfo As ElementInfo = outerInstance.createElementInfo(e, Me)

						If childInfo IsNot Nothing Then
							addChild(childInfo)
						Else
							loadChildren(e)
						End If
						counter += 1
					Loop
				End If
			End Sub

			''' <summary>
			''' Returns the index of the child in the parent, or -1 for the
			''' root or if the parent isn't valid.
			''' </summary>
			Public Overridable Property indexInParent As Integer
				Get
					If parent Is Nothing OrElse (Not parent.valid) Then Return -1
					Return parent.IndexOf(Me)
				End Get
			End Property

			''' <summary>
			''' Returns the Element this <code>ElementInfo</code> represents.
			''' </summary>
			Public Overridable Property element As Element
				Get
					Return element
				End Get
			End Property

			''' <summary>
			''' Returns the parent of this Element, or null for the root.
			''' </summary>
			Public Overridable Property parent As ElementInfo
				Get
					Return parent
				End Get
			End Property

			''' <summary>
			''' Returns the index of the specified child, or -1 if
			''' <code>child</code> isn't a valid child.
			''' </summary>
			Public Overridable Function indexOf(ByVal child As ElementInfo) As Integer
				Dim children As ArrayList = Me.children

				If children IsNot Nothing Then Return children.IndexOf(child)
				Return -1
			End Function

			''' <summary>
			''' Returns the child ElementInfo at <code>index</code>, or null
			''' if <code>index</code> isn't a valid index.
			''' </summary>
			Public Overridable Function getChild(ByVal index As Integer) As ElementInfo
				If validateIfNecessary() Then
					Dim children As List(Of ElementInfo) = Me.children

					If children IsNot Nothing AndAlso index >= 0 AndAlso index < children.Count Then Return children(index)
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns the number of children the ElementInfo contains.
			''' </summary>
			Public Overridable Property childCount As Integer
				Get
					validateIfNecessary()
					Return If(children Is Nothing, 0, children.Count)
				End Get
			End Property

			''' <summary>
			''' Adds a new child to this ElementInfo.
			''' </summary>
			Protected Friend Overridable Sub addChild(ByVal child As ElementInfo)
				If children Is Nothing Then children = New List(Of ElementInfo)
				children.Add(child)
			End Sub

			''' <summary>
			''' Returns the View corresponding to this ElementInfo, or null
			''' if the ElementInfo can't be validated.
			''' </summary>
			Protected Friend Overridable Property view As View
				Get
					If Not validateIfNecessary() Then Return Nothing
					Dim lock As Object = outerInstance.lock()
					Try
						Dim rootView As View = outerInstance.rootView
						Dim e As Element = element
						Dim start As Integer = e.startOffset
    
						If rootView IsNot Nothing Then Return getView(rootView, e, start)
						Return Nothing
					Finally
						outerInstance.unlock(lock)
					End Try
				End Get
			End Property

			''' <summary>
			''' Returns the Bounds for this ElementInfo, or null
			''' if the ElementInfo can't be validated.
			''' </summary>
			Public Overridable Property bounds As Rectangle
				Get
					If Not validateIfNecessary() Then Return Nothing
					Dim lock As Object = outerInstance.lock()
					Try
						Dim ___bounds As Rectangle = outerInstance.rootEditorRect
						Dim rootView As View = outerInstance.rootView
						Dim e As Element = element
    
						If ___bounds IsNot Nothing AndAlso rootView IsNot Nothing Then
							Try
								Return rootView.modelToView(e.startOffset, Position.Bias.Forward, e.endOffset, Position.Bias.Backward, ___bounds).bounds
							Catch ble As BadLocationException
							End Try
						End If
					Finally
						outerInstance.unlock(lock)
					End Try
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Returns true if this ElementInfo is valid.
			''' </summary>
			Protected Friend Overridable Property valid As Boolean
				Get
					Return ___isValid
				End Get
			End Property

			''' <summary>
			''' Returns the AttributeSet associated with the Element, this will
			''' return null if the ElementInfo can't be validated.
			''' </summary>
			Protected Friend Overridable Property attributes As AttributeSet
				Get
					If validateIfNecessary() Then Return element.attributes
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Returns the AttributeSet associated with the View that is
			''' representing this Element, this will
			''' return null if the ElementInfo can't be validated.
			''' </summary>
			Protected Friend Overridable Property viewAttributes As AttributeSet
				Get
					If validateIfNecessary() Then
						Dim ___view As View = view
    
						If ___view IsNot Nothing Then Return ___view.element.attributes
						Return element.attributes
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Convenience method for getting an integer attribute from the passed
			''' in AttributeSet.
			''' </summary>
			Protected Friend Overridable Function getIntAttr(ByVal attrs As AttributeSet, ByVal key As Object, ByVal deflt As Integer) As Integer
				If attrs IsNot Nothing AndAlso attrs.isDefined(key) Then
					Dim i As Integer
					Dim val As String = CStr(attrs.getAttribute(key))
					If val Is Nothing Then
						i = deflt
					Else
						Try
							i = Math.Max(0, Convert.ToInt32(val))
						Catch x As NumberFormatException
							i = deflt
						End Try
					End If
					Return i
				End If
				Return deflt
			End Function

			''' <summary>
			''' Validates the ElementInfo if necessary.  Some ElementInfos may
			''' never be valid again.  You should check <code>isValid</code> before
			''' using one.  This will reload the children and invoke
			''' <code>validate</code> if the ElementInfo is invalid and can become
			''' valid again. This will return true if the receiver is valid.
			''' </summary>
			Protected Friend Overridable Function validateIfNecessary() As Boolean
				If (Not valid) AndAlso canBeValid Then
					children = Nothing
					Dim lock As Object = outerInstance.lock()

					Try
						validate()
					Finally
						outerInstance.unlock(lock)
					End Try
				End If
				Return valid
			End Function

			''' <summary>
			''' Invalidates the ElementInfo. Subclasses should override this
			''' if they need to reset state once invalid.
			''' </summary>
			Protected Friend Overridable Sub invalidate(ByVal first As Boolean)
				If Not valid Then
					If canBeValid AndAlso (Not first) Then canBeValid = False
					Return
				End If
				___isValid = False
				canBeValid = first
				If children IsNot Nothing Then
					For Each ___child As ElementInfo In children
						___child.invalidate(False)
					Next ___child
					children = Nothing
				End If
			End Sub

			Private Function getView(ByVal parent As View, ByVal e As Element, ByVal start As Integer) As View
				If parent.element Is e Then Return parent
				Dim index As Integer = parent.getViewIndex(start, Position.Bias.Forward)

				If index <> -1 AndAlso index < parent.viewCount Then Return getView(parent.getView(index), e, start)
				Return Nothing
			End Function

			Private Function getClosestInfoIndex(ByVal index As Integer) As Integer
				For counter As Integer = 0 To childCount - 1
					Dim info As ElementInfo = getChild(counter)

					If index < info.element.endOffset OrElse index = info.element.startOffset Then Return counter
				Next counter
				Return -1
			End Function

			Private Sub update(ByVal e As DocumentEvent)
				If Not valid Then Return
				Dim ___parent As ElementInfo = parent
				Dim ___element As Element = element

				Do
					Dim ec As DocumentEvent.ElementChange = e.getChange(___element)
					If ec IsNot Nothing Then
						If ___element Is element Then
							' One of our children changed.
							invalidate(True)
						ElseIf ___parent IsNot Nothing Then
							___parent.invalidate(___parent Is outerInstance.rootInfo)
						End If
						Return
					End If
					___element = ___element.parentElement
				Loop While ___parent IsNot Nothing AndAlso ___element IsNot Nothing AndAlso ___element IsNot ___parent.element

				If childCount > 0 Then
					Dim elem As Element = element
					Dim pos As Integer = e.offset
					Dim index0 As Integer = getClosestInfoIndex(pos)
					If index0 = -1 AndAlso e.type Is DocumentEvent.EventType.REMOVE AndAlso pos >= elem.endOffset Then index0 = childCount - 1
					Dim info As ElementInfo = If(index0 >= 0, getChild(index0), Nothing)
					If info IsNot Nothing AndAlso (info.element.startOffset = pos) AndAlso (pos > 0) Then index0 = Math.Max(index0 - 1, 0)
					Dim index1 As Integer
					If e.type IsNot DocumentEvent.EventType.REMOVE Then
						index1 = getClosestInfoIndex(pos + e.length)
						If index1 < 0 Then index1 = childCount - 1
					Else
						index1 = index0
						' A remove may result in empty elements.
						Do While (index1 + 1) < childCount AndAlso getChild(index1 + 1).element.endOffset = getChild(index1 + 1).element.startOffset
							index1 += 1
						Loop
					End If
					index0 = Math.Max(index0, 0)
					' The check for isValid is here as in the process of
					' forwarding update our child may invalidate us.
					Dim i As Integer = index0
					Do While i <= index1 AndAlso valid
						getChild(i).update(e)
						i += 1
					Loop
				End If
			End Sub
		End Class

		''' <summary>
		''' DocumentListener installed on the current Document.  Will invoke
		''' <code>update</code> on the <code>RootInfo</code> in response to
		''' any event.
		''' </summary>
		Private Class DocumentHandler
			Implements DocumentListener

			Private ReadOnly outerInstance As AccessibleHTML

			Public Sub New(ByVal outerInstance As AccessibleHTML)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
				outerInstance.rootInfo.update(e)
			End Sub
			Public Overridable Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
				outerInstance.rootInfo.update(e)
			End Sub
			Public Overridable Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
				outerInstance.rootInfo.update(e)
			End Sub
		End Class

	'    
	'     * PropertyChangeListener installed on the editor.
	'     
		Private Class PropertyChangeHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As AccessibleHTML

			Public Sub New(ByVal outerInstance As AccessibleHTML)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
				If evt.propertyName.Equals("document") Then outerInstance.document = outerInstance.editor.document
			End Sub
		End Class
	End Class

End Namespace
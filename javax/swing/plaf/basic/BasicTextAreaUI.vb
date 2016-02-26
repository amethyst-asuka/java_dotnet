Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.text
Imports javax.swing.plaf

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
Namespace javax.swing.plaf.basic

	''' <summary>
	''' Provides the look and feel for a plain text editor.  In this
	''' implementation the default UI is extended to act as a simple
	''' view factory.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class BasicTextAreaUI
		Inherits BasicTextUI

		''' <summary>
		''' Creates a UI for a JTextArea.
		''' </summary>
		''' <param name="ta"> a text area </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal ta As JComponent) As ComponentUI
			Return New BasicTextAreaUI
		End Function

		''' <summary>
		''' Constructs a new BasicTextAreaUI object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Fetches the name used as a key to look up properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name ("TextArea") </returns>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "TextArea"
			End Get
		End Property

		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			'the fix for 4785160 is undone
		End Sub

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' This is implemented to rebuild the View when the
		''' <em>WrapLine</em> or the <em>WrapStyleWord</em> property changes.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			MyBase.propertyChange(evt)
			If evt.propertyName.Equals("lineWrap") OrElse evt.propertyName.Equals("wrapStyleWord") OrElse evt.propertyName.Equals("tabSize") Then
				' rebuild the view
				modelChanged()
			ElseIf "editable".Equals(evt.propertyName) Then
				updateFocusTraversalKeys()
			End If
		End Sub


		''' <summary>
		''' The method is overridden to take into account caret width.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <returns> the preferred size </returns>
		''' <exception cref="IllegalArgumentException"> if invalid value is passed
		''' 
		''' @since 1.5 </exception>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Return MyBase.getPreferredSize(c)
			'the fix for 4785160 is undone
		End Function

		''' <summary>
		''' The method is overridden to take into account caret width.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <returns> the minimum size </returns>
		''' <exception cref="IllegalArgumentException"> if invalid value is passed
		''' 
		''' @since 1.5 </exception>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As Dimension
			Return MyBase.getMinimumSize(c)
			'the fix for 4785160 is undone
		End Function

		''' <summary>
		''' Creates the view for an element.  Returns a WrappedPlainView or
		''' PlainView.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <returns> the view </returns>
		Public Overrides Function create(ByVal elem As Element) As View
			Dim doc As Document = elem.document
			Dim i18nFlag As Object = doc.getProperty("i18n") 'AbstractDocument.I18NProperty
			If (i18nFlag IsNot Nothing) AndAlso i18nFlag.Equals(Boolean.TRUE) Then
				' build a view that support bidi
				Return createI18N(elem)
			Else
				Dim c As JTextComponent = component
				If TypeOf c Is JTextArea Then
					Dim area As JTextArea = CType(c, JTextArea)
					Dim v As View
					If area.lineWrap Then
						v = New WrappedPlainView(elem, area.wrapStyleWord)
					Else
						v = New PlainView(elem)
					End If
					Return v
				End If
			End If
			Return Nothing
		End Function

		Friend Overridable Function createI18N(ByVal elem As Element) As View
			Dim kind As String = elem.name
			If kind IsNot Nothing Then
				If kind.Equals(AbstractDocument.ContentElementName) Then
					Return New PlainParagraph(elem)
				ElseIf kind.Equals(AbstractDocument.ParagraphElementName) Then
					Return New BoxView(elem, View.Y_AXIS)
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim i18nFlag As Object = CType(c, JTextComponent).document.getProperty("i18n")
			Dim insets As Insets = c.insets
			If Boolean.TRUE.Equals(i18nFlag) Then
				Dim ___rootView As View = getRootView(CType(c, JTextComponent))
				If ___rootView.viewCount > 0 Then
					height = height - insets.top - insets.bottom
					Dim ___baseline As Integer = insets.top
					Dim fieldBaseline As Integer = BasicHTML.getBaseline(___rootView.getView(0), width - insets.left - insets.right, height)
					If fieldBaseline < 0 Then Return -1
					Return ___baseline + fieldBaseline
				End If
				Return -1
			End If
			Dim fm As FontMetrics = c.getFontMetrics(c.font)
			Return insets.top + fm.ascent
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Return Component.BaselineResizeBehavior.CONSTANT_ASCENT
		End Function


		''' <summary>
		''' Paragraph for representing plain-text lines that support
		''' bidirectional text.
		''' </summary>
		Friend Class PlainParagraph
			Inherits ParagraphView

			Friend Sub New(ByVal elem As Element)
				MyBase.New(elem)
				layoutPool = New LogicalView(elem)
				layoutPool.parent = Me
			End Sub

			Public Overrides Property parent As View
				Set(ByVal parent As View)
					MyBase.parent = parent
					If parent IsNot Nothing Then propertiesFromAttributestes()
				End Set
			End Property

			Protected Friend Overrides Sub setPropertiesFromAttributes()
				Dim c As Component = container
				If (c IsNot Nothing) AndAlso ((Not c.componentOrientation.leftToRight)) Then
					justification = StyleConstants.ALIGN_RIGHT
				Else
					justification = StyleConstants.ALIGN_LEFT
				End If
			End Sub

			''' <summary>
			''' Fetch the constraining span to flow against for
			''' the given child index.
			''' </summary>
			Public Overrides Function getFlowSpan(ByVal index As Integer) As Integer
				Dim c As Component = container
				If TypeOf c Is JTextArea Then
					Dim area As JTextArea = CType(c, JTextArea)
					If Not area.lineWrap Then Return Integer.MaxValue
				End If
				Return MyBase.getFlowSpan(index)
			End Function

			Protected Friend Overridable Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As SizeRequirements) As SizeRequirements
				Dim req As SizeRequirements = MyBase.calculateMinorAxisRequirements(axis, r)
				Dim c As Component = container
				If TypeOf c Is JTextArea Then
					Dim area As JTextArea = CType(c, JTextArea)
					If Not area.lineWrap Then
						' min is pref if unwrapped
						req.minimum = req.preferred
					Else
						req.minimum = 0
						req.preferred = width
						If req.preferred = Integer.MaxValue Then req.preferred = 100
					End If
				End If
				Return req
			End Function

			''' <summary>
			''' Sets the size of the view.  If the size has changed, layout
			''' is redone.  The size is the full size of the view including
			''' the inset areas.
			''' </summary>
			''' <param name="width"> the width >= 0 </param>
			''' <param name="height"> the height >= 0 </param>
			Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
				If CInt(Fix(width)) <> width Then preferenceChanged(Nothing, True, True)
				MyBase.sizeize(width, height)
			End Sub

			''' <summary>
			''' This class can be used to represent a logical view for
			''' a flow.  It keeps the children updated to reflect the state
			''' of the model, gives the logical child views access to the
			''' view hierarchy, and calculates a preferred span.  It doesn't
			''' do any rendering, layout, or model/view translation.
			''' </summary>
			Friend Class LogicalView
				Inherits CompositeView

				Friend Sub New(ByVal elem As Element)
					MyBase.New(elem)
				End Sub

				Protected Friend Overrides Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
					Dim elem As Element = element
					If elem.elementCount > 0 Then Return elem.getElementIndex(pos)
					Return 0
				End Function

				Protected Friend Overridable Function updateChildren(ByVal ec As javax.swing.event.DocumentEvent.ElementChange, ByVal e As javax.swing.event.DocumentEvent, ByVal f As ViewFactory) As Boolean
					Return False
				End Function

				Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
					Dim elem As Element = element
					If elem.elementCount > 0 Then
						MyBase.loadChildren(f)
					Else
						Dim v As View = New GlyphView(elem)
						append(v)
					End If
				End Sub

				Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
					If viewCount <> 1 Then Throw New Exception("One child view is assumed.")

					Dim v As View = getView(0)
					Return v.getPreferredSpan(axis)
				End Function

				''' <summary>
				''' Forward the DocumentEvent to the given child view.  This
				''' is implemented to reparent the child to the logical view
				''' (the children may have been parented by a row in the flow
				''' if they fit without breaking) and then execute the superclass
				''' behavior.
				''' </summary>
				''' <param name="v"> the child view to forward the event to. </param>
				''' <param name="e"> the change information from the associated document </param>
				''' <param name="a"> the current allocation of the view </param>
				''' <param name="f"> the factory to use to rebuild if the view has children </param>
				''' <seealso cref= #forwardUpdate
				''' @since 1.3 </seealso>
				Protected Friend Overridable Sub forwardUpdateToView(ByVal v As View, ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
					v.parent = Me
					MyBase.forwardUpdateToView(v, e, a, f)
				End Sub

				' The following methods don't do anything useful, they
				' simply keep the class from being abstract.

				Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
				End Sub

				Protected Friend Overrides Function isBefore(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean
					Return False
				End Function

				Protected Friend Overrides Function isAfter(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean
					Return False
				End Function

				Protected Friend Overrides Function getViewAtPoint(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As View
					Return Nothing
				End Function

				Protected Friend Overrides Sub childAllocation(ByVal index As Integer, ByVal a As Rectangle)
				End Sub
			End Class
		End Class

	End Class

End Namespace
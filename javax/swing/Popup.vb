'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing



	''' <summary>
	''' Popups are used to display a <code>Component</code> to the user, typically
	''' on top of all the other <code>Component</code>s in a particular containment
	''' hierarchy. <code>Popup</code>s have a very small life cycle. Once you
	''' have obtained a <code>Popup</code>, and hidden it (invoked the
	''' <code>hide</code> method), you should no longer
	''' invoke any methods on it. This allows the <code>PopupFactory</code> to cache
	''' <code>Popup</code>s for later use.
	''' <p>
	''' The general contract is that if you need to change the size of the
	''' <code>Component</code>, or location of the <code>Popup</code>, you should
	''' obtain a new <code>Popup</code>.
	''' <p>
	''' <code>Popup</code> does not descend from <code>Component</code>, rather
	''' implementations of <code>Popup</code> are responsible for creating
	''' and maintaining their own <code>Component</code>s to render the
	''' requested <code>Component</code> to the user.
	''' <p>
	''' You typically do not explicitly create an instance of <code>Popup</code>,
	''' instead obtain one from a <code>PopupFactory</code>.
	''' </summary>
	''' <seealso cref= PopupFactory
	''' 
	''' @since 1.4 </seealso>
	Public Class Popup
		''' <summary>
		''' The Component representing the Popup.
		''' </summary>
		Private component As Component

		''' <summary>
		''' Creates a <code>Popup</code> for the Component <code>owner</code>
		''' containing the Component <code>contents</code>. <code>owner</code>
		''' is used to determine which <code>Window</code> the new
		''' <code>Popup</code> will parent the <code>Component</code> the
		''' <code>Popup</code> creates to.
		''' A null <code>owner</code> implies there is no valid parent.
		''' <code>x</code> and
		''' <code>y</code> specify the preferred initial location to place
		''' the <code>Popup</code> at. Based on screen size, or other paramaters,
		''' the <code>Popup</code> may not display at <code>x</code> and
		''' <code>y</code>.
		''' </summary>
		''' <param name="owner">    Component mouse coordinates are relative to, may be null </param>
		''' <param name="contents"> Contents of the Popup </param>
		''' <param name="x">        Initial x screen coordinate </param>
		''' <param name="y">        Initial y screen coordinate </param>
		''' <exception cref="IllegalArgumentException"> if contents is null </exception>
		Protected Friend Sub New(ByVal owner As Component, ByVal contents As Component, ByVal x As Integer, ByVal y As Integer)
			Me.New()
			If contents Is Nothing Then Throw New System.ArgumentException("Contents must be non-null")
			reset(owner, contents, x, y)
		End Sub

		''' <summary>
		''' Creates a <code>Popup</code>. This is provided for subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Makes the <code>Popup</code> visible. If the <code>Popup</code> is
		''' currently visible, this has no effect.
		''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub show()
			Dim ___component As Component = component

			If ___component IsNot Nothing Then ___component.show()
		End Sub

		''' <summary>
		''' Hides and disposes of the <code>Popup</code>. Once a <code>Popup</code>
		''' has been disposed you should no longer invoke methods on it. A
		''' <code>dispose</code>d <code>Popup</code> may be reclaimed and later used
		''' based on the <code>PopupFactory</code>. As such, if you invoke methods
		''' on a <code>disposed</code> <code>Popup</code>, indeterminate
		''' behavior will result.
		''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub hide()
			Dim ___component As Component = component

			If TypeOf ___component Is JWindow Then
				___component.hide()
				CType(___component, JWindow).contentPane.removeAll()
			End If
			Dispose()
		End Sub

		''' <summary>
		''' Frees any resources the <code>Popup</code> may be holding onto.
		''' </summary>
		Friend Overridable Sub dispose()
			Dim ___component As Component = component
			Dim window As Window = SwingUtilities.getWindowAncestor(___component)

			If TypeOf ___component Is JWindow Then
				CType(___component, Window).Dispose()
				___component = Nothing
			End If
			' If our parent is a DefaultFrame, we need to dispose it, too.
			If TypeOf window Is DefaultFrame Then window.Dispose()
		End Sub

		''' <summary>
		''' Resets the <code>Popup</code> to an initial state.
		''' </summary>
		Friend Overridable Sub reset(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer)
			If component Is Nothing Then component = createComponent(owner)

			Dim c As Component = component

			If TypeOf c Is JWindow Then
				Dim ___component As JWindow = CType(component, JWindow)

				___component.locationion(ownerX, ownerY)
				___component.contentPane.add(contents, BorderLayout.CENTER)
				___component.invalidate()
				___component.validate()
				If ___component.visible Then pack()
			End If
		End Sub


		''' <summary>
		''' Causes the <code>Popup</code> to be sized to fit the preferred size
		''' of the <code>Component</code> it contains.
		''' </summary>
		Friend Overridable Sub pack()
			Dim ___component As Component = component

			If TypeOf ___component Is Window Then CType(___component, Window).pack()
		End Sub

		''' <summary>
		''' Returns the <code>Window</code> to use as the parent of the
		''' <code>Window</code> created for the <code>Popup</code>. This creates
		''' a new <code>DefaultFrame</code>, if necessary.
		''' </summary>
		Private Function getParentWindow(ByVal owner As Component) As Window
			Dim window As Window = Nothing

			If TypeOf owner Is Window Then
				window = CType(owner, Window)
			ElseIf owner IsNot Nothing Then
				window = SwingUtilities.getWindowAncestor(owner)
			End If
			If window Is Nothing Then window = New DefaultFrame
			Return window
		End Function

		''' <summary>
		''' Creates the Component to use as the parent of the <code>Popup</code>.
		''' The default implementation creates a <code>Window</code>, subclasses
		''' should override.
		''' </summary>
		Friend Overridable Function createComponent(ByVal owner As Component) As Component
			If GraphicsEnvironment.headless Then Return Nothing
			Return New HeavyWeightWindow(getParentWindow(owner))
		End Function

		''' <summary>
		''' Returns the <code>Component</code> returned from
		''' <code>createComponent</code> that will hold the <code>Popup</code>.
		''' </summary>
		Friend Overridable Property component As Component
			Get
				Return component
			End Get
		End Property


		''' <summary>
		''' Component used to house window.
		''' </summary>
		Friend Class HeavyWeightWindow
			Inherits JWindow
			Implements sun.awt.ModalExclude

			Friend Sub New(ByVal parent As Window)
				MyBase.New(parent)
				focusableWindowState = False
				type = Window.Type.POPUP

				' Popups are typically transient and most likely won't benefit
				' from true double buffering.  Turn it off here.
				rootPane.useTrueDoubleBuffering = False
				' Try to set "always-on-top" for the popup window.
				' Applets usually don't have sufficient permissions to do it.
				' In this case simply ignore the exception.
				Try
					alwaysOnTop = True
				Catch se As SecurityException
					' setAlwaysOnTop is restricted,
					' the exception is ignored
				End Try
			End Sub

			Public Overrides Sub update(ByVal g As Graphics)
				paint(g)
			End Sub

			Public Overridable Sub show()
				Me.pack()
				If width > 0 AndAlso height > 0 Then MyBase.show()
			End Sub
		End Class


		''' <summary>
		''' Used if no valid Window ancestor of the supplied owner is found.
		''' <p>
		''' PopupFactory uses this as a way to know when the Popup shouldn't
		''' be cached based on the Window.
		''' </summary>
		Friend Class DefaultFrame
			Inherits Frame

		End Class
	End Class

End Namespace
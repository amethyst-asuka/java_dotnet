Imports System

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
Namespace javax.swing



	''' <summary>
	''' This class has been obsoleted by the 1.4 focus APIs. While client code may
	''' still use this class, developers are strongly encouraged to use
	''' <code>java.awt.KeyboardFocusManager</code> and
	''' <code>java.awt.DefaultKeyboardFocusManager</code> instead.
	''' <p>
	''' Please see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	''' How to Use the Focus Subsystem</a>,
	''' a section in <em>The Java Tutorial</em>, and the
	''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	''' for more information.
	''' </summary>
	''' <seealso cref= <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	''' 
	''' @author Arnaud Weber
	''' @author David Mendenhall </seealso>
	Public MustInherit Class FocusManager
		Inherits DefaultKeyboardFocusManager

		''' <summary>
		''' This field is obsolete, and its use is discouraged since its
		''' specification is incompatible with the 1.4 focus APIs.
		''' The current FocusManager is no longer a property of the UI.
		''' Client code must query for the current FocusManager using
		''' <code>KeyboardFocusManager.getCurrentKeyboardFocusManager()</code>.
		''' See the Focus Specification for more information.
		''' </summary>
		''' <seealso cref= java.awt.KeyboardFocusManager#getCurrentKeyboardFocusManager </seealso>
		''' <seealso cref= <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a> </seealso>
		Public Const FOCUS_MANAGER_CLASS_PROPERTY As String = "FocusManagerClassName"

		Private Shared enabled As Boolean = True

		''' <summary>
		''' Returns the current <code>KeyboardFocusManager</code> instance
		''' for the calling thread's context.
		''' </summary>
		''' <returns> this thread's context's <code>KeyboardFocusManager</code> </returns>
		''' <seealso cref= #setCurrentManager </seealso>
		Public Property Shared currentManager As FocusManager
			Get
				Dim manager As KeyboardFocusManager = KeyboardFocusManager.currentKeyboardFocusManager
				If TypeOf manager Is FocusManager Then
					Return CType(manager, FocusManager)
				Else
					Return New DelegatingDefaultFocusManager(manager)
				End If
			End Get
			Set(ByVal aFocusManager As FocusManager)
				' Note: This method is not backward-compatible with 1.3 and earlier
				' releases. It now throws a SecurityException in an applet, whereas
				' in previous releases, it did not. This issue was discussed at
				' length, and ultimately approved by Hans.
				Dim toSet As KeyboardFocusManager = If(TypeOf aFocusManager Is DelegatingDefaultFocusManager, CType(aFocusManager, DelegatingDefaultFocusManager).delegate, aFocusManager)
				KeyboardFocusManager.currentKeyboardFocusManager = toSet
			End Set
		End Property


		''' <summary>
		''' Changes the current <code>KeyboardFocusManager</code>'s default
		''' <code>FocusTraversalPolicy</code> to
		''' <code>DefaultFocusTraversalPolicy</code>.
		''' </summary>
		''' <seealso cref= java.awt.DefaultFocusTraversalPolicy </seealso>
		''' <seealso cref= java.awt.KeyboardFocusManager#setDefaultFocusTraversalPolicy </seealso>
		''' @deprecated as of 1.4, replaced by
		''' <code>KeyboardFocusManager.setDefaultFocusTraversalPolicy(FocusTraversalPolicy)</code> 
		<Obsolete("as of 1.4, replaced by")> _
		Public Shared Sub disableSwingFocusManager()
			If enabled Then
				enabled = False
				KeyboardFocusManager.currentKeyboardFocusManager.defaultFocusTraversalPolicy = New DefaultFocusTraversalPolicy
			End If
		End Sub

		''' <summary>
		''' Returns whether the application has invoked
		''' <code>disableSwingFocusManager()</code>.
		''' </summary>
		''' <seealso cref= #disableSwingFocusManager </seealso>
		''' @deprecated As of 1.4, replaced by
		'''   <code>KeyboardFocusManager.getDefaultFocusTraversalPolicy()</code> 
		<Obsolete("As of 1.4, replaced by")> _
		Public Property Shared focusManagerEnabled As Boolean
			Get
				Return enabled
			End Get
		End Property
	End Class

End Namespace
'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.accessibility


	''' <summary>
	''' <P>Class AccessibleState describes a component's particular state.  The actual
	''' state of the component is defined as an AccessibleStateSet, which is a
	''' composed set of AccessibleStates.
	''' <p>The toDisplayString method allows you to obtain the localized string
	''' for a locale independent key from a predefined ResourceBundle for the
	''' keys defined in this class.
	''' <p>The constants in this class present a strongly typed enumeration
	''' of common object roles.  A public constructor for this class has been
	''' purposely omitted and applications should use one of the constants
	''' from this class.  If the constants in this class are not sufficient
	''' to describe the role of an object, a subclass should be generated
	''' from this class and it should provide constants in a similar manner.
	''' 
	''' @author      Willie Walker
	''' @author      Peter Korn
	''' </summary>
	Public Class AccessibleState
		Inherits AccessibleBundle

		' If you add or remove anything from here, make sure you
		' update AccessibleResourceBundle.java.

		''' <summary>
		''' Indicates a window is currently the active window.  This includes
		''' windows, dialogs, frames, etc.  In addition, this state is used
		''' to indicate the currently active child of a component such as a
		''' list, table, or tree.  For example, the active child of a list
		''' is the child that is drawn with a rectangle around it. </summary>
		''' <seealso cref= AccessibleRole#WINDOW </seealso>
		''' <seealso cref= AccessibleRole#FRAME </seealso>
		''' <seealso cref= AccessibleRole#DIALOG </seealso>
		Public Shared ReadOnly ACTIVE As New AccessibleState("active")

		''' <summary>
		''' Indicates this object is currently pressed.  This is usually
		''' associated with buttons and indicates the user has pressed a
		''' mouse button while the pointer was over the button and has
		''' not yet released the mouse button. </summary>
		''' <seealso cref= AccessibleRole#PUSH_BUTTON </seealso>
		Public Shared ReadOnly PRESSED As New AccessibleState("pressed")

		''' <summary>
		''' Indicates that the object is armed.  This is usually used on buttons
		''' that have been pressed but not yet released, and the mouse pointer
		''' is still over the button. </summary>
		''' <seealso cref= AccessibleRole#PUSH_BUTTON </seealso>
		Public Shared ReadOnly ARMED As New AccessibleState("armed")

		''' <summary>
		''' Indicates the current object is busy.  This is usually used on objects
		''' such as progress bars, sliders, or scroll bars to indicate they are
		''' in a state of transition. </summary>
		''' <seealso cref= AccessibleRole#PROGRESS_BAR </seealso>
		''' <seealso cref= AccessibleRole#SCROLL_BAR </seealso>
		''' <seealso cref= AccessibleRole#SLIDER </seealso>
		Public Shared ReadOnly BUSY As New AccessibleState("busy")

		''' <summary>
		''' Indicates this object is currently checked.  This is usually used on
		''' objects such as toggle buttons, radio buttons, and check boxes. </summary>
		''' <seealso cref= AccessibleRole#TOGGLE_BUTTON </seealso>
		''' <seealso cref= AccessibleRole#RADIO_BUTTON </seealso>
		''' <seealso cref= AccessibleRole#CHECK_BOX </seealso>
		Public Shared ReadOnly CHECKED As New AccessibleState("checked")

		''' <summary>
		''' Indicates the user can change the contents of this object.  This
		''' is usually used primarily for objects that allow the user to
		''' enter text.  Other objects, such as scroll bars and sliders,
		''' are automatically editable if they are enabled. </summary>
		''' <seealso cref= #ENABLED </seealso>
		Public Shared ReadOnly EDITABLE As New AccessibleState("editable")

		''' <summary>
		''' Indicates this object allows progressive disclosure of its children.
		''' This is usually used with hierarchical objects such as trees and
		''' is often paired with the EXPANDED or COLLAPSED states. </summary>
		''' <seealso cref= #EXPANDED </seealso>
		''' <seealso cref= #COLLAPSED </seealso>
		''' <seealso cref= AccessibleRole#TREE </seealso>
		Public Shared ReadOnly EXPANDABLE As New AccessibleState("expandable")

		''' <summary>
		''' Indicates this object is collapsed.  This is usually paired with the
		''' EXPANDABLE state and is used on objects that provide progressive
		''' disclosure such as trees. </summary>
		''' <seealso cref= #EXPANDABLE </seealso>
		''' <seealso cref= #EXPANDED </seealso>
		''' <seealso cref= AccessibleRole#TREE </seealso>
		Public Shared ReadOnly COLLAPSED As New AccessibleState("collapsed")

		''' <summary>
		''' Indicates this object is expanded.  This is usually paired with the
		''' EXPANDABLE state and is used on objects that provide progressive
		''' disclosure such as trees. </summary>
		''' <seealso cref= #EXPANDABLE </seealso>
		''' <seealso cref= #COLLAPSED </seealso>
		''' <seealso cref= AccessibleRole#TREE </seealso>
		Public Shared ReadOnly EXPANDED As New AccessibleState("expanded")

		''' <summary>
		''' Indicates this object is enabled.  The absence of this state from an
		''' object's state set indicates this object is not enabled.  An object
		''' that is not enabled cannot be manipulated by the user.  In a graphical
		''' display, it is usually grayed out.
		''' </summary>
		Public Shared ReadOnly ENABLED As New AccessibleState("enabled")

		''' <summary>
		''' Indicates this object can accept keyboard focus, which means all
		''' events resulting from typing on the keyboard will normally be
		''' passed to it when it has focus. </summary>
		''' <seealso cref= #FOCUSED </seealso>
		Public Shared ReadOnly FOCUSABLE As New AccessibleState("focusable")

		''' <summary>
		''' Indicates this object currently has the keyboard focus. </summary>
		''' <seealso cref= #FOCUSABLE </seealso>
		Public Shared ReadOnly FOCUSED As New AccessibleState("focused")

		''' <summary>
		''' Indicates this object is minimized and is represented only by an
		''' icon.  This is usually only associated with frames and internal
		''' frames. </summary>
		''' <seealso cref= AccessibleRole#FRAME </seealso>
		''' <seealso cref= AccessibleRole#INTERNAL_FRAME </seealso>
		Public Shared ReadOnly ICONIFIED As New AccessibleState("iconified")

		''' <summary>
		''' Indicates something must be done with this object before the
		''' user can interact with an object in a different window.  This
		''' is usually associated only with dialogs. </summary>
		''' <seealso cref= AccessibleRole#DIALOG </seealso>
		Public Shared ReadOnly MODAL As New AccessibleState("modal")

		''' <summary>
		''' Indicates this object paints every pixel within its
		''' rectangular region. A non-opaque component paints only some of
		''' its pixels, allowing the pixels underneath it to "show through".
		''' A component that does not fully paint its pixels therefore
		''' provides a degree of transparency. </summary>
		''' <seealso cref= Accessible#getAccessibleContext </seealso>
		''' <seealso cref= AccessibleContext#getAccessibleComponent </seealso>
		''' <seealso cref= AccessibleComponent#getBounds </seealso>
		Public Shared ReadOnly OPAQUE As New AccessibleState("opaque")

		''' <summary>
		''' Indicates the size of this object is not fixed. </summary>
		''' <seealso cref= Accessible#getAccessibleContext </seealso>
		''' <seealso cref= AccessibleContext#getAccessibleComponent </seealso>
		''' <seealso cref= AccessibleComponent#getSize </seealso>
		''' <seealso cref= AccessibleComponent#setSize </seealso>
		Public Shared ReadOnly RESIZABLE As New AccessibleState("resizable")


		''' <summary>
		''' Indicates this object allows more than one of its children to
		''' be selected at the same time. </summary>
		''' <seealso cref= Accessible#getAccessibleContext </seealso>
		''' <seealso cref= AccessibleContext#getAccessibleSelection </seealso>
		''' <seealso cref= AccessibleSelection </seealso>
		Public Shared ReadOnly MULTISELECTABLE As New AccessibleState("multiselectable")

		''' <summary>
		''' Indicates this object is the child of an object that allows its
		''' children to be selected, and that this child is one of those
		''' children that can be selected. </summary>
		''' <seealso cref= #SELECTED </seealso>
		''' <seealso cref= Accessible#getAccessibleContext </seealso>
		''' <seealso cref= AccessibleContext#getAccessibleSelection </seealso>
		''' <seealso cref= AccessibleSelection </seealso>
		Public Shared ReadOnly SELECTABLE As New AccessibleState("selectable")

		''' <summary>
		''' Indicates this object is the child of an object that allows its
		''' children to be selected, and that this child is one of those
		''' children that has been selected. </summary>
		''' <seealso cref= #SELECTABLE </seealso>
		''' <seealso cref= Accessible#getAccessibleContext </seealso>
		''' <seealso cref= AccessibleContext#getAccessibleSelection </seealso>
		''' <seealso cref= AccessibleSelection </seealso>
		Public Shared ReadOnly SELECTED As New AccessibleState("selected")

		''' <summary>
		''' Indicates this object, the object's parent, the object's parent's
		''' parent, and so on, are all visible.  Note that this does not
		''' necessarily mean the object is painted on the screen.  It might
		''' be occluded by some other showing object. </summary>
		''' <seealso cref= #VISIBLE </seealso>
		Public Shared ReadOnly SHOWING As New AccessibleState("showing")

		''' <summary>
		''' Indicates this object is visible.  Note: this means that the
		''' object intends to be visible; however, it may not in fact be
		''' showing on the screen because one of the objects that this object
		''' is contained by is not visible. </summary>
		''' <seealso cref= #SHOWING </seealso>
		Public Shared ReadOnly VISIBLE As New AccessibleState("visible")

		''' <summary>
		''' Indicates the orientation of this object is vertical.  This is
		''' usually associated with objects such as scrollbars, sliders, and
		''' progress bars. </summary>
		''' <seealso cref= #VERTICAL </seealso>
		''' <seealso cref= AccessibleRole#SCROLL_BAR </seealso>
		''' <seealso cref= AccessibleRole#SLIDER </seealso>
		''' <seealso cref= AccessibleRole#PROGRESS_BAR </seealso>
		Public Shared ReadOnly VERTICAL As New AccessibleState("vertical")

		''' <summary>
		''' Indicates the orientation of this object is horizontal.  This is
		''' usually associated with objects such as scrollbars, sliders, and
		''' progress bars. </summary>
		''' <seealso cref= #HORIZONTAL </seealso>
		''' <seealso cref= AccessibleRole#SCROLL_BAR </seealso>
		''' <seealso cref= AccessibleRole#SLIDER </seealso>
		''' <seealso cref= AccessibleRole#PROGRESS_BAR </seealso>
		Public Shared ReadOnly HORIZONTAL As New AccessibleState("horizontal")

		''' <summary>
		''' Indicates this (text) object can contain only a single line of text
		''' </summary>
		Public Shared ReadOnly SINGLE_LINE As New AccessibleState("singleline")

		''' <summary>
		''' Indicates this (text) object can contain multiple lines of text
		''' </summary>
		Public Shared ReadOnly MULTI_LINE As New AccessibleState("multiline")

		''' <summary>
		''' Indicates this object is transient.  An assistive technology should
		''' not add a PropertyChange listener to an object with transient state,
		''' as that object will never generate any events.  Transient objects
		''' are typically created to answer Java Accessibility method queries,
		''' but otherwise do not remain linked to the underlying object (for
		''' example, those objects underneath lists, tables, and trees in Swing,
		''' where only one actual UI Component does shared rendering duty for
		''' all of the data objects underneath the actual list/table/tree elements).
		''' 
		''' @since 1.5
		''' 
		''' </summary>
		Public Shared ReadOnly TRANSIENT As New AccessibleState("transient")

		''' <summary>
		''' Indicates this object is responsible for managing its
		''' subcomponents.  This is typically used for trees and tables
		''' that have a large number of subcomponents and where the
		''' objects are created only when needed and otherwise remain virtual.
		''' The application should not manage the subcomponents directly.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly MANAGES_DESCENDANTS As New AccessibleState("managesDescendants")

		''' <summary>
		''' Indicates that the object state is indeterminate.  An example
		''' is selected text that is partially bold and partially not
		''' bold. In this case the attributes associated with the selected
		''' text are indeterminate.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly INDETERMINATE As New AccessibleState("indeterminate")

		''' <summary>
		''' A state indicating that text is truncated by a bounding rectangle
		''' and that some of the text is not displayed on the screen.  An example
		''' is text in a spreadsheet cell that is truncated by the bounds of
		''' the cell.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly TRUNCATED As New AccessibleState("truncated")

		''' <summary>
		''' Creates a new AccessibleState using the given locale independent key.
		''' This should not be a public method.  Instead, it is used to create
		''' the constants in this file to make it a strongly typed enumeration.
		''' Subclasses of this class should enforce similar policy.
		''' <p>
		''' The key String should be a locale independent key for the state.
		''' It is not intended to be used as the actual String to display
		''' to the user.  To get the localized string, use toDisplayString.
		''' </summary>
		''' <param name="key"> the locale independent name of the state. </param>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Protected Friend Sub New(ByVal key As String)
			Me.key = key
		End Sub
	End Class

End Namespace
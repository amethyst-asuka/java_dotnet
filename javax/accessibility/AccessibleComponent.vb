'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' The AccessibleComponent interface should be supported by any object
	''' that is rendered on the screen.  This interface provides the standard
	''' mechanism for an assistive technology to determine and set the
	''' graphical representation of an object.  Applications can determine
	''' if an object supports the AccessibleComponent interface by first
	''' obtaining its AccessibleContext
	''' and then calling the
	''' <seealso cref="AccessibleContext#getAccessibleComponent"/> method.
	''' If the return value is not null, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleComponent
	''' 
	''' @author      Peter Korn
	''' @author      Hans Muller
	''' @author      Willie Walker </seealso>
	Public Interface AccessibleComponent

		''' <summary>
		''' Gets the background color of this object.
		''' </summary>
		''' <returns> the background color, if supported, of the object;
		''' otherwise, null </returns>
		''' <seealso cref= #setBackground </seealso>
		Property background As Color


		''' <summary>
		''' Gets the foreground color of this object.
		''' </summary>
		''' <returns> the foreground color, if supported, of the object;
		''' otherwise, null </returns>
		''' <seealso cref= #setForeground </seealso>
		Property foreground As Color


		''' <summary>
		''' Gets the Cursor of this object.
		''' </summary>
		''' <returns> the Cursor, if supported, of the object; otherwise, null </returns>
		''' <seealso cref= #setCursor </seealso>
		Property cursor As Cursor


		''' <summary>
		''' Gets the Font of this object.
		''' </summary>
		''' <returns> the Font,if supported, for the object; otherwise, null </returns>
		''' <seealso cref= #setFont </seealso>
		Property font As Font


		''' <summary>
		''' Gets the FontMetrics of this object.
		''' </summary>
		''' <param name="f"> the Font </param>
		''' <returns> the FontMetrics, if supported, the object; otherwise, null </returns>
		''' <seealso cref= #getFont </seealso>
		Function getFontMetrics(ByVal f As Font) As FontMetrics

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
		Property enabled As Boolean


		''' <summary>
		''' Determines if the object is visible.  Note: this means that the
		''' object intends to be visible; however, it may not be
		''' showing on the screen because one of the objects that this object
		''' is contained by is currently not visible.  To determine if an object is
		''' showing on the screen, use isShowing().
		''' <p>Objects that are visible will also have the
		''' AccessibleState.VISIBLE state set in their AccessibleStateSets.
		''' </summary>
		''' <returns> true if object is visible; otherwise, false </returns>
		''' <seealso cref= #setVisible </seealso>
		''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
		''' <seealso cref= AccessibleState#VISIBLE </seealso>
		''' <seealso cref= AccessibleStateSet </seealso>
		Property visible As Boolean


		''' <summary>
		''' Determines if the object is showing.  This is determined by checking
		''' the visibility of the object and its ancestors.
		''' Note: this
		''' will return true even if the object is obscured by another (for example,
		''' it is underneath a menu that was pulled down).
		''' </summary>
		''' <returns> true if object is showing; otherwise, false </returns>
		ReadOnly Property showing As Boolean

		''' <summary>
		''' Checks whether the specified point is within this object's bounds,
		''' where the point's x and y coordinates are defined to be relative to the
		''' coordinate system of the object.
		''' </summary>
		''' <param name="p"> the Point relative to the coordinate system of the object </param>
		''' <returns> true if object contains Point; otherwise false </returns>
		''' <seealso cref= #getBounds </seealso>
		Function contains(ByVal p As Point) As Boolean

		''' <summary>
		''' Returns the location of the object on the screen.
		''' </summary>
		''' <returns> the location of the object on screen; null if this object
		''' is not on the screen </returns>
		''' <seealso cref= #getBounds </seealso>
		''' <seealso cref= #getLocation </seealso>
		ReadOnly Property locationOnScreen As Point

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
		Property location As Point


		''' <summary>
		''' Gets the bounds of this object in the form of a Rectangle object.
		''' The bounds specify this object's width, height, and location
		''' relative to its parent.
		''' </summary>
		''' <returns> A rectangle indicating this component's bounds; null if
		''' this object is not on the screen. </returns>
		''' <seealso cref= #contains </seealso>
		Property bounds As Rectangle


		''' <summary>
		''' Returns the size of this object in the form of a Dimension object.
		''' The height field of the Dimension object contains this object's
		''' height, and the width field of the Dimension object contains this
		''' object's width.
		''' </summary>
		''' <returns> A Dimension object that indicates the size of this component;
		''' null if this object is not on the screen </returns>
		''' <seealso cref= #setSize </seealso>
		Property size As Dimension


		''' <summary>
		''' Returns the Accessible child, if one exists, contained at the local
		''' coordinate Point.
		''' </summary>
		''' <param name="p"> The point relative to the coordinate system of this object. </param>
		''' <returns> the Accessible, if it exists, at the specified location;
		''' otherwise null </returns>
		Function getAccessibleAt(ByVal p As Point) As Accessible

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
		ReadOnly Property focusTraversable As Boolean

		''' <summary>
		''' Requests focus for this object.  If this object cannot accept focus,
		''' nothing will happen.  Otherwise, the object will attempt to take
		''' focus. </summary>
		''' <seealso cref= #isFocusTraversable </seealso>
		Sub requestFocus()

		''' <summary>
		''' Adds the specified focus listener to receive focus events from this
		''' component.
		''' </summary>
		''' <param name="l"> the focus listener </param>
		''' <seealso cref= #removeFocusListener </seealso>
		Sub addFocusListener(ByVal l As FocusListener)

		''' <summary>
		''' Removes the specified focus listener so it no longer receives focus
		''' events from this component.
		''' </summary>
		''' <param name="l"> the focus listener </param>
		''' <seealso cref= #addFocusListener </seealso>
		Sub removeFocusListener(ByVal l As FocusListener)
	End Interface

End Namespace
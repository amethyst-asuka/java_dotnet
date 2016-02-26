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
Namespace javax.swing.border


	''' <summary>
	''' Interface describing an object capable of rendering a border
	''' around the edges of a swing component.
	''' For examples of using borders see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/border.htmll">How to Use Borders</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' <p>
	''' In the Swing component set, borders supercede Insets as the
	''' mechanism for creating a (decorated or plain) area around the
	''' edge of a component.
	''' <p>
	''' Usage Notes:
	''' <ul>
	''' <li>Use EmptyBorder to create a plain border (this mechanism
	'''     replaces its predecessor, <code>setInsets</code>).
	''' <li>Use CompoundBorder to nest multiple border objects, creating
	'''     a single, combined border.
	''' <li>Border instances are designed to be shared. Rather than creating
	'''     a new border object using one of border classes, use the
	'''     BorderFactory methods, which produces a shared instance of the
	'''     common border types.
	''' <li>Additional border styles include BevelBorder, SoftBevelBorder,
	'''     EtchedBorder, LineBorder, TitledBorder, and MatteBorder.
	''' <li>To create a new border class, subclass AbstractBorder.
	''' </ul>
	''' 
	''' @author David Kloba
	''' @author Amy Fowler </summary>
	''' <seealso cref= javax.swing.BorderFactory </seealso>
	''' <seealso cref= EmptyBorder </seealso>
	''' <seealso cref= CompoundBorder </seealso>
	Public Interface Border
		''' <summary>
		''' Paints the border for the specified component with the specified
		''' position and size. </summary>
		''' <param name="c"> the component for which this border is being painted </param>
		''' <param name="g"> the paint graphics </param>
		''' <param name="x"> the x position of the painted border </param>
		''' <param name="y"> the y position of the painted border </param>
		''' <param name="width"> the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)

		''' <summary>
		''' Returns the insets of the border. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets

		''' <summary>
		''' Returns whether or not the border is opaque.  If the border
		''' is opaque, it is responsible for filling in it's own
		''' background when painting.
		''' </summary>
		ReadOnly Property borderOpaque As Boolean
	End Interface

End Namespace
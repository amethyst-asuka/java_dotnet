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
	''' A composite Border class used to compose two Border objects
	''' into a single border by nesting an inside Border object within
	''' the insets of an outside Border object.
	''' 
	''' For example, this class may be used to add blank margin space
	''' to a component with an existing decorative border:
	''' 
	''' <pre>
	'''    Border border = comp.getBorder();
	'''    Border margin = new EmptyBorder(10,10,10,10);
	'''    comp.setBorder(new CompoundBorder(border, margin));
	''' </pre>
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
	''' @author David Kloba
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class CompoundBorder
		Inherits AbstractBorder

		Protected Friend outsideBorder As Border
		Protected Friend insideBorder As Border

		''' <summary>
		''' Creates a compound border with null outside and inside borders.
		''' </summary>
		Public Sub New()
			Me.outsideBorder = Nothing
			Me.insideBorder = Nothing
		End Sub

		''' <summary>
		''' Creates a compound border with the specified outside and
		''' inside borders.  Either border may be null. </summary>
		''' <param name="outsideBorder"> the outside border </param>
		''' <param name="insideBorder"> the inside border to be nested </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal outsideBorder As Border, ByVal insideBorder As Border)
			Me.outsideBorder = outsideBorder
			Me.insideBorder = insideBorder
		End Sub

		''' <summary>
		''' Returns whether or not the compound border is opaque.
		''' </summary>
		''' <returns> {@code true} if the inside and outside borders
		'''         are each either {@code null} or opaque;
		'''         or {@code false} otherwise </returns>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return (outsideBorder Is Nothing OrElse outsideBorder.borderOpaque) AndAlso (insideBorder Is Nothing OrElse insideBorder.borderOpaque)
			End Get
		End Property

		''' <summary>
		''' Paints the compound border by painting the outside border
		''' with the specified position and size and then painting the
		''' inside border at the specified position and size offset by
		''' the insets of the outside border. </summary>
		''' <param name="c"> the component for which this border is being painted </param>
		''' <param name="g"> the paint graphics </param>
		''' <param name="x"> the x position of the painted border </param>
		''' <param name="y"> the y position of the painted border </param>
		''' <param name="width"> the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim nextInsets As java.awt.Insets
			Dim px, py, pw, ph As Integer

			px = x
			py = y
			pw = width
			ph = height

			If outsideBorder IsNot Nothing Then
				outsideBorder.paintBorder(c, g, px, py, pw, ph)

				nextInsets = outsideBorder.getBorderInsets(c)
				px += nextInsets.left
				py += nextInsets.top
				pw = pw - nextInsets.right - nextInsets.left
				ph = ph - nextInsets.bottom - nextInsets.top
			End If
			If insideBorder IsNot Nothing Then insideBorder.paintBorder(c, g, px, py, pw, ph)

		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			Dim nextInsets As java.awt.Insets

				insets.bottom = 0
					insets.right = insets.bottom
						insets.left = insets.right
						insets.top = insets.left
			If outsideBorder IsNot Nothing Then
				nextInsets = outsideBorder.getBorderInsets(c)
				insets.top += nextInsets.top
				insets.left += nextInsets.left
				insets.right += nextInsets.right
				insets.bottom += nextInsets.bottom
			End If
			If insideBorder IsNot Nothing Then
				nextInsets = insideBorder.getBorderInsets(c)
				insets.top += nextInsets.top
				insets.left += nextInsets.left
				insets.right += nextInsets.right
				insets.bottom += nextInsets.bottom
			End If
			Return insets
		End Function

		''' <summary>
		''' Returns the outside border object.
		''' </summary>
		Public Overridable Property outsideBorder As Border
			Get
				Return outsideBorder
			End Get
		End Property

		''' <summary>
		''' Returns the inside border object.
		''' </summary>
		Public Overridable Property insideBorder As Border
			Get
				Return insideBorder
			End Get
		End Property
	End Class

End Namespace
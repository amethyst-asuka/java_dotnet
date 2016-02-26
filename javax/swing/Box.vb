Imports javax.accessibility

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
	''' A lightweight container
	''' that uses a BoxLayout object as its layout manager.
	''' Box provides several class methods
	''' that are useful for containers using BoxLayout --
	''' even non-Box containers.
	''' 
	''' <p>
	''' The <code>Box</code> class can create several kinds
	''' of invisible components
	''' that affect layout:
	''' glue, struts, and rigid areas.
	''' If all the components your <code>Box</code> contains
	''' have a fixed size,
	''' you might want to use a glue component
	''' (returned by <code>createGlue</code>)
	''' to control the components' positions.
	''' If you need a fixed amount of space between two components,
	''' try using a strut
	''' (<code>createHorizontalStrut</code> or <code>createVerticalStrut</code>).
	''' If you need an invisible component
	''' that always takes up the same amount of space,
	''' get it by invoking <code>createRigidArea</code>.
	''' <p>
	''' If you are implementing a <code>BoxLayout</code> you
	''' can find further information and examples in
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/layout/box.html">How to Use BoxLayout</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= BoxLayout
	''' 
	''' @author  Timothy Prinzing </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class Box
		Inherits JComponent
		Implements Accessible

		''' <summary>
		''' Creates a <code>Box</code> that displays its components
		''' along the the specified axis.
		''' </summary>
		''' <param name="axis">  can be <seealso cref="BoxLayout#X_AXIS"/>,
		'''              <seealso cref="BoxLayout#Y_AXIS"/>,
		'''              <seealso cref="BoxLayout#LINE_AXIS"/> or
		'''              <seealso cref="BoxLayout#PAGE_AXIS"/>. </param>
		''' <exception cref="AWTError"> if the <code>axis</code> is invalid </exception>
		''' <seealso cref= #createHorizontalBox </seealso>
		''' <seealso cref= #createVerticalBox </seealso>
		Public Sub New(ByVal axis As Integer)
			MyBase.New()
			MyBase.layout = New BoxLayout(Me, axis)
		End Sub

		''' <summary>
		''' Creates a <code>Box</code> that displays its components
		''' from left to right. If you want a <code>Box</code> that
		''' respects the component orientation you should create the
		''' <code>Box</code> using the constructor and pass in
		''' <code>BoxLayout.LINE_AXIS</code>, eg:
		''' <pre>
		'''   Box lineBox = new Box(BoxLayout.LINE_AXIS);
		''' </pre>
		''' </summary>
		''' <returns> the box </returns>
		Public Shared Function createHorizontalBox() As Box
			Return New Box(BoxLayout.X_AXIS)
		End Function

		''' <summary>
		''' Creates a <code>Box</code> that displays its components
		''' from top to bottom. If you want a <code>Box</code> that
		''' respects the component orientation you should create the
		''' <code>Box</code> using the constructor and pass in
		''' <code>BoxLayout.PAGE_AXIS</code>, eg:
		''' <pre>
		'''   Box lineBox = new Box(BoxLayout.PAGE_AXIS);
		''' </pre>
		''' </summary>
		''' <returns> the box </returns>
		Public Shared Function createVerticalBox() As Box
			Return New Box(BoxLayout.Y_AXIS)
		End Function

		''' <summary>
		''' Creates an invisible component that's always the specified size.
		''' <!-- WHEN WOULD YOU USE THIS AS OPPOSED TO A STRUT? -->
		''' </summary>
		''' <param name="d"> the dimensions of the invisible component </param>
		''' <returns> the component </returns>
		''' <seealso cref= #createGlue </seealso>
		''' <seealso cref= #createHorizontalStrut </seealso>
		''' <seealso cref= #createVerticalStrut </seealso>
		Public Shared Function createRigidArea(ByVal d As Dimension) As Component
			Return New Filler(d, d, d)
		End Function

		''' <summary>
		''' Creates an invisible, fixed-width component.
		''' In a horizontal box,
		''' you typically use this method
		''' to force a certain amount of space between two components.
		''' In a vertical box,
		''' you might use this method
		''' to force the box to be at least the specified width.
		''' The invisible component has no height
		''' unless excess space is available,
		''' in which case it takes its share of available space,
		''' just like any other component that has no maximum height.
		''' </summary>
		''' <param name="width"> the width of the invisible component, in pixels &gt;= 0 </param>
		''' <returns> the component </returns>
		''' <seealso cref= #createVerticalStrut </seealso>
		''' <seealso cref= #createGlue </seealso>
		''' <seealso cref= #createRigidArea </seealso>
		Public Shared Function createHorizontalStrut(ByVal width As Integer) As Component
			Return New Filler(New Dimension(width,0), New Dimension(width,0), New Dimension(width, Short.MaxValue))
		End Function

		''' <summary>
		''' Creates an invisible, fixed-height component.
		''' In a vertical box,
		''' you typically use this method
		''' to force a certain amount of space between two components.
		''' In a horizontal box,
		''' you might use this method
		''' to force the box to be at least the specified height.
		''' The invisible component has no width
		''' unless excess space is available,
		''' in which case it takes its share of available space,
		''' just like any other component that has no maximum width.
		''' </summary>
		''' <param name="height"> the height of the invisible component, in pixels &gt;= 0 </param>
		''' <returns> the component </returns>
		''' <seealso cref= #createHorizontalStrut </seealso>
		''' <seealso cref= #createGlue </seealso>
		''' <seealso cref= #createRigidArea </seealso>
		Public Shared Function createVerticalStrut(ByVal height As Integer) As Component
			Return New Filler(New Dimension(0,height), New Dimension(0,height), New Dimension(Short.MaxValue, height))
		End Function

		''' <summary>
		''' Creates an invisible "glue" component
		''' that can be useful in a Box
		''' whose visible components have a maximum width
		''' (for a horizontal box)
		''' or height (for a vertical box).
		''' You can think of the glue component
		''' as being a gooey substance
		''' that expands as much as necessary
		''' to fill the space between its neighboring components.
		''' 
		''' <p>
		''' 
		''' For example, suppose you have
		''' a horizontal box that contains two fixed-size components.
		''' If the box gets extra space,
		''' the fixed-size components won't become larger,
		''' so where does the extra space go?
		''' Without glue,
		''' the extra space goes to the right of the second component.
		''' If you put glue between the fixed-size components,
		''' then the extra space goes there.
		''' If you put glue before the first fixed-size component,
		''' the extra space goes there,
		''' and the fixed-size components are shoved against the right
		''' edge of the box.
		''' If you put glue before the first fixed-size component
		''' and after the second fixed-size component,
		''' the fixed-size components are centered in the box.
		''' 
		''' <p>
		''' 
		''' To use glue,
		''' call <code>Box.createGlue</code>
		''' and add the returned component to a container.
		''' The glue component has no minimum or preferred size,
		''' so it takes no space unless excess space is available.
		''' If excess space is available,
		''' then the glue component takes its share of available
		''' horizontal or vertical space,
		''' just like any other component that has no maximum width or height.
		''' </summary>
		''' <returns> the component </returns>
		Public Shared Function createGlue() As Component
			Return New Filler(New Dimension(0,0), New Dimension(0,0), New Dimension(Short.MaxValue, Short.MaxValue))
		End Function

		''' <summary>
		''' Creates a horizontal glue component.
		''' </summary>
		''' <returns> the component </returns>
		Public Shared Function createHorizontalGlue() As Component
			Return New Filler(New Dimension(0,0), New Dimension(0,0), New Dimension(Short.MaxValue, 0))
		End Function

		''' <summary>
		''' Creates a vertical glue component.
		''' </summary>
		''' <returns> the component </returns>
		Public Shared Function createVerticalGlue() As Component
			Return New Filler(New Dimension(0,0), New Dimension(0,0), New Dimension(0, Short.MaxValue))
		End Function

		''' <summary>
		''' Throws an AWTError, since a Box can use only a BoxLayout.
		''' </summary>
		''' <param name="l"> the layout manager to use </param>
		Public Overridable Property layout As LayoutManager
			Set(ByVal l As LayoutManager)
				Throw New AWTError("Illegal request")
			End Set
		End Property

		''' <summary>
		''' Paints this <code>Box</code>.  If this <code>Box</code> has a UI this
		''' method invokes super's implementation, otherwise if this
		''' <code>Box</code> is opaque the <code>Graphics</code> is filled
		''' using the background.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> to paint to </param>
		''' <exception cref="NullPointerException"> if <code>g</code> is null
		''' @since 1.6 </exception>
		Protected Friend Overrides Sub paintComponent(ByVal g As Graphics)
			If ui IsNot Nothing Then
				' On the off chance some one created a UI, honor it
				MyBase.paintComponent(g)
			ElseIf opaque Then
				g.color = background
				g.fillRect(0, 0, width, height)
			End If
		End Sub


		''' <summary>
		''' An implementation of a lightweight component that participates in
		''' layout but has no view.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Class Filler
			Inherits JComponent
			Implements Accessible
			''' <summary>
			''' Constructor to create shape with the given size ranges.
			''' </summary>
			''' <param name="min">   Minimum size </param>
			''' <param name="pref">  Preferred size </param>
			''' <param name="max">   Maximum size </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal min As Dimension, ByVal pref As Dimension, ByVal max As Dimension)
				minimumSize = min
				preferredSize = pref
				maximumSize = max
			End Sub

			''' <summary>
			''' Change the size requests for this shape.  An invalidate() is
			''' propagated upward as a result so that layout will eventually
			''' happen with using the new sizes.
			''' </summary>
			''' <param name="min">   Value to return for getMinimumSize </param>
			''' <param name="pref">  Value to return for getPreferredSize </param>
			''' <param name="max">   Value to return for getMaximumSize </param>
			Public Overridable Sub changeShape(ByVal min As Dimension, ByVal pref As Dimension, ByVal max As Dimension)
				minimumSize = min
				preferredSize = pref
				maximumSize = max
				revalidate()
			End Sub

			' ---- Component methods ------------------------------------------

			''' <summary>
			''' Paints this <code>Filler</code>.  If this
			''' <code>Filler</code> has a UI this method invokes super's
			''' implementation, otherwise if this <code>Filler</code> is
			''' opaque the <code>Graphics</code> is filled using the
			''' background.
			''' </summary>
			''' <param name="g"> the <code>Graphics</code> to paint to </param>
			''' <exception cref="NullPointerException"> if <code>g</code> is null
			''' @since 1.6 </exception>
			Protected Friend Overrides Sub paintComponent(ByVal g As Graphics)
				If ui IsNot Nothing Then
					' On the off chance some one created a UI, honor it
					MyBase.paintComponent(g)
				ElseIf opaque Then
					g.color = background
					g.fillRect(0, 0, width, height)
				End If
			End Sub

	'///////////////
	' Accessibility support for Box$Filler
	'//////////////

			''' <summary>
			''' Gets the AccessibleContext associated with this Box.Filler.
			''' For box fillers, the AccessibleContext takes the form of an
			''' AccessibleBoxFiller.
			''' A new AccessibleAWTBoxFiller instance is created if necessary.
			''' </summary>
			''' <returns> an AccessibleBoxFiller that serves as the
			'''         AccessibleContext of this Box.Filler. </returns>
			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					If accessibleContext Is Nothing Then accessibleContext = New AccessibleBoxFiller(Me)
					Return accessibleContext
				End Get
			End Property

			''' <summary>
			''' This class implements accessibility support for the
			''' <code>Box.Filler</code> class.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend Class AccessibleBoxFiller
				Inherits AccessibleAWTComponent

				Private ReadOnly outerInstance As Box.Filler

				Public Sub New(ByVal outerInstance As Box.Filler)
					Me.outerInstance = outerInstance
				End Sub

				' AccessibleContext methods
				'
				''' <summary>
				''' Gets the role of this object.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of
				'''   the object (AccessibleRole.FILLER) </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Overridable Property accessibleRole As AccessibleRole
					Get
						Return AccessibleRole.FILLER
					End Get
				End Property
			End Class
		End Class

	'///////////////
	' Accessibility support for Box
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this Box.
		''' For boxes, the AccessibleContext takes the form of an
		''' AccessibleBox.
		''' A new AccessibleAWTBox instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleBox that serves as the
		'''         AccessibleContext of this Box </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleBox(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Box</code> class.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleBox
			Inherits AccessibleAWTContainer

			Private ReadOnly outerInstance As Box

			Public Sub New(ByVal outerInstance As Box)
				Me.outerInstance = outerInstance
			End Sub

			' AccessibleContext methods
			'
			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			'''   object (AccessibleRole.FILLER) </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.FILLER
				End Get
			End Property
		End Class ' inner class AccessibleBox
	End Class

End Namespace
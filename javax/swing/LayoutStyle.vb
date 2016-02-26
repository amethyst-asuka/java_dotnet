'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>LayoutStyle</code> provides information about how to position
	''' components.  This class is primarily useful for visual tools and
	''' layout managers.  Most developers will not need to use this class.
	''' <p>
	''' You typically don't set or create a
	''' <code>LayoutStyle</code>.  Instead use the static method
	''' <code>getInstance</code> to obtain the current instance.
	''' 
	''' @since 1.6
	''' </summary>
	Public MustInherit Class LayoutStyle
		''' <summary>
		''' Sets the shared instance of <code>LayoutStyle</code>.  Specifying
		''' <code>null</code> results in using the <code>LayoutStyle</code> from
		''' the current <code>LookAndFeel</code>.
		''' </summary>
		''' <param name="style"> the <code>LayoutStyle</code>, or <code>null</code> </param>
		''' <seealso cref= #getInstance </seealso>
		Public Shared Property instance As LayoutStyle
			Set(ByVal style As LayoutStyle)
				SyncLock GetType(LayoutStyle)
					If style Is Nothing Then
						sun.awt.AppContext.appContext.remove(GetType(LayoutStyle))
					Else
						sun.awt.AppContext.appContext.put(GetType(LayoutStyle), style)
					End If
				End SyncLock
			End Set
			Get
				Dim style As LayoutStyle
				SyncLock GetType(LayoutStyle)
					style = CType(sun.awt.AppContext.appContext.get(GetType(LayoutStyle)), LayoutStyle)
				End SyncLock
				If style Is Nothing Then Return UIManager.lookAndFeel.layoutStyle
				Return style
			End Get
		End Property



		''' <summary>
		''' <code>ComponentPlacement</code> is an enumeration of the
		''' possible ways two components can be placed relative to each
		''' other.  <code>ComponentPlacement</code> is used by the
		''' <code>LayoutStyle</code> method <code>getPreferredGap</code>.  Refer to
		''' <code>LayoutStyle</code> for more information.
		''' </summary>
		''' <seealso cref= LayoutStyle#getPreferredGap(JComponent,JComponent,
		'''      ComponentPlacement,int,Container)
		''' @since 1.6 </seealso>
		Public Enum ComponentPlacement
			''' <summary>
			''' Enumeration value indicating the two components are
			''' visually related and will be placed in the same parent.
			''' For example, a <code>JLabel</code> providing a label for a
			''' <code>JTextField</code> is typically visually associated
			''' with the <code>JTextField</code>; the constant <code>RELATED</code>
			''' is used for this.
			''' </summary>
			RELATED

			''' <summary>
			''' Enumeration value indicating the two components are
			''' visually unrelated and will be placed in the same parent.
			''' For example, groupings of components are usually visually
			''' separated; the constant <code>UNRELATED</code> is used for this.
			''' </summary>
			UNRELATED

			''' <summary>
			''' Enumeration value indicating the distance to indent a component
			''' is being requested.  For example, often times the children of
			''' a label will be horizontally indented from the label.  To determine
			''' the preferred distance for such a gap use the
			''' <code>INDENT</code> type.
			''' <p>
			''' This value is typically only useful with a direction of
			''' <code>EAST</code> or <code>WEST</code>.
			''' </summary>
			INDENT
		End Enum


		''' <summary>
		''' Creates a new <code>LayoutStyle</code>.  You generally don't
		''' create a <code>LayoutStyle</code>.  Instead use the method
		''' <code>getInstance</code> to obtain the current
		''' <code>LayoutStyle</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the amount of space to use between two components.
		''' The return value indicates the distance to place
		''' <code>component2</code> relative to <code>component1</code>.
		''' For example, the following returns the amount of space to place
		''' between <code>component2</code> and <code>component1</code>
		''' when <code>component2</code> is placed vertically above
		''' <code>component1</code>:
		''' <pre>
		'''   int gap = getPreferredGap(component1, component2,
		'''                             ComponentPlacement.RELATED,
		'''                             SwingConstants.NORTH, parent);
		''' </pre>
		''' The <code>type</code> parameter indicates the relation between
		''' the two components.  If the two components will be contained in
		''' the same parent and are showing similar logically related
		''' items, use <code>RELATED</code>.  If the two components will be
		''' contained in the same parent but show logically unrelated items
		''' use <code>UNRELATED</code>.  Some look and feels may not
		''' distinguish between the <code>RELATED</code> and
		''' <code>UNRELATED</code> types.
		''' <p>
		''' The return value is not intended to take into account the
		''' current size and position of <code>component2</code> or
		''' <code>component1</code>.  The return value may take into
		''' consideration various properties of the components.  For
		''' example, the space may vary based on font size, or the preferred
		''' size of the component.
		''' </summary>
		''' <param name="component1"> the <code>JComponent</code>
		'''               <code>component2</code> is being placed relative to </param>
		''' <param name="component2"> the <code>JComponent</code> being placed </param>
		''' <param name="position"> the position <code>component2</code> is being placed
		'''        relative to <code>component1</code>; one of
		'''        <code>SwingConstants.NORTH</code>,
		'''        <code>SwingConstants.SOUTH</code>,
		'''        <code>SwingConstants.EAST</code> or
		'''        <code>SwingConstants.WEST</code> </param>
		''' <param name="type"> how the two components are being placed </param>
		''' <param name="parent"> the parent of <code>component2</code>; this may differ
		'''        from the actual parent and it may be <code>null</code> </param>
		''' <returns> the amount of space to place between the two components </returns>
		''' <exception cref="NullPointerException"> if <code>component1</code>,
		'''         <code>component2</code> or <code>type</code> is
		'''         <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>position</code> is not
		'''         one of <code>SwingConstants.NORTH</code>,
		'''         <code>SwingConstants.SOUTH</code>,
		'''         <code>SwingConstants.EAST</code> or
		'''         <code>SwingConstants.WEST</code> </exception>
		''' <seealso cref= LookAndFeel#getLayoutStyle
		''' @since 1.6 </seealso>
		Public MustOverride Function getPreferredGap(ByVal component1 As JComponent, ByVal component2 As JComponent, ByVal type As ComponentPlacement, ByVal position As Integer, ByVal parent As java.awt.Container) As Integer

		''' <summary>
		''' Returns the amount of space to place between the component and specified
		''' edge of its parent.
		''' </summary>
		''' <param name="component"> the <code>JComponent</code> being positioned </param>
		''' <param name="position"> the position <code>component</code> is being placed
		'''        relative to its parent; one of
		'''        <code>SwingConstants.NORTH</code>,
		'''        <code>SwingConstants.SOUTH</code>,
		'''        <code>SwingConstants.EAST</code> or
		'''        <code>SwingConstants.WEST</code> </param>
		''' <param name="parent"> the parent of <code>component</code>; this may differ
		'''        from the actual parent and may be <code>null</code> </param>
		''' <returns> the amount of space to place between the component and specified
		'''         edge </returns>
		''' <exception cref="IllegalArgumentException"> if <code>position</code> is not
		'''         one of <code>SwingConstants.NORTH</code>,
		'''         <code>SwingConstants.SOUTH</code>,
		'''         <code>SwingConstants.EAST</code> or
		'''         <code>SwingConstants.WEST</code> </exception>
		Public MustOverride Function getContainerGap(ByVal component As JComponent, ByVal position As Integer, ByVal parent As java.awt.Container) As Integer
	End Class

End Namespace
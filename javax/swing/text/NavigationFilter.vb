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
Namespace javax.swing.text


	''' <summary>
	''' <code>NavigationFilter</code> can be used to restrict where the cursor can
	''' be positioned. When the default cursor positioning actions attempt to
	''' reposition the cursor they will call into the
	''' <code>NavigationFilter</code>, assuming
	''' the <code>JTextComponent</code> has a non-null
	''' <code>NavigationFilter</code> set. In this manner
	''' the <code>NavigationFilter</code> can effectively restrict where the
	''' cursor can be positioned. Similarly <code>DefaultCaret</code> will call
	''' into the <code>NavigationFilter</code> when the user is changing the
	''' selection to further restrict where the cursor can be positioned.
	''' <p>
	''' Subclasses can conditionally call into supers implementation to restrict
	''' where the cursor can be placed, or call directly into the
	''' <code>FilterBypass</code>.
	''' </summary>
	''' <seealso cref= javax.swing.text.Caret </seealso>
	''' <seealso cref= javax.swing.text.DefaultCaret </seealso>
	''' <seealso cref= javax.swing.text.View
	''' 
	''' @since 1.4 </seealso>
	Public Class NavigationFilter
		''' <summary>
		''' Invoked prior to the Caret setting the dot. The default implementation
		''' calls directly into the <code>FilterBypass</code> with the passed
		''' in arguments. Subclasses may wish to conditionally
		''' call super with a different location, or invoke the necessary method
		''' on the <code>FilterBypass</code>
		''' </summary>
		''' <param name="fb"> FilterBypass that can be used to mutate caret position </param>
		''' <param name="dot"> the position &gt;= 0 </param>
		''' <param name="bias"> Bias to place the dot at </param>
		Public Overridable Sub setDot(ByVal fb As FilterBypass, ByVal dot As Integer, ByVal bias As Position.Bias)
			fb.dotDot(dot, bias)
		End Sub

		''' <summary>
		''' Invoked prior to the Caret moving the dot. The default implementation
		''' calls directly into the <code>FilterBypass</code> with the passed
		''' in arguments. Subclasses may wish to conditionally
		''' call super with a different location, or invoke the necessary
		''' methods on the <code>FilterBypass</code>.
		''' </summary>
		''' <param name="fb"> FilterBypass that can be used to mutate caret position </param>
		''' <param name="dot"> the position &gt;= 0 </param>
		''' <param name="bias"> Bias for new location </param>
		Public Overridable Sub moveDot(ByVal fb As FilterBypass, ByVal dot As Integer, ByVal bias As Position.Bias)
			fb.moveDot(dot, bias)
		End Sub

		''' <summary>
		''' Returns the next visual position to place the caret at from an
		''' existing position. The default implementation simply forwards the
		''' method to the root View. Subclasses may wish to further restrict the
		''' location based on additional criteria.
		''' </summary>
		''' <param name="text"> JTextComponent containing text </param>
		''' <param name="pos"> Position used in determining next position </param>
		''' <param name="bias"> Bias used in determining next position </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard.
		'''  This will be one of the following values:
		''' <ul>
		''' <li>SwingConstants.WEST
		''' <li>SwingConstants.EAST
		''' <li>SwingConstants.NORTH
		''' <li>SwingConstants.SOUTH
		''' </ul> </param>
		''' <param name="biasRet"> Used to return resulting Bias of next position </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position </returns>
		''' <exception cref="BadLocationException"> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code>
		'''          doesn't have one of the legal values above </exception>
		Public Overridable Function getNextVisualPositionFrom(ByVal text As JTextComponent, ByVal pos As Integer, ByVal bias As Position.Bias, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			Return text.uI.getNextVisualPositionFrom(text, pos, bias, direction, biasRet)
		End Function


		''' <summary>
		''' Used as a way to circumvent calling back into the caret to
		''' position the cursor. Caret implementations that wish to support
		''' a NavigationFilter must provide an implementation that will
		''' not callback into the NavigationFilter.
		''' @since 1.4
		''' </summary>
		Public MustInherit Class FilterBypass
			''' <summary>
			''' Returns the Caret that is changing.
			''' </summary>
			''' <returns> Caret that is changing </returns>
			Public MustOverride ReadOnly Property caret As Caret

			''' <summary>
			''' Sets the caret location, bypassing the NavigationFilter.
			''' </summary>
			''' <param name="dot"> the position &gt;= 0 </param>
			''' <param name="bias"> Bias to place the dot at </param>
			Public MustOverride Sub setDot(ByVal dot As Integer, ByVal bias As Position.Bias)

			''' <summary>
			''' Moves the caret location, bypassing the NavigationFilter.
			''' </summary>
			''' <param name="dot"> the position &gt;= 0 </param>
			''' <param name="bias"> Bias for new location </param>
			Public MustOverride Sub moveDot(ByVal dot As Integer, ByVal bias As Position.Bias)
		End Class
	End Class

End Namespace
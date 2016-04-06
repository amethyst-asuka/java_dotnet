'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt

	''' <summary>
	''' Capabilities and properties of buffers.
	''' </summary>
	''' <seealso cref= java.awt.image.BufferStrategy#getCapabilities() </seealso>
	''' <seealso cref= GraphicsConfiguration#getBufferCapabilities
	''' @author Michael Martak
	''' @since 1.4 </seealso>
	Public Class BufferCapabilities
		Implements Cloneable

		Private frontCaps As ImageCapabilities
		Private backCaps As ImageCapabilities
		Private flipContents As FlipContents

		''' <summary>
		''' Creates a new object for specifying buffering capabilities </summary>
		''' <param name="frontCaps"> the capabilities of the front buffer; cannot be
		''' <code>null</code> </param>
		''' <param name="backCaps"> the capabilities of the back and intermediate buffers;
		''' cannot be <code>null</code> </param>
		''' <param name="flipContents"> the contents of the back buffer after page-flipping,
		''' <code>null</code> if page flipping is not used (implies blitting) </param>
		''' <exception cref="IllegalArgumentException"> if frontCaps or backCaps are
		''' <code>null</code> </exception>
		Public Sub New(  frontCaps As ImageCapabilities,   backCaps As ImageCapabilities,   flipContents_Renamed As FlipContents)
			If frontCaps Is Nothing OrElse backCaps Is Nothing Then Throw New IllegalArgumentException("Image capabilities specified cannot be null")
			Me.frontCaps = frontCaps
			Me.backCaps = backCaps
			Me.flipContents = flipContents_Renamed
		End Sub

		''' <returns> the image capabilities of the front (displayed) buffer </returns>
		Public Overridable Property frontBufferCapabilities As ImageCapabilities
			Get
				Return frontCaps
			End Get
		End Property

		''' <returns> the image capabilities of all back buffers (intermediate buffers
		''' are considered back buffers) </returns>
		Public Overridable Property backBufferCapabilities As ImageCapabilities
			Get
				Return backCaps
			End Get
		End Property

		''' <returns> whether or not the buffer strategy uses page flipping; a set of
		''' buffers that uses page flipping
		''' can swap the contents internally between the front buffer and one or
		''' more back buffers by switching the video pointer (or by copying memory
		''' internally).  A non-flipping set of
		''' buffers uses blitting to copy the contents from one buffer to
		''' another; when this is the case, <code>getFlipContents</code> returns
		''' <code>null</code> </returns>
		Public Overridable Property pageFlipping As Boolean
			Get
				Return (flipContents IsNot Nothing)
			End Get
		End Property

		''' <returns> the resulting contents of the back buffer after page-flipping.
		''' This value is <code>null</code> when the <code>isPageFlipping</code>
		''' returns <code>false</code>, implying blitting.  It can be one of
		''' <code>FlipContents.UNDEFINED</code>
		''' (the assumed default), <code>FlipContents.BACKGROUND</code>,
		''' <code>FlipContents.PRIOR</code>, or
		''' <code>FlipContents.COPIED</code>. </returns>
		''' <seealso cref= #isPageFlipping </seealso>
		''' <seealso cref= FlipContents#UNDEFINED </seealso>
		''' <seealso cref= FlipContents#BACKGROUND </seealso>
		''' <seealso cref= FlipContents#PRIOR </seealso>
		''' <seealso cref= FlipContents#COPIED </seealso>
		Public Overridable Property flipContents As FlipContents
			Get
				Return flipContents
			End Get
		End Property

		''' <returns> whether page flipping is only available in full-screen mode.  If this
		''' is <code>true</code>, full-screen exclusive mode is required for
		''' page-flipping. </returns>
		''' <seealso cref= #isPageFlipping </seealso>
		''' <seealso cref= GraphicsDevice#setFullScreenWindow </seealso>
		Public Overridable Property fullScreenRequired As Boolean
			Get
				Return False
			End Get
		End Property

		''' <returns> whether or not
		''' page flipping can be performed using more than two buffers (one or more
		''' intermediate buffers as well as the front and back buffer). </returns>
		''' <seealso cref= #isPageFlipping </seealso>
		Public Overridable Property multiBufferAvailable As Boolean
			Get
				Return False
			End Get
		End Property

		''' <returns> a copy of this BufferCapabilities object. </returns>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Since we implement Cloneable, this should never happen
				Throw New InternalError(e)
			End Try
		End Function

		' Inner class FlipContents
		''' <summary>
		''' A type-safe enumeration of the possible back buffer contents after
		''' page-flipping
		''' @since 1.4
		''' </summary>
		Public NotInheritable Class FlipContents
			Inherits AttributeValue

			Private Shared I_UNDEFINED As Integer = 0
			Private Shared I_BACKGROUND As Integer = 1
			Private Shared I_PRIOR As Integer = 2
			Private Shared I_COPIED As Integer = 3

			Private Shared ReadOnly NAMES As String() = { "undefined", "background", "prior", "copied" }

			''' <summary>
			''' When flip contents are <code>UNDEFINED</code>, the
			''' contents of the back buffer are undefined after flipping. </summary>
			''' <seealso cref= #isPageFlipping </seealso>
			''' <seealso cref= #getFlipContents </seealso>
			''' <seealso cref= #BACKGROUND </seealso>
			''' <seealso cref= #PRIOR </seealso>
			''' <seealso cref= #COPIED </seealso>
			Public Shared ReadOnly UNDEFINED As New FlipContents(I_UNDEFINED)

			''' <summary>
			''' When flip contents are <code>BACKGROUND</code>, the
			''' contents of the back buffer are cleared with the background color after
			''' flipping. </summary>
			''' <seealso cref= #isPageFlipping </seealso>
			''' <seealso cref= #getFlipContents </seealso>
			''' <seealso cref= #UNDEFINED </seealso>
			''' <seealso cref= #PRIOR </seealso>
			''' <seealso cref= #COPIED </seealso>
			Public Shared ReadOnly BACKGROUND As New FlipContents(I_BACKGROUND)

			''' <summary>
			''' When flip contents are <code>PRIOR</code>, the
			''' contents of the back buffer are the prior contents of the front buffer
			''' (a true page flip). </summary>
			''' <seealso cref= #isPageFlipping </seealso>
			''' <seealso cref= #getFlipContents </seealso>
			''' <seealso cref= #UNDEFINED </seealso>
			''' <seealso cref= #BACKGROUND </seealso>
			''' <seealso cref= #COPIED </seealso>
			Public Shared ReadOnly PRIOR As New FlipContents(I_PRIOR)

			''' <summary>
			''' When flip contents are <code>COPIED</code>, the
			''' contents of the back buffer are copied to the front buffer when
			''' flipping. </summary>
			''' <seealso cref= #isPageFlipping </seealso>
			''' <seealso cref= #getFlipContents </seealso>
			''' <seealso cref= #UNDEFINED </seealso>
			''' <seealso cref= #BACKGROUND </seealso>
			''' <seealso cref= #PRIOR </seealso>
			Public Shared ReadOnly COPIED As New FlipContents(I_COPIED)

			Private Sub New(  type As Integer)
				MyBase.New(type, NAMES)
			End Sub

		End Class ' Inner class FlipContents

	End Class

End Namespace
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

Namespace java.awt


	''' <summary>
	''' The <code>DisplayMode</code> class encapsulates the bit depth, height,
	''' width, and refresh rate of a <code>GraphicsDevice</code>. The ability to
	''' change graphics device's display mode is platform- and
	''' configuration-dependent and may not always be available
	''' (see <seealso cref="GraphicsDevice#isDisplayChangeSupported"/>).
	''' <p>
	''' For more information on full-screen exclusive mode API, see the
	''' <a href="https://docs.oracle.com/javase/tutorial/extra/fullscreen/index.html">
	''' Full-Screen Exclusive Mode API Tutorial</a>.
	''' </summary>
	''' <seealso cref= GraphicsDevice </seealso>
	''' <seealso cref= GraphicsDevice#isDisplayChangeSupported </seealso>
	''' <seealso cref= GraphicsDevice#getDisplayModes </seealso>
	''' <seealso cref= GraphicsDevice#setDisplayMode
	''' @author Michael Martak
	''' @since 1.4 </seealso>

	Public NotInheritable Class DisplayMode

		Private size As Dimension
		Private bitDepth As Integer
		Private refreshRate As Integer

		''' <summary>
		''' Create a new display mode object with the supplied parameters. </summary>
		''' <param name="width"> the width of the display, in pixels </param>
		''' <param name="height"> the height of the display, in pixels </param>
		''' <param name="bitDepth"> the bit depth of the display, in bits per
		'''        pixel.  This can be <code>BIT_DEPTH_MULTI</code> if multiple
		'''        bit depths are available. </param>
		''' <param name="refreshRate"> the refresh rate of the display, in hertz.
		'''        This can be <code>REFRESH_RATE_UNKNOWN</code> if the
		'''        information is not available. </param>
		''' <seealso cref= #BIT_DEPTH_MULTI </seealso>
		''' <seealso cref= #REFRESH_RATE_UNKNOWN </seealso>
		Public Sub New(  width As Integer,   height As Integer,   bitDepth As Integer,   refreshRate As Integer)
			Me.size = New Dimension(width, height)
			Me.bitDepth = bitDepth
			Me.refreshRate = refreshRate
		End Sub

		''' <summary>
		''' Returns the height of the display, in pixels. </summary>
		''' <returns> the height of the display, in pixels </returns>
		Public Property height As Integer
			Get
				Return size.height
			End Get
		End Property

		''' <summary>
		''' Returns the width of the display, in pixels. </summary>
		''' <returns> the width of the display, in pixels </returns>
		Public Property width As Integer
			Get
				Return size.width
			End Get
		End Property

		''' <summary>
		''' Value of the bit depth if multiple bit depths are supported in this
		''' display mode. </summary>
		''' <seealso cref= #getBitDepth </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BIT_DEPTH_MULTI As Integer = -1

		''' <summary>
		''' Returns the bit depth of the display, in bits per pixel.  This may be
		''' <code>BIT_DEPTH_MULTI</code> if multiple bit depths are supported in
		''' this display mode.
		''' </summary>
		''' <returns> the bit depth of the display, in bits per pixel. </returns>
		''' <seealso cref= #BIT_DEPTH_MULTI </seealso>
		Public Property bitDepth As Integer
			Get
				Return bitDepth
			End Get
		End Property

		''' <summary>
		''' Value of the refresh rate if not known. </summary>
		''' <seealso cref= #getRefreshRate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const REFRESH_RATE_UNKNOWN As Integer = 0

		''' <summary>
		''' Returns the refresh rate of the display, in hertz.  This may be
		''' <code>REFRESH_RATE_UNKNOWN</code> if the information is not available.
		''' </summary>
		''' <returns> the refresh rate of the display, in hertz. </returns>
		''' <seealso cref= #REFRESH_RATE_UNKNOWN </seealso>
		Public Property refreshRate As Integer
			Get
				Return refreshRate
			End Get
		End Property

		''' <summary>
		''' Returns whether the two display modes are equal. </summary>
		''' <returns> whether the two display modes are equal </returns>
		Public Overrides Function Equals(  dm As DisplayMode) As Boolean
			If dm Is Nothing Then Return False
			Return (height = dm.height AndAlso width = dm.width AndAlso bitDepth = dm.bitDepth AndAlso refreshRate = dm.refreshRate)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function Equals(  dm As Object) As Boolean
			If TypeOf dm Is DisplayMode Then
				Return Equals(CType(dm, DisplayMode))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return width + height + bitDepth * 7 + refreshRate * 13
		End Function

	End Class

End Namespace
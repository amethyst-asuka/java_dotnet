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
	''' Capabilities and properties of images.
	''' @author Michael Martak
	''' @since 1.4
	''' </summary>
	Public Class ImageCapabilities
		Implements Cloneable

		Private accelerated As Boolean = False

		''' <summary>
		''' Creates a new object for specifying image capabilities. </summary>
		''' <param name="accelerated"> whether or not an accelerated image is desired </param>
		Public Sub New(ByVal accelerated As Boolean)
			Me.accelerated = accelerated
		End Sub

		''' <summary>
		''' Returns <code>true</code> if the object whose capabilities are
		''' encapsulated in this <code>ImageCapabilities</code> can be or is
		''' accelerated. </summary>
		''' <returns> whether or not an image can be, or is, accelerated.  There are
		''' various platform-specific ways to accelerate an image, including
		''' pixmaps, VRAM, AGP.  This is the general acceleration method (as
		''' opposed to residing in system memory). </returns>
		Public Overridable Property accelerated As Boolean
			Get
				Return accelerated
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the <code>VolatileImage</code>
		''' described by this <code>ImageCapabilities</code> can lose
		''' its surfaces. </summary>
		''' <returns> whether or not a volatile image is subject to losing its surfaces
		''' at the whim of the operating system. </returns>
		Public Overridable Property trueVolatile As Boolean
			Get
				Return False
			End Get
		End Property

		''' <returns> a copy of this ImageCapabilities object. </returns>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Since we implement Cloneable, this should never happen
				Throw New InternalError(e)
			End Try
		End Function

	End Class

End Namespace
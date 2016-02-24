Imports System

'
' * Copyright (c) 2006, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' This is the superclass for Paints which use a multiple color
	''' gradient to fill in their raster.  It provides storage for variables and
	''' enumerated values common to
	''' {@code LinearGradientPaint} and {@code RadialGradientPaint}.
	''' 
	''' @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	''' @since 1.6
	''' </summary>
	Public MustInherit Class MultipleGradientPaint
		Implements Paint

			Public MustOverride Function createContext(ByVal cm As java.awt.image.ColorModel, ByVal deviceBounds As Rectangle, ByVal userBounds As java.awt.geom.Rectangle2D, ByVal xform As java.awt.geom.AffineTransform, ByVal hints As RenderingHints) As PaintContext Implements Paint.createContext

		''' <summary>
		''' The method to use when painting outside the gradient bounds.
		''' @since 1.6
		''' </summary>
		Public Enum CycleMethod
			''' <summary>
			''' Use the terminal colors to fill the remaining area.
			''' </summary>
			NO_CYCLE

			''' <summary>
			''' Cycle the gradient colors start-to-end, end-to-start
			''' to fill the remaining area.
			''' </summary>
			REFLECT

			''' <summary>
			''' Cycle the gradient colors start-to-end, start-to-end
			''' to fill the remaining area.
			''' </summary>
			REPEAT
		End Enum

		''' <summary>
		''' The color space in which to perform the gradient interpolation.
		''' @since 1.6
		''' </summary>
		Public Enum ColorSpaceType
			''' <summary>
			''' Indicates that the color interpolation should occur in sRGB space.
			''' </summary>
			SRGB

			''' <summary>
			''' Indicates that the color interpolation should occur in linearized
			''' RGB space.
			''' </summary>
			LINEAR_RGB
		End Enum

		''' <summary>
		''' The transparency of this paint object. </summary>
		Friend ReadOnly transparency As Integer

		''' <summary>
		''' Gradient keyframe values in the range 0 to 1. </summary>
		Friend ReadOnly fractions As Single()

		''' <summary>
		''' Gradient colors. </summary>
		Friend ReadOnly colors As Color()

		''' <summary>
		''' Transform to apply to gradient. </summary>
		Friend ReadOnly gradientTransform As java.awt.geom.AffineTransform

		''' <summary>
		''' The method to use when painting outside the gradient bounds. </summary>
		Friend ReadOnly cycleMethod As CycleMethod

		''' <summary>
		''' The color space in which to perform the gradient interpolation. </summary>
		Friend ReadOnly colorSpace As ColorSpaceType

		''' <summary>
		''' The following fields are used only by MultipleGradientPaintContext
		''' to cache certain values that remain constant and do not need to be
		''' recalculated for each context created from this paint instance.
		''' </summary>
		Friend model As java.awt.image.ColorModel
		Friend normalizedIntervals As Single()
		Friend isSimpleLookup As Boolean
		Friend gradients As SoftReference(Of Integer()())
		Friend gradient As SoftReference(Of Integer())
		Friend fastGradientArraySize As Integer

		''' <summary>
		''' Package-private constructor.
		''' </summary>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors corresponding to each fractional value </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT} </param>
		''' <param name="colorSpace"> which color space to use for interpolation,
		'''                   either {@code SRGB} or {@code LINEAR_RGB} </param>
		''' <param name="gradientTransform"> transform to apply to the gradient
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code gradientTransform} is null,
		''' or {@code cycleMethod} is null,
		''' or {@code colorSpace} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Friend Sub New(ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod, ByVal colorSpace As ColorSpaceType, ByVal gradientTransform As java.awt.geom.AffineTransform)
			If fractions Is Nothing Then Throw New NullPointerException("Fractions array cannot be null")

			If colors Is Nothing Then Throw New NullPointerException("Colors array cannot be null")

			If cycleMethod Is Nothing Then Throw New NullPointerException("Cycle method cannot be null")

			If colorSpace Is Nothing Then Throw New NullPointerException("Color space cannot be null")

			If gradientTransform Is Nothing Then Throw New NullPointerException("Gradient transform cannot be " & "null")

			If fractions.Length <> colors.Length Then Throw New IllegalArgumentException("Colors and fractions must " & "have equal size")

			If colors.Length < 2 Then Throw New IllegalArgumentException("User must specify at least " & "2 colors")

			' check that values are in the proper range and progress
			' in increasing order from 0 to 1
			Dim previousFraction As Single = -1.0f
			For Each currentFraction As Single In fractions
				If currentFraction < 0f OrElse currentFraction > 1f Then Throw New IllegalArgumentException("Fraction values must " & "be in the range 0 to 1: " & currentFraction)

				If currentFraction <= previousFraction Then Throw New IllegalArgumentException("Keyframe fractions " & "must be increasing: " & currentFraction)

				previousFraction = currentFraction
			Next currentFraction

			' We have to deal with the cases where the first gradient stop is not
			' equal to 0 and/or the last gradient stop is not equal to 1.
			' In both cases, create a new point and replicate the previous
			' extreme point's color.
			Dim fixFirst As Boolean = False
			Dim fixLast As Boolean = False
			Dim len As Integer = fractions.Length
			Dim [off] As Integer = 0

			If fractions(0) <> 0f Then
				' first stop is not equal to zero, fix this condition
				fixFirst = True
				len += 1
				[off] += 1
			End If
			If fractions(fractions.Length-1) <> 1f Then
				' last stop is not equal to one, fix this condition
				fixLast = True
				len += 1
			End If

			Me.fractions = New Single(len - 1){}
			Array.Copy(fractions, 0, Me.fractions, [off], fractions.Length)
			Me.colors = New Color(len - 1){}
			Array.Copy(colors, 0, Me.colors, [off], colors.Length)

			If fixFirst Then
				Me.fractions(0) = 0f
				Me.colors(0) = colors(0)
			End If
			If fixLast Then
				Me.fractions(len-1) = 1f
				Me.colors(len-1) = colors(colors.Length - 1)
			End If

			' copy some flags
			Me.colorSpace = colorSpace
			Me.cycleMethod = cycleMethod

			' copy the gradient transform
			Me.gradientTransform = New java.awt.geom.AffineTransform(gradientTransform)

			' determine transparency
			Dim opaque As Boolean = True
			For i As Integer = 0 To colors.Length - 1
				opaque = opaque AndAlso (colors(i).alpha = &Hff)
			Next i
			Me.transparency = If(opaque, OPAQUE, TRANSLUCENT)
		End Sub

		''' <summary>
		''' Returns a copy of the array of floats used by this gradient
		''' to calculate color distribution.
		''' The returned array always has 0 as its first value and 1 as its
		''' last value, with increasing values in between.
		''' </summary>
		''' <returns> a copy of the array of floats used by this gradient to
		''' calculate color distribution </returns>
		Public Property fractions As Single()
			Get
				Return java.util.Arrays.copyOf(fractions, fractions.Length)
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the array of colors used by this gradient.
		''' The first color maps to the first value in the fractions array,
		''' and the last color maps to the last value in the fractions array.
		''' </summary>
		''' <returns> a copy of the array of colors used by this gradient </returns>
		Public Property colors As Color()
			Get
				Return java.util.Arrays.copyOf(colors, colors.Length)
			End Get
		End Property

		''' <summary>
		''' Returns the enumerated type which specifies cycling behavior.
		''' </summary>
		''' <returns> the enumerated type which specifies cycling behavior </returns>
		Public Property cycleMethod As CycleMethod
			Get
				Return cycleMethod
			End Get
		End Property

		''' <summary>
		''' Returns the enumerated type which specifies color space for
		''' interpolation.
		''' </summary>
		''' <returns> the enumerated type which specifies color space for
		''' interpolation </returns>
		Public Property colorSpace As ColorSpaceType
			Get
				Return colorSpace
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the transform applied to the gradient.
		''' 
		''' <p>
		''' Note that if no transform is applied to the gradient
		''' when it is created, the identity transform is used.
		''' </summary>
		''' <returns> a copy of the transform applied to the gradient </returns>
		Public Property transform As java.awt.geom.AffineTransform
			Get
				Return New java.awt.geom.AffineTransform(gradientTransform)
			End Get
		End Property

		''' <summary>
		''' Returns the transparency mode for this {@code Paint} object.
		''' </summary>
		''' <returns> {@code OPAQUE} if all colors used by this
		'''         {@code Paint} object are opaque,
		'''         {@code TRANSLUCENT} if at least one of the
		'''         colors used by this {@code Paint} object is not opaque. </returns>
		''' <seealso cref= java.awt.Transparency </seealso>
		Public Property transparency As Integer Implements Transparency.getTransparency
			Get
				Return transparency
			End Get
		End Property
	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.geom


	''' <summary>
	''' The <code>AffineTransform</code> class represents a 2D affine transform
	''' that performs a linear mapping from 2D coordinates to other 2D
	''' coordinates that preserves the "straightness" and
	''' "parallelness" of lines.  Affine transformations can be constructed
	''' using sequences of translations, scales, flips, rotations, and shears.
	''' <p>
	''' Such a coordinate transformation can be represented by a 3 row by
	''' 3 column matrix with an implied last row of [ 0 0 1 ].  This matrix
	''' transforms source coordinates {@code (x,y)} into
	''' destination coordinates {@code (x',y')} by considering
	''' them to be a column vector and multiplying the coordinate vector
	''' by the matrix according to the following process:
	''' <pre>
	'''      [ x']   [  m00  m01  m02  ] [ x ]   [ m00x + m01y + m02 ]
	'''      [ y'] = [  m10  m11  m12  ] [ y ] = [ m10x + m11y + m12 ]
	'''      [ 1 ]   [   0    0    1   ] [ 1 ]   [         1         ]
	''' </pre>
	''' <h3><a name="quadrantapproximation">Handling 90-Degree Rotations</a></h3>
	''' <p>
	''' In some variations of the <code>rotate</code> methods in the
	''' <code>AffineTransform</code> [Class], a double-precision argument
	''' specifies the angle of rotation in radians.
	''' These methods have special handling for rotations of approximately
	''' 90 degrees (including multiples such as 180, 270, and 360 degrees),
	''' so that the common case of quadrant rotation is handled more
	''' efficiently.
	''' This special handling can cause angles very close to multiples of
	''' 90 degrees to be treated as if they were exact multiples of
	''' 90 degrees.
	''' For small multiples of 90 degrees the range of angles treated
	''' as a quadrant rotation is approximately 0.00000121 degrees wide.
	''' This section explains why such special care is needed and how
	''' it is implemented.
	''' <p>
	''' Since 90 degrees is represented as <code>PI/2</code> in radians,
	''' and since PI is a transcendental (and therefore irrational) number,
	''' it is not possible to exactly represent a multiple of 90 degrees as
	''' an exact double precision value measured in radians.
	''' As a result it is theoretically impossible to describe quadrant
	''' rotations (90, 180, 270 or 360 degrees) using these values.
	''' Double precision floating point values can get very close to
	''' non-zero multiples of <code>PI/2</code> but never close enough
	''' for the sine or cosine to be exactly 0.0, 1.0 or -1.0.
	''' The implementations of <code>Math.sin()</code> and
	''' <code>Math.cos()</code> correspondingly never return 0.0
	''' for any case other than <code>Math.sin(0.0)</code>.
	''' These same implementations do, however, return exactly 1.0 and
	''' -1.0 for some range of numbers around each multiple of 90
	''' degrees since the correct answer is so close to 1.0 or -1.0 that
	''' the double precision significand cannot represent the difference
	''' as accurately as it can for numbers that are near 0.0.
	''' <p>
	''' The net result of these issues is that if the
	''' <code>Math.sin()</code> and <code>Math.cos()</code> methods
	''' are used to directly generate the values for the matrix modifications
	''' during these radian-based rotation operations then the resulting
	''' transform is never strictly classifiable as a quadrant rotation
	''' even for a simple case like <code>rotate (System.Math.PI/2.0)</code>,
	''' due to minor variations in the matrix caused by the non-0.0 values
	''' obtained for the sine and cosine.
	''' If these transforms are not classified as quadrant rotations then
	''' subsequent code which attempts to optimize further operations based
	''' upon the type of the transform will be relegated to its most general
	''' implementation.
	''' <p>
	''' Because quadrant rotations are fairly common,
	''' this class should handle these cases reasonably quickly, both in
	''' applying the rotations to the transform and in applying the resulting
	''' transform to the coordinates.
	''' To facilitate this optimal handling, the methods which take an angle
	''' of rotation measured in radians attempt to detect angles that are
	''' intended to be quadrant rotations and treat them as such.
	''' These methods therefore treat an angle <em>theta</em> as a quadrant
	''' rotation if either <code>Math.sin(<em>theta</em>)</code> or
	''' <code>Math.cos(<em>theta</em>)</code> returns exactly 1.0 or -1.0.
	''' As a rule of thumb, this property holds true for a range of
	''' approximately 0.0000000211 radians (or 0.00000121 degrees) around
	''' small multiples of <code>Math.PI/2.0</code>.
	''' 
	''' @author Jim Graham
	''' @since 1.2
	''' </summary>
	<Serializable> _
	Public Class AffineTransform
		Implements Cloneable

	'    
	'     * This constant is only useful for the cached type field.
	'     * It indicates that the type has been decached and must be recalculated.
	'     
		Private Const TYPE_UNKNOWN As Integer = -1

		''' <summary>
		''' This constant indicates that the transform defined by this object
		''' is an identity transform.
		''' An identity transform is one in which the output coordinates are
		''' always the same as the input coordinates.
		''' If this transform is anything other than the identity transform,
		''' the type will either be the constant GENERAL_TRANSFORM or a
		''' combination of the appropriate flag bits for the various coordinate
		''' conversions that this transform performs. </summary>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_IDENTITY As Integer = 0

		''' <summary>
		''' This flag bit indicates that the transform defined by this object
		''' performs a translation in addition to the conversions indicated
		''' by other flag bits.
		''' A translation moves the coordinates by a constant amount in x
		''' and y without changing the length or angle of vectors. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_TRANSLATION As Integer = 1

		''' <summary>
		''' This flag bit indicates that the transform defined by this object
		''' performs a uniform scale in addition to the conversions indicated
		''' by other flag bits.
		''' A uniform scale multiplies the length of vectors by the same amount
		''' in both the x and y directions without changing the angle between
		''' vectors.
		''' This flag bit is mutually exclusive with the TYPE_GENERAL_SCALE flag. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_UNIFORM_SCALE As Integer = 2

		''' <summary>
		''' This flag bit indicates that the transform defined by this object
		''' performs a general scale in addition to the conversions indicated
		''' by other flag bits.
		''' A general scale multiplies the length of vectors by different
		''' amounts in the x and y directions without changing the angle
		''' between perpendicular vectors.
		''' This flag bit is mutually exclusive with the TYPE_UNIFORM_SCALE flag. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_GENERAL_SCALE As Integer = 4

		''' <summary>
		''' This constant is a bit mask for any of the scale flag bits. </summary>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE
		''' @since 1.2 </seealso>
		Public Shared ReadOnly TYPE_MASK_SCALE As Integer = (TYPE_UNIFORM_SCALE Or TYPE_GENERAL_SCALE)

		''' <summary>
		''' This flag bit indicates that the transform defined by this object
		''' performs a mirror image flip about some axis which changes the
		''' normally right handed coordinate system into a left handed
		''' system in addition to the conversions indicated by other flag bits.
		''' A right handed coordinate system is one where the positive X
		''' axis rotates counterclockwise to overlay the positive Y axis
		''' similar to the direction that the fingers on your right hand
		''' curl when you stare end on at your thumb.
		''' A left handed coordinate system is one where the positive X
		''' axis rotates clockwise to overlay the positive Y axis similar
		''' to the direction that the fingers on your left hand curl.
		''' There is no mathematical way to determine the angle of the
		''' original flipping or mirroring transformation since all angles
		''' of flip are identical given an appropriate adjusting rotation. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_FLIP As Integer = 64
	'     NOTE: TYPE_FLIP was added after GENERAL_TRANSFORM was in public
	'     * circulation and the flag bits could no longer be conveniently
	'     * renumbered without introducing binary incompatibility in outside
	'     * code.
	'     

		''' <summary>
		''' This flag bit indicates that the transform defined by this object
		''' performs a quadrant rotation by some multiple of 90 degrees in
		''' addition to the conversions indicated by other flag bits.
		''' A rotation changes the angles of vectors by the same amount
		''' regardless of the original direction of the vector and without
		''' changing the length of the vector.
		''' This flag bit is mutually exclusive with the TYPE_GENERAL_ROTATION flag. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_QUADRANT_ROTATION As Integer = 8

		''' <summary>
		''' This flag bit indicates that the transform defined by this object
		''' performs a rotation by an arbitrary angle in addition to the
		''' conversions indicated by other flag bits.
		''' A rotation changes the angles of vectors by the same amount
		''' regardless of the original direction of the vector and without
		''' changing the length of the vector.
		''' This flag bit is mutually exclusive with the
		''' TYPE_QUADRANT_ROTATION flag. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_GENERAL_ROTATION As Integer = 16

		''' <summary>
		''' This constant is a bit mask for any of the rotation flag bits. </summary>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION
		''' @since 1.2 </seealso>
		Public Shared ReadOnly TYPE_MASK_ROTATION As Integer = (TYPE_QUADRANT_ROTATION Or TYPE_GENERAL_ROTATION)

		''' <summary>
		''' This constant indicates that the transform defined by this object
		''' performs an arbitrary conversion of the input coordinates.
		''' If this transform can be classified by any of the above constants,
		''' the type will either be the constant TYPE_IDENTITY or a
		''' combination of the appropriate flag bits for the various coordinate
		''' conversions that this transform performs. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #getType
		''' @since 1.2 </seealso>
		Public Const TYPE_GENERAL_TRANSFORM As Integer = 32

		''' <summary>
		''' This constant is used for the internal state variable to indicate
		''' that no calculations need to be performed and that the source
		''' coordinates only need to be copied to their destinations to
		''' complete the transformation equation of this transform. </summary>
		''' <seealso cref= #APPLY_TRANSLATE </seealso>
		''' <seealso cref= #APPLY_SCALE </seealso>
		''' <seealso cref= #APPLY_SHEAR </seealso>
		''' <seealso cref= #state </seealso>
		Friend Const APPLY_IDENTITY As Integer = 0

		''' <summary>
		''' This constant is used for the internal state variable to indicate
		''' that the translation components of the matrix (m02 and m12) need
		''' to be added to complete the transformation equation of this transform. </summary>
		''' <seealso cref= #APPLY_IDENTITY </seealso>
		''' <seealso cref= #APPLY_SCALE </seealso>
		''' <seealso cref= #APPLY_SHEAR </seealso>
		''' <seealso cref= #state </seealso>
		Friend Const APPLY_TRANSLATE As Integer = 1

		''' <summary>
		''' This constant is used for the internal state variable to indicate
		''' that the scaling components of the matrix (m00 and m11) need
		''' to be factored in to complete the transformation equation of
		''' this transform.  If the APPLY_SHEAR bit is also set then it
		''' indicates that the scaling components are not both 0.0.  If the
		''' APPLY_SHEAR bit is not also set then it indicates that the
		''' scaling components are not both 1.0.  If neither the APPLY_SHEAR
		''' nor the APPLY_SCALE bits are set then the scaling components
		''' are both 1.0, which means that the x and y components contribute
		''' to the transformed coordinate, but they are not multiplied by
		''' any scaling factor. </summary>
		''' <seealso cref= #APPLY_IDENTITY </seealso>
		''' <seealso cref= #APPLY_TRANSLATE </seealso>
		''' <seealso cref= #APPLY_SHEAR </seealso>
		''' <seealso cref= #state </seealso>
		Friend Const APPLY_SCALE As Integer = 2

		''' <summary>
		''' This constant is used for the internal state variable to indicate
		''' that the shearing components of the matrix (m01 and m10) need
		''' to be factored in to complete the transformation equation of this
		''' transform.  The presence of this bit in the state variable changes
		''' the interpretation of the APPLY_SCALE bit as indicated in its
		''' documentation. </summary>
		''' <seealso cref= #APPLY_IDENTITY </seealso>
		''' <seealso cref= #APPLY_TRANSLATE </seealso>
		''' <seealso cref= #APPLY_SCALE </seealso>
		''' <seealso cref= #state </seealso>
		Friend Const APPLY_SHEAR As Integer = 4

	'    
	'     * For methods which combine together the state of two separate
	'     * transforms and dispatch based upon the combination, these constants
	'     * specify how far to shift one of the states so that the two states
	'     * are mutually non-interfering and provide constants for testing the
	'     * bits of the shifted (HI) state.  The methods in this class use
	'     * the convention that the state of "this" transform is unshifted and
	'     * the state of the "other" or "argument" transform is shifted (HI).
	'     
		Private Const HI_SHIFT As Integer = 3
		Private Shared ReadOnly HI_IDENTITY As Integer = APPLY_IDENTITY << HI_SHIFT
		Private Shared ReadOnly HI_TRANSLATE As Integer = APPLY_TRANSLATE << HI_SHIFT
		Private Shared ReadOnly HI_SCALE As Integer = APPLY_SCALE << HI_SHIFT
		Private Shared ReadOnly HI_SHEAR As Integer = APPLY_SHEAR << HI_SHIFT

		''' <summary>
		''' The X coordinate scaling element of the 3x3
		''' affine transformation matrix.
		''' 
		''' @serial
		''' </summary>
		Friend m00 As Double

		''' <summary>
		''' The Y coordinate shearing element of the 3x3
		''' affine transformation matrix.
		''' 
		''' @serial
		''' </summary>
		 Friend m10 As Double

		''' <summary>
		''' The X coordinate shearing element of the 3x3
		''' affine transformation matrix.
		''' 
		''' @serial
		''' </summary>
		 Friend m01 As Double

		''' <summary>
		''' The Y coordinate scaling element of the 3x3
		''' affine transformation matrix.
		''' 
		''' @serial
		''' </summary>
		 Friend m11 As Double

		''' <summary>
		''' The X coordinate of the translation element of the
		''' 3x3 affine transformation matrix.
		''' 
		''' @serial
		''' </summary>
		 Friend m02 As Double

		''' <summary>
		''' The Y coordinate of the translation element of the
		''' 3x3 affine transformation matrix.
		''' 
		''' @serial
		''' </summary>
		 Friend m12 As Double

		''' <summary>
		''' This field keeps track of which components of the matrix need to
		''' be applied when performing a transformation. </summary>
		''' <seealso cref= #APPLY_IDENTITY </seealso>
		''' <seealso cref= #APPLY_TRANSLATE </seealso>
		''' <seealso cref= #APPLY_SCALE </seealso>
		''' <seealso cref= #APPLY_SHEAR </seealso>
		<NonSerialized> _
		Friend state As Integer

		''' <summary>
		''' This field caches the current transformation type of the matrix. </summary>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_FLIP </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM </seealso>
		''' <seealso cref= #TYPE_UNKNOWN </seealso>
		''' <seealso cref= #getType </seealso>
		<NonSerialized> _
		Private type As Integer

		Private Sub New(ByVal m00 As Double, ByVal m10 As Double, ByVal m01 As Double, ByVal m11 As Double, ByVal m02 As Double, ByVal m12 As Double, ByVal state As Integer)
			Me.m00 = m00
			Me.m10 = m10
			Me.m01 = m01
			Me.m11 = m11
			Me.m02 = m02
			Me.m12 = m12
			Me.state = state
			Me.type = TYPE_UNKNOWN
		End Sub

		''' <summary>
		''' Constructs a new <code>AffineTransform</code> representing the
		''' Identity transformation.
		''' @since 1.2
		''' </summary>
		Public Sub New()
				m11 = 1.0
				m00 = m11
			' m01 = m10 = m02 = m12 = 0.0;         /* Not needed. */
			' state = APPLY_IDENTITY;              /* Not needed. */
			' type = TYPE_IDENTITY;                /* Not needed. */
		End Sub

		''' <summary>
		''' Constructs a new <code>AffineTransform</code> that is a copy of
		''' the specified <code>AffineTransform</code> object. </summary>
		''' <param name="Tx"> the <code>AffineTransform</code> object to copy
		''' @since 1.2 </param>
		Public Sub New(ByVal Tx As AffineTransform)
			Me.m00 = Tx.m00
			Me.m10 = Tx.m10
			Me.m01 = Tx.m01
			Me.m11 = Tx.m11
			Me.m02 = Tx.m02
			Me.m12 = Tx.m12
			Me.state = Tx.state
			Me.type = Tx.type
		End Sub

		''' <summary>
		''' Constructs a new <code>AffineTransform</code> from 6 floating point
		''' values representing the 6 specifiable entries of the 3x3
		''' transformation matrix.
		''' </summary>
		''' <param name="m00"> the X coordinate scaling element of the 3x3 matrix </param>
		''' <param name="m10"> the Y coordinate shearing element of the 3x3 matrix </param>
		''' <param name="m01"> the X coordinate shearing element of the 3x3 matrix </param>
		''' <param name="m11"> the Y coordinate scaling element of the 3x3 matrix </param>
		''' <param name="m02"> the X coordinate translation element of the 3x3 matrix </param>
		''' <param name="m12"> the Y coordinate translation element of the 3x3 matrix
		''' @since 1.2 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal m00 As Single, ByVal m10 As Single, ByVal m01 As Single, ByVal m11 As Single, ByVal m02 As Single, ByVal m12 As Single)
			Me.m00 = m00
			Me.m10 = m10
			Me.m01 = m01
			Me.m11 = m11
			Me.m02 = m02
			Me.m12 = m12
			updateState()
		End Sub

		''' <summary>
		''' Constructs a new <code>AffineTransform</code> from an array of
		''' floating point values representing either the 4 non-translation
		''' entries or the 6 specifiable entries of the 3x3 transformation
		''' matrix.  The values are retrieved from the array as
		''' {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;[m02&nbsp;m12]}. </summary>
		''' <param name="flatmatrix"> the float array containing the values to be set
		''' in the new <code>AffineTransform</code> object. The length of the
		''' array is assumed to be at least 4. If the length of the array is
		''' less than 6, only the first 4 values are taken. If the length of
		''' the array is greater than 6, the first 6 values are taken.
		''' @since 1.2 </param>
		Public Sub New(ByVal flatmatrix As Single())
			m00 = flatmatrix(0)
			m10 = flatmatrix(1)
			m01 = flatmatrix(2)
			m11 = flatmatrix(3)
			If flatmatrix.Length > 5 Then
				m02 = flatmatrix(4)
				m12 = flatmatrix(5)
			End If
			updateState()
		End Sub

		''' <summary>
		''' Constructs a new <code>AffineTransform</code> from 6 double
		''' precision values representing the 6 specifiable entries of the 3x3
		''' transformation matrix.
		''' </summary>
		''' <param name="m00"> the X coordinate scaling element of the 3x3 matrix </param>
		''' <param name="m10"> the Y coordinate shearing element of the 3x3 matrix </param>
		''' <param name="m01"> the X coordinate shearing element of the 3x3 matrix </param>
		''' <param name="m11"> the Y coordinate scaling element of the 3x3 matrix </param>
		''' <param name="m02"> the X coordinate translation element of the 3x3 matrix </param>
		''' <param name="m12"> the Y coordinate translation element of the 3x3 matrix
		''' @since 1.2 </param>
		Public Sub New(ByVal m00 As Double, ByVal m10 As Double, ByVal m01 As Double, ByVal m11 As Double, ByVal m02 As Double, ByVal m12 As Double)
			Me.m00 = m00
			Me.m10 = m10
			Me.m01 = m01
			Me.m11 = m11
			Me.m02 = m02
			Me.m12 = m12
			updateState()
		End Sub

		''' <summary>
		''' Constructs a new <code>AffineTransform</code> from an array of
		''' double precision values representing either the 4 non-translation
		''' entries or the 6 specifiable entries of the 3x3 transformation
		''' matrix. The values are retrieved from the array as
		''' {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;[m02&nbsp;m12]}. </summary>
		''' <param name="flatmatrix"> the double array containing the values to be set
		''' in the new <code>AffineTransform</code> object. The length of the
		''' array is assumed to be at least 4. If the length of the array is
		''' less than 6, only the first 4 values are taken. If the length of
		''' the array is greater than 6, the first 6 values are taken.
		''' @since 1.2 </param>
		Public Sub New(ByVal flatmatrix As Double())
			m00 = flatmatrix(0)
			m10 = flatmatrix(1)
			m01 = flatmatrix(2)
			m11 = flatmatrix(3)
			If flatmatrix.Length > 5 Then
				m02 = flatmatrix(4)
				m12 = flatmatrix(5)
			End If
			updateState()
		End Sub

		''' <summary>
		''' Returns a transform representing a translation transformation.
		''' The matrix representing the returned transform is:
		''' <pre>
		'''          [   1    0    tx  ]
		'''          [   0    1    ty  ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="tx"> the distance by which coordinates are translated in the
		''' X axis direction </param>
		''' <param name="ty"> the distance by which coordinates are translated in the
		''' Y axis direction </param>
		''' <returns> an <code>AffineTransform</code> object that represents a
		'''  translation transformation, created with the specified vector.
		''' @since 1.2 </returns>
		Public Shared Function getTranslateInstance(ByVal tx As Double, ByVal ty As Double) As AffineTransform
			Dim Tx_Renamed As New AffineTransform
			Tx_Renamed.toTranslationion(tx, ty)
			Return Tx_Renamed
		End Function

		''' <summary>
		''' Returns a transform representing a rotation transformation.
		''' The matrix representing the returned transform is:
		''' <pre>
		'''          [   cos(theta)    -sin(theta)    0   ]
		'''          [   sin(theta)     cos(theta)    0   ]
		'''          [       0              0         1   ]
		''' </pre>
		''' Rotating by a positive angle theta rotates points on the positive
		''' X axis toward the positive Y axis.
		''' Note also the discussion of
		''' <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		''' above. </summary>
		''' <param name="theta"> the angle of rotation measured in radians </param>
		''' <returns> an <code>AffineTransform</code> object that is a rotation
		'''  transformation, created with the specified angle of rotation.
		''' @since 1.2 </returns>
		Public Shared Function getRotateInstance(ByVal theta As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toRotation = theta
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform that rotates coordinates around an anchor point.
		''' This operation is equivalent to translating the coordinates so
		''' that the anchor point is at the origin (S1), then rotating them
		''' about the new origin (S2), and finally translating so that the
		''' intermediate origin is restored to the coordinates of the original
		''' anchor point (S3).
		''' <p>
		''' This operation is equivalent to the following sequence of calls:
		''' <pre>
		'''     AffineTransform Tx = new AffineTransform();
		'''     Tx.translate(anchorx, anchory);    // S3: final translation
		'''     Tx.rotate(theta);                  // S2: rotate around anchor
		'''     Tx.translate(-anchorx, -anchory);  // S1: translate anchor to origin
		''' </pre>
		''' The matrix representing the returned transform is:
		''' <pre>
		'''          [   cos(theta)    -sin(theta)    x-x*cos+y*sin  ]
		'''          [   sin(theta)     cos(theta)    y-x*sin-y*cos  ]
		'''          [       0              0               1        ]
		''' </pre>
		''' Rotating by a positive angle theta rotates points on the positive
		''' X axis toward the positive Y axis.
		''' Note also the discussion of
		''' <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		''' above.
		''' </summary>
		''' <param name="theta"> the angle of rotation measured in radians </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point </param>
		''' <returns> an <code>AffineTransform</code> object that rotates
		'''  coordinates around the specified point by the specified angle of
		'''  rotation.
		''' @since 1.2 </returns>
		Public Shared Function getRotateInstance(ByVal theta As Double, ByVal anchorx As Double, ByVal anchory As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toRotationion(theta, anchorx, anchory)
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform that rotates coordinates according to
		''' a rotation vector.
		''' All coordinates rotate about the origin by the same amount.
		''' The amount of rotation is such that coordinates along the former
		''' positive X axis will subsequently align with the vector pointing
		''' from the origin to the specified vector coordinates.
		''' If both <code>vecx</code> and <code>vecy</code> are 0.0,
		''' an identity transform is returned.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     AffineTransform.getRotateInstance (System.Math.atan2(vecy, vecx));
		''' </pre>
		''' </summary>
		''' <param name="vecx"> the X coordinate of the rotation vector </param>
		''' <param name="vecy"> the Y coordinate of the rotation vector </param>
		''' <returns> an <code>AffineTransform</code> object that rotates
		'''  coordinates according to the specified rotation vector.
		''' @since 1.6 </returns>
		Public Shared Function getRotateInstance(ByVal vecx As Double, ByVal vecy As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toRotationion(vecx, vecy)
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform that rotates coordinates around an anchor
		''' point according to a rotation vector.
		''' All coordinates rotate about the specified anchor coordinates
		''' by the same amount.
		''' The amount of rotation is such that coordinates along the former
		''' positive X axis will subsequently align with the vector pointing
		''' from the origin to the specified vector coordinates.
		''' If both <code>vecx</code> and <code>vecy</code> are 0.0,
		''' an identity transform is returned.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     AffineTransform.getRotateInstance (System.Math.atan2(vecy, vecx),
		'''                                       anchorx, anchory);
		''' </pre>
		''' </summary>
		''' <param name="vecx"> the X coordinate of the rotation vector </param>
		''' <param name="vecy"> the Y coordinate of the rotation vector </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point </param>
		''' <returns> an <code>AffineTransform</code> object that rotates
		'''  coordinates around the specified point according to the
		'''  specified rotation vector.
		''' @since 1.6 </returns>
		Public Shared Function getRotateInstance(ByVal vecx As Double, ByVal vecy As Double, ByVal anchorx As Double, ByVal anchory As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toRotationion(vecx, vecy, anchorx, anchory)
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform that rotates coordinates by the specified
		''' number of quadrants.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     AffineTransform.getRotateInstance(numquadrants * System.Math.PI / 2.0);
		''' </pre>
		''' Rotating by a positive number of quadrants rotates points on
		''' the positive X axis toward the positive Y axis. </summary>
		''' <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		''' <returns> an <code>AffineTransform</code> object that rotates
		'''  coordinates by the specified number of quadrants.
		''' @since 1.6 </returns>
		Public Shared Function getQuadrantRotateInstance(ByVal numquadrants As Integer) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toQuadrantRotation = numquadrants
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform that rotates coordinates by the specified
		''' number of quadrants around the specified anchor point.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     AffineTransform.getRotateInstance(numquadrants * System.Math.PI / 2.0,
		'''                                       anchorx, anchory);
		''' </pre>
		''' Rotating by a positive number of quadrants rotates points on
		''' the positive X axis toward the positive Y axis.
		''' </summary>
		''' <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point </param>
		''' <returns> an <code>AffineTransform</code> object that rotates
		'''  coordinates by the specified number of quadrants around the
		'''  specified anchor point.
		''' @since 1.6 </returns>
		Public Shared Function getQuadrantRotateInstance(ByVal numquadrants As Integer, ByVal anchorx As Double, ByVal anchory As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toQuadrantRotationion(numquadrants, anchorx, anchory)
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform representing a scaling transformation.
		''' The matrix representing the returned transform is:
		''' <pre>
		'''          [   sx   0    0   ]
		'''          [   0    sy   0   ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="sx"> the factor by which coordinates are scaled along the
		''' X axis direction </param>
		''' <param name="sy"> the factor by which coordinates are scaled along the
		''' Y axis direction </param>
		''' <returns> an <code>AffineTransform</code> object that scales
		'''  coordinates by the specified factors.
		''' @since 1.2 </returns>
		Public Shared Function getScaleInstance(ByVal sx As Double, ByVal sy As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toScaleale(sx, sy)
			Return Tx
		End Function

		''' <summary>
		''' Returns a transform representing a shearing transformation.
		''' The matrix representing the returned transform is:
		''' <pre>
		'''          [   1   shx   0   ]
		'''          [  shy   1    0   ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="shx"> the multiplier by which coordinates are shifted in the
		''' direction of the positive X axis as a factor of their Y coordinate </param>
		''' <param name="shy"> the multiplier by which coordinates are shifted in the
		''' direction of the positive Y axis as a factor of their X coordinate </param>
		''' <returns> an <code>AffineTransform</code> object that shears
		'''  coordinates by the specified multipliers.
		''' @since 1.2 </returns>
		Public Shared Function getShearInstance(ByVal shx As Double, ByVal shy As Double) As AffineTransform
			Dim Tx As New AffineTransform
			Tx.toShearear(shx, shy)
			Return Tx
		End Function

		''' <summary>
		''' Retrieves the flag bits describing the conversion properties of
		''' this transform.
		''' The return value is either one of the constants TYPE_IDENTITY
		''' or TYPE_GENERAL_TRANSFORM, or a combination of the
		''' appropriate flag bits.
		''' A valid combination of flag bits is an exclusive OR operation
		''' that can combine
		''' the TYPE_TRANSLATION flag bit
		''' in addition to either of the
		''' TYPE_UNIFORM_SCALE or TYPE_GENERAL_SCALE flag bits
		''' as well as either of the
		''' TYPE_QUADRANT_ROTATION or TYPE_GENERAL_ROTATION flag bits. </summary>
		''' <returns> the OR combination of any of the indicated flags that
		''' apply to this transform </returns>
		''' <seealso cref= #TYPE_IDENTITY </seealso>
		''' <seealso cref= #TYPE_TRANSLATION </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE </seealso>
		''' <seealso cref= #TYPE_GENERAL_SCALE </seealso>
		''' <seealso cref= #TYPE_QUADRANT_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_ROTATION </seealso>
		''' <seealso cref= #TYPE_GENERAL_TRANSFORM
		''' @since 1.2 </seealso>
		Public Overridable Property type As Integer
			Get
				If type = TYPE_UNKNOWN Then calculateType()
				Return type
			End Get
		End Property

		''' <summary>
		''' This is the utility function to calculate the flag bits when
		''' they have not been cached. </summary>
		''' <seealso cref= #getType </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub calculateType()
			Dim ret As Integer = TYPE_IDENTITY
			Dim sgn0, sgn1 As Boolean
			Dim M0, M1, M2, M3 As Double
			updateState()
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				ret = TYPE_TRANSLATION
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M0 = m00
				M2 = m01
				M3 = m10
				M1 = m11
				If M0 * M2 + M3 * M1 <> 0 Then
					' Transformed unit vectors are not perpendicular...
					Me.type = TYPE_GENERAL_TRANSFORM
					Return
				End If
				sgn0 = (M0 >= 0.0)
				sgn1 = (M1 >= 0.0)
				If sgn0 = sgn1 Then
					' sgn(M0) == sgn(M1) therefore sgn(M2) == -sgn(M3)
					' This is the "unflipped" (right-handed) state
					If M0 <> M1 OrElse M2 <> -M3 Then
						ret = ret Or (TYPE_GENERAL_ROTATION Or TYPE_GENERAL_SCALE)
					ElseIf M0 * M1 - M2 * M3 <> 1.0 Then
						ret = ret Or (TYPE_GENERAL_ROTATION Or TYPE_UNIFORM_SCALE)
					Else
						ret = ret Or TYPE_GENERAL_ROTATION
					End If
				Else
					' sgn(M0) == -sgn(M1) therefore sgn(M2) == sgn(M3)
					' This is the "flipped" (left-handed) state
					If M0 <> -M1 OrElse M2 <> M3 Then
						ret = ret Or (TYPE_GENERAL_ROTATION Or TYPE_FLIP Or TYPE_GENERAL_SCALE)
					ElseIf M0 * M1 - M2 * M3 <> 1.0 Then
						ret = ret Or (TYPE_GENERAL_ROTATION Or TYPE_FLIP Or TYPE_UNIFORM_SCALE)
					Else
						ret = ret Or (TYPE_GENERAL_ROTATION Or TYPE_FLIP)
					End If
				End If
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				ret = TYPE_TRANSLATION
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				sgn0 = ((M0 = m01) >= 0.0)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				sgn1 = ((M1 = m10) >= 0.0)
				If sgn0 <> sgn1 Then
					' Different signs - simple 90 degree rotation
					If M0 <> -M1 Then
						ret = ret Or (TYPE_QUADRANT_ROTATION Or TYPE_GENERAL_SCALE)
					ElseIf M0 <> 1.0 AndAlso M0 <> -1.0 Then
						ret = ret Or (TYPE_QUADRANT_ROTATION Or TYPE_UNIFORM_SCALE)
					Else
						ret = ret Or TYPE_QUADRANT_ROTATION
					End If
				Else
					' Same signs - 90 degree rotation plus an axis flip too
					If M0 = M1 Then
						ret = ret Or (TYPE_QUADRANT_ROTATION Or TYPE_FLIP Or TYPE_UNIFORM_SCALE)
					Else
						ret = ret Or (TYPE_QUADRANT_ROTATION Or TYPE_FLIP Or TYPE_GENERAL_SCALE)
					End If
				End If
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				ret = TYPE_TRANSLATION
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SCALE)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				sgn0 = ((M0 = m00) >= 0.0)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				sgn1 = ((M1 = m11) >= 0.0)
				If sgn0 = sgn1 Then
					If sgn0 Then
						' Both scaling factors non-negative - simple scale
						' Note: APPLY_SCALE implies M0, M1 are not both 1
						If M0 = M1 Then
							ret = ret Or TYPE_UNIFORM_SCALE
						Else
							ret = ret Or TYPE_GENERAL_SCALE
						End If
					Else
						' Both scaling factors negative - 180 degree rotation
						If M0 <> M1 Then
							ret = ret Or (TYPE_QUADRANT_ROTATION Or TYPE_GENERAL_SCALE)
						ElseIf M0 <> -1.0 Then
							ret = ret Or (TYPE_QUADRANT_ROTATION Or TYPE_UNIFORM_SCALE)
						Else
							ret = ret Or TYPE_QUADRANT_ROTATION
						End If
					End If
				Else
					' Scaling factor signs different - flip about some axis
					If M0 = -M1 Then
						If M0 = 1.0 OrElse M0 = -1.0 Then
							ret = ret Or TYPE_FLIP
						Else
							ret = ret Or (TYPE_FLIP Or TYPE_UNIFORM_SCALE)
						End If
					Else
						ret = ret Or (TYPE_FLIP Or TYPE_GENERAL_SCALE)
					End If
				End If
			Case (APPLY_TRANSLATE)
				ret = TYPE_TRANSLATION
			Case (APPLY_IDENTITY)
			End Select
			Me.type = ret
		End Sub

		''' <summary>
		''' Returns the determinant of the matrix representation of the transform.
		''' The determinant is useful both to determine if the transform can
		''' be inverted and to get a single value representing the
		''' combined X and Y scaling of the transform.
		''' <p>
		''' If the determinant is non-zero, then this transform is
		''' invertible and the various methods that depend on the inverse
		''' transform do not need to throw a
		''' <seealso cref="NoninvertibleTransformException"/>.
		''' If the determinant is zero then this transform can not be
		''' inverted since the transform maps all input coordinates onto
		''' a line or a point.
		''' If the determinant is near enough to zero then inverse transform
		''' operations might not carry enough precision to produce meaningful
		''' results.
		''' <p>
		''' If this transform represents a uniform scale, as indicated by
		''' the <code>getType</code> method then the determinant also
		''' represents the square of the uniform scale factor by which all of
		''' the points are expanded from or contracted towards the origin.
		''' If this transform represents a non-uniform scale or more general
		''' transform then the determinant is not likely to represent a
		''' value useful for any purpose other than determining if inverse
		''' transforms are possible.
		''' <p>
		''' Mathematically, the determinant is calculated using the formula:
		''' <pre>
		'''          |  m00  m01  m02  |
		'''          |  m10  m11  m12  |  =  m00 * m11 - m01 * m10
		'''          |   0    0    1   |
		''' </pre>
		''' </summary>
		''' <returns> the determinant of the matrix used to transform the
		''' coordinates. </returns>
		''' <seealso cref= #getType </seealso>
		''' <seealso cref= #createInverse </seealso>
		''' <seealso cref= #inverseTransform </seealso>
		''' <seealso cref= #TYPE_UNIFORM_SCALE
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property determinant As Double
			Get
				Select Case state
				Case Else
					stateError()
					' NOTREACHED 
	'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SHEAR Or APPLY_SCALE)
					Return m00 * m11 - m01 * m10
				Case (APPLY_SHEAR Or APPLY_TRANSLATE), (APPLY_SHEAR)
					Return -(m01 * m10)
				Case (APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SCALE)
					Return m00 * m11
				Case (APPLY_TRANSLATE), (APPLY_IDENTITY)
					Return 1.0
				End Select
			End Get
		End Property

		''' <summary>
		''' Manually recalculates the state of the transform when the matrix
		''' changes too much to predict the effects on the state.
		''' The following table specifies what the various settings of the
		''' state field say about the values of the corresponding matrix
		''' element fields.
		''' Note that the rules governing the SCALE fields are slightly
		''' different depending on whether the SHEAR flag is also set.
		''' <pre>
		'''                     SCALE            SHEAR          TRANSLATE
		'''                    m00/m11          m01/m10          m02/m12
		''' 
		''' IDENTITY             1.0              0.0              0.0
		''' TRANSLATE (TR)       1.0              0.0          not both 0.0
		''' SCALE (SC)       not both 1.0         0.0              0.0
		''' TR | SC          not both 1.0         0.0          not both 0.0
		''' SHEAR (SH)           0.0          not both 0.0         0.0
		''' TR | SH              0.0          not both 0.0     not both 0.0
		''' SC | SH          not both 0.0     not both 0.0         0.0
		''' TR | SC | SH     not both 0.0     not both 0.0     not both 0.0
		''' </pre>
		''' </summary>
		Friend Overridable Sub updateState()
			If m01 = 0.0 AndAlso m10 = 0.0 Then
				If m00 = 1.0 AndAlso m11 = 1.0 Then
					If m02 = 0.0 AndAlso m12 = 0.0 Then
						state = APPLY_IDENTITY
						type = TYPE_IDENTITY
					Else
						state = APPLY_TRANSLATE
						type = TYPE_TRANSLATION
					End If
				Else
					If m02 = 0.0 AndAlso m12 = 0.0 Then
						state = APPLY_SCALE
						type = TYPE_UNKNOWN
					Else
						state = (APPLY_SCALE Or APPLY_TRANSLATE)
						type = TYPE_UNKNOWN
					End If
				End If
			Else
				If m00 = 0.0 AndAlso m11 = 0.0 Then
					If m02 = 0.0 AndAlso m12 = 0.0 Then
						state = APPLY_SHEAR
						type = TYPE_UNKNOWN
					Else
						state = (APPLY_SHEAR Or APPLY_TRANSLATE)
						type = TYPE_UNKNOWN
					End If
				Else
					If m02 = 0.0 AndAlso m12 = 0.0 Then
						state = (APPLY_SHEAR Or APPLY_SCALE)
						type = TYPE_UNKNOWN
					Else
						state = (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
						type = TYPE_UNKNOWN
					End If
				End If
			End If
		End Sub

	'    
	'     * Convenience method used internally to throw exceptions when
	'     * a case was forgotten in a switch statement.
	'     
		Private Sub stateError()
			Throw New InternalError("missing case in transform state switch")
		End Sub

		''' <summary>
		''' Retrieves the 6 specifiable values in the 3x3 affine transformation
		''' matrix and places them into an array of double precisions values.
		''' The values are stored in the array as
		''' {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;m02&nbsp;m12&nbsp;}.
		''' An array of 4 doubles can also be specified, in which case only the
		''' first four elements representing the non-transform
		''' parts of the array are retrieved and the values are stored into
		''' the array as {&nbsp;m00&nbsp;m10&nbsp;m01&nbsp;m11&nbsp;} </summary>
		''' <param name="flatmatrix"> the double array used to store the returned
		''' values. </param>
		''' <seealso cref= #getScaleX </seealso>
		''' <seealso cref= #getScaleY </seealso>
		''' <seealso cref= #getShearX </seealso>
		''' <seealso cref= #getShearY </seealso>
		''' <seealso cref= #getTranslateX </seealso>
		''' <seealso cref= #getTranslateY
		''' @since 1.2 </seealso>
		Public Overridable Sub getMatrix(ByVal flatmatrix As Double())
			flatmatrix(0) = m00
			flatmatrix(1) = m10
			flatmatrix(2) = m01
			flatmatrix(3) = m11
			If flatmatrix.Length > 5 Then
				flatmatrix(4) = m02
				flatmatrix(5) = m12
			End If
		End Sub

		''' <summary>
		''' Returns the X coordinate scaling element (m00) of the 3x3
		''' affine transformation matrix. </summary>
		''' <returns> a double value that is the X coordinate of the scaling
		'''  element of the affine transformation matrix. </returns>
		''' <seealso cref= #getMatrix
		''' @since 1.2 </seealso>
		Public Overridable Property scaleX As Double
			Get
				Return m00
			End Get
		End Property

		''' <summary>
		''' Returns the Y coordinate scaling element (m11) of the 3x3
		''' affine transformation matrix. </summary>
		''' <returns> a double value that is the Y coordinate of the scaling
		'''  element of the affine transformation matrix. </returns>
		''' <seealso cref= #getMatrix
		''' @since 1.2 </seealso>
		Public Overridable Property scaleY As Double
			Get
				Return m11
			End Get
		End Property

		''' <summary>
		''' Returns the X coordinate shearing element (m01) of the 3x3
		''' affine transformation matrix. </summary>
		''' <returns> a double value that is the X coordinate of the shearing
		'''  element of the affine transformation matrix. </returns>
		''' <seealso cref= #getMatrix
		''' @since 1.2 </seealso>
		Public Overridable Property shearX As Double
			Get
				Return m01
			End Get
		End Property

		''' <summary>
		''' Returns the Y coordinate shearing element (m10) of the 3x3
		''' affine transformation matrix. </summary>
		''' <returns> a double value that is the Y coordinate of the shearing
		'''  element of the affine transformation matrix. </returns>
		''' <seealso cref= #getMatrix
		''' @since 1.2 </seealso>
		Public Overridable Property shearY As Double
			Get
				Return m10
			End Get
		End Property

		''' <summary>
		''' Returns the X coordinate of the translation element (m02) of the
		''' 3x3 affine transformation matrix. </summary>
		''' <returns> a double value that is the X coordinate of the translation
		'''  element of the affine transformation matrix. </returns>
		''' <seealso cref= #getMatrix
		''' @since 1.2 </seealso>
		Public Overridable Property translateX As Double
			Get
				Return m02
			End Get
		End Property

		''' <summary>
		''' Returns the Y coordinate of the translation element (m12) of the
		''' 3x3 affine transformation matrix. </summary>
		''' <returns> a double value that is the Y coordinate of the translation
		'''  element of the affine transformation matrix. </returns>
		''' <seealso cref= #getMatrix
		''' @since 1.2 </seealso>
		Public Overridable Property translateY As Double
			Get
				Return m12
			End Get
		End Property

		''' <summary>
		''' Concatenates this transform with a translation transformation.
		''' This is equivalent to calling concatenate(T), where T is an
		''' <code>AffineTransform</code> represented by the following matrix:
		''' <pre>
		'''          [   1    0    tx  ]
		'''          [   0    1    ty  ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="tx"> the distance by which coordinates are translated in the
		''' X axis direction </param>
		''' <param name="ty"> the distance by which coordinates are translated in the
		''' Y axis direction
		''' @since 1.2 </param>
		Public Overridable Sub translate(ByVal tx As Double, ByVal ty As Double)
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				m02 = tx * m00 + ty * m01 + m02
				m12 = tx * m10 + ty * m11 + m12
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_SHEAR Or APPLY_SCALE
					If type <> TYPE_UNKNOWN Then type -= TYPE_TRANSLATION
				End If
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE)
				m02 = tx * m00 + ty * m01
				m12 = tx * m10 + ty * m11
				If m02 <> 0.0 OrElse m12 <> 0.0 Then
					state = APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE
					type = type Or TYPE_TRANSLATION
				End If
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				m02 = ty * m01 + m02
				m12 = tx * m10 + m12
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_SHEAR
					If type <> TYPE_UNKNOWN Then type -= TYPE_TRANSLATION
				End If
				Return
			Case (APPLY_SHEAR)
				m02 = ty * m01
				m12 = tx * m10
				If m02 <> 0.0 OrElse m12 <> 0.0 Then
					state = APPLY_SHEAR Or APPLY_TRANSLATE
					type = type Or TYPE_TRANSLATION
				End If
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				m02 = tx * m00 + m02
				m12 = ty * m11 + m12
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_SCALE
					If type <> TYPE_UNKNOWN Then type -= TYPE_TRANSLATION
				End If
				Return
			Case (APPLY_SCALE)
				m02 = tx * m00
				m12 = ty * m11
				If m02 <> 0.0 OrElse m12 <> 0.0 Then
					state = APPLY_SCALE Or APPLY_TRANSLATE
					type = type Or TYPE_TRANSLATION
				End If
				Return
			Case (APPLY_TRANSLATE)
				m02 = tx + m02
				m12 = ty + m12
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_IDENTITY
					type = TYPE_IDENTITY
				End If
				Return
			Case (APPLY_IDENTITY)
				m02 = tx
				m12 = ty
				If tx <> 0.0 OrElse ty <> 0.0 Then
					state = APPLY_TRANSLATE
					type = TYPE_TRANSLATION
				End If
				Return
			End Select
		End Sub

		' Utility methods to optimize rotate methods.
		' These tables translate the flags during predictable quadrant
		' rotations where the shear and scale values are swapped and negated.
		Private Shared ReadOnly rot90conversion As Integer() = { APPLY_SHEAR, APPLY_SHEAR Or APPLY_TRANSLATE, APPLY_SHEAR, APPLY_SHEAR Or APPLY_TRANSLATE, APPLY_SCALE, APPLY_SCALE Or APPLY_TRANSLATE, APPLY_SHEAR Or APPLY_SCALE, APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE }
		Private Sub rotate90()
			Dim M0 As Double = m00
			m00 = m01
			m01 = -M0
			M0 = m10
			m10 = m11
			m11 = -M0
			Dim state As Integer = rot90conversion(Me.state)
			If (state And (APPLY_SHEAR Or APPLY_SCALE)) = APPLY_SCALE AndAlso m00 = 1.0 AndAlso m11 = 1.0 Then state -= APPLY_SCALE
			Me.state = state
			type = TYPE_UNKNOWN
		End Sub
		Private Sub rotate180()
			m00 = -m00
			m11 = -m11
			Dim state As Integer = Me.state
			If (state And (APPLY_SHEAR)) <> 0 Then
				' If there was a shear, then this rotation has no
				' effect on the state.
				m01 = -m01
				m10 = -m10
			Else
				' No shear means the SCALE state may toggle when
				' m00 and m11 are negated.
				If m00 = 1.0 AndAlso m11 = 1.0 Then
					Me.state = state And Not APPLY_SCALE
				Else
					Me.state = state Or APPLY_SCALE
				End If
			End If
			type = TYPE_UNKNOWN
		End Sub
		Private Sub rotate270()
			Dim M0 As Double = m00
			m00 = -m01
			m01 = M0
			M0 = m10
			m10 = -m11
			m11 = M0
			Dim state As Integer = rot90conversion(Me.state)
			If (state And (APPLY_SHEAR Or APPLY_SCALE)) = APPLY_SCALE AndAlso m00 = 1.0 AndAlso m11 = 1.0 Then state -= APPLY_SCALE
			Me.state = state
			type = TYPE_UNKNOWN
		End Sub

		''' <summary>
		''' Concatenates this transform with a rotation transformation.
		''' This is equivalent to calling concatenate(R), where R is an
		''' <code>AffineTransform</code> represented by the following matrix:
		''' <pre>
		'''          [   cos(theta)    -sin(theta)    0   ]
		'''          [   sin(theta)     cos(theta)    0   ]
		'''          [       0              0         1   ]
		''' </pre>
		''' Rotating by a positive angle theta rotates points on the positive
		''' X axis toward the positive Y axis.
		''' Note also the discussion of
		''' <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		''' above. </summary>
		''' <param name="theta"> the angle of rotation measured in radians
		''' @since 1.2 </param>
		Public Overridable Sub rotate(ByVal theta As Double)
			Dim sin As Double = System.Math.Sin(theta)
			If sin = 1.0 Then
				rotate90()
			ElseIf sin = -1.0 Then
				rotate270()
			Else
				Dim cos As Double = System.Math.Cos(theta)
				If cos = -1.0 Then
					rotate180()
				ElseIf cos <> 1.0 Then
					Dim M0, M1 As Double
					M0 = m00
					M1 = m01
					m00 = cos * M0 + sin * M1
					m01 = -sin * M0 + cos * M1
					M0 = m10
					M1 = m11
					m10 = cos * M0 + sin * M1
					m11 = -sin * M0 + cos * M1
					updateState()
				End If
			End If
		End Sub

		''' <summary>
		''' Concatenates this transform with a transform that rotates
		''' coordinates around an anchor point.
		''' This operation is equivalent to translating the coordinates so
		''' that the anchor point is at the origin (S1), then rotating them
		''' about the new origin (S2), and finally translating so that the
		''' intermediate origin is restored to the coordinates of the original
		''' anchor point (S3).
		''' <p>
		''' This operation is equivalent to the following sequence of calls:
		''' <pre>
		'''     translate(anchorx, anchory);      // S3: final translation
		'''     rotate(theta);                    // S2: rotate around anchor
		'''     translate(-anchorx, -anchory);    // S1: translate anchor to origin
		''' </pre>
		''' Rotating by a positive angle theta rotates points on the positive
		''' X axis toward the positive Y axis.
		''' Note also the discussion of
		''' <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		''' above.
		''' </summary>
		''' <param name="theta"> the angle of rotation measured in radians </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point
		''' @since 1.2 </param>
		Public Overridable Sub rotate(ByVal theta As Double, ByVal anchorx As Double, ByVal anchory As Double)
			' REMIND: Simple for now - optimize later
			translate(anchorx, anchory)
			rotate(theta)
			translate(-anchorx, -anchory)
		End Sub

		''' <summary>
		''' Concatenates this transform with a transform that rotates
		''' coordinates according to a rotation vector.
		''' All coordinates rotate about the origin by the same amount.
		''' The amount of rotation is such that coordinates along the former
		''' positive X axis will subsequently align with the vector pointing
		''' from the origin to the specified vector coordinates.
		''' If both <code>vecx</code> and <code>vecy</code> are 0.0,
		''' no additional rotation is added to this transform.
		''' This operation is equivalent to calling:
		''' <pre>
		'''          rotate (System.Math.atan2(vecy, vecx));
		''' </pre>
		''' </summary>
		''' <param name="vecx"> the X coordinate of the rotation vector </param>
		''' <param name="vecy"> the Y coordinate of the rotation vector
		''' @since 1.6 </param>
		Public Overridable Sub rotate(ByVal vecx As Double, ByVal vecy As Double)
			If vecy = 0.0 Then
				If vecx < 0.0 Then rotate180()
				' If vecx > 0.0 - no rotation
				' If vecx == 0.0 - undefined rotation - treat as no rotation
			ElseIf vecx = 0.0 Then
				If vecy > 0.0 Then
					rotate90() ' vecy must be < 0.0
				Else
					rotate270()
				End If
			Else
				Dim len As Double = System.Math.Sqrt(vecx * vecx + vecy * vecy)
				Dim sin As Double = vecy / len
				Dim cos As Double = vecx / len
				Dim M0, M1 As Double
				M0 = m00
				M1 = m01
				m00 = cos * M0 + sin * M1
				m01 = -sin * M0 + cos * M1
				M0 = m10
				M1 = m11
				m10 = cos * M0 + sin * M1
				m11 = -sin * M0 + cos * M1
				updateState()
			End If
		End Sub

		''' <summary>
		''' Concatenates this transform with a transform that rotates
		''' coordinates around an anchor point according to a rotation
		''' vector.
		''' All coordinates rotate about the specified anchor coordinates
		''' by the same amount.
		''' The amount of rotation is such that coordinates along the former
		''' positive X axis will subsequently align with the vector pointing
		''' from the origin to the specified vector coordinates.
		''' If both <code>vecx</code> and <code>vecy</code> are 0.0,
		''' the transform is not modified in any way.
		''' This method is equivalent to calling:
		''' <pre>
		'''     rotate (System.Math.atan2(vecy, vecx), anchorx, anchory);
		''' </pre>
		''' </summary>
		''' <param name="vecx"> the X coordinate of the rotation vector </param>
		''' <param name="vecy"> the Y coordinate of the rotation vector </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point
		''' @since 1.6 </param>
		Public Overridable Sub rotate(ByVal vecx As Double, ByVal vecy As Double, ByVal anchorx As Double, ByVal anchory As Double)
			' REMIND: Simple for now - optimize later
			translate(anchorx, anchory)
			rotate(vecx, vecy)
			translate(-anchorx, -anchory)
		End Sub

		''' <summary>
		''' Concatenates this transform with a transform that rotates
		''' coordinates by the specified number of quadrants.
		''' This is equivalent to calling:
		''' <pre>
		'''     rotate(numquadrants * System.Math.PI / 2.0);
		''' </pre>
		''' Rotating by a positive number of quadrants rotates points on
		''' the positive X axis toward the positive Y axis. </summary>
		''' <param name="numquadrants"> the number of 90 degree arcs to rotate by
		''' @since 1.6 </param>
		Public Overridable Sub quadrantRotate(ByVal numquadrants As Integer)
			Select Case numquadrants And 3
			Case 0
			Case 1
				rotate90()
			Case 2
				rotate180()
			Case 3
				rotate270()
			End Select
		End Sub

		''' <summary>
		''' Concatenates this transform with a transform that rotates
		''' coordinates by the specified number of quadrants around
		''' the specified anchor point.
		''' This method is equivalent to calling:
		''' <pre>
		'''     rotate(numquadrants * System.Math.PI / 2.0, anchorx, anchory);
		''' </pre>
		''' Rotating by a positive number of quadrants rotates points on
		''' the positive X axis toward the positive Y axis.
		''' </summary>
		''' <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point
		''' @since 1.6 </param>
		Public Overridable Sub quadrantRotate(ByVal numquadrants As Integer, ByVal anchorx As Double, ByVal anchory As Double)
			Select Case numquadrants And 3
			Case 0
				Return
			Case 1
				m02 += anchorx * (m00 - m01) + anchory * (m01 + m00)
				m12 += anchorx * (m10 - m11) + anchory * (m11 + m10)
				rotate90()
			Case 2
				m02 += anchorx * (m00 + m00) + anchory * (m01 + m01)
				m12 += anchorx * (m10 + m10) + anchory * (m11 + m11)
				rotate180()
			Case 3
				m02 += anchorx * (m00 + m01) + anchory * (m01 - m00)
				m12 += anchorx * (m10 + m11) + anchory * (m11 - m10)
				rotate270()
			End Select
			If m02 = 0.0 AndAlso m12 = 0.0 Then
				state = state And Not APPLY_TRANSLATE
			Else
				state = state Or APPLY_TRANSLATE
			End If
		End Sub

		''' <summary>
		''' Concatenates this transform with a scaling transformation.
		''' This is equivalent to calling concatenate(S), where S is an
		''' <code>AffineTransform</code> represented by the following matrix:
		''' <pre>
		'''          [   sx   0    0   ]
		'''          [   0    sy   0   ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="sx"> the factor by which coordinates are scaled along the
		''' X axis direction </param>
		''' <param name="sy"> the factor by which coordinates are scaled along the
		''' Y axis direction
		''' @since 1.2 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub scale(ByVal sx As Double, ByVal sy As Double)
			Dim state As Integer = Me.state
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SHEAR Or APPLY_SCALE)
				m00 *= sx
				m11 *= sy
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_TRANSLATE), (APPLY_SHEAR)
				m01 *= sy
				m10 *= sx
				If m01 = 0 AndAlso m10 = 0 Then
					state = state And APPLY_TRANSLATE
					If m00 = 1.0 AndAlso m11 = 1.0 Then
						Me.type = (If(state = APPLY_IDENTITY, TYPE_IDENTITY, TYPE_TRANSLATION))
					Else
						state = state Or APPLY_SCALE
						Me.type = TYPE_UNKNOWN
					End If
					Me.state = state
				End If
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SCALE)
				m00 *= sx
				m11 *= sy
				If m00 = 1.0 AndAlso m11 = 1.0 Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Me.state = (state = state And APPLY_TRANSLATE)
					Me.type = (If(state = APPLY_IDENTITY, TYPE_IDENTITY, TYPE_TRANSLATION))
				Else
					Me.type = TYPE_UNKNOWN
				End If
				Return
			Case (APPLY_TRANSLATE), (APPLY_IDENTITY)
				m00 = sx
				m11 = sy
				If sx <> 1.0 OrElse sy <> 1.0 Then
					Me.state = state Or APPLY_SCALE
					Me.type = TYPE_UNKNOWN
				End If
				Return
			End Select
		End Sub

		''' <summary>
		''' Concatenates this transform with a shearing transformation.
		''' This is equivalent to calling concatenate(SH), where SH is an
		''' <code>AffineTransform</code> represented by the following matrix:
		''' <pre>
		'''          [   1   shx   0   ]
		'''          [  shy   1    0   ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="shx"> the multiplier by which coordinates are shifted in the
		''' direction of the positive X axis as a factor of their Y coordinate </param>
		''' <param name="shy"> the multiplier by which coordinates are shifted in the
		''' direction of the positive Y axis as a factor of their X coordinate
		''' @since 1.2 </param>
		Public Overridable Sub shear(ByVal shx As Double, ByVal shy As Double)
			Dim state As Integer = Me.state
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SHEAR Or APPLY_SCALE)
				Dim M0, M1 As Double
				M0 = m00
				M1 = m01
				m00 = M0 + M1 * shy
				m01 = M0 * shx + M1

				M0 = m10
				M1 = m11
				m10 = M0 + M1 * shy
				m11 = M0 * shx + M1
				updateState()
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE), (APPLY_SHEAR)
				m00 = m01 * shy
				m11 = m10 * shx
				If m00 <> 0.0 OrElse m11 <> 0.0 Then Me.state = state Or APPLY_SCALE
				Me.type = TYPE_UNKNOWN
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SCALE)
				m01 = m00 * shx
				m10 = m11 * shy
				If m01 <> 0.0 OrElse m10 <> 0.0 Then Me.state = state Or APPLY_SHEAR
				Me.type = TYPE_UNKNOWN
				Return
			Case (APPLY_TRANSLATE), (APPLY_IDENTITY)
				m01 = shx
				m10 = shy
				If m01 <> 0.0 OrElse m10 <> 0.0 Then
					Me.state = state Or APPLY_SCALE Or APPLY_SHEAR
					Me.type = TYPE_UNKNOWN
				End If
				Return
			End Select
		End Sub

		''' <summary>
		''' Resets this transform to the Identity transform.
		''' @since 1.2
		''' </summary>
		Public Overridable Sub setToIdentity()
				m11 = 1.0
				m00 = m11
				m12 = 0.0
					m02 = m12
						m01 = m02
						m10 = m01
			state = APPLY_IDENTITY
			type = TYPE_IDENTITY
		End Sub

		''' <summary>
		''' Sets this transform to a translation transformation.
		''' The matrix representing this transform becomes:
		''' <pre>
		'''          [   1    0    tx  ]
		'''          [   0    1    ty  ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="tx"> the distance by which coordinates are translated in the
		''' X axis direction </param>
		''' <param name="ty"> the distance by which coordinates are translated in the
		''' Y axis direction
		''' @since 1.2 </param>
		Public Overridable Sub setToTranslation(ByVal tx As Double, ByVal ty As Double)
			m00 = 1.0
			m10 = 0.0
			m01 = 0.0
			m11 = 1.0
			m02 = tx
			m12 = ty
			If tx <> 0.0 OrElse ty <> 0.0 Then
				state = APPLY_TRANSLATE
				type = TYPE_TRANSLATION
			Else
				state = APPLY_IDENTITY
				type = TYPE_IDENTITY
			End If
		End Sub

		''' <summary>
		''' Sets this transform to a rotation transformation.
		''' The matrix representing this transform becomes:
		''' <pre>
		'''          [   cos(theta)    -sin(theta)    0   ]
		'''          [   sin(theta)     cos(theta)    0   ]
		'''          [       0              0         1   ]
		''' </pre>
		''' Rotating by a positive angle theta rotates points on the positive
		''' X axis toward the positive Y axis.
		''' Note also the discussion of
		''' <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		''' above. </summary>
		''' <param name="theta"> the angle of rotation measured in radians
		''' @since 1.2 </param>
		Public Overridable Property toRotation As Double
			Set(ByVal theta As Double)
				Dim sin As Double = System.Math.Sin(theta)
				Dim cos As Double
				If sin = 1.0 OrElse sin = -1.0 Then
					cos = 0.0
					state = APPLY_SHEAR
					type = TYPE_QUADRANT_ROTATION
				Else
					cos = System.Math.Cos(theta)
					If cos = -1.0 Then
						sin = 0.0
						state = APPLY_SCALE
						type = TYPE_QUADRANT_ROTATION
					ElseIf cos = 1.0 Then
						sin = 0.0
						state = APPLY_IDENTITY
						type = TYPE_IDENTITY
					Else
						state = APPLY_SHEAR Or APPLY_SCALE
						type = TYPE_GENERAL_ROTATION
					End If
				End If
				m00 = cos
				m10 = sin
				m01 = -sin
				m11 = cos
				m02 = 0.0
				m12 = 0.0
			End Set
		End Property

		''' <summary>
		''' Sets this transform to a translated rotation transformation.
		''' This operation is equivalent to translating the coordinates so
		''' that the anchor point is at the origin (S1), then rotating them
		''' about the new origin (S2), and finally translating so that the
		''' intermediate origin is restored to the coordinates of the original
		''' anchor point (S3).
		''' <p>
		''' This operation is equivalent to the following sequence of calls:
		''' <pre>
		'''     setToTranslation(anchorx, anchory); // S3: final translation
		'''     rotate(theta);                      // S2: rotate around anchor
		'''     translate(-anchorx, -anchory);      // S1: translate anchor to origin
		''' </pre>
		''' The matrix representing this transform becomes:
		''' <pre>
		'''          [   cos(theta)    -sin(theta)    x-x*cos+y*sin  ]
		'''          [   sin(theta)     cos(theta)    y-x*sin-y*cos  ]
		'''          [       0              0               1        ]
		''' </pre>
		''' Rotating by a positive angle theta rotates points on the positive
		''' X axis toward the positive Y axis.
		''' Note also the discussion of
		''' <a href="#quadrantapproximation">Handling 90-Degree Rotations</a>
		''' above.
		''' </summary>
		''' <param name="theta"> the angle of rotation measured in radians </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point
		''' @since 1.2 </param>
		Public Overridable Sub setToRotation(ByVal theta As Double, ByVal anchorx As Double, ByVal anchory As Double)
			toRotation = theta
			Dim sin As Double = m10
			Dim oneMinusCos As Double = 1.0 - m00
			m02 = anchorx * oneMinusCos + anchory * sin
			m12 = anchory * oneMinusCos - anchorx * sin
			If m02 <> 0.0 OrElse m12 <> 0.0 Then
				state = state Or APPLY_TRANSLATE
				type = type Or TYPE_TRANSLATION
			End If
		End Sub

		''' <summary>
		''' Sets this transform to a rotation transformation that rotates
		''' coordinates according to a rotation vector.
		''' All coordinates rotate about the origin by the same amount.
		''' The amount of rotation is such that coordinates along the former
		''' positive X axis will subsequently align with the vector pointing
		''' from the origin to the specified vector coordinates.
		''' If both <code>vecx</code> and <code>vecy</code> are 0.0,
		''' the transform is set to an identity transform.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     setToRotation (System.Math.atan2(vecy, vecx));
		''' </pre>
		''' </summary>
		''' <param name="vecx"> the X coordinate of the rotation vector </param>
		''' <param name="vecy"> the Y coordinate of the rotation vector
		''' @since 1.6 </param>
		Public Overridable Sub setToRotation(ByVal vecx As Double, ByVal vecy As Double)
			Dim sin, cos As Double
			If vecy = 0 Then
				sin = 0.0
				If vecx < 0.0 Then
					cos = -1.0
					state = APPLY_SCALE
					type = TYPE_QUADRANT_ROTATION
				Else
					cos = 1.0
					state = APPLY_IDENTITY
					type = TYPE_IDENTITY
				End If
			ElseIf vecx = 0 Then
				cos = 0.0
				sin = If(vecy > 0.0, 1.0, -1.0)
				state = APPLY_SHEAR
				type = TYPE_QUADRANT_ROTATION
			Else
				Dim len As Double = System.Math.Sqrt(vecx * vecx + vecy * vecy)
				cos = vecx / len
				sin = vecy / len
				state = APPLY_SHEAR Or APPLY_SCALE
				type = TYPE_GENERAL_ROTATION
			End If
			m00 = cos
			m10 = sin
			m01 = -sin
			m11 = cos
			m02 = 0.0
			m12 = 0.0
		End Sub

		''' <summary>
		''' Sets this transform to a rotation transformation that rotates
		''' coordinates around an anchor point according to a rotation
		''' vector.
		''' All coordinates rotate about the specified anchor coordinates
		''' by the same amount.
		''' The amount of rotation is such that coordinates along the former
		''' positive X axis will subsequently align with the vector pointing
		''' from the origin to the specified vector coordinates.
		''' If both <code>vecx</code> and <code>vecy</code> are 0.0,
		''' the transform is set to an identity transform.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     setToTranslation (System.Math.atan2(vecy, vecx), anchorx, anchory);
		''' </pre>
		''' </summary>
		''' <param name="vecx"> the X coordinate of the rotation vector </param>
		''' <param name="vecy"> the Y coordinate of the rotation vector </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point
		''' @since 1.6 </param>
		Public Overridable Sub setToRotation(ByVal vecx As Double, ByVal vecy As Double, ByVal anchorx As Double, ByVal anchory As Double)
			toRotationion(vecx, vecy)
			Dim sin As Double = m10
			Dim oneMinusCos As Double = 1.0 - m00
			m02 = anchorx * oneMinusCos + anchory * sin
			m12 = anchory * oneMinusCos - anchorx * sin
			If m02 <> 0.0 OrElse m12 <> 0.0 Then
				state = state Or APPLY_TRANSLATE
				type = type Or TYPE_TRANSLATION
			End If
		End Sub

		''' <summary>
		''' Sets this transform to a rotation transformation that rotates
		''' coordinates by the specified number of quadrants.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     setToRotation(numquadrants * System.Math.PI / 2.0);
		''' </pre>
		''' Rotating by a positive number of quadrants rotates points on
		''' the positive X axis toward the positive Y axis. </summary>
		''' <param name="numquadrants"> the number of 90 degree arcs to rotate by
		''' @since 1.6 </param>
		Public Overridable Property toQuadrantRotation As Integer
			Set(ByVal numquadrants As Integer)
				Select Case numquadrants And 3
				Case 0
					m00 = 1.0
					m10 = 0.0
					m01 = 0.0
					m11 = 1.0
					m02 = 0.0
					m12 = 0.0
					state = APPLY_IDENTITY
					type = TYPE_IDENTITY
				Case 1
					m00 = 0.0
					m10 = 1.0
					m01 = -1.0
					m11 = 0.0
					m02 = 0.0
					m12 = 0.0
					state = APPLY_SHEAR
					type = TYPE_QUADRANT_ROTATION
				Case 2
					m00 = -1.0
					m10 = 0.0
					m01 = 0.0
					m11 = -1.0
					m02 = 0.0
					m12 = 0.0
					state = APPLY_SCALE
					type = TYPE_QUADRANT_ROTATION
				Case 3
					m00 = 0.0
					m10 = -1.0
					m01 = 1.0
					m11 = 0.0
					m02 = 0.0
					m12 = 0.0
					state = APPLY_SHEAR
					type = TYPE_QUADRANT_ROTATION
				End Select
			End Set
		End Property

		''' <summary>
		''' Sets this transform to a translated rotation transformation
		''' that rotates coordinates by the specified number of quadrants
		''' around the specified anchor point.
		''' This operation is equivalent to calling:
		''' <pre>
		'''     setToRotation(numquadrants * System.Math.PI / 2.0, anchorx, anchory);
		''' </pre>
		''' Rotating by a positive number of quadrants rotates points on
		''' the positive X axis toward the positive Y axis.
		''' </summary>
		''' <param name="numquadrants"> the number of 90 degree arcs to rotate by </param>
		''' <param name="anchorx"> the X coordinate of the rotation anchor point </param>
		''' <param name="anchory"> the Y coordinate of the rotation anchor point
		''' @since 1.6 </param>
		Public Overridable Sub setToQuadrantRotation(ByVal numquadrants As Integer, ByVal anchorx As Double, ByVal anchory As Double)
			Select Case numquadrants And 3
			Case 0
				m00 = 1.0
				m10 = 0.0
				m01 = 0.0
				m11 = 1.0
				m02 = 0.0
				m12 = 0.0
				state = APPLY_IDENTITY
				type = TYPE_IDENTITY
			Case 1
				m00 = 0.0
				m10 = 1.0
				m01 = -1.0
				m11 = 0.0
				m02 = anchorx + anchory
				m12 = anchory - anchorx
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_SHEAR
					type = TYPE_QUADRANT_ROTATION
				Else
					state = APPLY_SHEAR Or APPLY_TRANSLATE
					type = TYPE_QUADRANT_ROTATION Or TYPE_TRANSLATION
				End If
			Case 2
				m00 = -1.0
				m10 = 0.0
				m01 = 0.0
				m11 = -1.0
				m02 = anchorx + anchorx
				m12 = anchory + anchory
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_SCALE
					type = TYPE_QUADRANT_ROTATION
				Else
					state = APPLY_SCALE Or APPLY_TRANSLATE
					type = TYPE_QUADRANT_ROTATION Or TYPE_TRANSLATION
				End If
			Case 3
				m00 = 0.0
				m10 = -1.0
				m01 = 1.0
				m11 = 0.0
				m02 = anchorx - anchory
				m12 = anchory + anchorx
				If m02 = 0.0 AndAlso m12 = 0.0 Then
					state = APPLY_SHEAR
					type = TYPE_QUADRANT_ROTATION
				Else
					state = APPLY_SHEAR Or APPLY_TRANSLATE
					type = TYPE_QUADRANT_ROTATION Or TYPE_TRANSLATION
				End If
			End Select
		End Sub

		''' <summary>
		''' Sets this transform to a scaling transformation.
		''' The matrix representing this transform becomes:
		''' <pre>
		'''          [   sx   0    0   ]
		'''          [   0    sy   0   ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="sx"> the factor by which coordinates are scaled along the
		''' X axis direction </param>
		''' <param name="sy"> the factor by which coordinates are scaled along the
		''' Y axis direction
		''' @since 1.2 </param>
		Public Overridable Sub setToScale(ByVal sx As Double, ByVal sy As Double)
			m00 = sx
			m10 = 0.0
			m01 = 0.0
			m11 = sy
			m02 = 0.0
			m12 = 0.0
			If sx <> 1.0 OrElse sy <> 1.0 Then
				state = APPLY_SCALE
				type = TYPE_UNKNOWN
			Else
				state = APPLY_IDENTITY
				type = TYPE_IDENTITY
			End If
		End Sub

		''' <summary>
		''' Sets this transform to a shearing transformation.
		''' The matrix representing this transform becomes:
		''' <pre>
		'''          [   1   shx   0   ]
		'''          [  shy   1    0   ]
		'''          [   0    0    1   ]
		''' </pre> </summary>
		''' <param name="shx"> the multiplier by which coordinates are shifted in the
		''' direction of the positive X axis as a factor of their Y coordinate </param>
		''' <param name="shy"> the multiplier by which coordinates are shifted in the
		''' direction of the positive Y axis as a factor of their X coordinate
		''' @since 1.2 </param>
		Public Overridable Sub setToShear(ByVal shx As Double, ByVal shy As Double)
			m00 = 1.0
			m01 = shx
			m10 = shy
			m11 = 1.0
			m02 = 0.0
			m12 = 0.0
			If shx <> 0.0 OrElse shy <> 0.0 Then
				state = (APPLY_SHEAR Or APPLY_SCALE)
				type = TYPE_UNKNOWN
			Else
				state = APPLY_IDENTITY
				type = TYPE_IDENTITY
			End If
		End Sub

        ''' <summary>
        ''' Sets this transform to a copy of the transform in the specified
        ''' <code>AffineTransform</code> object. </summary>
        ''' <param name="Tx"> the <code>AffineTransform</code> object from which to
        ''' copy the transform
        ''' @since 1.2 </param>
        Public Overridable WriteOnly Property transform As AffineTransform
            Set(ByVal Tx As AffineTransform)
                Me.m00 = Tx.m00
                Me.m10 = Tx.m10
                Me.m01 = Tx.m01
                Me.m11 = Tx.m11
                Me.m02 = Tx.m02
                Me.m12 = Tx.m12
                Me.state = Tx.state
                Me.type = Tx.type
            End Set
        End Property

        ''' <summary>
        ''' Sets this transform to the matrix specified by the 6
        ''' double precision values.
        ''' </summary>
        ''' <param name="m00"> the X coordinate scaling element of the 3x3 matrix </param>
        ''' <param name="m10"> the Y coordinate shearing element of the 3x3 matrix </param>
        ''' <param name="m01"> the X coordinate shearing element of the 3x3 matrix </param>
        ''' <param name="m11"> the Y coordinate scaling element of the 3x3 matrix </param>
        ''' <param name="m02"> the X coordinate translation element of the 3x3 matrix </param>
        ''' <param name="m12"> the Y coordinate translation element of the 3x3 matrix
        ''' @since 1.2 </param>
        Public Overridable Sub setTransform(ByVal m00 As Double, ByVal m10 As Double, ByVal m01 As Double, ByVal m11 As Double, ByVal m02 As Double, ByVal m12 As Double)
			Me.m00 = m00
			Me.m10 = m10
			Me.m01 = m01
			Me.m11 = m11
			Me.m02 = m02
			Me.m12 = m12
			updateState()
		End Sub

		''' <summary>
		''' Concatenates an <code>AffineTransform</code> <code>Tx</code> to
		''' this <code>AffineTransform</code> Cx in the most commonly useful
		''' way to provide a new user space
		''' that is mapped to the former user space by <code>Tx</code>.
		''' Cx is updated to perform the combined transformation.
		''' Transforming a point p by the updated transform Cx' is
		''' equivalent to first transforming p by <code>Tx</code> and then
		''' transforming the result by the original transform Cx like this:
		''' Cx'(p) = Cx(Tx(p))
		''' In matrix notation, if this transform Cx is
		''' represented by the matrix [this] and <code>Tx</code> is represented
		''' by the matrix [Tx] then this method does the following:
		''' <pre>
		'''          [this] = [this] x [Tx]
		''' </pre> </summary>
		''' <param name="Tx"> the <code>AffineTransform</code> object to be
		''' concatenated with this <code>AffineTransform</code> object. </param>
		''' <seealso cref= #preConcatenate
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub concatenate(ByVal Tx As AffineTransform)
			Dim M0, M1 As Double
			Dim T00, T01, T10, T11 As Double
			Dim T02, T12 As Double
			Dim mystate As Integer = state
			Dim txstate As Integer = Tx.state
			Select Case (txstate << HI_SHIFT) Or mystate

				' ---------- Tx == IDENTITY cases ---------- 
			Case (HI_IDENTITY Or APPLY_IDENTITY), (HI_IDENTITY Or APPLY_TRANSLATE), (HI_IDENTITY Or APPLY_SCALE), (HI_IDENTITY Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_IDENTITY Or APPLY_SHEAR), (HI_IDENTITY Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_IDENTITY Or APPLY_SHEAR Or APPLY_SCALE), (HI_IDENTITY Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				Return

				' ---------- this == IDENTITY cases ---------- 
			Case (HI_SHEAR Or HI_SCALE Or HI_TRANSLATE Or APPLY_IDENTITY)
				m01 = Tx.m01
				m10 = Tx.m10
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_SCALE Or HI_TRANSLATE Or APPLY_IDENTITY)
				m00 = Tx.m00
				m11 = Tx.m11
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_TRANSLATE Or APPLY_IDENTITY)
				m02 = Tx.m02
				m12 = Tx.m12
				state = txstate
				type = Tx.type
				Return
			Case (HI_SHEAR Or HI_SCALE Or APPLY_IDENTITY)
				m01 = Tx.m01
				m10 = Tx.m10
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_SCALE Or APPLY_IDENTITY)
				m00 = Tx.m00
				m11 = Tx.m11
				state = txstate
				type = Tx.type
				Return
			Case (HI_SHEAR Or HI_TRANSLATE Or APPLY_IDENTITY)
				m02 = Tx.m02
				m12 = Tx.m12
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_SHEAR Or APPLY_IDENTITY)
				m01 = Tx.m01
				m10 = Tx.m10
					m11 = 0.0
					m00 = m11
				state = txstate
				type = Tx.type
				Return

				' ---------- Tx == TRANSLATE cases ---------- 
			Case (HI_TRANSLATE Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_TRANSLATE Or APPLY_SHEAR Or APPLY_SCALE), (HI_TRANSLATE Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_TRANSLATE Or APPLY_SHEAR), (HI_TRANSLATE Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_TRANSLATE Or APPLY_SCALE), (HI_TRANSLATE Or APPLY_TRANSLATE)
				translate(Tx.m02, Tx.m12)
				Return

				' ---------- Tx == SCALE cases ---------- 
			Case (HI_SCALE Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_SHEAR Or APPLY_SCALE), (HI_SCALE Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_SHEAR), (HI_SCALE Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_SCALE), (HI_SCALE Or APPLY_TRANSLATE)
				scale(Tx.m00, Tx.m11)
				Return

				' ---------- Tx == SHEAR cases ---------- 
			Case (HI_SHEAR Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_SHEAR Or APPLY_SCALE)
				T01 = Tx.m01
				T10 = Tx.m10
				M0 = m00
				m00 = m01 * T10
				m01 = M0 * T01
				M0 = m10
				m10 = m11 * T10
				m11 = M0 * T01
				type = TYPE_UNKNOWN
				Return
			Case (HI_SHEAR Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_SHEAR)
				m00 = m01 * Tx.m10
				m01 = 0.0
				m11 = m10 * Tx.m01
				m10 = 0.0
				state = mystate Xor (APPLY_SHEAR Or APPLY_SCALE)
				type = TYPE_UNKNOWN
				Return
			Case (HI_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_SCALE)
				m01 = m00 * Tx.m01
				m00 = 0.0
				m10 = m11 * Tx.m10
				m11 = 0.0
				state = mystate Xor (APPLY_SHEAR Or APPLY_SCALE)
				type = TYPE_UNKNOWN
				Return
			Case (HI_SHEAR Or APPLY_TRANSLATE)
				m00 = 0.0
				m01 = Tx.m01
				m10 = Tx.m10
				m11 = 0.0
				state = APPLY_TRANSLATE Or APPLY_SHEAR
				type = TYPE_UNKNOWN
				Return
			End Select
			' If Tx has more than one attribute, it is not worth optimizing
			' all of those cases...
			T00 = Tx.m00
			T01 = Tx.m01
			T02 = Tx.m02
			T10 = Tx.m10
			T11 = Tx.m11
			T12 = Tx.m12
			Select Case mystate
			Case Else
				stateError()
				' NOTREACHED 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE)
				state = mystate Or txstate
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M0 = m00
				M1 = m01
				m00 = T00 * M0 + T10 * M1
				m01 = T01 * M0 + T11 * M1
				m02 += T02 * M0 + T12 * M1

				M0 = m10
				M1 = m11
				m10 = T00 * M0 + T10 * M1
				m11 = T01 * M0 + T11 * M1
				m12 += T02 * M0 + T12 * M1
				type = TYPE_UNKNOWN
				Return

			Case (APPLY_SHEAR Or APPLY_TRANSLATE), (APPLY_SHEAR)
				M0 = m01
				m00 = T10 * M0
				m01 = T11 * M0
				m02 += T12 * M0

				M0 = m10
				m10 = T00 * M0
				m11 = T01 * M0
				m12 += T02 * M0

			Case (APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SCALE)
				M0 = m00
				m00 = T00 * M0
				m01 = T01 * M0
				m02 += T02 * M0

				M0 = m11
				m10 = T10 * M0
				m11 = T11 * M0
				m12 += T12 * M0

			Case (APPLY_TRANSLATE)
				m00 = T00
				m01 = T01
				m02 += T02

				m10 = T10
				m11 = T11
				m12 += T12
				state = txstate Or APPLY_TRANSLATE
				type = TYPE_UNKNOWN
				Return
			End Select
			updateState()
		End Sub

		''' <summary>
		''' Concatenates an <code>AffineTransform</code> <code>Tx</code> to
		''' this <code>AffineTransform</code> Cx
		''' in a less commonly used way such that <code>Tx</code> modifies the
		''' coordinate transformation relative to the absolute pixel
		''' space rather than relative to the existing user space.
		''' Cx is updated to perform the combined transformation.
		''' Transforming a point p by the updated transform Cx' is
		''' equivalent to first transforming p by the original transform
		''' Cx and then transforming the result by
		''' <code>Tx</code> like this:
		''' Cx'(p) = Tx(Cx(p))
		''' In matrix notation, if this transform Cx
		''' is represented by the matrix [this] and <code>Tx</code> is
		''' represented by the matrix [Tx] then this method does the
		''' following:
		''' <pre>
		'''          [this] = [Tx] x [this]
		''' </pre> </summary>
		''' <param name="Tx"> the <code>AffineTransform</code> object to be
		''' concatenated with this <code>AffineTransform</code> object. </param>
		''' <seealso cref= #concatenate
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub preConcatenate(ByVal Tx As AffineTransform)
			Dim M0, M1 As Double
			Dim T00, T01, T10, T11 As Double
			Dim T02, T12 As Double
			Dim mystate As Integer = state
			Dim txstate As Integer = Tx.state
			Select Case (txstate << HI_SHIFT) Or mystate
			Case (HI_IDENTITY Or APPLY_IDENTITY), (HI_IDENTITY Or APPLY_TRANSLATE), (HI_IDENTITY Or APPLY_SCALE), (HI_IDENTITY Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_IDENTITY Or APPLY_SHEAR), (HI_IDENTITY Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_IDENTITY Or APPLY_SHEAR Or APPLY_SCALE), (HI_IDENTITY Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				' Tx is IDENTITY...
				Return

			Case (HI_TRANSLATE Or APPLY_IDENTITY), (HI_TRANSLATE Or APPLY_SCALE), (HI_TRANSLATE Or APPLY_SHEAR), (HI_TRANSLATE Or APPLY_SHEAR Or APPLY_SCALE)
				' Tx is TRANSLATE, this has no TRANSLATE
				m02 = Tx.m02
				m12 = Tx.m12
				state = mystate Or APPLY_TRANSLATE
				type = type Or TYPE_TRANSLATION
				Return

			Case (HI_TRANSLATE Or APPLY_TRANSLATE), (HI_TRANSLATE Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_TRANSLATE Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_TRANSLATE Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				' Tx is TRANSLATE, this has one too
				m02 = m02 + Tx.m02
				m12 = m12 + Tx.m12
				Return

			Case (HI_SCALE Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_IDENTITY)
				' Only these two existing states need a new state
				state = mystate Or APPLY_SCALE
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_SCALE Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_SHEAR Or APPLY_SCALE), (HI_SCALE Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_SHEAR), (HI_SCALE Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SCALE Or APPLY_SCALE)
				' Tx is SCALE, this is anything
				T00 = Tx.m00
				T11 = Tx.m11
				If (mystate And APPLY_SHEAR) <> 0 Then
					m01 = m01 * T00
					m10 = m10 * T11
					If (mystate And APPLY_SCALE) <> 0 Then
						m00 = m00 * T00
						m11 = m11 * T11
					End If
				Else
					m00 = m00 * T00
					m11 = m11 * T11
				End If
				If (mystate And APPLY_TRANSLATE) <> 0 Then
					m02 = m02 * T00
					m12 = m12 * T11
				End If
				type = TYPE_UNKNOWN
				Return
			Case (HI_SHEAR Or APPLY_SHEAR Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_SHEAR)
				mystate = mystate Or APPLY_SCALE
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_SHEAR Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_IDENTITY), (HI_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_SCALE)
				state = mystate Xor APPLY_SHEAR
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (HI_SHEAR Or APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (HI_SHEAR Or APPLY_SHEAR Or APPLY_SCALE)
				' Tx is SHEAR, this is anything
				T01 = Tx.m01
				T10 = Tx.m10

				M0 = m00
				m00 = m10 * T01
				m10 = M0 * T10

				M0 = m01
				m01 = m11 * T01
				m11 = M0 * T10

				M0 = m02
				m02 = m12 * T01
				m12 = M0 * T10
				type = TYPE_UNKNOWN
				Return
			End Select
			' If Tx has more than one attribute, it is not worth optimizing
			' all of those cases...
			T00 = Tx.m00
			T01 = Tx.m01
			T02 = Tx.m02
			T10 = Tx.m10
			T11 = Tx.m11
			T12 = Tx.m12
			Select Case mystate
			Case Else
				stateError()
				' NOTREACHED 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M0 = m02
				M1 = m12
				T02 += M0 * T00 + M1 * T01
				T12 += M0 * T10 + M1 * T11

				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE)
				m02 = T02
				m12 = T12

				M0 = m00
				M1 = m10
				m00 = M0 * T00 + M1 * T01
				m10 = M0 * T10 + M1 * T11

				M0 = m01
				M1 = m11
				m01 = M0 * T00 + M1 * T01
				m11 = M0 * T10 + M1 * T11

			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M0 = m02
				M1 = m12
				T02 += M0 * T00 + M1 * T01
				T12 += M0 * T10 + M1 * T11

				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR)
				m02 = T02
				m12 = T12

				M0 = m10
				m00 = M0 * T01
				m10 = M0 * T11

				M0 = m01
				m01 = M0 * T00
				m11 = M0 * T10

			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M0 = m02
				M1 = m12
				T02 += M0 * T00 + M1 * T01
				T12 += M0 * T10 + M1 * T11

				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SCALE)
				m02 = T02
				m12 = T12

				M0 = m00
				m00 = M0 * T00
				m10 = M0 * T10

				M0 = m11
				m01 = M0 * T01
				m11 = M0 * T11

			Case (APPLY_TRANSLATE)
				M0 = m02
				M1 = m12
				T02 += M0 * T00 + M1 * T01
				T12 += M0 * T10 + M1 * T11

				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_IDENTITY)
				m02 = T02
				m12 = T12

				m00 = T00
				m10 = T10

				m01 = T01
				m11 = T11

				state = mystate Or txstate
				type = TYPE_UNKNOWN
				Return
			End Select
			updateState()
		End Sub

		''' <summary>
		''' Returns an <code>AffineTransform</code> object representing the
		''' inverse transformation.
		''' The inverse transform Tx' of this transform Tx
		''' maps coordinates transformed by Tx back
		''' to their original coordinates.
		''' In other words, Tx'(Tx(p)) = p = Tx(Tx'(p)).
		''' <p>
		''' If this transform maps all coordinates onto a point or a line
		''' then it will not have an inverse, since coordinates that do
		''' not lie on the destination point or line will not have an inverse
		''' mapping.
		''' The <code>getDeterminant</code> method can be used to determine if this
		''' transform has no inverse, in which case an exception will be
		''' thrown if the <code>createInverse</code> method is called. </summary>
		''' <returns> a new <code>AffineTransform</code> object representing the
		''' inverse transformation. </returns>
		''' <seealso cref= #getDeterminant </seealso>
		''' <exception cref="NoninvertibleTransformException">
		''' if the matrix cannot be inverted.
		''' @since 1.2 </exception>
		Public Overridable Function createInverse() As AffineTransform
			Dim det As Double
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return Nothing
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				det = m00 * m11 - m01 * m10
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				Return New AffineTransform(m11 / det, -m10 / det, -m01 / det, m00 / det, (m01 * m12 - m11 * m02) / det, (m10 * m02 - m00 * m12) / det, (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE))
			Case (APPLY_SHEAR Or APPLY_SCALE)
				det = m00 * m11 - m01 * m10
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				Return New AffineTransform(m11 / det, -m10 / det, -m01 / det, m00 / det, 0.0, 0.0, (APPLY_SHEAR Or APPLY_SCALE))
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				If m01 = 0.0 OrElse m10 = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				Return New AffineTransform(0.0, 1.0 / m01, 1.0 / m10, 0.0, -m12 / m10, -m02 / m01, (APPLY_SHEAR Or APPLY_TRANSLATE))
			Case (APPLY_SHEAR)
				If m01 = 0.0 OrElse m10 = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				Return New AffineTransform(0.0, 1.0 / m01, 1.0 / m10, 0.0, 0.0, 0.0, (APPLY_SHEAR))
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				If m00 = 0.0 OrElse m11 = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				Return New AffineTransform(1.0 / m00, 0.0, 0.0, 1.0 / m11, -m02 / m00, -m12 / m11, (APPLY_SCALE Or APPLY_TRANSLATE))
			Case (APPLY_SCALE)
				If m00 = 0.0 OrElse m11 = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				Return New AffineTransform(1.0 / m00, 0.0, 0.0, 1.0 / m11, 0.0, 0.0, (APPLY_SCALE))
			Case (APPLY_TRANSLATE)
				Return New AffineTransform(1.0, 0.0, 0.0, 1.0, -m02, -m12, (APPLY_TRANSLATE))
			Case (APPLY_IDENTITY)
				Return New AffineTransform
			End Select

			' NOTREACHED 
		End Function

		''' <summary>
		''' Sets this transform to the inverse of itself.
		''' The inverse transform Tx' of this transform Tx
		''' maps coordinates transformed by Tx back
		''' to their original coordinates.
		''' In other words, Tx'(Tx(p)) = p = Tx(Tx'(p)).
		''' <p>
		''' If this transform maps all coordinates onto a point or a line
		''' then it will not have an inverse, since coordinates that do
		''' not lie on the destination point or line will not have an inverse
		''' mapping.
		''' The <code>getDeterminant</code> method can be used to determine if this
		''' transform has no inverse, in which case an exception will be
		''' thrown if the <code>invert</code> method is called. </summary>
		''' <seealso cref= #getDeterminant </seealso>
		''' <exception cref="NoninvertibleTransformException">
		''' if the matrix cannot be inverted.
		''' @since 1.6 </exception>
		Public Overridable Sub invert()
			Dim M00_Renamed, M01_Renamed, M02_Renamed As Double
			Dim M10_Renamed, M11_Renamed, M12_Renamed As Double
			Dim det As Double
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M11_Renamed = m11
				M12_Renamed = m12
				det = M00_Renamed * M11_Renamed - M01_Renamed * M10_Renamed
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				m00 = M11_Renamed / det
				m10 = -M10_Renamed / det
				m01 = -M01_Renamed / det
				m11 = M00_Renamed / det
				m02 = (M01_Renamed * M12_Renamed - M11_Renamed * M02_Renamed) / det
				m12 = (M10_Renamed * M02_Renamed - M00_Renamed * M12_Renamed) / det
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				det = M00_Renamed * M11_Renamed - M01_Renamed * M10_Renamed
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				m00 = M11_Renamed / det
				m10 = -M10_Renamed / det
				m01 = -M01_Renamed / det
				m11 = M00_Renamed / det
				' m02 = 0.0;
				' m12 = 0.0;
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M12_Renamed = m12
				If M01_Renamed = 0.0 OrElse M10_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				' m00 = 0.0;
				m10 = 1.0 / M01_Renamed
				m01 = 1.0 / M10_Renamed
				' m11 = 0.0;
				m02 = -M12_Renamed / M10_Renamed
				m12 = -M02_Renamed / M01_Renamed
			Case (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				If M01_Renamed = 0.0 OrElse M10_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				' m00 = 0.0;
				m10 = 1.0 / M01_Renamed
				m01 = 1.0 / M10_Renamed
				' m11 = 0.0;
				' m02 = 0.0;
				' m12 = 0.0;
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M02_Renamed = m02
				M11_Renamed = m11
				M12_Renamed = m12
				If M00_Renamed = 0.0 OrElse M11_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				m00 = 1.0 / M00_Renamed
				' m10 = 0.0;
				' m01 = 0.0;
				m11 = 1.0 / M11_Renamed
				m02 = -M02_Renamed / M00_Renamed
				m12 = -M12_Renamed / M11_Renamed
			Case (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				If M00_Renamed = 0.0 OrElse M11_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				m00 = 1.0 / M00_Renamed
				' m10 = 0.0;
				' m01 = 0.0;
				m11 = 1.0 / M11_Renamed
				' m02 = 0.0;
				' m12 = 0.0;
			Case (APPLY_TRANSLATE)
				' m00 = 1.0;
				' m10 = 0.0;
				' m01 = 0.0;
				' m11 = 1.0;
				m02 = -m02
				m12 = -m12
			Case (APPLY_IDENTITY)
				' m00 = 1.0;
				' m10 = 0.0;
				' m01 = 0.0;
				' m11 = 1.0;
				' m02 = 0.0;
				' m12 = 0.0;
			End Select
		End Sub

		''' <summary>
		''' Transforms the specified <code>ptSrc</code> and stores the result
		''' in <code>ptDst</code>.
		''' If <code>ptDst</code> is <code>null</code>, a new <seealso cref="Point2D"/>
		''' object is allocated and then the result of the transformation is
		''' stored in this object.
		''' In either case, <code>ptDst</code>, which contains the
		''' transformed point, is returned for convenience.
		''' If <code>ptSrc</code> and <code>ptDst</code> are the same
		''' object, the input point is correctly overwritten with
		''' the transformed point. </summary>
		''' <param name="ptSrc"> the specified <code>Point2D</code> to be transformed </param>
		''' <param name="ptDst"> the specified <code>Point2D</code> that stores the
		''' result of transforming <code>ptSrc</code> </param>
		''' <returns> the <code>ptDst</code> after transforming
		''' <code>ptSrc</code> and storing the result in <code>ptDst</code>.
		''' @since 1.2 </returns>
		Public Overridable Function transform(ByVal ptSrc As Point2D, ByVal ptDst As Point2D) As Point2D
			If ptDst Is Nothing Then
				If TypeOf ptSrc Is Point2D.Double Then
					ptDst = New Point2D.Double
				Else
					ptDst = New Point2D.Float
				End If
			End If
			' Copy source coords into local variables in case src == dst
			Dim x As Double = ptSrc.x
			Dim y As Double = ptSrc.y
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return Nothing
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				ptDst.locationion(x * m00 + y * m01 + m02, x * m10 + y * m11 + m12)
				Return ptDst
			Case (APPLY_SHEAR Or APPLY_SCALE)
				ptDst.locationion(x * m00 + y * m01, x * m10 + y * m11)
				Return ptDst
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				ptDst.locationion(y * m01 + m02, x * m10 + m12)
				Return ptDst
			Case (APPLY_SHEAR)
				ptDst.locationion(y * m01, x * m10)
				Return ptDst
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				ptDst.locationion(x * m00 + m02, y * m11 + m12)
				Return ptDst
			Case (APPLY_SCALE)
				ptDst.locationion(x * m00, y * m11)
				Return ptDst
			Case (APPLY_TRANSLATE)
				ptDst.locationion(x + m02, y + m12)
				Return ptDst
			Case (APPLY_IDENTITY)
				ptDst.locationion(x, y)
				Return ptDst
			End Select

			' NOTREACHED 
		End Function

		''' <summary>
		''' Transforms an array of point objects by this transform.
		''' If any element of the <code>ptDst</code> array is
		''' <code>null</code>, a new <code>Point2D</code> object is allocated
		''' and stored into that element before storing the results of the
		''' transformation.
		''' <p>
		''' Note that this method does not take any precautions to
		''' avoid problems caused by storing results into <code>Point2D</code>
		''' objects that will be used as the source for calculations
		''' further down the source array.
		''' This method does guarantee that if a specified <code>Point2D</code>
		''' object is both the source and destination for the same single point
		''' transform operation then the results will not be stored until
		''' the calculations are complete to avoid storing the results on
		''' top of the operands.
		''' If, however, the destination <code>Point2D</code> object for one
		''' operation is the same object as the source <code>Point2D</code>
		''' object for another operation further down the source array then
		''' the original coordinates in that point are overwritten before
		''' they can be converted. </summary>
		''' <param name="ptSrc"> the array containing the source point objects </param>
		''' <param name="ptDst"> the array into which the transform point objects are
		''' returned </param>
		''' <param name="srcOff"> the offset to the first point object to be
		''' transformed in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed point object that is stored in the destination array </param>
		''' <param name="numPts"> the number of point objects to be transformed
		''' @since 1.2 </param>
		Public Overridable Sub transform(ByVal ptSrc As Point2D(), ByVal srcOff As Integer, ByVal ptDst As Point2D(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim state As Integer = Me.state
			numPts -= 1
			Do While numPts >= 0
				' Copy source coords into local variables in case src == dst
				Dim src As Point2D = ptSrc(srcOff)
				srcOff += 1
				Dim x As Double = src.x
				Dim y As Double = src.y
				Dim dst As Point2D = ptDst(dstOff)
				dstOff += 1
				If dst Is Nothing Then
					If TypeOf src Is Point2D.Double Then
						dst = New Point2D.Double
					Else
						dst = New Point2D.Float
					End If
					ptDst(dstOff - 1) = dst
				End If
				Select Case state
				Case Else
					stateError()
					' NOTREACHED 
					Return
				Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
					dst.locationion(x * m00 + y * m01 + m02, x * m10 + y * m11 + m12)
				Case (APPLY_SHEAR Or APPLY_SCALE)
					dst.locationion(x * m00 + y * m01, x * m10 + y * m11)
				Case (APPLY_SHEAR Or APPLY_TRANSLATE)
					dst.locationion(y * m01 + m02, x * m10 + m12)
				Case (APPLY_SHEAR)
					dst.locationion(y * m01, x * m10)
				Case (APPLY_SCALE Or APPLY_TRANSLATE)
					dst.locationion(x * m00 + m02, y * m11 + m12)
				Case (APPLY_SCALE)
					dst.locationion(x * m00, y * m11)
				Case (APPLY_TRANSLATE)
					dst.locationion(x + m02, y + m12)
				Case (APPLY_IDENTITY)
					dst.locationion(x, y)
				End Select
				numPts -= 1
			Loop

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Transforms an array of floating point coordinates by this transform.
		''' The two coordinate array sections can be exactly the same or
		''' can be overlapping sections of the same array without affecting the
		''' validity of the results.
		''' This method ensures that no source coordinates are overwritten by a
		''' previous operation before they can be transformed.
		''' The coordinates are stored in the arrays starting at the specified
		''' offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		''' <param name="srcPts"> the array containing the source point coordinates.
		''' Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		''' <param name="dstPts"> the array into which the transformed point coordinates
		''' are returned.  Each point is stored as a pair of x,&nbsp;y
		''' coordinates. </param>
		''' <param name="srcOff"> the offset to the first point to be transformed
		''' in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed point that is stored in the destination array </param>
		''' <param name="numPts"> the number of points to be transformed
		''' @since 1.2 </param>
		Public Overridable Sub transform(ByVal srcPts As Single(), ByVal srcOff As Integer, ByVal dstPts As Single(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim M00_Renamed, M01_Renamed, M02_Renamed, M10_Renamed, M11_Renamed, M12_Renamed As Double ' For caching
			If dstPts = srcPts AndAlso dstOff > srcOff AndAlso dstOff < srcOff + numPts * 2 Then
				' If the arrays overlap partially with the destination higher
				' than the source and we transform the coordinates normally
				' we would overwrite some of the later source coordinates
				' with results of previous transformations.
				' To get around this we use arraycopy to copy the points
				' to their final destination with correct overwrite
				' handling and then transform them in place in the new
				' safer location.
				Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				' srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff
			End If
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M00_Renamed * x + M01_Renamed * y + M02_Renamed)
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x + M11_Renamed * y + M12_Renamed)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M00_Renamed * x + M01_Renamed * y)
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x + M11_Renamed * y)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M01_Renamed * srcPts(srcOff) + M02_Renamed)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x + M12_Renamed)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M01_Renamed * srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M02_Renamed = m02
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(M00_Renamed * srcPts(srcOff) + M02_Renamed)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M11_Renamed * srcPts(srcOff) + M12_Renamed)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(M00_Renamed * srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M11_Renamed * srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_TRANSLATE)
				M02_Renamed = m02
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(srcPts(srcOff) + M02_Renamed)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(srcPts(srcOff) + M12_Renamed)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_IDENTITY)
				If srcPts <> dstPts OrElse srcOff <> dstOff Then Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				Return
			End Select

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Transforms an array of double precision coordinates by this transform.
		''' The two coordinate array sections can be exactly the same or
		''' can be overlapping sections of the same array without affecting the
		''' validity of the results.
		''' This method ensures that no source coordinates are
		''' overwritten by a previous operation before they can be transformed.
		''' The coordinates are stored in the arrays starting at the indicated
		''' offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		''' <param name="srcPts"> the array containing the source point coordinates.
		''' Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		''' <param name="dstPts"> the array into which the transformed point
		''' coordinates are returned.  Each point is stored as a pair of
		''' x,&nbsp;y coordinates. </param>
		''' <param name="srcOff"> the offset to the first point to be transformed
		''' in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed point that is stored in the destination array </param>
		''' <param name="numPts"> the number of point objects to be transformed
		''' @since 1.2 </param>
		Public Overridable Sub transform(ByVal srcPts As Double(), ByVal srcOff As Integer, ByVal dstPts As Double(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim M00_Renamed, M01_Renamed, M02_Renamed, M10_Renamed, M11_Renamed, M12_Renamed As Double ' For caching
			If dstPts = srcPts AndAlso dstOff > srcOff AndAlso dstOff < srcOff + numPts * 2 Then
				' If the arrays overlap partially with the destination higher
				' than the source and we transform the coordinates normally
				' we would overwrite some of the later source coordinates
				' with results of previous transformations.
				' To get around this we use arraycopy to copy the points
				' to their final destination with correct overwrite
				' handling and then transform them in place in the new
				' safer location.
				Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				' srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff
			End If
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M00_Renamed * x + M01_Renamed * y + M02_Renamed
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x + M11_Renamed * y + M12_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M00_Renamed * x + M01_Renamed * y
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x + M11_Renamed * y
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M01_Renamed * srcPts(srcOff) + M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x + M12_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M01_Renamed * srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M02_Renamed = m02
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = M00_Renamed * srcPts(srcOff) + M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M11_Renamed * srcPts(srcOff) + M12_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = M00_Renamed * srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M11_Renamed * srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_TRANSLATE)
				M02_Renamed = m02
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = srcPts(srcOff) + M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = srcPts(srcOff) + M12_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_IDENTITY)
				If srcPts <> dstPts OrElse srcOff <> dstOff Then Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				Return
			End Select

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Transforms an array of floating point coordinates by this transform
		''' and stores the results into an array of doubles.
		''' The coordinates are stored in the arrays starting at the specified
		''' offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		''' <param name="srcPts"> the array containing the source point coordinates.
		''' Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		''' <param name="dstPts"> the array into which the transformed point coordinates
		''' are returned.  Each point is stored as a pair of x,&nbsp;y
		''' coordinates. </param>
		''' <param name="srcOff"> the offset to the first point to be transformed
		''' in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed point that is stored in the destination array </param>
		''' <param name="numPts"> the number of points to be transformed
		''' @since 1.2 </param>
		Public Overridable Sub transform(ByVal srcPts As Single(), ByVal srcOff As Integer, ByVal dstPts As Double(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim M00_Renamed, M01_Renamed, M02_Renamed, M10_Renamed, M11_Renamed, M12_Renamed As Double ' For caching
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M00_Renamed * x + M01_Renamed * y + M02_Renamed
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x + M11_Renamed * y + M12_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M00_Renamed * x + M01_Renamed * y
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x + M11_Renamed * y
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M01_Renamed * srcPts(srcOff) + M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x + M12_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = M01_Renamed * srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M10_Renamed * x
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M02_Renamed = m02
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = M00_Renamed * srcPts(srcOff) + M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M11_Renamed * srcPts(srcOff) + M12_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = M00_Renamed * srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = M11_Renamed * srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_TRANSLATE)
				M02_Renamed = m02
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = srcPts(srcOff) + M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = srcPts(srcOff) + M12_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_IDENTITY)
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = srcPts(srcOff)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			End Select

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Transforms an array of double precision coordinates by this transform
		''' and stores the results into an array of floats.
		''' The coordinates are stored in the arrays starting at the specified
		''' offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		''' <param name="srcPts"> the array containing the source point coordinates.
		''' Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		''' <param name="dstPts"> the array into which the transformed point
		''' coordinates are returned.  Each point is stored as a pair of
		''' x,&nbsp;y coordinates. </param>
		''' <param name="srcOff"> the offset to the first point to be transformed
		''' in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed point that is stored in the destination array </param>
		''' <param name="numPts"> the number of point objects to be transformed
		''' @since 1.2 </param>
		Public Overridable Sub transform(ByVal srcPts As Double(), ByVal srcOff As Integer, ByVal dstPts As Single(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim M00_Renamed, M01_Renamed, M02_Renamed, M10_Renamed, M11_Renamed, M12_Renamed As Double ' For caching
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M00_Renamed * x + M01_Renamed * y + M02_Renamed)
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x + M11_Renamed * y + M12_Renamed)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M00_Renamed * x + M01_Renamed * y)
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x + M11_Renamed * y)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M01_Renamed * srcPts(srcOff) + M02_Renamed)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x + M12_Renamed)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = CSng(M01_Renamed * srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M10_Renamed * x)
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M02_Renamed = m02
				M11_Renamed = m11
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(M00_Renamed * srcPts(srcOff) + M02_Renamed)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M11_Renamed * srcPts(srcOff) + M12_Renamed)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(M00_Renamed * srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(M11_Renamed * srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_TRANSLATE)
				M02_Renamed = m02
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(srcPts(srcOff) + M02_Renamed)
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(srcPts(srcOff) + M12_Renamed)
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_IDENTITY)
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = CSng(srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = CSng(srcPts(srcOff))
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			End Select

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Inverse transforms the specified <code>ptSrc</code> and stores the
		''' result in <code>ptDst</code>.
		''' If <code>ptDst</code> is <code>null</code>, a new
		''' <code>Point2D</code> object is allocated and then the result of the
		''' transform is stored in this object.
		''' In either case, <code>ptDst</code>, which contains the transformed
		''' point, is returned for convenience.
		''' If <code>ptSrc</code> and <code>ptDst</code> are the same
		''' object, the input point is correctly overwritten with the
		''' transformed point. </summary>
		''' <param name="ptSrc"> the point to be inverse transformed </param>
		''' <param name="ptDst"> the resulting transformed point </param>
		''' <returns> <code>ptDst</code>, which contains the result of the
		''' inverse transform. </returns>
		''' <exception cref="NoninvertibleTransformException">  if the matrix cannot be
		'''                                         inverted.
		''' @since 1.2 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function inverseTransform(ByVal ptSrc As Point2D, ByVal ptDst As Point2D) As Point2D
			If ptDst Is Nothing Then
				If TypeOf ptSrc Is Point2D.Double Then
					ptDst = New Point2D.Double
				Else
					ptDst = New Point2D.Float
				End If
			End If
			' Copy source coords into local variables in case src == dst
			Dim x As Double = ptSrc.x
			Dim y As Double = ptSrc.y
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				x -= m02
				y -= m12
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR Or APPLY_SCALE)
				Dim det As Double = m00 * m11 - m01 * m10
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				ptDst.locationion((x * m11 - y * m01) / det, (y * m00 - x * m10) / det)
				Return ptDst
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				x -= m02
				y -= m12
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SHEAR)
				If m01 = 0.0 OrElse m10 = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				ptDst.locationion(y / m10, x / m01)
				Return ptDst
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				x -= m02
				y -= m12
				' NOBREAK 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case (APPLY_SCALE)
				If m00 = 0.0 OrElse m11 = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				ptDst.locationion(x / m00, y / m11)
				Return ptDst
			Case (APPLY_TRANSLATE)
				ptDst.locationion(x - m02, y - m12)
				Return ptDst
			Case (APPLY_IDENTITY)
				ptDst.locationion(x, y)
				Return ptDst
			End Select

			' NOTREACHED 
		End Function

		''' <summary>
		''' Inverse transforms an array of double precision coordinates by
		''' this transform.
		''' The two coordinate array sections can be exactly the same or
		''' can be overlapping sections of the same array without affecting the
		''' validity of the results.
		''' This method ensures that no source coordinates are
		''' overwritten by a previous operation before they can be transformed.
		''' The coordinates are stored in the arrays starting at the specified
		''' offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		''' <param name="srcPts"> the array containing the source point coordinates.
		''' Each point is stored as a pair of x,&nbsp;y coordinates. </param>
		''' <param name="dstPts"> the array into which the transformed point
		''' coordinates are returned.  Each point is stored as a pair of
		''' x,&nbsp;y coordinates. </param>
		''' <param name="srcOff"> the offset to the first point to be transformed
		''' in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed point that is stored in the destination array </param>
		''' <param name="numPts"> the number of point objects to be transformed </param>
		''' <exception cref="NoninvertibleTransformException">  if the matrix cannot be
		'''                                         inverted.
		''' @since 1.2 </exception>
		Public Overridable Sub inverseTransform(ByVal srcPts As Double(), ByVal srcOff As Integer, ByVal dstPts As Double(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim M00_Renamed, M01_Renamed, M02_Renamed, M10_Renamed, M11_Renamed, M12_Renamed As Double ' For caching
			Dim det As Double
			If dstPts = srcPts AndAlso dstOff > srcOff AndAlso dstOff < srcOff + numPts * 2 Then
				' If the arrays overlap partially with the destination higher
				' than the source and we transform the coordinates normally
				' we would overwrite some of the later source coordinates
				' with results of previous transformations.
				' To get around this we use arraycopy to copy the points
				' to their final destination with correct overwrite
				' handling and then transform them in place in the new
				' safer location.
				Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				' srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff
			End If
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M11_Renamed = m11
				M12_Renamed = m12
				det = M00_Renamed * M11_Renamed - M01_Renamed * M10_Renamed
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff) - M02_Renamed
					srcOff += 1
					Dim y As Double = srcPts(srcOff) - M12_Renamed
					srcOff += 1
					dstPts(dstOff) = (x * M11_Renamed - y * M01_Renamed) / det
					dstOff += 1
					dstPts(dstOff) = (y * M00_Renamed - x * M10_Renamed) / det
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				det = M00_Renamed * M11_Renamed - M01_Renamed * M10_Renamed
				If System.Math.Abs(det) <= java.lang.[Double].MIN_VALUE Then Throw New NoninvertibleTransformException("Determinant is " & det)
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = (x * M11_Renamed - y * M01_Renamed) / det
					dstOff += 1
					dstPts(dstOff) = (y * M00_Renamed - x * M10_Renamed) / det
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE)
				M01_Renamed = m01
				M02_Renamed = m02
				M10_Renamed = m10
				M12_Renamed = m12
				If M01_Renamed = 0.0 OrElse M10_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff) - M02_Renamed
					srcOff += 1
					dstPts(dstOff) = (srcPts(srcOff) - M12_Renamed) / M10_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = x / M01_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				If M01_Renamed = 0.0 OrElse M10_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = srcPts(srcOff) / M10_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = x / M01_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE)
				M00_Renamed = m00
				M02_Renamed = m02
				M11_Renamed = m11
				M12_Renamed = m12
				If M00_Renamed = 0.0 OrElse M11_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = (srcPts(srcOff) - M02_Renamed) / M00_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = (srcPts(srcOff) - M12_Renamed) / M11_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				If M00_Renamed = 0.0 OrElse M11_Renamed = 0.0 Then Throw New NoninvertibleTransformException("Determinant is 0")
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = srcPts(srcOff) / M00_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = srcPts(srcOff) / M11_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_TRANSLATE)
				M02_Renamed = m02
				M12_Renamed = m12
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = srcPts(srcOff) - M02_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = srcPts(srcOff) - M12_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_IDENTITY)
				If srcPts <> dstPts OrElse srcOff <> dstOff Then Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				Return
			End Select

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Transforms the relative distance vector specified by
		''' <code>ptSrc</code> and stores the result in <code>ptDst</code>.
		''' A relative distance vector is transformed without applying the
		''' translation components of the affine transformation matrix
		''' using the following equations:
		''' <pre>
		'''  [  x' ]   [  m00  m01 (m02) ] [  x  ]   [ m00x + m01y ]
		'''  [  y' ] = [  m10  m11 (m12) ] [  y  ] = [ m10x + m11y ]
		'''  [ (1) ]   [  (0)  (0) ( 1 ) ] [ (1) ]   [     (1)     ]
		''' </pre>
		''' If <code>ptDst</code> is <code>null</code>, a new
		''' <code>Point2D</code> object is allocated and then the result of the
		''' transform is stored in this object.
		''' In either case, <code>ptDst</code>, which contains the
		''' transformed point, is returned for convenience.
		''' If <code>ptSrc</code> and <code>ptDst</code> are the same object,
		''' the input point is correctly overwritten with the transformed
		''' point. </summary>
		''' <param name="ptSrc"> the distance vector to be delta transformed </param>
		''' <param name="ptDst"> the resulting transformed distance vector </param>
		''' <returns> <code>ptDst</code>, which contains the result of the
		''' transformation.
		''' @since 1.2 </returns>
		Public Overridable Function deltaTransform(ByVal ptSrc As Point2D, ByVal ptDst As Point2D) As Point2D
			If ptDst Is Nothing Then
				If TypeOf ptSrc Is Point2D.Double Then
					ptDst = New Point2D.Double
				Else
					ptDst = New Point2D.Float
				End If
			End If
			' Copy source coords into local variables in case src == dst
			Dim x As Double = ptSrc.x
			Dim y As Double = ptSrc.y
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return Nothing
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SHEAR Or APPLY_SCALE)
				ptDst.locationion(x * m00 + y * m01, x * m10 + y * m11)
				Return ptDst
			Case (APPLY_SHEAR Or APPLY_TRANSLATE), (APPLY_SHEAR)
				ptDst.locationion(y * m01, x * m10)
				Return ptDst
			Case (APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SCALE)
				ptDst.locationion(x * m00, y * m11)
				Return ptDst
			Case (APPLY_TRANSLATE), (APPLY_IDENTITY)
				ptDst.locationion(x, y)
				Return ptDst
			End Select

			' NOTREACHED 
		End Function

		''' <summary>
		''' Transforms an array of relative distance vectors by this
		''' transform.
		''' A relative distance vector is transformed without applying the
		''' translation components of the affine transformation matrix
		''' using the following equations:
		''' <pre>
		'''  [  x' ]   [  m00  m01 (m02) ] [  x  ]   [ m00x + m01y ]
		'''  [  y' ] = [  m10  m11 (m12) ] [  y  ] = [ m10x + m11y ]
		'''  [ (1) ]   [  (0)  (0) ( 1 ) ] [ (1) ]   [     (1)     ]
		''' </pre>
		''' The two coordinate array sections can be exactly the same or
		''' can be overlapping sections of the same array without affecting the
		''' validity of the results.
		''' This method ensures that no source coordinates are
		''' overwritten by a previous operation before they can be transformed.
		''' The coordinates are stored in the arrays starting at the indicated
		''' offset in the order <code>[x0, y0, x1, y1, ..., xn, yn]</code>. </summary>
		''' <param name="srcPts"> the array containing the source distance vectors.
		''' Each vector is stored as a pair of relative x,&nbsp;y coordinates. </param>
		''' <param name="dstPts"> the array into which the transformed distance vectors
		''' are returned.  Each vector is stored as a pair of relative
		''' x,&nbsp;y coordinates. </param>
		''' <param name="srcOff"> the offset to the first vector to be transformed
		''' in the source array </param>
		''' <param name="dstOff"> the offset to the location of the first
		''' transformed vector that is stored in the destination array </param>
		''' <param name="numPts"> the number of vector coordinate pairs to be
		''' transformed
		''' @since 1.2 </param>
		Public Overridable Sub deltaTransform(ByVal srcPts As Double(), ByVal srcOff As Integer, ByVal dstPts As Double(), ByVal dstOff As Integer, ByVal numPts As Integer)
			Dim M00_Renamed, M01_Renamed, M10_Renamed, M11_Renamed As Double ' For caching
			If dstPts = srcPts AndAlso dstOff > srcOff AndAlso dstOff < srcOff + numPts * 2 Then
				' If the arrays overlap partially with the destination higher
				' than the source and we transform the coordinates normally
				' we would overwrite some of the later source coordinates
				' with results of previous transformations.
				' To get around this we use arraycopy to copy the points
				' to their final destination with correct overwrite
				' handling and then transform them in place in the new
				' safer location.
				Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				' srcPts = dstPts;         // They are known to be equal.
				srcOff = dstOff
			End If
			Select Case state
			Case Else
				stateError()
				' NOTREACHED 
				Return
			Case (APPLY_SHEAR Or APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SHEAR Or APPLY_SCALE)
				M00_Renamed = m00
				M01_Renamed = m01
				M10_Renamed = m10
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					Dim y As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = x * M00_Renamed + y * M01_Renamed
					dstOff += 1
					dstPts(dstOff) = x * M10_Renamed + y * M11_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SHEAR Or APPLY_TRANSLATE), (APPLY_SHEAR)
				M01_Renamed = m01
				M10_Renamed = m10
				numPts -= 1
				Do While numPts >= 0
					Dim x As Double = srcPts(srcOff)
					srcOff += 1
					dstPts(dstOff) = srcPts(srcOff) * M01_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = x * M10_Renamed
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_SCALE Or APPLY_TRANSLATE), (APPLY_SCALE)
				M00_Renamed = m00
				M11_Renamed = m11
				numPts -= 1
				Do While numPts >= 0
					dstPts(dstOff) = srcPts(srcOff) * M00_Renamed
					srcOff += 1
					dstOff += 1
					dstPts(dstOff) = srcPts(srcOff) * M11_Renamed
					srcOff += 1
					dstOff += 1
					numPts -= 1
				Loop
				Return
			Case (APPLY_TRANSLATE), (APPLY_IDENTITY)
				If srcPts <> dstPts OrElse srcOff <> dstOff Then Array.Copy(srcPts, srcOff, dstPts, dstOff, numPts * 2)
				Return
			End Select

			' NOTREACHED 
		End Sub

		''' <summary>
		''' Returns a new <seealso cref="Shape"/> object defined by the geometry of the
		''' specified <code>Shape</code> after it has been transformed by
		''' this transform. </summary>
		''' <param name="pSrc"> the specified <code>Shape</code> object to be
		''' transformed by this transform. </param>
		''' <returns> a new <code>Shape</code> object that defines the geometry
		''' of the transformed <code>Shape</code>, or null if {@code pSrc} is null.
		''' @since 1.2 </returns>
		Public Overridable Function createTransformedShape(ByVal pSrc As java.awt.Shape) As java.awt.Shape
			If pSrc Is Nothing Then Return Nothing
			Return New Path2D.Double(pSrc, Me)
		End Function

		' Round values to sane precision for printing
		' Note that System.Math.sin (System.Math.PI) has an error of about 10^-16
		Private Shared Function _matround(ByVal matval As Double) As Double
			Return System.Math.rint(matval * 1E15) / 1E15
		End Function

		''' <summary>
		''' Returns a <code>String</code> that represents the value of this
		''' <seealso cref="Object"/>. </summary>
		''' <returns> a <code>String</code> representing the value of this
		''' <code>Object</code>.
		''' @since 1.2 </returns>
		Public Overrides Function ToString() As String
			Return ("AffineTransform[[" & _matround(m00) & ", " & _matround(m01) & ", " & _matround(m02) & "], [" & _matround(m10) & ", " & _matround(m11) & ", " & _matround(m12) & "]]")
		End Function

		''' <summary>
		''' Returns <code>true</code> if this <code>AffineTransform</code> is
		''' an identity transform. </summary>
		''' <returns> <code>true</code> if this <code>AffineTransform</code> is
		''' an identity transform; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overridable Property identity As Boolean
			Get
				Return (state = APPLY_IDENTITY OrElse (type = TYPE_IDENTITY))
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this <code>AffineTransform</code> object. </summary>
		''' <returns> an <code>Object</code> that is a copy of this
		''' <code>AffineTransform</code> object.
		''' @since 1.2 </returns>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Returns the hashcode for this transform. </summary>
		''' <returns>      a hash code for this transform.
		''' @since 1.2 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim bits As Long = java.lang.[Double].doubleToLongBits(m00)
			bits = bits * 31 + java.lang.[Double].doubleToLongBits(m01)
			bits = bits * 31 + java.lang.[Double].doubleToLongBits(m02)
			bits = bits * 31 + java.lang.[Double].doubleToLongBits(m10)
			bits = bits * 31 + java.lang.[Double].doubleToLongBits(m11)
			bits = bits * 31 + java.lang.[Double].doubleToLongBits(m12)
			Return ((CInt(bits)) Xor (CInt(Fix(bits >> 32))))
		End Function

		''' <summary>
		''' Returns <code>true</code> if this <code>AffineTransform</code>
		''' represents the same affine coordinate transform as the specified
		''' argument. </summary>
		''' <param name="obj"> the <code>Object</code> to test for equality with this
		''' <code>AffineTransform</code> </param>
		''' <returns> <code>true</code> if <code>obj</code> equals this
		''' <code>AffineTransform</code> object; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is AffineTransform) Then Return False

			Dim a As AffineTransform = CType(obj, AffineTransform)

			Return ((m00 = a.m00) AndAlso (m01 = a.m01) AndAlso (m02 = a.m02) AndAlso (m10 = a.m10) AndAlso (m11 = a.m11) AndAlso (m12 = a.m12))
		End Function

	'     Serialization support.  A readObject method is neccessary because
	'     * the state field is part of the implementation of this particular
	'     * AffineTransform and not part of the public specification.  The
	'     * state variable's value needs to be recalculated on the fly by the
	'     * readObject method as it is in the 6-argument matrix constructor.
	'     

	'    
	'     * JDK 1.2 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 1330973210523860834L

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			updateState()
		End Sub
	End Class

End Namespace
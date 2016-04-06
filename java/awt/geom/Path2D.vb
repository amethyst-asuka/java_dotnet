Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics

'
' * Copyright (c) 2006, 2015, Oracle and/or its affiliates. All rights reserved.
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
    ''' The {@code Path2D} class provides a simple, yet flexible
    ''' shape which represents an arbitrary geometric path.
    ''' It can fully represent any path which can be iterated by the
    ''' <seealso cref="PathIterator"/> interface including all of its segment
    ''' types and winding rules and it implements all of the
    ''' basic hit testing methods of the <seealso cref="Shape"/> interface.
    ''' <p>
    ''' Use <seealso cref="Path2D.Float"/> when dealing with data that can be represented
    ''' and used with floating point precision.  Use <seealso cref="Path2D.Double"/>
    ''' for data that requires the accuracy or range of double precision.
    ''' <p>
    ''' {@code Path2D} provides exactly those facilities required for
    ''' basic construction and management of a geometric path and
    ''' implementation of the above interfaces with little added
    ''' interpretation.
    ''' If it is useful to manipulate the interiors of closed
    ''' geometric shapes beyond simple hit testing then the
    ''' <seealso cref="Area"/> class provides additional capabilities
    ''' specifically targeted at closed figures.
    ''' While both classes nominally implement the {@code Shape}
    ''' interface, they differ in purpose and together they provide
    ''' two useful views of a geometric shape where {@code Path2D}
    ''' deals primarily with a trajectory formed by path segments
    ''' and {@code Area} deals more with interpretation and manipulation
    ''' of enclosed regions of 2D geometric space.
    ''' <p>
    ''' The <seealso cref="PathIterator"/> interface has more detailed descriptions
    ''' of the types of segments that make up a path and the winding rules
    ''' that control how to determine which regions are inside or outside
    ''' the path.
    ''' 
    ''' @author Jim Graham
    ''' @since 1.6
    ''' </summary>
    Public MustInherit Class Path2D : Inherits java.lang.Object
        Implements java.awt.Shape, Cloneable

		''' <summary>
		''' An even-odd winding rule for determining the interior of
		''' a path.
		''' </summary>
		''' <seealso cref= PathIterator#WIND_EVEN_ODD
		''' @since 1.6 </seealso>
		Public Shared ReadOnly WIND_EVEN_ODD As Integer = PathIterator.WIND_EVEN_ODD

		''' <summary>
		''' A non-zero winding rule for determining the interior of a
		''' path.
		''' </summary>
		''' <seealso cref= PathIterator#WIND_NON_ZERO
		''' @since 1.6 </seealso>
		Public Shared ReadOnly WIND_NON_ZERO As Integer = PathIterator.WIND_NON_ZERO

		' For code simplicity, copy these constants to our namespace
		' and cast them to byte constants for easy storage.
		Private Shared ReadOnly SEG_MOVETO As SByte = CByte(PathIterator.SEG_MOVETO)
		Private Shared ReadOnly SEG_LINETO As SByte = CByte(PathIterator.SEG_LINETO)
		Private Shared ReadOnly SEG_QUADTO As SByte = CByte(PathIterator.SEG_QUADTO)
		Private Shared ReadOnly SEG_CUBICTO As SByte = CByte(PathIterator.SEG_CUBICTO)
		Private Shared ReadOnly SEG_CLOSE As SByte = CByte(PathIterator.SEG_CLOSE)

		<NonSerialized> _
		Friend pointTypes As SByte()
		<NonSerialized> _
		Friend numTypes As Integer
		<NonSerialized> _
		Friend numCoords As Integer
		<NonSerialized> _
		Friend windingRule As Integer

		Friend Const INIT_SIZE As Integer = 20
		Friend Const EXPAND_MAX As Integer = 500
		Friend Shared ReadOnly EXPAND_MAX_COORDS As Integer = EXPAND_MAX * 2
		Friend Const EXPAND_MIN As Integer = 10 ' ensure > 6 (cubics)

		''' <summary>
		''' Constructs a new empty {@code Path2D} object.
		''' It is assumed that the package sibling subclass that is
		''' defaulting to this constructor will fill in all values.
		''' 
		''' @since 1.6
		''' </summary>
		' private protected 
		Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs a new {@code Path2D} object from the given
		''' specified initial values.
		''' This method is only intended for internal use and should
		''' not be made public if the other constructors for this class
		''' are ever exposed.
		''' </summary>
		''' <param name="rule"> the winding rule </param>
		''' <param name="initialTypes"> the size to make the initial array to
		'''                     store the path segment types
		''' @since 1.6 </param>
		' private protected 
		Friend Sub New(  rule As Integer,   initialTypes As Integer)
			windingRule = rule
			Me.pointTypes = New SByte(initialTypes - 1){}
		End Sub

		Friend MustOverride Function cloneCoordsFloat(  at As AffineTransform) As Single()
		Friend MustOverride Function cloneCoordsDouble(  at As AffineTransform) As Double()
		Friend MustOverride Sub append(  x As Single,   y As Single)
		Friend MustOverride Sub append(  x As Double,   y As Double)
		Friend MustOverride Function getPoint(  coordindex As Integer) As Point2D
		Friend MustOverride Sub needRoom(  needMove As Boolean,   newCoords As Integer)
		Friend MustOverride Function pointCrossings(  px As Double,   py As Double) As Integer
		Friend MustOverride Function rectCrossings(  rxmin As Double,   rymin As Double,   rxmax As Double,   rymax As Double) As Integer

		Friend Shared Function expandPointTypes(  oldPointTypes As SByte(),   needed As Integer) As SByte()
			Dim oldSize As Integer = oldPointTypes.Length
			Dim newSizeMin As Integer = oldSize + needed
			If newSizeMin < oldSize Then Throw New ArrayIndexOutOfBoundsException("pointTypes exceeds maximum capacity !")
			' growth algorithm computation
			Dim grow As Integer = oldSize
			If grow > EXPAND_MAX Then
				grow = System.Math.Max(EXPAND_MAX, oldSize >> 3) ' 1/8th min
			ElseIf grow < EXPAND_MIN Then
				grow = EXPAND_MIN
			End If
			Debug.Assert(grow > 0)

			Dim newSize As Integer = oldSize + grow
			If newSize < newSizeMin Then newSize =  java.lang.[Integer].Max_Value
			Do
				Try
					' try allocating the larger array
					Return java.util.Arrays.copyOf(oldPointTypes, newSize)
				Catch oome As OutOfMemoryError
					If newSize = newSizeMin Then Throw oome
				End Try
				newSize = newSizeMin + (newSize - newSizeMin) \ 2
			Loop
		End Function

		''' <summary>
		''' The {@code Float} class defines a geometric path with
		''' coordinates stored in single precision floating point.
		''' 
		''' @since 1.6
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits Path2D

			<NonSerialized> _
			Friend floatCoords As Single()

			''' <summary>
			''' Constructs a new empty single precision {@code Path2D} object
			''' with a default winding rule of <seealso cref="#WIND_NON_ZERO"/>.
			''' 
			''' @since 1.6
			''' </summary>
			Public Sub New()
				Me.New(WIND_NON_ZERO, INIT_SIZE)
			End Sub

			''' <summary>
			''' Constructs a new empty single precision {@code Path2D} object
			''' with the specified winding rule to control operations that
			''' require the interior of the path to be defined.
			''' </summary>
			''' <param name="rule"> the winding rule </param>
			''' <seealso cref= #WIND_EVEN_ODD </seealso>
			''' <seealso cref= #WIND_NON_ZERO
			''' @since 1.6 </seealso>
			Public Sub New(  rule As Integer)
				Me.New(rule, INIT_SIZE)
			End Sub

			''' <summary>
			''' Constructs a new empty single precision {@code Path2D} object
			''' with the specified winding rule and the specified initial
			''' capacity to store path segments.
			''' This number is an initial guess as to how many path segments
			''' will be added to the path, but the storage is expanded as
			''' needed to store whatever path segments are added.
			''' </summary>
			''' <param name="rule"> the winding rule </param>
			''' <param name="initialCapacity"> the estimate for the number of path segments
			'''                        in the path </param>
			''' <seealso cref= #WIND_EVEN_ODD </seealso>
			''' <seealso cref= #WIND_NON_ZERO
			''' @since 1.6 </seealso>
			Public Sub New(  rule As Integer,   initialCapacity As Integer)
				MyBase.New(rule, initialCapacity)
				floatCoords = New Single(initialCapacity * 2 - 1){}
			End Sub

			''' <summary>
			''' Constructs a new single precision {@code Path2D} object
			''' from an arbitrary <seealso cref="Shape"/> object.
			''' All of the initial geometry and the winding rule for this path are
			''' taken from the specified {@code Shape} object.
			''' </summary>
			''' <param name="s"> the specified {@code Shape} object
			''' @since 1.6 </param>
			Public Sub New(  s As java.awt.Shape)
				Me.New(s, Nothing)
			End Sub

			''' <summary>
			''' Constructs a new single precision {@code Path2D} object
			''' from an arbitrary <seealso cref="Shape"/> object, transformed by an
			''' <seealso cref="AffineTransform"/> object.
			''' All of the initial geometry and the winding rule for this path are
			''' taken from the specified {@code Shape} object and transformed
			''' by the specified {@code AffineTransform} object.
			''' </summary>
			''' <param name="s"> the specified {@code Shape} object </param>
			''' <param name="at"> the specified {@code AffineTransform} object
			''' @since 1.6 </param>
			Public Sub New(  s As java.awt.Shape,   at As AffineTransform)
				If TypeOf s Is Path2D Then
					Dim p2d As Path2D = CType(s, Path2D)
					windingRule = p2d.windingRule
					Me.numTypes = p2d.numTypes
					' trim arrays:
					Me.pointTypes = java.util.Arrays.copyOf(p2d.pointTypes, p2d.numTypes)
					Me.numCoords = p2d.numCoords
					Me.floatCoords = p2d.cloneCoordsFloat(at)
				Else
					Dim pi As PathIterator = s.getPathIterator(at)
					windingRule = pi.windingRule
					Me.pointTypes = New SByte(INIT_SIZE - 1){}
					Me.floatCoords = New Single(INIT_SIZE * 2 - 1){}
					append(pi, False)
				End If
			End Sub

			Friend Overrides Function cloneCoordsFloat(  at As AffineTransform) As Single()
				' trim arrays:
				Dim ret As Single()
				If at Is Nothing Then
					ret = java.util.Arrays.copyOf(floatCoords, numCoords)
				Else
					ret = New Single(numCoords - 1){}
					at.transform(floatCoords, 0, ret, 0, numCoords \ 2)
				End If
				Return ret
			End Function

			Friend Overrides Function cloneCoordsDouble(  at As AffineTransform) As Double()
				' trim arrays:
				Dim ret As Double() = New Double(numCoords - 1){}
				If at Is Nothing Then
					For i As Integer = 0 To numCoords - 1
						ret(i) = floatCoords(i)
					Next i
				Else
					at.transform(floatCoords, 0, ret, 0, numCoords \ 2)
				End If
				Return ret
			End Function

			Friend Overrides Sub append(  x As Single,   y As Single)
				floatCoords(numCoords) = x
				numCoords += 1
				floatCoords(numCoords) = y
				numCoords += 1
			End Sub

			Friend Overrides Sub append(  x As Double,   y As Double)
				floatCoords(numCoords) = CSng(x)
				numCoords += 1
				floatCoords(numCoords) = CSng(y)
				numCoords += 1
			End Sub

			Friend Overrides Function getPoint(  coordindex As Integer) As Point2D
				Return New Point2D.Float(floatCoords(coordindex), floatCoords(coordindex+1))
			End Function

			Friend Overrides Sub needRoom(  needMove As Boolean,   newCoords As Integer)
				If (numTypes = 0) AndAlso needMove Then Throw New IllegalPathStateException("missing initial moveto " & "in path definition")
				If numTypes >= pointTypes.Length Then pointTypes = expandPointTypes(pointTypes, 1)
				If numCoords > (floatCoords.Length - newCoords) Then floatCoords = expandCoords(floatCoords, newCoords)
			End Sub

			Shared Function expandCoords(  oldCoords As Single(),   needed As Integer) As Single()
				Dim oldSize As Integer = oldCoords.Length
				Dim newSizeMin As Integer = oldSize + needed
				If newSizeMin < oldSize Then Throw New ArrayIndexOutOfBoundsException("coords exceeds maximum capacity !")
				' growth algorithm computation
				Dim grow As Integer = oldSize
				If grow > EXPAND_MAX_COORDS Then
					grow = System.Math.Max(EXPAND_MAX_COORDS, oldSize >> 3) ' 1/8th min
				ElseIf grow < EXPAND_MIN Then
					grow = EXPAND_MIN
				End If
				Debug.Assert(grow > needed)

				Dim newSize As Integer = oldSize + grow
				If newSize < newSizeMin Then newSize =  java.lang.[Integer].Max_Value
				Do
					Try
						' try allocating the larger array
						Return java.util.Arrays.copyOf(oldCoords, newSize)
					Catch oome As OutOfMemoryError
						If newSize = newSizeMin Then Throw oome
					End Try
					newSize = newSizeMin + (newSize - newSizeMin) \ 2
				Loop
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub moveTo(  x As Double,   y As Double)
				If numTypes > 0 AndAlso pointTypes(numTypes - 1) = SEG_MOVETO Then
					floatCoords(numCoords-2) = CSng(x)
					floatCoords(numCoords-1) = CSng(y)
				Else
					needRoom(False, 2)
					pointTypes(numTypes) = SEG_MOVETO
					numTypes += 1
					floatCoords(numCoords) = CSng(x)
					numCoords += 1
					floatCoords(numCoords) = CSng(y)
					numCoords += 1
				End If
			End Sub

			''' <summary>
			''' Adds a point to the path by moving to the specified
			''' coordinates specified in float precision.
			''' <p>
			''' This method provides a single precision variant of
			''' the double precision {@code moveTo()} method on the
			''' base {@code Path2D} class.
			''' </summary>
			''' <param name="x"> the specified X coordinate </param>
			''' <param name="y"> the specified Y coordinate </param>
			''' <seealso cref= Path2D#moveTo
			''' @since 1.6 </seealso>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Sub moveTo(  x As Single,   y As Single)
				If numTypes > 0 AndAlso pointTypes(numTypes - 1) = SEG_MOVETO Then
					floatCoords(numCoords-2) = x
					floatCoords(numCoords-1) = y
				Else
					needRoom(False, 2)
					pointTypes(numTypes) = SEG_MOVETO
					numTypes += 1
					floatCoords(numCoords) = x
					numCoords += 1
					floatCoords(numCoords) = y
					numCoords += 1
				End If
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub lineTo(  x As Double,   y As Double)
				needRoom(True, 2)
				pointTypes(numTypes) = SEG_LINETO
				numTypes += 1
				floatCoords(numCoords) = CSng(x)
				numCoords += 1
				floatCoords(numCoords) = CSng(y)
				numCoords += 1
			End Sub

			''' <summary>
			''' Adds a point to the path by drawing a straight line from the
			''' current coordinates to the new specified coordinates
			''' specified in float precision.
			''' <p>
			''' This method provides a single precision variant of
			''' the double precision {@code lineTo()} method on the
			''' base {@code Path2D} class.
			''' </summary>
			''' <param name="x"> the specified X coordinate </param>
			''' <param name="y"> the specified Y coordinate </param>
			''' <seealso cref= Path2D#lineTo
			''' @since 1.6 </seealso>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Sub lineTo(  x As Single,   y As Single)
				needRoom(True, 2)
				pointTypes(numTypes) = SEG_LINETO
				numTypes += 1
				floatCoords(numCoords) = x
				numCoords += 1
				floatCoords(numCoords) = y
				numCoords += 1
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub quadTo(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double)
				needRoom(True, 4)
				pointTypes(numTypes) = SEG_QUADTO
				numTypes += 1
				floatCoords(numCoords) = CSng(x1)
				numCoords += 1
				floatCoords(numCoords) = CSng(y1)
				numCoords += 1
				floatCoords(numCoords) = CSng(x2)
				numCoords += 1
				floatCoords(numCoords) = CSng(y2)
				numCoords += 1
			End Sub

			''' <summary>
			''' Adds a curved segment, defined by two new points, to the path by
			''' drawing a Quadratic curve that intersects both the current
			''' coordinates and the specified coordinates {@code (x2,y2)},
			''' using the specified point {@code (x1,y1)} as a quadratic
			''' parametric control point.
			''' All coordinates are specified in float precision.
			''' <p>
			''' This method provides a single precision variant of
			''' the double precision {@code quadTo()} method on the
			''' base {@code Path2D} class.
			''' </summary>
			''' <param name="x1"> the X coordinate of the quadratic control point </param>
			''' <param name="y1"> the Y coordinate of the quadratic control point </param>
			''' <param name="x2"> the X coordinate of the final end point </param>
			''' <param name="y2"> the Y coordinate of the final end point </param>
			''' <seealso cref= Path2D#quadTo
			''' @since 1.6 </seealso>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Sub quadTo(  x1 As Single,   y1 As Single,   x2 As Single,   y2 As Single)
				needRoom(True, 4)
				pointTypes(numTypes) = SEG_QUADTO
				numTypes += 1
				floatCoords(numCoords) = x1
				numCoords += 1
				floatCoords(numCoords) = y1
				numCoords += 1
				floatCoords(numCoords) = x2
				numCoords += 1
				floatCoords(numCoords) = y2
				numCoords += 1
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub curveTo(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double,   x3 As Double,   y3 As Double)
				needRoom(True, 6)
				pointTypes(numTypes) = SEG_CUBICTO
				numTypes += 1
				floatCoords(numCoords) = CSng(x1)
				numCoords += 1
				floatCoords(numCoords) = CSng(y1)
				numCoords += 1
				floatCoords(numCoords) = CSng(x2)
				numCoords += 1
				floatCoords(numCoords) = CSng(y2)
				numCoords += 1
				floatCoords(numCoords) = CSng(x3)
				numCoords += 1
				floatCoords(numCoords) = CSng(y3)
				numCoords += 1
			End Sub

			''' <summary>
			''' Adds a curved segment, defined by three new points, to the path by
			''' drawing a B&eacute;zier curve that intersects both the current
			''' coordinates and the specified coordinates {@code (x3,y3)},
			''' using the specified points {@code (x1,y1)} and {@code (x2,y2)} as
			''' B&eacute;zier control points.
			''' All coordinates are specified in float precision.
			''' <p>
			''' This method provides a single precision variant of
			''' the double precision {@code curveTo()} method on the
			''' base {@code Path2D} class.
			''' </summary>
			''' <param name="x1"> the X coordinate of the first B&eacute;zier control point </param>
			''' <param name="y1"> the Y coordinate of the first B&eacute;zier control point </param>
			''' <param name="x2"> the X coordinate of the second B&eacute;zier control point </param>
			''' <param name="y2"> the Y coordinate of the second B&eacute;zier control point </param>
			''' <param name="x3"> the X coordinate of the final end point </param>
			''' <param name="y3"> the Y coordinate of the final end point </param>
			''' <seealso cref= Path2D#curveTo
			''' @since 1.6 </seealso>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Sub curveTo(  x1 As Single,   y1 As Single,   x2 As Single,   y2 As Single,   x3 As Single,   y3 As Single)
				needRoom(True, 6)
				pointTypes(numTypes) = SEG_CUBICTO
				numTypes += 1
				floatCoords(numCoords) = x1
				numCoords += 1
				floatCoords(numCoords) = y1
				numCoords += 1
				floatCoords(numCoords) = x2
				numCoords += 1
				floatCoords(numCoords) = y2
				numCoords += 1
				floatCoords(numCoords) = x3
				numCoords += 1
				floatCoords(numCoords) = y3
				numCoords += 1
			End Sub

			Friend Overrides Function pointCrossings(  px As Double,   py As Double) As Integer
				If numTypes = 0 Then Return 0
				Dim movx, movy, curx, cury, endx, endy As Double
				Dim coords As Single() = floatCoords
					movx = coords(0)
					curx = movx
					movy = coords(1)
					cury = movy
				Dim crossings As Integer = 0
				Dim ci As Integer = 2
				For i As Integer = 1 To numTypes - 1
					Select Case pointTypes(i)
					Case PathIterator.SEG_MOVETO
						If cury <> movy Then crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy)
							curx = coords(ci)
							movx = curx
						ci += 1
							cury = coords(ci)
							movy = cury
						ci += 1
					Case PathIterator.SEG_LINETO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, endx = coords(ci++), endy = coords(ci++))
						curx = endx
						cury = endy
					Case PathIterator.SEG_QUADTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings += sun.awt.geom.Curve.pointCrossingsForQuad(px, py, curx, cury, coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
				Case PathIterator.SEG_CUBICTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings += sun.awt.geom.Curve.pointCrossingsForCubic(px, py, curx, cury, coords(ci++), coords(ci++), coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
					Case PathIterator.SEG_CLOSE
						If cury <> movy Then crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy)
						curx = movx
						cury = movy
					End Select
				Next i
				If cury <> movy Then crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy)
				Return crossings
			End Function

			Friend Overrides Function rectCrossings(  rxmin As Double,   rymin As Double,   rxmax As Double,   rymax As Double) As Integer
				If numTypes = 0 Then Return 0
				Dim coords As Single() = floatCoords
				Dim curx, cury, movx, movy, endx, endy As Double
					movx = coords(0)
					curx = movx
					movy = coords(1)
					cury = movy
				Dim crossings As Integer = 0
				Dim ci As Integer = 2
				Dim i As Integer = 1
				Do While crossings <> sun.awt.geom.Curve.RECT_INTERSECTS AndAlso i < numTypes
					Select Case pointTypes(i)
					Case PathIterator.SEG_MOVETO
						If curx <> movx OrElse cury <> movy Then crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy)
						' Count should always be a multiple of 2 here.
						' assert((crossings & 1) != 0);
							curx = coords(ci)
							movx = curx
						ci += 1
							cury = coords(ci)
							movy = cury
						ci += 1
					Case PathIterator.SEG_LINETO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, endx = coords(ci++), endy = coords(ci++))
						curx = endx
						cury = endy
					Case PathIterator.SEG_QUADTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings = sun.awt.geom.Curve.rectCrossingsForQuad(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
					Case PathIterator.SEG_CUBICTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings = sun.awt.geom.Curve.rectCrossingsForCubic(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords(ci++), coords(ci++), coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
					Case PathIterator.SEG_CLOSE
						If curx <> movx OrElse cury <> movy Then crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy)
						curx = movx
						cury = movy
						' Count should always be a multiple of 2 here.
						' assert((crossings & 1) != 0);
					End Select
					i += 1
				Loop
				If crossings <> sun.awt.geom.Curve.RECT_INTERSECTS AndAlso (curx <> movx OrElse cury <> movy) Then crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy)
				' Count should always be a multiple of 2 here.
				' assert((crossings & 1) != 0);
				Return crossings
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			Public NotOverridable Overrides Sub append(  pi As PathIterator,   connect As Boolean)
				Dim coords As Single() = New Single(5){}
				Do While Not pi.done
					Select Case pi.currentSegment(coords)
					Case SEG_MOVETO
						If (Not connect) OrElse numTypes < 1 OrElse numCoords < 1 Then
							moveTo(coords(0), coords(1))
							Exit Select
						End If
						If pointTypes(numTypes - 1) <> SEG_CLOSE AndAlso floatCoords(numCoords-2) = coords(0) AndAlso floatCoords(numCoords-1) = coords(1) Then Exit Select
						lineTo(coords(0), coords(1))
					Case SEG_LINETO
						lineTo(coords(0), coords(1))
					Case SEG_QUADTO
						quadTo(coords(0), coords(1), coords(2), coords(3))
					Case SEG_CUBICTO
						curveTo(coords(0), coords(1), coords(2), coords(3), coords(4), coords(5))
					Case SEG_CLOSE
						closePath()
					End Select
					pi.next()
					connect = False
				Loop
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			Public NotOverridable Overrides Sub transform(  at As AffineTransform)
				at.transform(floatCoords, 0, floatCoords, 0, numCoords \ 2)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property bounds2D As Rectangle2D
				Get
					Dim x1, y1, x2, y2 As Single
					Dim i As Integer = numCoords
					If i > 0 Then
						i -= 1
							y2 = floatCoords(i)
							y1 = y2
						i -= 1
							x2 = floatCoords(i)
							x1 = x2
						Do While i > 0
							i -= 1
							Dim y As Single = floatCoords(i)
							i -= 1
							Dim x As Single = floatCoords(i)
							If x < x1 Then x1 = x
							If y < y1 Then y1 = y
							If x > x2 Then x2 = x
							If y > y2 Then y2 = y
						Loop
					Else
							y2 = 0.0f
								x2 = y2
									y1 = x2
									x1 = y1
					End If
					Return New Rectangle2D.Float(x1, y1, x2 - x1, y2 - y1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' <p>
			''' The iterator for this class is not multi-threaded safe,
			''' which means that the {@code Path2D} class does not
			''' guarantee that modifications to the geometry of this
			''' {@code Path2D} object do not affect any iterations of
			''' that geometry that are already in process.
			''' 
			''' @since 1.6
			''' </summary>
			Public Function getPathIterator(  at As AffineTransform) As PathIterator
				If at Is Nothing Then
					Return New CopyIterator(Me)
				Else
					Return New TxIterator(Me, at)
				End If
			End Function

			''' <summary>
			''' Creates a new object of the same class as this object.
			''' </summary>
			''' <returns>     a clone of this instance. </returns>
			''' <exception cref="OutOfMemoryError">    if there is not enough memory. </exception>
			''' <seealso cref=        java.lang.Cloneable
			''' @since      1.6 </seealso>
			Public NotOverridable Overrides Function clone() As Object
				' Note: It would be nice to have this return Path2D
				' but one of our subclasses (GeneralPath) needs to
				' offer "public Object clone()" for backwards
				' compatibility so we cannot restrict it further.
				' REMIND: Can we do both somehow?
				If TypeOf Me Is GeneralPath Then
					Return New GeneralPath(Me)
				Else
					Return New Path2D.Float(Me)
				End If
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 6990832515060788886L

			''' <summary>
			''' Writes the default serializable fields to the
			''' {@code ObjectOutputStream} followed by an explicit
			''' serialization of the path segments stored in this
			''' path.
			''' 
			''' @serialData
			''' <a name="Path2DSerialData"><!-- --></a>
			''' <ol>
			''' <li>The default serializable fields.
			''' There are no default serializable fields as of 1.6.
			''' <li>followed by
			''' a byte indicating the storage type of the original object
			''' as a hint (SERIAL_STORAGE_FLT_ARRAY)
			''' <li>followed by
			''' an integer indicating the number of path segments to follow (NP)
			''' or -1 to indicate an unknown number of path segments follows
			''' <li>followed by
			''' an integer indicating the total number of coordinates to follow (NC)
			''' or -1 to indicate an unknown number of coordinates follows
			''' (NC should always be even since coordinates always appear in pairs
			'''  representing an x,y pair)
			''' <li>followed by
			''' a byte indicating the winding rule
			''' (<seealso cref="#WIND_EVEN_ODD WIND_EVEN_ODD"/> or
			'''  <seealso cref="#WIND_NON_ZERO WIND_NON_ZERO"/>)
			''' <li>followed by
			''' {@code NP} (or unlimited if {@code NP < 0}) sets of values consisting of
			''' a single byte indicating a path segment type
			''' followed by one or more pairs of float or double
			''' values representing the coordinates of the path segment
			''' <li>followed by
			''' a byte indicating the end of the path (SERIAL_PATH_END).
			''' </ol>
			''' <p>
			''' The following byte value constants are used in the serialized form
			''' of {@code Path2D} objects:
			''' <table>
			''' <tr>
			''' <th>Constant Name</th>
			''' <th>Byte Value</th>
			''' <th>Followed by</th>
			''' <th>Description</th>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_STORAGE_FLT_ARRAY}</td>
			''' <td>0x30</td>
			''' <td></td>
			''' <td>A hint that the original {@code Path2D} object stored
			''' the coordinates in a Java array of floats.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_STORAGE_DBL_ARRAY}</td>
			''' <td>0x31</td>
			''' <td></td>
			''' <td>A hint that the original {@code Path2D} object stored
			''' the coordinates in a Java array of doubles.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_MOVETO}</td>
			''' <td>0x40</td>
			''' <td>2 floats</td>
			''' <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_LINETO}</td>
			''' <td>0x41</td>
			''' <td>2 floats</td>
			''' <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_QUADTO}</td>
			''' <td>0x42</td>
			''' <td>4 floats</td>
			''' <td>A <seealso cref="#quadTo quadTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_CUBICTO}</td>
			''' <td>0x43</td>
			''' <td>6 floats</td>
			''' <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_MOVETO}</td>
			''' <td>0x50</td>
			''' <td>2 doubles</td>
			''' <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_LINETO}</td>
			''' <td>0x51</td>
			''' <td>2 doubles</td>
			''' <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_QUADTO}</td>
			''' <td>0x52</td>
			''' <td>4 doubles</td>
			''' <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_CUBICTO}</td>
			''' <td>0x53</td>
			''' <td>6 doubles</td>
			''' <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_CLOSE}</td>
			''' <td>0x60</td>
			''' <td></td>
			''' <td>A <seealso cref="#closePath closePath"/> path segment.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_PATH_END}</td>
			''' <td>0x61</td>
			''' <td></td>
			''' <td>There are no more path segments following.</td>
			''' </table>
			''' 
			''' @since 1.6
			''' </summary>
			Private Sub writeObject(  s As java.io.ObjectOutputStream)
				MyBase.writeObject(s, False)
			End Sub

			''' <summary>
			''' Reads the default serializable fields from the
			''' {@code ObjectInputStream} followed by an explicit
			''' serialization of the path segments stored in this
			''' path.
			''' <p>
			''' There are no default serializable fields as of 1.6.
			''' <p>
			''' The serial data for this object is described in the
			''' writeObject method.
			''' 
			''' @since 1.6
			''' </summary>
			Private Sub readObject(  s As java.io.ObjectInputStream)
				MyBase.readObject(s, False)
			End Sub

			Friend Class CopyIterator
				Inherits Path2D.Iterator

				Friend floatCoords As Single()

				Friend Sub New(  p2df As Path2D.Float)
					MyBase.New(p2df)
					Me.floatCoords = p2df.floatCoords
				End Sub

				Public Overridable Function currentSegment(  coords As Single()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then Array.Copy(floatCoords, pointIdx, coords, 0, numCoords)
					Return type
				End Function

				Public Overridable Function currentSegment(  coords As Double()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then
						For i As Integer = 0 To numCoords - 1
							coords(i) = floatCoords(pointIdx + i)
						Next i
					End If
					Return type
				End Function
			End Class

			Friend Class TxIterator
				Inherits Path2D.Iterator

				Friend floatCoords As Single()
				Friend affine As AffineTransform

				Friend Sub New(  p2df As Path2D.Float,   at As AffineTransform)
					MyBase.New(p2df)
					Me.floatCoords = p2df.floatCoords
					Me.affine = at
				End Sub

				Public Overridable Function currentSegment(  coords As Single()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then affine.transform(floatCoords, pointIdx, coords, 0, numCoords \ 2)
					Return type
				End Function

				Public Overridable Function currentSegment(  coords As Double()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then affine.transform(floatCoords, pointIdx, coords, 0, numCoords \ 2)
					Return type
				End Function
			End Class

		End Class

		''' <summary>
		''' The {@code Double} class defines a geometric path with
		''' coordinates stored in double precision floating point.
		''' 
		''' @since 1.6
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits Path2D

			<NonSerialized> _
			Friend doubleCoords As Double()

			''' <summary>
			''' Constructs a new empty double precision {@code Path2D} object
			''' with a default winding rule of <seealso cref="#WIND_NON_ZERO"/>.
			''' 
			''' @since 1.6
			''' </summary>
			Function java.lang.Double() As [Public] Overridable
				Me(WIND_NON_ZERO, INIT_SIZE)
			End Function

			''' <summary>
			''' Constructs a new empty double precision {@code Path2D} object
			''' with the specified winding rule to control operations that
			''' require the interior of the path to be defined.
			''' </summary>
			''' <param name="rule"> the winding rule </param>
			''' <seealso cref= #WIND_EVEN_ODD </seealso>
			''' <seealso cref= #WIND_NON_ZERO
			''' @since 1.6 </seealso>
			Function java.lang.Double(  rule As Integer) As [Public] Overridable
				Me(rule, INIT_SIZE)
			End Function

			''' <summary>
			''' Constructs a new empty double precision {@code Path2D} object
			''' with the specified winding rule and the specified initial
			''' capacity to store path segments.
			''' This number is an initial guess as to how many path segments
			''' are in the path, but the storage is expanded as needed to store
			''' whatever path segments are added to this path.
			''' </summary>
			''' <param name="rule"> the winding rule </param>
			''' <param name="initialCapacity"> the estimate for the number of path segments
			'''                        in the path </param>
			''' <seealso cref= #WIND_EVEN_ODD </seealso>
			''' <seealso cref= #WIND_NON_ZERO
			''' @since 1.6 </seealso>
			Function java.lang.Double(  rule As Integer,   initialCapacity As Integer) As [Public] Overridable
				MyBase(rule, initialCapacity)
				doubleCoords = New Double(initialCapacity * 2 - 1){}
			End Function

			''' <summary>
			''' Constructs a new double precision {@code Path2D} object
			''' from an arbitrary <seealso cref="Shape"/> object.
			''' All of the initial geometry and the winding rule for this path are
			''' taken from the specified {@code Shape} object.
			''' </summary>
			''' <param name="s"> the specified {@code Shape} object
			''' @since 1.6 </param>
			Function java.lang.Double(  s As java.awt.Shape) As [Public] Overridable
				Me(s, Nothing)
			End Function

			''' <summary>
			''' Constructs a new double precision {@code Path2D} object
			''' from an arbitrary <seealso cref="Shape"/> object, transformed by an
			''' <seealso cref="AffineTransform"/> object.
			''' All of the initial geometry and the winding rule for this path are
			''' taken from the specified {@code Shape} object and transformed
			''' by the specified {@code AffineTransform} object.
			''' </summary>
			''' <param name="s"> the specified {@code Shape} object </param>
			''' <param name="at"> the specified {@code AffineTransform} object
			''' @since 1.6 </param>
			Function java.lang.Double(  s As java.awt.Shape,   at As AffineTransform) As [Public] Overridable
				If TypeOf s Is Path2D Then
					Dim p2d As Path2D = CType(s, Path2D)
					windingRule = p2d.windingRule
					Me.numTypes = p2d.numTypes
					' trim arrays:
					Me.pointTypes = java.util.Arrays.copyOf(p2d.pointTypes, p2d.numTypes)
					Me.numCoords = p2d.numCoords
					Me.doubleCoords = p2d.cloneCoordsDouble(at)
				Else
					Dim pi As PathIterator = s.getPathIterator(at)
					windingRule = pi.windingRule
					Me.pointTypes = New SByte(INIT_SIZE - 1){}
					Me.doubleCoords = New Double(INIT_SIZE * 2 - 1){}
					append(pi, False)
				End If
			End Function

			Friend Overrides Function cloneCoordsFloat(  at As AffineTransform) As Single()
				' trim arrays:
				Dim ret As Single() = New Single(numCoords - 1){}
				If at Is Nothing Then
					For i As Integer = 0 To numCoords - 1
						ret(i) = CSng(doubleCoords(i))
					Next i
				Else
					at.transform(doubleCoords, 0, ret, 0, numCoords \ 2)
				End If
				Return ret
			End Function

			Friend Overrides Function cloneCoordsDouble(  at As AffineTransform) As Double()
				' trim arrays:
				Dim ret As Double()
				If at Is Nothing Then
					ret = java.util.Arrays.copyOf(doubleCoords, numCoords)
				Else
					ret = New Double(numCoords - 1){}
					at.transform(doubleCoords, 0, ret, 0, numCoords \ 2)
				End If
				Return ret
			End Function

			Friend Overrides Sub append(  x As Single,   y As Single)
				doubleCoords(numCoords) = x
				numCoords += 1
				doubleCoords(numCoords) = y
				numCoords += 1
			End Sub

			Friend Overrides Sub append(  x As Double,   y As Double)
				doubleCoords(numCoords) = x
				numCoords += 1
				doubleCoords(numCoords) = y
				numCoords += 1
			End Sub

			Friend Overrides Function getPoint(  coordindex As Integer) As Point2D
				Return New Point2D.Double(doubleCoords(coordindex), doubleCoords(coordindex+1))
			End Function

			Friend Overrides Sub needRoom(  needMove As Boolean,   newCoords As Integer)
				If (numTypes = 0) AndAlso needMove Then Throw New IllegalPathStateException("missing initial moveto " & "in path definition")
				If numTypes >= pointTypes.Length Then pointTypes = expandPointTypes(pointTypes, 1)
				If numCoords > (doubleCoords.Length - newCoords) Then doubleCoords = expandCoords(doubleCoords, newCoords)
			End Sub

			Friend Shared Function expandCoords(  oldCoords As Double(),   needed As Integer) As Double()
				Dim oldSize As Integer = oldCoords.Length
				Dim newSizeMin As Integer = oldSize + needed
				If newSizeMin < oldSize Then Throw New ArrayIndexOutOfBoundsException("coords exceeds maximum capacity !")
				' growth algorithm computation
				Dim grow As Integer = oldSize
				If grow > EXPAND_MAX_COORDS Then
					grow = System.Math.Max(EXPAND_MAX_COORDS, oldSize >> 3) ' 1/8th min
				ElseIf grow < EXPAND_MIN Then
					grow = EXPAND_MIN
				End If
				Debug.Assert(grow > needed)

				Dim newSize As Integer = oldSize + grow
				If newSize < newSizeMin Then newSize =  java.lang.[Integer].Max_Value
				Do
					Try
						' try allocating the larger array
						Return java.util.Arrays.copyOf(oldCoords, newSize)
					Catch oome As OutOfMemoryError
						If newSize = newSizeMin Then Throw oome
					End Try
					newSize = newSizeMin + (newSize - newSizeMin) \ 2
				Loop
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub moveTo(  x As Double,   y As Double)
				If numTypes > 0 AndAlso pointTypes(numTypes - 1) = SEG_MOVETO Then
					doubleCoords(numCoords-2) = x
					doubleCoords(numCoords-1) = y
				Else
					needRoom(False, 2)
					pointTypes(numTypes) = SEG_MOVETO
					numTypes += 1
					doubleCoords(numCoords) = x
					numCoords += 1
					doubleCoords(numCoords) = y
					numCoords += 1
				End If
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub lineTo(  x As Double,   y As Double)
				needRoom(True, 2)
				pointTypes(numTypes) = SEG_LINETO
				numTypes += 1
				doubleCoords(numCoords) = x
				numCoords += 1
				doubleCoords(numCoords) = y
				numCoords += 1
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub quadTo(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double)
				needRoom(True, 4)
				pointTypes(numTypes) = SEG_QUADTO
				numTypes += 1
				doubleCoords(numCoords) = x1
				numCoords += 1
				doubleCoords(numCoords) = y1
				numCoords += 1
				doubleCoords(numCoords) = x2
				numCoords += 1
				doubleCoords(numCoords) = y2
				numCoords += 1
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public NotOverridable Overrides Sub curveTo(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double,   x3 As Double,   y3 As Double)
				needRoom(True, 6)
				pointTypes(numTypes) = SEG_CUBICTO
				numTypes += 1
				doubleCoords(numCoords) = x1
				numCoords += 1
				doubleCoords(numCoords) = y1
				numCoords += 1
				doubleCoords(numCoords) = x2
				numCoords += 1
				doubleCoords(numCoords) = y2
				numCoords += 1
				doubleCoords(numCoords) = x3
				numCoords += 1
				doubleCoords(numCoords) = y3
				numCoords += 1
			End Sub

			Friend Overrides Function pointCrossings(  px As Double,   py As Double) As Integer
				If numTypes = 0 Then Return 0
				Dim movx, movy, curx, cury, endx, endy As Double
				Dim coords As Double() = doubleCoords
					movx = coords(0)
					curx = movx
					movy = coords(1)
					cury = movy
				Dim crossings As Integer = 0
				Dim ci As Integer = 2
				For i As Integer = 1 To numTypes - 1
					Select Case pointTypes(i)
					Case PathIterator.SEG_MOVETO
						If cury <> movy Then crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy)
							curx = coords(ci)
							movx = curx
						ci += 1
							cury = coords(ci)
							movy = cury
						ci += 1
					Case PathIterator.SEG_LINETO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, endx = coords(ci++), endy = coords(ci++))
						curx = endx
						cury = endy
					Case PathIterator.SEG_QUADTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings += sun.awt.geom.Curve.pointCrossingsForQuad(px, py, curx, cury, coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
				Case PathIterator.SEG_CUBICTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings += sun.awt.geom.Curve.pointCrossingsForCubic(px, py, curx, cury, coords(ci++), coords(ci++), coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
					Case PathIterator.SEG_CLOSE
						If cury <> movy Then crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy)
						curx = movx
						cury = movy
					End Select
				Next i
				If cury <> movy Then crossings += sun.awt.geom.Curve.pointCrossingsForLine(px, py, curx, cury, movx, movy)
				Return crossings
			End Function

			Friend Overrides Function rectCrossings(  rxmin As Double,   rymin As Double,   rxmax As Double,   rymax As Double) As Integer
				If numTypes = 0 Then Return 0
				Dim coords As Double() = doubleCoords
				Dim curx, cury, movx, movy, endx, endy As Double
					movx = coords(0)
					curx = movx
					movy = coords(1)
					cury = movy
				Dim crossings As Integer = 0
				Dim ci As Integer = 2
				Dim i As Integer = 1
				Do While crossings <> sun.awt.geom.Curve.RECT_INTERSECTS AndAlso i < numTypes
					Select Case pointTypes(i)
					Case PathIterator.SEG_MOVETO
						If curx <> movx OrElse cury <> movy Then crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy)
						' Count should always be a multiple of 2 here.
						' assert((crossings & 1) != 0);
							curx = coords(ci)
							movx = curx
						ci += 1
							cury = coords(ci)
							movy = cury
						ci += 1
					Case PathIterator.SEG_LINETO
						endx = coords(ci)
						ci += 1
						endy = coords(ci)
						ci += 1
						crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, endx, endy)
						curx = endx
						cury = endy
					Case PathIterator.SEG_QUADTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings = sun.awt.geom.Curve.rectCrossingsForQuad(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
					Case PathIterator.SEG_CUBICTO
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						crossings = sun.awt.geom.Curve.rectCrossingsForCubic(crossings, rxmin, rymin, rxmax, rymax, curx, cury, coords(ci++), coords(ci++), coords(ci++), coords(ci++), endx = coords(ci++), endy = coords(ci++), 0)
						curx = endx
						cury = endy
					Case PathIterator.SEG_CLOSE
						If curx <> movx OrElse cury <> movy Then crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy)
						curx = movx
						cury = movy
						' Count should always be a multiple of 2 here.
						' assert((crossings & 1) != 0);
					End Select
					i += 1
				Loop
				If crossings <> sun.awt.geom.Curve.RECT_INTERSECTS AndAlso (curx <> movx OrElse cury <> movy) Then crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, rxmin, rymin, rxmax, rymax, curx, cury, movx, movy)
				' Count should always be a multiple of 2 here.
				' assert((crossings & 1) != 0);
				Return crossings
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			Public NotOverridable Overrides Sub append(  pi As PathIterator,   connect As Boolean)
				Dim coords As Double() = New Double(5){}
				Do While Not pi.done
					Select Case pi.currentSegment(coords)
					Case SEG_MOVETO
						If (Not connect) OrElse numTypes < 1 OrElse numCoords < 1 Then
							moveTo(coords(0), coords(1))
							Exit Select
						End If
						If pointTypes(numTypes - 1) <> SEG_CLOSE AndAlso doubleCoords(numCoords-2) = coords(0) AndAlso doubleCoords(numCoords-1) = coords(1) Then Exit Select
						lineTo(coords(0), coords(1))
					Case SEG_LINETO
						lineTo(coords(0), coords(1))
					Case SEG_QUADTO
						quadTo(coords(0), coords(1), coords(2), coords(3))
					Case SEG_CUBICTO
						curveTo(coords(0), coords(1), coords(2), coords(3), coords(4), coords(5))
					Case SEG_CLOSE
						closePath()
					End Select
					pi.next()
					connect = False
				Loop
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			Public NotOverridable Overrides Sub transform(  at As AffineTransform)
				at.transform(doubleCoords, 0, doubleCoords, 0, numCoords \ 2)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.6
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property bounds2D As Rectangle2D
				Get
					Dim x1, y1, x2, y2 As Double
					Dim i As Integer = numCoords
					If i > 0 Then
						i -= 1
							y2 = doubleCoords(i)
							y1 = y2
						i -= 1
							x2 = doubleCoords(i)
							x1 = x2
						Do While i > 0
							i -= 1
							Dim y As Double = doubleCoords(i)
							i -= 1
							Dim x As Double = doubleCoords(i)
							If x < x1 Then x1 = x
							If y < y1 Then y1 = y
							If x > x2 Then x2 = x
							If y > y2 Then y2 = y
						Loop
					Else
							y2 = 0.0
								x2 = y2
									y1 = x2
									x1 = y1
					End If
					Return New Rectangle2D.Double(x1, y1, x2 - x1, y2 - y1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' <p>
			''' The iterator for this class is not multi-threaded safe,
			''' which means that the {@code Path2D} class does not
			''' guarantee that modifications to the geometry of this
			''' {@code Path2D} object do not affect any iterations of
			''' that geometry that are already in process.
			''' </summary>
			''' <param name="at"> an {@code AffineTransform} </param>
			''' <returns> a new {@code PathIterator} that iterates along the boundary
			'''         of this {@code Shape} and provides access to the geometry
			'''         of this {@code Shape}'s outline
			''' @since 1.6 </returns>
			Public Function getPathIterator(  at As AffineTransform) As PathIterator
				If at Is Nothing Then
					Return New CopyIterator(Me)
				Else
					Return New TxIterator(Me, at)
				End If
			End Function

			''' <summary>
			''' Creates a new object of the same class as this object.
			''' </summary>
			''' <returns>     a clone of this instance. </returns>
			''' <exception cref="OutOfMemoryError">    if there is not enough memory. </exception>
			''' <seealso cref=        java.lang.Cloneable
			''' @since      1.6 </seealso>
			Public NotOverridable Overrides Function clone() As Object
				' Note: It would be nice to have this return Path2D
				' but one of our subclasses (GeneralPath) needs to
				' offer "public Object clone()" for backwards
				' compatibility so we cannot restrict it further.
				' REMIND: Can we do both somehow?
				Return New Path2D.Double(Me)
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 1826762518450014216L

			''' <summary>
			''' Writes the default serializable fields to the
			''' {@code ObjectOutputStream} followed by an explicit
			''' serialization of the path segments stored in this
			''' path.
			''' 
			''' @serialData
			''' <a name="Path2DSerialData"><!-- --></a>
			''' <ol>
			''' <li>The default serializable fields.
			''' There are no default serializable fields as of 1.6.
			''' <li>followed by
			''' a byte indicating the storage type of the original object
			''' as a hint (SERIAL_STORAGE_DBL_ARRAY)
			''' <li>followed by
			''' an integer indicating the number of path segments to follow (NP)
			''' or -1 to indicate an unknown number of path segments follows
			''' <li>followed by
			''' an integer indicating the total number of coordinates to follow (NC)
			''' or -1 to indicate an unknown number of coordinates follows
			''' (NC should always be even since coordinates always appear in pairs
			'''  representing an x,y pair)
			''' <li>followed by
			''' a byte indicating the winding rule
			''' (<seealso cref="#WIND_EVEN_ODD WIND_EVEN_ODD"/> or
			'''  <seealso cref="#WIND_NON_ZERO WIND_NON_ZERO"/>)
			''' <li>followed by
			''' {@code NP} (or unlimited if {@code NP < 0}) sets of values consisting of
			''' a single byte indicating a path segment type
			''' followed by one or more pairs of float or double
			''' values representing the coordinates of the path segment
			''' <li>followed by
			''' a byte indicating the end of the path (SERIAL_PATH_END).
			''' </ol>
			''' <p>
			''' The following byte value constants are used in the serialized form
			''' of {@code Path2D} objects:
			''' <table>
			''' <tr>
			''' <th>Constant Name</th>
			''' <th>Byte Value</th>
			''' <th>Followed by</th>
			''' <th>Description</th>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_STORAGE_FLT_ARRAY}</td>
			''' <td>0x30</td>
			''' <td></td>
			''' <td>A hint that the original {@code Path2D} object stored
			''' the coordinates in a Java array of floats.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_STORAGE_DBL_ARRAY}</td>
			''' <td>0x31</td>
			''' <td></td>
			''' <td>A hint that the original {@code Path2D} object stored
			''' the coordinates in a Java array of doubles.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_MOVETO}</td>
			''' <td>0x40</td>
			''' <td>2 floats</td>
			''' <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_LINETO}</td>
			''' <td>0x41</td>
			''' <td>2 floats</td>
			''' <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_QUADTO}</td>
			''' <td>0x42</td>
			''' <td>4 floats</td>
			''' <td>A <seealso cref="#quadTo quadTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_FLT_CUBICTO}</td>
			''' <td>0x43</td>
			''' <td>6 floats</td>
			''' <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_MOVETO}</td>
			''' <td>0x50</td>
			''' <td>2 doubles</td>
			''' <td>A <seealso cref="#moveTo moveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_LINETO}</td>
			''' <td>0x51</td>
			''' <td>2 doubles</td>
			''' <td>A <seealso cref="#lineTo lineTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_QUADTO}</td>
			''' <td>0x52</td>
			''' <td>4 doubles</td>
			''' <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_DBL_CUBICTO}</td>
			''' <td>0x53</td>
			''' <td>6 doubles</td>
			''' <td>A <seealso cref="#curveTo curveTo"/> path segment follows.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_SEG_CLOSE}</td>
			''' <td>0x60</td>
			''' <td></td>
			''' <td>A <seealso cref="#closePath closePath"/> path segment.</td>
			''' </tr>
			''' <tr>
			''' <td>{@code SERIAL_PATH_END}</td>
			''' <td>0x61</td>
			''' <td></td>
			''' <td>There are no more path segments following.</td>
			''' </table>
			''' 
			''' @since 1.6
			''' </summary>
			Private Sub writeObject(  s As java.io.ObjectOutputStream)
				MyBase.writeObject(s, True)
			End Sub

			''' <summary>
			''' Reads the default serializable fields from the
			''' {@code ObjectInputStream} followed by an explicit
			''' serialization of the path segments stored in this
			''' path.
			''' <p>
			''' There are no default serializable fields as of 1.6.
			''' <p>
			''' The serial data for this object is described in the
			''' writeObject method.
			''' 
			''' @since 1.6
			''' </summary>
			Private Sub readObject(  s As java.io.ObjectInputStream)
				MyBase.readObject(s, True)
			End Sub

			Friend Class CopyIterator
				Inherits Path2D.Iterator

				Friend doubleCoords As Double()

				Friend Sub New(  p2dd As Path2D.Double)
					MyBase.New(p2dd)
					Me.doubleCoords = p2dd.doubleCoords
				End Sub

				Public Overridable Function currentSegment(  coords As Single()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then
						For i As Integer = 0 To numCoords - 1
							coords(i) = CSng(doubleCoords(pointIdx + i))
						Next i
					End If
					Return type
				End Function

				Public Overridable Function currentSegment(  coords As Double()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then Array.Copy(doubleCoords, pointIdx, coords, 0, numCoords)
					Return type
				End Function
			End Class

			Friend Class TxIterator
				Inherits Path2D.Iterator

				Friend doubleCoords As Double()
				Friend affine As AffineTransform

				Friend Sub New(  p2dd As Path2D.Double,   at As AffineTransform)
					MyBase.New(p2dd)
					Me.doubleCoords = p2dd.doubleCoords
					Me.affine = at
				End Sub

				Public Overridable Function currentSegment(  coords As Single()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then affine.transform(doubleCoords, pointIdx, coords, 0, numCoords \ 2)
					Return type
				End Function

				Public Overridable Function currentSegment(  coords As Double()) As Integer
					Dim type As Integer = path.pointTypes(typeIdx)
					Dim numCoords As Integer = curvecoords(type)
					If numCoords > 0 Then affine.transform(doubleCoords, pointIdx, coords, 0, numCoords \ 2)
					Return type
				End Function
			End Class
		End Class

		''' <summary>
		''' Adds a point to the path by moving to the specified
		''' coordinates specified in double precision.
		''' </summary>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate
		''' @since 1.6 </param>
		Public MustOverride Sub moveTo(  x As Double,   y As Double)

		''' <summary>
		''' Adds a point to the path by drawing a straight line from the
		''' current coordinates to the new specified coordinates
		''' specified in double precision.
		''' </summary>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate
		''' @since 1.6 </param>
		Public MustOverride Sub lineTo(  x As Double,   y As Double)

		''' <summary>
		''' Adds a curved segment, defined by two new points, to the path by
		''' drawing a Quadratic curve that intersects both the current
		''' coordinates and the specified coordinates {@code (x2,y2)},
		''' using the specified point {@code (x1,y1)} as a quadratic
		''' parametric control point.
		''' All coordinates are specified in double precision.
		''' </summary>
		''' <param name="x1"> the X coordinate of the quadratic control point </param>
		''' <param name="y1"> the Y coordinate of the quadratic control point </param>
		''' <param name="x2"> the X coordinate of the final end point </param>
		''' <param name="y2"> the Y coordinate of the final end point
		''' @since 1.6 </param>
		Public MustOverride Sub quadTo(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double)

		''' <summary>
		''' Adds a curved segment, defined by three new points, to the path by
		''' drawing a B&eacute;zier curve that intersects both the current
		''' coordinates and the specified coordinates {@code (x3,y3)},
		''' using the specified points {@code (x1,y1)} and {@code (x2,y2)} as
		''' B&eacute;zier control points.
		''' All coordinates are specified in double precision.
		''' </summary>
		''' <param name="x1"> the X coordinate of the first B&eacute;zier control point </param>
		''' <param name="y1"> the Y coordinate of the first B&eacute;zier control point </param>
		''' <param name="x2"> the X coordinate of the second B&eacute;zier control point </param>
		''' <param name="y2"> the Y coordinate of the second B&eacute;zier control point </param>
		''' <param name="x3"> the X coordinate of the final end point </param>
		''' <param name="y3"> the Y coordinate of the final end point
		''' @since 1.6 </param>
		Public MustOverride Sub curveTo(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double,   x3 As Double,   y3 As Double)

		''' <summary>
		''' Closes the current subpath by drawing a straight line back to
		''' the coordinates of the last {@code moveTo}.  If the path is already
		''' closed then this method has no effect.
		''' 
		''' @since 1.6
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub closePath()
			If numTypes = 0 OrElse pointTypes(numTypes - 1) <> SEG_CLOSE Then
				needRoom(True, 0)
				pointTypes(numTypes) = SEG_CLOSE
				numTypes += 1
			End If
		End Sub

		''' <summary>
		''' Appends the geometry of the specified {@code Shape} object to the
		''' path, possibly connecting the new geometry to the existing path
		''' segments with a line segment.
		''' If the {@code connect} parameter is {@code true} and the
		''' path is not empty then any initial {@code moveTo} in the
		''' geometry of the appended {@code Shape}
		''' is turned into a {@code lineTo} segment.
		''' If the destination coordinates of such a connecting {@code lineTo}
		''' segment match the ending coordinates of a currently open
		''' subpath then the segment is omitted as superfluous.
		''' The winding rule of the specified {@code Shape} is ignored
		''' and the appended geometry is governed by the winding
		''' rule specified for this path.
		''' </summary>
		''' <param name="s"> the {@code Shape} whose geometry is appended
		'''          to this path </param>
		''' <param name="connect"> a boolean to control whether or not to turn an initial
		'''                {@code moveTo} segment into a {@code lineTo} segment
		'''                to connect the new geometry to the existing path
		''' @since 1.6 </param>
		Public Sub append(  s As java.awt.Shape,   connect As Boolean)
			append(s.getPathIterator(Nothing), connect)
		End Sub

		''' <summary>
		''' Appends the geometry of the specified
		''' <seealso cref="PathIterator"/> object
		''' to the path, possibly connecting the new geometry to the existing
		''' path segments with a line segment.
		''' If the {@code connect} parameter is {@code true} and the
		''' path is not empty then any initial {@code moveTo} in the
		''' geometry of the appended {@code Shape} is turned into a
		''' {@code lineTo} segment.
		''' If the destination coordinates of such a connecting {@code lineTo}
		''' segment match the ending coordinates of a currently open
		''' subpath then the segment is omitted as superfluous.
		''' The winding rule of the specified {@code Shape} is ignored
		''' and the appended geometry is governed by the winding
		''' rule specified for this path.
		''' </summary>
		''' <param name="pi"> the {@code PathIterator} whose geometry is appended to
		'''           this path </param>
		''' <param name="connect"> a boolean to control whether or not to turn an initial
		'''                {@code moveTo} segment into a {@code lineTo} segment
		'''                to connect the new geometry to the existing path
		''' @since 1.6 </param>
		Public MustOverride Sub append(  pi As PathIterator,   connect As Boolean)

		''' <summary>
		''' Returns the fill style winding rule.
		''' </summary>
		''' <returns> an integer representing the current winding rule. </returns>
		''' <seealso cref= #WIND_EVEN_ODD </seealso>
		''' <seealso cref= #WIND_NON_ZERO </seealso>
		''' <seealso cref= #setWindingRule
		''' @since 1.6 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property windingRule As Integer
			Get
				Return windingRule
			End Get
			Set(  rule As Integer)
				If rule <> WIND_EVEN_ODD AndAlso rule <> WIND_NON_ZERO Then Throw New IllegalArgumentException("winding rule must be " & "WIND_EVEN_ODD or " & "WIND_NON_ZERO")
				windingRule = rule
			End Set
		End Property


		''' <summary>
		''' Returns the coordinates most recently added to the end of the path
		''' as a <seealso cref="Point2D"/> object.
		''' </summary>
		''' <returns> a {@code Point2D} object containing the ending coordinates of
		'''         the path or {@code null} if there are no points in the path.
		''' @since 1.6 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property currentPoint As Point2D
			Get
				Dim index As Integer = numCoords
				If numTypes < 1 OrElse index < 1 Then Return Nothing
				If pointTypes(numTypes - 1) = SEG_CLOSE Then
				loop:
					For i As Integer = numTypes - 2 To 1 Step -1
						Select Case pointTypes(i)
						Case SEG_MOVETO
							GoTo loop
						Case SEG_LINETO
							index -= 2
						Case SEG_QUADTO
							index -= 4
						Case SEG_CUBICTO
							index -= 6
						Case SEG_CLOSE
						End Select
					Next i
				End If
				Return getPoint(index - 2)
			End Get
		End Property

		''' <summary>
		''' Resets the path to empty.  The append position is set back to the
		''' beginning of the path and all coordinates and point types are
		''' forgotten.
		''' 
		''' @since 1.6
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub reset()
				numCoords = 0
				numTypes = numCoords
		End Sub

		''' <summary>
		''' Transforms the geometry of this path using the specified
		''' <seealso cref="AffineTransform"/>.
		''' The geometry is transformed in place, which permanently changes the
		''' boundary defined by this object.
		''' </summary>
		''' <param name="at"> the {@code AffineTransform} used to transform the area
		''' @since 1.6 </param>
		Public MustOverride Sub transform(  at As AffineTransform)

		''' <summary>
		''' Returns a new {@code Shape} representing a transformed version
		''' of this {@code Path2D}.
		''' Note that the exact type and coordinate precision of the return
		''' value is not specified for this method.
		''' The method will return a Shape that contains no less precision
		''' for the transformed geometry than this {@code Path2D} currently
		''' maintains, but it may contain no more precision either.
		''' If the tradeoff of precision vs. storage size in the result is
		''' important then the convenience constructors in the
		''' <seealso cref="Path2D.Float#Path2D.Float(Shape, AffineTransform) Path2D.Float"/>
		''' and
		''' <seealso cref="Path2D.Double#Path2D.Double(Shape, AffineTransform) Path2D.Double"/>
		''' subclasses should be used to make the choice explicit.
		''' </summary>
		''' <param name="at"> the {@code AffineTransform} used to transform a
		'''           new {@code Shape}. </param>
		''' <returns> a new {@code Shape}, transformed with the specified
		'''         {@code AffineTransform}.
		''' @since 1.6 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Function createTransformedShape(  at As AffineTransform) As java.awt.Shape
			Dim p2d As Path2D = CType(clone(), Path2D)
			If at IsNot Nothing Then p2d.transform(at)
			Return p2d
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Property bounds As java.awt.Rectangle
			Get
				Return bounds2D.bounds
			End Get
		End Property

		''' <summary>
		''' Tests if the specified coordinates are inside the closed
		''' boundary of the specified <seealso cref="PathIterator"/>.
		''' <p>
		''' This method provides a basic facility for implementors of
		''' the <seealso cref="Shape"/> interface to implement support for the
		''' <seealso cref="Shape#contains(double, double)"/> method.
		''' </summary>
		''' <param name="pi"> the specified {@code PathIterator} </param>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <returns> {@code true} if the specified coordinates are inside the
		'''         specified {@code PathIterator}; {@code false} otherwise
		''' @since 1.6 </returns>
		Public Shared Function contains(  pi As PathIterator,   x As Double,   y As Double) As Boolean
			If x * 0.0 + y * 0.0 = 0.0 Then
	'             N * 0.0 is 0.0 only if N is finite.
	'             * Here we know that both x and y are finite.
	'             
				Dim mask As Integer = (If(pi.windingRule = WIND_NON_ZERO, -1, 1))
				Dim cross As Integer = sun.awt.geom.Curve.pointCrossingsForPath(pi, x, y)
				Return ((cross And mask) <> 0)
			Else
	'             Either x or y was infinite or NaN.
	'             * A NaN always produces a negative response to any test
	'             * and Infinity values cannot be "inside" any path so
	'             * they should return false as well.
	'             
				Return False
			End If
		End Function

		''' <summary>
		''' Tests if the specified <seealso cref="Point2D"/> is inside the closed
		''' boundary of the specified <seealso cref="PathIterator"/>.
		''' <p>
		''' This method provides a basic facility for implementors of
		''' the <seealso cref="Shape"/> interface to implement support for the
		''' <seealso cref="Shape#contains(Point2D)"/> method.
		''' </summary>
		''' <param name="pi"> the specified {@code PathIterator} </param>
		''' <param name="p"> the specified {@code Point2D} </param>
		''' <returns> {@code true} if the specified coordinates are inside the
		'''         specified {@code PathIterator}; {@code false} otherwise
		''' @since 1.6 </returns>
		Public Shared Function contains(  pi As PathIterator,   p As Point2D) As Boolean
			Return contains(pi, p.x, p.y)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Function contains(  x As Double,   y As Double) As Boolean
			If x * 0.0 + y * 0.0 = 0.0 Then
	'             N * 0.0 is 0.0 only if N is finite.
	'             * Here we know that both x and y are finite.
	'             
				If numTypes < 2 Then Return False
				Dim mask As Integer = (If(windingRule = WIND_NON_ZERO, -1, 1))
				Return ((pointCrossings(x, y) And mask) <> 0)
			Else
	'             Either x or y was infinite or NaN.
	'             * A NaN always produces a negative response to any test
	'             * and Infinity values cannot be "inside" any path so
	'             * they should return false as well.
	'             
				Return False
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Function contains(  p As Point2D) As Boolean
			Return contains(p.x, p.y)
		End Function

		''' <summary>
		''' Tests if the specified rectangular area is entirely inside the
		''' closed boundary of the specified <seealso cref="PathIterator"/>.
		''' <p>
		''' This method provides a basic facility for implementors of
		''' the <seealso cref="Shape"/> interface to implement support for the
		''' <seealso cref="Shape#contains(double, double, double, double)"/> method.
		''' <p>
		''' This method object may conservatively return false in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such segments could lie entirely within the interior of the
		''' path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		''' winding rule or if the segments are retraced in the reverse
		''' direction such that the two sets of segments cancel each
		''' other out without any exterior area falling between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' </summary>
		''' <param name="pi"> the specified {@code PathIterator} </param>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <param name="w"> the width of the specified rectangular area </param>
		''' <param name="h"> the height of the specified rectangular area </param>
		''' <returns> {@code true} if the specified {@code PathIterator} contains
		'''         the specified rectangular area; {@code false} otherwise.
		''' @since 1.6 </returns>
		Public Shared Function contains(  pi As PathIterator,   x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If java.lang.[Double].IsNaN(x+w) OrElse java.lang.[Double].IsNaN(y+h) Then Return False
			If w <= 0 OrElse h <= 0 Then Return False
			Dim mask As Integer = (If(pi.windingRule = WIND_NON_ZERO, -1, 2))
			Dim crossings As Integer = sun.awt.geom.Curve.rectCrossingsForPath(pi, x, y, x+w, y+h)
			Return (crossings <> sun.awt.geom.Curve.RECT_INTERSECTS AndAlso (crossings And mask) <> 0)
		End Function

		''' <summary>
		''' Tests if the specified <seealso cref="Rectangle2D"/> is entirely inside the
		''' closed boundary of the specified <seealso cref="PathIterator"/>.
		''' <p>
		''' This method provides a basic facility for implementors of
		''' the <seealso cref="Shape"/> interface to implement support for the
		''' <seealso cref="Shape#contains(Rectangle2D)"/> method.
		''' <p>
		''' This method object may conservatively return false in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such segments could lie entirely within the interior of the
		''' path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		''' winding rule or if the segments are retraced in the reverse
		''' direction such that the two sets of segments cancel each
		''' other out without any exterior area falling between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' </summary>
		''' <param name="pi"> the specified {@code PathIterator} </param>
		''' <param name="r"> a specified {@code Rectangle2D} </param>
		''' <returns> {@code true} if the specified {@code PathIterator} contains
		'''         the specified {@code Rectangle2D}; {@code false} otherwise.
		''' @since 1.6 </returns>
		Public Shared Function contains(  pi As PathIterator,   r As Rectangle2D) As Boolean
			Return contains(pi, r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' This method object may conservatively return false in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such segments could lie entirely within the interior of the
		''' path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		''' winding rule or if the segments are retraced in the reverse
		''' direction such that the two sets of segments cancel each
		''' other out without any exterior area falling between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' 
		''' @since 1.6
		''' </summary>
		Public Function contains(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If java.lang.[Double].IsNaN(x+w) OrElse java.lang.[Double].IsNaN(y+h) Then Return False
			If w <= 0 OrElse h <= 0 Then Return False
			Dim mask As Integer = (If(windingRule = WIND_NON_ZERO, -1, 2))
			Dim crossings As Integer = rectCrossings(x, y, x+w, y+h)
			Return (crossings <> sun.awt.geom.Curve.RECT_INTERSECTS AndAlso (crossings And mask) <> 0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' This method object may conservatively return false in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such segments could lie entirely within the interior of the
		''' path if they are part of a path with a <seealso cref="#WIND_NON_ZERO"/>
		''' winding rule or if the segments are retraced in the reverse
		''' direction such that the two sets of segments cancel each
		''' other out without any exterior area falling between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' 
		''' @since 1.6
		''' </summary>
		Public Function contains(  r As Rectangle2D) As Boolean
			Return contains(r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' Tests if the interior of the specified <seealso cref="PathIterator"/>
		''' intersects the interior of a specified set of rectangular
		''' coordinates.
		''' <p>
		''' This method provides a basic facility for implementors of
		''' the <seealso cref="Shape"/> interface to implement support for the
		''' <seealso cref="Shape#intersects(double, double, double, double)"/> method.
		''' <p>
		''' This method object may conservatively return true in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such a case may occur if some set of segments of the
		''' path are retraced in the reverse direction such that the
		''' two sets of segments cancel each other out without any
		''' interior area between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' </summary>
		''' <param name="pi"> the specified {@code PathIterator} </param>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <param name="w"> the width of the specified rectangular coordinates </param>
		''' <param name="h"> the height of the specified rectangular coordinates </param>
		''' <returns> {@code true} if the specified {@code PathIterator} and
		'''         the interior of the specified set of rectangular
		'''         coordinates intersect each other; {@code false} otherwise.
		''' @since 1.6 </returns>
		Public Shared Function intersects(  pi As PathIterator,   x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If java.lang.[Double].IsNaN(x+w) OrElse java.lang.[Double].IsNaN(y+h) Then Return False
			If w <= 0 OrElse h <= 0 Then Return False
			Dim mask As Integer = (If(pi.windingRule = WIND_NON_ZERO, -1, 2))
			Dim crossings As Integer = sun.awt.geom.Curve.rectCrossingsForPath(pi, x, y, x+w, y+h)
			Return (crossings = sun.awt.geom.Curve.RECT_INTERSECTS OrElse (crossings And mask) <> 0)
		End Function

		''' <summary>
		''' Tests if the interior of the specified <seealso cref="PathIterator"/>
		''' intersects the interior of a specified <seealso cref="Rectangle2D"/>.
		''' <p>
		''' This method provides a basic facility for implementors of
		''' the <seealso cref="Shape"/> interface to implement support for the
		''' <seealso cref="Shape#intersects(Rectangle2D)"/> method.
		''' <p>
		''' This method object may conservatively return true in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such a case may occur if some set of segments of the
		''' path are retraced in the reverse direction such that the
		''' two sets of segments cancel each other out without any
		''' interior area between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' </summary>
		''' <param name="pi"> the specified {@code PathIterator} </param>
		''' <param name="r"> the specified {@code Rectangle2D} </param>
		''' <returns> {@code true} if the specified {@code PathIterator} and
		'''         the interior of the specified {@code Rectangle2D}
		'''         intersect each other; {@code false} otherwise.
		''' @since 1.6 </returns>
		Public Shared Function intersects(  pi As PathIterator,   r As Rectangle2D) As Boolean
			Return intersects(pi, r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' This method object may conservatively return true in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such a case may occur if some set of segments of the
		''' path are retraced in the reverse direction such that the
		''' two sets of segments cancel each other out without any
		''' interior area between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' 
		''' @since 1.6
		''' </summary>
		Public Function intersects(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If java.lang.[Double].IsNaN(x+w) OrElse java.lang.[Double].IsNaN(y+h) Then Return False
			If w <= 0 OrElse h <= 0 Then Return False
			Dim mask As Integer = (If(windingRule = WIND_NON_ZERO, -1, 2))
			Dim crossings As Integer = rectCrossings(x, y, x+w, y+h)
			Return (crossings = sun.awt.geom.Curve.RECT_INTERSECTS OrElse (crossings And mask) <> 0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' This method object may conservatively return true in
		''' cases where the specified rectangular area intersects a
		''' segment of the path, but that segment does not represent a
		''' boundary between the interior and exterior of the path.
		''' Such a case may occur if some set of segments of the
		''' path are retraced in the reverse direction such that the
		''' two sets of segments cancel each other out without any
		''' interior area between them.
		''' To determine whether segments represent true boundaries of
		''' the interior of the path would require extensive calculations
		''' involving all of the segments of the path and the winding
		''' rule and are thus beyond the scope of this implementation.
		''' 
		''' @since 1.6
		''' </summary>
		Public Function intersects(  r As Rectangle2D) As Boolean
			Return intersects(r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The iterator for this class is not multi-threaded safe,
		''' which means that this {@code Path2D} class does not
		''' guarantee that modifications to the geometry of this
		''' {@code Path2D} object do not affect any iterations of
		''' that geometry that are already in process.
		''' 
		''' @since 1.6
		''' </summary>
		Public Function getPathIterator(  at As AffineTransform,   flatness As Double) As PathIterator
			Return New FlatteningPathIterator(getPathIterator(at), flatness)
		End Function

		''' <summary>
		''' Creates a new object of the same class as this object.
		''' </summary>
		''' <returns>     a clone of this instance. </returns>
		''' <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		''' <seealso cref=        java.lang.Cloneable
		''' @since      1.6 </seealso>
		Public MustOverride Function clone() As Object
			' Note: It would be nice to have this return Path2D
			' but one of our subclasses (GeneralPath) needs to
			' offer "public Object clone()" for backwards
			' compatibility so we cannot restrict it further.
			' REMIND: Can we do both somehow?

	'    
	'     * Support fields and methods for serializing the subclasses.
	'     
		Private Const SERIAL_STORAGE_FLT_ARRAY As SByte = &H30
		Private Const SERIAL_STORAGE_DBL_ARRAY As SByte = &H31

		Private Const SERIAL_SEG_FLT_MOVETO As SByte = &H40
		Private Const SERIAL_SEG_FLT_LINETO As SByte = &H41
		Private Const SERIAL_SEG_FLT_QUADTO As SByte = &H42
		Private Const SERIAL_SEG_FLT_CUBICTO As SByte = &H43

		Private Const SERIAL_SEG_DBL_MOVETO As SByte = &H50
		Private Const SERIAL_SEG_DBL_LINETO As SByte = &H51
		Private Const SERIAL_SEG_DBL_QUADTO As SByte = &H52
		Private Const SERIAL_SEG_DBL_CUBICTO As SByte = &H53

		Private Const SERIAL_SEG_CLOSE As SByte = &H60
		Private Const SERIAL_PATH_END As SByte = &H61

		Friend Sub writeObject(  s As java.io.ObjectOutputStream,   isdbl As Boolean)
			s.defaultWriteObject()

			Dim fCoords As Single()
			Dim dCoords As Double()

			If isdbl Then
				dCoords = CType(Me, Path2D.Double).doubleCoords
				fCoords = Nothing
			Else
				fCoords = CType(Me, Path2D.Float).floatCoords
				dCoords = Nothing
			End If

			Dim numTypes As Integer = Me.numTypes

			s.writeByte(If(isdbl, SERIAL_STORAGE_DBL_ARRAY, SERIAL_STORAGE_FLT_ARRAY))
			s.writeInt(numTypes)
			s.writeInt(numCoords)
			s.writeByte(CByte(windingRule))

			Dim cindex As Integer = 0
			For i As Integer = 0 To numTypes - 1
				Dim npoints As Integer
				Dim serialtype As SByte
				Select Case pointTypes(i)
				Case SEG_MOVETO
					npoints = 1
					serialtype = (If(isdbl, SERIAL_SEG_DBL_MOVETO, SERIAL_SEG_FLT_MOVETO))
				Case SEG_LINETO
					npoints = 1
					serialtype = (If(isdbl, SERIAL_SEG_DBL_LINETO, SERIAL_SEG_FLT_LINETO))
				Case SEG_QUADTO
					npoints = 2
					serialtype = (If(isdbl, SERIAL_SEG_DBL_QUADTO, SERIAL_SEG_FLT_QUADTO))
				Case SEG_CUBICTO
					npoints = 3
					serialtype = (If(isdbl, SERIAL_SEG_DBL_CUBICTO, SERIAL_SEG_FLT_CUBICTO))
				Case SEG_CLOSE
					npoints = 0
					serialtype = SERIAL_SEG_CLOSE

				Case Else
					' Should never happen
					Throw New InternalError("unrecognized path type")
				End Select
				s.writeByte(serialtype)
				npoints -= 1
				Do While npoints >= 0
					If isdbl Then
						s.writeDouble(dCoords(cindex))
						cindex += 1
						s.writeDouble(dCoords(cindex))
						cindex += 1
					Else
						s.writeFloat(fCoords(cindex))
						cindex += 1
						s.writeFloat(fCoords(cindex))
						cindex += 1
					End If
					npoints -= 1
				Loop
			Next i
			s.writeByte(SERIAL_PATH_END)
		End Sub

		Friend Sub readObject(  s As java.io.ObjectInputStream,   storedbl As Boolean)
			s.defaultReadObject()

			' The subclass calls this method with the storage type that
			' they want us to use (storedbl) so we ignore the storage
			' method hint from the stream.
			s.readByte()
			Dim nT As Integer = s.readInt()
			Dim nC As Integer = s.readInt()
			Try
				windingRule = s.readByte()
			Catch iae As IllegalArgumentException
				Throw New java.io.InvalidObjectException(iae.Message)
			End Try

			pointTypes = New SByte(If(nT < 0, INIT_SIZE, nT) - 1){}
			If nC < 0 Then nC = INIT_SIZE * 2
			If storedbl Then
				CType(Me, Path2D.Double).doubleCoords = New Double(nC - 1){}
			Else
				CType(Me, Path2D.Float).floatCoords = New Single(nC - 1){}
			End If

		PATHDONE:
			Dim i As Integer = 0
			Do While nT < 0 OrElse i < nT
				Dim isdbl As Boolean
				Dim npoints As Integer
				Dim segtype As SByte

				Dim serialtype As SByte = s.readByte()
				Select Case serialtype
				Case SERIAL_SEG_FLT_MOVETO
					isdbl = False
					npoints = 1
					segtype = SEG_MOVETO
				Case SERIAL_SEG_FLT_LINETO
					isdbl = False
					npoints = 1
					segtype = SEG_LINETO
				Case SERIAL_SEG_FLT_QUADTO
					isdbl = False
					npoints = 2
					segtype = SEG_QUADTO
				Case SERIAL_SEG_FLT_CUBICTO
					isdbl = False
					npoints = 3
					segtype = SEG_CUBICTO

				Case SERIAL_SEG_DBL_MOVETO
					isdbl = True
					npoints = 1
					segtype = SEG_MOVETO
				Case SERIAL_SEG_DBL_LINETO
					isdbl = True
					npoints = 1
					segtype = SEG_LINETO
				Case SERIAL_SEG_DBL_QUADTO
					isdbl = True
					npoints = 2
					segtype = SEG_QUADTO
				Case SERIAL_SEG_DBL_CUBICTO
					isdbl = True
					npoints = 3
					segtype = SEG_CUBICTO

				Case SERIAL_SEG_CLOSE
					isdbl = False
					npoints = 0
					segtype = SEG_CLOSE

				Case SERIAL_PATH_END
					If nT < 0 Then GoTo PATHDONE
					Throw New java.io.StreamCorruptedException("unexpected PATH_END")

				Case Else
					Throw New java.io.StreamCorruptedException("unrecognized path type")
				End Select
				needRoom(segtype <> SEG_MOVETO, npoints * 2)
				If isdbl Then
					npoints -= 1
					Do While npoints >= 0
						append(s.readDouble(), s.readDouble())
						npoints -= 1
					Loop
				Else
					npoints -= 1
					Do While npoints >= 0
						append(s.readFloat(), s.readFloat())
						npoints -= 1
					Loop
				End If
				pointTypes(numTypes) = segtype
				numTypes += 1
				i += 1
			Loop
			If nT >= 0 AndAlso s.readByte() <> SERIAL_PATH_END Then Throw New java.io.StreamCorruptedException("missing PATH_END")
		End Sub

		Friend MustInherit Class [Iterator]
			Implements PathIterator

				Public MustOverride Function currentSegment(  coords As Double()) As Integer Implements PathIterator.currentSegment
				Public MustOverride Function currentSegment(  coords As Single()) As Integer Implements PathIterator.currentSegment
			Friend typeIdx As Integer
			Friend pointIdx As Integer
			Friend path As Path2D

			Friend Shared ReadOnly curvecoords As Integer() = {2, 2, 4, 6, 0}

			Friend Sub New(  path As Path2D)
				Me.path = path
			End Sub

			Public Overridable Property windingRule As Integer Implements PathIterator.getWindingRule
				Get
					Return path.windingRule
				End Get
			End Property

			Public Overridable Property done As Boolean Implements PathIterator.isDone
				Get
					Return (typeIdx >= path.numTypes)
				End Get
			End Property

			Public Overridable Sub [next]() Implements PathIterator.next
				Dim type As Integer = path.pointTypes(typeIdx)
				typeIdx += 1
				pointIdx += curvecoords(type)
			End Sub
		End Class
	End Class

End Namespace
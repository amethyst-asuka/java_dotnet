Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' Map is used to represent a map element that is part of an HTML document.
	''' Once a Map has been created, and any number of areas have been added,
	''' you can test if a point falls inside the map via the contains method.
	''' 
	''' @author  Scott Violet
	''' </summary>
	<Serializable> _
	Friend Class Map
		''' <summary>
		''' Name of the Map. </summary>
		Private name As String
		''' <summary>
		''' An array of AttributeSets. </summary>
		Private areaAttributes As List(Of javax.swing.text.AttributeSet)
		''' <summary>
		''' An array of RegionContainments, will slowly grow to match the
		''' length of areaAttributes as needed. 
		''' </summary>
		Private areas As List(Of RegionContainment)

		Public Sub New()
		End Sub

		Public Sub New(ByVal name As String)
			Me.name = name
		End Sub

		''' <summary>
		''' Returns the name of the Map.
		''' </summary>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Defines a region of the Map, based on the passed in AttributeSet.
		''' </summary>
		Public Overridable Sub addArea(ByVal [as] As javax.swing.text.AttributeSet)
			If [as] Is Nothing Then Return
			If areaAttributes Is Nothing Then areaAttributes = New List(Of javax.swing.text.AttributeSet)(2)
			areaAttributes.Add([as].copyAttributes())
		End Sub

		''' <summary>
		''' Removes the previously created area.
		''' </summary>
		Public Overridable Sub removeArea(ByVal [as] As javax.swing.text.AttributeSet)
			If [as] IsNot Nothing AndAlso areaAttributes IsNot Nothing Then
				Dim numAreas As Integer = If(areas IsNot Nothing, areas.Count, 0)
				For counter As Integer = areaAttributes.Count - 1 To 0 Step -1
					If areaAttributes(counter).isEqual([as]) Then
						areaAttributes.RemoveAt(counter)
						If counter < numAreas Then areas.RemoveAt(counter)
					End If
				Next counter
			End If
		End Sub

		''' <summary>
		''' Returns the AttributeSets representing the differet areas of the Map.
		''' </summary>
		Public Overridable Property areas As javax.swing.text.AttributeSet()
			Get
				Dim numAttributes As Integer = If(areaAttributes IsNot Nothing, areaAttributes.Count, 0)
				If numAttributes <> 0 Then
					Dim retValue As javax.swing.text.AttributeSet() = New javax.swing.text.AttributeSet(numAttributes - 1){}
    
					areaAttributes.CopyTo(retValue)
					Return retValue
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the AttributeSet that contains the passed in location,
		''' <code>x</code>, <code>y</code>. <code>width</code>, <code>height</code>
		''' gives the size of the region the map is defined over. If a matching
		''' area is found, the AttribueSet for it is returned.
		''' </summary>
		Public Overridable Function getArea(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As javax.swing.text.AttributeSet
			Dim numAttributes As Integer = If(areaAttributes IsNot Nothing, areaAttributes.Count, 0)

			If numAttributes > 0 Then
				Dim numAreas As Integer = If(areas IsNot Nothing, areas.Count, 0)

				If areas Is Nothing Then areas = New List(Of RegionContainment)(numAttributes)
				For counter As Integer = 0 To numAttributes - 1
					If counter >= numAreas Then areas.Add(createRegionContainment(areaAttributes(counter)))
					Dim rc As RegionContainment = areas(counter)
					If rc IsNot Nothing AndAlso rc.contains(x, y, width, height) Then Return areaAttributes(counter)
				Next counter
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Creates and returns an instance of RegionContainment that can be
		''' used to test if a particular point lies inside a region.
		''' </summary>
		Protected Friend Overridable Function createRegionContainment(ByVal attributes As javax.swing.text.AttributeSet) As RegionContainment
			Dim shape As Object = attributes.getAttribute(HTML.Attribute.SHAPE)

			If shape Is Nothing Then shape = "rect"
			If TypeOf shape Is String Then
				Dim shapeString As String = CStr(shape).ToLower()
				Dim rc As RegionContainment = Nothing

				Try
					If shapeString.Equals("rect") Then
						rc = New RectangleRegionContainment(attributes)
					ElseIf shapeString.Equals("circle") Then
						rc = New CircleRegionContainment(attributes)
					ElseIf shapeString.Equals("poly") Then
						rc = New PolygonRegionContainment(attributes)
					ElseIf shapeString.Equals("default") Then
						rc = DefaultRegionContainment.sharedInstance()
					End If
				Catch re As Exception
					' Something wrong with attributes.
					rc = Nothing
				End Try
				Return rc
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Creates and returns an array of integers from the String
		''' <code>stringCoords</code>. If one of the values represents a
		''' % the returned value with be negative. If a parse error results
		''' from trying to parse one of the numbers null is returned.
		''' </summary>
		Protected Friend Shared Function extractCoords(ByVal stringCoords As Object) As Integer()
			If stringCoords Is Nothing OrElse Not(TypeOf stringCoords Is String) Then Return Nothing

			Dim st As New java.util.StringTokenizer(CStr(stringCoords), ", " & vbTab & vbLf & vbCr)
			Dim retValue As Integer() = Nothing
			Dim numCoords As Integer = 0

			Do While st.hasMoreElements()
				Dim token As String = st.nextToken()
				Dim scale As Integer

				If token.EndsWith("%") Then
					scale = -1
					token = token.Substring(0, token.Length - 1)
				Else
					scale = 1
				End If
				Try
					Dim intValue As Integer = Convert.ToInt32(token)

					If retValue Is Nothing Then
						retValue = New Integer(3){}
					ElseIf numCoords = retValue.Length Then
						Dim temp As Integer() = New Integer(retValue.Length * 2 - 1){}

						Array.Copy(retValue, 0, temp, 0, retValue.Length)
						retValue = temp
					End If
					retValue(numCoords) = intValue * scale
					numCoords += 1
				Catch nfe As NumberFormatException
					Return Nothing
				End Try
			Loop
			If numCoords > 0 AndAlso numCoords <> retValue.Length Then
				Dim temp As Integer() = New Integer(numCoords - 1){}

				Array.Copy(retValue, 0, temp, 0, numCoords)
				retValue = temp
			End If
			Return retValue
		End Function


		''' <summary>
		''' Defines the interface used for to check if a point is inside a
		''' region.
		''' </summary>
		Friend Interface RegionContainment
			''' <summary>
			''' Returns true if the location <code>x</code>, <code>y</code>
			''' falls inside the region defined in the receiver.
			''' <code>width</code>, <code>height</code> is the size of
			''' the enclosing region.
			''' </summary>
			Function contains(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
		End Interface


		''' <summary>
		''' Used to test for containment in a rectangular region.
		''' </summary>
		Friend Class RectangleRegionContainment
			Implements RegionContainment

			''' <summary>
			''' Will be non-null if one of the values is a percent, and any value
			''' that is non null indicates it is a percent
			''' (order is x, y, width, height). 
			''' </summary>
			Friend percents As Single()
			''' <summary>
			''' Last value of width passed in. </summary>
			Friend lastWidth As Integer
			''' <summary>
			''' Last value of height passed in. </summary>
			Friend lastHeight As Integer
			''' <summary>
			''' Top left. </summary>
			Friend x0 As Integer
			Friend y0 As Integer
			''' <summary>
			''' Bottom right. </summary>
			Friend x1 As Integer
			Friend y1 As Integer

			Public Sub New(ByVal [as] As javax.swing.text.AttributeSet)
				Dim coords As Integer() = Map.extractCoords([as].getAttribute(HTML.Attribute.COORDS))

				percents = Nothing
				If coords Is Nothing OrElse coords.Length <> 4 Then
					Throw New Exception("Unable to parse rectangular area")
				Else
					x0 = coords(0)
					y0 = coords(1)
					x1 = coords(2)
					y1 = coords(3)
					If x0 < 0 OrElse y0 < 0 OrElse x1 < 0 OrElse y1 < 0 Then
						percents = New Single(3){}
							lastHeight = -1
							lastWidth = lastHeight
						For counter As Integer = 0 To 3
							If coords(counter) < 0 Then
								percents(counter) = Math.Abs(coords(counter)) / 100.0f
							Else
								percents(counter) = -1.0f
							End If
						Next counter
					End If
				End If
			End Sub

			Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
				If percents Is Nothing Then Return contains(x, y)
				If lastWidth <> width OrElse lastHeight <> height Then
					lastWidth = width
					lastHeight = height
					If percents(0) <> -1.0f Then x0 = CInt(Fix(percents(0) * width))
					If percents(1) <> -1.0f Then y0 = CInt(Fix(percents(1) * height))
					If percents(2) <> -1.0f Then x1 = CInt(Fix(percents(2) * width))
					If percents(3) <> -1.0f Then y1 = CInt(Fix(percents(3) * height))
				End If
				Return contains(x, y)
			End Function

			Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
				Return ((x >= x0 AndAlso x <= x1) AndAlso (y >= y0 AndAlso y <= y1))
			End Function
		End Class


		''' <summary>
		''' Used to test for containment in a polygon region.
		''' </summary>
		Friend Class PolygonRegionContainment
			Inherits java.awt.Polygon
			Implements RegionContainment

			''' <summary>
			''' If any value is a percent there will be an entry here for the
			''' percent value. Use percentIndex to find out the index for it. 
			''' </summary>
			Friend percentValues As Single()
			Friend percentIndexs As Integer()
			''' <summary>
			''' Last value of width passed in. </summary>
			Friend lastWidth As Integer
			''' <summary>
			''' Last value of height passed in. </summary>
			Friend lastHeight As Integer

			Public Sub New(ByVal [as] As javax.swing.text.AttributeSet)
				Dim coords As Integer() = Map.extractCoords([as].getAttribute(HTML.Attribute.COORDS))

				If coords Is Nothing OrElse coords.Length = 0 OrElse coords.Length Mod 2 <> 0 Then
					Throw New Exception("Unable to parse polygon area")
				Else
					Dim numPercents As Integer = 0

						lastHeight = -1
						lastWidth = lastHeight
					For counter As Integer = coords.Length - 1 To 0 Step -1
						If coords(counter) < 0 Then numPercents += 1
					Next counter

					If numPercents > 0 Then
						percentIndexs = New Integer(numPercents - 1){}
						percentValues = New Single(numPercents - 1){}
						Dim counter As Integer = coords.Length - 1
						Dim pCounter As Integer = 0
						Do While counter >= 0
							If coords(counter) < 0 Then
								percentValues(pCounter) = coords(counter) / -100.0f
								percentIndexs(pCounter) = counter
								pCounter += 1
							End If
							counter -= 1
						Loop
					Else
						percentIndexs = Nothing
						percentValues = Nothing
					End If
					npoints = coords.Length \ 2
					xpoints = New Integer(npoints - 1){}
					ypoints = New Integer(npoints - 1){}

					For counter As Integer = 0 To npoints - 1
						xpoints(counter) = coords(counter + counter)
						ypoints(counter) = coords(counter + counter + 1)
					Next counter
				End If
			End Sub

			Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
				If percentValues Is Nothing OrElse (lastWidth = width AndAlso lastHeight = height) Then Return contains(x, y)
				' Force the bounding box to be recalced.
				bounds = Nothing
				lastWidth = width
				lastHeight = height
				Dim fWidth As Single = CSng(width)
				Dim fHeight As Single = CSng(height)
				For counter As Integer = percentValues.Length - 1 To 0 Step -1
					If percentIndexs(counter) Mod 2 = 0 Then
						' x
						xpoints(percentIndexs(counter) \ 2) = CInt(Fix(percentValues(counter) * fWidth))
					Else
						' y
						ypoints(percentIndexs(counter) \ 2) = CInt(Fix(percentValues(counter) * fHeight))
					End If
				Next counter
				Return contains(x, y)
			End Function
		End Class


		''' <summary>
		''' Used to test for containment in a circular region.
		''' </summary>
		Friend Class CircleRegionContainment
			Implements RegionContainment

			''' <summary>
			''' X origin of the circle. </summary>
			Friend x As Integer
			''' <summary>
			''' Y origin of the circle. </summary>
			Friend y As Integer
			''' <summary>
			''' Radius of the circle. </summary>
			Friend radiusSquared As Integer
			''' <summary>
			''' Non-null indicates one of the values represents a percent. </summary>
			Friend percentValues As Single()
			''' <summary>
			''' Last value of width passed in. </summary>
			Friend lastWidth As Integer
			''' <summary>
			''' Last value of height passed in. </summary>
			Friend lastHeight As Integer

			Public Sub New(ByVal [as] As javax.swing.text.AttributeSet)
				Dim coords As Integer() = Map.extractCoords([as].getAttribute(HTML.Attribute.COORDS))

				If coords Is Nothing OrElse coords.Length <> 3 Then Throw New Exception("Unable to parse circular area")
				x = coords(0)
				y = coords(1)
				radiusSquared = coords(2) * coords(2)
				If coords(0) < 0 OrElse coords(1) < 0 OrElse coords(2) < 0 Then
						lastHeight = -1
						lastWidth = lastHeight
					percentValues = New Single(2){}
					For counter As Integer = 0 To 2
						If coords(counter) < 0 Then
							percentValues(counter) = coords(counter) / -100.0f
						Else
							percentValues(counter) = -1.0f
						End If
					Next counter
				Else
					percentValues = Nothing
				End If
			End Sub

			Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
				If percentValues IsNot Nothing AndAlso (lastWidth <> width OrElse lastHeight <> height) Then
					Dim newRad As Integer = Math.Min(width, height) / 2

					lastWidth = width
					lastHeight = height
					If percentValues(0) <> -1.0f Then Me.x = CInt(Fix(percentValues(0) * width))
					If percentValues(1) <> -1.0f Then Me.y = CInt(Fix(percentValues(1) * height))
					If percentValues(2) <> -1.0f Then
						radiusSquared = CInt(Fix(percentValues(2) * Math.Min(width, height)))
						radiusSquared *= radiusSquared
					End If
				End If
				Return (((x - Me.x) * (x - Me.x) + (y - Me.y) * (y - Me.y)) <= radiusSquared)
			End Function
		End Class


		''' <summary>
		''' An implementation that will return true if the x, y location is
		''' inside a rectangle defined by origin 0, 0, and width equal to
		''' width passed in, and height equal to height passed in.
		''' </summary>
		Friend Class DefaultRegionContainment
			Implements RegionContainment

			''' <summary>
			''' A global shared instance. </summary>
			Friend Shared si As DefaultRegionContainment = Nothing

			Public Shared Function sharedInstance() As DefaultRegionContainment
				If si Is Nothing Then si = New DefaultRegionContainment
				Return si
			End Function

			Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
				Return (x <= width AndAlso x >= 0 AndAlso y >= 0 AndAlso y <= width)
			End Function
		End Class
	End Class

End Namespace
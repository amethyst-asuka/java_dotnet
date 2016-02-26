Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.Threading
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
	''' A collection of utility methods for Swing.
	''' 
	''' @author unknown
	''' </summary>
	Public Class SwingUtilities
		Implements SwingConstants

		' These states are system-wide, rather than AppContext wide.
		Private Shared canAccessEventQueue As Boolean = False
		Private Shared eventQueueTested As Boolean = False

		''' <summary>
		''' Indicates if we should change the drop target when a
		''' {@code TransferHandler} is set.
		''' </summary>
		Private Shared suppressDropSupport As Boolean

		''' <summary>
		''' Indiciates if we've checked the system property for suppressing
		''' drop support.
		''' </summary>
		Private Shared checkedSuppressDropSupport As Boolean


		''' <summary>
		''' Returns true if <code>setTransferHandler</code> should change the
		''' <code>DropTarget</code>.
		''' </summary>
		Private Property Shared suppressDropTarget As Boolean
			Get
				If Not checkedSuppressDropSupport Then
					suppressDropSupport = Convert.ToBoolean(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("suppressSwingDropSupport")))
					checkedSuppressDropSupport = True
				End If
				Return suppressDropSupport
			End Get
		End Property

		''' <summary>
		''' Installs a {@code DropTarget} on the component as necessary for a
		''' {@code TransferHandler} change.
		''' </summary>
		Friend Shared Sub installSwingDropTargetAsNecessary(ByVal c As Component, ByVal t As TransferHandler)

			If Not suppressDropTarget Then
				Dim dropHandler As java.awt.dnd.DropTarget = c.dropTarget
				If (dropHandler Is Nothing) OrElse (TypeOf dropHandler Is javax.swing.plaf.UIResource) Then
					If t Is Nothing Then
						c.dropTarget = Nothing
					ElseIf Not GraphicsEnvironment.headless Then
						c.dropTarget = New TransferHandler.SwingDropTarget(c)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Return true if <code>a</code> contains <code>b</code>
		''' </summary>
		Public Shared Function isRectangleContainingRectangle(ByVal a As Rectangle, ByVal b As Rectangle) As Boolean
			Return b.x >= a.x AndAlso (b.x + b.width) <= (a.x + a.width) AndAlso b.y >= a.y AndAlso (b.y + b.height) <= (a.y + a.height)
		End Function

		''' <summary>
		''' Return the rectangle (0,0,bounds.width,bounds.height) for the component <code>aComponent</code>
		''' </summary>
		Public Shared Function getLocalBounds(ByVal aComponent As Component) As Rectangle
			Dim b As New Rectangle(aComponent.bounds)
				b.y = 0
				b.x = b.y
			Return b
		End Function


		''' <summary>
		''' Returns the first <code>Window </code> ancestor of <code>c</code>, or
		''' {@code null} if <code>c</code> is not contained inside a <code>Window</code>.
		''' </summary>
		''' <param name="c"> <code>Component</code> to get <code>Window</code> ancestor
		'''        of. </param>
		''' <returns> the first <code>Window </code> ancestor of <code>c</code>, or
		'''         {@code null} if <code>c</code> is not contained inside a
		'''         <code>Window</code>.
		''' @since 1.3 </returns>
		Public Shared Function getWindowAncestor(ByVal c As Component) As Window
			Dim p As Container = c.parent
			Do While p IsNot Nothing
				If TypeOf p Is Window Then Return CType(p, Window)
				p = p.parent
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Converts the location <code>x</code> <code>y</code> to the
		''' parents coordinate system, returning the location.
		''' </summary>
		Friend Shared Function convertScreenLocationToParent(ByVal parent As Container, ByVal x As Integer, ByVal y As Integer) As Point
			Dim p As Container = parent
			Do While p IsNot Nothing
				If TypeOf p Is Window Then
					Dim point As New Point(x, y)

					SwingUtilities.convertPointFromScreen(point, parent)
					Return point
				End If
				p = p.parent
			Loop
			Throw New Exception("convertScreenLocationToParent: no window ancestor")
		End Function

		''' <summary>
		''' Convert a <code>aPoint</code> in <code>source</code> coordinate system to
		''' <code>destination</code> coordinate system.
		''' If <code>source</code> is {@code null}, <code>aPoint</code> is assumed to be in <code>destination</code>'s
		''' root component coordinate system.
		''' If <code>destination</code> is {@code null}, <code>aPoint</code> will be converted to <code>source</code>'s
		''' root component coordinate system.
		''' If both <code>source</code> and <code>destination</code> are {@code null}, return <code>aPoint</code>
		''' without any conversion.
		''' </summary>
		Public Shared Function convertPoint(ByVal source As Component, ByVal aPoint As Point, ByVal destination As Component) As Point
			Dim p As Point

			If source Is Nothing AndAlso destination Is Nothing Then Return aPoint
			If source Is Nothing Then
				source = getWindowAncestor(destination)
				If source Is Nothing Then Throw New Exception("Source component not connected to component tree hierarchy")
			End If
			p = New Point(aPoint)
			convertPointToScreen(p,source)
			If destination Is Nothing Then
				destination = getWindowAncestor(source)
				If destination Is Nothing Then Throw New Exception("Destination component not connected to component tree hierarchy")
			End If
			convertPointFromScreen(p,destination)
			Return p
		End Function

		''' <summary>
		''' Convert the point <code>(x,y)</code> in <code>source</code> coordinate system to
		''' <code>destination</code> coordinate system.
		''' If <code>source</code> is {@code null}, <code>(x,y)</code> is assumed to be in <code>destination</code>'s
		''' root component coordinate system.
		''' If <code>destination</code> is {@code null}, <code>(x,y)</code> will be converted to <code>source</code>'s
		''' root component coordinate system.
		''' If both <code>source</code> and <code>destination</code> are {@code null}, return <code>(x,y)</code>
		''' without any conversion.
		''' </summary>
		Public Shared Function convertPoint(ByVal source As Component, ByVal x As Integer, ByVal y As Integer, ByVal destination As Component) As Point
			Dim point As New Point(x,y)
			Return convertPoint(source,point,destination)
		End Function

		''' <summary>
		''' Convert the rectangle <code>aRectangle</code> in <code>source</code> coordinate system to
		''' <code>destination</code> coordinate system.
		''' If <code>source</code> is {@code null}, <code>aRectangle</code> is assumed to be in <code>destination</code>'s
		''' root component coordinate system.
		''' If <code>destination</code> is {@code null}, <code>aRectangle</code> will be converted to <code>source</code>'s
		''' root component coordinate system.
		''' If both <code>source</code> and <code>destination</code> are {@code null}, return <code>aRectangle</code>
		''' without any conversion.
		''' </summary>
		Public Shared Function convertRectangle(ByVal source As Component, ByVal aRectangle As Rectangle, ByVal destination As Component) As Rectangle
			Dim point As New Point(aRectangle.x,aRectangle.y)
			point = convertPoint(source,point,destination)
			Return New Rectangle(point.x,point.y,aRectangle.width,aRectangle.height)
		End Function

		''' <summary>
		''' Convenience method for searching above <code>comp</code> in the
		''' component hierarchy and returns the first object of class <code>c</code> it
		''' finds. Can return {@code null}, if a class <code>c</code> cannot be found.
		''' </summary>
		Public Shared Function getAncestorOfClass(ByVal c As Type, ByVal comp As Component) As Container
			If comp Is Nothing OrElse c Is Nothing Then Return Nothing

			Dim parent As Container = comp.parent
			Do While parent IsNot Nothing AndAlso Not(c.IsInstanceOfType(parent))
				parent = parent.parent
			Loop
			Return parent
		End Function

		''' <summary>
		''' Convenience method for searching above <code>comp</code> in the
		''' component hierarchy and returns the first object of <code>name</code> it
		''' finds. Can return {@code null}, if <code>name</code> cannot be found.
		''' </summary>
		Public Shared Function getAncestorNamed(ByVal name As String, ByVal comp As Component) As Container
			If comp Is Nothing OrElse name Is Nothing Then Return Nothing

			Dim parent As Container = comp.parent
			Do While parent IsNot Nothing AndAlso Not(name.Equals(parent.name))
				parent = parent.parent
			Loop
			Return parent
		End Function

		''' <summary>
		''' Returns the deepest visible descendent Component of <code>parent</code>
		''' that contains the location <code>x</code>, <code>y</code>.
		''' If <code>parent</code> does not contain the specified location,
		''' then <code>null</code> is returned.  If <code>parent</code> is not a
		''' container, or none of <code>parent</code>'s visible descendents
		''' contain the specified location, <code>parent</code> is returned.
		''' </summary>
		''' <param name="parent"> the root component to begin the search </param>
		''' <param name="x"> the x target location </param>
		''' <param name="y"> the y target location </param>
		Public Shared Function getDeepestComponentAt(ByVal parent As Component, ByVal x As Integer, ByVal y As Integer) As Component
			If Not parent.contains(x, y) Then Return Nothing
			If TypeOf parent Is Container Then
				Dim components As Component() = CType(parent, Container).components
				For Each comp As Component In components
					If comp IsNot Nothing AndAlso comp.visible Then
						Dim loc As Point = comp.location
						If TypeOf comp Is Container Then
							comp = getDeepestComponentAt(comp, x - loc.x, y - loc.y)
						Else
							comp = comp.getComponentAt(x - loc.x, y - loc.y)
						End If
						If comp IsNot Nothing AndAlso comp.visible Then Return comp
					End If
				Next comp
			End If
			Return parent
		End Function


		''' <summary>
		''' Returns a MouseEvent similar to <code>sourceEvent</code> except that its x
		''' and y members have been converted to <code>destination</code>'s coordinate
		''' system.  If <code>source</code> is {@code null}, <code>sourceEvent</code> x and y members
		''' are assumed to be into <code>destination</code>'s root component coordinate system.
		''' If <code>destination</code> is <code>null</code>, the
		''' returned MouseEvent will be in <code>source</code>'s coordinate system.
		''' <code>sourceEvent</code> will not be changed. A new event is returned.
		''' the <code>source</code> field of the returned event will be set
		''' to <code>destination</code> if destination is non-{@code null}
		''' use the translateMouseEvent() method to translate a mouse event from
		''' one component to another without changing the source.
		''' </summary>
		Public Shared Function convertMouseEvent(ByVal source As Component, ByVal sourceEvent As MouseEvent, ByVal destination As Component) As MouseEvent
			Dim p As Point = convertPoint(source,New Point(sourceEvent.x, sourceEvent.y), destination)
			Dim newSource As Component

			If destination IsNot Nothing Then
				newSource = destination
			Else
				newSource = source
			End If

			Dim newEvent As MouseEvent
			If TypeOf sourceEvent Is MouseWheelEvent Then
				Dim sourceWheelEvent As MouseWheelEvent = CType(sourceEvent, MouseWheelEvent)
				newEvent = New MouseWheelEvent(newSource, sourceWheelEvent.iD, sourceWheelEvent.when, sourceWheelEvent.modifiers Or sourceWheelEvent.modifiersEx, p.x,p.y, sourceWheelEvent.xOnScreen, sourceWheelEvent.yOnScreen, sourceWheelEvent.clickCount, sourceWheelEvent.popupTrigger, sourceWheelEvent.scrollType, sourceWheelEvent.scrollAmount, sourceWheelEvent.wheelRotation)
			ElseIf TypeOf sourceEvent Is javax.swing.event.MenuDragMouseEvent Then
				Dim sourceMenuDragEvent As javax.swing.event.MenuDragMouseEvent = CType(sourceEvent, javax.swing.event.MenuDragMouseEvent)
				newEvent = New javax.swing.event.MenuDragMouseEvent(newSource, sourceMenuDragEvent.iD, sourceMenuDragEvent.when, sourceMenuDragEvent.modifiers Or sourceMenuDragEvent.modifiersEx, p.x,p.y, sourceMenuDragEvent.xOnScreen, sourceMenuDragEvent.yOnScreen, sourceMenuDragEvent.clickCount, sourceMenuDragEvent.popupTrigger, sourceMenuDragEvent.path, sourceMenuDragEvent.menuSelectionManager)
			Else
				newEvent = New MouseEvent(newSource, sourceEvent.iD, sourceEvent.when, sourceEvent.modifiers Or sourceEvent.modifiersEx, p.x,p.y, sourceEvent.xOnScreen, sourceEvent.yOnScreen, sourceEvent.clickCount, sourceEvent.popupTrigger, sourceEvent.button)
			End If
			Return newEvent
		End Function


		''' <summary>
		''' Convert a point from a component's coordinate system to
		''' screen coordinates.
		''' </summary>
		''' <param name="p">  a Point object (converted to the new coordinate system) </param>
		''' <param name="c">  a Component object </param>
		Public Shared Sub convertPointToScreen(ByVal p As Point, ByVal c As Component)
				Dim b As Rectangle
				Dim x, y As Integer

				Do
					If TypeOf c Is JComponent Then
						x = c.x
						y = c.y
					ElseIf TypeOf c Is java.applet.Applet OrElse TypeOf c Is java.awt.Window Then
						Try
							Dim pp As Point = c.locationOnScreen
							x = pp.x
							y = pp.y
						Catch icse As IllegalComponentStateException
							x = c.x
							y = c.y
						End Try
					Else
						x = c.x
						y = c.y
					End If

					p.x += x
					p.y += y

					If TypeOf c Is java.awt.Window OrElse TypeOf c Is java.applet.Applet Then Exit Do
					c = c.parent
				Loop While c IsNot Nothing
		End Sub

		''' <summary>
		''' Convert a point from a screen coordinates to a component's
		''' coordinate system
		''' </summary>
		''' <param name="p">  a Point object (converted to the new coordinate system) </param>
		''' <param name="c">  a Component object </param>
		Public Shared Sub convertPointFromScreen(ByVal p As Point, ByVal c As Component)
			Dim b As Rectangle
			Dim x, y As Integer

			Do
				If TypeOf c Is JComponent Then
					x = c.x
					y = c.y
				ElseIf TypeOf c Is java.applet.Applet OrElse TypeOf c Is java.awt.Window Then
					Try
						Dim pp As Point = c.locationOnScreen
						x = pp.x
						y = pp.y
					Catch icse As IllegalComponentStateException
						x = c.x
						y = c.y
					End Try
				Else
					x = c.x
					y = c.y
				End If

				p.x -= x
				p.y -= y

				If TypeOf c Is java.awt.Window OrElse TypeOf c Is java.applet.Applet Then Exit Do
				c = c.parent
			Loop While c IsNot Nothing
		End Sub

		''' <summary>
		''' Returns the first <code>Window </code> ancestor of <code>c</code>, or
		''' {@code null} if <code>c</code> is not contained inside a <code>Window</code>.
		''' <p>
		''' Note: This method provides the same functionality as
		''' <code>getWindowAncestor</code>.
		''' </summary>
		''' <param name="c"> <code>Component</code> to get <code>Window</code> ancestor
		'''        of. </param>
		''' <returns> the first <code>Window </code> ancestor of <code>c</code>, or
		'''         {@code null} if <code>c</code> is not contained inside a
		'''         <code>Window</code>. </returns>
		Public Shared Function windowForComponent(ByVal c As Component) As Window
			Return getWindowAncestor(c)
		End Function

		''' <summary>
		''' Return <code>true</code> if a component <code>a</code> descends from a component <code>b</code>
		''' </summary>
		Public Shared Function isDescendingFrom(ByVal a As Component, ByVal b As Component) As Boolean
			If a Is b Then Return True
			Dim p As Container = a.parent
			Do While p IsNot Nothing
				If p Is b Then Return True
				p=p.parent
			Loop
			Return False
		End Function


		''' <summary>
		''' Convenience to calculate the intersection of two rectangles
		''' without allocating a new rectangle.
		''' If the two rectangles don't intersect,
		''' then the returned rectangle begins at (0,0)
		''' and has zero width and height.
		''' </summary>
		''' <param name="x">       the X coordinate of the first rectangle's top-left point </param>
		''' <param name="y">       the Y coordinate of the first rectangle's top-left point </param>
		''' <param name="width">   the width of the first rectangle </param>
		''' <param name="height">  the height of the first rectangle </param>
		''' <param name="dest">    the second rectangle
		''' </param>
		''' <returns> <code>dest</code>, modified to specify the intersection </returns>
		Public Shared Function computeIntersection(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal dest As Rectangle) As Rectangle
			Dim x1 As Integer = If(x > dest.x, x, dest.x)
			Dim x2 As Integer = If((x+width) < (dest.x + dest.width), (x+width), (dest.x + dest.width))
			Dim y1 As Integer = If(y > dest.y, y, dest.y)
			Dim y2 As Integer = (If((y + height) < (dest.y + dest.height), (y+height), (dest.y + dest.height)))

			dest.x = x1
			dest.y = y1
			dest.width = x2 - x1
			dest.height = y2 - y1

			' If rectangles don't intersect, return zero'd intersection.
			If dest.width < 0 OrElse dest.height < 0 Then
					dest.height = 0
						dest.width = dest.height
							dest.y = dest.width
							dest.x = dest.y
			End If

			Return dest
		End Function

		''' <summary>
		''' Convenience method that calculates the union of two rectangles
		''' without allocating a new rectangle.
		''' </summary>
		''' <param name="x"> the x-coordinate of the first rectangle </param>
		''' <param name="y"> the y-coordinate of the first rectangle </param>
		''' <param name="width"> the width of the first rectangle </param>
		''' <param name="height"> the height of the first rectangle </param>
		''' <param name="dest">  the coordinates of the second rectangle; the union
		'''    of the two rectangles is returned in this rectangle </param>
		''' <returns> the <code>dest</code> <code>Rectangle</code> </returns>
		Public Shared Function computeUnion(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal dest As Rectangle) As Rectangle
			Dim x1 As Integer = If(x < dest.x, x, dest.x)
			Dim x2 As Integer = If((x+width) > (dest.x + dest.width), (x+width), (dest.x + dest.width))
			Dim y1 As Integer = If(y < dest.y, y, dest.y)
			Dim y2 As Integer = If((y+height) > (dest.y + dest.height), (y+height), (dest.y + dest.height))

			dest.x = x1
			dest.y = y1
			dest.width = (x2 - x1)
			dest.height= (y2 - y1)
			Return dest
		End Function

		''' <summary>
		''' Convenience returning an array of rect representing the regions within
		''' <code>rectA</code> that do not overlap with <code>rectB</code>. If the
		''' two Rects do not overlap, returns an empty array
		''' </summary>
		Public Shared Function computeDifference(ByVal rectA As Rectangle, ByVal rectB As Rectangle) As Rectangle()
			If rectB Is Nothing OrElse (Not rectA.intersects(rectB)) OrElse isRectangleContainingRectangle(rectB,rectA) Then Return New Rectangle(){}

			Dim t As New Rectangle
			Dim a As Rectangle=Nothing, b As Rectangle=Nothing, c As Rectangle=Nothing, d As Rectangle=Nothing
			Dim result As Rectangle()
			Dim rectCount As Integer = 0

			' rectA contains rectB 
			If isRectangleContainingRectangle(rectA,rectB) Then
				t.x = rectA.x
				t.y = rectA.y
				t.width = rectB.x - rectA.x
				t.height = rectA.height
				If t.width > 0 AndAlso t.height > 0 Then
					a = New Rectangle(t)
					rectCount += 1
				End If

				t.x = rectB.x
				t.y = rectA.y
				t.width = rectB.width
				t.height = rectB.y - rectA.y
				If t.width > 0 AndAlso t.height > 0 Then
					b = New Rectangle(t)
					rectCount += 1
				End If

				t.x = rectB.x
				t.y = rectB.y + rectB.height
				t.width = rectB.width
				t.height = rectA.y + rectA.height - (rectB.y + rectB.height)
				If t.width > 0 AndAlso t.height > 0 Then
					c = New Rectangle(t)
					rectCount += 1
				End If

				t.x = rectB.x + rectB.width
				t.y = rectA.y
				t.width = rectA.x + rectA.width - (rectB.x + rectB.width)
				t.height = rectA.height
				If t.width > 0 AndAlso t.height > 0 Then
					d = New Rectangle(t)
					rectCount += 1
				End If
			Else
				' 1 
				If rectB.x <= rectA.x AndAlso rectB.y <= rectA.y Then
					If (rectB.x + rectB.width) > (rectA.x + rectA.width) Then

						t.x = rectA.x
						t.y = rectB.y + rectB.height
						t.width = rectA.width
						t.height = rectA.y + rectA.height - (rectB.y + rectB.height)
						If t.width > 0 AndAlso t.height > 0 Then
							a = t
							rectCount += 1
						End If
					ElseIf (rectB.y + rectB.height) > (rectA.y + rectA.height) Then
						t.boundsnds((rectB.x + rectB.width), rectA.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							a = t
							rectCount += 1
						End If
					Else
						t.boundsnds((rectB.x + rectB.width), rectA.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), (rectB.y + rectB.height) - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, (rectB.y + rectB.height), rectA.width, (rectA.y + rectA.height) - (rectB.y + rectB.height))
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If
					End If
				ElseIf rectB.x <= rectA.x AndAlso (rectB.y + rectB.height) >= (rectA.y + rectA.height) Then
					If (rectB.x + rectB.width) > (rectA.x + rectA.width) Then
						t.boundsnds(rectA.x, rectA.y, rectA.width, rectB.y - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = t
							rectCount += 1
						End If
					Else
						t.boundsnds(rectA.x, rectA.y, rectA.width, rectB.y - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If
						t.boundsnds((rectB.x + rectB.width), rectB.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), (rectA.y + rectA.height) - rectB.y)
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If
					End If
				ElseIf rectB.x <= rectA.x Then
					If (rectB.x + rectB.width) >= (rectA.x + rectA.width) Then
						t.boundsnds(rectA.x, rectA.y, rectA.width, rectB.y - rectA.y)
						If t.width>0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, (rectB.y + rectB.height), rectA.width, (rectA.y + rectA.height) - (rectB.y + rectB.height))
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If
					Else
						t.boundsnds(rectA.x, rectA.y, rectA.width, rectB.y - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds((rectB.x + rectB.width), rectB.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), rectB.height)
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, (rectB.y + rectB.height), rectA.width, (rectA.y + rectA.height) - (rectB.y + rectB.height))
						If t.width > 0 AndAlso t.height > 0 Then
							c = New Rectangle(t)
							rectCount += 1
						End If
					End If
				ElseIf rectB.x <= (rectA.x + rectA.width) AndAlso (rectB.x + rectB.width) > (rectA.x + rectA.width) Then
					If rectB.y <= rectA.y AndAlso (rectB.y + rectB.height) > (rectA.y + rectA.height) Then
						t.boundsnds(rectA.x, rectA.y, rectB.x - rectA.x, rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							a = t
							rectCount += 1
						End If
					ElseIf rectB.y <= rectA.y Then
						t.boundsnds(rectA.x, rectA.y, rectB.x - rectA.x, (rectB.y + rectB.height) - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, (rectB.y + rectB.height), rectA.width, (rectA.y + rectA.height) - (rectB.y + rectB.height))
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If
					ElseIf (rectB.y + rectB.height) > (rectA.y + rectA.height) Then
						t.boundsnds(rectA.x, rectA.y, rectA.width, rectB.y - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, rectB.y, rectB.x - rectA.x, (rectA.y + rectA.height) - rectB.y)
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If
					Else
						t.boundsnds(rectA.x, rectA.y, rectA.width, rectB.y - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, rectB.y, rectB.x - rectA.x, rectB.height)
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectA.x, (rectB.y + rectB.height), rectA.width, (rectA.y + rectA.height) - (rectB.y + rectB.height))
						If t.width > 0 AndAlso t.height > 0 Then
							c = New Rectangle(t)
							rectCount += 1
						End If
					End If
				ElseIf rectB.x >= rectA.x AndAlso (rectB.x + rectB.width) <= (rectA.x + rectA.width) Then
					If rectB.y <= rectA.y AndAlso (rectB.y + rectB.height) > (rectA.y + rectA.height) Then
						t.boundsnds(rectA.x, rectA.y, rectB.x - rectA.x, rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If
						t.boundsnds((rectB.x + rectB.width), rectA.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If
					ElseIf rectB.y <= rectA.y Then
						t.boundsnds(rectA.x, rectA.y, rectB.x - rectA.x, rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectB.x, (rectB.y + rectB.height), rectB.width, (rectA.y + rectA.height) - (rectB.y + rectB.height))
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds((rectB.x + rectB.width), rectA.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							c = New Rectangle(t)
							rectCount += 1
						End If
					Else
						t.boundsnds(rectA.x, rectA.y, rectB.x - rectA.x, rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							a = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds(rectB.x, rectA.y, rectB.width, rectB.y - rectA.y)
						If t.width > 0 AndAlso t.height > 0 Then
							b = New Rectangle(t)
							rectCount += 1
						End If

						t.boundsnds((rectB.x + rectB.width), rectA.y, (rectA.x + rectA.width) - (rectB.x + rectB.width), rectA.height)
						If t.width > 0 AndAlso t.height > 0 Then
							c = New Rectangle(t)
							rectCount += 1
						End If
					End If
				End If
			End If

			result = New Rectangle(rectCount - 1){}
			rectCount = 0
			If a IsNot Nothing Then
				result(rectCount) = a
				rectCount += 1
			End If
			If b IsNot Nothing Then
				result(rectCount) = b
				rectCount += 1
			End If
			If c IsNot Nothing Then
				result(rectCount) = c
				rectCount += 1
			End If
			If d IsNot Nothing Then
				result(rectCount) = d
				rectCount += 1
			End If
			Return result
		End Function

		''' <summary>
		''' Returns true if the mouse event specifies the left mouse button.
		''' </summary>
		''' <param name="anEvent">  a MouseEvent object </param>
		''' <returns> true if the left mouse button was active </returns>
		Public Shared Function isLeftMouseButton(ByVal anEvent As MouseEvent) As Boolean
			 Return ((anEvent.modifiersEx And InputEvent.BUTTON1_DOWN_MASK) <> 0 OrElse anEvent.button = MouseEvent.BUTTON1)
		End Function

		''' <summary>
		''' Returns true if the mouse event specifies the middle mouse button.
		''' </summary>
		''' <param name="anEvent">  a MouseEvent object </param>
		''' <returns> true if the middle mouse button was active </returns>
		Public Shared Function isMiddleMouseButton(ByVal anEvent As MouseEvent) As Boolean
			Return ((anEvent.modifiersEx And InputEvent.BUTTON2_DOWN_MASK) <> 0 OrElse anEvent.button = MouseEvent.BUTTON2)
		End Function

		''' <summary>
		''' Returns true if the mouse event specifies the right mouse button.
		''' </summary>
		''' <param name="anEvent">  a MouseEvent object </param>
		''' <returns> true if the right mouse button was active </returns>
		Public Shared Function isRightMouseButton(ByVal anEvent As MouseEvent) As Boolean
			Return ((anEvent.modifiersEx And InputEvent.BUTTON3_DOWN_MASK) <> 0 OrElse anEvent.button = MouseEvent.BUTTON3)
		End Function

		''' <summary>
		''' Compute the width of the string using a font with the specified
		''' "metrics" (sizes).
		''' </summary>
		''' <param name="fm">   a FontMetrics object to compute with </param>
		''' <param name="str">  the String to compute </param>
		''' <returns> an int containing the string width </returns>
		Public Shared Function computeStringWidth(ByVal fm As FontMetrics, ByVal str As String) As Integer
			' You can't assume that a string's width is the sum of its
			' characters' widths in Java2D -- it may be smaller due to
			' kerning, etc.
			Return sun.swing.SwingUtilities2.stringWidth(Nothing, fm, str)
		End Function

		''' <summary>
		''' Compute and return the location of the icons origin, the
		''' location of origin of the text baseline, and a possibly clipped
		''' version of the compound labels string.  Locations are computed
		''' relative to the viewR rectangle.
		''' The JComponents orientation (LEADING/TRAILING) will also be taken
		''' into account and translated into LEFT/RIGHT values accordingly.
		''' </summary>
		Public Shared Function layoutCompoundLabel(ByVal c As JComponent, ByVal fm As FontMetrics, ByVal text As String, ByVal icon As Icon, ByVal verticalAlignment As Integer, ByVal horizontalAlignment As Integer, ByVal verticalTextPosition As Integer, ByVal horizontalTextPosition As Integer, ByVal viewR As Rectangle, ByVal iconR As Rectangle, ByVal textR As Rectangle, ByVal textIconGap As Integer) As String
			Dim orientationIsLeftToRight As Boolean = True
			Dim hAlign As Integer = horizontalAlignment
			Dim hTextPos As Integer = horizontalTextPosition

			If c IsNot Nothing Then
				If Not(c.componentOrientation.leftToRight) Then orientationIsLeftToRight = False
			End If

			' Translate LEADING/TRAILING values in horizontalAlignment
			' to LEFT/RIGHT values depending on the components orientation
			Select Case horizontalAlignment
			Case LEADING
				hAlign = If(orientationIsLeftToRight, LEFT, RIGHT)
			Case TRAILING
				hAlign = If(orientationIsLeftToRight, RIGHT, LEFT)
			End Select

			' Translate LEADING/TRAILING values in horizontalTextPosition
			' to LEFT/RIGHT values depending on the components orientation
			Select Case horizontalTextPosition
			Case LEADING
				hTextPos = If(orientationIsLeftToRight, LEFT, RIGHT)
			Case TRAILING
				hTextPos = If(orientationIsLeftToRight, RIGHT, LEFT)
			End Select

			Return layoutCompoundLabelImpl(c, fm, text, icon, verticalAlignment, hAlign, verticalTextPosition, hTextPos, viewR, iconR, textR, textIconGap)
		End Function

		''' <summary>
		''' Compute and return the location of the icons origin, the
		''' location of origin of the text baseline, and a possibly clipped
		''' version of the compound labels string.  Locations are computed
		''' relative to the viewR rectangle.
		''' This layoutCompoundLabel() does not know how to handle LEADING/TRAILING
		''' values in horizontalTextPosition (they will default to RIGHT) and in
		''' horizontalAlignment (they will default to CENTER).
		''' Use the other version of layoutCompoundLabel() instead.
		''' </summary>
		Public Shared Function layoutCompoundLabel(ByVal fm As FontMetrics, ByVal text As String, ByVal icon As Icon, ByVal verticalAlignment As Integer, ByVal horizontalAlignment As Integer, ByVal verticalTextPosition As Integer, ByVal horizontalTextPosition As Integer, ByVal viewR As Rectangle, ByVal iconR As Rectangle, ByVal textR As Rectangle, ByVal textIconGap As Integer) As String
			Return layoutCompoundLabelImpl(Nothing, fm, text, icon, verticalAlignment, horizontalAlignment, verticalTextPosition, horizontalTextPosition, viewR, iconR, textR, textIconGap)
		End Function

		''' <summary>
		''' Compute and return the location of the icons origin, the
		''' location of origin of the text baseline, and a possibly clipped
		''' version of the compound labels string.  Locations are computed
		''' relative to the viewR rectangle.
		''' This layoutCompoundLabel() does not know how to handle LEADING/TRAILING
		''' values in horizontalTextPosition (they will default to RIGHT) and in
		''' horizontalAlignment (they will default to CENTER).
		''' Use the other version of layoutCompoundLabel() instead.
		''' </summary>
		Private Shared Function layoutCompoundLabelImpl(ByVal c As JComponent, ByVal fm As FontMetrics, ByVal text As String, ByVal icon As Icon, ByVal verticalAlignment As Integer, ByVal horizontalAlignment As Integer, ByVal verticalTextPosition As Integer, ByVal horizontalTextPosition As Integer, ByVal viewR As Rectangle, ByVal iconR As Rectangle, ByVal textR As Rectangle, ByVal textIconGap As Integer) As String
	'         Initialize the icon bounds rectangle iconR.
	'         

			If icon IsNot Nothing Then
				iconR.width = icon.iconWidth
				iconR.height = icon.iconHeight
			Else
					iconR.height = 0
					iconR.width = iconR.height
			End If

	'         Initialize the text bounds rectangle textR.  If a null
	'         * or and empty String was specified we substitute "" here
	'         * and use 0,0,0,0 for textR.
	'         

			Dim textIsEmpty As Boolean = (text Is Nothing) OrElse text.Equals("")
			Dim lsb As Integer = 0
			Dim rsb As Integer = 0
	'         Unless both text and icon are non-null, we effectively ignore
	'         * the value of textIconGap.
	'         
			Dim gap As Integer

			Dim v As javax.swing.text.View
			If textIsEmpty Then
					textR.height = 0
					textR.width = textR.height
				text = ""
				gap = 0
			Else
				Dim availTextWidth As Integer
				gap = If(icon Is Nothing, 0, textIconGap)

				If horizontalTextPosition = CENTER Then
					availTextWidth = viewR.width
				Else
					availTextWidth = viewR.width - (iconR.width + gap)
				End If
				v = If(c IsNot Nothing, CType(c.getClientProperty("html"), javax.swing.text.View), Nothing)
				If v IsNot Nothing Then
					textR.width = Math.Min(availTextWidth, CInt(Fix(v.getPreferredSpan(javax.swing.text.View.X_AXIS))))
					textR.height = CInt(Fix(v.getPreferredSpan(javax.swing.text.View.Y_AXIS)))
				Else
					textR.width = sun.swing.SwingUtilities2.stringWidth(c, fm, text)
					lsb = sun.swing.SwingUtilities2.getLeftSideBearing(c, fm, text)
					If lsb < 0 Then textR.width -= lsb
					If textR.width > availTextWidth Then
						text = sun.swing.SwingUtilities2.clipString(c, fm, text, availTextWidth)
						textR.width = sun.swing.SwingUtilities2.stringWidth(c, fm, text)
					End If
					textR.height = fm.height
				End If
			End If


	'         Compute textR.x,y given the verticalTextPosition and
	'         * horizontalTextPosition properties
	'         

			If verticalTextPosition = TOP Then
				If horizontalTextPosition <> CENTER Then
					textR.y = 0
				Else
					textR.y = -(textR.height + gap)
				End If
			ElseIf verticalTextPosition = CENTER Then
				textR.y = (iconR.height / 2) - (textR.height / 2)
			Else ' (verticalTextPosition == BOTTOM)
				If horizontalTextPosition <> CENTER Then
					textR.y = iconR.height - textR.height
				Else
					textR.y = (iconR.height + gap)
				End If
			End If

			If horizontalTextPosition = LEFT Then
				textR.x = -(textR.width + gap)
			ElseIf horizontalTextPosition = CENTER Then
				textR.x = (iconR.width / 2) - (textR.width / 2)
			Else ' (horizontalTextPosition == RIGHT)
				textR.x = (iconR.width + gap)
			End If

			' WARNING: DefaultTreeCellEditor uses a shortened version of
			' this algorithm to position it's Icon. If you change how this
			' is calculated, be sure and update DefaultTreeCellEditor too.

	'         labelR is the rectangle that contains iconR and textR.
	'         * Move it to its proper position given the labelAlignment
	'         * properties.
	'         *
	'         * To avoid actually allocating a Rectangle, Rectangle.union
	'         * has been inlined below.
	'         
			Dim labelR_x As Integer = Math.Min(iconR.x, textR.x)
			Dim labelR_width As Integer = Math.Max(iconR.x + iconR.width, textR.x + textR.width) - labelR_x
			Dim labelR_y As Integer = Math.Min(iconR.y, textR.y)
			Dim labelR_height As Integer = Math.Max(iconR.y + iconR.height, textR.y + textR.height) - labelR_y

			Dim dx, dy As Integer

			If verticalAlignment = TOP Then
				dy = viewR.y - labelR_y
			ElseIf verticalAlignment = CENTER Then
				dy = (viewR.y + (viewR.height / 2)) - (labelR_y + (labelR_height \ 2))
			Else ' (verticalAlignment == BOTTOM)
				dy = (viewR.y + viewR.height) - (labelR_y + labelR_height)
			End If

			If horizontalAlignment = LEFT Then
				dx = viewR.x - labelR_x
			ElseIf horizontalAlignment = RIGHT Then
				dx = (viewR.x + viewR.width) - (labelR_x + labelR_width)
			Else ' (horizontalAlignment == CENTER)
				dx = (viewR.x + (viewR.width / 2)) - (labelR_x + (labelR_width \ 2))
			End If

	'         Translate textR and glypyR by dx,dy.
	'         

			textR.x += dx
			textR.y += dy

			iconR.x += dx
			iconR.y += dy

			If lsb < 0 Then
				' lsb is negative. Shift the x location so that the text is
				' visually drawn at the right location.
				textR.x -= lsb

				textR.width += lsb
			End If
			If rsb > 0 Then textR.width -= rsb

			Return text
		End Function


		''' <summary>
		''' Paints a component to the specified <code>Graphics</code>.
		''' This method is primarily useful to render
		''' <code>Component</code>s that don't exist as part of the visible
		''' containment hierarchy, but are used for rendering.  For
		''' example, if you are doing your own rendering and want to render
		''' some text (or even HTML), you could make use of
		''' <code>JLabel</code>'s text rendering support and have it paint
		''' directly by way of this method, without adding the label to the
		''' visible containment hierarchy.
		''' <p>
		''' This method makes use of <code>CellRendererPane</code> to handle
		''' the actual painting, and is only recommended if you use one
		''' component for rendering.  If you make use of multiple components
		''' to handle the rendering, as <code>JTable</code> does, use
		''' <code>CellRendererPane</code> directly.  Otherwise, as described
		''' below, you could end up with a <code>CellRendererPane</code>
		''' per <code>Component</code>.
		''' <p>
		''' If <code>c</code>'s parent is not a <code>CellRendererPane</code>,
		''' a new <code>CellRendererPane</code> is created, <code>c</code> is
		''' added to it, and the <code>CellRendererPane</code> is added to
		''' <code>p</code>.  If <code>c</code>'s parent is a
		''' <code>CellRendererPane</code> and the <code>CellRendererPane</code>s
		''' parent is not <code>p</code>, it is added to <code>p</code>.
		''' <p>
		''' The component should either descend from <code>JComponent</code>
		''' or be another kind of lightweight component.
		''' A lightweight component is one whose "lightweight" property
		''' (returned by the <code>Component</code>
		''' <code>isLightweight</code> method)
		''' is true. If the Component is not lightweight, bad things map happen:
		''' crashes, exceptions, painting problems...
		''' </summary>
		''' <param name="g">  the <code>Graphics</code> object to draw on </param>
		''' <param name="c">  the <code>Component</code> to draw </param>
		''' <param name="p">  the intermediate <code>Container</code> </param>
		''' <param name="x">  an int specifying the left side of the area draw in, in pixels,
		'''           measured from the left edge of the graphics context </param>
		''' <param name="y">  an int specifying the top of the area to draw in, in pixels
		'''           measured down from the top edge of the graphics context </param>
		''' <param name="w">  an int specifying the width of the area draw in, in pixels </param>
		''' <param name="h">  an int specifying the height of the area draw in, in pixels
		''' </param>
		''' <seealso cref= CellRendererPane </seealso>
		''' <seealso cref= java.awt.Component#isLightweight </seealso>
		Public Shared Sub paintComponent(ByVal g As Graphics, ByVal c As Component, ByVal p As Container, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			getCellRendererPane(c, p).paintComponent(g, c, p, x, y, w, h,False)
		End Sub

		''' <summary>
		''' Paints a component to the specified <code>Graphics</code>.  This
		''' is a cover method for
		''' <seealso cref="#paintComponent(Graphics,Component,Container,int,int,int,int)"/>.
		''' Refer to it for more information.
		''' </summary>
		''' <param name="g">  the <code>Graphics</code> object to draw on </param>
		''' <param name="c">  the <code>Component</code> to draw </param>
		''' <param name="p">  the intermediate <code>Container</code> </param>
		''' <param name="r">  the <code>Rectangle</code> to draw in
		''' </param>
		''' <seealso cref= #paintComponent(Graphics,Component,Container,int,int,int,int) </seealso>
		''' <seealso cref= CellRendererPane </seealso>
		Public Shared Sub paintComponent(ByVal g As Graphics, ByVal c As Component, ByVal p As Container, ByVal r As Rectangle)
			paintComponent(g, c, p, r.x, r.y, r.width, r.height)
		End Sub


	'    
	'     * Ensures that cell renderer <code>c</code> has a
	'     * <code>ComponentShell</code> parent and that
	'     * the shell's parent is p.
	'     
		Private Shared Function getCellRendererPane(ByVal c As Component, ByVal p As Container) As CellRendererPane
			Dim shell As Container = c.parent
			If TypeOf shell Is CellRendererPane Then
				If shell.parent IsNot p Then p.add(shell)
			Else
				shell = New CellRendererPane
				shell.add(c)
				p.add(shell)
			End If
			Return CType(shell, CellRendererPane)
		End Function

		''' <summary>
		''' A simple minded look and feel change: ask each node in the tree
		''' to <code>updateUI()</code> -- that is, to initialize its UI property
		''' with the current look and feel.
		''' </summary>
		Public Shared Sub updateComponentTreeUI(ByVal c As Component)
			updateComponentTreeUI0(c)
			c.invalidate()
			c.validate()
			c.repaint()
		End Sub

		Private Shared Sub updateComponentTreeUI0(ByVal c As Component)
			If TypeOf c Is JComponent Then
				Dim jc As JComponent = CType(c, JComponent)
				jc.updateUI()
				Dim jpm As JPopupMenu =jc.componentPopupMenu
				If jpm IsNot Nothing Then updateComponentTreeUI(jpm)
			End If
			Dim children As Component() = Nothing
			If TypeOf c Is JMenu Then
				children = CType(c, JMenu).menuComponents
			ElseIf TypeOf c Is Container Then
				children = CType(c, Container).components
			End If
			If children IsNot Nothing Then
				For Each child As Component In children
					updateComponentTreeUI0(child)
				Next child
			End If
		End Sub


		''' <summary>
		''' Causes <i>doRun.run()</i> to be executed asynchronously on the
		''' AWT event dispatching thread.  This will happen after all
		''' pending AWT events have been processed.  This method should
		''' be used when an application thread needs to update the GUI.
		''' In the following example the <code>invokeLater</code> call queues
		''' the <code>Runnable</code> object <code>doHelloWorld</code>
		''' on the event dispatching thread and
		''' then prints a message.
		''' <pre>
		''' Runnable doHelloWorld = new Runnable() {
		'''     public void run() {
		'''         System.out.println("Hello World on " + Thread.currentThread());
		'''     }
		''' };
		''' 
		''' SwingUtilities.invokeLater(doHelloWorld);
		''' System.out.println("This might well be displayed before the other message.");
		''' </pre>
		''' If invokeLater is called from the event dispatching thread --
		''' for example, from a JButton's ActionListener -- the <i>doRun.run()</i> will
		''' still be deferred until all pending events have been processed.
		''' Note that if the <i>doRun.run()</i> throws an uncaught exception
		''' the event dispatching thread will unwind (not the current thread).
		''' <p>
		''' Additional documentation and examples for this method can be
		''' found in
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency in Swing</a>.
		''' <p>
		''' As of 1.3 this method is just a cover for <code>java.awt.EventQueue.invokeLater()</code>.
		''' <p>
		''' Unlike the rest of Swing, this method can be invoked from any thread.
		''' </summary>
		''' <seealso cref= #invokeAndWait </seealso>
		Public Shared Sub invokeLater(ByVal doRun As Runnable)
			EventQueue.invokeLater(doRun)
		End Sub


		''' <summary>
		''' Causes <code>doRun.run()</code> to be executed synchronously on the
		''' AWT event dispatching thread.  This call blocks until
		''' all pending AWT events have been processed and (then)
		''' <code>doRun.run()</code> returns. This method should
		''' be used when an application thread needs to update the GUI.
		''' It shouldn't be called from the event dispatching thread.
		''' Here's an example that creates a new application thread
		''' that uses <code>invokeAndWait</code> to print a string from the event
		''' dispatching thread and then, when that's finished, print
		''' a string from the application thread.
		''' <pre>
		''' final Runnable doHelloWorld = new Runnable() {
		'''     public void run() {
		'''         System.out.println("Hello World on " + Thread.currentThread());
		'''     }
		''' };
		''' 
		''' Thread appThread = new Thread() {
		'''     public void run() {
		'''         try {
		'''             SwingUtilities.invokeAndWait(doHelloWorld);
		'''         }
		'''         catch (Exception e) {
		'''             e.printStackTrace();
		'''         }
		'''         System.out.println("Finished on " + Thread.currentThread());
		'''     }
		''' };
		''' appThread.start();
		''' </pre>
		''' Note that if the <code>Runnable.run</code> method throws an
		''' uncaught exception
		''' (on the event dispatching thread) it's caught and rethrown, as
		''' an <code>InvocationTargetException</code>, on the caller's thread.
		''' <p>
		''' Additional documentation and examples for this method can be
		''' found in
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency in Swing</a>.
		''' <p>
		''' As of 1.3 this method is just a cover for
		''' <code>java.awt.EventQueue.invokeAndWait()</code>.
		''' </summary>
		''' <exception cref="InterruptedException"> if we're interrupted while waiting for
		'''             the event dispatching thread to finish executing
		'''             <code>doRun.run()</code> </exception>
		''' <exception cref="InvocationTargetException">  if an exception is thrown
		'''             while running <code>doRun</code>
		''' </exception>
		''' <seealso cref= #invokeLater </seealso>
		Public Shared Sub invokeAndWait(ByVal doRun As Runnable)
			EventQueue.invokeAndWait(doRun)
		End Sub

		''' <summary>
		''' Returns true if the current thread is an AWT event dispatching thread.
		''' <p>
		''' As of 1.3 this method is just a cover for
		''' <code>java.awt.EventQueue.isDispatchThread()</code>.
		''' </summary>
		''' <returns> true if the current thread is an AWT event dispatching thread </returns>
		Public Property Shared eventDispatchThread As Boolean
			Get
				Return EventQueue.dispatchThread
			End Get
		End Property


	'    
	'     * --- Accessibility Support ---
	'     *
	'     

		''' <summary>
		''' Get the index of this object in its accessible parent.<p>
		''' 
		''' Note: as of the Java 2 platform v1.3, it is recommended that developers call
		''' Component.AccessibleAWTComponent.getAccessibleIndexInParent() instead
		''' of using this method.
		''' </summary>
		''' <returns> -1 of this object does not have an accessible parent.
		''' Otherwise, the index of the child in its accessible parent. </returns>
		Public Shared Function getAccessibleIndexInParent(ByVal c As Component) As Integer
			Return c.accessibleContext.accessibleIndexInParent
		End Function

		''' <summary>
		''' Returns the <code>Accessible</code> child contained at the
		''' local coordinate <code>Point</code>, if one exists.
		''' Otherwise returns <code>null</code>.
		''' </summary>
		''' <returns> the <code>Accessible</code> at the specified location,
		'''    if it exists; otherwise <code>null</code> </returns>
		Public Shared Function getAccessibleAt(ByVal c As Component, ByVal p As Point) As Accessible
			If TypeOf c Is Container Then
				Return c.accessibleContext.accessibleComponent.getAccessibleAt(p)
			ElseIf TypeOf c Is Accessible Then
				Dim a As Accessible = CType(c, Accessible)
				If a IsNot Nothing Then
					Dim ac As AccessibleContext = a.accessibleContext
					If ac IsNot Nothing Then
						Dim acmp As AccessibleComponent
						Dim location As Point
						Dim nchildren As Integer = ac.accessibleChildrenCount
						For i As Integer = 0 To nchildren - 1
							a = ac.getAccessibleChild(i)
							If (a IsNot Nothing) Then
								ac = a.accessibleContext
								If ac IsNot Nothing Then
									acmp = ac.accessibleComponent
									If (acmp IsNot Nothing) AndAlso (acmp.showing) Then
										location = acmp.location
										Dim np As New Point(p.x-location.x, p.y-location.y)
										If acmp.contains(np) Then Return a
									End If
								End If
							End If
						Next i
					End If
				End If
				Return CType(c, Accessible)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Get the state of this object. <p>
		''' 
		''' Note: as of the Java 2 platform v1.3, it is recommended that developers call
		''' Component.AccessibleAWTComponent.getAccessibleIndexInParent() instead
		''' of using this method.
		''' </summary>
		''' <returns> an instance of AccessibleStateSet containing the current state
		''' set of the object </returns>
		''' <seealso cref= AccessibleState </seealso>
		Public Shared Function getAccessibleStateSet(ByVal c As Component) As AccessibleStateSet
			Return c.accessibleContext.accessibleStateSet
		End Function

		''' <summary>
		''' Returns the number of accessible children in the object.  If all
		''' of the children of this object implement Accessible, than this
		''' method should return the number of children of this object. <p>
		''' 
		''' Note: as of the Java 2 platform v1.3, it is recommended that developers call
		''' Component.AccessibleAWTComponent.getAccessibleIndexInParent() instead
		''' of using this method.
		''' </summary>
		''' <returns> the number of accessible children in the object. </returns>
		Public Shared Function getAccessibleChildrenCount(ByVal c As Component) As Integer
			Return c.accessibleContext.accessibleChildrenCount
		End Function

		''' <summary>
		''' Return the nth Accessible child of the object. <p>
		''' 
		''' Note: as of the Java 2 platform v1.3, it is recommended that developers call
		''' Component.AccessibleAWTComponent.getAccessibleIndexInParent() instead
		''' of using this method.
		''' </summary>
		''' <param name="i"> zero-based index of child </param>
		''' <returns> the nth Accessible child of the object </returns>
		Public Shared Function getAccessibleChild(ByVal c As Component, ByVal i As Integer) As Accessible
			Return c.accessibleContext.getAccessibleChild(i)
		End Function

		''' <summary>
		''' Return the child <code>Component</code> of the specified
		''' <code>Component</code> that is the focus owner, if any.
		''' </summary>
		''' <param name="c"> the root of the <code>Component</code> hierarchy to
		'''        search for the focus owner </param>
		''' <returns> the focus owner, or <code>null</code> if there is no focus
		'''         owner, or if the focus owner is not <code>comp</code>, or a
		'''         descendant of <code>comp</code>
		''' </returns>
		''' <seealso cref= java.awt.KeyboardFocusManager#getFocusOwner </seealso>
		''' @deprecated As of 1.4, replaced by
		'''   <code>KeyboardFocusManager.getFocusOwner()</code>. 
		<Obsolete("As of 1.4, replaced by")> _
		Public Shared Function findFocusOwner(ByVal c As Component) As Component
			Dim focusOwner As Component = KeyboardFocusManager.currentKeyboardFocusManager.focusOwner

			' verify focusOwner is a descendant of c
			Dim temp As Component = focusOwner
			Do While temp IsNot Nothing
				If temp Is c Then Return focusOwner
				temp = IIf(TypeOf temp Is Window, Nothing, temp.parent;)
			Loop

			Return Nothing
		End Function

		''' <summary>
		''' If c is a JRootPane descendant return its JRootPane ancestor.
		''' If c is a RootPaneContainer then return its JRootPane. </summary>
		''' <returns> the JRootPane for Component c or {@code null}. </returns>
		Public Shared Function getRootPane(ByVal c As Component) As JRootPane
			If TypeOf c Is RootPaneContainer Then Return CType(c, RootPaneContainer).rootPane
			Do While c IsNot Nothing
				If TypeOf c Is JRootPane Then Return CType(c, JRootPane)
				c = c.parent
			Loop
			Return Nothing
		End Function


		''' <summary>
		''' Returns the root component for the current component tree. </summary>
		''' <returns> the first ancestor of c that's a Window or the last Applet ancestor </returns>
		Public Shared Function getRoot(ByVal c As Component) As Component
			Dim applet As Component = Nothing
			Dim p As Component = c
			Do While p IsNot Nothing
				If TypeOf p Is Window Then Return p
				If TypeOf p Is Applet Then applet = p
				p = p.parent
			Loop
			Return applet
		End Function

		Friend Shared Function getPaintingOrigin(ByVal c As JComponent) As JComponent
			Dim p As Container = c
			p = p.parent
			Do While TypeOf p Is JComponent
				Dim jp As JComponent = CType(p, JComponent)
				If jp.paintingOrigin Then Return jp
				p = p.parent
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Process the key bindings for the <code>Component</code> associated with
		''' <code>event</code>. This method is only useful if
		''' <code>event.getComponent()</code> does not descend from
		''' <code>JComponent</code>, or your are not invoking
		''' <code>super.processKeyEvent</code> from within your
		''' <code>JComponent</code> subclass. <code>JComponent</code>
		''' automatically processes bindings from within its
		''' <code>processKeyEvent</code> method, hence you rarely need
		''' to directly invoke this method.
		''' </summary>
		''' <param name="event"> KeyEvent used to identify which bindings to process, as
		'''              well as which Component has focus. </param>
		''' <returns> true if a binding has found and processed
		''' @since 1.4 </returns>
		Public Shared Function processKeyBindings(ByVal [event] As KeyEvent) As Boolean
			If [event] IsNot Nothing Then
				If [event].consumed Then Return False

				Dim component As Component = [event].component
				Dim pressed As Boolean = ([event].iD = KeyEvent.KEY_PRESSED)

				If Not isValidKeyEventForKeyBindings([event]) Then Return False
				' Find the first JComponent in the ancestor hierarchy, and
				' invoke processKeyBindings on it
				Do While component IsNot Nothing
					If TypeOf component Is JComponent Then Return CType(component, JComponent).processKeyBindings([event], pressed)
					If (TypeOf component Is Applet) OrElse (TypeOf component Is Window) Then Return JComponent.processKeyBindingsForAllComponents([event], CType(component, Container), pressed)
					component = component.parent
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' Returns true if the <code>e</code> is a valid KeyEvent to use in
		''' processing the key bindings associated with JComponents.
		''' </summary>
		Friend Shared Function isValidKeyEventForKeyBindings(ByVal e As KeyEvent) As Boolean
			Return True
		End Function

		''' <summary>
		''' Invokes <code>actionPerformed</code> on <code>action</code> if
		''' <code>action</code> is enabled (and non-{@code null}). The command for the
		''' ActionEvent is determined by:
		''' <ol>
		'''   <li>If the action was registered via
		'''       <code>registerKeyboardAction</code>, then the command string
		'''       passed in ({@code null} will be used if {@code null} was passed in).
		'''   <li>Action value with name Action.ACTION_COMMAND_KEY, unless {@code null}.
		'''   <li>String value of the KeyEvent, unless <code>getKeyChar</code>
		'''       returns KeyEvent.CHAR_UNDEFINED..
		''' </ol>
		''' This will return true if <code>action</code> is non-{@code null} and
		''' actionPerformed is invoked on it.
		''' 
		''' @since 1.3
		''' </summary>
		Public Shared Function notifyAction(ByVal action As Action, ByVal ks As KeyStroke, ByVal [event] As KeyEvent, ByVal sender As Object, ByVal modifiers As Integer) As Boolean
			If action Is Nothing Then Return False
			If TypeOf action Is sun.swing.UIAction Then
				If Not CType(action, sun.swing.UIAction).isEnabled(sender) Then Return False
			ElseIf Not action.enabled Then
				Return False
			End If
			Dim commandO As Object
			Dim stayNull As Boolean

			' Get the command object.
			commandO = action.getValue(Action.ACTION_COMMAND_KEY)
			If commandO Is Nothing AndAlso (TypeOf action Is JComponent.ActionStandin) Then
				' ActionStandin is used for historical reasons to support
				' registerKeyboardAction with a null value.
				stayNull = True
			Else
				stayNull = False
			End If

			' Convert it to a string.
			Dim command As String

			If commandO IsNot Nothing Then
				command = commandO.ToString()
			ElseIf (Not stayNull) AndAlso [event].keyChar <> KeyEvent.CHAR_UNDEFINED Then
				command = Convert.ToString([event].keyChar)
			Else
				' Do null for undefined chars, or if registerKeyboardAction
				' was called with a null.
				command = Nothing
			End If
			action.actionPerformed(New ActionEvent(sender, ActionEvent.ACTION_PERFORMED, command, [event].when, modifiers))
			Return True
		End Function


		''' <summary>
		''' Convenience method to change the UI InputMap for <code>component</code>
		''' to <code>uiInputMap</code>. If <code>uiInputMap</code> is {@code null},
		''' this removes any previously installed UI InputMap.
		''' 
		''' @since 1.3
		''' </summary>
		Public Shared Sub replaceUIInputMap(ByVal component As JComponent, ByVal type As Integer, ByVal uiInputMap As InputMap)
			Dim map As InputMap = component.getInputMap(type, (uiInputMap IsNot Nothing))

			Do While map IsNot Nothing
				Dim parent As InputMap = map.parent
				If parent Is Nothing OrElse (TypeOf parent Is javax.swing.plaf.UIResource) Then
					map.parent = uiInputMap
					Return
				End If
				map = parent
			Loop
		End Sub


		''' <summary>
		''' Convenience method to change the UI ActionMap for <code>component</code>
		''' to <code>uiActionMap</code>. If <code>uiActionMap</code> is {@code null},
		''' this removes any previously installed UI ActionMap.
		''' 
		''' @since 1.3
		''' </summary>
		Public Shared Sub replaceUIActionMap(ByVal component As JComponent, ByVal uiActionMap As ActionMap)
			Dim map As ActionMap = component.getActionMap((uiActionMap IsNot Nothing))

			Do While map IsNot Nothing
				Dim parent As ActionMap = map.parent
				If parent Is Nothing OrElse (TypeOf parent Is javax.swing.plaf.UIResource) Then
					map.parent = uiActionMap
					Return
				End If
				map = parent
			Loop
		End Sub


		''' <summary>
		''' Returns the InputMap provided by the UI for condition
		''' <code>condition</code> in component <code>component</code>.
		''' <p>This will return {@code null} if the UI has not installed a InputMap
		''' of the specified type.
		''' 
		''' @since 1.3
		''' </summary>
		Public Shared Function getUIInputMap(ByVal component As JComponent, ByVal condition As Integer) As InputMap
			Dim map As InputMap = component.getInputMap(condition, False)
			Do While map IsNot Nothing
				Dim parent As InputMap = map.parent
				If TypeOf parent Is javax.swing.plaf.UIResource Then Return parent
				map = parent
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Returns the ActionMap provided by the UI
		''' in component <code>component</code>.
		''' <p>This will return {@code null} if the UI has not installed an ActionMap.
		''' 
		''' @since 1.3
		''' </summary>
		Public Shared Function getUIActionMap(ByVal component As JComponent) As ActionMap
			Dim map As ActionMap = component.getActionMap(False)
			Do While map IsNot Nothing
				Dim parent As ActionMap = map.parent
				If TypeOf parent Is javax.swing.plaf.UIResource Then Return parent
				map = parent
			Loop
			Return Nothing
		End Function


		' Don't use String, as it's not guaranteed to be unique in a Hashtable.
		Private Shared ReadOnly sharedOwnerFrameKey As Object = New StringBuilder("SwingUtilities.sharedOwnerFrame")

		Friend Class SharedOwnerFrame
			Inherits Frame
			Implements WindowListener

			Public Overridable Sub addNotify()
				MyBase.addNotify()
				installListeners()
			End Sub

			''' <summary>
			''' Install window listeners on owned windows to watch for displayability changes
			''' </summary>
			Friend Overridable Sub installListeners()
				Dim windows As Window() = ownedWindows
				For Each window As Window In windows
					If window IsNot Nothing Then
						window.removeWindowListener(Me)
						window.addWindowListener(Me)
					End If
				Next window
			End Sub

			''' <summary>
			''' Watches for displayability changes and disposes shared instance if there are no
			''' displayable children left.
			''' </summary>
			Public Overridable Sub windowClosed(ByVal e As WindowEvent)
				SyncLock treeLock
					Dim windows As Window() = ownedWindows
					For Each window As Window In windows
						If window IsNot Nothing Then
							If window.displayable Then Return
							window.removeWindowListener(Me)
						End If
					Next window
					Dispose()
				End SyncLock
			End Sub
			Public Overridable Sub windowOpened(ByVal e As WindowEvent)
			End Sub
			Public Overridable Sub windowClosing(ByVal e As WindowEvent)
			End Sub
			Public Overridable Sub windowIconified(ByVal e As WindowEvent)
			End Sub
			Public Overridable Sub windowDeiconified(ByVal e As WindowEvent)
			End Sub
			Public Overridable Sub windowActivated(ByVal e As WindowEvent)
			End Sub
			Public Overridable Sub windowDeactivated(ByVal e As WindowEvent)
			End Sub

			Public Overridable Sub show()
				' This frame can never be shown
			End Sub
			Public Overridable Sub dispose()
				Try
					toolkit.systemEventQueue
					MyBase.Dispose()
				Catch e As Exception
					' untrusted code not allowed to dispose
				End Try
			End Sub
		End Class

		''' <summary>
		''' Returns a toolkit-private, shared, invisible Frame
		''' to be the owner for JDialogs and JWindows created with
		''' {@code null} owners. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Friend Property Shared sharedOwnerFrame As Frame
			Get
				Dim ___sharedOwnerFrame As Frame = CType(SwingUtilities.appContextGet(sharedOwnerFrameKey), Frame)
				If ___sharedOwnerFrame Is Nothing Then
					___sharedOwnerFrame = New SharedOwnerFrame
					SwingUtilities.appContextPut(sharedOwnerFrameKey, ___sharedOwnerFrame)
				End If
				Return ___sharedOwnerFrame
			End Get
		End Property

		''' <summary>
		''' Returns a SharedOwnerFrame's shutdown listener to dispose the SharedOwnerFrame
		''' if it has no more displayable children. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Friend Property Shared sharedOwnerFrameShutdownListener As WindowListener
			Get
				Dim ___sharedOwnerFrame As Frame = sharedOwnerFrame
				Return CType(___sharedOwnerFrame, WindowListener)
			End Get
		End Property

	'     Don't make these AppContext accessors public or protected --
	'     * since AppContext is in sun.awt in 1.2, we shouldn't expose it
	'     * even indirectly with a public API.
	'     
		' REMIND(aim): phase out use of 4 methods below since they
		' are just private covers for AWT methods (?)

		Friend Shared Function appContextGet(ByVal key As Object) As Object
			Return sun.awt.AppContext.appContext.get(key)
		End Function

		Friend Shared Sub appContextPut(ByVal key As Object, ByVal value As Object)
			sun.awt.AppContext.appContext.put(key, value)
		End Sub

		Friend Shared Sub appContextRemove(ByVal key As Object)
			sun.awt.AppContext.appContext.remove(key)
		End Sub


		Friend Shared Function loadSystemClass(ByVal className As String) As Type
			sun.reflect.misc.ReflectUtil.checkPackageAccess(className)
			Return Type.GetType(className, True, Thread.CurrentThread.contextClassLoader)
		End Function


	'   
	'     * Convenience function for determining ComponentOrientation.  Helps us
	'     * avoid having Munge directives throughout the code.
	'     
		Friend Shared Function isLeftToRight(ByVal c As Component) As Boolean
			Return c.componentOrientation.leftToRight
		End Function
		Private Sub New()
			Throw New Exception("SwingUtilities is just a container for static methods")
		End Sub

		''' <summary>
		''' Returns true if the Icon <code>icon</code> is an instance of
		''' ImageIcon, and the image it contains is the same as <code>image</code>.
		''' </summary>
		Friend Shared Function doesIconReferenceImage(ByVal icon As Icon, ByVal image As Image) As Boolean
			Dim iconImage As Image = If(icon IsNot Nothing AndAlso (TypeOf icon Is ImageIcon), CType(icon, ImageIcon).image, Nothing)
			Return (iconImage Is image)
		End Function

		''' <summary>
		''' Returns index of the first occurrence of <code>mnemonic</code>
		''' within string <code>text</code>. Matching algorithm is not
		''' case-sensitive.
		''' </summary>
		''' <param name="text"> The text to search through, may be {@code null} </param>
		''' <param name="mnemonic"> The mnemonic to find the character for. </param>
		''' <returns> index into the string if exists, otherwise -1 </returns>
		Friend Shared Function findDisplayedMnemonicIndex(ByVal text As String, ByVal mnemonic As Integer) As Integer
			If text Is Nothing OrElse mnemonic = ControlChars.NullChar Then Return -1

			Dim uc As Char = Char.ToUpper(ChrW(mnemonic))
			Dim lc As Char = Char.ToLower(ChrW(mnemonic))

			Dim uci As Integer = text.IndexOf(uc)
			Dim lci As Integer = text.IndexOf(lc)

			If uci = -1 Then
				Return lci
			ElseIf lci = -1 Then
				Return uci
			Else
				Return If(lci < uci, lci, uci)
			End If
		End Function

		''' <summary>
		''' Stores the position and size of
		''' the inner painting area of the specified component
		''' in <code>r</code> and returns <code>r</code>.
		''' The position and size specify the bounds of the component,
		''' adjusted so as not to include the border area (the insets).
		''' This method is useful for classes
		''' that implement painting code.
		''' </summary>
		''' <param name="c">  the JComponent in question; if {@code null}, this method returns {@code null} </param>
		''' <param name="r">  the Rectangle instance to be modified;
		'''           may be {@code null} </param>
		''' <returns> {@code null} if the Component is {@code null};
		'''         otherwise, returns the passed-in rectangle (if non-{@code null})
		'''         or a new rectangle specifying position and size information
		''' 
		''' @since 1.4 </returns>
		Public Shared Function calculateInnerArea(ByVal c As JComponent, ByVal r As Rectangle) As Rectangle
			If c Is Nothing Then Return Nothing
			Dim rect As Rectangle = r
			Dim insets As Insets = c.insets

			If rect Is Nothing Then rect = New Rectangle

			rect.x = insets.left
			rect.y = insets.top
			rect.width = c.width - insets.left - insets.right
			rect.height = c.height - insets.top - insets.bottom

			Return rect
		End Function

		Friend Shared Sub updateRendererOrEditorUI(ByVal rendererOrEditor As Object)
			If rendererOrEditor Is Nothing Then Return

			Dim component As Component = Nothing

			If TypeOf rendererOrEditor Is Component Then component = CType(rendererOrEditor, Component)
			If TypeOf rendererOrEditor Is DefaultCellEditor Then component = CType(rendererOrEditor, DefaultCellEditor).component

			If component IsNot Nothing Then SwingUtilities.updateComponentTreeUI(component)
		End Sub

		''' <summary>
		''' Returns the first ancestor of the {@code component}
		''' which is not an instance of <seealso cref="JLayer"/>.
		''' </summary>
		''' <param name="component"> {@code Component} to get
		''' the first ancestor of, which is not a <seealso cref="JLayer"/> instance.
		''' </param>
		''' <returns> the first ancestor of the {@code component}
		''' which is not an instance of <seealso cref="JLayer"/>.
		''' If such an ancestor can not be found, {@code null} is returned.
		''' </returns>
		''' <exception cref="NullPointerException"> if {@code component} is {@code null} </exception>
		''' <seealso cref= JLayer
		''' 
		''' @since 1.7 </seealso>
		Public Shared Function getUnwrappedParent(ByVal component As Component) As Container
			Dim parent As Container = component.parent
			Do While TypeOf parent Is JLayer
				parent = parent.parent
			Loop
			Return parent
		End Function

		''' <summary>
		''' Returns the first {@code JViewport}'s descendant
		''' which is not an instance of {@code JLayer}.
		''' If such a descendant can not be found, {@code null} is returned.
		''' 
		''' If the {@code viewport}'s view component is not a {@code JLayer},
		''' this method is equivalent to <seealso cref="JViewport#getView()"/>
		''' otherwise <seealso cref="JLayer#getView()"/> will be recursively
		''' called on all descending {@code JLayer}s.
		''' </summary>
		''' <param name="viewport"> {@code JViewport} to get the first descendant of,
		''' which in not a {@code JLayer} instance.
		''' </param>
		''' <returns> the first {@code JViewport}'s descendant
		''' which is not an instance of {@code JLayer}.
		''' If such a descendant can not be found, {@code null} is returned.
		''' </returns>
		''' <exception cref="NullPointerException"> if {@code viewport} is {@code null} </exception>
		''' <seealso cref= JViewport#getView() </seealso>
		''' <seealso cref= JLayer
		''' 
		''' @since 1.7 </seealso>
		Public Shared Function getUnwrappedView(ByVal viewport As JViewport) As Component
			Dim view As Component = viewport.view
			Do While TypeOf view Is JLayer
				view = CType(view, JLayer).view
			Loop
			Return view
		End Function

	   ''' <summary>
	   ''' Retrieves the validate root of a given container.
	   '''  
	   ''' If the container is contained within a {@code CellRendererPane}, this
	   ''' method returns {@code null} due to the synthetic nature of the {@code
	   ''' CellRendererPane}.
	   ''' <p>
	   ''' The component hierarchy must be displayable up to the toplevel component
	   ''' (either a {@code Frame} or an {@code Applet} object.) Otherwise this
	   ''' method returns {@code null}.
	   ''' <p>
	   ''' If the {@code visibleOnly} argument is {@code true}, the found validate
	   ''' root and all its parents up to the toplevel component must also be
	   ''' visible. Otherwise this method returns {@code null}.
	   ''' </summary>
	   ''' <returns> the validate root of the given container or null </returns>
	   ''' <seealso cref= java.awt.Component#isDisplayable() </seealso>
	   ''' <seealso cref= java.awt.Component#isVisible()
	   ''' @since 1.7 </seealso>
		Friend Shared Function getValidateRoot(ByVal c As Container, ByVal visibleOnly As Boolean) As Container
			Dim ___root As Container = Nothing

			Do While c IsNot Nothing
				If (Not c.displayable) OrElse TypeOf c Is CellRendererPane Then Return Nothing
				If c.validateRoot Then
					___root = c
					Exit Do
				End If
				c = c.parent
			Loop

			If ___root Is Nothing Then Return Nothing

			Do While c IsNot Nothing
				If (Not c.displayable) OrElse (visibleOnly AndAlso (Not c.visible)) Then Return Nothing
				If TypeOf c Is Window OrElse TypeOf c Is Applet Then Return ___root
				c = c.parent
			Loop

			Return Nothing
		End Function
	End Class

End Namespace
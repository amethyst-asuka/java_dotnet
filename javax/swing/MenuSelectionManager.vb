Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports javax.swing.event

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A MenuSelectionManager owns the selection in menu hierarchy.
	''' 
	''' @author Arnaud Weber
	''' </summary>
	Public Class MenuSelectionManager
		Private selection As New List(Of MenuElement)

		' diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes
		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		Private Shared ReadOnly MENU_SELECTION_MANAGER_KEY As New StringBuilder("javax.swing.MenuSelectionManager")

		''' <summary>
		''' Returns the default menu selection manager.
		''' </summary>
		''' <returns> a MenuSelectionManager object </returns>
		Public Shared Function defaultManager() As MenuSelectionManager
			SyncLock MENU_SELECTION_MANAGER_KEY
				Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim msm As MenuSelectionManager = CType(context.get(MENU_SELECTION_MANAGER_KEY), MenuSelectionManager)
				If msm Is Nothing Then
					msm = New MenuSelectionManager
					context.put(MENU_SELECTION_MANAGER_KEY, msm)

					' installing additional listener if found in the AppContext
					Dim o As Object = context.get(sun.swing.SwingUtilities2.MENU_SELECTION_MANAGER_LISTENER_KEY)
					If o IsNot Nothing AndAlso TypeOf o Is ChangeListener Then msm.addChangeListener(CType(o, ChangeListener))
				End If

				Return msm
			End SyncLock
		End Function

		''' <summary>
		''' Only one ChangeEvent is needed per button model instance since the
		''' event's only state is the source property.  The source of events
		''' generated is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing
		Protected Friend listenerList As New EventListenerList

		''' <summary>
		''' Changes the selection in the menu hierarchy.  The elements
		''' in the array are sorted in order from the root menu
		''' element to the currently selected menu element.
		''' <p>
		''' Note that this method is public but is used by the look and
		''' feel engine and should not be called by client applications.
		''' </summary>
		''' <param name="path">  an array of <code>MenuElement</code> objects specifying
		'''        the selected path </param>
		Public Overridable Property selectedPath As MenuElement()
			Set(ByVal path As MenuElement())
				Dim i, c As Integer
				Dim currentSelectionCount As Integer = selection.Count
				Dim firstDifference As Integer = 0
    
				If path Is Nothing Then path = New MenuElement(){}
    
				If DEBUG Then
					Console.Write("Previous:  ")
					printMenuElementArray(selectedPath)
					Console.Write("New:  ")
					printMenuElementArray(path)
				End If
    
				i=0
				c=path.Length
				Do While i<c
					If i < currentSelectionCount AndAlso selection(i) Is path(i) Then
						firstDifference += 1
					Else
						Exit Do
					End If
					i += 1
				Loop
    
				For i = currentSelectionCount - 1 To firstDifference Step -1
					Dim [me] As MenuElement = selection(i)
					selection.RemoveAt(i)
					[me].menuSelectionChanged(False)
				Next i
    
				i = firstDifference
				c = path.Length
				Do While i < c
					If path(i) IsNot Nothing Then
						selection.Add(path(i))
						path(i).menuSelectionChanged(True)
					End If
					i += 1
				Loop
    
				fireStateChanged()
			End Set
			Get
				Dim res As MenuElement() = New MenuElement(selection.Count - 1){}
				Dim i, c As Integer
				i=0
				c=selection.Count
				Do While i<c
					res(i) = selection(i)
					i += 1
				Loop
				Return res
			End Get
		End Property


		''' <summary>
		''' Tell the menu selection to close and unselect all the menu components. Call this method
		''' when a choice has been made
		''' </summary>
		Public Overridable Sub clearSelectedPath()
			If selection.Count > 0 Then selectedPath = Nothing
		End Sub

		''' <summary>
		''' Adds a ChangeListener to the button.
		''' </summary>
		''' <param name="l"> the listener to add </param>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener)
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a ChangeListener from the button.
		''' </summary>
		''' <param name="l"> the listener to remove </param>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener)
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this MenuSelectionManager with addChangeListener().
		''' </summary>
		''' <returns> all of the <code>ChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			' Guaranteed to return a non-null array
			Dim listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = listeners.Length-2 To 0 Step -2
				If listeners(i) Is GetType(ChangeListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' When a MenuElement receives an event from a MouseListener, it should never process the event
		''' directly. Instead all MenuElements should call this method with the event.
		''' </summary>
		''' <param name="event">  a MouseEvent object </param>
		Public Overridable Sub processMouseEvent(ByVal [event] As MouseEvent)
			Dim screenX, screenY As Integer
			Dim p As Point
			Dim i, c, j, d As Integer
			Dim mc As Component
			Dim r2 As Rectangle
			Dim cWidth, cHeight As Integer
			Dim menuElement As MenuElement
			Dim subElements As MenuElement()
			Dim path As MenuElement()
			Dim tmp As List(Of MenuElement)
			Dim selectionSize As Integer
			p = [event].point

			Dim source As Component = [event].component

			If (source IsNot Nothing) AndAlso (Not source.showing) Then Return

			Dim type As Integer = [event].iD
			Dim modifiers As Integer = [event].modifiers
			' 4188027: drag enter/exit added in JDK 1.1.7A, JDK1.2
			If (type=MouseEvent.MOUSE_ENTERED OrElse type=MouseEvent.MOUSE_EXITED) AndAlso ((modifiers And (InputEvent.BUTTON1_MASK Or InputEvent.BUTTON2_MASK Or InputEvent.BUTTON3_MASK)) <>0) Then Return

			If source IsNot Nothing Then SwingUtilities.convertPointToScreen(p, source)

			screenX = p.x
			screenY = p.y

			tmp = CType(selection.clone(), List(Of MenuElement))
			selectionSize = tmp.Count
			Dim success As Boolean = False
			i=selectionSize - 1
			Do While i >= 0 AndAlso success = False
				menuElement = CType(tmp(i), MenuElement)
				subElements = menuElement.subElements

				path = Nothing
				j = 0
				d = subElements.Length
				Do While j < d AndAlso success = False
					If subElements(j) Is Nothing Then
						j += 1
						Continue Do
					End If
					mc = subElements(j).component
					If Not mc.showing Then
						j += 1
						Continue Do
					End If
					If TypeOf mc Is JComponent Then
						cWidth = mc.width
						cHeight = mc.height
					Else
						r2 = mc.bounds
						cWidth = r2.width
						cHeight = r2.height
					End If
					p.x = screenX
					p.y = screenY
					SwingUtilities.convertPointFromScreen(p,mc)

					''' <summary>
					''' Send the event to visible menu element if menu element currently in
					'''  the selected path or contains the event location
					''' </summary>
					If (p.x >= 0 AndAlso p.x < cWidth AndAlso p.y >= 0 AndAlso p.y < cHeight) Then
						Dim k As Integer
						If path Is Nothing Then
							path = New MenuElement(i+2 - 1){}
							For k = 0 To i
								path(k) = CType(tmp(k), MenuElement)
							Next k
						End If
						path(i+1) = subElements(j)
						Dim currentSelection As MenuElement() = selectedPath

						' Enter/exit detection -- needs tuning...
						If currentSelection(currentSelection.Length-1) IsNot path(i+1) AndAlso (currentSelection.Length < 2 OrElse currentSelection(currentSelection.Length-2) IsNot path(i+1)) Then
							Dim oldMC As Component = currentSelection(currentSelection.Length-1).component

							Dim exitEvent As New MouseEvent(oldMC, MouseEvent.MOUSE_EXITED, [event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)
							currentSelection(currentSelection.Length-1).processMouseEvent(exitEvent, path, Me)

							Dim enterEvent As New MouseEvent(mc, MouseEvent.MOUSE_ENTERED, [event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)
							subElements(j).processMouseEvent(enterEvent, path, Me)
						End If
						Dim mouseEvent As New MouseEvent(mc, [event].iD,[event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)
						subElements(j).processMouseEvent(mouseEvent, path, Me)
						success = True
						[event].consume()
					End If
					j += 1
				Loop
				i -= 1
			Loop
		End Sub

		Private Sub printMenuElementArray(ByVal path As MenuElement())
			printMenuElementArray(path, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private void printMenuElementArray(MenuElement path() , boolean dumpStack)
			Console.WriteLine("Path is(")
			Dim i, j As Integer
			i=0
			j=path.length
			Do While i<j
				For k As Integer = 0 To i
					Console.Write("  ")
				Next k
				Dim [me] As MenuElement = path(i)
				If TypeOf [me] Is JMenuItem Then
					Console.WriteLine(CType([me], JMenuItem).text & ", ")
				ElseIf TypeOf [me] Is JMenuBar Then
					Console.WriteLine("JMenuBar, ")
				ElseIf TypeOf [me] Is JPopupMenu Then
					Console.WriteLine("JPopupMenu, ")
				ElseIf [me] Is Nothing Then
					Console.WriteLine("NULL , ")
				Else
					Console.WriteLine("" & [me] & ", ")
				End If
				i += 1
			Loop
			Console.WriteLine(")")

			If dumpStack = True Then Thread.dumpStack()

		''' <summary>
		''' Returns the component in the currently selected path
		''' which contains sourcePoint.
		''' </summary>
		''' <param name="source"> The component in whose coordinate space sourcePoint
		'''        is given </param>
		''' <param name="sourcePoint"> The point which is being tested </param>
		''' <returns> The component in the currently selected path which
		'''         contains sourcePoint (relative to the source component's
		'''         coordinate space.  If sourcePoint is not inside a component
		'''         on the currently selected path, null is returned. </returns>
		public Component componentForPoint(Component source, Point sourcePoint)
			Dim screenX, screenY As Integer
			Dim p As Point = sourcePoint
			Dim i, c, j, d As Integer
			Dim mc As Component
			Dim r2 As Rectangle
			Dim cWidth, cHeight As Integer
			Dim menuElement As MenuElement
			Dim subElements As MenuElement()
			Dim tmp As List(Of MenuElement)
			Dim selectionSize As Integer

			SwingUtilities.convertPointToScreen(p,source)

			screenX = p.x
			screenY = p.y

			tmp = CType(selection.clone(), List(Of MenuElement))
			selectionSize = tmp.Count
			For i = selectionSize - 1 To 0 Step -1
				menuElement = CType(tmp(i), MenuElement)
				subElements = menuElement.subElements

				j = 0
				d = subElements.Length
				Do While j < d
					If subElements(j) Is Nothing Then
						j += 1
						Continue Do
					End If
					mc = subElements(j).component
					If Not mc.showing Then
						j += 1
						Continue Do
					End If
					If TypeOf mc Is JComponent Then
						cWidth = mc.width
						cHeight = mc.height
					Else
						r2 = mc.bounds
						cWidth = r2.width
						cHeight = r2.height
					End If
					p.x = screenX
					p.y = screenY
					SwingUtilities.convertPointFromScreen(p,mc)

					''' <summary>
					''' Return the deepest component on the selection
					'''  path in whose bounds the event's point occurs
					''' </summary>
					If p.x >= 0 AndAlso p.x < cWidth AndAlso p.y >= 0 AndAlso p.y < cHeight Then Return mc
					j += 1
				Loop
			Next i
			Return Nothing

		''' <summary>
		''' When a MenuElement receives an event from a KeyListener, it should never process the event
		''' directly. Instead all MenuElements should call this method with the event.
		''' </summary>
		''' <param name="e">  a KeyEvent object </param>
		public void processKeyEvent(KeyEvent e)
			Dim sel2 As MenuElement() = New MenuElement(){}
			sel2 = selection.ToArray(sel2)
			Dim selSize As Integer = sel2.Length
			Dim path As MenuElement()

			If selSize < 1 Then Return

			For i As Integer = selSize-1 To 0 Step -1
				Dim elem As MenuElement = sel2(i)
				Dim subs As MenuElement() = elem.subElements
				path = Nothing

				For j As Integer = 0 To subs.Length - 1
					If subs(j) Is Nothing OrElse (Not subs(j).component.showing) OrElse (Not subs(j).component.enabled) Then Continue For

					If path Is Nothing Then
						path = New MenuElement(i+2 - 1){}
						Array.Copy(sel2, 0, path, 0, i+1)
					End If
					path(i+1) = subs(j)
					subs(j).processKeyEvent(e, path, Me)
					If e.consumed Then Return
				Next j
			Next i

			' finally dispatch event to the first component in path
			path = New MenuElement(0){}
			path(0) = sel2(0)
			path(0).processKeyEvent(e, path, Me)
			If e.consumed Then Return

		''' <summary>
		''' Return true if c is part of the currently used menu
		''' </summary>
		public Boolean isComponentPartOfCurrentMenu(Component c)
			If selection.Count > 0 Then
				Dim [me] As MenuElement = selection(0)
				Return isComponentPartOfCurrentMenu([me],c)
			Else
				Return False
			End If

		private Boolean isComponentPartOfCurrentMenu(MenuElement root,Component c)
			Dim children As MenuElement()
			Dim i, d As Integer

			If root Is Nothing Then Return False

			If root.component = c Then
				Return True
			Else
				children = root.subElements
				i=0
				d=children.Length
				Do While i<d
					If isComponentPartOfCurrentMenu(children(i),c) Then Return True
					i += 1
				Loop
			End If
			Return False
	End Class

End Namespace
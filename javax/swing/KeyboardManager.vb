Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Threading
Imports javax.swing.event

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The KeyboardManager class is used to help dispatch keyboard actions for the
	''' WHEN_IN_FOCUSED_WINDOW style actions.  Actions with other conditions are handled
	''' directly in JComponent.
	'''  
	''' Here's a description of the symantics of how keyboard dispatching should work
	''' atleast as I understand it.
	'''  
	''' KeyEvents are dispatched to the focused component.  The focus manager gets first
	''' crack at processing this event.  If the focus manager doesn't want it, then
	''' the JComponent calls super.processKeyEvent() this allows listeners a chance
	''' to process the event.
	'''  
	''' If none of the listeners "consumes" the event then the keybindings get a shot.
	''' This is where things start to get interesting.  First, KeyStokes defined with the
	''' WHEN_FOCUSED condition get a chance.  If none of these want the event, then the component
	''' walks though it's parents looked for actions of type WHEN_ANCESTOR_OF_FOCUSED_COMPONENT.
	'''  
	''' If no one has taken it yet, then it winds up here.  We then look for components registered
	''' for WHEN_IN_FOCUSED_WINDOW events and fire to them.  Note that if none of those are found
	''' then we pass the event to the menubars and let them have a crack at it.  They're handled differently.
	'''  
	''' Lastly, we check if we're looking at an internal frame.  If we are and no one wanted the event
	''' then we move up to the InternalFrame's creator and see if anyone wants the event (and so on and so on).
	'''  
	''' </summary>
	''' <seealso cref= InputMap </seealso>
	Friend Class KeyboardManager

		Friend Shared currentManager As New KeyboardManager

		''' <summary>
		''' maps top-level containers to a sub-hashtable full of keystrokes
		''' </summary>
		Friend containerMap As New Dictionary(Of Container, Hashtable)

		''' <summary>
		''' Maps component/keystroke pairs to a topLevel container
		''' This is mainly used for fast unregister operations
		''' </summary>
		Friend componentKeyStrokeMap As New Dictionary(Of ComponentKeyStrokePair, Container)

		Public Property Shared currentManager As KeyboardManager
			Get
				Return currentManager
			End Get
			Set(ByVal km As KeyboardManager)
				currentManager = km
			End Set
		End Property


		''' <summary>
		''' register keystrokes here which are for the WHEN_IN_FOCUSED_WINDOW
		''' case.
		''' Other types of keystrokes will be handled by walking the hierarchy
		''' That simplifies some potentially hairy stuff.
		''' </summary>
		 Public Overridable Sub registerKeyStroke(ByVal k As KeyStroke, ByVal c As JComponent)
			 Dim topContainer As Container = getTopAncestor(c)
			 If topContainer Is Nothing Then Return
			 Dim keyMap As Hashtable = containerMap(topContainer)

			 If keyMap Is Nothing Then ' lazy evaluate one keyMap = registerNewTopContainer(topContainer)

			 Dim tmp As Object = keyMap(k)
			 If tmp Is Nothing Then
				 keyMap(k) = c ' if there's a Vector there then add to it.
			 ElseIf TypeOf tmp Is ArrayList Then
				 Dim v As ArrayList = CType(tmp, ArrayList)
				 If Not v.Contains(c) Then ' only add if this keystroke isn't registered for this component v.Add(c)
			 ElseIf TypeOf tmp Is JComponent Then
			   ' if a JComponent is there then remove it and replace it with a vector
			   ' Then add the old compoennt and the new compoent to the vector
			   ' then insert the vector in the table
			   If tmp IsNot c Then ' this means this is already registered for this component, no need to dup
				   Dim v As New List(Of JComponent)
				   v.Add(CType(tmp, JComponent))
				   v.Add(c)
				   keyMap(k) = v
			   End If
			 Else
				 Console.WriteLine("Unexpected condition in registerKeyStroke")
				 Thread.dumpStack()
			 End If

			 componentKeyStrokeMap(New ComponentKeyStrokePair(Me, c,k)) = topContainer

			 ' Check for EmbeddedFrame case, they know how to process accelerators even
			 ' when focus is not in Java
			 If TypeOf topContainer Is sun.awt.EmbeddedFrame Then CType(topContainer, sun.awt.EmbeddedFrame).registerAccelerator(k)
		 End Sub

		 ''' <summary>
		 ''' Find the top focusable Window, Applet, or InternalFrame
		 ''' </summary>
		 Private Shared Function getTopAncestor(ByVal c As JComponent) As Container
			Dim p As Container = c.parent
			Do While p IsNot Nothing
				If TypeOf p Is Window AndAlso CType(p, Window).focusableWindow OrElse TypeOf p Is Applet OrElse TypeOf p Is JInternalFrame Then Return p
				p = p.parent
			Loop
			Return Nothing
		 End Function

		 Public Overridable Sub unregisterKeyStroke(ByVal ks As KeyStroke, ByVal c As JComponent)

		   ' component may have already been removed from the hierarchy, we
		   ' need to look up the container using the componentKeyStrokeMap.

			 Dim ckp As New ComponentKeyStrokePair(Me, c,ks)

			 Dim topContainer As Container = componentKeyStrokeMap(ckp)

			 If topContainer Is Nothing Then ' never heard of this pairing, so bail Return

			 Dim keyMap As Hashtable = containerMap(topContainer)
			 If keyMap Is Nothing Then ' this should never happen, but I'm being safe
				 Thread.dumpStack()
				 Return
			 End If

			 Dim tmp As Object = keyMap(ks)
			 If tmp Is Nothing Then ' this should never happen, but I'm being safe
				 Thread.dumpStack()
				 Return
			 End If

			 If TypeOf tmp Is JComponent AndAlso tmp Is c Then
				 keyMap.Remove(ks) ' remove the KeyStroke from the Map
				 'System.out.println("removed a stroke" + ks); ' this means there is more than one component reg for this key
			 ElseIf TypeOf tmp Is ArrayList Then
				 Dim v As ArrayList = CType(tmp, ArrayList)
				 v.Remove(c)
				 If v.Count = 0 Then keyMap.Remove(ks) ' remove the KeyStroke from the Map
			 End If

			 If keyMap.Count = 0 Then ' if no more bindings in this table containerMap.Remove(topContainer) ' remove table to enable GC

			 componentKeyStrokeMap.Remove(ckp)

			 ' Check for EmbeddedFrame case, they know how to process accelerators even
			 ' when focus is not in Java
			 If TypeOf topContainer Is sun.awt.EmbeddedFrame Then CType(topContainer, sun.awt.EmbeddedFrame).unregisterAccelerator(ks)
		 End Sub

		''' <summary>
		''' This method is called when the focused component (and none of
		''' its ancestors) want the key event.  This will look up the keystroke
		''' to see if any chidren (or subchildren) of the specified container
		''' want a crack at the event.
		''' If one of them wants it, then it will "DO-THE-RIGHT-THING"
		''' </summary>
		Public Overridable Function fireKeyboardAction(ByVal e As KeyEvent, ByVal pressed As Boolean, ByVal topAncestor As Container) As Boolean

			 If e.consumed Then
				  Console.WriteLine("Acquired pre-used event!")
				  Thread.dumpStack()
			 End If

			 ' There may be two keystrokes associated with a low-level key event;
			 ' in this case a keystroke made of an extended key code has a priority.
			 Dim ks As KeyStroke
			 Dim ksE As KeyStroke = Nothing


			 If e.iD = KeyEvent.KEY_TYPED Then
				   ks=KeyStroke.getKeyStroke(e.keyChar)
			 Else
				   If e.keyCode <> e.extendedKeyCode Then ksE=KeyStroke.getKeyStroke(e.extendedKeyCode, e.modifiers, (Not pressed))
				   ks=KeyStroke.getKeyStroke(e.keyCode, e.modifiers, (Not pressed))
			 End If

			 Dim keyMap As Hashtable = containerMap(topAncestor)
			 If keyMap IsNot Nothing Then ' this container isn't registered, so bail

				 Dim tmp As Object = Nothing
				 ' extended code has priority
				 If ksE IsNot Nothing Then
					 tmp = keyMap(ksE)
					 If tmp IsNot Nothing Then ks = ksE
				 End If
				 If tmp Is Nothing Then tmp = keyMap(ks)

				 If tmp Is Nothing Then
				   ' don't do anything
				 ElseIf TypeOf tmp Is JComponent Then
					 Dim c As JComponent = CType(tmp, JComponent)
					 If c.showing AndAlso c.enabled Then ' only give it out if enabled and visible
						 fireBinding(c, ks, e, pressed)
					 End If 'more than one comp registered for this
				 ElseIf TypeOf tmp Is ArrayList Then
					 Dim v As ArrayList = CType(tmp, ArrayList)
					 ' There is no well defined order for WHEN_IN_FOCUSED_WINDOW
					 ' bindings, but we give precedence to those bindings just
					 ' added. This is done so that JMenus WHEN_IN_FOCUSED_WINDOW
					 ' bindings are accessed before those of the JRootPane (they
					 ' both have a WHEN_IN_FOCUSED_WINDOW binding for enter).
					 For counter As Integer = v.Count - 1 To 0 Step -1
						 Dim c As JComponent = CType(v(counter), JComponent)
						 'System.out.println("Trying collision: " + c + " vector = "+ v.size());
						 If c.showing AndAlso c.enabled Then ' don't want to give these out
							 fireBinding(c, ks, e, pressed)
							 If e.consumed Then Return True
						 End If
					 Next counter
				 Else
					 Console.WriteLine("Unexpected condition in fireKeyboardAction " & tmp)
					 ' This means that tmp wasn't null, a JComponent, or a Vector.  What is it?
					 Thread.dumpStack()
				 End If
			 End If

			 If e.consumed Then Return True
			 ' if no one else handled it, then give the menus a crack
			 ' The're handled differently.  The key is to let any JMenuBars
			 ' process the event
			 If keyMap IsNot Nothing Then
				 Dim v As ArrayList = CType(keyMap(GetType(JMenuBar)), ArrayList)
				 If v IsNot Nothing Then
					 Dim iter As System.Collections.IEnumerator = v.elements()
					 Do While iter.hasMoreElements()
						 Dim mb As JMenuBar = CType(iter.nextElement(), JMenuBar)
						 If mb.showing AndAlso mb.enabled Then ' don't want to give these out
							 Dim extended As Boolean = (ksE IsNot Nothing) AndAlso Not ksE.Equals(ks)
							 If extended Then fireBinding(mb, ksE, e, pressed)
							 If (Not extended) OrElse (Not e.consumed) Then fireBinding(mb, ks, e, pressed)
							 If e.consumed Then Return True
						 End If
					 Loop
				 End If
			 End If

			 Return e.consumed
		End Function

		Friend Overridable Sub fireBinding(ByVal c As JComponent, ByVal ks As KeyStroke, ByVal e As KeyEvent, ByVal pressed As Boolean)
			If c.processKeyBinding(ks, e, JComponent.WHEN_IN_FOCUSED_WINDOW, pressed) Then e.consume()
		End Sub

		Public Overridable Sub registerMenuBar(ByVal mb As JMenuBar)
			Dim top As Container = getTopAncestor(mb)
			If top Is Nothing Then Return
			Dim keyMap As Hashtable = containerMap(top)

			If keyMap Is Nothing Then ' lazy evaluate one keyMap = registerNewTopContainer(top)
			' use the menubar class as the key
			Dim menuBars As ArrayList = CType(keyMap(GetType(JMenuBar)), ArrayList)

			If menuBars Is Nothing Then ' if we don't have a list of menubars,
									 ' then make one.
				menuBars = New ArrayList
				keyMap(GetType(JMenuBar)) = menuBars
			End If

			If Not menuBars.Contains(mb) Then menuBars.Add(mb)
		End Sub


		Public Overridable Sub unregisterMenuBar(ByVal mb As JMenuBar)
			Dim topContainer As Container = getTopAncestor(mb)
			If topContainer Is Nothing Then Return
			Dim keyMap As Hashtable = containerMap(topContainer)
			If keyMap IsNot Nothing Then
				Dim v As ArrayList = CType(keyMap(GetType(JMenuBar)), ArrayList)
				If v IsNot Nothing Then
					v.Remove(mb)
					If v.Count = 0 Then
						keyMap.Remove(GetType(JMenuBar))
						If keyMap.Count = 0 Then containerMap.Remove(topContainer)
					End If
				End If
			End If
		End Sub
		Protected Friend Overridable Function registerNewTopContainer(ByVal topContainer As Container) As Hashtable
				 Dim keyMap As New Hashtable
				 containerMap(topContainer) = keyMap
				 Return keyMap
		End Function

		''' <summary>
		''' This class is used to create keys for a hashtable
		''' which looks up topContainers based on component, keystroke pairs
		''' This is used to make unregistering KeyStrokes fast
		''' </summary>
		Friend Class ComponentKeyStrokePair
			Private ReadOnly outerInstance As KeyboardManager

			Friend component As Object
			Friend keyStroke As Object

			Public Sub New(ByVal outerInstance As KeyboardManager, ByVal comp As Object, ByVal key As Object)
					Me.outerInstance = outerInstance
				component = comp
				keyStroke = key
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is ComponentKeyStrokePair) Then Return False
				Dim ckp As ComponentKeyStrokePair = CType(o, ComponentKeyStrokePair)
				Return ((component.Equals(ckp.component)) AndAlso (keyStroke.Equals(ckp.keyStroke)))
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return component.GetHashCode() * keyStroke.GetHashCode()
			End Function

		End Class

	End Class ' end KeyboardManager

End Namespace
Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>CardLayout</code> object is a layout manager for a
	''' container. It treats each component in the container as a card.
	''' Only one card is visible at a time, and the container acts as
	''' a stack of cards. The first component added to a
	''' <code>CardLayout</code> object is the visible component when the
	''' container is first displayed.
	''' <p>
	''' The ordering of cards is determined by the container's own internal
	''' ordering of its component objects. <code>CardLayout</code>
	''' defines a set of methods that allow an application to flip
	''' through these cards sequentially, or to show a specified card.
	''' The <seealso cref="CardLayout#addLayoutComponent"/>
	''' method can be used to associate a string identifier with a given card
	''' for fast random access.
	''' 
	''' @author      Arthur van Hoff </summary>
	''' <seealso cref=         java.awt.Container
	''' @since       JDK1.0 </seealso>

	<Serializable> _
	Public Class CardLayout
		Implements LayoutManager2

		Private Const serialVersionUID As Long = -4328196481005934313L

	'    
	'     * This creates a Vector to store associated
	'     * pairs of components and their names.
	'     * @see java.util.Vector
	'     
		Friend vector As New List(Of Card)

	'    
	'     * A pair of Component and String that represents its name.
	'     
		<Serializable> _
		Friend Class Card
			Private ReadOnly outerInstance As CardLayout

			Friend Const serialVersionUID As Long = 6640330810709497518L
			Public name As String
			Public comp As Component
			Public Sub New(ByVal outerInstance As CardLayout, ByVal cardName As String, ByVal cardComponent As Component)
					Me.outerInstance = outerInstance
				name = cardName
				comp = cardComponent
			End Sub
		End Class

	'    
	'     * Index of Component currently displayed by CardLayout.
	'     
		Friend currentCard As Integer = 0


	'    
	'    * A cards horizontal Layout gap (inset). It specifies
	'    * the space between the left and right edges of a
	'    * container and the current component.
	'    * This should be a non negative Integer.
	'    * @see getHgap()
	'    * @see setHgap()
	'    
		Friend hgap As Integer

	'    
	'    * A cards vertical Layout gap (inset). It specifies
	'    * the space between the top and bottom edges of a
	'    * container and the current component.
	'    * This should be a non negative Integer.
	'    * @see getVgap()
	'    * @see setVgap()
	'    
		Friend vgap As Integer

		''' <summary>
		''' @serialField tab         Hashtable
		'''      deprectated, for forward compatibility only
		''' @serialField hgap        int
		''' @serialField vgap        int
		''' @serialField vector      Vector
		''' @serialField currentCard int
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("tab", GetType(Hashtable)), New java.io.ObjectStreamField("hgap", Integer.TYPE), New java.io.ObjectStreamField("vgap", Integer.TYPE), New java.io.ObjectStreamField("vector", GetType(ArrayList)), New java.io.ObjectStreamField("currentCard", Integer.TYPE) }

		''' <summary>
		''' Creates a new card layout with gaps of size zero.
		''' </summary>
		Public Sub New()
			Me.New(0, 0)
		End Sub

		''' <summary>
		''' Creates a new card layout with the specified horizontal and
		''' vertical gaps. The horizontal gaps are placed at the left and
		''' right edges. The vertical gaps are placed at the top and bottom
		''' edges. </summary>
		''' <param name="hgap">   the horizontal gap. </param>
		''' <param name="vgap">   the vertical gap. </param>
		Public Sub New(ByVal hgap As Integer, ByVal vgap As Integer)
			Me.hgap = hgap
			Me.vgap = vgap
		End Sub

		''' <summary>
		''' Gets the horizontal gap between components. </summary>
		''' <returns>    the horizontal gap between components. </returns>
		''' <seealso cref=       java.awt.CardLayout#setHgap(int) </seealso>
		''' <seealso cref=       java.awt.CardLayout#getVgap()
		''' @since     JDK1.1 </seealso>
		Public Overridable Property hgap As Integer
			Get
				Return hgap
			End Get
			Set(ByVal hgap As Integer)
				Me.hgap = hgap
			End Set
		End Property


		''' <summary>
		''' Gets the vertical gap between components. </summary>
		''' <returns> the vertical gap between components. </returns>
		''' <seealso cref=       java.awt.CardLayout#setVgap(int) </seealso>
		''' <seealso cref=       java.awt.CardLayout#getHgap() </seealso>
		Public Overridable Property vgap As Integer
			Get
				Return vgap
			End Get
			Set(ByVal vgap As Integer)
				Me.vgap = vgap
			End Set
		End Property


		''' <summary>
		''' Adds the specified component to this card layout's internal
		''' table of names. The object specified by <code>constraints</code>
		''' must be a string. The card layout stores this string as a key-value
		''' pair that can be used for random access to a particular card.
		''' By calling the <code>show</code> method, an application can
		''' display the component with the specified name. </summary>
		''' <param name="comp">          the component to be added. </param>
		''' <param name="constraints">   a tag that identifies a particular
		'''                                        card in the layout. </param>
		''' <seealso cref=       java.awt.CardLayout#show(java.awt.Container, java.lang.String) </seealso>
		''' <exception cref="IllegalArgumentException">  if the constraint is not a string. </exception>
		Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object) Implements LayoutManager2.addLayoutComponent
		  SyncLock comp.treeLock
			  If constraints Is Nothing Then constraints = ""
			If TypeOf constraints Is String Then
				addLayoutComponent(CStr(constraints), comp)
			Else
				Throw New IllegalArgumentException("cannot add to layout: constraint must be a string")
			End If
		  End SyncLock
		End Sub

		''' @deprecated   replaced by
		'''      <code>addLayoutComponent(Component, Object)</code>. 
		<Obsolete("  replaced by")> _
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component) Implements LayoutManager.addLayoutComponent
			SyncLock comp.treeLock
				If vector.Count > 0 Then comp.visible = False
				For i As Integer = 0 To vector.Count - 1
					If CType(vector(i), Card).name.Equals(name) Then
						CType(vector(i), Card).comp = comp
						Return
					End If
				Next i
				vector.Add(New Card(Me, name, comp))
			End SyncLock
		End Sub

		''' <summary>
		''' Removes the specified component from the layout.
		''' If the card was visible on top, the next card underneath it is shown. </summary>
		''' <param name="comp">   the component to be removed. </param>
		''' <seealso cref=     java.awt.Container#remove(java.awt.Component) </seealso>
		''' <seealso cref=     java.awt.Container#removeAll() </seealso>
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component) Implements LayoutManager.removeLayoutComponent
			SyncLock comp.treeLock
				For i As Integer = 0 To vector.Count - 1
					If CType(vector(i), Card).comp Is comp Then
						' if we remove current component we should show next one
						If comp.visible AndAlso (comp.parent IsNot Nothing) Then [next](comp.parent)

						vector.RemoveAt(i)

						' correct currentCard if this is necessary
						If currentCard > i Then currentCard -= 1
						Exit For
					End If
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Determines the preferred size of the container argument using
		''' this card layout. </summary>
		''' <param name="parent"> the parent container in which to do the layout </param>
		''' <returns>  the preferred dimensions to lay out the subcomponents
		'''                of the specified container </returns>
		''' <seealso cref=     java.awt.Container#getPreferredSize </seealso>
		''' <seealso cref=     java.awt.CardLayout#minimumLayoutSize </seealso>
		Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension Implements LayoutManager.preferredLayoutSize
			SyncLock parent.treeLock
				Dim insets_Renamed As Insets = parent.insets
				Dim ncomponents As Integer = parent.componentCount
				Dim w As Integer = 0
				Dim h As Integer = 0

				For i As Integer = 0 To ncomponents - 1
					Dim comp As Component = parent.getComponent(i)
					Dim d As Dimension = comp.preferredSize
					If d.width > w Then w = d.width
					If d.height > h Then h = d.height
				Next i
				Return New Dimension(insets_Renamed.left + insets_Renamed.right + w + hgap*2, insets_Renamed.top + insets_Renamed.bottom + h + vgap*2)
			End SyncLock
		End Function

		''' <summary>
		''' Calculates the minimum size for the specified panel. </summary>
		''' <param name="parent"> the parent container in which to do the layout </param>
		''' <returns>    the minimum dimensions required to lay out the
		'''                subcomponents of the specified container </returns>
		''' <seealso cref=       java.awt.Container#doLayout </seealso>
		''' <seealso cref=       java.awt.CardLayout#preferredLayoutSize </seealso>
		Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension Implements LayoutManager.minimumLayoutSize
			SyncLock parent.treeLock
				Dim insets_Renamed As Insets = parent.insets
				Dim ncomponents As Integer = parent.componentCount
				Dim w As Integer = 0
				Dim h As Integer = 0

				For i As Integer = 0 To ncomponents - 1
					Dim comp As Component = parent.getComponent(i)
					Dim d As Dimension = comp.minimumSize
					If d.width > w Then w = d.width
					If d.height > h Then h = d.height
				Next i
				Return New Dimension(insets_Renamed.left + insets_Renamed.right + w + hgap*2, insets_Renamed.top + insets_Renamed.bottom + h + vgap*2)
			End SyncLock
		End Function

		''' <summary>
		''' Returns the maximum dimensions for this layout given the components
		''' in the specified target container. </summary>
		''' <param name="target"> the component which needs to be laid out </param>
		''' <seealso cref= Container </seealso>
		''' <seealso cref= #minimumLayoutSize </seealso>
		''' <seealso cref= #preferredLayoutSize </seealso>
		Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension Implements LayoutManager2.maximumLayoutSize
			Return New Dimension(Integer.MaxValue, Integer.MaxValue)
		End Function

		''' <summary>
		''' Returns the alignment along the x axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Public Overridable Function getLayoutAlignmentX(ByVal parent As Container) As Single Implements LayoutManager2.getLayoutAlignmentX
			Return 0.5f
		End Function

		''' <summary>
		''' Returns the alignment along the y axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Public Overridable Function getLayoutAlignmentY(ByVal parent As Container) As Single Implements LayoutManager2.getLayoutAlignmentY
			Return 0.5f
		End Function

		''' <summary>
		''' Invalidates the layout, indicating that if the layout manager
		''' has cached information it should be discarded.
		''' </summary>
		Public Overridable Sub invalidateLayout(ByVal target As Container) Implements LayoutManager2.invalidateLayout
		End Sub

		''' <summary>
		''' Lays out the specified container using this card layout.
		''' <p>
		''' Each component in the <code>parent</code> container is reshaped
		''' to be the size of the container, minus space for surrounding
		''' insets, horizontal gaps, and vertical gaps.
		''' </summary>
		''' <param name="parent"> the parent container in which to do the layout </param>
		''' <seealso cref=       java.awt.Container#doLayout </seealso>
		Public Overridable Sub layoutContainer(ByVal parent As Container) Implements LayoutManager.layoutContainer
			SyncLock parent.treeLock
				Dim insets_Renamed As Insets = parent.insets
				Dim ncomponents As Integer = parent.componentCount
				Dim comp As Component = Nothing
				Dim currentFound As Boolean = False

				For i As Integer = 0 To ncomponents - 1
					comp = parent.getComponent(i)
					comp.boundsnds(hgap + insets_Renamed.left, vgap + insets_Renamed.top, parent.width - (hgap*2 + insets_Renamed.left + insets_Renamed.right), parent.height - (vgap*2 + insets_Renamed.top + insets_Renamed.bottom))
					If comp.visible Then currentFound = True
				Next i

				If (Not currentFound) AndAlso ncomponents > 0 Then parent.getComponent(0).visible = True
			End SyncLock
		End Sub

		''' <summary>
		''' Make sure that the Container really has a CardLayout installed.
		''' Otherwise havoc can ensue!
		''' </summary>
		Friend Overridable Sub checkLayout(ByVal parent As Container)
			If parent.layout IsNot Me Then Throw New IllegalArgumentException("wrong parent for CardLayout")
		End Sub

		''' <summary>
		''' Flips to the first card of the container. </summary>
		''' <param name="parent">   the parent container in which to do the layout </param>
		''' <seealso cref=       java.awt.CardLayout#last </seealso>
		Public Overridable Sub first(ByVal parent As Container)
			SyncLock parent.treeLock
				checkLayout(parent)
				Dim ncomponents As Integer = parent.componentCount
				For i As Integer = 0 To ncomponents - 1
					Dim comp As Component = parent.getComponent(i)
					If comp.visible Then
						comp.visible = False
						Exit For
					End If
				Next i
				If ncomponents > 0 Then
					currentCard = 0
					parent.getComponent(0).visible = True
					parent.validate()
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Flips to the next card of the specified container. If the
		''' currently visible card is the last one, this method flips to the
		''' first card in the layout. </summary>
		''' <param name="parent">   the parent container in which to do the layout </param>
		''' <seealso cref=       java.awt.CardLayout#previous </seealso>
		Public Overridable Sub [next](ByVal parent As Container)
			SyncLock parent.treeLock
				checkLayout(parent)
				Dim ncomponents As Integer = parent.componentCount
				For i As Integer = 0 To ncomponents - 1
					Dim comp As Component = parent.getComponent(i)
					If comp.visible Then
						comp.visible = False
						currentCard = (i + 1) Mod ncomponents
						comp = parent.getComponent(currentCard)
						comp.visible = True
						parent.validate()
						Return
					End If
				Next i
				showDefaultComponent(parent)
			End SyncLock
		End Sub

		''' <summary>
		''' Flips to the previous card of the specified container. If the
		''' currently visible card is the first one, this method flips to the
		''' last card in the layout. </summary>
		''' <param name="parent">   the parent container in which to do the layout </param>
		''' <seealso cref=       java.awt.CardLayout#next </seealso>
		Public Overridable Sub previous(ByVal parent As Container)
			SyncLock parent.treeLock
				checkLayout(parent)
				Dim ncomponents As Integer = parent.componentCount
				For i As Integer = 0 To ncomponents - 1
					Dim comp As Component = parent.getComponent(i)
					If comp.visible Then
						comp.visible = False
						currentCard = (If(i > 0, i-1, ncomponents-1))
						comp = parent.getComponent(currentCard)
						comp.visible = True
						parent.validate()
						Return
					End If
				Next i
				showDefaultComponent(parent)
			End SyncLock
		End Sub

		Friend Overridable Sub showDefaultComponent(ByVal parent As Container)
			If parent.componentCount > 0 Then
				currentCard = 0
				parent.getComponent(0).visible = True
				parent.validate()
			End If
		End Sub

		''' <summary>
		''' Flips to the last card of the container. </summary>
		''' <param name="parent">   the parent container in which to do the layout </param>
		''' <seealso cref=       java.awt.CardLayout#first </seealso>
		Public Overridable Sub last(ByVal parent As Container)
			SyncLock parent.treeLock
				checkLayout(parent)
				Dim ncomponents As Integer = parent.componentCount
				For i As Integer = 0 To ncomponents - 1
					Dim comp As Component = parent.getComponent(i)
					If comp.visible Then
						comp.visible = False
						Exit For
					End If
				Next i
				If ncomponents > 0 Then
					currentCard = ncomponents - 1
					parent.getComponent(currentCard).visible = True
					parent.validate()
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Flips to the component that was added to this layout with the
		''' specified <code>name</code>, using <code>addLayoutComponent</code>.
		''' If no such component exists, then nothing happens. </summary>
		''' <param name="parent">   the parent container in which to do the layout </param>
		''' <param name="name">     the component name </param>
		''' <seealso cref=       java.awt.CardLayout#addLayoutComponent(java.awt.Component, java.lang.Object) </seealso>
		Public Overridable Sub show(ByVal parent As Container, ByVal name As String)
			SyncLock parent.treeLock
				checkLayout(parent)
				Dim [next] As Component = Nothing
				Dim ncomponents As Integer = vector.Count
				For i As Integer = 0 To ncomponents - 1
					Dim card_Renamed As Card = CType(vector(i), Card)
					If card_Renamed.name.Equals(name) Then
						[next] = card_Renamed.comp
						currentCard = i
						Exit For
					End If
				Next i
				If ([next] IsNot Nothing) AndAlso (Not [next].visible) Then
					ncomponents = parent.componentCount
					For i As Integer = 0 To ncomponents - 1
						Dim comp As Component = parent.getComponent(i)
						If comp.visible Then
							comp.visible = False
							Exit For
						End If
					Next i
					[next].visible = True
					parent.validate()
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Returns a string representation of the state of this card layout. </summary>
		''' <returns>    a string representation of this card layout. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[hgap=" & hgap & ",vgap=" & vgap & "]"
		End Function

		''' <summary>
		''' Reads serializable fields from stream.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			hgap = f.get("hgap", 0)
			vgap = f.get("vgap", 0)

			If f.defaulted("vector") Then
				'  pre-1.4 stream
				Dim tab As Dictionary(Of String, Component) = CType(f.get("tab", Nothing), Hashtable)
                vector = New List(Of Card)
                If tab IsNot Nothing AndAlso tab.Count > 0 Then
					Dim e As System.Collections.IEnumerator(Of String) = tab.Keys.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim key As String = CStr(e.nextElement())
						Dim comp As Component = CType(tab(key), Component)
						vector.Add(New Card(Me, key, comp))
						If comp.visible Then currentCard = vector.Count - 1
					Loop
				End If
			Else
				vector = CType(f.get("vector", Nothing), ArrayList)
				currentCard = f.get("currentCard", 0)
			End If
		End Sub

		''' <summary>
		''' Writes serializable fields to stream.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim tab As New Dictionary(Of String, Component)
			Dim ncomponents As Integer = vector.Count
			For i As Integer = 0 To ncomponents - 1
				Dim card_Renamed As Card = CType(vector(i), Card)
				tab(card_Renamed.name) = card_Renamed.comp
			Next i

			Dim f As java.io.ObjectOutputStream.PutField = s.putFields()
			f.put("hgap", hgap)
			f.put("vgap", vgap)
			f.put("vector", vector)
			f.put("currentCard", currentCard)
			f.put("tab", tab)
			s.writeFields()
		End Sub
	End Class

End Namespace
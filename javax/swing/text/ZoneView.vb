Imports System
Imports System.Collections.Generic
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
Namespace javax.swing.text

	''' <summary>
	''' ZoneView is a View implementation that creates zones for which
	''' the child views are not created or stored until they are needed
	''' for display or model/view translations.  This enables a substantial
	''' reduction in memory consumption for situations where the model
	''' being represented is very large, by building view objects only for
	''' the region being actively viewed/edited.  The size of the children
	''' can be estimated in some way, or calculated asynchronously with
	''' only the result being saved.
	''' <p>
	''' ZoneView extends BoxView to provide a box that implements
	''' zones for its children.  The zones are special View implementations
	''' (the children of an instance of this class) that represent only a
	''' portion of the model that an instance of ZoneView is responsible
	''' for.  The zones don't create child views until an attempt is made
	''' to display them. A box shaped view is well suited to this because:
	'''   <ul>
	'''   <li>
	'''   Boxes are a heavily used view, and having a box that
	'''   provides this behavior gives substantial opportunity
	'''   to plug the behavior into a view hierarchy from the
	'''   view factory.
	'''   <li>
	'''   Boxes are tiled in one direction, so it is easy to
	'''   divide them into zones in a reliable way.
	'''   <li>
	'''   Boxes typically have a simple relationship to the model (i.e. they
	'''   create child views that directly represent the child elements).
	'''   <li>
	'''   Boxes are easier to estimate the size of than some other shapes.
	'''   </ul>
	''' <p>
	''' The default behavior is controlled by two properties, maxZoneSize
	''' and maxZonesLoaded.  Setting maxZoneSize to Integer.MAX_VALUE would
	''' have the effect of causing only one zone to be created.  This would
	''' effectively turn the view into an implementation of the decorator
	''' pattern.  Setting maxZonesLoaded to a value of Integer.MAX_VALUE would
	''' cause zones to never be unloaded.  For simplicity, zones are created on
	''' boundaries represented by the child elements of the element the view is
	''' responsible for.  The zones can be any View implementation, but the
	''' default implementation is based upon AsyncBoxView which supports fairly
	''' large zones efficiently.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View
	''' @since   1.3 </seealso>
	Public Class ZoneView
		Inherits BoxView

		Friend maxZoneSize As Integer = 8 * 1024
		Friend maxZonesLoaded As Integer = 3
		Friend loadedZones As List(Of View)

		''' <summary>
		''' Constructs a ZoneView.
		''' </summary>
		''' <param name="elem"> the element this view is responsible for </param>
		''' <param name="axis"> either View.X_AXIS or View.Y_AXIS </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem, axis)
			loadedZones = New List(Of View)
		End Sub

		''' <summary>
		''' Get the current maximum zone size.
		''' </summary>
		Public Overridable Property maximumZoneSize As Integer
			Get
				Return maxZoneSize
			End Get
			Set(ByVal size As Integer)
				maxZoneSize = size
			End Set
		End Property


		''' <summary>
		''' Get the current setting of the number of zones
		''' allowed to be loaded at the same time.
		''' </summary>
		Public Overridable Property maxZonesLoaded As Integer
			Get
				Return maxZonesLoaded
			End Get
			Set(ByVal mzl As Integer)
				If mzl < 1 Then Throw New System.ArgumentException("ZoneView.setMaxZonesLoaded must be greater than 0.")
				maxZonesLoaded = mzl
				unloadOldZones()
			End Set
		End Property


		''' <summary>
		''' Called by a zone when it gets loaded.  This happens when
		''' an attempt is made to display or perform a model/view
		''' translation on a zone that was in an unloaded state.
		''' This is implemented to check if the maximum number of
		''' zones was reached and to unload the oldest zone if so.
		''' </summary>
		''' <param name="zone"> the child view that was just loaded. </param>
		Protected Friend Overridable Sub zoneWasLoaded(ByVal zone As View)
			'System.out.println("loading: " + zone.getStartOffset() + "," + zone.getEndOffset());
			loadedZones.Add(zone)
			unloadOldZones()
		End Sub

		Friend Overridable Sub unloadOldZones()
			Do While loadedZones.Count > maxZonesLoaded
				Dim zone As View = loadedZones(0)
				loadedZones.RemoveAt(0)
				unloadZone(zone)
			Loop
		End Sub

		''' <summary>
		''' Unload a zone (Convert the zone to its memory saving state).
		''' The zones are expected to represent a subset of the
		''' child elements of the element this view is responsible for.
		''' Therefore, the default implementation is to simple remove
		''' all the children.
		''' </summary>
		''' <param name="zone"> the child view desired to be set to an
		'''  unloaded state. </param>
		Protected Friend Overridable Sub unloadZone(ByVal zone As View)
			'System.out.println("unloading: " + zone.getStartOffset() + "," + zone.getEndOffset());
			zone.removeAll()
		End Sub

		''' <summary>
		''' Determine if a zone is in the loaded state.
		''' The zones are expected to represent a subset of the
		''' child elements of the element this view is responsible for.
		''' Therefore, the default implementation is to return
		''' true if the view has children.
		''' </summary>
		Protected Friend Overridable Function isZoneLoaded(ByVal zone As View) As Boolean
			Return (zone.viewCount > 0)
		End Function

		''' <summary>
		''' Create a view to represent a zone for the given
		''' range within the model (which should be within
		''' the range of this objects responsibility).  This
		''' is called by the zone management logic to create
		''' new zones.  Subclasses can provide a different
		''' implementation for a zone by changing this method.
		''' </summary>
		''' <param name="p0"> the start of the desired zone.  This should
		'''  be &gt;= getStartOffset() and &lt; getEndOffset().  This
		'''  value should also be &lt; p1. </param>
		''' <param name="p1"> the end of the desired zone.  This should
		'''  be &gt; getStartOffset() and &lt;= getEndOffset().  This
		'''  value should also be &gt; p0. </param>
		Protected Friend Overridable Function createZone(ByVal p0 As Integer, ByVal p1 As Integer) As View
			Dim doc As Document = document
			Dim zone As View
			Try
				zone = New Zone(Me, element, doc.createPosition(p0), doc.createPosition(p1))
			Catch ble As BadLocationException
				' this should puke in some way.
				Throw New StateInvariantError(ble.Message)
			End Try
			Return zone
		End Function

		''' <summary>
		''' Loads all of the children to initialize the view.
		''' This is called by the <code>setParent</code> method.
		''' This is reimplemented to not load any children directly
		''' (as they are created by the zones).  This method creates
		''' the initial set of zones.  Zones don't actually get
		''' populated however until an attempt is made to display
		''' them or to do model/view coordinate translation.
		''' </summary>
		''' <param name="f"> the view factory </param>
		Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
			' build the first zone.
			Dim doc As Document = document
			Dim offs0 As Integer = startOffset
			Dim offs1 As Integer = endOffset
			append(createZone(offs0, offs1))
			handleInsert(offs0, offs1 - offs0)
		End Sub

		''' <summary>
		''' Returns the child view index representing the given position in
		''' the model.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <returns>  index of the view representing the given position, or
		'''   -1 if no view represents that position </returns>
		Protected Friend Overrides Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
			' PENDING(prinz) this could be done as a binary
			' search, and probably should be.
			Dim n As Integer = viewCount
			If pos = endOffset Then Return n - 1
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				If pos >= v.startOffset AndAlso pos < v.endOffset Then Return i
			Next i
			Return -1
		End Function

		Friend Overridable Sub handleInsert(ByVal pos As Integer, ByVal length As Integer)
			Dim index As Integer = getViewIndex(pos, Position.Bias.Forward)
			Dim v As View = getView(index)
			Dim offs0 As Integer = v.startOffset
			Dim offs1 As Integer = v.endOffset
			If (offs1 - offs0) > maxZoneSize Then splitZone(index, offs0, offs1)
		End Sub

		Friend Overridable Sub handleRemove(ByVal pos As Integer, ByVal length As Integer)
			' IMPLEMENT
		End Sub

		''' <summary>
		''' Break up the zone at the given index into pieces
		''' of an acceptable size.
		''' </summary>
		Friend Overridable Sub splitZone(ByVal index As Integer, ByVal offs0 As Integer, ByVal offs1 As Integer)
			' divide the old zone into a new set of bins
			Dim elem As Element = element
			Dim doc As Document = elem.document
			Dim zones As New List(Of View)
			Dim offs As Integer = offs0
			Do
				offs0 = offs
				offs = Math.Min(getDesiredZoneEnd(offs0), offs1)
				zones.Add(createZone(offs0, offs))
			Loop While offs < offs1
			Dim oldZone As View = getView(index)
			Dim newZones As View() = New View(zones.Count - 1){}
			zones.CopyTo(newZones)
			replace(index, 1, newZones)
		End Sub

		''' <summary>
		''' Returns the zone position to use for the
		''' end of a zone that starts at the given
		''' position.  By default this returns something
		''' close to half the max zone size.
		''' </summary>
		Friend Overridable Function getDesiredZoneEnd(ByVal pos As Integer) As Integer
			Dim elem As Element = element
			Dim index As Integer = elem.getElementIndex(pos + (maxZoneSize \ 2))
			Dim child As Element = elem.getElement(index)
			Dim offs0 As Integer = child.startOffset
			Dim offs1 As Integer = child.endOffset
			If (offs1 - pos) > maxZoneSize Then
				If offs0 > pos Then Return offs0
			End If
			Return offs1
		End Function

		' ---- View methods ----------------------------------------------------

		''' <summary>
		''' The superclass behavior will try to update the child views
		''' which is not desired in this case, since the children are
		''' zones and not directly effected by the changes to the
		''' associated element.  This is reimplemented to do nothing
		''' and return false.
		''' </summary>
		Protected Friend Overrides Function updateChildren(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal f As ViewFactory) As Boolean
			Return False
		End Function

		''' <summary>
		''' Gives notification that something was inserted into the document
		''' in a location that this view is responsible for.  This is largely
		''' delegated to the superclass, but is reimplemented to update the
		''' relevant zone (i.e. determine if a zone needs to be split into a
		''' set of 2 or more zones).
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overrides Sub insertUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			handleInsert(changes.offset, changes.length)
			MyBase.insertUpdate(changes, a, f)
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.  This is largely
		''' delegated to the superclass, but is reimplemented to update the
		''' relevant zones (i.e. determine if zones need to be removed or
		''' joined with another zone).
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overrides Sub removeUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			handleRemove(changes.offset, changes.length)
			MyBase.removeUpdate(changes, a, f)
		End Sub

		''' <summary>
		''' Internally created view that has the purpose of holding
		''' the views that represent the children of the ZoneView
		''' that have been arranged in a zone.
		''' </summary>
		Friend Class Zone
			Inherits AsyncBoxView

			Private ReadOnly outerInstance As ZoneView


			Private start As Position
			Private [end] As Position

			Public Sub New(ByVal outerInstance As ZoneView, ByVal elem As Element, ByVal start As Position, ByVal [end] As Position)
					Me.outerInstance = outerInstance
				MyBase.New(elem, outerInstance.axis)
				Me.start = start
				Me.end = [end]
			End Sub

			''' <summary>
			''' Creates the child views and populates the
			''' zone with them.  This is done by translating
			''' the positions to child element index locations
			''' and building views to those elements.  If the
			''' zone is already loaded, this does nothing.
			''' </summary>
			Public Overridable Sub load()
				If Not loaded Then
					estimatedMajorSpan = True
					Dim e As Element = element
					Dim f As ViewFactory = viewFactory
					Dim index0 As Integer = e.getElementIndex(startOffset)
					Dim index1 As Integer = e.getElementIndex(endOffset)
					Dim added As View() = New View(index1 - index0){}
					For i As Integer = index0 To index1
						added(i - index0) = f.create(e.getElement(i))
					Next i
					replace(0, 0, added)

					outerInstance.zoneWasLoaded(Me)
				End If
			End Sub

			''' <summary>
			''' Removes the child views and returns to a
			''' state of unloaded.
			''' </summary>
			Public Overridable Sub unload()
				estimatedMajorSpan = True
				removeAll()
			End Sub

			''' <summary>
			''' Determines if the zone is in the loaded state
			''' or not.
			''' </summary>
			Public Overridable Property loaded As Boolean
				Get
					Return (viewCount <> 0)
				End Get
			End Property

			''' <summary>
			''' This method is reimplemented to not build the children
			''' since the children are created when the zone is loaded
			''' rather then when it is placed in the view hierarchy.
			''' The major span is estimated at this point by building
			''' the first child (but not storing it), and calling
			''' setEstimatedMajorSpan(true) followed by setSpan for
			''' the major axis with the estimated span.
			''' </summary>
			Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
				' mark the major span as estimated
				estimatedMajorSpan = True

				' estimate the span
				Dim elem As Element = element
				Dim index0 As Integer = elem.getElementIndex(startOffset)
				Dim index1 As Integer = elem.getElementIndex(endOffset)
				Dim nChildren As Integer = index1 - index0

				' replace this with something real
				'setSpan(getMajorAxis(), nChildren * 10);

				Dim first As View = f.create(elem.getElement(index0))
				first.parent = Me
				Dim w As Single = first.getPreferredSpan(X_AXIS)
				Dim h As Single = first.getPreferredSpan(Y_AXIS)
				If majorAxis = X_AXIS Then
					w *= nChildren
				Else
					h += nChildren
				End If

				sizeize(w, h)
			End Sub

			''' <summary>
			''' Publish the changes in preferences upward to the parent
			''' view.
			''' <p>
			''' This is reimplemented to stop the superclass behavior
			''' if the zone has not yet been loaded.  If the zone is
			''' unloaded for example, the last seen major span is the
			''' best estimate and a calculated span for no children
			''' is undesirable.
			''' </summary>
			Protected Friend Overrides Sub flushRequirementChanges()
				If loaded Then MyBase.flushRequirementChanges()
			End Sub

			''' <summary>
			''' Returns the child view index representing the given position in
			''' the model.  Since the zone contains a cluster of the overall
			''' set of child elements, we can determine the index fairly
			''' quickly from the model by subtracting the index of the
			''' start offset from the index of the position given.
			''' </summary>
			''' <param name="pos"> the position >= 0 </param>
			''' <returns>  index of the view representing the given position, or
			'''   -1 if no view represents that position
			''' @since 1.3 </returns>
			Public Overrides Function getViewIndex(ByVal pos As Integer, ByVal b As Position.Bias) As Integer
				Dim isBackward As Boolean = (b Is Position.Bias.Backward)
				pos = If(isBackward, Math.Max(0, pos - 1), pos)
				Dim elem As Element = element
				Dim index1 As Integer = elem.getElementIndex(pos)
				Dim index0 As Integer = elem.getElementIndex(startOffset)
				Return index1 - index0
			End Function

			Protected Friend Overrides Function updateChildren(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal f As ViewFactory) As Boolean
				' the structure of this element changed.
				Dim removedElems As Element() = ec.childrenRemoved
				Dim addedElems As Element() = ec.childrenAdded
				Dim elem As Element = element
				Dim index0 As Integer = elem.getElementIndex(startOffset)
				Dim index1 As Integer = elem.getElementIndex(endOffset-1)
				Dim index As Integer = ec.index
				If (index >= index0) AndAlso (index <= index1) Then
					' The change is in this zone
					Dim replaceIndex As Integer = index - index0
					Dim nadd As Integer = Math.Min(index1 - index0 + 1, addedElems.Length)
					Dim nremove As Integer = Math.Min(index1 - index0 + 1, removedElems.Length)
					Dim added As View() = New View(nadd - 1){}
					For i As Integer = 0 To nadd - 1
						added(i) = f.create(addedElems(i))
					Next i
					replace(replaceIndex, nremove, added)
				End If
				Return True
			End Function

			' --- View methods ----------------------------------

			''' <summary>
			''' Fetches the attributes to use when rendering.  This view
			''' isn't directly responsible for an element so it returns
			''' the outer classes attributes.
			''' </summary>
			Public Property Overrides attributes As AttributeSet
				Get
					Return outerInstance.attributes
				End Get
			End Property

			''' <summary>
			''' Renders using the given rendering surface and area on that
			''' surface.  This is implemented to load the zone if its not
			''' already loaded, and then perform the superclass behavior.
			''' </summary>
			''' <param name="g"> the rendering surface to use </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <seealso cref= View#paint </seealso>
			Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
				load()
				MyBase.paint(g, a)
			End Sub

			''' <summary>
			''' Provides a mapping from the view coordinate space to the logical
			''' coordinate space of the model.  This is implemented to first
			''' make sure the zone is loaded before providing the superclass
			''' behavior.
			''' </summary>
			''' <param name="x">   x coordinate of the view location to convert >= 0 </param>
			''' <param name="y">   y coordinate of the view location to convert >= 0 </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the location within the model that best represents the
			'''  given point in the view >= 0 </returns>
			''' <seealso cref= View#viewToModel </seealso>
			Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
				load()
				Return MyBase.viewToModel(x, y, a, bias)
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.  This is
			''' implemented to provide the superclass behavior after first
			''' making sure the zone is loaded (The zone must be loaded to
			''' make this calculation).
			''' </summary>
			''' <param name="pos"> the position to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position </returns>
			''' <exception cref="BadLocationException">  if the given position does not represent a
			'''   valid location in the associated document </exception>
			''' <seealso cref= View#modelToView </seealso>
			Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
				load()
				Return MyBase.modelToView(pos, a, b)
			End Function

			''' <summary>
			''' Start of the zones range.
			''' </summary>
			''' <seealso cref= View#getStartOffset </seealso>
			Public Property Overrides startOffset As Integer
				Get
					Return start.offset
				End Get
			End Property

			''' <summary>
			''' End of the zones range.
			''' </summary>
			Public Property Overrides endOffset As Integer
				Get
					Return [end].offset
				End Get
			End Property

			''' <summary>
			''' Gives notification that something was inserted into
			''' the document in a location that this view is responsible for.
			''' If the zone has been loaded, the superclass behavior is
			''' invoked, otherwise this does nothing.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			''' <seealso cref= View#insertUpdate </seealso>
			Public Overrides Sub insertUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				If loaded Then MyBase.insertUpdate(e, a, f)
			End Sub

			''' <summary>
			''' Gives notification that something was removed from the document
			''' in a location that this view is responsible for.
			''' If the zone has been loaded, the superclass behavior is
			''' invoked, otherwise this does nothing.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			''' <seealso cref= View#removeUpdate </seealso>
			Public Overrides Sub removeUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				If loaded Then MyBase.removeUpdate(e, a, f)
			End Sub

			''' <summary>
			''' Gives notification from the document that attributes were changed
			''' in a location that this view is responsible for.
			''' If the zone has been loaded, the superclass behavior is
			''' invoked, otherwise this does nothing.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			''' <seealso cref= View#removeUpdate </seealso>
			Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				If loaded Then MyBase.changedUpdate(e, a, f)
			End Sub

		End Class
	End Class

End Namespace
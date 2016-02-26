Imports System.Collections.Generic
Imports System.Text
import static javax.swing.ClientPropertyKey.PopupFactory_FORCE_HEAVYWEIGHT_POPUP

'
' * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>PopupFactory</code>, as the name implies, is used to obtain
	''' instances of <code>Popup</code>s. <code>Popup</code>s are used to
	''' display a <code>Component</code> above all other <code>Component</code>s
	''' in a particular containment hierarchy. The general contract is that
	''' once you have obtained a <code>Popup</code> from a
	''' <code>PopupFactory</code>, you must invoke <code>hide</code> on the
	''' <code>Popup</code>. The typical usage is:
	''' <pre>
	'''   PopupFactory factory = PopupFactory.getSharedInstance();
	'''   Popup popup = factory.getPopup(owner, contents, x, y);
	'''   popup.show();
	'''   ...
	'''   popup.hide();
	''' </pre>
	''' </summary>
	''' <seealso cref= Popup
	''' 
	''' @since 1.4 </seealso>
	Public Class PopupFactory
		''' <summary>
		''' The shared instanceof <code>PopupFactory</code> is per
		''' <code>AppContext</code>. This is the key used in the
		''' <code>AppContext</code> to locate the <code>PopupFactory</code>.
		''' </summary>
		Private Shared ReadOnly SharedInstanceKey As Object = New StringBuilder("PopupFactory.SharedInstanceKey")

		''' <summary>
		''' Max number of items to store in any one particular cache.
		''' </summary>
		Private Const MAX_CACHE_SIZE As Integer = 5

		''' <summary>
		''' Key used to indicate a light weight popup should be used.
		''' </summary>
		Friend Const LIGHT_WEIGHT_POPUP As Integer = 0

		''' <summary>
		''' Key used to indicate a medium weight Popup should be used.
		''' </summary>
		Friend Const MEDIUM_WEIGHT_POPUP As Integer = 1

	'    
	'     * Key used to indicate a heavy weight Popup should be used.
	'     
		Friend Const HEAVY_WEIGHT_POPUP As Integer = 2

		''' <summary>
		''' Default type of Popup to create.
		''' </summary>
		Private popupType As Integer = LIGHT_WEIGHT_POPUP


		''' <summary>
		''' Sets the <code>PopupFactory</code> that will be used to obtain
		''' <code>Popup</code>s.
		''' This will throw an <code>IllegalArgumentException</code> if
		''' <code>factory</code> is null.
		''' </summary>
		''' <param name="factory"> Shared PopupFactory </param>
		''' <exception cref="IllegalArgumentException"> if <code>factory</code> is null </exception>
		''' <seealso cref= #getPopup </seealso>
		Public Shared Property sharedInstance As PopupFactory
			Set(ByVal factory As PopupFactory)
				If factory Is Nothing Then Throw New System.ArgumentException("PopupFactory can not be null")
				SwingUtilities.appContextPut(SharedInstanceKey, factory)
			End Set
			Get
				Dim factory As PopupFactory = CType(SwingUtilities.appContextGet(SharedInstanceKey), PopupFactory)
    
				If factory Is Nothing Then
					factory = New PopupFactory
					sharedInstance = factory
				End If
				Return factory
			End Get
		End Property



		''' <summary>
		''' Provides a hint as to the type of <code>Popup</code> that should
		''' be created.
		''' </summary>
		Friend Overridable Property popupType As Integer
			Set(ByVal type As Integer)
				popupType = type
			End Set
			Get
				Return popupType
			End Get
		End Property


		''' <summary>
		''' Creates a <code>Popup</code> for the Component <code>owner</code>
		''' containing the Component <code>contents</code>. <code>owner</code>
		''' is used to determine which <code>Window</code> the new
		''' <code>Popup</code> will parent the <code>Component</code> the
		''' <code>Popup</code> creates to. A null <code>owner</code> implies there
		''' is no valid parent. <code>x</code> and
		''' <code>y</code> specify the preferred initial location to place
		''' the <code>Popup</code> at. Based on screen size, or other paramaters,
		''' the <code>Popup</code> may not display at <code>x</code> and
		''' <code>y</code>.
		''' </summary>
		''' <param name="owner">    Component mouse coordinates are relative to, may be null </param>
		''' <param name="contents"> Contents of the Popup </param>
		''' <param name="x">        Initial x screen coordinate </param>
		''' <param name="y">        Initial y screen coordinate </param>
		''' <exception cref="IllegalArgumentException"> if contents is null </exception>
		''' <returns> Popup containing Contents </returns>
		Public Overridable Function getPopup(ByVal owner As Component, ByVal contents As Component, ByVal x As Integer, ByVal y As Integer) As Popup
			If contents Is Nothing Then Throw New System.ArgumentException("Popup.getPopup must be passed non-null contents")

			Dim ___popupType As Integer = getPopupType(owner, contents, x, y)
			Dim ___popup As Popup = getPopup(owner, contents, x, y, ___popupType)

			If ___popup Is Nothing Then ___popup = getPopup(owner, contents, x, y, HEAVY_WEIGHT_POPUP)
			Return ___popup
		End Function

		''' <summary>
		''' Returns the popup type to use for the specified parameters.
		''' </summary>
		Private Function getPopupType(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Integer
			Dim ___popupType As Integer = popupType

			If owner Is Nothing OrElse invokerInHeavyWeightPopup(owner) Then
				___popupType = HEAVY_WEIGHT_POPUP
			ElseIf ___popupType = LIGHT_WEIGHT_POPUP AndAlso Not(TypeOf contents Is JToolTip) AndAlso Not(TypeOf contents Is JPopupMenu) Then
				___popupType = MEDIUM_WEIGHT_POPUP
			End If

			' Check if the parent component is an option pane.  If so we need to
			' force a heavy weight popup in order to have event dispatching work
			' correctly.
			Dim c As Component = owner
			Do While c IsNot Nothing
				If TypeOf c Is JComponent Then
					If CType(c, JComponent).getClientProperty(PopupFactory_FORCE_HEAVYWEIGHT_POPUP) Is Boolean.TRUE Then
						___popupType = HEAVY_WEIGHT_POPUP
						Exit Do
					End If
				End If
				c = c.parent
			Loop

			Return ___popupType
		End Function

		''' <summary>
		''' Obtains the appropriate <code>Popup</code> based on
		''' <code>popupType</code>.
		''' </summary>
		Private Function getPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer, ByVal popupType As Integer) As Popup
			If GraphicsEnvironment.headless Then Return getHeadlessPopup(owner, contents, ownerX, ownerY)

			Select Case popupType
			Case LIGHT_WEIGHT_POPUP
				Return getLightWeightPopup(owner, contents, ownerX, ownerY)
			Case MEDIUM_WEIGHT_POPUP
				Return getMediumWeightPopup(owner, contents, ownerX, ownerY)
			Case HEAVY_WEIGHT_POPUP
				Dim ___popup As Popup = getHeavyWeightPopup(owner, contents, ownerX, ownerY)
				If (java.security.AccessController.doPrivileged(sun.awt.OSInfo.oSTypeAction) = sun.awt.OSInfo.OSType.MACOSX) AndAlso (owner IsNot Nothing) AndAlso (sun.awt.EmbeddedFrame.getAppletIfAncestorOf(owner) IsNot Nothing) Then CType(___popup, HeavyWeightPopup).cacheEnabled = False
				Return ___popup
			End Select
			Return Nothing
		End Function

		''' <summary>
		''' Creates a headless popup
		''' </summary>
		Private Function getHeadlessPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
			Return HeadlessPopup.getHeadlessPopup(owner, contents, ownerX, ownerY)
		End Function

		''' <summary>
		''' Creates a light weight popup.
		''' </summary>
		Private Function getLightWeightPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
			Return LightWeightPopup.getLightWeightPopup(owner, contents, ownerX, ownerY)
		End Function

		''' <summary>
		''' Creates a medium weight popup.
		''' </summary>
		Private Function getMediumWeightPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
			Return MediumWeightPopup.getMediumWeightPopup(owner, contents, ownerX, ownerY)
		End Function

		''' <summary>
		''' Creates a heavy weight popup.
		''' </summary>
		Private Function getHeavyWeightPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
			If GraphicsEnvironment.headless Then Return getMediumWeightPopup(owner, contents, ownerX, ownerY)
			Return HeavyWeightPopup.getHeavyWeightPopup(owner, contents, ownerX, ownerY)
		End Function

		''' <summary>
		''' Returns true if the Component <code>i</code> inside a heavy weight
		''' <code>Popup</code>.
		''' </summary>
		Private Function invokerInHeavyWeightPopup(ByVal i As Component) As Boolean
			If i IsNot Nothing Then
				Dim parent As Container
				parent = i.parent
				Do While parent IsNot Nothing
					If TypeOf parent Is Popup.HeavyWeightWindow Then Return True
					parent = parent.parent
				Loop
			End If
			Return False
		End Function


		''' <summary>
		''' Popup implementation that uses a Window as the popup.
		''' </summary>
		Private Class HeavyWeightPopup
			Inherits Popup

			Private Shared ReadOnly heavyWeightPopupCacheKey As Object = New StringBuilder("PopupFactory.heavyWeightPopupCache")

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private isCacheEnabled As Boolean = True

			''' <summary>
			''' Returns either a new or recycled <code>Popup</code> containing
			''' the specified children.
			''' </summary>
			Friend Shared Function getHeavyWeightPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
				Dim window As Window = If(owner IsNot Nothing, SwingUtilities.getWindowAncestor(owner), Nothing)
				Dim popup As HeavyWeightPopup = Nothing

				If window IsNot Nothing Then popup = getRecycledHeavyWeightPopup(window)

				Dim focusPopup As Boolean = False
				If contents IsNot Nothing AndAlso contents.focusable Then
					If TypeOf contents Is JPopupMenu Then
						Dim jpm As JPopupMenu = CType(contents, JPopupMenu)
						Dim popComps As Component() = jpm.components
						For Each popComp As Component In popComps
							If Not(TypeOf popComp Is MenuElement) AndAlso Not(TypeOf popComp Is JSeparator) Then
								focusPopup = True
								Exit For
							End If
						Next popComp
					End If
				End If

				If popup Is Nothing OrElse CType(popup.component, JWindow).focusableWindowState <> focusPopup Then

					If popup IsNot Nothing Then popup._dispose()

					popup = New HeavyWeightPopup
				End If

				popup.reset(owner, contents, ownerX, ownerY)

				If focusPopup Then
					Dim wnd As JWindow = CType(popup.component, JWindow)
					wnd.focusableWindowState = True
					' Set window name. We need this in BasicPopupMenuUI
					' to identify focusable popup window.
					wnd.name = "###focusableSwingPopup###"
				End If

				Return popup
			End Function

			''' <summary>
			''' Returns a previously disposed heavy weight <code>Popup</code>
			''' associated with <code>window</code>. This will return null if
			''' there is no <code>HeavyWeightPopup</code> associated with
			''' <code>window</code>.
			''' </summary>
			Private Shared Function getRecycledHeavyWeightPopup(ByVal w As Window) As HeavyWeightPopup
				SyncLock GetType(HeavyWeightPopup)
					Dim cache As IList(Of HeavyWeightPopup)
					Dim heavyPopupCache As IDictionary(Of Window, IList(Of HeavyWeightPopup)) = heavyWeightPopupCache

					If heavyPopupCache.ContainsKey(w) Then
						cache = heavyPopupCache(w)
					Else
						Return Nothing
					End If
					If cache.Count > 0 Then
						Dim r As HeavyWeightPopup = cache(0)
						cache.RemoveAt(0)
						Return r
					End If
					Return Nothing
				End SyncLock
			End Function

			''' <summary>
			''' Returns the cache to use for heavy weight popups. Maps from
			''' <code>Window</code> to a <code>List</code> of
			''' <code>HeavyWeightPopup</code>s.
			''' </summary>
			Private Property Shared heavyWeightPopupCache As IDictionary(Of Window, IList(Of HeavyWeightPopup))
				Get
					SyncLock GetType(HeavyWeightPopup)
						Dim cache As IDictionary(Of Window, IList(Of HeavyWeightPopup)) = CType(SwingUtilities.appContextGet(heavyWeightPopupCacheKey), IDictionary(Of Window, IList(Of HeavyWeightPopup)))
    
						If cache Is Nothing Then
							cache = New Dictionary(Of Window, IList(Of HeavyWeightPopup))(2)
							SwingUtilities.appContextPut(heavyWeightPopupCacheKey, cache)
						End If
						Return cache
					End SyncLock
				End Get
			End Property

			''' <summary>
			''' Recycles the passed in <code>HeavyWeightPopup</code>.
			''' </summary>
			Private Shared Sub recycleHeavyWeightPopup(ByVal popup As HeavyWeightPopup)
				SyncLock GetType(HeavyWeightPopup)
					Dim cache As IList(Of HeavyWeightPopup)
					Dim window As Window = SwingUtilities.getWindowAncestor(popup.component)
					Dim heavyPopupCache As IDictionary(Of Window, IList(Of HeavyWeightPopup)) = heavyWeightPopupCache

					If TypeOf window Is Popup.DefaultFrame OrElse (Not window.visible) Then
						' If the Window isn't visible, we don't cache it as we
						' likely won't ever get a windowClosed event to clean up.
						' We also don't cache DefaultFrames as this indicates
						' there wasn't a valid Window parent, and thus we don't
						' know when to clean up.
						popup._dispose()
						Return
					ElseIf heavyPopupCache.ContainsKey(window) Then
						cache = heavyPopupCache(window)
					Else
						cache = New List(Of HeavyWeightPopup)
						heavyPopupCache(window) = cache
						' Clean up if the Window is closed
						Dim w As Window = window

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						w.addWindowListener(New java.awt.event.WindowAdapter()
	'					{
	'						public void windowClosed(WindowEvent e)
	'						{
	'							List<HeavyWeightPopup> popups;
	'
	'							synchronized(HeavyWeightPopup.class)
	'							{
	'								Map<Window, List<HeavyWeightPopup>> heavyPopupCache2 = getHeavyWeightPopupCache();
	'
	'								popups = heavyPopupCache2.remove(w);
	'							}
	'							if (popups != Nothing)
	'							{
	'								for (int counter = popups.size() - 1; counter >= 0; counter -= 1)
	'								{
	'									popups.get(counter)._dispose();
	'								}
	'							}
	'						}
	'					});
					End If

					If cache.Count < MAX_CACHE_SIZE Then
						cache.Add(popup)
					Else
						popup._dispose()
					End If
				End SyncLock
			End Sub

			''' <summary>
			''' Enables or disables cache for current object.
			''' </summary>
			Friend Overridable Property cacheEnabled As Boolean
				Set(ByVal enable As Boolean)
					isCacheEnabled = enable
				End Set
			End Property

			'
			' Popup methods
			'
			Public Overrides Sub hide()
				MyBase.hide()
				If isCacheEnabled Then
					recycleHeavyWeightPopup(Me)
				Else
					Me._dispose()
				End If
			End Sub

			''' <summary>
			''' As we recycle the <code>Window</code>, we don't want to dispose it,
			''' thus this method does nothing, instead use <code>_dipose</code>
			''' which will handle the disposing.
			''' </summary>
			Friend Overrides Sub dispose()
			End Sub

			Friend Overridable Sub _dispose()
				MyBase.Dispose()
			End Sub
		End Class



		''' <summary>
		''' ContainerPopup consolidates the common code used in the light/medium
		''' weight implementations of <code>Popup</code>.
		''' </summary>
		Private Class ContainerPopup
			Inherits Popup

			''' <summary>
			''' Component we are to be added to. </summary>
			Friend owner As Component
			''' <summary>
			''' Desired x location. </summary>
			Friend x As Integer
			''' <summary>
			''' Desired y location. </summary>
			Friend y As Integer

			Public Overrides Sub hide()
				Dim ___component As Component = component

				If ___component IsNot Nothing Then
					Dim parent As Container = ___component.parent

					If parent IsNot Nothing Then
						Dim bounds As Rectangle = ___component.bounds

						parent.remove(___component)
						parent.repaint(bounds.x, bounds.y, bounds.width, bounds.height)
					End If
				End If
				owner = Nothing
			End Sub
			Public Overrides Sub pack()
				Dim ___component As Component = component

				If ___component IsNot Nothing Then ___component.size = ___component.preferredSize
			End Sub

			Friend Overrides Sub reset(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer)
				If (TypeOf owner Is JFrame) OrElse (TypeOf owner Is JDialog) OrElse (TypeOf owner Is JWindow) Then owner = CType(owner, RootPaneContainer).layeredPane
				MyBase.reset(owner, contents, ownerX, ownerY)

				x = ownerX
				y = ownerY
				Me.owner = owner
			End Sub

			Friend Overridable Function overlappedByOwnedWindow() As Boolean
				Dim ___component As Component = component
				If owner IsNot Nothing AndAlso ___component IsNot Nothing Then
					Dim w As Window = SwingUtilities.getWindowAncestor(owner)
					If w Is Nothing Then Return False
					Dim ownedWindows As Window() = w.ownedWindows
					If ownedWindows IsNot Nothing Then
						Dim bnd As Rectangle = ___component.bounds
						For Each window As Window In ownedWindows
							If window.visible AndAlso bnd.intersects(window.bounds) Then Return True
						Next window
					End If
				End If
				Return False
			End Function

			''' <summary>
			''' Returns true if popup can fit the screen and the owner's top parent.
			''' It determines can popup be lightweight or mediumweight.
			''' </summary>
			Friend Overridable Function fitsOnScreen() As Boolean
				Dim result As Boolean = False
				Dim ___component As Component = component
				If owner IsNot Nothing AndAlso ___component IsNot Nothing Then
					Dim popupWidth As Integer = ___component.width
					Dim popupHeight As Integer = ___component.height

					Dim parent As Container = CType(SwingUtilities.getRoot(owner), Container)
					If TypeOf parent Is JFrame OrElse TypeOf parent Is JDialog OrElse TypeOf parent Is JWindow Then

						Dim parentBounds As Rectangle = parent.bounds
						Dim i As Insets = parent.insets
						parentBounds.x += i.left
						parentBounds.y += i.top
						parentBounds.width -= i.left + i.right
						parentBounds.height -= i.top + i.bottom

						If JPopupMenu.canPopupOverlapTaskBar() Then
							Dim gc As GraphicsConfiguration = parent.graphicsConfiguration
							Dim popupArea As Rectangle = getContainerPopupArea(gc)
							result = parentBounds.intersection(popupArea).contains(x, y, popupWidth, popupHeight)
						Else
							result = parentBounds.contains(x, y, popupWidth, popupHeight)
						End If
					ElseIf TypeOf parent Is JApplet Then
						Dim parentBounds As Rectangle = parent.bounds
						Dim p As Point = parent.locationOnScreen
						parentBounds.x = p.x
						parentBounds.y = p.y
						result = parentBounds.contains(x, y, popupWidth, popupHeight)
					End If
				End If
				Return result
			End Function

			Friend Overridable Function getContainerPopupArea(ByVal gc As GraphicsConfiguration) As Rectangle
				Dim screenBounds As Rectangle
				Dim toolkit As Toolkit = Toolkit.defaultToolkit
				Dim insets As Insets
				If gc IsNot Nothing Then
					' If we have GraphicsConfiguration use it
					' to get screen bounds
					screenBounds = gc.bounds
					insets = toolkit.getScreenInsets(gc)
				Else
					' If we don't have GraphicsConfiguration use primary screen
					screenBounds = New Rectangle(toolkit.screenSize)
					insets = New Insets(0, 0, 0, 0)
				End If
				' Take insets into account
				screenBounds.x += insets.left
				screenBounds.y += insets.top
				screenBounds.width -= (insets.left + insets.right)
				screenBounds.height -= (insets.top + insets.bottom)
				Return screenBounds
			End Function
		End Class


		''' <summary>
		''' Popup implementation that is used in headless environment.
		''' </summary>
		Private Class HeadlessPopup
			Inherits ContainerPopup

			Friend Shared Function getHeadlessPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
				Dim popup As New HeadlessPopup
				popup.reset(owner, contents, ownerX, ownerY)
				Return popup
			End Function

			Friend Overrides Function createComponent(ByVal owner As Component) As Component
				Return New Panel(New BorderLayout)
			End Function

			Public Overrides Sub show()
			End Sub
			Public Overrides Sub hide()
			End Sub
		End Class


		''' <summary>
		''' Popup implementation that uses a JPanel as the popup.
		''' </summary>
		Private Class LightWeightPopup
			Inherits ContainerPopup

			Private Shared ReadOnly lightWeightPopupCacheKey As Object = New StringBuilder("PopupFactory.lightPopupCache")

			''' <summary>
			''' Returns a light weight <code>Popup</code> implementation. If
			''' the <code>Popup</code> needs more space that in available in
			''' <code>owner</code>, this will return null.
			''' </summary>
			Friend Shared Function getLightWeightPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
				Dim popup As LightWeightPopup = recycledLightWeightPopup

				If popup Is Nothing Then popup = New LightWeightPopup
				popup.reset(owner, contents, ownerX, ownerY)
				If (Not popup.fitsOnScreen()) OrElse popup.overlappedByOwnedWindow() Then
					popup.hide()
					Return Nothing
				End If
				Return popup
			End Function

			''' <summary>
			''' Returns the cache to use for heavy weight popups.
			''' </summary>
			Private Property Shared lightWeightPopupCache As IList(Of LightWeightPopup)
				Get
					Dim cache As IList(Of LightWeightPopup) = CType(SwingUtilities.appContextGet(lightWeightPopupCacheKey), IList(Of LightWeightPopup))
					If cache Is Nothing Then
						cache = New List(Of LightWeightPopup)
						SwingUtilities.appContextPut(lightWeightPopupCacheKey, cache)
					End If
					Return cache
				End Get
			End Property

			''' <summary>
			''' Recycles the LightWeightPopup <code>popup</code>.
			''' </summary>
			Private Shared Sub recycleLightWeightPopup(ByVal popup As LightWeightPopup)
				SyncLock GetType(LightWeightPopup)
					Dim lightPopupCache As IList(Of LightWeightPopup) = lightWeightPopupCache
					If lightPopupCache.Count < MAX_CACHE_SIZE Then lightPopupCache.Add(popup)
				End SyncLock
			End Sub

			''' <summary>
			''' Returns a previously used <code>LightWeightPopup</code>, or null
			''' if none of the popups have been recycled.
			''' </summary>
			Private Property Shared recycledLightWeightPopup As LightWeightPopup
				Get
					SyncLock GetType(LightWeightPopup)
						Dim lightPopupCache As IList(Of LightWeightPopup) = lightWeightPopupCache
						If lightPopupCache.Count > 0 Then
							Dim r As LightWeightPopup = lightPopupCache(0)
							lightPopupCache.RemoveAt(0)
							Return r
						End If
						Return Nothing
					End SyncLock
				End Get
			End Property



			'
			' Popup methods
			'
			Public Overrides Sub hide()
				MyBase.hide()

				Dim ___component As Container = CType(component, Container)

				___component.removeAll()
				recycleLightWeightPopup(Me)
			End Sub
			Public Overrides Sub show()
				Dim parent As Container = Nothing

				If owner IsNot Nothing Then parent = (If(TypeOf owner Is Container, CType(owner, Container), owner.parent))

				' Try to find a JLayeredPane and Window to add
				Dim p As Container = parent
				Do While p IsNot Nothing
					If TypeOf p Is JRootPane Then
						If TypeOf p.parent Is JInternalFrame Then
							p = p.parent
							Continue Do
						End If
						parent = CType(p, JRootPane).layeredPane
						' Continue, so that if there is a higher JRootPane, we'll
						' pick it up.
					ElseIf TypeOf p Is Window Then
						If parent Is Nothing Then parent = p
						Exit Do
					ElseIf TypeOf p Is JApplet Then
						' Painting code stops at Applets, we don't want
						' to add to a Component above an Applet otherwise
						' you'll never see it painted.
						Exit Do
					End If
					p = p.parent
				Loop

				Dim p As Point = SwingUtilities.convertScreenLocationToParent(parent, x, y)
				Dim ___component As Component = component

				___component.locationion(p.x, p.y)
				If TypeOf parent Is JLayeredPane Then
					parent.add(___component, JLayeredPane.POPUP_LAYER, 0)
				Else
					parent.add(___component)
				End If
			End Sub

			Friend Overrides Function createComponent(ByVal owner As Component) As Component
				Dim ___component As JComponent = New JPanel(New BorderLayout, True)

				___component.opaque = True
				Return ___component
			End Function

			'
			' Local methods
			'

			''' <summary>
			''' Resets the <code>Popup</code> to an initial state.
			''' </summary>
			Friend Overrides Sub reset(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer)
				MyBase.reset(owner, contents, ownerX, ownerY)

				Dim ___component As JComponent = CType(component, JComponent)

				___component.opaque = contents.opaque
				___component.locationion(ownerX, ownerY)
				___component.add(contents, BorderLayout.CENTER)
				contents.invalidate()
				pack()
			End Sub
		End Class


		''' <summary>
		''' Popup implementation that uses a Panel as the popup.
		''' </summary>
		Private Class MediumWeightPopup
			Inherits ContainerPopup

			Private Shared ReadOnly mediumWeightPopupCacheKey As Object = New StringBuilder("PopupFactory.mediumPopupCache")

			''' <summary>
			''' Child of the panel. The contents are added to this. </summary>
			Private rootPane As JRootPane


			''' <summary>
			''' Returns a medium weight <code>Popup</code> implementation. If
			''' the <code>Popup</code> needs more space that in available in
			''' <code>owner</code>, this will return null.
			''' </summary>
			Friend Shared Function getMediumWeightPopup(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer) As Popup
				Dim popup As MediumWeightPopup = recycledMediumWeightPopup

				If popup Is Nothing Then popup = New MediumWeightPopup
				popup.reset(owner, contents, ownerX, ownerY)
				If (Not popup.fitsOnScreen()) OrElse popup.overlappedByOwnedWindow() Then
					popup.hide()
					Return Nothing
				End If
				Return popup
			End Function

			''' <summary>
			''' Returns the cache to use for medium weight popups.
			''' </summary>
			Private Property Shared mediumWeightPopupCache As IList(Of MediumWeightPopup)
				Get
					Dim cache As IList(Of MediumWeightPopup) = CType(SwingUtilities.appContextGet(mediumWeightPopupCacheKey), IList(Of MediumWeightPopup))
    
					If cache Is Nothing Then
						cache = New List(Of MediumWeightPopup)
						SwingUtilities.appContextPut(mediumWeightPopupCacheKey, cache)
					End If
					Return cache
				End Get
			End Property

			''' <summary>
			''' Recycles the MediumWeightPopup <code>popup</code>.
			''' </summary>
			Private Shared Sub recycleMediumWeightPopup(ByVal popup As MediumWeightPopup)
				SyncLock GetType(MediumWeightPopup)
					Dim mediumPopupCache As IList(Of MediumWeightPopup) = mediumWeightPopupCache
					If mediumPopupCache.Count < MAX_CACHE_SIZE Then mediumPopupCache.Add(popup)
				End SyncLock
			End Sub

			''' <summary>
			''' Returns a previously used <code>MediumWeightPopup</code>, or null
			''' if none of the popups have been recycled.
			''' </summary>
			Private Property Shared recycledMediumWeightPopup As MediumWeightPopup
				Get
					SyncLock GetType(MediumWeightPopup)
						Dim mediumPopupCache As IList(Of MediumWeightPopup) = mediumWeightPopupCache
						If mediumPopupCache.Count > 0 Then
							Dim r As MediumWeightPopup = mediumPopupCache(0)
							mediumPopupCache.RemoveAt(0)
							Return r
						End If
						Return Nothing
					End SyncLock
				End Get
			End Property


			'
			' Popup
			'

			Public Overrides Sub hide()
				MyBase.hide()
				rootPane.contentPane.removeAll()
				recycleMediumWeightPopup(Me)
			End Sub
			Public Overrides Sub show()
				Dim ___component As Component = component
				Dim parent As Container = Nothing

				If owner IsNot Nothing Then parent = owner.parent
	'            
	'              Find the top level window,
	'              if it has a layered pane,
	'              add to that, otherwise
	'              add to the window. 
				Do While Not(TypeOf parent Is Window OrElse TypeOf parent Is java.applet.Applet) AndAlso (parent IsNot Nothing)
					parent = parent.parent
				Loop
				' Set the visibility to false before adding to workaround a
				' bug in Solaris in which the Popup gets added at the wrong
				' location, which will result in a mouseExit, which will then
				' result in the ToolTip being removed.
				If TypeOf parent Is RootPaneContainer Then
					parent = CType(parent, RootPaneContainer).layeredPane
					Dim p As Point = SwingUtilities.convertScreenLocationToParent(parent, x, y)
					___component.visible = False
					___component.locationion(p.x, p.y)
					parent.add(___component, JLayeredPane.POPUP_LAYER, 0)
				Else
					Dim p As Point = SwingUtilities.convertScreenLocationToParent(parent, x, y)

					___component.locationion(p.x, p.y)
					___component.visible = False
					parent.add(___component)
				End If
				___component.visible = True
			End Sub

			Friend Overrides Function createComponent(ByVal owner As Component) As Component
				Dim ___component As Panel = New MediumWeightComponent

				rootPane = New JRootPane
				' NOTE: this uses setOpaque vs LookAndFeel.installProperty as
				' there is NO reason for the RootPane not to be opaque. For
				' painting to work the contentPane must be opaque, therefor the
				' RootPane can also be opaque.
				rootPane.opaque = True
				___component.add(rootPane, BorderLayout.CENTER)
				Return ___component
			End Function

			''' <summary>
			''' Resets the <code>Popup</code> to an initial state.
			''' </summary>
			Friend Overrides Sub reset(ByVal owner As Component, ByVal contents As Component, ByVal ownerX As Integer, ByVal ownerY As Integer)
				MyBase.reset(owner, contents, ownerX, ownerY)

				Dim ___component As Component = component

				___component.locationion(ownerX, ownerY)
				rootPane.contentPane.add(contents, BorderLayout.CENTER)
				contents.invalidate()
				___component.validate()
				pack()
			End Sub


			' This implements SwingHeavyWeight so that repaints on it
			' are processed by the RepaintManager and SwingPaintEventDispatcher.
			Private Class MediumWeightComponent
				Inherits Panel
				Implements SwingHeavyWeight

				Friend Sub New()
					MyBase.New(New BorderLayout)
				End Sub
			End Class
		End Class
	End Class

End Namespace
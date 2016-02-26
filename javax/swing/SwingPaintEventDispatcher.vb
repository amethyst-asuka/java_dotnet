'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Swing's PaintEventDispatcher.  If the component specified by the PaintEvent
	''' is a top level Swing component (JFrame, JWindow, JDialog, JApplet), this
	''' will forward the request to the RepaintManager for eventual painting.
	''' 
	''' </summary>
	Friend Class SwingPaintEventDispatcher
		Inherits sun.awt.PaintEventDispatcher

		Private Shared ReadOnly SHOW_FROM_DOUBLE_BUFFER As Boolean
		Private Shared ReadOnly ERASE_BACKGROUND As Boolean

		Shared Sub New()
			SHOW_FROM_DOUBLE_BUFFER = "true".Equals(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.showFromDoubleBuffer", "true")))
			ERASE_BACKGROUND = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("swing.nativeErase"))
		End Sub

		Public Overridable Function createPaintEvent(ByVal component As java.awt.Component, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As java.awt.event.PaintEvent
			If TypeOf component Is RootPaneContainer Then
				Dim appContext As sun.awt.AppContext = sun.awt.SunToolkit.targetToAppContext(component)
				Dim rm As RepaintManager = RepaintManager.currentManager(appContext)
				If (Not SHOW_FROM_DOUBLE_BUFFER) OrElse (Not rm.show(CType(component, java.awt.Container), x, y, w, h)) Then rm.nativeAddDirtyRegion(appContext, CType(component, java.awt.Container), x, y, w, h)
				' For backward compatibility generate an empty paint
				' event.  Not doing this broke parts of Netbeans.
				Return New sun.awt.event.IgnorePaintEvent(component, java.awt.event.PaintEvent.PAINT, New java.awt.Rectangle(x, y, w, h))
			ElseIf TypeOf component Is SwingHeavyWeight Then
				Dim appContext As sun.awt.AppContext = sun.awt.SunToolkit.targetToAppContext(component)
				Dim rm As RepaintManager = RepaintManager.currentManager(appContext)
				rm.nativeAddDirtyRegion(appContext, CType(component, java.awt.Container), x, y, w, h)
				Return New sun.awt.event.IgnorePaintEvent(component, java.awt.event.PaintEvent.PAINT, New java.awt.Rectangle(x, y, w, h))
			End If
			Return MyBase.createPaintEvent(component, x, y, w, h)
		End Function

		Public Overridable Function shouldDoNativeBackgroundErase(ByVal c As java.awt.Component) As Boolean
			Return ERASE_BACKGROUND OrElse Not(TypeOf c Is RootPaneContainer)
		End Function

		Public Overridable Function queueSurfaceDataReplacing(ByVal c As java.awt.Component, ByVal r As Runnable) As Boolean
			If TypeOf c Is RootPaneContainer Then
				Dim appContext As sun.awt.AppContext = sun.awt.SunToolkit.targetToAppContext(c)
				RepaintManager.currentManager(appContext).nativeQueueSurfaceDataRunnable(appContext, c, r)
				Return True
			End If
			Return MyBase.queueSurfaceDataReplacing(c, r)
		End Function
	End Class

End Namespace
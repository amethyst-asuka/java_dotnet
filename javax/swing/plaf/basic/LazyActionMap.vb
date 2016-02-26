Imports System
Imports System.Diagnostics
Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.basic

	''' <summary>
	''' An ActionMap that populates its contents as necessary. The
	''' contents are populated by invoking the <code>loadActionMap</code>
	''' method on the passed in Object.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class LazyActionMap
		Inherits ActionMapUIResource

		''' <summary>
		''' Object to invoke <code>loadActionMap</code> on. This may be
		''' a Class object.
		''' </summary>
		<NonSerialized> _
		Private _loader As Object

		''' <summary>
		''' Installs an ActionMap that will be populated by invoking the
		''' <code>loadActionMap</code> method on the specified Class
		''' when necessary.
		''' <p>
		''' This should be used if the ActionMap can be shared.
		''' </summary>
		''' <param name="c"> JComponent to install the ActionMap on. </param>
		''' <param name="loaderClass"> Class object that gets loadActionMap invoked
		'''                    on. </param>
		''' <param name="defaultsKey"> Key to use to defaults table to check for
		'''        existing map and what resulting Map will be registered on. </param>
		Friend Shared Sub installLazyActionMap(ByVal c As JComponent, ByVal loaderClass As Type, ByVal defaultsKey As String)
			Dim map As ActionMap = CType(UIManager.get(defaultsKey), ActionMap)
			If map Is Nothing Then
				map = New LazyActionMap(loaderClass)
				UIManager.lookAndFeelDefaults(defaultsKey) = map
			End If
			SwingUtilities.replaceUIActionMap(c, map)
		End Sub

		''' <summary>
		''' Returns an ActionMap that will be populated by invoking the
		''' <code>loadActionMap</code> method on the specified Class
		''' when necessary.
		''' <p>
		''' This should be used if the ActionMap can be shared.
		''' </summary>
		''' <param name="c"> JComponent to install the ActionMap on. </param>
		''' <param name="loaderClass"> Class object that gets loadActionMap invoked
		'''                    on. </param>
		''' <param name="defaultsKey"> Key to use to defaults table to check for
		'''        existing map and what resulting Map will be registered on. </param>
		Friend Shared Function getActionMap(ByVal loaderClass As Type, ByVal defaultsKey As String) As ActionMap
			Dim map As ActionMap = CType(UIManager.get(defaultsKey), ActionMap)
			If map Is Nothing Then
				map = New LazyActionMap(loaderClass)
				UIManager.lookAndFeelDefaults(defaultsKey) = map
			End If
			Return map
		End Function


		Private Sub New(ByVal loader As Type)
			_loader = loader
		End Sub

		Public Overridable Sub put(ByVal action As Action)
			put(action.getValue(Action.NAME), action)
		End Sub

		Public Overrides Sub put(ByVal key As Object, ByVal action As Action)
			loadIfNecessary()
			MyBase.put(key, action)
		End Sub

		Public Overrides Function [get](ByVal key As Object) As Action
			loadIfNecessary()
			Return MyBase.get(key)
		End Function

		Public Overrides Sub remove(ByVal key As Object)
			loadIfNecessary()
			MyBase.remove(key)
		End Sub

		Public Overrides Sub clear()
			loadIfNecessary()
			MyBase.clear()
		End Sub

		Public Overrides Function keys() As Object()
			loadIfNecessary()
			Return MyBase.keys()
		End Function

		Public Overrides Function size() As Integer
			loadIfNecessary()
			Return MyBase.size()
		End Function

		Public Overrides Function allKeys() As Object()
			loadIfNecessary()
			Return MyBase.allKeys()
		End Function

		Public Overrides Property parent As ActionMap
			Set(ByVal map As ActionMap)
				loadIfNecessary()
				MyBase.parent = map
			End Set
		End Property

		Private Sub loadIfNecessary()
			If _loader IsNot Nothing Then
				Dim loader As Object = _loader

				_loader = Nothing
				Dim klass As Type = CType(loader, [Class])
				Try
					Dim method As Method = klass.getDeclaredMethod("loadActionMap", New Type() { GetType(LazyActionMap) })
					method.invoke(klass, New Object() { Me })
				Catch nsme As NoSuchMethodException
					Debug.Assert(False, "LazyActionMap unable to load actions " & klass)
				Catch iae As IllegalAccessException
					Debug.Assert(False, "LazyActionMap unable to load actions " & iae)
				Catch ite As InvocationTargetException
					Debug.Assert(False, "LazyActionMap unable to load actions " & ite)
				Catch iae As System.ArgumentException
					Debug.Assert(False, "LazyActionMap unable to load actions " & iae)
				End Try
			End If
		End Sub
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans



	''' <summary>
	''' The {@code ThreadGroupContext} is an application-dependent
	''' context referenced by the specific <seealso cref="ThreadGroup"/>.
	''' This is a replacement for the <seealso cref="sun.awt.AppContext"/>.
	''' 
	''' @author  Sergey Malenkov
	''' </summary>
	Friend NotInheritable Class ThreadGroupContext

		Private Shared ReadOnly contexts As WeakIdentityMap(Of ThreadGroupContext) = New WeakIdentityMapAnonymousInnerClassHelper(Of T)

		Private Class WeakIdentityMapAnonymousInnerClassHelper(Of T)
			Inherits WeakIdentityMap(Of T)

			Protected Friend Overridable Function create(ByVal key As Object) As ThreadGroupContext
				Return New ThreadGroupContext
			End Function
		End Class

		''' <summary>
		''' Returns the appropriate {@code ThreadGroupContext} for the caller,
		''' as determined by its {@code ThreadGroup}.
		''' </summary>
		''' <returns>  the application-dependent context </returns>
		Shared context As ThreadGroupContext
			Get
				Return contexts.get(Thread.CurrentThread.threadGroup)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private isDesignTime_Renamed As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private isGuiAvailable_Renamed As Boolean?

		Private beanInfoCache As IDictionary(Of [Class], BeanInfo)
		Private beanInfoFinder As com.sun.beans.finder.BeanInfoFinder
		Private propertyEditorFinder As com.sun.beans.finder.PropertyEditorFinder

		Private Sub New()
		End Sub

		Friend Property designTime As Boolean
			Get
				Return Me.isDesignTime_Renamed
			End Get
			Set(ByVal isDesignTime As Boolean)
				Me.isDesignTime_Renamed = isDesignTime
			End Set
		End Property



		Friend Property guiAvailable As Boolean
			Get
				Dim isGuiAvailable_Renamed As Boolean? = Me.isGuiAvailable_Renamed
				Return If(isGuiAvailable_Renamed IsNot Nothing, isGuiAvailable_Renamed, (Not java.awt.GraphicsEnvironment.headless))
			End Get
			Set(ByVal isGuiAvailable As Boolean)
				Me.isGuiAvailable_Renamed = Convert.ToBoolean(isGuiAvailable)
			End Set
		End Property



		Friend Function getBeanInfo(ByVal type As Class) As BeanInfo
			Return If(Me.beanInfoCache IsNot Nothing, Me.beanInfoCache(type), Nothing)
		End Function

		Friend Function putBeanInfo(ByVal type As Class, ByVal info As BeanInfo) As BeanInfo
			If Me.beanInfoCache Is Nothing Then Me.beanInfoCache = New java.util.WeakHashMap(Of )
				Me.beanInfoCache(type) = info
				Return Me.beanInfoCache(type)
		End Function

		Friend Sub removeBeanInfo(ByVal type As Class)
			If Me.beanInfoCache IsNot Nothing Then Me.beanInfoCache.Remove(type)
		End Sub

		Friend Sub clearBeanInfoCache()
			If Me.beanInfoCache IsNot Nothing Then Me.beanInfoCache.Clear()
		End Sub


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Property beanInfoFinder As com.sun.beans.finder.BeanInfoFinder
			Get
				If Me.beanInfoFinder Is Nothing Then Me.beanInfoFinder = New com.sun.beans.finder.BeanInfoFinder
				Return Me.beanInfoFinder
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Property propertyEditorFinder As com.sun.beans.finder.PropertyEditorFinder
			Get
				If Me.propertyEditorFinder Is Nothing Then Me.propertyEditorFinder = New com.sun.beans.finder.PropertyEditorFinder
				Return Me.propertyEditorFinder
			End Get
		End Property
	End Class

End Namespace
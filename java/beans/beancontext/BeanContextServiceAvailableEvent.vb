Imports System.Collections

'
' * Copyright (c) 1998, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans.beancontext




	''' <summary>
	''' <p>
	''' This event type is used by the BeanContextServicesListener in order to
	''' identify the service being registered.
	''' </p>
	''' </summary>

	Public Class BeanContextServiceAvailableEvent
		Inherits java.beans.beancontext.BeanContextEvent

		Private Const serialVersionUID As Long = -5333985775656400778L

		''' <summary>
		''' Construct a <code>BeanContextAvailableServiceEvent</code>. </summary>
		''' <param name="bcs"> The context in which the service has become available </param>
		''' <param name="sc"> A <code>Class</code> reference to the newly available service </param>
		Public Sub New(ByVal bcs As java.beans.beancontext.BeanContextServices, ByVal sc As Class)
			MyBase.New(CType(bcs, BeanContext))

			serviceClass = sc
		End Sub

		''' <summary>
		''' Gets the source as a reference of type <code>BeanContextServices</code>. </summary>
		''' <returns> The context in which the service has become available </returns>
		Public Overridable Property sourceAsBeanContextServices As java.beans.beancontext.BeanContextServices
			Get
				Return CType(beanContext, java.beans.beancontext.BeanContextServices)
			End Get
		End Property

		''' <summary>
		''' Gets the service class that is the subject of this notification. </summary>
		''' <returns> A <code>Class</code> reference to the newly available service </returns>
		Public Overridable Property serviceClass As Class
			Get
				Return serviceClass
			End Get
		End Property

		''' <summary>
		''' Gets the list of service dependent selectors. </summary>
		''' <returns> the current selectors available from the service </returns>
		Public Overridable Property currentServiceSelectors As IEnumerator
			Get
				Return CType(source, java.beans.beancontext.BeanContextServices).getCurrentServiceSelectors(serviceClass)
			End Get
		End Property

	'    
	'     * fields
	'     

		''' <summary>
		''' A <code>Class</code> reference to the newly available service
		''' </summary>
		Protected Friend serviceClass As Class
	End Class

End Namespace
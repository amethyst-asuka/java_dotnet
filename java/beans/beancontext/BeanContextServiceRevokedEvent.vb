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
	''' This event type is used by the
	''' <code>BeanContextServiceRevokedListener</code> in order to
	''' identify the service being revoked.
	''' </p>
	''' </summary>
	Public Class BeanContextServiceRevokedEvent
		Inherits java.beans.beancontext.BeanContextEvent

		Private Const serialVersionUID As Long = -1295543154724961754L

		''' <summary>
		''' Construct a <code>BeanContextServiceEvent</code>. </summary>
		''' <param name="bcs"> the <code>BeanContextServices</code>
		''' from which this service is being revoked </param>
		''' <param name="sc"> the service that is being revoked </param>
		''' <param name="invalidate"> <code>true</code> for immediate revocation </param>
		Public Sub New(  bcs As java.beans.beancontext.BeanContextServices,   sc As [Class],   invalidate As Boolean)
			MyBase.New(CType(bcs, BeanContext))

			serviceClass = sc
			invalidateRefs = invalidate
		End Sub

		''' <summary>
		''' Gets the source as a reference of type <code>BeanContextServices</code> </summary>
		''' <returns> the <code>BeanContextServices</code> from which
		''' this service is being revoked </returns>
		Public Overridable Property sourceAsBeanContextServices As java.beans.beancontext.BeanContextServices
			Get
				Return CType(beanContext, java.beans.beancontext.BeanContextServices)
			End Get
		End Property

		''' <summary>
		''' Gets the service class that is the subject of this notification </summary>
		''' <returns> A <code>Class</code> reference to the
		''' service that is being revoked </returns>
		Public Overridable Property serviceClass As  [Class]
			Get
				Return serviceClass
			End Get
		End Property

		''' <summary>
		''' Checks this event to determine whether or not
		''' the service being revoked is of a particular class. </summary>
		''' <param name="service"> the service of interest (should be non-null) </param>
		''' <returns> <code>true</code> if the service being revoked is of the
		''' same class as the specified service </returns>
		Public Overridable Function isServiceClass(  service As [Class]) As Boolean
			Return serviceClass.Equals(service)
		End Function

		''' <summary>
		''' Reports if the current service is being forcibly revoked,
		''' in which case the references are now invalidated and unusable. </summary>
		''' <returns> <code>true</code> if current service is being forcibly revoked </returns>
		Public Overridable Property currentServiceInvalidNow As Boolean
			Get
				Return invalidateRefs
			End Get
		End Property

		''' <summary>
		''' fields
		''' </summary>

		''' <summary>
		''' A <code>Class</code> reference to the service that is being revoked.
		''' </summary>
		Protected Friend serviceClass As  [Class]
		Private invalidateRefs As Boolean
	End Class

End Namespace
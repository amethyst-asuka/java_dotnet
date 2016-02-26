'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' Defines the management interface  of an object of class MBeanServerDelegate.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface MBeanServerDelegateMBean

		''' <summary>
		''' Returns the MBean server agent identity.
		''' </summary>
		''' <returns> the agent identity. </returns>
		ReadOnly Property mBeanServerId As String

		''' <summary>
		''' Returns the full name of the JMX specification implemented
		''' by this product.
		''' </summary>
		''' <returns> the specification name. </returns>
		ReadOnly Property specificationName As String

		''' <summary>
		''' Returns the version of the JMX specification implemented
		''' by this product.
		''' </summary>
		''' <returns> the specification version. </returns>
		ReadOnly Property specificationVersion As String

		''' <summary>
		''' Returns the vendor of the JMX specification implemented
		''' by this product.
		''' </summary>
		''' <returns> the specification vendor. </returns>
		ReadOnly Property specificationVendor As String

		''' <summary>
		''' Returns the JMX implementation name (the name of this product).
		''' </summary>
		''' <returns> the implementation name. </returns>
		ReadOnly Property implementationName As String

		''' <summary>
		''' Returns the JMX implementation version (the version of this product).
		''' </summary>
		''' <returns> the implementation version. </returns>
		ReadOnly Property implementationVersion As String

		''' <summary>
		''' Returns the JMX implementation vendor (the vendor of this product).
		''' </summary>
		''' <returns> the implementation vendor. </returns>
		ReadOnly Property implementationVendor As String

	End Interface

End Namespace
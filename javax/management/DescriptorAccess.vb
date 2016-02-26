'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
'
' * @author    IBM Corp.
' *
' * Copyright IBM Corp. 1999-2000.  All rights reserved.
' 

Namespace javax.management

	''' <summary>
	''' This interface is used to gain access to descriptors of the Descriptor class
	''' which are associated with a JMX component, i.e. MBean, MBeanInfo,
	''' MBeanAttributeInfo, MBeanNotificationInfo,
	''' MBeanOperationInfo, MBeanParameterInfo.
	''' <P>
	''' ModelMBeans make extensive use of this interface in ModelMBeanInfo classes.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface DescriptorAccess
		Inherits DescriptorRead

		''' <summary>
		''' Sets Descriptor (full replace).
		''' </summary>
		''' <param name="inDescriptor"> replaces the Descriptor associated with the
		''' component implementing this interface. If the inDescriptor is invalid for the
		''' type of Info object it is being set for, an exception is thrown.  If the
		''' inDescriptor is null, then the Descriptor will revert to its default value
		''' which should contain, at a minimum, the descriptor name and descriptorType.
		''' </param>
		''' <seealso cref= #getDescriptor </seealso>
		WriteOnly Property descriptor As Descriptor
	End Interface

End Namespace
'
' * Copyright (c) 2004, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Interface to read the Descriptor of a management interface element
	''' such as an MBeanInfo.
	''' @since 1.6
	''' </summary>
	Public Interface DescriptorRead
	   ''' <summary>
	   ''' Returns a copy of Descriptor.
	   ''' </summary>
	   ''' <returns> Descriptor associated with the component implementing this interface.
	   ''' The return value is never null, but the returned descriptor may be empty. </returns>
		ReadOnly Property descriptor As Descriptor
	End Interface

End Namespace
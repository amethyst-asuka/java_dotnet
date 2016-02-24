Imports System

'
' * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' This is a support class to make it easier for people to provide
	''' BeanInfo classes.
	''' <p>
	''' It defaults to providing "noop" information, and can be selectively
	''' overriden to provide more explicit information on chosen topics.
	''' When the introspector sees the "noop" values, it will apply low
	''' level introspection and design patterns to automatically analyze
	''' the target bean.
	''' </summary>

	Public Class SimpleBeanInfo
		Implements BeanInfo

		''' <summary>
		''' Deny knowledge about the class and customizer of the bean.
		''' You can override this if you wish to provide explicit info.
		''' </summary>
		Public Overridable Property beanDescriptor As BeanDescriptor Implements BeanInfo.getBeanDescriptor
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Deny knowledge of properties. You can override this
		''' if you wish to provide explicit property info.
		''' </summary>
		Public Overridable Property propertyDescriptors As PropertyDescriptor() Implements BeanInfo.getPropertyDescriptors
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Deny knowledge of a default property. You can override this
		''' if you wish to define a default property for the bean.
		''' </summary>
		Public Overridable Property defaultPropertyIndex As Integer Implements BeanInfo.getDefaultPropertyIndex
			Get
				Return -1
			End Get
		End Property

		''' <summary>
		''' Deny knowledge of event sets. You can override this
		''' if you wish to provide explicit event set info.
		''' </summary>
		Public Overridable Property eventSetDescriptors As EventSetDescriptor() Implements BeanInfo.getEventSetDescriptors
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Deny knowledge of a default event. You can override this
		''' if you wish to define a default event for the bean.
		''' </summary>
		Public Overridable Property defaultEventIndex As Integer Implements BeanInfo.getDefaultEventIndex
			Get
				Return -1
			End Get
		End Property

		''' <summary>
		''' Deny knowledge of methods. You can override this
		''' if you wish to provide explicit method info.
		''' </summary>
		Public Overridable Property methodDescriptors As MethodDescriptor() Implements BeanInfo.getMethodDescriptors
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Claim there are no other relevant BeanInfo objects.  You
		''' may override this if you want to (for example) return a
		''' BeanInfo for a base class.
		''' </summary>
		Public Overridable Property additionalBeanInfo As BeanInfo() Implements BeanInfo.getAdditionalBeanInfo
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Claim there are no icons available.  You can override
		''' this if you want to provide icons for your bean.
		''' </summary>
		Public Overridable Function getIcon(ByVal iconKind As Integer) As java.awt.Image Implements BeanInfo.getIcon
			Return Nothing
		End Function

		''' <summary>
		''' This is a utility method to help in loading icon images.
		''' It takes the name of a resource file associated with the
		''' current object's class file and loads an image object
		''' from that file.  Typically images will be GIFs.
		''' <p> </summary>
		''' <param name="resourceName">  A pathname relative to the directory
		'''          holding the class file of the current class.  For example,
		'''          "wombat.gif". </param>
		''' <returns>  an image object.  May be null if the load failed. </returns>
		Public Overridable Function loadImage(ByVal resourceName As String) As java.awt.Image
			Try
				Dim url As java.net.URL = Me.GetType().getResource(resourceName)
				If url IsNot Nothing Then
					Dim ip As java.awt.image.ImageProducer = CType(url.content, java.awt.image.ImageProducer)
					If ip IsNot Nothing Then Return java.awt.Toolkit.defaultToolkit.createImage(ip)
				End If
			Catch ignored As Exception
			End Try
			Return Nothing
		End Function
	End Class

End Namespace
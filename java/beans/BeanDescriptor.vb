'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A BeanDescriptor provides global information about a "bean",
	''' including its Java [Class], its displayName, etc.
	''' <p>
	''' This is one of the kinds of descriptor returned by a BeanInfo object,
	''' which also returns descriptors for properties, method, and events.
	''' </summary>

	Public Class BeanDescriptor
		Inherits FeatureDescriptor

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private beanClassRef As Reference(Of ? As [Class])
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private customizerClassRef As Reference(Of ? As [Class])

		''' <summary>
		''' Create a BeanDescriptor for a bean that doesn't have a customizer.
		''' </summary>
		''' <param name="beanClass">  The Class object of the Java class that implements
		'''          the bean.  For example sun.beans.OurButton.class. </param>
		Public Sub New(ByVal beanClass As [Class])
			Me.New(beanClass, Nothing)
		End Sub

		''' <summary>
		''' Create a BeanDescriptor for a bean that has a customizer.
		''' </summary>
		''' <param name="beanClass">  The Class object of the Java class that implements
		'''          the bean.  For example sun.beans.OurButton.class. </param>
		''' <param name="customizerClass">  The Class object of the Java class that implements
		'''          the bean's Customizer.  For example sun.beans.OurButtonCustomizer.class. </param>
		Public Sub New(ByVal beanClass As [Class], ByVal customizerClass As [Class])
			Me.beanClassRef = getWeakReference(beanClass)
			Me.customizerClassRef = getWeakReference(customizerClass)

			Dim name_Renamed As String = beanClass.name
			Do While name_Renamed.IndexOf("."c) >= 0
				name_Renamed = name_Renamed.Substring(name_Renamed.IndexOf("."c)+1)
			Loop
			name = name_Renamed
		End Sub

		''' <summary>
		''' Gets the bean's Class object.
		''' </summary>
		''' <returns> The Class object for the bean. </returns>
		Public Overridable Property beanClass As  [Class]
			Get
				Return If(Me.beanClassRef IsNot Nothing, Me.beanClassRef.get(), Nothing)
			End Get
		End Property

		''' <summary>
		''' Gets the Class object for the bean's customizer.
		''' </summary>
		''' <returns> The Class object for the bean's customizer.  This may
		''' be null if the bean doesn't have a customizer. </returns>
		Public Overridable Property customizerClass As  [Class]
			Get
				Return If(Me.customizerClassRef IsNot Nothing, Me.customizerClassRef.get(), Nothing)
			End Get
		End Property

	'    
	'     * Package-private dup constructor
	'     * This must isolate the new object from any changes to the old object.
	'     
		Friend Sub New(ByVal old As BeanDescriptor)
			MyBase.New(old)
			beanClassRef = old.beanClassRef
			customizerClassRef = old.customizerClassRef
		End Sub

		Friend Overrides Sub appendTo(ByVal sb As StringBuilder)
			appendTo(sb, "beanClass", Me.beanClassRef)
			appendTo(sb, "customizerClass", Me.customizerClassRef)
		End Sub
	End Class

End Namespace
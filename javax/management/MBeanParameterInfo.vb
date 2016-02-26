Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Describes an argument of an operation exposed by an MBean.
	''' Instances of this class are immutable.  Subclasses may be mutable
	''' but this is not recommended.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanParameterInfo
		Inherits MBeanFeatureInfo
		Implements ICloneable

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = 7432616882776782338L

		' All zero-length arrays are interchangeable. 
		Friend Shared ReadOnly NO_PARAMS As MBeanParameterInfo() = New MBeanParameterInfo(){}

		''' <summary>
		''' @serial The type or class name of the data.
		''' </summary>
		Private ReadOnly type As String


		''' <summary>
		''' Constructs an <CODE>MBeanParameterInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the data </param>
		''' <param name="type"> The type or class name of the data </param>
		''' <param name="description"> A human readable description of the data. Optional. </param>
		Public Sub New(ByVal name As String, ByVal type As String, ByVal description As String)
			Me.New(name, type, description, CType(Nothing, Descriptor))
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanParameterInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the data </param>
		''' <param name="type"> The type or class name of the data </param>
		''' <param name="description"> A human readable description of the data. Optional. </param>
		''' <param name="descriptor"> The descriptor for the operation.  This may be null
		''' which is equivalent to an empty descriptor.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal name As String, ByVal type As String, ByVal description As String, ByVal descriptor As Descriptor)
			MyBase.New(name, description, descriptor)

			Me.type = type
		End Sub


		''' <summary>
		''' <p>Returns a shallow clone of this instance.
		''' The clone is obtained by simply calling <tt>super.clone()</tt>,
		''' thus calling the default native shallow cloning mechanism
		''' implemented by <tt>Object.clone()</tt>.
		''' No deeper cloning of any internal field is made.</p>
		''' 
		''' <p>Since this class is immutable, cloning is chiefly of
		''' interest to subclasses.</p>
		''' </summary>
		 Public Overridable Function clone() As Object
			 Try
				 Return MyBase.clone()
			 Catch e As CloneNotSupportedException
				 ' should not happen as this class is cloneable
				 Return Nothing
			 End Try
		 End Function

		''' <summary>
		''' Returns the type or class name of the data.
		''' </summary>
		''' <returns> the type string. </returns>
		Public Overridable Property type As String
			Get
				Return type
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & "description=" & description & ", " & "name=" & name & ", " & "type=" & type & ", " & "descriptor=" & descriptor & "]"
		End Function

		''' <summary>
		''' Compare this MBeanParameterInfo to another.
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanParameterInfo such
		''' that its <seealso cref="#getName()"/>, <seealso cref="#getType()"/>,
		''' <seealso cref="#getDescriptor()"/>, and {@link
		''' #getDescription()} values are equal (not necessarily identical)
		''' to those of this MBeanParameterInfo. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanParameterInfo) Then Return False
			Dim p As MBeanParameterInfo = CType(o, MBeanParameterInfo)
			Return (java.util.Objects.Equals(p.name, name) AndAlso java.util.Objects.Equals(p.type, type) AndAlso java.util.Objects.Equals(p.description, description) AndAlso java.util.Objects.Equals(p.descriptor, descriptor))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return java.util.Objects.hash(name, type)
		End Function
	End Class

End Namespace
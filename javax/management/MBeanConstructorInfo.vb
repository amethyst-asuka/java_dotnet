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
	''' Describes a constructor exposed by an MBean.  Instances of this
	''' class are immutable.  Subclasses may be mutable but this is not
	''' recommended.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanConstructorInfo
		Inherits MBeanFeatureInfo
		Implements ICloneable

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = 4433990064191844427L

		Friend Shared ReadOnly NO_CONSTRUCTORS As MBeanConstructorInfo() = New MBeanConstructorInfo(){}

		''' <seealso cref= MBeanInfo#arrayGettersSafe </seealso>
		<NonSerialized> _
		Private ReadOnly arrayGettersSafe As Boolean

		''' <summary>
		''' @serial The signature of the method, that is, the class names of the arguments.
		''' </summary>
		Private ReadOnly signature As MBeanParameterInfo()

		''' <summary>
		''' Constructs an <CODE>MBeanConstructorInfo</CODE> object.  The
		''' <seealso cref="Descriptor"/> of the constructed object will include
		''' fields contributed by any annotations on the {@code
		''' Constructor} object that contain the <seealso cref="DescriptorKey"/>
		''' meta-annotation.
		''' </summary>
		''' <param name="description"> A human readable description of the operation. </param>
		''' <param name="constructor"> The <CODE>java.lang.reflect.Constructor</CODE>
		''' object describing the MBean constructor. </param>
		Public Sub New(Of T1)(ByVal description As String, ByVal constructor As Constructor(Of T1))
			Me.New(constructor.name, description, constructorSignature(constructor), com.sun.jmx.mbeanserver.Introspector.descriptorForElement(constructor))
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanConstructorInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the constructor. </param>
		''' <param name="signature"> <CODE>MBeanParameterInfo</CODE> objects
		''' describing the parameters(arguments) of the constructor.  This
		''' may be null with the same effect as a zero-length array. </param>
		''' <param name="description"> A human readable description of the constructor. </param>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As MBeanParameterInfo())
			Me.New(name, description, signature, Nothing)
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanConstructorInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the constructor. </param>
		''' <param name="signature"> <CODE>MBeanParameterInfo</CODE> objects
		''' describing the parameters(arguments) of the constructor.  This
		''' may be null with the same effect as a zero-length array. </param>
		''' <param name="description"> A human readable description of the constructor. </param>
		''' <param name="descriptor"> The descriptor for the constructor.  This may be null
		''' which is equivalent to an empty descriptor.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As MBeanParameterInfo(), ByVal descriptor As Descriptor)
			MyBase.New(name, description, descriptor)

			If signature Is Nothing OrElse signature.Length = 0 Then
				signature = MBeanParameterInfo.NO_PARAMS
			Else
				signature = signature.clone()
			End If
			Me.signature = signature
			Me.arrayGettersSafe = MBeanInfo.arrayGettersSafe(Me.GetType(), GetType(MBeanConstructorInfo))
		End Sub


		''' <summary>
		''' <p>Returns a shallow clone of this instance.  The clone is
		''' obtained by simply calling <tt>super.clone()</tt>, thus calling
		''' the default native shallow cloning mechanism implemented by
		''' <tt>Object.clone()</tt>.  No deeper cloning of any internal
		''' field is made.</p>
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
		''' <p>Returns the list of parameters for this constructor.  Each
		''' parameter is described by an <CODE>MBeanParameterInfo</CODE>
		''' object.</p>
		''' 
		''' <p>The returned array is a shallow copy of the internal array,
		''' which means that it is a copy of the internal array of
		''' references to the <CODE>MBeanParameterInfo</CODE> objects but
		''' that each referenced <CODE>MBeanParameterInfo</CODE> object is
		''' not copied.</p>
		''' </summary>
		''' <returns>  An array of <CODE>MBeanParameterInfo</CODE> objects. </returns>
		Public Overridable Property signature As MBeanParameterInfo()
			Get
				If signature.Length = 0 Then
					Return signature
				Else
					Return signature.clone()
				End If
			End Get
		End Property

		Private Function fastGetSignature() As MBeanParameterInfo()
			If arrayGettersSafe Then
				Return signature
			Else
				Return signature
			End If
		End Function

		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & "description=" & description & ", " & "name=" & name & ", " & "signature=" & java.util.Arrays.asList(fastGetSignature()) & ", " & "descriptor=" & descriptor & "]"
		End Function

		''' <summary>
		''' Compare this MBeanConstructorInfo to another.
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanConstructorInfo such
		''' that its <seealso cref="#getName()"/>, <seealso cref="#getDescription()"/>,
		''' <seealso cref="#getSignature()"/>, and <seealso cref="#getDescriptor()"/>
		''' values are equal (not necessarily
		''' identical) to those of this MBeanConstructorInfo.  Two
		''' signature arrays are equal if their elements are pairwise
		''' equal. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanConstructorInfo) Then Return False
			Dim p As MBeanConstructorInfo = CType(o, MBeanConstructorInfo)
			Return (java.util.Objects.Equals(p.name, name) AndAlso java.util.Objects.Equals(p.description, description) AndAlso java.util.Arrays.Equals(p.fastGetSignature(), fastGetSignature()) AndAlso java.util.Objects.Equals(p.descriptor, descriptor))
		End Function

	'     Unlike attributes and operations, it's quite likely we'll have
	'       more than one constructor with the same name and even
	'       description, so we include the parameter array in the hashcode.
	'       We don't include the description, though, because it could be
	'       quite long and yet the same between constructors.  Likewise for
	'       the descriptor.  
		Public Overrides Function GetHashCode() As Integer
			Return java.util.Objects.hash(name) Xor java.util.Arrays.hashCode(fastGetSignature())
		End Function

		Private Shared Function constructorSignature(Of T1)(ByVal cn As Constructor(Of T1)) As MBeanParameterInfo()
			Dim classes As Type() = cn.parameterTypes
			Dim annots As Annotation()() = cn.parameterAnnotations
			Return MBeanOperationInfo.parameters(classes, annots)
		End Function
	End Class

End Namespace
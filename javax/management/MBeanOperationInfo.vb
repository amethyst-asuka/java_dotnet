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
	''' Describes a management operation exposed by an MBean.  Instances of
	''' this class are immutable.  Subclasses may be mutable but this is
	''' not recommended.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanOperationInfo
		Inherits MBeanFeatureInfo
		Implements ICloneable

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = -6178860474881375330L

		Friend Shared ReadOnly NO_OPERATIONS As MBeanOperationInfo() = New MBeanOperationInfo(){}

		''' <summary>
		''' Indicates that the operation is read-like:
		''' it returns information but does not change any state.
		''' </summary>
		Public Const INFO As Integer = 0

		''' <summary>
		''' Indicates that the operation is write-like: it has an effect but does
		''' not return any information from the MBean.
		''' </summary>
		Public Const ACTION As Integer = 1

		''' <summary>
		''' Indicates that the operation is both read-like and write-like:
		''' it has an effect, and it also returns information from the MBean.
		''' </summary>
		Public Const ACTION_INFO As Integer = 2

		''' <summary>
		''' Indicates that the impact of the operation is unknown or cannot be
		''' expressed using one of the other values.
		''' </summary>
		Public Const UNKNOWN As Integer = 3

		''' <summary>
		''' @serial The method's return value.
		''' </summary>
		Private ReadOnly type As String

		''' <summary>
		''' @serial The signature of the method, that is, the class names
		''' of the arguments.
		''' </summary>
		Private ReadOnly signature As MBeanParameterInfo()

		''' <summary>
		''' @serial The impact of the method, one of
		'''         <CODE>INFO</CODE>,
		'''         <CODE>ACTION</CODE>,
		'''         <CODE>ACTION_INFO</CODE>,
		'''         <CODE>UNKNOWN</CODE>
		''' </summary>
		Private ReadOnly impact As Integer

		''' <seealso cref= MBeanInfo#arrayGettersSafe </seealso>
		<NonSerialized> _
		Private ReadOnly arrayGettersSafe As Boolean


		''' <summary>
		''' Constructs an <CODE>MBeanOperationInfo</CODE> object.  The
		''' <seealso cref="Descriptor"/> of the constructed object will include
		''' fields contributed by any annotations on the {@code Method}
		''' object that contain the <seealso cref="DescriptorKey"/> meta-annotation.
		''' </summary>
		''' <param name="method"> The <CODE>java.lang.reflect.Method</CODE> object
		''' describing the MBean operation. </param>
		''' <param name="description"> A human readable description of the operation. </param>
		Public Sub New(ByVal description As String, ByVal method As Method)
			Me.New(method.name, description, methodSignature(method), method.returnType.name, UNKNOWN, com.sun.jmx.mbeanserver.Introspector.descriptorForElement(method))
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanOperationInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the method. </param>
		''' <param name="description"> A human readable description of the operation. </param>
		''' <param name="signature"> <CODE>MBeanParameterInfo</CODE> objects
		''' describing the parameters(arguments) of the method.  This may be
		''' null with the same effect as a zero-length array. </param>
		''' <param name="type"> The type of the method's return value. </param>
		''' <param name="impact"> The impact of the method, one of
		''' <seealso cref="#INFO"/>, <seealso cref="#ACTION"/>, <seealso cref="#ACTION_INFO"/>,
		''' <seealso cref="#UNKNOWN"/>. </param>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As MBeanParameterInfo(), ByVal type As String, ByVal impact As Integer)
			Me.New(name, description, signature, type, impact, CType(Nothing, Descriptor))
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanOperationInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the method. </param>
		''' <param name="description"> A human readable description of the operation. </param>
		''' <param name="signature"> <CODE>MBeanParameterInfo</CODE> objects
		''' describing the parameters(arguments) of the method.  This may be
		''' null with the same effect as a zero-length array. </param>
		''' <param name="type"> The type of the method's return value. </param>
		''' <param name="impact"> The impact of the method, one of
		''' <seealso cref="#INFO"/>, <seealso cref="#ACTION"/>, <seealso cref="#ACTION_INFO"/>,
		''' <seealso cref="#UNKNOWN"/>. </param>
		''' <param name="descriptor"> The descriptor for the operation.  This may be null
		''' which is equivalent to an empty descriptor.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As MBeanParameterInfo(), ByVal type As String, ByVal impact As Integer, ByVal descriptor As Descriptor)

			MyBase.New(name, description, descriptor)

			If signature Is Nothing OrElse signature.Length = 0 Then
				signature = MBeanParameterInfo.NO_PARAMS
			Else
				signature = signature.clone()
			End If
			Me.signature = signature
			Me.type = type
			Me.impact = impact
			Me.arrayGettersSafe = MBeanInfo.arrayGettersSafe(Me.GetType(), GetType(MBeanOperationInfo))
		End Sub

		''' <summary>
		''' <p>Returns a shallow clone of this instance.
		''' The clone is obtained by simply calling <tt>super.clone()</tt>,
		''' thus calling the default native shallow cloning mechanism
		''' implemented by <tt>Object.clone()</tt>.
		''' No deeper cloning of any internal field is made.</p>
		''' 
		''' <p>Since this class is immutable, cloning is chiefly of interest
		''' to subclasses.</p>
		''' </summary>
		 Public Overrides Function clone() As Object
			 Try
				 Return MyBase.clone()
			 Catch e As CloneNotSupportedException
				 ' should not happen as this class is cloneable
				 Return Nothing
			 End Try
		 End Function

		''' <summary>
		''' Returns the type of the method's return value.
		''' </summary>
		''' <returns> the return type. </returns>
		Public Overridable Property returnType As String
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' <p>Returns the list of parameters for this operation.  Each
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
				' If MBeanOperationInfo was created in our implementation,
				' signature cannot be null - because our constructors replace
				' null with MBeanParameterInfo.NO_PARAMS;
				'
				' However, signature could be null if an  MBeanOperationInfo is
				' deserialized from a byte array produced by another implementation.
				' This is not very likely but possible, since the serial form says
				' nothing against it. (see 6373150)
				'
				If signature Is Nothing Then
					' if signature is null simply return an empty array .
					'
					Return MBeanParameterInfo.NO_PARAMS
				ElseIf signature.Length = 0 Then
					Return signature
				Else
					Return signature.clone()
				End If
			End Get
		End Property

		Private Function fastGetSignature() As MBeanParameterInfo()
			If arrayGettersSafe Then
				' if signature is null simply return an empty array .
				' see getSignature() above.
				'
				If signature Is Nothing Then
					Return MBeanParameterInfo.NO_PARAMS
				Else
					Return signature
				End If
			Else
				Return signature
			End If
		End Function

		''' <summary>
		''' Returns the impact of the method, one of
		''' <CODE>INFO</CODE>, <CODE>ACTION</CODE>, <CODE>ACTION_INFO</CODE>, <CODE>UNKNOWN</CODE>.
		''' </summary>
		''' <returns> the impact code. </returns>
		Public Overridable Property impact As Integer
			Get
				Return impact
			End Get
		End Property

		Public Overrides Function ToString() As String
			Dim impactString As String
			Select Case impact
			Case ACTION
				impactString = "action"
			Case ACTION_INFO
				impactString = "action/info"
			Case INFO
				impactString = "info"
			Case UNKNOWN
				impactString = "unknown"
			Case Else
				impactString = "(" & impact & ")"
			End Select
			Return Me.GetType().name & "[" & "description=" & description & ", " & "name=" & name & ", " & "returnType=" & returnType & ", " & "signature=" & java.util.Arrays.asList(fastGetSignature()) & ", " & "impact=" & impactString & ", " & "descriptor=" & descriptor & "]"
		End Function

		''' <summary>
		''' Compare this MBeanOperationInfo to another.
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanOperationInfo such
		''' that its <seealso cref="#getName()"/>, <seealso cref="#getReturnType()"/>, {@link
		''' #getDescription()}, <seealso cref="#getImpact()"/>, <seealso cref="#getDescriptor()"/>
		''' and <seealso cref="#getSignature()"/> values are equal (not necessarily identical)
		''' to those of this MBeanConstructorInfo.  Two signature arrays
		''' are equal if their elements are pairwise equal. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanOperationInfo) Then Return False
			Dim p As MBeanOperationInfo = CType(o, MBeanOperationInfo)
			Return (java.util.Objects.Equals(p.name, name) AndAlso java.util.Objects.Equals(p.returnType, returnType) AndAlso java.util.Objects.Equals(p.description, description) AndAlso p.impact = impact AndAlso java.util.Arrays.Equals(p.fastGetSignature(), fastGetSignature()) AndAlso java.util.Objects.Equals(p.descriptor, descriptor))
		End Function

	'     We do not include everything in the hashcode.  We assume that
	'       if two operations are different they'll probably have different
	'       names or types.  The penalty we pay when this assumption is
	'       wrong should be less than the penalty we would pay if it were
	'       right and we needlessly hashed in the description and the
	'       parameter array.  
		Public Overrides Function GetHashCode() As Integer
			Return java.util.Objects.hash(name, returnType)
		End Function

		Private Shared Function methodSignature(ByVal method As Method) As MBeanParameterInfo()
			Dim classes As Type() = method.parameterTypes
			Dim annots As Annotation()() = method.parameterAnnotations
			Return parameters(classes, annots)
		End Function

		Friend Shared Function parameters(ByVal classes As Type(), ByVal annots As Annotation()()) As MBeanParameterInfo()
			Dim params As MBeanParameterInfo() = New MBeanParameterInfo(classes.Length - 1){}
			assert(classes.Length = annots.Length)

			For i As Integer = 0 To classes.Length - 1
				Dim d As Descriptor = com.sun.jmx.mbeanserver.Introspector.descriptorForAnnotations(annots(i))
				Dim pn As String = "p" & (i + 1)
				params(i) = New MBeanParameterInfo(pn, classes(i).name, "", d)
			Next i

			Return params
		End Function
	End Class

End Namespace
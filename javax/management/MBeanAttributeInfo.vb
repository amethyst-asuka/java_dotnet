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
	''' Describes an MBean attribute exposed for management.  Instances of
	''' this class are immutable.  Subclasses may be mutable but this is
	''' not recommended.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class MBeanAttributeInfo
		Inherits MBeanFeatureInfo
		Implements ICloneable ' serialVersionUID not constant

		' Serial version 
		Private Shared ReadOnly Shadows serialVersionUID As Long
		Shared Sub New()
	'         For complicated reasons, the serialVersionUID changed
	'           between JMX 1.0 and JMX 1.1, even though JMX 1.1 did not
	'           have compatibility code for this class.  So the
	'           serialization produced by this class with JMX 1.2 and
	'           jmx.serial.form=1.0 is not the same as that produced by
	'           this class with JMX 1.1 and jmx.serial.form=1.0.  However,
	'           the serialization without that property is the same, and
	'           that is the only form required by JMX 1.2.
	'        
			Dim uid As Long = 8644704819898565848L
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				If "1.0".Equals(form) Then uid = 7043855487133450673L
			Catch e As Exception
				' OK: exception means no compat with 1.0, too bad
			End Try
			serialVersionUID = uid
		End Sub

		Friend Shared ReadOnly NO_ATTRIBUTES As MBeanAttributeInfo() = New MBeanAttributeInfo(){}

		''' <summary>
		''' @serial The actual attribute type.
		''' </summary>
		Private ReadOnly ___attributeType As String

		''' <summary>
		''' @serial The attribute write right.
		''' </summary>
		Private ReadOnly isWrite As Boolean

		''' <summary>
		''' @serial The attribute read right.
		''' </summary>
		Private ReadOnly isRead As Boolean

		''' <summary>
		''' @serial Indicates if this method is a "is"
		''' </summary>
		Private ReadOnly [is] As Boolean


		''' <summary>
		''' Constructs an <CODE>MBeanAttributeInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the attribute. </param>
		''' <param name="type"> The type or class name of the attribute. </param>
		''' <param name="description"> A human readable description of the attribute. </param>
		''' <param name="isReadable"> True if the attribute has a getter method, false otherwise. </param>
		''' <param name="isWritable"> True if the attribute has a setter method, false otherwise. </param>
		''' <param name="isIs"> True if this attribute has an "is" getter, false otherwise.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code isIs} is true but
		''' {@code isReadable} is not, or if {@code isIs} is true and
		''' {@code type} is not {@code boolean} or {@code java.lang.Boolean}.
		''' (New code should always use {@code boolean} rather than
		''' {@code java.lang.Boolean}.) </exception>
		Public Sub New(ByVal name As String, ByVal type As String, ByVal description As String, ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean)
			Me.New(name, type, description, isReadable, isWritable, isIs, CType(Nothing, Descriptor))
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanAttributeInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the attribute. </param>
		''' <param name="type"> The type or class name of the attribute. </param>
		''' <param name="description"> A human readable description of the attribute. </param>
		''' <param name="isReadable"> True if the attribute has a getter method, false otherwise. </param>
		''' <param name="isWritable"> True if the attribute has a setter method, false otherwise. </param>
		''' <param name="isIs"> True if this attribute has an "is" getter, false otherwise. </param>
		''' <param name="descriptor"> The descriptor for the attribute.  This may be null
		''' which is equivalent to an empty descriptor.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code isIs} is true but
		''' {@code isReadable} is not, or if {@code isIs} is true and
		''' {@code type} is not {@code boolean} or {@code java.lang.Boolean}.
		''' (New code should always use {@code boolean} rather than
		''' {@code java.lang.Boolean}.)
		''' 
		''' @since 1.6 </exception>
		Public Sub New(ByVal name As String, ByVal type As String, ByVal description As String, ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal descriptor As Descriptor)
			MyBase.New(name, description, descriptor)

			Me.___attributeType = type
			Me.isRead = isReadable
			Me.isWrite = isWritable
			If isIs AndAlso (Not isReadable) Then Throw New System.ArgumentException("Cannot have an ""is"" getter " & "for a non-readable attribute")
			If isIs AndAlso (Not type.Equals("java.lang.Boolean")) AndAlso (Not type.Equals("boolean")) Then Throw New System.ArgumentException("Cannot have an ""is"" getter " & "for a non-boolean attribute")
			Me.is = isIs
		End Sub

		''' <summary>
		''' <p>This constructor takes the name of a simple attribute, and Method
		''' objects for reading and writing the attribute.  The <seealso cref="Descriptor"/>
		''' of the constructed object will include fields contributed by any
		''' annotations on the {@code Method} objects that contain the
		''' <seealso cref="DescriptorKey"/> meta-annotation.
		''' </summary>
		''' <param name="name"> The programmatic name of the attribute. </param>
		''' <param name="description"> A human readable description of the attribute. </param>
		''' <param name="getter"> The method used for reading the attribute value.
		'''          May be null if the property is write-only. </param>
		''' <param name="setter"> The method used for writing the attribute value.
		'''          May be null if the attribute is read-only. </param>
		''' <exception cref="IntrospectionException"> There is a consistency
		''' problem in the definition of this attribute. </exception>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal getter As Method, ByVal setter As Method)
			Me.New(name, attributeType(getter, setter), description, (getter IsNot Nothing), (setter IsNot Nothing), isIs(getter), ImmutableDescriptor.union(com.sun.jmx.mbeanserver.Introspector.descriptorForElement(getter), com.sun.jmx.mbeanserver.Introspector.descriptorForElement(setter)))
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
		''' Returns the class name of the attribute.
		''' </summary>
		''' <returns> the class name. </returns>
		Public Overridable Property type As String
			Get
				Return ___attributeType
			End Get
		End Property

		''' <summary>
		''' Whether the value of the attribute can be read.
		''' </summary>
		''' <returns> True if the attribute can be read, false otherwise. </returns>
		Public Overridable Property readable As Boolean
			Get
				Return isRead
			End Get
		End Property

		''' <summary>
		''' Whether new values can be written to the attribute.
		''' </summary>
		''' <returns> True if the attribute can be written to, false otherwise. </returns>
		Public Overridable Property writable As Boolean
			Get
				Return isWrite
			End Get
		End Property

		''' <summary>
		''' Indicates if this attribute has an "is" getter.
		''' </summary>
		''' <returns> true if this attribute has an "is" getter. </returns>
		Public Overridable Property [is] As Boolean
			Get
				Return [is]
			End Get
		End Property

		Public Overrides Function ToString() As String
			Dim access As String
			If readable Then
				If writable Then
					access = "read/write"
				Else
					access = "read-only"
				End If
			ElseIf writable Then
				access = "write-only"
			Else
				access = "no-access"
			End If

			Return Me.GetType().name & "[" & "description=" & description & ", " & "name=" & name & ", " & "type=" & type & ", " & access & ", " & (If([is], "isIs, ", "")) & "descriptor=" & descriptor & "]"
		End Function

		''' <summary>
		''' Compare this MBeanAttributeInfo to another.
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanAttributeInfo such
		''' that its <seealso cref="#getName()"/>, <seealso cref="#getType()"/>, {@link
		''' #getDescription()}, <seealso cref="#isReadable()"/>, {@link
		''' #isWritable()}, and <seealso cref="#isIs()"/> values are equal (not
		''' necessarily identical) to those of this MBeanAttributeInfo. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanAttributeInfo) Then Return False
			Dim p As MBeanAttributeInfo = CType(o, MBeanAttributeInfo)
			Return (java.util.Objects.Equals(p.name, name) AndAlso java.util.Objects.Equals(p.type, type) AndAlso java.util.Objects.Equals(p.description, description) AndAlso java.util.Objects.Equals(p.descriptor, descriptor) AndAlso p.readable = readable AndAlso p.writable = writable AndAlso p.is = [is])
		End Function

	'     We do not include everything in the hashcode.  We assume that
	'       if two operations are different they'll probably have different
	'       names or types.  The penalty we pay when this assumption is
	'       wrong should be less than the penalty we would pay if it were
	'       right and we needlessly hashed in the description and parameter
	'       array.  
		Public Overrides Function GetHashCode() As Integer
			Return java.util.Objects.hash(name, type)
		End Function

		Private Shared Function isIs(ByVal getter As Method) As Boolean
			Return (getter IsNot Nothing AndAlso getter.name.StartsWith("is") AndAlso (getter.returnType.Equals(Boolean.TYPE) OrElse getter.returnType.Equals(GetType(Boolean))))
		End Function

		''' <summary>
		''' Finds the type of the attribute.
		''' </summary>
		Private Shared Function attributeType(ByVal getter As Method, ByVal setter As Method) As String
			Dim ___type As Type = Nothing

			If getter IsNot Nothing Then
				If getter.parameterTypes.length <> 0 Then Throw New IntrospectionException("bad getter arg count")
				___type = getter.returnType
				If ___type Is Void.TYPE Then Throw New IntrospectionException("getter " & getter.name & " returns void")
			End If

			If setter IsNot Nothing Then
				Dim params As Type() = setter.parameterTypes
				If params.Length <> 1 Then Throw New IntrospectionException("bad setter arg count")
				If ___type Is Nothing Then
					___type = params(0)
				ElseIf ___type IsNot params(0) Then
					Throw New IntrospectionException("type mismatch between " & "getter and setter")
				End If
			End If

			If ___type Is Nothing Then Throw New IntrospectionException("getter and setter cannot " & "both be null")

			Return ___type.name
		End Function

	End Class

End Namespace
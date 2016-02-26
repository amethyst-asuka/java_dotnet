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
	''' <p>Provides general information for an MBean descriptor object.
	''' The feature described can be an attribute, an operation, a
	''' parameter, or a notification.  Instances of this class are
	''' immutable.  Subclasses may be mutable but this is not
	''' recommended.</p>
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class MBeanFeatureInfo
		Implements DescriptorRead


		' Serial version 
		Friend Const serialVersionUID As Long = 3952882688968447265L

		''' <summary>
		''' The name of the feature.  It is recommended that subclasses call
		''' <seealso cref="#getName"/> rather than reading this field, and that they
		''' not change it.
		''' 
		''' @serial The name of the feature.
		''' </summary>
		Protected Friend name As String

		''' <summary>
		''' The human-readable description of the feature.  It is
		''' recommended that subclasses call <seealso cref="#getDescription"/> rather
		''' than reading this field, and that they not change it.
		''' 
		''' @serial The human-readable description of the feature.
		''' </summary>
		Protected Friend description As String

		''' <summary>
		''' @serial The Descriptor for this MBeanFeatureInfo.  This field
		''' can be null, which is equivalent to an empty Descriptor.
		''' </summary>
		<NonSerialized> _
		Private descriptor As Descriptor


		''' <summary>
		''' Constructs an <CODE>MBeanFeatureInfo</CODE> object.  This
		''' constructor is equivalent to {@code MBeanFeatureInfo(name,
		''' description, (Descriptor) null}.
		''' </summary>
		''' <param name="name"> The name of the feature. </param>
		''' <param name="description"> A human readable description of the feature. </param>
		Public Sub New(ByVal name As String, ByVal description As String)
			Me.New(name, description, Nothing)
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanFeatureInfo</CODE> object.
		''' </summary>
		''' <param name="name"> The name of the feature. </param>
		''' <param name="description"> A human readable description of the feature. </param>
		''' <param name="descriptor"> The descriptor for the feature.  This may be null
		''' which is equivalent to an empty descriptor.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal descriptor As Descriptor)
			Me.name = name
			Me.description = description
			Me.descriptor = descriptor
		End Sub

		''' <summary>
		''' Returns the name of the feature.
		''' </summary>
		''' <returns> the name of the feature. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the human-readable description of the feature.
		''' </summary>
		''' <returns> the human-readable description of the feature. </returns>
		Public Overridable Property description As String
			Get
				Return description
			End Get
		End Property

		''' <summary>
		''' Returns the descriptor for the feature.  Changing the returned value
		''' will have no affect on the original descriptor.
		''' </summary>
		''' <returns> a descriptor that is either immutable or a copy of the original.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property descriptor As Descriptor Implements DescriptorRead.getDescriptor
			Get
				Return CType(ImmutableDescriptor.nonNullDescriptor(descriptor).clone(), Descriptor)
			End Get
		End Property

		''' <summary>
		''' Compare this MBeanFeatureInfo to another.
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanFeatureInfo such
		''' that its <seealso cref="#getName()"/>, <seealso cref="#getDescription()"/>, and
		''' <seealso cref="#getDescriptor()"/>
		''' values are equal (not necessarily identical) to those of this
		''' MBeanFeatureInfo. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanFeatureInfo) Then Return False
			Dim p As MBeanFeatureInfo = CType(o, MBeanFeatureInfo)
			Return (java.util.Objects.Equals(p.name, name) AndAlso java.util.Objects.Equals(p.description, description) AndAlso java.util.Objects.Equals(p.descriptor, descriptor))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return name.GetHashCode() Xor description.GetHashCode() Xor descriptor.GetHashCode()
		End Function

		''' <summary>
		''' Serializes an <seealso cref="MBeanFeatureInfo"/> to an <seealso cref="ObjectOutputStream"/>.
		''' @serialData
		''' For compatibility reasons, an object of this class is serialized as follows.
		''' <p>
		''' The method <seealso cref="ObjectOutputStream#defaultWriteObject defaultWriteObject()"/>
		''' is called first to serialize the object except the field {@code descriptor}
		''' which is declared as transient. The field {@code descriptor} is serialized
		''' as follows:
		'''     <ul>
		'''     <li>If {@code descriptor} is an instance of the class
		'''        <seealso cref="ImmutableDescriptor"/>, the method {@link ObjectOutputStream#write
		'''        write(int val)} is called to write a byte with the value {@code 1},
		'''        then the method <seealso cref="ObjectOutputStream#writeObject writeObject(Object obj)"/>
		'''        is called twice to serialize the field names and the field values of the
		'''        {@code descriptor}, respectively as a {@code String[]} and an
		'''        {@code Object[]};</li>
		'''     <li>Otherwise, the method <seealso cref="ObjectOutputStream#write write(int val)"/>
		''' is called to write a byte with the value {@code 0}, then the method
		''' <seealso cref="ObjectOutputStream#writeObject writeObject(Object obj)"/> is called
		''' to serialize directly the field {@code descriptor}.
		'''     </ul>
		''' 
		''' @since 1.6
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			out.defaultWriteObject()

			If descriptor IsNot Nothing AndAlso descriptor.GetType() Is GetType(ImmutableDescriptor) Then

				out.write(1)

				Dim names As String() = descriptor.fieldNames

				out.writeObject(names)
				out.writeObject(descriptor.getFieldValues(names))
			Else
				out.write(0)

				out.writeObject(descriptor)
			End If
		End Sub

		''' <summary>
		''' Deserializes an <seealso cref="MBeanFeatureInfo"/> from an <seealso cref="ObjectInputStream"/>.
		''' @serialData
		''' For compatibility reasons, an object of this class is deserialized as follows.
		''' <p>
		''' The method <seealso cref="ObjectInputStream#defaultReadObject defaultReadObject()"/>
		''' is called first to deserialize the object except the field
		''' {@code descriptor}, which is not serialized in the default way. Then the method
		''' <seealso cref="ObjectInputStream#read read()"/> is called to read a byte, the field
		''' {@code descriptor} is deserialized according to the value of the byte value:
		'''    <ul>
		'''    <li>1. The method <seealso cref="ObjectInputStream#readObject readObject()"/>
		'''       is called twice to obtain the field names (a {@code String[]}) and
		'''       the field values (a {@code Object[]}) of the {@code descriptor}.
		'''       The two obtained values then are used to construct
		'''       an <seealso cref="ImmutableDescriptor"/> instance for the field
		'''       {@code descriptor};</li>
		'''    <li>0. The value for the field {@code descriptor} is obtained directly
		'''       by calling the method <seealso cref="ObjectInputStream#readObject readObject()"/>.
		'''       If the obtained value is null, the field {@code descriptor} is set to
		'''       <seealso cref="ImmutableDescriptor#EMPTY_DESCRIPTOR EMPTY_DESCRIPTOR"/>;</li>
		'''    <li>-1. This means that there is no byte to read and that the object is from
		'''       an earlier version of the JMX API. The field {@code descriptor} is set
		'''       to <seealso cref="ImmutableDescriptor#EMPTY_DESCRIPTOR EMPTY_DESCRIPTOR"/></li>
		'''    <li>Any other value. A <seealso cref="StreamCorruptedException"/> is thrown.</li>
		'''    </ul>
		''' 
		''' @since 1.6
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)

			[in].defaultReadObject()

			Select Case [in].read()
			Case 1
				Dim names As String() = CType([in].readObject(), String())

				Dim values As Object() = CType([in].readObject(), Object())
				descriptor = If(names.Length = 0, ImmutableDescriptor.EMPTY_DESCRIPTOR, New ImmutableDescriptor(names, values))

			Case 0
				descriptor = CType([in].readObject(), Descriptor)

				If descriptor Is Nothing Then descriptor = ImmutableDescriptor.EMPTY_DESCRIPTOR

			Case -1 ' from an earlier version of the JMX API
				descriptor = ImmutableDescriptor.EMPTY_DESCRIPTOR

			Case Else
				Throw New java.io.StreamCorruptedException("Got unexpected byte.")
			End Select
		End Sub
	End Class

End Namespace
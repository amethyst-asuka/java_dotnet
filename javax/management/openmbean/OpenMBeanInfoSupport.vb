Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.openmbean


	' java import
	'


	''' <summary>
	''' The {@code OpenMBeanInfoSupport} class describes the management
	''' information of an <i>open MBean</i>: it is a subclass of {@link
	''' javax.management.MBeanInfo}, and it implements the {@link
	''' OpenMBeanInfo} interface.  Note that an <i>open MBean</i> is
	''' recognized as such if its {@code getMBeanInfo()} method returns an
	''' instance of a class which implements the OpenMBeanInfo interface,
	''' typically {@code OpenMBeanInfoSupport}.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OpenMBeanInfoSupport
		Inherits javax.management.MBeanInfo
		Implements OpenMBeanInfo

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = 4349395935420511492L

		' As this instance is immutable, these two values
		' need only be calculated once.
		<NonSerialized> _
		Private myHashCode As Integer? = Nothing
		<NonSerialized> _
		Private myToString As String = Nothing


		''' <summary>
		''' <p>Constructs an {@code OpenMBeanInfoSupport} instance, which
		''' describes a class of open MBeans with the specified {@code
		''' className}, {@code description}, {@code openAttributes}, {@code
		''' openConstructors} , {@code openOperations} and {@code
		''' notifications}.</p>
		''' 
		''' <p>The {@code openAttributes}, {@code openConstructors},
		''' {@code openOperations} and {@code notifications}
		''' array parameters are internally copied, so that subsequent changes
		''' to the arrays referenced by these parameters have no effect on this
		''' instance.</p>
		''' </summary>
		''' <param name="className"> The fully qualified Java class name of the
		''' open MBean described by this <CODE>OpenMBeanInfoSupport</CODE>
		''' instance.
		''' </param>
		''' <param name="description"> A human readable description of the open
		''' MBean described by this <CODE>OpenMBeanInfoSupport</CODE>
		''' instance.
		''' </param>
		''' <param name="openAttributes"> The list of exposed attributes of the
		''' described open MBean; Must be an array of instances of a
		''' subclass of {@code MBeanAttributeInfo}, typically {@code
		''' OpenMBeanAttributeInfoSupport}.
		''' </param>
		''' <param name="openConstructors"> The list of exposed public constructors
		''' of the described open MBean; Must be an array of instances of a
		''' subclass of {@code MBeanConstructorInfo}, typically {@code
		''' OpenMBeanConstructorInfoSupport}.
		''' </param>
		''' <param name="openOperations"> The list of exposed operations of the
		''' described open MBean.  Must be an array of instances of a
		''' subclass of {@code MBeanOperationInfo}, typically {@code
		''' OpenMBeanOperationInfoSupport}.
		''' </param>
		''' <param name="notifications"> The list of notifications emitted by the
		''' described open MBean.
		''' </param>
		''' <exception cref="ArrayStoreException"> If {@code openAttributes}, {@code
		''' openConstructors} or {@code openOperations} is not an array of
		''' instances of a subclass of {@code MBeanAttributeInfo}, {@code
		''' MBeanConstructorInfo} or {@code MBeanOperationInfo}
		''' respectively. </exception>
		Public Sub New(ByVal className As String, ByVal description As String, ByVal openAttributes As OpenMBeanAttributeInfo(), ByVal openConstructors As OpenMBeanConstructorInfo(), ByVal openOperations As OpenMBeanOperationInfo(), ByVal notifications As javax.management.MBeanNotificationInfo())
			Me.New(className, description, openAttributes, openConstructors, openOperations, notifications, CType(Nothing, javax.management.Descriptor))
		End Sub

		''' <summary>
		''' <p>Constructs an {@code OpenMBeanInfoSupport} instance, which
		''' describes a class of open MBeans with the specified {@code
		''' className}, {@code description}, {@code openAttributes}, {@code
		''' openConstructors} , {@code openOperations}, {@code
		''' notifications}, and {@code descriptor}.</p>
		''' 
		''' <p>The {@code openAttributes}, {@code openConstructors}, {@code
		''' openOperations} and {@code notifications} array parameters are
		''' internally copied, so that subsequent changes to the arrays
		''' referenced by these parameters have no effect on this
		''' instance.</p>
		''' </summary>
		''' <param name="className"> The fully qualified Java class name of the
		''' open MBean described by this <CODE>OpenMBeanInfoSupport</CODE>
		''' instance.
		''' </param>
		''' <param name="description"> A human readable description of the open
		''' MBean described by this <CODE>OpenMBeanInfoSupport</CODE>
		''' instance.
		''' </param>
		''' <param name="openAttributes"> The list of exposed attributes of the
		''' described open MBean; Must be an array of instances of a
		''' subclass of {@code MBeanAttributeInfo}, typically {@code
		''' OpenMBeanAttributeInfoSupport}.
		''' </param>
		''' <param name="openConstructors"> The list of exposed public constructors
		''' of the described open MBean; Must be an array of instances of a
		''' subclass of {@code MBeanConstructorInfo}, typically {@code
		''' OpenMBeanConstructorInfoSupport}.
		''' </param>
		''' <param name="openOperations"> The list of exposed operations of the
		''' described open MBean.  Must be an array of instances of a
		''' subclass of {@code MBeanOperationInfo}, typically {@code
		''' OpenMBeanOperationInfoSupport}.
		''' </param>
		''' <param name="notifications"> The list of notifications emitted by the
		''' described open MBean.
		''' </param>
		''' <param name="descriptor"> The descriptor for the MBean.  This may be null
		''' which is equivalent to an empty descriptor.
		''' </param>
		''' <exception cref="ArrayStoreException"> If {@code openAttributes}, {@code
		''' openConstructors} or {@code openOperations} is not an array of
		''' instances of a subclass of {@code MBeanAttributeInfo}, {@code
		''' MBeanConstructorInfo} or {@code MBeanOperationInfo}
		''' respectively.
		''' 
		''' @since 1.6 </exception>
		Public Sub New(ByVal className As String, ByVal description As String, ByVal openAttributes As OpenMBeanAttributeInfo(), ByVal openConstructors As OpenMBeanConstructorInfo(), ByVal openOperations As OpenMBeanOperationInfo(), ByVal notifications As javax.management.MBeanNotificationInfo(), ByVal descriptor As javax.management.Descriptor)
			MyBase.New(className, description, attributeArray(openAttributes), constructorArray(openConstructors), operationArray(openOperations),If(notifications Is Nothing, Nothing, notifications.clone()), descriptor)
		End Sub


		Private Shared Function attributeArray(ByVal src As OpenMBeanAttributeInfo()) As javax.management.MBeanAttributeInfo()
			If src Is Nothing Then Return Nothing
			Dim dst As javax.management.MBeanAttributeInfo() = New javax.management.MBeanAttributeInfo(src.Length - 1){}
			Array.Copy(src, 0, dst, 0, src.Length)
			' may throw an ArrayStoreException
			Return dst
		End Function

		Private Shared Function constructorArray(ByVal src As OpenMBeanConstructorInfo()) As javax.management.MBeanConstructorInfo()
			If src Is Nothing Then Return Nothing
			Dim dst As javax.management.MBeanConstructorInfo() = New javax.management.MBeanConstructorInfo(src.Length - 1){}
			Array.Copy(src, 0, dst, 0, src.Length)
			' may throw an ArrayStoreException
			Return dst
		End Function

		Private Shared Function operationArray(ByVal src As OpenMBeanOperationInfo()) As javax.management.MBeanOperationInfo()
			If src Is Nothing Then Return Nothing
			Dim dst As javax.management.MBeanOperationInfo() = New javax.management.MBeanOperationInfo(src.Length - 1){}
			Array.Copy(src, 0, dst, 0, src.Length)
			Return dst
		End Function



		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' <p>Compares the specified {@code obj} parameter with this
		''' {@code OpenMBeanInfoSupport} instance for equality.</p>
		''' 
		''' <p>Returns {@code true} if and only if all of the following
		''' statements are true:
		''' 
		''' <ul>
		''' <li>{@code obj} is non null,</li>
		''' <li>{@code obj} also implements the {@code OpenMBeanInfo}
		''' interface,</li>
		''' <li>their class names are equal</li>
		''' <li>their infos on attributes, constructors, operations and
		''' notifications are equal</li>
		''' </ul>
		''' 
		''' This ensures that this {@code equals} method works properly for
		''' {@code obj} parameters which are different implementations of
		''' the {@code OpenMBeanInfo} interface.
		''' </summary>
		''' <param name="obj"> the object to be compared for equality with this
		''' {@code OpenMBeanInfoSupport} instance;
		''' </param>
		''' <returns> {@code true} if the specified object is equal to this
		''' {@code OpenMBeanInfoSupport} instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not a OpenMBeanInfo, return false
			'
			Dim other As OpenMBeanInfo
			Try
				other = CType(obj, OpenMBeanInfo)
			Catch e As ClassCastException
				Return False
			End Try

			' Now, really test for equality between this OpenMBeanInfo
			' implementation and the other:
			'

			' their MBean className should be equal
			If Not java.util.Objects.Equals(Me.className, other.className) Then Return False

			' their infos on attributes should be equal (order not
			' significant => equality between sets, not arrays or lists)
			If Not sameArrayContents(Me.attributes, other.attributes) Then Return False

			' their infos on constructors should be equal (order not
			' significant => equality between sets, not arrays or lists)
			If Not sameArrayContents(Me.constructors, other.constructors) Then Return False

			' their infos on operations should be equal (order not
			' significant => equality between sets, not arrays or lists)
			If Not sameArrayContents(Me.operations, other.operations) Then Return False

			' their infos on notifications should be equal (order not
			' significant => equality between sets, not arrays or lists)
			If Not sameArrayContents(Me.notifications, other.notifications) Then Return False

			' All tests for equality were successful
			'
			Return True
		End Function

		Private Shared Function sameArrayContents(Of T)(ByVal a1 As T(), ByVal a2 As T()) As Boolean
			Return (New HashSet(Of T)(java.util.Arrays.asList(a1))
				   .Equals(New HashSet(Of T)(java.util.Arrays.asList(a2))))
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this {@code
		''' OpenMBeanInfoSupport} instance.</p>
		''' 
		''' <p>The hash code of an {@code OpenMBeanInfoSupport} instance is
		''' the sum of the hash codes of all elements of information used
		''' in {@code equals} comparisons (ie: its class name, and its
		''' infos on attributes, constructors, operations and
		''' notifications, where the hashCode of each of these arrays is
		''' calculated by a call to {@code new
		''' java.util.HashSet(java.util.Arrays.asList(this.getSignature)).hashCode()}).</p>
		''' 
		''' <p>This ensures that {@code t1.equals(t2)} implies that {@code
		''' t1.hashCode()==t2.hashCode()} for any two {@code
		''' OpenMBeanInfoSupport} instances {@code t1} and {@code t2}, as
		''' required by the general contract of the method {@link
		''' Object#hashCode() Object.hashCode()}.</p>
		''' 
		''' <p>However, note that another instance of a class implementing
		''' the {@code OpenMBeanInfo} interface may be equal to this {@code
		''' OpenMBeanInfoSupport} instance as defined by {@link
		''' #equals(java.lang.Object)}, but may have a different hash code
		''' if it is calculated differently.</p>
		''' 
		''' <p>As {@code OpenMBeanInfoSupport} instances are immutable, the
		''' hash code for this instance is calculated once, on the first
		''' call to {@code hashCode}, and then the same value is returned
		''' for subsequent calls.</p>
		''' </summary>
		''' <returns> the hash code value for this {@code
		''' OpenMBeanInfoSupport} instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done
			' (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then
				Dim value As Integer = 0
				If Me.className IsNot Nothing Then value += Me.className.GetHashCode()
				value += arraySetHash(Me.attributes)
				value += arraySetHash(Me.constructors)
				value += arraySetHash(Me.operations)
				value += arraySetHash(Me.notifications)
				myHashCode = Convert.ToInt32(value)
			End If

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		Private Shared Function arraySetHash(Of T)(ByVal a As T()) As Integer
			Return (New HashSet(Of T)(java.util.Arrays.asList(a))).GetHashCode()
		End Function



		''' <summary>
		''' <p>Returns a string representation of this {@code
		''' OpenMBeanInfoSupport} instance.</p>
		''' 
		''' <p>The string representation consists of the name of this class
		''' (ie {@code javax.management.openmbean.OpenMBeanInfoSupport}),
		''' the MBean class name, the string representation of infos on
		''' attributes, constructors, operations and notifications of the
		''' described MBean and the string representation of the descriptor.</p>
		''' 
		''' <p>As {@code OpenMBeanInfoSupport} instances are immutable, the
		''' string representation for this instance is calculated once, on
		''' the first call to {@code toString}, and then the same value is
		''' returned for subsequent calls.</p>
		''' </summary>
		''' <returns> a string representation of this {@code
		''' OpenMBeanInfoSupport} instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the string value if it has not yet been done (ie
			' 1st call to toString())
			'
			If myToString Is Nothing Then myToString = (New StringBuilder).Append(Me.GetType().name).append("(mbean_class_name=").append(Me.className).append(",attributes=").append(java.util.Arrays.asList(Me.attributes).ToString()).append(",constructors=").append(java.util.Arrays.asList(Me.constructors).ToString()).append(",operations=").append(java.util.Arrays.asList(Me.operations).ToString()).append(",notifications=").append(java.util.Arrays.asList(Me.notifications).ToString()).append(",descriptor=").append(Me.descriptor).append(")").ToString()

			' return always the same string representation for this
			' instance (immutable)
			'
			Return myToString
		End Function

	End Class

End Namespace
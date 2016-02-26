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
	''' <p>The <CODE>MBeanNotificationInfo</CODE> class is used to describe the
	''' characteristics of the different notification instances
	''' emitted by an MBean, for a given Java class of notification.
	''' If an MBean emits notifications that can be instances of different Java classes,
	''' then the metadata for that MBean should provide an <CODE>MBeanNotificationInfo</CODE>
	''' object for each of these notification Java classes.</p>
	''' 
	''' <p>Instances of this class are immutable.  Subclasses may be
	''' mutable but this is not recommended.</p>
	''' 
	''' <p>This class extends <CODE>javax.management.MBeanFeatureInfo</CODE>
	''' and thus provides <CODE>name</CODE> and <CODE>description</CODE> fields.
	''' The <CODE>name</CODE> field should be the fully qualified Java class name of
	''' the notification objects described by this class.</p>
	''' 
	''' <p>The <CODE>getNotifTypes</CODE> method returns an array of
	''' strings containing the notification types that the MBean may
	''' emit. The notification type is a dot-notation string which
	''' describes what the emitted notification is about, not the Java
	''' class of the notification.  A single generic notification class can
	''' be used to send notifications of several types.  All of these types
	''' are returned in the string array result of the
	''' <CODE>getNotifTypes</CODE> method.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanNotificationInfo
		Inherits MBeanFeatureInfo
		Implements ICloneable

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = -3888371564530107064L

		Private Shared ReadOnly NO_TYPES As String() = New String(){}

		Friend Shared ReadOnly NO_NOTIFICATIONS As MBeanNotificationInfo() = New MBeanNotificationInfo(){}

		''' <summary>
		''' @serial The different types of the notification.
		''' </summary>
		Private types As String()

		''' <seealso cref= MBeanInfo#arrayGettersSafe </seealso>
		<NonSerialized> _
		Private ReadOnly arrayGettersSafe As Boolean

		''' <summary>
		''' Constructs an <CODE>MBeanNotificationInfo</CODE> object.
		''' </summary>
		''' <param name="notifTypes"> The array of strings (in dot notation)
		''' containing the notification types that the MBean may emit.
		''' This may be null with the same effect as a zero-length array. </param>
		''' <param name="name"> The fully qualified Java class name of the
		''' described notifications. </param>
		''' <param name="description"> A human readable description of the data. </param>
		Public Sub New(ByVal notifTypes As String(), ByVal name As String, ByVal description As String)
			Me.New(notifTypes, name, description, Nothing)
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanNotificationInfo</CODE> object.
		''' </summary>
		''' <param name="notifTypes"> The array of strings (in dot notation)
		''' containing the notification types that the MBean may emit.
		''' This may be null with the same effect as a zero-length array. </param>
		''' <param name="name"> The fully qualified Java class name of the
		''' described notifications. </param>
		''' <param name="description"> A human readable description of the data. </param>
		''' <param name="descriptor"> The descriptor for the notifications.  This may be null
		''' which is equivalent to an empty descriptor.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal notifTypes As String(), ByVal name As String, ByVal description As String, ByVal descriptor As Descriptor)
			MyBase.New(name, description, descriptor)

	'         We do not validate the notifTypes, since the spec just says
	'           they are dot-separated, not that they must look like Java
	'           classes.  E.g. the spec doesn't forbid "sun.prob.25" as a
	'           notifType, though it doesn't explicitly allow it
	'           either.  

			Me.types = If(notifTypes IsNot Nothing AndAlso notifTypes.Length > 0, notifTypes.clone(), NO_TYPES)
			Me.arrayGettersSafe = MBeanInfo.arrayGettersSafe(Me.GetType(), GetType(MBeanNotificationInfo))
		End Sub


		''' <summary>
		''' Returns a shallow clone of this instance.
		''' The clone is obtained by simply calling <tt>super.clone()</tt>,
		''' thus calling the default native shallow cloning mechanism
		''' implemented by <tt>Object.clone()</tt>.
		''' No deeper cloning of any internal field is made.
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
		''' Returns the array of strings (in dot notation) containing the
		''' notification types that the MBean may emit.
		''' </summary>
		''' <returns> the array of strings.  Changing the returned array has no
		''' effect on this MBeanNotificationInfo. </returns>
		Public Overridable Property notifTypes As String()
			Get
				If types.Length = 0 Then
					Return NO_TYPES
				Else
					Return types.clone()
				End If
			End Get
		End Property

		Private Function fastGetNotifTypes() As String()
			If arrayGettersSafe Then
				Return types
			Else
				Return notifTypes
			End If
		End Function

		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & "description=" & description & ", " & "name=" & name & ", " & "notifTypes=" & java.util.Arrays.asList(fastGetNotifTypes()) & ", " & "descriptor=" & descriptor & "]"
		End Function

		''' <summary>
		''' Compare this MBeanNotificationInfo to another.
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanNotificationInfo
		''' such that its <seealso cref="#getName()"/>, <seealso cref="#getDescription()"/>,
		''' <seealso cref="#getDescriptor()"/>,
		''' and <seealso cref="#getNotifTypes()"/> values are equal (not necessarily
		''' identical) to those of this MBeanNotificationInfo.  Two
		''' notification type arrays are equal if their corresponding
		''' elements are equal.  They are not equal if they have the same
		''' elements but in a different order. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanNotificationInfo) Then Return False
			Dim p As MBeanNotificationInfo = CType(o, MBeanNotificationInfo)
			Return (java.util.Objects.Equals(p.name, name) AndAlso java.util.Objects.Equals(p.description, description) AndAlso java.util.Objects.Equals(p.descriptor, descriptor) AndAlso java.util.Arrays.Equals(p.fastGetNotifTypes(), fastGetNotifTypes()))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = name.GetHashCode()
			For i As Integer = 0 To types.Length - 1
				hash = hash Xor types(i).GetHashCode()
			Next i
			Return hash
		End Function

		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			Dim gf As java.io.ObjectInputStream.GetField = ois.readFields()
			Dim t As String() = CType(gf.get("types", Nothing), String())

			types = If(t IsNot Nothing AndAlso t.Length <> 0, t.clone(), NO_TYPES)
		End Sub
	End Class

End Namespace
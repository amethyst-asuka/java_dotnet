Imports System
Imports System.Collections.Generic
import static javax.management.ImmutableDescriptor.nonNullDescriptor

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
	''' <p>Describes the management interface exposed by an MBean; that is,
	''' the set of attributes and operations which are available for
	''' management operations.  Instances of this class are immutable.
	''' Subclasses may be mutable but this is not recommended.</p>
	''' 
	''' <p id="info-changed">Usually the {@code MBeanInfo} for any given MBean does
	''' not change over the lifetime of that MBean.  Dynamic MBeans can change their
	''' {@code MBeanInfo} and in that case it is recommended that they emit a {@link
	''' Notification} with a <seealso cref="Notification#getType() type"/> of {@code
	''' "jmx.mbean.info.changed"} and a {@link Notification#getUserData()
	''' userData} that is the new {@code MBeanInfo}.  This is not required, but
	''' provides a conventional way for clients of the MBean to discover the change.
	''' See also the <a href="Descriptor.html#immutableInfo">immutableInfo</a> and
	''' <a href="Descriptor.html#infoTimeout">infoTimeout</a> fields in the {@code
	''' MBeanInfo} <seealso cref="Descriptor"/>.</p>
	''' 
	''' <p>The contents of the <code>MBeanInfo</code> for a Dynamic MBean
	''' are determined by its {@link DynamicMBean#getMBeanInfo
	''' getMBeanInfo()} method.  This includes Open MBeans and Model
	''' MBeans, which are kinds of Dynamic MBeans.</p>
	''' 
	''' <p>The contents of the <code>MBeanInfo</code> for a Standard MBean
	''' are determined by the MBean server as follows:</p>
	''' 
	''' <ul>
	''' 
	''' <li><seealso cref="#getClassName()"/> returns the Java class name of the MBean
	''' object;
	''' 
	''' <li><seealso cref="#getConstructors()"/> returns the list of all public
	''' constructors in that object;
	''' 
	''' <li><seealso cref="#getAttributes()"/> returns the list of all attributes
	''' whose existence is deduced from the presence in the MBean interface
	''' of a <code>get<i>Name</i></code>, <code>is<i>Name</i></code>, or
	''' <code>set<i>Name</i></code> method that conforms to the conventions
	''' for Standard MBeans;
	''' 
	''' <li><seealso cref="#getOperations()"/> returns the list of all methods in
	''' the MBean interface that do not represent attributes;
	''' 
	''' <li><seealso cref="#getNotifications()"/> returns an empty array if the MBean
	''' does not implement the <seealso cref="NotificationBroadcaster"/> interface,
	''' otherwise the result of calling {@link
	''' NotificationBroadcaster#getNotificationInfo()} on it;
	''' 
	''' <li><seealso cref="#getDescriptor()"/> returns a descriptor containing the contents
	''' of any descriptor annotations in the MBean interface (see
	''' <seealso cref="DescriptorKey &#64;DescriptorKey"/>).
	''' 
	''' </ul>
	''' 
	''' <p>The description returned by <seealso cref="#getDescription()"/> and the
	''' descriptions of the contained attributes and operations are not specified.</p>
	''' 
	''' <p>The remaining details of the <code>MBeanInfo</code> for a
	''' Standard MBean are not specified.  This includes the description of
	''' any contained constructors, and notifications; the names
	''' of parameters to constructors and operations; and the descriptions of
	''' constructor parameters.</p>
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class MBeanInfo
		Implements ICloneable, DescriptorRead

		' Serial version 
		Friend Const serialVersionUID As Long = -6451021435135161911L

		''' <summary>
		''' @serial The Descriptor for the MBean.  This field
		''' can be null, which is equivalent to an empty Descriptor.
		''' </summary>
		<NonSerialized> _
		Private descriptor As Descriptor

		''' <summary>
		''' @serial The human readable description of the class.
		''' </summary>
		Private ReadOnly description As String

		''' <summary>
		''' @serial The MBean qualified name.
		''' </summary>
		Private ReadOnly className As String

		''' <summary>
		''' @serial The MBean attribute descriptors.
		''' </summary>
		Private ReadOnly attributes As MBeanAttributeInfo()

		''' <summary>
		''' @serial The MBean operation descriptors.
		''' </summary>
		Private ReadOnly operations As MBeanOperationInfo()

		 ''' <summary>
		 ''' @serial The MBean constructor descriptors.
		 ''' </summary>
		Private ReadOnly constructors As MBeanConstructorInfo()

		''' <summary>
		''' @serial The MBean notification descriptors.
		''' </summary>
		Private ReadOnly notifications As MBeanNotificationInfo()

		<NonSerialized> _
		Private ___hashCode As Integer

		''' <summary>
		''' <p>True if this class is known not to override the array-valued
		''' getters of MBeanInfo.  Obviously true for MBeanInfo itself, and true
		''' for a subclass where we succeed in reflecting on the methods
		''' and discover they are not overridden.</p>
		''' 
		''' <p>The purpose of this variable is to avoid cloning the arrays
		''' when doing operations like <seealso cref="#equals"/> where we know they
		''' will not be changed.  If a subclass overrides a getter, we
		''' cannot access the corresponding array directly.</p>
		''' </summary>
		<NonSerialized> _
		Private ReadOnly ___arrayGettersSafe As Boolean

		''' <summary>
		''' Constructs an <CODE>MBeanInfo</CODE>.
		''' </summary>
		''' <param name="className"> The name of the Java class of the MBean described
		''' by this <CODE>MBeanInfo</CODE>.  This value may be any
		''' syntactically legal Java class name.  It does not have to be a
		''' Java class known to the MBean server or to the MBean's
		''' ClassLoader.  If it is a Java class known to the MBean's
		''' ClassLoader, it is recommended but not required that the
		''' class's public methods include those that would appear in a
		''' Standard MBean implementing the attributes and operations in
		''' this MBeanInfo. </param>
		''' <param name="description"> A human readable description of the MBean (optional). </param>
		''' <param name="attributes"> The list of exposed attributes of the MBean.
		''' This may be null with the same effect as a zero-length array. </param>
		''' <param name="constructors"> The list of public constructors of the
		''' MBean.  This may be null with the same effect as a zero-length
		''' array. </param>
		''' <param name="operations"> The list of operations of the MBean.  This
		''' may be null with the same effect as a zero-length array. </param>
		''' <param name="notifications"> The list of notifications emitted.  This
		''' may be null with the same effect as a zero-length array. </param>
		Public Sub New(ByVal className As String, ByVal description As String, ByVal attributes As MBeanAttributeInfo(), ByVal constructors As MBeanConstructorInfo(), ByVal operations As MBeanOperationInfo(), ByVal notifications As MBeanNotificationInfo())
			Me.New(className, description, attributes, constructors, operations, notifications, Nothing)
		End Sub

		''' <summary>
		''' Constructs an <CODE>MBeanInfo</CODE>.
		''' </summary>
		''' <param name="className"> The name of the Java class of the MBean described
		''' by this <CODE>MBeanInfo</CODE>.  This value may be any
		''' syntactically legal Java class name.  It does not have to be a
		''' Java class known to the MBean server or to the MBean's
		''' ClassLoader.  If it is a Java class known to the MBean's
		''' ClassLoader, it is recommended but not required that the
		''' class's public methods include those that would appear in a
		''' Standard MBean implementing the attributes and operations in
		''' this MBeanInfo. </param>
		''' <param name="description"> A human readable description of the MBean (optional). </param>
		''' <param name="attributes"> The list of exposed attributes of the MBean.
		''' This may be null with the same effect as a zero-length array. </param>
		''' <param name="constructors"> The list of public constructors of the
		''' MBean.  This may be null with the same effect as a zero-length
		''' array. </param>
		''' <param name="operations"> The list of operations of the MBean.  This
		''' may be null with the same effect as a zero-length array. </param>
		''' <param name="notifications"> The list of notifications emitted.  This
		''' may be null with the same effect as a zero-length array. </param>
		''' <param name="descriptor"> The descriptor for the MBean.  This may be null
		''' which is equivalent to an empty descriptor.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal className As String, ByVal description As String, ByVal attributes As MBeanAttributeInfo(), ByVal constructors As MBeanConstructorInfo(), ByVal operations As MBeanOperationInfo(), ByVal notifications As MBeanNotificationInfo(), ByVal descriptor As Descriptor)

			Me.className = className

			Me.description = description

			If attributes Is Nothing Then attributes = MBeanAttributeInfo.NO_ATTRIBUTES
			Me.attributes = attributes

			If operations Is Nothing Then operations = MBeanOperationInfo.NO_OPERATIONS
			Me.operations = operations

			If constructors Is Nothing Then constructors = MBeanConstructorInfo.NO_CONSTRUCTORS
			Me.constructors = constructors

			If notifications Is Nothing Then notifications = MBeanNotificationInfo.NO_NOTIFICATIONS
			Me.notifications = notifications

			If descriptor Is Nothing Then descriptor = ImmutableDescriptor.EMPTY_DESCRIPTOR
			Me.descriptor = descriptor

			Me.___arrayGettersSafe = arrayGettersSafe(Me.GetType(), GetType(MBeanInfo))
		End Sub

		''' <summary>
		''' <p>Returns a shallow clone of this instance.
		''' The clone is obtained by simply calling <tt>super.clone()</tt>,
		''' thus calling the default native shallow cloning mechanism
		''' implemented by <tt>Object.clone()</tt>.
		''' No deeper cloning of any internal field is made.</p>
		''' 
		''' <p>Since this class is immutable, the clone method is chiefly of
		''' interest to subclasses.</p>
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
		''' Returns the name of the Java class of the MBean described by
		''' this <CODE>MBeanInfo</CODE>.
		''' </summary>
		''' <returns> the class name. </returns>
		Public Overridable Property className As String
			Get
				Return className
			End Get
		End Property

		''' <summary>
		''' Returns a human readable description of the MBean.
		''' </summary>
		''' <returns> the description. </returns>
		Public Overridable Property description As String
			Get
				Return description
			End Get
		End Property

		''' <summary>
		''' Returns the list of attributes exposed for management.
		''' Each attribute is described by an <CODE>MBeanAttributeInfo</CODE> object.
		''' 
		''' The returned array is a shallow copy of the internal array,
		''' which means that it is a copy of the internal array of
		''' references to the <CODE>MBeanAttributeInfo</CODE> objects
		''' but that each referenced <CODE>MBeanAttributeInfo</CODE> object is not copied.
		''' </summary>
		''' <returns>  An array of <CODE>MBeanAttributeInfo</CODE> objects. </returns>
		Public Overridable Property attributes As MBeanAttributeInfo()
			Get
				Dim [as] As MBeanAttributeInfo() = nonNullAttributes()
				If [as].Length = 0 Then
					Return [as]
				Else
					Return [as].clone()
				End If
			End Get
		End Property

		Private Function fastGetAttributes() As MBeanAttributeInfo()
			If ___arrayGettersSafe Then
				Return nonNullAttributes()
			Else
				Return attributes
			End If
		End Function

		''' <summary>
		''' Return the value of the attributes field, or an empty array if
		''' the field is null.  This can't happen with a
		''' normally-constructed instance of this class, but can if the
		''' instance was deserialized from another implementation that
		''' allows the field to be null.  It would be simpler if we enforced
		''' the class invariant that these fields cannot be null by writing
		''' a readObject() method, but that would require us to define the
		''' various array fields as non-final, which is annoying because
		''' conceptually they are indeed final.
		''' </summary>
		Private Function nonNullAttributes() As MBeanAttributeInfo()
			Return If(attributes Is Nothing, MBeanAttributeInfo.NO_ATTRIBUTES, attributes)
		End Function

		''' <summary>
		''' Returns the list of operations  of the MBean.
		''' Each operation is described by an <CODE>MBeanOperationInfo</CODE> object.
		''' 
		''' The returned array is a shallow copy of the internal array,
		''' which means that it is a copy of the internal array of
		''' references to the <CODE>MBeanOperationInfo</CODE> objects
		''' but that each referenced <CODE>MBeanOperationInfo</CODE> object is not copied.
		''' </summary>
		''' <returns>  An array of <CODE>MBeanOperationInfo</CODE> objects. </returns>
		Public Overridable Property operations As MBeanOperationInfo()
			Get
				Dim os As MBeanOperationInfo() = nonNullOperations()
				If os.Length = 0 Then
					Return os
				Else
					Return os.clone()
				End If
			End Get
		End Property

		Private Function fastGetOperations() As MBeanOperationInfo()
			If ___arrayGettersSafe Then
				Return nonNullOperations()
			Else
				Return operations
			End If
		End Function

		Private Function nonNullOperations() As MBeanOperationInfo()
			Return If(operations Is Nothing, MBeanOperationInfo.NO_OPERATIONS, operations)
		End Function

		''' <summary>
		''' <p>Returns the list of the public constructors of the MBean.
		''' Each constructor is described by an
		''' <CODE>MBeanConstructorInfo</CODE> object.</p>
		''' 
		''' <p>The returned array is a shallow copy of the internal array,
		''' which means that it is a copy of the internal array of
		''' references to the <CODE>MBeanConstructorInfo</CODE> objects but
		''' that each referenced <CODE>MBeanConstructorInfo</CODE> object
		''' is not copied.</p>
		''' 
		''' <p>The returned list is not necessarily exhaustive.  That is,
		''' the MBean may have a public constructor that is not in the
		''' list.  In this case, the MBean server can construct another
		''' instance of this MBean's class using that constructor, even
		''' though it is not listed here.</p>
		''' </summary>
		''' <returns>  An array of <CODE>MBeanConstructorInfo</CODE> objects. </returns>
		Public Overridable Property constructors As MBeanConstructorInfo()
			Get
				Dim cs As MBeanConstructorInfo() = nonNullConstructors()
				If cs.Length = 0 Then
					Return cs
				Else
					Return cs.clone()
				End If
			End Get
		End Property

		Private Function fastGetConstructors() As MBeanConstructorInfo()
			If ___arrayGettersSafe Then
				Return nonNullConstructors()
			Else
				Return constructors
			End If
		End Function

		Private Function nonNullConstructors() As MBeanConstructorInfo()
			Return If(constructors Is Nothing, MBeanConstructorInfo.NO_CONSTRUCTORS, constructors)
		End Function

		''' <summary>
		''' Returns the list of the notifications emitted by the MBean.
		''' Each notification is described by an <CODE>MBeanNotificationInfo</CODE> object.
		''' 
		''' The returned array is a shallow copy of the internal array,
		''' which means that it is a copy of the internal array of
		''' references to the <CODE>MBeanNotificationInfo</CODE> objects
		''' but that each referenced <CODE>MBeanNotificationInfo</CODE> object is not copied.
		''' </summary>
		''' <returns>  An array of <CODE>MBeanNotificationInfo</CODE> objects. </returns>
		Public Overridable Property notifications As MBeanNotificationInfo()
			Get
				Dim ns As MBeanNotificationInfo() = nonNullNotifications()
				If ns.Length = 0 Then
					Return ns
				Else
					Return ns.clone()
				End If
			End Get
		End Property

		Private Function fastGetNotifications() As MBeanNotificationInfo()
			If ___arrayGettersSafe Then
				Return nonNullNotifications()
			Else
				Return notifications
			End If
		End Function

		Private Function nonNullNotifications() As MBeanNotificationInfo()
			Return If(notifications Is Nothing, MBeanNotificationInfo.NO_NOTIFICATIONS, notifications)
		End Function

		''' <summary>
		''' Get the descriptor of this MBeanInfo.  Changing the returned value
		''' will have no affect on the original descriptor.
		''' </summary>
		''' <returns> a descriptor that is either immutable or a copy of the original.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property descriptor As Descriptor Implements DescriptorRead.getDescriptor
			Get
				Return CType(nonNullDescriptor(descriptor).clone(), Descriptor)
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & "description=" & description & ", " & "attributes=" & java.util.Arrays.asList(fastGetAttributes()) & ", " & "constructors=" & java.util.Arrays.asList(fastGetConstructors()) & ", " & "operations=" & java.util.Arrays.asList(fastGetOperations()) & ", " & "notifications=" & java.util.Arrays.asList(fastGetNotifications()) & ", " & "descriptor=" & descriptor & "]"
		End Function

		''' <summary>
		''' <p>Compare this MBeanInfo to another.  Two MBeanInfo objects
		''' are equal if and only if they return equal values for {@link
		''' #getClassName()}, for <seealso cref="#getDescription()"/>, and for
		''' <seealso cref="#getDescriptor()"/>, and the
		''' arrays returned by the two objects for {@link
		''' #getAttributes()}, <seealso cref="#getOperations()"/>, {@link
		''' #getConstructors()}, and <seealso cref="#getNotifications()"/> are
		''' pairwise equal.  Here "equal" means {@link
		''' Object#equals(Object)}, not identity.</p>
		''' 
		''' <p>If two MBeanInfo objects return the same values in one of
		''' their arrays but in a different order then they are not equal.</p>
		''' </summary>
		''' <param name="o"> the object to compare to.
		''' </param>
		''' <returns> true if and only if <code>o</code> is an MBeanInfo that is equal
		''' to this one according to the rules above. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is MBeanInfo) Then Return False
			Dim p As MBeanInfo = CType(o, MBeanInfo)
			If (Not isEqual(className, p.className)) OrElse (Not isEqual(description, p.description)) OrElse (Not descriptor.Equals(p.descriptor)) Then Return False

			Return (java.util.Arrays.Equals(p.fastGetAttributes(), fastGetAttributes()) AndAlso java.util.Arrays.Equals(p.fastGetOperations(), fastGetOperations()) AndAlso java.util.Arrays.Equals(p.fastGetConstructors(), fastGetConstructors()) AndAlso java.util.Arrays.Equals(p.fastGetNotifications(), fastGetNotifications()))
		End Function

		Public Overrides Function GetHashCode() As Integer
	'         Since computing the hashCode is quite expensive, we cache it.
	'           If by some terrible misfortune the computed value is 0, the
	'           caching won't work and we will recompute it every time.
	'
	'           We don't bother synchronizing, because, at worst, n different
	'           threads will compute the same hashCode at the same time.  
			If ___hashCode <> 0 Then Return ___hashCode

			___hashCode = java.util.Objects.hash(className, descriptor) Xor java.util.Arrays.hashCode(fastGetAttributes()) Xor java.util.Arrays.hashCode(fastGetOperations()) Xor java.util.Arrays.hashCode(fastGetConstructors()) Xor java.util.Arrays.hashCode(fastGetNotifications())

			Return ___hashCode
		End Function

		''' <summary>
		''' Cached results of previous calls to arrayGettersSafe.  This is
		''' a WeakHashMap so that we don't prevent a class from being
		''' garbage collected just because we know whether it's immutable.
		''' </summary>
		Private Shared ReadOnly arrayGettersSafeMap As IDictionary(Of Type, Boolean?) = New java.util.WeakHashMap(Of Type, Boolean?)

		''' <summary>
		''' Return true if <code>subclass</code> is known to preserve the
		''' immutability of <code>immutableClass</code>.  The class
		''' <code>immutableClass</code> is a reference class that is known
		''' to be immutable.  The subclass <code>subclass</code> is
		''' considered immutable if it does not override any public method
		''' of <code>immutableClass</code> whose name begins with "get".
		''' This is obviously not an infallible test for immutability,
		''' but it works for the public interfaces of the MBean*Info classes.
		''' </summary>
		Friend Shared Function arrayGettersSafe(ByVal subclass As Type, ByVal immutableClass As Type) As Boolean
			If subclass Is immutableClass Then Return True
			SyncLock arrayGettersSafeMap
				Dim safe As Boolean? = arrayGettersSafeMap(subclass)
				If safe Is Nothing Then
					Try
						Dim action As New ArrayGettersSafeAction(subclass, immutableClass)
						safe = java.security.AccessController.doPrivileged(action) ' e.g. SecurityException
					Catch e As Exception
						' We don't know, so we assume it isn't.  
						safe = False
					End Try
					arrayGettersSafeMap(subclass) = safe
				End If
				Return safe
			End SyncLock
		End Function

	'    
	'     * The PrivilegedAction stuff is probably overkill.  We can be
	'     * pretty sure the caller does have the required privileges -- a
	'     * JMX user that can't do reflection can't even use Standard
	'     * MBeans!  But there's probably a performance gain by not having
	'     * to check the whole call stack.
	'     
		Private Class ArrayGettersSafeAction
			Implements java.security.PrivilegedAction(Of Boolean?)

			Private ReadOnly subclass As Type
			Private ReadOnly immutableClass As Type

			Friend Sub New(ByVal subclass As Type, ByVal immutableClass As Type)
				Me.subclass = subclass
				Me.immutableClass = immutableClass
			End Sub

			Public Overridable Function run() As Boolean?
				Dim methods As Method() = immutableClass.GetMethods()
				For i As Integer = 0 To methods.Length - 1
					Dim method As Method = methods(i)
					Dim methodName As String = method.name
					If methodName.StartsWith("get") AndAlso method.parameterTypes.length = 0 AndAlso method.returnType.array Then
						Try
							Dim submethod As Method = subclass.GetMethod(methodName)
							If Not submethod.Equals(method) Then Return False
						Catch e As NoSuchMethodException
							Return False
						End Try
					End If
				Next i
				Return True
			End Function
		End Class

		Private Shared Function isEqual(ByVal s1 As String, ByVal s2 As String) As Boolean
			Dim ret As Boolean

			If s1 Is Nothing Then
				ret = (s2 Is Nothing)
			Else
				ret = s1.Equals(s2)
			End If

			Return ret
		End Function

		''' <summary>
		''' Serializes an <seealso cref="MBeanInfo"/> to an <seealso cref="ObjectOutputStream"/>.
		''' @serialData
		''' For compatibility reasons, an object of this class is serialized as follows.
		''' <p>
		''' The method <seealso cref="ObjectOutputStream#defaultWriteObject defaultWriteObject()"/>
		''' is called first to serialize the object except the field {@code descriptor}
		''' which is declared as transient. The field {@code descriptor} is serialized
		''' as follows:
		'''     <ul>
		'''     <li> If {@code descriptor} is an instance of the class
		'''        <seealso cref="ImmutableDescriptor"/>, the method {@link ObjectOutputStream#write
		'''        write(int val)} is called to write a byte with the value {@code 1},
		'''        then the method <seealso cref="ObjectOutputStream#writeObject writeObject(Object obj)"/>
		'''        is called twice to serialize the field names and the field values of the
		'''        {@code descriptor}, respectively as a {@code String[]} and an
		'''        {@code Object[]};</li>
		'''     <li> Otherwise, the method <seealso cref="ObjectOutputStream#write write(int val)"/>
		'''        is called to write a byte with the value {@code 0}, then the method
		'''        <seealso cref="ObjectOutputStream#writeObject writeObject(Object obj)"/> is called
		'''        to serialize the field {@code descriptor} directly.
		'''     </ul>
		''' 
		''' @since 1.6
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			out.defaultWriteObject()

			If descriptor.GetType() Is GetType(ImmutableDescriptor) Then
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
		''' Deserializes an <seealso cref="MBeanInfo"/> from an <seealso cref="ObjectInputStream"/>.
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
		'''       an earlier version of the JMX API. The field {@code descriptor} is set to
		'''       <seealso cref="ImmutableDescriptor#EMPTY_DESCRIPTOR EMPTY_DESCRIPTOR"/>.</li>
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
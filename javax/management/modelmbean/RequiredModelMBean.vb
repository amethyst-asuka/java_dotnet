Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.MODELMBEAN_LOGGER

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
'
' * @author    IBM Corp.
' *
' * Copyright IBM Corp. 1999-2000.  All rights reserved.
' 


Namespace javax.management.modelmbean

	' java imports 






	''' <summary>
	''' This class is the implementation of a ModelMBean. An appropriate
	''' implementation of a ModelMBean must be shipped with every JMX Agent
	''' and the class must be named RequiredModelMBean.
	''' <P>
	''' Java resources wishing to be manageable instantiate the
	''' RequiredModelMBean using the MBeanServer's createMBean method.
	''' The resource then sets the MBeanInfo and Descriptors for the
	''' RequiredModelMBean instance. The attributes and operations exposed
	''' via the ModelMBeanInfo for the ModelMBean are accessible
	''' from MBeans, connectors/adaptors like other MBeans. Through the
	''' Descriptors, values and methods in the managed application can be
	''' defined and mapped to attributes and operations of the ModelMBean.
	''' This mapping can be defined in an XML formatted file or dynamically and
	''' programmatically at runtime.
	''' <P>
	''' Every RequiredModelMBean which is instantiated in the MBeanServer
	''' becomes manageable:<br>
	''' its attributes and operations become remotely accessible through the
	''' connectors/adaptors connected to that MBeanServer.
	''' <P>
	''' A Java object cannot be registered in the MBeanServer unless it is a
	''' JMX compliant MBean. By instantiating a RequiredModelMBean, resources
	''' are guaranteed that the MBean is valid.
	''' 
	''' MBeanException and RuntimeOperationsException must be thrown on every
	''' public method.  This allows for wrapping exceptions from distributed
	''' communications (RMI, EJB, etc.)
	''' 
	''' @since 1.5
	''' </summary>

	Public Class RequiredModelMBean
		Implements ModelMBean, javax.management.MBeanRegistration, javax.management.NotificationEmitter

		''' <summary>
		'''********************************** </summary>
		' attributes                        
		''' <summary>
		'''********************************** </summary>
		Friend modelMBeanInfo As ModelMBeanInfo

	'     Notification broadcaster for any notification to be sent
	'     * from the application through the RequiredModelMBean.  
		Private generalBroadcaster As javax.management.NotificationBroadcasterSupport = Nothing

		' Notification broadcaster for attribute change notifications 
		Private attributeBroadcaster As javax.management.NotificationBroadcasterSupport = Nothing

	'     handle, name, or reference for instance on which the actual invoke
	'     * and operations will be executed 
		Private managedResource As Object = Nothing


		' records the registering in MBeanServer 
		Private registered As Boolean = False
		<NonSerialized> _
		Private server As javax.management.MBeanServer = Nothing

		Private Shared ReadOnly javaSecurityAccess As sun.misc.JavaSecurityAccess = sun.misc.SharedSecrets.javaSecurityAccess
		Private ReadOnly acc As java.security.AccessControlContext = java.security.AccessController.context

		''' <summary>
		'''********************************** </summary>
		' constructors                      
		''' <summary>
		'''********************************** </summary>

		''' <summary>
		''' Constructs an <CODE>RequiredModelMBean</CODE> with an empty
		''' ModelMBeanInfo.
		''' <P>
		''' The RequiredModelMBean's MBeanInfo and Descriptors
		''' can be customized using the <seealso cref="#setModelMBeanInfo"/> method.
		''' After the RequiredModelMBean's MBeanInfo and Descriptors are
		''' customized, the RequiredModelMBean can be registered with
		''' the MBeanServer.
		''' </summary>
		''' <exception cref="MBeanException"> Wraps a distributed communication Exception.
		''' </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a {@link
		''' RuntimeException} during the construction of the object.
		'''  </exception>
		Public Sub New()
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "RequiredModelMBean()", "Entry")
			modelMBeanInfo = createDefaultModelMBeanInfo()
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "RequiredModelMBean()", "Exit")
		End Sub

		''' <summary>
		''' Constructs a RequiredModelMBean object using ModelMBeanInfo passed in.
		''' As long as the RequiredModelMBean is not registered
		''' with the MBeanServer yet, the RequiredModelMBean's MBeanInfo and
		''' Descriptors can be customized using the <seealso cref="#setModelMBeanInfo"/>
		''' method.
		''' After the RequiredModelMBean's MBeanInfo and Descriptors are
		''' customized, the RequiredModelMBean can be registered with the
		''' MBeanServer.
		''' </summary>
		''' <param name="mbi"> The ModelMBeanInfo object to be used by the
		'''            RequiredModelMBean. The given ModelMBeanInfo is cloned
		'''            and modified as specified by <seealso cref="#setModelMBeanInfo"/>
		''' </param>
		''' <exception cref="MBeanException"> Wraps a distributed communication Exception. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		'''    {link java.lang.IllegalArgumentException}:
		'''          The MBeanInfo passed in parameter is null.
		''' 
		'''  </exception>
		Public Sub New(ByVal mbi As ModelMBeanInfo)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "RequiredModelMBean(MBeanInfo)", "Entry")
			modelMBeanInfo = mbi

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "RequiredModelMBean(MBeanInfo)", "Exit")
		End Sub


		''' <summary>
		'''********************************** </summary>
		' initializers                      
		''' <summary>
		'''********************************** </summary>

		''' <summary>
		''' Initializes a ModelMBean object using ModelMBeanInfo passed in.
		''' This method makes it possible to set a customized ModelMBeanInfo on
		''' the ModelMBean as long as it is not registered with the MBeanServer.
		''' <br>
		''' Once the ModelMBean's ModelMBeanInfo (with Descriptors) are
		''' customized and set on the ModelMBean, the  ModelMBean be
		''' registered with the MBeanServer.
		''' <P>
		''' If the ModelMBean is currently registered, this method throws
		''' a <seealso cref="javax.management.RuntimeOperationsException"/> wrapping an
		''' <seealso cref="IllegalStateException"/>
		''' <P>
		''' If the given <var>inModelMBeanInfo</var> does not contain any
		''' <seealso cref="ModelMBeanNotificationInfo"/> for the <code>GENERIC</code>
		''' or <code>ATTRIBUTE_CHANGE</code> notifications, then the
		''' RequiredModelMBean will supply its own default
		''' <seealso cref="ModelMBeanNotificationInfo ModelMBeanNotificationInfo"/>s for
		''' those missing notifications.
		''' </summary>
		''' <param name="mbi"> The ModelMBeanInfo object to be used
		'''        by the ModelMBean.
		''' </param>
		''' <exception cref="MBeanException"> Wraps a distributed communication
		'''        Exception. </exception>
		''' <exception cref="RuntimeOperationsException">
		''' <ul><li>Wraps an <seealso cref="IllegalArgumentException"/> if
		'''         the MBeanInfo passed in parameter is null.</li>
		'''     <li>Wraps an <seealso cref="IllegalStateException"/> if the ModelMBean
		'''         is currently registered in the MBeanServer.</li>
		''' </ul>
		''' 
		'''  </exception>
		Public Overridable Property modelMBeanInfo Implements ModelMBean.setModelMBeanInfo As ModelMBeanInfo
			Set(ByVal mbi As ModelMBeanInfo)
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)","Entry")
    
				If mbi Is Nothing Then
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)", "ModelMBeanInfo is null: Raising exception.")
					Dim x As Exception = New System.ArgumentException("ModelMBeanInfo must not be null")
					Dim exceptionText As String = "Exception occurred trying to initialize the " & "ModelMBeanInfo of the RequiredModelMBean"
					Throw New javax.management.RuntimeOperationsException(x,exceptionText)
				End If
    
				If registered Then
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)", "RequiredMBean is registered: Raising exception.")
					Dim exceptionText As String = "Exception occurred trying to set the " & "ModelMBeanInfo of the RequiredModelMBean"
					Dim x As Exception = New IllegalStateException("cannot call setModelMBeanInfo while ModelMBean is registered")
					Throw New javax.management.RuntimeOperationsException(x,exceptionText)
				End If
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)", "Setting ModelMBeanInfo to " & printModelMBeanInfo(mbi))
					Dim noOfNotifications As Integer = 0
					If mbi.notifications IsNot Nothing Then noOfNotifications = mbi.notifications.Length
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)", "ModelMBeanInfo notifications has " & noOfNotifications & " elements")
				End If
    
				modelMBeanInfo = CType(mbi.clone(), ModelMBeanInfo)
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)","set mbeanInfo to: " & printModelMBeanInfo(modelMBeanInfo))
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setModelMBeanInfo(ModelMBeanInfo)","Exit")
				End If
			End Set
		End Property


		''' <summary>
		''' Sets the instance handle of the object against which to
		''' execute all methods in this ModelMBean management interface
		''' (MBeanInfo and Descriptors).
		''' </summary>
		''' <param name="mr"> Object that is the managed resource </param>
		''' <param name="mr_type"> The type of reference for the managed resource.
		'''     <br>Can be: "ObjectReference", "Handle", "IOR", "EJBHandle",
		'''         or "RMIReference".
		'''     <br>In this implementation only "ObjectReference" is supported.
		''' </param>
		''' <exception cref="MBeanException"> The initializer of the object has
		'''            thrown an exception. </exception>
		''' <exception cref="InstanceNotFoundException"> The managed resource
		'''            object could not be found </exception>
		''' <exception cref="InvalidTargetObjectTypeException"> The managed
		'''            resource type should be "ObjectReference". </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a {@link
		'''            RuntimeException} when setting the resource.
		'''  </exception>
		Public Overridable Sub setManagedResource(ByVal mr As Object, ByVal mr_type As String) Implements ModelMBean.setManagedResource
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setManagedResource(Object,String)","Entry")

			' check that the mr_type is supported by this JMXAgent
			' only "objectReference" is supported
			If (mr_type Is Nothing) OrElse ((Not mr_type.ToUpper()) = "objectReference".ToUpper()) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setManagedResource(Object,String)", "Managed Resource Type is not supported: " & mr_type)
				Throw New InvalidTargetObjectTypeException(mr_type)
			End If

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setManagedResource(Object,String)", "Managed Resource is valid")
			managedResource = mr

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setManagedResource(Object, String)", "Exit")
		End Sub

		''' <summary>
		''' <p>Instantiates this MBean instance with the data found for
		''' the MBean in the persistent store.  The data loaded could include
		''' attribute and operation values.</p>
		''' 
		''' <p>This method should be called during construction or
		''' initialization of this instance, and before the MBean is
		''' registered with the MBeanServer.</p>
		''' 
		''' <p>If the implementation of this class does not support
		''' persistence, an <seealso cref="MBeanException"/> wrapping a {@link
		''' ServiceNotFoundException} is thrown.</p>
		''' </summary>
		''' <exception cref="MBeanException"> Wraps another exception, or
		''' persistence is not supported </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps exceptions from the
		''' persistence mechanism </exception>
		''' <exception cref="InstanceNotFoundException"> Could not find or load
		''' this MBean from persistent storage </exception>
		Public Overridable Sub load() Implements PersistentMBean.load
			Dim x As New javax.management.ServiceNotFoundException("Persistence not supported for this MBean")
			Throw New javax.management.MBeanException(x, x.Message)
		End Sub

			''' <summary>
			''' <p>Captures the current state of this MBean instance and writes
			''' it out to the persistent store.  The state stored could include
			''' attribute and operation values.</p>
			''' 
			''' <p>If the implementation of this class does not support
			''' persistence, an <seealso cref="MBeanException"/> wrapping a {@link
			''' ServiceNotFoundException} is thrown.</p>
			''' 
			''' <p>Persistence policy from the MBean and attribute descriptor
			''' is used to guide execution of this method. The MBean should be
			''' stored if 'persistPolicy' field is:</p>
			''' 
			''' <PRE>{@literal  != "never"
			'''   = "always"
			'''   = "onTimer" and now > 'lastPersistTime' + 'persistPeriod'
			'''   = "NoMoreOftenThan" and now > 'lastPersistTime' + 'persistPeriod'
			'''   = "onUnregister"
			''' }</PRE>
			''' 
			''' <p>Do not store the MBean if 'persistPolicy' field is:</p>
			''' <PRE>{@literal
			'''    = "never"
			'''    = "onUpdate"
			'''    = "onTimer" && now < 'lastPersistTime' + 'persistPeriod'
			''' }</PRE>
			''' </summary>
			''' <exception cref="MBeanException"> Wraps another exception, or
			''' persistence is not supported </exception>
			''' <exception cref="RuntimeOperationsException"> Wraps exceptions from the
			''' persistence mechanism </exception>
			''' <exception cref="InstanceNotFoundException"> Could not find/access the
			''' persistent store </exception>
		Public Overridable Sub store() Implements PersistentMBean.store
			Dim x As New javax.management.ServiceNotFoundException("Persistence not supported for this MBean")
			Throw New javax.management.MBeanException(x, x.Message)
		End Sub

		''' <summary>
		'''********************************** </summary>
		' DynamicMBean Interface            
		''' <summary>
		'''********************************** </summary>

		''' <summary>
		''' The resolveForCacheValue method checks the descriptor passed in to
		''' see if there is a valid cached value in the descriptor.
		''' The valid value will be in the 'value' field if there is one.
		''' If the 'currencyTimeLimit' field in the descriptor is:
		''' <ul>
		'''   <li><b>&lt;0</b> Then the value is not cached and is never valid.
		'''         Null is returned. The 'value' and 'lastUpdatedTimeStamp'
		'''         fields are cleared.</li>
		'''   <li><b>=0</b> Then the value is always cached and always valid.
		'''         The 'value' field is returned.
		'''         The 'lastUpdatedTimeStamp' field is not checked.</li>
		'''   <li><b>&gt;0</b> Represents the number of seconds that the
		'''         'value' field is valid.
		'''         The 'value' field is no longer valid when
		'''         'lastUpdatedTimeStamp' + 'currencyTimeLimit' &gt; Now.
		'''       <ul>
		'''       <li>When 'value' is valid, 'valid' is returned.</li>
		'''       <li>When 'value' is no longer valid then null is returned and
		'''           'value' and 'lastUpdatedTimeStamp' fields are cleared.</li>
		'''       </ul>
		'''   </li>
		''' </ul>
		''' 
		''' 
		''' </summary>
		Private Function resolveForCacheValue(ByVal descr As javax.management.Descriptor) As Object

			Dim tracing As Boolean = MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER)
			Const mth As String = "resolveForCacheValue(Descriptor)"
			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Entry")

			Dim response As Object = Nothing
			Dim resetValue As Boolean = False, returnCachedValue As Boolean = True
			Dim currencyPeriod As Long = 0

			If descr Is Nothing Then
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "Input Descriptor is null")
				Return response
			End If

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "descriptor is " & descr)

			Dim mmbDescr As javax.management.Descriptor = modelMBeanInfo.mBeanDescriptor
			If mmbDescr Is Nothing Then
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth,"MBean Descriptor is null")
				'return response;
			End If

			Dim objExpTime As Object = descr.getFieldValue("currencyTimeLimit")

			Dim expTime As String
			If objExpTime IsNot Nothing Then
				expTime = objExpTime.ToString()
			Else
				expTime = Nothing
			End If

			If (expTime Is Nothing) AndAlso (mmbDescr IsNot Nothing) Then
				objExpTime = mmbDescr.getFieldValue("currencyTimeLimit")
				If objExpTime IsNot Nothing Then
					expTime = objExpTime.ToString()
				Else
					expTime = Nothing
				End If
			End If

			If expTime IsNot Nothing Then
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth,"currencyTimeLimit: " & expTime)

				' convert seconds to milliseconds for time comparison
				currencyPeriod = ((New Long?(expTime))) * 1000
				If currencyPeriod < 0 Then
					' if currencyTimeLimit is -1 then value is never cached 
					returnCachedValue = False
					resetValue = True
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, currencyPeriod & ": never Cached")
				ElseIf currencyPeriod = 0 Then
					' if currencyTimeLimit is 0 then value is always cached 
					returnCachedValue = True
					resetValue = False
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "always valid Cache")
				Else
					Dim objtStamp As Object = descr.getFieldValue("lastUpdatedTimeStamp")

					Dim tStamp As String
					If objtStamp IsNot Nothing Then
						tStamp = objtStamp.ToString()
					Else
						tStamp = Nothing
					End If

					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "lastUpdatedTimeStamp: " & tStamp)

					If tStamp Is Nothing Then tStamp = "0"

					Dim lastTime As Long = (New Long?(tStamp))

					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "currencyPeriod:" & currencyPeriod & " lastUpdatedTimeStamp:" & lastTime)

					Dim now As Long = (DateTime.Now).time

					If now < (lastTime + currencyPeriod) Then
						returnCachedValue = True
						resetValue = False
						If tracing Then
							MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, " timed valid Cache for " & now & " < " & (lastTime + currencyPeriod))
						End If ' value is expired
					Else
						returnCachedValue = False
						resetValue = True
						If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "timed expired cache for " & now & " > " & (lastTime + currencyPeriod))
					End If
				End If
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "returnCachedValue:" & returnCachedValue & " resetValue: " & resetValue)

				If returnCachedValue = True Then
					Dim currValue As Object = descr.getFieldValue("value")
					If currValue IsNot Nothing Then
						' error/validity check return value here 
						response = currValue
						' need to cast string cached value to type 
						If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "valid Cache value: " & currValue)

					Else
						response = Nothing
						If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth,"no Cached value")
					End If
				End If

				If resetValue = True Then
					' value is not current, so remove it 
					descr.removeField("lastUpdatedTimeStamp")
					descr.removeField("value")
					response = Nothing
					modelMBeanInfo.descriptortor(descr,Nothing)
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth,"reset cached value to null")
				End If
			End If

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Exit")

			Return response
		End Function

		''' <summary>
		''' Returns the attributes, operations, constructors and notifications
		''' that this RequiredModelMBean exposes for management.
		''' </summary>
		''' <returns>  An instance of ModelMBeanInfo allowing retrieval all
		'''          attributes, operations, and Notifications of this MBean.
		''' 
		'''  </returns>
		Public Overridable Property mBeanInfo As javax.management.MBeanInfo
			Get
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getMBeanInfo()","Entry")
    
				If modelMBeanInfo Is Nothing Then
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getMBeanInfo()","modelMBeanInfo is null")
					modelMBeanInfo = createDefaultModelMBeanInfo()
					'return new ModelMBeanInfo(" ", "", null, null, null, null);
				End If
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getMBeanInfo()","ModelMBeanInfo is " & modelMBeanInfo.className & " for " & modelMBeanInfo.description)
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getMBeanInfo()",printModelMBeanInfo(modelMBeanInfo))
				End If
    
				Return (CType(modelMBeanInfo.clone(), javax.management.MBeanInfo))
			End Get
		End Property

		Private Function printModelMBeanInfo(ByVal info As ModelMBeanInfo) As String
			Dim retStr As New StringBuilder
			If info Is Nothing Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "printModelMBeanInfo(ModelMBeanInfo)", "ModelMBeanInfo to print is null, " & "printing local ModelMBeanInfo")
				info = modelMBeanInfo
			End If

			retStr.Append(vbLf & "MBeanInfo for ModelMBean is:")
			retStr.Append(vbLf & "CLASSNAME: " & vbTab & info.className)
			retStr.Append(vbLf & "DESCRIPTION: " & vbTab & info.description)


			Try
				retStr.Append(vbLf & "MBEAN DESCRIPTOR: " & vbTab & info.mBeanDescriptor)
			Catch e As Exception
				retStr.Append(vbLf & "MBEAN DESCRIPTOR: " & vbTab & " is invalid")
			End Try

			retStr.Append(vbLf & "ATTRIBUTES")

			Dim attrInfo As javax.management.MBeanAttributeInfo() = info.attributes
			If (attrInfo IsNot Nothing) AndAlso (attrInfo.Length>0) Then
				For i As Integer = 0 To attrInfo.Length - 1
					Dim attInfo As ModelMBeanAttributeInfo = CType(attrInfo(i), ModelMBeanAttributeInfo)
					retStr.Append(" ** NAME: " & vbTab & attInfo.name)
					retStr.Append("    DESCR: " & vbTab & attInfo.description)
					retStr.Append("    TYPE: " & vbTab & attInfo.type & "    READ: " & vbTab & attInfo.readable & "    WRITE: " & vbTab & attInfo.writable)
					retStr.Append("    DESCRIPTOR: " & attInfo.descriptor.ToString())
				Next i
			Else
				retStr.Append(" ** No attributes **")
			End If

			retStr.Append(vbLf & "CONSTRUCTORS")
			Dim constrInfo As javax.management.MBeanConstructorInfo() = info.constructors
			If (constrInfo IsNot Nothing) AndAlso (constrInfo.Length > 0) Then
				For i As Integer = 0 To constrInfo.Length - 1
					Dim ctorInfo As ModelMBeanConstructorInfo = CType(constrInfo(i), ModelMBeanConstructorInfo)
					retStr.Append(" ** NAME: " & vbTab & ctorInfo.name)
					retStr.Append("    DESCR: " & vbTab & ctorInfo.description)
					retStr.Append("    PARAM: " & vbTab & ctorInfo.signature.Length & " parameter(s)")
					retStr.Append("    DESCRIPTOR: " & ctorInfo.descriptor.ToString())
				Next i
			Else
				retStr.Append(" ** No Constructors **")
			End If

			retStr.Append(vbLf & "OPERATIONS")
			Dim opsInfo As javax.management.MBeanOperationInfo() = info.operations
			If (opsInfo IsNot Nothing) AndAlso (opsInfo.Length>0) Then
				For i As Integer = 0 To opsInfo.Length - 1
					Dim operInfo As ModelMBeanOperationInfo = CType(opsInfo(i), ModelMBeanOperationInfo)
					retStr.Append(" ** NAME: " & vbTab & operInfo.name)
					retStr.Append("    DESCR: " & vbTab & operInfo.description)
					retStr.Append("    PARAM: " & vbTab & operInfo.signature.Length & " parameter(s)")
					retStr.Append("    DESCRIPTOR: " & operInfo.descriptor.ToString())
				Next i
			Else
				retStr.Append(" ** No operations ** ")
			End If

			retStr.Append(vbLf & "NOTIFICATIONS")

			Dim notifInfo As javax.management.MBeanNotificationInfo() = info.notifications
			If (notifInfo IsNot Nothing) AndAlso (notifInfo.Length>0) Then
				For i As Integer = 0 To notifInfo.Length - 1
					Dim nInfo As ModelMBeanNotificationInfo = CType(notifInfo(i), ModelMBeanNotificationInfo)
					retStr.Append(" ** NAME: " & vbTab & nInfo.name)
					retStr.Append("    DESCR: " & vbTab & nInfo.description)
					retStr.Append("    DESCRIPTOR: " & nInfo.descriptor.ToString())
				Next i
			Else
				retStr.Append(" ** No notifications **")
			End If

			retStr.Append(" ** ModelMBean: End of MBeanInfo ** ")

			Return retStr.ToString()
		End Function

		''' <summary>
		''' Invokes a method on or through a RequiredModelMBean and returns
		''' the result of the method execution.
		''' <P>
		''' If the given method to be invoked, together with the provided
		''' signature, matches one of RequiredModelMbean
		''' accessible methods, this one will be call. Otherwise the call to
		''' the given method will be tried on the managed resource.
		''' <P>
		''' The last value returned by an operation may be cached in
		''' the operation's descriptor which
		''' is in the ModelMBeanOperationInfo's descriptor.
		''' The valid value will be in the 'value' field if there is one.
		''' If the 'currencyTimeLimit' field in the descriptor is:
		''' <UL>
		''' <LI><b>&lt;0</b> Then the value is not cached and is never valid.
		'''      The operation method is invoked.
		'''      The 'value' and 'lastUpdatedTimeStamp' fields are cleared.</LI>
		''' <LI><b>=0</b> Then the value is always cached and always valid.
		'''      The 'value' field is returned. If there is no 'value' field
		'''      then the operation method is invoked for the attribute.
		'''      The 'lastUpdatedTimeStamp' field and `value' fields are set to
		'''      the operation's return value and the current time stamp.</LI>
		''' <LI><b>&gt;0</b> Represents the number of seconds that the 'value'
		'''      field is valid.
		'''      The 'value' field is no longer valid when
		'''      'lastUpdatedTimeStamp' + 'currencyTimeLimit' &gt; Now.
		'''      <UL>
		'''         <LI>When 'value' is valid, 'value' is returned.</LI>
		'''         <LI>When 'value' is no longer valid then the operation
		'''             method is invoked. The 'lastUpdatedTimeStamp' field
		'''             and `value' fields are updated.</lI>
		'''      </UL>
		''' </LI>
		''' </UL>
		''' 
		''' <p><b>Note:</b> because of inconsistencies in previous versions of
		''' this specification, it is recommended not to use negative or zero
		''' values for <code>currencyTimeLimit</code>.  To indicate that a
		''' cached value is never valid, omit the
		''' <code>currencyTimeLimit</code> field.  To indicate that it is
		''' always valid, use a very large number for this field.</p>
		''' </summary>
		''' <param name="opName"> The name of the method to be invoked. The
		'''     name can be the fully qualified method name including the
		'''     classname, or just the method name if the classname is
		'''     defined in the 'class' field of the operation descriptor. </param>
		''' <param name="opArgs"> An array containing the parameters to be set
		'''     when the operation is invoked </param>
		''' <param name="sig"> An array containing the signature of the
		'''     operation. The class objects will be loaded using the same
		'''     class loader as the one used for loading the MBean on which
		'''     the operation was invoked.
		''' </param>
		''' <returns>  The object returned by the method, which represents the
		'''     result of invoking the method on the specified managed resource.
		''' </returns>
		''' <exception cref="MBeanException">  Wraps one of the following Exceptions:
		''' <UL>
		''' <LI> An Exception thrown by the managed object's invoked method.</LI>
		''' <LI> <seealso cref="ServiceNotFoundException"/>: No ModelMBeanOperationInfo or
		'''      no descriptor defined for the specified operation or the managed
		'''      resource is null.</LI>
		''' <LI> <seealso cref="InvalidTargetObjectTypeException"/>: The 'targetType'
		'''      field value is not 'objectReference'.</LI>
		''' </UL> </exception>
		''' <exception cref="ReflectionException">  Wraps an <seealso cref="java.lang.Exception"/>
		'''      thrown while trying to invoke the method. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		'''      <seealso cref="IllegalArgumentException"/> Method name is null.
		''' 
		'''  </exception>
	'    
	'      The requirement to be able to invoke methods on the
	'      RequiredModelMBean class itself makes this method considerably
	'      more complicated than it might otherwise be.  Note that, unlike
	'      earlier versions, we do not allow you to invoke such methods if
	'      they are not explicitly mentioned in the ModelMBeanInfo.  Doing
	'      so was potentially a security problem, and certainly very
	'      surprising.
	'
	'      We do not look for the method in the RequiredModelMBean class
	'      itself if:
	'      (a) there is a "targetObject" field in the Descriptor for the
	'      operation; or
	'      (b) there is a "class" field in the Descriptor for the operation
	'      and the named class is not RequiredModelMBean or one of its
	'      superinterfaces; or
	'      (c) the name of the operation is not the name of a method in
	'      RequiredModelMBean (this is just an optimization).
	'
	'      In cases (a) and (b), if you have gone to the trouble of adding
	'      those fields specifically for this operation then presumably you
	'      do not want RequiredModelMBean's methods to be called.
	'
	'      We have to pay attention to class loading issues.  If the
	'      "class" field is present, the named class has to be resolved
	'      relative to RequiredModelMBean's class loader to test the
	'      condition (b) above, and relative to the managed resource's
	'      class loader to ensure that the managed resource is in fact of
	'      the named class (or a subclass).  The class names in the sig
	'      array likewise have to be resolved, first against
	'      RequiredModelMBean's class loader, then against the managed
	'      resource's class loader.  There is no point in using any other
	'      loader because when we call Method.invoke we must call it on
	'      a Method that is implemented by the target object.
	'     
		Public Overridable Function invoke(ByVal opName As String, ByVal opArgs As Object(), ByVal sig As String()) As Object

			Dim tracing As Boolean = MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER)
			Const mth As String = "invoke(String, Object[], String[])"

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Entry")

			If opName Is Nothing Then
				Dim x As Exception = New System.ArgumentException("Method name must not be null")
				Throw New javax.management.RuntimeOperationsException(x, "An exception occurred while trying to " & "invoke a method on a RequiredModelMBean")
			End If

			Dim opClassName As String = Nothing
			Dim opMethodName As String

			' Parse for class name and method
			Dim opSplitter As Integer = opName.LastIndexOf(".")
			If opSplitter > 0 Then
				opClassName = opName.Substring(0,opSplitter)
				opMethodName = opName.Substring(opSplitter+1)
			Else
				opMethodName = opName
			End If

	'         Ignore anything after a left paren.  We keep this for
	'           compatibility but it isn't specified.  
			opSplitter = opMethodName.IndexOf("(")
			If opSplitter > 0 Then opMethodName = opMethodName.Substring(0,opSplitter)

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Finding operation " & opName & " as " & opMethodName)

			Dim opInfo As ModelMBeanOperationInfo = modelMBeanInfo.getOperation(opMethodName)
			If opInfo Is Nothing Then
				Dim msg As String = "Operation " & opName & " not in ModelMBeanInfo"
				Throw New javax.management.MBeanException(New javax.management.ServiceNotFoundException(msg), msg)
			End If

			Dim opDescr As javax.management.Descriptor = opInfo.descriptor
			If opDescr Is Nothing Then
				Const msg As String = "Operation descriptor null"
				Throw New javax.management.MBeanException(New javax.management.ServiceNotFoundException(msg), msg)
			End If

			Dim cached As Object = resolveForCacheValue(opDescr)
			If cached IsNot Nothing Then
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Returning cached value")
				Return cached
			End If

			If opClassName Is Nothing Then opClassName = CStr(opDescr.getFieldValue("class"))
			' may still be null now

			opMethodName = CStr(opDescr.getFieldValue("name"))
			If opMethodName Is Nothing Then
				Const msg As String = "Method descriptor must include `name' field"
				Throw New javax.management.MBeanException(New javax.management.ServiceNotFoundException(msg), msg)
			End If

			Dim targetTypeField As String = CStr(opDescr.getFieldValue("targetType"))
			If targetTypeField IsNot Nothing AndAlso (Not targetTypeField.ToUpper()) = "objectReference".ToUpper() Then
				Dim msg As String = "Target type must be objectReference: " & targetTypeField
				Throw New javax.management.MBeanException(New InvalidTargetObjectTypeException(msg), msg)
			End If

			Dim targetObjectField As Object = opDescr.getFieldValue("targetObject")
			If tracing AndAlso targetObjectField IsNot Nothing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Found target object in descriptor")

	'         Now look for the method, either in RequiredModelMBean itself
	'           or in the target object.  Set "method" and "targetObject"
	'           appropriately.  
			Dim method As Method
			Dim targetObject As Object

			method = findRMMBMethod(opMethodName, targetObjectField, opClassName, sig)

			If method IsNot Nothing Then
				targetObject = Me
			Else
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "looking for method in managedResource class")
				If targetObjectField IsNot Nothing Then
					targetObject = targetObjectField
				Else
					targetObject = managedResource
					If targetObject Is Nothing Then
						Dim msg As String = "managedResource for invoke " & opName & " is null"
						Dim snfe As Exception = New javax.management.ServiceNotFoundException(msg)
						Throw New javax.management.MBeanException(snfe)
					End If
				End If

				Dim targetClass As Type

				If opClassName IsNot Nothing Then
					Try
						Dim stack As java.security.AccessControlContext = java.security.AccessController.context
						Dim obj As Object = targetObject
						Dim className As String = opClassName
						Dim caughtException As ClassNotFoundException() = New ClassNotFoundException(0){}

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						targetClass = javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Class>()
	'					{
	'
	'						@Override public Class run()
	'						{
	'							try
	'							{
	'								ReflectUtil.checkPackageAccess(className);
	'								final ClassLoader targetClassLoader = obj.getClass().getClassLoader();
	'								Return Class.forName(className, False, targetClassLoader);
	'							}
	'							catch (ClassNotFoundException e)
	'							{
	'								caughtException[0] = e;
	'							}
	'							Return Nothing;
	'						}
	'					}, stack, acc);

						If caughtException(0) IsNot Nothing Then Throw caughtException(0)
					Catch e As ClassNotFoundException
						Dim msg As String = "class for invoke " & opName & " not found"
						Throw New javax.management.ReflectionException(e, msg)
					End Try
				Else
					targetClass = targetObject.GetType()
				End If

				method = resolveMethod(targetClass, opMethodName, sig)
			End If

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "found " & opMethodName & ", now invoking")

			Dim result As Object = invokeMethod(opName, method, targetObject, opArgs)

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "successfully invoked method")

			If result IsNot Nothing Then cacheResult(opInfo, opDescr, result)

			Return result
		End Function

		Private Function resolveMethod(ByVal targetClass As Type, ByVal opMethodName As String, ByVal sig As String()) As Method
			Dim tracing As Boolean = MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER)

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,"resolveMethod", "resolving " & targetClass.name & "." & opMethodName)

			Dim argClasses As Type()

			If sig Is Nothing Then
				argClasses = Nothing
			Else
				Dim stack As java.security.AccessControlContext = java.security.AccessController.context
				Dim caughtException As javax.management.ReflectionException() = New javax.management.ReflectionException(0){}
				Dim targetClassLoader As ClassLoader = targetClass.classLoader
				argClasses = New Type(sig.Length - 1){}

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Void>()
	'			{
	'
	'				@Override public Void run()
	'				{
	'					for (int i = 0; i < sig.length; i += 1)
	'					{
	'						if (tracing)
	'						{
	'							MODELMBEAN_LOGGER.logp(Level.FINER, RequiredModelMBean.class.getName(),"resolveMethod", "resolve type " + sig[i]);
	'						}
	'						argClasses[i] = (Class) primitiveClassMap.get(sig[i]);
	'						if (argClasses[i] == Nothing)
	'						{
	'							try
	'							{
	'								ReflectUtil.checkPackageAccess(sig[i]);
	'								argClasses[i] = Class.forName(sig[i], False, targetClassLoader);
	'							}
	'							catch (ClassNotFoundException e)
	'							{
	'								if (tracing)
	'								{
	'									MODELMBEAN_LOGGER.logp(Level.FINER, RequiredModelMBean.class.getName(), "resolveMethod", "class not found");
	'								}
	'								final String msg = "Parameter class not found";
	'								caughtException[0] = New ReflectionException(e, msg);
	'							}
	'						}
	'					}
	'					Return Nothing;
	'				}
	'			}, stack, acc);

				If caughtException(0) IsNot Nothing Then Throw caughtException(0)
			End If

			Try
				Return targetClass.GetMethod(opMethodName, argClasses)
			Catch e As NoSuchMethodException
				Dim msg As String = "Target method not found: " & targetClass.name & "." & opMethodName
				Throw New javax.management.ReflectionException(e, msg)
			End Try
		End Function

	'     Map e.g. "int" to int.class.  Goodness knows how many time this
	'       particular wheel has been reinvented.  
		Private Shared ReadOnly primitiveClasses As Type() = { GetType(Integer), GetType(Long), GetType(Boolean), GetType(Double), GetType(Single), GetType(Short), GetType(SByte), GetType(Char) }
		Private Shared ReadOnly primitiveClassMap As IDictionary(Of String, Type) = New Dictionary(Of String, Type)
		Shared Sub New()
			For i As Integer = 0 To primitiveClasses.Length - 1
				Dim c As Type = primitiveClasses(i)
				primitiveClassMap(c.name) = c
			Next i
			primitiveTypes = New String() { Boolean.TYPE.name, SByte.TYPE.name, Char.TYPE.name, Short.TYPE.name, Integer.TYPE.name, Long.TYPE.name, Single.TYPE.name, Double.TYPE.name, Void.TYPE.name }
			primitiveWrappers = New String() { GetType(Boolean).name, GetType(SByte?).name, GetType(Char?).name, GetType(Short).name, GetType(Integer).name, GetType(Long).name, GetType(Single?).name, GetType(Double).name, GetType(Void).name }
		End Sub

	'     Find a method in RequiredModelMBean as determined by the given
	'       parameters.  Return null if there is none, or if the parameters
	'       exclude using it.  Called from invoke. 
		Private Function findRMMBMethod(ByVal opMethodName As String, ByVal targetObjectField As Object, ByVal opClassName As String, ByVal sig As String()) As Method
			Dim tracing As Boolean = MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER)

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "invoke(String, Object[], String[])", "looking for method in RequiredModelMBean class")

			If Not isRMMBMethodName(opMethodName) Then Return Nothing
			If targetObjectField IsNot Nothing Then Return Nothing
			Dim rmmbClass As Type = GetType(RequiredModelMBean)
			Dim targetClass As Type
			If opClassName Is Nothing Then
				targetClass = rmmbClass
			Else
				Dim stack As java.security.AccessControlContext = java.security.AccessController.context
				Dim className As String = opClassName
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				targetClass = javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Class>()
	'			{
	'
	'				@Override public Class run()
	'				{
	'					try
	'					{
	'						ReflectUtil.checkPackageAccess(className);
	'						final ClassLoader targetClassLoader = rmmbClass.getClassLoader();
	'						Class clz = Class.forName(className, False, targetClassLoader);
	'						if (!rmmbClass.isAssignableFrom(clz))
	'							Return Nothing;
	'						Return clz;
	'					}
	'					catch (ClassNotFoundException e)
	'					{
	'						Return Nothing;
	'					}
	'				}
	'			}, stack, acc);
			End If
			Try
				Return If(targetClass IsNot Nothing, resolveMethod(targetClass, opMethodName, sig), Nothing)
			Catch e As javax.management.ReflectionException
				Return Nothing
			End Try
		End Function

	'    
	'     * Invoke the given method, and throw the somewhat unpredictable
	'     * appropriate exception if the method itself gets an exception.
	'     
		Private Function invokeMethod(ByVal opName As String, ByVal method As Method, ByVal targetObject As Object, ByVal opArgs As Object()) As Object
			Try
				Dim caughtException As Exception() = New Exception(0){}
				Dim stack As java.security.AccessControlContext = java.security.AccessController.context
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Object rslt = javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Object>()
	'			{
	'
	'				@Override public Object run()
	'				{
	'					try
	'					{
	'						ReflectUtil.checkPackageAccess(method.getDeclaringClass());
	'						Return MethodUtil.invoke(method, targetObject, opArgs);
	'					}
	'					catch (InvocationTargetException e)
	'					{
	'						caughtException[0] = e;
	'					}
	'					catch (IllegalAccessException e)
	'					{
	'						caughtException[0] = e;
	'					}
	'					Return Nothing;
	'				}
	'			}, stack, acc);
				If caughtException(0) IsNot Nothing Then
					If TypeOf caughtException(0) Is Exception Then
						Throw CType(caughtException(0), Exception)
					ElseIf TypeOf caughtException(0) Is Exception Then
						Throw CType(caughtException(0), [Error])
					End If
				End If
				Return rslt
			Catch ree As javax.management.RuntimeErrorException
				Throw New javax.management.RuntimeOperationsException(ree, "RuntimeException occurred in RequiredModelMBean " & "while trying to invoke operation " & opName)
			Catch re As Exception
				Throw New javax.management.RuntimeOperationsException(re, "RuntimeException occurred in RequiredModelMBean " & "while trying to invoke operation " & opName)
			Catch iae As IllegalAccessException
				Throw New javax.management.ReflectionException(iae, "IllegalAccessException occurred in " & "RequiredModelMBean while trying to " & "invoke operation " & opName)
			Catch ite As InvocationTargetException
				Dim mmbTargEx As Exception = ite.targetException
				If TypeOf mmbTargEx Is Exception Then
					Throw New javax.management.MBeanException(CType(mmbTargEx, Exception), "RuntimeException thrown in RequiredModelMBean " & "while trying to invoke operation " & opName)
				ElseIf TypeOf mmbTargEx Is Exception Then
					Throw New javax.management.RuntimeErrorException(CType(mmbTargEx, [Error]), "Error occurred in RequiredModelMBean while trying " & "to invoke operation " & opName)
				ElseIf TypeOf mmbTargEx Is javax.management.ReflectionException Then
					Throw CType(mmbTargEx, javax.management.ReflectionException)
				Else
					Throw New javax.management.MBeanException(CType(mmbTargEx, Exception), "Exception thrown in RequiredModelMBean " & "while trying to invoke operation " & opName)
				End If
			Catch err As Exception
				Throw New javax.management.RuntimeErrorException(err, "Error occurred in RequiredModelMBean while trying " & "to invoke operation " & opName)
			Catch e As Exception
				Throw New javax.management.ReflectionException(e, "Exception occurred in RequiredModelMBean while " & "trying to invoke operation " & opName)
			End Try
		End Function

	'    
	'     * Cache the result of an operation in the descriptor, if that is
	'     * called for by the descriptor's configuration.  Note that we
	'     * don't remember operation parameters when caching the result, so
	'     * this is unlikely to be useful if there are any.
	'     
		Private Sub cacheResult(ByVal opInfo As ModelMBeanOperationInfo, ByVal opDescr As javax.management.Descriptor, ByVal result As Object)

			Dim mmbDesc As javax.management.Descriptor = modelMBeanInfo.mBeanDescriptor

			Dim objctl As Object = opDescr.getFieldValue("currencyTimeLimit")
			Dim ctl As String
			If objctl IsNot Nothing Then
				ctl = objctl.ToString()
			Else
				ctl = Nothing
			End If
			If (ctl Is Nothing) AndAlso (mmbDesc IsNot Nothing) Then
				objctl = mmbDesc.getFieldValue("currencyTimeLimit")
				If objctl IsNot Nothing Then
					ctl = objctl.ToString()
				Else
					ctl = Nothing
				End If
			End If
			If (ctl IsNot Nothing) AndAlso Not(ctl.Equals("-1")) Then
				opDescr.fieldeld("value", result)
				opDescr.fieldeld("lastUpdatedTimeStamp", Convert.ToString((DateTime.Now).time))


				modelMBeanInfo.descriptortor(opDescr, "operation")
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "invoke(String,Object[],Object[])", "new descriptor is " & opDescr)
			End If
		End Sub

	'    
	'     * Determine whether the given name is the name of a public method
	'     * in this class.  This is only an optimization: it prevents us
	'     * from trying to do argument type lookups and reflection on a
	'     * method that will obviously fail because it has the wrong name.
	'     *
	'     * The first time this method is called we do the reflection, and
	'     * every other time we reuse the remembered values.
	'     *
	'     * It's conceivable that the (possibly malicious) first caller
	'     * doesn't have the required permissions to do reflection, in
	'     * which case we don't touch anything so as not to interfere
	'     * with a later permissionful caller.
	'     
		Private Shared rmmbMethodNames As java.util.Set(Of String)
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function isRMMBMethodName(ByVal name As String) As Boolean
			If rmmbMethodNames Is Nothing Then
				Try
					Dim names As java.util.Set(Of String) = New HashSet(Of String)
					Dim methods As Method() = GetType(RequiredModelMBean).methods
					For i As Integer = 0 To methods.Length - 1
						names.add(methods(i).name)
					Next i
					rmmbMethodNames = names
				Catch e As Exception
					Return True
					' This is only an optimization so we'll go on to discover
					' whether the name really is an RMMB method.
				End Try
			End If
			Return rmmbMethodNames.contains(name)
		End Function

		''' <summary>
		''' Returns the value of a specific attribute defined for this
		''' ModelMBean.
		''' The last value returned by an attribute may be cached in the
		''' attribute's descriptor.
		''' The valid value will be in the 'value' field if there is one.
		''' If the 'currencyTimeLimit' field in the descriptor is:
		''' <UL>
		''' <LI>  <b>&lt;0</b> Then the value is not cached and is never valid.
		'''       The getter method is invoked for the attribute.
		'''       The 'value' and 'lastUpdatedTimeStamp' fields are cleared.</LI>
		''' <LI>  <b>=0</b> Then the value is always cached and always valid.
		'''       The 'value' field is returned. If there is no'value' field
		'''       then the getter method is invoked for the attribute.
		'''       The 'lastUpdatedTimeStamp' field and `value' fields are set
		'''       to the attribute's value and the current time stamp.</LI>
		''' <LI>  <b>&gt;0</b> Represents the number of seconds that the 'value'
		'''       field is valid.
		'''       The 'value' field is no longer valid when
		'''       'lastUpdatedTimeStamp' + 'currencyTimeLimit' &gt; Now.
		'''   <UL>
		'''        <LI>When 'value' is valid, 'value' is returned.</LI>
		'''        <LI>When 'value' is no longer valid then the getter
		'''            method is invoked for the attribute.
		'''            The 'lastUpdatedTimeStamp' field and `value' fields
		'''            are updated.</LI>
		'''   </UL></LI>
		''' </UL>
		''' 
		''' <p><b>Note:</b> because of inconsistencies in previous versions of
		''' this specification, it is recommended not to use negative or zero
		''' values for <code>currencyTimeLimit</code>.  To indicate that a
		''' cached value is never valid, omit the
		''' <code>currencyTimeLimit</code> field.  To indicate that it is
		''' always valid, use a very large number for this field.</p>
		''' 
		''' <p>If the 'getMethod' field contains the name of a valid
		''' operation descriptor, then the method described by the
		''' operation descriptor is executed.  The response from the
		''' method is returned as the value of the attribute.  If the
		''' operation fails or the returned value is not compatible with
		''' the declared type of the attribute, an exception will be thrown.</p>
		''' 
		''' <p>If no 'getMethod' field is defined then the default value of the
		''' attribute is returned. If the returned value is not compatible with
		''' the declared type of the attribute, an exception will be thrown.</p>
		''' 
		''' <p>The declared type of the attribute is the String returned by
		''' <seealso cref="ModelMBeanAttributeInfo#getType()"/>.  A value is compatible
		''' with this type if one of the following is true:
		''' <ul>
		''' <li>the value is null;</li>
		''' <li>the declared name is a primitive type name (such as "int")
		'''     and the value is an instance of the corresponding wrapper
		'''     type (such as java.lang.Integer);</li>
		''' <li>the name of the value's class is identical to the declared name;</li>
		''' <li>the declared name can be loaded by the value's class loader and
		'''     produces a class to which the value can be assigned.</li>
		''' </ul>
		''' 
		''' <p>In this implementation, in every case where the getMethod needs to
		''' be called, because the method is invoked through the standard "invoke"
		''' method and thus needs operationInfo, an operation must be specified
		''' for that getMethod so that the invocation works correctly.</p>
		''' </summary>
		''' <param name="attrName"> A String specifying the name of the
		''' attribute to be retrieved. It must match the name of a
		''' ModelMBeanAttributeInfo.
		''' </param>
		''' <returns> The value of the retrieved attribute from the
		''' descriptor 'value' field or from the invocation of the
		''' operation in the 'getMethod' field of the descriptor.
		''' </returns>
		''' <exception cref="AttributeNotFoundException"> The specified attribute is
		'''    not accessible in the MBean.
		'''    The following cases may result in an AttributeNotFoundException:
		'''    <UL>
		'''      <LI> No ModelMBeanInfo was found for the Model MBean.</LI>
		'''      <LI> No ModelMBeanAttributeInfo was found for the specified
		'''           attribute name.</LI>
		'''      <LI> The ModelMBeanAttributeInfo isReadable method returns
		'''           'false'.</LI>
		'''    </UL> </exception>
		''' <exception cref="MBeanException">  Wraps one of the following Exceptions:
		'''    <UL>
		'''      <LI> <seealso cref="InvalidAttributeValueException"/>: A wrong value type
		'''           was received from the attribute's getter method or
		'''           no 'getMethod' field defined in the descriptor for
		'''           the attribute and no default value exists.</LI>
		'''      <LI> <seealso cref="ServiceNotFoundException"/>: No
		'''           ModelMBeanOperationInfo defined for the attribute's
		'''           getter method or no descriptor associated with the
		'''           ModelMBeanOperationInfo or the managed resource is
		'''           null.</LI>
		'''      <LI> <seealso cref="InvalidTargetObjectTypeException"/> The 'targetType'
		'''           field value is not 'objectReference'.</LI>
		'''      <LI> An Exception thrown by the managed object's getter.</LI>
		'''    </UL> </exception>
		''' <exception cref="ReflectionException">  Wraps an <seealso cref="java.lang.Exception"/>
		'''    thrown while trying to invoke the getter. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		'''    <seealso cref="IllegalArgumentException"/>: The attribute name in
		'''    parameter is null.
		''' </exception>
		''' <seealso cref= #setAttribute(javax.management.Attribute)
		'''  </seealso>
		Public Overridable Function getAttribute(ByVal attrName As String) As Object Implements DynamicMBean.getAttribute
			If attrName Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("attributeName must not be null"), "Exception occurred trying to get attribute of a " & "RequiredModelMBean")
			Const mth As String = "getAttribute(String)"
			Dim tracing As Boolean = MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER)
			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Entry with " & attrName)

			' Check attributeDescriptor for getMethod 
			Dim response As Object

			Try
				If modelMBeanInfo Is Nothing Then Throw New javax.management.AttributeNotFoundException("getAttribute failed: ModelMBeanInfo not found for " & attrName)

				Dim attrInfo As ModelMBeanAttributeInfo = modelMBeanInfo.getAttribute(attrName)
				Dim mmbDesc As javax.management.Descriptor = modelMBeanInfo.mBeanDescriptor

				If attrInfo Is Nothing Then Throw New javax.management.AttributeNotFoundException("getAttribute failed:" & " ModelMBeanAttributeInfo not found for " & attrName)

				Dim attrDescr As javax.management.Descriptor = attrInfo.descriptor
				If attrDescr IsNot Nothing Then
					If Not attrInfo.readable Then Throw New javax.management.AttributeNotFoundException("getAttribute failed: " & attrName & " is not readable ")

					response = resolveForCacheValue(attrDescr)

					' return current cached value 
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "*** cached value is " & response)

					If response Is Nothing Then
						' no cached value, run getMethod 
						If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "**** cached value is null - getting getMethod")
						Dim attrGetMethod As String = CStr(attrDescr.getFieldValue("getMethod"))

						If attrGetMethod IsNot Nothing Then
							' run method from operations descriptor 
							If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "invoking a getMethod for " & attrName)

							Dim getResponse As Object = invoke(attrGetMethod, New Object() {}, New String() {})

							If getResponse IsNot Nothing Then
								' error/validity check return value here
								If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "got a non-null response " & "from getMethod" & vbLf)

								response = getResponse

								' change cached value in attribute descriptor
								Dim objctl As Object = attrDescr.getFieldValue("currencyTimeLimit")

								Dim ctl As String
								If objctl IsNot Nothing Then
									ctl = objctl.ToString()
								Else
									ctl = Nothing
								End If

								If (ctl Is Nothing) AndAlso (mmbDesc IsNot Nothing) Then
									objctl = mmbDesc.getFieldValue("currencyTimeLimit")
									If objctl IsNot Nothing Then
										ctl = objctl.ToString()
									Else
										ctl = Nothing
									End If
								End If

								If (ctl IsNot Nothing) AndAlso Not(ctl.Equals("-1")) Then
									If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "setting cached value and " & "lastUpdatedTime in descriptor")
									attrDescr.fieldeld("value", response)
									Dim stamp As String = Convert.ToString((DateTime.Now).time)
									attrDescr.fieldeld("lastUpdatedTimeStamp", stamp)
									attrInfo.descriptor = attrDescr
									modelMBeanInfo.descriptortor(attrDescr, "attribute")
									If tracing Then
										MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth,"new descriptor is " & attrDescr)
										MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth,"AttributeInfo descriptor is " & attrInfo.descriptor)
										Dim attStr As String = modelMBeanInfo.getDescriptor(attrName,"attribute").ToString()
										MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "modelMBeanInfo: AttributeInfo " & "descriptor is " & attStr)
									End If
								End If
							Else
								' response was invalid or really returned null
								If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "got a null response from getMethod" & vbLf)
								response = Nothing
							End If
						Else
							' not getMethod so return descriptor (default) value
							Dim qualifier As String=""
							response = attrDescr.getFieldValue("value")
							If response Is Nothing Then
								qualifier="default "
								response = attrDescr.getFieldValue("default")
							End If
							If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "could not find getMethod for " & attrName & ", returning descriptor " & qualifier & "value")
							' !! cast response to right class
						End If
					End If

					' make sure response class matches type field
					Dim respType As String = attrInfo.type
					If response IsNot Nothing Then
						Dim responseClass As String = response.GetType().name
						If Not respType.Equals(responseClass) Then
							Dim wrongType As Boolean = False
							Dim primitiveType As Boolean = False
							Dim correspondingTypes As Boolean = False
							For i As Integer = 0 To primitiveTypes.Length - 1
								If respType.Equals(primitiveTypes(i)) Then
									primitiveType = True
									If responseClass.Equals(primitiveWrappers(i)) Then correspondingTypes = True
									Exit For
								End If
							Next i
							If primitiveType Then
								' inequality may come from primitive/wrapper class
								If Not correspondingTypes Then wrongType = True
							Else
								' inequality may come from type subclassing
								Dim subtype As Boolean
								Try
									Dim respClass As Type = response.GetType()
									Dim caughException As Exception() = New Exception(0){}

									Dim stack As java.security.AccessControlContext = java.security.AccessController.context

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'									Class c = javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Class>()
	'								{
	'
	'									@Override public Class run()
	'									{
	'										try
	'										{
	'											ReflectUtil.checkPackageAccess(respType);
	'											ClassLoader cl = respClass.getClassLoader();
	'											Return Class.forName(respType, True, cl);
	'										}
	'										catch (Exception e)
	'										{
	'											caughException[0] = e;
	'										}
	'										Return Nothing;
	'									}
	'								}, stack, acc);

									If caughException(0) IsNot Nothing Then Throw caughException(0)

									subtype = c.isInstance(response)
								Catch e As Exception
									subtype = False

									If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Exception: ",e)
								End Try
								If Not subtype Then wrongType = True
							End If
							If wrongType Then
								If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Wrong response type '" & respType & "'")
								' throw exception, didn't get
								' back right attribute type
								Throw New javax.management.MBeanException(New javax.management.InvalidAttributeValueException("Wrong value type received for get attribute"), "An exception occurred while trying to get an " & "attribute value through a RequiredModelMBean")
							End If
						End If
					End If
				Else
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "getMethod failed " & attrName & " not in attributeDescriptor" & vbLf)
					Throw New javax.management.MBeanException(New javax.management.InvalidAttributeValueException("Unable to resolve attribute value, " & "no getMethod defined in descriptor for attribute"), "An exception occurred while trying to get an " & "attribute value through a RequiredModelMBean")
				End If

			Catch mbe As javax.management.MBeanException
				Throw mbe
			Catch t As javax.management.AttributeNotFoundException
				Throw t
			Catch e As Exception
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "getMethod failed with " & e.Message & " exception type " & (e.GetType()).ToString())
				Throw New javax.management.MBeanException(e,"An exception occurred while trying " & "to get an attribute value: " & e.Message)
			End Try

			If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Exit")

			Return response
		End Function

		''' <summary>
		''' Returns the values of several attributes in the ModelMBean.
		''' Executes a getAttribute for each attribute name in the
		''' attrNames array passed in.
		''' </summary>
		''' <param name="attrNames"> A String array of names of the attributes
		''' to be retrieved.
		''' </param>
		''' <returns> The array of the retrieved attributes.
		''' </returns>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		''' <seealso cref="IllegalArgumentException"/>: The object name in parameter is
		''' null or attributes in parameter is null.
		''' </exception>
		''' <seealso cref= #setAttributes(javax.management.AttributeList) </seealso>
		Public Overridable Function getAttributes(ByVal attrNames As String()) As javax.management.AttributeList
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getAttributes(String[])","Entry")

			If attrNames Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("attributeNames must not be null"), "Exception occurred trying to get attributes of a " & "RequiredModelMBean")

			Dim responseList As New javax.management.AttributeList
			For i As Integer = 0 To attrNames.Length - 1
				Try
					responseList.Add(New javax.management.Attribute(attrNames(i), getAttribute(attrNames(i))))
				Catch e As Exception
					' eat exceptions because interface doesn't have an
					' exception on it
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getAttributes(String[])", "Failed to get """ & attrNames(i) & """: ", e)
				End Try
			Next i

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getAttributes(String[])","Exit")

			Return responseList
		End Function

		''' <summary>
		''' Sets the value of a specific attribute of a named ModelMBean.
		''' 
		''' If the 'setMethod' field of the attribute's descriptor
		''' contains the name of a valid operation descriptor, then the
		''' method described by the operation descriptor is executed.
		''' In this implementation, the operation descriptor must be specified
		''' correctly and assigned to the modelMBeanInfo so that the 'setMethod'
		''' works correctly.
		''' The response from the method is set as the value of the attribute
		''' in the descriptor.
		''' 
		''' <p>If currencyTimeLimit is &gt; 0, then the new value for the
		''' attribute is cached in the attribute descriptor's
		''' 'value' field and the 'lastUpdatedTimeStamp' field is set to
		''' the current time stamp.
		''' 
		''' <p>If the persist field of the attribute's descriptor is not null
		''' then Persistence policy from the attribute descriptor is used to
		''' guide storing the attribute in a persistent store.
		''' <br>Store the MBean if 'persistPolicy' field is:
		''' <UL>
		''' <Li> != "never"</Li>
		''' <Li> = "always"</Li>
		''' <Li> = "onUpdate"</Li>
		''' <Li> {@literal = "onTimer" and now > 'lastPersistTime' + 'persistPeriod'}</Li>
		''' <Li> {@literal = "NoMoreOftenThan" and now > 'lastPersistTime' +
		'''         'persistPeriod'}</Li>
		''' </UL>
		''' Do not store the MBean if 'persistPolicy' field is:
		''' <UL>
		''' <Li> = "never"</Li>
		''' <Li> = {@literal = "onTimer" && now < 'lastPersistTime' + 'persistPeriod'}</Li>
		''' <Li> = "onUnregister"</Li>
		''' <Li> = {@literal = "NoMoreOftenThan" and now < 'lastPersistTime' +
		'''        'persistPeriod'}</Li>
		''' </UL>
		''' 
		''' <p>The ModelMBeanInfo of the Model MBean is stored in a file.
		''' </summary>
		''' <param name="attribute"> The Attribute instance containing the name of
		'''        the attribute to be set and the value it is to be set to.
		''' 
		''' </param>
		''' <exception cref="AttributeNotFoundException"> The specified attribute is
		'''   not accessible in the MBean.
		'''   <br>The following cases may result in an AttributeNotFoundException:
		'''   <UL>
		'''     <LI> No ModelMBeanAttributeInfo is found for the specified
		'''          attribute.</LI>
		'''     <LI> The ModelMBeanAttributeInfo's isWritable method returns
		'''          'false'.</LI>
		'''   </UL> </exception>
		''' <exception cref="InvalidAttributeValueException"> No descriptor is defined
		'''   for the specified attribute. </exception>
		''' <exception cref="MBeanException"> Wraps one of the following Exceptions:
		'''   <UL>
		'''     <LI> An Exception thrown by the managed object's setter.</LI>
		'''     <LI> A <seealso cref="ServiceNotFoundException"/> if a setMethod field is
		'''          defined in the descriptor for the attribute and the managed
		'''          resource is null; or if no setMethod field is defined and
		'''          caching is not enabled for the attribute.
		'''          Note that if there is no getMethod field either, then caching
		'''          is automatically enabled.</LI>
		'''     <LI> <seealso cref="InvalidTargetObjectTypeException"/> The 'targetType'
		'''          field value is not 'objectReference'.</LI>
		'''     <LI> An Exception thrown by the managed object's getter.</LI>
		'''   </UL> </exception>
		''' <exception cref="ReflectionException">  Wraps an <seealso cref="java.lang.Exception"/>
		'''   thrown while trying to invoke the setter. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		'''   <seealso cref="IllegalArgumentException"/>: The attribute in parameter is
		'''   null.
		''' </exception>
		''' <seealso cref= #getAttribute(java.lang.String)
		'''  </seealso>
		Public Overridable Property attribute As javax.management.Attribute
			Set(ByVal attribute As javax.management.Attribute)
				Dim tracing As Boolean = MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER)
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute()","Entry")
    
				If attribute Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("attribute must not be null"), "Exception occurred trying to set an attribute of a " & "RequiredModelMBean")
    
				' run setMethod if there is one 
				' return cached value if its current 
				' set cached value in descriptor and set date/time 
				' send attribute change Notification 
				' check persistence policy and persist if need be 
				Dim attrName As String = attribute.name
				Dim attrValue As Object = attribute.value
				Dim updateDescriptor As Boolean = False
    
				Dim attrInfo As ModelMBeanAttributeInfo = modelMBeanInfo.getAttribute(attrName)
    
				If attrInfo Is Nothing Then Throw New javax.management.AttributeNotFoundException("setAttribute failed: " & attrName & " is not found ")
    
				Dim mmbDesc As javax.management.Descriptor = modelMBeanInfo.mBeanDescriptor
				Dim attrDescr As javax.management.Descriptor = attrInfo.descriptor
    
				If attrDescr IsNot Nothing Then
					If Not attrInfo.writable Then Throw New javax.management.AttributeNotFoundException("setAttribute failed: " & attrName & " is not writable ")
    
					Dim attrSetMethod As String = CStr(attrDescr.getFieldValue("setMethod"))
					Dim attrGetMethod As String = CStr(attrDescr.getFieldValue("getMethod"))
    
					Dim attrType As String = attrInfo.type
					Dim currValue As Object = "Unknown"
    
					Try
						currValue = Me.getAttribute(attrName)
					Catch t As Exception
						' OK: Default "Unknown" value used for unknown attribute
					End Try
    
					Dim oldAttr As New javax.management.Attribute(attrName, currValue)
    
					' run method from operations descriptor 
					If attrSetMethod Is Nothing Then
						If attrValue IsNot Nothing Then
							Try
								Dim clazz As Type = loadClass(attrType)
								If Not clazz.IsInstanceOfType(attrValue) Then Throw New javax.management.InvalidAttributeValueException(clazz.name & " expected, " & attrValue.GetType().name & " received.")
							Catch x As ClassNotFoundException
								If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)","Class " & attrType & " for attribute " & attrName & " not found: ", x)
							End Try
						End If
						updateDescriptor = True
					Else
						invoke(attrSetMethod, (New Object() {attrValue}), (New String() {attrType}))
					End If
    
					' change cached value 
					Dim objctl As Object = attrDescr.getFieldValue("currencyTimeLimit")
					Dim ctl As String
					If objctl IsNot Nothing Then
						ctl = objctl.ToString()
					Else
						ctl = Nothing
					End If
    
					If (ctl Is Nothing) AndAlso (mmbDesc IsNot Nothing) Then
						objctl = mmbDesc.getFieldValue("currencyTimeLimit")
						If objctl IsNot Nothing Then
							ctl = objctl.ToString()
						Else
							ctl = Nothing
						End If
					End If
    
					Dim updateCache As Boolean = ((ctl IsNot Nothing) AndAlso Not(ctl.Equals("-1")))
    
					 If attrSetMethod Is Nothing AndAlso (Not updateCache) AndAlso attrGetMethod IsNot Nothing Then Throw New javax.management.MBeanException(New javax.management.ServiceNotFoundException("No " & "setMethod field is defined in the descriptor for " & attrName & " attribute and caching is not enabled " & "for it"))
    
					If updateCache OrElse updateDescriptor Then
						If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)", "setting cached value of " & attrName & " to " & attrValue)
    
						attrDescr.fieldeld("value", attrValue)
    
						If updateCache Then
							Dim currtime As String = Convert.ToString((DateTime.Now).time)
    
							attrDescr.fieldeld("lastUpdatedTimeStamp", currtime)
						End If
    
						attrInfo.descriptor = attrDescr
    
						modelMBeanInfo.descriptortor(attrDescr,"attribute")
						If tracing Then
							Dim strb As (New StringBuilder).Append("new descriptor is ").append(attrDescr).append(". AttributeInfo descriptor is ").append(attrInfo.descriptor).append(". AttributeInfo descriptor is ").append(modelMBeanInfo.getDescriptor(attrName,"attribute"))
							MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)",strb.ToString())
						End If
    
					End If
    
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)","sending sendAttributeNotification")
					sendAttributeChangeNotification(oldAttr,attribute)
	 ' if descriptor ... else no descriptor
				Else
    
					If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)","setMethod failed " & attrName & " not in attributeDescriptor" & vbLf)
    
					Throw New javax.management.InvalidAttributeValueException("Unable to resolve attribute value, " & "no defined in descriptor for attribute")
				End If ' else no descriptor
    
				If tracing Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)", "Exit")
    
			End Set
		End Property

		''' <summary>
		''' Sets the values of an array of attributes of this ModelMBean.
		''' Executes the setAttribute() method for each attribute in the list.
		''' </summary>
		''' <param name="attributes"> A list of attributes: The identification of the
		''' attributes to be set and  the values they are to be set to.
		''' </param>
		''' <returns>  The array of attributes that were set, with their new
		'''    values in Attribute instances.
		''' </returns>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		'''   <seealso cref="IllegalArgumentException"/>: The object name in parameter
		'''   is null or attributes in parameter is null.
		''' </exception>
		''' <seealso cref= #getAttributes
		'''  </seealso>
		Public Overridable Function setAttributes(ByVal attributes As javax.management.AttributeList) As javax.management.AttributeList

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "setAttribute(Attribute)", "Entry")

			If attributes Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("attributes must not be null"), "Exception occurred trying to set attributes of a " & "RequiredModelMBean")

			Dim responseList As New javax.management.AttributeList

			' Go through the list of attributes
			For Each attr As javax.management.Attribute In attributes.asList()
				Try
					attribute = attr
					responseList.Add(attr)
				Catch excep As Exception
					responseList.Remove(attr)
				End Try
			Next attr

			Return responseList
		End Function



		Private Function createDefaultModelMBeanInfo() As ModelMBeanInfo
			Return (New ModelMBeanInfoSupport((Me.GetType().name), "Default ModelMBean", Nothing, Nothing, Nothing, Nothing))
		End Function

		''' <summary>
		'''********************************** </summary>
		' NotificationBroadcaster Interface 
		''' <summary>
		'''********************************** </summary>


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeToLog(ByVal logFileName As String, ByVal logEntry As String)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "writeToLog(String, String)", "Notification Logging to " & logFileName & ": " & logEntry)
			If (logFileName Is Nothing) OrElse (logEntry Is Nothing) Then
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "writeToLog(String, String)", "Bad input parameters, will not log this entry.")
				Return
			End If

			Dim fos As New java.io.FileOutputStream(logFileName, True)
			Try
				Dim logOut As New java.io.PrintStream(fos)
				logOut.println(logEntry)
				logOut.close()
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "writeToLog(String, String)","Successfully opened log " & logFileName)
			Catch e As Exception
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "writeToLog(String, String)", "Exception " & e.ToString() & " trying to write to the Notification log file " & logFileName)
				Throw e
			Finally
				fos.close()
			End Try
		End Sub


		''' <summary>
		''' Registers an object which implements the NotificationListener
		''' interface as a listener.  This
		''' object's 'handleNotification()' method will be invoked when any
		''' notification is issued through or by the ModelMBean.  This does
		''' not include attributeChangeNotifications.  They must be registered
		''' for independently.
		''' </summary>
		''' <param name="listener"> The listener object which will handles
		'''        notifications emitted by the registered MBean. </param>
		''' <param name="filter"> The filter object. If null, no filtering will be
		'''        performed before handling notifications. </param>
		''' <param name="handback"> The context to be sent to the listener with
		'''        the notification when a notification is emitted.
		''' </param>
		''' <exception cref="IllegalArgumentException"> The listener cannot be null.
		''' </exception>
		''' <seealso cref= #removeNotificationListener </seealso>
		Public Overridable Sub addNotificationListener(ByVal listener As javax.management.NotificationListener, ByVal filter As javax.management.NotificationFilter, ByVal handback As Object)
			Dim mth As String = "addNotificationListener(" & "NotificationListener, NotificationFilter, Object)"
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Entry")

			If listener Is Nothing Then Throw New System.ArgumentException("notification listener must not be null")

			If generalBroadcaster Is Nothing Then generalBroadcaster = New javax.management.NotificationBroadcasterSupport

			generalBroadcaster.addNotificationListener(listener, filter, handback)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "NotificationListener added")
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Exit")
			End If
		End Sub

		''' <summary>
		''' Removes a listener for Notifications from the RequiredModelMBean.
		''' </summary>
		''' <param name="listener"> The listener name which was handling notifications
		'''    emitted by the registered MBean.
		'''    This method will remove all information related to this listener.
		''' </param>
		''' <exception cref="ListenerNotFoundException"> The listener is not registered
		'''    in the MBean or is null.
		''' </exception>
		''' <seealso cref= #addNotificationListener
		'''  </seealso>
		Public Overridable Sub removeNotificationListener(ByVal listener As javax.management.NotificationListener)
			If listener Is Nothing Then Throw New javax.management.ListenerNotFoundException("Notification listener is null")

			Const mth As String="removeNotificationListener(NotificationListener)"
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Entry")

			If generalBroadcaster Is Nothing Then Throw New javax.management.ListenerNotFoundException("No notification listeners registered")


			generalBroadcaster.removeNotificationListener(listener)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Exit")

		End Sub

		Public Overridable Sub removeNotificationListener(ByVal listener As javax.management.NotificationListener, ByVal filter As javax.management.NotificationFilter, ByVal handback As Object)

			If listener Is Nothing Then Throw New javax.management.ListenerNotFoundException("Notification listener is null")

			Dim mth As String = "removeNotificationListener(" & "NotificationListener, NotificationFilter, Object)"

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Entry")

			If generalBroadcaster Is Nothing Then Throw New javax.management.ListenerNotFoundException("No notification listeners registered")


			generalBroadcaster.removeNotificationListener(listener,filter, handback)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Exit")

		End Sub

		Public Overridable Sub sendNotification(ByVal ntfyObj As javax.management.Notification) Implements ModelMBean.sendNotification
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "sendNotification(Notification)", "Entry")

			If ntfyObj Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("notification object must not be " & "null"), "Exception occurred trying to send a notification from a " & "RequiredModelMBean")


			' log notification if specified in descriptor
			Dim ntfyDesc As javax.management.Descriptor = modelMBeanInfo.getDescriptor(ntfyObj.type,"notification")
			Dim mmbDesc As javax.management.Descriptor = modelMBeanInfo.mBeanDescriptor

			If ntfyDesc IsNot Nothing Then
				Dim logging As String = CStr(ntfyDesc.getFieldValue("log"))

				If logging Is Nothing Then
					If mmbDesc IsNot Nothing Then logging = CStr(mmbDesc.getFieldValue("log"))
				End If

				If (logging IsNot Nothing) AndAlso (logging.ToUpper() = "t".ToUpper() OrElse logging.ToUpper() = "true".ToUpper()) Then

					Dim logfile As String = CStr(ntfyDesc.getFieldValue("logfile"))
					If logfile Is Nothing Then
						If mmbDesc IsNot Nothing Then logfile = CStr(mmbDesc.getFieldValue("logfile"))
					End If
					If logfile IsNot Nothing Then
						Try
							writeToLog(logfile,"LogMsg: " & ((New DateTime(ntfyObj.timeStamp)).ToString()) & " " & ntfyObj.type & " " & ntfyObj.message & " Severity = " & CStr(ntfyDesc.getFieldValue("severity")))
						Catch e As Exception
							If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINE) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINE, GetType(RequiredModelMBean).name, "sendNotification(Notification)", "Failed to log " & ntfyObj.type & " notification: ", e)
						End Try
					End If
				End If
			End If
			If generalBroadcaster IsNot Nothing Then generalBroadcaster.sendNotification(ntfyObj)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "sendNotification(Notification)", "sendNotification sent provided notification object")
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "sendNotification(Notification)"," Exit")
			End If

		End Sub


		Public Overridable Sub sendNotification(ByVal ntfyText As String) Implements ModelMBean.sendNotification
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "sendNotification(String)","Entry")

			If ntfyText Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("notification message must not " & "be null"), "Exception occurred trying to send a text notification " & "from a ModelMBean")

			Dim myNtfyObj As New javax.management.Notification("jmx.modelmbean.generic", Me, 1, ntfyText)
			sendNotification(myNtfyObj)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "sendNotification(String)","Notification sent")
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "sendNotification(String)","Exit")
			End If
		End Sub

		''' <summary>
		''' Returns `true' if the notification `notifName' is found
		''' in `info'. (bug 4744667)
		''' 
		''' </summary>
		Private Shared Function hasNotification(ByVal info As ModelMBeanInfo, ByVal notifName As String) As Boolean
			Try
				If info Is Nothing Then
					Return False
				Else
					Return (info.getNotification(notifName) IsNot Nothing)
				End If
			Catch x As javax.management.MBeanException
				Return False
			Catch r As javax.management.RuntimeOperationsException
				Return False
			End Try
		End Function

		''' <summary>
		''' Creates a default ModelMBeanNotificationInfo for GENERIC
		''' notification.  (bug 4744667)
		''' 
		''' </summary>
		Private Shared Function makeGenericInfo() As ModelMBeanNotificationInfo
			Dim genericDescriptor As javax.management.Descriptor = New DescriptorSupport(New String() { "name=GENERIC", "descriptorType=notification", "log=T", "severity=6", "displayName=jmx.modelmbean.generic"})

			Return New ModelMBeanNotificationInfo(New String() {"jmx.modelmbean.generic"}, "GENERIC", "A text notification has been issued by the managed resource", genericDescriptor)
		End Function

		''' <summary>
		''' Creates a default ModelMBeanNotificationInfo for ATTRIBUTE_CHANGE
		''' notification.  (bug 4744667)
		''' 
		''' </summary>
		Private Shared Function makeAttributeChangeInfo() As ModelMBeanNotificationInfo
			Dim attributeDescriptor As javax.management.Descriptor = New DescriptorSupport(New String() { "name=ATTRIBUTE_CHANGE", "descriptorType=notification", "log=T", "severity=6", "displayName=jmx.attribute.change"})

			Return New ModelMBeanNotificationInfo(New String() {"jmx.attribute.change"}, "ATTRIBUTE_CHANGE", "Signifies that an observed MBean attribute value has changed", attributeDescriptor)
		End Function

		''' <summary>
		''' Returns the array of Notifications always generated by the
		''' RequiredModelMBean.
		''' <P>
		''' 
		''' RequiredModelMBean may always send also two additional notifications:
		''' <UL>
		'''   <LI> One with descriptor <code>"name=GENERIC,descriptorType=notification,log=T,severity=6,displayName=jmx.modelmbean.generic"</code></LI>
		'''   <LI> Second is a standard attribute change notification
		'''        with descriptor <code>"name=ATTRIBUTE_CHANGE,descriptorType=notification,log=T,severity=6,displayName=jmx.attribute.change"</code></LI>
		''' </UL>
		''' Thus these two notifications are always added to those specified
		''' by the application.
		''' </summary>
		''' <returns> MBeanNotificationInfo[]
		''' 
		'''  </returns>
		Public Overridable Property notificationInfo As javax.management.MBeanNotificationInfo()
			Get
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getNotificationInfo()","Entry")
    
				' Using hasNotification() is not optimal, but shouldn't really
				' matter in this context...
    
				' hasGeneric==true if GENERIC notification is present.
				' (bug 4744667)
				Dim hasGeneric As Boolean = hasNotification(modelMBeanInfo,"GENERIC")
    
				' hasAttributeChange==true if ATTRIBUTE_CHANGE notification is
				' present.
				' (bug 4744667)
				Dim hasAttributeChange As Boolean = hasNotification(modelMBeanInfo,"ATTRIBUTE_CHANGE")
    
				' User supplied list of notification infos.
				'
				Dim currInfo As ModelMBeanNotificationInfo() = CType(modelMBeanInfo.notifications, ModelMBeanNotificationInfo())
    
				' Length of the returned list of notification infos:
				'    length of user suplied list + possibly 1 for GENERIC, +
				'    possibly 1 for ATTRIBUTE_CHANGE
				'    (bug 4744667)
				Dim len As Integer = ((If(currInfo Is Nothing, 0, currInfo.Length)) + (If(hasGeneric, 0, 1)) + (If(hasAttributeChange, 0, 1)))
    
				' Returned list of notification infos:
				'
				Dim respInfo As ModelMBeanNotificationInfo() = New ModelMBeanNotificationInfo(len - 1){}
    
				' Preserve previous ordering (JMX 1.1)
				'
    
				' Counter of "standard" notification inserted before user
				' supplied notifications.
				'
				Dim inserted As Integer=0
				If Not hasGeneric Then
					' We need to add description for GENERIC notification
					' (bug 4744667)
					respInfo(inserted) = makeGenericInfo()
					inserted += 1
				End If
    
    
				If Not hasAttributeChange Then
					' We need to add description for ATTRIBUTE_CHANGE notification
					' (bug 4744667)
					respInfo(inserted) = makeAttributeChangeInfo()
					inserted += 1
				End If
    
				' Now copy user supplied list in returned list.
				'
				Dim count As Integer = currInfo.Length
				Dim offset As Integer = inserted
				For j As Integer = 0 To count - 1
					respInfo(offset+j) = currInfo(j)
				Next j
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, "getNotificationInfo()","Exit")
    
				Return respInfo
			End Get
		End Property


		Public Overridable Sub addAttributeChangeNotificationListener(ByVal inlistener As javax.management.NotificationListener, ByVal inAttributeName As String, ByVal inhandback As Object) Implements ModelMBean.addAttributeChangeNotificationListener
			Dim mth As String="addAttributeChangeNotificationListener(" & "NotificationListener, String, Object)"

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Entry")

			If inlistener Is Nothing Then Throw New System.ArgumentException("Listener to be registered must not be null")


			If attributeBroadcaster Is Nothing Then attributeBroadcaster = New javax.management.NotificationBroadcasterSupport

			Dim currFilter As New javax.management.AttributeChangeNotificationFilter

			Dim attrInfo As javax.management.MBeanAttributeInfo() = modelMBeanInfo.attributes
			Dim found As Boolean = False
			If inAttributeName Is Nothing Then
				If (attrInfo IsNot Nothing) AndAlso (attrInfo.Length>0) Then
					For i As Integer = 0 To attrInfo.Length - 1
						currFilter.enableAttribute(attrInfo(i).name)
					Next i
				End If
			Else
				If (attrInfo IsNot Nothing) AndAlso (attrInfo.Length>0) Then
					For i As Integer = 0 To attrInfo.Length - 1
						If inAttributeName.Equals(attrInfo(i).name) Then
							found = True
							currFilter.enableAttribute(inAttributeName)
							Exit For
						End If
					Next i
				End If
				If Not found Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("The attribute name does not exist"), "Exception occurred trying to add an " & "AttributeChangeNotification listener")
			End If

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				Dim enabledAttrs As List(Of String) = currFilter.enabledAttributes
				Dim s As String = If(enabledAttrs.Count > 1, "[" & enabledAttrs(0) & ", ...]", enabledAttrs.ToString())
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name, mth, "Set attribute change filter to " & s)
			End If

			attributeBroadcaster.addNotificationListener(inlistener,currFilter, inhandback)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "Notification listener added for " & inAttributeName)
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Exit")
			End If
		End Sub

		Public Overridable Sub removeAttributeChangeNotificationListener(ByVal inlistener As javax.management.NotificationListener, ByVal inAttributeName As String) Implements ModelMBean.removeAttributeChangeNotificationListener
			If inlistener Is Nothing Then Throw New javax.management.ListenerNotFoundException("Notification listener is null")

			Dim mth As String = "removeAttributeChangeNotificationListener(" & "NotificationListener, String)"

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Entry")


			If attributeBroadcaster Is Nothing Then Throw New javax.management.ListenerNotFoundException("No attribute change notification listeners registered")


			Dim attrInfo As javax.management.MBeanAttributeInfo() = modelMBeanInfo.attributes
			Dim found As Boolean = False
			If (attrInfo IsNot Nothing) AndAlso (attrInfo.Length>0) Then
				For i As Integer = 0 To attrInfo.Length - 1
					If attrInfo(i).name.Equals(inAttributeName) Then
						found = True
						Exit For
					End If
				Next i
			End If

			If ((Not found)) AndAlso (inAttributeName IsNot Nothing) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid attribute name"), "Exception occurred trying to remove " & "attribute change notification listener")

			' note: 
	'         this may be a problem if the same listener is registered for
	'           multiple attributes with multiple filters and/or handback
	'           objects.  It may remove all of them 

			attributeBroadcaster.removeNotificationListener(inlistener)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Exit")
		End Sub

		Public Overridable Sub sendAttributeChangeNotification(ByVal ntfyObj As javax.management.AttributeChangeNotification) Implements ModelMBean.sendAttributeChangeNotification
			Dim mth As String = "sendAttributeChangeNotification(" & "AttributeChangeNotification)"

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth,"Entry")

			If ntfyObj Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("attribute change notification object must not be null"), "Exception occurred trying to send " & "attribute change notification of a ModelMBean")

			Dim oldv As Object = ntfyObj.oldValue
			Dim newv As Object = ntfyObj.newValue

			If oldv Is Nothing Then oldv = "null"
			If newv Is Nothing Then newv = "null"

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "Sending AttributeChangeNotification with " & ntfyObj.attributeName + ntfyObj.attributeType + ntfyObj.newValue + ntfyObj.oldValue)

			' log notification if specified in descriptor
			Dim ntfyDesc As javax.management.Descriptor = modelMBeanInfo.getDescriptor(ntfyObj.type,"notification")
			Dim mmbDesc As javax.management.Descriptor = modelMBeanInfo.mBeanDescriptor

			Dim logging, logfile As String

			If ntfyDesc IsNot Nothing Then
				logging =CStr(ntfyDesc.getFieldValue("log"))
				If logging Is Nothing Then
					If mmbDesc IsNot Nothing Then logging = CStr(mmbDesc.getFieldValue("log"))
				End If
				If (logging IsNot Nothing) AndAlso (logging.ToUpper() = "t".ToUpper() OrElse logging.ToUpper() = "true".ToUpper()) Then
					logfile = CStr(ntfyDesc.getFieldValue("logfile"))
					If logfile Is Nothing Then
						If mmbDesc IsNot Nothing Then logfile = CStr(mmbDesc.getFieldValue("logfile"))
					End If

					If logfile IsNot Nothing Then
						Try
							writeToLog(logfile,"LogMsg: " & ((New DateTime(ntfyObj.timeStamp)).ToString()) & " " & ntfyObj.type & " " & ntfyObj.message & " Name = " & ntfyObj.attributeName & " Old value = " & oldv & " New value = " & newv)
						Catch e As Exception
							If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINE) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINE, GetType(RequiredModelMBean).name,mth, "Failed to log " & ntfyObj.type & " notification: ", e)
						End Try
					End If
				End If
			ElseIf mmbDesc IsNot Nothing Then
				logging = CStr(mmbDesc.getFieldValue("log"))
				If (logging IsNot Nothing) AndAlso (logging.ToUpper() = "t".ToUpper() OrElse logging.ToUpper() = "true".ToUpper()) Then
					logfile = CStr(mmbDesc.getFieldValue("logfile"))

					If logfile IsNot Nothing Then
						Try
							writeToLog(logfile,"LogMsg: " & ((New DateTime(ntfyObj.timeStamp)).ToString()) & " " & ntfyObj.type & " " & ntfyObj.message & " Name = " & ntfyObj.attributeName & " Old value = " & oldv & " New value = " & newv)
						Catch e As Exception
							If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINE) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINE, GetType(RequiredModelMBean).name,mth, "Failed to log " & ntfyObj.type & " notification: ", e)
						End Try
					End If
				End If
			End If
			If attributeBroadcaster IsNot Nothing Then attributeBroadcaster.sendNotification(ntfyObj)

			' XXX Revisit: This is a quickfix: it would be better to have a
			'     single broadcaster. However, it is not so simple because
			'     removeAttributeChangeNotificationListener() should
			'     remove only listeners whose filter is an instanceof
			'     AttributeChangeNotificationFilter.
			'
			If generalBroadcaster IsNot Nothing Then generalBroadcaster.sendNotification(ntfyObj)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "sent notification")
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "Exit")
			End If
		End Sub

		Public Overridable Sub sendAttributeChangeNotification(ByVal inOldVal As javax.management.Attribute, ByVal inNewVal As javax.management.Attribute) Implements ModelMBean.sendAttributeChangeNotification
			Const mth As String = "sendAttributeChangeNotification(Attribute, Attribute)"
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "Entry")

			' do we really want to do this?
			If (inOldVal Is Nothing) OrElse (inNewVal Is Nothing) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Attribute object must not be null"), "Exception occurred trying to send " & "attribute change notification of a ModelMBean")


			If Not(inOldVal.name.Equals(inNewVal.name)) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Attribute names are not the same"), "Exception occurred trying to send " & "attribute change notification of a ModelMBean")


			Dim newVal As Object = inNewVal.value
			Dim oldVal As Object = inOldVal.value
			Dim className As String = "unknown"
			If newVal IsNot Nothing Then className = newVal.GetType().name
			If oldVal IsNot Nothing Then className = oldVal.GetType().name

			Dim myNtfyObj As New javax.management.AttributeChangeNotification(Me, 1, ((New DateTime)), "AttributeChangeDetected", inOldVal.name, className, inOldVal.value, inNewVal.value)

			sendAttributeChangeNotification(myNtfyObj)

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(RequiredModelMBean).name,mth, "Exit")

		End Sub

		''' <summary>
		''' Return the Class Loader Repository used to perform class loading.
		''' Subclasses may wish to redefine this method in order to return
		''' the appropriate <seealso cref="javax.management.loading.ClassLoaderRepository"/>
		''' that should be used in this object.
		''' </summary>
		''' <returns> the Class Loader Repository.
		'''  </returns>
		Protected Friend Overridable Property classLoaderRepository As javax.management.loading.ClassLoaderRepository
			Get
				Return javax.management.MBeanServerFactory.getClassLoaderRepository(server)
			End Get
		End Property

		Private Function loadClass(ByVal className As String) As Type
			Dim stack As java.security.AccessControlContext = java.security.AccessController.context
			Dim caughtException As ClassNotFoundException() = New ClassNotFoundException(0){}

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Class c = javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Class>()
	'		{
	'
	'			@Override public Class run()
	'			{
	'				try
	'				{
	'					ReflectUtil.checkPackageAccess(className);
	'					Return Class.forName(className);
	'				}
	'				catch (ClassNotFoundException e)
	'				{
	'					final ClassLoaderRepository clr = getClassLoaderRepository();
	'					try
	'					{
	'						if (clr == Nothing)
	'							throw New ClassNotFoundException(className);
	'						Return clr.loadClass(className);
	'					}
	'					catch (ClassNotFoundException ex)
	'					{
	'						caughtException[0] = ex;
	'					}
	'				}
	'				Return Nothing;
	'			}
	'		}, stack, acc);

			If caughtException(0) IsNot Nothing Then Throw caughtException(0)

			Return c
		End Function


		''' <summary>
		'''********************************** </summary>
		' MBeanRegistration Interface       
		''' <summary>
		'''********************************** </summary>

		''' <summary>
		''' Allows the MBean to perform any operations it needs before
		''' being registered in the MBean server.  If the name of the MBean
		''' is not specified, the MBean can provide a name for its
		''' registration.  If any exception is raised, the MBean will not be
		''' registered in the MBean server.
		''' <P>
		''' In order to ensure proper run-time semantics of RequireModelMBean,
		''' Any subclass of RequiredModelMBean overloading or overriding this
		''' method should call <code>super.preRegister(server, name)</code>
		''' in its own <code>preRegister</code> implementation.
		''' </summary>
		''' <param name="server"> The MBean server in which the MBean will be registered.
		''' </param>
		''' <param name="name"> The object name of the MBean.  This name is null if
		''' the name parameter to one of the <code>createMBean</code> or
		''' <code>registerMBean</code> methods in the <seealso cref="MBeanServer"/>
		''' interface is null.  In that case, this method must return a
		''' non-null ObjectName for the new MBean.
		''' </param>
		''' <returns> The name under which the MBean is to be registered.
		''' This value must not be null.  If the <code>name</code>
		''' parameter is not null, it will usually but not necessarily be
		''' the returned value.
		''' </returns>
		''' <exception cref="java.lang.Exception"> This exception will be caught by
		''' the MBean server and re-thrown as an
		''' {@link javax.management.MBeanRegistrationException
		''' MBeanRegistrationException}. </exception>
		Public Overridable Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName
			' Since ModelMbeanInfo cannot be null (otherwise exception
			' thrown at creation)
			' no exception thrown on ModelMBeanInfo not set.
			If name Is Nothing Then Throw New NullPointerException("name of RequiredModelMBean to registered is null")
			Me.server = server
			Return name
		End Function

		''' <summary>
		''' Allows the MBean to perform any operations needed after having been
		''' registered in the MBean server or after the registration has failed.
		''' <P>
		''' In order to ensure proper run-time semantics of RequireModelMBean,
		''' Any subclass of RequiredModelMBean overloading or overriding this
		''' method should call <code>super.postRegister(registrationDone)</code>
		''' in its own <code>postRegister</code> implementation.
		''' </summary>
		''' <param name="registrationDone"> Indicates whether or not the MBean has
		''' been successfully registered in the MBean server. The value
		''' false means that the registration phase has failed. </param>
		Public Overridable Sub postRegister(ByVal registrationDone As Boolean?)
			registered = registrationDone
		End Sub

		''' <summary>
		''' Allows the MBean to perform any operations it needs before
		''' being unregistered by the MBean server.
		''' <P>
		''' In order to ensure proper run-time semantics of RequireModelMBean,
		''' Any subclass of RequiredModelMBean overloading or overriding this
		''' method should call <code>super.preDeregister()</code> in its own
		''' <code>preDeregister</code> implementation.
		''' </summary>
		''' <exception cref="java.lang.Exception"> This exception will be caught by
		''' the MBean server and re-thrown as an
		''' {@link javax.management.MBeanRegistrationException
		''' MBeanRegistrationException}. </exception>
		Public Overridable Sub preDeregister()
		End Sub

		''' <summary>
		''' Allows the MBean to perform any operations needed after having been
		''' unregistered in the MBean server.
		''' <P>
		''' In order to ensure proper run-time semantics of RequireModelMBean,
		''' Any subclass of RequiredModelMBean overloading or overriding this
		''' method should call <code>super.postDeregister()</code> in its own
		''' <code>postDeregister</code> implementation.
		''' </summary>
		Public Overridable Sub postDeregister()
			registered = False
			Me.server=Nothing
		End Sub

		Private Shared ReadOnly primitiveTypes As String()
		Private Shared ReadOnly primitiveWrappers As String()
	End Class

End Namespace
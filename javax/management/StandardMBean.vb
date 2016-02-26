Imports System
Imports System.Collections.Generic
import static com.sun.jmx.defaults.JmxProperties.MISC_LOGGER

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>An MBean whose management interface is determined by reflection
	''' on a Java interface.</p>
	''' 
	''' <p>This class brings more flexibility to the notion of Management
	''' Interface in the use of Standard MBeans.  Straightforward use of
	''' the patterns for Standard MBeans described in the JMX Specification
	''' means that there is a fixed relationship between the implementation
	''' class of an MBean and its management interface (i.e., if the
	''' implementation class is Thing, the management interface must be
	''' ThingMBean).  This class makes it possible to keep the convenience
	''' of specifying the management interface with a Java interface,
	''' without requiring that there be any naming relationship between the
	''' implementation and interface classes.</p>
	''' 
	''' <p>By making a DynamicMBean out of an MBean, this class makes
	''' it possible to select any interface implemented by the MBean as its
	''' management interface, provided that it complies with JMX patterns
	''' (i.e., attributes defined by getter/setter etc...).</p>
	''' 
	''' <p> This class also provides hooks that make it possible to supply
	''' custom descriptions and names for the <seealso cref="MBeanInfo"/> returned by
	''' the DynamicMBean interface.</p>
	''' 
	''' <p>Using this class, an MBean can be created with any
	''' implementation class name <i>Impl</i> and with a management
	''' interface defined (as for current Standard MBeans) by any interface
	''' <i>Intf</i>, in one of two general ways:</p>
	''' 
	''' <ul>
	''' 
	''' <li>Using the public constructor
	'''     {@link #StandardMBean(java.lang.Object, java.lang.Class, boolean)
	'''     StandardMBean(impl,interface)}:
	'''     <pre>
	'''     MBeanServer mbs;
	'''     ...
	'''     Impl impl = new Impl(...);
	'''     StandardMBean mbean = new StandardMBean(impl, Intf.class, false);
	'''     mbs.registerMBean(mbean, objectName);
	'''     </pre></li>
	''' 
	''' <li>Subclassing StandardMBean:
	'''     <pre>
	'''     public class Impl extends StandardMBean implements Intf {
	'''        public Impl() {
	'''          super(Intf.class, false);
	'''       }
	'''       // implement methods of Intf
	'''     }
	''' 
	'''     [...]
	''' 
	'''     MBeanServer mbs;
	'''     ....
	'''     Impl impl = new Impl();
	'''     mbs.registerMBean(impl, objectName);
	'''     </pre></li>
	''' 
	''' </ul>
	''' 
	''' <p>In either case, the class <i>Impl</i> must implement the
	''' interface <i>Intf</i>.</p>
	''' 
	''' <p>Standard MBeans based on the naming relationship between
	''' implementation and interface classes are of course still
	''' available.</p>
	''' 
	''' <p>This class may also be used to construct MXBeans.  The usage
	''' is exactly the same as for Standard MBeans except that in the
	''' examples above, the {@code false} parameter to the constructor or
	''' {@code super(...)} invocation is instead {@code true}.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class StandardMBean
		Implements DynamicMBean, MBeanRegistration

		Private Shared ReadOnly descriptors As com.sun.jmx.mbeanserver.DescriptorCache = com.sun.jmx.mbeanserver.DescriptorCache.getInstance(JMX.proof)

		''' <summary>
		''' The DynamicMBean that wraps the MXBean or Standard MBean implementation.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private mbean As com.sun.jmx.mbeanserver.MBeanSupport(Of ?)

		''' <summary>
		''' The cached MBeanInfo.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private cachedMBeanInfo As MBeanInfo

		''' <summary>
		''' Make a DynamicMBean out of <var>implementation</var>, using the
		''' specified <var>mbeanInterface</var> class. </summary>
		''' <param name="implementation"> The implementation of this MBean.
		'''        If <code>null</code>, and null implementation is allowed,
		'''        then the implementation is assumed to be <var>this</var>. </param>
		''' <param name="mbeanInterface"> The Management Interface exported by this
		'''        MBean's implementation. If <code>null</code>, then this
		'''        object will use standard JMX design pattern to determine
		'''        the management interface associated with the given
		'''        implementation. </param>
		''' <param name="nullImplementationAllowed"> <code>true</code> if a null
		'''        implementation is allowed. If null implementation is allowed,
		'''        and a null implementation is passed, then the implementation
		'''        is assumed to be <var>this</var>. </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''    <var>implementation</var> is null, and null is not allowed.
		'''  </exception>
		Private Sub construct(Of T)(ByVal implementation As T, ByVal mbeanInterface As Type, ByVal nullImplementationAllowed As Boolean, ByVal isMXBean As Boolean)
			If implementation Is Nothing Then
				' Have to use (T)this rather than mbeanInterface.cast(this)
				' because mbeanInterface might be null.
				If nullImplementationAllowed Then
					implementation = com.sun.jmx.mbeanserver.Util.cast(Of T)(Me)
				Else
					Throw New System.ArgumentException("implementation is null")
				End If
			End If
			If isMXBean Then
				If mbeanInterface Is Nothing Then mbeanInterface = com.sun.jmx.mbeanserver.Util.cast(com.sun.jmx.mbeanserver.Introspector.getMXBeanInterface(implementation.GetType()))
				Me.mbean = New com.sun.jmx.mbeanserver.MXBeanSupport(implementation, mbeanInterface)
			Else
				If mbeanInterface Is Nothing Then mbeanInterface = com.sun.jmx.mbeanserver.Util.cast(com.sun.jmx.mbeanserver.Introspector.getStandardMBeanInterface(implementation.GetType()))
				Me.mbean = New com.sun.jmx.mbeanserver.StandardMBeanSupport(implementation, mbeanInterface)
			End If
		End Sub

		''' <summary>
		''' <p>Make a DynamicMBean out of the object
		''' <var>implementation</var>, using the specified
		''' <var>mbeanInterface</var> class.</p>
		''' </summary>
		''' <param name="implementation"> The implementation of this MBean. </param>
		''' <param name="mbeanInterface"> The Management Interface exported by this
		'''        MBean's implementation. If <code>null</code>, then this
		'''        object will use standard JMX design pattern to determine
		'''        the management interface associated with the given
		'''        implementation. </param>
		''' @param <T> Allows the compiler to check
		''' that {@code implementation} does indeed implement the class
		''' described by {@code mbeanInterface}.  The compiler can only
		''' check this if {@code mbeanInterface} is a class literal such
		''' as {@code MyMBean.class}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''    <var>implementation</var> is null. </exception>
		''' <exception cref="NotCompliantMBeanException"> if the <var>mbeanInterface</var>
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if the given <var>implementation</var> does not implement the
		'''    specified interface.
		'''  </exception>
		Public Sub New(Of T)(ByVal implementation As T, ByVal mbeanInterface As Type)
			construct(implementation, mbeanInterface, False, False)
		End Sub

		''' <summary>
		''' <p>Make a DynamicMBean out of <var>this</var>, using the specified
		''' <var>mbeanInterface</var> class.</p>
		''' 
		''' <p>Calls {@link #StandardMBean(java.lang.Object, java.lang.Class)
		'''       this(this,mbeanInterface)}.
		''' This constructor is reserved to subclasses.</p>
		''' </summary>
		''' <param name="mbeanInterface"> The Management Interface exported by this
		'''        MBean.
		''' </param>
		''' <exception cref="NotCompliantMBeanException"> if the <var>mbeanInterface</var>
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if <var>this</var> does not implement the specified interface.
		'''  </exception>
		Protected Friend Sub New(ByVal mbeanInterface As Type)
			construct(Nothing, mbeanInterface, True, False)
		End Sub

		''' <summary>
		''' <p>Make a DynamicMBean out of the object
		''' <var>implementation</var>, using the specified
		''' <var>mbeanInterface</var> class, and choosing whether the
		''' resultant MBean is an MXBean.  This constructor can be used
		''' to make either Standard MBeans or MXBeans.  Unlike the
		''' constructor <seealso cref="#StandardMBean(Object, Class)"/>, it
		''' does not throw NotCompliantMBeanException.</p>
		''' </summary>
		''' <param name="implementation"> The implementation of this MBean. </param>
		''' <param name="mbeanInterface"> The Management Interface exported by this
		'''        MBean's implementation. If <code>null</code>, then this
		'''        object will use standard JMX design pattern to determine
		'''        the management interface associated with the given
		'''        implementation. </param>
		''' <param name="isMXBean"> If true, the {@code mbeanInterface} parameter
		''' names an MXBean interface and the resultant MBean is an MXBean. </param>
		''' @param <T> Allows the compiler to check
		''' that {@code implementation} does indeed implement the class
		''' described by {@code mbeanInterface}.  The compiler can only
		''' check this if {@code mbeanInterface} is a class literal such
		''' as {@code MyMBean.class}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''    <var>implementation</var> is null, or if the <var>mbeanInterface</var>
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if the given <var>implementation</var> does not implement the
		'''    specified interface.
		''' 
		''' @since 1.6
		'''  </exception>
		Public Sub New(Of T)(ByVal implementation As T, ByVal mbeanInterface As Type, ByVal isMXBean As Boolean)
			Try
				construct(implementation, mbeanInterface, False, isMXBean)
			Catch e As NotCompliantMBeanException
				Throw New System.ArgumentException(e)
			End Try
		End Sub

		''' <summary>
		''' <p>Make a DynamicMBean out of <var>this</var>, using the specified
		''' <var>mbeanInterface</var> class, and choosing whether the resulting
		''' MBean is an MXBean.  This constructor can be used
		''' to make either Standard MBeans or MXBeans.  Unlike the
		''' constructor <seealso cref="#StandardMBean(Object, Class)"/>, it
		''' does not throw NotCompliantMBeanException.</p>
		''' 
		''' <p>Calls {@link #StandardMBean(java.lang.Object, java.lang.Class, boolean)
		'''       this(this, mbeanInterface, isMXBean)}.
		''' This constructor is reserved to subclasses.</p>
		''' </summary>
		''' <param name="mbeanInterface"> The Management Interface exported by this
		'''        MBean. </param>
		''' <param name="isMXBean"> If true, the {@code mbeanInterface} parameter
		''' names an MXBean interface and the resultant MBean is an MXBean.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the <var>mbeanInterface</var>
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if <var>this</var> does not implement the specified interface.
		''' 
		''' @since 1.6
		'''  </exception>
		Protected Friend Sub New(ByVal mbeanInterface As Type, ByVal isMXBean As Boolean)
			Try
				construct(Nothing, mbeanInterface, True, isMXBean)
			Catch e As NotCompliantMBeanException
				Throw New System.ArgumentException(e)
			End Try
		End Sub

		''' <summary>
		''' <p>Replace the implementation object wrapped in this object.</p>
		''' </summary>
		''' <param name="implementation"> The new implementation of this Standard MBean
		''' (or MXBean). The <code>implementation</code> object must implement
		''' the Standard MBean (or MXBean) interface that was supplied when this
		''' <code>StandardMBean</code> was constructed.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		''' <var>implementation</var> is null.
		''' </exception>
		''' <exception cref="NotCompliantMBeanException"> if the given
		''' <var>implementation</var> does not implement the
		''' Standard MBean (or MXBean) interface that was
		''' supplied at construction.
		''' </exception>
		''' <seealso cref= #getImplementation
		'''  </seealso>
		Public Overridable Property implementation As Object
			Set(ByVal implementation As Object)
    
				If implementation Is Nothing Then Throw New System.ArgumentException("implementation is null")
    
				If mXBean Then
					Me.mbean = New com.sun.jmx.mbeanserver.MXBeanSupport(implementation, com.sun.jmx.mbeanserver.Util.cast(Of Type)(mBeanInterface))
				Else
					Me.mbean = New com.sun.jmx.mbeanserver.StandardMBeanSupport(implementation, com.sun.jmx.mbeanserver.Util.cast(Of Type)(mBeanInterface))
				End If
			End Set
			Get
				Return mbean.resource
			End Get
		End Property


		''' <summary>
		''' Get the Management Interface of this Standard MBean (or MXBean). </summary>
		''' <returns> The management interface of this Standard MBean (or MXBean).
		'''  </returns>
		Public Property mBeanInterface As Type
			Get
				Return mbean.mBeanInterface
			End Get
		End Property

		''' <summary>
		''' Get the class of the implementation of this Standard MBean (or MXBean). </summary>
		''' <returns> The class of the implementation of this Standard MBean (or MXBean).
		'''  </returns>
		Public Overridable Property implementationClass As Type
			Get
				Return mbean.resource.GetType()
			End Get
		End Property

		' ------------------------------------------------------------------
		' From the DynamicMBean interface.
		' ------------------------------------------------------------------
		Public Overridable Function getAttribute(ByVal attribute As String) As Object Implements DynamicMBean.getAttribute
			Return mbean.getAttribute(attribute)
		End Function

		' ------------------------------------------------------------------
		' From the DynamicMBean interface.
		' ------------------------------------------------------------------
		Public Overridable Property attribute Implements DynamicMBean.setAttribute As Attribute
			Set(ByVal attribute As Attribute)
				mbean.attribute = attribute
			End Set
		End Property

		' ------------------------------------------------------------------
		' From the DynamicMBean interface.
		' ------------------------------------------------------------------
		Public Overridable Function getAttributes(ByVal attributes As String()) As AttributeList Implements DynamicMBean.getAttributes
			Return mbean.getAttributes(attributes)
		End Function

		' ------------------------------------------------------------------
		' From the DynamicMBean interface.
		' ------------------------------------------------------------------
		Public Overridable Function setAttributes(ByVal attributes As AttributeList) As AttributeList Implements DynamicMBean.setAttributes
			Return mbean.attributestes(attributes)
		End Function

		' ------------------------------------------------------------------
		' From the DynamicMBean interface.
		' ------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public Object invoke(String actionName, Object params() , String signature()) throws MBeanException, ReflectionException
			Return mbean.invoke(actionName, params, signature)

		''' <summary>
		''' Get the <seealso cref="MBeanInfo"/> for this MBean.
		''' <p>
		''' This method implements
		''' {@link javax.management.DynamicMBean#getMBeanInfo()
		'''   DynamicMBean.getMBeanInfo()}.
		''' <p>
		''' This method first calls <seealso cref="#getCachedMBeanInfo()"/> in order to
		''' retrieve the cached MBeanInfo for this MBean, if any. If the
		''' MBeanInfo returned by <seealso cref="#getCachedMBeanInfo()"/> is not null,
		''' then it is returned.<br>
		''' Otherwise, this method builds a default MBeanInfo for this MBean,
		''' using the Management Interface specified for this MBean.
		''' <p>
		''' While building the MBeanInfo, this method calls the customization
		''' hooks that make it possible for subclasses to supply their custom
		''' descriptions, parameter names, etc...<br>
		''' Finally, it calls {@link #cacheMBeanInfo(javax.management.MBeanInfo)
		''' cacheMBeanInfo()} in order to cache the new MBeanInfo. </summary>
		''' <returns> The cached MBeanInfo for that MBean, if not null, or a
		'''         newly built MBeanInfo if none was cached.
		'''  </returns>
		public MBeanInfo mBeanInfo
			Try
				Dim cached As MBeanInfo = cachedMBeanInfo
				If cached IsNot Nothing Then Return cached
			Catch x As Exception
				If MISC_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MISC_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MBeanServerFactory).name, "getMBeanInfo", "Failed to get cached MBeanInfo", x)
			End Try

			If MISC_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MISC_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerFactory).name, "getMBeanInfo", "Building MBeanInfo for " & implementationClass.name)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim msupport As com.sun.jmx.mbeanserver.MBeanSupport(Of ?) = mbean
			Dim bi As MBeanInfo = msupport.mBeanInfo
			Dim impl As Object = msupport.resource

			Dim immutableInfo As Boolean = immutableInfo(Me.GetType())

			Dim cname As String = getClassName(bi)
			Dim text As String = getDescription(bi)
			Dim ctors As MBeanConstructorInfo() = getConstructors(bi,impl)
			Dim attrs As MBeanAttributeInfo() = getAttributes(bi)
			Dim ops As MBeanOperationInfo() = getOperations(bi)
			Dim ntfs As MBeanNotificationInfo() = getNotifications(bi)
			Dim desc As Descriptor = getDescriptor(bi, immutableInfo)

			Dim nmbi As New MBeanInfo(cname, text, attrs, ctors, ops, ntfs, desc)
			Try
				cacheMBeanInfo(nmbi)
			Catch x As Exception
				If MISC_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MISC_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MBeanServerFactory).name, "getMBeanInfo", "Failed to cache MBeanInfo", x)
			End Try

			Return nmbi

		''' <summary>
		''' Customization hook:
		''' Get the className that will be used in the MBeanInfo returned by
		''' this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom class name.  The default implementation returns
		''' <seealso cref="MBeanInfo#getClassName() info.getClassName()"/>. </summary>
		''' <param name="info"> The default MBeanInfo derived by reflection. </param>
		''' <returns> the class name for the new MBeanInfo.
		'''  </returns>
		protected String getClassName(MBeanInfo info)
			If info Is Nothing Then Return implementationClass.name
			Return info.className

		''' <summary>
		''' Customization hook:
		''' Get the description that will be used in the MBeanInfo returned by
		''' this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom MBean description.  The default implementation returns
		''' <seealso cref="MBeanInfo#getDescription() info.getDescription()"/>. </summary>
		''' <param name="info"> The default MBeanInfo derived by reflection. </param>
		''' <returns> the description for the new MBeanInfo.
		'''  </returns>
		protected String getDescription(MBeanInfo info)
			If info Is Nothing Then Return Nothing
			Return info.description

		''' <summary>
		''' <p>Customization hook:
		''' Get the description that will be used in the MBeanFeatureInfo
		''' returned by this MBean.</p>
		''' 
		''' <p>Subclasses may redefine this method in order to supply
		''' their custom description.  The default implementation returns
		''' {@link MBeanFeatureInfo#getDescription()
		''' info.getDescription()}.</p>
		''' 
		''' <p>This method is called by
		'''      <seealso cref="#getDescription(MBeanAttributeInfo)"/>,
		'''      <seealso cref="#getDescription(MBeanOperationInfo)"/>,
		'''      <seealso cref="#getDescription(MBeanConstructorInfo)"/>.</p>
		''' </summary>
		''' <param name="info"> The default MBeanFeatureInfo derived by reflection. </param>
		''' <returns> the description for the given MBeanFeatureInfo.
		'''  </returns>
		protected String getDescription(MBeanFeatureInfo info)
			If info Is Nothing Then Return Nothing
			Return info.description

		''' <summary>
		''' Customization hook:
		''' Get the description that will be used in the MBeanAttributeInfo
		''' returned by this MBean.
		''' 
		''' <p>Subclasses may redefine this method in order to supply their
		''' custom description.  The default implementation returns {@link
		''' #getDescription(MBeanFeatureInfo)
		''' getDescription((MBeanFeatureInfo) info)}. </summary>
		''' <param name="info"> The default MBeanAttributeInfo derived by reflection. </param>
		''' <returns> the description for the given MBeanAttributeInfo.
		'''  </returns>
		protected String getDescription(MBeanAttributeInfo info)
			Return getDescription(CType(info, MBeanFeatureInfo))

		''' <summary>
		''' Customization hook:
		''' Get the description that will be used in the MBeanConstructorInfo
		''' returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom description.
		''' The default implementation returns {@link
		''' #getDescription(MBeanFeatureInfo)
		''' getDescription((MBeanFeatureInfo) info)}. </summary>
		''' <param name="info"> The default MBeanConstructorInfo derived by reflection. </param>
		''' <returns> the description for the given MBeanConstructorInfo.
		'''  </returns>
		protected String getDescription(MBeanConstructorInfo info)
			Return getDescription(CType(info, MBeanFeatureInfo))

		''' <summary>
		''' Customization hook:
		''' Get the description that will be used for the  <var>sequence</var>
		''' MBeanParameterInfo of the MBeanConstructorInfo returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom description.  The default implementation returns
		''' <seealso cref="MBeanParameterInfo#getDescription() param.getDescription()"/>.
		''' </summary>
		''' <param name="ctor">  The default MBeanConstructorInfo derived by reflection. </param>
		''' <param name="param"> The default MBeanParameterInfo derived by reflection. </param>
		''' <param name="sequence"> The sequence number of the parameter considered
		'''        ("0" for the first parameter, "1" for the second parameter,
		'''        etc...). </param>
		''' <returns> the description for the given MBeanParameterInfo.
		'''  </returns>
		protected String getDescription(MBeanConstructorInfo ctor, MBeanParameterInfo param, Integer sequence)
			If param Is Nothing Then Return Nothing
			Return param.description

		''' <summary>
		''' Customization hook:
		''' Get the name that will be used for the <var>sequence</var>
		''' MBeanParameterInfo of the MBeanConstructorInfo returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom parameter name.  The default implementation returns
		''' <seealso cref="MBeanParameterInfo#getName() param.getName()"/>.
		''' </summary>
		''' <param name="ctor">  The default MBeanConstructorInfo derived by reflection. </param>
		''' <param name="param"> The default MBeanParameterInfo derived by reflection. </param>
		''' <param name="sequence"> The sequence number of the parameter considered
		'''        ("0" for the first parameter, "1" for the second parameter,
		'''        etc...). </param>
		''' <returns> the name for the given MBeanParameterInfo.
		'''  </returns>
		protected String getParameterName(MBeanConstructorInfo ctor, MBeanParameterInfo param, Integer sequence)
			If param Is Nothing Then Return Nothing
			Return param.name

		''' <summary>
		''' Customization hook:
		''' Get the description that will be used in the MBeanOperationInfo
		''' returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom description.  The default implementation returns
		''' {@link #getDescription(MBeanFeatureInfo)
		''' getDescription((MBeanFeatureInfo) info)}. </summary>
		''' <param name="info"> The default MBeanOperationInfo derived by reflection. </param>
		''' <returns> the description for the given MBeanOperationInfo.
		'''  </returns>
		protected String getDescription(MBeanOperationInfo info)
			Return getDescription(CType(info, MBeanFeatureInfo))

		''' <summary>
		''' Customization hook:
		''' Get the <var>impact</var> flag of the operation that will be used in
		''' the MBeanOperationInfo returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom impact flag.  The default implementation returns
		''' <seealso cref="MBeanOperationInfo#getImpact() info.getImpact()"/>. </summary>
		''' <param name="info"> The default MBeanOperationInfo derived by reflection. </param>
		''' <returns> the impact flag for the given MBeanOperationInfo.
		'''  </returns>
		protected Integer getImpact(MBeanOperationInfo info)
			If info Is Nothing Then Return MBeanOperationInfo.UNKNOWN
			Return info.impact

		''' <summary>
		''' Customization hook:
		''' Get the name that will be used for the <var>sequence</var>
		''' MBeanParameterInfo of the MBeanOperationInfo returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom parameter name.  The default implementation returns
		''' <seealso cref="MBeanParameterInfo#getName() param.getName()"/>.
		''' </summary>
		''' <param name="op">    The default MBeanOperationInfo derived by reflection. </param>
		''' <param name="param"> The default MBeanParameterInfo derived by reflection. </param>
		''' <param name="sequence"> The sequence number of the parameter considered
		'''        ("0" for the first parameter, "1" for the second parameter,
		'''        etc...). </param>
		''' <returns> the name to use for the given MBeanParameterInfo.
		'''  </returns>
		protected String getParameterName(MBeanOperationInfo op, MBeanParameterInfo param, Integer sequence)
			If param Is Nothing Then Return Nothing
			Return param.name

		''' <summary>
		''' Customization hook:
		''' Get the description that will be used for the  <var>sequence</var>
		''' MBeanParameterInfo of the MBeanOperationInfo returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom description.  The default implementation returns
		''' <seealso cref="MBeanParameterInfo#getDescription() param.getDescription()"/>.
		''' </summary>
		''' <param name="op">    The default MBeanOperationInfo derived by reflection. </param>
		''' <param name="param"> The default MBeanParameterInfo derived by reflection. </param>
		''' <param name="sequence"> The sequence number of the parameter considered
		'''        ("0" for the first parameter, "1" for the second parameter,
		'''        etc...). </param>
		''' <returns> the description for the given MBeanParameterInfo.
		'''  </returns>
		protected String getDescription(MBeanOperationInfo op, MBeanParameterInfo param, Integer sequence)
			If param Is Nothing Then Return Nothing
			Return param.description

		''' <summary>
		''' Customization hook:
		''' Get the MBeanConstructorInfo[] that will be used in the MBeanInfo
		''' returned by this MBean.
		''' <br>
		''' By default, this method returns <code>null</code> if the wrapped
		''' implementation is not <var>this</var>. Indeed, if the wrapped
		''' implementation is not this object itself, it will not be possible
		''' to recreate a wrapped implementation by calling the implementation
		''' constructors through <code>MBeanServer.createMBean(...)</code>.<br>
		''' Otherwise, if the wrapped implementation is <var>this</var>,
		''' <var>ctors</var> is returned.
		''' <br>
		''' Subclasses may redefine this method in order to modify this
		''' behavior, if needed. </summary>
		''' <param name="ctors"> The default MBeanConstructorInfo[] derived by reflection. </param>
		''' <param name="impl">  The wrapped implementation. If <code>null</code> is
		'''        passed, the wrapped implementation is ignored and
		'''        <var>ctors</var> is returned. </param>
		''' <returns> the MBeanConstructorInfo[] for the new MBeanInfo.
		'''  </returns>
		protected MBeanConstructorInfo() getConstructors(MBeanConstructorInfo() ctors, Object impl)
				If ctors Is Nothing Then Return Nothing
				If impl IsNot Nothing AndAlso impl IsNot Me Then Return Nothing
				Return ctors

		''' <summary>
		''' Customization hook:
		''' Get the MBeanNotificationInfo[] that will be used in the MBeanInfo
		''' returned by this MBean.
		''' <br>
		''' Subclasses may redefine this method in order to supply their
		''' custom notifications. </summary>
		''' <param name="info"> The default MBeanInfo derived by reflection. </param>
		''' <returns> the MBeanNotificationInfo[] for the new MBeanInfo.
		'''  </returns>
		MBeanNotificationInfo() getNotifications(MBeanInfo info)
			Return Nothing

		''' <summary>
		''' <p>Get the Descriptor that will be used in the MBeanInfo
		''' returned by this MBean.</p>
		''' 
		''' <p>Subclasses may redefine this method in order to supply
		''' their custom descriptor.</p>
		''' 
		''' <p>The default implementation of this method returns a Descriptor
		''' that contains at least the field {@code interfaceClassName}, with
		''' value <seealso cref="#getMBeanInterface()"/>.getName(). It may also contain
		''' the field {@code immutableInfo}, with a value that is the string
		''' {@code "true"} if the implementation can determine that the
		''' {@code MBeanInfo} returned by <seealso cref="#getMBeanInfo()"/> will always
		''' be the same. It may contain other fields: fields defined by the
		''' JMX specification must have appropriate values, and other fields
		''' must follow the conventions for non-standard field names.</p>
		''' </summary>
		''' <param name="info"> The default MBeanInfo derived by reflection. </param>
		''' <returns> the Descriptor for the new MBeanInfo. </returns>
		Descriptor getDescriptor(MBeanInfo info, Boolean immutableInfo)
			Dim desc As ImmutableDescriptor
			If info Is Nothing OrElse info.descriptor Is Nothing OrElse info.descriptor.fieldNames.length = 0 Then
				Dim interfaceClassNameS As String = "interfaceClassName=" & mBeanInterface.name
				Dim immutableInfoS As String = "immutableInfo=" & immutableInfo
				desc = New ImmutableDescriptor(interfaceClassNameS, immutableInfoS)
				desc = descriptors.get(desc)
			Else
				Dim d As Descriptor = info.descriptor
				Dim fields As IDictionary(Of String, Object) = New Dictionary(Of String, Object)
				For Each fieldName As String In d.fieldNames
					If fieldName.Equals("immutableInfo") Then
						' Replace immutableInfo as the underlying MBean/MXBean
						' could already implement NotificationBroadcaster and
						' return immutableInfo=true in its MBeanInfo.
						fields(fieldName) = Convert.ToString(immutableInfo)
					Else
						fields(fieldName) = d.getFieldValue(fieldName)
					End If
				Next fieldName
				desc = New ImmutableDescriptor(fields)
			End If
			Return desc

		''' <summary>
		''' Customization hook:
		''' Return the MBeanInfo cached for this object.
		''' 
		''' <p>Subclasses may redefine this method in order to implement their
		''' own caching policy.  The default implementation stores one
		''' <seealso cref="MBeanInfo"/> object per instance.
		''' </summary>
		''' <returns> The cached MBeanInfo, or null if no MBeanInfo is cached.
		''' </returns>
		''' <seealso cref= #cacheMBeanInfo(MBeanInfo)
		'''  </seealso>
		protected MBeanInfo cachedMBeanInfo
			Return cachedMBeanInfo

		''' <summary>
		''' Customization hook:
		''' cache the MBeanInfo built for this object.
		''' 
		''' <p>Subclasses may redefine this method in order to implement
		''' their own caching policy.  The default implementation stores
		''' <code>info</code> in this instance.  A subclass can define
		''' other policies, such as not saving <code>info</code> (so it is
		''' reconstructed every time <seealso cref="#getMBeanInfo()"/> is called) or
		''' sharing a unique <seealso cref="MBeanInfo"/> object when several
		''' <code>StandardMBean</code> instances have equal {@link
		''' MBeanInfo} values.
		''' </summary>
		''' <param name="info"> the new <code>MBeanInfo</code> to cache.  Any
		''' previously cached value is discarded.  This parameter may be
		''' null, in which case there is no new cached value.
		'''  </param>
		protected void cacheMBeanInfo(MBeanInfo info)
			cachedMBeanInfo = info

		private Boolean mXBean
			Return mbean.mXBean

		private static (Of T) Boolean identicalArrays(T() a, T() b)
			If a = b Then Return True
			If a Is Nothing OrElse b Is Nothing OrElse a.length <> b.length Then Return False
			For i As Integer = 0 To a.length - 1
				If a(i) <> b(i) Then Return False
			Next i
			Return True

		private static (Of T) Boolean equal(T a, T b)
			If a = b Then Return True
			If a Is Nothing OrElse b Is Nothing Then Return False
			Return a.Equals(b)

		private static MBeanParameterInfo customize(MBeanParameterInfo pi, String name, String description)
			If equal(name, pi.name) AndAlso equal(description, pi.description) Then
				Return pi
			ElseIf TypeOf pi Is javax.management.openmbean.OpenMBeanParameterInfo Then
				Dim opi As javax.management.openmbean.OpenMBeanParameterInfo = CType(pi, javax.management.openmbean.OpenMBeanParameterInfo)
				Return New javax.management.openmbean.OpenMBeanParameterInfoSupport(name, description, opi.openType, pi.descriptor)
			Else
				Return New MBeanParameterInfo(name, pi.type, description, pi.descriptor)
			End If

		private static MBeanConstructorInfo customize(MBeanConstructorInfo ci, String description, MBeanParameterInfo() signature)
			If equal(description, ci.description) AndAlso identicalArrays(signature, ci.signature) Then Return ci
			If TypeOf ci Is javax.management.openmbean.OpenMBeanConstructorInfo Then
				Dim oparams As javax.management.openmbean.OpenMBeanParameterInfo() = paramsToOpenParams(signature)
				Return New javax.management.openmbean.OpenMBeanConstructorInfoSupport(ci.name, description, oparams, ci.descriptor)
			Else
				Return New MBeanConstructorInfo(ci.name, description, signature, ci.descriptor)
			End If

		private static MBeanOperationInfo customize(MBeanOperationInfo oi, String description, MBeanParameterInfo() signature, Integer impact)
			If equal(description, oi.description) AndAlso identicalArrays(signature, oi.signature) AndAlso impact = oi.impact Then Return oi
			If TypeOf oi Is javax.management.openmbean.OpenMBeanOperationInfo Then
				Dim ooi As javax.management.openmbean.OpenMBeanOperationInfo = CType(oi, javax.management.openmbean.OpenMBeanOperationInfo)
				Dim oparams As javax.management.openmbean.OpenMBeanParameterInfo() = paramsToOpenParams(signature)
				Return New javax.management.openmbean.OpenMBeanOperationInfoSupport(oi.name, description, oparams, ooi.returnOpenType, impact, oi.descriptor)
			Else
				Return New MBeanOperationInfo(oi.name, description, signature, oi.returnType, impact, oi.descriptor)
			End If

		private static MBeanAttributeInfo customize(MBeanAttributeInfo ai, String description)
			If equal(description, ai.description) Then Return ai
			If TypeOf ai Is javax.management.openmbean.OpenMBeanAttributeInfo Then
				Dim oai As javax.management.openmbean.OpenMBeanAttributeInfo = CType(ai, javax.management.openmbean.OpenMBeanAttributeInfo)
				Return New javax.management.openmbean.OpenMBeanAttributeInfoSupport(ai.name, description, oai.openType, ai.readable, ai.writable, ai.is, ai.descriptor)
			Else
				Return New MBeanAttributeInfo(ai.name, ai.type, description, ai.readable, ai.writable, ai.is, ai.descriptor)
			End If

		private static javax.management.openmbean.OpenMBeanParameterInfo() paramsToOpenParams(MBeanParameterInfo() params)
			If TypeOf params Is javax.management.openmbean.OpenMBeanParameterInfo() Then Return CType(params, javax.management.openmbean.OpenMBeanParameterInfo())
			Dim oparams As javax.management.openmbean.OpenMBeanParameterInfo() = New javax.management.openmbean.OpenMBeanParameterInfoSupport(params.length - 1){}
			Array.Copy(params, 0, oparams, 0, params.length)
			Return oparams

		' ------------------------------------------------------------------
		' Build the custom MBeanConstructorInfo[]
		' ------------------------------------------------------------------
		private MBeanConstructorInfo() getConstructors(MBeanInfo info, Object impl)
			Dim ctors As MBeanConstructorInfo() = getConstructors(info.constructors, impl)
			If ctors Is Nothing Then Return Nothing
			Dim ctorlen As Integer = ctors.Length
			Dim nctors As MBeanConstructorInfo() = New MBeanConstructorInfo(ctorlen - 1){}
			For i As Integer = 0 To ctorlen - 1
				Dim c As MBeanConstructorInfo = ctors(i)
				Dim params As MBeanParameterInfo() = c.signature
				Dim nps As MBeanParameterInfo()
				If params IsNot Nothing Then
					Dim plen As Integer = params.Length
					nps = New MBeanParameterInfo(plen - 1){}
					For ii As Integer = 0 To plen - 1
						Dim p As MBeanParameterInfo = params(ii)
						nps(ii) = customize(p, getParameterName(c,p,ii), getDescription(c,p,ii))
					Next ii
				Else
					nps = Nothing
				End If
				nctors(i) = customize(c, getDescription(c), nps)
			Next i
			Return nctors

		' ------------------------------------------------------------------
		' Build the custom MBeanOperationInfo[]
		' ------------------------------------------------------------------
		private MBeanOperationInfo() getOperations(MBeanInfo info)
			Dim ops As MBeanOperationInfo() = info.operations
			If ops Is Nothing Then Return Nothing
			Dim oplen As Integer = ops.Length
			Dim nops As MBeanOperationInfo() = New MBeanOperationInfo(oplen - 1){}
			For i As Integer = 0 To oplen - 1
				Dim o As MBeanOperationInfo = ops(i)
				Dim params As MBeanParameterInfo() = o.signature
				Dim nps As MBeanParameterInfo()
				If params IsNot Nothing Then
					Dim plen As Integer = params.Length
					nps = New MBeanParameterInfo(plen - 1){}
					For ii As Integer = 0 To plen - 1
						Dim p As MBeanParameterInfo = params(ii)
						nps(ii) = customize(p, getParameterName(o,p,ii), getDescription(o,p,ii))
					Next ii
				Else
					nps = Nothing
				End If
				nops(i) = customize(o, getDescription(o), nps, getImpact(o))
			Next i
			Return nops

		' ------------------------------------------------------------------
		' Build the custom MBeanAttributeInfo[]
		' ------------------------------------------------------------------
		private MBeanAttributeInfo() getAttributes(MBeanInfo info)
			Dim atts As MBeanAttributeInfo() = info.attributes
			If atts Is Nothing Then Return Nothing ' should not happen
			Dim natts As MBeanAttributeInfo()
			Dim attlen As Integer = atts.Length
			natts = New MBeanAttributeInfo(attlen - 1){}
			For i As Integer = 0 To attlen - 1
				Dim a As MBeanAttributeInfo = atts(i)
				natts(i) = customize(a, getDescription(a))
			Next i
			Return natts

		''' <summary>
		''' <p>Allows the MBean to perform any operations it needs before
		''' being registered in the MBean server.  If the name of the MBean
		''' is not specified, the MBean can provide a name for its
		''' registration.  If any exception is raised, the MBean will not be
		''' registered in the MBean server.</p>
		''' 
		''' <p>The default implementation of this method returns the {@code name}
		''' parameter.  It does nothing else for
		''' Standard MBeans.  For MXBeans, it records the {@code MBeanServer}
		''' and {@code ObjectName} parameters so they can be used to translate
		''' inter-MXBean references.</p>
		''' 
		''' <p>It is good practice for a subclass that overrides this method
		''' to call the overridden method via {@code super.preRegister(...)}.
		''' This is necessary if this object is an MXBean that is referenced
		''' by attributes or operations in other MXBeans.</p>
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
		''' <exception cref="IllegalArgumentException"> if this is an MXBean and
		''' {@code name} is null.
		''' </exception>
		''' <exception cref="InstanceAlreadyExistsException"> if this is an MXBean and
		''' it has already been registered under another name (in this
		''' MBean Server or another).
		''' </exception>
		''' <exception cref="Exception"> no other checked exceptions are thrown by
		''' this method but {@code Exception} is declared so that subclasses
		''' can override the method and throw their own exceptions.
		''' 
		''' @since 1.6 </exception>
		public ObjectName preRegister(MBeanServer server, ObjectName name) throws Exception
			mbean.register(server, name)
			Return name

		''' <summary>
		''' <p>Allows the MBean to perform any operations needed after having been
		''' registered in the MBean server or after the registration has failed.</p>
		''' 
		''' <p>The default implementation of this method does nothing for
		''' Standard MBeans.  For MXBeans, it undoes any work done by
		''' <seealso cref="#preRegister preRegister"/> if registration fails.</p>
		''' 
		''' <p>It is good practice for a subclass that overrides this method
		''' to call the overridden method via {@code super.postRegister(...)}.
		''' This is necessary if this object is an MXBean that is referenced
		''' by attributes or operations in other MXBeans.</p>
		''' </summary>
		''' <param name="registrationDone"> Indicates whether or not the MBean has
		''' been successfully registered in the MBean server. The value
		''' false means that the registration phase has failed.
		''' 
		''' @since 1.6 </param>
		public void postRegister(Boolean? registrationDone)
			If Not registrationDone Then mbean.unregister()

		''' <summary>
		''' <p>Allows the MBean to perform any operations it needs before
		''' being unregistered by the MBean server.</p>
		''' 
		''' <p>The default implementation of this method does nothing.</p>
		''' 
		''' <p>It is good practice for a subclass that overrides this method
		''' to call the overridden method via {@code super.preDeregister(...)}.</p>
		''' </summary>
		''' <exception cref="Exception"> no checked exceptions are throw by this method
		''' but {@code Exception} is declared so that subclasses can override
		''' this method and throw their own exceptions.
		''' 
		''' @since 1.6 </exception>
		public void preDeregister() throws Exception

		''' <summary>
		''' <p>Allows the MBean to perform any operations needed after having been
		''' unregistered in the MBean server.</p>
		''' 
		''' <p>The default implementation of this method does nothing for
		''' Standard MBeans.  For MXBeans, it removes any information that
		''' was recorded by the <seealso cref="#preRegister preRegister"/> method.</p>
		''' 
		''' <p>It is good practice for a subclass that overrides this method
		''' to call the overridden method via {@code super.postRegister(...)}.
		''' This is necessary if this object is an MXBean that is referenced
		''' by attributes or operations in other MXBeans.</p>
		''' 
		''' @since 1.6
		''' </summary>
		public void postDeregister()
			mbean.unregister()

		'
		' MBeanInfo immutability
		'

		''' <summary>
		''' Cached results of previous calls to immutableInfo. This is
		''' a WeakHashMap so that we don't prevent a class from being
		''' garbage collected just because we know whether its MBeanInfo
		''' is immutable.
		''' </summary>
		private static final IDictionary(Of Type, Boolean?) mbeanInfoSafeMap = New java.util.WeakHashMap(Of Type, Boolean?)

		''' <summary>
		''' Return true if {@code subclass} is known to preserve the immutability
		''' of the {@code MBeanInfo}. The {@code subclass} is considered to have
		''' an immutable {@code MBeanInfo} if it does not override any of the
		''' getMBeanInfo, getCachedMBeanInfo, cacheMBeanInfo and getNotificationInfo
		''' methods.
		''' </summary>
		static Boolean immutableInfo(Type subclass)
			If subclass = GetType(StandardMBean) OrElse subclass = GetType(StandardEmitterMBean) Then Return True
			SyncLock mbeanInfoSafeMap
				Dim safe As Boolean? = mbeanInfoSafeMap(subclass)
				If safe Is Nothing Then
					Try
						Dim action As New MBeanInfoSafeAction(subclass)
						safe = java.security.AccessController.doPrivileged(action) ' e.g. SecurityException
					Catch e As Exception
						' We don't know, so we assume it isn't.  
						safe = False
					End Try
					mbeanInfoSafeMap(subclass) = safe
				End If
				Return safe
			End SyncLock

		static Boolean [overrides](Type subclass, Type superclass, String name, Type... params)
			Dim c As Type = subclass
			Do While c IsNot superclass
				Try
					c.getDeclaredMethod(name, params)
					Return True
				Catch e As NoSuchMethodException
					' OK: this class doesn't override it
				End Try
				c = c.BaseType
			Loop
			Return False

		private static class MBeanInfoSafeAction implements java.security.PrivilegedAction(Of Boolean?)

			private final Type subclass

			MBeanInfoSafeAction(Type subclass)
				Me.subclass = subclass

			public Boolean? run()
				' Check for "void cacheMBeanInfo(MBeanInfo)" method.
				'
				If [overrides](subclass, GetType(StandardMBean), "cacheMBeanInfo", GetType(MBeanInfo)) Then Return False

				' Check for "MBeanInfo getCachedMBeanInfo()" method.
				'
				If [overrides](subclass, GetType(StandardMBean), "getCachedMBeanInfo", CType(Nothing, Type())) Then Return False

				' Check for "MBeanInfo getMBeanInfo()" method.
				'
				If [overrides](subclass, GetType(StandardMBean), "getMBeanInfo", CType(Nothing, Type())) Then Return False

				' Check for "MBeanNotificationInfo[] getNotificationInfo()"
				' method.
				'
				' This method is taken into account for the MBeanInfo
				' immutability checks if and only if the given subclass is
				' StandardEmitterMBean itself or can be assigned to
				' StandardEmitterMBean.
				'
				If subclass.IsSubclassOf(GetType(StandardEmitterMBean)) Then
					If [overrides](subclass, GetType(StandardEmitterMBean), "getNotificationInfo", CType(Nothing, Type())) Then Return False
				End If
				Return True
	End Class

End Namespace
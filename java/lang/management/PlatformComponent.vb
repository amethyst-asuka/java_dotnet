'
' * Copyright (c) 2008, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.management




	''' <summary>
	''' This enum class defines the list of platform components
	''' that provides monitoring and management support.
	''' Each enum represents one MXBean interface. A MXBean
	''' instance could implement one or more MXBean interfaces.
	''' 
	''' For example, com.sun.management.GarbageCollectorMXBean
	''' extends java.lang.management.GarbageCollectorMXBean
	''' and there is one set of garbage collection MXBean instances,
	''' each of which implements both c.s.m. and j.l.m. interfaces.
	''' There are two separate enums GARBAGE_COLLECTOR
	''' and SUN_GARBAGE_COLLECTOR so that ManagementFactory.getPlatformMXBeans(Class)
	''' will return the list of MXBeans of the specified type.
	''' 
	''' To add a new MXBean interface for the Java platform,
	''' add a new enum constant and implement the MXBeanFetcher.
	''' </summary>
	Friend Enum PlatformComponent

		''' <summary>
		''' Class loading system of the Java virtual machine.
		''' </summary>
		CLASS_LOADING("java.lang.management.ClassLoadingMXBean",
			"java.lang"
			"ClassLoading"
			defaultKeyProperties = 
			[True] ' singleton
			[New] = 

		''' <summary>
		''' Compilation system of the Java virtual machine.
		''' </summary>
		COMPILATION = ' singleton

		''' <summary>
		''' Memory system of the Java virtual machine.
		''' </summary>
		MEMORY = ' singleton

		''' <summary>
		''' Garbage Collector in the Java virtual machine.
		''' </summary>
		GARBAGE_COLLECTOR = ' zero or more instances

		''' <summary>
		''' Memory manager in the Java virtual machine.
		''' </summary>
		MEMORY_MANAGER = ' zero or more instances
			GARBAGE_COLLECTOR)
			MEMORY_POOL = ' zero or more instances

		''' <summary>
		''' Operating system on which the Java virtual machine is running
		''' </summary>
		OPERATING_SYSTEM = ' singleton

		''' <summary>
		''' Runtime system of the Java virtual machine.
		''' </summary>
		RUNTIME = ' singleton

		''' <summary>
		''' Threading system of the Java virtual machine.
		''' </summary>
		THREADING = ' singleton


		''' <summary>
		''' Logging facility.
		''' </summary>
		LOGGING = ' singleton

		''' <summary>
		''' Buffer pools.
		''' </summary>
		BUFFER_POOL = ' zero or more instances


		' Sun Platform Extension

		''' <summary>
		''' Sun extension garbage collector that performs collections in cycles.
		''' </summary>
		SUN_GARBAGE_COLLECTOR = ' zero or more instances

		''' <summary>
		''' Sun extension operating system on which the Java virtual machine
		''' is running.
		''' </summary>
		SUN_OPERATING_SYSTEM = ' singleton

		''' <summary>
		''' Unix operating system.
		''' </summary>
		SUN_UNIX_OPERATING_SYSTEM = ' singleton

		''' <summary>
		''' Diagnostic support for the HotSpot Virtual Machine.
		''' </summary>
		HOTSPOT_DIAGNOSTIC = ' singleton


		''' <summary>
		''' A task that returns the MXBeans for a component.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		interface MXBeanFetcher<T extends PlatformManagedObject>
			[public] = 

	'    
	'     * Returns a list of the GC MXBeans of the given type.
	'     
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static (Of T As GarbageCollectorMXBean) java.util.List(Of T) getGcMXBeanList(Class gcMXBeanIntf)
	'	{
	'		List<GarbageCollectorMXBean> list = ManagementFactoryHelper.getGarbageCollectorMXBeans();
	'		List<T> result = New ArrayList<>(list.size());
	'		for (GarbageCollectorMXBean m : list)
	'		{
	'			if (gcMXBeanIntf.isInstance(m))
	'			{
	'				result.add(gcMXBeanIntf.cast(m));
	'			}
	'		}
	'		Return result;
	'	}

	'    
	'     * Returns the OS mxbean instance of the given type.
	'     
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static (Of T As OperatingSystemMXBean) java.util.List(Of T) getOSMXBeanList(Class osMXBeanIntf)
	'	{
	'		OperatingSystemMXBean m = ManagementFactoryHelper.getOperatingSystemMXBean();
	'		if (osMXBeanIntf.isInstance(m))
	'		{
	'			Return Collections.singletonList(osMXBeanIntf.cast(m));
	'		}
	'		else
	'		{
	'			Return Collections.emptyList();
	'		}
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final String mxbeanInterfaceName;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final String domain;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final String type;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final java.util.Set(Of String) keyProperties;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final MXBeanFetcher(Of JavaToDotNetGenericWildcard) fetcher;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final PlatformComponent[] subComponents;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final boolean singleton;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private PlatformComponent(String intfName, String domain, String type, java.util.Set(Of String) keyProperties, boolean singleton, MXBeanFetcher(Of JavaToDotNetGenericWildcard) fetcher, PlatformComponent... subComponents)
	'	{
	'		Me.mxbeanInterfaceName = intfName;
	'		Me.domain = domain;
	'		Me.type = type;
	'		Me.keyProperties = keyProperties;
	'		Me.singleton = singleton;
	'		Me.fetcher = fetcher;
	'		Me.subComponents = subComponents;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static java.util.Set(Of String) defaultKeyProps;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static java.util.Set(Of String) defaultKeyProperties()
	'	{
	'		if (defaultKeyProps == Nothing)
	'		{
	'			defaultKeyProps = Collections.singleton("type");
	'		}
	'		Return defaultKeyProps;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static java.util.Set(Of String) keyProperties(String... keyNames)
	'	{
	'		Set<String> set = New HashSet<>();
	'		set.add("type");
	'		for (String s : keyNames)
	'		{
	'			set.add(s);
	'		}
	'		Return set;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		boolean isSingleton()
	'	{
	'		Return singleton;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		String getMXBeanInterfaceName()
	'	{
	'		Return mxbeanInterfaceName;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		Class getMXBeanInterface()
	'	{
	'		try
	'		{
	'			' Lazy loading the MXBean interface only when it is needed
	'			Return (Class) Class.forName(mxbeanInterfaceName, False, PlatformManagedObject.class.getClassLoader());
	'		}
	'		catch (ClassNotFoundException x)
	'		{
	'			throw New AssertionError(x);
	'		}
	'	}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		(Of T As PlatformManagedObject) java.util.List(Of T) getMXBeans(Class mxbeanInterface)
	'	{
	'		Return (List<T>) fetcher.getMXBeans();
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		(Of T As PlatformManagedObject) T getSingletonMXBean(Class mxbeanInterface)
	'	{
	'		if (!singleton)
	'			throw New IllegalArgumentException(mxbeanInterfaceName + " can have zero or more than one instances");
	'
	'		List<T> list = getMXBeans(mxbeanInterface);
	'		assert list.size() == 1;
	'		Return list.isEmpty() ? Nothing : list.get(0);
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		(Of T As PlatformManagedObject) T getSingletonMXBean(javax.management.MBeanServerConnection mbs, Class mxbeanInterface) throws java.io.IOException
	'	{
	'		if (!singleton)
	'			throw New IllegalArgumentException(mxbeanInterfaceName + " can have zero or more than one instances");
	'
	'		' ObjectName of a singleton MXBean contains only domain and type
	'		assert keyProperties.size() == 1;
	'		String on = domain + ":type=" + type;
	'		Return ManagementFactory.newPlatformMXBeanProxy(mbs, on, mxbeanInterface);
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		(Of T As PlatformManagedObject) java.util.List(Of T) getMXBeans(javax.management.MBeanServerConnection mbs, Class mxbeanInterface) throws java.io.IOException
	'	{
	'		List<T> result = New ArrayList<>();
	'		for (ObjectName on : getObjectNames(mbs))
	'		{
	'			result.add(ManagementFactory.newPlatformMXBeanProxy(mbs, on.getCanonicalName(), mxbeanInterface));
	'		}
	'		Return result;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private java.util.Set(Of javax.management.ObjectName) getObjectNames(javax.management.MBeanServerConnection mbs) throws java.io.IOException
	'	{
	'		String domainAndType = domain + ":type=" + type;
	'		if (keyProperties.size() > 1)
	'		{
	'			' if there are more than 1 key properties (i.e. other than "type")
	'			domainAndType += ",*";
	'		}
	'		ObjectName on = Util.newObjectName(domainAndType);
	'		Set<ObjectName> set = mbs.queryNames(on, Nothing);
	'		for (PlatformComponent pc : subComponents)
	'		{
	'			set.addAll(pc.getObjectNames(mbs));
	'		}
	'		Return set;
	'	}

		' a map from MXBean interface name to PlatformComponent
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static java.util.Map(Of String, PlatformComponent) enumMap;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static synchronized  Sub  ensureInitialized()
	'	{
	'		if (enumMap == Nothing)
	'		{
	'			enumMap = New HashMap<>();
	'			for (PlatformComponent pc: PlatformComponent.values())
	'			{
	'				' Use String as the key rather than Class<?> to avoid
	'				' causing unnecessary class loading of management interface
	'				enumMap.put(pc.getMXBeanInterfaceName(), pc);
	'			}
	'		}
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static boolean isPlatformMXBean(String cn)
	'	{
	'		ensureInitialized();
	'		Return enumMap.containsKey(cn);
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static (Of T As PlatformManagedObject) PlatformComponent getPlatformComponent(Class mxbeanInterface)
	'	{
	'		ensureInitialized();
	'		String cn = mxbeanInterface.getName();
	'		PlatformComponent pc = enumMap.get(cn);
	'		if (pc != Nothing && pc.getMXBeanInterface() == mxbeanInterface)
	'			Return pc;
	'		Return Nothing;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final long serialVersionUID = 6992337162326171013L;
	End Enum

End Namespace
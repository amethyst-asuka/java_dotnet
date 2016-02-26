Imports System
Imports System.Collections

'
' * Copyright (c) 2000, 2002, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.print



	''' <summary>
	''' Implementations of this class provide lookup services for
	''' print services (typically equivalent to printers) of a particular type.
	''' <p>
	''' Multiple implementations may be installed concurrently.
	''' All implementations must be able to describe the located printers
	''' as instances of a PrintService.
	''' Typically implementations of this service class are located
	''' automatically in JAR files (see the SPI JAR file specification).
	''' These classes must be instantiable using a default constructor.
	''' Alternatively applications may explicitly register instances
	''' at runtime.
	''' <p>
	''' Applications use only the static methods of this abstract class.
	''' The instance methods are implemented by a service provider in a subclass
	''' and the unification of the results from all installed lookup classes
	''' are reported by the static methods of this class when called by
	''' the application.
	''' <p>
	''' A PrintServiceLookup implementor is recommended to check for the
	''' SecurityManager.checkPrintJobAccess() to deny access to untrusted code.
	''' Following this recommended policy means that untrusted code may not
	''' be able to locate any print services. Downloaded applets are the most
	''' common example of untrusted code.
	''' <p>
	''' This check is made on a per lookup service basis to allow flexibility in
	''' the policy to reflect the needs of different lookup services.
	''' <p>
	''' Services which are registered by registerService(PrintService)
	''' will not be included in lookup results if a security manager is
	''' installed and its checkPrintJobAccess() method denies access.
	''' </summary>

	Public MustInherit Class PrintServiceLookup

		Friend Class Services
			Private listOfLookupServices As ArrayList = Nothing
			Private registeredServices As ArrayList = Nothing
		End Class

		Private Property Shared servicesForContext As Services
			Get
				Dim ___services As Services = CType(sun.awt.AppContext.appContext.get(GetType(Services)), Services)
				If ___services Is Nothing Then
					___services = New Services
					sun.awt.AppContext.appContext.put(GetType(Services), ___services)
				End If
				Return ___services
			End Get
		End Property

		Private Property Shared listOfLookupServices As ArrayList
			Get
				Return servicesForContext.listOfLookupServices
			End Get
		End Property

		Private Shared Function initListOfLookupServices() As ArrayList
			Dim ___listOfLookupServices As New ArrayList
			servicesForContext.listOfLookupServices = ___listOfLookupServices
			Return ___listOfLookupServices
		End Function


		Private Property Shared registeredServices As ArrayList
			Get
				Return servicesForContext.registeredServices
			End Get
		End Property

		Private Shared Function initRegisteredServices() As ArrayList
			Dim ___registeredServices As New ArrayList
			servicesForContext.registeredServices = ___registeredServices
			Return ___registeredServices
		End Function

		''' <summary>
		''' Locates print services capable of printing the specified
		''' <seealso cref="DocFlavor"/>.
		''' </summary>
		''' <param name="flavor"> the flavor to print. If null, this constraint is not
		'''        used. </param>
		''' <param name="attributes"> attributes that the print service must support.
		''' If null this constraint is not used.
		''' </param>
		''' <returns> array of matching <code>PrintService</code> objects
		''' representing print services that support the specified flavor
		''' attributes.  If no services match, the array is zero-length. </returns>
		Public Shared Function lookupPrintServices(ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As PrintService()
			Dim list As ArrayList = getServices(flavor, attributes)
			Return CType(list.ToArray(GetType(PrintService)), PrintService())
		End Function


		''' <summary>
		''' Locates MultiDoc print Services capable of printing MultiDocs
		''' containing all the specified doc flavors.
		''' <P> This method is useful to help locate a service that can print
		''' a <code>MultiDoc</code> in which the elements may be different
		''' flavors. An application could perform this itself by multiple lookups
		''' on each <code>DocFlavor</code> in turn and collating the results,
		''' but the lookup service may be able to do this more efficiently.
		''' </summary>
		''' <param name="flavors"> the flavors to print. If null or empty this
		'''        constraint is not used.
		''' Otherwise return only multidoc print services that can print all
		''' specified doc flavors. </param>
		''' <param name="attributes"> attributes that the print service must
		''' support.  If null this constraint is not used.
		''' </param>
		''' <returns> array of matching <seealso cref="MultiDocPrintService"/> objects.
		''' If no services match, the array is zero-length.
		'''  </returns>
		Public Shared Function lookupMultiDocPrintServices(ByVal flavors As DocFlavor(), ByVal attributes As javax.print.attribute.AttributeSet) As MultiDocPrintService()
			Dim list As ArrayList = getMultiDocServices(flavors, attributes)
			Return CType(list.ToArray(GetType(MultiDocPrintService)), MultiDocPrintService())
		End Function


		''' <summary>
		''' Locates the default print service for this environment.
		''' This may return null.
		''' If multiple lookup services each specify a default, the
		''' chosen service is not precisely defined, but a
		''' platform native service, rather than an installed service,
		''' is usually returned as the default.  If there is no clearly
		''' identifiable
		''' platform native default print service, the default is the first
		''' to be located in an implementation-dependent manner.
		''' <p>
		''' This may include making use of any preferences API that is available
		''' as part of the Java or native platform.
		''' This algorithm may be overridden by a user setting the property
		''' javax.print.defaultPrinter.
		''' A service specified must be discovered to be valid and currently
		''' available to be returned as the default.
		''' </summary>
		''' <returns> the default PrintService. </returns>

		Public Shared Function lookupDefaultPrintService() As PrintService

			Dim psIterator As IEnumerator = allLookupServices.GetEnumerator()
			Do While psIterator.hasNext()
				Try
					Dim lus As PrintServiceLookup = CType(psIterator.next(), PrintServiceLookup)
					Dim service As PrintService = lus.defaultPrintService
					If service IsNot Nothing Then Return service
				Catch e As Exception
				End Try
			Loop
			Return Nothing
		End Function


		''' <summary>
		''' Allows an application to explicitly register a class that
		''' implements lookup services. The registration will not persist
		''' across VM invocations.
		''' This is useful if an application needs to make a new service
		''' available that is not part of the installation.
		''' If the lookup service is already registered, or cannot be registered,
		''' the method returns false.
		''' <p>
		''' </summary>
		''' <param name="sp"> an implementation of a lookup service. </param>
		''' <returns> <code>true</code> if the new lookup service is newly
		'''         registered; <code>false</code> otherwise. </returns>
		Public Shared Function registerServiceProvider(ByVal sp As PrintServiceLookup) As Boolean
			SyncLock GetType(PrintServiceLookup)
				Dim psIterator As IEnumerator = allLookupServices.GetEnumerator()
				Do While psIterator.hasNext()
					Try
						Dim lus As Object = psIterator.next()
						If lus.GetType() Is sp.GetType() Then Return False
					Catch e As Exception
					End Try
				Loop
				listOfLookupServices.Add(sp)
				Return True
			End SyncLock

		End Function


		''' <summary>
		''' Allows an application to directly register an instance of a
		''' class which implements a print service.
		''' The lookup operations for this service will be
		''' performed by the PrintServiceLookup class using the attribute
		''' values and classes reported by the service.
		''' This may be less efficient than a lookup
		''' service tuned for that service.
		''' Therefore registering a <code>PrintServiceLookup</code> instance
		''' instead is recommended.
		''' The method returns true if this service is not previously
		''' registered and is now successfully registered.
		''' This method should not be called with StreamPrintService instances.
		''' They will always fail to register and the method will return false. </summary>
		''' <param name="service"> an implementation of a print service. </param>
		''' <returns> <code>true</code> if the service is newly
		'''         registered; <code>false</code> otherwise. </returns>

		Public Shared Function registerService(ByVal service As PrintService) As Boolean
			SyncLock GetType(PrintServiceLookup)
				If TypeOf service Is StreamPrintService Then Return False
				Dim ___registeredServices As ArrayList = registeredServices
				If ___registeredServices Is Nothing Then
					___registeredServices = initRegisteredServices()
				Else
				  If ___registeredServices.Contains(service) Then Return False
				End If
				___registeredServices.Add(service)
				Return True
			End SyncLock
		End Function


	   ''' <summary>
	   ''' Locates services that can be positively confirmed to support
	   ''' the combination of attributes and DocFlavors specified.
	   ''' This method is not called directly by applications.
	   ''' <p>
	   ''' Implemented by a service provider, used by the static methods
	   ''' of this class.
	   ''' <p>
	   ''' The results should be the same as obtaining all the PrintServices
	   ''' and querying each one individually on its support for the
	   ''' specified attributes and flavors, but the process can be more
	   ''' efficient by taking advantage of the capabilities of lookup services
	   ''' for the print services.
	   ''' </summary>
	   ''' <param name="flavor"> of document required.  If null it is ignored. </param>
	   ''' <param name="attributes"> required to be supported. If null this
	   ''' constraint is not used. </param>
	   ''' <returns> array of matching PrintServices. If no services match, the
	   ''' array is zero-length. </returns>
		Public MustOverride Function getPrintServices(ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As PrintService()

		''' <summary>
		''' Not called directly by applications.
		''' Implemented by a service provider, used by the static methods
		''' of this class. </summary>
		''' <returns> array of all PrintServices known to this lookup service
		''' class. If none are found, the array is zero-length. </returns>
		Public MustOverride ReadOnly Property printServices As PrintService()


	   ''' <summary>
	   ''' Not called directly by applications.
	   ''' <p>
	   ''' Implemented by a service provider, used by the static methods
	   ''' of this class.
	   ''' <p>
	   ''' Locates MultiDoc print services which can be positively confirmed
	   ''' to support the combination of attributes and DocFlavors specified.
	   ''' <p>
	   ''' </summary>
	   ''' <param name="flavors"> of documents required. If null or empty it is ignored. </param>
	   ''' <param name="attributes"> required to be supported. If null this
	   ''' constraint is not used. </param>
	   ''' <returns> array of matching PrintServices. If no services match, the
	   ''' array is zero-length. </returns>
		Public MustOverride Function getMultiDocPrintServices(ByVal flavors As DocFlavor(), ByVal attributes As javax.print.attribute.AttributeSet) As MultiDocPrintService()

		''' <summary>
		''' Not called directly by applications.
		''' Implemented by a service provider, and called by the print lookup
		''' service </summary>
		''' <returns> the default PrintService for this lookup service.
		''' If there is no default, returns null. </returns>
		Public MustOverride ReadOnly Property defaultPrintService As PrintService

		Private Property Shared allLookupServices As ArrayList
			Get
				SyncLock GetType(PrintServiceLookup)
					Dim ___listOfLookupServices As ArrayList = listOfLookupServices
					If ___listOfLookupServices IsNot Nothing Then
						Return ___listOfLookupServices
					Else
						___listOfLookupServices = initListOfLookupServices()
					End If
					Try
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'					java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction()
		'				{
		'						public Object run()
		'						{
		'							Iterator<PrintServiceLookup> iterator = ServiceLoader.load(PrintServiceLookup.class).iterator();
		'							ArrayList los = getListOfLookupServices();
		'							while (iterator.hasNext())
		'							{
		'								try
		'								{
		'									los.add(iterator.next());
		'								}
		'								catch (ServiceConfigurationError err)
		'								{
		'									' In the applet case, we continue 
		'									if (System.getSecurityManager() != Nothing)
		'									{
		'										err.printStackTrace();
		'									}
		'									else
		'									{
		'										throw err;
		'									}
		'								}
		'							}
		'							Return Nothing;
		'						}
		'				});
					Catch e As java.security.PrivilegedActionException
					End Try
    
					Return ___listOfLookupServices
				End SyncLock
			End Get
		End Property

		Private Shared Function getServices(ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As ArrayList

			Dim listOfServices As New ArrayList
			Dim psIterator As IEnumerator = allLookupServices.GetEnumerator()
			Do While psIterator.hasNext()
				Try
					Dim lus As PrintServiceLookup = CType(psIterator.next(), PrintServiceLookup)
					Dim ___services As PrintService()=Nothing
					If flavor Is Nothing AndAlso attributes Is Nothing Then
						Try
						___services = lus.printServices
						Catch tr As Exception
						End Try
					Else
						___services = lus.getPrintServices(flavor, attributes)
					End If
					If ___services Is Nothing Then Continue Do
					For i As Integer = 0 To ___services.Length - 1
						listOfServices.Add(___services(i))
					Next i
				Catch e As Exception
				End Try
			Loop
			' add any directly registered services 
			Dim ___registeredServices As ArrayList = Nothing
			Try
			  Dim security As SecurityManager = System.securityManager
			  If security IsNot Nothing Then security.checkPrintJobAccess()
			  ___registeredServices = registeredServices
			Catch se As SecurityException
			End Try
			If ___registeredServices IsNot Nothing Then
				Dim ___services As PrintService() = CType(___registeredServices.ToArray(GetType(PrintService)), PrintService())
				For i As Integer = 0 To ___services.Length - 1
					If Not listOfServices.Contains(___services(i)) Then
						If flavor Is Nothing AndAlso attributes Is Nothing Then
							listOfServices.Add(___services(i))
						ElseIf ((flavor IsNot Nothing AndAlso ___services(i).isDocFlavorSupported(flavor)) OrElse flavor Is Nothing) AndAlso Nothing Is ___services(i).getUnsupportedAttributes(flavor, attributes) Then
							listOfServices.Add(___services(i))
						End If
					End If
				Next i
			End If
			Return listOfServices
		End Function

		Private Shared Function getMultiDocServices(ByVal flavors As DocFlavor(), ByVal attributes As javax.print.attribute.AttributeSet) As ArrayList


			Dim listOfServices As New ArrayList
			Dim psIterator As IEnumerator = allLookupServices.GetEnumerator()
			Do While psIterator.hasNext()
				Try
					Dim lus As PrintServiceLookup = CType(psIterator.next(), PrintServiceLookup)
					Dim ___services As MultiDocPrintService() = lus.getMultiDocPrintServices(flavors, attributes)
					If ___services Is Nothing Then Continue Do
					For i As Integer = 0 To ___services.Length - 1
						listOfServices.Add(___services(i))
					Next i
				Catch e As Exception
				End Try
			Loop
			' add any directly registered services 
			Dim ___registeredServices As ArrayList = Nothing
			Try
			  Dim security As SecurityManager = System.securityManager
			  If security IsNot Nothing Then security.checkPrintJobAccess()
			  ___registeredServices = registeredServices
			Catch e As Exception
			End Try
			If ___registeredServices IsNot Nothing Then
				Dim ___services As PrintService() = CType(___registeredServices.ToArray(GetType(PrintService)), PrintService())
				For i As Integer = 0 To ___services.Length - 1
					If TypeOf ___services(i) Is MultiDocPrintService AndAlso (Not listOfServices.Contains(___services(i))) Then
						If flavors Is Nothing OrElse flavors.Length = 0 Then
							listOfServices.Add(___services(i))
						Else
							Dim supported As Boolean = True
							For f As Integer = 0 To flavors.Length - 1
								If ___services(i).isDocFlavorSupported(flavors(f)) Then

									If ___services(i).getUnsupportedAttributes(flavors(f), attributes) IsNot Nothing Then
											supported = False
											Exit For
									End If
								Else
									supported = False
									Exit For
								End If
							Next f
							If supported Then listOfServices.Add(___services(i))
						End If
					End If
				Next i
			End If
			Return listOfServices
		End Function

	End Class

End Namespace
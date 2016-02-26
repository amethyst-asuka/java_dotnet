Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.spi


	''' <summary>
	''' A superinterface for functionality common to all Image I/O service
	''' provider interfaces (SPIs).  For more information on service
	''' provider classes, see the class comment for the
	''' <code>IIORegistry</code> class.
	''' </summary>
	''' <seealso cref= IIORegistry </seealso>
	''' <seealso cref= javax.imageio.spi.ImageReaderSpi </seealso>
	''' <seealso cref= javax.imageio.spi.ImageWriterSpi </seealso>
	''' <seealso cref= javax.imageio.spi.ImageTranscoderSpi </seealso>
	''' <seealso cref= javax.imageio.spi.ImageInputStreamSpi
	'''  </seealso>
	Public MustInherit Class IIOServiceProvider
		Implements javax.imageio.spi.RegisterableService

			Public MustOverride Sub onDeregistration(ByVal registry As javax.imageio.spi.ServiceRegistry, ByVal category As Type)
			Public MustOverride Sub onRegistration(ByVal registry As javax.imageio.spi.ServiceRegistry, ByVal category As Type)

		''' <summary>
		''' A <code>String</code> to be returned from
		''' <code>getVendorName</code>, initially <code>null</code>.
		''' Constructors should set this to a non-<code>null</code> value.
		''' </summary>
		Protected Friend vendorName As String

		''' <summary>
		''' A <code>String</code> to be returned from
		''' <code>getVersion</code>, initially null.  Constructors should
		''' set this to a non-<code>null</code> value.
		''' </summary>
		Protected Friend version As String

		''' <summary>
		''' Constructs an <code>IIOServiceProvider</code> with a given
		''' vendor name and version identifier.
		''' </summary>
		''' <param name="vendorName"> the vendor name. </param>
		''' <param name="version"> a version identifier.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>vendorName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>version</code>
		''' is <code>null</code>. </exception>
		Public Sub New(ByVal vendorName As String, ByVal version As String)
			If vendorName Is Nothing Then Throw New System.ArgumentException("vendorName == null!")
			If version Is Nothing Then Throw New System.ArgumentException("version == null!")
			Me.vendorName = vendorName
			Me.version = version
		End Sub

		''' <summary>
		''' Constructs a blank <code>IIOServiceProvider</code>.  It is up
		''' to the subclass to initialize instance variables and/or
		''' override method implementations in order to ensure that the
		''' <code>getVendorName</code> and <code>getVersion</code> methods
		''' will return non-<code>null</code> values.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' A callback that will be called exactly once after the Spi class
		''' has been instantiated and registered in a
		''' <code>ServiceRegistry</code>.  This may be used to verify that
		''' the environment is suitable for this service, for example that
		''' native libraries can be loaded.  If the service cannot function
		''' in the environment where it finds itself, it should deregister
		''' itself from the registry.
		''' 
		''' <p> Only the registry should call this method.
		''' 
		''' <p> The default implementation does nothing.
		''' </summary>
		''' <seealso cref= ServiceRegistry#registerServiceProvider(Object provider) </seealso>
		Public Overridable Sub onRegistration(ByVal registry As javax.imageio.spi.ServiceRegistry, ByVal category As Type)
		End Sub

		''' <summary>
		''' A callback that will be whenever the Spi class has been
		''' deregistered from a <code>ServiceRegistry</code>.
		''' 
		''' <p> Only the registry should call this method.
		''' 
		''' <p> The default implementation does nothing.
		''' </summary>
		''' <seealso cref= ServiceRegistry#deregisterServiceProvider(Object provider) </seealso>
		Public Overridable Sub onDeregistration(ByVal registry As javax.imageio.spi.ServiceRegistry, ByVal category As Type)
		End Sub

		''' <summary>
		''' Returns the name of the vendor responsible for creating this
		''' service provider and its associated implementation.  Because
		''' the vendor name may be used to select a service provider,
		''' it is not localized.
		''' 
		''' <p> The default implementation returns the value of the
		''' <code>vendorName</code> instance variable.
		''' </summary>
		''' <returns> a non-<code>null</code> <code>String</code> containing
		''' the name of the vendor. </returns>
		Public Overridable Property vendorName As String
			Get
				Return vendorName
			End Get
		End Property

		''' <summary>
		''' Returns a string describing the version
		''' number of this service provider and its associated
		''' implementation.  Because the version may be used by transcoders
		''' to identify the service providers they understand, this method
		''' is not localized.
		''' 
		''' <p> The default implementation returns the value of the
		''' <code>version</code> instance variable.
		''' </summary>
		''' <returns> a non-<code>null</code> <code>String</code> containing
		''' the version of this service provider. </returns>
		Public Overridable Property version As String
			Get
				Return version
			End Get
		End Property

		''' <summary>
		''' Returns a brief, human-readable description of this service
		''' provider and its associated implementation.  The resulting
		''' string should be localized for the supplied
		''' <code>Locale</code>, if possible.
		''' </summary>
		''' <param name="locale"> a <code>Locale</code> for which the return value
		''' should be localized.
		''' </param>
		''' <returns> a <code>String</code> containing a description of this
		''' service provider. </returns>
		Public MustOverride Function getDescription(ByVal locale As java.util.Locale) As String
	End Class

End Namespace
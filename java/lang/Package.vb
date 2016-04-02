Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang






	''' <summary>
	''' {@code Package} objects contain version information
	''' about the implementation and specification of a Java package.
	''' This versioning information is retrieved and made available
	''' by the <seealso cref="ClassLoader"/> instance that
	''' loaded the class(es).  Typically, it is stored in the manifest that is
	''' distributed with the classes.
	''' 
	''' <p>The set of classes that make up the package may implement a
	''' particular specification and if so the specification title, version number,
	''' and vendor strings identify that specification.
	''' An application can ask if the package is
	''' compatible with a particular version, see the {@link
	''' #isCompatibleWith isCompatibleWith}
	''' method for details.
	''' 
	''' <p>Specification version numbers use a syntax that consists of nonnegative
	''' decimal integers separated by periods ".", for example "2.0" or
	''' "1.2.3.4.5.6.7".  This allows an extensible number to be used to represent
	''' major, minor, micro, etc. versions.  The version specification is described
	''' by the following formal grammar:
	''' <blockquote>
	''' <dl>
	''' <dt><i>SpecificationVersion:</i>
	''' <dd><i>Digits RefinedVersion<sub>opt</sub></i>
	''' 
	''' <dt><i>RefinedVersion:</i>
	''' <dd>{@code .} <i>Digits</i>
	''' <dd>{@code .} <i>Digits RefinedVersion</i>
	''' 
	''' <dt><i>Digits:</i>
	''' <dd><i>Digit</i>
	''' <dd><i>Digits</i>
	''' 
	''' <dt><i>Digit:</i>
	''' <dd>any character for which <seealso cref="Character#isDigit"/> returns {@code true},
	''' e.g. 0, 1, 2, ...
	''' </dl>
	''' </blockquote>
	''' 
	''' <p>The implementation title, version, and vendor strings identify an
	''' implementation and are made available conveniently to enable accurate
	''' reporting of the packages involved when a problem occurs. The contents
	''' all three implementation strings are vendor specific. The
	''' implementation version strings have no specified syntax and should
	''' only be compared for equality with desired version identifiers.
	''' 
	''' <p>Within each {@code ClassLoader} instance all classes from the same
	''' java package have the same Package object.  The static methods allow a package
	''' to be found by name or the set of all packages known to the current class
	''' loader to be found.
	''' </summary>
	''' <seealso cref= ClassLoader#definePackage </seealso>
	Public Class Package
		Implements java.lang.reflect.AnnotatedElement

		''' <summary>
		''' Return the name of this package.
		''' </summary>
		''' <returns>  The fully-qualified name of this package as defined in section 6.5.3 of
		'''          <cite>The Java&trade; Language Specification</cite>,
		'''          for example, {@code java.lang} </returns>
		Public Overridable Property name As String
			Get
				Return pkgName
			End Get
		End Property


		''' <summary>
		''' Return the title of the specification that this package implements. </summary>
		''' <returns> the specification title, null is returned if it is not known. </returns>
		Public Overridable Property specificationTitle As String
			Get
				Return specTitle
			End Get
		End Property

		''' <summary>
		''' Returns the version number of the specification
		''' that this package implements.
		''' This version string must be a sequence of nonnegative decimal
		''' integers separated by "."'s and may have leading zeros.
		''' When version strings are compared the most significant
		''' numbers are compared. </summary>
		''' <returns> the specification version, null is returned if it is not known. </returns>
		Public Overridable Property specificationVersion As String
			Get
				Return specVersion
			End Get
		End Property

		''' <summary>
		''' Return the name of the organization, vendor,
		''' or company that owns and maintains the specification
		''' of the classes that implement this package. </summary>
		''' <returns> the specification vendor, null is returned if it is not known. </returns>
		Public Overridable Property specificationVendor As String
			Get
				Return specVendor
			End Get
		End Property

		''' <summary>
		''' Return the title of this package. </summary>
		''' <returns> the title of the implementation, null is returned if it is not known. </returns>
		Public Overridable Property implementationTitle As String
			Get
				Return implTitle
			End Get
		End Property

		''' <summary>
		''' Return the version of this implementation. It consists of any string
		''' assigned by the vendor of this implementation and does
		''' not have any particular syntax specified or expected by the Java
		''' runtime. It may be compared for equality with other
		''' package version strings used for this implementation
		''' by this vendor for this package. </summary>
		''' <returns> the version of the implementation, null is returned if it is not known. </returns>
		Public Overridable Property implementationVersion As String
			Get
				Return implVersion
			End Get
		End Property

		''' <summary>
		''' Returns the name of the organization,
		''' vendor or company that provided this implementation. </summary>
		''' <returns> the vendor that implemented this package.. </returns>
		Public Overridable Property implementationVendor As String
			Get
				Return implVendor
			End Get
		End Property

		''' <summary>
		''' Returns true if this package is sealed.
		''' </summary>
		''' <returns> true if the package is sealed, false otherwise </returns>
		Public Overridable Property sealed As Boolean
			Get
				Return sealBase IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Returns true if this package is sealed with respect to the specified
		''' code source url.
		''' </summary>
		''' <param name="url"> the code source url </param>
		''' <returns> true if this package is sealed with respect to url </returns>
		Public Overridable Function isSealed(ByVal url As java.net.URL) As Boolean
			Return url.Equals(sealBase)
		End Function

		''' <summary>
		''' Compare this package's specification version with a
		''' desired version. It returns true if
		''' this packages specification version number is greater than or equal
		''' to the desired version number. <p>
		''' 
		''' Version numbers are compared by sequentially comparing corresponding
		''' components of the desired and specification strings.
		''' Each component is converted as a decimal integer and the values
		''' compared.
		''' If the specification value is greater than the desired
		''' value true is returned. If the value is less false is returned.
		''' If the values are equal the period is skipped and the next pair of
		''' components is compared.
		''' </summary>
		''' <param name="desired"> the version string of the desired version. </param>
		''' <returns> true if this package's version number is greater
		'''          than or equal to the desired version number
		''' </returns>
		''' <exception cref="NumberFormatException"> if the desired or current version
		'''          is not of the correct dotted form. </exception>
		Public Overridable Function isCompatibleWith(ByVal desired As String) As Boolean
			If specVersion Is Nothing OrElse specVersion.length() < 1 Then Throw New NumberFormatException("Empty version string")

			Dim sa As String() = specVersion.Split("\.", -1)
			Dim si As Integer() = New Integer(sa.Length - 1){}
			For i As Integer = 0 To sa.Length - 1
				si(i) = Convert.ToInt32(sa(i))
				If si(i) < 0 Then Throw NumberFormatException.forInputString("" & si(i))
			Next i

			Dim da As String() = desired.Split("\.", -1)
			Dim di As Integer() = New Integer(da.Length - 1){}
			For i As Integer = 0 To da.Length - 1
				di(i) = Convert.ToInt32(da(i))
				If di(i) < 0 Then Throw NumberFormatException.forInputString("" & di(i))
			Next i

			Dim len As Integer = System.Math.Max(di.Length, si.Length)
			For i As Integer = 0 To len - 1
				Dim d As Integer = (If(i < di.Length, di(i), 0))
				Dim s As Integer = (If(i < si.Length, si(i), 0))
				If s < d Then Return False
				If s > d Then Return True
			Next i
			Return True
		End Function

		''' <summary>
		''' Find a package by name in the callers {@code ClassLoader} instance.
		''' The callers {@code ClassLoader} instance is used to find the package
		''' instance corresponding to the named class. If the callers
		''' {@code ClassLoader} instance is null then the set of packages loaded
		''' by the system {@code ClassLoader} instance is searched to find the
		''' named package. <p>
		''' 
		''' Packages have attributes for versions and specifications only if the class
		''' loader created the package instance with the appropriate attributes. Typically,
		''' those attributes are defined in the manifests that accompany the classes.
		''' </summary>
		''' <param name="name"> a package name, for example, java.lang. </param>
		''' <returns> the package of the requested name. It may be null if no package
		'''          information is available from the archive or codebase. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getPackage(ByVal name As String) As Package
			Dim l As  ClassLoader = ClassLoader.getClassLoader(sun.reflect.Reflection.callerClass)
			If l IsNot Nothing Then
				Return l.getPackage(name)
			Else
				Return getSystemPackage(name)
			End If
		End Function

		''' <summary>
		''' Get all the packages currently known for the caller's {@code ClassLoader}
		''' instance.  Those packages correspond to classes loaded via or accessible by
		''' name to that {@code ClassLoader} instance.  If the caller's
		''' {@code ClassLoader} instance is the bootstrap {@code ClassLoader}
		''' instance, which may be represented by {@code null} in some implementations,
		''' only packages corresponding to classes loaded by the bootstrap
		''' {@code ClassLoader} instance will be returned.
		''' </summary>
		''' <returns> a new array of packages known to the callers {@code ClassLoader}
		''' instance.  An zero length array is returned if none are known. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		PublicShared ReadOnly Propertypackages As Package()
			Get
				Dim l As  ClassLoader = ClassLoader.getClassLoader(sun.reflect.Reflection.callerClass)
				If l IsNot Nothing Then
					Return l.packages
				Else
					Return systemPackages
				End If
			End Get
		End Property

		''' <summary>
		''' Get the package for the specified class.
		''' The class's class loader is used to find the package instance
		''' corresponding to the specified class. If the class loader
		''' is the bootstrap class loader, which may be represented by
		''' {@code null} in some implementations, then the set of packages
		''' loaded by the bootstrap class loader is searched to find the package.
		''' <p>
		''' Packages have attributes for versions and specifications only
		''' if the class loader created the package
		''' instance with the appropriate attributes. Typically those
		''' attributes are defined in the manifests that accompany
		''' the classes.
		''' </summary>
		''' <param name="c"> the class to get the package of. </param>
		''' <returns> the package of the class. It may be null if no package
		'''          information is available from the archive or codebase.   </returns>
		Shared Function getPackage(ByVal c As [Class]) As Package
			Dim name_Renamed As String = c.name
			Dim i As Integer = name_Renamed.LastIndexOf("."c)
			If i <> -1 Then
				name_Renamed = name_Renamed.Substring(0, i)
				Dim cl As  ClassLoader = c.classLoader
				If cl IsNot Nothing Then
					Return cl.getPackage(name_Renamed)
				Else
					Return getSystemPackage(name_Renamed)
				End If
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Return the hash code computed from the package name. </summary>
		''' <returns> the hash code computed from the package name. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return pkgName.GetHashCode()
		End Function

		''' <summary>
		''' Returns the string representation of this Package.
		''' Its value is the string "package " and the package name.
		''' If the package title is defined it is appended.
		''' If the package version is defined it is appended. </summary>
		''' <returns> the string representation of the package. </returns>
		Public Overrides Function ToString() As String
			Dim spec As String = specTitle
			Dim ver As String = specVersion
			If spec IsNot Nothing AndAlso spec.length() > 0 Then
				spec = ", " & spec
			Else
				spec = ""
			End If
			If ver IsNot Nothing AndAlso ver.length() > 0 Then
				ver = ", version " & ver
			Else
				ver = ""
			End If
			Return "package " & pkgName + spec + ver
		End Function

		Private Property packageInfo As  [Class]
			Get
				If packageInfo Is Nothing Then
					Try
					packageInfo = Type.GetType(pkgName & ".package-info", False, loader)
					Catch ex As  ClassNotFoundException
						' store a proxy for the package info that has no annotations
	'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
	'					class PackageInfoProxy
		'				{
		'				}
					packageInfo = GetType(PackageInfoProxy)
					End Try
				End If
				Return packageInfo
			End Get
		End Property

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overridable Function getAnnotation(Of A As Annotation)(ByVal annotationClass As [Class]) As A
			Return packageInfo.getAnnotation(annotationClass)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.5 </exception>
		Public Overrides Function isAnnotationPresent(ByVal annotationClass As [Class]) As Boolean
			Return outerInstance.isAnnotationPresent(annotationClass)
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getAnnotationsByType(Of A As Annotation)(ByVal annotationClass As [Class]) As A()
			Return packageInfo.getAnnotationsByType(annotationClass)
		End Function

		''' <summary>
		''' @since 1.5
		''' </summary>
		Public Overridable Property annotations As Annotation()
			Get
				Return packageInfo.GetCustomAttributes(True)
			End Get
		End Property

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getDeclaredAnnotation(Of A As Annotation)(ByVal annotationClass As [Class]) As A
			Return packageInfo.getDeclaredAnnotation(annotationClass)
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		Public Overrides Function getDeclaredAnnotationsByType(Of A As Annotation)(ByVal annotationClass As [Class]) As A()
			Return packageInfo.getDeclaredAnnotationsByType(annotationClass)
		End Function

		''' <summary>
		''' @since 1.5
		''' </summary>
		Public Overridable Property declaredAnnotations As Annotation()
			Get
				Return packageInfo.GetCustomAttributes(False)
			End Get
		End Property

		''' <summary>
		''' Construct a package instance with the specified version
		''' information. </summary>
		''' <param name="name"> the name of the package </param>
		''' <param name="spectitle"> the title of the specification </param>
		''' <param name="specversion"> the version of the specification </param>
		''' <param name="specvendor"> the organization that maintains the specification </param>
		''' <param name="impltitle"> the title of the implementation </param>
		''' <param name="implversion"> the version of the implementation </param>
		''' <param name="implvendor"> the organization that maintains the implementation </param>
		Friend Sub New(ByVal name As String, ByVal spectitle As String, ByVal specversion As String, ByVal specvendor As String, ByVal impltitle As String, ByVal implversion As String, ByVal implvendor As String, ByVal sealbase As java.net.URL, ByVal loader As  ClassLoader)
			pkgName = name
			Me.implTitle = impltitle
			Me.implVersion = implversion
			Me.implVendor = implvendor
			Me.specTitle = spectitle
			Me.specVersion = specversion
			Me.specVendor = specvendor
			Me.sealBase = sealbase
			Me.loader = loader
		End Sub

	'    
	'     * Construct a package using the attributes from the specified manifest.
	'     *
	'     * @param name the package name
	'     * @param man the optional manifest for the package
	'     * @param url the optional code source url for the package
	'     
		Private Sub New(ByVal name As String, ByVal man As java.util.jar.Manifest, ByVal url As java.net.URL, ByVal loader As  ClassLoader)
			Dim path As String = name.replace("."c, "/"c) & "/"
			Dim sealed_Renamed As String = Nothing
			Dim specTitle As String= Nothing
			Dim specVersion As String= Nothing
			Dim specVendor As String= Nothing
			Dim implTitle As String= Nothing
			Dim implVersion As String= Nothing
			Dim implVendor As String= Nothing
			Dim sealBase As java.net.URL= Nothing
			Dim attr As java.util.jar.Attributes = man.getAttributes(path)
			If attr IsNot Nothing Then
				specTitle = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_TITLE)
				specVersion = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VERSION)
				specVendor = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VENDOR)
				implTitle = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_TITLE)
				implVersion = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VERSION)
				implVendor = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VENDOR)
				sealed_Renamed = attr.getValue(java.util.jar.Attributes.Name.SEALED)
			End If
			attr = man.mainAttributes
			If attr IsNot Nothing Then
				If specTitle Is Nothing Then specTitle = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_TITLE)
				If specVersion Is Nothing Then specVersion = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VERSION)
				If specVendor Is Nothing Then specVendor = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VENDOR)
				If implTitle Is Nothing Then implTitle = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_TITLE)
				If implVersion Is Nothing Then implVersion = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VERSION)
				If implVendor Is Nothing Then implVendor = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VENDOR)
				If sealed_Renamed Is Nothing Then sealed_Renamed = attr.getValue(java.util.jar.Attributes.Name.SEALED)
			End If
			If "true".equalsIgnoreCase(sealed_Renamed) Then sealBase = url
			pkgName = name
			Me.specTitle = specTitle
			Me.specVersion = specVersion
			Me.specVendor = specVendor
			Me.implTitle = implTitle
			Me.implVersion = implVersion
			Me.implVendor = implVendor
			Me.sealBase = sealBase
			Me.loader = loader
		End Sub

	'    
	'     * Returns the loaded system package for the specified name.
	'     
		Shared Function getSystemPackage(ByVal name As String) As Package
			SyncLock pkgs
				Dim pkg As Package = pkgs(name)
				If pkg Is Nothing Then
					name = name.replace("."c, "/"c) & "/"
					Dim fn As String = getSystemPackage0(name)
					If fn IsNot Nothing Then pkg = defineSystemPackage(name, fn)
				End If
				Return pkg
			End SyncLock
		End Function

	'    
	'     * Return an array of loaded system packages.
	'     
		Shared systemPackages As Package()
			Get
				' First, update the system package map with new package names
				Dim names As String() = systemPackages0
				SyncLock pkgs
					For i As Integer = 0 To names.Length - 1
						defineSystemPackage(names(i), getSystemPackage0(names(i)))
					Next i
					Return pkgs.Values.ToArray(New Package(pkgs.Count - 1){})
				End SyncLock
			End Get
		End Property

		Private Shared Function defineSystemPackage(ByVal iname As String, ByVal fn As String) As Package
			Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Package
				Dim name As String = iname
				' Get the cached code source url for the file name
				Dim url As java.net.URL = urls(fn)
				If url Is Nothing Then
					' URL not found, so create one
					Dim file As New File(fn)
					Try
						url = sun.net.www.ParseUtil.fileToEncodedURL(file)
					Catch e As java.net.MalformedURLException
					End Try
					If url IsNot Nothing Then
						urls(fn) = url
						' If loading a JAR file, then also cache the manifest
						If file.file Then mans(fn) = loadManifest(fn)
					End If
				End If
				' Convert to "."-separated package name
				name = name.Substring(0, name.length() - 1).Replace("/"c, "."c)
				Dim pkg As Package
				Dim man As java.util.jar.Manifest = mans(fn)
				If man IsNot Nothing Then
					pkg = New Package(name, man, url, Nothing)
				Else
					pkg = New Package(name, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
				End If
				pkgs(name) = pkg
				Return pkg
			End Function
		End Class

	'    
	'     * Returns the Manifest for the specified JAR file name.
	'     
		Private Shared Function loadManifest(ByVal fn As String) As java.util.jar.Manifest
			Using fis As New java.io.FileInputStream(fn), jis As New java.util.jar.JarInputStream(fis, False)
					Try
					Return jis.manifest
				Catch e As java.io.IOException
					Return Nothing
				End Try
			End Using
		End Function

		' The map of loaded system packages
		Private Shared pkgs As IDictionary(Of String, Package) = New Dictionary(Of String, Package)(31)

		' Maps each directory or zip file name to its corresponding url
		Private Shared urls As IDictionary(Of String, java.net.URL) = New Dictionary(Of String, java.net.URL)(10)

		' Maps each code source url for a jar file to its manifest
		Private Shared mans As IDictionary(Of String, java.util.jar.Manifest) = New Dictionary(Of String, java.util.jar.Manifest)(10)

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getSystemPackage0(ByVal name As String) As String
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getSystemPackages0() As String()
		End Function

	'    
	'     * Private storage for the package name and attributes.
	'     
		Private ReadOnly pkgName As String
		Private ReadOnly specTitle As String
		Private ReadOnly specVersion As String
		Private ReadOnly specVendor As String
		Private ReadOnly implTitle As String
		Private ReadOnly implVersion As String
		Private ReadOnly implVendor As String
		Private ReadOnly sealBase As java.net.URL
		<NonSerialized> _
		Private ReadOnly loader As  ClassLoader
		<NonSerialized> _
		Private packageInfo As  [Class]
	End Class

End Namespace
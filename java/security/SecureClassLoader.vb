Imports System.Collections.Generic

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

Namespace java.security



	''' <summary>
	''' This class extends ClassLoader with additional support for defining
	''' classes with an associated code source and permissions which are
	''' retrieved by the system policy by default.
	''' 
	''' @author  Li Gong
	''' @author  Roland Schemers
	''' </summary>
	Public Class SecureClassLoader
		Inherits ClassLoader

	'    
	'     * If initialization succeed this is set to true and security checks will
	'     * succeed. Otherwise the object is not initialized and the object is
	'     * useless.
	'     
		Private ReadOnly initialized As Boolean

		' HashMap that maps CodeSource to ProtectionDomain
		' @GuardedBy("pdcache")
		Private ReadOnly pdcache As New Dictionary(Of CodeSource, ProtectionDomain)(11)

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("scl")

		Shared Sub New()
			ClassLoader.registerAsParallelCapable()
		End Sub

		''' <summary>
		''' Creates a new SecureClassLoader using the specified parent
		''' class loader for delegation.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls the security manager's {@code checkCreateClassLoader}
		''' method  to ensure creation of a class loader is allowed.
		''' <p> </summary>
		''' <param name="parent"> the parent ClassLoader </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkCreateClassLoader} method doesn't allow
		'''             creation of a class loader. </exception>
		''' <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		Protected Friend Sub New(  parent As  ClassLoader)
			MyBase.New(parent)
			' this is to make the stack depth consistent with 1.1
			Dim security_Renamed As SecurityManager = System.securityManager
			If security_Renamed IsNot Nothing Then security_Renamed.checkCreateClassLoader()
			initialized = True
		End Sub

		''' <summary>
		''' Creates a new SecureClassLoader using the default parent class
		''' loader for delegation.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls the security manager's {@code checkCreateClassLoader}
		''' method  to ensure creation of a class loader is allowed.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkCreateClassLoader} method doesn't allow
		'''             creation of a class loader. </exception>
		''' <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		Protected Friend Sub New()
			MyBase.New()
			' this is to make the stack depth consistent with 1.1
			Dim security_Renamed As SecurityManager = System.securityManager
			If security_Renamed IsNot Nothing Then security_Renamed.checkCreateClassLoader()
			initialized = True
		End Sub

		''' <summary>
		''' Converts an array of bytes into an instance of class [Class],
		''' with an optional CodeSource. Before the
		''' class can be used it must be resolved.
		''' <p>
		''' If a non-null CodeSource is supplied a ProtectionDomain is
		''' constructed and associated with the class being defined.
		''' <p> </summary>
		''' <param name="name"> the expected name of the [Class], or {@code null}
		'''                  if not known, using '.' and not '/' as the separator
		'''                  and without a trailing ".class" suffix. </param>
		''' <param name="b">    the bytes that make up the class data. The bytes in
		'''             positions {@code off} through {@code off+len-1}
		'''             should have the format of a valid class file as defined by
		'''             <cite>The Java&trade; Virtual Machine Specification</cite>. </param>
		''' <param name="off">  the start offset in {@code b} of the class data </param>
		''' <param name="len">  the length of the class data </param>
		''' <param name="cs">   the associated CodeSource, or {@code null} if none </param>
		''' <returns> the {@code Class} object created from the data,
		'''         and optional CodeSource. </returns>
		''' <exception cref="ClassFormatError"> if the data did not contain a valid class </exception>
		''' <exception cref="IndexOutOfBoundsException"> if either {@code off} or
		'''             {@code len} is negative, or if
		'''             {@code off+len} is greater than {@code b.length}.
		''' </exception>
		''' <exception cref="SecurityException"> if an attempt is made to add this class
		'''             to a package that contains classes that were signed by
		'''             a different set of certificates than this [Class], or if
		'''             the class name begins with "java.". </exception>
		Protected Friend Function defineClass(  name As String,   b As SByte(),   [off] As Integer,   len As Integer,   cs As CodeSource) As  [Class]
			Return defineClass(name, b, [off], len, getProtectionDomain(cs))
		End Function

		''' <summary>
		''' Converts a <seealso cref="java.nio.ByteBuffer ByteBuffer"/>
		''' into an instance of class {@code Class}, with an optional CodeSource.
		''' Before the class can be used it must be resolved.
		''' <p>
		''' If a non-null CodeSource is supplied a ProtectionDomain is
		''' constructed and associated with the class being defined.
		''' <p> </summary>
		''' <param name="name"> the expected name of the [Class], or {@code null}
		'''                  if not known, using '.' and not '/' as the separator
		'''                  and without a trailing ".class" suffix. </param>
		''' <param name="b">    the bytes that make up the class data.  The bytes from positions
		'''                  {@code b.position()} through {@code b.position() + b.limit() -1}
		'''                  should have the format of a valid class file as defined by
		'''                  <cite>The Java&trade; Virtual Machine Specification</cite>. </param>
		''' <param name="cs">   the associated CodeSource, or {@code null} if none </param>
		''' <returns> the {@code Class} object created from the data,
		'''         and optional CodeSource. </returns>
		''' <exception cref="ClassFormatError"> if the data did not contain a valid class </exception>
		''' <exception cref="SecurityException"> if an attempt is made to add this class
		'''             to a package that contains classes that were signed by
		'''             a different set of certificates than this [Class], or if
		'''             the class name begins with "java.".
		''' 
		''' @since  1.5 </exception>
		Protected Friend Function defineClass(  name As String,   b As java.nio.ByteBuffer,   cs As CodeSource) As  [Class]
			Return defineClass(name, b, getProtectionDomain(cs))
		End Function

		''' <summary>
		''' Returns the permissions for the given CodeSource object.
		''' <p>
		''' This method is invoked by the defineClass method which takes
		''' a CodeSource as an argument when it is constructing the
		''' ProtectionDomain for the class being defined.
		''' <p> </summary>
		''' <param name="codesource"> the codesource.
		''' </param>
		''' <returns> the permissions granted to the codesource.
		'''  </returns>
		Protected Friend Overridable Function getPermissions(  codesource As CodeSource) As PermissionCollection
			check()
			Return New Permissions ' ProtectionDomain defers the binding
		End Function

	'    
	'     * Returned cached ProtectionDomain for the specified CodeSource.
	'     
		Private Function getProtectionDomain(  cs As CodeSource) As ProtectionDomain
			If cs Is Nothing Then Return Nothing

			Dim pd As ProtectionDomain = Nothing
			SyncLock pdcache
				pd = pdcache(cs)
				If pd Is Nothing Then
					Dim perms As PermissionCollection = getPermissions(cs)
					pd = New ProtectionDomain(cs, perms, Me, Nothing)
					pdcache(cs) = pd
					If debug IsNot Nothing Then
						debug.println(" getPermissions " & pd)
						debug.println("")
					End If
				End If
			End SyncLock
			Return pd
		End Function

	'    
	'     * Check to make sure the class loader has been initialized.
	'     
		Private Sub check()
			If Not initialized Then Throw New SecurityException("ClassLoader object not initialized")
		End Sub

	End Class

End Namespace
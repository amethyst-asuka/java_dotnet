Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' The {@code Compiler} class is provided to support Java-to-native-code
	''' compilers and related services. By design, the {@code Compiler} class does
	''' nothing; it serves as a placeholder for a JIT compiler implementation.
	''' 
	''' <p> When the Java Virtual Machine first starts, it determines if the system
	''' property {@code java.compiler} exists. (System properties are accessible
	''' through <seealso cref="System#getProperty(String)"/> and {@link
	''' System#getProperty(String, String)}.  If so, it is assumed to be the name of
	''' a library (with a platform-dependent exact location and type); {@link
	''' System#loadLibrary} is called to load that library. If this loading
	''' succeeds, the function named {@code java_lang_Compiler_start()} in that
	''' library is called.
	''' 
	''' <p> If no compiler is available, these methods do nothing.
	''' 
	''' @author  Frank Yellin
	''' @since   JDK1.0
	''' </summary>
	Public NotInheritable Class Compiler
		Private Sub New() ' don't make instances
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initialize()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub registerNatives()
		End Sub

		Shared Sub New()
			registerNatives()
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				Dim loaded As Boolean = False
				Dim jit As String = System.getProperty("java.compiler")
				If (jit IsNot Nothing) AndAlso ((Not jit.Equals("NONE"))) AndAlso ((Not jit.Equals(""))) Then
					Try
'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
'						System.loadLibrary(jit)
						initialize()
						loaded = True
					Catch e As UnsatisfiedLinkError
						Console.Error.WriteLine("Warning: JIT compiler """ & jit & """ not found. Will use interpreter.")
					End Try
				End If
				Dim info As String = System.getProperty("java.vm.info")
				If loaded Then
					System.propertyrty("java.vm.info", info & ", " & jit)
				Else
					System.propertyrty("java.vm.info", info & ", nojit")
				End If
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Compiles the specified class.
		''' </summary>
		''' <param name="clazz">
		'''         A class
		''' </param>
		''' <returns>  {@code true} if the compilation succeeded; {@code false} if the
		'''          compilation failed or no compiler is available
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code clazz} is {@code null} </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function compileClass(ByVal clazz As Class) As Boolean
		End Function

		''' <summary>
		''' Compiles all classes whose name matches the specified string.
		''' </summary>
		''' <param name="string">
		'''         The name of the classes to compile
		''' </param>
		''' <returns>  {@code true} if the compilation succeeded; {@code false} if the
		'''          compilation failed or no compiler is available
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code string} is {@code null} </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function compileClasses(ByVal [string] As String) As Boolean
		End Function

		''' <summary>
		''' Examines the argument type and its fields and perform some documented
		''' operation.  No specific operations are required.
		''' </summary>
		''' <param name="any">
		'''         An argument
		''' </param>
		''' <returns>  A compiler-specific value, or {@code null} if no compiler is
		'''          available
		''' </returns>
		''' <exception cref="NullPointerException">
		'''          If {@code any} is {@code null} </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function command(ByVal any As Object) As Object
		End Function

		''' <summary>
		''' Cause the Compiler to resume operation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub enable()
		End Sub

		''' <summary>
		''' Cause the Compiler to cease operation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub disable()
		End Sub
	End Class

End Namespace
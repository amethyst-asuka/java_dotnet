Imports System.Collections.Generic

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.jar


	Friend Class JavaUtilJarAccessImpl
		Implements sun.misc.JavaUtilJarAccess

		Public Overridable Function jarFileHasClassPathAttribute(ByVal jar As JarFile) As Boolean
			Return jar.hasClassPathAttribute()
		End Function

		Public Overridable Function getCodeSources(ByVal jar As JarFile, ByVal url As java.net.URL) As java.security.CodeSource()
			Return jar.getCodeSources(url)
		End Function

		Public Overridable Function getCodeSource(ByVal jar As JarFile, ByVal url As java.net.URL, ByVal name As String) As java.security.CodeSource
			Return jar.getCodeSource(url, name)
		End Function

		Public Overridable Function entryNames(ByVal jar As JarFile, ByVal cs As java.security.CodeSource()) As System.Collections.IEnumerator(Of String)
			Return jar.entryNames(cs)
		End Function

		Public Overridable Function entries2(ByVal jar As JarFile) As System.Collections.IEnumerator(Of JarEntry)
			Return jar.entries2()
		End Function

		Public Overridable Sub setEagerValidation(ByVal jar As JarFile, ByVal eager As Boolean)
			jar.eagerValidation = eager
		End Sub

		Public Overridable Function getManifestDigests(ByVal jar As JarFile) As IList(Of Object)
			Return jar.manifestDigests
		End Function
	End Class

End Namespace
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

		Public Overridable Function jarFileHasClassPathAttribute(  jar As JarFile) As Boolean
			Return jar.hasClassPathAttribute()
		End Function

		Public Overridable Function getCodeSources(  jar As JarFile,   url As java.net.URL) As java.security.CodeSource()
			Return jar.getCodeSources(url)
		End Function

		Public Overridable Function getCodeSource(  jar As JarFile,   url As java.net.URL,   name As String) As java.security.CodeSource
			Return jar.getCodeSource(url, name)
		End Function

		Public Overridable Function entryNames(  jar As JarFile,   cs As java.security.CodeSource()) As System.Collections.IEnumerator(Of String)
			Return jar.entryNames(cs)
		End Function

		Public Overridable Function entries2(  jar As JarFile) As System.Collections.IEnumerator(Of JarEntry)
			Return jar.entries2()
		End Function

		Public Overridable Sub setEagerValidation(  jar As JarFile,   eager As Boolean)
			jar.eagerValidation = eager
		End Sub

		Public Overridable Function getManifestDigests(  jar As JarFile) As IList(Of Object)
			Return jar.manifestDigests
		End Function
	End Class

End Namespace
Imports System

'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright IBM Corp. 1993 - 1997 - All Rights Reserved
' *
' * The original version of this source code and documentation is
' * copyrighted and owned by IBM, Inc. These materials are provided under
' * terms of a License Agreement between IBM and Sun. This technology is
' * protected by multiple US and International patents. This notice and
' * attribution to IBM may not be removed.
' *
' 


Namespace javax.rmi.CORBA



	Friend Class GetORBPropertiesFileAction
		Implements java.security.PrivilegedAction

		Private debug As Boolean = False

		Public Sub New()
		End Sub

		Private Function getSystemProperty(ByVal name As String) As String
			' This will not throw a SecurityException because this
			' class was loaded from rt.jar using the bootstrap classloader.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			String propValue = (String) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'		{
	'				public java.lang.Object run()
	'				{
	'					Return System.getProperty(name);
	'				}
	'			}
		   )

			Return propValue
		End Function

		Private Sub getPropertiesFromFile(ByVal props As java.util.Properties, ByVal fileName As String)
			Try
				Dim file As New File(fileName)
				If Not file.exists() Then Return

				Dim [in] As New java.io.FileInputStream(file)

				Try
					props.load([in])
				Finally
					[in].close()
				End Try
			Catch exc As Exception
				If debug Then Console.WriteLine("ORB properties file " & fileName & " not found: " & exc)
			End Try
		End Sub

		Public Overridable Function run() As Object
			Dim defaults As New java.util.Properties

			Dim javaHome As String = getSystemProperty("java.home")
			Dim fileName As String = javaHome + File.separator & "lib" & File.separator & "orb.properties"

			getPropertiesFromFile(defaults, fileName)

			Dim results As New java.util.Properties(defaults)

			Dim userHome As String = getSystemProperty("user.home")
			fileName = userHome + File.separator & "orb.properties"

			getPropertiesFromFile(results, fileName)
			Return results
		End Function
	End Class

End Namespace
'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.datatype


	''' <summary>
	''' This class is duplicated for each JAXP subpackage so keep it in sync.
	''' It is package private and therefore is not exposed as part of the JAXP
	''' API.
	''' 
	''' Security related methods that only work on J2SE 1.2 and newer.
	''' </summary>
	Friend Class SecuritySupport


		Friend Overridable Property contextClassLoader As ClassLoader
			Get
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return (ClassLoader) AccessController.doPrivileged(New PrivilegedAction()
		'		{
		'			public Object run()
		'			{
		'				ClassLoader cl = Nothing;
		'				try
		'				{
		'					cl = Thread.currentThread().getContextClassLoader();
		'				}
		'				catch (SecurityException ex)
		'				{
		'				}
		'				Return cl;
		'			}
		'		});
			End Get
		End Property

		Friend Overridable Function getSystemProperty(ByVal propName As String) As String
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return (String) AccessController.doPrivileged(New PrivilegedAction()
	'		{
	'				public Object run()
	'				{
	'					Return System.getProperty(propName);
	'				}
	'			});
		End Function

		Friend Overridable Function getFileInputStream(ByVal file As File) As FileInputStream
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return (FileInputStream) AccessController.doPrivileged(New PrivilegedExceptionAction()
	'			{
	'					public Object run() throws FileNotFoundException
	'					{
	'						Return New FileInputStream(file);
	'					}
	'				});
			Catch e As PrivilegedActionException
				Throw CType(e.exception, FileNotFoundException)
			End Try
		End Function

		Friend Overridable Function getResourceAsStream(ByVal cl As ClassLoader, ByVal name As String) As InputStream
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return (InputStream) AccessController.doPrivileged(New PrivilegedAction()
	'		{
	'				public Object run()
	'				{
	'					InputStream ris;
	'					if (cl == Nothing)
	'					{
	'						ris = Object.class.getResourceAsStream(name);
	'					}
	'					else
	'					{
	'						ris = cl.getResourceAsStream(name);
	'					}
	'					Return ris;
	'				}
	'			});
		End Function

		Friend Overridable Function doesFileExist(ByVal f As File) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		Return ((java.lang.Boolean) AccessController.doPrivileged(New PrivilegedAction()
	'	{
	'				public Object run()
	'				{
	'					Return New java.lang.Boolean(f.exists());
	'				}
	'			})).booleanValue();
		End Function

	End Class

End Namespace
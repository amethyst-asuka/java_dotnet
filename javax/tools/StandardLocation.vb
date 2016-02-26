'
' * Copyright (c) 2006, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.tools



	''' <summary>
	''' Standard locations of file objects.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
	Public Enum StandardLocation

		''' <summary>
		''' Location of new class files.
		''' </summary>
		CLASS_OUTPUT

		''' <summary>
		''' Location of new source files.
		''' </summary>
		SOURCE_OUTPUT

		''' <summary>
		''' Location to search for user class files.
		''' </summary>
		CLASS_PATH

		''' <summary>
		''' Location to search for existing source files.
		''' </summary>
		SOURCE_PATH

		''' <summary>
		''' Location to search for annotation processors.
		''' </summary>
		ANNOTATION_PROCESSOR_PATH

		''' <summary>
		''' Location to search for platform classes.  Sometimes called
		''' the boot class path.
		''' </summary>
		PLATFORM_CLASS_PATH

		''' <summary>
		''' Location of new native header files.
		''' @since 1.8
		''' </summary>
		NATIVE_HEADER_OUTPUT

		''' <summary>
		''' Gets a location object with the given name.  The following
		''' property must hold: {@code locationFor(x) ==
		''' locationFor(y)} if and only if {@code x.equals(y)}.
		''' The returned location will be an output location if and only if
		''' name ends with {@code "_OUTPUT"}.
		''' </summary>
		''' <param name="name"> a name </param>
		''' <returns> a location </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static javax.tools.JavaFileManager.Location locationFor(final String name)
	'	{
	'		if (locations.isEmpty())
	'		{
	'			' can't use valueOf which throws IllegalArgumentException
	'			for (Location location : values())
	'				locations.putIfAbsent(location.getName(), location);
	'		}
	'		locations.putIfAbsent(name.toString(), New Location() ' null-check
	'		{
	'				public String getName()
	'				{
	'					Return name;
	'				}
	'				public boolean isOutputLocation()
	'				{
	'					Return name.endsWith("_OUTPUT");
	'				}
	'			});
	'		Return locations.get(name);
	'	}
		'where
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private static final ConcurrentMap(Of String, javax.tools.JavaFileManager.Location) locations = New ConcurrentHashMap(Of String, javax.tools.JavaFileManager.Location)();

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public String getName()
	'	{
	'		Return name();
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isOutputLocation()
	'	{
	'		switch (Me)
	'		{
	'			case CLASS_OUTPUT:
	'			case SOURCE_OUTPUT:
	'			case NATIVE_HEADER_OUTPUT:
	'				Return True;
	'			default:
	'				Return False;
	'		}
	'	}
	End Enum

End Namespace
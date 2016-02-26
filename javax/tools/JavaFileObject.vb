'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' File abstraction for tools operating on Java&trade; programming language
	''' source and class files.
	''' 
	''' <p>All methods in this interface might throw a SecurityException if
	''' a security exception occurs.
	''' 
	''' <p>Unless explicitly allowed, all methods in this interface might
	''' throw a NullPointerException if given a {@code null} argument.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @author Jonathan Gibbons </summary>
	''' <seealso cref= JavaFileManager
	''' @since 1.6 </seealso>
	Public Interface JavaFileObject
		Inherits FileObject

		''' <summary>
		''' Kinds of JavaFileObjects.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		enum Kind
	'	{
	'		''' <summary>
	'		''' Source files written in the Java programming language.  For
	'		''' example, regular files ending with {@code .java}.
	'		''' </summary>
	'		SOURCE(".java"),
	'
	'		''' <summary>
	'		''' Class files for the Java Virtual Machine.  For example,
	'		''' regular files ending with {@code .class}.
	'		''' </summary>
	'		CLASS(".class"),
	'
	'		''' <summary>
	'		''' HTML files.  For example, regular files ending with {@code
	'		''' .html}.
	'		''' </summary>
	'		HTML(".html"),
	'
	'		''' <summary>
	'		''' Any other kind.
	'		''' </summary>
	'		OTHER("");
	'		''' <summary>
	'		''' The extension which (by convention) is normally used for
	'		''' this kind of file object.  If no convention exists, the
	'		''' empty string ({@code ""}) is used.
	'		''' </summary>
	'		public final String extension;
	'		private Kind(String extension)
	'		{
	'			extension.getClass(); ' null check
	'			Me.extension = extension;
	'		}
	'	};

		''' <summary>
		''' Gets the kind of this file object.
		''' </summary>
		''' <returns> the kind </returns>
		ReadOnly Property kind As Kind

		''' <summary>
		''' Checks if this file object is compatible with the specified
		''' simple name and kind.  A simple name is a single identifier
		''' (not qualified) as defined in
		''' <cite>The Java&trade; Language Specification</cite>,
		''' section 6.2 "Names and Identifiers".
		''' </summary>
		''' <param name="simpleName"> a simple name of a class </param>
		''' <param name="kind"> a kind </param>
		''' <returns> {@code true} if this file object is compatible; false
		''' otherwise </returns>
		Function isNameCompatible(ByVal simpleName As String, ByVal kind As Kind) As Boolean

		''' <summary>
		''' Provides a hint about the nesting level of the class
		''' represented by this file object.  This method may return
		''' <seealso cref="NestingKind#MEMBER"/> to mean
		''' <seealso cref="NestingKind#LOCAL"/> or <seealso cref="NestingKind#ANONYMOUS"/>.
		''' If the nesting level is not known or this file object does not
		''' represent a class file this method returns {@code null}.
		''' </summary>
		''' <returns> the nesting kind, or {@code null} if the nesting kind
		''' is not known </returns>
		ReadOnly Property nestingKind As javax.lang.model.element.NestingKind

		''' <summary>
		''' Provides a hint about the access level of the class represented
		''' by this file object.  If the access level is not known or if
		''' this file object does not represent a class file this method
		''' returns {@code null}.
		''' </summary>
		''' <returns> the access level </returns>
		ReadOnly Property accessLevel As javax.lang.model.element.Modifier

	End Interface

End Namespace
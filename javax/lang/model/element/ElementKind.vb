'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model.element

	''' <summary>
	''' The {@code kind} of an element.
	''' 
	''' <p>Note that it is possible additional element kinds will be added
	''' to accommodate new, currently unknown, language structures added to
	''' future versions of the Java&trade; programming language.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= Element
	''' @since 1.6 </seealso>
	Public Enum ElementKind

		''' <summary>
		''' A package. </summary>
		PACKAGE

		' Declared types
		''' <summary>
		''' An enum type. </summary>
		[ENUM]
		''' <summary>
		''' A class not described by a more specific kind (like {@code ENUM}). </summary>
		[CLASS]
		''' <summary>
		''' An annotation type. </summary>
		ANNOTATION_TYPE
		''' <summary>
		''' An interface not described by a more specific kind (like
		''' {@code ANNOTATION_TYPE}).
		''' </summary>
		[INTERFACE]

		' Variables
		''' <summary>
		''' An enum constant. </summary>
		ENUM_CONSTANT
		''' <summary>
		''' A field not described by a more specific kind (like
		''' {@code ENUM_CONSTANT}).
		''' </summary>
		FIELD
		''' <summary>
		''' A parameter of a method or constructor. </summary>
		PARAMETER
		''' <summary>
		''' A local variable. </summary>
		LOCAL_VARIABLE
		''' <summary>
		''' A parameter of an exception handler. </summary>
		EXCEPTION_PARAMETER

		' Executables
		''' <summary>
		''' A method. </summary>
		METHOD
		''' <summary>
		''' A constructor. </summary>
		CONSTRUCTOR
		''' <summary>
		''' A static initializer. </summary>
		STATIC_INIT
		''' <summary>
		''' An instance initializer. </summary>
		INSTANCE_INIT

		''' <summary>
		''' A type parameter. </summary>
		TYPE_PARAMETER

		''' <summary>
		''' An implementation-reserved element.  This is not the element
		''' you are looking for.
		''' </summary>
		OTHER

		''' <summary>
		''' A resource variable.
		''' @since 1.7
		''' </summary>
		RESOURCE_VARIABLE


		''' <summary>
		''' Returns {@code true} if this is a kind of class:
		''' either {@code CLASS} or {@code ENUM}.
		''' </summary>
		''' <returns> {@code true} if this is a kind of class </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isClass()
	'	{
	'		Return Me == CLASS || Me == ENUM;
	'	}

		''' <summary>
		''' Returns {@code true} if this is a kind of interface:
		''' either {@code INTERFACE} or {@code ANNOTATION_TYPE}.
		''' </summary>
		''' <returns> {@code true} if this is a kind of interface </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isInterface()
	'	{
	'		Return Me == INTERFACE || Me == ANNOTATION_TYPE;
	'	}

		''' <summary>
		''' Returns {@code true} if this is a kind of field:
		''' either {@code FIELD} or {@code ENUM_CONSTANT}.
		''' </summary>
		''' <returns> {@code true} if this is a kind of field </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isField()
	'	{
	'		Return Me == FIELD || Me == ENUM_CONSTANT;
	'	}
	End Enum

End Namespace
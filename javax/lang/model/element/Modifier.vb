'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents a modifier on a program element such
	''' as a class, method, or field.
	''' 
	''' <p>Not all modifiers are applicable to all kinds of elements.
	''' When two or more modifiers appear in the source code of an element
	''' then it is customary, though not required, that they appear in the same
	''' order as the constants listed in the detail section below.
	''' 
	''' <p>Note that it is possible additional modifiers will be added in
	''' future versions of the platform.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>

	Public Enum Modifier

		' See JLS sections 8.1.1, 8.3.1, 8.4.3, 8.8.3, and 9.1.1.
		' java.lang.reflect.Modifier includes INTERFACE, but that's a VMism.

		''' <summary>
		''' The modifier {@code public} </summary>
			  [PUBLIC]
		''' <summary>
		''' The modifier {@code protected} </summary>
		   [PROTECTED]
		''' <summary>
		''' The modifier {@code private} </summary>
			 [PRIVATE]
		''' <summary>
		''' The modifier {@code abstract} </summary>
			ABSTRACT
		''' <summary>
		''' The modifier {@code default}
		''' @since 1.8
		''' </summary>
		 [DEFAULT]
		''' <summary>
		''' The modifier {@code static} </summary>
			  [STATIC]
		''' <summary>
		''' The modifier {@code final} </summary>
			   FINAL
		''' <summary>
		''' The modifier {@code transient} </summary>
		   TRANSIENT
		''' <summary>
		''' The modifier {@code volatile} </summary>
			VOLATILE
		''' <summary>
		''' The modifier {@code synchronized} </summary>
		SYNCHRONIZED
		''' <summary>
		''' The modifier {@code native} </summary>
			  NATIVE
		''' <summary>
		''' The modifier {@code strictfp} </summary>
			STRICTFP

		''' <summary>
		''' Returns this modifier's name in lowercase.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public String toString()
	'	{
	'		Return name().toLowerCase(java.util.Locale.US);
	'	}
	End Enum

End Namespace
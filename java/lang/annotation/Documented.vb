'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.annotation

	''' <summary>
	''' Indicates that annotations with a type are to be documented by javadoc
	''' and similar tools by default.  This type should be used to annotate the
	''' declarations of types whose annotations affect the use of annotated
	''' elements by their clients.  If a type declaration is annotated with
	''' Documented, its annotations become part of the public API
	''' of the annotated elements.
	''' 
	''' @author  Joshua Bloch
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class Documented
		Inherits System.Attribute

	End Class

End Namespace
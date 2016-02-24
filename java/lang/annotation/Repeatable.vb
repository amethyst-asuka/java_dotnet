'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The annotation type {@code java.lang.annotation.Repeatable} is
	''' used to indicate that the annotation type whose declaration it
	''' (meta-)annotates is <em>repeatable</em>. The value of
	''' {@code @Repeatable} indicates the <em>containing annotation
	''' type</em> for the repeatable annotation type.
	''' 
	''' @since 1.8
	''' @jls 9.6 Annotation Types
	''' @jls 9.7 Annotations
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class Repeatable
		Inherits System.Attribute

		''' <summary>
		''' Indicates the <em>containing annotation type</em> for the
		''' repeatable annotation type. </summary>
		''' <returns> the containing annotation type </returns>
		Class value()
	End Class

End Namespace
'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Indicates how long annotations with the annotated type are to
	''' be retained.  If no Retention annotation is present on
	''' an annotation type declaration, the retention policy defaults to
	''' {@code RetentionPolicy.CLASS}.
	''' 
	''' <p>A Retention meta-annotation has effect only if the
	''' meta-annotated type is used directly for annotation.  It has no
	''' effect if the meta-annotated type is used as a member type in
	''' another annotation type.
	''' 
	''' @author  Joshua Bloch
	''' @since 1.5
	''' @jls 9.6.3.2 @Retention
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class Retention
		Inherits System.Attribute

		''' <summary>
		''' Returns the retention policy. </summary>
		''' <returns> the retention policy </returns>
		RetentionPolicy value()
	End Class

End Namespace
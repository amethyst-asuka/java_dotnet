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

Namespace javax.annotation.processing


	''' <summary>
	''' An annotation used to indicate what annotation types an annotation
	''' processor supports.  The {@link
	''' Processor#getSupportedAnnotationTypes} method can construct its
	''' result from the value of this annotation, as done by {@link
	''' AbstractProcessor#getSupportedAnnotationTypes}.  Only {@linkplain
	''' Processor#getSupportedAnnotationTypes strings conforming to the
	''' grammar} should be used as values.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class SupportedAnnotationTypes
		Inherits System.Attribute

		''' <summary>
		''' Returns the names of the supported annotation types. </summary>
		''' <returns> the names of the supported annotation types </returns>
		String () value()
	End Class

End Namespace
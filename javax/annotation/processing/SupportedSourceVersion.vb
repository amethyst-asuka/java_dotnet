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
	''' An annotation used to indicate the latest source version an
	''' annotation processor supports.  The {@link
	''' Processor#getSupportedSourceVersion} method can construct its
	''' result from the value of this annotation, as done by {@link
	''' AbstractProcessor#getSupportedSourceVersion}.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class SupportedSourceVersion
		Inherits System.Attribute

		''' <summary>
		''' Returns the latest supported source version. </summary>
		''' <returns> the latest supported source version </returns>
		public property value() as javax.lang.model.SourceVersion
	End Class

End Namespace
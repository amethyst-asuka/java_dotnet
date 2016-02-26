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
	''' An annotation used to indicate what options an annotation processor
	''' supports.  The <seealso cref="Processor#getSupportedOptions"/> method can
	''' construct its result from the value of this annotation, as done by
	''' <seealso cref="AbstractProcessor#getSupportedOptions"/>.  Only {@linkplain
	''' Processor#getSupportedOptions strings conforming to the
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
	Public Class SupportedOptions
		Inherits System.Attribute

		''' <summary>
		''' Returns the supported options. </summary>
		''' <returns> the supported options </returns>
		public property  value() as String ()
	End Class

End Namespace
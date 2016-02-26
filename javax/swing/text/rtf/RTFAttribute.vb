'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf


	''' <summary>
	''' This interface describes a class which defines a 1-1 mapping between
	''' an RTF keyword and a SwingText attribute.
	''' </summary>
	Friend Interface RTFAttribute
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int D_CHARACTER = 0;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int D_PARAGRAPH = 1;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int D_SECTION = 2;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int D_DOCUMENT = 3;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int D_META = 4;

	'     These next three should really be public variables,
	'       but you can't declare public variables in an interface... 
		' int domain; 
		Function domain() As Integer
		' String swingName; 
		Function swingName() As Object
		' String rtfName; 
		Function rtfName() As String

		Function [set](ByVal target As javax.swing.text.MutableAttributeSet) As Boolean
		Function [set](ByVal target As javax.swing.text.MutableAttributeSet, ByVal parameter As Integer) As Boolean

		Function setDefault(ByVal target As javax.swing.text.MutableAttributeSet) As Boolean

		' TODO: This method is poorly thought out 
		Function write(ByVal source As javax.swing.text.AttributeSet, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean

		Function writeValue(ByVal value As Object, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean
	End Interface

End Namespace
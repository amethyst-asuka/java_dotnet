'
' * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans



	''' <summary>
	''' Indicates that an attribute called "transient"
	''' should be declared with the given {@code value}
	''' when the <seealso cref="Introspector"/> constructs
	''' a <seealso cref="PropertyDescriptor"/> or <seealso cref="EventSetDescriptor"/>
	''' classes associated with the annotated code element.
	''' A {@code true} value for the "transient" attribute
	''' indicates to encoders derived from <seealso cref="Encoder"/>
	''' that this feature should be ignored.
	''' <p>
	''' The {@code Transient} annotation may be be used
	''' in any of the methods that are involved
	''' in a <seealso cref="FeatureDescriptor"/> subclass
	''' to identify the transient feature in the annotated class and its subclasses.
	''' Normally, the method that starts with "get" is the best place
	''' to put the annotation and it is this declaration
	''' that takes precedence in the case of multiple annotations
	''' being defined for the same feature.
	''' <p>
	''' To declare a feature non-transient in a class
	''' whose superclass declares it transient,
	''' use {@code @Transient(false)}.
	''' In all cases, the <seealso cref="Introspector"/> decides
	''' if a feature is transient by referring to the annotation
	''' on the most specific superclass.
	''' If no {@code Transient} annotation is present
	''' in any superclass the feature is not transient.
	''' 
	''' @since 1.7
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class Transient
		Inherits System.Attribute

		''' <summary>
		''' Returns whether or not the {@code Introspector} should
		''' construct artifacts for the annotated method. </summary>
		''' <returns> whether or not the {@code Introspector} should
		''' construct artifacts for the annotated method </returns>
		Boolean value() default True
	End Class

End Namespace
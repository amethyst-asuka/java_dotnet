'
' * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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
	''' The ParameterDescriptor class allows bean implementors to provide
	''' additional information on each of their parameters, beyond the
	''' low level type information provided by the java.lang.reflect.Method
	''' class.
	''' <p>
	''' Currently all our state comes from the FeatureDescriptor base class.
	''' </summary>

	Public Class ParameterDescriptor
		Inherits FeatureDescriptor

		''' <summary>
		''' Public default constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Package private dup constructor.
		''' This must isolate the new object from any changes to the old object.
		''' </summary>
		Friend Sub New(ByVal old As ParameterDescriptor)
			MyBase.New(old)
		End Sub

	End Class

End Namespace
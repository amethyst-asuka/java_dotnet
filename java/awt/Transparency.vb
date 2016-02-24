'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt


	''' <summary>
	''' The <code>Transparency</code> interface defines the common transparency
	''' modes for implementing classes.
	''' </summary>
	Public Interface Transparency

		''' <summary>
		''' Represents image data that is guaranteed to be completely opaque,
		''' meaning that all pixels have an alpha value of 1.0.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public final static int OPAQUE = 1;

		''' <summary>
		''' Represents image data that is guaranteed to be either completely
		''' opaque, with an alpha value of 1.0, or completely transparent,
		''' with an alpha value of 0.0.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public final static int BITMASK = 2;

		''' <summary>
		''' Represents image data that contains or might contain arbitrary
		''' alpha values between and including 0.0 and 1.0.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public final static int TRANSLUCENT = 3;

		''' <summary>
		''' Returns the type of this <code>Transparency</code>. </summary>
		''' <returns> the field type of this <code>Transparency</code>, which is
		'''          either OPAQUE, BITMASK or TRANSLUCENT. </returns>
		ReadOnly Property transparency As Integer
	End Interface

End Namespace
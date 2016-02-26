'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws


	''' <summary>
	''' A WebServiceFeature is used to represent a feature that can be
	''' enabled or disabled for a web service.
	''' <p>
	''' The JAX-WS specification will define some standard features and
	''' JAX-WS implementors are free to define additional features if
	''' necessary.  Vendor specific features may not be portable so
	''' caution should be used when using them. Each Feature definition
	''' MUST define a <code>public static final String ID</code>
	''' that can be used in the Feature annotation to refer
	''' to the feature. This ID MUST be unique across all features
	''' of all vendors.  When defining a vendor specific feature ID,
	''' use a vendor specific namespace in the ID string.
	''' </summary>
	''' <seealso cref= javax.xml.ws.RespectBindingFeature </seealso>
	''' <seealso cref= javax.xml.ws.soap.AddressingFeature </seealso>
	''' <seealso cref= javax.xml.ws.soap.MTOMFeature
	''' 
	''' @since 2.1 </seealso>
	Public MustInherit Class WebServiceFeature
	   ''' <summary>
	   ''' Each Feature definition MUST define a public static final
	   ''' String ID that can be used in the Feature annotation to refer
	   ''' to the feature.
	   ''' </summary>
	   ' public static final String ID = "some unique feature Identifier";

	   ''' <summary>
	   ''' Get the unique identifier for this WebServiceFeature.
	   ''' </summary>
	   ''' <returns> the unique identifier for this feature. </returns>
	   Public MustOverride ReadOnly Property iD As String

	   ''' <summary>
	   ''' Specifies if the feature is enabled or disabled
	   ''' </summary>
	   Protected Friend enabled As Boolean = False


	   Protected Friend Sub New()
	   End Sub


	   ''' <summary>
	   ''' Returns <code>true</code> if this feature is enabled.
	   ''' </summary>
	   ''' <returns> <code>true</code> if and only if the feature is enabled . </returns>
	   Public Overridable Property enabled As Boolean
		   Get
			   Return enabled
		   End Get
	   End Property
	End Class

End Namespace
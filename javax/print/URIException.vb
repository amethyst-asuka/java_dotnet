'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print


	''' <summary>
	''' Interface URIException is a mixin interface which a subclass of {@link
	''' PrintException PrintException} can implement to report an error condition
	''' involving a URI address. The Print Service API does not define any print
	''' exception classes that implement interface URIException, that being left to
	''' the Print Service implementor's discretion.
	''' 
	''' </summary>

	Public Interface URIException

		''' <summary>
		''' Indicates that the printer cannot access the URI address.
		''' For example, the printer might report this error if it goes to get
		''' the print data and cannot even establish a connection to the
		''' URI address.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int URIInaccessible = 1;

		''' <summary>
		''' Indicates that the printer does not support the URI
		''' scheme ("http", "ftp", etc.) in the URI address.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int URISchemeNotSupported = 2;

		''' <summary>
		''' Indicates any kind of problem not specifically identified
		''' by the other reasons.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int URIOtherProblem = -1;

		''' <summary>
		''' Return the URI. </summary>
		''' <returns> the URI that is the cause of this exception. </returns>
		ReadOnly Property unsupportedURI As java.net.URI

		''' <summary>
		''' Return the reason for the event. </summary>
		''' <returns> one of the predefined reasons enumerated in this interface. </returns>
		ReadOnly Property reason As Integer

	End Interface

End Namespace
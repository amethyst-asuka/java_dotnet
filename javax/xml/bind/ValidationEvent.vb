Imports System

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

Namespace javax.xml.bind

	''' <summary>
	''' This event indicates that a problem was encountered while validating the
	''' incoming XML data during an unmarshal operation, while performing
	''' on-demand validation of the Java content tree, or while marshalling the
	''' Java content tree back to XML data.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= Validator </seealso>
	''' <seealso cref= ValidationEventHandler
	''' @since JAXB1.0 </seealso>
	Public Interface ValidationEvent

		''' <summary>
		''' Conditions that are not errors or fatal errors as defined by the
		''' XML 1.0 recommendation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int WARNING = 0;

		''' <summary>
		''' Conditions that correspond to the definition of "error" in section
		''' 1.2 of the W3C XML 1.0 Recommendation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int ERROR = 1;

		''' <summary>
		''' Conditions that correspond to the definition of "fatal error" in section
		''' 1.2 of the W3C XML 1.0 Recommendation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int FATAL_ERROR = 2;

		''' <summary>
		''' Retrieve the severity code for this warning/error.
		''' 
		''' <p>
		''' Must be one of <tt>ValidationError.WARNING</tt>,
		''' <tt>ValidationError.ERROR</tt>, or <tt>ValidationError.FATAL_ERROR</tt>.
		''' </summary>
		''' <returns> the severity code for this warning/error </returns>
		ReadOnly Property severity As Integer

		''' <summary>
		''' Retrieve the text message for this warning/error.
		''' </summary>
		''' <returns> the text message for this warning/error or null if one wasn't set </returns>
		ReadOnly Property message As String

		''' <summary>
		''' Retrieve the linked exception for this warning/error.
		''' </summary>
		''' <returns> the linked exception for this warning/error or null if one
		'''         wasn't set </returns>
		ReadOnly Property linkedException As Exception

		''' <summary>
		''' Retrieve the locator for this warning/error.
		''' </summary>
		''' <returns> the locator that indicates where the warning/error occurred </returns>
		ReadOnly Property locator As ValidationEventLocator

	End Interface

End Namespace
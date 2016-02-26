'
' * Copyright (c) 1999, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.ldap

	''' <summary>
	''' This interface represents an LDAPv3 control as defined in
	''' <A HREF="http://www.ietf.org/rfc/rfc2251.txt">RFC 2251</A>.
	''' <p>
	''' The LDAPv3 protocol uses controls to send and receive additional data
	''' to affect the behavior of predefined operations.
	''' Controls can be sent along with any LDAP operation to the server.
	''' These are referred to as <em>request controls</em>. For example, a
	''' "sort" control can be sent with an LDAP search operation to
	''' request that the results be returned in a particular order.
	''' Solicited and unsolicited controls can also be returned with
	''' responses from the server. Such controls are referred to as
	''' <em>response controls</em>. For example, an LDAP server might
	''' define a special control to return change notifications.
	''' <p>
	''' This interface is used to represent both request and response controls.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Vincent Ryan
	''' </summary>
	''' <seealso cref= ControlFactory
	''' @since 1.3 </seealso>
	Public Interface Control
		Inherits java.io.Serializable

		''' <summary>
		''' Indicates a critical control.
		''' The value of this constant is <tt>true</tt>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final boolean CRITICAL = True;

		''' <summary>
		''' Indicates a non-critical control.
		''' The value of this constant is <tt>false</tt>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final boolean NONCRITICAL = False;

		''' <summary>
		''' Retrieves the object identifier assigned for the LDAP control.
		''' </summary>
		''' <returns> The non-null object identifier string. </returns>
		ReadOnly Property iD As String

		''' <summary>
		''' Determines the criticality of the LDAP control.
		''' A critical control must not be ignored by the server.
		''' In other words, if the server receives a critical control
		''' that it does not support, regardless of whether the control
		''' makes sense for the operation, the operation will not be performed
		''' and an <tt>OperationNotSupportedException</tt> will be thrown. </summary>
		''' <returns> true if this control is critical; false otherwise. </returns>
		ReadOnly Property critical As Boolean

		''' <summary>
		''' Retrieves the ASN.1 BER encoded value of the LDAP control.
		''' The result is the raw BER bytes including the tag and length of
		''' the control's value. It does not include the controls OID or criticality.
		'''  
		''' Null is returned if the value is absent.
		''' </summary>
		''' <returns> A possibly null byte array representing the ASN.1 BER encoded
		'''         value of the LDAP control. </returns>
		ReadOnly Property encodedValue As SByte()

		' static final long serialVersionUID = -591027748900004825L;
	End Interface

End Namespace
Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace org.omg.CORBA_2_3.portable


	''' <summary>
	''' OutputStream provides interface for writing of all of the mapped IDL type
	''' to the stream. It extends org.omg.CORBA.portable.OutputStream, and defines
	''' new methods defined by CORBA 2.3.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.portable.OutputStream
	''' @author  OMG
	''' @since   JDK1.2 </seealso>

	Public MustInherit Class OutputStream
		Inherits org.omg.CORBA.portable.OutputStream

		Private Const ALLOW_SUBCLASS_PROP As String = "jdk.corba.allowOutputStreamSubclass"
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private static final boolean allowSubclass = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.lang.Boolean>()
	'	{
	'			@Override public java.lang.Boolean run()
	'			{
	'			String prop = System.getProperty(ALLOW_SUBCLASS_PROP);
	'				Return prop == Nothing ? False : (prop.equalsIgnoreCase("false") ? False : True);
	'			}
	'		});

		Private Shared Function checkPermission() As Void
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				If Not allowSubclass Then sm.checkPermission(New java.io.SerializablePermission("enableSubclassImplementation"))
			End If
			Return Nothing
		End Function
		Private Sub New(ByVal ignore As Void)
		End Sub

		''' <summary>
		''' Create a new instance of this class.
		''' 
		''' throw SecurityException if SecurityManager is installed and
		''' enableSubclassImplementation SerializablePermission
		''' is not granted or jdk.corba.allowOutputStreamSubclass system
		''' property is either not set or is set to 'false'
		''' </summary>
		Public Sub New()
			Me.New(checkPermission())
		End Sub

		''' <summary>
		''' Marshals a value type to the output stream. </summary>
		''' <param name="value"> is the acutal value to write </param>
		Public Overridable Sub write_value(ByVal value As java.io.Serializable)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Marshals a value type to the output stream. </summary>
		''' <param name="value"> is the acutal value to write </param>
		''' <param name="clz"> is the declared type of the value to be marshaled </param>
		Public Overridable Sub write_value(ByVal value As java.io.Serializable, ByVal clz As Type)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Marshals a value type to the output stream. </summary>
		''' <param name="value"> is the acutal value to write </param>
		''' <param name="repository_id"> identifies the type of the value type to
		''' be marshaled </param>
		Public Overridable Sub write_value(ByVal value As java.io.Serializable, ByVal repository_id As String)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Marshals a value type to the output stream. </summary>
		''' <param name="value"> is the acutal value to write </param>
		''' <param name="factory"> is the instance of the helper to be used for marshaling
		''' the boxed value </param>
		Public Overridable Sub write_value(ByVal value As java.io.Serializable, ByVal factory As org.omg.CORBA.portable.BoxedValueHelper)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Marshals a value object or a stub object. </summary>
		''' <param name="obj"> the actual value object to marshal or the stub to be marshalled </param>
		Public Overridable Sub write_abstract_interface(ByVal obj As Object)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

	End Class

End Namespace
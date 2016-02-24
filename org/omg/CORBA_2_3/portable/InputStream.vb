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
	''' InputStream provides for the reading of all of the mapped IDL types
	''' from the stream. It extends org.omg.CORBA.portable.InputStream.  This
	''' class defines new methods that were added for CORBA 2.3.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.portable.InputStream
	''' @author  OMG
	''' @since   JDK1.2 </seealso>

	Public MustInherit Class InputStream
		Inherits org.omg.CORBA.portable.InputStream


		Private Const ALLOW_SUBCLASS_PROP As String = "jdk.corba.allowInputStreamSubclass"

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
		''' is not granted or jdk.corba.allowInputStreamSubclass system
		''' property is either not set or is set to 'false'
		''' </summary>
		Public Sub New()
			Me.New(checkPermission())
		End Sub

		''' <summary>
		''' Unmarshalls a value type from the input stream. </summary>
		''' <returns> the value type unmarshalled from the input stream </returns>
		Public Overridable Function read_value() As java.io.Serializable
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Unmarshalls a value type from the input stream. </summary>
		''' <param name="clz"> is the declared type of the value to be unmarshalled </param>
		''' <returns> the value unmarshalled from the input stream </returns>
		Public Overridable Function read_value(ByVal clz As Type) As java.io.Serializable
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Unmarshalls a value type from the input stream. </summary>
		''' <param name="factory"> is the instance fo the helper to be used for
		''' unmarshalling the value type </param>
		''' <returns> the value unmarshalled from the input stream </returns>
		Public Overridable Function read_value(ByVal factory As org.omg.CORBA.portable.BoxedValueHelper) As java.io.Serializable
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Unmarshalls a value type from the input stream. </summary>
		''' <param name="rep_id"> identifies the type of the value to be unmarshalled </param>
		''' <returns> value type unmarshalled from the input stream </returns>
		Public Overridable Function read_value(ByVal rep_id As String) As java.io.Serializable
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Unmarshalls a value type from the input stream. </summary>
		''' <param name="value"> is an uninitialized value which is added to the orb's
		''' indirection table before calling Streamable._read() or
		''' CustomMarshal.unmarshal() to unmarshal the value. </param>
		''' <returns> value type unmarshalled from the input stream </returns>
		Public Overridable Function read_value(ByVal value As java.io.Serializable) As java.io.Serializable
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Unmarshal the value object or a suitable stub object. </summary>
		''' <returns> ORB runtime returns the value object or a suitable stub object. </returns>
		Public Overridable Function read_abstract_interface() As Object
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Unmarshal the class object or the stub class corresponding to the passed type. </summary>
		''' <param name="clz"> is the Class object for the stub class which corresponds to
		''' the type that is statically expected. </param>
		''' <returns> ORB runtime returns the value object or a suitable stub object. </returns>
		Public Overridable Function read_abstract_interface(ByVal clz As Type) As Object
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

	End Class

End Namespace
'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Constants written into the Object Serialization Stream.
	''' 
	''' @author  unascribed
	''' @since JDK 1.1
	''' </summary>
	Public Interface ObjectStreamConstants

		''' <summary>
		''' Magic number that is written to the stream header.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static short STREAM_MAGIC = (short)&Haced;

		''' <summary>
		''' Version number that is written to the stream header.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static short STREAM_VERSION = 5;

	'     Each item in the stream is preceded by a tag
	'     

		''' <summary>
		''' First tag value.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_BASE = &H70;

		''' <summary>
		''' Null object reference.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_NULL = (byte)&H70;

		''' <summary>
		''' Reference to an object already written into the stream.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_REFERENCE = (byte)&H71;

		''' <summary>
		''' new Class Descriptor.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_CLASSDESC = (byte)&H72;

		''' <summary>
		''' new Object.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_OBJECT = (byte)&H73;

		''' <summary>
		''' new String.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_STRING = (byte)&H74;

		''' <summary>
		''' new Array.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_ARRAY = (byte)&H75;

		''' <summary>
		''' Reference to Class.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_CLASS = (byte)&H76;

		''' <summary>
		''' Block of optional data. Byte following tag indicates number
		''' of bytes in this block data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_BLOCKDATA = (byte)&H77;

		''' <summary>
		''' End of optional block data blocks for an object.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_ENDBLOCKDATA = (byte)&H78;

		''' <summary>
		''' Reset stream context. All handles written into stream are reset.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_RESET = (byte)&H79;

		''' <summary>
		''' long Block data. The long following the tag indicates the
		''' number of bytes in this block data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_BLOCKDATALONG= (byte)&H7A;

		''' <summary>
		''' Exception during write.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_EXCEPTION = (byte)&H7B;

		''' <summary>
		''' Long string.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_LONGSTRING = (byte)&H7C;

		''' <summary>
		''' new Proxy Class Descriptor.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_PROXYCLASSDESC = (byte)&H7D;

		''' <summary>
		''' new Enum constant.
		''' @since 1.5
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_ENUM = (byte)&H7E;

		''' <summary>
		''' Last tag value.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte TC_MAX = (byte)&H7E;

		''' <summary>
		''' First wire handle to be assigned.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static int baseWireHandle = &H7e0000;


		''' <summary>
		'''*************************************************** </summary>
		' Bit masks for ObjectStreamClass flag.

		''' <summary>
		''' Bit mask for ObjectStreamClass flag. Indicates a Serializable class
		''' defines its own writeObject method.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte SC_WRITE_METHOD = &H1;

		''' <summary>
		''' Bit mask for ObjectStreamClass flag. Indicates Externalizable data
		''' written in Block Data mode.
		''' Added for PROTOCOL_VERSION_2.
		''' </summary>
		''' <seealso cref= #PROTOCOL_VERSION_2
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte SC_BLOCK_DATA = &H8;

		''' <summary>
		''' Bit mask for ObjectStreamClass flag. Indicates class is Serializable.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte SC_SERIALIZABLE = &H2;

		''' <summary>
		''' Bit mask for ObjectStreamClass flag. Indicates class is Externalizable.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte SC_EXTERNALIZABLE = &H4;

		''' <summary>
		''' Bit mask for ObjectStreamClass flag. Indicates class is an enum type.
		''' @since 1.5
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static byte SC_ENUM = &H10;


		' ******************************************************************
		' Security permissions 

		''' <summary>
		''' Enable substitution of one object for another during
		''' serialization/deserialization.
		''' </summary>
		''' <seealso cref= java.io.ObjectOutputStream#enableReplaceObject(boolean) </seealso>
		''' <seealso cref= java.io.ObjectInputStream#enableResolveObject(boolean)
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static SerializablePermission SUBSTITUTION_PERMISSION = New SerializablePermission("enableSubstitution");

		''' <summary>
		''' Enable overriding of readObject and writeObject.
		''' </summary>
		''' <seealso cref= java.io.ObjectOutputStream#writeObjectOverride(Object) </seealso>
		''' <seealso cref= java.io.ObjectInputStream#readObjectOverride()
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static SerializablePermission SUBCLASS_IMPLEMENTATION_PERMISSION = New SerializablePermission("enableSubclassImplementation");
	   ''' <summary>
	   ''' A Stream Protocol Version. <p>
	   ''' 
	   ''' All externalizable data is written in JDK 1.1 external data
	   ''' format after calling this method. This version is needed to write
	   ''' streams containing Externalizable data that can be read by
	   ''' pre-JDK 1.1.6 JVMs.
	   ''' </summary>
	   ''' <seealso cref= java.io.ObjectOutputStream#useProtocolVersion(int)
	   ''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public final static int PROTOCOL_VERSION_1 = 1;


	   ''' <summary>
	   ''' A Stream Protocol Version. <p>
	   ''' 
	   ''' This protocol is written by JVM 1.2.
	   ''' 
	   ''' Externalizable data is written in block data mode and is
	   ''' terminated with TC_ENDBLOCKDATA. Externalizable class descriptor
	   ''' flags has SC_BLOCK_DATA enabled. JVM 1.1.6 and greater can
	   ''' read this format change.
	   ''' 
	   ''' Enables writing a nonSerializable class descriptor into the
	   ''' stream. The serialVersionUID of a nonSerializable class is
	   ''' set to 0L.
	   ''' </summary>
	   ''' <seealso cref= java.io.ObjectOutputStream#useProtocolVersion(int) </seealso>
	   ''' <seealso cref= #SC_BLOCK_DATA
	   ''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public final static int PROTOCOL_VERSION_2 = 2;
	End Interface

End Namespace
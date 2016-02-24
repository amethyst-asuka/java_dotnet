'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.sql

	''' <summary>
	''' <P>Defines the constants that are used to identify generic
	''' SQL types, called JDBC types.
	''' <p> </summary>
	''' <seealso cref= SQLType
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
	Public Enum JDBCType

		''' <summary>
		''' Identifies the generic SQL type {@code BIT}.
		''' </summary>
		BIT = Types.BIT
		''' <summary>
		''' Identifies the generic SQL type {@code TINYINT}.
		''' </summary>
		TINYINT = Types.TINYINT
		''' <summary>
		''' Identifies the generic SQL type {@code SMALLINT}.
		''' </summary>
		SMALLINT = Types.SMALLINT
		''' <summary>
		''' Identifies the generic SQL type {@code INTEGER}.
		''' </summary>
		[INTEGER] = Types.INTEGER
		''' <summary>
		''' Identifies the generic SQL type {@code BIGINT}.
		''' </summary>
		BIGINT = Types.BIGINT
		''' <summary>
		''' Identifies the generic SQL type {@code FLOAT}.
		''' </summary>
		FLOAT = Types.FLOAT
		''' <summary>
		''' Identifies the generic SQL type {@code REAL}.
		''' </summary>
		REAL = Types.REAL
		''' <summary>
		''' Identifies the generic SQL type {@code DOUBLE}.
		''' </summary>
		[DOUBLE] = Types.DOUBLE
		''' <summary>
		''' Identifies the generic SQL type {@code NUMERIC}.
		''' </summary>
		NUMERIC = Types.NUMERIC
		''' <summary>
		''' Identifies the generic SQL type {@code DECIMAL}.
		''' </summary>
		[DECIMAL] = Types.DECIMAL
		''' <summary>
		''' Identifies the generic SQL type {@code CHAR}.
		''' </summary>
		[CHAR] = Types.CHAR
		''' <summary>
		''' Identifies the generic SQL type {@code VARCHAR}.
		''' </summary>
		VARCHAR = Types.VARCHAR
		''' <summary>
		''' Identifies the generic SQL type {@code LONGVARCHAR}.
		''' </summary>
		LONGVARCHAR = Types.LONGVARCHAR
		''' <summary>
		''' Identifies the generic SQL type {@code DATE}.
		''' </summary>
		[DATE] = Types.DATE
		''' <summary>
		''' Identifies the generic SQL type {@code TIME}.
		''' </summary>
		TIME = Types.TIME
		''' <summary>
		''' Identifies the generic SQL type {@code TIMESTAMP}.
		''' </summary>
		TIMESTAMP = Types.TIMESTAMP
		''' <summary>
		''' Identifies the generic SQL type {@code BINARY}.
		''' </summary>
		BINARY = Types.BINARY
		''' <summary>
		''' Identifies the generic SQL type {@code VARBINARY}.
		''' </summary>
		VARBINARY = Types.VARBINARY
		''' <summary>
		''' Identifies the generic SQL type {@code LONGVARBINARY}.
		''' </summary>
		LONGVARBINARY = Types.LONGVARBINARY
		''' <summary>
		''' Identifies the generic SQL value {@code NULL}.
		''' </summary>
		NULL = Types.NULL
		''' <summary>
		''' Indicates that the SQL type
		''' is database-specific and gets mapped to a Java object that can be
		''' accessed via the methods getObject and setObject.
		''' </summary>
		OTHER = Types.OTHER
		''' <summary>
		''' Indicates that the SQL type
		''' is database-specific and gets mapped to a Java object that can be
		''' accessed via the methods getObject and setObject.
		''' </summary>
		JAVA_OBJECT = Types.JAVA_OBJECT
		''' <summary>
		''' Identifies the generic SQL type {@code DISTINCT}.
		''' </summary>
		DISTINCT = Types.DISTINCT
		''' <summary>
		''' Identifies the generic SQL type {@code STRUCT}.
		''' </summary>
		STRUCT = Types.STRUCT
		''' <summary>
		''' Identifies the generic SQL type {@code ARRAY}.
		''' </summary>
		ARRAY = Types.ARRAY
		''' <summary>
		''' Identifies the generic SQL type {@code BLOB}.
		''' </summary>
		BLOB = Types.BLOB
		''' <summary>
		''' Identifies the generic SQL type {@code CLOB}.
		''' </summary>
		CLOB = Types.CLOB
		''' <summary>
		''' Identifies the generic SQL type {@code REF}.
		''' </summary>
		REF = Types.REF
		''' <summary>
		''' Identifies the generic SQL type {@code DATALINK}.
		''' </summary>
		DATALINK = Types.DATALINK
		''' <summary>
		''' Identifies the generic SQL type {@code BOOLEAN}.
		''' </summary>
		[BOOLEAN] = Types.BOOLEAN

		' JDBC 4.0 Types 

		''' <summary>
		''' Identifies the SQL type {@code ROWID}.
		''' </summary>
		ROWID = Types.ROWID
		''' <summary>
		''' Identifies the generic SQL type {@code NCHAR}.
		''' </summary>
		NCHAR = Types.NCHAR
		''' <summary>
		''' Identifies the generic SQL type {@code NVARCHAR}.
		''' </summary>
		NVARCHAR = Types.NVARCHAR
		''' <summary>
		''' Identifies the generic SQL type {@code LONGNVARCHAR}.
		''' </summary>
		LONGNVARCHAR = Types.LONGNVARCHAR
		''' <summary>
		''' Identifies the generic SQL type {@code NCLOB}.
		''' </summary>
		NCLOB = Types.NCLOB
		''' <summary>
		''' Identifies the generic SQL type {@code SQLXML}.
		''' </summary>
		SQLXML = Types.SQLXML

		' JDBC 4.2 Types 

		''' <summary>
		''' Identifies the generic SQL type {@code REF_CURSOR}.
		''' </summary>
		REF_CURSOR = Types.REF_CURSOR

		''' <summary>
		''' Identifies the generic SQL type {@code TIME_WITH_TIMEZONE}.
		''' </summary>
		TIME_WITH_TIMEZONE = Types.TIME_WITH_TIMEZONE

		''' <summary>
		''' Identifies the generic SQL type {@code TIMESTAMP_WITH_TIMEZONE}.
		''' </summary>
		TIMESTAMP_WITH_TIMEZONE = Types.TIMESTAMP_WITH_TIMEZONE

		''' <summary>
		''' The Integer value for the JDBCType.  It maps to a value in
		''' {@code Types.java}
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private java.lang.Integer type;

		''' <summary>
		''' Constructor to specify the data type value from {@code Types) for
		''' this data type. </summary>
		''' <param name="type"> The value from {@code Types) for this data type </param>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		JDBCType(final java.lang.Integer type)
	'	{
	'		Me.type = type;
	'	}

		''' <summary>
		''' {@inheritDoc } </summary>
		''' <returns> The name of this {@code SQLType}. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public String getName()
	'	{
	'		Return name();
	'	}
		''' <summary>
		''' Returns the name of the vendor that supports this data type. </summary>
		''' <returns>  The name of the vendor for this data type which is
		''' {@literal java.sql} for JDBCType. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public String getVendor()
	'	{
	'		Return "java.sql";
	'	}

		''' <summary>
		''' Returns the vendor specific type number for the data type. </summary>
		''' <returns>  An Integer representing the data type. For {@code JDBCType},
		''' the value will be the same value as in {@code Types} for the data type. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public java.lang.Integer getVendorTypeNumber()
	'	{
	'		Return type;
	'	}
		''' <summary>
		''' Returns the {@code JDBCType} that corresponds to the specified
		''' {@code Types} value </summary>
		''' <param name="type"> {@code Types} value </param>
		''' <returns> The {@code JDBCType} constant </returns>
		''' <exception cref="IllegalArgumentException"> if this enum type has no constant with
		''' the specified {@code Types} value </exception>
		''' <seealso cref= Types </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static JDBCType valueOf(int type)
	'	{
	'		for(JDBCType sqlType : JDBCType.class.getEnumConstants())
	'		{
	'			if(type == sqlType.type)
	'				Return sqlType;
	'		}
	'		throw New IllegalArgumentException("Type:" + type + " is not a valid " + "Types.java value.");
	'	}
	End Enum

End Namespace
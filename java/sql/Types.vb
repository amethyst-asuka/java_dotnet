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

Namespace java.sql

	''' <summary>
	''' <P>The class that defines the constants that are used to identify generic
	''' SQL types, called JDBC types.
	''' <p>
	''' This class is never instantiated.
	''' </summary>
	Public Class Types

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>BIT</code>.
	''' </summary>
			Public Const BIT As Integer = -7

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>TINYINT</code>.
	''' </summary>
			Public Const TINYINT As Integer = -6

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>SMALLINT</code>.
	''' </summary>
			Public Const SMALLINT As Integer = 5

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>INTEGER</code>.
	''' </summary>
			Public Const [INTEGER] As Integer = 4

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>BIGINT</code>.
	''' </summary>
			Public Const BIGINT As Integer = -5

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>FLOAT</code>.
	''' </summary>
			Public Const FLOAT As Integer = 6

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>REAL</code>.
	''' </summary>
			Public Const REAL As Integer = 7


	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>DOUBLE</code>.
	''' </summary>
			Public Const [DOUBLE] As Integer = 8

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>NUMERIC</code>.
	''' </summary>
			Public Const NUMERIC As Integer = 2

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>DECIMAL</code>.
	''' </summary>
			Public Const [DECIMAL] As Integer = 3

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>CHAR</code>.
	''' </summary>
			Public Const [CHAR] As Integer = 1

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>VARCHAR</code>.
	''' </summary>
			Public Const VARCHAR As Integer = 12

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>LONGVARCHAR</code>.
	''' </summary>
			Public Const LONGVARCHAR As Integer = -1


	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>DATE</code>.
	''' </summary>
			Public Const [DATE] As Integer = 91

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>TIME</code>.
	''' </summary>
			Public Const TIME As Integer = 92

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>TIMESTAMP</code>.
	''' </summary>
			Public Const TIMESTAMP As Integer = 93


	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>BINARY</code>.
	''' </summary>
			Public Const BINARY As Integer = -2

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>VARBINARY</code>.
	''' </summary>
			Public Const VARBINARY As Integer = -3

	''' <summary>
	''' <P>The constant in the Java programming language, sometimes referred
	''' to as a type code, that identifies the generic SQL type
	''' <code>LONGVARBINARY</code>.
	''' </summary>
			Public Const LONGVARBINARY As Integer = -4

	''' <summary>
	''' <P>The constant in the Java programming language
	''' that identifies the generic SQL value
	''' <code>NULL</code>.
	''' </summary>
			Public Const NULL As Integer = 0

		''' <summary>
		''' The constant in the Java programming language that indicates
		''' that the SQL type is database-specific and
		''' gets mapped to a Java object that can be accessed via
		''' the methods <code>getObject</code> and <code>setObject</code>.
		''' </summary>
			Public Const OTHER As Integer = 1111



		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>JAVA_OBJECT</code>.
		''' @since 1.2
		''' </summary>
			Public Const JAVA_OBJECT As Integer = 2000

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>DISTINCT</code>.
		''' @since 1.2
		''' </summary>
			Public Const DISTINCT As Integer = 2001

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>STRUCT</code>.
		''' @since 1.2
		''' </summary>
			Public Const STRUCT As Integer = 2002

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>ARRAY</code>.
		''' @since 1.2
		''' </summary>
			Public Const ARRAY As Integer = 2003

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>BLOB</code>.
		''' @since 1.2
		''' </summary>
			Public Const BLOB As Integer = 2004

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>CLOB</code>.
		''' @since 1.2
		''' </summary>
			Public Const CLOB As Integer = 2005

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' <code>REF</code>.
		''' @since 1.2
		''' </summary>
			Public Const REF As Integer = 2006

		''' <summary>
		''' The constant in the Java programming language, somtimes referred to
		''' as a type code, that identifies the generic SQL type <code>DATALINK</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const DATALINK As Integer = 70

		''' <summary>
		''' The constant in the Java programming language, somtimes referred to
		''' as a type code, that identifies the generic SQL type <code>BOOLEAN</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Const [BOOLEAN] As Integer = 16

		'------------------------- JDBC 4.0 -----------------------------------

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type <code>ROWID</code>
		''' 
		''' @since 1.6
		''' 
		''' </summary>
		Public Const ROWID As Integer = -8

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type <code>NCHAR</code>
		''' 
		''' @since 1.6
		''' </summary>
		Public Const NCHAR As Integer = -15

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type <code>NVARCHAR</code>.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const NVARCHAR As Integer = -9

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type <code>LONGNVARCHAR</code>.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const LONGNVARCHAR As Integer = -16

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type <code>NCLOB</code>.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const NCLOB As Integer = 2011

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type <code>XML</code>.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const SQLXML As Integer = 2009

		'--------------------------JDBC 4.2 -----------------------------

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type {@code REF CURSOR}.
		''' 
		''' @since 1.8
		''' </summary>
		Public Const REF_CURSOR As Integer = 2012

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' {@code TIME WITH TIMEZONE}.
		''' 
		''' @since 1.8
		''' </summary>
		Public Const TIME_WITH_TIMEZONE As Integer = 2013

		''' <summary>
		''' The constant in the Java programming language, sometimes referred to
		''' as a type code, that identifies the generic SQL type
		''' {@code TIMESTAMP WITH TIMEZONE}.
		''' 
		''' @since 1.8
		''' </summary>
		Public Const TIMESTAMP_WITH_TIMEZONE As Integer = 2014

		' Prevent instantiation
		Private Sub New()
		End Sub
	End Class

End Namespace
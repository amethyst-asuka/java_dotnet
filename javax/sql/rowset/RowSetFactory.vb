'
' * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql.rowset


	''' <summary>
	''' An interface that defines the implementation of a factory that is used
	''' to obtain different types of {@code RowSet} implementations.
	''' 
	''' @author Lance Andersen
	''' @since 1.7
	''' </summary>
	Public Interface RowSetFactory

		''' <summary>
		''' <p>Creates a new instance of a CachedRowSet.</p>
		''' </summary>
		''' <returns> A new instance of a CachedRowSet.
		''' </returns>
		''' <exception cref="SQLException"> if a CachedRowSet cannot
		'''   be created.
		''' 
		''' @since 1.7 </exception>
		Function createCachedRowSet() As CachedRowSet

		''' <summary>
		''' <p>Creates a new instance of a FilteredRowSet.</p>
		''' </summary>
		''' <returns> A new instance of a FilteredRowSet.
		''' </returns>
		''' <exception cref="SQLException"> if a FilteredRowSet cannot
		'''   be created.
		''' 
		''' @since 1.7 </exception>
		Function createFilteredRowSet() As FilteredRowSet

		''' <summary>
		''' <p>Creates a new instance of a JdbcRowSet.</p>
		''' </summary>
		''' <returns> A new instance of a JdbcRowSet.
		''' </returns>
		''' <exception cref="SQLException"> if a JdbcRowSet cannot
		'''   be created.
		''' 
		''' @since 1.7 </exception>
		Function createJdbcRowSet() As JdbcRowSet

		''' <summary>
		''' <p>Creates a new instance of a JoinRowSet.</p>
		''' </summary>
		''' <returns> A new instance of a JoinRowSet.
		''' </returns>
		''' <exception cref="SQLException"> if a JoinRowSet cannot
		'''   be created.
		''' 
		''' @since 1.7 </exception>
		Function createJoinRowSet() As JoinRowSet

		''' <summary>
		''' <p>Creates a new instance of a WebRowSet.</p>
		''' </summary>
		''' <returns> A new instance of a WebRowSet.
		''' </returns>
		''' <exception cref="SQLException"> if a WebRowSet cannot
		'''   be created.
		''' 
		''' @since 1.7 </exception>
		Function createWebRowSet() As WebRowSet

	End Interface
End Namespace
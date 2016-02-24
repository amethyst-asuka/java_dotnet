'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util.function

	''' <summary>
	''' Represents a supplier of results.
	''' 
	''' <p>There is no requirement that a new or distinct result be returned each
	''' time the supplier is invoked.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#get()"/>.
	''' </summary>
	''' @param <T> the type of results supplied by this supplier
	''' 
	''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface Supplier(Of T)

		''' <summary>
		''' Gets a result.
		''' </summary>
		''' <returns> a result </returns>
		Function [get]() As T
	End Interface

End Namespace
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
	''' Represents a supplier of {@code double}-valued results.  This is the
	''' {@code double}-producing primitive specialization of <seealso cref="Supplier"/>.
	''' 
	''' <p>There is no requirement that a distinct result be returned each
	''' time the supplier is invoked.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#getAsDouble()"/>.
	''' </summary>
	''' <seealso cref= Supplier
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface DoubleSupplier

		''' <summary>
		''' Gets a result.
		''' </summary>
		''' <returns> a result </returns>
		ReadOnly Property asDouble As Double
	End Interface

End Namespace
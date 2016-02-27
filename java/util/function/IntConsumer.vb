'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' Represents an operation that accepts a single {@code int}-valued argument and
    ''' returns no result.  This is the primitive type specialization of
    ''' <seealso cref="Consumer"/> for {@code int}.  Unlike most other functional interfaces,
    ''' {@code IntConsumer} is expected to operate via side-effects.
    ''' 
    ''' <p>This is a <a href="package-summary.html">functional interface</a>
    ''' whose functional method is <seealso cref="#accept(int)"/>.
    ''' </summary>
    ''' <seealso cref= Consumer
    ''' @since 1.8 </seealso>
    'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    Public Interface IntConsumer

        ''' <summary>
        ''' Performs this operation on the given argument.
        ''' </summary>
        ''' <param name="value"> the input argument </param>
        Sub accept(ByVal value As Integer)

        ''' <summary>
        ''' Returns a composed {@code IntConsumer} that performs, in sequence, this
        ''' operation followed by the {@code after} operation. If performing either
        ''' operation throws an exception, it is relayed to the caller of the
        ''' composed operation.  If performing this operation throws an exception,
        ''' the {@code after} operation will not be performed.
        ''' </summary>
        ''' <param name="after"> the operation to perform after this operation </param>
        ''' <returns> a composed {@code IntConsumer} that performs in sequence this
        ''' operation followed by the {@code after} operation </returns>
        ''' <exception cref="NullPointerException"> if {@code after} is null </exception>
        Function andThen(ByVal after As IntConsumer) As IntConsumer
    End Interface
End Namespace
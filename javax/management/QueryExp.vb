'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management

	' java import


	''' <summary>
	''' <p>Represents relational constraints similar to database query "where
	''' clauses". Instances of QueryExp are returned by the static methods of the
	''' <seealso cref="Query"/> class.</p>
	''' 
	''' <p>It is possible, but not
	''' recommended, to create custom queries by implementing this
	''' interface.  In that case, it is better to extend the {@link
	''' QueryEval} class than to implement the interface directly, so that
	''' the <seealso cref="#setMBeanServer"/> method works correctly.
	''' </summary>
	''' <seealso cref= MBeanServer#queryNames MBeanServer.queryNames
	''' @since 1.5 </seealso>
	Public Interface QueryExp
		Inherits java.io.Serializable


		 ''' <summary>
		 ''' Applies the QueryExp on an MBean.
		 ''' </summary>
		 ''' <param name="name"> The name of the MBean on which the QueryExp will be applied.
		 ''' </param>
		 ''' <returns>  True if the query was successfully applied to the MBean, false otherwise
		 ''' </returns>
		 ''' <exception cref="BadStringOperationException"> </exception>
		 ''' <exception cref="BadBinaryOpValueExpException"> </exception>
		 ''' <exception cref="BadAttributeValueExpException"> </exception>
		 ''' <exception cref="InvalidApplicationException"> </exception>
		 Function apply(ByVal name As ObjectName) As Boolean

		 ''' <summary>
		 ''' Sets the MBean server on which the query is to be performed.
		 ''' </summary>
		 ''' <param name="s"> The MBean server on which the query is to be performed. </param>
		 WriteOnly Property mBeanServer As MBeanServer

	End Interface

End Namespace
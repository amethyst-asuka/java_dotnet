Imports System

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' Represents values that can be passed as arguments to
	''' relational expressions. Strings, numbers, attributes are valid values
	''' and should be represented by implementations of <CODE>ValueExp</CODE>.
	''' 
	''' @since 1.5
	''' </summary>
	'
	'  We considered generifying this interface as ValueExp<T>, where T is
	'  the Java type that this expression generates.  This allows some additional
	'  checking in the various methods of the Query class, but in practice
	'  not much.  Typically you have something like
	'  Query.lt(Query.attr("A"), Query.value(5)).  We can arrange for Query.value
	'  to have type ValueExp<Integer> (or maybe ValueExp<Long> or ValueExp<Number>)
	'  but for Query.attr we can't do better than ValueExp<?> or plain ValueExp.
	'  So even though we could define Query.lt as:
	'  QueryExp <T> lt(ValueExp<T> v1, ValueExp<T> v2)
	'  and thus prevent comparing a
	'  number against a string, in practice the first ValueExp will almost always
	'  be a Query.attr so this check serves no purpose.  You would have to
	'  write Query.<Number>attr("A"), for example, which would be awful.  And,
	'  if you wrote Query.<Integer>attr("A") you would then discover that you
	'  couldn't compare it against Query.value(5) if the latter is defined as
	'  ValueExp<Number>, or against Query.value(5L) if it is defined as
	'  ValueExp<Integer>.
	'
	'  Worse, for Query.in we would like to define:
	'  QueryExp <T> in(ValueExp<T> val, ValueExp<T>[] valueList)
	'  but this is unusable because you cannot write
	'  "new ValueExp<Integer>[] {...}" (the compiler forbids it).
	'
	'  The few mistakes you might catch with this generification certainly
	'  wouldn't justify the hassle of modifying user code to get the checks
	'  to be made and the "unchecked" warnings that would arise if it
	'  wasn't so modified.
	'
	'  We could reconsider this if the Query methods were augmented, for example
	'  with:
	'  AttributeValueExp<Number> numberAttr(String name);
	'  AttributeValueExp<String> stringAttr(String name);
	'  AttributeValueExp<Boolean> booleanAttr(String name);
	'  QueryExp <T> in(ValueExp<T> val, Set<ValueExp<T>> valueSet).
	'  But it's not really clear what numberAttr should do if it finds that the
	'  attribute is not in fact a Number.
	' 
	Public Interface ValueExp
		Inherits java.io.Serializable

		''' <summary>
		''' Applies the ValueExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the ValueExp will be applied.
		''' </param>
		''' <returns>  The <CODE>ValueExp</CODE>.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Function apply(ByVal name As ObjectName) As ValueExp

		''' <summary>
		''' Sets the MBean server on which the query is to be performed.
		''' </summary>
		''' <param name="s"> The MBean server on which the query is to be performed.
		''' </param>
		''' @deprecated This method is not needed because a
		''' <code>ValueExp</code> can access the MBean server in which it
		''' is being evaluated by using <seealso cref="QueryEval#getMBeanServer()"/>. 
		<Obsolete("This method is not needed because a")> _
		WriteOnly Property mBeanServer As MBeanServer
	End Interface

End Namespace
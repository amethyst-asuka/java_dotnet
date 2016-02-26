'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Constructs query object constraints.</p>
	''' 
	''' <p>The MBean Server can be queried for MBeans that meet a particular
	''' condition, using its <seealso cref="MBeanServer#queryNames queryNames"/> or
	''' <seealso cref="MBeanServer#queryMBeans queryMBeans"/> method.  The <seealso cref="QueryExp"/>
	''' parameter to the method can be any implementation of the interface
	''' {@code QueryExp}, but it is usually best to obtain the {@code QueryExp}
	''' value by calling the static methods in this class.  This is particularly
	''' true when querying a remote MBean Server: a custom implementation of the
	''' {@code QueryExp} interface might not be present in the remote MBean Server,
	''' but the methods in this class return only standard classes that are
	''' part of the JMX implementation.</p>
	''' 
	''' <p>As an example, suppose you wanted to find all MBeans where the {@code
	''' Enabled} attribute is {@code true} and the {@code Owner} attribute is {@code
	''' "Duke"}. Here is how you could construct the appropriate {@code QueryExp} by
	''' chaining together method calls:</p>
	''' 
	''' <pre>
	''' QueryExp query =
	'''     Query.and(Query.eq(Query.attr("Enabled"), Query.value(true)),
	'''               Query.eq(Query.attr("Owner"), Query.value("Duke")));
	''' </pre>
	''' 
	''' @since 1.5
	''' </summary>
	 Public Class Query
		 Inherits Object


		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#gt"/> query.  This is chiefly
		 ''' of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___GT As Integer = 0

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#lt"/> query.  This is chiefly
		 ''' of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___LT As Integer = 1

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#geq"/> query.  This is chiefly
		 ''' of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const GE As Integer = 2

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#leq"/> query.  This is chiefly
		 ''' of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const LE As Integer = 3

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#eq"/> query.  This is chiefly
		 ''' of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___EQ As Integer = 4


		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#plus"/> expression.  This
		 ''' is chiefly of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___PLUS As Integer = 0

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#minus"/> expression.  This
		 ''' is chiefly of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___MINUS As Integer = 1

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#times"/> expression.  This
		 ''' is chiefly of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___TIMES As Integer = 2

		 ''' <summary>
		 ''' A code representing the <seealso cref="Query#div"/> expression.  This is
		 ''' chiefly of interest for the serialized form of queries.
		 ''' </summary>
		 Public Const ___DIV As Integer = 3


		 ''' <summary>
		 ''' Basic constructor.
		 ''' </summary>
		 Public Sub New()
		 End Sub


		 ''' <summary>
		 ''' Returns a query expression that is the conjunction of two other query
		 ''' expressions.
		 ''' </summary>
		 ''' <param name="q1"> A query expression. </param>
		 ''' <param name="q2"> Another query expression.
		 ''' </param>
		 ''' <returns>  The conjunction of the two arguments.  The returned object
		 ''' will be serialized as an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.AndQueryExp">
		 ''' javax.management.AndQueryExp</a>. </returns>
		 Public Shared Function [and](ByVal q1 As QueryExp, ByVal q2 As QueryExp) As QueryExp
			 Return New AndQueryExp(q1, q2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that is the disjunction of two other query
		 ''' expressions.
		 ''' </summary>
		 ''' <param name="q1"> A query expression. </param>
		 ''' <param name="q2"> Another query expression.
		 ''' </param>
		 ''' <returns>  The disjunction of the two arguments.  The returned object
		 ''' will be serialized as an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.OrQueryExp">
		 ''' javax.management.OrQueryExp</a>. </returns>
		 Public Shared Function [or](ByVal q1 As QueryExp, ByVal q2 As QueryExp) As QueryExp
			 Return New OrQueryExp(q1, q2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a "greater than" constraint on
		 ''' two values.
		 ''' </summary>
		 ''' <param name="v1"> A value expression. </param>
		 ''' <param name="v2"> Another value expression.
		 ''' </param>
		 ''' <returns> A "greater than" constraint on the arguments.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryRelQueryExp">
		 ''' javax.management.BinaryRelQueryExp</a> with a {@code relOp} equal
		 ''' to <seealso cref="#GT"/>. </returns>
		 Public Shared Function gt(ByVal v1 As ValueExp, ByVal v2 As ValueExp) As QueryExp
			 Return New BinaryRelQueryExp(___GT, v1, v2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a "greater than or equal
		 ''' to" constraint on two values.
		 ''' </summary>
		 ''' <param name="v1"> A value expression. </param>
		 ''' <param name="v2"> Another value expression.
		 ''' </param>
		 ''' <returns> A "greater than or equal to" constraint on the
		 ''' arguments.  The returned object will be serialized as an
		 ''' instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryRelQueryExp">
		 ''' javax.management.BinaryRelQueryExp</a> with a {@code relOp} equal
		 ''' to <seealso cref="#GE"/>. </returns>
		 Public Shared Function geq(ByVal v1 As ValueExp, ByVal v2 As ValueExp) As QueryExp
			 Return New BinaryRelQueryExp(GE, v1, v2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a "less than or equal to"
		 ''' constraint on two values.
		 ''' </summary>
		 ''' <param name="v1"> A value expression. </param>
		 ''' <param name="v2"> Another value expression.
		 ''' </param>
		 ''' <returns> A "less than or equal to" constraint on the arguments.
		 ''' The returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryRelQueryExp">
		 ''' javax.management.BinaryRelQueryExp</a> with a {@code relOp} equal
		 ''' to <seealso cref="#LE"/>. </returns>
		 Public Shared Function leq(ByVal v1 As ValueExp, ByVal v2 As ValueExp) As QueryExp
			 Return New BinaryRelQueryExp(LE, v1, v2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a "less than" constraint on
		 ''' two values.
		 ''' </summary>
		 ''' <param name="v1"> A value expression. </param>
		 ''' <param name="v2"> Another value expression.
		 ''' </param>
		 ''' <returns> A "less than" constraint on the arguments.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryRelQueryExp">
		 ''' javax.management.BinaryRelQueryExp</a> with a {@code relOp} equal
		 ''' to <seealso cref="#LT"/>. </returns>
		 Public Shared Function lt(ByVal v1 As ValueExp, ByVal v2 As ValueExp) As QueryExp
			 Return New BinaryRelQueryExp(___LT, v1, v2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents an equality constraint on
		 ''' two values.
		 ''' </summary>
		 ''' <param name="v1"> A value expression. </param>
		 ''' <param name="v2"> Another value expression.
		 ''' </param>
		 ''' <returns> A "equal to" constraint on the arguments.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryRelQueryExp">
		 ''' javax.management.BinaryRelQueryExp</a> with a {@code relOp} equal
		 ''' to <seealso cref="#EQ"/>. </returns>
		 Public Shared Function eq(ByVal v1 As ValueExp, ByVal v2 As ValueExp) As QueryExp
			 Return New BinaryRelQueryExp(___EQ, v1, v2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents the constraint that one
		 ''' value is between two other values.
		 ''' </summary>
		 ''' <param name="v1"> A value expression that is "between" v2 and v3. </param>
		 ''' <param name="v2"> Value expression that represents a boundary of the constraint. </param>
		 ''' <param name="v3"> Value expression that represents a boundary of the constraint.
		 ''' </param>
		 ''' <returns> The constraint that v1 lies between v2 and v3.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BetweenQueryExp">
		 ''' javax.management.BetweenQueryExp</a>. </returns>
		 Public Shared Function between(ByVal v1 As ValueExp, ByVal v2 As ValueExp, ByVal v3 As ValueExp) As QueryExp
			 Return New BetweenQueryExp(v1, v2, v3)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a matching constraint on
		 ''' a string argument. The matching syntax is consistent with file globbing:
		 ''' supports "<code>?</code>", "<code>*</code>", "<code>[</code>",
		 ''' each of which may be escaped with "<code>\</code>";
		 ''' character classes may use "<code>!</code>" for negation and
		 ''' "<code>-</code>" for range.
		 ''' (<code>*</code> for any character sequence,
		 ''' <code>?</code> for a single arbitrary character,
		 ''' <code>[...]</code> for a character sequence).
		 ''' For example: <code>a*b?c</code> would match a string starting
		 ''' with the character <code>a</code>, followed
		 ''' by any number of characters, followed by a <code>b</code>,
		 ''' any single character, and a <code>c</code>.
		 ''' </summary>
		 ''' <param name="a"> An attribute expression </param>
		 ''' <param name="s"> A string value expression representing a matching constraint
		 ''' </param>
		 ''' <returns> A query expression that represents the matching
		 ''' constraint on the string argument.  The returned object will
		 ''' be serialized as an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.MatchQueryExp">
		 ''' javax.management.MatchQueryExp</a>. </returns>
		 Public Shared Function match(ByVal a As AttributeValueExp, ByVal s As StringValueExp) As QueryExp
			 Return New MatchQueryExp(a, s)
		 End Function

		 ''' <summary>
		 ''' <p>Returns a new attribute expression.  See <seealso cref="AttributeValueExp"/>
		 ''' for a detailed description of the semantics of the expression.</p>
		 ''' 
		 ''' <p>Evaluating this expression for a given
		 ''' <code>objectName</code> includes performing {@link
		 ''' MBeanServer#getAttribute MBeanServer.getAttribute(objectName,
		 ''' name)}.</p>
		 ''' </summary>
		 ''' <param name="name"> The name of the attribute.
		 ''' </param>
		 ''' <returns> An attribute expression for the attribute named {@code name}. </returns>
		 Public Shared Function attr(ByVal name As String) As AttributeValueExp
			 Return New AttributeValueExp(name)
		 End Function

		 ''' <summary>
		 ''' <p>Returns a new qualified attribute expression.</p>
		 ''' 
		 ''' <p>Evaluating this expression for a given
		 ''' <code>objectName</code> includes performing {@link
		 ''' MBeanServer#getObjectInstance
		 ''' MBeanServer.getObjectInstance(objectName)} and {@link
		 ''' MBeanServer#getAttribute MBeanServer.getAttribute(objectName,
		 ''' name)}.</p>
		 ''' </summary>
		 ''' <param name="className"> The name of the class possessing the attribute. </param>
		 ''' <param name="name"> The name of the attribute.
		 ''' </param>
		 ''' <returns> An attribute expression for the attribute named name.
		 ''' The returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.QualifiedAttributeValueExp">
		 ''' javax.management.QualifiedAttributeValueExp</a>. </returns>
		 Public Shared Function attr(ByVal className As String, ByVal name As String) As AttributeValueExp
			 Return New QualifiedAttributeValueExp(className, name)
		 End Function

		 ''' <summary>
		 ''' <p>Returns a new class attribute expression which can be used in any
		 ''' Query call that expects a ValueExp.</p>
		 ''' 
		 ''' <p>Evaluating this expression for a given
		 ''' <code>objectName</code> includes performing {@link
		 ''' MBeanServer#getObjectInstance
		 ''' MBeanServer.getObjectInstance(objectName)}.</p>
		 ''' </summary>
		 ''' <returns> A class attribute expression.  The returned object
		 ''' will be serialized as an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.ClassAttributeValueExp">
		 ''' javax.management.ClassAttributeValueExp</a>. </returns>
		 Public Shared Function classattr() As AttributeValueExp
			 Return New ClassAttributeValueExp
		 End Function

		 ''' <summary>
		 ''' Returns a constraint that is the negation of its argument.
		 ''' </summary>
		 ''' <param name="queryExp"> The constraint to negate.
		 ''' </param>
		 ''' <returns> A negated constraint.  The returned object will be
		 ''' serialized as an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.NotQueryExp">
		 ''' javax.management.NotQueryExp</a>. </returns>
		 Public Shared Function [not](ByVal queryExp As QueryExp) As QueryExp
			 Return New NotQueryExp(queryExp)
		 End Function

		 ''' <summary>
		 ''' Returns an expression constraining a value to be one of an explicit list.
		 ''' </summary>
		 ''' <param name="val"> A value to be constrained. </param>
		 ''' <param name="valueList"> An array of ValueExps.
		 ''' </param>
		 ''' <returns> A QueryExp that represents the constraint.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.InQueryExp">
		 ''' javax.management.InQueryExp</a>. </returns>
		 Public Shared Function [in](ByVal val As ValueExp, ByVal valueList As ValueExp()) As QueryExp
			 Return New InQueryExp(val, valueList)
		 End Function

		 ''' <summary>
		 ''' Returns a new string expression.
		 ''' </summary>
		 ''' <param name="val"> The string value.
		 ''' </param>
		 ''' <returns>  A ValueExp object containing the string argument. </returns>
		 Public Shared Function value(ByVal val As String) As StringValueExp
			 Return New StringValueExp(val)
		 End Function

		 ''' <summary>
		 ''' Returns a numeric value expression that can be used in any Query call
		 ''' that expects a ValueExp.
		 ''' </summary>
		 ''' <param name="val"> An instance of Number.
		 ''' </param>
		 ''' <returns> A ValueExp object containing the argument.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.NumericValueExp">
		 ''' javax.management.NumericValueExp</a>. </returns>
		 Public Shared Function value(ByVal val As Number) As ValueExp
			 Return New NumericValueExp(val)
		 End Function

		 ''' <summary>
		 ''' Returns a numeric value expression that can be used in any Query call
		 ''' that expects a ValueExp.
		 ''' </summary>
		 ''' <param name="val"> An int value.
		 ''' </param>
		 ''' <returns> A ValueExp object containing the argument.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.NumericValueExp">
		 ''' javax.management.NumericValueExp</a>. </returns>
		 Public Shared Function value(ByVal val As Integer) As ValueExp
			 Return New NumericValueExp(CLng(val))
		 End Function

		 ''' <summary>
		 ''' Returns a numeric value expression that can be used in any Query call
		 ''' that expects a ValueExp.
		 ''' </summary>
		 ''' <param name="val"> A long value.
		 ''' </param>
		 ''' <returns> A ValueExp object containing the argument.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.NumericValueExp">
		 ''' javax.management.NumericValueExp</a>. </returns>
		 Public Shared Function value(ByVal val As Long) As ValueExp
			 Return New NumericValueExp(val)
		 End Function

		 ''' <summary>
		 ''' Returns a numeric value expression that can be used in any Query call
		 ''' that expects a ValueExp.
		 ''' </summary>
		 ''' <param name="val"> A float value.
		 ''' </param>
		 ''' <returns> A ValueExp object containing the argument.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.NumericValueExp">
		 ''' javax.management.NumericValueExp</a>. </returns>
		 Public Shared Function value(ByVal val As Single) As ValueExp
			 Return New NumericValueExp(CDbl(val))
		 End Function

		 ''' <summary>
		 ''' Returns a numeric value expression that can be used in any Query call
		 ''' that expects a ValueExp.
		 ''' </summary>
		 ''' <param name="val"> A double value.
		 ''' </param>
		 ''' <returns>  A ValueExp object containing the argument.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.NumericValueExp">
		 ''' javax.management.NumericValueExp</a>. </returns>
		 Public Shared Function value(ByVal val As Double) As ValueExp
			 Return New NumericValueExp(val)
		 End Function

		 ''' <summary>
		 ''' Returns a boolean value expression that can be used in any Query call
		 ''' that expects a ValueExp.
		 ''' </summary>
		 ''' <param name="val"> A boolean value.
		 ''' </param>
		 ''' <returns> A ValueExp object containing the argument.  The
		 ''' returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BooleanValueExp">
		 ''' javax.management.BooleanValueExp</a>. </returns>
		 Public Shared Function value(ByVal val As Boolean) As ValueExp
			 Return New BooleanValueExp(val)
		 End Function

		 ''' <summary>
		 ''' Returns a binary expression representing the sum of two numeric values,
		 ''' or the concatenation of two string values.
		 ''' </summary>
		 ''' <param name="value1"> The first '+' operand. </param>
		 ''' <param name="value2"> The second '+' operand.
		 ''' </param>
		 ''' <returns> A ValueExp representing the sum or concatenation of
		 ''' the two arguments.  The returned object will be serialized as
		 ''' an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryOpValueExp">
		 ''' javax.management.BinaryOpValueExp</a> with an {@code op} equal to
		 ''' <seealso cref="#PLUS"/>. </returns>
		 Public Shared Function plus(ByVal value1 As ValueExp, ByVal value2 As ValueExp) As ValueExp
			 Return New BinaryOpValueExp(___PLUS, value1, value2)
		 End Function

		 ''' <summary>
		 ''' Returns a binary expression representing the product of two numeric values.
		 ''' 
		 ''' </summary>
		 ''' <param name="value1"> The first '*' operand. </param>
		 ''' <param name="value2"> The second '*' operand.
		 ''' </param>
		 ''' <returns> A ValueExp representing the product.  The returned
		 ''' object will be serialized as an instance of the non-public
		 ''' class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryOpValueExp">
		 ''' javax.management.BinaryOpValueExp</a> with an {@code op} equal to
		 ''' <seealso cref="#TIMES"/>. </returns>
		 Public Shared Function times(ByVal value1 As ValueExp, ByVal value2 As ValueExp) As ValueExp
			 Return New BinaryOpValueExp(___TIMES, value1, value2)
		 End Function

		 ''' <summary>
		 ''' Returns a binary expression representing the difference between two numeric
		 ''' values.
		 ''' </summary>
		 ''' <param name="value1"> The first '-' operand. </param>
		 ''' <param name="value2"> The second '-' operand.
		 ''' </param>
		 ''' <returns> A ValueExp representing the difference between two
		 ''' arguments.  The returned object will be serialized as an
		 ''' instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryOpValueExp">
		 ''' javax.management.BinaryOpValueExp</a> with an {@code op} equal to
		 ''' <seealso cref="#MINUS"/>. </returns>
		 Public Shared Function minus(ByVal value1 As ValueExp, ByVal value2 As ValueExp) As ValueExp
			 Return New BinaryOpValueExp(___MINUS, value1, value2)
		 End Function

		 ''' <summary>
		 ''' Returns a binary expression representing the quotient of two numeric
		 ''' values.
		 ''' </summary>
		 ''' <param name="value1"> The first '/' operand. </param>
		 ''' <param name="value2"> The second '/' operand.
		 ''' </param>
		 ''' <returns> A ValueExp representing the quotient of two arguments.
		 ''' The returned object will be serialized as an instance of the
		 ''' non-public class
		 ''' <a href="../../serialized-form.html#javax.management.BinaryOpValueExp">
		 ''' javax.management.BinaryOpValueExp</a> with an {@code op} equal to
		 ''' <seealso cref="#DIV"/>. </returns>
		 Public Shared Function div(ByVal value1 As ValueExp, ByVal value2 As ValueExp) As ValueExp
			 Return New BinaryOpValueExp(___DIV, value1, value2)
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a matching constraint on
		 ''' a string argument. The value must start with the given literal string
		 ''' value.
		 ''' </summary>
		 ''' <param name="a"> An attribute expression. </param>
		 ''' <param name="s"> A string value expression representing the beginning of the
		 ''' string value.
		 ''' </param>
		 ''' <returns> The constraint that a matches s.  The returned object
		 ''' will be serialized as an instance of the non-public class
		 ''' 
		 ''' <a href="../../serialized-form.html#javax.management.MatchQueryExp">
		 ''' javax.management.MatchQueryExp</a>. </returns>
		 Public Shared Function initialSubString(ByVal a As AttributeValueExp, ByVal s As StringValueExp) As QueryExp
			 Return New MatchQueryExp(a, New StringValueExp(escapeString(s.value) & "*"))
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a matching constraint on
		 ''' a string argument. The value must contain the given literal string
		 ''' value.
		 ''' </summary>
		 ''' <param name="a"> An attribute expression. </param>
		 ''' <param name="s"> A string value expression representing the substring.
		 ''' </param>
		 ''' <returns> The constraint that a matches s.  The returned object
		 ''' will be serialized as an instance of the non-public class
		 ''' 
		 ''' <a href="../../serialized-form.html#javax.management.MatchQueryExp">
		 ''' javax.management.MatchQueryExp</a>. </returns>
		 Public Shared Function anySubString(ByVal a As AttributeValueExp, ByVal s As StringValueExp) As QueryExp
			 Return New MatchQueryExp(a, New StringValueExp("*" & escapeString(s.value) & "*"))
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents a matching constraint on
		 ''' a string argument. The value must end with the given literal string
		 ''' value.
		 ''' </summary>
		 ''' <param name="a"> An attribute expression. </param>
		 ''' <param name="s"> A string value expression representing the end of the string
		 ''' value.
		 ''' </param>
		 ''' <returns> The constraint that a matches s.  The returned object
		 ''' will be serialized as an instance of the non-public class
		 ''' 
		 ''' <a href="../../serialized-form.html#javax.management.MatchQueryExp">
		 ''' javax.management.MatchQueryExp</a>. </returns>
		 Public Shared Function finalSubString(ByVal a As AttributeValueExp, ByVal s As StringValueExp) As QueryExp
			 Return New MatchQueryExp(a, New StringValueExp("*" & escapeString(s.value)))
		 End Function

		 ''' <summary>
		 ''' Returns a query expression that represents an inheritance constraint
		 ''' on an MBean class.
		 ''' <p>Example: to find MBeans that are instances of
		 ''' <seealso cref="NotificationBroadcaster"/>, use
		 ''' {@code Query.isInstanceOf(Query.value(NotificationBroadcaster.class.getName()))}.
		 ''' </p>
		 ''' <p>Evaluating this expression for a given
		 ''' <code>objectName</code> includes performing {@link
		 ''' MBeanServer#isInstanceOf MBeanServer.isInstanceOf(objectName,
		 ''' ((StringValueExp)classNameValue.apply(objectName)).getValue()}.</p>
		 ''' </summary>
		 ''' <param name="classNameValue"> The <seealso cref="StringValueExp"/> returning the name
		 '''        of the class of which selected MBeans should be instances. </param>
		 ''' <returns> a query expression that represents an inheritance
		 ''' constraint on an MBean class.  The returned object will be
		 ''' serialized as an instance of the non-public class
		 ''' <a href="../../serialized-form.html#javax.management.InstanceOfQueryExp">
		 ''' javax.management.InstanceOfQueryExp</a>.
		 ''' @since 1.6 </returns>
		 Public Shared Function isInstanceOf(ByVal classNameValue As StringValueExp) As QueryExp
			Return New InstanceOfQueryExp(classNameValue)
		 End Function

		 ''' <summary>
		 ''' Utility method to escape strings used with
		 ''' Query.{initial|any|final}SubString() methods.
		 ''' </summary>
		 Private Shared Function escapeString(ByVal s As String) As String
			 If s Is Nothing Then Return Nothing
			 s = s.Replace("\", "\\")
			 s = s.Replace("*", "\*")
			 s = s.Replace("?", "\?")
			 s = s.Replace("[", "\[")
			 Return s
		 End Function
	 End Class

End Namespace
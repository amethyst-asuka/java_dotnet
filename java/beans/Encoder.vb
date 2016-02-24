Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.beans



	''' <summary>
	''' An <code>Encoder</code> is a class which can be used to create
	''' files or streams that encode the state of a collection of
	''' JavaBeans in terms of their public APIs. The <code>Encoder</code>,
	''' in conjunction with its persistence delegates, is responsible for
	''' breaking the object graph down into a series of <code>Statements</code>s
	''' and <code>Expression</code>s which can be used to create it.
	''' A subclass typically provides a syntax for these expressions
	''' using some human readable form - like Java source code or XML.
	''' 
	''' @since 1.4
	''' 
	''' @author Philip Milne
	''' </summary>

	Public Class Encoder
		Private ReadOnly finder As New com.sun.beans.finder.PersistenceDelegateFinder
		Private bindings As IDictionary(Of Object, Expression) = New java.util.IdentityHashMap(Of Object, Expression)
		Private exceptionListener As ExceptionListener
		Friend executeStatements As Boolean = True
		Private attributes As IDictionary(Of Object, Object)

		''' <summary>
		''' Write the specified object to the output stream.
		''' The serialized form will denote a series of
		''' expressions, the combined effect of which will create
		''' an equivalent object when the input stream is read.
		''' By default, the object is assumed to be a <em>JavaBean</em>
		''' with a nullary constructor, whose state is defined by
		''' the matching pairs of "setter" and "getter" methods
		''' returned by the Introspector.
		''' </summary>
		''' <param name="o"> The object to be written to the stream.
		''' </param>
		''' <seealso cref= XMLDecoder#readObject </seealso>
		Protected Friend Overridable Sub writeObject(ByVal o As Object)
			If o Is Me Then Return
			Dim info As PersistenceDelegate = getPersistenceDelegate(If(o Is Nothing, Nothing, o.GetType()))
			info.writeObject(o, Me)
		End Sub

		''' <summary>
		''' Sets the exception handler for this stream to <code>exceptionListener</code>.
		''' The exception handler is notified when this stream catches recoverable
		''' exceptions.
		''' </summary>
		''' <param name="exceptionListener"> The exception handler for this stream;
		'''       if <code>null</code> the default exception listener will be used.
		''' </param>
		''' <seealso cref= #getExceptionListener </seealso>
		Public Overridable Property exceptionListener As ExceptionListener
			Set(ByVal exceptionListener As ExceptionListener)
				Me.exceptionListener = exceptionListener
			End Set
			Get
				Return If(exceptionListener IsNot Nothing, exceptionListener, Statement.defaultExceptionListener)
			End Get
		End Property


		Friend Overridable Function getValue(ByVal exp As Expression) As Object
			Try
				Return If(exp Is Nothing, Nothing, exp.value)
			Catch e As Exception
				exceptionListener.exceptionThrown(e)
				Throw New RuntimeException("failed to evaluate: " & exp.ToString())
			End Try
		End Function

		''' <summary>
		''' Returns the persistence delegate for the given type.
		''' The persistence delegate is calculated by applying
		''' the following rules in order:
		''' <ol>
		''' <li>
		''' If a persistence delegate is associated with the given type
		''' by using the <seealso cref="#setPersistenceDelegate"/> method
		''' it is returned.
		''' <li>
		''' A persistence delegate is then looked up by the name
		''' composed of the the fully qualified name of the given type
		''' and the "PersistenceDelegate" postfix.
		''' For example, a persistence delegate for the {@code Bean} class
		''' should be named {@code BeanPersistenceDelegate}
		''' and located in the same package.
		''' <pre>
		''' public class Bean { ... }
		''' public class BeanPersistenceDelegate { ... }</pre>
		''' The instance of the {@code BeanPersistenceDelegate} class
		''' is returned for the {@code Bean} class.
		''' <li>
		''' If the type is {@code null},
		''' a shared internal persistence delegate is returned
		''' that encodes {@code null} value.
		''' <li>
		''' If the type is a {@code enum} declaration,
		''' a shared internal persistence delegate is returned
		''' that encodes constants of this enumeration
		''' by their names.
		''' <li>
		''' If the type is a primitive type or the corresponding wrapper,
		''' a shared internal persistence delegate is returned
		''' that encodes values of the given type.
		''' <li>
		''' If the type is an array,
		''' a shared internal persistence delegate is returned
		''' that encodes an array of the appropriate type and length,
		''' and each of its elements as if they are properties.
		''' <li>
		''' If the type is a proxy,
		''' a shared internal persistence delegate is returned
		''' that encodes a proxy instance by using
		''' the <seealso cref="java.lang.reflect.Proxy#newProxyInstance"/> method.
		''' <li>
		''' If the <seealso cref="BeanInfo"/> for this type has a <seealso cref="BeanDescriptor"/>
		''' which defined a "persistenceDelegate" attribute,
		''' the value of this named attribute is returned.
		''' <li>
		''' In all other cases the default persistence delegate is returned.
		''' The default persistence delegate assumes the type is a <em>JavaBean</em>,
		''' implying that it has a default constructor and that its state
		''' may be characterized by the matching pairs of "setter" and "getter"
		''' methods returned by the <seealso cref="Introspector"/> class.
		''' The default constructor is the constructor with the greatest number
		''' of parameters that has the <seealso cref="ConstructorProperties"/> annotation.
		''' If none of the constructors has the {@code ConstructorProperties} annotation,
		''' then the nullary constructor (constructor with no parameters) will be used.
		''' For example, in the following code fragment, the nullary constructor
		''' for the {@code Foo} class will be used,
		''' while the two-parameter constructor
		''' for the {@code Bar} class will be used.
		''' <pre>
		''' public class Foo {
		'''     public Foo() { ... }
		'''     public Foo(int x) { ... }
		''' }
		''' public class Bar {
		'''     public Bar() { ... }
		'''     &#64;ConstructorProperties({"x"})
		'''     public Bar(int x) { ... }
		'''     &#64;ConstructorProperties({"x", "y"})
		'''     public Bar(int x, int y) { ... }
		''' }</pre>
		''' </ol>
		''' </summary>
		''' <param name="type">  the class of the objects </param>
		''' <returns> the persistence delegate for the given type
		''' </returns>
		''' <seealso cref= #setPersistenceDelegate </seealso>
		''' <seealso cref= java.beans.Introspector#getBeanInfo </seealso>
		''' <seealso cref= java.beans.BeanInfo#getBeanDescriptor </seealso>
		Public Overridable Function getPersistenceDelegate(ByVal type As Class) As PersistenceDelegate
			Dim pd As PersistenceDelegate = Me.finder.find(type)
			If pd Is Nothing Then
				pd = MetaData.getPersistenceDelegate(type)
				If pd IsNot Nothing Then Me.finder.register(type, pd)
			End If
			Return pd
		End Function

		''' <summary>
		''' Associates the specified persistence delegate with the given type.
		''' </summary>
		''' <param name="type">  the class of objects that the specified persistence delegate applies to </param>
		''' <param name="delegate">  the persistence delegate for instances of the given type
		''' </param>
		''' <seealso cref= #getPersistenceDelegate </seealso>
		''' <seealso cref= java.beans.Introspector#getBeanInfo </seealso>
		''' <seealso cref= java.beans.BeanInfo#getBeanDescriptor </seealso>
		Public Overridable Sub setPersistenceDelegate(ByVal type As Class, ByVal [delegate] As PersistenceDelegate)
			Me.finder.register(type, [delegate])
		End Sub

		''' <summary>
		''' Removes the entry for this instance, returning the old entry.
		''' </summary>
		''' <param name="oldInstance"> The entry that should be removed. </param>
		''' <returns> The entry that was removed.
		''' </returns>
		''' <seealso cref= #get </seealso>
		Public Overridable Function remove(ByVal oldInstance As Object) As Object
			Dim exp As Expression = bindings.Remove(oldInstance)
			Return getValue(exp)
		End Function

		''' <summary>
		''' Returns a tentative value for <code>oldInstance</code> in
		''' the environment created by this stream. A persistence
		''' delegate can use its <code>mutatesTo</code> method to
		''' determine whether this value may be initialized to
		''' form the equivalent object at the output or whether
		''' a new object must be instantiated afresh. If the
		''' stream has not yet seen this value, null is returned.
		''' </summary>
		''' <param name="oldInstance"> The instance to be looked up. </param>
		''' <returns> The object, null if the object has not been seen before. </returns>
		Public Overridable Function [get](ByVal oldInstance As Object) As Object
			If oldInstance Is Nothing OrElse oldInstance Is Me OrElse oldInstance.GetType() Is GetType(String) Then Return oldInstance
			Dim exp As Expression = bindings(oldInstance)
			Return getValue(exp)
		End Function

		Private Function writeObject1(ByVal oldInstance As Object) As Object
			Dim o As Object = [get](oldInstance)
			If o Is Nothing Then
				writeObject(oldInstance)
				o = [get](oldInstance)
			End If
			Return o
		End Function

		Private Function cloneStatement(ByVal oldExp As Statement) As Statement
			Dim oldTarget As Object = oldExp.target
			Dim newTarget As Object = writeObject1(oldTarget)

			Dim oldArgs As Object() = oldExp.arguments
			Dim newArgs As Object() = New Object(oldArgs.Length - 1){}
			For i As Integer = 0 To oldArgs.Length - 1
				newArgs(i) = writeObject1(oldArgs(i))
			Next i
			Dim newExp As Statement = If(GetType(Statement).Equals(oldExp.GetType()), New Statement(newTarget, oldExp.methodName, newArgs), New Expression(newTarget, oldExp.methodName, newArgs))
			newExp.loader = oldExp.loader
			Return newExp
		End Function

		''' <summary>
		''' Writes statement <code>oldStm</code> to the stream.
		''' The <code>oldStm</code> should be written entirely
		''' in terms of the callers environment, i.e. the
		''' target and all arguments should be part of the
		''' object graph being written. These expressions
		''' represent a series of "what happened" expressions
		''' which tell the output stream how to produce an
		''' object graph like the original.
		''' <p>
		''' The implementation of this method will produce
		''' a second expression to represent the same expression in
		''' an environment that will exist when the stream is read.
		''' This is achieved simply by calling <code>writeObject</code>
		''' on the target and all the arguments and building a new
		''' expression with the results.
		''' </summary>
		''' <param name="oldStm"> The expression to be written to the stream. </param>
		Public Overridable Sub writeStatement(ByVal oldStm As Statement)
			' System.out.println("writeStatement: " + oldExp);
			Dim newStm As Statement = cloneStatement(oldStm)
			If oldStm.target IsNot Me AndAlso executeStatements Then
				Try
					newStm.execute()
				Catch e As Exception
					exceptionListener.exceptionThrown(New Exception("Encoder: discarding statement " & newStm, e))
				End Try
			End If
		End Sub

		''' <summary>
		''' The implementation first checks to see if an
		''' expression with this value has already been written.
		''' If not, the expression is cloned, using
		''' the same procedure as <code>writeStatement</code>,
		''' and the value of this expression is reconciled
		''' with the value of the cloned expression
		''' by calling <code>writeObject</code>.
		''' </summary>
		''' <param name="oldExp"> The expression to be written to the stream. </param>
		Public Overridable Sub writeExpression(ByVal oldExp As Expression)
			' System.out.println("Encoder::writeExpression: " + oldExp);
			Dim oldValue As Object = getValue(oldExp)
			If [get](oldValue) IsNot Nothing Then Return
			bindings(oldValue) = CType(cloneStatement(oldExp), Expression)
			writeObject(oldValue)
		End Sub

		Friend Overridable Sub clear()
			bindings.Clear()
		End Sub

		' Package private method for setting an attributes table for the encoder
		Friend Overridable Sub setAttribute(ByVal key As Object, ByVal value As Object)
			If attributes Is Nothing Then attributes = New Dictionary(Of )
			attributes(key) = value
		End Sub

		Friend Overridable Function getAttribute(ByVal key As Object) As Object
			If attributes Is Nothing Then Return Nothing
			Return attributes(key)
		End Function
	End Class

End Namespace
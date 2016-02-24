Imports System
Imports sun.reflect.misc

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>DefaultPersistenceDelegate</code> is a concrete implementation of
	''' the abstract <code>PersistenceDelegate</code> class and
	''' is the delegate used by default for classes about
	''' which no information is available. The <code>DefaultPersistenceDelegate</code>
	''' provides, version resilient, public API-based persistence for
	''' classes that follow the JavaBeans&trade; conventions without any class specific
	''' configuration.
	''' <p>
	''' The key assumptions are that the class has a nullary constructor
	''' and that its state is accurately represented by matching pairs
	''' of "setter" and "getter" methods in the order they are returned
	''' by the Introspector.
	''' In addition to providing code-free persistence for JavaBeans,
	''' the <code>DefaultPersistenceDelegate</code> provides a convenient means
	''' to effect persistent storage for classes that have a constructor
	''' that, while not nullary, simply requires some property values
	''' as arguments.
	''' </summary>
	''' <seealso cref= #DefaultPersistenceDelegate(String[]) </seealso>
	''' <seealso cref= java.beans.Introspector
	''' 
	''' @since 1.4
	''' 
	''' @author Philip Milne </seealso>

	Public Class DefaultPersistenceDelegate
		Inherits PersistenceDelegate

		Private Shared ReadOnly EMPTY As String() = {}
		Private ReadOnly constructor As String()
		Private definesEquals_Renamed As Boolean?

		''' <summary>
		''' Creates a persistence delegate for a class with a nullary constructor.
		''' </summary>
		''' <seealso cref= #DefaultPersistenceDelegate(java.lang.String[]) </seealso>
		Public Sub New()
			Me.constructor = EMPTY
		End Sub

		''' <summary>
		''' Creates a default persistence delegate for a class with a
		''' constructor whose arguments are the values of the property
		''' names as specified by <code>constructorPropertyNames</code>.
		''' The constructor arguments are created by
		''' evaluating the property names in the order they are supplied.
		''' To use this class to specify a single preferred constructor for use
		''' in the serialization of a particular type, we state the
		''' names of the properties that make up the constructor's
		''' arguments. For example, the <code>Font</code> class which
		''' does not define a nullary constructor can be handled
		''' with the following persistence delegate:
		''' 
		''' <pre>
		'''     new DefaultPersistenceDelegate(new String[]{"name", "style", "size"});
		''' </pre>
		''' </summary>
		''' <param name="constructorPropertyNames"> The property names for the arguments of this constructor.
		''' </param>
		''' <seealso cref= #instantiate </seealso>
		Public Sub New(ByVal constructorPropertyNames As String())
			Me.constructor = If(constructorPropertyNames Is Nothing, EMPTY, constructorPropertyNames.clone())
		End Sub

		Private Shared Function definesEquals(ByVal type As Class) As Boolean
			Try
				Return type Is type.getMethod("equals", GetType(Object)).declaringClass
			Catch e As NoSuchMethodException
				Return False
			End Try
		End Function

		Private Function definesEquals(ByVal instance As Object) As Boolean
			If definesEquals_Renamed IsNot Nothing Then
				Return (definesEquals_Renamed Is Boolean.TRUE)
			Else
				Dim result As Boolean = definesEquals(instance.GetType())
				definesEquals_Renamed = If(result, Boolean.TRUE, Boolean.FALSE)
				Return result
			End If
		End Function

		''' <summary>
		''' If the number of arguments in the specified constructor is non-zero and
		''' the class of <code>oldInstance</code> explicitly declares an "equals" method
		''' this method returns the value of <code>oldInstance.equals(newInstance)</code>.
		''' Otherwise, this method uses the superclass's definition which returns true if the
		''' classes of the two instances are equal.
		''' </summary>
		''' <param name="oldInstance"> The instance to be copied. </param>
		''' <param name="newInstance"> The instance that is to be modified. </param>
		''' <returns> True if an equivalent copy of <code>newInstance</code> may be
		'''         created by applying a series of mutations to <code>oldInstance</code>.
		''' </returns>
		''' <seealso cref= #DefaultPersistenceDelegate(String[]) </seealso>
		Protected Friend Overrides Function mutatesTo(ByVal oldInstance As Object, ByVal newInstance As Object) As Boolean
			' Assume the instance is either mutable or a singleton
			' if it has a nullary constructor.
			Return If((constructor.Length = 0) OrElse (Not definesEquals(oldInstance)), MyBase.mutatesTo(oldInstance, newInstance), oldInstance.Equals(newInstance))
		End Function

		''' <summary>
		''' This default implementation of the <code>instantiate</code> method returns
		''' an expression containing the predefined method name "new" which denotes a
		''' call to a constructor with the arguments as specified in
		''' the <code>DefaultPersistenceDelegate</code>'s constructor.
		''' </summary>
		''' <param name="oldInstance"> The instance to be instantiated. </param>
		''' <param name="out"> The code output stream. </param>
		''' <returns> An expression whose value is <code>oldInstance</code>.
		''' </returns>
		''' <exception cref="NullPointerException"> if {@code out} is {@code null}
		'''                              and this value is used in the method
		''' </exception>
		''' <seealso cref= #DefaultPersistenceDelegate(String[]) </seealso>
		Protected Friend Overrides Function instantiate(ByVal oldInstance As Object, ByVal out As Encoder) As Expression
			Dim nArgs As Integer = constructor.Length
			Dim type As Class = oldInstance.GetType()
			Dim constructorArgs As Object() = New Object(nArgs - 1){}
			For i As Integer = 0 To nArgs - 1
				Try
					Dim method As Method = findMethod(type, Me.constructor(i))
					constructorArgs(i) = MethodUtil.invoke(method, oldInstance, New Object(){})
				Catch e As Exception
					out.exceptionListener.exceptionThrown(e)
				End Try
			Next i
			Return New Expression(oldInstance, oldInstance.GetType(), "new", constructorArgs)
		End Function

		Private Function findMethod(ByVal type As Class, ByVal [property] As String) As Method
			If [property] Is Nothing Then Throw New IllegalArgumentException("Property name is null")
			Dim pd As PropertyDescriptor = getPropertyDescriptor(type, [property])
			If pd Is Nothing Then Throw New IllegalStateException("Could not find property by the name " & [property])
			Dim method As Method = pd.readMethod
			If method Is Nothing Then Throw New IllegalStateException("Could not find getter for the property " & [property])
			Return method
		End Function

		Private Sub doProperty(ByVal type As Class, ByVal pd As PropertyDescriptor, ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			Dim getter As Method = pd.readMethod
			Dim setter As Method = pd.writeMethod

			If getter IsNot Nothing AndAlso setter IsNot Nothing Then
				Dim oldGetExp As New Expression(oldInstance, getter.name, New Object(){})
				Dim newGetExp As New Expression(newInstance, getter.name, New Object(){})
				Dim oldValue As Object = oldGetExp.value
				Dim newValue As Object = newGetExp.value
				out.writeExpression(oldGetExp)
				If Not java.util.Objects.Equals(newValue, out.get(oldValue)) Then
					' Search for a static constant with this value;
					Dim e As Object = CType(pd.getValue("enumerationValues"), Object())
					If TypeOf e Is Object() AndAlso Array.getLength(e) Mod 3 = 0 Then
						Dim a As Object() = CType(e, Object())
						For i As Integer = 0 To a.Length - 1 Step 3
							Try
							   Dim f As Field = type.getField(CStr(a(i)))
							   If f.get(Nothing).Equals(oldValue) Then
								   out.remove(oldValue)
								   out.writeExpression(New Expression(oldValue, f, "get", New Object(){Nothing}))
							   End If
							Catch ex As Exception
							End Try
						Next i
					End If
					invokeStatement(oldInstance, setter.name, New Object(){oldValue}, out)
				End If
			End If
		End Sub

		Friend Shared Sub invokeStatement(ByVal instance As Object, ByVal methodName As String, ByVal args As Object(), ByVal out As Encoder)
			out.writeStatement(New Statement(instance, methodName, args))
		End Sub

		' Write out the properties of this instance.
		Private Sub initBean(ByVal type As Class, ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			For Each field As Field In type.fields
				If Not ReflectUtil.isPackageAccessible(field.declaringClass) Then Continue For
				Dim [mod] As Integer = field.modifiers
				If Modifier.isFinal([mod]) OrElse Modifier.isStatic([mod]) OrElse Modifier.isTransient([mod]) Then Continue For
				Try
					Dim oldGetExp As New Expression(field, "get", New Object() { oldInstance })
					Dim newGetExp As New Expression(field, "get", New Object() { newInstance })
					Dim oldValue As Object = oldGetExp.value
					Dim newValue As Object = newGetExp.value
					out.writeExpression(oldGetExp)
					If Not java.util.Objects.Equals(newValue, out.get(oldValue)) Then out.writeStatement(New Statement(field, "set", New Object() { oldInstance, oldValue }))
				Catch exception_Renamed As Exception
					out.exceptionListener.exceptionThrown(exception_Renamed)
				End Try
			Next field
			Dim info As BeanInfo
			Try
				info = Introspector.getBeanInfo(type)
			Catch exception_Renamed As IntrospectionException
				Return
			End Try
			' Properties
			For Each d As PropertyDescriptor In info.propertyDescriptors
				If d.transient Then Continue For
				Try
					doProperty(type, d, oldInstance, newInstance, out)
				Catch e As Exception
					out.exceptionListener.exceptionThrown(e)
				End Try
			Next d

			' Listeners
	'        
	'        Pending(milne). There is a general problem with the archival of
	'        listeners which is unresolved as of 1.4. Many of the methods
	'        which install one object inside another (typically "add" methods
	'        or setters) automatically install a listener on the "child" object
	'        so that its "parent" may respond to changes that are made to it.
	'        For example the JTable:setModel() method automatically adds a
	'        TableModelListener (the JTable itself in this case) to the supplied
	'        table model.
	'
	'        We do not need to explicitly add these listeners to the model in an
	'        archive as they will be added automatically by, in the above case,
	'        the JTable's "setModel" method. In some cases, we must specifically
	'        avoid trying to do this since the listener may be an inner class
	'        that cannot be instantiated using public API.
	'
	'        No general mechanism currently
	'        exists for differentiating between these kind of listeners and
	'        those which were added explicitly by the user. A mechanism must
	'        be created to provide a general means to differentiate these
	'        special cases so as to provide reliable persistence of listeners
	'        for the general case.
	'        
			If Not type.IsSubclassOf(GetType(java.awt.Component)) Then Return ' Just handle the listeners of Components for now.
			For Each d As EventSetDescriptor In info.eventSetDescriptors
				If d.transient Then Continue For
				Dim listenerType As Class = d.listenerType


				' The ComponentListener is added automatically, when
				' Contatiner:add is called on the parent.
				If listenerType Is GetType(java.awt.event.ComponentListener) Then Continue For

				' JMenuItems have a change listener added to them in
				' their "add" methods to enable accessibility support -
				' see the add method in JMenuItem for details. We cannot
				' instantiate this instance as it is a private inner class
				' and do not need to do this anyway since it will be created
				' and installed by the "add" method. Special case this for now,
				' ignoring all change listeners on JMenuItems.
				If listenerType Is GetType(javax.swing.event.ChangeListener) AndAlso type Is GetType(javax.swing.JMenuItem) Then Continue For

				Dim oldL As EventListener() = New EventListener(){}
				Dim newL As EventListener() = New EventListener(){}
				Try
					Dim m As Method = d.getListenerMethod
					oldL = CType(MethodUtil.invoke(m, oldInstance, New Object(){}), EventListener())
					newL = CType(MethodUtil.invoke(m, newInstance, New Object(){}), EventListener())
				Catch e2 As Exception
					Try
						Dim m As Method = type.getMethod("getListeners", New [Class](){GetType(Class)})
						oldL = CType(MethodUtil.invoke(m, oldInstance, New Object(){listenerType}), EventListener())
						newL = CType(MethodUtil.invoke(m, newInstance, New Object(){listenerType}), EventListener())
					Catch e3 As Exception
						Return
					End Try
				End Try

				' Asssume the listeners are in the same order and that there are no gaps.
				' Eventually, this may need to do true differencing.
				Dim addListenerMethodName As String = d.addListenerMethod.name
				For i As Integer = newL.Length To oldL.Length - 1
					' System.out.println("Adding listener: " + addListenerMethodName + oldL[i]);
					invokeStatement(oldInstance, addListenerMethodName, New Object(){oldL(i)}, out)
				Next i

				Dim removeListenerMethodName As String = d.removeListenerMethod.name
				For i As Integer = oldL.Length To newL.Length - 1
					invokeStatement(oldInstance, removeListenerMethodName, New Object(){newL(i)}, out)
				Next i
			Next d
		End Sub

		''' <summary>
		''' This default implementation of the <code>initialize</code> method assumes
		''' all state held in objects of this type is exposed via the
		''' matching pairs of "setter" and "getter" methods in the order
		''' they are returned by the Introspector. If a property descriptor
		''' defines a "transient" attribute with a value equal to
		''' <code>Boolean.TRUE</code> the property is ignored by this
		''' default implementation. Note that this use of the word
		''' "transient" is quite independent of the field modifier
		''' that is used by the <code>ObjectOutputStream</code>.
		''' <p>
		''' For each non-transient property, an expression is created
		''' in which the nullary "getter" method is applied
		''' to the <code>oldInstance</code>. The value of this
		''' expression is the value of the property in the instance that is
		''' being serialized. If the value of this expression
		''' in the cloned environment <code>mutatesTo</code> the
		''' target value, the new value is initialized to make it
		''' equivalent to the old value. In this case, because
		''' the property value has not changed there is no need to
		''' call the corresponding "setter" method and no statement
		''' is emitted. If not however, the expression for this value
		''' is replaced with another expression (normally a constructor)
		''' and the corresponding "setter" method is called to install
		''' the new property value in the object. This scheme removes
		''' default information from the output produced by streams
		''' using this delegate.
		''' <p>
		''' In passing these statements to the output stream, where they
		''' will be executed, side effects are made to the <code>newInstance</code>.
		''' In most cases this allows the problem of properties
		''' whose values depend on each other to actually help the
		''' serialization process by making the number of statements
		''' that need to be written to the output smaller. In general,
		''' the problem of handling interdependent properties is reduced to
		''' that of finding an order for the properties in
		''' a class such that no property value depends on the value of
		''' a subsequent property.
		''' </summary>
		''' <param name="type"> the type of the instances </param>
		''' <param name="oldInstance"> The instance to be copied. </param>
		''' <param name="newInstance"> The instance that is to be modified. </param>
		''' <param name="out"> The stream to which any initialization statements should be written.
		''' </param>
		''' <exception cref="NullPointerException"> if {@code out} is {@code null}
		''' </exception>
		''' <seealso cref= java.beans.Introspector#getBeanInfo </seealso>
		''' <seealso cref= java.beans.PropertyDescriptor </seealso>
		Protected Friend Overrides Sub initialize(ByVal type As Class, ByVal oldInstance As Object, ByVal newInstance As Object, ByVal out As Encoder)
			' System.out.println("DefulatPD:initialize" + type);
			MyBase.initialize(type, oldInstance, newInstance, out)
			If oldInstance.GetType() Is type Then ' !type.isInterface()) { initBean(type, oldInstance, newInstance, out)
		End Sub

		Private Shared Function getPropertyDescriptor(ByVal type As Class, ByVal [property] As String) As PropertyDescriptor
			Try
				For Each pd As PropertyDescriptor In Introspector.getBeanInfo(type).propertyDescriptors
					If [property].Equals(pd.name) Then Return pd
				Next pd
			Catch exception_Renamed As IntrospectionException
			End Try
			Return Nothing
		End Function
	End Class

End Namespace
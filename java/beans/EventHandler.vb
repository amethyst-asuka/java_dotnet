Imports System
Imports System.Threading

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
	''' The <code>EventHandler</code> class provides
	''' support for dynamically generating event listeners whose methods
	''' execute a simple statement involving an incoming event object
	''' and a target object.
	''' <p>
	''' The <code>EventHandler</code> class is intended to be used by interactive tools, such as
	''' application builders, that allow developers to make connections between
	''' beans. Typically connections are made from a user interface bean
	''' (the event <em>source</em>)
	''' to an application logic bean (the <em>target</em>). The most effective
	''' connections of this kind isolate the application logic from the user
	''' interface.  For example, the <code>EventHandler</code> for a
	''' connection from a <code>JCheckBox</code> to a method
	''' that accepts a boolean value can deal with extracting the state
	''' of the check box and passing it directly to the method so that
	''' the method is isolated from the user interface layer.
	''' <p>
	''' Inner classes are another, more general way to handle events from
	''' user interfaces.  The <code>EventHandler</code> class
	''' handles only a subset of what is possible using inner
	''' classes. However, <code>EventHandler</code> works better
	''' with the long-term persistence scheme than inner classes.
	''' Also, using <code>EventHandler</code> in large applications in
	''' which the same interface is implemented many times can
	''' reduce the disk and memory footprint of the application.
	''' <p>
	''' The reason that listeners created with <code>EventHandler</code>
	''' have such a small
	''' footprint is that the <code>Proxy</code> [Class], on which
	''' the <code>EventHandler</code> relies, shares implementations
	''' of identical
	''' interfaces. For example, if you use
	''' the <code>EventHandler</code> <code>create</code> methods to make
	''' all the <code>ActionListener</code>s in an application,
	''' all the action listeners will be instances of a single class
	''' (one created by the <code>Proxy</code> [Class]).
	''' In general, listeners based on
	''' the <code>Proxy</code> class require one listener class
	''' to be created per <em>listener type</em> (interface),
	''' whereas the inner class
	''' approach requires one class to be created per <em>listener</em>
	''' (object that implements the interface).
	''' 
	''' <p>
	''' You don't generally deal directly with <code>EventHandler</code>
	''' instances.
	''' Instead, you use one of the <code>EventHandler</code>
	''' <code>create</code> methods to create
	''' an object that implements a given listener interface.
	''' This listener object uses an <code>EventHandler</code> object
	''' behind the scenes to encapsulate information about the
	''' event, the object to be sent a message when the event occurs,
	''' the message (method) to be sent, and any argument
	''' to the method.
	''' The following section gives examples of how to create listener
	''' objects using the <code>create</code> methods.
	''' 
	''' <h2>Examples of Using EventHandler</h2>
	''' 
	''' The simplest use of <code>EventHandler</code> is to install
	''' a listener that calls a method on the target object with no arguments.
	''' In the following example we create an <code>ActionListener</code>
	''' that invokes the <code>toFront</code> method on an instance
	''' of <code>javax.swing.JFrame</code>.
	''' 
	''' <blockquote>
	''' <pre>
	''' myButton.addActionListener(
	'''    (ActionListener)EventHandler.create(ActionListener.class, frame, "toFront"));
	''' </pre>
	''' </blockquote>
	''' 
	''' When <code>myButton</code> is pressed, the statement
	''' <code>frame.toFront()</code> will be executed.  One could get
	''' the same effect, with some additional compile-time type safety,
	''' by defining a new implementation of the <code>ActionListener</code>
	''' interface and adding an instance of it to the button:
	''' 
	''' <blockquote>
	''' <pre>
	''' //Equivalent code using an inner class instead of EventHandler.
	''' myButton.addActionListener(new ActionListener() {
	'''    public  Sub  actionPerformed(ActionEvent e) {
	'''        frame.toFront();
	'''    }
	''' });
	''' </pre>
	''' </blockquote>
	''' 
	''' The next simplest use of <code>EventHandler</code> is
	''' to extract a property value from the first argument
	''' of the method in the listener interface (typically an event object)
	''' and use it to set the value of a property in the target object.
	''' In the following example we create an <code>ActionListener</code> that
	''' sets the <code>nextFocusableComponent</code> property of the target
	''' (myButton) object to the value of the "source" property of the event.
	''' 
	''' <blockquote>
	''' <pre>
	''' EventHandler.create(ActionListener.class, myButton, "nextFocusableComponent", "source")
	''' </pre>
	''' </blockquote>
	''' 
	''' This would correspond to the following inner class implementation:
	''' 
	''' <blockquote>
	''' <pre>
	''' //Equivalent code using an inner class instead of EventHandler.
	''' new ActionListener() {
	'''    public  Sub  actionPerformed(ActionEvent e) {
	'''        myButton.setNextFocusableComponent((Component)e.getSource());
	'''    }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' It's also possible to create an <code>EventHandler</code> that
	''' just passes the incoming event object to the target's action.
	''' If the fourth <code>EventHandler.create</code> argument is
	''' an empty string, then the event is just passed along:
	''' 
	''' <blockquote>
	''' <pre>
	''' EventHandler.create(ActionListener.class, target, "doActionEvent", "")
	''' </pre>
	''' </blockquote>
	''' 
	''' This would correspond to the following inner class implementation:
	''' 
	''' <blockquote>
	''' <pre>
	''' //Equivalent code using an inner class instead of EventHandler.
	''' new ActionListener() {
	'''    public  Sub  actionPerformed(ActionEvent e) {
	'''        target.doActionEvent(e);
	'''    }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Probably the most common use of <code>EventHandler</code>
	''' is to extract a property value from the
	''' <em>source</em> of the event object and set this value as
	''' the value of a property of the target object.
	''' In the following example we create an <code>ActionListener</code> that
	''' sets the "label" property of the target
	''' object to the value of the "text" property of the
	''' source (the value of the "source" property) of the event.
	''' 
	''' <blockquote>
	''' <pre>
	''' EventHandler.create(ActionListener.class, myButton, "label", "source.text")
	''' </pre>
	''' </blockquote>
	''' 
	''' This would correspond to the following inner class implementation:
	''' 
	''' <blockquote>
	''' <pre>
	''' //Equivalent code using an inner class instead of EventHandler.
	''' new ActionListener {
	'''    public  Sub  actionPerformed(ActionEvent e) {
	'''        myButton.setLabel(((JTextField)e.getSource()).getText());
	'''    }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' The event property may be "qualified" with an arbitrary number
	''' of property prefixes delimited with the "." character. The "qualifying"
	''' names that appear before the "." characters are taken as the names of
	''' properties that should be applied, left-most first, to
	''' the event object.
	''' <p>
	''' For example, the following action listener
	''' 
	''' <blockquote>
	''' <pre>
	''' EventHandler.create(ActionListener.class, target, "a", "b.c.d")
	''' </pre>
	''' </blockquote>
	''' 
	''' might be written as the following inner class
	''' (assuming all the properties had canonical getter methods and
	''' returned the appropriate types):
	''' 
	''' <blockquote>
	''' <pre>
	''' //Equivalent code using an inner class instead of EventHandler.
	''' new ActionListener {
	'''    public  Sub  actionPerformed(ActionEvent e) {
	'''        target.setA(e.getB().getC().isD());
	'''    }
	''' }
	''' </pre>
	''' </blockquote>
	''' The target property may also be "qualified" with an arbitrary number
	''' of property prefixs delimited with the "." character.  For example, the
	''' following action listener:
	''' <pre>
	'''   EventHandler.create(ActionListener.class, target, "a.b", "c.d")
	''' </pre>
	''' might be written as the following inner class
	''' (assuming all the properties had canonical getter methods and
	''' returned the appropriate types):
	''' <pre>
	'''   //Equivalent code using an inner class instead of EventHandler.
	'''   new ActionListener {
	'''     public  Sub  actionPerformed(ActionEvent e) {
	'''         target.getA().setB(e.getC().isD());
	'''    }
	''' }
	''' </pre>
	''' <p>
	''' As <code>EventHandler</code> ultimately relies on reflection to invoke
	''' a method we recommend against targeting an overloaded method.  For example,
	''' if the target is an instance of the class <code>MyTarget</code> which is
	''' defined as:
	''' <pre>
	'''   public class MyTarget {
	'''     public  Sub  doIt(String);
	'''     public  Sub  doIt(Object);
	'''   }
	''' </pre>
	''' Then the method <code>doIt</code> is overloaded.  EventHandler will invoke
	''' the method that is appropriate based on the source.  If the source is
	''' null, then either method is appropriate and the one that is invoked is
	''' undefined.  For that reason we recommend against targeting overloaded
	''' methods.
	''' </summary>
	''' <seealso cref= java.lang.reflect.Proxy </seealso>
	''' <seealso cref= java.util.EventObject
	''' 
	''' @since 1.4
	''' 
	''' @author Mark Davidson
	''' @author Philip Milne
	''' @author Hans Muller
	'''  </seealso>
	Public Class EventHandler
		Implements InvocationHandler

		Private target As Object
		Private action As String
		Private ReadOnly eventPropertyName As String
		Private ReadOnly listenerMethodName As String
		Private ReadOnly acc As java.security.AccessControlContext = java.security.AccessController.context

		''' <summary>
		''' Creates a new <code>EventHandler</code> object;
		''' you generally use one of the <code>create</code> methods
		''' instead of invoking this constructor directly.  Refer to
		''' {@link java.beans.EventHandler#create(Class, Object, String, String)
		''' the general version of create} for a complete description of
		''' the <code>eventPropertyName</code> and <code>listenerMethodName</code>
		''' parameter.
		''' </summary>
		''' <param name="target"> the object that will perform the action </param>
		''' <param name="action"> the name of a (possibly qualified) property or method on
		'''        the target </param>
		''' <param name="eventPropertyName"> the (possibly qualified) name of a readable property of the incoming event </param>
		''' <param name="listenerMethodName"> the name of the method in the listener interface that should trigger the action
		''' </param>
		''' <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>action</code> is null
		''' </exception>
		''' <seealso cref= EventHandler </seealso>
		''' <seealso cref= #create(Class, Object, String, String, String) </seealso>
		''' <seealso cref= #getTarget </seealso>
		''' <seealso cref= #getAction </seealso>
		''' <seealso cref= #getEventPropertyName </seealso>
		''' <seealso cref= #getListenerMethodName </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(  target As Object,   action As String,   eventPropertyName As String,   listenerMethodName As String)
			Me.target = target
			Me.action = action
			If target Is Nothing Then Throw New NullPointerException("target must be non-null")
			If action Is Nothing Then Throw New NullPointerException("action must be non-null")
			Me.eventPropertyName = eventPropertyName
			Me.listenerMethodName = listenerMethodName
		End Sub

		''' <summary>
		''' Returns the object to which this event handler will send a message.
		''' </summary>
		''' <returns> the target of this event handler </returns>
		''' <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		Public Overridable Property target As Object
			Get
				Return target
			End Get
		End Property

		''' <summary>
		''' Returns the name of the target's writable property
		''' that this event handler will set,
		''' or the name of the method that this event handler
		''' will invoke on the target.
		''' </summary>
		''' <returns> the action of this event handler </returns>
		''' <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		Public Overridable Property action As String
			Get
				Return action
			End Get
		End Property

		''' <summary>
		''' Returns the property of the event that should be
		''' used in the action applied to the target.
		''' </summary>
		''' <returns> the property of the event
		''' </returns>
		''' <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		Public Overridable Property eventPropertyName As String
			Get
				Return eventPropertyName
			End Get
		End Property

		''' <summary>
		''' Returns the name of the method that will trigger the action.
		''' A return value of <code>null</code> signifies that all methods in the
		''' listener interface trigger the action.
		''' </summary>
		''' <returns> the name of the method that will trigger the action
		''' </returns>
		''' <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		Public Overridable Property listenerMethodName As String
			Get
				Return listenerMethodName
			End Get
		End Property

		Private Function applyGetters(  target As Object,   getters As String) As Object
			If getters Is Nothing OrElse getters.Equals("") Then Return target
			Dim firstDot As Integer = getters.IndexOf("."c)
			If firstDot = -1 Then firstDot = getters.length()
			Dim first As String = getters.Substring(0, firstDot)
			Dim rest As String = getters.Substring (System.Math.Min(firstDot + 1, getters.length()))

			Try
				Dim getter As Method = Nothing
				If target IsNot Nothing Then
					getter = Statement.getMethod(target.GetType(), "get" & NameGenerator.capitalize(first), New [Class](){})
					If getter Is Nothing Then getter = Statement.getMethod(target.GetType(), "is" & NameGenerator.capitalize(first), New [Class](){})
					If getter Is Nothing Then getter = Statement.getMethod(target.GetType(), first, New [Class](){})
				End If
				If getter Is Nothing Then Throw New RuntimeException("No method called: " & first & " defined on " & target)
				Dim newTarget As Object = sun.reflect.misc.MethodUtil.invoke(getter, target, New Object(){})
				Return applyGetters(newTarget, rest)
			Catch e As Exception
				Throw New RuntimeException("Failed to call method: " & first & " on " & target, e)
			End Try
		End Function

		''' <summary>
		''' Extract the appropriate property value from the event and
		''' pass it to the action associated with
		''' this <code>EventHandler</code>.
		''' </summary>
		''' <param name="proxy"> the proxy object </param>
		''' <param name="method"> the method in the listener interface </param>
		''' <returns> the result of applying the action to the target
		''' </returns>
		''' <seealso cref= EventHandler </seealso>
		Public Overridable Function invoke(  proxy As Object,   method As Method,   arguments As Object()) As Object Implements InvocationHandler.invoke
			Dim acc As java.security.AccessControlContext = Me.acc
			If (acc Is Nothing) AndAlso (System.securityManager IsNot Nothing) Then Throw New SecurityException("AccessControlContext is not set")
            Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T))
        End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				Return outerInstance.invokeInternal(proxy, method, arguments)
			End Function
		End Class

		Private Function invokeInternal(  proxy As Object,   method As Method,   arguments As Object()) As Object
			Dim methodName As String = method.name
			If method.declaringClass = GetType(Object) Then
				' Handle the Object public methods.
				If methodName.Equals("hashCode") Then
					Return New Integer?(System.identityHashCode(proxy))
				ElseIf methodName.Equals("equals") Then
					Return (If(proxy Is arguments(0),  java.lang.[Boolean].TRUE,  java.lang.[Boolean].FALSE))
				ElseIf methodName.Equals("toString") Then
					Return proxy.GetType().name + AscW("@"c) +  java.lang.[Integer].toHexString(proxy.GetHashCode())
				End If
			End If

			If listenerMethodName Is Nothing OrElse listenerMethodName.Equals(methodName) Then
				Dim argTypes As  [Class]() = Nothing
				Dim newArgs As Object() = Nothing

				If eventPropertyName Is Nothing Then ' Nullary method.
					newArgs = New Object(){}
					argTypes = New [Class](){}
				Else
					Dim input As Object = applyGetters(arguments(0), eventPropertyName)
					newArgs = New Object(){input}
					argTypes = New [Class](){If(input Is Nothing, Nothing, input.GetType())}
				End If
				Try
					Dim lastDot As Integer = action.LastIndexOf("."c)
					If lastDot <> -1 Then
						target = applyGetters(target, action.Substring(0, lastDot))
						action = action.Substring(lastDot + 1)
					End If
					Dim targetMethod As Method = Statement.getMethod(target.GetType(), action, argTypes)
					If targetMethod Is Nothing Then targetMethod = Statement.getMethod(target.GetType(), "set" & NameGenerator.capitalize(action), argTypes)
					If targetMethod Is Nothing Then
						Dim argTypeString As String = If(argTypes.Length = 0, " with no arguments", " with argument " & argTypes(0))
						Throw New RuntimeException("No method called " & action & " on " & target.GetType() + argTypeString)
					End If
					Return sun.reflect.misc.MethodUtil.invoke(targetMethod, target, newArgs)
				Catch ex As IllegalAccessException
					Throw New RuntimeException(ex)
				Catch ex As InvocationTargetException
					Dim th As Throwable = ex.targetException
					Throw If(TypeOf th Is RuntimeException, CType(th, RuntimeException), New RuntimeException(th))
				End Try
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Creates an implementation of <code>listenerInterface</code> in which
		''' <em>all</em> of the methods in the listener interface apply
		''' the handler's <code>action</code> to the <code>target</code>. This
		''' method is implemented by calling the other, more general,
		''' implementation of the <code>create</code> method with both
		''' the <code>eventPropertyName</code> and the <code>listenerMethodName</code>
		''' taking the value <code>null</code>. Refer to
		''' {@link java.beans.EventHandler#create(Class, Object, String, String)
		''' the general version of create} for a complete description of
		''' the <code>action</code> parameter.
		''' <p>
		''' To create an <code>ActionListener</code> that shows a
		''' <code>JDialog</code> with <code>dialog.show()</code>,
		''' one can write:
		''' 
		''' <blockquote>
		''' <pre>
		''' EventHandler.create(ActionListener.class, dialog, "show")
		''' </pre>
		''' </blockquote>
		''' </summary>
		''' @param <T> the type to create </param>
		''' <param name="listenerInterface"> the listener interface to create a proxy for </param>
		''' <param name="target"> the object that will perform the action </param>
		''' <param name="action"> the name of a (possibly qualified) property or method on
		'''        the target </param>
		''' <returns> an object that implements <code>listenerInterface</code>
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>listenerInterface</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>action</code> is null
		''' </exception>
		''' <seealso cref= #create(Class, Object, String, String) </seealso>
		Public Shared Function create(Of T)(  listenerInterface As [Class],   target As Object,   action As String) As T
			Return create(listenerInterface, target, action, Nothing, Nothing)
		End Function

		''' <summary>
		''' /**
		''' Creates an implementation of <code>listenerInterface</code> in which
		''' <em>all</em> of the methods pass the value of the event
		''' expression, <code>eventPropertyName</code>, to the final method in the
		''' statement, <code>action</code>, which is applied to the <code>target</code>.
		''' This method is implemented by calling the
		''' more general, implementation of the <code>create</code> method with
		''' the <code>listenerMethodName</code> taking the value <code>null</code>.
		''' Refer to
		''' {@link java.beans.EventHandler#create(Class, Object, String, String)
		''' the general version of create} for a complete description of
		''' the <code>action</code> and <code>eventPropertyName</code> parameters.
		''' <p>
		''' To create an <code>ActionListener</code> that sets the
		''' the text of a <code>JLabel</code> to the text value of
		''' the <code>JTextField</code> source of the incoming event,
		''' you can use the following code:
		''' 
		''' <blockquote>
		''' <pre>
		''' EventHandler.create(ActionListener.class, label, "text", "source.text");
		''' </pre>
		''' </blockquote>
		''' 
		''' This is equivalent to the following code:
		''' <blockquote>
		''' <pre>
		''' //Equivalent code using an inner class instead of EventHandler.
		''' new ActionListener() {
		'''    public  Sub  actionPerformed(ActionEvent event) {
		'''        label.setText(((JTextField)(event.getSource())).getText());
		'''     }
		''' };
		''' </pre>
		''' </blockquote>
		''' </summary>
		''' @param <T> the type to create </param>
		''' <param name="listenerInterface"> the listener interface to create a proxy for </param>
		''' <param name="target"> the object that will perform the action </param>
		''' <param name="action"> the name of a (possibly qualified) property or method on
		'''        the target </param>
		''' <param name="eventPropertyName"> the (possibly qualified) name of a readable property of the incoming event
		''' </param>
		''' <returns> an object that implements <code>listenerInterface</code>
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>listenerInterface</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>action</code> is null
		''' </exception>
		''' <seealso cref= #create(Class, Object, String, String, String) </seealso>
		Public Shared Function create(Of T)(  listenerInterface As [Class],   target As Object,   action As String,   eventPropertyName As String) As T
			Return create(listenerInterface, target, action, eventPropertyName, Nothing)
		End Function

		''' <summary>
		''' Creates an implementation of <code>listenerInterface</code> in which
		''' the method named <code>listenerMethodName</code>
		''' passes the value of the event expression, <code>eventPropertyName</code>,
		''' to the final method in the statement, <code>action</code>, which
		''' is applied to the <code>target</code>. All of the other listener
		''' methods do nothing.
		''' <p>
		''' The <code>eventPropertyName</code> string is used to extract a value
		''' from the incoming event object that is passed to the target
		''' method.  The common case is the target method takes no arguments, in
		''' which case a value of null should be used for the
		''' <code>eventPropertyName</code>.  Alternatively if you want
		''' the incoming event object passed directly to the target method use
		''' the empty string.
		''' The format of the <code>eventPropertyName</code> string is a sequence of
		''' methods or properties where each method or
		''' property is applied to the value returned by the preceding method
		''' starting from the incoming event object.
		''' The syntax is: <code>propertyName{.propertyName}*</code>
		''' where <code>propertyName</code> matches a method or
		''' property.  For example, to extract the <code>point</code>
		''' property from a <code>MouseEvent</code>, you could use either
		''' <code>"point"</code> or <code>"getPoint"</code> as the
		''' <code>eventPropertyName</code>.  To extract the "text" property from
		''' a <code>MouseEvent</code> with a <code>JLabel</code> source use any
		''' of the following as <code>eventPropertyName</code>:
		''' <code>"source.text"</code>,
		''' <code>"getSource.text"</code> <code>"getSource.getText"</code> or
		''' <code>"source.getText"</code>.  If a method can not be found, or an
		''' exception is generated as part of invoking a method a
		''' <code>RuntimeException</code> will be thrown at dispatch time.  For
		''' example, if the incoming event object is null, and
		''' <code>eventPropertyName</code> is non-null and not empty, a
		''' <code>RuntimeException</code> will be thrown.
		''' <p>
		''' The <code>action</code> argument is of the same format as the
		''' <code>eventPropertyName</code> argument where the last property name
		''' identifies either a method name or writable property.
		''' <p>
		''' If the <code>listenerMethodName</code> is <code>null</code>
		''' <em>all</em> methods in the interface trigger the <code>action</code> to be
		''' executed on the <code>target</code>.
		''' <p>
		''' For example, to create a <code>MouseListener</code> that sets the target
		''' object's <code>origin</code> property to the incoming <code>MouseEvent</code>'s
		''' location (that's the value of <code>mouseEvent.getPoint()</code>) each
		''' time a mouse button is pressed, one would write:
		''' <blockquote>
		''' <pre>
		''' EventHandler.create(MouseListener.class, target, "origin", "point", "mousePressed");
		''' </pre>
		''' </blockquote>
		''' 
		''' This is comparable to writing a <code>MouseListener</code> in which all
		''' of the methods except <code>mousePressed</code> are no-ops:
		''' 
		''' <blockquote>
		''' <pre>
		''' //Equivalent code using an inner class instead of EventHandler.
		''' new MouseAdapter() {
		'''    public  Sub  mousePressed(MouseEvent e) {
		'''        target.setOrigin(e.getPoint());
		'''    }
		''' };
		''' </pre>
		''' </blockquote>
		''' </summary>
		''' @param <T> the type to create </param>
		''' <param name="listenerInterface"> the listener interface to create a proxy for </param>
		''' <param name="target"> the object that will perform the action </param>
		''' <param name="action"> the name of a (possibly qualified) property or method on
		'''        the target </param>
		''' <param name="eventPropertyName"> the (possibly qualified) name of a readable property of the incoming event </param>
		''' <param name="listenerMethodName"> the name of the method in the listener interface that should trigger the action
		''' </param>
		''' <returns> an object that implements <code>listenerInterface</code>
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>listenerInterface</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		''' <exception cref="NullPointerException"> if <code>action</code> is null
		''' </exception>
		''' <seealso cref= EventHandler </seealso>
		Public Shared Function create(Of T)(  listenerInterface As [Class],   target As Object,   action As String,   eventPropertyName As String,   listenerMethodName As String) As T
			' Create this first to verify target/action are non-null
			Dim handler As New EventHandler(target, action, eventPropertyName, listenerMethodName)
			If listenerInterface Is Nothing Then Throw New NullPointerException("listenerInterface must be non-null")
			Dim loader As  ClassLoader = getClassLoader(listenerInterface)
			Dim interfaces As  [Class]() = {listenerInterface}
			Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function run() As T
				Return CType(Proxy.newProxyInstance(loader, interfaces, handler), T)
			End Function
		End Class

		Private Shared Function getClassLoader(  type As [Class]) As  ClassLoader
			sun.reflect.misc.ReflectUtil.checkPackageAccess(type)
			Dim loader As  ClassLoader = type.classLoader
			If loader Is Nothing Then
				loader = Thread.CurrentThread.contextClassLoader ' avoid use of BCP
				If loader Is Nothing Then loader = ClassLoader.systemClassLoader
			End If
			Return loader
		End Function
	End Class

End Namespace
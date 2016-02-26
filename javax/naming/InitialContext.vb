Imports System.Collections
Imports System.Collections.Generic

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

Namespace javax.naming


	''' <summary>
	''' This class is the starting context for performing naming operations.
	''' <p>
	''' All naming operations are relative to a context.
	''' The initial context implements the Context interface and
	''' provides the starting point for resolution of names.
	''' <p>
	''' <a name=ENVIRONMENT></a>
	''' When the initial context is constructed, its environment
	''' is initialized with properties defined in the environment parameter
	''' passed to the constructor, and in any
	''' <a href=Context.html#RESOURCEFILES>application resource files</a>.
	''' In addition, a small number of standard JNDI properties may
	''' be specified as system properties or as applet parameters
	''' (through the use of <seealso cref="Context#APPLET"/>).
	''' These special properties are listed in the field detail sections of the
	''' <a href=Context.html#field_detail><tt>Context</tt></a> and
	''' <a href=ldap/LdapContext.html#field_detail><tt>LdapContext</tt></a>
	''' interface documentation.
	''' <p>
	''' JNDI determines each property's value by merging
	''' the values from the following two sources, in order:
	''' <ol>
	''' <li>
	''' The first occurrence of the property from the constructor's
	''' environment parameter and (for appropriate properties) the applet
	''' parameters and system properties.
	''' <li>
	''' The application resource files (<tt>jndi.properties</tt>).
	''' </ol>
	''' For each property found in both of these two sources, or in
	''' more than one application resource file, the property's value
	''' is determined as follows.  If the property is
	''' one of the standard JNDI properties that specify a list of JNDI
	''' factories (see <a href=Context.html#LISTPROPS><tt>Context</tt></a>),
	''' all of the values are
	''' concatenated into a single colon-separated list.  For other
	''' properties, only the first value found is used.
	''' 
	''' <p>
	''' The initial context implementation is determined at runtime.
	''' The default policy uses the environment property
	''' "{@link Context#INITIAL_CONTEXT_FACTORY java.naming.factory.initial}",
	''' which contains the class name of the initial context factory.
	''' An exception to this policy is made when resolving URL strings, as described
	''' below.
	''' <p>
	''' When a URL string (a <tt>String</tt> of the form
	''' <em>scheme_id:rest_of_name</em>) is passed as a name parameter to
	''' any method, a URL context factory for handling that scheme is
	''' located and used to resolve the URL.  If no such factory is found,
	''' the initial context specified by
	''' <tt>"java.naming.factory.initial"</tt> is used.  Similarly, when a
	''' <tt>CompositeName</tt> object whose first component is a URL string is
	''' passed as a name parameter to any method, a URL context factory is
	''' located and used to resolve the first name component.
	''' See {@link NamingManager#getURLContext
	''' <tt>NamingManager.getURLContext()</tt>} for a description of how URL
	''' context factories are located.
	''' <p>
	''' This default policy of locating the initial context and URL context
	''' factories may be overridden
	''' by calling
	''' <tt>NamingManager.setInitialContextFactoryBuilder()</tt>.
	''' <p>
	''' NoInitialContextException is thrown when an initial context cannot
	''' be instantiated. This exception can be thrown during any interaction
	''' with the InitialContext, not only when the InitialContext is constructed.
	''' For example, the implementation of the initial context might lazily
	''' retrieve the context only when actual methods are invoked on it.
	''' The application should not have any dependency on when the existence
	''' of an initial context is determined.
	''' <p>
	''' When the environment property "java.naming.factory.initial" is
	''' non-null, the InitialContext constructor will attempt to create the
	''' initial context specified therein. At that time, the initial context factory
	''' involved might throw an exception if a problem is encountered. However,
	''' it is provider implementation-dependent when it verifies and indicates
	''' to the users of the initial context any environment property- or
	''' connection- related problems. It can do so lazily--delaying until
	''' an operation is performed on the context, or eagerly, at the time
	''' the context is constructed.
	''' <p>
	''' An InitialContext instance is not synchronized against concurrent
	''' access by multiple threads. Multiple threads each manipulating a
	''' different InitialContext instance need not synchronize.
	''' Threads that need to access a single InitialContext instance
	''' concurrently should synchronize amongst themselves and provide the
	''' necessary locking.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context </seealso>
	''' <seealso cref= NamingManager#setInitialContextFactoryBuilder
	'''      NamingManager.setInitialContextFactoryBuilder
	''' @since JNDI 1.1 / Java 2 Platform, Standard Edition, v 1.3 </seealso>

	Public Class InitialContext
		Implements Context

		''' <summary>
		''' The environment associated with this InitialContext.
		''' It is initialized to null and is updated by the constructor
		''' that accepts an environment or by the <tt>init()</tt> method. </summary>
		''' <seealso cref= #addToEnvironment </seealso>
		''' <seealso cref= #removeFromEnvironment </seealso>
		''' <seealso cref= #getEnvironment </seealso>
		Protected Friend myProps As Dictionary(Of Object, Object) = Nothing

		''' <summary>
		''' Field holding the result of calling NamingManager.getInitialContext().
		''' It is set by getDefaultInitCtx() the first time getDefaultInitCtx()
		''' is called. Subsequent invocations of getDefaultInitCtx() return
		''' the value of defaultInitCtx. </summary>
		''' <seealso cref= #getDefaultInitCtx </seealso>
		Protected Friend defaultInitCtx As Context = Nothing

		''' <summary>
		''' Field indicating whether the initial context has been obtained
		''' by calling NamingManager.getInitialContext().
		''' If true, its result is in <code>defaultInitCtx</code>.
		''' </summary>
		Protected Friend gotDefault As Boolean = False

		''' <summary>
		''' Constructs an initial context with the option of not
		''' initializing it.  This may be used by a constructor in
		''' a subclass when the value of the environment parameter
		''' is not yet known at the time the <tt>InitialContext</tt>
		''' constructor is called.  The subclass's constructor will
		''' call this constructor, compute the value of the environment,
		''' and then call <tt>init()</tt> before returning.
		''' </summary>
		''' <param name="lazy">
		'''          true means do not initialize the initial context; false
		'''          is equivalent to calling <tt>new InitialContext()</tt> </param>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= #init(Hashtable)
		''' @since 1.3 </seealso>
		Protected Friend Sub New(ByVal lazy As Boolean)
			If Not lazy Then init(Nothing)
		End Sub

		''' <summary>
		''' Constructs an initial context.
		''' No environment properties are supplied.
		''' Equivalent to <tt>new InitialContext(null)</tt>.
		''' </summary>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= #InitialContext(Hashtable) </seealso>
		Public Sub New()
			init(Nothing)
		End Sub

		''' <summary>
		''' Constructs an initial context using the supplied environment.
		''' Environment properties are discussed in the class description.
		''' 
		''' <p> This constructor will not modify <tt>environment</tt>
		''' or save a reference to it, but may save a clone.
		''' Caller should not modify mutable keys and values in
		''' <tt>environment</tt> after it has been passed to the constructor.
		''' </summary>
		''' <param name="environment">
		'''          environment used to create the initial context.
		'''          Null indicates an empty environment.
		''' </param>
		''' <exception cref="NamingException"> if a naming exception is encountered </exception>
		Public Sub New(Of T1)(ByVal environment As Dictionary(Of T1))
			If environment IsNot Nothing Then environment = CType(environment.clone(), Hashtable)
			init(environment)
		End Sub

		''' <summary>
		''' Initializes the initial context using the supplied environment.
		''' Environment properties are discussed in the class description.
		''' 
		''' <p> This method will modify <tt>environment</tt> and save
		''' a reference to it.  The caller may no longer modify it.
		''' </summary>
		''' <param name="environment">
		'''          environment used to create the initial context.
		'''          Null indicates an empty environment.
		''' </param>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= #InitialContext(boolean)
		''' @since 1.3 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Overridable Sub init(Of T1)(ByVal environment As Dictionary(Of T1))
			myProps = CType(com.sun.naming.internal.ResourceManager.getInitialEnvironment(environment), Dictionary(Of Object, Object))

			If myProps(Context.INITIAL_CONTEXT_FACTORY) IsNot Nothing Then defaultInitCtx
		End Sub

		''' <summary>
		''' A static method to retrieve the named object.
		''' This is a shortcut method equivalent to invoking:
		''' <p>
		''' <code>
		'''        InitialContext ic = new InitialContext();
		'''        Object obj = ic.lookup();
		''' </code>
		''' <p> If <tt>name</tt> is empty, returns a new instance of this context
		''' (which represents the same naming context as this context, but its
		''' environment may be modified independently and it may be accessed
		''' concurrently).
		''' </summary>
		''' @param <T> the type of the returned object </param>
		''' <param name="name">
		'''          the name of the object to look up </param>
		''' <returns>  the object bound to <tt>name</tt> </returns>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= #doLookup(String) </seealso>
		''' <seealso cref= #lookup(Name)
		''' @since 1.6 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function doLookup(Of T)(ByVal name As Name) As T
			Return CType((New InitialContext).lookup(name), T)
		End Function

	   ''' <summary>
	   ''' A static method to retrieve the named object.
	   ''' See <seealso cref="#doLookup(Name)"/> for details. </summary>
	   ''' @param <T> the type of the returned object </param>
	   ''' <param name="name">
	   '''          the name of the object to look up </param>
	   ''' <returns>  the object bound to <tt>name</tt> </returns>
	   ''' <exception cref="NamingException"> if a naming exception is encountered
	   ''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function doLookup(Of T)(ByVal name As String) As T
			Return CType((New InitialContext).lookup(name), T)
		End Function

		Private Shared Function getURLScheme(ByVal str As String) As String
			Dim colon_posn As Integer = str.IndexOf(":"c)
			Dim slash_posn As Integer = str.IndexOf("/"c)

			If colon_posn > 0 AndAlso (slash_posn = -1 OrElse colon_posn < slash_posn) Then Return str.Substring(0, colon_posn)
			Return Nothing
		End Function

		''' <summary>
		''' Retrieves the initial context by calling
		''' <code>NamingManager.getInitialContext()</code>
		''' and cache it in defaultInitCtx.
		''' Set <code>gotDefault</code> so that we know we've tried this before. </summary>
		''' <returns> The non-null cached initial context. </returns>
		''' <exception cref="NoInitialContextException"> If cannot find an initial context. </exception>
		''' <exception cref="NamingException"> If a naming exception was encountered. </exception>
		Protected Friend Overridable Property defaultInitCtx As Context
			Get
				If Not gotDefault Then
					defaultInitCtx = javax.naming.spi.NamingManager.getInitialContext(myProps)
					gotDefault = True
				End If
				If defaultInitCtx Is Nothing Then Throw New NoInitialContextException
    
				Return defaultInitCtx
			End Get
		End Property

		''' <summary>
		''' Retrieves a context for resolving the string name <code>name</code>.
		''' If <code>name</code> name is a URL string, then attempt
		''' to find a URL context for it. If none is found, or if
		''' <code>name</code> is not a URL string, then return
		''' <code>getDefaultInitCtx()</code>.
		''' <p>
		''' See getURLOrDefaultInitCtx(Name) for description
		''' of how a subclass should use this method. </summary>
		''' <param name="name"> The non-null name for which to get the context. </param>
		''' <returns> A URL context for <code>name</code> or the cached
		'''         initial context. The result cannot be null. </returns>
		''' <exception cref="NoInitialContextException"> If cannot find an initial context. </exception>
		''' <exception cref="NamingException"> In a naming exception is encountered. </exception>
		''' <seealso cref= javax.naming.spi.NamingManager#getURLContext </seealso>
		Protected Friend Overridable Function getURLOrDefaultInitCtx(ByVal name As String) As Context
			If javax.naming.spi.NamingManager.hasInitialContextFactoryBuilder() Then Return defaultInitCtx
			Dim scheme As String = getURLScheme(name)
			If scheme IsNot Nothing Then
				Dim ctx As Context = javax.naming.spi.NamingManager.getURLContext(scheme, myProps)
				If ctx IsNot Nothing Then Return ctx
			End If
			Return defaultInitCtx
		End Function

		''' <summary>
		''' Retrieves a context for resolving <code>name</code>.
		''' If the first component of <code>name</code> name is a URL string,
		''' then attempt to find a URL context for it. If none is found, or if
		''' the first component of <code>name</code> is not a URL string,
		''' then return <code>getDefaultInitCtx()</code>.
		''' <p>
		''' When creating a subclass of InitialContext, use this method as
		''' follows.
		''' Define a new method that uses this method to get an initial
		''' context of the desired subclass.
		''' <blockquote><pre>
		''' protected XXXContext getURLOrDefaultInitXXXCtx(Name name)
		''' throws NamingException {
		'''  Context answer = getURLOrDefaultInitCtx(name);
		'''  if (!(answer instanceof XXXContext)) {
		'''    if (answer == null) {
		'''      throw new NoInitialContextException();
		'''    } else {
		'''      throw new NotContextException("Not an XXXContext");
		'''    }
		'''  }
		'''  return (XXXContext)answer;
		''' }
		''' </pre></blockquote>
		''' When providing implementations for the new methods in the subclass,
		''' use this newly defined method to get the initial context.
		''' <blockquote><pre>
		''' public Object XXXMethod1(Name name, ...) {
		'''  throws NamingException {
		'''    return getURLOrDefaultInitXXXCtx(name).XXXMethod1(name, ...);
		''' }
		''' </pre></blockquote>
		''' </summary>
		''' <param name="name"> The non-null name for which to get the context. </param>
		''' <returns> A URL context for <code>name</code> or the cached
		'''         initial context. The result cannot be null. </returns>
		''' <exception cref="NoInitialContextException"> If cannot find an initial context. </exception>
		''' <exception cref="NamingException"> In a naming exception is encountered.
		''' </exception>
		''' <seealso cref= javax.naming.spi.NamingManager#getURLContext </seealso>
		Protected Friend Overridable Function getURLOrDefaultInitCtx(ByVal name As Name) As Context
			If javax.naming.spi.NamingManager.hasInitialContextFactoryBuilder() Then Return defaultInitCtx
			If name.size() > 0 Then
				Dim first As String = name.get(0)
				Dim scheme As String = getURLScheme(first)
				If scheme IsNot Nothing Then
					Dim ctx As Context = javax.naming.spi.NamingManager.getURLContext(scheme, myProps)
					If ctx IsNot Nothing Then Return ctx
				End If
			End If
			Return defaultInitCtx
		End Function

	' Context methods
	' Most Javadoc is deferred to the Context interface.

		Public Overridable Function lookup(ByVal name As String) As Object Implements Context.lookup
			Return getURLOrDefaultInitCtx(name).lookup(name)
		End Function

		Public Overridable Function lookup(ByVal name As Name) As Object Implements Context.lookup
			Return getURLOrDefaultInitCtx(name).lookup(name)
		End Function

		Public Overridable Sub bind(ByVal name As String, ByVal obj As Object) Implements Context.bind
			getURLOrDefaultInitCtx(name).bind(name, obj)
		End Sub

		Public Overridable Sub bind(ByVal name As Name, ByVal obj As Object) Implements Context.bind
			getURLOrDefaultInitCtx(name).bind(name, obj)
		End Sub

		Public Overridable Sub rebind(ByVal name As String, ByVal obj As Object) Implements Context.rebind
			getURLOrDefaultInitCtx(name).rebind(name, obj)
		End Sub

		Public Overridable Sub rebind(ByVal name As Name, ByVal obj As Object) Implements Context.rebind
			getURLOrDefaultInitCtx(name).rebind(name, obj)
		End Sub

		Public Overridable Sub unbind(ByVal name As String) Implements Context.unbind
			getURLOrDefaultInitCtx(name).unbind(name)
		End Sub

		Public Overridable Sub unbind(ByVal name As Name) Implements Context.unbind
			getURLOrDefaultInitCtx(name).unbind(name)
		End Sub

		Public Overridable Sub rename(ByVal oldName As String, ByVal newName As String) Implements Context.rename
			getURLOrDefaultInitCtx(oldName).rename(oldName, newName)
		End Sub

		Public Overridable Sub rename(ByVal oldName As Name, ByVal newName As Name) Implements Context.rename
			getURLOrDefaultInitCtx(oldName).rename(oldName, newName)
		End Sub

		Public Overridable Function list(ByVal name As String) As NamingEnumeration(Of NameClassPair) Implements Context.list
			Return (getURLOrDefaultInitCtx(name).list(name))
		End Function

		Public Overridable Function list(ByVal name As Name) As NamingEnumeration(Of NameClassPair) Implements Context.list
			Return (getURLOrDefaultInitCtx(name).list(name))
		End Function

		Public Overridable Function listBindings(ByVal name As String) As NamingEnumeration(Of Binding) Implements Context.listBindings
			Return getURLOrDefaultInitCtx(name).listBindings(name)
		End Function

		Public Overridable Function listBindings(ByVal name As Name) As NamingEnumeration(Of Binding) Implements Context.listBindings
			Return getURLOrDefaultInitCtx(name).listBindings(name)
		End Function

		Public Overridable Sub destroySubcontext(ByVal name As String) Implements Context.destroySubcontext
			getURLOrDefaultInitCtx(name).destroySubcontext(name)
		End Sub

		Public Overridable Sub destroySubcontext(ByVal name As Name) Implements Context.destroySubcontext
			getURLOrDefaultInitCtx(name).destroySubcontext(name)
		End Sub

		Public Overridable Function createSubcontext(ByVal name As String) As Context Implements Context.createSubcontext
			Return getURLOrDefaultInitCtx(name).createSubcontext(name)
		End Function

		Public Overridable Function createSubcontext(ByVal name As Name) As Context Implements Context.createSubcontext
			Return getURLOrDefaultInitCtx(name).createSubcontext(name)
		End Function

		Public Overridable Function lookupLink(ByVal name As String) As Object Implements Context.lookupLink
			Return getURLOrDefaultInitCtx(name).lookupLink(name)
		End Function

		Public Overridable Function lookupLink(ByVal name As Name) As Object Implements Context.lookupLink
			Return getURLOrDefaultInitCtx(name).lookupLink(name)
		End Function

		Public Overridable Function getNameParser(ByVal name As String) As NameParser Implements Context.getNameParser
			Return getURLOrDefaultInitCtx(name).getNameParser(name)
		End Function

		Public Overridable Function getNameParser(ByVal name As Name) As NameParser Implements Context.getNameParser
			Return getURLOrDefaultInitCtx(name).getNameParser(name)
		End Function

		''' <summary>
		''' Composes the name of this context with a name relative to
		''' this context.
		''' Since an initial context may never be named relative
		''' to any context other than itself, the value of the
		''' <tt>prefix</tt> parameter must be an empty name (<tt>""</tt>).
		''' </summary>
		Public Overridable Function composeName(ByVal name As String, ByVal prefix As String) As String Implements Context.composeName
			Return name
		End Function

		''' <summary>
		''' Composes the name of this context with a name relative to
		''' this context.
		''' Since an initial context may never be named relative
		''' to any context other than itself, the value of the
		''' <tt>prefix</tt> parameter must be an empty name.
		''' </summary>
		Public Overridable Function composeName(ByVal name As Name, ByVal prefix As Name) As Name Implements Context.composeName
			Return CType(name.clone(), Name)
		End Function

		Public Overridable Function addToEnvironment(ByVal propName As String, ByVal propVal As Object) As Object Implements Context.addToEnvironment
			myProps(propName) = propVal
			Return defaultInitCtx.addToEnvironment(propName, propVal)
		End Function

		Public Overridable Function removeFromEnvironment(ByVal propName As String) As Object Implements Context.removeFromEnvironment
			myProps.Remove(propName)
			Return defaultInitCtx.removeFromEnvironment(propName)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property environment As Dictionary(Of ?, ?) Implements Context.getEnvironment
			Get
				Return defaultInitCtx.environment
			End Get
		End Property

		Public Overridable Sub close() Implements Context.close
			myProps = Nothing
			If defaultInitCtx IsNot Nothing Then
				defaultInitCtx.close()
				defaultInitCtx = Nothing
			End If
			gotDefault = False
		End Sub

		Public Overridable Property nameInNamespace As String Implements Context.getNameInNamespace
			Get
				Return defaultInitCtx.nameInNamespace
			End Get
		End Property
	End Class

End Namespace
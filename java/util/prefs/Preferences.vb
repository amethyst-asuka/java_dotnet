Imports System
Imports System.Collections.Generic
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

Namespace java.util.prefs


	' These imports needed only as a workaround for a JavaDoc bug

	''' <summary>
	''' A node in a hierarchical collection of preference data.  This class
	''' allows applications to store and retrieve user and system
	''' preference and configuration data.  This data is stored
	''' persistently in an implementation-dependent backing store.  Typical
	''' implementations include flat files, OS-specific registries,
	''' directory servers and SQL databases.  The user of this class needn't
	''' be concerned with details of the backing store.
	''' 
	''' <p>There are two separate trees of preference nodes, one for user
	''' preferences and one for system preferences.  Each user has a separate user
	''' preference tree, and all users in a given system share the same system
	''' preference tree.  The precise description of "user" and "system" will vary
	''' from implementation to implementation.  Typical information stored in the
	''' user preference tree might include font choice, color choice, or preferred
	''' window location and size for a particular application.  Typical information
	''' stored in the system preference tree might include installation
	''' configuration data for an application.
	''' 
	''' <p>Nodes in a preference tree are named in a similar fashion to
	''' directories in a hierarchical file system.   Every node in a preference
	''' tree has a <i>node name</i> (which is not necessarily unique),
	''' a unique <i>absolute path name</i>, and a path name <i>relative</i> to each
	''' ancestor including itself.
	''' 
	''' <p>The root node has a node name of the empty string ("").  Every other
	''' node has an arbitrary node name, specified at the time it is created.  The
	''' only restrictions on this name are that it cannot be the empty string, and
	''' it cannot contain the slash character ('/').
	''' 
	''' <p>The root node has an absolute path name of <tt>"/"</tt>.  Children of
	''' the root node have absolute path names of <tt>"/" + </tt><i>&lt;node
	''' name&gt;</i>.  All other nodes have absolute path names of <i>&lt;parent's
	''' absolute path name&gt;</i><tt> + "/" + </tt><i>&lt;node name&gt;</i>.
	''' Note that all absolute path names begin with the slash character.
	''' 
	''' <p>A node <i>n</i>'s path name relative to its ancestor <i>a</i>
	''' is simply the string that must be appended to <i>a</i>'s absolute path name
	''' in order to form <i>n</i>'s absolute path name, with the initial slash
	''' character (if present) removed.  Note that:
	''' <ul>
	''' <li>No relative path names begin with the slash character.
	''' <li>Every node's path name relative to itself is the empty string.
	''' <li>Every node's path name relative to its parent is its node name (except
	''' for the root node, which does not have a parent).
	''' <li>Every node's path name relative to the root is its absolute path name
	''' with the initial slash character removed.
	''' </ul>
	''' 
	''' <p>Note finally that:
	''' <ul>
	''' <li>No path name contains multiple consecutive slash characters.
	''' <li>No path name with the exception of the root's absolute path name
	''' ends in the slash character.
	''' <li>Any string that conforms to these two rules is a valid path name.
	''' </ul>
	''' 
	''' <p>All of the methods that modify preferences data are permitted to operate
	''' asynchronously; they may return immediately, and changes will eventually
	''' propagate to the persistent backing store with an implementation-dependent
	''' delay.  The <tt>flush</tt> method may be used to synchronously force
	''' updates to the backing store.  Normal termination of the Java Virtual
	''' Machine will <i>not</i> result in the loss of pending updates -- an explicit
	''' <tt>flush</tt> invocation is <i>not</i> required upon termination to ensure
	''' that pending updates are made persistent.
	''' 
	''' <p>All of the methods that read preferences from a <tt>Preferences</tt>
	''' object require the invoker to provide a default value.  The default value is
	''' returned if no value has been previously set <i>or if the backing store is
	''' unavailable</i>.  The intent is to allow applications to operate, albeit
	''' with slightly degraded functionality, even if the backing store becomes
	''' unavailable.  Several methods, like <tt>flush</tt>, have semantics that
	''' prevent them from operating if the backing store is unavailable.  Ordinary
	''' applications should have no need to invoke any of these methods, which can
	''' be identified by the fact that they are declared to throw {@link
	''' BackingStoreException}.
	''' 
	''' <p>The methods in this class may be invoked concurrently by multiple threads
	''' in a single JVM without the need for external synchronization, and the
	''' results will be equivalent to some serial execution.  If this class is used
	''' concurrently <i>by multiple JVMs</i> that store their preference data in
	''' the same backing store, the data store will not be corrupted, but no
	''' other guarantees are made concerning the consistency of the preference
	''' data.
	''' 
	''' <p>This class contains an export/import facility, allowing preferences
	''' to be "exported" to an XML document, and XML documents representing
	''' preferences to be "imported" back into the system.  This facility
	''' may be used to back up all or part of a preference tree, and
	''' subsequently restore from the backup.
	''' 
	''' <p>The XML document has the following DOCTYPE declaration:
	''' <pre>{@code
	''' <!DOCTYPE preferences SYSTEM "http://java.sun.com/dtd/preferences.dtd">
	''' }</pre>
	''' Note that the system URI (http://java.sun.com/dtd/preferences.dtd) is
	''' <i>not</i> accessed when exporting or importing preferences; it merely
	''' serves as a string to uniquely identify the DTD, which is:
	''' <pre>{@code
	'''    <?xml version="1.0" encoding="UTF-8"?>
	''' 
	'''    <!-- DTD for a Preferences tree. -->
	''' 
	'''    <!-- The preferences element is at the root of an XML document
	'''         representing a Preferences tree. -->
	'''    <!ELEMENT preferences (root)>
	''' 
	'''    <!-- The preferences element contains an optional version attribute,
	'''          which specifies version of DTD. -->
	'''    <!ATTLIST preferences EXTERNAL_XML_VERSION CDATA "0.0" >
	''' 
	'''    <!-- The root element has a map representing the root's preferences
	'''         (if any), and one node for each child of the root (if any). -->
	'''    <!ELEMENT root (map, node*) >
	''' 
	'''    <!-- Additionally, the root contains a type attribute, which
	'''         specifies whether it's the system or user root. -->
	'''    <!ATTLIST root
	'''              type (system|user) #REQUIRED >
	''' 
	'''    <!-- Each node has a map representing its preferences (if any),
	'''         and one node for each child (if any). -->
	'''    <!ELEMENT node (map, node*) >
	''' 
	'''    <!-- Additionally, each node has a name attribute -->
	'''    <!ATTLIST node
	'''              name CDATA #REQUIRED >
	''' 
	'''    <!-- A map represents the preferences stored at a node (if any). -->
	'''    <!ELEMENT map (entry*) >
	''' 
	'''    <!-- An entry represents a single preference, which is simply
	'''          a key-value pair. -->
	'''    <!ELEMENT entry EMPTY >
	'''    <!ATTLIST entry
	'''              key   CDATA #REQUIRED
	'''              value CDATA #REQUIRED >
	''' }</pre>
	''' 
	''' Every <tt>Preferences</tt> implementation must have an associated {@link
	''' PreferencesFactory} implementation.  Every Java(TM) SE implementation must provide
	''' some means of specifying which <tt>PreferencesFactory</tt> implementation
	''' is used to generate the root preferences nodes.  This allows the
	''' administrator to replace the default preferences implementation with an
	''' alternative implementation.
	''' 
	''' <p>Implementation note: In Sun's JRE, the <tt>PreferencesFactory</tt>
	''' implementation is located as follows:
	''' 
	''' <ol>
	''' 
	''' <li><p>If the system property
	''' <tt>java.util.prefs.PreferencesFactory</tt> is defined, then it is
	''' taken to be the fully-qualified name of a class implementing the
	''' <tt>PreferencesFactory</tt> interface.  The class is loaded and
	''' instantiated; if this process fails then an unspecified error is
	''' thrown.</p></li>
	''' 
	''' <li><p> If a <tt>PreferencesFactory</tt> implementation class file
	''' has been installed in a jar file that is visible to the
	''' <seealso cref="java.lang.ClassLoader#getSystemClassLoader system class loader"/>,
	''' and that jar file contains a provider-configuration file named
	''' <tt>java.util.prefs.PreferencesFactory</tt> in the resource
	''' directory <tt>META-INF/services</tt>, then the first class name
	''' specified in that file is taken.  If more than one such jar file is
	''' provided, the first one found will be used.  The class is loaded
	''' and instantiated; if this process fails then an unspecified error
	''' is thrown.  </p></li>
	''' 
	''' <li><p>Finally, if neither the above-mentioned system property nor
	''' an extension jar file is provided, then the system-wide default
	''' <tt>PreferencesFactory</tt> implementation for the underlying
	''' platform is loaded and instantiated.</p></li>
	''' 
	''' </ol>
	''' 
	''' @author  Josh Bloch
	''' @since   1.4
	''' </summary>
	Public MustInherit Class Preferences

		Private Shared ReadOnly factory_Renamed As PreferencesFactory = factory()

		Private Shared Function factory() As PreferencesFactory
			' 1. Try user-specified system property
			Dim factoryName As String = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			If factoryName IsNot Nothing Then
				' FIXME: This code should be run in a doPrivileged and
				' not use the context classloader, to avoid being
				' dependent on the invoking thread.
				' Checking AllPermission also seems wrong.
				Try
					Return CType(Type.GetType(factoryName, False, ClassLoader.systemClassLoader).newInstance(), PreferencesFactory)
				Catch ex As Exception
					Try
						' workaround for javaws, plugin,
						' load factory class using non-system classloader
						Dim sm As SecurityManager = System.securityManager
						If sm IsNot Nothing Then sm.checkPermission(New java.security.AllPermission)
						Return CType(Type.GetType(factoryName, False, Thread.CurrentThread.contextClassLoader).newInstance(), PreferencesFactory)
					Catch e As Exception
						Throw New InternalError("Can't instantiate Preferences factory " & factoryName, e)
					End Try
				End Try
			End If

			Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return System.getProperty("java.util.prefs.PreferencesFactory")
			End Function
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As PreferencesFactory
				Return factory1()
			End Function
		End Class

		Private Shared Function factory1() As PreferencesFactory
			' 2. Try service provider interface
			Dim itr As IEnumerator(Of PreferencesFactory) = java.util.ServiceLoader.load(GetType(PreferencesFactory), ClassLoader.systemClassLoader).GetEnumerator()

			' choose first provider instance
			Do While itr.MoveNext()
				Try
					Return itr.Current
				Catch sce As java.util.ServiceConfigurationError
					If TypeOf sce.cause Is SecurityException Then Continue Do
					Throw sce
				End Try
			Loop

			' 3. Use platform-specific system-wide default
			Dim osName As String = System.getProperty("os.name")
			Dim platformFactory As String
			If osName.StartsWith("Windows") Then
				platformFactory = "java.util.prefs.WindowsPreferencesFactory"
			ElseIf osName.contains("OS X") Then
				platformFactory = "java.util.prefs.MacOSXPreferencesFactory"
			Else
				platformFactory = "java.util.prefs.FileSystemPreferencesFactory"
			End If
			Try
				Return CType(Type.GetType(platformFactory, False, GetType(Preferences).classLoader).newInstance(), PreferencesFactory)
			Catch e As Exception
				Throw New InternalError("Can't instantiate platform default Preferences factory " & platformFactory, e)
			End Try
		End Function

		''' <summary>
		''' Maximum length of string allowed as a key (80 characters).
		''' </summary>
		Public Const MAX_KEY_LENGTH As Integer = 80

		''' <summary>
		''' Maximum length of string allowed as a value (8192 characters).
		''' </summary>
		Public Const MAX_VALUE_LENGTH As Integer = 8*1024

		''' <summary>
		''' Maximum length of a node name (80 characters).
		''' </summary>
		Public Const MAX_NAME_LENGTH As Integer = 80

		''' <summary>
		''' Returns the preference node from the calling user's preference tree
		''' that is associated (by convention) with the specified class's package.
		''' The convention is as follows: the absolute path name of the node is the
		''' fully qualified package name, preceded by a slash (<tt>'/'</tt>), and
		''' with each period (<tt>'.'</tt>) replaced by a slash.  For example the
		''' absolute path name of the node associated with the class
		''' <tt>com.acme.widget.Foo</tt> is <tt>/com/acme/widget</tt>.
		''' 
		''' <p>This convention does not apply to the unnamed package, whose
		''' associated preference node is <tt>&lt;unnamed&gt;</tt>.  This node
		''' is not intended for long term use, but for convenience in the early
		''' development of programs that do not yet belong to a package, and
		''' for "throwaway" programs.  <i>Valuable data should not be stored
		''' at this node as it is shared by all programs that use it.</i>
		''' 
		''' <p>A class <tt>Foo</tt> wishing to access preferences pertaining to its
		''' package can obtain a preference node as follows: <pre>
		'''    static Preferences prefs = Preferences.userNodeForPackage(Foo.class);
		''' </pre>
		''' This idiom obviates the need for using a string to describe the
		''' preferences node and decreases the likelihood of a run-time failure.
		''' (If the class name is misspelled, it will typically result in a
		''' compile-time error.)
		''' 
		''' <p>Invoking this method will result in the creation of the returned
		''' node and its ancestors if they do not already exist.  If the returned
		''' node did not exist prior to this call, this node and any ancestors that
		''' were created by this call are not guaranteed to become permanent until
		''' the <tt>flush</tt> method is called on the returned node (or one of its
		''' ancestors or descendants).
		''' </summary>
		''' <param name="c"> the class for whose package a user preference node is desired. </param>
		''' <returns> the user preference node associated with the package of which
		'''         <tt>c</tt> is a member. </returns>
		''' <exception cref="NullPointerException"> if <tt>c</tt> is <tt>null</tt>. </exception>
		''' <exception cref="SecurityException"> if a security manager is present and
		'''         it denies <tt>RuntimePermission("preferences")</tt>. </exception>
		''' <seealso cref=    RuntimePermission </seealso>
		Public Shared Function userNodeForPackage(ByVal c As [Class]) As Preferences
			Return userRoot().node(nodeName(c))
		End Function

		''' <summary>
		''' Returns the preference node from the system preference tree that is
		''' associated (by convention) with the specified class's package.  The
		''' convention is as follows: the absolute path name of the node is the
		''' fully qualified package name, preceded by a slash (<tt>'/'</tt>), and
		''' with each period (<tt>'.'</tt>) replaced by a slash.  For example the
		''' absolute path name of the node associated with the class
		''' <tt>com.acme.widget.Foo</tt> is <tt>/com/acme/widget</tt>.
		''' 
		''' <p>This convention does not apply to the unnamed package, whose
		''' associated preference node is <tt>&lt;unnamed&gt;</tt>.  This node
		''' is not intended for long term use, but for convenience in the early
		''' development of programs that do not yet belong to a package, and
		''' for "throwaway" programs.  <i>Valuable data should not be stored
		''' at this node as it is shared by all programs that use it.</i>
		''' 
		''' <p>A class <tt>Foo</tt> wishing to access preferences pertaining to its
		''' package can obtain a preference node as follows: <pre>
		'''  static Preferences prefs = Preferences.systemNodeForPackage(Foo.class);
		''' </pre>
		''' This idiom obviates the need for using a string to describe the
		''' preferences node and decreases the likelihood of a run-time failure.
		''' (If the class name is misspelled, it will typically result in a
		''' compile-time error.)
		''' 
		''' <p>Invoking this method will result in the creation of the returned
		''' node and its ancestors if they do not already exist.  If the returned
		''' node did not exist prior to this call, this node and any ancestors that
		''' were created by this call are not guaranteed to become permanent until
		''' the <tt>flush</tt> method is called on the returned node (or one of its
		''' ancestors or descendants).
		''' </summary>
		''' <param name="c"> the class for whose package a system preference node is desired. </param>
		''' <returns> the system preference node associated with the package of which
		'''         <tt>c</tt> is a member. </returns>
		''' <exception cref="NullPointerException"> if <tt>c</tt> is <tt>null</tt>. </exception>
		''' <exception cref="SecurityException"> if a security manager is present and
		'''         it denies <tt>RuntimePermission("preferences")</tt>. </exception>
		''' <seealso cref=    RuntimePermission </seealso>
		Public Shared Function systemNodeForPackage(ByVal c As [Class]) As Preferences
			Return systemRoot().node(nodeName(c))
		End Function

		''' <summary>
		''' Returns the absolute path name of the node corresponding to the package
		''' of the specified object.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if the package has node preferences
		'''         node associated with it. </exception>
		Private Shared Function nodeName(ByVal c As [Class]) As String
			If c.array Then Throw New IllegalArgumentException("Arrays have no associated preferences node.")
			Dim className As String = c.name
			Dim pkgEndIndex As Integer = className.LastIndexOf("."c)
			If pkgEndIndex < 0 Then Return "/<unnamed>"
			Dim packageName As String = className.Substring(0, pkgEndIndex)
			Return "/" & packageName.replace("."c, "/"c)
		End Function

		''' <summary>
		''' This permission object represents the permission required to get
		''' access to the user or system root (which in turn allows for all
		''' other operations).
		''' </summary>
		Private Shared prefsPerm As java.security.Permission = New RuntimePermission("preferences")

		''' <summary>
		''' Returns the root preference node for the calling user.
		''' </summary>
		''' <returns> the root preference node for the calling user. </returns>
		''' <exception cref="SecurityException"> If a security manager is present and
		'''         it denies <tt>RuntimePermission("preferences")</tt>. </exception>
		''' <seealso cref=    RuntimePermission </seealso>
		Public Shared Function userRoot() As Preferences
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(prefsPerm)

			Return factory_Renamed.userRoot()
		End Function

		''' <summary>
		''' Returns the root preference node for the system.
		''' </summary>
		''' <returns> the root preference node for the system. </returns>
		''' <exception cref="SecurityException"> If a security manager is present and
		'''         it denies <tt>RuntimePermission("preferences")</tt>. </exception>
		''' <seealso cref=    RuntimePermission </seealso>
		Public Shared Function systemRoot() As Preferences
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(prefsPerm)

			Return factory_Renamed.systemRoot()
		End Function

		''' <summary>
		''' Sole constructor. (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Associates the specified value with the specified key in this
		''' preference node.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated. </param>
		''' <param name="value"> value to be associated with the specified key. </param>
		''' <exception cref="NullPointerException"> if key or value is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''       <tt>MAX_KEY_LENGTH</tt> or if <tt>value.length</tt> exceeds
		'''       <tt>MAX_VALUE_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public MustOverride Sub put(ByVal key As String, ByVal value As String)

		''' <summary>
		''' Returns the value associated with the specified key in this preference
		''' node.  Returns the specified default if there is no value associated
		''' with the key, or the backing store is inaccessible.
		''' 
		''' <p>Some implementations may store default values in their backing
		''' stores.  If there is no value associated with the specified key
		''' but there is such a <i>stored default</i>, it is returned in
		''' preference to the specified default.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>. </param>
		''' <returns> the value associated with <tt>key</tt>, or <tt>def</tt>
		'''         if no value is associated with <tt>key</tt>, or the backing
		'''         store is inaccessible. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>.  (A
		'''         <tt>null</tt> value for <tt>def</tt> <i>is</i> permitted.) </exception>
		Public MustOverride Function [get](ByVal key As String, ByVal def As String) As String

		''' <summary>
		''' Removes the value associated with the specified key in this preference
		''' node, if any.
		''' 
		''' <p>If this implementation supports <i>stored defaults</i>, and there is
		''' such a default for the specified preference, the stored default will be
		''' "exposed" by this call, in the sense that it will be returned
		''' by a succeeding call to <tt>get</tt>.
		''' </summary>
		''' <param name="key"> key whose mapping is to be removed from the preference node. </param>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public MustOverride Sub remove(ByVal key As String)

		''' <summary>
		''' Removes all of the preferences (key-value associations) in this
		''' preference node.  This call has no effect on any descendants
		''' of this node.
		''' 
		''' <p>If this implementation supports <i>stored defaults</i>, and this
		''' node in the preferences hierarchy contains any such defaults,
		''' the stored defaults will be "exposed" by this call, in the sense that
		''' they will be returned by succeeding calls to <tt>get</tt>.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #removeNode() </seealso>
		Public MustOverride Sub clear()

		''' <summary>
		''' Associates a string representing the specified int value with the
		''' specified key in this preference node.  The associated string is the
		''' one that would be returned if the int value were passed to
		''' <seealso cref="Integer#toString(int)"/>.  This method is intended for use in
		''' conjunction with <seealso cref="#getInt"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #getInt(String,int) </seealso>
		Public MustOverride Sub putInt(ByVal key As String, ByVal value As Integer)

		''' <summary>
		''' Returns the int value represented by the string associated with the
		''' specified key in this preference node.  The string is converted to
		''' an integer as by <seealso cref="Integer#parseInt(String)"/>.  Returns the
		''' specified default if there is no value associated with the key,
		''' the backing store is inaccessible, or if
		''' <tt> java.lang.[Integer].parseInt(String)</tt> would throw a {@link
		''' NumberFormatException} if the associated value were passed.  This
		''' method is intended for use in conjunction with <seealso cref="#putInt"/>.
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and such a
		''' default exists, is accessible, and could be converted to an int
		''' with <tt> java.lang.[Integer].parseInt</tt>, this int is returned in preference to
		''' the specified default.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as an int. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as an int,
		'''        or the backing store is inaccessible. </param>
		''' <returns> the int value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         an int. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <seealso cref= #putInt(String,int) </seealso>
		''' <seealso cref= #get(String,String) </seealso>
		Public MustOverride Function getInt(ByVal key As String, ByVal def As Integer) As Integer

		''' <summary>
		''' Associates a string representing the specified long value with the
		''' specified key in this preference node.  The associated string is the
		''' one that would be returned if the long value were passed to
		''' <seealso cref="Long#toString(long)"/>.  This method is intended for use in
		''' conjunction with <seealso cref="#getLong"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #getLong(String,long) </seealso>
		Public MustOverride Sub putLong(ByVal key As String, ByVal value As Long)

		''' <summary>
		''' Returns the long value represented by the string associated with the
		''' specified key in this preference node.  The string is converted to
		''' a long as by <seealso cref="Long#parseLong(String)"/>.  Returns the
		''' specified default if there is no value associated with the key,
		''' the backing store is inaccessible, or if
		''' <tt>Long.parseLong(String)</tt> would throw a {@link
		''' NumberFormatException} if the associated value were passed.  This
		''' method is intended for use in conjunction with <seealso cref="#putLong"/>.
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and such a
		''' default exists, is accessible, and could be converted to a long
		''' with <tt>Long.parseLong</tt>, this long is returned in preference to
		''' the specified default.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a java.lang.[Long]. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a long,
		'''        or the backing store is inaccessible. </param>
		''' <returns> the long value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a java.lang.[Long]. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <seealso cref= #putLong(String,long) </seealso>
		''' <seealso cref= #get(String,String) </seealso>
		Public MustOverride Function getLong(ByVal key As String, ByVal def As Long) As Long

		''' <summary>
		''' Associates a string representing the specified boolean value with the
		''' specified key in this preference node.  The associated string is
		''' <tt>"true"</tt> if the value is true, and <tt>"false"</tt> if it is
		''' false.  This method is intended for use in conjunction with
		''' <seealso cref="#getBoolean"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #getBoolean(String,boolean) </seealso>
		''' <seealso cref= #get(String,String) </seealso>
		Public MustOverride Sub putBoolean(ByVal key As String, ByVal value As Boolean)

		''' <summary>
		''' Returns the boolean value represented by the string associated with the
		''' specified key in this preference node.  Valid strings
		''' are <tt>"true"</tt>, which represents true, and <tt>"false"</tt>, which
		''' represents false.  Case is ignored, so, for example, <tt>"TRUE"</tt>
		''' and <tt>"False"</tt> are also valid.  This method is intended for use in
		''' conjunction with <seealso cref="#putBoolean"/>.
		''' 
		''' <p>Returns the specified default if there is no value
		''' associated with the key, the backing store is inaccessible, or if the
		''' associated value is something other than <tt>"true"</tt> or
		''' <tt>"false"</tt>, ignoring case.
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and such a
		''' default exists and is accessible, it is used in preference to the
		''' specified default, unless the stored default is something other than
		''' <tt>"true"</tt> or <tt>"false"</tt>, ignoring case, in which case the
		''' specified default is used.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a  java.lang.[Boolean]. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a boolean,
		'''        or the backing store is inaccessible. </param>
		''' <returns> the boolean value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a  java.lang.[Boolean]. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <seealso cref= #get(String,String) </seealso>
		''' <seealso cref= #putBoolean(String,boolean) </seealso>
		Public MustOverride Function getBoolean(ByVal key As String, ByVal def As Boolean) As Boolean

		''' <summary>
		''' Associates a string representing the specified float value with the
		''' specified key in this preference node.  The associated string is the
		''' one that would be returned if the float value were passed to
		''' <seealso cref="Float#toString(float)"/>.  This method is intended for use in
		''' conjunction with <seealso cref="#getFloat"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #getFloat(String,float) </seealso>
		Public MustOverride Sub putFloat(ByVal key As String, ByVal value As Single)

		''' <summary>
		''' Returns the float value represented by the string associated with the
		''' specified key in this preference node.  The string is converted to an
		''' integer as by <seealso cref="Float#parseFloat(String)"/>.  Returns the specified
		''' default if there is no value associated with the key, the backing store
		''' is inaccessible, or if <tt>Float.parseFloat(String)</tt> would throw a
		''' <seealso cref="NumberFormatException"/> if the associated value were passed.
		''' This method is intended for use in conjunction with <seealso cref="#putFloat"/>.
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and such a
		''' default exists, is accessible, and could be converted to a float
		''' with <tt>Float.parseFloat</tt>, this float is returned in preference to
		''' the specified default.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a float. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a float,
		'''        or the backing store is inaccessible. </param>
		''' <returns> the float value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a float. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <seealso cref= #putFloat(String,float) </seealso>
		''' <seealso cref= #get(String,String) </seealso>
		Public MustOverride Function getFloat(ByVal key As String, ByVal def As Single) As Single

		''' <summary>
		''' Associates a string representing the specified double value with the
		''' specified key in this preference node.  The associated string is the
		''' one that would be returned if the double value were passed to
		''' <seealso cref="Double#toString(double)"/>.  This method is intended for use in
		''' conjunction with <seealso cref="#getDouble"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #getDouble(String,double) </seealso>
		Public MustOverride Sub putDouble(ByVal key As String, ByVal value As Double)

		''' <summary>
		''' Returns the double value represented by the string associated with the
		''' specified key in this preference node.  The string is converted to an
		''' integer as by <seealso cref="Double#parseDouble(String)"/>.  Returns the specified
		''' default if there is no value associated with the key, the backing store
		''' is inaccessible, or if <tt>Double.parseDouble(String)</tt> would throw a
		''' <seealso cref="NumberFormatException"/> if the associated value were passed.
		''' This method is intended for use in conjunction with <seealso cref="#putDouble"/>.
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and such a
		''' default exists, is accessible, and could be converted to a double
		''' with <tt>Double.parseDouble</tt>, this double is returned in preference
		''' to the specified default.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a java.lang.[Double]. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a double,
		'''        or the backing store is inaccessible. </param>
		''' <returns> the double value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a java.lang.[Double]. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		''' <seealso cref= #putDouble(String,double) </seealso>
		''' <seealso cref= #get(String,String) </seealso>
		Public MustOverride Function getDouble(ByVal key As String, ByVal def As Double) As Double

		''' <summary>
		''' Associates a string representing the specified byte array with the
		''' specified key in this preference node.  The associated string is
		''' the <i>Base64</i> encoding of the byte array, as defined in <a
		''' href=http://www.ietf.org/rfc/rfc2045.txt>RFC 2045</a>, Section 6.8,
		''' with one minor change: the string will consist solely of characters
		''' from the <i>Base64 Alphabet</i>; it will not contain any newline
		''' characters.  Note that the maximum length of the byte array is limited
		''' to three quarters of <tt>MAX_VALUE_LENGTH</tt> so that the length
		''' of the Base64 encoded String does not exceed <tt>MAX_VALUE_LENGTH</tt>.
		''' This method is intended for use in conjunction with
		''' <seealso cref="#getByteArray"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key or value is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if key.length() exceeds MAX_KEY_LENGTH
		'''         or if value.length exceeds MAX_VALUE_LENGTH*3/4. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #getByteArray(String,byte[]) </seealso>
		''' <seealso cref= #get(String,String) </seealso>
		Public MustOverride Sub putByteArray(ByVal key As String, ByVal value As SByte())

		''' <summary>
		''' Returns the byte array value represented by the string associated with
		''' the specified key in this preference node.  Valid strings are
		''' <i>Base64</i> encoded binary data, as defined in <a
		''' href=http://www.ietf.org/rfc/rfc2045.txt>RFC 2045</a>, Section 6.8,
		''' with one minor change: the string must consist solely of characters
		''' from the <i>Base64 Alphabet</i>; no newline characters or
		''' extraneous characters are permitted.  This method is intended for use
		''' in conjunction with <seealso cref="#putByteArray"/>.
		''' 
		''' <p>Returns the specified default if there is no value
		''' associated with the key, the backing store is inaccessible, or if the
		''' associated value is not a valid Base64 encoded byte array
		''' (as defined above).
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and such a
		''' default exists and is accessible, it is used in preference to the
		''' specified default, unless the stored default is not a valid Base64
		''' encoded byte array (as defined above), in which case the
		''' specified default is used.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a byte array. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a byte array,
		'''        or the backing store is inaccessible. </param>
		''' <returns> the byte array value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a byte array. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>.  (A
		'''         <tt>null</tt> value for <tt>def</tt> <i>is</i> permitted.) </exception>
		''' <seealso cref= #get(String,String) </seealso>
		''' <seealso cref= #putByteArray(String,byte[]) </seealso>
		Public MustOverride Function getByteArray(ByVal key As String, ByVal def As SByte()) As SByte()

		''' <summary>
		''' Returns all of the keys that have an associated value in this
		''' preference node.  (The returned array will be of size zero if
		''' this node has no preferences.)
		''' 
		''' <p>If the implementation supports <i>stored defaults</i> and there
		''' are any such defaults at this node that have not been overridden,
		''' by explicit preferences, the defaults are returned in the array in
		''' addition to any explicit preferences.
		''' </summary>
		''' <returns> an array of the keys that have an associated value in this
		'''         preference node. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public MustOverride Function keys() As String()

		''' <summary>
		''' Returns the names of the children of this preference node, relative to
		''' this node.  (The returned array will be of size zero if this node has
		''' no children.)
		''' </summary>
		''' <returns> the names of the children of this preference node. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public MustOverride Function childrenNames() As String()

		''' <summary>
		''' Returns the parent of this preference node, or <tt>null</tt> if this is
		''' the root.
		''' </summary>
		''' <returns> the parent of this preference node. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public MustOverride Function parent() As Preferences

		''' <summary>
		''' Returns the named preference node in the same tree as this node,
		''' creating it and any of its ancestors if they do not already exist.
		''' Accepts a relative or absolute path name.  Relative path names
		''' (which do not begin with the slash character <tt>('/')</tt>) are
		''' interpreted relative to this preference node.
		''' 
		''' <p>If the returned node did not exist prior to this call, this node and
		''' any ancestors that were created by this call are not guaranteed
		''' to become permanent until the <tt>flush</tt> method is called on
		''' the returned node (or one of its ancestors or descendants).
		''' </summary>
		''' <param name="pathName"> the path name of the preference node to return. </param>
		''' <returns> the specified preference node. </returns>
		''' <exception cref="IllegalArgumentException"> if the path name is invalid (i.e.,
		'''         it contains multiple consecutive slash characters, or ends
		'''         with a slash character and is more than one character long). </exception>
		''' <exception cref="NullPointerException"> if path name is <tt>null</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #flush() </seealso>
		Public MustOverride Function node(ByVal pathName As String) As Preferences

		''' <summary>
		''' Returns true if the named preference node exists in the same tree
		''' as this node.  Relative path names (which do not begin with the slash
		''' character <tt>('/')</tt>) are interpreted relative to this preference
		''' node.
		''' 
		''' <p>If this node (or an ancestor) has already been removed with the
		''' <seealso cref="#removeNode()"/> method, it <i>is</i> legal to invoke this method,
		''' but only with the path name <tt>""</tt>; the invocation will return
		''' <tt>false</tt>.  Thus, the idiom <tt>p.nodeExists("")</tt> may be
		''' used to test whether <tt>p</tt> has been removed.
		''' </summary>
		''' <param name="pathName"> the path name of the node whose existence
		'''        is to be checked. </param>
		''' <returns> true if the specified node exists. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalArgumentException"> if the path name is invalid (i.e.,
		'''         it contains multiple consecutive slash characters, or ends
		'''         with a slash character and is more than one character long). </exception>
		''' <exception cref="NullPointerException"> if path name is <tt>null</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method and
		'''         <tt>pathName</tt> is not the empty string (<tt>""</tt>). </exception>
		Public MustOverride Function nodeExists(ByVal pathName As String) As Boolean

		''' <summary>
		''' Removes this preference node and all of its descendants, invalidating
		''' any preferences contained in the removed nodes.  Once a node has been
		''' removed, attempting any method other than <seealso cref="#name()"/>,
		''' <seealso cref="#absolutePath()"/>, <seealso cref="#isUserNode()"/>, <seealso cref="#flush()"/> or
		''' <seealso cref="#node(String) nodeExists("")"/> on the corresponding
		''' <tt>Preferences</tt> instance will fail with an
		''' <tt>IllegalStateException</tt>.  (The methods defined on <seealso cref="Object"/>
		''' can still be invoked on a node after it has been removed; they will not
		''' throw <tt>IllegalStateException</tt>.)
		''' 
		''' <p>The removal is not guaranteed to be persistent until the
		''' <tt>flush</tt> method is called on this node (or an ancestor).
		''' 
		''' <p>If this implementation supports <i>stored defaults</i>, removing a
		''' node exposes any stored defaults at or below this node.  Thus, a
		''' subsequent call to <tt>nodeExists</tt> on this node's path name may
		''' return <tt>true</tt>, and a subsequent call to <tt>node</tt> on this
		''' path name may return a (different) <tt>Preferences</tt> instance
		''' representing a non-empty collection of preferences and/or children.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has already
		'''         been removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="UnsupportedOperationException"> if this method is invoked on
		'''         the root node. </exception>
		''' <seealso cref= #flush() </seealso>
		Public MustOverride Sub removeNode()

		''' <summary>
		''' Returns this preference node's name, relative to its parent.
		''' </summary>
		''' <returns> this preference node's name, relative to its parent. </returns>
		Public MustOverride Function name() As String

		''' <summary>
		''' Returns this preference node's absolute path name.
		''' </summary>
		''' <returns> this preference node's absolute path name. </returns>
		Public MustOverride Function absolutePath() As String

		''' <summary>
		''' Returns <tt>true</tt> if this preference node is in the user
		''' preference tree, <tt>false</tt> if it's in the system preference tree.
		''' </summary>
		''' <returns> <tt>true</tt> if this preference node is in the user
		'''         preference tree, <tt>false</tt> if it's in the system
		'''         preference tree. </returns>
		Public MustOverride ReadOnly Property userNode As Boolean

		''' <summary>
		''' Returns a string representation of this preferences node,
		''' as if computed by the expression:<tt>(this.isUserNode() ? "User" :
		''' "System") + " Preference Node: " + this.absolutePath()</tt>.
		''' </summary>
		Public MustOverride Function ToString() As String

		''' <summary>
		''' Forces any changes in the contents of this preference node and its
		''' descendants to the persistent store.  Once this method returns
		''' successfully, it is safe to assume that all changes made in the
		''' subtree rooted at this node prior to the method invocation have become
		''' permanent.
		''' 
		''' <p>Implementations are free to flush changes into the persistent store
		''' at any time.  They do not need to wait for this method to be called.
		''' 
		''' <p>When a flush occurs on a newly created node, it is made persistent,
		''' as are any ancestors (and descendants) that have yet to be made
		''' persistent.  Note however that any preference value changes in
		''' ancestors are <i>not</i> guaranteed to be made persistent.
		''' 
		''' <p> If this method is invoked on a node that has been removed with
		''' the <seealso cref="#removeNode()"/> method, flushSpi() is invoked on this node,
		''' but not on others.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <seealso cref=    #sync() </seealso>
		Public MustOverride Sub flush()

		''' <summary>
		''' Ensures that future reads from this preference node and its
		''' descendants reflect any changes that were committed to the persistent
		''' store (from any VM) prior to the <tt>sync</tt> invocation.  As a
		''' side-effect, forces any changes in the contents of this preference node
		''' and its descendants to the persistent store, as if the <tt>flush</tt>
		''' method had been invoked on this node.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref=    #flush() </seealso>
		Public MustOverride Sub sync()

		''' <summary>
		''' Registers the specified listener to receive <i>preference change
		''' events</i> for this preference node.  A preference change event is
		''' generated when a preference is added to this node, removed from this
		''' node, or when the value associated with a preference is changed.
		''' (Preference change events are <i>not</i> generated by the {@link
		''' #removeNode()} method, which generates a <i>node change event</i>.
		''' Preference change events <i>are</i> generated by the <tt>clear</tt>
		''' method.)
		''' 
		''' <p>Events are only guaranteed for changes made within the same JVM
		''' as the registered listener, though some implementations may generate
		''' events for changes made outside this JVM.  Events may be generated
		''' before the changes have been made persistent.  Events are not generated
		''' when preferences are modified in descendants of this node; a caller
		''' desiring such events must register with each descendant.
		''' </summary>
		''' <param name="pcl"> The preference change listener to add. </param>
		''' <exception cref="NullPointerException"> if <tt>pcl</tt> is null. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #removePreferenceChangeListener(PreferenceChangeListener) </seealso>
		''' <seealso cref= #addNodeChangeListener(NodeChangeListener) </seealso>
		Public MustOverride Sub addPreferenceChangeListener(ByVal pcl As PreferenceChangeListener)

		''' <summary>
		''' Removes the specified preference change listener, so it no longer
		''' receives preference change events.
		''' </summary>
		''' <param name="pcl"> The preference change listener to remove. </param>
		''' <exception cref="IllegalArgumentException"> if <tt>pcl</tt> was not a registered
		'''         preference change listener on this node. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #addPreferenceChangeListener(PreferenceChangeListener) </seealso>
		Public MustOverride Sub removePreferenceChangeListener(ByVal pcl As PreferenceChangeListener)

		''' <summary>
		''' Registers the specified listener to receive <i>node change events</i>
		''' for this node.  A node change event is generated when a child node is
		''' added to or removed from this node.  (A single <seealso cref="#removeNode()"/>
		''' invocation results in multiple <i>node change events</i>, one for every
		''' node in the subtree rooted at the removed node.)
		''' 
		''' <p>Events are only guaranteed for changes made within the same JVM
		''' as the registered listener, though some implementations may generate
		''' events for changes made outside this JVM.  Events may be generated
		''' before the changes have become permanent.  Events are not generated
		''' when indirect descendants of this node are added or removed; a
		''' caller desiring such events must register with each descendant.
		''' 
		''' <p>Few guarantees can be made regarding node creation.  Because nodes
		''' are created implicitly upon access, it may not be feasible for an
		''' implementation to determine whether a child node existed in the backing
		''' store prior to access (for example, because the backing store is
		''' unreachable or cached information is out of date).  Under these
		''' circumstances, implementations are neither required to generate node
		''' change events nor prohibited from doing so.
		''' </summary>
		''' <param name="ncl"> The <tt>NodeChangeListener</tt> to add. </param>
		''' <exception cref="NullPointerException"> if <tt>ncl</tt> is null. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #removeNodeChangeListener(NodeChangeListener) </seealso>
		''' <seealso cref= #addPreferenceChangeListener(PreferenceChangeListener) </seealso>
		Public MustOverride Sub addNodeChangeListener(ByVal ncl As NodeChangeListener)

		''' <summary>
		''' Removes the specified <tt>NodeChangeListener</tt>, so it no longer
		''' receives change events.
		''' </summary>
		''' <param name="ncl"> The <tt>NodeChangeListener</tt> to remove. </param>
		''' <exception cref="IllegalArgumentException"> if <tt>ncl</tt> was not a registered
		'''         <tt>NodeChangeListener</tt> on this node. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #addNodeChangeListener(NodeChangeListener) </seealso>
		Public MustOverride Sub removeNodeChangeListener(ByVal ncl As NodeChangeListener)

		''' <summary>
		''' Emits on the specified output stream an XML document representing all
		''' of the preferences contained in this node (but not its descendants).
		''' This XML document is, in effect, an offline backup of the node.
		''' 
		''' <p>The XML document will have the following DOCTYPE declaration:
		''' <pre>{@code
		''' <!DOCTYPE preferences SYSTEM "http://java.sun.com/dtd/preferences.dtd">
		''' }</pre>
		''' The UTF-8 character encoding will be used.
		''' 
		''' <p>This method is an exception to the general rule that the results of
		''' concurrently executing multiple methods in this class yields
		''' results equivalent to some serial execution.  If the preferences
		''' at this node are modified concurrently with an invocation of this
		''' method, the exported preferences comprise a "fuzzy snapshot" of the
		''' preferences contained in the node; some of the concurrent modifications
		''' may be reflected in the exported data while others may not.
		''' </summary>
		''' <param name="os"> the output stream on which to emit the XML document. </param>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="BackingStoreException"> if preference data cannot be read from
		'''         backing store. </exception>
		''' <seealso cref=    #importPreferences(InputStream) </seealso>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public MustOverride Sub exportNode(ByVal os As java.io.OutputStream)

		''' <summary>
		''' Emits an XML document representing all of the preferences contained
		''' in this node and all of its descendants.  This XML document is, in
		''' effect, an offline backup of the subtree rooted at the node.
		''' 
		''' <p>The XML document will have the following DOCTYPE declaration:
		''' <pre>{@code
		''' <!DOCTYPE preferences SYSTEM "http://java.sun.com/dtd/preferences.dtd">
		''' }</pre>
		''' The UTF-8 character encoding will be used.
		''' 
		''' <p>This method is an exception to the general rule that the results of
		''' concurrently executing multiple methods in this class yields
		''' results equivalent to some serial execution.  If the preferences
		''' or nodes in the subtree rooted at this node are modified concurrently
		''' with an invocation of this method, the exported preferences comprise a
		''' "fuzzy snapshot" of the subtree; some of the concurrent modifications
		''' may be reflected in the exported data while others may not.
		''' </summary>
		''' <param name="os"> the output stream on which to emit the XML document. </param>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="BackingStoreException"> if preference data cannot be read from
		'''         backing store. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref=    #importPreferences(InputStream) </seealso>
		''' <seealso cref=    #exportNode(OutputStream) </seealso>
		Public MustOverride Sub exportSubtree(ByVal os As java.io.OutputStream)

		''' <summary>
		''' Imports all of the preferences represented by the XML document on the
		''' specified input stream.  The document may represent user preferences or
		''' system preferences.  If it represents user preferences, the preferences
		''' will be imported into the calling user's preference tree (even if they
		''' originally came from a different user's preference tree).  If any of
		''' the preferences described by the document inhabit preference nodes that
		''' do not exist, the nodes will be created.
		''' 
		''' <p>The XML document must have the following DOCTYPE declaration:
		''' <pre>{@code
		''' <!DOCTYPE preferences SYSTEM "http://java.sun.com/dtd/preferences.dtd">
		''' }</pre>
		''' (This method is designed for use in conjunction with
		''' <seealso cref="#exportNode(OutputStream)"/> and
		''' <seealso cref="#exportSubtree(OutputStream)"/>.
		''' 
		''' <p>This method is an exception to the general rule that the results of
		''' concurrently executing multiple methods in this class yields
		''' results equivalent to some serial execution.  The method behaves
		''' as if implemented on top of the other public methods in this [Class],
		''' notably <seealso cref="#node(String)"/> and <seealso cref="#put(String, String)"/>.
		''' </summary>
		''' <param name="is"> the input stream from which to read the XML document. </param>
		''' <exception cref="IOException"> if reading from the specified input stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="InvalidPreferencesFormatException"> Data on input stream does not
		'''         constitute a valid XML document with the mandated document type. </exception>
		''' <exception cref="SecurityException"> If a security manager is present and
		'''         it denies <tt>RuntimePermission("preferences")</tt>. </exception>
		''' <seealso cref=    RuntimePermission </seealso>
		Public Shared Sub importPreferences(ByVal [is] As java.io.InputStream)
			XmlSupport.importPreferences([is])
		End Sub
	End Class

End Namespace
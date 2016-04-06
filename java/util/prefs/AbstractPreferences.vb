Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

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
	''' This class provides a skeletal implementation of the <seealso cref="Preferences"/>
	''' [Class], greatly easing the task of implementing it.
	''' 
	''' <p><strong>This class is for <tt>Preferences</tt> implementers only.
	''' Normal users of the <tt>Preferences</tt> facility should have no need to
	''' consult this documentation.  The <seealso cref="Preferences"/> documentation
	''' should suffice.</strong>
	''' 
	''' <p>Implementors must override the nine abstract service-provider interface
	''' (SPI) methods: <seealso cref="#getSpi(String)"/>, <seealso cref="#putSpi(String,String)"/>,
	''' <seealso cref="#removeSpi(String)"/>, <seealso cref="#childSpi(String)"/>, {@link
	''' #removeNodeSpi()}, <seealso cref="#keysSpi()"/>, <seealso cref="#childrenNamesSpi()"/>, {@link
	''' #syncSpi()} and <seealso cref="#flushSpi()"/>.  All of the concrete methods specify
	''' precisely how they are implemented atop these SPI methods.  The implementor
	''' may, at his discretion, override one or more of the concrete methods if the
	''' default implementation is unsatisfactory for any reason, such as
	''' performance.
	''' 
	''' <p>The SPI methods fall into three groups concerning exception
	''' behavior. The <tt>getSpi</tt> method should never throw exceptions, but it
	''' doesn't really matter, as any exception thrown by this method will be
	''' intercepted by <seealso cref="#get(String,String)"/>, which will return the specified
	''' default value to the caller.  The <tt>removeNodeSpi, keysSpi,
	''' childrenNamesSpi, syncSpi</tt> and <tt>flushSpi</tt> methods are specified
	''' to throw <seealso cref="BackingStoreException"/>, and the implementation is required
	''' to throw this checked exception if it is unable to perform the operation.
	''' The exception propagates outward, causing the corresponding API method
	''' to fail.
	''' 
	''' <p>The remaining SPI methods <seealso cref="#putSpi(String,String)"/>, {@link
	''' #removeSpi(String)} and <seealso cref="#childSpi(String)"/> have more complicated
	''' exception behavior.  They are not specified to throw
	''' <tt>BackingStoreException</tt>, as they can generally obey their contracts
	''' even if the backing store is unavailable.  This is true because they return
	''' no information and their effects are not required to become permanent until
	''' a subsequent call to <seealso cref="Preferences#flush()"/> or
	''' <seealso cref="Preferences#sync()"/>. Generally speaking, these SPI methods should not
	''' throw exceptions.  In some implementations, there may be circumstances
	''' under which these calls cannot even enqueue the requested operation for
	''' later processing.  Even under these circumstances it is generally better to
	''' simply ignore the invocation and return, rather than throwing an
	''' exception.  Under these circumstances, however, all subsequent invocations
	''' of <tt>flush()</tt> and <tt>sync</tt> should return <tt>false</tt>, as
	''' returning <tt>true</tt> would imply that all previous operations had
	''' successfully been made permanent.
	''' 
	''' <p>There is one circumstance under which <tt>putSpi, removeSpi and
	''' childSpi</tt> <i>should</i> throw an exception: if the caller lacks
	''' sufficient privileges on the underlying operating system to perform the
	''' requested operation.  This will, for instance, occur on most systems
	''' if a non-privileged user attempts to modify system preferences.
	''' (The required privileges will vary from implementation to
	''' implementation.  On some implementations, they are the right to modify the
	''' contents of some directory in the file system; on others they are the right
	''' to modify contents of some key in a registry.)  Under any of these
	''' circumstances, it would generally be undesirable to let the program
	''' continue executing as if these operations would become permanent at a later
	''' time.  While implementations are not required to throw an exception under
	''' these circumstances, they are encouraged to do so.  A {@link
	''' SecurityException} would be appropriate.
	''' 
	''' <p>Most of the SPI methods require the implementation to read or write
	''' information at a preferences node.  The implementor should beware of the
	''' fact that another VM may have concurrently deleted this node from the
	''' backing store.  It is the implementation's responsibility to recreate the
	''' node if it has been deleted.
	''' 
	''' <p>Implementation note: In Sun's default <tt>Preferences</tt>
	''' implementations, the user's identity is inherited from the underlying
	''' operating system and does not change for the lifetime of the virtual
	''' machine.  It is recognized that server-side <tt>Preferences</tt>
	''' implementations may have the user identity change from request to request,
	''' implicitly passed to <tt>Preferences</tt> methods via the use of a
	''' static <seealso cref="ThreadLocal"/> instance.  Authors of such implementations are
	''' <i>strongly</i> encouraged to determine the user at the time preferences
	''' are accessed (for example by the <seealso cref="#get(String,String)"/> or {@link
	''' #put(String,String)} method) rather than permanently associating a user
	''' with each <tt>Preferences</tt> instance.  The latter behavior conflicts
	''' with normal <tt>Preferences</tt> usage and would lead to great confusion.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     Preferences
	''' @since   1.4 </seealso>
	Public MustInherit Class AbstractPreferences
		Inherits Preferences

		''' <summary>
		''' Our name relative to parent.
		''' </summary>
		Private ReadOnly name_Renamed As String

		''' <summary>
		''' Our absolute path name.
		''' </summary>
		Private ReadOnly absolutePath_Renamed As String

		''' <summary>
		''' Our parent node.
		''' </summary>
		Friend ReadOnly parent_Renamed As AbstractPreferences

		''' <summary>
		''' Our root node.
		''' </summary>
		Private ReadOnly root As AbstractPreferences ' Relative to this node

		''' <summary>
		''' This field should be <tt>true</tt> if this node did not exist in the
		''' backing store prior to the creation of this object.  The field
		''' is initialized to false, but may be set to true by a subclass
		''' constructor (and should not be modified thereafter).  This field
		''' indicates whether a node change event should be fired when
		''' creation is complete.
		''' </summary>
		Protected Friend newNode As Boolean = False

		''' <summary>
		''' All known unremoved children of this node.  (This "cache" is consulted
		''' prior to calling childSpi() or getChild().
		''' </summary>
		Private kidCache As Map(Of String, AbstractPreferences) = New HashMap(Of String, AbstractPreferences)

		''' <summary>
		''' This field is used to keep track of whether or not this node has
		''' been removed.  Once it's set to true, it will never be reset to false.
		''' </summary>
		Private removed As Boolean = False

		''' <summary>
		''' Registered preference change listeners.
		''' </summary>
		Private prefListeners_Renamed As PreferenceChangeListener() = New PreferenceChangeListener(){}

		''' <summary>
		''' Registered node change listeners.
		''' </summary>
		Private nodeListeners_Renamed As NodeChangeListener() = New NodeChangeListener(){}

		''' <summary>
		''' An object whose monitor is used to lock this node.  This object
		''' is used in preference to the node itself to reduce the likelihood of
		''' intentional or unintentional denial of service due to a locked node.
		''' To avoid deadlock, a node is <i>never</i> locked by a thread that
		''' holds a lock on a descendant of that node.
		''' </summary>
		Protected Friend ReadOnly lock As New Object

		''' <summary>
		''' Creates a preference node with the specified parent and the specified
		''' name relative to its parent.
		''' </summary>
		''' <param name="parent"> the parent of this preference node, or null if this
		'''               is the root. </param>
		''' <param name="name"> the name of this preference node, relative to its parent,
		'''             or <tt>""</tt> if this is the root. </param>
		''' <exception cref="IllegalArgumentException"> if <tt>name</tt> contains a slash
		'''          (<tt>'/'</tt>),  or <tt>parent</tt> is <tt>null</tt> and
		'''          name isn't <tt>""</tt>. </exception>
		Protected Friend Sub New(  parent As AbstractPreferences,   name As String)
			If parent Is Nothing Then
				If Not name.Equals("") Then Throw New IllegalArgumentException("Root name '" & name & "' must be """"")
				Me.absolutePath_Renamed = "/"
				root = Me
			Else
				If name.IndexOf("/"c) <> -1 Then Throw New IllegalArgumentException("Name '" & name & "' contains '/'")
				If name.Equals("") Then Throw New IllegalArgumentException("Illegal name: empty string")

				root = parent.root
				absolutePath_Renamed = (If(parent Is root, "/" & name, parent.absolutePath() & "/" & name))
			End If
			Me.name_Renamed = name
			Me.parent_Renamed = parent
		End Sub

		''' <summary>
		''' Implements the <tt>put</tt> method as per the specification in
		''' <seealso cref="Preferences#put(String,String)"/>.
		''' 
		''' <p>This implementation checks that the key and value are legal,
		''' obtains this preference node's lock, checks that the node
		''' has not been removed, invokes <seealso cref="#putSpi(String,String)"/>, and if
		''' there are any preference change listeners, enqueues a notification
		''' event for processing by the event dispatch thread.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated. </param>
		''' <param name="value"> value to be associated with the specified key. </param>
		''' <exception cref="NullPointerException"> if key or value is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''       <tt>MAX_KEY_LENGTH</tt> or if <tt>value.length</tt> exceeds
		'''       <tt>MAX_VALUE_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub put(  key As String,   value As String)
			If key Is Nothing OrElse value Is Nothing Then Throw New NullPointerException
			If key.length() > MAX_KEY_LENGTH Then Throw New IllegalArgumentException("Key too long: " & key)
			If value.length() > MAX_VALUE_LENGTH Then Throw New IllegalArgumentException("Value too long: " & value)

			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				putSpi(key, value)
				enqueuePreferenceChangeEvent(key, value)
			End SyncLock
		End Sub

		''' <summary>
		''' Implements the <tt>get</tt> method as per the specification in
		''' <seealso cref="Preferences#get(String,String)"/>.
		''' 
		''' <p>This implementation first checks to see if <tt>key</tt> is
		''' <tt>null</tt> throwing a <tt>NullPointerException</tt> if this is
		''' the case.  Then it obtains this preference node's lock,
		''' checks that the node has not been removed, invokes {@link
		''' #getSpi(String)}, and returns the result, unless the <tt>getSpi</tt>
		''' invocation returns <tt>null</tt> or throws an exception, in which case
		''' this invocation returns <tt>def</tt>.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>. </param>
		''' <returns> the value associated with <tt>key</tt>, or <tt>def</tt>
		'''         if no value is associated with <tt>key</tt>. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if key is <tt>null</tt>.  (A
		'''         <tt>null</tt> default <i>is</i> permitted.) </exception>
		Public Overrides Function [get](  key As String,   def As String) As String
			If key Is Nothing Then Throw New NullPointerException("Null key")
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				Dim result As String = Nothing
				Try
					result = getSpi(key)
				Catch e As Exception
					' Ignoring exception causes default to be returned
				End Try
				Return (If(result Is Nothing, def, result))
			End SyncLock
		End Function

		''' <summary>
		''' Implements the <tt>remove(String)</tt> method as per the specification
		''' in <seealso cref="Preferences#remove(String)"/>.
		''' 
		''' <p>This implementation obtains this preference node's lock,
		''' checks that the node has not been removed, invokes
		''' <seealso cref="#removeSpi(String)"/> and if there are any preference
		''' change listeners, enqueues a notification event for processing by the
		''' event dispatch thread.
		''' </summary>
		''' <param name="key"> key whose mapping is to be removed from the preference node. </param>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc}. </exception>
		Public Overrides Sub remove(  key As String)
			Objects.requireNonNull(key, "Specified key cannot be null")
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				removeSpi(key)
				enqueuePreferenceChangeEvent(key, Nothing)
			End SyncLock
		End Sub

		''' <summary>
		''' Implements the <tt>clear</tt> method as per the specification in
		''' <seealso cref="Preferences#clear()"/>.
		''' 
		''' <p>This implementation obtains this preference node's lock,
		''' invokes <seealso cref="#keys()"/> to obtain an array of keys, and
		''' iterates over the array invoking <seealso cref="#remove(String)"/> on each key.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub clear()
			SyncLock lock
				Dim keys As String() = keys()
				For i As Integer = 0 To keys.Length - 1
					remove(keys(i))
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Implements the <tt>putInt</tt> method as per the specification in
		''' <seealso cref="Preferences#putInt(String,int)"/>.
		''' 
		''' <p>This implementation translates <tt>value</tt> to a string with
		''' <seealso cref="Integer#toString(int)"/> and invokes <seealso cref="#put(String,String)"/>
		''' on the result.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub putInt(  key As String,   value As Integer)
			put(key, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Implements the <tt>getInt</tt> method as per the specification in
		''' <seealso cref="Preferences#getInt(String,int)"/>.
		''' 
		''' <p>This implementation invokes {@link #get(String,String) <tt>get(key,
		''' null)</tt>}.  If the return value is non-null, the implementation
		''' attempts to translate it to an <tt>int</tt> with
		''' <seealso cref="Integer#parseInt(String)"/>.  If the attempt succeeds, the return
		''' value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as an int. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as an int. </param>
		''' <returns> the int value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         an int. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		Public Overrides Function getInt(  key As String,   def As Integer) As Integer
			Dim result As Integer = def
			Try
				Dim value As String = [get](key, Nothing)
				If value IsNot Nothing Then result = Convert.ToInt32(value)
			Catch e As NumberFormatException
				' Ignoring exception causes specified default to be returned
			End Try

			Return result
		End Function

		''' <summary>
		''' Implements the <tt>putLong</tt> method as per the specification in
		''' <seealso cref="Preferences#putLong(String,long)"/>.
		''' 
		''' <p>This implementation translates <tt>value</tt> to a string with
		''' <seealso cref="Long#toString(long)"/> and invokes <seealso cref="#put(String,String)"/>
		''' on the result.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub putLong(  key As String,   value As Long)
			put(key, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Implements the <tt>getLong</tt> method as per the specification in
		''' <seealso cref="Preferences#getLong(String,long)"/>.
		''' 
		''' <p>This implementation invokes {@link #get(String,String) <tt>get(key,
		''' null)</tt>}.  If the return value is non-null, the implementation
		''' attempts to translate it to a <tt>long</tt> with
		''' <seealso cref="Long#parseLong(String)"/>.  If the attempt succeeds, the return
		''' value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a java.lang.[Long]. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a java.lang.[Long]. </param>
		''' <returns> the long value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a java.lang.[Long]. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		Public Overrides Function getLong(  key As String,   def As Long) As Long
			Dim result As Long = def
			Try
				Dim value As String = [get](key, Nothing)
				If value IsNot Nothing Then result = Convert.ToInt64(value)
			Catch e As NumberFormatException
				' Ignoring exception causes specified default to be returned
			End Try

			Return result
		End Function

		''' <summary>
		''' Implements the <tt>putBoolean</tt> method as per the specification in
		''' <seealso cref="Preferences#putBoolean(String,boolean)"/>.
		''' 
		''' <p>This implementation translates <tt>value</tt> to a string with
		''' <seealso cref="String#valueOf(boolean)"/> and invokes <seealso cref="#put(String,String)"/>
		''' on the result.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub putBoolean(  key As String,   value As Boolean)
			put(key, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Implements the <tt>getBoolean</tt> method as per the specification in
		''' <seealso cref="Preferences#getBoolean(String,boolean)"/>.
		''' 
		''' <p>This implementation invokes {@link #get(String,String) <tt>get(key,
		''' null)</tt>}.  If the return value is non-null, it is compared with
		''' <tt>"true"</tt> using <seealso cref="String#equalsIgnoreCase(String)"/>.  If the
		''' comparison returns <tt>true</tt>, this invocation returns
		''' <tt>true</tt>.  Otherwise, the original return value is compared with
		''' <tt>"false"</tt>, again using <seealso cref="String#equalsIgnoreCase(String)"/>.
		''' If the comparison returns <tt>true</tt>, this invocation returns
		''' <tt>false</tt>.  Otherwise, this invocation returns <tt>def</tt>.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a  java.lang.[Boolean]. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a  java.lang.[Boolean]. </param>
		''' <returns> the boolean value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a  java.lang.[Boolean]. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		Public Overrides Function getBoolean(  key As String,   def As Boolean) As Boolean
			Dim result As Boolean = def
			Dim value As String = [get](key, Nothing)
			If value IsNot Nothing Then
				If value.equalsIgnoreCase("true") Then
					result = True
				ElseIf value.equalsIgnoreCase("false") Then
					result = False
				End If
			End If

			Return result
		End Function

		''' <summary>
		''' Implements the <tt>putFloat</tt> method as per the specification in
		''' <seealso cref="Preferences#putFloat(String,float)"/>.
		''' 
		''' <p>This implementation translates <tt>value</tt> to a string with
		''' <seealso cref="Float#toString(float)"/> and invokes <seealso cref="#put(String,String)"/>
		''' on the result.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub putFloat(  key As String,   value As Single)
			put(key, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Implements the <tt>getFloat</tt> method as per the specification in
		''' <seealso cref="Preferences#getFloat(String,float)"/>.
		''' 
		''' <p>This implementation invokes {@link #get(String,String) <tt>get(key,
		''' null)</tt>}.  If the return value is non-null, the implementation
		''' attempts to translate it to an <tt>float</tt> with
		''' <seealso cref="Float#parseFloat(String)"/>.  If the attempt succeeds, the return
		''' value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a float. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a float. </param>
		''' <returns> the float value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a float. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		Public Overrides Function getFloat(  key As String,   def As Single) As Single
			Dim result As Single = def
			Try
				Dim value As String = [get](key, Nothing)
				If value IsNot Nothing Then result = Convert.ToSingle(value)
			Catch e As NumberFormatException
				' Ignoring exception causes specified default to be returned
			End Try

			Return result
		End Function

		''' <summary>
		''' Implements the <tt>putDouble</tt> method as per the specification in
		''' <seealso cref="Preferences#putDouble(String,double)"/>.
		''' 
		''' <p>This implementation translates <tt>value</tt> to a string with
		''' <seealso cref="Double#toString(double)"/> and invokes <seealso cref="#put(String,String)"/>
		''' on the result.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		'''         <tt>MAX_KEY_LENGTH</tt>. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub putDouble(  key As String,   value As Double)
			put(key, Convert.ToString(value))
		End Sub

		''' <summary>
		''' Implements the <tt>getDouble</tt> method as per the specification in
		''' <seealso cref="Preferences#getDouble(String,double)"/>.
		''' 
		''' <p>This implementation invokes {@link #get(String,String) <tt>get(key,
		''' null)</tt>}.  If the return value is non-null, the implementation
		''' attempts to translate it to an <tt>double</tt> with
		''' <seealso cref="Double#parseDouble(String)"/>.  If the attempt succeeds, the return
		''' value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a java.lang.[Double]. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a java.lang.[Double]. </param>
		''' <returns> the double value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a java.lang.[Double]. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		Public Overrides Function getDouble(  key As String,   def As Double) As Double
			Dim result As Double = def
			Try
				Dim value As String = [get](key, Nothing)
				If value IsNot Nothing Then result = Convert.ToDouble(value)
			Catch e As NumberFormatException
				' Ignoring exception causes specified default to be returned
			End Try

			Return result
		End Function

		''' <summary>
		''' Implements the <tt>putByteArray</tt> method as per the specification in
		''' <seealso cref="Preferences#putByteArray(String,byte[])"/>.
		''' </summary>
		''' <param name="key"> key with which the string form of value is to be associated. </param>
		''' <param name="value"> value whose string form is to be associated with key. </param>
		''' <exception cref="NullPointerException"> if key or value is <tt>null</tt>. </exception>
		''' <exception cref="IllegalArgumentException"> if key.length() exceeds MAX_KEY_LENGTH
		'''         or if value.length exceeds MAX_VALUE_LENGTH*3/4. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Sub putByteArray(  key As String,   value As SByte())
			put(key, Base64.byteArrayToBase64(value))
		End Sub

		''' <summary>
		''' Implements the <tt>getByteArray</tt> method as per the specification in
		''' <seealso cref="Preferences#getByteArray(String,byte[])"/>.
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned as a byte array. </param>
		''' <param name="def"> the value to be returned in the event that this
		'''        preference node has no value associated with <tt>key</tt>
		'''        or the associated value cannot be interpreted as a byte array. </param>
		''' <returns> the byte array value represented by the string associated with
		'''         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		'''         associated value does not exist or cannot be interpreted as
		'''         a byte array. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>.  (A
		'''         <tt>null</tt> value for <tt>def</tt> <i>is</i> permitted.) </exception>
		Public Overrides Function getByteArray(  key As String,   def As SByte()) As SByte()
			Dim result As SByte() = def
			Dim value As String = [get](key, Nothing)
			Try
				If value IsNot Nothing Then result = Base64.base64ToByteArray(value)
			Catch e As RuntimeException
				' Ignoring exception causes specified default to be returned
			End Try

			Return result
		End Function

		''' <summary>
		''' Implements the <tt>keys</tt> method as per the specification in
		''' <seealso cref="Preferences#keys()"/>.
		''' 
		''' <p>This implementation obtains this preference node's lock, checks that
		''' the node has not been removed and invokes <seealso cref="#keysSpi()"/>.
		''' </summary>
		''' <returns> an array of the keys that have an associated value in this
		'''         preference node. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Function keys() As String()
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				Return keysSpi()
			End SyncLock
		End Function

		''' <summary>
		''' Implements the <tt>children</tt> method as per the specification in
		''' <seealso cref="Preferences#childrenNames()"/>.
		''' 
		''' <p>This implementation obtains this preference node's lock, checks that
		''' the node has not been removed, constructs a <tt>TreeSet</tt> initialized
		''' to the names of children already cached (the children in this node's
		''' "child-cache"), invokes <seealso cref="#childrenNamesSpi()"/>, and adds all of the
		''' returned child-names into the set.  The elements of the tree set are
		''' dumped into a <tt>String</tt> array using the <tt>toArray</tt> method,
		''' and this array is returned.
		''' </summary>
		''' <returns> the names of the children of this preference node. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #cachedChildren() </seealso>
		Public Overrides Function childrenNames() As String()
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				Dim s As IDictionary(Of String, AbstractPreferences).KeyCollection = New TreeSet(Of String)(kidCache.Keys)
				For Each kid As String In childrenNamesSpi()
					s.add(kid)
				Next kid
				Return s.ToArray(EMPTY_STRING_ARRAY)
			End SyncLock
		End Function

		Private Shared ReadOnly EMPTY_STRING_ARRAY As String() = New String(){}

		''' <summary>
		''' Returns all known unremoved children of this node.
		''' </summary>
		''' <returns> all known unremoved children of this node. </returns>
		Protected Friend Function cachedChildren() As AbstractPreferences()
			Return kidCache.values().ToArray(EMPTY_ABSTRACT_PREFS_ARRAY)
		End Function

		Private Shared ReadOnly EMPTY_ABSTRACT_PREFS_ARRAY As AbstractPreferences() = New AbstractPreferences(){}

		''' <summary>
		''' Implements the <tt>parent</tt> method as per the specification in
		''' <seealso cref="Preferences#parent()"/>.
		''' 
		''' <p>This implementation obtains this preference node's lock, checks that
		''' the node has not been removed and returns the parent value that was
		''' passed to this node's constructor.
		''' </summary>
		''' <returns> the parent of this preference node. </returns>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Function parent() As Preferences
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				Return parent_Renamed
			End SyncLock
		End Function

		''' <summary>
		''' Implements the <tt>node</tt> method as per the specification in
		''' <seealso cref="Preferences#node(String)"/>.
		''' 
		''' <p>This implementation obtains this preference node's lock and checks
		''' that the node has not been removed.  If <tt>path</tt> is <tt>""</tt>,
		''' this node is returned; if <tt>path</tt> is <tt>"/"</tt>, this node's
		''' root is returned.  If the first character in <tt>path</tt> is
		''' not <tt>'/'</tt>, the implementation breaks <tt>path</tt> into
		''' tokens and recursively traverses the path from this node to the
		''' named node, "consuming" a name and a slash from <tt>path</tt> at
		''' each step of the traversal.  At each step, the current node is locked
		''' and the node's child-cache is checked for the named node.  If it is
		''' not found, the name is checked to make sure its length does not
		''' exceed <tt>MAX_NAME_LENGTH</tt>.  Then the <seealso cref="#childSpi(String)"/>
		''' method is invoked, and the result stored in this node's child-cache.
		''' If the newly created <tt>Preferences</tt> object's <seealso cref="#newNode"/>
		''' field is <tt>true</tt> and there are any node change listeners,
		''' a notification event is enqueued for processing by the event dispatch
		''' thread.
		''' 
		''' <p>When there are no more tokens, the last value found in the
		''' child-cache or returned by <tt>childSpi</tt> is returned by this
		''' method.  If during the traversal, two <tt>"/"</tt> tokens occur
		''' consecutively, or the final token is <tt>"/"</tt> (rather than a name),
		''' an appropriate <tt>IllegalArgumentException</tt> is thrown.
		''' 
		''' <p> If the first character of <tt>path</tt> is <tt>'/'</tt>
		''' (indicating an absolute path name) this preference node's
		''' lock is dropped prior to breaking <tt>path</tt> into tokens, and
		''' this method recursively traverses the path starting from the root
		''' (rather than starting from this node).  The traversal is otherwise
		''' identical to the one described for relative path names.  Dropping
		''' the lock on this node prior to commencing the traversal at the root
		''' node is essential to avoid the possibility of deadlock, as per the
		''' <seealso cref="#lock locking invariant"/>.
		''' </summary>
		''' <param name="path"> the path name of the preference node to return. </param>
		''' <returns> the specified preference node. </returns>
		''' <exception cref="IllegalArgumentException"> if the path name is invalid (i.e.,
		'''         it contains multiple consecutive slash characters, or ends
		'''         with a slash character and is more than one character long). </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		Public Overrides Function node(  path As String) As Preferences
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")
				If path.Equals("") Then Return Me
				If path.Equals("/") Then Return root
				If path.Chars(0) <> "/"c Then Return node(New StringTokenizer(path, "/", True))
			End SyncLock

			' Absolute path.  Note that we've dropped our lock to avoid deadlock
			Return root.node(New StringTokenizer(path.Substring(1), "/", True))
		End Function

		''' <summary>
		''' tokenizer contains <name> {'/' <name>}*
		''' </summary>
		Private Function node(  path As StringTokenizer) As Preferences
			Dim token As String = path.nextToken()
			If token.Equals("/") Then ' Check for consecutive slashes Throw New IllegalArgumentException("Consecutive slashes in path")
			SyncLock lock
				Dim child_Renamed As AbstractPreferences = kidCache.get(token)
				If child_Renamed Is Nothing Then
					If token.length() > MAX_NAME_LENGTH Then Throw New IllegalArgumentException("Node name " & token & " too long")
					child_Renamed = childSpi(token)
					If child_Renamed.newNode Then enqueueNodeAddedEvent(child_Renamed)
					kidCache.put(token, child_Renamed)
				End If
				If Not path.hasMoreTokens() Then Return child_Renamed
				path.nextToken() ' Consume slash
				If Not path.hasMoreTokens() Then Throw New IllegalArgumentException("Path ends with slash")
				Return child_Renamed.node(path)
			End SyncLock
		End Function

		''' <summary>
		''' Implements the <tt>nodeExists</tt> method as per the specification in
		''' <seealso cref="Preferences#nodeExists(String)"/>.
		''' 
		''' <p>This implementation is very similar to <seealso cref="#node(String)"/>,
		''' except that <seealso cref="#getChild(String)"/> is used instead of {@link
		''' #childSpi(String)}.
		''' </summary>
		''' <param name="path"> the path name of the node whose existence is to be checked. </param>
		''' <returns> true if the specified node exists. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalArgumentException"> if the path name is invalid (i.e.,
		'''         it contains multiple consecutive slash characters, or ends
		'''         with a slash character and is more than one character long). </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method and
		'''         <tt>pathname</tt> is not the empty string (<tt>""</tt>). </exception>
		Public Overrides Function nodeExists(  path As String) As Boolean
			SyncLock lock
				If path.Equals("") Then Return Not removed
				If removed Then Throw New IllegalStateException("Node has been removed.")
				If path.Equals("/") Then Return True
				If path.Chars(0) <> "/"c Then Return nodeExists(New StringTokenizer(path, "/", True))
			End SyncLock

			' Absolute path.  Note that we've dropped our lock to avoid deadlock
			Return root.nodeExists(New StringTokenizer(path.Substring(1), "/", True))
		End Function

		''' <summary>
		''' tokenizer contains <name> {'/' <name>}*
		''' </summary>
		Private Function nodeExists(  path As StringTokenizer) As Boolean
			Dim token As String = path.nextToken()
			If token.Equals("/") Then ' Check for consecutive slashes Throw New IllegalArgumentException("Consecutive slashes in path")
			SyncLock lock
				Dim child_Renamed As AbstractPreferences = kidCache.get(token)
				If child_Renamed Is Nothing Then child_Renamed = getChild(token)
				If child_Renamed Is Nothing Then Return False
				If Not path.hasMoreTokens() Then Return True
				path.nextToken() ' Consume slash
				If Not path.hasMoreTokens() Then Throw New IllegalArgumentException("Path ends with slash")
				Return child_Renamed.nodeExists(path)
			End SyncLock
		End Function

		''' 
		''' <summary>
		''' Implements the <tt>removeNode()</tt> method as per the specification in
		''' <seealso cref="Preferences#removeNode()"/>.
		''' 
		''' <p>This implementation checks to see that this node is the root; if so,
		''' it throws an appropriate exception.  Then, it locks this node's parent,
		''' and calls a recursive helper method that traverses the subtree rooted at
		''' this node.  The recursive method locks the node on which it was called,
		''' checks that it has not already been removed, and then ensures that all
		''' of its children are cached: The <seealso cref="#childrenNamesSpi()"/> method is
		''' invoked and each returned child name is checked for containment in the
		''' child-cache.  If a child is not already cached, the {@link
		''' #childSpi(String)} method is invoked to create a <tt>Preferences</tt>
		''' instance for it, and this instance is put into the child-cache.  Then
		''' the helper method calls itself recursively on each node contained in its
		''' child-cache.  Next, it invokes <seealso cref="#removeNodeSpi()"/>, marks itself
		''' as removed, and removes itself from its parent's child-cache.  Finally,
		''' if there are any node change listeners, it enqueues a notification
		''' event for processing by the event dispatch thread.
		''' 
		''' <p>Note that the helper method is always invoked with all ancestors up
		''' to the "closest non-removed ancestor" locked.
		''' </summary>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has already
		'''         been removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <exception cref="UnsupportedOperationException"> if this method is invoked on
		'''         the root node. </exception>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Public Overrides Sub removeNode()
			If Me Is root Then Throw New UnsupportedOperationException("Can't remove the root!")
			SyncLock parent_Renamed.lock
				removeNode2()
				parent_Renamed.kidCache.remove(name_Renamed)
			End SyncLock
		End Sub

	'    
	'     * Called with locks on all nodes on path from parent of "removal root"
	'     * to this (including the former but excluding the latter).
	'     
		Private Sub removeNode2()
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node already removed.")

				' Ensure that all children are cached
				Dim kidNames As String() = childrenNamesSpi()
				For i As Integer = 0 To kidNames.Length - 1
					If Not kidCache.containsKey(kidNames(i)) Then kidCache.put(kidNames(i), childSpi(kidNames(i)))
				Next i

				' Recursively remove all cached children
				Dim i As [Iterator](Of AbstractPreferences) = kidCache.values().GetEnumerator()
				Do While i.MoveNext()
					Try
						i.Current.removeNode2()
						i.remove()
					Catch x As BackingStoreException
					End Try
				Loop

				' Now we have no descendants - it's time to die!
				removeNodeSpi()
				removed = True
				parent_Renamed.enqueueNodeRemovedEvent(Me)
			End SyncLock
		End Sub

		''' <summary>
		''' Implements the <tt>name</tt> method as per the specification in
		''' <seealso cref="Preferences#name()"/>.
		''' 
		''' <p>This implementation merely returns the name that was
		''' passed to this node's constructor.
		''' </summary>
		''' <returns> this preference node's name, relative to its parent. </returns>
		Public Overrides Function name() As String
			Return name_Renamed
		End Function

		''' <summary>
		''' Implements the <tt>absolutePath</tt> method as per the specification in
		''' <seealso cref="Preferences#absolutePath()"/>.
		''' 
		''' <p>This implementation merely returns the absolute path name that
		''' was computed at the time that this node was constructed (based on
		''' the name that was passed to this node's constructor, and the names
		''' that were passed to this node's ancestors' constructors).
		''' </summary>
		''' <returns> this preference node's absolute path name. </returns>
		Public Overrides Function absolutePath() As String
			Return absolutePath_Renamed
		End Function

		''' <summary>
		''' Implements the <tt>isUserNode</tt> method as per the specification in
		''' <seealso cref="Preferences#isUserNode()"/>.
		''' 
		''' <p>This implementation compares this node's root node (which is stored
		''' in a private field) with the value returned by
		''' <seealso cref="Preferences#userRoot()"/>.  If the two object references are
		''' identical, this method returns true.
		''' </summary>
		''' <returns> <tt>true</tt> if this preference node is in the user
		'''         preference tree, <tt>false</tt> if it's in the system
		'''         preference tree. </returns>
		Public  Overrides ReadOnly Property  userNode As Boolean
			Get
				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Boolean?
				Return outerInstance.root Is Preferences.userRoot()
			End Function
		End Class

		Public Overrides Sub addPreferenceChangeListener(  pcl As PreferenceChangeListener)
			If pcl Is Nothing Then Throw New NullPointerException("Change listener is null.")
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				' Copy-on-write
				Dim old As PreferenceChangeListener() = prefListeners_Renamed
				prefListeners_Renamed = New PreferenceChangeListener(old.Length){}
				Array.Copy(old, 0, prefListeners_Renamed, 0, old.Length)
				prefListeners_Renamed(old.Length) = pcl
			End SyncLock
			startEventDispatchThreadIfNecessary()
		End Sub

		Public Overrides Sub removePreferenceChangeListener(  pcl As PreferenceChangeListener)
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")
				If (prefListeners_Renamed Is Nothing) OrElse (prefListeners_Renamed.Length = 0) Then Throw New IllegalArgumentException("Listener not registered.")

				' Copy-on-write
				Dim newPl As PreferenceChangeListener() = New PreferenceChangeListener(prefListeners_Renamed.Length - 2){}
				Dim i As Integer = 0
				Do While i < newPl.Length AndAlso prefListeners_Renamed(i) IsNot pcl
						newPl(i) = prefListeners_Renamed(i)
						i += 1
				Loop

				If i = newPl.Length AndAlso prefListeners_Renamed(i) IsNot pcl Then Throw New IllegalArgumentException("Listener not registered.")
				Do While i < newPl.Length
						i += 1
				Loop
						newPl(i) = prefListeners_Renamed(i)
				prefListeners_Renamed = newPl
			End SyncLock
		End Sub

		Public Overrides Sub addNodeChangeListener(  ncl As NodeChangeListener)
			If ncl Is Nothing Then Throw New NullPointerException("Change listener is null.")
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")

				' Copy-on-write
				If nodeListeners_Renamed Is Nothing Then
					nodeListeners_Renamed = New NodeChangeListener(0){}
					nodeListeners_Renamed(0) = ncl
				Else
					Dim old As NodeChangeListener() = nodeListeners_Renamed
					nodeListeners_Renamed = New NodeChangeListener(old.Length){}
					Array.Copy(old, 0, nodeListeners_Renamed, 0, old.Length)
					nodeListeners_Renamed(old.Length) = ncl
				End If
			End SyncLock
			startEventDispatchThreadIfNecessary()
		End Sub

		Public Overrides Sub removeNodeChangeListener(  ncl As NodeChangeListener)
			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed.")
				If (nodeListeners_Renamed Is Nothing) OrElse (nodeListeners_Renamed.Length = 0) Then Throw New IllegalArgumentException("Listener not registered.")

				' Copy-on-write
				Dim i As Integer = 0
				Do While i < nodeListeners_Renamed.Length AndAlso nodeListeners_Renamed(i) IsNot ncl
					i += 1
				Loop
				If i = nodeListeners_Renamed.Length Then Throw New IllegalArgumentException("Listener not registered.")
				Dim newNl As NodeChangeListener() = New NodeChangeListener(nodeListeners_Renamed.Length - 2){}
				If i <> 0 Then Array.Copy(nodeListeners_Renamed, 0, newNl, 0, i)
				If i <> newNl.Length Then Array.Copy(nodeListeners_Renamed, i + 1, newNl, i, newNl.Length - i)
				nodeListeners_Renamed = newNl
			End SyncLock
		End Sub

		' "SPI" METHODS

		''' <summary>
		''' Put the given key-value association into this preference node.  It is
		''' guaranteed that <tt>key</tt> and <tt>value</tt> are non-null and of
		''' legal length.  Also, it is guaranteed that this node has not been
		''' removed.  (The implementor needn't check for any of these things.)
		''' 
		''' <p>This method is invoked with the lock on this node held. </summary>
		''' <param name="key"> the key </param>
		''' <param name="value"> the value </param>
		Protected Friend MustOverride Sub putSpi(  key As String,   value As String)

		''' <summary>
		''' Return the value associated with the specified key at this preference
		''' node, or <tt>null</tt> if there is no association for this key, or the
		''' association cannot be determined at this time.  It is guaranteed that
		''' <tt>key</tt> is non-null.  Also, it is guaranteed that this node has
		''' not been removed.  (The implementor needn't check for either of these
		''' things.)
		''' 
		''' <p> Generally speaking, this method should not throw an exception
		''' under any circumstances.  If, however, if it does throw an exception,
		''' the exception will be intercepted and treated as a <tt>null</tt>
		''' return value.
		''' 
		''' <p>This method is invoked with the lock on this node held.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> the value associated with the specified key at this preference
		'''          node, or <tt>null</tt> if there is no association for this
		'''          key, or the association cannot be determined at this time. </returns>
		Protected Friend MustOverride Function getSpi(  key As String) As String

		''' <summary>
		''' Remove the association (if any) for the specified key at this
		''' preference node.  It is guaranteed that <tt>key</tt> is non-null.
		''' Also, it is guaranteed that this node has not been removed.
		''' (The implementor needn't check for either of these things.)
		''' 
		''' <p>This method is invoked with the lock on this node held. </summary>
		''' <param name="key"> the key </param>
		Protected Friend MustOverride Sub removeSpi(  key As String)

		''' <summary>
		''' Removes this preference node, invalidating it and any preferences that
		''' it contains.  The named child will have no descendants at the time this
		''' invocation is made (i.e., the <seealso cref="Preferences#removeNode()"/> method
		''' invokes this method repeatedly in a bottom-up fashion, removing each of
		''' a node's descendants before removing the node itself).
		''' 
		''' <p>This method is invoked with the lock held on this node and its
		''' parent (and all ancestors that are being removed as a
		''' result of a single invocation to <seealso cref="Preferences#removeNode()"/>).
		''' 
		''' <p>The removal of a node needn't become persistent until the
		''' <tt>flush</tt> method is invoked on this node (or an ancestor).
		''' 
		''' <p>If this node throws a <tt>BackingStoreException</tt>, the exception
		''' will propagate out beyond the enclosing <seealso cref="#removeNode()"/>
		''' invocation.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Protected Friend MustOverride Sub removeNodeSpi()

		''' <summary>
		''' Returns all of the keys that have an associated value in this
		''' preference node.  (The returned array will be of size zero if
		''' this node has no preferences.)  It is guaranteed that this node has not
		''' been removed.
		''' 
		''' <p>This method is invoked with the lock on this node held.
		''' 
		''' <p>If this node throws a <tt>BackingStoreException</tt>, the exception
		''' will propagate out beyond the enclosing <seealso cref="#keys()"/> invocation.
		''' </summary>
		''' <returns> an array of the keys that have an associated value in this
		'''         preference node. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Protected Friend MustOverride Function keysSpi() As String()

		''' <summary>
		''' Returns the names of the children of this preference node.  (The
		''' returned array will be of size zero if this node has no children.)
		''' This method need not return the names of any nodes already cached,
		''' but may do so without harm.
		''' 
		''' <p>This method is invoked with the lock on this node held.
		''' 
		''' <p>If this node throws a <tt>BackingStoreException</tt>, the exception
		''' will propagate out beyond the enclosing <seealso cref="#childrenNames()"/>
		''' invocation.
		''' </summary>
		''' <returns> an array containing the names of the children of this
		'''         preference node. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Protected Friend MustOverride Function childrenNamesSpi() As String()

		''' <summary>
		''' Returns the named child if it exists, or <tt>null</tt> if it does not.
		''' It is guaranteed that <tt>nodeName</tt> is non-null, non-empty,
		''' does not contain the slash character ('/'), and is no longer than
		''' <seealso cref="#MAX_NAME_LENGTH"/> characters.  Also, it is guaranteed
		''' that this node has not been removed.  (The implementor needn't check
		''' for any of these things if he chooses to override this method.)
		''' 
		''' <p>Finally, it is guaranteed that the named node has not been returned
		''' by a previous invocation of this method or <seealso cref="#childSpi"/> after the
		''' last time that it was removed.  In other words, a cached value will
		''' always be used in preference to invoking this method.  (The implementor
		''' needn't maintain his own cache of previously returned children if he
		''' chooses to override this method.)
		''' 
		''' <p>This implementation obtains this preference node's lock, invokes
		''' <seealso cref="#childrenNames()"/> to get an array of the names of this node's
		''' children, and iterates over the array comparing the name of each child
		''' with the specified node name.  If a child node has the correct name,
		''' the <seealso cref="#childSpi(String)"/> method is invoked and the resulting
		''' node is returned.  If the iteration completes without finding the
		''' specified name, <tt>null</tt> is returned.
		''' </summary>
		''' <param name="nodeName"> name of the child to be searched for. </param>
		''' <returns> the named child if it exists, or null if it does not. </returns>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Protected Friend Overridable Function getChild(  nodeName As String) As AbstractPreferences
			SyncLock lock
				' assert kidCache.get(nodeName)==null;
				Dim kidNames As String() = childrenNames()
				For i As Integer = 0 To kidNames.Length - 1
					If kidNames(i).Equals(nodeName) Then Return childSpi(kidNames(i))
				Next i
			End SyncLock
			Return Nothing
		End Function

		''' <summary>
		''' Returns the named child of this preference node, creating it if it does
		''' not already exist.  It is guaranteed that <tt>name</tt> is non-null,
		''' non-empty, does not contain the slash character ('/'), and is no longer
		''' than <seealso cref="#MAX_NAME_LENGTH"/> characters.  Also, it is guaranteed that
		''' this node has not been removed.  (The implementor needn't check for any
		''' of these things.)
		''' 
		''' <p>Finally, it is guaranteed that the named node has not been returned
		''' by a previous invocation of this method or <seealso cref="#getChild(String)"/>
		''' after the last time that it was removed.  In other words, a cached
		''' value will always be used in preference to invoking this method.
		''' Subclasses need not maintain their own cache of previously returned
		''' children.
		''' 
		''' <p>The implementer must ensure that the returned node has not been
		''' removed.  If a like-named child of this node was previously removed, the
		''' implementer must return a newly constructed <tt>AbstractPreferences</tt>
		''' node; once removed, an <tt>AbstractPreferences</tt> node
		''' cannot be "resuscitated."
		''' 
		''' <p>If this method causes a node to be created, this node is not
		''' guaranteed to be persistent until the <tt>flush</tt> method is
		''' invoked on this node or one of its ancestors (or descendants).
		''' 
		''' <p>This method is invoked with the lock on this node held.
		''' </summary>
		''' <param name="name"> The name of the child node to return, relative to
		'''        this preference node. </param>
		''' <returns> The named child node. </returns>
		Protected Friend MustOverride Function childSpi(  name As String) As AbstractPreferences

		''' <summary>
		''' Returns the absolute path name of this preferences node.
		''' </summary>
		Public Overrides Function ToString() As String
			Return (If(Me.userNode, "User", "System")) & " Preference Node: " & Me.absolutePath()
		End Function

		''' <summary>
		''' Implements the <tt>sync</tt> method as per the specification in
		''' <seealso cref="Preferences#sync()"/>.
		''' 
		''' <p>This implementation calls a recursive helper method that locks this
		''' node, invokes syncSpi() on it, unlocks this node, and recursively
		''' invokes this method on each "cached child."  A cached child is a child
		''' of this node that has been created in this VM and not subsequently
		''' removed.  In effect, this method does a depth first traversal of the
		''' "cached subtree" rooted at this node, calling syncSpi() on each node in
		''' the subTree while only that node is locked. Note that syncSpi() is
		''' invoked top-down.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		'''         removed with the <seealso cref="#removeNode()"/> method. </exception>
		''' <seealso cref= #flush() </seealso>
		Public Overrides Sub sync()
			sync2()
		End Sub

		Private Sub sync2()
			Dim cachedKids As AbstractPreferences()

			SyncLock lock
				If removed Then Throw New IllegalStateException("Node has been removed")
				syncSpi()
				cachedKids = cachedChildren()
			End SyncLock

			For i As Integer = 0 To cachedKids.Length - 1
				cachedKids(i).sync2()
			Next i
		End Sub

		''' <summary>
		''' This method is invoked with this node locked.  The contract of this
		''' method is to synchronize any cached preferences stored at this node
		''' with any stored in the backing store.  (It is perfectly possible that
		''' this node does not exist on the backing store, either because it has
		''' been deleted by another VM, or because it has not yet been created.)
		''' Note that this method should <i>not</i> synchronize the preferences in
		''' any subnodes of this node.  If the backing store naturally syncs an
		''' entire subtree at once, the implementer is encouraged to override
		''' sync(), rather than merely overriding this method.
		''' 
		''' <p>If this node throws a <tt>BackingStoreException</tt>, the exception
		''' will propagate out beyond the enclosing <seealso cref="#sync()"/> invocation.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Protected Friend MustOverride Sub syncSpi()

		''' <summary>
		''' Implements the <tt>flush</tt> method as per the specification in
		''' <seealso cref="Preferences#flush()"/>.
		''' 
		''' <p>This implementation calls a recursive helper method that locks this
		''' node, invokes flushSpi() on it, unlocks this node, and recursively
		''' invokes this method on each "cached child."  A cached child is a child
		''' of this node that has been created in this VM and not subsequently
		''' removed.  In effect, this method does a depth first traversal of the
		''' "cached subtree" rooted at this node, calling flushSpi() on each node in
		''' the subTree while only that node is locked. Note that flushSpi() is
		''' invoked top-down.
		''' 
		''' <p> If this method is invoked on a node that has been removed with
		''' the <seealso cref="#removeNode()"/> method, flushSpi() is invoked on this node,
		''' but not on others.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		''' <seealso cref= #flush() </seealso>
		Public Overrides Sub flush()
			flush2()
		End Sub

		Private Sub flush2()
			Dim cachedKids As AbstractPreferences()

			SyncLock lock
				flushSpi()
				If removed Then Return
				cachedKids = cachedChildren()
			End SyncLock

			For i As Integer = 0 To cachedKids.Length - 1
				cachedKids(i).flush2()
			Next i
		End Sub

		''' <summary>
		''' This method is invoked with this node locked.  The contract of this
		''' method is to force any cached changes in the contents of this
		''' preference node to the backing store, guaranteeing their persistence.
		''' (It is perfectly possible that this node does not exist on the backing
		''' store, either because it has been deleted by another VM, or because it
		''' has not yet been created.)  Note that this method should <i>not</i>
		''' flush the preferences in any subnodes of this node.  If the backing
		''' store naturally flushes an entire subtree at once, the implementer is
		''' encouraged to override flush(), rather than merely overriding this
		''' method.
		''' 
		''' <p>If this node throws a <tt>BackingStoreException</tt>, the exception
		''' will propagate out beyond the enclosing <seealso cref="#flush()"/> invocation.
		''' </summary>
		''' <exception cref="BackingStoreException"> if this operation cannot be completed
		'''         due to a failure in the backing store, or inability to
		'''         communicate with it. </exception>
		Protected Friend MustOverride Sub flushSpi()

		''' <summary>
		''' Returns <tt>true</tt> iff this node (or an ancestor) has been
		''' removed with the <seealso cref="#removeNode()"/> method.  This method
		''' locks this node prior to returning the contents of the private
		''' field used to track this state.
		''' </summary>
		''' <returns> <tt>true</tt> iff this node (or an ancestor) has been
		'''       removed with the <seealso cref="#removeNode()"/> method. </returns>
		Protected Friend Overridable Property removed As Boolean
			Get
				SyncLock lock
					Return removed
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Queue of pending notification events.  When a preference or node
		''' change event for which there are one or more listeners occurs,
		''' it is placed on this queue and the queue is notified.  A background
		''' thread waits on this queue and delivers the events.  This decouples
		''' event delivery from preference activity, greatly simplifying
		''' locking and reducing opportunity for deadlock.
		''' </summary>
		Private Shared ReadOnly eventQueue As List(Of EventObject) = New LinkedList(Of EventObject)

		''' <summary>
		''' These two classes are used to distinguish NodeChangeEvents on
		''' eventQueue so the event dispatch thread knows whether to call
		''' childAdded or childRemoved.
		''' </summary>
		Private Class NodeAddedEvent
			Inherits NodeChangeEvent

			Private ReadOnly outerInstance As AbstractPreferences

			Private Const serialVersionUID As Long = -6743557530157328528L
			Friend Sub New(  outerInstance As AbstractPreferences,   parent As Preferences,   child As Preferences)
					Me.outerInstance = outerInstance
				MyBase.New(parent, child)
			End Sub
		End Class
		Private Class NodeRemovedEvent
			Inherits NodeChangeEvent

			Private ReadOnly outerInstance As AbstractPreferences

			Private Const serialVersionUID As Long = 8735497392918824837L
			Friend Sub New(  outerInstance As AbstractPreferences,   parent As Preferences,   child As Preferences)
					Me.outerInstance = outerInstance
				MyBase.New(parent, child)
			End Sub
		End Class

		''' <summary>
		''' A single background thread ("the event notification thread") monitors
		''' the event queue and delivers events that are placed on the queue.
		''' </summary>
		Private Class EventDispatchThread
			Inherits Thread

			Public Overrides Sub run()
				Do
					' Wait on eventQueue till an event is present
					Dim [event] As EventObject = Nothing
					SyncLock eventQueue
						Try
							Do While eventQueue.empty
								eventQueue.wait()
							Loop
							[event] = eventQueue.remove(0)
						Catch e As InterruptedException
							' XXX Log "Event dispatch thread interrupted. Exiting"
							Return
						End Try
					End SyncLock

					' Now we have event & hold no locks; deliver evt to listeners
					Dim src As AbstractPreferences=CType([event].source, AbstractPreferences)
					If TypeOf [event] Is PreferenceChangeEvent Then
						Dim pce As PreferenceChangeEvent = CType([event], PreferenceChangeEvent)
						Dim listeners As PreferenceChangeListener() = src.prefListeners()
						For i As Integer = 0 To listeners.Length - 1
							listeners(i).preferenceChange(pce)
						Next i
					Else
						Dim nce As NodeChangeEvent = CType([event], NodeChangeEvent)
						Dim listeners As NodeChangeListener() = src.nodeListeners()
						If TypeOf nce Is NodeAddedEvent Then
							For i As Integer = 0 To listeners.Length - 1
								listeners(i).childAdded(nce)
							Next i
						Else
							' assert nce instanceof NodeRemovedEvent;
							For i As Integer = 0 To listeners.Length - 1
								listeners(i).childRemoved(nce)
							Next i
						End If
					End If
				Loop
			End Sub
		End Class

		Private Shared eventDispatchThread As Thread = Nothing

		''' <summary>
		''' This method starts the event dispatch thread the first time it
		''' is called.  The event dispatch thread will be started only
		''' if someone registers a listener.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub startEventDispatchThreadIfNecessary()
			If eventDispatchThread Is Nothing Then
				' XXX Log "Starting event dispatch thread"
				eventDispatchThread = New EventDispatchThread
				eventDispatchThread.daemon = True
				eventDispatchThread.start()
			End If
		End Sub

		''' <summary>
		''' Return this node's preference/node change listeners.  Even though
		''' we're using a copy-on-write lists, we use synchronized accessors to
		''' ensure information transmission from the writing thread to the
		''' reading thread.
		''' </summary>
		Friend Overridable Function prefListeners() As PreferenceChangeListener()
			SyncLock lock
				Return prefListeners_Renamed
			End SyncLock
		End Function
		Friend Overridable Function nodeListeners() As NodeChangeListener()
			SyncLock lock
				Return nodeListeners_Renamed
			End SyncLock
		End Function

		''' <summary>
		''' Enqueue a preference change event for delivery to registered
		''' preference change listeners unless there are no registered
		''' listeners.  Invoked with this.lock held.
		''' </summary>
		Private Sub enqueuePreferenceChangeEvent(  key As String,   newValue As String)
			If prefListeners_Renamed.Length <> 0 Then
				SyncLock eventQueue
					eventQueue.add(New PreferenceChangeEvent(Me, key, newValue))
					eventQueue.notify()
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Enqueue a "node added" event for delivery to registered node change
		''' listeners unless there are no registered listeners.  Invoked with
		''' this.lock held.
		''' </summary>
		Private Sub enqueueNodeAddedEvent(  child As Preferences)
			If nodeListeners_Renamed.Length <> 0 Then
				SyncLock eventQueue
					eventQueue.add(New NodeAddedEvent(Me, Me, child))
					eventQueue.notify()
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Enqueue a "node removed" event for delivery to registered node change
		''' listeners unless there are no registered listeners.  Invoked with
		''' this.lock held.
		''' </summary>
		Private Sub enqueueNodeRemovedEvent(  child As Preferences)
			If nodeListeners_Renamed.Length <> 0 Then
				SyncLock eventQueue
					eventQueue.add(New NodeRemovedEvent(Me, Me, child))
					eventQueue.notify()
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Implements the <tt>exportNode</tt> method as per the specification in
		''' <seealso cref="Preferences#exportNode(OutputStream)"/>.
		''' </summary>
		''' <param name="os"> the output stream on which to emit the XML document. </param>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="BackingStoreException"> if preference data cannot be read from
		'''         backing store. </exception>
		Public Overridable Sub exportNode(  os As OutputStream)
			XmlSupport.export(os, Me, False)
		End Sub

		''' <summary>
		''' Implements the <tt>exportSubtree</tt> method as per the specification in
		''' <seealso cref="Preferences#exportSubtree(OutputStream)"/>.
		''' </summary>
		''' <param name="os"> the output stream on which to emit the XML document. </param>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="BackingStoreException"> if preference data cannot be read from
		'''         backing store. </exception>
		Public Overridable Sub exportSubtree(  os As OutputStream)
			XmlSupport.export(os, Me, True)
		End Sub
	End Class

End Namespace
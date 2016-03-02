Imports Microsoft.VisualBasic
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
Namespace java.awt


	''' <summary>
	''' An <code>AWTKeyStroke</code> represents a key action on the
	''' keyboard, or equivalent input device. <code>AWTKeyStroke</code>s
	''' can correspond to only a press or release of a
	''' particular key, just as <code>KEY_PRESSED</code> and
	''' <code>KEY_RELEASED</code> <code>KeyEvent</code>s do;
	''' alternately, they can correspond to typing a specific Java character, just
	''' as <code>KEY_TYPED</code> <code>KeyEvent</code>s do.
	''' In all cases, <code>AWTKeyStroke</code>s can specify modifiers
	''' (alt, shift, control, meta, altGraph, or a combination thereof) which must be present
	''' during the action for an exact match.
	''' <p>
	''' <code>AWTKeyStrokes</code> are immutable, and are intended
	''' to be unique. Client code should never create an
	''' <code>AWTKeyStroke</code> on its own, but should instead use
	''' a variant of <code>getAWTKeyStroke</code>. Client use of these factory
	''' methods allows the <code>AWTKeyStroke</code> implementation
	''' to cache and share instances efficiently.
	''' </summary>
	''' <seealso cref= #getAWTKeyStroke
	''' 
	''' @author Arnaud Weber
	''' @author David Mendenhall
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class AWTKeyStroke
		Friend Const serialVersionUID As Long = -6430539691155161871L

		Private Shared modifierKeywords As IDictionary(Of String, Integer?)
		''' <summary>
		''' Associates VK_XXX (as a String) with code (as Integer). This is
		''' done to avoid the overhead of the reflective call to find the
		''' constant.
		''' </summary>
		Private Shared vks As VKCollection

		'A key for the collection of AWTKeyStrokes within AppContext.
		Private Shared APP_CONTEXT_CACHE_KEY As New Object
		'A key withing the cache
		Private Shared APP_CONTEXT_KEYSTROKE_KEY As New AWTKeyStroke

        '    
        '     * Reads keystroke class from AppContext and if null, puts there the
        '     * AWTKeyStroke class.
        '     * Must be called under locked AWTKeyStro
        '     
        Private Property Shared aWTKeyStrokeClass As [Class]
            Get
                Dim clazz As [Class] = CType(sun.awt.AppContext.appContext.get(GetType(AWTKeyStroke)), [Class])
                If clazz Is Nothing Then
                    clazz = GetType(AWTKeyStroke)
                    sun.awt.AppContext.appContext.put(GetType(AWTKeyStroke), GetType(AWTKeyStroke))
                End If
                Return clazz
            End Get
        End Property

        Private keyChar As Char = java.awt.event.KeyEvent.CHAR_UNDEFINED
		Private keyCode As Integer = java.awt.event.KeyEvent.VK_UNDEFINED
		Private modifiers As Integer
		Private onKeyRelease As Boolean

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
		End Sub

		''' <summary>
		''' Constructs an <code>AWTKeyStroke</code> with default values.
		''' The default values used are:
		''' <table border="1" summary="AWTKeyStroke default values">
		''' <tr><th>Property</th><th>Default Value</th></tr>
		''' <tr>
		'''    <td>Key Char</td>
		'''    <td><code>KeyEvent.CHAR_UNDEFINED</code></td>
		''' </tr>
		''' <tr>
		'''    <td>Key Code</td>
		'''    <td><code>KeyEvent.VK_UNDEFINED</code></td>
		''' </tr>
		''' <tr>
		'''    <td>Modifiers</td>
		'''    <td>none</td>
		''' </tr>
		''' <tr>
		'''    <td>On key release?</td>
		'''    <td><code>false</code></td>
		''' </tr>
		''' </table>
		''' 
		''' <code>AWTKeyStroke</code>s should not be constructed
		''' by client code. Use a variant of <code>getAWTKeyStroke</code>
		''' instead.
		''' </summary>
		''' <seealso cref= #getAWTKeyStroke </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>AWTKeyStroke</code> with the specified
		''' values. <code>AWTKeyStroke</code>s should not be constructed
		''' by client code. Use a variant of <code>getAWTKeyStroke</code>
		''' instead.
		''' </summary>
		''' <param name="keyChar"> the character value for a keyboard key </param>
		''' <param name="keyCode"> the key code for this <code>AWTKeyStroke</code> </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <param name="onKeyRelease"> <code>true</code> if this
		'''        <code>AWTKeyStroke</code> corresponds
		'''        to a key release; <code>false</code> otherwise </param>
		''' <seealso cref= #getAWTKeyStroke </seealso>
		Protected Friend Sub New(ByVal keyChar As Char, ByVal keyCode As Integer, ByVal modifiers As Integer, ByVal onKeyRelease As Boolean)
			Me.keyChar = keyChar
			Me.keyCode = keyCode
			Me.modifiers = modifiers
			Me.onKeyRelease = onKeyRelease
		End Sub

		''' <summary>
		''' Registers a new class which the factory methods in
		''' <code>AWTKeyStroke</code> will use when generating new
		''' instances of <code>AWTKeyStroke</code>s. After invoking this
		''' method, the factory methods will return instances of the specified
		''' Class. The specified Class must be either <code>AWTKeyStroke</code>
		''' or derived from <code>AWTKeyStroke</code>, and it must have a
		''' no-arg constructor. The constructor can be of any accessibility,
		''' including <code>private</code>. This operation
		''' flushes the current <code>AWTKeyStroke</code> cache.
		''' </summary>
		''' <param name="subclass"> the new Class of which the factory methods should create
		'''        instances </param>
		''' <exception cref="IllegalArgumentException"> if subclass is <code>null</code>,
		'''         or if subclass does not have a no-arg constructor </exception>
		''' <exception cref="ClassCastException"> if subclass is not
		'''         <code>AWTKeyStroke</code>, or a class derived from
		'''         <code>AWTKeyStroke</code> </exception>
		Protected Friend Shared Sub registerSubclass(ByVal subclass As [Class])
			If subclass Is Nothing Then Throw New IllegalArgumentException("subclass cannot be null")
			SyncLock GetType(AWTKeyStroke)
				Dim keyStrokeClass As  [Class] = CType(sun.awt.AppContext.appContext.get(GetType(AWTKeyStroke)), [Class])
				If keyStrokeClass IsNot Nothing AndAlso keyStrokeClass.Equals(subclass) Then Return
			End SyncLock
			If Not subclass.IsSubclassOf(GetType(AWTKeyStroke)) Then Throw New ClassCastException("subclass is not derived from AWTKeyStroke")

			Dim ctor_Renamed As Constructor = getCtor(subclass)

			Dim couldNotInstantiate As String = "subclass could not be instantiated"

			If ctor_Renamed Is Nothing Then Throw New IllegalArgumentException(couldNotInstantiate)
			Try
				Dim stroke As AWTKeyStroke = CType(ctor_Renamed.newInstance(CType(Nothing, Object())), AWTKeyStroke)
				If stroke Is Nothing Then Throw New IllegalArgumentException(couldNotInstantiate)
			Catch e As NoSuchMethodError
				Throw New IllegalArgumentException(couldNotInstantiate)
			Catch e As ExceptionInInitializerError
				Throw New IllegalArgumentException(couldNotInstantiate)
			Catch e As InstantiationException
				Throw New IllegalArgumentException(couldNotInstantiate)
			Catch e As IllegalAccessException
				Throw New IllegalArgumentException(couldNotInstantiate)
			Catch e As InvocationTargetException
				Throw New IllegalArgumentException(couldNotInstantiate)
			End Try

			SyncLock GetType(AWTKeyStroke)
				sun.awt.AppContext.appContext.put(GetType(AWTKeyStroke), subclass)
				sun.awt.AppContext.appContext.remove(APP_CONTEXT_CACHE_KEY)
				sun.awt.AppContext.appContext.remove(APP_CONTEXT_KEYSTROKE_KEY)
			End SyncLock
		End Sub

	'     returns noarg Constructor for class with accessible flag. No security
	'       threat as accessible flag is set only for this Constructor object,
	'       not for Class constructor.
	'     
		Private Shared Function getCtor(ByVal clazz As [Class]) As Constructor
			Dim ctor_Renamed As Constructor = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Return CType(ctor_Renamed, Constructor)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Constructor
				Try
                    Dim ctor As Constructor = clazz.getDeclaredConstructor(CType(Nothing, [Class]()))
                    If ctor IsNot Nothing Then ctor.accessible = True
					Return ctor
				Catch e As SecurityException
				Catch e As NoSuchMethodException
				End Try
				Return Nothing
			End Function
		End Class

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function getCachedStroke(ByVal keyChar As Char, ByVal keyCode As Integer, ByVal modifiers As Integer, ByVal onKeyRelease As Boolean) As AWTKeyStroke
			Dim cache As IDictionary(Of AWTKeyStroke, AWTKeyStroke) = CType(sun.awt.AppContext.appContext.get(APP_CONTEXT_CACHE_KEY), IDictionary)
			Dim cacheKey As AWTKeyStroke = CType(sun.awt.AppContext.appContext.get(APP_CONTEXT_KEYSTROKE_KEY), AWTKeyStroke)

			If cache Is Nothing Then
				cache = New Dictionary(Of )
				sun.awt.AppContext.appContext.put(APP_CONTEXT_CACHE_KEY, cache)
			End If

			If cacheKey Is Nothing Then
				Try
					Dim clazz As  [Class] = aWTKeyStrokeClass
					cacheKey = CType(getCtor(clazz).newInstance(CType(Nothing, Object())), AWTKeyStroke)
					sun.awt.AppContext.appContext.put(APP_CONTEXT_KEYSTROKE_KEY, cacheKey)
				Catch e As InstantiationException
					assert(False)
				Catch e As IllegalAccessException
					assert(False)
				Catch e As InvocationTargetException
					assert(False)
				End Try
			End If
			cacheKey.keyChar = keyChar
			cacheKey.keyCode = keyCode
			cacheKey.modifiers = mapNewModifiers(mapOldModifiers(modifiers))
			cacheKey.onKeyRelease = onKeyRelease

			Dim stroke As AWTKeyStroke = CType(cache(cacheKey), AWTKeyStroke)
			If stroke Is Nothing Then
				stroke = cacheKey
				cache(stroke) = stroke
				sun.awt.AppContext.appContext.remove(APP_CONTEXT_KEYSTROKE_KEY)
			End If
			Return stroke
		End Function

		''' <summary>
		''' Returns a shared instance of an <code>AWTKeyStroke</code>
		''' that represents a <code>KEY_TYPED</code> event for the
		''' specified character.
		''' </summary>
		''' <param name="keyChar"> the character value for a keyboard key </param>
		''' <returns> an <code>AWTKeyStroke</code> object for that key </returns>
		Public Shared Function getAWTKeyStroke(ByVal keyChar As Char) As AWTKeyStroke
			Return getCachedStroke(keyChar, java.awt.event.KeyEvent.VK_UNDEFINED, 0, False)
		End Function

		''' <summary>
		''' Returns a shared instance of an {@code AWTKeyStroke}
		''' that represents a {@code KEY_TYPED} event for the
		''' specified Character object and a set of modifiers. Note
		''' that the first parameter is of type Character rather than
		''' char. This is to avoid inadvertent clashes with
		''' calls to <code>getAWTKeyStroke(int keyCode, int modifiers)</code>.
		''' 
		''' The modifiers consist of any combination of following:<ul>
		''' <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		''' <li>java.awt.event.InputEvent.META_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		''' </ul>
		''' The old modifiers listed below also can be used, but they are
		''' mapped to _DOWN_ modifiers. <ul>
		''' <li>java.awt.event.InputEvent.SHIFT_MASK
		''' <li>java.awt.event.InputEvent.CTRL_MASK
		''' <li>java.awt.event.InputEvent.META_MASK
		''' <li>java.awt.event.InputEvent.ALT_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		''' </ul>
		''' also can be used, but they are mapped to _DOWN_ modifiers.
		''' 
		''' Since these numbers are all different powers of two, any combination of
		''' them is an integer in which each bit represents a different modifier
		''' key. Use 0 to specify no modifiers.
		''' </summary>
		''' <param name="keyChar"> the Character object for a keyboard character </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <returns> an <code>AWTKeyStroke</code> object for that key </returns>
		''' <exception cref="IllegalArgumentException"> if <code>keyChar</code> is
		'''       <code>null</code>
		''' </exception>
		''' <seealso cref= java.awt.event.InputEvent </seealso>
		Public Shared Function getAWTKeyStroke(ByVal keyChar As Character, ByVal modifiers As Integer) As AWTKeyStroke
			If keyChar Is Nothing Then Throw New IllegalArgumentException("keyChar cannot be null")
			Return getCachedStroke(keyChar, java.awt.event.KeyEvent.VK_UNDEFINED, modifiers, False)
		End Function

		''' <summary>
		''' Returns a shared instance of an <code>AWTKeyStroke</code>,
		''' given a numeric key code and a set of modifiers, specifying
		''' whether the key is activated when it is pressed or released.
		''' <p>
		''' The "virtual key" constants defined in
		''' <code>java.awt.event.KeyEvent</code> can be
		''' used to specify the key code. For example:<ul>
		''' <li><code>java.awt.event.KeyEvent.VK_ENTER</code>
		''' <li><code>java.awt.event.KeyEvent.VK_TAB</code>
		''' <li><code>java.awt.event.KeyEvent.VK_SPACE</code>
		''' </ul>
		''' Alternatively, the key code may be obtained by calling
		''' <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code>.
		''' 
		''' The modifiers consist of any combination of:<ul>
		''' <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		''' <li>java.awt.event.InputEvent.META_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		''' </ul>
		''' The old modifiers <ul>
		''' <li>java.awt.event.InputEvent.SHIFT_MASK
		''' <li>java.awt.event.InputEvent.CTRL_MASK
		''' <li>java.awt.event.InputEvent.META_MASK
		''' <li>java.awt.event.InputEvent.ALT_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		''' </ul>
		''' also can be used, but they are mapped to _DOWN_ modifiers.
		''' 
		''' Since these numbers are all different powers of two, any combination of
		''' them is an integer in which each bit represents a different modifier
		''' key. Use 0 to specify no modifiers.
		''' </summary>
		''' <param name="keyCode"> an int specifying the numeric code for a keyboard key </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <param name="onKeyRelease"> <code>true</code> if the <code>AWTKeyStroke</code>
		'''        should represent a key release; <code>false</code> otherwise </param>
		''' <returns> an AWTKeyStroke object for that key
		''' </returns>
		''' <seealso cref= java.awt.event.KeyEvent </seealso>
		''' <seealso cref= java.awt.event.InputEvent </seealso>
		Public Shared Function getAWTKeyStroke(ByVal keyCode As Integer, ByVal modifiers As Integer, ByVal onKeyRelease As Boolean) As AWTKeyStroke
			Return getCachedStroke(java.awt.event.KeyEvent.CHAR_UNDEFINED, keyCode, modifiers, onKeyRelease)
		End Function

		''' <summary>
		''' Returns a shared instance of an <code>AWTKeyStroke</code>,
		''' given a numeric key code and a set of modifiers. The returned
		''' <code>AWTKeyStroke</code> will correspond to a key press.
		''' <p>
		''' The "virtual key" constants defined in
		''' <code>java.awt.event.KeyEvent</code> can be
		''' used to specify the key code. For example:<ul>
		''' <li><code>java.awt.event.KeyEvent.VK_ENTER</code>
		''' <li><code>java.awt.event.KeyEvent.VK_TAB</code>
		''' <li><code>java.awt.event.KeyEvent.VK_SPACE</code>
		''' </ul>
		''' The modifiers consist of any combination of:<ul>
		''' <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		''' <li>java.awt.event.InputEvent.META_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		''' </ul>
		''' The old modifiers <ul>
		''' <li>java.awt.event.InputEvent.SHIFT_MASK
		''' <li>java.awt.event.InputEvent.CTRL_MASK
		''' <li>java.awt.event.InputEvent.META_MASK
		''' <li>java.awt.event.InputEvent.ALT_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		''' </ul>
		''' also can be used, but they are mapped to _DOWN_ modifiers.
		''' 
		''' Since these numbers are all different powers of two, any combination of
		''' them is an integer in which each bit represents a different modifier
		''' key. Use 0 to specify no modifiers.
		''' </summary>
		''' <param name="keyCode"> an int specifying the numeric code for a keyboard key </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <returns> an <code>AWTKeyStroke</code> object for that key
		''' </returns>
		''' <seealso cref= java.awt.event.KeyEvent </seealso>
		''' <seealso cref= java.awt.event.InputEvent </seealso>
		Public Shared Function getAWTKeyStroke(ByVal keyCode As Integer, ByVal modifiers As Integer) As AWTKeyStroke
			Return getCachedStroke(java.awt.event.KeyEvent.CHAR_UNDEFINED, keyCode, modifiers, False)
		End Function

		''' <summary>
		''' Returns an <code>AWTKeyStroke</code> which represents the
		''' stroke which generated a given <code>KeyEvent</code>.
		''' <p>
		''' This method obtains the keyChar from a <code>KeyTyped</code>
		''' event, and the keyCode from a <code>KeyPressed</code> or
		''' <code>KeyReleased</code> event. The <code>KeyEvent</code> modifiers are
		''' obtained for all three types of <code>KeyEvent</code>.
		''' </summary>
		''' <param name="anEvent"> the <code>KeyEvent</code> from which to
		'''      obtain the <code>AWTKeyStroke</code> </param>
		''' <exception cref="NullPointerException"> if <code>anEvent</code> is null </exception>
		''' <returns> the <code>AWTKeyStroke</code> that precipitated the event </returns>
		Public Shared Function getAWTKeyStrokeForEvent(ByVal anEvent As java.awt.event.KeyEvent) As AWTKeyStroke
			Dim id As Integer = anEvent.iD
			Select Case id
			  Case java.awt.event.KeyEvent.KEY_PRESSED, KeyEvent.KEY_RELEASED
				Return getCachedStroke(java.awt.event.KeyEvent.CHAR_UNDEFINED, anEvent.keyCode, anEvent.modifiers, (id = java.awt.event.KeyEvent.KEY_RELEASED))
			  Case java.awt.event.KeyEvent.KEY_TYPED
				Return getCachedStroke(anEvent.keyChar, java.awt.event.KeyEvent.VK_UNDEFINED, anEvent.modifiers, False)
			  Case Else
				' Invalid ID for this KeyEvent
				Return Nothing
			End Select
		End Function

		''' <summary>
		''' Parses a string and returns an <code>AWTKeyStroke</code>.
		''' The string must have the following syntax:
		''' <pre>
		'''    &lt;modifiers&gt;* (&lt;typedID&gt; | &lt;pressedReleasedID&gt;)
		''' 
		'''    modifiers := shift | control | ctrl | meta | alt | altGraph
		'''    typedID := typed &lt;typedKey&gt;
		'''    typedKey := string of length 1 giving Unicode character.
		'''    pressedReleasedID := (pressed | released) key
		'''    key := KeyEvent key code name, i.e. the name following "VK_".
		''' </pre>
		''' If typed, pressed or released is not specified, pressed is assumed. Here
		''' are some examples:
		''' <pre>
		'''     "INSERT" =&gt; getAWTKeyStroke(KeyEvent.VK_INSERT, 0);
		'''     "control DELETE" =&gt; getAWTKeyStroke(KeyEvent.VK_DELETE, InputEvent.CTRL_MASK);
		'''     "alt shift X" =&gt; getAWTKeyStroke(KeyEvent.VK_X, InputEvent.ALT_MASK | InputEvent.SHIFT_MASK);
		'''     "alt shift released X" =&gt; getAWTKeyStroke(KeyEvent.VK_X, InputEvent.ALT_MASK | InputEvent.SHIFT_MASK, true);
		'''     "typed a" =&gt; getAWTKeyStroke('a');
		''' </pre>
		''' </summary>
		''' <param name="s"> a String formatted as described above </param>
		''' <returns> an <code>AWTKeyStroke</code> object for that String </returns>
		''' <exception cref="IllegalArgumentException"> if <code>s</code> is <code>null</code>,
		'''        or is formatted incorrectly </exception>
		Public Shared Function getAWTKeyStroke(ByVal s As String) As AWTKeyStroke
			If s Is Nothing Then Throw New IllegalArgumentException("String cannot be null")

			Const errmsg As String = "String formatted incorrectly"

			Dim st As New java.util.StringTokenizer(s, " ")

			Dim mask As Integer = 0
			Dim released As Boolean = False
			Dim typed As Boolean = False
			Dim pressed As Boolean = False

			SyncLock GetType(AWTKeyStroke)
				If modifierKeywords Is Nothing Then
					Dim uninitializedMap As IDictionary(Of String, Integer?) = New Dictionary(Of String, Integer?)(8, 1.0f)
					uninitializedMap("shift") = Convert.ToInt32(java.awt.event.InputEvent.SHIFT_DOWN_MASK Or java.awt.event.InputEvent.SHIFT_MASK)
					uninitializedMap("control") = Convert.ToInt32(java.awt.event.InputEvent.CTRL_DOWN_MASK Or java.awt.event.InputEvent.CTRL_MASK)
					uninitializedMap("ctrl") = Convert.ToInt32(java.awt.event.InputEvent.CTRL_DOWN_MASK Or java.awt.event.InputEvent.CTRL_MASK)
					uninitializedMap("meta") = Convert.ToInt32(java.awt.event.InputEvent.META_DOWN_MASK Or java.awt.event.InputEvent.META_MASK)
					uninitializedMap("alt") = Convert.ToInt32(java.awt.event.InputEvent.ALT_DOWN_MASK Or java.awt.event.InputEvent.ALT_MASK)
					uninitializedMap("altGraph") = Convert.ToInt32(java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK Or java.awt.event.InputEvent.ALT_GRAPH_MASK)
					uninitializedMap("button1") = Convert.ToInt32(java.awt.event.InputEvent.BUTTON1_DOWN_MASK)
					uninitializedMap("button2") = Convert.ToInt32(java.awt.event.InputEvent.BUTTON2_DOWN_MASK)
					uninitializedMap("button3") = Convert.ToInt32(java.awt.event.InputEvent.BUTTON3_DOWN_MASK)
					modifierKeywords = java.util.Collections.synchronizedMap(uninitializedMap)
				End If
			End SyncLock

			Dim count As Integer = st.countTokens()

			For i As Integer = 1 To count
				Dim token As String = st.nextToken()

				If typed Then
					If token.length() <> 1 OrElse i <> count Then Throw New IllegalArgumentException(errmsg)
					Return getCachedStroke(token.Chars(0), java.awt.event.KeyEvent.VK_UNDEFINED, mask, False)
				End If

				If pressed OrElse released OrElse i = count Then
					If i <> count Then Throw New IllegalArgumentException(errmsg)

					Dim keyCodeName As String = "VK_" & token
					Dim keyCode_Renamed As Integer = getVKValue(keyCodeName)

					Return getCachedStroke(java.awt.event.KeyEvent.CHAR_UNDEFINED, keyCode_Renamed, mask, released)
				End If

				If token.Equals("released") Then
					released = True
					Continue For
				End If
				If token.Equals("pressed") Then
					pressed = True
					Continue For
				End If
				If token.Equals("typed") Then
					typed = True
					Continue For
				End If

				Dim tokenMask As Integer? = CInt(Fix(modifierKeywords(token)))
				If tokenMask IsNot Nothing Then
					mask = mask Or tokenMask
				Else
					Throw New IllegalArgumentException(errmsg)
				End If
			Next i

			Throw New IllegalArgumentException(errmsg)
		End Function

		Private Property Shared vKCollection As VKCollection
			Get
				If vks Is Nothing Then vks = New VKCollection
				Return vks
			End Get
		End Property
		''' <summary>
		''' Returns the integer constant for the KeyEvent.VK field named
		''' <code>key</code>. This will throw an
		''' <code>IllegalArgumentException</code> if <code>key</code> is
		''' not a valid constant.
		''' </summary>
		Private Shared Function getVKValue(ByVal key As String) As Integer
			Dim vkCollect As VKCollection = vKCollection

			Dim value As Integer? = vkCollect.findCode(key)

			If value Is Nothing Then
				Dim keyCode_Renamed As Integer = 0
				Const errmsg As String = "String formatted incorrectly"

				Try
					keyCode_Renamed = GetType(java.awt.event.KeyEvent).getField(key).getInt(GetType(java.awt.event.KeyEvent))
				Catch nsfe As NoSuchFieldException
					Throw New IllegalArgumentException(errmsg)
				Catch iae As IllegalAccessException
					Throw New IllegalArgumentException(errmsg)
				End Try
				value = Convert.ToInt32(keyCode_Renamed)
				vkCollect.put(key, value)
			End If
			Return value
		End Function

		''' <summary>
		''' Returns the character for this <code>AWTKeyStroke</code>.
		''' </summary>
		''' <returns> a char value </returns>
		''' <seealso cref= #getAWTKeyStroke(char) </seealso>
		''' <seealso cref= KeyEvent#getKeyChar </seealso>
		Public Property keyChar As Char
			Get
				Return keyChar
			End Get
		End Property

		''' <summary>
		''' Returns the numeric key code for this <code>AWTKeyStroke</code>.
		''' </summary>
		''' <returns> an int containing the key code value </returns>
		''' <seealso cref= #getAWTKeyStroke(int,int) </seealso>
		''' <seealso cref= KeyEvent#getKeyCode </seealso>
		Public Property keyCode As Integer
			Get
				Return keyCode
			End Get
		End Property

		''' <summary>
		''' Returns the modifier keys for this <code>AWTKeyStroke</code>.
		''' </summary>
		''' <returns> an int containing the modifiers </returns>
		''' <seealso cref= #getAWTKeyStroke(int,int) </seealso>
		Public Property modifiers As Integer
			Get
				Return modifiers
			End Get
		End Property

		''' <summary>
		''' Returns whether this <code>AWTKeyStroke</code> represents a key release.
		''' </summary>
		''' <returns> <code>true</code> if this <code>AWTKeyStroke</code>
		'''          represents a key release; <code>false</code> otherwise </returns>
		''' <seealso cref= #getAWTKeyStroke(int,int,boolean) </seealso>
		Public Property onKeyRelease As Boolean
			Get
				Return onKeyRelease
			End Get
		End Property

		''' <summary>
		''' Returns the type of <code>KeyEvent</code> which corresponds to
		''' this <code>AWTKeyStroke</code>.
		''' </summary>
		''' <returns> <code>KeyEvent.KEY_PRESSED</code>,
		'''         <code>KeyEvent.KEY_TYPED</code>,
		'''         or <code>KeyEvent.KEY_RELEASED</code> </returns>
		''' <seealso cref= java.awt.event.KeyEvent </seealso>
		Public Property keyEventType As Integer
			Get
				If keyCode = java.awt.event.KeyEvent.VK_UNDEFINED Then
					Return java.awt.event.KeyEvent.KEY_TYPED
				Else
					Return If(onKeyRelease, java.awt.event.KeyEvent.KEY_RELEASED, java.awt.event.KeyEvent.KEY_PRESSED)
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a numeric value for this object that is likely to be unique,
		''' making it a good choice as the index value in a hash table.
		''' </summary>
		''' <returns> an int that represents this object </returns>
		Public Overrides Function GetHashCode() As Integer
			Return ((AscW(keyChar)) + 1) * (2 * (keyCode + 1)) * (modifiers + 1) + (If(onKeyRelease, 1, 2))
		End Function

		''' <summary>
		''' Returns true if this object is identical to the specified object.
		''' </summary>
		''' <param name="anObject"> the Object to compare this object to </param>
		''' <returns> true if the objects are identical </returns>
		Public NotOverridable Overrides Function Equals(ByVal anObject As Object) As Boolean
			If TypeOf anObject Is AWTKeyStroke Then
				Dim ks As AWTKeyStroke = CType(anObject, AWTKeyStroke)
				Return (ks.keyChar = keyChar AndAlso ks.keyCode = keyCode AndAlso ks.onKeyRelease = onKeyRelease AndAlso ks.modifiers = modifiers)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a string that displays and identifies this object's properties.
		''' The <code>String</code> returned by this method can be passed
		''' as a parameter to <code>getAWTKeyStroke(String)</code> to produce
		''' a key stroke equal to this key stroke.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		''' <seealso cref= #getAWTKeyStroke(String) </seealso>
		Public Overrides Function ToString() As String
			If keyCode = java.awt.event.KeyEvent.VK_UNDEFINED Then
				Return getModifiersText(modifiers) & "typed " & AscW(keyChar)
			Else
				Return getModifiersText(modifiers) + (If(onKeyRelease, "released", "pressed")) & " " & getVKText(keyCode)
			End If
		End Function

		Friend Shared Function getModifiersText(ByVal modifiers As Integer) As String
			Dim buf As New StringBuilder

			If (modifiers And java.awt.event.InputEvent.SHIFT_DOWN_MASK) <> 0 Then buf.append("shift ")
			If (modifiers And java.awt.event.InputEvent.CTRL_DOWN_MASK) <> 0 Then buf.append("ctrl ")
			If (modifiers And java.awt.event.InputEvent.META_DOWN_MASK) <> 0 Then buf.append("meta ")
			If (modifiers And java.awt.event.InputEvent.ALT_DOWN_MASK) <> 0 Then buf.append("alt ")
			If (modifiers And java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK) <> 0 Then buf.append("altGraph ")
			If (modifiers And java.awt.event.InputEvent.BUTTON1_DOWN_MASK) <> 0 Then buf.append("button1 ")
			If (modifiers And java.awt.event.InputEvent.BUTTON2_DOWN_MASK) <> 0 Then buf.append("button2 ")
			If (modifiers And java.awt.event.InputEvent.BUTTON3_DOWN_MASK) <> 0 Then buf.append("button3 ")

			Return buf.ToString()
		End Function

		Friend Shared Function getVKText(ByVal keyCode As Integer) As String
			Dim vkCollect As VKCollection = vKCollection
			Dim key As Integer? = Convert.ToInt32(keyCode)
			Dim name As String = vkCollect.findName(key)
			If name IsNot Nothing Then Return name.Substring(3)
			Dim expected_modifiers As Integer = (Modifier.PUBLIC Or Modifier.STATIC Or Modifier.FINAL)

			Dim fields As Field() = GetType(java.awt.event.KeyEvent).declaredFields
			For i As Integer = 0 To fields.Length - 1
				Try
					If fields(i).modifiers = expected_modifiers AndAlso fields(i).type =  java.lang.[Integer].TYPE AndAlso fields(i).name.StartsWith("VK_") AndAlso fields(i).getInt(GetType(java.awt.event.KeyEvent)) = keyCode Then
						name = fields(i).name
						vkCollect.put(name, key)
						Return name.Substring(3)
					End If
				Catch e As IllegalAccessException
					assert(False)
				End Try
			Next i
			Return "UNKNOWN"
		End Function

		''' <summary>
		''' Returns a cached instance of <code>AWTKeyStroke</code> (or a subclass of
		''' <code>AWTKeyStroke</code>) which is equal to this instance.
		''' </summary>
		''' <returns> a cached instance which is equal to this instance </returns>
		Protected Friend Overridable Function readResolve() As Object
			SyncLock GetType(AWTKeyStroke)
				If Me.GetType().Equals(aWTKeyStrokeClass) Then Return getCachedStroke(keyChar, keyCode, modifiers, onKeyRelease)
			End SyncLock
			Return Me
		End Function

		Private Shared Function mapOldModifiers(ByVal modifiers As Integer) As Integer
			If (modifiers And java.awt.event.InputEvent.SHIFT_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.SHIFT_DOWN_MASK
			If (modifiers And java.awt.event.InputEvent.ALT_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.ALT_DOWN_MASK
			If (modifiers And java.awt.event.InputEvent.ALT_GRAPH_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
			If (modifiers And java.awt.event.InputEvent.CTRL_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.CTRL_DOWN_MASK
			If (modifiers And java.awt.event.InputEvent.META_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.META_DOWN_MASK

			modifiers = modifiers And java.awt.event.InputEvent.SHIFT_DOWN_MASK Or java.awt.event.InputEvent.ALT_DOWN_MASK Or java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK Or java.awt.event.InputEvent.CTRL_DOWN_MASK Or java.awt.event.InputEvent.META_DOWN_MASK Or java.awt.event.InputEvent.BUTTON1_DOWN_MASK Or java.awt.event.InputEvent.BUTTON2_DOWN_MASK Or java.awt.event.InputEvent.BUTTON3_DOWN_MASK

			Return modifiers
		End Function

		Private Shared Function mapNewModifiers(ByVal modifiers As Integer) As Integer
			If (modifiers And java.awt.event.InputEvent.SHIFT_DOWN_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.SHIFT_MASK
			If (modifiers And java.awt.event.InputEvent.ALT_DOWN_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.ALT_MASK
			If (modifiers And java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.ALT_GRAPH_MASK
			If (modifiers And java.awt.event.InputEvent.CTRL_DOWN_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.CTRL_MASK
			If (modifiers And java.awt.event.InputEvent.META_DOWN_MASK) <> 0 Then modifiers = modifiers Or java.awt.event.InputEvent.META_MASK

			Return modifiers
		End Function

	End Class

	Friend Class VKCollection
		Friend code2name As IDictionary(Of Integer?, String)
		Friend name2code As IDictionary(Of String, Integer?)

		Public Sub New()
			code2name = New Dictionary(Of )
			name2code = New Dictionary(Of )
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub put(ByVal name As String, ByVal code As Integer?)
			assert((name IsNot Nothing) AndAlso (code IsNot Nothing))
			assert(findName(code) Is Nothing)
			assert(findCode(name) Is Nothing)
			code2name(code) = name
			name2code(name) = code
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function findCode(ByVal name As String) As Integer?
			assert(name IsNot Nothing)
			Return CInt(Fix(name2code(name)))
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function findName(ByVal code As Integer?) As String
			assert(code IsNot Nothing)
			Return CStr(code2name(code))
		End Function
	End Class

End Namespace
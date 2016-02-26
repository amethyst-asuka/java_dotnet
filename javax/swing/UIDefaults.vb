Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.border

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing





	''' <summary>
	''' A table of defaults for Swing components.  Applications can set/get
	''' default values via the <code>UIManager</code>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= UIManager
	''' @author Hans Muller </seealso>
	Public Class UIDefaults
		Inherits Dictionary(Of Object, Object)

		Private Shared ReadOnly PENDING As New Object

		Private changeSupport As javax.swing.event.SwingPropertyChangeSupport

		Private resourceBundles As List(Of String)

		Private defaultLocale As java.util.Locale = java.util.Locale.default

		''' <summary>
		''' Maps from a Locale to a cached Map of the ResourceBundle. This is done
		''' so as to avoid an exception being thrown when a value is asked for.
		''' Access to this should be done while holding a lock on the
		''' UIDefaults, eg synchronized(this).
		''' </summary>
		Private resourceCache As IDictionary(Of java.util.Locale, IDictionary(Of String, Object))

		''' <summary>
		''' Creates an empty defaults table.
		''' </summary>
		Public Sub New()
			Me.New(700,.75f)
		End Sub

		''' <summary>
		''' Creates an empty defaults table with the specified initial capacity and
		''' load factor.
		''' </summary>
		''' <param name="initialCapacity">   the initial capacity of the defaults table </param>
		''' <param name="loadFactor">        the load factor of the defaults table </param>
		''' <seealso cref= java.util.Hashtable
		''' @since 1.6 </seealso>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			MyBase.New(initialCapacity, loadFactor)
			resourceCache = New Dictionary(Of java.util.Locale, IDictionary(Of String, Object))
		End Sub


		''' <summary>
		''' Creates a defaults table initialized with the specified
		''' key/value pairs.  For example:
		''' <pre>
		'''    Object[] uiDefaults = {
		'''         "Font", new Font("Dialog", Font.BOLD, 12),
		'''        "Color", Color.red,
		'''         "five", new Integer(5)
		'''    }
		'''    UIDefaults myDefaults = new UIDefaults(uiDefaults);
		''' </pre> </summary>
		''' <param name="keyValueList">  an array of objects containing the key/value
		'''          pairs </param>
		Public Sub New(ByVal keyValueList As Object())
			MyBase.New(keyValueList.Length \ 2)
			For i As Integer = 0 To keyValueList.Length - 1 Step 2
				MyBase.put(keyValueList(i), keyValueList(i + 1))
			Next i
		End Sub

		''' <summary>
		''' Returns the value for key.  If the value is a
		''' <code>UIDefaults.LazyValue</code> then the real
		''' value is computed with <code>LazyValue.createValue()</code>,
		''' the table entry is replaced, and the real value is returned.
		''' If the value is an <code>UIDefaults.ActiveValue</code>
		''' the table entry is not replaced - the value is computed
		''' with <code>ActiveValue.createValue()</code> for each
		''' <code>get()</code> call.
		''' 
		''' If the key is not found in the table then it is searched for in the list
		''' of resource bundles maintained by this object.  The resource bundles are
		''' searched most recently added first using the locale returned by
		''' <code>getDefaultLocale</code>.  <code>LazyValues</code> and
		''' <code>ActiveValues</code> are not supported in the resource bundles.
		''' 
		''' </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> the value for <code>key</code> </returns>
		''' <seealso cref= LazyValue </seealso>
		''' <seealso cref= ActiveValue </seealso>
		''' <seealso cref= java.util.Hashtable#get </seealso>
		''' <seealso cref= #getDefaultLocale </seealso>
		''' <seealso cref= #addResourceBundle
		''' @since 1.4 </seealso>
		Public Overridable Function [get](ByVal key As Object) As Object
			Dim value As Object = getFromHashtable(key)
			Return If(value IsNot Nothing, value, getFromResourceBundle(key, Nothing))
		End Function

		''' <summary>
		''' Looks up up the given key in our Hashtable and resolves LazyValues
		''' or ActiveValues.
		''' </summary>
		Private Function getFromHashtable(ByVal key As Object) As Object
	'         Quickly handle the common case, without grabbing
	'         * a lock.
	'         
			Dim value As Object = MyBase.get(key)
			If (value IsNot PENDING) AndAlso Not(TypeOf value Is ActiveValue) AndAlso Not(TypeOf value Is LazyValue) Then Return value

	'         If the LazyValue for key is being constructed by another
	'         * thread then wait and then return the new value, otherwise drop
	'         * the lock and construct the ActiveValue or the LazyValue.
	'         * We use the special value PENDING to mark LazyValues that
	'         * are being constructed.
	'         
			SyncLock Me
				value = MyBase.get(key)
				If value Is PENDING Then
					Do
						Try
							Me.wait()
						Catch e As InterruptedException
						End Try
						value = MyBase.get(key)
					Loop While value Is PENDING
					Return value
				ElseIf TypeOf value Is LazyValue Then
					MyBase.put(key, PENDING)
				ElseIf Not(TypeOf value Is ActiveValue) Then
					Return value
				End If
			End SyncLock

	'         At this point we know that the value of key was
	'         * a LazyValue or an ActiveValue.
	'         
			If TypeOf value Is LazyValue Then
				Try
	'                 If an exception is thrown we'll just put the LazyValue
	'                 * back in the table.
	'                 
					value = CType(value, LazyValue).createValue(Me)
				Finally
					SyncLock Me
						If value Is Nothing Then
							MyBase.remove(key)
						Else
							MyBase.put(key, value)
						End If
						Me.notifyAll()
					End SyncLock
				End Try
			Else
				value = CType(value, ActiveValue).createValue(Me)
			End If

			Return value
		End Function


		''' <summary>
		''' Returns the value for key associated with the given locale.
		''' If the value is a <code>UIDefaults.LazyValue</code> then the real
		''' value is computed with <code>LazyValue.createValue()</code>,
		''' the table entry is replaced, and the real value is returned.
		''' If the value is an <code>UIDefaults.ActiveValue</code>
		''' the table entry is not replaced - the value is computed
		''' with <code>ActiveValue.createValue()</code> for each
		''' <code>get()</code> call.
		''' 
		''' If the key is not found in the table then it is searched for in the list
		''' of resource bundles maintained by this object.  The resource bundles are
		''' searched most recently added first using the given locale.
		''' <code>LazyValues</code> and <code>ActiveValues</code> are not supported
		''' in the resource bundles.
		''' </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired <code>locale</code> </param>
		''' <returns> the value for <code>key</code> </returns>
		''' <seealso cref= LazyValue </seealso>
		''' <seealso cref= ActiveValue </seealso>
		''' <seealso cref= java.util.Hashtable#get </seealso>
		''' <seealso cref= #addResourceBundle
		''' @since 1.4 </seealso>
		Public Overridable Function [get](ByVal key As Object, ByVal l As java.util.Locale) As Object
			Dim value As Object = getFromHashtable(key)
			Return If(value IsNot Nothing, value, getFromResourceBundle(key, l))
		End Function

		''' <summary>
		''' Looks up given key in our resource bundles.
		''' </summary>
		Private Function getFromResourceBundle(ByVal key As Object, ByVal l As java.util.Locale) As Object

			If resourceBundles Is Nothing OrElse resourceBundles.Count = 0 OrElse Not(TypeOf key Is String) Then Return Nothing

			' A null locale means use the default locale.
			If l Is Nothing Then
				If defaultLocale Is Nothing Then
					Return Nothing
				Else
					l = defaultLocale
				End If
			End If

			SyncLock Me
				Return getResourceCache(l)(key)
			End SyncLock
		End Function

		''' <summary>
		''' Returns a Map of the known resources for the given locale.
		''' </summary>
		Private Function getResourceCache(ByVal l As java.util.Locale) As IDictionary(Of String, Object)
			Dim values As IDictionary(Of String, Object) = resourceCache(l)

			If values Is Nothing Then
				values = New TextAndMnemonicHashMap
				For i As Integer = resourceBundles.Count-1 To 0 Step -1
					Dim bundleName As String = resourceBundles(i)
					Try
						Dim c As java.util.ResourceBundle.Control = sun.util.CoreResourceBundleControl.getRBControlInstance(bundleName)
						Dim b As java.util.ResourceBundle
						If c IsNot Nothing Then
							b = java.util.ResourceBundle.getBundle(bundleName, l, c)
						Else
							b = java.util.ResourceBundle.getBundle(bundleName, l)
						End If
						Dim keys As System.Collections.IEnumerator = b.keys

						Do While keys.hasMoreElements()
							Dim key As String = CStr(keys.nextElement())

							If values(key) Is Nothing Then
								Dim value As Object = b.getObject(key)

								values(key) = value
							End If
						Loop
					Catch mre As java.util.MissingResourceException
						' Keep looking
					End Try
				Next i
				resourceCache(l) = values
			End If
			Return values
		End Function

		''' <summary>
		''' Sets the value of <code>key</code> to <code>value</code> for all locales.
		''' If <code>key</code> is a string and the new value isn't
		''' equal to the old one, fire a <code>PropertyChangeEvent</code>.
		''' If value is <code>null</code>, the key is removed from the table.
		''' </summary>
		''' <param name="key">    the unique <code>Object</code> who's value will be used
		'''          to retrieve the data value associated with it </param>
		''' <param name="value">  the new <code>Object</code> to store as data under
		'''          that key </param>
		''' <returns> the previous <code>Object</code> value, or <code>null</code> </returns>
		''' <seealso cref= #putDefaults </seealso>
		''' <seealso cref= java.util.Hashtable#put </seealso>
		Public Overridable Function put(ByVal key As Object, ByVal value As Object) As Object
			Dim oldValue As Object = If(value Is Nothing, MyBase.remove(key), MyBase.put(key, value))
			If TypeOf key Is String Then firePropertyChange(CStr(key), oldValue, value)
			Return oldValue
		End Function


		''' <summary>
		''' Puts all of the key/value pairs in the database and
		''' unconditionally generates one <code>PropertyChangeEvent</code>.
		''' The events oldValue and newValue will be <code>null</code> and its
		''' <code>propertyName</code> will be "UIDefaults".  The key/value pairs are
		''' added for all locales.
		''' </summary>
		''' <param name="keyValueList">  an array of key/value pairs </param>
		''' <seealso cref= #put </seealso>
		''' <seealso cref= java.util.Hashtable#put </seealso>
		Public Overridable Sub putDefaults(ByVal keyValueList As Object())
			Dim i As Integer = 0
			Dim max As Integer = keyValueList.Length
			Do While i < max
				Dim value As Object = keyValueList(i + 1)
				If value Is Nothing Then
					MyBase.remove(keyValueList(i))
				Else
					MyBase.put(keyValueList(i), value)
				End If
				i += 2
			Loop
			firePropertyChange("UIDefaults", Nothing, Nothing)
		End Sub


		''' <summary>
		''' If the value of <code>key</code> is a <code>Font</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is a <code>Font</code>,
		'''          return the <code>Font</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getFont(ByVal key As Object) As java.awt.Font
			Dim value As Object = [get](key)
			Return If(TypeOf value Is java.awt.Font, CType(value, java.awt.Font), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is a <code>Font</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is a <code>Font</code>,
		'''          return the <code>Font</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getFont(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Font
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is java.awt.Font, CType(value, java.awt.Font), Nothing)
		End Function

		''' <summary>
		''' If the value of <code>key</code> is a <code>Color</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is a <code>Color</code>,
		'''          return the <code>Color</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getColor(ByVal key As Object) As java.awt.Color
			Dim value As Object = [get](key)
			Return If(TypeOf value Is java.awt.Color, CType(value, java.awt.Color), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is a <code>Color</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is a <code>Color</code>,
		'''          return the <code>Color</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getColor(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Color
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is java.awt.Color, CType(value, java.awt.Color), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> is an <code>Icon</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is an <code>Icon</code>,
		'''          return the <code>Icon</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getIcon(ByVal key As Object) As Icon
			Dim value As Object = [get](key)
			Return If(TypeOf value Is Icon, CType(value, Icon), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is an <code>Icon</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is an <code>Icon</code>,
		'''          return the <code>Icon</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getIcon(ByVal key As Object, ByVal l As java.util.Locale) As Icon
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is Icon, CType(value, Icon), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> is a <code>Border</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is a <code>Border</code>,
		'''          return the <code>Border</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getBorder(ByVal key As Object) As Border
			Dim value As Object = [get](key)
			Return If(TypeOf value Is Border, CType(value, Border), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is a <code>Border</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is a <code>Border</code>,
		'''          return the <code>Border</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getBorder(ByVal key As Object, ByVal l As java.util.Locale) As Border
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is Border, CType(value, Border), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> is a <code>String</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is a <code>String</code>,
		'''          return the <code>String</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getString(ByVal key As Object) As String
			Dim value As Object = [get](key)
			Return If(TypeOf value Is String, CStr(value), Nothing)
		End Function

		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is a <code>String</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired <code>Locale</code> </param>
		''' <returns> if the value for <code>key</code> for the given
		'''          <code>Locale</code> is a <code>String</code>,
		'''          return the <code>String</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getString(ByVal key As Object, ByVal l As java.util.Locale) As String
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is String, CStr(value), Nothing)
		End Function

		''' <summary>
		''' If the value of <code>key</code> is an <code>Integer</code> return its
		''' integer value, otherwise return 0. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is an <code>Integer</code>,
		'''          return its value, otherwise return 0 </returns>
		Public Overridable Function getInt(ByVal key As Object) As Integer
			Dim value As Object = [get](key)
			Return If(TypeOf value Is Integer?, CInt(Fix(value)), 0)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is an <code>Integer</code> return its integer value, otherwise return 0. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is an <code>Integer</code>,
		'''          return its value, otherwise return 0
		''' @since 1.4 </returns>
		Public Overridable Function getInt(ByVal key As Object, ByVal l As java.util.Locale) As Integer
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is Integer?, CInt(Fix(value)), 0)
		End Function


		''' <summary>
		''' If the value of <code>key</code> is boolean, return the
		''' boolean value, otherwise return false.
		''' </summary>
		''' <param name="key"> an <code>Object</code> specifying the key for the desired boolean value </param>
		''' <returns> if the value of <code>key</code> is boolean, return the
		'''         boolean value, otherwise return false.
		''' @since 1.4 </returns>
		Public Overridable Function getBoolean(ByVal key As Object) As Boolean
			Dim value As Object = [get](key)
			Return If(TypeOf value Is Boolean?, CBool(value), False)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is boolean, return the boolean value, otherwise return false.
		''' </summary>
		''' <param name="key"> an <code>Object</code> specifying the key for the desired boolean value </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''         is boolean, return the
		'''         boolean value, otherwise return false.
		''' @since 1.4 </returns>
		Public Overridable Function getBoolean(ByVal key As Object, ByVal l As java.util.Locale) As Boolean
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is Boolean?, CBool(value), False)
		End Function


		''' <summary>
		''' If the value of <code>key</code> is an <code>Insets</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is an <code>Insets</code>,
		'''          return the <code>Insets</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getInsets(ByVal key As Object) As java.awt.Insets
			Dim value As Object = [get](key)
			Return If(TypeOf value Is java.awt.Insets, CType(value, java.awt.Insets), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is an <code>Insets</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is an <code>Insets</code>,
		'''          return the <code>Insets</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getInsets(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Insets
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is java.awt.Insets, CType(value, java.awt.Insets), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> is a <code>Dimension</code> return it,
		''' otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <returns> if the value for <code>key</code> is a <code>Dimension</code>,
		'''          return the <code>Dimension</code> object; otherwise return
		'''          <code>null</code> </returns>
		Public Overridable Function getDimension(ByVal key As Object) As java.awt.Dimension
			Dim value As Object = [get](key)
			Return If(TypeOf value Is java.awt.Dimension, CType(value, java.awt.Dimension), Nothing)
		End Function


		''' <summary>
		''' If the value of <code>key</code> for the given <code>Locale</code>
		''' is a <code>Dimension</code> return it, otherwise return <code>null</code>. </summary>
		''' <param name="key"> the desired key </param>
		''' <param name="l"> the desired locale </param>
		''' <returns> if the value for <code>key</code> and <code>Locale</code>
		'''          is a <code>Dimension</code>,
		'''          return the <code>Dimension</code> object; otherwise return
		'''          <code>null</code>
		''' @since 1.4 </returns>
		Public Overridable Function getDimension(ByVal key As Object, ByVal l As java.util.Locale) As java.awt.Dimension
			Dim value As Object = [get](key,l)
			Return If(TypeOf value Is java.awt.Dimension, CType(value, java.awt.Dimension), Nothing)
		End Function


		''' <summary>
		''' The value of <code>get(uidClassID)</code> must be the
		''' <code>String</code> name of a
		''' class that implements the corresponding <code>ComponentUI</code>
		''' class.  If the class hasn't been loaded before, this method looks
		''' up the class with <code>uiClassLoader.loadClass()</code> if a non
		''' <code>null</code>
		''' class loader is provided, <code>classForName()</code> otherwise.
		''' <p>
		''' If a mapping for <code>uiClassID</code> exists or if the specified
		''' class can't be found, return <code>null</code>.
		''' <p>
		''' This method is used by <code>getUI</code>, it's usually
		''' not necessary to call it directly.
		''' </summary>
		''' <param name="uiClassID">  a string containing the class ID </param>
		''' <param name="uiClassLoader"> the object which will load the class </param>
		''' <returns> the value of <code>Class.forName(get(uidClassID))</code> </returns>
		''' <seealso cref= #getUI </seealso>
		Public Overridable Function getUIClass(ByVal uiClassID As String, ByVal uiClassLoader As ClassLoader) As Type
			Try
				Dim className As String = CStr([get](uiClassID))
				If className IsNot Nothing Then
					sun.reflect.misc.ReflectUtil.checkPackageAccess(className)

					Dim cls As Type = CType([get](className), [Class])
					If cls Is Nothing Then
						If uiClassLoader Is Nothing Then
							cls = SwingUtilities.loadSystemClass(className)
						Else
							cls = uiClassLoader.loadClass(className)
						End If
						If cls IsNot Nothing Then put(className, cls)
					End If
					Return cls
				End If
			Catch e As ClassNotFoundException
				Return Nothing
			Catch e As ClassCastException
				Return Nothing
			End Try
			Return Nothing
		End Function


		''' <summary>
		''' Returns the L&amp;F class that renders this component.
		''' </summary>
		''' <param name="uiClassID"> a string containing the class ID </param>
		''' <returns> the Class object returned by
		'''          <code>getUIClass(uiClassID, null)</code> </returns>
		Public Overridable Function getUIClass(ByVal uiClassID As String) As Type
			Return getUIClass(uiClassID, Nothing)
		End Function


		''' <summary>
		''' If <code>getUI()</code> fails for any reason,
		''' it calls this method before returning <code>null</code>.
		''' Subclasses may choose to do more or less here.
		''' </summary>
		''' <param name="msg"> message string to print </param>
		''' <seealso cref= #getUI </seealso>
		Protected Friend Overridable Sub getUIError(ByVal msg As String)
			Console.Error.WriteLine("UIDefaults.getUI() failed: " & msg)
			Try
				Throw New Exception
			Catch e As Exception
				e.printStackTrace()
			End Try
		End Sub

		''' <summary>
		''' Creates an <code>ComponentUI</code> implementation for the
		''' specified component.  In other words create the look
		''' and feel specific delegate object for <code>target</code>.
		''' This is done in two steps:
		''' <ul>
		''' <li> Look up the name of the <code>ComponentUI</code> implementation
		''' class under the value returned by <code>target.getUIClassID()</code>.
		''' <li> Use the implementation classes static <code>createUI()</code>
		''' method to construct a look and feel delegate.
		''' </ul> </summary>
		''' <param name="target">  the <code>JComponent</code> which needs a UI </param>
		''' <returns> the <code>ComponentUI</code> object </returns>
		Public Overridable Function getUI(ByVal target As JComponent) As javax.swing.plaf.ComponentUI

			Dim cl As Object = [get]("ClassLoader")
			Dim uiClassLoader As ClassLoader = If(cl IsNot Nothing, CType(cl, ClassLoader), target.GetType().classLoader)
			Dim ___uiClass As Type = getUIClass(target.uIClassID, uiClassLoader)
			Dim uiObject As Object = Nothing

			If ___uiClass Is Nothing Then
				getUIError("no ComponentUI class for: " & target)
			Else
				Try
					Dim m As Method = CType([get](___uiClass), Method)
					If m Is Nothing Then
						m = ___uiClass.GetMethod("createUI", New Type(){GetType(JComponent)})
						put(___uiClass, m)
					End If
					uiObject = sun.reflect.misc.MethodUtil.invoke(m, Nothing, New Object(){target})
				Catch e As NoSuchMethodException
					getUIError("static createUI() method not found in " & ___uiClass)
				Catch e As Exception
					getUIError("createUI() failed for " & target & " " & e)
				End Try
			End If

			Return CType(uiObject, javax.swing.plaf.ComponentUI)
		End Function

		''' <summary>
		''' Adds a <code>PropertyChangeListener</code> to the listener list.
		''' The listener is registered for all properties.
		''' <p>
		''' A <code>PropertyChangeEvent</code> will get fired whenever a default
		''' is changed.
		''' </summary>
		''' <param name="listener">  the <code>PropertyChangeListener</code> to be added </param>
		''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			If changeSupport Is Nothing Then changeSupport = New javax.swing.event.SwingPropertyChangeSupport(Me)
			changeSupport.addPropertyChangeListener(listener)
		End Sub


		''' <summary>
		''' Removes a <code>PropertyChangeListener</code> from the listener list.
		''' This removes a <code>PropertyChangeListener</code> that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the <code>PropertyChangeListener</code> to be removed </param>
		''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			If changeSupport IsNot Nothing Then changeSupport.removePropertyChangeListener(listener)
		End Sub


		''' <summary>
		''' Returns an array of all the <code>PropertyChangeListener</code>s added
		''' to this UIDefaults with addPropertyChangeListener().
		''' </summary>
		''' <returns> all of the <code>PropertyChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property propertyChangeListeners As java.beans.PropertyChangeListener()
			Get
				If changeSupport Is Nothing Then Return New java.beans.PropertyChangeListener(){}
				Return changeSupport.propertyChangeListeners
			End Get
		End Property


		''' <summary>
		''' Support for reporting bound property changes.  If oldValue and
		''' newValue are not equal and the <code>PropertyChangeEvent</code>x
		''' listener list isn't empty, then fire a
		''' <code>PropertyChange</code> event to each listener.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property
		'''          that was changed </param>
		''' <param name="oldValue">  the old value of the property </param>
		''' <param name="newValue">  the new value of the property </param>
		''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
		Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If changeSupport IsNot Nothing Then changeSupport.firePropertyChange(propertyName, oldValue, newValue)
		End Sub


		''' <summary>
		''' Adds a resource bundle to the list of resource bundles that are
		''' searched for localized values.  Resource bundles are searched in the
		''' reverse order they were added.  In other words, the most recently added
		''' bundle is searched first.
		''' </summary>
		''' <param name="bundleName">  the base name of the resource bundle to be added </param>
		''' <seealso cref= java.util.ResourceBundle </seealso>
		''' <seealso cref= #removeResourceBundle
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addResourceBundle(ByVal bundleName As String)
			If bundleName Is Nothing Then Return
			If resourceBundles Is Nothing Then resourceBundles = New List(Of String)(5)
			If Not resourceBundles.Contains(bundleName) Then
				resourceBundles.Add(bundleName)
				resourceCache.Clear()
			End If
		End Sub


		''' <summary>
		''' Removes a resource bundle from the list of resource bundles that are
		''' searched for localized defaults.
		''' </summary>
		''' <param name="bundleName">  the base name of the resource bundle to be removed </param>
		''' <seealso cref= java.util.ResourceBundle </seealso>
		''' <seealso cref= #addResourceBundle
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeResourceBundle(ByVal bundleName As String)
			If resourceBundles IsNot Nothing Then resourceBundles.Remove(bundleName)
			resourceCache.Clear()
		End Sub

		''' <summary>
		''' Sets the default locale.  The default locale is used in retrieving
		''' localized values via <code>get</code> methods that do not take a
		''' locale argument.  As of release 1.4, Swing UI objects should retrieve
		''' localized values using the locale of their component rather than the
		''' default locale.  The default locale exists to provide compatibility with
		''' pre 1.4 behaviour.
		''' </summary>
		''' <param name="l"> the new default locale </param>
		''' <seealso cref= #getDefaultLocale </seealso>
		''' <seealso cref= #get(Object) </seealso>
		''' <seealso cref= #get(Object,Locale)
		''' @since 1.4 </seealso>
		Public Overridable Property defaultLocale As java.util.Locale
			Set(ByVal l As java.util.Locale)
				defaultLocale = l
			End Set
			Get
				Return defaultLocale
			End Get
		End Property


		''' <summary>
		''' This class enables one to store an entry in the defaults
		''' table that isn't constructed until the first time it's
		''' looked up with one of the <code>getXXX(key)</code> methods.
		''' Lazy values are useful for defaults that are expensive
		''' to construct or are seldom retrieved.  The first time
		''' a <code>LazyValue</code> is retrieved its "real value" is computed
		''' by calling <code>LazyValue.createValue()</code> and the real
		''' value is used to replace the <code>LazyValue</code> in the
		''' <code>UIDefaults</code>
		''' table.  Subsequent lookups for the same key return
		''' the real value.  Here's an example of a <code>LazyValue</code>
		''' that constructs a <code>Border</code>:
		''' <pre>
		'''  Object borderLazyValue = new UIDefaults.LazyValue() {
		'''      public Object createValue(UIDefaults table) {
		'''          return new BorderFactory.createLoweredBevelBorder();
		'''      }
		'''  };
		''' 
		'''  uiDefaultsTable.put("MyBorder", borderLazyValue);
		''' </pre>
		''' </summary>
		''' <seealso cref= UIDefaults#get </seealso>
		Public Interface LazyValue
			''' <summary>
			''' Creates the actual value retrieved from the <code>UIDefaults</code>
			''' table. When an object that implements this interface is
			''' retrieved from the table, this method is used to create
			''' the real value, which is then stored in the table and
			''' returned to the calling method.
			''' </summary>
			''' <param name="table">  a <code>UIDefaults</code> table </param>
			''' <returns> the created <code>Object</code> </returns>
			Function createValue(ByVal table As UIDefaults) As Object
		End Interface


		''' <summary>
		''' This class enables one to store an entry in the defaults
		''' table that's constructed each time it's looked up with one of
		''' the <code>getXXX(key)</code> methods. Here's an example of
		''' an <code>ActiveValue</code> that constructs a
		''' <code>DefaultListCellRenderer</code>:
		''' <pre>
		'''  Object cellRendererActiveValue = new UIDefaults.ActiveValue() {
		'''      public Object createValue(UIDefaults table) {
		'''          return new DefaultListCellRenderer();
		'''      }
		'''  };
		''' 
		'''  uiDefaultsTable.put("MyRenderer", cellRendererActiveValue);
		''' </pre>
		''' </summary>
		''' <seealso cref= UIDefaults#get </seealso>
		Public Interface ActiveValue
			''' <summary>
			''' Creates the value retrieved from the <code>UIDefaults</code> table.
			''' The object is created each time it is accessed.
			''' </summary>
			''' <param name="table">  a <code>UIDefaults</code> table </param>
			''' <returns> the created <code>Object</code> </returns>
			Function createValue(ByVal table As UIDefaults) As Object
		End Interface

		''' <summary>
		''' This class provides an implementation of <code>LazyValue</code>
		''' which can be
		''' used to delay loading of the Class for the instance to be created.
		''' It also avoids creation of an anonymous inner class for the
		''' <code>LazyValue</code>
		''' subclass.  Both of these improve performance at the time that a
		''' a Look and Feel is loaded, at the cost of a slight performance
		''' reduction the first time <code>createValue</code> is called
		''' (since Reflection APIs are used).
		''' @since 1.3
		''' </summary>
		Public Class ProxyLazyValue
			Implements LazyValue

			Private acc As java.security.AccessControlContext
			Private className As String
			Private methodName As String
			Private args As Object()

			''' <summary>
			''' Creates a <code>LazyValue</code> which will construct an instance
			''' when asked.
			''' </summary>
			''' <param name="c">    a <code>String</code> specifying the classname
			'''             of the instance to be created on demand </param>
			Public Sub New(ByVal c As String)
				Me.New(c, CStr(Nothing))
			End Sub
			''' <summary>
			''' Creates a <code>LazyValue</code> which will construct an instance
			''' when asked.
			''' </summary>
			''' <param name="c">    a <code>String</code> specifying the classname of
			'''              the class
			'''              containing a static method to be called for
			'''              instance creation </param>
			''' <param name="m">    a <code>String</code> specifying the static
			'''              method to be called on class c </param>
			Public Sub New(ByVal c As String, ByVal m As String)
				Me.New(c, m, Nothing)
			End Sub
			''' <summary>
			''' Creates a <code>LazyValue</code> which will construct an instance
			''' when asked.
			''' </summary>
			''' <param name="c">    a <code>String</code> specifying the classname
			'''              of the instance to be created on demand </param>
			''' <param name="o">    an array of <code>Objects</code> to be passed as
			'''              paramaters to the constructor in class c </param>
			Public Sub New(ByVal c As String, ByVal o As Object())
				Me.New(c, Nothing, o)
			End Sub
			''' <summary>
			''' Creates a <code>LazyValue</code> which will construct an instance
			''' when asked.
			''' </summary>
			''' <param name="c">    a <code>String</code> specifying the classname
			'''              of the class
			'''              containing a static method to be called for
			'''              instance creation. </param>
			''' <param name="m">    a <code>String</code> specifying the static method
			'''              to be called on class c </param>
			''' <param name="o">    an array of <code>Objects</code> to be passed as
			'''              paramaters to the static method in class c </param>
			Public Sub New(ByVal c As String, ByVal m As String, ByVal o As Object())
				acc = java.security.AccessController.context
				className = c
				methodName = m
				If o IsNot Nothing Then args = o.clone()
			End Sub

			''' <summary>
			''' Creates the value retrieved from the <code>UIDefaults</code> table.
			''' The object is created each time it is accessed.
			''' </summary>
			''' <param name="table">  a <code>UIDefaults</code> table </param>
			''' <returns> the created <code>Object</code> </returns>
			Public Overridable Function createValue(ByVal table As UIDefaults) As Object
				' In order to pick up the security policy in effect at the
				' time of creation we use a doPrivileged with the
				' AccessControlContext that was in place when this was created.
				If acc Is Nothing AndAlso System.securityManager IsNot Nothing Then Throw New SecurityException("null AccessControlContext")
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'			{
	'				public Object run()
	'				{
	'					try
	'					{
	'						Class c;
	'						Object cl;
	'						' See if we should use a separate ClassLoader
	'						if (table == Nothing || !((cl = table.get("ClassLoader")) instanceof ClassLoader))
	'						{
	'							cl = Thread.currentThread().getContextClassLoader();
	'							if (cl == Nothing)
	'							{
	'								' Fallback to the system class loader.
	'								cl = ClassLoader.getSystemClassLoader();
	'							}
	'						}
	'						ReflectUtil.checkPackageAccess(className);
	'						c = Class.forName(className, True, (ClassLoader)cl);
	'						SwingUtilities2.checkAccess(c.getModifiers());
	'						if (methodName != Nothing)
	'						{
	'							Class[] types = getClassArray(args);
	'							Method m = c.getMethod(methodName, types);
	'							Return MethodUtil.invoke(m, c, args);
	'						}
	'						else
	'						{
	'							Class[] types = getClassArray(args);
	'							Constructor constructor = c.getConstructor(types);
	'							SwingUtilities2.checkAccess(constructor.getModifiers());
	'							Return constructor.newInstance(args);
	'						}
	'					}
	'					catch(Exception e)
	'					{
	'						' Ideally we would throw an exception, unfortunately
	'						' often times there are errors as an initial look and
	'						' feel is loaded before one can be switched. Perhaps a
	'						' flag should be added for debugging, so that if true
	'						' the exception would be thrown.
	'					}
	'					Return Nothing;
	'				}
	'			}, acc);
			End Function

	'        
	'         * Coerce the array of class types provided into one which
	'         * looks the way the Reflection APIs expect.  This is done
	'         * by substituting primitive types for their Object counterparts,
	'         * and superclasses for subclasses used to add the
	'         * <code>UIResource</code> tag.
	'         
			Private Function getClassArray(ByVal args As Object()) As Type()
				Dim types As Type() = Nothing
				If args IsNot Nothing Then
					types = New Type(args.Length - 1){}
					For i As Integer = 0 To args.Length - 1
	'                     PENDING(ges): At present only the primitive types
	'                       used are handled correctly; this should eventually
	'                       handle all primitive types 
						If TypeOf args(i) Is Integer? Then
							types(i)=Integer.TYPE
						ElseIf TypeOf args(i) Is Boolean? Then
							types(i)=Boolean.TYPE
						ElseIf TypeOf args(i) Is javax.swing.plaf.ColorUIResource Then
	'                         PENDING(ges) Currently the Reflection APIs do not
	'                           search superclasses of parameters supplied for
	'                           constructor/method lookup.  Since we only have
	'                           one case where this is needed, we substitute
	'                           directly instead of adding a massive amount
	'                           of mechanism for this.  Eventually this will
	'                           probably need to handle the general case as well.
	'                           
							types(i)=GetType(java.awt.Color)
						Else
							types(i)=args(i).GetType()
						End If
					Next i
				End If
				Return types
			End Function

			Private Function printArgs(ByVal array As Object()) As String
				Dim s As String = "{"
				If array IsNot Nothing Then
					For i As Integer = 0 To array.Length-2
						s = s + array(i) & ","
					Next i
					s = s + array(array.Length-1) & "}"
				Else
					s = s & "}"
				End If
				Return s
			End Function
		End Class


		''' <summary>
		''' <code>LazyInputMap</code> will create a <code>InputMap</code>
		''' in its <code>createValue</code>
		''' method. The bindings are passed in in the constructor.
		''' The bindings are an array with
		''' the even number entries being string <code>KeyStrokes</code>
		''' (eg "alt SPACE") and
		''' the odd number entries being the value to use in the
		''' <code>InputMap</code> (and the key in the <code>ActionMap</code>).
		''' @since 1.3
		''' </summary>
		Public Class LazyInputMap
			Implements LazyValue

			''' <summary>
			''' Key bindings are registered under. </summary>
			Private bindings As Object()

			Public Sub New(ByVal bindings As Object())
				Me.bindings = bindings
			End Sub

			''' <summary>
			''' Creates an <code>InputMap</code> with the bindings that are
			''' passed in.
			''' </summary>
			''' <param name="table"> a <code>UIDefaults</code> table </param>
			''' <returns> the <code>InputMap</code> </returns>
			Public Overridable Function createValue(ByVal table As UIDefaults) As Object
				If bindings IsNot Nothing Then
					Dim km As InputMap = LookAndFeel.makeInputMap(bindings)
					Return km
				End If
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' <code>TextAndMnemonicHashMap</code> stores swing resource strings. Many of strings
		''' can have a mnemonic. For example:
		'''   FileChooser.saveButton.textAndMnemonic=&Save
		''' For this case method get returns "Save" for the key "FileChooser.saveButtonText" and
		''' mnemonic "S" for the key "FileChooser.saveButtonMnemonic"
		''' 
		''' There are several patterns for the text and mnemonic suffixes which are checked by the
		''' <code>TextAndMnemonicHashMap</code> class.
		''' Patterns which are converted to the xxx.textAndMnemonic key:
		''' (xxxNameText, xxxNameMnemonic)
		''' (xxxNameText, xxxMnemonic)
		''' (xxx.nameText, xxx.mnemonic)
		''' (xxxText, xxxMnemonic)
		''' 
		''' These patterns can have a mnemonic index in format
		''' (xxxDisplayedMnemonicIndex)
		''' 
		''' Pattern which is converted to the xxx.titleAndMnemonic key:
		''' (xxxTitle, xxxMnemonic)
		''' 
		''' </summary>
		Private Class TextAndMnemonicHashMap
			Inherits Dictionary(Of String, Object)

			Friend Const AND_MNEMONIC As String = "AndMnemonic"
			Friend Const TITLE_SUFFIX As String = ".titleAndMnemonic"
			Friend Const TEXT_SUFFIX As String = ".textAndMnemonic"

			Public Overrides Function [get](ByVal key As Object) As Object

				Dim value As Object = MyBase.get(key)

				If value Is Nothing Then

					Dim checkTitle As Boolean = False

					Dim stringKey As String = key.ToString()
					Dim compositeKey As String = Nothing

					If stringKey.EndsWith(AND_MNEMONIC) Then Return Nothing

					If stringKey.EndsWith(".mnemonic") Then
						compositeKey = composeKey(stringKey, 9, TEXT_SUFFIX)
					ElseIf stringKey.EndsWith("NameMnemonic") Then
						compositeKey = composeKey(stringKey, 12, TEXT_SUFFIX)
					ElseIf stringKey.EndsWith("Mnemonic") Then
						compositeKey = composeKey(stringKey, 8, TEXT_SUFFIX)
						checkTitle = True
					End If

					If compositeKey IsNot Nothing Then
						value = MyBase.get(compositeKey)
						If value Is Nothing AndAlso checkTitle Then
							compositeKey = composeKey(stringKey, 8, TITLE_SUFFIX)
							value = MyBase.get(compositeKey)
						End If

						Return If(value Is Nothing, Nothing, getMnemonicFromProperty(value.ToString()))
					End If

					If stringKey.EndsWith("NameText") Then
						compositeKey = composeKey(stringKey, 8, TEXT_SUFFIX)
					ElseIf stringKey.EndsWith(".nameText") Then
						compositeKey = composeKey(stringKey, 9, TEXT_SUFFIX)
					ElseIf stringKey.EndsWith("Text") Then
						compositeKey = composeKey(stringKey, 4, TEXT_SUFFIX)
					ElseIf stringKey.EndsWith("Title") Then
						compositeKey = composeKey(stringKey, 5, TITLE_SUFFIX)
					End If

					If compositeKey IsNot Nothing Then
						value = MyBase.get(compositeKey)
						Return If(value Is Nothing, Nothing, getTextFromProperty(value.ToString()))
					End If

					If stringKey.EndsWith("DisplayedMnemonicIndex") Then
						compositeKey = composeKey(stringKey, 22, TEXT_SUFFIX)
						value = MyBase.get(compositeKey)
						If value Is Nothing Then
							compositeKey = composeKey(stringKey, 22, TITLE_SUFFIX)
							value = MyBase.get(compositeKey)
						End If
						Return If(value Is Nothing, Nothing, getIndexFromProperty(value.ToString()))
					End If
				End If

				Return value
			End Function

			Friend Overridable Function composeKey(ByVal key As String, ByVal reduce As Integer, ByVal sufix As String) As String
				Return key.Substring(0, key.Length - reduce) + sufix
			End Function

			Friend Overridable Function getTextFromProperty(ByVal text As String) As String
				Return text.Replace("&", "")
			End Function

			Friend Overridable Function getMnemonicFromProperty(ByVal text As String) As String
				Dim index As Integer = text.IndexOf("&"c)
				If 0 <= index AndAlso index < text.Length - 1 Then
					Dim c As Char = text.Chars(index + 1)
					Return Convert.ToString(AscW(Char.ToUpper(c)))
				End If
				Return Nothing
			End Function

			Friend Overridable Function getIndexFromProperty(ByVal text As String) As String
				Dim index As Integer = text.IndexOf("&"c)
				Return If(index = -1, Nothing, Convert.ToString(index))
			End Function
		End Class

	End Class

End Namespace
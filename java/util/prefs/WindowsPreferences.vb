Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2000, 2002, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' Windows registry based implementation of  <tt>Preferences</tt>.
	''' <tt>Preferences</tt>' <tt>systemRoot</tt> and <tt>userRoot</tt> are stored in
	''' <tt>HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Prefs</tt> and
	''' <tt>HKEY_CURRENT_USER\Software\JavaSoft\Prefs</tt> correspondingly.
	''' 
	''' @author  Konstantin Kladko </summary>
	''' <seealso cref= Preferences </seealso>
	''' <seealso cref= PreferencesFactory
	''' @since 1.4 </seealso>

	Friend Class WindowsPreferences
		Inherits AbstractPreferences

		''' <summary>
		''' Logger for error messages
		''' </summary>
		Private Shared logger_Renamed As sun.util.logging.PlatformLogger

		''' <summary>
		''' Windows registry path to <tt>Preferences</tt>'s root nodes.
		''' </summary>
		Private Shared ReadOnly WINDOWS_ROOT_PATH As SByte() = stringToByteArray("Software\JavaSoft\Prefs")

		''' <summary>
		''' Windows handles to <tt>HKEY_CURRENT_USER</tt> and
		''' <tt>HKEY_LOCAL_MACHINE</tt> hives.
		''' </summary>
		Private Const HKEY_CURRENT_USER As Integer = &H80000001L
		Private Const HKEY_LOCAL_MACHINE As Integer = &H80000002L

		''' <summary>
		''' Mount point for <tt>Preferences</tt>'  user root.
		''' </summary>
		Private Const USER_ROOT_NATIVE_HANDLE As Integer = HKEY_CURRENT_USER

		''' <summary>
		''' Mount point for <tt>Preferences</tt>'  system root.
		''' </summary>
		Private Const SYSTEM_ROOT_NATIVE_HANDLE As Integer = HKEY_LOCAL_MACHINE

		''' <summary>
		''' Maximum byte-encoded path length for Windows native functions,
		''' ending <tt>null</tt> character not included.
		''' </summary>
		Private Const MAX_WINDOWS_PATH_LENGTH As Integer = 256

		''' <summary>
		''' User root node.
		''' </summary>
		Friend Shared ReadOnly userRoot As Preferences = New WindowsPreferences(USER_ROOT_NATIVE_HANDLE, WINDOWS_ROOT_PATH)

		''' <summary>
		''' System root node.
		''' </summary>
		Friend Shared ReadOnly systemRoot As Preferences = New WindowsPreferences(SYSTEM_ROOT_NATIVE_HANDLE, WINDOWS_ROOT_PATH)

		'  Windows error codes. 
		Private Const ERROR_SUCCESS As Integer = 0
		Private Const ERROR_FILE_NOT_FOUND As Integer = 2
		Private Const ERROR_ACCESS_DENIED As Integer = 5

		' Constants used to interpret returns of native functions    
		Private Const NATIVE_HANDLE As Integer = 0
		Private Const ERROR_CODE As Integer = 1
		Private Const SUBKEYS_NUMBER As Integer = 0
		Private Const VALUES_NUMBER As Integer = 2
		Private Shadows Const MAX_KEY_LENGTH As Integer = 3
		Private Const MAX_VALUE_NAME_LENGTH As Integer = 4
		Private Const DISPOSITION As Integer = 2
		Private Const REG_CREATED_NEW_KEY As Integer = 1
		Private Const REG_OPENED_EXISTING_KEY As Integer = 2
		Private Const NULL_NATIVE_HANDLE As Integer = 0

		' Windows security masks 
		Private Const DELETE As Integer = &H10000
		Private Const KEY_QUERY_VALUE As Integer = 1
		Private Const KEY_SET_VALUE As Integer = 2
		Private Const KEY_CREATE_SUB_KEY As Integer = 4
		Private Const KEY_ENUMERATE_SUB_KEYS As Integer = 8
		Private Const KEY_READ As Integer = &H20019
		Private Const KEY_WRITE As Integer = &H20006
		Private Const KEY_ALL_ACCESS As Integer = &Hf003f

		''' <summary>
		''' Initial time between registry access attempts, in ms. The time is doubled
		''' after each failing attempt (except the first).
		''' </summary>
		Private Shared INIT_SLEEP_TIME As Integer = 50

		''' <summary>
		''' Maximum number of registry access attempts.
		''' </summary>
		Private Shared MAX_ATTEMPTS As Integer = 5

		''' <summary>
		''' BackingStore availability flag.
		''' </summary>
		Private isBackingStoreAvailable As Boolean = True

		''' <summary>
		''' Java wrapper for Windows registry API RegOpenKey()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegOpenKey(ByVal hKey As Integer, ByVal subKey As SByte(), ByVal securityMask As Integer) As Integer()
		End Function
		''' <summary>
		''' Retries RegOpenKey() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegOpenKey1(ByVal hKey As Integer, ByVal subKey As SByte(), ByVal securityMask As Integer) As Integer()
			Dim result As Integer() = WindowsRegOpenKey(hKey, subKey, securityMask)
			If result(ERROR_CODE) = ERROR_SUCCESS Then
				Return result
			ElseIf result(ERROR_CODE) = ERROR_FILE_NOT_FOUND Then
				logger().warning("Trying to recreate Windows registry node " & byteArrayToString(subKey) & " at root 0x" & Integer.toHexString(hKey) & ".")
				' Try recreation
				Dim handle As Integer = WindowsRegCreateKeyEx(hKey, subKey)(NATIVE_HANDLE)
				WindowsRegCloseKey(handle)
				Return WindowsRegOpenKey(hKey, subKey, securityMask)
			ElseIf result(ERROR_CODE) <> ERROR_ACCESS_DENIED Then
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegOpenKey(hKey, subKey, securityMask)
					If result(ERROR_CODE) = ERROR_SUCCESS Then Return result
				Next i
			End If
			Return result
		End Function

		 ''' <summary>
		 ''' Java wrapper for Windows registry API RegCloseKey()
		 ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegCloseKey(ByVal hKey As Integer) As Integer
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegCreateKeyEx()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegCreateKeyEx(ByVal hKey As Integer, ByVal subKey As SByte()) As Integer()
		End Function

		''' <summary>
		''' Retries RegCreateKeyEx() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegCreateKeyEx1(ByVal hKey As Integer, ByVal subKey As SByte()) As Integer()
			Dim result As Integer() = WindowsRegCreateKeyEx(hKey, subKey)
			If result(ERROR_CODE) = ERROR_SUCCESS Then
				Return result
			Else
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegCreateKeyEx(hKey, subKey)
					If result(ERROR_CODE) = ERROR_SUCCESS Then Return result
				Next i
			End If
			Return result
		End Function
		''' <summary>
		''' Java wrapper for Windows registry API RegDeleteKey()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegDeleteKey(ByVal hKey As Integer, ByVal subKey As SByte()) As Integer
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegFlushKey()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegFlushKey(ByVal hKey As Integer) As Integer
		End Function

		''' <summary>
		''' Retries RegFlushKey() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegFlushKey1(ByVal hKey As Integer) As Integer
			Dim result As Integer = WindowsRegFlushKey(hKey)
			If result = ERROR_SUCCESS Then
				Return result
			Else
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegFlushKey(hKey)
					If result = ERROR_SUCCESS Then Return result
				Next i
			End If
			Return result
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegQueryValueEx()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegQueryValueEx(ByVal hKey As Integer, ByVal valueName As SByte()) As SByte()
		End Function
		''' <summary>
		''' Java wrapper for Windows registry API RegSetValueEx()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegSetValueEx(ByVal hKey As Integer, ByVal valueName As SByte(), ByVal value As SByte()) As Integer
		End Function
		''' <summary>
		''' Retries RegSetValueEx() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegSetValueEx1(ByVal hKey As Integer, ByVal valueName As SByte(), ByVal value As SByte()) As Integer
			Dim result As Integer = WindowsRegSetValueEx(hKey, valueName, value)
			If result = ERROR_SUCCESS Then
				Return result
			Else
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegSetValueEx(hKey, valueName, value)
					If result = ERROR_SUCCESS Then Return result
				Next i
			End If
			Return result
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegDeleteValue()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegDeleteValue(ByVal hKey As Integer, ByVal valueName As SByte()) As Integer
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegQueryInfoKey()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegQueryInfoKey(ByVal hKey As Integer) As Integer()
		End Function

		''' <summary>
		''' Retries RegQueryInfoKey() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegQueryInfoKey1(ByVal hKey As Integer) As Integer()
			Dim result As Integer() = WindowsRegQueryInfoKey(hKey)
			If result(ERROR_CODE) = ERROR_SUCCESS Then
				Return result
			Else
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegQueryInfoKey(hKey)
					If result(ERROR_CODE) = ERROR_SUCCESS Then Return result
				Next i
			End If
			Return result
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegEnumKeyEx()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegEnumKeyEx(ByVal hKey As Integer, ByVal subKeyIndex As Integer, ByVal maxKeyLength As Integer) As SByte()
		End Function

		''' <summary>
		''' Retries RegEnumKeyEx() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegEnumKeyEx1(ByVal hKey As Integer, ByVal subKeyIndex As Integer, ByVal maxKeyLength As Integer) As SByte()
			Dim result As SByte() = WindowsRegEnumKeyEx(hKey, subKeyIndex, maxKeyLength)
			If result IsNot Nothing Then
				Return result
			Else
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegEnumKeyEx(hKey, subKeyIndex, maxKeyLength)
					If result IsNot Nothing Then Return result
				Next i
			End If
			Return result
		End Function

		''' <summary>
		''' Java wrapper for Windows registry API RegEnumValue()
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function WindowsRegEnumValue(ByVal hKey As Integer, ByVal valueIndex As Integer, ByVal maxValueNameLength As Integer) As SByte()
		End Function
		''' <summary>
		''' Retries RegEnumValueEx() MAX_ATTEMPTS times before giving up.
		''' </summary>
		Private Shared Function WindowsRegEnumValue1(ByVal hKey As Integer, ByVal valueIndex As Integer, ByVal maxValueNameLength As Integer) As SByte()
			Dim result As SByte() = WindowsRegEnumValue(hKey, valueIndex, maxValueNameLength)
			If result IsNot Nothing Then
				Return result
			Else
				Dim sleepTime As Long = INIT_SLEEP_TIME
				For i As Integer = 0 To MAX_ATTEMPTS - 1
					Try
						Thread.Sleep(sleepTime)
					Catch e As InterruptedException
						Return result
					End Try
					sleepTime *= 2
					result = WindowsRegEnumValue(hKey, valueIndex, maxValueNameLength)
					If result IsNot Nothing Then Return result
				Next i
			End If
			Return result
		End Function

		''' <summary>
		''' Constructs a <tt>WindowsPreferences</tt> node, creating underlying
		''' Windows registry node and all its Windows parents, if they are not yet
		''' created.
		''' Logs a warning message, if Windows Registry is unavailable.
		''' </summary>
		Private Sub New(ByVal parent As WindowsPreferences, ByVal name As String)
			MyBase.New(parent, name)
			Dim parentNativeHandle As Integer = parent.openKey(KEY_CREATE_SUB_KEY, KEY_READ)
			If parentNativeHandle = NULL_NATIVE_HANDLE Then
				' if here, openKey failed and logged
				isBackingStoreAvailable = False
				Return
			End If
			Dim result As Integer() = WindowsRegCreateKeyEx1(parentNativeHandle, toWindowsName(name))
			If result(ERROR_CODE) <> ERROR_SUCCESS Then
				logger().warning("Could not create windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegCreateKeyEx(...) returned error code " & result(ERROR_CODE) & ".")
				isBackingStoreAvailable = False
				Return
			End If
			newNode = (result(DISPOSITION) = REG_CREATED_NEW_KEY)
			closeKey(parentNativeHandle)
			closeKey(result(NATIVE_HANDLE))
		End Sub

		''' <summary>
		''' Constructs a root node creating the underlying
		''' Windows registry node and all of its parents, if they have not yet been
		''' created.
		''' Logs a warning message, if Windows Registry is unavailable. </summary>
		''' <param name="rootNativeHandle"> Native handle to one of Windows top level keys. </param>
		''' <param name="rootDirectory"> Path to root directory, as a byte-encoded string. </param>
		Private Sub New(ByVal rootNativeHandle As Integer, ByVal rootDirectory As SByte())
			MyBase.New(Nothing, "")
			Dim result As Integer() = WindowsRegCreateKeyEx1(rootNativeHandle, rootDirectory)
			If result(ERROR_CODE) <> ERROR_SUCCESS Then
				logger().warning("Could not open/create prefs root node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegCreateKeyEx(...) returned error code " & result(ERROR_CODE) & ".")
				isBackingStoreAvailable = False
				Return
			End If
			' Check if a new node
			newNode = (result(DISPOSITION) = REG_CREATED_NEW_KEY)
			closeKey(result(NATIVE_HANDLE))
		End Sub

		''' <summary>
		''' Returns Windows absolute path of the current node as a byte array.
		''' Java "/" separator is transformed into Windows "\". </summary>
		''' <seealso cref= Preferences#absolutePath() </seealso>
		Private Function windowsAbsolutePath() As SByte()
			Dim bstream As New java.io.ByteArrayOutputStream
			bstream.write(WINDOWS_ROOT_PATH, 0, WINDOWS_ROOT_PATH.Length-1)
			Dim tokenizer As New java.util.StringTokenizer(absolutePath(), "/")
			Do While tokenizer.hasMoreTokens()
				bstream.write(AscW("\"c))
				Dim nextName As String = tokenizer.nextToken()
				Dim windowsNextName As SByte() = toWindowsName(nextName)
				bstream.write(windowsNextName, 0, windowsNextName.Length-1)
			Loop
			bstream.write(0)
			Return bstream.toByteArray()
		End Function

		''' <summary>
		''' Opens current node's underlying Windows registry key using a
		''' given security mask. </summary>
		''' <param name="securityMask"> Windows security mask. </param>
		''' <returns> Windows registry key's handle. </returns>
		''' <seealso cref= #openKey(byte[], int) </seealso>
		''' <seealso cref= #openKey(int, byte[], int) </seealso>
		''' <seealso cref= #closeKey(int) </seealso>
		Private Function openKey(ByVal securityMask As Integer) As Integer
			Return openKey(securityMask, securityMask)
		End Function

		''' <summary>
		''' Opens current node's underlying Windows registry key using a
		''' given security mask. </summary>
		''' <param name="mask1"> Preferred Windows security mask. </param>
		''' <param name="mask2"> Alternate Windows security mask. </param>
		''' <returns> Windows registry key's handle. </returns>
		''' <seealso cref= #openKey(byte[], int) </seealso>
		''' <seealso cref= #openKey(int, byte[], int) </seealso>
		''' <seealso cref= #closeKey(int) </seealso>
		Private Function openKey(ByVal mask1 As Integer, ByVal mask2 As Integer) As Integer
			Return openKey(windowsAbsolutePath(), mask1, mask2)
		End Function

		 ''' <summary>
		 ''' Opens Windows registry key at a given absolute path using a given
		 ''' security mask. </summary>
		 ''' <param name="windowsAbsolutePath"> Windows absolute path of the
		 '''        key as a byte-encoded string. </param>
		 ''' <param name="mask1"> Preferred Windows security mask. </param>
		 ''' <param name="mask2"> Alternate Windows security mask. </param>
		 ''' <returns> Windows registry key's handle. </returns>
		 ''' <seealso cref= #openKey(int) </seealso>
		 ''' <seealso cref= #openKey(int, byte[],int) </seealso>
		 ''' <seealso cref= #closeKey(int) </seealso>
		Private Function openKey(ByVal windowsAbsolutePath As SByte(), ByVal mask1 As Integer, ByVal mask2 As Integer) As Integer
	'          Check if key's path is short enough be opened at once
	'            otherwise use a path-splitting procedure 
			If windowsAbsolutePath.Length <= MAX_WINDOWS_PATH_LENGTH + 1 Then
				Dim result As Integer() = WindowsRegOpenKey1(rootNativeHandle(), windowsAbsolutePath, mask1)
				If result(ERROR_CODE) = ERROR_ACCESS_DENIED AndAlso mask2 <> mask1 Then result = WindowsRegOpenKey1(rootNativeHandle(), windowsAbsolutePath, mask2)

				If result(ERROR_CODE) <> ERROR_SUCCESS Then
					logger().warning("Could not open windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegOpenKey(...) returned error code " & result(ERROR_CODE) & ".")
					result(NATIVE_HANDLE) = NULL_NATIVE_HANDLE
					If result(ERROR_CODE) = ERROR_ACCESS_DENIED Then Throw New SecurityException("Could not open windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ": Access denied")
				End If
				Return result(NATIVE_HANDLE)
			Else
				Return openKey(rootNativeHandle(), windowsAbsolutePath, mask1, mask2)
			End If
		End Function

		 ''' <summary>
		 ''' Opens Windows registry key at a given relative path
		 ''' with respect to a given Windows registry key. </summary>
		 ''' <param name="windowsAbsolutePath"> Windows relative path of the
		 '''        key as a byte-encoded string. </param>
		 ''' <param name="nativeHandle"> handle to the base Windows key. </param>
		 ''' <param name="mask1"> Preferred Windows security mask. </param>
		 ''' <param name="mask2"> Alternate Windows security mask. </param>
		 ''' <returns> Windows registry key's handle. </returns>
		 ''' <seealso cref= #openKey(int) </seealso>
		 ''' <seealso cref= #openKey(byte[],int) </seealso>
		 ''' <seealso cref= #closeKey(int) </seealso>
		Private Function openKey(ByVal nativeHandle As Integer, ByVal windowsRelativePath As SByte(), ByVal mask1 As Integer, ByVal mask2 As Integer) As Integer
		' If the path is short enough open at once. Otherwise split the path 
			If windowsRelativePath.Length <= MAX_WINDOWS_PATH_LENGTH + 1 Then
				Dim result As Integer() = WindowsRegOpenKey1(nativeHandle, windowsRelativePath, mask1)
				If result(ERROR_CODE) = ERROR_ACCESS_DENIED AndAlso mask2 <> mask1 Then result = WindowsRegOpenKey1(nativeHandle, windowsRelativePath, mask2)

				If result(ERROR_CODE) <> ERROR_SUCCESS Then
					logger().warning("Could not open windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(nativeHandle) & ". Windows RegOpenKey(...) returned error code " & result(ERROR_CODE) & ".")
					result(NATIVE_HANDLE) = NULL_NATIVE_HANDLE
				End If
				Return result(NATIVE_HANDLE)
			Else
				Dim separatorPosition As Integer = -1
				' Be greedy - open the longest possible path
				For i As Integer = MAX_WINDOWS_PATH_LENGTH To 1 Step -1
					If windowsRelativePath(i) = (AscW("\"c)) Then
						separatorPosition = i
						Exit For
					End If
				Next i
				' Split the path and do the recursion
				Dim nextRelativeRoot As SByte() = New SByte(separatorPosition){}
				Array.Copy(windowsRelativePath, 0, nextRelativeRoot,0, separatorPosition)
				nextRelativeRoot(separatorPosition) = 0
				Dim nextRelativePath As SByte() = New SByte(windowsRelativePath.Length - separatorPosition - 2){}
				Array.Copy(windowsRelativePath, separatorPosition+1, nextRelativePath, 0, nextRelativePath.Length)
				Dim nextNativeHandle As Integer = openKey(nativeHandle, nextRelativeRoot, mask1, mask2)
				If nextNativeHandle = NULL_NATIVE_HANDLE Then Return NULL_NATIVE_HANDLE
				Dim result As Integer = openKey(nextNativeHandle, nextRelativePath, mask1,mask2)
				closeKey(nextNativeHandle)
				Return result
			End If
		End Function

		 ''' <summary>
		 ''' Closes Windows registry key.
		 ''' Logs a warning if Windows registry is unavailable. </summary>
		 ''' <param name="key">'s Windows registry handle. </param>
		 ''' <seealso cref= #openKey(int) </seealso>
		 ''' <seealso cref= #openKey(byte[],int) </seealso>
		 ''' <seealso cref= #openKey(int, byte[],int) </seealso>
		Private Sub closeKey(ByVal nativeHandle As Integer)
			Dim result As Integer = WindowsRegCloseKey(nativeHandle)
			If result <> ERROR_SUCCESS Then logger().warning("Could not close windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegCloseKey(...) returned error code " & result & ".")
		End Sub

		 ''' <summary>
		 ''' Implements <tt>AbstractPreferences</tt> <tt>putSpi()</tt> method.
		 ''' Puts name-value pair into the underlying Windows registry node.
		 ''' Logs a warning, if Windows registry is unavailable. </summary>
		 ''' <seealso cref= #getSpi(String) </seealso>
		Protected Friend Overrides Sub putSpi(ByVal javaName As String, ByVal value As String)
			Dim nativeHandle As Integer = openKey(KEY_SET_VALUE)
			If nativeHandle = NULL_NATIVE_HANDLE Then
				isBackingStoreAvailable = False
				Return
			End If
			Dim result As Integer = WindowsRegSetValueEx1(nativeHandle, toWindowsName(javaName), toWindowsValueString(value))
			If result <> ERROR_SUCCESS Then
				logger().warning("Could not assign value to key " & byteArrayToString(toWindowsName(javaName)) & " at Windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegSetValueEx(...) returned error code " & result & ".")
				isBackingStoreAvailable = False
			End If
			closeKey(nativeHandle)
		End Sub

		''' <summary>
		''' Implements <tt>AbstractPreferences</tt> <tt>getSpi()</tt> method.
		''' Gets a string value from the underlying Windows registry node.
		''' Logs a warning, if Windows registry is unavailable. </summary>
		''' <seealso cref= #putSpi(String, String) </seealso>
		Protected Friend Overrides Function getSpi(ByVal javaName As String) As String
		Dim nativeHandle As Integer = openKey(KEY_QUERY_VALUE)
		If nativeHandle = NULL_NATIVE_HANDLE Then Return Nothing
		Dim resultObject As Object = WindowsRegQueryValueEx(nativeHandle, toWindowsName(javaName))
		If resultObject Is Nothing Then
			closeKey(nativeHandle)
			Return Nothing
		End If
		closeKey(nativeHandle)
		Return toJavaValueString(CType(resultObject, SByte()))
		End Function

		''' <summary>
		''' Implements <tt>AbstractPreferences</tt> <tt>removeSpi()</tt> method.
		''' Deletes a string name-value pair from the underlying Windows registry
		''' node, if this value still exists.
		''' Logs a warning, if Windows registry is unavailable or key has already
		''' been deleted.
		''' </summary>
		Protected Friend Overrides Sub removeSpi(ByVal key As String)
			Dim nativeHandle As Integer = openKey(KEY_SET_VALUE)
			If nativeHandle = NULL_NATIVE_HANDLE Then Return
			Dim result As Integer = WindowsRegDeleteValue(nativeHandle, toWindowsName(key))
			If result <> ERROR_SUCCESS AndAlso result <> ERROR_FILE_NOT_FOUND Then
				logger().warning("Could not delete windows registry value " & byteArrayToString(windowsAbsolutePath()) & "\" & toWindowsName(key) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegDeleteValue(...) returned error code " & result & ".")
				isBackingStoreAvailable = False
			End If
			closeKey(nativeHandle)
		End Sub

		''' <summary>
		''' Implements <tt>AbstractPreferences</tt> <tt>keysSpi()</tt> method.
		''' Gets value names from the underlying Windows registry node.
		''' Throws a BackingStoreException and logs a warning, if
		''' Windows registry is unavailable.
		''' </summary>
		Protected Friend Overrides Function keysSpi() As String()
			' Find out the number of values
			Dim nativeHandle As Integer = openKey(KEY_QUERY_VALUE)
			If nativeHandle = NULL_NATIVE_HANDLE Then Throw New BackingStoreException("Could not open windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ".")
			Dim result As Integer() = WindowsRegQueryInfoKey1(nativeHandle)
			If result(ERROR_CODE) <> ERROR_SUCCESS Then
				Dim info As String = "Could not query windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegQueryInfoKeyEx(...) returned error code " & result(ERROR_CODE) & "."
				logger().warning(info)
				Throw New BackingStoreException(info)
			End If
			Dim maxValueNameLength As Integer = result(MAX_VALUE_NAME_LENGTH)
			Dim valuesNumber As Integer = result(VALUES_NUMBER)
			If valuesNumber = 0 Then
				closeKey(nativeHandle)
				Return New String(){}
			End If
			' Get the values
			Dim valueNames As String() = New String(valuesNumber - 1){}
			For i As Integer = 0 To valuesNumber - 1
				Dim windowsName As SByte() = WindowsRegEnumValue1(nativeHandle, i, maxValueNameLength+1)
				If windowsName Is Nothing Then
					Dim info As String = "Could not enumerate value #" & i & "  of windows node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & "."
					logger().warning(info)
					Throw New BackingStoreException(info)
				End If
				valueNames(i) = toJavaName(windowsName)
			Next i
			closeKey(nativeHandle)
			Return valueNames
		End Function

		''' <summary>
		''' Implements <tt>AbstractPreferences</tt> <tt>childrenNamesSpi()</tt> method.
		''' Calls Windows registry to retrive children of this node.
		''' Throws a BackingStoreException and logs a warning message,
		''' if Windows registry is not available.
		''' </summary>
		Protected Friend Overrides Function childrenNamesSpi() As String()
			' Open key
			Dim nativeHandle As Integer = openKey(KEY_ENUMERATE_SUB_KEYS Or KEY_QUERY_VALUE)
			If nativeHandle = NULL_NATIVE_HANDLE Then Throw New BackingStoreException("Could not open windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ".")
			' Get number of children
			Dim result As Integer() = WindowsRegQueryInfoKey1(nativeHandle)
			If result(ERROR_CODE) <> ERROR_SUCCESS Then
				Dim info As String = "Could not query windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegQueryInfoKeyEx(...) returned error code " & result(ERROR_CODE) & "."
				logger().warning(info)
				Throw New BackingStoreException(info)
			End If
			Dim maxKeyLength As Integer = result(MAX_KEY_LENGTH)
			Dim subKeysNumber As Integer = result(SUBKEYS_NUMBER)
			If subKeysNumber = 0 Then
				closeKey(nativeHandle)
				Return New String(){}
			End If
			Dim subkeys As String() = New String(subKeysNumber - 1){}
			Dim children As String() = New String(subKeysNumber - 1){}
			' Get children
			For i As Integer = 0 To subKeysNumber - 1
				Dim windowsName As SByte() = WindowsRegEnumKeyEx1(nativeHandle, i, maxKeyLength+1)
				If windowsName Is Nothing Then
					Dim info As String = "Could not enumerate key #" & i & "  of windows node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". "
					logger().warning(info)
					Throw New BackingStoreException(info)
				End If
				Dim javaName As String = toJavaName(windowsName)
				children(i) = javaName
			Next i
			closeKey(nativeHandle)
			Return children
		End Function

		''' <summary>
		''' Implements <tt>Preferences</tt> <tt>flush()</tt> method.
		''' Flushes Windows registry changes to disk.
		''' Throws a BackingStoreException and logs a warning message if Windows
		''' registry is not available.
		''' </summary>
		Public Overrides Sub flush()

			If removed Then
				parent_Renamed.flush()
				Return
			End If
			If Not isBackingStoreAvailable Then Throw New BackingStoreException("flush(): Backing store not available.")
			Dim nativeHandle As Integer = openKey(KEY_READ)
			If nativeHandle = NULL_NATIVE_HANDLE Then Throw New BackingStoreException("Could not open windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ".")
			Dim result As Integer = WindowsRegFlushKey1(nativeHandle)
			If result <> ERROR_SUCCESS Then
				Dim info As String = "Could not flush windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegFlushKey(...) returned error code " & result & "."
				logger().warning(info)
				Throw New BackingStoreException(info)
			End If
			closeKey(nativeHandle)
		End Sub


		''' <summary>
		''' Implements <tt>Preferences</tt> <tt>sync()</tt> method.
		''' Flushes Windows registry changes to disk. Equivalent to flush(). </summary>
		''' <seealso cref= flush() </seealso>
		Public Overrides Sub sync()
			If removed Then Throw New IllegalStateException("Node has been removed")
			flush()
		End Sub

		''' <summary>
		''' Implements <tt>AbstractPreferences</tt> <tt>childSpi()</tt> method.
		''' Constructs a child node with a
		''' given name and creates its underlying Windows registry node,
		''' if it does not exist.
		''' Logs a warning message, if Windows Registry is unavailable.
		''' </summary>
		Protected Friend Overrides Function childSpi(ByVal name As String) As AbstractPreferences
			Return New WindowsPreferences(Me, name)
		End Function

		''' <summary>
		''' Implements <tt>AbstractPreferences</tt> <tt>removeNodeSpi()</tt> method.
		''' Deletes underlying Windows registry node.
		''' Throws a BackingStoreException and logs a warning, if Windows registry
		''' is not available.
		''' </summary>
		Public Overrides Sub removeNodeSpi()
			Dim parentNativeHandle As Integer = CType(parent(), WindowsPreferences).openKey(DELETE)
			If parentNativeHandle = NULL_NATIVE_HANDLE Then Throw New BackingStoreException("Could not open parent windows registry node of " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ".")
			Dim result As Integer = WindowsRegDeleteKey(parentNativeHandle, toWindowsName(name()))
			If result <> ERROR_SUCCESS Then
				Dim info As String = "Could not delete windows registry node " & byteArrayToString(windowsAbsolutePath()) & " at root 0x" & Integer.toHexString(rootNativeHandle()) & ". Windows RegDeleteKeyEx(...) returned error code " & result & "."
				logger().warning(info)
				Throw New BackingStoreException(info)
			End If
			closeKey(parentNativeHandle)
		End Sub

		''' <summary>
		''' Converts value's or node's name from its byte array representation to
		''' java string. Two encodings, simple and altBase64 are used. See
		''' <seealso cref="#toWindowsName(String) toWindowsName()"/> for a detailed
		''' description of encoding conventions. </summary>
		''' <param name="windowsNameArray"> Null-terminated byte array. </param>
		Private Shared Function toJavaName(ByVal windowsNameArray As SByte()) As String
			Dim windowsName As String = byteArrayToString(windowsNameArray)
			' check if Alt64
			If (windowsName.length() > 1) AndAlso (windowsName.Substring(0, 2).Equals("/!")) Then Return toJavaAlt64Name(windowsName)
			Dim javaName As New StringBuilder
			Dim ch As Char
			' Decode from simple encoding
			For i As Integer = 0 To windowsName.length() - 1
				ch = windowsName.Chars(i)
				If ch = "/"c Then
					Dim [next] As Char = " "c
					[next] = windowsName.Chars(i+1)
					If (windowsName.length() > i + 1) AndAlso ([next] >= "A"c) AndAlso ([next] <= "Z"c) Then
						ch = [next]
						i += 1
					ElseIf (windowsName.length() > i + 1) AndAlso ([next] = "/"c) Then
						ch = "\"c
						i += 1
					End If
				ElseIf ch = "\"c Then
					ch = "/"c
				End If
				javaName.append(ch)
			Next i
			Return javaName.ToString()
		End Function

		''' <summary>
		''' Converts value's or node's name from its Windows representation to java
		''' string, using altBase64 encoding. See
		''' <seealso cref="#toWindowsName(String) toWindowsName()"/> for a detailed
		''' description of encoding conventions.
		''' </summary>

		Private Shared Function toJavaAlt64Name(ByVal windowsName As String) As String
			Dim byteBuffer As SByte() = Base64.altBase64ToByteArray(windowsName.Substring(2))
			Dim result As New StringBuilder
			For i As Integer = 0 To byteBuffer.Length - 1
				Dim firstbyte As Integer = (byteBuffer(i) And &Hff)
				i += 1
				Dim secondbyte As Integer = (byteBuffer(i) And &Hff)
				result.append(CChar((firstbyte << 8) + secondbyte))
			Next i
			Return result.ToString()
		End Function

		''' <summary>
		''' Converts value's or node's name to its Windows representation
		''' as a byte-encoded string.
		''' Two encodings, simple and altBase64 are used.
		''' <p>
		''' <i>Simple</i> encoding is used, if java string does not contain
		''' any characters less, than 0x0020, or greater, than 0x007f.
		''' Simple encoding adds "/" character to capital letters, i.e.
		''' "A" is encoded as "/A". Character '\' is encoded as '//',
		''' '/' is encoded as '\'.
		''' The constructed string is converted to byte array by truncating the
		''' highest byte and adding the terminating <tt>null</tt> character.
		''' <p>
		''' <i>altBase64</i>  encoding is used, if java string does contain at least
		''' one character less, than 0x0020, or greater, than 0x007f.
		''' This encoding is marked by setting first two bytes of the
		''' Windows string to '/!'. The java name is then encoded using
		''' byteArrayToAltBase64() method from
		''' Base64 class.
		''' </summary>
		Private Shared Function toWindowsName(ByVal javaName As String) As SByte()
			Dim windowsName As New StringBuilder
			For i As Integer = 0 To javaName.length() - 1
				Dim ch As Char = javaName.Chars(i)
				If (AscW(ch) < &H20) OrElse (AscW(ch) > &H7f) Then Return toWindowsAlt64Name(javaName)
				If ch = "\"c Then
					windowsName.append("//")
				ElseIf ch = "/"c Then
					windowsName.append("\"c)
				ElseIf (ch >= "A"c) AndAlso (ch <="Z"c) Then
					windowsName.append("/"c).append(ch)
				Else
					windowsName.append(ch)
				End If
			Next i
			Return stringToByteArray(windowsName.ToString())
		End Function

		''' <summary>
		''' Converts value's or node's name to its Windows representation
		''' as a byte-encoded string, using altBase64 encoding. See
		''' <seealso cref="#toWindowsName(String) toWindowsName()"/> for a detailed
		''' description of encoding conventions.
		''' </summary>
		Private Shared Function toWindowsAlt64Name(ByVal javaName As String) As SByte()
			Dim javaNameArray As SByte() = New SByte(2*javaName.length() - 1){}
			' Convert to byte pairs
			Dim counter As Integer = 0
			For i As Integer = 0 To javaName.length() - 1
				Dim ch As Integer = AscW(javaName.Chars(i))
				javaNameArray(counter) = CByte(CInt(CUInt(ch) >> 8))
				counter += 1
				javaNameArray(counter) = CByte(ch)
				counter += 1
			Next i

			Return stringToByteArray("/!" & Base64.byteArrayToAltBase64(javaNameArray))
		End Function

		''' <summary>
		''' Converts value string from its Windows representation
		''' to java string.  See
		''' <seealso cref="#toWindowsValueString(String) toWindowsValueString()"/> for the
		''' description of the encoding algorithm.
		''' </summary>
		 Private Shared Function toJavaValueString(ByVal windowsNameArray As SByte()) As String
			' Use modified native2ascii algorithm
			Dim windowsName As String = byteArrayToString(windowsNameArray)
			Dim javaName As New StringBuilder
			Dim ch As Char
			For i As Integer = 0 To windowsName.length() - 1
				ch = windowsName.Chars(i)
				If ch = "/"c Then
					Dim [next] As Char = " "c

					[next] = windowsName.Chars(i + 1)
					If windowsName.length() > i + 1 AndAlso [next] = "u"c Then
						If windowsName.length() < i + 6 Then
							Exit For
						Else
							ch = ChrW(Convert.ToInt32(windowsName.Substring(i + 2, i + 6 - (i + 2)), 16))
							i += 5
						End If
					Else
					If (windowsName.length() > i + 1) AndAlso ((windowsName.Chars(i+1)) >= "A"c) AndAlso ([next] <= "Z"c) Then
						ch = [next]
						i += 1
					ElseIf (windowsName.length() > i + 1) AndAlso ([next] = "/"c) Then
						ch = "\"c
						i += 1
					End If
					End If
				ElseIf ch = "\"c Then
					ch = "/"c
				End If
				javaName.append(ch)
			Next i
			Return javaName.ToString()
		 End Function

		''' <summary>
		''' Converts value string to it Windows representation.
		''' as a byte-encoded string.
		''' Encoding algorithm adds "/" character to capital letters, i.e.
		''' "A" is encoded as "/A". Character '\' is encoded as '//',
		''' '/' is encoded as  '\'.
		''' Then encoding scheme similar to jdk's native2ascii converter is used
		''' to convert java string to a byte array of ASCII characters.
		''' </summary>
		Private Shared Function toWindowsValueString(ByVal javaName As String) As SByte()
			Dim windowsName As New StringBuilder
			For i As Integer = 0 To javaName.length() - 1
				Dim ch As Char = javaName.Chars(i)
				If (AscW(ch) < &H20) OrElse (AscW(ch) > &H7f) Then
					' write \udddd
					windowsName.append("/u")
					Dim hex As String = Integer.toHexString(javaName.Chars(i))
					Dim hex4 As New StringBuilder(hex)
					hex4.reverse()
					Dim len As Integer = 4 - hex4.length()
					For j As Integer = 0 To len - 1
						hex4.append("0"c)
					Next j
					For j As Integer = 0 To 3
						windowsName.append(hex4.Chars(3 - j))
					Next j
				ElseIf ch = "\"c Then
					windowsName.append("//")
				ElseIf ch = "/"c Then
					windowsName.append("\"c)
				ElseIf (ch >= "A"c) AndAlso (ch <="Z"c) Then
					windowsName.append("/"c).append(ch)
				Else
					windowsName.append(ch)
				End If
			Next i
			Return stringToByteArray(windowsName.ToString())
		End Function

		''' <summary>
		''' Returns native handle for the top Windows node for this node.
		''' </summary>
		Private Function rootNativeHandle() As Integer
			Return (If(userNode, USER_ROOT_NATIVE_HANDLE, SYSTEM_ROOT_NATIVE_HANDLE))
		End Function

		''' <summary>
		''' Returns this java string as a null-terminated byte array
		''' </summary>
		Private Shared Function stringToByteArray(ByVal str As String) As SByte()
			Dim result As SByte() = New SByte(str.length()){}
			For i As Integer = 0 To str.length() - 1
				result(i) = AscW(str.Chars(i))
			Next i
			result(str.length()) = 0
			Return result
		End Function

		''' <summary>
		''' Converts a null-terminated byte array to java string
		''' </summary>
		Private Shared Function byteArrayToString(ByVal array As SByte()) As String
			Dim result As New StringBuilder
			For i As Integer = 0 To array.Length - 2
				result.append(ChrW(array(i)))
			Next i
			Return result.ToString()
		End Function

	   ''' <summary>
	   ''' Empty, never used implementation  of AbstractPreferences.flushSpi().
	   ''' </summary>
		Protected Friend Overrides Sub flushSpi()
			' assert false;
		End Sub

	   ''' <summary>
	   ''' Empty, never used implementation  of AbstractPreferences.flushSpi().
	   ''' </summary>
		Protected Friend Overrides Sub syncSpi()
			' assert false;
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function logger() As sun.util.logging.PlatformLogger
			If logger_Renamed Is Nothing Then logger_Renamed = sun.util.logging.PlatformLogger.getLogger("java.util.prefs")
			Return logger_Renamed
		End Function
	End Class

End Namespace
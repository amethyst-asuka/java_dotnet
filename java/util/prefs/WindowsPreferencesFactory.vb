'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' Implementation of  <tt>PreferencesFactory</tt> to return
	''' WindowsPreferences objects.
	''' 
	''' @author  Konstantin Kladko </summary>
	''' <seealso cref= Preferences </seealso>
	''' <seealso cref= WindowsPreferences
	''' @since 1.4 </seealso>
	Friend Class WindowsPreferencesFactory
		Implements PreferencesFactory

		''' <summary>
		''' Returns WindowsPreferences.userRoot
		''' </summary>
		Public Overridable Function userRoot() As Preferences Implements PreferencesFactory.userRoot
			Return WindowsPreferences.userRoot
		End Function

		''' <summary>
		''' Returns WindowsPreferences.systemRoot
		''' </summary>
		Public Overridable Function systemRoot() As Preferences Implements PreferencesFactory.systemRoot
			Return WindowsPreferences.systemRoot
		End Function
	End Class

End Namespace
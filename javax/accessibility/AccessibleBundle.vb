Imports System
Imports System.Collections

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

Namespace javax.accessibility


	''' <summary>
	''' <p>Base class used to maintain a strongly typed enumeration.  This is
	''' the superclass of <seealso cref="AccessibleState"/> and <seealso cref="AccessibleRole"/>.
	''' <p>The toDisplayString method allows you to obtain the localized string
	''' for a locale independent key from a predefined ResourceBundle for the
	''' keys defined in this class.  This localized string is intended to be
	''' readable by humans.
	''' </summary>
	''' <seealso cref= AccessibleRole </seealso>
	''' <seealso cref= AccessibleState
	''' 
	''' @author      Willie Walker
	''' @author      Peter Korn
	''' @author      Lynn Monsanto </seealso>
	Public MustInherit Class AccessibleBundle

		Private Shared table As New Hashtable
		Private ReadOnly defaultResourceBundleName As String = "com.sun.accessibility.internal.resources.accessibility"

		''' <summary>
		''' Construct an {@code AccessibleBundle}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' The locale independent name of the state.  This is a programmatic
		''' name that is not intended to be read by humans. </summary>
		''' <seealso cref= #toDisplayString </seealso>
		Protected Friend key As String = Nothing

		''' <summary>
		''' Obtains the key as a localized string.
		''' If a localized string cannot be found for the key, the
		''' locale independent key stored in the role will be returned.
		''' This method is intended to be used only by subclasses so that they
		''' can specify their own resource bundles which contain localized
		''' strings for their keys. </summary>
		''' <param name="resourceBundleName"> the name of the resource bundle to use for
		''' lookup </param>
		''' <param name="locale"> the locale for which to obtain a localized string </param>
		''' <returns> a localized String for the key. </returns>
		Protected Friend Overridable Function toDisplayString(ByVal resourceBundleName As String, ByVal locale As java.util.Locale) As String

			' loads the resource bundle if necessary
			loadResourceBundle(resourceBundleName, locale)

			' returns the localized string
			Dim o As Object = table(locale)
			If o IsNot Nothing AndAlso TypeOf o Is Hashtable Then
					Dim resourceTable As Hashtable = CType(o, Hashtable)
					o = resourceTable(key)

					If o IsNot Nothing AndAlso TypeOf o Is String Then Return CStr(o)
			End If
			Return key
		End Function

		''' <summary>
		''' Obtains the key as a localized string.
		''' If a localized string cannot be found for the key, the
		''' locale independent key stored in the role will be returned.
		''' </summary>
		''' <param name="locale"> the locale for which to obtain a localized string </param>
		''' <returns> a localized String for the key. </returns>
		Public Overridable Function toDisplayString(ByVal locale As java.util.Locale) As String
			Return toDisplayString(defaultResourceBundleName, locale)
		End Function

		''' <summary>
		''' Gets localized string describing the key using the default locale. </summary>
		''' <returns> a localized String describing the key for the default locale </returns>
		Public Overridable Function toDisplayString() As String
			Return toDisplayString(java.util.Locale.default)
		End Function

		''' <summary>
		''' Gets localized string describing the key using the default locale. </summary>
		''' <returns> a localized String describing the key using the default locale </returns>
		''' <seealso cref= #toDisplayString </seealso>
		Public Overrides Function ToString() As String
			Return toDisplayString()
		End Function

	'    
	'     * Loads the Accessibility resource bundle if necessary.
	'     
		Private Sub loadResourceBundle(ByVal resourceBundleName As String, ByVal locale As java.util.Locale)
			If Not table.Contains(locale) Then

				Try
					Dim resourceTable As New Hashtable

					Dim bundle As java.util.ResourceBundle = java.util.ResourceBundle.getBundle(resourceBundleName, locale)

					Dim iter As System.Collections.IEnumerator = bundle.keys
					Do While iter.hasMoreElements()
						Dim key As String = CStr(iter.nextElement())
						resourceTable(key) = bundle.getObject(key)
					Loop

					table(locale) = resourceTable
				Catch e As java.util.MissingResourceException
					Console.Error.WriteLine("loadResourceBundle: " & e)
					' Just return so toDisplayString() returns the
					' non-localized key.
					Return
				End Try
			End If
		End Sub

	End Class

End Namespace
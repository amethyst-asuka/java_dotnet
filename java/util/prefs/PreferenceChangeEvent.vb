'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' An event emitted by a <tt>Preferences</tt> node to indicate that
	''' a preference has been added, removed or has had its value changed.<p>
	''' 
	''' Note, that although PreferenceChangeEvent inherits Serializable interface
	''' from EventObject, it is not intended to be Serializable. Appropriate
	''' serialization methods are implemented to throw NotSerializableException.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref= Preferences </seealso>
	''' <seealso cref= PreferenceChangeListener </seealso>
	''' <seealso cref= NodeChangeEvent
	''' @since   1.4
	''' @serial exclude </seealso>
	Public Class PreferenceChangeEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Key of the preference that changed.
		''' 
		''' @serial
		''' </summary>
		Private key As String

		''' <summary>
		''' New value for preference, or <tt>null</tt> if it was removed.
		''' 
		''' @serial
		''' </summary>
		Private newValue As String

		''' <summary>
		''' Constructs a new <code>PreferenceChangeEvent</code> instance.
		''' </summary>
		''' <param name="node">  The Preferences node that emitted the event. </param>
		''' <param name="key">  The key of the preference that was changed. </param>
		''' <param name="newValue">  The new value of the preference, or <tt>null</tt>
		'''                  if the preference is being removed. </param>
		Public Sub New(  node As Preferences,   key As String,   newValue As String)
			MyBase.New(node)
			Me.key = key
			Me.newValue = newValue
		End Sub

		''' <summary>
		''' Returns the preference node that emitted the event.
		''' </summary>
		''' <returns>  The preference node that emitted the event. </returns>
		Public Overridable Property node As Preferences
			Get
				Return CType(source, Preferences)
			End Get
		End Property

		''' <summary>
		''' Returns the key of the preference that was changed.
		''' </summary>
		''' <returns>  The key of the preference that was changed. </returns>
		Public Overridable Property key As String
			Get
				Return key
			End Get
		End Property

		''' <summary>
		''' Returns the new value for the preference.
		''' </summary>
		''' <returns>  The new value for the preference, or <tt>null</tt> if the
		'''          preference was removed. </returns>
		Public Overridable Property newValue As String
			Get
				Return newValue
			End Get
		End Property

		''' <summary>
		''' Throws NotSerializableException, since NodeChangeEvent objects
		''' are not intended to be serializable.
		''' </summary>
		 Private Sub writeObject(  out As java.io.ObjectOutputStream)
			 Throw New java.io.NotSerializableException("Not serializable.")
		 End Sub

		''' <summary>
		''' Throws NotSerializableException, since PreferenceChangeEvent objects
		''' are not intended to be serializable.
		''' </summary>
		 Private Sub readObject(  [in] As java.io.ObjectInputStream)
			 Throw New java.io.NotSerializableException("Not serializable.")
		 End Sub

		' Defined so that this class isn't flagged as a potential problem when
		' searches for missing serialVersionUID fields are done.
		Private Const serialVersionUID As Long = 793724513368024975L
	End Class

End Namespace
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
	''' a child of that node has been added or removed.<p>
	''' 
	''' Note, that although NodeChangeEvent inherits Serializable interface from
	''' java.util.EventObject, it is not intended to be Serializable. Appropriate
	''' serialization methods are implemented to throw NotSerializableException.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     Preferences </seealso>
	''' <seealso cref=     NodeChangeListener </seealso>
	''' <seealso cref=     PreferenceChangeEvent
	''' @since   1.4
	''' @serial  exclude </seealso>

	Public Class NodeChangeEvent
		Inherits java.util.EventObject

		''' <summary>
		''' The node that was added or removed.
		''' 
		''' @serial
		''' </summary>
		Private child As Preferences

		''' <summary>
		''' Constructs a new <code>NodeChangeEvent</code> instance.
		''' </summary>
		''' <param name="parent">  The parent of the node that was added or removed. </param>
		''' <param name="child">   The node that was added or removed. </param>
		Public Sub New(  parent As Preferences,   child As Preferences)
			MyBase.New(parent)
			Me.child = child
		End Sub

		''' <summary>
		''' Returns the parent of the node that was added or removed.
		''' </summary>
		''' <returns>  The parent Preferences node whose child was added or removed </returns>
		Public Overridable Property parent As Preferences
			Get
				Return CType(source, Preferences)
			End Get
		End Property

		''' <summary>
		''' Returns the node that was added or removed.
		''' </summary>
		''' <returns>  The node that was added or removed. </returns>
		Public Overridable Property child As Preferences
			Get
				Return child
			End Get
		End Property

		''' <summary>
		''' Throws NotSerializableException, since NodeChangeEvent objects are not
		''' intended to be serializable.
		''' </summary>
		 Private Sub writeObject(  out As java.io.ObjectOutputStream)
			 Throw New java.io.NotSerializableException("Not serializable.")
		 End Sub

		''' <summary>
		''' Throws NotSerializableException, since NodeChangeEvent objects are not
		''' intended to be serializable.
		''' </summary>
		 Private Sub readObject(  [in] As java.io.ObjectInputStream)
			 Throw New java.io.NotSerializableException("Not serializable.")
		 End Sub

		' Defined so that this class isn't flagged as a potential problem when
		' searches for missing serialVersionUID fields are done.
		Private Const serialVersionUID As Long = 8068949086596572957L
	End Class

End Namespace
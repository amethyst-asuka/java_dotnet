'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' * The original version of this source code and documentation
' * is copyrighted and owned by Taligent, Inc., a wholly-owned
' * subsidiary of IBM. These materials are provided under terms
' * of a License Agreement between Taligent and Sun. This technology
' * is protected by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.util

	''' <summary>
	''' Signals that a resource is missing. </summary>
	''' <seealso cref= java.lang.Exception </seealso>
	''' <seealso cref= ResourceBundle
	''' @author      Mark Davis
	''' @since       JDK1.1 </seealso>
	Public Class MissingResourceException
		Inherits RuntimeException

		''' <summary>
		''' Constructs a MissingResourceException with the specified information.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="className"> the name of the resource class </param>
		''' <param name="key"> the key for the missing resource. </param>
		Public Sub New(ByVal s As String, ByVal className As String, ByVal key As String)
			MyBase.New(s)
			Me.className = className
			Me.key = key
		End Sub

		''' <summary>
		''' Constructs a <code>MissingResourceException</code> with
		''' <code>message</code>, <code>className</code>, <code>key</code>,
		''' and <code>cause</code>. This constructor is package private for
		''' use by <code>ResourceBundle.getBundle</code>.
		''' </summary>
		''' <param name="message">
		'''        the detail message </param>
		''' <param name="className">
		'''        the name of the resource class </param>
		''' <param name="key">
		'''        the key for the missing resource. </param>
		''' <param name="cause">
		'''        the cause (which is saved for later retrieval by the
		'''        <seealso cref="Throwable.getCause()"/> method). (A null value is
		'''        permitted, and indicates that the cause is nonexistent
		'''        or unknown.) </param>
		Friend Sub New(ByVal message As String, ByVal className As String, ByVal key As String, ByVal cause As Throwable)
			MyBase.New(message, cause)
			Me.className = className
			Me.key = key
		End Sub

		''' <summary>
		''' Gets parameter passed by constructor.
		''' </summary>
		''' <returns> the name of the resource class </returns>
		Public Overridable Property className As String
			Get
				Return className
			End Get
		End Property

		''' <summary>
		''' Gets parameter passed by constructor.
		''' </summary>
		''' <returns> the key for the missing resource </returns>
		Public Overridable Property key As String
			Get
				Return key
			End Get
		End Property

		'============ privates ============

		' serialization compatibility with JDK1.1
		Private Shadows Const serialVersionUID As Long = -4876345176062000401L

		''' <summary>
		''' The class name of the resource bundle requested by the user.
		''' @serial
		''' </summary>
		Private className As String

		''' <summary>
		''' The name of the specific resource requested by the user.
		''' @serial
		''' </summary>
		Private key As String
	End Class

End Namespace
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports javax.accessibility

'
' * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>Label</code> object is a component for placing text in a
	''' container. A label displays a single line of read-only text.
	''' The text can be changed by the application, but a user cannot edit it
	''' directly.
	''' <p>
	''' For example, the code&nbsp;.&nbsp;.&nbsp;.
	''' 
	''' <hr><blockquote><pre>
	''' setLayout(new FlowLayout(FlowLayout.CENTER, 10, 10));
	''' add(new Label("Hi There!"));
	''' add(new Label("Another Label"));
	''' </pre></blockquote><hr>
	''' <p>
	''' produces the following labels:
	''' <p>
	''' <img src="doc-files/Label-1.gif" alt="Two labels: 'Hi There!' and 'Another label'"
	''' style="float:center; margin: 7px 10px;">
	''' 
	''' @author      Sami Shaio
	''' @since       JDK1.0
	''' </summary>
	Public Class Label
		Inherits Component
		Implements Accessible

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Indicates that the label should be left justified.
		''' </summary>
		Public Const LEFT As Integer = 0

		''' <summary>
		''' Indicates that the label should be centered.
		''' </summary>
		Public Const CENTER As Integer = 1

		''' <summary>
		''' Indicates that the label should be right justified.
		''' @since   JDK1.0t.
		''' </summary>
		Public Const RIGHT As Integer = 2

		''' <summary>
		''' The text of this label.
		''' This text can be modified by the program
		''' but never by the user.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getText() </seealso>
		''' <seealso cref= #setText(String) </seealso>
		Friend text As String

		''' <summary>
		''' The label's alignment.  The default alignment is set
		''' to be left justified.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getAlignment() </seealso>
		''' <seealso cref= #setAlignment(int) </seealso>
		Friend alignment As Integer = LEFT

		Private Const base As String = "label"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = 3094126758329070636L

		''' <summary>
		''' Constructs an empty label.
		''' The text of the label is the empty string <code>""</code>. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New("", LEFT)
		End Sub

		''' <summary>
		''' Constructs a new label with the specified string of text,
		''' left justified. </summary>
		''' <param name="text"> the string that the label presents.
		'''        A <code>null</code> value
		'''        will be accepted without causing a NullPointerException
		'''        to be thrown. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal text As String)
			Me.New(text, LEFT)
		End Sub

		''' <summary>
		''' Constructs a new label that presents the specified string of
		''' text with the specified alignment.
		''' Possible values for <code>alignment</code> are <code>Label.LEFT</code>,
		''' <code>Label.RIGHT</code>, and <code>Label.CENTER</code>. </summary>
		''' <param name="text"> the string that the label presents.
		'''        A <code>null</code> value
		'''        will be accepted without causing a NullPointerException
		'''        to be thrown. </param>
		''' <param name="alignment">   the alignment value. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal text As String, ByVal alignment As Integer)
			GraphicsEnvironment.checkHeadless()
			Me.text = text
			alignment = alignment
		End Sub

		''' <summary>
		''' Read a label from an object input stream. </summary>
		''' <exception cref="HeadlessException"> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns
		''' <code>true</code>
		''' @serial
		''' @since 1.4 </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			GraphicsEnvironment.checkHeadless()
			s.defaultReadObject()
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is <code>null</code>.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Label)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the peer for this label.  The peer allows us to
		''' modify the appearance of the label without changing its
		''' functionality.
		''' </summary>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createLabel(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the current alignment of this label. Possible values are
		''' <code>Label.LEFT</code>, <code>Label.RIGHT</code>, and
		''' <code>Label.CENTER</code>. </summary>
		''' <seealso cref=        java.awt.Label#setAlignment </seealso>
		Public Overridable Property alignment As Integer
			Get
				Return alignment
			End Get
			Set(ByVal alignment As Integer)
				Select Case alignment
				  Case LEFT, CENTER, RIGHT
					Me.alignment = alignment
					Dim peer_Renamed As java.awt.peer.LabelPeer = CType(Me.peer, java.awt.peer.LabelPeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.alignment = alignment
					Return
				End Select
				Throw New IllegalArgumentException("improper alignment: " & alignment)
			End Set
		End Property


		''' <summary>
		''' Gets the text of this label. </summary>
		''' <returns>     the text of this label, or <code>null</code> if
		'''             the text has been set to <code>null</code>. </returns>
		''' <seealso cref=        java.awt.Label#setText </seealso>
		Public Overridable Property text As String
			Get
				Return text
			End Get
			Set(ByVal text As String)
				Dim testvalid As Boolean = False
				SyncLock Me
					If text <> Me.text AndAlso (Me.text Is Nothing OrElse (Not Me.text.Equals(text))) Then
						Me.text = text
						Dim peer_Renamed As java.awt.peer.LabelPeer = CType(Me.peer, java.awt.peer.LabelPeer)
						If peer_Renamed IsNot Nothing Then peer_Renamed.text = text
						testvalid = True
					End If
				End SyncLock
    
				' This could change the preferred size of the Component.
				If testvalid Then invalidateIfValid()
			End Set
		End Property


		''' <summary>
		''' Returns a string representing the state of this <code>Label</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>     the parameter string of this label </returns>
		Protected Friend Overrides Function paramString() As String
			Dim align As String = ""
			Select Case alignment
				Case LEFT
					align = "left"
				Case CENTER
					align = "center"
				Case RIGHT
					align = "right"
			End Select
			Return MyBase.paramString() & ",align=" & align & ",text=" & text
		End Function

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the AccessibleContext associated with this Label.
		''' For labels, the AccessibleContext takes the form of an
		''' AccessibleAWTLabel.
		''' A new AccessibleAWTLabel instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTLabel that serves as the
		'''         AccessibleContext of this Label
		''' @since 1.3 </returns>
		Public  Overrides ReadOnly Property  accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTLabel(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Label</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to label user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTLabel
			Inherits AccessibleAWTComponent

			Private ReadOnly outerInstance As Label

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -3568967560160480438L

			Public Sub New(ByVal outerInstance As Label)
					Me.outerInstance = outerInstance
				MyBase.New()
			End Sub

			''' <summary>
			''' Get the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be null if this
			''' object does not have a name </returns>
			''' <seealso cref= AccessibleContext#setAccessibleName </seealso>
			Public Overridable Property accessibleName As String
				Get
					If accessibleName IsNot Nothing Then
						Return accessibleName
					Else
						If outerInstance.text Is Nothing Then
							Return MyBase.accessibleName
						Else
							Return outerInstance.text
						End If
					End If
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.LABEL
				End Get
			End Property

		End Class ' inner class AccessibleAWTLabel

	End Class

End Namespace
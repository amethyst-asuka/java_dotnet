Imports javax.swing
Imports javax.swing.border

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal



	''' <summary>
	''' The default editor for Metal editable combo boxes
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Steve Wilson
	''' </summary>
	Public Class MetalComboBoxEditor
		Inherits javax.swing.plaf.basic.BasicComboBoxEditor

		Public Sub New()
			MyBase.New()
			'editor.removeFocusListener(this);
			editor = New JTextFieldAnonymousInnerClassHelper

			editor.border = New EditorBorder(Me)
			'editor.addFocusListener(this);
		End Sub

		Private Class JTextFieldAnonymousInnerClassHelper
			Inherits JTextField

						' workaround for 4530952
			Public Overrides Property text As String
				Set(ByVal s As String)
					If text.Equals(s) Then Return
					MyBase.text = s
				End Set
			End Property
		' The preferred and minimum sizes are overriden and padded by
		' 4 to keep the size as it previously was.  Refer to bugs
		' 4775789 and 4517214 for details.
		Public Property Overrides preferredSize As Dimension
			Get
				Dim pref As Dimension = MyBase.preferredSize
				pref.height += 4
				Return pref
			End Get
		End Property
		Public Property Overrides minimumSize As Dimension
			Get
				Dim min As Dimension = MyBase.minimumSize
				min.height += 4
				Return min
			End Get
		End Property
		End Class

	   ''' <summary>
	   ''' The default editor border <code>Insets</code>. This field
	   ''' might not be used.
	   ''' </summary>
		Protected Friend Shared editorBorderInsets As New Insets(2, 2, 2, 0)

		Friend Class EditorBorder
			Inherits AbstractBorder

			Private ReadOnly outerInstance As MetalComboBoxEditor

			Public Sub New(ByVal outerInstance As MetalComboBoxEditor)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub paintBorder(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				g.translate(x, y)

				If MetalLookAndFeel.usingOcean() Then
					g.color = MetalLookAndFeel.controlDarkShadow
					g.drawRect(0, 0, w, h - 1)
					g.color = MetalLookAndFeel.controlShadow
					g.drawRect(1, 1, w - 2, h - 3)
				Else
					g.color = MetalLookAndFeel.controlDarkShadow
					g.drawLine(0, 0, w-1, 0)
					g.drawLine(0, 0, 0, h-2)
					g.drawLine(0, h-2, w-1, h-2)
					g.color = MetalLookAndFeel.controlHighlight
					g.drawLine(1, 1, w-1, 1)
					g.drawLine(1, 1, 1, h-1)
					g.drawLine(1, h-1, w-1, h-1)
					g.color = MetalLookAndFeel.control
					g.drawLine(1, h-2, 1, h-2)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Function getBorderInsets(ByVal c As Component, ByVal insets As Insets) As Insets
				insets.set(2, 2, 2, 0)
				Return insets
			End Function
		End Class


		''' <summary>
		''' A subclass of BasicComboBoxEditor that implements UIResource.
		''' BasicComboBoxEditor doesn't implement UIResource
		''' directly so that applications can safely override the
		''' cellRenderer property with BasicListCellRenderer subclasses.
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
		Public Class UIResource
			Inherits MetalComboBoxEditor
			Implements javax.swing.plaf.UIResource

		End Class
	End Class

End Namespace
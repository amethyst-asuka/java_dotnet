Imports javax.accessibility
Imports javax.swing.event
Imports javax.swing.text

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
	''' A class to monitor the progress of some operation. If it looks
	''' like the operation will take a while, a progress dialog will be popped up.
	''' When the ProgressMonitor is created it is given a numeric range and a
	''' descriptive string. As the operation progresses, call the setProgress method
	''' to indicate how far along the [min,max] range the operation is.
	''' Initially, there is no ProgressDialog. After the first millisToDecideToPopup
	''' milliseconds (default 500) the progress monitor will predict how long
	''' the operation will take.  If it is longer than millisToPopup (default 2000,
	''' 2 seconds) a ProgressDialog will be popped up.
	''' <p>
	''' From time to time, when the Dialog box is visible, the progress bar will
	''' be updated when setProgress is called.  setProgress won't always update
	''' the progress bar, it will only be done if the amount of progress is
	''' visibly significant.
	''' 
	''' <p>
	''' 
	''' For further documentation and examples see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/progress.html">How to Monitor Progress</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' </summary>
	''' <seealso cref= ProgressMonitorInputStream
	''' @author James Gosling
	''' @author Lynn Monsanto (accessibility) </seealso>
	Public Class ProgressMonitor
		Implements Accessible

		Private root As ProgressMonitor
		Private dialog As JDialog
		Private pane As JOptionPane
		Private myBar As JProgressBar
		Private noteLabel As JLabel
		Private parentComponent As java.awt.Component
		Private note As String
		Private cancelOption As Object() = Nothing
		Private message As Object
		Private T0 As Long
		Private millisToDecideToPopup As Integer = 500
		Private millisToPopup As Integer = 2000
		Private min As Integer
		Private max As Integer


		''' <summary>
		''' Constructs a graphic object that shows progress, typically by filling
		''' in a rectangular bar as the process nears completion.
		''' </summary>
		''' <param name="parentComponent"> the parent component for the dialog box </param>
		''' <param name="message"> a descriptive message that will be shown
		'''        to the user to indicate what operation is being monitored.
		'''        This does not change as the operation progresses.
		'''        See the message parameters to methods in
		'''        <seealso cref="JOptionPane#message"/>
		'''        for the range of values. </param>
		''' <param name="note"> a short note describing the state of the
		'''        operation.  As the operation progresses, you can call
		'''        setNote to change the note displayed.  This is used,
		'''        for example, in operations that iterate through a
		'''        list of files to show the name of the file being processes.
		'''        If note is initially null, there will be no note line
		'''        in the dialog box and setNote will be ineffective </param>
		''' <param name="min"> the lower bound of the range </param>
		''' <param name="max"> the upper bound of the range </param>
		''' <seealso cref= JDialog </seealso>
		''' <seealso cref= JOptionPane </seealso>
		Public Sub New(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal note As String, ByVal min As Integer, ByVal max As Integer)
			Me.New(parentComponent, message, note, min, max, Nothing)
		End Sub


		Private Sub New(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal note As String, ByVal min As Integer, ByVal max As Integer, ByVal group As ProgressMonitor)
			Me.min = min
			Me.max = max
			Me.parentComponent = parentComponent

			cancelOption = New Object(0){}
			cancelOption(0) = UIManager.getString("OptionPane.cancelButtonText")

			Me.message = message
			Me.note = note
			If group IsNot Nothing Then
				root = If(group.root IsNot Nothing, group.root, group)
				T0 = root.T0
				dialog = root.dialog
			Else
				T0 = System.currentTimeMillis()
			End If
		End Sub


		Private Class ProgressOptionPane
			Inherits JOptionPane

			Private ReadOnly outerInstance As ProgressMonitor

			Friend Sub New(ByVal outerInstance As ProgressMonitor, ByVal messageList As Object)
					Me.outerInstance = outerInstance
				MyBase.New(messageList, JOptionPane.INFORMATION_MESSAGE, JOptionPane.DEFAULT_OPTION, Nothing, outerInstance.cancelOption, Nothing)
			End Sub


			Public Property Overrides maxCharactersPerLineCount As Integer
				Get
					Return 60
				End Get
			End Property


			' Equivalent to JOptionPane.createDialog,
			' but create a modeless dialog.
			' This is necessary because the Solaris implementation doesn't
			' support Dialog.setModal yet.
			Public Overrides Function createDialog(ByVal parentComponent As java.awt.Component, ByVal title As String) As JDialog
				Dim dialog As JDialog

				Dim window As java.awt.Window = JOptionPane.getWindowForComponent(parentComponent)
				If TypeOf window Is java.awt.Frame Then
					dialog = New JDialog(CType(window, java.awt.Frame), title, False)
				Else
					dialog = New JDialog(CType(window, java.awt.Dialog), title, False)
				End If
				If TypeOf window Is SwingUtilities.SharedOwnerFrame Then
					Dim ownerShutdownListener As java.awt.event.WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
					dialog.addWindowListener(ownerShutdownListener)
				End If
				Dim contentPane As java.awt.Container = dialog.contentPane

				contentPane.layout = New java.awt.BorderLayout
				contentPane.add(Me, java.awt.BorderLayout.CENTER)
				dialog.pack()
				dialog.locationRelativeTo = parentComponent
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				dialog.addWindowListener(New java.awt.event.WindowAdapter()
	'			{
	'				boolean gotFocus = False;
	'
	'				public void windowClosing(WindowEvent we)
	'				{
	'					setValue(cancelOption[0]);
	'				}
	'
	'				public void windowActivated(WindowEvent we)
	'				{
	'					' Once window gets focus, set initial focus
	'					if (!gotFocus)
	'					{
	'						selectInitialValue();
	'						gotFocus = True;
	'					}
	'				}
	'			});

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				addPropertyChangeListener(New java.beans.PropertyChangeListener()
	'			{
	'				public void propertyChange(PropertyChangeEvent event)
	'				{
	'					if(dialog.isVisible() && event.getSource() == ProgressOptionPane.this && (event.getPropertyName().equals(VALUE_PROPERTY) || event.getPropertyName().equals(INPUT_VALUE_PROPERTY)))
	'					{
	'						dialog.setVisible(False);
	'						dialog.dispose();
	'					}
	'				}
	'			});

				Return dialog
			End Function

			'///////////////
			' Accessibility support for ProgressOptionPane
			'//////////////

			''' <summary>
			''' Gets the AccessibleContext for the ProgressOptionPane
			''' </summary>
			''' <returns> the AccessibleContext for the ProgressOptionPane
			''' @since 1.5 </returns>
			Public Property Overrides accessibleContext As AccessibleContext
				Get
					Return outerInstance.accessibleContext
				End Get
			End Property

	'        
	'         * Returns the AccessibleJOptionPane
	'         
			Private Property accessibleJOptionPane As AccessibleContext
				Get
					Return MyBase.accessibleContext
				End Get
			End Property
		End Class


		''' <summary>
		''' Indicate the progress of the operation being monitored.
		''' If the specified value is &gt;= the maximum, the progress
		''' monitor is closed. </summary>
		''' <param name="nv"> an int specifying the current value, between the
		'''        maximum and minimum specified for this component </param>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= #close </seealso>
		Public Overridable Property progress As Integer
			Set(ByVal nv As Integer)
				If nv >= max Then
					close()
				Else
					If myBar IsNot Nothing Then
						myBar.value = nv
					Else
						Dim T As Long = System.currentTimeMillis()
						Dim dT As Long = CInt(T-T0)
						If dT >= millisToDecideToPopup Then
							Dim predictedCompletionTime As Integer
							If nv > min Then
								predictedCompletionTime = CInt(dT * (max - min) \ (nv - min))
							Else
								predictedCompletionTime = millisToPopup
							End If
							If predictedCompletionTime >= millisToPopup Then
								myBar = New JProgressBar
								myBar.minimum = min
								myBar.maximum = max
								myBar.value = nv
								If note IsNot Nothing Then noteLabel = New JLabel(note)
								pane = New ProgressOptionPane(Me, New Object() {message, noteLabel, myBar})
								dialog = pane.createDialog(parentComponent, UIManager.getString("ProgressMonitor.progressText"))
								dialog.show()
							End If
						End If
					End If
				End If
			End Set
		End Property


		''' <summary>
		''' Indicate that the operation is complete.  This happens automatically
		''' when the value set by setProgress is &gt;= max, but it may be called
		''' earlier if the operation ends early.
		''' </summary>
		Public Overridable Sub close()
			If dialog IsNot Nothing Then
				dialog.visible = False
				dialog.Dispose()
				dialog = Nothing
				pane = Nothing
				myBar = Nothing
			End If
		End Sub


		''' <summary>
		''' Returns the minimum value -- the lower end of the progress value.
		''' </summary>
		''' <returns> an int representing the minimum value </returns>
		''' <seealso cref= #setMinimum </seealso>
		Public Overridable Property minimum As Integer
			Get
				Return min
			End Get
			Set(ByVal m As Integer)
				If myBar IsNot Nothing Then myBar.minimum = m
				min = m
			End Set
		End Property




		''' <summary>
		''' Returns the maximum value -- the higher end of the progress value.
		''' </summary>
		''' <returns> an int representing the maximum value </returns>
		''' <seealso cref= #setMaximum </seealso>
		Public Overridable Property maximum As Integer
			Get
				Return max
			End Get
			Set(ByVal m As Integer)
				If myBar IsNot Nothing Then myBar.maximum = m
				max = m
			End Set
		End Property




		''' <summary>
		''' Returns true if the user hits the Cancel button in the progress dialog.
		''' </summary>
		Public Overridable Property canceled As Boolean
			Get
				If pane Is Nothing Then Return False
				Dim v As Object = pane.value
				Return ((v IsNot Nothing) AndAlso (cancelOption.Length = 1) AndAlso (v.Equals(cancelOption(0))))
			End Get
		End Property


		''' <summary>
		''' Specifies the amount of time to wait before deciding whether or
		''' not to popup a progress monitor.
		''' </summary>
		''' <param name="millisToDecideToPopup">  an int specifying the time to wait,
		'''        in milliseconds </param>
		''' <seealso cref= #getMillisToDecideToPopup </seealso>
		Public Overridable Property millisToDecideToPopup As Integer
			Set(ByVal millisToDecideToPopup As Integer)
				Me.millisToDecideToPopup = millisToDecideToPopup
			End Set
			Get
				Return millisToDecideToPopup
			End Get
		End Property




		''' <summary>
		''' Specifies the amount of time it will take for the popup to appear.
		''' (If the predicted time remaining is less than this time, the popup
		''' won't be displayed.)
		''' </summary>
		''' <param name="millisToPopup">  an int specifying the time in milliseconds </param>
		''' <seealso cref= #getMillisToPopup </seealso>
		Public Overridable Property millisToPopup As Integer
			Set(ByVal millisToPopup As Integer)
				Me.millisToPopup = millisToPopup
			End Set
			Get
				Return millisToPopup
			End Get
		End Property




		''' <summary>
		''' Specifies the additional note that is displayed along with the
		''' progress message. Used, for example, to show which file the
		''' is currently being copied during a multiple-file copy.
		''' </summary>
		''' <param name="note">  a String specifying the note to display </param>
		''' <seealso cref= #getNote </seealso>
		Public Overridable Property note As String
			Set(ByVal note As String)
				Me.note = note
				If noteLabel IsNot Nothing Then noteLabel.text = note
			End Set
			Get
				Return note
			End Get
		End Property



		'///////////////
		' Accessibility support
		'//////////////

		''' <summary>
		''' The <code>AccessibleContext</code> for the <code>ProgressMonitor</code>
		''' @since 1.5
		''' </summary>
		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		Private accessibleJOptionPane As AccessibleContext = Nothing

		''' <summary>
		''' Gets the <code>AccessibleContext</code> for the
		''' <code>ProgressMonitor</code>
		''' </summary>
		''' <returns> the <code>AccessibleContext</code> for the
		''' <code>ProgressMonitor</code>
		''' @since 1.5 </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleProgressMonitor(Me)
				If pane IsNot Nothing AndAlso accessibleJOptionPane Is Nothing Then
					' Notify the AccessibleProgressMonitor that the
					' ProgressOptionPane was created. It is necessary
					' to poll for ProgressOptionPane creation because
					' the ProgressMonitor does not have a Component
					' to add a listener to until the ProgressOptionPane
					' is created.
					If TypeOf ___accessibleContext Is AccessibleProgressMonitor Then CType(___accessibleContext, AccessibleProgressMonitor).optionPaneCreated()
				End If
				Return ___accessibleContext
			End Get
		End Property

		''' <summary>
		''' <code>AccessibleProgressMonitor</code> implements accessibility
		''' support for the <code>ProgressMonitor</code> class.
		''' @since 1.5
		''' </summary>
		Protected Friend Class AccessibleProgressMonitor
			Inherits AccessibleContext
			Implements AccessibleText, ChangeListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As ProgressMonitor


	'        
	'         * The accessibility hierarchy for ProgressMonitor is a flattened
	'         * version of the ProgressOptionPane component hierarchy.
	'         *
	'         * The ProgressOptionPane component hierarchy is:
	'         *   JDialog
	'         *     ProgressOptionPane
	'         *       JPanel
	'         *         JPanel
	'         *           JLabel
	'         *           JLabel
	'         *           JProgressBar
	'         *
	'         * The AccessibleProgessMonitor accessibility hierarchy is:
	'         *   AccessibleJDialog
	'         *     AccessibleProgressMonitor
	'         *       AccessibleJLabel
	'         *       AccessibleJLabel
	'         *       AccessibleJProgressBar
	'         *
	'         * The abstraction presented to assitive technologies by
	'         * the AccessibleProgressMonitor is that a dialog contains a
	'         * progress monitor with three children: a message, a note
	'         * label and a progress bar.
	'         

			Private oldModelValue As Object

			''' <summary>
			''' AccessibleProgressMonitor constructor
			''' </summary>
			Protected Friend Sub New(ByVal outerInstance As ProgressMonitor)
					Me.outerInstance = outerInstance
			End Sub

	'        
	'         * Initializes the AccessibleContext now that the ProgressOptionPane
	'         * has been created. Because the ProgressMonitor is not a Component
	'         * implementing the Accessible interface, an AccessibleContext
	'         * must be synthesized from the ProgressOptionPane and its children.
	'         *
	'         * For other AWT and Swing classes, the inner class that implements
	'         * accessibility for the class extends the inner class that implements
	'         * implements accessibility for the super class. AccessibleProgressMonitor
	'         * cannot extend AccessibleJOptionPane and must therefore delegate calls
	'         * to the AccessibleJOptionPane.
	'         
			Private Sub optionPaneCreated()
				outerInstance.accessibleJOptionPane = CType(outerInstance.pane, ProgressOptionPane).accessibleJOptionPane

				' add a listener for progress bar ChangeEvents
				If outerInstance.myBar IsNot Nothing Then outerInstance.myBar.addChangeListener(Me)

				' add a listener for note label PropertyChangeEvents
				If outerInstance.noteLabel IsNot Nothing Then outerInstance.noteLabel.addPropertyChangeListener(Me)
			End Sub

			''' <summary>
			''' Invoked when the target of the listener has changed its state.
			''' </summary>
			''' <param name="e">  a <code>ChangeEvent</code> object. Must not be null. </param>
			''' <exception cref="NullPointerException"> if the parameter is null. </exception>
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If e Is Nothing Then Return
				If outerInstance.myBar IsNot Nothing Then
					' the progress bar value changed
					Dim newModelValue As Object = outerInstance.myBar.value
					firePropertyChange(ACCESSIBLE_VALUE_PROPERTY, oldModelValue, newModelValue)
					oldModelValue = newModelValue
				End If
			End Sub

			''' <summary>
			''' This method gets called when a bound property is changed.
			''' </summary>
			''' <param name="e"> A <code>PropertyChangeEvent</code> object describing
			''' the event source and the property that has changed. Must not be null. </param>
			''' <exception cref="NullPointerException"> if the parameter is null. </exception>
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				If e.source Is outerInstance.noteLabel AndAlso e.propertyName = "text" Then firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, 0)
			End Sub

			' ===== Begin AccessileContext ===== 

			''' <summary>
			''' Gets the accessibleName property of this object.  The accessibleName
			''' property of an object is a localized String that designates the purpose
			''' of the object.  For example, the accessibleName property of a label
			''' or button might be the text of the label or button itself.  In the
			''' case of an object that doesn't display its name, the accessibleName
			''' should still be set.  For example, in the case of a text field used
			''' to enter the name of a city, the accessibleName for the en_US locale
			''' could be 'city.'
			''' </summary>
			''' <returns> the localized name of the object; null if this
			''' object does not have a name
			''' </returns>
			''' <seealso cref= #setAccessibleName </seealso>
			Public Property Overrides accessibleName As String
				Get
					If accessibleName IsNot Nothing Then ' defined in AccessibleContext
						Return accessibleName
					ElseIf outerInstance.accessibleJOptionPane IsNot Nothing Then
						' delegate to the AccessibleJOptionPane
						Return outerInstance.accessibleJOptionPane.accessibleName
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the accessibleDescription property of this object.  The
			''' accessibleDescription property of this object is a short localized
			''' phrase describing the purpose of the object.  For example, in the
			''' case of a 'Cancel' button, the accessibleDescription could be
			''' 'Ignore changes and close dialog box.'
			''' </summary>
			''' <returns> the localized description of the object; null if
			''' this object does not have a description
			''' </returns>
			''' <seealso cref= #setAccessibleDescription </seealso>
			Public Property Overrides accessibleDescription As String
				Get
					If accessibleDescription IsNot Nothing Then ' defined in AccessibleContext
						Return accessibleDescription
					ElseIf outerInstance.accessibleJOptionPane IsNot Nothing Then
						' delegate to the AccessibleJOptionPane
						Return outerInstance.accessibleJOptionPane.accessibleDescription
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the role of this object.  The role of the object is the generic
			''' purpose or use of the class of this object.  For example, the role
			''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
			''' AccessibleRole are provided so component developers can pick from
			''' a set of predefined roles.  This enables assistive technologies to
			''' provide a consistent interface to various tweaked subclasses of
			''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
			''' that act like a push button) as well as distinguish between subclasses
			''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
			''' and AccessibleRole.RADIO_BUTTON for radio buttons).
			''' <p>Note that the AccessibleRole class is also extensible, so
			''' custom component developers can define their own AccessibleRole's
			''' if the set of predefined roles is inadequate.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Property Overrides accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PROGRESS_MONITOR
				End Get
			End Property

			''' <summary>
			''' Gets the state set of this object.  The AccessibleStateSet of an object
			''' is composed of a set of unique AccessibleStates.  A change in the
			''' AccessibleStateSet of an object will cause a PropertyChangeEvent to
			''' be fired for the ACCESSIBLE_STATE_PROPERTY property.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the
			''' current state set of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState </seealso>
			''' <seealso cref= #addPropertyChangeListener </seealso>
			Public Property Overrides accessibleStateSet As AccessibleStateSet
				Get
					If outerInstance.accessibleJOptionPane IsNot Nothing Then Return outerInstance.accessibleJOptionPane.accessibleStateSet
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the Accessible parent of this object.
			''' </summary>
			''' <returns> the Accessible parent of this object; null if this
			''' object does not have an Accessible parent </returns>
			Public Property Overrides accessibleParent As Accessible
				Get
					Return outerInstance.dialog
				End Get
			End Property

	'        
	'         * Returns the parent AccessibleContext
	'         
			Private Property parentAccessibleContext As AccessibleContext
				Get
					If outerInstance.dialog IsNot Nothing Then Return outerInstance.dialog.accessibleContext
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the 0-based index of this object in its accessible parent.
			''' </summary>
			''' <returns> the 0-based index of this object in its parent; -1 if this
			''' object does not have an accessible parent.
			''' </returns>
			''' <seealso cref= #getAccessibleParent </seealso>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			''' <seealso cref= #getAccessibleChild </seealso>
			Public Property Overrides accessibleIndexInParent As Integer
				Get
					If outerInstance.accessibleJOptionPane IsNot Nothing Then Return outerInstance.accessibleJOptionPane.accessibleIndexInParent
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children of the object.
			''' </summary>
			''' <returns> the number of accessible children of the object. </returns>
			Public Property Overrides accessibleChildrenCount As Integer
				Get
					' return the number of children in the JPanel containing
					' the message, note label and progress bar
					Dim ac As AccessibleContext = panelAccessibleContext
					If ac IsNot Nothing Then Return ac.accessibleChildrenCount
					Return 0
				End Get
			End Property

			''' <summary>
			''' Returns the specified Accessible child of the object.  The Accessible
			''' children of an Accessible object are zero-based, so the first child
			''' of an Accessible child is at index 0, the second child is at index 1,
			''' and so on.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the Accessible child of the object </returns>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
				' return a child in the JPanel containing the message, note label
				' and progress bar
				Dim ac As AccessibleContext = panelAccessibleContext
				If ac IsNot Nothing Then Return ac.getAccessibleChild(i)
				Return Nothing
			End Function

	'        
	'         * Returns the AccessibleContext for the JPanel containing the
	'         * message, note label and progress bar
	'         
			Private Property panelAccessibleContext As AccessibleContext
				Get
					If outerInstance.myBar IsNot Nothing Then
						Dim c As java.awt.Component = outerInstance.myBar.parent
						If TypeOf c Is Accessible Then Return c.accessibleContext
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the locale of the component. If the component does not have a
			''' locale, then the locale of its parent is returned.
			''' </summary>
			''' <returns> this component's locale.  If this component does not have
			''' a locale, the locale of its parent is returned.
			''' </returns>
			''' <exception cref="IllegalComponentStateException">
			''' If the Component does not have its own locale and has not yet been
			''' added to a containment hierarchy such that the locale can be
			''' determined from the containing parent. </exception>
			Public Property Overrides locale As java.util.Locale
				Get
					If outerInstance.accessibleJOptionPane IsNot Nothing Then Return outerInstance.accessibleJOptionPane.locale
					Return Nothing
				End Get
			End Property

			' ===== end AccessibleContext ===== 

			''' <summary>
			''' Gets the AccessibleComponent associated with this object that has a
			''' graphical representation.
			''' </summary>
			''' <returns> AccessibleComponent if supported by object; else return null </returns>
			''' <seealso cref= AccessibleComponent </seealso>
			Public Property Overrides accessibleComponent As AccessibleComponent
				Get
					If outerInstance.accessibleJOptionPane IsNot Nothing Then Return outerInstance.accessibleJOptionPane.accessibleComponent
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the AccessibleValue associated with this object that supports a
			''' Numerical value.
			''' </summary>
			''' <returns> AccessibleValue if supported by object; else return null </returns>
			''' <seealso cref= AccessibleValue </seealso>
			Public Property Overrides accessibleValue As AccessibleValue
				Get
					If outerInstance.myBar IsNot Nothing Then Return outerInstance.myBar.accessibleContext.accessibleValue
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the AccessibleText associated with this object presenting
			''' text on the display.
			''' </summary>
			''' <returns> AccessibleText if supported by object; else return null </returns>
			''' <seealso cref= AccessibleText </seealso>
			Public Property Overrides accessibleText As AccessibleText
				Get
					If noteLabelAccessibleText IsNot Nothing Then Return Me
					Return Nothing
				End Get
			End Property

	'        
	'         * Returns the note label AccessibleText
	'         
			Private Property noteLabelAccessibleText As AccessibleText
				Get
					If outerInstance.noteLabel IsNot Nothing Then Return outerInstance.noteLabel.accessibleContext.accessibleText
					Return Nothing
				End Get
			End Property

			' ===== Begin AccessibleText impl ===== 

			''' <summary>
			''' Given a point in local coordinates, return the zero-based index
			''' of the character under that Point.  If the point is invalid,
			''' this method returns -1.
			''' </summary>
			''' <param name="p"> the Point in local coordinates </param>
			''' <returns> the zero-based index of the character under Point p; if
			''' Point is invalid return -1. </returns>
			Public Overridable Function getIndexAtPoint(ByVal p As java.awt.Point) As Integer
				Dim at As AccessibleText = noteLabelAccessibleText
				If at IsNot Nothing AndAlso sameWindowAncestor(outerInstance.pane, outerInstance.noteLabel) Then
					' convert point from the option pane bounds
					' to the note label bounds.
					Dim noteLabelPoint As java.awt.Point = SwingUtilities.convertPoint(outerInstance.pane, p, outerInstance.noteLabel)
					If noteLabelPoint IsNot Nothing Then Return at.getIndexAtPoint(noteLabelPoint)
				End If
				Return -1
			End Function

			''' <summary>
			''' Determines the bounding box of the character at the given
			''' index into the string.  The bounds are returned in local
			''' coordinates.  If the index is invalid an empty rectangle is returned.
			''' </summary>
			''' <param name="i"> the index into the String </param>
			''' <returns> the screen coordinates of the character's bounding box,
			''' if index is invalid return an empty rectangle. </returns>
			Public Overridable Function getCharacterBounds(ByVal i As Integer) As java.awt.Rectangle
				Dim at As AccessibleText = noteLabelAccessibleText
				If at IsNot Nothing AndAlso sameWindowAncestor(outerInstance.pane, outerInstance.noteLabel) Then
					' return rectangle in the option pane bounds
					Dim noteLabelRect As java.awt.Rectangle = at.getCharacterBounds(i)
					If noteLabelRect IsNot Nothing Then Return SwingUtilities.convertRectangle(outerInstance.noteLabel, noteLabelRect, outerInstance.pane)
				End If
				Return Nothing
			End Function

	'        
	'         * Returns whether source and destination components have the
	'         * same window ancestor
	'         
			Private Function sameWindowAncestor(ByVal src As java.awt.Component, ByVal dest As java.awt.Component) As Boolean
				If src Is Nothing OrElse dest Is Nothing Then Return False
				Return SwingUtilities.getWindowAncestor(src) Is SwingUtilities.getWindowAncestor(dest)
			End Function

			''' <summary>
			''' Returns the number of characters (valid indicies)
			''' </summary>
			''' <returns> the number of characters </returns>
			Public Overridable Property charCount As Integer Implements AccessibleText.getCharCount
				Get
					Dim at As AccessibleText = noteLabelAccessibleText
					If at IsNot Nothing Then ' JLabel contains HTML text Return at.charCount
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the zero-based offset of the caret.
			''' 
			''' Note: That to the right of the caret will have the same index
			''' value as the offset (the caret is between two characters). </summary>
			''' <returns> the zero-based offset of the caret. </returns>
			Public Overridable Property caretPosition As Integer Implements AccessibleText.getCaretPosition
				Get
					Dim at As AccessibleText = noteLabelAccessibleText
					If at IsNot Nothing Then ' JLabel contains HTML text Return at.caretPosition
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the String at a given index.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence </returns>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAtIndex
				Dim at As AccessibleText = noteLabelAccessibleText
				If at IsNot Nothing Then ' JLabel contains HTML text Return at.getAtIndex(part, index)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the String after a given index.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence </returns>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAfterIndex
				Dim at As AccessibleText = noteLabelAccessibleText
				If at IsNot Nothing Then ' JLabel contains HTML text Return at.getAfterIndex(part, index)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the String before a given index.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence </returns>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getBeforeIndex
				Dim at As AccessibleText = noteLabelAccessibleText
				If at IsNot Nothing Then ' JLabel contains HTML text Return at.getBeforeIndex(part, index)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the AttributeSet for a given character at a given index
			''' </summary>
			''' <param name="i"> the zero-based index into the text </param>
			''' <returns> the AttributeSet of the character </returns>
			Public Overridable Function getCharacterAttribute(ByVal i As Integer) As AttributeSet Implements AccessibleText.getCharacterAttribute
				Dim at As AccessibleText = noteLabelAccessibleText
				If at IsNot Nothing Then ' JLabel contains HTML text Return at.getCharacterAttribute(i)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the start offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the start of the selection </returns>
			Public Overridable Property selectionStart As Integer Implements AccessibleText.getSelectionStart
				Get
					Dim at As AccessibleText = noteLabelAccessibleText
					If at IsNot Nothing Then ' JLabel contains HTML text Return at.selectionStart
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the end offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the end of the selection </returns>
			Public Overridable Property selectionEnd As Integer Implements AccessibleText.getSelectionEnd
				Get
					Dim at As AccessibleText = noteLabelAccessibleText
					If at IsNot Nothing Then ' JLabel contains HTML text Return at.selectionEnd
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the portion of the text that is selected.
			''' </summary>
			''' <returns> the String portion of the text that is selected </returns>
			Public Overridable Property selectedText As String Implements AccessibleText.getSelectedText
				Get
					Dim at As AccessibleText = noteLabelAccessibleText
					If at IsNot Nothing Then ' JLabel contains HTML text Return at.selectedText
					Return Nothing
				End Get
			End Property
			' ===== End AccessibleText impl ===== 
		End Class
		' inner class AccessibleProgressMonitor

	End Class

End Namespace
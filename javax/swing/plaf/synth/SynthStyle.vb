Imports System
Imports System.Collections.Generic
Imports javax.swing

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth


	''' <summary>
	''' <code>SynthStyle</code> is a set of style properties.
	''' Each <code>SynthUI</code> references at least one
	''' <code>SynthStyle</code> that is obtained using a
	''' <code>SynthStyleFactory</code>. You typically don't need to interact with
	''' this class directly, rather you will load a
	''' <a href="doc-files/synthFileFormat.html">Synth File Format file</a> into
	''' <code>SynthLookAndFeel</code> that will create a set of SynthStyles.
	''' </summary>
	''' <seealso cref= SynthLookAndFeel </seealso>
	''' <seealso cref= SynthStyleFactory
	''' 
	''' @since 1.5
	''' @author Scott Violet </seealso>
	Public MustInherit Class SynthStyle
		''' <summary>
		''' Contains the default values for certain properties.
		''' </summary>
		Private Shared DEFAULT_VALUES As IDictionary(Of Object, Object)

		''' <summary>
		''' Shared SynthGraphics.
		''' </summary>
		Private Shared ReadOnly SYNTH_GRAPHICS As New SynthGraphicsUtils

		''' <summary>
		''' Adds the default values that we know about to DEFAULT_VALUES.
		''' </summary>
		Private Shared Sub populateDefaultValues()
			Dim buttonMap As Object = New UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" })
			DEFAULT_VALUES("Button.focusInputMap") = buttonMap
			DEFAULT_VALUES("CheckBox.focusInputMap") = buttonMap
			DEFAULT_VALUES("RadioButton.focusInputMap") = buttonMap
			DEFAULT_VALUES("ToggleButton.focusInputMap") = buttonMap
			DEFAULT_VALUES("SynthArrowButton.focusInputMap") = buttonMap
			DEFAULT_VALUES("List.dropLineColor") = Color.BLACK
			DEFAULT_VALUES("Tree.dropLineColor") = Color.BLACK
			DEFAULT_VALUES("Table.dropLineColor") = Color.BLACK
			DEFAULT_VALUES("Table.dropLineShortColor") = Color.RED

			Dim multilineInputMap As Object = New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "UP", javax.swing.text.DefaultEditorKit.upAction, "KP_UP", javax.swing.text.DefaultEditorKit.upAction, "DOWN", javax.swing.text.DefaultEditorKit.downAction, "KP_DOWN", javax.swing.text.DefaultEditorKit.downAction, "PAGE_UP", javax.swing.text.DefaultEditorKit.pageUpAction, "PAGE_DOWN", javax.swing.text.DefaultEditorKit.pageDownAction, "shift PAGE_UP", "selection-page-up", "shift PAGE_DOWN", "selection-page-down", "ctrl shift PAGE_UP", "selection-page-left", "ctrl shift PAGE_DOWN", "selection-page-right", "shift UP", javax.swing.text.DefaultEditorKit.selectionUpAction, "shift KP_UP", javax.swing.text.DefaultEditorKit.selectionUpAction, "shift DOWN", javax.swing.text.DefaultEditorKit.selectionDownAction, "shift KP_DOWN", javax.swing.text.DefaultEditorKit.selectionDownAction, "ENTER", javax.swing.text.DefaultEditorKit.insertBreakAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "TAB", javax.swing.text.DefaultEditorKit.insertTabAction, "ctrl BACK_SLASH", "unselect", "ctrl HOME", javax.swing.text.DefaultEditorKit.beginAction, "ctrl END", javax.swing.text.DefaultEditorKit.endAction, "ctrl shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginAction, "ctrl shift END", javax.swing.text.DefaultEditorKit.selectionEndAction, "ctrl T", "next-link-action", "ctrl shift T", "previous-link-action", "ctrl SPACE", "activate-link-action", "control shift O", "toggle-componentOrientation" }) 'DefaultEditorKit.toggleComponentOrientation - DefaultEditorKit.unselectAction
			DEFAULT_VALUES("EditorPane.focusInputMap") = multilineInputMap
			DEFAULT_VALUES("TextArea.focusInputMap") = multilineInputMap
			DEFAULT_VALUES("TextPane.focusInputMap") = multilineInputMap

			Dim fieldInputMap As Object = New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "ENTER", JTextField.notifyAction, "ctrl BACK_SLASH", "unselect", "control shift O", "toggle-componentOrientation" }) 'DefaultEditorKit.toggleComponentOrientation - DefaultEditorKit.unselectAction
			DEFAULT_VALUES("TextField.focusInputMap") = fieldInputMap
			DEFAULT_VALUES("PasswordField.focusInputMap") = fieldInputMap


			DEFAULT_VALUES("ComboBox.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "ESCAPE", "hidePopup", "PAGE_UP", "pageUpPassThrough", "PAGE_DOWN", "pageDownPassThrough", "HOME", "homePassThrough", "END", "endPassThrough", "DOWN", "selectNext", "KP_DOWN", "selectNext", "alt DOWN", "togglePopup", "alt KP_DOWN", "togglePopup", "alt UP", "togglePopup", "alt KP_UP", "togglePopup", "SPACE", "spacePopup", "ENTER", "enterPressed", "UP", "selectPrevious", "KP_UP", "selectPrevious" })

			DEFAULT_VALUES("Desktop.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "ctrl F5", "restore", "ctrl F4", "close", "ctrl F7", "move", "ctrl F8", "resize", "RIGHT", "right", "KP_RIGHT", "right", "shift RIGHT", "shrinkRight", "shift KP_RIGHT", "shrinkRight", "LEFT", "left", "KP_LEFT", "left", "shift LEFT", "shrinkLeft", "shift KP_LEFT", "shrinkLeft", "UP", "up", "KP_UP", "up", "shift UP", "shrinkUp", "shift KP_UP", "shrinkUp", "DOWN", "down", "KP_DOWN", "down", "shift DOWN", "shrinkDown", "shift KP_DOWN", "shrinkDown", "ESCAPE", "escape", "ctrl F9", "minimize", "ctrl F10", "maximize", "ctrl F6", "selectNextFrame", "ctrl TAB", "selectNextFrame", "ctrl alt F6", "selectNextFrame", "shift ctrl alt F6", "selectPreviousFrame", "ctrl F12", "navigateNext", "shift ctrl F12", "navigatePrevious" })

			DEFAULT_VALUES("FileChooser.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "ESCAPE", "cancelSelection", "F2", "editFileName", "F5", "refresh", "BACK_SPACE", "Go Up", "ENTER", "approveSelection", "ctrl ENTER", "approveSelection" })

			DEFAULT_VALUES("FormattedTextField.focusInputMap") = New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "ENTER", JTextField.notifyAction, "ctrl BACK_SLASH", "unselect", "control shift O", "toggle-componentOrientation", "ESCAPE", "reset-field-edit", "UP", "increment", "KP_UP", "increment", "DOWN", "decrement", "KP_DOWN", "decrement" })

			DEFAULT_VALUES("InternalFrame.icon") = LookAndFeel.makeIcon(GetType(javax.swing.plaf.basic.BasicLookAndFeel), "icons/JavaCup16.png")

			DEFAULT_VALUES("InternalFrame.windowBindings") = New Object() { "shift ESCAPE", "showSystemMenu", "ctrl SPACE", "showSystemMenu", "ESCAPE", "hideSystemMenu"}

			DEFAULT_VALUES("List.focusInputMap") = New UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "UP", "selectPreviousRow", "KP_UP", "selectPreviousRow", "shift UP", "selectPreviousRowExtendSelection", "shift KP_UP", "selectPreviousRowExtendSelection", "ctrl shift UP", "selectPreviousRowExtendSelection", "ctrl shift KP_UP", "selectPreviousRowExtendSelection", "ctrl UP", "selectPreviousRowChangeLead", "ctrl KP_UP", "selectPreviousRowChangeLead", "DOWN", "selectNextRow", "KP_DOWN", "selectNextRow", "shift DOWN", "selectNextRowExtendSelection", "shift KP_DOWN", "selectNextRowExtendSelection", "ctrl shift DOWN", "selectNextRowExtendSelection", "ctrl shift KP_DOWN", "selectNextRowExtendSelection", "ctrl DOWN", "selectNextRowChangeLead", "ctrl KP_DOWN", "selectNextRowChangeLead", "LEFT", "selectPreviousColumn", "KP_LEFT", "selectPreviousColumn", "shift LEFT", "selectPreviousColumnExtendSelection", "shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl shift LEFT", "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl LEFT", "selectPreviousColumnChangeLead", "ctrl KP_LEFT", "selectPreviousColumnChangeLead", "RIGHT", "selectNextColumn", "KP_RIGHT", "selectNextColumn", "shift RIGHT", "selectNextColumnExtendSelection", "shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl shift RIGHT", "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl RIGHT", "selectNextColumnChangeLead", "ctrl KP_RIGHT", "selectNextColumnChangeLead", "HOME", "selectFirstRow", "shift HOME", "selectFirstRowExtendSelection", "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME", "selectFirstRowChangeLead", "END", "selectLastRow", "shift END", "selectLastRowExtendSelection", "ctrl shift END", "selectLastRowExtendSelection", "ctrl END", "selectLastRowChangeLead", "PAGE_UP", "scrollUp", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollUpExtendSelection", "ctrl PAGE_UP", "scrollUpChangeLead", "PAGE_DOWN", "scrollDown", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl PAGE_DOWN", "scrollDownChangeLead", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo" })

			DEFAULT_VALUES("List.focusInputMap.RightToLeft") = New UIDefaults.LazyInputMap(New Object() { "LEFT", "selectNextColumn", "KP_LEFT", "selectNextColumn", "shift LEFT", "selectNextColumnExtendSelection", "shift KP_LEFT", "selectNextColumnExtendSelection", "ctrl shift LEFT", "selectNextColumnExtendSelection", "ctrl shift KP_LEFT", "selectNextColumnExtendSelection", "ctrl LEFT", "selectNextColumnChangeLead", "ctrl KP_LEFT", "selectNextColumnChangeLead", "RIGHT", "selectPreviousColumn", "KP_RIGHT", "selectPreviousColumn", "shift RIGHT", "selectPreviousColumnExtendSelection", "shift KP_RIGHT", "selectPreviousColumnExtendSelection", "ctrl shift RIGHT", "selectPreviousColumnExtendSelection", "ctrl shift KP_RIGHT", "selectPreviousColumnExtendSelection", "ctrl RIGHT", "selectPreviousColumnChangeLead", "ctrl KP_RIGHT", "selectPreviousColumnChangeLead" })

			DEFAULT_VALUES("MenuBar.windowBindings") = New Object() { "F10", "takeFocus" }

			DEFAULT_VALUES("OptionPane.windowBindings") = New Object() { "ESCAPE", "close" }

			DEFAULT_VALUES("RootPane.defaultButtonWindowKeyBindings") = New Object() { "ENTER", "press", "released ENTER", "release", "ctrl ENTER", "press", "ctrl released ENTER", "release" }

			DEFAULT_VALUES("RootPane.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "shift F10", "postPopup" })

			DEFAULT_VALUES("ScrollBar.anecstorInputMap") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "positiveUnitIncrement", "KP_RIGHT", "positiveUnitIncrement", "DOWN", "positiveUnitIncrement", "KP_DOWN", "positiveUnitIncrement", "PAGE_DOWN", "positiveBlockIncrement", "LEFT", "negativeUnitIncrement", "KP_LEFT", "negativeUnitIncrement", "UP", "negativeUnitIncrement", "KP_UP", "negativeUnitIncrement", "PAGE_UP", "negativeBlockIncrement", "HOME", "minScroll", "END", "maxScroll" })

			DEFAULT_VALUES("ScrollBar.ancestorInputMap.RightToLeft") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "negativeUnitIncrement", "KP_RIGHT", "negativeUnitIncrement", "LEFT", "positiveUnitIncrement", "KP_LEFT", "positiveUnitIncrement" })

			DEFAULT_VALUES("ScrollPane.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "unitScrollRight", "KP_RIGHT", "unitScrollRight", "DOWN", "unitScrollDown", "KP_DOWN", "unitScrollDown", "LEFT", "unitScrollLeft", "KP_LEFT", "unitScrollLeft", "UP", "unitScrollUp", "KP_UP", "unitScrollUp", "PAGE_UP", "scrollUp", "PAGE_DOWN", "scrollDown", "ctrl PAGE_UP", "scrollLeft", "ctrl PAGE_DOWN", "scrollRight", "ctrl HOME", "scrollHome", "ctrl END", "scrollEnd" })
			DEFAULT_VALUES("ScrollPane.ancestorInputMap.RightToLeft") = New UIDefaults.LazyInputMap(New Object() { "ctrl PAGE_UP", "scrollRight", "ctrl PAGE_DOWN", "scrollLeft" })

			DEFAULT_VALUES("SplitPane.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "UP", "negativeIncrement", "DOWN", "positiveIncrement", "LEFT", "negativeIncrement", "RIGHT", "positiveIncrement", "KP_UP", "negativeIncrement", "KP_DOWN", "positiveIncrement", "KP_LEFT", "negativeIncrement", "KP_RIGHT", "positiveIncrement", "HOME", "selectMin", "END", "selectMax", "F8", "startResize", "F6", "toggleFocus", "ctrl TAB", "focusOutForward", "ctrl shift TAB", "focusOutBackward" })

			DEFAULT_VALUES("Spinner.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "UP", "increment", "KP_UP", "increment", "DOWN", "decrement", "KP_DOWN", "decrement" })

			DEFAULT_VALUES("Slider.focusInputMap") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "positiveUnitIncrement", "KP_RIGHT", "positiveUnitIncrement", "DOWN", "negativeUnitIncrement", "KP_DOWN", "negativeUnitIncrement", "PAGE_DOWN", "negativeBlockIncrement", "ctrl PAGE_DOWN", "negativeBlockIncrement", "LEFT", "negativeUnitIncrement", "KP_LEFT", "negativeUnitIncrement", "UP", "positiveUnitIncrement", "KP_UP", "positiveUnitIncrement", "PAGE_UP", "positiveBlockIncrement", "ctrl PAGE_UP", "positiveBlockIncrement", "HOME", "minScroll", "END", "maxScroll" })

			DEFAULT_VALUES("Slider.focusInputMap.RightToLeft") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "negativeUnitIncrement", "KP_RIGHT", "negativeUnitIncrement", "LEFT", "positiveUnitIncrement", "KP_LEFT", "positiveUnitIncrement" })

			DEFAULT_VALUES("TabbedPane.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "ctrl PAGE_DOWN", "navigatePageDown", "ctrl PAGE_UP", "navigatePageUp", "ctrl UP", "requestFocus", "ctrl KP_UP", "requestFocus" })

			DEFAULT_VALUES("TabbedPane.focusInputMap") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "navigateRight", "KP_RIGHT", "navigateRight", "LEFT", "navigateLeft", "KP_LEFT", "navigateLeft", "UP", "navigateUp", "KP_UP", "navigateUp", "DOWN", "navigateDown", "KP_DOWN", "navigateDown", "ctrl DOWN", "requestFocusForVisibleComponent", "ctrl KP_DOWN", "requestFocusForVisibleComponent" })

			DEFAULT_VALUES("Table.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "RIGHT", "selectNextColumn", "KP_RIGHT", "selectNextColumn", "shift RIGHT", "selectNextColumnExtendSelection", "shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl shift RIGHT", "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl RIGHT", "selectNextColumnChangeLead", "ctrl KP_RIGHT", "selectNextColumnChangeLead", "LEFT", "selectPreviousColumn", "KP_LEFT", "selectPreviousColumn", "shift LEFT", "selectPreviousColumnExtendSelection", "shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl shift LEFT", "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl LEFT", "selectPreviousColumnChangeLead", "ctrl KP_LEFT", "selectPreviousColumnChangeLead", "DOWN", "selectNextRow", "KP_DOWN", "selectNextRow", "shift DOWN", "selectNextRowExtendSelection", "shift KP_DOWN", "selectNextRowExtendSelection", "ctrl shift DOWN", "selectNextRowExtendSelection", "ctrl shift KP_DOWN", "selectNextRowExtendSelection", "ctrl DOWN", "selectNextRowChangeLead", "ctrl KP_DOWN", "selectNextRowChangeLead", "UP", "selectPreviousRow", "KP_UP", "selectPreviousRow", "shift UP", "selectPreviousRowExtendSelection", "shift KP_UP", "selectPreviousRowExtendSelection", "ctrl shift UP", "selectPreviousRowExtendSelection", "ctrl shift KP_UP", "selectPreviousRowExtendSelection", "ctrl UP", "selectPreviousRowChangeLead", "ctrl KP_UP", "selectPreviousRowChangeLead", "HOME", "selectFirstColumn", "shift HOME", "selectFirstColumnExtendSelection", "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME", "selectFirstRow", "END", "selectLastColumn", "shift END", "selectLastColumnExtendSelection", "ctrl shift END", "selectLastRowExtendSelection", "ctrl END", "selectLastRow", "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollLeftExtendSelection", "ctrl PAGE_UP", "scrollLeftChangeSelection", "PAGE_DOWN", "scrollDownChangeSelection", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollRightExtendSelection", "ctrl PAGE_DOWN", "scrollRightChangeSelection", "TAB", "selectNextColumnCell", "shift TAB", "selectPreviousColumnCell", "ENTER", "selectNextRowCell", "shift ENTER", "selectPreviousRowCell", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "ESCAPE", "cancel", "F2", "startEditing", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo", "F8", "focusHeader" })

		   DEFAULT_VALUES("TableHeader.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "SPACE", "toggleSortOrder", "LEFT", "selectColumnToLeft", "KP_LEFT", "selectColumnToLeft", "RIGHT", "selectColumnToRight", "KP_RIGHT", "selectColumnToRight", "alt LEFT", "moveColumnLeft", "alt KP_LEFT", "moveColumnLeft", "alt RIGHT", "moveColumnRight", "alt KP_RIGHT", "moveColumnRight", "alt shift LEFT", "resizeLeft", "alt shift KP_LEFT", "resizeLeft", "alt shift RIGHT", "resizeRight", "alt shift KP_RIGHT", "resizeRight", "ESCAPE", "focusTable" })

			DEFAULT_VALUES("Tree.ancestorInputMap") = New UIDefaults.LazyInputMap(New Object() { "ESCAPE", "cancel" })
			DEFAULT_VALUES("Tree.focusInputMap") = New UIDefaults.LazyInputMap(New Object() { "ADD", "expand", "SUBTRACT", "collapse", "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "UP", "selectPrevious", "KP_UP", "selectPrevious", "shift UP", "selectPreviousExtendSelection", "shift KP_UP", "selectPreviousExtendSelection", "ctrl shift UP", "selectPreviousExtendSelection", "ctrl shift KP_UP", "selectPreviousExtendSelection", "ctrl UP", "selectPreviousChangeLead", "ctrl KP_UP", "selectPreviousChangeLead", "DOWN", "selectNext", "KP_DOWN", "selectNext", "shift DOWN", "selectNextExtendSelection", "shift KP_DOWN", "selectNextExtendSelection", "ctrl shift DOWN", "selectNextExtendSelection", "ctrl shift KP_DOWN", "selectNextExtendSelection", "ctrl DOWN", "selectNextChangeLead", "ctrl KP_DOWN", "selectNextChangeLead", "RIGHT", "selectChild", "KP_RIGHT", "selectChild", "LEFT", "selectParent", "KP_LEFT", "selectParent", "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollUpExtendSelection", "ctrl PAGE_UP", "scrollUpChangeLead", "PAGE_DOWN", "scrollDownChangeSelection", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl PAGE_DOWN", "scrollDownChangeLead", "HOME", "selectFirst", "shift HOME", "selectFirstExtendSelection", "ctrl shift HOME", "selectFirstExtendSelection", "ctrl HOME", "selectFirstChangeLead", "END", "selectLast", "shift END", "selectLastExtendSelection", "ctrl shift END", "selectLastExtendSelection", "ctrl END", "selectLastChangeLead", "F2", "startEditing", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "ctrl LEFT", "scrollLeft", "ctrl KP_LEFT", "scrollLeft", "ctrl RIGHT", "scrollRight", "ctrl KP_RIGHT", "scrollRight", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo" })
			DEFAULT_VALUES("Tree.focusInputMap.RightToLeft") = New UIDefaults.LazyInputMap(New Object() { "RIGHT", "selectParent", "KP_RIGHT", "selectParent", "LEFT", "selectChild", "KP_LEFT", "selectChild" })
		End Sub

		''' <summary>
		''' Returns the default value for the specified property, or null if there
		''' is no default for the specified value.
		''' </summary>
		Private Shared Function getDefaultValue(ByVal key As Object) As Object
			SyncLock GetType(SynthStyle)
				If DEFAULT_VALUES Is Nothing Then
					DEFAULT_VALUES = New Dictionary(Of Object, Object)
					populateDefaultValues()
				End If
				Dim value As Object = DEFAULT_VALUES(key)
				If TypeOf value Is UIDefaults.LazyValue Then
					value = CType(value, UIDefaults.LazyValue).createValue(Nothing)
					DEFAULT_VALUES(key) = value
				End If
				Return value
			End SyncLock
		End Function

		''' <summary>
		''' Constructs a SynthStyle.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the <code>SynthGraphicUtils</code> for the specified context.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <returns> SynthGraphicsUtils </returns>
		Public Overridable Function getGraphicsUtils(ByVal context As SynthContext) As SynthGraphicsUtils
			Return SYNTH_GRAPHICS
		End Function

		''' <summary>
		''' Returns the color for the specified state. This gives precedence to
		''' foreground and background of the <code>JComponent</code>. If the
		''' <code>Color</code> from the <code>JComponent</code> is not appropriate,
		''' or not used, this will invoke <code>getColorForState</code>. Subclasses
		''' should generally not have to override this, instead override
		''' <seealso cref="#getColorForState"/>.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="type"> Type of color being requested. </param>
		''' <returns> Color </returns>
		Public Overridable Function getColor(ByVal context As SynthContext, ByVal type As ColorType) As Color
			Dim c As JComponent = context.component
			Dim id As Region = context.region

			If (context.componentState And SynthConstants.DISABLED) <> 0 Then
				'This component is disabled, so return the disabled color.
				'In some cases this means ignoring the color specified by the
				'developer on the component. In other cases it means using a
				'specified disabledTextColor, such as on JTextComponents.
				'For example, JLabel doesn't specify a disabled color that the
				'developer can set, yet it should have a disabled color to the
				'text when the label is disabled. This code allows for that.
				If TypeOf c Is javax.swing.text.JTextComponent Then
					Dim txt As javax.swing.text.JTextComponent = CType(c, javax.swing.text.JTextComponent)
					Dim disabledColor As Color = txt.disabledTextColor
					If disabledColor Is Nothing OrElse TypeOf disabledColor Is javax.swing.plaf.UIResource Then Return getColorForState(context, type)
				ElseIf TypeOf c Is JLabel AndAlso (type Is ColorType.FOREGROUND OrElse type Is ColorType.TEXT_FOREGROUND) Then
					Return getColorForState(context, type)
				End If
			End If

			' If the developer has specified a color, prefer it. Otherwise, get
			' the color for the state.
			Dim ___color As Color = Nothing
			If Not id.subregion Then
				If type Is ColorType.BACKGROUND Then
					___color = c.background
				ElseIf type Is ColorType.FOREGROUND Then
					___color = c.foreground
				ElseIf type Is ColorType.TEXT_FOREGROUND Then
					___color = c.foreground
				End If
			End If

			If ___color Is Nothing OrElse TypeOf ___color Is javax.swing.plaf.UIResource Then ___color = getColorForState(context, type)

			If ___color Is Nothing Then
				' No color, fallback to that of the widget.
				If type Is ColorType.BACKGROUND OrElse type Is ColorType.TEXT_BACKGROUND Then
					Return c.background
				ElseIf type Is ColorType.FOREGROUND OrElse type Is ColorType.TEXT_FOREGROUND Then
					Return c.foreground
				End If
			End If
			Return ___color
		End Function

		''' <summary>
		''' Returns the color for the specified state. This should NOT call any
		''' methods on the <code>JComponent</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="type"> Type of color being requested. </param>
		''' <returns> Color to render with </returns>
		Protected Friend MustOverride Function getColorForState(ByVal context As SynthContext, ByVal type As ColorType) As Color

		''' <summary>
		''' Returns the Font for the specified state. This redirects to the
		''' <code>JComponent</code> from the <code>context</code> as necessary.
		''' If this does not redirect
		''' to the JComponent <seealso cref="#getFontForState"/> is invoked.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <returns> Font to render with </returns>
		Public Overridable Function getFont(ByVal context As SynthContext) As Font
			Dim c As JComponent = context.component
			If context.componentState = SynthConstants.ENABLED Then Return c.font
			Dim cFont As Font = c.font
			If cFont IsNot Nothing AndAlso Not(TypeOf cFont Is javax.swing.plaf.UIResource) Then Return cFont
			Return getFontForState(context)
		End Function

		''' <summary>
		''' Returns the font for the specified state. This should NOT call any
		''' method on the <code>JComponent</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <returns> Font to render with </returns>
		Protected Friend MustOverride Function getFontForState(ByVal context As SynthContext) As Font

		''' <summary>
		''' Returns the Insets that are used to calculate sizing information.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="insets"> Insets to place return value in. </param>
		''' <returns> Sizing Insets. </returns>
		Public Overridable Function getInsets(ByVal context As SynthContext, ByVal insets As Insets) As Insets
			If insets Is Nothing Then insets = New Insets(0, 0, 0, 0)
				insets.right = 0
					insets.left = insets.right
						insets.bottom = insets.left
						insets.top = insets.bottom
			Return insets
		End Function

		''' <summary>
		''' Returns the <code>SynthPainter</code> that will be used for painting.
		''' This may return null.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <returns> SynthPainter to use </returns>
		Public Overridable Function getPainter(ByVal context As SynthContext) As SynthPainter
			Return Nothing
		End Function

		''' <summary>
		''' Returns true if the region is opaque.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <returns> true if region is opaque. </returns>
		Public Overridable Function isOpaque(ByVal context As SynthContext) As Boolean
			Return True
		End Function

		''' <summary>
		''' Getter for a region specific style property.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="key"> Property being requested. </param>
		''' <returns> Value of the named property </returns>
		Public Overridable Function [get](ByVal context As SynthContext, ByVal key As Object) As Object
			Return getDefaultValue(key)
		End Function

		Friend Overridable Sub installDefaults(ByVal context As SynthContext, ByVal ui As SynthUI)
			' Special case the Border as this will likely change when the LAF
			' can have more control over this.
			If Not context.subregion Then
				Dim c As JComponent = context.component
				Dim border As javax.swing.border.Border = c.border

				If border Is Nothing OrElse TypeOf border Is javax.swing.plaf.UIResource Then c.border = New SynthBorder(ui, getInsets(context, Nothing))
			End If
			installDefaults(context)
		End Sub

		''' <summary>
		''' Installs the necessary state from this Style on the
		''' <code>JComponent</code> from <code>context</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying component to install properties
		'''        to. </param>
		Public Overridable Sub installDefaults(ByVal context As SynthContext)
			If Not context.subregion Then
				Dim c As JComponent = context.component
				Dim ___region As Region = context.region
				Dim ___font As Font = c.font

				If ___font Is Nothing OrElse (TypeOf ___font Is javax.swing.plaf.UIResource) Then c.font = getFontForState(context)
				Dim background As Color = c.background
				If background Is Nothing OrElse (TypeOf background Is javax.swing.plaf.UIResource) Then c.background = getColorForState(context, ColorType.BACKGROUND)
				Dim foreground As Color = c.foreground
				If foreground Is Nothing OrElse (TypeOf foreground Is javax.swing.plaf.UIResource) Then c.foreground = getColorForState(context, ColorType.FOREGROUND)
				LookAndFeel.installProperty(c, "opaque", Convert.ToBoolean(isOpaque(context)))
			End If
		End Sub

		''' <summary>
		''' Uninstalls any state that this style installed on
		''' the <code>JComponent</code> from <code>context</code>.
		''' <p>
		''' Styles should NOT depend upon this being called, in certain cases
		''' it may never be called.
		''' </summary>
		''' <param name="context"> SynthContext identifying component to install properties
		'''        to. </param>
		Public Overridable Sub uninstallDefaults(ByVal context As SynthContext)
			If Not context.subregion Then
				' NOTE: because getForeground, getBackground and getFont will look
				' at the parent Container, if we set them to null it may
				' mean we they return a non-null and non-UIResource value
				' preventing install from correctly settings its colors/font. For
				' this reason we do not uninstall the fg/bg/font.

				Dim c As JComponent = context.component
				Dim border As javax.swing.border.Border = c.border

				If TypeOf border Is javax.swing.plaf.UIResource Then c.border = Nothing
			End If
		End Sub

		''' <summary>
		''' Convenience method to get a specific style property whose value is
		''' a <code>Number</code>. If the value is a <code>Number</code>,
		''' <code>intValue</code> is returned, otherwise <code>defaultValue</code>
		''' is returned.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="key"> Property being requested. </param>
		''' <param name="defaultValue"> Value to return if the property has not been
		'''        specified, or is not a Number </param>
		''' <returns> Value of the named property </returns>
		Public Overridable Function getInt(ByVal context As SynthContext, ByVal key As Object, ByVal defaultValue As Integer) As Integer
			Dim value As Object = [get](context, key)

			If TypeOf value Is Number Then Return CType(value, Number)
			Return defaultValue
		End Function

		''' <summary>
		''' Convenience method to get a specific style property whose value is
		''' an Boolean.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="key"> Property being requested. </param>
		''' <param name="defaultValue"> Value to return if the property has not been
		'''        specified, or is not a Boolean </param>
		''' <returns> Value of the named property </returns>
		Public Overridable Function getBoolean(ByVal context As SynthContext, ByVal key As Object, ByVal defaultValue As Boolean) As Boolean
			Dim value As Object = [get](context, key)

			If TypeOf value Is Boolean? Then Return CBool(value)
			Return defaultValue
		End Function

		''' <summary>
		''' Convenience method to get a specific style property whose value is
		''' an Icon.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="key"> Property being requested. </param>
		''' <returns> Value of the named property, or null if not specified </returns>
		Public Overridable Function getIcon(ByVal context As SynthContext, ByVal key As Object) As Icon
			Dim value As Object = [get](context, key)

			If TypeOf value Is Icon Then Return CType(value, Icon)
			Return Nothing
		End Function

		''' <summary>
		''' Convenience method to get a specific style property whose value is
		''' a String.
		''' </summary>
		''' <param name="context"> SynthContext identifying requester </param>
		''' <param name="key"> Property being requested. </param>
		''' <param name="defaultValue"> Value to return if the property has not been
		'''        specified, or is not a String </param>
		''' <returns> Value of the named property </returns>
		Public Overridable Function getString(ByVal context As SynthContext, ByVal key As Object, ByVal defaultValue As String) As String
			Dim value As Object = [get](context, key)

			If TypeOf value Is String Then Return CStr(value)
			Return defaultValue
		End Function
	End Class

End Namespace
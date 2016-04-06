Imports System

'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright IBM Corp. 1998 - All Rights Reserved
' *
' * The original version of this source code and documentation is copyrighted
' * and owned by IBM, Inc. These materials are provided under terms of a
' * License Agreement between IBM and Sun. This technology is protected by
' * multiple US and International patents. This notice and attribution to IBM
' * may not be removed.
' *
' 

Namespace java.awt


	''' <summary>
	''' The ComponentOrientation class encapsulates the language-sensitive
	''' orientation that is to be used to order the elements of a component
	''' or of text. It is used to reflect the differences in this ordering
	''' between Western alphabets, Middle Eastern (such as Hebrew), and Far
	''' Eastern (such as Japanese).
	''' <p>
	''' Fundamentally, this governs items (such as characters) which are laid out
	''' in lines, with the lines then laid out in a block. This also applies
	''' to items in a widget: for example, in a check box where the box is
	''' positioned relative to the text.
	''' <p>
	''' There are four different orientations used in modern languages
	''' as in the following table.<br>
	''' <pre>
	''' LT          RT          TL          TR
	''' A B C       C B A       A D G       G D A
	''' D E F       F E D       B E H       H E B
	''' G H I       I H G       C F I       I F C
	''' </pre><br>
	''' (In the header, the two-letter abbreviation represents the item direction
	''' in the first letter, and the line direction in the second. For example,
	''' LT means "items left-to-right, lines top-to-bottom",
	''' TL means "items top-to-bottom, lines left-to-right", and so on.)
	''' <p>
	''' The orientations are:
	''' <ul>
	''' <li>LT - Western Europe (optional for Japanese, Chinese, Korean)
	''' <li>RT - Middle East (Arabic, Hebrew)
	''' <li>TR - Japanese, Chinese, Korean
	''' <li>TL - Mongolian
	''' </ul>
	''' Components whose view and controller code depends on orientation
	''' should use the <code>isLeftToRight()</code> and
	''' <code>isHorizontal()</code> methods to
	''' determine their behavior. They should not include switch-like
	''' code that keys off of the constants, such as:
	''' <pre>
	''' if (orientation == LEFT_TO_RIGHT) {
	'''   ...
	''' } else if (orientation == RIGHT_TO_LEFT) {
	'''   ...
	''' } else {
	'''   // Oops
	''' }
	''' </pre>
	''' This is unsafe, since more constants may be added in the future and
	''' since it is not guaranteed that orientation objects will be unique.
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ComponentOrientation
	'    
	'     * serialVersionUID
	'     
		Private Const serialVersionUID As Long = -4113291392143563828L

		' Internal constants used in the implementation
		Private Const UNK_BIT As Integer = 1
		Private Const HORIZ_BIT As Integer = 2
		Private Const LTR_BIT As Integer = 4

		''' <summary>
		''' Items run left to right and lines flow top to bottom
		''' Examples: English, French.
		''' </summary>
		Public Shared ReadOnly LEFT_TO_RIGHT As New ComponentOrientation(HORIZ_BIT Or LTR_BIT)

		''' <summary>
		''' Items run right to left and lines flow top to bottom
		''' Examples: Arabic, Hebrew.
		''' </summary>
		Public Shared ReadOnly RIGHT_TO_LEFT As New ComponentOrientation(HORIZ_BIT)

		''' <summary>
		''' Indicates that a component's orientation has not been set.
		''' To preserve the behavior of existing applications,
		''' isLeftToRight will return true for this value.
		''' </summary>
		Public Shared ReadOnly UNKNOWN As New ComponentOrientation(HORIZ_BIT Or LTR_BIT Or UNK_BIT)

        ''' <summary>
        ''' Are lines horizontal?
        ''' This will return true for horizontal, left-to-right writing
        ''' systems such as Roman.
        ''' </summary>
        Public ReadOnly Property horizontal As Boolean
            Get
                Return (orientation And HORIZ_BIT) <> 0
            End Get
        End Property

        ''' <summary>
        ''' HorizontalLines: Do items run left-to-right?<br>
        ''' Vertical Lines:  Do lines run left-to-right?<br>
        ''' This will return true for horizontal, left-to-right writing
        ''' systems such as Roman.
        ''' </summary>
        Public ReadOnly Property leftToRight As Boolean
            Get
                Return (orientation And LTR_BIT) <> 0
            End Get
        End Property

        ''' <summary>
        ''' Returns the orientation that is appropriate for the given locale. </summary>
        ''' <param name="locale"> the specified locale </param>
        Public Shared Function getOrientation(  locale As java.util.Locale) As ComponentOrientation
			' A more flexible implementation would consult a ResourceBundle
			' to find the appropriate orientation.  Until pluggable locales
			' are introduced however, the flexiblity isn't really needed.
			' So we choose efficiency instead.
			Dim lang As String = locale.language
			If "iw".Equals(lang) OrElse "ar".Equals(lang) OrElse "fa".Equals(lang) OrElse "ur".Equals(lang) Then
				Return RIGHT_TO_LEFT
			Else
				Return LEFT_TO_RIGHT
			End If
		End Function

        ''' <summary>
        ''' Returns the orientation appropriate for the given ResourceBundle's
        ''' localization.  Three approaches are tried, in the following order:
        ''' <ol>
        ''' <li>Retrieve a ComponentOrientation object from the ResourceBundle
        '''      using the string "Orientation" as the key.
        ''' <li>Use the ResourceBundle.getLocale to determine the bundle's
        '''      locale, then return the orientation for that locale.
        ''' <li>Return the default locale's orientation.
        ''' </ol>
        ''' </summary>
        ''' @deprecated As of J2SE 1.4, use <seealso cref="#getOrientation(java.util.Locale)"/>. 
        <Obsolete("As of J2SE 1.4, use <seealso cref=""#getOrientation(java.util.Locale)""/>.")>
        Public Shared Function getOrientation(  bdl As java.util.ResourceBundle) As ComponentOrientation
			Dim result As ComponentOrientation = Nothing

			Try
				result = CType(bdl.getObject("Orientation"), ComponentOrientation)
			Catch e As Exception
			End Try

			If result Is Nothing Then result = getOrientation(bdl.locale)
			If result Is Nothing Then result = getOrientation(java.util.Locale.default)
			Return result
		End Function

		Private orientation As Integer

		Private Sub New(  value As Integer)
			orientation = value
		End Sub
	End Class

End Namespace
'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' <P>Class AccessibleRelation describes a relation between the
	''' object that implements the AccessibleRelation and one or more other
	''' objects.  The actual relations that an object has with other
	''' objects are defined as an AccessibleRelationSet, which is a composed
	''' set of AccessibleRelations.
	''' <p>The toDisplayString method allows you to obtain the localized string
	''' for a locale independent key from a predefined ResourceBundle for the
	''' keys defined in this class.
	''' <p>The constants in this class present a strongly typed enumeration
	''' of common object roles. If the constants in this class are not sufficient
	''' to describe the role of an object, a subclass should be generated
	''' from this class and it should provide constants in a similar manner.
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.3
	''' </summary>
	Public Class AccessibleRelation
		Inherits AccessibleBundle

	'    
	'     * The group of objects that participate in the relation.
	'     * The relation may be one-to-one or one-to-many.  For
	'     * example, in the case of a LABEL_FOR relation, the target
	'     * vector would contain a list of objects labeled by the object
	'     * that implements this AccessibleRelation.  In the case of a
	'     * MEMBER_OF relation, the target vector would contain all
	'     * of the components that are members of the same group as the
	'     * object that implements this AccessibleRelation.
	'     
		Private target As Object() = New Object(){}

		''' <summary>
		''' Indicates an object is a label for one or more target objects.
		''' </summary>
		''' <seealso cref= #getTarget </seealso>
		''' <seealso cref= #CONTROLLER_FOR </seealso>
		''' <seealso cref= #CONTROLLED_BY </seealso>
		''' <seealso cref= #LABELED_BY </seealso>
		''' <seealso cref= #MEMBER_OF </seealso>
		Public Shared ReadOnly LABEL_FOR As New String("labelFor")

		''' <summary>
		''' Indicates an object is labeled by one or more target objects.
		''' </summary>
		''' <seealso cref= #getTarget </seealso>
		''' <seealso cref= #CONTROLLER_FOR </seealso>
		''' <seealso cref= #CONTROLLED_BY </seealso>
		''' <seealso cref= #LABEL_FOR </seealso>
		''' <seealso cref= #MEMBER_OF </seealso>
		Public Shared ReadOnly LABELED_BY As New String("labeledBy")

		''' <summary>
		''' Indicates an object is a member of a group of one or more
		''' target objects.
		''' </summary>
		''' <seealso cref= #getTarget </seealso>
		''' <seealso cref= #CONTROLLER_FOR </seealso>
		''' <seealso cref= #CONTROLLED_BY </seealso>
		''' <seealso cref= #LABEL_FOR </seealso>
		''' <seealso cref= #LABELED_BY </seealso>
		Public Shared ReadOnly MEMBER_OF As New String("memberOf")

		''' <summary>
		''' Indicates an object is a controller for one or more target
		''' objects.
		''' </summary>
		''' <seealso cref= #getTarget </seealso>
		''' <seealso cref= #CONTROLLED_BY </seealso>
		''' <seealso cref= #LABEL_FOR </seealso>
		''' <seealso cref= #LABELED_BY </seealso>
		''' <seealso cref= #MEMBER_OF </seealso>
		Public Shared ReadOnly CONTROLLER_FOR As New String("controllerFor")

		''' <summary>
		''' Indicates an object is controlled by one or more target
		''' objects.
		''' </summary>
		''' <seealso cref= #getTarget </seealso>
		''' <seealso cref= #CONTROLLER_FOR </seealso>
		''' <seealso cref= #LABEL_FOR </seealso>
		''' <seealso cref= #LABELED_BY </seealso>
		''' <seealso cref= #MEMBER_OF </seealso>
		Public Shared ReadOnly CONTROLLED_BY As New String("controlledBy")

		''' <summary>
		''' Indicates an object is logically contiguous with a second
		''' object where the second object occurs after the object.
		''' An example is a paragraph of text that runs to the end of
		''' a page and continues on the next page with an intervening
		''' text footer and/or text header.  The two parts of
		''' the paragraph are separate text elements but are related
		''' in that the second element is a continuation
		''' of the first
		''' element.  In other words, the first element "flows to"
		''' the second element.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const FLOWS_TO As String = "flowsTo"

		''' <summary>
		''' Indicates an object is logically contiguous with a second
		''' object where the second object occurs before the object.
		''' An example is a paragraph of text that runs to the end of
		''' a page and continues on the next page with an intervening
		''' text footer and/or text header.  The two parts of
		''' the paragraph are separate text elements but are related
		''' in that the second element is a continuation of the first
		''' element.  In other words, the second element "flows from"
		''' the second element.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const FLOWS_FROM As String = "flowsFrom"

		''' <summary>
		''' Indicates that an object is a subwindow of one or more
		''' objects.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const SUBWINDOW_OF As String = "subwindowOf"

		''' <summary>
		''' Indicates that an object is a parent window of one or more
		''' objects.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const PARENT_WINDOW_OF As String = "parentWindowOf"

		''' <summary>
		''' Indicates that an object has one or more objects
		''' embedded in it.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const EMBEDS As String = "embeds"

		''' <summary>
		''' Indicates that an object is embedded in one or more
		''' objects.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const EMBEDDED_BY As String = "embeddedBy"

		''' <summary>
		''' Indicates that an object is a child node of one
		''' or more objects.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const CHILD_NODE_OF As String = "childNodeOf"

		''' <summary>
		''' Identifies that the target group for a label has changed
		''' </summary>
		Public Const LABEL_FOR_PROPERTY As String = "labelForProperty"

		''' <summary>
		''' Identifies that the objects that are doing the labeling have changed
		''' </summary>
		Public Const LABELED_BY_PROPERTY As String = "labeledByProperty"

		''' <summary>
		''' Identifies that group membership has changed.
		''' </summary>
		Public Const MEMBER_OF_PROPERTY As String = "memberOfProperty"

		''' <summary>
		''' Identifies that the controller for the target object has changed
		''' </summary>
		Public Const CONTROLLER_FOR_PROPERTY As String = "controllerForProperty"

		''' <summary>
		''' Identifies that the target object that is doing the controlling has
		''' changed
		''' </summary>
		Public Const CONTROLLED_BY_PROPERTY As String = "controlledByProperty"

		''' <summary>
		''' Indicates the FLOWS_TO relation between two objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const FLOWS_TO_PROPERTY As String = "flowsToProperty"

		''' <summary>
		''' Indicates the FLOWS_FROM relation between two objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const FLOWS_FROM_PROPERTY As String = "flowsFromProperty"

		''' <summary>
		''' Indicates the SUBWINDOW_OF relation between two or more objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const SUBWINDOW_OF_PROPERTY As String = "subwindowOfProperty"

		''' <summary>
		''' Indicates the PARENT_WINDOW_OF relation between two or more objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const PARENT_WINDOW_OF_PROPERTY As String = "parentWindowOfProperty"

		''' <summary>
		''' Indicates the EMBEDS relation between two or more objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const EMBEDS_PROPERTY As String = "embedsProperty"

		''' <summary>
		''' Indicates the EMBEDDED_BY relation between two or more objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const EMBEDDED_BY_PROPERTY As String = "embeddedByProperty"

		''' <summary>
		''' Indicates the CHILD_NODE_OF relation between two or more objects
		''' has changed.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const CHILD_NODE_OF_PROPERTY As String = "childNodeOfProperty"

		''' <summary>
		''' Create a new AccessibleRelation using the given locale independent key.
		''' The key String should be a locale independent key for the relation.
		''' It is not intended to be used as the actual String to display
		''' to the user.  To get the localized string, use toDisplayString.
		''' </summary>
		''' <param name="key"> the locale independent name of the relation. </param>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Public Sub New(ByVal key As String)
			Me.key = key
			Me.target = Nothing
		End Sub

		''' <summary>
		''' Creates a new AccessibleRelation using the given locale independent key.
		''' The key String should be a locale independent key for the relation.
		''' It is not intended to be used as the actual String to display
		''' to the user.  To get the localized string, use toDisplayString.
		''' </summary>
		''' <param name="key"> the locale independent name of the relation. </param>
		''' <param name="target"> the target object for this relation </param>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Public Sub New(ByVal key As String, ByVal target As Object)
			Me.key = key
			Me.target = New Object(0){}
			Me.target(0) = target
		End Sub

		''' <summary>
		''' Creates a new AccessibleRelation using the given locale independent key.
		''' The key String should be a locale independent key for the relation.
		''' It is not intended to be used as the actual String to display
		''' to the user.  To get the localized string, use toDisplayString.
		''' </summary>
		''' <param name="key"> the locale independent name of the relation. </param>
		''' <param name="target"> the target object(s) for this relation </param>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Public Sub New(ByVal key As String, ByVal target As Object ())
			Me.key = key
			Me.target = target
		End Sub

		''' <summary>
		''' Returns the key for this relation
		''' </summary>
		''' <returns> the key for this relation
		''' </returns>
		''' <seealso cref= #CONTROLLER_FOR </seealso>
		''' <seealso cref= #CONTROLLED_BY </seealso>
		''' <seealso cref= #LABEL_FOR </seealso>
		''' <seealso cref= #LABELED_BY </seealso>
		''' <seealso cref= #MEMBER_OF </seealso>
		Public Overridable Property key As String
			Get
				Return Me.key
			End Get
		End Property

		''' <summary>
		''' Returns the target objects for this relation
		''' </summary>
		''' <returns> an array containing the target objects for this relation </returns>
		Public Overridable Property target As Object()
			Get
				If target Is Nothing Then target = New Object(){}
				Dim retval As Object() = New Object(target.Length - 1){}
				For i As Integer = 0 To target.Length - 1
					retval(i) = target(i)
				Next i
				Return retval
			End Get
			Set(ByVal target As Object)
				Me.target = New Object(0){}
				Me.target(0) = target
			End Set
		End Property


		''' <summary>
		''' Sets the target objects for this relation
		''' </summary>
		''' <param name="target"> an array containing the target objects for this relation </param>
		Public Overridable Property target As Object ()
			Set(ByVal target As Object ())
				Me.target = target
			End Set
		End Property
	End Class

End Namespace
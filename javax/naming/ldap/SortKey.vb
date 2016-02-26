'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.ldap

	''' <summary>
	''' A sort key and its associated sort parameters.
	''' This class implements a sort key which is used by the LDAPv3
	''' Control for server-side sorting of search results as defined in
	''' <a href="http://www.ietf.org/rfc/rfc2891.txt">RFC 2891</a>.
	''' 
	''' @since 1.5 </summary>
	''' <seealso cref= SortControl
	''' @author Vincent Ryan </seealso>
	Public Class SortKey

	'    
	'     * The ID of the attribute to sort by.
	'     
		Private attrID As String

	'    
	'     * The sort order. Ascending order, by default.
	'     
		Private reverseOrder As Boolean = False

	'    
	'     * The ID of the matching rule to use for ordering attribute values.
	'     
		Private matchingRuleID As String = Nothing

		''' <summary>
		''' Creates the default sort key for an attribute. Entries will be sorted
		''' according to the specified attribute in ascending order using the
		''' ordering matching rule defined for use with that attribute.
		''' </summary>
		''' <param name="attrID">  The non-null ID of the attribute to be used as a sort
		'''          key. </param>
		Public Sub New(ByVal attrID As String)
			Me.attrID = attrID
		End Sub

		''' <summary>
		''' Creates a sort key for an attribute. Entries will be sorted according to
		''' the specified attribute in the specified sort order and using the
		''' specified matching rule, if supplied.
		''' </summary>
		''' <param name="attrID">          The non-null ID of the attribute to be used as
		'''                          a sort key. </param>
		''' <param name="ascendingOrder">  If true then entries are arranged in ascending
		'''                          order. Otherwise there are arranged in
		'''                          descending order. </param>
		''' <param name="matchingRuleID">  The possibly null ID of the matching rule to
		'''                          use to order the attribute values. If not
		'''                          specified then the ordering matching rule
		'''                          defined for the sort key attribute is used. </param>
		Public Sub New(ByVal attrID As String, ByVal ascendingOrder As Boolean, ByVal matchingRuleID As String)

			Me.attrID = attrID
			reverseOrder = ((Not ascendingOrder))
			Me.matchingRuleID = matchingRuleID
		End Sub

		''' <summary>
		''' Retrieves the attribute ID of the sort key.
		''' </summary>
		''' <returns>    The non-null Attribute ID of the sort key. </returns>
		Public Overridable Property attributeID As String
			Get
				Return attrID
			End Get
		End Property

		''' <summary>
		''' Determines the sort order.
		''' </summary>
		''' <returns>    true if the sort order is ascending, false if descending. </returns>
		Public Overridable Property ascending As Boolean
			Get
				Return ((Not reverseOrder))
			End Get
		End Property

		''' <summary>
		''' Retrieves the matching rule ID used to order the attribute values.
		''' </summary>
		''' <returns>    The possibly null matching rule ID. If null then the
		'''            ordering matching rule defined for the sort key attribute
		'''            is used. </returns>
		Public Overridable Property matchingRuleID As String
			Get
				Return matchingRuleID
			End Get
		End Property
	End Class

End Namespace
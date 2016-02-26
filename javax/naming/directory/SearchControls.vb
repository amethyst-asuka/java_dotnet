Imports System

'
' * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.naming.directory

	''' <summary>
	''' This class encapsulates
	''' factors that determine scope of search and what gets returned
	''' as a result of the search.
	''' <p>
	''' A SearchControls instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single SearchControls instance should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	<Serializable> _
	Public Class SearchControls
		''' <summary>
		''' Search the named object.
		''' <p>
		''' The NamingEnumeration that results from search()
		''' using OBJECT_SCOPE will contain one or zero element.
		''' The enumeration contains one element if the named object satisfies
		''' the search filter specified in search().
		''' The element will have as its name the empty string because the names
		''' of elements in the NamingEnumeration are relative to the
		''' target context--in this case, the target context is the named object.
		''' It contains zero element if the named object does not satisfy
		''' the search filter specified in search().
		''' <p>
		''' The value of this constant is <tt>0</tt>.
		''' </summary>
		Public Const OBJECT_SCOPE As Integer = 0

		''' <summary>
		''' Search one level of the named context.
		''' <p>
		''' The NamingEnumeration that results from search()
		''' using ONELEVEL_SCOPE contains elements with
		''' objects in the named context that satisfy
		''' the search filter specified in search().
		''' The names of elements in the NamingEnumeration are atomic names
		''' relative to the named context.
		''' <p>
		''' The value of this constant is <tt>1</tt>.
		''' </summary>
		Public Const ONELEVEL_SCOPE As Integer = 1
		''' <summary>
		''' Search the entire subtree rooted at the named object.
		''' <p>
		''' If the named object is not a DirContext, search only the object.
		''' If the named object is a DirContext, search the subtree
		''' rooted at the named object, including the named object itself.
		''' <p>
		''' The search will not cross naming system boundaries.
		''' <p>
		''' The NamingEnumeration that results from search()
		''' using SUBTREE_SCOPE contains elements of objects
		''' from the subtree (including the named context)
		''' that satisfy the search filter specified in search().
		''' The names of elements in the NamingEnumeration are either
		''' relative to the named context or is a URL string.
		''' If the named context satisfies the search filter, it is
		''' included in the enumeration with the empty string as
		''' its name.
		''' <p>
		''' The value of this constant is <tt>2</tt>.
		''' </summary>
		Public Const SUBTREE_SCOPE As Integer = 2

		''' <summary>
		''' Contains the scope with which to apply the search. One of
		''' <tt>ONELEVEL_SCOPE</tt>, <tt>OBJECT_SCOPE</tt>, or
		''' <tt>SUBTREE_SCOPE</tt>.
		''' @serial
		''' </summary>
		Private searchScope As Integer

		''' <summary>
		''' Contains the milliseconds to wait before returning
		''' from search.
		''' @serial
		''' </summary>
		Private timeLimit As Integer

		''' <summary>
		''' Indicates whether JNDI links are dereferenced during
		''' search.
		''' @serial
		''' </summary>
		Private derefLink As Boolean

		''' <summary>
		'''  Indicates whether object is returned in <tt>SearchResult</tt>.
		''' @serial
		''' </summary>
		Private returnObj As Boolean

		''' <summary>
		''' Contains the maximum number of SearchResults to return.
		''' @serial
		''' </summary>
		Private countLimit As Long

		''' <summary>
		'''  Contains the list of attributes to be returned in
		''' <tt>SearchResult</tt> for each matching entry of search. <tt>null</tt>
		''' indicates that all attributes are to be returned.
		''' @serial
		''' </summary>
		Private attributesToReturn As String()

		''' <summary>
		''' Constructs a search constraints using defaults.
		''' <p>
		''' The defaults are:
		''' <ul>
		''' <li>search one level
		''' <li>no maximum return limit for search results
		''' <li>no time limit for search
		''' <li>return all attributes associated with objects that satisfy
		'''   the search filter.
		''' <li>do not return named object  (return only name and class)
		''' <li>do not dereference links during search
		''' </ul>
		''' </summary>
		Public Sub New()
			searchScope = ONELEVEL_SCOPE
			timeLimit = 0 ' no limit
			countLimit = 0 ' no limit
			derefLink = False
			returnObj = False
			attributesToReturn = Nothing ' return all
		End Sub

		''' <summary>
		''' Constructs a search constraints using arguments. </summary>
		''' <param name="scope">     The search scope.  One of:
		'''                  OBJECT_SCOPE, ONELEVEL_SCOPE, SUBTREE_SCOPE. </param>
		''' <param name="timelim">   The number of milliseconds to wait before returning.
		'''                  If 0, wait indefinitely. </param>
		''' <param name="deref">     If true, dereference links during search. </param>
		''' <param name="countlim">  The maximum number of entries to return.  If 0, return
		'''                  all entries that satisfy filter. </param>
		''' <param name="retobj">    If true, return the object bound to the name of the
		'''                  entry; if false, do not return object. </param>
		''' <param name="attrs">     The identifiers of the attributes to return along with
		'''                  the entry.  If null, return all attributes. If empty
		'''                  return no attributes. </param>
		Public Sub New(ByVal scope As Integer, ByVal countlim As Long, ByVal timelim As Integer, ByVal attrs As String(), ByVal retobj As Boolean, ByVal deref As Boolean)
			searchScope = scope
			timeLimit = timelim ' no limit
			derefLink = deref
			returnObj = retobj
			countLimit = countlim ' no limit
			attributesToReturn = attrs ' return all
		End Sub

		''' <summary>
		''' Retrieves the search scope of these SearchControls.
		''' <p>
		''' One of OBJECT_SCOPE, ONELEVEL_SCOPE, SUBTREE_SCOPE.
		''' </summary>
		''' <returns> The search scope of this SearchControls. </returns>
		''' <seealso cref= #setSearchScope </seealso>
		Public Overridable Property searchScope As Integer
			Get
				Return searchScope
			End Get
			Set(ByVal scope As Integer)
				searchScope = scope
			End Set
		End Property

		''' <summary>
		''' Retrieves the time limit of these SearchControls in milliseconds.
		''' <p>
		''' If the value is 0, this means to wait indefinitely. </summary>
		''' <returns> The time limit of these SearchControls in milliseconds. </returns>
		''' <seealso cref= #setTimeLimit </seealso>
		Public Overridable Property timeLimit As Integer
			Get
				Return timeLimit
			End Get
			Set(ByVal ms As Integer)
				timeLimit = ms
			End Set
		End Property

		''' <summary>
		''' Determines whether links will be dereferenced during the search.
		''' </summary>
		''' <returns> true if links will be dereferenced; false otherwise. </returns>
		''' <seealso cref= #setDerefLinkFlag </seealso>
		Public Overridable Property derefLinkFlag As Boolean
			Get
				Return derefLink
			End Get
			Set(ByVal [on] As Boolean)
				derefLink = [on]
			End Set
		End Property

		''' <summary>
		''' Determines whether objects will be returned as part of the result.
		''' </summary>
		''' <returns> true if objects will be returned; false otherwise. </returns>
		''' <seealso cref= #setReturningObjFlag </seealso>
		Public Overridable Property returningObjFlag As Boolean
			Get
				Return returnObj
			End Get
			Set(ByVal [on] As Boolean)
				returnObj = [on]
			End Set
		End Property

		''' <summary>
		''' Retrieves the maximum number of entries that will be returned
		''' as a result of the search.
		''' <p>
		''' 0 indicates that all entries will be returned. </summary>
		''' <returns> The maximum number of entries that will be returned. </returns>
		''' <seealso cref= #setCountLimit </seealso>
		Public Overridable Property countLimit As Long
			Get
				Return countLimit
			End Get
			Set(ByVal limit As Long)
				countLimit = limit
			End Set
		End Property

		''' <summary>
		''' Retrieves the attributes that will be returned as part of the search.
		''' <p>
		''' A value of null indicates that all attributes will be returned.
		''' An empty array indicates that no attributes are to be returned.
		''' </summary>
		''' <returns> An array of attribute ids identifying the attributes that
		''' will be returned. Can be null. </returns>
		''' <seealso cref= #setReturningAttributes </seealso>
		Public Overridable Property returningAttributes As String()
			Get
				Return attributesToReturn
			End Get
			Set(ByVal attrs As String())
				attributesToReturn = attrs
			End Set
		End Property







		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability.
		''' </summary>
		Private Const serialVersionUID As Long = -2480540967773454797L
	End Class

End Namespace
'
' * Copyright (c) 1999, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.event

	''' <summary>
	''' Contains methods for registering listeners to be notified
	''' of events fired when objects named in a directory context changes.
	''' <p>
	''' The methods in this interface support identification of objects by
	''' <A HREF="http://www.ietf.org/rfc/rfc2254.txt">RFC 2254</a>
	''' search filters.
	''' 
	''' <P>Using the search filter, it is possible to register interest in objects
	''' that do not exist at the time of registration but later come into existence and
	''' satisfy the filter.  However, there might be limitations in the extent
	''' to which this can be supported by the service provider and underlying
	''' protocol/service.  If the caller submits a filter that cannot be
	''' supported in this way, <tt>addNamingListener()</tt> throws an
	''' <tt>InvalidSearchFilterException</tt>.
	''' <p>
	''' See <tt>EventContext</tt> for a description of event source
	''' and target, and information about listener registration/deregistration
	''' that are also applicable to methods in this interface.
	''' See the
	''' <a href=package-summary.html#THREADING>package description</a>
	''' for information on threading issues.
	''' <p>
	''' A <tt>SearchControls</tt> or array object
	''' passed as a parameter to any method is owned by the caller.
	''' The service provider will not modify the object or keep a reference to it.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Interface EventDirContext
		Inherits EventContext, javax.naming.directory.DirContext

		''' <summary>
		''' Adds a listener for receiving naming events fired
		''' when objects identified by the search filter <tt>filter</tt> at
		''' the object named by target are modified.
		''' <p>
		''' The scope, returningObj flag, and returningAttributes flag from
		''' the search controls <tt>ctls</tt> are used to control the selection
		''' of objects that the listener is interested in,
		''' and determines what information is returned in the eventual
		''' <tt>NamingEvent</tt> object. Note that the requested
		''' information to be returned might not be present in the <tt>NamingEvent</tt>
		''' object if they are unavailable or could not be obtained by the
		''' service provider or service.
		''' </summary>
		''' <param name="target"> The nonnull name of the object resolved relative to this context. </param>
		''' <param name="filter"> The nonnull string filter (see RFC2254). </param>
		''' <param name="ctls">   The possibly null search controls. If null, the default
		'''        search controls are used. </param>
		''' <param name="l">  The nonnull listener. </param>
		''' <exception cref="NamingException"> If a problem was encountered while
		''' adding the listener. </exception>
		''' <seealso cref= EventContext#removeNamingListener </seealso>
		''' <seealso cref= javax.naming.directory.DirContext#search(javax.naming.Name, java.lang.String, javax.naming.directory.SearchControls) </seealso>
		Sub addNamingListener(ByVal target As javax.naming.Name, ByVal filter As String, ByVal ctls As javax.naming.directory.SearchControls, ByVal l As NamingListener)

		''' <summary>
		''' Adds a listener for receiving naming events fired when
		''' objects identified by the search filter <tt>filter</tt> at the
		''' object named by the string target name are modified.
		''' See the overload that accepts a <tt>Name</tt> for details of
		''' how this method behaves.
		''' </summary>
		''' <param name="target"> The nonnull string name of the object resolved relative to this context. </param>
		''' <param name="filter"> The nonnull string filter (see RFC2254). </param>
		''' <param name="ctls">   The possibly null search controls. If null, the default
		'''        search controls is used. </param>
		''' <param name="l">  The nonnull listener. </param>
		''' <exception cref="NamingException"> If a problem was encountered while
		''' adding the listener. </exception>
		''' <seealso cref= EventContext#removeNamingListener </seealso>
		''' <seealso cref= javax.naming.directory.DirContext#search(java.lang.String, java.lang.String, javax.naming.directory.SearchControls) </seealso>
		Sub addNamingListener(ByVal target As String, ByVal filter As String, ByVal ctls As javax.naming.directory.SearchControls, ByVal l As NamingListener)

		''' <summary>
		''' Adds a listener for receiving naming events fired
		''' when objects identified by the search filter <tt>filter</tt> and
		''' filter arguments at the object named by the target are modified.
		''' The scope, returningObj flag, and returningAttributes flag from
		''' the search controls <tt>ctls</tt> are used to control the selection
		''' of objects that the listener is interested in,
		''' and determines what information is returned in the eventual
		''' <tt>NamingEvent</tt> object.  Note that the requested
		''' information to be returned might not be present in the <tt>NamingEvent</tt>
		''' object if they are unavailable or could not be obtained by the
		''' service provider or service.
		''' </summary>
		''' <param name="target"> The nonnull name of the object resolved relative to this context. </param>
		''' <param name="filter"> The nonnull string filter (see RFC2254). </param>
		''' <param name="filterArgs"> The possibly null array of arguments for the filter. </param>
		''' <param name="ctls">   The possibly null search controls. If null, the default
		'''        search controls are used. </param>
		''' <param name="l">  The nonnull listener. </param>
		''' <exception cref="NamingException"> If a problem was encountered while
		''' adding the listener. </exception>
		''' <seealso cref= EventContext#removeNamingListener </seealso>
		''' <seealso cref= javax.naming.directory.DirContext#search(javax.naming.Name, java.lang.String, java.lang.Object[], javax.naming.directory.SearchControls) </seealso>
		Sub addNamingListener(ByVal target As javax.naming.Name, ByVal filter As String, ByVal filterArgs As Object(), ByVal ctls As javax.naming.directory.SearchControls, ByVal l As NamingListener)

		''' <summary>
		''' Adds a listener for receiving naming events fired when
		''' objects identified by the search filter <tt>filter</tt>
		''' and filter arguments at the
		''' object named by the string target name are modified.
		''' See the overload that accepts a <tt>Name</tt> for details of
		''' how this method behaves.
		''' </summary>
		''' <param name="target"> The nonnull string name of the object resolved relative to this context. </param>
		''' <param name="filter"> The nonnull string filter (see RFC2254). </param>
		''' <param name="filterArgs"> The possibly null array of arguments for the filter. </param>
		''' <param name="ctls">   The possibly null search controls. If null, the default
		'''        search controls is used. </param>
		''' <param name="l">  The nonnull listener. </param>
		''' <exception cref="NamingException"> If a problem was encountered while
		''' adding the listener. </exception>
		''' <seealso cref= EventContext#removeNamingListener </seealso>
		''' <seealso cref= javax.naming.directory.DirContext#search(java.lang.String, java.lang.String, java.lang.Object[], javax.naming.directory.SearchControls)       </seealso>
		Sub addNamingListener(ByVal target As String, ByVal filter As String, ByVal filterArgs As Object(), ByVal ctls As javax.naming.directory.SearchControls, ByVal l As NamingListener)
	End Interface

End Namespace
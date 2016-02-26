Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements the LDAPv3 Extended Request for StartTLS as
	''' defined in
	''' <a href="http://www.ietf.org/rfc/rfc2830.txt">Lightweight Directory
	''' Access Protocol (v3): Extension for Transport Layer Security</a>
	''' 
	''' The object identifier for StartTLS is 1.3.6.1.4.1.1466.20037
	''' and no extended request value is defined.
	''' <p>
	''' <tt>StartTlsRequest</tt>/<tt>StartTlsResponse</tt> are used to establish
	''' a TLS connection over the existing LDAP connection associated with
	''' the JNDI context on which <tt>extendedOperation()</tt> is invoked.
	''' Typically, a JNDI program uses these classes as follows.
	''' <blockquote><pre>
	''' import javax.naming.ldap.*;
	''' 
	''' // Open an LDAP association
	''' LdapContext ctx = new InitialLdapContext();
	''' 
	''' // Perform a StartTLS extended operation
	''' StartTlsResponse tls =
	'''     (StartTlsResponse) ctx.extendedOperation(new StartTlsRequest());
	''' 
	''' // Open a TLS connection (over the existing LDAP association) and get details
	''' // of the negotiated TLS session: cipher suite, peer certificate, etc.
	''' SSLSession session = tls.negotiate();
	''' 
	''' // ... use ctx to perform protected LDAP operations
	''' 
	''' // Close the TLS connection (revert back to the underlying LDAP association)
	''' tls.close();
	''' 
	''' // ... use ctx to perform unprotected LDAP operations
	''' 
	''' // Close the LDAP association
	''' ctx.close;
	''' </pre></blockquote>
	''' 
	''' @since 1.4 </summary>
	''' <seealso cref= StartTlsResponse
	''' @author Vincent Ryan </seealso>
	Public Class StartTlsRequest
		Implements ExtendedRequest

		' Constant

		''' <summary>
		''' The StartTLS extended request's assigned object identifier
		''' is 1.3.6.1.4.1.1466.20037.
		''' </summary>
		Public Const OID As String = "1.3.6.1.4.1.1466.20037"


		' Constructors

		''' <summary>
		''' Constructs a StartTLS extended request.
		''' </summary>
		Public Sub New()
		End Sub


		' ExtendedRequest methods

		''' <summary>
		''' Retrieves the StartTLS request's object identifier string.
		''' </summary>
		''' <returns> The object identifier string, "1.3.6.1.4.1.1466.20037". </returns>
	ReadOnly	Public Overridable Property iD As String Implements ExtendedRequest.getID
			Get
				Return OID
			End Get
		End Property

		''' <summary>
		''' Retrieves the StartTLS request's ASN.1 BER encoded value.
		''' Since the request has no defined value, null is always
		''' returned.
		''' </summary>
		''' <returns> The null value. </returns>
	ReadOnly	Public Overridable Property encodedValue As SByte() Implements ExtendedRequest.getEncodedValue
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Creates an extended response object that corresponds to the
		''' LDAP StartTLS extended request.
		''' <p>
		''' The result must be a concrete subclass of StartTlsResponse
		''' and must have a public zero-argument constructor.
		''' <p>
		''' This method locates the implementation class by locating
		''' configuration files that have the name:
		''' <blockquote><tt>
		'''     META-INF/services/javax.naming.ldap.StartTlsResponse
		''' </tt></blockquote>
		''' The configuration files and their corresponding implementation classes must
		''' be accessible to the calling thread's context class loader.
		''' <p>
		''' Each configuration file should contain a list of fully-qualified class
		''' names, one per line.  Space and tab characters surrounding each name, as
		''' well as blank lines, are ignored.  The comment character is <tt>'#'</tt>
		''' (<tt>0x23</tt>); on each line all characters following the first comment
		''' character are ignored.  The file must be encoded in UTF-8.
		''' <p>
		''' This method will return an instance of the first implementation
		''' class that it is able to load and instantiate successfully from
		''' the list of class names collected from the configuration files.
		''' This method uses the calling thread's context classloader to find the
		''' configuration files and to load the implementation class.
		''' <p>
		''' If no class can be found in this way, this method will use
		''' an implementation-specific way to locate an implementation.
		''' If none is found, a NamingException is thrown.
		''' </summary>
		''' <param name="id">         The object identifier of the extended response.
		'''                   Its value must be "1.3.6.1.4.1.1466.20037" or null.
		'''                   Both values are equivalent. </param>
		''' <param name="berValue">   The possibly null ASN.1 BER encoded value of the
		'''                   extended response. This is the raw BER bytes
		'''                   including the tag and length of the response value.
		'''                   It does not include the response OID.
		'''                   Its value is ignored because a Start TLS response
		'''                   is not expected to contain any response value. </param>
		''' <param name="offset">     The starting position in berValue of the bytes to use.
		'''                   Its value is ignored because a Start TLS response
		'''                   is not expected to contain any response value. </param>
		''' <param name="length">     The number of bytes in berValue to use.
		'''                   Its value is ignored because a Start TLS response
		'''                   is not expected to contain any response value. </param>
		''' <returns>           The StartTLS extended response object. </returns>
		''' <exception cref="NamingException"> If a naming exception was encountered
		'''                   while creating the StartTLS extended response object. </exception>
		Public Overridable Function createExtendedResponse(ByVal id As String, ByVal berValue As SByte(), ByVal offset As Integer, ByVal length As Integer) As ExtendedResponse Implements ExtendedRequest.createExtendedResponse

			' Confirm that the object identifier is correct
			If (id IsNot Nothing) AndAlso ((Not id.Equals(OID))) Then Throw New javax.naming.ConfigurationException("Start TLS received the following response instead of " & OID & ": " & id)

			Dim resp As StartTlsResponse = Nothing

			Dim sl As java.util.ServiceLoader(Of StartTlsResponse) = java.util.ServiceLoader.load(GetType(StartTlsResponse), contextClassLoader)
			Dim iter As IEnumerator(Of StartTlsResponse) = sl.GetEnumerator()

			Do While resp Is Nothing AndAlso privilegedHasNext(iter)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				resp = iter.next()
			Loop
			If resp IsNot Nothing Then Return resp
			Try
				Dim helper As com.sun.naming.internal.VersionHelper = com.sun.naming.internal.VersionHelper.versionHelper
				Dim clas As Type = helper.loadClass("com.sun.jndi.ldap.ext.StartTlsResponseImpl")

				resp = CType(clas.newInstance(), StartTlsResponse)

			Catch e As IllegalAccessException
				Throw wrapException(e)

			Catch e As InstantiationException
				Throw wrapException(e)

			Catch e As ClassNotFoundException
				Throw wrapException(e)
			End Try

			Return resp
		End Function

	'    
	'     * Wrap an exception, thrown while attempting to load the StartTlsResponse
	'     * class, in a configuration exception.
	'     
		Private Function wrapException(ByVal e As Exception) As javax.naming.ConfigurationException
			Dim ce As New javax.naming.ConfigurationException("Cannot load implementation of javax.naming.ldap.StartTlsResponse")

			ce.rootCause = e
			Return ce
		End Function

	'    
	'     * Acquire the class loader associated with this thread.
	'     
		Private Property contextClassLoader As ClassLoader
			Get
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<ClassLoader>()
		'		{
		'				public ClassLoader run()
		'				{
		'					Return Thread.currentThread().getContextClassLoader();
		'				}
		'			}
			   )
			End Get
		End Property

		Private Shared Function privilegedHasNext(ByVal iter As IEnumerator(Of StartTlsResponse)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.lang.Boolean answer = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.lang.Boolean>()
	'		{
	'			public java.lang.Boolean run()
	'			{
	'				Return Boolean.valueOf(iter.hasNext());
	'			}
	'		});
			Return answer
		End Function

		Private Const serialVersionUID As Long = 4441679576360753397L
	End Class

End Namespace
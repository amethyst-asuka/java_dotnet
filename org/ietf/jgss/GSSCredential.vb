Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.ietf.jgss

	''' <summary>
	''' This interface encapsulates the GSS-API credentials for an entity.  A
	''' credential contains all the necessary cryptographic information to
	''' enable the creation of a context on behalf of the entity that it
	''' represents.  It may contain multiple, distinct, mechanism specific
	''' credential elements, each containing information for a specific
	''' security mechanism, but all referring to the same entity. A credential
	''' may be used to perform context initiation, acceptance, or both.<p>
	''' 
	''' Credentials are instantiated using one of the
	''' <code>createCredential</code> methods in the {@link GSSManager
	''' GSSManager} class. GSS-API credential creation is not
	''' intended to provide a "login to the network" function, as such a
	''' function would involve the creation of new credentials rather than
	''' merely acquiring a handle to existing credentials. The
	''' <a href=package-summary.html#useSubjectCredsOnly>section on credential
	''' acquisition</a> in the package level description describes
	''' how existing credentials are acquired in the Java platform. GSS-API
	''' implementations must impose a local access-control policy on callers to
	''' prevent unauthorized callers from acquiring credentials to which they
	''' are not entitled. <p>
	''' 
	''' Applications will create a credential object passing the desired
	''' parameters.  The application can then use the query methods to obtain
	''' specific information about the instantiated credential object.
	''' When the credential is no longer needed, the application should call
	''' the <seealso cref="#dispose() dispose"/> method to release any resources held by
	''' the credential object and to destroy any cryptographically sensitive
	''' information.<p>
	''' 
	''' This example code demonstrates the creation of a GSSCredential
	''' implementation for a specific entity, querying of its fields, and its
	''' release when it is no longer needed:<p>
	''' <pre>
	'''    GSSManager manager = GSSManager.getInstance();
	''' 
	'''    // start by creating a name object for the entity
	'''    GSSName name = manager.createName("myusername", GSSName.NT_USER_NAME);
	''' 
	'''    // now acquire credentials for the entity
	'''    GSSCredential cred = manager.createCredential(name,
	'''                    GSSCredential.ACCEPT_ONLY);
	''' 
	'''    // display credential information - name, remaining lifetime,
	'''    // and the mechanisms it has been acquired over
	'''    System.out.println(cred.getName().toString());
	'''    System.out.println(cred.getRemainingLifetime());
	''' 
	'''    Oid [] mechs = cred.getMechs();
	'''    if (mechs != null) {
	'''            for (int i = 0; i < mechs.length; i++)
	'''                    System.out.println(mechs[i].toString());
	'''    }
	''' 
	'''    // release system resources held by the credential
	'''    cred.dispose();
	''' </pre>
	''' </summary>
	''' <seealso cref= GSSManager#createCredential(int) </seealso>
	''' <seealso cref= GSSManager#createCredential(GSSName, int, Oid, int) </seealso>
	''' <seealso cref= GSSManager#createCredential(GSSName, int, Oid[], int) </seealso>
	''' <seealso cref= #dispose()
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4 </seealso>
	Public Interface GSSCredential
		Inherits ICloneable

		''' <summary>
		''' Credential usage flag requesting that it be usable
		''' for both context initiation and acceptance.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int INITIATE_AND_ACCEPT = 0;


		''' <summary>
		''' Credential usage flag requesting that it be usable
		''' for context initiation only.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int INITIATE_ONLY = 1;


		''' <summary>
		''' Credential usage flag requesting that it be usable
		''' for context acceptance only.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int ACCEPT_ONLY = 2;


		''' <summary>
		''' A lifetime constant representing the default credential lifetime. This
		''' value it set to 0.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DEFAULT_LIFETIME = 0;

		''' <summary>
		''' A lifetime constant representing indefinite credential lifetime.
		''' This value must is set to the maximum integer value in Java -
		''' <seealso cref="java.lang.Integer#MAX_VALUE Integer.MAX_VALUE"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int INDEFINITE_LIFETIME = Integer.MAX_VALUE;

		''' <summary>
		''' Releases any sensitive information that the GSSCredential object may
		''' be containing.  Applications should call this method as soon as the
		''' credential is no longer needed to minimize the time any sensitive
		''' information is maintained.
		''' </summary>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Sub dispose()

		''' <summary>
		'''  Retrieves the name of the entity that the credential asserts.
		''' </summary>
		''' <returns> a GSSName representing the entity
		''' </returns>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		ReadOnly Property name As GSSName

		''' <summary>
		''' Retrieves a Mechanism Name of the entity that the credential
		''' asserts. This is equivalent to calling {@link
		''' GSSName#canonicalize(Oid) canonicalize} on the value returned by
		''' the other form of <seealso cref="#getName() getName"/>.
		''' </summary>
		''' <param name="mech"> the Oid of the mechanism for which the Mechanism Name
		''' should be returned. </param>
		''' <returns> a GSSName representing the entity canonicalized for the
		''' desired mechanism
		''' </returns>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_MECH GSSException.BAD_MECH"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function getName(ByVal mech As Oid) As GSSName

		''' <summary>
		''' Returns the remaining lifetime in seconds for a credential.  The
		''' remaining lifetime is the minimum lifetime amongst all of the underlying
		''' mechanism specific credential elements.
		''' </summary>
		''' <returns> the minimum remaining lifetime in seconds for this
		''' credential. A return value of {@link #INDEFINITE_LIFETIME
		''' INDEFINITE_LIFETIME} indicates that the credential does
		''' not expire. A return value of 0 indicates that the credential is
		''' already expired.
		''' </returns>
		''' <seealso cref= #getRemainingInitLifetime(Oid) </seealso>
		''' <seealso cref= #getRemainingAcceptLifetime(Oid)
		''' </seealso>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		ReadOnly Property remainingLifetime As Integer

		''' <summary>
		''' Returns the lifetime in seconds for the credential to remain capable
		''' of initiating security contexts using the specified mechanism. This
		''' method queries the initiator credential element that belongs to the
		''' specified mechanism.
		''' </summary>
		''' <returns> the number of seconds remaining in the life of this credential
		''' element. A return value of {@link #INDEFINITE_LIFETIME
		''' INDEFINITE_LIFETIME} indicates that the credential element does not
		''' expire.  A return value of 0 indicates that the credential element is
		''' already expired.
		''' </returns>
		''' <param name="mech"> the Oid of the mechanism whose initiator credential element
		''' should be queried.
		''' </param>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_MECH GSSException.BAD_MECH"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function getRemainingInitLifetime(ByVal mech As Oid) As Integer

		''' <summary>
		''' Returns the lifetime in seconds for the credential to remain capable
		''' of accepting security contexts using the specified mechanism. This
		''' method queries the acceptor credential element that belongs to the
		''' specified mechanism.
		''' </summary>
		''' <returns> the number of seconds remaining in the life of this credential
		''' element. A return value of {@link #INDEFINITE_LIFETIME
		''' INDEFINITE_LIFETIME} indicates that the credential element does not
		''' expire.  A return value of 0 indicates that the credential element is
		''' already expired.
		''' </returns>
		''' <param name="mech"> the Oid of the mechanism whose acceptor credential element
		''' should be queried.
		''' </param>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_MECH GSSException.BAD_MECH"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function getRemainingAcceptLifetime(ByVal mech As Oid) As Integer

		''' <summary>
		''' Returns the credential usage mode. In other words, it
		''' tells us if this credential can be used for initiating or accepting
		''' security contexts. It does not tell us which mechanism(s) has to be
		''' used in order to do so. It is expected that an application will allow
		''' the GSS-API to pick a default mechanism after calling this method.
		''' </summary>
		''' <returns> The return value will be one of {@link #INITIATE_ONLY
		''' INITIATE_ONLY}, <seealso cref="#ACCEPT_ONLY ACCEPT_ONLY"/>, and {@link
		''' #INITIATE_AND_ACCEPT INITIATE_AND_ACCEPT}.
		''' </returns>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		ReadOnly Property usage As Integer

		''' <summary>
		''' Returns the credential usage mode for a specific mechanism. In other
		''' words, it tells us if this credential can be used
		''' for initiating or accepting security contexts with a given underlying
		''' mechanism.
		''' </summary>
		''' <returns> The return value will be one of {@link #INITIATE_ONLY
		''' INITIATE_ONLY}, <seealso cref="#ACCEPT_ONLY ACCEPT_ONLY"/>, and {@link
		''' #INITIATE_AND_ACCEPT INITIATE_AND_ACCEPT}. </returns>
		''' <param name="mech"> the Oid of the mechanism whose credentials usage mode is
		''' to be determined.
		''' </param>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_MECH GSSException.BAD_MECH"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function getUsage(ByVal mech As Oid) As Integer

		''' <summary>
		''' Returns a list of mechanisms supported by this credential. It does
		''' not tell us which ones can be used to initiate
		''' contexts and which ones can be used to accept contexts. The
		''' application must call the <seealso cref="#getUsage(Oid) getUsage"/> method with
		''' each of the returned Oid's to determine the possible modes of
		''' usage.
		''' </summary>
		''' <returns> an array of Oid's corresponding to the supported mechanisms.
		''' </returns>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		ReadOnly Property mechs As Oid()

		''' <summary>
		''' Adds a mechanism specific credential-element to an existing
		''' credential.  This method allows the construction of credentials, one
		''' mechanism at a time.<p>
		''' 
		''' This routine is envisioned to be used mainly by context acceptors
		''' during the creation of acceptor credentials which are to be used
		''' with a variety of clients using different security mechanisms.<p>
		''' 
		''' This routine adds the new credential element "in-place".  To add the
		''' element in a new credential, first call <code>clone</code> to obtain a
		''' copy of this credential, then call its <code>add</code> method.<p>
		''' 
		''' As always, GSS-API implementations must impose a local access-control
		''' policy on callers to prevent unauthorized callers from acquiring
		''' credentials to which they are not entitled.
		''' 
		''' Non-default values for initLifetime and acceptLifetime cannot always
		''' be honored by the underlying mechanisms, thus callers should be
		''' prepared to call {@link #getRemainingInitLifetime(Oid)
		''' getRemainingInitLifetime} and {@link #getRemainingAcceptLifetime(Oid)
		''' getRemainingAcceptLifetime} on the credential.
		''' </summary>
		''' <param name="name"> the name of the principal for whom this credential is to
		''' be acquired.  Use <code>null</code> to specify the default
		''' principal. </param>
		''' <param name="initLifetime"> the number of seconds that the credential element
		''' should remain valid for initiating of security contexts. Use {@link
		''' GSSCredential#INDEFINITE_LIFETIME GSSCredential.INDEFINITE_LIFETIME}
		''' to request that the credentials have the maximum permitted lifetime
		''' for this.  Use {@link GSSCredential#DEFAULT_LIFETIME
		''' GSSCredential.DEFAULT_LIFETIME} to request default credential lifetime
		''' for this. </param>
		''' <param name="acceptLifetime"> the number of seconds that the credential
		''' element should remain valid for accepting security contexts. Use {@link
		''' GSSCredential#INDEFINITE_LIFETIME GSSCredential.INDEFINITE_LIFETIME}
		''' to request that the credentials have the maximum permitted lifetime
		''' for this.  Use {@link GSSCredential#DEFAULT_LIFETIME
		''' GSSCredential.DEFAULT_LIFETIME} to request default credential lifetime
		''' for this. </param>
		''' <param name="mech"> the mechanism over which the credential is to be acquired. </param>
		''' <param name="usage"> the usage mode that this credential
		''' element should add to the credential. The value
		''' of this parameter must be one of:
		''' <seealso cref="#INITIATE_AND_ACCEPT INITIATE_AND_ACCEPT"/>,
		''' <seealso cref="#ACCEPT_ONLY ACCEPT_ONLY"/>, and
		''' <seealso cref="#INITIATE_ONLY INITIATE_ONLY"/>.
		''' </param>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         {@link GSSException#DUPLICATE_ELEMENT
		'''                          GSSException.DUPLICATE_ELEMENT},
		'''         <seealso cref="GSSException#BAD_MECH GSSException.BAD_MECH"/>,
		'''         <seealso cref="GSSException#BAD_NAMETYPE GSSException.BAD_NAMETYPE"/>,
		'''         <seealso cref="GSSException#NO_CRED GSSException.NO_CRED"/>,
		'''         {@link GSSException#CREDENTIALS_EXPIRED
		'''                                  GSSException.CREDENTIALS_EXPIRED},
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Sub add(ByVal name As GSSName, ByVal initLifetime As Integer, ByVal acceptLifetime As Integer, ByVal mech As Oid, ByVal usage As Integer)

		''' <summary>
		''' Tests if this GSSCredential asserts the same entity as the supplied
		''' object.  The two credentials must be acquired over the same
		''' mechanisms and must refer to the same principal.
		''' </summary>
		''' <returns> <code>true</code> if the two GSSCredentials assert the same
		''' entity; <code>false</code> otherwise. </returns>
		''' <param name="another"> another GSSCredential for comparison to this one </param>
		Function Equals(ByVal another As Object) As Boolean

		''' <summary>
		''' Returns a hashcode value for this GSSCredential.
		''' </summary>
		''' <returns> a hashCode value </returns>
		Function GetHashCode() As Integer

	End Interface

End Namespace
Imports sun.security.jgss.spi

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
	''' This interface encapsulates a single GSS-API principal entity. The
	''' application obtains an implementation of this interface
	''' through one of the <code>createName</code> methods that exist in the {@link
	''' GSSManager GSSManager} class. Conceptually a GSSName contains many
	''' representations of the entity or many primitive name elements, one for
	''' each supported underlying mechanism. In GSS terminology, a GSSName that
	''' contains an element from just one mechanism is called a Mechanism Name
	''' (MN)<p>
	''' 
	''' Since different authentication mechanisms may employ different
	''' namespaces for identifying their principals, GSS-API's naming support is
	''' necessarily complex in multi-mechanism environments (or even in some
	''' single-mechanism environments where the underlying mechanism supports
	''' multiple namespaces). Different name formats and their definitions are
	''' identified with <seealso cref="Oid Oid's"/> and some standard types
	''' are defined in this interface. The format of the names can be derived
	''' based on the unique <code>Oid</code> of its name type.<p>
	''' 
	''' Included below are code examples utilizing the <code>GSSName</code> interface.
	''' The code below creates a <code>GSSName</code>, converts it to an MN, performs a
	''' comparison, obtains a printable representation of the name, exports it
	''' to a byte array and then re-imports to obtain a
	''' new <code>GSSName</code>.<p>
	''' <pre>
	'''      GSSManager manager = GSSManager.getInstance();
	''' 
	'''      // create a host based service name
	'''      GSSName name = manager.createName("service@host",
	'''                   GSSName.NT_HOSTBASED_SERVICE);
	''' 
	'''      Oid krb5 = new Oid("1.2.840.113554.1.2.2");
	''' 
	'''      GSSName mechName = name.canonicalize(krb5);
	''' 
	'''      // the above two steps are equivalent to the following
	'''      GSSName mechName = manager.createName("service@host",
	'''                      GSSName.NT_HOSTBASED_SERVICE, krb5);
	''' 
	'''      // perform name comparison
	'''      if (name.equals(mechName))
	'''              print("Names are equals.");
	''' 
	'''      // obtain textual representation of name and its printable
	'''      // name type
	'''      print(mechName.toString() +
	'''                      mechName.getStringNameType().toString());
	''' 
	'''      // export and re-import the name
	'''      byte [] exportName = mechName.export();
	''' 
	'''      // create a new name object from the exported buffer
	'''      GSSName newName = manager.createName(exportName,
	'''                      GSSName.NT_EXPORT_NAME);
	''' 
	''' </pre> </summary>
	''' <seealso cref= #export() </seealso>
	''' <seealso cref= #equals(GSSName) </seealso>
	''' <seealso cref= GSSManager#createName(String, Oid) </seealso>
	''' <seealso cref= GSSManager#createName(String, Oid, Oid) </seealso>
	''' <seealso cref= GSSManager#createName(byte[], Oid)
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4 </seealso>
	Public Interface GSSName

		''' <summary>
		''' Oid indicating a host-based service name form.  It is used to
		''' represent services associated with host computers.  This name form
		''' is constructed using two elements, "service" and "hostname", as
		''' follows: service@hostname.<p>
		''' 
		''' It represents the following Oid value:<br>
		'''  <code>{ iso(1) member-body(2) United
		''' States(840) mit(113554) infosys(1) gssapi(2) generic(1) service_name(4)
		''' }</code>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Oid NT_HOSTBASED_SERVICE = Oid.getInstance("1.2.840.113554.1.2.1.4");

		''' <summary>
		''' Name type to indicate a named user on a local system.<p>
		''' It represents the following Oid value:<br>
		'''  <code>{ iso(1) member-body(2) United
		''' States(840) mit(113554) infosys(1) gssapi(2) generic(1) user_name(1)
		''' }</code>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Oid NT_USER_NAME = Oid.getInstance("1.2.840.113554.1.2.1.1");

		''' <summary>
		''' Name type to indicate a numeric user identifier corresponding to a
		''' user on a local system. (e.g. Uid).<p>
		''' 
		'''  It represents the following Oid value:<br>
		''' <code>{ iso(1) member-body(2) United States(840) mit(113554)
		''' infosys(1) gssapi(2) generic(1) machine_uid_name(2) }</code>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Oid NT_MACHINE_UID_NAME = Oid.getInstance("1.2.840.113554.1.2.1.2");

		''' <summary>
		''' Name type to indicate a string of digits representing the numeric
		''' user identifier of a user on a local system.<p>
		''' 
		''' It represents the following Oid value:<br>
		''' <code>{ iso(1) member-body(2) United
		''' States(840) mit(113554) infosys(1) gssapi(2) generic(1)
		''' string_uid_name(3) }</code>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Oid NT_STRING_UID_NAME = Oid.getInstance("1.2.840.113554.1.2.1.3");

		''' <summary>
		''' Name type for representing an anonymous entity.<p>
		''' It represents the following Oid value:<br>
		''' <code>{ 1(iso), 3(org), 6(dod), 1(internet),
		''' 5(security), 6(nametypes), 3(gss-anonymous-name) }</code>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Oid NT_ANONYMOUS = Oid.getInstance("1.3.6.1.5.6.3");

		''' <summary>
		''' Name type used to indicate an exported name produced by the export
		''' method.<p>
		''' 
		''' It represents the following Oid value:<br> <code>{ 1(iso),
		''' 3(org), 6(dod), 1(internet), 5(security), 6(nametypes),
		''' 4(gss-api-exported-name) }</code>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Oid NT_EXPORT_NAME = Oid.getInstance("1.3.6.1.5.6.4");

		''' <summary>
		''' Compares two <code>GSSName</code> objects to determine if they refer to the
		''' same entity.
		''' </summary>
		''' <param name="another"> the <code>GSSName</code> to compare this name with </param>
		''' <returns> true if the two names contain at least one primitive element
		''' in common. If either of the names represents an anonymous entity, the
		''' method will return false.
		''' </returns>
		''' <exception cref="GSSException"> when the names cannot be compared, containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_NAMETYPE GSSException.BAD_NAMETYPE"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function Equals(ByVal another As GSSName) As Boolean

		''' <summary>
		''' Compares this <code>GSSName</code> object to another Object that might be a
		''' <code>GSSName</code>. The behaviour is exactly the same as in {@link
		''' #equals(GSSName) equals} except that no GSSException is thrown;
		''' instead, false will be returned in the situation where an error
		''' occurs. </summary>
		''' <returns> true if the object to compare to is also a <code>GSSName</code> and the two
		''' names refer to the same entity. </returns>
		''' <param name="another"> the object to compare this name to </param>
		''' <seealso cref= #equals(GSSName) </seealso>
		Function Equals(ByVal another As Object) As Boolean

		''' <summary>
		''' Returns a hashcode value for this GSSName.
		''' </summary>
		''' <returns> a hashCode value </returns>
		Function GetHashCode() As Integer

		''' <summary>
		''' Creates a name that is canonicalized for some
		''' mechanism.
		''' </summary>
		''' <returns> a <code>GSSName</code> that contains just one primitive
		''' element representing this name in a canonicalized form for the desired
		''' mechanism. </returns>
		''' <param name="mech"> the oid for the mechanism for which the canonical form of
		''' the name is requested.
		''' </param>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_MECH GSSException.BAD_MECH"/>,
		'''         <seealso cref="GSSException#BAD_NAMETYPE GSSException.BAD_NAMETYPE"/>,
		'''         <seealso cref="GSSException#BAD_NAME GSSException.BAD_NAME"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function canonicalize(ByVal mech As Oid) As GSSName

		''' <summary>
		''' Returns a canonical contiguous byte representation of a mechanism name
		''' (MN), suitable for direct, byte by byte comparison by authorization
		''' functions.  If the name is not an MN, implementations may throw a
		''' GSSException with the NAME_NOT_MN status code.  If an implementation
		''' chooses not to throw an exception, it should use some system specific
		''' default mechanism to canonicalize the name and then export
		''' it. Structurally, an exported name object consists of a header
		''' containing an OID identifying the mechanism that authenticated the
		''' name, and a trailer containing the name itself, where the syntax of
		''' the trailer is defined by the individual mechanism specification. The
		''' format of the header of the output buffer is specified in RFC 2743.<p>
		''' 
		''' The exported name is useful when used in large access control lists
		''' where the overhead of creating a <code>GSSName</code> object on each
		''' name and invoking the equals method on each name from the ACL may be
		''' prohibitive.<p>
		''' 
		''' Exported names may be re-imported by using the byte array factory
		''' method {@link GSSManager#createName(byte[], Oid)
		''' GSSManager.createName} and specifying the NT_EXPORT_NAME as the name
		''' type object identifier. The resulting <code>GSSName</code> name will
		''' also be a MN.<p> </summary>
		''' <returns> a byte[] containing the exported name. RFC 2743 defines the
		''' "Mechanism-Independent Exported Name Object Format" for these bytes.
		''' </returns>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#BAD_NAME GSSException.BAD_NAME"/>,
		'''         <seealso cref="GSSException#BAD_NAMETYPE GSSException.BAD_NAMETYPE"/>,
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		Function export() As SByte()

		''' <summary>
		''' Returns a textual representation of the <code>GSSName</code> object.  To retrieve
		''' the printed name format, which determines the syntax of the returned
		''' string, use the <seealso cref="#getStringNameType() getStringNameType"/>
		''' method.
		''' </summary>
		''' <returns> a String representing this name in printable form. </returns>
		Function ToString() As String

		''' <summary>
		''' Returns the name type of the printable
		''' representation of this name that can be obtained from the <code>
		''' toString</code> method.
		''' </summary>
		''' <returns> an Oid representing the namespace of the name returned
		''' from the toString method.
		''' </returns>
		''' <exception cref="GSSException"> containing the following
		''' major error codes:
		'''         <seealso cref="GSSException#FAILURE GSSException.FAILURE"/> </exception>
		ReadOnly Property stringNameType As Oid

		''' <summary>
		''' Tests if this name object represents an anonymous entity.
		''' </summary>
		''' <returns> true if this is an anonymous name, false otherwise. </returns>
		ReadOnly Property anonymous As Boolean

		''' <summary>
		''' Tests if this name object represents a Mechanism Name (MN). An MN is
		''' a GSSName the contains exactly one mechanism's primitive name
		''' element.
		''' </summary>
		''' <returns> true if this is an MN, false otherwise. </returns>
		ReadOnly Property mN As Boolean

	End Interface

End Namespace
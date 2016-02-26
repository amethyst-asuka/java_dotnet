'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

''' <summary>
''' This package contains utility classes related to the Kerberos network
''' authentication protocol. They do not provide much Kerberos support
''' themselves.<p>
''' 
''' The Kerberos network authentication protocol is defined in
''' <a href=http://www.ietf.org/rfc/rfc4120.txt>RFC 4120</a>. The Java
''' platform contains support for the client side of Kerberos via the
''' <seealso cref="org.ietf.jgss"/> package. There might also be
''' a login module that implements
''' <seealso cref="javax.security.auth.spi.LoginModule LoginModule"/> to authenticate
''' Kerberos principals.<p>
''' 
''' You can provide the name of your default realm and Key Distribution
''' Center (KDC) host for that realm using the system properties
''' {@code java.security.krb5.realm} and {@code java.security.krb5.kdc}.
''' Both properties must be set.
''' Alternatively, the {@code java.security.krb5.conf} system property can
''' be set to the location of an MIT style {@code krb5.conf} configuration
''' file. If none of these system properties are set, the {@code krb5.conf}
''' file is searched for in an implementation-specific manner. Typically,
''' an implementation will first look for a {@code krb5.conf} file in
''' {@code <java-home>/lib/security} and failing that, in an OS-specific
''' location.<p>
''' 
''' @since JDK1.4
''' </summary>
Namespace javax.security.auth.kerberos

End Namespace
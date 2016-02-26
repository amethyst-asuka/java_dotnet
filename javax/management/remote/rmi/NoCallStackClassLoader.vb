Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.remote.rmi


	''' <summary>
	'''    <p>A class loader that only knows how to define a limited number
	'''    of classes, and load a limited number of other classes through
	'''    delegation to another loader.  It is used to get around a problem
	'''    with Serialization, in particular as used by RMI (including
	'''    RMI/IIOP).  The JMX Remote API defines exactly what class loader
	'''    must be used to deserialize arguments on the server, and return
	'''    values on the client.  We communicate this class loader to RMI by
	'''    setting it as the context class loader.  RMI uses the context
	'''    class loader to load classes as it deserializes, which is what we
	'''    want.  However, before consulting the context class loader, it
	'''    looks up the call stack for a class with a non-null class loader,
	'''    and uses that if it finds one.  So, in the standalone version of
	'''    javax.management.remote, if the class you're looking for is known
	'''    to the loader of jmxremote.jar (typically the system class loader)
	'''    then that loader will load it.  This contradicts the class-loading
	'''    semantics required.
	''' 
	'''    <p>We get around the problem by ensuring that the search up the
	'''    call stack will find a non-null class loader that doesn't load any
	'''    classes of interest, namely this one.  So even though this loader
	'''    is indeed consulted during deserialization, it never finds the
	'''    class being deserialized.  RMI then proceeds to use the context
	'''    class loader, as we require.
	''' 
	'''    <p>This loader is constructed with the name and byte-code of one
	'''    or more classes that it defines, and a class-loader to which it
	'''    will delegate certain other classes required by that byte-code.
	'''    We construct the byte-code somewhat painstakingly, by compiling
	'''    the Java code directly, converting into a string, copying that
	'''    string into the class that needs this loader, and using the
	'''    stringToBytes method to convert it into the byte array.  We
	'''    compile with -g:none because there's not much point in having
	'''    line-number information and the like in these directly-encoded
	'''    classes.
	''' 
	'''    <p>The referencedClassNames should contain the names of all
	'''    classes that are referenced by the classes defined by this loader.
	'''    It is not necessary to include standard J2SE classes, however.
	'''    Here, a class is referenced if it is the superclass or a
	'''    superinterface of a defined class, or if it is the type of a
	'''    field, parameter, or return value.  A class is not referenced if
	'''    it only appears in the throws clause of a method or constructor.
	'''    Of course, referencedClassNames should not contain any classes
	'''    that the user might want to deserialize, because the whole point
	'''    of this loader is that it does not find such classes.
	''' </summary>

	Friend Class NoCallStackClassLoader
		Inherits ClassLoader

		''' <summary>
		''' Simplified constructor when this loader only defines one class. </summary>
		Public Sub New(ByVal className As String, ByVal byteCode As SByte(), ByVal referencedClassNames As String(), ByVal referencedClassLoader As ClassLoader, ByVal protectionDomain As java.security.ProtectionDomain)
			Me.New(New String() {className}, New SByte() {byteCode}, referencedClassNames, referencedClassLoader, protectionDomain)
		End Sub

		Public Sub New(ByVal classNames As String(), ByVal byteCodes As SByte()(), ByVal referencedClassNames As String(), ByVal referencedClassLoader As ClassLoader, ByVal protectionDomain As java.security.ProtectionDomain)
			MyBase.New(Nothing)

			' Validation. 
			If classNames Is Nothing OrElse classNames.Length = 0 OrElse byteCodes Is Nothing OrElse classNames.Length <> byteCodes.Length OrElse referencedClassNames Is Nothing OrElse protectionDomain Is Nothing Then Throw New System.ArgumentException
			For i As Integer = 0 To classNames.Length - 1
				If classNames(i) Is Nothing OrElse byteCodes(i) Is Nothing Then Throw New System.ArgumentException
			Next i
			For i As Integer = 0 To referencedClassNames.Length - 1
				If referencedClassNames(i) Is Nothing Then Throw New System.ArgumentException
			Next i

			Me.classNames = classNames
			Me.byteCodes = byteCodes
			Me.referencedClassNames = referencedClassNames
			Me.referencedClassLoader = referencedClassLoader
			Me.protectionDomain = protectionDomain
		End Sub

	'     This method is called at most once per name.  Define the name
	'     * if it is one of the classes whose byte code we have, or
	'     * delegate the load if it is one of the referenced classes.
	'     
		Protected Friend Overrides Function findClass(ByVal name As String) As Type
			' Note: classNames is guaranteed by the constructor to be non-null.
			For i As Integer = 0 To classNames.Length - 1
				If name.Equals(classNames(i)) Then Return defineClass(classNames(i), byteCodes(i), 0, byteCodes(i).Length, protectionDomain)
			Next i

	'         If the referencedClassLoader is null, it is the bootstrap
	'         * class loader, and there's no point in delegating to it
	'         * because it's already our parent class loader.
	'         
			If referencedClassLoader IsNot Nothing Then
				For i As Integer = 0 To referencedClassNames.Length - 1
					If name.Equals(referencedClassNames(i)) Then Return referencedClassLoader.loadClass(name)
				Next i
			End If

			Throw New ClassNotFoundException(name)
		End Function

		Private ReadOnly classNames As String()
		Private ReadOnly byteCodes As SByte()()
		Private ReadOnly referencedClassNames As String()
		Private ReadOnly referencedClassLoader As ClassLoader
		Private ReadOnly protectionDomain As java.security.ProtectionDomain

		''' <summary>
		''' <p>Construct a <code>byte[]</code> using the characters of the
		''' given <code>String</code>.  Only the low-order byte of each
		''' character is used.  This method is useful to reduce the
		''' footprint of classes that include big byte arrays (e.g. the
		''' byte code of other classes), because a string takes up much
		''' less space in a class file than the byte code to initialize a
		''' <code>byte[]</code> with the same number of bytes.</p>
		''' 
		''' <p>We use just one byte per character even though characters
		''' contain two bytes.  The resultant output length is much the
		''' same: using one byte per character is shorter because it has
		''' more characters in the optimal 1-127 range but longer because
		''' it has more zero bytes (which are frequent, and are encoded as
		''' two bytes in classfile UTF-8).  But one byte per character has
		''' two key advantages: (1) you can see the string constants, which
		''' is reassuring, (2) you don't need to know whether the class
		''' file length is odd.</p>
		''' 
		''' <p>This method differs from <seealso cref="String#getBytes()"/> in that
		''' it does not use any encoding.  So it is guaranteed that each
		''' byte of the result is numerically identical (mod 256) to the
		''' corresponding character of the input.
		''' </summary>
		Public Shared Function stringToBytes(ByVal s As String) As SByte()
			Dim slen As Integer = s.Length
			Dim bytes As SByte() = New SByte(slen - 1){}
			For i As Integer = 0 To slen - 1
				bytes(i) = AscW(s.Chars(i))
			Next i
			Return bytes
		End Function
	End Class

	'
	'
	'You can use the following Emacs function to convert class files into
	'strings to be used by the stringToBytes method above.  Select the
	'whole (defun...) with the mouse and type M-x eval-region, or save it
	'to a file and do M-x load-file.  Then visit the *.class file and do
	'M-x class-string.
	'
	';; class-string.el
	';; visit the *.class file with emacs, then invoke this function
	'
	'(defun class-string ()
	'  "Construct a Java string whose bytes are the same as the current
	'buffer.  The resultant string is put in a buffer called *string*,
	'possibly with a numeric suffix like <2>.  From there it can be
	'insert-buffer'd into a Java program."
	'  (interactive)
	'  (let* ((s (buffer-string))
	'         (slen (length s))
	'         (i 0)
	'         (buf (generate-new-buffer "*string*")))
	'    (set-buffer buf)
	'    (insert "\"")
	'    (while (< i slen)
	'      (if (> (current-column) 61)
	'          (insert "\"+\n\""))
	'      (let ((c (aref s i)))
	'        (insert (cond
	'                 ((> c 126) (format "\\%o" c))
	'                 ((= c ?\") "\\\"")
	'                 ((= c ?\\) "\\\\")
	'                 ((< c 33)
	'                  (let ((nextc (if (< (1+ i) slen)
	'                                   (aref s (1+ i))
	'                                 ?\0)))
	'                    (cond
	'                     ((and (<= nextc ?7) (>= nextc ?0))
	'                      (format "\\%03o" c))
	'                     (t
	'                      (format "\\%o" c)))))
	'                 (t c))))
	'      (setq i (1+ i)))
	'    (insert "\"")
	'    (switch-to-buffer buf)))
	'
	'Alternatively, the following class reads a class file and outputs a string
	'that can be used by the stringToBytes method above.
	'
	'import java.io.File;
	'import java.io.FileInputStream;
	'import java.io.IOException;
	'
	'public class BytesToString {
	'
	'    public static void main(String[] args) throws IOException {
	'        File f = new File(args[0]);
	'        int len = (int)f.length();
	'        byte[] classBytes = new byte[len];
	'
	'        FileInputStream in = new FileInputStream(args[0]);
	'        try {
	'            int pos = 0;
	'            for (;;) {
	'                int n = in.read(classBytes, pos, (len-pos));
	'                if (n < 0)
	'                    throw new RuntimeException("class file changed??");
	'                pos += n;
	'                if (pos >= n)
	'                    break;
	'            }
	'        } finally {
	'            in.close();
	'        }
	'
	'        int pos = 0;
	'        boolean lastWasOctal = false;
	'        for (int i=0; i<len; i++) {
	'            int value = classBytes[i];
	'            if (value < 0)
	'                value += 256;
	'            String s = null;
	'            if (value == '\\')
	'                s = "\\\\";
	'            else if (value == '\"')
	'                s = "\\\"";
	'            else {
	'                if ((value >= 32 && value < 127) && ((!lastWasOctal ||
	'                    (value < '0' || value > '7')))) {
	'                    s = Character.toString((char)value);
	'                }
	'            }
	'            if (s == null) {
	'                s = "\\" + Integer.toString(value, 8);
	'                lastWasOctal = true;
	'            } else {
	'                lastWasOctal = false;
	'            }
	'            if (pos > 61) {
	'                System.out.print("\"");
	'                if (i<len)
	'                    System.out.print("+");
	'                System.out.println();
	'                pos = 0;
	'            }
	'            if (pos == 0)
	'                System.out.print("                \"");
	'            System.out.print(s);
	'            pos += s.length();
	'        }
	'        System.out.println("\"");
	'    }
	'}
	'
	'

End Namespace
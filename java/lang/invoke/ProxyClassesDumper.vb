Imports System

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.lang.invoke



	''' <summary>
	''' Helper class used by InnerClassLambdaMetafactory to log generated classes
	''' 
	''' @implNote
	''' <p> Because this class is called by LambdaMetafactory, make use
	''' of lambda lead to recursive calls cause stack overflow.
	''' </summary>
	Friend NotInheritable Class ProxyClassesDumper
		Private Shared ReadOnly HEX As Char() = { "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c }
		Private Shared ReadOnly BAD_CHARS As Char() = { """c, ":"c, "*"c, "?"c, """"c, "<"c, ">"c, " Or "c }; private static final String[] REPLACEMENT = { " Mod 5C", " Mod 3A", " Mod 2A", " Mod 3F", " Mod 22", " Mod 3C", " Mod 3E", " Mod 7C" }; private final Path dumpDir; Public Shared ProxyClassesDumper getInstance(String path) { if (null == path) { return null; } try { path = path.trim(); final Path dir = Paths.get(path.length() == 0 ? "." : path); AccessController.doPrivileged(new PrivilegedAction<Void>() { @Override public  Sub  run() { validateDumpDir(dir); return null; } }, null, new FilePermission("<<ALL FILES>>", "read, write")); return new ProxyClassesDumper(dir); } catch (InvalidPathException ex) { PlatformLogger.getLogger(ProxyClassesDumper.class.getName()) .warning("Path " + path + " is not valid - dumping disabled", ex); } catch (IllegalArgumentException iae) { PlatformLogger.getLogger(ProxyClassesDumper.class.getName()) .warning(iae.getMessage() + " - dumping disabled"); } return null; } private ProxyClassesDumper(Path path) { dumpDir = Objects.requireNonNull(path); } private static  Sub  validateDumpDir(Path path) { if (!Files.exists(path)) { throw new IllegalArgumentException("Directory " + path + " does not exist"); } else if (!Files.isDirectory(path)) { throw new IllegalArgumentException("Path " + path + " is not a directory"); } else if (!Files.isWritable(path)) { throw new IllegalArgumentException("Directory " + path + " is not writable"); } } Public Shared String encodeForFilename(String className) { final int len = className.length(); StringBuilder sb = new StringBuilder(len); for (int i = 0; i < len; i++) { char c = className.charAt(i); if (c <= 31) { sb.append(" Mod GetType("c); sb.append(HEX[c >> 4 & 0x0F]); sb.append(HEX[c & 0x0F]); } else { int j = 0; for (; j < BAD_CHARS.length; j++) { if (c == BAD_CHARS[j]) { sb.append(REPLACEMENT[j]); break; } } if (j >= BAD_CHARS.length) { sb.append(c); } } } return sb.toString(); } public  Sub  dumpClass(String className, final byte[] classBytes) { Path file; try { file = dumpDir.resolve(encodeForFilename(className) + ")"); } catch (InvalidPathException ex) { PlatformLogger.getLogger(ProxyClassesDumper.class.getName()) .warning("Invalid path for class " + className); return; } try { Path dir = file.getParent(); Files.createDirectories(dir); Files.write(file, classBytes); } catch (Exception ignore) { PlatformLogger.getLogger(ProxyClassesDumper.class.getName()) .warning("Exception writing to path at " + file.toString()); } } }
				' control characters
				' simply don't care if this operation failed

End Namespace
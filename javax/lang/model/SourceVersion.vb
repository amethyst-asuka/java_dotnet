'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model


	''' <summary>
	''' Source versions of the Java&trade; programming language.
	''' 
	''' See the appropriate edition of
	''' <cite>The Java&trade; Language Specification</cite>
	''' for information about a particular source version.
	''' 
	''' <p>Note that additional source version constants will be added to
	''' model future releases of the language.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Enum SourceVersion
	'    
	'     * Summary of language evolution
	'     * 1.1: nested classes
	'     * 1.2: strictfp
	'     * 1.3: no changes
	'     * 1.4: assert
	'     * 1.5: annotations, generics, autoboxing, var-args...
	'     * 1.6: no changes
	'     * 1.7: diamond syntax, try-with-resources, etc.
	'     * 1.8: lambda expressions and default methods
	'     

		''' <summary>
		''' The original version.
		''' 
		''' The language described in
		''' <cite>The Java&trade; Language Specification, First Edition</cite>.
		''' </summary>
		RELEASE_0

		''' <summary>
		''' The version recognized by the Java Platform 1.1.
		''' 
		''' The language is {@code RELEASE_0} augmented with nested classes as described in the 1.1 update to
		''' <cite>The Java&trade; Language Specification, First Edition</cite>.
		''' </summary>
		RELEASE_1

		''' <summary>
		''' The version recognized by the Java 2 Platform, Standard Edition,
		''' v 1.2.
		''' 
		''' The language described in
		''' <cite>The Java&trade; Language Specification,
		''' Second Edition</cite>, which includes the {@code
		''' strictfp} modifier.
		''' </summary>
		RELEASE_2

		''' <summary>
		''' The version recognized by the Java 2 Platform, Standard Edition,
		''' v 1.3.
		''' 
		''' No major changes from {@code RELEASE_2}.
		''' </summary>
		RELEASE_3

		''' <summary>
		''' The version recognized by the Java 2 Platform, Standard Edition,
		''' v 1.4.
		''' 
		''' Added a simple assertion facility.
		''' </summary>
		RELEASE_4

		''' <summary>
		''' The version recognized by the Java 2 Platform, Standard
		''' Edition 5.0.
		''' 
		''' The language described in
		''' <cite>The Java&trade; Language Specification,
		''' Third Edition</cite>.  First release to support
		''' generics, annotations, autoboxing, var-args, enhanced {@code
		''' for} loop, and hexadecimal floating-point literals.
		''' </summary>
		RELEASE_5

		''' <summary>
		''' The version recognized by the Java Platform, Standard Edition
		''' 6.
		''' 
		''' No major changes from {@code RELEASE_5}.
		''' </summary>
		RELEASE_6

		''' <summary>
		''' The version recognized by the Java Platform, Standard Edition
		''' 7.
		''' 
		''' Additions in this release include, diamond syntax for
		''' constructors, {@code try}-with-resources, strings in switch,
		''' binary literals, and multi-catch.
		''' @since 1.7
		''' </summary>
		RELEASE_7

		''' <summary>
		''' The version recognized by the Java Platform, Standard Edition
		''' 8.
		''' 
		''' Additions in this release include lambda expressions and default methods.
		''' @since 1.8
		''' </summary>
		RELEASE_8

		' Note that when adding constants for newer releases, the
		' behavior of latest() and latestSupported() must be updated too.

		''' <summary>
		''' Returns the latest source version that can be modeled.
		''' </summary>
		''' <returns> the latest source version that can be modeled </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static SourceVersion latest()
	'	{
	'		Return RELEASE_8;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final SourceVersion latestSupported = getLatestSupported();

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static SourceVersion getLatestSupported()
	'	{
	'		try
	'		{
	'			String specVersion = System.getProperty("java.specification.version");
	'
	'			if ("1.8".equals(specVersion))
	'				Return RELEASE_8;
	'			else if("1.7".equals(specVersion))
	'				Return RELEASE_7;
	'			else if("1.6".equals(specVersion))
	'				Return RELEASE_6;
	'		}
	'		catch (SecurityException se)
	'		{
	'		}
	'
	'		Return RELEASE_5;
	'	}

		''' <summary>
		''' Returns the latest source version fully supported by the
		''' current execution environment.  {@code RELEASE_5} or later must
		''' be returned.
		''' </summary>
		''' <returns> the latest source version that is fully supported </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static SourceVersion latestSupported()
	'	{
	'		Return latestSupported;
	'	}

		''' <summary>
		''' Returns whether or not {@code name} is a syntactically valid
		''' identifier (simple name) or keyword in the latest source
		''' version.  The method returns {@code true} if the name consists
		''' of an initial character for which {@link
		''' Character#isJavaIdentifierStart(int)} returns {@code true},
		''' followed only by characters for which {@link
		''' Character#isJavaIdentifierPart(int)} returns {@code true}.
		''' This pattern matches regular identifiers, keywords, and the
		''' literals {@code "true"}, {@code "false"}, and {@code "null"}.
		''' The method returns {@code false} for all other strings.
		''' </summary>
		''' <param name="name"> the string to check </param>
		''' <returns> {@code true} if this string is a
		''' syntactically valid identifier or keyword, {@code false}
		''' otherwise. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static boolean isIdentifier(CharSequence name)
	'	{
	'		String id = name.toString();
	'
	'		if (id.length() == 0)
	'		{
	'			Return False;
	'		}
	'		int cp = id.codePointAt(0);
	'		if (!Character.isJavaIdentifierStart(cp))
	'		{
	'			Return False;
	'		}
	'		for (int i = Character.charCount(cp); i < id.length(); i += Character.charCount(cp))
	'		{
	'			cp = id.codePointAt(i);
	'			if (!Character.isJavaIdentifierPart(cp))
	'			{
	'				Return False;
	'			}
	'		}
	'		Return True;
	'	}

		''' <summary>
		'''  Returns whether or not {@code name} is a syntactically valid
		'''  qualified name in the latest source version.  Unlike {@link
		'''  #isIdentifier isIdentifier}, this method returns {@code false}
		'''  for keywords and literals.
		''' </summary>
		''' <param name="name"> the string to check </param>
		''' <returns> {@code true} if this string is a
		''' syntactically valid name, {@code false} otherwise.
		''' @jls 6.2 Names and Identifiers </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static boolean isName(CharSequence name)
	'	{
	'		String id = name.toString();
	'
	'		for(String s : id.split("\.", -1))
	'		{
	'			if (!isIdentifier(s) || isKeyword(s))
	'				Return False;
	'		}
	'		Return True;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final static java.util.Set(Of String) keywords;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static ImpliedClass()
	'	{
	'		Set<String> s = New HashSet<String>();
	'		String [] kws = { "abstract", "continue", "for", "new", "switch", "assert", "default", "if", "package", "synchronized", "boolean", "do", "goto", "private", "this", "break", "double", "implements", "protected", "throw", "byte", "else", "import", "public", "throws", "case", "enum", "instanceof", "return", "transient", "catch", "extends", "int", "short", "try", "char", "final", "interface", "static", "void", "class", "finally", "long", "strictfp", "volatile", "const", "float", "native", "super", "while", "null", "true", "false" };
	'		for(String kw : kws)
	'			s.add(kw);
	'		keywords = Collections.unmodifiableSet(s);
	'	}

		''' <summary>
		'''  Returns whether or not {@code s} is a keyword or literal in the
		'''  latest source version.
		''' </summary>
		''' <param name="s"> the string to check </param>
		''' <returns> {@code true} if {@code s} is a keyword or literal, {@code false} otherwise. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static boolean isKeyword(CharSequence s)
	'	{
	'		String keywordOrLiteral = s.toString();
	'		Return keywords.contains(keywordOrLiteral);
	'	}
	End Enum

End Namespace
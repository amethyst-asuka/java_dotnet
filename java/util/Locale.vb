Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' * The original version of this source code and documentation
' * is copyrighted and owned by Taligent, Inc., a wholly-owned
' * subsidiary of IBM. These materials are provided under terms
' * of a License Agreement between Taligent and Sun. This technology
' * is protected by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.util



    ''' <summary>
    ''' A <code>Locale</code> object represents a specific geographical, political,
    ''' or cultural region. An operation that requires a <code>Locale</code> to perform
    ''' its task is called <em>locale-sensitive</em> and uses the <code>Locale</code>
    ''' to tailor information for the user. For example, displaying a number
    ''' is a locale-sensitive operation&mdash; the number should be formatted
    ''' according to the customs and conventions of the user's native country,
    ''' region, or culture.
    ''' 
    ''' <p> The {@code Locale} class implements IETF BCP 47 which is composed of
    ''' <a href="http://tools.ietf.org/html/rfc4647">RFC 4647 "Matching of Language
    ''' Tags"</a> and <a href="http://tools.ietf.org/html/rfc5646">RFC 5646 "Tags
    ''' for Identifying Languages"</a> with support for the LDML (UTS#35, "Unicode
    ''' Locale Data Markup Language") BCP 47-compatible extensions for locale data
    ''' exchange.
    ''' 
    ''' <p> A <code>Locale</code> object logically consists of the fields
    ''' described below.
    ''' 
    ''' <dl>
    '''   <dt><a name="def_language"><b>language</b></a></dt>
    ''' 
    '''   <dd>ISO 639 alpha-2 or alpha-3 language code, or registered
    '''   language subtags up to 8 alpha letters (for future enhancements).
    '''   When a language has both an alpha-2 code and an alpha-3 code, the
    '''   alpha-2 code must be used.  You can find a full list of valid
    '''   language codes in the IANA Language Subtag Registry (search for
    '''   "Type: language").  The language field is case insensitive, but
    '''   <code>Locale</code> always canonicalizes to lower case.</dd>
    ''' 
    '''   <dd>Well-formed language values have the form
    '''   <code>[a-zA-Z]{2,8}</code>.  Note that this is not the the full
    '''   BCP47 language production, since it excludes extlang.  They are
    '''   not needed since modern three-letter language codes replace
    '''   them.</dd>
    ''' 
    '''   <dd>Example: "en" (English), "ja" (Japanese), "kok" (Konkani)</dd>
    ''' 
    '''   <dt><a name="def_script"><b>script</b></a></dt>
    ''' 
    '''   <dd>ISO 15924 alpha-4 script code.  You can find a full list of
    '''   valid script codes in the IANA Language Subtag Registry (search
    '''   for "Type: script").  The script field is case insensitive, but
    '''   <code>Locale</code> always canonicalizes to title case (the first
    '''   letter is upper case and the rest of the letters are lower
    '''   case).</dd>
    ''' 
    '''   <dd>Well-formed script values have the form
    '''   <code>[a-zA-Z]{4}</code></dd>
    ''' 
    '''   <dd>Example: "Latn" (Latin), "Cyrl" (Cyrillic)</dd>
    ''' 
    '''   <dt><a name="def_region"><b>country (region)</b></a></dt>
    ''' 
    '''   <dd>ISO 3166 alpha-2 country code or UN M.49 numeric-3 area code.
    '''   You can find a full list of valid country and region codes in the
    '''   IANA Language Subtag Registry (search for "Type: region").  The
    '''   country (region) field is case insensitive, but
    '''   <code>Locale</code> always canonicalizes to upper case.</dd>
    ''' 
    '''   <dd>Well-formed country/region values have
    '''   the form <code>[a-zA-Z]{2} | [0-9]{3}</code></dd>
    ''' 
    '''   <dd>Example: "US" (United States), "FR" (France), "029"
    '''   (Caribbean)</dd>
    ''' 
    '''   <dt><a name="def_variant"><b>variant</b></a></dt>
    ''' 
    '''   <dd>Any arbitrary value used to indicate a variation of a
    '''   <code>Locale</code>.  Where there are two or more variant values
    '''   each indicating its own semantics, these values should be ordered
    '''   by importance, with most important first, separated by
    '''   underscore('_').  The variant field is case sensitive.</dd>
    ''' 
    '''   <dd>Note: IETF BCP 47 places syntactic restrictions on variant
    '''   subtags.  Also BCP 47 subtags are strictly used to indicate
    '''   additional variations that define a language or its dialects that
    '''   are not covered by any combinations of language, script and
    '''   region subtags.  You can find a full list of valid variant codes
    '''   in the IANA Language Subtag Registry (search for "Type: variant").
    ''' 
    '''   <p>However, the variant field in <code>Locale</code> has
    '''   historically been used for any kind of variation, not just
    '''   language variations.  For example, some supported variants
    '''   available in Java SE Runtime Environments indicate alternative
    '''   cultural behaviors such as calendar type or number script.  In
    '''   BCP 47 this kind of information, which does not identify the
    '''   language, is supported by extension subtags or private use
    '''   subtags.</dd>
    ''' 
    '''   <dd>Well-formed variant values have the form <code>SUBTAG
    '''   (('_'|'-') SUBTAG)*</code> where <code>SUBTAG =
    '''   [0-9][0-9a-zA-Z]{3} | [0-9a-zA-Z]{5,8}</code>. (Note: BCP 47 only
    '''   uses hyphen ('-') as a delimiter, this is more lenient).</dd>
    ''' 
    '''   <dd>Example: "polyton" (Polytonic Greek), "POSIX"</dd>
    ''' 
    '''   <dt><a name="def_extensions"><b>extensions</b></a></dt>
    ''' 
    '''   <dd>A map from single character keys to string values, indicating
    '''   extensions apart from language identification.  The extensions in
    '''   <code>Locale</code> implement the semantics and syntax of BCP 47
    '''   extension subtags and private use subtags. The extensions are
    '''   case insensitive, but <code>Locale</code> canonicalizes all
    '''   extension keys and values to lower case. Note that extensions
    '''   cannot have empty values.</dd>
    ''' 
    '''   <dd>Well-formed keys are single characters from the set
    '''   <code>[0-9a-zA-Z]</code>.  Well-formed values have the form
    '''   <code>SUBTAG ('-' SUBTAG)*</code> where for the key 'x'
    '''   <code>SUBTAG = [0-9a-zA-Z]{1,8}</code> and for other keys
    '''   <code>SUBTAG = [0-9a-zA-Z]{2,8}</code> (that is, 'x' allows
    '''   single-character subtags).</dd>
    ''' 
    '''   <dd>Example: key="u"/value="ca-japanese" (Japanese Calendar),
    '''   key="x"/value="java-1-7"</dd>
    ''' </dl>
    ''' 
    ''' <b>Note:</b> Although BCP 47 requires field values to be registered
    ''' in the IANA Language Subtag Registry, the <code>Locale</code> class
    ''' does not provide any validation features.  The <code>Builder</code>
    ''' only checks if an individual field satisfies the syntactic
    ''' requirement (is well-formed), but does not validate the value
    ''' itself.  See <seealso cref="Builder"/> for details.
    ''' 
    ''' <h3><a name="def_locale_extension">Unicode locale/language extension</a></h3>
    ''' 
    ''' <p>UTS#35, "Unicode Locale Data Markup Language" defines optional
    ''' attributes and keywords to override or refine the default behavior
    ''' associated with a locale.  A keyword is represented by a pair of
    ''' key and type.  For example, "nu-thai" indicates that Thai local
    ''' digits (value:"thai") should be used for formatting numbers
    ''' (key:"nu").
    ''' 
    ''' <p>The keywords are mapped to a BCP 47 extension value using the
    ''' extension key 'u' (<seealso cref="#UNICODE_LOCALE_EXTENSION"/>).  The above
    ''' example, "nu-thai", becomes the extension "u-nu-thai".code
    ''' 
    ''' <p>Thus, when a <code>Locale</code> object contains Unicode locale
    ''' attributes and keywords,
    ''' <code>getExtension(UNICODE_LOCALE_EXTENSION)</code> will return a
    ''' String representing this information, for example, "nu-thai".  The
    ''' <code>Locale</code> class also provides {@link
    ''' #getUnicodeLocaleAttributes}, <seealso cref="#getUnicodeLocaleKeys"/>, and
    ''' <seealso cref="#getUnicodeLocaleType"/> which allow you to access Unicode
    ''' locale attributes and key/type pairs directly.  When represented as
    ''' a string, the Unicode Locale Extension lists attributes
    ''' alphabetically, followed by key/type sequences with keys listed
    ''' alphabetically (the order of subtags comprising a key's type is
    ''' fixed when the type is defined)
    ''' 
    ''' <p>A well-formed locale key has the form
    ''' <code>[0-9a-zA-Z]{2}</code>.  A well-formed locale type has the
    ''' form <code>"" | [0-9a-zA-Z]{3,8} ('-' [0-9a-zA-Z]{3,8})*</code> (it
    ''' can be empty, or a series of subtags 3-8 alphanums in length).  A
    ''' well-formed locale attribute has the form
    ''' <code>[0-9a-zA-Z]{3,8}</code> (it is a single subtag with the same
    ''' form as a locale type subtag).
    ''' 
    ''' <p>The Unicode locale extension specifies optional behavior in
    ''' locale-sensitive services.  Although the LDML specification defines
    ''' various keys and values, actual locale-sensitive service
    ''' implementations in a Java Runtime Environment might not support any
    ''' particular Unicode locale attributes or key/type pairs.
    ''' 
    ''' <h4>Creating a Locale</h4>
    ''' 
    ''' <p>There are several different ways to create a <code>Locale</code>
    ''' object.
    ''' 
    ''' <h5>Builder</h5>
    ''' 
    ''' <p>Using <seealso cref="Builder"/> you can construct a <code>Locale</code> object
    ''' that conforms to BCP 47 syntax.
    ''' 
    ''' <h5>Constructors</h5>
    ''' 
    ''' <p>The <code>Locale</code> class provides three constructors:
    ''' <blockquote>
    ''' <pre>
    '''     <seealso cref="#Locale(String language)"/>
    '''     <seealso cref="#Locale(String language, String country)"/>
    '''     <seealso cref="#Locale(String language, String country, String variant)"/>
    ''' </pre>
    ''' </blockquote>
    ''' These constructors allow you to create a <code>Locale</code> object
    ''' with language, country and variant, but you cannot specify
    ''' script or extensions.
    ''' 
    ''' <h5>Factory Methods</h5>
    ''' 
    ''' <p>The method <seealso cref="#forLanguageTag"/> creates a <code>Locale</code>
    ''' object for a well-formed BCP 47 language tag.
    ''' 
    ''' <h5>Locale Constants</h5>
    ''' 
    ''' <p>The <code>Locale</code> class provides a number of convenient constants
    ''' that you can use to create <code>Locale</code> objects for commonly used
    ''' locales. For example, the following creates a <code>Locale</code> object
    ''' for the United States:
    ''' <blockquote>
    ''' <pre>
    '''     Locale.US
    ''' </pre>
    ''' </blockquote>
    ''' 
    ''' <h4><a name="LocaleMatching">Locale Matching</a></h4>
    ''' 
    ''' <p>If an application or a system is internationalized and provides localized
    ''' resources for multiple locales, it sometimes needs to find one or more
    ''' locales (or language tags) which meet each user's specific preferences. Note
    ''' that a term "language tag" is used interchangeably with "locale" in this
    ''' locale matching documentation.
    ''' 
    ''' <p>In order to do matching a user's preferred locales to a set of language
    ''' tags, <a href="http://tools.ietf.org/html/rfc4647">RFC 4647 Matching of
    ''' Language Tags</a> defines two mechanisms: filtering and lookup.
    ''' <em>Filtering</em> is used to get all matching locales, whereas
    ''' <em>lookup</em> is to choose the best matching locale.
    ''' Matching is done case-insensitively. These matching mechanisms are described
    ''' in the following sections.
    ''' 
    ''' <p>A user's preference is called a <em>Language Priority List</em> and is
    ''' expressed as a list of language ranges. There are syntactically two types of
    ''' language ranges: basic and extended. See
    ''' <seealso cref="Locale.LanguageRange Locale.LanguageRange"/> for details.
    ''' 
    ''' <h5>Filtering</h5>
    ''' 
    ''' <p>The filtering operation returns all matching language tags. It is defined
    ''' in RFC 4647 as follows:
    ''' "In filtering, each language range represents the least specific language
    ''' tag (that is, the language tag with fewest number of subtags) that is an
    ''' acceptable match. All of the language tags in the matching set of tags will
    ''' have an equal or greater number of subtags than the language range. Every
    ''' non-wildcard subtag in the language range will appear in every one of the
    ''' matching language tags."
    ''' 
    ''' <p>There are two types of filtering: filtering for basic language ranges
    ''' (called "basic filtering") and filtering for extended language ranges
    ''' (called "extended filtering"). They may return different results by what
    ''' kind of language ranges are included in the given Language Priority List.
    ''' <seealso cref="Locale.FilteringMode"/> is a parameter to specify how filtering should
    ''' be done.
    ''' 
    ''' <h5>Lookup</h5>
    ''' 
    ''' <p>The lookup operation returns the best matching language tags. It is
    ''' defined in RFC 4647 as follows:
    ''' "By contrast with filtering, each language range represents the most
    ''' specific tag that is an acceptable match.  The first matching tag found,
    ''' according to the user's priority, is considered the closest match and is the
    ''' item returned."
    ''' 
    ''' <p>For example, if a Language Priority List consists of two language ranges,
    ''' {@code "zh-Hant-TW"} and {@code "en-US"}, in prioritized order, lookup
    ''' method progressively searches the language tags below in order to find the
    ''' best matching language tag.
    ''' <blockquote>
    ''' <pre>
    '''    1. zh-Hant-TW
    '''    2. zh-Hant
    '''    3. zh
    '''    4. en-US
    '''    5. en
    ''' </pre>
    ''' </blockquote>
    ''' If there is a language tag which matches completely to a language range
    ''' above, the language tag is returned.
    ''' 
    ''' <p>{@code "*"} is the special language range, and it is ignored in lookup.
    ''' 
    ''' <p>If multiple language tags match as a result of the subtag {@code '*'}
    ''' included in a language range, the first matching language tag returned by
    ''' an <seealso cref="Iterator"/> over a <seealso cref="Collection"/> of language tags is treated as
    ''' the best matching one.
    ''' 
    ''' <h4>Use of Locale</h4>
    ''' 
    ''' <p>Once you've created a <code>Locale</code> you can query it for information
    ''' about itself. Use <code>getCountry</code> to get the country (or region)
    ''' code and <code>getLanguage</code> to get the language code.
    ''' You can use <code>getDisplayCountry</code> to get the
    ''' name of the country suitable for displaying to the user. Similarly,
    ''' you can use <code>getDisplayLanguage</code> to get the name of
    ''' the language suitable for displaying to the user. Interestingly,
    ''' the <code>getDisplayXXX</code> methods are themselves locale-sensitive
    ''' and have two versions: one that uses the default
    ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale and one
    ''' that uses the locale specified as an argument.
    ''' 
    ''' <p>The Java Platform provides a number of classes that perform locale-sensitive
    ''' operations. For example, the <code>NumberFormat</code> class formats
    ''' numbers, currency, and percentages in a locale-sensitive manner. Classes
    ''' such as <code>NumberFormat</code> have several convenience methods
    ''' for creating a default object of that type. For example, the
    ''' <code>NumberFormat</code> class provides these three convenience methods
    ''' for creating a default <code>NumberFormat</code> object:
    ''' <blockquote>
    ''' <pre>
    '''     NumberFormat.getInstance()
    '''     NumberFormat.getCurrencyInstance()
    '''     NumberFormat.getPercentInstance()
    ''' </pre>
    ''' </blockquote>
    ''' Each of these methods has two variants; one with an explicit locale
    ''' and one without; the latter uses the default
    ''' <seealso cref="Locale.Category#FORMAT FORMAT"/> locale:
    ''' <blockquote>
    ''' <pre>
    '''     NumberFormat.getInstance(myLocale)
    '''     NumberFormat.getCurrencyInstance(myLocale)
    '''     NumberFormat.getPercentInstance(myLocale)
    ''' </pre>
    ''' </blockquote>
    ''' A <code>Locale</code> is the mechanism for identifying the kind of object
    ''' (<code>NumberFormat</code>) that you would like to get. The locale is
    ''' <STRONG>just</STRONG> a mechanism for identifying objects,
    ''' <STRONG>not</STRONG> a container for the objects themselves.
    ''' 
    ''' <h4>Compatibility</h4>
    ''' 
    ''' <p>In order to maintain compatibility with existing usage, Locale's
    ''' constructors retain their behavior prior to the Java Runtime
    ''' Environment version 1.7.  The same is largely true for the
    ''' <code>toString</code> method. Thus Locale objects can continue to
    ''' be used as they were. In particular, clients who parse the output
    ''' of toString into language, country, and variant fields can continue
    ''' to do so (although this is strongly discouraged), although the
    ''' variant field will have additional information in it if script or
    ''' extensions are present.
    ''' 
    ''' <p>In addition, BCP 47 imposes syntax restrictions that are not
    ''' imposed by Locale's constructors. This means that conversions
    ''' between some Locales and BCP 47 language tags cannot be made without
    ''' losing information. Thus <code>toLanguageTag</code> cannot
    ''' represent the state of locales whose language, country, or variant
    ''' do not conform to BCP 47.
    ''' 
    ''' <p>Because of these issues, it is recommended that clients migrate
    ''' away from constructing non-conforming locales and use the
    ''' <code>forLanguageTag</code> and <code>Locale.Builder</code> APIs instead.
    ''' Clients desiring a string representation of the complete locale can
    ''' then always rely on <code>toLanguageTag</code> for this purpose.
    ''' 
    ''' <h5><a name="special_cases_constructor">Special cases</a></h5>
    ''' 
    ''' <p>For compatibility reasons, two
    ''' non-conforming locales are treated as special cases.  These are
    ''' <b><tt>ja_JP_JP</tt></b> and <b><tt>th_TH_TH</tt></b>. These are ill-formed
    ''' in BCP 47 since the variants are too  java.lang.[Short]. To ease migration to BCP 47,
    ''' these are treated specially during construction.  These two cases (and only
    ''' these) cause a constructor to generate an extension, all other values behave
    ''' exactly as they did prior to Java 7.
    ''' 
    ''' <p>Java has used <tt>ja_JP_JP</tt> to represent Japanese as used in
    ''' Japan together with the Japanese Imperial calendar. This is now
    ''' representable using a Unicode locale extension, by specifying the
    ''' Unicode locale key <tt>ca</tt> (for "calendar") and type
    ''' <tt>japanese</tt>. When the Locale constructor is called with the
    ''' arguments "ja", "JP", "JP", the extension "u-ca-japanese" is
    ''' automatically added.
    ''' 
    ''' <p>Java has used <tt>th_TH_TH</tt> to represent Thai as used in
    ''' Thailand together with Thai digits. This is also now representable using
    ''' a Unicode locale extension, by specifying the Unicode locale key
    ''' <tt>nu</tt> (for "number") and value <tt>thai</tt>. When the Locale
    ''' constructor is called with the arguments "th", "TH", "TH", the
    ''' extension "u-nu-thai" is automatically added.
    ''' 
    ''' <h5>Serialization</h5>
    ''' 
    ''' <p>During serialization, writeObject writes all fields to the output
    ''' stream, including extensions.
    ''' 
    ''' <p>During deserialization, readResolve adds extensions as described
    ''' in <a href="#special_cases_constructor">Special Cases</a>, only
    ''' for the two cases th_TH_TH and ja_JP_JP.
    ''' 
    ''' <h5>Legacy language codes</h5>
    ''' 
    ''' <p>Locale's constructor has always converted three language codes to
    ''' their earlier, obsoleted forms: <tt>he</tt> maps to <tt>iw</tt>,
    ''' <tt>yi</tt> maps to <tt>ji</tt>, and <tt>id</tt> maps to
    ''' <tt>in</tt>.  This continues to be the case, in order to not break
    ''' backwards compatibility.
    ''' 
    ''' <p>The APIs added in 1.7 map between the old and new language codes,
    ''' maintaining the old codes internal to Locale (so that
    ''' <code>getLanguage</code> and <code>toString</code> reflect the old
    ''' code), but using the new codes in the BCP 47 language tag APIs (so
    ''' that <code>toLanguageTag</code> reflects the new one). This
    ''' preserves the equivalence between Locales no matter which code or
    ''' API is used to construct them. Java's default resource bundle
    ''' lookup mechanism also implements this mapping, so that resources
    ''' can be named using either convention, see <seealso cref="ResourceBundle.Control"/>.
    ''' 
    ''' <h5>Three-letter language/country(region) codes</h5>
    ''' 
    ''' <p>The Locale constructors have always specified that the language
    ''' and the country param be two characters in length, although in
    ''' practice they have accepted any length.  The specification has now
    ''' been relaxed to allow language codes of two to eight characters and
    ''' country (region) codes of two to three characters, and in
    ''' particular, three-letter language codes and three-digit region
    ''' codes as specified in the IANA Language Subtag Registry.  For
    ''' compatibility, the implementation still does not impose a length
    ''' constraint.
    ''' </summary>
    ''' <seealso cref= Builder </seealso>
    ''' <seealso cref= ResourceBundle </seealso>
    ''' <seealso cref= java.text.Format </seealso>
    ''' <seealso cref= java.text.NumberFormat </seealso>
    ''' <seealso cref= java.text.Collator
    ''' @author Mark Davis
    ''' @since 1.1 </seealso>
    <Serializable>
    Public NotInheritable Class Locale : Inherits java.lang.Object
        Implements Cloneable

		Private Shared ReadOnly LOCALECACHE As New Cache

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly ENGLISH As Locale = createConstant("en", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly FRENCH As Locale = createConstant("fr", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly GERMAN As Locale = createConstant("de", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly ITALIAN As Locale = createConstant("it", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly JAPANESE As Locale = createConstant("ja", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly KOREAN As Locale = createConstant("ko", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly CHINESE As Locale = createConstant("zh", "")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly SIMPLIFIED_CHINESE As Locale = createConstant("zh", "CN")

		''' <summary>
		''' Useful constant for language.
		''' </summary>
		Public Shared ReadOnly TRADITIONAL_CHINESE As Locale = createConstant("zh", "TW")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly FRANCE As Locale = createConstant("fr", "FR")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly GERMANY As Locale = createConstant("de", "DE")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly ITALY As Locale = createConstant("it", "IT")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly JAPAN As Locale = createConstant("ja", "JP")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly KOREA As Locale = createConstant("ko", "KR")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly CHINA As Locale = SIMPLIFIED_CHINESE

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly PRC As Locale = SIMPLIFIED_CHINESE

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly TAIWAN As Locale = TRADITIONAL_CHINESE

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly UK As Locale = createConstant("en", "GB")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly US As Locale = createConstant("en", "US")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly CANADA As Locale = createConstant("en", "CA")

		''' <summary>
		''' Useful constant for country.
		''' </summary>
		Public Shared ReadOnly CANADA_FRENCH As Locale = createConstant("fr", "CA")

		''' <summary>
		''' Useful constant for the root locale.  The root locale is the locale whose
		''' language, country, and variant are empty ("") strings.  This is regarded
		''' as the base locale of all locales, and is used as the language/country
		''' neutral locale for the locale sensitive operations.
		''' 
		''' @since 1.6
		''' </summary>
		Public Shared ReadOnly ROOT As Locale = createConstant("", "")

		''' <summary>
		''' The key for the private use extension ('x').
		''' </summary>
		''' <seealso cref= #getExtension(char) </seealso>
		''' <seealso cref= Builder#setExtension(char, String)
		''' @since 1.7 </seealso>
		Public Const PRIVATE_USE_EXTENSION As Char = "x"c

		''' <summary>
		''' The key for Unicode locale extension ('u').
		''' </summary>
		''' <seealso cref= #getExtension(char) </seealso>
		''' <seealso cref= Builder#setExtension(char, String)
		''' @since 1.7 </seealso>
		Public Const UNICODE_LOCALE_EXTENSION As Char = "u"c

		''' <summary>
		''' serialization ID
		''' </summary>
		Friend Const serialVersionUID As Long = 9149081749638150636L

		''' <summary>
		''' Display types for retrieving localized names from the name providers.
		''' </summary>
		Private Const DISPLAY_LANGUAGE As Integer = 0
		Private Const DISPLAY_COUNTRY As Integer = 1
		Private Const DISPLAY_VARIANT As Integer = 2
		Private Const DISPLAY_SCRIPT As Integer = 3

		''' <summary>
		''' Private constructor used by getInstance method
		''' </summary>
		Private Sub New(  baseLocale As sun.util.locale.BaseLocale,   extensions As sun.util.locale.LocaleExtensions)
			Me.baseLocale = baseLocale
			Me.localeExtensions = extensions
		End Sub

		''' <summary>
		''' Construct a locale from language, country and variant.
		''' This constructor normalizes the language value to lowercase and
		''' the country value to uppercase.
		''' <p>
		''' <b>Note:</b>
		''' <ul>
		''' <li>ISO 639 is not a stable standard; some of the language codes it defines
		''' (specifically "iw", "ji", and "in") have changed.  This constructor accepts both the
		''' old codes ("iw", "ji", and "in") and the new codes ("he", "yi", and "id"), but all other
		''' API on Locale will return only the OLD codes.
		''' <li>For backward compatibility reasons, this constructor does not make
		''' any syntactic checks on the input.
		''' <li>The two cases ("ja", "JP", "JP") and ("th", "TH", "TH") are handled specially,
		''' see <a href="#special_cases_constructor">Special Cases</a> for more information.
		''' </ul>
		''' </summary>
		''' <param name="language"> An ISO 639 alpha-2 or alpha-3 language code, or a language subtag
		''' up to 8 characters in length.  See the <code>Locale</code> class description about
		''' valid language values. </param>
		''' <param name="country"> An ISO 3166 alpha-2 country code or a UN M.49 numeric-3 area code.
		''' See the <code>Locale</code> class description about valid country values. </param>
		''' <param name="variant"> Any arbitrary value used to indicate a variation of a <code>Locale</code>.
		''' See the <code>Locale</code> class description for the details. </param>
		''' <exception cref="NullPointerException"> thrown if any argument is null. </exception>
		Public Sub New(  language As String,   country As String,   [variant] As String)
			If language Is Nothing OrElse country Is Nothing OrElse [variant] Is Nothing Then Throw New NullPointerException
			baseLocale = sun.util.locale.BaseLocale.getInstance(convertOldISOCodes(language), "", country, [variant])
			localeExtensions = getCompatibilityExtensions(language, "", country, [variant])
		End Sub

		''' <summary>
		''' Construct a locale from language and country.
		''' This constructor normalizes the language value to lowercase and
		''' the country value to uppercase.
		''' <p>
		''' <b>Note:</b>
		''' <ul>
		''' <li>ISO 639 is not a stable standard; some of the language codes it defines
		''' (specifically "iw", "ji", and "in") have changed.  This constructor accepts both the
		''' old codes ("iw", "ji", and "in") and the new codes ("he", "yi", and "id"), but all other
		''' API on Locale will return only the OLD codes.
		''' <li>For backward compatibility reasons, this constructor does not make
		''' any syntactic checks on the input.
		''' </ul>
		''' </summary>
		''' <param name="language"> An ISO 639 alpha-2 or alpha-3 language code, or a language subtag
		''' up to 8 characters in length.  See the <code>Locale</code> class description about
		''' valid language values. </param>
		''' <param name="country"> An ISO 3166 alpha-2 country code or a UN M.49 numeric-3 area code.
		''' See the <code>Locale</code> class description about valid country values. </param>
		''' <exception cref="NullPointerException"> thrown if either argument is null. </exception>
		Public Sub New(  language As String,   country As String)
			Me.New(language, country, "")
		End Sub

		''' <summary>
		''' Construct a locale from a language code.
		''' This constructor normalizes the language value to lowercase.
		''' <p>
		''' <b>Note:</b>
		''' <ul>
		''' <li>ISO 639 is not a stable standard; some of the language codes it defines
		''' (specifically "iw", "ji", and "in") have changed.  This constructor accepts both the
		''' old codes ("iw", "ji", and "in") and the new codes ("he", "yi", and "id"), but all other
		''' API on Locale will return only the OLD codes.
		''' <li>For backward compatibility reasons, this constructor does not make
		''' any syntactic checks on the input.
		''' </ul>
		''' </summary>
		''' <param name="language"> An ISO 639 alpha-2 or alpha-3 language code, or a language subtag
		''' up to 8 characters in length.  See the <code>Locale</code> class description about
		''' valid language values. </param>
		''' <exception cref="NullPointerException"> thrown if argument is null.
		''' @since 1.4 </exception>
		Public Sub New(  language As String)
			Me.New(language, "", "")
		End Sub

		''' <summary>
		''' This method must be called only for creating the Locale.*
		''' constants due to making shortcuts.
		''' </summary>
		Private Shared Function createConstant(  lang As String,   country As String) As Locale
			Dim base As sun.util.locale.BaseLocale = sun.util.locale.BaseLocale.createInstance(lang, country)
			Return getInstance(base, Nothing)
		End Function

		''' <summary>
		''' Returns a <code>Locale</code> constructed from the given
		''' <code>language</code>, <code>country</code> and
		''' <code>variant</code>. If the same <code>Locale</code> instance
		''' is available in the cache, then that instance is
		''' returned. Otherwise, a new <code>Locale</code> instance is
		''' created and cached.
		''' </summary>
		''' <param name="language"> lowercase 2 to 8 language code. </param>
		''' <param name="country"> uppercase two-letter ISO-3166 code and numric-3 UN M.49 area code. </param>
		''' <param name="variant"> vendor and browser specific code. See class description. </param>
		''' <returns> the <code>Locale</code> instance requested </returns>
		''' <exception cref="NullPointerException"> if any argument is null. </exception>
		Shared Function getInstance(  language As String,   country As String,   [variant] As String) As Locale
			Return getInstance(language, "", country, [variant], Nothing)
		End Function

		Shared Function getInstance(  language As String,   script As String,   country As String,   [variant] As String,   extensions As sun.util.locale.LocaleExtensions) As Locale
			If language Is Nothing OrElse script Is Nothing OrElse country Is Nothing OrElse [variant] Is Nothing Then Throw New NullPointerException

			If extensions Is Nothing Then extensions = getCompatibilityExtensions(language, script, country, [variant])

			Dim baseloc As sun.util.locale.BaseLocale = sun.util.locale.BaseLocale.getInstance(language, script, country, [variant])
			Return getInstance(baseloc, extensions)
		End Function

		Shared Function getInstance(  baseloc As sun.util.locale.BaseLocale,   extensions As sun.util.locale.LocaleExtensions) As Locale
			Dim key As New LocaleKey(baseloc, extensions)
			Return LOCALECACHE.get(key)
		End Function

		Private Class Cache
			Inherits sun.util.locale.LocaleObjectCache(Of LocaleKey, Locale)

			Private Sub New()
			End Sub

			Protected Friend Overrides Function createObject(  key As LocaleKey) As Locale
				Return New Locale(key.base, key.exts)
			End Function
		End Class

		Private NotInheritable Class LocaleKey
			Private ReadOnly base As sun.util.locale.BaseLocale
			Private ReadOnly exts As sun.util.locale.LocaleExtensions
			Private ReadOnly hash As Integer

			Private Sub New(  baseLocale As sun.util.locale.BaseLocale,   extensions As sun.util.locale.LocaleExtensions)
				base = baseLocale
				exts = extensions

				' Calculate the hash value here because it's always used.
				Dim h As Integer = base.GetHashCode()
				If exts IsNot Nothing Then h = h Xor exts.GetHashCode()
				hash = h
			End Sub

			Public Overrides Function Equals(  obj As Object) As Boolean
				If Me Is obj Then Return True
				If Not(TypeOf obj Is LocaleKey) Then Return False
				Dim other As LocaleKey = CType(obj, LocaleKey)
				If hash <> other.hash OrElse (Not base.Equals(other.base)) Then Return False
				If exts Is Nothing Then Return other.exts Is Nothing
				Return exts.Equals(other.exts)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function
		End Class

        ''' <summary>
        ''' Gets the current value of the default locale for this instance
        ''' of the Java Virtual Machine.
        ''' <p>
        ''' The Java Virtual Machine sets the default locale during startup
        ''' based on the host environment. It is used by many locale-sensitive
        ''' methods if no locale is explicitly specified.
        ''' It can be changed using the
        ''' <seealso cref="#setDefault(java.util.Locale) setDefault"/> method.
        ''' </summary>
        ''' <returns> the default locale for this instance of the Java Virtual Machine </returns>
        Public Shared Property [default] As Locale
            Get
                ' do not synchronize this method - see 4071298
                Return defaultLocale
            End Get
            Set(  newLocale As Locale)
                defaultult(Category.DISPLAY, newLocale)
                defaultult(Category.FORMAT, newLocale)
                defaultLocale = newLocale
            End Set
        End Property

        ''' <summary>
        ''' Gets the current value of the default locale for the specified Category
        ''' for this instance of the Java Virtual Machine.
        ''' <p>
        ''' The Java Virtual Machine sets the default locale during startup based
        ''' on the host environment. It is used by many locale-sensitive methods
        ''' if no locale is explicitly specified. It can be changed using the
        ''' setDefault(Locale.Category, Locale) method.
        ''' </summary>
        ''' <param name="category"> - the specified category to get the default locale </param>
        ''' <exception cref="NullPointerException"> - if category is null </exception>
        ''' <returns> the default locale for the specified Category for this instance
        '''     of the Java Virtual Machine </returns>
        ''' <seealso cref= #setDefault(Locale.Category, Locale)
        ''' @since 1.7 </seealso>
        Public Shared Function getDefault(  category As Locale.Category) As Locale
			' do not synchronize this method - see 4071298
			Select Case category
			Case Locale.Category.DISPLAY
				If defaultDisplayLocale Is Nothing Then
					SyncLock GetType(Locale)
						If defaultDisplayLocale Is Nothing Then defaultDisplayLocale = initDefault(category)
					End SyncLock
				End If
				Return defaultDisplayLocale
			Case Locale.Category.FORMAT
				If defaultFormatLocale Is Nothing Then
					SyncLock GetType(Locale)
						If defaultFormatLocale Is Nothing Then defaultFormatLocale = initDefault(category)
					End SyncLock
				End If
				Return defaultFormatLocale
			Case Else
				Debug.Assert(False, "Unknown Category")
			End Select
			Return [default]
		End Function

		Private Shared Function initDefault() As Locale
			Dim language_Renamed, region, script_Renamed, country_Renamed, variant_Renamed As String
			language_Renamed = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.language", "en"))
			' for compatibility, check for old user.region property
			region = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.region"))
			If region IsNot Nothing Then
				' region can be of form country, country_variant, or _variant
				Dim i As Integer = region.IndexOf("_"c)
				If i >= 0 Then
					country_Renamed = region.Substring(0, i)
					variant_Renamed = region.Substring(i + 1)
				Else
					country_Renamed = region
					variant_Renamed = ""
				End If
				script_Renamed = ""
			Else
				script_Renamed = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.script", ""))
				country_Renamed = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.country", ""))
				variant_Renamed = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.variant", ""))
			End If

			Return getInstance(language_Renamed, script_Renamed, country_Renamed, variant_Renamed, Nothing)
		End Function

		Private Shared Function initDefault(  category As Locale.Category) As Locale
			Return getInstance(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(category.languageKey, defaultLocale.language)), java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(category.scriptKey, defaultLocale.script)), java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(category.countryKey, defaultLocale.country)), java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(category.variantKey, defaultLocale.variant)), Nothing)
		End Function


		''' <summary>
		''' Sets the default locale for the specified Category for this instance
		''' of the Java Virtual Machine. This does not affect the host locale.
		''' <p>
		''' If there is a security manager, its checkPermission method is called
		''' with a PropertyPermission("user.language", "write") permission before
		''' the default locale is changed.
		''' <p>
		''' The Java Virtual Machine sets the default locale during startup based
		''' on the host environment. It is used by many locale-sensitive methods
		''' if no locale is explicitly specified.
		''' <p>
		''' Since changing the default locale may affect many different areas of
		''' functionality, this method should only be used if the caller is
		''' prepared to reinitialize locale-sensitive code running within the
		''' same Java Virtual Machine.
		''' <p>
		''' </summary>
		''' <param name="category"> - the specified category to set the default locale </param>
		''' <param name="newLocale"> - the new default locale </param>
		''' <exception cref="SecurityException"> - if a security manager exists and its
		'''     checkPermission method doesn't allow the operation. </exception>
		''' <exception cref="NullPointerException"> - if category and/or newLocale is null </exception>
		''' <seealso cref= SecurityManager#checkPermission(java.security.Permission) </seealso>
		''' <seealso cref= PropertyPermission </seealso>
		''' <seealso cref= #getDefault(Locale.Category)
		''' @since 1.7 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub setDefault(  category As Locale.Category,   newLocale As Locale)
			If category Is Nothing Then Throw New NullPointerException("Category cannot be NULL")
			If newLocale Is Nothing Then Throw New NullPointerException("Can't set default locale to NULL")

			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New PropertyPermission("user.language", "write"))
			Select Case category
			Case Locale.Category.DISPLAY
				defaultDisplayLocale = newLocale
			Case Locale.Category.FORMAT
				defaultFormatLocale = newLocale
			Case Else
				Debug.Assert(False, "Unknown Category")
			End Select
		End Sub

        ''' <summary>
        ''' Returns an array of all installed locales.
        ''' The returned array represents the union of locales supported
        ''' by the Java runtime environment and by installed
        ''' <seealso cref="java.util.spi.LocaleServiceProvider LocaleServiceProvider"/>
        ''' implementations.  It must contain at least a <code>Locale</code>
        ''' instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
        ''' </summary>
        ''' <returns> An array of installed locales. </returns>
        Public Shared Property availableLocales As Locale()
            Get
                Return sun.util.locale.provider.LocaleServiceProviderPool.allAvailableLocales
            End Get
        End Property

        ''' <summary>
        ''' Returns a list of all 2-letter country codes defined in ISO 3166.
        ''' Can be used to create Locales.
        ''' <p>
        ''' <b>Note:</b> The <code>Locale</code> class also supports other codes for
        ''' country (region), such as 3-letter numeric UN M.49 area codes.
        ''' Therefore, the list returned by this method does not contain ALL valid
        ''' codes that can be used to create Locales.
        ''' </summary>
        ''' <returns> An array of ISO 3166 two-letter country codes. </returns>
        Public Shared Property iSOCountries As String()
            Get
                If iSOCountries Is Nothing Then iSOCountries = getISO2Table(LocaleISOData.isoCountryTable)
                Dim result As String() = New String(iSOCountries.Length - 1) {}
                Array.Copy(iSOCountries, 0, result, 0, iSOCountries.Length)
                Return result
            End Get
        End Property

        ''' <summary>
        ''' Returns a list of all 2-letter language codes defined in ISO 639.
        ''' Can be used to create Locales.
        ''' <p>
        ''' <b>Note:</b>
        ''' <ul>
        ''' <li>ISO 639 is not a stable standard&mdash; some languages' codes have changed.
        ''' The list this function returns includes both the new and the old codes for the
        ''' languages whose codes have changed.
        ''' <li>The <code>Locale</code> class also supports language codes up to
        ''' 8 characters in length.  Therefore, the list returned by this method does
        ''' not contain ALL valid codes that can be used to create Locales.
        ''' </ul>
        ''' </summary>
        ''' <returns> Am array of ISO 639 two-letter language codes. </returns>
        Public Shared Property iSOLanguages As String()
            Get
                If iSOLanguages Is Nothing Then iSOLanguages = getISO2Table(LocaleISOData.isoLanguageTable)
                Dim result As String() = New String(iSOLanguages.Length - 1) {}
                Array.Copy(iSOLanguages, 0, result, 0, iSOLanguages.Length)
                Return result
            End Get
        End Property

        Private Shared Function getISO2Table(  table As String) As String()
			Dim len As Integer = table.length() \ 5
			Dim isoTable As String() = New String(len - 1){}
			Dim i As Integer = 0
			Dim j As Integer = 0
			Do While i < len
				isoTable(i) = table.Substring(j, 2)
				i += 1
				j += 5
			Loop
			Return isoTable
		End Function

		''' <summary>
		''' Returns the language code of this Locale.
		''' 
		''' <p><b>Note:</b> ISO 639 is not a stable standard&mdash; some languages' codes have changed.
		''' Locale's constructor recognizes both the new and the old codes for the languages
		''' whose codes have changed, but this function always returns the old code.  If you
		''' want to check for a specific language whose code has changed, don't do
		''' <pre>
		''' if (locale.getLanguage().equals("he")) // BAD!
		'''    ...
		''' </pre>
		''' Instead, do
		''' <pre>
		''' if (locale.getLanguage().equals(new Locale("he").getLanguage()))
		'''    ...
		''' </pre> </summary>
		''' <returns> The language code, or the empty string if none is defined. </returns>
		''' <seealso cref= #getDisplayLanguage </seealso>
		Public Property language As String
			Get
				Return baseLocale.language
			End Get
		End Property

		''' <summary>
		''' Returns the script for this locale, which should
		''' either be the empty string or an ISO 15924 4-letter script
		''' code. The first letter is uppercase and the rest are
		''' lowercase, for example, 'Latn', 'Cyrl'.
		''' </summary>
		''' <returns> The script code, or the empty string if none is defined. </returns>
		''' <seealso cref= #getDisplayScript
		''' @since 1.7 </seealso>
		Public Property script As String
			Get
				Return baseLocale.script
			End Get
		End Property

		''' <summary>
		''' Returns the country/region code for this locale, which should
		''' either be the empty string, an uppercase ISO 3166 2-letter code,
		''' or a UN M.49 3-digit code.
		''' </summary>
		''' <returns> The country/region code, or the empty string if none is defined. </returns>
		''' <seealso cref= #getDisplayCountry </seealso>
		Public Property country As String
			Get
				Return baseLocale.region
			End Get
		End Property

		''' <summary>
		''' Returns the variant code for this locale.
		''' </summary>
		''' <returns> The variant code, or the empty string if none is defined. </returns>
		''' <seealso cref= #getDisplayVariant </seealso>
		Public Property [variant] As String
			Get
				Return baseLocale.variant
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this {@code Locale} has any <a href="#def_extensions">
		''' extensions</a>.
		''' </summary>
		''' <returns> {@code true} if this {@code Locale} has any extensions
		''' @since 1.8 </returns>
		Public Function hasExtensions() As Boolean
			Return localeExtensions IsNot Nothing
		End Function

		''' <summary>
		''' Returns a copy of this {@code Locale} with no <a href="#def_extensions">
		''' extensions</a>. If this {@code Locale} has no extensions, this {@code Locale}
		''' is returned.
		''' </summary>
		''' <returns> a copy of this {@code Locale} with no extensions, or {@code this}
		'''         if {@code this} has no extensions
		''' @since 1.8 </returns>
		Public Function stripExtensions() As Locale
			Return If(hasExtensions(), Locale.getInstance(baseLocale, Nothing), Me)
		End Function

		''' <summary>
		''' Returns the extension (or private use) value associated with
		''' the specified key, or null if there is no extension
		''' associated with the key. To be well-formed, the key must be one
		''' of <code>[0-9A-Za-z]</code>. Keys are case-insensitive, so
		''' for example 'z' and 'Z' represent the same extension.
		''' </summary>
		''' <param name="key"> the extension key </param>
		''' <returns> The extension, or null if this locale defines no
		''' extension for the specified key. </returns>
		''' <exception cref="IllegalArgumentException"> if key is not well-formed </exception>
		''' <seealso cref= #PRIVATE_USE_EXTENSION </seealso>
		''' <seealso cref= #UNICODE_LOCALE_EXTENSION
		''' @since 1.7 </seealso>
		Public Function getExtension(  key As Char) As String
			If Not sun.util.locale.LocaleExtensions.isValidKey(key) Then Throw New IllegalArgumentException("Ill-formed extension key: " & AscW(key))
			Return If(hasExtensions(), localeExtensions.getExtensionValue(key), Nothing)
		End Function

        ''' <summary>
        ''' Returns the set of extension keys associated with this locale, or the
        ''' empty set if it has no extensions. The returned set is unmodifiable.
        ''' The keys will all be lower-case.
        ''' </summary>
        ''' <returns> The set of extension keys, or the empty set if this locale has
        ''' no extensions.
        ''' @since 1.7 </returns>
        Public ReadOnly Property extensionKeys As [Set](Of Character)
            Get
                If Not hasExtensions() Then Return Collections.emptySet()
                Return localeExtensions.keys
            End Get
        End Property

        ''' <summary>
        ''' Returns the set of unicode locale attributes associated with
        ''' this locale, or the empty set if it has no attributes. The
        ''' returned set is unmodifiable.
        ''' </summary>
        ''' <returns> The set of attributes.
        ''' @since 1.7 </returns>
        Public ReadOnly Property unicodeLocaleAttributes As [Set](Of String)
            Get
                If Not hasExtensions() Then Return Collections.emptySet()
                Return localeExtensions.unicodeLocaleAttributes
            End Get
        End Property

        ''' <summary>
        ''' Returns the Unicode locale type associated with the specified Unicode locale key
        ''' for this locale. Returns the empty string for keys that are defined with no type.
        ''' Returns null if the key is not defined. Keys are case-insensitive. The key must
        ''' be two alphanumeric characters ([0-9a-zA-Z]), or an IllegalArgumentException is
        ''' thrown.
        ''' </summary>
        ''' <param name="key"> the Unicode locale key </param>
        ''' <returns> The Unicode locale type associated with the key, or null if the
        ''' locale does not define the key. </returns>
        ''' <exception cref="IllegalArgumentException"> if the key is not well-formed </exception>
        ''' <exception cref="NullPointerException"> if <code>key</code> is null
        ''' @since 1.7 </exception>
        Public Function getUnicodeLocaleType(  key As String) As String
			If Not isUnicodeExtensionKey(key) Then Throw New IllegalArgumentException("Ill-formed Unicode locale key: " & key)
			Return If(hasExtensions(), localeExtensions.getUnicodeLocaleType(key), Nothing)
		End Function

        ''' <summary>
        ''' Returns the set of Unicode locale keys defined by this locale, or the empty set if
        ''' this locale has none.  The returned set is immutable.  Keys are all lower case.
        ''' </summary>
        ''' <returns> The set of Unicode locale keys, or the empty set if this locale has
        ''' no Unicode locale keywords.
        ''' @since 1.7 </returns>
        Public ReadOnly Property unicodeLocaleKeys As [Set](Of String)
            Get
                If localeExtensions Is Nothing Then Return Collections.emptySet()
                Return localeExtensions.unicodeLocaleKeys
            End Get
        End Property

        ''' <summary>
        ''' Package locale method returning the Locale's BaseLocale,
        ''' used by ResourceBundle </summary>
        ''' <returns> base locale of this Locale </returns>
        Friend ReadOnly Property baseLocale As sun.util.locale.BaseLocale
            Get
                Return baseLocale
            End Get
        End Property

        ''' <summary>
        ''' Package private method returning the Locale's LocaleExtensions,
        ''' used by ResourceBundle. </summary>
        ''' <returns> locale exnteions of this Locale,
        '''         or {@code null} if no extensions are defined </returns>
        Friend Property localeExtensions As sun.util.locale.LocaleExtensions
			 Get
				 Return localeExtensions
			 End Get
		 End Property

        ''' <summary>
        ''' Returns a string representation of this <code>Locale</code>
        ''' object, consisting of language, country, variant, script,
        ''' and extensions as below:
        ''' <blockquote>
        ''' language + "_" + country + "_" + (variant + "_#" | "#") + script + "-" + extensions
        ''' </blockquote>
        ''' 
        ''' Language is always lower case, country is always upper case, script is always title
        ''' case, and extensions are always lower case.  Extensions and private use subtags
        ''' will be in canonical order as explained in <seealso cref="#toLanguageTag"/>.
        ''' 
        ''' <p>When the locale has neither script nor extensions, the result is the same as in
        ''' Java 6 and prior.
        ''' 
        ''' <p>If both the language and country fields are missing, this function will return
        ''' the empty string, even if the variant, script, or extensions field is present (you
        ''' can't have a locale with just a variant, the variant must accompany a well-formed
        ''' language or country code).
        ''' 
        ''' <p>If script or extensions are present and variant is missing, no underscore is
        ''' added before the "#".
        ''' 
        ''' <p>This behavior is designed to support debugging and to be compatible with
        ''' previous uses of <code>toString</code> that expected language, country, and variant
        ''' fields only.  To represent a Locale as a String for interchange purposes, use
        ''' <seealso cref="#toLanguageTag"/>.
        ''' 
        ''' <p>Examples: <ul>
        ''' <li><tt>en</tt></li>
        ''' <li><tt>de_DE</tt></li>
        ''' <li><tt>_GB</tt></li>
        ''' <li><tt>en_US_WIN</tt></li>
        ''' <li><tt>de__POSIX</tt></li>
        ''' <li><tt>zh_CN_#Hans</tt></li>
        ''' <li><tt>zh_TW_#Hant-x-java</tt></li>
        ''' <li><tt>th_TH_TH_#u-nu-thai</tt></li></ul>
        ''' </summary>
        ''' <returns> A string representation of the Locale, for debugging. </returns>
        ''' <seealso cref= #getDisplayName </seealso>
        ''' <seealso cref= #toLanguageTag </seealso>
        Public Overrides Function ToString() As String
            Dim l As Boolean = (baseLocale.language.length() <> 0)
            Dim s As Boolean = (baseLocale.script.length() <> 0)
            Dim r As Boolean = (baseLocale.region.length() <> 0)
            Dim v As Boolean = (baseLocale.variant.length() <> 0)
            Dim e As Boolean = (localeExtensions IsNot Nothing AndAlso localeExtensions.iD.length() <> 0)

            Dim result As New StringBuilder(baseLocale.language)
            If r OrElse (l AndAlso (v OrElse s OrElse e)) Then result.append("_"c).append(baseLocale.region) ' This may just append '_'
            If v AndAlso (l OrElse r) Then result.append("_"c).append(baseLocale.variant)

            If s AndAlso (l OrElse r) Then result.append("_#").append(baseLocale.script)

            If e AndAlso (l OrElse r) Then
                result.append("_"c)
                If Not s Then result.append("#"c)
                result.append(localeExtensions.iD)
            End If

            Return result.ToString()
        End Function

        ''' <summary>
        ''' Returns a well-formed IETF BCP 47 language tag representing
        ''' this locale.
        ''' 
        ''' <p>If this <code>Locale</code> has a language, country, or
        ''' variant that does not satisfy the IETF BCP 47 language tag
        ''' syntax requirements, this method handles these fields as
        ''' described below:
        ''' 
        ''' <p><b>Language:</b> If language is empty, or not <a
        ''' href="#def_language" >well-formed</a> (for example "a" or
        ''' "e2"), it will be emitted as "und" (Undetermined).
        ''' 
        ''' <p><b>Country:</b> If country is not <a
        ''' href="#def_region">well-formed</a> (for example "12" or "USA"),
        ''' it will be omitted.
        ''' 
        ''' <p><b>Variant:</b> If variant <b>is</b> <a
        ''' href="#def_variant">well-formed</a>, each sub-segment
        ''' (delimited by '-' or '_') is emitted as a subtag.  Otherwise:
        ''' <ul>
        ''' 
        ''' <li>if all sub-segments match <code>[0-9a-zA-Z]{1,8}</code>
        ''' (for example "WIN" or "Oracle_JDK_Standard_Edition"), the first
        ''' ill-formed sub-segment and all following will be appended to
        ''' the private use subtag.  The first appended subtag will be
        ''' "lvariant", followed by the sub-segments in order, separated by
        ''' hyphen. For example, "x-lvariant-WIN",
        ''' "Oracle-x-lvariant-JDK-Standard-Edition".
        ''' 
        ''' <li>if any sub-segment does not match
        ''' <code>[0-9a-zA-Z]{1,8}</code>, the variant will be truncated
        ''' and the problematic sub-segment and all following sub-segments
        ''' will be omitted.  If the remainder is non-empty, it will be
        ''' emitted as a private use subtag as above (even if the remainder
        ''' turns out to be well-formed).  For example,
        ''' "Solaris_isjustthecoolestthing" is emitted as
        ''' "x-lvariant-Solaris", not as "solaris".</li></ul>
        ''' 
        ''' <p><b>Special Conversions:</b> Java supports some old locale
        ''' representations, including deprecated ISO language codes,
        ''' for compatibility. This method performs the following
        ''' conversions:
        ''' <ul>
        ''' 
        ''' <li>Deprecated ISO language codes "iw", "ji", and "in" are
        ''' converted to "he", "yi", and "id", respectively.
        ''' 
        ''' <li>A locale with language "no", country "NO", and variant
        ''' "NY", representing Norwegian Nynorsk (Norway), is converted
        ''' to a language tag "nn-NO".</li></ul>
        ''' 
        ''' <p><b>Note:</b> Although the language tag created by this
        ''' method is well-formed (satisfies the syntax requirements
        ''' defined by the IETF BCP 47 specification), it is not
        ''' necessarily a valid BCP 47 language tag.  For example,
        ''' <pre>
        '''   new Locale("xx", "YY").toLanguageTag();</pre>
        ''' 
        ''' will return "xx-YY", but the language subtag "xx" and the
        ''' region subtag "YY" are invalid because they are not registered
        ''' in the IANA Language Subtag Registry.
        ''' </summary>
        ''' <returns> a BCP47 language tag representing the locale </returns>
        ''' <seealso cref= #forLanguageTag(String)
        ''' @since 1.7 </seealso>
        Public Function toLanguageTag() As String
			If languageTag IsNot Nothing Then Return languageTag

			Dim tag As sun.util.locale.LanguageTag = sun.util.locale.LanguageTag.parseLocale(baseLocale, localeExtensions)
			Dim buf As New StringBuilder

			Dim subtag As String = tag.language
			If subtag.length() > 0 Then buf.append(sun.util.locale.LanguageTag.canonicalizeLanguage(subtag))

			subtag = tag.script
			If subtag.length() > 0 Then
				buf.append(sun.util.locale.LanguageTag.SEP)
				buf.append(sun.util.locale.LanguageTag.canonicalizeScript(subtag))
			End If

			subtag = tag.region
			If subtag.length() > 0 Then
				buf.append(sun.util.locale.LanguageTag.SEP)
				buf.append(sun.util.locale.LanguageTag.canonicalizeRegion(subtag))
			End If

			List(Of String)subtags = tag.variants
			For Each s As String In subtags
				buf.append(sun.util.locale.LanguageTag.SEP)
				' preserve casing
				buf.append(s)
			Next s

			subtags = tag.extensions
			For Each s As String In subtags
				buf.append(sun.util.locale.LanguageTag.SEP)
				buf.append(sun.util.locale.LanguageTag.canonicalizeExtension(s))
			Next s

			subtag = tag.privateuse
			If subtag.length() > 0 Then
				If buf.length() > 0 Then buf.append(sun.util.locale.LanguageTag.SEP)
				buf.append(sun.util.locale.LanguageTag.PRIVATEUSE).append(sun.util.locale.LanguageTag.SEP)
				' preserve casing
				buf.append(subtag)
			End If

			Dim langTag As String = buf.ToString()
			SyncLock Me
				If languageTag Is Nothing Then languageTag = langTag
			End SyncLock
			Return languageTag
		End Function

		''' <summary>
		''' Returns a locale for the specified IETF BCP 47 language tag string.
		''' 
		''' <p>If the specified language tag contains any ill-formed subtags,
		''' the first such subtag and all following subtags are ignored.  Compare
		''' to <seealso cref="Locale.Builder#setLanguageTag"/> which throws an exception
		''' in this case.
		''' 
		''' <p>The following <b>conversions</b> are performed:<ul>
		''' 
		''' <li>The language code "und" is mapped to language "".
		''' 
		''' <li>The language codes "he", "yi", and "id" are mapped to "iw",
		''' "ji", and "in" respectively. (This is the same canonicalization
		''' that's done in Locale's constructors.)
		''' 
		''' <li>The portion of a private use subtag prefixed by "lvariant",
		''' if any, is removed and appended to the variant field in the
		''' result locale (without case normalization).  If it is then
		''' empty, the private use subtag is discarded:
		''' 
		''' <pre>
		'''     Locale loc;
		'''     loc = Locale.forLanguageTag("en-US-x-lvariant-POSIX");
		'''     loc.getVariant(); // returns "POSIX"
		'''     loc.getExtension('x'); // returns null
		''' 
		'''     loc = Locale.forLanguageTag("de-POSIX-x-URP-lvariant-Abc-Def");
		'''     loc.getVariant(); // returns "POSIX_Abc_Def"
		'''     loc.getExtension('x'); // returns "urp"
		''' </pre>
		''' 
		''' <li>When the languageTag argument contains an extlang subtag,
		''' the first such subtag is used as the language, and the primary
		''' language subtag and other extlang subtags are ignored:
		''' 
		''' <pre>
		'''     Locale.forLanguageTag("ar-aao").getLanguage(); // returns "aao"
		'''     Locale.forLanguageTag("en-abc-def-us").toString(); // returns "abc_US"
		''' </pre>
		''' 
		''' <li>Case is normalized except for variant tags, which are left
		''' unchanged.  Language is normalized to lower case, script to
		''' title case, country to upper case, and extensions to lower
		''' case.
		''' 
		''' <li>If, after processing, the locale would exactly match either
		''' ja_JP_JP or th_TH_TH with no extensions, the appropriate
		''' extensions are added as though the constructor had been called:
		''' 
		''' <pre>
		'''    Locale.forLanguageTag("ja-JP-x-lvariant-JP").toLanguageTag();
		'''    // returns "ja-JP-u-ca-japanese-x-lvariant-JP"
		'''    Locale.forLanguageTag("th-TH-x-lvariant-TH").toLanguageTag();
		'''    // returns "th-TH-u-nu-thai-x-lvariant-TH"
		''' </pre></ul>
		''' 
		''' <p>This implements the 'Language-Tag' production of BCP47, and
		''' so supports grandfathered (regular and irregular) as well as
		''' private use language tags.  Stand alone private use tags are
		''' represented as empty language and extension 'x-whatever',
		''' and grandfathered tags are converted to their canonical replacements
		''' where they exist.
		''' 
		''' <p>Grandfathered tags with canonical replacements are as follows:
		''' 
		''' <table summary="Grandfathered tags with canonical replacements">
		''' <tbody align="center">
		''' <tr><th>grandfathered tag</th><th>&nbsp;</th><th>modern replacement</th></tr>
		''' <tr><td>art-lojban</td><td>&nbsp;</td><td>jbo</td></tr>
		''' <tr><td>i-ami</td><td>&nbsp;</td><td>ami</td></tr>
		''' <tr><td>i-bnn</td><td>&nbsp;</td><td>bnn</td></tr>
		''' <tr><td>i-hak</td><td>&nbsp;</td><td>hak</td></tr>
		''' <tr><td>i-klingon</td><td>&nbsp;</td><td>tlh</td></tr>
		''' <tr><td>i-lux</td><td>&nbsp;</td><td>lb</td></tr>
		''' <tr><td>i-navajo</td><td>&nbsp;</td><td>nv</td></tr>
		''' <tr><td>i-pwn</td><td>&nbsp;</td><td>pwn</td></tr>
		''' <tr><td>i-tao</td><td>&nbsp;</td><td>tao</td></tr>
		''' <tr><td>i-tay</td><td>&nbsp;</td><td>tay</td></tr>
		''' <tr><td>i-tsu</td><td>&nbsp;</td><td>tsu</td></tr>
		''' <tr><td>no-bok</td><td>&nbsp;</td><td>nb</td></tr>
		''' <tr><td>no-nyn</td><td>&nbsp;</td><td>nn</td></tr>
		''' <tr><td>sgn-BE-FR</td><td>&nbsp;</td><td>sfb</td></tr>
		''' <tr><td>sgn-BE-NL</td><td>&nbsp;</td><td>vgt</td></tr>
		''' <tr><td>sgn-CH-DE</td><td>&nbsp;</td><td>sgg</td></tr>
		''' <tr><td>zh-guoyu</td><td>&nbsp;</td><td>cmn</td></tr>
		''' <tr><td>zh-hakka</td><td>&nbsp;</td><td>hak</td></tr>
		''' <tr><td>zh-min-nan</td><td>&nbsp;</td><td>nan</td></tr>
		''' <tr><td>zh-xiang</td><td>&nbsp;</td><td>hsn</td></tr>
		''' </tbody>
		''' </table>
		''' 
		''' <p>Grandfathered tags with no modern replacement will be
		''' converted as follows:
		''' 
		''' <table summary="Grandfathered tags with no modern replacement">
		''' <tbody align="center">
		''' <tr><th>grandfathered tag</th><th>&nbsp;</th><th>converts to</th></tr>
		''' <tr><td>cel-gaulish</td><td>&nbsp;</td><td>xtg-x-cel-gaulish</td></tr>
		''' <tr><td>en-GB-oed</td><td>&nbsp;</td><td>en-GB-x-oed</td></tr>
		''' <tr><td>i-default</td><td>&nbsp;</td><td>en-x-i-default</td></tr>
		''' <tr><td>i-enochian</td><td>&nbsp;</td><td>und-x-i-enochian</td></tr>
		''' <tr><td>i-mingo</td><td>&nbsp;</td><td>see-x-i-mingo</td></tr>
		''' <tr><td>zh-min</td><td>&nbsp;</td><td>nan-x-zh-min</td></tr>
		''' </tbody>
		''' </table>
		''' 
		''' <p>For a list of all grandfathered tags, see the
		''' IANA Language Subtag Registry (search for "Type: grandfathered").
		''' 
		''' <p><b>Note</b>: there is no guarantee that <code>toLanguageTag</code>
		''' and <code>forLanguageTag</code> will round-trip.
		''' </summary>
		''' <param name="languageTag"> the language tag </param>
		''' <returns> The locale that best represents the language tag. </returns>
		''' <exception cref="NullPointerException"> if <code>languageTag</code> is <code>null</code> </exception>
		''' <seealso cref= #toLanguageTag() </seealso>
		''' <seealso cref= java.util.Locale.Builder#setLanguageTag(String)
		''' @since 1.7 </seealso>
		Public Shared Function forLanguageTag(  languageTag As String) As Locale
			Dim tag As sun.util.locale.LanguageTag = sun.util.locale.LanguageTag.parse(languageTag, Nothing)
			Dim bldr As New sun.util.locale.InternalLocaleBuilder
			bldr.languageTag = tag
			Dim base As sun.util.locale.BaseLocale = bldr.baseLocale
			Dim exts As sun.util.locale.LocaleExtensions = bldr.localeExtensions
			If exts Is Nothing AndAlso base.variant.length() > 0 Then exts = getCompatibilityExtensions(base.language, base.script, base.region, base.variant)
			Return getInstance(base, exts)
		End Function

        ''' <summary>
        ''' Returns a three-letter abbreviation of this locale's language.
        ''' If the language matches an ISO 639-1 two-letter code, the
        ''' corresponding ISO 639-2/T three-letter lowercase code is
        ''' returned.  The ISO 639-2 language codes can be found on-line,
        ''' see "Codes for the Representation of Names of Languages Part 2:
        ''' Alpha-3 Code".  If the locale specifies a three-letter
        ''' language, the language is returned as is.  If the locale does
        ''' not specify a language the empty string is returned.
        ''' </summary>
        ''' <returns> A three-letter abbreviation of this locale's language. </returns>
        ''' <exception cref="MissingResourceException"> Throws MissingResourceException if
        ''' three-letter language abbreviation is not available for this locale. </exception>
        Public ReadOnly Property iSO3Language As String
            Get
                Dim lang As String = baseLocale.language
                If lang.Length() = 3 Then Return lang

                Dim language3 As String = getISO3Code(lang, LocaleISOData.isoLanguageTable)
                If language3 Is Nothing Then Throw New MissingResourceException("Couldn't find 3-letter language code for " & lang, "FormatData_" & ToString(), "ShortLanguage")
                Return language3
            End Get
        End Property

        ''' <summary>
        ''' Returns a three-letter abbreviation for this locale's country.
        ''' If the country matches an ISO 3166-1 alpha-2 code, the
        ''' corresponding ISO 3166-1 alpha-3 uppercase code is returned.
        ''' If the locale doesn't specify a country, this will be the empty
        ''' string.
        ''' 
        ''' <p>The ISO 3166-1 codes can be found on-line.
        ''' </summary>
        ''' <returns> A three-letter abbreviation of this locale's country. </returns>
        ''' <exception cref="MissingResourceException"> Throws MissingResourceException if the
        ''' three-letter country abbreviation is not available for this locale. </exception>
        Public ReadOnly Property iSO3Country As String
            Get
                Dim country3 As String = getISO3Code(baseLocale.region, LocaleISOData.isoCountryTable)
                If country3 Is Nothing Then Throw New MissingResourceException("Couldn't find 3-letter country code for " & baseLocale.region, "FormatData_" & ToString(), "ShortCountry")
                Return country3
            End Get
        End Property

        Private Shared Function getISO3Code(  iso2Code As String,   table As String) As String
			Dim codeLength As Integer = iso2Code.length()
			If codeLength = 0 Then Return ""

			Dim tableLength As Integer = table.length()
			Dim index As Integer = tableLength
			If codeLength = 2 Then
				Dim c1 As Char = iso2Code.Chars(0)
				Dim c2 As Char = iso2Code.Chars(1)
				For index = 0 To tableLength - 1 Step 5
					If table.Chars(index) = c1 AndAlso table.Chars(index + 1) = c2 Then Exit For
				Next index
			End If
			Return If(index < tableLength, table.Substring(index + 2, index + 5 - (index + 2)), Nothing)
		End Function

        ''' <summary>
        ''' Returns a name for the locale's language that is appropriate for display to the
        ''' user.
        ''' If possible, the name returned will be localized for the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
        ''' For example, if the locale is fr_FR and the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
        ''' is en_US, getDisplayLanguage() will return "French"; if the locale is en_US and
        ''' the default <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale is fr_FR,
        ''' getDisplayLanguage() will return "anglais".
        ''' If the name returned cannot be localized for the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale,
        ''' (say, we don't have a Japanese name for Croatian),
        ''' this function falls back on the English name, and uses the ISO code as a last-resort
        ''' value.  If the locale doesn't specify a language, this function returns the empty string.
        ''' </summary>
        ''' <returns> The name of the display language. </returns>
        Public ReadOnly Property displayLanguage As String
            Get
                Return getDisplayLanguage(getDefault(Category.DISPLAY))
            End Get
        End Property

        ''' <summary>
        ''' Returns a name for the locale's language that is appropriate for display to the
        ''' user.
        ''' If possible, the name returned will be localized according to inLocale.
        ''' For example, if the locale is fr_FR and inLocale
        ''' is en_US, getDisplayLanguage() will return "French"; if the locale is en_US and
        ''' inLocale is fr_FR, getDisplayLanguage() will return "anglais".
        ''' If the name returned cannot be localized according to inLocale,
        ''' (say, we don't have a Japanese name for Croatian),
        ''' this function falls back on the English name, and finally
        ''' on the ISO code as a last-resort value.  If the locale doesn't specify a language,
        ''' this function returns the empty string.
        ''' </summary>
        ''' <param name="inLocale"> The locale for which to retrieve the display language. </param>
        ''' <returns> The name of the display language appropriate to the given locale. </returns>
        ''' <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
        Public Function getDisplayLanguage(  inLocale As Locale) As String
			Return getDisplayString(baseLocale.language, inLocale, DISPLAY_LANGUAGE)
		End Function

        ''' <summary>
        ''' Returns a name for the the locale's script that is appropriate for display to
        ''' the user. If possible, the name will be localized for the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.  Returns
        ''' the empty string if this locale doesn't specify a script code.
        ''' </summary>
        ''' <returns> the display name of the script code for the current default
        '''     <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
        ''' @since 1.7 </returns>
        Public ReadOnly Property displayScript As String
            Get
                Return getDisplayScript(getDefault(Category.DISPLAY))
            End Get
        End Property

        ''' <summary>
        ''' Returns a name for the locale's script that is appropriate
        ''' for display to the user. If possible, the name will be
        ''' localized for the given locale. Returns the empty string if
        ''' this locale doesn't specify a script code.
        ''' </summary>
        ''' <param name="inLocale"> The locale for which to retrieve the display script. </param>
        ''' <returns> the display name of the script code for the current default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale </returns>
        ''' <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code>
        ''' @since 1.7 </exception>
        Public Function getDisplayScript(  inLocale As Locale) As String
			Return getDisplayString(baseLocale.script, inLocale, DISPLAY_SCRIPT)
		End Function

        ''' <summary>
        ''' Returns a name for the locale's country that is appropriate for display to the
        ''' user.
        ''' If possible, the name returned will be localized for the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
        ''' For example, if the locale is fr_FR and the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
        ''' is en_US, getDisplayCountry() will return "France"; if the locale is en_US and
        ''' the default <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale is fr_FR,
        ''' getDisplayCountry() will return "Etats-Unis".
        ''' If the name returned cannot be localized for the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale,
        ''' (say, we don't have a Japanese name for Croatia),
        ''' this function falls back on the English name, and uses the ISO code as a last-resort
        ''' value.  If the locale doesn't specify a country, this function returns the empty string.
        ''' </summary>
        ''' <returns> The name of the country appropriate to the locale. </returns>
        Public ReadOnly Property displayCountry As String
            Get
                Return getDisplayCountry(getDefault(Category.DISPLAY))
            End Get
        End Property

        ''' <summary>
        ''' Returns a name for the locale's country that is appropriate for display to the
        ''' user.
        ''' If possible, the name returned will be localized according to inLocale.
        ''' For example, if the locale is fr_FR and inLocale
        ''' is en_US, getDisplayCountry() will return "France"; if the locale is en_US and
        ''' inLocale is fr_FR, getDisplayCountry() will return "Etats-Unis".
        ''' If the name returned cannot be localized according to inLocale.
        ''' (say, we don't have a Japanese name for Croatia),
        ''' this function falls back on the English name, and finally
        ''' on the ISO code as a last-resort value.  If the locale doesn't specify a country,
        ''' this function returns the empty string.
        ''' </summary>
        ''' <param name="inLocale"> The locale for which to retrieve the display country. </param>
        ''' <returns> The name of the country appropriate to the given locale. </returns>
        ''' <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
        Public Function getDisplayCountry(  inLocale As Locale) As String
			Return getDisplayString(baseLocale.region, inLocale, DISPLAY_COUNTRY)
		End Function

		Private Function getDisplayString(  code As String,   inLocale As Locale,   type As Integer) As String
			If code.length() = 0 Then Return ""

			If inLocale Is Nothing Then Throw New NullPointerException

			Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.util.spi.LocaleNameProvider))
			Dim key As String = (If(type = DISPLAY_VARIANT, "%%" & code, code))
			Dim result As String = pool.getLocalizedObject(LocaleNameGetter.INSTANCE, inLocale, key, type, code)
				If result IsNot Nothing Then Return result

			Return code
		End Function

        ''' <summary>
        ''' Returns a name for the locale's variant code that is appropriate for display to the
        ''' user.  If possible, the name will be localized for the default
        ''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.  If the locale
        ''' doesn't specify a variant code, this function returns the empty string.
        ''' </summary>
        ''' <returns> The name of the display variant code appropriate to the locale. </returns>
        Public ReadOnly Property displayVariant As String
            Get
                Return getDisplayVariant(getDefault(Category.DISPLAY))
            End Get
        End Property

        ''' <summary>
        ''' Returns a name for the locale's variant code that is appropriate for display to the
        ''' user.  If possible, the name will be localized for inLocale.  If the locale
        ''' doesn't specify a variant code, this function returns the empty string.
        ''' </summary>
        ''' <param name="inLocale"> The locale for which to retrieve the display variant code. </param>
        ''' <returns> The name of the display variant code appropriate to the given locale. </returns>
        ''' <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
        Public Function getDisplayVariant(  inLocale As Locale) As String
			If baseLocale.variant.length() = 0 Then Return ""

			Dim lr As sun.util.locale.provider.LocaleResources = sun.util.locale.provider.LocaleProviderAdapter.forJRE().getLocaleResources(inLocale)

			Dim names As String() = getDisplayVariantArray(inLocale)

			' Get the localized patterns for formatting a list, and use
			' them to format the list.
			Return formatList(names, lr.getLocaleName("ListPattern"), lr.getLocaleName("ListCompositionPattern"))
		End Function

        ''' <summary>
        ''' Returns a name for the locale that is appropriate for display to the
        ''' user. This will be the values returned by getDisplayLanguage(),
        ''' getDisplayScript(), getDisplayCountry(), and getDisplayVariant() assembled
        ''' into a single string. The the non-empty values are used in order,
        ''' with the second and subsequent names in parentheses.  For example:
        ''' <blockquote>
        ''' language (script, country, variant)<br>
        ''' language (country)<br>
        ''' language (variant)<br>
        ''' script (country)<br>
        ''' country<br>
        ''' </blockquote>
        ''' depending on which fields are specified in the locale.  If the
        ''' language, script, country, and variant fields are all empty,
        ''' this function returns the empty string.
        ''' </summary>
        ''' <returns> The name of the locale appropriate to display. </returns>
        Public ReadOnly Property displayName As String
            Get
                Return getDisplayName(getDefault(Category.DISPLAY))
            End Get
        End Property

        ''' <summary>
        ''' Returns a name for the locale that is appropriate for display
        ''' to the user.  This will be the values returned by
        ''' getDisplayLanguage(), getDisplayScript(),getDisplayCountry(),
        ''' and getDisplayVariant() assembled into a single string.
        ''' The non-empty values are used in order,
        ''' with the second and subsequent names in parentheses.  For example:
        ''' <blockquote>
        ''' language (script, country, variant)<br>
        ''' language (country)<br>
        ''' language (variant)<br>
        ''' script (country)<br>
        ''' country<br>
        ''' </blockquote>
        ''' depending on which fields are specified in the locale.  If the
        ''' language, script, country, and variant fields are all empty,
        ''' this function returns the empty string.
        ''' </summary>
        ''' <param name="inLocale"> The locale for which to retrieve the display name. </param>
        ''' <returns> The name of the locale appropriate to display. </returns>
        ''' <exception cref="NullPointerException"> if <code>inLocale</code> is <code>null</code> </exception>
        Public Function getDisplayName(  inLocale As Locale) As String
			Dim lr As sun.util.locale.provider.LocaleResources = sun.util.locale.provider.LocaleProviderAdapter.forJRE().getLocaleResources(inLocale)

			Dim languageName As String = getDisplayLanguage(inLocale)
			Dim scriptName As String = getDisplayScript(inLocale)
			Dim countryName As String = getDisplayCountry(inLocale)
			Dim variantNames As String() = getDisplayVariantArray(inLocale)

			' Get the localized patterns for formatting a display name.
			Dim displayNamePattern As String = lr.getLocaleName("DisplayNamePattern")
			Dim listPattern As String = lr.getLocaleName("ListPattern")
			Dim listCompositionPattern As String = lr.getLocaleName("ListCompositionPattern")

			' The display name consists of a main name, followed by qualifiers.
			' Typically, the format is "MainName (Qualifier, Qualifier)" but this
			' depends on what pattern is stored in the display locale.
			Dim mainName As String = Nothing
			Dim qualifierNames As String() = Nothing

			' The main name is the language, or if there is no language, the script,
			' then if no script, the country. If there is no language/script/country
			' (an anomalous situation) then the display name is simply the variant's
			' display name.
			If languageName.length() = 0 AndAlso scriptName.length() = 0 AndAlso countryName.length() = 0 Then
				If variantNames.Length = 0 Then
					Return ""
				Else
					Return formatList(variantNames, listPattern, listCompositionPattern)
				End If
			End If
			Dim names As New List(Of String)(4)
			If languageName.length() <> 0 Then names.add(languageName)
			If scriptName.length() <> 0 Then names.add(scriptName)
			If countryName.length() <> 0 Then names.add(countryName)
			If variantNames.Length <> 0 Then names.addAll(variantNames)

			' The first one in the main name
			mainName = names.get(0)

			' Others are qualifiers
			Dim numNames As Integer = names.size()
			qualifierNames = If(numNames > 1, names.subList(1, numNames).ToArray(New String(numNames - 2){}), New String(){})

			' Create an array whose first element is the number of remaining
			' elements.  This serves as a selector into a ChoiceFormat pattern from
			' the resource.  The second and third elements are the main name and
			' the qualifier; if there are no qualifiers, the third element is
			' unused by the format pattern.
			Dim displayNames As Object() = { New Integer?(If(qualifierNames.Length <> 0, 2, 1)), mainName,If(qualifierNames.Length <> 0, formatList(qualifierNames, listPattern, listCompositionPattern), Nothing)}

			If displayNamePattern IsNot Nothing Then
				Return (New java.text.MessageFormat(displayNamePattern)).format(displayNames)
			Else
				' If we cannot get the message format pattern, then we use a simple
				' hard-coded pattern.  This should not occur in practice unless the
				' installation is missing some core files (FormatData etc.).
				Dim result As New StringBuilder
				result.append(CStr(displayNames(1)))
				If displayNames.Length > 2 Then
					result.append(" (")
					result.append(CStr(displayNames(2)))
					result.append(")"c)
				End If
				Return result.ToString()
			End If
		End Function

		''' <summary>
		''' Overrides Cloneable.
		''' </summary>
		Public Overrides Function clone() As Object
			Try
				Dim that As Locale = CType(MyBase.clone(), Locale)
				Return that
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Override hashCode.
		''' Since Locales are often used in hashtables, caches the value
		''' for speed.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim hc As Integer = hashCodeValue
			If hc = 0 Then
				hc = baseLocale.GetHashCode()
				If localeExtensions IsNot Nothing Then hc = hc Xor localeExtensions.GetHashCode()
				hashCodeValue = hc
			End If
			Return hc
		End Function

		' Overrides

		''' <summary>
		''' Returns true if this Locale is equal to another object.  A Locale is
		''' deemed equal to another Locale with identical language, script, country,
		''' variant and extensions, and unequal to all other objects.
		''' </summary>
		''' <returns> true if this Locale is equal to the specified object. </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then ' quick check Return True
			If Not(TypeOf obj Is Locale) Then Return False
			Dim otherBase As sun.util.locale.BaseLocale = CType(obj, Locale).baseLocale
			If Not baseLocale.Equals(otherBase) Then Return False
			If localeExtensions Is Nothing Then Return CType(obj, Locale).localeExtensions Is Nothing
			Return localeExtensions.Equals(CType(obj, Locale).localeExtensions)
		End Function

		' ================= privates =====================================

		<NonSerialized> _
		Private baseLocale As sun.util.locale.BaseLocale
		<NonSerialized> _
		Private localeExtensions As sun.util.locale.LocaleExtensions

		''' <summary>
		''' Calculated hashcode
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private hashCodeValue As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared defaultLocale As Locale = initDefault()
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared defaultDisplayLocale As Locale = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared defaultFormatLocale As Locale = Nothing

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private languageTag As String

		''' <summary>
		''' Return an array of the display names of the variant. </summary>
		''' <param name="bundle"> the ResourceBundle to use to get the display names </param>
		''' <returns> an array of display names, possible of zero length. </returns>
		Private Function getDisplayVariantArray(  inLocale As Locale) As String()
			' Split the variant name into tokens separated by '_'.
			Dim tokenizer As New StringTokenizer(baseLocale.variant, "_")
			Dim names As String() = New String(tokenizer.countTokens() - 1){}

			' For each variant token, lookup the display name.  If
			' not found, use the variant name itself.
			For i As Integer = 0 To names.Length - 1
				names(i) = getDisplayString(tokenizer.nextToken(), inLocale, DISPLAY_VARIANT)
			Next i

			Return names
		End Function

		''' <summary>
		''' Format a list using given pattern strings.
		''' If either of the patterns is null, then a the list is
		''' formatted by concatenation with the delimiter ','. </summary>
		''' <param name="stringList"> the list of strings to be formatted. </param>
		''' <param name="listPattern"> should create a MessageFormat taking 0-3 arguments
		''' and formatting them into a list. </param>
		''' <param name="listCompositionPattern"> should take 2 arguments
		''' and is used by composeList. </param>
		''' <returns> a string representing the list. </returns>
		Private Shared Function formatList(  stringList As String(),   listPattern As String,   listCompositionPattern As String) As String
			' If we have no list patterns, compose the list in a simple,
			' non-localized way.
			If listPattern Is Nothing OrElse listCompositionPattern Is Nothing Then
				Dim result As New StringBuilder
				For i As Integer = 0 To stringList.Length - 1
					If i > 0 Then result.append(","c)
					result.append(stringList(i))
				Next i
				Return result.ToString()
			End If

			' Compose the list down to three elements if necessary
			If stringList.Length > 3 Then
				Dim format As New java.text.MessageFormat(listCompositionPattern)
				stringList = composeList(format, stringList)
			End If

			' Rebuild the argument list with the list length as the first element
			Dim args As Object() = New Object(stringList.Length){}
			Array.Copy(stringList, 0, args, 1, stringList.Length)
			args(0) = New Integer?(stringList.Length)

			' Format it using the pattern in the resource
			Dim format As New java.text.MessageFormat(listPattern)
			Return format.format(args)
		End Function

		''' <summary>
		''' Given a list of strings, return a list shortened to three elements.
		''' Shorten it by applying the given format to the first two elements
		''' recursively. </summary>
		''' <param name="format"> a format which takes two arguments </param>
		''' <param name="list"> a list of strings </param>
		''' <returns> if the list is three elements or shorter, the same list;
		''' otherwise, a new list of three elements. </returns>
		Private Shared Function composeList(  format As java.text.MessageFormat,   list As String()) As String()
			If list.Length <= 3 Then Return list

			' Use the given format to compose the first two elements into one
			Dim listItems As String() = { list(0), list(1) }
			Dim newItem As String = format.format(listItems)

			' Form a new list one element shorter
			Dim newList As String() = New String(list.Length-2){}
			Array.Copy(list, 2, newList, 1, newList.Length-1)
			newList(0) = newItem

			' Recurse
			Return composeList(format, newList)
		End Function

		' Duplicate of sun.util.locale.UnicodeLocaleExtension.isKey in order to
		' avoid its class loading.
		Private Shared Function isUnicodeExtensionKey(  s As String) As Boolean
			' 2alphanum
			Return (s.length() = 2) AndAlso sun.util.locale.LocaleUtils.isAlphaNumericString(s)
		End Function

		''' <summary>
		''' @serialField language    String
		'''      language subtag in lower case. (See <a href="java/util/Locale.html#getLanguage()">getLanguage()</a>)
		''' @serialField country     String
		'''      country subtag in upper case. (See <a href="java/util/Locale.html#getCountry()">getCountry()</a>)
		''' @serialField variant     String
		'''      variant subtags separated by LOWLINE characters. (See <a href="java/util/Locale.html#getVariant()">getVariant()</a>)
		''' @serialField hashcode    int
		'''      deprecated, for forward compatibility only
		''' @serialField script      String
		'''      script subtag in title case (See <a href="java/util/Locale.html#getScript()">getScript()</a>)
		''' @serialField extensions  String
		'''      canonical representation of extensions, that is,
		'''      BCP47 extensions in alphabetical order followed by
		'''      BCP47 private use subtags, all in lower case letters
		'''      separated by HYPHEN-MINUS characters.
		'''      (See <a href="java/util/Locale.html#getExtensionKeys()">getExtensionKeys()</a>,
		'''      <a href="java/util/Locale.html#getExtension(char)">getExtension(char)</a>)
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("language", GetType(String)), New java.io.ObjectStreamField("country", GetType(String)), New java.io.ObjectStreamField("variant", GetType(String)), New java.io.ObjectStreamField("hashcode", GetType(Integer)), New java.io.ObjectStreamField("script", GetType(String)), New java.io.ObjectStreamField("extensions", GetType(String)) }

		''' <summary>
		''' Serializes this <code>Locale</code> to the specified <code>ObjectOutputStream</code>. </summary>
		''' <param name="out"> the <code>ObjectOutputStream</code> to write </param>
		''' <exception cref="IOException">
		''' @since 1.7 </exception>
		Private Sub writeObject(  out As java.io.ObjectOutputStream)
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("language", baseLocale.language)
			fields.put("script", baseLocale.script)
			fields.put("country", baseLocale.region)
			fields.put("variant", baseLocale.variant)
			fields.put("extensions",If(localeExtensions Is Nothing, "", localeExtensions.iD))
			fields.put("hashcode", -1) ' place holder just for backward support
			out.writeFields()
		End Sub

		''' <summary>
		''' Deserializes this <code>Locale</code>. </summary>
		''' <param name="in"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="IOException"> </exception>
		''' <exception cref="ClassNotFoundException"> </exception>
		''' <exception cref="IllformedLocaleException">
		''' @since 1.7 </exception>
		Private Sub readObject(  [in] As java.io.ObjectInputStream)
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			Dim language_Renamed As String = CStr(fields.get("language", ""))
			Dim script_Renamed As String = CStr(fields.get("script", ""))
			Dim country_Renamed As String = CStr(fields.get("country", ""))
			Dim variant_Renamed As String = CStr(fields.get("variant", ""))
			Dim extStr As String = CStr(fields.get("extensions", ""))
			baseLocale = sun.util.locale.BaseLocale.getInstance(convertOldISOCodes(language_Renamed), script_Renamed, country_Renamed, variant_Renamed)
			If extStr.length() > 0 Then
				Try
					Dim bldr As New sun.util.locale.InternalLocaleBuilder
					bldr.extensions = extStr
					localeExtensions = bldr.localeExtensions
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message)
				End Try
			Else
				localeExtensions = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns a cached <code>Locale</code> instance equivalent to
		''' the deserialized <code>Locale</code>. When serialized
		''' language, country and variant fields read from the object data stream
		''' are exactly "ja", "JP", "JP" or "th", "TH", "TH" and script/extensions
		''' fields are empty, this method supplies <code>UNICODE_LOCALE_EXTENSION</code>
		''' "ca"/"japanese" (calendar type is "japanese") or "nu"/"thai" (number script
		''' type is "thai"). See <a href="Locale.html#special_cases_constructor">Special Cases</a>
		''' for more information.
		''' </summary>
		''' <returns> an instance of <code>Locale</code> equivalent to
		''' the deserialized <code>Locale</code>. </returns>
		''' <exception cref="java.io.ObjectStreamException"> </exception>
		Private Function readResolve() As Object
			Return getInstance(baseLocale.language, baseLocale.script, baseLocale.region, baseLocale.variant, localeExtensions)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared isoLanguages As String() = Nothing

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared isoCountries As String() = Nothing

		Private Shared Function convertOldISOCodes(  language As String) As String
			' we accept both the old and the new ISO codes for the languages whose ISO
			' codes have changed, but we always store the OLD code, for backward compatibility
			language = sun.util.locale.LocaleUtils.toLowerString(language).intern()
			If language = "he" Then
				Return "iw"
			ElseIf language = "yi" Then
				Return "ji"
			ElseIf language = "id" Then
				Return "in"
			Else
				Return language
			End If
		End Function

		Private Shared Function getCompatibilityExtensions(  language As String,   script As String,   country As String,   [variant] As String) As sun.util.locale.LocaleExtensions
			Dim extensions As sun.util.locale.LocaleExtensions = Nothing
			' Special cases for backward compatibility support
			If sun.util.locale.LocaleUtils.caseIgnoreMatch(language, "ja") AndAlso script.length() = 0 AndAlso sun.util.locale.LocaleUtils.caseIgnoreMatch(country, "jp") AndAlso "JP".Equals([variant]) Then
				' ja_JP_JP -> u-ca-japanese (calendar = japanese)
				extensions = sun.util.locale.LocaleExtensions.CALENDAR_JAPANESE
			ElseIf sun.util.locale.LocaleUtils.caseIgnoreMatch(language, "th") AndAlso script.length() = 0 AndAlso sun.util.locale.LocaleUtils.caseIgnoreMatch(country, "th") AndAlso "TH".Equals([variant]) Then
				' th_TH_TH -> u-nu-thai (numbersystem = thai)
				extensions = sun.util.locale.LocaleExtensions.NUMBER_THAI
			End If
			Return extensions
		End Function

		''' <summary>
		''' Obtains a localized locale names from a LocaleNameProvider
		''' implementation.
		''' </summary>
		Private Class LocaleNameGetter
			Implements sun.util.locale.provider.LocaleServiceProviderPool.LocalizedObjectGetter(Of java.util.spi.LocaleNameProvider, String)

			Private Shared ReadOnly INSTANCE As New LocaleNameGetter

			Public Overrides Function getObject(  localeNameProvider As java.util.spi.LocaleNameProvider,   locale_Renamed As Locale,   key As String, ParamArray   params As Object()) As String
				Debug.Assert(params.Length = 2)
				Dim type As Integer = CInt(Fix(params(0)))
				Dim code As String = CStr(params(1))

				Select Case type
				Case DISPLAY_LANGUAGE
					Return localeNameProvider.getDisplayLanguage(code, locale_Renamed)
				Case DISPLAY_COUNTRY
					Return localeNameProvider.getDisplayCountry(code, locale_Renamed)
				Case DISPLAY_VARIANT
					Return localeNameProvider.getDisplayVariant(code, locale_Renamed)
				Case DISPLAY_SCRIPT
					Return localeNameProvider.getDisplayScript(code, locale_Renamed)
				Case Else
					Debug.Assert(False) ' shouldn't happen
				End Select

				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Enum for locale categories.  These locale categories are used to get/set
		''' the default locale for the specific functionality represented by the
		''' category.
		''' </summary>
		''' <seealso cref= #getDefault(Locale.Category) </seealso>
		''' <seealso cref= #setDefault(Locale.Category, Locale)
		''' @since 1.7 </seealso>
		Public Enum Category

			''' <summary>
			''' Category used to represent the default locale for
			''' displaying user interfaces.
			''' </summary>
			DISPLAY("user.language.display",
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'					"user.script.display",
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'					"user.country.display",
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'					"user.variant.display"),

			''' <summary>
			''' Category used to represent the default locale for
			''' formatting dates, numbers, and/or currencies.
			''' </summary>
			FORMAT("user.language.format",
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'				   "user.script.format",
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'				   "user.country.format",
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'				   "user.variant.format");

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			Category(String languageKey, String scriptKey, String countryKey, String variantKey)
	'		{
	'			Me.languageKey = languageKey;
	'			Me.scriptKey = scriptKey;
	'			Me.countryKey = countryKey;
	'			Me.variantKey = variantKey;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			final String languageKey;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			final String scriptKey;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			final String countryKey;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			final String variantKey;
		End Enum

		''' <summary>
		''' <code>Builder</code> is used to build instances of <code>Locale</code>
		''' from values configured by the setters.  Unlike the <code>Locale</code>
		''' constructors, the <code>Builder</code> checks if a value configured by a
		''' setter satisfies the syntax requirements defined by the <code>Locale</code>
		''' class.  A <code>Locale</code> object created by a <code>Builder</code> is
		''' well-formed and can be transformed to a well-formed IETF BCP 47 language tag
		''' without losing information.
		''' 
		''' <p><b>Note:</b> The <code>Locale</code> class does not provide any
		''' syntactic restrictions on variant, while BCP 47 requires each variant
		''' subtag to be 5 to 8 alphanumerics or a single numeric followed by 3
		''' alphanumerics.  The method <code>setVariant</code> throws
		''' <code>IllformedLocaleException</code> for a variant that does not satisfy
		''' this restriction. If it is necessary to support such a variant, use a
		''' Locale constructor.  However, keep in mind that a <code>Locale</code>
		''' object created this way might lose the variant information when
		''' transformed to a BCP 47 language tag.
		''' 
		''' <p>The following example shows how to create a <code>Locale</code> object
		''' with the <code>Builder</code>.
		''' <blockquote>
		''' <pre>
		'''     Locale aLocale = new Builder().setLanguage("sr").setScript("Latn").setRegion("RS").build();
		''' </pre>
		''' </blockquote>
		''' 
		''' <p>Builders can be reused; <code>clear()</code> resets all
		''' fields to their default values.
		''' </summary>
		''' <seealso cref= Locale#forLanguageTag
		''' @since 1.7 </seealso>
		Public NotInheritable Class Builder
			Private ReadOnly localeBuilder As sun.util.locale.InternalLocaleBuilder

			''' <summary>
			''' Constructs an empty Builder. The default value of all
			''' fields, extensions, and private use information is the
			''' empty string.
			''' </summary>
			Public Sub New()
				localeBuilder = New sun.util.locale.InternalLocaleBuilder
			End Sub

			''' <summary>
			''' Resets the <code>Builder</code> to match the provided
			''' <code>locale</code>.  Existing state is discarded.
			''' 
			''' <p>All fields of the locale must be well-formed, see <seealso cref="Locale"/>.
			''' 
			''' <p>Locales with any ill-formed fields cause
			''' <code>IllformedLocaleException</code> to be thrown, except for the
			''' following three cases which are accepted for compatibility
			''' reasons:<ul>
			''' <li>Locale("ja", "JP", "JP") is treated as "ja-JP-u-ca-japanese"
			''' <li>Locale("th", "TH", "TH") is treated as "th-TH-u-nu-thai"
			''' <li>Locale("no", "NO", "NY") is treated as "nn-NO"</ul>
			''' </summary>
			''' <param name="locale"> the locale </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>locale</code> has
			''' any ill-formed fields. </exception>
			''' <exception cref="NullPointerException"> if <code>locale</code> is null. </exception>
			Public Function setLocale(  locale_Renamed As Locale) As Builder
				Try
					localeBuilder.localeale(locale_Renamed.baseLocale, locale_Renamed.localeExtensions)
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Resets the Builder to match the provided IETF BCP 47
			''' language tag.  Discards the existing state.  Null and the
			''' empty string cause the builder to be reset, like {@link
			''' #clear}.  Grandfathered tags (see {@link
			''' Locale#forLanguageTag}) are converted to their canonical
			''' form before being processed.  Otherwise, the language tag
			''' must be well-formed (see <seealso cref="Locale"/>) or an exception is
			''' thrown (unlike <code>Locale.forLanguageTag</code>, which
			''' just discards ill-formed and following portions of the
			''' tag).
			''' </summary>
			''' <param name="languageTag"> the language tag </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>languageTag</code> is ill-formed </exception>
			''' <seealso cref= Locale#forLanguageTag(String) </seealso>
			Public Function setLanguageTag(  languageTag As String) As Builder
				Dim sts As New sun.util.locale.ParseStatus
				Dim tag As sun.util.locale.LanguageTag = sun.util.locale.LanguageTag.parse(languageTag, sts)
				If sts.error Then Throw New IllformedLocaleException(sts.errorMessage, sts.errorIndex)
				localeBuilder.languageTag = tag
				Return Me
			End Function

			''' <summary>
			''' Sets the language.  If <code>language</code> is the empty string or
			''' null, the language in this <code>Builder</code> is removed.  Otherwise,
			''' the language must be <a href="./Locale.html#def_language">well-formed</a>
			''' or an exception is thrown.
			''' 
			''' <p>The typical language value is a two or three-letter language
			''' code as defined in ISO639.
			''' </summary>
			''' <param name="language"> the language </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>language</code> is ill-formed </exception>
			Public Function setLanguage(  language As String) As Builder
				Try
					localeBuilder.language = language
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Sets the script. If <code>script</code> is null or the empty string,
			''' the script in this <code>Builder</code> is removed.
			''' Otherwise, the script must be <a href="./Locale.html#def_script">well-formed</a> or an
			''' exception is thrown.
			''' 
			''' <p>The typical script value is a four-letter script code as defined by ISO 15924.
			''' </summary>
			''' <param name="script"> the script </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>script</code> is ill-formed </exception>
			Public Function setScript(  script As String) As Builder
				Try
					localeBuilder.script = script
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Sets the region.  If region is null or the empty string, the region
			''' in this <code>Builder</code> is removed.  Otherwise,
			''' the region must be <a href="./Locale.html#def_region">well-formed</a> or an
			''' exception is thrown.
			''' 
			''' <p>The typical region value is a two-letter ISO 3166 code or a
			''' three-digit UN M.49 area code.
			''' 
			''' <p>The country value in the <code>Locale</code> created by the
			''' <code>Builder</code> is always normalized to upper case.
			''' </summary>
			''' <param name="region"> the region </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>region</code> is ill-formed </exception>
			Public Function setRegion(  region As String) As Builder
				Try
					localeBuilder.region = region
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Sets the variant.  If variant is null or the empty string, the
			''' variant in this <code>Builder</code> is removed.  Otherwise, it
			''' must consist of one or more <a href="./Locale.html#def_variant">well-formed</a>
			''' subtags, or an exception is thrown.
			''' 
			''' <p><b>Note:</b> This method checks if <code>variant</code>
			''' satisfies the IETF BCP 47 variant subtag's syntax requirements,
			''' and normalizes the value to lowercase letters.  However,
			''' the <code>Locale</code> class does not impose any syntactic
			''' restriction on variant, and the variant value in
			''' <code>Locale</code> is case sensitive.  To set such a variant,
			''' use a Locale constructor.
			''' </summary>
			''' <param name="variant"> the variant </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>variant</code> is ill-formed </exception>
			Public Function setVariant(  [variant] As String) As Builder
				Try
					localeBuilder.variant = [variant]
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Sets the extension for the given key. If the value is null or the
			''' empty string, the extension is removed.  Otherwise, the extension
			''' must be <a href="./Locale.html#def_extensions">well-formed</a> or an exception
			''' is thrown.
			''' 
			''' <p><b>Note:</b> The key {@link Locale#UNICODE_LOCALE_EXTENSION
			''' UNICODE_LOCALE_EXTENSION} ('u') is used for the Unicode locale extension.
			''' Setting a value for this key replaces any existing Unicode locale key/type
			''' pairs with those defined in the extension.
			''' 
			''' <p><b>Note:</b> The key {@link Locale#PRIVATE_USE_EXTENSION
			''' PRIVATE_USE_EXTENSION} ('x') is used for the private use code. To be
			''' well-formed, the value for this key needs only to have subtags of one to
			''' eight alphanumeric characters, not two to eight as in the general case.
			''' </summary>
			''' <param name="key"> the extension key </param>
			''' <param name="value"> the extension value </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>key</code> is illegal
			''' or <code>value</code> is ill-formed </exception>
			''' <seealso cref= #setUnicodeLocaleKeyword(String, String) </seealso>
			Public Function setExtension(  key As Char,   value As String) As Builder
				Try
					localeBuilder.extensionion(key, value)
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Sets the Unicode locale keyword type for the given key.  If the type
			''' is null, the Unicode keyword is removed.  Otherwise, the key must be
			''' non-null and both key and type must be <a
			''' href="./Locale.html#def_locale_extension">well-formed</a> or an exception
			''' is thrown.
			''' 
			''' <p>Keys and types are converted to lower case.
			''' 
			''' <p><b>Note</b>:Setting the 'u' extension via <seealso cref="#setExtension"/>
			''' replaces all Unicode locale keywords with those defined in the
			''' extension.
			''' </summary>
			''' <param name="key"> the Unicode locale key </param>
			''' <param name="type"> the Unicode locale type </param>
			''' <returns> This builder. </returns>
			''' <exception cref="IllformedLocaleException"> if <code>key</code> or <code>type</code>
			''' is ill-formed </exception>
			''' <exception cref="NullPointerException"> if <code>key</code> is null </exception>
			''' <seealso cref= #setExtension(char, String) </seealso>
			Public Function setUnicodeLocaleKeyword(  key As String,   type As String) As Builder
				Try
					localeBuilder.unicodeLocaleKeywordord(key, type)
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Adds a unicode locale attribute, if not already present, otherwise
			''' has no effect.  The attribute must not be null and must be <a
			''' href="./Locale.html#def_locale_extension">well-formed</a> or an exception
			''' is thrown.
			''' </summary>
			''' <param name="attribute"> the attribute </param>
			''' <returns> This builder. </returns>
			''' <exception cref="NullPointerException"> if <code>attribute</code> is null </exception>
			''' <exception cref="IllformedLocaleException"> if <code>attribute</code> is ill-formed </exception>
			''' <seealso cref= #setExtension(char, String) </seealso>
			Public Function addUnicodeLocaleAttribute(  attribute As String) As Builder
				Try
					localeBuilder.addUnicodeLocaleAttribute(attribute)
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Removes a unicode locale attribute, if present, otherwise has no
			''' effect.  The attribute must not be null and must be <a
			''' href="./Locale.html#def_locale_extension">well-formed</a> or an exception
			''' is thrown.
			''' 
			''' <p>Attribute comparision for removal is case-insensitive.
			''' </summary>
			''' <param name="attribute"> the attribute </param>
			''' <returns> This builder. </returns>
			''' <exception cref="NullPointerException"> if <code>attribute</code> is null </exception>
			''' <exception cref="IllformedLocaleException"> if <code>attribute</code> is ill-formed </exception>
			''' <seealso cref= #setExtension(char, String) </seealso>
			Public Function removeUnicodeLocaleAttribute(  attribute As String) As Builder
				Try
					localeBuilder.removeUnicodeLocaleAttribute(attribute)
				Catch e As sun.util.locale.LocaleSyntaxException
					Throw New IllformedLocaleException(e.Message, e.errorIndex)
				End Try
				Return Me
			End Function

			''' <summary>
			''' Resets the builder to its initial, empty state.
			''' </summary>
			''' <returns> This builder. </returns>
			Public Function clear() As Builder
				localeBuilder.clear()
				Return Me
			End Function

			''' <summary>
			''' Resets the extensions to their initial, empty state.
			''' Language, script, region and variant are unchanged.
			''' </summary>
			''' <returns> This builder. </returns>
			''' <seealso cref= #setExtension(char, String) </seealso>
			Public Function clearExtensions() As Builder
				localeBuilder.clearExtensions()
				Return Me
			End Function

			''' <summary>
			''' Returns an instance of <code>Locale</code> created from the fields set
			''' on this builder.
			''' 
			''' <p>This applies the conversions listed in <seealso cref="Locale#forLanguageTag"/>
			''' when constructing a Locale. (Grandfathered tags are handled in
			''' <seealso cref="#setLanguageTag"/>.)
			''' </summary>
			''' <returns> A Locale. </returns>
			Public Function build() As Locale
				Dim baseloc As sun.util.locale.BaseLocale = localeBuilder.baseLocale
				Dim extensions As sun.util.locale.LocaleExtensions = localeBuilder.localeExtensions
				If extensions Is Nothing AndAlso baseloc.variant.length() > 0 Then extensions = getCompatibilityExtensions(baseloc.language, baseloc.script, baseloc.region, baseloc.variant)
				Return Locale.getInstance(baseloc, extensions)
			End Function
		End Class

		''' <summary>
		''' This enum provides constants to select a filtering mode for locale
		''' matching. Refer to <a href="http://tools.ietf.org/html/rfc4647">RFC 4647
		''' Matching of Language Tags</a> for details.
		''' 
		''' <p>As an example, think of two Language Priority Lists each of which
		''' includes only one language range and a set of following language tags:
		''' 
		''' <pre>
		'''    de (German)
		'''    de-DE (German, Germany)
		'''    de-Deva (German, in Devanagari script)
		'''    de-Deva-DE (German, in Devanagari script, Germany)
		'''    de-DE-1996 (German, Germany, orthography of 1996)
		'''    de-Latn-DE (German, in Latin script, Germany)
		'''    de-Latn-DE-1996 (German, in Latin script, Germany, orthography of 1996)
		''' </pre>
		''' 
		''' The filtering method will behave as follows:
		''' 
		''' <table cellpadding=2 summary="Filtering method behavior">
		''' <tr>
		''' <th>Filtering Mode</th>
		''' <th>Language Priority List: {@code "de-DE"}</th>
		''' <th>Language Priority List: {@code "de-*-DE"}</th>
		''' </tr>
		''' <tr>
		''' <td valign=top>
		''' <seealso cref="FilteringMode#AUTOSELECT_FILTERING AUTOSELECT_FILTERING"/>
		''' </td>
		''' <td valign=top>
		''' Performs <em>basic</em> filtering and returns {@code "de-DE"} and
		''' {@code "de-DE-1996"}.
		''' </td>
		''' <td valign=top>
		''' Performs <em>extended</em> filtering and returns {@code "de-DE"},
		''' {@code "de-Deva-DE"}, {@code "de-DE-1996"}, {@code "de-Latn-DE"}, and
		''' {@code "de-Latn-DE-1996"}.
		''' </td>
		''' </tr>
		''' <tr>
		''' <td valign=top>
		''' <seealso cref="FilteringMode#EXTENDED_FILTERING EXTENDED_FILTERING"/>
		''' </td>
		''' <td valign=top>
		''' Performs <em>extended</em> filtering and returns {@code "de-DE"},
		''' {@code "de-Deva-DE"}, {@code "de-DE-1996"}, {@code "de-Latn-DE"}, and
		''' {@code "de-Latn-DE-1996"}.
		''' </td>
		''' <td valign=top>Same as above.</td>
		''' </tr>
		''' <tr>
		''' <td valign=top>
		''' <seealso cref="FilteringMode#IGNORE_EXTENDED_RANGES IGNORE_EXTENDED_RANGES"/>
		''' </td>
		''' <td valign=top>
		''' Performs <em>basic</em> filtering and returns {@code "de-DE"} and
		''' {@code "de-DE-1996"}.
		''' </td>
		''' <td valign=top>
		''' Performs <em>basic</em> filtering and returns {@code null} because
		''' nothing matches.
		''' </td>
		''' </tr>
		''' <tr>
		''' <td valign=top>
		''' <seealso cref="FilteringMode#MAP_EXTENDED_RANGES MAP_EXTENDED_RANGES"/>
		''' </td>
		''' <td valign=top>Same as above.</td>
		''' <td valign=top>
		''' Performs <em>basic</em> filtering and returns {@code "de-DE"} and
		''' {@code "de-DE-1996"} because {@code "de-*-DE"} is mapped to
		''' {@code "de-DE"}.
		''' </td>
		''' </tr>
		''' <tr>
		''' <td valign=top>
		''' <seealso cref="FilteringMode#REJECT_EXTENDED_RANGES REJECT_EXTENDED_RANGES"/>
		''' </td>
		''' <td valign=top>Same as above.</td>
		''' <td valign=top>
		''' Throws <seealso cref="IllegalArgumentException"/> because {@code "de-*-DE"} is
		''' not a valid basic language range.
		''' </td>
		''' </tr>
		''' </table>
		''' </summary>
		''' <seealso cref= #filter(List, Collection, FilteringMode) </seealso>
		''' <seealso cref= #filterTags(List, Collection, FilteringMode)
		''' 
		''' @since 1.8 </seealso>
		Public Enum FilteringMode
			''' <summary>
			''' Specifies automatic filtering mode based on the given Language
			''' Priority List consisting of language ranges. If all of the ranges
			''' are basic, basic filtering is selected. Otherwise, extended
			''' filtering is selected.
			''' </summary>
			AUTOSELECT_FILTERING

			''' <summary>
			''' Specifies extended filtering.
			''' </summary>
			EXTENDED_FILTERING

			''' <summary>
			''' Specifies basic filtering: Note that any extended language ranges
			''' included in the given Language Priority List are ignored.
			''' </summary>
			IGNORE_EXTENDED_RANGES

			''' <summary>
			''' Specifies basic filtering: If any extended language ranges are
			''' included in the given Language Priority List, they are mapped to the
			''' basic language range. Specifically, a language range starting with a
			''' subtag {@code "*"} is treated as a language range {@code "*"}. For
			''' example, {@code "*-US"} is treated as {@code "*"}. If {@code "*"} is
			''' not the first subtag, {@code "*"} and extra {@code "-"} are removed.
			''' For example, {@code "ja-*-JP"} is mapped to {@code "ja-JP"}.
			''' </summary>
			MAP_EXTENDED_RANGES

			''' <summary>
			''' Specifies basic filtering: If any extended language ranges are
			''' included in the given Language Priority List, the list is rejected
			''' and the filtering method throws <seealso cref="IllegalArgumentException"/>.
			''' </summary>
			REJECT_EXTENDED_RANGES
		End Enum

		''' <summary>
		''' This class expresses a <em>Language Range</em> defined in
		''' <a href="http://tools.ietf.org/html/rfc4647">RFC 4647 Matching of
		''' Language Tags</a>. A language range is an identifier which is used to
		''' select language tag(s) meeting specific requirements by using the
		''' mechanisms described in <a href="Locale.html#LocaleMatching">Locale
		''' Matching</a>. A list which represents a user's preferences and consists
		''' of language ranges is called a <em>Language Priority List</em>.
		''' 
		''' <p>There are two types of language ranges: basic and extended. In RFC
		''' 4647, the syntax of language ranges is expressed in
		''' <a href="http://tools.ietf.org/html/rfc4234">ABNF</a> as follows:
		''' <blockquote>
		''' <pre>
		'''     basic-language-range    = (1*8ALPHA *("-" 1*8alphanum)) / "*"
		'''     extended-language-range = (1*8ALPHA / "*")
		'''                               *("-" (1*8alphanum / "*"))
		'''     alphanum                = ALPHA / DIGIT
		''' </pre>
		''' </blockquote>
		''' For example, {@code "en"} (English), {@code "ja-JP"} (Japanese, Japan),
		''' {@code "*"} (special language range which matches any language tag) are
		''' basic language ranges, whereas {@code "*-CH"} (any languages,
		''' Switzerland), {@code "es-*"} (Spanish, any regions), and
		''' {@code "zh-Hant-*"} (Traditional Chinese, any regions) are extended
		''' language ranges.
		''' </summary>
		''' <seealso cref= #filter </seealso>
		''' <seealso cref= #filterTags </seealso>
		''' <seealso cref= #lookup </seealso>
		''' <seealso cref= #lookupTag
		''' 
		''' @since 1.8 </seealso>
		Public NotInheritable Class LanguageRange

		   ''' <summary>
		   ''' A constant holding the maximum value of weight, 1.0, which indicates
		   ''' that the language range is a good fit for the user.
		   ''' </summary>
			Public Const MAX_WEIGHT As Double = 1.0

		   ''' <summary>
		   ''' A constant holding the minimum value of weight, 0.0, which indicates
		   ''' that the language range is not a good fit for the user.
		   ''' </summary>
			Public Const MIN_WEIGHT As Double = 0.0

			Private ReadOnly range As String
			Private ReadOnly weight As Double

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private hash As Integer = 0

			''' <summary>
			''' Constructs a {@code LanguageRange} using the given {@code range}.
			''' Note that no validation is done against the IANA Language Subtag
			''' Registry at time of construction.
			''' 
			''' <p>This is equivalent to {@code LanguageRange(range, MAX_WEIGHT)}.
			''' </summary>
			''' <param name="range"> a language range </param>
			''' <exception cref="NullPointerException"> if the given {@code range} is
			'''     {@code null} </exception>
			Public Sub New(  range As String)
				Me.New(range, MAX_WEIGHT)
			End Sub

			''' <summary>
			''' Constructs a {@code LanguageRange} using the given {@code range} and
			''' {@code weight}. Note that no validation is done against the IANA
			''' Language Subtag Registry at time of construction.
			''' </summary>
			''' <param name="range">  a language range </param>
			''' <param name="weight"> a weight value between {@code MIN_WEIGHT} and
			'''     {@code MAX_WEIGHT} </param>
			''' <exception cref="NullPointerException"> if the given {@code range} is
			'''     {@code null} </exception>
			''' <exception cref="IllegalArgumentException"> if the given {@code weight} is less
			'''     than {@code MIN_WEIGHT} or greater than {@code MAX_WEIGHT} </exception>
			Public Sub New(  range As String,   weight As Double)
				If range Is Nothing Then Throw New NullPointerException
				If weight < MIN_WEIGHT OrElse weight > MAX_WEIGHT Then Throw New IllegalArgumentException("weight=" & weight)

				range = range.ToLower()

				' Do syntax check.
				Dim isIllFormed As Boolean = False
				Dim subtags As String() = range.Split("-")
				If isSubtagIllFormed(subtags(0), True) OrElse range.EndsWith("-") Then
					isIllFormed = True
				Else
					For i As Integer = 1 To subtags.Length - 1
						If isSubtagIllFormed(subtags(i), False) Then
							isIllFormed = True
							Exit For
						End If
					Next i
				End If
				If isIllFormed Then Throw New IllegalArgumentException("range=" & range)

				Me.range = range
				Me.weight = weight
			End Sub

			Private Shared Function isSubtagIllFormed(  subtag As String,   isFirstSubtag As Boolean) As Boolean
				If subtag.Equals("") OrElse subtag.length() > 8 Then
					Return True
				ElseIf subtag.Equals("*") Then
					Return False
				End If
				Dim charArray As Char() = subtag.ToCharArray()
				If isFirstSubtag Then ' ALPHA
					For Each c As Char In charArray
						If c < "a"c OrElse c > "z"c Then Return True
					Next c ' ALPHA / DIGIT
				Else
					For Each c As Char In charArray
						If c < "0"c OrElse (c > "9"c AndAlso c < "a"c) OrElse c > "z"c Then Return True
					Next c
				End If
				Return False
			End Function

			''' <summary>
			''' Returns the language range of this {@code LanguageRange}.
			''' </summary>
			''' <returns> the language range. </returns>
			Public Property range As String
				Get
					Return range
				End Get
			End Property

			''' <summary>
			''' Returns the weight of this {@code LanguageRange}.
			''' </summary>
			''' <returns> the weight value. </returns>
			Public Property weight As Double
				Get
					Return weight
				End Get
			End Property

			''' <summary>
			''' Parses the given {@code ranges} to generate a Language Priority List.
			''' 
			''' <p>This method performs a syntactic check for each language range in
			''' the given {@code ranges} but doesn't do validation using the IANA
			''' Language Subtag Registry.
			''' 
			''' <p>The {@code ranges} to be given can take one of the following
			''' forms:
			''' 
			''' <pre>
			'''   "Accept-Language: ja,en;q=0.4"  (weighted list with Accept-Language prefix)
			'''   "ja,en;q=0.4"                   (weighted list)
			'''   "ja,en"                         (prioritized list)
			''' </pre>
			''' 
			''' In a weighted list, each language range is given a weight value.
			''' The weight value is identical to the "quality value" in
			''' <a href="http://tools.ietf.org/html/rfc2616">RFC 2616</a>, and it
			''' expresses how much the user prefers  the language. A weight value is
			''' specified after a corresponding language range followed by
			''' {@code ";q="}, and the default weight value is {@code MAX_WEIGHT}
			''' when it is omitted.
			''' 
			''' <p>Unlike a weighted list, language ranges in a prioritized list
			''' are sorted in the descending order based on its priority. The first
			''' language range has the highest priority and meets the user's
			''' preference most.
			''' 
			''' <p>In either case, language ranges are sorted in descending order in
			''' the Language Priority List based on priority or weight. If a
			''' language range appears in the given {@code ranges} more than once,
			''' only the first one is included on the Language Priority List.
			''' 
			''' <p>The returned list consists of language ranges from the given
			''' {@code ranges} and their equivalents found in the IANA Language
			''' Subtag Registry. For example, if the given {@code ranges} is
			''' {@code "Accept-Language: iw,en-us;q=0.7,en;q=0.3"}, the elements in
			''' the list to be returned are:
			''' 
			''' <pre>
			'''  <b>Range</b>                                   <b>Weight</b>
			'''    "iw" (older tag for Hebrew)             1.0
			'''    "he" (new preferred code for Hebrew)    1.0
			'''    "en-us" (English, United States)        0.7
			'''    "en" (English)                          0.3
			''' </pre>
			''' 
			''' Two language ranges, {@code "iw"} and {@code "he"}, have the same
			''' highest priority in the list. By adding {@code "he"} to the user's
			''' Language Priority List, locale-matching method can find Hebrew as a
			''' matching locale (or language tag) even if the application or system
			''' offers only {@code "he"} as a supported locale (or language tag).
			''' </summary>
			''' <param name="ranges"> a list of comma-separated language ranges or a list of
			'''     language ranges in the form of the "Accept-Language" header
			'''     defined in <a href="http://tools.ietf.org/html/rfc2616">RFC
			'''     2616</a> </param>
			''' <returns> a Language Priority List consisting of language ranges
			'''     included in the given {@code ranges} and their equivalent
			'''     language ranges if available. The list is modifiable. </returns>
			''' <exception cref="NullPointerException"> if {@code ranges} is null </exception>
			''' <exception cref="IllegalArgumentException"> if a language range or a weight
			'''     found in the given {@code ranges} is ill-formed </exception>
			Public Shared Function parse(  ranges As String) As List(Of LanguageRange)
				Return sun.util.locale.LocaleMatcher.parse(ranges)
			End Function

			''' <summary>
			''' Parses the given {@code ranges} to generate a Language Priority
			''' List, and then customizes the list using the given {@code map}.
			''' This method is equivalent to
			''' {@code mapEquivalents(parse(ranges), map)}.
			''' </summary>
			''' <param name="ranges"> a list of comma-separated language ranges or a list
			'''     of language ranges in the form of the "Accept-Language" header
			'''     defined in <a href="http://tools.ietf.org/html/rfc2616">RFC
			'''     2616</a> </param>
			''' <param name="map"> a map containing information to customize language ranges </param>
			''' <returns> a Language Priority List with customization. The list is
			'''     modifiable. </returns>
			''' <exception cref="NullPointerException"> if {@code ranges} is null </exception>
			''' <exception cref="IllegalArgumentException"> if a language range or a weight
			'''     found in the given {@code ranges} is ill-formed </exception>
			''' <seealso cref= #parse(String) </seealso>
			''' <seealso cref= #mapEquivalents </seealso>
			Public Shared Function parse(  ranges As String,   map As Map(Of String, List(Of String))) As List(Of LanguageRange)
				Return mapEquivalents(parse(ranges), map)
			End Function

			''' <summary>
			''' Generates a new customized Language Priority List using the given
			''' {@code priorityList} and {@code map}. If the given {@code map} is
			''' empty, this method returns a copy of the given {@code priorityList}.
			''' 
			''' <p>In the map, a key represents a language range whereas a value is
			''' a list of equivalents of it. {@code '*'} cannot be used in the map.
			''' Each equivalent language range has the same weight value as its
			''' original language range.
			''' 
			''' <pre>
			'''  An example of map:
			'''    <b>Key</b>                            <b>Value</b>
			'''      "zh" (Chinese)                 "zh",
			'''                                     "zh-Hans"(Simplified Chinese)
			'''      "zh-HK" (Chinese, Hong Kong)   "zh-HK"
			'''      "zh-TW" (Chinese, Taiwan)      "zh-TW"
			''' </pre>
			''' 
			''' The customization is performed after modification using the IANA
			''' Language Subtag Registry.
			''' 
			''' <p>For example, if a user's Language Priority List consists of five
			''' language ranges ({@code "zh"}, {@code "zh-CN"}, {@code "en"},
			''' {@code "zh-TW"}, and {@code "zh-HK"}), the newly generated Language
			''' Priority List which is customized using the above map example will
			''' consists of {@code "zh"}, {@code "zh-Hans"}, {@code "zh-CN"},
			''' {@code "zh-Hans-CN"}, {@code "en"}, {@code "zh-TW"}, and
			''' {@code "zh-HK"}.
			''' 
			''' <p>{@code "zh-HK"} and {@code "zh-TW"} aren't converted to
			''' {@code "zh-Hans-HK"} nor {@code "zh-Hans-TW"} even if they are
			''' included in the Language Priority List. In this example, mapping
			''' is used to clearly distinguish Simplified Chinese and Traditional
			''' Chinese.
			''' 
			''' <p>If the {@code "zh"}-to-{@code "zh"} mapping isn't included in the
			''' map, a simple replacement will be performed and the customized list
			''' won't include {@code "zh"} and {@code "zh-CN"}.
			''' </summary>
			''' <param name="priorityList"> user's Language Priority List </param>
			''' <param name="map"> a map containing information to customize language ranges </param>
			''' <returns> a new Language Priority List with customization. The list is
			'''     modifiable. </returns>
			''' <exception cref="NullPointerException"> if {@code priorityList} is {@code null} </exception>
			''' <seealso cref= #parse(String, Map) </seealso>
			Public Shared Function mapEquivalents(  priorityList As List(Of LanguageRange),   map As Map(Of String, List(Of String))) As List(Of LanguageRange)
				Return sun.util.locale.LocaleMatcher.mapEquivalents(priorityList, map)
			End Function

			''' <summary>
			''' Returns a hash code value for the object.
			''' </summary>
			''' <returns>  a hash code value for this object. </returns>
			Public Overrides Function GetHashCode() As Integer
				If hash = 0 Then
					Dim result As Integer = 17
					result = 37*result + range.GetHashCode()
					Dim bitsWeight As Long = java.lang.[Double].doubleToLongBits(weight)
					result = 37*result + CInt(Fix(bitsWeight Xor (CLng(CULng(bitsWeight) >> 32))))
					hash = result
				End If
				Return hash
			End Function

			''' <summary>
			''' Compares this object to the specified object. The result is true if
			''' and only if the argument is not {@code null} and is a
			''' {@code LanguageRange} object that contains the same {@code range}
			''' and {@code weight} values as this object.
			''' </summary>
			''' <param name="obj"> the object to compare with </param>
			''' <returns>  {@code true} if this object's {@code range} and
			'''     {@code weight} are the same as the {@code obj}'s; {@code false}
			'''     otherwise. </returns>
			Public Overrides Function Equals(  obj As Object) As Boolean
				If Me Is obj Then Return True
				If Not(TypeOf obj Is LanguageRange) Then Return False
				Dim other As LanguageRange = CType(obj, LanguageRange)
				Return hash = other.hash AndAlso range.Equals(other.range) AndAlso weight = other.weight
			End Function
		End Class

		''' <summary>
		''' Returns a list of matching {@code Locale} instances using the filtering
		''' mechanism defined in RFC 4647.
		''' </summary>
		''' <param name="priorityList"> user's Language Priority List in which each language
		'''     tag is sorted in descending order based on priority or weight </param>
		''' <param name="locales"> {@code Locale} instances used for matching </param>
		''' <param name="mode"> filtering mode </param>
		''' <returns> a list of {@code Locale} instances for matching language tags
		'''     sorted in descending order based on priority or weight, or an empty
		'''     list if nothing matches. The list is modifiable. </returns>
		''' <exception cref="NullPointerException"> if {@code priorityList} or {@code locales}
		'''     is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if one or more extended language ranges
		'''     are included in the given list when
		'''     <seealso cref="FilteringMode#REJECT_EXTENDED_RANGES"/> is specified
		''' 
		''' @since 1.8 </exception>
		Public Shared Function filter(  priorityList As List(Of LanguageRange),   locales As Collection(Of Locale),   mode As FilteringMode) As List(Of Locale)
			Return sun.util.locale.LocaleMatcher.filter(priorityList, locales, mode)
		End Function

		''' <summary>
		''' Returns a list of matching {@code Locale} instances using the filtering
		''' mechanism defined in RFC 4647. This is equivalent to
		''' <seealso cref="#filter(List, Collection, FilteringMode)"/> when {@code mode} is
		''' <seealso cref="FilteringMode#AUTOSELECT_FILTERING"/>.
		''' </summary>
		''' <param name="priorityList"> user's Language Priority List in which each language
		'''     tag is sorted in descending order based on priority or weight </param>
		''' <param name="locales"> {@code Locale} instances used for matching </param>
		''' <returns> a list of {@code Locale} instances for matching language tags
		'''     sorted in descending order based on priority or weight, or an empty
		'''     list if nothing matches. The list is modifiable. </returns>
		''' <exception cref="NullPointerException"> if {@code priorityList} or {@code locales}
		'''     is {@code null}
		''' 
		''' @since 1.8 </exception>
		Public Shared Function filter(  priorityList As List(Of LanguageRange),   locales As Collection(Of Locale)) As List(Of Locale)
			Return filter(priorityList, locales, FilteringMode.AUTOSELECT_FILTERING)
		End Function

		''' <summary>
		''' Returns a list of matching languages tags using the basic filtering
		''' mechanism defined in RFC 4647.
		''' </summary>
		''' <param name="priorityList"> user's Language Priority List in which each language
		'''     tag is sorted in descending order based on priority or weight </param>
		''' <param name="tags"> language tags </param>
		''' <param name="mode"> filtering mode </param>
		''' <returns> a list of matching language tags sorted in descending order
		'''     based on priority or weight, or an empty list if nothing matches.
		'''     The list is modifiable. </returns>
		''' <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		'''     {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if one or more extended language ranges
		'''     are included in the given list when
		'''     <seealso cref="FilteringMode#REJECT_EXTENDED_RANGES"/> is specified
		''' 
		''' @since 1.8 </exception>
		Public Shared Function filterTags(  priorityList As List(Of LanguageRange),   tags As Collection(Of String),   mode As FilteringMode) As List(Of String)
			Return sun.util.locale.LocaleMatcher.filterTags(priorityList, tags, mode)
		End Function

		''' <summary>
		''' Returns a list of matching languages tags using the basic filtering
		''' mechanism defined in RFC 4647. This is equivalent to
		''' <seealso cref="#filterTags(List, Collection, FilteringMode)"/> when {@code mode}
		''' is <seealso cref="FilteringMode#AUTOSELECT_FILTERING"/>.
		''' </summary>
		''' <param name="priorityList"> user's Language Priority List in which each language
		'''     tag is sorted in descending order based on priority or weight </param>
		''' <param name="tags"> language tags </param>
		''' <returns> a list of matching language tags sorted in descending order
		'''     based on priority or weight, or an empty list if nothing matches.
		'''     The list is modifiable. </returns>
		''' <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		'''     {@code null}
		''' 
		''' @since 1.8 </exception>
		Public Shared Function filterTags(  priorityList As List(Of LanguageRange),   tags As Collection(Of String)) As List(Of String)
			Return filterTags(priorityList, tags, FilteringMode.AUTOSELECT_FILTERING)
		End Function

		''' <summary>
		''' Returns a {@code Locale} instance for the best-matching language
		''' tag using the lookup mechanism defined in RFC 4647.
		''' </summary>
		''' <param name="priorityList"> user's Language Priority List in which each language
		'''     tag is sorted in descending order based on priority or weight </param>
		''' <param name="locales"> {@code Locale} instances used for matching </param>
		''' <returns> the best matching <code>Locale</code> instance chosen based on
		'''     priority or weight, or {@code null} if nothing matches. </returns>
		''' <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		'''     {@code null}
		''' 
		''' @since 1.8 </exception>
		Public Shared Function lookup(  priorityList As List(Of LanguageRange),   locales As Collection(Of Locale)) As Locale
			Return sun.util.locale.LocaleMatcher.lookup(priorityList, locales)
		End Function

		''' <summary>
		''' Returns the best-matching language tag using the lookup mechanism
		''' defined in RFC 4647.
		''' </summary>
		''' <param name="priorityList"> user's Language Priority List in which each language
		'''     tag is sorted in descending order based on priority or weight </param>
		''' <param name="tags"> language tangs used for matching </param>
		''' <returns> the best matching language tag chosen based on priority or
		'''     weight, or {@code null} if nothing matches. </returns>
		''' <exception cref="NullPointerException"> if {@code priorityList} or {@code tags} is
		'''     {@code null}
		''' 
		''' @since 1.8 </exception>
		Public Shared Function lookupTag(  priorityList As List(Of LanguageRange),   tags As Collection(Of String)) As String
			Return sun.util.locale.LocaleMatcher.lookupTag(priorityList, tags)
		End Function

	End Class

End Namespace
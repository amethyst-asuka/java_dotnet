Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.regex



	''' <summary>
	''' A compiled representation of a regular expression.
	''' 
	''' <p> A regular expression, specified as a string, must first be compiled into
	''' an instance of this class.  The resulting pattern can then be used to create
	''' a <seealso cref="Matcher"/> object that can match arbitrary {@linkplain
	''' java.lang.CharSequence character sequences} against the regular
	''' expression.  All of the state involved in performing a match resides in the
	''' matcher, so many matchers can share the same pattern.
	''' 
	''' <p> A typical invocation sequence is thus
	''' 
	''' <blockquote><pre>
	''' Pattern p = Pattern.<seealso cref="#compile compile"/>("a*b");
	''' Matcher m = p.<seealso cref="#matcher matcher"/>("aaaaab");
	''' boolean b = m.<seealso cref="Matcher#matches matches"/>();</pre></blockquote>
	''' 
	''' <p> A <seealso cref="#matches matches"/> method is defined by this class as a
	''' convenience for when a regular expression is used just once.  This method
	''' compiles an expression and matches an input sequence against it in a single
	''' invocation.  The statement
	''' 
	''' <blockquote><pre>
	''' boolean b = Pattern.matches("a*b", "aaaaab");</pre></blockquote>
	''' 
	''' is equivalent to the three statements above, though for repeated matches it
	''' is less efficient since it does not allow the compiled pattern to be reused.
	''' 
	''' <p> Instances of this class are immutable and are safe for use by multiple
	''' concurrent threads.  Instances of the <seealso cref="Matcher"/> class are not safe for
	''' such use.
	''' 
	''' 
	''' <h3><a name="sum">Summary of regular-expression constructs</a></h3>
	''' 
	''' <table border="0" cellpadding="1" cellspacing="0"
	'''  summary="Regular expression constructs, and what they match">
	''' 
	''' <tr align="left">
	''' <th align="left" id="construct">Construct</th>
	''' <th align="left" id="matches">Matches</th>
	''' </tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="characters">Characters</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct characters"><i>x</i></td>
	'''     <td headers="matches">The character <i>x</i></td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\\</tt></td>
	'''     <td headers="matches">The backslash character</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\0</tt><i>n</i></td>
	'''     <td headers="matches">The character with octal value <tt>0</tt><i>n</i>
	'''         (0&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;7)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\0</tt><i>nn</i></td>
	'''     <td headers="matches">The character with octal value <tt>0</tt><i>nn</i>
	'''         (0&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;7)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\0</tt><i>mnn</i></td>
	'''     <td headers="matches">The character with octal value <tt>0</tt><i>mnn</i>
	'''         (0&nbsp;<tt>&lt;=</tt>&nbsp;<i>m</i>&nbsp;<tt>&lt;=</tt>&nbsp;3,
	'''         0&nbsp;<tt>&lt;=</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;7)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\x</tt><i>hh</i></td>
	'''     <td headers="matches">The character with hexadecimal&nbsp;value&nbsp;<tt>0x</tt><i>hh</i></td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>&#92;u</tt><i>hhhh</i></td>
	'''     <td headers="matches">The character with hexadecimal&nbsp;value&nbsp;<tt>0x</tt><i>hhhh</i></td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>&#92;x</tt><i>{h...h}</i></td>
	'''     <td headers="matches">The character with hexadecimal&nbsp;value&nbsp;<tt>0x</tt><i>h...h</i>
	'''         (<seealso cref="java.lang.Character#MIN_CODE_POINT Character.MIN_CODE_POINT"/>
	'''         &nbsp;&lt;=&nbsp;<tt>0x</tt><i>h...h</i>&nbsp;&lt;=&nbsp;
	'''          <seealso cref="java.lang.Character#MAX_CODE_POINT Character.MAX_CODE_POINT"/>)</td></tr>
	''' <tr><td valign="top" headers="matches"><tt>\t</tt></td>
	'''     <td headers="matches">The tab character (<tt>'&#92;u0009'</tt>)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\n</tt></td>
	'''     <td headers="matches">The newline (line feed) character (<tt>'&#92;u000A'</tt>)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\r</tt></td>
	'''     <td headers="matches">The carriage-return character (<tt>'&#92;u000D'</tt>)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\f</tt></td>
	'''     <td headers="matches">The form-feed character (<tt>'&#92;u000C'</tt>)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\a</tt></td>
	'''     <td headers="matches">The alert (bell) character (<tt>'&#92;u0007'</tt>)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\e</tt></td>
	'''     <td headers="matches">The escape character (<tt>'&#92;u001B'</tt>)</td></tr>
	''' <tr><td valign="top" headers="construct characters"><tt>\c</tt><i>x</i></td>
	'''     <td headers="matches">The control character corresponding to <i>x</i></td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="classes">Character classes</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct classes">{@code [abc]}</td>
	'''     <td headers="matches">{@code a}, {@code b}, or {@code c} (simple [Class])</td></tr>
	''' <tr><td valign="top" headers="construct classes">{@code [^abc]}</td>
	'''     <td headers="matches">Any character except {@code a}, {@code b}, or {@code c} (negation)</td></tr>
	''' <tr><td valign="top" headers="construct classes">{@code [a-zA-Z]}</td>
	'''     <td headers="matches">{@code a} through {@code z}
	'''         or {@code A} through {@code Z}, inclusive (range)</td></tr>
	''' <tr><td valign="top" headers="construct classes">{@code [a-d[m-p]]}</td>
	'''     <td headers="matches">{@code a} through {@code d},
	'''      or {@code m} through {@code p}: {@code [a-dm-p]} (union)</td></tr>
	''' <tr><td valign="top" headers="construct classes">{@code [a-z&&[def]]}</td>
	'''     <td headers="matches">{@code d}, {@code e}, or {@code f} (intersection)</tr>
	''' <tr><td valign="top" headers="construct classes">{@code [a-z&&[^bc]]}</td>
	'''     <td headers="matches">{@code a} through {@code z},
	'''         except for {@code b} and {@code c}: {@code [ad-z]} (subtraction)</td></tr>
	''' <tr><td valign="top" headers="construct classes">{@code [a-z&&[^m-p]]}</td>
	'''     <td headers="matches">{@code a} through {@code z},
	'''          and not {@code m} through {@code p}: {@code [a-lq-z]}(subtraction)</td></tr>
	''' <tr><th>&nbsp;</th></tr>
	''' 
	''' <tr align="left"><th colspan="2" id="predef">Predefined character classes</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct predef"><tt>.</tt></td>
	'''     <td headers="matches">Any character (may or may not match <a href="#lt">line terminators</a>)</td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\d</tt></td>
	'''     <td headers="matches">A digit: <tt>[0-9]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\D</tt></td>
	'''     <td headers="matches">A non-digit: <tt>[^0-9]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\h</tt></td>
	'''     <td headers="matches">A horizontal whitespace character:
	'''     <tt>[ \t\xA0&#92;u1680&#92;u180e&#92;u2000-&#92;u200a&#92;u202f&#92;u205f&#92;u3000]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\H</tt></td>
	'''     <td headers="matches">A non-horizontal whitespace character: <tt>[^\h]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\s</tt></td>
	'''     <td headers="matches">A whitespace character: <tt>[ \t\n\x0B\f\r]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\S</tt></td>
	'''     <td headers="matches">A non-whitespace character: <tt>[^\s]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\v</tt></td>
	'''     <td headers="matches">A vertical whitespace character: <tt>[\n\x0B\f\r\x85&#92;u2028&#92;u2029]</tt>
	'''     </td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\V</tt></td>
	'''     <td headers="matches">A non-vertical whitespace character: <tt>[^\v]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\w</tt></td>
	'''     <td headers="matches">A word character: <tt>[a-zA-Z_0-9]</tt></td></tr>
	''' <tr><td valign="top" headers="construct predef"><tt>\W</tt></td>
	'''     <td headers="matches">A non-word character: <tt>[^\w]</tt></td></tr>
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="posix"><b>POSIX character classes (US-ASCII only)</b></th></tr>
	''' 
	''' <tr><td valign="top" headers="construct posix">{@code \p{Lower}}</td>
	'''     <td headers="matches">A lower-case alphabetic character: {@code [a-z]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Upper}}</td>
	'''     <td headers="matches">An upper-case alphabetic character:{@code [A-Z]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{ASCII}}</td>
	'''     <td headers="matches">All ASCII:{@code [\x00-\x7F]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Alpha}}</td>
	'''     <td headers="matches">An alphabetic character:{@code [\p{Lower}\p{Upper}]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Digit}}</td>
	'''     <td headers="matches">A decimal digit: {@code [0-9]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Alnum}}</td>
	'''     <td headers="matches">An alphanumeric character:{@code [\p{Alpha}\p{Digit}]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Punct}}</td>
	'''     <td headers="matches">Punctuation: One of {@code !"#$%&'()*+,-./:;<=>?@[\]^_`{|}~}</td></tr>
	'''     <!-- {@code [\!"#\$%&'\(\)\*\+,\-\./:;\<=\>\?@\[\\\]\^_`\{\|\}~]}
	'''          {@code [\X21-\X2F\X31-\X40\X5B-\X60\X7B-\X7E]} -->
	''' <tr><td valign="top" headers="construct posix">{@code \p{Graph}}</td>
	'''     <td headers="matches">A visible character: {@code [\p{Alnum}\p{Punct}]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Print}}</td>
	'''     <td headers="matches">A printable character: {@code [\p{Graph}\x20]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Blank}}</td>
	'''     <td headers="matches">A space or a tab: {@code [ \t]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Cntrl}}</td>
	'''     <td headers="matches">A control character: {@code [\x00-\x1F\x7F]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{XDigit}}</td>
	'''     <td headers="matches">A hexadecimal digit: {@code [0-9a-fA-F]}</td></tr>
	''' <tr><td valign="top" headers="construct posix">{@code \p{Space}}</td>
	'''     <td headers="matches">A whitespace character: {@code [ \t\n\x0B\f\r]}</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2">java.lang.Character classes (simple <a href="#jcc">java character type</a>)</th></tr>
	''' 
	''' <tr><td valign="top"><tt>\p{javaLowerCase}</tt></td>
	'''     <td>Equivalent to java.lang.Character.isLowerCase()</td></tr>
	''' <tr><td valign="top"><tt>\p{javaUpperCase}</tt></td>
	'''     <td>Equivalent to java.lang.Character.isUpperCase()</td></tr>
	''' <tr><td valign="top"><tt>\p{javaWhitespace}</tt></td>
	'''     <td>Equivalent to java.lang.Character.isWhitespace()</td></tr>
	''' <tr><td valign="top"><tt>\p{javaMirrored}</tt></td>
	'''     <td>Equivalent to java.lang.Character.isMirrored()</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="unicode">Classes for Unicode scripts, blocks, categories and binary properties</th></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code \p{IsLatin}}</td>
	'''     <td headers="matches">A Latin&nbsp;script character (<a href="#usc">script</a>)</td></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code \p{InGreek}}</td>
	'''     <td headers="matches">A character in the Greek&nbsp;block (<a href="#ubc">block</a>)</td></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code \p{Lu}}</td>
	'''     <td headers="matches">An uppercase letter (<a href="#ucc">category</a>)</td></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code \p{IsAlphabetic}}</td>
	'''     <td headers="matches">An alphabetic character (<a href="#ubpc">binary property</a>)</td></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code \p{Sc}}</td>
	'''     <td headers="matches">A currency symbol</td></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code \P{InGreek}}</td>
	'''     <td headers="matches">Any character except one in the Greek block (negation)</td></tr>
	''' <tr><td valign="top" headers="construct unicode">{@code [\p{L}&&[^\p{Lu}]]}</td>
	'''     <td headers="matches">Any letter except an uppercase letter (subtraction)</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="bounds">Boundary matchers</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct bounds"><tt>^</tt></td>
	'''     <td headers="matches">The beginning of a line</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>$</tt></td>
	'''     <td headers="matches">The end of a line</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>\b</tt></td>
	'''     <td headers="matches">A word boundary</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>\B</tt></td>
	'''     <td headers="matches">A non-word boundary</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>\A</tt></td>
	'''     <td headers="matches">The beginning of the input</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>\G</tt></td>
	'''     <td headers="matches">The end of the previous match</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>\Z</tt></td>
	'''     <td headers="matches">The end of the input but for the final
	'''         <a href="#lt">terminator</a>, if&nbsp;any</td></tr>
	''' <tr><td valign="top" headers="construct bounds"><tt>\z</tt></td>
	'''     <td headers="matches">The end of the input</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="lineending">Linebreak matcher</th></tr>
	''' <tr><td valign="top" headers="construct lineending"><tt>\R</tt></td>
	'''     <td headers="matches">Any Unicode linebreak sequence, is equivalent to
	'''     <tt>&#92;u000D&#92;u000A|[&#92;u000A&#92;u000B&#92;u000C&#92;u000D&#92;u0085&#92;u2028&#92;u2029]
	'''     </tt></td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="greedy">Greedy quantifiers</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct greedy"><i>X</i><tt>?</tt></td>
	'''     <td headers="matches"><i>X</i>, once or not at all</td></tr>
	''' <tr><td valign="top" headers="construct greedy"><i>X</i><tt>*</tt></td>
	'''     <td headers="matches"><i>X</i>, zero or more times</td></tr>
	''' <tr><td valign="top" headers="construct greedy"><i>X</i><tt>+</tt></td>
	'''     <td headers="matches"><i>X</i>, one or more times</td></tr>
	''' <tr><td valign="top" headers="construct greedy"><i>X</i><tt>{</tt><i>n</i><tt>}</tt></td>
	'''     <td headers="matches"><i>X</i>, exactly <i>n</i> times</td></tr>
	''' <tr><td valign="top" headers="construct greedy"><i>X</i><tt>{</tt><i>n</i><tt>,}</tt></td>
	'''     <td headers="matches"><i>X</i>, at least <i>n</i> times</td></tr>
	''' <tr><td valign="top" headers="construct greedy"><i>X</i><tt>{</tt><i>n</i><tt>,</tt><i>m</i><tt>}</tt></td>
	'''     <td headers="matches"><i>X</i>, at least <i>n</i> but not more than <i>m</i> times</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="reluc">Reluctant quantifiers</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct reluc"><i>X</i><tt>??</tt></td>
	'''     <td headers="matches"><i>X</i>, once or not at all</td></tr>
	''' <tr><td valign="top" headers="construct reluc"><i>X</i><tt>*?</tt></td>
	'''     <td headers="matches"><i>X</i>, zero or more times</td></tr>
	''' <tr><td valign="top" headers="construct reluc"><i>X</i><tt>+?</tt></td>
	'''     <td headers="matches"><i>X</i>, one or more times</td></tr>
	''' <tr><td valign="top" headers="construct reluc"><i>X</i><tt>{</tt><i>n</i><tt>}?</tt></td>
	'''     <td headers="matches"><i>X</i>, exactly <i>n</i> times</td></tr>
	''' <tr><td valign="top" headers="construct reluc"><i>X</i><tt>{</tt><i>n</i><tt>,}?</tt></td>
	'''     <td headers="matches"><i>X</i>, at least <i>n</i> times</td></tr>
	''' <tr><td valign="top" headers="construct reluc"><i>X</i><tt>{</tt><i>n</i><tt>,</tt><i>m</i><tt>}?</tt></td>
	'''     <td headers="matches"><i>X</i>, at least <i>n</i> but not more than <i>m</i> times</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="poss">Possessive quantifiers</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct poss"><i>X</i><tt>?+</tt></td>
	'''     <td headers="matches"><i>X</i>, once or not at all</td></tr>
	''' <tr><td valign="top" headers="construct poss"><i>X</i><tt>*+</tt></td>
	'''     <td headers="matches"><i>X</i>, zero or more times</td></tr>
	''' <tr><td valign="top" headers="construct poss"><i>X</i><tt>++</tt></td>
	'''     <td headers="matches"><i>X</i>, one or more times</td></tr>
	''' <tr><td valign="top" headers="construct poss"><i>X</i><tt>{</tt><i>n</i><tt>}+</tt></td>
	'''     <td headers="matches"><i>X</i>, exactly <i>n</i> times</td></tr>
	''' <tr><td valign="top" headers="construct poss"><i>X</i><tt>{</tt><i>n</i><tt>,}+</tt></td>
	'''     <td headers="matches"><i>X</i>, at least <i>n</i> times</td></tr>
	''' <tr><td valign="top" headers="construct poss"><i>X</i><tt>{</tt><i>n</i><tt>,</tt><i>m</i><tt>}+</tt></td>
	'''     <td headers="matches"><i>X</i>, at least <i>n</i> but not more than <i>m</i> times</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="logical">Logical operators</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct logical"><i>XY</i></td>
	'''     <td headers="matches"><i>X</i> followed by <i>Y</i></td></tr>
	''' <tr><td valign="top" headers="construct logical"><i>X</i><tt>|</tt><i>Y</i></td>
	'''     <td headers="matches">Either <i>X</i> or <i>Y</i></td></tr>
	''' <tr><td valign="top" headers="construct logical"><tt>(</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches">X, as a <a href="#cg">capturing group</a></td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="backref">Back references</th></tr>
	''' 
	''' <tr><td valign="bottom" headers="construct backref"><tt>\</tt><i>n</i></td>
	'''     <td valign="bottom" headers="matches">Whatever the <i>n</i><sup>th</sup>
	'''     <a href="#cg">capturing group</a> matched</td></tr>
	''' 
	''' <tr><td valign="bottom" headers="construct backref"><tt>\</tt><i>k</i>&lt;<i>name</i>&gt;</td>
	'''     <td valign="bottom" headers="matches">Whatever the
	'''     <a href="#groupname">named-capturing group</a> "name" matched</td></tr>
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="quot">Quotation</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct quot"><tt>\</tt></td>
	'''     <td headers="matches">Nothing, but quotes the following character</td></tr>
	''' <tr><td valign="top" headers="construct quot"><tt>\Q</tt></td>
	'''     <td headers="matches">Nothing, but quotes all characters until <tt>\E</tt></td></tr>
	''' <tr><td valign="top" headers="construct quot"><tt>\E</tt></td>
	'''     <td headers="matches">Nothing, but ends quoting started by <tt>\Q</tt></td></tr>
	'''     <!-- Metachars: !$()*+.<>?[\]^{|} -->
	''' 
	''' <tr><th>&nbsp;</th></tr>
	''' <tr align="left"><th colspan="2" id="special">Special constructs (named-capturing and non-capturing)</th></tr>
	''' 
	''' <tr><td valign="top" headers="construct special"><tt>(?&lt;<a href="#groupname">name</a>&gt;</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, as a named-capturing group</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?:</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, as a non-capturing group</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?idmsuxU-idmsuxU)&nbsp;</tt></td>
	'''     <td headers="matches">Nothing, but turns match flags <a href="#CASE_INSENSITIVE">i</a>
	''' <a href="#UNIX_LINES">d</a> <a href="#MULTILINE">m</a> <a href="#DOTALL">s</a>
	''' <a href="#UNICODE_CASE">u</a> <a href="#COMMENTS">x</a> <a href="#UNICODE_CHARACTER_CLASS">U</a>
	''' on - off</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?idmsux-idmsux:</tt><i>X</i><tt>)</tt>&nbsp;&nbsp;</td>
	'''     <td headers="matches"><i>X</i>, as a <a href="#cg">non-capturing group</a> with the
	'''         given flags <a href="#CASE_INSENSITIVE">i</a> <a href="#UNIX_LINES">d</a>
	''' <a href="#MULTILINE">m</a> <a href="#DOTALL">s</a> <a href="#UNICODE_CASE">u</a >
	''' <a href="#COMMENTS">x</a> on - off</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?=</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, via zero-width positive lookahead</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?!</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, via zero-width negative lookahead</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?&lt;=</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, via zero-width positive lookbehind</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?&lt;!</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, via zero-width negative lookbehind</td></tr>
	''' <tr><td valign="top" headers="construct special"><tt>(?&gt;</tt><i>X</i><tt>)</tt></td>
	'''     <td headers="matches"><i>X</i>, as an independent, non-capturing group</td></tr>
	''' 
	''' </table>
	''' 
	''' <hr>
	''' 
	''' 
	''' <h3><a name="bs">Backslashes, escapes, and quoting</a></h3>
	''' 
	''' <p> The backslash character (<tt>'\'</tt>) serves to introduce escaped
	''' constructs, as defined in the table above, as well as to quote characters
	''' that otherwise would be interpreted as unescaped constructs.  Thus the
	''' expression <tt>\\</tt> matches a single backslash and <tt>\{</tt> matches a
	''' left brace.
	''' 
	''' <p> It is an error to use a backslash prior to any alphabetic character that
	''' does not denote an escaped construct; these are reserved for future
	''' extensions to the regular-expression language.  A backslash may be used
	''' prior to a non-alphabetic character regardless of whether that character is
	''' part of an unescaped construct.
	''' 
	''' <p> Backslashes within string literals in Java source code are interpreted
	''' as required by
	''' <cite>The Java&trade; Language Specification</cite>
	''' as either Unicode escapes (section 3.3) or other character escapes (section 3.10.6)
	''' It is therefore necessary to double backslashes in string
	''' literals that represent regular expressions to protect them from
	''' interpretation by the Java bytecode compiler.  The string literal
	''' <tt>"&#92;b"</tt>, for example, matches a single backspace character when
	''' interpreted as a regular expression, while <tt>"&#92;&#92;b"</tt> matches a
	''' word boundary.  The string literal <tt>"&#92;(hello&#92;)"</tt> is illegal
	''' and leads to a compile-time error; in order to match the string
	''' <tt>(hello)</tt> the string literal <tt>"&#92;&#92;(hello&#92;&#92;)"</tt>
	''' must be used.
	''' 
	''' <h3><a name="cc">Character Classes</a></h3>
	''' 
	'''    <p> Character classes may appear within other character classes, and
	'''    may be composed by the union operator (implicit) and the intersection
	'''    operator (<tt>&amp;&amp;</tt>).
	'''    The union operator denotes a class that contains every character that is
	'''    in at least one of its operand classes.  The intersection operator
	'''    denotes a class that contains every character that is in both of its
	'''    operand classes.
	''' 
	'''    <p> The precedence of character-class operators is as follows, from
	'''    highest to lowest:
	''' 
	'''    <blockquote><table border="0" cellpadding="1" cellspacing="0"
	'''                 summary="Precedence of character class operators.">
	'''      <tr><th>1&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''        <td>Literal escape&nbsp;&nbsp;&nbsp;&nbsp;</td>
	'''        <td><tt>\x</tt></td></tr>
	'''     <tr><th>2&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''        <td>Grouping</td>
	'''        <td><tt>[...]</tt></td></tr>
	'''     <tr><th>3&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''        <td>Range</td>
	'''        <td><tt>a-z</tt></td></tr>
	'''      <tr><th>4&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''        <td>Union</td>
	'''        <td><tt>[a-e][i-u]</tt></td></tr>
	'''      <tr><th>5&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''        <td>Intersection</td>
	'''        <td>{@code [a-z&&[aeiou]]}</td></tr>
	'''    </table></blockquote>
	''' 
	'''    <p> Note that a different set of metacharacters are in effect inside
	'''    a character class than outside a character class. For instance, the
	'''    regular expression <tt>.</tt> loses its special meaning inside a
	'''    character [Class], while the expression <tt>-</tt> becomes a range
	'''    forming metacharacter.
	''' 
	''' <h3><a name="lt">Line terminators</a></h3>
	''' 
	''' <p> A <i>line terminator</i> is a one- or two-character sequence that marks
	''' the end of a line of the input character sequence.  The following are
	''' recognized as line terminators:
	''' 
	''' <ul>
	''' 
	'''   <li> A newline (line feed) character&nbsp;(<tt>'\n'</tt>),
	''' 
	'''   <li> A carriage-return character followed immediately by a newline
	'''   character&nbsp;(<tt>"\r\n"</tt>),
	''' 
	'''   <li> A standalone carriage-return character&nbsp;(<tt>'\r'</tt>),
	''' 
	'''   <li> A next-line character&nbsp;(<tt>'&#92;u0085'</tt>),
	''' 
	'''   <li> A line-separator character&nbsp;(<tt>'&#92;u2028'</tt>), or
	''' 
	'''   <li> A paragraph-separator character&nbsp;(<tt>'&#92;u2029</tt>).
	''' 
	''' </ul>
	''' <p>If <seealso cref="#UNIX_LINES"/> mode is activated, then the only line terminators
	''' recognized are newline characters.
	''' 
	''' <p> The regular expression <tt>.</tt> matches any character except a line
	''' terminator unless the <seealso cref="#DOTALL"/> flag is specified.
	''' 
	''' <p> By default, the regular expressions <tt>^</tt> and <tt>$</tt> ignore
	''' line terminators and only match at the beginning and the end, respectively,
	''' of the entire input sequence. If <seealso cref="#MULTILINE"/> mode is activated then
	''' <tt>^</tt> matches at the beginning of input and after any line terminator
	''' except at the end of input. When in <seealso cref="#MULTILINE"/> mode <tt>$</tt>
	''' matches just before a line terminator or the end of the input sequence.
	''' 
	''' <h3><a name="cg">Groups and capturing</a></h3>
	''' 
	''' <h4><a name="gnumber">Group number</a></h4>
	''' <p> Capturing groups are numbered by counting their opening parentheses from
	''' left to right.  In the expression <tt>((A)(B(C)))</tt>, for example, there
	''' are four such groups: </p>
	''' 
	''' <blockquote><table cellpadding=1 cellspacing=0 summary="Capturing group numberings">
	''' <tr><th>1&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''     <td><tt>((A)(B(C)))</tt></td></tr>
	''' <tr><th>2&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''     <td><tt>(A)</tt></td></tr>
	''' <tr><th>3&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''     <td><tt>(B(C))</tt></td></tr>
	''' <tr><th>4&nbsp;&nbsp;&nbsp;&nbsp;</th>
	'''     <td><tt>(C)</tt></td></tr>
	''' </table></blockquote>
	''' 
	''' <p> Group zero always stands for the entire expression.
	''' 
	''' <p> Capturing groups are so named because, during a match, each subsequence
	''' of the input sequence that matches such a group is saved.  The captured
	''' subsequence may be used later in the expression, via a back reference, and
	''' may also be retrieved from the matcher once the match operation is complete.
	''' 
	''' <h4><a name="groupname">Group name</a></h4>
	''' <p>A capturing group can also be assigned a "name", a <tt>named-capturing group</tt>,
	''' and then be back-referenced later by the "name". Group names are composed of
	''' the following characters. The first character must be a <tt>letter</tt>.
	''' 
	''' <ul>
	'''   <li> The uppercase letters <tt>'A'</tt> through <tt>'Z'</tt>
	'''        (<tt>'&#92;u0041'</tt>&nbsp;through&nbsp;<tt>'&#92;u005a'</tt>),
	'''   <li> The lowercase letters <tt>'a'</tt> through <tt>'z'</tt>
	'''        (<tt>'&#92;u0061'</tt>&nbsp;through&nbsp;<tt>'&#92;u007a'</tt>),
	'''   <li> The digits <tt>'0'</tt> through <tt>'9'</tt>
	'''        (<tt>'&#92;u0030'</tt>&nbsp;through&nbsp;<tt>'&#92;u0039'</tt>),
	''' </ul>
	''' 
	''' <p> A <tt>named-capturing group</tt> is still numbered as described in
	''' <a href="#gnumber">Group number</a>.
	''' 
	''' <p> The captured input associated with a group is always the subsequence
	''' that the group most recently matched.  If a group is evaluated a second time
	''' because of quantification then its previously-captured value, if any, will
	''' be retained if the second evaluation fails.  Matching the string
	''' <tt>"aba"</tt> against the expression <tt>(a(b)?)+</tt>, for example, leaves
	''' group two set to <tt>"b"</tt>.  All captured input is discarded at the
	''' beginning of each match.
	''' 
	''' <p> Groups beginning with <tt>(?</tt> are either pure, <i>non-capturing</i> groups
	''' that do not capture text and do not count towards the group total, or
	''' <i>named-capturing</i> group.
	''' 
	''' <h3> Unicode support </h3>
	''' 
	''' <p> This class is in conformance with Level 1 of <a
	''' href="http://www.unicode.org/reports/tr18/"><i>Unicode Technical
	''' Standard #18: Unicode Regular Expression</i></a>, plus RL2.1
	''' Canonical Equivalents.
	''' <p>
	''' <b>Unicode escape sequences</b> such as <tt>&#92;u2014</tt> in Java source code
	''' are processed as described in section 3.3 of
	''' <cite>The Java&trade; Language Specification</cite>.
	''' Such escape sequences are also implemented directly by the regular-expression
	''' parser so that Unicode escapes can be used in expressions that are read from
	''' files or from the keyboard.  Thus the strings <tt>"&#92;u2014"</tt> and
	''' <tt>"\\u2014"</tt>, while not equal, compile into the same pattern, which
	''' matches the character with hexadecimal value <tt>0x2014</tt>.
	''' <p>
	''' A Unicode character can also be represented in a regular-expression by
	''' using its <b>Hex notation</b>(hexadecimal code point value) directly as described in construct
	''' <tt>&#92;x{...}</tt>, for example a supplementary character U+2011F
	''' can be specified as <tt>&#92;x{2011F}</tt>, instead of two consecutive
	''' Unicode escape sequences of the surrogate pair
	''' <tt>&#92;uD840</tt><tt>&#92;uDD1F</tt>.
	''' <p>
	''' Unicode scripts, blocks, categories and binary properties are written with
	''' the <tt>\p</tt> and <tt>\P</tt> constructs as in Perl.
	''' <tt>\p{</tt><i>prop</i><tt>}</tt> matches if
	''' the input has the property <i>prop</i>, while <tt>\P{</tt><i>prop</i><tt>}</tt>
	''' does not match if the input has that property.
	''' <p>
	''' Scripts, blocks, categories and binary properties can be used both inside
	''' and outside of a character class.
	''' 
	''' <p>
	''' <b><a name="usc">Scripts</a></b> are specified either with the prefix {@code Is}, as in
	''' {@code IsHiragana}, or by using  the {@code script} keyword (or its short
	''' form {@code sc})as in {@code script=Hiragana} or {@code sc=Hiragana}.
	''' <p>
	''' The script names supported by <code>Pattern</code> are the valid script names
	''' accepted and defined by
	''' <seealso cref="java.lang.Character.UnicodeScript#forName(String) UnicodeScript.forName"/>.
	''' 
	''' <p>
	''' <b><a name="ubc">Blocks</a></b> are specified with the prefix {@code In}, as in
	''' {@code InMongolian}, or by using the keyword {@code block} (or its short
	''' form {@code blk}) as in {@code block=Mongolian} or {@code blk=Mongolian}.
	''' <p>
	''' The block names supported by <code>Pattern</code> are the valid block names
	''' accepted and defined by
	''' <seealso cref="java.lang.Character.UnicodeBlock#forName(String) UnicodeBlock.forName"/>.
	''' <p>
	''' 
	''' <b><a name="ucc">Categories</a></b> may be specified with the optional prefix {@code Is}:
	''' Both {@code \p{L}} and {@code \p{IsL}} denote the category of Unicode
	''' letters. Same as scripts and blocks, categories can also be specified
	''' by using the keyword {@code general_category} (or its short form
	''' {@code gc}) as in {@code general_category=Lu} or {@code gc=Lu}.
	''' <p>
	''' The supported categories are those of
	''' <a href="http://www.unicode.org/unicode/standard/standard.html">
	''' <i>The Unicode Standard</i></a> in the version specified by the
	''' <seealso cref="java.lang.Character Character"/> class. The category names are those
	''' defined in the Standard, both normative and informative.
	''' <p>
	''' 
	''' <b><a name="ubpc">Binary properties</a></b> are specified with the prefix {@code Is}, as in
	''' {@code IsAlphabetic}. The supported binary properties by <code>Pattern</code>
	''' are
	''' <ul>
	'''   <li> Alphabetic
	'''   <li> Ideographic
	'''   <li> Letter
	'''   <li> Lowercase
	'''   <li> Uppercase
	'''   <li> Titlecase
	'''   <li> Punctuation
	'''   <Li> Control
	'''   <li> White_Space
	'''   <li> Digit
	'''   <li> Hex_Digit
	'''   <li> Join_Control
	'''   <li> Noncharacter_Code_Point
	'''   <li> Assigned
	''' </ul>
	''' <p>
	''' The following <b>Predefined Character classes</b> and <b>POSIX character classes</b>
	''' are in conformance with the recommendation of <i>Annex C: Compatibility Properties</i>
	''' of <a href="http://www.unicode.org/reports/tr18/"><i>Unicode Regular Expression
	''' </i></a>, when <seealso cref="#UNICODE_CHARACTER_CLASS"/> flag is specified.
	''' 
	''' <table border="0" cellpadding="1" cellspacing="0"
	'''  summary="predefined and posix character classes in Unicode mode">
	''' <tr align="left">
	''' <th align="left" id="predef_classes">Classes</th>
	''' <th align="left" id="predef_matches">Matches</th>
	''' </tr>
	''' <tr><td><tt>\p{Lower}</tt></td>
	'''     <td>A lowercase character:<tt>\p{IsLowercase}</tt></td></tr>
	''' <tr><td><tt>\p{Upper}</tt></td>
	'''     <td>An uppercase character:<tt>\p{IsUppercase}</tt></td></tr>
	''' <tr><td><tt>\p{ASCII}</tt></td>
	'''     <td>All ASCII:<tt>[\x00-\x7F]</tt></td></tr>
	''' <tr><td><tt>\p{Alpha}</tt></td>
	'''     <td>An alphabetic character:<tt>\p{IsAlphabetic}</tt></td></tr>
	''' <tr><td><tt>\p{Digit}</tt></td>
	'''     <td>A decimal digit character:<tt>p{IsDigit}</tt></td></tr>
	''' <tr><td><tt>\p{Alnum}</tt></td>
	'''     <td>An alphanumeric character:<tt>[\p{IsAlphabetic}\p{IsDigit}]</tt></td></tr>
	''' <tr><td><tt>\p{Punct}</tt></td>
	'''     <td>A punctuation character:<tt>p{IsPunctuation}</tt></td></tr>
	''' <tr><td><tt>\p{Graph}</tt></td>
	'''     <td>A visible character: <tt>[^\p{IsWhite_Space}\p{gc=Cc}\p{gc=Cs}\p{gc=Cn}]</tt></td></tr>
	''' <tr><td><tt>\p{Print}</tt></td>
	'''     <td>A printable character: {@code [\p{Graph}\p{Blank}&&[^\p{Cntrl}]]}</td></tr>
	''' <tr><td><tt>\p{Blank}</tt></td>
	'''     <td>A space or a tab: {@code [\p{IsWhite_Space}&&[^\p{gc=Zl}\p{gc=Zp}\x0a\x0b\x0c\x0d\x85]]}</td></tr>
	''' <tr><td><tt>\p{Cntrl}</tt></td>
	'''     <td>A control character: <tt>\p{gc=Cc}</tt></td></tr>
	''' <tr><td><tt>\p{XDigit}</tt></td>
	'''     <td>A hexadecimal digit: <tt>[\p{gc=Nd}\p{IsHex_Digit}]</tt></td></tr>
	''' <tr><td><tt>\p{Space}</tt></td>
	'''     <td>A whitespace character:<tt>\p{IsWhite_Space}</tt></td></tr>
	''' <tr><td><tt>\d</tt></td>
	'''     <td>A digit: <tt>\p{IsDigit}</tt></td></tr>
	''' <tr><td><tt>\D</tt></td>
	'''     <td>A non-digit: <tt>[^\d]</tt></td></tr>
	''' <tr><td><tt>\s</tt></td>
	'''     <td>A whitespace character: <tt>\p{IsWhite_Space}</tt></td></tr>
	''' <tr><td><tt>\S</tt></td>
	'''     <td>A non-whitespace character: <tt>[^\s]</tt></td></tr>
	''' <tr><td><tt>\w</tt></td>
	'''     <td>A word character: <tt>[\p{Alpha}\p{gc=Mn}\p{gc=Me}\p{gc=Mc}\p{Digit}\p{gc=Pc}\p{IsJoin_Control}]</tt></td></tr>
	''' <tr><td><tt>\W</tt></td>
	'''     <td>A non-word character: <tt>[^\w]</tt></td></tr>
	''' </table>
	''' <p>
	''' <a name="jcc">
	''' Categories that behave like the java.lang.Character
	''' boolean is<i>methodname</i> methods (except for the deprecated ones) are
	''' available through the same <tt>\p{</tt><i>prop</i><tt>}</tt> syntax where
	''' the specified property has the name <tt>java<i>methodname</i></tt></a>.
	''' 
	''' <h3> Comparison to Perl 5 </h3>
	''' 
	''' <p>The <code>Pattern</code> engine performs traditional NFA-based matching
	''' with ordered alternation as occurs in Perl 5.
	''' 
	''' <p> Perl constructs not supported by this class: </p>
	''' 
	''' <ul>
	'''    <li><p> Predefined character classes (Unicode character)
	'''    <p><tt>\X&nbsp;&nbsp;&nbsp;&nbsp;</tt>Match Unicode
	'''    <a href="http://www.unicode.org/reports/tr18/#Default_Grapheme_Clusters">
	'''    <i>extended grapheme cluster</i></a>
	'''    </p></li>
	''' 
	'''    <li><p> The backreference constructs, <tt>\g{</tt><i>n</i><tt>}</tt> for
	'''    the <i>n</i><sup>th</sup><a href="#cg">capturing group</a> and
	'''    <tt>\g{</tt><i>name</i><tt>}</tt> for
	'''    <a href="#groupname">named-capturing group</a>.
	'''    </p></li>
	''' 
	'''    <li><p> The named character construct, <tt>\N{</tt><i>name</i><tt>}</tt>
	'''    for a Unicode character by its name.
	'''    </p></li>
	''' 
	'''    <li><p> The conditional constructs
	'''    <tt>(?(</tt><i>condition</i><tt>)</tt><i>X</i><tt>)</tt> and
	'''    <tt>(?(</tt><i>condition</i><tt>)</tt><i>X</i><tt>|</tt><i>Y</i><tt>)</tt>,
	'''    </p></li>
	''' 
	'''    <li><p> The embedded code constructs <tt>(?{</tt><i>code</i><tt>})</tt>
	'''    and <tt>(??{</tt><i>code</i><tt>})</tt>,</p></li>
	''' 
	'''    <li><p> The embedded comment syntax <tt>(?#comment)</tt>, and </p></li>
	''' 
	'''    <li><p> The preprocessing operations <tt>\l</tt> <tt>&#92;u</tt>,
	'''    <tt>\L</tt>, and <tt>\U</tt>.  </p></li>
	''' 
	''' </ul>
	''' 
	''' <p> Constructs supported by this class but not by Perl: </p>
	''' 
	''' <ul>
	''' 
	'''    <li><p> Character-class union and intersection as described
	'''    <a href="#cc">above</a>.</p></li>
	''' 
	''' </ul>
	''' 
	''' <p> Notable differences from Perl: </p>
	''' 
	''' <ul>
	''' 
	'''    <li><p> In Perl, <tt>\1</tt> through <tt>\9</tt> are always interpreted
	'''    as back references; a backslash-escaped number greater than <tt>9</tt> is
	'''    treated as a back reference if at least that many subexpressions exist,
	'''    otherwise it is interpreted, if possible, as an octal escape.  In this
	'''    class octal escapes must always begin with a zero. In this [Class],
	'''    <tt>\1</tt> through <tt>\9</tt> are always interpreted as back
	'''    references, and a larger number is accepted as a back reference if at
	'''    least that many subexpressions exist at that point in the regular
	'''    expression, otherwise the parser will drop digits until the number is
	'''    smaller or equal to the existing number of groups or it is one digit.
	'''    </p></li>
	''' 
	'''    <li><p> Perl uses the <tt>g</tt> flag to request a match that resumes
	'''    where the last match left off.  This functionality is provided implicitly
	'''    by the <seealso cref="Matcher"/> class: Repeated invocations of the {@link
	'''    Matcher#find find} method will resume where the last match left off,
	'''    unless the matcher is reset.  </p></li>
	''' 
	'''    <li><p> In Perl, embedded flags at the top level of an expression affect
	'''    the whole expression.  In this [Class], embedded flags always take effect
	'''    at the point at which they appear, whether they are at the top level or
	'''    within a group; in the latter case, flags are restored at the end of the
	'''    group just as in Perl.  </p></li>
	''' 
	''' </ul>
	''' 
	''' 
	''' <p> For a more precise description of the behavior of regular expression
	''' constructs, please see <a href="http://www.oreilly.com/catalog/regex3/">
	''' <i>Mastering Regular Expressions, 3nd Edition</i>, Jeffrey E. F. Friedl,
	''' O'Reilly and Associates, 2006.</a>
	''' </p>
	''' </summary>
	''' <seealso cref= java.lang.String#split(String, int) </seealso>
	''' <seealso cref= java.lang.String#split(String)
	''' 
	''' @author      Mike McCloskey
	''' @author      Mark Reinhold
	''' @author      JSR-51 Expert Group
	''' @since       1.4
	''' @spec        JSR-51 </seealso>

	<Serializable> _
	Public NotInheritable Class Pattern

		''' <summary>
		''' Regular expression modifier values.  Instead of being passed as
		''' arguments, they can also be passed as inline modifiers.
		''' For example, the following statements have the same effect.
		''' <pre>
		''' RegExp r1 = RegExp.compile("abc", Pattern.I|Pattern.M);
		''' RegExp r2 = RegExp.compile("(?im)abc", 0);
		''' </pre>
		''' 
		''' The flags are duplicated so that the familiar Perl match flag
		''' names are available.
		''' </summary>

		''' <summary>
		''' Enables Unix lines mode.
		''' 
		''' <p> In this mode, only the <tt>'\n'</tt> line terminator is recognized
		''' in the behavior of <tt>.</tt>, <tt>^</tt>, and <tt>$</tt>.
		''' 
		''' <p> Unix lines mode can also be enabled via the embedded flag
		''' expression&nbsp;<tt>(?d)</tt>.
		''' </summary>
		Public Const UNIX_LINES As Integer = &H1

		''' <summary>
		''' Enables case-insensitive matching.
		''' 
		''' <p> By default, case-insensitive matching assumes that only characters
		''' in the US-ASCII charset are being matched.  Unicode-aware
		''' case-insensitive matching can be enabled by specifying the {@link
		''' #UNICODE_CASE} flag in conjunction with this flag.
		''' 
		''' <p> Case-insensitive matching can also be enabled via the embedded flag
		''' expression&nbsp;<tt>(?i)</tt>.
		''' 
		''' <p> Specifying this flag may impose a slight performance penalty.  </p>
		''' </summary>
		Public Const CASE_INSENSITIVE As Integer = &H2

		''' <summary>
		''' Permits whitespace and comments in pattern.
		''' 
		''' <p> In this mode, whitespace is ignored, and embedded comments starting
		''' with <tt>#</tt> are ignored until the end of a line.
		''' 
		''' <p> Comments mode can also be enabled via the embedded flag
		''' expression&nbsp;<tt>(?x)</tt>.
		''' </summary>
		Public Const COMMENTS As Integer = &H4

		''' <summary>
		''' Enables multiline mode.
		''' 
		''' <p> In multiline mode the expressions <tt>^</tt> and <tt>$</tt> match
		''' just after or just before, respectively, a line terminator or the end of
		''' the input sequence.  By default these expressions only match at the
		''' beginning and the end of the entire input sequence.
		''' 
		''' <p> Multiline mode can also be enabled via the embedded flag
		''' expression&nbsp;<tt>(?m)</tt>.  </p>
		''' </summary>
		Public Const MULTILINE As Integer = &H8

		''' <summary>
		''' Enables literal parsing of the pattern.
		''' 
		''' <p> When this flag is specified then the input string that specifies
		''' the pattern is treated as a sequence of literal characters.
		''' Metacharacters or escape sequences in the input sequence will be
		''' given no special meaning.
		''' 
		''' <p>The flags CASE_INSENSITIVE and UNICODE_CASE retain their impact on
		''' matching when used in conjunction with this flag. The other flags
		''' become superfluous.
		''' 
		''' <p> There is no embedded flag character for enabling literal parsing.
		''' @since 1.5
		''' </summary>
		Public Const LITERAL As Integer = &H10

		''' <summary>
		''' Enables dotall mode.
		''' 
		''' <p> In dotall mode, the expression <tt>.</tt> matches any character,
		''' including a line terminator.  By default this expression does not match
		''' line terminators.
		''' 
		''' <p> Dotall mode can also be enabled via the embedded flag
		''' expression&nbsp;<tt>(?s)</tt>.  (The <tt>s</tt> is a mnemonic for
		''' "single-line" mode, which is what this is called in Perl.)  </p>
		''' </summary>
		Public Const DOTALL As Integer = &H20

		''' <summary>
		''' Enables Unicode-aware case folding.
		''' 
		''' <p> When this flag is specified then case-insensitive matching, when
		''' enabled by the <seealso cref="#CASE_INSENSITIVE"/> flag, is done in a manner
		''' consistent with the Unicode Standard.  By default, case-insensitive
		''' matching assumes that only characters in the US-ASCII charset are being
		''' matched.
		''' 
		''' <p> Unicode-aware case folding can also be enabled via the embedded flag
		''' expression&nbsp;<tt>(?u)</tt>.
		''' 
		''' <p> Specifying this flag may impose a performance penalty.  </p>
		''' </summary>
		Public Const UNICODE_CASE As Integer = &H40

		''' <summary>
		''' Enables canonical equivalence.
		''' 
		''' <p> When this flag is specified then two characters will be considered
		''' to match if, and only if, their full canonical decompositions match.
		''' The expression <tt>"a&#92;u030A"</tt>, for example, will match the
		''' string <tt>"&#92;u00E5"</tt> when this flag is specified.  By default,
		''' matching does not take canonical equivalence into account.
		''' 
		''' <p> There is no embedded flag character for enabling canonical
		''' equivalence.
		''' 
		''' <p> Specifying this flag may impose a performance penalty.  </p>
		''' </summary>
		Public Const CANON_EQ As Integer = &H80

		''' <summary>
		''' Enables the Unicode version of <i>Predefined character classes</i> and
		''' <i>POSIX character classes</i>.
		''' 
		''' <p> When this flag is specified then the (US-ASCII only)
		''' <i>Predefined character classes</i> and <i>POSIX character classes</i>
		''' are in conformance with
		''' <a href="http://www.unicode.org/reports/tr18/"><i>Unicode Technical
		''' Standard #18: Unicode Regular Expression</i></a>
		''' <i>Annex C: Compatibility Properties</i>.
		''' <p>
		''' The UNICODE_CHARACTER_CLASS mode can also be enabled via the embedded
		''' flag expression&nbsp;<tt>(?U)</tt>.
		''' <p>
		''' The flag implies UNICODE_CASE, that is, it enables Unicode-aware case
		''' folding.
		''' <p>
		''' Specifying this flag may impose a performance penalty.  </p>
		''' @since 1.7
		''' </summary>
		Public Const UNICODE_CHARACTER_CLASS As Integer = &H100

	'     Pattern has only two serialized components: The pattern string
	'     * and the flags, which are all that is needed to recompile the pattern
	'     * when it is deserialized.
	'     

		''' <summary>
		''' use serialVersionUID from Merlin b59 for interoperability </summary>
		Private Const serialVersionUID As Long = 5073258162644648461L

		''' <summary>
		''' The original regular-expression pattern string.
		''' 
		''' @serial
		''' </summary>
		Private pattern_Renamed As String

		''' <summary>
		''' The original pattern flags.
		''' 
		''' @serial
		''' </summary>
		Private flags_Renamed As Integer

		''' <summary>
		''' Boolean indicating this Pattern is compiled; this is necessary in order
		''' to lazily compile deserialized Patterns.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private compiled As Boolean = False

		''' <summary>
		''' The normalized pattern string.
		''' </summary>
		<NonSerialized> _
		Private normalizedPattern As String

		''' <summary>
		''' The starting point of state machine for the find operation.  This allows
		''' a match to start anywhere in the input.
		''' </summary>
		<NonSerialized> _
		Friend root As Node

		''' <summary>
		''' The root of object tree for a match operation.  The pattern is matched
		''' at the beginning.  This may include a find that uses BnM or a First
		''' node.
		''' </summary>
		<NonSerialized> _
		Friend matchRoot As Node

		''' <summary>
		''' Temporary storage used by parsing pattern slice.
		''' </summary>
		<NonSerialized> _
		Friend buffer As Integer()

		''' <summary>
		''' Map the "name" of the "named capturing group" to its group id
		''' node.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend namedGroups As IDictionary(Of String, Integer?)

		''' <summary>
		''' Temporary storage used while parsing group references.
		''' </summary>
		<NonSerialized> _
		Friend groupNodes As GroupHead()

		''' <summary>
		''' Temporary null terminated code point array used by pattern compiling.
		''' </summary>
		<NonSerialized> _
		Private temp As Integer()

		''' <summary>
		''' The number of capturing groups in this Pattern. Used by matchers to
		''' allocate storage needed to perform a match.
		''' </summary>
		<NonSerialized> _
		Friend capturingGroupCount As Integer

		''' <summary>
		''' The local variable count used by parsing tree. Used by matchers to
		''' allocate storage needed to perform a match.
		''' </summary>
		<NonSerialized> _
		Friend localCount As Integer

		''' <summary>
		''' Index into the pattern string that keeps track of how much has been
		''' parsed.
		''' </summary>
		<NonSerialized> _
		Private cursor As Integer

		''' <summary>
		''' Holds the length of the pattern string.
		''' </summary>
		<NonSerialized> _
		Private patternLength As Integer

		''' <summary>
		''' If the Start node might possibly match supplementary characters.
		''' It is set to true during compiling if
		''' (1) There is supplementary char in pattern, or
		''' (2) There is complement node of Category or Block
		''' </summary>
		<NonSerialized> _
		Private hasSupplementary As Boolean

		''' <summary>
		''' Compiles the given regular expression into a pattern.
		''' </summary>
		''' <param name="regex">
		'''         The expression to be compiled </param>
		''' <returns> the given regular expression compiled into a pattern </returns>
		''' <exception cref="PatternSyntaxException">
		'''          If the expression's syntax is invalid </exception>
		Public Shared Function compile(  regex As String) As Pattern
			Return New Pattern(regex, 0)
		End Function

		''' <summary>
		''' Compiles the given regular expression into a pattern with the given
		''' flags.
		''' </summary>
		''' <param name="regex">
		'''         The expression to be compiled
		''' </param>
		''' <param name="flags">
		'''         Match flags, a bit mask that may include
		'''         <seealso cref="#CASE_INSENSITIVE"/>, <seealso cref="#MULTILINE"/>, <seealso cref="#DOTALL"/>,
		'''         <seealso cref="#UNICODE_CASE"/>, <seealso cref="#CANON_EQ"/>, <seealso cref="#UNIX_LINES"/>,
		'''         <seealso cref="#LITERAL"/>, <seealso cref="#UNICODE_CHARACTER_CLASS"/>
		'''         and <seealso cref="#COMMENTS"/>
		''' </param>
		''' <returns> the given regular expression compiled into a pattern with the given flags </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If bit values other than those corresponding to the defined
		'''          match flags are set in <tt>flags</tt>
		''' </exception>
		''' <exception cref="PatternSyntaxException">
		'''          If the expression's syntax is invalid </exception>
		Public Shared Function compile(  regex As String,   flags As Integer) As Pattern
			Return New Pattern(regex, flags)
		End Function

		''' <summary>
		''' Returns the regular expression from which this pattern was compiled.
		''' </summary>
		''' <returns>  The source of this pattern </returns>
		Public Function pattern() As String
			Return pattern_Renamed
		End Function

		''' <summary>
		''' <p>Returns the string representation of this pattern. This
		''' is the regular expression from which this pattern was
		''' compiled.</p>
		''' </summary>
		''' <returns>  The string representation of this pattern
		''' @since 1.5 </returns>
		Public Overrides Function ToString() As String
			Return pattern_Renamed
		End Function

		''' <summary>
		''' Creates a matcher that will match the given input against this pattern.
		''' </summary>
		''' <param name="input">
		'''         The character sequence to be matched
		''' </param>
		''' <returns>  A new matcher for this pattern </returns>
		Public Function matcher(  input As CharSequence) As Matcher
			If Not compiled Then
				SyncLock Me
					If Not compiled Then compile()
				End SyncLock
			End If
			Dim m As New Matcher(Me, input)
			Return m
		End Function

		''' <summary>
		''' Returns this pattern's match flags.
		''' </summary>
		''' <returns>  The match flags specified when this pattern was compiled </returns>
		Public Function flags() As Integer
			Return flags_Renamed
		End Function

		''' <summary>
		''' Compiles the given regular expression and attempts to match the given
		''' input against it.
		''' 
		''' <p> An invocation of this convenience method of the form
		''' 
		''' <blockquote><pre>
		''' Pattern.matches(regex, input);</pre></blockquote>
		''' 
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>
		''' Pattern.compile(regex).matcher(input).matches()</pre></blockquote>
		''' 
		''' <p> If a pattern is to be used multiple times, compiling it once and reusing
		''' it will be more efficient than invoking this method each time.  </p>
		''' </summary>
		''' <param name="regex">
		'''         The expression to be compiled
		''' </param>
		''' <param name="input">
		'''         The character sequence to be matched </param>
		''' <returns> whether or not the regular expression matches on the input </returns>
		''' <exception cref="PatternSyntaxException">
		'''          If the expression's syntax is invalid </exception>
		Public Shared Function matches(  regex As String,   input As CharSequence) As Boolean
			Dim p As Pattern = Pattern.compile(regex)
			Dim m As Matcher = p.matcher(input)
			Return m.matches()
		End Function

		''' <summary>
		''' Splits the given input sequence around matches of this pattern.
		''' 
		''' <p> The array returned by this method contains each substring of the
		''' input sequence that is terminated by another subsequence that matches
		''' this pattern or is terminated by the end of the input sequence.  The
		''' substrings in the array are in the order in which they occur in the
		''' input. If this pattern does not match any subsequence of the input then
		''' the resulting array has just one element, namely the input sequence in
		''' string form.
		''' 
		''' <p> When there is a positive-width match at the beginning of the input
		''' sequence then an empty leading substring is included at the beginning
		''' of the resulting array. A zero-width match at the beginning however
		''' never produces such empty leading substring.
		''' 
		''' <p> The <tt>limit</tt> parameter controls the number of times the
		''' pattern is applied and therefore affects the length of the resulting
		''' array.  If the limit <i>n</i> is greater than zero then the pattern
		''' will be applied at most <i>n</i>&nbsp;-&nbsp;1 times, the array's
		''' length will be no greater than <i>n</i>, and the array's last entry
		''' will contain all input beyond the last matched delimiter.  If <i>n</i>
		''' is non-positive then the pattern will be applied as many times as
		''' possible and the array can have any length.  If <i>n</i> is zero then
		''' the pattern will be applied as many times as possible, the array can
		''' have any length, and trailing empty strings will be discarded.
		''' 
		''' <p> The input <tt>"boo:and:foo"</tt>, for example, yields the following
		''' results with these parameters:
		''' 
		''' <blockquote><table cellpadding=1 cellspacing=0
		'''              summary="Split examples showing regex, limit, and result">
		''' <tr><th align="left"><i>Regex&nbsp;&nbsp;&nbsp;&nbsp;</i></th>
		'''     <th align="left"><i>Limit&nbsp;&nbsp;&nbsp;&nbsp;</i></th>
		'''     <th align="left"><i>Result&nbsp;&nbsp;&nbsp;&nbsp;</i></th></tr>
		''' <tr><td align=center>:</td>
		'''     <td align=center>2</td>
		'''     <td><tt>{ "boo", "and:foo" }</tt></td></tr>
		''' <tr><td align=center>:</td>
		'''     <td align=center>5</td>
		'''     <td><tt>{ "boo", "and", "foo" }</tt></td></tr>
		''' <tr><td align=center>:</td>
		'''     <td align=center>-2</td>
		'''     <td><tt>{ "boo", "and", "foo" }</tt></td></tr>
		''' <tr><td align=center>o</td>
		'''     <td align=center>5</td>
		'''     <td><tt>{ "b", "", ":and:f", "", "" }</tt></td></tr>
		''' <tr><td align=center>o</td>
		'''     <td align=center>-2</td>
		'''     <td><tt>{ "b", "", ":and:f", "", "" }</tt></td></tr>
		''' <tr><td align=center>o</td>
		'''     <td align=center>0</td>
		'''     <td><tt>{ "b", "", ":and:f" }</tt></td></tr>
		''' </table></blockquote>
		''' </summary>
		''' <param name="input">
		'''         The character sequence to be split
		''' </param>
		''' <param name="limit">
		'''         The result threshold, as described above
		''' </param>
		''' <returns>  The array of strings computed by splitting the input
		'''          around matches of this pattern </returns>
		Public Function split(  input As CharSequence,   limit As Integer) As String()
			Dim index As Integer = 0
			Dim matchLimited As Boolean = limit > 0
			Dim matchList As New List(Of String)
			Dim m As Matcher = matcher(input)

			' Add segments before each match found
			Do While m.find()
				If (Not matchLimited) OrElse matchList.size() < limit - 1 Then
					If index = 0 AndAlso index = m.start() AndAlso m.start() = m.end() Then Continue Do
					Dim match As String = input.subSequence(index, m.start()).ToString()
					matchList.add(match)
					index = m.end() ' last one
				ElseIf matchList.size() = limit - 1 Then
					Dim match As String = input.subSequence(index, input.length()).ToString()
					matchList.add(match)
					index = m.end()
				End If
			Loop

			' If no match was found, return this
			If index = 0 Then Return New String() {input.ToString()}

			' Add remaining segment
			If (Not matchLimited) OrElse matchList.size() < limit Then matchList.add(input.subSequence(index, input.length()).ToString())

			' Construct result
			Dim resultSize As Integer = matchList.size()
			If limit = 0 Then
				Do While resultSize > 0 AndAlso matchList.get(resultSize-1).Equals("")
					resultSize -= 1
				Loop
			End If
			Dim result As String() = New String(resultSize - 1){}
			Return matchList.subList(0, resultSize).ToArray(result)
		End Function

		''' <summary>
		''' Splits the given input sequence around matches of this pattern.
		''' 
		''' <p> This method works as if by invoking the two-argument {@link
		''' #split(java.lang.CharSequence, int) split} method with the given input
		''' sequence and a limit argument of zero.  Trailing empty strings are
		''' therefore not included in the resulting array. </p>
		''' 
		''' <p> The input <tt>"boo:and:foo"</tt>, for example, yields the following
		''' results with these expressions:
		''' 
		''' <blockquote><table cellpadding=1 cellspacing=0
		'''              summary="Split examples showing regex and result">
		''' <tr><th align="left"><i>Regex&nbsp;&nbsp;&nbsp;&nbsp;</i></th>
		'''     <th align="left"><i>Result</i></th></tr>
		''' <tr><td align=center>:</td>
		'''     <td><tt>{ "boo", "and", "foo" }</tt></td></tr>
		''' <tr><td align=center>o</td>
		'''     <td><tt>{ "b", "", ":and:f" }</tt></td></tr>
		''' </table></blockquote>
		''' 
		''' </summary>
		''' <param name="input">
		'''         The character sequence to be split
		''' </param>
		''' <returns>  The array of strings computed by splitting the input
		'''          around matches of this pattern </returns>
		Public Function split(  input As CharSequence) As String()
			Return Split(input, 0)
		End Function

		''' <summary>
		''' Returns a literal pattern <code>String</code> for the specified
		''' <code>String</code>.
		''' 
		''' <p>This method produces a <code>String</code> that can be used to
		''' create a <code>Pattern</code> that would match the string
		''' <code>s</code> as if it were a literal pattern.</p> Metacharacters
		''' or escape sequences in the input sequence will be given no special
		''' meaning.
		''' </summary>
		''' <param name="s"> The string to be literalized </param>
		''' <returns>  A literal string replacement
		''' @since 1.5 </returns>
		Public Shared Function quote(  s As String) As String
			Dim slashEIndex As Integer = s.IndexOf("\E")
			If slashEIndex = -1 Then Return "\Q" & s & "\E"

			Dim sb As New StringBuilder(s.length() * 2)
			sb.append("\Q")
			slashEIndex = 0
			Dim current As Integer = 0
			slashEIndex = s.IndexOf("\E", current)
			Do While slashEIndex <> -1
				sb.append(s.Substring(current, slashEIndex - current))
				current = slashEIndex + 2
				sb.append("\E\\E\Q")
				slashEIndex = s.IndexOf("\E", current)
			Loop
			sb.append(s.Substring(current, s.length() - current))
			sb.append("\E")
			Return sb.ToString()
		End Function

		''' <summary>
		''' Recompile the Pattern instance from a stream.  The original pattern
		''' string is read in and the object tree is recompiled from it.
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)

			' Read in all fields
			s.defaultReadObject()

			' Initialize counts
			capturingGroupCount = 1
			localCount = 0

			' if length > 0, the Pattern is lazily compiled
			compiled = False
			If pattern_Renamed.length() = 0 Then
				root = New Start(lastAccept)
				matchRoot = lastAccept
				compiled = True
			End If
		End Sub

		''' <summary>
		''' This private constructor is used to create all Patterns. The pattern
		''' string and match flags are all that is needed to completely describe
		''' a Pattern. An empty pattern string results in an object tree with
		''' only a Start node and a LastNode node.
		''' </summary>
		Private Sub New(  p As String,   f As Integer)
			pattern_Renamed = p
			flags_Renamed = f

			' to use UNICODE_CASE if UNICODE_CHARACTER_CLASS present
			If (flags_Renamed And UNICODE_CHARACTER_CLASS) <> 0 Then flags_Renamed = flags_Renamed Or UNICODE_CASE

			' Reset group index count
			capturingGroupCount = 1
			localCount = 0

			If pattern_Renamed.length() > 0 Then
				compile()
			Else
				root = New Start(lastAccept)
				matchRoot = lastAccept
			End If
		End Sub

		''' <summary>
		''' The pattern is converted to normalizedD form and then a pure group
		''' is constructed to match canonical equivalences of the characters.
		''' </summary>
		Private Sub normalize()
			Dim inCharClass As Boolean = False
			Dim lastCodePoint As Integer = -1

			' Convert pattern into normalizedD form
			normalizedPattern = java.text.Normalizer.normalize(pattern_Renamed, java.text.Normalizer.Form.NFD)
			patternLength = normalizedPattern.length()

			' Modify pattern to match canonical equivalences
			Dim newPattern As New StringBuilder(patternLength)
			Dim i As Integer=0
			Do While i<patternLength
				Dim c As Integer = normalizedPattern.codePointAt(i)
				Dim sequenceBuffer As StringBuilder
				If (Character.getType(c) = Character.NON_SPACING_MARK) AndAlso (lastCodePoint <> -1) Then
					sequenceBuffer = New StringBuilder
					sequenceBuffer.appendCodePoint(lastCodePoint)
					sequenceBuffer.appendCodePoint(c)
					Do While Character.getType(c) = Character.NON_SPACING_MARK
						i += Character.charCount(c)
						If i >= patternLength Then Exit Do
						c = normalizedPattern.codePointAt(i)
						sequenceBuffer.appendCodePoint(c)
					Loop
					Dim ea As String = produceEquivalentAlternation(sequenceBuffer.ToString())
					newPattern.length = newPattern.length()-Character.charCount(lastCodePoint)
					newPattern.append("(?:").append(ea).append(")")
				ElseIf c = AscW("["c) AndAlso lastCodePoint <> AscW("\"c) Then
					i = normalizeCharClass(newPattern, i)
				Else
					newPattern.appendCodePoint(c)
				End If
				lastCodePoint = c
				i += Character.charCount(c)
			Loop
			normalizedPattern = newPattern.ToString()
		End Sub

		''' <summary>
		''' Complete the character class being parsed and add a set
		''' of alternations to it that will match the canonical equivalences
		''' of the characters within the class.
		''' </summary>
		Private Function normalizeCharClass(  newPattern As StringBuilder,   i As Integer) As Integer
			Dim charClass As New StringBuilder
			Dim eq As StringBuilder = Nothing
			Dim lastCodePoint As Integer = -1
			Dim result As String

			i += 1
			charClass.append("[")
			Do
				Dim c As Integer = normalizedPattern.codePointAt(i)
				Dim sequenceBuffer As StringBuilder

				If c = AscW("]"c) AndAlso lastCodePoint <> AscW("\"c) Then
					charClass.append(ChrW(c))
					Exit Do
				ElseIf Character.getType(c) = Character.NON_SPACING_MARK Then
					sequenceBuffer = New StringBuilder
					sequenceBuffer.appendCodePoint(lastCodePoint)
					Do While Character.getType(c) = Character.NON_SPACING_MARK
						sequenceBuffer.appendCodePoint(c)
						i += Character.charCount(c)
						If i >= normalizedPattern.length() Then Exit Do
						c = normalizedPattern.codePointAt(i)
					Loop
					Dim ea As String = produceEquivalentAlternation(sequenceBuffer.ToString())

					charClass.length = charClass.length()-Character.charCount(lastCodePoint)
					If eq Is Nothing Then eq = New StringBuilder
					eq.append("|"c)
					eq.append(ea)
				Else
					charClass.appendCodePoint(c)
					i += 1
				End If
				If i = normalizedPattern.length() Then Throw error("Unclosed character class")
				lastCodePoint = c
			Loop

			If eq IsNot Nothing Then
				result = "(?:" & charClass.ToString() & eq.ToString() & ")"
			Else
				result = charClass.ToString()
			End If

			newPattern.append(result)
			Return i
		End Function

		''' <summary>
		''' Given a specific sequence composed of a regular character and
		''' combining marks that follow it, produce the alternation that will
		''' match all canonical equivalences of that sequence.
		''' </summary>
		Private Function produceEquivalentAlternation(  source As String) As String
			Dim len As Integer = countChars(source, 0, 1)
			If source.length() = len Then Return source

			Dim base As String = source.Substring(0,len)
			Dim combiningMarks As String = source.Substring(len)

			Dim perms As String() = producePermutations(combiningMarks)
			Dim result As New StringBuilder(source)

			' Add combined permutations
			For x As Integer = 0 To perms.Length - 1
				Dim [next] As String = base + perms(x)
				If x>0 Then result.append("|" & [next])
				[next] = composeOneStep([next])
				If [next] IsNot Nothing Then result.append("|" & produceEquivalentAlternation([next]))
			Next x
			Return result.ToString()
		End Function

		''' <summary>
		''' Returns an array of strings that have all the possible
		''' permutations of the characters in the input string.
		''' This is used to get a list of all possible orderings
		''' of a set of combining marks. Note that some of the permutations
		''' are invalid because of combining class collisions, and these
		''' possibilities must be removed because they are not canonically
		''' equivalent.
		''' </summary>
		Private Function producePermutations(  input As String) As String()
			If input.length() = countChars(input, 0, 1) Then Return New String() {input}

			If input.length() = countChars(input, 0, 2) Then
				Dim c0 As Integer = Character.codePointAt(input, 0)
				Dim c1 As Integer = Character.codePointAt(input, Character.charCount(c0))
				If getClass(c1) = getClass(c0) Then Return New String() {input}
				Dim result As String() = New String(1){}
				result(0) = input
				Dim sb As New StringBuilder(2)
				sb.appendCodePoint(c1)
				sb.appendCodePoint(c0)
				result(1) = sb.ToString()
				Return result
			End If

			Dim length As Integer = 1
			Dim nCodePoints As Integer = countCodePoints(input)
			For x As Integer = 1 To nCodePoints - 1
				length = length * (x+1)
			Next x

			Dim temp As String() = New String(length - 1){}

			Dim combClass As Integer() = New Integer(nCodePoints - 1){}
			Dim x As Integer=0
			Dim i As Integer=0
			Do While x<nCodePoints
				Dim c As Integer = Character.codePointAt(input, i)
				combClass(x) = getClass(c)
				i += Character.charCount(c)
				x += 1
			Loop

			' For each char, take it out and add the permutations
			' of the remaining chars
			Dim index As Integer = 0
			Dim len As Integer
			' offset maintains the index in code units.
	loop:
	x = 0
	Dim offset As Integer=0
	Do While x<nCodePoints
				len = countChars(input, offset, 1)
				Dim skip As Boolean = False
				For y As Integer = x-1 To 0 Step -1
					If combClass(y) = combClass(x) Then GoTo loop
				Next y
				Dim sb As New StringBuilder(input)
				Dim otherChars As String = sb.delete(offset, offset+len).ToString()
				Dim subResult As String() = producePermutations(otherChars)

				Dim prefix As String = input.Substring(offset, len)
				For y As Integer = 0 To subResult.Length - 1
					temp(index) = prefix + subResult(y)
					index += 1
				Next y
		x += 1
		offset+=len
	Loop
			Dim result As String() = New String(index - 1){}
			For x As Integer = 0 To index - 1
				result(x) = temp(x)
			Next x
			Return result
		End Function

		Private Function getClass(  c As Integer) As Integer
			Return sun.text.Normalizer.getCombiningClass(c)
		End Function

		''' <summary>
		''' Attempts to compose input by combining the first character
		''' with the first combining mark following it. Returns a String
		''' that is the composition of the leading character with its first
		''' combining mark followed by the remaining combining marks. Returns
		''' null if the first two characters cannot be further composed.
		''' </summary>
		Private Function composeOneStep(  input As String) As String
			Dim len As Integer = countChars(input, 0, 2)
			Dim firstTwoCharacters As String = input.Substring(0, len)
			Dim result As String = java.text.Normalizer.normalize(firstTwoCharacters, java.text.Normalizer.Form.NFC)

			If result.Equals(firstTwoCharacters) Then
				Return Nothing
			Else
				Dim remainder As String = input.Substring(len)
				Return result + remainder
			End If
		End Function

		''' <summary>
		''' Preprocess any \Q...\E sequences in `temp', meta-quoting them.
		''' See the description of `quotemeta' in perlfunc(1).
		''' </summary>
		Private Sub RemoveQEQuoting()
			Dim pLen As Integer = patternLength
			Dim i As Integer = 0
			Do While i < pLen-1
				If temp(i) <> AscW("\"c) Then
					i += 1
				ElseIf temp(i + 1) <> AscW("Q"c) Then
					i += 2
				Else
					Exit Do
				End If
			Loop
			If i >= pLen - 1 Then ' No \Q sequence found Return
			Dim j As Integer = i
			i += 2
			Dim newtemp As Integer() = New Integer(j + 3*(pLen-i) + 2 - 1){}
			Array.Copy(temp, 0, newtemp, 0, j)

			Dim inQuote As Boolean = True
			Dim beginQuote As Boolean = True
			Do While i < pLen
				Dim c As Integer = temp(i)
				i += 1
				If (Not ASCII.isAscii(c)) OrElse ASCII.isAlpha(c) Then
					newtemp(j) = c
					j += 1
				ElseIf ASCII.isDigit(c) Then
					If beginQuote Then newtemp(j) = """c; j += 1; newtemp[j++] = "x"c; newtemp[j++] = "3"c; } newtemp[j++] = c; } else if (c != "\"c) { if (inQuote) newtemp[j++] = "\"c; newtemp[j++] = c; } else if (inQuote) { if (temp[i] == "E"c) { i++; inQuote = false; } else { newtemp[j++] = "\"c; newtemp[j++] = "\"c; } } else { if (temp[i] == "Q"c) { i++; inQuote = true; beginQuote = true; continue; } else { newtemp[j++] = c; if (i != pLen) newtemp[j++] = temp[i++]; } } beginQuote = false; } patternLength = j; temp = Arrays.copyOf(newtemp, j + 2); } private  Sub  compile() { if (has(CANON_EQ) && !has(LITERAL)) { normalize(); } else { normalizedPattern = pattern; } patternLength = normalizedPattern.length(); temp = new int[patternLength + 2]; hasSupplementary = false; int c, count = 0; for (int x = 0; x < patternLength; x += Character.charCount(c)) { c = normalizedPattern.codePointAt(x); if (isSupplementary(c)) { hasSupplementary = true; } temp[count++] = c; } patternLength = count; if (! has(LITERAL)) RemoveQEQuoting(); buffer = new int[32]; groupNodes = new GroupHead[10]; namedGroups = null; if (has(LITERAL)) { matchRoot = newSlice(temp, patternLength, hasSupplementary); matchRoot.next = lastAccept; } else { matchRoot = expr(lastAccept); if (patternLength != cursor) { if (peek() == ")"c) { throw error("Unmatched closing ')'"); } else { throw error("Unexpected internal error"); } } } if (matchRoot instanceof Slice) { root = BnM.optimize(matchRoot); if (root == matchRoot) { root = hasSupplementary ? new StartS(matchRoot) : new Start(matchRoot); } } else if (matchRoot instanceof Begin || matchRoot instanceof First) { root = matchRoot; } else { root = hasSupplementary ? new StartS(matchRoot) : new Start(matchRoot); } temp = null; buffer = null; groupNodes = null; patternLength = 0; compiled = true; } Map<String, Integer> namedGroups() { if (namedGroups == null) namedGroups = new HashMap<>(2); return namedGroups; } private static  Sub  printObjectTree(Node node) { while(node != null) { if (node instanceof Prolog) { System.out.println(node); printObjectTree(((Prolog)node).loop); System.out.println("**** end contents prolog loop"); } else if (node instanceof Loop) { System.out.println(node); printObjectTree(((Loop)node).body); System.out.println("**** end contents Loop body"); } else if (node instanceof Curly) { System.out.println(node); printObjectTree(((Curly)node).atom); System.out.println("**** end contents Curly body"); } else if (node instanceof GroupCurly) { System.out.println(node); printObjectTree(((GroupCurly)node).atom); System.out.println("**** end contents GroupCurly body"); } else if (node instanceof GroupTail) { System.out.println(node); System.out.println("Tail next is "+node.next); return; } else { System.out.println(node); } node = node.next; if (node != null) System.out.println("->next:"); if (node == Pattern.accept) { System.out.println("Accept Node"); node = null; } } } static final class TreeInfo { int minLength; int maxLength; boolean maxValid; boolean deterministic; TreeInfo() { reset(); }  Sub  reset() { minLength = 0; maxLength = 0; maxValid = true; deterministic = true; } } private boolean has(int f) { return (flags & f) != 0; } private  Sub  accept(int ch, String s) { int testChar = temp[cursor++]; if (has(COMMENTS)) testChar = parsePastWhitespace(testChar); if (ch != testChar) { throw error(s); } } private  Sub  mark(int c) { temp[patternLength] = c; } private int peek() { int ch = temp[cursor]; if (has(COMMENTS)) ch = peekPastWhitespace(ch); return ch; } private int read() { int ch = temp[cursor++]; if (has(COMMENTS)) ch = parsePastWhitespace(ch); return ch; } private int readEscaped() { int ch = temp[cursor++]; return ch; } private int next() { int ch = temp[++cursor]; if (has(COMMENTS)) ch = peekPastWhitespace(ch); return ch; } private int nextEscaped() { int ch = temp[++cursor]; return ch; } private int peekPastWhitespace(int ch) { while (ASCII.isSpace(ch) || ch == "#"c) { while (ASCII.isSpace(ch)) ch = temp[++cursor]; if (ch == "#"c) { ch = peekPastLine(); } } return ch; } private int parsePastWhitespace(int ch) { while (ASCII.isSpace(ch) || ch == "#"c) { while (ASCII.isSpace(ch)) ch = temp[cursor++]; if (ch == "#"c) ch = parsePastLine(); } return ch; } private int parsePastLine() { int ch = temp[cursor++]; while (ch != 0 && !isLineSeparator(ch)) ch = temp[cursor++]; return ch; } private int peekPastLine() { int ch = temp[++cursor]; while (ch != 0 && !isLineSeparator(ch)) ch = temp[++cursor]; return ch; } private boolean isLineSeparator(int ch) { if (has(UNIX_LINES)) { return ch == ControlChars.Lf; } else { return (ch == ControlChars.Lf || ch == ControlChars.Cr || (ch|1) == ChrW(&H2029) || ch == ChrW(&H0085)); } } private int skip() { int i = cursor; int ch = temp[i+1]; cursor = i + 2; return ch; } private  Sub  unread() { cursor--; } private PatternSyntaxException error(String s) { return new PatternSyntaxException(s, normalizedPattern,  cursor - 1); } private boolean findSupplementary(int start, int end) { for (int i = start; i < end; i++) { if (isSupplementary(temp[i])) return true; } return false; } private static final boolean isSupplementary(int ch) { return ch >= Character.MIN_SUPPLEMENTARY_CODE_POINT || Character.isSurrogate((char)ch); } private Node expr(Node end) { Node prev = null; Node firstTail = null; Branch branch = null; Node branchConn = null; for (;;) { Node node = sequence(end); Node nodeTail = root; if (prev == null) { prev = node; firstTail = nodeTail; } else { if (branchConn == null) { branchConn = new BranchConn(); branchConn.next = end; } if (node == end) { node = null; } else { nodeTail.next = branchConn; } if (prev == branch) { branch.add(node); } else { if (prev == end) { prev = null; } else { firstTail.next = branchConn; } prev = branch = new Branch(prev, node, branchConn); } } if (peek() != "|"c) { return prev; } next(); } } @SuppressWarnings("fallthrough") private Node sequence(Node end) { Node head = null; Node tail = null; Node node = null; LOOP: for (;;) { int ch = peek(); switch (ch) { case "("c: node = group0(); if (node == null) continue; if (head == null) head = node; else tail.next = node; tail = root; continue; case "["c: node = clazz(true); break; case "\"c: ch = nextEscaped(); if (ch == "p"c || ch == "P"c) { boolean oneLetter = true; boolean comp = (ch == "P"c); ch = next(); if (ch != "{"c) { unread(); } else { oneLetter = false; } node = family(oneLetter, comp); } else { unread(); node = atom(); } break; case "^"c: next(); if (has(MULTILINE)) { if (has(UNIX_LINES)) node = new UnixCaret(); else node = new Caret(); } else { node = new Begin(); } break; case "$"c: next(); if (has(UNIX_LINES)) node = new UnixDollar(has(MULTILINE)); else node = new Dollar(has(MULTILINE)); break; case "."c: next(); if (has(DOTALL)) { node = new All(); } else { if (has(UNIX_LINES)) node = new UnixDot(); else { node = new Dot(); } } break; case "|"c: case ")"c: break LOOP; case "]"c: case " ' Now interpreting dangling ] and } as literals -  Consume { if present
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					"c: node = atom(); break; case "?"c: case "*"c: case " & "c: next(); throw error("Dangling meta character
					' Fall through
		''' <summary>
		''' Parse and add a new Single or Slice.
		''' </summary>
					' Unwind meta escape sequence
					' Fall through
		''' <summary>
		''' Parses a backref greedily, taking as many numbers as it
		''' can. The first digit is always treated as a backref, but
		''' multi digit numbers are only treated as a backref if at
		''' least that many backrefs exist at this point in the regex.
		''' </summary>
					' Add another number if it doesn't make a group
					' that doesn't exist
		''' <summary>
		''' Parses an escape sequence to determine the actual value that needs
		''' to be matched.
		''' If -1 is returned and create was true a new object was added to the tree
		''' to handle the escape sequence.
		''' If the returned value is greater than zero, it is the value that
		''' matches the escape sequence.
		''' </summary>
				' '\v' was implemented as VT/0x0B in releases < 1.8 (though
				' undocumented). In JDK8 '\v' is specified as a predefined
				' character class for all vertical whitespace characters.
				' So [-1, root=VertWS node] pair is returned (instead of a
				' single 0x0B). This breaks the range if '\v' is used as
				' the start or end value, such as [\v-...] or [...-\v], in
				' which a single definite value (0x0B) is expected. For
				' compatibility concern '\013'/0x0B is returned if isrange.
		''' <summary>
		''' Parse a character [Class], and return the node that matches it.
		''' 
		''' Consumes a ] on the way out if consume is true. Usually consume
		''' is true except for the case of [abc&&def] where def is a separate
		''' right hand node with "understood" brackets.
		''' </summary>
						' Negates if first char in a [Class], otherwise literal
							' ^ not first in [Class], treat as literal
							' treat as a literal &
	'         Bits can only handle codepoints in [u+0000-u+00ff] range.
	'           Use "single" node instead of bits when dealing with unicode
	'           case folding for codepoints listed below.
	'           (1)Uppercase out of range: u+00ff, u+00b5
	'              toUpperCase(u+00ff) -> u+0178
	'              toUpperCase(u+00b5) -> u+039c
	'           (2)LatinSmallLetterLongS u+17f
	'              toUpperCase(u+017f) -> u+0053
	'           (3)LatinSmallLetterDotlessI u+131
	'              toUpperCase(u+0131) -> u+0049
	'           (4)LatinCapitalLetterIWithDotAbove u+0130
	'              toLowerCase(u+0130) -> u+0069
	'           (5)KelvinSign u+212a
	'              toLowerCase(u+212a) ==> u+006B
	'           (6)AngstromSign u+212b
	'              toLowerCase(u+212b) ==> u+00e5
	'        
		''' <summary>
		''' Parse a single character or a character range in a character class
		''' and return its representative node.
		''' </summary>
					' Consume { if present
		''' <summary>
		''' Parses a Unicode character family and returns its representative node.
		''' </summary>
				' property construct \p{name=value}
					' \p{inBlockName}
					' \p{isGeneralCategory} and \p{isScriptName}
		''' <summary>
		''' Returns a CharProperty matching all characters belong to
		''' a UnicodeScript.
		''' </summary>
		''' <summary>
		''' Returns a CharProperty matching all characters in a UnicodeBlock.
		''' </summary>
		''' <summary>
		''' Returns a CharProperty matching all characters in a named property.
		''' </summary>
		''' <summary>
		''' Parses and returns the name of a "named capturing group", the trailing
		''' ">" is consumed after parsing.
		''' </summary>
		''' <summary>
		''' Parses a group and returns the head node of a set of nodes that process
		''' the group. Sometimes a double return system is used where the tail is
		''' returned in root.
		''' </summary>
						' named captured group
			' Check for quantifiers
				' Discover if the group is deterministic
		''' <summary>
		''' Create group head and tail nodes using double return. If the group is
		''' created with anonymous true then it is a pure group and should not
		''' affect group counting.
		''' </summary>
		''' <summary>
		''' Parses inlined match flags and set them appropriately.
		''' </summary>
		''' <summary>
		''' Parses the second part of inlined match flags and turns off
		''' flags appropriately.
		''' </summary>
		''' <summary>
		''' Processes repetition. If the next character peeked is a quantifier
		''' then new nodes must be appended to handle the repetition.
		''' Prev could be a single or a group, so it could be a chain of nodes.
		''' </summary>
		''' <summary>
		'''  Utility method for parsing control escape sequences.
		''' </summary>
		''' <summary>
		'''  Utility method for parsing octal escape sequences.
		''' </summary>
		''' <summary>
		'''  Utility method for parsing hexadecimal escape sequences.
		''' </summary>
		''' <summary>
		'''  Utility method for parsing unicode escape sequences.
		''' </summary>
		'
		' Utility methods for code point support
		'
			' optimization
		''' <summary>
		'''  Creates a bit vector for matching Latin-1 values. A normal BitClass
		'''  never matches values above Latin-1, and a complemented BitClass always
		'''  matches values above Latin-1.
		''' </summary>
		''' <summary>
		'''  Returns a suitably optimized, single character matcher.
		''' </summary>
		''' <summary>
		'''  Utility method for creating a string slice matcher.
		''' </summary>
		''' <summary>
		''' The following classes are the building components of the object
		''' tree that represents a compiled regular expression. The object tree
		''' is made of individual elements that handle constructs in the Pattern.
		''' Each type of object knows how to match its equivalent construct with
		''' the match() method.
		''' </summary>
		''' <summary>
		''' Base class for all node classes. Subclasses should override the match()
		''' method as appropriate. This class is an accepting node, so its match()
		''' always returns true.
		''' </summary>
			''' <summary>
			''' This method implements the classic accept node.
			''' </summary>
			''' <summary>
			''' This method is good for all zero length assertions.
			''' </summary>
			''' <summary>
			''' This method implements the classic accept node with
			''' the addition of a check to see if the match occurred
			''' using all of the input.
			''' </summary>
		''' <summary>
		''' Used for REs that can start anywhere within the input string.
		''' This basically tries to match repeatedly at each spot in the
		''' input string, moving forward after each try. An anchored search
		''' or a BnM will bypass this node completely.
		''' </summary>
	'    
	'     * StartS supports supplementary characters, including unpaired surrogates.
	'     
					'if ((ret = next.match(matcher, i, seq)) || i == guard)
					' Optimization to move to the next character. This is
					' faster than countChars(seq, i, 1).
		''' <summary>
		''' Node to anchor at the beginning of input. This object implements the
		''' match for a \A sequence, and the caret anchor will use this if not in
		''' multiline mode.
		''' </summary>
		''' <summary>
		''' Node to anchor at the end of input. This is the absolute end, so this
		''' should not match at the last newline before the end as $ will.
		''' </summary>
		''' <summary>
		''' Node to anchor at the beginning of a line. This is essentially the
		''' object to match for the multiline ^.
		''' </summary>
				' Perl does not match ^ at end of input even after newline
					' Should treat /r/n as one newline
		''' <summary>
		''' Node to anchor at the beginning of a line when in unixdot mode.
		''' </summary>
				' Perl does not match ^ at end of input even after newline
		''' <summary>
		''' Node to match the location where the last match ended.
		''' This is used for the \G construct.
		''' </summary>
		''' <summary>
		''' Node to anchor at the end of a line or the end of input based on the
		''' multiline mode.
		''' 
		''' When not in multiline mode, the $ can only match at the very end
		''' of the input, unless the input ends in a line terminator in which
		''' it matches right before the last line terminator.
		''' 
		''' Note that \r\n is considered an atomic line terminator.
		''' 
		''' Like ^ the $ operator matches at a position, it does not match the
		''' line terminators themselves.
		''' </summary>
				' Matches before any line terminator; also matches at the
				' end of input
				' Before line terminator:
				' If multiline, we match here no matter what
				' If not multiline, fall through so that the end
				' is marked as hit; this must be a /r/n or a /n
				' at the very end so the end was hit; more input
				' could make this not match here
						 ' No match between \r\n
				' Matched at current end so hit end
				' If a $ matches because of end of input, then more input
				' could cause it to fail!
		''' <summary>
		''' Node to anchor at the end of a line or the end of input based on the
		''' multiline mode when in unix lines mode.
		''' </summary>
						' If not multiline, then only possible to
						' match at very end or one before end
						' If multiline return next.match without setting
						' matcher.hitEnd
				' Matching because at the end or 1 before the end;
				' more input could change this so set hitEnd
				' If a $ matches because of end of input, then more input
				' could cause it to fail!
		''' <summary>
		''' Node class that matches a Unicode line ending '\R'
		''' </summary>
				' (u+000Du+000A|[u+000Au+000Bu+000Cu+000Du+0085u+2028u+2029])
		''' <summary>
		''' Abstract node class to match one character satisfying some
		''' boolean property.
		''' </summary>
		''' <summary>
		''' Optimized version of CharProperty that works only for
		''' properties never satisfied by Supplementary characters.
		''' </summary>
		''' <summary>
		''' Node class that matches a Supplementary Unicode character
		''' </summary>
		''' <summary>
		''' Optimization -- matches a given BMP character
		''' </summary>
		''' <summary>
		''' Case insensitive matches a given BMP character
		''' </summary>
		''' <summary>
		''' Unicode case insensitive matches a given Unicode character
		''' </summary>
		''' <summary>
		''' Node class that matches a Unicode block.
		''' </summary>
		''' <summary>
		''' Node class that matches a Unicode script
		''' </summary>
		''' <summary>
		''' Node class that matches a Unicode category.
		''' </summary>
		''' <summary>
		''' Node class that matches a Unicode "type"
		''' </summary>
		''' <summary>
		''' Node class that matches a POSIX type.
		''' </summary>
		''' <summary>
		''' Node class that matches a Perl vertical whitespace
		''' </summary>
		''' <summary>
		''' Node class that matches a Perl horizontal whitespace
		''' </summary>
		''' <summary>
		''' Base class for all Slice nodes
		''' </summary>
		''' <summary>
		''' Node class for a case sensitive/BMP-only sequence of literal
		''' characters.
		''' </summary>
		''' <summary>
		''' Node class for a case_insensitive/BMP-only sequence of literal
		''' characters.
		''' </summary>
		''' <summary>
		''' Node class for a unicode_case_insensitive/BMP-only sequence of
		''' literal characters. Uses unicode case folding.
		''' </summary>
		''' <summary>
		''' Node class for a case sensitive sequence of literal characters
		''' including supplementary characters.
		''' </summary>
		''' <summary>
		''' Node class for a case insensitive sequence of literal characters
		''' including supplementary characters.
		''' </summary>
		''' <summary>
		''' Node class for a case insensitive sequence of literal characters.
		''' Uses unicode case folding.
		''' </summary>
		''' <summary>
		''' Returns node for matching characters within an explicit value range.
		''' </summary>
		''' <summary>
		''' Returns node for matching characters within an explicit value
		''' range in a case insensitive manner.
		''' </summary>
		''' <summary>
		''' Implements the Unicode category ALL and the dot metacharacter when
		''' in dotall mode.
		''' </summary>
		''' <summary>
		''' Node class for the dot metacharacter when dotall is not enabled.
		''' </summary>
		''' <summary>
		''' Node class for the dot metacharacter when dotall is not enabled
		''' but UNIX_LINES is enabled.
		''' </summary>
		''' <summary>
		''' The 0 or 1 quantifier. This one class implements all three types.
		''' </summary>
		''' <summary>
		''' Handles the curly-brace style repetition with a specified minimum and
		''' maximum occurrences. The * quantifier is handled as a special case.
		''' This class handles the three types.
		''' </summary>
			' Greedy match.
			' i is the index to start matching at
			' j is the number of atoms that have matched
					' We have matched the maximum... continue with the rest of
					' the regular expression
					' k is the length of this match
					' Move up index and number matched
					' We are greedy so match as many as we can
					' Handle backing off if match fails
			' Reluctant match. At this point, the minimum has been satisfied.
			' i is the index to start matching at
			' j is the number of atoms that have matched
					' Try finishing match without consuming any more
					' At the maximum, no match found
					' Okay, must try one more atom
					' If we haven't moved forward then must break out
					' Move up index and number matched
				' Save original info
		''' <summary>
		''' Handles the curly-brace style repetition with a specified minimum and
		''' maximum occurrences in deterministic cases. This is an iterative
		''' optimization over the Prolog and Loop system which would handle this
		''' in a recursive way. The * quantifier is handled as a special case.
		''' If capture is true then this class saves group settings and ensures
		''' that groups are unset when backing off of a group match.
		''' </summary>
				' Notify GroupTail there is no need to setup group info
				' because it will be set here
			' Aggressive group match
				' don't back off passing the starting "j"
						' backing off
			' Reluctant matching
			' Possessive matching
				' Save original info
		''' <summary>
		''' A Guard node at the end of each atom node in a Branch. It
		''' serves the purpose of chaining the "match" operation to
		''' "next" but not the "study", so we can collect the TreeInfo
		''' of each atom node without including the TreeInfo of the
		''' "next".
		''' </summary>
		''' <summary>
		''' Handles the branching of alternations. Note this is also used for
		''' the ? quantifier to branch between the case where it matches once
		''' and where it does not occur.
		''' </summary>
		''' <summary>
		''' The GroupHead saves the location where the group begins in the locals
		''' and restores them when the match is done.
		''' 
		''' The matchRef is used when a reference to this group is accessed later
		''' in the expression. The locals will have a negative value in them to
		''' indicate that we do not want to unset the group if the reference
		''' doesn't match.
		''' </summary>
		''' <summary>
		''' Recursive reference to a group in the regular expression. It calls
		''' matchRef because if the reference fails to match we would not unset
		''' the group.
		''' </summary>
		''' <summary>
		''' The GroupTail handles the setting of group beginning and ending
		''' locations when groups are successfully matched. It must also be able to
		''' unset groups that have to be backed off of.
		''' 
		''' The GroupTail node is also used when a previous group is referenced,
		''' and in that case no group information needs to be set.
		''' </summary>
					' Save the group so we can unset it if it
					' backs off of a match.
					' This is a group reference case. We don't need to save any
					' group info because it isn't really a group.
		''' <summary>
		''' This sets up a loop to handle a recursive quantifier structure.
		''' </summary>
		''' <summary>
		''' Handles the repetition count for a greedy Curly. The matchInit
		''' is called from the Prolog to save the index of where the group
		''' beginning is stored. A zero length group check occurs in the
		''' normal match but is skipped in the matchInit.
		''' </summary>
				' Avoid infinite loop in zero-length case.
					' This block is for before we reach the minimum
					' iterations required for the loop to match
						' If match failed we must backtrack, so
						' the loop count should NOT be incremented
						' Return success or failure since we are under
						' minimum
					' This block is for after we have the minimum
					' iterations required for the loop to match
						' If match failed we must backtrack, so
						' the loop count should NOT be incremented
		''' <summary>
		''' Handles the repetition count for a reluctant Curly. The matchInit
		''' is called from the Prolog to save the index of where the group
		''' beginning is stored. A zero length group check occurs in the
		''' normal match but is skipped in the matchInit.
		''' </summary>
				' Check for zero length group
						' If match failed we must backtrack, so
						' the loop count should NOT be incremented
						' If match failed we must backtrack, so
						' the loop count should NOT be incremented
		''' <summary>
		''' Refers to a group in the regular expression. Attempts to match
		''' whatever the group referred to last matched.
		''' </summary>
				' If the referenced group didn't match, neither can this
				' If there isn't enough input left no match
				' Check each new char to make sure it matches what the group
				' referenced matched last time around
				' If the referenced group didn't match, neither can this
				' If there isn't enough input left no match
				' Check each new char to make sure it matches what the group
				' referenced matched last time around
		''' <summary>
		''' Searches until the next instance of its atom. This is useful for
		''' finding the atom efficiently without passing an instance of it
		''' (greedy problem) and without a lot of wasted search time (reluctant
		''' problem).
		''' </summary>
		''' <summary>
		''' Zero width positive lookahead.
		''' </summary>
				' Relax transparent region boundaries for lookahead
					' Reinstate region boundaries
		''' <summary>
		''' Zero width negative lookahead.
		''' </summary>
				' Relax transparent region boundaries for lookahead
						' If a negative lookahead succeeds then more input
						' could cause it to fail!
					' Reinstate region boundaries
		''' <summary>
		''' For use with lookbehinds; matches the position where the lookbehind
		''' was encountered.
		''' </summary>
		''' <summary>
		''' Zero width positive lookbehind.
		''' </summary>
				' Set end boundary
				' Relax transparent region boundaries for lookbehind
		''' <summary>
		''' Zero width positive lookbehind, including supplementary
		''' characters or unpaired surrogates.
		''' </summary>
				' Set end boundary
				' Relax transparent region boundaries for lookbehind
		''' <summary>
		''' Zero width negative lookbehind.
		''' </summary>
				' Relax transparent region boundaries for lookbehind
				' Reinstate region boundaries
		''' <summary>
		''' Zero width negative lookbehind, including supplementary
		''' characters or unpaired surrogates.
		''' </summary>
				' Relax transparent region boundaries for lookbehind
				'Reinstate region boundaries
		''' <summary>
		''' Returns the set union of two CharProperty nodes.
		''' </summary>
		''' <summary>
		''' Returns the set intersection of two CharProperty nodes.
		''' </summary>
		''' <summary>
		''' Returns the set difference of two CharProperty nodes.
		''' </summary>
		''' <summary>
		''' Handles word boundaries. Includes a field to allow this one class to
		''' deal with the different types of word boundaries we can match. The word
		''' characters include underscores, letters, and digits. Non spacing marks
		''' can are also part of a word if they have a base character, otherwise
		''' they are ignored for purposes of finding word boundaries.
		''' </summary>
					' Tried to access char past the end
					' The addition of another char could wreck a boundary
		''' <summary>
		''' Non spacing marks only count as word characters in bounds calculations
		''' if they have a base character.
		''' </summary>
		''' <summary>
		''' Attempts to match a slice in the input using the Boyer-Moore string
		''' matching algorithm. The algorithm is based on the idea that the
		''' pattern can be shifted farther ahead in the search text if it is
		''' matched right to left.
		''' <p>
		''' The pattern is compared to the input one character at a time, from
		''' the rightmost character in the pattern to the left. If the characters
		''' all match the pattern has been found. If a character does not match,
		''' the pattern is shifted right a distance that is the maximum of two
		''' functions, the bad character shift and the good suffix shift. This
		''' shift moves the attempted match position through the input more
		''' quickly than a naive one position at a time check.
		''' <p>
		''' The bad character shift is based on the character from the text that
		''' did not match. If the character does not appear in the pattern, the
		''' pattern can be shifted completely beyond the bad character. If the
		''' character does occur in the pattern, the pattern can be shifted to
		''' line the pattern up with the next occurrence of that character.
		''' <p>
		''' The good suffix shift is based on the idea that some subset on the right
		''' side of the pattern has matched. When a bad character is found, the
		''' pattern can be shifted right by the pattern length if the subset does
		''' not occur again in pattern, or by the amount of distance to the
		''' next occurrence of the subset in the pattern.
		''' 
		''' Boyer-Moore search methods adapted from code by Amy Yu.
		''' </summary>
			''' <summary>
			''' Pre calculates arrays needed to generate the bad character
			''' shift and the good suffix shift. Only the last seven bits
			''' are used to see if chars match; This keeps the tables small
			''' and covers the heavily used ASCII range, but occasionally
			''' results in an aliased match for the bad character shift.
			''' </summary>
				' The BM algorithm requires a bit of overhead;
				' If the pattern is short don't use it, since
				' a shift larger than the pattern length cannot
				' be used anyway.
				' Precalculate part of the bad character shift
				' It is a table for where in the pattern each
				' lower 7-bit value occurs
				' Precalculate the good suffix shift
				' i is the shift amount being considered
					' j is the beginning index of suffix being considered
						' Testing for good suffix
							' src[j..len] is a good suffix
							' No match. The array has already been
							' filled up with correct values before.
					' This fills up the remaining of optoSft
					' any suffix can not have larger shift amount
					' then its sub-suffix. Why???
				' Set the guard value because of unicode compression
				' Loop over all possible match positions in text
					' Loop over pattern from right to left
							' Shift search to the right by the maximum of the
							' bad character shift and the good suffix shift
					' Entire pattern matched starting at i
				' BnM is only used as the leading node in the unanchored case,
				' and it replaced its Start() which always searches to the end
				' if it doesn't find what it's looking for, so hitEnd is true.
		''' <summary>
		''' Supplementary support version of BnM(). Unpaired surrogates are
		''' also handled by this class.
		''' </summary>
				' Loop over all possible match positions in text
					' Loop over pattern from right to left
							' Shift search to the right by the maximum of the
							' bad character shift and the good suffix shift
					' Entire pattern matched starting at i
	'/////////////////////////////////////////////////////////////////////////////
	'/////////////////////////////////////////////////////////////////////////////
		''' <summary>
		'''  This must be the very first initializer.
		''' </summary>
				' Unicode character property aliases, defined in
				' http://www.unicode.org/Public/UNIDATA/PropertyValueAliases.txt
				' Posix regular expression character classes, defined in
				' http://www.unix.org/onlinepubs/009695399/basedefs/xbd_chap09.html
				' Java character properties, defined by methods in Character.java
		''' <summary>
		''' Creates a predicate which can be used to match a string.
		''' </summary>
		''' <returns>  The predicate which can be used for matching on a string
		''' @since   1.8 </returns>
		''' <summary>
		''' Creates a stream from the given input sequence around matches of this
		''' pattern.
		''' 
		''' <p> The stream returned by this method contains each substring of the
		''' input sequence that is terminated by another subsequence that matches
		''' this pattern or is terminated by the end of the input sequence.  The
		''' substrings in the stream are in the order in which they occur in the
		''' input. Trailing empty strings will be discarded and not encountered in
		''' the stream.
		''' 
		''' <p> If this pattern does not match any subsequence of the input then
		''' the resulting stream has just one element, namely the input sequence in
		''' string form.
		''' 
		''' <p> When there is a positive-width match at the beginning of the input
		''' sequence then an empty leading substring is included at the beginning
		''' of the stream. A zero-width match at the beginning however never produces
		''' such empty leading substring.
		''' 
		''' <p> If the input sequence is mutable, it must remain constant during the
		''' execution of the terminal stream operation.  Otherwise, the result of the
		''' terminal stream operation is undefined.
		''' </summary>
		''' <param name="input">
		'''          The character sequence to be split
		''' </param>
		''' <returns>  The stream of strings computed by splitting the input
		'''          around matches of this pattern </returns>
		''' <seealso cref=     #split(CharSequence)
		''' @since   1.8 </seealso>
				' The start position of the next sub-sequence of input
				' when current == input.length there are no more elements
				' null if the next element, if any, needs to obtained
				' > 0 if there are N next empty elements
					' Consume the next matching element
					' Count sequence of matching empty elements
												  ' match at the beginning of the input
					' Consume last matching element
						' Ignore a terminal sequence of matching empty elements

End Namespace
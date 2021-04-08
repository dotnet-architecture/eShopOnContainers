@if (@X)==(@Y) @end /* Harmless hybrid line that begins a JScript comment
@goto :Batch

::JREPL.BAT by Dave Benham
::/History
::
::    2018-10-20 v7.15: Add a string literal syntax to the /INC and /EXC options.
::    2018-10-15 v7.14: Bug fix - User defined variables declared in /JBEG named
::                      str and/or obj were getting clobbered.
::                      Bug fix - Internal variable xbytes was visibile to user
::                      supplied JScript.
::    2018-07-19 v7.13: Bug fix - /INC and /EXC regex failed to match any
::                      line that immediately followed a prior block.
::                      Added ADO code to create XBYTES.DAT in case CERTUTIL is
::                      missing.
::    2018-07-18 v7.12: Fixed XBYTES.DAT creation cleanup bugs, and improved docs
::    2018-03-26 v7.11: Add support for /O "-|UTF-?|NB" (overwrite without BOM)
::    2018-03-14 v7.10: Now can block BOM in ADO output files by appending |NB
::                      to |CharSet in the /O option and OpenOutput() function.
::    2017-11-23 v7.9: Allow escape sequences with /T "" coupled with /XSEQ
::                     Added /PREPL option to augment /P behavior
::                     Bug fix - Force /L when /T "" used, as per documentation
::                     Bug fix - Allow /?charset/search to include non alpha
::    2017-11-13 v7.8: Added \x{nn-mm} and \x{nn-mm,CharSet} escape sequences
::                     Split /X into /XFILE and /XSEQ - /X implies both
::                     Add :FILE syntax for /K and /R to load searches from file
::                     Fixed /XSEQ escaped backslash bug with /INC, /EXC, AND /P
::    2017-10-24 v7.7: Fixed broken Microsoft documentation links
::                     Allow /O "-|CharSet"
::                     Fixed decode(Str[,CharSet]) bug when CharSet is undefined
::    2017-10-08 v7.6: Fixed /?Intro syntax help for /?Charset/[Query]
::    2017-10-08 v7.5: Added /?CHARSET and /?XREGEXP web page help options
::                     Added /?CHARSET/[query] List character sets help option
::                     Fixed ADO output.WriteLine() to use \r\n instead of \n
::                     Improved documentation: /EXC, /OFF, /U, /?HELP, decode()
::    2017-09-25 v7.4: Modified /X \xnn extended ASCII escape sequence to support
::                     any single byte character set.
::                     Added /X \x{nn,Charset} escape sequence.
::                     Added /XBYTES and /XBYTESOFF options.
::                     Modified decode() to support the new /X \xnn behavior.
::    2017-09-23 v7.3: Fixed /O - support for ADO input.
::    2017-09-23 v7.2: Improved documentation of new 7.0 features.
::                     Bug fix - /T FILE ADO support was broken
::    2017-09-08 v7.1: Bug fix - v7.0 failed if Find or Replace contained )
::    2017-09-08 v7.0: Added /XREG and /TFLAG for XRegExp regex support.
::                     Added /UTF for UTF-16LE support.
::                     Added /X support for the \u{N} unicode escape sequence.
::                     Added |CharSet syntax for file names to allow reading
::                     and writing via ADO with a specified character set.
::                     Exposed the fso FileSystemObject to user JScript.
::                     Augmented openOutput for Unicode and ADO support.
::    2017-08-25 v6.8: Added /X support for the \c caret escape sequence.
::                     Added /APP - append to the output file.
::                     Added the openOutput(file[,appendBoolean]) function.
::    2017-04-09 v6.7: Corrected /OFF /EXC & /INC documentation + spelling fixes.
::    2016-12-23 v6.6: Help correction: Fixed return codes in /?RETURN section.
::    2016-11-13 v6.5: Modify /X to consistently preserve extended ASCII.
::                     New option /RTN writes result to a variable.
::    2016-11-01 v6.4: Bug fix - v6.3 had inverted /EXC result.
::    2016-10-13 v6.3: Improved performance by dynamically generating main loop
::                     code based on chosen options.
::    2016-10-13 v6.2: Bug fix - /J, /JQ, /JMATCH, /JMATCHQ did not work with /P.
::    2016-10-08 v6.1: Bug fix - v6.0 broke /JBEG and /JLIB, all fixed.
::    2016-10-08 v6.0: Added /K - search and write matching lines.
::                     Added /R - search and write non-matching lines.
::                     Added /MATCH - search and write each match on a new line.
::                     Added /P - Pre-filter regex before normal search/replace.
::                     Added /PFLAG - set search flags for /P regex
::                     Added /JQ and /JMATCHQ as Quick forms of /J and /JMATCH.
::                     Augmented /INC and /EXC so can now specify lines by regex.
::                     Changed behavior - /V now applies to /INC and /EXC.
::                     Improved performance of /INC, /EXC, /T, /JBEGLN, /JENDLN.
::                     Added HISTORY and UPDATE topics to the help system.
::    2016-09-27 v5.2: Bug fix - Search & Replace now ignore /V if /T FILE used.
::                     Added a /T FILE example to the documentation.
::    2016-09-20 v5.1: Added the FILE alternative for the /T option.
::    2016-09-18 v5.0: Added the /U option for Unix line terminators of /n.
::    2016-08-04 v4.6: Fixed the /N documentation (repaired missing line)
::    2016-08-03 v4.5: Added /D option to specify delimiter for /N and /OFF.
::    2016-08-02 v4.4: Bug fix - /C count was wrong when last line did not end
::                     with new line. This also affected /INC and /EXC.
::    2016-07-30 v4.3: Added rpad() function and improved lpad()
::    2016-06-24 v4.2: Improved the /?Options help.
::    2016-06-23 v4.1: Added /T option examples to the help.
::                     Added ability to request help on a single option or topic.
::    2016-06-19 v4.0: Added the /INC and /EXC options.
::    2016-03-27 v3.8: Bug fix - Hide leaked global variables i, lib, libs, rtn2.
::                     Bug fix - Work around %~f0 bug when command is quoted.
::                     Bug fix - Use /OPTIONS instead of OPTIONS as a variable
::                     name within the option parser so that it is unlikely to
::                     collide with a user defined variable name.
::    2016-01-14 v3.7: Reworked error handling a bit.
::                     Bug fix - \xnn and \unnnn could fail in a regex search
::                     if result was a meta-character and /X option was used.
::    2015-07-15 v3.6: Added /?? option for paged help.
::    2015-06-12 v3.5: Bug fix for $n or $nn in replace string when /T is
::                     used without /J or /JMATCH or /L
::    2015-01-22 v3.4: Bug fix - Use /TEST instead of TEST as a variable name
::                     within the option parser so that it is unlikely to
::                     collide with a user defined variable name.
::    2014-12-24 v3.3: Bug fix for when /JMATCH is combined with /M or /S
::    2014-12-09 v3.2: Bug fix for /T without /JMATCH - fixed dynamic repl func
::                     Added GOTO at top for improved startup performance
::    2014-11-25 v3.1: Added /JLIB option
::                     Exception handler reports when regex is bad
::                     Fix /X bug with extended ASCII
::    2014-11-23 v3.0: Added /JBEGLN and /JENDLN options
::                     Added skip, quit, and lpad() global variables/functions
::                     Exception handler reports when error in user code
::    2014-11-21 v2.2: Bug fix for /T with /L option.
::    2014-11-20 v2.1: Bug fix for /T option when match is an empty string
::    2014-11-17 v2.0: Added /T (translate) and /C (count input lines) options
::    2014-11-14 v1.0: Initial release derived from REPL.BAT v6.2
::/
::============ Documentation ===========
::/INTRO
:::
:::JREPL  Search  Replace  [/Option  [Value]]...
:::JREPL  /?[?][Topic|/Option|CHARSET/[Query]|HELP]
:::
:::  Performs a global regular expression search and replace operation on
:::  each line of ASCII input from stdin and prints the result to stdout.
:::
:::  Each parameter may be optionally enclosed by double quotes. The double
:::  quotes are not considered part of the argument. The quotes are required
:::  if the parameter contains a batch token delimiter like space, tab, comma,
:::  semicolon. The quotes should also be used if the argument contains a
:::  batch special character like &, |, etc. so that the special character
:::  does not need to be escaped with ^.
:::
:::  Search  - By default, this is a case sensitive JScript (ECMA) regular
:::            expression expressed as a string.
:::
:::            JScript regex syntax documentation is available at
:::            https://msdn.microsoft.com/en-us/library/ae5bf541.aspx
:::
:::  Replace - By default, this is the string to be used as a replacement for
:::            each found search expression. Full support is provided for
:::            substitution patterns available to the JScript replace method.
:::
:::            For example, $& represents the portion of the source that matched
:::            the entire search pattern, $1 represents the first captured
:::            submatch, $2 the second captured submatch, etc. A $ literal
:::            can be escaped as $$.
:::
:::            An empty replacement string must be represented as "".
:::
:::            Replace substitution pattern syntax is fully documented at
:::            https://msdn.microsoft.com/en-US/library/efy6s3e6.aspx
:::
:::  The meaning of extended ASCII byte codes >= 128 (0x80) is dependent on the
:::  active code page. Extended ASCII within arguments and variables requires
:::  the /XFILE option. Binary input with NULL bytes requires the /M option.
::/OPTIONS
:::
:::  Options:  Behavior may be altered by appending one or more options.
:::  The option names are case insensitive, and may appear in any order
:::  after the Replace argument.
:::
::      /A                     - write Altered lines only
::      /APP                   - Append results to the output file
::      /B                     - match Beginning of line
::      /C                     - Count number of source lines
::      /D                     - Delimiter for /N and /OFF
::      /E                     - match End of line
::      /EXC BlockList         - EXClude lines from selected blocks
::      /F InFile[|CharSet[NB]]- read input from a File
::      /I                     - Ignore case
::      /INC BlockList         - INClude lines from selected blocks
::      /J                     - JScript replace expressions
::      /JBEG InitCode         - initialization JScript code
::      /JBEGLN NewLineCode    - line initialization JScript code
::      /JEND FinalCode        - finalization JScript code
::      /JENDLN EndLineCode    - line finalization JScript code
::      /JLIB FileList         - load file(s) of initialization code
::      /JMATCH                - write matching JScript replacements only
::      /JMATCHQ               - new Quick form of /JMATCH
::      /JQ                    - new Quick form of /J
::      /K Context or Pre:Post - search and Keep lines that match
::      /L                     - Literal search
::      /M                     - Multi-line mode
::      /MATCH                 - Search and print each match, one per line
::      /N MinWidth            - prefix output with liNe numbers
::      /O OutFile[|CharSet[|NB]] - write Output to a file
::      /OFF MinWidth          - add char OFFsets to /K, /JMATCHQ, /MATCH output
::      /P Regex               - only search/replace strings that match a Regex
::      /PFLAG Flags           - set the /P regex Flags to "g", "gi", "", or "i"
::      /PREPL FilterReplCode  - selectively Search/Replace captured /P groups
::      /R Context or Pre:Post - search and Reject lines that match
::      /RTN ReturnVar[:Line#] - Return result in a variable
::      /S VarName             - Source is read from a variable
::      /T DelimChar or FILE   - Translate multiple search/replace pairs
::      /TFLAG Flags           - Specify XRegExp flags for use with /T
::      /U                     - Unix line terminators (\n instead of \r\n)
::      /UTF                   - All input and output as UTF-16LE (BOM optional)
::      /V                     - use Variables for Search/Replace and code
::      /X                     - shorthand for combined /XFILE and /XSEQ
::      /XBYTES                - force creation of new XBYTES.DAT
::      /XBYTESOFF             - force /XSEQ \xnn to be treated as Windows-1252
::      /XFILE                 - preserve extended ASCII in args via temp files
::      /XREG FileList         - adds XRegExp support to JREPL
::      /XSEQ                  - enable extended escape sequences
::/
:::      /A  - Only write altered lines. Unaltered lines are discarded.
:::            If the /S option is used, then write the result only if
:::            there was a change anywhere in the string. The /A option
:::            is incompatible with the /M option unless the /S option
:::            is also present.
:::
:::      /APP - Modify /O behavior to Append results to the output file.
:::
:::      /B  - The Search must match the Beginning of a line.
:::            Mostly used with literal searches.
:::
:::      /C  - Count the number of input lines and store the result in global
:::            variable cnt. This value can be useful in JScript code associated
:::            with any of the /Jxxx options.
:::
:::            This option is implicitly enabled if /INC or /EXC contains a
:::            negative value.
:::
:::            This option is incompatible with the /M and /S options.
:::
:::            If the input data is piped or redirected, then the data is first
:::            written to a temporary file, so processing does not start until
:::            the end-of-file is reached.
:::
:::      /D Delimiter
:::
:::            Specifies the Delimiter string to use after line numbers and/or
:::            byte offsets that are output due to the /N or /OFF options.
:::            The default value is a colon. The delimiter may be set to an
:::            empty string by using /D "".
:::
:::      /E  - The Search must match the End of a line.
:::            Mostly used with literal searches.
:::
:::      /EXC BlockList
:::
:::            Exclude (do not search/replace) lines that appear within at least
:::            one block within BlockList. A block may be a single line, or a
:::            contiguous range of lines between a start and end line. The /EXC
:::            option is incompatible with /M and /S.
:::
:::            The syntax for specifying a BlockList is complex. Whitespace
:::            should not appear anywhere except for possibly within a Regex or
:::            String.
:::
:::              BlockList     = {Block}[,{Block}]...
:::              {Block}       = {SingleLine}|{LineRange}
:::              {SingleLine}  = {LineSpec}[{Offset}]
:::              {LineRange}   = {LineSpec}[{Offset}]:{EndLineSpec}[{Offset}]
:::              {LineSpec}    = [-]LineNumber|{Regex}[/]|{String}[/]
:::              {EndLineSpec} = [-]LineNumber|+Number|{Regex}|{String}
:::              {Regex}       = /Regex/[i|b|e]...
:::              {String}      = 'String'[i|b|e]...
:::              {Offset}      = +Number|-Number
:::
:::            A line may be identified by its LineNumber, or by a Regex that
:::            matches the line, or a String literal that is found in the line.
:::            Once identified, a line position may be adjusted forward or
:::            backward via the optional Offset.
:::
:::            A negative LineNumber is counted from the end of the file.
:::            So 1 is the first line of input, and -1 is the last line. The /C
:::            option is automatically activated if any block identifies a line
:::            via a negative line number.
:::
:::            A Block Regex uses mostly standard JScript syntax. Both a Regex
:::            and a String may use any of the escape sequences defined by the
:::            /XSEQ option, even if the /XSEQ option has not been set. Any /
:::            literal within a Regex must be escaped as \/. Any ' literal
:::            within a String must be escaped as ''.
:::
:::            A Regex or String may be followed by any combination of the
:::            following flags:
:::              i - Ignore case
:::              b - The search must match the beginning of a line
:::              e - The search must match the end of a line
:::            /EXC and /INC ignore the /I option.
:::
:::            A line or range start that is identified by a Regex or String may
:::            match multiple lines within the input. Only the first matching
:::            line is used if the Regex or String is terminated by an extra /.
:::
:::            A Line block or Range start that is identified by a Regex or
:::            String cannot have a negative Offset.
:::
:::            If the end of a Range is specified as + followed by a Number,
:::            then the Number is treated as an offset from the start of the
:::            Range (after any start Offset has been applied).
:::
:::            If the end of a Range is specified as a Regex or String, then the
:::            block end is the first line that matches after the beginning of
:::            the block. The extra / cannot be used with an end of Range Regex
:::            or String. The Offset must be greater than or equal to -1 if a 
:::            Regex or String is used. If the end of Range Regex or String is
:::            not found, then the block continues to the end of file.
:::
:::            Examples:
:::
:::              /EXC "1:5,10,-5:-1"
:::                 Exclude the first 5, 10th, and last 5 lines.
:::
:::              /EXC "/^:/"
:::                 Exclude all lines that begin with a colon
:::
:::              /EXC "/^Begin$/+1:/^End$/-1"
:::                 Exclude all lines that are after a "Begin" line, and before
:::                 the next "End" line. Multiple blocks may be excluded.
:::
:::              /EXC "/^DATA/i/:+10"
:::                 Exclude the first line that begins with DATA, ignoring case,
:::                 and exclude the next 10 lines as well.
:::
:::              /EXC "'[START]':'[STOP]'"
:::                 Exclude lines beginning with a line that contains the literal
:::                 [START] and ending with the next line that contains [STOP].
:::
:::              /EXC "'[START]'be:'[STOP]'be"
:::                 Exclude lines beginning with a [START] line (exact match)
:::                 and ending with the next [STOP] line (exact match).
:::
:::      /F InFile[|CharSet|[NB]]
:::
:::            Input is read from file InFile instead of stdin.
:::
:::            If |CharSet (internet character set name) is appended to InFile,
:::            then the file is opened via ADO using the specified CharSet value.
:::            JREPL still recognizes both \n and \r\n as input line terminators
:::            when using ADO. Both ADO and the CharSet must be available on the
:::            local system. Appending |NB to the |CharSet normally has no impact.
:::            The |NB No BOM flag is only useful when combined with /O -.     
:::
:::      /I  - Ignore case when searching.
:::
:::      /INC BlockList
:::
:::            Only Include (search/replace) lines that appear within at least
:::            one block within BlockList. A block may be a single line, or a
:::            contiguous range of lines between a start and end line. The /INC
:::            option is incompatible with /M and /S.
:::
:::            A line within an /INC block is not included if it also appears
:::            within an /EXC block.
:::
:::            See the /EXC help for the syntax of a BlockList.
:::
:::            Examples:
:::
:::              /INC "1:5,10,-5:-1"
:::                 Include the first 5, 10th, and last 5 lines.
:::
:::              /INC "/^:/"
:::                 Include all lines that begin with a colon
:::
:::              /INC "/^Begin$/+1:/^End$/-1"
:::                 Include all lines that are after a "Begin" line, and before
:::                 the next "End" line. Multiple blocks may be included.
:::
:::              /INC "/^DATA/i/:+10"
:::                 Include the first line that begins with DATA, ignoring case,
:::                 and include the next 10 lines as well.
:::
:::              /INC "'[START]':'[STOP]'"
:::                 Include lines starting with a line that contains the literal
:::                 "[START]" and ending with the next line that contains "[STOP]".
:::
:::              /INC "'[START]'be:'[STOP]'be"
:::                 Include lines beginning with a "[START]" line (exact match)
:::                 and ending with the next "[STOP]" line (exact match).
:::
:::      /J  - A deprecated form of /JQ that is slow because the JScript code
:::            is executed via the eval() function each and every match. This
:::            form does not use $txt - The replace value is taken as the value
:::            of the last JScript expression within Replace.
:::
:::            This option is only preserved so as not to break existing scripts.
:::
:::      /JBEG InitCode
:::
:::            JScript inititialization code to run prior to loading any input.
:::            This is useful for initializing user defined variables for
:::            accumulating information across matches. The default code is an
:::            empty string.
:::
:::      /JBEGLN NewLineCode
:::
:::            JScript code to run at the beginning of each line, prior to
:::            performing any search on the line. The line content may be
:::            manipulated via the $txt variable. The default code is an empty
:::            string. This option is incompatible with the /M and /S options.
:::
:::      /JEND FinalCode
:::
:::            JScript termination code to run when there is no more input to
:::            read. This is useful for writing summarization results.
:::            The default code is an empty string.
:::
:::      /JENDLN EndLineCode
:::
:::            JScript code to run at the end of each line, after all matches
:::            on the line have been found, but before the result is printed.
:::            The final result can be modified via the $txt variable. Setting
:::            $txt to false discards the line without printing. The $txt value
:::            is ignored if the /JMATCH option has been used. The default
:::            code is an empty string. This option is incompatible with the
:::            /M and /S options.
:::
:::      /JLIB FileList
:::
:::            Specifies one or more files that contain libraries of JScript
:::            code to load before /JBEG is run. Multiple files are delimited
:::            by forward slashes (/). Useful for declaring global variables
:::            and functions in a way that is reusable.
:::
:::      /JMATCH - A deprecated form of /JMATCHQ that is slow because the JScript
:::            code is executed via the eval() function each and every match.
:::            This form does not use $txt - The replace value is taken as the
:::            value of the last JScript expression within Replace.
:::
:::            This option is only preserved so as not to break existing scripts.
:::
:::      /JMATCHQ - Write each Replace value on a new line, discarding all text
:::            that does not match the Search. The Replace argument is one or
:::            more JScript statements with access to the same $ variables
:::            available to the /JQ option. The code must store the final replace
:::            value in variable $txt. A $txt value of false indicates the match
:::            is to be ignored.
:::
:::            Note the trailing Q stands for Quick :-)
:::
:::      /JQ - The Replace argument is one or more JScript statements that
:::            define the replacement value, and possibly do more. The code
:::            must store the final replace value in variable $txt.
:::
:::            The following variables contain details about each match:
:::
:::              $0 is the substring that matched the Search
:::              $1 through $n are the captured submatch strings
:::              $off is the offset where the match occurred
:::              $src is the original source string
:::
:::            Note the trailing Q stands for Quick :-)
:::
:::      /K PreContext:PostContext[:FILE]
:::      /K Context[:FILE]
:::
:::            Keep matches - Search and write out lines that contain at least
:::            one match, without doing any replacement. The Replace argument is
:::            still required, but is ignored.
:::
:::            The integers PreContext and PostContext specify how many non-
:::            matching lines to write before the match, and after the match,
:::            respectively. If a single Context integer is given, then the same
:::            number of non-matching lines are written before and after.
:::            A Context of 0 writes only matching lines.
:::
:::            If :FILE is appended to the context, then the Search parameter
:::            specifies a file containing one or more search terms, one term
:::            per line. A line matches if any of the search terms are found
:::            witin the line. The file can be opened via ADO if |CharSet
:::            (internet character set name) is appended to the file name.
:::            Note: the /V option does not apply to Search if /K :FILE is used.
:::
:::            /K is incompatible with /A, /J, /JQ, /JMATCH, /JMATCHQ, /M,
:::            /MATCH, /R, /S, and /T.
:::
:::      /L  - The Search is treated as a string literal instead of a
:::            regular expression. Also, all $ found in the Replace string
:::            are treated as $ literals.
:::
:::      /M  - Multi-line mode. The entire input is read and processed in one
:::            pass instead of line by line, thus enabling search for \n. This
:::            also enables preservation of the original line terminators.
:::            The /M option is incompatible with the /A option unless the /S
:::            option is also present.
:::
:::            Note: If working with binary data containing NULL bytes,
:::                  then the /M option must be used.
:::
:::      /MATCH - Search and write out each matching string on a new line,
:::            discarding any non-matching text. The Replace argument is
:::            ignored, but is still required.
:::
:::            /MATCH is incompatible with /A, /J, /JQ, /JMATCH, /JMATCHQ,
:::            /R and /T.
:::
:::      /N MinWidth
:::
:::            Precede each output line with the line number of the source line,
:::            followed by a delimiter (colon by default). The default delimiter
:::            can be overridden with the /D option.
:::
:::            Line 1 is the first line of the source.
:::
:::            The MinWidth value specifies the minimum number of digits to
:::            display. The default value is 0, meaning do not display the
:::            line number. A value of 1 displays the line numbers without any
:::            zero padding.
:::
:::            The /N option is ignored if the /M or /S option is used.
:::
:::      /O OutFile[|CharSet[|NB]]
:::
:::            Output is written to file OutFile instead of stdout. Any existing
:::            OutFile is overwritten unless the /APP option is also used.
:::
:::            If |CharSet (internet character set name) is appended to OutFile,
:::            then the file is opened via ADO using the specified CharSet value.
:::            The output line terminator still defaults to \r\n when using ADO,
:::            and may be changed to \n with the \U option. Both ADO and the
:::            CharSet must be available on the local system. Unicode files
:::            written by ADO have a BOM by default. Appending |NB (or |anyvalue)
:::            to the CharSet blocks the BOM from being written.
:::
:::            If /F InFile is also used, then an OutFile value of "-" overwrites
:::            the original InFile with the output. A value of "-" preserves the
:::            original input character set (and also any |NB No BOM indicator).
:::            A value of "-|" explicitly transforms the file into the machine
:::            default character set. A "-|CharSet[|NB]" value explicitly
:::            transforms the file into the specified character set. The output
:::            is first written to a temporary file with the same path and name,
:::            with .new appended. Upon completion, the temp file is moved to
:::            replace the InFile.
:::
:::            It is rarely useful, but /APP may be combined with /O -. But /APP
:::            cannot be combined with /O "-|CharSet".
:::
:::      /OFF MinWidth
:::
:::            Ignored unless /JMATCHQ, /JMATCH, /MATCH, or /K is used.
:::            Precede each line of output with the offset of the match within
:::            the original source string, followed by a delimiter (colon by
:::            default). The default delimiter can be overridden with the /D
:::            option. The offset follows the line number if the /N option is
:::            also used.
:::
:::            Offset 0 is the first character of the source string. The source
:::            string is normally the current line. But if the /M option is used
:::            then the source string is the entire file.
:::
:::            The MinWidth value specifies the minimum number of digits to
:::            display. The default value is 0, meaning do not display the
:::            offset. A value of 1 displays the offsets without any zero
:::            padding.
:::
:::      /P FilterRegex
:::
:::            Only Search/Replace strings that match the Pre-filter regular
:::            expression FilterRegex. All escape sequences defined by /XSEQ are
:::            available to FilterRegex, even if /XSEQ has not been set.
:::
:::            FilterRegex is a global, case sensitive search by default.
:::            The behavior may be changed via the /PFLAG option.
:::
:::            By default, /P passes the entire matched filter string to the
:::            main Search/Replace routine. If your FilterRegex includes captured
:::            groups, then you can add the /PREPL option to selectively pass one
:::            or more captured groups instead.
:::
:::            The /P option ignores /I, but honors /M.
:::
:::            The /P option may be combined with /INC and/or /EXC, in which case
:::            /P is applied after lines have been included and/or excluded.
:::
:::            From the standpoint of the main "Search" argument, ^ matches the
:::            beginning of the matched filter, and $ matches the end of the
:::            matched filter.
:::
:::            Example - Substitute X for each character within curly braces,
:::                      including the braces.
:::
:::               echo abc{xyz}def|jrepl . X /p "{.*?}"
:::
:::            result:
:::
:::               abcXXXXXdef
:::
:::            See /PREPL for an example showing how to preserve the enclosing
:::            braces.
:::
:::      /PFLAG Flags
:::
:::            Set the search flags to be used when defining the /P FilterRegex.
:::            Possible values are:
:::              "g"  - global, case sensitive (default)
:::              "gi" - global, ignore case
:::              ""   - first match only, case sensitive
:::              "i"  - first match only, ignore case
:::
:::            If the search is not global, then the first match of each line
:::            is used. If the /M option is used, then a non-global search uses
:::            only the first match of the entire input.
:::
:::            Note that the /P FilterRegex multiline mode is contolled by the
:::            /M option. The "m" flag cannot be used with /PFLAG.
:::
:::      /PREPL FilterReplaceCode
:::
:::            Specify a JScript expression FilterReplaceCode that controls
:::            what portion of the /P Pre-filter match is passed on to the main
:::            Search/Replace routine, and what portion is preserved as-is.
:::
:::            The expression is mostly standard JScript, and should evaluate to
:::            a string value. $0 is the entire Pre-filter match, and $1 through
:::            $N are the captured groups. The only non-standard syntax is the
:::            use of curly braces to indicate what string expression gets passed
:::            on to the main Search/Replace. Prior to executing the /P filter,
:::            each brace expression within /PREPL is transformed as follows:
:::
:::               {Expression}  -->  (Expression).replace(Search,Replace)
:::
:::            Any JScript is allowed within /PREPL, except string literals
:::            should not contain $, {, or }.
:::
:::            Using /P without /PREPL is the same as using /P with /PREPL "{$0}"
:::
:::            /PREPL cannot be used with /K or /R.
:::
:::            Note that neither /V nor /XFILE apply to /PREPL.
:::
:::            Example - Substitute X for each character within curly braces,
:::                      excluding the braces.
:::
:::               echo abc{xyz}def|jrepl . X /p "({)(.*?)(})" /prepl "$1+{$2}+$3"
:::
:::            result:
:::
:::               abc{XXX}def
:::
:::      /R PreContext:PostContext[:FILE]
:::      /R Context[:FILE]
:::
:::            Reject matches - Search and write out lines that do not contain
:::            any matches, without doing any replacement. The Replace argument
:::            is still required, but is ignored.
:::
:::            The integers PreContext and PostContext specify how many matching
:::            lines to write before the non-match, and after the non-match,
:::            respectively. If a single Context integer is given, then the same
:::            number of matching lines are written before and after.
:::            A Context of 0 writes only non-matching lines.
:::
:::            If :FILE is appended to the context, then the Search parameter
:::            specifies a file containing one or more search terms, one term
:::            per line. A line is rejected if any of the search terms are found
:::            witin the line. The file can be opened via ADO if |CharSet
:::            (internet character set name) is appended to the file name.
:::            Note: the /V option does not apply to Search if /K :FILE is used.
:::
:::            /R is incomptaible with /A, /J, /JQ, /JMATCH, /JMATCHQ, /K, /M,
:::            /MATCH, /S, and /T.
:::
:::      /RTN ReturnVar[:[-]LineNumber]
:::
:::            Write the result to variable ReturnVar.
:::
:::            If the optional LineNumber is present, then only that specified
:::            line within the result set is returned. A LineNumber of 1 is the
:::            first line. A negative LineNumber is measured from the end of the
:::            result set, so -1 is the last line.
:::
:::            All byte codes except NULL (0x00) are preserved, regardless
:::            whether delayed expansion is enabled or not. An error is thrown
:::            and no value stored if the result contains NULL.
:::
:::            An error is thrown and no value stored if the value does not fit
:::            within a variable. The maximum returned length varies depending
:::            on the variable name and result content. The longest possible
:::            returned length is 8179 bytes.
:::
:::            The line terminator of the last match is suppressed if /MATCH,
:::            /JMATCH, or /JMATCHQ is used.
:::
:::      /S VarName
:::
:::            The source is read from environment variable VarName instead
:::            of from stdin. Without the /M option, ^ anchors the beginning
:::            of the string, and $ the end of the string. With the /M option,
:::            ^ anchors the beginning of a line, and $ the end of a line.
:::
:::            The variable name must not begin with /.
:::
:::      /T DelimiterChar
:::      /T FILE
:::
:::            The /T option is very similar to the Oracle Translate() function,
:::            or the unix tr command, or the sed y command.
:::
:::            The Search represents a set of search expressions, and Replace
:::            is a like sized set of replacement expressions. Expressions are
:::            delimited by DelimiterChar (a single character). If DelimiterChar
:::            is an empty string, then each character is treated as its own
:::            expression. The /L option is implicitly set if DelimiterChar is
:::            empty. Normally escape sequences are interpreted after the search
:::            and replace strings are split into expressions. But if the
:::            DelimiterChar is empty and /XSEQ is used, then escape sequences
:::            are interpreted prior to the split at every character.
:::
:::            An alternate syntax is to specify the word FILE instead of a
:::            DelimiterChar, in which case the Search and Replace parameters
:::            specify files that contain the search and replace expressions,
:::            one expression per line. Each file can be opened via ADO if
:::            |CharSet (internet character set name) is appended to the file
:::            name. Note that the /V option does not apply to Search and Replace
:::            if /T FILE is used.
:::
:::            Each substring from the input that matches a particular search
:::            expression is translated into the corresponding replacement
:::            expression.
:::
:::            The search expressions may be regular expressions, possibly with
:::            captured groups. Note that each expression is itself converted into
:::            a captured group behind the scene, and the operation is performed
:::            as a single search/replace upon execution. So backreferences within
:::            each regex, and $n references within each replacement expression,
:::            must be adjusted accordingly. The total number of expressions plus
:::            captured groups must not exceed 99.
:::
:::            If an expression must include a delimiter, then an escape
:::            sequence must be used (not an issue if the FILE syntax is used).
:::
:::            Search expressions are tested from left to right. The left most
:::            matching expression takes precedence when there are multiple
:::            matching expressions.
:::
:::        Examples using /T:
:::
:::          ROT13 - Simple character substitution is achieved by setting the
:::          /T delimiter to an empty string. The search and replace strings
:::          must have identical length. The use of line continuation aligns
:::          the replace string directly below the search string, thus making
:::          it very easy to see exactly how each character will be translated.
:::          The "a" in the search string will be replaced by the "n" in the
:::          replace string. And you can see the symmetry in that the "n" will
:::          be replaced by "a".
:::
:::            echo The quick brown fox jumps over a lazy dog | jrepl^
:::             "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"^
:::             "nopqrstuvwxyzabcdefghijklmNOPQRSTUVWXYZABCDEFGHIJKLM"^
:::             /t ""
:::
:::            -- OUTPUT --
:::
:::            Gur dhvpx oebja sbk whzcf bire n ynml qbt
:::
:::          Simple string substitution - The /T option specifies that string
:::          expressions are delimited by a space. The /L option prevents "."
:::          from being interpreted as a regex wildcard character.
:::
:::            echo The blackbird flew through the blue sky. | jrepl^
:::             "black blue sky ." "blue black night !" /l /t " "
:::
:::            -- OUTPUT--
:::
:::            The bluebird flew through the black night!
:::
:::          Simple string substitution using FILE - This is the same as the
:::          prior example, except now the Search and Replace strings are in
:::          the following files:
:::
:::            find.txt          repl.txt
:::            --------          --------
:::            black             blue
:::            blue              black
:::            sky               night
:::            .                 !
:::
:::          The following command yields the same output as before:
:::
:::            echo The blackbird flew through the blue sky. | jrepl^
:::             find.txt repl.txt /l /t file
:::
:::          Pig Latin - This example shows how /T can be used with regular
:::          expressions, and it demonstrates how the numbering of captured
:::          groups must be adjusted. The /T delimiter is set to a space.
:::
:::          The first regex is captured as $1, and it matches words that begin
:::          with a consonant. The first captured group ($2) contains the initial
:::          sequence of consonants, and the second captured group ($3) contains
:::          the balance of the word. The corresponding replacement string moves
:::          $2 after $3, with a "-" in between, and appends "ay".
:::
:::          The second regex matches any word, and it is captured as $4 because
:::          the prior regex ended with group $3. Because the first regex matched
:::          all words that begin with consonants, the only thing the second
:::          regex can match is a word that begins with a vowel. The replacement
:::          string simply adds "-yay" to the end of $4. Note that $0 could have
:::          been used instead of $4, and it would yield the same result.
:::
:::            echo Can you speak Pig Latin? | jrepl^
:::             "\b((?:qu(?=[aeiou])|[bcdfghj-np-twxz])+)([a-z']+)\b \b[a-z']+\b"^
:::             "$3-$2ay $4-yay" /t " " /i
:::
:::            -- OUTPUT --
:::
:::            an-Cay you-yay eak-spay ig-Pay atin-Lay?
:::
:::          Pig-Latin with proper capitalization - This is simply an extension
:::          of the prior example. The /JBEG option defines a fixCaps() function
:::          that checks if the translated word is all lower case, except for one
:::          capital letter after the "-". If so, then the initial letter is
:::          capitalized, and the remainder is converted to lower caae. The /JQ
:::          option treats the replacement strings as JScript expressions. The
:::          first replacement expression uses fixCaps() to properly restore case.
:::
:::            echo Can you speak Pig Latin? | jrepl^
:::             "\b((?:qu(?=[aeiou])|[bcdfghj-np-twxz])+)([a-z']+)\b \b[a-z']+\b"^
:::             "$txt=fixCaps($3+'-'+$2+'ay') $txt=$4+'-yay'"^
:::             /t " " /i /j /jbeg ^"^
:::             function fixCaps(str){^
:::               return str.search(/[a-z']+-[A-Z][a-z]*$/)==0 ?^
:::               str.substr(0,1).toUpperCase()+str.substr(1).toLowerCase() : str^
:::             }^"
:::
:::            -- OUTPUT --
:::
:::            An-cay you-yay eak-spay Ig-pay Atin-lay?
:::
:::      /TFLAG Flags
:::
:::            Used to specify XRegExp non-standard mode flags for use with /T.
:::            /TFLAG is ignored unless both /T and /XREG are used.
:::
:::      /U  - Write lines using a Unix line terminator \n instead of Windows
:::            terminator of \r\n. This has no effect if the /M option is used
:::            unless /MATCH, /JMATCH, or /JMATCHQ is also used.
:::
:::            Note that /U does not affect input.ReadLine or output.WriteLine
:::            methods in user supplied JScript. ReadLine always accepts both
:::            \r\n and \n as line terminators. And WriteLine always terminates
:::            lines with \r\n.
:::
:::      /UTF - All input and output encodings are Unicode UTF-16 Little
:::            Endian (UTF-16LE). This includes stdin and stdout. The only
:::            exceptions are /JLIB and /XREG files, which are still read
:::            as ASCII.
:::
:::            The \xFF\xFE BOM is optional for input.
:::
:::            Output files will automatically have the \xFF\xFE BOM inserted.
:::            But stdout will not have the BOM.
:::
:::            Regular expression support of Unicode can be improved by using
:::            the /XREG option.
:::
:::            Variables are never written to temporary files (/XFILE is ignored)
:::            if /UTF is used.
:::
:::            Unfortunately, /UTF is incompatible with /RTN.
:::
:::      /V  - Search, Replace, /INC BlockList, /EXC BlockList, /P FilterRegex,
:::            /JBEG InitCode, /JBEGLN NewLineCode, /JEND FinalCode, and
:::            /JENDLN EndLineCode all represent the names of environment
:::            variables that contain the respective values. An undefined
:::            variable is treated as an empty string.
:::
:::            Variable names beginning with / are reserved for option storage
:::            and other internal uses. So user defined variables used with /V
:::            must not have a name that begins with /.
:::
:::      /X  - Shorthand for combined /XFILE and /XSEQ.
:::
:::      /XBYTES - Force creation of a new XBYTES.DAT file for use by the /XSEQ
:::            option when decoding \xnn sequences.
:::
:::      /XBYTESOFF - Force JREPL to use pre v7.4 behavior where /XSEQ \xnn is
:::            always interpreted as Windows-1252.
:::
:::      /XFILE - Preserves extended ASCII characters that may appear within
:::            command line arguments and/or variables by first writing the
:::            values to temporary files within the %TEMP% directory. Extended
:::            ASCII values are byte codes >= 128 (0x80). This option is ignored
:::            (no temporary files written) if /UTF is also used.
:::
:::            Temporary files may be needed when the cmd.exe active code page
:::            does not match the default character set used by the CSCRIPT
:::            JSCRIPT engine.
:::
:::      /XREG FileList
:::
:::            Adds support for XRegExp by loading the xregexp files specified
:::            in FileList before any /JLIB code is loaded. Multiple files are
:::            delimited by forward slashes (/). If FileList is simply a dot,
:::            then substitute the value of environment variable XREGEXP for
:::            the FileList.
:::
:::            The simplest option is to load "xregexp-all.js", but this
:::            includes all available XRegExp options and addons, some of which
:::            are unlikely to be useful to JREPL. Alternatively you can load
:::            only the specific modules you need, but they must be loaded in the
:::            correct order.
:::
:::            Once the XRegExp module(s) are loaded, all user supplied regular
:::            expressions are created using the XRegExp constructor rather than
:::            the standard RegExp constructor. Also, XRegExp.install('natives')
:::            is executed so that many standard regular expression methods are
:::            overridden by XRegExp methods.
:::
:::            /XREG requires XRegExp version 2.0.0 or 3.x.x. JREPL will not
:::            support version 4.x.x (when it is released) because v4.x.x
:::            is scheduled to drop support for XRegExp.install('natives'). 
:::
:::            One of the key features of XRegExp is that it extends the JScript
:::            regular expression syntax to support named capture groups, as in
:::            (?<name>anyCapturedExpression). Named backreference syntax in
:::            regular expressions is \k<name>. Named group syntax in Replace
:::            strings is ${name}, and in Replace JScript code the syntax is
:::            $0.name
:::
:::            The /T option is no longer limited to 99 capture groups when
:::            /XREG is used. However, /T replace expressions must reference a
:::            captured group by name if the capture index is 100 or above.
:::
:::            Every /T search expression is automatically given a capture group
:::            name of Tn, where n is the 0 based index of the /T expression.
:::
:::            XRegExp also adds support for non-standard mode flags:
:::                n - Explicit capture
:::                s - Dot matches all
:::                x - Free spacing and line comments
:::                A - Astral
:::            These flags can generally be applied by using (?flags) syntax
:::            at the begining of any regex. This is true for /P, /INC, /EXC,
:::            and most Find regular expressions. The one exception is /T doesn't
:::            support (?flags) at the beginning of the Find string. The /TFLAG
:::            option should be used to specify XRegExp flags for use with /T.
:::
:::            XRegExp also improves regular expression support for Unicode via
:::            \p{Category}, \p{Script}, \p{InBlock}, \p{Property} escape
:::            sequences, as well as the negated forms \P{...} and \p{^...}.
:::            Note that example usage on xregexp.com shows use of doubled back
:::            slashes like \\p{...}. But JREPL automatically does the doubling
:::            for you, so you should use \p{...} instead.
:::
:::            See xregexp.com for more information about the capabilities of
:::            XRegExp, and for links to download XRegExp.
:::
:::      /XSEQ - Enables extended escape sequences for both Search strings and
:::            Replacement strings, with support for the following sequences:
:::
:::            \\     -  Backslash
:::            \b     -  Backspace
:::            \c     -  Caret (^)
:::            \f     -  Formfeed
:::            \n     -  Newline
:::            \q     -  Quote (")
:::            \r     -  Carriage Return
:::            \t     -  Horizontal Tab
:::            \v     -  Vertical Tab
:::            \xnn   -  Extended ASCII byte code expressed as 2 hex digits nn.
:::                      The code is mapped to the correct Unicode code point,
:::                      depending on the chosen character set. If used within
:::                      a Find string, then the input character set is used. If
:::                      within a Replacement string, then the output character
:::                      set is used. If the selected character set is invalid or
:::                      not a single byte character set, then \xnn is treated as
:::                      a Unicode code point. Note that extended ASCII character
:::                      class ranges like [\xnn-\xnn] should not be used because
:::                      the intended range likely does not map to a contiguous
:::                      set of Unicode code points - use [\x{nn-mm}] instead.
:::            \x{nn-mm} - A range of extended ASCII byte codes for use within
:::                      a regular expression character class expression. The
:::                      The min value nn and max value mm are expressed as hex
:::                      digits. The range is automatically expanded into the
:::                      full set of mapped Unicode code points. The character
:::                      set mapping rules are the same as for \xnn.
:::            \x{nn,CharSet} - Same as \xnn, except explicitly uses CharSet
:::                      character set mapping.
:::            \x{nn-mm,CharSet} - Same as \x{nn-mm}, except explicitly uses
:::                      CharSet character set mapping.
:::            \unnnn -  Unicode code point expressed as 4 hex digits nnnn.
:::            \u{N}  -  Any Unicode code point where N is 1 to 6 hex digits
:::
:::            JREPL automatically creates an XBYTES.DAT file containing all 256
:::            possible byte codes. The XBYTES.DAT file is preferentially created
:::            in "%ALLUSERSPROFILE%\JREPL\" if at all possible. Otherwise the
:::            file is created in "%TEMP%\JREPL\" instead. JREPL uses the file
:::            to establish the correct \xnn byte code mapping for each character
:::            set. Once created, successive runs reuse the same XBYTES.DAT file.
:::            If the file gets corrupted, then use the /XBYTES option to force
:::            creation of a new XBYTES.DAT file. If JREPL cannot create the file
:::            for any reason, then JREPL silently defaults to using pre v7.4
:::            behavior where /XSEQ \xnn is interpreted as Windows-1252. Creation
:::            of XBYTES.DAT requires either CERTUTIL.EXE or ADO. It is possible
:::            that both may be missing from an XP machine.
:::
:::            Without the /XSEQ option, only standard JSCRIPT escape sequences
:::            \\, \b, \f, \n, \r, \t, \v, \xnn, \unnnn are available for the
:::            search strings. And the \xnn sequence represents a unicode
:::            code point, not extended ASCII.
:::
:::            Extended escape sequences are supported even when the /L option
:::            is used. Both Search and Replace support all of the extended
:::            escape sequences if both the /XSEQ and /L options are combined.
:::
:::            Extended escape sequences are not applied to JScript code when
:::            using any of the /Jxxx options. Use the decode() function if
:::            extended escape sequences are needed within the code.
:::
::/JSCRIPT
:::
:::  The following global JScript variables/objects/functions are available for
:::  use in JScript code associated with the /Jxxx options. User code may safely
:::  declare additional variables/objects/functions because all other internal
:::  objects used by JREPL are hidden behind an opaque _g object.
:::
:::      ln     - Within /JBEGLN, /JENDLN, and Replace code = current line number
:::               Within /JBEG code = 0
:::               Within /JEND code = total number of lines read.
:::               This value is always 0 if the /M or /S option is used.
:::
:::      cnt    - The total number of lines in the input. The value is undefined
:::               unless the /C option is used.
:::
:::      skip   - If true, do not search/replace any more lines until the value
:::               becomes false. /JBEGLN and /JENDLN code are still executed for
:::               each line, regardless. If set to true while in the midst of
:::               searching a line, then that search will continue to the end of
:::               the current line.
:::
:::               The default value is false.
:::
:::               This variable has no impact if the /M or /S options is used.
:::
:::               Note that this variable operates independently of the /INC
:::               and /EXC options.
:::
:::      quit   - If true, then do not read any more lines of input. The current
:::               line is still processed to completion, and /JEND code is still
:::               executed afterward.
:::
:::               The default value is false.
:::
:::               This variable has no impact if the /M or /S options is used.
:::
:::      env('varName')
:::
:::               Access to environment variable named varName.
:::
:::      decode( String [,CharSet] )
:::
:::               Decodes extended escape sequences within String as defined by
:::               the /XSEQ option, and returns the result. CharSet specifies the
:::               single byte character set to use for \xnn escape sequences.
:::               If CharSet is 'input', then the character set of the input is
:::               used. If CharSet is 'output', then the character set of the
:::               output is used. If CharSet is 'default' or undefined, then the
:::               default character set for the machine is used. Otherwise,
:::               CharSet should be a valid internet character set name understood
:::               by the machine. If the selected character set is invalid or not
:::               a single byte character set, then \xnn is treated as a Unicode
:::               code point.
:::
:::               All backslashes within String must be escaped an extra time to
:::               use this function in your code.
:::
:::               Examples:
:::                  quote literal:       decode('\\q','output')
:::                  extended ASCII(128): decode('\\x80','output')
:::                  backslash literal:   decode('\\\\','output')
:::
:::               This function is only needed if you use any \q, \c, or \u{N}
:::               escape sequences, or \xnn escape sequence for extended ASCII.
:::               
:::      lpad( value, padString )
:::      lpad( value, length [,padString] )
:::
:::               Used to left pad a value to a minimum width string. If the
:::               value already has string width >= the desired length, then no
:::               change is made. Otherwise it left pads the value with the
:::               characters of the pad string to the desired length. If only
:::               padString is specified, then the value is padded to the length
:::               of padString. If length is specified with a padString, then
:::               padString is replicated as needed to get the desired length.
:::               If length is specified without padString, then spaces are used
:::               for the padString.
:::
:::               Examples:
:::                  lpad(15,'    ')        returns '  15'
:::                  lpad(15,4)             returns '  15'
:::                  lpad(15,'0000')        returns '0015'
:::                  lpad(15,4,'0')         returns '0015'
:::                  lpad(19011,4,'0')      returns '19011'
:::                  lpad('A','. . . . .')  returns '. . . . .A'
:::                  lpad('A',9,'. ')       returns '. . . . .A'
:::                  lpad('AB','. . . . .') returns '. . . . AB'
:::                  lpad('AB',9,'. ')      returns '. . . . AB'
:::
:::      rpad( value, padString )
:::      rpad( value, length [,padString] )
:::
:::               Used to right pad a value to a minimum width string. If the
:::               value already has string width >= the desired length, then no
:::               change is made. Otherwise it right pads the value with the
:::               characters of the pad string to the desired length. If only
:::               padString is specified, then the value is padded to the length
:::               of padString. If length is specified with a padString, then
:::               padString is replicated as needed to get the desired length.
:::               If length is specified without padString, then spaces are used
:::               for the padString.
:::
:::               Examples:
:::                  rpad('hello','          ')  returns 'hello     '
:::                  rpad('hello',10)            returns 'hello     '
:::                  rpad('hello',' . . . . .')  returns 'hello. . .'
:::                  rpad('hello',10,' .')       returns 'hello. . .'
:::                  rpad('hell',' . . . . .')   returns 'hell . . .'
:::                  rpad('hell',10,' .')        returns 'hell . . .'
:::                  rpad('hello',2)             returns 'hello'
:::
:::      inc( [blockNum] )
:::
:::               A boolean function that returns true or false.
:::
:::               Returns true if the current line appears within the specified
:::               /INC blockNum. A blockNum of 0 specifies the first /INC block.
:::
:::               If blockNum is not specified, then returns true if the current
:::               line appears within any /INC block.
:::
:::      exc( [blockNum] )
:::
:::               A boolean function that returns true or false.
:::
:::               Returns true if the current line appears within the specified
:::               /EXC blockNum. A blockNum of 0 specifies the first /EXC block.
:::
:::               If blockNum is not specified, then returns true if the current
:::               line appears within any /EXC block.
:::
:::      fso    - An instantiation of the FileSystemObject object.
:::
:::      input  - The TextStream object from which input is read.
:::               This may be stdin or a file.
:::
:::               If the file was opened by ADO with |CharSet, then input is
:::               an object that partially emulates a TextStream object, with
:::               a private ADO Stream doing the actual work. The following
:::               public members are available to the ADO object:
:::
:::                 Property           Method
:::                 -------------      ----------
:::                 AtEndOfStream      Read
:::                                    ReadLine  (line terminator \n or \r\n)
:::                                    SkipLine  (line terminator \n or \r\n)
:::                                    Write
:::                                    WriteLine (line terminator always \r\n)
:::                                    Close
:::
:::      output - The TextStream object to which the output is written.
:::               This may be stdout or a file. If /RTN is used, then the output
:::               is first written to a temporary file before it is read and
:::               stored in the return variable.
:::
:::               If the file was opened by ADO with |CharSet, then output is
:::               an object that partially emulates a TextStream object (see the
:::               input object).
:::
:::      stdin  - Equivalent to WScript.StdIn
:::
:::      stdout - Equivalent to WScript.StdOut
:::
:::      stderr - Equivalent to WScript.StdErr
:::
:::      openOutput( fileName[|CharSet[|NB]] [,appendBoolean [,utfBoolean]] )
:::
:::               Open a new TextStream object for writing and assign it to the
:::               output variable. If appendBoolean is truthy, then open the file
:::               for appending.
:::
:::               If |CharSet is appended to the fileName, then open the file
:::               using ADO and the specified internet character set name. The
:::               output variable will be set to an object that partially
:::               emulates a TextStream object (see the input object). Unicode
:::               written by ADO will have a BOM by default. The BOM is blocked
:::               by appending |NB (or |anyValue) to the CharSet.
:::
:::               If utfBoolean is truthy, then output is encoded as unicode
:::               (UTF-16LE). The unicode file will automatically have the BOM
:::               unless opened for appending. The utfBoolean argument is ignored
:::               if |CharSet is also specified.
:::
:::               If fileName is falsey, then output is written to stdout.
:::
:::               All subsequent output will be written to the new destination.
:::
:::               Any prior output file is automatically closed.
:::
:::  See the /JQ option help for info about local $ variables that may be used
:::  within replacement code when using /JQ or /JMATCHQ.
::/HELP
:::
:::  Help is available by supplying a single argument beginning with /? or /??:
:::
:::      /?        - Writes all available help to stdout.
:::      /??       - Same as /? except uses MORE for pagination.
:::
:::      /?Topic   - Writes help about the specified topic to stdout.
:::                  Valid topics are:
:::
:::                    INTRO   - Basic syntax and default behavior
:::                    OPTIONS - Brief summary of all options
:::                    JSCRIPT - JREPL objects available to user JScript
:::                    RETURN  - All possible return codes
:::                    VERSION - Display the version of JREPL.BAT
:::                    HISTORY - A summary of all releases
:::                    HELP    - Lists all methods of getting help
:::
:::                  Example: List a summary of all available options
:::
:::                     jrepl /?options
:::
:::      /?WebTopic - Opens up a web page within your browser about a topic.
:::                  Valid web topics are:
:::
:::                    REGEX   - Microsoft regular expression documentation
:::                    REPLACE - Microsoft Replace method documentation
:::                    UPDATE  - DosTips release page for JREPL.BAT
:::                    CHARSET - List of possible character set names for ADO I/O
:::                              Some character sets may not be installed
:::                    XREGEXP - xRegExp.com home page (extended regex docs)
:::
:::      /?/Option - Writes detailed help about the specified /Option to stdout.
:::
:::                  Example: Display paged help about the /T option
:::
:::                     jrepl /??/t
:::
:::      /?CHARSET/[Query] - List all character set names for use with ADO I/O
:::                  that are installed on this computer. Optionally restrict
:::                  the list to names that contain Query. Wildcards * and ? may
:::                  be used within Query. The default Query is an empty string,
:::                  meaning list all available character sets. The list is
:::                  generated via reg.exe.
:::
:::                  Examples:
:::
:::                     jrepl /??charset/    - Paged list of all available names
:::                     jrepl /?charset/utf  - List of names containing "utf"
::/RETURN
:::
:::  Possible ERRORLEVEL Return Codes:
:::
:::      If /? was used, and no other argument
:::          0 = Only possible return
:::
:::      If /MATCH, /JMATCH, /JMATCHQ, /K, and /R were not used
:::          0 = At least one change was made
:::          1 = No change was made
:::          2 = Invalid call syntax or incompatible options
:::          3 = JScript runtime error
:::
:::      If /MATCH, /JMATCH, /JMATCHQ, /K, or /R was used
:::          0 = At least one line was written
:::          1 = No line was written
:::          2 = Invalid call syntax or incompatible options
:::          3 = JScript runtime error
::/VERSION
:::
:::  JREPL.BAT version 7.15 was written by Dave Benham, and originally posted at
:::  http://www.dostips.com/forum/viewtopic.php?f=3&t=6044
::/

============= :Batch portion ===========
@echo off
setlocal disableDelayedExpansion

:: Process Help
if .%2 equ . call :help "%~1" && exit /b 0 || call :exitErr "Insufficient arguments"

:: Define options
set ^"/options= /A: /APP: /B: /C: /D:":" /E: /EXC:"" /F:"" /I: /INC:""^
                /J: /JBEG:"" /JBEGLN:"" /JEND:"" /JENDLN:"" /JLIB:"" /JMATCH: /JMATCHQ: /JQ:^
                /K:"" /L: /M: /MATCH: /N:0 /O:"" /OFF:0 /P:"" /PFLAG:"g" /PREPL:"" /R:"" /RTN:"" /S:""^
                /T:"none" /TFLAG:"" /U: /UTF: /V: /X: /XBYTES: /XBYTESOFF: /XFILE: /XSEQ: /XREG:"" "
:: Set default option values
for %%O in (%/options%) do for /f "tokens=1,* delims=:" %%A in ("%%O") do set "%%A=%%~B"

:: Get options
:loop
if not "%~3"=="" (
  set "/test=%~3"
  setlocal enableDelayedExpansion
  if "!/test:~0,1!" neq "/" call :exitErr "Too many arguments"
  set "/test=!/options:*%~3:=! "
  if "!/test!"=="!/options! " (
      endlocal
      call :exitErr "Invalid option %~3"
  ) else if "!/test:~0,1!"==" " (
      endlocal
      set "%~3=1"
  ) else (
      endlocal
      set "%~3=%~4"
      shift /3
  )
  shift /3
  goto :loop
)

:: Validate options
if defined /M if defined /A if not defined /S                                      call :exitErr "/M cannot be used with /A without /S"
if "%/O%" equ "-" if not defined /F                                                call :exitErr "Output = - but Input file not specified"
if defined /F if defined /S                                                        call :exiterr "/S cannot be used with /F"
if defined /F for %%A in ("%/F%") do for %%B in ("%/O%") do if "%%~fA" equ "%%~fB" call :exitErr "Output file cannot match Input file"
if defined /RTN if defined /O                                                      call :exitErr "/O and /RTN are mutually exclusive"
if defined /RTN if defined /UTF                                                    call :exitErr "/UTF and /RTN are mutually exclusive"
if "%/EXC%%/INC%%/C%%/JBEGLN%%/JENDLN%" neq "" if "%/M%%/S%" neq ""                call :exitErr "/C, /JBEGLN, and /JENDLN cannot be used with /M or /S"
for /f "tokens=2" %%A in ("%/J% %/JQ% %/JMATCH% %/JMATCHQ% %/K% %/R% %/MATCH%") do call :exitErr "/J, /JQ, /JMATCH, /JMATCHQ, /MATCH, /K and /R are all mutually exclusive"
if "%/K%%/R%" neq "" if "%/A%%/M%%/PREPL%%/S%%/T%" neq "none"                      call :exitErr "/K, /R cannot be used with /A, /M, /S, /T or /PREPL"
if defined /MATCH if "%/A%%/T%" neq "none"                                         call :exitErr "/MATCH cannot be used with /A or /T"
for /f delims^=giGI^ eol^= %%A in ("%/PFLAG%") do                                  call :exitErr "Invalid /PFLAG value"
for /f "delims=| eol=| tokens=2*" %%A in ("%/APP%|%/O%x") do if %%A==- if .%%B neq . call :exitErr "/APP cannot be combined with /O - with CharSet"

:: Transform options
if "%/XREG%"=="." (set /XREG=%XREGEXP%)
if defined /X set "/XFILE=1" & set "/XSEQ=1"
if defined /MATCH set "/JMATCHQ=1"
if defined /JMATCHQ set "/JMATCH=1"
if defined /JMATCH set "/J=1"
if defined /JQ set "/J=1"
if defined /UTF set "/UTF=//u" & set "/XFILE="
if not defined /T set "/L=1"
if "%/M%%/S%" neq "" set "/N=0"
if defined /RTN (
  setlocal enableDelayedExpansion
  for /f "eol=: delims=: tokens=1,2" %%A in ("!/RTN!") do (
    endlocal
    set "/RTN=%%A"
    set "/RTN_LINE=%%B"
  )
)

if defined /XBYTESOFF set "/XBYTES=" & goto :endXBytes
if defined /XBYTES set "/XBYTES=" & goto :createXBytes
for %%F in (
  "%ALLUSERSPROFILE%\JREPL\XBYTES.DAT"
  "%TEMP%\JREPL\XBYTES.DAT"
  "%TMP%\JREPL\XBYTES.DAT"
) do if "%%~zF" equ "256" set "/XBYTES=%%~fF" & goto :endXBytes

:createXBytes
:: Attempt to create XBYTES.DAT via CERTUTIL. If able to write to the JREPL
:: subdirectory, but unable to create correct file, then pass task to JScript.
for %%F in (
  "%ALLUSERSPROFILE%"
  "%TEMP%"
  "%TMP%"
) do if %%F neq "" for %%F in ("%%~F\JREPL\XBYTES.DAT") do (
  del %%F
  md "%%~dpF"
  (  >"%%~dpnF.HEX" (
    for %%A in (0 1 2 3 4 5 6 7 8 9 A B C D E F) do for %%B in (0 1 2 3 4 5 6 7 8 9 A B C D E F) do echo %%A%%B
  )) && (
    set "/XBYTES=%%~fF"
    certutil.exe -f -decodehex "%%~dpnF.HEX" "%%~fF"
    for %%G in (%%F) do if "%%~zG" neq "256" del %%F
    del "%%~dpnF.HEX"
    goto :endXBytes
  )
) >nul 2>nul
:endXBytes

set ^"/FIND=%1"
set ^"/REPL=%2"
call :GetScript /SCRIPT
set "/LOCK="

set "/FindReplVar="
if defined /UTF (
  set "/FindReplVar=1"
  set "/FIND2=%/FIND:"=%"
  set "/REPL2=%/REPL:"=%"
  set "/FIND=/FIND2"
  set "/REPL=/REPL2"
  goto :noLock
)
if defined /V if /i "%/T%" neq "FILE" set "/FindReplVar=1"
if defined /XFILE if /i "%/T%" neq "FILE" set "/FindReplVar=1"
if defined /RTN goto :lock
if not defined /XFILE goto :noLock
if defined /FindReplVar goto :lock
if not defined /JBEG if not defined /JBEGLN if not defined /JEND if not defined /JENDLN if not defined /INC if not defined /EXC if not defined /P if not defined /S goto :noLock

:lock
setlocal enableDelayedExpansion
set "/LOCK=jrepl.bat.!date:\=-!_!time::=.!_!random!.temp"
set "/LOCK=!/LOCK:/=-!"
for /f "delims=" %%F in ("!temp!\!/LOCK::=-!") do (
  endlocal
  set "/LOCK=%%~fF"
)
if defined /RTN set "/O=%/LOCK%.RTN"
9>&2 2>nul (
  8>"%/LOCK%" (
    2>&9 (
      if defined /XFILE (
        setlocal enableDelayedExpansion
        if defined /S call :writeVar S
        if defined /V (
          if defined /FindReplVar (
            call :writeVar FIND
            call :writeVar REPL
          )
          if defined /JBEG   call :writeVar JBEG
          if defined /JBEGLN call :writeVar JBEGLN
          if defined /JEND   call :writeVar JEND
          if defined /JENDLN call :writeVar JENDLN
          if defined /INC    call :writeVar INC
          if defined /EXC    call :writeVar EXC
          if defined /P      call :writeVar P
        ) else (
          if defined /FindReplVar (
            (echo(!/FIND:^"=!) >"!/LOCK!.FIND"
            (echo(!/REPL:^"=!) >"!/LOCK!.REPL"
          )
          if defined /JBEG (echo(!/JBEG!) >"!/LOCK!.JBEG"
          if defined /JBEGLN (echo(!/JBEGLN!) >"!/LOCK!.JBEGLN"
          if defined /JEND (echo(!/JEND!) >"!/LOCK!.JEND"
          if defined /JENDLN (echo(!/JENDLN!) >"!/LOCK!.JENDLN"
          if defined /INC (echo(!/INC!) >"!/LOCK!.INC"
          if defined /EXC (echo(!/EXC!) >"!/LOCK!.EXC"
          if defined /P (echo(!/P!) >"!/LOCK!.P"
        )
        endlocal
      )
      call :execute
    )
  )
  if errorlevel 3 (del "%/LOCK%*"&exit /b 3)
  if errorlevel 1 (del "%/LOCK%*"&(call)) else del "%/LOCK%*"
  if "%/RTN%" equ "" exit /b
) || goto :lock

:writeVar
for /f delims^=^ eol^= %%A in ("!/%1!") do (echo(!%%A!) >"!/LOCK!.%1"
exit /b

:noLock
call :execute
exit /b %errorlevel%

:execute
cscript.exe //E:JScript //nologo %/UTF% "%/SCRIPT%" %/FIND% %/REPL%
if not defined /RTN exit /b %errorlevel%

::returnVar
if errorlevel 3 exit /b %errorlevel%
set "/ERR=%errorlevel%"
set "/NORMAL="
for /f "usebackq delims=" %%A in ("%/LOCK%.RTN") do (
  if not defined /NORMAL (
    set "/NORMAL=%%A"
  ) else set "/DELAYED=%%A"
)
for /f %%2 in (
  'copy /z "%/SCRIPT%" nul' %= This generates CR =%
) do for %%1 in (^"^
%= This generates quoted LF =%
^") do for /f "tokens=1,2" %%3 in (^"%% "") do (
  (goto) 2>nul
  (goto) 2>nul
  if "^!^" equ "^!" (
    set "%/RTN%=%/DELAYED:~1%"!
  ) else (
    set "%/RTN%=%/NORMAL:~1%"
  )
  if %/ERR% equ 0 (call ) else (call)
)

:GetScript
set "%1=%~f0"
exit /b

:help
setlocal
set "help=%~1"
setlocal enableDelayedExpansion
if "!help:~0,2!" neq "/?" exit /b 1
set "noMore=1"
set "help=!help:~2!"
if defined help if "!help:~0,1!" equ "?" (
  set "noMore="
  set "help=!help:~1!"
)
for /f "delims=" %%A in ("/!help!") do if /i "%%~pA" equ "\CharSet\" ( %= /?CHARSET/ =%
  echo(
  if defined noMore (
    for /f "delims=" %%F in ('reg query HKCR\MIME\Database\Charset /k /f "%%~nxA"') do echo %%~nF
  ) else (
    (cmd /c "for /f "delims=" %%F in ('reg query HKCR\MIME\Database\Charset /k /f "%%~nxA"') do @echo %%~nF") | more /e
  )
  exit /b 0
)
if defined help if "!help:~0,2!" equ "/?" set "help=help"
for /f "delims=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ/ eol=a" %%A in ("!help!") do (
  echo(
  echo Invalid /? option
  exit /b 0
)
if /i "!help!" equ "regex" (
    explorer "https://msdn.microsoft.com/en-us/library/ae5bf541.aspx"
    exit /b 0
) else if /i "!help!" equ "replace" (
    explorer "https://msdn.microsoft.com/en-US/library/efy6s3e6.aspx"
    exit /b 0
) else if /i "!help!" equ "update" (
    explorer "http://www.dostips.com/forum/viewtopic.php?f=3&t=6044"
    exit /b 0
) else if /i "!help!" equ "charset" (
    explorer "https://msdn.microsoft.com/en-us/library/windows/desktop/dd317756.aspx"
    exit /b 0
) else if /i "!help!" equ "xregexp" (
    explorer "http:xregexp.com"
    exit /b 0
) else if "!help!" equ "" ( %= /? =%
    set "find=^:::(.*)"
    set "repl=$1"
    set ^"cmd="%~f0" find repl /v /a /f "%~f0"^"
) else if "!help:~0,1!" equ "/" (   %= /?/Option =%
    set "find=^:::(.*)"
    set "repl=$1"
    set "help=!help:/=\/!"
    set "inc=/^^::: {6}!help!(?= |$)/i/:/^^::: {6}\/(?^!!help:~2!(?= |$))|^^::\//i-1"
    set "help=!help:\/=/!"
    set ^"cmd=echo(^&call "%~f0" find repl /v /jmatch /inc inc /f "%~f0"^|^|echo Help not found for option %help%^"
) else ( %= /?Topic =%
    set "find=^:::?(.*)"
    set "repl=$1"
    set "inc=/^^::\/!help:/=\/!$/i/+1:/^^::\//-1"
    set ^"cmd="%~f0" find repl /v /jmatch /inc inc /f "%~f0"^|^|(echo(^&echo Help not found for topic %help%^)^"
)
if defined noMore (
  setlocal
  set "pathext=."
  call %cmd%
) else (%cmd%) | more /e
exit /b 0

:exitErr
>&2 (
  echo ERROR: %~1.
  echo   Use JREPL /? or JREPL /?? to get help.
  (goto) 2>nul
  exit /b 2
)


************* JScript portion **********/
var _g=new Object();
_g.loc='';
try {
  var env=WScript.CreateObject("WScript.Shell").Environment("Process"),
      cnt,
      ln=0,
      skip=false,
      quit=false,
      fso,
      stdin=WScript.StdIn,
      stdout=WScript.Stdout,
      stderr=WScript.Stderr,
      output,
      input;
  _g.ForReading=1;
  _g.ForWriting=2;
  _g.ForAppending=8;
  _g.FileFormat = env('/UTF') ? -1 : 0;
  _g.TemporaryFolder=2;
  fso = new ActiveXObject("Scripting.FileSystemObject");
  _g.inFile=env('/F');
  _g.inFileA=_g.inFile.split('|');
  _g.outFile=env('/O');
  _g.outFileA=_g.outFile.split('|');
  if (_g.outFileA[0]=='-') {
    if (_g.outFileA[1]===undefined) {_g.outFileA[1]=_g.inFileA[1]; _g.outFileA[2]=_g.inFileA[2];}
    _g.outFile = _g.inFileA[0]+'.new'+(_g.outFileA[1]?'|'+_g.outFileA[1]:'')+(_g.outFileA[2]?'|'+_g.outFileA[2]:'');
    if (env('/APP')) fso.CopyFile( _g.inFileA[0], _g.inFileA[0]+'.new', true );
  }
  _g.tempFile='';
  _g.delim=env('/D');
  _g.term=env('/U')?'\n':'\r\n';

  _g.ADOStream = function( name, mode, format, noBom) {
    var that = this;
    var bomSize = 0;
    try {
      var stream = WScript.CreateObject("ADODB.Stream");
    } catch(ex) {
      throw new Error(215,'ADO unavailable');
    }
    try {
      stream.CharSet = format;
    } catch(ex) {
      throw new Error(215,'ADO character set "'+format+'" is invalid or unavailable');
    }
    stream.LineSeparator = (mode==_g.ForReading) ? 10 : -1;
    stream.Open();
    if (mode !== _g.ForReading && noBom) {
      stream.WriteText("");
      stream.Position = bomSize = stream.Size;
    }
    switch (mode) {
      case _g.ForReading:
        stream.LoadFromFile(name);
        break;
      case _g.ForAppending:
        stream.LoadFromFile(name);
        stream.Position = stream.Size;
      case _g.ForWriting:
        break;
      default:
        throw new Error(215, 'Invalid file mode');
    }
    this.AtEndOfStream = stream.EOS;

    this.ReadLine = function() {
      if (mode!=_g.ForReading) throw new Error(215, 'Bad file mode');
      var str = stream.ReadText(-2);
      that.AtEndOfStream = stream.EOS;
      return str.slice(-1)=='\r' ? str.slice(0,-1) : str;
    }

    this.Read = function(size) {
      if (mode!=_g.ForReading) throw new Error(215, 'Bad file mode');
      var str = stream.ReadText(size)
      that.AtEndOfStream = stream.EOS;
      return str;
    }

    this.SkipLine = function() {
      if (mode!=_g.ForReading) throw new Error(215, 'Bad file mode');
      stream.SkipLine();
      that.AtEndOfStream = stream.EOS;
    }

    this.Write = function(str) {
      if (mode==_g.ForReading) throw new Error(215, 'Bad file mode');
      stream.WriteText(str);
    }

    this.WriteLine = function(str) {
      if (mode==_g.ForReading) throw new Error(215, 'Bad file mode');
      stream.WriteText(str,1);
    }

    this.Close = function() {
      if (mode!==_g.ForReading){
        if (bomSize) {
          var noBomStream = WScript.CreateObject("ADODB.Stream");
          noBomStream.Type = 1;
          noBomStream.Mode = 3;
          noBomStream.Open();
          stream.Position = bomSize;
          stream.CopyTo(noBomStream);
          noBomStream.SaveToFile( name, 2 );
          noBomStream.Flush();
          noBomStream.Close();
          noBomStream = null;
        } else stream.SaveToFile( name, 2 );
      }
      stream.Close();
      stream=null;
    }
  }

  _g.openInput = function( fileName ) {
    var file;
    if (fileName) {
      file = fileName.split('|');
      if (file[1]) {
        file = new _g.ADOStream( file[0], _g.ForReading, file[1], file[2] );
        return file;
      }
      else return fso.OpenTextFile( fileName, _g.ForReading, false, _g.FileFormat );
    }
    else return stdin;
  }

  _g.charMap = new Object();
  _g.readVar = function( val, ref, ext ) {
    var input, buf=1024;
    if (!env('/XFILE') || !val) return (ref && val) ? env(val) : val;
    _g.loc=' reading '+env('/LOCK')+ext;
    input=fso.OpenTextFile( env('/LOCK')+ext, _g.ForReading );
    val='';
    while (!input.AtEndOfStream) {
      val+=input.Read(buf);
      buf*=2;
    }
    input.Close();
    _g.loc=''
    return val.slice(0,-2);
  }

  _g.xbytes = env('/XBYTES');
  if (_g.xbytes && !(fso.FileExists(_g.xbytes))) try {
    // Unable to create file with CERTUTIL, so now try with ADO
    var Stream=WScript.CreateObject('ADODB.Stream'),
        Node=WScript.CreateObject('Microsoft.XMLDOM').createElement('e');
    Node.dataType='bin.base64';
    Node.text='AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4v'
    + 'MDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5f'
    + 'YGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6P'
    + 'kJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/'
    + 'wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v'
    + '8PHy8/T19vf4+fr7/P3+/w==';
    Stream.Type=1;
    Stream.Open();
    Stream.Write(Node.nodeTypedValue);
    Stream.SaveToFile(_g.xbytes);
  } catch(e) {
    _g.xbytes = '';
  }
  var decode = _g.xbytes ?
    // Default dynamic character set decode() for v7.4 and beyond
    function(str, charSet, searchSwitch) {
      function u(codeUnit) {return '\\u'+lpad(codeUnit.toString(16),4,'0');}
      function xToUTF16(byte,charSet) {
        if (typeof _g.charMap[charSet]==='undefined') {
          if (charSet=='default' && _g.utf) {
            _g.charMap[charSet]=false
          } else {
            var stream = _g.openInput( _g.xbytes+(charSet=='default'?'':'|'+charSet) );
            try {
              _g.charMap[charSet] = stream.Read(256);
              stream.Close();
              if (_g.charMap[charSet].length!=256) _g.charMap[charSet]=false;
            } catch(e) {
              _g.charMap[charSet]=false;
            }
          }
        }
        return  u( _g.charMap[charSet] ? _g.charMap[charSet].charCodeAt(byte) : byte );
      }
      function xRange(min,max,charSet) {
        var str='', i;
        for (i=min; i<=max; i++ ) str+=xToUTF16(i,charSet);
        return str;
      }
      function uToUTF16(codePoint) {
        if (codePoint <= 0xFFFF) return u(codePoint);
        codePoint -= 0x10000;
        return u(0xD800|(codePoint>>10)) + u(0xDC00|(codePoint&1023));
      }
      if (charSet===undefined) charSet='default';
      if (charSet=='input') charSet = _g.inFileA[1] ? _g.inFileA[1] : 'default';
      if (charSet=='output') charSet = _g.outFileA[1] ? _g.outFileA[1] : 'default';
      return str.replace(
        /\\(?:\\|b|c|f|n|q|r|t|v|x([0-9a-fA-F]{2})|x\{([0-9a-fA-F]{2}),([^}]+)}|u[0-9a-fA-F]{4}|u\{([0-9a-fA-F]+)\}|x\{([0-9a-fA-F]{2})-([0-9a-fA-F]{2})(?:,([^}]+))?})/g,
        function($0,$1,$2,$3,$4,$5,$6,$7) {
          if ($0=='\\q') return '"';
          if ($0=='\\c') return '^';
          if ($1) $0=xToUTF16(parseInt($1,16),charSet);
          if ($2) $0=xToUTF16(parseInt($2,16),$3);
          if ($4) $0=uToUTF16(parseInt($4,16));
          if ($5) $0=xRange(parseInt($5,16),parseInt($6,16),($7?$7:charSet));
          return searchSwitch===false ? $0 : eval('"'+$0+'"');
        }
      );
    }
    : // Pre-v7.4 decode() that assumes Windows-1252, only used if XBYTES.DAT not available or disabled.
    function(str, ignore, searchSwitch) {
      function toUTF16(codePoint) {
        function u(codeUnit) {return '\\u'+lpad(codeUnit.toString(16),4,'0');}
        if (codePoint <= 0xFFFF) return u(codePoint);
        codePoint -= 0x10000;
        return u(0xD800|(codePoint>>10)) + u(0xDC00|(codePoint&1023));
      }
      str=str.replace(
        /\\(\\|b|c|f|n|q|r|t|v|x80|x82|x83|x84|x85|x86|x87|x88|x89|x8[aA]|x8[bB]|x8[cC]|x8[eE]|x91|x92|x93|x94|x95|x96|x97|x98|x99|x9[aA]|x9[bB]|x9[cC]|x9[dD]|x9[eE]|x9[fF]|x[0-9a-fA-F]{2}|u[0-9a-fA-F]{4}|u\{([0-9a-fA-F]+)\}|x\{([0-9a-fA-F]{2}),[^}]+\})/g,
        function($0,$1,$2,$3) {
          if ($3) {
            $1='x'+$3;
            $0='\\'+$1;
          }
          switch ($1.toLowerCase()) {
            case 'q':   return '"';
            case 'c':   return '^';
            case 'x80': return '\u20AC';
            case 'x82': return '\u201A';
            case 'x83': return '\u0192';
            case 'x84': return '\u201E';
            case 'x85': return '\u2026';
            case 'x86': return '\u2020';
            case 'x87': return '\u2021';
            case 'x88': return '\u02C6';
            case 'x89': return '\u2030';
            case 'x8a': return '\u0160';
            case 'x8b': return '\u2039';
            case 'x8c': return '\u0152';
            case 'x8e': return '\u017D';
            case 'x91': return '\u2018';
            case 'x92': return '\u2019';
            case 'x93': return '\u201C';
            case 'x94': return '\u201D';
            case 'x95': return '\u2022';
            case 'x96': return '\u2013';
            case 'x97': return '\u2014';
            case 'x98': return '\u02DC';
            case 'x99': return '\u2122';
            case 'x9a': return '\u0161';
            case 'x9b': return '\u203A';
            case 'x9c': return '\u0153';
            case 'x9d': return '\u009D';
            case 'x9e': return '\u017E';
            case 'x9f': return '\u0178';
            default:    if ($2) $0=toUTF16(parseInt($2,16));
                        return searchSwitch===false ? $0 : eval('"'+$0+'"');
          }
        }
      );
      return str;
    }
  ;

  _g.getCount = function() {
    if (cnt>=0) return;
    cnt=0;
    if (_g.inFile=='') {
      _g.tempFile=fso.GetSpecialFolder(_g.TemporaryFolder).path+'\\'+fso.GetTempName();
      _g.inFile=_g.tempFile;
      var output=fso.OpenTextFile(_g.tempFile,_g.ForWriting,true,_g.FileFormat);
      while (!input.AtEndOfStream) {
        output.WriteLine(input.ReadLine());
        cnt++
      }
      output.Close();
    } else {
      while (!input.AtEndOfStream) {
        input.SkipLine();
        cnt++;
      }
      input.Close();
    }
    input = _g.openInput(_g.inFile);
  }

  _g.loc=' opening input file';
  input = _g.openInput(_g.inFile);
  _g.loc='';

  if (env('/C')) _g.getCount();

  openOutput( _g.outFile, env('/APP'), _g.FileFormat );

  if (env('/XREG')) {
    _g.loc=' while loading /XREG library';
    _g.libs=env('/XREG').split('/');
    for (_g.i=0; _g.i<_g.libs.length; _g.i++) {
      _g.lib=fso.OpenTextFile(_g.libs[_g.i],_g.ForReading);
      if (!_g.lib.AtEndOfStream) eval(_g.lib.ReadAll());
      _g.lib.Close();
    }
    _g.loc=' while initializing /XREG library';
    _g.newRegExp = function(pattern,flags){ return new XRegExp(pattern,flags); }
    XRegExp.install('natives');
    _g.loc='';
    _g.XRegExp = true;
  } else {
    _g.newRegExp = function(pattern,flags){ return new RegExp(pattern,flags); }  
    _g.XRegExp = false;
  }  

  if (env('/JLIB')) {
    _g.loc=' while loading /JLIB code';
    _g.libs=env('/JLIB').split('/');
    for (_g.i=0; _g.i<_g.libs.length; _g.i++) {
      _g.lib=fso.OpenTextFile(_g.libs[_g.i],_g.ForReading);
      if (!_g.lib.AtEndOfStream) eval(_g.lib.ReadAll());
      _g.lib.Close();
    }
    _g.loc='';
  }

  _g.loc=' in /JBEG code';
  eval( _g.readVar( env('/JBEG'), env('/V'), '.JBEG' ) );
  _g.loc='';

  _g.fmtNum=function(val,pad){return pad.length==0 ? '' : lpad(val,pad)+_g.delim;}

  _g.writeMatch=function(str,off,lnPad,offPad) {
    if (str!==false) {
      _g.rtn=0;
      output.Write(_g.fmtNum(ln,lnPad)+_g.fmtNum(off,offPad)+str+_g.term);
    }
  }

  _g.defineObjectInternal=function(){
    _g.loc=' while defining '+_g.defineObjectObj;
    eval(_g.defineObjectStr);
    _g.loc='';
  }
  _g.defineObject=function(str,obj) {
    _g.defineObjectStr=str;
    _g.defineObjectObj=obj;
    _g.defineObjectInternal();
  }

  if (env('/PREPL')) {
    _g.defineObject(
      '_g.filterReplace=function(){ return '
        + env('/PREPL').replace(/\$(\d+)/g,'arguments[$1]')
        .replace(/{([^}]*)}/g,'($1).replace(_g.search,_g.filterReplace2)')
        +';}'
      ,'/PREPL'
    );
  } else {
    _g.filterReplace=function(str) {
      return str.replace(_g.search,_g.filterReplace2);
    }
  }

  _g.main=function() {
    _g.rtn=1;
    var args=WScript.Arguments;
    var search =  env('/FindReplVar') ? _g.readVar( args.Item(0), env('/V')||env('/UTF'), '.FIND' ) : args.Item(0);
    var replace = env('/FindReplVar') ? _g.readVar( args.Item(1), env('/V')||env('/UTF'), '.REPL' ) : args.Item(1);
    var multi=env('/M')!='';
    var literal=env('/L')!='';
    var alterations=env('/A')!='';
    var srcVar=env('/S');
    var jexpr=env('/J')!='';
    var jmatch=env('/JMATCH')!='';
    var jmatchq=env('/JMATCHQ')!='';
    var jquick=env('/JQ')!='';
    var translate=env('/T');
    var filter = _g.readVar( env('/P'), env('/V'), '.P' );
    var keep, reject, context, krfile=false;
    var rtnVar=env('/RTN');
    if (reject=env('/R')) {
      if (!/^\d+(:\d+)?(:FILE)?$/i.test(reject)) throw new Error(209, 'Invalid /R Context');
      context = reject.toUpperCase().split(':')
      krfile=(context[context.length-1]=='FILE');
      context[0]=Number(context[0]);
      context[1]=(context.length==1 || context[1]=='FILE')?context[0]:Number(context[1]);
    }
    if (keep=env('/K')) {
      if (!/^\d+(:\d+)?(:FILE)?$/i.test(keep)) throw new Error(208, 'Invalid /K Context');
      context = keep.toUpperCase().split(':')
      krfile=(context[context.length-1]=='FILE');
      context[0]=Number(context[0]);
      context[1]=(context.length==1 || context[1]=='FILE')?context[0]:Number(context[1]);
    }
    var options = (keep||reject)?"":"g";
    _g.begLn = _g.readVar( env('/JBEGLN'), env('/V'), '.JBEGLN' );
    _g.endLn = _g.readVar( env('/JENDLN'), env('/V'), '.JENDLN' );

    _g.incBlock = new Array();
    _g.excBlock = new Array();
    _g.incBlock.dynamic = false;
    _g.excBlock.dynamic = false;
    var blockMatch,
        blockSearch = /(?:(-?\d+)|(?:\/((?:\\\/|[^/])+)\/|'((?:''|[^'])+)')([ibe]*)(\/)?)([+-]\d+)?(:(?:(-?\d+)|(\+\d+)|(?:\/((?:\\\/|[^/])+)\/|'((?:''|[^'])+)')([ibe]*))([+-]\d+)?)?(?:,(?=.)|$)?|(.+)/g;
    /*                    1            2                   3               4       5     6         7    8       9            1                   1               1        1                         1
                                                                                                                             0                   1               2        3                         4
        line or range begin
          spec
            1 = line number
            2 = regex
              4 = i|b|e flags
              5 = singleton
            3 = string
              4 = i|b|e flags
              5 = singleton
          6 = offset
        7 = range end
          spec
            8 = line number
            9 = offset from range begin
            10 = regex
              12 = i|b|e flags
            11 = string
              12 = i|b|e flags
          13 = offset
        14 = error
    */
    _g.Block = function(match) {
      if (match[14]) throw new Error(210, 'Invalid block syntax');
      this.offset=match[6]?Number(match[6]):0;
      if (match[1]) {
        this.type='lineNum';
        if ((this.spec=Number(match[1])) < 0) _g.getCount();
        this.lineNum=this.spec+this.offset+(this.spec<0?cnt+1:0);
      } else {
        this.type='regex';
        this.spec=_g.newRegExp( (match[4].search('b')+1?'^':'') + (
            match[2] ? decode(match[2],'input',false) :
            decode(match[3].replace(/''/g,"'"),'input',true).replace(/([.^$*+?()[{\\|])/g,"\\$1")
          ) + (match[4].search('e')+1?'$':''),
          match[4].search('i')+1?'i':''
        );
        this.spec.singleton=match[5]?true:false;
        this.lineNum=void 0;
        if (this.offset<0) throw new Error(211, 'Regex/String offset cannot be negative');
      }
      if (match[7]) {
        this.endOffset=Number(match[13]);
        if (match[8]) {
          this.endType='lineNum';
          if ((this.endSpec=Number(match[8])) < 0) _g.getCount();
          this.endLineNum=this.endSpec+this.endOffset+(this.endSpec<0?cnt+1:0);
        } else if (match[9]) {
          this.endType='offset';
          this.endSpec=Number(match[9]);
          this.endLineNum=this.lineNum+this.endSpec+this.endOffset;
        } else {
          this.endType='regex';
          this.endSpec=_g.newRegExp( (match[12].search('b')+1?'^':'') + (
              match[10] ? decode(match[10],'input',false) :
              decode(match[11].replace(/''/g,"'"),'input',true).replace(/([.^$*+?()[{\\|])/g,"\\$1")
            ) + (match[12].search('e')+1?'$':''),
            match[12].search('i')+1?'i':''
          );
          this.endLineNum=void 0;
          if (this.endOffset<-1) throw new Error(212, 'End-range Regex/String offset cannot be less than -1');
        }
      } else {
         this.endType=void 0;
         this.endSpec=void 0;
         this.endLineNum=this.lineNum;
      }
    }
    _g.setBlocks = function(blocks,str) {
      if (blocks.dynamic==true) {
        for (var i=0; i<blocks.length; i++) {
          var block = blocks[i];
          if (ln>block.endLineNum && block.type=='regex' && !block.spec.singleton)
            block.lineNum=block.endLineNum=void 0;
          if (!block.lineNum && block.spec.test(str)) {
            block.lineNum = ln+block.offset;
            if (!block.endLineNum) {
              if (!block.endType)
                block.endLineNum=block.lineNum;
              else if (block.endType=='offset')
                block.endLineNum=block.lineNum+block.endSpec+block.endOffset;
            }
          }
          if (!block.endLineNum && ln>block.lineNum && block.endSpec.test(str))
            block.endLineNum = ln+block.endOffset;
        }
      }
    }
    var str = _g.readVar( env('/INC'), env('/V'), '.INC' );
    while ( (blockMatch=blockSearch.exec(str)) !== null ) {
      _g.loc=' while parsing /INC block['+_g.incBlock.length+']';
      var block = new _g.Block(blockMatch);
      _g.incBlock.dynamic=(_g.incBlock.dynamic || block.type=='regex' || block.endType=='regex');
      _g.incBlock.push(block);
    }
    str = _g.readVar( env('/EXC'), env('/V'), '.EXC' );
    while ( (blockMatch=blockSearch.exec(str)) !== null ) {
      _g.loc=' while parsing /EXC block['+_g.excBlock.length+']';
      var block = new _g.Block(blockMatch);
      _g.excBlock.dynamic=(_g.excBlock.dynamic || block.type=='regex' || block.endType=='regex');
      _g.excBlock.push(new _g.Block(blockMatch));
    }
    _g.loc='';

    if (multi) options+='m';
    if (env('/MATCH')) replace='$txt=$0';
    if (_g.begLn) _g.defineObject("_g.begLn=function($txt){_g.loc=' in /JBEGLN code';"+_g.begLn+";_g.loc='';return $txt;}",'/JBEGLN code');
    if (_g.endLn) _g.defineObject("_g.endLn=function($txt){_g.loc=' in /JENDLN code';"+_g.endLn+";_g.loc='';return $txt;}",'/JENDLN code');
    if (env('/I')) options+='i';

    var lnWidth=parseInt(env('/N'),10),
        offWidth=parseInt(env('/OFF'),10),
        lnPad=lnWidth>0?Array(lnWidth+1).join('0'):'',
        offPad=offWidth>0?Array(offWidth+1).join('0'):'',
        xcnt=0, test;
    if (krfile) {                    // KEEP or REJECT File
      _g.loc=' loading '+(keep?'/K':'/R')+' Search file';
      var f = _g.openInput(search);
      search='';
      while (!f.AtEndOfStream) {
        str=f.ReadLine();
        if (env('/XSEQ')) str=decode(str,'input',literal);
        if (literal) str=str.replace(/([.^$*+?()[{\\|])/g,"\\$1");
        if (env('/B')) str="^"+str;
        if (env('/E')) str=str+"$";
        search+=(search?'|':'')+str;
      }
      f.Close();
      search=_g.newRegExp(search,options);
      _g.loc='';
    } else if (translate=='none') {  // Normal
      if (env('/XSEQ')) {
        if (!jexpr) replace=decode(replace,'output');
        search=decode(search,'input',literal);
      }
      if (literal) {
        search=search.replace(/([.^$*+?()[{\\|])/g,"\\$1");
        if (!jexpr) replace=replace.replace(/\$/g,"$$$$");
      }
      if (env('/B')) search="^"+search;
      if (env('/E')) search=search+"$";
      _g.loc=' in Search regular expression';
      search=_g.newRegExp(search,options);
      _g.loc='';
      if (jexpr) {
        _g.loc=' in Search regular expression';
        test=_g.newRegExp('.|'+search,options);
        _g.loc='';
        'x'.replace(test,function(){xcnt=arguments.length-2; return '';});
        _g.replFunc='_g.replFunc=function($0';
        for (var i=1; i<xcnt; i++) _g.replFunc+=',$'+i;
        _g.replFunc+=',$off,$src){_g.loc=" in Replace code";';
        if (jquick||jmatchq) {
          _g.replFunc+='var $txt;'+replace+';';
          _g.replFunc+=
            jmatch ? '_g.writeMatch($txt,$off,\''+lnPad+'\',\''+offPad+'\');_g.loc="";return $0;}'
                   : '_g.loc="";return $txt;}';
        } else _g.replFunc+=
            jmatch ? '_g.writeMatch(eval(_g.replace),$off,\''+lnPad+'\',\''+offPad+'\');_g.loc="";return $0;}'
                   : '_g.rtn2=eval(_g.replace);_g.loc="";return _g.rtn2;}';
        _g.defineObject(_g.replFunc,'/J or /JMATCH code');
      }
      _g.replace=replace;
    } else {                         // /T
      if (translate.toLowerCase()=='file') {
        var f
        _g.loc=' loading /T Search file';
        f = _g.openInput(search);
        search=[];
        while (!f.AtEndOfStream) search[search.length]=f.ReadLine();
        f.Close();
        _g.loc=' loading /T Replace file';
        f = _g.openInput(replace);
        replace=[];
        while (!f.AtEndOfStream) replace[replace.length]=f.ReadLine();
        f.Close();
        _g.loc='';
      } else {
        if (translate.length>1) throw new Error(203, 'Invalid /T delimiter');
        if (translate.length==0 && env('/XSEQ')) {
          search=decode(search,'input',literal);
          replace=decode(replace,'output');
        }
        search=search.split(translate);
        var replace=replace.split(translate);
      }
      if (search.length>99 && !_g.XRegExp) throw new Error(202, '/T expression count exceeds 99');
      if (search.length!=replace.length) throw new Error(201, 'Mismatched search and replace /T expressions');
      var j=1;
      if (!jexpr) jquick=1;
      if (jquick) _g.replace='';
      else _g.replace=[];
      for (var i=0; i<search.length; i++) {
        if (env('/XSEQ')) search[i]=decode(search[i],'input',literal);
        if (literal) {
          search[i]=search[i].replace(/([.^$*+?()[{\\|])/g,"\\$1");
        } else {
          _g.loc=' in Search regular expression';
          test=_g.newRegExp('.|'+search[i],options+(_g.XRegExp?env('/TFLAG'):''));
          _g.loc='';
          'x'.replace(test,function(){xcnt=arguments.length-3;return '';});
        }
        if (j+xcnt>99 && !_g.XRegExp) throw new Error(202, '/T expressions + captured expressions exceeds 99');
        if (env('/B')) search[i]="^"+search[i];
        if (env('/E')) search[i]=search[i]+"$";
        if (_g.XRegExp) search[i]="?<T"+i+">"+search[i];
        if (jquick|jmatchq) {
          if (!jexpr) {
            replace[i]="'" + (env('/XSEQ')==''?replace[i]:decode(replace[i],'output')).replace(/[\\']/g,"\\$&") + "'";
            replace[i]=replace[i].replace(/\n/g, "\\n");
            replace[i]=replace[i].replace(/\r/g, "\\r");
            if (!literal) {
              if (_g.XRegExp) {
                replace[i]='$txt='+replace[i].replace(
                  /\$([$&`0]|\\'|\{0\}|(\d)(\d)?|\{((\d)(\d)?)\}|\{([^}]+)\})/g,
                  function($0,$1,$2,$3,$4,$5,$6,$7){
                    return ($1=="$") ? "$":
                           ($1=="&" || $1=="0" || $1=="{0}") ? "'+$0+'":
                           ($1=="`") ? "'+$src.substr(0,$off)+'":
                           ($1=="\\'") ? "'+$src.substr($off+$0.length)+'":
                           ($7) ? "'+$0."+$7+"+'":
                           (Number($1)-j<=xcnt && Number($1)>=j) ? "'+"+$0+"+'":
                           (Number($2)-j<=xcnt && Number($2)>=j) ? "'+$"+$2+"+'"+$3:
                           (Number($4)-j<=xcnt && Number($4)>=j) ? "'+$"+$4+"+'":
                           (Number($5)-j<=xcnt && Number($5)>=j) ? "'+$"+$5+"+'"+$6:
                           $0;
                  }
                );
              } else {
                replace[i]='$txt='+replace[i].replace(
                  /\$([$&`0]|\\'|(\d)(\d)?)/g,
                  function($0,$1,$2,$3){
                    return ($1=="$") ? "$":
                           ($1=="&") ? "'+$0+'":
                           ($1=="`") ? "'+$src.substr(0,$off)+'":
                           ($1=="\\'") ? "'+$src.substr($off+$0.length)+'":
                           (Number($1)-j<=xcnt && Number($1)>=j) ? "'+"+$0+"+'":
                           (Number($2)-j<=xcnt && Number($2)>=j) ? "'+$"+$2+"+'"+$3:
                           $0;
                  }
                );
              }
            } else replace[i]='$txt='+replace[i];
          }
          _g.replace+='if(arguments['+j+']!==undefined){'+replace[i]+';}';
        } else {
          _g.replace[j]=replace[i];
        }
        j+=xcnt+1;
      }
      search='('+search.join(')|(')+')';
      _g.loc=' in Search regular expression';
      search=_g.newRegExp( search, options+(_g.XRegExp?env('/TFLAG'):'') );
      _g.loc='';
      _g.replFunc='_g.replFunc=function($0';
      for (var i=1; i<j; i++) _g.replFunc+=',$'+i;
      _g.replFunc+=',$off,$src){_g.loc=" in Replace code";';
      if (jquick||jmatchq) {
        _g.replFunc+='var $txt;'+_g.replace+ (
           jmatch ? '_g.writeMatch($txt,$off,\''+lnPad+'\',\''+offPad+'\');_g.loc="";return $0;}'
                  : '_g.loc="";return $txt;}' );
      } else {
        _g.replFunc+='for(_g.i=1;_g.i<arguments.length-2;_g.i++)if(arguments[_g.i]!==undefined)'+ (
           jmatch ? '{_g.writeMatch(eval(_g.replace[_g.i]),$off,\''+lnPad+'\',\''+offPad+'\');_g.loc="";return $0;}}'
                  : '{_g.rtn2=eval(_g.replace[_g.i]);_g.loc="";return _g.rtn2;}}' );
      }
      _g.defineObject(_g.replFunc,'/J or /JMATCH code');
      jexpr=true;
    }

    var str1, str2;
    var repl=jexpr?_g.replFunc:_g.replace;
    if (filter!='') {
      _g.loc=' in /P FilterRegex';
      filter = _g.newRegExp( decode(filter,'input',false), env('/PFLAG').toLowerCase()+(env('/M')?'m':'') );
      _g.loc='';
      if (!keep&&!reject) {
        _g.search=search;
        search=filter;
        _g.filterReplace2=repl;
        repl=_g.filterReplace;
      }
    }
    if (srcVar) {
      str1=_g.readVar( srcVar, srcVar, '.S' );
      str2=str1.replace(search,repl);
      if (str1!=str2) _g.rtn=0;
      if (!jmatch && (!alterations || str1!=str2)) output.Write(str2+(multi?'':_g.term));
    } else if (multi){
      var buf=1024;
      str1="";
      while (!input.AtEndOfStream) {
        str1+=input.Read(buf);
        buf*=2;
      }
      str2=str1.replace(search,repl);
      if (!jmatch) output.Write(str2);
      if (str1!=str2) _g.rtn=0;
    } else if (keep||reject){
      var match, arr, filterResult, post, pre=new Array();
      var cmd='while(!input.AtEndOfStream&&!quit){match=reject;str1=input.ReadLine();';
      if ( _g.incBlock.length || _g.excBlock.length || lnWidth
           || _g.begLn || _g.endLn || env(env('/V')?env('/JEND'):'/JEND')
         ) cmd+='ln++;';
      if (_g.incBlock.dynamic) cmd+='_g.setBlocks(_g.incBlock,str1);';
      if (_g.excBlock.dynamic) cmd+='_g.setBlocks(_g.excBlock,str1);';
      if (_g.begLn) cmd+='str1=_g.begLn(str1);';
      str1='';str2='if(';
      if (_g.incBlock.length) {str1+=str2+'inc()';str2='&&';}
      if (_g.excBlock.length) {str1+=str2+'!exc()';str2='&&';}
      if (_g.begLn||_g.endLn||jexpr||env(env('/V')?env('/JBEG'):'/JBEG')) {str1+=str2+'!skip';}
      if (str1) cmd+=str1+')';
      if (!filter) {
        cmd+='if ((arr=search.exec(str1))!==null) match=!reject;';
      } else if (!filter.global) {
        cmd+='if ((filterResult=filter.exec(str1))!==null && (arr=search.exec(filterResult[0])!==null)) match=!reject;';
      } else {
        cmd+='{filter.lastIndex=0;';
        cmd+='while (match==reject && (filterResult=filter.exec(str1))!==null)';
        cmd+='if ((arr=search.exec(filterResult[0]))!==null) match=!reject;}';
      }
      if (_g.endLn) cmd += 'str1=_g.endLn(str1);';
      cmd+='if (str1!==false && match) {';
      if (context[0]){
        cmd+='while (pre.length) output.Write(';
        if (lnWidth) cmd+='_g.fmtNum(ln-pre.length,lnPad)+';
        if (keep&&offWidth) cmd+='lpad("",offPad.length)+_g.delim+';
        cmd+='pre.pop()+_g.term);';
      }
      cmd+='output.Write(';
      if (lnWidth) cmd+='_g.fmtNum(ln,lnPad)+';
      if (keep&&offWidth) cmd+='_g.fmtNum(arr.index,offPad)+';
      cmd+='str1+_g.term);_g.rtn=0;';
      if (context[1]) {
        cmd+='post=context[1];}else if(post>0){output.Write(';
        if (lnWidth) cmd+='_g.fmtNum(ln,lnPad)+';
        if (keep&&offWidth) cmd+='lpad("",offPad.length)+_g.delim+';
        cmd+='(str1?str1:"")+_g.term);post--;';
      }
      if (context[0]) cmd+='}else{pre.unshift(str1?str1:"");if(pre.length>context[0])pre.pop();';
      cmd+='}}';
      eval(cmd);
    } else {
      var cmd='while(!input.AtEndOfStream&&!quit){str2=str1=input.ReadLine();';
      if ( _g.incBlock.length || _g.excBlock.length || lnWidth
           || _g.begLn || _g.endLn|| jexpr || env(env('/V')?env('/JEND'):'/JEND')
         ) cmd+='ln++;';
      if (_g.incBlock.dynamic) cmd+='_g.setBlocks(_g.incBlock,str2);';
      if (_g.excBlock.dynamic) cmd+='_g.setBlocks(_g.excBlock,str2);';
      if (_g.begLn) cmd+='str2=_g.begLn(str2);';
      str1='';str2='if(';
      if (_g.incBlock.length) {str1+=str2+'inc()';str2='&&';}
      if (_g.excBlock.length) {str1+=str2+'!exc()';str2='&&';}
      if (_g.begLn||_g.endLn||jexpr||env(env('/V')?env('/JBEG'):'/JBEG')) {str1+=str2+'!skip';}
      if (str1) cmd+=str1+')';
      cmd+='str2=str2.replace(search,repl);';
      if (_g.endLn) cmd+='str2=_g.endLn(str2);';
      if (!jmatch) {
        str1='';str2='if(';
        if (_g.begLn||jexpr) {str1+=str2+'str2!==false';str2='&&';}
        if (alterations) {str1+=str2+'str1!=str2';}
        if (str1) cmd+=str1+')';
        cmd+='output.Write('+(lnWidth>0?'_g.fmtNum(ln,lnPad)+':'')+'str2+_g.term);';
        cmd+='if (str1!=str2) _g.rtn=0;';
      }
      cmd+='}'
      eval(cmd);
    }
  }

  _g.main();

  _g.loc=' in /JEND code';
  eval( _g.readVar( env('/JEND'), env('/V'), '.JEND' ) );
  _g.loc='';
  if (_g.inFile) input.Close();
  if (_g.outFile) output.Close();
  if (_g.outFileA[0]=='-') {
    fso.GetFile(_g.inFileA[0]).Delete();
    fso.GetFile(_g.inFileA[0]+'.new').Move(_g.inFileA[0]);
  }
  if (_g.tempFile) fso.GetFile(_g.tempFile).Delete();


  if (env('/RTN')) {
    _g.rtnVar = function() {
      var val, str1, str2, buf=1024, arr, n;
      input = fso.OpenTextFile(_g.outFile,_g.ForReading);
      val='';
      while (!input.AtEndOfStream) {
        val+=input.Read(buf);
        buf*=2;
      }
      input.Close();
      if (env('/RTN_LINE')&&(n=parseInt(env('/RTN_LINE')))) {
        arr=val.split(/\r?\n/);
        n = n>0 ? n-1 : arr.length+n;
        val = typeof arr[n]==='undefined' ? '' : arr[n];
      } else if ((env('/MATCH')||env('/JMATCH')||env('/JMATCHQ'))&&val.slice(-_g.term.length)===_g.term){
        val=val.slice(0,-_g.term.length);
      }
      output = fso.OpenTextFile(_g.outFile,_g.ForWriting,true);
      str1='x'+val.replace(/%/g,'%3').replace(/\n/mg,'%~1').replace(/\r/mg,'%2').replace(/"/g,'%4');
      str2=str1.replace(/[!^]/g,'^$&');
      if (str2.length + env('/RTN').length > 8181) throw new Error(213, 'Result too long to fit within variable');
      if (str2.indexOf('\x00')>=0) throw new Error(214, 'Null bytes (0x00) cannot be returned in a variable');
      output.WriteLine(str1);
      output.WriteLine(str2);
      output.Close();
    }
    _g.rtnVar();
  }

  WScript.Quit(_g.rtn);
} catch(e) {
  WScript.Stderr.WriteLine("JScript runtime error"+_g.loc+": "+e.message);
  WScript.Quit(3);
}

function lpad( val, arg2, arg3 ) {
  var rtn=val.toString(), len, pad, cnt;
  if (typeof arg2 === "string") {
    pad = arg2;
    len = arg2.length;
  } else {
    len = arg2;
    pad = arg3 ? arg3 : '                                                  ';
    while (pad.length < len) pad+=pad;
  }
  return (rtn.length<len) ? pad.slice(0,len-rtn.length)+rtn : rtn;
}

function rpad( val, arg2, arg3 ) {
  var rtn=val.toString(), len, pad, cnt;
  if (typeof arg2 === "string") {
    pad = arg2;
    len = arg2.length;
  } else {
    len = arg2;
    pad = typeof arg3 === "string" ? arg3 : '                                                  ';
    while (pad.length < len) pad+=pad;
  }
  return (rtn.length<pad.length) ? rtn+pad.slice(rtn.length-len) : rtn;
}

function inc(n) {
  for (var i=n?n:0, end=n?n+1:_g.incBlock.length; i<end; i++) {
    var block = _g.incBlock[i];
    if (ln>=block.lineNum && ln<=(block.endLineNum?block.endLineNum:ln)) return true;
  }
  return (_g.incBlock.length==0);
}

function exc(n) {
  for (var i=n?n:0, end=n?n+1:_g.excBlock.length; i<end; i++) {
    var block = _g.excBlock[i];
    if (ln>=block.lineNum && ln<=(block.endLineNum?block.endLineNum:ln)) return true;
  }
  return false;
}

function openOutput( fileName, append, utf ) {
  _g.loc=' opening output file';
  if (output && output!==stdout) output.Close();
  if (fileName) {
    var file = fileName.split('|');
    if (file[1]) output=new _g.ADOStream( file[0], append?_g.ForAppending:_g.ForWriting, file[1], file[2] );
    else output=fso.OpenTextFile( fileName, append?_g.ForAppending:_g.ForWriting, true, utf?-1:0 );
  }
  else output=stdout;
  _g.loc='';
}

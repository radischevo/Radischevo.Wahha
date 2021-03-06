﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public class HtmlTypographer
	{
		#region Instance Fields
		private HtmlTypographerSettings _settings;
		private HtmlElementFormatter _formatter;
		private HtmlTypographerState _state;
        private StringBuffer _input;
        private StringBuilder _output;
        #endregion

        #region Constructors
		public HtmlTypographer(string input)
			: this(input, null, null)
		{
		}

		public HtmlTypographer(string input,
			HtmlTypographerSettings settings)
			: this(input, settings, null)
		{
		}

        public HtmlTypographer(string input, 
			HtmlTypographerSettings settings, 
			HtmlTypographerState state)
        {
			Precondition.Require(input, () => Error.ArgumentNull("input"));
			
			_input = new StringBuffer(input);
			_output = new StringBuilder();
			_state = state ?? new HtmlTypographerState();
			_settings = settings ?? new HtmlTypographerSettings();
        }
        #endregion

        #region Instance Properties
        protected StringBuilder Output
        {
            get
            {
                return _output;
            }
        }

        protected StringBuffer Input
        {
            get
            {
                return _input;
            }
        }

		public HtmlTypographerState State
		{
			get
			{
				return _state;
			}
		}

		public HtmlTypographerSettings Settings
		{
			get
			{
				return _settings;
			}
		}

		public HtmlElementFormatter Formatter
		{
			get
			{
				return _formatter;
			}
			set
			{
				_formatter = value;
			}
		}
        #endregion

        #region Instance Methods
        public string Execute()
        {
            PreProcess();

            while (Input.Read())
                WriteCharacter(Input.Current);

            PostProcess();
            return Output.ToString();
        }

        protected virtual void PreProcess()
        {
        }

        protected virtual void Write(char ch)
        {
            _output.Append(ch);
        }

        protected virtual void Write(string str)
        {
            _output.Append(str);
        }

        protected virtual string FormatElement(HtmlElementBuilder element,
            HtmlElementRenderMode mode)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));

            if (Formatter == null)
                return element.ToString(mode);
            
			return Formatter(element, mode);
        }

        protected virtual void WriteCharacter(char ch)
        {
            switch (ch)
            {
                case '\n':
                    WriteNewLine(ch);
                    break;
                case '<':
                    WriteLessThan(ch);
                    break;
                case '>':
                    WriteGreaterThan(ch);
                    break;
                case '=':
                    WriteEqual(ch);
                    break;
                case '-':
                case '\u2013':
                case '\u2014':
                case '\u2010':
                    WriteDash(ch);
                    break;
                case '\'':
                case '\u2018':
                case '\u2019':
                    WriteSingleQuote(ch);
                    break;
                case '\"':
                case '\u00ab':
                case '\u201e':
                case '\u00bb':
                case '\u201c':
                    WriteDoubleQuote(ch);
                    break;
                case ' ':
                case '\u00a0':
                    WriteWhitespace(ch);
                    break;
                case ',':
                case ';':
                case ':':
                case '.':
                case '!':
                case '?':
                case '\u2026':
                    ProcessPunctuation(ch);
                    break;
                case '&':
                    WriteAmpersand(ch);
                    break;
                default:
                    WriteCharacterWithCheck(ch);
                    break;
            }
        }

        protected virtual void WritePunctuation(char ch)
        {
            switch (ch)
            {
                case '\u2026':
                    Write(ch);
                    while (_input.Read('.')) ;
                    break;
                case '.':
                    WriteEllipsis(ch);
                    break;
                case '!':
                    WriteExclamation(ch);
                    break;
                case '?':
                    WriteQuestion(ch);
                    break;
                default:
                    Write(ch);
                    break;
            }
        }

        protected virtual void PostProcess()
        {
            CloseNoBreak();
            ApplyNonRegexReplacements();
            ApplyRegexReplacements();
            RewriteOutputAsUnicode();
        }

        private void RewriteOutputAsUnicode()
        {
            if (!Settings.EncodeSpecialSymbols)
                return;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _output.Length; ++i)
            {
                if (_output[i] >= '\u00A0' && _output[i] < '\u0410' ||
                    _output[i] > '\u0451')
                {
                    sb.Append("&#");
                    sb.Append(((int)_output[i]));
                    sb.Append(';');
                }
                else
                    sb.Append(_output[i]);
            }
            _output = sb;
        }

        private void ApplyNonRegexReplacements()
        {
            foreach (StringReplacementRule rule in Settings.Replacements
				.Where(r => r.Mode == StringReplacementMode.Default))
                _output.Replace(rule.Pattern, rule.Replacement);
        }

        private void ApplyRegexReplacements()
        {
            List<StringReplacementRule> replacements = Settings.Replacements
                .Where(r => r.Mode == StringReplacementMode.Regex).ToList();

            if (replacements.Count < 1)
                return;

            string str = _output.ToString();
            foreach (StringReplacementRule rule in replacements)
                str = Regex.Replace(str, rule.Pattern, rule.Replacement, rule.Options);

            _output.Length = 0;
            _output.Append(str);
        }

        private void CloseNoBreak()
        {
            if (State.WithinNoBreak)
            {
                State.WithinNoBreak = false;
                Write(FormatElement(new HtmlElementBuilder("nobr"), 
                    HtmlElementRenderMode.EndTag));
            }
        }

        private void CloseQuotes()
        {
            if (State.Quotes.Count > 0)
            {
                int index = _output.Length;
                while (Char.IsWhiteSpace(_output[--index]) ||
                    ".,:!?".IndexOf(_output[index]) > -1) ;

                index++;
                while (State.Quotes.Count > 0)
                {
                    switch (State.Quotes.Pop())
                    {
                        case '\u00ab':
                            _output.Insert(index, '\u00bb');
                            index += 7;
                            break;
                        case '\u201e':
                            _output.Insert(index, '\u201c');
                            index += 7;
                            break;
                        case '\'':
                            _output.Insert(index, '\u2019');
                            index += 7;
                            break;
                    }
                }
            }
        }

        private void WriteCharacterWithCheck(char ch)
        {
            if (Settings.ExtractLinks && Char.IsLetter(ch) && (
                _input.Match("http://") || _input.Match("https://") ||
                _input.Match("ftp://") || _input.Match("mailto:") ||
                _input.Match("www.")))
            {
                StringBuilder lb = new StringBuilder();
                string url;

                // едим все до пробела, или до конца строки
                do
                {
                    if (Char.IsWhiteSpace(_input.Current))
                    {
                        _input.Position--;
                        break;
                    }
                    lb.Append(_input.Current);
                }
                while (_input.Read());

                // отдаем обратно не буквы-цифры и слеши
                while (lb.Length > 0 && Char.IsPunctuation(lb[lb.Length - 1]))
                {
                    lb.Length--;
                    _input.Position--;
                }

                url = (lb[0] == 'w') ? "http://" + lb.ToString() : lb.ToString();
                Uri uri;
				if (Uri.TryCreate(url, UriKind.Absolute, out uri))
				{
					if (String.Equals(uri.Scheme, "mailto", StringComparison.OrdinalIgnoreCase))
						lb.Remove(0, 7);

					HtmlElementBuilder tag = new HtmlElementBuilder("a");
					tag.Attributes["href"] = uri.OriginalString;
					tag.InnerHtml = SecurityElement.Escape(lb.ToString());

					Write(FormatElement(tag, HtmlElementRenderMode.Normal));
				}
				else
					_output.Append(lb.ToString());
            }
            else if (Char.IsWhiteSpace(ch))
            {
                CloseNoBreak();
                Write(ch);
            }
            else
                Write(ch);
        }

		private void ProcessPunctuation(char ch)
		{
			if (Settings.ApplyPunctuationRules)
				ProcessPunctuationWithRules(ch);
			else
				Write(ch);
		}

        private void ProcessPunctuationWithRules(char ch)
        {
			if (Char.IsWhiteSpace(_input.Offset(1)) || Char.IsLetterOrDigit(_input.Offset(1)))
                while (_output.Length > 0 && Char.IsWhiteSpace(_output[_output.Length - 1]))
                    _output.Length--;

            CloseNoBreak();
            WritePunctuation(ch);

            if (Char.IsLetter(_input.Offset(1)))
                Write(' ');
        }

        private void WriteWhitespace(char ch)
        {
            CloseNoBreak();

            int length = 0;
            int index = _output.Length - 1;

            while (index > 0 && Char.IsLetterOrDigit(_output[index--])) 
                length++;

            while (_input.Read(ch)) ;

            if (length > 0 && length < 4 &&
                Char.IsLetterOrDigit(_input.Offset(1)))
                Write('\u00a0');
            else
                Write(ch);
        }

        private void WriteNewLine(char ch)
        {
            if (_output.Length > 0 && _output[_output.Length - 1] == '\r')
                _output.Length--;

            if (_output.Length > 0 && Settings.AutoLineBreak)
                Write(FormatElement(new HtmlElementBuilder("br"), 
                    HtmlElementRenderMode.Normal));

            Write(Environment.NewLine);
        }

        private void WriteGreaterThan(char ch)
        {
            if (_input.Offset(1) == '=')
            {
                Write('\u2265');
                _input.Position++;
            }
            else
                Write("&#62;");
        }

        private void WriteLessThan(char ch)
        {
            if (_input.Offset(1, 3) == "-->")
            {
                Write('\u2194');
                _input.Position += 3;
            }
            else if (_input.Offset(1, 2) == "--")
            {
                Write('\u2190');
                _input.Position += 2;
            }
            else if (_input.Offset(1, 3) == "==>")
            {
                Write('\u21d4');
                _input.Position += 3;
            }
            else if (_input.Offset(1, 2) == "==")
            {
                Write('\u21d0');
                _input.Position += 2;
            }
            else if (_input.Offset(1) == '=')
            {
                Write('\u2264');
                _input.Position++;
            }
            else if (_input.Offset(1) == '>')
            {
                Write('\u2260');
                _input.Position++;
            }
            else
                Write("&#60;");
        }

        private void WriteEqual(char ch)
        {
            if (_input.Offset(1, 2) == "=>")
            {
                Write('\u21d2');
                _input.Position += 2;
            }
            else
                Write(ch);
        }

        private void WriteAmpersand(char ch)
        {
            if (_input.Offset(1) == '<' ||
                Char.IsWhiteSpace(_input.Offset(1)))
                Write("&amp;");
            else
                Write('&');
        }

        private void WriteEllipsis(char ch)
        {
            if (_input.Offset(1) == '.')
            {
                if (_input.Offset(2) == '.')
                {
                    Write('\u2026');
                    _input.Position += 2;
                }
                while (_input.Read('.')) ;
            }
            else
                Write(ch);
        }

        private void WriteExclamation(char ch)
        {
            int maxSymbolCount = 3;
            int symbolCount = 0;
            if (_output.Length > 0)
            {
                switch (_output[_output.Length - 1])
                {
                    case '!':
                        maxSymbolCount = 0;
                        break;
                    case '?':
                        maxSymbolCount = 2;
                        break;
                }
            }
            do
            {
                if (symbolCount++ < maxSymbolCount)
                    Write(ch);
            }
            while (_input.Read('!'));
        }

        private void WriteQuestion(char ch)
        {
            Write(ch);
            while (_input.Read(ch)) ;
        }

        private void WriteDoubleQuote(char ch)
        {
            if (State.Quotes.Count > 0 &&
               (Char.IsLetterOrDigit(_input.Offset(-1)) ||
                 (_input.Offset(1) == StringBuffer.EOF || 
                   Char.IsPunctuation(_input.Offset(1)) || 
                   Char.IsWhiteSpace(_input.Offset(1)))
               )) // надо закрываться
            {
                if (State.Quotes.Peek() == '\u00ab')
                    Write('\u00bb');
                else if (State.Quotes.Peek() == '\u201e')
                    Write('\u201c');

                State.Quotes.Pop();
            }
            else if (Char.IsLetterOrDigit(_input.Offset(1))) // надо открываться
            {
                if (State.Quotes.Count > 0) // уже что-то было
                {
                    Write('\u201e');
                    State.Quotes.Push('\u201e');
                }
                else // первая открывающая
                {
					Write('\u00ab');
                    State.Quotes.Push('\u00ab');
                }
            }
        }

        private void WriteSingleQuote(char ch)
        {
            if (State.Quotes.Count > 0 &&
               (Char.IsLetterOrDigit(_input.Offset(-1)) ||
                 (_input.Offset(1) == StringBuffer.EOF || 
                  Char.IsPunctuation(_input.Offset(1)) || 
                  Char.IsWhiteSpace(_input.Offset(1)))
               )) // надо закрываться
            {
                if (State.Quotes.Peek() == '\'')
                    _output.Append('\u2019');

                State.Quotes.Pop();
            }
            else if (Char.IsLetterOrDigit(_input.Offset(1))) // надо открываться
            {
                _output.Append('\u2018');
                State.Quotes.Push('\'');
            }
        }

        private void WriteDash(char ch)
        {
            if (Char.IsWhiteSpace(_input.Offset(-1)) &&
                Char.IsWhiteSpace(_input.Offset(1))) // any - any = ?
            {
                if (Char.IsDigit(_input.Offset(-2)) &&
                    Char.IsDigit(_input.Offset(2))) // 1 - 1 = minus sign
                    Write('\u2212');
                else
                    Write('\u2013'); // en dash
            }
            else if (Char.IsDigit(_input.Offset(-1)) &&
                Char.IsDigit(_input.Offset(1))) // 1-1 = minus
            {
                Write('\u2212');
            }
            else if (_input.Offset(1) == '-') // any-??
            {
                if (_input.Offset(2) == '>') // any--> = right arrow
                {
                    Write('\u2192');
                    _input.Position += 2;
                }
                else
                {
                    Write('\u2014'); // any--any = em dash
                    _input.Position++;
                }
            }
            else if (Char.IsLetter(_input.Offset(-1)) &&
                Char.IsLetter(_input.Offset(1))
                && Settings.InsertNoBreakTags && !State.WithinNoBreak) // дефис
            {
                int index = _output.Length;
                while (index > 0 && !Char.IsWhiteSpace(_output[--index])) ;

                _output.Insert((index > 0) ? index + 1 : 0, 
					FormatElement(new HtmlElementBuilder("nobr"), 
                    HtmlElementRenderMode.StartTag));

                State.WithinNoBreak = true;
                Write('\u2010');
            }
            else
                Write(ch);
        }
        #endregion
    }
}

using System.Globalization;

namespace Calculator;

/// <summary>
/// Evaluates arithmetic expressions using a recursive-descent parser.
///
/// Grammar (lowest to highest precedence):
///   expression := term (('+' | '-') term)*
///   term       := unary (('*' | '/' | '%') unary)*
///   unary      := ('+' | '-') unary | power
///   power      := primary ('^' unary)?        (right-associative)
///   primary    := number | '(' expression ')'
/// </summary>
public sealed class Evaluator
{
    private string _text = "";
    private int _pos;

    public double Evaluate(string expression)
    {
        _text = expression;
        _pos = 0;

        double result = ParseExpression();
        SkipWhitespace();
        if (_pos < _text.Length)
            throw new FormatException($"Unexpected character '{_text[_pos]}' at position {_pos + 1}.");

        return result;
    }

    private double ParseExpression()
    {
        double value = ParseTerm();
        while (true)
        {
            if (Match('+')) value += ParseTerm();
            else if (Match('-')) value -= ParseTerm();
            else return value;
        }
    }

    private double ParseTerm()
    {
        double value = ParseUnary();
        while (true)
        {
            if (Match('*'))
            {
                value *= ParseUnary();
            }
            else if (Match('/'))
            {
                double divisor = ParseUnary();
                if (divisor == 0) throw new DivideByZeroException("Division by zero.");
                value /= divisor;
            }
            else if (Match('%'))
            {
                double divisor = ParseUnary();
                if (divisor == 0) throw new DivideByZeroException("Modulo by zero.");
                value %= divisor;
            }
            else
            {
                return value;
            }
        }
    }

    private double ParseUnary()
    {
        if (Match('+')) return ParseUnary();
        if (Match('-')) return -ParseUnary();
        return ParsePower();
    }

    private double ParsePower()
    {
        double value = ParsePrimary();
        if (Match('^')) value = Math.Pow(value, ParseUnary());
        return value;
    }

    private double ParsePrimary()
    {
        if (Match('('))
        {
            double value = ParseExpression();
            if (!Match(')')) throw new FormatException("Missing closing parenthesis.");
            return value;
        }

        SkipWhitespace();
        int start = _pos;
        while (_pos < _text.Length && (char.IsDigit(_text[_pos]) || _text[_pos] == '.'))
            _pos++;

        if (start == _pos)
        {
            throw _pos < _text.Length
                ? new FormatException($"Unexpected character '{_text[_pos]}' at position {_pos + 1}.")
                : new FormatException("Unexpected end of expression.");
        }

        string number = _text[start.._pos];
        if (!double.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result))
            throw new FormatException($"Invalid number '{number}'.");

        return result;
    }

    private bool Match(char expected)
    {
        SkipWhitespace();
        if (_pos < _text.Length && _text[_pos] == expected)
        {
            _pos++;
            return true;
        }
        return false;
    }

    private void SkipWhitespace()
    {
        while (_pos < _text.Length && char.IsWhiteSpace(_text[_pos]))
            _pos++;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopus.Client
{
    /// <summary>
    /// Modified implementation of the URI Template Spec RFC6570 for use in the Octopus Deploy RESTful API client.
    /// </summary>
    /// <remarks>
    /// This implementation is from: https://github.com/darrelmiller/UriTemplates. The class was renamed so as not to
    /// conflict with
    /// the UriTemplate class built into .NET, and the static
    /// <see
    ///     cref="Resolve(string,System.Collections.Generic.IDictionary{string,object})" />
    /// methods were added.
    /// </remarks>
    public class UrlTemplate
    {
        const string UriReservedSymbols = ":/?#[]@!$&'()*+,;=";
        const string UriUnreservedSymbols = "-._~";

        static readonly Dictionary<char, OperatorInfo> Operators = new Dictionary<char, OperatorInfo>
        {
            {
                '\0',
                new OperatorInfo
                {
                    Default = true,
                    First = "",
                    Seperator = ',',
                    Named = false,
                    IfEmpty = "",
                    AllowReserved = false
                }
            },
            {
                '+',
                new OperatorInfo
                {
                    Default = false,
                    First = "",
                    Seperator = ',',
                    Named = false,
                    IfEmpty = "",
                    AllowReserved = true
                }
            },
            {
                '.',
                new OperatorInfo
                {
                    Default = false,
                    First = ".",
                    Seperator = '.',
                    Named = false,
                    IfEmpty = "",
                    AllowReserved = false
                }
            },
            {
                '/',
                new OperatorInfo
                {
                    Default = false,
                    First = "/",
                    Seperator = '/',
                    Named = false,
                    IfEmpty = "",
                    AllowReserved = false
                }
            },
            {
                ';',
                new OperatorInfo
                {
                    Default = false,
                    First = ";",
                    Seperator = ';',
                    Named = true,
                    IfEmpty = "",
                    AllowReserved = false
                }
            },
            {
                '?',
                new OperatorInfo
                {
                    Default = false,
                    First = "?",
                    Seperator = '&',
                    Named = true,
                    IfEmpty = "=",
                    AllowReserved = false
                }
            },
            {
                '&',
                new OperatorInfo
                {
                    Default = false,
                    First = "&",
                    Seperator = '&',
                    Named = true,
                    IfEmpty = "=",
                    AllowReserved = false
                }
            },
            {
                '#',
                new OperatorInfo
                {
                    Default = false,
                    First = "#",
                    Seperator = ',',
                    Named = false,
                    IfEmpty = "",
                    AllowReserved = true
                }
            }
        };

        readonly Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        readonly string template;
        bool errorDetected;
        List<string> parameterNames;
        StringBuilder result;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlTemplate" /> class.
        /// </summary>
        /// <param name="template"> The template. </param>
        public UrlTemplate(string template)
        {
            this.template = template;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        public void SetParameter(string name, object value)
        {
            parameters[name] = value;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        public void SetParameter(string name, string value)
        {
            parameters[name] = value;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        public void SetParameter(string name, IEnumerable<string> value)
        {
            parameters[name] = value;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        public void SetParameter(string name, IDictionary<string, string> value)
        {
            parameters[name] = value;
        }

        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        /// <returns> </returns>
        public IEnumerable<string> GetParameterNames()
        {
            var parameterNames = new List<string>();
            this.parameterNames = parameterNames;
            Resolve();
            this.parameterNames = null;
            return parameterNames;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <returns> </returns>
        /// <exception cref="System.ArgumentException">
        /// Malformed template :  + result
        /// or
        /// Malformed template :  + result
        /// </exception>
        public string Resolve()
        {
            var currentState = States.CopyingLiterals;
            result = new StringBuilder();
            StringBuilder currentExpression = null;
            foreach (var character in template)
            {
                switch (currentState)
                {
                    case States.CopyingLiterals:
                        if (character == '{')
                        {
                            currentState = States.ParsingExpression;
                            currentExpression = new StringBuilder();
                        }
                        else
                        {
                            result.Append(character);
                        }
                        break;
                    case States.ParsingExpression:
                        if (character == '}')
                        {
                            ProcessExpression(currentExpression);

                            currentState = States.CopyingLiterals;
                        }
                        else
                        {
                            currentExpression.Append(character);
                        }

                        break;
                }
            }
            if (currentState == States.ParsingExpression)
            {
                result.Append("{");
                result.Append(currentExpression);

                throw new ArgumentException("Malformed template : " + result);
            }

            if (errorDetected)
            {
                throw new ArgumentException("Malformed template : " + result);
            }
            return result.ToString();
        }

        void ProcessExpression(StringBuilder currentExpression)
        {
            if (currentExpression.Length == 0)
            {
                errorDetected = true;
                result.Append("{}");
                return;
            }

            var op = GetOperator(currentExpression[0]);

            var firstChar = op.Default ? 0 : 1;

            var varSpec = new VarSpec(op);
            for (var i = firstChar; i < currentExpression.Length; i++)
            {
                var currentChar = currentExpression[i];
                switch (currentChar)
                {
                    case '*':
                        varSpec.Explode = true;
                        break;
                    case ':': // Parse Prefix Modifier
                        var prefixText = new StringBuilder();
                        currentChar = currentExpression[++i];
                        while (currentChar >= '0' && currentChar <= '9' && i < currentExpression.Length)
                        {
                            prefixText.Append(currentChar);
                            i++;
                            if (i < currentExpression.Length) currentChar = currentExpression[i];
                        }
                        varSpec.PrefixLength = int.Parse(prefixText.ToString());
                        i--;
                        break;
                    case ',':
                        var success = ProcessVariable(varSpec);
                        var isFirst = varSpec.First;
                        // Reset for new variable
                        varSpec = new VarSpec(op);
                        if (success || !isFirst) varSpec.First = false;

                        break;
                    default:
                        varSpec.VarName.Append(currentChar);
                        break;
                }
            }
            ProcessVariable(varSpec);
        }

        bool ProcessVariable(VarSpec varSpec)
        {
            var varname = varSpec.VarName.ToString();
            if (parameterNames != null) parameterNames.Add(varname);

            if (!parameters.ContainsKey(varname)
                || parameters[varname] == null
                || (parameters[varname] is IList && ((IList)parameters[varname]).Count == 0)
                || (parameters[varname] is IDictionary && ((IDictionary)parameters[varname]).Count == 0))
                return false;

            if (varSpec.First)
            {
                result.Append(varSpec.OperatorInfo.First);
            }
            else
            {
                result.Append(varSpec.OperatorInfo.Seperator);
            }

            var value = parameters[varname];

            // Handle Strings
            if (value is string)
            {
                var stringValue = (string)value;
                if (varSpec.OperatorInfo.Named)
                {
                    AppendName(varname, varSpec.OperatorInfo, string.IsNullOrEmpty(stringValue));
                }
                AppendValue(stringValue, varSpec.PrefixLength, varSpec.OperatorInfo.AllowReserved);
            }
            else
            {
                // Handle Lists
                var list = value as IEnumerable<string>;
                if (list != null)
                {
                    if (varSpec.OperatorInfo.Named && !varSpec.Explode) // exploding will prefix with list name
                    {
                        AppendName(varname, varSpec.OperatorInfo, list.Count() == 0);
                    }

                    AppendList(varSpec.OperatorInfo, varSpec.Explode, varname, list);
                }
                else
                {
                    // Handle associative arrays
                    var dictionary = value as IDictionary<string, string>;
                    if (dictionary != null)
                    {
                        if (varSpec.OperatorInfo.Named && !varSpec.Explode) // exploding will prefix with list name
                        {
                            AppendName(varname, varSpec.OperatorInfo, dictionary.Count() == 0);
                        }
                        AppendDictionary(varSpec.OperatorInfo, varSpec.Explode, dictionary);
                    }
                    else
                    {
                        var stringValue = value == null ? null : value.ToString();
                        if (varSpec.OperatorInfo.Named)
                        {
                            AppendName(varname, varSpec.OperatorInfo, string.IsNullOrEmpty(stringValue));
                        }
                        AppendValue(stringValue, varSpec.PrefixLength, varSpec.OperatorInfo.AllowReserved);
                    }
                }
            }
            return true;
        }

        void AppendDictionary(OperatorInfo op, bool explode, IDictionary<string, string> dictionary)
        {
            foreach (var key in dictionary.Keys)
            {
                result.Append(key);
                if (explode) result.Append('=');
                else result.Append(',');
                AppendValue(dictionary[key], 0, op.AllowReserved);

                if (explode)
                {
                    result.Append(op.Seperator);
                }
                else
                {
                    result.Append(',');
                }
            }
            if (dictionary.Count() > 0)
            {
                result.Remove(result.Length - 1, 1);
            }
        }

        void AppendList(OperatorInfo op, bool explode, string variable, IEnumerable<string> list)
        {
            foreach (var item in list)
            {
                if (op.Named && explode)
                {
                    result.Append(variable);
                    result.Append("=");
                }
                AppendValue(item, 0, op.AllowReserved);

                result.Append(explode ? op.Seperator : ',');
            }
            if (list.Count() > 0)
            {
                result.Remove(result.Length - 1, 1);
            }
        }

        void AppendValue(string value, int prefixLength, bool allowReserved)
        {
            if (prefixLength != 0)
            {
                if (prefixLength < value.Length)
                {
                    value = value.Substring(0, prefixLength);
                }
            }

            result.Append(Encode(value, allowReserved));
        }

        void AppendName(string variable, OperatorInfo op, bool valueIsEmpty)
        {
            result.Append(variable);
            if (valueIsEmpty)
            {
                result.Append(op.IfEmpty);
            }
            else
            {
                result.Append("=");
            }
        }

        static string Encode(string p, bool allowReserved)
        {
            var result = new StringBuilder();
            foreach (var c in p)
            {
                if ((c >= 'A' && c <= 'z') //Alpha
                    || (c >= '0' && c <= '9') // Digit
                    || UriUnreservedSymbols.IndexOf(c) != -1
                    // Unreserved symbols  - These should never be percent encoded
                    || (allowReserved && UriReservedSymbols.IndexOf(c) != -1))
                    // Reserved symbols - should be included if requested (+)
                {
                    result.Append(c);
                }
                else
                {
                    result.Append(Uri.HexEscape(c));
                }
            }

            return result.ToString();
        }

        static OperatorInfo GetOperator(char operatorIndicator)
        {
            OperatorInfo op;
            switch (operatorIndicator)
            {
                case '+':
                case ';':
                case '/':
                case '#':
                case '&':
                case '?':
                case '.':
                    op = Operators[operatorIndicator];
                    break;

                default:
                    op = Operators['\0'];
                    break;
            }
            return op;
        }

        /// <summary>
        /// Resolves the specified template spec.
        /// </summary>
        /// <param name="templateSpec"> The template spec. </param>
        /// <param name="parameters"> The parameters. </param>
        /// <returns> </returns>
        public static string Resolve(string templateSpec, object parameters)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (parameters != null)
            {
                var properties = parameters.GetType().GetProperties();
                foreach (var property in properties)
                {
                    dictionary[property.Name] = property.GetValue(parameters, null);
                }
            }

            return Resolve(templateSpec, dictionary);
        }

        /// <summary>
        /// Resolves the specified template spec.
        /// </summary>
        /// <param name="templateSpec"> The template spec. </param>
        /// <param name="parameters"> The parameters. </param>
        /// <returns> </returns>
        public static string Resolve(string templateSpec, IDictionary<string, object> parameters)
        {
            var template = new UrlTemplate(templateSpec);
            parameters = parameters ?? new Dictionary<string, object>();
            foreach (var param in parameters)
            {
                template.SetParameter(param.Key, param.Value);
            }

            return template.Resolve();
        }

        #region Nested type: OperatorInfo

        class OperatorInfo
        {
            public bool Default { get; set; }
            public string First { get; set; }
            public char Seperator { get; set; }
            public bool Named { get; set; }
            public string IfEmpty { get; set; }
            public bool AllowReserved { get; set; }
        }

        #endregion

        #region Nested type: States

        enum States
        {
            CopyingLiterals,
            ParsingExpression
        }

        #endregion

        #region Nested type: VarSpec

        class VarSpec
        {
            readonly OperatorInfo _operatorInfo;
            public readonly StringBuilder VarName = new StringBuilder();
            public bool Explode;
            public bool First = true;
            public int PrefixLength;

            public VarSpec(OperatorInfo operatorInfo)
            {
                _operatorInfo = operatorInfo;
            }

            public OperatorInfo OperatorInfo
            {
                get { return _operatorInfo; }
            }
        }

        #endregion
    }
}